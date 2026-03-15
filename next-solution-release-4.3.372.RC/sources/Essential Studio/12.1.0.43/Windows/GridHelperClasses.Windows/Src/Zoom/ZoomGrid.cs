#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.Windows.Forms.Grid;
using System.Windows.Forms;
using System.Drawing;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using System.ComponentModel;
namespace Syncfusion.GridHelperClasses
{
    /// <summary>
    /// Class that enables to perform the zooming operation
    /// </summary>
    public class ZoomGrid  
    {
        /// <summary>
        /// Variable used for setting the zooming option in window
        /// </summary>
       public System.Windows.Forms.PictureBox zoomWindow;
       
        GridControl gridControl;
        GridDataBoundGrid boundgrid;

        bool IsGridcontrol = false;
        bool IsGridDataboundGrid = false;

        private static bool zoomCell_gc = false;
        private static bool zoomCell_gdbg = false;

        int defRowHeight_gc;
        int defColWidth_gc;

        int defRowHeight_gdbg;
        int defColWidth_gdbg;
        
        float fontSize_gc;        
        static float currentPercent_gc;

        float fontSize_gdbg;       
        private static float currentPercent_gdbg;

        /// <summary>
        /// Constructor to initialize the GridControl and  Picture Box
        /// </summary>
        /// <param name="grid"></param>
        public ZoomGrid(GridControl grid)			
		{
            
            gridControl = grid;
            IsGridcontrol = true;                   

            zoomWindow = new System.Windows.Forms.PictureBox();       
            this.zoomWindow.BackColor = System.Drawing.Color.Transparent;           
            this.zoomWindow.Size = new System.Drawing.Size(168, 16);            
            this.zoomWindow.Visible = false;
                      
            fontSize_gc = gridControl.BaseStylesMap["Standard"].StyleInfo.Font.Size;
            defRowHeight_gc = gridControl.DefaultRowHeight;
            defColWidth_gc = gridControl.DefaultColWidth;            
            this.gridControl.TableStyle.WrapText = false;
    
            gridControl.CellClick += new GridCellClickEventHandler(gridControl_CellClick);
            this.zoomWindow.Click += new EventHandler(zoomWindow_Click);            
            gridControl.CurrentCellControlLostFocus += new ControlEventHandler(gridControl_CurrentCellControlLostFocus);
		}
        /// <summary>
        /// Constructor to initialize the DataBoundGrid and  Picture Box
        /// </summary>
        /// <param name="grid"></param>
        public ZoomGrid(GridDataBoundGrid grid)
        {
            boundgrid = grid;
            IsGridDataboundGrid = true;
            
            zoomWindow = new System.Windows.Forms.PictureBox();
            this.zoomWindow.BackColor = System.Drawing.Color.Transparent;
            this.zoomWindow.Size = new System.Drawing.Size(168, 16);
            this.zoomWindow.Visible = false;

            fontSize_gdbg = boundgrid.BaseStylesMap["Standard"].StyleInfo.Font.Size;
            defRowHeight_gdbg = boundgrid.DefaultRowHeight;
            defColWidth_gdbg = boundgrid.DefaultColWidth;

            this.boundgrid.TableStyle.WrapText = false;

            boundgrid.CellClick += new GridCellClickEventHandler(boundgrid_CellClick);
            this.zoomWindow.Click += new EventHandler(zoomWindow_Click);
            this.boundgrid.CurrentCellControlLostFocus += new ControlEventHandler(boundgrid_CurrentCellControlLostFocus);
        }
        /// <summary>
        /// The folowing method may allow the user to get the current zoom percentage of the GridControl
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public int  GetCurrentZoomSize(GridControl grid)
        {
            return int.Parse(currentPercent_gc.ToString());
        }
        /// <summary>
        /// The folowing method may allow the user to get the current zoom percentage of the DataBoundGrid
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public int GetCurrentZoomSize(GridDataBoundGrid grid)
        {
            return int.Parse(currentPercent_gdbg.ToString());
        }
        /// <summary>
        /// Property is set to enable or disable the zooming of the cells in GridControl
        /// </summary>
      public static bool ZoomGridControlCell
        {
            get
            {
                return zoomCell_gc;
            }
            set
            {
                zoomCell_gc = value;
               
            }
        }
      /// <summary>
      /// Property is set to enable or disable the zooming of the cells in GridDataBoundGrid
      /// </summary>
      public static bool ZoomGridDataBoundCell
      {
          get
          {
              return zoomCell_gdbg;
          }
          set
          {
              zoomCell_gdbg = value;
          }
      }

      /// <summary>
      /// The following event is to hide the picture box when current cell lost focus
      /// </summary>
      void boundgrid_CurrentCellControlLostFocus(object sender, ControlEventArgs e)
      {
          if (this.zoomWindow.Visible)
              this.zoomWindow.Visible = false;
      }


      /// <summary>
      /// The following event is to hide the picture box when current cell lost focus
      /// </summary>
      void gridControl_CurrentCellControlLostFocus(object sender, ControlEventArgs e)
      {
          if (this.zoomWindow.Visible)
              this.zoomWindow.Visible = false;
      }

     /// <summary>
     /// The following event is to hide the picture box when clicked on it
     /// </summary>
     /// <param name="sender"></param>
     /// <param name="e"></param>
        void zoomWindow_Click(object sender, EventArgs e)
        {
            if (this.zoomWindow.Visible)
                this.zoomWindow.Visible = false;
        }
        /// <summary>
        /// Following event is to zoom a GridControl cell
        /// Checks if the "ZoomGridDataBoundCell" is true and the zoom percentage is either zero or hundred to zoom the grid
        /// When a cell is clicked, a picture box appears over the cell which shows the value of a grid cell in a larger font size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void gridControl_CellClick(object sender, GridCellClickEventArgs e)
        {
            
                if (ZoomGridControlCell &&( currentPercent_gc==100 ||currentPercent_gc==0) )
                {
                    if (e.RowIndex > 0 && e.ColIndex > 0)
                    {
                        if (!zoomWindow.Visible)
                            this.zoomWindow.Visible = true;
                        Point p1 = new Point(0, 0);
                        Size s = new Size(this.gridControl.ColWidths[e.ColIndex] + 10, this.gridControl.RowHeights[e.RowIndex] + 5);
                        s.Width += 50;
                        s.Height += 30;
                        Rectangle rect = new Rectangle(p1, s);
                        zoomWindow.Size = s;

                        Bitmap bmp = new Bitmap(s.Width, s.Height);
                        Graphics g = Graphics.FromImage(bmp);
                        GridStyleInfo style = gridControl[e.RowIndex, e.ColIndex];                        
                        bool wrapText = style.WrapText;
                        float size = style.Font.Size;
                        style.Font.Size = 15.5f;
                        style.WrapText = true;
                        gridControl.DrawSingleCell(g, e.RowIndex, e.ColIndex, rect, style, true, true);
                        g.Dispose();

                        this.zoomWindow.Image = bmp;
                        this.zoomWindow.BorderStyle = BorderStyle.FixedSingle;
                        this.zoomWindow.Visible = true;
                        Point pt = this.gridControl.ViewLayout.RowColToPoint(e.RowIndex, e.ColIndex, GridCellSizeKind.VisibleSize);
                        pt.X = pt.X - 18;
                        pt.Y = pt.Y - 18;
                        zoomWindow.Location = pt;
                        style.WrapText = wrapText;
                        style.Font.Size = size;
                        this.gridControl.Controls.Add(this.zoomWindow); 
                        this.zoomWindow.BringToFront();
                        this.gridControl.CurrentCell.MoveTo(GridRangeInfo.Cell(e.RowIndex, e.ColIndex), GridSetCurrentCellOptions.ScrollInView);
                        this.gridControl.Refresh();
                    }
                }
                else
                {
                    this.zoomWindow.Visible = false;
                    this.zoomWindow.Hide();
                }
            
                                       
        }
        /// <summary>
        /// Following event is to zoom a DataBoundGrid cell
        /// Checks if the "ZoomGridDataBoundCell" is true and the zoom percentage is either zero or hundred to zoom the grid
        /// When a cell is clicked, a picture box appears over the cell which shows the value of a grid cell in a larger font size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void boundgrid_CellClick(object sender, GridCellClickEventArgs e)
        {           
                if (ZoomGridDataBoundCell && (currentPercent_gdbg == 100 || currentPercent_gdbg == 0))
                {
                    if (e.RowIndex > 0 && e.ColIndex > 0)
                    {
                        if (!zoomWindow.Visible)
                            this.zoomWindow.Visible = true;
                        Point p1 = new Point(0, 0);
                        Size s = new Size(this.boundgrid.Model.ColWidths[e.ColIndex] + 10, this.boundgrid.Model.RowHeights[e.RowIndex] + 5);
                        s.Width += 50;
                        s.Height += 30;
                        Rectangle rect = new Rectangle(p1, s);
                        zoomWindow.Size = s;

                        Bitmap bmp = new Bitmap(s.Width, s.Height);
                        Graphics g = Graphics.FromImage(bmp);
                        GridStyleInfo style = boundgrid[e.RowIndex, e.ColIndex];
                        float size = style.Font.Size;
                        style.Font.Size = 15.5f;
                        boundgrid.DrawSingleCell(g, e.RowIndex, e.ColIndex, rect, style, true, true);
                        g.Dispose();

                        this.zoomWindow.Image = bmp;
                        this.zoomWindow.BorderStyle = BorderStyle.FixedSingle;
                        this.zoomWindow.Visible = true;
                        Point pt = this.boundgrid.ViewLayout.RowColToPoint(e.RowIndex, e.ColIndex, GridCellSizeKind.VisibleSize);
                        pt.X = pt.X - 18;
                        pt.Y = pt.Y - 18;
                        zoomWindow.Location = pt;

                        style.Font.Size = size;
                        this.boundgrid.Controls.Add(this.zoomWindow);
                        this.boundgrid.CurrentCell.MoveTo(GridRangeInfo.Cell(e.RowIndex, e.ColIndex), GridSetCurrentCellOptions.ScrollInView);
                        this.boundgrid.Refresh();
                    }
                }
                else
                {
                    this.zoomWindow.Visible = false;
                    this.zoomWindow.Hide();
                }
           

        }
        /// <summary>
        /// This method determines whether to zoom GridControl or DataBoundGrid or both by checking the boolean variable of each grid
        /// </summary>
        /// <param name="percentage"></param>
        public void zoomGrid(string percentage)
        {            
            this.zoomWindow.Hide();

            if (currentPercent_gc==100)
            {
                ZoomGridControlCell = true;
                ZoomGridDataBoundCell = true;
            }
            else
            {
                ZoomGridControlCell = false;
                ZoomGridDataBoundCell = false;
            }

            if (IsGridDataboundGrid)
            {               
                zoomGridDataBoundGrid(percentage);                
            }

            if (IsGridcontrol)
                zoomGridControl(percentage);            
        }
        /// <summary>
        /// Follwing method is to zoom the GridControl 
        /// "Percent" indicates the percentage of the grid to be zoomed
        /// In this method, each row's height and column's width  and Font Size of each cell are modified according to the percentage
        /// </summary>
        /// <param name="percent"></param>
        private void zoomGridControl(string percent)
        {                    
            currentPercent_gc = float.Parse(percent);
            float usePercent = currentPercent_gc / 100;

                this.gridControl.BeginUpdate();
                for (int i = 0; i <= gridControl.ColCount; i++)
                {
                    for (int j = 0; j <= gridControl.RowCount; j++)
                    {                       
                        this.gridControl[j, i].Font.Size = fontSize_gc * usePercent;
                        this.gridControl.Model.RowHeights[j] = (int)(usePercent * defRowHeight_gc);
                    }
                   
                    this.gridControl.Model.ColWidths[i] = (int)(usePercent * defColWidth_gc);                    
                }             
               
                this.gridControl.EndUpdate();
                this.gridControl.Refresh();                 
        }

        /// <summary>
        /// Follwing method is to zoom the DataboundGrid 
        /// "Percent" indicates the percentage of the grid  to be zoomed
        /// In this method, each row's height and column's width  and Font Size of each cell (column wise) are modified according to the percentage
        /// </summary>
        /// <param name="percent"></param>
        private void zoomGridDataBoundGrid(string percent)
        {  
            currentPercent_gdbg = float.Parse(percent);
            float usePercent = currentPercent_gdbg / 100;

            this.boundgrid.BeginUpdate();
            for (int i = 0; i <= boundgrid.Model.ColCount; i++)
            {
                boundgrid.Model.ColStyles[i].Font.Size = fontSize_gdbg * usePercent;
                this.boundgrid.Model.ColWidths[i] = (int)(usePercent * defColWidth_gdbg);
            }
            for (int i = 0; i <= boundgrid.Model.RowCount; i++)
            {
                this.boundgrid.Model.RowHeights[i] = (int)(usePercent * defRowHeight_gdbg);
            }
            boundgrid.BaseStylesMap["Column Header"].StyleInfo.Font.Size = fontSize_gdbg * usePercent;
            boundgrid.BaseStylesMap["Row Header"].StyleInfo.Font.Size = fontSize_gdbg * usePercent;

            this.boundgrid.EndUpdate();
            this.boundgrid.Refresh();
        }
        
    }  
}
