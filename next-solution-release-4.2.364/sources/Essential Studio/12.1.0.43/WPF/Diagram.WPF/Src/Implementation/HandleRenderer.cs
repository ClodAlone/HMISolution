// <copyright file="HandleRenderer.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents Resize helper class which helps in the resizing operation.
    /// </summary>
    internal class HandleRenderer
    {
        #region Class fields

        /// <summary>
        /// Used to store the handle color.
        /// </summary>
        private static Brush m_handleColor = Brushes.GreenYellow;

        /// <summary>
        /// Used to store the connector type.
        /// </summary>
        private static ConnectorType m_connectionType = ConnectorType.Orthogonal;

        /// <summary>
        ///  Used to store the disabled handle color.
        /// </summary>
        private static Brush m_handleDisabledColor = Brushes.Gray;

        /// <summary>
        ///  Used to store the handle outline color.
        /// </summary>
        private static Brush m_handleOutlineColor = Brushes.Black;

        /// <summary>
        ///  Used to store the handle brush color.
        /// </summary>
        private static Brush m_sbrushHandle = Brushes.GreenYellow;

        /// <summary>
        ///  Used to store the handle pen..
        /// </summary>
        private static Pen m_spenHandleOutline = new Pen(m_handleOutlineColor, 0f);

        #endregion

        #region Class properties

        /// <summary>
        /// Gets or sets the  color to be used to fill the  Handle.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Color to fill the Handle with.
        /// </value>
        internal static Brush HandleColor
        {
            get
            {
                return m_handleColor;
            }

            set
            {
                if (value != m_handleColor)
                {
                    m_handleColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the  color to be used to fill the  Handle Border.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Color to fill the Handle Border with.
        /// </value>
        internal static Brush HandleOutlineColor
        {
            get
            {
                return m_handleOutlineColor;
            }

            set
            {
                if (value != m_handleOutlineColor)
                {
                    m_handleOutlineColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the  color to be used to fill the  Handle when it is in disabled state.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Color to fill the Handle with.
        /// </value>
        internal static Brush HandleDisabledColor
        {
            get
            {
                return m_handleDisabledColor;
            }

            set
            {
                if (value != m_handleDisabledColor)
                {
                    m_handleDisabledColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of connection to be used.
        /// </summary>
        /// <value>
        /// Type: <see cref="ConnectorType"/>
        /// Enum specifying the type of the connector to be used.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set ConnectorType in C#.
        /// <code language="C#">
        ///  connObject.ConnectorType = ConnectorType.Orthogonal;
        /// </code>
        /// </example>
        internal static ConnectorType ConnectorType
        {
            get
            {
                return m_connectionType;
            }

            set
            {
                if (value != m_connectionType)
                {
                    m_connectionType = value;
                }
            }
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Calculates the drag limits.
        /// </summary>
        /// <param name="selectedItems">IEnumerable Collection instance.</param>
        /// <param name="minoffx">The minoffx.</param>
        /// <param name="minoffy">The minoffy.</param>
        /// <param name="minimumHorDelta">The minimum horizontal delta.</param>
        /// <param name="minimumVertDelta">The minimum vertical delta.</param>
        internal static void ComputeDragBoundaries(IEnumerable<Node> selectedItems, out double minoffx, out double minoffy, out double minimumHorDelta, out double minimumVertDelta)
        {
            minoffx = double.MaxValue;
            minoffy = double.MaxValue;
            minimumHorDelta = double.MaxValue;
            minimumVertDelta = double.MaxValue;

            foreach (Node item in selectedItems)
            {
                double offx = item.PxOffsetX;
                double offy = item.PxOffsetY;

                minoffx = double.IsNaN(offx) ? 0 : Math.Min(offx, minoffx);
                minoffy = double.IsNaN(offy) ? 0 : Math.Min(offy, minoffy);
                minimumVertDelta = Math.Min(minimumVertDelta, item.ActualHeight - item.MinHeight);
                minimumHorDelta = Math.Min(minimumHorDelta, item.ActualWidth - item.MinWidth);
            }
        }

        /// <summary>
        /// Computes the drag boundaries.
        /// </summary>
        /// <param name="gnode">The group node.</param>
        /// <param name="minoffx">The minoffx.</param>
        /// <param name="minoffy">The minoffy.</param>
        /// <param name="minimumHorDelta">The minimum hor delta.</param>
        /// <param name="minimumVertDelta">The minimum vert delta.</param>
        internal static void ComputeDragBoundaries(Group gnode, out double minoffx, out double minoffy, out double minimumHorDelta, out double minimumVertDelta)
        {
            minoffx = double.MaxValue;
            minoffy = double.MaxValue;
            minimumHorDelta = double.MaxValue;
            minimumVertDelta = double.MaxValue;
            CollectionExt gnodechildren = new CollectionExt();
            for (int i = 0; i < gnode.NodeChildren.Count; i++)
            {
                if (gnode.NodeChildren[i] is Node)
                {
                    gnodechildren.Add(gnode.NodeChildren[i] as Node);
                }
            }

            gnodechildren.Add(gnode);
            foreach (Node item in gnodechildren)
            {
                double offx = item.PxOffsetX;
                double offy = item.PxOffsetY;

                minoffx = double.IsNaN(offx) ? 0 : Math.Min(offx, minoffx);
                minoffy = double.IsNaN(offy) ? 0 : Math.Min(offy, minoffy);
                minimumVertDelta = Math.Min(minimumVertDelta, item.ActualHeight - item.MinHeight);
                minimumHorDelta = Math.Min(minimumHorDelta, item.ActualWidth - item.MinWidth);
            }
        }

        /// <summary>
        /// Resizes the left side of the node.
        /// </summary>
        /// <param name="scale">Scale factor.</param>
        /// <param name="item">Node instance.</param>
        /// <param name="designer">IDiagramPage instance.</param>
        /// <param name="m_units">The measure units.</param>
        internal static void ResizeLeft(double scale, Node item, IDiagramPage designer, MeasureUnits m_units)
        {
            if (item.m_TempSize.Width == 0)
            {
                item.m_TempSize.Width = Math.Abs(item.m_ResizeHorDelta);
            }
            else
            {
                double delta = item.m_TempSize.Width * (scale - 1);
                //item.m_TempPosition.X = item.m_TempPosition.X - delta;
                //if (item.RotateAngle != 0 || item.RotateAngle != 360)
                {
                    Matrix mat = Matrix.Identity;
                    mat.Rotate((item.RenderTransform as RotateTransform).Angle);
                    MatrixTransform trans = new MatrixTransform(mat);
                    Point pt = trans.Transform(new Point(-delta, 0));
                    item.m_TempPosition.X += pt.X;
                    item.m_TempPosition.Y += pt.Y;
                }
                item.m_TempSize.Width = item.m_TempSize.Width * scale;
            }
            item.SnapWidth(true);
            item.SnapHeight(true);
        }

        /// <summary>
        /// Resizes the top side of the node.
        /// </summary>
        /// <param name="scale">Scale factor.</param>
        /// <param name="item">Node instance.</param>
        /// <param name="designer">IDiagramPage instance.</param>
        /// <param name="m_units">The measure units..</param>
        internal static void ResizeTop(double scale, Node item, IDiagramPage designer, MeasureUnits m_units)
        {
            if (item.m_TempSize.Height == 0)
            {
                item.m_TempSize.Height = Math.Abs(item.m_ResizeVertDelta);
            }
            else
            {
                double delta = item.m_TempSize.Height * (scale - 1);
                //item.m_TempPosition.Y = item.m_TempPosition.Y - delta;

                //if (item.RotateAngle != 0 || item.RotateAngle != 360)
                {
                    Matrix mat = Matrix.Identity;
                    mat.Rotate((item.RenderTransform as RotateTransform).Angle);
                    MatrixTransform trans = new MatrixTransform(mat);
                    Point pt = trans.Transform(new Point(0, -delta));
                    item.m_TempPosition.X += pt.X;
                    item.m_TempPosition.Y += pt.Y;
                }
                item.m_TempSize.Height = item.m_TempSize.Height * scale;
            }
            item.SnapWidth(true);
            item.SnapHeight(true);
        }

        /// <summary>
        /// Resizes the right side of the node.
        /// </summary>
        /// <param name="scale">Scale factor.</param>
        /// <param name="item">Node instance.</param>
        /// <param name="designer">IDiagramPage instance.</param>
        /// <param name="m_units">The measure units..</param>
        internal static void ResizeRight(double scale, Node item, IDiagramPage designer, MeasureUnits m_units)
        {
            //IEnumerable<Node> items = designer.SelectionList.GetGroupList(item).Cast<Node>();

            //double left = item.LogicalOffsetX;

            //double tempLeft = item.LogicalOffsetX;
            //double delta = (tempLeft - left) * (scale - 1);

            //item.LogicalOffsetX = tempLeft + delta;

            //item.Width = item.ActualWidth * scale;
            if (item.m_TempSize.Width == 0)
            {
                item.m_TempSize.Width = Math.Abs(item.m_ResizeHorDelta);
            }
            else
                item.m_TempSize.Width = item.m_TempSize.Width * scale;
            item.SnapWidth(false);
        }

        /// <summary>
        /// Resizes the bottom side of the node.
        /// </summary>
        /// <param name="scale">Scale factor.</param>
        /// <param name="item">Node instance.</param>
        /// <param name="designer">IDiagramPage instance.</param>
        /// <param name="m_units">The measure units..</param>
        internal static void ResizeBottom(double scale, Node item, IDiagramPage designer, MeasureUnits m_units)
        {
            //IEnumerable<Node> items = designer.SelectionList.GetGroupList(item).Cast<Node>();
            //double top = item.LogicalOffsetY;

            //double tempTop = item.LogicalOffsetY;
            //double delta = (tempTop - top) * (scale - 1);

            //item.LogicalOffsetY = tempTop + delta;
            //item.Height = item.ActualHeight * scale;
            if (item.m_TempSize.Height == 0)
            {
                item.m_TempSize.Height = Math.Abs(item.m_ResizeVertDelta);
            }
            else
                item.m_TempSize.Height = item.m_TempSize.Height * scale;
            item.SnapHeight(false);
        }

        #endregion
    }

    /// <summary>
    /// Represents Node Resize Thumb .
    /// </summary>
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
    public class ResizerThumb : Thumb
    {
        #region Class Variables

        /// <summary>
        /// Used to store the new size.
        /// </summary>
        private Size m_newsize;

        /// <summary>
        /// Used to store the current node.
        /// </summary>
        private Node node;

        /// <summary>
        /// Used to store the DiagramControl instance.
        /// </summary>
        private DiagramControl dc;
        
        /// <summary>
        /// Refers to the <see cref="DiagramView"/> instance.
        /// </summary>
        private DiagramView dview;

        internal VerticalAlignment ResizerVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(ResizerVerticalAlignmentProperty); }
            set { SetValue(ResizerVerticalAlignmentProperty, value); }
        }

        internal HorizontalAlignment ResizerHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(ResizerHorizontalAlignmentProperty); }
            set { SetValue(ResizerHorizontalAlignmentProperty, value); }
        }

        internal  static readonly DependencyProperty ResizerVerticalAlignmentProperty = DependencyProperty.Register("ResizerVerticalAlignment", typeof(VerticalAlignment), typeof(ResizerThumb), new PropertyMetadata(null));

        internal   static readonly DependencyProperty ResizerHorizontalAlignmentProperty = DependencyProperty.Register("ResizerHorizontalAlignment", typeof(HorizontalAlignment), typeof(ResizerThumb), new PropertyMetadata(null));  
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="ResizerThumb"/> class.
        /// </summary>
        public ResizerThumb()
        {
            DragStarted += new DragStartedEventHandler(ResizerThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(Resizer_DragDelta);
            DragCompleted += new DragCompletedEventHandler(ResizerThumb_DragCompleted);
        }

        ///// <summary>
        ///// Handles the LayoutUpdated event of the ResizerThumb control.
        ///// </summary>
        ///// <param name="sender">The source of the event.</param>
        ///// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        //private void ResizerThumb_LayoutUpdated(object sender, EventArgs e)
        //{
        //    if (node.AllowResize)
        //    {
        //        if (this.ResizerHorizontalAlignment == HorizontalAlignment.Left)
        //        {
        //            if (this.ResizerVerticalAlignment == VerticalAlignment.Top)
        //            {
        //                this.Cursor = Cursors.SizeNWSE;
        //            }
        //            else if (this.ResizerVerticalAlignment == VerticalAlignment.Bottom)
        //            {
        //                this.Cursor = Cursors.SizeNESW;
        //            }
        //            else
        //            {
        //                this.Cursor = Cursors.SizeWE;
        //            }
        //        }
        //        else
        //            if (this.ResizerHorizontalAlignment == HorizontalAlignment.Right)
        //            {
        //                if (this.ResizerVerticalAlignment == VerticalAlignment.Top)
        //                {
        //                    this.Cursor = Cursors.SizeNESW;
        //                }
        //                else if (this.ResizerVerticalAlignment == VerticalAlignment.Bottom)
        //                {
        //                    this.Cursor = Cursors.SizeNWSE;
        //                }
        //                else
        //                {
        //                    this.Cursor = Cursors.SizeWE;
        //                }
        //            }
        //            else
        //                if (this.ResizerHorizontalAlignment == HorizontalAlignment.Stretch)
        //                {
        //                    if (this.ResizerVerticalAlignment == VerticalAlignment.Top)
        //                    {
        //                        this.Cursor = Cursors.SizeNS;
        //                    }
        //                    else if (this.ResizerVerticalAlignment == VerticalAlignment.Bottom)
        //                    {
        //                        this.Cursor = Cursors.SizeNS;
        //                    }
        //                }
        //    }
        //    else
        //    {
        //        this.Cursor = Cursors.Arrow;
        //    }
        //}

        #endregion

        #region Overrides

        /// <summary>
        /// Raises the <see cref="OnInitialized"/> event. 
        /// This method is invoked whenever <see cref="OnInitialized"/> property is set to true internally. 
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            try
            {
                dc = DiagramPage.GetDiagramControl((FrameworkElement)this);
                base.OnInitialized(e);
                node = this.DataContext as Node;
                foreach (ConnectionPort port in node.Ports)
                {
                    port.PreviousPortPoint = new Point(node.Width / 2, node.Height / 2);
                }
            }
            catch
            {
            }
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Handles the DragStarted event of the ResizerThumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragStartedEventArgs"/> instance containing the event data.</param>
        private void ResizerThumb_DragStarted(object sender, DragStartedEventArgs e)
        {

            dc.View.DragDelta = "Yes";
            dview = Node.GetDiagramView(node);
            if (!(dview.undo || dview.redo))
            {
                dview.RedoStack.Clear();//
            }

            if (node is Group)
            {
                foreach (INodeGroup nodechild in (node as Group).NodeChildren)
                {
                    if (nodechild is Node)
                    {
                        foreach (ConnectionPort port in (nodechild as Node).Ports)
                        {
                            port.PreviousPortPoint = new Point(port.Left, port.Top);
                        }

                        (nodechild as Node).Oldsize = new Size((nodechild as Node).Width, (nodechild as Node).Height);
                    }
                }
            }

            IDiagramPage mPage = VisualTreeHelper.GetParent(node) as IDiagramPage;

            IEnumerable<Node> selectedNodes = mPage.SelectionList.OfType<Node>();
            if (dc != null && dc.Model != null)
            {
                pre = Mouse.GetPosition(dc.View.Page);
                foreach (Node n in dc.Model.Nodes)
                {
                    n.m_TempPosition = new Point(n.PxOffsetX, n.PxOffsetY);
                    n.m_TempSize = new Size(n.Width, n.Height);
                }
            }

            foreach (Node item in selectedNodes)
            {
                foreach (ConnectionPort port in item.Ports)
                {
                    port.PreviousPortPoint = new Point(port.Left, port.Top);
                    if (dview != null && dview.UndoRedoEnabled && !(item is Group) && selectedNodes.Count() > 0)
                    {
                        dview.UndoStack.Push(port);
                        dview.UndoStack.Push(port.PreviousPortPoint);
                    }
                }

                item.Oldsize = new Size(item.Width, item.Height);

                if ((dc.View.Page as DiagramPage).EnableResizingCurrentNodeOnMultipleSelection)
                {
                    node.ResizeThisNode = true;

                    foreach (Group g in node.Groups)
                    {
                        g.ResizeThisNode = true;
                    }

                    if (item != node && !(item is Group))
                    {
                        item.ResizeThisNode = false;
                    }
                }

                Point oldposition = new Point(item.PxOffsetX, item.PxOffsetY);// new Point(MeasureUnitsConverter.ToPixels(item.LogicalOffsetX, (dc.View.Page as DiagramPage).MeasurementUnits), MeasureUnitsConverter.ToPixels(item.LogicalOffsetY, (dc.View.Page as DiagramPage).MeasurementUnits));
                if (dview != null && dview.UndoRedoEnabled && !(item is Group) && selectedNodes.Count() > 0)
                {
                    dview.UndoStack.Push(dview.DragDelta.ToString());
                    dview.UndoStack.Push(item);
                    dview.UndoStack.Push(item.Oldsize);
                    dview.UndoStack.Push(oldposition);
                    dview.UndoStack.Push(selectedNodes.Count());
                    dview.UndoStack.Push("Resized");
                    dview.DragDelta = "No";
                }
            }
        }

        Point current;
        Point pre;
        double horChanage;
        double verChanage;

        /// <summary>
        /// Handles the DragDelta event of the Resizer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void Resizer_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (dc.View.IsPageEditable)
            {
                dc.View.IsResized = true;
                dview = Node.GetDiagramView(node);
                dview.Ispositionchanged = true;
                if (node.AllowResize)
                {
                    if (node.Content is Viewbox)
                    {
                        (node.Content as Viewbox).Width = node.Width;
                        (node.Content as Viewbox).Height = node.Height;
                    }

                    NodeRoutedEventArgs newEventArgs = new NodeRoutedEventArgs(node);
                    newEventArgs.RoutedEvent = DiagramView.NodeResizingEvent;
                    RaiseEvent(newEventArgs);
                    DiagramView.IsOtherEvent = true;

                    Rotator rotator = (Rotator)node.Template.FindName("PART_Rotator", node);

                    IDiagramPage mPage = VisualTreeHelper.GetParent(node) as IDiagramPage;

                    if (node != null && mPage != null && node.IsSelected)
                    {
                        current = Mouse.GetPosition(mPage as DiagramPage);
                        Matrix mat = new Matrix();
                        //node.RotateAngle = 0;
                        mat.RotateAt(-(node.RenderTransform as RotateTransform).Angle, this.ActualWidth / 2, this.ActualHeight / 2);
                        MatrixTransform trans = new MatrixTransform(mat);
                        Point start = trans.Transform(current);
                        Point end = trans.Transform(pre);
                        horChanage = start.X - end.X;
                        verChanage = start.Y - end.Y;
                        pre = current;
                        double minLeft, minTop, minimumHorDelta, minimumVertDelta;

                        Point delta = new Point(0, 0);
                        this.node.m_MouseResizing = true;

                        IEnumerable<Node> selectedNodes = mPage.SelectionList.OfType<Node>();
                        if (node is Group)
                        {
                            HandleRenderer.ComputeDragBoundaries(node as Group, out minLeft, out minTop, out minimumHorDelta, out minimumVertDelta);
                            fun(mPage, this.node, sender, e, minimumVertDelta, minimumHorDelta, minTop, minLeft, new Point(0, 0));
                            delta = new Point(this.node.m_TempSize.Width - this.node.Width, this.node.m_TempSize.Height - this.node.Height);
                        }
                        else
                        {
                            HandleRenderer.ComputeDragBoundaries(selectedNodes, out minLeft, out minTop, out minimumHorDelta, out minimumVertDelta);
                            fun(mPage, this.node, sender, e, minimumVertDelta, minimumHorDelta, minTop, minLeft, new Point(0, 0));
                            delta = new Point(this.node.m_TempSize.Width - this.node.Width, this.node.m_TempSize.Height - this.node.Height);

                            if (this.node.ProportionalResize)
                            {
                                if (this.ResizerHorizontalAlignment == HorizontalAlignment.Stretch || this.ResizerVerticalAlignment == VerticalAlignment.Stretch)
                                {
                                    if (this.ResizerVerticalAlignment == VerticalAlignment.Stretch && (this.ResizerHorizontalAlignment == HorizontalAlignment.Left || this.ResizerHorizontalAlignment == HorizontalAlignment.Right))
                                    {
                                        if (this.node.Width != this.node.Oldsize.Width)
                                        {
                                            if (this.ResizerHorizontalAlignment == HorizontalAlignment.Right)
                                            {
                                                if ((this.node.Height + this.horChanage) >= 0)
                                                {
                                                    this.node.m_TempSize.Height = this.node.Height + this.horChanage;
                                                    this.node.SnapHeight(false);
                                                }
                                            }
                                            else if (this.ResizerHorizontalAlignment == HorizontalAlignment.Left)
                                            {
                                                if (Mouse.GetPosition(dview).X > 0)
                                                {
                                                    this.node.m_TempSize.Height = this.node.Height - this.horChanage;
                                                    this.node.SnapHeight(false);
                                                }
                                                else
                                                {
                                                    if ((this.node.m_TempPosition.X - this.horChanage) <= 0)
                                                    {
                                                        this.node.m_TempSize.Height = this.node.Height - this.horChanage;
                                                        this.node.SnapHeight(false);
                                                    }
                                                    else
                                                    {
                                                        string str = (this.node.m_TempPosition.X - this.horChanage).ToString();
                                                        if (!str.Contains('E'))
                                                        {
                                                            this.node.m_TempPosition.X = this.node.m_TempPosition.X - this.horChanage;
                                                            this.node.m_TempSize.Width = this.node.Width + this.horChanage;
                                                            this.node.SnapWidth(true);
                                                        }
                                                        else if ((this.node.m_TempPosition.X - this.horChanage) > 0)
                                                        {
                                                            this.node.m_TempSize.Height = this.node.Height - this.horChanage;
                                                            this.node.SnapHeight(false);
                                                        }
                                                    }
                                                }

                                                
                                            }
                                        }
                                    }
                                    else if (this.ResizerHorizontalAlignment == HorizontalAlignment.Stretch && (this.ResizerVerticalAlignment == VerticalAlignment.Top || this.ResizerVerticalAlignment == VerticalAlignment.Bottom))
                                    {
                                        if (this.node.Height != this.node.Oldsize.Height)
                                        {
                                            if (this.ResizerVerticalAlignment == VerticalAlignment.Bottom)
                                            {
                                                if ((this.node.Width + this.verChanage) >= 0)
                                                {
                                                    double x = (this.node.m_TempSize.Height - this.node.Oldsize.Height) / this.node.Oldsize.Height;
                                                    this.node.m_TempSize.Width = this.node.Oldsize.Width + (this.node.Oldsize.Width * x);
                                                    this.node.SnapWidth(false);
                                                }
                                            }
                                            else if (this.ResizerVerticalAlignment == VerticalAlignment.Top)
                                            {
                                                if (Mouse.GetPosition(dview).Y > 0)
                                                {
                                                    this.node.m_TempSize.Width = this.node.Width - this.verChanage;
                                                    this.node.SnapWidth(false);
                                                }
                                                else
                                                {
                                                    if ((this.node.m_TempPosition.Y - this.verChanage) <= 0)
                                                    {
                                                        this.node.m_TempSize.Width = this.node.m_TempSize.Width - this.verChanage;
                                                        this.node.SnapWidth(false);
                                                    }
                                                    else
                                                    {
                                                        string str = (this.node.m_TempPosition.Y - this.verChanage).ToString();
                                                        if (!str.Contains('E'))
                                                        {
                                                            this.node.m_TempPosition.Y = this.node.m_TempPosition.Y - this.verChanage;
                                                            this.node.m_TempSize.Height = this.node.Height + this.verChanage;
                                                            this.node.SnapHeight(true);
                                                        }
                                                        else
                                                        {
                                                            this.node.m_TempSize.Width = this.node.m_TempSize.Width - this.verChanage;
                                                            this.node.SnapWidth(false);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (this.ResizerHorizontalAlignment != HorizontalAlignment.Stretch && this.ResizerVerticalAlignment != VerticalAlignment.Stretch)
                                {
                                    if (this.ResizerVerticalAlignment == VerticalAlignment.Bottom && this.ResizerHorizontalAlignment == HorizontalAlignment.Right)
                                    {
                                            if ((this.node.Width + this.verChanage) >= 0)
                                            {
                                                    if (Mouse.GetPosition(dview).Y >= (this.node.m_TempPosition.Y + this.node.Height) && (Mouse.GetPosition(dview).X <= (this.node.m_TempPosition.X + this.node.Width + this.Width)) && Mouse.GetPosition(dview).Y > this.node.m_TempPosition.Y && (Mouse.GetPosition(dview).X >= (this.node.m_TempPosition.X)))
                                                    {
                                                        this.node.m_TempSize.Width = this.node.Width + this.verChanage;
                                                         this.node.SnapWidth(false);
                                                    }
                                                    else if ((Mouse.GetPosition(dview).Y < (this.node.m_TempPosition.Y + this.node.Height)) && Mouse.GetPosition(dview).Y > this.node.m_TempPosition.Y && Mouse.GetPosition(dview).X > this.node.m_TempPosition.X)
                                                    {
                                                        this.node.m_TempSize.Height = this.node.Height - this.verChanage;
                                                        this.node.SnapHeight(false);
                                                    }
                                                    else if ((Mouse.GetPosition(dview).X > (this.node.m_TempPosition.X + this.node.Width + this.Width)) && (Mouse.GetPosition(dview).Y > this.node.m_TempPosition.Y))
                                                    {
                                                        if ((this.node.Height - this.verChanage) >= 0)
                                                        {
                                                            if (this.verChanage < 0)
                                                            {
                                                                this.node.m_TempSize.Height = this.node.Height - this.verChanage;
                                                                this.node.SnapHeight(false);
                                                            }
                                                            else
                                                            {
                                                                this.node.m_TempSize.Width = this.node.Width + this.verChanage;
                                                                this.node.SnapWidth(false);
                                                            }
                                                        }
                                                    }
                                            }

                                            if (this.horChanage != 0)
                                            {
                                                if ((this.node.Height + (int)this.horChanage) >= 0)
                                                {
                                                        if (Mouse.GetPosition(dview).X >= (this.node.m_TempPosition.X + this.node.Width) && ((Mouse.GetPosition(dview).Y <= (this.node.m_TempPosition.Y + this.node.Height + this.Height))) && Mouse.GetPosition(dview).X > this.node.m_TempPosition.X && Mouse.GetPosition(dview).Y > this.node.m_TempPosition.Y)
                                                        {
                                                            this.node.m_TempSize.Height = this.node.Height + this.horChanage;
                                                            this.node.SnapHeight(false);
                                                        }
                                                        else if ((Mouse.GetPosition(dview).X <= (this.node.m_TempPosition.X + this.node.Width)) && Mouse.GetPosition(dview).X > this.node.m_TempPosition.X && Mouse.GetPosition(dview).Y > this.node.m_TempPosition.Y)
                                                        {
                                                            this.node.m_TempSize.Width = this.node.Width - this.horChanage;
                                                            this.node.SnapWidth(false);
                                                        }
                                                        else if ((Mouse.GetPosition(dview).Y > (this.node.m_TempPosition.Y + this.node.Height + this.Height)) && (Mouse.GetPosition(dview).X > this.node.m_TempPosition.X))
                                                        {
                                                            this.node.m_TempSize.Width = this.node.Width - this.horChanage;
                                                            this.node.SnapWidth(false);
                                                        }
                                                }
                                            }
                                    }
                                    else if (this.ResizerVerticalAlignment == VerticalAlignment.Top && this.ResizerHorizontalAlignment == HorizontalAlignment.Right)
                                    {
                                            if (this.verChanage != 0)
                                            {
                                                if ((this.node.Width + this.verChanage) >= 0)
                                                {
                                                    if (Mouse.GetPosition(dview).Y>0 && Mouse.GetPosition(dview).Y <= (this.node.m_TempPosition.Y) && Mouse.GetPosition(dview).X > this.node.m_TempPosition.X && Mouse.GetPosition(dview).X <= (this.node.m_TempPosition.X + this.node.Width + this.Width))
                                                    {
                                                        this.node.m_TempSize.Width = this.node.Width - this.verChanage;
                                                        this.node.SnapWidth(false);
                                                    }
                                                    else if ((Mouse.GetPosition(dview).Y > 0 && Mouse.GetPosition(dview).Y > (this.node.m_TempPosition.Y)) && Mouse.GetPosition(dview).X > this.node.m_TempPosition.X)
                                                    {
                                                         this.node.m_TempPosition.Y = this.node.m_TempPosition.Y - this.verChanage;
                                                         this.node.m_TempSize.Height = this.node.Height + this.verChanage;
                                                         this.node.SnapHeight(true);
                                                    }
                                                    else if (Mouse.GetPosition(dview).Y > 0 && Mouse.GetPosition(dview).X > (this.node.m_TempPosition.X + this.node.m_TempSize.Width + this.Width))
                                                    {
                                                        if (this.node.m_TempSize.Height - this.verChanage > 0)
                                                        {
                                                            if (this.verChanage > 0)
                                                            {
                                                                this.node.m_TempPosition.Y = this.node.m_TempPosition.Y - this.verChanage;
                                                                this.node.m_TempSize.Height = this.node.m_TempSize.Height + this.verChanage;
                                                                this.node.SnapHeight(true);
                                                            }
                                                            else if (this.verChanage < 0)
                                                            {
                                                                this.node.m_TempSize.Width = this.node.Width - this.verChanage;
                                                                this.node.SnapWidth(false);
                                                            }
                                                        }
                                                    }
                                                    else if (Mouse.GetPosition(dview).Y <= 0)
                                                    {
                                                         this.node.m_TempPosition.Y = this.node.m_TempPosition.Y - this.verChanage;
                                                         this.node.m_TempSize.Height = this.node.m_TempSize.Height + this.verChanage;
                                                         this.node.SnapHeight(true);
                                                    }
                                                }
                                            }
                                            if (this.horChanage != 0)
                                            {
                                                if (Mouse.GetPosition(dview).X >= (this.node.m_TempPosition.X + this.node.Width + this.Width) && (Mouse.GetPosition(dview).X > this.node.m_TempPosition.X) && (Mouse.GetPosition(dview).Y <= this.node.m_TempPosition.Y))
                                                {
                                                    if (this.node.m_TempPosition.Y > 0 && Mouse.GetPosition(dview).Y >= this.node.m_TempPosition.Y - this.Height && Mouse.GetPosition(dview).Y <= this.node.m_TempPosition.Y)
                                                    {
                                                        this.node.m_TempPosition.Y = this.node.m_TempPosition.Y - this.horChanage;
                                                        this.node.m_TempSize.Height = this.node.Height + this.horChanage;
                                                        this.node.SnapHeight(true);
                                                    }
                                                    else if (Mouse.GetPosition(dview).Y <= this.node.m_TempPosition.Y - this.Height)
                                                    {
                                                        this.node.m_TempSize.Width = this.node.m_TempSize.Width - this.horChanage;
                                                        this.node.SnapWidth(false);
                                                    }
                                                }
                                                else if ((Mouse.GetPosition(dview).X >= (this.node.m_TempPosition.X + this.node.Width + this.Width) && (Mouse.GetPosition(dview).X > this.node.m_TempPosition.X) && (Mouse.GetPosition(dview).Y > this.node.m_TempPosition.Y)))
                                                {
                                                    if ((this.node.m_TempPosition.Y - this.horChanage)<=0)
                                                    {
                                                        this.node.m_TempSize.Width = this.node.m_TempSize.Width - this.horChanage;
                                                        this.node.SnapWidth(false);
                                                    }
                                                    else
                                                    {
                                                        string str = (this.node.m_TempPosition.Y - this.horChanage).ToString();
                                                        if (!str.Contains('E'))
                                                        {
                                                            this.node.m_TempPosition.Y = this.node.m_TempPosition.Y - this.horChanage;
                                                            this.node.m_TempSize.Height = this.node.Height + this.horChanage;
                                                            this.node.SnapHeight(true);
                                                        }
                                                        else
                                                        {
                                                            this.node.m_TempSize.Width = this.node.m_TempSize.Width - this.horChanage;
                                                            this.node.SnapWidth(false);
                                                        }
                                                    }
                                                    
                                                }
                                                else if (Mouse.GetPosition(dview).X <= (this.node.m_TempPosition.X + this.node.Width + this.Width) && Mouse.GetPosition(dview).X > this.node.m_TempPosition.X)
                                                {
                                                    this.node.m_TempSize.Width = this.node.m_TempSize.Width - this.horChanage;
                                                    this.node.SnapWidth(false);
                                                }
                                                else if (this.node.m_TempPosition.Y <= 0)
                                                {
                                                    this.node.m_TempSize.Width = this.node.m_TempSize.Width - this.horChanage;
                                                    this.node.SnapWidth(false);
                                                }
                                            }                                        
                                    }
                                    if (this.ResizerVerticalAlignment == VerticalAlignment.Bottom && this.ResizerHorizontalAlignment == HorizontalAlignment.Left)
                                    {
                                        if (this.verChanage != 0)
                                        {
                                                if (Mouse.GetPosition(dview).Y >= (this.node.m_TempPosition.Y + this.node.Height) && Mouse.GetPosition(dview).X <= this.node.m_TempPosition.X && Mouse.GetPosition(dview).X < (this.node.m_TempPosition.X + this.node.Width))
                                                {
                                                    if (Mouse.GetPosition(dview).X >= this.node.m_TempPosition.X - this.Width)
                                                    {
                                                        this.node.m_TempPosition.X = this.node.m_TempPosition.X - this.verChanage;
                                                        this.node.m_TempSize.Width = this.node.Width + this.verChanage;
                                                        this.node.SnapWidth(true);
                                                    }
                                                    else if (Mouse.GetPosition(dview).X < this.node.m_TempPosition.X - this.Width)
                                                    {
                                                        if (this.verChanage < 0)
                                                        {
                                                            this.node.m_TempSize.Height = this.node.Height - this.verChanage;
                                                            this.node.SnapHeight(false);
                                                        }
                                                        else
                                                        {
                                                            if ((this.node.m_TempPosition.X - this.verChanage) < 0)
                                                            {
                                                                this.node.m_TempSize.Height = this.node.Height - this.verChanage;
                                                                this.node.SnapHeight(false);
                                                            }
                                                            else
                                                            {
                                                                this.node.m_TempPosition.X = this.node.m_TempPosition.X - this.verChanage;
                                                                this.node.m_TempSize.Width = this.node.Width + this.verChanage;
                                                                this.node.SnapWidth(true);
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (Mouse.GetPosition(dview).X > 0 && Mouse.GetPosition(dview).Y < (this.node.m_TempPosition.Y + this.node.Height) && (Mouse.GetPosition(dview).X <= this.node.m_TempPosition.X) && Mouse.GetPosition(dview).Y > this.node.m_TempPosition.Y)
                                                {
                                                    this.node.m_TempSize.Height = this.node.Height - this.verChanage;
                                                    this.node.SnapHeight(false);
                                                }
                                                else if (Mouse.GetPosition(dview).X > 0 && Mouse.GetPosition(dview).X > this.node.m_TempPosition.X)
                                                {
                                                    if (this.verChanage > 0)
                                                    {
                                                        if (Mouse.GetPosition(dview).Y > (this.node.m_TempPosition.Y + this.node.Height))
                                                        {
                                                            if ((this.node.m_TempPosition.X - this.verChanage) <= 0)
                                                            {
                                                                this.node.m_TempSize.Height = this.node.Height - this.verChanage;
                                                                this.node.SnapHeight(false);
                                                            }
                                                            else
                                                            {
                                                                string str = (this.node.m_TempPosition.X - this.verChanage).ToString();
                                                                if (!str.Contains('E'))
                                                                {
                                                                    this.node.m_TempPosition.X = this.node.m_TempPosition.X - this.verChanage;
                                                                    this.node.m_TempSize.Width = this.node.Width + this.verChanage;
                                                                    this.node.SnapWidth(true);
                                                                }
                                                                else if ((this.node.m_TempPosition.X - this.verChanage) > 0)
                                                                {
                                                                    this.node.m_TempSize.Height = this.node.Height - this.verChanage;
                                                                    this.node.SnapHeight(false);
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            this.node.m_TempSize.Height = this.node.Height - this.verChanage;
                                                            this.node.SnapHeight(false);
                                                        }
                                                    }
                                                    else if (this.verChanage < 0)
                                                    {
                                                         this.node.m_TempPosition.X = this.node.m_TempPosition.X - this.verChanage;
                                                         this.node.m_TempSize.Width = this.node.Width + this.verChanage;
                                                         this.node.SnapWidth(true);
                                                    }
                                                }      
                                                else if(Mouse.GetPosition(dview).X<=0)
                                                {
                                                    this.node.m_TempSize.Height = this.node.Height - this.verChanage;
                                                    this.node.SnapHeight(false);
                                                }
                                        }
                                        if (this.horChanage != 0)
                                        {
                                            if (Mouse.GetPosition(dview).X>0 && Mouse.GetPosition(dview).X <= this.node.m_TempPosition.X)
                                            {
                                                if (Mouse.GetPosition(dview).Y <= (this.node.m_TempPosition.Y + this.node.Height + this.Height))
                                                {
                                                    this.node.m_TempSize.Height = this.node.Height - this.horChanage;
                                                    this.node.SnapHeight(false);
                                                }
                                                else
                                                {
                                                    this.node.m_TempPosition.X = this.node.m_TempPosition.X - this.horChanage;
                                                    this.node.m_TempSize.Width = this.node.Width + this.horChanage;
                                                    this.node.SnapWidth(true);
                                                }

                                            }
                                            else if (Mouse.GetPosition(dview).X > 0 && Mouse.GetPosition(dview).X > this.node.m_TempPosition.X)
                                            {
                                                this.node.m_TempPosition.X = this.node.m_TempPosition.X - this.horChanage;
                                                this.node.m_TempSize.Width = this.node.Width + this.horChanage;
                                                this.node.SnapWidth(true);
                                            }
                                            else if (Mouse.GetPosition(dview).X <= 0)
                                            {
                                                this.node.m_TempPosition.X = this.node.m_TempPosition.X - this.horChanage;
                                                this.node.m_TempSize.Width = this.node.Width + this.horChanage;
                                                this.node.SnapWidth(true);
                                            }
                                        }
                                    }
                                    else if (this.ResizerVerticalAlignment == VerticalAlignment.Top && this.ResizerHorizontalAlignment == HorizontalAlignment.Left)
                                    {
                                        if (this.verChanage != 0)
                                        {
                                            if ((this.node.Width + this.verChanage) >= 0)
                                            {
                                                if (Mouse.GetPosition(dview).Y > 0 && Mouse.GetPosition(dview).X > 0 && Mouse.GetPosition(dview).Y <= (this.node.m_TempPosition.Y))
                                                    {
                                                        if ((this.node.m_TempPosition.X + this.verChanage) < 0)
                                                        {
                                                            this.node.m_TempPosition.Y = this.node.m_TempPosition.Y - this.verChanage;
                                                            this.node.m_TempSize.Height = this.node.Height + this.verChanage;
                                                            this.node.SnapHeight(true);
                                                        }
                                                        else
                                                        {
                                                            string str=(this.node.m_TempPosition.X + this.verChanage).ToString();
                                                            if (!str.Contains('E'))
                                                            {
                                                                this.node.m_TempPosition.X = this.node.m_TempPosition.X + this.verChanage;
                                                                this.node.m_TempSize.Width = this.node.Width - this.verChanage;
                                                                this.node.SnapWidth(true);
                                                            }
                                                            else
                                                            {
                                                                this.node.m_TempPosition.Y = this.node.m_TempPosition.Y - this.verChanage;
                                                                this.node.m_TempSize.Height = this.node.Height + this.verChanage;
                                                                this.node.SnapHeight(true);
                                                            }
                                                        }
                                                    }
                                                else if (Mouse.GetPosition(dview).Y > 0 && Mouse.GetPosition(dview).X > 0 && Mouse.GetPosition(dview).Y > (this.node.m_TempPosition.Y) && Mouse.GetPosition(dview).X <= this.node.m_TempPosition.X && Mouse.GetPosition(dview).X >= (this.node.m_TempPosition.X - this.Width))
                                                    {
                                                        this.node.m_TempPosition.Y = this.node.m_TempPosition.Y - this.verChanage;
                                                        this.node.m_TempSize.Height = this.node.Height + this.verChanage;
                                                        this.node.SnapHeight(true);
                                                    }
                                                else if (Mouse.GetPosition(dview).Y > 0 && Mouse.GetPosition(dview).X > 0 && Mouse.GetPosition(dview).Y > (this.node.m_TempPosition.Y) && Mouse.GetPosition(dview).X < (this.node.m_TempPosition.X - this.Width))
                                                    {
                                                        this.node.m_TempPosition.Y = this.node.m_TempPosition.Y - this.verChanage;
                                                        this.node.m_TempSize.Height = this.node.Height + this.verChanage;
                                                        this.node.SnapHeight(true);
                                                    }
                                                else if (Mouse.GetPosition(dview).Y <= 0 || Mouse.GetPosition(dview).X <= 0)
                                                {
                                                    this.node.m_TempPosition.Y = this.node.m_TempPosition.Y - this.verChanage;
                                                    this.node.m_TempSize.Height = this.node.Height + this.verChanage;
                                                    this.node.SnapHeight(true);
                                                }
                                            }
                                        }
                                        if (this.horChanage != 0)
                                        {
                                            if ((this.node.Height + (int)this.horChanage) >= 0)
                                            {
                                                if (Mouse.GetPosition(dview).X > 0 && Mouse.GetPosition(dview).Y > 0 && Mouse.GetPosition(dview).X <= (this.node.m_TempPosition.X) && Mouse.GetPosition(dview).Y <= this.node.m_TempPosition.Y && Mouse.GetPosition(dview).Y >= (this.node.m_TempPosition.Y - this.Height))
                                                {
                                                    this.node.m_TempPosition.Y = this.node.m_TempPosition.Y + this.horChanage;
                                                    this.node.m_TempSize.Height = this.node.Height - this.horChanage;
                                                    this.node.SnapHeight(true);
                                                }
                                                else if (Mouse.GetPosition(dview).X > 0 && Mouse.GetPosition(dview).Y > 0 && Mouse.GetPosition(dview).X > (this.node.m_TempPosition.X) && Mouse.GetPosition(dview).Y <= this.node.m_TempPosition.Y)
                                                {
                                                    this.node.m_TempPosition.X = this.node.m_TempPosition.X - this.horChanage;
                                                    this.node.m_TempSize.Width = this.node.Width + this.horChanage;
                                                    this.node.SnapWidth(true);
                                                }
                                                else if (Mouse.GetPosition(dview).X > 0 && Mouse.GetPosition(dview).Y > 0 && Mouse.GetPosition(dview).X <= this.node.m_TempPosition.X && Mouse.GetPosition(dview).Y > this.node.m_TempPosition.Y)
                                                {
                                                    if ((this.node.m_TempPosition.Y + this.horChanage) <= 0)
                                                    {
                                                        this.node.m_TempPosition.X = this.node.m_TempPosition.X - this.horChanage;
                                                        this.node.m_TempSize.Width = this.node.Width + this.horChanage;
                                                        this.node.SnapWidth(true);
                                                    }
                                                    else
                                                    {
                                                        string str = (this.node.m_TempPosition.Y + this.horChanage).ToString();
                                                        if (!str.Contains('E'))
                                                        {
                                                            this.node.m_TempPosition.Y = this.node.m_TempPosition.Y + this.horChanage;
                                                            this.node.m_TempSize.Height = this.node.Height - this.horChanage;
                                                            this.node.SnapHeight(true);
                                                        }
                                                        else
                                                        {
                                                            this.node.m_TempPosition.X = this.node.m_TempPosition.X - this.horChanage;
                                                            this.node.m_TempSize.Width = this.node.Width + this.horChanage;
                                                            this.node.SnapWidth(true);
                                                        }
                                                    }
                                                }
                                                else if (Mouse.GetPosition(dview).X > 0 && Mouse.GetPosition(dview).Y > 0 && Mouse.GetPosition(dview).X <= (this.node.m_TempPosition.X) && Mouse.GetPosition(dview).Y < (this.node.m_TempPosition.Y - this.Height))
                                                {
                                                    if (this.horChanage < 0)
                                                    {
                                                        this.node.m_TempPosition.Y = this.node.m_TempPosition.Y + this.horChanage;
                                                        this.node.m_TempSize.Height = this.node.Height - this.horChanage;
                                                        this.node.SnapHeight(true);
                                                    }
                                                    else
                                                    {
                                                        this.node.m_TempPosition.X = this.node.m_TempPosition.X - this.horChanage;
                                                        this.node.m_TempSize.Width = this.node.Width + this.horChanage;
                                                        this.node.SnapWidth(true);
                                                    }
                                                }
                                                else if (Mouse.GetPosition(dview).X <= 0 || Mouse.GetPosition(dview).Y <= 0)
                                                {
                                                    this.node.m_TempPosition.X = this.node.m_TempPosition.X - this.horChanage;
                                                    this.node.m_TempSize.Width = this.node.Width + this.horChanage;
                                                    this.node.SnapWidth(true);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (node is Group)
                        {
                            CollectionExt gnodechildren = new CollectionExt();
                            for (int i = 0; i < (node as Group).NodeChildren.Count; i++)
                            {
                                if ((node as Group).NodeChildren[i] is Node)
                                {
                                    gnodechildren.Add((node as Group).NodeChildren[i] as Node);
                                }
                            }

                            gnodechildren.Add(node);

                            foreach (Node item in gnodechildren)
                            {
                                if (!item.m_MouseResizing)
                                {
                                    item.ResizeThisNode = true;
                                    fun(mPage, item, sender, e, /*resizeVertDelta, resizeHorDelta, scale,*/ minimumVertDelta, minimumHorDelta, minTop, minLeft, delta);
                                }
                            }
                        }
                        else
                        {
                            foreach (Node item in selectedNodes)
                            {
                                if (!item.m_MouseResizing)
                                {
                                    fun(mPage, item, sender, e, /*resizeVertDelta, resizeHorDelta, scale,*/ minimumVertDelta, minimumHorDelta, minTop, minLeft, delta);
                                }
                            }
                        }
                        this.node.m_MouseResizing = false;
                        e.Handled = true;
                    }
                }
            }
        }

        private void fun(IDiagramPage mPage, Node item, object sender, DragDeltaEventArgs e,  double minimumVertDelta, double minimumHorDelta, double minTop, double minLeft, Point delta)
        {
            double resizeVertDelta;
            double resizeHorDelta;
            double scale;

            if (!(mPage as DiagramPage).EnableResizingCurrentNodeOnMultipleSelection)
            {
                item.ResizeThisNode = true;
            }
            if (item.AllowResize)
            {

                if (item != null && item.ParentID == Guid.Empty && item.ResizeThisNode)
                {
                    switch (this.VerticalAlignment)
                    {
                        case System.Windows.VerticalAlignment.Bottom:

                            resizeVertDelta = Math.Min(-verChanage, minimumVertDelta);
                            item.m_ResizeVertDelta = resizeVertDelta;
                            scale = Math.Max(0, (item.m_TempSize.Height - resizeVertDelta) / item.m_TempSize.Height);
                            HandleRenderer.ResizeBottom(scale, item, mPage, (mPage as DiagramPage).MeasurementUnits);
                            delta.X = item.Width > delta.X ? delta.X : 0;
                            delta.Y = item.Height > delta.Y ? delta.Y : 0;
                            item.Width -= delta.X;
                            if ((sender as Thumb).HorizontalAlignment == System.Windows.HorizontalAlignment.Stretch)
                            {
                                item.Height -= delta.Y;
                            }
                            m_newsize = new Size(item.Width, item.Height);
                            foreach (ConnectionPort port in item.Ports)
                            {
                                port.Left = (m_newsize.Width / (item as Node).Oldsize.Width) * port.PreviousPortPoint.X;
                                port.Top = (m_newsize.Height / (item as Node).Oldsize.Height) * port.PreviousPortPoint.Y;
                                TranslateTransform tr = new TranslateTransform(port.Left - port.ActualWidth / 2, port.Top - port.ActualHeight / 2);
                                port.RenderTransform = tr;
                            }

                            break;

                        case System.Windows.VerticalAlignment.Top:
                            double top = Canvas.GetTop(item);
                            if ((mPage as DiagramPage).MeasurementUnits == MeasureUnits.Kilometer || (mPage as DiagramPage).MeasurementUnits == MeasureUnits.Meter || (mPage as DiagramPage).MeasurementUnits == MeasureUnits.Inch ||
                                (mPage as DiagramPage).MeasurementUnits == MeasureUnits.Foot || (mPage as DiagramPage).MeasurementUnits == MeasureUnits.Yard || (mPage as DiagramPage).MeasurementUnits == MeasureUnits.Mile)
                            {
                                double dragv = Math.Min(Math.Max(-minTop, verChanage), minimumVertDelta);
                                if (dragv < 0)
                                {
                                    resizeVertDelta = dragv;
                                    item.m_ResizeVertDelta = resizeVertDelta;// MeasureUnitsConverter.Convert(dragv, (mPage as DiagramPage).MeasurementUnits, MeasureUnits.HalfInch);
                                }
                                else
                                {
                                    resizeVertDelta = dragv;
                                    item.m_ResizeVertDelta = resizeVertDelta;
                                }
                            }
                            else
                            {
                                resizeVertDelta = Math.Min(Math.Max(-minTop, verChanage), minimumVertDelta);
                                item.m_ResizeVertDelta = resizeVertDelta;
                            }

                            //scale = (item.ActualHeight - resizeVertDelta) / item.ActualHeight;
                            //HandleRenderer.ResizeTop(scale, item, mPage, (mPage as DiagramPage).MeasurementUnits);
                            //m_newsize = new Size(item.Width, item.Height);
                            //foreach (ConnectionPort port in item.Ports)
                            //{
                            //    port.Left = (m_newsize.Width / (item as Node).Oldsize.Width) * port.PreviousPortPoint.X;
                            //    port.Top = (m_newsize.Height / (item as Node).Oldsize.Height) * port.PreviousPortPoint.Y;
                            //    TranslateTransform tr = new TranslateTransform(port.Left, port.Top);
                            //    port.RenderTransform = tr;
                            //}

                            scale = Math.Max(0, (item.m_TempSize.Height - resizeVertDelta) / item.m_TempSize.Height);
                            if (scale > 0)
                            {
                                HandleRenderer.ResizeTop(scale, item, mPage, (mPage as DiagramPage).MeasurementUnits);
                                delta.X = item.Width > delta.X ? delta.X : 0;
                                delta.Y = item.Height > delta.Y ? delta.Y : 0;
                                item.Width -= delta.X;
                                if ((sender as Thumb).HorizontalAlignment == System.Windows.HorizontalAlignment.Stretch)
                                {
                                    item.Height -= delta.Y;
                                }
                                item.PxOffsetY += delta.Y;// MeasureUnitsConverter.FromPixels(delta.Y, item.MeasurementUnits);
                                this.m_newsize = new Size(item.Width, item.Height);
                                foreach (ConnectionPort port in item.Ports)
                                {
                                    port.Left = (m_newsize.Width / (item as Node).Oldsize.Width) * port.PreviousPortPoint.X;
                                    port.Top = (m_newsize.Height / (item as Node).Oldsize.Height) * port.PreviousPortPoint.Y;
                                    TranslateTransform tr = new TranslateTransform(port.Left - port.ActualWidth / 2, port.Top - port.ActualHeight / 2);
                                    port.RenderTransform = tr;
                                }
                            }
                            break;

                        default:
                            break;
                    }

                    switch (this.HorizontalAlignment)
                    {
                        case System.Windows.HorizontalAlignment.Left:
                            double left = Canvas.GetLeft(item);
                            if ((mPage as DiagramPage).MeasurementUnits == MeasureUnits.Kilometer || (mPage as DiagramPage).MeasurementUnits == MeasureUnits.Meter || (mPage as DiagramPage).MeasurementUnits == MeasureUnits.Inch ||
                               (mPage as DiagramPage).MeasurementUnits == MeasureUnits.Foot || (mPage as DiagramPage).MeasurementUnits == MeasureUnits.Yard || (mPage as DiagramPage).MeasurementUnits == MeasureUnits.Mile)
                            {
                                double dragh = Math.Min(Math.Max(-minLeft, horChanage), minimumHorDelta);
                                if (dragh < 0)
                                {
                                    resizeHorDelta = dragh;
                                    item.m_ResizeHorDelta = resizeHorDelta;// MeasureUnitsConverter.Convert(dragh, (mPage as DiagramPage).MeasurementUnits, MeasureUnits.HalfInch);
                                }
                                else
                                {
                                    resizeHorDelta = dragh;
                                    item.m_ResizeHorDelta = resizeHorDelta;
                                }
                            }
                            else
                            {
                                resizeHorDelta = Math.Min(Math.Max(-minLeft, horChanage), minimumHorDelta);
                                item.m_ResizeHorDelta = resizeHorDelta;
                            }

                            //scale = (item.ActualWidth - resizeHorDelta) / item.ActualWidth;
                            //HandleRenderer.ResizeLeft(scale, item, mPage, (mPage as DiagramPage).MeasurementUnits);
                            //m_newsize = new Size(item.Width, item.Height);
                            scale = scale = Math.Max(0, (item.m_TempSize.Width - resizeHorDelta) / item.m_TempSize.Width);
                            if (scale > 0)
                            {
                                HandleRenderer.ResizeLeft(scale, item, mPage, (mPage as DiagramPage).MeasurementUnits);
                                delta.X = item.Width > delta.X ? delta.X : 0;
                                delta.Y = item.Height > delta.Y ? delta.Y : 0;
                                item.Width -= delta.X;
                                item.Height -= delta.Y;
                                item.PxOffsetX += delta.X;// MeasureUnitsConverter.FromPixels(delta.X, item.MeasurementUnits);
                                this.m_newsize = new Size(item.Width, item.Height);
                            }
                            foreach (ConnectionPort port in item.Ports)
                            {
                                port.Left = (m_newsize.Width / (item as Node).Oldsize.Width) * port.PreviousPortPoint.X;
                                port.Top = (m_newsize.Height / (item as Node).Oldsize.Height) * port.PreviousPortPoint.Y;
                                TranslateTransform tr = new TranslateTransform(port.Left - port.ActualWidth / 2, port.Top - port.ActualHeight / 2);
                                port.RenderTransform = tr;
                            }

                            break;

                        case System.Windows.HorizontalAlignment.Right:
                            //Point current = Mouse.GetPosition(mPage as DiagramPage);
                            //Point pre = new Point(this.node.LogicalOffsetX + this.node.m_TempSize.Width, this.node.LogicalOffsetY + this.node.m_TempSize.Height);
                            //double horChanage = -(pre.X - current.X);

                            resizeHorDelta = Math.Min(-horChanage, minimumHorDelta);
                            item.m_ResizeHorDelta = resizeHorDelta;
                            scale = Math.Max(0, (item.m_TempSize.Width - resizeHorDelta) / item.m_TempSize.Width);
                            if (scale > 0)
                            {
                                HandleRenderer.ResizeRight(scale, item, mPage, (mPage as DiagramPage).MeasurementUnits);
                                delta.X = item.Width > delta.X ? delta.X : 0;
                                delta.Y = item.Height > delta.Y ? delta.Y : 0;
                                item.Width -= delta.X;
                                item.Height -= delta.Y;
                                m_newsize = new Size(item.Width, item.Height);
                                foreach (ConnectionPort port in item.Ports)
                                {
                                    port.Left = (m_newsize.Width / (item as Node).Oldsize.Width) * port.PreviousPortPoint.X;
                                    port.Top = (m_newsize.Height / (item as Node).Oldsize.Height) * port.PreviousPortPoint.Y;
                                    TranslateTransform tr = new TranslateTransform(port.Left - port.ActualWidth / 2, port.Top - port.ActualHeight / 2);
                                    port.RenderTransform = tr;
                                }
                            }

                            break;

                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the DragCompleted event of the ResizerThumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragCompletedEventArgs"/> instance containing the event data.</param>
        private void ResizerThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            dc.View.IsResized = false;
            if (node.AllowResize)
            {
                NodeRoutedEventArgs newEventArgs = new NodeRoutedEventArgs(node);
                newEventArgs.RoutedEvent = DiagramView.NodeResizedEvent;
                RaiseEvent(newEventArgs);
                DiagramView.IsOtherEvent = true;
            }
        }

        #endregion
    }
}
