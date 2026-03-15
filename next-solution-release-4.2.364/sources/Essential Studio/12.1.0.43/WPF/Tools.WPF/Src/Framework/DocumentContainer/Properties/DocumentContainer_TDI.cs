// <copyright file="DocumentContainer_TDI.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the DocumentContainer  partial class
    /// </summary>

    public partial class DocumentContainer
    {
        #region Events
        /// <summary>
        /// Occurs when [on close all tabs].
        /// </summary>
        public event OnCloseTabsEventHandler CloseAllTabs;
        
        /// <summary>
        /// Occurs when [on close other tabs].
        /// </summary>
        public event OnCloseTabsEventHandler CloseOtherTabs;
        
        /// <summary>
        /// Event that is raised when ShowTabListContextMenu property is changed.
        /// </summary>
        public event PropertyChangedCallback ShowTabListContextMenuChanged;
        
        /// <summary>
        /// Event that is raised when ShowTabItemContextMenu property is changed.
        /// </summary>
        public event PropertyChangedCallback ShowTabItemContextMenuChanged;
        
        /// <summary>
        /// Event that is raised when ShowDragAdorner property is changed.
        /// </summary>
        public event PropertyChangedCallback ShowDragAdornerChanged;
        
        /// <summary>
        /// Event that is raised when DragDropTemplate property is changed.
        /// </summary>
        public event PropertyChangedCallback DragDropTemplateChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the ShowTabListContextMenu dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if [show tab list context menu]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowTabListContextMenu
        {
            get
            {
                return (bool)GetValue(ShowTabListContextMenuProperty);
            }

            set
            {
                SetValue(ShowTabListContextMenuProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [collapse default tab list context menu items].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [collapse default tab list context menu items]; otherwise, <c>false</c>.
        /// </value>
        public bool CollapseDefaultTabListContextMenuItems
        {
            get
            {
                return (bool)GetValue(CollapseDefaultTabListContextMenuItemsProperty);
            }
            set
            {
                SetValue(CollapseDefaultTabListContextMenuItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tab list context menu items.
        /// </summary>
        /// <value>The tab list context menu items.</value>
        public DocumentTabItemMenuItemCollection TabListContextMenuItems
        {
            get
            {
                return (DocumentTabItemMenuItemCollection)GetValue(TabListContextMenuItemsProperty);
            }
            set
            {
                SetValue(TabListContextMenuItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the ShowTabItemContextMenu dependency property.
        /// </summary>
        public bool ShowTabItemContextMenu
        {
            get
            {
                return (bool)GetValue(ShowTabItemContextMenuProperty);
            }

            set
            {
                SetValue(ShowTabItemContextMenuProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether the ShowDragAdorner dependency property.
        /// </summary>
        public bool ShowDragAdorner
        {
            get
            {
                return (bool)GetValue(ShowDragAdornerProperty);
            }

            set
            {
                SetValue(ShowDragAdornerProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the value of the DragDropTemplate dependency property.
        /// </summary>
        public DataTemplate DragDropTemplate
        {
            get
            {
                return (DataTemplate)GetValue(DragDropTemplateProperty);
            }

            set
            {
                SetValue(DragDropTemplateProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets the value of the HideTDIHeaderOnSingleChild dependency property
        /// </summary>
        public bool HideTDIHeaderOnSingleChild
        {
            get
            {
                return (bool)GetValue(HideTDIHeaderOnSingleChildProperty);
            }
            set
            {
                SetValue(HideTDIHeaderOnSingleChildProperty, value);
            }
        }


        /// <summary>
        /// Gets or Sets the value of TabGroupEnabled dependency property
        /// </summary>
        public bool TabGroupEnabled
        {
            get 
            { 
                return (bool)GetValue(TabGroupEnabledProperty);
            }
            set
            { 
                SetValue(TabGroupEnabledProperty, value); 
            }
        } 
        #endregion

        #region Implementation
        /// <summary>
        /// Raises the close all tabs event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.CloseTabEventArgs"/> instance containing the event data.</param>
        protected internal virtual void RaiseCloseAllTabsEvent(object sender, CloseTabEventArgs e)
        {
            if (null != CloseAllTabs)
            {
                CloseAllTabs(this, e);
            }
        }
        
        /// <summary>
        /// Raises the close other tabs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.CloseTabEventArgs"/> instance containing the event data.</param>
        protected internal virtual void RaiseCloseOtherTabs(object sender, CloseTabEventArgs e)
        {
            if (null != CloseOtherTabs)
            {
                CloseOtherTabs(this, e);
            }
        }
        
        /// <summary>
        /// Raises ShowTabListContextMenuChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnShowTabListContextMenuChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != ShowTabListContextMenuChanged)
            {
                ShowTabListContextMenuChanged(this, e);
            }
        }
        
        /// <summary>
        /// Raises ShowTabItemContextMenuChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnShowTabItemContextMenuChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != ShowTabItemContextMenuChanged)
            {
                ShowTabItemContextMenuChanged(this, e);
            }
        }
        
        /// <summary>
        /// Raises ShowDragAdornerChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnShowDragAdornerChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != ShowDragAdornerChanged)
            {
                ShowDragAdornerChanged(this, e);
            }
        }
        
        /// <summary>
        /// Raises DragDropTemplateChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnDragDropTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != DragDropTemplateChanged)
            {
                DragDropTemplateChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnShowTabListContextMenuChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnShowTabListContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            if (instance != null && instance.m_tabControlCollection.Count > 0)
            {
                foreach (DocumentTabControl tab in instance.m_tabControlCollection)
                {
                    BindingUtils.SetBinding(tab, instance, TabControlExt.ShowTabListContextMenuProperty, DocumentContainer.ShowTabListContextMenuProperty);
                }
            }
            else if (instance != null)
            {
                instance.m_showtablistcontextmenuchanged = true;
            }
            instance.OnShowTabListContextMenuChanged(e);
        }

        /// <summary>
        /// Called when [collapse default tab list context menu items changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCollapseDefaultTabListContextMenuItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            if (instance != null && instance.m_tabControlCollection.Count > 0)
            {
                foreach (DocumentTabControl tab in instance.m_tabControlCollection)
                {
                    BindingUtils.SetBinding(tab, instance, TabControlExt.CollapseDefaultTabListContextMenuItemsProperty, DocumentContainer.CollapseDefaultTabListContextMenuItemsProperty);
                }
            }
            else if (instance != null)
            {
                instance.m_collapsedefaulttablistcontextmenuitemschanged = true;
            }
        }

        /// <summary>
        /// Called when [hide TDI header on single child changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHideTDIHeaderOnSingleChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            if (instance.Mode == DocumentContainerMode.TDI)
            {
                TDILayoutPanel panel = instance.m_layoutPanel as TDILayoutPanel;
                if (panel != null)
                {
                    panel.SetTDIHide();
                }
            }
        }
        /// <summary>
        /// Calls OnShowTabItemContextMenuChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnShowTabItemContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            if (instance != null && instance.m_tabControlCollection.Count > 0)
            {
                foreach (DocumentTabControl tab in instance.m_tabControlCollection)
                {
                    BindingUtils.SetBinding(tab, instance, TabControlExt.ShowTabItemContextMenuProperty, DocumentContainer.ShowTabItemContextMenuProperty);
                }
            }
            else if (instance != null)
            {
                instance.m_showtabitemcontextmenuchanged = true;
            }
            instance.OnShowTabItemContextMenuChanged(e);
        }
        
        /// <summary>
        /// Calls OnShowDragAdornerChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnShowDragAdornerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnShowDragAdornerChanged(e);
        }
        
        /// <summary>
        /// Calls OnDragDropTemplateChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDragDropTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnDragDropTemplateChanged(e);
        }
        #endregion

        #region	Dependency propeties
        /// <summary>
        /// ShowTabListContextMenu Property
        /// </summary>
        public static readonly DependencyProperty ShowTabListContextMenuProperty = DependencyProperty.Register("ShowTabListContextMenu", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnShowTabListContextMenuChanged)));


        /// <summary>
        /// Represents the CollapseDefaultTabListContextMenuItemsProperty 
        /// </summary>
        public static readonly DependencyProperty CollapseDefaultTabListContextMenuItemsProperty =
             DependencyProperty.Register("CollapseDefaultTabListContextMenuItems", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnCollapseDefaultTabListContextMenuItemsChanged)));

        /// <summary>
        /// Represents the TabListContextMenuProperty 
        /// </summary>
        public static readonly DependencyProperty TabListContextMenuItemsProperty =
             DependencyProperty.Register("TabListContextMenuItems", typeof(DocumentTabItemMenuItemCollection), typeof(DocumentContainer), new FrameworkPropertyMetadata(new DocumentTabItemMenuItemCollection()));


        /// <summary>
        /// ShowTabItemContextMenu Property
        /// </summary>
        public static readonly DependencyProperty ShowTabItemContextMenuProperty = DependencyProperty.Register("ShowTabItemContextMenu", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnShowTabItemContextMenuChanged)));
        
        /// <summary>
        /// ShowDragAdorner Property
        /// </summary>
        public static readonly DependencyProperty ShowDragAdornerProperty = DependencyProperty.Register("ShowDragAdorner", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnShowDragAdornerChanged)));
        /// <summary>
        /// HideTDIHeaderOnSingleChild Property
        /// </summary>
        public static readonly DependencyProperty HideTDIHeaderOnSingleChildProperty = DependencyProperty.Register("HideTDIHeaderOnSingleChild", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnHideTDIHeaderOnSingleChildChanged)));
        
        /// <summary>
        /// DragDropTemplate Property
        /// </summary>
        public static readonly DependencyProperty DragDropTemplateProperty = DependencyProperty.Register("DragDropTemplate", typeof(DataTemplate), typeof(DocumentContainer), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDragDropTemplateChanged)));

        /// <summary>
        /// Disabling Dynamic TabGroup Creation while dragging tabitem at right and bottom edge of container.
        /// </summary>
        public static readonly DependencyProperty TabGroupEnabledProperty = DependencyProperty.Register("TabGroupEnabled", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(true));
        #endregion
    }
}