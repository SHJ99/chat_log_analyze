using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Numerics;
using System.Data.SqlClient;
using System.Windows.Forms.DataVisualization.Charting;
using System.Text.RegularExpressions;
using Moda.Korean.TwitterKoreanProcessorCS;
using java.sql;
using scala.collection;

namespace MultiChatServer
{
    public partial class statistics : MetroForm
    {

        static string _server = "155.230.235.248";
        static int _port = 54036;
        static string _database = "mydb";
        static string _id = "swUser01";
        static string _pw = "swdbUser01";
        //string _connectionAddress = "";
        static string _connectionAddress = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", _server, _port, _database, _id, _pw);
        //SqlConnection connection = new SqlConnection(_connectionAddress);
        MySqlConnection mysql = new MySqlConnection(_connectionAddress);


        //MySqlConnection connection = new MySqlConnection("Server=155.230.235.248; port=54036; Database=mydb; Uid=swUser01;Pwd=swdbUser01;");
        ChatForm chatForm;

        string age="";
        string sex="";
        string name="";
        string time="";
        string wholedate = "";
        string startdate = "";
        string enddate = "";

        public statistics()
        {
            InitializeComponent();
        }
        public statistics(ChatForm chatForm)
        {
            this.chatForm = chatForm;
            InitializeComponent();
        }

        private void call_Click(object sender, EventArgs e)
        {
            call_Db();
        }

        /// <summary>
        /// 단어 검색기능
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            chatForm.txtHistory.Select(0, chatForm.txtHistory.TextLength);
            chatForm.txtHistory.SelectionColor = Color.Black;        

            Queue<int> iFindStartIndex=new Queue<int>();
            int iFindLength = textBox4.Text.Length;
            //MessageBox.Show(chatForm.txtHistory.Text.Length.ToString());


            iFindStartIndex = FindMyText(textBox4.Text, textBox4.Text.Length, chatForm.txtHistory.Text.Length);

            if (iFindStartIndex.Count==0)
            {
                return;
            }

            while (iFindStartIndex.Count!=0)
            {
                
                int i = iFindStartIndex.Dequeue();
                //MessageBox.Show("i : "+i.ToString());
                chatForm.txtHistory.Select(i, iFindLength);
                chatForm.txtHistory.SelectionColor = Color.Red;
            }

        }


        
        private void statistics_Load(object sender, EventArgs e)
        {
            //monthCalendar1.SelectionStart = DateTime.Now;
            //monthCalendar1.SelectionEnd = DateTime.Now.AddDays(3);
            //textBox4.Hide();
            //button1.Hide();
        }

        private Queue<int> FindMyText(string searchText, int searchStart, int searchEnd)
        {
            Queue<int> q = new Queue<int>();
            //int returnValue = -1;
            if (searchText.Length > 0 && searchStart >= 0)
            {
                while ((searchEnd >= searchStart || searchEnd == -1)&&searchStart <searchEnd)
                {
                    int indexToText = chatForm.txtHistory.Find(searchText, searchStart, searchEnd, RichTextBoxFinds.MatchCase);
                    if (indexToText >= 0)
                    {
                        //MessageBox.Show(indexToText.ToString() + " " + searchEnd.ToString());
                        q.Enqueue(indexToText);
                        searchStart = indexToText + 1;
                        //MessageBox.Show("searchStart : " + searchStart.ToString());
                    }
                    else
                    {
                        return q;
                    }
                }
            }
            return q;
        }

        
        void call_Db()
        {


            //var results = TwitterKoreanProcessorCS.Tokenize("형태소를 분석합니다");
            
           

            bool ismatch=true;
            //sql 변수 초기화
            {

            //이름
                name = textBox3.Text;
                
                //무결성 검사
                Regex regex = new Regex(@"[a-횧]");
                ismatch = regex.IsMatch(name);

                if(!ismatch && name != "")
                {
                    MessageBox.Show("이름 형식이 올바르지 않습니다");
                    return;
                }

                //초기화
                if (name == "")
                {
                    name = "name regexp '[a-힇]'";
                }
                else if (textBox3.Text.Length >= 1)
                {
                    name = "name regexp '[" + name + "]+'";
                }

                //MessageBox.Show(name);



            //나이
                //무결성 검사
                string a, b;
                a = textBox1.Text;
                b = textBox2.Text;

                Regex regex1 = new Regex(@"[0-9]");
                bool match1 = regex1.IsMatch(a);
                bool match2 = regex1.IsMatch(b);

                if ( !match1 && textBox1.Text != "" )
                {
                    MessageBox.Show("나이 형식이 올바르지 않습니다");
                    textBox1.Clear();
                    textBox1.Focus();
                    return;
                }

                if ( !match2 && textBox2.Text != "" )
                {          
                    MessageBox.Show("나이 형식이 올바르지 않습니다");
                    textBox2.Clear();
                    textBox2.Focus();
                    return;
                }

                if (textBox1.Text != "" && textBox2.Text != "")
                {
                    if (int.Parse(textBox1.Text) > int.Parse(textBox2.Text))
                    {
                        MessageBox.Show("나이 형식이 올바르지 않습니다");
                        textBox1.Clear();
                        textBox2.Clear();
                        textBox1.Focus();
                        return;
                    }
                }
                
                //초기화
                if (textBox2.Text == "")
                {
                    age = " and age >=" + textBox1.Text;
                }

                age = " and age >= "+ textBox1.Text +" and age<=" + textBox2.Text;

                if (textBox1.Text=="" && textBox2.Text == "")
                {
                    age = " and age regexp '[:digit:]'";
                }

                //MessageBox.Show(age);


                
            
            //성별
                //초기화
                if (radioButton1.Checked == true)
                {
                    sex = " and sex =1";
                }
                else if (radioButton3.Checked == true)
                {
                    sex = " and sex = 1 or sex = 2";
                }
                else
                {
                    sex = " and sex = 2";
                }

                //MessageBox.Show(sex);

            //날짜
                //초기화
                if (startdate == "" && enddate == "")
                {
                    wholedate = "";
                    textBox5.Text = "모든날짜에서 검색합니다";
                }
                else
                {
                    wholedate = " BETWEEN '"+ startdate + "' AND '" + enddate+"'";
                    //MessageBox.Show(wholedate);
                }

            }


            string str = "";
            var chart_elmt = new Dictionary<string, int>();
            //mysql 커맨드 초기화
            {
                mysql.Open();


                string command = "select chat from chat inner join userinfo using (id) where " + name + age + sex + wholedate+";";
                
                //MessageBox.Show(command);

                MySqlCommand cmd = new MySqlCommand(command, mysql);



                
                cmd.Prepare();

                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    str = rdr["Chat"].ToString();
                    //MessageBox.Show(str);
                    var tokens = TwitterKoreanProcessorCS.Tokenize(str);
                    var tokens2 = TwitterKoreanProcessorCS.Stem(tokens);

                    //StringBuilder result = new StringBuilder();

                    foreach (var token in tokens2)
                    {
                        /*result.AppendFormat(format: "{0}({1}) [{2},{3}] / ",
                           args: new object[] { token.Text, token.Pos.ToString(), token.Offset, token.Length });*/
                        //MessageBox.Show(result.ToString());

                        //해당 단어가 한국어 명사이거나 영어인 경우 저장
                        if (token.Pos.ToString() == "Noun" || token.Pos.ToString() == "ProperNoun" | token.Pos.ToString() == "Alpha")
                        {
                            //디버깅용 코드
                            //MessageBox.Show(token.Text);

                            if (!chart_elmt.ContainsKey(token.Text))
                            {
                                chart_elmt.Add(token.Text, 1);
                            }
                            else
                            {
                                chart_elmt[token.Text]++;
                            }
                        }
                        //해쉬태그인경우 해쉬태그 제외 후 저장
                        if (token.Pos.ToString() == "Hashtag")
                        {
                            string tok = token.Text;
                            tok = tok.Substring(1);
                            //MessageBox.Show(tok);
                            if (!chart_elmt.ContainsKey(tok))
                            {
                                chart_elmt.Add(tok, 1);
                            }
                            else
                            {
                                chart_elmt[tok]++;
                            }
                        }

                    }
                }
                mysql.Close();
            }

            //차트표시
            {
                chart1.Series.Clear();
                chart1.Series.Add("빈도수가 높은 상위 5개 단어");

                var quertAsc = chart_elmt.OrderByDescending(x => x.Value);


                foreach (var dic in quertAsc)
                {
                    string k = dic.Key;
                    string v = dic.Value.ToString();
                    //MessageBox.Show(k + "  " + v);
                    chart1.Series[0].Points.AddXY(k, v);

                    //MessageBox.Show(chart1.Series[0].Points.Count.ToString());

                    if (chart1.Series[0].Points.Count > 5)
                        break;
                }

            }




        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            startdate = monthCalendar1.SelectionStart.ToShortDateString();
            enddate = monthCalendar1.SelectionEnd.ToShortDateString();
            textBox5.Text = startdate+"~"+enddate;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            wholedate = "";
            startdate = "";
            enddate = "";
            textBox5.Text = "모든날짜에서 검색합니다";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            searchLog froms = new searchLog();
            froms.Show();
        }
    }

}
