// <copyright file="RotateThumb.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents the Thumb used for rotating the <see cref="Node"/>.
    /// </summary>
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
    public class RotateThumb : Thumb
    {
        #region Class fields

        /// <summary>
        /// Used to store the grouped nodes.
        /// </summary>
        private CollectionExt gnodechildren;

        /*
        /// <summary>
        /// Used to store the render transform origin
        /// </summary>
        //private double x, y;
        */

        /// <summary>
        /// Used to store the cursor used.
        /// </summary>
        private Cursor m_cursor;

        /*
        /// <summary>
        /// Used to store the center point.
        /// </summary>
        //private Point centerPoint;

        /// <summary>
        /// Used to store the start vector
        /// </summary>
        //private Vector initialVector;

        /// <summary>
        /// Used to store the start angle.
        /// </summary>
        // This variable is no longer used
        //private double startAngle;
        */

        /// <summary>
        /// Used to store the page.
        /// </summary>
        private DiagramPage diagramPage;

        /*
        /// <summary>
        /// Used to store the rotate transform object.
        /// </summary>
        //private RotateTransform rotTransform;
        */

        /// <summary>
        /// Used to store the node.
        /// </summary>
        private Node node;

        /// <summary>
        /// Used to store the diagram control instance.
        /// </summary>
        private DiagramControl dc;

        /// <summary>
        /// Used to store the rotate angle.
        /// </summary>
        private double rotateangle = 0;

        /*
        /// <summary>
        /// Used To temporarily Store the node angle.
        /// </summary>
        //private double temp_nodeangle;
        */

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="RotateThumb"/> class.
        /// </summary>
        public RotateThumb()
        {
            DragDelta += new DragDeltaEventHandler(this.RotateThumb_DragDelta);
            DragStarted += new DragStartedEventHandler(this.RotateThumb_DragStarted);
            DragCompleted += new DragCompletedEventHandler(RotateThumb_DragCompleted);
        }

        #endregion

        #region Overrides


        
        /// <summary>
        /// Raises the <see cref="OnInitialized"/> event. 
        /// This method is invoked whenever <see cref="OnInitialized"/> property is set to true internally. 
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            dc = DiagramPage.GetDiagramControl((FrameworkElement)this);
            base.OnInitialized(e);
            if (BrowserInteropHelper.IsBrowserHosted)
            {
                this.Cursor = Cursors.UpArrow;
            }
            else
            {
                Assembly ass = Assembly.GetExecutingAssembly();
                Stream stream = ass.GetManifestResourceStream("Syncfusion.Windows.Diagram.Icons.Rotate.cur");
                m_cursor = new Cursor(stream);
                this.Cursor = m_cursor;
            }

            node = this.DataContext as Node;
        }
        #endregion

        #region Implementation

        Point pivot;
        double zeroError;

        /// <summary>
        /// Handles the DragStarted event of the RotateThumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragStartedEventArgs"/> instance containing the event data.</param>
        private void RotateThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            pivot = node.TranslatePoint(new Point(this.ActualWidth / 2, this.ActualHeight / 2), node.Page);
            Point start = Mouse.GetPosition(node.Page);
            zeroError = ConnectorBase.findAngle(start, pivot) - node.RotateAngle;
            dc.View.IsRotating = true;
            //temp_nodeangle = node.RotateAngle;
            Node.rotatebycode = false;
            if (node.AllowRotate)
            {
                if (dc.View.IsPageEditable)
                {
                    if (node != null)
                    {
                        this.diagramPage = VisualTreeHelper.GetParent(node) as DiagramPage;
                        if (node is Group)
                        {
                            gnodechildren = new CollectionExt();
                            for (int i = 0; i < (node as Group).NodeChildren.Count; i++)
                            {
                                if (!((node as Group).NodeChildren[i] is LineConnector))
                                {
                                    gnodechildren.Add((node as Group).NodeChildren[i] as INodeGroup);
                                }
                            }

                            gnodechildren.Add(node);

                            //foreach (INodeGroup item in gnodechildren)
                            //{
                            //    if (this.diagramPage != null)
                            //    {
                            //        ////Get the current mouse position.
                            //        Point startPoint = Mouse.GetPosition(this.diagramPage);
                            //        if (!(item is LineConnector))
                            //        {
                            //            this.centerPoint = node.TranslatePoint(new Point(node.Width * x, node.Height * y), this.diagramPage);

                            //            this.initialVector = Point.Subtract(startPoint, this.centerPoint);

                            //            //this.rotTransform = (item as Node).RenderTransform as RotateTransform;
                            //            //if (this.rotTransform == null)
                            //            //{
                            //            //    (item as Node).RenderTransform = new RotateTransform(0);
                            //            //    this.startAngle = 0;
                            //            //}
                            //            //else
                            //            //{
                            //            //    this.startAngle = this.rotTransform.Angle;
                            //            //}
                            //        }
                            //        else
                            //        {
                            //            (item as LineConnector).Width = ((item as LineConnector).PxStartPointPosition.X + (item as LineConnector).PxEndPointPosition.X) / 2;
                            //            (item as LineConnector).Height = ((item as LineConnector).PxStartPointPosition.Y + (item as LineConnector).PxEndPointPosition.Y) / 2;

                            //            (item as LineConnector).RenderTransformOrigin = new Point(x, y);
                            //            this.centerPoint = node.TranslatePoint(new Point(node.Width * x, node.Height * y), this.diagramPage);

                            //            this.initialVector = Point.Subtract(startPoint, this.centerPoint);

                            //            //this.rotTransform = (item as LineConnector).RenderTransform as RotateTransform;
                            //            //if (this.rotTransform == null)
                            //            //{
                            //            //    (item as LineConnector).RenderTransform = new RotateTransform(0);
                            //            //    this.startAngle = 0;
                            //            //}
                            //            //else
                            //            //{
                            //            //    this.startAngle = this.rotTransform.Angle;
                            //            //}
                            //        }
                            //    }
                            //}
                        }
                        //else
                        //{
                        //    if (this.diagramPage != null)
                        //    {
                        //        ////Get the current mouse position.
                        //        Point startPoint = Mouse.GetPosition(this.diagramPage);
                        //        this.centerPoint = node.TranslatePoint(new Point(node.Width * x, node.Height * y), this.diagramPage);

                        //        this.initialVector = Point.Subtract(startPoint, this.centerPoint);

                        //        //this.rotTransform = node.RenderTransform as RotateTransform;
                        //        //if (this.rotTransform == null)
                        //        //{
                        //        //    node.RenderTransform = new RotateTransform(0);
                        //        //    this.startAngle = 0;
                        //        //}
                        //        //else
                        //        //{
                        //        //    this.startAngle = this.rotTransform.Angle;
                        //        //}
                        //    }
                        //}
                    }
                }
            }
        }

        /// <summary>
        /// Handles the DragDelta event of the RotateThumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void RotateThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Point current = Mouse.GetPosition(node.Page);
            double currentAngle = ConnectorBase.findAngle(current, pivot);
            double newRotateAngle = currentAngle - zeroError;

            if (node.AllowRotate)
            {
                if (dc.View.IsPageEditable)
                {
                    //dc.View.IsMouseUponly = false;
                    if (node is Group)
                    {
                        node.RotateAngle = newRotateAngle;
                        foreach (INodeGroup item in gnodechildren)
                        {
                            if (item is Node)
                            {
                                if (!(item as Node).AllowRotate)
                                {
                                    continue;
                                }
                            }
                            dc.View.Ispositionchanged = true;
                            if (!(item is LineConnector))
                            {
                                NodeRoutedEventArgs newEventArgs = new NodeRoutedEventArgs(item as Node);
                                newEventArgs.RoutedEvent = DiagramView.NodeRotationChangingEvent;
                                RaiseEvent(newEventArgs);
                                DiagramView.IsOtherEvent = true;
                            }

                            node = this.DataContext as Node;
                            if (this.diagramPage != null)
                            {
                                //////Get the position of the page.
                                //Point pageposition = Mouse.GetPosition(this.diagramPage);
                                //////Get the new vector after drag
                                //Vector dragVector = Point.Subtract(pageposition, this.centerPoint);
                                //////Find the angle between the two vectors
                                // rotateangle = Vector.AngleBetween(this.initialVector, dragVector);
                                //////Apply the rotate transform
                                if (!(item is LineConnector))
                                {
                                    //RotateTransform rotateTransform = (item as Node).RenderTransform as RotateTransform;
                                    //rotateTransform.Angle = this.startAngle + Math.Round(rotateangle, 0);
                                    
                                }
                                else
                                {
                                    RotateTransform rotateTransform = (item as LineConnector).RenderTransform as RotateTransform;
                                    //rotateTransform.Angle = this.startAngle + Math.Round(rotateangle, 0);
                                    rotateTransform.Angle = Math.Round(rotateangle, 0);
                                    ////(item as LineConnector).UpdateConnectorPathGeometry();
                                }

                                this.diagramPage.InvalidateMeasure();
                                e.Handled = true;
                            }
                        }
                    }
                    else
                    {
                        dc.View.Ispositionchanged = true;
                        NodeRoutedEventArgs newEventArgs = new NodeRoutedEventArgs(node);
                        newEventArgs.RoutedEvent = DiagramView.NodeRotationChangingEvent;
                        RaiseEvent(newEventArgs);
                        DiagramView.IsOtherEvent = true;
                        node = this.DataContext as Node;
                        if (this.diagramPage != null && node.IsSelected)
                        {
                            //////Get the position of the page.
                            //Point pageposition = Mouse.GetPosition(this.diagramPage);
                            //////Get the new vector after drag
                            //Vector dragVector = Point.Subtract(pageposition, this.centerPoint);
                            //////Find the angle between the two vectors
                            // rotateangle = Vector.AngleBetween(this.initialVector, dragVector);
                            //////Apply the rotate transform
                            //RotateTransform rotateTransform = node.RenderTransform as RotateTransform;
                            //rotateTransform.Angle = newRotateAngle;// this.startAngle + Math.Round(rotateangle, 0);
                            node.m_IsResizing = true;
                            node.RotateAngle = newRotateAngle;//rotateTransform.Angle;
                            this.diagramPage.InvalidateMeasure();
                            e.Handled = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the DragCompleted event of the RotateThumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragCompletedEventArgs"/> instance containing the event data.</param>
        private void RotateThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            //node.RotateAngle = temp_nodeangle;
            node.m_IsResizing = false;
            dc.View.IsRotating = false;
            if (node.AllowRotate)
            {
                //if (node.RotateAngle == this.startAngle)
                //{
                //    node.RotateAngle = this.startAngle + Math.Round(rotateangle, 0);
                //    if (node is Group)
                //    {
                //        dc.View.NodeRotateCount = (node as Group).NodeChildren.Count + 1;

                //        foreach (ICommon shape in (node as Group).NodeChildren)
                //        {
                //            if (shape is Node)
                //            {
                //                (shape as Node).RotateAngle = this.startAngle + Math.Round(rotateangle, 0);
                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    node.RotateAngle = this.startAngle;
                //}
                
                NodeRoutedEventArgs newEventArgs = new NodeRoutedEventArgs(node);
                newEventArgs.RoutedEvent = DiagramView.NodeRotationChangedEvent;
                RaiseEvent(newEventArgs);
                DiagramView.IsOtherEvent = true;
            }
        }

        #endregion
    }
}
