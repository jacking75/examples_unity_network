using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;


public class Chat : MonoBehaviour
{
	private NetLib.TransportTCP m_transport;


	private ChatState m_state = ChatState.HOST_TYPE_SELECT;

	private string m_hostAddress = "";

	private const int m_port = 32452;

	private string m_NickName = "test1";

	private string m_sendComment = "";
	private string m_prevComment = "";

	private string m_chatMessage = "";

	private List<string> m_message;

	public Texture texture_title = null;
	public Texture texture_bg = null;

	// 말 풍선 표시용 텍스처.
	public Texture texture_main = null;
	public Texture texture_belo = null;
	public Texture texture_kado_lu = null;
	public Texture texture_kado_ru = null;
	public Texture texture_kado_ld = null;
	public Texture texture_kado_rd = null;
	public Texture texture_tofu = null;
	public Texture texture_daizu = null;

	private static float KADO_SIZE = 16.0f;
	private static float FONT_SIZE = 13.0f;
	private static float FONG_HEIGHT = 18.0f;
	private static int MESSAGE_LINE = 18;

	enum ChatState
	{
		HOST_TYPE_SELECT = 0,   // 방 선택.
		CHATTING,               // 채팅 중.
		LEAVE,                  // 나가기.
		ERROR,                  // 오류.
	};



	// Use this for initialization
	void Start()
	{
		//IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
		//System.Net.IPAddress hostAddress = hostEntry.AddressList[0];
		//Debug.Log(hostEntry.HostName);
		m_hostAddress = "127.0.0.1";

		m_transport = new NetLib.TransportTCP();
		m_transport.DebugPrintFunc = Debug.Log;
		m_transport.Start();

		m_message = new List<string>();
	}

	// Update is called once per frame
	void Update()
	{
		switch (m_state)
		{
			case ChatState.HOST_TYPE_SELECT:
				m_message.Clear();
				break;

			case ChatState.CHATTING:
				UpdateChatting();
				break;

			case ChatState.LEAVE:
				UpdateLeave();
				break;
		}
	}

	void UpdateChatting()
	{
		var packet = m_transport.GetPacket();

		if (packet.PacketID == (UInt16)PACKET_ID.PACKET_ID_SIMPLE_CHAT)
		{
			var responseBody = CSMsgPackPacket.ChatNtfPkt.Parser.ParseFrom(packet.BodyData);
			
			string message = $"[{responseBody.UserID}] {responseBody.Msg}";
			
			Debug.Log("Recv data:" + message);

			m_chatMessage += message + "   ";// + "\n";
			AddMessage(ref m_message, message);
		}
		else if (packet.PacketID == NetLib.PacketDef.SysPacketIDDisConnectdFromServer)
		{
			AddMessage(ref m_message, "서버와 접속이 끊어졌습니다");
		}
	}

	void UpdateLeave()
	{
		m_transport.Disconnect();

		m_message.Clear();

		m_state = ChatState.HOST_TYPE_SELECT;
	}

	void OnGUI()
	{
		switch (m_state)
		{
			case ChatState.HOST_TYPE_SELECT:
				GUI.DrawTexture(new Rect(0, 0, 800, 600), this.texture_title);
				SelectHostTypeGUI();
				break;

			case ChatState.CHATTING:
				GUI.DrawTexture(new Rect(0, 0, 800, 600), this.texture_bg);
				ChattingGUI();
				break;

			case ChatState.ERROR:
				GUI.DrawTexture(new Rect(0, 0, 800, 600), this.texture_title);
				ErrorGUI();
				break;
		}
	}


	void SelectHostTypeGUI()
	{
		float sx = 800.0f;
		float sy = 600.0f;
		float px = sx * 0.5f - 100.0f;
		float py = sy * 0.75f;

		Rect labelRect = new Rect(px, py + 90, 200, 30);
		GUIStyle style = new GUIStyle();
		style.fontStyle = FontStyle.Bold;
		style.normal.textColor = Color.white;
		GUI.Label(labelRect, "서버 IP 주소", style);

		Rect textRect = new Rect(px + 80, py + 80, 70, 30);
		m_hostAddress = GUI.TextField(textRect, m_hostAddress);


		Rect labelRect2 = new Rect(px, py + 125, 200, 30);
		GUIStyle style2 = new GUIStyle();
		style2.fontStyle = FontStyle.Bold;
		style2.normal.textColor = Color.white;
		GUI.Label(labelRect2, "유저 닉네임", style);

		Rect textRect2 = new Rect(px + 80, py + 110, 70, 30);
		m_NickName = GUI.TextField(textRect2, m_NickName);

		if (GUI.Button(new Rect(px, py + 40, 200, 30), "채팅방 들어가기"))
		{
			bool ret = m_transport.Connect(m_hostAddress, m_port);

			if (ret)
			{
				m_state = ChatState.CHATTING;
				AddMessage(ref m_message, "서버에 접속 했습니다");
			}
			else
			{
				m_state = ChatState.ERROR;
			}
		}
	}

	void ChattingGUI()
	{
		Rect commentRect = new Rect(220, 450, 300, 30);
		m_sendComment = GUI.TextField(commentRect, m_sendComment, 15);

		bool isSent = GUI.Button(new Rect(530, 450, 100, 30), "말하기");
		if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
		{
			if (m_sendComment == m_prevComment)
			{
				isSent = true;
				m_prevComment = "";
			}
			else
			{
				m_prevComment = m_sendComment;
			}
		}

		if (isSent == true)
		{
			var requestPkt = new CSMsgPackPacket.ChatReqPkt();
			requestPkt.UserID = m_NickName;
			requestPkt.Msg = m_sendComment;

			System.IO.MemoryStream reqStream = new System.IO.MemoryStream();
			var output = new Google.Protobuf.CodedOutputStream(reqStream);
			requestPkt.WriteTo(output); 

			PostSendPacket(PACKET_ID.PACKET_ID_SIMPLE_CHAT, reqStream.ToArray());
			
			m_sendComment = "";
		}


		if (GUI.Button(new Rect(700, 560, 80, 30), "나가기"))
		{
			m_state = ChatState.LEAVE;
		}

		if (m_transport.IsConnected)
		{
			// 콩장수의(클라이언트 측) 메시지 표시. 
			DispBalloon(ref m_message, new Vector2(600.0f, 200.0f), new Vector2(340.0f, 360.0f), Color.green, false);
			GUI.DrawTexture(new Rect(600.0f, 370.0f, 145.0f, 200.0f), this.texture_daizu);
		}
	}

	void ErrorGUI()
	{
		float sx = 800.0f;
		float sy = 600.0f;
		float px = sx * 0.5f - 150.0f;
		float py = sy * 0.5f;

		if (GUI.Button(new Rect(px, py, 300, 80), "접속에 실패했습니다.\n\n버튼을 누르세요."))
		{
			m_state = ChatState.HOST_TYPE_SELECT;
		}
	}

	public void PostSendPacket(PACKET_ID packetID, byte[] bodyData)
	{
		if (m_transport.IsConnected == false)
		{
			return;
		}

		Int16 bodyDataSize = 0;
		if (bodyData != null)
		{
			bodyDataSize = (Int16)bodyData.Length;
		}
		var packetSize = bodyDataSize + NetLib.PacketDef.PACKET_HEADER_SIZE;

		List<byte> dataSource = new List<byte>();
		dataSource.AddRange(BitConverter.GetBytes((UInt16)packetSize));
		dataSource.AddRange(BitConverter.GetBytes((UInt16)packetID));
		dataSource.AddRange(new byte[] { (byte)0 });

		if (bodyData != null)
		{
			dataSource.AddRange(bodyData);
		}

		m_transport.Send(dataSource.ToArray());
	}

	void AddMessage(ref List<string> messages, string str)
	{
		while (messages.Count >= MESSAGE_LINE)
		{
			messages.RemoveAt(0);
		}

		messages.Add(str);
	}

	void DispBalloon(ref List<string> messages, Vector2 position, Vector2 size, Color color, bool left)
	{
		// 말풍선 테두리를 그립니다.
		DrawBaloonFrame(position, size, color, left);

		// 채팅 문장을 표시합니다. 	
		foreach (string s in messages)
		{
			DrawText(s, position, size);
			position.y += FONG_HEIGHT;
		}
	}

	void DrawBaloonFrame(Vector2 position, Vector2 size, Color color, bool left)
	{
		GUI.color = color;

		float kado_size = KADO_SIZE;

		Vector2 p, s;

		s.x = size.x - kado_size * 2.0f;
		s.y = size.y;

		// 한 가운데.
		p = position - s / 2.0f;
		GUI.DrawTexture(new Rect(p.x, p.y, s.x, s.y), this.texture_main);

		// 좌.
		p.x = position.x - s.x / 2.0f - kado_size;
		p.y = position.y - s.y / 2.0f + kado_size;
		GUI.DrawTexture(new Rect(p.x, p.y, kado_size, size.y - kado_size * 2.0f), this.texture_main);

		// 우.
		p.x = position.x + s.x / 2.0f;
		p.y = position.y - s.y / 2.0f + kado_size;
		GUI.DrawTexture(new Rect(p.x, p.y, kado_size, size.y - kado_size * 2.0f), this.texture_main);

		// 좌상.
		p.x = position.x - s.x / 2.0f - kado_size;
		p.y = position.y - s.y / 2.0f;
		GUI.DrawTexture(new Rect(p.x, p.y, kado_size, kado_size), this.texture_kado_lu);

		// 우상.
		p.x = position.x + s.x / 2.0f;
		p.y = position.y - s.y / 2.0f;
		GUI.DrawTexture(new Rect(p.x, p.y, kado_size, kado_size), this.texture_kado_ru);

		// 좌하.
		p.x = position.x - s.x / 2.0f - kado_size;
		p.y = position.y + s.y / 2.0f - kado_size;
		GUI.DrawTexture(new Rect(p.x, p.y, kado_size, kado_size), this.texture_kado_ld);

		// 우하.
		p.x = position.x + s.x / 2.0f;
		p.y = position.y + s.y / 2.0f - kado_size;
		GUI.DrawTexture(new Rect(p.x, p.y, kado_size, kado_size), this.texture_kado_rd);

		// 말풍선 기호.
		p.x = position.x - kado_size;
		p.y = position.y + s.y / 2.0f;
		GUI.DrawTexture(new Rect(p.x, p.y, kado_size, kado_size), this.texture_belo);

		GUI.color = Color.white;
	}

	void DrawText(string message, Vector2 position, Vector2 size)
	{
		if (message == "")
		{
			return;
		}

		GUIStyle style = new GUIStyle();
		style.fontSize = 16;
		style.normal.textColor = Color.white;

		Vector2 balloon_size, text_size;

		text_size.x = message.Length * FONT_SIZE;
		text_size.y = FONG_HEIGHT;

		balloon_size.x = text_size.x + KADO_SIZE * 2.0f;
		balloon_size.y = text_size.y + KADO_SIZE;

		Vector2 p;

		p.x = position.x - size.x / 2.0f + KADO_SIZE;
		p.y = position.y - size.y / 2.0f + KADO_SIZE;
		//p.x = position.x - text_size.x/2.0f;
		//p.y = position.y - text_size.y/2.0f;

		GUI.Label(new Rect(p.x, p.y, text_size.x, text_size.y), message, style);
	}

	void OnApplicationQuit()
	{
		if (m_transport != null)
		{
			m_transport.Disconnect();
		}
	}



	public enum PACKET_ID : ushort
	{
		PACKET_ID_ECHO = 101,
		PACKET_ID_SIMPLE_CHAT = 103,
	}
}
