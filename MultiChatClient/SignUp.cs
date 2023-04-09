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

    public partial class SignUp : MetroForm
    {
        ChatForm f;
        bool idcheck = false;
        public SignUp()
        {
            InitializeComponent();
        }
        public SignUp(ChatForm f)
        {
            InitializeComponent();
            metroRadioButton1.Checked = true;
            this.f = f;
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            if (!idcheck)
            {
                MetroFramework.MetroMessageBox.Show(this,"아이디 중복 체크 안됨");
                return;
            }
            int s;

            if (metroRadioButton1.Checked) s = 0;
            else s = 1;

            f.acceptSignUpData(metroTextBox1.Text, metroTextBox2.Text, metroTextBox3.Text, metroTextBox4.Text, s);
            MetroFramework.MetroMessageBox.Show(this, "회원가입 완료", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information, 130);
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            if (f.checkDuplication(metroTextBox1.Text))
            {
                MetroFramework.MetroMessageBox.Show(this, "유효한 아이디입니다");
                idcheck = true;
            }
            else
                MetroFramework.MetroMessageBox.Show(this, "중복된 아이디입니다");
        }
    }
}
