using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServerCommon;

namespace LobbyServer
{
    public class UserManager
    {
        int MaxUserCount;
       UInt64 UserSequenceNumber = 0;

        Dictionary<string, User> UserNetSessionIDMap = new Dictionary<string, User>();
        Dictionary<string, User> UserIDMap = new Dictionary<string, User>();

        public void Init(int maxUserCount)
        {
            MaxUserCount = maxUserCount;
        }

        public ERROR_CODE AddUser(string sessionID)
        {
            if(IsFullUserCount())
            {
                return ERROR_CODE.LOGIN_FULL_USER_COUNT;
            }

            if (UserNetSessionIDMap.ContainsKey(sessionID))
            {
                return ERROR_CODE.ADD_USER_NET_SESSION_ID_DUPLICATION;
            }


            ++UserSequenceNumber;
            
            var user = new User();
            user.SetNetInfo(UserSequenceNumber, sessionID);
            UserNetSessionIDMap.Add(sessionID, user);
            
            return ERROR_CODE.NONE;
        }

        
        public ERROR_CODE RemoveUser(string netSessionID)
        {
            var user = GetUserByNetSessionID(netSessionID);

            if(user == null)
            {
                return ERROR_CODE.REMOVE_USER_SEARCH_FAILURE_USER_ID;
            }

            UserNetSessionIDMap.Remove(netSessionID);

            if (user.IsAuthenticated)
            {
                UserIDMap.Remove(user.ID);
            }
            
            return ERROR_CODE.NONE;
        }

        public User GetUserByNetSessionID(string netSessionID)
        {
            UserNetSessionIDMap.TryGetValue(netSessionID, out var user);
            return user;
        }

        bool IsFullUserCount()
        {
            return MaxUserCount <= UserNetSessionIDMap.Count();
         }
                
    }

    public class User
    {
        UInt64 SequenceNumber = 0;
        public string NetSessionID { get; private set; }
        public int LobbyNumber { get; private set; } = -1;
        public string ID { get; private set; }

        public bool IsPreAuthenticated { get; private set; }
        public bool IsAuthenticated { get; private set; }

        public void SetNetInfo(UInt64 sequence, string sessionID)
        {
            SequenceNumber = sequence;
            NetSessionID = sessionID;
        }

        public void SetPreAuthenticated()
        {
            IsPreAuthenticated = true;
        }

        public void SetAuthenticatedUser(string userID)
        {
            IsPreAuthenticated = false;
            IsAuthenticated = true;
            ID = userID;
        }               
       
        public void EnteredLobby(int lobbyNumber)
        {
            LobbyNumber = lobbyNumber;
        }

        public void LeaveLobby()
        {
            LobbyNumber = -1;
        }
                
        public bool IsStateLobby() { return LobbyNumber != -1; }
    }
    
}
