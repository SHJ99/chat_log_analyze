using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiChatClient
{
    public partial class indiv_info : Form
    {
        public indiv_info()
        {
            InitializeComponent();
            
        }
        public indiv_info(ChatForm cht)
        {

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            this.Close();
        }
    }
}
