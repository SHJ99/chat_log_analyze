using MetroFramework.Forms;
using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Web;
using MySql.Data.MySqlClient;

namespace MultiChatServer {
    public partial class ChatForm : MetroForm {
        static string _server = "155.230.235.248";
        static int _port = 54036;
        static string _database = "mydb";
        static string _id = "swUser01";
        static string _pw = "swdbUser01";
        static string _connectionAddress = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", _server, _port, _database, _id, _pw);
        MySqlConnection mysql = new MySqlConnection(_connectionAddress);
        int chatid = 0;

        delegate void AppendTextDelegate(Control ctrl, string s);
        AppendTextDelegate _textAppender;
        Socket mainSock;
        IPAddress thisAddress;

        public ChatForm() {

            statistics stc = new statistics(this);
            stc.Show();

            InitializeComponent();
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            _textAppender = new AppendTextDelegate(AppendText);
        }

        void AppendText(Control ctrl, string s) {
            if (ctrl.InvokeRequired) ctrl.Invoke(_textAppender, ctrl, s);
            else {
                string source = ctrl.Text;
                ctrl.Text = source + Environment.NewLine + s;
            }
        }

        void OnFormLoaded(object sender, EventArgs e) {
            IPHostEntry he = Dns.GetHostEntry(Dns.GetHostName());

            // 처음으로 발견되는 ipv4 주소를 사용한다.
            foreach (IPAddress addr in he.AddressList) {
                if (addr.AddressFamily == AddressFamily.InterNetwork) {
                    thisAddress = addr;
                    break;
                }    
            }

            // 주소가 없다면..
            if (thisAddress == null)
                // 로컬호스트 주소를 사용한다.
                thisAddress = IPAddress.Loopback;

            //txtAddress.Text = thisAddress.ToString();
            txtPort.Focus();
        }
        void BeginStartServer(object sender, EventArgs e) 
        {
            try
            {
                int port;
                if (!int.TryParse(txtPort.Text, out port))
                {
                    MsgBoxHelper.Error("포트 번호가 잘못 입력되었거나 입력되지 않았습니다.");
                    txtPort.Focus();
                    txtPort.SelectAll();
                    return;
                }

                //바인딩 되어있는지 확인
                if (mainSock.IsBound)
                {
                    MsgBoxHelper.Info("이미 서버에 연결되어 있습니다!");
                    return;
                }

                // 서버에서 클라이언트의 연결 요청을 대기하기 위해
                // 소켓을 열어둔다.

                IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, port);
                mainSock.Bind(serverEP);
                mainSock.Listen(10);

                // 비동기적으로 클라이언트의 연결 요청을 받는다.
                mainSock.BeginAccept(AcceptCallback, null);
                MsgBoxHelper.Info("서버가구동 되었습니다!");
            }
            catch
            {
                MsgBoxHelper.Error("서버 시작시 오류가 발생하였습니다.");
            }

        }

        List<Socket> connectedClients = new List<Socket>();
        void AcceptCallback(IAsyncResult ar) {

            try
            {
                // 클라이언트의 연결 요청을 수락한다.
                Socket client = mainSock.EndAccept(ar);

                // 또 다른 클라이언트의 연결을 대기한다.
                mainSock.BeginAccept(AcceptCallback, null);

                AsyncObject obj = new AsyncObject(4096);
                obj.WorkingSocket = client;

                // 연결된 클라이언트 리스트에 추가해준다.
                connectedClients.Add(client);

                // 텍스트박스에 클라이언트가 연결되었다고 써준다.
                AppendText(txtHistory, string.Format("클라이언트 (@ {0})가 연결되었습니다.", client.RemoteEndPoint));

                // 클라이언트의 데이터를 받는다.
                client.BeginReceive(obj.Buffer, 0, 4096, 0, DataReceived, obj);
            }
            catch 
            {
                //
                return;
            }
        }

        void DataReceived(IAsyncResult ar) 
        {
            try
            {
                // BeginReceive에서 추가적으로 넘어온 데이터를 AsyncObject 형식으로 변환한다.
                AsyncObject obj = (AsyncObject)ar.AsyncState;

                // 데이터 수신을 끝낸다.
                int received = obj.WorkingSocket.EndReceive(ar);

                // 받은 데이터가 없으면(연결끊어짐) 끝낸다.
                if (received <= 0)
                {
                    obj.WorkingSocket.Close();
                    return;
                }

                // 텍스트로 변환한다.
                string text = Encoding.UTF8.GetString(obj.Buffer);    


                // 0x01 기준으로 짜른다.
                // tokens[0] - 보낸 사람 IP
                // tokens[1] - 보낸 메세지
                string[] tokens = text.Split('\x01');
                string name = tokens[0];
                string id = tokens[1];
                string msg = tokens[2];


                // 텍스트박스에 추가해준다.
                // 비동기식으로 작업하기 때문에 폼의 UI 스레드에서 작업을 해줘야 한다.
                // 따라서 대리자를 통해 처리한다.
                AppendText(txtHistory, string.Format("[받음]{0}({1}) : {2}", name, id, msg));
                msg = msg.Replace("\0", string.Empty);
                Send_db(name, id, msg); //DB로 전송


                // #사용 웹검색기능
                if (msg[0] == '#')
                {
                    string search_text = msg;

                    search_text = search_text.Remove(0, 1);
                    for (int i = 0; i < search_text.Length; i++)
                    {
                        if (search_text[i] == '#')
                        {
                            search_text = search_text.Remove(i, search_text.Length - i);
                            break;
                        }
                    }
                    //MessageBox.Show(search_text);

                    string search_text2 = HttpUtility.HtmlEncode("https://www.google.com/search?q=") + HttpUtility.UrlEncodeUnicode(search_text);

                    search_text2 = search_text2.Replace("%00","");
                    


                    foreach (char c in search_text)
                    {
                        if (char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.OtherLetter)
                        {
                            //인코딩 방식을 지정
                            System.Text.Encoding utf8 = System.Text.Encoding.UTF8;

                            //변환하고자 하는 문자열을 UTF8 방식으로 변환하여 byte 배열로 반환
                            byte[] utf8Bytes = utf8.GetBytes(search_text);

                            //UTF-8을 string으로 변한
                            string utf8String = "";
                            //MessageBox.Show(" - Encode: ");
                            foreach (byte b in utf8Bytes)
                            {
                                utf8String += "%" + String.Format("{0:X}", b);
                            }
                            //MessageBox.Show(utf8String);

                            search_text2 = HttpUtility.HtmlEncode("https://www.google.com/search?q=") + HttpUtility.HtmlEncode(utf8String);
                            search_text2 = search_text2.Replace("%0", "");
                        }
                    }

                    //string s= HttpUtility.HtmlEncode(search_text2);


                    AppendText(txtHistory, search_text2);


                }



                // for을 통해 "역순"으로 클라이언트에게 데이터를 보낸다.
                for (int i = connectedClients.Count - 1; i >= 0; i--)
                {
                    Socket socket = connectedClients[i];
                    if (socket != obj.WorkingSocket)
                    {
                        try { socket.Send(obj.Buffer); }
                        catch
                        {
                            // 오류 발생하면 전송 취소하고 리스트에서 삭제한다.
                            try { socket.Dispose(); } catch { }
                            connectedClients.RemoveAt(i);
                        }
                    }
                }
                //msg = msg.Replace("\0", string.Empty);
                //Send_db(ip, msg); //DB로 전송

                // 데이터를 받은 후엔 다시 버퍼를 비워주고 같은 방법으로 수신을 대기한다.
                obj.ClearBuffer();

                // 수신 대기
                obj.WorkingSocket.BeginReceive(obj.Buffer, 0, 4096, 0, DataReceived, obj);
            }
            catch //(Exception ex)
            {
                //
                return;
            }
        }

        void OnSendData(object sender, EventArgs e) {
            // 서버가 대기중인지 확인한다.
            if (!mainSock.IsBound) {
                MsgBoxHelper.Warn("서버가 실행되고 있지 않습니다!");
                return;
            }
            
            // 보낼 텍스트
            string tts = txtTTS.Text.Trim();
            if (string.IsNullOrEmpty(tts)) {
                MsgBoxHelper.Warn("텍스트가 입력되지 않았습니다!");
                txtTTS.Focus();
                return;
            }


            // 문자열을 utf8 형식의 바이트로 변환한다.
            byte[] bDts = Encoding.UTF8.GetBytes(thisAddress.ToString() + '\x01' + tts);

            // 연결된 모든 클라이언트에게 전송한다.
            for (int i = connectedClients.Count - 1; i >= 0; i--) {
                Socket socket = connectedClients[i];
                try { socket.Send(bDts); } catch {
                    // 오류 발생하면 전송 취소하고 리스트에서 삭제한다.
                    try { socket.Dispose(); } catch { }
                    connectedClients.RemoveAt(i);
                }
            }

            // 전송 완료 후 텍스트박스에 추가하고, 원래의 내용은 지운다.
            AppendText(txtHistory, string.Format("[보냄]서버 : {0}",  tts));
            


            //웹검색 기능
            if (tts[0] == '#')
            {

                string search_text = tts;

                search_text=search_text.Remove(0,1);
                for (int i = 0; i < search_text.Length; i++)
                {
                    if (search_text[i] == '#')
                    {
                        search_text = search_text.Remove(i, search_text.Length - i);
                        break;
                    }
                }
                string search_text2 = HttpUtility.HtmlEncode("https://www.google.com/search?q=") + HttpUtility.HtmlEncode(HttpUtility.UrlEncodeUnicode(search_text));

                
                foreach(char c in search_text)
                {
                    if(char.GetUnicodeCategory(c)==System.Globalization.UnicodeCategory.OtherLetter)
                    {
                        //인코딩 방식을 지정
                        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;

                        //변환하고자 하는 문자열을 UTF8 방식으로 변환하여 byte 배열로 반환
                        byte[] utf8Bytes = utf8.GetBytes(search_text);

                        //UTF-8을 string으로 변한
                        string utf8String = "";
                        //MessageBox.Show(" - Encode: ");
                        foreach (byte b in utf8Bytes)
                        {
                            utf8String += "%" + String.Format("{0:X}", b);
                        }
                        //MessageBox.Show(utf8String);

                        search_text2 = HttpUtility.HtmlEncode("https://www.google.com/search?q=") + HttpUtility.HtmlEncode(utf8String);
                    }
                }

                //string s= HttpUtility.HtmlEncode(search_text2);


                AppendText(txtHistory, search_text2);

                /*if (MessageBox.Show(search_text +" 웹페이지에서 검색하시겠습니까?", "YesOrNo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("https://www.google.com/search?q=" + search_text);
                }*/
                //MessageBox.Show(search_text);
            }
            txtTTS.Clear();
        }

        private void ChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainSock.Close();
        }

        private void txtTTS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OnSendData(sender, e);
            }
        }







        //텍스트박스의 링크클릭 처리이벤트함수
        public System.Diagnostics.Process p = new System.Diagnostics.Process();
        
        private void txtHistory_LinkClicked(object sender, LinkClickedEventArgs e) 
        {
            p = System.Diagnostics.Process.Start("chrome.exe", e.LinkText);
            //StopWebProcess();
        }

        public void StopWebProcess()
        {
            p.Kill();
        }
        //서버에서 전송받은 채팅을 데이터베이스로 저장시키는 함수
        public void Send_db(string name, string id, string str)
        {
            //AppendText(txtHistory, "call data ");
            Random rand = new Random();
            chatid = rand.Next();
            mysql.Open(); //데이터베이스 오픈
            //AppendText(txtHistory, "연결성공");

            MySqlCommand cmd = new MySqlCommand("INSERT INTO  mydb.chat(Date, User, Chat, id) VALUES (NOW(), @user, @chat, @chatid)", mysql);
            //AppendText(txtHistory, cmd.ToString());

            cmd.Parameters.AddWithValue("@user", name);
            cmd.Parameters.AddWithValue("@chat", str);
            cmd.Parameters.AddWithValue("@chatid", id);
            //AppendText(txtHistory, cmd.ToString());
            cmd.Prepare();
            if (cmd.ExecuteNonQuery() != 1)
                AppendText(txtHistory, string.Format("Failed to insert data."));

            //AppendText(txtHistory, cmd.ToString());

            /*
            if (cmd.ExecuteNonQuery() != 1)
                AppendText(txtHistory, string.Format("Failed to insert data."));
            else
                AppendText(txtHistory, string.Format("insert data."));
            */
            mysql.Close();

        }
    }
}