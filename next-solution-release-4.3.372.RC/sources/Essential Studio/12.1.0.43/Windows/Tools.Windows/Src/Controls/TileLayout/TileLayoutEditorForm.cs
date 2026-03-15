#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    public partial class TileLayoutEditorForm : Form
    {

        TileLayout tileLayout;
        public TileLayoutEditorForm(TileLayout tile)
        {
            InitializeComponent();
            tileLayout = tile;
            foreach (Control group in tileLayout.Controls)
            {
              
                    this.listBox1.Items.Add (group.Name);
 
            }
            groupCount = this.listBox1.Items.Count;
        }
        int groupCount = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            LayoutGroup group = new LayoutGroup();
            group.Name = "Group" +Convert.ToString (groupCount );
            tileLayout.Groups.Add(group);
            group.Height = tileLayout.Height - 40;
            group.Width = 200;
            this.listBox1.Items.Add(group.Name);
            groupCount++;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (Control group in tileLayout.Controls)
            {
                if (group.Name == this.listBox1.SelectedItem.ToString())
                {
                    tileLayout.Controls.Remove(group);
                    this.listBox1.Items.Remove(this.listBox1.SelectedItem );
                    return ;
                }
            }
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
               foreach (Control group in tileLayout.Controls)
            {
                if (group.Name == this.listBox1.SelectedItem.ToString())
                {
                    this.propertyGrid1.SelectedObject = group;
                    return ;
                }
            }
           
        }
    }
}
