using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using ServerCommon;

namespace GameNetwork
{
    public class GameNetworkServer : MonoBehaviour
    {
        private static GameNetworkServer instance = null;
                
        ClientNetLib.TransportTCP Network;
                
        public Queue<string> ChatMsgQueue { get; set; } = new Queue<string>();
               
        public bool GetIsConnected() { return Network.IsConnected; }
        public void Disconnect() { Network.Disconnect(); }

      
        public CLIENT_STATUS ClientStatus { get; set; } = new CLIENT_STATUS();
        public string UserID { get; set; } = "";
        public string RivalID { get; set; } = "";

        public Single SyncPacketInterval { get; set; } = 0.1f;


        public enum CLIENT_STATUS
        {
            NONE = 0,
            LOGIN = 1,
            ROOM = 2,
            GAME = 3,
        }


        // 
        public static GameNetworkServer Instance {
            get {
                return instance;
            }
        }

        void Awake() 
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


        void Init() {
            ClientStatus = CLIENT_STATUS.NONE;

            Network = new ClientNetLib.TransportTCP();
            Network.DebugPrintFunc = WriteDebugLog;            
        }

        void OnApplicationQuit()
        {
            Debug.Log("Close GameServerNetwork");
            Disconnect();
        }

        //게임서버 네트워크 부분
        public void ConnectToServer(string ip_address, int port_val)
        {
            if (Network.Connect(ip_address, port_val))
            {
                Debug.Log("게임 서버 접속성공!");
            }
            else {
                Debug.Log("게임 서버 접속실패");
            }
        }

        public void RequestLogin(string loginID, string loginPW) 
        {
            UserID = loginID;

            var request = new LoginReqPacket();
            request.UserID = loginID;
            
            var bodyData = request.ToBytes();

            Debug.Log("게임 서버 로그인 요청");
            PostSendPacket(PACKET_ID.REQ_GAME_LOGIN, bodyData);
        }


        public void RequestRoomEnter(int RoomID)
        {
            if (ClientStatus == CLIENT_STATUS.LOGIN)
            {
                var request = new RoomEnterReqPacket();
                request.RoomNumber = RoomID;
                var bodyData = request.ToBytes();
                PostSendPacket(PACKET_ID.REQ_ROOM_ENTER, bodyData);
            }
            else
            {
               Debug.LogError("로그인 상태가 아닙니다.");
            }
        }


        public void RequestChatMsg(string Msg)
        {
            var request = new RoomChatReqPacket();
            request.Msg = Msg;
            var bodyData = request.ToBytes();
            PostSendPacket(PACKET_ID.REQ_ROOM_CHAT, bodyData);
        }


        // 게임플레이 네트워크 부분
        public void SendGameStartPacket()
        {
            PostSendPacket(PACKET_ID.REQ_GAME_START, null);
        }

        public void SendSynchronizePacket(GameSyncReqPacket packet)
        {
            var bodyData = packet.ToBytes();
            PostSendPacket(PACKET_ID.REQ_GAME_SYNC, bodyData);
         }

        public void SendGameEndPacket()
        {
            PostSendPacket(PACKET_ID.REQ_GAME_END, null);
        }

        public void ProcessGameServerPacket()
        {
            var packetList = ReadPacket();

            if (packetList != null)
            {
                foreach (var packet in packetList)
                {
                    if (packet.PacketID == ClientNetLib.PacketDef.SysPacketIDDisConnectdFromServer)
                    {
                        //SetDisconnectd();
                        Debug.Log("서버와 접속 종료 !!!");
                    }
                    else
                    {
                        GameServerPacketHandler.Process(packet);
                    }
                }
            }
        }


        //네트워크 Read/Send 스레드 부분
        void PostSendPacket(PACKET_ID packetID, byte[] bodyData)
        {
            var packetHeaderSize = ClientNetLib.PacketDef.PACKET_HEADER_SIZE;

            if (Network.IsConnected == false)
            {
                Debug.LogWarning("서버에 접속하지 않았습니다");
                return;
            }

            List<byte> dataSource = new List<byte>();
            UInt16 packetSize = 0;

            if (bodyData != null)
            {
                packetSize = (UInt16)(bodyData.Length + packetHeaderSize);
            }
            else
            {
                packetSize = (UInt16)(packetHeaderSize);
            }

            dataSource.AddRange(BitConverter.GetBytes(packetSize));
            dataSource.AddRange(BitConverter.GetBytes((UInt16)packetID));
            dataSource.AddRange(new byte[] { (byte)0 });
            if (bodyData != null)
            {
                dataSource.AddRange(bodyData);
            }

            Network.Send(dataSource.ToArray());
        }

        List<ClientNetLib.PacketData> ReadPacket()
        {
            if (Network.IsConnected == false)
            {
                return default(List<ClientNetLib.PacketData>);
            }

            return Network.GetPacket();                       
        }
      
        void WriteDebugLog(string msg)
        {
            Debug.Log(msg);
        }


        

    }






}
