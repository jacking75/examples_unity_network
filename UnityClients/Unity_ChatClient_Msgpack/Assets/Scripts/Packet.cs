using MessagePack;
using System.Collections;
using System.Collections.Generic;



namespace CSMsgPackPacket
{
    [MessagePackObject]
    public class ChatReqPkt
    {
        [Key(0)]
        public string UserID { get; set; }

        [Key(1)]
        public string Msg { get; set; }
    }

    [MessagePackObject]
    public class ChatNtfPkt
    {
        [Key(0)]
        public string UserID { get; set; }

        [Key(1)]
        public string Msg { get; set; }
    }
}