using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MessagePack;



namespace LobbyServer
{
    public class PKHCommon : PKHandler
    {
        public void RegistPacketHandler(Dictionary<int, Action<ServerPacketData>> packetHandlerMap)
        {            
            packetHandlerMap.Add((int)SYS_PACKET_ID.NTF_IN_CONNECT_CLIENT, NotifyInConnectClient);
            packetHandlerMap.Add((int)SYS_PACKET_ID.NTF_IN_DISCONNECT_CLIENT, NotifyInDisConnectClient);

            packetHandlerMap.Add((int)CL_PACKET_ID.REQ_GAME_LOGIN, RequestLogin);
        }

        public void NotifyInConnectClient(ServerPacketData requestData)
        {
            var errorCode = UserMgr.AddUser(requestData.SessionID);
            
            if (errorCode != ERROR_CODE.NONE)
            {
                GameServer.MainLogger.Error($"Fail AddUser. NetSessionID: {requestData.SessionID}");

                //TODO: 클라이언트에게 에러를 보낸다. 그리고 이 유저가 지정된 시간까지 나가지 않는 경우 제거하도록 한다.

                return;
            }

            GameServer.MainLogger.Debug($"AddUser. NetSessionID: {requestData.SessionID}");
            return;
        }

        public void NotifyInDisConnectClient(ServerPacketData requestData)
        {
            var user = UserMgr.GetUserByNetSessionID(requestData.SessionID);
            
            if (user != null)
            {
                var lobbyNum = user.RoomNumber;

                if (lobbyNum != PacketDef.INVALID_LOBBY_NUMBER)
                {
                    var packet = new PKTInternalNtfRoomLeave()
                    {
                        RoomNumber = lobbyNum,
                        UserID = user.ID,
                    };

                    var packetBodyData = MessagePackSerializer.Serialize(packet);
                    var internalPacket = new ServerPacketData();
                    internalPacket.Assign(requestData.SessionID, (UInt16)SYS_PACKET_ID.NTF_IN_ROOM_LEAVE, packetBodyData);

                    ServerNetwork.Distribute(internalPacket);
                }

                UserMgr.RemoveUser(requestData.SessionID);
            }
                        
            GameServer.MainLogger.Debug($"Current Connected Session Count: {ServerNetwork.SessionCount}");
        }


        public void RequestLogin(ServerPacketData packetData)
        {
            var sessionID = packetData.SessionID;
            GameServer.MainLogger.Debug("로그인 요청 받음");

            try
            {
                // 중복 체크만 한다

                var user = UserMgr.GetUserByNetSessionID(sessionID);
                
                var requestPkt = new LoginReqPacket();
                requestPkt.Decode(packetData.BodyData);

                user.SetAuthenticatedUser(requestPkt.UserID);

                ResponseLoginToClient(ERROR_CODE.NONE, sessionID);
                GameServer.MainLogger.Debug("로그인 완료");

            }
            catch(Exception ex)
            {
                // 패킷 해제에 의해서 로그가 남지 않도록 로그 수준을 Debug로 한다.
                GameServer.MainLogger.Debug(ex.ToString());
            }
        }

              
        void ResponseLoginToClient(ERROR_CODE errorCode, string sessionID)
        {
            var resLogin = new LoginResPacket()
            {
                Result = (short)errorCode
            };

            ServerNetwork.SendData(sessionID, (UInt16)CL_PACKET_ID.RES_GAME_LOGIN, resLogin.ToBytes());
        }             

    }
}
