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
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using System.Windows.Input;

namespace Syncfusion.Windows.Diagram
{
    public partial class FourQuadrantPanel : Grid
    {
        DiagramView view = null;

        public FourQuadrantPanel()
            : base()
        {
            this.Loaded += new RoutedEventHandler(ScrollableGrid_Loaded);
            if (dc != null && dc.View != null)
            {
                view = dc.View;
                view.ScrollGrid = this as FourQuadrantPanel;
            }
        }
        void HorizontalThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (view != null)
            {
                if (view.EnableVirtualization)
                {
                    this.callCalculate();
                    ////if (disp != null)
                    ////{
                    ////    disp.Abort();
                    ////}

                    //disp = this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.SystemIdle,
                    //                  new Syncfusion.Windows.Diagram.DiagramModel.Initialize(this.callCalculate));
                }
                view.UpdateViewGridOrigin();
                Size view_Size = view.returnViewGridSize(_MasterParent);
                (view.viewgrid.Children[1] as DiagramViewGrid).Arrange(new Rect(-(view.Page as DiagramPage).Left, -(view.Page as DiagramPage).Top, view_Size.Width, view_Size.Height));
            }
            else
            {

            }
        }
        void VerticalThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (view != null)
            {
                if (view.EnableVirtualization)
                {
                    this.callCalculate();
                    ////if (disp != null)
                    ////{
                    ////    disp.Abort();
                    ////}

                    //disp = this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.SystemIdle,
                    //                  new Syncfusion.Windows.Diagram.DiagramModel.Initialize(this.callCalculate));
                }

                view.UpdateViewGridOrigin();
            }
            else
            {

            }
        }

        void ScrollableGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (_ScrollOwner != null)
            {
                this.Loaded -= new RoutedEventHandler(ScrollableGrid_Loaded);
                _ScrollOwner.ScrollChanged += new ScrollChangedEventHandler(_ScrollOwner_ScrollChanged);
            }
            else
            {
                _ScrollOwner = this._MasterParent.ScrollOwner;
                this.Loaded -= new RoutedEventHandler(ScrollableGrid_Loaded);
                _ScrollOwner.ScrollChanged += new ScrollChangedEventHandler(_ScrollOwner_ScrollChanged);
            }
            view = this.TemplatedParent as DiagramView;
            view.ScrollGrid = this as FourQuadrantPanel;
            view.VerifyVirtualization();
            if (view.BoundaryBackground != null)
            {
                _ScrollOwner.SetBinding(ScrollViewer.BackgroundProperty, view.BoundaryBackground);
            }
            else
            {
                view.ScrollGrid = this;
            }
            if (view.ScrollGrid._MasterParent!=null)
            {
                view.ScrollGrid._MasterParent.ExtraPanning +=
                            new OverviewContentHolder.ExtraPanningEventEventHandler(_MasterParent_ExtraPanning);
                double _w = (view.Scrollviewer.HorizontalOffset + view.Scrollviewer.ViewportWidth) -
                            view.Scrollviewer.ExtentWidth;
                double _h = (view.Scrollviewer.VerticalOffset + view.Scrollviewer.ViewportHeight) -
                            view.Scrollviewer.ExtentHeight;
                if (_w > 0 && _h > 0)
                {
                    _MasterParent_ExtraPanning(null, new OverviewContentHolder.ExtraPanningEventEventArgs(true) { ExtraSize = new Size() { Width = _w, Height = _h } });
                }
            }
        }

        void _MasterParent_ExtraPanning(object sender, OverviewContentHolder.ExtraPanningEventEventArgs evtArgs)
        {
            this.m_height = new Size(evtArgs.ExtraSize.Width + this.m_oldSize.Width, evtArgs.ExtraSize.Height + this.m_oldSize.Height);
            this.m_oldSize = this.m_height;
            this.InvalidateMeasure();
            this.InvalidateArrange();
        }

        DispatcherOperation disp;
        bool evenctcheck = true;
        bool horizontalcheck = true;
        bool verticalcheck = true;
        double Viewheight;
        double ViewWidth;

        void _ScrollOwner_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (evenctcheck)
            {
                if (horizontalcheck)
                {
                    ScrollBar Horizontalthumb = (ScrollBar)(VisualTreeHelper.GetChild(sender as ScrollViewer, 0) as FrameworkElement).FindName("PART_HorizontalScrollBar"); 
                    if (Horizontalthumb.Track != null)
                    {
                        Horizontalthumb.Track.Thumb.DragCompleted += new DragCompletedEventHandler(HorizontalThumb_DragCompleted);
                        horizontalcheck = false;
                    }
                    VerifyScrollVirtual();
                }

                if (verticalcheck)
                {
                    ScrollBar VerticalThumb = (ScrollBar)(VisualTreeHelper.GetChild(sender as ScrollViewer, 0) as FrameworkElement).FindName("PART_VerticalScrollBar");
                    if (VerticalThumb.Track != null)
                    {
                        VerticalThumb.Track.Thumb.DragCompleted += new DragCompletedEventHandler(VerticalThumb_DragCompleted);
                        verticalcheck = false;
                    }
                    VerifyScrollVirtual();
                }
                if (!horizontalcheck && !verticalcheck)
                { evenctcheck = false; }

            }
            if (ViewWidth != (sender as ScrollViewer).ViewportWidth)
            {
                if (Viewheight != (sender as ScrollViewer).ViewportHeight)
                {
                    if (dc.View.EnableVirtualization)
                    {
                        this.callCalculate();
                    }
                }
                else
                {
                    if (dc.View.EnableVirtualization)
                    {
                        this.callCalculate();
                    }
                }
            }
            else if (Viewheight != (sender as ScrollViewer).ViewportHeight)
            {
                if (dc.View.EnableVirtualization)
                {
                    this.callCalculate();
                }
            }
            ViewWidth = (sender as ScrollViewer).ViewportWidth;
            Viewheight = (sender as ScrollViewer).ViewportHeight;
            if(view._contentHolder!=null)
            view._contentHolder.InvalidateMeasure();
            if (view != null)
            {
                if (view.EnableVirtualization)
                {
                    if (disp != null)
                    {
                        disp.Abort();
                    }
                    disp = this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.SystemIdle,
                                      new Syncfusion.Windows.Diagram.DiagramModel.Initialize(this.callCalculate));
                }
            }
            //else
            //{

            //}
        }

        internal void callCalculate()
        {
            this.calculatesize(_MasterParent.ScrollOwner);
        }
        internal void MakeVirtualization()
        {
            if (view != null)
            {
                if (view.EnableVirtualization)
                {
                    if (disp != null)
                    {
                        disp.Abort();
                    }
                    disp = this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.SystemIdle,
                                      new Syncfusion.Windows.Diagram.DiagramModel.Initialize(this.callCalculate));
                }
            }

        }
        internal void VerifyScrollVirtual()
        {
            if (view != null)
            {
                if (view.EnableVirtualization)
                {
                    if (disp != null)
                    {
                        disp.Abort();
                    }
                    disp = this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.SystemIdle,
                                      new Syncfusion.Windows.Diagram.DiagramModel.Initialize(this.callCalculate));
                }
                else
                {
                    if (disp != null)
                    {
                        disp.Abort();
                    }
                }
            }

        }

        internal void CallNormailzation()
        {
            this.CalculateNormalization();
            view.Page.InvalidateMeasure();
            view.Page.InvalidateArrange();
        }

        private void CalculateNormalization()
        {
            view = dc.View;
            foreach (UIElement node in (view.Page as DiagramPage).AllChildren)
            {
                if (node is Node)
                {
                    if (!(node as Node).IsInternallyLoaded)
                    {
                        if (!view.Page.Children.Contains(node))
                        {
                            (view.Page as DiagramPage).AddingChildren(node);
                        }

                    }

                }
                else if (node is LineConnector)
                {

                    if (!(node as LineConnector).IsInternallyLoaded)
                    {
                        if (!view.Page.Children.Contains(node))
                        {
                            (view.Page as DiagramPage).AddingChildren(node);
                        }

                    }

                }

            }

        }
        internal ScrollViewer _ScrollOwner;
        internal void VirtualzingNode(Node element)
        {
            double Negativeaxistop = -(view.Page as DiagramPage).Top;
            double NegativeaxisLeft = -(view.Page as DiagramPage).Left;
            Rect viewablesize = new Rect();
            if (dc != null && !dc.IsLoaded)
            {
                view = dc.View;
                viewablesize = new Rect(0, 0, Constraint.Width, Constraint.Height);
            }
            else
            {
                if (NegativeaxisLeft < 0)
                {
                    if (Negativeaxistop < 0)
                    {

                        viewablesize = new Rect(_ScrollOwner.HorizontalOffset + NegativeaxisLeft, _ScrollOwner.VerticalOffset + Negativeaxistop, _ScrollOwner.ViewportWidth, _ScrollOwner.ViewportHeight);
                    }
                    else
                    {
                        viewablesize = new Rect(_ScrollOwner.HorizontalOffset + NegativeaxisLeft, _ScrollOwner.VerticalOffset, _ScrollOwner.ViewportWidth, _ScrollOwner.ViewportHeight);
                    }

                }

                else if (Negativeaxistop < 0)
                {
                    viewablesize = new Rect(_ScrollOwner.HorizontalOffset, _ScrollOwner.VerticalOffset + Negativeaxistop, _ScrollOwner.ViewportWidth, _ScrollOwner.ViewportHeight);
                }
                else
                {
                    viewablesize = new Rect(_ScrollOwner.HorizontalOffset, _ScrollOwner.VerticalOffset, _ScrollOwner.ViewportWidth, _ScrollOwner.ViewportHeight);
                }

            }
           
            if (!(element as Node).AllowVirtualization)
            {
                if (!(element as Node).IsInternallyLoaded)
                {
                    (view.Page as DiagramPage).AddingChildren(element);
                    (element as Node).Measure(new Size((element as Node).Width, (element as Node).Height));
                    (element as Node).Arrange(new Rect((element as Node).OffsetX, (element as Node).OffsetY, (element as Node).Width, (element as Node).Height));
                }
            }
            if (element is UIElement)
            {
                Rect nodebounds = new Rect((element as Node).PxOffsetX * view.CurrentZoom, (element as Node).PxOffsetY * view.CurrentZoom, (element as Node)._Width, (element as Node)._Height);
                
                if (viewablesize.IntersectsWith(nodebounds))
                {
                    if (!(element as Node).IsInternallyLoaded)
                    {
                        (view.Page as DiagramPage).AddingChildren(element);
                    }
                }
                else if ((element as Node).IsInternallyLoaded)
                {
                    if ((element as Node).AllowVirtualization)
                    {
                        if (!view.EnableCaching)
                        {
                            if (!(element as Node).currentdragging)
                            {
                                (view.Page as DiagramPage).RemovingChildren(element);
                            }
                        }

                    }
                }
            }

        }
        internal void VirtualizingLine(LineConnector element)
        {
            double Negativeaxistop = -(view.Page as DiagramPage).Top;
            double NegativeaxisLeft = -(view.Page as DiagramPage).Left;
            Rect viewablesize = new Rect();
            if (dc != null && !dc.IsLoaded)
            {
                view = dc.View;
                viewablesize = new Rect(0, 0, Constraint.Width, Constraint.Height);
            }
            else
            {
                if (NegativeaxisLeft < 0)
                {
                    if (Negativeaxistop < 0)
                    {

                        viewablesize = new Rect(_ScrollOwner.HorizontalOffset + NegativeaxisLeft, _ScrollOwner.VerticalOffset + Negativeaxistop, _ScrollOwner.ViewportWidth, _ScrollOwner.ViewportHeight);
                    }
                    else
                    {
                        viewablesize = new Rect(_ScrollOwner.HorizontalOffset + NegativeaxisLeft, _ScrollOwner.VerticalOffset, _ScrollOwner.ViewportWidth, _ScrollOwner.ViewportHeight);
                    }

                }

                else if (Negativeaxistop < 0)
                {
                    viewablesize = new Rect(_ScrollOwner.HorizontalOffset, _ScrollOwner.VerticalOffset + Negativeaxistop, _ScrollOwner.ViewportWidth, _ScrollOwner.ViewportHeight);
                }
                else
                {
                    viewablesize = new Rect(_ScrollOwner.HorizontalOffset, _ScrollOwner.VerticalOffset, _ScrollOwner.ViewportWidth, _ScrollOwner.ViewportHeight);
                }

            }
           
            Rect linebounds = new Rect();
            if (!(element as LineConnector).AllowVirtualization)
            {
                if (!(element as LineConnector).IsInternallyLoaded)
                {
                    (view.Page as DiagramPage).AddingChildren(element);
                }
                //(element as LineConnector).UpdateConnectorPathGeometry();
            }
            if (element is ICommon)
            {
                if ((element as LineConnector).IsInternallyLoaded)
                {
                    if ((element as LineConnector).HeadNode == null && (element as LineConnector).TailNode == null)
                    {
                        linebounds = (element as LineConnector).GetBounds();
                        if (viewablesize.IntersectsWith(linebounds))
                        {
                            if (!(element as LineConnector).IsInternallyLoaded)
                            {
                                (view.Page as DiagramPage).AddingChildren(element);
                            }
                        }
                        else if (!(element as LineConnector).IsInternallyLoaded && !view.EnableCaching)
                        {
                            (view.Page as DiagramPage).RemovingChildren(element);
                        }
                        else if (!view.EnableCaching)
                        {
                            (view.Page as DiagramPage).RemovingChildren(element);
                        }
 
                    }
                }
                else if (!(element as LineConnector).IsInternallyLoaded && !view.EnableCaching)
                {
                    (view.Page as DiagramPage).RemovingChildren(element);
                }
                else if (!view.EnableCaching)
                {
                    (view.Page as DiagramPage).RemovingChildren(element);
                }
            }
            if ((element as LineConnector).HeadNode != null && (element as LineConnector).TailNode != null)
            {

                if (((element as LineConnector).HeadNode as Node).IsInternallyLoaded || ((element as LineConnector).TailNode as Node).IsInternallyLoaded)
                {
                    if (!(element as LineConnector).IsInternallyLoaded)
                    {
                        (view.Page as DiagramPage).AddingChildren(element);
                    }
                }
                else
                {
                    if (!(element as LineConnector).IsInternallyLoaded && !view.EnableCaching)
                    {

                        (view.Page as DiagramPage).RemovingChildren(element);
                    }
                    else if (!view.EnableCaching)
                    {
                        (view.Page as DiagramPage).RemovingChildren(element);
                    }
                }
            }
            else
            {
               
                    linebounds = LineBounds(element as LineConnector);
                    linebounds.X = linebounds.X * view.CurrentZoom;
                    linebounds.Y = linebounds.Y * view.CurrentZoom;
                    linebounds.Width = linebounds.Width * view.CurrentZoom;
                    linebounds.Height = linebounds.Height * view.CurrentZoom;
                    if (viewablesize.IntersectsWith(linebounds))
                    {
                        if (!(element as LineConnector).IsInternallyLoaded)
                        {
                            (view.Page as DiagramPage).AddingChildren(element);
                        }
                    }
               
            }
            //(element as LineConnector).UpdateConnectorPathGeometry();
            //(view.Page as DiagramPage).InvalidateMeasure();
            //(view.Page as DiagramPage).InvalidateArrange();

        }
        private int CompareBandIndex(UIElement bar1, UIElement bar2)
        {
            if (bar1 is Node && bar2 is Node)
            {
                if ((bar1 as Node).OffsetX > (bar2 as Node).OffsetX)
                    return 1;
                else if ((bar1 as Node).OffsetX == (bar2 as Node).OffsetX)
                    return 0;
                else
                    return -1;
            }
            else
            {
                return 0;
            }
        }

        internal Rect GetViewableSize(ScrollViewer scroll)
        {
            Rect viewablesize = new Rect();
            
            if (dc != null && !dc.IsLoaded)
            {
                view = dc.View;
                viewablesize = new Rect(0, 0, Constraint.Width, Constraint.Height);
                return viewablesize;
            }
            else
            {
                double Negativeaxistop = -(view.Page as DiagramPage).Top;
                double NegativeaxisLeft = -(view.Page as DiagramPage).Left;
                if (NegativeaxisLeft < 0)
                {
                    if (Negativeaxistop < 0)
                    {
                        viewablesize = new Rect(scroll.HorizontalOffset + NegativeaxisLeft, scroll.VerticalOffset + Negativeaxistop, scroll.ViewportWidth, scroll.ViewportHeight);
                    }
                    else
                    {
                        viewablesize = new Rect(scroll.HorizontalOffset + NegativeaxisLeft, scroll.VerticalOffset, scroll.ViewportWidth, scroll.ViewportHeight);
                    }
                }
                else if (Negativeaxistop < 0)
                {
                    viewablesize = new Rect(scroll.HorizontalOffset, scroll.VerticalOffset + Negativeaxistop, scroll.ViewportWidth, scroll.ViewportHeight);
                }
                else
                {
                    viewablesize = new Rect(scroll.HorizontalOffset, scroll.VerticalOffset, scroll.ViewportWidth, scroll.ViewportHeight);
                }
                return viewablesize;
            }
        }
        bool onecheck=true;
        private void calculatesize(ScrollViewer scroll)
        {
            if (dc == null)
            {
                dc = DiagramPage.GetDiagramControl(this);
            }
           if(dc!=null&&!dc.IsLoaded&&dc.Model.LayoutType!=LayoutType.None&&onecheck)
            {
                if (dc.Model.LayoutType == LayoutType.HierarchicalTreeLayout)
                {
                    HierarchicalTreeLayout tree = new HierarchicalTreeLayout(dc.Model, dc.View);
                    tree.RefreshLayout();
                }
               else  if (dc.Model.LayoutType == LayoutType.DirectedTreeLayout)
                {
                    DirectedTreeLayout tree = new DirectedTreeLayout(dc.Model, dc.View);
                    tree.RefreshLayout();
                }
                else if (dc.Model.LayoutType == LayoutType.RadialTreeLayout)
                {
                    RadialTreeLayout tree = new RadialTreeLayout(dc.Model, dc.View);
                    tree.RefreshLayout();
                }
                else if (dc.Model.LayoutType == LayoutType.TableLayout)
                {
                    TableLayout tree = new TableLayout(dc.Model, dc.View);
                    tree.RefreshLayout();
                }
                else if (dc.Model.LayoutType == LayoutType.BowtieLayout)
                {
                    BowtieLayout tree = new BowtieLayout(dc.Model, dc.View);
                    tree.RefreshLayout();
                }
                onecheck = false;
            }

            Rect viewablesize = new Rect();
            _ScrollOwner = scroll;
            viewablesize = GetViewableSize(scroll);
            foreach (UIElement element in dc.Model.Nodes)
            {
                VirtualzingNode(element as Node);
            }

            foreach (UIElement element in dc.Model.Connections)
            {
                VirtualizingLine(element as LineConnector);
            }
        }
        private const double LineSize = 16;
        private const double WheelSize = 3 * LineSize;

        //private bool _CanHorizontallyScroll;
        //private bool _CanVerticallyScroll;
        //private ScrollViewer _ScrollOwner;
        //internal Vector _Offset;
        //private Size _Extent;
        //private Size _Viewport;
        Size ButIHave;
        Size INeed;

        internal void VirtualizationRelatedToLine()
        {
            foreach (UIElement element in view.dc.Model.Connections)
            {


                if (!(element as LineConnector).AllowVirtualization)
                {
                    (view.Page as DiagramPage).AddingChildren(element);

                    //}
                    //if (element is ICommon)
                    //{
                    //    if ((element as LineConnector).IsLoaded)
                    //    {
                    //        linebounds = (element as LineConnector).GetBounds();
                    //    }
                    //    else
                    //    {
                    //        linebounds = LineBounds(element as LineConnector);

                    //    }
                    //    if (viewablesize.IntersectsWith(linebounds))
                    //    {


                    //        if (!view.Page.Children.Contains(element))
                    //        {
                    //            (view.Page as DiagramPage).AddingChildren(element);
                    //            // view.Page.Children.Add(element);
                    //        }

                    //    }
                    //    else
                    //    {

                    //        view.Page.Children.Remove(element);
                    //    }
                    //}
                    if (view.EnableCaching)
                    {
                        if (view.Page.Children.Contains(element))
                        {
                            if ((element as LineConnector).HeadNode != null)
                            {
                                if (!(view.Page as DiagramPage).Children.Contains(((element as LineConnector).HeadNode as Node)))
                                {

                                    (view.Page as DiagramPage).AddingChildren(((element as LineConnector).HeadNode as Node));
                                }
                            }
                            if ((element as LineConnector).TailNode != null)
                            {
                                if (!(view.Page as DiagramPage).Children.Contains(((element as LineConnector).TailNode as Node)))
                                { (view.Page as DiagramPage).AddingChildren(((element as LineConnector).TailNode as Node)); }
                            }
                        }
                    }
                }
            }

        }
        internal Rect LineBounds(LineConnector line)
        {
            Rect rect = new Rect();
            if (line.PxStartPointPosition.X < line.PxEndPointPosition.X)
            {
                rect = new Rect(line.PxStartPointPosition.X, line.PxStartPointPosition.Y, Math.Abs(line.PxEndPointPosition.X - line.PxStartPointPosition.X), Math.Abs(line.PxEndPointPosition.Y - line.PxStartPointPosition.Y));
                return rect;
            }
            else
            {
                rect = new Rect(line.PxEndPointPosition.X, line.PxEndPointPosition.Y, Math.Abs(line.PxStartPointPosition.X - line.PxEndPointPosition.X), Math.Abs(line.PxStartPointPosition.Y - line.PxEndPointPosition.Y));
                return rect;
            }
        }


        //protected void VerifyScrollData()
        //{
        //    //if (double.IsInfinity(viewport.Width))
        //    //{ viewport.Width = extent.Width; }

        //    //if (double.IsInfinity(viewport.Height))
        //    //{ viewport.Height = extent.Height; }

        //    _Extent = this.DesiredSize;
        //    _Viewport = new Size(this.ActualWidth, this.ActualHeight);

        //    //_Offset.X = Math.Max(0,
        //    //  Math.Min(_Offset.X, ExtentWidth - ViewportWidth));
        //    //_Offset.Y = Math.Max(0,
        //    //  Math.Min(_Offset.Y, ExtentHeight - ViewportHeight));

        //    if (ScrollOwner != null)
        //    { ScrollOwner.InvalidateScrollInfo(); }
        //}

        //protected void VerifyScrollData(Size viewport, Size extent)
        //{
        //    if (double.IsInfinity(viewport.Width))
        //    { viewport.Width = extent.Width; }

        //    if (double.IsInfinity(viewport.Height))
        //    { viewport.Height = extent.Height; }

        //    _Extent = extent;
        //    _Viewport = viewport;

        //    _Offset.X = Math.Max(0,
        //      Math.Min(_Offset.X, ExtentWidth - ViewportWidth));
        //    _Offset.Y = Math.Max(0,
        //      Math.Min(_Offset.Y, ExtentHeight - ViewportHeight));

        //    if (ScrollOwner != null)
        //    { ScrollOwner.InvalidateScrollInfo(); }
        //}

       

        //private double CalculateNewVisibleArea(double top1,double bottom1, double top2, double bottom2)
        //{
        //    bool offBottom = top2 < top1 && bottom2 < bottom1;
        //    bool offTop = bottom2 > bottom1 && top2 > top1;
        //    bool tooLarge = (bottom2 - top2) > (bottom1 - top1);

        //    if (!offBottom && !offTop)
        //    { return top1; } 

        //    if ((offBottom && !tooLarge) || (offTop && tooLarge))
        //    { return top2; }

        //    return (bottom2 - (bottom1 - top1));
        //}
        //public void SetHorizontalOffset(double offset)
        //{
        //    //offset = Math.Max(0,
        //    //  Math.Min(offset, ExtentWidth - ViewportWidth));
        //    //if (offset != _Offset.X)
        //    //{
        //    //    _Offset.X = offset;
        //    //    InvalidateArrange();
        //    //}
        //    MakeVirtualization();
        //}

        //public void SetVerticalOffset(double offset)
        //{
        //    //offset = Math.Max(0,
        //    //  Math.Min(offset, ExtentHeight - ViewportHeight));
        //    //if (offset != _Offset.Y)
        //    //{
        //    //    _Offset.Y = offset;
        //    //    InvalidateArrange();
        //    //}
        //    //MakeVirtualization();
        //}

      
        internal DiagramControl dc;
        internal Size Constraint;
        internal OverviewContentHolder _MasterParent;
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            dc = GetDiagramControl(this);
        }


        private DiagramControl GetDiagramControl(DependencyObject dep)
        {
            DependencyObject parent =LogicalTreeHelper.GetParent(dep);
            while (parent != null)
            {
                if (parent is DiagramControl)
                {
                    return parent as DiagramControl;
                }
                DependencyObject temp = LogicalTreeHelper.GetParent(parent);
                if (temp == null && parent is FrameworkElement)
                {
                        parent = VisualTreeHelper.GetParent(parent);
                }
                else
                {
                    parent = temp;
                }
            }
            return null;
        }
        private OverviewContentHolder returnHolder(FrameworkElement pane)
        {
            if (pane is OverviewContentHolder)
            {
                return pane as OverviewContentHolder;
            }
            else
            {
                return returnHolder(VisualTreeHelper.GetParent(pane) as FrameworkElement);
            }
        }
        protected override Size MeasureOverride(Size constraint)
        {
            if (dc == null)
            {
                dc = DiagramPage.GetDiagramControl(this);
            }
            if (_MasterParent == null)
            {
                _MasterParent = returnHolder(this); //this.Parent as OverviewContentHolder;                
                _MasterParent.ScrollOwner.ScrollChanged += new ScrollChangedEventHandler(_ScrollOwner_ScrollChanged);
                dc.View._contentHolder = _MasterParent;
                _MasterParent.MouseDown += new System.Windows.Input.MouseButtonEventHandler(_MasterParent_MouseDown);
                if (!dc.View._contentHolder.EnableFitToPage)
                {
                    dc.View._contentHolder.EnableFitToPage = dc.View.EnableFitToPage;
                }
                view=dc.View;
                view.ScrollGrid = this;
                //_MasterParent.IsPanEnabled = view.IsPanEnabled;
            }
            if (dc != null && !dc.IsLoaded)
            {
                if(!double.IsInfinity(constraint.Width))
                Constraint.Width = constraint.Width;
                if (!double.IsInfinity(constraint.Height))
                    Constraint.Height = constraint.Height;
                if (dc.View.EnableVirtualization)
                {
                    this.callCalculate();
                }
                else
                {
                    this.CallNormailzation();
                }
            }
            ButIHave = constraint;
            INeed = base.MeasureOverride(constraint);
            foreach (FrameworkElement ele in this.Children)
            {
                ele.Measure(INeed);
            }
            if (double.IsInfinity(ButIHave.Width))
            { ButIHave.Width = INeed.Width; }

            if (double.IsInfinity(ButIHave.Height))
            { ButIHave.Height = INeed.Height; }
            //VerifyScrollData(ButIHave, INeed);
           //INeed = new Size(Math.Max(_MasterParent.ViewportWidth,_MasterParent.ExtentWidth),Math.Max(_MasterParent.ViewportHeight, _MasterParent.ExtentHeight));
           //if (!view.SizeToContent)
           //    INeed = new Size(Math.Max(view.BoundaryConstraintsArea.Width, INeed.Width), Math.Max(view.BoundaryConstraintsArea.Height, INeed.Height));
            return new Size(Math.Abs(INeed.Width - (view.Page as DiagramPage).Left + m_height.Width),Math.Abs(INeed.Height - (view.Page as DiagramPage).Top + m_height.Height));
        } 

        void _MasterParent_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
        }

        private Point ReturnViewGridSize(Point topleft)
        {
            double _left = topleft.X + (view.Page as DiagramPage).PxGridHorizontalOffset - (topleft.X % (view.Page as DiagramPage).PxGridHorizontalOffset);
            double _top = topleft.Y + (view.Page as DiagramPage).PxGridVerticalOffset - (topleft.Y % (view.Page as DiagramPage).PxGridVerticalOffset);
            return new Point(_left, _top);
        }
        internal Size m_height;
        internal Size m_oldSize = new Size(0, 0);
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            //Size actualSize = base.ArrangeOverride(arrangeSize);
            foreach (FrameworkElement ele in this.Children)
            {
                DiagramPage page = view.Page as DiagramPage;
                if (ele is ContentPresenter && page != null)
                {
                    ele.Arrange(new Rect(new Point(0,0), ele.DesiredSize));
                    continue;
                }
                if (view != null && view.Page != null)
                {
                   
                    if (!(ele is System.Windows.Shapes.Rectangle))
                    {
                        if (ele is DiagramViewGrid&&!dc.View.IsDragged)
                        {
                            Size view_Size = view.returnViewGridSize(_MasterParent);
                            Point viewpoint=ReturnViewGridSize(new Point(-page.Left,-page.Top));
                            ele.Arrange(new Rect(viewpoint,new Size(view_Size.Width + page.Left, view_Size.Height + page.Top)));
                        }
                    }
                    else
                    {
                        if (ele is Rectangle && view != null)
                        {
                            if (!view.SizeToContent)
                            {
                               // ele.Margin = new Thickness(view.BoundaryConstraintsArea.Left, view.BoundaryConstraintsArea.Top, 0, 0);
                                ele.Arrange(new Rect(view.BoundaryConstraintsArea.Left + view.PageMargin.Left , view.BoundaryConstraintsArea.Top + view.PageMargin.Top , view.BoundaryConstraintsArea.Right - view.BoundaryConstraintsArea.Left, view.BoundaryConstraintsArea.Bottom - view.BoundaryConstraintsArea.Top));
                            }
                            else
                            {
                                ele.Arrange(new Rect(new Point(-page.Left, -page.Top), new Size(page.DesiredSize.Width + page.Left ,page.DesiredSize.Height + page.Top)));
                            }
                        }
                        //if (view != null && !view.SizeToContent)
                        //{
                        //    ele.Arrange(new Rect(view.BoundaryConstraintsArea.Left - HorizontalOffset + (view.Page as DiagramPage).Left+view.PageMargin.Left, view.BoundaryConstraintsArea.Top - VerticalOffset + (view.Page as DiagramPage).Top+view.PageMargin.Top, view.BoundaryConstraintsArea.Right-view.PageMargin.Right, view.BoundaryConstraintsArea.Bottom-view.PageMargin.Bottom+VerticalOffset));
                        //}
                        //else
                        //{
                        //    ele.Arrange(new Rect(new Point(-HorizontalOffset, -VerticalOffset), new Size(ele.ActualWidth, ele.ActualHeight)));
                        //}
                    }
                }
            }
            //VerifyScrollData(ButIHave, INeed);
            // INeed = new Size(Math.Max(_MasterParent.ViewportWidth,_MasterParent.ExtentWidth),Math.Max(_MasterParent.ViewportHeight, _MasterParent.ExtentHeight));
            //if(!view.SizeToContent)
            // INeed = new Size(Math.Max(view.BoundaryConstraintsArea.Width, INeed.Width), Math.Max(view.BoundaryConstraintsArea.Height, INeed.Height));
            return new Size(Math.Abs(INeed.Width - (view.Page as DiagramPage).Left + m_height.Width),Math.Abs(INeed.Height - (view.Page as DiagramPage).Top+m_height.Height));
             // return new Size(INeed.Width-(view.Page as DiagramPage).Left,INeed.Height-(view.Page as DiagramPage).Top);;
        }
        
    }
}
