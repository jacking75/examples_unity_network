using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LobbyServer
{
    public class ServerPacketData
    {
        public UInt16 PacketSize;
        public string SessionID;         
        public UInt16 PacketID;        
        public SByte Type;
        public byte[] BodyData;
                
        
        public void Assign(string sessionID, UInt16 packetID, byte[] packetBodyData)
        {
            SessionID = sessionID;

            PacketID = packetID;
            
            if (packetBodyData.Length > 0)
            {
                BodyData = packetBodyData;
            }
        }
                
        public static ServerPacketData MakeNTFInConnectOrDisConnectClientPacket(bool isConnect, string sessionID)
        {
            var packet = new ServerPacketData();
            
            if (isConnect)
            {
                packet.PacketID = (UInt16)SYS_PACKET_ID.NTF_IN_CONNECT_CLIENT;
            }
            else
            {
                packet.PacketID = (UInt16)SYS_PACKET_ID.NTF_IN_DISCONNECT_CLIENT;
            }

            packet.SessionID = sessionID;
            return packet;
        }               
        
    }


    public class PKTInternalNtfLobbyLeave
    {
        public int LobbyNumber;
        public string UserID;

        public byte[] Encode()
        {
            List<byte> dataSource = new List<byte>();
            dataSource.AddRange(BitConverter.GetBytes((UInt32)LobbyNumber));
            dataSource.AddRange(Encoding.UTF8.GetBytes(UserID));
            return dataSource.ToArray();
        }

        public void Decode(byte[] bodyData)
        {
            LobbyNumber = BitConverter.ToInt32(bodyData, 0);
            UserID = Encoding.UTF8.GetString(bodyData, 4, (bodyData.Length - 4));
        }
    }

    

}
