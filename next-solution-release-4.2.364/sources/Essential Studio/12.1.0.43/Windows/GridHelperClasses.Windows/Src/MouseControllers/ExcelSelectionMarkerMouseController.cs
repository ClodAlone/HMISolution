#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Any infringement will be prosecuted under
//  applicable laws. 
//
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Windows.Forms;

namespace Syncfusion.GridHelperClasses
{
    /// <summary>
    /// A helper class to set up formula range selection implementing IMouseController interface 
    /// to be used with MouseControllerDispatcher.
    /// </summary>
    /// <remarks>
    /// Any Mouse Controller needs to implement the IMouseController interface.<para/>
    /// In its implementation of MouseController.HitTest, the mouse controller should determine whether your
    /// controller wants to handle the mouse events based current context.<para/>
    /// </remarks>
    public class ExcelSelectionMarkerMouseController:IMouseController
    {
        private GridControlBase owner;
		private GridRangeInfo activeRange,setRange;
		private int lastHitTestCode = GridHitTestContext.None;
		private const int HitExcelMarker = 101;
		private const int delta = 3;
		private DragWindow dragWindow;
        private System.Windows.Forms.Cursor cursor1;
        private string[,] list;
        private string[,] listCpy;
        private string[] libFunc;
        private string[] formulaSep = {"=","+","-","*","/",",",":"};
        private string[] braceSep = { "{", "}", "(", ")", "=" };
        private string[] fullMonth = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        private string[] halfMonth = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        private string[] parseArgumentDateSeperator = { "/", " " };
        private StringCollection operCollection, referCollection,halfMonthCollection,fullMonthCollection, libFunctions;
        private string formattedVal = string.Empty;
        private System.Windows.Forms.ContextMenuStrip menuStrip;
        private bool isCopySeries = false;
        private DateTime dateTime = DateTime.Now;
        private GridFormulaEngine engine;

        /// <summary>
        /// Initializes a new <see cref="ExcelSelectionMarkerMouseController"/> and attaches it to a grid.
        /// </summary>
        /// <param name="owner">The grid control</param>
		public ExcelSelectionMarkerMouseController(GridControlBase owner)
		{   
			this.owner = owner;
            engine = new GridFormulaEngine(owner.Model);
            operCollection = new StringCollection();
            referCollection = new StringCollection();
            halfMonthCollection = new StringCollection();
            fullMonthCollection = new StringCollection();
            libFunctions = new StringCollection();
            libFunc = new string[engine.LibraryFunctions.Count];
            menuStrip = new System.Windows.Forms.ContextMenuStrip();
            operCollection.AddRange(formulaSep);
            engine.LibraryFunctions.Keys.CopyTo(libFunc, 0);
            libFunctions.AddRange(libFunc);   

            halfMonthCollection.AddRange(halfMonth);
            fullMonthCollection.AddRange(fullMonth);
            menuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(menuStrip_ItemClicked);
            menuStrip.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(menuStrip_Closed);
            owner.KeyDown += new System.Windows.Forms.KeyEventHandler(owner_KeyDown);
		}
        /// <summary>
        /// A method to support Ctrl+D or Ctrl+R  for fill series like excel.
        /// </summary>
        /// <param name="sender">GridDataBoundGrid</param>
        /// <param name="e">The event data.</param>
        void owner_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            GridRangeInfo range = owner.Model.SelectedRanges.ActiveRange;
            GridStyleInfoStoreTable table;
            //Ctrl + D
            if (e.Control && e.KeyCode == Keys.D)
            {
                if (range.Height == 1)
                {
                    if (owner.Model[range.Top - 1, range.Left] != null)
                    {
                        table = owner.Model.GetCells(GridRangeInfo.Cell(range.Top - 1, range.Left));
                        owner.Model.SetCells(GridRangeInfo.Cell(range.Top, range.Left), table, false, true);
                    }
                }
                else
                {
                    owner.CurrentCell.MoveTo(range.Bottom, range.Right);
                    for (int i = range.Top; i < range.Bottom; i++)
                    {
                        for (int j = range.Left; j <= range.Right; j++)
                        {
                            table = owner.Model.GetCells(GridRangeInfo.Cell(i, j));
                            owner.Model.SetCells(GridRangeInfo.Cell(i + 1, j), table, false, true);
                        }
                    }
                }
            }

            //Ctrl + R
            if (e.Control && e.KeyCode == Keys.R)
            {
                if (range.Width == 1)
                {
                    if (owner.Model[range.Top, range.Left - 1] != null)
                    {
                        table = owner.Model.GetCells(GridRangeInfo.Cell(range.Top, range.Left - 1));
                        owner.Model.SetCells(GridRangeInfo.Cell(range.Top, range.Left), table, false, true);
                    }
                }
                else
                {
                    owner.CurrentCell.MoveTo(range.Bottom, range.Right);
                    for (int i = range.Top; i <= range.Bottom; i++)
                    {
                        for (int j = range.Left; j < range.Right; j++)
                        {
                            table = owner.Model.GetCells(GridRangeInfo.Cell(i, j));
                            owner.Model.SetCells(GridRangeInfo.Cell(i, j + 1), table, false, true);
                        }
                    }
                }
            }
            owner.RefreshRange(range);
        }

        /// <summary>
        /// A method to remove drag window on closing context menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="ToolStripDropDownClosedEventArgs"/> holding event data.</param>
        private void menuStrip_Closed(object sender, System.Windows.Forms.ToolStripDropDownClosedEventArgs e)
        {
            if (dragWindow != null)
            {
                dragWindow.Visible = false;
                dragWindow = null;
            }
        }

        /// <summary>
        /// A method that selects an item in context menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="ToolStripItemClickedEventArgs"/> holding event data.</param>
        private void menuStrip_ItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {
            dragWindow.Visible = false;
            dragWindow = null;
            if (e.ClickedItem.Text == "Fill Series")
            {
                isCopySeries = false;
                FillSeries(range, rowIndex, colIndex, i, j);
            }
            else if (e.ClickedItem.Text == "Copy Series")
            {
                isCopySeries = true;
                FillSeries(range, rowIndex, colIndex, i, j);
            }
        }

        /// <summary>
        /// A method to get bounds of the specific range passed in.
        /// </summary>
        /// <param name="range">The range from which bounds to be taken.</param>
        /// <returns>Dimensions of rectangular range.</returns>
        private Rectangle GetBounds(GridRangeInfo range)
        {
            Rectangle bounds = owner.RangeInfoToRectangle(range, GridRangeOptions.None);
            bounds.Intersect(owner.ClientRectangle);
            bounds = owner.RectangleToScreen(bounds);
            return bounds;
        }

        #region IMouseController Members
        /// <summary>
        /// CancelMode is called for the active controller after a MouseDown message when the mouse operation is cancelled.
        /// </summary>
        public void CancelMode()
        {
            
        }

        /// <summary>
        /// Cursor will only be called if previous call to HitTest was successful.
        /// </summary>
        public System.Windows.Forms.Cursor Cursor
        {
            get
            {
                if (cursor1 == null)
                {
                    System.IO.Stream stream = GetType().Module.Assembly.GetManifestResourceStream("ExcelMarker.cross.CUR");
                    if (stream != null)
                        cursor1 = new System.Windows.Forms.Cursor(stream);
                    else
                        cursor1 = System.Windows.Forms.Cursors.Cross;

                }
                // could check latestHitTestCode here if this controller has
                // different HitTest states.
                return cursor1;
            }
        }

        /// <summary>
        /// HitTest is called to determine whether your controller wants to handle the mouse events based current context.
        /// </summary>
        /// <remarks>
        /// The current winner of the vote is specified through the controller paramter. Your implementation of HitTest
        /// can decide if it wants to override the existing vote or leave it.
        /// </remarks>
        /// <param name="mouseEventArgs">A <see cref="MouseEventArgs"/> holding event data..</param>
        /// <param name="controller">A <see cref="IMouseController"/> that has indicated to handle the mouse event.</param>
        /// <returns>A non-zero value if the button can and wants to handle the mouse event; 0 if the
        /// mouse event is unrelated for this button.</returns>
        public int HitTest(System.Windows.Forms.MouseEventArgs mouseEventArgs, IMouseController controller)
        {
            lastHitTestCode = GridHitTestContext.None;

            // Our vote has higher priority than other controllers
            if (mouseEventArgs.Button == System.Windows.Forms.MouseButtons.Left)
            {
                GridRangeInfo range = owner.Selections.Ranges.ActiveRange;
                if (!range.IsEmpty)
                {
                    Point point = new Point(mouseEventArgs.X, mouseEventArgs.Y);
                    point.Offset(-delta, -delta);
                    int rowIndex, colIndex;
                    owner.PointToRowCol(point, out rowIndex, out colIndex);

                    if (rowIndex == range.Bottom && colIndex == range.Right)
                    {
                        point.Offset(delta * 2 + 1, delta * 2 + 1);
                        int rowIndex2, colIndex2;
                        owner.PointToRowCol(point, out rowIndex2, out colIndex2);
                        if (rowIndex2 != rowIndex && colIndex2 != colIndex)
                            lastHitTestCode = HitExcelMarker;
                    }
                }
            }
            return lastHitTestCode;
        }

        /// <summary>
        /// MouseDown is called when this controller signaled in HitTest that it wants to handle mouse events and the
        /// user pressed the mouse button.
        /// </summary>
        /// <remarks>
        /// MouseDown is called and this controller will become the active controller and receive all subsequent mouse message
        /// until the mouse button is released or the mouse operation is cancelled.
        /// </remarks>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public void MouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (e.Clicks == 1)
                {
                    // Start automatic scrolling when user drags mouse out of window
                    owner.AutoScrolling = System.Windows.Forms.ScrollBars.Both;

                    // 
                    int rowIndex, colIndex;
                    Point point = new Point(e.X, e.Y);
                    owner.PointToRowCol(point, out rowIndex, out colIndex);
                    activeRange = owner.Selections.Ranges.ActiveRange;

                    dragWindow = new DragWindow();
                    dragWindow.Opacity = .4f;
                    dragWindow.BackColor = SystemColors.Menu;
                    dragWindow.Bounds = GetBounds(activeRange);
                    dragWindow.ShowWindowTopMost();
                }
            }
        }

        /// <summary>
        /// MouseHover is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHover
        /// is called after MouseHoverEnter.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public void MouseHover(System.Windows.Forms.MouseEventArgs e)
        {
            
        }

        /// <summary>
        /// MouseHoverEnter is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHoverEnter
        /// is called before the first time MouseHover is called.
        /// </summary>
        public void MouseHoverEnter()
        {
            
        }

        /// <summary>
        /// MouseHoverLeave is called when the hovering ends, either when user has moved mouse away from hittest area
        /// or when the user has pressed a mouse button.
        /// </summary>
        /// <param name="e"></param>
        public void MouseHoverLeave(EventArgs e)
        {
            
        }

        /// <summary>
        /// MouseMove is called for the active controller after a MouseDown message when the user moves the mouse pointer.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public void MouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            // Set current position
            int rowIndex, colIndex;
            Point point = new Point(e.X - delta, e.Y - delta);
            try
            {
                owner.PointToRowCol(point, out rowIndex, out colIndex, 0);
                rowIndex = Math.Max(owner.TopRowIndex, rowIndex);
                colIndex = Math.Max(owner.LeftColIndex, colIndex);
                GridRangeInfo range;
                Rectangle rect = owner.GetCellRenderer(rowIndex, colIndex).GetCellBoundsCore(rowIndex, colIndex);
                if (colIndex > activeRange.Left)
                {
                    Rectangle halfRect = new Rectangle(rect.Location, new Size(rect.Width / 2, rect.Height));
                    if (halfRect.Contains(point))
                        colIndex -= 1;
                }
                else if (colIndex < activeRange.Left)
                {
                    Rectangle halfRect = new Rectangle(new Point(rect.X + (rect.Width / 2),rect.Y), new Size(rect.Width / 2, rect.Height));
                    if (halfRect.Contains(point))
                        colIndex += 1;
                }

                if (rowIndex > activeRange.Top)
                {
                    Rectangle midRect = new Rectangle(rect.Location, new Size(rect.Width, rect.Height / 2));
                    if (midRect.Contains(point))
                        rowIndex -= 1;
                }
                else if (rowIndex < activeRange.Top)
                {
                    Rectangle midRect = new Rectangle(new Point(rect.X,rect.Y + (rect.Height / 2)), new Size(rect.Width, rect.Height / 2));
                    if (midRect.Contains(point))
                        rowIndex += 1;
                }

                if (rowIndex != activeRange.Bottom && colIndex <= activeRange.Right && colIndex >= activeRange.Left)
                    range = GridRangeInfo.Cells(activeRange.Top, activeRange.Left, rowIndex, activeRange.Right);
                else
                    range = GridRangeInfo.Cells(activeRange.Top, activeRange.Left, activeRange.Bottom, colIndex);
                setRange = range;
                dragWindow.Bounds = GetBounds(range);
                dragWindow.ShowWindowTopMost();
            }
            catch
            {
            }
        }

        int rowIndex, colIndex, i = -1, j = -1;
        GridRangeInfo range = GridRangeInfo.Empty;

        /// <summary>
        /// MouseUp is called for the active controller after a MouseDown message when the user releases the mouse button.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public void MouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            // Stop automatic scrolling 
            owner.AutoScrolling = System.Windows.Forms.ScrollBars.None;

            Point point = new Point(e.X, e.Y);
            owner.PointToRowCol(point, out rowIndex, out colIndex);
           
            menuStrip.Items.Clear();
            menuStrip.Items.Add("Copy Series");
            menuStrip.Items.Add("Fill Series");

            Rectangle rect = owner.GetCellRenderer(setRange.Bottom, setRange.Right).GetCellBoundsCore(setRange.Bottom, setRange.Right);
            range = setRange;
            if (dragWindow == null)
            {
                rowIndex = -1;
                colIndex = -1;
                range = GridRangeInfo.Empty;
                i = -1;
                j = -1;
            }
            Point pt = new Point(rect.X + rect.Width, rect.Y + rect.Height);
            menuStrip.Show(owner,pt);
        }
        /// <summary>
        /// Fillseries for numeric ended single and multi-line cell values.
        /// </summary>
        /// <param name="strValue">Source text value.</param>
        private void SetNumericEndedCells(ref string strValue)
        {
            string value = string.Empty;
            int end = strValue.Length - 1;
            while (end >= 0)
            {
                if (!char.IsDigit(strValue[end]))
                {
                    break;
                }
                else
                {
                    value = strValue[end] + value;
                }
                end--;
            }
            string zeros = string.Empty;
            int index = value.Length;
            int res = 0;
            while (index > 0)
            {
                zeros += "0";
                index--;
            }
            if (int.TryParse(value, out res))
            {
                res++;
                strValue = strValue.Substring(0, strValue.Length - value.Length) + res.ToString(zeros);
            }
        }

        /// <summary>
        /// A method to apply Fill Series/Copy Series functionality in selected range.
        /// </summary>
        /// <param name="range">Range to which series to be filled in.</param>
        /// <param name="rowIndex">Row Index of the last cell in selected range.</param>
        /// <param name="colIndex">Column Index of the last cell in selected range.</param>
        /// <param name="i">An integer variable to iterate through cells.</param>
        /// <param name="j">An integer variable to iterate through cells.</param>
        private void FillSeries(GridRangeInfo range, int rowIndex, int colIndex, int i, int j)
        {
            bool first = true;
            int res, p = 0, iterateRowIndex = -1;
            double val;
            double prev = 0.0;
            string[,] format;
            DateTime dt;
            GridStyleInfoStoreTable styleStore = null;
            
            list = new string[activeRange.Bottom - activeRange.Top + 1, activeRange.Right - activeRange.Left + 1];
            listCpy = new string[activeRange.Bottom - activeRange.Top + 1, activeRange.Right - activeRange.Left + 1];
            format = new string[activeRange.Bottom - activeRange.Top + 1, activeRange.Right - activeRange.Left + 1];

            for (int row = activeRange.Top; row <= activeRange.Bottom; row++)
            {
                i++;
                j = -1;
                for (int col = activeRange.Left; col <= activeRange.Right; col++)
                {
                    j++;
                    owner.CurrentCell.MoveTo(row, col);
                    if (!isCopySeries && owner.GetCellRenderer(row, col).StyleInfo.CellValueType == typeof(DateTime))
                    {
                        listCpy[i, j] = list[i, j] = owner.GetCellRenderer(row, col).ControlValue.ToString();
                        format[i, j] = owner.GetCellRenderer(row, col).StyleInfo.Format;
                    }
                    else if (owner.GetCellRenderer(row, col) is GridCheckBoxCellRenderer)
                    {
                        styleStore = owner.Model.GetCells(GridRangeInfo.Cell(row, col));
                        format[i, j] = "CheckBox";
                        listCpy[i, j] = list[i, j] = owner.GetCellRenderer(row, col).ControlText;
                    }
                    else
                    {
                        listCpy[i, j] = list[i, j] = owner.GetCellRenderer(row, col).ControlText;
                    }
                }
            }
            i = -1;
            if (activeRange.Top == range.Top && activeRange.Bottom == range.Bottom)
            {
                if (activeRange.Left != range.Left)
                {
                    int count = activeRange.Left - range.Left;

                    if (count > 0)
                    {
                        int m, n, l = -1;
                        for (m = 0, i = activeRange.Top; i <= activeRange.Bottom; i++, m++)
                        {
                            if (m == (activeRange.Bottom - activeRange.Top + 1))
                                m = 0;
                            for (n = 0, l = activeRange.Right - activeRange.Left, j = activeRange.Left - 1; j >= range.Left; j--, l--, n++)
                            {
                                if (n == (activeRange.Right - activeRange.Left + 1))
                                    n = 0;
                                if (l == -1)
                                    l = activeRange.Right - activeRange.Left;
                                owner.CurrentCell.MoveTo(i, j);
                                double x = 0.0;
                                int y = activeRange.Bottom - activeRange.Top + 1;
                                if (y == 1)
                                {
                                    if (!string.IsNullOrEmpty(format[m, n]) && format[m, n].Equals("CheckBox"))
                                    {
                                        if (owner.GetCellRenderer(i, j).StyleInfo.CellValueType == null
                                            || owner.GetCellRenderer(i, j).StyleInfo.CellValueType == typeof(bool)
                                            || owner.GetCellRenderer(i, j).StyleInfo.CellValueType == typeof(string))
                                        {
                                            owner.Model.SetCells(GridRangeInfo.Cell(i, j), styleStore, false, true);
                                        }
                                    }
                                    else if (DateTime.TryParse(list[m, n], out dt) && !isCopySeries)
                                    {
                                        dt = dt.AddDays(1);
                                        list[m, n] = dt.ToString();
                                        string date = !string.IsNullOrEmpty(format[m, n]) ? string.Format("{0:" + format[m, n] + "}", dt) : dt.ToString();
                                        owner.CurrentCell.Renderer.ControlText = date;
                                    }
                                    else if (double.TryParse(list[m, n], out x))
                                    {
                                        if (!isCopySeries)
                                            x -= 1;
                                        list[m, n] = x.ToString();
                                        owner.CurrentCell.Renderer.ControlText = list[m, n];
                                    }
                                    else if (list[m, n].StartsWith("="))
                                    {
                                        if (isCopySeries)
                                            owner.CurrentCell.Renderer.ControlText = list[m, n];
                                        else
                                        {
                                            string[] formulaVal = list[m, n].Split(formulaSep, StringSplitOptions.RemoveEmptyEntries);
                                            string dep = string.Empty;
                                            for (int d = 0; d < formulaVal.Length; d++)
                                            {
                                                dep = formulaVal[d];
                                                foreach (string lib in libFunc)
                                                    if (dep.Contains(lib))
                                                        dep = dep.Replace(lib, string.Empty);
                                                foreach (string brace in braceSep)
                                                    if (dep.Contains(brace))
                                                        dep = dep.Replace(brace, string.Empty);
                                                formulaVal[d] = dep;
                                            }
                                            int value1;
                                            Hashtable ht = new Hashtable();
                                            for (int r = 0; r < formulaVal.Length; r++)
                                            {
                                                ht.Add("enc" + r, formulaVal[r]);
                                                list[m, n] = list[m, n].Replace(formulaVal[r], "enc" + r);
                                            }

                                            for (int q = 0; q < formulaVal.Length; q++)
                                            {
                                                string s1 = formulaVal[q];
                                                int ch1;
                                                for (ch1 = 0; ch1 < s1.Length; ch1++)
                                                {
                                                    if (int.TryParse(s1[ch1].ToString(), out value1))
                                                        break;
                                                }
                                                string str1 = s1.Substring(0, ch1);
                                                formattedVal = s1.Remove(0, ch1);
                                                char[] charac = str1.ToCharArray();
                                                bool c = false;
                                                int len1 = 0;
                                                for (int len = charac.Length - 1; len >= 0; len--)
                                                {
                                                    if (charac[len].ToString().Equals("a"))
                                                    {
                                                        charac.SetValue('z', len);
                                                        c = true;
                                                    }
                                                    else if (charac[len].ToString().Equals("A"))
                                                    {
                                                        charac.SetValue('Z', len);
                                                        c = true;
                                                    }
                                                    else
                                                    {
                                                        charac.SetValue(((char)(charac[len] - 1)), len);
                                                        c = false;
                                                        break;
                                                    }
                                                }
                                                str1 = string.Empty;
                                                if (c)
                                                {
                                                    charac.SetValue(' ', 0);
                                                    len1 = 1;
                                                    c = false;
                                                    if (len1.Equals(charac.Length))
                                                    {
                                                        list[m, n] = "#REF!";
                                                        break;
                                                    }
                                                }
                                                for (int len = len1; len < charac.Length; len++)
                                                {
                                                    str1 += charac[len].ToString();
                                                }
                                                ht["enc" + q] = str1 + formattedVal;
                                            }

                                            for (int r = 0; r < formulaVal.Length; r++)
                                            {
                                                list[m, n] = list[m, n].Replace("enc" + r, ht["enc" + r].ToString());
                                            }
                                            owner.CurrentCell.Renderer.ControlText = list[m, n];
                                        }
                                    }
                                    else if (fullMonthCollection.Contains(list[m, n]))
                                    {
                                        if (!isCopySeries)
                                        {
                                            int index = fullMonthCollection.IndexOf(list[m, n]);
                                            if (index == 0)
                                                index = fullMonthCollection.Count;
                                            list[m, n] = fullMonthCollection[index - 1];
                                        }
                                        owner.CurrentCell.Renderer.ControlText = list[m, n];
                                    }
                                    else if (halfMonthCollection.Contains(list[m, n]))
                                    {
                                        if (!isCopySeries)
                                        {
                                            int index = halfMonthCollection.IndexOf(list[m, n]);
                                            if (index == 0)
                                                index = halfMonthCollection.Count;
                                            list[m, n] = halfMonthCollection[index - 1];
                                        }
                                        owner.CurrentCell.Renderer.ControlText = list[m, n];
                                    }
                                    else if (list[m, n] == "#REF!")
                                        owner.CurrentCell.Renderer.ControlText = list[m, n];
                                    else if (int.TryParse(list[m, n][list[m, n].Length - 1].ToString(), out res) && !isCopySeries)
                                    {
                                        SetNumericEndedCells(ref list[m, n]);
                                        owner.CurrentCell.Renderer.ControlText = list[m, n];
                                    }
                                }
                                else if (y == 2)
                                {
                                    double val1, val2;
                                    if (double.TryParse(listCpy[1, n], out val1) && double.TryParse(listCpy[0, n], out val2))
                                    {
                                        val1 = double.Parse(listCpy[1, n]);
                                        val2 = double.Parse(listCpy[0, n]);
                                        if (first || iterateRowIndex != m)
                                        {
                                            prev = double.Parse(listCpy[0, n]);
                                            first = false;
                                            iterateRowIndex = m;
                                        }
                                        x = val1 - val2;
                                        if(!isCopySeries)
                                            prev -= x;
                                        list[m, n] = prev.ToString();
                                        owner.CurrentCell.Renderer.ControlText = list[m, n];
                                    }
                                    else if (list[m, n].StartsWith("="))
                                    {
                                        if (isCopySeries)
                                            owner.CurrentCell.Renderer.ControlText = list[m, n];
                                        else
                                        {
                                            string[] formulaVal = list[m, n].Split(formulaSep, StringSplitOptions.RemoveEmptyEntries);
                                            string dep = string.Empty;
                                            for (int d = 0; d < formulaVal.Length; d++)
                                            {
                                                dep = formulaVal[d];
                                                foreach (string lib in libFunc)
                                                    if (dep.Contains(lib))
                                                        dep = dep.Replace(lib, string.Empty);
                                                foreach (string brace in braceSep)
                                                    if (dep.Contains(brace))
                                                        dep = dep.Replace(brace, string.Empty);
                                                formulaVal[d] = dep;
                                            }
                                            int value1;
                                            Hashtable ht = new Hashtable();
                                            for (int r = 0; r < formulaVal.Length; r++)
                                            {
                                                ht.Add("enc" + r, formulaVal[r]);
                                                list[m, n] = list[m, n].Replace(formulaVal[r], "enc" + r);
                                            }

                                            for (int q = 0; q < formulaVal.Length; q++)
                                            {
                                                string s1 = formulaVal[q];
                                                int ch1;
                                                for (ch1 = 0; ch1 < s1.Length; ch1++)
                                                {
                                                    if (int.TryParse(s1[ch1].ToString(), out value1))
                                                        break;
                                                }
                                                string str1 = s1.Substring(0, ch1);
                                                formattedVal = s1.Remove(0, ch1);
                                                char[] charac = str1.ToCharArray();
                                                bool c = false;
                                                int len1 = 0;
                                                for (int len = charac.Length - 1; len >= 0; len--)
                                                {
                                                    if (charac[len].ToString().Equals("a"))
                                                    {
                                                        charac.SetValue('z', len);
                                                        c = true;
                                                    }
                                                    else if (charac[len].ToString().Equals("A"))
                                                    {
                                                        charac.SetValue('Z', len);
                                                        c = true;
                                                    }
                                                    else
                                                    {
                                                        charac.SetValue(((char)(charac[len] - 1)), len);
                                                        c = false;
                                                        break;
                                                    }
                                                }
                                                str1 = string.Empty;
                                                if (c)
                                                {
                                                    charac.SetValue(' ', 0);
                                                    len1 = 1;
                                                    c = false;
                                                    if (len1.Equals(charac.Length))
                                                    {
                                                        list[m, n] = "#REF!";
                                                        break;
                                                    }
                                                }
                                                for (int len = len1; len < charac.Length; len++)
                                                {
                                                    str1 += charac[len].ToString();
                                                }
                                                ht["enc" + q] = str1 + formattedVal;
                                            }

                                            for (int r = 0; r < formulaVal.Length; r++)
                                            {
                                                list[m, n] = list[m, n].Replace("enc" + r, ht["enc" + r].ToString());
                                            }
                                            owner.CurrentCell.Renderer.ControlText = list[m, n];
                                        }
                                    }
                                    else if (fullMonthCollection.Contains(list[m, l]))
                                    {
                                        if (!isCopySeries)
                                        {
                                            int index = fullMonthCollection.IndexOf(list[m, l]);
                                            if (index == 0)
                                                index = fullMonthCollection.Count;
                                            list[m, l] = fullMonthCollection[index - 1];
                                        }
                                        owner.CurrentCell.Renderer.ControlText = list[m, l];
                                    }
                                    else if (halfMonthCollection.Contains(list[m, l]))
                                    {
                                        if (!isCopySeries)
                                        {
                                            int index = halfMonthCollection.IndexOf(list[m, l]);
                                            if (index == 0)
                                                index = halfMonthCollection.Count;
                                            list[m, l] = halfMonthCollection[index - 1];
                                        }
                                        owner.CurrentCell.Renderer.ControlText = list[m, l];
                                    }
                                    else
                                    {
                                        //code for irregular intervals
                                        if (int.TryParse(list[m, l], out res))
                                        {
                                            if(!isCopySeries)
                                                res--;
                                            owner.CurrentCell.Renderer.ControlText = res.ToString();
                                            list[m, l] = res.ToString();
                                        }
                                        else if (double.TryParse(list[m, l], out val))
                                        {
                                            if(!isCopySeries)
                                                val--;
                                            owner.CurrentCell.Renderer.ControlText = val.ToString();
                                            list[m, l] = val.ToString();
                                        }
                                        else
                                            owner.CurrentCell.Renderer.ControlText = list[m, l];
                                    }
                                }
                                else if (y > 2)
                                {
                                    double val1, val2, val3;
                                    if (double.TryParse(listCpy[2, n], out val1) && double.TryParse(listCpy[1, n], out val2) && double.TryParse(listCpy[0, n], out val3))
                                    {
                                        val1 = double.Parse(listCpy[2, n]);
                                        val2 = double.Parse(listCpy[1, n]);
                                        val3 = double.Parse(listCpy[0, n]);
                                        if ((val1 - val2) == (val2 - val3))
                                        {
                                            if (first || iterateRowIndex != m)
                                            {
                                                prev = double.Parse(listCpy[0, n]);
                                                first = false;
                                                iterateRowIndex = m;
                                            }
                                            x = val1 - val2;
                                            if(!isCopySeries)
                                                prev -= x;
                                            list[m, n] = prev.ToString();
                                            owner.CurrentCell.Renderer.ControlText = list[m, n];
                                        }
                                        else
                                        {
                                            //code for irregular intervals
                                            if (first || iterateRowIndex != m)
                                            {
                                                first = false;
                                                iterateRowIndex = m;
                                            }
                                            if (double.TryParse(listCpy[l, n], out prev))
                                            {
                                                if(!isCopySeries)
                                                    prev -= 1;
                                                list[m, n] = prev.ToString();
                                            }
                                            else
                                                list[m, n] = listCpy[l, n];
                                            owner.CurrentCell.Renderer.ControlText = list[m, n];
                                        }
                                    }
                                    else if (list[m, n].StartsWith("="))
                                    {
                                        if (isCopySeries)
                                            owner.CurrentCell.Renderer.ControlText = list[m, n];
                                        else
                                        {
                                            string[] formulaVal = list[m, n].Split(formulaSep, StringSplitOptions.RemoveEmptyEntries);
                                            string dep = string.Empty;
                                            for (int d = 0; d < formulaVal.Length; d++)
                                            {
                                                dep = formulaVal[d];
                                                foreach (string lib in libFunc)
                                                    if (dep.Contains(lib))
                                                        dep = dep.Replace(lib, string.Empty);
                                                foreach (string brace in braceSep)
                                                    if (dep.Contains(brace))
                                                        dep = dep.Replace(brace, string.Empty);
                                                formulaVal[d] = dep;
                                            }
                                            int value1;
                                            Hashtable ht = new Hashtable();
                                            for (int r = 0; r < formulaVal.Length; r++)
                                            {
                                                ht.Add("enc" + r, formulaVal[r]);
                                                list[m, n] = list[m, n].Replace(formulaVal[r], "enc" + r);
                                            }

                                            for (int q = 0; q < formulaVal.Length; q++)
                                            {
                                                string s1 = formulaVal[q];
                                                int ch1;
                                                for (ch1 = 0; ch1 < s1.Length; ch1++)
                                                {
                                                    if (int.TryParse(s1[ch1].ToString(), out value1))
                                                        break;
                                                }
                                                string str1 = s1.Substring(0, ch1);
                                                formattedVal = s1.Remove(0, ch1);
                                                char[] charac = str1.ToCharArray();
                                                bool c = false;
                                                int len1 = 0;
                                                for (int len = charac.Length - 1; len >= 0; len--)
                                                {
                                                    if (charac[len].ToString().Equals("a"))
                                                    {
                                                        charac.SetValue('z', len);
                                                        c = true;
                                                    }
                                                    else if (charac[len].ToString().Equals("A"))
                                                    {
                                                        charac.SetValue('Z', len);
                                                        c = true;
                                                    }
                                                    else
                                                    {
                                                        charac.SetValue(((char)(charac[len] - 1)), len);
                                                        c = false;
                                                        break;
                                                    }
                                                }
                                                str1 = string.Empty;
                                                if (c)
                                                {
                                                    charac.SetValue(' ', 0);
                                                    len1 = 1;
                                                    c = false;
                                                    if (len1.Equals(charac.Length))
                                                    {
                                                        list[m, n] = "#REF!";
                                                        break;
                                                    }
                                                }
                                                for (int len = len1; len < charac.Length; len++)
                                                {
                                                    str1 += charac[len].ToString();
                                                }
                                                ht["enc" + q] = str1 + formattedVal;
                                            }

                                            for (int r = 0; r < formulaVal.Length; r++)
                                            {
                                                list[m, n] = list[m, n].Replace("enc" + r, ht["enc" + r].ToString());
                                            }
                                            owner.CurrentCell.Renderer.ControlText = list[m, n];
                                        }
                                    }
                                    else
                                    {
                                        //code for irregular intervals
                                        if (first || iterateRowIndex != m)
                                        {
                                            first = false;
                                            iterateRowIndex = m;
                                        }
                                        if (double.TryParse(listCpy[m, l], out prev))
                                        {
                                            if (!isCopySeries)
                                                prev -= 1;
                                            list[m, n] = prev.ToString();
                                        }
                                        else if (int.TryParse(list[m, n][list[m, n].Length - 1].ToString(), out res) && !isCopySeries)
                                        {
                                            SetNumericEndedCells(ref list[m, n]);
                                        }
                                        else
                                            list[m, n] = listCpy[m, l];
                                        owner.CurrentCell.Renderer.ControlText = list[m, n];
                                    }
                                }
                            }
                        }
                    }
                    //Console.WriteLine("Right-Left");
                }
                else
                {
                    int count = range.Right - activeRange.Right;
                    if (count > 0)
                    {
                        int m = -1, n = -1;
                        for (m = 0, i = activeRange.Top; i <= activeRange.Bottom; i++, m++)
                        {
                            if (m == (activeRange.Bottom - activeRange.Top + 1))
                                m = 0;
                            referCollection.Clear();
                            for (n = 0, j = activeRange.Right + 1; j <= range.Right; j++, n++)
                            {
                                if (n == (activeRange.Right - activeRange.Left + 1))
                                    n = 0;
                                owner.CurrentCell.MoveTo(i, j);
                                for (int k = 0; k < (activeRange.Right - activeRange.Left + 1); k++)
                                    if (!int.TryParse(list[m, k], out res) && !double.TryParse(list[m, k], out val))
                                        p++;
                                if (p > 0)
                                {
                                    p = 0;
                                    if (!string.IsNullOrEmpty(format[m, n]) && format[m, n].Equals("CheckBox"))
                                    {
                                        if (owner.GetCellRenderer(i, j).StyleInfo.CellValueType == null
                                            || owner.GetCellRenderer(i, j).StyleInfo.CellValueType == typeof(bool)
                                            || owner.GetCellRenderer(i, j).StyleInfo.CellValueType == typeof(string))
                                        {
                                            owner.Model.SetCells(GridRangeInfo.Cell(i, j), styleStore, false, true);
                                        }
                                    }
                                    else if (DateTime.TryParse(list[m, n], out dt) && !isCopySeries)
                                    {
                                        dt = dt.AddDays(1);
                                        list[m, n] = dt.ToString();
                                        string date = !string.IsNullOrEmpty(format[m, n]) ? string.Format("{0:" + format[m, n] + "}", dt) : dt.ToString();
                                        owner.CurrentCell.Renderer.ControlText = date;
                                    }
                                    else if (int.TryParse(list[m, n], out res))
                                    {
                                        if(!isCopySeries)
                                            res++;
                                        owner.CurrentCell.Renderer.ControlText = res.ToString();
                                        list[m, n] = res.ToString();
                                    }
                                    else if (double.TryParse(list[m, n], out val))
                                    {
                                        if(!isCopySeries)
                                            val++;
                                        owner.CurrentCell.Renderer.ControlText = val.ToString();
                                        list[m, n] = val.ToString();
                                    }
                                    else if (list[m, n].StartsWith("="))
                                    {
                                        if (isCopySeries)
                                            owner.CurrentCell.Renderer.ControlText = list[m, n];
                                        else
                                        {
                                            string[] formulaVal = list[m, n].Split(formulaSep, StringSplitOptions.RemoveEmptyEntries);
                                            string dep = string.Empty;
                                            for (int d = 0; d < formulaVal.Length; d++)
                                            {
                                                dep = formulaVal[d];
                                                foreach (string lib in libFunc)
                                                    if (dep.Contains(lib))
                                                        dep = dep.Replace(lib, string.Empty);
                                                foreach (string brace in braceSep)
                                                    if (dep.Contains(brace))
                                                        dep = dep.Replace(brace, string.Empty);
                                                formulaVal[d] = dep;
                                            }
                                            int value1;
                                            Hashtable ht = new Hashtable();
                                            for (int r = 0; r < formulaVal.Length; r++)
                                            {
                                                ht.Add("enc" + r, formulaVal[r]);
                                                list[m, n] = list[m, n].Replace(formulaVal[r], "enc" + r);
                                            }

                                            for (int q = 0; q < formulaVal.Length; q++)
                                            {
                                                string s1 = formulaVal[q];
                                                int ch1;
                                                for (ch1 = 0; ch1 < s1.Length; ch1++)
                                                {
                                                    if (int.TryParse(s1[ch1].ToString(), out value1))
                                                        break;
                                                }
                                                string str1 = s1.Substring(0, ch1);
                                                string temp = string.Empty;
                                                formattedVal = s1.Remove(0, ch1);
                                                char[] charac = str1.ToCharArray();
                                                bool c = false;
                                                for (int len = charac.Length - 1; len >= 0; len--)
                                                {
                                                    if (charac[len].ToString().Equals("z"))
                                                    {
                                                        charac.SetValue('a', len);
                                                        temp = "a";
                                                        c = true;
                                                    }
                                                    else if (charac[len].ToString().Equals("Z"))
                                                    {
                                                        charac.SetValue('A', len);
                                                        temp = "A";
                                                        c = true;
                                                    }
                                                    else
                                                    {
                                                        charac.SetValue(((char)(charac[len] + 1)), len);
                                                        c = false;
                                                        break;
                                                    }
                                                }
                                                str1 = string.Empty;
                                                if (c)
                                                    str1 += temp;
                                                for (int len = 0; len < charac.Length; len++)
                                                {
                                                    str1 += charac[len].ToString();
                                                }
                                                ht["enc" + q] = str1 + formattedVal;
                                            }

                                            for (int r = 0; r < formulaVal.Length; r++)
                                            {
                                                list[m, n] = list[m, n].Replace("enc" + r, ht["enc" + r].ToString());
                                            }
                                            owner.CurrentCell.Renderer.ControlText = list[m, n];
                                        }
                                    }
                                    else if (fullMonthCollection.Contains(list[m, n]))
                                    {
                                        if (!isCopySeries)
                                        {
                                            int index = fullMonthCollection.IndexOf(list[m, n]);
                                            if (index == fullMonthCollection.Count - 1)
                                                index = -1;
                                            list[m, n] = fullMonthCollection[index + 1];
                                        }
                                        owner.CurrentCell.Renderer.ControlText = list[m, n];
                                    }
                                    else if (halfMonthCollection.Contains(list[m, n]))
                                    {
                                        if (!isCopySeries)
                                        {
                                            int index = halfMonthCollection.IndexOf(list[m, n]);
                                            if (index == halfMonthCollection.Count - 1)
                                                index = -1;
                                            list[m, n] = halfMonthCollection[index + 1];
                                        }
                                        owner.CurrentCell.Renderer.ControlText = list[m, n];
                                    }
                                    else if (int.TryParse(list[m, n][list[m, n].Length - 1].ToString(), out res) && !isCopySeries)
                                    {
                                        SetNumericEndedCells(ref list[m, n]);
                                        owner.CurrentCell.Renderer.ControlText = list[m, n];
                                    }
                                    else
                                        owner.CurrentCell.Renderer.ControlText = list[m, n];
                                }
                                else
                                {
                                    double x = 0.0;
                                    int y = activeRange.Right - activeRange.Left + 1;
                                    if (y == 1)
                                    {
                                        x = double.Parse(list[m, n]);
                                        if(!isCopySeries)
                                            x += 1;
                                        list[m, n] = x.ToString();
                                        owner.CurrentCell.Renderer.ControlText = list[m, n];
                                    }
                                    else if (y == 2)
                                    {
                                        double val1 = double.Parse(listCpy[m, 1]);
                                        double val2 = double.Parse(listCpy[m, 0]);
                                        if (first || iterateRowIndex != m)
                                        {
                                            prev = double.Parse(listCpy[m, y - 1]);
                                            first = false;
                                            iterateRowIndex = m;
                                        }
                                        x = val1 - val2;
                                        if(!isCopySeries)
                                            prev += x;
                                        list[m, n] = prev.ToString();
                                        owner.CurrentCell.Renderer.ControlText = list[m, n];
                                    }
                                    else if (y > 2)
                                    {
                                        double val1 = double.Parse(listCpy[m, 2]);
                                        double val2 = double.Parse(listCpy[m, 1]);
                                        double val3 = double.Parse(listCpy[m, 0]);
                                        if ((val1 - val2) == (val2 - val3))
                                        {
                                            if (first || iterateRowIndex != m)
                                            {
                                                prev = double.Parse(listCpy[m, y - 1]);
                                                first = false;
                                                iterateRowIndex = m;
                                            }
                                            x = val1 - val2;
                                            if(!isCopySeries)
                                                prev += x;
                                            list[m, n] = prev.ToString();
                                            owner.CurrentCell.Renderer.ControlText = list[m, n];
                                        }
                                        else
                                        {
                                            //code for irregular intervals
                                            if (int.TryParse(list[m, n], out res))
                                            {
                                                if(!isCopySeries)
                                                    res++;
                                                owner.CurrentCell.Renderer.ControlText = res.ToString();
                                                list[m, n] = res.ToString();
                                            }
                                            else if (double.TryParse(list[m, n], out val))
                                            {
                                                if(!isCopySeries)
                                                    val++;
                                                owner.CurrentCell.Renderer.ControlText = val.ToString();
                                                list[m, n] = val.ToString();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //Console.WriteLine("Left-Right");
                }
            }
            else if (activeRange.Left == range.Left && activeRange.Right == range.Right)
            {
                if (activeRange.Top != range.Top)
                {
                    int count = activeRange.Top - range.Top;
                    if (count > 0)
                    {
                        int m, n, l = -1;
                        for (n = 0, j = activeRange.Left; j <= activeRange.Right; j++, n++)
                        {
                            if (n == (activeRange.Right - activeRange.Left + 1))
                                n = 0;
                            for (m = 0, l = activeRange.Bottom - activeRange.Top, i = activeRange.Top - 1; i >= range.Top; i--, l--, m++)
                            {
                                if (m == activeRange.Bottom - activeRange.Top + 1)
                                    m = 0;
                                if (l == -1)
                                    l = activeRange.Bottom - activeRange.Top;
                                owner.CurrentCell.MoveTo(i, j);
                                double x = 0.0;
                                int y = activeRange.Bottom - activeRange.Top + 1;
                                if (y == 1)
                                {
                                    if (!string.IsNullOrEmpty(format[m, n]) && format[m, n].Equals("CheckBox"))
                                    {
                                        if (owner.GetCellRenderer(i, j).StyleInfo.CellValueType == null
                                            || owner.GetCellRenderer(i, j).StyleInfo.CellValueType == typeof(bool)
                                            || owner.GetCellRenderer(i, j).StyleInfo.CellValueType == typeof(string))
                                        {
                                            owner.Model.SetCells(GridRangeInfo.Cell(i, j), styleStore, false, true);
                                        }
                                    }
                                    else if (DateTime.TryParse(list[m, n], out dt) && !isCopySeries)
                                    {
                                        dt = dt.AddDays(1);
                                        list[m, n] = dt.ToString();
                                        string date = !string.IsNullOrEmpty(format[m, n]) ? string.Format("{0:" + format[m, n] + "}", dt) : dt.ToString();
                                        owner.CurrentCell.Renderer.ControlText = date;
                                    }
                                    else if (double.TryParse(list[m, n], out x))
                                    {
                                        if(!isCopySeries)
                                            x -= 1;
                                        list[m, n] = x.ToString();
                                    }
                                    else if (list[m, n].StartsWith("="))
                                    {
                                        if (isCopySeries)
                                            owner.CurrentCell.Renderer.ControlText = list[m, n];
                                        else
                                        {
                                            string[] formulaVal = list[m, n].Split(formulaSep, StringSplitOptions.RemoveEmptyEntries);
                                            string dep = string.Empty;
                                            for (int d = 0; d < formulaVal.Length; d++)
                                            {
                                                dep = formulaVal[d];
                                                foreach (string lib in libFunc)
                                                    if (dep.Contains(lib))
                                                        dep = dep.Replace(lib, string.Empty);
                                                foreach (string brace in braceSep)
                                                    if (dep.Contains(brace))
                                                        dep = dep.Replace(brace, string.Empty);
                                                formulaVal[d] = dep;
                                            }
                                            int value1;
                                            Hashtable ht = new Hashtable();
                                            for (int r = 0; r < formulaVal.Length; r++)
                                            {
                                                ht.Add("enc" + r, formulaVal[r]);
                                                list[m, n] = list[m, n].Replace(formulaVal[r], "enc" + r);
                                            }

                                            for (int q = 0; q < formulaVal.Length; q++)
                                            {
                                                string s1 = formulaVal[q];
                                                int ch1;
                                                for (ch1 = 0; ch1 < s1.Length; ch1++)
                                                {
                                                    if (int.TryParse(s1[ch1].ToString(), out value1))
                                                        break;
                                                }
                                                string str1 = s1.Substring(ch1);
                                                formattedVal = s1.Remove(ch1);
                                                int k1 = int.Parse(str1);
                                                if (k1 > 1)
                                                {
                                                    k1--;
                                                    formattedVal += k1.ToString();
                                                }
                                                else
                                                {
                                                    list[m, n] = "#REF";
                                                    break;
                                                }
                                                ht["enc" + q] = formattedVal;
                                            }

                                            for (int r = 0; r < formulaVal.Length; r++)
                                            {
                                                list[m, n] = list[m, n].Replace("enc" + r, ht["enc" + r].ToString());
                                            }
                                            
                                            owner.CurrentCell.Renderer.ControlText = list[m, n];
                                        }
                                    }
                                    else if (fullMonthCollection.Contains(list[m, n]))
                                    {
                                        if (!isCopySeries)
                                        {
                                            int index = fullMonthCollection.IndexOf(list[m, n]);
                                            if (index == 0)
                                                index = fullMonthCollection.Count;
                                            list[m, n] = fullMonthCollection[index - 1];
                                        }
                                    }
                                    else if (halfMonthCollection.Contains(list[m, n]))
                                    {
                                        if (!isCopySeries)
                                        {
                                            int index = halfMonthCollection.IndexOf(list[m, n]);
                                            if (index == 0)
                                                index = halfMonthCollection.Count;
                                            list[m, n] = halfMonthCollection[index - 1];
                                        }
                                    }
                                    else if (int.TryParse(list[m, n][list[m, n].Length - 1].ToString(), out res) && !isCopySeries)
                                    {
                                        SetNumericEndedCells(ref list[m, n]);
                                    }
                                    owner.CurrentCell.Renderer.ControlText = list[m, n];
                                }
                                else if (y == 2)
                                {
                                    double val1, val2;
                                    DateTime dt1, dt2;
                                    if (DateTime.TryParse(list[0, n], out dt1) && DateTime.TryParse(list[1, n], out dt2) && !isCopySeries)
                                    {
                                        int days = (dt1 - dt2).Days;
                                        dt = dt1.AddDays(days);
                                        list[1, n] = dt1.ToString();
                                        list[0, n] = dt.ToString();
                                        string date = !string.IsNullOrEmpty(format[m, n]) ? string.Format("{0:" + format[m, n] + "}", dt) : dt.ToString();
                                        owner.CurrentCell.Renderer.ControlText = date;
                                    }
                                    else if (double.TryParse(listCpy[1, n], out val1) && double.TryParse(listCpy[0, n], out val2))
                                    {
                                        val1 = double.Parse(listCpy[1, n]);
                                        val2 = double.Parse(listCpy[0, n]);
                                        if (first || iterateRowIndex != n)
                                        {
                                            prev = double.Parse(list[0, n]);
                                            first = false;
                                            iterateRowIndex = n;
                                        }
                                        x = val1 - val2;
                                        if(!isCopySeries)
                                            prev -= x;
                                        list[m, n] = prev.ToString();
                                        owner.CurrentCell.Renderer.ControlText = list[m, n];
                                    }
                                    else if (list[l, n].StartsWith("="))
                                    {
                                        if (!isCopySeries)
                                        {
                                            string[] formulaVal = list[l, n].Split(formulaSep, StringSplitOptions.RemoveEmptyEntries);
                                            string dep = string.Empty;
                                            for (int d = 0; d < formulaVal.Length; d++)
                                            {
                                                dep = formulaVal[d];
                                                foreach (string lib in libFunc)
                                                    if (dep.Contains(lib))
                                                        dep = dep.Replace(lib, string.Empty);
                                                foreach (string brace in braceSep)
                                                    if (dep.Contains(brace))
                                                        dep = dep.Replace(brace, string.Empty);
                                                formulaVal[d] = dep;
                                            }
                                            int value1;
                                            Hashtable ht = new Hashtable();
                                            for (int r = 0; r < formulaVal.Length; r++)
                                            {
                                                ht.Add("enc" + r, formulaVal[r]);
                                                list[l, n] = list[l, n].Replace(formulaVal[r], "enc" + r);
                                            }

                                            for (int q = 0; q < formulaVal.Length; q++)
                                            {
                                                string s1 = formulaVal[q];
                                                int ch1;
                                                for (ch1 = 0; ch1 < s1.Length; ch1++)
                                                {
                                                    if (int.TryParse(s1[ch1].ToString(), out value1))
                                                        break;
                                                }
                                                string str1 = s1.Substring(ch1);
                                                formattedVal = s1.Remove(ch1, s1.Length - 1);
                                                if (!char.IsLetter(formattedVal, 0))
                                                    formattedVal = string.Empty;
                                                int k1 = int.Parse(str1);
                                                if (k1 > 1)
                                                {
                                                    k1 -= y;
                                                    formattedVal += k1.ToString();
                                                }
                                                else
                                                {
                                                    list[l, n] = "#REF";
                                                    break;
                                                }
                                                ht["enc" + q] = formattedVal;
                                            }

                                            for (int r = 0; r < formulaVal.Length; r++)
                                            {
                                                list[l, n] = list[l, n].Replace("enc" + r, ht["enc" + r].ToString());
                                            }
                                        }
                                        owner.CurrentCell.Renderer.ControlText = list[l, n];
                                    }
                                    else if (fullMonthCollection.Contains(list[l, n]))
                                    {
                                        if (!isCopySeries)
                                        {
                                            int index = fullMonthCollection.IndexOf(list[l, n]);
                                            if (index == 0)
                                                index = fullMonthCollection.Count;
                                            list[l, n] = fullMonthCollection[index - 1];
                                        }
                                        owner.CurrentCell.Renderer.ControlText = list[l, n];
                                    }
                                    else if (halfMonthCollection.Contains(list[l, n]))
                                    {
                                        if (!isCopySeries)
                                        {
                                            int index = halfMonthCollection.IndexOf(list[l, n]);
                                            if (index == 0)
                                                index = halfMonthCollection.Count;
                                            list[l, n] = halfMonthCollection[index - 1];
                                        }
                                        owner.CurrentCell.Renderer.ControlText = list[l, n];
                                    }
                                    else
                                    {
                                        //code for irregular intervals
                                        if (int.TryParse(list[l, n], out res))
                                        {
                                            if(!isCopySeries)
                                                res--;
                                            owner.CurrentCell.Renderer.ControlText = res.ToString();
                                            list[l, n] = res.ToString();
                                        }
                                        else if (double.TryParse(list[l, n], out val))
                                        {
                                            if(!isCopySeries)
                                                val--;
                                            owner.CurrentCell.Renderer.ControlText = val.ToString();
                                            list[l, n] = val.ToString();
                                        }
                                        else
                                            owner.CurrentCell.Renderer.ControlText = list[l, n];
                                    }
                                }
                                else if (y > 2)
                                {
                                    double val1, val2, val3;
                                    DateTime dt1, dt2;
                                    int interval = -1;
                                    for (int index = y; index > 0; index -= 2)
                                    {
                                        if (index == y && interval == -1)
                                        {
                                            if (DateTime.TryParse(list[index - 1, n], out dt1) && DateTime.TryParse(list[index - 2, n], out dt2))
                                            {
                                                interval = (dt2 - dt1).Days;
                                            }
                                        }
                                        else
                                        {
                                            if (DateTime.TryParse(list[index, n], out dt1) && DateTime.TryParse(list[index - 1, n], out dt2)
                                                && interval == (dt2 - dt1).Days)
                                            {
                                                continue;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (interval != -1)
                                    {
                                        if (DateTime.TryParse(list[0, n], out dt1) && DateTime.TryParse(list[1, n], out dt2) && !isCopySeries)
                                        {
                                            dt = dt1.AddDays(interval);
                                            list[0, n] = dt.ToString();
                                            list[1, n] = dt1.ToString();
                                            string date = !string.IsNullOrEmpty(format[m, n]) ? string.Format("{0:" + format[m, n] + "}", dt) : dt.ToString();
                                            owner.CurrentCell.Renderer.ControlText = date;
                                        }
                                    }
                                    else if (double.TryParse(listCpy[2, n], out val1) && double.TryParse(listCpy[1, n], out val2) && double.TryParse(listCpy[0, n], out val3))
                                    {
                                        val1 = double.Parse(listCpy[2, n]);
                                        val2 = double.Parse(listCpy[1, n]);
                                        val3 = double.Parse(listCpy[0, n]);
                                        if ((val1 - val2) == (val2 - val3))
                                        {
                                            if (first || iterateRowIndex != n)
                                            {
                                                prev = double.Parse(listCpy[0, n]);
                                                first = false;
                                                iterateRowIndex = n;
                                            }
                                            x = val1 - val2;
                                            if(!isCopySeries)
                                                prev -= x;
                                            list[m, n] = prev.ToString();
                                            owner.CurrentCell.Renderer.ControlText = list[m, n];
                                        }
                                    }
                                    else if (listCpy[l, n].StartsWith("="))
                                    {
                                        if (!isCopySeries)
                                        {
                                            string[] formulaVal = listCpy[l, n].Split(formulaSep, StringSplitOptions.RemoveEmptyEntries);
                                            string cpy = listCpy[l, n];
                                            string dep = string.Empty;
                                            for (int d = 0; d < formulaVal.Length; d++)
                                            {
                                                dep = formulaVal[d];
                                                foreach (string lib in libFunc)
                                                    if (dep.Contains(lib))
                                                        dep = dep.Replace(lib, string.Empty);
                                                foreach (string brace in braceSep)
                                                    if (dep.Contains(brace))
                                                        dep = dep.Replace(brace, string.Empty);
                                                formulaVal[d] = dep;
                                            }
                                            int value1;
                                            Hashtable ht = new Hashtable();
                                            for (int r = 0; r < formulaVal.Length; r++)
                                            {
                                                ht.Add("enc" + r, formulaVal[r]);
                                                cpy = cpy.Replace(formulaVal[r], "enc" + r);
                                            }

                                            for (int q = 0; q < formulaVal.Length; q++)
                                            {
                                                string s1 = formulaVal[q];
                                                int ch1;
                                                for (ch1 = 0; ch1 < s1.Length; ch1++)
                                                {
                                                    if (int.TryParse(s1[ch1].ToString(), out value1))
                                                        break;
                                                }
                                                string str1 = s1.Substring(ch1);
                                                formattedVal = s1.Remove(ch1, s1.Length - 1);
                                                int k1 = int.Parse(str1);
                                                if (k1 > 1)
                                                {
                                                    k1 -= y;
                                                    formattedVal += k1.ToString();
                                                }
                                                else
                                                {
                                                    list[l, n] = "#REF";
                                                    break;
                                                }
                                                ht["enc" + q] = formattedVal;
                                            }

                                            for (int r = 0; r < formulaVal.Length; r++)
                                            {
                                                cpy = cpy.Replace("enc" + r, ht["enc" + r].ToString());
                                            }
                                            list[l, n] = cpy;
                                        }
                                        owner.CurrentCell.Renderer.ControlText = list[l, n];
                                    }
                                    else
                                    {
                                        //code for irregular intervals
                                        if (first || iterateRowIndex != n)
                                        {
                                            first = false;
                                            iterateRowIndex = n;
                                        }
                                        if (double.TryParse(listCpy[l, n], out prev))
                                        {
                                            if(!isCopySeries)
                                                prev -= 1;
                                            list[m, n] = prev.ToString();
                                        }
                                        else
                                            list[m, n] = listCpy[l, n];
                                        owner.CurrentCell.Renderer.ControlText = list[m, n];
                                    }
                                }
                                else
                                {
                                    //code for irregular intervals
                                    if (first || iterateRowIndex != n)
                                    {
                                        first = false;
                                        iterateRowIndex = n;
                                    }
                                    if (double.TryParse(listCpy[l, n], out prev))
                                    {
                                        if(!isCopySeries)
                                            prev -= 1;
                                        list[m, n] = prev.ToString();
                                    }
                                    else
                                        list[m, n] = listCpy[l, n];
                                    owner.CurrentCell.Renderer.ControlText = list[m, n];
                                }
                            }
                        }
                    }
                    //Console.WriteLine("Bottom-Top");
                }
                else
                {

                    int count = range.Bottom - activeRange.Bottom;

                    if (count > 0)
                    {
                        int m, n = -1,l = -1;
                        for (n = 0, j = activeRange.Left; j <= activeRange.Right; j++, n++)
                        {
                            if (n == (activeRange.Right - activeRange.Left + 1))
                                n = 0;
                            referCollection.Clear();

                            for (m = 0, l = 0, i = activeRange.Bottom + 1; i <= range.Bottom; i++, l++, m++)
                            {
                                if (m == (activeRange.Bottom - activeRange.Top + 1))
                                    m = 0;
                                if (l == -1)
                                    l = 0;

                                owner.CurrentCell.MoveTo(i, j);
                                for (int k = 0; k < (activeRange.Right - activeRange.Left + 1); k++)
                                    if (!int.TryParse(list[m, k], out res) && !double.TryParse(list[m, k], out val)
                                        && !DateTime.TryParse(list[m, n], out dt))
                                        p++;
                                if (p > 0)
                                {
                                    p = 0;
                                    if (!string.IsNullOrEmpty(format[m, n]) && format[m, n].Equals("CheckBox"))
                                    {
                                        if (owner.GetCellRenderer(i, j).StyleInfo.CellValueType == null
                                            || owner.GetCellRenderer(i, j).StyleInfo.CellValueType == typeof(bool)
                                            || owner.GetCellRenderer(i, j).StyleInfo.CellValueType == typeof(string))
                                        {
                                            owner.Model.SetCells(GridRangeInfo.Cell(i, j), styleStore, false, true);
                                        }
                                    }
                                    else if (int.TryParse(list[m, n], out res))
                                    {
                                        if(!isCopySeries)
                                            res++;
                                        owner.CurrentCell.Renderer.ControlText = res.ToString();
                                        list[m, n] = res.ToString();
                                    }
                                    else if (double.TryParse(list[m, n], out val))
                                    {
                                        if(!isCopySeries)
                                            val++;
                                        owner.CurrentCell.Renderer.ControlText = val.ToString();
                                        list[m, n] = val.ToString();
                                    }
                                    else if (list[m, n].StartsWith("="))
                                    {
                                        if (!isCopySeries)
                                        {
                                            string[] formulaVal = list[m, n].Split(formulaSep, StringSplitOptions.RemoveEmptyEntries);
                                            string dep = string.Empty;
                                            for (int d = 0; d < formulaVal.Length; d++)
                                            {
                                                dep = formulaVal[d];
                                                foreach (string lib in libFunc)
                                                    if (dep.Contains(lib))
                                                        dep = dep.Replace(lib, string.Empty);
                                                foreach (string brace in braceSep)
                                                    if (dep.Contains(brace))
                                                        dep = dep.Replace(brace, string.Empty);
                                                formulaVal[d] = dep;
                                            }
                                            int value1;
                                            Hashtable ht = new Hashtable();
                                            for (int r = 0; r < formulaVal.Length; r++)
                                            {
                                                ht.Add("enc" + r, formulaVal[r]);
                                                list[m, n] = list[m, n].Replace(formulaVal[r], "enc" + r);
                                            }

                                            for (int q = 0; q < formulaVal.Length; q++)
                                            {
                                                string s1 = formulaVal[q];
                                                int ch1;
                                                for (ch1 = 0; ch1 < s1.Length; ch1++)
                                                {
                                                    if (int.TryParse(s1[ch1].ToString(), out value1))
                                                        break;
                                                }
                                                string str1 = s1.Substring(ch1);
                                                formattedVal = s1.Remove(ch1);
                                                int k1 = int.Parse(str1);
                                                k1++;
                                                formattedVal += k1.ToString();
                                                ht["enc" + q] = formattedVal;
                                            }

                                            for (int r = 0; r < formulaVal.Length; r++)
                                            {
                                                list[m, n] = list[m, n].Replace("enc" + r, ht["enc" + r].ToString());
                                            }
                                        }
                                        owner.CurrentCell.Renderer.ControlText = list[m, n];
                                    }
                                    else if (fullMonthCollection.Contains(list[m, n]))
                                    {
                                        int index = fullMonthCollection.IndexOf(list[m, n]);
                                        string month = fullMonthCollection[index];
                                        if (month.Equals(list[m, n], StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            if (!isCopySeries)
                                            {
                                                if (index == fullMonthCollection.Count - 1)
                                                    index = -1;
                                                list[m, n] = fullMonthCollection[index + 1];
                                            }
                                            owner.CurrentCell.Renderer.ControlText = list[m, n];
                                        }
                                    }
                                    else if (halfMonthCollection.Contains(list[m, n]))
                                    {
                                        if (!isCopySeries)
                                        {
                                            int index = halfMonthCollection.IndexOf(list[m, n]);
                                            if (index == halfMonthCollection.Count - 1)
                                                index = -1;
                                            list[m, n] = halfMonthCollection[index + 1];
                                        }
                                        owner.CurrentCell.Renderer.ControlText = list[m, n];
                                    }
                                    else if (int.TryParse(list[m, n][list[m, n].Length - 1].ToString(), out res) && !isCopySeries)
                                    {
                                        SetNumericEndedCells(ref list[m, n]);
                                        owner.CurrentCell.Renderer.ControlText = list[m, n];
                                    }
                                    else
                                        owner.CurrentCell.Renderer.ControlText = list[m, n];
                                }
                                else
                                {
                                    double x = 0.0;
                                    int y = activeRange.Bottom - activeRange.Top + 1;
                                    if (y == 1)
                                    {
                                        if (DateTime.TryParse(list[m, n], out dt) && !isCopySeries)
                                        {
                                            dt = dt.AddDays(1);
                                            list[m, n] = dt.ToString();
                                            string date = !string.IsNullOrEmpty(format[m, n]) ? string.Format("{0:" + format[m, n] + "}", dt) : dt.ToString();
                                            owner.CurrentCell.Renderer.ControlText = date;
                                        }
                                        else
                                        {
                                            if (double.TryParse(list[m, n], out x))
                                            {
                                                if (!isCopySeries)
                                                    x += 1;
                                                list[m, n] = x.ToString();
                                            }
                                            owner.CurrentCell.Renderer.ControlText = list[m, n];
                                        }
                                    }
                                    else if (y == 2)
                                    {
                                        double val1, val2;
                                        DateTime dt1, dt2;
                                        if (DateTime.TryParse(list[1, n], out dt1) && DateTime.TryParse(list[0, n], out dt2) && !isCopySeries)
                                        {
                                            int days = (dt1 - dt2).Days;
                                            dt = dt1.AddDays(days);
                                            list[0, n] = dt1.ToString();
                                            list[1, n] = dt.ToString();
                                            string date = !string.IsNullOrEmpty(format[m, n]) ? string.Format("{0:" + format[m, n] + "}", dt) : dt.ToString();
                                            owner.CurrentCell.Renderer.ControlText = date;
                                        }
                                        else if (double.TryParse(listCpy[1, n], out val1) && double.TryParse(listCpy[0, n], out val2))
                                        {
                                            val1 = double.Parse(listCpy[1, n]);
                                            val2 = double.Parse(listCpy[0, n]);
                                            if (first || iterateRowIndex != n)
                                            {
                                                prev = double.Parse(list[0, n]);
                                                first = false;
                                                iterateRowIndex = n;
                                            }
                                            x = val1 - val2;
                                            if (!isCopySeries)
                                                prev -= x;
                                            list[m, n] = prev.ToString();
                                            owner.CurrentCell.Renderer.ControlText = list[m, n];
                                        }
                                        else if (list[l, n].StartsWith("="))
                                        {
                                            if (!isCopySeries)
                                            {
                                                string[] formulaVal = list[l, n].Split(formulaSep, StringSplitOptions.RemoveEmptyEntries);
                                                string dep = string.Empty;
                                                for (int d = 0; d < formulaVal.Length; d++)
                                                {
                                                    dep = formulaVal[d];
                                                    foreach (string lib in libFunc)
                                                        if (dep.Contains(lib))
                                                            dep = dep.Replace(lib, string.Empty);
                                                    foreach (string brace in braceSep)
                                                        if (dep.Contains(brace))
                                                            dep = dep.Replace(brace, string.Empty);
                                                    formulaVal[d] = dep;
                                                }
                                                int value1;
                                                Hashtable ht = new Hashtable();
                                                for (int r = 0; r < formulaVal.Length; r++)
                                                {
                                                    ht.Add("enc" + r, formulaVal[r]);
                                                    list[l, n] = list[l, n].Replace(formulaVal[r], "enc" + r);
                                                }

                                                for (int q = 0; q < formulaVal.Length; q++)
                                                {
                                                    string s1 = formulaVal[q];
                                                    int ch1;
                                                    for (ch1 = 0; ch1 < s1.Length; ch1++)
                                                    {
                                                        if (int.TryParse(s1[ch1].ToString(), out value1))
                                                            break;
                                                    }
                                                    string str1 = s1.Substring(ch1);
                                                    formattedVal = s1.Remove(ch1);
                                                    int k1 = int.Parse(str1);
                                                    k1 += y;
                                                    formattedVal += k1.ToString();
                                                    ht["enc" + q] = formattedVal;
                                                }

                                                for (int r = 0; r < formulaVal.Length; r++)
                                                {
                                                    list[l, n] = list[l, n].Replace("enc" + r, ht["enc" + r].ToString());
                                                }
                                            }
                                            owner.CurrentCell.Renderer.ControlText = list[l, n];
                                        }
                                        else
                                        {
                                            //code for irregular intervals
                                            if (int.TryParse(list[m, n], out res))
                                            {
                                                if (!isCopySeries)
                                                    res++;
                                                owner.CurrentCell.Renderer.ControlText = res.ToString();
                                                list[m, n] = res.ToString();
                                            }
                                            else if (double.TryParse(list[m, n], out val))
                                            {
                                                if (!isCopySeries)
                                                    val++;
                                                owner.CurrentCell.Renderer.ControlText = val.ToString();
                                                list[m, n] = val.ToString();
                                            }
                                            else
                                                owner.CurrentCell.Renderer.ControlText = list[m, n];
                                        }
                                    }
                                    else if (y > 2)
                                    {
                                        DateTime dt1,dt2;
                                        int interval =-1;
                                        for (int index = y; index > 0; index -= 2)
                                        {
                                            if (index == y && interval == -1)
                                            {
                                                if (DateTime.TryParse(list[index - 1, n], out dt1) && DateTime.TryParse(list[index - 2, n], out dt2))
                                                {
                                                    interval = (dt1 - dt2).Days;
                                                }
                                            }
                                            else
                                            {
                                                if (DateTime.TryParse(list[index, n], out dt1) && DateTime.TryParse(list[index - 1, n], out dt2)
                                                    && interval == (dt1 - dt2).Days)
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                        if (interval != -1)
                                        {
                                            if (DateTime.TryParse(list[y - 1, n], out dt1) && DateTime.TryParse(list[y - 2, n], out dt2) && !isCopySeries)
                                            {
                                                dt = dt1.AddDays(interval);
                                                list[y - 1, n] = dt.ToString();
                                                list[y - 2, n] = dt1.ToString();
                                                string date = !string.IsNullOrEmpty(format[m, n]) ? string.Format("{0:" + format[m, n] + "}", dt) : dt.ToString();
                                                owner.CurrentCell.Renderer.ControlText = date;
                                            }
                                        }
                                        //code for irregular intervals
                                        else if (int.TryParse(list[m, n], out res))
                                        {
                                            if (!isCopySeries)
                                                res++;
                                            owner.CurrentCell.Renderer.ControlText = res.ToString();
                                            list[m, n] = res.ToString();
                                        }
                                        else if (double.TryParse(list[m, n], out val))
                                        {
                                            if (!isCopySeries)
                                                val++;
                                            owner.CurrentCell.Renderer.ControlText = val.ToString();
                                            list[m, n] = val.ToString();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //Console.WriteLine("Top-Bottom");
                }
            }
        }

        /// <summary>
        /// Gets the name of the mouse controller.
        /// </summary>
        public string Name
        {
            get { return "ExcelMarker"; }
        }

        #endregion
    }
}
