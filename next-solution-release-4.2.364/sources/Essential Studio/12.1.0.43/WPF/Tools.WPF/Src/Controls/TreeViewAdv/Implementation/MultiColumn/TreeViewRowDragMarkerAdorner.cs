// <copyright file="TreeViewRowDragMarkerAdorner.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

#region file using

using System.Windows;
using Syncfusion.Windows.Shared;

#endregion file using

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the class for TreeViewRowDragmarker Adorner
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TreeViewRowDragMarkerAdorner : TemplatedAdornerBase
    {
        #region Initialize/Finalize methods

        /// <summary>
        /// Initializes static members of the <see cref="TreeViewRowDragMarkerAdorner"/> class.
        /// </summary>
        static TreeViewRowDragMarkerAdorner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeViewRowDragMarkerTemplatedAdornerInternalControl), new FrameworkPropertyMetadata(typeof(TreeViewRowDragMarkerAdorner)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewRowDragMarkerAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">adorned Element</param>
        public TreeViewRowDragMarkerAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            Loaded += new RoutedEventHandler(TreeViewRowDragMarkerAdorner_Loaded);
        }

        #endregion Initialize/Finalize methods

        #region Implementation

        /// <summary>
        /// Handles the Loaded event of the TreeViewRowDragMarkerAdorner control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void TreeViewRowDragMarkerAdorner_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = AdornedElement as FrameworkElement;

            if (element != null)
            {
                InnerControl.SetBinding(FrameworkElement.HeightProperty, Binder.Bind(element, "ActualHeight"));
            }
        }

        #endregion Implementation
    }

    /// <summary>
    /// Represents the class for TreeView Row DragMarker Template Adorner InternalControl
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TreeViewRowDragMarkerTemplatedAdornerInternalControl : TemplatedAdornerInternalControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewRowDragMarkerTemplatedAdornerInternalControl"/> class.
        /// </summary>
        /// <param name="adorner">The adorner.</param>
        public TreeViewRowDragMarkerTemplatedAdornerInternalControl(TemplatedAdornerBase adorner)
            : base(adorner)
        {
        
        }
    }
}