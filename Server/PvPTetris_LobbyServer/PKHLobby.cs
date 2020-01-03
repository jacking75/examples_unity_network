using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MessagePack;



namespace LobbyServer
{
    public class PKHLobby : PKHandler
    {
        MatchingSystem MatchingSys = new MatchingSystem();

        List<Lobby> LobbyList = null;
        int StartRoomNumber;
        
        public void SetLobbyList(List<Lobby> lobbyList)
        {
            LobbyList = lobbyList;
            StartRoomNumber = lobbyList[0].Number;

            MatchingSys.SendPacket = ServerNetwork.SendData;
        }

        public void RegistPacketHandler(Dictionary<int, Action<ServerPacketData>> packetHandlerMap)
        {
            packetHandlerMap.Add((int)CL_PACKET_ID.REQ_LOBBY_ENTER, RequestLobbyEnter);
            packetHandlerMap.Add((int)CL_PACKET_ID.REQ_LOBBY_LEAVE, RequestLobbyLeave);
            packetHandlerMap.Add((int)SYS_PACKET_ID.NTF_IN_LOBBY_LEAVE, NotifyLeaveInternal);
            packetHandlerMap.Add((int)CL_PACKET_ID.REQ_LOBBY_CHAT, RequestLobbyChat);
            packetHandlerMap.Add((int)CL_PACKET_ID.REQ_LOBBY_MATCH, RequestLobbyMatch);

        }



        #region PacketHandler
        public void RequestLobbyEnter(ServerPacketData packetData)
        {
            var sessionID = packetData.SessionID;
            LobbyServer.MainLogger.Debug("RequestLobbyEnter");

            try
            {
                var user = UserMgr.GetUserByNetSessionID(sessionID);

                if (user == null || user.IsAuthenticated == false)
                {
                    ResponseLobbyEnterToClient(sessionID, ERROR_CODE.LOBBY_ENTER_INVALID_USER);
                    return;
                }

                if (user.IsStateLobby())
                {
                    ResponseLobbyEnterToClient(sessionID, ERROR_CODE.LOBBY_ENTER_INVALID_STATE);
                    return;
                }

                var reqData = new LobbyEnterReqPacket();
                reqData.Decode(packetData.BodyData);

                var lobby = GetLobby(reqData.LobbyNumber);
                if (lobby == null)
                {
                    ResponseLobbyEnterToClient(sessionID, ERROR_CODE.LOBBY_ENTER_INVALID_ROOM_NUMBER);
                    return;
                }

                if (lobby.AddUser(user.ID, sessionID) == false)
                {
                    ResponseLobbyEnterToClient(sessionID, ERROR_CODE.LOBBY_ENTER_FAIL_ADD_USER);
                    return;
                }


                user.EnteredLobby(reqData.LobbyNumber);

                ResponseLobbyEnterToClient(sessionID, ERROR_CODE.NONE);

                LobbyServer.MainLogger.Debug("RequestLobbyEnter - Success");
            }
            catch (Exception ex)
            {
                LobbyServer.MainLogger.Error(ex.ToString());
            }
        }
                

        public void RequestLobbyLeave(ServerPacketData packetData)
        {
            var sessionID = packetData.SessionID;
            LobbyServer.MainLogger.Debug("RequestLobbyLeave");

            try
            {
                var user = UserMgr.GetUserByNetSessionID(sessionID);
                if(user == null)
                {
                    return;
                }

                if(LeaveLobbyUser(sessionID, user.LobbyNumber) == false)
                {
                    return;
                }

                user.LeaveLobby();

                ResponseLobbyLeaveToClient(sessionID, ERROR_CODE.NONE);

                LobbyServer.MainLogger.Debug("RequestLeave - Success");
            }
            catch (Exception ex)
            {
                LobbyServer.MainLogger.Error(ex.ToString());
            }
        }

        public void NotifyLeaveInternal(ServerPacketData packetData)
        {
            LobbyServer.MainLogger.Debug($"NotifyLeaveInternal. SessionID: {packetData.SessionID}");

            var reqData = MessagePackSerializer.Deserialize<PKTInternalNtfLobbyLeave>(packetData.BodyData);
            LeaveLobbyUser(packetData.SessionID, reqData.LobbyNumber);
        }



        public void RequestLobbyChat(ServerPacketData packetData)
        {
            var sessionID = packetData.SessionID;
            LobbyServer.MainLogger.Debug("RequestChat");

            try
            {
                var lobbyObject = CheckLobbyAndLobbyUser(sessionID);
                if(lobbyObject.Item1 == false)
                {
                    return;
                }

                var reqData = new LobbyChatReqPacket();
                reqData.Decode(packetData.BodyData);

                ResponseLobbyChatToClient(sessionID, ERROR_CODE.NONE);


                var notifyPacket = new LobbyChatNtfPacket()
                {
                    Msg = reqData.Msg,
                };

                lobbyObject.Item2.Broadcast((UInt16)CL_PACKET_ID.NTF_LOBBY_CHAT, notifyPacket.ToBytes());

                LobbyServer.MainLogger.Debug("RequestChat - Success");
            }
            catch (Exception ex)
            {
                LobbyServer.MainLogger.Error(ex.ToString());
            }
        }


        public void RequestLobbyMatch(ServerPacketData packetData)
        {
            var sessionID = packetData.SessionID;
            LobbyServer.MainLogger.Debug("RequestMatch");

            try
            {
                var lobbyObject = CheckLobbyAndLobbyUser(sessionID);
                if (lobbyObject.Item1 == false)
                {
                    return;
                }

                var lobbyUser = lobbyObject.Item3;
                                
                ResponseLobbyMatchToClient(sessionID, ERROR_CODE.NONE);

                var user = new MatchUser();
                user.LobbyNetSessionID = sessionID;
                MatchingSys.Add(user);

                MatchingSys.Process(LobbyServer.ServerOpt.GameServerIP, (UInt16)LobbyServer.ServerOpt.GameServerPort);
            }
            catch (Exception ex)
            {
                LobbyServer.MainLogger.Error(ex.ToString());
            }
        }
        #endregion


        

        #region private member
        Lobby GetLobby(int roomNumber)
        {
            var index = roomNumber - StartRoomNumber;

            if (index < 0 || index >= LobbyList.Count())
            {
                return null;
            }

            return LobbyList[index];
        }

        (bool, Lobby, LobbyUser) CheckLobbyAndLobbyUser(string userNetSessionID)
        {
            var user = UserMgr.GetUserByNetSessionID(userNetSessionID);
            if (user == null)
            {
                return (false, null, null);
            }

            var lobbyNumber = user.LobbyNumber;
            var lobby = GetLobby(lobbyNumber);

            if (lobby == null)
            {
                return (false, null, null);
            }


            var lobbyUser = lobby.GetUser(userNetSessionID);

            if (lobbyUser == null)
            {
                return (false, lobby, null);
            }

            return (true, lobby, lobbyUser);
        }

        void ResponseLobbyEnterToClient(string sessionID, ERROR_CODE errorCode)
        {
            var responsePkt = new LobbyEnterResPacket()
            {
                Result = (short)errorCode
            };

            ServerNetwork.SendData(sessionID, (UInt16)CL_PACKET_ID.RES_LOBBY_ENTER, responsePkt.ToBytes());
        }

        void ResponseLobbyLeaveToClient(string sessionID, ERROR_CODE errorCode)
        {
            var responsePkt = new LobbyLeaveResPacket()
            {
                Result = (short)errorCode
            };

            ServerNetwork.SendData(sessionID, (UInt16)CL_PACKET_ID.RES_LOBBY_LEAVE, responsePkt.ToBytes());
        }

        void ResponseLobbyChatToClient(string sessionID, ERROR_CODE errorCode)
        {
            var responsePkt = new LobbyChatResPacket()
            {
                Result = (short)errorCode
            };
                        
            ServerNetwork.SendData(sessionID, (UInt16)CL_PACKET_ID.RES_LOBBY_CHAT, responsePkt.ToBytes());
        }

        
        bool LeaveLobbyUser(string sessionID, int lobbyNumber)
        {
            LobbyServer.MainLogger.Debug($"LeaveLobbyUser. SessionID:{sessionID}");

            var lobby = GetLobby(lobbyNumber);
            if (lobby == null)
            {
                return false;
            }

            var lobbyUser = lobby.GetUser(sessionID);
            if (lobbyUser == null)
            {
                return false;
            }

            var userID = lobbyUser.UserID;
            lobby.RemoveUser(lobbyUser);

            return true;
        }

        
        void ResponseLobbyMatchToClient(string sessionID, ERROR_CODE errorCode)
        {
            var responsePkt = new LobbyMatchResPacket()
            {
                Result = (short)errorCode
            };

            ServerNetwork.SendData(sessionID, (UInt16)CL_PACKET_ID.RES_LOBBY_MATCH, responsePkt.ToBytes());
        }

        
        #endregion


    }
}
