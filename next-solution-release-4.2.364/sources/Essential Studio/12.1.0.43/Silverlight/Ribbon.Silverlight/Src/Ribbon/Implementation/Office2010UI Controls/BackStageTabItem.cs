#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents BackStageTabItem class.
    /// </summary>
    public class BackstageTabItem : ContentControl,IBackStageColor
    {
        #region Properties

        /// <summary>
        /// Gets or sets whether the tab is selected
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(BackstageTabItem), new PropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));



        /// <summary>
        /// Called when [is selected changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BackstageTabItem source = (BackstageTabItem)d;
            if (source.IsSelected)
                source.BackStageParent.SelectedItem = source;
            source.UpdateVisualState(source);
            source.HandleSingleSelectedItem();
        }

        /// <summary>
        /// Gets parent Back Stage Element.
        /// </summary>
        internal Backstage BackStageParent
        {
            get
            {
                return (ItemsControl.ItemsControlFromItemContainer(this) as Backstage);
            }
        }

        /// <summary>
        /// Gets or sets tab items text
        /// </summary>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }



        /// <summary>
        /// Gets or Sets Tab Item Text field. It is a dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object),
            typeof(BackstageTabItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets a value indicating whether the mouse pointer is located over this element (including child elements in the visual tree).
        /// </summary>
        /// <value></value>
        /// <returns>true if mouse pointer is over the element or its child elements; otherwise, false. The default is false.</returns>
        public bool IsMouseOver
        {
            get { return (bool)GetValue(IsMouseOverProperty); }
            set { SetValue(IsMouseOverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMouseOver.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsMouseOverProperty =
            DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(BackstageTabItem), new PropertyMetadata(false, new PropertyChangedCallback(OnIsMouseOverChanged)));


        /// <summary>
        /// Called when [is mouse over changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsMouseOverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BackstageTabItem source = (BackstageTabItem)d;
            source.UpdateVisualState(source);
        }

        /// <summary>
        /// 
        /// </summary>
        public Brush BackStageColor
        {
            get { return (Brush)GetValue(BackStageColorProperty); }
            set { SetValue(BackStageColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackStageColor.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty BackStageColorProperty =
            DependencyProperty.Register("BackStageColor", typeof(Brush), typeof(BackstageTabItem), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));


        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static BackstageTabItem()
        {

        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BackstageTabItem()
        {
            this.DefaultStyleKey = typeof(BackstageTabItem);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Called when the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property changes.
        /// </summary>
        /// <param name="oldContent">The old value of the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property.</param>
        /// <param name="newContent">The new value of the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (IsSelected && BackStageParent != null)
            {
                BackStageParent.SelectedTabContent = newContent;
            }
        }


        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> routed event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!IsSelected)
            {
                if (BackStageParent != null)
                {
                    BackStageParent.SelectedItem = this;
                }
                IsSelected = true;
                HandleSingleSelectedItem();
            }
            e.Handled = true;
        }

        /// <summary>
        /// Handles the single selected item.
        /// </summary>
        private void HandleSingleSelectedItem()
        {
            foreach (var item in this.BackStageParent.Items)
            {
                if (item is BackstageTabItem)
                {
                    BackstageTabItem backStageItem = (BackstageTabItem)item;

                    if (backStageItem != this.BackStageParent.SelectedItem)
                    {
                        backStageItem.IsSelected = false;
                        UpdateVisualState(backStageItem);
                    }
                }
            }
        }

        #endregion


        #region Event handling

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseEnter"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.IsMouseOver = true;
            UpdateVisualState(this);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeave"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.IsMouseOver = false;
            UpdateVisualState(this);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateVisualState(this);
        }


        /// <summary>
        /// Updates the state of the visual.
        /// </summary>
        /// <param name="item">The item.</param>
        private void UpdateVisualState(BackstageTabItem item)
        {

            if (item.IsMouseOver && item.IsEnabled)
                VisualStateManager.GoToState(item, "IsMouseOverEnabled", true);
            else
                VisualStateManager.GoToState(item, "Normal", true);

            if (item.IsSelected)
                VisualStateManager.GoToState(item, "IsSelected", true);

            if (!item.IsEnabled)
                VisualStateManager.GoToState(item, "Disabled", true);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        public void OnBackStageColorChanged(Brush color)
        {
            BackStageColor = color;
        }
    }
}