using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_test_client
{
    public partial class mainForm
    {
        Dictionary<PACKET_ID, Action<byte[]>> PacketFuncDic = new Dictionary<PACKET_ID, Action<byte[]>>();

        void SetPacketHandler()
        {
            PacketFuncDic.Add(PACKET_ID.PACKET_ID_ECHO, PacketProcess_Echo);
            PacketFuncDic.Add(PACKET_ID.PACKET_ID_SIMPLE_CHAT, PacketProcess_SimpleChat);

            PacketFuncDic.Add(PACKET_ID.RES_LOBBY_LOGIN, PacketProcess_LobbyLoginRes);
            PacketFuncDic.Add(PACKET_ID.RES_LOBBY_ENTER, PacketProcess_LobbyEnterRes);
            PacketFuncDic.Add(PACKET_ID.RES_LOBBY_LEAVE, PacketProcess_LobbyLeaveRes);
            PacketFuncDic.Add(PACKET_ID.RES_LOBBY_CHAT, PacketProcess_LobbyChatRes);
            PacketFuncDic.Add(PACKET_ID.NTF_LOBBY_CHAT, PacketProcess_LobbyChatNotify);
            PacketFuncDic.Add(PACKET_ID.RES_LOBBY_MATCH, PacketProcess_LobbyMatchRes);
            PacketFuncDic.Add(PACKET_ID.NTF_LOBBY_MATCH, PacketProcess_LobbyMatchNotify);

            PacketFuncDic.Add(PACKET_ID.RES_GAME_LOGIN, PacketProcess_GameLoginRes);
            PacketFuncDic.Add(PACKET_ID.RES_ROOM_ENTER, PacketProcess_RoomEnterRes);
            PacketFuncDic.Add(PACKET_ID.RES_ROOM_LEAVE, PacketProcess_RoomLeaveRes);
            PacketFuncDic.Add(PACKET_ID.RES_ROOM_CHAT, PacketProcess_RoomChatRes);          
            PacketFuncDic.Add(PACKET_ID.NTF_ROOM_CHAT, PacketProcess_RoomChatNotify);
            PacketFuncDic.Add(PACKET_ID.RES_GAME_START, PacketProcess_GameStartRes);
            PacketFuncDic.Add(PACKET_ID.NTF_GAME_START, PacketProcess_GameStartNotify);
            PacketFuncDic.Add(PACKET_ID.NTF_GAME_SYNC, PacketProcess_GameSyncNotify);
            PacketFuncDic.Add(PACKET_ID.RES_GAME_END, PacketProcess_GameEndRes);
            PacketFuncDic.Add(PACKET_ID.NTF_GAME_END, PacketProcess_GameEndNotify);
        }

        void PacketProcess(ClientNetLib.PacketData packet)
        {
            var packetType = (PACKET_ID)packet.PacketID;
            //DevLog.Write("Packet Error:  PacketID:{packet.PacketID.ToString()},  Error: {(ERROR_CODE)packet.Result}");
            //DevLog.Write("RawPacket: " + packet.PacketID.ToString() + ", " + PacketDump.Bytes(packet.BodyData));

            if (PacketFuncDic.ContainsKey(packetType))
            {
                PacketFuncDic[packetType](packet.BodyData);
            }
            else
            {
                DevLog.Write("Unknown Packet Id: " + packet.PacketID.ToString());
            }         
        }

        void PacketProcess_Echo(byte[] bodyData)
        {
            DevLog.Write($"Echo 받음:  {bodyData.Length}");
        }

        void PacketProcess_SimpleChat(byte[] bodyData)
        {
            var stringData = Encoding.UTF8.GetString(bodyData);
            DevLog.Write($"SimpleChat 받음: {stringData}");
        }

        void PacketProcess_ErrorNotify(byte[] bodyData)
        {
            var notifyPkt = new ErrorNtfPacket();
            notifyPkt.FromBytes(bodyData);

            DevLog.Write($"에러 통보 받음:  {notifyPkt.Error}");
        }


        void PacketProcess_LobbyLoginRes(byte[] bodyData)
        {
            var responsePkt = new LoginResPacket();
            responsePkt.Decode(bodyData);

            DevLog.Write($"로그인 결과:  {(ERROR_CODE)responsePkt.Result}");

            if ((ERROR_CODE)responsePkt.Result == ERROR_CODE.NONE)
            {
                labelStatus.Text = "로비에 로그인 완료";
            }
        }

        void PacketProcess_LobbyEnterRes(byte[] bodyData)
        {
            var responsePkt = new LobbyEnterResPacket();
            responsePkt.Decode(bodyData);

            DevLog.Write($"로비 입장 결과:  {(ERROR_CODE)responsePkt.Result}");

            if ((ERROR_CODE)responsePkt.Result == ERROR_CODE.NONE)
            {
                labelStatus.Text = "로비에 입장 완료";
            }
        }

        void PacketProcess_LobbyLeaveRes(byte[] bodyData)
        {
            var responsePkt = new LobbyLeaveResPacket();
            responsePkt.Decode(bodyData);

            DevLog.Write($"로비 나가기 결과:  {(ERROR_CODE)responsePkt.Result}");

            if ((ERROR_CODE)responsePkt.Result == ERROR_CODE.NONE)
            {
                labelStatus.Text = "로비에서 나간 상태";
            }
        }

        void PacketProcess_LobbyChatRes(byte[] bodyData)
        {
            var responsePkt = new LobbyChatResPacket();
            responsePkt.Decode(bodyData);

            DevLog.Write($"로비 채팅 요청 결과:  {(ERROR_CODE)responsePkt.Result}");
        }

        void PacketProcess_LobbyChatNotify(byte[] bodyData)
        {
            var responsePkt = new LobbyChatNtfPacket();
            responsePkt.Decode(bodyData);

            DevLog.Write($"로비 채팅:  {responsePkt.Msg}");
        }

        void PacketProcess_LobbyMatchRes(byte[] bodyData)
        {
            var responsePkt = new LobbyMatchResPacket();
            responsePkt.Decode(bodyData);

            DevLog.Write($"매칭 요청 결과:  {(ERROR_CODE)responsePkt.Result}");

            if ((ERROR_CODE)responsePkt.Result == ERROR_CODE.NONE)
            {
                labelStatus.Text = "매칭 요청 중인 상태";
            }
        }

        void PacketProcess_LobbyMatchNotify(byte[] bodyData)
        {
            var responsePkt = new LobbyMatchNtfPacket();
            responsePkt.Decode(bodyData);

            DevLog.Write($"매칭 통보 받음: {responsePkt.GameServerIP}, {responsePkt.GameServerPort}");
            labelStatus.Text = "게임 서버에 접속해야 한다";
        }


        void PacketProcess_GameLoginRes(byte[] bodyData)
        {
            var responsePkt = new GameServerLoginResPacket();
            responsePkt.Decode(bodyData);

            DevLog.Write($"게임 서버에 로그인 결과:  {(ERROR_CODE)responsePkt.Result}");

            if ((ERROR_CODE)responsePkt.Result == ERROR_CODE.NONE)
            {
                labelStatus.Text = "게임 서버에 로그인 완료";
            }
        }

        void PacketProcess_RoomEnterRes(byte[] bodyData)
        {
            var responsePkt = new RoomEnterResPacket();
            responsePkt.Decode(bodyData);

            DevLog.Write($"게임 서버 방 입장 요청 결과:  {(ERROR_CODE)responsePkt.Result}");

            if ((ERROR_CODE)responsePkt.Result == ERROR_CODE.NONE)
            {
                labelStatus.Text = "게임 방에 입장 완료";
            }
        }
                        
        void PacketProcess_RoomLeaveRes(byte[] bodyData)
        {
            var responsePkt = new RoomLeaveResPacket();
            responsePkt.Decode(bodyData);

            DevLog.Write($"게임 서버 방 나가기 요청 결과:  {(ERROR_CODE)responsePkt.Result}");

            if ((ERROR_CODE)responsePkt.Result == ERROR_CODE.NONE)
            {
                labelStatus.Text = "게임 방을 나간 상태";
            }
        }
            
        void PacketProcess_RoomChatRes(byte[] bodyData)
        {
            var responsePkt = new RoomChatResPacket();
            responsePkt.Decode(bodyData);

            DevLog.Write($"게임 서버 방 채팅 요청 결과:  {(ERROR_CODE)responsePkt.Result}");            
        }
        
        void PacketProcess_RoomChatNotify(byte[] bodyData)
        {
            var responsePkt = new RoomChatNtfPacket();
            responsePkt.Decode(bodyData);

            AddRoomChatMessageList(responsePkt.Msg);
        }

        void PacketProcess_GameStartRes(byte[] bodyData)
        {
            var responsePkt = new GameStartResPacket();
            responsePkt.Decode(bodyData);

            DevLog.Write($"게임 서버 게임 시작 요청 결과:  {(ERROR_CODE)responsePkt.Result}");
        }

        void PacketProcess_GameStartNotify(byte[] bodyData)
        {
            DevLog.Write("게임 서버 게임 시작 상태");
        }
        
        void PacketProcess_GameSyncNotify(byte[] bodyData)
        {
            DevLog.Write($"GameSyncReqPacket 받음:");
        }

        void PacketProcess_GameEndRes(byte[] bodyData)
        {
            var responsePkt = new GameEndResPacket();
            responsePkt.Decode(bodyData);

            DevLog.Write($"게임 서버 게임 완료 요청 결과:  {(ERROR_CODE)responsePkt.Result}");

            if ((ERROR_CODE)responsePkt.Result == ERROR_CODE.NONE)
            {
                labelStatus.Text = "게임 완료 상태";
            }
        }

        void PacketProcess_GameEndNotify(byte[] bodyData)
        {
            labelStatus.Text = "게임 완료 상태";
        }









        void AddRoomChatMessageList(string msgssage)
        {
            if (listBoxRoomChatMsg.Items.Count > 512)
            {
                listBoxRoomChatMsg.Items.Clear();
            }

            listBoxRoomChatMsg.Items.Add(msgssage);
            listBoxRoomChatMsg.SelectedIndex = listBoxRoomChatMsg.Items.Count - 1;
        }
    }
}
