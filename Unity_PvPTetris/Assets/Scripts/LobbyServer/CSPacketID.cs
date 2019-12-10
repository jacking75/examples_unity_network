using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobbyServer
{    
    // 클라이언트 - 로비 패킷 ID
    public enum CL_PACKET_ID : int
    {
        REQ_RES_TEST_ECHO = 101,

        CS_BEGIN = 201,

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

        CS_END          = 300,
    }



    public enum ERROR_CODE : Int16
    {
        NONE = 0, // 에러가 아니다

        // 서버 초기화 에라
        REDIS_INIT_FAIL = 1,    // Redis 초기화 에러

        // 로그인 
        LOGIN_FULL_USER_COUNT = 201,
        ADD_USER_NET_SESSION_ID_DUPLICATION = 202,
        REMOVE_USER_SEARCH_FAILURE_USER_ID = 203,

        LOBBY_ENTER_INVALID_USER = 211,
        LOBBY_ENTER_INVALID_STATE = 212,
        LOBBY_ENTER_INVALID_ROOM_NUMBER = 214,
        LOBBY_ENTER_FAIL_ADD_USER = 215,
    }


}
