// <copyright file="NodeSelectionAdorner.cs" company="Syncfusion">
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
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents the adorner used for node selection.
    /// </summary>
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
    public class NodeSelectionAdorner : Adorner
    {
        #region Class variables

        /// <summary>
        /// Used to store the start point.
        /// </summary>
        private Point? m_startposition;
     
        /// <summary>
        /// Used to store the start point.
        /// </summary>
        private Point? m_endposition;
      
        /// <summary>
        /// Used to store the pen.
        /// </summary>
        private Pen selectionPen;
       
        /// <summary>
        /// Used to store the Page instance..
        /// </summary>
        private Control m_diagramPanel;
        
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeSelectionAdorner"/> class.
        /// </summary>
        /// <param name="diagramPanel">The diagram panel.</param>
        /// <param name="dragStartPoint">The drag start point.</param>
        public NodeSelectionAdorner(Control diagramPanel, Point? dragStartPoint)
            : base(diagramPanel as UIElement)
        {
            this.m_diagramPanel = diagramPanel;
            this.m_startposition = dragStartPoint;
            selectionPen = new Pen(Brushes.Black, 1);
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.m_diagramPanel);
            adornerLayer.IsHitTestVisible = true;
        }
        #endregion

        #region Overrides
      
        /// <summary>
        /// Provides class handling for the MouseMove routed event that occurs when the mouse 
        /// pointer  is over this control.
        /// </summary>
        /// <param name="e">The MouseEventArgs</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            this.ClipToBounds = true;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ////Get the current position as end position.
                Point temp = e.GetPosition(this);// ((m_diagramPanel as DiagramView).Scrollviewer);
                Rect r = new Rect((m_diagramPanel as DiagramView).Scrollviewer.TranslatePoint(new Point(0, 0), this), (m_diagramPanel as DiagramView).Scrollviewer.RenderSize);
               
                if (temp.X < r.Left)
                {
                    temp.X = r.Left;
                }
                else if (temp.X > r.Right)
                {
                    temp.X = r.Right;
                }
                if (temp.Y < r.Top)
                {
                    temp.Y = r.Top;
                }
                else if (temp.Y > r.Bottom)
                {
                    temp.Y = r.Bottom;
                }
                m_endposition = temp;// e.GetPosition(this);

                if (!this.IsMouseCaptured &&!(m_diagramPanel as DiagramView).IsPanEnabled)
                {
                    this.CaptureMouse();
                }
                
                InvalidateSelection();
                this.InvalidateVisual();
            }
            else
            {
                if (this.IsMouseCaptured&&!(m_diagramPanel as DiagramView).IsPanEnabled)
                {
                    this.ReleaseMouseCapture();
                }
                //AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.m_diagramPanel);
                //if (adornerLayer != null)
                //{
                //    adornerLayer.Remove(this);

                //}
            }

            e.Handled = true;
        }

        /// <summary>
        /// Provides class handling for the MouseUp routed event that occurs when the mouse 
        /// button is released while the mouse pointer is over this control.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs</param>
        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                this.ReleaseMouseCapture();
            }

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.m_diagramPanel);
            if (adornerLayer != null)
            {
                adornerLayer.Remove(this);
                
            }
            adornerLayer.IsHitTestVisible = false;
           e.Handled = true;
        }

        /// <summary>
        /// Calls render of the NodeSelectionAdorner.
        /// </summary>
        /// <param name="dc">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));
            if (this.m_startposition.HasValue && this.m_endposition.HasValue)
            {
                Rect rect = new Rect(this.m_startposition.Value, this.m_endposition.Value);
                dc.DrawRectangle(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#22000000")), selectionPen, rect);
            }
        }


        #endregion

        #region Methods

      /// <summary>
      /// Updates selection.
      /// </summary>
        private void InvalidateSelection()
        {
           
            Rect selectedArea = new Rect(m_startposition.Value, m_endposition.Value);
            
            foreach (FrameworkElement item in (m_diagramPanel as DiagramView).Page.Children.OfType<ICommon>())
            {
                Rect itemRect = VisualTreeHelper.GetDescendantBounds(item);
                if (item is Node)
                {
                    itemRect = new Rect(0, 0, item.ActualWidth, item.ActualHeight);
                }
             
                if (item is LineConnector)
                {
                    itemRect = (item as LineConnector).GetBounds();
                }
                Rect itemBounds = item.TransformToAncestor((m_diagramPanel as DiagramView)).TransformBounds(itemRect);
                if (selectedArea.Contains(itemBounds))
                {
                    if (item is IEdge)
                    {
                        IEdge line = item as IEdge;
                        if ((line as LineConnector).AllowSelect)
                        {
                            (((m_diagramPanel as DiagramView).Page) as IDiagramPage).SelectionList.Add(item as ICommon);
                        }
                    }
                    else
                    {
                        IShape node = item as IShape;
                        if (node.ParentID == Guid.Empty && (node as Node).AllowSelect)
                        {
                            (((m_diagramPanel as DiagramView).Page) as IDiagramPage).SelectionList.Add(node);
                        }
                    }
                }
                else
                {
                    if (item is IEdge)
                    {
                        (((m_diagramPanel as DiagramView).Page) as IDiagramPage).SelectionList.Remove(item as ICommon);
                    }
                    else
                    {
                        IShape node = item as IShape;
                        if (node.ParentID == Guid.Empty)
                        {
                            (((m_diagramPanel as DiagramView).Page) as IDiagramPage).SelectionList.Remove(node);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
