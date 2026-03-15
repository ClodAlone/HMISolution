// <copyright file="ListSwicthPreviewControl.cs" company="Syncfusion">
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
using Syncfusion.Windows.Shared;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This control presents actions for list switching.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ListSwicthPreviewControl : SwicthPreviewControlBase
    {
        #region Constants
        /// <summary>
        /// This constant presents a name of element from Template of control.
        /// </summary>
        private const string LIST_BOX_NAME = "PART_ListBox";
        
        /// <summary>
        /// This constant presents a name of element from Template of control.
        /// </summary>
        private const string PREVIEWBORDER_NAME = "PART_PreviewBorder";
        #endregion

        #region Private members
        /// <summary>
        /// This is collection of elements in current control.
        /// </summary>
        private readonly ObservableFrameworkElements m_Collections = new ObservableFrameworkElements();

        /// <summary>
        /// This member presents a element from Template of control.
        /// </summary>
        private ListBox m_listBox = null;
        
        /// <summary>
        /// This is index of selected item.
        /// </summary>
        private int m_index = 0;
        
        /// <summary>
        /// This member presents preview element from Template of control.
        /// </summary>
        private PreviewBorder m_previewBorder = null;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="ListSwicthPreviewControl"/> class.
        /// </summary>
        static ListSwicthPreviewControl()
        {
            EnvironmentTest.ValidateLicense(typeof(ListSwicthPreviewControl));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListSwicthPreviewControl), new FrameworkPropertyMetadata(typeof(ListSwicthPreviewControl)));
        }
        #endregion

        #region Properties
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
            base.OnApplyTemplate();
            m_listBox = GetTemplateChild(LIST_BOX_NAME) as ListBox;

            if (null == m_listBox)
            {
                throw new NotImplementedException("Incorrect template!");
            }
            else
            {
                m_listBox.ApplyTemplate();
                m_previewBorder = m_listBox.Template.FindName(PREVIEWBORDER_NAME, m_listBox) as PreviewBorder;

                if (null == m_previewBorder)
                {
                    throw new NotImplementedException("Incorrect template!");
                }
            }

            m_listBox.ItemsSource = m_Collections;
            SetIndex(m_index);
        }
        
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
            if (isForward)
            {
                ForwardSwitchVisual();
            }
            else
            {
                BackforwardSwitchVisual();
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
        /// Sets the index.
        /// </summary>
        /// <param name="value">The value.</param>
        private void SetIndex(int value)
        {
            m_index = value;

            if (null != m_listBox && 0 < m_listBox.Items.Count)
            {
                m_listBox.SelectedIndex = value;
                FrameworkElement item = (FrameworkElement)m_listBox.SelectedItem;
                PrepareVisual(item);
            }
        }
        
        /// <summary>
        /// Forwards the switch visual elements.
        /// </summary>
        private void ForwardSwitchVisual()
        {
            int count = m_Collections.Count - 1;
            SwitchQuickVisual(count, 0, 1);
        }
        
        /// <summary>
        /// Back forward the switch visual elements.
        /// </summary>
        private void BackforwardSwitchVisual()
        {
            int count = m_Collections.Count - 1;
            SwitchQuickVisual(0, count, -1);
        }
        
        /// <summary>
        /// Switches the visual elements.
        /// </summary>
        /// <param name="bound">The bound.</param>
        /// <param name="newBound">The new bound.</param>
        /// <param name="inc">The increment</param>
        private void SwitchQuickVisual(int bound, int newBound, int inc)
        {
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
        /// Prepares the visual element.
        /// </summary>
        /// <param name="item">The item FrameworkElement.</param>
        private void PrepareVisual(FrameworkElement item)
        {
            m_listBox.ScrollIntoView(item);
            FrameworkElement dContext = (FrameworkElement)item.DataContext;
            DataContext = dContext;
            SwicthPreviewControlBase.SetCustomBrush(m_previewBorder, dContext);
        }
        #endregion
    }
}