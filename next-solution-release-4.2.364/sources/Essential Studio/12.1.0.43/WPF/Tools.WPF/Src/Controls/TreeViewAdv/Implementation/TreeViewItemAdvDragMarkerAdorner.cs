// <copyright file="TreeViewItemAdvDragMarkerAdorner.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

#region file using

using System.Windows;
using System.Windows.Documents;
using Syncfusion.Windows.Shared;

#endregion file using

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Class is used to create the visual adorner for
    /// drag marker of the <see cref="TreeViewItemAdv"/> when it is dragged.
    /// </summary>
    /// <list type="table">
    /// <listheader>
    /// <term>Help Page</term>
    /// <description>Syntax</description>
    /// </listheader>
    /// <example>
    /// <list type="table">
    /// <listheader>
    /// <description>C#</description>
    /// </listheader>
    /// <example><code>public class TreeViewItemAdvDragMarkerAdorner : <see cref="Adorner"/></code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example>
    /// <code language="XAML">
    /// You cannot use this managed class in XAML.
    /// </code>
    /// </example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// An class that provides functionality
    /// for virtualizing <see cref="TreeViewItemAdv"/> and shows fake items.
    /// </remarks>
    /// <seealso cref="TreeViewAdv"/>
    /// <seealso cref="TreeViewItemAdv"/>
    /// <seealso cref="Adorner"/>
    /// <exclude/>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TreeViewItemAdvDragMarkerAdorner : TemplatedAdornerBase
    {
        #region Initialize/Finalize methods

        /// <summary>
        /// Initializes the <see cref="TreeViewItemAdvDragMarkerAdorner"/> class.
        /// </summary>
        static TreeViewItemAdvDragMarkerAdorner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeViewItemAdvDragMarkerAdornerrInternalControl), new FrameworkPropertyMetadata(typeof(TreeViewItemAdvDragMarkerAdorner)));
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="adornedElement">Element to be adorned.</param>
        public TreeViewItemAdvDragMarkerAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            Loaded += new RoutedEventHandler(TreeViewItemAdvDragMarkerAdorner_Loaded);
        }

        #endregion Initialize/Finalize methods

        #region Implementation

        /// <summary>
        /// Occurs when the element is laid out, rendered, and ready for interaction.
        /// </summary>
        private void TreeViewItemAdvDragMarkerAdorner_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = AdornedElement as FrameworkElement;

            if (element != null)
            {
                InnerControl.SetBinding(FrameworkElement.WidthProperty, Binder.Bind(element, "ActualWidth"));
            }
        }

        /// <summary>
        /// Sets style for InnerControl.
        /// </summary>
        protected internal void SetStyle(Style style)
        {
            InnerControl.Style = style;
        }

        #endregion Implementation
    }

#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    /// <summary>
    /// Class for TreeViewItemAdvDragMarkerAdornerrInternalControl
    /// </summary>
    public class TreeViewItemAdvDragMarkerAdornerrInternalControl : TemplatedAdornerInternalControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewItemAdvDragMarkerAdornerrInternalControl"/> class.
        /// </summary>
        /// <param name="adorner">Given adorner.</param>
        public TreeViewItemAdvDragMarkerAdornerrInternalControl(TemplatedAdornerBase adorner)
            : base(adorner)
        {
        }
    }
}