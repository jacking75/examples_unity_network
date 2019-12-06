using MessagePack; 
//https://github.com/neuecc/MessagePack-CSharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobbyServer
{    
    [MessagePackObject]
    public class PKTMsgPackHead
    {
        [Key(0)]
        public Byte[] Head = new Byte[5];
    }


    [MessagePackObject]
    public class PKTReqLobbyLogin : PKTMsgPackHead
    {
        [Key(1)]
        public string UserID { get; set; }
        [Key(2)]
        public string AuthToken { get; set; }
    }

    [MessagePackObject]
    public class PKTResLobbyLogin : PKTMsgPackHead
    {
        [Key(1)]
        public short Result;
    }


    [MessagePackObject]
    public class PKNtfLobbyMustClose : PKTMsgPackHead
    {
        [Key(1)]
        public short Result;
    }



    [MessagePackObject]
    public class PKTReqLobbyEnter : PKTMsgPackHead
    {
        [Key(1)]
        public int LobbyNumber;
    }

    [MessagePackObject]
    public class PKTResLobbyEnter : PKTMsgPackHead
    {
        [Key(1)]
        public short Result;
    }



    [MessagePackObject]
    public class PKTReqLobbyLeave
    {
    }

    [MessagePackObject]
    public class PKTResLobbyLeave : PKTMsgPackHead
    {
        [Key(1)]
        public short Result;
    }



    [MessagePackObject]
    public class PKTReqLobbyChat : PKTMsgPackHead
    {
        [Key(1)]
        public string ChatMessage;
    }

    [MessagePackObject]
    public class PKTResLobbyChat : PKTMsgPackHead
    {
        [Key(1)]
        public short Result;
    }

    [MessagePackObject]
    public class PKTNtfLobbyChat : PKTMsgPackHead
    {
        [Key(1)]
        public string UserID;

        [Key(2)]
        public string ChatMessage;
    }



    [MessagePackObject]
    public class PKTReqLobbyMatch : PKTMsgPackHead
    {
        [Key(1)]
        public int Dummy;
    }

    [MessagePackObject]
    public class PKTResLobbyMatch : PKTMsgPackHead
    {
        [Key(1)]
        public short Result;
    }

    [MessagePackObject]
    public class PKTNtfLobbyMatch : PKTMsgPackHead
    {
        [Key(1)]
        public string GameServerIP;
        [Key(2)]
        public UInt16 GameServerPort;
        [Key(3)]
        public Int32 RoomNumber;
    }


    public class PacketDef
    {
        public const Int16 PACKET_HEADER_SIZE = 5;
        public const Int16 PACKET_HEADER_SIZE_OF_MSGPACK = 8;
        public const Int16 PACKET_HEADER_MSGPACK_START_POS = 3;

        public const int MAX_USER_ID_BYTE_LENGTH = 20;
        public const int MAX_USER_PW_BYTE_LENGTH = 20;

        public const int INVALID_LOBBY_NUMBER = -1;


        static public void SetHeadInfo(byte[] packetData, UInt16 packetId, UInt16 size)
        {
            ServerCommon.FastBinaryWrite.UInt16(packetData, PACKET_HEADER_MSGPACK_START_POS, size);
            ServerCommon.FastBinaryWrite.UInt16(packetData, (PACKET_HEADER_MSGPACK_START_POS + 2), packetId);
        }
    }
}
