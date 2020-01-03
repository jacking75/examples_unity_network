using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace LobbyServer
{
    class MatchingSystem
    {
        Int32 RoomNumber = -1;

        ConcurrentQueue<MatchUser> MatchUserQueue = new ConcurrentQueue<MatchUser>();

        public Func<string, UInt16, byte[], bool> SendPacket;

                
        
        public void Add(MatchUser data)
        {
            MatchUserQueue.Enqueue(data);
        }


        public bool Process(string gameServerIP, UInt16 gameServerPort)
        {
            if(MatchUserQueue.Count < 2)
            {
                System.Threading.Thread.Sleep(1);
                return false;
            }

            MatchUser user1, user2;
            if (MatchUserQueue.TryDequeue(out user1) == false)
            {
                return false;
            }

            if (MatchUserQueue.TryDequeue(out user2) == false)
            {
                return false;
            }


            MatchingComplete(user1, user2, gameServerIP, gameServerPort);
            return true;
        }

        void MatchingComplete(MatchUser user1, MatchUser user2, string gameServerIP, UInt16 gameServerPort)
        {
            // 사용할 수 있는 방을 얻는다
            ++RoomNumber;
            
            // 유저들이 있는 로비서버에 통보한다
            NotifyLobbyMatchToClient(user1.LobbyNetSessionID, gameServerIP, gameServerPort, RoomNumber);
            NotifyLobbyMatchToClient(user2.LobbyNetSessionID, gameServerIP, gameServerPort, RoomNumber);
                       
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
