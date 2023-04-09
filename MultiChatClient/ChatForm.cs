using MetroFramework.Forms;
using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Web;
using MySql.Data.MySqlClient;

namespace MultiChatClient {
    public partial class ChatForm : MetroForm {
        delegate void AppendTextDelegate(Control ctrl, string s);
        AppendTextDelegate _textAppender;
        Socket mainSock;

        public bool isLogin = false;

        string _server = "155.230.235.248";
        int _port = 54036;
        string _database = "mydb";
        string _id = "swUser01";
        string _pw = "swdbUser01";
        string _connectionAddress = "";

        String myId = "";
        String myName = "";

        public ChatForm() {



            // 회원정보 창 삭제하셔도 됩니다
            //indiv_info idv= new indiv_info(this);
            //idv.ShowDialog();



            //emoticon em = new emoticon(this);
            //em.Show();



            InitializeComponent();
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            _textAppender = new AppendTextDelegate(AppendText);

            _connectionAddress = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", _server, _port, _database, _id, _pw);


            this.Opacity = 0;
            this.Visible = false;
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;

            comboBox_init();

            logInForm logInForm = new logInForm(this);
            logInForm.Show();
        }

        public void comboBox_init()
        {
            metroComboBox1.Items.Add(Centvrio.Emoji.FacePositive.HeartEyes);
            metroComboBox1.Items.Add(Centvrio.Emoji.FaceNegative.ScreamingInFear);
            metroComboBox1.Items.Add(Centvrio.Emoji.FaceNegative.LoudlyCrying);
            metroComboBox1.Items.Add(Centvrio.Emoji.FaceNegative.Crying);
            metroComboBox1.Items.Add(Centvrio.Emoji.FaceNegative.Disappointed);
            metroComboBox1.Items.Add(Centvrio.Emoji.FaceNegative.Fearful);
            metroComboBox1.Items.Add(Centvrio.Emoji.FaceNegative.Crazy);
            metroComboBox1.Items.Add(Centvrio.Emoji.FaceNegative.Dizzy);
            metroComboBox1.Items.Add(Centvrio.Emoji.FaceNeutral.Thinking);
            metroComboBox1.Items.Add(Centvrio.Emoji.FaceNeutral.Tired);
            metroComboBox1.Items.Add(Centvrio.Emoji.FaceNeutral.Sleeping);
            metroComboBox1.Items.Add(Centvrio.Emoji.FaceNeutral.Expressionless);
            metroComboBox1.Items.Add(Centvrio.Emoji.FaceNegative.Angry);
            metroComboBox1.Items.Add(Centvrio.Emoji.FaceNeutral.Confused);
            metroComboBox1.Items.Add(Centvrio.Emoji.FacePositive.BigEyes);
            metroComboBox1.Items.Add(Centvrio.Emoji.FacePositive.Smiling);
            metroComboBox1.Items.Add(Centvrio.Emoji.FacePositive.Beaming);
            metroComboBox1.Items.Add(Centvrio.Emoji.FacePositive.FloorLaughing);
            metroComboBox1.Items.Add(Centvrio.Emoji.FacePositive.ThreeHearts);
        }
        void AppendText(Control ctrl, string s) {
            if (ctrl.InvokeRequired) ctrl.Invoke(_textAppender, ctrl, s);
            else {
                string source = ctrl.Text;
                ctrl.Text = source + Environment.NewLine + s;
               
            }


        }

        public bool LoginCheck(String id, String pw)
        {
            try
            {
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();


                    string Query = string.Format("SELECT * FROM userinfo WHERE ID=\"{0}\" AND PW=\"{1}\";", id, pw);

                    MySqlCommand command = new MySqlCommand(Query, mysql);

                    object v = command.ExecuteScalar();

                    if (v == null)
                    {
                        

                        
                        return false;
                    }

                    MySqlDataReader table = command.ExecuteReader();
                    table.Read();
                    myId = table["ID"].ToString();
                    myName = table["Name"].ToString();
                    table.Close();
                   

                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);

            }
            return true;
        }

        public void acceptSignUpData(String id, String pw, String name, String birth, int sex) // 12/16 추가
        {
            try
            {
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();
                    int age = Int32.Parse(birth);
                    string insertQuery = string.Format("INSERT INTO userinfo (ID, PW, Name, Age, sex) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');",
                                                                                        id, pw, name, age, sex);
                    MySqlCommand command = new MySqlCommand(insertQuery, mysql);
                    if (command.ExecuteNonQuery() != 1)
                    {
                        MessageBox.Show("Failed to insert data.");
                        return;
                    }

                   

                }

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }


        }

        public bool checkDuplication(String id)
        {

            using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
            {
                mysql.Open();


                string Query = string.Format("SELECT * FROM userinfo WHERE ID=\"{0}\";", id);

                MySqlCommand command = new MySqlCommand(Query, mysql);

                object v = command.ExecuteScalar();

                if (v == null)
                {
                   
                    return true;
                }

               
                return false;

            }


        }
        void OnFormLoaded(object sender, EventArgs e) {
            IPHostEntry he = Dns.GetHostEntry(Dns.GetHostName());

            // 처음으로 발견되는 ipv4 주소를 사용한다.
            IPAddress defaultHostAddress = null;
            foreach (IPAddress addr in he.AddressList) {
                if (addr.AddressFamily == AddressFamily.InterNetwork) {
                    defaultHostAddress = addr;
                    break;
                }
            }

            // 주소가 없다면..
            if (defaultHostAddress == null)
                // 로컬호스트 주소를 사용한다.
                defaultHostAddress = IPAddress.Loopback;

            //txtAddress.Text = defaultHostAddress.ToString();
        }

        void OnConnectToServer(object sender, EventArgs e) {

        } // 사용 안함

        public void ConnectServer() 
        {

            String serverIp = "127.0.0.1"; // 완셩하면 ip주소 수정
            int port = 9000;

            try { mainSock.Connect(serverIp, port); }
            catch (Exception ex)
            {
                MsgBoxHelper.Error("연결에 실패했습니다!\n오류 내용: {0}", MessageBoxButtons.OK, ex.Message);
                return;
            }

            AppendText(txtHistory, "서버와 연결되었습니다.");

            AsyncObject obj = new AsyncObject(4096);
            obj.WorkingSocket = mainSock;
            mainSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);

            string t = string.Format("ID : {0}", myId);
            metroLabel1.Text = t;
            t = string.Format("{0}님 환영합니다", myName);
            metroLabel2.Text = t;


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




                //웹검색기능
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
                    string search_text2 = HttpUtility.HtmlEncode("https://www.google.com/search?q=") + HttpUtility.HtmlEncode(HttpUtility.UrlEncodeUnicode(search_text));

                    search_text2 = search_text2.Replace("%00", "");
                    

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




                // 클라이언트에선 데이터를 전달해줄 필요가 없으므로 바로 수신 대기한다.
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

            // 서버 ip 주소와 메세지를 담도록 만든다.
            IPEndPoint ip = (IPEndPoint) mainSock.LocalEndPoint;
            string addr = ip.Address.ToString();

            // 문자열을 utf8 형식의 바이트로 변환한다.
            byte[] bDts = Encoding.UTF8.GetBytes( myName + '\x01' + myId + '\x01' + tts);

            // 서버에 전송한다.
            mainSock.Send(bDts);

            // 전송 완료 후 텍스트박스에 추가하고, 원래의 내용은 지운다.
            AppendText(txtHistory, string.Format("[보냄]{0}({1}) : {2}", myName,myId, tts));


            //웹검색기능
            if (tts[0] == '#')
            {

                string search_text = tts;

                search_text = search_text.Remove(0, 1);
                for (int i = 0; i < search_text.Length; i++)
                {
                    if (search_text[i] == '#')
                    {
                        search_text = search_text.Remove(i, search_text.Length - i);
                        break;
                    }
                }
                string search_text2 = HttpUtility.HtmlEncode("https://www.google.com/search?q=") + HttpUtility.HtmlEncode(HttpUtility.UrlEncodeUnicode(search_text));


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

        // ip 유효성 검사
        public bool IsAvailableIP(string ip)
        {
            // ip 값이 null이면 실패 반환
            if (ip == null)
                return false;

            // ip 길이가 15자 넘거나 7보다 작으면 실패를 반환    
            if (ip.Length > 15 || ip.Length < 7)
                return false;

            // 숫자 갯수
            int nNumCount = 0;

            // '.' 갯수 
            int nDotCount = 0;

            for (int i = 0; i < ip.Length; i++)
            {
                if (ip[i] < '0' || ip[i] > '9')
                {
                    if ('.' == ip[i])
                    {
                        ++nDotCount;
                        nNumCount = 0;
                    }
                    else
                        return false;
                }
                else
                {
                    //'.'이 4개 이상이면 실패 반환
                    if (++nNumCount > 3)
                        return false;
                }
            }
            // '.' 3개 아니여도 실패 반환
            if (nDotCount != 3)
                return false;

            return true;
        }

        private void ChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainSock.Close();
        }

        private void txtTTS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OnSendData(sender,  e);
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

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtTTS.Text += metroComboBox1.Text;
        }
    }
}