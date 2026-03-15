// <copyright file="DocumentContainer_Properties.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using System.Collections;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents DocumentContainer partial class
    /// </summary>

    public partial class DocumentContainer
    {
        #region Constants
        /// <summary>
        /// Represents width of minimized window
        /// </summary>
        internal const double MINIMIZED_WIDTH = 124;

        /// <summary>
        /// Represents height of minimized window
        /// </summary>
        internal const double MINIMIZED_HEIGHT = 24;
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when CornerRadius property is changed.
        /// </summary>
        public event PropertyChangedCallback CornerRadiusChanged;

        /// <summary>
        /// Event that is raised when IsLogicalOwnershipEnabled property is changed.
        /// </summary>
        public event PropertyChangedCallback IsLogicalOwnershipEnabledChanged;

        /// <summary>
        /// Event that is raised when ActiveDocument property is changed.
        /// </summary>
        public event PropertyChangedCallback ActiveDocumentChanged;

        /// <summary>
        /// Occurs when IsKeepCircle property is changed.
        /// </summary>
        public event PropertyChangedCallback IsKeepCircleChanged;

        /// <summary>
        /// Event that is raised when SwitchMode property is changed.
        /// </summary>
        public event PropertyChangedCallback SwitchModeChanged;

        /// <summary>
        /// Event that is raised when PersistState property is changed.
        /// </summary>
        public event PropertyChangedCallback PersistStateChanged;

        /// <summary>
        /// Event that is raised when DefaultMenuItemsPanelTemplate property is changed.
        /// </summary>
        public event PropertyChangedCallback DefaultMenuItemsPanelTemplateChanged;

        /// <summary>
        /// Event that is raised when IsAllowMDIResize property is changed.
        /// </summary>
        public event PropertyChangedCallback IsAllowMDIResizeChanged;

        /// <summary>
        /// Event that is raised when UseInteropCompatibility property is changed.
        /// </summary>
        public event PropertyChangedCallback UseInteropCompatibilityChanged;

        /// <summary>
        /// Event that is raised when ShowingFlipControl property is changed.
        /// </summary>
        public event PropertyChangedCallback ShowingFlipControlChanged;

        /// <summary>
        /// Bubbling routed event, used to inform user about the change of the MDIContextMenuItemsCollection.
        /// </summary>
        public static readonly RoutedEvent MDIContextMenuItemsCollectionPropertyChangedEvent = EventManager.RegisterRoutedEvent("MDIContextMenuItemsCollectionChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<DocumentContextMenuItemsCollection>), typeof(DocumentContainer));

        /// <summary>
        /// Bubbling routed event, used to inform user about the change of the MDIBounds.
        /// </summary>
        public static readonly RoutedEvent MDIBoundsChangedEvent = EventManager.RegisterRoutedEvent("MDIBoundsChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Rect>), typeof(DocumentContainer));

        /// <summary>
        /// Bubbling routed event, used to inform user about the change of the MDIMinimizedBounds.
        /// </summary>
        public static readonly RoutedEvent MDIMinimizedBoundsChangedEvent = EventManager.RegisterRoutedEvent("MDIMinimizedBoundsChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Rect>), typeof(DocumentContainer));

        /// <summary>
        /// Routed event, raised when the MDIWindowState changes.
        /// </summary>
        public static readonly RoutedEvent MDIWindowStateChanged = EventManager.RegisterRoutedEvent("MDIWindowStateChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<MDIWindowState>), typeof(DocumentContainer));

        /// <summary>
        /// Event that is raised when IsInMDIMaximizedState property is changed.
        /// </summary>
        public event PropertyChangedCallback IsInMDIMaximizedStateChanged;

        /// <summary>
        /// Event that is raised when DisabledButtonsBehavior property is changed.
        /// </summary>
        public event PropertyChangedCallback DisabledButtonsBehaviorChanged;

        /// <summary>
        /// Event that is raised when Mode property is changed.
        /// </summary>
        public event PropertyChangedCallback ModeChanged;

        /// <summary>
        /// Event that is raised when IsDocumentStateRequired property is changed.
        /// </summary>
        public event PropertyChangedCallback IsDocumentStateRequiredChanged;

        /// <summary>
        /// Event that is raised when CanMDIMaximize property is changed.
        /// </summary>
        public event PropertyChangedCallback CanMDIMaximizeChanged;

        /// <summary>
        /// Event that is raised when CanMDIMinimize property is changed.
        /// </summary>
        public event PropertyChangedCallback CanMDIMinimizeChanged;

        /// <summary>
        /// Event that is raised when DelayPreviewTime property is changed.
        /// </summary>
        public event PropertyChangedCallback DelayPreviewTimeChanged;

        /// <summary>
        /// Event that is raised when MDICommandsTarget property is changed.
        /// </summary>
        public event PropertyChangedCallback MDICommandsTargetChanged;

        /// <summary>
        /// Event that is raised when IsEnabledScroll property is changed.
        /// </summary>
        public event PropertyChangedCallback IsEnabledScrollChanged;

        /// <summary>
        /// Routed event that is raised when the document is about to close.
        /// User can cancel the closure by setting Cancel to true.
        /// </summary>
        public static readonly RoutedEvent DocumentClosingEvent = EventManager.RegisterRoutedEvent("DocumentClosing", RoutingStrategy.Bubble, typeof(CancelingRoutedEventHandler), typeof(DocumentContainer));

        /// <summary>
        /// Event that is raised when close button 
        /// of the active document in MDI mode or active tab in TDI mode is pressed.
        /// </summary>
        public event CloseButtonEventHandler CloseButtonClick;

        /// <summary>
        /// Event that is raised when UseFlyClose property is changed.
        /// </summary>
        public event PropertyChangedCallback UseFlyCloseChanged;

        /// <summary>
        /// Occurs when [document closing].
        /// </summary>
        public event CancelingRoutedEventHandler DocumentClosing
        {
            add
            {
                AddHandler(DocumentClosingEvent, value);
            }

            remove
            {
                RemoveHandler(DocumentClosingEvent, value);
            }
        }

        /// <summary>
        /// Event that is raised when Document Tab is Closed.
        /// </summary>
        public event TabClosedEventHandler TabClosed;

        /// <summary>
        /// Event that is raised when Document TabGroup is Created.
        /// </summary>
        public event TabGroupEventHandler TabGroupCreated;

        /// <summary>
        /// Event that is raised when Document Tab is moved to other TabGroup.
        /// </summary>
        public event TabGroupEventHandler MoveToOtherTabGroup;
        #endregion

        #region Properties

        public bool IsLazyLoaded
        {
            get { return (bool)GetValue(IsLazyLoadedProperty); }
            set { SetValue(IsLazyLoadedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value of the CornerRadius dependency property.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }

            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }
        /// <summary>
        /// Gets or Sets IsTDIDragDropEnabled attached property
        /// </summary>
        public bool IsTDIDragDropEnabled
        {
            get
            {
                return (bool)GetValue(IsTDIDragDropEnabledProperty);
            }
            set
            {
                SetValue(IsTDIDragDropEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is logical ownership enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is logical ownership enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsLogicalOwnershipEnabled
        {
            get
            {
                return (bool)GetValue(IsLogicalOwnershipEnabledProperty);
            }

            set
            {
                SetValue(IsLogicalOwnershipEnabledProperty, value);
            }
        }



        public IEnumerable ItemsSource
        {
            get 
            { 
                return (IEnumerable)GetValue(ItemsSourceProperty); 
            }
            set
            { 
                SetValue(ItemsSourceProperty, value); 
            }
        }

        private UIElement m_ActiveDocumentInternal = null;
        /// <summary>
        /// Gets or sets the value of the ActiveDocument dependency property.
        /// </summary>
        public UIElement ActiveDocument
        {
            get
            {
                return (UIElement)GetValue(ActiveDocumentProperty);
            }

            set
            {
                SetValue(ActiveDocumentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether of the IsInMDIMaximizedState dependency property.
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

        /// <summary>
        /// Gets or sets a value indicating whether this instance is keep circle.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is keep circle; otherwise, <c>false</c>.
        /// </value>
        public bool IsKeepCircle
        {
            get
            {
                return (bool)GetValue(IsKeepCircleProperty);
            }

            set
            {
                SetValue(IsKeepCircleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the behavior of the buttons, that represent disabled commands.
        /// </summary>
        public DisabledButtonsBehavior DisabledButtonsBehavior
        {
            get
            {
                return (DisabledButtonsBehavior)GetValue(DisabledButtonsBehaviorProperty);
            }

            set
            {
                SetValue(DisabledButtonsBehaviorProperty, value);
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
        /// Gets or sets current representation mode of the container. This is a dependency property.
        /// </summary>
        public DocumentContainerMode Mode
        {
            get
            {
                return (DocumentContainerMode)GetValue(ModeProperty);
            }

            set
            {
                SetValue(ModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets value of IsTouchEnabled property of the container. This is a dependency property.
        /// </summary>
        internal bool IsTouchEnabled
        {
            get 
            { 
                return (bool)GetValue(IsTouchEnabledProperty); 
            }

            set 
            { 
                SetValue(IsTouchEnabledProperty, value); 
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
        /// Gets or sets the type of the document close button.
        /// </summary>
        /// <value>The type of the document close button.</value>
        public CloseButtonType TDICloseButtonType
        {
            get
            {
                return (CloseButtonType)GetValue(TDICloseButtonTypeProperty);
            }

            set
            {
                SetValue(TDICloseButtonTypeProperty, value);
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
        /// Gets or sets a value indicating whether this instance is document state required.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is document state required; otherwise, <c>false</c>.
        /// </value>
        public bool IsDocumentStateRequired
        {
            get
            {
                return (bool)GetValue(IsDocumentStateRequiredProperty);
            }

            set
            {
                SetValue(IsDocumentStateRequiredProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CanMDIMaximize dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can MDI maximize; otherwise, <c>false</c>.
        /// </value>
        public bool CanMDIMaximize
        {
            get
            {
                return (bool)GetValue(CanMDIMaximizeProperty);
            }

            set
            {
                SetValue(CanMDIMaximizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CanMDIMinimize dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can MDI minimize; otherwise, <c>false</c>.
        /// </value>
        public bool CanMDIMinimize
        {
            get
            {
                return (bool)GetValue(CanMDIMinimizeProperty);
            }

            set
            {
                SetValue(CanMDIMinimizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the DelayPreviewTime dependency property.
        /// </summary>
        /// <value>The delay preview time.</value>
        public TimeSpan DelayPreviewTime
        {
            get
            {
                return (TimeSpan)GetValue(DelayPreviewTimeProperty);
            }

            set
            {
                SetValue(DelayPreviewTimeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IsEnabledScroll dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is enabled scroll; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabledScroll
        {
            get
            {
                return (bool)GetValue(IsEnabledScrollProperty);
            }

            set
            {
                SetValue(IsEnabledScrollProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the PersistState dependency property.
        /// </summary>
        /// <value><c>true</c> if [persist state]; otherwise, <c>false</c>.</value>
        public bool PersistState
        {
            get
            {
                return (bool)GetValue(PersistStateProperty);
            }

            set
            {
                SetValue(PersistStateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the menu items panel template.
        /// </summary>
        /// <value>The menu items panel template.</value>
        public ItemsPanelTemplate MenuItemsPanelTemplate
        {
            get
            {
                return (ItemsPanelTemplate)GetValue(MenuItemsPanelTemplateProperty);
            }

            set
            {
                SetValue(MenuItemsPanelTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the DefaultMenuItemsPanelTemplate dependency property.
        /// </summary>
        public ItemsPanelTemplate DefaultMenuItemsPanelTemplate
        {
            get
            {
                return (ItemsPanelTemplate)GetValue(DefaultMenuItemsPanelTemplateProperty);
            }

            protected set
            {
                SetValue(DefaultMenuItemsPanelTemplatePropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IsAllowMDIResize dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is allow MDI resize; otherwise, <c>false</c>.
        /// </value>
        public bool IsAllowMDIResize
        {
            get
            {
                return (bool)GetValue(IsAllowMDIResizeProperty);
            }

            set
            {
                SetValue(IsAllowMDIResizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the UseFlyClose dependency property.
        /// </summary>
        /// <value><c>true</c> if [use fly close]; otherwise, <c>false</c>.</value>
        public bool UseFlyClose
        {
            get
            {
                return (bool)GetValue(UseFlyCloseProperty);
            }

            set
            {
                SetValue(UseFlyCloseProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the UseInteropCompatibility dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if [use interop compatibility]; otherwise, <c>false</c>.
        /// </value>
        public bool UseInteropCompatibility
        {
            get
            {
                return (bool)GetValue(UseInteropCompatibilityProperty);
            }

            set
            {
                SetValue(UseInteropCompatibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is tab preview enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is tab preview enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsTabPreviewEnabled
        {
            get
            {
                return (bool)GetValue(IsTabPreviewEnabledProperty);
            }
            set
            {
                SetValue(IsTabPreviewEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the ShowingFlipControl dependency property.
        /// </summary>
        /// <value><c>true</c> if [showing flip control]; otherwise, <c>false</c>.</value>
        public bool ShowingFlipControl
        {
            get
            {
                return (bool)GetValue(ShowingFlipControlProperty);
            }

            protected set
            {
                SetValue(ShowingFlipControlPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the MDICommandsTarget dependency property.
        /// </summary>
        protected object MDICommandsTarget
        {
            get
            {
                return GetValue(MDICommandsTargetProperty);
            }

            set
            {
                SetValue(MDICommandsTargetPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the AddTabDocumentAtLast dependency property.
        /// </summary>
        /// <value><c>true</c> if [the document to be added at last]; otherwise, <c>false</c>.</value>
        public bool AddTabDocumentAtLast
        {
            get
            {
                return (bool)GetValue(AddTabDocumentAtLastProperty);
            }

            set
            {
                SetValue(AddTabDocumentAtLastProperty, value);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Raises UseFlyCloseChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnUseFlyCloseChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != UseFlyCloseChanged)
            {
                UseFlyCloseChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises CornerRadusChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnCornerRadusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CornerRadiusChanged != null)
            {
                CornerRadiusChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises IsLogicalOwnershipEnabledChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected void OnIsLogicalOwnershipEnabledChangedInternal(DependencyPropertyChangedEventArgs e)
        {
            if (IsLogicalOwnershipEnabledChanged != null)
            {
                IsLogicalOwnershipEnabledChanged(this, e);
            }

            OnIsLogicalOwnershipEnabledChanged(e);
        }

        /// <summary>
        /// Raises ActiveDocumentChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnActiveDocumentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ActiveDocumentChanged != null)
            {
                ActiveDocumentChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises IsInMDIMaximizedStateChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsInMDIMaximizedStateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsInMDIMaximizedStateChanged != null)
            {
                IsInMDIMaximizedStateChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises DisabledButtonsBehaviorChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnDisabledButtonsBehaviorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DisabledButtonsBehaviorChanged != null)
            {
                DisabledButtonsBehaviorChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:IsKeepCircleChanged"/> event.
        /// </summary>
        /// <param name="arg">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnIsKeepCircleChanged(DependencyPropertyChangedEventArgs arg)
        {
            if (null != IsKeepCircleChanged)
            {
                IsKeepCircleChanged(this, arg);
            }
        }

        /// <summary>
        /// Raises SwitchModeChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnSwitchModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SwitchModeChanged != null)
            {
                SwitchModeChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises ModeChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ModeChanged != null)
            {
                ModeChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises IsDocumentStateRequiredChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsDocumentStateRequiredChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsDocumentStateRequiredChanged != null)
            {
                IsDocumentStateRequiredChanged(this, e);
            }

            m_layoutPanel.ResetVisibleList();
        }

        /// <summary>
        /// Updates property value cache and raises CanMDIMaximizeChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnCanMDIMaximizeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CanMDIMaximizeChanged != null)
            {
                CanMDIMaximizeChanged(this, e);
            }

            CoerceValue(IsInMDIMaximizedStateProperty);
        }

        /// <summary>
        /// Updates property value cache and raises CanMDIMinimizeChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnCanMDIMinimizeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CanMDIMinimizeChanged != null)
            {
                CanMDIMinimizeChanged(this, e);
            }

            foreach (DependencyObject obj in Items)
            {
                obj.CoerceValue(MDIWindowStateProperty);
            }
        }

        /// <summary>
        /// Updates property value cache and raises DelayPreviewTimeChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnDelayPreviewTimeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DelayPreviewTimeChanged != null)
            {
                DelayPreviewTimeChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises MDICommandsTargetChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnMDICommandsTargetChanged(DependencyPropertyChangedEventArgs e)
        {
            if (MDICommandsTargetChanged != null)
            {
                MDICommandsTargetChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises IsEnabledScrollChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsEnabledScrollChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsEnabledScrollChanged != null)
            {
                IsEnabledScrollChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises PersistStateChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnPersistStateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PersistStateChanged != null)
            {
                PersistStateChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises DefaultMenuItemsPanelTemplateChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnDefaultMenuItemsPanelTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DefaultMenuItemsPanelTemplateChanged != null)
            {
                DefaultMenuItemsPanelTemplateChanged(this, e);
            }
        }

        /// <summary>
        /// Raises IsAllowMDIResizeChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsAllowMDIResizeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != IsAllowMDIResizeChanged)
            {
                IsAllowMDIResizeChanged(this, e);
            }
        }

        /// <summary>
        /// Raises UseInteropCompatibilityChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnUseInteropCompatibilityChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != UseInteropCompatibilityChanged)
            {
                UseInteropCompatibilityChanged(this, e);
            }
        }

        /// <summary>
        /// Raises ShowingFlipControlChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnShowingFlipControlChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != ShowingFlipControlChanged)
            {
                ShowingFlipControlChanged(this, e);
            }
        }
        #endregion

        #region Getters and Setters
        /// <summary>
        /// Gets the MDI minimize bounds.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <returns>Rect GetMDIMinimizedBounds </returns>
        public static Rect GetMDIMinimizedBounds(DependencyObject obj)
        {
            return (Rect)obj.GetValue(MDIMinimizedBoundsProperty);
        }

        /// <summary>
        /// Gets the MDI bounds.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <returns>Rect GetMDIBounds</returns>
        public static Rect GetMDIBounds(DependencyObject obj)
        {
            return (Rect)obj.GetValue(MDIBoundsProperty);
        }

        /// <summary>
        /// Gets the SizetoContentInMDI property
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <returns>bool</returns>
        public static bool GetSizetoContentInMDI(DependencyObject obj)
        {
            return (bool)obj.GetValue(SizetoContentInMDIProperty);
        }

        /// <summary>
        /// Gets the SizetoContentInternal property
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <returns>bool</returns>
        internal static bool GetSizetoContentInternal(DependencyObject obj)
        {
            return (bool)obj.GetValue(SizetoContentInternalProperty);
        }

        /// <summary>
        /// Sets the MDI bounds.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <param name="value">The value.</param>
        public static void SetMDIMinimizedBounds(DependencyObject obj, Rect value)
        {
            obj.SetValue(MDIMinimizedBoundsProperty, value);
        }

        /// <summary>
        /// Sets the MDI bounds.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <param name="value">The value SetMDIBounds.</param>
        public static void SetMDIBounds(DependencyObject obj, Rect value)
        {
            obj.SetValue(MDIBoundsProperty, value);
        }
        /// <summary>
        /// Sets the SizetoContentInMDI property
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <param name="value">The value SetMDIBounds.</param>
        public static void SetSizetoContentInMDI(DependencyObject obj, bool value)
        {
            obj.SetValue(SizetoContentInMDIProperty, value);
        }


        /// <summary>
        /// Sets the SizetoContentInternal property
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <param name="value">The value SetMDIBounds.</param>
        internal static void SetSizetoContentInternal(DependencyObject obj, bool value)
        {
            obj.SetValue(SizetoContentInternalProperty, value);
        }
       
        /// <summary>
        /// Gets header of the object.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns> object value type</returns>
        public static object GetHeader(DependencyObject obj)
        {
            return obj.GetValue(HeaderProperty);
        }

        /// <summary>
        /// Sets header of the object.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">The value HeaderProperty.</param>
        public static void SetHeader(DependencyObject obj, object value)
        {
            obj.SetValue(HeaderProperty, value);
        }


        /// <summary>
        /// Gets value of the HeaderTemplate property.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns>Data Template type</returns>
        public static DataTemplate GetHeaderTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(HeaderTemplateProperty);
        }

        /// <summary>
        /// Sets the value of the HeaderTemplate property.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">The value DependencyObject.</param>
        public static void SetHeaderTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(HeaderTemplateProperty, value);
        }

        /// <summary>
        /// Gets Icon attached property value.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns> Brush IconProperty</returns>
        public static Brush GetIcon(DependencyObject obj)
        {
            return (Brush)obj.GetValue(IconProperty);
        }

        /// <summary>
        /// Sets Icon attached property value.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">The value DependencyObject.</param>
        public static void SetIcon(DependencyObject obj, Brush value)
        {
            obj.SetValue(IconProperty, value);
        }

        /// <summary>
        /// Gets the document container.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns> Document Container</returns>
        public static DocumentContainer GetDocumentContainer(DependencyObject obj)
        {
            return (DocumentContainer)obj.GetValue(DocumentContainerProperty);
        }

        /// <summary>
        /// Sets the document container.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">The value DependencyObject.</param>
        internal static void SetDocumentContainer(DependencyObject obj, DocumentContainer value)
        {
            obj.SetValue(DocumentContainerPropertyKey, value);
        }

        /// <summary>
        /// Gets the state of the MDI window.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns> MDIWindow State type</returns>
        public static MDIWindowState GetMDIWindowState(DependencyObject obj)
        {
            return (MDIWindowState)obj.GetValue(MDIWindowStateProperty);
        }

        /// <summary>
        /// Sets the state of the MDI window.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">The value DependencyObject.</param>
        public static void SetMDIWindowState(DependencyObject obj, MDIWindowState value)
        {
            obj.SetValue(MDIWindowStateProperty, value);
        }

        /// <summary>
        /// Sets the document tab item context menu items.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetDocumentTabItemContextMenuItems(DependencyObject obj, DocumentTabItemMenuItemCollection value)
        {
            obj.SetValue(DocumentTabItemContextMenuItemsProperty, value);
        }

        /// <summary>
        /// Gets the document tab item context menu items.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static DocumentTabItemMenuItemCollection GetDocumentTabItemContextMenuItems(DependencyObject obj)
        {
            return (DocumentTabItemMenuItemCollection)obj.GetValue(DocumentTabItemContextMenuItemsProperty);
        }

        /// <summary>
        /// Gets the can close.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns>bool value type</returns>
        public static bool GetCanClose(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanCloseProperty);
        }

        /// <summary>
        /// Sets the can close.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetCanClose(DependencyObject obj, bool value)
        {
            obj.SetValue(CanCloseProperty, value);
        }

        /// <summary>
        /// Gets the can closeArg.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns>bool value type</returns>
        public static bool GetCanCloseArg(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanCloseArgProperty);
        }

        /// <summary>
        /// Sets the can close.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetCanCloseArg(DependencyObject obj, bool value)
        {
            obj.SetValue(CanCloseArgProperty, value);
        }

        /// <summary>
        /// Gets the allow MDI resize.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns>bool value type</returns>
        public static bool GetAllowMDIResize(DependencyObject obj)
        {
            return (bool)obj.GetValue(AllowMDIResizeProperty);
        }

        /// <summary>
        /// Sets the allow MDI resize.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetAllowMDIResize(DependencyObject obj, bool value)
        {
            obj.SetValue(AllowMDIResizeProperty, value);
        }

        /// <summary>
        /// Gets the document preview.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns> Brush DocumentPreviewProperty</returns>
        public static Brush GetDocumentPreview(DependencyObject obj)
        {
            return (Brush)obj.GetValue(DocumentPreviewProperty);
        }

        /// <summary>
        /// Sets the document preview.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">The value DependencyObject.</param>
        public static void SetDocumentPreview(DependencyObject obj, Brush value)
        {
            obj.SetValue(DocumentPreviewProperty, value);
        }

        /// <summary>
        /// Gets the MDI window context menu items collection.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns>DocumentContextMenuItems Collection</returns>
        public static DocumentContextMenuItemsCollection GetMDIContextMenuItemsCollection(DependencyObject obj)
        {
            return (DocumentContextMenuItemsCollection)obj.GetValue(MDIContextMenuItemsCollectionProperty);
        }

        /// <summary>
        /// Sets the MDI window context menu items collection.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">The value DependencyObject.</param>
        public static void SetMDIContextMenuItemsCollection(DependencyObject obj, DocumentContextMenuItemsCollection value)
        {
            obj.SetValue(MDIContextMenuItemsCollectionProperty, value);
        }

        /// <summary>
        /// Gets the value of the IsCommandMenu dependency property.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns>bool value type </returns>
        public static bool GetIsCommandMenu(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsCommandMenuProperty);
        }

        /// <summary>
        /// Sets the value of the IsCommandMenu dependency property.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetIsCommandMenu(DependencyObject obj, bool value)
        {
            obj.SetValue(IsCommandMenuProperty, value);
        }

        /// <summary>
        /// Gets the tab caption tool tip.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns> object TabCaptionToolTip  </returns>
        public static object GetTabCaptionToolTip(DependencyObject obj)
        {
            return obj.GetValue(TabCaptionToolTipProperty);
        }

        /// <summary>
        /// Sets the tab caption tool tip.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">The value DependencyObject.</param>
        public static void SetTabCaptionToolTip(DependencyObject obj, object value)
        {
            obj.SetValue(TabCaptionToolTipProperty, value);
        }

        /// <summary>
        /// Gets the document tab header style.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns>The value DependencyObject.</returns>
        public static object GetDocumentTabItemStyle(DependencyObject obj)
        {
            return obj.GetValue(DocumentTabItemStyleProperty);
        }

        /// <summary>
        /// Sets the document tab header style.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">The value DependencyObject.</param>
        public static void SetDocumentTabItemStyle(DependencyObject obj, object value)
        {
            obj.SetValue(DocumentTabItemStyleProperty, value);
        }

        /// <summary>
        /// Gets the document tab header style.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns>The value DependencyObject.</returns>
        public static object GetDocumentTabControlStyle(DependencyObject obj)
        {
            return obj.GetValue(DocumentTabControlStyleProperty);
        }

        /// <summary>
        /// Sets the document tab header style.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">The value DependencyObject.</param>
        public static void SetDocumentTabControlStyle(DependencyObject obj, object value)
        {
            obj.SetValue(DocumentTabControlStyleProperty, value);
        }

        /// <summary>
        /// Gets the document tab header style.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns>The value DependencyObject.</returns>
        public static object GetDocumentMDIHeaderStyle(DependencyObject obj)
        {
            return obj.GetValue(DocumentMDIHeaderStyleProperty);
        }

        /// <summary>
        /// Sets the document tab header style.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">The value DependencyObject.</param>
        public static void SetDocumentMDIHeaderStyle(DependencyObject obj, object value)
        {
            obj.SetValue(DocumentMDIHeaderStyleProperty, value);
        }

        /// <summary>
        /// Gets the can Drag.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <returns>The value DependencyObject.</returns>
        public static bool GetCanDrag(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanDragProperty);
        }

        /// <summary>
        /// Sets the can drag.
        /// </summary>
        /// <param name="obj">The obj DependencyObject.</param>
        /// <param name="value">The value DependencyObject.</param>
        public static void SetCanDrag(DependencyObject obj, bool value)
        {
            obj.SetValue(CanDragProperty, value);
        }



        internal static bool GetIsLogicalChild(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsLogicalChildProperty);
        }

        internal static void SetIsLogicalChild(DependencyObject obj, bool value)
        {
            obj.SetValue(IsLogicalChildProperty, value);
        }
        #endregion

        #region Static helper methods
        /// <summary>
        /// Calls OnIsInMDIMaximizedStateChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsInMDIMaximizedStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnIsInMDIMaximizedStateChanged(e);
        }

        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            
        }

        /// <summary>
        /// Calls OnCornerRadusChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCornerRadusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnCornerRadusChanged(e);
        }

        private static void OnItemSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.m_Children.SourceCollection = e.NewValue as IEnumerable;
        }

        /// <summary>
        /// Calls OnIsLogicalOwnershipEnabledChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsLogicalOwnershipEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnIsLogicalOwnershipEnabledChangedInternal(e);
        }

        /// <summary>
        /// Calls OnActiveDocumentChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnActiveDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            MDIWindowState state = MDIWindowState.Normal;
            if ((e.OldValue as FrameworkElement) != null)
               state= DocumentContainer.GetMDIWindowState(e.OldValue as FrameworkElement);         
            if (state == MDIWindowState.Maximized)
           {
                FrameworkElement element = e.NewValue as FrameworkElement;
                if(element!=null)
               DocumentContainer.SetMDIWindowState(element,state);
            }
            if (!instance.m_isInternalChangeActiveDocument)
            {
                instance.SetActiveDocument((UIElement)e.NewValue);
                instance.MainWindowHeaderActive = false;
            }
            if (instance.Mode == DocumentContainerMode.TDI)
            {
                TDILayoutPanel tdipanel = instance.m_layoutPanel as TDILayoutPanel;
                if (tdipanel != null)
                {
                    TabItemExt olditem = tdipanel.GetTabItem(e.OldValue as UIElement);
                    TabItemExt newitem = tdipanel.GetTabItem(e.NewValue as UIElement);
                    if (newitem != null)
                    {
                        if (newitem.TabControlParent != null)
                        {
                            newitem.IsTabGroupFocus = true;
                            newitem.TabControlParent.IsTabGroupFocus = true;
                        }
                        ContentPresenter headpresenter = newitem.Template.FindName("Content", newitem) as ContentPresenter;
                        if (headpresenter != null && newitem!=null)
                        {
                            if (newitem.Parent != null)
                            {
                                TextElement.SetFontWeight(headpresenter, ((DocumentTabControl)newitem.Parent).SelectedItemFontWeight);
                            }
                            else
                            {
                                TextElement.SetFontWeight(headpresenter, FontWeights.SemiBold);
                            }
                        }
                        else if (headpresenter != null)
                        {
                            TextElement.SetFontWeight(headpresenter, FontWeights.SemiBold);
                        }
                    }
                    if (olditem != null)
                    {
                        if (olditem.TabControlParent != null && newitem != null && newitem.TabControlParent != null)
                        {
                            if (olditem.TabControlParent != newitem.TabControlParent)
                            {
                                olditem.IsTabGroupFocus = false;
                                olditem.TabControlParent.IsTabGroupFocus = false;
                            }
                        }
                        ContentPresenter headpresenter = olditem.Template.FindName("Content", olditem) as ContentPresenter;
                        if (headpresenter != null && newitem != null)
                        {
                            TextElement.SetFontWeight(headpresenter, FontWeights.Normal);
                        }
                    }
                }

            }
            instance.SetActiveItem();
            instance.OnActiveDocumentChanged(e);
        }

        /// <summary>
        /// Calls OnMDIBoundsChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMDIBoundsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement)d;
            RoutedPropertyChangedEventArgs<Rect> args = new RoutedPropertyChangedEventArgs<Rect>(
                (Rect)e.OldValue, (Rect)e.NewValue, MDIBoundsChangedEvent);
            element.RaiseEvent(args);
        }

        /// <summary>
        /// Calls OnSizetoContentInMDIChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSizetoContentInMDIChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = d as FrameworkElement;
            if ((bool)e.NewValue && DocumentContainer.GetSizetoContentInternal(element))
            {
                Rect rect = DocumentContainer.GetMDIBounds(element);
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                if (element.DesiredSize.Height > 0 && element.DesiredSize.Width > 0)
                {
                    rect.Width = element.DesiredSize.Width + 6;
                    rect.Height = element.DesiredSize.Height + 27;
                    DocumentContainer.SetMDIBounds(element, rect);
                    DocumentContainer.SetSizetoContentInternal(element, false);
                }
            }
        }

        /// <summary>
        /// Called when [MDI minimized bounds changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMDIMinimizedBoundsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement)d;

            RoutedPropertyChangedEventArgs<Rect> args = new RoutedPropertyChangedEventArgs<Rect>((Rect)e.OldValue, (Rect)e.NewValue, MDIMinimizedBoundsChangedEvent);

            element.RaiseEvent(args);
        }

        /// <summary>
        /// Calls OnDisabledButtonsBehaviorChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDisabledButtonsBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnDisabledButtonsBehaviorChanged(e);
        }

        /// <summary>
        /// Calls OnDocumentContainerChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDocumentContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(MDIWindowStateProperty);
        }

        /// <summary>
        /// Calls OnMDIWindowStateChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMDIWindowStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = d as FrameworkElement;
            DocumentContainer container = VisualUtils.FindAncestor(element, typeof(DocumentContainer)) as DocumentContainer;
            if (container == null)
                container = GetDocumentContainer(element);
            if (element != null)
            {
                RoutedPropertyChangedEventArgs<MDIWindowState> args =
                    new RoutedPropertyChangedEventArgs<MDIWindowState>((MDIWindowState)e.OldValue, (MDIWindowState)e.NewValue, MDIWindowStateChanged);

                element.RaiseEvent(args);
                if (container != null)
                {
                    MDILayoutPanel mdipanel = container.m_layoutPanel as MDILayoutPanel;
                    if ((MDIWindowState)e.NewValue == MDIWindowState.Maximized && container.IsInMDIMaximizedState)
                    {
                        for (int i = 0; i < mdipanel.Wrappers.Count; i++)
                        {
                            bool CanExecute = (element is MDIWindow) ? mdipanel.Wrappers[i] != element : mdipanel.Wrappers[i].Content != element;
                            if ((DocumentContainer.GetMDIWindowState(mdipanel.Wrappers[i]) == MDIWindowState.Maximized || DocumentContainer.GetMDIWindowState(mdipanel.Wrappers[i].Content) == MDIWindowState.Maximized)
                                && CanExecute)
                            {
                                mdipanel.Wrappers[i].SetNormalState();
                            }
                        }
                    }
                    container.ExecuteWindowState(element);
                }
            }

        }

        internal void ExecuteWindowState(FrameworkElement element)
        {
            DocumentContainer container = VisualUtils.FindAncestor(element, typeof(DocumentContainer)) as DocumentContainer;
            if (container == null)
                container = GetDocumentContainer(element);
            if (container != null)
            {
               MDIWindow window = element as MDIWindow;
               if (window == null) 
                   window = VisualUtils.FindAncestor(element, typeof(MDIWindow)) as MDIWindow;
             
                if (window != null)
                {
                    switch (DocumentContainer.GetMDIWindowState(element))
                    {
                        case MDIWindowState.Maximized:
                            {
                                window.SetMaximizeState();
                                break;
                            }
                        case MDIWindowState.Minimized:
                            {
                                window.SetMinimizedState();
                                break;
                            }
                        case MDIWindowState.Normal:
                            {
                                window.SetNormalState();
                                break;
                            }
                    }
                }
            }
            
        }

        /// <summary>
        /// Called when [is keep circle changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="arg">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsKeepCircleChanged(DependencyObject d, DependencyPropertyChangedEventArgs arg)
        {
            DocumentContainer container = (DocumentContainer)d;
            container.OnIsKeepCircleChanged(arg);
        }

        /// <summary>
        /// Calls OnSwitchModeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSwitchModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.SwitchModeValidate((SwitchMode)e.NewValue);
            instance.OnSwitchModeChanged(e);
        }

        /// <summary>
        /// Calls OnModeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer owner = (DocumentContainer)d;
            if ((DocumentContainerMode)e.NewValue == DocumentContainerMode.TDI)
            {
                IsMDILayoutset = false;
                owner.MdiActiveElement = (FrameworkElement)owner.ActiveDocument;
            }
            owner.OnModeChanged(e);
        }

        /// <summary>
        /// Calls OnTDIFullScreenModeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTDIFullScreenModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer owner = (DocumentContainer)d;
            if (owner != null && owner.m_tabControlCollection.Count > 0)
            {
                foreach (DocumentTabControl tab in owner.m_tabControlCollection)
                {
                    BindingUtils.SetBinding(tab, owner, TabControlExt.FullScreenModeProperty, DocumentContainer.TDIFullScreenModeProperty);
                }
            }
            else if (owner != null)
            {
                owner.m_tdifullscreenmodechanged = true;
            }
        }

        /// <summary>
        /// Called when [TDI close button type changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTDICloseButtonTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer owner = (DocumentContainer)d;
            if (owner != null && owner.m_tabControlCollection.Count > 0)
            {
                foreach (DocumentTabControl tab in owner.m_tabControlCollection)
                {
                    BindingUtils.SetBinding(tab, owner, TabControlExt.CloseButtonTypeProperty, DocumentContainer.TDICloseButtonTypeProperty);
                }
            }
            else if (owner != null)
            {
                owner.m_tdiclosebuttontypechanged = true;
            }
        }

        /// <summary>
        /// Calls OnTDIToolBarTrayChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTDIToolBarTrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer owner = (DocumentContainer)d;
            if (owner != null && owner.m_tabControlCollection.Count > 0)
            {
                foreach (DocumentTabControl tab in owner.m_tabControlCollection)
                {
                    BindingUtils.SetBinding(tab, owner, TabControlExt.ToolBarTrayProperty, DocumentContainer.TDIToolBarTrayProperty);
                }
            }
            else if (owner != null)
            {
                owner.m_tditoolbartraychanged = true;
            }
        }

        /// <summary>
        /// Calls OnIsDocumentStateRequiredChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsDocumentStateRequiredChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnIsDocumentStateRequiredChanged(e);
        }

        /// <summary>
        /// Calls OnCanMDIMaximizeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCanMDIMaximizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnCanMDIMaximizeChanged(e);
        }

        /// <summary>
        /// Calls OnCanMDIMinimizeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCanMDIMinimizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnCanMDIMinimizeChanged(e);
        }

        /// <summary>
        /// Calls OnAllowMDIResizeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAllowMDIResizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(CanMDIMaximizeProperty);
        }

        /// <summary>
        /// Calls OnDelayPreviewTimeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDelayPreviewTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.m_Timer.Interval = (TimeSpan)e.NewValue;
            instance.OnDelayPreviewTimeChanged(e);
        }

        /// <summary>
        /// Calls OnMDICommandsTargetChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMDICommandsTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnMDICommandsTargetChanged(e);
        }

        /// <summary>
        /// Calls OnIsEnabledScrollChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsEnabledScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnIsEnabledScrollChanged(e);
        }

        /// <summary>
        /// Calls OnPersistStateChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPersistStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnPersistStateChanged(e);
        }

        /// <summary>
        /// Called when [context menu item collection changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnContextMenuItemCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Calls OnDefaultMenuItemsPanelTemplateChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDefaultMenuItemsPanelTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnDefaultMenuItemsPanelTemplateChanged(e);
        }

        /// <summary>
        /// Calls OnIsAllowMDIResizeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsAllowMDIResizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnIsAllowMDIResizeChanged(e);
        }

        /// <summary>
        /// Calls OnUseFlyCloseChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnUseFlyCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.OnUseFlyCloseChanged(e);
        }

        /// <summary>
        /// Calls OnUseInteropCompatibilityChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnUseInteropCompatibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;
            instance.CoerceValue(DocumentContainer.IsEnabledScrollProperty);
            instance.OnUseInteropCompatibilityChanged(e);
        }

        /// <summary>
        /// Calls OnShowingFlipControlChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnShowingFlipControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer instance = (DocumentContainer)d;

            foreach (UIElement obj in  instance.ILayoutPanel.GetOrderedItems())
            {
                obj.CoerceValue(DocumentContainer.MDIWindowStateProperty);
            }

            instance.OnShowingFlipControlChanged(e);
        }

        internal static void OnIsLogicalChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DocumentContainer doccontainer = DocumentContainer.GetDocumentContainer(d);
            if (doccontainer != null && doccontainer.Items.Contains(d as FrameworkElement))
            {
                doccontainer.AddLogicalChild(d);
            }
        }
        #endregion

        #region Coercements
        /// <summary>
        /// Coerces the MDI bounds validate.
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns> object rect</returns>
        public static object CoerceMDIBoundsValidate(DependencyObject d, object baseValue)
        {
            Rect rect = (Rect)baseValue;

            if (MINIMIZED_WIDTH > rect.Width)
            {
                rect.Width = MINIMIZED_WIDTH;
            }

            if (MINIMIZED_HEIGHT > rect.Height)
            {
                rect.Height = MINIMIZED_HEIGHT;
            }

            return rect;
        }

        /// <summary>
        /// Coerces the value of the MDIWindow state attached property.
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="baseValue">The base value DependencyObject.</param>
        /// <returns>object baseValue</returns>
        private static object CoerceMDIWindowStateProperty(DependencyObject d, object baseValue)
        {
            DocumentContainer container = GetDocumentContainer(d);

            if (container == null)
            {
                return baseValue;
            }

            return container.CanMDIMinimize
                     && !(container.ShowingFlipControl && SwitchMode.VistaFlip == container.SwitchMode)
                        ? (MDIWindowState)baseValue
                        : MDIWindowState.Normal;
        }

        /// <summary>
        /// Coerces IsInMDIMaximizedState property depending on the CanMDIMaximize property value.
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="baseValue">The base value DependencyObject.</param>
        /// <returns>object baseValue </returns>
        private static object CoerceIsInMDIMaximizedStateProperty(DependencyObject d, object baseValue)
        {
            DocumentContainer container = (DocumentContainer)d;
            return container.CanMDIMaximize && (bool)baseValue;
        }

        /// <summary>
        /// Coerces IsInMDIMaximizedState property depending on the CanMDIMaximize property value.
        /// </summary>
        /// <param name="d">The d DependencyObject. </param>
        /// <param name="baseValue">The base value DependencyObject.</param>
        /// <returns>object baseValue</returns>
        private static object CoerceCanMDIMaximizeProperty(DependencyObject d, object baseValue)
        {
            DocumentContainer container = (DocumentContainer)d;
            return DocumentContainer.GetAllowMDIResize(container) && (bool)baseValue;
        }

        /// <summary>
        /// Coerces the is keep circle.
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="baseValue">The base value DependencyObject.</param>
        /// <returns> object baseValue</returns>
        private static object CoerceIsKeepCircle(DependencyObject d, object baseValue)
        {
            DocumentContainer owner = (DocumentContainer)d;
            return DocumentContainerMode.TDI == owner.Mode ? true : baseValue;
        }

        /// <summary>
        /// Coerces the is enabled scroll.
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="baseValue">The base value DependencyObject.</param>
        /// <returns>object baseValue</returns>
        private static object CoerceIsEnabledScroll(DependencyObject d, object baseValue)
        {
            DocumentContainer owner = (DocumentContainer)d;
            return !owner.UseInteropCompatibility && (bool)baseValue;
        }
        #endregion

        #region Dependency Properies

        public static readonly DependencyProperty IsLazyLoadedProperty = DependencyProperty.Register("IsLazyLoaded", typeof(bool), typeof(DocumentContainer), new PropertyMetadata(false));

        /// <summary>
        /// Presents corner radius.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(DocumentContainer), new FrameworkPropertyMetadata(new CornerRadius(), new PropertyChangedCallback(OnCornerRadusChanged)));

        /// <summary>
        /// Shows to be children as logical children whether not.
        /// </summary>
        public static readonly DependencyProperty IsLogicalOwnershipEnabledProperty = DependencyProperty.Register("IsLogicalOwnershipEnabled", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsLogicalOwnershipEnabledChanged)));

        /// <summary>
        /// Presents bound of window in MDI mode.
        /// </summary>
        public static readonly DependencyProperty MDIBoundsProperty = DependencyProperty.RegisterAttached("MDIBounds", typeof(Rect), typeof(DocumentContainer), new FrameworkPropertyMetadata(new Rect(0, 0, 100, 50), new PropertyChangedCallback(OnMDIBoundsChanged), new CoerceValueCallback(CoerceMDIBoundsValidate)));

        /// <summary>
        /// Presents bound of window in MDI mode.
        /// </summary>
        public static readonly DependencyProperty SizetoContentInMDIProperty = DependencyProperty.RegisterAttached("SizetoContentInMDI", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnSizetoContentInMDIChanged)));

        /// <summary>
        /// Presents bound of window in MDI mode.
        /// </summary>
        internal static readonly DependencyProperty SizetoContentInternalProperty = DependencyProperty.RegisterAttached("SizetoContentInternal", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Presents MDI window size in minimized state.
        /// </summary>
        public static readonly DependencyProperty MDIMinimizedBoundsProperty = DependencyProperty.RegisterAttached("MDIMinimizedBounds", typeof(Rect), typeof(DocumentContainer), new FrameworkPropertyMetadata(new Rect(new Point(0, 0), new Point(MINIMIZED_WIDTH, MINIMIZED_HEIGHT)), new PropertyChangedCallback(OnMDIMinimizedBoundsChanged)));

        /// <summary>
        /// Presents active document.
        /// </summary>
        public static readonly DependencyProperty ActiveDocumentProperty = DependencyProperty.Register("ActiveDocument", typeof(UIElement), typeof(DocumentContainer), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnActiveDocumentChanged)));

        /// <summary>
        /// Presents to have maximized state whether not.
        /// </summary>
        public static readonly DependencyProperty IsInMDIMaximizedStateProperty = DependencyProperty.Register("IsInMDIMaximizedState", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsInMDIMaximizedStateChanged), new CoerceValueCallback(CoerceIsInMDIMaximizedStateProperty)));

        /// <summary>
        /// Specifies header of the document. This is an attached dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DockingManager.HeaderProperty.AddOwner(typeof (DocumentContainer));

        /// <summary>
        /// Specifies the IsTDIDragDropEnabled attached property
        /// </summary>
        public static readonly DependencyProperty IsTDIDragDropEnabledProperty = DependencyProperty.Register("IsTDIDragDropEnabled",typeof(bool),typeof(DocumentContainer), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Specifies data template, used to display header of the document. This is an attached dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty = DockingManager.HeaderTemplateProperty.AddOwner(typeof(DocumentContainer));

        /// <summary>
        /// Specifies an icon, used for documents. This is an inheritable attached dependency property.
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.RegisterAttached("Icon", typeof(Brush), typeof(DocumentContainer), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Presents behavior for disabled button.
        /// </summary>
        public static readonly DependencyProperty DisabledButtonsBehaviorProperty = DependencyProperty.Register("DisabledButtonsBehavior", typeof(DisabledButtonsBehavior), typeof(DocumentContainer), new FrameworkPropertyMetadata(DisabledButtonsBehavior.Disable, new PropertyChangedCallback(OnDisabledButtonsBehaviorChanged)));

        /// <summary>
        /// Presents key for DocCon as attach property in children.
        /// </summary>
        public static readonly DependencyPropertyKey DocumentContainerPropertyKey = DependencyProperty.RegisterAttachedReadOnly("DocumentContainer", typeof(DocumentContainer), typeof(DocumentContainer), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDocumentContainerChanged)));

        /// <summary>
        ///  Presents DocCon as attach property in children.
        /// </summary>
        public static readonly DependencyProperty DocumentContainerProperty = DocumentContainerPropertyKey.DependencyProperty;

        /// <summary>
        /// presents state for MDI windows.
        /// </summary>
        public static readonly DependencyProperty MDIWindowStateProperty = DependencyProperty.RegisterAttached("MDIWindowState", typeof(MDIWindowState), typeof(DocumentContainer), new FrameworkPropertyMetadata(MDIWindowState.Normal, new PropertyChangedCallback(OnMDIWindowStateChanged), new CoerceValueCallback(CoerceMDIWindowStateProperty)));

        /// <summary>
        /// Identified DocumentContainer.DocumentTabItemContextMenuItems dependency property
        /// </summary>
        public static readonly DependencyProperty DocumentTabItemContextMenuItemsProperty =
          DependencyProperty.RegisterAttached("DocumentTabItemContextMenuItems", typeof(DocumentTabItemMenuItemCollection), typeof(DocumentContainer), new FrameworkPropertyMetadata(new DocumentTabItemMenuItemCollection()));

        /// <summary>
        /// Presents mode for change active item.
        /// </summary>
        public static readonly DependencyProperty IsKeepCircleProperty = DependencyProperty.Register("IsKeepCircle", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsKeepCircleChanged), new CoerceValueCallback(CoerceIsKeepCircle)));

        /// <summary>
        /// Identifies DocumentContainer.SwitchModeProperty dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        public static readonly DependencyProperty SwitchModeProperty = DependencyProperty.Register("SwitchMode", typeof(SwitchMode), typeof(DocumentContainer), new FrameworkPropertyMetadata(SwitchMode.Immediate, new PropertyChangedCallback(OnSwitchModeChanged)));

        /// <summary>
        /// Gets or sets current representation mode of the container. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(DocumentContainerMode), typeof(DocumentContainer), new FrameworkPropertyMetadata(DocumentContainerMode.MDI, new PropertyChangedCallback(OnModeChanged)));

        /// <summary>
        /// Gets or sets current representation TDIFullScreenMode of the container. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty TDIFullScreenModeProperty = DependencyProperty.Register("TDIFullScreenMode", typeof(FullScreenMode), typeof(DocumentContainer), new FrameworkPropertyMetadata(FullScreenMode.None, new PropertyChangedCallback(OnTDIFullScreenModeChanged)));

        /// <summary>
        /// Identifies DocumentContainer.IsTouchEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTouchEnabledProperty =
            DependencyProperty.Register("IsTouchEnabled", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets current closebutton type of the container. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty TDICloseButtonTypeProperty = DependencyProperty.Register("TDICloseButtonType", typeof(CloseButtonType), typeof(DocumentContainer), new FrameworkPropertyMetadata(CloseButtonType.Common, new PropertyChangedCallback(OnTDICloseButtonTypeChanged)));

        /// <summary>
        /// Gets or sets current representation TDIToolBarTray of the container. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty TDIToolBarTrayProperty = DependencyProperty.Register("TDIToolBarTray", typeof(ToolBarTray), typeof(DocumentContainer), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTDIToolBarTrayChanged)));

        /// <summary>
        /// Identifies DocumentContainer.IsDocumentStateRequiredProperty dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        public static readonly DependencyProperty IsDocumentStateRequiredProperty = DependencyProperty.Register("IsDocumentStateRequired", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsDocumentStateRequiredChanged)));

        /// <summary>
        /// Presents can document will be maximized whether not.
        /// </summary>
        public static readonly DependencyProperty CanMDIMaximizeProperty = DependencyProperty.Register("CanMDIMaximize", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnCanMDIMaximizeChanged), new CoerceValueCallback(CoerceCanMDIMaximizeProperty)));

        /// <summary>
        /// Presents can document will be minimized whether not.
        /// </summary>
        public static readonly DependencyProperty CanMDIMinimizeProperty = DependencyProperty.Register("CanMDIMinimize", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnCanMDIMinimizeChanged)));

        /// <summary>
        /// Presents can document will be closed whether not.
        /// </summary>
        public static readonly DependencyProperty CanCloseProperty = DockingManager.CanCloseProperty.AddOwner(typeof(DocumentContainer));

        /// <summary>
        /// Presents the document will be closed based on e.Cancel
        /// </summary>
        internal static readonly DependencyProperty CanCloseArgProperty = DependencyProperty.Register("CanCloseArg", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(false));
        /// <summary>
        /// Presents can document will be resize whether not.
        /// </summary>
        public static readonly DependencyProperty AllowMDIResizeProperty = DependencyProperty.RegisterAttached("AllowMDIResize", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnAllowMDIResizeChanged)));

        /// <summary>
        /// This property presents time of delay for preview is showed.
        /// </summary>
        public static readonly DependencyProperty DelayPreviewTimeProperty = DependencyProperty.Register("DelayPreviewTime", typeof(TimeSpan), typeof(DocumentContainer), new FrameworkPropertyMetadata(TimeSpan.FromMilliseconds(0.2), new PropertyChangedCallback(OnDelayPreviewTimeChanged)));

        /// <summary>
        /// Identifies DocumentContainer.DocumentPreviewProperty dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        public static readonly DependencyProperty DocumentPreviewProperty = DependencyProperty.RegisterAttached("DocumentPreview", typeof(Brush), typeof(DocumentContainer), new UIPropertyMetadata(null));

        /// <summary>
        /// This property indicates a scroll support.
        /// <remarks>Only for MDI mode.</remarks>
        /// </summary>
        public static readonly DependencyProperty IsEnabledScrollProperty = DependencyProperty.Register("IsEnabledScroll", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsEnabledScrollChanged), new CoerceValueCallback(CoerceIsEnabledScroll)));

        /// <summary>
        /// Persist state.
        /// </summary>
        public static readonly DependencyProperty PersistStateProperty = DependencyProperty.Register("PersistState", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnPersistStateChanged)));

        /// <summary>
        /// Indicates the context menu items collection
        /// </summary>
        public static readonly DependencyProperty MDIContextMenuItemsCollectionProperty = DependencyProperty.RegisterAttached("MDIContextMenuItemsCollection", typeof(DocumentContextMenuItemsCollection), typeof(DocumentContainer), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContextMenuItemCollectionChanged)));

        /// <summary>
        /// Presents command menu, for add custom panel there.
        /// </summary>
        public static readonly DependencyProperty IsCommandMenuProperty = DependencyProperty.RegisterAttached("IsCommandMenu", typeof(bool), typeof(DockingManager), new UIPropertyMetadata(false));

        /// <summary>
        /// Presents tab's ToolTip in TDI mode.
        /// </summary>
        public static readonly DependencyProperty TabCaptionToolTipProperty = DependencyProperty.RegisterAttached("TabCaptionToolTip", typeof(object), typeof(DocumentContainer), new UIPropertyMetadata(null));

        /// <summary>
        /// Presents ItemsPanelTemplate of command menu.
        /// </summary>
        public static readonly DependencyProperty MenuItemsPanelTemplateProperty = DependencyProperty.Register("MenuItemsPanelTemplate", typeof(ItemsPanelTemplate), typeof(DocumentContainer), new UIPropertyMetadata(null));

        /// <summary>
        /// Presents property  for global allow/deny resize of MDI window.
        /// </summary>
        public static readonly DependencyProperty IsAllowMDIResizeProperty = DependencyProperty.Register("IsAllowMDIResize", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsAllowMDIResizeChanged)));

        /// <summary>
        /// Presents style of close MDIWindow.
        /// </summary>
        public static readonly DependencyProperty UseFlyCloseProperty = DependencyProperty.Register("UseFlyClose", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnUseFlyCloseChanged)));

        /// <summary>
        /// Identifies DocumentContainer.MDICommandsTargetPropertyKey  property.
        /// </summary>
        protected static readonly DependencyPropertyKey MDICommandsTargetPropertyKey = DependencyProperty.RegisterReadOnly("MDICommandsTarget", typeof(object), typeof(DocumentContainer), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnMDICommandsTargetChanged)));

        /// <summary>
        /// Identifies DocumentContainer.MDICommandsTargetProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty MDICommandsTargetProperty = MDICommandsTargetPropertyKey.DependencyProperty;

        /// <summary>
        /// Identifies DocumentContainer.DefaultMenuItemsPanelTemplatePropertyKey dependency property.
        /// </summary>
        protected static readonly DependencyPropertyKey DefaultMenuItemsPanelTemplatePropertyKey = DependencyProperty.RegisterReadOnly("DefaultMenuItemsPanelTemplate", typeof(ItemsPanelTemplate), typeof(DocumentContainer), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDefaultMenuItemsPanelTemplateChanged)));

        /// <summary>
        /// presents key for  DocumentContainer.DefaultMenuItemsPanelTemplateProperty.
        /// </summary>
        public static readonly DependencyProperty DefaultMenuItemsPanelTemplateProperty = DefaultMenuItemsPanelTemplatePropertyKey.DependencyProperty;

        /// <summary>
        /// Indicate that control has WinForm controls.
        /// </summary>
        public static readonly DependencyProperty UseInteropCompatibilityProperty = DependencyProperty.Register("UseInteropCompatibility", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnUseInteropCompatibilityChanged)));

        /// <summary>
        /// Indicate that whether tabpreview controls.
        /// </summary>
        public static readonly DependencyProperty IsTabPreviewEnabledProperty = DependencyProperty.Register("IsTabPreviewEnabled", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// presents key for protect ShowingFlipControlProperty.
        /// </summary>
        protected static readonly DependencyPropertyKey ShowingFlipControlPropertyKey = DependencyProperty.RegisterReadOnly("ShowingFlipControl", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnShowingFlipControlChanged)));

        /// <summary>
        /// Presents property for indicate whether preview control is showing now.
        /// </summary>
        public static readonly DependencyProperty ShowingFlipControlProperty = ShowingFlipControlPropertyKey.DependencyProperty;

        /// <summary>
        /// Presents property for customizing the Document Tab Header's Style 
        /// </summary>
        public static readonly DependencyProperty DocumentTabItemStyleProperty = DependencyProperty.RegisterAttached("DocumentTabItemStyle", typeof(Style), typeof(DocumentContainer), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Presents property for customizing the Document Tab Control's Style 
        /// </summary>
        public static readonly DependencyProperty DocumentTabControlStyleProperty = DependencyProperty.RegisterAttached("DocumentTabControlStyle", typeof(Style), typeof(DocumentContainer), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Presents property for customizing the Document MDI Header's Style 
        /// </summary>
        public static readonly DependencyProperty DocumentMDIHeaderStyleProperty =
                    DependencyProperty.RegisterAttached("DocumentMDIHeaderStyle", typeof(Style), typeof(DocumentContainer), new FrameworkPropertyMetadata(null));
       
        /// <summary>
        /// Presents property for customizing the Document Tab Header's create position 
        /// </summary>
        public static readonly DependencyProperty AddTabDocumentAtLastProperty = DependencyProperty.Register("AddTabDocumentAtLast", typeof(bool), typeof(DocumentContainer), new FrameworkPropertyMetadata(false));

        // Using a DependencyProperty as the backing store for Itemssource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(DocumentContainer), new UIPropertyMetadata(null, new PropertyChangedCallback(OnItemSourcePropertyChanged)));

        // Using a DependencyProperty as the backing store for CanDrag.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanDragProperty = DockingManager.CanDragTabProperty.AddOwner(typeof(DocumentContainer));


        // Using a DependencyProperty as the backing store for HasLazyLoading.  This enables animation, styling, binding, etc...
        //This property is implemented for IsLazyloading scenario.
        internal static readonly DependencyProperty IsLogicalChildProperty = DependencyProperty.RegisterAttached("IsLogicalChild",typeof(bool),typeof(DocumentContainer),new PropertyMetadata(false,new PropertyChangedCallback(OnIsLogicalChildChanged))) ;
           
        #endregion
    }
}