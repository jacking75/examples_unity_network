using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;

using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketEngine;


//TODO 1. 주기적으로 접속한 세션이 패킷을 주고 받았는지 조사(좀비 클라이언트 검사)

namespace ChatServer
{
    public class MainServer : AppServer<ClientSession, EFBinaryRequestInfo>
    {
        public static ChatServerOption ServerOption;
        public static SuperSocket.SocketBase.Logging.ILog MainLogger;

        SuperSocket.SocketBase.Config.IServerConfig m_Config;

        
        public MainServer()
            : base(new DefaultReceiveFilterFactory<ReceiveFilter, EFBinaryRequestInfo>())
        {
            NewSessionConnected += new SessionHandler<ClientSession>(OnConnected);
            SessionClosed += new SessionHandler<ClientSession, CloseReason>(OnClosed);
            NewRequestReceived += new RequestHandler<ClientSession, EFBinaryRequestInfo>(OnPacketReceived);
        }

        public void InitConfig(ChatServerOption option)
        {
            ServerOption = option;

            m_Config = new SuperSocket.SocketBase.Config.ServerConfig
            {
                Name = option.Name,
                Ip = "Any",
                Port = option.Port,
                Mode = SocketMode.Tcp,
                MaxConnectionNumber = option.MaxConnectionNumber,
                MaxRequestLength = option.MaxRequestLength,
                ReceiveBufferSize = option.ReceiveBufferSize,
                SendBufferSize = option.SendBufferSize
            };
        }

        public void CreateStartServer()
        {
            try
            {
                bool bResult = Setup(new SuperSocket.SocketBase.Config.RootConfig(), m_Config, logFactory: new SuperSocket.SocketBase.Logging.NLogLogFactory());

                if (bResult == false)
                {
                    Console.WriteLine("[ERROR] 서버 네트워크 설정 실패 ㅠㅠ");
                    return;
                }
                else
                {
                    MainLogger = base.Logger;
                    MainLogger.Info("서버 초기화 성공");
                }


                CreateComponent();

                Start();

                MainLogger.Info("서버 생성 성공");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] 서버 생성 실패: {ex.ToString()}");
            }
        }


        public void StopServer()
        {
            Stop();
        }

        public ERROR_CODE CreateComponent()
        {            
            MainLogger.Info("CreateComponent - Success");
            return ERROR_CODE.NONE;
        }

        public bool SendData(string sessionID, byte[] sendData)
        {
            var session = GetSessionByID(sessionID);

            try
            {
                if (session == null)
                {
                    return false;
                }

                session.Send(sendData, 0, sendData.Length);
            }
            catch (Exception ex)
            {
                // TimeoutException 예외가 발생할 수 있다
                MainServer.MainLogger.Error($"{ex.ToString()},  {ex.StackTrace}");

                session.SendEndWhenSendingTimeOut();
                session.Close();
            }
            return true;
        }


        void OnConnected(ClientSession session)
        {
            MainLogger.Info(string.Format("세션 번호 {0} 접속", session.SessionID));
        }

        void OnClosed(ClientSession session, CloseReason reason)
        {
            MainLogger.Info(string.Format("세션 번호 {0} 접속해제: {1}", session.SessionID, reason.ToString()));                        
        }

        void OnPacketReceived(ClientSession session, EFBinaryRequestInfo reqInfo)
        {
            MainLogger.Debug($"세션 번호: {session.SessionID}, 받은 데이터 크기: {reqInfo.Size}, ThreadId: {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            if (reqInfo.PacketID == (UInt16)PACKETID.SIMPLE_CHAT)
            {
                var sendPacket = new SimpleChatPacket();
                sendPacket.SetValue(reqInfo.Body);

                foreach (var client in GetAllSessions())
                {
                    client.Send(sendPacket.Data, 0, sendPacket.Data.Length);
                }
            }
        }
    }


    public class ClientSession : AppSession<ClientSession, EFBinaryRequestInfo>
    {
    }
}
