#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.ComponentModel;
using Syncfusion.UI.Xaml.Diagram.Controller;
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINRT_USING
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using ShapesPath = Windows.UI.Xaml.Shapes.Path;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using ShapesPath=System.Windows.Shapes.Path;
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    internal partial class PageSettingsWrapper : WrapperBase, IPageSettings
    {
        private MeasurementUnit _unit = null;

        private bool _mHeightCustomized = false;

        private bool _mWidthCustomized = false;
        public PageSettingsWrapper(IPageSettings source, SharedData shared)
            : base(shared)
        {
            Source = source;

        }

        public void OnPageSettingsChanged()
        {
            Source = SharedData.Graph.PageSettings;

            UpdatePageBounds(_left, _right, _top, _bottom);
        }

        private void UnitChangedEvent(object sender, UnitToUnitEventArgs<double> current)
        {
            SharedData._unitchanging = true;
            if (SharedData.Graph.InternalNodes != null)
            {
                foreach (var node in SharedData.Graph.InternalNodes)
                {
                    node.OffsetX = current.Invoke(node.OffsetX);
                    node.OffsetY = current.Invoke(node.OffsetY);
                    node.UnitWidth = current.Invoke(node.UnitWidth);
                    node.UnitHeight = current.Invoke(node.UnitHeight);
                }
            }
            if (SharedData.Graph.InternalConnectors != null)
            {
                foreach (var con in SharedData.Graph.InternalConnectors)
                {
                    con.SourcePoint = new Point(current.Invoke(con.SourcePoint.X), current.Invoke(con.SourcePoint.Y));
                    con.TargetPoint = new Point(current.Invoke(con.TargetPoint.X), current.Invoke(con.TargetPoint.Y));
                }
            }
            (SharedData.Graph.InternalSelectedItems.View as Selector).InvalidateMeasure();
            Thickness view = PrintMargin;
            double Left = current.Invoke(view.Left);
            double Right = current.Invoke(view.Right);
            double top = current.Invoke(view.Top);
            double bottom = current.Invoke(view.Bottom);
            PrintMargin = new Thickness(Left, top, Right, bottom);
            PageWidth = current.Invoke(PageWidth);
            PageHeight = current.Invoke(PageHeight);
            SharedData._unitchanging = false;

            if (SharedData.Graph.HorizontalRuler != null)
            {
                SharedData.Graph.HorizontalRuler.PrepareUnit(Unit);
            }
            if (SharedData.Graph.VerticalRuler != null)
            {
                SharedData.Graph.VerticalRuler.PrepareUnit(Unit);
            }
            SharedData.SnapSettingsController.PrepareUnit();

            //SharedData.PageSettingsController.PrepareUnit();
        }


        internal void UpdatePageBreaks()
        {
            SharedData.GridLinePanel.Clear();

            if (ShowPageBreaks && _mWidthCustomized && _mHeightCustomized)
            {
                if (SharedData != null && SharedData.ScrollViewer != null)
                {
                    if (MultiplePage && ShowPageBreaks)
                    {
                        //To optimize, PrintMargin can be replaced by any local variable
                        Rect view = SharedData.ScrollViewer.Viewport;
                        double currentZoom = SharedData.ScrollViewer.CurrentZoom;
                        int columncount = SetColumnCount(_right - _left, PageWidth);
                        int rowcount = SetRowCount(_bottom - _top, PageHeight);

                        double left = -view.Left + (PrintMargin.Left + _left) * currentZoom;
                        double top = -view.Top + (PrintMargin.Top + _top) * currentZoom;

                        double width = (PageWidth * columncount - PrintMargin.Right * 2) * currentZoom;
                        double height = (PageHeight * rowcount - PrintMargin.Bottom * 2) * currentZoom;

                        SharedData.GridLinePanel.DrawVLine(new Point(left, top), new Point(left + width, top));
                        SharedData.GridLinePanel.DrawHLine(new Point(left, top), new Point(left, top + height));

                        for (int column = 1; column < columncount; column++)
                        {
                            SharedData.GridLinePanel.DrawVLine(new Point(left + (PageWidth * column - PrintMargin.Right) * currentZoom, top),
                                new Point(left + (PageWidth * column - PrintMargin.Right) * currentZoom, top + height));
                        }
                        for (int row = 1; row < rowcount; row++)
                        {
                            SharedData.GridLinePanel.DrawHLine(new Point(left, top + (PageHeight * row - PrintMargin.Bottom) * currentZoom),
                                new Point(left + width, top + (PageHeight * row - PrintMargin.Bottom) * currentZoom));
                        }
                        SharedData.GridLinePanel.DrawVLine(new Point(left + width, top), new Point(left + width, top + height));
                        SharedData.GridLinePanel.DrawHLine(new Point(left, top + height), new Point(left + width, top + height));
                    }
                }
            }
        }

        //Calculating Row count
        private int SetRowCount(double height, double p)
        {
            int count = 0;
            int value = (int)(height / p);
            double diff = height - (value * p);
            if (diff > 1)
            {
                count = value + 1;
            }
            else
            {
                count = value;
            }
            return count;
        }

        //Calculating Colum Count
        private int SetColumnCount(double width, double p)
        {
            int count = 0;
            int value = (int)(width / p);
            double diff = width - (value * p);
            if (diff > 1)
            {
                count = value + 1;
            }
            else
            {
                count = value;
            }
            return count;

        }

        internal int CalculateRow(double compareHeight, double checkHeight)
        {
            int vcount = 1;
            if (compareHeight > checkHeight)
            {
                vcount = SetRowCount(compareHeight, checkHeight);
            }
            return vcount;
        }

        internal int CalculateColumn(double compareWidth, double checkWidth)
        {
            int hcount = 1;
            if (compareWidth > checkWidth)
            {
                hcount = SetColumnCount(compareWidth, checkWidth);
            }
            return hcount;
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            if (propertyName == "PageBackground")
            {
                if (SharedData.ScrollViewer != null)
                    SharedData.ScrollViewer.SetPageBackground(PageBackground, propertyName);
            }
            else if (propertyName == "PageWidth" || propertyName == "PageHeight")
            {
                if (propertyName.Equals("PageWidth"))
                {
                    if (!double.IsNaN(PageWidth) && PageWidth > 0)
                    {
                        _mWidthCustomized = true;
                    }
                    else
                        _mWidthCustomized = false;
                }
                else if (propertyName.Equals("PageHeight"))
                {
                    if (!double.IsNaN(PageHeight) && PageHeight > 0)
                    {
                        _mHeightCustomized = true;
                    }
                    else
                        _mHeightCustomized = false;
                }

                if (!SharedData._unitchanging)
                {
                    SharedData.SpatialSearch.UpdatePageBounds();
                }
                if (PageWidth == PageHeight)
                {
                    return;
                }
                if (PageWidth > PageHeight)
                {
                    if (PageOrientation == PageOrientation.Portrait)
                    {
                        PageOrientation = PageOrientation.Landscape;
                    }
                }
                else if (PageHeight > PageWidth)
                {
                    if (PageOrientation == PageOrientation.Landscape)
                    {
                        PageOrientation = PageOrientation.Portrait;
                    }
                }
                //this.UpdatePageBreaks();
#if SyncfusionFramework4_5_1 && WINRT
                UpdateExportSettings();
#endif
            }
            else if (propertyName == "PageOrientation")
            {
                bool swap = false;
                if (PageOrientation == PageOrientation.Landscape)
                {
                    if (PageHeight > PageWidth)
                    {
                        swap = true;
                    }
                }
                else
                {
                    if (PageWidth > PageHeight)
                    {
                        swap = true;
                    }
                }
                if (swap)
                {
                    double temp = PageWidth;
                    PageWidth = PageHeight;
                    PageHeight = temp;
                }
                //this.UpdatePageBreaks();
            }
            else if (propertyName == "Unit")
            {
                OnUnitChanged();
            }
            else if (propertyName.Equals("PageBorderThickness"))
            {
                if (SharedData.ScrollViewer != null)
                    SharedData.ScrollViewer.SetPageBackground(PageBorderThickness, propertyName);
            }
            else if (propertyName.Equals("PageBorderBrush"))
            {
                if (SharedData.ScrollViewer != null)
                    SharedData.ScrollViewer.SetPageBackground(PageBorderBrush, propertyName);
            }
            else if (propertyName == "ShowPageBreaks")
            {
                if (!SharedData._unitchanging)
                    this.UpdatePageBreaks();
            }
            else if (propertyName == "PrintMargin")
            {
                if (!SharedData._unitchanging)
                    UpdatePageBreaks();
            }
            else if (propertyName == "MultiplePage")
            {
                SharedData.SpatialSearch.UpdatePageBounds();
            }
        }

        internal void UpdatePageBounds(double left, double right, double top, double bottom)
        {
            if (SharedData.ScrollViewer != null)
            {
                Rect view = SharedData.ScrollViewer.Viewport;

                if (_mHeightCustomized && _mWidthCustomized)
                {
                    if (MultiplePage && right - left > 0 && bottom - top > 0)
                    {
                        _left = Math.Floor(left / PageWidth) * PageWidth;
                        _top = Math.Floor(top / PageHeight) * PageHeight;
                        _right = Math.Ceiling(right / PageWidth) * PageWidth;
                        _bottom = Math.Ceiling(bottom / PageHeight) * PageHeight;
                    }
                    else
                    {
                        _left = 0; _right = PageWidth; _top = 0;
                        _bottom = PageHeight;
                    }
                }
                else
                {
                    _left = left;
                    _right = right;
                    _top = top;
                    _bottom = bottom;
                }
                ScrollChanged current = SharedData.ScrollViewer._mCurrentState;
                current.PageBounds = new Rect(_left, _top, _right - _left, _bottom - _top);
                if (current.PageBounds != SharedData.ScrollViewer._mCurrentState.PageBounds)
                {
                    UpdateBackground(SharedData.ScrollViewer.Viewport);
                }
                current.ContentBounds = new Rect(new Point(left != long.MaxValue ? left : 0, top != long.MaxValue ? top : 0),
                                                 new Point(right != long.MinValue ? right : 0, bottom != long.MinValue ? bottom : 0));
                SharedData.ScrollViewer.InvokeViewportChangedEvent(current);
            }
        }

      internal  double _left = 0;
      internal double _top = 0;
      internal double _right = 0;
      internal double _bottom = 0;

        internal void UpdateBackground(Rect view)
        {
            Canvas.SetLeft(SharedData.ScrollViewer._pageBackground, -view.Left + _left * SharedData.ScrollViewer.CurrentZoom);
            Canvas.SetTop(SharedData.ScrollViewer._pageBackground, -view.Top + _top * SharedData.ScrollViewer.CurrentZoom);
            SharedData.ScrollViewer._pageBackground.Width = (_right - _left) * SharedData.ScrollViewer.CurrentZoom;
            SharedData.ScrollViewer._pageBackground.Height = (_bottom - _top) * SharedData.ScrollViewer.CurrentZoom;
            if (ShowPageBreaks)
                UpdatePageBreaks();
#if SyncfusionFramework4_5_1 && WINRT
            UpdateExportSettings();
#endif
        }

        protected override void SharedDataInitialized()
        {
        }

        protected override void SourceChanged()
        {
            if (ShowPageBreaks)
            {
                UpdatePageBreaks();
            }
            OnUnitChanged();
            OnPropertyChanged(PageSettingsConstants.PageOrientation);
            OnPropertyChanged(PageSettingsConstants.PageWidth);
            OnPropertyChanged(PageSettingsConstants.PageHeight);
            OnPropertyChanged(PageSettingsConstants.PageBackground);
            OnPropertyChanged(PageSettingsConstants.PageBorderBrush);
            OnPropertyChanged(PageSettingsConstants.PageBorderThickness);
        }

        private void OnUnitChanged()
        {
            if (_unit != null)
            {
                _unit.UnitChangedEvent -= UnitChangedEvent;
            }
            _unit = Unit;
            if (_unit != null)
            {
                _unit.UnitChangedEvent += UnitChangedEvent;
            }
            if (SharedData.Graph.HorizontalRuler != null)
            {
                SharedData.Graph.HorizontalRuler.PrepareUnit(_unit);
            }
            if (SharedData.Graph.VerticalRuler != null)
            {
                SharedData.Graph.VerticalRuler.PrepareUnit(_unit);
            }
            PrintMargin = SharedData.Unit.ToUnit(PrintMargin);
            //PrepareUnit();
        }

        public override void Dispose()
        {
            if (Unit != null)
            {
                Unit.UnitChangedEvent -= UnitChangedEvent;
            }
        }


#if SyncfusionFramework4_5_1 && WINRT
        PageSetup _mPageSetup = PageSetup.PrintandPageProperties;
        public PageSetup PageSetup
        {
            get
            {
                return _mPageSetup;
             
            }
            set
            {
                _mPageSetup = value;
     
            }
        }

        internal void UpdateRowandColoumn(out int vcount, out int hcount, double width, double height)
        {
            vcount = CalculateRow((_bottom - _top), height);
            hcount = CalculateColumn((_right - _left), width);
        }

        internal void UpdateExportSettings()
        {
            if (SharedData.Graph.ExportSettings != null && SharedData.ScrollViewer != null)
            {
                if (SharedData.Graph.ExportSettings.ExportMode == ExportMode.PageSettings)
                {
                    if (SharedData.Graph.ExportSettings.Clip.Equals(Rect.Empty))
                    {

                        SharedData.Graph.ExportSettings.ColumnCount = CalculateColumn((_right - _left), PageWidth);
                        SharedData.Graph.ExportSettings.RowCount = CalculateRow((_bottom - _top), PageHeight);
                    }
                }
            }
        }
#endif
    }
}
