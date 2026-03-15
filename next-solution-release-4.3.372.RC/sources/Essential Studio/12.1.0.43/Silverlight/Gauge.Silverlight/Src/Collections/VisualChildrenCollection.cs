#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Gauge
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    ///  Collection of visual children.
    /// </summary>
    /// <typeparam name="T">object of any type</typeparam>
    public class VisualChildrenCollection<T> : ObservableCollection<T>
    {        
        #region Private members

        /// <summary>
        /// Parent of visual element.
        /// </summary>
        private Panel mvisualParent;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Parent of visual element.
        /// </summary>
        public Panel VisualParent
        {
            get
            {
                return this.mvisualParent;
            }

            set
            {
                this.mvisualParent = value;
            }
        }

        #endregion
        
        #region Implementation
        /// <summary>
        /// Updates the Panel children
        /// </summary>
        internal void UpdatePanelChildren()
        {
            foreach (T item in this.Items)
            {
                Control c = item as Control;
                if (c != null && this.mvisualParent != null)
                {
                    if (!this.mvisualParent.Children.Contains(c))
                    {
                        if (c.Parent != null)
                        {
                            (c.Parent as Panel).Children.Remove(c);
                        }
                        this.mvisualParent.Children.Add(c);
                    }
                }
            }
        }

        #endregion
        
        #region Overrides

        /// <summary>
        /// Notifies when the Collection is changed
        /// </summary>
        /// <param name="e">Provides the details of the event</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (this.mvisualParent != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (Control elem in e.NewItems)
                    {
                        if (!this.mvisualParent.Children.Contains(elem))
                        {
                            this.mvisualParent.Children.Add(elem);
                        }
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (Control elem in e.OldItems)
                    {
                        this.mvisualParent.Children.Remove(elem);
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    this.mvisualParent.Children.Clear();
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Collection of ranges.
    /// </summary>
    public class RangesCollection : VisualChildrenCollection<RangeBase>
    {
    }

    /// <summary>
    /// Collection of pointers.
    /// </summary>
    public class PointersCollection : VisualChildrenCollection<PointerBase>
    {
    }

    /// <summary>
    /// Collection of ticks.
    /// </summary>
    public class TicksCollection : VisualChildrenCollection<TickSetBase>
    {
    }

    /// <summary>
    /// Collection of linear pointers.
    /// </summary>
    public class LinearPointersCollection : VisualChildrenCollection<LinearPointer>
    {
    }
}
