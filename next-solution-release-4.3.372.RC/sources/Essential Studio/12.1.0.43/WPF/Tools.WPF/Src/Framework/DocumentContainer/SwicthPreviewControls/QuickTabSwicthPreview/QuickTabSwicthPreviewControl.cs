// <copyright file="QuickTabSwicthPreviewControl.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Syncfusion.Windows.Shared;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents switcher control for QuickTab action.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class QuickTabSwicthPreviewControl : SwicthPreviewControlBase
    {
        #region Constants
        /// <summary>
        /// This member presents a element's name from Template of control.
        /// </summary>
        private const string GALLERY_GROUP_NAME = "PART_GalleryGroup";
        #endregion

        #region Private members
        /// <summary>
        /// This is collection of elements in current control.
        /// </summary>
        private readonly ObservableFrameworkElements m_Collections = new ObservableFrameworkElements();

        /// <summary>
        /// This member presents a element from Template of control.
        /// </summary>
        private GalleryGroup m_galleryGroup = null;
        
        /// <summary>
        /// This is index of selected item.
        /// </summary>
        private int m_index = 0;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="QuickTabSwicthPreviewControl"/> class.
        /// </summary>
        static QuickTabSwicthPreviewControl()
        {
            EnvironmentTest.ValidateLicense(typeof(QuickTabSwicthPreviewControl));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QuickTabSwicthPreviewControl), new FrameworkPropertyMetadata(typeof(QuickTabSwicthPreviewControl)));
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when CloseButtonVisible property is changed.
        /// </summary>
        public event PropertyChangedCallback CloseButtonVisibleChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether of the CloseButtonVisible dependency property.
        /// </summary>
        public bool CloseButtonVisible
        {
            get
            {
                return (bool)GetValue(CloseButtonVisibleProperty);
            }

            set
            {
                SetValue(CloseButtonVisibleProperty, value);
            }
        }
        
        /// <summary>
        /// Gets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public override object SelectedItem
        {
            get
            {
                if (0 < m_Collections.Count)
                {
                    return m_Collections[m_index].DataContext;
                }
                else
                {
                    return null;
                }
            }
        }
        
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public override IEnumerable Items
        {
            get
            {
                return m_Collections;
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether [used main collection].
        /// <remarks>Always <c>true</c> for this control.</remarks> 
        /// </summary>
        /// <value>
        /// <c>true</c> if [used main collection]; otherwise, <c>false</c>.
        /// </value>
        public override bool UsedMainCollection
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// When overridden in a derived class, is invoked whenever 
        /// application code or internal processes call 
        /// <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (null != m_galleryGroup)
            {
                m_galleryGroup.PreviewMouseMove -= new MouseEventHandler(OnGalleryGroupPreviewMouseMove);
#if !SyncfusionFramework3_5
                //m_galleryGroup.PreviewTouchMove -= m_galleryGroup_PreviewTouchMove;
#endif
            }

            base.OnApplyTemplate();

            m_galleryGroup = GetTemplateChild(GALLERY_GROUP_NAME) as GalleryGroup;
            m_galleryGroup.PreviewMouseMove += new MouseEventHandler(OnGalleryGroupPreviewMouseMove);
#if !SyncfusionFramework3_5
            //m_galleryGroup.PreviewTouchMove += m_galleryGroup_PreviewTouchMove;
#endif

            if (null == m_galleryGroup)
            {
                throw new NotImplementedException();
            }

            m_galleryGroup.ItemsSource = m_Collections;
            SetSelectedItem();
        }

#if !SyncfusionFramework3_5
        //void m_galleryGroup_PreviewTouchMove(object sender, TouchEventArgs e)
        //{
        //    e.Handled = true;
        //}
#endif
        
        /// <summary>
        /// Clears the items.
        /// </summary>
        public override void ClearItems()
        {
            m_Collections.Clear();
            m_index = 0;
            Visibility = Visibility.Collapsed;
        }
        
        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item ContentControl.</param>
        public override void AddItem(ContentControl item)
        {
            m_Collections.Add(item);
        }
        
        /// <summary>
        /// Moves the item.
        /// </summary>
        /// <param name="isForward">if set to <c>true</c> [is forward].</param>
        /// <param name="switchDirection">The switch direction.</param>
        public override void MoveItem(bool isForward, SwitchDirection switchDirection)
        {
            if (SwitchDirection.Vertical == switchDirection && 1 != m_galleryGroup.RowCount)
            {
                SwitchVisualVertical(isForward);
            }
            else
            {
                SwitchVisualHorizontal(isForward);
            }
        }
        
        /// <summary>
        /// Shows and selects the first item.
        /// </summary>
        public override void ShowSelectedItem()
        {
            SetIndex(0);
        }
        #endregion

        #region Implemenation
        /// <summary>
        /// Updates property value cache and raises CloseButtonVisibleChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnCloseButtonVisibleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CloseButtonVisibleChanged != null)
            {
                CloseButtonVisibleChanged(this, e);
            }
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item ContentControl.</param>
        internal void Remove(ContentControl item)
        {
            int index = m_Collections.IndexOf(item);
            m_Collections.Remove(item);
            int count = m_Collections.Count;

            if (0 != count)
            {
                if (count > index)
                {
                    m_index = index;
                }
                else
                {
                    m_index = 0;
                }

                SetSelectedItem();
            }
            else
            {
                Visibility = Visibility.Collapsed;
                Mouse.Capture(null);
            }
        }

        /// <summary>
        /// Sets the index.
        /// </summary>
        /// <param name="value">The value.</param>
        private void SetIndex(int value)
        {
            m_index = value;

            if (null != m_galleryGroup)
            {
                SetSelectedItem();
            }
        }
        
        /// <summary>
        /// Sets the selected item.
        /// </summary>
        private void SetSelectedItem()
        {
            if (0 < m_Collections.Count)
            {
                ItemWindow itemWindow = (ItemWindow)m_Collections[m_index];
                itemWindow.IsSelected = true;
            }
        }
        
        /// <summary>
        /// Switches the visual horizontal.
        /// </summary>
        /// <param name="isForward">if set to <c>true</c> [is forward].</param>
        private void SwitchVisualHorizontal(bool isForward)
        {
            int count = m_Collections.Count - 1;
            int bound = 0, newBound = count, inc = -1;

            if (isForward)
            {
                bound = count;
                newBound = 0;
                inc = 1;
            }

            if (bound == m_index)
            {
                SetIndex(newBound);
            }
            else
            {
                SetIndex(m_index + inc);
            }
        }
        
        /// <summary>
        /// Switches the visual vertical.
        /// </summary>
        /// <param name="isForward">if set to <c>true</c> [is forward].</param>
        private void SwitchVisualVertical(bool isForward)
        {
            int rowCount = m_galleryGroup.RowCount;
            int colCount = m_galleryGroup.ColCount;

            int row = (int)Math.Floor((double)m_index / colCount);
            int index = m_index % colCount;
            int collectCount = m_Collections.Count;
            int lastRowItemCount = collectCount % colCount;
            int newIndex;

            if (isForward)
            {
                if ((rowCount - 2 > row) || (0 == lastRowItemCount && rowCount - 1 > row)
                    || (rowCount - 1 != row && lastRowItemCount > index))
                {
                    newIndex = m_index + colCount;
                }
                else
                {
                    newIndex = index;
                }
            }
            else
            {
                if (0 == row)
                {
                    newIndex = collectCount - lastRowItemCount + index;

                    if (lastRowItemCount <= index)
                    {
                        newIndex -= colCount;
                    }
                }
                else
                {
                    newIndex = m_index - colCount;
                }
            }

            SetIndex(newIndex);
        }

        /// <summary>
        /// Calls OnCloseButtonVisibleChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCloseButtonVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QuickTabSwicthPreviewControl instance = (QuickTabSwicthPreviewControl)d;
            instance.OnCloseButtonVisibleChanged(e);
        }
        
        /// <summary>
        /// Called when [gallery group preview mouse move].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private static void OnGalleryGroupPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
                e.Handled = true;
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Presents close button of item windows visibility.
        /// </summary>
        public static readonly DependencyProperty CloseButtonVisibleProperty = DependencyProperty.Register("CloseButtonVisible", typeof(bool), typeof(QuickTabSwicthPreviewControl), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnCloseButtonVisibleChanged)));
        #endregion
    }
}