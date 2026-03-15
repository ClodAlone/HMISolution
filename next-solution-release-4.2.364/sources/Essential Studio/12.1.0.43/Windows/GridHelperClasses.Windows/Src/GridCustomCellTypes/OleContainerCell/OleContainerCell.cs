#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Drawing;
using System.Windows.Forms;
using System.Drawing;
using Syncfusion.Windows.Forms.Tools;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using Syncfusion.Windows.Forms.Grid.Grouping;
using System.Text.RegularExpressions;
namespace Syncfusion.GridHelperClasses
{
    #region the cell model class
    /// <summary>
    /// Implements a data model for the OleContainerCell.
    /// </summary>
    public class OleContainerCellModel : GridGenericControlCellModel
    {
        /// <summary>
        /// Initializes a new <see cref="OleContainerCellModel"/> object. 
        /// </summary>
        /// <param name="grid">GridModel.</param>
        public OleContainerCellModel(GridModel grid)
            : base(grid)
        {

        }
        /// <summary>
        /// Creates a cell renderer.
        /// </summary>
        /// <param name="control">GridControlBase.</param>
        /// <returns>A new <see cref="OleContainerCellRenderer"/>specifies for a <see cref="GridControlBase"/></returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new OleContainerCellRenderer(control, this);
        }
    }
    #endregion

    #region the cell renderer class
    /// <summary>
    /// Implements a cell renderer for the OleContainerCell.
    /// </summary>
    public class OleContainerCellRenderer : GridGenericControlCellRenderer
    {
        private OLEContainer OleContainerCell = new OLEContainer();
        /// <summary>
        /// constructor for OleContainerCellRenderer.
        /// </summary>
        /// <param name="grid">GridControlBase.</param>
        /// <param name="cellModel">GridCellModelBase.</param>
        public OleContainerCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            grid.Controls.Add(OleContainerCell);
            FixControlParent(OleContainerCell);
            //show & hide to make sure it is initilized properly for teh first use...
            OleContainerCell.Show();
            OleContainerCell.Hide();
        }

        #region usual renderer overrides

        /// <summary>
        /// Draws the content of the cell.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="clientRectangle">Cell rectangle.</param>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <param name="style">Cell style information.</param>
        protected override void OnDraw(System.Drawing.Graphics g, System.Drawing.Rectangle clientRectangle, int rowIndex, int colIndex, Syncfusion.Windows.Forms.Grid.GridStyleInfo style)
        {
            style.Control = this.OleContainerCell;

            if (!string.IsNullOrEmpty(style.Description))
            {
                this.OleContainerCell.CreateLink(style.Description, style);
                base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
            }

        }
        /// <summary>
        /// Gets the cursor in the grid
        /// </summary>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <returns></returns>
        protected override System.Windows.Forms.Cursor OnGetCursor(int rowIndex, int colIndex)
        {
            return Cursors.Hand;
        }
        /// <summary>
        /// Sets the layout for the grid
        /// </summary>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="style">GridStyleInfo</param>
        /// <param name="innerBounds">Rectangle</param>
        /// <param name="buttonsBounds">Rectangle</param>
        /// <returns>Rectangle</returns>
        protected override Rectangle OnLayout(int rowIndex, int colIndex, GridStyleInfo style, Rectangle innerBounds, Rectangle[] buttonsBounds)
        {
            Rectangle r = base.OnLayout(rowIndex, colIndex, style, innerBounds, buttonsBounds);

            r.Inflate(-1, -1);

            return r;
        }

        #endregion

        #region Click stuff

        /// <summary>
        /// Action performed while click on the cell.
        /// </summary>
        /// <param name="rowIndex">Row Index.</param>
        /// <param name="colIndex">Column Index.</param>
        /// <param name="e">MouseEventArgs</param>
        protected override void OnClick(int rowIndex, int colIndex, MouseEventArgs e)
        {
            base.OnClick(rowIndex, colIndex, e);
            GridStyleInfo style = this.Grid.Model[rowIndex, colIndex];

            if (e.Button == MouseButtons.Left)
            {
                ClickControl();

                if (!string.IsNullOrEmpty(style.Description))
                {
                    this.OleContainerCell.SetSourceDoc(style.Description);
                    this.OleContainerCell.Open();
                    CurrentCell.Deactivate(true);
                    CurrentCell.SetCurrentCellNoActivate(rowIndex, colIndex);
                }
            }
        }

        private Timer t;
        ///// <internal/>
        private void ClickControl()
        {
            t = new Timer();
            t.Interval = 20;
            t.Tick += new EventHandler(click);
            t.Start();
        }
        /// <internal/>
        private void click(object sender, EventArgs e)
        {
            t.Stop();
            t.Tick -= new EventHandler(click);
            Point p = this.OleContainerCell.PointToClient(Control.MousePosition);
            ActiveXSnapshot.FakeLeftMouseClick(this.OleContainerCell, p);
            t.Dispose();
            t = null;
        }
        #endregion

    }

    #endregion
    #region derived OLEContainer
    /// <summary>
    /// Defines a custom OLEContainer control that can be embedded in a grid cell.
    /// </summary>
    public partial class OLEContainer : Control
    {
        /// <summary>
        /// Ole container for holding dataw
        /// </summary>
        public OLEContainer()
        {
            InitializeComponent();
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(150, 150);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            // 
            // OLEContainer
            // 
            this.Controls.Add(this.pictureBox);
            this.Name = "OLEContainer";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion


        private System.Windows.Forms.PictureBox pictureBox;

        string sourceDoc;  
        /// <summary>
        /// Creates a link to between the Container and Grid Cell to initiate displaying icons in the cells
        /// </summary>
        public void CreateLink(string sourceDoc, GridStyleInfo style)
        {
            if (this.sourceDoc == null)
                this.sourceDoc = sourceDoc;

            Image pBoxImage = null;

            if (style.ImageList != null && style.ImageIndex != -1)
                pBoxImage = style.ImageList.Images[style.ImageIndex];
            else
            {
                if (Icon.ExtractAssociatedIcon(sourceDoc) != null)
                    pBoxImage = Icon.ExtractAssociatedIcon(sourceDoc).ToBitmap();
                else
                {
                    try
                    {
                        // file may be from WEB
                        MatchCollection mc = Regex.Matches(sourceDoc, @"(www[^ \s]+|http[^ \s]+)([\s]|$)", RegexOptions.IgnoreCase);
                        sourceDoc = mc[0].Value;
                        pBoxImage = sourceDoc != null ? DynamicFilterBitmaps.GetBitmap("browser") : DynamicFilterBitmaps.GetBitmap("unk");
                    }
                    catch
                    {
                    }
                }
            }
           
            this.pictureBox.Image = pBoxImage;
            this.pictureBox.Visible = true;
        }

        /// <summary>
        /// Gets the file to open
        /// done in a separate method (not in Create Link) to avoid drawing picture box everytime
        /// </summary>
        internal void SetSourceDoc(string file)
        {
            this.sourceDoc = file;
        }

        /// <summary>
        /// initiates the action to open the file
        /// </summary>
        public void Open()
        {
            Process proc = new Process();
            proc.StartInfo = new ProcessStartInfo(sourceDoc);
            try
            {
                proc.Start();
            }
            catch
            {
                MessageBox.Show("Sorry!! No application is associated with this type of file");
            }
        }
    }

    #endregion

}