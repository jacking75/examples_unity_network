using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_test_client
{
    class PacketDef
    {
        public const int MAX_USER_ID_BYTE_LENGTH = 16;
        public const int MAX_USER_PW_BYTE_LENGTH = 16;
    }

    public enum PACKET_ID : ushort
    {
        PACKET_ID_ECHO = 101,
        PACKET_ID_SIMPLE_CHAT = 103,

        // To LobbyServer
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


        // To GameServer
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


    public enum ERROR_CODE : Int16
    {
        ERROR_NONE = 0,



        ERROR_CODE_USER_MGR_INVALID_USER_UNIQUEID = 112,

        ERROR_CODE_PUBLIC_CHANNEL_IN_USER = 114,

        ERROR_CODE_PUBLIC_CHANNEL_INVALIDE_NUMBER = 115,
    }
}
