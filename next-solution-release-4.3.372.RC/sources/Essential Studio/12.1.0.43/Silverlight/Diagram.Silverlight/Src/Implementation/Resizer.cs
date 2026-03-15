#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Windows.Data;

    /// <summary>
    /// Represents the Resizer class which enables resizing of the node.
    /// </summary>
    public class Resizer : Control
    {
        private Thumb bottom = new Thumb();
        private Thumb bottomleft = new Thumb();
        private Thumb bottomright = new Thumb();
        private StackExt<object> m_DelayStack = new StackExt<object>();

        /// <summary>
        /// Used to store the DiagramControl instance.
        /// </summary>
        private DiagramControl dc;

        /// <summary>
        /// Refers to the <see cref="DiagramView"/> instance.
        /// </summary>
        private DiagramView dview;

        private Thumb left = new Thumb();

        /// <summary>
        /// Used to store the new size.
        /// </summary>
        private Size mnewsize;

        /// <summary>
        /// Used to store the current node.
        /// </summary>
        private Node node;

        private Thumb right = new Thumb();
        private Thumb top = new Thumb();
        private Thumb topleft = new Thumb();
        private Thumb topright = new Thumb();

        /// <summary>
        /// Initializes a new instance of the <see cref="Resizer"/> class.
        /// </summary>
        public Resizer()
        {
            this.DefaultStyleKey = typeof(Resizer);
        }

      internal Style LeftResizer
        {
            get { return (Style)GetValue(LeftResizerProperty); }
            set { SetValue(LeftResizerProperty, value); }
        }

      internal Style RightResizer
        {
            get { return (Style)GetValue(RightResizerProperty); }
            set { SetValue(RightResizerProperty, value); }
        }

      internal Style BottomResizer
        {
            get { return (Style)GetValue(BottomResizerProperty); }
            set { SetValue(BottomResizerProperty, value); }
        }

      internal Style TopResizer
        {
            get { return (Style)GetValue(TopResizerProperty); }
            set { SetValue(TopResizerProperty, value); }
        }

      internal Style TopLeftCornerResizer
        {
            get { return (Style)GetValue(TopLeftCornerResizerProperty); }
            set { SetValue(TopLeftCornerResizerProperty, value); }
        }

      internal Style TopRightCornerResizer
        {
            get { return (Style)GetValue(TopRightCornerResizerProperty); }
            set { SetValue(TopRightCornerResizerProperty, value); }
        }

      internal Style BottomLeftCornerResizer
        {
            get { return (Style)GetValue(BottomLeftCornerResizerProperty); }
            set { SetValue(BottomLeftCornerResizerProperty, value); }
        }

      internal Style BottomRightCornerResizer
        {
            get { return (Style)GetValue(BottomRightCornerResizerProperty); }
            set { SetValue(BottomRightCornerResizerProperty, value); }
        }


      internal static readonly DependencyProperty LeftResizerProperty = DependencyProperty.Register("LeftResizer", typeof(Style), typeof(Resizer), new PropertyMetadata(null));
      internal static readonly DependencyProperty RightResizerProperty = DependencyProperty.Register("RightResizer", typeof(Style), typeof(Resizer), new PropertyMetadata(null));
      internal static readonly DependencyProperty BottomResizerProperty = DependencyProperty.Register("BottomResizer", typeof(Style), typeof(Resizer), new PropertyMetadata(null));
      internal static readonly DependencyProperty TopResizerProperty = DependencyProperty.Register("TopResizer", typeof(Style), typeof(Resizer), new PropertyMetadata(null));
      internal static readonly DependencyProperty TopLeftCornerResizerProperty = DependencyProperty.Register("TopLeftCornerResizer", typeof(Style), typeof(Resizer), new PropertyMetadata(null));
      internal static readonly DependencyProperty TopRightCornerResizerProperty = DependencyProperty.Register("TopRightCornerResizer", typeof(Style), typeof(Resizer), new PropertyMetadata(null));
      internal static readonly DependencyProperty BottomLeftCornerResizerProperty = DependencyProperty.Register("BottomLeftCornerResizer", typeof(Style), typeof(Resizer), new PropertyMetadata(null));
      internal static readonly DependencyProperty BottomRightCornerResizerProperty = DependencyProperty.Register("BottomRightCornerResizer", typeof(Style), typeof(Resizer), new PropertyMetadata(null));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.node = this.DataContext as Node;
            this.dc = DiagramPage.GetDiagramControl(this.node);
            this.top = GetTemplateChild("PART_Top") as Thumb;
            this.bottom = GetTemplateChild("PART_Bottom") as Thumb;
            this.left = GetTemplateChild("PART_Left") as Thumb;
            this.right = GetTemplateChild("PART_Right") as Thumb;
            this.topleft = GetTemplateChild("PART_TopLeftCorner") as Thumb;
            this.topright = GetTemplateChild("PART_TopRightCorner") as Thumb;
            this.bottomleft = GetTemplateChild("PART_BottomLeftCorner") as Thumb;
            this.bottomright = GetTemplateChild("PART_BottomRightCorner") as Thumb;
            this.RegisterEvents(this.top);
            HandleRenderer.SetHorizontalProperty(top, System.Windows.HorizontalAlignment.Stretch);
            HandleRenderer.SetVerticalProperty(top, System.Windows.VerticalAlignment.Top);
            this.RegisterEvents(this.bottom);
            HandleRenderer.SetHorizontalProperty(bottom, System.Windows.HorizontalAlignment.Stretch);
            HandleRenderer.SetVerticalProperty(bottom, System.Windows.VerticalAlignment.Bottom);
            this.RegisterEvents(this.right);
            HandleRenderer.SetHorizontalProperty(right, System.Windows.HorizontalAlignment.Right);
            HandleRenderer.SetVerticalProperty(right, System.Windows.VerticalAlignment.Stretch);
            this.RegisterEvents(this.left);
            HandleRenderer.SetHorizontalProperty(left, System.Windows.HorizontalAlignment.Left);
            HandleRenderer.SetVerticalProperty(left, System.Windows.VerticalAlignment.Stretch);
            this.RegisterEvents(this.topleft);
            HandleRenderer.SetHorizontalProperty(topleft, System.Windows.HorizontalAlignment.Left);
            HandleRenderer.SetVerticalProperty(topleft, System.Windows.VerticalAlignment.Top);
            this.RegisterEvents(this.topright);
            HandleRenderer.SetHorizontalProperty(topright, System.Windows.HorizontalAlignment.Right);
            HandleRenderer.SetVerticalProperty(topright, System.Windows.VerticalAlignment.Top);
            this.RegisterEvents(this.bottomleft);
            HandleRenderer.SetHorizontalProperty(bottomleft, System.Windows.HorizontalAlignment.Left);
            HandleRenderer.SetVerticalProperty(bottomleft, System.Windows.VerticalAlignment.Bottom);
            this.RegisterEvents(this.bottomright);
            HandleRenderer.SetHorizontalProperty(bottomright, System.Windows.HorizontalAlignment.Right);
            HandleRenderer.SetVerticalProperty(bottomright, System.Windows.VerticalAlignment.Bottom);

            Node noderesizer = Resizer.FindParent<Node>(this);

            this.SetBinding(Resizer.TopResizerProperty, new Binding { Source = noderesizer, Path = new PropertyPath("TopResizer") });
            this.SetBinding(Resizer.LeftResizerProperty, new Binding("LeftResizer") { Source = noderesizer });
            this.SetBinding(Resizer.RightResizerProperty, new Binding("RightResizer") { Source = noderesizer });
            this.SetBinding(Resizer.BottomResizerProperty, new Binding("BottomResizer") { Source = noderesizer });
            this.SetBinding(Resizer.TopLeftCornerResizerProperty, new Binding("TopLeftCornerResizer") { Source = noderesizer });
            this.SetBinding(Resizer.TopRightCornerResizerProperty, new Binding("TopRightCornerResizer") { Source = noderesizer });
            this.SetBinding(Resizer.BottomLeftCornerResizerProperty, new Binding("BottomLeftCornerResizer") { Source = noderesizer });
            this.SetBinding(Resizer.BottomRightCornerResizerProperty, new Binding("BottomRightCornerResizer") { Source = noderesizer });
        }

        private static T FindParent<T>(UIElement control) where T : UIElement
        {
            UIElement p = VisualTreeHelper.GetParent(control) as UIElement;
            if (p != null)
            {
                if (p is T)
                    return p as T;
                else
                    return Resizer.FindParent<T>(p);
            }
            return null;
        }

        private void RegisterEvents(Thumb thumb)
        {
            thumb.LayoutUpdated += new EventHandler(this.ResizerThumb_LayoutUpdated);
            thumb.DragDelta += new DragDeltaEventHandler(this.Resizer_DragDelta);
            thumb.DragStarted += new DragStartedEventHandler(this.ResizerThumb_DragStarted);
            thumb.DragCompleted += new DragCompletedEventHandler(thumb_DragCompleted);
        }

        void thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
          
            if (!dc.View.IsResized)
            {
                NodeRoutedEventArgs newEventArgs = new NodeRoutedEventArgs(node);
                dview.OnNodeResized(node, newEventArgs);
				foreach (object o in m_DelayStack)
                {

                    if (o is string)
                    {
                        if (((string)o).Equals("Start"))
                        {
                            this.dview.tUndoStack.Push("Stop");
                        }
                        else if (((string)o).Equals("Stop"))
                        {
                            this.dview.tUndoStack.Push("Start");
                        }
                    }
                    else
                    {
                        this.dview.tUndoStack.Push(o);
                    }
                }
               
            }
            m_DelayStack.Clear();
            dc.View.IsResized = true;
        }

        /// <summary>
        /// Handles the DragDelta event of the Resizer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void Resizer_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.dc.View.IsPageEditable)
            {
                this.dc.View.IsResized = false;
                this.dc.View.IsMouseUponly = false;
                this.dview = Node.GetDiagramView(this.node);
                this.dview.Ispositionchanged = true;
                if (this.node.AllowResize)
                {
                    IDiagramPage mpage = VisualTreeHelper.GetParent(this.node) as IDiagramPage;
                    (mpage as DiagramPage).Istransformed = true;
                    NodeRoutedEventArgs newEventArgs = new NodeRoutedEventArgs(node);
                    dc.View.OnNodeResizing(node, newEventArgs);
                    DiagramView.IsOtherEvent = true;
                    if (this.node != null && mpage != null && this.node.IsSelected)
                    {
                        Node refNode = null;
                        this.node.m_MouseResizing = true;
                        double minLeft = 0, minTop = 0, minimumHorDelta = 0, minimumVertDelta = 0;

                        List<Node> selectedNodes = mpage.SelectionList.OfType<Node>().ToList();
                        IEnumerable<Group> tempNodes = mpage.SelectionList.OfType<Group>();
                        foreach (Group g in tempNodes)
                        {
                            selectedNodes.AddRange(g.NodeChildren.OfType<Node>());
                            selectedNodes.Remove(g);
                        }
                        Point delta = new Point(0, 0);
                        if (!(this.node is Group))
                        {
                            HandleRenderer.ComputeDragBoundaries(selectedNodes, out minLeft, out minTop, out minimumHorDelta, out minimumVertDelta);
                            DoDrag(this.node, mpage, sender, e, minLeft, minTop, minimumHorDelta, minimumVertDelta, new Point(0, 0));
                            delta = new Point(this.node.m_TempSize.Width - this.node.Width, this.node.m_TempSize.Height - this.node.Height);

                        }
                        else if(selectedNodes.Count>0)
                        {
                            HandleRenderer.ComputeDragBoundaries(selectedNodes, out minLeft, out minTop, out minimumHorDelta, out minimumVertDelta);
                            refNode = selectedNodes.First<Node>();
                            refNode.m_MouseResizing = true;
                            DoDrag(refNode, mpage, sender, e, minLeft, minTop, minimumHorDelta, minimumVertDelta, new Point(0, 0));
                            delta = new Point(refNode.m_TempSize.Width - refNode.Width, refNode.m_TempSize.Height - refNode.Height);
                        }
                        foreach (Node item in selectedNodes)
                        {
                            if (!item.m_MouseResizing)
                            {
                                DoDrag(item, mpage, sender, e, minLeft, minTop, minimumHorDelta, minimumVertDelta, delta);
                            }
                        }
                        this.node.UpdateLayout();
                        this.node.m_MouseResizing = false;
                        if (refNode != null)
                        {
                            refNode.m_MouseResizing = false;
                            refNode = null;
                        }
                    }

                    foreach (Node node1 in Selectnode)
                    {
                        foreach (ConnectionPort port in node1.Ports)
                        {
                            port.PxLeft = (port.Node.Width / port.mNodeoldSize.Width) * port.Oldpoint.X;
                            port.PxTop = (port.Node.Height / port.mNodeoldSize.Height) * port.Oldpoint.Y;
                        }
                    }
                }
            }
        }

        private void DoDrag(Node item, IDiagramPage mpage, object sender, DragDeltaEventArgs e, double minLeft, double minTop, double minimumHorDelta, double minimumVertDelta, Point delta)
        {
            double resizeVertDelta, resizeHorDelta, scale;

            if (!(mpage as DiagramPage).EnableResizingCurrentNodeOnMultipleSelection)
            {
                item.ResizeThisNode = true;
            }

            if (item != null && item.ParentID == Guid.Empty && item.ResizeThisNode)

            {
               
                switch (HandleRenderer.GetVerticalProperty(sender as DependencyObject))
                {
                    case System.Windows.VerticalAlignment.Bottom:
                        resizeVertDelta = Math.Min(-e.VerticalChange, minimumVertDelta);
                        scale = (item.m_TempSize.Height - resizeVertDelta) / item.m_TempSize.Height;
                        if (scale > 0)
                        {
                            HandleRenderer.ResizeBottom(scale, item, mpage, (mpage as DiagramPage).MeasurementUnits);
                            delta.X = item.Width > delta.X ? delta.X : 0;
                            delta.Y = item.Height > delta.Y ? delta.Y : 0;
                            item.Width -= delta.X;
                            if ((sender as Thumb).HorizontalAlignment == System.Windows.HorizontalAlignment.Stretch)
                            {
                                item.Height -= delta.Y;
                            }
                            this.mnewsize = new Size(item.Width, item.Height);
                        }
                        break;

                    case System.Windows.VerticalAlignment.Top:
                        double top = Canvas.GetTop(item);
                        
                        {
                            resizeVertDelta = Math.Min(Math.Max(-minTop, e.VerticalChange), minimumVertDelta);
                        }

                        scale = (item.m_TempSize.Height - resizeVertDelta) / item.m_TempSize.Height;
                        if (scale > 0)
                        {
                            HandleRenderer.ResizeTop(scale, item, mpage, (mpage as DiagramPage).MeasurementUnits);
                            delta.X = item.Width > delta.X ? delta.X : 0;
                            delta.Y = item.Height > delta.Y ? delta.Y : 0;
                            item.Width -= delta.X;
                            if ((sender as Thumb).HorizontalAlignment == System.Windows.HorizontalAlignment.Stretch)
                            {
                                item.Height -= delta.Y;
                            }
                            item.PxOffsetY += delta.Y;
                            this.mnewsize = new Size(item.Width, item.Height);
                        }
                        break;

                    default:
                        break;
                }

               
                switch (HandleRenderer.GetHorizontalProperty(sender as DependencyObject))
                {
                    case System.Windows.HorizontalAlignment.Left:
                        double left = Canvas.GetLeft(item);
                      
                        {
                            resizeHorDelta = Math.Min(Math.Max(-minLeft, e.HorizontalChange), minimumHorDelta);
                        }

                        scale = (item.m_TempSize.Width - resizeHorDelta) / item.m_TempSize.Width;
                        if (scale > 0)
                        {
                            HandleRenderer.ResizeLeft(scale, item, mpage, (mpage as DiagramPage).MeasurementUnits);
                            delta.X = item.Width > delta.X ? delta.X : 0;
                            delta.Y = item.Height > delta.Y ? delta.Y : 0;
                            item.Width -= delta.X;
                            item.Height -= delta.Y;
                            item.PxOffsetX += delta.X;
                            this.mnewsize = new Size(item.Width, item.Height);
                        }
                        break;

                    case System.Windows.HorizontalAlignment.Right:
                        resizeHorDelta = Math.Min(-e.HorizontalChange, minimumHorDelta);
                        scale = (item.m_TempSize.Width - resizeHorDelta) / item.m_TempSize.Width;
                        if (scale > 0)
                        {
                            HandleRenderer.ResizeRight(scale, item, mpage, (mpage as DiagramPage).MeasurementUnits);
                            delta.X = item.Width > delta.X ? delta.X : 0;
                            delta.Y = item.Height > delta.Y ? delta.Y : 0;
                            item.Width -= delta.X;
                            item.Height -= delta.Y;
                            this.mnewsize = new Size(item.Width, item.Height);
                        }
                        break;

                    default:
                        break;
                }
            }
        }
        List<Node> Selectnode=new List<Node>();
        /// <summary>
        /// Handles the DragStarted event of the ResizerThumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragStartedEventArgs"/> instance containing the event data.</param>
        private void ResizerThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
      
            dc.View.IsResized = true;
            this.dview = Node.GetDiagramView(this.node);
            foreach(Node n in dc.Model.Nodes)
            {
                n.m_TempSize = new Size(n.ActualWidth, n.ActualHeight);
                //n.Oldsize = new Size(n.ActualWidth, n.ActualHeight);
                n.m_TempPosition = new Point(n.PxOffsetX,n.PxOffsetY);
            }
            Selectnode.Clear();
            IDiagramPage mpage = VisualTreeHelper.GetParent(this.node) as IDiagramPage;
            IEnumerable<Node> selectedNodes = mpage.SelectionList.OfType<Node>();
            foreach (Node node in selectedNodes)
            {
                Selectnode.Add(node);
                foreach (ConnectionPort port in node.Ports)
                {
                    port.Oldpoint = new Point(port.PxLeft, port.PxTop);
                    port.mNodeoldSize = new Size(port.Node._Width, port.Node._Height);
                    port.oldCenterPosition = port.PxCenterPosition;
                }
            }
            if (selectedNodes.Count() > 1)
            {
                m_DelayStack.Push("Stop");
            }
            else if (selectedNodes.Count() == 1 && selectedNodes.First() is Group)
            {
                m_DelayStack.Push("Stop");
            }
            foreach (Node item in selectedNodes)
            {
                if (item is Group)
                {
                    foreach (Node n in (item as Group).NodeChildren.OfType<Node>())
                    {
                        m_DelayStack.Push(new NodeOperation(NodeOperations.Resized, n));
                    }
                }
                else
                {
                    m_DelayStack.Push(new NodeOperation(NodeOperations.Resized, item));
                }
            }
            if (selectedNodes.Count() > 1)
            {
                m_DelayStack.Push("Start");
            }
            else if (selectedNodes.Count() == 1 && selectedNodes.First() is Group)
            {
                m_DelayStack.Push("Start");
            }
        }

        /// <summary>
        /// Handles the LayoutUpdated event of the ResizerThumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ResizerThumb_LayoutUpdated(object sender, EventArgs e)
        {
        }
             
        #region Class Variables

        #endregion

        #region Initialization

        #endregion

        #region Implementation

        #endregion
    }
}
