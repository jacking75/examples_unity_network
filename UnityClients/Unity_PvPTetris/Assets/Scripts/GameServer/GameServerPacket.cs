using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace GameNetwork
{

    public enum PACKET_ID : ushort
    {
        INVALID = 0,

        SYSTEM_CLIENT_CONNECT = 11,
        SYSTEM_CLIENT_DISCONNECTD = 12,

        #region DevPacket
        DEV_ECHO_REQ = 101,
        DEV_ECHO_RES = 102,
        #endregion

        REQ_GAME_LOGIN = 302,
        RES_GAME_LOGIN = 303,

        REQ_ROOM_ENTER = 306,
        RES_ROOM_ENTER = 307,

        REQ_ROOM_LEAVE = 311,
        RES_ROOM_LEAVE = 312,

        REQ_ROOM_CHAT = 316,
        RES_ROOM_CHAT = 317,
        NTF_ROOM_CHAT = 318,

        REQ_GAME_START = 321,
        RES_GAME_START = 322,
        NTF_GAME_START = 323,

        REQ_GAME_SYNC = 326,
        NTF_GAME_SYNC = 327,

        REQ_GAME_END = 341,
        RES_GAME_END = 342,
        NTF_GAME_END = 343,

        //TODO 방에서 매칭된 상대정보를 가져오는 패킷. 이후 매칭서버와 통신구조에 따라 패킷의 형식이 달라질 수 있음
        //RivalUserInfoNtf = 220,

        //GameStartReqPkt = 301,
        //GameStartResPkt = 302,
        //GameStartNtfPkt = 303,

        //GameSyncReqPkt = 304,
        //GameSyncNtfPkt = 305,

        //GameScoreUpdateReqPkt = 306,
        //GameScoreUpdateNtfPkt = 307,
        //GameScoreUpdateResPkt = 308,

        //GameEndReqPkt = 311,
        //GameEndResPkt = 312,
        //GameEndNtfPkt = 313,
    }

    public enum ERROR_CODE : ushort
    {
        NONE = 0,

        USER_MGR_INVALID_USER_INDEX = 11,
        USER_MGR_INVALID_USER_UNIQUEID = 12,

        LOGIN_USER_ALREADY = 31,
        LOGIN_USER_USED_ALL_OBJ = 32,

        NEW_ROOM_USED_ALL_OBJ = 41,
        NEW_ROOM_FAIL_ENTER = 42,

        ENTER_ROOM_NOT_FINDE_USER = 51,
        ENTER_ROOM_INVALID_USER_STATUS = 52,
        ENTER_ROOM_NOT_USED_STATUS = 53,
        ENTER_ROOM_FULL_USER = 54,

        ROOM_INVALID_INDEX = 61,
        ROOM_NOT_USED = 62,
        ROOM_TOO_MANY_PACKET = 63,

        LEAVE_ROOM_INVALID_ROOM_INDEX = 71,

        CHAT_ROOM_INVALID_ROOM_INDEX = 81,
    }

    public enum EVENT_TYPE : Int16  {
        //구현상 편의를 위해 Spawner클래스의 SpawnGroup과 Index를 맞춤
        NONE = -1,
        SPAWN_GROUP_I = 0,
        SPAWN_GROUP_J = 1,
        SPAWN_GROUP_L = 2,
        SPAWN_GROUP_O = 3,
        SPAWN_GROUP_S = 4,
        SPAWN_GROUP_T = 5,
        SPAWN_GROUP_Z = 6,
        MOVE_LEFT = 7,
        MOVE_RIGHT = 8,
        MOVE_DOWN = 9,
        ROTATE = 10,
        DELETE_ROW = 11,

    }



    public class PacketDataValue
    {
        public const int USER_ID_LENGTH = 20;
        public const int USER_PW_LENGTH = 20;
        public const int MAX_CHAT_SIZE = 257;
    }

    //struct PacketData
    //{
    //    public Int16 DataSize;
    //    public Int16 PacketID;
    //    public SByte Type;
    //    public byte[] BodyData;
    //}

    public class LoginReqPacket
    {
        public string UserID;
                
        public byte[] ToBytes()
        {
            return Encoding.UTF8.GetBytes(UserID);
        }

        public void Decode(byte[] bodyData)
        {
            UserID = Encoding.UTF8.GetString(bodyData);
        }
    }

    public class LoginResPacket
    {
        public Int16 Result;

        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(Result);
        }

        public void Decode(byte[] bodyData)
        {
            Result = BitConverter.ToInt16(bodyData, 0);
        }
    }

    public class RoomEnterReqPacket
    {
        public int RoomNumber;

        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(RoomNumber);
        }

        public void Decode(byte[] bodyData)
        {
            RoomNumber = BitConverter.ToInt32(bodyData, 0);
        }
    }

    public class RoomEnterResPacket
    {
        public Int16 Result;
        public string RivalUserID;

        public byte[] ToBytes()
        {
            List<byte> dataSource = new List<byte>();
            dataSource.AddRange(BitConverter.GetBytes(Result));
            dataSource.AddRange(Encoding.UTF8.GetBytes(RivalUserID));

            return dataSource.ToArray();
        }

        public void Decode(byte[] bodyData)
        {
            var idLen = bodyData.Length - 2;

            Result = BitConverter.ToInt16(bodyData, 0);
            RivalUserID = Encoding.UTF8.GetString(bodyData, 2, idLen);
        }
    }


    public class RoomLeaveResPacket
    {
        public Int16 Result;

        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(Result);
        }

        public void Decode(byte[] bodyData)
        {
            Result = BitConverter.ToInt16(bodyData, 0);
        }
    }



    public class RoomChatReqPacket
    {
        public string Msg;

        public byte[] ToBytes()
        {
            return Encoding.UTF8.GetBytes(Msg);
        }

        public void Decode(byte[] bodyData)
        {
            Msg = Encoding.UTF8.GetString(bodyData);
        }
    }

    public class RoomChatResPacket
    {
        public Int16 Result;

        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(Result);
        }

        public void Decode(byte[] bodyData)
        {
            Result = BitConverter.ToInt16(bodyData, 0);
        }
    }

    public class RoomChatNtfPacket
    {
        public string Msg;

        public byte[] ToBytes()
        {
            return Encoding.UTF8.GetBytes(Msg);
        }

        public void Decode(byte[] bodyData)
        {
            Msg = Encoding.UTF8.GetString(bodyData);
        }
    }


    //REQ_GAME_START

    public class GameStartResPacket
    {
        public Int16 Result;

        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(Result);
        }

        public void Decode(byte[] bodyData)
        {
            Result = BitConverter.ToInt16(bodyData, 0);
        }
    }

    //NTF_GAME_START


    //REQ_GAME_END
    public class GameEndRequestPacket
    {
    }

    public class GameEndResPacket
    {
        public Int16 Result;

        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(Result);
        }

        public void Decode(byte[] bodyData)
        {
            Result = BitConverter.ToInt16(bodyData, 0);
        }
    }

    //NTF_GAME_END


    public class GameSyncReqPacket
    {
        public Int16[] EventRecordArr6 = new Int16[6];
        public Int32 Score;
        public Int32 Line;
        public Int32 Level;

        public byte[] ToBytes()
        {
            byte[] EventRecordArr6Buf = new byte[EventRecordArr6.Length * sizeof(Int16)];
            Buffer.BlockCopy(EventRecordArr6, 0, EventRecordArr6Buf, 0, EventRecordArr6Buf.Length);

            List<byte> dataSource = new List<byte>();
            dataSource.AddRange(EventRecordArr6Buf);
            dataSource.AddRange(BitConverter.GetBytes(Score));
            dataSource.AddRange(BitConverter.GetBytes(Line));
            dataSource.AddRange(BitConverter.GetBytes(Level));
            return dataSource.ToArray();
        }

        public void Decode(byte[] bodyData)
        {
            Buffer.BlockCopy(bodyData, 0, EventRecordArr6, 0, EventRecordArr6.Length);

            var pos = EventRecordArr6.Length * sizeof(Int16);
            Score = BitConverter.ToInt32(bodyData, pos);
            pos += 4;
            Line = BitConverter.ToInt32(bodyData, pos);
            pos += 4;
            Level = BitConverter.ToInt32(bodyData, pos);
        }
    }

    public class GameSyncNtfPacket
    {
        public Int16[] EventRecordArr6 = new Int16[6];
        public Int32 Score;
        public Int32 Line;
        public Int32 Level;

        public byte[] ToBytes()
        {
            byte[] EventRecordArr6Buf = new byte[EventRecordArr6.Length * sizeof(Int16)];
            Buffer.BlockCopy(EventRecordArr6, 0, EventRecordArr6Buf, 0, EventRecordArr6Buf.Length);

            List<byte> dataSource = new List<byte>();
            dataSource.AddRange(EventRecordArr6Buf);
            dataSource.AddRange(BitConverter.GetBytes(Score));
            dataSource.AddRange(BitConverter.GetBytes(Line));
            dataSource.AddRange(BitConverter.GetBytes(Level));
            return dataSource.ToArray();
        }

        public void Decode(byte[] bodyData)
        {
            Buffer.BlockCopy(bodyData, 0, EventRecordArr6, 0, EventRecordArr6.Length);

            var pos = EventRecordArr6.Length * sizeof(Int16);
            Score = BitConverter.ToInt32(bodyData, pos);
            pos += 4;
            Line = BitConverter.ToInt32(bodyData, pos);
            pos += 4;
            Level = BitConverter.ToInt32(bodyData, pos);
        }
    }


}
