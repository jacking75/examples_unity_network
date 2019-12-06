using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace LobbyServer
{
    class MatchingSystem
    {
        string GameServerIP = "127.0.0";
        UInt16 GameServerPort = 11021;

        Int32 RoomNumber = -1;

        bool IsRunning = false;

        ConcurrentQueue<MatchUser> MatchUserQueue = new ConcurrentQueue<MatchUser>();

        public Func<string, UInt16, byte[], bool> SendPacket;

                
        
        public void Add(MatchUser data)
        {
            MatchUserQueue.Enqueue(data);
        }


        public void Process()
        {
            var mqDataEncodingBuffer = new byte[8012];

            while (IsRunning)
            {
                if(MatchUserQueue.Count < 2)
                {
                    System.Threading.Thread.Sleep(1);
                    continue;
                }

                MatchUser user1, user2;
                if (MatchUserQueue.TryDequeue(out user1) == false)
                {
                    continue;
                }

                if (MatchUserQueue.TryDequeue(out user2) == false)
                {
                    continue;
                }


                MatchingComplete(user1, user2);
            }
        }

        void MatchingComplete(MatchUser user1, MatchUser user2)
        {
            // 사용할 수 있는 방을 얻는다
            ++RoomNumber;

            // 유저들이 있는 로비서버에 통보한다
            NotifyLobbyMatchToClient(user1.LobbyNetSessionID, GameServerIP, GameServerPort, RoomNumber);
            NotifyLobbyMatchToClient(user2.LobbyNetSessionID, GameServerIP, GameServerPort, RoomNumber);
                       
        }

        void NotifyLobbyMatchToClient(string sessionID, string ip, UInt16 port, Int32 roomNumber)
        {
            var notifyPkt = new LobbyMatchNtfPacket()
            {
                IP = ip,
                Port = port,
                RoomNumber = roomNumber
            };

            SendPacket(sessionID, (UInt16)CL_PACKET_ID.NTF_LOBBY_MATCH, notifyPkt.ToBytes());
        }
    }

    class MatchUser
    {
        public string LobbyNetSessionID;
    }
}
