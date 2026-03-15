#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows.Input;
using System.Windows;
using System.Linq;

#if !WinRT
using System.Windows.Controls;
using Syncfusion.Windows.Controls.Scroll;
using System.Security.Permissions;
namespace Syncfusion.Windows.Controls.Grid
{
#else
using System.ComponentModel;
using Syncfusion.WinRT.Controls.Scroll;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
namespace Syncfusion.WinRT.Controls.Grid
{
#endif
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridColumnAutoSizer : IDisposable
    {
        #region Fields

        private int dragColumn = -1;
#if SILVERLIGHT || WinRT
        internal static readonly double ScrollBarWidth = 19d;
#else
        internal static readonly double ScrollBarWidth = SystemParameters.VerticalScrollBarWidth;
#endif

        internal static readonly double Border = 1;
        private IMouseController mouseController = null;
        private IMouseController tempMouseController = null;
        protected FrameworkElement frameWorkElement = null;
        private bool listenToCurrentCellEditing = true;

        #endregion Fields

        #region ctor
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="gcB"></param>
        public GridColumnAutoSizer(GridModel model)
        {
            this.Model = model;
        }

        private void GridControl_CurrentCellEditingComplete(object sender, ComponentModel.SyncfusionRoutedEventArgs args)
        {
            if (this.Model.Options.ColumnSizer != GridControlLengthUnitType.Star && this.Model.Options.ColumnSizer != GridControlLengthUnitType.None)
            {
                foreach (var gridView in this.Model.Views)
                {
                    // this.ApplySizes();
#if (!SILVERLIGHT &&  !WinRT)
                    gridView.CurrentCell.Refresh();
#endif
                    var styleinfo = (GridStyleInfo)this.Model[this.Model.Views.First().CurrentCell.RangeInfo.Top, this.Model.Views.First().CurrentCell.RangeInfo.Left];
                    var cellModelBase = styleinfo.CellModel;
                    var size = cellModelBase.CalculatePreferredCellSize(this.Model.Views.First().CurrentCell.RowIndex, this.Model.Views.First().CurrentCell.ColumnIndex, styleinfo, GridQueryBounds.Width);
                    if (this.Model.ColumnWidths[gridView.CurrentCell.ColumnIndex] <= size.Width)
                    {
                        this.Model.ColumnWidths[gridView.CurrentCell.ColumnIndex] = size.Width;
                    }
                }
            }
        }

        #endregion ctor

        public GridModel Model
        {
            get;
            private set;
        }

        #region Events

        /// <summary>
        /// Column Width Line size changed
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">RangeChangedEventArguments</param>
        private void ColumnWidths_LineSizeChanged(object sender, RangeChangedEventArgs e)
        {
            if (this.Model.Options.ColumnSizer == GridControlLengthUnitType.Star)
            {
                this.dragColumn = e.To;
                this.ApplySizes();
            }
        }

        /// <summary>
        /// Parent Element Size changed Event Handler
        /// </summary>
        /// <param name="sender">Sender Object </param>
        /// <param name="e">SizeChangedEventArgs</param>
        private void _SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Model.Options.ColumnSizer == GridControlLengthUnitType.Star || this.Model.Options.ColumnSizer == GridControlLengthUnitType.AutoWithLastColumnFill)
            {
                this.ApplySizes();
            }
        }

        #endregion Events

        #region Methods
        public bool ListenToSizeChanged { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether Allow the Grid to calculate the size from the Available parent space.
        /// </summary>
        /// <value>
        /// 	<c>true</c> grid wait for the availabel size form the parent; otherwise, <c>false</c> set calculated size for grid.
        /// </value>
        public bool AllowAutoCalculateSize { get; set; }
        internal bool WireLoadedEvent { get; set; }
        /// <summary>
        /// Apply Sizes as per the user request
        /// </summary>
        public void ApplySizes()
        {
            if (this.Model.Options.ColumnSizer == GridControlLengthUnitType.None)
            {
                return;
            }

            if (this.frameWorkElement == null)
            {
#if (!SILVERLIGHT &&  !WinRT)
                if (this.Model.Views.First().IsLoaded)
                {
                    EnsureParent();
                }
                else if (this.WireLoadedEvent)
                {
                    this.Model.Views.First().Loaded += new RoutedEventHandler(GridColumnAutoSizer_Loaded);
                }
#else
                if (this.frameWorkElement != null)
                {
                    if (this.frameWorkElement.ActualWidth <= 0)
                    {
                        this.frameWorkElement.Loaded += new RoutedEventHandler(frameWorkElement_Loaded);
                    }
                    else
                    {
                        this.ResizeColumns();
                    }
                }
#endif
            }

            if (this.frameWorkElement != null)
            {
#if (!SILVERLIGHT &&  !WinRT)
                if (this.frameWorkElement.IsLoaded)
                {
                    this.ResizeColumns();
                }
                else
                {
                    this.frameWorkElement.Loaded += new RoutedEventHandler(frameWorkElement_Loaded);
                }
#else
                if (this.frameWorkElement.ActualWidth <= 0)
                {
                    this.frameWorkElement.Loaded += new RoutedEventHandler(frameWorkElement_Loaded);
                }
                else
                {
                    this.ResizeColumns();
                }
#endif
            }
        }

        void GridColumnAutoSizer_Loaded(object sender, RoutedEventArgs e)
        {
            this.Model.Views.First().Loaded -= new RoutedEventHandler(GridColumnAutoSizer_Loaded);
            this.EnsureParent();
        }

        private void EnsureParent()
        {
#if !WinRT
            if (!(this.Model is GridDataChildTableModel) && this.listenToCurrentCellEditing)
            {
                this.listenToCurrentCellEditing = false;
                this.Model.Views.First().CurrentCellEditingComplete += new Syncfusion.Windows.ComponentModel.GridRoutedEventHandler(GridControl_CurrentCellEditingComplete);
            }
#endif
            this.frameWorkElement = this.Model.Views.First().Parent as FrameworkElement;
            if (this.ListenToSizeChanged)
            {
                this.frameWorkElement.SizeChanged += new SizeChangedEventHandler(this._SizeChanged);
            }
#if (!SILVERLIGHT &&  !WinRT)

            if (this.frameWorkElement.IsLoaded)
            {
                this.ResizeColumns();
            }
            else
            {
                this.frameWorkElement.Loaded += new RoutedEventHandler(frameWorkElement_Loaded);
            }
#else
            if (this.frameWorkElement.ActualWidth <= 0)
            {
                this.frameWorkElement.Loaded += new RoutedEventHandler(frameWorkElement_Loaded);
            }
            else
            {
                this.ResizeColumns();
            }
#endif
        }

        private void frameWorkElement_Loaded(object sender, RoutedEventArgs e)
        {
            this.frameWorkElement.Loaded -= new RoutedEventHandler(frameWorkElement_Loaded);
            this.ResizeColumns();
        }

        internal void ResizeColumns()
        {
            if (this.Model.ColumnCount <= 0)
            {
                return;
            }
            var maxLength = this.Model.RowCount;
            if (this.Model.Options.MaxLength > 0 && this.Model.Options.MaxLength <= this.Model.RowCount)
            {
                maxLength = this.Model.Options.MaxLength;
            }

            if (this.frameWorkElement != null)
            {
                switch (this.Model.Options.ColumnSizer)
                {
                    case GridControlLengthUnitType.None:
                        this.MouseControllerAdder();
                        this.Dispose();
                        break;

                    case GridControlLengthUnitType.Auto:
                        this.ApplySizes(new GridRangeInfo(0, 0, maxLength, this.Model.ColumnCount - 1));
                        this.MouseControllerRemover();
                        break;

                    case GridControlLengthUnitType.AutoWithLastColumnFill:
                        double val = 0;
                        if (this.frameWorkElement is ScrollViewer)
                        {
                            val += ((ScrollViewer)this.frameWorkElement).ComputedVerticalScrollBarVisibility == Visibility.Visible ? GridColumnAutoSizer.ScrollBarWidth : 0;
                        }

                        var targetWidth = GridColumnAutoSizer.Border + this.frameWorkElement.ActualWidth;
                        this.ApplySizes(new GridRangeInfo(0, 0, maxLength, (this.Model.ColumnCount - 1)));
                        for (int p = 0; p < this.Model.ColumnCount - 1; p++)
                        {
                            val += this.Model.ColumnWidths[p] + 0.09;
                        }

                        this.Model.Views.First().SetColumnWidth((this.Model.ColumnCount - 1), (targetWidth - val - 1));
                        this.Model.Views.First().ScrollColumns.ResetLineResize();
                        this.MouseControllerRemover();
                        break;

                    case GridControlLengthUnitType.SizeToCells:
                        this.ApplySizes(new GridRangeInfo(this.Model.HeaderRows, this.Model.HeaderColumns, maxLength, this.Model.ColumnCount - 1));
                        this.MouseControllerRemover();
                        break;

                    case GridControlLengthUnitType.SizeToHeader:
                        var range = new GridRangeInfo(0, 0, this.Model.HeaderRows - 1, this.Model.ColumnCount - 1);
                        this.ApplySizes(range);
                        this.MouseControllerRemover();
                        break;

                    case GridControlLengthUnitType.Star:
                        this.ApplyStarSizer(0);
                        break;
                }
            }
        }

        protected virtual void ApplyStarSizer(int group)
        {
            this.Model.Views.First().ColumnWidths.LineSizeChanged -= new RangeChangedEventHandler(this.ColumnWidths_LineSizeChanged);
            var targetWidth = this.frameWorkElement.ActualWidth - GridColumnAutoSizer.Border;
            var colWidth = 0d;
            if (this.frameWorkElement is ScrollViewer)
            {
                targetWidth -= ((ScrollViewer)this.frameWorkElement).ComputedVerticalScrollBarVisibility == Visibility.Visible ? GridColumnAutoSizer.ScrollBarWidth : 0;
            }

            int lineSize = 0;
            int hiddenlineCount = 0;

            for (int k = 0; k < this.Model.ColumnCount; k++)
            {
                if (this.Model.ColumnWidths.GetHidden(k, out lineSize))
                {
                    hiddenlineCount++;
                }
            }


            if (this.dragColumn > -1)
            {
                targetWidth -= this.Model.ColumnWidths[this.dragColumn];
                if (group > 0)
                {
                    colWidth = (targetWidth - (group * 26d)) / (this.Model.ColumnCount - 1 - group);
                }
                else
                {
                    colWidth = targetWidth / (this.Model.ColumnCount - 1);
                }
            }
            else
            {
                if (group > 0)
                    colWidth = (targetWidth - (group * 26d)) / (this.Model.ColumnCount - group);
                else
                    colWidth = targetWidth / (this.Model.ColumnCount - hiddenlineCount);
            }

            for (int i = group; i < this.Model.ColumnCount; i++)
            {
                if (this.dragColumn == i)
                {
                    i++;
                }

                if (i == this.Model.ColumnCount)
                {
                    break;
                }

                this.Model.Views.First().SetColumnWidth(i, colWidth);
            }

            this.MouseControllerAdder();
            this.Model.Views.First().ScrollColumns.ResetLineResize();
            this.Model.Views.First().ColumnWidths.LineSizeChanged += new RangeChangedEventHandler(this.ColumnWidths_LineSizeChanged);
        }

        protected virtual void ApplySizes(GridRangeInfo range)
        {
            this.Model.ResizeColumnsToFit(range, GridResizeToFitOptions.None);
        }

        #region IDisposable
        /// <summary>
        /// IDisposable interace implementation
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
            if (dispose)
            {
                this.UnwireEvents();
                if (this.mouseController != null)
                {
                    this.mouseController = null;
                }

                if (this.tempMouseController != null)
                {
                    this.tempMouseController = null;
                }

                if (this.frameWorkElement != null)
                {
                    this.frameWorkElement = null;
                }
            }
        }

        private void UnwireEvents()
        {
#if !WinRT
            if (!(this.Model is GridDataChildTableModel) && this.Model.Views != null && this.Model.Views.Count() > 0)
            {
                this.Model.Views.First().CurrentCellEditingComplete -= GridControl_CurrentCellEditingComplete;
            }
#endif
            if (this.frameWorkElement != null)
            {
                this.frameWorkElement.SizeChanged -= this._SizeChanged;
            }
        }
        #endregion IDisposable

        /// <summary>
        /// Removes Mouse Controller
        /// </summary>
        private void MouseControllerRemover()
        {
            this.mouseController = this.Model.Views.First().Model.Views.First().MouseControllerDispatcher.Find("ResizeColumnsMouseController");
            if (this.mouseController != null)
            {
                this.tempMouseController = this.mouseController;
                this.Model.Views.First().MouseControllerDispatcher.Remove(this.mouseController);
                this.mouseController = null;
            }
        }

        /// <summary>
        /// Removes Mouse Controller
        /// </summary>
        private void MouseControllerAdder()
        {
            this.mouseController = this.Model.Views.First().MouseControllerDispatcher.Find("ResizeColumnsMouseController");
            if (this.mouseController == null && this.tempMouseController != null)
            {
                this.Model.Views.First().MouseControllerDispatcher.Add(this.tempMouseController);
            }
        }
        #endregion Methods
    }

    //// Summary:
    ////     Defines constants that describe how Syncfusion.Windows.Controls.GridControl elements,
    ////     such as columns, are sized.
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public enum GridControlLengthUnitType
    {
        //// Summary:
        //// No Sizing
        None = 0,
        ////
        //// Summary:
        ////     The unit of measure is based on the size of the cells and the column header.
        Auto,
        ////
        //// Summary:
        ////     The unit of measure is based on the size of the cells and the column header with Last column fill.
        AutoWithLastColumnFill,
        ////
        //// Summary:
        ////     The unit of measure is based on the size of the cells and the column header with Last column fill and for only once.
        AutoOnLoad,
        ////
        //// Summary:
        ////     The unit of measure is based on the size of the cells and the column header with Last column fill and for only once.
        AutoOnLoadWithLastColumnFill,
        ////
        //// Summary:
        ////     The unit of measure is based on the size of the cells.
        SizeToCells,
        ////
        //// Summary:
        ////     The unit of measure is based on the size of the column header.
        SizeToHeader,
        ////
        //// Summary:
        ////     The unit of measure is a weighted proportion of the available space.
        Star,

    }
}