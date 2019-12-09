using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobbyServer
{
    // 301 ~ 500
    public enum ERROR_CODE : short
    {
        NONE                        = 0, // 에러가 아니다
                
        LOGIN_FULL_USER_COUNT = 301,
        ADD_USER_NET_SESSION_ID_DUPLICATION = 302,
        REMOVE_USER_SEARCH_FAILURE_USER_ID = 303,

        ROOM_ENTER_INVALID_USER = 311,
        ROOM_ENTER_INVALID_STATE = 312,
        ROOM_ENTER_INVALID_ROOM_NUMBER = 314,
        ROOM_ENTER_FAIL_ADD_USER = 315,
        ROOM_ENTER_INVALID_ROOM_STATE = 316,
        ROOM_ENTER_INVALID_ROOM_USER_COUNT = 317,

        GAME_START_INVALID_ROOM_STATE = 321,
        GAME_START_INVALID_ROOM_USER_COUNT = 322,
    }

    public enum SYS_PACKET_ID : UInt16
    {
        NTF_IN_CONNECT_CLIENT = 21,
        NTF_IN_DISCONNECT_CLIENT = 22,

        NTF_IN_ROOM_LEAVE = 51,
    }



    // 301 ~ 350
    public enum CL_PACKET_ID : UInt16
    {
        CS_BEGIN        = 301,

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
    }

    

    class PacketDef
    {
        public const Int16 PACKET_HEADER_SIZE = 5;

        public const int INVALID_LOBBY_NUMBER = -1;

        public const int MAX_USER_ID_BYTE_LENGTH = 16;
        public const int MAX_USER_PW_BYTE_LENGTH = 16;
    }


    public class LoginReqPacket
    {
        public string UserID;
        
        public void SetValue(string userID)
        {
            UserID = userID;
        }

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

        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(Result);
        }

        public void Decode(byte[] bodyData)
        {
            Result = BitConverter.ToInt16(bodyData, 0);
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
        Int16[] EventRecordArr6 = new Int16[6];
        Int32 Score;
        Int32 Line;
        Int32 Level;

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
        Int16[] EventRecordArr6 = new Int16[6];
        Int32 Score;
        Int32 Line;
        Int32 Level;

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
