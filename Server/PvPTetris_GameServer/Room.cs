using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobbyServer
{
    public class Room
    {
        public int Index { get; private set; }
        public int Number { get; private set; }

        public bool IsGamming = false;


        int MaxUserCount = 0;

        List<RoomUser> UserList = new List<RoomUser>();

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

            var roomUser = new RoomUser();
            roomUser.Set(userID, netSessionID);
            UserList.Add(roomUser);

            return true;
        }

        public void RemoveUser(string netSessionID)
        {
            var index = UserList.FindIndex(x => x.NetSessionID == netSessionID);
            UserList.RemoveAt(index);
        }

        public bool RemoveUser(RoomUser user)
        {
            return UserList.Remove(user);
        }

        public RoomUser GetUserByID(string userID)
        {
            return UserList.Find(x => x.UserID == userID);
        }

        public RoomUser GetUser(string netSessionID)
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


    public class RoomUser
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
