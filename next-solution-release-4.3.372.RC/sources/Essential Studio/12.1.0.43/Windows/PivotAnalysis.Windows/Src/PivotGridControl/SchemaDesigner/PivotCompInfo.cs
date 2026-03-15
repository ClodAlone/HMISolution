#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Windows.Forms.Grid;
using System.Windows.Forms;
using System.Drawing;
using Syncfusion.PivotAnalysis.Base;
using System.Collections;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.PivotAnalysis
{

    public class PivotCompInfo : MetroForm
    {
        private Label fieldNamelbl;
        private Label fieldHeaderlbl;
        private Label descriptionlbl;
        private Label formatlbl;
        private Label summarylbl;
        private Label showValuelbl;
        private TextBox fieldheadertxt;
        private TextBox descriptiontxt;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private Label fieldvallbl;
        private Button okbtn;
        private Button cancelbtn;
        private Panel panel1;
        private TextBox formattxt;
        private bool shouldUpdate = false;
        
        public PivotCompInfo()
            : base()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            InitializeComponent();
            BindComboItems();
        }

        private void InitializeComponent()
        {
            this.fieldNamelbl = new System.Windows.Forms.Label();
            this.fieldHeaderlbl = new System.Windows.Forms.Label();
            this.descriptionlbl = new System.Windows.Forms.Label();
            this.formatlbl = new System.Windows.Forms.Label();
            this.summarylbl = new System.Windows.Forms.Label();
            this.showValuelbl = new System.Windows.Forms.Label();
            this.fieldheadertxt = new System.Windows.Forms.TextBox();
            this.descriptiontxt = new System.Windows.Forms.TextBox();
            this.formattxt = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.fieldvallbl = new System.Windows.Forms.Label();
            this.okbtn = new System.Windows.Forms.Button();
            this.cancelbtn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // fieldNamelbl
            // 
            this.fieldNamelbl.AutoSize = true;
            this.fieldNamelbl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fieldNamelbl.Location = new System.Drawing.Point(21, 29);
            this.fieldNamelbl.Name = "fieldNamelbl";
            this.fieldNamelbl.Size = new System.Drawing.Size(64, 13);
            this.fieldNamelbl.TabIndex = 0;
            this.fieldNamelbl.Text = "Field Name";
            // 
            // fieldHeaderlbl
            // 
            this.fieldHeaderlbl.AutoSize = true;
            this.fieldHeaderlbl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fieldHeaderlbl.Location = new System.Drawing.Point(21, 63);
            this.fieldHeaderlbl.Name = "fieldHeaderlbl";
            this.fieldHeaderlbl.Size = new System.Drawing.Size(72, 13);
            this.fieldHeaderlbl.TabIndex = 1;
            this.fieldHeaderlbl.Text = "Field Header";
            // 
            // descriptionlbl
            // 
            this.descriptionlbl.AutoSize = true;
            this.descriptionlbl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.descriptionlbl.Location = new System.Drawing.Point(21, 94);
            this.descriptionlbl.Name = "descriptionlbl";
            this.descriptionlbl.Size = new System.Drawing.Size(66, 13);
            this.descriptionlbl.TabIndex = 2;
            this.descriptionlbl.Text = "Description";
            // 
            // formatlbl
            // 
            this.formatlbl.AutoSize = true;
            this.formatlbl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.formatlbl.Location = new System.Drawing.Point(21, 127);
            this.formatlbl.Name = "formatlbl";
            this.formatlbl.Size = new System.Drawing.Size(43, 13);
            this.formatlbl.TabIndex = 3;
            this.formatlbl.Text = "Format";
            // 
            // summarylbl
            // 
            this.summarylbl.AutoSize = true;
            this.summarylbl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.summarylbl.Location = new System.Drawing.Point(21, 163);
            this.summarylbl.Name = "summarylbl";
            this.summarylbl.Size = new System.Drawing.Size(109, 13);
            this.summarylbl.TabIndex = 4;
            this.summarylbl.Text = "Summarize Value By";
            // 
            // showValuelbl
            // 
            this.showValuelbl.AutoSize = true;
            this.showValuelbl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showValuelbl.Location = new System.Drawing.Point(21, 197);
            this.showValuelbl.Name = "showValuelbl";
            this.showValuelbl.Size = new System.Drawing.Size(83, 13);
            this.showValuelbl.TabIndex = 5;
            this.showValuelbl.Text = "Show Value As";
            // 
            // fieldheadertxt
            // 
            this.fieldheadertxt.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fieldheadertxt.Location = new System.Drawing.Point(135, 60);
            this.fieldheadertxt.Name = "fieldheadertxt";
            this.fieldheadertxt.Size = new System.Drawing.Size(145, 22);
            this.fieldheadertxt.TabIndex = 7;
            this.fieldheadertxt.TextChanged += new System.EventHandler(this.text_TextChanged);
            // 
            // descriptiontxt
            // 
            this.descriptiontxt.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.descriptiontxt.Location = new System.Drawing.Point(135, 91);
            this.descriptiontxt.Name = "descriptiontxt";
            this.descriptiontxt.Size = new System.Drawing.Size(145, 22);
            this.descriptiontxt.TabIndex = 8;
            this.descriptiontxt.TextChanged += new System.EventHandler(this.text_TextChanged);
            // 
            // formattxt
            // 
            this.formattxt.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.formattxt.Location = new System.Drawing.Point(135, 124);
            this.formattxt.Name = "formattxt";
            this.formattxt.Size = new System.Drawing.Size(145, 22);
            this.formattxt.TabIndex = 9;
            this.formattxt.TextChanged += new System.EventHandler(this.text_TextChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(135, 163);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(145, 21);
            this.comboBox1.TabIndex = 10;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox_SelectedIndexChanged);
            // 
            // comboBox2
            // 
            this.comboBox2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(135, 197);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(145, 21);
            this.comboBox2.TabIndex = 11;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox_SelectedIndexChanged);
            // 
            // fieldvallbl
            // 
            this.fieldvallbl.AutoSize = true;
            this.fieldvallbl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fieldvallbl.Location = new System.Drawing.Point(132, 29);
            this.fieldvallbl.Name = "fieldvallbl";
            this.fieldvallbl.Size = new System.Drawing.Size(61, 13);
            this.fieldvallbl.TabIndex = 12;
            this.fieldvallbl.Text = "FieldName";
            // 
            // okbtn
            // 
            this.okbtn.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okbtn.Location = new System.Drawing.Point(133, 253);
            this.okbtn.Name = "okbtn";
            this.okbtn.Size = new System.Drawing.Size(76, 23);
            this.okbtn.TabIndex = 13;
            this.okbtn.Text = "OK";
            this.okbtn.UseVisualStyleBackColor = true;
            this.okbtn.Click += new System.EventHandler(this.okbtn_Click);
            // 
            // cancelbtn
            // 
            this.cancelbtn.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelbtn.Location = new System.Drawing.Point(230, 253);
            this.cancelbtn.Name = "cancelbtn";
            this.cancelbtn.Size = new System.Drawing.Size(69, 23);
            this.cancelbtn.TabIndex = 14;
            this.cancelbtn.Text = "Cancel";
            this.cancelbtn.UseVisualStyleBackColor = true;
            this.cancelbtn.Click += new System.EventHandler(this.cancelbtn_Click);
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(311, 290);
            this.panel1.TabIndex = 15;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // PivotCompInfo
            // 
            this.BorderThickness = 2;
            this.CaptionAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ClientSize = new System.Drawing.Size(311, 290);
            this.Controls.Add(this.cancelbtn);
            this.Controls.Add(this.okbtn);
            this.Controls.Add(this.fieldvallbl);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.formattxt);
            this.Controls.Add(this.descriptiontxt);
            this.Controls.Add(this.fieldheadertxt);
            this.Controls.Add(this.showValuelbl);
            this.Controls.Add(this.summarylbl);
            this.Controls.Add(this.formatlbl);
            this.Controls.Add(this.descriptionlbl);
            this.Controls.Add(this.fieldHeaderlbl);
            this.Controls.Add(this.fieldNamelbl);
            this.Controls.Add(this.panel1);
            this.DropShadow = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PivotCompInfo";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = " Pivot Computation Information";
            this.Load += new System.EventHandler(this.PivotCompInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #region Helper Methids
        PivotGridControl grid; PivotComputationInfo comp;
        /// <summary>
        /// Bind the items with the values
        /// </summary>
        internal void WireCompInfo(GridStyleInfo style, PivotGridControl grid)
        {
            this.grid = grid;
             comp = this.grid.PivotCalculations.Where(l => l.FieldName == style.CellValue.ToString()).AsQueryable().ToList().First();
             if (comp != null)
             {
                 this.fieldheadertxt.Text = comp.FieldName;
                 this.fieldvallbl.Text = comp.FieldName;
                 this.descriptiontxt.Text = comp.Description;
                 this.formattxt.Text = comp.Format;
                 this.comboBox1.Text = comp.SummaryType.ToString();
                 this.comboBox2.Text = comp.CalculationType.ToString();
             }
        }

        /// <summary>
        /// ind the combobox items
        /// </summary>
        private void BindComboItems()
        {
            Array list = (Array)Enum.GetValues(typeof(SummaryType));
            ArrayList ds = new ArrayList();
            foreach (var itm in list)
            {
                ds.Add(itm);
            }
            this.comboBox1.DataSource = ds;


            Array list2 = (Array)Enum.GetValues(typeof(CalculationType));
            ArrayList ds2 = new ArrayList();
            foreach (var itm in list2)
            {
                ds2.Add(itm);
            }
            this.comboBox2.DataSource = ds2;
        }

        #endregion

        #region Event Handlers
        private void okbtn_Click(object sender, EventArgs e)
        {
            comp = this.grid.PivotEngine.PivotCalculations.FirstOrDefault(l => l.FieldName == this.fieldheadertxt.Text);
            if (comp != null && shouldUpdate)
            {
                this.grid.TableControl.BeginUpdate();
                this.grid.TableControl.GroupDropArea.BeginUpdate();
                this.grid.TableControl.RowGroupDropArea.BeginUpdate();
                this.grid.TableControl.FilterArea.BeginUpdate();

                int n1 = this.grid.PivotCalculations.IndexOf(comp);
                this.grid.PivotCalculations.Remove(comp);
                comp.Format = this.formattxt.Text;
                comp.SummaryType = (SummaryType)this.comboBox1.SelectedItem;
                comp.CalculationType = (CalculationType)this.comboBox2.SelectedItem;
                comp.FieldHeader = this.fieldheadertxt.Text;
                comp.Description = this.descriptiontxt.Text;

                this.grid.PivotCalculations.Insert(n1, comp);
                this.grid.TableControl.FilterArea.EndUpdate();
                this.grid.TableControl.RowGroupDropArea.EndUpdate();
                this.grid.TableControl.GroupDropArea.EndUpdate();
                this.grid.TableControl.EndUpdate();
            }

            this.Close();
        }

        private void cancelbtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PivotCompInfo_Load(object sender, EventArgs e)
        {
            shouldUpdate = false;
            if (this.grid != null)
            {
                switch (this.grid.GridVisualStyles)
                {
                    case GridVisualStyles.Metro:
                        this.BorderColor = Color.FromArgb(27, 161, 226);
                        break;

                    case GridVisualStyles.Office2007Blue:
                    case GridVisualStyles.Office2010Blue:
                        this.BorderColor = Color.FromArgb(206, 228, 252);
                        break;
                    
                    case GridVisualStyles.Office2007Black:
                    case GridVisualStyles.Office2007Silver:
                    case GridVisualStyles.Office2010Silver:
                        this.BorderColor = Color.FromArgb(212, 216, 222);
                        break;
                    case GridVisualStyles.Office2010Black:
                        this.BorderColor = Color.Black;
                        break;
                    
                    default:
                        this.BorderColor = Color.Gray;
                        break;

                }
            }
        }
       
        private void text_TextChanged(object sender, EventArgs e)
        {
            shouldUpdate = true;
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            shouldUpdate = true;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.LightGray), 2), this.panel1.ClientRectangle);
        }
        
        #endregion

    }
}
