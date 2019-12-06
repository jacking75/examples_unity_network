using System.Collections;
using System;
using System.Net.Sockets;
using System.Threading;


namespace NetLib
{
	public class TransportTCP
	{		
		// 클라이언트와의 접속용 소켓.
		private Socket TcpSocket = null;

		System.Collections.Concurrent.ConcurrentQueue<byte[]> SendQueue = new System.Collections.Concurrent.ConcurrentQueue<byte[]>();
		System.Collections.Concurrent.ConcurrentQueue<byte[]> RecvQueue = new System.Collections.Concurrent.ConcurrentQueue<byte[]>();

		PacketBufferManager PacketBuffer = new PacketBufferManager();

		// 접속 플래그.
		public bool IsConnected { get; private set; } = false;

		
		// 이벤트 통지 델리게이트.
		public delegate void EventHandler(NetEventState state);

		
		// 스레스 실행 플래그.
		protected bool IsRunThreadLoop = false;

		protected Thread ThreadHandle = null;

		private const int MtuSize = 1400;

		public System.Action<string> DebugPrintFunc;


		// Use this for initialization
		public void Start()
		{			
			PacketBuffer.Init(8096, PacketDef.PACKET_HEADER_SIZE, 1024);
		}

				
		// 접속.
		public bool Connect(string address, int port)
		{
			DebugPrintFunc("TransportTCP connect called.");

			bool ret = false;
			try
			{
				TcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				TcpSocket.NoDelay = true;
				TcpSocket.SendBufferSize = 0;
				TcpSocket.Connect(address, port);
				ret = LaunchThread();
			}
			catch
			{
				TcpSocket = null;
			}

			if (ret == true)
			{
				IsConnected = true;
				DebugPrintFunc("Connection success.");
			}
			else
			{
				IsConnected = false;
				DebugPrintFunc("Connect fail");
			}

			return IsConnected;
		}

		// 끊기.
		public void Disconnect()
		{
			IsConnected = false;

			if (TcpSocket != null)
			{
				IsRunThreadLoop = false;

				// 소켓 클로즈.
				TcpSocket.Shutdown(SocketShutdown.Both);
				TcpSocket.Close();
				TcpSocket = null;
			}						
		}

		// 송신처리.
		public void Send(byte[] data)
		{
			SendQueue.Enqueue(data);
		}

		// 수신처리.
		public bool Receive(out byte[] buffer)
		{
			return RecvQueue.TryDequeue(out buffer);
		}
				
		// 스레드 실행 함수.
		bool LaunchThread()
		{
			try
			{
				// Dispatch용 스레드 시작.
				IsRunThreadLoop = true;
				ThreadHandle = new Thread(new ThreadStart(Dispatch));
				ThreadHandle.Start();
			}
			catch
			{
				DebugPrintFunc("Cannot launch thread.");
				return false;
			}

			return true;
		}

		// 스레드 측의 송수신 처리.
		void Dispatch()
		{
			DebugPrintFunc("Dispatch thread started.");

			while (IsRunThreadLoop)
			{
				// 클라이언트와의 송수신을 처리합니다.
				if (TcpSocket != null && IsConnected == true)
				{

					// 송신처리.
					DispatchSend();

					// 수신처리.
					DispatchReceive();
				}

				Thread.Sleep(5);
			}

			DebugPrintFunc("Dispatch thread ended.");
		}
				
		// 스레드 측 송신처리 .
		void DispatchSend()
		{
			try
			{
				// 송신처리.
				if (TcpSocket.Poll(0, SelectMode.SelectWrite))
				{
					byte[] buffer = null;

					if( SendQueue.TryDequeue(out buffer) )
					{
						TcpSocket.Send(buffer, buffer.Length, SocketFlags.None);
					}					
				}
			}
			catch
			{
				return;
			}
		}

		// 스레드 측의 수신처리.
		void DispatchReceive()
		{
			// 수신처리.
			try
			{
				byte[] buffer = new byte[MtuSize];

				while (TcpSocket.Poll(0, SelectMode.SelectRead))
				{
					int recvSize = TcpSocket.Receive(buffer, buffer.Length, SocketFlags.None);
					if (recvSize == 0)
					{
						var closedBuffer = new byte[1];
						RecvQueue.Enqueue(buffer);

						DebugPrintFunc("Disconnected recv from client.");
						Disconnect();
					}
					else if (recvSize > 0)
					{
						var recvData = new byte[recvSize];
						Buffer.BlockCopy(buffer, 0, recvData, 0, recvSize);
						RecvQueue.Enqueue(recvData);
					}
				}
			}
			catch
			{
				return;
			}
		}


		// 애플리케이션 레이어에서 호출해야 한다. 스레드 세이프하지 않다.
		public PacketData GetPacket()
		{
			var packet = new PacketData();
			const Int16 PacketHeaderSize = PacketDef.PACKET_HEADER_SIZE;

			byte[] buffer = null;
			var result = Receive(out buffer);
			if (result == false)
			{
				return packet;
			}

			if (buffer.Length > 1)
			{
				PacketBuffer.Write(buffer, 0, buffer.Length);

				var data = PacketBuffer.Read();
				if (data.Count < 1)
				{
					return packet;
				}

				
				packet.DataSize = (UInt16)(data.Count - PacketHeaderSize);
				packet.PacketID = BitConverter.ToUInt16(data.Array, data.Offset + 2);
				packet.Type = (SByte)data.Array[(data.Offset + 4)];
				packet.BodyData = new byte[packet.DataSize];
				Buffer.BlockCopy(data.Array, (data.Offset + PacketHeaderSize), packet.BodyData, 0, (data.Count - PacketHeaderSize));

				return packet;
			}

			// 서버에서 접속을 종료하였음을 알린다.
			packet.PacketID = PacketDef.SysPacketIDDisConnectdFromServer;
			return packet;
		}
	}

	
}