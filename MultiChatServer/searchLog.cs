using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MultiChatServer
{
    public partial class searchLog : MetroForm
    {
        static string _server = "155.230.235.248";
        static int _port = 54036;
        static string _database = "mydb";
        static string _id = "swUser01";
        static string _pw = "swdbUser01";
        static string _connectionAddress = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", _server, _port, _database, _id, _pw);
        MySqlConnection mysql = new MySqlConnection(_connectionAddress);
        int chatid = 0;

        public searchLog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string keyword = textBox1.Text;
            mysql.Open();
            //MySqlCommand cmd = new MySqlCommand("SELECT * FROM mydb.chat where chat like '%@log%'", mysql);
            string selectQuery = string.Format("SELECT * FROM mydb.chat where chat like '%{0}%'", textBox1.Text);
            MySqlCommand command = new MySqlCommand(selectQuery, mysql);
            MySqlDataReader table = command.ExecuteReader();

            listView1.Items.Clear();

            
            while (table.Read())
            {
                ListViewItem item = new ListViewItem();
                item.Text = table["Date"].ToString();
                item.SubItems.Add(table["User"].ToString());
                item.SubItems.Add(table["id"].ToString());
                item.SubItems.Add(table["Chat"].ToString());

                listView1.Items.Add(item);
            }
            table.Close();
        }
    }
}
