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
using Syncfusion.Windows.Forms.Grid.Grouping;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Grouping;
using System.Collections;

namespace Syncfusion.GridHelperClasses.Zoom
{
    public partial class ZoomGroupingGrid : Form    
    {
        static bool  zoomCellValue;
        GridGroupingControl groupingGrid;
        int defRowHeight;
        int defColWidth;

        float fontSize;
        /// <summary>
        /// Variable  checking the state of grid
        /// </summary>
        #pragma warning disable
        bool alreadyGridInZoom = false;
        #pragma warning enable
        private static float currentPercent;      
        /// <summary>
        /// Constructor to initialize the GridGroupingControl, Picture Box and the  Popup Container which holds the picturebox
        /// </summary>
        /// <param name="grid"></param>
         public ZoomGroupingGrid(GridGroupingControl grid)
        {
            groupingGrid = grid;            

            this.popupControlContainer1 = new Syncfusion.Windows.Forms.PopupControlContainer();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.popupControlContainer1.Controls.Add(this.pictureBox1);
            this.pictureBox1.BackColor = Color.White;
            this.popupControlContainer1.Size = new System.Drawing.Size(200, 75);
            this.pictureBox1.Size = new System.Drawing.Size(200, 75);           
            
            this.pictureBox1.Visible = false;           
            this.popupControlContainer1.Hide();            
                 
            fontSize = groupingGrid.TableModel.BaseStylesMap["Standard"].StyleInfo.Font.Size;
            defRowHeight = groupingGrid.TableControl.DefaultRowHeight;
            defColWidth = groupingGrid.TableControl.DefaultColWidth;          
           

            this.groupingGrid.TableModel.TableStyle.WrapText = false;

            this.groupingGrid.TableControl.CellClick += new Windows.Forms.Grid.GridCellClickEventHandler(TableControl_CellClick);
            this.groupingGrid.TableControlCurrentCellStartEditing += new GridTableControlCancelEventHandler(groupingGrid_TableControlCurrentCellStartEditing);
            this.groupingGrid.TableControlCurrentCellMoved += new GridTableControlCurrentCellMovedEventHandler(groupingGrid_TableControlCurrentCellMoved);
            this.pictureBox1.Click += new EventHandler(pictureBox1_Click);

            IterateRelatedTable1(this.groupingGrid.TableDescriptor.Relations);
            
         }
         /// <summary>
         /// The following event is to hide the picture box when clicked on it
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
         void pictureBox1_Click(object sender, EventArgs e)
         {
             if (this.pictureBox1.Visible)
             {
                 this.pictureBox1.Visible = false;
                 this.popupControlContainer1.Hide();
                 this.popupControlContainer1.HidePopup();
             }

         }
         bool childclick = false;
         GridTableControl tc;
        /// <summary>
        /// The following method iterates the nested table (child table) to hook the click event for the  specific child table dynamically
        /// </summary>
        /// <param name="relations"></param>
         public void IterateRelatedTable1(GridRelationDescriptorCollection relations)
         {
             foreach (GridRelationDescriptor tb in relations)
             {                
                  tc = this.groupingGrid.GetTableControl(tb.ChildTableName);
                 tc.CellClick += new GridCellClickEventHandler(tc_CellClick);
                 tc.CurrentCellMoved += new GridCurrentCellMovedEventHandler(tc_CurrentCellMoved);
                 tc.CurrentCellStartEditing += new CancelEventHandler(tc_CurrentCellStartEditing);
                 if (relations.Count>0)
                 {
                     IterateRelatedTable1(relations[0].ChildTableDescriptor.Relations);
                 }
             }
         }
         /// <summary>
         /// Following event is cancelled to prevent the current cell to enter into edit mode and ensures zooming the cell
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
         void tc_CurrentCellStartEditing(object sender, CancelEventArgs e)
         {
             if (childclick)
             {
                 e.Cancel = true;
                 childclick = false;
             }
         }
         /// <summary>
         /// Following event disables the boolean variable "click" when focus moves out of current cell
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
         void tc_CurrentCellMoved(object sender, GridCurrentCellMovedEventArgs e)
         {
             childclick = false;
         }
         /// <summary>
         /// Following event is to zoom the GridGroupingControl Child Table cells
         /// Checks if the "ZoomGridDataBoundCell" is true and the zooming percentage is either zero or hundred to zoom the grid
         /// When a cell is clicked, a picture box present within the popup container appears which shows the value of the grid cell in a larger font size
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>

         void tc_CellClick(object sender, GridCellClickEventArgs e)
         {
             GridNestedTableControl control = (GridNestedTableControl)sender;

             if (zoomCell && (currentPercent == 100 || currentPercent == 0))
             {

                 if (e.RowIndex > 0 && e.ColIndex > 0)
                 {
                     style = control.GetViewStyleInfo(e.RowIndex, e.ColIndex);
                     click = true;
                     Point exact_Point = control.ViewLayout.RowColToPoint(e.RowIndex, e.ColIndex, GridCellSizeKind.VisibleSize);
                     p = control.PointToScreen(exact_Point);
                     p.X = p.X - 18;
                     p.Y = p.Y - 18;

                     this.pictureBox1.Paint += new PaintEventHandler(pictureBox1_Paint);
                     this.pictureBox1.BorderStyle = BorderStyle.FixedSingle;
                     this.pictureBox1.Visible = true;
                     this.pictureBox1.BackColor = style.BackColor;
                     this.popupControlContainer1.ShowPopup(p);
                 }
             }
             else
             {
                 this.pictureBox1.Visible = false;
                 this.pictureBox1.Hide();
             }
         }

      /// <summary>
      /// Following event disables the boolean variable click when focus moves out of current cell
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
         void groupingGrid_TableControlCurrentCellMoved(object sender, GridTableControlCurrentCellMovedEventArgs e)
         {
             click = false;
             
         }
         

         bool click = false;
        /// <summary>
        /// Following event is cancelled to prevent the current cell to enter into edit mode and ensures zooming the cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
         void groupingGrid_TableControlCurrentCellStartEditing(object sender, GridTableControlCancelEventArgs e)
         {
             if (click)
             {
                 e.Inner.Cancel = true;
                 click = false;
             }
         }
        
         Point p = new Point();
         GridStyleInfo style;
         /// <summary>
         /// Following event is to zoom the GridGroupingControl Parent Table cells
         /// Checks if the "ZoomGridDataBoundCell" is true and the zoom percentage is either zero or hundred to zoom the grid
         /// When a cell is clicked, a picture box precent with a popupo container appears which shows the value of a grid cell in a larger font size
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>

         void TableControl_CellClick(object sender, Windows.Forms.Grid.GridCellClickEventArgs e)
         {
             if (zoomCell && (currentPercent == 100 || currentPercent == 0))
             {
                 if (e.RowIndex > 0 && e.ColIndex > 0)
                 {
                     click = true;
                     Point exact_Point = this.groupingGrid.TableControl.ViewLayout.RowColToPoint(e.RowIndex, e.ColIndex, GridCellSizeKind.VisibleSize);
                     p = this.groupingGrid.PointToScreen(exact_Point);
                     p.X = p.X - 18;
                     p.Y = p.Y - 18;
                     style = this.groupingGrid.TableControl.GetViewStyleInfo(e.RowIndex, e.ColIndex);

                     this.pictureBox1.Paint += new PaintEventHandler(pictureBox1_Paint);
                     this.pictureBox1.BorderStyle = BorderStyle.FixedSingle;
                     this.pictureBox1.Visible = true;
                     this.pictureBox1.BackColor = style.BackColor;
                     this.popupControlContainer1.ShowPopup(p);
                 }
             }
             else
             {
                 this.pictureBox1.Visible = false;
                 this.pictureBox1.Hide();
             }
         }
        /// <summary>
        /// The following event draws the text in the style object at the specified location with the specified Font size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
         void pictureBox1_Paint(object sender, PaintEventArgs e)
         {             
                 using (Font myFont = new Font("Arial", 15.5f))
                 {
                     e.Graphics.DrawString(style.Text, myFont, Brushes.Black, new Point(2, 2));                     
                 }            

         }
         /// <summary>
         /// Follwing method is to zoom the GridGroupingControl 
         /// "Percent" indicates the percentage of Grid to be zoomed
         /// In this method, each row's height and column's width  and Font Size of any cell are modified according to the percentage
         /// </summary>
         /// <param name="percent"></param>
         public void zoomGrid(string percent)
         {           
             alreadyGridInZoom = true;
             currentPercent = float.Parse(percent);

             if (currentPercent== 100)
             {
                 zoomCell = true;                               
             }
             else
             {
                 zoomCell = false;                
             }

             if (currentPercent == 0)
                 this.groupingGrid.Hide();              
             else
             {
                 if (currentPercent == 100)
                 {
                     for (int i = 0; i <= groupingGrid.TableModel.ColCount; i++)
                     {
                         this.groupingGrid.TableModel.ColWidths[i] = (int)(defColWidth);
                     }
                     for (int i = 0; i <= groupingGrid.TableModel.RowCount; i++)
                     {
                         this.groupingGrid.TableModel.RowHeights[i] = (int)(defRowHeight);
                     }
                     this.groupingGrid.TableDescriptor.Appearance.AnyCell.Font.Size = fontSize;
                     this.groupingGrid.TableModel.ColWidths.ResizeToFit(GridRangeInfo.Cells(0, 0, this.groupingGrid.TableModel.RowCount, this.groupingGrid.TableModel.ColCount), GridResizeToFitOptions.IncludeCellsWithinCoveredRange);
                 }
                 else
                 {
                     float usePercent = currentPercent / 100;
                     this.groupingGrid.TableDescriptor.Appearance.AnyCell.Font.Size = fontSize * usePercent;

                     this.groupingGrid.BeginUpdate();
                     for (int i = 0; i <= groupingGrid.TableModel.ColCount; i++)
                     {
                         this.groupingGrid.TableModel.ColWidths[i] = (int)(usePercent * defColWidth);
                     }
                     for (int i = 0; i <= groupingGrid.TableModel.RowCount; i++)
                     {
                         this.groupingGrid.TableModel.RowHeights[i] = (int)(usePercent * defRowHeight);
                     }

                     this.groupingGrid.TableModel.RowHeights.ResizeToFit(GridRangeInfo.Cells(0, 0, this.groupingGrid.TableModel.RowCount, this.groupingGrid.TableModel.ColCount), GridResizeToFitOptions.IncludeCellsWithinCoveredRange);
                     this.groupingGrid.TableModel.ColWidths.ResizeToFit(GridRangeInfo.Cells(0, 0, this.groupingGrid.TableModel.RowCount, this.groupingGrid.TableModel.ColCount), GridResizeToFitOptions.IncludeCellsWithinCoveredRange);

                 }
                     if(this.groupingGrid.HasChildren)
                     IterateRelatedTable(this.groupingGrid.Table);

                     this.groupingGrid.EndUpdate(false);
                     this.groupingGrid.TableControl.Refresh();
                     this.groupingGrid.Refresh();                 
             }
             
         }

       /// <summary>
       /// Following method is to iterate the nested tables (child and inner child tables) to set their corresponding RowHeight and ColWidths while zooming GridGroupingControl
       /// </summary>
       /// <param name="table"></param>
         void IterateRelatedTable(Table table)
         {           
             foreach (Table tb in table.RelatedTables)
             {
                 GridTableModel tm = this.groupingGrid.GetTableModel(tb.TableDescriptor.Name);
                 
                     tm.RowHeights.ResizeToFit(GridRangeInfo.Cells(0, 0, tm.RowCount, tm.ColCount), GridResizeToFitOptions.IncludeCellsWithinCoveredRange);
                     tm.ColWidths.ResizeToFit(GridRangeInfo.Cells(0, 0, tm.RowCount, tm.ColCount), GridResizeToFitOptions.IncludeCellsWithinCoveredRange);
                 
                 if (tb.RelatedTables.Count > 0)
                 {                
                     IterateRelatedTable(tb);                    
                 }
             }
         }
        /// <summary>
        /// Property is set to enable or disable the zooming of the cells
        /// </summary>
         public static bool zoomCell
         {
             get
             {
                 return zoomCellValue;
             }
             set
             {
                 zoomCellValue = value;
                
             }
         }
         /// <summary>
         /// The folowing method may allow the user to get the zoom percentage of the grid for GridGroupingControl
         /// </summary>
         /// <param name="grid"></param>
         /// <returns></returns>
         public int GetCurrentZoomSize(GridControl grid)
         {
             return int.Parse(currentPercent.ToString());
         }
             
    }
}
    

