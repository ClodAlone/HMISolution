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
    public partial class TestWebsite : Form
    {
        private IISWebsite _website = null;

        public TestWebsite(IISWebsite Website)
        {
            InitializeComponent();

            this._website = Website;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            
        }

        private void TestWebsite_Load(object sender, EventArgs e)
        {
            this.tbName.Text = this._website.Name;
            this.tbPort.Text = this._website.Port.ToString();
            this.tbPath.Text = this._website.Root.Path;

            // display web dirs
         
            this.WebDirs.Nodes.Add("root", this._website.Root.Path);

            SearchSubDirs(this._website.Root, this.WebDirs.Nodes["root"]);
        }

        private void SearchSubDirs(IISWebVirturalDir parent, TreeNode node)
        {
            foreach (string dir in parent.EnumSubVirtualDirs())
            {
                node.Nodes.Add(dir, dir);

                IISWebVirturalDir vd = parent.OpenSubVirtualDir(dir);
                SearchSubDirs(vd, node.Nodes[dir]);
            }
        }

        private void BtnChgName_Click(object sender, EventArgs e)
        {
            this._website.Name = this.tbName.Text.Trim();

            MessageBox.Show("done");
        }

        private void btnChgPort_Click(object sender, EventArgs e)
        {
            this._website.Port = int.Parse(this.tbPort.Text.Trim());
    
            MessageBox.Show("done");
        }

        private void btnChgPath_Click(object sender, EventArgs e)
        {
            this._website.Root.Path = this.tbPath.Text.Trim();
  
            MessageBox.Show("done");
        }
    }
}