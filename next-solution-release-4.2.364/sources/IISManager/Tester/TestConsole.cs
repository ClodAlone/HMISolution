using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using IISManager;

namespace Tester
{
    public partial class TestConsole : Form
    {
        public TestConsole()
        {
            InitializeComponent();
        }

        private void BindCombox()
        {
            string[] websites = IISWebsite.ExistedWebsites;

            cbWebsiteList.DataSource = websites;
        }

        private void TestConsole_Load(object sender, EventArgs e)
        {
            BindCombox();
        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            string name = cbWebsiteList.SelectedValue as string;
            IISWebsite website = IISWebsite.OpenWebsite(name);

            TestWebsite tester = new TestWebsite(website);
            
            this.Hide();
            tester.ShowDialog();
            this.Show();
        }

        private void BtnNewSite_Click(object sender, EventArgs e)
        {
            string name = this.tbNewSite.Text.Trim();
            int port = int.Parse(this.tbPort.Text.Trim());
            string path = this.tbRootPath.Text.Trim();
            IISWebsite.CreateWebsite(name, port, path);
            BindCombox();

            MessageBox.Show("done");
         
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            IISWebsite.DeleteWebsite(this.cbWebsiteList.SelectedValue.ToString());
        }
    }
}