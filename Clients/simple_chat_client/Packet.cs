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
        public string GameServerIP;
        public UInt16 GameServerPort;
        public Int32 RoomNumber;
        //public string Info; //ip__port__roomNumber

        public byte[] ToBytes()
        {
            var temp = $"{GameServerIP}__{GameServerPort}__{RoomNumber}";
            return Encoding.UTF8.GetBytes(temp);
        }

        public void Decode(byte[] bodyData)
        {
            string[] separatingStrings = { "__" };

            var dataFormat = Encoding.UTF8.GetString(bodyData);
            var elements = dataFormat.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);

            GameServerIP = elements[0];
            GameServerPort = UInt16.Parse(elements[1]);
            RoomNumber = Int32.Parse(elements[2]);
        }
    }



    // To GameServer
    public class GameServerLoginReqPacket
    {
        public string UserID;
                
        public byte[] ToBytes()
        {
            return Encoding.UTF8.GetBytes(UserID);
        }

        public void Decode(byte[] bodyData)
        {
            UserID = Encoding.UTF8.GetString(bodyData);
        }
    }

    public class GameServerLoginResPacket
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

    public class RoomEnterReqPacket
    {
        public int RoomNumber;

        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(RoomNumber);
        }

        public void Decode(byte[] bodyData)
        {
            RoomNumber = BitConverter.ToInt32(bodyData, 0);
        }
    }

    public class RoomEnterResPacket
    {
        public Int16 Result;
        public string RivalUserID;

        public byte[] ToBytes()
        {
            List<byte> dataSource = new List<byte>();
            dataSource.AddRange(BitConverter.GetBytes(Result));
            dataSource.AddRange(Encoding.UTF8.GetBytes(RivalUserID));

            return dataSource.ToArray();
        }

        public void Decode(byte[] bodyData)
        {
            var idLen = bodyData.Length - 2;

            Result = BitConverter.ToInt16(bodyData, 0);
            RivalUserID = Encoding.UTF8.GetString(bodyData, 2, idLen);
        }
    }


    public class RoomLeaveResPacket
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



    public class RoomChatReqPacket
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

    public class RoomChatResPacket
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

    public class RoomChatNtfPacket
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


    //REQ_GAME_START

    public class GameStartResPacket
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

    //NTF_GAME_START


    //REQ_GAME_END

    public class GameEndResPacket
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

    //NTF_GAME_END


    public class GameSyncReqPacket
    {
        public Int16[] EventRecordArr6 = new Int16[6];
        public Int32 Score;
        public Int32 Line;
        public Int32 Level;

        public byte[] ToBytes()
        {
            byte[] EventRecordArr6Buf = new byte[EventRecordArr6.Length * sizeof(Int16)];
            Buffer.BlockCopy(EventRecordArr6, 0, EventRecordArr6Buf, 0, EventRecordArr6Buf.Length);

            List<byte> dataSource = new List<byte>();
            dataSource.AddRange(EventRecordArr6Buf);
            dataSource.AddRange(BitConverter.GetBytes(Score));
            dataSource.AddRange(BitConverter.GetBytes(Line));
            dataSource.AddRange(BitConverter.GetBytes(Level));
            return dataSource.ToArray();
        }

        public void Decode(byte[] bodyData)
        {
            Buffer.BlockCopy(bodyData, 0, EventRecordArr6, 0, EventRecordArr6.Length);

            var pos = EventRecordArr6.Length * sizeof(Int16);
            Score = BitConverter.ToInt32(bodyData, pos);
            pos += 4;
            Line = BitConverter.ToInt32(bodyData, pos);
            pos += 4;
            Level = BitConverter.ToInt32(bodyData, pos);
        }
    }

    public class GameSyncNtfPacket
    {
        Int16[] EventRecordArr6 = new Int16[6];
        Int32 Score;
        Int32 Line;
        Int32 Level;

        public byte[] ToBytes()
        {
            byte[] EventRecordArr6Buf = new byte[EventRecordArr6.Length * sizeof(Int16)];
            Buffer.BlockCopy(EventRecordArr6, 0, EventRecordArr6Buf, 0, EventRecordArr6Buf.Length);

            List<byte> dataSource = new List<byte>();
            dataSource.AddRange(EventRecordArr6Buf);
            dataSource.AddRange(BitConverter.GetBytes(Score));
            dataSource.AddRange(BitConverter.GetBytes(Line));
            dataSource.AddRange(BitConverter.GetBytes(Level));
            return dataSource.ToArray();
        }

        public void Decode(byte[] bodyData)
        {
            Buffer.BlockCopy(bodyData, 0, EventRecordArr6, 0, EventRecordArr6.Length);

            var pos = EventRecordArr6.Length * sizeof(Int16);
            Score = BitConverter.ToInt32(bodyData, pos);
            pos += 4;
            Line = BitConverter.ToInt32(bodyData, pos);
            pos += 4;
            Level = BitConverter.ToInt32(bodyData, pos);
        }
    }
}
