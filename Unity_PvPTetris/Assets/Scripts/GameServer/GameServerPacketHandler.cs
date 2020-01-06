using System;
using UnityEngine;

namespace GameNetwork
{
    class GameServerPacketHandler
    {
        public static void Process(ClientNetLib.PacketData packet)
        {
            var packetType = (PACKET_ID)packet.PacketID;

            switch (packetType)  {
                case PACKET_ID.RES_GAME_LOGIN:
                    {
                        ProcessLoginResponse(packet);
                        break;
                    }

                case PACKET_ID.RES_ROOM_ENTER:
                    {
                        ProcessEnterRoomResponse(packet);
                        break;
                    }

                case PACKET_ID.NTF_ROOM_CHAT:
                    {
                        ProcessChatRoomNotify(packet);
                        break;
                    }

                case PACKET_ID.RES_GAME_START:
                    {
                        ProcessGameStartResponse(packet);
                        break;
                    }

                case PACKET_ID.NTF_GAME_START:
                    {
                        ProcessGameStartNotify(packet);
                        break;
                    }

                case PACKET_ID.NTF_GAME_SYNC:
                    {
                        ProcessGameSyncNotify(packet);
                        break;
                    }

                case PACKET_ID.RES_GAME_END:
                    {
                        ProcessGameEndResponse(packet);
                        break;
                    }

                case PACKET_ID.NTF_GAME_END:
                    {
                        ProcessGameEndNotify(packet);
                        break;  
                    }
            }
        }



        static void ProcessLoginResponse(ClientNetLib.PacketData packet)
        {
            var response = new LoginResPacket();
            response.Decode(packet.BodyData);

            if (response.Result == (Int16)ERROR_CODE.NONE)
            {
                Debug.Log("게임 서버 로그인 성공");
                GameNetworkServer.Instance.ClientStatus = GameNetworkServer.CLIENT_STATUS.LOGIN;
                //로그인 성공처리
            }
            else
            {
                GameNetworkServer.Instance.UserID = "";
            }
        }

        static void ProcessEnterRoomResponse(ClientNetLib.PacketData packet)
        {
            var response = new RoomEnterResPacket();
            response.Decode(packet.BodyData);
            LobbySceneManager.roomEnterRes.Result = response.Result;

            if (response.Result == (Int16)ERROR_CODE.NONE)
            {
                Debug.Log("방 입장성공");
                GameNetworkServer.Instance.ClientStatus = GameNetworkServer.CLIENT_STATUS.ROOM;
                GameNetworkServer.Instance.RivalID = response.RivalUserID;
                LobbySceneManager.isWatingEnterRoomRes = false;
            }
            else
            {
                Debug.Log("방 입장실패");
                if (LobbySceneManager.GetMatchedRooom() != -1)
                {
                    GameNetworkServer.Instance.RequestRoomEnter(LobbySceneManager.GetMatchedRooom());
                }
                
            }
        }

        static void ProcessChatRoomNotify(ClientNetLib.PacketData packet)
        {
            var response = new RoomChatNtfPacket();
            response.Decode(packet.BodyData);
            GameNetworkServer.Instance.ChatMsgQueue.Enqueue(response.Msg);
        }


        static void ProcessGameStartResponse(ClientNetLib.PacketData packet)
        {
            var response = new GameStartResPacket();
            response.Decode(packet.BodyData);
            //TODO Result에 따른 처리 구현하기
        }

        static void ProcessGameStartNotify(ClientNetLib.PacketData packet)
        {
           GameNetworkServer.Instance.ClientStatus = GameNetworkServer.CLIENT_STATUS.GAME;
            
            Spawner.isGameStart = true;
        }

        static void ProcessGameSyncNotify(ClientNetLib.PacketData packet)
        {
            var response = new GameSyncNtfPacket();
            response.Decode(packet.BodyData);
            
            if (ShadowGrid.RecvSyncPacketQueue != null)
            {
                ShadowGrid.RecvSyncPacketQueue.Enqueue(response);
            }
        }

        static void ProcessGameEndResponse(ClientNetLib.PacketData packet)
        {
            var response = new GameEndResPacket();
            response.Decode(packet.BodyData);
            //TODO Result에 따른 처리 구현하기
        }

        static void ProcessGameEndNotify(ClientNetLib.PacketData packet)
        {
            Spawner.isGameEndPacketArrived = true;
            GameNetworkServer.Instance.Disconnect();
        }
    }
}
