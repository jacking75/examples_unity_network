using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace NetLib
{
	public class TransportTCP
	{		
		// 클라이언트와의 접속용 소켓.
		private Socket TcpSocket = null;

		// 송신 버퍼.
		//private PacketQueue m_sendQueue;
		// 수신 버퍼.
		//private PacketQueue m_recvQueue;
		System.Collections.Concurrent.ConcurrentQueue<byte[]> SendQueue = new System.Collections.Concurrent.ConcurrentQueue<byte[]>();
		System.Collections.Concurrent.ConcurrentQueue<byte[]> RecvQueue = new System.Collections.Concurrent.ConcurrentQueue<byte[]>();
				
		// 접속 플래그.
		public bool IsConnected { get; private set; } = false;

		
		// 이벤트 통지 델리게이트.
		public delegate void EventHandler(NetEventState state);

		//private EventHandler m_handler;

		
		// 스레스 실행 플래그.
		protected bool IsRunThreadLoop = false;

		protected Thread ThreadHandle = null;

		private const int MtuSize = 1400;

		public System.Action<string> DebugPrintFunc;

		// Use this for initialization
		void Start()
		{
			//m_sendQueue = new PacketQueue();
			//m_recvQueue = new PacketQueue();
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

			//if (m_handler != null)
			//{
			//	// 접속 결과를 통지합니다. 
			//	NetEventState state = new NetEventState();
			//	state.type = NetEventType.Connect;
			//	state.result = (IsConnected == true) ? NetEventResult.Success : NetEventResult.Failure;
			//	m_handler(state);
			//	DebugPrintFunc("event handler called");
			//}

			return IsConnected;
		}

		// 끊기.
		public void Disconnect()
		{
			IsConnected = false;

			if (TcpSocket != null)
			{
				// 소켓 클로즈.
				TcpSocket.Shutdown(SocketShutdown.Both);
				TcpSocket.Close();
				TcpSocket = null;
			}

			// 끊기를 통지합니다.
			//if (m_handler != null)
			//{
			//	NetEventState state = new NetEventState();
			//	state.type = NetEventType.Disconnect;
			//	state.result = NetEventResult.Success;
			//	m_handler(state);
			//}
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

		//// 이벤트 통지함수 등록.
		//public void RegisterEventHandler(EventHandler handler)
		//{
		//	m_handler += handler;
		//}

		//// 이벤트 통지함수 삭제.
		//public void UnregisterEventHandler(EventHandler handler)
		//{
		//	m_handler -= handler;
		//}

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
				while (TcpSocket.Poll(0, SelectMode.SelectRead))
				{
					byte[] buffer = new byte[MtuSize];

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
						RecvQueue.Enqueue(buffer);
					}
				}
			}
			catch
			{
				return;
			}
		}


	}
}