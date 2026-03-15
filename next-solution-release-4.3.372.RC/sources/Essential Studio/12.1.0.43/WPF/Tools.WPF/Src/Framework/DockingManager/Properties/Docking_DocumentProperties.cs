// <copyright file="Docking_DocumentProperties.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents DockingManager's properties.
    /// </summary>

    public partial class DockingManager
    {
        #region Events
        
        /// <summary>
        /// Event that is raised when ContainerStyle property is changed.
        /// </summary>
        public event PropertyChangedCallback ContainerStyleChanged;
        
        /// <summary>
        /// Event that is raised when Mode property is changed.
        /// </summary>
        public event PropertyChangedCallback ContainerModeChanged;
        
        /// <summary>
        /// Event that is raised when SwitchMode property is changed.
        /// </summary>
        public event PropertyChangedCallback SwitchModeChanged;
        
        /// <summary>
        /// Occurs when [on close all tabs].
        /// </summary>
        public event OnCloseTabsEventHandler CloseAllTabs;
        
        /// <summary>
        /// Occurs when [on close other tabs].
        /// </summary>
        public event OnCloseTabsEventHandler CloseOtherTabs;
       
        /// <summary>
        /// Event that is raised when close button 
        /// of the active document in MDI mode or active tab in TDI mode is pressed.
        /// </summary>
        public event CloseButtonEventHandler CloseButtonClick;

        /// <summary>
        /// Occurs when [is selected document click].
        /// </summary>
        public event IsSelectedChangedHandler IsSelectedDocument;
     
        /// <summary>
        /// Event that is raised when ShowTabListContextMenu property is changed.
        /// </summary>
        public event PropertyChangedCallback ShowTabListContextMenuChanged;
        
        /// <summary>
        /// Event that is raised when ShowTabItemContextMenu property is changed.
        /// </summary>       
        public event PropertyChangedCallback ShowTabItemContextMenuChanged;
        
        /// <summary>
        /// Event that is raised when the IsMDIMaximizedState property is changed.
        /// </summary>
        public event PropertyChangedCallback IsInMDIMaximizedStateChanged;
        #endregion

        #region Propeties
        /// <summary>
        /// Gets or sets the value of the ContainerStyle dependency property.
        /// </summary>
        public Style ContainerStyle
        {
            get
            {
                return (Style)GetValue(ContainerStyleProperty);
            }

            set
            {
                SetValue(ContainerStyleProperty, value);
            }
        }
       
        /// <summary>
        /// Gets or sets the value of the Mode dependency property.
        /// </summary>
        public DocumentContainerMode ContainerMode
        {
            get
            {
                return (DocumentContainerMode)GetValue(ContainerModeProperty);
            }

            set
            {
                SetValue(ContainerModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the behavior of the buttons, that represent disabled commands.
        /// </summary>
        public DisabledButtonsBehavior DisabledCloseButtonsBehavior
        {
            get
            {
                return (DisabledButtonsBehavior)GetValue(DisabledCloseButtonsBehaviorProperty);
            }

            set
            {
                SetValue(DisabledCloseButtonsBehaviorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the type of the document close button.
        /// </summary>
        /// <value>The type of the document close button.</value>
        public CloseButtonType DocumentCloseButtonType
        {
            get
            {
                return (CloseButtonType)GetValue(DocumentCloseButtonTypeProperty);
            }

            set
            {
                SetValue(DocumentCloseButtonTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets current representation TDIFullScreenMode of the container. This is a dependency property.
        /// </summary>
        public FullScreenMode TDIFullScreenMode
        {
            get
            {
                return (FullScreenMode)GetValue(TDIFullScreenModeProperty);
            }

            set
            {
                SetValue(TDIFullScreenModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets current representation TDIToolBarTray of the container. This is a dependency property.
        /// </summary>
        public ToolBarTray TDIToolBarTray
        {
            get
            {
                return (ToolBarTray)GetValue(TDIToolBarTrayProperty);
            }

            set
            {
                SetValue(TDIToolBarTrayProperty, value);
            }
        }
       
        /// <summary>
        /// Gets or sets the value of the SwitchMode dependency property.
        /// </summary>
        public SwitchMode SwitchMode
        {
            get
            {
                return (SwitchMode)GetValue(SwitchModeProperty);
            }

            set
            {
                SetValue(SwitchModeProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether [show tab list context menu]. ShowTabListContextMenu dependency property.
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
        /// Gets or sets a value indicating whether [show tab item context menu]. ShowTabItemContextMenu dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if [show tab item context menu]; otherwise, <c>false</c>.
        /// </value>
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
        /// Gets or sets a value indicating whether this instance is in MDI maximized state.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is in MDI maximized state; otherwise, <c>false</c>.
        /// </value>
        public bool IsInMDIMaximizedState
        {
            get
            {
                return (bool)GetValue(IsInMDIMaximizedStateProperty);
            }

            set
            {
                SetValue(IsInMDIMaximizedStateProperty, value);
            }
        }
        #endregion
                
        #region Implementation
        /// <summary>
        /// Raises the close all tabs event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.CloseTabEventArgs"/> instance containing the event data.</param>
        protected virtual void RaiseCloseAllTabsEvent(object sender, CloseTabEventArgs e)
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
        protected virtual void RaiseCloseOtherTabs(object sender, CloseTabEventArgs e)
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
        /// Raises ContainerStyleChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnContainerStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != ContainerStyleChanged)
            {
                ContainerStyleChanged(this, e);
            }
        }
       
        /// <summary>
        /// Raises ModeChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnContainerModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != ContainerModeChanged)
            {
                ContainerModeChanged(this, e);
            }
        }
        
        /// <summary>
        /// Raises SwitchModeChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnSwitchModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != SwitchModeChanged)
            {
                SwitchModeChanged(this, e);
            }
            if (m_container != null)
            {
                DocumentContainer container = m_container as DocumentContainer;
                if (container != null)
                {
                    container.SwitchMode = (SwitchMode)e.NewValue;
                }
            }
        }
       
        /// <summary>
        /// Calls when item closes.
        /// </summary>
        /// <param name="e">Event arguments</param>
        internal virtual void FireCloseButtonClick(CloseButtonEventArgs e)
        {
            if (CloseButtonClick != null)
            {
                CloseButtonClick(this, e);
            }
        }

        internal virtual void FireTabClosed(CloseTabEventArgs e)
        {
            if (TabClosed != null)
            {
                TabClosed(this, e);
            }
        }

        internal virtual void FireTabGroupCreated(TabGroupEventArgs e)
        {
            if (TabGroupCreated != null)
            {
                TabGroupCreated(this, e);
            }
        }

        internal virtual void FireMoveToOtherTabGroup(TabGroupEventArgs e)
        {
            if (MoveToOtherTabGroup != null)
            {
                MoveToOtherTabGroup(this, e);
            }
        }
        /// <summary>
        /// Fires the is selected document changed.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.IsSelectedChangedEventArgs"/> instance containing the event data.</param>
        internal virtual void FireIsSelectedDocumentChanged(IsSelectedChangedEventArgs e)
        {
            if (IsSelectedDocument != null)
            {
                IsSelectedDocument(this, e);
            }
        }
       
        /// <summary>
        /// Raises the <see cref="E:IsInMDIMaximizedStateChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnIsInMDIMaximizedStateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != ContainerModeChanged)
            {
                IsInMDIMaximizedStateChanged(this, e);
            }
        }
       
        /// <summary>
        /// Calls OnContainerStyleChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnContainerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnContainerStyleChanged(e);
        }
       
        /// <summary>
        /// Calls OnModeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnContainerModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnContainerModeChanged(e);
        }
       
        /// <summary>
        /// Calls OnSwitchModeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSwitchModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnSwitchModeChanged(e);
        }
        
        /// <summary>
        /// Calls OnShowTabListContextMenuChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnShowTabListContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            if ((instance.m_container as DocumentContainer) != null)
            {
                BindingUtils.SetBinding((instance.m_container as DocumentContainer), instance, DocumentContainer.ShowTabListContextMenuProperty, DockingManager.ShowTabListContextMenuProperty);
            }
            else
            {
                instance.m_showtablistcontextmenuchanged = true;
            }
            instance.OnShowTabListContextMenuChanged(e);
        }
        
        /// <summary>
        /// Calls OnShowTabItemContextMenuChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnShowTabItemContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            if ((instance.m_container as DocumentContainer) != null)
            {
                BindingUtils.SetBinding((instance.m_container as DocumentContainer), instance, DocumentContainer.ShowTabItemContextMenuProperty, DockingManager.ShowTabItemContextMenuProperty);
            }
            else
            {
                instance.m_showtabitemcontextmenuchanged = true;
            }
            instance.OnShowTabItemContextMenuChanged(e);
        }

        /// <summary>
        /// Called when [TDI full screen mode changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTDIFullScreenModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            if ((instance.m_container as DocumentContainer) != null)
            {
                BindingUtils.SetBinding((instance.m_container as DocumentContainer), instance, DocumentContainer.TDIFullScreenModeProperty, DockingManager.TDIFullScreenModeProperty);
            }
            else
            {
                instance.m_tdifullscreenmodechanged = true;
            }
        }

        /// <summary>
        /// Called when [TDI tool bar tray changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTDIToolBarTrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            if ((instance.m_container as DocumentContainer) != null)
            {
                BindingUtils.SetBinding((instance.m_container as DocumentContainer), instance, DocumentContainer.TDIToolBarTrayProperty, DockingManager.TDIToolBarTrayProperty);
            }
            else
            {
                instance.m_tditoolbartraychanged = true;
            }
        }

        /// <summary>
        /// Called when [collapse default tab list context menu items changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCollapseDefaultTabListContextMenuItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            if ((instance.m_container as DocumentContainer) != null)
            {
                BindingUtils.SetBinding((instance.m_container as DocumentContainer), instance, DocumentContainer.CollapseDefaultTabListContextMenuItemsProperty, DockingManager.CollapseDefaultTabListContextMenuItemsProperty);
            }
            else
            {
                instance.m_collapsedefaulttablistcontextmenuitemschanged = true;
            }
        }

        /// <summary>
        /// Called when [TDI close button type changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDocumentCloseButtonTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            if ((instance.m_container as DocumentContainer) != null)
            {
                BindingUtils.SetBinding((instance.m_container as DocumentContainer), instance, DocumentContainer.TDICloseButtonTypeProperty, DockingManager.DocumentCloseButtonTypeProperty);
            }
            else
            {
                instance.m_documentclosebuttontypechanged = true;
            }
        }

        /// <summary>
        /// Called when [is in MDI maximized state changed].
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsInMDIMaximizedStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockingManager instance = (DockingManager)d;
            instance.OnIsInMDIMaximizedStateChanged(e);
        }
        #endregion

        #region Dependency properties
       
        /// <summary>
        /// Presents style of central element in here.
        /// </summary>
        public static readonly DependencyProperty ContainerStyleProperty = 
            DependencyProperty.Register("ContainerStyle", typeof(Style), typeof(DockingManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContainerStyleChanged)));
       
        /// <summary>
        /// Presents bridge with DocumentConatainer for Mode of DocumentConatainer.
        /// </summary>
        public static readonly DependencyProperty ContainerModeProperty =
            DependencyProperty.Register("ContainerMode", typeof(DocumentContainerMode), typeof(DockingManager), new FrameworkPropertyMetadata(DocumentContainerMode.TDI, new PropertyChangedCallback(OnContainerModeChanged)));

        /// <summary>
        /// Presents behavior for disabled close button.
        /// </summary>
        public static readonly DependencyProperty DisabledCloseButtonsBehaviorProperty = DependencyProperty.Register("DisabledCloseButtonsBehavior", typeof(DisabledButtonsBehavior), typeof(DockingManager), new FrameworkPropertyMetadata(DisabledButtonsBehavior.Disable,FrameworkPropertyMetadataOptions.AffectsRender));
        
        /// <summary>
        /// Presents behavior of document tabs CloseButtonType
        /// </summary>
        public static readonly DependencyProperty DocumentCloseButtonTypeProperty = DependencyProperty.Register("DocumentCloseButtonType", typeof(CloseButtonType), typeof(DockingManager), new FrameworkPropertyMetadata(CloseButtonType.Common, new PropertyChangedCallback(OnDocumentCloseButtonTypeChanged)));

        /// <summary>
        /// Presents bridge with DocumentContainer for TDIFullScreenModeProperty
        /// </summary>
        public static readonly DependencyProperty TDIFullScreenModeProperty = DependencyProperty.Register("TDIFullScreenMode", typeof(FullScreenMode), typeof(DockingManager), new FrameworkPropertyMetadata(FullScreenMode.None,new PropertyChangedCallback(OnTDIFullScreenModeChanged)));

        /// <summary>
        /// Gets or sets current representation TDIToolBarTray of the container. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty TDIToolBarTrayProperty = DependencyProperty.Register("TDIToolBarTray", typeof(ToolBarTray), typeof(DockingManager), new FrameworkPropertyMetadata(null,new PropertyChangedCallback(OnTDIToolBarTrayChanged)));

        /// <summary>
        /// Presents switch mode for Docking.
        /// </summary>
        public static readonly DependencyProperty SwitchModeProperty =
            DependencyProperty.Register("SwitchMode", typeof(SwitchMode), typeof(DockingManager), new FrameworkPropertyMetadata(SwitchMode.VS2005, new PropertyChangedCallback(OnSwitchModeChanged)));
        
        /// <summary>
        /// Indicates that need to show tab list or not.
        /// </summary>
        public static readonly DependencyProperty ShowTabListContextMenuProperty =
            DependencyProperty.Register("ShowTabListContextMenu", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnShowTabListContextMenuChanged)));

        /// <summary>
        /// Presents property that indicates whether or not shows context menu of item.
        /// </summary>
        public static readonly DependencyProperty ShowTabItemContextMenuProperty =
            DependencyProperty.Register("ShowTabItemContextMenu", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnShowTabItemContextMenuChanged)));

        /// <summary>
        /// Presents bridge with DocumentConatainer for IsInMDIMaximized for DocumentConatainer.
        /// </summary>
        public static readonly DependencyProperty IsInMDIMaximizedStateProperty =
            DependencyProperty.Register("IsInMDIMaximizedState", typeof(bool), typeof(DockingManager), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsInMDIMaximizedStateChanged)));
        #endregion
    }
}