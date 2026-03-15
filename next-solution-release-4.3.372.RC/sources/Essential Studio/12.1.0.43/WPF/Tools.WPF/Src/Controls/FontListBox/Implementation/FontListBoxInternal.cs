// <copyright file="FontListBoxInternal.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Used for drawing of the <see cref="Syncfusion.Windows.Tools.Controls.FontListBox"/>.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class FontListBoxInternal : Selector
    {
        #region Private fields

        /// <summary>
        /// Index of the item over which is mouse.
        /// </summary>
        private int m_isMouseOverIndex = -1;

        /// <summary>
        /// Index of the first item that has focus.
        /// </summary>
        private int m_focusedItemIndex = -1;

        /// <summary>
        /// Used for prevention of inappropriate calls of OnMouseMove.
        /// </summary>
        private bool m_skip = false;

        #endregion Private fields

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="FontListBoxInternal"/> class. Overrides some properties from base class.
        /// </summary>
        static FontListBoxInternal()
        {
            SelectedItemProperty.OverrideMetadata(typeof(FontListBoxInternal), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemChanged)));
            FocusableProperty.OverrideMetadata(typeof(FontListBoxInternal), new FrameworkPropertyMetadata(true));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontListBoxInternal"/> class.
        /// </summary>
        public FontListBoxInternal()
        {
            this.FocusVisualStyle = null;
        }

        #endregion Initialization

        #region Implementation

        /// <summary>
        /// Invoked when <see cref="System.Windows.Controls.ItemsControl.ItemsSource"/> property changes.
        /// </summary>
        /// <param name="oldValue">Old value of the source.</param>
        /// <param name="newValue">New value of the source.</param>
        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            if (SelectedItem != null)
            {
                ContainerFromItem(SelectedItem).HasFocus = true;
            }
        }

        /// <summary>
        /// Invoked when <see cref="System.Windows.UIElement.GotFocus"/> event is raised.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (!IsMouseOver)
            {
                foreach (object obj in Items)
                {
                    if (ContainerFromItem(obj).HasFocus)
                    {
                        m_focusedItemIndex = Items.IndexOf(obj);
                        break;
                    }
                }
            }

            base.OnGotFocus(e);
        }

        /// <summary>
        /// Invoked when <see cref="E:System.Windows.Input.Mouse.MouseEnter"/>�attached event is raised.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            Focus();

            if (SelectedItem != null)
            {
                ContainerFromItem(SelectedItem).HasFocus = false;
            }

            base.OnMouseEnter(e);
        }

        /// <summary>
        /// Invoked when <see cref="E:System.Windows.Input.Mouse.MouseMove"/>�attached event is raised.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (m_skip == false)
            {
                FontListBoxInternalItem item = null;

                if (FocusedItem != null)
                {
                    item = ContainerFromItem(FocusedItem);
                    if (item != null)
                    {
                        item.HasFocus = false;
                    }
                }

                if (m_isMouseOverIndex != -1)
                {
                    item = ContainerFromIndex(m_isMouseOverIndex);
                    if (item != null)
                    {
                        item.HasFocus = false;
                    }
                }

                if (m_focusedItemIndex != -1)
                {
                    item = ContainerFromIndex(m_focusedItemIndex);
                    if (item != null)
                    {
                        item.HasFocus = false;
                    }
                }

                foreach (object obj in Items)
                {
                    FontListBoxInternalItem temp = ContainerFromItem(obj);
                    if (temp.IsMouseOver)
                    {
                        temp.HasFocus = true;
                        m_isMouseOverIndex = Items.IndexOf(obj);

                        FontFamilyRecord mouseOverItem = (FontFamilyRecord)Items[m_isMouseOverIndex];

                        if (FocusedItem == null)
                        {
                            FocusedItem = mouseOverItem;
                        }
                        else
                        {
                            FontFamilyRecord focusedItem = (FontFamilyRecord)FocusedItem;
                            if (focusedItem.Name != mouseOverItem.Name || focusedItem.Purpose != mouseOverItem.Purpose)
                            {
                                FocusedItem = mouseOverItem;
                            }
                        }

                        break;
                    }
                }
            }

            m_skip = false;
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Invoked when <see cref="E:System.Windows.Input.Mouse.MouseMove"/>�attached event is raised.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (m_isMouseOverIndex != -1)
            {
                FontListBoxInternalItem item = ContainerFromIndex(m_isMouseOverIndex);

                if (item != null)
                {
                    item.HasFocus = false;
                }
            }

            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Invoked when <see cref="E:System.Windows.Input.Mouse.MouseDown"/>�attached event is raised.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> that contains the event data.
        /// This event data reports details about the mouse button that was pressed and the handled state.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            object originalSource = e.OriginalSource;

            if (originalSource is FrameworkElement)
            {
                object item = GetItem((FrameworkElement)originalSource);

                if (item is FontListBoxInternalItem)
                {
                    FontListBoxInternalItem focusedItemContainer = ContainerFromItem(FocusedItem);

                    if (focusedItemContainer != null)
                    {
                        focusedItemContainer.HasFocus = false;
                    }

                    focusedItemContainer = (FontListBoxInternalItem)item;

                    focusedItemContainer.HasFocus = true;

                    object focusedItem = ItemContainerGenerator.ItemFromContainer(focusedItemContainer);
                    object selectedItem = SelectedItem;

                    if (selectedItem != null)
                    {
                        string selectedItemName = ((FontFamilyRecord)selectedItem).Name;
                        string focusedItemName = ((FontFamilyRecord)focusedItem).Name;

                        string selectedItemPurpose = ((FontFamilyRecord)selectedItem).Purpose;
                        string focusedItemPurpose = ((FontFamilyRecord)focusedItem).Purpose;

                        if (selectedItemName != focusedItemName || selectedItemPurpose != focusedItemPurpose)
                        {
                            SelectedItem = focusedItem;
                        }
                    }
                    else
                    {
                        SelectedItem = focusedItem;
                    }

                    m_focusedItemIndex = -1;

                    m_isMouseOverIndex = -1;

                    HasFocus = false;
                }
            }

            e.Handled = true;
        }

        /// <summary>
        /// Invoked when the <see cref="System.Windows.UIElement.KeyDown"/> event is received.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            m_skip = true;
            if (!e.Handled)
            {
                switch (e.Key)
                {
                    case Key.Escape:

                        FontFamilyRecord focusedItem = (FontFamilyRecord)FocusedItem;

                        FontListBoxInternalItem item = ContainerFromItem(focusedItem);

                        if (item != null)
                        {
                            item.HasFocus = false;
                        }

                        FontFamilyRecord selectedItem = (FontFamilyRecord)SelectedItem;

                        if (selectedItem != focusedItem)
                        {
                            focusedItem = selectedItem;
                        }

                        if ((FontFamilyRecord)FocusedItem != focusedItem && FocusedItem != null)
                        {
                            FocusedItem = focusedItem;
                        }

                        HasFocus = false;

                        e.Handled = true;
                        break;

                    case Key.Enter:

                        if (m_isMouseOverIndex != -1)
                        {
                            FontFamilyRecord mouseOverItem = (FontFamilyRecord)Items[m_isMouseOverIndex];

                            if (FocusedItem == null)
                            {
                                FocusedItem = mouseOverItem;
                            }
                            else
                            {
                                focusedItem = (FontFamilyRecord)FocusedItem;
                                if (focusedItem.Name != mouseOverItem.Name || focusedItem.Purpose != mouseOverItem.Purpose)
                                {
                                    FocusedItem = mouseOverItem;
                                }
                            }
                        }

                        if (SelectedItem == null)
                        {
                            SelectedItem = FocusedItem;
                        }
                        else
                        {
                            selectedItem = (FontFamilyRecord)SelectedItem;
                            focusedItem = (FontFamilyRecord)FocusedItem;

                            if (focusedItem.Name != selectedItem.Name || focusedItem.Purpose != selectedItem.Purpose)
                            {
                                SelectedItem = focusedItem;
                            }
                        }

                        HasFocus = false;
                        e.Handled = true;
                        break;

                    case Key.Up:
                        FocusUpDown(-1);
                        e.Handled = true;
                        break;

                    case Key.Down:
                        FocusUpDown(1);
                        e.Handled = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Checks whether object is it own container.
        /// </summary>
        /// <param name="item">Object to check.</param>
        /// <returns>True if it is own container, otherwise false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is FontListBoxInternalItem;
        }

        /// <summary>
        /// Creates and returns new instance of <see cref="Syncfusion.Windows.Tools.Controls.FontListBoxInternalItem"/>.
        /// </summary>
        /// <returns>
        /// Returns new instance of <see cref="Syncfusion.Windows.Tools.Controls.FontListBoxInternalItem"/>.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new FontListBoxInternalItem();
        }

        /// <summary>
        /// Invoked when SelectedItem property is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object that contains the event data.</param>
        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FontListBoxInternal instance = (FontListBoxInternal)sender;
            FontFamilyRecord record = (FontFamilyRecord)e.NewValue;

            if (record == null)
            {
                return;
            }

            if (record != (FontFamilyRecord)instance.FocusedItem)
            {
                if (instance.m_focusedItemIndex != -1)
                {
                    instance.ContainerFromIndex(instance.m_focusedItemIndex + 1).HasFocus = false;
                }

                FontListBoxInternalItem item = instance.ContainerFromItem(record);

                FontListBoxInternalItem focusedItem = instance.ContainerFromItem(instance.FocusedItem);

                if (focusedItem != null)
                {
                    focusedItem.HasFocus = false;
                }

                if (item != null)
                {
                    item.HasFocus = true;
                    instance.FocusedItem = instance.ItemContainerGenerator.ItemFromContainer(item);
                }
            }
        }

        /// <summary>
        /// Navigates the focus between items.
        /// </summary>
        /// <param name="step">Step to move the focus over.</param>
        private void FocusUpDown(int step)
        {
            if (!HasFocus && !IsVisible)
            {
                HasFocus = true;
                return;
            }

            object focusedItem = FocusedItem;
            int index = -1;

            if (focusedItem == null && step == 1)
            {
                focusedItem = Items[Items.Count - 1];
            }

            if (focusedItem == null && step == -1)
            {
                focusedItem = Items[0];
            }
            if (m_isMouseOverIndex != -1)
            {
                index = m_isMouseOverIndex;
            }
            else if (m_focusedItemIndex != -1 && m_isMouseOverIndex == -1)
            {
                index = m_focusedItemIndex;
            }
            else
            {
                if (focusedItem != null)
                {
                    index = Items.IndexOf(focusedItem);
                }
            }

            ContainerFromIndex(index).HasFocus = false;
            index += step;

            if (index < 0)
            {
                index = Items.Count - 1;
            }

            if (index >= Items.Count)
            {
                index = 0;
            }
            m_focusedItemIndex = index;
            if (focusedItem != null)
            {
                ContainerFromItem(focusedItem).HasFocus = false;
            }

            focusedItem = Items[index];

            FocusedItem = focusedItem;

            ContainerFromItem(focusedItem).HasFocus = true;

            ContainerFromItem(focusedItem).BringIntoView();

            m_isMouseOverIndex = -1;
        }

        /// <summary>
        /// Gets the item from the visual tree.
        /// </summary>
        /// <param name="frameworkElement">Element from which to search the item.</param>
        /// <returns>The item to be returned.</returns>
        private FontListBoxInternalItem GetItem(FrameworkElement frameworkElement)
        {
            if (frameworkElement.TemplatedParent != null)
            {
                FrameworkElement result = (FrameworkElement)frameworkElement.TemplatedParent;

                while (!(result is FontListBoxInternalItem) && result != null)
                {
                    result = (FrameworkElement)result.TemplatedParent;
                }

                if (result is FontListBoxInternalItem)
                {
                    return (FontListBoxInternalItem)result;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Calls OnFocusedItemChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnFocusedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FontListBoxInternal instance = (FontListBoxInternal)d;
            instance.OnFocusedItemChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises FocusedItemChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnFocusedItemChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FocusedItem != null)
            {
                //FontListBoxInternalItem item = ContainerFromItem(FocusedItem);
                //item.BringIntoView();
                ////else
                ////   Debug.WriteLine("NullCount : "+nullcount++);

                if (ContainerFromItem(FocusedItem) != null)
                {
                    //ContainerFromItem(FocusedItem).BringIntoView();
                }
            }

            if (FocusedItemChanged != null)
            {
                FocusedItemChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnHasFocusChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnHasFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FontListBoxInternal instance = (FontListBoxInternal)d;
            instance.OnHasFocusChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises HasFocusChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnHasFocusChanged(DependencyPropertyChangedEventArgs e)
        {
            FontFamilyRecord focusedItem = (FontFamilyRecord)FocusedItem;
            FontFamilyRecord selectedItem = (FontFamilyRecord)SelectedItem;

            if (HasFocus == false)
            {
                m_isMouseOverIndex = -1;

                if (selectedItem != null)
                {
                    if (IsMouseOver)
                    {
                        foreach (object o in Items)
                        {
                            FontFamilyRecord r = (FontFamilyRecord)o;
                            if (r.Type == FontFamilyRecordType.RecentlyUsed && r.Name != selectedItem.Name)
                            {
                                ContainerFromItem(r).HasFocus = false;
                                break;
                            }
                        }
                        if (selectedItem.Name != focusedItem.Name || selectedItem.Purpose != focusedItem.Purpose)
                        {
                            SelectedItem = focusedItem;
                        }
                    }
                }
            }

            if (HasFocus == true)
            {
                Focus();

                if (focusedItem != null)
                {
                    FontListBoxInternalItem item = null;
                    if ((focusedItem as FontFamilyRecord).Type != FontFamilyRecordType.Theme)
                    {
                        foreach (object obj in Items)
                        {
                            if (obj is FontFamilyRecord)
                            {
                                FontFamilyRecord record = (FontFamilyRecord)obj;
                                if (record.Type == FontFamilyRecordType.RecentlyUsed)
                                {
                                    m_focusedItemIndex = Items.IndexOf(record);
                                    item = ContainerFromIndex(m_focusedItemIndex);

                                    m_isMouseOverIndex = -1;

                                    break;
                                }
                            }
                        }

                        if (item != null)
                        {
                            ContainerFromItem(focusedItem).HasFocus = false;
                            item.HasFocus = true;
                            item.BringIntoView();
                        }
                    }
                    else
                    {
                        item = ContainerFromItem(focusedItem);
                        if (item != null)
                        {
                            item.HasFocus = true;
                        }
                    }
                }

                if (focusedItem == null && selectedItem != null)
                {
                    FocusedItem = selectedItem;
                }
            }

            if (HasFocusChanged != null)
            {
                HasFocusChanged(this, e);
            }
        }

        /// <summary>
        /// Gets container of <see cref="Syncfusion.Windows.Tools.Controls.FontListBoxInternalItem"/> type from item.
        /// </summary>
        /// <param name="item">Item from which to get the container.</param>
        /// <returns>Returns container of <see cref="Syncfusion.Windows.Tools.Controls.FontListBoxInternalItem"/> type.</returns>
        private FontListBoxInternalItem ContainerFromItem(object item)
        {
            return (FontListBoxInternalItem)ItemContainerGenerator.ContainerFromItem(item);
        }

        /// <summary>
        /// Gets container of <see cref="Syncfusion.Windows.Tools.Controls.FontListBoxInternalItem"/> type from index.
        /// </summary>
        /// <param name="index">Index of the item from which to get the container.</param>
        /// <returns>Returns container of <see cref="Syncfusion.Windows.Tools.Controls.FontListBoxInternalItem"/> type.</returns>
        private FontListBoxInternalItem ContainerFromIndex(int index)
        {
            return (FontListBoxInternalItem)ItemContainerGenerator.ContainerFromIndex(index);
        }

        #endregion Implementation

        #region Properties

        /// <summary>
        /// Gets or sets the focused item of <see cref="Syncfusion.Windows.Tools.Controls.FontListBox"/> control.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="object"/>
        /// </value>
        /// <seealso cref="object"/>
        public object FocusedItem
        {
            get
            {
                return (object)GetValue(FocusedItemProperty);
            }

            set
            {
                SetValue(FocusedItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Syncfusion.Windows.Tools.Controls.FontListBox"/> control has focus.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        /// <seealso cref="bool"/>
        public bool HasFocus
        {
            get
            {
                return (bool)GetValue(HasFocusProperty);
            }

            set
            {
                SetValue(HasFocusProperty, value);
            }
        }

        #endregion Properties

        #region Events

        /// <summary>
        /// Event that is raised when FocusedItem property is changed.
        /// </summary>
        public event PropertyChangedCallback FocusedItemChanged;

        /// <summary>
        /// Event that is raised when HasFocus property is changed.
        /// </summary>
        public event PropertyChangedCallback HasFocusChanged;

        #endregion Events

        #region	Dependency properties

        /// <summary>
        /// Identifies FocusedItem dependency property.
        /// </summary>
        public static readonly DependencyProperty FocusedItemProperty =
            DependencyProperty.Register("FocusedItem", typeof(object), typeof(FontListBoxInternal), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnFocusedItemChanged)));

        /// <summary>
        /// Identifies HasFocus dependency property.
        /// </summary>
        public static readonly DependencyProperty HasFocusProperty =
            DependencyProperty.Register("HasFocus", typeof(bool), typeof(FontListBoxInternal), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnHasFocusChanged)));

        #endregion
    }
}