using System.Collections;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace ClientNetLib
{
	public class TransportTCP
	{		
		// 클라이언트와의 접속용 소켓.
		private Socket TcpSocket = null;

		System.Collections.Concurrent.ConcurrentQueue<byte[]> SendQueue = new System.Collections.Concurrent.ConcurrentQueue<byte[]>();
		System.Collections.Concurrent.ConcurrentQueue<byte[]> RecvQueue = new System.Collections.Concurrent.ConcurrentQueue<byte[]>();

		PacketBufferManager PacketBuffer = null;

		// 접속 플래그.
		public bool IsConnected { get; private set; } = false;
						
		// 스레스 실행 플래그.
		protected bool IsRunThreadLoop = false;

		protected Thread ThreadHandle = null;

		private const int MtuSize = 1000;

		public System.Action<string> DebugPrintFunc;

								
		// 접속.
		public bool Connect(string address, int port)
		{
			DebugPrintFunc("TransportTCP connect called.");

			if (PacketBuffer == null)
			{
				PacketBuffer = new PacketBufferManager();
				PacketBuffer.Init((MtuSize*8), PacketDef.PACKET_HEADER_SIZE, MtuSize);
			}

			bool ret = false;
			try
			{
				TcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				//TcpSocket.NoDelay = true;
				//TcpSocket.SendBufferSize = 16800;
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


		// 애플리케이션 레이어에서 호출해야 한다. 메인 스레드에서 호출한다
		public List<PacketData> GetPacket()
		{
			var packetList = new List<PacketData>();
			const Int16 PacketHeaderSize = PacketDef.PACKET_HEADER_SIZE;

			byte[] buffer = null;
			var result = Receive(out buffer);
			if (result == false)
			{
				return packetList;
			}

			if (buffer.Length > 1)
			{
				PacketBuffer.Write(buffer, 0, buffer.Length);

				while (true)
				{
					var data = PacketBuffer.Read();
					if (data.Count < 1)
					{
						return packetList;
					}

					var packet = new PacketData();
					packet.DataSize = (UInt16)(data.Count - PacketHeaderSize);
					packet.PacketID = BitConverter.ToUInt16(data.Array, data.Offset + 2);
					packet.Type = (SByte)data.Array[(data.Offset + 4)];
					packet.BodyData = new byte[packet.DataSize];
					Buffer.BlockCopy(data.Array, (data.Offset + PacketHeaderSize), packet.BodyData, 0, (data.Count - PacketHeaderSize));
					packetList.Add(packet);
				}
			}
			else
			{
				// 서버에서 접속을 종료하였음을 알린다.
				var packet = new PacketData();
				packet.PacketID = PacketDef.SysPacketIDDisConnectdFromServer;
				packetList.Add(packet);
			}

			return packetList;
		}

		// 송신처리.
		public void Send(byte[] data)
		{
			SendQueue.Enqueue(data);
		}


		// 수신처리.
		bool Receive(out byte[] buffer)
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
	}



	public class PacketDef
	{
		public const Int16 PACKET_HEADER_SIZE = 5;

		public const UInt16 SysPacketIDDisConnectdFromServer = 1;
	}

	public struct PacketData
	{
		public UInt16 DataSize;
		public UInt16 PacketID;
		public SByte Type;
		public byte[] BodyData;
	}

}