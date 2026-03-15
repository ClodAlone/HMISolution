// <copyright file="DragProvider.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Interop;
using System.Reflection;
using System.IO;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Documents;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents Node Drag class.
    /// </summary>
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
    public class DragProvider : Thumb
    {
        #region Class variables

        /// <summary>
        /// Used to store the cursor used.
        /// </summary>
        private Cursor m_cursor;

        /// <summary>
        /// Used to store the drag delta.
        /// </summary>
        private Point dragdelta;

        /// <summary>
        /// Represents the rotate transform.
        /// </summary>
        private RotateTransform rotateTransform;

        /// <summary>
        /// Used to hold the boolean value for IsDragging property.
        /// </summary>
        private static bool d = false;

        /// <summary>
        /// Used to store the DiagramControl instance.
        /// </summary>
        private DiagramControl dc;

        /// <summary>
        /// Used to store the old position of the nodes.
        /// </summary>
        private ObservableCollection<Point> oldpositioncollection = new ObservableCollection<Point>();

        /// <summary>
        /// Used to store the selected items.
        /// </summary>
        private ObservableCollection<INodeGroup> itemcollection = new ObservableCollection<INodeGroup>();

        /// <summary>
        /// Checks if drag operation is performed.
        /// </summary>
        private bool isdragged = false;
        private bool IsReachedBound;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="DragProvider"/> class.
        /// </summary>
        public DragProvider()
        {
            DragDelta += new DragDeltaEventHandler(DragProvider_DragDelta);
            DragStarted += new DragStartedEventHandler(DragProvider_DragStarted);
            DragCompleted += new DragCompletedEventHandler(DragProvider_DragCompleted);
            this.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(DragProvider_PreviewMouseLeftButtonUp);
        }

        void DragProvider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dc.View.IsDragged = true;
        }

        List<Node> Viewportnodes;

        /// <summary>
        /// Handles the DragStarted event of the DragProvider control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragStartedEventArgs"/> instance containing the event data.</param>
        private void DragProvider_DragStarted(object sender, DragStartedEventArgs e)
        {
            
            dc.View.IsDragged = false;
            IsReachedBound = false;
            if (dc.View.LineRoutingEnabled)
            {
                dc.View._LineRoute = true;
            }
            dc.View._LineRoute = true;
            IShape node = this.DataContext as Node;
            if (node != null)
            {
                (node as Node).m_MouseMoving = true;
            }
            Viewportnodes = dc.View.ViewportNodes();
            Point p = new Point();
            if (node != null && dc != null && dc.Model != null)
            {
                pre = Mouse.GetPosition(dc.View.Page);
                foreach (Node n in dc.Model.Nodes)
                {
                    n.m_TempPosition = new Point(n.PxOffsetX, n.PxOffsetY);
                    n.m_TempSize = new Size(n.Width, n.Height);
                    n.OldOffset = new Point(n.PxOffsetX, n.PxOffsetY);
                }
                if (node.Groups.Count > 0)
                {
                    foreach (Group g in node.Groups)
                    {
                        g.m_TempPosition = new Point(g.PxOffsetX, g.PxOffsetY);
                        g.m_TempSize = new Size(g.Width, g.Height);
                        p.X = (g as Group).PxOffsetX;
                        p.Y = (g as Group).PxOffsetY;
                        (g as Group).m_Delta = p;
                        foreach (object o in g.NodeChildren)
                        {
                            if (o is Node)
                            {
                                Node n = o as Node;
                                n.m_TempPosition = new Point(n.PxOffsetX, n.PxOffsetY);
                                n.m_TempSize = new Size(n.Width, n.Height);
                                (n as Node).m_Delta.X = Math.Abs((n as Node).PxOffsetX - p.X);
                                (n as Node).m_Delta.Y = Math.Abs((n as Node).PxOffsetY - p.Y);
                            }
                        }
                    }
                }
                foreach (LineConnector lc in dc.Model.Connections)
                {
                    lc.m_TempStartPoint = lc.PxStartPointPosition;
                    lc.m_TempEndPoint = lc.PxEndPointPosition;
                    lc.m_TempInerPts = new List<Point>();
                    if (lc.IntermediatePoints != null)
                    {
                        foreach (Point pt in lc.IntermediatePoints)
                        {
                            lc.m_TempInerPts.Add(pt);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the DragCompleted event of the DragProvider control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragCompletedEventArgs"/> instance containing the event data.</param>
        private void DragProvider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            dc.View.IsDragged = false;
            if (dc.View.SnapSettings != null)
            {
                dc.View.SnapSettings.Clear();
            }
            if (dc.View.LineRoutingEnabled)
            {
                dc.View._LineRoute = false;
            }
            dc.View._LineRoute = false;
            IShape node = this.DataContext as Node;          
            if (DragProvider.Isdragging)
            {
                (dc.View.Page as DiagramPage).InvalidateMeasure();
                (dc.View.Page as DiagramPage).InvalidateArrange();
                (node as Node).currentdragging = false;
                (node as Node).RaiseNodeDragEndEvent();
                DiagramView.IsOtherEvent = true;
            }
            if ((!(dc.View.undo || dc.View.redo)) && e.HorizontalChange > 0 && e.VerticalChange > 0)
            {
                dc.View.RedoStack.Clear();//
            }

            if (dc.View.Isdragdelta)
            {
                isdragged = false;
                int i = 0;
                if (dc != null && dc.View != null && dc.View.UndoRedoEnabled)
                {
                    foreach (INodeGroup n in itemcollection)
                    {
                        dc.View.UndoStack.Push((n as Node).OldOffset);
                        dc.View.UndoStack.Push(n as Node);
                        dc.View.UndoStack.Push(itemcollection.Count);
                        dc.View.UndoStack.Push("Dragged");
                        i++;
                    }
                }
                itemcollection.Clear();
                if (dc.View.EnableVirtualization)
                {
                    dc.View.ScrollGrid.callCalculate();
                }
                dc.View.Isdragdelta = false;

            }
            //foreach (ICommon n in dc.View.SelectionList.OfType<ICommon>())
            //{
            //    if (n is Group)
            //    {
            //        //(n as Group).DragCancel = false;
            //        foreach (INodeGroup n1 in (n as Group).NodeChildren)
            //        {
            //            if (n1 is Node)
            //            {
            //                //(n1 as Node).DragCancel = false;
            //            }                        
            //        }
            //    }
            //    else if (n is Node)
            //    {
            //        //(n as Node).DragCancel = false;
            //    }                
            //}  
            dc.View.AllowMoveX = true;
            dc.View.AllowMoveY = true;
            dc.View.OnlyX = false;
            dc.View.OnlyY = false;
            dc.View.ScrollGrid.InvalidateMeasure();
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Measurement unit property.
        /// <value>
        /// Type: <see cref="MeasureUnits"/>
        /// Enum specifying the unit to be used.
        /// </value>
        /// </summary>
        internal MeasureUnits MeasurementUnits
        {
            get
            {
                return (MeasureUnits)GetValue(MeasurementUnitsProperty);
            }

            set
            {
                SetValue(MeasurementUnitsProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether the node is dragged.
        /// </summary>
        /// <value><c>true</c> if node is been dragged; otherwise, <c>false</c>.</value>
        internal static bool Isdragging
        {
            get
            {
                return d;
            }

            set
            {
                d = value;
            }
        }

        #endregion

        #region DPs

        /// <summary>
        /// Specifies the MeasurementUnits dependency property.
        /// </summary>
        public static readonly DependencyProperty MeasurementUnitsProperty = DependencyProperty.Register("MeasurementUnits", typeof(MeasureUnits), typeof(DragProvider), new PropertyMetadata(MeasureUnits.Pixel, new PropertyChangedCallback(OnMeasurementUnitChanged)));

        #endregion

        #region Implementation

        /// <summary>
        /// Called when [measurement unit changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMeasurementUnitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Provides class handling for the <see cref="E:System.Windows.UIElement.MouseMove"/> event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            
            IShape node = this.DataContext as Node;
            IDiagramPage diagramPanel = VisualTreeHelper.GetParent(node as Node) as IDiagramPage;
            dc = (node as Node).Nodediagramcontrol;
            if (dc == null)
            {
                dc = DiagramPage.GetDiagramControl((FrameworkElement)diagramPanel);
            }
            if (dc.View.IsPageEditable && (node as Node).AllowSelect && (node as Node).AllowMove)
            {
                if (dc.View.IsPanEnabled)
                {

                    if (BrowserInteropHelper.IsBrowserHosted)
                    {
                        this.Cursor = System.Windows.Input.Cursors.Hand;
                    }
                    else
                    {
                        if (m_cursor == null)
                        {
                            Assembly ass = Assembly.GetExecutingAssembly();
                            Stream stream = ass.GetManifestResourceStream("Syncfusion.Windows.Diagram.Icons.Released.cur");
                            m_cursor = new Cursor(stream);
                        }
                        this.Cursor = m_cursor;
                    }
                }
                else
                {
                    if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))
                    {
                        if ((node as Node).AddConnectionPortEnabled)
                        {
                            Assembly ass = Assembly.GetExecutingAssembly();
                            System.IO.Stream stream = ass.GetManifestResourceStream("Syncfusion.Windows.Diagram.Icons.InsertVertex.cur");
                            this.Cursor = new Cursor(stream);
                        }
                        else
                        {
                            this.Cursor = System.Windows.Input.Cursors.SizeAll;
                        }
                        //this.Cursor = System.Windows.Input.Cursors.Arrow;
                    }
                    else
                    {
                        this.Cursor = System.Windows.Input.Cursors.SizeAll;
                    }
                }
            }
            else
            {
                this.Cursor = System.Windows.Input.Cursors.Arrow;
            }
            if ((Keyboard.Modifiers != ModifierKeys.Control) && (Keyboard.Modifiers != ModifierKeys.Shift))
            {
                base.OnMouseMove(e);
            }
        }

        Point current;
        Point pre;
        double horChanage;
        double verChanage;


        private bool CanMove()
        {
            if (!dc.View.EnableDrawingTools)
            {
                return true;
            }
            else if (dc.View.EnableDrawingTools && dc.View.NodeMode == ConnectionMode.Move)
                {
                    return true;
                }
            else
            {
            return false;
            }
        }

        bool acc = false;
        /// <summary>
        /// Handles the DragDelta event of the DragProvider control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void DragProvider_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Node node = this.DataContext as Node;
            dc.View.IsDragged = true;
            dc.View.Isdragdelta = true;
          
            VirtualizingPanel diagramPanel1 = (node as Node).Page as VirtualizingPanel;
            if (CanMove())
            {
                if ((node as Node).AllowMove)
                {
                    (node as Node).currentdragging = true;
                    (node as Node).RaiseNodeDragStartEvent();
                    NodeRoutedEventArgs newEventArgs = new NodeRoutedEventArgs(node as Node);
                    if (dc.View.IsPageEditable)
                    {
                        newEventArgs.RoutedEvent = DiagramView.NodeDraggingEvent;
                        RaiseEvent(newEventArgs);
                    }
                    //if (newEventArgs.Cancel == true)
                    //{
                    //    (node as Node).DragCancel = true;
                    //}
                    if (newEventArgs.Cancel == false)// && (node as Node).DragCancel == false)
                    {
                        DiagramView.IsOtherEvent = true;
                        IDiagramPage diagramPanel = VisualTreeHelper.GetParent(node as Node) as IDiagramPage;
                        DragProvider.Isdragging = true;
                        dc = node.Nodediagramcontrol as DiagramControl;
                        if (dc == null)
                        {
                            dc = DiagramPage.GetDiagramControl((FrameworkElement)diagramPanel);
                        }
                        if (dc.View.IsPageEditable)
                        {
                            current = Mouse.GetPosition(dc.View.Page as DiagramPage);
                            horChanage = current.X - pre.X;
                            verChanage = current.Y - pre.Y;
                            pre = current;
                            if (node is Layer && (node != null && diagramPanel != null))
                            {
                                Node n = this.DataContext as Node;
                                this.rotateTransform = n.RenderTransform as RotateTransform;
                                var nodes = diagramPanel.SelectionList.OfType<IShape>();
                                Node item = node;
                                ////foreach (IShape item in nodes)
                                {
                                    if (!isdragged)
                                    {
                                        foreach (Node nod in (node as Layer).Nodes)
                                        {
                                            //(nod as Node).OldOffset = new Point((nod as Node).PxOffsetX, (nod as Node).PxOffsetY);
                                            itemcollection.Add(nod as IShape);
                                        }
                                    }

                                    if (item is Layer)
                                    {
                                        DragGroup(dc, e, (item as Group), diagramPanel, new Point(0, 0));
                                    }

                                    dragdelta = new Point(e.HorizontalChange, e.VerticalChange);
                                    if (dc.View.SnapToVerticalGrid)
                                    {
                                        //dragdelta.X;
                                    }
                                    if (this.rotateTransform != null)
                                    {
                                        dragdelta = this.rotateTransform.Transform(dragdelta);
                                    }

                                    double offsetX = item.PxOffsetX;
                                    double offsetY = item.PxOffsetY;
                                    if (double.IsNaN(offsetX))
                                    {
                                        offsetX = 0;
                                    }

                                    if (double.IsNaN(offsetY))
                                    {
                                        offsetY = 0;
                                    }

                                    double dx = dragdelta.X;
                                    double dy = dragdelta.Y;
                                    item.PxOffsetX = offsetX + dx;
                                    item.PxOffsetY = offsetY + dy;
                                }

                                isdragged = true;
                            }
                            else if (node != null && diagramPanel != null && node.IsGrouped && !node.IsSelected)
                            {
                                foreach (Group g in (node as Node).Groups)
                                {
                                    if (g.IsSelected && g.AllowMove)
                                    {
                                        if (!dc.View.BoundaryConstraintsEnabled || ((g.RectBounds.Right + horChanage < dc.View.BoundaryConstraintsArea.Right) && (g.RectBounds.Left + horChanage > dc.View.BoundaryConstraintsArea.Left) && (g.RectBounds.Bottom + verChanage < dc.View.BoundaryConstraintsArea.Bottom) && (g.RectBounds.Top + verChanage >= dc.View.BoundaryConstraintsArea.Top) && !IsReachedBound))
                                        {
                                            Point delta = new Point(0, 0);
                                            delta = getDelta(node as Node, e);
                                            //setDragDelta(e);
                                            //horChanage = horChanage - delta.X;
                                            //verChanage = verChanage - delta.Y;
                                            CollectionExt gnodechildren = new CollectionExt();

                                            foreach (Group gnode in (node as Node).Groups)
                                            {
                                                //For fixing the Dragging the group inside the Group issue
                                                if ((gnode as Group).IsSelected)
                                                {
                                                    gnodechildren.Add(gnode);
                                                }
                                            }

                                            Node n = this.DataContext as Node;
                                            this.rotateTransform = n.RenderTransform as RotateTransform;
                                            foreach (INodeGroup item in dc.View.SelectionList)
                                            {
                                                if (!gnodechildren.Contains(item))
                                                {
                                                    gnodechildren.Add(item);
                                                }
                                            }

                                            foreach (INodeGroup item in gnodechildren)
                                            {
                                                if (item is Group)
                                                {
                                                    if (!(item is LineConnector))
                                                    {
                                                        if (!isdragged)
                                                        {
                                                            (item as Group).OldOffset = new Point((item as Group).PxOffsetX, (item as Group).PxOffsetY);
                                                            itemcollection.Add(item as INodeGroup);
                                                        }

                                                        if (item is Group)
                                                        {
                                                            DragGroup(dc, e, (item as Group), diagramPanel, delta);
                                                        }
                                                        double offsetX = (item as Group).m_TempPosition.X;
                                                        double offsetY = (item as Group).m_TempPosition.Y;

                                                        if (double.IsNaN(offsetX))
                                                        {
                                                            offsetX = 0;
                                                        }

                                                        if (double.IsNaN(offsetY))
                                                        {
                                                            offsetY = 0;
                                                        }

                                                        if (!(item as Group).m_MouseMoving)
                                                        {
                                                            //The following snippet has been commented, since this is the duplication of calculating group offset values. 

                                                            //(item as Group).m_TempPosition.X += dragdelta.X;
                                                            //(item as Group).m_TempPosition.Y += dragdelta.Y;
                                                            //(item as Group).PxOffsetX = (item as Group).m_TempPosition.X - delta.X;
                                                            //(item as Group).PxOffsetY = (item as Group).m_TempPosition.Y - delta.Y;
                                                        }
                                                        else
                                                        {
                                                            (item as Group).m_MouseMoving = false;
                                                        }
                                                    }
                                                    else
                                                        if (item is LineConnector)
                                                        {
                                                            LineConnector lc = (item as LineConnector);
                                                            lc.m_TempStartPoint.X += dragdelta.X;
                                                            lc.m_TempEndPoint.X += dragdelta.X;
                                                            lc.m_TempStartPoint.Y += dragdelta.Y;
                                                            lc.m_TempEndPoint.Y += dragdelta.Y;
                                                            lc.PxStartPointPosition = new Point(lc.m_TempStartPoint.X - delta.X, lc.m_TempStartPoint.Y - delta.Y);
                                                            lc.PxEndPointPosition = new Point(lc.m_TempEndPoint.X - delta.X, lc.m_TempEndPoint.Y - delta.Y);

                                                            if (lc.IntermediatePoints != null)
                                                                for (int i = 0; i < lc.IntermediatePoints.Count; i++)
                                                                {
                                                                    lc.m_TempInerPts[i] = new Point(lc.m_TempInerPts[i].X + dragdelta.X, lc.m_TempInerPts[i].Y + dragdelta.Y);
                                                                    lc.IntermediatePoints[i] = new Point(lc.m_TempInerPts[i].X - delta.X, lc.m_TempInerPts[i].Y - delta.Y);
                                                                }
                                                            lc.InvalidateConnectorPathGeometry();
                                                        }
                                                }

                                                else
                                                    if (item is Node)
                                                    {
                                                        if (!isdragged)
                                                        {
                                                            //(item as Node).OldOffset = new Point((item as Node).PxOffsetX, (item as Node).PxOffsetY);
                                                            itemcollection.Add(item as INodeGroup);
                                                        }

                                                        double offsetX = (item as Node).m_TempPosition.X;
                                                        double offsetY = (item as Node).m_TempPosition.Y;

                                                        if (double.IsNaN(offsetX))
                                                        {
                                                            offsetX = 0;
                                                        }

                                                        if (double.IsNaN(offsetY))
                                                        {
                                                            offsetY = 0;
                                                        }

                                                        if (!(item as Node).m_MouseMoving)
                                                        {

                                                            (item as Node).m_TempPosition.X += dragdelta.X;
                                                            (item as Node).m_TempPosition.Y += dragdelta.Y;
                                                            (item as Node).PxOffsetX = (item as Node).m_TempPosition.X - delta.X;
                                                            (item as Node).PxOffsetY = (item as Node).m_TempPosition.Y - delta.Y;
                                                        }
                                                        else
                                                        {
                                                            (item as Node).m_MouseMoving = false;
                                                        }
                                                    }

                                                    else
                                                        if (item is LineConnector)
                                                        {
                                                            LineConnector lc = (item as LineConnector);
                                                            lc.m_TempStartPoint.X += dragdelta.X;
                                                            lc.m_TempEndPoint.X += dragdelta.X;
                                                            lc.m_TempStartPoint.Y += dragdelta.Y;
                                                            lc.m_TempEndPoint.Y += dragdelta.Y;
                                                            lc.PxStartPointPosition = new Point(lc.m_TempStartPoint.X - delta.X, lc.m_TempStartPoint.Y - delta.Y);
                                                            lc.PxEndPointPosition = new Point(lc.m_TempEndPoint.X - delta.X, lc.m_TempEndPoint.Y - delta.Y);

                                                            if (lc.IntermediatePoints != null)
                                                                for (int i = 0; i < lc.IntermediatePoints.Count; i++)
                                                                {
                                                                    lc.m_TempInerPts[i] = new Point(lc.m_TempInerPts[i].X + dragdelta.X, lc.m_TempInerPts[i].Y + dragdelta.Y);
                                                                    lc.IntermediatePoints[i] = new Point(lc.m_TempInerPts[i].X - delta.X, lc.m_TempInerPts[i].Y - delta.Y);
                                                                }
                                                            lc.InvalidateConnectorPathGeometry();
                                                        }
                                            }
                                        }
                                        else
                                        {
                                            IsReachedBound = true;
                                        }
                                    }
                                }

                                isdragged = true;
                            }
                            else if (node != null && diagramPanel != null && node.IsSelected)
                            {
                                Node n = this.DataContext as Node;
                                acc = false;
                                if (dc.View.SelectionList.Count > 1)
                                {
                                    foreach (Node n1 in dc.View.SelectionList.OfType<Node>())
                                    {
                                        Rect rect = new Rect(n1.OffsetX, n1.OffsetY, n1.Width, n1.Height);
                                        if (!dc.View.BoundaryConstraintsEnabled || ((n1.RectBounds.Right + horChanage < dc.View.BoundaryConstraintsArea.Right) && (n1.RectBounds.Left + horChanage > dc.View.BoundaryConstraintsArea.Left) && (n1.RectBounds.Bottom + verChanage < dc.View.BoundaryConstraintsArea.Bottom) && (n1.RectBounds.Top + verChanage > dc.View.BoundaryConstraintsArea.Top) && !IsReachedBound))
                                        {
                                        }
                                        else
                                        {
                                            acc = true;
                                        }
                                    }
                                }
                                this.rotateTransform = n.RenderTransform as RotateTransform;
                                var nodes = diagramPanel.SelectionList.OfType<IShape>().ToList();
                                var lines = diagramPanel.SelectionList.OfType<IEdge>().ToList();
                                if (!acc)
                                {
                                Point delta;
                                delta = getDelta(node as Node, e);
                                foreach (IShape item in nodes)
                                {
                                    if (!isdragged)
                                    {
                                        //(item as Node).OldOffset = new Point((item as Node).PxOffsetX, (item as Node).PxOffsetY);
                                        itemcollection.Add(item as IShape);
                                    }

                                    if (item is Group)
                                    {

                                        DragGroup(dc, e, (item as Group), diagramPanel, delta);
                                    }

                                    double offsetX = (item as Node).m_TempPosition.X;
                                    double offsetY = (item as Node).m_TempPosition.Y;
                                    if (double.IsNaN(offsetX))
                                    {
                                        offsetX = 0;
                                    }

                                    if (double.IsNaN(offsetY))
                                    {
                                        offsetY = 0;
                                    }

                                    if (!(item as Node).m_MouseMoving)
                                    {
                                        if ((item as Node).AllowMove)
                                        {
                                            (item as Node).m_TempPosition.X += dragdelta.X;
                                            (item as Node).m_TempPosition.Y += dragdelta.Y;
                                            (item as Node).PxOffsetX = (item as Node).m_TempPosition.X - delta.X;
                                            (item as Node).PxOffsetY = (item as Node).m_TempPosition.Y - delta.Y;
                                        }
                                    }
                                    else
                                    {
                                        (item as Node).m_MouseMoving = false;
                                    }
                                }

                                foreach (IEdge item in lines)
                                {
                                    if (item is LineConnector)
                                    {
                                        LineConnector lc = (item as LineConnector);
                                        lc.m_TempStartPoint.X += dragdelta.X;
                                        lc.m_TempEndPoint.X += dragdelta.X;
                                        lc.m_TempStartPoint.Y += dragdelta.Y;
                                        lc.m_TempEndPoint.Y += dragdelta.Y;
                                        lc.PxStartPointPosition = new Point(lc.m_TempStartPoint.X - delta.X, lc.m_TempStartPoint.Y - delta.Y);
                                        lc.PxEndPointPosition = new Point(lc.m_TempEndPoint.X - delta.X, lc.m_TempEndPoint.Y - delta.Y);

                                        if (lc.IntermediatePoints != null && lc.m_TempInerPts.Count==lc.IntermediatePoints.Count)
                                            for (int i = 0; i < lc.IntermediatePoints.Count; i++)
                                            {
                                                lc.m_TempInerPts[i] = new Point(lc.m_TempInerPts[i].X + dragdelta.X, lc.m_TempInerPts[i].Y + dragdelta.Y);
                                                lc.IntermediatePoints[i] = new Point(lc.m_TempInerPts[i].X - delta.X, lc.m_TempInerPts[i].Y - delta.Y);                                               
                                            }
                                        lc.InvalidateConnectorPathGeometry();
                                    }
                                }
                                isdragged = true;
                            }
                            else
                            {
                                IsReachedBound = true;
                            }
                            }

                            dc.View.Ispositionchanged = true;
                            (this.dc.View.Page as DiagramPage).InvalidateMeasure();
                            e.Handled = true;
                        }
                    }
                    else// if (newEventArgs.Cancel == true || (node as Node).DragCancel == true)
                    {
                        foreach (ICommon n in dc.View.SelectionList.OfType<ICommon>())
                        {
                            if (n is Group)
                            {
                                (n as Group).OffsetX = (n as Group).x;
                                (n as Group).OffsetY = (n as Group).y;
                                foreach (INodeGroup n1 in (n as Group).NodeChildren)
                                {
                                    if (n1 is Node)
                                    {
                                        (n1 as Node).OffsetY = (n1 as Node).y;
                                        (n1 as Node).OffsetX = (n1 as Node).x;
                                    }
                                    else
                                    {
                                        (n1 as LineConnector).StartPointPosition = (n1 as LineConnector).sp;
                                        (n1 as LineConnector).EndPointPosition = (n1 as LineConnector).ep;
                                        (n1 as LineConnector).InvalidateConnectorPathGeometry();
                                    }
                                }
                                (n as Group).ReleaseStylusCapture();
                            }
                            if (n is Node)
                            {
                                (n as Node).OffsetY = (n as Node).y;
                                (n as Node).OffsetX = (n as Node).x;
                                (n as Node).ReleaseStylusCapture();
                            }
                            if (n is LineConnector)
                            {
                                (n as LineConnector).StartPointPosition = (n as LineConnector).sp;
                                (n as LineConnector).EndPointPosition = (n as LineConnector).ep;
                                (n as LineConnector).InvalidateConnectorPathGeometry();
                            }
                            Node.mouseup = true;
                        }
                        (this.dc.View.Page as DiagramPage).InvalidateMeasure();
                    }
                }
            }
        }
        double tempX;
        double tempY;
        private void setDragDelta(DragDeltaEventArgs e)
        {
            dragdelta = new Point(e.HorizontalChange, e.VerticalChange);
            if (dc.View.SnapToHorizontalGrid)
            {
                dragdelta.Y = verChanage;
            }
            if (dc.View.SnapToVerticalGrid)
            {
                dragdelta.X = horChanage;
            }
            if (dc.View.DirectionBehaviourEnabled == true)
            {
                tempX = horChanage;
                tempY = verChanage;
                if (horChanage < 0)
                    tempX = horChanage * -1;

                if (verChanage < 0)
                    tempY = verChanage * -1;

                if (!(dc.View.OnlyY))
                {
                    if (tempX > tempY)
                    {
                        dc.View.OnlyX = true;
                    }
                }

                if (!(dc.View.OnlyX))
                {
                    if (tempY > tempX)
                    {
                        dc.View.OnlyY = true;
                    }
                }
            }

            if (!dc.View._RunTimeMovement && !dc.View.DirectionBehaviourEnabled)
            {
                if (dc.View.TranslateRailsMode == TranslateRailsMode.TranslateRailsX)
                {
                    dc.View.OnlyX = true;
                }
                if (dc.View.TranslateRailsMode == TranslateRailsMode.TranslateRailsY)
                {
                    dc.View.OnlyY = true;
                }
            }

            if (dc.View.OnlyX == true)
            {
                dc.View.AllowMoveY = false;
            }

            if (dc.View.OnlyY == true)
            {
                dc.View.AllowMoveX = false;
            }
            
            if (this.rotateTransform != null)
            {
                Point pt = this.rotateTransform.Transform(new Point(e.HorizontalChange, e.VerticalChange));
                if (!dc.View.SnapToHorizontalGrid)
                {
                    dragdelta.Y = pt.Y;
                }
                if (!dc.View.SnapToVerticalGrid)
                {
                    dragdelta.X = pt.X;
                }
            }
            if (dc.View.AllowMoveX == false)
            {
                dragdelta.X = 0.0;
            }

            if (dc.View.AllowMoveY == false)
            {
                dragdelta.Y = 0.0;
            }
        }

        private Point getDelta(Node node, DragDeltaEventArgs e)
        {
            node.m_MouseMoving = true;
            setDragDelta(e);
            node.m_TempPosition.X += dragdelta.X;
            node.m_TempPosition.Y += dragdelta.Y;

            Point delta = new Point(node.m_TempPosition.X, node.m_TempPosition.Y);
            //delta.X = this.current.X - this.pre.X - horChanage;
            //delta.Y = this.current.Y - this.pre.Y - verChanage;
            double offsetX = node.PxOffsetX;
            double offsetY = node.PxOffsetY;
            double dx = dragdelta.X;
            double dy = dragdelta.Y;
            if (dc.View.SnapSettings != null)
            {
                dc.View.SnapSettings.Clear();
            }
            if (dc.View.SnapToHorizontalGrid)
            {
                double oldvalue = node.PxOffsetY;
                node.PxOffsetY = Node.Round(node.m_TempPosition.Y, dc.View.PxSnapOffsetY);
                //if (/*oldvalue != node.PxOffsetY&&*/dc.View.SnapSettings != null)
                //{
                //    dc.View.SnapSettings.dia_page = dc.View.Page as DiagramPage;
                //    if (Viewportnodes.Count > 0 && dc.View.SnapSettings.EnableSnapNode)
                //    {
                //        dc.View.SnapSettings.GetLinePoint(node, Viewportnodes, "Horizontal");
                //    }
                //}
            }
            else
            {
                node.PxOffsetY += dy;
            }
            if (dc.View.SnapToVerticalGrid)
            {
                double oldvalue = node.PxOffsetX;
                node.PxOffsetX = Node.Round(node.m_TempPosition.X, dc.View.PxSnapOffsetX);
                //if (/* != node.PxOffsetX &&*/ dc.View.SnapSettings != null)
                //{
                //    dc.View.SnapSettings.dia_page = dc.View.Page as DiagramPage;
                //    if (Viewportnodes.Count > 0 && dc.View.SnapSettings.EnableSnapNode)
                //    {
                //        dc.View.SnapSettings.GetLinePoint(node, Viewportnodes, "Vertical");
                //    }
                //}
            }
            else
            {
                node.PxOffsetX += dx;
            }

            if (/*oldvalue != node.PxOffsetY&&*/dc.View.SnapSettings != null)
            {
                dc.View.SnapSettings.dia_page = dc.View.Page as DiagramPage;
                if (Viewportnodes.Count > 0)// && dc.View.SnapSettings.EnableSnapNode)
                {
                    dc.View.SnapSettings.GetLinePoint(node, Viewportnodes, "Horizontal");
                }
            }

            if (/* != node.PxOffsetX &&*/ dc.View.SnapSettings != null)
            {
                dc.View.SnapSettings.dia_page = dc.View.Page as DiagramPage;
                if (Viewportnodes.Count > 0)// && dc.View.SnapSettings.EnableSnapNode)
                {
                    dc.View.SnapSettings.GetLinePoint(node, Viewportnodes, "Vertical");
                }
            }

            delta.X = delta.X - node.PxOffsetX;
            delta.Y = delta.Y - node.PxOffsetY;
            if (delta.X > 0)
            {

            }
            if (delta.Y > 0)
            {

            }
            if (!dc.View.SnapToHorizontalGrid)
            {
                delta.Y = 0;
            }
            if (!dc.View.SnapToVerticalGrid)
            {
                delta.X = 0;
            }
            return delta;
        }
   
        /// <summary>
        /// Drags the group.
        /// </summary>
        /// <param name="dc">The DiagramControl object.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        /// <param name="g">The group.</param>
        /// <param name="diagramPanel">The diagram panel.</param>
        private void DragGroup(DiagramControl dc, DragDeltaEventArgs e, Group g, IDiagramPage diagramPanel, Point delta)
        {
           

            CollectionExt gnodechildren = new CollectionExt();
            if (g is Layer)
            {
                foreach (INodeGroup item in (g as Layer).Nodes)
                {
                    if (!gnodechildren.Contains(item))
                    {
                        gnodechildren.Add(item);
                    }
                }
                foreach (INodeGroup item in (g as Layer).Lines)
                {
                    if (!gnodechildren.Contains(item))
                    {
                        gnodechildren.Add(item);
                    }
                }
            }
            else
            {
                gnodechildren = g.NodeChildren;
            }
            foreach (INodeGroup item in gnodechildren)
            {
                setDragDelta(e);

                if (item is Node)
                {
                    if (!(item as Node).IsSelected)
                    {
                        double offsetX = (item as Node).m_TempPosition.X;
                        double offsetY = (item as Node).m_TempPosition.Y;
                        if (double.IsNaN(offsetX))
                        {
                            offsetX = 0;
                        }

                        if (double.IsNaN(offsetY))
                        {
                            offsetY = 0;
                        }

                        if (!(item as Node).m_MouseMoving)
                        {
                            if ((item as Node).AllowMove)
                            {
                                (item as Node).m_TempPosition.X += dragdelta.X;
                                (item as Node).m_TempPosition.Y += dragdelta.Y;
                                (item as Node).PxOffsetX = (item as Node).m_TempPosition.X - delta.X;
                                (item as Node).PxOffsetY = (item as Node).m_TempPosition.Y - delta.Y;
                            }
                        }

                        else
                        {
                            (item as Node).m_MouseMoving = false;
                        }
                    }
                       
                }
                else
                    if (item is LineConnector)
                    {
                        if (!(item as LineConnector).IsSelected)
                        {
                            LineConnector lc = (item as LineConnector);
                            lc.m_TempStartPoint.X += dragdelta.X;
                            lc.m_TempEndPoint.X += dragdelta.X;
                            lc.m_TempStartPoint.Y += dragdelta.Y;
                            lc.m_TempEndPoint.Y += dragdelta.Y;

                            lc.PxStartPointPosition = new Point(lc.m_TempStartPoint.X - delta.X, lc.m_TempStartPoint.Y - delta.Y);
                            lc.PxEndPointPosition = new Point(lc.m_TempEndPoint.X - delta.X, lc.m_TempEndPoint.Y - delta.Y);

                            if (lc.IntermediatePoints != null)
                                for (int i = 0; i < lc.IntermediatePoints.Count; i++)
                                {
                                    lc.m_TempInerPts[i] = new Point(lc.m_TempInerPts[i].X + dragdelta.X, lc.m_TempInerPts[i].Y + dragdelta.Y);
                                    lc.IntermediatePoints[i] = new Point(lc.m_TempInerPts[i].X - delta.X, lc.m_TempInerPts[i].Y - delta.Y);
                                }
                            lc.InvalidateConnectorPathGeometry();
                        }
                    }

                }
            }
        #endregion
        }
    
}
