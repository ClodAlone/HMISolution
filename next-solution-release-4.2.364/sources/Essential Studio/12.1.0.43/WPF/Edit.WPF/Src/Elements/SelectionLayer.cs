// <copyright file="SelectionLayer.cs" company="Syncfusion">
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
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Input;

namespace Syncfusion.Windows.Edit
{
    /// <summary>
    /// SelectionLayer class is used as an adorner for cursor. The adorner gets invalidated when the SelectionPath property is changed. 
    /// </summary>

#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    internal class SelectionLayer : Adorner
    {
        #region Dependency Properties

        /// <summary>
        /// Gets or sets the SelectionPath
        /// </summary>
        private static readonly DependencyProperty SelectionPathProperty = DependencyProperty.Register("SelectionPath", typeof(RectangleGeometry), typeof(SelectionLayer), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectionPathChanged)));

        #endregion

        #region DP Setter and Getters

        /// <summary>
        /// Gets or sets the SelectionPath
        /// </summary>
        public RectangleGeometry SelectionPath
        {
            get
            {
                return (RectangleGeometry)GetValue(SelectionPathProperty);
            }

            set
            {
                SetValue(SelectionPathProperty, value);
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionLayer"/> class.
        /// </summary>
        /// <param name="adornedElement">Gets the UIElement object from the reporting source</param>
        public SelectionLayer(UIElement adornedElement)
            : base(adornedElement) 
        {
            this.Cursor = Cursors.Arrow;
        }
        #endregion

        #region Events

       /// <summary>
        /// Gets called when SelectionPath property of the SelectionLayer gets changed. Also it triggers
        /// the SelectionPathChanged Event of the SelectionLayer.
        /// </summary>
        /// <param name="d">DepdendencyObject, returns SelectionLayer</param>
        /// <param name="e">DependencyPropertyChangedEventArgs, returns old and new
        /// value</param>
        private static void OnSelectionPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SelectionLayer layer = (SelectionLayer)d;
            layer.InvalidateVisual();
        }

        #endregion

        #region Overrides

        /// <summary>
        /// override to perform operations when OnRender
        /// </summary>
        /// <param name="drawingContext">Gets the DrawingContext object from the reporting source</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (SelectionPath != null)
            {
                Brush brush = (Brush)new BrushConverter().ConvertFromString("#77A7A7A7");
                drawingContext.DrawGeometry(brush, new Pen(brush, 0d), SelectionPath);
            }
        }
        #endregion
    }
}
