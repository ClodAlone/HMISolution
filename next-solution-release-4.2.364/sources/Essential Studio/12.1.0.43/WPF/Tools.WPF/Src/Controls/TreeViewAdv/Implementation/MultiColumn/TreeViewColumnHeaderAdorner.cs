// <copyright file="TreeViewColumnHeaderAdorner.cs" company="Syncfusion">
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
    /// Class represents the Tree View column header Adorner
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TreeViewColumnHeaderAdorner : TemplatedAdornerBase
    {
        #region Initialize/Finalize methods

        /// <summary>
        /// Initializes static members of the <see cref="TreeViewColumnHeaderAdorner"/> class.
        /// </summary>
        static TreeViewColumnHeaderAdorner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeViewColumnHeaderTemplatedAdornerInternalControl), new FrameworkPropertyMetadata(typeof(TreeViewColumnHeaderAdorner)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewColumnHeaderAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">adorned Element</param>
        public TreeViewColumnHeaderAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            Loaded += new RoutedEventHandler(TreeViewColumnHeaderAdorner_Loaded);
        }

        #endregion Initialize/Finalize methods

        #region Implementation

        /// <summary>
        /// Handles the Loaded event of the TreeViewColumnHeaderAdorner control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void TreeViewColumnHeaderAdorner_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = AdornedElement as FrameworkElement;

            if (element != null)
            {
                InnerControl.SetBinding(FrameworkElement.WidthProperty, Binder.Bind(element, "ActualWidth"));
                InnerControl.SetBinding(FrameworkElement.HeightProperty, Binder.Bind(element, "ActualHeight"));
            }
        }

        #endregion Implementation
    }

    /// <summary>
    /// Class represents the TreeViewColumnHeader Template Adorner (InternalControl)
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TreeViewColumnHeaderTemplatedAdornerInternalControl : TemplatedAdornerInternalControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewColumnHeaderTemplatedAdornerInternalControl"/> class.
        /// </summary>
        /// <param name="adorner">Given adorner.</param>
        public TreeViewColumnHeaderTemplatedAdornerInternalControl(TemplatedAdornerBase adorner)
            : base(adorner)
        {
        
        }
    }
}