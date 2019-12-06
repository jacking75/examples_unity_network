using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading.Tasks.Dataflow;

using ServerCommon;

namespace LobbyServer
{
    class PacketProcessor
    {
        bool IsThreadRunning = false;
        System.Threading.Thread ProcessThread = null;

        //receive쪽에서 처리하지 않아도 Post에서 블럭킹 되지 않는다. 
        //BufferBlock<T>(DataflowBlockOptions) 에서 DataflowBlockOptions의 BoundedCapacity로 버퍼 가능 수 지정. 
        //BoundedCapacity 보다 크게 쌓이면 블럭킹 된다. default 값은 -1이고 무한대이다.
        BufferBlock<ServerPacketData> MsgBuffer = new BufferBlock<ServerPacketData>();

        UserManager UserMgr = new UserManager();

        Tuple<int,int> LobbyNumberRange = new Tuple<int, int>(-1, -1);
        List<Lobby> LobbyList = new List<Lobby>();

        Dictionary<int, Action<ServerPacketData>> PacketHandlerMap = new Dictionary<int, Action<ServerPacketData>>();
        PKHCommon CommonPacketHandler = new PKHCommon();
        PKHLobby LobbyPacketHandler = new PKHLobby();
                

        //TODO MainServer를 인자로 주지말고, func을 인자로 넘겨주는 것이 좋다
        public void CreateAndStart(List<Lobby> lobbyList, LobbyServer mainServer)
        {
            var maxUserCount = LobbyServer.ServerOption.LobbyMaxCount * LobbyServer.ServerOption.LobbyMaxUserCount;
            UserMgr.Init(maxUserCount);

            LobbyList = lobbyList;
            var minlobbyNum = LobbyList[0].Number;
            var maxlobbyNum = LobbyList[0].Number + LobbyList.Count() - 1;
            LobbyNumberRange = new Tuple<int, int>(minlobbyNum, maxlobbyNum);
            
            RegistPacketHandler(mainServer);

            IsThreadRunning = true;
            ProcessThread = new System.Threading.Thread(this.Process);
            ProcessThread.Start();
        }
        
        public void Destory()
        {
            IsThreadRunning = false;
            MsgBuffer.Complete();
        }
              
        public void InsertPacket(ServerPacketData data)
        {
            MsgBuffer.Post(data);
        }

        public void SetMQ(Action<byte[]> sendToDBserverFunc, Action<byte[]> sendToMatchServerFunc)
        {
            CommonPacketHandler.SetMq(sendToDBserverFunc, sendToMatchServerFunc);
            LobbyPacketHandler.SetMq(sendToDBserverFunc, sendToMatchServerFunc);
        }


        void RegistPacketHandler(LobbyServer serverNetwork)
        {            
            CommonPacketHandler.Init(serverNetwork, UserMgr);
            CommonPacketHandler.RegistPacketHandler(PacketHandlerMap);            

            LobbyPacketHandler.Init(serverNetwork, UserMgr);
            LobbyPacketHandler.SetLobbyList(LobbyList);
            LobbyPacketHandler.RegistPacketHandler(PacketHandlerMap);
        }

        void Process()
        {
            while (IsThreadRunning)
            {
                //System.Threading.Thread.Sleep(64); //테스트 용
                try
                {
                    var packet = MsgBuffer.Receive();

                    if (PacketHandlerMap.ContainsKey(packet.PacketID))
                    {
                        PacketHandlerMap[packet.PacketID](packet);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("세션 번호 {0}, PacketID {1}, 받은 데이터 크기: {2}", packet.SessionID, packet.PacketID, packet.BodyData.Length);
                    }
                }
                catch (Exception ex)
                {
                    IsThreadRunning.IfTrue(() => LobbyServer.MainLogger.Error(ex.ToString()));
                }
            }
        }


        //Test 기능
        //public void DevMQTest()
        //{
        //    LobbyPacketHandler.DevTest();
        //}
    }
}
