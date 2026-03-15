// <copyright file="InternalPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Used for drawing <see cref="Syncfusion.Windows.Tools.Controls.FontListBox"/>.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class InternalPanel : Panel
    {
        #region	Private	fields

        /// <summary>
        /// Collection for storing theme fonts.
        /// </summary>
        private FontListBoxInternalItemCollection m_themeFonts;

        /// <summary>
        /// Collection for storing recently used fonts.
        /// </summary>
        private FontListBoxInternalItemCollection m_recentlyUsedFonts;

        /// <summary>
        /// Collection for storing fonts.
        /// </summary>
        private FontListBoxInternalItemCollection m_allFonts;

        /// <summary>
        /// Single group header for theme fonts.
        /// </summary>
        private GroupHeader m_themeHeader;

        /// <summary>
        /// Single group header for recently used fonts.
        /// </summary>
        private GroupHeader m_recentlyUsedHeader;

        /// <summary>
        /// Single group header for all fonts.
        /// </summary>
        private GroupHeader m_allHeader;

        /// <summary>
        /// Indicates whether theme fonts are not empty.
        /// </summary>
        private int m_iTheme;

        /// <summary>
        /// Indicates whether recently used fonts are not empty.
        /// </summary>
        private int m_iRecently;

        /// <summary>
        /// Indicates whether control is initialized.
        /// </summary>
        private bool m_bInitialized = false;

        #endregion

        #region	Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalPanel"/> class.
        /// </summary>
        public InternalPanel()
        {
        }

        #endregion

        #region	Implementation

        /// <summary>
        /// Raises the <see cref="System.Windows.FrameworkElement.Initialized"/> event. This method
        /// is invoked whenever <see cref="System.Windows.FrameworkElement.IsInitialized"/> is set
        /// to true internally.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            m_themeHeader = new GroupHeader();
            m_recentlyUsedHeader = new GroupHeader();
            m_allHeader = new GroupHeader();

            m_themeHeader.Text = "   Theme Fonts";
            m_recentlyUsedHeader.Text = "   Recently Used Fonts";
            m_allHeader.Text = "   All Fonts";
            //DependencyPropertyDescriptor descr = DependencyPropertyDescriptor.FromProperty(SkinStorage.VisualStyleProperty, typeof(InternalPanel));
            //descr.AddValueChanged(this, ProcessSkinChange);

            DependencyPropertyDescriptor descr = DependencyPropertyDescriptor.FromProperty(FrameworkElement.FlowDirectionProperty, typeof(InternalPanel));
            descr.AddValueChanged(this, ProcessFlowChange);
            this.Unloaded -= new RoutedEventHandler(InternalPanel_Unloaded);
            this.Unloaded += new RoutedEventHandler(InternalPanel_Unloaded);
            base.OnInitialized(e);
        }

        private void InternalPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            if (m_themeFonts != null)
                m_themeFonts.Clear();
            if (m_allFonts != null)
                m_allFonts.Clear();
            if (m_recentlyUsedFonts != null)
                m_recentlyUsedFonts.Clear();
        }

        /// <summary>
        /// Invoked when <see cref="Syncfusion.Windows.Shared.SkinStorage.VisualStyleProperty"/> property is changed.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        //private void ProcessSkinChange(object sender, EventArgs args)
        //{
        //    string visualStyle = SkinStorage.GetVisualStyle(this);
        //    Shared.DictionaryList visualStylesList = SkinStorage.GetVisualStylesList(this);

        //    foreach (FontListBoxInternalItem item in InternalChildren)
        //    {
        //        SkinStorage.SetVisualStyle(item, visualStyle);
        //        SkinStorage.SetVisualStylesList(item, visualStylesList);
        //    }
        //}

        /// <summary>
        /// Processes the flow change.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ProcessFlowChange(object sender, EventArgs args)
        {
            //string visualStyle = SkinStorage.GetVisualStyle(this);
            //  Shared.DictionaryList visualStylesList = SkinStorage.GetVisualStylesList(this);

            FlowDirection d = FlowDirection;

            foreach (FontListBoxInternalItem item in InternalChildren)
            {
                SetFlowDirection(item, d);
            }
        }

        /// <summary>
        /// Measures the size in layout required for child elements and determines a size for the control.
        /// </summary>
        /// <param name="constraint">The available size that this element can give to child elements. Infinity
        /// can be specified as a value to indicate that the element will size to whatever
        /// content is available.</param>
        /// <returns> The size that this element determines it needs during layout, based on its
        /// calculations of child element sizes.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            UIElementCollection internalChildren = this.InternalChildren;
            int childrenCount = internalChildren.Count;

            if (childrenCount == 0)
            {
                return new Size();
            }

            double childrenHeight = 0;
            double childrenWidth = 0;

            m_themeFonts = new FontListBoxInternalItemCollection();
            m_recentlyUsedFonts = new FontListBoxInternalItemCollection();
            m_allFonts = new FontListBoxInternalItemCollection();

            foreach (FontListBoxInternalItem item in internalChildren)
            {
                item.Measure(constraint);

                Size desired = item.DesiredSize;

                if (!double.IsNaN(desired.Height))
                {
                    childrenHeight += desired.Height;
                }

                if (!double.IsNaN(desired.Width))
                {
                    childrenWidth = Math.Max(childrenWidth, desired.Width);
                }

                FontFamilyRecord record = (FontFamilyRecord)item.Content;

                switch (record.Type)
                {
                    case FontFamilyRecordType.Theme:
                        m_themeFonts.Add(item);
                        break;

                    case FontFamilyRecordType.RecentlyUsed:
                        m_recentlyUsedFonts.Add(item);
                        break;

                    case FontFamilyRecordType.Common:
                        m_allFonts.Add(item);
                        break;
                }
            }

            m_allHeader.Measure(constraint);
            m_recentlyUsedHeader.Measure(constraint);
            m_themeHeader.Measure(constraint);

            childrenHeight += m_allHeader.DesiredSize.Height;
            childrenWidth = Math.Max(childrenWidth, m_allHeader.DesiredSize.Width);

            m_iTheme = (m_themeFonts.Count > 0) ? 1 : 0;
            m_iRecently = (m_recentlyUsedFonts.Count > 0) ? 1 : 0;

            if (m_iRecently == 1)
            {
                //m_recentlyUsedHeader.Visibility = Visibility.Visible;
                childrenHeight += m_recentlyUsedHeader.DesiredSize.Height;
                childrenWidth = Math.Max(childrenWidth, m_recentlyUsedHeader.DesiredSize.Width);
            }
            //else if (m_recentlyUsedHeader != null)
            //{
            //    m_recentlyUsedHeader.Visibility = Visibility.Collapsed;
            //}

            if (m_iTheme == 1)
            {
                //m_themeHeader.Visibility = Visibility.Visible;
                childrenHeight += m_themeHeader.DesiredSize.Height;
                childrenWidth = Math.Max(childrenWidth, m_themeHeader.DesiredSize.Width);
            }
            //else if (m_themeHeader != null)
            //{
            //    m_themeHeader.Visibility = Visibility.Collapsed;
            //}

            Size size = new Size(childrenWidth, childrenHeight);
            return size;
        }

        /// <summary>
        /// Positions child elements and determines a size for the control.
        /// </summary>
        /// <param name="arrangeSize">The final area within the parent that this element should use to arrange
        /// itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            UIElementCollection internalChildren = this.InternalChildren;
            int childrenCount = internalChildren.Count;

            if (childrenCount == 0)
            {
                return new Size();
            }

            double y = 0;

            double childHeight = internalChildren[0].DesiredSize.Height;

            double childWidth = arrangeSize.Width;

            if (m_iTheme == 1)
            {
                ArrangeHeaderedCollection(0, ref y, m_themeHeader, m_themeFonts, childWidth, childHeight);
            }

            if (m_iRecently == 1)
            {
                ArrangeHeaderedCollection(0, ref y, m_recentlyUsedHeader, m_recentlyUsedFonts, childWidth, childHeight);
            }

            ArrangeHeaderedCollection(0, ref y, m_allHeader, m_allFonts, childWidth, childHeight);

            return arrangeSize;
        }

        /// <summary>
        /// Gets a <see cref="System.Windows.Media.Visual"/> child of this <see cref="System.Windows.Controls.Panel"/>
        /// at the specified index position.
        /// </summary>
        /// <param name="index">The index position of the <see cref="System.Windows.Media.Visual"/> child.</param>
        /// <returns>A <see cref="System.Windows.Media.Visual"/> child of the parent <see cref="System.Windows.Controls.Panel"/>
        /// element.</returns>
        protected override Visual GetVisualChild(int index)
        {
            int collCount = this.InternalChildren.Count;

            if (index >= collCount)
            {
                index -= collCount;

                switch (index)
                {
                    case 0:
                        return m_themeHeader;
                    case 1:
                        return m_recentlyUsedHeader;
                    case 2:
                        return m_allHeader;
                }
            }

            return base.GetVisualChild(index);
        }

        /// <summary>
        /// Gets the number of child <see cref="System.Windows.Media.Visual"/> objects in this instance
        /// of <see cref="System.Windows.Controls.Panel"/>.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                int collCount = this.InternalChildren.Count;

                if (collCount == 0)
                {
                    return base.VisualChildrenCount;
                }

                if (m_bInitialized == false)
                {
                    AddHeaders();
                }

                return collCount + 3;
            }
        }

        /// <summary>
        /// Adds group headers to the control for setting parent-child relationship.
        /// </summary>
        private void AddHeaders()
        {
            m_bInitialized = true;

            AddVisualChild(m_themeHeader);

            AddVisualChild(m_recentlyUsedHeader);

            AddVisualChild(m_allHeader);
        }

        /// <summary>
        /// Positions child elements and forms a recursive layout update.
        /// </summary>
        /// <param name="x">X parameter used for arranging elements.</param>
        /// <param name="y">Y parameter used for arranging elements.</param>
        /// <param name="header">Header element to arrange.</param>
        /// <param name="collection">Collection that contains elements for arranging.</param>
        /// <param name="elementWidth">Width parameter used for arranging elements.</param>
        /// <param name="elementHeight">Height parameter used for arranging elements</param>
        private void ArrangeHeaderedCollection(double x, ref double y, GroupHeader header, ICollection collection, double elementWidth, double elementHeight)
        {
            double headerHeight = Math.Max(header.DesiredSize.Height, header.Height);

            if (Double.IsNaN(headerHeight))
            {
                headerHeight = 0;
            }

            header.Arrange(new Rect(x, y, elementWidth, headerHeight));
            y += headerHeight;

            foreach (UIElement element in collection)
            {
                element.Arrange(new Rect(x, y, elementWidth, elementHeight));
                y += elementHeight;
            }
        }

        /// <summary>
        /// Calls OnGroupHeaderStyleChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnGroupHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InternalPanel instance = (InternalPanel)d;
            instance.OnGroupHeaderStyleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises
        /// GroupHeaderStyleChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnGroupHeaderStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GroupHeaderStyle != null)
            {
                Style style = GroupHeaderStyle;

                m_themeHeader.Style = style;
                m_recentlyUsedHeader.Style = style;
                m_allHeader.Style = style;
            }

            if (GroupHeaderStyleChanged != null)
            {
                GroupHeaderStyleChanged(this, e);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the style for group header. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Style"/>
        /// </value>
        /// <seealso cref="Style"/>
        public Style GroupHeaderStyle
        {
            get
            {
                return (Style)GetValue(GroupHeaderStyleProperty);
            }

            set
            {
                SetValue(GroupHeaderStyleProperty, value);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when GroupHeaderStyle property is changed.
        /// </summary>
        public event PropertyChangedCallback GroupHeaderStyleChanged;

        #endregion

        #region	Dependency Properties

        /// <summary>
        /// Identifies GroupHeaderStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupHeaderStyleProperty =
            DependencyProperty.Register("GroupHeaderStyle", typeof(Style), typeof(InternalPanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnGroupHeaderStyleChanged)));

        #endregion
    }
}