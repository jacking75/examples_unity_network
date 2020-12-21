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
        ClientNetLib.TransportTCP TcpNetLobbyServer = new ClientNetLib.TransportTCP();

        bool IsBackGroundProcessRunning = false;

        System.Windows.Threading.DispatcherTimer dispatcherUITimer;


        public mainForm()
        {
            InitializeComponent();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            TcpNetLobbyServer.DebugPrintFunc = WriteDebugLog;
            
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

            TcpNetLobbyServer.Disconnect();
        }

        
        void WriteDebugLog(string msg)
        {
            DevLog.Write(msg, LOG_LEVEL.DEBUG);
        }

                

        void Update(object sender, EventArgs e)
        {
            ProcessLog();

            try
            {
                if (TcpNetLobbyServer.IsConnected == false)
                {
                    return;
                }

                var packetList = TcpNetLobbyServer.GetPacket();

                foreach(var packet in packetList)
                {
                    if (packet.PacketID == ClientNetLib.PacketDef.SysPacketIDDisConnectdFromServer)
                    {
                        SetDisconnectd();
                        DevLog.Write("서버와 접속 종료 !!!", LOG_LEVEL.INFO);
                    }
                    else
                    {
                        PacketProcess(packet);
                    }
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
            
            labelStatus.Text = "서버 접속이 끊어짐";
        }

        public void PostSendPacket(PACKET_ID packetID, byte[] bodyData)
        {
            if (TcpNetLobbyServer.IsConnected == false)
            {
                DevLog.Write("서버 연결이 되어 있지 않습니다", LOG_LEVEL.ERROR);
                return;
            }

            Int16 bodyDataSize = 0;
            if (bodyData != null)
            {
                bodyDataSize = (Int16)bodyData.Length;
            }
            var packetSize = bodyDataSize + ClientNetLib.PacketDef.PACKET_HEADER_SIZE;

            List<byte> dataSource = new List<byte>();
            dataSource.AddRange(BitConverter.GetBytes((UInt16)packetSize));
            dataSource.AddRange(BitConverter.GetBytes((UInt16)packetID));
            dataSource.AddRange(new byte[] { (byte)0 });
            
            if (bodyData != null)
            {
                dataSource.AddRange(bodyData);
            }

            TcpNetLobbyServer.Send(dataSource.ToArray());
        }

             
        private void btnConnect_Click(object sender, EventArgs e)
        {
            string address = textBoxIP.Text;

            if (checkBoxLocalHostIP.Checked)
            {
                address = "127.0.0.1";
            }

            int port = Convert.ToInt32(textBoxPort.Text);

            if (TcpNetLobbyServer.Connect(address, port))
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
            TcpNetLobbyServer.Disconnect();
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

        // 간이 채팅
        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textSendText.Text))
            {
                MessageBox.Show("보낼 텍스트를 입력하세요");
                return;
            }

            string message = $"[{DateTime.Now.ToString("HH:mm:ss")}] {textBoxUserID.Text}: {textSendText.Text}";
            var body = Encoding.UTF8.GetBytes(message);

            PostSendPacket(PACKET_ID.PACKET_ID_SIMPLE_CHAT, body);

            DevLog.Write($"Simple Chat 요청:  {textSendText.Text}, {body.Length}");
        }

        
    }
}
