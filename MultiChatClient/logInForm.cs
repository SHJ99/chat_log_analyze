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

namespace MultiChatClient
{
    public partial class logInForm : MetroForm
    {
        ChatForm cf;
        public logInForm()
        {
            InitializeComponent();
        }

        public logInForm(ChatForm cf)
        {
            InitializeComponent();
            this.cf = cf;

  
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
           
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            SignUp f = new SignUp(cf);
            f.Show();
        }

        private void metroTile1_Click(object sender, EventArgs e)
        {
            if (!cf.LoginCheck(metroTextBox1.Text, metroTextBox2.Text))
            {
                MetroFramework.MetroMessageBox.Show(this, "아이디 또는 비밀번호가 잘못되었습니다", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error, 130);
                return;
            }
            MetroFramework.MetroMessageBox.Show(this, "환영합니다.", "로그인 성공", MessageBoxButtons.OK, MessageBoxIcon.Information, 130);

            cf.Visible = true;
            cf.ShowInTaskbar = true;
            cf.Opacity = 1;
            //cf.WindowState = FormWindowState.Normal;
          
            cf.ConnectServer();
            this.Close();
        }

        private void metroCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(metroCheckBox1.Checked==true)
                metroTextBox2.PasswordChar = default(char);
            else
                metroTextBox2.PasswordChar = '*';
        }
    }
}
