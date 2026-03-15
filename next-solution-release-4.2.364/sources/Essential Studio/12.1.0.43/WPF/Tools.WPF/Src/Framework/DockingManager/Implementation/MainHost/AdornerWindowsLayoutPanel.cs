// <copyright file="AdornerWindowsLayoutPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the class for the Adorner window layout panel
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class AdornerWindowsLayoutPanel : Panel
    {
        #region Class members
        /// <summary>
        /// specify m_items.
        /// </summary>
        private ObservableFrameworkElements m_items;

        /// <summary>
        /// specify adorner layer.
        /// </summary>
        private AdornerLayer m_adornerLayer;

        /// <summary>
        /// specify UI Element adorner.
        /// </summary>
        private UIElementAdorner m_dockingAdorner;

        /// <summary>
        /// specify docking manager.
        /// </summary>
        private DockingManager m_dockingManager;
        #endregion

        #region Class propertis
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public ObservableFrameworkElements Items
        {
            get
            {
                return m_items;
            }
        }
        #endregion

        #region DP getters / setters
        /// <summary>
        /// Gets the placement rectangle
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>return rect value.</returns>
        public static Rect GetPlacementRactangle(DependencyObject obj)
        {
            return (Rect)obj.GetValue(AdornerWindowsLayoutPanel.PlacementRectangleProperty);
        }

        /// <summary>
        /// Sets the placement rectangle
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value.</param>
        public static void SetPlacementRactangle(DependencyObject obj, Rect value)
        {
            obj.SetValue(AdornerWindowsLayoutPanel.PlacementRectangleProperty, value);
        }
        #endregion

        #region Initializatioon
        /// <summary>
        /// Initializes a new instance of the <see cref="AdornerWindowsLayoutPanel"/> class.
        /// </summary>
        /// <param name="dockingManager">The docking manager.</param>
        public AdornerWindowsLayoutPanel(DockingManager dockingManager)
        {
            m_dockingManager = dockingManager;

            m_items = new ObservableFrameworkElements();
            m_adornerLayer = AdornerLayer.GetAdornerLayer(dockingManager);
            m_dockingAdorner = new UIElementAdorner(dockingManager as UIElement, this);

            if (m_adornerLayer != null)
            {
                m_adornerLayer.Add(m_dockingAdorner);
            }

            Items.CollectionChanged += new NotifyCollectionChangedEventHandler(OnItemsCollectionChanged);
        }

        /// <summary>
        /// Called when [items collection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddVisualItems(e.NewItems);
                    InvalidateMeasure();
                    InvalidateArrange();
                    InvalidateVisual();
                    break;

                case NotifyCollectionChangedAction.Remove:
                    RemoveVisualItems(e.OldItems);
                    InvalidateVisual();
                    break;

                case NotifyCollectionChangedAction.Reset:

                    break;
            }
        }

        /// <summary>
        /// Gets the number of child <see cref="T:System.Windows.Media.Visual"/> objects in this instance of <see cref="T:System.Windows.Controls.Panel"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of child <see cref="T:System.Windows.Media.Visual"/> objects.
        /// </returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return Items.Count;
            }
        }

        /// <summary>
        /// Gets a <see cref="T:System.Windows.Media.Visual"/> child of this <see cref="T:System.Windows.Controls.Panel"/> at the specified index position.
        /// </summary>
        /// <param name="index">The index position of the <see cref="T:System.Windows.Media.Visual"/> child.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Media.Visual"/> child of the parent <see cref="T:System.Windows.Controls.Panel"/> element.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return Items[index];
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (FrameworkElement item in Items)
            {
                item.Arrange(AdornerWindowsLayoutPanel.GetPlacementRactangle(item));
            }

            return finalSize;
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Adds the visual items.
        /// </summary>
        /// <param name="items">The items.</param>
        private void AddVisualItems(IList items)
        {
            if (items != null)
            {
                foreach (FrameworkElement item in items)
                {
                    AddVisualChild(item);
                }
            }
        }

        /// <summary>
        /// Removes the visual items.
        /// </summary>
        /// <param name="items">The items.</param>
        private void RemoveVisualItems(IList items)
        {
            if (items != null)
            {
                foreach (FrameworkElement item in items)
                {
                    RemoveVisualChild(item);
                }
            }
        }

        /// <summary>
        /// Called when [placement rectangle changed].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnPlacementRectangleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AdornerFloatWindow window = d as AdornerFloatWindow;

            if (window != null)
            {
                DockingManager owner = window.Parent as DockingManager;

                if (owner != null)
                {
                    AdornerWindowsLayoutPanel panel = owner.AdornerWindowsLayoutPanel;

                    if (panel != null)
                    {
                        window.Arrange(AdornerWindowsLayoutPanel.GetPlacementRactangle(window));
                    }
                }
            }
        }

        /// <summary>
        /// Sets the on top.
        /// </summary>
        /// <param name="adornerFloatWindow">The adorner float window.</param>
        internal void SetOnTop(AdornerFloatWindow adornerFloatWindow)
        {
            int index = Items.IndexOf(adornerFloatWindow);

            if (index < Items.Count - 1)
            {
                ////Items.Move( index, Items.Count - 1 );
                Items.Remove(adornerFloatWindow);
                Items.Add(adornerFloatWindow);

                foreach (FrameworkElement item in Items)
                {
                    RemoveVisualChild(item);
                    AddVisualChild(item);
                }

                InvalidateVisual();
            }
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Represents the PlacementRectangle DependencyProperty
        /// </summary>
        public static DependencyProperty PlacementRectangleProperty =
            DependencyProperty.RegisterAttached("PlacementRectangle", typeof(Rect), typeof(AdornerWindowsLayoutPanel), new PropertyMetadata(Rect.Empty, new PropertyChangedCallback(OnPlacementRectangleChanged)));
        #endregion
    }
}
