// <copyright file="RotateOriginPointer.cs" company="Syncfusion">
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
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents the rotate pointer used for rotation.
    /// </summary>
    #if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    public class RotateOriginPointer : Thumb
    {
        #region Class fields
        /// <summary>
        /// Used to store the rotate pointer object.
        /// </summary>
        private Border m_rotatePointer;

        /// <summary>
        /// Used to store the origin point end position
        /// </summary>
        private Point endpoint;

        /// <summary>
        /// Used to store the origin point previous position
        /// </summary>
        private Point previousOriginPoint;

        /// <summary>
        /// Used to store the node which contains this pointer.
        /// </summary>
        private Node node;

        /// <summary>
        /// Used to store the page instance.
        /// </summary>
        private DiagramPage diagramPage;

        /// <summary>
        /// Used to store the DiagramControl instance.
        /// </summary>
        private DiagramControl dc;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="RotateOriginPointer"/> class.
        /// </summary>
        static RotateOriginPointer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RotateOriginPointer), new FrameworkPropertyMetadata(typeof(RotateOriginPointer)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RotateOriginPointer"/> class.
        /// </summary>
        public RotateOriginPointer()
        {
            DragStarted += new DragStartedEventHandler(RotateOriginPointer_DragStarted);
            DragDelta += new DragDeltaEventHandler(RotateOriginPointer_DragDelta);
            DragCompleted += new DragCompletedEventHandler(RotateOriginPointer_DragCompleted);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Invoked whenever application code or internal processes call
        /// <see cref="System.Windows.FrameworkElement.ApplyTemplate"/> method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.RotatePointer = GetTemplateChild("RotateOriginPointerStyle") as Border;
            dc = DiagramPage.GetDiagramControl((FrameworkElement)this);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the rotate pointer position .
        /// </summary>
        /// <value>
        /// Type: <see cref="Point"/>
        /// RotatePointer's position.
        /// </value>
        internal Point PreviousOriginPoint
        {
            get
            {
                return previousOriginPoint;
            }

            set
            {
                if (previousOriginPoint != value)
                {
                    previousOriginPoint = value;
                }
            }
        }

        /// <summary>
        ///  Gets or sets the rotate pointer  .
        /// </summary>
        /// <value>
        /// Type: <see cref="Border"/>
        /// RotatePointer.
        /// </value>
        internal Border RotatePointer
        {
            get
            {
                return m_rotatePointer;
            }

            set
            {
                if (m_rotatePointer != value)
                {
                    m_rotatePointer = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the rotate pointer's current position with respect to the node.
        /// </summary>
        /// <value>
        /// Type: <see cref="Point"/>
        /// RotatePointer's current position.
        /// </value>
        internal Point CurrentOriginPoint
        {
            get
            {
                return endpoint;
            }

            set
            {
                if (endpoint != value)
                {
                    endpoint = value;
                }
            }
        }
        
        #endregion

        #region Implementation

        /// <summary>
        /// Handles the DragStarted event of the RotateOriginPointer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragStartedEventArgs"/> instance containing the event data.</param>
        private void RotateOriginPointer_DragStarted(object sender, DragStartedEventArgs e)
        {
            if (dc.View.IsPageEditable)
            {
                this.PreviousOriginPoint = Mouse.GetPosition(this);
            }
        }

        /// <summary>
        /// Handles the DragDelta event of the RotateOriginPointer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
      private void RotateOriginPointer_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (dc.View.IsPageEditable)
            {
                node = this.DataContext as Node;
                    CurrentOriginPoint = Mouse.GetPosition(this.DataContext as Node);
                    this.diagramPage = VisualTreeHelper.GetParent(node) as DiagramPage;
                    TranslateTransform tr = new TranslateTransform((CurrentOriginPoint.X - node.Width / 2), (CurrentOriginPoint.Y - node.Height / 2));
                    this.RotatePointer.RenderTransform = tr;
            }
        }

       /// <summary>
       /// Handles the DragCompleted event of the RotateOriginPointer control.
       /// </summary>
       /// <param name="sender">The source of the event.</param>
       /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragCompletedEventArgs"/> instance containing the event data.</param>
       private void RotateOriginPointer_DragCompleted(object sender, DragCompletedEventArgs e)
       {
       }

        #endregion
    }
}
