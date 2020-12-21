using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace LobbyServer
{
    public class PKHRoom : PKHandler
    {
        List<Room> RoomList = null;
        int StartRoomNumber;
        
        public void SetLobbyList(List<Room> roomList)
        {
            RoomList = roomList;
            StartRoomNumber = roomList[0].Number;
        }

        public void RegistPacketHandler(Dictionary<int, Action<ServerPacketData>> packetHandlerMap)
        {
            packetHandlerMap.Add((int)CL_PACKET_ID.REQ_ROOM_ENTER, RequestRoomEnter);
            packetHandlerMap.Add((int)CL_PACKET_ID.REQ_ROOM_LEAVE, RequestRoomLeave);
            packetHandlerMap.Add((int)SYS_PACKET_ID.NTF_IN_ROOM_LEAVE, NotifyLeaveInternal);
            packetHandlerMap.Add((int)CL_PACKET_ID.REQ_ROOM_CHAT, RequestRoomChat);
            
            packetHandlerMap.Add((int)CL_PACKET_ID.REQ_GAME_START, RequestGameStart);
            packetHandlerMap.Add((int)CL_PACKET_ID.REQ_GAME_SYNC, RequestGameSync);
            packetHandlerMap.Add((int)CL_PACKET_ID.REQ_GAME_END, RequestGameEnd);
        }



        #region PacketHandler
        public void RequestRoomEnter(ServerPacketData packetData)
        {
            var sessionID = packetData.SessionID;
            GameServer.MainLogger.Debug("RequestRoomEnter");

            try
            {
                var user = UserMgr.GetUserByNetSessionID(sessionID);

                if (user == null || user.IsAuthenticated == false)
                {
                    ResponseRoomEnterToClient(sessionID, ERROR_CODE.ROOM_ENTER_INVALID_USER);
                    return;
                }

                if (user.IsStateLobby())
                {
                    ResponseRoomEnterToClient(sessionID, ERROR_CODE.ROOM_ENTER_INVALID_STATE);
                    return;
                }

                var reqData = new RoomEnterReqPacket();
                reqData.Decode(packetData.BodyData);

                var room = GetRoom(reqData.RoomNumber);
                if (room == null)
                {
                    ResponseRoomEnterToClient(sessionID, ERROR_CODE.ROOM_ENTER_INVALID_ROOM_NUMBER);
                    return;
                }

                if(room.IsGamming)
                {
                    ResponseRoomEnterToClient(sessionID, ERROR_CODE.ROOM_ENTER_INVALID_ROOM_STATE);
                    return;
                }

                if (room.CurrentUserCount() > 1)
                {
                    ResponseGameStartToClient(sessionID, ERROR_CODE.ROOM_ENTER_INVALID_ROOM_USER_COUNT);
                }

                var rivalUserID = "empty";
                if(room.CurrentUserCount() == 1)
                {
                    rivalUserID = room.FirstUserID();
                }

                if (room.AddUser(user.ID, sessionID) == false)
                {
                    ResponseRoomEnterToClient(sessionID, ERROR_CODE.ROOM_ENTER_FAIL_ADD_USER);
                    return;
                }


                user.EnteredLobby(reqData.RoomNumber);

                ResponseRoomEnterToClient(sessionID, ERROR_CODE.NONE, rivalUserID);

                GameServer.MainLogger.Debug("RequestRoomEnter - Success");
            }
            catch (Exception ex)
            {
                GameServer.MainLogger.Error(ex.ToString());
            }
        }
                

        public void RequestRoomLeave(ServerPacketData packetData)
        {
            var sessionID = packetData.SessionID;
            GameServer.MainLogger.Debug("RequestRoomLeave");

            try
            {
                var user = UserMgr.GetUserByNetSessionID(sessionID);
                if(user == null)
                {
                    return;
                }
                                
                if (LeaveRoomUser(sessionID, user.RoomNumber) == false)
                {
                    return;
                }

                user.LeaveRoom();

                ResponseRoomLeaveToClient(sessionID, ERROR_CODE.NONE);
                
                GameServer.MainLogger.Debug("RequestLeave - Success");
            }
            catch (Exception ex)
            {
                GameServer.MainLogger.Error(ex.ToString());
            }
        }

        public void NotifyLeaveInternal(ServerPacketData packetData)
        {
            GameServer.MainLogger.Debug($"NotifyLeaveInternal. SessionID: {packetData.SessionID}");

            var ntfData = new PKTInternalNtfRoomLeave();
            ntfData.Decode(packetData.BodyData);
  
            LeaveRoomUser(packetData.SessionID, ntfData.RoomNumber);
        }



        public void RequestRoomChat(ServerPacketData packetData)
        {
            var sessionID = packetData.SessionID;
            GameServer.MainLogger.Debug("RequestChat");

            try
            {
                var roomObject = CheckRoomAndRoomUser(sessionID);
                if(roomObject.Item1 == false)
                {
                    return;
                }

                var reqData = new RoomChatReqPacket();
                reqData.Decode(packetData.BodyData);

                ResponseRoomChatToClient(sessionID, ERROR_CODE.NONE);


                var notifyPacket = new RoomChatNtfPacket()
                {
                    Msg = reqData.Msg,
                };

                roomObject.Item2.Broadcast((UInt16)CL_PACKET_ID.NTF_ROOM_CHAT, notifyPacket.ToBytes());

                GameServer.MainLogger.Debug("RequestChat - Success");
            }
            catch (Exception ex)
            {
                GameServer.MainLogger.Error(ex.ToString());
            }
        }


        public void RequestGameStart(ServerPacketData packetData)
        {
            var sessionID = packetData.SessionID;
            GameServer.MainLogger.Debug("RequestMatch");

            try
            {
                var roomObject = CheckRoomAndRoomUser(sessionID);
                if (roomObject.Item1 == false)
                {
                    return;
                }

                var room = roomObject.Item2;
                var roomUser = roomObject.Item3;
                        
                if(room.IsGamming)
                {
                    ResponseGameStartToClient(sessionID, ERROR_CODE.GAME_START_INVALID_ROOM_STATE);
                }

                if(room.CurrentUserCount() != 2)
                {
                    ResponseGameStartToClient(sessionID, ERROR_CODE.GAME_START_INVALID_ROOM_USER_COUNT);
                }


                ResponseGameStartToClient(sessionID, ERROR_CODE.NONE);

                room.Broadcast((UInt16)CL_PACKET_ID.NTF_GAME_START, null);

                room.IsGamming = true;                                
            }
            catch (Exception ex)
            {
                GameServer.MainLogger.Error(ex.ToString());
            }
        }


        public void RequestGameSync(ServerPacketData packetData)
        {
            var roomObject = CheckRoomAndRoomUser(packetData.SessionID);
            if (roomObject.Item1 == false)
            {
                return;
            }
            
            var room = roomObject.Item2;

            if (room.IsGamming == false)
            {
                return;
            }


            var ntfPacket = new GameSyncNtfPacket();
            ntfPacket.Decode(packetData.BodyData);

            room.Broadcast((UInt16)CL_PACKET_ID.NTF_GAME_SYNC, ntfPacket.ToBytes());         
        }


        public void RequestGameEnd(ServerPacketData packetData)
        {
            var sessionID = packetData.SessionID;

            var roomObject = CheckRoomAndRoomUser(packetData.SessionID);
            if (roomObject.Item1 == false)
            {
                return;
            }

            var room = roomObject.Item2;

            if (room.IsGamming == false)
            {
                return;
            }


            var resPacket = new GameEndResPacket();
            resPacket.Result = (Int16)ERROR_CODE.NONE;
            resPacket.Decode(packetData.BodyData);
            ServerNetwork.SendData(sessionID, (UInt16)CL_PACKET_ID.RES_GAME_END, resPacket.ToBytes());
                                    
            room.Broadcast((UInt16)CL_PACKET_ID.NTF_GAME_END, null);


            room.IsGamming = false;
        }
        #endregion
        


        #region private member
        Room GetRoom(int roomNumber)
        {
            var index = roomNumber - StartRoomNumber;

            if (index < 0 || index >= RoomList.Count())
            {
                return null;
            }

            return RoomList[index];
        }

        (bool, Room, RoomUser) CheckRoomAndRoomUser(string userNetSessionID)
        {
            var user = UserMgr.GetUserByNetSessionID(userNetSessionID);
            if (user == null)
            {
                return (false, null, null);
            }

            var roomNumber = user.RoomNumber;
            var room = GetRoom(roomNumber);

            if (room == null)
            {
                return (false, null, null);
            }


            var roomUser = room.GetUser(userNetSessionID);
            if (roomUser == null)
            {
                return (false, room, null);
            }

            return (true, room, roomUser);
        }

        void ResponseRoomEnterToClient(string sessionID, ERROR_CODE errorCode, string rivalUserID="empty")
        {
            var responsePkt = new RoomEnterResPacket()
            {
                Result = (short)errorCode,
                RivalUserID = rivalUserID
            };

            ServerNetwork.SendData(sessionID, (UInt16)CL_PACKET_ID.RES_ROOM_ENTER, responsePkt.ToBytes());
        }

        void ResponseRoomLeaveToClient(string sessionID, ERROR_CODE errorCode)
        {
            var responsePkt = new RoomLeaveResPacket()
            {
                Result = (short)errorCode
            };

            ServerNetwork.SendData(sessionID, (UInt16)CL_PACKET_ID.RES_ROOM_LEAVE, responsePkt.ToBytes());
        }

        void ResponseRoomChatToClient(string sessionID, ERROR_CODE errorCode)
        {
            var responsePkt = new RoomChatResPacket()
            {
                Result = (short)errorCode
            };
                        
            ServerNetwork.SendData(sessionID, (UInt16)CL_PACKET_ID.RES_ROOM_CHAT, responsePkt.ToBytes());
        }

        
        bool LeaveRoomUser(string sessionID, int lobbyNumber)
        {
            GameServer.MainLogger.Debug($"LeaveRoomUser. SessionID:{sessionID}");

            var room = GetRoom(lobbyNumber);
            if (room == null)
            {
                return false;
            }

            var roomUser = room.GetUser(sessionID);
            if (roomUser == null)
            {
                return false;
            }

            var userID = roomUser.UserID;
            room.RemoveUser(roomUser);

            room.IsGamming = false;
            return true;
        }

        
        void ResponseGameStartToClient(string sessionID, ERROR_CODE errorCode)
        {
            var responsePkt = new GameStartResPacket()
            {
                Result = (short)errorCode
            };

            ServerNetwork.SendData(sessionID, (UInt16)CL_PACKET_ID.RES_GAME_START, responsePkt.ToBytes());
        }
        
        #endregion


    }
}
