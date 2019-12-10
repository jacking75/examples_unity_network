using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using ServerCommon;

namespace LobbyServer
{
    public enum CLIENT_LOBBY_STATE
    {
        NONE = 0,
        LOGIN = 1,
        LOBBY = 2,
    }


    public class LobbyNetworkServer : MonoBehaviour
    {
        private static LobbyNetworkServer instance = null;
        private string address = "127.0.0.1";
        private int port = Convert.ToInt32("11021");
        private const ushort PacketHeaderSize = 5;

        public string UserID { get; set; } = "";
        
        public string AuthToken { get; set; } = "";
        
        public CLIENT_LOBBY_STATE m_ClientState { get; set; }

        NetLib.TransportTCP Network;

        //ClientSimpleTcp Network = new ClientSimpleTcp();
        //PacketBufferManager PacketBuffer = new PacketBufferManager();

        //Queue<byte[]> RecvPacketQueue = new Queue<byte[]>();
        //Queue<byte[]> SendPacketQueue = new Queue<byte[]>();

        public Queue<string> ChatMsgQueue { get; set; } = new Queue<string>();
        
        //bool IsNetworkThreadRunning = false;

        //System.Threading.Thread NetworkReadThread = null;
        //System.Threading.Thread NetworkSendThread = null;
        //System.Threading.Thread ProcessReceivedPacketThread = null;

        // Start is called before the first frame update
        void Start()
        {            
        }

        // Update is called once per frame
        void Update()
        {
        }


        void Init()
        {
            Network = new NetLib.TransportTCP();
            Network.DebugPrintFunc = WriteDebugLog;
            Network.Start();
            Network.Connect(address, port);
            //PacketBuffer.Init((8096 * 10), 5, 1024);
            m_ClientState = CLIENT_LOBBY_STATE.NONE;

            //IsNetworkThreadRunning = true;
            //NetworkReadThread = new System.Threading.Thread(this.NetworkReadProcess);
            //NetworkReadThread.Start();
            //NetworkSendThread = new System.Threading.Thread(this.NetworkSendProcess);
            //NetworkSendThread.Start();
            //ProcessReceivedPacketThread = new System.Threading.Thread(this.ProcessReceivedPacket);
            //ProcessReceivedPacketThread.Start();
        }


        public static LobbyNetworkServer Instance
        {
            get
            {
                return instance;
            }
        }

        private void Awake()
        {
            //TODO 주석 처리 후 문제 없으면 삭제하자
            if (instance)
            {
                DestroyImmediate(gameObject);
                return;
            }

            instance = this;
            Init();
            DontDestroyOnLoad(gameObject);
        }

        void OnApplicationQuit()
        {
            Debug.Log("Close LobbyServerNetwork");
            Network.Disconnect();
        }



        bool CheckNetworkConnected()
        {
            //아래는 requestSend부분
            if (Network.IsConnected == false)
            {
                Debug.LogWarning("서버에 접속하지 않았습니다");
                return false;
            }
            else
            {
                return true;
            }
        }


        public void LoginRequest(string userID, string authToken)
        {
            var lobbyLoginPkt = new LoginReqPacket()
            {
                UserID = userID,
            };
           
            try
            {                
                PostSendPacket(CL_PACKET_ID.REQ_LOBBY_LOGIN, lobbyLoginPkt.ToBytes());
            }
            catch(Exception e)
            {
                Debug.Log(e.ToString());
            }
        }


        public void LobbyEnterRequest(int roomNumber)
        {
            var requestPkt = new LobbyEnterReqPacket()
            {
                LobbyNumber = roomNumber,
            };

            //아래는 requestSend부분
            if (CheckNetworkConnected() == false)
            {
                return;
            }

            try
            {
                PostSendPacket(CL_PACKET_ID.REQ_LOBBY_ENTER, requestPkt.ToBytes());
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }


        public void LobbyChatRequest(string message)
        {
            var requestPkt = new LobbyChatReqPacket()
            {
                Msg = message
            };

            if (CheckNetworkConnected() == false)
            {
                return;
            }

            try
            {
                PostSendPacket(CL_PACKET_ID.REQ_LOBBY_CHAT, requestPkt.ToBytes());
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }


        public void MatchingRequest()
        {
            if (CheckNetworkConnected() == false)
            {
                return;
            }

            try
            {                
                PostSendPacket(CL_PACKET_ID.REQ_LOBBY_MATCH, null);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }


        //void NetworkReadProcess()
        //{
        //    while (IsNetworkThreadRunning)
        //    {
        //        System.Threading.Thread.Sleep(32);

        //        if (Network.IsConnected() == false)
        //        {
        //            continue;
        //        }

        //        var recvData = Network.Receive();

        //        if (recvData.Count > 0)
        //        {
        //            PacketBuffer.Write(recvData.Array, recvData.Offset, recvData.Count);

        //            while (true)
        //            {
        //                //TODO Read를 대충 Overide했는데 이 부분도 정리필요
        //                var data = PacketBuffer.Read(3);
        //                if (data.Count < 1)
        //                {
        //                    break;
        //                }

        //                UInt16 DataSize = (UInt16)data.Count;
        //                byte[] packetData = new byte[DataSize];
        //                Buffer.BlockCopy(data.Array, data.Offset, packetData, 0, DataSize);

        //                lock (((System.Collections.ICollection)RecvPacketQueue).SyncRoot)
        //                {
        //                    RecvPacketQueue.Enqueue(packetData);
        //                }
        //            }
        //        }                
        //    }
        //}


        //void NetworkSendProcess()
        //{
        //    while (IsNetworkThreadRunning)
        //    {
        //        System.Threading.Thread.Sleep(32);

        //        if (Network.IsConnected() == false)
        //        {
        //            continue;
        //        }

        //        lock (((System.Collections.ICollection)RecvPacketQueue).SyncRoot)
        //        {
        //            if (SendPacketQueue.Count > 0)
        //            {
        //                var packet = SendPacketQueue.Dequeue();
        //                Network.Send(packet);
        //            }
        //        }
        //    }
        //}



        void PostSendPacket(CL_PACKET_ID packetID, byte[] bodyData)
        {
            if (Network.IsConnected == false)
            {
                Debug.LogWarning("서버에 접속하지 않았습니다");
                return;
            }

            List<byte> dataSource = new List<byte>();
            var packetSize = 0;

            if (bodyData != null)
            {
                packetSize = (Int16)(bodyData.Length + 5);
            }
            else
            {
                packetSize = (Int16)(PacketDef.PACKET_HEADER_SIZE);
            }

            dataSource.AddRange(BitConverter.GetBytes((Int16)packetSize));
            dataSource.AddRange(BitConverter.GetBytes((Int16)packetID));
            dataSource.AddRange(new byte[] { (byte)0 });
            if (bodyData != null)
            {
                dataSource.AddRange(bodyData);
            }

            Network.Send(dataSource.ToArray());
        }


        public NetLib.PacketData ReadPacket()
        {
            if (Network.IsConnected == false)
            {
                return default(NetLib.PacketData);
            }

            return Network.GetPacket();
        }

        //void ProcessReceivedPacket()
        //{
        //    while (IsNetworkThreadRunning)
        //    {
        //        System.Threading.Thread.Sleep(32);
        //        ReadPacketQueueProcess();
        //    }
        //}

        //void ReadPacketQueueProcess()
        //{
        //    try
        //    {
        //        LobbyServerPacket packet = new LobbyServerPacket();
        //        lock (((System.Collections.ICollection)RecvPacketQueue).SyncRoot)
        //        {
        //            if (RecvPacketQueue.Count() > 0)
        //            {
        //                LobbyServerPacketHandler.Process(RecvPacketQueue.Dequeue());
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.LogError(ex.Message);
        //    }
        //}


        void WriteDebugLog(string msg)
        {
            Debug.Log(msg);
        }
    }

}