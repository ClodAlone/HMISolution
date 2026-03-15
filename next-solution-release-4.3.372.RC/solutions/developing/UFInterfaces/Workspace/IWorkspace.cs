using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DocumentManager.ComponentService;

namespace UFInterfaces
{
    public interface IWorkspace : IUFInterfaceBase, IEditableObject
    {
        void AddDockingChildren(FrameworkElement el, String header,
                                UFInterfaces.DockState state, UFInterfaces.DockSide side, bool bCanClose = true, bool bCanFloat = true, UFInterfaces.DockSide layoutGroupDockSide = DockSide.Left, String itemID = null);

        bool IsDockedChildren(FrameworkElement el);

        void RemoveDockingChildren(FrameworkElement el);

        void ReloadDockState(FrameworkElement el, String layoutFilePath, VFS.FileSystemProviderBase fileSystemProvider = null);

        void ShowSystemLog();

        void FlashDockedElement(String header);

        void FlashDockedElement(FrameworkElement el);

        void SetChangedDocumentTitle(FrameworkElement el, bool bSet);
        void SetChangedDocumentTitle(FrameworkElement el, String title, bool bSet);

        void ActivateDockedElement(String header);

        void ActivateDockedElement(FrameworkElement el);

        void ActivatePreviousActiveElement();

        void SetDesiredHeightAndWidthInDockedMode(FrameworkElement el, double Height, double Width);

        void SetDesiredSideMode(FrameworkElement el, String target);

        void SetDockedElementIcon(FrameworkElement el, Brush b);

        UFInterfaces.DockState GetElementDockState(FrameworkElement el);

        void SetElementDockState(FrameworkElement el, UFInterfaces.DockState dockState);

        UFInterfaces.DockSide GetElementDockSide(FrameworkElement el);

        bool GetElementIsSelectedTab(FrameworkElement el);

        void AddMenuItem(MenuItem item, int nPos, CommandBindingCollection commandBinding);

        void AddBarManagerItem(FrameworkElement barManager, CommandBindingCollection globalCommandBindings, string componentTitle = "", string componentTypeScheme = "");

        void RemoveBarManagerItem(FrameworkElement barManager, CommandBindingCollection commandBinding, string componentTypeScheme = "");

        void AddBarManagerGlobalCommands(CommandBindingCollection commandBinding);

        void RemoveBarManagerCommands(CommandBindingCollection commandBinding);

        void RemoveMenuItem(MenuItem item, CommandBindingCollection commandBinding);

        void AddRibbonTabItem(RibbonTab item, int nPos, CommandBindingCollection commandBinding, String contextTabGroup, Color backColor);

        void RemoveRibbonTabItem(RibbonTab item, CommandBindingCollection commandBinding, String contextTabGroup, Color backColor);

        void AddRibbonsTabItem(List<RibbonTab> list, int nPos, CommandBindingCollection commandBinding, String contextTabGroup, Color backColor);

        void RemoveRibbonsTabItem(List<RibbonTab> list, CommandBindingCollection commandBinding, String contextTabGroup, Color backColor);

        void SetMainMenuVisibility(Visibility visibility);

        void SetRibbonVisibility(Visibility visibility);

        void ShowTaskBarTooltip(String Title, String Content, int nTimeout);

        void RegisterComponent(IComponent component);

        bool LayoutLoadRequest(string path);

        bool LayoutSaveRequest(string path, UserControl layoutContent = null, VFS.FileSystemProviderBase fileSystemProvider = null);

        bool HasContentRendered();

        bool IsWorkspaceLoaded { get; set; }
        bool IsBusy { get; set; }
        String BusyContent { get; set; }
        void ResetBusy();
        void RestoreBusy();

        void UpdateProgressState(int value, int maxvalue, string description, TaskbarItemProgressState state);
        void ResetProgressState();
        void IncrementProgressState(int step = 1);


        event EventHandler WorkspaceLoading;
        event EventHandler WorkspaceLoaded;
        event EventHandler ContentRendered;
        event EventHandler<CancelEventArgs> Closing;
        event EventHandler Closed;

        event RoutedEventHandler WindowActivated;
        event RoutedEventHandler WindowDeactivated;
        event RoutedEventHandler WindowDragEnd;
        event RoutedEventHandler WindowDragStart;
        event RoutedEventHandler WindowVisibilityChanged;

        event RoutedEventHandler BeforeContextMenuOpen;
        event RoutedEventHandler AutoHideAnimationStart;
        event RoutedEventHandler AutoHideAnimationStop;

        event SelectionChangedEventHandler RibbonSelectionChanged;

        event EventHandler<CancelEventArgs> ActiveWindowChanging;
        event PropertyChangedCallback ActiveWindowChanged;

        event OnCloseTabsEventHandler CloseAllTabs;
        event CloseButtonEventHandler CloseButtonClick;
        event OnCloseTabsEventHandler CloseOtherTabs;
        event DockStateHandler DockStateChanged;
        event EventHandler<CancelEventArgs> DockItemRestoring;
        event EventHandler DockItemRestored;
        event ElementHiddenEventHandler ElementHidden;
        event ElementShownEventHandler ElementShown;

        event EventHandler<CancelEventArgs> ContextContentChanging;
        event EventHandler ContextContentChanged;

        event EventHandler RefreshCurrentContents;
        void ForceRefreshCurrentContents();

        event EventHandler EasyModeChanged;
        bool IsInEasyMode { get; }

        string ThemeKey { get; }

        Object ContextObject { get; set; }
        IList ContextObjects { get; set; }
        IEntityReference GetEntityReferenceContextObject(Object obj);

        IDocument ContextDocument { get; set; }
        event EventHandler<CancelEventArgs> ContextDocumentChanging;
        event EventHandler ContextDocumentChanged;

        void UpdateContextNow();

        IList GetFriendObjects(Object friend, Type expectedType);

        bool IsComponentHidden(string componentName);

        event EventHandler<GetFriendObjectsEventArgs> PromptFriendObjects;

        Object GetDocumentEditorObject(Object obj);

        event EventHandler<GetDocumentEditorObjectEventArgs> PromptDocumentEditorObject;

        UserControl GetSmartTagsEditorObject(Object obj);

        event EventHandler<GetSmartTagsEditorObjectEventArgs> PromptSmartTagsEditorObject;

        FrameworkElement ActiveWindow { get; set; }

        void StatusText(String text);

        event EventHandler<ProgressStateChangedEventArgs> ProgressStateChanged;
    }
    /// <summary>
    /// Represents the thumbnail progress bar state.
    /// </summary>
    public enum TaskbarItemProgressState
    {
        //
        // Summary:
        //     No progress indicator is displayed in the taskbar button.
        None = 0,
        //
        // Summary:
        //     A pulsing green indicator is displayed in the taskbar button.
        Indeterminate = 1,
        //
        // Summary:
        //     A green progress indicator is displayed in the taskbar button.
        Normal = 2,
        //
        // Summary:
        //     A red progress indicator is displayed in the taskbar button.
        Error = 3,
        //
        // Summary:
        //     A yellow progress indicator is displayed in the taskbar button.
        Paused = 4
    }
    /// <summary>
    /// Describes how control is docked to it's container.
    /// </summary>
    public enum DockSide
    {
        /// <summary>
        /// Control is docked to the left side of it's container.
        /// </summary>
        Left,

        /// <summary>
        /// Control is docked to the right side of it's container.
        /// </summary>
        Top,

        /// <summary>
        /// Control is docked to the top side of it's container.
        /// </summary>
        Right,

        /// <summary>
        /// Control is docked to the bottom side of it's container.
        /// </summary>
        Bottom,

        /// <summary>
        /// Control is docked as a tab page of it's container.
        /// </summary>
        Tabbed,

        /// <summary>
        /// Control is not docked.
        /// </summary>
        None
    }

    /// <summary>
    /// Specifies the control state.
    /// </summary>
    public enum DockState
    {
        /// <summary>
        /// Control is docked to the docking manager's surface.
        /// </summary>
        Dock,

        /// <summary>
        /// Control is not docked to the docking manager's surface.
        /// </summary>
        Float,

        /// <summary>
        /// Control is not visible at all.
        /// </summary>
        Hidden,

        /// <summary>
        /// Control is hidden and will show if mouse move under tab.
        /// </summary>
        AutoHidden,

        /// <summary>
        /// Control is tabbed or MDI document.
        /// </summary>
        Document
    }

    public class CloseButtonEventArgs
    {
        public CancelEventArgs Cancel;
        public FrameworkElement TargetItem;
        public CloseButtonEventArgs()
        {
            
        }
        public CloseButtonEventArgs(FrameworkElement targetItem)
        {
            TargetItem = targetItem;
        }
    }

    public delegate void CloseButtonEventHandler (object sender, CloseButtonEventArgs e);

    public class DockStateEventArgs
    {
        public DockState NewState;
        public DockState OldState;
    }

    public delegate void DockStateHandler(FrameworkElement sender, DockStateEventArgs e);

    public class RibbonButton { }
    public class RibbonTab { }
    public class RibbonBar { }

    public delegate void ElementShownEventHandler(object sender, EventArgs e);
    public delegate void ElementHiddenEventHandler(object sender, EventArgs e);
    public delegate void OnCloseTabsEventHandler(object sender, EventArgs e);

    public class ProgressStateChangedEventArgs : EventArgs
    {
        public double Value;
        public double MaxValue;
        public string Description;
        public TaskbarItemProgressState State;
    }
}