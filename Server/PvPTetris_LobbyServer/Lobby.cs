using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobbyServer
{
    public class Lobby
    {
        public int Index { get; private set; }
        public int Number { get; private set; }

        int MaxUserCount = 0;

        List<LobbyUser> UserList = new List<LobbyUser>();

        public static Func<string, UInt16, byte[], bool> NetSendFunc;


        public void Init(int index, int number, int maxUserCount)
        {
            Index = index;
            Number = number;
            MaxUserCount = maxUserCount;
        }

        public bool AddUser(string userID, string netSessionID)
        {
            if(GetUser(userID) != null)
            {
                return false;
            }

            var roomUser = new LobbyUser();
            roomUser.Set(userID, netSessionID);
            UserList.Add(roomUser);

            return true;
        }

        public void RemoveUser(string netSessionID)
        {
            var index = UserList.FindIndex(x => x.NetSessionID == netSessionID);
            UserList.RemoveAt(index);
        }

        public bool RemoveUser(LobbyUser user)
        {
            return UserList.Remove(user);
        }

        public LobbyUser GetUserByID(string userID)
        {
            return UserList.Find(x => x.UserID == userID);
        }

        public LobbyUser GetUser(string netSessionID)
        {
            return UserList.Find(x => x.NetSessionID == netSessionID);
        }

        public int CurrentUserCount()
        {
            return UserList.Count();
        }
               
        public void Broadcast(UInt16 packetID, byte[] bodyData)
        {
            foreach(var user in UserList)
            {
                NetSendFunc(user.NetSessionID, packetID, bodyData);
            }
        }

    }


    public class LobbyUser
    {
        public string UserID { get; private set; }
        public string NetSessionID { get; private set; }

        public void Set(string userID, string netSessionID)
        {
            UserID = userID;
            NetSessionID = netSessionID;
        }
    }
}
