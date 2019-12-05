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
        REQ_RES_TEST_ECHO = 101,
        
               
        // 클라이언트
        CS_BEGIN        = 1001,

        REQ_LOGIN       = 1002,
        RES_LOGIN       = 1003,
        NTF_MUST_CLOSE       = 1005,

        REQ_ROOM_ENTER = 1015,
        RES_ROOM_ENTER = 1016,
        NTF_ROOM_USER_LIST = 1017,
        NTF_ROOM_NEW_USER = 1018,

        REQ_ROOM_LEAVE = 1021,
        RES_ROOM_LEAVE = 1022,
        NTF_ROOM_LEAVE_USER = 1023,

        REQ_ROOM_CHAT = 1026,
        NTF_ROOM_CHAT = 1027,

        CS_END          = 1100,


        // 시스템, 서버 - 서버
        SS_START    = 8001,

        NTF_IN_CONNECT_CLIENT = 8011,
        NTF_IN_DISCONNECT_CLIENT = 8012,

        REQ_SS_SERVERINFO = 8021,
        RES_SS_SERVERINFO = 8023,

        REQ_IN_ROOM_ENTER = 8031,
        RES_IN_ROOM_ENTER = 8032,

        NTF_IN_ROOM_LEAVE = 8036,


        // DB 8101 ~ 9000
        REQ_DB_LOGIN = 8101,
        RES_DB_LOGIN = 8102,
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
            var packetIDbuf = BitConverter.GetBytes((UInt16)PACKETID.REQ_RES_TEST_ECHO);

            Data = new byte[PacketDef.PACKET_HEADER_SIZE];

            Buffer.BlockCopy(BitConverter.GetBytes(totalSize), 0, Data, 0, 2);
            Buffer.BlockCopy(packetIDbuf, 0, Data, 2, 2);
            Buffer.BlockCopy(bodyData, 0, Data, 5, bodyData.Length);
        }

        
    }
}
