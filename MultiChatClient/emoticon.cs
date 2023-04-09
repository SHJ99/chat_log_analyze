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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MultiChatClient
{
    public partial class emoticon : MetroForm
    {
        ChatForm ch;
        public emoticon()
        {
            InitializeComponent();
            comboBox_init();
        }
        public emoticon(ChatForm cht)
        {
            ch = cht;         
            InitializeComponent();
            comboBox_init();
        }
        
        public void comboBox_init()
        {
            comboBox1.Items.Add(Centvrio.Emoji.FacePositive.HeartEyes);
            comboBox1.Items.Add(Centvrio.Emoji.FaceNegative.ScreamingInFear);
            comboBox1.Items.Add(Centvrio.Emoji.FaceNegative.LoudlyCrying);
            comboBox1.Items.Add(Centvrio.Emoji.FaceNegative.Crying);
            comboBox1.Items.Add(Centvrio.Emoji.FaceNegative.Disappointed);
            comboBox1.Items.Add(Centvrio.Emoji.FaceNegative.Fearful);
            comboBox1.Items.Add(Centvrio.Emoji.FaceNegative.Crazy);
            comboBox1.Items.Add(Centvrio.Emoji.FaceNegative.Dizzy);
            comboBox1.Items.Add(Centvrio.Emoji.FaceNeutral.Thinking);
            comboBox1.Items.Add(Centvrio.Emoji.FaceNeutral.Tired);
            comboBox1.Items.Add(Centvrio.Emoji.FaceNeutral.Sleeping);
            comboBox1.Items.Add(Centvrio.Emoji.FaceNeutral.Expressionless);
            comboBox1.Items.Add(Centvrio.Emoji.FaceNegative.Angry);
            comboBox1.Items.Add(Centvrio.Emoji.FaceNeutral.Confused);
            comboBox1.Items.Add(Centvrio.Emoji.FacePositive.BigEyes);
            comboBox1.Items.Add(Centvrio.Emoji.FacePositive.Smiling);
            comboBox1.Items.Add(Centvrio.Emoji.FacePositive.Beaming);
            comboBox1.Items.Add(Centvrio.Emoji.FacePositive.FloorLaughing);
            comboBox1.Items.Add(Centvrio.Emoji.FacePositive.ThreeHearts);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ch.txtTTS.Text += comboBox1.Text;
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar== (char)Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }
    }
}
