#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Docking Manager class.
    /// </summary>
    public partial class DockingManager
    {
        /// <summary>
        /// Occurs when [preview window closed].
        /// </summary>
        public event CloseEventHander PreviewWindowClosed;
        /// <summary>
        /// Occurs when [window closed].
        /// </summary>
        public event WindowCloseEventHander WindowClosed;
        /// <summary>
        /// Occurs when [preview window pinned].
        /// </summary>
        public event PreviewWindowPinnedEventHander PreviewWindowPinned;
        /// <summary>
        /// Occurs when [window pinned].
        /// </summary>
        public event WindowPinnedEventHander WindowPinned;
        /// <summary>
        /// Occurs when [preview window un pinned].
        /// </summary>
        public event PreviewWindowUnPinnedEventHander PreviewWindowUnPinned;
        /// <summary>
        /// Occurs when [window un pinned].
        /// </summary>
        public event WindowUnPinnedEventHander WindowUnPinned;

        /// <summary>
        /// Occurs when [drag allow].
        /// </summary>
        public event DragAllowEventHandler DragAllow;

        /// <summary>
        /// Occurs when [dock allow].
        /// </summary>
        public event DockAllowEventHandler DockAllow;

        /// <summary>
        /// Occurs when [dock state changed].
        /// </summary>
        public event DockStateChangedEventHandler DockStateChanged;

        /// <summary>
        /// Occurs when DickState is changing
        /// </summary>
        public event DockStateChangingEventHandler DockStateChanging;

        /// <summary>
        /// Occurs if there is any control not initialized while loading states.
        /// </summary>
        public event InitializeControlOnLoadEventHandler InitializeControlOnLoad;

        /// <summary>
        /// Occurs before the context menu is opened
        /// </summary>
        public event DockContextMenuEventHandler DockContextMenu;

        /// <summary>
        /// Occurs when the window is maximized
        /// </summary>
        public event MaximizedEventHandler Maximized;

        /// <summary>
        /// Occurs when the window is maximizing
        /// </summary>
        public event MaximizingEventHandler Maximizing;

        /// <summary>
        /// Occurs when the autohide context menu is opening
        /// </summary>
        public event AutoHideTabContextMenuOpenEventHandler AutoHideTabContextMenuOpen;

        /// <summary>
        /// Invokes the preview event.
        /// </summary>
        /// <param name="elem">The elem.</param>
        /// <param name="eventCanceled">if set to <c>true</c> [event canceled].</param>
        protected internal void InvokePreviewEvent(UIElement elem, ref bool eventCanceled)
        {
            if (PreviewWindowClosed != null)
            {
                CloseEventArgs closeEventArgs = new CloseEventArgs(false, elem);
                PreviewWindowClosed(this, closeEventArgs);
                eventCanceled = closeEventArgs.Canceled;
            }
        }

        /// <summary>
        /// Invokes the event.
        /// </summary>
        /// <param name="elem">The elem.</param>
        protected internal void InvokeEvent(UIElement elem)
        {
            if (WindowClosed != null)
            {
                WindowCloseEventArgs windowcloseEventArgs = new WindowCloseEventArgs(elem);
                WindowClosed(this, windowcloseEventArgs);                
            }
        }


        /// <summary>
        /// Invokes the preview eventfor pinned.
        /// </summary>
        /// <param name="elem">The elem.</param>
        /// <param name="eventCanceled">if set to <c>true</c> [event canceled].</param>
        protected internal void InvokePreviewEventforPinned(UIElement elem, ref bool eventCanceled)
        {
            if (PreviewWindowClosed != null)
            {
                PreviewWindowPinnedEventArgs previewWindowPinnedEventArgs = new PreviewWindowPinnedEventArgs(false, elem);
                PreviewWindowPinned(this, previewWindowPinnedEventArgs);
                eventCanceled = previewWindowPinnedEventArgs.Canceled;
            }
        }

        /// <summary>
        /// Invokes the event for pinned.
        /// </summary>
        /// <param name="elem">The elem.</param>
        protected internal void InvokeEventForPinned(UIElement elem)
        {
            if (WindowPinned != null)
            {
                WindowPinnedEventArgs windowPinnedEventArgs = new WindowPinnedEventArgs(elem);
                WindowPinned(this, windowPinnedEventArgs);        
            }
        }


        /// <summary>
        /// Invokes the preview eventfor un pinned.
        /// </summary>
        /// <param name="elem">The elem.</param>
        /// <param name="eventCanceled">if set to <c>true</c> [event canceled].</param>
        protected internal void InvokePreviewEventforUnPinned(UIElement elem, ref bool eventCanceled)
        {
            if (PreviewWindowClosed != null)
            {
                PreviewWindowUnPinnedEventArgs previewWindowUnPinnedEventArgs = new PreviewWindowUnPinnedEventArgs(false, elem);
                PreviewWindowUnPinned(this, previewWindowUnPinnedEventArgs);
                eventCanceled = previewWindowUnPinnedEventArgs.Canceled;
            }
        }

        /// <summary>
        /// Invokes the eventfor un pinned.
        /// </summary>
        /// <param name="elem">The elem.</param>
        protected internal void InvokeEventforUnPinned(UIElement elem)
        {
            if (WindowUnPinned != null)
            {
                WindowUnPinnedEventArgs windowUnPinnedEventArgs = new WindowUnPinnedEventArgs(elem);
                WindowUnPinned(this, windowUnPinnedEventArgs);
            }
        }

        /// <summary>
        /// Fires the drag allow event.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Tools.Controls.DragAllowEventArgs"/> instance containing the event data.</param>
        protected internal void FireDragAllow(DragAllowEventArgs args)
        {
            if (DragAllow != null)
                DragAllow(this, args);
        }

        /// <summary>
        /// Fires the dock allow event.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Tools.Controls.DockAllowEventArgs"/> instance containing the event data.</param>
        protected internal void FireDockAllow(DockAllowEventArgs args)
        {
            if (DockAllow != null)
                DockAllow(this, args);
        }

        /// <summary>
        /// Fires the DockStareChangedevent.
        /// </summary>
        /// <param name="uiElement"></param>
        /// <param name="oldState"></param>
        /// <param name="newState"></param>
        protected internal void FireDockStateChanged(UIElement uiElement, object oldState, object newState)
        {
            if (DockStateChanged != null)
            {
                DockStateChangedEventArgs args = new DockStateChangedEventArgs(uiElement, oldState, newState);
                DockStateChanged(this, args);
            }
        }

        /// <summary>
        /// Fires the DockState chnanging event
        /// </summary>
        /// <param name="args"></param>
        protected internal void FireDockStateChanging(DockStateChangingEventArgs args)
        {
            if (this.DockStateChanging != null)
                DockStateChanging(this, args);
        }

        /// <summary>
        /// Fires the DockContextMenuOpening event
        /// </summary>
        /// <param name="args"></param>
        protected internal void FireDockContextMenuOpening(DockContextMenuEventArgs args)
        {
            if (this.DockContextMenu != null)
                DockContextMenu(this, args);
        }

        /// <summary>
        /// Fires the maximizing event
        /// </summary>
        /// <param name="args"></param>
        protected internal void FireMaximizing(MaximizingEventArgs args)
        {
            if(this.Maximizing != null)
            {
                Maximizing(this, args);
            }
        }

        /// <summary>
        /// Fires Maximized event
        /// </summary>
        /// <param name="uiElement"></param>
        protected internal void FireMaximized(UIElement uiElement)
        {
            if (this.Maximized != null)
            {
                MaximizedEventArgs args = new MaximizedEventArgs(uiElement);
                Maximized(this, args);
            }
        }

        /// <summary>
        /// Fires AutoHideContextMenuOpen event
        /// </summary>
        /// <param name="args"></param>
        protected internal void FireAutoHideContextMenuOpen(AutoHideTabContextMenuOpenEventArgs args)
        {
            if(this.AutoHideTabContextMenuOpen != null)
            {
                AutoHideTabContextMenuOpen(this, args);
            }
        }
    }


    /// <summary>
    /// Represents the PreviewWindowUnPinned EventHander
    /// </summary>
    public delegate void PreviewWindowUnPinnedEventHander(object sender, PreviewWindowUnPinnedEventArgs args);
    /// <summary>
    /// Represents the PreviewWindowUnPinned EventArgs
    /// </summary>
    public class PreviewWindowUnPinnedEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewWindowUnPinnedEventArgs"/> class.
        /// </summary>
        public PreviewWindowUnPinnedEventArgs()
        {

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewWindowUnPinnedEventArgs"/> class.
        /// </summary>
        /// <param name="canceled">if set to <c>true</c> [canceled].</param>
        /// <param name="elem">The elem.</param>
        public PreviewWindowUnPinnedEventArgs(bool canceled, UIElement elem)
        {
            this.Canceled = canceled;
            this.element = elem;
        }

        /// <summary>
        /// Gets or sets the element.
        /// </summary>
        /// <value>The element.</value>
        public UIElement element
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PreviewWindowUnPinnedEventArgs"/> is canceled.
        /// </summary>
        /// <value><c>true</c> if canceled; otherwise, <c>false</c>.</value>
        public bool Canceled
        {
            get;
            set;
        }
    }





    /// <summary>
    /// Represents the WindowUnPinned EventHander.
    /// </summary>
    public delegate void WindowUnPinnedEventHander(object sender, WindowUnPinnedEventArgs args);

    /// <summary>
    /// Represents the WindowUnPinned EventArgs.
    /// </summary>
    public class WindowUnPinnedEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowUnPinnedEventArgs"/> class.
        /// </summary>
        public WindowUnPinnedEventArgs()
        {

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="WindowUnPinnedEventArgs"/> class.
        /// </summary>
        /// <param name="elem">The elem.</param>
        public WindowUnPinnedEventArgs(UIElement elem)
        {
            this.element = elem;
        }

        /// <summary>
        /// Gets or sets the element.
        /// </summary>
        /// <value>The element.</value>
        public UIElement element
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents the WindowPinned EventHander.
    /// </summary>
    public delegate void WindowPinnedEventHander(object sender, WindowPinnedEventArgs args);
    /// <summary>
    /// Represents the WindowPinned EventArgs.
    /// </summary>
    public class WindowPinnedEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowPinnedEventArgs"/> class.
        /// </summary>
        public WindowPinnedEventArgs()
        {

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="WindowPinnedEventArgs"/> class.
        /// </summary>
        /// <param name="elem">The elem.</param>
        public WindowPinnedEventArgs(UIElement elem)
        {
            this.element = elem;
        }

        /// <summary>
        /// Gets or sets the element.
        /// </summary>
        /// <value>The element.</value>
        public UIElement element
        {
            get;
            set;
        }
    }



    /// <summary>
    /// Represents the PreviewWindowPinned EventHander.
    /// </summary>
    public delegate void PreviewWindowPinnedEventHander(object sender, PreviewWindowPinnedEventArgs args);
    /// <summary>
    /// Represents the Preview Window Pinned Event args.
    /// </summary>
    public class PreviewWindowPinnedEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewWindowPinnedEventArgs"/> class.
        /// </summary>
        public PreviewWindowPinnedEventArgs()
        {

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewWindowPinnedEventArgs"/> class.
        /// </summary>
        /// <param name="canceled">if set to <c>true</c> [canceled].</param>
        /// <param name="elem">The elem.</param>
        public PreviewWindowPinnedEventArgs(bool canceled, UIElement elem)
        {
            this.Canceled = canceled;
            this.element = elem;
        }

        /// <summary>
        /// Gets or sets the element.
        /// </summary>
        /// <value>The element.</value>
        public UIElement element
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PreviewWindowPinnedEventArgs"/> is canceled.
        /// </summary>
        /// <value><c>true</c> if canceled; otherwise, <c>false</c>.</value>
        public bool Canceled
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents the CloseEvent Hander.
    /// </summary>
    public delegate void CloseEventHander(object sender, CloseEventArgs args);
    /// <summary>
    /// Class to represent dragging event arguments
    /// </summary>
    public class CloseEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseEventArgs"/> class.
        /// </summary>
        public CloseEventArgs()
        {

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="CloseEventArgs"/> class.
        /// </summary>
        /// <param name="canceled">if set to <c>true</c> [canceled].</param>
        /// <param name="elem">The elem.</param>
        public CloseEventArgs(bool canceled, UIElement elem)
        {
            this.Canceled = canceled;
            this.element = elem;
        }

        /// <summary>
        /// Gets or sets the element.
        /// </summary>
        /// <value>The element.</value>
        public UIElement element
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CloseEventArgs"/> is canceled.
        /// </summary>
        /// <value><c>true</c> if canceled; otherwise, <c>false</c>.</value>
        public bool Canceled
        {
            get;
            set;
        }
    }



    /// <summary>
    /// Represents the WindowClose EventHander.
    /// </summary>
    public delegate void WindowCloseEventHander(object sender, WindowCloseEventArgs args);

    /// <summary>
    /// Class to represent dragging event arguments
    /// </summary>
      public class WindowCloseEventArgs : EventArgs
      {

          /// <summary>
          /// Initializes a new instance of the <see cref="WindowCloseEventArgs"/> class.
          /// </summary>
          public WindowCloseEventArgs()
          {

          }


          /// <summary>
          /// Initializes a new instance of the <see cref="WindowCloseEventArgs"/> class.
          /// </summary>
          /// <param name="elem">The elem.</param>
          public WindowCloseEventArgs(UIElement elem)
          {
              this.element = elem;             
          }

          /// <summary>
          /// Gets or sets the element.
          /// </summary>
          /// <value>The element.</value>
          public UIElement element
          {
              get;
              set;
          }  
      }

      /// <summary>
      /// 
      /// </summary>
    public delegate void DragAllowEventHandler(object sender, DragAllowEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    public class DragAllowEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DragAllowEventArgs"/> class.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        public DragAllowEventArgs(UIElement uiElement): base()
        {
            this.UIElement = uiElement;
        }

        /// <summary>
        /// Gets or sets the UI element.
        /// </summary>
        /// <value>The UI element.</value>
        public UIElement UIElement
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public delegate void DockAllowEventHandler(object sender, DockAllowEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    public class DockAllowEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DockAllowEventArgs"/> class.
        /// </summary>
        /// <param name="draggedElement">The dragged element.</param>
        /// <param name="targetElement">The target element.</param>
        public DockAllowEventArgs(UIElement draggedElement, UIElement targetElement)
        {
            this.DraggedElement = draggedElement;
            this.TargetElement = targetElement;
        }

        /// <summary>
        /// Gets or sets the dragged element.
        /// </summary>
        /// <value>The dragged element.</value>
        public UIElement DraggedElement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the target element.
        /// </summary>
        /// <value>The target control.</value>
        public UIElement TargetElement
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public delegate void DockStateChangedEventHandler(object sender, DockStateChangedEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    public class DockStateChangedEventArgs: EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DockStateChangedEventArgs"/> class.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="oldState">The old state.</param>
        /// <param name="newState">State of the ne.</param>
        public DockStateChangedEventArgs(UIElement uiElement, object oldState,object newState)
        {
            this.UIElement = uiElement;
            this.OldState = oldState;
            this.NewState = newState;
        }

        /// <summary>
        /// Gets or sets the UI element.
        /// </summary>
        /// <value>The UI element.</value>
        public UIElement UIElement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the old state.
        /// </summary>
        /// <value>The old state.</value>
        public object OldState
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the new state.
        /// </summary>
        /// <value>The new state.</value>
        public object NewState
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void DockStateChangingEventHandler(object sender,  DockStateChangingEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    public class DockStateChangingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes new instance of DockStateChangingEventArgs
        /// </summary>
        /// <param name="uielement"></param>
        /// <param name="oldState"></param>
        /// <param name="newState"></param>
        /// <param name="targetSide"></param>
        public DockStateChangingEventArgs(UIElement uielement, object oldState, object newState,object targetSide)
        {
            this.UIElement = uielement;
            this.OldState = oldState;
            this.NewState = newState;
            this.TargetSide = targetSide;
        }

        /// <summary>
        /// Gets or Sets UIElement
        /// </summary>
        public UIElement UIElement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets old DockState
        /// </summary>
        public object OldState
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets new DockState
        /// </summary>
        public object NewState
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public object TargetSide
        {
            get; set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void InitializeControlOnLoadEventHandler(object sender, InitializeControlOnLoadEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    public class InitializeControlOnLoadEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes the InitializeControlOnLoadEventArgs class
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="control"></param>
        public InitializeControlOnLoadEventArgs(string caption, string control)
        {
            this.Caption = caption;
            this.Control = control;
        }

        /// <summary>
        /// Gets or Sets the Caption name of the window.
        /// </summary>
        public string Caption
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the control type.
        /// </summary>
        public string Control
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void DockContextMenuEventHandler(object sender, DockContextMenuEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    public class DockContextMenuEventArgs: CancelEventArgs
    {
        /// <summary>
        /// Initializes the DockContextMenuOpeningEventArgs class
        /// </summary>
        /// <param name="contextMenu"></param>
        /// <param name="uiElement"></param>
        public DockContextMenuEventArgs(ContextMenuAdv contextMenu, UIElement uiElement)
        {
            ContextMenu = contextMenu;
            UIElement = uiElement;
        }

        /// <summary>
        /// Gets or Sets ContextMenu
        /// </summary>
        public ContextMenuAdv ContextMenu
        { 
            get; 
            set;
        }

        /// <summary>
        /// Gets or Sets UIELement
        /// </summary>
        public UIElement UIElement
        { 
            get; 
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void MaximizingEventHandler(object sender, MaximizingEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    public class MaximizingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes instance for MaximizingEventArgs class
        /// </summary>
        /// <param name="uiElement"></param>
        public MaximizingEventArgs(UIElement uiElement)
        {
            UIElement = uiElement;
        }

        /// <summary>
        /// Gets or Sets the UIElement
        /// </summary>
        public UIElement UIElement
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void MaximizedEventHandler(object sender, MaximizedEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    public class MaximizedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes instance for MaximizedEventArgs class
        /// </summary>
        /// <param name="uiElement"></param>
        public MaximizedEventArgs(UIElement uiElement)
        {
            UIElement = uiElement;
        }

        /// <summary>
        /// Gets or Sets the UIElement
        /// </summary>
        public UIElement UIElement
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void AutoHideTabContextMenuOpenEventHandler(object sender, AutoHideTabContextMenuOpenEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    public class AutoHideTabContextMenuOpenEventArgs: CancelEventArgs
    {
        /// <summary>
        /// Initializes the new instance of AutoHideContextMenuOpenEventArgs
        /// </summary>
        /// <param name="contextMenu"></param>
        public AutoHideTabContextMenuOpenEventArgs(ContextMenuAdv contextMenu)
        {
            ContextMenu = contextMenu;
        }

        /// <summary>
        /// Gets or Sets the context menu
        /// </summary>
        public ContextMenuAdv ContextMenu
        {
            get; 
            set;
        }
    }
}
