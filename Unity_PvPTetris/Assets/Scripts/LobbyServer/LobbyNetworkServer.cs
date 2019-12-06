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
        private string address = "10.14.0.81";
        private int port = Convert.ToInt32("11021");
        private const ushort PacketHeaderSize = 5;

        public string UserID { get; set; } = "";
        public string AuthToken { get; set; } = "";
        public CLIENT_LOBBY_STATE m_ClientState { get; set; }

        ClientSimpleTcp Network = new ClientSimpleTcp();
        PacketBufferManager PacketBuffer = new PacketBufferManager();

        Queue<byte[]> RecvPacketQueue = new Queue<byte[]>();
        Queue<byte[]> SendPacketQueue = new Queue<byte[]>();
        public Queue<PKTNtfLobbyChat> ChatMsgQueue { get; set; } = new Queue<PKTNtfLobbyChat>();
        bool IsNetworkThreadRunning = false;

        System.Threading.Thread NetworkReadThread = null;
        System.Threading.Thread NetworkSendThread = null;
        System.Threading.Thread ProcessReceivedPacketThread = null;

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
            Network.Connect(address, port);
            PacketBuffer.Init((8096 * 10), 5, 1024);
            m_ClientState = CLIENT_LOBBY_STATE.NONE;

            IsNetworkThreadRunning = true;
            NetworkReadThread = new System.Threading.Thread(this.NetworkReadProcess);
            NetworkReadThread.Start();
            NetworkSendThread = new System.Threading.Thread(this.NetworkSendProcess);
            NetworkSendThread.Start();
            ProcessReceivedPacketThread = new System.Threading.Thread(this.ProcessReceivedPacket);
            ProcessReceivedPacketThread.Start();
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
            if (instance)
            {
                DestroyImmediate(gameObject);
                return;
            }
            instance = this;
            Init();
            DontDestroyOnLoad(gameObject);
        }


        bool CheckNetworkConnected()
        {
            //아래는 requestSend부분
            if (Network.IsConnected() == false)
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
            var lobbyLoginPkt = new PKTReqLobbyLogin()
            {
                UserID = userID,
                AuthToken = authToken
            };
           
            try
            {
                byte[] data = MessagePackSerializer.Serialize(lobbyLoginPkt);
                this.UserID = userID;
                this.AuthToken = authToken;
                PacketDef.SetHeadInfo(data, (UInt16)CL_PACKET_ID.REQ_LOGIN, (UInt16)data.Length);
                Network.Send(data);
            }
            catch(Exception e)
            {
                Debug.Log(e.ToString());
            }
        }


        public void LobbyEnterRequest(int roomNumber)
        {
            var lobbyEnterPkt = new PKTReqLobbyEnter()
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
                byte[] data = MessagePackSerializer.Serialize(lobbyEnterPkt);
                PacketDef.SetHeadInfo(data, (UInt16)CL_PACKET_ID.REQ_LOBBY_ENTER, (UInt16)data.Length);
                Network.Send(data);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }


        public void LobbyChatRequest(string message)
        {
            var lobbyChatReqPkt = new PKTReqLobbyChat()
            {
                ChatMessage = message
            };

            if (CheckNetworkConnected() == false)
            {
                return;
            }

            try
            {
                byte[] data = MessagePackSerializer.Serialize(lobbyChatReqPkt);
                PacketDef.SetHeadInfo(data, (UInt16)CL_PACKET_ID.REQ_LOBBY_CHAT, (UInt16)data.Length);
                Network.Send(data);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }


        public void MatchingRequest()
        {
            var matchingReqPkt = new PKTReqLobbyMatch()
            {
                Dummy = 1
            };

            if (CheckNetworkConnected() == false)
            {
                return;
            }

            try
            {
                byte[] data = MessagePackSerializer.Serialize(matchingReqPkt);
                PacketDef.SetHeadInfo(data, (UInt16)CL_PACKET_ID.REQ_LOBBY_MATCH, (UInt16)data.Length);
                Network.Send(data);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }


        void NetworkReadProcess()
        {
            while (IsNetworkThreadRunning)
            {
                System.Threading.Thread.Sleep(32);

                if (Network.IsConnected() == false)
                {
                    continue;
                }

                var recvData = Network.Receive();

                if (recvData.Count > 0)
                {
                    PacketBuffer.Write(recvData.Array, recvData.Offset, recvData.Count);

                    while (true)
                    {
                        //TODO Read를 대충 Overide했는데 이 부분도 정리필요
                        var data = PacketBuffer.Read(3);
                        if (data.Count < 1)
                        {
                            break;
                        }

                        UInt16 DataSize = (UInt16)data.Count;
                        byte[] packetData = new byte[DataSize];
                        Buffer.BlockCopy(data.Array, data.Offset, packetData, 0, DataSize);

                        lock (((System.Collections.ICollection)RecvPacketQueue).SyncRoot)
                        {
                            RecvPacketQueue.Enqueue(packetData);
                        }
                    }
                }
                else
                {
                    /* var packet = new LobbyServerPacket();
                     packet.PacketID = (Int16)CL_PACKET_ID.CS_END;
                     packet.PacketSize = 5;

                     lock (((System.Collections.ICollection)RecvPacketQueue).SyncRoot)
                     {
                         RecvPacketQueue.Enqueue(packet);
                     } */
                }
            }
        }


        void NetworkSendProcess()
        {
            while (IsNetworkThreadRunning)
            {
                System.Threading.Thread.Sleep(32);

                if (Network.IsConnected() == false)
                {
                    continue;
                }

                lock (((System.Collections.ICollection)RecvPacketQueue).SyncRoot)
                {
                    if (SendPacketQueue.Count > 0)
                    {
                        var packet = SendPacketQueue.Dequeue();
                        Network.Send(packet);
                    }
                }
            }
        }



        void PostSendPacket(CL_PACKET_ID packetID, byte[] bodyData)
        {
            if (Network.IsConnected() == false)
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
            SendPacketQueue.Enqueue(dataSource.ToArray());
        }


        void ProcessReceivedPacket()
        {
            while (IsNetworkThreadRunning)
            {
                System.Threading.Thread.Sleep(32);
                ReadPacketQueueProcess();
            }
        }


        void ReadPacketQueueProcess()
        {
            try
            {
                LobbyServerPacket packet = new LobbyServerPacket();
                lock (((System.Collections.ICollection)RecvPacketQueue).SyncRoot)
                {
                    if (RecvPacketQueue.Count() > 0)
                    {
                        LobbyServerPacketHandler.Process(RecvPacketQueue.Dequeue());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }

    }



}