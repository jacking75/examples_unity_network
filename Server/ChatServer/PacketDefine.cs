using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSBaseLib
{
    // 0 ~ 9999
    public enum ERROR_CODE : short
    {
        NONE                        = 0, // 에러가 아니다

        // 서버 초기화 에라
        REDIS_INIT_FAIL             = 1,    // Redis 초기화 에러

        // 로그인 
        LOGIN_INVALID_AUTHTOKEN             = 1001, // 로그인 실패: 잘못된 인증 토큰
        ADD_USER_DUPLICATION                = 1002,
        REMOVE_USER_SEARCH_FAILURE_USER_ID  = 1003,
        USER_AUTH_SEARCH_FAILURE_USER_ID    = 1004,
        USER_AUTH_ALREADY_SET_AUTH          = 1005,
        LOGIN_ALREADY_WORKING = 1006,
        LOGIN_FULL_USER_COUNT = 1007,

        DB_LOGIN_INVALID_PASSWORD   = 1011,
        DB_LOGIN_EMPTY_USER         = 1012,
        DB_LOGIN_EXCEPTION          = 1013,

        ROOM_ENTER_INVALID_STATE = 1021,
        ROOM_ENTER_INVALID_USER = 1022,
        ROOM_ENTER_ERROR_SYSTEM = 1023,
        ROOM_ENTER_INVALID_ROOM_NUMBER = 1024,
        ROOM_ENTER_FAIL_ADD_USER = 1025,
    }

    // 1 ~ 10000
    public enum PACKETID : UInt16
    {
        NTF_IN_CONNECT_CLIENT = 11,
        NTF_IN_DISCONNECT_CLIENT = 12, 


        PACKET_ID_ECHO = 101,
        PACKET_ID_SIMPLE_CHAT = 103,

        PACKET_ID_CHAT_REQ = 105,
        PACKET_ID_CHAT_NTF = 106,
                
    }



    class PacketDef
    {
        public const Int16 PACKET_HEADER_SIZE = 5;
        public const int MAX_USER_ID_BYTE_LENGTH = 16;
        public const int MAX_USER_PW_BYTE_LENGTH = 16;
    }



    public class EchoResPacket
    {
        public byte[] Data;
        
        public void SetValue(byte[] bodyData)
        {
            var totalSize = (UInt16)(PacketDef.PACKET_HEADER_SIZE + bodyData.Length);
            var packetIDbuf = BitConverter.GetBytes((UInt16)PACKETID.PACKET_ID_ECHO);

            Data = new byte[PacketDef.PACKET_HEADER_SIZE + bodyData.Length];

            Buffer.BlockCopy(BitConverter.GetBytes(totalSize), 0, Data, 0, 2);
            Buffer.BlockCopy(packetIDbuf, 0, Data, 2, 2);
            Buffer.BlockCopy(bodyData, 0, Data, 5, bodyData.Length);
        }        
    }

    public class SimpleChatPacket
    {
        public byte[] Data;

        public void SetValue(byte[] bodyData)
        {
            var totalSize = (UInt16)(PacketDef.PACKET_HEADER_SIZE + bodyData.Length);
            var packetIDbuf = BitConverter.GetBytes((UInt16)PACKETID.PACKET_ID_SIMPLE_CHAT);

            Data = new byte[PacketDef.PACKET_HEADER_SIZE + bodyData.Length];

            Buffer.BlockCopy(BitConverter.GetBytes(totalSize), 0, Data, 0, 2);
            Buffer.BlockCopy(packetIDbuf, 0, Data, 2, 2);
            Buffer.BlockCopy(bodyData, 0, Data, 5, bodyData.Length);
        }
    }


    public class MakePacket
    {
        static public byte[] Create(PACKETID packetId, byte[] bodyData)
        {
            var totalSize = (UInt16)(PacketDef.PACKET_HEADER_SIZE + bodyData.Length);
            var packetIDbuf = BitConverter.GetBytes((UInt16)packetId);

            var packet = new byte[PacketDef.PACKET_HEADER_SIZE + bodyData.Length];

            Buffer.BlockCopy(BitConverter.GetBytes(totalSize), 0, packet, 0, 2);
            Buffer.BlockCopy(packetIDbuf, 0, packet, 2, 2);
            Buffer.BlockCopy(bodyData, 0, packet, 5, bodyData.Length);

            return packet;
        }
    }

}
