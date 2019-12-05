using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace csharp_test_client
{
    public partial class mainForm : Form
    {
        NetLib.TransportTCP TcpTransport = new NetLib.TransportTCP();

        bool IsBackGroundProcessRunning = false;

        PacketBufferManager PacketBuffer = new PacketBufferManager();
        
        System.Windows.Threading.DispatcherTimer dispatcherUITimer;


        public mainForm()
        {
            InitializeComponent();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            PacketBuffer.Init((8096 * 10), PacketDef.PACKET_HEADER_SIZE, 1024);
                        
            IsBackGroundProcessRunning = true;
            dispatcherUITimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherUITimer.Tick += new EventHandler(Update);
            dispatcherUITimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherUITimer.Start();

            btnDisconnect.Enabled = false;

            SetPacketHandler();
            DevLog.Write("프로그램 시작 !!!", LOG_LEVEL.INFO);
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            IsBackGroundProcessRunning = false;

            TcpTransport.Disconnect();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string address = textBoxIP.Text;

            if (checkBoxLocalHostIP.Checked)
            {
                address = "127.0.0.1";
            }

            int port = Convert.ToInt32(textBoxPort.Text);

            if (TcpTransport.Connect(address, port))
            {
                labelStatus.Text = string.Format("{0}. 서버에 접속 중", DateTime.Now);
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;

                DevLog.Write($"서버에 접속 중", LOG_LEVEL.INFO);
            }
            else
            {
                labelStatus.Text = string.Format("{0}. 서버에 접속 실패", DateTime.Now);
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            SetDisconnectd();
            TcpTransport.Disconnect();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textSendText.Text))
            {
                MessageBox.Show("보낼 텍스트를 입력하세요");
                return;
            }

            var body = Encoding.UTF8.GetBytes(textSendText.Text);

            PostSendPacket(PACKET_ID.PACKET_ID_ECHO, body);

            DevLog.Write($"Echo 요청:  {textSendText.Text}, {body.Length}");
        }



        PacketData NetworkReadProcess()
        {
            const Int16 PacketHeaderSize = PacketDef.PACKET_HEADER_SIZE;
            
            byte[] buffer = null;
            var result = TcpTransport.Receive(out buffer);
            if(result == false)
            {
                return default(PacketData);
            }

            if (buffer.Length > 1)
            {
                PacketBuffer.Write(buffer, 0, buffer.Length);

                var data = PacketBuffer.Read();
                if (data.Count < 1)
                {
                    return default(PacketData);
                }

                var packet = new PacketData();
                packet.DataSize = (UInt16)(data.Count - PacketHeaderSize);
                packet.PacketID = BitConverter.ToUInt16(data.Array, data.Offset + 2);
                packet.Type = (SByte)data.Array[(data.Offset + 4)];
                packet.BodyData = new byte[packet.DataSize];
                Buffer.BlockCopy(data.Array, (data.Offset + PacketHeaderSize), packet.BodyData, 0, (data.Count - PacketHeaderSize));

                return packet;
                //DevLog.Write($"받은 데이터: {recvData.Item2}", LOG_LEVEL.INFO);
            }
            

            SetDisconnectd();
            DevLog.Write("서버와 접속 종료 !!!", LOG_LEVEL.INFO);            
            return default(PacketData);
        }
                

        void Update(object sender, EventArgs e)
        {
            ProcessLog();

            try
            {
                if (TcpTransport.IsConnected == false)
                {
                    return;
                }

                var packet = NetworkReadProcess();

                if (packet.PacketID != 0)
                {
                    PacketProcess(packet);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("ReadPacketQueueProcess. error:{0}", ex.Message));
            }
        }

        private void ProcessLog()
        {
            // 너무 이 작업만 할 수 없으므로 일정 작업 이상을 하면 일단 패스한다.
            int logWorkCount = 0;

            while (IsBackGroundProcessRunning)
            {
                System.Threading.Thread.Sleep(1);

                string msg;

                if (DevLog.GetLog(out msg))
                {
                    ++logWorkCount;

                    if (listBoxLog.Items.Count > 512)
                    {
                        listBoxLog.Items.Clear();
                    }

                    listBoxLog.Items.Add(msg);
                    listBoxLog.SelectedIndex = listBoxLog.Items.Count - 1;
                }
                else
                {
                    break;
                }

                if (logWorkCount > 8)
                {
                    break;
                }
            }
        }


        public void SetDisconnectd()
        {
            if (btnConnect.Enabled == false)
            {
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
            }

            listBoxRoomChatMsg.Items.Clear();
            listBoxRoomUserList.Items.Clear();

            labelStatus.Text = "서버 접속이 끊어짐";
        }

        public void PostSendPacket(PACKET_ID packetID, byte[] bodyData)
        {
            if (TcpTransport.IsConnected == false)
            {
                DevLog.Write("서버 연결이 되어 있지 않습니다", LOG_LEVEL.ERROR);
                return;
            }

            Int16 bodyDataSize = 0;
            if (bodyData != null)
            {
                bodyDataSize = (Int16)bodyData.Length;
            }
            var packetSize = bodyDataSize + PacketDef.PACKET_HEADER_SIZE;

            List<byte> dataSource = new List<byte>();
            dataSource.AddRange(BitConverter.GetBytes((UInt16)packetSize));
            dataSource.AddRange(BitConverter.GetBytes((UInt16)packetID));
            dataSource.AddRange(new byte[] { (byte)0 });
            
            if (bodyData != null)
            {
                dataSource.AddRange(bodyData);
            }

            TcpTransport.Send(dataSource.ToArray());
        }

        void AddRoomUserList(Int64 userUniqueId, string userID)
        {
            var msg = $"{userUniqueId}: {userID}";
            listBoxRoomUserList.Items.Add(msg);
        }

        void RemoveRoomUserList(Int64 userUniqueId)
        {
            object removeItem = null;

            foreach( var user in listBoxRoomUserList.Items)
            {
                var items = user.ToString().Split(":");
                if( items[0].ToInt64() == userUniqueId)
                {
                    removeItem = user;
                    return;
                }
            }

            if (removeItem != null)
            {
                listBoxRoomUserList.Items.Remove(removeItem);
            }
        }


        // 로그인 요청
        private void button2_Click(object sender, EventArgs e)
        {
            //var loginReq = new LoginReqPacket();
            //loginReq.SetValue(textBoxUserID.Text, textBoxUserPW.Text);
                    
            //PostSendPacket(PACKET_ID.PACKET_ID_LOGIN_REQ, loginReq.ToBytes());            
            //DevLog.Write($"로그인 요청:  {textBoxUserID.Text}, {textBoxUserPW.Text}");
        }

        private void btn_RoomEnter_Click(object sender, EventArgs e)
        {
            //var requestPkt = new RoomEnterReqPacket();
            //requestPkt.SetValue(textBoxRoomNumber.Text.ToInt32());

            //PostSendPacket(PACKET_ID.PACKET_ID_ROOM_ENTER_REQ, requestPkt.ToBytes());
            //DevLog.Write($"방 입장 요청:  {textBoxRoomNumber.Text} 번");
        }

        private void btn_RoomLeave_Click(object sender, EventArgs e)
        {
            //PostSendPacket(PACKET_ID.PACKET_ID_ROOM_LEAVE_REQ,  null);
            //DevLog.Write($"방 입장 요청:  {textBoxRoomNumber.Text} 번");
        }

        private void btnRoomChat_Click(object sender, EventArgs e)
        {
            //if(textBoxRoomSendMsg.Text.IsEmpty())
            //{
            //    MessageBox.Show("채팅 메시지를 입력하세요");
            //    return;
            //}

            //var requestPkt = new RoomChatReqPacket();
            //requestPkt.SetValue(textBoxRoomSendMsg.Text);

            //PostSendPacket(PACKET_ID.PACKET_ID_ROOM_CHAT_REQ, requestPkt.ToBytes());
            //DevLog.Write($"방 채팅 요청");
        }

        private void btnRoomRelay_Click(object sender, EventArgs e)
        {
            //if( textBoxRelay.Text.IsEmpty())
            //{
            //    MessageBox.Show("릴레이 할 데이터가 없습니다");
            //    return;
            //}
            
            //var bodyData = Encoding.UTF8.GetBytes(textBoxRelay.Text);
            //PostSendPacket(PACKET_ID.PACKET_ID_ROOM_RELAY_REQ, bodyData);
            //DevLog.Write($"방 릴레이 요청");
        }
    }
}
