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
        
        public string UserID { get; set; } = "";
        
        public string AuthToken { get; set; } = "";
        
        public CLIENT_LOBBY_STATE m_ClientState { get; set; }

        ClientNetLib.TransportTCP Network;
                
        public Queue<string> ChatMsgQueue { get; set; } = new Queue<string>();
        
        
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
            Network = new ClientNetLib.TransportTCP();
            Network.DebugPrintFunc = WriteDebugLog;
                        
            m_ClientState = CLIENT_LOBBY_STATE.NONE;            
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


        public void LoginRequest(UserIDInfo userInfo)
        {
            var lobbyLoginPkt = new LoginReqPacket()
            {
                UserID = userInfo.UserID,
            };
           
            try
            {
                Network.Connect(userInfo.LobbyServerIP, userInfo.LobbyServerPort);

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


        public List<ClientNetLib.PacketData> ReadPacket()
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