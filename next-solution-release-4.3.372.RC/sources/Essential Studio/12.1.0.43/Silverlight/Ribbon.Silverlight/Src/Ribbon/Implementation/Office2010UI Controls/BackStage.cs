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
using System.Windows.Controls.Primitives;
using System.Windows;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Input;
using Syncfusion.Windows.Shared;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents BackStage class.
    /// </summary>
    public class Backstage : ItemsControl,IBackStageColor
    {
        #region Properties

        /// <summary>
        /// Gets or sets the content of the selected tab.
        /// </summary>
        /// <value>The content of the selected tab.</value>
        public object SelectedTabContent
        {
            get { return (object)GetValue(SelectedTabContentProperty); }
            set { SetValue(SelectedTabContentProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedTabContent.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedTabContentProperty =
            DependencyProperty.Register("SelectedTabContent", typeof(object), typeof(Backstage), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public BackstageTabItem SelectedItem
        {
            get { return (BackstageTabItem)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(BackstageTabItem), typeof(Backstage), new PropertyMetadata(null, OnSelectedItemChanged));

        /// <summary>
        /// Gets or sets the index of the current focused element.
        /// </summary>
        /// <value>The index of the current focused element.</value>
        public int CurrentFocusedElementIndex { get; set; }

        /// <summary>
        /// Gets or sets the index of the selected.
        /// </summary>
        /// <value>The index of the selected.</value>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedIndex.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Backstage), new PropertyMetadata(0, new PropertyChangedCallback(OnSelectedIndexChanged)));


        /// <summary>
        /// Called when [selected index changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Backstage source = (Backstage)d;
            source.CurrentFocusedElementIndex = source.SelectedIndex;
        }

        /// <summary>
        /// Called when [selected item changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Backstage source = (Backstage)d;
            source.FindSelectedItemIndex();
            source.UpdateSelectedTabItemContent();
        }

        /// <summary>
        /// 
        /// </summary>
        public Brush BackStageColor
        {
            get { return (Brush)GetValue(BackStageColorProperty); }
            set { SetValue(BackStageColorProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for BackStageColor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BackStageColorProperty =
            DependencyProperty.Register("BackStageColor", typeof(Brush), typeof(Backstage), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));


        /// <summary>
        /// Finds the index of the selected item.
        /// </summary>
        private void FindSelectedItemIndex()
        {
            this.SelectedIndex = this.ItemContainerGenerator.IndexFromContainer(this.SelectedItem);

            if (SelectedIndex == -1)
                this.SelectedIndex = this.Items.IndexOf(this.SelectedItem);
        }



        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static Backstage()
        {

        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Backstage()
        {
            this.DefaultStyleKey = typeof(Backstage);            
        }

        

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }


        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new BackstageTabItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// true if the item is (or is eligible to be) its own container; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return ((item is BackstageTabItem) || (item is BackStageCommandButton));
        }

        /// <summary>
        /// Updates the current selection when an item in the <see cref="T:System.Windows.Controls.Primitives.Selector"/> has changed
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if ((e.Action == NotifyCollectionChangedAction.Remove))
            {
                int startIndex = e.OldStartingIndex + 1;
                if (startIndex > base.Items.Count)
                {
                    startIndex = 0;
                }
                BackstageTabItem item = FindNextTabItem(startIndex, -1);
                if (item != null)
                {
                    item.IsSelected = true;
                    this.SelectedItem = item;
                }
            }
            if (this.SelectedItem == null)
                SelectFirstBackStageTabItem();
        }

        private void SelectFirstBackStageTabItem()
        {
            foreach (var item in this.Items)

                if (item != null && item is BackstageTabItem)
                {
                    (item as BackstageTabItem).IsSelected = true;
                    break;
                }
        }


        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> routed event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            e.Handled = true;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets the current selected tab item.
        /// </summary>
        /// <returns></returns>
        private BackstageTabItem GetCurrentSelectedTabItem()
        {
            object selectedItem = this.SelectedItem;

            if (selectedItem == null)
                return null;

            BackstageTabItem item = selectedItem as BackstageTabItem;
            if (item == null)
            {
                item = FindNextTabItem(this.SelectedIndex, 1);
                this.SelectedItem = item;
            }
            return item;
        }

        /// <summary>
        /// Finds the next tab item.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="direction">The direction.</param>
        /// <returns></returns>
        private BackstageTabItem FindNextTabItem(int startIndex, int direction)
        {
            if (direction != 0)
            {
                int index = startIndex;
                for (int i = 0; i < base.Items.Count; i++)
                {
                    index += direction;

                    if (index >= base.Items.Count)
                        index = 0;

                    else if (index < 0)
                        index = base.Items.Count - 1;

                    BackstageTabItem item2 = base.ItemContainerGenerator.ContainerFromIndex(index) as BackstageTabItem;

                    if (((item2 != null) && item2.IsEnabled) && (item2.Visibility == Visibility.Visible))
                        return item2;
                }
            }
            return null;
        }


        /// <summary>
        /// Updates the content of the selected tab item.
        /// </summary>
        private void UpdateSelectedTabItemContent()
        {
            if (this.SelectedIndex < 0)
                this.SelectedTabContent = null;
            else
            {
                BackstageTabItem selectedTabItem = this.GetCurrentSelectedTabItem();
                if (selectedTabItem != null)
                {
                    this.SelectedTabContent = selectedTabItem.Content;
                    UpdateLayout();
                }
            }
        }

        #endregion


        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.KeyDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            int count = this.Items.Count;
            int currentSelectedIndex = this.CurrentFocusedElementIndex;
            int index = this.CurrentFocusedElementIndex;
            do
            {
                if (e.Key == Key.Up)
                {
                    index = index - 1;
                    if (index < 0)
                        index = this.Items.Count - 1;
                }
                else if (e.Key == Key.Down)
                {
                    index = index + 1;
                    if (index >= this.Items.Count)
                        index = 0;
                }

                FrameworkElement element = ItemContainerGenerator.ContainerFromIndex(index) as FrameworkElement;
                if (element is BackStageCommandButton)
                {
                    BackStageCommandButton button = (BackStageCommandButton)element;
                    button.IsMouseOver = true;
                    this.CurrentFocusedElementIndex = index;
                    break;
                }

                else if (element is BackstageTabItem)
                {
                    HideAllMouseOverElements();
                    BackstageTabItem tabItem = (BackstageTabItem)element;
                    if (tabItem.IsSelected)
                        this.CurrentFocusedElementIndex = index;
                    tabItem.IsSelected = true;
                    break;
                }
            }

            while (index != currentSelectedIndex);

        }

        /// <summary>
        /// Hides all mouse over elements.
        /// </summary>
        private void HideAllMouseOverElements()
        {
            foreach (var item in this.Items)
                if (item is BackStageCommandButton)
                {
                    BackStageCommandButton button = (BackStageCommandButton)item;
                    if (button != null)
                        button.IsMouseOver = false;
                }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        public void OnBackStageColorChanged(Brush color)
        {
            BackStageColor = color;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IBackStageColor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        void OnBackStageColorChanged(Brush color);
    }
}