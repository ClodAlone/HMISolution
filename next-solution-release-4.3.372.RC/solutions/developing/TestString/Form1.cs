using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestString
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                textBox2.Text = WPFUtilities.CryptString.CryptString.DecryptString(textBox1.Text);
            }
            catch(Exception ex) 
            {
                Console.WriteLine(string.Format("Error: {0}", ex.ToString()));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                textBox2.Text = WPFUtilities.CryptString.CryptString.EncryptString(textBox1.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error: {0}", ex.ToString()));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            const String pathData = "{0}@{1}?{2}";
            var result = String.Format(pathData, textBox3.Text, textBox4.Text, textBox5.Text);
            textBox1.Text = result;
        }
    }
}
