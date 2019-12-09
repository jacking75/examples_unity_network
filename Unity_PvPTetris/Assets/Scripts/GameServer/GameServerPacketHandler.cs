using UnityEngine;

namespace GameNetwork
{
    class GameServerPacketHandler
    {
        public static void Process(NetLib.PacketData packet)
        {
            var packetType = (PACKET_ID)packet.PacketID;

            switch (packetType)  {
                case PACKET_ID.LoginRes:
                    {
                        ProcessLoginResponse(packet);
                        break;
                    }

                case PACKET_ID.EnterRoomRes:
                    {
                        ProcessEnterRoomResponse(packet);
                        break;
                    }

                case PACKET_ID.ChatRoomNtf:
                    {
                        ProcessChatRoomNotify(packet);
                        break;
                    }

                case PACKET_ID.GameStartResPkt:
                    {
                        ProcessGameStartResponse(packet);
                        break;
                    }

                case PACKET_ID.GameStartNtfPkt:
                    {
                        ProcessGameStartNotify(packet);
                        break;
                    }

                case PACKET_ID.GameSyncNtfPkt:
                    {
                        ProcessGameSyncNotify(packet);
                        break;
                    }

                case PACKET_ID.GameEndResPkt:
                    {
                        ProcessGameEndResponse(packet);
                        break;
                    }

                case PACKET_ID.GameEndNtfPkt:
                    {
                        ProcessGameEndNotify(packet);
                        break;  
                    }
            }
        }



        static void ProcessLoginResponse(NetLib.PacketData packet)
        {
            var response = new LoginResPacket();
            response.FromBytes(packet.BodyData);

            if (response.Result == ERROR_CODE.NONE)
            {
                Debug.Log("로그인성공");
                GameNetworkServer.Instance.ClientStatus = GameNetworkServer.CLIENT_STATUS.LOGIN;
                //로그인 성공처리
            }
            else
            {
                GameNetworkServer.Instance.UserID = "";
            }
        }

        static void ProcessEnterRoomResponse(NetLib.PacketData packet)
        {
            var response = new RoomEnterResPacket();
            response.FromBytes(packet.BodyData);
            LobbySceneManager.roomEnterRes.Result = response.Result;

            if (response.Result == ERROR_CODE.NONE)
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

        static void ProcessChatRoomNotify(NetLib.PacketData packet)
        {
            var response = new RoomChatNotPacket();
            response.FromBytes(packet.BodyData);
            GameNetworkServer.Instance.ChatMsgQueue.Enqueue(response);
        }


        static void ProcessGameStartResponse(NetLib.PacketData packet)
        {
            var response = new GameStartResponsePacket();
            response.FromBytes(packet.BodyData);
            //TODO Result에 따른 처리 구현하기
        }

        static void ProcessGameStartNotify(NetLib.PacketData packet)
        {
           var response = new GameStartNotifyPacket();
            GameNetworkServer.Instance.ClientStatus = GameNetworkServer.CLIENT_STATUS.GAME;
            Spawner.isGameStart = true;
        }

        static void ProcessGameSyncNotify(NetLib.PacketData packet)
        {
            var response = new GameSynchronizeNotifyPacket();
            response.FromBytes(packet.BodyData);
            
            if (ShadowGrid.RecvSyncPacketQueue != null)
            {
                ShadowGrid.RecvSyncPacketQueue.Enqueue(response);
            }
        }

        static void ProcessGameEndResponse(NetLib.PacketData packet)
        {
            var response = new GameEndResponsePacket();
            response.FromBytes(packet.BodyData);
            //TODO Result에 따른 처리 구현하기
        }

        static void ProcessGameEndNotify(NetLib.PacketData packet)
        {
            Spawner.isGameEndPacketArrived = true;
            GameNetworkServer.Instance.Disconnect();
        }
    }
}
