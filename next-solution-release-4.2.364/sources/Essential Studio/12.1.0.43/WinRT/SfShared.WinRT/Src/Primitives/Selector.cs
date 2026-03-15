// <copyright file="Selector.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows;
namespace Syncfusion.WP.Primitives
#else
#if SILVERLIGHT
using System.Windows.Controls;
using System.Windows;
namespace Syncfusion.Tools.Primitives
#else
#if WPF
using System.Windows.Controls;
using System.Windows;
namespace Syncfusion.Windows.Primitives
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
namespace Syncfusion.UI.Xaml.Primitives
#endif
#endif
#endif
{
    /// <summary>
    /// Represents a class for defining the selection properties.
    /// </summary>
    public class Selector : ItemsControl
    {

        #region Dependency Properties

        /// <summary>
        /// Gets or sets tthe index for the selecte item.
        /// </summary>
        /// <value>
        /// The default value is -1
        /// </value>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedIndex.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Selector), new PropertyMetadata(-1, new PropertyChangedCallback(OnSelectedIndexChanged)));

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(Selector), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemChanged)));

        #endregion

        #region Override Methods
        /// <summary>
        /// Initializes the SelectedItem property.
        /// </summary>
#if !WINRT
        public override void OnApplyTemplate()
#else
            protected override void OnApplyTemplate()
#endif
        {
            if (SelectedIndex >= 0)
            {
                SelectedItem = this.Items[SelectedIndex];
            }
            else
            {
                SelectedItem = null;
            }
            base.OnApplyTemplate();
        }

        #endregion

        #region Callback Methods

        /// <summary>
        /// Called when the Selector SelectedItem Property changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The DependencyPropertyChangedEventArgs instance containing the event data.</param>
        private static void OnSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Selector instance = obj as Selector;
            instance.OnSelectedItemChanged(args);
            instance.OnSelectionChanged(args);
        }

        /// <summary>
        /// Called when the Selector SelectedItem Property changed.
        /// </summary>
        /// <param name="args">The DependencyPropertyChangedEventArgs instance containing the event data.</param>
        protected void OnSelectedItemChanged(DependencyPropertyChangedEventArgs args)
        {
            if (SelectedItem != null)
            {
                if (SelectedItem.Equals(DataContext))
                {
                    SelectedIndex = ItemContainerGenerator.IndexFromContainer(SelectedItem as DependencyObject);
                }
                else
                {
                    SelectedIndex = Items.IndexOf(SelectedItem);
                }
            }
            else
                SelectedIndex = -1;           
        }

        /// <summary>
        /// Called when the Selector SelectedItem Property changed.
        /// </summary>
        /// <param name="args">The DependencyPropertyChangedEventArgs instance containing the event data.</param>
        protected virtual void OnSelectionChanged(DependencyPropertyChangedEventArgs args)
        {
#if !WPF
            if (SelectionChanged != null)
#endif
            {
#if !WINRT
                System.Collections.IList oldItems =new List<object>();
                System.Collections.IList newItems = new List<object>();    
#else
                IList<object> oldItems = new List<object>();
                IList<object> newItems = new List<object>();    
#endif
                oldItems.Add(args.OldValue);
                newItems.Add(args.NewValue);
#if WPF
                SelectionChangedEventArgs selectionargs = new SelectionChangedEventArgs(SelectionChangedEvent, oldItems, newItems);
                RaiseEvent(selectionargs);
#else
                SelectionChangedEventArgs selectionargs = new SelectionChangedEventArgs(oldItems, newItems);
                SelectionChanged(this, selectionargs);
#endif
            }            
        }

        /// <summary>
        /// Called when the Selector SelectedIndex Property changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The DependencyPropertyChangedEventArgs instance containing the event data.</param>
        private static void OnSelectedIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Selector instance = obj as Selector;
            instance.OnSelectedIndexChanged(args);
        }

        /// <summary>
        /// Called when the Selector SelectedIndex Property changed.
        /// </summary>
        /// <param name="args">The DependencyPropertyChangedEventArgs instance containing the event data.</param>
        protected void OnSelectedIndexChanged(DependencyPropertyChangedEventArgs args)
        {
            if (SelectedIndex >= 0 && SelectedIndex < this.Items.Count)
            {
                SelectedItem = this.Items[SelectedIndex];
            }
            else
            {
                SelectedItem = null;
            }           
        }

        #endregion       

        #region  Events
        /// <summary>
        /// Occurs when the selection has changed
        /// </summary>
#if WPF
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(Selector));

        /// <summary>
        /// add remove handlers
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }
#else
        public event SelectionChangedEventHandler SelectionChanged;
#endif
        #endregion
    }
}
