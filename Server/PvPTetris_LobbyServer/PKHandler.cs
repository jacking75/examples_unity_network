using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LobbyServer
{
    public class PKHandler
    {
        protected LobbyServer ServerNetwork;
        protected UserManager UserMgr = null;

        protected Action<byte[]> SendToDBserver;
        protected Action<byte[]> SendToMatchServer;


        public void Init(LobbyServer serverNetwork, UserManager userMgr)
        {
            ServerNetwork = serverNetwork;
            UserMgr = userMgr;
        }      
                
    }
}
