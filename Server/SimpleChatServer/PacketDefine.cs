using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
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
          
    }

    // 1 ~ 10000
    public enum PACKETID : UInt16
    {
        NTF_IN_CONNECT_CLIENT = 11,
        NTF_IN_DISCONNECT_CLIENT = 12, 

        SIMPLE_CHAT = 103,                
    }



    class PacketDef
    {
        public const Int16 HEADER_SIZE = 5;
        public const int MAX_USER_ID_BYTE_LENGTH = 16;
        public const int MAX_USER_PW_BYTE_LENGTH = 16;
    }



    public class SimpleChatPacket
    {
        public byte[] Data;

        public void SetValue(byte[] bodyData)
        {
            var totalSize = (UInt16)(PacketDef.HEADER_SIZE + bodyData.Length);
            var packetIDbuf = BitConverter.GetBytes((UInt16)PACKETID.SIMPLE_CHAT);

            Data = new byte[PacketDef.HEADER_SIZE + bodyData.Length];

            Buffer.BlockCopy(BitConverter.GetBytes(totalSize), 0, Data, 0, 2);
            Buffer.BlockCopy(packetIDbuf, 0, Data, 2, 2);
            Buffer.BlockCopy(bodyData, 0, Data, 5, bodyData.Length);
        }
    }

}
