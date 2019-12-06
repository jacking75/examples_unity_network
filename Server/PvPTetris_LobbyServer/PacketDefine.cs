using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobbyServer
{
    // 0 ~ 9999
    public enum ERROR_CODE : short
    {
        NONE                        = 0, // 에러가 아니다

        // 서버 초기화 에라
        REDIS_INIT_FAIL             = 1,    // Redis 초기화 에러

        LOGIN_FULL_USER_COUNT = 101,
        ADD_USER_NET_SESSION_ID_DUPLICATION = 102,
        REMOVE_USER_SEARCH_FAILURE_USER_ID = 103,

        LOBBY_ENTER_INVALID_USER = 111,
        LOBBY_ENTER_INVALID_STATE = 112,
        LOBBY_ENTER_INVALID_ROOM_NUMBER = 114,
        LOBBY_ENTER_FAIL_ADD_USER = 115,
    }

    public enum SYS_PACKET_ID : UInt16
    {
        NTF_IN_CONNECT_CLIENT = 21,
        NTF_IN_DISCONNECT_CLIENT = 22,

        NTF_IN_LOBBY_LEAVE = 51,
    }



    // 101 ~ 1000
    public enum CL_PACKET_ID : UInt16
    {
        CS_BEGIN        = 201,

        REQ_LOBBY_LOGIN = 202,
        RES_LOBBY_LOGIN = 203,

        REQ_LOBBY_ENTER = 206,
        RES_LOBBY_ENTER = 207,

        REQ_LOBBY_LEAVE = 211,
        RES_LOBBY_LEAVE = 212,

        REQ_LOBBY_CHAT = 216,
        RES_LOBBY_CHAT = 217,
        NTF_LOBBY_CHAT = 218,

        REQ_LOBBY_MATCH = 221,
        RES_LOBBY_MATCH = 222,
        NTF_LOBBY_MATCH = 223,
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

    public class LobbyEnterReqPacket
    {
        public int LobbyNumber;
        
        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(LobbyNumber);
        }

        public void Decode(byte[] bodyData)
        {
            LobbyNumber = BitConverter.ToInt32(bodyData, 0);
        }
    }

    public class LobbyEnterResPacket
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


    public class LobbyLeaveResPacket
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



    public class LobbyChatReqPacket
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

    public class LobbyChatResPacket
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

    public class LobbyChatNtfPacket
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



    public class LobbyMatchResPacket
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


    public class LobbyMatchNtfPacket
    {
        public string IP;
        public UInt16 Port;
        public Int32 RoomNumber;
        //public string Info; //ip__port__roomNumber

        public byte[] ToBytes()
        {
            var temp = $"{IP}__{Port}__{RoomNumber}";
            return Encoding.UTF8.GetBytes(temp);
        }

        public void Decode(byte[] bodyData)
        {
            //bodyData를 __로 나눈다
        }
    }
}
