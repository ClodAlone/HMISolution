#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Grid.Olap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Syncfusion.Olap.Engine;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Controls.Grid;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Olap.Data;

    /// <summary>
    /// GridCell tooltip control
    /// </summary>
    internal class PopupValueCellToolTip : Popup
    {
        #region Private Variables

        private PivotValueCellData _CellData;

        private PivotEngine _Engine;

        #endregion

        #region Constructor Initilize /Finalize

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupValueCellToolTip"/> class.
        /// </summary>
        public PopupValueCellToolTip()
        {
            this.AllowsTransparency = true;
            this.Placement = PlacementMode.Relative;
            this.PlacementRectangle = new Rect(1, 1, 1, 1);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Value cell data, these values will be reflected in the tooltip
        /// </summary>
        public PivotValueCellData CellData
        {
            get
            {
                return _CellData;
            }
            set
            {
                _CellData = value;
                UpdatePopupVisualValueCellData();

            }
        }

        /// <summary>
        /// Gets or sets the pivot engine, Engine data will be displayed as tooltip in the
        /// header cells
        /// </summary>
        public PivotEngine Engine
        {
            get
            {
                return _Engine;
            }
            set
            {
                _Engine = value;
                UpdatePopupVisialEngineData();

            }
        }

        /// <summary>
        /// Header cells tooltip content control
        /// </summary>
        public OlapGridBase GridControl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets value indicating whether this instance is visible
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance ; otherwise, <see langword="false" />.
        /// </value>
        protected bool IsShowing
        {
            get;
            set;
        }

        /// <summary>
        /// Value cell tooltip content border
        /// </summary>
        public Border PopupContent
        {
            get;
            set;
        }

        /// <summary>
        /// Header cell tooltip content border
        /// </summary>
        public Border PopupHeaderContent
        {
            get;
            set;
        }


        /// <summary>
        /// Value cell tooltip content
        /// </summary>
        public TextBlock Text
        {
            get;
            set;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Engine data will be rendered in the control and displays as a tooltip
        /// </summary>
        internal UIElement GetPopupEngineContent()
        {
            //this.PopupHeaderContent = null;
            //if (this.GridControl == null)
            {
                this.GridControl = new OlapGridBase
                {
                    //Margin = new Thickness(5d),
                    ColumnHeaderStyle = new OlapGridCellStyle
                    {
                        Background = Brushes.Transparent,
                    },
                    RowHeaderStyle = new OlapGridCellStyle
                    {
                        Background = Brushes.Transparent
                    },
                    ValueCellsStyle = new OlapGridCellStyle
                    {
                        Background = Brushes.Transparent
                    }
                };
                this.GridControl.Model = new Syncfusion.Windows.Grid.Olap.Control.OlapGridModel(this.GridControl);
            }
            //if (this.PopupHeaderContent == null)
            {
                this.PopupHeaderContent = new Border
                {
                    Margin = new Thickness(5d),
                    BorderThickness = new Thickness(1d),
                    BorderBrush = Brushes.Black,
                    Child = this.GridControl,
                    Background = Brushes.LightYellow
                };
            }
            this.PopupHeaderContent.InvalidateVisual();
            return this.PopupHeaderContent;
        }

        /// <summary>
        /// Value cell data will be rendered as a control and displayed in the value cell
        /// tooltips
        /// </summary>
        internal UIElement GetPopupValueCellContent()
        {
            if (this.Text == null)
            {
                this.Text = new TextBlock
                {
                    Margin = new Thickness(5d),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };
            }
            if (this.PopupContent == null)
            {
                this.PopupContent = new Border
                {
                    BorderThickness = new Thickness(1d),
                    BorderBrush = Brushes.Black,
                    Child = this.Text,
                    Background = Brushes.LightYellow
                };
            }
            return this.PopupContent;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Hides the tooltip if visible
        /// </summary>
        public void Hide()
        {
            this.IsShowing = false;
            this.IsOpen = false;
            this.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Displays and moves the tooltip to the specified position
        /// </summary>
        /// <param name="p">Tooltip position</param>
        public void Move(Point p)
        {
            this.Show();
            this.PlacementRectangle = new Rect(p.X + 3d, p.Y + 3d, this.Width, this.Height); // 3d is padding to ignore the mouse override in popup
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Displayes the tooltip
        /// </summary>
        private void Show()
        {
            if (!this.IsShowing)
            {
                this.IsShowing = true;
                this.IsOpen = true;
                this.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Updated the popup engine visual
        /// </summary>
        private void UpdatePopupVisialEngineData()
        {
            if (this.Engine != null)
            {
                this.Child = GetPopupEngineContent();
                this.GridControl.Engine = this.Engine;
                this.GridControl.InvalidateCells();
            }
            else
            {
                this.Hide();
            }
        }

        /// <summary>
        /// Updates the popup value cell tooltip visual
        /// </summary>
        private void UpdatePopupVisualValueCellData()
        {
            if (CellData != null)
            {
                this.Child = GetPopupValueCellContent();
#if Debug
                Console.WriteLine("Visual updated");
#endif
                this.Text.Inlines.Clear();
                this.Text.Inlines.Add("Measure: " + CellData.Measure + "\n");
                string data = string.Empty;
                for (int i = 0; i < CellData.Columns.Count; i++)
                {
                    if (i != 0)
                        data += " - ";
                    data += CellData.Columns[i];
                }
                this.Text.Inlines.Add("Columns: " + data + "\n");
                data = string.Empty;
                for (int i = 0; i < CellData.Rows.Count; i++)
                {
                    if (i != 0)
                        data += " - ";
                    data += CellData.Rows[i];
                }
                this.Text.Inlines.Add("Rows: " + data + "\n");
                this.Text.Inlines.Add("Value: " + CellData.Value);
#if Debug
                foreach (System.Windows.Documents.Run item in this.Text.Inlines)
                {
                    Console.WriteLine(item.Text);
                }
#endif
                this.Text.InvalidateVisual();
            }
            else
            {
                this.Hide();
            }
        }

        #endregion

    }

    /// <summary>
    /// GridCell Mouse HOver controller
    /// </summary>
    internal class GridCellHOverController : IMouseController
    {
        #region Private Variables

        private System.Windows.Threading.DispatcherTimer dispatchTimer;
        private PopupValueCellToolTip popupValueCellToolTip;
        private Point prevPoint = new Point();
        private RowColumnIndex prevRowColIndex = RowColumnIndex.Empty;

        #endregion

        #region Public Variables

        /// <summary>
        /// Controller name
        /// </summary>
        public const string GridCellHOverControllerName = "GridCellHOverController";

        #endregion

        #region Initilize/Finalize

        /// <summary>
        /// Initializes a new instance of the <see cref="GridCellHOverController"/> class.
        /// </summary>
        /// <param name="grid">The grid.</param>
        public GridCellHOverController(OlapGridBase grid)
        {
            this.Grid = grid;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the cursor to be displayed.
        /// </summary>
        /// <value></value>
        public System.Windows.Input.Cursor Cursor
        {
            get
            {
                return this.Grid.RaiseGridCellCursor();
            }
        }

        /// <summary>
        /// Gets or sets the grid.
        /// </summary>
        /// <value>The grid.</value>
        public OlapGridBase Grid
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the hit test info.
        /// </summary>
        /// <value>The hit test info.</value>
        internal GridCellHitTestInfo HitTestInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this controller supports the cancel mouse capture feature and the context of the mouse operation can be changed
        /// while the user drags the pressed mouse.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if supports the cancel mouse capture feature; otherwise, <c>false</c>.
        /// </value>
        bool IMouseController.SupportsCancelMouseCapture
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the controller supports the mouse
        /// tracking feature which allows MouseControllerDispatcher to emulate
        /// a pressed mouse operation similar
        /// to the way a combobox selects the item in a dropped list box while
        /// hovering the mouse over the dropped listbox and simulating
        /// a MouseUp when the user presses the mouse.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the controller supports mouse tracking; otherwise, <c>false</c>.
        /// </value>
        bool IMouseController.SupportsMouseTracking
        {
            get { return false; }
        }

        /// <summary>
        /// Returns the name of this mouse controller.
        /// </summary>
        /// <value></value>
        public string Name
        {
            get { return GridCellHOverController.GridCellHOverControllerName; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// CancelMode is called for the active controller after a MouseDown message when the mouse operation is cancelled.
        /// </summary>
        public void CancelMode()
        {
        }

        /// <summary>
        /// HitTest is called to determine whether your controller wants to handle the mouse events based current context.
        /// </summary>
        /// <param name="mouseEventArgs">The <see cref="MouseControllerEventArgs"/> instance containing the event data.</param>
        /// <param name="controller">The controller.</param>
        /// <returns>
        /// A value not equal to 0 indicates that the controller wants to handle the mouse input; otherwise if equal to 0 the controller is not handling it.
        /// </returns>
        /// <remarks>
        /// The current winner of the vote is specified through the controller parameter. Your implementation of HitTest
        /// can decide if it wants to override the existing vote or leave it.
        /// </remarks>
        public int HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller)
        {
            var point = mouseEventArgs.Location;
            this.HitTestInfo = null;
            var rowColIndex = this.Grid.PointToCellRowColumnIndex(point);
            if (this.Grid.Engine != null)
            {
                OlapGridCellStyleInfoIdentity cellIdentity = this.Grid.Model[rowColIndex.RowIndex, rowColIndex.ColumnIndex].Tag as OlapGridCellStyleInfoIdentity;
                if (cellIdentity != null)
                {
                    PivotCellDescriptor cellDescriptor = cellIdentity.CellDescriptor; 
                    if (cellDescriptor != null)
                    {
                        if (cellDescriptor.CellType == Syncfusion.Olap.Engine.PivotCellDescriptorType.ColumnHeader ||
                            cellDescriptor.CellType == Syncfusion.Olap.Engine.PivotCellDescriptorType.RowHeader)
                        {
                            Member m_member = cellDescriptor.Tag as Member;
                            if (m_member != null)
                            {
                                if (m_member.UniqueName.Contains("Measure"))
                                {
                                    return 0;
                                }
                            }

                            if (this.Grid.ShowHeaderCellsToolTip)
                            {
                                this.HitTestInfo = new GridCellHitTestInfo(this.Grid, mouseEventArgs.Location);
                            }
                            else
                            {
                                return 0;
                            }
                        }
                        else if (cellDescriptor.CellType == Syncfusion.Olap.Engine.PivotCellDescriptorType.Value)
                        {
                            if (!(cellDescriptor.CellExTypes.Contains("SummaryRow") ||
                                cellDescriptor.CellExTypes.Contains("SummaryColumn")))
                            {
                                if (cellIdentity.IsHyperlinkCell)
                                {
                                    return 0;
                                }
                                if (this.Grid.ShowValueCellsToolTip)
                                {
                                    this.HitTestInfo = new GridCellHitTestInfo(this.Grid, mouseEventArgs.Location);
                                }
                                else
                                {
                                    return 0;
                                }
                            }
                        }
                    }
                }
            }
            if (this.HitTestInfo != null)
            {
                return 1;
            }
            if (this.popupValueCellToolTip != null && this.popupValueCellToolTip.IsOpen)
            {
                this.popupValueCellToolTip.Hide();
            }
            return 0;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles the Tick event of the dispatchTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void dispatchTimer_Tick(object sender, EventArgs e)
        {
            System.Windows.Threading.DispatcherTimer dispatchTimer = sender as System.Windows.Threading.DispatcherTimer;
            if (dispatchTimer != null)
            {
                Point point = (Point)dispatchTimer.Tag;
                if (point == prevPoint)
                {
                    this.popupValueCellToolTip.Move(point);
                }
                dispatchTimer.Stop();
                dispatchTimer = null;
            }
        }

        /// <summary>
        /// Gets the value cell data.
        /// </summary>
        /// <returns></returns>
        private PivotValueCellData GetValueCellData()
        {
            if (this.HitTestInfo != null)
            {
                PivotCellDescriptor cellDescriptor = this.Grid.Engine[this.HitTestInfo.RowColIndex.RowIndex, this.HitTestInfo.RowColIndex.ColumnIndex];
                if (cellDescriptor.CellType == Syncfusion.Olap.Engine.PivotCellDescriptorType.Value &&
                    !(cellDescriptor.CellExTypes.Contains("SummaryRow") && cellDescriptor.CellExTypes.Contains("SummaryColumn"))
                    )
                    return this.Grid.Engine.GetCellData(cellDescriptor);
            }
            return null;
        }

        /// <summary>
        /// Hits the test.
        /// </summary>
        /// <param name="colIndex">Index of the col.</param>
        /// <returns></returns>
        private VisibleLineInfo HitTest(int colIndex)
        {
            return this.Grid.ScrollColumns.GetVisibleLineAtLineIndex(colIndex);
        }

        /// <summary>
        /// MouseDown is called when this controller signaled in HitTest that it wants to handle mouse events and the
        /// user pressed the mouse button.
        /// </summary>
        /// <param name="e">The <see cref="MouseControllerEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// MouseDown is called and this controller will become the active controller and receive all subsequent mouse messages
        /// until the mouse button is released or the mouse operation is cancelled.
        /// </remarks>
        void IMouseController.MouseDown(MouseControllerEventArgs e)
        {
            if (this.HitTestInfo != null)
            {
                if (e.Button.Value == System.Windows.Input.MouseButton.Left)
                {
                    this.Grid.RaiseGridCellClick(this.HitTestInfo.RowColIndex.RowIndex, this.HitTestInfo.RowColIndex.ColumnIndex);
                }
            }
        }

        /// <summary>
        /// MouseHover is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHover
        /// is called after MouseHoverEnter.
        /// </summary>
        /// <param name="e">The <see cref="MouseControllerEventArgs"/> instance containing the event data.</param>
        void IMouseController.MouseHover(MouseControllerEventArgs e)
        {
            var rowColIndex = this.Grid.PointToCellRowColumnIndex(e.Location);
            popupValueCellToolTip.Hide();
            if (prevRowColIndex != rowColIndex)
            {
                prevRowColIndex = rowColIndex;
#if Debug
                Console.WriteLine("Value CellData updated");
                Console.WriteLine(this.Grid.PointToScreen(e.Location).ToString());
#endif
                PivotCellDescriptor cellDescriptor = null;
                if (this.HitTestInfo != null)
                {
                    cellDescriptor = this.Grid.Engine[this.HitTestInfo.RowColIndex.RowIndex, this.HitTestInfo.RowColIndex.ColumnIndex];
                    if (cellDescriptor.CellType == Syncfusion.Olap.Engine.PivotCellDescriptorType.Value
                        && this.Grid.ShowValueCellsToolTip)
                    {
                        PivotValueCellData pivotValueCellData = this.Grid.Engine.GetCellData(cellDescriptor);
                        this.popupValueCellToolTip.CellData = pivotValueCellData;
                    }
                    else if ((cellDescriptor.CellType == Syncfusion.Olap.Engine.PivotCellDescriptorType.ColumnHeader ||
                        cellDescriptor.CellType == Syncfusion.Olap.Engine.PivotCellDescriptorType.RowHeader)
                        && this.Grid.ShowHeaderCellsToolTip)
                    {
                        this.popupValueCellToolTip.Engine = this.Grid.DataManager.GetExpandedRows(cellDescriptor);
                    }
                    else
                    {
                        this.popupValueCellToolTip.CellData = null;
                    }
                }
                else
                {
                    this.popupValueCellToolTip.CellData = null;
                }
            }
            var point = this.Grid.PointToScreen(e.Location);
            dispatchTimer =
                new System.Windows.Threading.DispatcherTimer();
            if (point != prevPoint)
            {
                dispatchTimer.Tick += new EventHandler(dispatchTimer_Tick);
                dispatchTimer.Interval = new TimeSpan(0, 0, 2);
                dispatchTimer.Tag = point;
                dispatchTimer.Start();
#if Debug
                Console.WriteLine("Mouse Moving");
#endif
            }
            else
            {
#if Debug
                Console.WriteLine("Mouse Moving Stoped");
#endif
            }
            prevPoint = point;

        }

        /// <summary>
        /// MouseHoverEnter is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHoverEnter
        /// is called before the MouseHover is called for the first time.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void IMouseController.MouseHoverEnter(System.Windows.Input.MouseEventArgs e)
        {
            if (popupValueCellToolTip == null)
            {
                this.popupValueCellToolTip = new PopupValueCellToolTip();
            }
#if Debug
            Console.WriteLine("Enter Called");
#endif

        }

        /// <summary>
        /// MouseHoverLeave is called when hovering ends either because user dragged mouse out of the hit-test area or
        /// when context changes (e.g. user pressed the mouse button).
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void IMouseController.MouseHoverLeave(System.Windows.Input.MouseEventArgs e)
        {
            if (this.popupValueCellToolTip != null)
            {
                this.popupValueCellToolTip.Hide();
            }
            if (this.dispatchTimer.IsEnabled)
            {
                this.dispatchTimer.Stop();
            }
            this.prevPoint = new Point();
#if Debug
            Console.WriteLine("Leave Called");
#endif
        }

        void IMouseController.MouseMove(MouseControllerEventArgs e)
        {
#if Debug
            Console.WriteLine("Mouse Move called");
#endif
        }

        /// <summary>
        /// MouseUp is called for the active controller after a MouseDown message when the user releases the mouse button.
        /// </summary>
        /// <param name="e">The <see cref="MouseControllerEventArgs"/> instance containing the event data.</param>
        void IMouseController.MouseUp(MouseControllerEventArgs e)
        {
        }

        /// <summary>
        /// RestoreMode is called when a controller should be reactivated. Prevoius state can be restored if it was backed up earlier when CancelMode was called.
        /// </summary>
        void IMouseController.RestoreMode()
        {
        }

        #endregion

        internal sealed class GridCellHitTestInfo
        {

            /// <summary>
            /// Initializes a new instance of the <see cref="GridCellHitTestInfo"/> class.
            /// </summary>
            /// <param name="grid">The grid.</param>
            /// <param name="point">The point.</param>
            internal GridCellHitTestInfo(OlapGridBase grid, Point point)
            {
                if (grid.PointToCellRowColumnIndex(point, true) != RowColumnIndex.Empty)
                {
                    this.RowColIndex = grid.PointToCellRowColumnIndex(point, true);
                    this.Location = point;
                }
            }

            /// <summary>
            /// Gets or sets the location.
            /// </summary>
            /// <value>The location.</value>
            internal Point Location
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the index of the row col.
            /// </summary>
            /// <value>The index of the row col.</value>
            internal RowColumnIndex RowColIndex
            {
                get;
                set;
            }
        }

        #region IMouseController Members


        #endregion
    }
}
