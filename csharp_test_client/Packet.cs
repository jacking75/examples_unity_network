using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_test_client
{
    public class PacketDump
    {
        public static string Bytes(byte[] byteArr)
        {
            StringBuilder sb = new StringBuilder("[");
            for (int i = 0; i < byteArr.Length; ++i)
            {
                sb.Append(byteArr[i] + " ");
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
    

    public class ErrorNtfPacket
    {
        public ERROR_CODE Error;

        public bool FromBytes(byte[] bodyData)
        {
            Error = (ERROR_CODE)BitConverter.ToInt16(bodyData, 0);
            return true;
        }
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
            var dataFormat = Encoding.UTF8.GetString(bodyData);
            var elements = dataFormat.Split("__");

            IP = elements[0];
            Port = elements[1].ToUInt16();
            RoomNumber = elements[2].ToInt32();
        }
    }
}
