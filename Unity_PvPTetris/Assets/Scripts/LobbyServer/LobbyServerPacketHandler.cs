using System;
using MessagePack;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ServerCommon;
namespace LobbyServer
{
    public class LobbyServerPacket
    {
        public UInt16 DataSize;
        public Int16 PacketID;
        public Byte Type;
        public byte[] BodyData;
    }


    public class LobbyServerPacketHandler
    {
        public static void Process(NetLib.PacketData packet)
        {
            var PacketID = (CL_PACKET_ID)packet.PacketID;

            switch (PacketID)
            {
                case CL_PACKET_ID.RES_LOBBY_LOGIN:
                    {
                        ProcessResponseLogin(packet.BodyData);
                        break;
                    }

                case CL_PACKET_ID.RES_LOBBY_ENTER:
                    {
                        ProcessResponseLobbyEnter(packet.BodyData);
                        break;
                    }

                case CL_PACKET_ID.RES_LOBBY_LEAVE:
                    {
                        ProcessResponseLobbyLeave(packet.BodyData);
                        break;
                    }

                case CL_PACKET_ID.RES_LOBBY_CHAT:
                    {
                        ProcessResponseLobbyChat(packet.BodyData);
                        break;
                    }

                case CL_PACKET_ID.NTF_LOBBY_CHAT:
                    {
                        ProcessNotifyLobbyChat(packet.BodyData);
                        break;
                    }

                case CL_PACKET_ID.RES_LOBBY_MATCH:
                    {
                        ProcessResponseLobbyMatch(packet.BodyData);
                        break;
                    }

                case CL_PACKET_ID.NTF_LOBBY_MATCH:
                    {
                        ProcessNotifyLobbyMatch(packet.BodyData);
                        break;
                    }
            }
        }


        static void ProcessResponseLogin(Byte[] data)
        {
            try
            {
                var response = new LoginResPacket();
                response.Decode(data);
                
                Debug.Log("로그인패킷 도착");
                if (response.Result == (Int16)ERROR_CODE.NONE)
                {
                    LobbyNetworkServer.Instance.m_ClientState = CLIENT_LOBBY_STATE.LOGIN;
                }
                else
                {
                    //TODO 에러코드에 따라 로그인 실패이유를 pop-up으로 안내해준다.
                    Debug.Log("로그인 실패");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }


        static void ProcessResponseLobbyEnter(Byte[] data)
        {
            try
            {
                var response = new LobbyEnterResPacket();
                response.Decode(data);
                
                Debug.Log("로비입장 패킷 도착");
                
                if (response.Result == (Int16)ERROR_CODE.NONE)
                {
                    LobbyNetworkServer.Instance.m_ClientState = CLIENT_LOBBY_STATE.LOBBY;
                }
                else
                {
                    //TODO 에러코드에 따라 로비입장 실패이유를 pop-up으로 안내해준다.
                    Debug.Log("로비입장 실패");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }


        static void ProcessResponseLobbyLeave(Byte[] data)
        {
            try
            {
                var response = new LobbyLeaveResPacket();
                response.Decode(data);

                Debug.Log("로비입장 퇴장패킷 도착");
                
                if (response.Result == (Int16)ERROR_CODE.NONE)
                {
                    LobbyNetworkServer.Instance.m_ClientState = CLIENT_LOBBY_STATE.LOGIN;
                }
                else
                {
                    //TODO 에러코드에 따라 실패이유를 pop-up으로 안내해준다.
                    Debug.Log("로비퇴장 실패");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }


        static void ProcessResponseLobbyChat(Byte[] data)
        {
            try
            {
                var response = new LobbyChatResPacket();
                response.Decode(data);

                Debug.Log("메세지 전송답변패킷 도착");
                
                if (response.Result != (Int16)ERROR_CODE.NONE)
                {
                    Debug.Log("메세지 전송 실패");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        static void ProcessNotifyLobbyChat(Byte[] data)
        {
            try
            {
                var response = new LobbyChatNtfPacket();
                response.Decode(data);

                Debug.Log("채팅알림 패킷 도착");
                
                LobbyNetworkServer.Instance.ChatMsgQueue.Enqueue(response.Msg);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        static void ProcessResponseLobbyMatch(Byte[] data)
        {
            try
            {
                var response = new LobbyMatchResPacket();
                response.Decode(data);

                Debug.Log("매칭응답 패킷 도착");
                
                if(response.Result == (short)ERROR_CODE.NONE)
                {
                    LobbySceneManager.isMatchingResArrived = true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        static void ProcessNotifyLobbyMatch(Byte[] data)
        {
            try
            {
                var response = new LobbyMatchNtfPacket();
                response.Decode(data);

                Debug.Log("매칭알림 패킷 도착");

                if( LobbySceneManager.FillMatchInfo(response.GameServerIP, response.GameServerPort, response.RoomNumber) == true)
                {
                    LobbySceneManager.isMatchingNtfArrived = true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
    }
}