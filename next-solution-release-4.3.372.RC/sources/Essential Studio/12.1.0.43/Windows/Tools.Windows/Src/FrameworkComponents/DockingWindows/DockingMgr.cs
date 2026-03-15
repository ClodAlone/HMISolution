#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.IO;
using Microsoft.Win32;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms.Design;
using System.Runtime.InteropServices;

using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Runtime.Serialization;
using Syncfusion.Windows.Forms.Design;
using Syncfusion.Windows.Forms.Tools.Design;
using Syncfusion.Windows.Forms.Tools.Renderers;
using Syncfusion.Windows.Forms.Tools.XPMenus;
using System.Xml.Serialization;
using System.Text;
using System.Xml;
using System.IO.IsolatedStorage;


namespace Syncfusion.Windows.Forms.Tools
{
	
# region Delegates & EventArgs classes
    /// <summary>
	/// Custom event argument class used for notifying users of dockstate changes.
	/// </summary>
	/// <remarks>
	/// The DockStateChangeEventArgs class is used by the <see cref="DockingManager"/> to
	/// provide event data for the <see cref="DockingManager.DockStateChanging"/> and <see cref="DockingManager.DockStateChanged"/> events.
	/// </remarks>
	/// <seealso cref="DockStateChangeEventHandler"/>
	public class DockStateChangeEventArgs : EventArgs
	{
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Control[] ctrlArray = null;

		/// <summary>
		/// Creates a new instance of the DockStateChangeEventArgs class.
		/// </summary>
		/// <param name="ctrls">The control(s) being docked/floated.</param>
		public DockStateChangeEventArgs(Control[] ctrls)
		{
			this.ctrlArray = ctrls;
		}

		/// <summary>
		/// Returns the collection of controls undergoing the dockstate transfer.
		/// </summary>
		public Control[] Controls
		{
			get { return this.ctrlArray; }
		}
	}

	/// <summary>
	/// Handles the <see cref="DockingManager.DockStateChanging"/> and
	/// <see cref="DockingManager.DockStateChanged"/> events.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="arg">A <see cref="DockStateChangeEventArgs"/> value that contains the event data.</param>
	public delegate void DockStateChangeEventHandler(Object sender, DockStateChangeEventArgs arg);


	/// <summary>
	/// Custom event argument base class used for providing data for docking window events.
	/// </summary>
	/// <remarks>
	/// The DockControlEventArgs class is used by the <see cref="DockingManager"/> to
	/// provide event data for a dockable control.
	/// </remarks>
	/// <seealso cref="DockVisibilityChangedEventArgs"/>
	/// <seealso cref="DockActivationChangedEventArgs"/>
	/// <seealso cref="AutoHideAnimationEventArgs"/>
	/// <seealso cref="DockStateUnavailableEventArgs"/>
	public class DockControlEventArgs : EventArgs
	{
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Control ctrl = null;

		/// <summary>
		/// Creates a new instance of the DockControlEventArgs class.
		/// </summary>
		/// <param name="ctrl">The control undergoing the state change.</param>
		public DockControlEventArgs(Control ctrl)
		{
			this.ctrl = ctrl;
		}

		/// <summary>
		/// Returns the control undergoing the state change.
		/// </summary>
		/// <value>A <see cref="System.Windows.Forms.Control"/> value.</value>
		public Control Control
		{
			get { return this.ctrl; }
		}
	}

    /// <summary>
    /// Handles the <see cref="DockingManager.DockControlMouseSelection"/> and
    /// <see cref="DockingManager.OnCaptionDoubleClicked"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="arg">A <see cref="DockControlMouseSelection"/> value that contains the selected control</param>
    public delegate void DockMouseSelectionEventHandler(Object sender, DockControlMouseSelection arg);
    /// <summary>
    /// Custom event argument class used for notifying users once mouse selection is done on control caption.
    /// </summary>
    public class DockControlMouseSelection : EventArgs
    {
        /// <summary>
        /// Gets control which is selected
        /// </summary>
        protected Control ctrl = null;

        /// <summary>
        /// Creates a new instance of the DockControlMouseSelection class.
        /// </summary>
        /// <param name="ctrl">Active Control</param>
        public DockControlMouseSelection(Control ctrl)
        {
            this.ctrl = ctrl;
        }

        /// <summary>
        /// Gets mouse selected control
        /// </summary>
        public Control Control
        {
            get { return this.ctrl; }
        }
    }

	/// <summary>
	/// Custom event argument class used for notifying users of DockVisibility state changes.
	/// </summary>
	/// <remarks>
	/// The DockVisibilityChangedEventArgs class is used by the <see cref="DockingManager"/> to
	/// provide event data for the <see cref="DockingManager.DockVisibilityChanged"/> event.
	/// </remarks>
	/// <seealso cref="DockVisibilityChangedEventHandler"/>
	/// <seealso cref="DockControlEventArgs"/>
	public class DockVisibilityChangedEventArgs : DockControlEventArgs
	{
        private DockingAction dockAction = DockingAction.Unknown;

        public DockingAction Action
        {
            get { return dockAction; }
        }
		/// <summary>
		/// Creates a new instance of the DockVisibilityChangedEventArgs class.
		/// </summary>
		/// <param name="ctrl">The control for which the DockVisibility state has changed.</param>
		public DockVisibilityChangedEventArgs(Control ctrl) : base(ctrl)
		{
            dockAction = DockingAction.Unknown;
		}
        /// <summary>
        /// Creates a new instance of the DockVisibilityChangedEventArgs class with specified arguments.
        /// </summary>
        /// <param name="ctrl">The control for which the DockVisibility state has changed.</param>
        /// <param name="dAction">The action which caused the event.</param>
        public DockVisibilityChangedEventArgs(Control ctrl, DockingAction dAction)
            : base(ctrl)
        {
            dockAction = dAction;
        }

	}

	/// <summary>
	/// Custom event argument class used for notifying users of DockVisibility state changing.
	/// </summary>
	/// <remarks>
	/// The DockVisibilityChangingEventArgs class is used by the <see cref="DockingManager"/> to
	/// provide event data for the <see cref="DockingManager.DockVisibilityChanging"/> event.
	/// </remarks>
	/// <seealso cref="DockVisibilityChangingEventHandler"/>
	/// <seealso cref="DockControlEventArgs"/>
	public class DockVisibilityChangingEventArgs : DockControlEventArgs
	{
		[Syncfusion.Documentation.DocumentationExclude()]
		private bool cancel = false;
        private DockingAction dockAction = DockingAction.Unknown;

        public DockingAction Action
        {
            get { return dockAction; }
        }
		
		/// <value>
		/// Indicates whether to close the selected docking window.
		/// </value>
		[
		DefaultValue(false)
		]
		public bool Cancel
		{
			get
			{
				return this.cancel;
			}
			set
			{
				if (this.cancel != value)
				{
					this.cancel = value;
				}
			}
		}
		/// <summary>
		/// Overloaded. Creates a new instance of the DockVisibilityChangedEventArgs class.
		/// </summary>
		/// <param name="ctrl">The control for which the DockVisibility state is changing.</param>
		public DockVisibilityChangingEventArgs(Control ctrl) : base(ctrl)
		{
			this.Cancel = false;
            this.dockAction = DockingAction.Unknown;
		}
		/// <summary>
		/// Creates a new instance of the DockVisibilityChangedEventArgs class 
		/// with specified <c>Cancel</c> property value.
		/// </summary>
		/// <param name="ctrl">The control for which the DockVisibility state is changing.</param>
		/// <param name="cncl">The value of Cancel property.</param>
		public DockVisibilityChangingEventArgs(Control ctrl, bool cncl) : base(ctrl)
		{
			this.Cancel = cncl;
            this.dockAction = DockingAction.Unknown;
		}
        /// <summary>
        /// Creates a new instance of the DockVisibilityChangedEventArgs class  
        /// with specified <c>Cancel</c> property value. 
        /// </summary> 
        /// <param name="ctrl">The control for which the DockVisibility state is changing.</param> 
        /// <param name="cncl">The value of Cancel property.</param> 
        public DockVisibilityChangingEventArgs(Control ctrl, bool cncl, DockingAction dAction)
            : base(ctrl)
        {
            this.Cancel = cncl;
            dockAction = dAction;
        }

	}

	/// <summary>
	/// Handles the <see cref="DockingManager.DockVisibilityChanged"/> event.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="arg">A <see cref="DockVisibilityChangedEventArgs"/> value that contains the event data.</param>
	public delegate void DockVisibilityChangedEventHandler(Object sender, DockVisibilityChangedEventArgs arg);

	/// <summary>
	/// Handles the <see cref="DockingManager.DockVisibilityChanging"/> event.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="arg">A <see cref="DockVisibilityChangingEventArgs"/> value that contains the event data.</param>
	public delegate void DockVisibilityChangingEventHandler(object sender, DockVisibilityChangingEventArgs arg);

    
	/// <summary>
	/// Custom event argument class used for notifying users of docking controls
	/// programmatically using context menu.
	/// </summary>
	/// <remarks>
	/// The DockMenuClickEventArgs class is used by the <see cref="DockingManager"/> to
	/// provide event data for the <see cref="DockingManager.DockMenuClick"/> event.
	/// </remarks>
	public class DockMenuClickEventArgs : DockControlEventArgs
	{
		[Syncfusion.Documentation.DocumentationExclude()]
		protected DockingStyle dockStyle = DockingStyle.Fill;
		/// <summary>
		/// Returns the docking style of the window.
		/// </summary>
		public DockingStyle DockingStyle
		{
			get { return dockStyle; }
		}
		
		/// <summary>
		/// Creates the instance for DockMenuClickEventArgs.
		/// </summary>
		public DockMenuClickEventArgs( Control dockControl, DockingStyle dockStyle )
			: base(dockControl)
		{
			this.dockStyle = dockStyle;
		}
	}

    /// <summary>
    /// Handles the <see cref="DockingManager.NewDockStateEndLoad"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="arg">A <see cref="DockStateLoadEventArgs"/> value that contains the event data.</param>
    public delegate void DockStateLoadEventHandler(object sender, DockStateLoadEventArgs arg);

    /// <summary>
    /// Custom event argument class used for notifying users of docking controls
    /// state is loaded successfully or not.
    /// </summary>
    /// <remarks>
    /// The DockStateLoadEventArgs class is used by the <see cref="DockingManager"/> to
    /// provide event data for the <see cref="DockingManager.NewDockStateEndLoad"/> event.
    /// </remarks>
    public class DockStateLoadEventArgs : EventArgs
    {
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool loadResultValue= false;

        /// <summary>
        /// Returns a bool value indicating whether the dock state is loaded successfully or not.
        /// </summary>
        public bool LoadResult
        {
            get { return loadResultValue; }
        }
        /// <summary>
        /// Creates a new instance of the <see cref="DockingManager.DockStateLoadEventArgs"/> class with the dock state loaded result.
        /// </summary>
        /// <param name="lResult">if set to <c>true</c> dock state is successfully loaded.</param>
            public DockStateLoadEventArgs(bool lResult)
            {
                this.loadResultValue=lResult;
            }
    }
	/// <summary>
	/// Handles the <see cref = DockingManager.DockMenuClick> event.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="arg">A <see cref="DockMenuClickEventArgs"/> value that contains the event data.</param>
	public delegate void DockMenuClickEventHandler( object sender, DockMenuClickEventArgs arg );

	/// <summary>
	/// Custom event argument class used for notifying users of activation state changes.
	/// </summary>
	/// <remarks>
	/// The DockActivationChangedEventArgs class is used by the <see cref="DockingManager"/> to
	/// provide event data for the <see cref="DockingManager.DockControlActivated"/> and
	/// <see cref="DockingManager.DockControlDeactivated"/> events.
	/// </remarks>
	/// <seealso cref="DockActivationChangedEventHandler"/>
	/// <seealso cref="DockControlEventArgs"/>
	public class DockActivationChangedEventArgs : DockControlEventArgs
	{
		/// <summary>
		/// Creates a new instance of the DockActivationChangedEventArgs class.
		/// </summary>
		/// <param name="ctrl">The control for which the activation state has changed.</param>
		public DockActivationChangedEventArgs(Control ctrl) : base(ctrl)
		{
		}
	}

	/// <summary>
	/// Handles the <see cref="DockingManager.DockControlActivated"/>
	/// and <see cref="DockingManager.DockControlDeactivated"/> events.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="arg">A <see cref="DockActivationChangedEventArgs"/> value that contains the event data.</param>
	public delegate void DockActivationChangedEventHandler(Object sender, DockActivationChangedEventArgs arg);


	/// <summary>
	/// Custom event argument class used for notifying users of the start/stop of an autohide animation.
	/// </summary>
	/// <remarks>
	/// The AutoHideAnimationEventArgs class is used by the <see cref="DockingManager"/> to
	/// provide event data for the <see cref="DockingManager.AutoHideAnimationStart"/> and
	/// <see cref="DockingManager.AutoHideAnimationStop"/> events.
	/// </remarks>
	/// <seealso cref="AutoHideAnimationEventHandler"/>
	/// <seealso cref="DockControlEventArgs"/>
	public class AutoHideAnimationEventArgs : DockControlEventArgs
	{
		[Syncfusion.Documentation.DocumentationExclude()]
		protected System.Windows.Forms.DockStyle dsBorder = DockStyle.None;
		
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Rectangle rcBounds = Rectangle.Empty;

        [Syncfusion.Documentation.DocumentationExclude()]
        protected AutoHideRollState state = AutoHideRollState.None;
		/// <summary>
		/// Returns the <see cref="DockingManager.HostForm"/> border along which the AutoHide tab is aligned.
		/// </summary>
		/// <value>A <see cref="System.Windows.Forms.DockStyle"/> value.</value>
		public DockStyle DockBorder
		{
			get { return this.dsBorder; }
		}

		/// <summary>
		/// Gets / sets the display bounds of the autohidden control.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Rectangle"/> value.</value>
		public Rectangle Bounds
		{
			get { return this.rcBounds; }
			set { this.rcBounds = value; }
		}

        /// <summary>
        /// Indicates the roll state of the autohidden control.
        /// </summary>
        public AutoHideRollState RollState
        {
            get { return this.state; }
            set { this.state = value; }
        }

        /// <summary>
        /// Creates a new instance of the AutoHideAnimationEventArgs class.
        /// </summary>
        /// <param name="ctrl">The control undergoing the autohide animation.</param>
        /// <param name="border">The dock style of the control.</param>
        /// <param name="bounds">The bounds of the control.</param>
		public AutoHideAnimationEventArgs(Control ctrl, DockStyle border, Rectangle bounds) : base(ctrl)
		{
			this.dsBorder = border;
			this.rcBounds = bounds;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoHideAnimationEventArgs"/> class.
        /// </summary>
        /// <param name="ctrl">The control undergoing the autohide animation.</param>
        /// <param name="border">The dock style of the control.</param>
        /// <param name="bounds">The bounds of the control.</param>
        /// <param name="state">The autohide roll state.</param>
        public AutoHideAnimationEventArgs(Control ctrl, DockStyle border, Rectangle bounds, AutoHideRollState state)
            : base(ctrl)
        {
            this.dsBorder = border;
            this.rcBounds = bounds;
            this.state = state;
        }
	}

    /// <summary>
    /// Indicates the roll state of the autohidden control.
    /// </summary>
    public enum AutoHideRollState
    {
        /// <summary>
        /// When the autohidden control is rolled in.
        /// </summary>
        RolledIn,

        /// <summary>
        /// When the autohidden control is rolled out.
        /// </summary>
        RolledOut,

        /// <summary>
        /// No state.
        /// </summary>
        None
    }

	/// <summary>
	/// Handles the <see cref="DockingManager.AutoHideAnimationStart"/>
	/// and <see cref="DockingManager.AutoHideAnimationStop"/> events.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="arg">An <see cref="AutoHideAnimationEventArgs"/> value that contains the event data.</param>
	public delegate void AutoHideAnimationEventHandler(Object sender, AutoHideAnimationEventArgs arg);


	/// <summary>
	/// Custom cancellable event argument class used for the <see cref="DockingManager.DragAllow"/> event.
	/// </summary>
	/// <seealso cref="DragAllowEventHandler"/>
	/// <remarks>The <see cref="DockingManager"/> uses the <see cref="DockingManager.DragAllow"/> event to provide information
	/// about an impending drag operation and provides a chance to accept/cancel the drag.
	/// </remarks>
	public class DragAllowEventArgs : CancelEventArgs
	{
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Control ctrlDrag = null;

		/// <summary>
		/// Returns the control that is about to be dragged.
		/// </summary>
		/// <value>A <see cref="System.Windows.Forms.Control"/> instance representing the control that is about to be dragged.</value>
		public Control Control
		{
			get { return this.ctrlDrag; }
		}

		/// <summary>
		/// Creates a new instance of the DockAllowEventArgs class.
		/// </summary>
		/// <param name="ctrldrag">The control being dragged.</param>
		public DragAllowEventArgs(Control ctrldrag)
		{
			this.ctrlDrag = ctrldrag;
		}
	}

	/// <summary>
	/// Handles the <see cref="DockingManager.DragAllow"/> event.
	/// </summary>
	/// <param name="sender"> The source of the event.</param>
	/// <param name="arg"> A <see cref="DragAllowEventArgs"/> value that provides the event data.</param>
	public delegate void DragAllowEventHandler(Object sender, DragAllowEventArgs arg);



	/// <summary>
	/// Custom cancellable event argument class used for the <see cref="DockingManager.DockAllow"/> event.
	/// </summary>
	/// <seealso cref="DockAllowEventHandler"/>
	/// <remarks>The <see cref="DockingManager"/> uses the <see cref="DockingManager.DockAllow"/> event to provide information
	/// about an impending dock operation and provides a chance to accept/cancel the
	/// dock based on the participating controls and the style.</remarks>
	public class DockAllowEventArgs : CancelEventArgs
	{
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Control ctrlDrag = null;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected Control ctrlTarget = null;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected Syncfusion.Windows.Forms.Tools.DockingStyle dStyle = Syncfusion.Windows.Forms.Tools.DockingStyle.Fill;

		/// <summary>
		/// Returns the control being dragged.
		/// </summary>
		/// <value>A Control object that represents the control being dragged.</value>
		public Control DragControl
		{
			get { return this.ctrlDrag; }
		}

		/// <summary>
		/// Returns the dock target control.
		/// </summary>
		/// <value>A Control object that represents the target control for the dock.</value>
		public Control TargetControl
		{
			get { return this.ctrlTarget; }
		}

		/// <summary>
		/// Returns the dock operation style.
		/// </summary>
		/// <value>A <see cref="Syncfusion.Windows.Forms.Tools.DockingStyle"/> value that represents the style of the dock
		/// operation.</value>
		public Syncfusion.Windows.Forms.Tools.DockingStyle DockStyle
		{
			get { return this.dStyle; }
		}

		/// <summary>
		/// Creates a new instance of the DockAllowEventArgs class.
		/// </summary>
		/// <param name="ctrldrag">The control being dragged.</param>
		/// <param name="ctrltarget">The target control.</param>
		/// <param name="style">The dock style.</param>
		public DockAllowEventArgs(Control ctrldrag, Control ctrltarget, Syncfusion.Windows.Forms.Tools.DockingStyle style)
		{
			this.ctrlDrag = ctrldrag;
			this.ctrlTarget = ctrltarget;
			this.dStyle = style;
		}
	}

	/// <summary>
	/// Handles the <see cref="DockingManager.DockAllow"/> event.
	/// </summary>
	/// <param name="sender"> The source of the event.</param>
	/// <param name="arg"> A <see cref="DockAllowEventArgs"/> value that provides the event data.</param>
	public delegate void DockAllowEventHandler(Object sender, DockAllowEventArgs arg);
	/// <summary>
	/// Handles the <see cref="DockingManager.ControlMaximizing"/> event.
	/// </summary>
	/// <param name="sender"> The source of the event.</param>
	/// <param name="args"> A <see cref="ControlMaximizeEventArgs"/> value that provides the event data.</param>
	public delegate void ControlMaximizeEventHandler(Object sender, ControlMaximizeEventArgs args);
	/// <summary>
	/// Handles the <see cref="DockingManager.ControlMaximized"/> event.
	/// </summary>
	/// <param name="sender"> The source of the event.</param>
	/// <param name="args"> A <see cref="ControlMaximizeEventArgs"/> value that provides the event data.</param>
	public delegate void ControlMaximizedEventHandler( Object sender, ControlMaximizedEventArgs args );
	/// <summary>
	/// Handles the <see cref="DockingManager.ControlMinimized"/> event.
	/// </summary>
	/// <param name="sender"> The source of the event.</param>
	/// <param name="args"> A <see cref="ControlMinimizedEventArgs"/> value that provides the event data.</param>
	public delegate void ControlMinimizeEventHandler(Object sender, ControlMinimizedEventArgs args);
	/// <summary>
	/// Handles the <see cref="DockingManager.ControlRestored"/> event.
	/// </summary>
	/// <param name="sender"> The source of the event.</param>
	/// <param name="args"> A <see cref="ControlRestoredEventArgs"/> value that provides the event data.</param>
	public delegate void ControlRestoreEventHandler(Object sender, ControlRestoredEventArgs args);
	
	/// <summary>
	/// Base size changed event args that informs user which controls size state changed.
	/// </summary>
	public class ControlSizeStateChangedEventArgs : EventArgs
	{
		protected Control m_control = null;

		/// <summary>
		/// returns changing control.
		/// </summary>
		public Control Control
		{
			get
			{
				return m_control;
			}
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="control">Changing control.</param>
		public ControlSizeStateChangedEventArgs( Control control )
		{
			m_control = control;
		}
	}

	/// <summary>
	/// Event args for size restore event.
	/// </summary>
	public class ControlRestoredEventArgs : ControlSizeStateChangedEventArgs
	{
		protected ControlSizeStates m_prevSizeState;
		/// <summary>
		/// returns previous size state of changing control.
		/// </summary>
		public ControlSizeStates PreviousSizeState
		{
			get
			{
				return m_prevSizeState;
			}
		}
		/// <summary>
		/// Overriden.
		/// </summary>
		/// <param name="control"></param>
		/// <param name="prevState">Previous size state of control.</param>
		public ControlRestoredEventArgs( Control control, ControlSizeStates prevState )
			: base( control )
		{
			m_prevSizeState = prevState;
		}
	}
	/// <summary>
	/// Arguments for control minimize event.
	/// </summary>
	/// <remarks>Contains only changing control.</remarks>
	public class ControlMinimizedEventArgs : ControlSizeStateChangedEventArgs
	{
		/// <summary>
		/// Overriden.
		/// </summary>
		/// <param name="control"></param>
		public ControlMinimizedEventArgs( Control control )
			: base(control)
		{ }
	}
	/// <summary>
	/// Arguments for control maximized event.
	/// </summary>
	/// <remarks>Contains only changing control.</remarks>
	public class ControlMaximizedEventArgs : ControlSizeStateChangedEventArgs
	{ 
		/// <summary>
		/// Overriden.
		/// </summary>
		/// <param name="control"></param>
		public ControlMaximizedEventArgs( Control control )
			: base(control)
		{ }
	}
	

	/// <summary>
	/// Arguments for Maximize control event.
	/// </summary>
	/// <remarks>
	/// Adds ability to cancel operation.
	/// </remarks>
	public class ControlMaximizeEventArgs : ControlSizeStateChangedEventArgs
	{	
		protected bool m_bCancel = false;
		/// <summary>
		/// Gets/sets if to cancel operation.
		/// </summary>
		public bool Cancel
		{
			get
			{
				return m_bCancel;
			}
			set
			{
				if( m_bCancel != value )
				{
					m_bCancel = value;
				}
			}
		}
		/// <summary>
		/// Overriden.
		/// </summary>
		/// <param name="control"></param>
		public ControlMaximizeEventArgs( Control control )
			: base(control)
		{ }
	}
	
	/// <summary>
	/// Custom event argument class used by the <see cref="DockingManager.DockContextMenu"/> event.
	/// </summary>
	/// <remarks>The <see cref="DockingManager"/> uses the <see cref="DockingManager.DockContextMenu"/> event to
	/// allow users to modify the context menu displayed when a docking window's caption
	/// is right-clicked.
	/// <seealso cref="DockContextMenuEventHandler"/>
	/// </remarks>
	public class DockContextMenuEventArgs : EventArgs
	{
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Control ctrlOwner = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected PopupMenu mnuContext = null;

		/// <summary>
		/// Returns the control that is displaying the context menu.
		/// </summary>
		public Control Owner
		{
			get { return this.ctrlOwner; }
		}

		/// <summary>
		/// Gets / sets the context menu to be displayed.
		/// </summary>
		/// <value>An instance of the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.PopupMenu"/> class.</value>
		public PopupMenu ContextMenu
		{
			get { return this.mnuContext; }
			set	{ this.mnuContext = value; }
		}

		/// <summary>
		/// Creates a new instance of the DockContextMenuEventArgs class.
		/// </summary>
		/// <param name="owner">The control displaying the menu.</param>
		/// <param name="menu">The menu being displayed.</param>
		public DockContextMenuEventArgs(Control owner, PopupMenu menu)
		{
			this.ctrlOwner = owner;
			this.mnuContext = menu;
		}
	}

	/// <summary>
	/// Handles the <see cref="DockingManager.DockContextMenu"/> event.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="arg">A <see cref="DockContextMenuEventArgs"/> value that provides the event data.</param>
	public delegate void DockContextMenuEventHandler(Object sender, DockContextMenuEventArgs arg);


	/// <summary>
	/// Custom event argument class used for obtaining the Graphics objects used for rendering
	/// the caption of docked windows.
	/// </summary>
	/// <remarks>The <see cref="DockingManager"/> uses the <see cref="DockingManager.ProvideGraphicsItems"/> event to
	/// obtain custom Graphics objects from the application to be used for drawing the caption area of docked windows.
	/// <p>
	/// NOTE: The main caption for floating windows is rendered by the Win32 system and cannot be customized.
	/// </p>
	/// <seealso cref="ProvideGraphicsItemsEventHandler"/>
	/// </remarks>
	public class ProvideGraphicsItemsEventArgs : EventArgs
	{
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Control ctrlDocked = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Rectangle rcBounds = Rectangle.Empty;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bActiveCaption = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Brush brBackground = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Color clrForeground = Color.Empty;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Font ftCaption = null;
	  
		/// <summary>
		/// Returns the dockable control for which the caption is being drawn.
		/// </summary>
		/// <value>A <see cref="System.Windows.Forms.Control"/> instance.</value>
		public Control Control
		{
			get { return this.ctrlDocked; }
		}

		/// <summary>
		/// Returns the bounds of the caption.
		/// </summary>
		/// <remarks>A <see cref="System.Drawing.Rectangle"/> value.</remarks>
		public Rectangle CaptionBounds
		{
			get { return this.rcBounds; }
		}

		/// <summary>
		/// Returns the active/inactive state of the docking window.
		/// </summary>
		/// <value>TRUE if the window is active.</value>
		public bool IsActiveCaption
		{
			get { return this.bActiveCaption; }
		}

		/// <summary>
		/// Gets / sets the Brush to be used for drawing the caption background.
		/// </summary>
		/// <value>An instance of the <see cref="System.Drawing.Brush"/> class.</value>
		public Brush CaptionBackground
		{
			get { return this.brBackground; }
			set	{ this.brBackground = value; }
		}

		/// <summary>
		/// Gets / sets the Color to be used for drawing the caption text and buttons.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Color"/> value.</value>
		public Color CaptionForeground
		{
			get { return this.clrForeground; }
			set { this.clrForeground = value; }
		}

		/// <summary>
		/// Gets / sets the Font to be used for the caption text.
		/// </summary>
		/// <value>An instance of the <see cref="System.Drawing.Font"/> class.</value>
		public Font CaptionFont
		{
			get { return this.ftCaption; }
			set { this.ftCaption = value; }
		}

		/// <summary>
		/// Creates a new instance of the <see cref="ProvideGraphicsItemsEventArgs"/> class.
		/// </summary>
		/// <param name="ctrl">The dockable control for which the caption is being drawn.</param>
		/// <param name="bounds">The caption bounds.</param>
		/// <param name="active">Indicates the active/inactive state of the control.</param>
		public ProvideGraphicsItemsEventArgs(Control ctrl, Rectangle bounds, bool active)
		{
			this.ctrlDocked = ctrl;
			this.rcBounds = bounds;
			this.bActiveCaption = active;
		}
	}

	/// <summary>
	/// Handles the <see cref="DockingManager.ProvideGraphicsItems"/> event.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="arg">A <see cref="ProvideGraphicsItemsEventArgs"/> value that contains the event data.</param>
	public delegate void ProvideGraphicsItemsEventHandler(Object sender, ProvideGraphicsItemsEventArgs arg);

	/// <summary>
	/// Custom event argument class used for notifying users that dock state information does not exist for a control.
	/// </summary>
	/// <remarks>
	/// The DockStateUnavailableEventArgs class is used by the <see cref="DockingManager"/> to
	/// provide event data for the <see cref="DockingManager.DockStateUnavailable"/> event.
	/// </remarks>
	/// <seealso cref="DockControlEventArgs"/>
	public class DockStateUnavailableEventArgs : DockControlEventArgs
	{
		/// <summary>
		/// Creates a new instance of the DockStateUnavailableEventArgs class.
		/// </summary>
		/// <param name="ctrl">The control for which the dock state information is not available.</param>
		public DockStateUnavailableEventArgs(Control ctrl) : base(ctrl)
		{
		}
	}

	/// <summary>
	/// Handles the <see cref="DockingManager.DockStateUnavailable"/> event.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="arg">A <see cref="DockStateUnavailableEventArgs"/> value that contains the event data.</param>
	public delegate void DockStateUnavailableEventHandler(Object sender, DockStateUnavailableEventArgs arg);


	/// <summary>
	/// Custom event argument class used by the <see cref="DockingManager"/> to notify that a previously persisted
	/// dockable control cannot be located during a <see cref="DockingManager.LoadDockState"/> operation.
	/// </summary>
	/// <remarks>
	/// Applications can use the <see cref="InitializeControlOnLoadEventArgs"/> as a hint to create and initialize
	/// controls selectively based on the control set in the previously persisted docking layout.
	/// </remarks>
	public class InitializeControlOnLoadEventArgs : EventArgs
	{
		[Syncfusion.Documentation.DocumentationExclude()]
		private String ctrlName = String.Empty;

		/// <summary>
		/// Returns the <see cref="Control.Name"/> property of the control.
		/// </summary>
		public String ControlName
		{
			get { return this.ctrlName; }
		}

		/// <summary>
		/// Creates a new instance of the InitializeControlOnLoadEventArgs class.
		/// </summary>
		/// <param name="ctrlname">The name of the control that the DockingManager is attempting to load.</param>
		public InitializeControlOnLoadEventArgs(String ctrlname)
		{
			this.ctrlName = ctrlname;
		}
	}

	/// <summary>
	/// Handles the <see cref="DockingManager.InitializeControlOnLoad"/> event.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="arg">A <see cref="InitializeControlOnLoadEventArgs"/> value that contains the event data.</param>
	public delegate void InitializeControlOnLoadEventHandler(Object sender, InitializeControlOnLoadEventArgs arg);


	/// <summary>
	/// Custom event argument class used for notifying users that a control is being transferred from
	/// it's current <see cref="DockingManager"/> to another manager.
	/// </summary>
	/// <remarks>
	/// The TransferManagerEventArgs class is used by the <see cref="DockingManager"/> to
	/// provide event data for the <see cref="DockingManager.TransferringFromManager"/> and <see cref="TransferredToManager"/>
	/// events.
	/// </remarks>
	/// <seealso cref="DockControlEventArgs"/>
	public class TransferManagerEventArgs : DockControlEventArgs
	{
		/// <summary>
		/// Creates a new instance of the TransferManagerEventArgs class.
		/// </summary>
		/// <param name="ctrl">The control whose <see cref="DockingManager"/> is undergoing the transfer.</param>
		public TransferManagerEventArgs(Control ctrl) : base(ctrl)
		{
		}
	}

	/// <summary>
	/// Handles the <see cref="DockingManager.TransferringFromManager"/>
	/// and <see cref="DockingManager.TransferredToManager"/> events.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="arg">A <see cref="TransferManagerEventArgs"/> value that contains the event data.</param>
	public delegate void TransferManagerEventHandler(Object sender, TransferManagerEventArgs arg);
    /// <summary>
    /// Custom event argument class used by the <see cref="DockingManager.AutoHideTabContextMenu"/> event.
    /// </summary>
    /// <remarks>The <see cref="DockingManager"/> uses the <see cref="DockingManager.AutoHideTabContextMenu"/> event to
    /// allow users to modify the context menu displayed when a AutoHideTab is right-clicked.
    /// <seealso cref="AutoHideTabContextMenuEventHandler"/>
    /// </remarks>
    public class AutoHideTabContextMenuEventArgs : EventArgs
    {
        [Syncfusion.Documentation.DocumentationExclude()]
        protected PopupMenu mnuContext = null;

        [Syncfusion.Documentation.DocumentationExclude()]
        protected Syncfusion.Windows.Forms.Tools.DockingStyle dStyle;
        /// <summary>
        /// Gets / sets the context menu to be displayed.
        /// </summary>
        /// <value>An instance of the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.PopupMenu"/> class.</value>
        public PopupMenu ContextMenu
        {
            get { return this.mnuContext; }
            set { this.mnuContext = value; }
        }
        /// <summary>
        /// Returns the side to where the AutoHideTab aligned.
        /// </summary>
        public DockingStyle Side
        {
            get { return dStyle; }
        }
        /// <summary>
        /// Creates a new instance of the AutoHideTabContextMenuEventArgs class.
        /// </summary>
        /// <param name="menu">The menu being displayed.</param>
        /// <param name="edge">The docked edge of AutoHidetab control displaying the menu.</param>
        public AutoHideTabContextMenuEventArgs(PopupMenu menu, DockingStyle edge)
        {
            this.mnuContext = menu;
            this.dStyle = edge;
        }
    }
    /// <summary>
    /// Handles the <see cref="DockingManager.AutoHideTabContextMenu"/> event.
    /// </summary>
	/// <param name="sender">The source of the event.</param>
    /// <param name="arg">A <see cref="AutoHideTabContextMenuEventArgs"/> value that contains the event data.</param>
    public delegate void AutoHideTabContextMenuEventHandler(object sender, AutoHideTabContextMenuEventArgs arg);
# endregion

# region Enum declarations
    /// <summary>
    /// Specifies the close action of docked controls. 
    /// </summary> 
    public enum DockingAction
    {
        /// <summary> 
        /// Specifies that the close action is by mouse. 
        /// </summary> 
        ByMouse,
        /// <summary> 
        /// Specifies that the close action is by code. 
        /// </summary> 
        Unknown
    }

    /// <summary>
    /// Specifies the docking behavior of docked controls. 
    /// </summary> 
    public enum DockBehavior
    {
        /// <summary> 
        /// Specifies that the Docking behavior based on Visual Studio 2008. 
        /// </summary> 
        VS2008,
        /// <summary> 
        /// Specifies that the Docking behavior based on Visual Studio 2010. 
        /// </summary> 
        VS2010
    }

    /// <summary>
    /// Specifies the style of dragging.
    /// </summary>
    /// <remarks>
    /// The DragProviderStyle enumeration is used by the <see cref="DockingManager"/> to enable the style of dragging
    /// the docking windows. VS2005 style is set for Visual Studio 2005 by default. Standard style will set for VS 2002 and VS 2003 .NET Framework.
    /// 
    /// </remarks>
    public enum DragProviderStyle
    {
        /// <summary>
        /// Enables the normal style of helper frame for docking windows.
        /// </summary>
        Standard = 0,
        /// <summary>
        /// Enables the Visual Studio 2005 style of docking.
        /// </summary>
        VS2005,
        /// <summary>
        /// Enables the Visual Studio 2005 Beta 2 style of docking.
        /// </summary>
        Whidbey,
		/// <summary>
		/// Enables visual studio 2008 style for the drag provider.
		/// </summary>
		VS2008,
        /// <summary>
		/// Enables visual studio 2010 style for the drag provider.
		/// </summary>
		VS2010,
		/// <summary>
		/// Enables visual studio 2012 style for the drag provider.
		/// </summary>
		VS2012
    }

	/// <summary>
	/// Specifies appearance and behavior of docking windows context menus.
	/// </summary>
	public enum DockMenuStyle
	{
		/// <summary>
		/// Appearance and behavior are similar to menus in Visual Studio 2003.
		/// </summary>
		VS2003,
		/// <summary>
		/// Appearance and behavior are similar to menus in Visual Studio 2005.
		/// </summary>
		VS2005
	}

    [Syncfusion.Documentation.DocumentationExclude()]
    public enum PainterType
    {
        Halftone, Colored, Custom
    };
    /// <summary>
	/// Specifies the Alignment style of the dock tab.
	/// </summary>
    /// 

	public enum DockTabAlignmentStyle 
	{
		/// <summary>
		/// The Tab is aligned to the Top.
		/// </summary>
		Top,
		/// <summary>
		/// The Tab is aligned to the Bottom.
		/// </summary>
		Bottom,
		/// <summary>
		/// The Tab is aligned to the Left.
		/// </summary>
		Left,
		/// <summary>
		/// The Tab is aligned to the Right.
		/// </summary>
		Right
	}
    
    /// <summary>
    /// Specifies the Selection style of the AutoHided window.
    /// </summary>
    public enum AutoHideSelectionStyle
    {
        /// <summary>
        /// The AutoHided window can be show/hide by MouseHover.
        /// </summary>
        MouseHover,
        /// <summary>
        /// The AuotHided window can be show/hide by Click.
        /// </summary>
        Click
    }

    /// <summary>
    /// Specifies the Alignment style of the dock label.
    /// </summary>
	public enum DockLabelAlignmentStyle
	{
		/// <summary>
		/// The Caption text is aligned to the Left by default.
		/// </summary>
		Default,
		/// <summary>
		/// The Caption text is aligned to the Left.
		/// </summary>
		Left,
		/// <summary>
		/// The Caption text is aligned to the Center.
		/// </summary>
		Center,
		/// <summary>
		/// The Caption text is aligned to the Right.
		/// </summary>
		Right
    }
    # endregion
    /// <summary>
	/// The DockingManager provides the functionality for creating and working with docking windows.
	/// </summary>
	/// <remarks>
	/// <p>
	/// The Essential Tools Docking Windows framework enables developers to add docking windows, similar
	/// to those found in the Microsoft Visual Studio.NET IDE, to their Windows Forms applications. At the
	/// most basic level a docking window may be defined as a control that attaches itself to a host form's
	/// border, is capable of being dragged around and docked to different edges within the form and can also be
	/// dragged off the host form and floated as an individual top-level window. The docking
	/// framework allows just about any child control on a form to be made into a fully qualified
	/// docking window. The framework, in addition to the core docking interactions, implements
	/// some highly advanced features such as multiple docking levels, nested docking, tabbed docking,
	/// tear-off tabs, autohide mode, state persistence etc. To facilitate the addition of these
	/// complex features, the DockingManager has a full-fledged WYSIWYG visual designer that enables
	/// developers to create the exact docking layout that they desire without having to write a single line of code.
	/// </p>
	/// <p>The DockingManager class is the central component of the Essential Tools Docking Windows implementation.
	/// The class coordinates and facilitates the multitude of complex interactions that take place between
	/// a dockable control and it's host form as well as between the dockable controls themselves.
	/// DockingManagers are form-centric and adding an instance of the component to a form makes the
	/// form into a 'dock-enabled' host. The DockingManager is implemented as an Extender Provider and
	/// upon adding it to a Form or UserControl, the controls that are immediate children of the container qualify for
	/// the docking services provided by the docking framework.
	/// </p>
	/// <p>
	/// The 'EnableDocking' (<see cref="DockingManager.SetEnableDocking"/>) extended property that the
	/// DockingManager adds to controls serves as the trigger for enabling/disabling a control as a dockable window.
	/// Upon setting the EnableDocking property, the control is enclosed within a dockable container and will be
	/// docked to a default border. The control can now be repositioned by dragging it around within the designer.
	/// The DockingManager persists the dock positions set during design time, ie., the dock state
	/// information, as a part of the application's resource and uses this persisted info when loading
	/// the application. Thus the DockingManager implements a true WYSIWYG visual designer. There is also
	/// a simple and intuitive API available for programmatic manipulation of the docking windows.
	/// </p>
	/// </remarks>
	/// <example>
	/// The sample code shows how to create and setup a simple docking windows layout constituting
	/// of a ListBox control docked to the left side of the form and having a width of 175 units, a second
	/// ListBox that is docked as a tab within the first ListBox, a TreeView control that is docked to the form's
	/// right border, has a width of 150 units and starts off in the AutoHide mode, and a
	/// CheckedListBox control that is initially a floating window.
	/// <p>
	/// NOTE: The layout initialization code shown here is required only when docking window is being used programmatically.
	/// When using the designer, the layout state will automatically be written to the application's resource file.
	/// </p>
	/// <coderef file="Tools\Samples\Docking Package\DockingWindows\SimpleCode\cs\Form1.cs" name="DockingWindows" lang="C#"><code lang="C#">
	///		private void InitializeDockingWindows()
	///		{
	///			// Create the DockingManager instance and set this form as the host form.
	///			this.dockingManager = new Syncfusion.Windows.Forms.Tools.DockingManager(this.components);
	///			this.dockingManager.BeginInit();
	///			this.dockingManager.HostForm = this;
	///
	///			// Disable state persistence
	///			this.dockingManager.PersistState = false;
	///			// Enable display of the default context menus
	///			this.dockingManager.EnableContextMenu = true;
	///			// Set the imagelist that will provide the icons for the docking windows.
	///			this.dockingManager.ImageList = this.ilDocking;
	///
	///			// Dock listbox1 to the left border of the form and with an initial
	///			// width of 175 units.
	///			// NOTE - Calling DockControl() on a control for the first time,
	///			// will initialize it as a docking window. This is the equivalent of
	///			// the DockingManager.SetEnableDocking() call.
	///			this.dockingManager.DockControl(this.listBox1, this,
	///				Syncfusion.Windows.Forms.Tools.Syncfusion.Windows.Forms.Tools.DockingStyle.Left, 175);
	///			// Set the text to be displayed in the dockingwindow caption
	///			this.dockingManager.SetDockLabel(this.listBox1, "ListBox 1");
	///			// The image index used for this control
	///			this.dockingManager.SetDockIcon(this.listBox1, 0);
	///
	///			// Now dock listbox2 as a tab onto listbox1
	///			this.dockingManager.DockControl(this.listBox2, this.listBox1,
	///				Syncfusion.Windows.Forms.Tools.Syncfusion.Windows.Forms.Tools.DockingStyle.Tabbed, 175);
	///			this.dockingManager.SetDockLabel(this.listBox2, "ListBox 2");
	///			this.dockingManager.SetDockIcon(this.listBox2, 1);
	///
	///			// Dock the treeView to the right border of the form with a width of 150.
	///			this.dockingManager.DockControl(this.treeView1, this, Syncfusion.Windows.Forms.Tools.DockingStyle.Right, 150);
	///			// Set treeView1 to start off in the AutoHide position.
	///			this.dockingManager.SetAutoHideMode(this.treeView1, true);
	///			this.dockingManager.SetDockLabel(this.treeView1, "TreeView");
	///			this.dockingManager.SetDockIcon(this.treeView1, 2);
	///
	///			// Set checkedListBox1 to be initially in a floating position.
	///			Rectangle rcfrm = this.Bounds;
	///			this.dockingManager.FloatControl(this.checkedListBox1,
	///				new Rectangle(rcfrm.Right+25,rcfrm.Bottom-250,175,300));
	///			this.dockingManager.SetDockLabel(this.checkedListBox1, "Checked ListBox");
	///			this.dockingManager.SetDockIcon(this.checkedListBox1, 3);
	///			this.dockingManager.EndInit();
	///		}</code></coderef>
	///
	///
	/// <coderef file="Tools\Samples\Docking Package\DockingWindows\SimpleCode\VB\Form1.vb" name="DockingWindows" lang="VB"><code lang="VB">
	///        Private Sub InitializeDockingWindows()
	///
	///            ' Create the DockingManager instance and set this form as the host form.
	///            Me.dockingManager = New Syncfusion.Windows.Forms.Tools.DockingManager(Me.components)
	///            Me.dockingManager.BeginInit()
	///            Me.dockingManager.HostForm = Me
	///
	///            ' Disable state persistence
	///            Me.dockingManager.PersistState = False
	///            ' Enable display of the default context menus
	///            Me.dockingManager.EnableContextMenu = True
	///            ' Set the imagelist that will provide the icons for the docking windows.
	///            Me.dockingManager.ImageList = Me.ilDocking
	///
	///            ' Dock listbox1 to the left border of the form and with an initial
	///            ' width of 175 units.
	///            ' NOTE - Calling DockControl() on a control for the first time,
	///            ' will initialize it as a docking window. This is the equivalent of
	///            ' the DockingManager.SetEnableDocking() call.
	///            Me.dockingManager.DockControl(Me.listBox1, Me, Syncfusion.Windows.Forms.Tools.Syncfusion.Windows.Forms.Tools.DockingStyle.Left, 175)
	///            ' Set the text to be displayed in the dockingwindow caption
	///            Me.dockingManager.SetDockLabel(Me.listBox1, "ListBox 1")
	///            ' The image index used for this control
	///            Me.dockingManager.SetDockIcon(Me.listBox1, 0)
	///
	///            ' Now dock listbox2 as a tab onto listbox1
	///            Me.dockingManager.DockControl(Me.listBox2, Me.listBox1, Syncfusion.Windows.Forms.Tools.Syncfusion.Windows.Forms.Tools.DockingStyle.Tabbed, 175)
	///            Me.dockingManager.SetDockLabel(Me.listBox2, "ListBox 2")
	///            Me.dockingManager.SetDockIcon(Me.listBox2, 1)
	///
	///            ' Dock the treeView to the right border of the form with a width of 150.
	///            Me.dockingManager.DockControl(Me.treeView1, Me, Syncfusion.Windows.Forms.Tools.DockingStyle.Right, 150)
	///            ' Set treeView1 to start off in the AutoHide position.
	///            Me.dockingManager.SetAutoHideMode(Me.treeView1, True)
	///            Me.dockingManager.SetDockLabel(Me.treeView1, "TreeView")
	///            Me.dockingManager.SetDockIcon(Me.treeView1, 2)
	///
	///            ' Set checkedListBox1 to be initially in a floating position.
	///            Dim rcfrm As Rectangle
	///            rcfrm = Me.Bounds
	///            Me.dockingManager.FloatControl(Me.checkedListBox1, New Rectangle((rcfrm.Right + 25), (rcfrm.Bottom - 250), 175, 300))
	///            Me.dockingManager.SetDockLabel(Me.checkedListBox1, "Checked ListBox")
	///            Me.dockingManager.SetDockIcon(Me.checkedListBox1, 3)
	///            Me.dockingManager.EndInit()
	///
	///        End Sub</code></coderef>
	///
	/// </example>
	/// <seealso cref="DockingClientPanel"/>
	[
	ToolboxBitmap(typeof(DockingManager), "ToolboxIcons.docking.bmp"),
	Designer( typeof(Syncfusion.Windows.Forms.Tools.Design.DockingManagerDesigner),
		typeof(System.ComponentModel.Design.IDesigner) ),
	DesignerSerializer( typeof(Syncfusion.Windows.Forms.Tools.Design.DockingManagerCDS),
		typeof(CodeDomSerializer)),
	ProvideProperty("EnableDocking", typeof(Control)),
	ToolboxItemFilter("System.Windows.Forms", ToolboxItemFilterType.Allow),
	Description("Provides the functionality for creating and working with docking windows.")
	]
	public class DockingManager : Component, IExtenderProvider, ISupportInitialize, IDockingManagerDesignerInvoke, IGetMsgProcListener, ICallWndProcListener,IVisualStyle 
	{
#region Initialization

        //static Field

        private static ArrayList DockingManagersList = new ArrayList();

        // Static Constructor
		static DockingManager()
        {
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
				new Syncfusion.Core.Licensing.LicensedComponent(typeof(DockingManager));
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			}
#if SINGLE_DLL_BUILD
			AppStateSerializer.SetBindingInfo("Syncfusion.Tools.Windows", typeof(DockingManager).Assembly);
			AppStateSerializer.SetBindingInfo("Syncfusion.Tools", typeof(DockingManager).Assembly);
#else
			AppStateSerializer.SetBindingInfo("Syncfusion.Tools.Frameworks", typeof(DockingManager).Assembly);
			// TO support backward compatibility
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Tools.Windows", typeof(DockingMgrSerializationWrapper).FullName, typeof(DockingMgrSerializationWrapper).Assembly);
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Tools.Windows", typeof(DHCSerializationWrapper).FullName, typeof(DHCSerializationWrapper).Assembly);
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Tools.Windows", typeof(DockInfo).FullName, typeof(DockInfo).Assembly);
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Tools.Windows", typeof(DockingStyle).FullName, typeof(DockingStyle).Assembly);
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Tools.Windows", typeof(DockPreference).FullName, typeof(DockPreference).Assembly);
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Tools.Windows", typeof(DCRelationship).FullName, typeof(DCRelationship).Assembly);
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Tools.Windows", typeof(DockingStyle).FullName, typeof(DockingStyle).Assembly);
#endif

        }

        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            if (DockingManagersList != null)
            {
                DockingManagersList.Clear();
                DockingManagersList = null;
            }
        }
        /// <summary>
        /// Overloaded. Creates a new instance of the <see cref="DockingManager"/> class.
        /// </summary>
        public DockingManager()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(DockingManager));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            this.InitializeDockingManager();
        }
        /// <summary>
        /// Creates a new instance of the <see cref="DockingManager"/> and initializes it with the container.
        /// </summary>
        /// <param name="container">An object implementing the <see cref="System.ComponentModel.IContainer"/> interface to associate with this instance of the DockingManager.</param>
        public DockingManager(IContainer container)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(DockingManager));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            container.Add(this);
            this.InitializeDockingManager();

            m_container = container;
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void InitializeDockingManager()
        {
            if (DockingManagersList == null)
                DockingManagersList = new ArrayList();
            DockingManagersList.Add(this);
            FramePainter = new FramePainter();
            this.bDesignProcess = IsDesignMode();
            InitializeDragProvider(providerStyle);
            autoHideButtonToolTip = SR.GetString(SR.DockPanelAutoHideButtonToolTip,this);
            closeButtonToolTip = SR.GetString(SR.DockPanelCloseButtonToolTip, this);
            menuButtonToolTip = SR.GetString(SR.DockPanelMenuButtonToolTip, this);
			maximizeButtonToolTip = SR.GetString(SR.DockPanelMaximizeButtonToolTip, this);
			restoreButtonToolTip = SR.GetString(SR.DockPanelRestoreButtonToolTip, this);
            ControllerSizeCalculator.SplitterWidth = splitterWidth;
            m_Renderer = new DockingManagerRenderer();

            // Initialize DockHost's pen for paiting borders.
            if (DockHost.pen == null || DockHost.pen.Color != this.BorderColor)
            {
                DockHost.pen = new Pen( this.BorderColor );
            }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
            m_Renderer.VisualStyle = VisualStyle.VS2005;
#else
            m_Renderer.VisualStyle = VisualStyle.Default;
#endif
            m_CaptionTextFont = new Font("Tahoma", 8.25f, FontStyle.Bold);
        }

		protected internal virtual void InitializeCaptionButtons()
		{
			if( m_CaptionButtons == null ||
                m_CaptionButtons != null && m_CaptionButtons.Count == 0)
			{
                LoadCaptionButtionsClearedState();
                if(!this.m_IsCaptionButtonsCleared)
				    m_CaptionButtons = GetDefaultCaptionButtons();

                this.m_IsCaptionButtonsCleared = false;
			}
			CaptionButtons.CollectionChanged += new EventHandler(CaptionButtons_CollectionChanged);
			CaptionButtons.CollectionItemChanged += new EventHandler(CaptionButtons_CollectionItemChanged);
			foreach (Control ctrl in alEnableDocking)
			{
				if (null == GetCustomCaptionButtons(ctrl))
					SetCustomCaptionButtons(ctrl, new CaptionButtonsCollection());
			}
			SynchronizeCaptionButtons();
		}

		protected internal CaptionButtonsCollection GetDefaultCaptionButtons()
		{
			CaptionButtonsCollection buttons = new CaptionButtonsCollection();

			CaptionButton cbClose = new CaptionButton( CaptionButtonType.Close );
			cbClose.Name = "CloseButton";
			buttons.Add( cbClose );

			CaptionButton cbPin = new CaptionButton( CaptionButtonType.Pin );
			cbPin.Name = "PinButton";
			buttons.Add( cbPin );

			CaptionButton cbMaximize = new CaptionButton( CaptionButtonType.Maximize );
			cbMaximize.Name = "MaximizeButton";
			buttons.Add( cbMaximize );

			CaptionButton cbRestore = new CaptionButton(CaptionButtonType.Restore);
			cbRestore.Name = "RestoreButton";
			buttons.Add(cbRestore);

            CaptionButton cbMenu = new CaptionButton(CaptionButtonType.Menu);
            cbMenu.Name = "MenuButton";
            buttons.Add(cbMenu);

			return buttons;
		}
        #endregion
# region Events declaration
        /// <summary>
		/// Occurs when the <see cref="DockingManager.ImageList"/> property changes.
		/// </summary>
		/// <remarks>
		///	The ImageListChanged event occurs when a new imagelist is assigned to the DockingManager.
		/// </remarks>
		[
		Description("Occurs when the ImageList is changed."),
		Category("Property Changed")
		]
		public event EventHandler ImageListChanged;

		/// <summary>
		/// Occurs when a docking window is about to be dragged.
		/// </summary>
		/// <remarks> The DragAllow event is used by the <see cref="DockingManager"/> to provide
		/// information about an upcoming drag operation. The drag can be cancelled by
		/// setting the event argument's Cancel property.
		/// </remarks>
        /// <example>
        /// This example shows how to cancel dragging on a particular control
        /// <code lang="C#">
        ///  private void dockingManager1_DragAllow(object sender, Syncfusion.Windows.Forms.Tools.DragAllowEventArgs arg){
        /// //Check the control which is going to be dragged and cancel according to that 
        /// if(arg.Control==panel1)
        /// arg.Cancel=true;
        /// }
        /// </code>
        /// <code lang="VB">
        /// Private Sub dockingManager1_DragAllow(ByVal sender As Object, ByVal arg As Syncfusion.Windows.Forms.Tools.DragAllowEventArgs) Handles dockingManager1.DragAllow
        /// 'Check the control which is going to be dragged and cancel according to that 
        ///     If(arg.Control==panel1)Then
        ///         arg.Cancel=true;
        ///     EndIf
        /// End Sub
        /// </code>
        ///</example>
		[
		Description("Occurs when a docking window is about to be dragged."),
		Category("DockState")
		]
		public event DragAllowEventHandler DragAllow;

        /// <summary>
        /// Occurs when a docking window is dragged over a potential dock target.
        /// </summary>
        /// <remarks> The DockAllow event is used by the <see cref="DockingManager"/> to provide
        /// information about a dock operation that is in progress. The dock can be cancelled by
        /// setting the event argument's Cancel property.
        /// </remarks>
        /// <example>
        /// <code lang="C#">
        ///  // This sample shows how to prevent tabbing between 2 controls.
        /// private void dockingManager1_DockAllow(object sender, Syncfusion.Windows.Forms.Tools.DockAllowEventArgs arg){
        /// //Checks if the each controls are trying to dock with each other, by DragControl and DockControl property 
        ///     if (((arg.DragControl == this.monthCalendarAdv1) && (arg.TargetControl == this.fontListBox1)) || ((arg.DragControl == this.fontListBox1) && (arg.TargetControl == this.monthCalendarAdv1)) && arg.DockStyle=DockingStyle.Tabbed){
        ///           // Cancel the Docking Action.
        ///           arg.Cancel = true;
        ///     }
        ///  }
        /// </code>
        /// </example>
		[
		Description("Occurs when a docking window is dragged over a potential dock target."),
		Category("DockState")
		]
		public event DockAllowEventHandler DockAllow;

		/// <summary>
		/// Occurs just before a dock operation takes place.
		/// </summary>
		[
		Description("Occurs just before a dock operation."),
		Category("DockState")
		]
		public event DockStateChangeEventHandler DockStateChanging;

		/// <summary>
		/// Occurs immediately after a dock operation.
		/// </summary>
		[
		Description("Occurs immediately after a dock operation."),
		Category("DockState")
		]
		public event DockStateChangeEventHandler DockStateChanged;
		/// <summary>
		/// Occurs before control is going to maximize.
		/// </summary>
		[
		Description("Occurs before control is going to maximize"),
		Category("DockState")
		]
		public event ControlMaximizeEventHandler ControlMaximizing;
		/// <summary>
		/// Occurs after control is maximized.
		/// </summary>
		[
		Description("Occurs after control is maximized."),
		Category("DockState")
		]
		public event ControlMaximizedEventHandler ControlMaximized;
		/// <summary>
		/// Occurs after control is minimized.
		/// </summary>
		[
		Description("Occurs after control is minimized."),
		Category("DockState")
		]
		public event ControlMinimizeEventHandler ControlMinimized;
		/// <summary>
		/// Occurs after control is restored.
		/// </summary>
		[
		Description("Occurs after control is restored."),
		Category("DockState")
		]
		public event ControlRestoreEventHandler ControlRestored;

		/// <summary>
		/// Occurs after a control's DockVisibility state has changed.
		/// </summary>
		[
		Description("Occurs after a control's DockVisibility state has changed."),
		Category("DockState")
		]
		public event DockVisibilityChangedEventHandler DockVisibilityChanged;

		/// <summary>
		/// Occurs when a control's DockVisibility state is changing.
		/// </summary>
        /// <example>
        /// This example demonstrates how to cancel closing of a docked control
        /// <code lang="C#">
        ///  private void dockingManager1_DockVisibilityChanging(object sender, Syncfusion.Windows.Forms.Tools.DockVisibilityChangingEventArgs arg){
        ///  //Check the control and cancel closing.
        ///     if (arg.Control == panel1)
        ///             arg.Cancel = true;
        /// }
        /// </code>
        /// <code lang="VB">
        ///  Private Sub dockingManager1_DockVisibilityChanging(ByVal sender As Object, ByVal arg As Syncfusion.Windows.Forms.Tools.DockContextMenuEventArgs) Handles dockingManager1.DockVisibilityChanging
        /// 'Check the control and cancel closing.
        /// if (arg.Control == Panel1) Then
        ///        arg.Cancel = true;
        /// End If
        /// End Sub
        /// </code>
        /// </example>
		[
		Description("Occurs during a control's DockVisibility state is changing."),
		Category("DockState")
		]
		public event DockVisibilityChangingEventHandler DockVisibilityChanging;

		/// <summary>
		/// Occurs when a dockable control gets activated.
		/// </summary>
		[
		Description("Occurs when a dockable control gets activated."),
		Category("Focus")
		]
		public event DockActivationChangedEventHandler DockControlActivated;

		/// <summary>
		/// Occurs when a dockable control gets deactivated.
		/// </summary>
		[
		Description("Occurs when a dockable control gets deactivated."),
		Category("Focus")
		]
		public event DockActivationChangedEventHandler DockControlDeactivated;

		/// <summary>
        /// Occurs just before the start of autohide animation.
		/// </summary>
        /// <example>This example describes how to prevent the animation when we hide controls
        /// <code lang="C#">
        /// private void dockingManager1_AutoHideAnimationStart(object sender, Syncfusion.Windows.Forms.Tools.AutoHideAnimationEventArgs arg)
        ///{
        ///    if (arg.DockBorder == DockStyle.Left || arg.DockBorder == DockStyle.Right)
        ///        Syncfusion.Windows.Forms.Tools.DockingManager.AnimationStep = arg.Bounds.Width;
        ///    else
        ///        Syncfusion.Windows.Forms.Tools.DockingManager.AnimationStep = arg.Bounds.Height;
        ///}
        /// </code>
        /// <code lang="VB">
        /// Private Sub DockingManager1_AutoHideAnimationStart(ByVal sender As System.Object, ByVal arg As Syncfusion.Windows.Forms.Tools.AutoHideAnimationEventArgs) Handles DockingManager1.AutoHideAnimationStart
        ///     If (arg.DockBorder = DockStyle.Left Or arg.DockBorder = DockStyle.Right) Then
        ///         Syncfusion.Windows.Forms.Tools.DockingManager.AnimationStep = arg.Bounds.Width
        ///     Else
        ///         Syncfusion.Windows.Forms.Tools.DockingManager.AnimationStep = arg.Bounds.Height
        ///     End If
        ///End Sub
        /// </code>
        /// </example>
		[
        Description("Occurs just before the start of autohide animation event."),
		Category("Behavior")
		]
		public event AutoHideAnimationEventHandler AutoHideAnimationStart;

		/// <summary>
        /// Occurs immediately after the end of autohide animation.
		/// </summary>
		[
        Description("Occurs immediately after the end of autohide animation event."),
		Category("Behavior")
		]
		public event AutoHideAnimationEventHandler AutoHideAnimationStop;

		/// <summary>
		/// Occurs when the right mouse button is clicked over a docking window's caption.
		/// </summary>
        /// <example>
        /// This example demonstrates how to remove menu for a particular control
        /// <code lang="C#">
        /// private void docMgr_DockContextMenu(object sender, Syncfusion.Windows.Forms.Tools.DockContextMenuEventArgs arg){
        /// //Checking the control and assigning an empty menu
        /// if (arg.Owner == panel1)
        ///       arg.ContextMenu = new Syncfusion.Windows.Forms.Tools.XPMenus.PopupMenu();
        /// }
        /// </code>
        /// <code lang="VB">
        ///  Private Sub dockingManager1_DockContextMenu(ByVal sender As Object, ByVal arg As Syncfusion.Windows.Forms.Tools.DockContextMenuEventArgs) Handles dockingManager1.DockContextMenu
        ///  'Checking the control and assigning an empty menu
        ///  If (arg.Owner == panel1) Then
        ///       arg.ContextMenu = new Syncfusion.Windows.Forms.Tools.XPMenus.PopupMenu()
        ///  End If
        ///  End Sub
        /// </code>
        /// </example>
		[
		Description("Occurs when the right mouse button is clicked over a docking window caption."),
		Category("Behavior")
		]
		public event DockContextMenuEventHandler DockContextMenu;

		/// <summary>
		/// Occurs when the redock context menu item has been clicked.
		/// </summary>
		[
		Description("The DockMenuClick event occurs when the redock context menu item has been clicked."),
		Category("Behavior")
		]
		public event DockMenuClickEventHandler DockMenuClick;

		/// <summary>
		///	Occurs just before a new dock state is loaded.
		/// </summary>
		[
		Description("Occurs just before a new dock state is loaded."),
		Category("DockState")
		]
		public event EventHandler NewDockStateBeginLoad;

		/// <summary>
		///	Occurs immediately after a new dock state has been loaded.
		/// </summary>
        /// <remarks>We can get the result of dock state loading operation if we cast the event handler argument to the DockStateLoadEventArgs</remarks>
        /// <example>
        /// <code language="C#">
        /// private void dockingManager1_NewDockStateEndLoad(object sender, EventArgs e)
        /// {
        ///     DockStateLoadEventArgs dsle = (DockStateLoadEventArgs)e;
        ///     Console.WriteLine(dsle.LoadResult.ToString());
        /// }
        /// </code>
        /// <code language="VB">
        /// Private Sub DockingManager_NewDockStateEndLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles DockingManager.NewDockStateEndLoad
        ///     Dim dsle As DockStateLoadEventArgs = CType(e, DockStateLoadEventArgs)
        ///     Console.WriteLine(dsle.LoadResult)
        /// End Sub
        /// </code>
        ///</example>
		[
		Description("Occurs immediately after a new dock state has been loaded."),
		Category("DockState")
		]
		public event EventHandler NewDockStateEndLoad;

		/// <summary>
		/// Occurs just before the start of feedback of a drag operation.
		/// </summary>
		[
		Description("Occurs just before the start of feedback of a drag operation."),
		Category("Behavior")
		]
		public event EventHandler DragFeedbackStart;

		/// <summary>
		/// Occurs immediately after the end of feedback of a drag operation.
		/// </summary>
		[
		Description("Occurs immediately after the end of feedback of a drag operation."),
		Category("Behavior")
		]
		public event EventHandler DragFeedbackStop;

		/// <summary>
		/// Occurs whenever a dockable control's caption needs to be painted.
		/// </summary>
        /// <example>
        /// This sample illustrates how to use this event to custom paint title bar of a docked control
        /// <code lang="C#">
        /// void dockingManager1_ProvideGraphicsItems(object sender, Syncfusion.Windows.Forms.Tools.ProvideGraphicsItemsEventArgs arg)
        ///{
        ///   if (arg.Control == panel1) //Checks if the control is panel1
        ///   {
        ///     if (arg.IsActiveCaption)//Different drawing for active and inactive states
        ///     {
        ///        arg.CaptionBackground = Brushes.Blue;
        ///        arg.CaptionFont = new Font("Times New Roman", 10);
        ///        arg.CaptionForeground = Color.White;
        ///     }
        ///     else{
        ///        arg.CaptionBackground = Brushes.Gray;
        ///        arg.CaptionFont = new Font("Times New Roman", 10);
        ///        arg.CaptionForeground = Color.White;
        ///     }
        ///   }
        ///}
        /// </code>
        /// <code lang="VB">
        /// Private Sub DockingManager1_ProvideGraphicsItems(ByVal sender As System.Object, ByVal arg As Syncfusion.Windows.Forms.Tools.ProvideGraphicsItemsEventArgs) Handles DockingManager1.ProvideGraphicsItems
        /// If arg.Control Is Panel1 Then ' Checks if the control is panel1
        ///     If arg.IsActiveCaption then 'Different drawing for active and inactive states
        ///         arg.CaptionBackground = Brushes.Blue
        ///         arg.CaptionFont = New Font("Times New Roman", 10)
        ///         arg.CaptionForeground = Color.White
        ///     Else
        ///         arg.CaptionBackground = Brushes.Gray
        ///         arg.CaptionFont = New Font("Times New Roman", 10)
        ///         arg.CaptionForeground = Color.White
        ///     End If
        /// End If
        /// End Sub
        /// </code>
        ///</example>
		[
		Description("Occurs whenever a docked window's caption needs to be painted."),
		Category("Appearance")
		]
		public event ProvideGraphicsItemsEventHandler ProvideGraphicsItems;

		/// <summary>
		/// Lets you specify a unique ID used to distinguish the persistence information
		/// of different instances of the Form type.
		/// </summary>
		/// <remarks>
		/// The default persistence logic assumes that applications will have only unique instances of top-level Forms.
		/// In applications that deviate from this normal and have multiple instances of the same top-level form, the
		/// persisted state of one form will be overridden by another as the default logic makes no attempt to distinguish between
		/// the multiples. The ProvidePersistenceID event allows users' to workaround this particular condition, by permitting
		/// unique identifiers to be assigned for each instance of the form.
		/// </remarks>
        [
        Description("Raises the ProvidePersistenceID event."),
        Category("DockState")
        ]
		public event ProvidePersistenceIDEventHandler ProvidePersistenceID;

		/// <summary>
		/// Occurs if serialized information is not available for a dockable control when loading a persisted dock state.
		/// </summary>
		/// <remarks>
		/// The <see cref="DockingManager"/> fires this event when it cannot find any persistence information for a dockable control when
		/// loading a saved dock state. The particular control's DockVisibility property will be set to FALSE and the control hidden.
		/// </remarks>
		[
		Description("Occurs if serialized information is not available."),
		Category("DockState")
		]
		public event DockStateUnavailableEventHandler DockStateUnavailable;

		/// <summary>
		/// Occurs when the DockingManager is not able to locate a control during
		/// a <see cref="DockingManager.LoadDockState"/> call.
		/// </summary>
		/// <remarks>
		/// The <see cref="DockingManager"/> fires this event when it is unable to find a previously persisted control
		/// during a <see cref="LoadDockState"/> operation. Applications can use this event as a hint to create and initialize
		/// controls selectively based on the control set present in the previously persisted docking layout.
		/// </remarks>
		[
		Description("Occurs when the DockingManager is not able to locate a control during a load state operation."),
		Category("DockState")
		]
		public event InitializeControlOnLoadEventHandler InitializeControlOnLoad;

		/// <summary>
		/// Occurs when a dockable control hosted by this <see cref="DockingManager"/> is about
		/// to be transferred to the docking layout hosted by some other DockingManager.
		/// </summary>
		/// <remarks>
		/// <seealso cref="DockingManager.TransferredToManager"/>
		/// <seealso cref="DockingManager.AddToTargetManagersList"/>
		/// <seealso cref="DockingManager.RemoveFromTargetManagersList"/>
		/// </remarks>
        [
        Description("Occurs when a dockable control hosted by this DockingManager is about to be transferred to the docking layout hosted by some other DockingManager."),
        Category("Behavior")
        ]
		public event TransferManagerEventHandler TransferringFromManager;

       
        /// <summary>
		/// Occurs after a dockable control that previously belonged to some other
		/// DockingManager has been transferred to the docking layout hosted by this <see cref="DockingManager"/>.
		/// </summary>
		/// <remarks>
		/// <seealso cref="DockingManager.TransferringFromManager"/>
		/// <seealso cref="DockingManager.AddToTargetManagersList"/>
		/// <seealso cref="DockingManager.RemoveFromTargetManagersList"/>
		/// </remarks>
        [
        Description("Occurs after a dockable control that previously belonged to some other."),
        Category("Behavior")
        ]
		public event TransferManagerEventHandler TransferredToManager;
        
        /// <summary>
        /// Occurs when the right mouse button is clicked over a AutoHideTabControl.
        /// </summary>
        [
        Description("Occurs when the right mouse button is clicked over a AutoHideTabControl."),
        Category("Behavior")
        ]
        public event AutoHideTabContextMenuEventHandler AutoHideTabContextMenu;
        # endregion
# region members declaration
      
        private bool m_PreviousParentVisibility;
        private Form m_PreviousParentForm;
		private string autoHideButtonToolTip = "";
		private string closeButtonToolTip = "";
		private string menuButtonToolTip = "";
		private string maximizeButtonToolTip = "";
		private string restoreButtonToolTip = "";
		private bool showToolTips = true;
        private bool showIconInAutoHideContextMenu = true;
		private bool m_FullCaptionsInAutoHideMode = false;
		private bool m_ShowCaption = true;
		private bool m_ShowImages = true;
		private bool showDockTabScrollButton = false;
        protected CaptionButtonsCollection m_CaptionButtons = new CaptionButtonsCollection();
        public static string BARITEM_IMAGES_PATH = "FrameworkComponents.DockingWindows.Images.HelpBarItemImage.";
        private bool m_showAutohidetabContextMenu = true;
        private bool m_EnableDoubleClickOnCaption = true;
        private bool m_DragFeedbackEventsOnSplitters = true;
        private bool bEnableDragAutoHiddenTabs = false;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		private bool m_enableSuperToolTip = false;
#endif
        private ArrayList alautohide = null;
		private BrushInfo bIActiveCaptionBackround=new BrushInfo(SystemColors.ActiveCaption);
		private Color cActiveCaptionForeGround =SystemColors.ActiveCaptionText;
		private Font fntActiveCaptionFont =SystemInformation.MenuFont;

		private BrushInfo bIInActiveCaptionBackround=new BrushInfo(SystemColors.Control);
		private Color cInActiveCaptionForeGround = SystemColors.ControlText;
		private Font fntInActiveCaptionFont =SystemInformation.MenuFont;
		private Control m_activeControl = null;
        private bool showCustomButtonsInFloating;
		[Syncfusion.Documentation.DocumentationExclude()]
        protected AutoHideSelectionStyle autoHideSelection = AutoHideSelectionStyle.MouseHover;
		// Contains information about last DockHost that received focus in designtime.
		internal DockHost dhLastActive = null; 

		// helper window for handling activation/deactivation events
		private NotifyNativeWindow m_nativeWindow = null;
		
		[Syncfusion.Documentation.DocumentationExclude()]
		protected DockHostController controllerInFocus = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Control ctrlLastActive = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected AHTabControl autoHideTabInView = null;

		protected internal AHTabControl ahTabAnimate = null;
		protected internal Control ctrlLastPainted = null;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal ArrayList alDockAreaControllers = new ArrayList();
		[Syncfusion.Documentation.DocumentationExclude()]
		protected ArrayList alFFControllers = new ArrayList();
		private ArrayList alHiddenFFControllers = new ArrayList();
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal MainFormController dcHostForm;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal ArrayList m_freezeResizeCtrls = new ArrayList();

		[Syncfusion.Documentation.DocumentationExclude()]
		protected IDragProvider m_dragProvider = null;
		protected Office2007Theme m_Office2007Theme = Office2007Theme.Blue;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        private DockMenuStyle m_MenuStyle=DockMenuStyle.VS2005 ;
        [Syncfusion.Documentation.DocumentationExclude()]
        protected VisualStyle m_RendererStyle = VisualStyle.VS2005;
        [Syncfusion.Documentation.DocumentationExclude()]
		protected DragProviderStyle providerStyle = DragProviderStyle.VS2005;
#else
        private DockMenuStyle m_MenuStyle=DockMenuStyle.VS2003;
        [Syncfusion.Documentation.DocumentationExclude()]
		protected VisualStyle m_RendererStyle=VisualStyle.Default;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected DragProviderStyle providerStyle;
#endif
        [Syncfusion.Documentation.DocumentationExclude()]
		protected DockingManagerRenderer m_Renderer;
		
        
        [Syncfusion.Documentation.DocumentationExclude()]
		protected bool bAutoHideEnabled = true;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bCloseEnabled = true;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bMaximizeEnabled = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bFormClientBorder = true;
        [Syncfusion.Documentation.DocumentationExclude()]
		protected bool bMenuButtonEnabled = true;
		protected bool bAllowTabsMoving = true;
        internal bool bHostFormClosing = false;
		protected ArrayList m_mdiZOrder = new ArrayList();


		[Syncfusion.Documentation.DocumentationExclude()]
		protected ImageList ilDockTabs;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bControlScopeImages = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bPersistState = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected const String strPersistKey = "DockStateInfo";
		
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bEnableContextMenu = true;

		// Designer Implementation
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Hashtable htIcon = new Hashtable();
        [Syncfusion.Documentation.DocumentationExclude()]
        protected Hashtable htmIcon = new Hashtable();
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Hashtable htText = new Hashtable();
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Hashtable htFloatOnly = new Hashtable();
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Hashtable htAllowFloating = new Hashtable();
		[Syncfusion.Documentation.DocumentationExclude()]
		protected ArrayList listAHOnLoad = new ArrayList();
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Hashtable htHiddenOnLoad = new Hashtable();
		[Syncfusion.Documentation.DocumentationExclude()]
		protected ArrayList alInheritedControls = new ArrayList();
		protected Hashtable htDockAbility = new Hashtable();
		protected Hashtable htOuterDockAbility = new Hashtable();
		protected Hashtable htCustomCaptionButtons = new Hashtable();
		
		[Syncfusion.Documentation.DocumentationExclude()]
		protected MemoryStream stmLayout = new MemoryStream();
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bLoadVisibility = true;
		
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal ArrayList alEnableDocking = new ArrayList();
		
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bInBeginEndInit = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bHoldEvents = false;
        protected int dactivatedFlag = 0;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bWaitOnLayoutEvent = false;

		protected internal bool bCancelVisibilityChangingEvent = false;
		protected internal bool bFiredVisibilityChangingEvent = false;
		protected internal bool bFiredVisibilityChangedEvent = false;
        protected internal bool bFreezeDockStateChangeEvents = false;
		protected internal bool bAllowActivationEvents = true;
		protected internal bool bMdiFormClosing = false;
		
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bDockToFill = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal DockingStyle dsDockFillAHBorder = DockingStyle.Fill;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bDisallowFloating = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bAutoHideActiveControl = false;

		// Tracks visibility state for a non-Form HostControl
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bFloatingVisibility = false;
		// References the Owner form for floating controls when the HostControl is a non-Form type
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal Form frmOwner = null;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bMDIActivatedVisibility = true;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bHostActivatedVisibility = true;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected Color m_borderColor = SystemColors.ControlDark;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool m_paintBorders = true;

		private bool layoutSuspended = false; // Specifies if the Layout is suspended

		private bool unHookedCallWnd = false;// Specifies if the CallWnd hook has been suspended
		private bool unHookedGetMsg = false;// Specifies if the GetMsg hook has been suspended

		private int m_nSuspendLayoutCount = 0; //Used to avoid recursive suspend layout.
		private int m_curentThreadId = 0;// Holds ThreadID on which designerHooks were enabled.
		private bool m_singleTabOperate = false;
		private bool m_bEscKeyPressed = false;
		private int m_lockCounter = 0;
		private bool m_bDesignDockStateLoad = false;
		private bool m_bOffice2007MdiChildForm = false;
		private Color m_mdiOffice2007CustomColor = Color.SkyBlue;
        private Color m_mdiOffice2010CustomColor = Color.SkyBlue;
        private Office2007Theme m_mdiOffice2007ColorScheme = Office2007Theme.Managed;
		private bool m_bCustomMdiColor = false;
		private bool m_bCustomMdiStyle = true;
        private bool m_isInitializing = true;
        private ArrayList m_NeedFocusControls = new ArrayList();
        private Control m_ValidatingCancelledControl = null;
        internal bool m_bLoadingDockState = false;
        private bool m_bNeedDTFPriorityController = true;  // Specifies whether PriorityController need to be set when in DockToFill mode.
        private bool importDockingControl = true;
        private bool needFloatFormRepaint = true;
        private bool reduceFlicker = false;

        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
        private string style;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return style;
            }
            set
            {
                style = value;

                if (value == "Office2007Blue")
                    Office2007Theme = Office2007Theme.Blue;
                else if (value == "Office2007Silver")
                    Office2007Theme = Office2007Theme.Silver;
                else if (value == "Office2007Black")
                    Office2007Theme = Office2007Theme.Black;
                else if (value == "Managed")
                    Office2007Theme = Office2007Theme.Managed;
                else if (value == "Default")
                    VisualStyle = VisualStyle.Default;
                else if (value == "Office2003")
                    VisualStyle = VisualStyle.Office2003;
                else if (value == "Office2007Outlook")
                    VisualStyle = VisualStyle.Office2007Outlook;
                else if (value == "OfficeXP")
                    VisualStyle = VisualStyle.OfficeXP;
               

            }
        }
		/// <summary>
        /// Gets or sets color scheme for Office2007 MDI children
		/// </summary>
		[Category( "Appearance" )
        , Description("Gets or sets color scheme for Office2007 MDI children")
		, DefaultValue( typeof( Office2007Theme ), "Managed" )]
		public Office2007Theme Office2007MdiColorScheme
		{
			get
			{
				return m_mdiOffice2007ColorScheme;
			}
			set
			{
				if( m_mdiOffice2007ColorScheme != value )
				{
					m_mdiOffice2007ColorScheme = value;

					foreach( Control ctrl in this.alEnableDocking )
					{
						DockingWrapperForm form = ctrl.Parent as DockingWrapperForm;

						if( form != null )
							form.ColorScheme = m_mdiOffice2007ColorScheme;
					}

					m_bCustomMdiColor = true;
				}
			}
		}
        
        /// <summary>
        /// Gets or sets a value indicating whether floating form need to be repainted.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if floating form repaint needed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>If there is any flickering while resizing floating form, this property can be set to false.
        /// This property is applicable for visual styles other than Default and VS2005.
        /// </remarks>
        [DefaultValue(true), Category("Behavior"), Description("Indicates whether floating form needs to be repainted.")]
        public bool NeedFloatFormRepaint
        {
            get { return needFloatFormRepaint; }
            set 
            {
                if (needFloatFormRepaint != value)
                    needFloatFormRepaint = value;
            }
        }

		/// <summary>
        /// Gets or sets if docking MDI children should be in Office2007 style.
		/// </summary>
		[ DefaultValue( false )
		, Category( "Appearance" )
        , Description("Gets or sets if docking MDI children should be in Office2007 style.")]
		public bool Office2007MdiChildForm
		{
			get
			{
				return m_bOffice2007MdiChildForm;
			}
			set
			{
				if( m_bOffice2007MdiChildForm != value )
				{
					m_bOffice2007MdiChildForm = value;

					if( !this.DesignMode )
					{
						foreach( Control ctrl in this.alEnableDocking )
						{
							DockingWrapperForm parentForm = ctrl.Parent as DockingWrapperForm;

							if( parentForm != null )
								parentForm.DisableOffice2007Style = !value;
						}
					}

					if( this.VisualStyle == VisualStyle.Office2007
						|| this.VisualStyle == VisualStyle.Office2007Outlook )
					{
						if( !value )
							m_bCustomMdiStyle = true;
					}
					else
						if( value )
							m_bCustomMdiStyle = true;
				}
			}
		}

		internal bool DesignDockStateLoad
		{
			get
			{
				return m_bDesignDockStateLoad;
			}
			set 
			{
				if( m_bDesignDockStateLoad != value )
				{
					m_bDesignDockStateLoad = value;
				}
			}
		}

		internal bool EscapeKeyPressed
		{
			get
			{
				return m_bEscKeyPressed;
			}
		}
     
        internal DockingManagerRenderer Renderer
		{
			get { return m_Renderer; }
		}

		protected internal bool HoldEvents
		{
			get
			{
				return  bHoldEvents;
			}
		}

		internal bool InBeginEndInit
		{
			get
			{
				return this.bInBeginEndInit;
			}
		}

		private Font m_CaptionTextFont;

		internal Font CaptionTextFont
		{
			get { return m_CaptionTextFont; }
		}

		internal void UpdateFonts(float DPI)
		{
			Font oldfont = DockingManager.ftSysInfoMenuFont;
			Font newfont = SystemInformation.MenuFont;
			if (oldfont != newfont)
				DockingManager.ftSysInfoMenuFont = newfont;

			if (this.DockTabHeight == oldfont.Height + 8)
				this.DockTabHeight = newfont.Height + 8;
			if (this.DockTabFont == oldfont)
				this.DockTabFont = newfont;
			if (this.AutoHideTabHeight == oldfont.Height + 8)
				this.AutoHideTabHeight = newfont.Height + 8;
			if (this.AutoHideTabFont == oldfont)
				this.AutoHideTabFont = newfont;

			const int nSysMenuFontDPI = 72;

			if (m_CaptionTextFont.Size * DPI != newfont.Size * nSysMenuFontDPI)
			{
				int nSize = Convert.ToInt32(newfont.Size * nSysMenuFontDPI / DPI);
				float ftSize = nSize;
				m_CaptionTextFont = new Font("Tahoma", ftSize, FontStyle.Bold);
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected DockTabAlignmentStyle dtAlignment = DockTabAlignmentStyle.Bottom;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected DockLabelAlignmentStyle dlAlignment = DockLabelAlignmentStyle.Default;

		/// <summary>
        /// Frame painter
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		public FramePainter FramePainter;
        /// <summary>
        /// Point where the floating form need to be shown. Added to increase the customization
        /// </summary>
        internal Point m_FloatingLocation;
        /// <summary>
        /// Controls Collection used to add the controls that need cusomization of  floating form location
        /// </summary>
        internal ArrayList m_CustomizeControlCollection = new ArrayList();
		[
		Syncfusion.Documentation.DocumentationExclude(),
		Browsable(false)
		]
		static public int AnimationSpeed = 1;	// Animation interval used by the autohide timer

		
        /// <summary>
        /// Gets/Sets the step size of autohide animation.It can be used to control the speed of animation
        /// </summary>
        /// <value> An integer value specifying step size.Default is 25</value>
        /// <example>This example describes how to prevent the animation when we hide controls
        /// <code lang="C#">
        /// private void dockingManager1_AutoHideAnimationStart(object sender, Syncfusion.Windows.Forms.Tools.AutoHideAnimationEventArgs arg)
        ///{
        ///    if (arg.DockBorder == DockStyle.Left || arg.DockBorder == DockStyle.Right)
        ///        Syncfusion.Windows.Forms.Tools.DockingManager.AnimationStep = arg.Bounds.Width;
        ///    else
        ///        Syncfusion.Windows.Forms.Tools.DockingManager.AnimationStep = arg.Bounds.Height;
        ///}
        /// </code>
        /// <code lang="VB">
        /// Private Sub DockingManager1_AutoHideAnimationStart(ByVal sender As System.Object, ByVal arg As Syncfusion.Windows.Forms.Tools.AutoHideAnimationEventArgs) Handles DockingManager1.AutoHideAnimationStart
        ///     If (arg.DockBorder = DockStyle.Left Or arg.DockBorder = DockStyle.Right) Then
        ///         Syncfusion.Windows.Forms.Tools.DockingManager.AnimationStep = arg.Bounds.Width
        ///     Else
        ///         Syncfusion.Windows.Forms.Tools.DockingManager.AnimationStep = arg.Bounds.Height
        ///     End If
        ///End Sub
        /// </code>
        /// </example>

        [
			Browsable(false)
		]
		static public int AnimationStep = 25;	// Animation sizing step used by the autohide timer

		[
		Syncfusion.Documentation.DocumentationExclude()
		]
		protected internal int MaxRedockFactor = 2;	// The max fraction of the form's dimension upto which redocking is allowed.

		[
		Syncfusion.Documentation.DocumentationExclude()
		]
		protected bool bDesignProcess = false;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bThemesEnabled = false;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool m_bIsMirrored = false;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal static int nShowInterval = 400;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal static int nHideInterval = 401;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bFreezeResizing = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool bApplyMinMaxExtents = true;

		[Syncfusion.Documentation.DocumentationExclude()]
		static protected internal Font ftSysInfoMenuFont = SystemInformation.MenuFont;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nDockTabHeight = DockingManager.ftSysInfoMenuFont.Height + 8;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Font ftDockTab = DockingManager.ftSysInfoMenuFont;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nAutoHideTabHeight = DockingManager.ftSysInfoMenuFont.Height + 8;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Font ftAutoHideTab = DockingManager.ftSysInfoMenuFont;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected ArrayList alTargetManagers = new ArrayList();

		internal ArrayList controllers = new ArrayList();

		private int lockUpdates = 0;
		private int splitterWidth = 4;

		internal bool lockedHostForm = false;

        private Keys browsingKey = Keys.None;
		
		internal bool LockUpdates
		{
			get { return lockUpdates > 0; }
        }

        protected internal ArrayList TargetManagers
        {
            get { return this.alTargetManagers; }
        }

        protected internal bool m_IsCaptionButtonsCleared = false;

        protected internal void SaveCaptionButtionsClearedState()
        {
            try
            {
                AppStateSerializer captionSerializer = new AppStateSerializer(SerializeMode.XMLFile, @"c:\temp\SaveCaption.xml");
                captionSerializer.SerializeObject("Caption_Button_state", this.m_IsCaptionButtonsCleared);
                captionSerializer.PersistNow();
            }
            catch (Exception ex)
            {
                Debug.Assert(false, "SaveCaptionButtonState failed", ex.Message);
            }
        }
        
        protected internal void LoadCaptionButtionsClearedState()
        {
            try
            {
                if (File.Exists(@"c:\temp\SaveCaption.xml"))
                {
                    AppStateSerializer captionSerializer = new AppStateSerializer(SerializeMode.XMLFile, @"c:\temp\SaveCaption.xml");
                    this.m_IsCaptionButtonsCleared = (bool)captionSerializer.DeserializeObject("Caption_Button_state");
                }
            }
            catch {}
        }

        # endregion

# region Private methods
        private bool IsDesignMode()
        {
            StackTrace stackTrace = new StackTrace();
            int frameCount = stackTrace.FrameCount - 1;

            for (int frame = 0; frame < frameCount; frame++)
            {
                Type type = stackTrace.GetFrame(frame).GetMethod().DeclaringType;
                if (typeof(IDesignerHost).IsAssignableFrom(type))
                    return true;
            }
            return false;
        }
        private DockControllerBase GetController(Control ptctrl, Point pt)
        {
            ArrayList managers = new ArrayList(this.alTargetManagers);
            managers.Add(this);

            DockControllerBase dctargetcontroller = null;
            DockingManager dockingmgr = null;
            DockControllerBase tmpController = null;
            Rectangle minRect = Rectangle.Empty;
            bool bFirst = true;

            // Find smaller controller of all existing and suitable.
            for (int i = 0, len = managers.Count; i < len; i++)
            {
                dockingmgr = managers[i] as DockingManager;

                if (dockingmgr != null)
                {
                    tmpController = this.GetDockControllerFromManager(dockingmgr, ptctrl, pt);

                    if (tmpController != null &&
                        ((tmpController.LayoutRect.Width <= minRect.Width &&
                            tmpController.LayoutRect.Height <= minRect.Height) || bFirst))
                    {
                        minRect = tmpController.LayoutRect;
                        bFirst = false;
                        dctargetcontroller = tmpController;
                    }
                }
            }

            return dctargetcontroller;
        }

		internal bool ReloadingDesigner
		{
			get
			{
				bool reloading = false;
				IDesignerHost idesignHost = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
				if( idesignHost != null )
				{
					IDesigner idsgnr = idesignHost.GetDesigner(this);
					if( idsgnr != null && idsgnr is DockingManagerDesigner )
					{
						DockingManagerDesigner dockManDesign = idsgnr as DockingManagerDesigner;
						reloading = dockManDesign.bReloading;
					}
				}
				return reloading;
			}
		}

        //Fix for issue #13051

        /// <summary>
        /// Get or sets the previous active control.
        /// </summary>
        /// <remarks>
        /// Internal Property used to persist the Last (previous) active control of the docking manager when the container of the docking manager lose the focus.
        /// This is mainly used to persist the control when the Main Form Lose it's focus when a pop up like messagebox is shown.
        /// </remarks>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        Syncfusion.Documentation.DocumentationExclude()
        ]
        protected internal Control LastActiveControl
        {
            get
            {
                return this.ctrlLastActive;
            }
            set
            {
                if (this.ctrlLastActive != value && value != null)
                    this.ctrlLastActive = value;
            }
        }

        //Fix for issue #13051

        /// <summary>
        /// Get or sets the Validating cancelled control if any.
        /// </summary>
        /// <remarks>
        /// Internal Property used to restore the focus when validating is cancelled by any of the child controls.
        /// </remarks>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        Syncfusion.Documentation.DocumentationExclude()
        ]
        protected internal Control ValidatingCancelledControl
        {
            get
            {
                return this.m_ValidatingCancelledControl;
            }
            set
            {
                if (this.m_ValidatingCancelledControl != value)
                    this.m_ValidatingCancelledControl = value;
            }
        }

        private void ReduceParentSize(DockControllerBase ctrl, SizingController sc)
		{
			SizingController parentSc = sc;
			DockControllerBase childSc = ctrl;

			if( !DockToFill )
			{
				while( true )
				{
					RefreshEmptySize( parentSc );
					SizingController nextParent = parentSc.ParentController as SizingController;

					if( nextParent != null
						&& ( nextParent.DockingOrder == parentSc.DockingOrder || nextParent.IsEmpty ) )
					{
						childSc = parentSc;
						parentSc = nextParent;
					}
					else
						break;
				}
			}
		}

		private void RefreshEmptySize( SizingController parentSc )
		{
			if( parentSc.IsEmpty )
			{
				if( parentSc.DockingOrder == DockPreference.Horizontal )
				{
					parentSc.LayoutRect = new Rectangle( parentSc.LayoutRect.Left, parentSc.LayoutRect.Top,
						0, parentSc.LayoutRect.Height );
				}
				else
				{
					parentSc.LayoutRect = new Rectangle( parentSc.LayoutRect.Left, parentSc.LayoutRect.Top,
						parentSc.LayoutRect.Width, 0 );
				}
				parentSc.DITransient.rcDockArea = Rectangle.Empty;
			}
		}

        private void ResizeSplitters(int newLength)
        {
            foreach (DockControllerBase controller in alDockAreaControllers)
            {
                DragSplitterController splitterController = controller as DragSplitterController;
                if (splitterController != null)
                {
                    switch (splitterController.DICurrent.DP)
                    {
                        case DockPreference.Horizontal:
                            {
                                splitterController.LayoutRect = new Rectangle
                                    (splitterController.LayoutRect.Left,
                                    splitterController.LayoutRect.Top, newLength,
                                    splitterController.LayoutRect.Height);
                                break;
                            }
                        case DockPreference.Vertical:
                            {
                                splitterController.LayoutRect = new Rectangle
                                    (splitterController.LayoutRect.Left,
                                    splitterController.LayoutRect.Top,
                                    splitterController.LayoutRect.Width,
                                    newLength);
                                break;
                            }
                    }
                }
            }
            (alDockAreaControllers[0] as MainFormController).AdjustLayout();
        }
        private void AssignNotifyWindow()
        {
            if (m_nativeWindow == null)
            {
                m_nativeWindow = new NotifyNativeWindow(this, HostControl);
                CreateParams cp = new CreateParams();
                cp.Parent = HostControl.Handle;
                m_nativeWindow.CreateHandle(cp);
            }
        }
        private static void PerformFormsExchange(DockControllerBase ctrl)
        {
            SizingController sc = null;
            // This code performs forms exchange between controllers if needed.
            if (ctrl.ToplevelController is FloatingFormController)
            {
                DockStateControllerBase undock = null;

                if (ctrl.ParentController is DockTabController)
                {
                    sc = ctrl.ParentController.ParentController as SizingController;
                    undock = ctrl.ParentController as DockStateControllerBase;
                }
                else
                {
                    sc = ctrl.ParentController as SizingController;
                    undock = ctrl as DockStateControllerBase;
                }

                if (sc != null
                    && !(sc.ParentController is FloatingFormController
                    && sc.ChildCount == 1))
                {
                    if (ctrl.ToplevelController.HostControl
                        == (ctrl as DockStateControllerBase).InternalForm)
                    {
                        DockControllerBase dcsibling = null;
                        int nindex = undock.DICurrent.nDockIndex;
                        if (nindex == sc.ChildCount - 1)
                            dcsibling = sc.GetChildAt(nindex - 2);
                        else
                            dcsibling = sc.GetChildAt(nindex + 2);
                        DockStateControllerBase sibling = dcsibling as DockStateControllerBase;
                        FloatingForm ff;

                        if (sibling != null && !(sibling is DockStateControllerWrapper))
                        {
                            ff = sibling.InternalForm;
                            sibling.InternalForm = undock.InternalForm;
                            undock.InternalForm = ff;
                        }
                        else
                        {
                            SizingController sizingCtrl = dcsibling as SizingController;
                            DockStateControllerBase dockCtrl = null;
							ArrayList controls = null;
								
							if( sizingCtrl != null )
							{
								controls = sizingCtrl.GetDockControllers();

								if( controls.Count > 0 )
									dockCtrl = controls[0] as DockStateControllerBase;
							}
							else
							{
								sizingCtrl = ctrl.ToplevelController.ChildControllers[0] as SizingController;

								if( sizingCtrl != null )
								{
									controls = sizingCtrl.GetDockControllers();

									if( controls.Count > 0 )
										dockCtrl = controls[0] as DockStateControllerBase;
								}
							}

                            if (dockCtrl != null)
                            {
                                ff = dockCtrl.InternalForm;
                                dockCtrl.InternalForm = undock.InternalForm;
                                undock.InternalForm = ff;
                            }
                        }
                    }
                }
            }
        }
        private void UpdateControllers()
        {
            foreach (DockControllerBase controller in alDockAreaControllers)
            {
                controller.UpdateControl();
            }

            foreach (FloatingFormController controller in alFFControllers)
            {
                controller.UpdateControl();
            }
        }
# endregion      

# region Internal properties

        /// <summary>
        /// Gets or sets the floating form's location.
        /// </summary>
        internal Point FloatingLocation
        {
            get
            {
                return this.m_FloatingLocation;
            }
            set
            {
                if (this.m_FloatingLocation != value)
                    this.m_FloatingLocation = value;
                
            }
        }

        /// <summary>
        /// Maintains the collecition of control that needs customization.
        /// </summary>
        internal ArrayList CustomizeControlCollection
        {
            get
            {
                return this.m_CustomizeControlCollection;
            }
            set
            {
                if (this.m_CustomizeControlCollection != value)
                    this.m_CustomizeControlCollection = value;
            }
        }

        internal bool IsMirrored
        {
            get
            {
                return m_bIsMirrored;
            }
        }
        internal new bool DesignMode
        {
            get { return base.DesignMode; }
        }
        internal bool DesignProcess
        {
            get { return this.bDesignProcess; }
        }
        
        private IContainer m_container;
        internal ComponentCollection HostFormComponents
        {
            get
            {
                if (m_container != null)
                {
                    return m_container.Components;
                }
                else
                {
                    return null;
                }
            }
        }
        internal ArrayList FloatingFormControllers
        {
            get
            {
                return alFFControllers;
            }
        }

		internal ArrayList MdiZOrder
		{
			get
			{
				return m_mdiZOrder;
			}
		}
        
        private bool m_bForbidWrapperLogic = false;
        internal bool ForbidWrapperLogic
        {
            get
            {
                return m_bForbidWrapperLogic;
            }
        }
        internal bool ApplyMinMaxExtents
        {
            get
            {
                return bApplyMinMaxExtents;
            }
        }
        
        private bool m_bForceHideActiveControl = false;
        internal bool ForceHideActiveControl
        {
            get
            {
                return m_bForceHideActiveControl;
            }
            set
            {
                if (m_bForceHideActiveControl != value)
                {
                    m_bForceHideActiveControl = value;
                }
            }
        }

        internal ArrayList NeedFocusControls
        {
            get
            {
                return this.m_NeedFocusControls;
            }
        }

        private int m_lockActivationEvents = 0;
        internal int LockActivationEvents
        {
            get { return m_lockActivationEvents; }
            set
            {
                m_lockActivationEvents = value >= 0 ? value : 0;
            }
        }

        private bool m_stopControlActivation = false;
        internal bool StopActivationEvents
        {
            get { return m_stopControlActivation; }
            set { m_stopControlActivation = value; }
        }

# endregion
# region public properties

        /// <summary>
        /// Gets or sets the visibility of the control's parent before docking.
        /// </summary>
        internal bool PreviousParentVisibility
        {
            get
            {
                return this.m_PreviousParentVisibility;
            }
            set
            {
                if (this.m_PreviousParentVisibility != value)
                {
                    this.m_PreviousParentVisibility = value;
                }
            }
        }

        /// <summary>
        /// Gets of sets the control which was the the parent of the control to be docked, before docking.
        /// </summary>
        internal Form PreviousParentForm
        {
            get
            {
                return this.m_PreviousParentForm;
            }
            set
            {
                if (this.m_PreviousParentForm != value)
                {
                    this.m_PreviousParentForm = value;
                }
            }
        }

		/// <summary>
		/// Gets or sets the Control hosting the <see cref="DockingManager"/> and all the associated dockable controls.
		/// </summary>
		/// <value>The <see cref="System.Windows.Forms.Control"/> that will host the docking windows.</value>
		/// <remarks>
		/// This property references the Control containing the DockingManager and all the dockable controls.
		/// A Control can contain only a single instance of the DockingManager.
		/// </remarks>
		[
		Browsable(false),
		Description("Gets or sets the form that will host the DockingManager.")
		]
		public ContainerControl HostControl
		{

			get
			{
				if (this.dcHostForm == null)
					return null;
				return this.dcHostForm.HostControl as ContainerControl;
			}

			set
			{
				if (value != null)
				{
					if ((this.dcHostForm == null) || ((this.dcHostForm != null) && (this.dcHostForm.HostControl != value)))
					{
						if ((this.bDockToFill == true) && (value is Form) && ((value as Form).IsMdiContainer))
						{
							Debug.Assert(false, "The DockToFill option may be used only with a non-MDIContainer form.");
							return;
						}
						this.dcHostForm = this.CreateMainFormController(value);
						Debug.Assert(this.dcHostForm != null);
						AddController(this.dcHostForm);

						if( HostControl.Created )
						{
							using(Graphics g = Graphics.FromHwnd( HostControl.Handle ))
							UpdateFonts( g.DpiY );
						}

						value.SystemColorsChanged += new EventHandler(this.HostControl_SystemColorsChanged);
						Control ctrlHost = value as Control;
						if (null != ctrlHost)
						{
							this.UpdateRightToLeftProperty(ctrlHost.RightToLeft);
							ctrlHost.RightToLeftChanged += new EventHandler(HostControl_RightToLeftChanged);
						}
						if (this.DesignProcess == false)
						{
							this.dcHostForm.UpdateFormClientSetting();
							this.dcHostForm.AdjustLayoutDockArea();
							value.Paint += new PaintEventHandler(this.HostControl_Paint);

							if (HostControl.IsHandleCreated)
							{
								AssignNotifyWindow();
							}

							if (value is Form)
							{
								(value as Form).Closed += new EventHandler(this.HostForm_Closed);
								value.ParentChanged += new EventHandler(this.HostControl_ParentChanged);
							}
							else
							{
								Form owner = value.ParentForm;
								if (owner != null)
								{
									this.frmOwner = owner;
									this.frmOwner.HandleCreated += new EventHandler(this.OwnerForm_HandleCreated);
								}
								// Run up the window hierarchy and subscribe to the ParentChanged and ControlRemoved events
								this.RecSubscribeHostControlEvents(value, true);
							}
						}
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets the form hosting the <see cref="DockingManager"/> and all the associated dockable controls.
		/// </summary>
		/// <value>The <see cref="System.Windows.Forms.Form"/> that will host the docking windows.</value>
		/// <remarks>
		/// This property references the form containing the DockingManager and all the dockable controls.
		/// A form can contain only a single instance of the DockingManager.
		/// </remarks>
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Description("Gets or sets the form that will host the DockingManager.")
		]
		public Form HostForm
		{
			get
			{
				if ((this.HostControl == null) || ((this.HostControl is Form) == false))
					return null;
				return (this.HostControl as Form);
			}

			set { this.HostControl = value; }
		}

        /// <summary>
        /// Gets the dock are controllers.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        EditorBrowsable(EditorBrowsableState.Never),
        Syncfusion.Documentation.DocumentationExclude()
        ]
		public ArrayList DockAreaControllers
		{
			get { return this.alDockAreaControllers; }
		}


		/// <summary>
        /// A MemoryStream containing the dockstate information set by the visual designer.
		/// </summary>
		[
		Browsable(false),
		DefaultValue(null),
		Description("The stream containing the dockstate data set by the visual designer.")
		]
		[Syncfusion.Documentation.DocumentationExclude()]
		public MemoryStream DockLayoutStream
		{
			get { return this.stmLayout; }
			set { this.stmLayout = value; }
		}

		/// <summary>
		/// Indicates whether the application's docking windows state should be persisted.
		/// </summary>
		/// <value>TRUE indicates the application's dock state will be persisted. Default is FALSE.</value>
		/// <remarks>
		/// When this property is set to TRUE, the application's dock state will be persisted upon application exit
		/// and restored during the subsequent launch.
		/// </remarks>
		[
		Description("Gets or sets a value indicating whether the application's docking windows state should be persisted."),
		DefaultValue(false),
		]
		public bool PersistState
		{
			get { return this.bPersistState; }
			set { this.bPersistState = value; }
		}

		/// <summary>
		/// Gets or sets the style of dragging.
		/// </summary>
		/// <remarks>
		/// The DragProviderStyle enumeration is used by the <see cref="DockingManager"/> to enable the style of dragging
		/// the docking windows. VS2005 style is set for Visual Studio 2005 by default. Standard style will be set for VS 2002 and VS 2003 .NET Framework.
		/// 
		/// </remarks>
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		[DefaultValue(DragProviderStyle.VS2005)]
#else
		[DefaultValue(DragProviderStyle.Standard)]
#endif
		[
		Category("Behavior"),
		Description("Docking provider style.")
		]
		public DragProviderStyle DragProviderStyle
		{
			get { return this.providerStyle; }
			set
			{
				if (providerStyle != value)
				{
					InitializeDragProvider(value);
				}
			}
		}

        /// <summary>
        /// Gets or sets the menu style.
        /// </summary>
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		[DefaultValue(DockMenuStyle.VS2005)]
#else
		[DefaultValue(DockMenuStyle.VS2003)]
#endif
		[
		Category("Behavior"),
		Description("Specifies behavior of context menus of docking windows.")
		]
		public DockMenuStyle MenuStyle
		{
			get
			{
				return m_MenuStyle;
			}
			set
			{
				if( m_MenuStyle != value )
				{
					m_MenuStyle = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets the visual style for the docking controls. OfficeXP style will reflect the Office2003 style.
		/// </summary>
		/// <value>A <see cref="Syncfusion.Windows.Forms.VisualStyle"/> containing the various visual styles.</value>
		/// 
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		[DefaultValue(VisualStyle.VS2005)]
#else
		[DefaultValue(VisualStyle.Default)]
#endif
		[
		Category("Behavior"),
        Description("Specifies the docking manager's visual style.")
		]
		public VisualStyle VisualStyle
		{
			get { return m_RendererStyle; }
			set
			{
				if( m_RendererStyle != value )
				{
					m_RendererStyle = value;
					if (value == VisualStyle.Metro)
					{
						(m_Renderer as DockingManagerRenderer).MetroColor = this.MetroColor;
                        (m_Renderer as DockingManagerRenderer).MetroCaptionColor = this.MetroCaptionColor;
                        (m_Renderer as DockingManagerRenderer).MetroButtonColor = this.MetroButtonColor;
                        (m_Renderer as DockingManagerRenderer).MetroSplitterColor = this.MetroSplitterBackColor;
                        bIInActiveCaptionBackround = new BrushInfo(Color.FromArgb(209,211,212));
					}
					m_Renderer.VisualStyle = value;
					if (value == VisualStyle.Office2007 || value == VisualStyle.Office2007Outlook)
					{
						if( !this.InBeginEndInit && !m_bCustomMdiStyle )
							this.Office2007MdiChildForm = true;

						m_Renderer.RefreshOffice2007Theme(this.Office2007Theme);
					}
                    if (value == VisualStyle.Office2010)
                    {
                        if (!this.InBeginEndInit && !m_bCustomMdiStyle)
                            this.Office2007MdiChildForm = false;

                        m_Renderer.RefreshOffice2010Theme(this.Office2010Theme);
                    }
                    else
                        if (!this.InBeginEndInit && !m_bCustomMdiStyle)
                            this.Office2007MdiChildForm = false;
					UpdateControllers();
					if( value != VisualStyle.Default )
					{
						this.Renderer.IsMirrored = this.IsMirrored;
					}
				}
			}
		}
        private Color metroCaption = Color.White;

        [Description("Indicates the metro caption color.")]
        [Category("Appearance")]
        public Color MetroCaptionColor
        {
            get
            {
                return metroCaption;
            }
            set
            {
                if (metroCaption != value)
                {
                    metroCaption = value;
                    (m_Renderer as DockingManagerRenderer).MetroCaptionColor = this.MetroCaptionColor;
                }
                
            }
        }

        private Color metroSplitter = ColorTranslator.FromHtml("#9B9FB7");

        [Description("Indicates the Metro Splitter Backcolor.")]
        [Category("Appearance")]
        public Color MetroSplitterBackColor
        {
            get
            {
                return metroSplitter;
            }
            set
            {
                if (metroSplitter != value)
                {
                    metroSplitter = value;
                    (m_Renderer as DockingManagerRenderer).MetroSplitterColor = this.MetroSplitterBackColor;
                }

            }
        }

        private Color metroButton = Color.FromArgb(255, 255, 255);

        [Description("Indicates the Metro Button color.")]
        [Category("Appearance")]
        public Color MetroButtonColor
        {
            get
            {
                return metroButton;
            }
            set
            {
                if (metroButton != value)
                {
                    metroButton = value;
                    (m_Renderer as DockingManagerRenderer).MetroButtonColor = this.MetroButtonColor;
                }
                
            }
        }
		/// <summary>
		///MetroColor
		/// </summary>
        private Color metroColor = ColorTranslator.FromHtml("#16A5DC");

        /// <summary>
        /// Indicates the metro color.
        /// </summary>
        /// <value>Default value is true.</value>
        [Description("Indicates the metro color.")]
        [Category("Appearance")]
        public Color MetroColor
        {
            get
            {
                return metroColor;
            }
            set
            {
                if (value != metroColor)
                {
                    metroColor = value;
                    if (m_Renderer != null && m_Renderer is DockingManagerRenderer && this.VisualStyle == Forms.VisualStyle.Metro)
                    {
                        (m_Renderer as DockingManagerRenderer).MetroColor = this.MetroColor;
                    }
                }
            }
        }
		/// <summary>
		/// Gets or sets color theme for Office2007-like visual styles.
		/// </summary>
		[
		Category("Appearance"),
		Description("Color theme for Office2007-like visual styles."),
		DefaultValue(Office2007Theme.Blue)
		]
		public Office2007Theme Office2007Theme
		{
			get { return m_Office2007Theme; }
			set
			{
				if ( m_Office2007Theme != value || value == Office2007Theme.Managed )
				{
					m_Office2007Theme = value;

					if( m_Renderer.VisualStyle == VisualStyle.Office2007 
						|| m_Renderer.VisualStyle == VisualStyle.Office2007Outlook )
					{
						Renderer.RefreshOffice2007Theme( value );
						UpdateControllers( );

						if( !m_bCustomMdiColor )
							Office2007MdiColorScheme = value;
					}
				}
			}
		}


        protected Office2010Theme m_Office2010Theme = Office2010Theme.Blue;

        /// <summary>
        /// Gets or sets color theme for Office2010-like visual styles.
        /// </summary>
        [
        Category("Appearance"),
        Description("Color theme for Office2010-like visual styles."),
        DefaultValue(Office2010Theme.Blue)
        ]
        public Office2010Theme Office2010Theme
        {
            get { return m_Office2010Theme; }
            set
            {
                if (m_Office2010Theme != value || m_Office2010Theme == Forms.Office2010Theme.Managed)
                {
                    m_Office2010Theme = value;

                    if (m_Renderer.VisualStyle == VisualStyle.Office2010)
                    {
                        Renderer.RefreshOffice2010Theme(value);
                        UpdateControllers();
                    }
                }
            }
        }

		/// <summary>
		/// Gets or sets the imagelist containing the image objects used by the dockable controls.
		/// </summary>
		/// <value>A <see cref="System.Windows.Forms.ImageList"/> containing the images associated with the various docking windows.</value>
		[
		DefaultValue(null),
		Category("Behavior"),
		Description("The imagelist associated with the DockingManager.")
		]
		public ImageList ImageList
		{
			get { return this.ilDockTabs; }
			set
			{
				this.ilDockTabs = value;
				UpdateControllers();
				OnImageListChanged(EventArgs.Empty);
			}
		}
		#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

        private int m_interval = 5000;
        /// <summary>
        /// Gets or sets the tooltipintervel for DockingManger using tooltip.
        /// </summary>
        /// <value>A <see cref="Syncfusion.Windows.Forms.Tools.SuperToolTip"/> instance.</value>
        [
        DefaultValue(5000),
        Category("Behavior"),
        Description("Gets or sets the tooltipintervel for DockingManger using tooltip.")
        ]        
        public int ToolTipInterval
        {
            get { return this.m_interval; }
            set
            {
                if (this.m_interval != value && value >= 0)
                {
                    this.m_interval = value;
                }
            }
        }

        private bool useBalloonStyleToolTip = false;
        /// <summary>
        /// Gets or sets the Ballon style for DockingManger using tooltip.
        /// </summary>
        /// <value>A <see cref="Syncfusion.Windows.Forms.Tools.SuperToolTip"/> instance.</value>
        [
        DefaultValue(false),
        Category("Behavior"),
        Description("Gets or sets the Ballon style for DockingManger using tooltip.")
        ]
        public bool UseBalloonStyleToolTip
        {
            get { return this.useBalloonStyleToolTip; }
            set
            {
                if (this.useBalloonStyleToolTip != value)
                {
                    this.useBalloonStyleToolTip = value;
                }
            }
        }

		private SuperToolTip m_toolTip = null;
		/// <summary>
		/// Gets or sets the tooltip used by the dockable controls.
		/// </summary>
		/// <value>A <see cref="Syncfusion.Windows.Forms.Tools.SuperToolTip"/> instance.</value>
		[
		DefaultValue(null),
		Category("Behavior"),
		Description("The supertooltip associated with the DockingManager.")		
		]
		[TypeConverter(typeof(ReferenceConverter))]
		public SuperToolTip SuperToolTip
		{
			get { return this.m_toolTip; }
			set
			{
				if( this.m_toolTip != value )
				{
					this.m_toolTip = value;
				}
			}
		}
		#endif

		/// <summary>
		/// Indicates whether controls will provide their own images.
		/// </summary>
		/// <value>A boolean value; the default is FALSE.</value>
		/// <remarks>
		/// Setting the ControlScopeImages property to TRUE denotes that dockable controls will furnish the actual Image
		/// objects during initialization using the <see cref="DockingManager.SetDockIcon"/> overload that accepts an Icon
		/// parameter and these images will be bound to the control's lifetime as a docking window. This contrasts with the
		/// default implementation where the <see cref="DockingManager"/> references an <see cref="System.Drawing.ImageList"/>
		/// object and controls merely provide the index to an Image in the ImageList.
		/// </remarks>
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		Description("Gets or sets a value indicating whether controls will provide their own images.")
		]
		public bool ControlScopeImages
		{
			get { return this.bControlScopeImages; }
			set
			{
				if(this.bControlScopeImages != value)
					this.bControlScopeImages = value;
			}
		}

		/// <summary>
		/// Determines whether to show images in captions of docked controls and floating forms.
		/// </summary>
		[
		Category("Appearance-Caption"),
		Description("Determines whether to show images in captions of docked controls and floating forms."),
		DefaultValue(true)
		]
		public bool ShowCaptionImages
		{
			get 
			{
				return m_ShowImages;
			}
			set
			{
				if( m_ShowImages != value )
				{
					m_ShowImages = value;
					for( int i = 0; i< alDockAreaControllers.Count; i++ )
					{
						DockHostController dhc = alDockAreaControllers[i] as DockHostController;
						if( dhc != null )
						{
							dhc.HostControl.Invalidate();
							continue;
						}
						FloatingFormController ffc = alDockAreaControllers[i] as FloatingFormController;
						if( ffc != null )
						{
							NativeMethodsHelper.RedrawWindow( ffc.HostControl.Handle, NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE );
						}
					}
				}
			}
		}

        /// <summary>
		/// Gets or sets if to display scroll button on DockTabControl.
        /// </summary>
        [
        Category("Appearance"),
        Description("Determines whether to show scroll buttons in the DockTabbed window or not when the tab size will be exceeded than the docked window width."),
        DefaultValue(false)
        ]
        public bool ShowDockTabScrollButton
        {
            get
            {
                return showDockTabScrollButton;
            }
            set
            {
                if (showDockTabScrollButton != value)
                {
                    showDockTabScrollButton = value;
                    this.UpdateControllers();
				}
			}
		}

        /// <summary>
        /// Determines whether to show icons in AutoHide context menu.
        /// </summary>
        [
        Category("Behavior"),
        Description("Determines whether to show icons in AutoHide context menu."),
        DefaultValue(true)
        ]
        public bool ShowIconInAutoHideContextMenu
        {
            get
            {
                return showIconInAutoHideContextMenu;
            }

            set
            {
                if (showIconInAutoHideContextMenu != value)
                    showIconInAutoHideContextMenu = value;
            }
        }

		/// <summary>
		/// Indicates the visibility state for docking panel button's tooltip.
		/// </summary>
		[DefaultValue(true), Category("Behavior"),
        Description("Indicates the visibility state for docking panel button's tooltip.")]
		public bool ShowToolTips
		{
			get { return showToolTips; }
			set { showToolTips = value; }
		}

        /// <summary>
        ///  Indicates whether the autohidden tabs can be dragged to make it float.
        /// </summary>
        /// <value>FALSE indicates that the dragging feature is disabled</value>
        /// <remarks>
        /// When this property is TRUE, all the autohidden tabs can be dragged to make them float.
        /// </remarks>
        [
        DefaultValue(false),
        Category("Behavior"),
        Description("Gets or sets a value indicating whether the autohidden tabs can be dragged to make it float.")
        ]
        public bool EnableDragAutoHiddenTabs
        {
            get { return bEnableDragAutoHiddenTabs; }
            set
            {
                if (bEnableDragAutoHiddenTabs != value)
                {
                    bEnableDragAutoHiddenTabs = value;
                }
            }
        }
		/// <summary>
		///  Indicates whether the autohide feature is enabled.
		/// </summary>
		/// <value>FALSE indicates that the autohide feature is disabled. Default is TRUE. </value>
		/// <remarks>
		/// When this property is TRUE, all docked windows will contain an autohide button that can be
		/// used to set/unset the particular control to/from the autohide mode.
		/// </remarks>
		[
		DefaultValue(true),
		Category("Behavior"),
		Description("Gets or sets a value indicating whether the autohide feature is enabled.")
		]
		public bool AutoHideEnabled
		{
			get { return this.bAutoHideEnabled; }
			set
			{
				if(this.bAutoHideEnabled != value)
				{
					this.bAutoHideEnabled = value;
					foreach(DockControllerBase dcbase in this.alDockAreaControllers)
					{
						if(dcbase is DockHostController)
						{
							DockHostController dhcHostController = dcbase as DockHostController;
							
							Control ctrlControl = dhcHostController.ctrlReference;
							
							DockHost dhHost = dhcHostController.HostControl as DockHost;
							if( null != dhHost )
							{
								if( 0 < dhHost.Controls.Count )
								{
									ctrlControl = dhHost.Controls[0];
								}
							}
							
							if( null != ctrlControl )
							{
								this.SetAutoHideButtonVisibility( ctrlControl, value );
							}
						}
					}
				}
			}
		}
		/// <summary>
		/// Specifies if the DockingManager is currently suspended.
		/// </summary>
		[Browsable(false)]
		public bool IsLayoutSuspended
		{
			get
			{
				return this.layoutSuspended;
			}
		}

		private bool forwardMenuShortcuts = true;
		/// <summary>
		/// Indicates whether the key combinations for menu shortcuts should be passed to the Host form
		/// </summary>
		/// <value>True indicates all the key combinations will pass to the HOST form. </value>
		[Browsable(true), DefaultValue(true),
        Description("Indicates whether the key combinations for menu shortcuts should be passed to the Host form")]
		public bool ForwardMenuShortcuts
		{
			get
			{
				return this.forwardMenuShortcuts;
			}

			set
			{
				this.forwardMenuShortcuts = value;
			}
		}
		/// <summary>
		///  Indicates whether the menu button is enabled.
		/// </summary>
		/// <value>FALSE indicates that the menu button is disabled. Default is TRUE. </value>
		/// <remarks>
		/// When this property is TRUE, all docked windows will contain the menu button that can be
		/// used to show context menu with dock/float/autohide functionalities
		/// </remarks>
		[
		DefaultValue(true),
		Category("Behavior"),
		Description("Gets or sets a value indicating whether the window position feature is enabled.")
		]
		public bool MenuButtonEnabled
		{
			get
			{
				return this.bMenuButtonEnabled;
			}
			set
			{
				if( this.bMenuButtonEnabled != value )
				{
					this.bMenuButtonEnabled = value;
					foreach(DockControllerBase dcb in alDockAreaControllers)
						if(dcb is DockHostController)
						{
							DockHost dh = dcb.HostControl as DockHost;
							SetMenuButtonVisibility(dh.Controls[0], value);
						}
				}
			}
		}

		/// <summary>
		/// Indicates whether the close button is present in docking windows.
		/// </summary>
		/// <value>FALSE indicates the close button is hidden. The default is TRUE.</value>
		/// <remarks>
		/// When this property is TRUE, all docking windows will contain a close button that can be
		/// used to hideItem the particular control.
		/// </remarks>
		[
		DefaultValue(true),
		Category("Behavior"),
		Description("Gets or sets a value indicating whether the close button is present in docking windows.")
		]
		public bool CloseEnabled
		{
			get { return this.bCloseEnabled; }
			set
			{
				if(this.bCloseEnabled != value)
				{
					this.bCloseEnabled = value;
					foreach(DockControllerBase dcbase in this.alDockAreaControllers)
					{
						if(dcbase is DockHostController)
						{
							DockHost dhost = dcbase.HostControl as DockHost;
							this.SetCloseButtonVisibility(dhost.Controls[0], value);
						}
					}
					foreach(FloatingFormController ffc in this.alFFControllers)
					{
						(ffc.HostControl as FloatingForm).CloseButtonVisibility = value;
						(ffc.HostControl as FloatingForm).UpdateControlBoxVisibility();
					}
				}
			}
		}
		/// <summary>
		/// Indicates whether the maximize button is present in docking windows.
		/// </summary>
		/// <value>FALSE indicates the maximize button is hidden. The default is TRUE.</value>
		/// <remarks>
		/// When this property is TRUE, all docking windows will contain a maximize button that can be
		/// used to maximize the particular control.
		/// </remarks>
		[
		DefaultValue(false),
		Category("Behavior"),
		Description("Gets or sets a value indicating whether the maximize button is present in docking windows.")
		]
		public bool MaximizeButtonEnabled
		{
			get { return this.bMaximizeEnabled; }
			set
			{
				if( this.bMaximizeEnabled != value )
				{
					this.bMaximizeEnabled = value;

					foreach( DockControllerBase dcbase in this.alDockAreaControllers )
					{
						DockHostController hostController = dcbase as DockHostController;
						if (hostController != null)
						{
							hostController.MaximizeButtonVisibility = value;
						}
					}
				}
			}
		}

        /// <summary>
        /// Gets or sets the caption buttons collection.
        /// </summary>        
        [Category("Appearance")]
        [Description("Gets or sets the caption buttons collection.")]
        [ DesignerSerializationVisibility( DesignerSerializationVisibility.Content ) ]
        public CaptionButtonsCollection CaptionButtons
        {
          get
          {
              return m_CaptionButtons;
          }
					set
					{
						if( m_CaptionButtons != value )
						{
							if( value == null )
							{
								throw new ArgumentNullException("CaptionButtons.");
							}
							if( m_CaptionButtons != null )
							{
								m_CaptionButtons.CollectionChanged -=new EventHandler(CaptionButtons_CollectionChanged);
								m_CaptionButtons.CollectionItemChanged -=new EventHandler(CaptionButtons_CollectionItemChanged);
							}
							m_CaptionButtons = value;
							m_CaptionButtons.CollectionChanged +=new EventHandler(CaptionButtons_CollectionChanged);
							m_CaptionButtons.CollectionItemChanged +=new EventHandler(CaptionButtons_CollectionItemChanged);
						}
					}
        }
		/// <summary>
		///  Gets or sets the alignment of DockLabels.
		/// </summary>
		[
		Category("Appearance"),
		Description("Determines the alignment of DockLabels."),
		DefaultValue(DockLabelAlignmentStyle.Default)
		]

		public DockLabelAlignmentStyle DockLabelAlignment
		{
			get
			{
				return this.dlAlignment;
			}
			set
			{
				if (this.dlAlignment != value)
				{
					this.dlAlignment = value;
					foreach (Control ctrl in alEnableDocking)
					{
						DockHost dh = ctrl.Parent as DockHost;
						if (dh != null)
						{
							dh.RefreshCaptionPainter();
							FloatingForm ffrm = dh.ParentForm as FloatingForm;
							if (ffrm != null)
							{
								// update floating form caption
								NativeMethodsHelper.RedrawWindow(ffrm.Handle, NativeMethods.RDW_INVALIDATE | NativeMethods.RDW_FRAME);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the Selection style of Auto Hide window.
		/// </summary>

		[
		Category("Behavior"),
		Description("Determines the selection style of the AutoHide window."),
		DefaultValue(AutoHideSelectionStyle.MouseHover)
		]

		public AutoHideSelectionStyle AutoHideSelectionStyle
		{
			get
			{
				return autoHideSelection;
			}
			set
			{
				autoHideSelection = value;
			}
		}

		/// <summary>
		/// Gets or sets the alignment of tabs in tab groups.
		/// </summary>
		[
		Category("Appearance"),
		Description("Determines the alignment of tabs in tab groups."),
		DefaultValue(DockTabAlignmentStyle.Bottom)
		]
		public DockTabAlignmentStyle DockTabAlignment
		{
			get
			{
				return dtAlignment;
			}
			set
			{
				if (dtAlignment != value)
					dtAlignment = value;
				foreach (Control ctrl in alEnableDocking)
				{
					DockHostController dhc = GetDockHostController(ctrl);
					if (dhc != null)
					{
						DockTabController dtc = dhc.ParentController as DockTabController;
                        if (dtc != null)
                        {
                            dtc.AdjustLayout();
						}
						dhc.HostControl.Invalidate();
					}
				}
			}
		}

        private void RefreshControlSize(Control ctrl, Size newSize)
        {
            this.SetControlSize(ctrl, newSize);
        }
		/// <summary>
		///  Indicates whether a context menu is displayed.
		/// </summary>
		/// <value>FALSE indicates that the context menu is not displayed. Default is TRUE.</value>
		/// <remarks> When this property is true, clicking the right mouse button over the caption area of a
		/// docking window will display a context menu. The menu can be tailored by handling
		/// the <see cref="DockingManager.DockContextMenu"/> event.
		/// </remarks>
		[
		DefaultValue(true),
		Category("Behavior"),
		Description("Gets or sets a value indicating whether a context menu is displayed.")
		]
		public bool EnableContextMenu
		{
			get { return this.bEnableContextMenu; }
			set { this.bEnableContextMenu = value; }
		}
		/// <summary>
		///  Indicates whether a AutoHideTab context menu is displayed.
		/// </summary>
		/// <value>FALSE indicates that the AutoHideTab context menu is not displayed. Default is TRUE.</value>
		/// <remarks> When this property is true, clicking the right mouse button over the AutoHideTab
		///  will display a context menu. The menu can be tailored by handling
		/// the <see cref="DockingManager.AutoHideTabContextMenu"/> event.
		/// </remarks>
		[
		DefaultValue(true),
		Category("Behavior"),
		Description("Gets or sets a value indicating whether a AutoHideTab context menu is displayed.")
		]
		public bool EnableAutoHideTabContextMenu
		{
			get { return m_showAutohidetabContextMenu; }
			set
			{
				if (value != m_showAutohidetabContextMenu)
					m_showAutohidetabContextMenu = value;
			}
		}
		/// <summary>
		///  Indicates whether a border is drawn around the host form's client rectangle.
		/// </summary>
		/// <value>FALSE indicates that the border is hidden. The default is TRUE.</value>
		/// <remarks>When this property is TRUE, the <see cref="DockingManager.HostForm"/>'s available client
		/// rectangle is enveloped by a single-line border. The border will not be drawn if the form is
		/// an MDIContainer or if it contains a <see cref="DockingClientPanel"/> control.
		/// </remarks>
		[
		DefaultValue(true),
		Category("Appearance"),
		Description("Gets or sets a value indicating whether a border is drawn around the host form's client rectangle.")
		]
		public bool HostFormClientBorder
		{
			get { return this.bFormClientBorder; }
			set
			{
				if(this.bFormClientBorder != value)
				{
					this.bFormClientBorder = value;
					if(this.HostControl != null)
						this.HostControl.Invalidate(false);
				}
			}
		}

		/// <summary>
		/// Indicates whether to paint docked control's borders.
		/// </summary>
		[
		Category("Appearance"),
		Description("Determines whether to paint docked control's borders"),
		DefaultValue(true)
		]
		public bool PaintBorders
		{
			get { return this.m_paintBorders; }
			set
			{
				if( this.m_paintBorders != value )
				{
					this.m_paintBorders = value;
					InvalidateDockControllers();
				}
			}
		}

		/// <summary>
		/// Gets or sets the border color of docked controls.
		/// </summary>
		[
		Category ("Appearance"),
        Description("Determines the border color of docked controls"),
		DefaultValueAttribute(typeof(Color), "ControlDark")
		]
		public Color BorderColor
		{
			get
			{
				return this.m_borderColor;
			}
			set
			{
				if( this.m_borderColor != value )
				{				
					this.m_borderColor = value;
					//Repaint all docked control's frames with
					//the selected color:
					DockHost.UpdatePenColor( value );
                    if(m_Renderer != null)
                        (m_Renderer as DockingManagerRenderer).MetroBorderColor = m_borderColor;
					InvalidateDockControllers();
				}
			}
		}
        /// <summary>
        /// Information about the brush using which the caption background is going to painted
        /// </summary>
        [Description("Information about the brush using which the caption background is going to painted"),
        Category("Appearance-Caption"),
     DefaultValueAttribute(typeof(BrushInfo), "Solid; ActiveCaption"),
        
        ]

		public BrushInfo ActiveCaptionBackground
		{
			get
			{
				return bIActiveCaptionBackround;
			}
			set
			{
				if (bIActiveCaptionBackround != value)
				{
					bIActiveCaptionBackround = value;

					foreach (DockControllerBase dcbase in this.alDockAreaControllers)
					{
						if (dcbase is DockHostController)
						{
							(dcbase.HostControl as DockHost).Invalidate();
						}
					}
				}
			}
		}
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        /// <summary>
        /// Gets or sets if to enable super tool tip for dock caption buttons.
        /// </summary>
        [Description("Gets or sets if to enable SuperToolTip for dock caption buttons"),
        Category("Appearance-Caption"),
        DefaultValueAttribute(typeof(bool), "false"),
        ]
		public bool EnableSuperToolTip
		{
			get
			{
				return m_enableSuperToolTip;
			}
			set
			{
				if( m_enableSuperToolTip != value )
				{
					m_enableSuperToolTip = value;
				}
			}
		}
#endif
        /// <summary>
        /// Information about the brush using which the caption background is going to painted when the docked control is in inactive state.
        /// </summary>
        [
        Description("Information about the brush using which the caption background is going to painted when the docked control is in inactive state."),
        Category("Appearance-Caption"),
        DefaultValueAttribute(typeof(BrushInfo), "Solid; Control"),
        
        ]
        

		public BrushInfo InActiveCaptionBackground
		{
			get
			{
				return bIInActiveCaptionBackround;
			}
			set
			{
				if (bIInActiveCaptionBackround != value)
				{
					bIInActiveCaptionBackround = value;

					foreach (DockControllerBase dcbase in this.alDockAreaControllers)
					{
						if (dcbase is DockHostController)
						{
							(dcbase.HostControl as DockHost).Invalidate();
						}
					}
				}
			}
		}
        /// <summary>
        /// Color of the caption text in active state.
        /// </summary>
        [Description("Color of the caption text in active state."),
        Category("Appearance-Caption"),
      DefaultValueAttribute(typeof(Color), "ActiveCaptionText")]

        public Color ActiveCaptionForeGround
		{
			get
			{
				return cActiveCaptionForeGround;
			}
			set
			{
				if (cActiveCaptionForeGround != value)
				{
					cActiveCaptionForeGround = value;
					foreach (DockControllerBase dcbase in this.alDockAreaControllers)
					{
						if (dcbase is DockHostController)
						{
							(dcbase.HostControl as DockHost).Invalidate();
						}
					}
                
				}
			}
		}
        private Color inActiveCaptionForeColor = SystemColors.ControlText;
        /// <summary>
        /// Color of the caption button in inactive state.
        /// </summary>
        [Description("Color of the caption button in inactive state."),
        Category("Appearance-Caption"),
        DefaultValueAttribute(typeof(Color), "ControlText")]
        public Color InActiveCaptionButtonForeColor
        {
            get { return inActiveCaptionForeColor; }
            set
            {
                if (inActiveCaptionForeColor != value)
                {
                    inActiveCaptionForeColor = value;
                }
            }
        }
        private Color activeCaptionForeColor = SystemColors.ActiveCaptionText;
        /// <summary>
        /// Color of the caption button in active state.
        /// </summary>
        [Description("Color of the caption button in active state."),
        Category("Appearance-Caption"),
        DefaultValueAttribute(typeof(Color), "ActiveCaptionText")]
        public Color ActiveCaptionButtonForeColor
        {
            get { return activeCaptionForeColor; }
            set
            {
                if (activeCaptionForeColor != value)
                {
                    activeCaptionForeColor = value;
                }
            }
        }
        /// <summary>
        /// Color of the caption text in inactive state.
        /// </summary>
        [Description("Color of the caption text in inactive state."),
        Category("Appearance-Caption"),
      DefaultValueAttribute(typeof(Color), "ControlText")]

		public Color InActiveCaptionForeGround
		{
			get
			{
				return cInActiveCaptionForeGround ;
			}
			set
			{
				if (cInActiveCaptionForeGround  != value)
				{
					cInActiveCaptionForeGround  = value;
					foreach (DockControllerBase dcbase in this.alDockAreaControllers)
					{
						if (dcbase is DockHostController)
						{
							(dcbase.HostControl as DockHost).Invalidate();
						}
					}

				}
			}
		}

        /// <summary>
        /// Gets or sets the Font of the active caption.
        /// </summary>
        [
        Description("Gets or sets the Font of the active caption."),
        Category("Appearance-Caption"),
        DefaultValueAttribute(typeof(Font), "Tahoma, 11world")
        ]
		public Font ActiveCaptionFont
		{
			get
			{
				return fntActiveCaptionFont;
			}
			set
			{
				if (fntActiveCaptionFont != value)
				{
					fntActiveCaptionFont = value;
					foreach (DockControllerBase dcbase in this.alDockAreaControllers)
					{
						if (dcbase is DockHostController)
						{
							(dcbase.HostControl as DockHost).Invalidate();
						}
					}
				}
			}
		}
        /// <summary>
        /// Gets or sets the font of the inactive caption.
        /// </summary>
        [Description("Gets or sets the font of the inactive caption."),
        Category("Appearance-Caption"),
      DefaultValueAttribute(typeof(Font), "Tahoma, 11world")
       
        ]

        public Font InActiveCaptionFont
		{
			get
			{
				return fntInActiveCaptionFont ;
			}
			set
			{
				if (fntInActiveCaptionFont  != value)
				{
					fntInActiveCaptionFont  = value;
					foreach (DockControllerBase dcbase in this.alDockAreaControllers)
					{
						if (dcbase is DockHostController)
						{
							(dcbase.HostControl as DockHost).Invalidate();
						}
					}
				}
			}
		}



        /// <summary>
        /// Gets or sets the value of the key, which can be used to tab through the docked controls.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// dockingManager1.BrowsingKey = Keys.F10;
        /// dockingManager1.BrowsingKey=((System.Windows.Forms.Keys)(Enum.Parse(typeof(Keys), "F12, Shift, Control"))); //This will set Ctrl+Shift+F12 as browsing key
        /// </code>
        ///<code lang="VB">
        /// DockingManager1.BrowsingKey = Keys.F10
        /// Me.DockingManager1.BrowsingKey = CType(Enum.Parse(typeof(Keys), "F12, Shift, Control"),System.Windows.Forms.Keys) 'This will set Ctrl+Shift+F12 as browsing key
        /// </code> 
        ///</example>
        [
        Category("Behavior"),
        Description("Determines the value of the key, which can be used to tab through the docked controls"),
        DefaultValue(Keys.None)
        ]
		public Keys BrowsingKey
		{
			get
			{
				return browsingKey;
			}
			set
			{
				if (this.browsingKey != value)
					this.browsingKey = value;
			}
		}

		/// <summary>
		/// Resets the <see cref="DockingManager.CaptionButtons"/> value.
		/// </summary>
		public void ResetCaptionButtons()
		{
			CaptionButtonsCollection cbDefault = GetDefaultCaptionButtons();
			this.CaptionButtons.Clear();
			this.CaptionButtons.MergeWith( cbDefault, true );
			SynchronizeCaptionButtons();
		}

		/// <summary>
		/// Resets the <see cref="DockingManager.BrowsingKey"/> value.
		/// </summary>
		public void ResetBrowsingKey()
		{
			browsingKey = Keys.None;
		}

		/// <summary>
		/// Returns an enumerator that can iterate through the list of dockable controls.
		/// </summary>
		/// <value>A <see cref="System.Collections.IEnumerator"/> for the control list.</value>
		/// <example>
		/// This example shows how to get the collection of controls
		/// <code lang="C#">
		/// IEnumerator ienum = this.dockingManager1.Controls; 	
		/// ArrayList dockedctrls = new ArrayList(); 	
		/// while(ienum.MoveNext()) 	
		///     dockedctrls.Add(ienum.Current); 	
		/// foreach(Control ctrl in dockedctrls) 	
		///     Console.WriteLine(ctrl.ToString()); 	
		/// </code>
		/// <code lang="VB">	
		/// Dim ienum As IEnumerator = Me.dockingManager1.Controls 	
		/// Dim dockedctrls As ArrayList = New ArrayList() 	
		/// Do While ienum.MoveNext() 	
		///     dockedctrls.Add(ienum.Current) 	
		/// Loop	
		/// Dim ctrl As Control	
		/// For Each ctrl In dockedctrls	
		/// Console.WriteLine(ctrl.ToString()) 	
		/// Next
		/// </code>
		/// </example>
		[
		Browsable(false),
		Description("Returns an enumerator that can iterate through the list of dockable controls."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public IEnumerator Controls
		{
			get 
			{
				ArrayList controls = GetControlsList();
				return controls.GetEnumerator();				
			}
		}

		/// <summary>
		/// Returns an array of dockable controls.
		/// </summary>
		[
		Browsable(false),
		Description("Returns an array of dockable controls."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Control[] ControlsArray
		{
			get 
			{
				ArrayList controls = GetControlsList();
				return controls.ToArray( typeof(Control) ) as Control[];				
			}
		}

		private ArrayList GetControlsList()
		{
			ArrayList controls = new ArrayList();
			GetControlsSequence(this.dcHostForm, controls);
			controls.Reverse();
			foreach( Control ctrl in this.alEnableDocking )
			{
				if( !controls.Contains(ctrl) )
					controls.Add(ctrl);
			}
			return controls;
		}

		private void GetControlsSequence( DockControllerBase controller, ArrayList controls )
		{			
			IEnumerator controlEnum = controller.ChildEnumerator;
			bool hostFirst = false;
			if( controlEnum != null )
			{
				controlEnum.Reset();
				while( controlEnum.MoveNext() )
				{
					if( controlEnum.Current is DockHostController )
					{
						hostFirst = true;
						controls.Add(( controlEnum.Current as DockHostController ).HostControl.Controls[0]);
					}
					else
					{
						if( controlEnum.Current is SizingController )
						{
							int i = controls.Count - 1;
							GetControlsSequence(controlEnum.Current as DockControllerBase, controls);
							if( hostFirst )
							{
								controls.Reverse(i, controls.Count - i);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Returns the last active docking window.
		/// </summary>
		/// <value>A <see cref="System.Windows.Forms.Control"/> value. Null if no window has been activated yet.</value>
		[
		Browsable(false),
		Description("Returns the last active docking window."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Control ActiveControl
		{
			get	{ return this.ctrlLastActive; }
		}

        /// <summary>
        /// Gets or Sets a value indicating whether custom buttons can be drawn in floating window
        /// </summary> 
        [
        Category("Appearance"),
        Description("Determines whether to show custom buttons in floating window"),
        DefaultValue(false)
        ]
        public bool ShowCustomButtonsInFloating
        {
            get
            {
                return this.showCustomButtonsInFloating;
            }
            set
            {
                if (this.showCustomButtonsInFloating != value)
                {
                    this.showCustomButtonsInFloating = value;
                    foreach (FloatingFormController ffc in this.alFFControllers)
                    {
                        ffc.RefreshFormCaption();                        
                    }
                }
            }
        }

        protected bool ShouldSerializeShowCustomButtonsInFloating()
        {
            return ShowCustomButtonsInFloating;
        }

        protected void ResetShowCustomButtonsInFloating()
        {
            this.ShowCustomButtonsInFloating = false;
        }

		/// <summary>
		/// Indicates whether docked control will occupy the form's full client region.
		/// </summary>
		/// <value>A boolean value; default is FALSE.</value>
		/// <remarks>
		/// When the DockToFill property is set to TRUE, controls are docked such that they occupy the
		/// host form's entire available client region.
		/// <p>
		/// The DockToFill option should not be set when the host form is an MDIContainer or if it contains an instance of the
		/// <see cref="DockingClientPanel"/> control.
		/// </p>
		/// </remarks>
		[
		DefaultValue(false),
		Category("Behavior"),
		Description("Gets or sets a value indicating whether docked controls will occupy the form's full client region.")
		]
		public bool DockToFill
		{
			get { return this.bDockToFill; }
			set
			{
				if(this.bDockToFill != value)
				{
					if(value == true)
					{
						if(this.HostControl != null)
						{
							if((this.HostControl is Form) && ((this.HostControl as Form).IsMdiContainer))
							{
								if(this.DesignMode == true)
								{
									MessageBox.Show("The DockToFill option is not valid when the host form is an MDIContainer.",
										"Essential Tools DockingManager", MessageBoxButtons.OK, MessageBoxIcon.Information);
								}
								else
								{
									Debug.Assert(false, "The DockToFill option may be used only with a non-MDIContainer form.");
								}
								return;
							}
							if((this.DesignMode == true) && (this.bDockToFill == false))
							{
								if((this.dcHostForm != null) && (this.dcHostForm.ChildCount > 0))
								{
									// If the first child of the MainFormController is not occupying the
									// full layout rect of the form, then a redocking is needed
									DockControllerBase dcchild = this.dcHostForm.GetChildAt(0);
									if(dcchild.LayoutRect != this.dcHostForm.LayoutRect)
										MessageBox.Show("Please undock and redock all docked windows so that the DockToFill layout may be correctly setup.",
											"Essential Tools DockingManager", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
								}
							}
						}
						this.MaxRedockFactor = 4;
					}
					else
					{
						if(this.dsDockFillAHBorder != DockingStyle.Fill)
							this.dsDockFillAHBorder = DockingStyle.Fill;
						this.MaxRedockFactor = 2;
					}
					this.bDockToFill = value;
				}
			}
		}

		/// <summary>
		/// Indicates whether the controls are allowed to be floated.
		/// </summary>
		/// <value>A boolean value. Default is FALSE.</value>
		/// <remarks>
		/// When the DisallowFloating property is set to TRUE, controls may be moved around
		/// and docked within the form or other dockable controls, but are not allowed to be floated.
		/// </remarks>
		[
		DefaultValue(false),
		Category("Behavior"),
		Description("Gets or sets a value indicating whether controls are allowed to be floated.")
		]
		public bool DisallowFloating
		{
			get	{ return this.bDisallowFloating; }
			set
			{
				if(this.bDisallowFloating != value)
				{
					if(value == true)
					{
						if((this.DesignMode == true) && (this.alFFControllers.Count > 0))
						{
							MessageBox.Show("Please dock all current floating windows.",
								"Essential Tools DockingManager", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
					}
					this.bDisallowFloating = value;
				}
			}
		}

		/// <summary>
		/// When docked control is in unpinned autohide state this value 
		/// indicates whether to slide back selected control.
		/// </summary>
		/// <value>A boolean value. Default is FALSE.</value>
		[
		DefaultValue(false),
		Category("Behavior"),
		Description("Gets or sets a value indicating whether to slide back selected autohidden control.")
		]
		public bool AutoHideActiveControl
		{
			get { return bAutoHideActiveControl; }
			set { bAutoHideActiveControl = value; }
		}

		/// <summary>
		/// Enables or disables the MDI child activation triggered floating control visibility.
		/// </summary>
		/// <value>A boolean value. Default is TRUE.</value>
		/// <remarks>
		/// When the MDIActivatedVisibility property is enabled floating controls associated with <see cref="DockingManager"/>s hosted in
		/// MDI child forms will be shown only when the particular form is the active MDI child. When the MDI child loses activation
		/// all floating windows tied to the DockingManager will be hidden.
		/// <p>
		/// NOTE: This property applies only when the DockingManager is hosted, either directly or indirectly through a ContainerControl, in an MDI child form.
		/// </p>
		/// <seealso cref="HostActivatedVisibility"/>
		/// </remarks>
		[
		Browsable(false),
		Category("Behavior"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool MDIActivatedVisibility
		{
			get { return this.bMDIActivatedVisibility; }

			set
			{
				if(this.bMDIActivatedVisibility != value)
					this.bMDIActivatedVisibility = value;
			}
		}

		/// <summary>
		/// Indicates whether to bind floating control visibility state to the host control's visibility.
		/// </summary>
		/// <value>A boolean value. Default is TRUE.</value>
		/// <remarks>
		/// When the HostActivatedVisibility property is enabled floating controls associated with a <see cref="DockingManager"/>
		/// hosted in a ContainerControl will be shown only when the host control is visible. Hiding the host control will
		/// automatically hide all floating windows tied to that control.
		/// <p>
		/// NOTE: This property applies only when the DockingManager is hosted in a ContainerControl.
		/// </p>
		/// <seealso cref="DockingManager.MDIActivatedVisibility"/>
		/// </remarks>
		[
		Browsable(false),
		Category("Behavior"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool HostActivatedVisibility
		{
			get { return this.bHostActivatedVisibility; }

			set
			{
				if(this.bHostActivatedVisibility != value)
					this.bHostActivatedVisibility = value;
			}
		}

		/// <summary>
		/// Indicates whether XP Themes(visual styles) should be used for the docking windows.
		/// </summary>
		/// <value>True to turn on themes; false otherwise.</value>
		[
		DefaultValue(false),
		Category(@"Appearance"),
		Description("Specifies whether XP Themes(visual styles) should be used for the docking windows.")
		]
		public bool ThemesEnabled
		{
			get{return this.bThemesEnabled;}
			set
			{
				if(this.bThemesEnabled != value)
				{
					this.bThemesEnabled = value;
					foreach(DockControllerBase dcbase in this.alDockAreaControllers)
					{
						if(dcbase is DockHostController)
						{
							DockHostController dhc = dcbase as DockHostController;
							if(dhc.DockVisibility == true)
							{
								if((dhc.ParentController != null) && (dhc.ParentController is DockTabController))
								{
									DockTabControl tabctrl = (dhc.ParentController as DockTabController).TabControl;
									tabctrl.TabStyle = tabctrl.GetRendererType();
									tabctrl.HotTrack = !tabctrl.HotTrack;

								}
								dhc.HostControl.Invalidate(false);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the interval between mouse movement across an autohide tab and showing or hiding the control.
		/// </summary>
		/// <value>An integer value specifying the time in milliseconds.</value>
		[
		DefaultValue(400),
		Category(@"Behavior"),
		Description("Specifies the interval for showing or hiding an autohidden control.")
		]
		public int AutoHideInterval
		{
			get { return DockingManager.nShowInterval; }

			set
			{
				if(DockingManager.nShowInterval != value)
				{
					DockingManager.nShowInterval = value;
					DockingManager.nHideInterval = value;
				}

				if( AHTabControl.tmrShowController != null &&
					AHTabControl.tmrShowController.Enabled != false )
				{
					AHTabControl.tmrShowController.Stop();
					AHTabControl.tmrShowController.Interval = DockingManager.nShowInterval;
					AHTabControl.tmrShowController.Start();
				}

				if( AHTabControl.tmrHideController != null &&
					AHTabControl.tmrHideController.Enabled != false )
				{
					AHTabControl.tmrHideController.Stop();
					AHTabControl.tmrHideController.Interval = DockingManager.nHideInterval;
					AHTabControl.tmrHideController.Start();
				}
			}
		}

		/// <summary>
		/// Gets or sets the width of splitters between docking windows.
		/// </summary>
		/// <value>Integer value between 0 and 30. Default value is 4. </value>
		[ 
		Category("Appearance"),
		DefaultValue(4),
		Description("Gets or sets a value indicating width of splitters between docking windows.")
		]
		public int SplitterWidth
		{
			get { return splitterWidth; }
			set 
			{
				if( ( value <= 30) && ( value >= 0) )
				{
					splitterWidth = value;
					ControllerSizeCalculator.SplitterWidth = value;
					ResizeSplitters( value );
					if( DesignMode && alEnableDocking.Count > 0 )
						UpdateDesigner();
				}
				else
				{
					throw new SplitterWidthException();
				}
			}
		}

		/// <summary>
		/// Indicates whether docked and floating windows can be resized using the medial splitters.
		/// </summary>
		/// <value>Resizing is disabled when TRUE. Default is FALSE.</value>
		[
		DefaultValue(false),
		Category(@"Behavior"),
		Description("Determines whether docked and floating windows can be resized."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false)
		]
		public bool FreezeResizing
		{
			get { return this.bFreezeResizing; }

			set
			{
				if(this.bFreezeResizing != value)
				{
					this.bFreezeResizing = value;

					if(this.HostControl != null)
					{
						// Depending on the value, set V/HSplit or the default Arrow cursor for the DragSplitters
						if(this.bFreezeResizing == true)
						{
							foreach(FloatingFormController ffc in this.alFFControllers)
							{
								FloatingForm fform = ffc.HostControl as FloatingForm;
								fform.FormBorderStyle = FormBorderStyle.FixedToolWindow;
								ffc.UpdateControl();
							}
						}
						else	// (bFreezeResizing == false)
						{
							foreach(FloatingFormController ffc in this.alFFControllers)
							{
								FloatingForm fform = ffc.HostControl as FloatingForm;
								fform.FormBorderStyle = FormBorderStyle.SizableToolWindow;
								ffc.UpdateControl();
							}
						}
					}
				}
			}
		}

		/// <summary>
		///  Indicates whether to paint panel's caption.
		/// </summary>
		/// <value>Boolean value. Default value is True. </value>
		///
		[
		DefaultValue(true),
		Category("Behavior"),
		Description("Specifies whether to paint panel's caption")
		]
		public bool ShowCaption
		{
			get { return m_ShowCaption; }
			set
			{
				m_ShowCaption = value;
				foreach(DockControllerBase dcb in alDockAreaControllers)
				{
					DockHostController hostController = dcb as DockHostController;
					if( hostController != null )
					{
						Control control = hostController.HostControl;

						// need to repaint panel's header.
						// workaroud. Do the same as in DockHostController.HideCaption
						control.Height += 1;
						control.Height -= 1;
					}
				}
			}
		}

		/// <summary>
		///  Indicates whether to show full autohide tabgroup's page caption.
		/// </summary>
		/// <value>Boolean value. Default value is FALSE. </value>
		///
		[
		DefaultValue(false),
		Category("Behavior"),
		Description("Specifies whether to show full autohide tabgroup's page caption.")
		]
		public bool FullCaptionsInAutoHideMode
		{
			get
			{
				return m_FullCaptionsInAutoHideMode;
			}
			set
			{
				if( m_FullCaptionsInAutoHideMode != value )
				{
					m_FullCaptionsInAutoHideMode = value;

					if( this.dcHostForm != null )
					{
						this.dcHostForm.ahTabCtrlL.ForceLayout();
						this.dcHostForm.ahTabCtrlT.ForceLayout();
						this.dcHostForm.ahTabCtrlB.ForceLayout();
						this.dcHostForm.ahTabCtrlR.ForceLayout();
					}
				}
			}
		}

		/// <summary>
		///  Gets or sets the height of the tab control used in tabbed docking groups.
		/// </summary>
		/// <value>An integer value.</value>
		[
		Category("Appearance"),
		Description("Gets or sets the height of the tab control used in tabbed docking groups.")
		]
		public int DockTabHeight
		{
			get	{ return this.nDockTabHeight; }
			set
			{
				if(this.nDockTabHeight != value)
				{
					this.nDockTabHeight = value;
					foreach(DockControllerBase dcbase in this.alDockAreaControllers)
					{
						if((dcbase is DockHostController) && (dcbase.ParentController is DockTabController))
						{
							DockTabController tabcontroller = dcbase.ParentController as DockTabController;
							tabcontroller.TabControl.ItemSize = new Size(0, this.nDockTabHeight);
							dcbase.ParentController.AdjustLayout();
						}
					}
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeDockTabHeight()
		{
			return (this.nDockTabHeight != (DockingManager.ftSysInfoMenuFont.Height+8));
		}

		/// <summary>
		/// Resets the <see cref="DockingManager.DockTabHeight"/> property to it's default value.
		/// </summary>
		public void ResetDockTabHeight()
		{
			this.DockTabHeight = DockingManager.ftSysInfoMenuFont.Height+8;
		}

        private DockBehavior m_dockbehavior = DockBehavior.VS2008;
        /// <summary>
        /// Gets/Sets Docking behavior of Docking Manager
        /// </summary>
        /// 
        [
        DefaultValue(DockBehavior.VS2008),
		Category("Behavior"),
        Description("Gets or sets a value indicating Docking behavior of DockingManager")
		]
        public DockBehavior DockBehavior
        {
            get 
            { 
                return m_dockbehavior; 
            }
            set 
            {
                if (m_dockbehavior != value)
                {
                    m_dockbehavior = value;
                    this.UpdateControllers();
                }
            }
        }
        

		/// <summary>
		///  Gets or sets the Font for the tab control used in tabbed docking groups.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Font"/> value.</value>
		[
		Category("Appearance"),
		Description("Gets or sets the Font for the tab control used in tabbed docking groups.")
		]
		public Font DockTabFont
		{
			get { return this.ftDockTab; }
			set
			{
				if(this.ftDockTab != value)
				{
					this.ftDockTab = value;
					foreach(DockControllerBase dcbase in this.alDockAreaControllers)
					{
						if((dcbase is DockHostController) && (dcbase.ParentController is DockTabController))
						{
							DockTabControl tabcontrol = (dcbase.ParentController as DockTabController).TabControl;
							if(tabcontrol.Font != this.ftDockTab)
							{
								tabcontrol.Font = this.ftDockTab;
								tabcontrol.ActiveTabFont = this.ftDockTab;
							}
						}
					}
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeDockTabFont()
		{
			return (this.ftDockTab != DockingManager.ftSysInfoMenuFont);
		}

		/// <summary>
		/// Resets the <see cref="DockingManager.DockTabFont"/> property to it's default value.
		/// </summary>
		public void ResetDockTabFont()
		{
			this.DockTabFont = DockingManager.ftSysInfoMenuFont;
		}
        
        /// <summary>
        /// Gets or sets a value indicating whether to reduce flickering in RTL mode on startup.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if reduce flickering in RTL mode; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        public bool ReduceFlickeringInRtl
        {
            get
            {
                return reduceFlicker;
            }
            set
            {
                if(reduceFlicker != value)
                    reduceFlicker = value;
            }
        }

		/// <summary>
		///  Gets or sets the height of the autohide tab control.
		/// </summary>
		/// <value>An integer value.</value>
		[
		Category("Appearance"),
		Description("Gets or sets the height of the autohide tab control.")
		]
		public int AutoHideTabHeight
		{
			get { return this.nAutoHideTabHeight; }
			set
			{
				if(this.nAutoHideTabHeight != value)
				{
					this.nAutoHideTabHeight = value;
					if(this.dcHostForm != null)
					{
						AHTabControl ahtab = this.dcHostForm.GetAHTabControl(DockingStyle.Left);
						ahtab.ItemSize = new Size(0, this.nAutoHideTabHeight);
						ahtab = this.dcHostForm.GetAHTabControl(DockingStyle.Top);
						ahtab.ItemSize = new Size(0, this.nAutoHideTabHeight);
						ahtab = this.dcHostForm.GetAHTabControl(DockingStyle.Right);
						ahtab.ItemSize = new Size(0, this.nAutoHideTabHeight);
						ahtab = this.dcHostForm.GetAHTabControl(DockingStyle.Bottom);
						ahtab.ItemSize = new Size(0, this.nAutoHideTabHeight);

						this.RecalcHostFormLayout();
					}
				}
			}
		}

		/// <summary>
		/// Determines whether allow to move tabs inside DockTabControl.
		/// </summary>
		[
		Category("Behavior"),
		Description("Determines whether allow to move tabs inside DockTabControl."),
		DefaultValue(true)
		]
		public bool AllowTabsMoving
		{
			get
			{
				return bAllowTabsMoving;
			}
			set
			{
				if( bAllowTabsMoving != value )
				{
					bAllowTabsMoving = value;
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeAutoHideTabHeight()
		{
			return (this.nAutoHideTabHeight != (DockingManager.ftSysInfoMenuFont.Height+8));
		}

		/// <summary>
		/// Resets the <see cref="DockingManager.AutoHideTabHeight"/> property to it's default value.
		/// </summary>
		public void ResetAutoHideTabHeight()
		{
			this.AutoHideTabHeight = DockingManager.ftSysInfoMenuFont.Height+8;
		}

		/// <summary>
		///  Gets or sets the font for the autohide tab control.
		/// </summary>
		/// <value>An integer value.</value>
		[
		Category("Appearance"),
		Description("Gets or sets the font for the autohide tab control.")
		]
		public Font AutoHideTabFont
		{
			get { return this.ftAutoHideTab; }
			set
			{
				if(this.ftAutoHideTab != value)
				{
					this.ftAutoHideTab = value;
					if(this.dcHostForm != null)
					{
						AHTabControl ahtab = this.dcHostForm.GetAHTabControl(DockingStyle.Left);
						ahtab.Font = this.ftAutoHideTab;
						ahtab.ActiveTabFont = this.ftAutoHideTab;
						ahtab = this.dcHostForm.GetAHTabControl(DockingStyle.Top);
						ahtab.Font = this.ftAutoHideTab;
						ahtab.ActiveTabFont = this.ftAutoHideTab;
						ahtab = this.dcHostForm.GetAHTabControl(DockingStyle.Right);
						ahtab.Font = this.ftAutoHideTab;
						ahtab.ActiveTabFont = this.ftAutoHideTab;
						ahtab = this.dcHostForm.GetAHTabControl(DockingStyle.Bottom);
						ahtab.Font = this.ftAutoHideTab;
						ahtab.ActiveTabFont = this.ftAutoHideTab;

						this.RecalcHostFormLayout();
					}
				}
			}
		}
        bool ShouldSerializeInActiveCaptionForeColor()
        {
            return InActiveCaptionButtonForeColor != SystemColors.ControlText;
        }
        bool ShouldSerializeActiveCaptionForeColor()
        {
            return ActiveCaptionButtonForeColor != SystemColors.ActiveCaptionText;
        }
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeAutoHideTabFont()
		{
			return (this.ftAutoHideTab != DockingManager.ftSysInfoMenuFont);
		}

        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeDockBehavior()
        {
            return (this.DockBehavior != Tools.DockBehavior.VS2008);
        }

		/// <summary>
		/// Resets the <see cref="DockingManager.AutoHideTabFont"/> property to it's default value.
		/// </summary>
		public void ResetAutoHideTabFont()
		{
			this.AutoHideTabFont = DockingManager.ftSysInfoMenuFont;
		}

        /// <summary>
        /// Resets the <see cref="DockingManager.DockBehavior"/> property to it's default value.
        /// </summary>
        public void ResetDockBehavior()
        {
            this.DockBehavior = Tools.DockBehavior.VS2008;
        }
        
        /// <summary>
        /// Resets the <see cref="DockingManager.DockBehavior"/> CaptionButtonForeColor to it's default value.
        /// </summary>
        public void ResetInAvtiveCaptionButtonForeColor()
        {
            this.InActiveCaptionButtonForeColor = SystemColors.ControlText;
        }
        /// <summary>
        /// Resets the <see cref="DockingManager.DockBehavior"/> CaptionButtonForeColor to it's default value.
        /// </summary>
        public void ResetActiveCaptionButtonForeColor()
        {
            this.ActiveCaptionButtonForeColor = SystemColors.ActiveCaptionText;
        }
		/// <summary>
		/// Returns the current RTL setting based on the host control's setting.
		/// </summary>
		[
		Category("Appearance"),
		Description("Returns the current RTL setting based on the host control's setting."),
		]
		public RightToLeft RightToLeft
		{
			get
			{
				RightToLeft rtl = RightToLeft.No;
				Control ctrl = this.HostControl;
				if (null != ctrl)
				{
					rtl = ctrl.RightToLeft;
				}

				return rtl;
			}
		}
		/// <summary>
		/// Enable or disable the firing of DragFeedback events upon dragging splitters.
		/// </summary>
		[
		Category("Behavior"),
		Description("Determines whether to rise DragFeedback events upon resizing / dragging splitters."),
		DefaultValue(true)
		]
		public bool DragFeedbackEventsOnSplitters
		{
			get
			{
				return m_DragFeedbackEventsOnSplitters;
			}
			set
			{
				if (value != m_DragFeedbackEventsOnSplitters)
					m_DragFeedbackEventsOnSplitters = value;
			}
		}
        
		/// <summary>
		/// Enable or disable the state transition upon double click on caption
		/// </summary>
		[
		Category("Behavior"),
		Description("Determines whether to perform state transition upon double clicking on caption."),
		DefaultValue(true)
		]
		public bool EnableDoubleClickOnCaption
		{
			get
			{
				return m_EnableDoubleClickOnCaption;
			}
			set
			{
				if (value != m_EnableDoubleClickOnCaption)
					m_EnableDoubleClickOnCaption = value;
			}
		}
		# endregion
# region Public methods

        /// <summary>
        /// Gets or sets whether PriorityController need to be set when in DockToFill mode.
        /// </summary>
        [
        Description("Gets or sets whether PriorityController need to be set when in DockToFill mode."),
        DefaultValue(true)
        ]
        public bool NeedDTFPriorityController
        {
            get
            {
                return this.m_bNeedDTFPriorityController;
            }
            set
            {
                if (this.m_bNeedDTFPriorityController != value)
                    this.m_bNeedDTFPriorityController = value;
            }
        }
                
		/// <summary>
		/// Begins the initialization of the <see cref="DockingManager"/> component.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void BeginInit()
		{
			// If BeginInit/EndInit has already been called once as in a derived Form, then unsubscribe
			// from the previous handler.
			if (this.HostControl != null)
			{
				this.HostControl.Layout -= new System.Windows.Forms.LayoutEventHandler(this.HostControl_Layout);
				this.HostControl.HandleCreated -= new EventHandler(this.HostControl_HandleCreated);
			}
			this.bInBeginEndInit = true;
			this.bApplyMinMaxExtents = false;
		}
		/// <summary>
		/// Ends the initialization of the <see cref="DockingManager"/> component.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void EndInit()
		{
			// In DesignMode, the designer state loading takes place in the designer's IDesignerHost.LoadComplete handler.
			if (this.DesignMode == false)
			{
				if (HostControl.IsHandleCreated)
				{
					AssignNotifyWindow();
					if (alEnableDocking.Count > 0)
					{
						HostControl.Layout += new LayoutEventHandler(this.HostControl_Layout);
					}
				}
				else
				{
					HostControl.HandleCreated += new EventHandler(this.HostControl_HandleCreated);
					if (alEnableDocking.Count > 0)
					{
						bWaitOnLayoutEvent = true;
					}
				}
			}

			if( this.VisualStyle == VisualStyle.Office2007
					|| this.VisualStyle == VisualStyle.Office2007Outlook )
			{
				if( this.Office2007MdiChildForm )
					m_bCustomMdiStyle = false;
			}
			else
				if( !this.Office2007MdiChildForm )
					m_bCustomMdiStyle = false;

			this.bInBeginEndInit = false;
			this.bApplyMinMaxExtents = true;
			InitializeCaptionButtons();
			UpdateControllers();
		}

		/// <summary>
		/// Locks host form's updates.
		/// </summary>
		/// <example>
		/// This example shows how to avoid flickering during loading a dockstate
		/// <code lang="C#">
		/// dockingManager1.LockHostFormUpdate();
		/// dockingManager1.LoadDockState();
		/// dockingManager1.UnlockHostFormUpdate();
		/// </code>
		/// <code lang="VB">
		/// DockingManager1.LockHostFormUpdate()
		/// DockingManager1.LoadDockState()
		/// DockingManager1.UnlockHostFormUpdate()
		/// </code>
		///</example>
		public void LockHostFormUpdate()
		{
			if( HostForm != null )
			{
				if( m_lockCounter == 0 )
					NativeMethods.LockWindowUpdate( HostForm.Handle );

				m_lockCounter++;
			}
		}
		/// <summary>
		/// Unlocks host form's updates.
		/// </summary>
		public void UnlockHostFormUpdate()
		{
			if( HostForm != null )
			{
				if( m_lockCounter > 0 )
					m_lockCounter--;

				if( m_lockCounter == 0 )
					NativeMethods.LockWindowUpdate( IntPtr.Zero );
			}
		}
		/// <summary>
		/// Locks panel repainting.
		/// </summary>
		public void LockDockPanelsUpdate()
		{
			lockUpdates++;
			if (lockUpdates == 1)
			{
				NativeMethodsHelper.SuspendRedrawWindow(this.HostControl.Handle);
			}
		}
		/// <summary>
		/// Unlocks panel repainting.
		/// </summary>
		public void UnlockDockPanelsUpdate()
		{
			lockUpdates--;
			if (lockUpdates == 0)
			{
				foreach (DockControllerBase dockControllerBase in alDockAreaControllers)
				{
					DockHostController dockHostController = dockControllerBase as DockHostController;
					if (dockHostController != null)
					{
						DockHost dockHost = dockHostController.HostControl as DockHost;
						dockHost.UpdateControlSize();
					}
				}

				NativeMethodsHelper.ResumeRedrawWindow(this.HostControl.Handle, true);

				DockControllerBase dcb = alDockAreaControllers[0] as DockControllerBase;

				if (null != dcb)
				{
					dcb.AdjustLayout();
				}
			}

			if (lockUpdates < 0)
				lockUpdates = 0;
		}

      
		/// <summary>
		/// Call this so that the DockingManager will not attempt to layout the elements on the form
		/// when another action is taking place (like merging MDI children into the menus)
		/// </summary>
		/// <param name="suspendLayout">Specifies if the Hooks used by the DockingManager should be temporarily unhooked for the duration of the suspension.</param>
		public void SuspendLayout()
		{
			if (++this.m_nSuspendLayoutCount == 1)
			{
				layoutSuspended = true;
#if DEBUG
				Console.WriteLine("SuspendLayout called - Count = 1");
#endif
			}
#if DEBUG
			else
				Console.WriteLine("SuspendLayout ignored - Count = " + m_nSuspendLayoutCount.ToString());
#endif

		}
		/// <summary>
		/// Call this so that the DockingManager can continue to layout elements on the form (if it was previously suspended).
		/// </summary>
		public void ResumeLayout(bool bRefresh)
		{
			if (--this.m_nSuspendLayoutCount == 0)
			{
#if DEBUG
				Console.WriteLine("ResumeLayout called - Count = 0");
#endif

				layoutSuspended = false;

				if (bRefresh)
					this.RecalcHostFormLayout();

				Control mdiClient = GetMdiClient();
				if (mdiClient != null)
				{
					mdiClient.Refresh();
				}
			}
#if DEBUG
			else
				Console.WriteLine("ResumeLayout ignored - Count = " + m_nSuspendLayoutCount.ToString());
#endif

		}
		/// <summary>
		/// Call this so that the DockingManager can continue to layout elements on the form (if it was previously suspended).
		/// </summary>
		public void ResumeLayout()
		{
			this.ResumeLayout(true);
		}

        /// <summary>
        /// Suspends listening to system wide hooks.
        /// </summary>        
        [Syncfusion.Documentation.DocumentationExclude()]
		public void SuspendHooks()
		{
			if (DesignerHooks.CallWndProcListContains(this, m_curentThreadId))
			{
				DesignerHooks.RemoveCallWndProcListener(this, m_curentThreadId);
				unHookedCallWnd = true;
			}
			if (DesignerHooks.GetMsgProcListContains(this, m_curentThreadId))
			{
				DesignerHooks.RemoveGetMsgProcListener(this, m_curentThreadId);
				unHookedGetMsg = true;
			}
		}

        /// <summary>
        /// Resumes listening to system wide hooks.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
		public void ResumeHooks()
		{
			//Hooks
			if (unHookedCallWnd == true)
			{
				unHookedCallWnd = false;
				if (DesignerHooks.CallWndProcListContains(this) == false)
					m_curentThreadId = DesignerHooks.AddCallWndProcListener(this);
			}
			if (unHookedGetMsg == true)
			{
				unHookedGetMsg = false;
				if (DesignerHooks.GetMsgProcListContains(this) == false)
					DesignerHooks.AddGetMsgProcListener(this);
			}
		}

        //IGetMsgProcListener implementation

        /// <summary>
        /// IGetMsgProcListener implementation
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wparam"></param>
        /// <param name="lparam"></param>
        [Syncfusion.Documentation.DocumentationExclude()]			
		public void GetMsgProc(int nCode, IntPtr wparam, IntPtr lparam)
		{
			if ((this.HostControl == null) || (this.HostControl.IsHandleCreated == false))
				return;

			Message msg = (Message)(Marshal.PtrToStructure(lparam, typeof(Message)));
			if (this.DesignProcess == true)
			{
				if (msg.Msg == 0x0201 /*WM_LBUTTONDOWN*/)
				{
					// Display floating forms if the WM_LBUTTONDOWN occurs over the HostControl's region, on one of it's
					// child controls or on of the floating forms.
					bool visibility = false;
					if ((msg.HWnd == this.HostControl.Handle)
						|| (Syncfusion.Runtime.InteropServices.NativeMethods.IsChild(this.HostControl.Handle, msg.HWnd) == true))
					{
						visibility = true;
					}
					else
					{
						foreach (DockControllerBase dcb in this.alFFControllers)
						{
							if ((dcb.HostControl.Visible == true)
								&& ((msg.HWnd == dcb.HostControl.Handle) || (Syncfusion.Runtime.InteropServices.NativeMethods.IsChild(dcb.HostControl.Handle, msg.HWnd))))
							{
								visibility = true;
								break;
							}
						}
					}
					foreach (DockControllerBase dcb in this.alFFControllers)
					{
						if (dcb.HostControl.Visible != visibility)
							dcb.HostControl.Visible = visibility;
					}
				}
				else if ((msg.Msg >= 0x0203 /*WM_LBUTTONDBLCLK*/) && (msg.Msg <= 0x0209 /*WM_MBUTTONDBLCLK*/))
				{
					foreach (DockControllerBase dcb in this.alFFControllers)
					{
						if (dcb.HostControl.Visible != false)
							dcb.HostControl.Visible = false;
					}
				}
			}
			else
			{
				if( msg.Msg == NativeMethods.WM_LBUTTONUP )
				{
					FloatingForm floatForm = Control.FromHandle( msg.HWnd ) as FloatingForm;
					if( floatForm != null && !floatForm.InSizeMove && floatForm.Visible )
					{
						Point location = new Point( NativeMethods.LOWORD( msg.LParam )
							, NativeMethods.HIWORD( msg.LParam ) );
						floatForm.HandleMouseUp( MouseButtons.Left, location );
					}
                    if (this.VisualStyle ==VisualStyle.Metro && this.AHInViewTab != null)
                    {
                        Point mousepos = Cursor.Position;
                        if (!(this.DragProvider.DraggingControl is DragSplitter))
                        {
                            this.AHInViewTab.MouseClick = true;
                            this.AHInViewTab.MFMouseMessageHandler(mousepos);
                        }
                    }
				}
				if (this.AHInViewTab != null)
				{
					if (((msg.Msg >= 0x0200 /*WM_MOUSEFIRST*/) && (msg.Msg <= 0x0209 /*WM_MOUSELAST*/))
						|| (msg.Msg == 0x02A3 /*WM_MOUSELEAVE*/))
					{
						// If the cursor has moved out of the hostform's bounds or is not over 1) the actual hostform surface,
						// 2) the autohide tab strip or 3) the displayed autohidden DockHost control surface then set mousepos
						// to Empty thus initiating the hideItem sequence.
						Point mousepos = Cursor.Position;

						Rectangle hostformbounds;
						if (this.HostControl.Parent == null)
							hostformbounds = this.HostControl.Bounds;
						else
							hostformbounds = this.HostControl.Parent.RectangleToScreen(this.HostControl.Bounds);
						if (
							(hostformbounds.Contains(mousepos) == false) ||
							(((msg.Msg == 0x0200 /*WM_MOUSEMOVE*/) || (msg.Msg == 0x02A3 /*WM_MOUSELEAVE*/) || (msg.Msg == 0x0201)
							|| (msg.Msg == 0x0207/*WM_MBUTTONDOWN*/) || (msg.Msg == 0x0204/*WM_RBUTTONDOWN*/)) &&
							/*(msg.HWnd != this.HostControl.Handle) &&*/ (msg.HWnd != this.AHInViewTab.Handle)
							&& (this.AHInViewTab.GetActiveTabGroupOrSubGroupDockHostRect().Contains(mousepos) == false))
							)
						{
							mousepos = Point.Empty;
							if (((msg.Msg == 0x0201/*WM_LBUTTONDOWN*/) || (msg.Msg == 0x0204/*WM_RBUTTONDOWN*/) ||
								(msg.Msg == 0x0207/*WM_MBUTTONDOWN*/)) && this.AutoHideActiveControl)
							{
								this.ForceHideActiveControl = true;
							}
						}

						if(!(this.DragProvider.DraggingControl is DragSplitter))
						{
                            if(this.VisualStyle ==VisualStyle.Metro)
							this.AHInViewTab.MouseClick = false;
							this.AHInViewTab.MFMouseMessageHandler(mousepos);
						}
					}

					else if ((msg.Msg == 0x00A0 /*WM_NCMOUSEMOVE*/) && (msg.HWnd == this.HostControl.Handle))
					{
						int nhittestval = (int)msg.WParam;
						if (((nhittestval >= 10/*HTLEFT*/) && (nhittestval <= 17/*HTBOTTOMRIGHT*/)) || (nhittestval == 2/*HTCAPTION*/) || (nhittestval == 5/*HTMENU*/))
							this.AHInViewTab.MFNCMouseMessageHandler(Point.Empty);
						else
						{
							Point mousepos = Cursor.Position;
							if (this.AHInViewTab.GetActiveTabGroupOrSubGroupRect().Contains(mousepos) == false)
								this.AHInViewTab.MFNCMouseMessageHandler(Point.Empty);
						}
					}

					else if ((msg.Msg == 0x02A2 /*WM_NCMOUSELEAVE*/) && (msg.HWnd == this.HostControl.Handle))
					{
						this.AHInViewTab.MFNCMouseMessageHandler(Point.Empty);
					}
				}

				if( DragProvider != null )
				{
					if( msg.Msg == NativeMethods.WM_KEYDOWN )
					{
						if( ( int )msg.WParam == NativeMethods.VK_ESCAPE )
						{
							if( DragProvider.DraggingControl == null )
								DragProvider.AllowDrag = false;
							else
								DragProvider.DraggingControl.AbortDrag();

							m_bEscKeyPressed = true;
						}
						if( ( int )msg.WParam == NativeMethods.VK_CONTROL )
						{
							DragProvider.ProcessCtrlKeyDown();
						}
					}

					else if( msg.Msg == NativeMethods.WM_KEYUP )
					{
						if( ( int )msg.WParam == NativeMethods.VK_ESCAPE )
						{
							DragProvider.AllowDrag = true;
							m_bEscKeyPressed = false;
						}
						if( ( int )msg.WParam == NativeMethods.VK_CONTROL )
						{
							DragProvider.ProcessCtrlKeyUp();
						}
					}
				}

				if (this.controllerInFocus != null)
				{
					if ((msg.Msg == 0x0201/*WM_LBUTTONDOWN*/) || (msg.Msg == 0x0204/*WM_RBUTTONDOWN*/) ||
						(msg.Msg == 0x0207/*WM_MBUTTONDOWN*/))
					{
						if (Syncfusion.Runtime.InteropServices.NativeMethods.IsChild(this.controllerInFocus.HostControl.Handle, msg.HWnd) == false)
						{
							DockHost dhost = this.controllerInFocus.HostControl as DockHost;
							dhost.Invalidate(dhost.TitleBar.CaptionRect, false);
						}
					}
				}
			}
		}

        /// <summary>
        /// ICallWndProcListener implementation.
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wparam"></param>
        /// <param name="lparam"></param>
        [Syncfusion.Documentation.DocumentationExclude()]			
		public void CallWndProc(int nCode, IntPtr wparam, IntPtr lparam)
		{
			if ((this.HostControl == null) || (this.HostControl.IsHandleCreated == false))
				return;

			Syncfusion.Runtime.InteropServices.NativeMethods.CWPSTRUCT cwp =
				(Syncfusion.Runtime.InteropServices.NativeMethods.CWPSTRUCT)(Marshal.PtrToStructure(lparam, typeof(Syncfusion.Runtime.InteropServices.NativeMethods.CWPSTRUCT)));
			if (nCode >= 0)
			{
				if (this.DesignProcess == true)
				{
					if (cwp.message == 0x001C/*WM_ACTIVATEAPP*/)	// Activation change in VS.NET
					{
						foreach (DockControllerBase dcb in this.alFFControllers)
						{
							if (dcb.HostControl.Visible != false)
								dcb.HostControl.Visible = false;
						}
					}
				}
				else
				{
					if ((cwp.message == 0x0222/*WM_MDIACTIVATE*/) && (this.bMDIActivatedVisibility == true))
					{
						if ((cwp.wParam != IntPtr.Zero) && (cwp.lParam != IntPtr.Zero))
						{
							// If either the HostControl or the Form hosting the HostControl is an MDI child form and this form
							// is being activated or deactivated then show or hideItem the floating forms associated with that
							// particular form.
							Form ownrfrm = null;
							if ((this.HostControl is Form) && ((this.HostControl as Form).IsMdiChild == true))
								ownrfrm = this.HostControl as Form;
							if ((ownrfrm == null) && (this.HostControl.ParentForm != null) && (this.HostControl.ParentForm.IsMdiChild == true))
								ownrfrm = this.HostControl.ParentForm;
							if (ownrfrm != null)
							{
								if (cwp.lParam == ownrfrm.Handle)	// The child form is being activated
								{
									if (Syncfusion.Runtime.InteropServices.NativeMethods.IsWindowVisible(this.HostControl.Handle) == true)
									{
										if (this.bFloatingVisibility == false)
										{
											this.bFloatingVisibility = true;
											foreach (FloatingFormController ffc in this.alFFControllers)
											{
												if (ffc.HostControl.Visible == false)
													ffc.HostControl.Visible = true;
											}
										}
									}
								}
								else	// The form is being deactivated
								{
									if (this.bFloatingVisibility == true)
									{
										this.bFloatingVisibility = false;
										foreach (FloatingFormController ffc in this.alFFControllers)
										{
											if (ffc.HostControl.Visible == true)
												ffc.HostControl.Visible = false;
										}
									}
								}
							}
						}
					}
				}
			}
		}


		/// <summary>
		/// Sets a new size for the dockable control.
		/// </summary>
		/// <param name="ctrl">The docked/floating control.</param>
		/// <param name="newsize">Specifies the new size of the control.</param>
		/// <remarks>
		/// The SetControlSize method changes the dimensions of the docked control by displacing the
		/// horizontal/vertical splitter that is closest to the particular control.
		/// </remarks>
		public void SetControlSize(Control ctrl, Size newsize)
		{
			if (ctrl == null)
				throw new ArgumentNullException("ctrl");

			DockHostController dhc = GetDockHostController(ctrl);
			if (dhc == null)
			{
				Debug.Assert(false, "The control has not been enabled for docking.");
				return;
			}

			if( !dhc.DockVisibility )
			{
				dhc.StoredSize = newsize;
				return;
			}
			
			if( dhc.bInMDIMode )
				return;

			if (bApplyMinMaxExtents)
			{
				Size minSize = dhc.MinimumSize;
				if (newsize.Width < minSize.Width)
				{
					newsize.Width = minSize.Width;
				}
				if (newsize.Height < minSize.Height)
				{
					newsize.Height = minSize.Height;
				}
			}

			if (dhc.bInAutoHide)
			{
                bool bTabbed = false;

                DockControllerBase dcbparent = dhc.ParentController;
                if (dcbparent is DockTabController)
                {
                    dcbparent = dcbparent.ParentController;
                    bTabbed = true;
                }

                DockHost dh = dhc.HostControl as DockHost;

                int borderSize = dh.BorderWidth;
                int captionSize = dh.CaptionHeight;

                int width = newsize.Width + 2 * borderSize;
                int height = newsize.Height + 2 * borderSize + captionSize;

                if (bTabbed)
                {
                    if (DockTabAlignment == DockTabAlignmentStyle.Bottom || DockTabAlignment == DockTabAlignmentStyle.Top)
                    {
                        height += 3; // Top/Bottom indent caused by appearing of DockTabControl.
                    }
                    else
                    {
                        width += 1; // Left/Right indent caused by appearing of DockTabControl.
                    }
                }

                Size dhcsize = new Size(width,height);

				dhc.SetAutohiddenControlSize(dhcsize);
			}
			else
			{
				Size szcurrent = ctrl.Size;
				// If the Control is part of a tabbed group then use the dimensions of the tab's HostControl
				if ((dhc.ParentController != null) && (dhc.ParentController is DockTabController))
				{
					DockTabController dtc = dhc.ParentController as DockTabController;
					szcurrent = dtc.HostControl.Controls[0].Size;
				}

				int ndiffX = newsize.Width - szcurrent.Width;
				int ndiffY = newsize.Height - szcurrent.Height;
                if (this.HostForm != null && this.HostForm.RightToLeft == System.Windows.Forms.RightToLeft.Yes)
                {
                    ndiffX *= -1;
                    ndiffY *= -1;
                }
				if (ndiffX != 0)
				{
					bool retval = this.IterSetSplitterPosition(dhc, true, ndiffX);
					if ((retval == false) && (dhc.Floating == true))
						dhc.HostControl.TopLevelControl.Width += ndiffX;
				}
				if (ndiffY != 0)
				{
					bool retval = this.IterSetSplitterPosition(dhc, false, ndiffY);
					if ((retval == false) && (dhc.Floating == true))
						dhc.HostControl.TopLevelControl.Height += ndiffY;
				}
			}
		}

        /// <summary>
        /// Gets the size of the dockable control.
        /// </summary>
        /// <param name="ctrl">The docked or floating control.</param>
        /// <returns>Size of the dockable control.</returns>
        public Size GetControlSize(Control ctrl)
        {
            if (ctrl == null)
                throw new ArgumentNullException("ctrl");

            DockHostController dhc = GetDockHostController(ctrl);
            if (dhc == null)
            {
                Debug.Assert(false, "The control has not been enabled for docking.");
                return Size.Empty;
            }

            if (dhc.bInAutoHide)
            {
                bool bTabbed = false;
                DockControllerBase dcbparent = dhc.ParentController;
                if (dcbparent is DockTabController)
                {
                    dcbparent = dcbparent.ParentController;
                    bTabbed = true;
                }

                DockControllerBase tlcontroller = dcbparent.ToplevelController;

                DragSplitterController splitter = dcbparent.GetChildAt(1) as DragSplitterController;
                if (splitter != null && !splitter.HostControl.Visible && ctrl.Size == Size.Empty)
                {
                    Size dhcsize = dhc.DINew.rcDockArea.Size;
                    Size tlsize = tlcontroller.LayoutRect.Size;

                    DockHost dh = dhc.HostControl as DockHost;

                    int borderSize = dh.BorderWidth;
                    int captionSize = dh.CaptionHeight;

                    int width = (dcbparent.DICurrent.DP == DockPreference.Horizontal) ?
                        dhcsize.Width - 2 * borderSize :
                        tlsize.Width - 2 * borderSize;
                    int height = (dcbparent.DICurrent.DP == DockPreference.Horizontal) ?
                        tlsize.Height - 2 * borderSize - captionSize :
                        dhcsize.Height - 2 * borderSize - captionSize;

                    if (bTabbed)
                    {
                        if (DockTabAlignment == DockTabAlignmentStyle.Bottom || DockTabAlignment == DockTabAlignmentStyle.Top)
                        {
                            height -= 3; // Top/Bottom indent caused by appearing of DockTabControl.
                        }
                        else
                        {
                            width -= 1; // Left/Right indent caused by appearing of DockTabControl.
                        }
                    }
                    return new Size(width, height);
                }
            }
            else
            {
                try
                {
                    RefreshControlSize(ctrl, ctrl.Size);
                }
                catch { }
            }
            return ctrl.Size;
        }

		/// <summary>
		/// Specifies the minimum width and height to which the dockable control can be resized to.
		/// </summary>
		/// <param name="ctrl">The docking window.</param>
		/// <param name="minsize">A <see cref="System.Drawing.Size"/> value specifying the minimum bounds. Default value is Size.Empty.</param>
		/// <remarks>
		/// <para>
		/// The SetControlMinimumSize method is a part of the DockingManager's programmatic API and is not exposed by
		/// the docking windows designer. The application should invoke this method for each dock-enabled control that
		/// requires a set minimum size. The best place to call this method is from a handler for the
		/// <see cref="DockingManager.NewDockStateEndLoad"/> event.
		/// </para>
		/// <para>
		/// Please note that the control's minimum bounds are only a hint. While the <see cref="DockingManager"/> will
		/// enforce the set extents far as possible, layout constraints may at times force it to overrun the minimum size.
		/// </para>
		/// </remarks>
		public void SetControlMinimumSize(Control ctrl, Size minsize)
		{
			DockHostController dhc = this.GetDockHostController(ctrl);
			if (dhc == null)
			{
				Debug.Assert(false, "The control has not been enabled for docking.");
				return;
			}
			dhc.MinimumSize = minsize;
		}
		/// <summary>
		/// Returns the minimum bounds specified for the dockable control.
		/// </summary>
		/// <param name="ctrl">The docking window.</param>
		/// <returns>A <see cref="System.Drawing.Size"/> value indicating the minimum bounds. The default value is Size.Empty.</returns>
		public Size GetControlMinimumSize(Control ctrl)
		{
			DockHostController dhc = this.GetDockHostController(ctrl);
			if (dhc == null)
			{
				Debug.Assert(false, "The control has not been enabled for docking.");
				return Size.Empty;
			}
			return dhc.MinimumSize;
		}

        //Implementation of the IExtenderProvider::CanExtend method and the extended properties.

        /// <summary>
        /// Implementation of the IExtenderProvider::CanExtend method.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        Syncfusion.Documentation.DocumentationExclude()
        ]
		public bool CanExtend(object target)
		{
			if ((target is Form) || (target is MdiClient) || (target is DockingClientPanel))
				return false;

			if (target is Control)
			{
				Control ctrlparent = (target as Control).Parent;
				// Extended properties are allowed only for the immediate children of the HostControl.
				if ((ctrlparent != null) && ((ctrlparent == this.HostControl) || (ctrlparent is DockHost)))
					return true;
			}
			return false;
		}
		/// <summary>
		/// Indicates whether the control is a docking window.
		/// </summary>
		/// <param name="ctrl">The control to be queried.</param>
		/// <returns>TRUE if the control is a docking window; FALSE otherwise.</returns>
		[
		Category("Syncfusion Docking"),
		DefaultValue(false),
        DesignerSerializationVisibilityAttribute(  DesignerSerializationVisibility.Hidden),
		Description("Enables/disables the control as a docking window.")
		]
		public bool GetEnableDocking(Control ctrl)
		{
			return this.alEnableDocking.Contains(ctrl);
		}
		/// <summary>
		/// Enables or disables the control as a docking window.
		/// </summary>
		/// <param name="ctrl">The control instance.</param>
		/// <param name="value">TRUE indicates that the control is set as a docking window; FALSE to disable a dock-enabled control.</param>
		[
		Category("Syncfusion Docking"),
		DefaultValue(false),
        DesignerSerializationVisibilityAttribute(  DesignerSerializationVisibility.Hidden),
		Description("Enables/disables the control as a docking window.")
		]
		public void SetEnableDocking(Control ctrl, bool value)
		{

			if (ctrl == null)
				throw new ArgumentNullException("ctrl", "Parameter 'ctrl' can not be null");

            this.PreviousParentForm = null;
			bool bsavestream = false;
			if (value)
			{
                Control tempCtrl = ctrl;
                while (tempCtrl.Parent != null)
                {
                    if (tempCtrl.Parent is DockHost)
                    {
                        tempCtrl = tempCtrl.Parent;
                    }
                    else if(tempCtrl.Parent is Form)
                    {
                        this.PreviousParentForm = tempCtrl.Parent as Form;
                        this.PreviousParentVisibility = tempCtrl.Parent.Visible;
                        break;
                    }
                    else
                        tempCtrl = tempCtrl.Parent;
                }

				if (this.alEnableDocking.Contains(ctrl) == false)
				{
					bsavestream = true;
					this.alEnableDocking.Add(ctrl);
				}

				// If the hostform is null or BeginInit-EndInit is in process, delay initialization.
				if ((this.dcHostForm == null) || (this.dcHostForm.HostControl == null) || (this.bInBeginEndInit == true))
				{
					ctrl.BindingContext = new BindingContext();
					return;
				}

				DockHostController dhc = GetDockHostController(ctrl);
				if (dhc != null)
					return; // Enable docking has already been called

				// If form is MdiChild remove it from MdiClient - fix for defect 677.
				Form form = ctrl as Form;
				if (form != null)
				{
					if (form.MdiParent != null)
						form.MdiParent = null;
					form.TopLevel = false;
					// Make form visible in order to perform correct docking operations - fix for defect 6732.
					form.Visible = true;
				}

				InternalEnableDocking(ctrl, Syncfusion.Windows.Forms.Tools.DockingStyle.Left, -1);

				if ((this.DesignMode == false) && (this.alEnableDocking.Count > 0))
				{
					if (DesignerHooks.GetMsgProcListContains(this) == false)
						m_curentThreadId = DesignerHooks.AddGetMsgProcListener(this);
				}

				if (!htCustomCaptionButtons.ContainsKey(ctrl))
				{
					htCustomCaptionButtons.Add(ctrl, new CaptionButtonsCollection());
				}

				ApplyDHCInitialSettings(ctrl);
				SynchronizeCaptionButtons();

                if (ctrl.Parent != null && this.PreviousParentForm != null)
                {
                    this.PreviousParentForm.Visible = this.PreviousParentVisibility;
                    this.PreviousParentForm = null;
                }
			}
			else
			{
				if (this.DesignMode == true)
				{
					IDesignerHost idh = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
					if ((idh != null) && (idh.Loading == false) && (this.alInheritedControls.Contains(ctrl) == true))
					{
						MessageBox.Show("This is an inherited control. Docking can be disabled only in the base form.", "Essential Tools DockingManager", MessageBoxButtons.OK, MessageBoxIcon.Information);
						return;
					}
				}

				DockHostController dhc = GetDockHostController(ctrl);
				if (dhc == null)
					return;
                
				if (this.ActiveControl == ctrl)
				{
					SetActiveControl(null);
					this.ctrlLastActive = null;

					if(ctrl.Parent is DockHost && ctrl.Parent.Visible)
						this.ctrlLastActive = (ctrl.Parent as DockHost).GetNextControl(ctrl);
				}

				if (this.lastActivated == ctrl)
					this.lastActivated = null;

				if (this.mouseActivatedControl == ctrl)
					this.mouseActivatedControl = null;

				if (htCustomCaptionButtons.ContainsKey(ctrl))
					htCustomCaptionButtons.Remove(ctrl);

				// Exit mdi, autohide, tabbed dock and floatonly modes
				if (dhc.bInMDIMode == true)
				{
					if (dhc.ctrlReference != null)
						this.SetAsMDIChild(ctrl, false);
					else
						dhc.bInMDIMode = false;
				}
				
				if( this.alEnableDocking.Contains(ctrl) )
				{
					bsavestream = true;
					this.alEnableDocking.Remove(ctrl);
				}

				Rectangle rclayout = dhc.HostControl.RectangleToScreen(dhc.HostControl.Controls[0].Bounds);

                //Fix for Defect # 3175
                this.bFiredVisibilityChangedEvent = true;
                this.bFiredVisibilityChangingEvent = true;

				dhc.CloseController();

                //Fix for Defect # 3175
                this.bFiredVisibilityChangedEvent = false;
                this.bFiredVisibilityChangingEvent = false;

				this.RemoveController(dhc);
				DockHost dhost = dhc.HostControl as DockHost;
				dhost.RecSubscribeChildControlEvents(ctrl, false);
				dhost.UnsubscribeDockHostEvents();
				ctrl.Visible = true;

				if (this.DesignMode == true)
				{
					this.dcHostForm.HostControl.Controls.Add(ctrl);
					ctrl.Bounds = this.dcHostForm.HostControl.RectangleToClient(rclayout);
				}
				else
				{
					dhc.HostControl.Controls.Remove(ctrl);

					// If the ControlOwnedImages flag is set then remove the control's image from the ImageList
					// and update the image list array
					if (this.bControlScopeImages == true)
						this.RemoveDockHostImageFromList(dhc);
				}
				if (this.controllerInFocus == dhc)
					this.controllerInFocus = null;

				ctrl.Parent = HostControl;
				ctrl.Location = dhc.ControlLocation;
				ctrl.Size = dhc.ControlSize;

                if (dhc.SharedForm != null && !dhc.SharedForm.Used)
                {
                    dhc.SharedForm.Dispose();
                    dhc.SharedForm = null;
                }

				dhc.Dispose();                
                if (newCollection != null)
                {
                    newCollection.Dispose();
                    newCollection = null;
                }
                if (collection != null)
                {
                    collection.Dispose();
                    collection = null;
                }
                if (cbRemovedButtons != null)
                {
                    cbRemovedButtons.Dispose();
                    cbRemovedButtons = null;
                }
				if (this.DesignMode == true)
				{
					if (this.alInheritedControls.Contains(ctrl) == false)
					{
						// Remove the control from the extended property hashtables
						if (this.htText.Contains(ctrl) == true)
							this.htText.Remove(ctrl);
						if (this.htIcon.Contains(ctrl) == true)
							this.htIcon.Remove(ctrl);
                        if (this.htmIcon.Contains(ctrl) == true)
                            this.htmIcon.Remove(ctrl);
					}
					if (this.htFloatOnly.Contains(ctrl) == true)
						this.htFloatOnly.Remove(ctrl);
					if (this.listAHOnLoad.Contains(ctrl) == true)
						this.listAHOnLoad.Remove(ctrl);
					if (this.htHiddenOnLoad.Contains(ctrl) == true)
						this.htHiddenOnLoad.Remove(ctrl);
				}

                
			}

			// If docking is enabled, then allow the dynamic extended properties to be refreshed.
			if (this.DesignMode == true)
			{
				RaiseEnabledDocking(ctrl, value);
				TypeDescriptor.Refresh(ctrl);
				if (bsavestream == true)
					this.UpdateDesigner();
			}
			else
			{
				if (this.alEnableDocking.Count == 0)
				{
					if (DesignerHooks.GetMsgProcListContains(this, m_curentThreadId))
						DesignerHooks.RemoveGetMsgProcListener(this, m_curentThreadId);
                    this.alHiddenFFControllers.Clear();
				}
			}
            // instances needs to be cleared once enabled state is set as false
            if (htDockAbility.Contains(ctrl))
                htDockAbility.Remove(ctrl);
            if (htOuterDockAbility.Contains(ctrl))
                htOuterDockAbility.Remove(ctrl);
		}

		/// <summary>
		/// Sets the text to be displayed in the docking window caption.
		/// </summary>
		/// <param name="ctrl">The dock-enabled control.</param>
		/// <param name="strtext">A String value representing the text caption.</param>
		[
		Category("Syncfusion Docking"),
		//DefaultValue(""),
		RefreshProperties(RefreshProperties.All),
		Description("The text displayed by the docking window caption.")
		]
		public void SetDockLabel(Control ctrl, String strtext)
		{
			DockHostController dhc = GetDockHostController(ctrl);

			if( dhc != null )
			{
				if( dhc.bInMDIMode && !this.GetFloatOnly(ctrl) )
				{
					Form frm = ( Form )this.HostControl;
					IEnumerator enumMDIChildren = frm.MdiChildren.GetEnumerator();
					while( enumMDIChildren.MoveNext() )
					{
						DockingWrapperForm form = enumMDIChildren.Current as DockingWrapperForm;
						if( form != null && dhc.ctrlReference.Equals(ctrl) && dhc.DockLabel.Equals(form.Text) && form.Name.Contains(ctrl.Name))
						{
							dhc.DockLabel = form.Text = strtext;
							return;
						}
					}
				}

				dhc.DockLabel = strtext;
				if( this.DesignMode == true )
				{
					if( this.htText.Contains(ctrl) == true )
						this.htText.Remove(ctrl);
					this.htText.Add(ctrl, strtext);
				}
				// Set new label for FloatingForm that contains current control
				FloatingFormController ffc = dhc.GetParentFloatingFormController();
				if( ffc != null )
				{
					ffc.HostControl.Text = strtext;
					ffc.RefreshFormCaption();
					NativeMethodsHelper.RedrawWindow(ffc.HostControl.Handle, NativeMethods.RDW_INVALIDATE | NativeMethods.RDW_FRAME);
				}
			}
			else
			{
				if( this.htText.Contains(ctrl) == true )
					this.htText.Remove(ctrl);
				this.htText.Add(ctrl, strtext);
			}
		}

		/// <summary>
		/// Returns the text displayed in the docking window caption.
		/// </summary>
		/// <param name="ctrl">The dock-enabled control.</param>
		/// <returns>A String value representing the text caption.</returns>
		[
		Category("Syncfusion Docking"),
		//DefaultValue(""),
		RefreshProperties(RefreshProperties.All),
		Description("The text displayed by the docking window caption.")
		]
		public String GetDockLabel(Control ctrl)
		{
			DockHostController dhc = GetDockHostController(ctrl);
			if (dhc != null)
				return dhc.DockLabel;
			else if (this.htText.Contains(ctrl) == true)
				return this.htText[ctrl] as String;
			else
				return String.Empty;
		}

		/// <summary>
		/// Returns the index of the image associated with the docking window.
		/// </summary>
		/// <param name="ctrl">The dock-enabled control.</param>
		/// <param name="index">A zero-based index into the <see cref="DockingManager.ImageList"/> property value.</param>
		[
		Category("Syncfusion Docking"),
		//DefaultValue(-1),
		RefreshProperties(RefreshProperties.All),
		Description("The index of the image associated with this docking window.")
		]
		public void SetDockIcon(Control ctrl, int index)
		{
			DockHostController dhc = this.GetDockHostController(ctrl);
			if (dhc != null)
			{
				dhc.ImageIndex = index;
				if (this.DesignMode == true)
				{
					if (this.htIcon.Contains(ctrl) == true)
						this.htIcon.Remove(ctrl);
					this.htIcon.Add(ctrl, index);
				}
				if (dhc.Floating)
				{
					FloatingFormController ffc = dhc.GetParentFloatingFormController();
					if (ffc != null)
					{
                        ffc.ImageIndex = index;
						NativeMethodsHelper.RedrawWindow(ffc.HostControl.Handle,
							NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE);
					}
				}
                if (this.IsMDIMode(ctrl))
                    UpdateMDIChildIcon(ctrl);
			}
			else
			{
				if (this.htIcon.Contains(ctrl) == true)
					this.htIcon.Remove(ctrl);
				this.htIcon.Add(ctrl, index);
			}
		}
        
        /// <summary>
        /// Sets the index of the image associated with this docking window at MDI Child state.
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="index"></param>
        [
        Category("Syncfusion Docking"),
            //DefaultValue(-1),
        RefreshProperties(RefreshProperties.All),
        Description("The index of the image associated with this docking window at MDI Child state.")
        ]
        public void SetMDIChildIcon(Control ctrl, int index)
        {
            DockHostController dhc = this.GetDockHostController(ctrl);
            if (dhc != null)
            {
                dhc.MdiImageIndex= index;
                if (this.DesignMode == true)
                {
                    if (this.htmIcon.Contains(ctrl) == true)
                        this.htmIcon.Remove(ctrl);
                    this.htmIcon.Add(ctrl, index);
                }               
            }
            else
            {
                if (this.htmIcon.Contains(ctrl) == true)
                    this.htmIcon.Remove(ctrl);
                this.htmIcon.Add(ctrl, index);
            }
            UpdateMDIChildIcon(ctrl);
            SetDockIcon(ctrl, index);
        }

       /// <summary>
        /// Sets the index of the image associated with this docking window at MDI Child state.
       /// </summary>
       /// <param name="ctrl"></param>
       /// <param name="image"></param>
        public void SetMDIChildIcon(Control ctrl, Icon image)
        {
            if (ctrl != null && image != null)
            {
                string imgKey = ctrl.GetHashCode().ToString();
                DockHostController dhc = this.GetDockHostController(ctrl);

                if (this.ImageList == null)
                    this.ImageList = new ImageList();

                bool imageAdded = false;
                if (this.ilDockTabs.Images.ContainsKey(imgKey) && dhc != null)
                {
                    int index = dhc.ImageIndex;

                    if (this.ilDockTabs.Images.Count == 1)
                    {
                        this.ilDockTabs.Images.RemoveByKey(imgKey);
                    }
                    else
                    {
                        using (ImageList tempList = new ImageList())
                        {

                            foreach (string str in this.ilDockTabs.Images.Keys)
                            {
                                tempList.Images.Add(str, this.ilDockTabs.Images[str]);
                            }
                            tempList.Images.RemoveByKey(imgKey);

                            this.ilDockTabs.Images.Clear();

                            int count = 0;
                            foreach (string str in tempList.Images.Keys)
                            {
                                if (count == index)
                                {
                                    this.ilDockTabs.Images.Add(imgKey, image);
                                    imageAdded = true;
                                }

                                this.ilDockTabs.Images.Add(str, tempList.Images[str]);
                                count++;
                            }
                        }
                    }
                }

                if (!imageAdded)
                    this.ilDockTabs.Images.Add(ctrl.GetHashCode().ToString(), image);

                Debug.Assert(dhc != null);
                dhc.MdiImageIndex = this.ilDockTabs.Images.IndexOfKey(imgKey);
                dhc.ControlImage = image;
                if (dhc.Floating)
                {
                    FloatingFormController ffc = dhc.GetParentFloatingFormController();
                    if (ffc != null)
                    {
                        
                        NativeMethodsHelper.RedrawWindow(ffc.HostControl.Handle,
                            NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE);
                    }
                }
                else
                {
                    NativeMethodsHelper.RedrawWindow(dhc.HostControl.Handle,
                                NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE);
                }
                UpdateMDIChildIcon(ctrl);
                SetDockIcon(ctrl, image);
            }
        }



		/// <summary>
		/// Sets the Image associated with the docking window.
		/// </summary>
		/// <param name="ctrl">The dock enabled control.</param>
		/// <param name="image">The <see cref="System.Drawing.Icon"/> representing the docking window.</param>
		/// <remarks>
		/// This overloaded version of the <see cref="DockingManager.SetDockIcon"/> method is normally used only
		/// in combination with the <see cref="DockingManager.ControlScopeImages"/> property. Setting ControlScopeImages to
		/// TRUE signifies that dockable controls will provide their own images objects during initialization and the
		/// scope of these images will be restricted to the control's existence as a docking window.
		/// <seealso cref="DockingManager.ControlScopeImages"/>
		/// </remarks>
		public void SetDockIcon(Control ctrl, Icon image)
		{
			if( ctrl != null && image != null )
			{

                string imgKey = ctrl.GetHashCode().ToString();
                DockHostController dhc = this.GetDockHostController(ctrl);

                if (this.ImageList == null)
                    this.ImageList = new ImageList();

                bool imageAdded = false;
                if (this.ilDockTabs.Images.ContainsKey(imgKey) && dhc != null)
                {
                    int index = dhc.ImageIndex;

                    if (this.ilDockTabs.Images.Count == 1)
                    {
                        this.ilDockTabs.Images.RemoveByKey(imgKey);
                    }
                    else
                    {
                        using (ImageList tempList = new ImageList())
                        {
                            foreach (string str in this.ilDockTabs.Images.Keys)
                            {
                                tempList.Images.Add(str, this.ilDockTabs.Images[str]);
                            }
                            tempList.Images.RemoveByKey(imgKey);

                            this.ilDockTabs.Images.Clear();

                            int count = 0;
                            foreach (string str in tempList.Images.Keys)
                            {
                                if (count == index)
                                {
                                    this.ilDockTabs.Images.Add(imgKey, image);
                                    imageAdded = true;
                                }

                                this.ilDockTabs.Images.Add(str, tempList.Images[str]);
                                count++;
                            }
                        }
                    }
                }

                if (!imageAdded)
                    this.ilDockTabs.Images.Add(ctrl.GetHashCode().ToString(), image);
				
				Debug.Assert( dhc != null );
                dhc.ImageIndex = this.ilDockTabs.Images.IndexOfKey(imgKey);
				dhc.ControlImage = image;
                
                if (dhc.Floating)
                {
                    FloatingFormController ffc = dhc.GetParentFloatingFormController();
                    if (ffc != null)
                    {
                        ffc.ImageIndex = dhc.ImageIndex;
                        NativeMethodsHelper.RedrawWindow(ffc.HostControl.Handle,
                            NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE);
                    }
                }
                else
                {
                    NativeMethodsHelper.RedrawWindow(dhc.HostControl.Handle,
                               NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE);
                }

                if (this.IsMDIMode(ctrl))
                    UpdateMDIChildIcon(ctrl);
			}
		}

		/// <summary>
		/// Returns the index of the image associated with the docking window.
		/// </summary>
		/// <param name="ctrl">The dock-enabled control.</param>
		/// <returns>A zero-based index into the <see cref="DockingManager.ImageList"/> property value.</returns>
		[
		Category("Syncfusion Docking"),
		//DefaultValue(-1),
		RefreshProperties(RefreshProperties.All),
		Description("The index of the image associated with this docking window.")
		]
		public int GetDockIcon(Control ctrl)
		{
			DockHostController dhc = GetDockHostController(ctrl);
			if (dhc != null)
				return dhc.ImageIndex;
			else if (this.htIcon.Contains(ctrl) == true)
				return (int)this.htIcon[ctrl];
			else
				return -1;
		}

        /// <summary>
        /// Gets the index of the image associated with this docking window at MDI Child state.
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        [
        Category("Syncfusion Docking"),
            //DefaultValue(-1),
        RefreshProperties(RefreshProperties.All),
        Description("The index of the image associated with this docking window at MDI Child state.")
        ]
        public int GetMDIChildIcon(Control ctrl)
        {
            DockHostController dhc = GetDockHostController(ctrl);
            if (dhc != null)
                return dhc.MdiImageIndex;
            else if (this.htmIcon.Contains(ctrl) == true)
                return (int)this.htmIcon[ctrl];
            else
                return -1;
        }

		/// <summary>
		/// Sets the control as a non-dockable float-only window.
		/// </summary>
		/// <param name="ctrl">The dock-enabled control.</param>
		/// <param name="bfloating">TRUE to disable docking.</param>
		[
		Category("Syncfusion Docking"),
		//DefaultValue(false),
		RefreshProperties(RefreshProperties.All),
		Description("Indicates whether the docking window is set for the float-only mode.")
		]
		public void SetFloatOnly(Control ctrl, bool bfloating)
		{
			DockHostController dhc = GetDockHostController(ctrl);
			if (dhc != null)
			{
				dhc.FloatOnly = bfloating;
				if (this.DesignMode == true)
				{
					if (this.htFloatOnly.Contains(ctrl) == true)
						this.htFloatOnly.Remove(ctrl);
					this.htFloatOnly.Add(ctrl, bfloating);
				}
			}
			else
			{
				if (this.htFloatOnly.Contains(ctrl) == true)
					this.htFloatOnly.Remove(ctrl);
				this.htFloatOnly.Add(ctrl, bfloating);
			}
		}

		/// <summary>
		/// Indicates whether the control is a non-dockable float-only docking window.
		/// </summary>
		/// <param name="ctrl">The dock-enabled control.</param>
		/// <returns>TRUE if the control is a float-only docking window.</returns>
		[
		Category("Syncfusion Docking"),
		//DefaultValue(false),
		RefreshProperties(RefreshProperties.All),
		Description("Indicates whether the docking window is set for the float-only mode.")
		]
		public bool GetFloatOnly(Control ctrl)
		{
			DockHostController dhc = GetDockHostController(ctrl);
			if (dhc != null)
				return dhc.FloatOnly;
			else if (this.htFloatOnly.Contains(ctrl) == true)
				return (bool)this.htFloatOnly[ctrl];
			else
				return false;
		}

		/// <summary>
		/// Indicates whether the control can transit to floating state.
		/// </summary>
		/// <param name="ctrl">The dock-enabled control.</param>
		/// <returns>TRUE if the control can transit to floating state.</returns>
		[
		Category( "Syncfusion Docking" ),
			DefaultValue( true ),
		RefreshProperties( RefreshProperties.All ),
		Description( "Indicates whether the docking window can be transited to floating state" )
		]
		public bool GetAllowFloating( Control ctrl )
		{
			DockHostController dhc = GetDockHostController( ctrl );
			if( dhc != null )
				return dhc.AllowFloating;
			else if( this.htAllowFloating.Contains( ctrl ) == true )
				return ( bool )this.htAllowFloating[ctrl];
			else
				return false;
		}

		/// <summary>
		/// Sets if the control can transit to floating state.
		/// </summary>
		/// <param name="ctrl">The dock-enabled control.</param>
		/// <param name="bfloating">TRUE to allow floating.</param>
		[
		Category( "Syncfusion Docking" ),
		DefaultValue( true ),
		RefreshProperties( RefreshProperties.All ),
		Description( "Indicates whether the docking window can be transited to floating state" )
		]
		public void SetAllowFloating( Control ctrl, bool bfloating )
		{
			DockHostController dhc = GetDockHostController( ctrl );
			if( dhc != null )
			{
				dhc.AllowFloating = bfloating;
				if( this.DesignMode == true )
				{
					if( this.htAllowFloating.Contains( ctrl ) == true )
						this.htAllowFloating.Remove( ctrl );
					this.htAllowFloating.Add( ctrl, bfloating );
				}
			}
			else
			{
				if( this.htAllowFloating.Contains( ctrl ) == true )
					this.htAllowFloating.Remove( ctrl );
				this.htAllowFloating.Add( ctrl, bfloating );
			}
		}

        /// <summary>
        /// Specifies whether the docking window should be in the autohide mode on application startup.
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="bautohide"></param>
        [
        Category("Syncfusion Docking"),
            //DefaultValue(false),
        RefreshProperties(RefreshProperties.All),
        Description("Specifies whether the docking window should be in the autohide mode on application startup.")
        ]
        [Syncfusion.Documentation.DocumentationExclude()]
		public void SetAutoHideOnLoad(Control ctrl, bool bautohide)
		{
			this.listAHOnLoad.Remove(ctrl);

			if( bautohide )
				this.listAHOnLoad.Add(ctrl);

			if (bautohide == true)
			{
				// If dockvisibilityonload is set, then remove it.
				if (this.htHiddenOnLoad.Contains(ctrl) == true)
				{
					this.htHiddenOnLoad.Remove(ctrl);
					this.htHiddenOnLoad.Add(ctrl, false);
				}
			}
			if( this.DesignMode && !this.bInBeginEndInit )
				this.UpdateDesigner();
		}

        /// <summary>
        /// Specifies whether the docking window should be in the autohide mode on application startup.
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        [
        Category("Syncfusion Docking"),
            //DefaultValue(false),
        RefreshProperties(RefreshProperties.All),
        Description("Specifies whether the docking window should be in the autohide mode on application startup.")
        ]
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool GetAutoHideOnLoad(Control ctrl)
        {
            return this.listAHOnLoad.Contains(ctrl);
        }

        /// <summary>
        /// Specifies whether the docking window should be hidden on application startup.
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="bhidden"></param>
        [
        Category("Syncfusion Docking"),
            //DefaultValue(false),
        RefreshProperties(RefreshProperties.All),
        Description("Specifies whether the docking window should be hidden on application startup.")
        ]
        [Syncfusion.Documentation.DocumentationExclude()]
        public void SetHiddenOnLoad(Control ctrl, bool bhidden)
        {
            if (this.htHiddenOnLoad.Contains(ctrl) == true)
                this.htHiddenOnLoad.Remove(ctrl);
            this.htHiddenOnLoad.Add(ctrl, bhidden);
            if (bhidden == true)
            {
                // If the control has the autohideonload value set, then remove if from the ahtable
                if (this.listAHOnLoad.Contains(ctrl))
                {
                    this.listAHOnLoad.Remove(ctrl);
                }
            }
        }

        /// <summary>
        /// Specifies whether the docking window should be hidden on application startup.
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        [
        Category("Syncfusion Docking"),
            //DefaultValue(false),
        RefreshProperties(RefreshProperties.All),
        Description("Specifies whether the docking window should be hidden on application startup.")
        ]
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool GetHiddenOnLoad(Control ctrl)
        {
            if (this.htHiddenOnLoad.Contains(ctrl) == true)
                return (bool)this.htHiddenOnLoad[ctrl];
            return false;
        }

        /// <summary>
        /// Specifies whether the docking window should not be resized.
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="freeze"></param>
        [
        Category("Syncfusion Docking"),
        RefreshProperties(RefreshProperties.All),
        Description("Specifies whether the docking window should not be resized.")
        ]
        public void SetFreezeResize(Control ctrl, bool freeze)
        {
            if (freeze)
            {
                if (!m_freezeResizeCtrls.Contains(ctrl))
                    m_freezeResizeCtrls.Add(ctrl);
            }
            else
            {
                if (m_freezeResizeCtrls.Contains(ctrl))
                    m_freezeResizeCtrls.Remove(ctrl);
            }

            DockHostController dhc = GetDockHostController(ctrl);

            if (dhc != null && !this.DesignMode)
                dhc.FreezeResize = freeze;
        }

        /// <summary>
        /// Specifies whether the docking window should not be resized.
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        [
        Category("Syncfusion Docking"),
        RefreshProperties(RefreshProperties.All),
        Description("Specifies whether the docking window should not be resized.")
        ]
        public bool GetFreezeResize(Control ctrl)
        {
            return m_freezeResizeCtrls.Contains(ctrl);
        }

        /// <summary>
        /// Indicates where user can dock in this control using drag providers (Arrow drag providers only).
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="ability"></param>
        [
        Category("Syncfusion Docking"),
        RefreshProperties(RefreshProperties.All),
        Description("Indicates where user can dock in this control using drag providers (Arrow drag providers only).")
        ]
        public void SetDockAbility(Control ctrl, DockAbility ability)
        {
            if (htDockAbility.Contains(ctrl))
            {
                htDockAbility.Remove(ctrl);
            }
            htDockAbility.Add(ctrl, ability);
        }

        [
        Category("Syncfusion Docking"),
        RefreshProperties(RefreshProperties.All),
        Description("Indicates where user can dock in this control using drag providers (Arrow drag providers only).")
        ]
        public void SetDockAbility(Control ctrl, int nAbility)
        {
            DockAbility ability = (DockAbility)(Enum.ToObject(typeof(DockAbility), nAbility));
            SetDockAbility(ctrl, ability);
        }

        /// <summary>
        /// Indicates where user can dock in this control using drag providers (Arrow drag providers only).
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="strAbility"></param>
        [
        Category("Syncfusion Docking"),
        RefreshProperties(RefreshProperties.All),
        Description("Indicates where user can dock in this control using drag providers (Arrow drag providers only).")
        ]
        public void SetDockAbility(Control ctrl, string strAbility)
        {
            DockAbility ability = (DockAbility)(Enum.Parse(typeof(DockAbility), strAbility));
            SetDockAbility(ctrl, ability);
        }

        /// <summary>
        /// Indicates whether user can dock in this control, using drag providers (Arrow drag providers only).
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        [
        Category("Syncfusion Docking"),
        RefreshProperties(RefreshProperties.All),
        Description("Indicates where user can dock in this control using drag providers (Arrow drag providers only).")
        ]
        public DockAbility GetDockAbility(Control ctrl)
        {
            if (htDockAbility.Contains(ctrl))
            {
                return (DockAbility)htDockAbility[ctrl];
            }
            else
            {
                return DockAbility.All;
            }
        }

        /// <summary>
        /// Indicates where user can dock this control using drag providers (Arrow drag providers only).
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="ability"></param>
        [
        Category("Syncfusion Docking"),
        RefreshProperties(RefreshProperties.All),
        Description("Indicates where user can dock this control using drag providers (Arrow drag providers only).")
        ]
        public void SetOuterDockAbility(Control ctrl, DockAbility ability)
        {
            if (htOuterDockAbility.Contains(ctrl))
            {
                htOuterDockAbility.Remove(ctrl);
            }
            htOuterDockAbility.Add(ctrl, ability);
        }

        [
        Category("Syncfusion Docking"),
        RefreshProperties(RefreshProperties.All),
        Description("Indicates where user can dock this control using drag providers (Arrow drag providers only).")
        ]
        public void SetOuterDockAbility(Control ctrl, int nAbility)
        {
            DockAbility ability = (DockAbility)(Enum.ToObject(typeof(DockAbility), nAbility));
            SetOuterDockAbility(ctrl, ability);
        }

        /// <summary>
        /// Indicates where user can dock this control using drag providers (Whidbey and VS2005 drag providers only).
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="strAbility"></param>
        [
        Category("Syncfusion Docking"),
        RefreshProperties(RefreshProperties.All),
        Description("Indicates where user can dock this control using drag providers (Whidbey and VS2005 drag providers only).")
        ]
        public void SetOuterDockAbility(Control ctrl, string strAbility)
        {
            DockAbility ability = (DockAbility)(Enum.Parse(typeof(DockAbility), strAbility));
            SetOuterDockAbility(ctrl, ability);
        }

        /// <summary>
        /// Indicates where user can dock this control using drag providers (Whidbey and VS2005 drag providers only).
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        [
        Category("Syncfusion Docking"),
        RefreshProperties(RefreshProperties.All),
        Description("Indicates where user can dock this control using drag providers (Whidbey and VS2005 drag providers only).")
        ]
        public DockAbility GetOuterDockAbility(Control ctrl)
        {
            if (htOuterDockAbility.Contains(ctrl))
            {
                return (DockAbility)htOuterDockAbility[ctrl];
            }
            else
            {
                return DockAbility.All;
            }
        }

        /// <summary>
        /// Gets custom caption buttons collection for each docked control.
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        [
        Category("Syncfusion Docking"),
        RefreshProperties(RefreshProperties.All),
        Description("Contains custom caption buttons collection for each docked control.")
        ]
        public CaptionButtonsCollection GetCustomCaptionButtons(Control ctrl)
        {
            if (htCustomCaptionButtons != null && htCustomCaptionButtons.ContainsKey(ctrl))
            {
                return htCustomCaptionButtons[ctrl] as CaptionButtonsCollection;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Sets custom caption buttons collection for each docked control.
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="buttons"></param>
        [
        Category("Syncfusion Docking"),
        RefreshProperties(RefreshProperties.All),
        Description("Contains custom caption buttons collection for each docked control.")
        ]

		public void SetCustomCaptionButtons(Control ctrl, CaptionButtonsCollection buttons)
		{
			if (htCustomCaptionButtons.ContainsKey(ctrl))
			{
				htCustomCaptionButtons[ctrl] = buttons;
			}
			else
			{
				htCustomCaptionButtons.Add(ctrl, buttons);
			}
		}

		/// <summary>
		/// Sets the auto hide button's tooltip.
		/// </summary>
		/// <param name="text">Tooltip text.</param>
		public void SetAutoHideButtonToolTip(string text)
		{
			foreach( CaptionButton button in this.CaptionButtons )
			{
				if( button.Type == CaptionButtonType.Pin )
					button.ToolTip = text;
			}

			autoHideButtonToolTip = text;
		}
		/// <summary>
		/// Returns the auto hide button's tooltip.
		/// </summary>
		/// <returns>A <see cref="System.String"/> value which is displaying as the tooltip of AutoHideButton.</returns>
		public string GetAutoHideButtonToolTip()
		{
			foreach( CaptionButton button in this.CaptionButtons )
			{
				if( button.Type == CaptionButtonType.Pin )
					return button.ToolTip;
			}

			return autoHideButtonToolTip;
		}
		/// <summary>
		/// Sets the close button's tooltip.
		/// </summary>
		/// <param name="text">Tooltip text.</param>
		public void SetCloseButtonToolTip(string text)
		{
			foreach( CaptionButton button in this.CaptionButtons )
			{
				if( button.Type == CaptionButtonType.Close )
					button.ToolTip = text;
			}

			closeButtonToolTip = text;
		}
		/// <summary>
		/// Returns the close button's tooltip.
		/// </summary>
		/// <returns>A <see cref="System.String"/>value which is displaying as the tooltip of Close Button.</returns>
		public string GetCloseButtonToolTip()
		{
			foreach( CaptionButton button in this.CaptionButtons )
			{
				if( button.Type == CaptionButtonType.Close )
					return button.ToolTip;
			}

			return closeButtonToolTip;
		}
		/// <summary>
		/// Sets the window position button's tooltip.
		/// </summary>
		/// <param name="text">Tooltip text.</param>
		public void SetMenuButtonToolTip(string text)
		{
			foreach( CaptionButton button in this.CaptionButtons )
			{
				if( button.Type == CaptionButtonType.Menu )
					button.ToolTip = text;
			}

			menuButtonToolTip = text;
		}
		/// <summary>
		/// Returns the window position button's tooltip.
		/// </summary>
		/// <returns>Text for window position button tooltip <see cref="System.String"/>.</returns>
		public string GetMenuButtonToolTip()
		{
			foreach( CaptionButton button in this.CaptionButtons )
			{
				if( button.Type == CaptionButtonType.Menu )
					return button.ToolTip;
			}

			return menuButtonToolTip;
		}
		/// <summary>
		/// Sets the maximize button's tooltip.
		/// </summary>
		/// <param name="text">Tooltip text.</param>
		public void SetMaximizeButtonToolTip( string text )
		{
			foreach( CaptionButton button in this.CaptionButtons )
			{
				if( button.Type == CaptionButtonType.Maximize )
					button.ToolTip = text;
			}

			maximizeButtonToolTip = text;
		}
		/// <summary>
		/// Returns the maximize button's tooltip.
		/// </summary>
		/// <returns>Text for maximize button tooltip <see cref="System.String"/>.</returns>
		public string GetMaximizeButtonToolTip()
		{
			foreach( CaptionButton button in this.CaptionButtons )
			{
				if( button.Type == CaptionButtonType.Maximize )
					return button.ToolTip;
			}

			return maximizeButtonToolTip;
		}
		/// <summary>
		/// Sets the restore button tooltip.
		/// </summary>
		/// <param name="text">Tooltip text.</param>
		public void SetRestoreButtonToolTip( string text )
		{
			restoreButtonToolTip = text;
		}
		/// <summary>
		/// Returns the restore button tooltip.
		/// </summary>
		/// <returns>Text for restore button tooltip <see cref="System.String"/>.</returns>
		public string GetRestoreButtonToolTip()
		{
			return restoreButtonToolTip;
		}
		/// <summary>
		/// Returns the visibility state for the docking window's autohide button.
		/// </summary>
		/// <param name="ctrl">The dock-enabled control.</param>
		/// <returns>TRUE if the autohide button is displayed. Default is TRUE.</returns>
		public bool GetAutoHideButtonVisibility(Control ctrl)
		{
			DockHostController dhc = GetDockHostController(ctrl);
			if (dhc != null)
			{
				return dhc.AutoHideButtonVisibility;
			}
			return false;
		}
		/// <summary>
		/// Sets the visibility state for the docking window's autohide button.
		/// </summary>
		/// <param name="ctrl">The dock-enabled control.</param>
		/// <param name="bvisible">TRUE to display the autohide button. Default is TRUE.</param>
		public void SetAutoHideButtonVisibility(Control ctrl, bool bvisible)
		{
			DockHostController dhc = GetDockHostController(ctrl);
			if (dhc != null)
			{
				if (dhc.AutoHideButtonVisibility != bvisible)
					dhc.AutoHideButtonVisibility = bvisible;
                if (dhc.DockVisibility && dhc.Floating)
                {
                    FloatingForm fform = dhc.HostControl.TopLevelControl as FloatingForm;
                    if (fform != null)
                    {
                        fform.AutoHideButtonVisibility = bvisible;
                        fform.UpdateControlBoxVisibility();
                    }
                }
			}
		}
		/// <summary>
		/// Sets the visibility state for the docking window's close button.
		/// </summary>
		/// <param name="ctrl">The dock-enabled control.</param>
		/// <param name="bvisible">TRUE to display the close button. Default is TRUE.</param>
		public void SetCloseButtonVisibility(Control ctrl, bool bvisible)
		{
			DockHostController dhc = GetDockHostController(ctrl);
			if (dhc != null)
			{
				if (dhc.CloseButtonVisibility != bvisible)
				{
					dhc.CloseButtonVisibility = bvisible;
					if ((dhc.DockVisibility == true) && (dhc.Floating == true))
					{
						FloatingForm fform = dhc.HostControl.TopLevelControl as FloatingForm;
						Debug.Assert(fform != null);
						fform.CloseButtonVisibility = bvisible;
						fform.UpdateControlBoxVisibility();
					}
				}
			}
		}
		/// <summary>
		/// Returns the visibility state for the docking window's close button.
		/// </summary>
		/// <param name="ctrl">The dock-enabled control.</param>
		/// <returns>TRUE if the close button is displayed. Default is TRUE.</returns>
		public bool GetCloseButtonVisibility(Control ctrl)
		{
			DockHostController dhc = GetDockHostController(ctrl);
			if (dhc != null)
			{
				return dhc.CloseButtonVisibility;
			}
			return false;
		}
		/// <summary>
		/// Sets the visibility state for the docking window's window position button.
		/// </summary>
		/// <param name="ctrl">The dock-enabled control.</param>
		/// <param name="bvisible">TRUE to display the window position button. Default is TRUE.</param>
		public void SetMenuButtonVisibility(Control ctrl, bool bvisible)
		{
			DockHostController dhc = GetDockHostController(ctrl);
			if (dhc != null && dhc.MenuButtonVisiblity != bvisible)
			{
				dhc.MenuButtonVisiblity = bvisible;
			}
		}
		/// <summary>
		/// Returns the visibility state for the docking window's window position button.
		/// </summary>
		/// <param name="ctrl">The dock-enabled control.</param>
		/// <returns>TRUE if the window position button is displayed. Default is TRUE.</returns>
		public bool GetMenuButtonVisibility(Control ctrl)
		{
			DockHostController dhc = GetDockHostController(ctrl);
			if (dhc != null)
			{
				return dhc.MenuButtonVisiblity;
			}
			return false;
		}
		

		/// <summary>
		/// Returns the docking window's visibility state.
		/// </summary>
		/// <remarks>
		/// A control's DockVisibility indicates whether the control is currently 'closed' or is
		/// an active participant in the interactions within the current set of docking windows.
		/// This is different from the Control.Visible property as a dockable control that is not
		/// visible may still be a part of the docking implementation such as when it is in the
		/// autohide or tabbed docking modes.
		/// </remarks>
		/// <param name="ctrl"> The control for which the DockVisibility is to be queried.</param>
		/// <returns> TRUE if the control is a part of the current dock set; FALSE if it has been closed.</returns>
		public bool GetDockVisibility(Control ctrl)
		{
			DockHostController dhost = GetDockHostController(ctrl);
			if (dhost != null)
				return dhost.DockVisibility;
			return false;
		}

        /// <summary>
        /// Sets the docking window's visibility state.
        /// </summary>
        /// <remarks>
        /// A control's DockVisibility indicates whether the control is currently 'closed' or is
        /// an active participant of the interactions within the current set of docking windows.
        /// This is different from the Control.Visible property as a dockable control that is not
        /// visible may still be a part of the docking implementation such as when it is in the
        /// autohide or tabbed docking modes.
        /// </remarks>
        /// <param name="ctrl">The control for which the DockVisibility is to be set.</param>
        /// <param name="bvisible">TRUE indicates that the control will be a part of the current dockset. Else
        /// the control will be closed. Clicking the 'X' button sets the DockVisibility to be false.</param>
        /// <param name="floatingLocation"> Location whether the floating form need to be displayed, in case of floating control</param>
        public void SetDockVisibility(Control ctrl, bool bvisible, Point floatingLocation)
        {
            if (bvisible)
            {
                this.FloatingLocation = floatingLocation;
                this.CustomizeControlCollection.Add(ctrl);
            }
            SetDockVisibility(ctrl, bvisible);
        }
		/// <summary>
		/// Sets the docking window's visibility state.
		/// </summary>
		/// <remarks>
		/// A control's DockVisibility indicates whether the control is currently 'closed' or is
		/// an active participant of the interactions within the current set of docking windows.
		/// This is different from the Control.Visible property as a dockable control that is not
		/// visible may still be a part of the docking implementation such as when it is in the
		/// autohide or tabbed docking modes.
		/// </remarks>
		/// <param name="ctrl"> The control for which the DockVisibility is to be set.</param>
		/// <param name="bvisible"> TRUE indicates that the control will be a part of the current dockset. Else
		/// the control will be closed. Clicking the 'X' button sets the DockVisibility to be false.</param>
		public void SetDockVisibility(Control ctrl, bool bvisible)
		{
			bool bPrevForbidFreeze = ForbidFreeze;
			ForbidFreeze = true;
			SuspendLayout();

            this.bFreezeDockStateChangeEvents = true;
			DockHostController dhost = GetDockHostController(ctrl);
			if ((dhost != null) && (dhost.DockVisibility != bvisible))
			{
				// If the DockHost is in an autohidden state and belongs to a AHTabControl that is at present
				// displaying a control or is in the midst of an animation
				if ((dhost.AutoHideMode == true) && (this.AHInViewTab != null)
					&& (this.dcHostForm.GetAHTabControl(dhost.DINew.dStyle) == this.AHInViewTab))
				{
					this.HideAutoHiddenControl();
                    dhost.DockVisibility = bvisible;
                    this.bFreezeDockStateChangeEvents = false;
					ResumeLayout();
					return;
				}

				if (dhost.bInMDIMode == true)
				{
                    if (ctrl.Visible == bvisible)
                    {
                        this.bFreezeDockStateChangeEvents = false;
						ResumeLayout();
                        return;
                    }

					DockVisibilityChangingEventArgs arg_changing = new DockVisibilityChangingEventArgs( ctrl );
					if( bFiredVisibilityChangingEvent == false )
					{
						FireDockVisibilityChangingEvent( arg_changing );
						bFiredVisibilityChangingEvent = !bHoldEvents;
						bCancelVisibilityChangingEvent = arg_changing.Cancel;
					}

					if (!bvisible && !arg_changing.Cancel && !bCancelVisibilityChangingEvent)
					{
						dhost.MdiChildState = true;

						if( dhost.ctrlReference != null )
						{
							Control parent = dhost.ctrlReference.Parent;
							if( parent != null )
								dhost.MdiChildBounds = parent.Bounds;
						}
						this.SetAsMDIChild(ctrl, bvisible);

						DockVisibilityChangedEventArgs arg_changed = new DockVisibilityChangedEventArgs( ctrl );
						if( bFiredVisibilityChangedEvent == false )
						{
							FireDockVisibilityChangedEvent( arg_changed );
							bFiredVisibilityChangedEvent = !bHoldEvents;
						}
					}
				}

                if(dhost.Floating ==true)
                {
                    dhost.AutoHiddedBeforeHide = false;
                }

                if (dhost.ParentController is DockTabController
                    && ((DockTabController)dhost.ParentController).IsClosing)
                    return;

				dhost.DockVisibility = bvisible;

				if( !bCancelVisibilityChangingEvent )
				{
					if (dhost.MdiChildState)
					{
						if (bvisible)
						{
							this.SetAsMDIChild(ctrl, bvisible);
							dhost.MdiChildState = false;
						}
					}
				}

				if (bvisible && dhost.Floating)
				{
					ctrl.TopLevelControl.Focus();
					ctrl.Focus();
				}
			}

			this.bFiredVisibilityChangedEvent = false;
			this.bFiredVisibilityChangingEvent = false;
			this.bCancelVisibilityChangingEvent = false;
            this.bFreezeDockStateChangeEvents = false;

			ResumeLayout();
			ForbidFreeze = bPrevForbidFreeze;
		}

		/// <summary>
		/// Maximizes the specified dockable control.
		/// </summary>
		/// <param name="ctrl">The control instance.</param>
		public void MaximizeControl(Control ctrl)
		{
			DockHostController dhc = this.GetDockHostController(ctrl);

			if(dhc != null && dhc.DockVisibility)
				dhc.MaximizeController();
		}

		/// <summary>
		/// Restores the specified dockable control.
		/// </summary>
		/// <param name="ctrl">The control instance.</param>
		public void RestoreControl(Control ctrl)
		{
			DockHostController dhc = this.GetDockHostController(ctrl);

			if(dhc != null && dhc.DockVisibility)
			{
				if( dhc.Maximized )
					dhc.RestoreController();
				else
					dhc.Minimized = Minimization.None;
			}
		}

		/// <summary>
		/// Activates the specified dockable control.
		/// </summary>
		/// <param name="ctrl">The control instance.</param>
		/// <remarks>
		/// If the control is in the AutoHide mode or is part of a tabbed docking group, then invoking
		/// this method will bring the control to the foreground and set focus to it.
		/// </remarks>
		public void ActivateControl(Control ctrl)
		{
			DockHostController dhc = this.GetDockHostController(ctrl);
			if (dhc == null)
			{
				Debug.Assert(false, "Error: Control has not been enabled for docking.\n");
				return;
			}

			//<Fix defectid=2244>
			if (ctrl != null)
				if (ctrl.Parent is DockingWrapperForm)
				{
					((DockingWrapperForm)ctrl.Parent).Activate();
					SetActiveControl(ctrl);
				}
			//</Fix>
			if (dhc.DockVisibility == true)
			{
				bool bNeedAnimate = true;
				AHTabPage ahSelectedPage = null;
				if (ahTabAnimate != null)
				{
					ahSelectedPage = ahTabAnimate.SelectedTab as AHTabPage;
					if (ahSelectedPage != null)
					{
						ahTabAnimate.HideController(ahSelectedPage.m_dhcClient, false, true);
					}
					bNeedAnimate = false;
				}
				if (dhc.AutoHideMode == true)
				{
					//Commented the below condition to fix the issue with AcitvateControl method does not work when we call it more than one time.
					//if(dhc.HostControl.Visible == false)
					//{
					if (this.autoHideTabInView != null)
					{
						bNeedAnimate = false;
						if (autoHideTabInView.Edge == dhc.DICurrent.dStyle)
						{
							ahSelectedPage = autoHideTabInView.SelectedTab as AHTabPage;
							if (ahSelectedPage != null && ahSelectedPage.m_dhcClient == dhc)
							{
								autoHideTabInView.ShowController(dhc, false);
								if (ctrl != null && ctrl.ContainsFocus == false)
								{
									ctrl.Focus();
									SetActiveControl(ctrl);
								}
								return;
							}
						}
					}

					DockingStyle ahborder;
					if (dhc.ParentController is DockTabController)
						ahborder = (dhc.ParentController as DockTabController).DINew.dStyle;
					else
						ahborder = dhc.DINew.dStyle;

					// The control is in the auto-hideItem mode; Invoke ShowController and set focus to the control.
					AHTabControl ahtab = this.dcHostForm.GetAHTabControl(ahborder);
					DockStateControllerBase dchost = null;
					if ((dhc.ParentController != null) && (dhc.ParentController is DockTabController))
					{
						DockTabController dtc = dhc.ParentController as Syncfusion.Windows.Forms.Tools.DockTabController;
						if (dtc.SelectedController != dhc)
						{
							// Set the autohide tab subgroup index as the selected subindex
							int nsubindex = -1;
							foreach (TabGroupData groupdata in ahtab.TabsData)
							{
								if (groupdata.Items.Count > 1)
								{
									foreach (TabGroupItem subitem in groupdata.Items)
									{
										if (subitem.Tag == dhc)
										{
											// Set the subindex as selected.
											nsubindex = groupdata.Items.IndexOf(subitem);
											groupdata.SelectedIndex = nsubindex;
											break;
										}
									}
								}
								if (nsubindex >= 0)
									break;
							}
							dtc.TabControl.SelectedIndex = nsubindex;
							dtc.HostControl.Visible = false;	// Setting the selectedindex, sets Visibility to TRUE.
						}
						dchost = dtc;
					}
					else
						dchost = dhc;

					ahtab.ShowController(dchost, bNeedAnimate, true);
					this.AHInViewTab = ahtab;
					//}
				}
				else	// If the control is a non-active tab, activate it.
				{
					if ((dhc.ParentController != null) && (dhc.ParentController is DockTabController))
					{
						DockTabController dtc = dhc.ParentController as DockTabController;
						if (dtc.SelectedController != dhc)
							dtc.SelectedController = dhc;
					}
				}
				if (ctrl != null && ctrl.ContainsFocus == false)
				{
					ctrl.Focus();
					SetActiveControl(ctrl);
				}
				if (autoHideTabInView != null && !dhc.bInAutoHide)
				{
					autoHideTabInView.MFMouseMessageHandler(Point.Empty);
				}
			}
		}

		/// <summary>
		/// Hides the locked autohidden control.
		/// </summary>
		/// <remarks>
		/// If an autohidden control is visible and in the locked mode, then invoking this method will
		/// unlock and hideItem the control.
		/// </remarks>
		/// <param name = "animate"> Indicates whether the locked autohidden control should 
		/// be hidden with an animation.</param>
		public void HideAutoHiddenControl(bool animate)
		{
			if (this.autoHideTabInView != null)
			{
				this.autoHideTabInView.HideController(null, animate, true);
				this.autoHideTabInView = null;
				this.HostControl.Focus();
			}
		}
		/// <summary>
		/// Hides the locked autohidden control.
		/// </summary>
		public void HideAutoHiddenControl()
		{
			this.HideAutoHiddenControl(false);
		}
		/// <summary>
		/// Transfers the dockable control into or out of the autohide mode.
		/// </summary>
		/// <param name="ctrl"> The dock-enabled control.</param>
		/// <param name="bautohide"> Indicates whether the control is set in autohide mode.</param>
		public void SetAutoHideMode(Control ctrl, bool bautohide)
		{
			DockHostController dhc = GetDockHostController(ctrl);
			if (dhc == null)
			{
				Debug.Assert(false, "Error: Control has not been enabled for docking.\n");
				return;
			}
			if ((dhc.Floating == false) && (dhc.bInMDIMode == false) )
			{
				// Fix for Wachovia 2/7/06
				// ToggleAutoHideMode() in DockHostController assumes that the controller has already been shown,
				// since most of its calls occur after the user clicks on the pin.
				if (bautohide == false && dhc.AutoHideMode == true)
				{
					DockingStyle ahborder;
					if (dhc.ParentController is DockTabController)
						ahborder = (dhc.ParentController as DockTabController).DINew.dStyle;
					else
						ahborder = dhc.DINew.dStyle;
					AHTabControl ahtab = this.dcHostForm.GetAHTabControl(ahborder);
					if (ahtab != null)
					{
						// An active window is in AutoHideState but it is visible in the form and not in pinned state. 
						// Hide the active window before active another autohided window
						if (this.GetAutoHideMode(this.ActiveControl))
							this.HideAutoHiddenControl();
						ahtab.ShowController(dhc, false);
					}
				}
				if (bautohide && AHInViewTab != null)
					AHInViewTab.StopAllAnimations();
				if( dhc.DockVisibility == true )
				{
					dhc.AutoHideMode = bautohide;
				}
				else
				{
					dhc.AutoHiddedBeforeHide = bautohide;
				}
			}
		}

        /// <summary>
        /// Transfers the dockable control into or out of the autohide mode.
        /// </summary>
        /// <param name="ctrl"> The dock-enabled control.</param>
        /// <param name="bautohide"> Indicates whether the control is set in autohide mode.</param>
        /// <param name="singleOperate"> Indicates whether only the specified control should go into autohide mode, if tabbed.</param>
        public void SetAutoHideMode(Control ctrl, bool autoHide, bool singleOperate)
        {
            ToggleAHState(ctrl, !this.GetAutoHideMode(ctrl), singleOperate, true);
        }

		/// <summary>
		/// Indicates the autohide mode of the control.
		/// </summary>
		/// <param name="ctrl"> The dockable control for which the autohide mode is being queried.</param>
		/// <returns> TRUE if the control is in autohide.</returns>
		public bool GetAutoHideMode(Control ctrl)
		{
			DockHostController dhc = GetDockHostController(ctrl);
			if (dhc == null)
			{
				Debug.Assert(false, "Error: Control has not been enabled for docking.\n");
				return false;
			}
			if( dhc.DockVisibility == true )
			{
				return dhc.AutoHideMode;
			}
			else
			{
				return dhc.AutoHiddedBeforeHide;
			}
		}

		private void SetDHCDockVisibility( DockHostController dhc, bool bVisible, bool bFireEvent )
		{
			bool prevVisChanging = this.bFiredVisibilityChangingEvent;
			bool prevVisChanged = this.bFiredVisibilityChangedEvent;
			this.bFiredVisibilityChangingEvent = !bFireEvent;
			this.bFiredVisibilityChangedEvent = !bFireEvent;
			dhc.DockVisibility = bVisible;
			this.bFiredVisibilityChangingEvent = prevVisChanging;
			this.bFiredVisibilityChangedEvent = prevVisChanged;
		}

        /// <summary>
        /// Sets the control as an MDI child.
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="bsetmdi"></param>
		public virtual void SetAsMDIChild(Control ctrl, bool bsetmdi)
		{
			SetAsMDIChild(ctrl, bsetmdi, Rectangle.Empty);
		}
		public virtual void SetAsMDIChild(Control ctrl, bool bsetmdi, Rectangle layout)
		{
			if (((this.HostControl is Form) == false) || ((this.HostControl as Form).IsMdiContainer == false))
			{
				Debug.Assert(false, "The DockingManager's HostControl has to be an MDIContainer for this functionality.\n");
				return;
			}
			DockHostController dhc = null;
			ArrayList targets = new ArrayList(this.alTargetManagers);
			targets.Add(this);

			foreach (DockingManager manager in targets)
			{
				dhc = manager.GetDockHostController(ctrl);

				if (dhc != null)
					break;
			}

			if (dhc == null)
			{
				Debug.Assert(false, "Error: Control has not been enabled for docking.\n");
				return;
			}

			SetAsMdiChild( dhc, bsetmdi, layout );
		}

        private void UpdateMDIChildIcon(Control ctrl)
        {
            if (!this.DesignMode)
            {
                DockHostController dhc = this.GetDockHostController(ctrl);
                // Create the DockingWrapperForm and add the control to the form
                if (dhc != null && this.IsMDIMode(ctrl))
                {
                    DockingWrapperForm mdiform = ctrl.Parent as DockingWrapperForm;
                    if (mdiform != null&&mdiform.Name.Contains(ctrl.Name))
                    {
                        mdiform.Text = dhc.HostControl.Text;
                        Rectangle mdiFormBounds = new Rectangle(dhc.MdiChildBounds.Location, dhc.MdiChildBounds.Size);

                        if ((this.ilDockTabs != null) && (dhc.MdiImageIndex < this.ilDockTabs.Images.Count))
                        {
                            if ((dhc.MdiImageIndex >= 0))
                            {
                                Bitmap iconbmp = new Bitmap(this.ilDockTabs.Images[dhc.MdiImageIndex], this.ImageList.ImageSize);
                                IntPtr intPtr = iconbmp.GetHicon();
                                Icon oldIcon = mdiform.Icon;
                                Icon newIcon = Icon.FromHandle(intPtr);
                                mdiform.Icon = newIcon.Clone() as Icon;
                                iconbmp.Dispose();
                                NativeMethods.DestroyIcon(newIcon.Handle);
                                NativeMethods.DestroyIcon(oldIcon.Handle);
                                
                            }
                            else if (dhc.ImageIndex >= 0)
                            {
                                Bitmap iconbmp = new Bitmap(this.ilDockTabs.Images[dhc.ImageIndex], this.ImageList.ImageSize);
                                IntPtr intPtr = iconbmp.GetHicon();
                                Icon oldIcon = mdiform.Icon;
                                Icon newIcon = Icon.FromHandle(intPtr);
                                mdiform.Icon = newIcon.Clone() as Icon;
                                iconbmp.Dispose();
                                NativeMethods.DestroyIcon(newIcon.Handle);
                                NativeMethods.DestroyIcon(oldIcon.Handle);
                            }
                        }
                    }
                }
            }
        }
		internal void SetAsMdiChild( DockHostController dhc, bool bsetmdi, Rectangle layout )
		{
			Control ctrl = null;

			if( dhc.bInMDIMode == bsetmdi )
				return;

			if( bsetmdi )
				ctrl = dhc.HostControl.Controls[0];
			else
				ctrl = dhc.ctrlReference;
			Control[] ctrls = new Control[] { ctrl };
			this.FireDockStateChangeEvent("DockStateChanging", new DockStateChangeEventArgs(ctrls));
			bool prevFreezeDockStateChangeEvents = this.bFreezeDockStateChangeEvents;
			this.bFreezeDockStateChangeEvents = true;

            bool prevCancelVisibilityChangingEvent = this.bCancelVisibilityChangingEvent;
            this.bCancelVisibilityChangingEvent = false;

			Form hostform = this.HostControl as Form;
			if (bsetmdi == true)
			{
				// Determine whether the DockingWrapperForm is created and skip all the next steps if it's so
				foreach (Control dockctrl in alEnableDocking)
				{
					if (dockctrl == ctrl)
						if (dockctrl.Parent is DockingWrapperForm)
						{
							bHoldEvents = false;
							return;
						}
				}

				// Reactivate the MainForm - Workaround for Windows Forms MDI child activation bug
				hostform.Activate();
				dhc.FloatOnly = false;

				// Hide the dockhostcontroller and set it's mdi state
				FloatingForm ff = dhc.HostControl.Parent as FloatingForm;
				if (ff != null)
				{
					DockTabController parent = dhc.ParentController as DockTabController;
					if (parent != null)
						parent.CheckFormOwnership(dhc);
					else
						PerformFormsExchange(dhc); 
					dhc.AssignFormBelongings(ff);
				}
				dhc.bInMDIMode = true;
				SetDHCDockVisibility(dhc, false, false);
				dhc.ctrlReference = ctrl;

				// Create the DockingWrapperForm and add the control to the form
				DockingWrapperForm mdiform = new DockingWrapperForm(this, ctrl);
				mdiform.Text = dhc.HostControl.Text;
                Rectangle mdiFormBounds = new Rectangle(dhc.MdiChildBounds.Location, dhc.MdiChildBounds.Size);

                if ((this.ilDockTabs != null))
				{
                    if (dhc.MdiImageIndex >= 0 && dhc.MdiImageIndex < this.ilDockTabs.Images.Count)
                    {
                        Bitmap iconbmp = new Bitmap(this.ilDockTabs.Images[dhc.MdiImageIndex], this.ImageList.ImageSize);
                        mdiform.Icon = Icon.FromHandle(iconbmp.GetHicon());
                        iconbmp.Dispose();
                    }
                    else if (dhc.ImageIndex >= 0 && dhc.ImageIndex < this.ilDockTabs.Images.Count)
                    {
                        Bitmap iconbmp = new Bitmap(this.ilDockTabs.Images[dhc.ImageIndex], this.ImageList.ImageSize);
                        mdiform.Icon = Icon.FromHandle(iconbmp.GetHicon());
                        iconbmp.Dispose();
                    
                    }
				}
				if ((this.CloseEnabled == false) || (dhc.CloseButtonVisibility == false))
				{
					mdiform.ControlBox = false;
					mdiform.MinimizeBox = true;
					mdiform.MaximizeBox = true;
				}
				mdiform.Controls.Add(ctrl);
				mdiform.WindowState = dhc.MdiWindowState;
				ctrl.Dock = System.Windows.Forms.DockStyle.Fill;
				mdiform.MdiParent = hostform;
				mdiform.ColorScheme = this.Office2007MdiColorScheme;

				// Forbid mdiform to catch focus using Tab key
				mdiform.TabStop = false;
				ctrl.Visible = true;

				// workaround for defect 675
				// If docked control is form, its Show method crashes
				// Need to send WM_NCACTIVATE message
                //if (ctrl is Form)
                //{
                //    NativeMethods.SendMessage(mdiform.Handle, NativeMethods.WM_NCACTIVATE,
                //        (IntPtr)1, IntPtr.Zero);
                //}

                try
                {
                    mdiform.Show();
                }
                catch
                { }

                if( mdiFormBounds != Rectangle.Empty )
                {
                    mdiform.Location = mdiFormBounds.Location;
                    mdiform.Size = mdiFormBounds.Size;
                }

                if (layout != Rectangle.Empty)
				{
					mdiform.Location = layout.Location;
					mdiform.Size = layout.Size;
				}
				mdiform.Activate();

				mdiform.dockable.Click += new EventHandler(this.MDIFormDockableItem_Click);
				mdiform.hideItem.Click += new EventHandler(this.MDIFormHideItem_Click);
				if (mdiform.floating != null)
					mdiform.floating.Click += new EventHandler(this.MDIFormFloatingItem_Click);
			}
			else
			{
				DockingWrapperForm mdiform = ctrl.Parent as DockingWrapperForm;
				if (mdiform != null)
				{
					dhc.HostControl.Controls.Add(ctrl);
					dhc.MdiWindowState = mdiform.WindowState;
					dhc.MdiChildBounds = mdiform.Bounds;

					// Restore menu for MDIChild form.
					Form frmMDIChild = ctrl as Form;

					if( frmMDIChild != null )
						frmMDIChild.Menu = mdiform.Menu;

					bAllowActivationEvents = bMdiFormClosing;
					mdiform.Hide();
					mdiform.MdiParent = null;
					if( bAllowActivationEvents )
						bAllowActivationEvents = false;

					mdiform.dockable.Click -= new EventHandler(this.MDIFormDockableItem_Click);
					mdiform.hideItem.Click -= new EventHandler(this.MDIFormHideItem_Click);

					dhc.bInMDIMode = false;
					dhc.ctrlReference = null;
					ctrl.Size = dhc.HostControl.ClientRectangle.Size;
					ctrl.Anchor = AnchorStyles.Left | AnchorStyles.Top;
					ctrl.Dock = DockStyle.None;

					SetDHCDockVisibility(dhc, true, false);

					if( !dhc.Closing && ctrl.TopLevelControl != null )
						Syncfusion.Runtime.InteropServices.NativeMethods.SetFocus( ctrl.Handle );

					if (mdiform.floating != null)
						mdiform.floating.Click -= new EventHandler(this.MDIFormFloatingItem_Click);
					if (mdiform.bInCloseProc == false)	// Else, already in Close proc.
						mdiform.Close();
					lastActivated = ctrl;
					bAllowActivationEvents = true;
				}
				UpdateFloatingFormsImages();
			}
			dhc.DIPrevious = dhc.DITransient;

            this.bCancelVisibilityChangingEvent = prevCancelVisibilityChangingEvent;
			this.bFreezeDockStateChangeEvents = prevFreezeDockStateChangeEvents;
			this.FireDockStateChangeEvent("DockStateChanged", new DockStateChangeEventArgs(ctrls));
		}

		/// <summary>
		/// Indicates whether the specified control is in MDI Child mode or not.
		/// </summary>
		/// <param name="ctrl">Instance of a control.</param>
		/// <returns>
		/// 	<c>true</c> if the specified control is in MDI Child mode; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>Control must be enabled for docking. </remarks>
		public bool IsMDIMode(Control ctrl)
		{
			DockHostController dhc = GetDockHostController(ctrl);
			if (dhc == null)
			{
				throw new ArgumentException(String.Format("Control {0} is not enable for docking.", ctrl.Name));
			}
			return dhc.bInMDIMode;
		}
       
		/// <summary>
		/// Indicates the dock/float state of the dockable control.
		/// </summary>
		/// <param name="ctrl"> The control for which the dock/float state is being queried.</param>
		/// <returns> TRUE if the control is floating.</returns>
		public bool IsFloating(Control ctrl)
		{
			DockHostController dhc = GetDockHostController(ctrl);
			if (dhc == null)
			{
				Debug.Assert(false, "Error: Control has not been enabled for docking.\n");
				return false;
			}
			return dhc.Floating;
		}
		/// <summary>
		/// Returns the current docking style of the control.
		/// </summary>
		/// <param name="ctrl">The Instance of a control.</param>
		/// <returns>A <see cref="Syncfusion.Windows.Forms.Tools.DockingStyle"/> value that specifies the dock type\position.</returns>
		/// <remarks>Control must be enabled for docking. It will return DockingStyle.Fill for Floating state and Tabbed group. </remarks>
		public DockingStyle GetDockStyle(Control ctrl)
		{
			DockHostController dhc = GetDockHostController(ctrl);
			if (dhc == null)
			{
				throw new ArgumentException(String.Format("Control {0} is not enable for docking.", ctrl.Name));
			}
			return dhc.DICurrent.dStyle;
		}


		/// <summary>
		/// Returns array of controls which are tabbed with the given control.
		/// </summary>
		/// <param name="ctrl"> 
		/// The instance of control whose tabbed siblings are to be returned
		/// </param>
		/// <returns> Array of controls </returns>
		public Control[] GetTabbedSiblings(Control ctrl)
		{
			DockHostController dhc = GetDockHostController(ctrl);
			//If the control is not enabled for docking raise exception
			if (dhc == null)
				throw new ArgumentException(String.Format("Control {0} is not enable for docking.",
					ctrl.Name));

			DockTabController dTabCtrlr = dhc.ParentController as DockTabController;
			//If the control is not tabbed raise exception
			if (dTabCtrlr == null)
				throw new ArgumentException("Control {0} is not tabbed", ctrl.Name);

			ArrayList ctrls = new ArrayList();
			DockTabControl dTabCtrl = dTabCtrlr.TabControl;
			//Iterate through all the DockTabPages and add the associated control into arraylist.
			foreach (DockTabPage tabpage in dTabCtrl.TabPages)
				ctrls.Add(tabpage.dhcClient.HostControl.Controls[0]);
			return ctrls.ToArray(typeof(Control)) as Control[];
		}
		/// <summary>
		/// Determines whether the second control is under the same group of the first control.
		/// </summary>
		/// <param name="ctrl1">The instance of a control.</param>
		/// <param name="ctrl2">The instance of a control.</param>
		/// <returns>
		/// 	<c>true</c> if the second control is under the first control tab group ; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>
		/// Both controls must be a part of tab group, otherwise it will return false.
		/// </remarks>
		public virtual bool IsSameTabbedGroup(Control ctrl1, Control ctrl2)
		{

			DockHostController dhc = GetDockHostController(ctrl1);
			if (dhc == null)
				return false;
			//Checks whether the control is under the DockTabControl
			if (dhc.ParentController is DockTabController)
			{
				DockTabControl docktab = (dhc.ParentController as DockTabController).TabControl;
				if (docktab == null)
					return false;
				//Iterates through all sibiling controls of a first control.
				foreach (DockTabPage tabpage in docktab.TabPages)
				{
					//Returns true if any one control is equal to the second control.
					if (ctrl2 == tabpage.dhcClient.HostControl.Controls[0])
						return true;
				}
			}
			return false;
		}
		/// <summary>
		/// Gets the tab position of the specified control.
		/// </summary>
		/// <param name="control">The instance of a control.</param>
		///	<returns>An Integer value that specifies the tab position of the control.</returns>
		/// <remarks>Control must be part of tab group.</remarks>
		public int GetTabPosition(Control control)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}

			DockHostController hostController = GetDockHostController(control);
			if (hostController == null)
			{
				throw new ArgumentException(String.Format("Control {0} is not enable for docking.",
					control.Name));
			}

			DockTabController tabController = hostController.ParentController as DockTabController;
			if (tabController == null)
			{
				throw new ArgumentException("Control {0} is not tabbed", control.Name);
			}

			DockTabControl tabControl = tabController.TabControl as DockTabControl;
			if (tabControl.TabPages.Count <= 0)
			{
				throw new ArgumentException("Control {0} is not tabbed", control.Name);
			}

			DockTabPage currentPage = null;
			//Iterates to find the corresponding tab page for the specified file.
			foreach (DockTabPage page in tabControl.TabPages)
			{
				if (page.dhcClient == hostController)
				{
					currentPage = page;
					break;
				}
			}

			//Returns the tab position of the control. 
			return (tabControl.TabPages.IndexOf(currentPage));
		}
		/// <summary>
		/// Sets the docked control's position within tab group.
		/// </summary>
		/// <param name="control">The control instance.</param>
		/// <param name="newPosition">New position of control's page.</param>
		/// <remarks>
		/// Control must be part of tab group. newPosition must be valid page index of tab group.
		/// </remarks>
		public void SetTabPosition(Control control, int newPosition)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}

			DockHostController hostController = GetDockHostController(control);
			if (hostController == null)
			{
				throw new ArgumentException(String.Format("Control {0} is not enable for docking.",
					control.Name));
			}

			DockTabController tabController = hostController.ParentController as DockTabController;
			if( null != tabController )
			{
				DockTabControl tabControl = tabController.TabControl as DockTabControl;
				if ((newPosition < 0) || (tabControl.TabPages.Count <= newPosition))
				{
					throw new ArgumentException("Position index is invalid");
				}

				DockTabPage currentPage = null;
				foreach (DockTabPage page in tabControl.TabPages)
				{
					if (page.dhcClient == hostController)
					{
						currentPage = page;
						break;
					}
				}

				//Assign source page index
				int index = tabControl.TabPages.IndexOf(currentPage);
				//Assign current selected page index
				int selectedIndex = tabControl.SelectedIndex;

				if (index != newPosition)
				{
					tabController.PauseActivation = true;
					DockTabPage tempPage = tabControl.TabPages[index] as DockTabPage;

					//Checks whether the page moves to forward direction 
					if (index < newPosition)
					{

						//Iterate all pages to get the target location
						for (int i = 0; i < tabControl.TabPages.Count; i++)
						{
							if (i < index) continue;
							if (i != newPosition)
								tabControl.TabPages[i] = tabControl.TabPages[i + 1];
							else
							{
								tabControl.TabPages[i] = tempPage;
								break;
							}
						}
					}
						//Checks whether the page moves to backward direction 
					else if (index > newPosition)
					{
						//Iterate all pages to get the target location
						for (int i = tabControl.TabPages.Count - 1; i >= 0; i--)
						{
							if (i > index) continue;
							if (i != newPosition)
								tabControl.TabPages[i] = tabControl.TabPages[i - 1];
							else
							{
								tabControl.TabPages[i] = tempPage;
								break;
							}
						}
					}
					//Activates the moved page in target location
					tabControl.SelectedIndex = newPosition;
					tabController.PauseActivation = false;

				}

				if (selectedIndex != index)
				{
					tabController.SetHostCtrlForSelection();
				}
			}
		}
		/// <summary>
		/// Indicates whether the specified control is tabbed or not.
		/// </summary>
		/// <param name="ctrl">The Instance of a control.</param>
		/// <returns>
		/// 	<c>true</c> if the specified control is tabbed; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>Control must be enabled for docking. </remarks>
		public bool IsTabbed(Control ctrl)
		{
			if (ctrl == null)
			{
				throw new ArgumentNullException("control");
			}

			DockHostController hostController = GetDockHostController(ctrl);
			if (hostController == null)
			{
				throw new ArgumentException(String.Format("Control {0} is not enable for docking.",
					ctrl.Name));
			}

			DockTabController tabController = hostController.ParentController as DockTabController;
			if (tabController != null)
			{
				return true;
			}
			return false;
		}

        /// <summary>
        /// If contextmenu is enabled, then displays the Syncfusion XP menus.
        /// Else fires the contextmenu event with a null popup menu.
        /// </summary>
        /// <param name="dhc"></param>
        /// <param name="pt"></param>
        [
        Syncfusion.Documentation.DocumentationExclude(),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
		public void ShowMenu(DockHostController dhc, Point pt)
		{
			ShowMenu( dhc, pt, false );
		}

		protected internal void ShowMenu( DockHostController dhc, Point pt, bool singleTabOperate )
		{
			// If contextmenu is enabled, then display the Syncfusion XP menus.
			// Else fire the contextmenu event with a null popup menu.
			Control owner = null;
			m_singleTabOperate = singleTabOperate;

			if( dhc.bInMDIMode )
			{
				owner = dhc.ctrlReference;
				DockingWrapperForm dwf = owner.Parent as DockingWrapperForm;

				if( dwf != null )
					dwf.InitializeMenu();
			}
			else
			{
				owner = dhc.HostControl.Controls[0];

				if( this.bEnableContextMenu == true )
				{
					PopupMenu menu = new PopupMenu();
					menu.ParentBarItem = new ParentBarItem();
					this.InitializeDefaultMenu(dhc, menu);
					DockContextMenuEventArgs dcmeargs = new DockContextMenuEventArgs(owner, menu);
					this.OnDockContextMenu(dcmeargs);
					if( dcmeargs.ContextMenu != null )
						dcmeargs.ContextMenu.Show(owner, owner.PointToClient(pt));
				}
				else
				{
					this.OnDockContextMenu(new DockContextMenuEventArgs(owner, null));
				}
			}
		}
		/// <summary>
		/// Shows the Docking caption context menu at the specified point.
		/// </summary>
		/// <param name="ctrl">Instance of a control</param>
		/// <param name="pt">The location of the menu to be displayed.</param>
		public void ShowMenu(Control ctrl, Point pt)
		{
			DockHostController dhc = GetDockHostController(ctrl);
			if( !ctrl.Visible )
			{
				throw new DockingManagerException("Can not show context menu for invisible control");
			}
            if (dhc == null && this.DockBehavior == Tools.DockBehavior.VS2010 && ctrl is FloatingForm)
            {
                dhc = GetFloatingDockHostController(ctrl);
            }
			if (dhc == null)
			{
				throw new DockingManagerException(String.Format("Control {0} is not enable for docking.", ctrl.Name));
			}
			ShowMenu(dhc, pt);
		}


		/// <summary>
		/// Adds the DockingManager to the target providers list belonging to the current manager.
		/// </summary>
		/// <param name="dockingmgr">The DockingManager to be added to the target list.</param>
		/// <remarks>
		/// Specifying a DockingManager as a target provider by adding it to another DockingManager's target list
		/// allows controls from the source manager to be dragged and docked onto the docking layout hosted
		/// by the target manager.
		/// <seealso cref="DockingManager.RemoveFromTargetManagersList"/>
		/// <seealso cref="DockingManager.TransferringFromManager"/>
		/// <seealso cref="DockingManager.TransferredToManager"/>
		/// </remarks>
		public void AddToTargetManagersList(DockingManager dockingmgr)
		{
			if ((dockingmgr != this) && (this.alTargetManagers.Contains(dockingmgr) == false))
				this.alTargetManagers.Add(dockingmgr);
		}

		/// <summary>
		/// Removes the DockingManager from the target providers list belonging to the current manager.
		/// </summary>
		/// <param name="dockingmgr">The DockingManager to be removed from the target list.</param>
		/// <remarks>
		/// Specifying a DockingManager as a target provider by adding it to another DockingManager's target list
		/// allows controls from the source manager to be dragged and docked onto the docking layout hosted
		/// by the target manager.
		/// <seealso cref="DockingManager.AddToTargetManagersList"/>
		/// <seealso cref="DockingManager.TransferringFromManager"/>
		/// <seealso cref="DockingManager.TransferredToManager"/>
		/// </remarks>
		public void RemoveFromTargetManagersList(DockingManager dockingmgr)
		{
			if (this.alTargetManagers.Contains(dockingmgr) == true)
				this.alTargetManagers.Remove(dockingmgr);
		}

        /// <summary>
        /// Serves to remove the specified controller from the docking manager.
        /// </summary>
        /// <param name="dhc">
        /// Dock host controller
        /// </param>
        [
        Syncfusion.Documentation.DocumentationExclude(),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
		public void RemoveControllerFromDockingManager(DockHostController dhc)
		{
			if( dhc.InternalFloatWrapper != null && dhc.InternalFloatWrapper.DockingManager == this )
				dhc.InternalFloatWrapper = null;
			if( dhc.InternalDockWrapper != null && dhc.InternalDockWrapper.DockingManager == this )
				dhc.InternalDockWrapper = null;

			dhc.SharedForm = null;
			if( dhc.InternalForm != null )
			{
				this.HiddenFFControllers.Remove( dhc.InternalForm.InternalController );
				dhc.InternalForm.Owner = null;
			}

			this.OnTransferringFromManager(new TransferManagerEventArgs(dhc.HostControl.Controls[0]));

			if (this.alDockAreaControllers.Contains(dhc) == true)
				this.alDockAreaControllers.Remove(dhc);
			Control control = dhc.HostControl.Controls[0];
			if (this.alEnableDocking.Contains(control) == true)
				this.alEnableDocking.Remove(control);
			if (this.ControlScopeImages == true)
				this.RemoveDockHostImageFromList(dhc);
			// Remove all previous state information
			if (dhc.Floating == true)
				dhc.DockDCRList.Clear();
			else
				dhc.FloatDCRList.Clear();
			if (dhc.DIPrevious.dController != null)
				dhc.DIPrevious.dController.ControllerChanged -= new ControllerChangedEH(dhc.dhc_ControllerChanged);
			dhc.DIPrevious = DockInfo.NullInfo;
			dhc.DockingManager = null;

			if ((this.DesignMode == false) && (this.alEnableDocking.Count == 0))
			{
				if (DesignerHooks.GetMsgProcListContains(this, m_curentThreadId))
					DesignerHooks.RemoveGetMsgProcListener(this, m_curentThreadId);
			}
		}
        
        /// <summary>
        /// Adds controller to the docking manager.
        /// </summary>
        /// <param name="dhc">
        /// Dock host controller.
        /// </param>
        [
        Syncfusion.Documentation.DocumentationExclude(),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
		public void AddControllerToDockingManager(DockHostController dhc)
		{
			dhc.DockingManager = this;
			Control control = dhc.HostControl.Controls[0];
			if (this.alEnableDocking.Contains(control) == false)
				this.alEnableDocking.Add(control);
			if (this.alDockAreaControllers.Contains(dhc) == false)
				this.alDockAreaControllers.Add(dhc);
            if ((this.ControlScopeImages == true) && (dhc.ControlImage != null))
            {
                this.SetDockIcon(dhc.HostControl.Controls[0], dhc.ControlImage);                
                this.SetMDIChildIcon(dhc.HostControl.Controls[0], dhc.ControlImage);
            }
			if( dhc.InternalForm != null )
			{
				FloatingFormController ffc = dhc.InternalForm.InternalController as FloatingFormController;
				ffc.DockingManager = this;
				this.HiddenFFControllers.Add( ffc );
			}

			if ((this.DesignMode == false) && (this.alEnableDocking.Count > 0))
			{
				if (DesignerHooks.GetMsgProcListContains(this) == false)
					m_curentThreadId = DesignerHooks.AddGetMsgProcListener(this);
			}

			this.OnTransferredToManager(new TransferManagerEventArgs(dhc.HostControl.Controls[0]));
		}


		/// <summary>
		/// Sets the RTL property for the specified control. 
		/// </summary>
		/// <param name="ctrl">The dock-enabled control.</param>
		/// <param name="bRTL">TRUE indicates that the control is set as Mirrored; FALSE to disable Mirrored for a specified control.</param>
		public void SetIsMirrored(Control ctrl, bool bRTL)
		{
			DockHostController dhc = GetDockHostController(ctrl);
			if (dhc != null)
			{
				if (dhc.IsMirrored != bRTL)
					dhc.IsMirrored = bRTL;
			}
		}
		/// <summary>
		/// Forces the host form to recalculate it's layout.
		/// </summary>
		public void RecalcHostFormLayout()
		{
			//Rectangle rclayout = this.dcHostForm.LayoutRect;
			//this.dcHostForm.LayoutRect = new Rectangle(rclayout.Left, rclayout.Top, rclayout.Width + 1, rclayout.Height + 1);
			//this.dcHostForm.LayoutRect = rclayout;
			this.dcHostForm.AdjustLayoutDockArea();
			this.dcHostForm.AdjustLayout();
		}

        /// <summary>
        /// Restricts dock fill auto hide border.
        /// </summary>
        /// <param name="style"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        [
        EditorBrowsable(EditorBrowsableState.Never)
        ]
		public void RestrictDockFillAutoHideBorder(DockingStyle style)
		{
			if (this.DockToFill == false)
			{
				Debug.Assert(false, "This method call is valid only when the DockingManager.DockToFill option is set.");
				return;
			}
			this.dsDockFillAHBorder = style;
		}
       
		/// <summary>
		/// Returns the serialized controls collection enumerator in the specified Serializer.
		/// </summary>
		public IEnumerator GetSerializedControls(AppStateSerializer serializer)
		{
			try
			{
				String strpersist = String.Concat(this.PersistenceID, this.PersistKey);
				DockingMgrSerializationWrapper dmgrserializer = serializer.DeserializeObject(strpersist) as DockingMgrSerializationWrapper;

				if (dmgrserializer != null)
					return dmgrserializer.htDHCWrapper.Keys.GetEnumerator();
			}
			catch (Exception)
			{
				// Do Nothing
			}
			return null;
		}

        /// <summary>
        /// Specifies whether control contains serialization information.
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="ctrl"></param>
        /// <returns></returns>
		public bool ContainsSerializationInfo(AppStateSerializer serializer, Control ctrl)
		{
			DHCSerializationWrapper dhcwrapper = null;

			try
			{
				// First attempt to look for persistence information for this control in the DockingManager's serialization data
				// for the entire control set.
				String strpersist = String.Concat(this.PersistenceID, this.PersistKey);
				DockingMgrSerializationWrapper dmgrserializer = serializer.DeserializeObject(strpersist) as DockingMgrSerializationWrapper;

				if (dmgrserializer != null)
					dhcwrapper = dmgrserializer.GetDHCSerializationWrapper(ctrl.Name);

				if (dhcwrapper == null)
				{
					// See if the dock state information for this control has been stored individually.
					strpersist = String.Concat(this.PersistenceID, this.PersistKey, ctrl.Name);
					dhcwrapper = serializer.DeserializeObject(strpersist) as DHCSerializationWrapper;
				}
			}
			catch (Exception)
			{
				// Do Nothing
			}

			return (dhcwrapper != null);
		}

        /// <summary>
        /// Applies deserialized state to the control.
        /// </summary>
        /// <param name="dmgrserializer">
        /// Docking manager serialization wrapper
        /// </param>
        [Syncfusion.Documentation.DocumentationExclude(),
        EditorBrowsable(EditorBrowsableState.Advanced)
        ]
		public virtual void ApplyDeserializedState(DockingMgrSerializationWrapper dmgrserializer)
		{
			m_bForbidWrapperLogic = true;
			StopAutoHideAnimation();

			ControllerWrapper cw = dmgrserializer.controllerWrapper;
			foreach (String ctrlname in dmgrserializer.htDHCWrapper.Keys)
			{
				bool bcontrolinitialized = false;
				foreach (Control enabledcontrol in this.alEnableDocking)
				{
					if (enabledcontrol.Name == ctrlname)
					{
						bcontrolinitialized = true;
						break;
					}
				}
				if (bcontrolinitialized == false)
				{
					this.OnInitializeControlOnLoad(new InitializeControlOnLoadEventArgs(ctrlname));
				}
			}

			this.dcHostForm.bOverlapSizing = false;	// Do not allow the MainFormController to do the overlap avoidance size reduction.

			Array dcarray = this.alDockAreaControllers.ToArray(typeof(DockControllerBase));

			// Run through the controller list and un-autohide and un-hideItem all dockhostcontrollers. Follow this up
			// with a redock/refloat of all controllers housed within docktabs. DockHostControllers need to be visible
			// and have unique dock/float states when applying the persisted layout state.
			foreach (DockControllerBase dcbase in dcarray)
			{
				if (dcbase is DockHostController)
				{
					DockHostController dhc = dcbase as DockHostController;

					DHCSerializationWrapper dhcwrapper = dmgrserializer.GetDHCSerializationWrapper(dhc);
					if (dhcwrapper == null)
					{
						// Serialized information is not available. Fire the DockStateUnavailable event and hide the control.
						if (dhc.HostControl.Controls.Count > 0)
							this.OnDockStateUnavailable(new DockStateUnavailableEventArgs(dhc.HostControl.Controls[0]));
						if( dhc.bInMDIMode )
							SetAsMdiChild( dhc, false, Rectangle.Empty );
						dhc.DockVisibility = false;
						continue;
					}

					if (dhcwrapper.uniqueName != null)
						dhc.UniqueName = dhcwrapper.uniqueName;
					dhc.DockEdge = dhcwrapper.dockEdge;

					if (dhc.bInMDIMode == true)
					{
						if (dhc.ctrlReference != null)
							this.SetAsMDIChild(dhc.ctrlReference, false);
						else
							dhc.bInMDIMode = false;
					}
					if( dhc.DockVisibility == false )
						dhc.DockVisibility = true;
					if (dhc.FloatOnly == true)
						dhc.FloatOnly = false;
					if (dhc.AutoHideMode == true)
						dhc.ToggleAutoHideMode();
					if (dhc.ParentController is DockTabController)
					{
						DockTabController dtc = dhc.ParentController as DockTabController;
						dtc.RemoveDockHostFromTab(dhc, false);
					}

					dhc.FloatDCRList.Clear();
					dhc.DockDCRList.Clear();

					if (this.DesignMode == false)
					{
						if (dhc.Floating == true)
							dhc.HostControl.Parent.Visible = false;
						else dhc.HostControl.Visible = false;
					}
				}
			}

			this.bLoadVisibility = false;	// Keeps dockhosts and floatingforms hidden till deserialization is complete

			// restore autohide index
			foreach (DockControllerBase dcbase in alDockAreaControllers)
			{
				DockHostController dhctrl = dcbase as DockHostController;
				if (dhctrl != null)
				{
					DHCSerializationWrapper dhcwrapper = dmgrserializer.GetDHCSerializationWrapper(dhctrl);
					if (dhcwrapper != null)
						dhctrl.AutoHideIndex = dhcwrapper.autoHideIndex;
				}
			}

			ArrayList alautohide = new ArrayList();
			bool allowFloat = this.DisallowFloating;
			this.DisallowFloating = false;
			foreach (DockControllerBase dcbase in dcarray)
			{
				if (dcbase is DockHostController)
				{
					DockHostController dhc = dcbase as DockHostController;

					DHCSerializationWrapper dhcwrapper = dmgrserializer.GetDHCSerializationWrapper(dhc);
					if (dhcwrapper == null)
						continue;

					// restore dock label
					dhc.DockLabel = dhcwrapper.strLabel;

					// If a currently docked controller is also docked in the persisted info, then undock and redock it
					// using the deserialized dockinfo.	Similarly, for floating controllers, dock and refloat them using
					// the deserialzed float dockinfo. This toggling of states allows the deserialized docking info to
					// be applied on the dockhosts while using their float/dock relationships.
					if (((dhc.Floating == false) && (dhcwrapper.dockInfoCurrent.DP != DockPreference.All))
						|| ((dhc.Floating == true) && (dhcwrapper.dockInfoCurrent.DP == DockPreference.All)))
					{
						if (!dhcwrapper.bInMDIMode)
						{
							dhc.InvokePrevDockFloatTransition(dhc.Floating);
						}
					}

					// Assign the depersisted lists to the dockhostcontroller
					dhc.DockDCRList = dhcwrapper.alDockDCR;
					dhc.FloatDCRList = dhcwrapper.alFloatDCR;
					dhc.InternalFloatWrapper = null;
					dhc.InternalDockWrapper = null;
					dhc.SharedForm = null;

					// Set the mainformcontroller reference
					if (dhcwrapper.dockInfoCurrent.DP != DockPreference.All)
						dhcwrapper.dockInfoCurrent.dController = this.dcHostForm;
					else
					{
						// The DragRectangle size is used while creating a new floating frame
						(dhc.HostControl as DockHost).DragRectangle = dhcwrapper.dockInfoCurrent.rcDockArea;
					}
					if (dhcwrapper.dockInfoPrevious.DP != DockPreference.All)
						dhcwrapper.dockInfoPrevious.dController = this.dcHostForm;
					dhc.DIPrevious = new DockInfo(dhcwrapper.dockInfoCurrent);

					if (!dhcwrapper.bAutoHideMode)
					{
						if (!dhcwrapper.bInMDIMode)
						{
							dhc.InvokePrevDockFloatTransition(true);
						}
						dhc.DIPrevious = new DockInfo(dhcwrapper.dockInfoPrevious);
						dhc.DockVisibility = dhcwrapper.bDockVisibility;
					}

					if (dhcwrapper.bAutoHideMode == true)
					{
						// locate controller index
						int index = alautohide.Count;
						if (dhc.AutoHideIndex != -1)
						{
							foreach (DockHostController dockHostController in alautohide)
							{
								if ((dockHostController.AutoHideIndex >= dhc.AutoHideIndex)
									|| (dockHostController.AutoHideIndex == -1))
								{
									index = alautohide.IndexOf(dockHostController);
									break;
								}
							}
						}

						alautohide.Insert(index, dhc);
					}
					dhc.FloatOnly = dhcwrapper.bFloatOnly;
					dhc.MdiChildState = dhcwrapper.bInMDIMode;
				}
			}
			this.DisallowFloating = allowFloat;
			this.bLoadVisibility = true;

			foreach (DockControllerBase dcbase in dcarray)
			{
				if (dcbase is DockHostController)
				{
					DockHostController dhc = dcbase as DockHostController;

					DHCSerializationWrapper dhcwrapper = dmgrserializer.GetDHCSerializationWrapper(dhc);
					if (dhcwrapper == null)
						continue;

					// restore selected pages in tab groups
					if (dhcwrapper.bSelectedPage)
					{
						DockTabController dockTabController = dhc.ParentController as DockTabController;
						if (dockTabController != null)
						{
							dockTabController.SelectedController = dhc;
						}
					}

					// After depersistence, set floatbounds to be empty so that the
					// first floating frame is always created with the actual control bounds.
					if (dhc.Floating == false)
					{
						dhc.DIPrevious.rcDockArea = Rectangle.Empty;
						(dhc.HostControl as DockHost).DragRectangle = Rectangle.Empty;
					}
					if (dhc.DockVisibility == true && !alautohide.Contains(dhc))
					{
						if ((dhc.ParentController is DockTabController) && (dhc.DockTab == null))
							continue;

						// If the During application startup floatingForms are displayed from the HostControl's Paint event handler as displaying toplevel forms
						// before the application's main form is displayed causes problems such as failure to apply the FormWindowState.Maximized state,
						// the Form.CreateDialog() method failing due to circular ownership etc.,
						if (dhc.Floating == true)
						{
							if (this.DesignProcess == false)
							{
								if ((this.HostControl.IsHandleCreated == true) && (Syncfusion.Runtime.InteropServices.NativeMethods.IsWindowVisible(this.HostControl.Handle) == true))
									dhc.HostControl.Parent.Visible = true;
							}
							else
							{
								// For design time contained UserControls floating windows are shown only during the mousedown event
								if ((this.DesignMode == true) || (this.HostControl is Form))
									dhc.HostControl.Parent.Visible = true;
							}
						}
						dhc.HostControl.Visible = true;
						if (dhc.ParentController is FloatingFormController && this.lockUpdates > 0)
						{
							dhc.ParentController.HostControl.Visible = true;
						}
					}
				}
			}

			// Apply the MDIChild mode settings
			foreach (DockControllerBase dcbase in dcarray)
			{
				if (dcbase is DockHostController)
				{
					DockHostController dhc = dcbase as DockHostController;
					if ((dhc.MdiChildState == true) && (dhc.DockVisibility == true) && (dhc.FloatOnly == false))
					{
						DHCSerializationWrapper dhcwrapper = dmgrserializer.GetDHCSerializationWrapper(dhc);
						Rectangle layout = new Rectangle(dhcwrapper.ctrlLocation, dhcwrapper.ctrlSize);
						if (dhc.HostControl.Controls.Count > 0)
							this.SetAsMDIChild(dhc.HostControl.Controls[0], true, layout);
					}
				}
			}

			// Set the control bounds
			if (cw != null)
			{
				MainFormController mfc = (MainFormController)(alDockAreaControllers[0]);
				mfc.StoreControllers(controllers);
				mfc.ApplyWrapper(cw);
				mfc.ResizeControllers(cw);
				mfc.AdjustLayout();
			}
			else
			{
				foreach (DockControllerBase dcbase in dcarray)
				{
					if (dcbase is DockHostController)
					{
						DockHostController dhc = dcbase as DockHostController;
						if (dhc.bInMDIMode == true)
							continue;
						DHCSerializationWrapper dhcwrapper = dmgrserializer.GetDHCSerializationWrapper(dhc);
						if (dhcwrapper == null)
							continue;
						if (dhcwrapper.ctrlSize != Size.Empty)
						{
							DockControllerBase dcblayout;
							if (dhc.ParentController is DockTabController)
								dcblayout = dhc.ParentController.ParentController;
							else
								dcblayout = dhc.ParentController;
							Size sznewsize = Size.Empty;
							if (dhcwrapper.layoutSize == Size.Empty)
								sznewsize = dhcwrapper.ctrlSize;
							else
								sznewsize = new Size((int)(((float)dhcwrapper.ctrlSize.Width / (float)dhcwrapper.layoutSize.Width) * (float)dcblayout.LayoutRect.Width), (int)(((float)dhcwrapper.ctrlSize.Height / (float)dhcwrapper.layoutSize.Height) * (float)dcblayout.LayoutRect.Height));
							this.SetControlSize(dhc.HostControl.Controls[0], sznewsize);
						}
					}
				}
			}
			foreach (DockHostController dhc in alautohide)
			{
				DHCSerializationWrapper dhcwrapper = dmgrserializer.GetDHCSerializationWrapper(dhc);
				dhc.InvokePrevDockFloatTransition(true);
				dhc.DIPrevious = new DockInfo(dhcwrapper.dockInfoPrevious);
				dhc.DockVisibility = dhcwrapper.bDockVisibility;
				dhc.DIPrevious.rcDockArea = Rectangle.Empty;
				(dhc.HostControl as DockHost).DragRectangle = Rectangle.Empty;
				dhc.DIPrevious = dhcwrapper.dockInfoCurrent;
			}
			foreach (DockHostController dhc in alautohide)
			{
				if (dhc.AutoHideMode == false)
				{
					dhc.ToggleAutoHideMode();
					if (dhc.ParentController is DockTabController)
					{
						DockTabController dockTab = dhc.ParentController as DockTabController;
						this.dcHostForm.GetAHTabControl(dockTab.DINew.dStyle).HideController(dhc, false);
					}
					else
						this.dcHostForm.GetAHTabControl(dhc.DINew.dStyle).HideController(dhc, false);
				}
			}
			alautohide.Clear();

			this.dcHostForm.bOverlapSizing = true;
			ResizeSplitters(splitterWidth);
			m_bForbidWrapperLogic = false;
		}

		protected internal TabbedMDIManager HostFormMdiManager
		{
			get
			{
				TabbedMDIManager mdiManager = null;
				if( this.HostForm != null )
					mdiManager = TabbedMDIManager.GetManagerForForm(this.HostForm);

				return mdiManager;
			}
		}

		internal void SaveMdiZOrder()
		{
			if( this.HostFormMdiManager != null )
			{

				TabbedMDIManager mdiManager = this.HostFormMdiManager;
				if( mdiManager.TabGroupHosts.Length > 0 )
				{
					MDITabPanel panel = mdiManager.TabGroupHosts[0].MDITabPanel;
					if( panel != null && panel.TabPages.Count != 0 )
					{
						m_mdiZOrder.Clear();

						foreach( TabPageAdv tabpage in panel.TabPages )
						{
							MDIChildTabData mdiData = tabpage.TabData as MDIChildTabData;

							if( mdiData != null && mdiData.MdiChild != null 
								&& mdiData.MdiChild is DockingWrapperForm && mdiData.MdiChild.Controls.Count != 0 )
							{
								Control dmControl = mdiData.MdiChild.Controls[0];

								DockHostController dhc = this.GetDockHostController(dmControl);

								if( dhc != null )
									m_mdiZOrder.Add(dhc.UniqueName);
							}
						}
					}
				}
			}
			else
			{
				Control ctrl = null;
				foreach( DockControllerBase ctrlBase in this.alDockAreaControllers )
				{
					DockHostController hostCtrl = ctrlBase as DockHostController;
					if( hostCtrl != null && hostCtrl.bInMDIMode )
					{
						ctrl = hostCtrl.ctrlReference;
						break;
					}
				}

				if( ctrl != null )
				{
					MdiClient client = ctrl.Parent.Parent as MdiClient;

					if( client != null )
					{
						if( m_mdiZOrder.Count == 0 )
						{
							foreach( Control child in client.Controls )
							{
                                DockHostController dhc = null;
                                if (child.Controls.Count > 0)
                                    dhc = this.GetDockHostController(child.Controls[0]);
                                if (dhc != null)
                                    m_mdiZOrder.Add(dhc.UniqueName);
							}
							m_mdiZOrder.Reverse();
						}
					}
				}
			}
		}

        /// <summary>
        /// Gets or sets a value indicating whether docking control should be imported.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if docking control should be imported; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DefaultValue(true)]        
        public bool ImportDockingControl
        {
            get { return importDockingControl; }
            set
            {
                if (importDockingControl != value)
                    importDockingControl = value;
            }
        }

        /// <summary>
        /// Applies deserialized state.
        /// </summary>
        /// <param name="dmgrserializer"></param>
        [Syncfusion.Documentation.DocumentationExclude(),
        EditorBrowsable(EditorBrowsableState.Advanced)
        ]
		public virtual void ApplyDeserializedState(DockingMgrSerializationWrapperAdv dmgrserializer)
		{
            this.SuspendLayout();
            this.LockHostFormUpdate();

            this.bLoadVisibility = false;	// Keeps dockhosts and floatingforms hidden till deserialization is complete
            bool isControlsInitializedOnLoad = false;
			StopAutoHideAnimation();
            
            ControllerWrapper cw = dmgrserializer.controllerWrapper;
			ArrayList ffw = dmgrserializer.FloatingControllerWrappers;
			ArrayList ahonload = dmgrserializer.AutohideOnLoad;

			foreach (String ctrlname in dmgrserializer.htDHCWrapper.Keys)
			{
				bool bcontrolinitialized = false;
				foreach (Control enabledcontrol in this.alEnableDocking)
				{
					if (enabledcontrol.Name == ctrlname)
					{
						bcontrolinitialized = true;
						break;
					}
				}
				if (bcontrolinitialized == false)
				{
                    if (importDockingControl)
                    {
                        try
                        {
                            if (DockingManagersList.Count > 1)
                            {
                                foreach (DockingManager dmgr in DockingManagersList)
                                {
                                    bool found = false;
                                    foreach (Control enabledcontrol in dmgr.alEnableDocking)
                                    {
                                        if (enabledcontrol.Name == ctrlname)
                                        {
                                            DockHostController dhc = dmgr.GetDockHostController(enabledcontrol);
                                            this.ImportControl(dhc);
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (found)
                                        break;
                                }
                            }
                        }
                        catch { }
                    }
                  
                    this.OnInitializeControlOnLoad(new InitializeControlOnLoadEventArgs(ctrlname));
                    isControlsInitializedOnLoad = true;
				}
			}

			this.dcHostForm.bOverlapSizing = false;	// Do not allow the MainFormController to do the overlap avoidance size reduction.

			Array dcarray = this.alDockAreaControllers.ToArray(typeof(DockControllerBase));

			// Run through the controller list and un-autohide and un-hideItem all dockhostcontrollers. Follow this up
			// with a redock/refloat of all controllers housed within docktabs. DockHostControllers need to be visible
			// and have unique dock/float states when applying the persisted layout state.
			foreach (DockControllerBase dcbase in dcarray)
			{
				if (dcbase is DockHostController)
				{
					DockHostController dhc = dcbase as DockHostController;

					DHCSerializationWrapper dhcwrapper = dmgrserializer.GetDHCSerializationWrapper(dhc);
					if (dhcwrapper == null)
					{
                        if (dhc.PreviousDockingMgr != null && dhc.PreviousDockingMgr != this)
                        {
                            try
                            {
                                dhc.PreviousDockingMgr.ImportControl(dhc);
                            }
                            catch { }
                        }
						// Serialized information is not available. Fire the DockStateUnavailable event and hide the control.
						if (dhc.HostControl.Controls.Count > 0)
							this.OnDockStateUnavailable(new DockStateUnavailableEventArgs(dhc.HostControl.Controls[0]));
						if( dhc.bInMDIMode )
							SetAsMdiChild( dhc, false, Rectangle.Empty );
						dhc.DockVisibility = false;
						continue;
					}

					dhc.DockLabel = dhcwrapper.strLabel;

					if( dhc.DockVisibility == false )
						dhc.DockVisibility = true;
					if (dhc.bInMDIMode)
					{
						if (dhc.ctrlReference != null)
							this.SetAsMDIChild(dhc.ctrlReference, false);
						else
							dhc.bInMDIMode = false;
					}
					if( dhc.FloatOnly == true )
						dhc.FloatOnly = false;
					if( dhc.FreezeResize )
						dhc.FreezeResize = false;
					if( !dhc.AllowFloating )
						dhc.AllowFloating = true;
					if( dhc.Floating )
					{
						if( dhc.ParentController is DockTabController )
							(dhc.ParentController as DockTabController).RemoveDockHostFromTab(dhc, true);
						else
							dhc.InvokePrevDockFloatTransition(false);

						dhc.SharedForm = null;
					}
					if (dhc.AutoHideMode == true)
					{
						dhc.ToggleAutoHideMode();
						dhc.LayoutRect = new Rectangle(dhc.LayoutRect.Location
							, dhcwrapper.layoutSize);
					}
					dhc.DockEdge = dhcwrapper.dockEdge;
					if( dhc.Maximized )
						dhc.RestoreController();
					if (dhc.ParentController is DockTabController)
					{
						DockTabController dtc = dhc.ParentController as DockTabController;
						dtc.RemoveDockHostFromTab(dhc, false);
					}

					dhc.MdiChildState = false;
					dhc.FloatDCRList.Clear();
					dhc.DockDCRList.Clear();

					if (this.DesignMode == false)
					{
                        if (dhc.Floating == true)
                            dhc.HostControl.Parent.Visible = false;
                        else
                        {
                            dhc.HostControl.Visible = false;
                        }
					}
					if( dhcwrapper.uniqueName != null )
						dhc.UniqueName = dhcwrapper.uniqueName;
				}
			}

			// restore autohide index
			foreach (DockControllerBase dcbase in alDockAreaControllers)
			{
				DockHostController dhctrl = dcbase as DockHostController;
				if (dhctrl != null)
				{
					DHCSerializationWrapper dhcwrapper = dmgrserializer.GetDHCSerializationWrapper(dhctrl);
					if (dhcwrapper != null)
						dhctrl.AutoHideIndex = dhcwrapper.autoHideIndex;
				}
			}
			ArrayList alautohideBuffer = new ArrayList();
			alautohide = new ArrayList();

			foreach (DockControllerBase dcbase in dcarray)
			{
				if (dcbase is DockHostController)
				{
					DockHostController dhc = dcbase as DockHostController;
					DHCSerializationWrapper dhcwrapper = dmgrserializer.GetDHCSerializationWrapper(dhc);

					if (dhcwrapper != null)
					{
						if (dhcwrapper.bAutoHideMode == true)
						{
							// locate controller index
							int index = alautohideBuffer.Count;
							if (dhc.AutoHideIndex != -1)
							{
								foreach( DockHostController dockHostController in alautohideBuffer )
								{
									if ((dockHostController.AutoHideIndex >= dhc.AutoHideIndex)
										|| (dockHostController.AutoHideIndex == -1))
									{
										index = alautohideBuffer.IndexOf( dockHostController );
										break;
									}
								}
							}
							alautohideBuffer.Insert( index, dhc );
						}

						if( dhcwrapper.dockAbility >= 0 )
							SetDockAbility( dhc.HostControl.Controls[0], dhcwrapper.dockAbility );
						if( dhcwrapper.outerDockAbility >= 0 )
							SetOuterDockAbility( dhc.HostControl.Controls[0], dhcwrapper.outerDockAbility );
                        if (!isControlsInitializedOnLoad)
                        {
                            if (dhcwrapper.imageIndex != -2)
                                dhc.ImageIndex = dhcwrapper.imageIndex;
                        }
						dhc.SharedForm = null;
					}
				}
			}

            // Apply the MDIChild mode settings
			ArrayList mdiSortedList = new ArrayList();
			DockHostController selectedMdiChild = null;

			foreach( DockControllerBase dcbase in dcarray )
			{
				if( dcbase is DockHostController )
				{
					DockHostController dhc = dcbase as DockHostController;
					DHCSerializationWrapper dhcwrapper = dmgrserializer.GetDHCSerializationWrapper(dhc);

					if( ( dhcwrapper != null ) && ( dhcwrapper.bInMDIMode ) && ( dhc.FloatOnly == false ) )
					{	
						if( dhcwrapper.bDockVisibility )
						{
							if( this.HostFormMdiManager != null && dmgrserializer.MdiZOrder != null
								&& dmgrserializer.MdiZOrder.Contains( dhc.UniqueName ) )
							{
								mdiSortedList.Add( dhc );

								if( dhcwrapper.bSelectedPage )
									selectedMdiChild = dhc;
							}
							else
							{
								Rectangle layout = new Rectangle( dhcwrapper.ctrlLocation, dhcwrapper.ctrlSize );
                                if (dhc.HostControl.Controls.Count > 0)
                                {
                                    dhc.MdiWindowState = dhcwrapper.windowState;
                                    this.SetAsMDIChild(dhc.HostControl.Controls[0], true, layout);
                                }
							}
						}
                        else
                        {
                            if (dhc.HostControl.Controls.Count > 0)
                            {
                                dhc.MdiWindowState = dhcwrapper.windowState;
                                dhc.MdiChildBounds = new Rectangle(dhcwrapper.ctrlLocation, dhcwrapper.ctrlSize);
                            }
                            dhc.MdiChildState = dhcwrapper.bInMDIMode;
                        }
					}
				}
			}

			RestoreMdiZOrder(dmgrserializer.MdiZOrder);

			if( mdiSortedList.Count > 0 )
			{
				foreach( string name in dmgrserializer.MdiZOrder )
				{
					foreach( DockHostController dhc in mdiSortedList )
						if( dhc != null && dhc.UniqueName == name )
						{
							this.SetAsMDIChild(dhc.HostControl.Controls[0], true);
							break;
						}
				}

				if( selectedMdiChild != null )
				{
					TabbedMDIManager mdiManager = this.HostFormMdiManager;
					if( mdiManager.TabGroupHosts.Length > 0 )
					{
						MDITabPanel panel = mdiManager.TabGroupHosts[0].MDITabPanel;
						if( panel != null && panel.TabPages.Count != 0 )
						{
							m_mdiZOrder.Clear();

							foreach( TabPageAdv tabpage in panel.TabPages )
							{
								MDIChildTabData mdiData = tabpage.TabData as MDIChildTabData;

								if( mdiData != null && mdiData.MdiChild != null
									&& mdiData.MdiChild is DockingWrapperForm && mdiData.MdiChild.Controls.Count != 0 )
								{
									Control dmControl = mdiData.MdiChild.Controls[0];

									DockHostController dhc = this.GetDockHostController(dmControl);

									if( dhc == selectedMdiChild )
										panel.SelectedTab = tabpage;
								}
							}
						}
					}
				}
			}

			foreach( FloatingFormController ffc in this.HiddenFFControllers )
			{
				FloatingForm ff = ffc.HostControl as FloatingForm;

				if( ff != null && ff.FormOwner != null )
					ff.FormOwner.InternalForm = null;

				ffc.Dispose();
			}

			this.HiddenFFControllers.Clear();
			ArrayList controllers = new ArrayList();

			foreach( FloatingFormControllerWrapper floatWrapper in ffw )
			{
				if( floatWrapper != null )
				{
					DockHostController mainHost = LocateDockController( floatWrapper );

					if( mainHost != null )
					{
						this.bLoadVisibility = true;
						mainHost.InternalForm = null;
						mainHost.SharedForm = null;
						DockControllerBase parent = mainHost.ParentController;
						parent.RemoveChild( mainHost );
						mainHost.DINew.rcDockArea.Height = 0;
						mainHost.InternalFloatWrapper = null;

						FloatingForm ff = mainHost.CreateFloatingFrame( floatWrapper.LayoutRect.Location, false );
						ff.Size = floatWrapper.LayoutRect.Size;
						parent.Refresh();

						ReArrangeControls( ff.InternalController, floatWrapper.Children );
						ff.Enable(false);
						ff.Used = true;
						( ff.InternalController as FloatingFormController ).RefreshFormCaption();
						ff.InternalController.AdjustLayout();
					}
					else
					{
						this.bLoadVisibility = false;
						FloatingForm ff = new FloatingForm( this );
						ff.Bounds = floatWrapper.LayoutRect;
						ff.Disable();
						ReArrangeControls( ff.InternalController, floatWrapper.Children );
						ff.Used = false;
					}
				}
			}
			alautohide.AddRange( alautohideBuffer );
			foreach (DockControllerBase dcb in this.dcHostForm.ChildControllers)
			{
				controllers.Add(dcb);
			}
			this.bLoadVisibility = true;
			ReArrangeControls(dcHostForm, cw.Children);

			foreach( DockControllerBase dcb in controllers )
				dcb.RemoveDockController();

			foreach (DockHostController dhc in alautohide)
			{
				DHCSerializationWrapper dhcwrapper = dmgrserializer.GetDHCSerializationWrapper(dhc);

				if (dhc.AutoHideMode == false)
				{
					if (dhc.ParentController is DockTabController)
					{
						DockTabController dockTab = dhc.ParentController as DockTabController;
						dockTab.LayoutRect = new Rectangle(dockTab.LayoutRect.Location,
							dhcwrapper.layoutSize);
						bool hideAll = true;

						foreach( DockTabPage tabPage in dockTab.TabControl.TabPages )
						{
							DHCSerializationWrapper tabWrapper 
								= dmgrserializer.GetDHCSerializationWrapper(tabPage.dhcClient);
							if( tabWrapper != null && tabWrapper.bSelectedPage )
								dockTab.SelectedController = tabPage.dhcClient;
							if( tabWrapper != null && !tabWrapper.bAutoHideMode )
								hideAll = false;
						}
						if( hideAll )
						{
							dockTab.HostController.ToggleAutoHideMode();
							controllers.Remove( dockTab.ParentController );
							this.dcHostForm.GetAHTabControl( dockTab.DINew.dStyle ).HideController( dhc, false );

							dockTab.DITransient.rcDockArea.Size = dhcwrapper.layoutSize;
							dockTab.DINew.rcDockArea.Size = dhcwrapper.layoutSize;

							foreach( DockTabPage page in dockTab.TabControl.TabPages )
							{
								page.dhcClient.DITransient.rcDockArea.Size = dhcwrapper.layoutSize;
								page.dhcClient.DINew.rcDockArea.Size = dhcwrapper.layoutSize;
							}
						}
						else
							this.ToggleAHState( dhc , true, true, false );
					}
					else
					{
						if( dhc.ParentController != null )
						{
							controllers.Remove( dhc.ParentController );
							dhc.LayoutRect = new Rectangle( dhc.LayoutRect.Location, dhcwrapper.layoutSize );
							dhc.ToggleAutoHideMode();
							this.dcHostForm.GetAHTabControl( dhc.DINew.dStyle ).HideController( dhc, false );
						}
					}

					dhc.DITransient.rcDockArea.Size = dhcwrapper.layoutSize;
					dhc.DINew.rcDockArea.Size = dhcwrapper.layoutSize;
				}
			}

			foreach( DockControllerBase baseCtrl in dcarray )
			{
				DockHostController dhc = baseCtrl as DockHostController;

				if( dhc != null )
				{
					DHCSerializationWrapper dhcwrapper = dmgrserializer.GetDHCSerializationWrapper(dhc);

					if( !alautohide.Contains(dhc) )
					{
						if( dhcwrapper != null )
						{
							dhc.LayoutRect = new Rectangle( dhc.LayoutRect.Location, dhcwrapper.layoutSize );
							if( dhcwrapper.transientRect.Width > 0 && dhcwrapper.transientRect.Height > 0 )
								dhc.DITransient.rcDockArea = dhcwrapper.transientRect;
							else
								dhc.DITransient.rcDockArea = dhc.LayoutRect;
						}
					}

					if( dhcwrapper != null )
					{
						bool inAutohide = dhc.bInAutoHide;
						if( !dhc.bInMDIMode )
						{
							dhc.DockVisibility = dhcwrapper.bDockVisibility;
                            // if( !dhc.DockVisibility )
                            //    ( dhc.HostControl as DockHost ).Bounds = dhc.DINew.rcDockArea;
						}

						if( dhcwrapper.previousFloat )
						{
							dhc.PrevWrapper = dhc.InternalFloatWrapper;
							if( dhc.PrevWrapper != null
								&& dhc.PrevWrapper.ToplevelController.HostControl != null )
							{
								dhc.SharedForm = dhc.PrevWrapper.ToplevelController.HostControl as FloatingForm;
								dhc.SharedForm.FormOwner = dhc;
							}
						}

						dhc.Maximized = dhcwrapper.maximized;
						dhc.PrevAutohideStyle = dhcwrapper.prevAHStyle;
						dhc.AllowFloating = dhcwrapper.bAllowFloating;
                        dhc.StoredDockSizes = dhcwrapper.m_storedDockSizes;
                        dhc.StoredFloatSizes = dhcwrapper.m_storedFloatSizes;

						if( dhcwrapper.tabSiblings.Count > 0 && !inAutohide )
						{
							if( !dhc.DockVisibility || dhc.bInMDIMode )
							{
								DockStateControllerWrapper tabWrap = new DockStateControllerWrapper( this, dhc );

								foreach( string uniqueName in dhcwrapper.tabSiblings )
								{
									DockHostController sibling = this.dcHostForm.QueryController( uniqueName ) as DockHostController;
									if( sibling != null )
									{
										tabWrap.DockRelationControllers.Add( sibling );

										if( sibling.ParentController != null )
											tabWrap.ParentController = sibling.ParentController;
									}
								}

								if( tabWrap.ParentController != null )
								{
									DockRelation relation = new DockRelation();
									relation.Relation = dhcwrapper.dockInfoPrevious;
									tabWrap.Relations[dhc.UniqueName] = relation;

									if( dhcwrapper.previousFloat )
										dhc.InternalFloatWrapper = tabWrap;
									else
										dhc.InternalDockWrapper = tabWrap;
								}
							}
							else if( dhcwrapper.bAutoHideMode && dhc.ParentController == null )
							{
								foreach( string uniqueName in dhcwrapper.tabSiblings )
								{
									DockHostController sibling = this.dcHostForm.QueryController( uniqueName ) as DockHostController;

									if( sibling != null && sibling.DockVisibility && !sibling.Floating )
									{
										DCRelationship dcr = new DCRelationship( 0, false, DockPreference.Tabbed
											, dhcwrapper.tabSiblings.IndexOf( dhc.UniqueName ) );
										dhc.DINew.nDockIndex = dcr.nIndex;
										sibling.InvokeTabbedDocking( dhc, dcr );
										this.ToggleAHState( dhc, true, true, false );
										break;
									}
								}
							}
						}

						if( dhcwrapper.bSelectedPage )
						{
							DockTabController dtc = dhc.ParentController as DockTabController;
							if( dtc != null )
								dtc.SelectedController = dhc;
						}
						if( dhcwrapper.alDockDCR.Count > 0)
							dhc.DCRCurrent = dhcwrapper.alDockDCR[0] as DCRelationship;

						dhc.AutoHiddedBeforeHide = dhcwrapper.bAutoHideMode;
						dhc.FreezeResize = dhcwrapper.bFreezeResize;
						dhc.FloatOnly = dhcwrapper.bFloatOnly;
					}
				}
			}

			alautohide.Clear();

			this.dcHostForm.bOverlapSizing = true;
			this.dcHostForm.AdjustLayout();

			foreach (FloatingFormController ffc in this.alFFControllers)
			{
                FloatingForm fForm = null;
                bool isHiddenOnLoad = false;                
                if (ffc.HostControl.Controls.Count > 0)
                {
                    if (ffc.HostControl.Controls[0].Controls.Count > 0)
                    {
                        isHiddenOnLoad = GetHiddenOnLoad(ffc.HostControl.Controls[0].Controls[0]);                        
                    }
                }

                if (m_isInitializing && isHiddenOnLoad)
                {
                    if (ffc.HostControl is FloatingForm)
                    {
                        fForm = ffc.HostControl as FloatingForm;
                        fForm.UpdateFormBorderStyle();
                    }
                }
                else
                    ffc.HostControl.Visible = true;

				ffc.AdjustLayout();
			}

			foreach( Control ctrl in m_freezeResizeCtrls )
			{
				DockHostController dhc = GetDockHostController(ctrl);
				if( dhc != null )
					dhc.FreezeResize = true;
			}

			if( ahonload != null && ahonload.Count > 0 )
			{
				listAHOnLoad.Clear();
				foreach( string name in ahonload )
				{
					DockHostController dhc = this.dcHostForm.QueryController( name ) as DockHostController;
					if( dhc != null )
						listAHOnLoad.Add( dhc.HostControl.Controls[0] );
				}
			}

            this.ResumeLayout();
            this.UnlockHostFormUpdate();
		}
		

		/// <summary>
		/// Reads the persisted dockstate from the Isolated Storage.
		/// </summary>
		/// <returns>TRUE if the read is successful.</returns>
		public bool LoadDockState()
		{
			return this.LoadDockState(AppStateSerializer.GetSingleton());
		}

		/// <summary>
		/// Reads a previously serialized dockstate.
		/// </summary>
		/// <param name="mode"> A <see cref="SerializeMode"/> value.</param>
		/// <param name="persistpath">The name of the IsolatedStorage/INI/XML file or the
		/// registry key containing the persisted dockstate information.</param>
		/// <returns>TRUE if the load is successful.</returns>
		/// <remarks>
		/// Reads the dockstate information from the specified persistent store and applies the new state.
		/// This method has been provided only to allow a higher degree of control over the
		/// serialization process. For normal state storage and retrieval it is advisable to
		/// use the <see cref="DockingManager.SaveDockState()"/> and <see cref="DockingManager.LoadDockState()"/>
		/// methods.
		/// <para>
		/// This method will be removed in a future version. Please use the more flexible LoadCommandBarState(AppStateSerializer) variant, instead.
		/// </para>
		/// </remarks>
		[Obsolete("This method will be removed in a future version. Please use the more flexible LoadDockState(AppStateSerializer) variant, instead.", false)]
		public virtual bool LoadDockState(SerializeMode mode, Object persistpath)
		{
			bool brestoreminmax = this.bApplyMinMaxExtents;
			this.bApplyMinMaxExtents = false;

			bool bretval = false;
            this.m_bLoadingDockState = true;
			try
			{
				this.OnNewDockStateBeginLoad(EventArgs.Empty);
				String strpersist = String.Empty;
				if (this.DesignProcess == false)
					strpersist = String.Concat(this.PersistenceID, this.PersistKey);	// Runtime
				else
					strpersist = DockingManager.strPersistKey;
				DockingMgrSerializationWrapper dmgrserializer = null;
				if ((mode == SerializeMode.IsolatedStorage) && (persistpath == null))
				{
					AppStateSerializer serializer = AppStateSerializer.GetSingleton();
					dmgrserializer = serializer.DeserializeObject(strpersist) as DockingMgrSerializationWrapper;
				}
				else	// User-invoked
				{
					dmgrserializer = AppStateSerializer.DeserializeIsolatedObject(mode, persistpath, strpersist) as DockingMgrSerializationWrapper;
				}
				if (dmgrserializer != null)
				{
					DockingMgrSerializationWrapperAdv advSerializer = dmgrserializer as DockingMgrSerializationWrapperAdv;

					if( advSerializer != null )
						this.ApplyDeserializedState(advSerializer);
					else
						this.ApplyDeserializedState(dmgrserializer);

					bretval = true;
				}
			}
			catch (Exception e)
			{
				Debug.Assert(false, "LoadDockState Failed.", e.Message);
			}
			finally
			{
				this.bApplyMinMaxExtents = brestoreminmax;
                this.m_bLoadingDockState = false;
			}

			DockStateLoadEventArgs args = new DockStateLoadEventArgs(bretval);
			this.OnNewDockStateEndLoad(args);
			return bretval;
		}

		/// <summary>
		/// Reads a previously serialized dockstate using the AppStateSerializer object.
		/// </summary>
		/// <param name="serializer">A reference to the <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/> instance.</param>
		/// <returns>TRUE if the load is successful.</returns>
		/// <remarks>
		/// Reads the dockstate information from the specified persistent store and applies the new state.
		/// This method has been provided only to allow a higher degree of control over the
		/// serialization process. For normal state storage and retrieval it is advisable to
		/// use the <see cref="DockingManager.SaveDockState()"/> and <see cref="DockingManager.LoadDockState()"/>
		/// methods.
		/// </remarks>
		/// <example>
		/// <code lang="C#">
		/// //Loading DockState from IsolatedStorage
		/// AppStateSErializer appstser=new AppStateSerializer(SerializeMode.IsolatedStorage, null);
		/// dockingManager1.LoadDockState(appstser);
		/// //Loading DockState from xml file(DockState.xml located in Application folder)
		/// AppStateSerializer appstser =new AppStateSerializer(SerializeMode.XMLFile, "DockState");
		/// dockingManager1.LoadDockState(appstser);
		/// </code>
		/// <code lang="VB">
		/// 'Loading DockState from IsolatedStorage
		/// Dim appstser As New AppStateSerializer(SerializeMode.IsolatedStorage, Nothing)
		/// dockingManager1.LoadDockState(appstser)
		/// 'Loading DockState from xml file(DockState.xml located in Application folder)
		/// Dim appstser As New AppStateSerializer(SerializeMode.XMLFile, "DockState")
		/// dockingManager1.LoadDockState(appstser)
		/// </code>
		/// </example>
		public virtual bool LoadDockState(AppStateSerializer serializer)
		{
			if (serializer == null)
				return false;

			bool brestoreminmax = this.bApplyMinMaxExtents;
			this.bApplyMinMaxExtents = false;

			bool bretval = false;
            this.m_bLoadingDockState = true;
			try
			{
				this.OnNewDockStateBeginLoad(EventArgs.Empty);
				String strpersist = String.Empty;
				if (this.DesignProcess == false)
					strpersist = String.Concat(this.PersistenceID, this.PersistKey);	// Runtime
				else
					strpersist = DockingManager.strPersistKey;
				DockingMgrSerializationWrapper dmgrserializer = serializer.DeserializeObject(strpersist) as DockingMgrSerializationWrapper;
				if (dmgrserializer != null)
				{
					if (dmgrserializer is DockingMgrSerializationWrapperAdv)
					{
						this.ApplyDeserializedState(dmgrserializer as DockingMgrSerializationWrapperAdv);
					}
					else
						this.ApplyDeserializedState(dmgrserializer);
					bretval = true;
				}
			}
			catch (Exception e)
			{
				Debug.Assert(false, "LoadDockState Failed.", e.Message);
			}
			finally
			{
				this.bApplyMinMaxExtents = brestoreminmax;
                this.m_bLoadingDockState = false;
			}            
			DockStateLoadEventArgs args = new DockStateLoadEventArgs(bretval);
			this.OnNewDockStateEndLoad(args);
			return bretval;
		}

		/// <summary>
		/// Reads a previously serialized dockstate for the specified dockable control and applies the new state.
		/// </summary>
		/// <param name="serializer">A reference to the <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/> instance.</param>
		/// <returns>TRUE if the load is successful.</returns>
		/// <remarks>
		/// When attempting to read from the store, the LoadDockState method first attempts to locate persisted data pertaining to this
		/// control from the stored dockstate information for the <see cref="DockingManager"/>'s full control set and failing that looks for
		/// dockstate information that is exclusive to the control.
		/// <seealso cref="DockingMangaer.SaveDockState()"/>
		/// <seealso cref="DockingManager.LoadDockState()"/>
		/// </remarks>
		public bool LoadDockState(AppStateSerializer serializer, Control ctrl)
		{
			DockHostController dhc = this.GetDockHostController(ctrl);
			if (dhc == null)
			{
				Debug.Assert(false, "Error: Control has not been enabled for docking.\n");
				return false;
			}

			bool brestoreminmax = this.bApplyMinMaxExtents;
			this.bApplyMinMaxExtents = false;

			bool bretval = false;
            this.m_bLoadingDockState = true;
			try
			{
				// First attempt to look for persistence information for this control in the DockingManager's serialization data
				// for the entire control set.
				DHCSerializationWrapper dhcwrapper = null;
				String strpersist = String.Concat(this.PersistenceID, this.PersistKey);
				DockingMgrSerializationWrapperAdv dmgrserializer = serializer.DeserializeObject(strpersist) as DockingMgrSerializationWrapperAdv;

				if (dmgrserializer != null)
					dhcwrapper = dmgrserializer.GetDHCSerializationWrapper(dhc);

				if (dhcwrapper == null)
				{
					// See if the dock state information for this control has been stored individually.
					strpersist = String.Concat(this.PersistenceID, this.PersistKey, ctrl.Name);
					dhcwrapper = serializer.DeserializeObject(strpersist) as DHCSerializationWrapper;
				}

				if( dmgrserializer != null )
				{
					this.ApplyDeserializedState(dhc, dmgrserializer);
					bretval = true;
				}
				else
				{
					// Serialized information is not available. Fire the DockStateUnavailable event and hide the control.
					this.OnDockStateUnavailable(new DockStateUnavailableEventArgs(ctrl));
				}

			}
			catch (Exception e)
			{
				Debug.Assert(false, "LoadDockState Failed.", e.Message);
			}
			finally
			{
				this.bApplyMinMaxExtents = brestoreminmax;
                this.m_bLoadingDockState = false;
			}            
			return bretval;
		}

		/// <summary>
		/// Restores the dockstate to that set within the visual designer.
		/// </summary>
		/// <returns>TRUE if the load is successful.</returns>
		public bool LoadDesignerDockState()
		{
			bool bretval = false;
            this.m_bLoadingDockState = true;

			ctrlLastActive = null;
			if ((this.stmLayout == null) || (this.stmLayout.Length <= 0))
			{
				Debug.Assert(false, "Error - Designer Default DockState is not available.\n");
				this.ApplyDHCInitialSettings();
				return false;
			}
			this.stmLayout.Seek(0, SeekOrigin.Begin);
			this.OnNewDockStateBeginLoad(EventArgs.Empty);

			bool brestoreminmax = this.bApplyMinMaxExtents;
			this.bApplyMinMaxExtents = false;

			if (this.LoadFromStream(this.stmLayout) == true)
			{
				this.ApplyDHCAHSettings();
				this.ApplyDHCFloatOnlySettings();
				this.ApplyDHCHiddenOnLoadSettings();
				bretval = true;
			}

			this.bApplyMinMaxExtents = brestoreminmax;
            this.m_bLoadingDockState = false;
			DockStateLoadEventArgs args = new DockStateLoadEventArgs(bretval);
			this.OnNewDockStateEndLoad(args);
			return bretval;
		}

		/// <summary>
		/// Saves the dockstate information for the specified dockable control.
		/// </summary>
		/// <param name="serializer">A reference to the <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/> instance.</param>
		/// <remarks>
		/// Takes a snapshot of the control's current dock state in the <see cref="DockingManager"/>'s layout and serializes this information
		/// to the persistence medium set in the AppStateSerializer.
		/// <seealso cref="DockingManager.SaveDockState()"/>
		/// <seealso cref="DockingManager.LoadDockState()"/>
		/// </remarks>
		public void SaveDockState(AppStateSerializer serializer, Control ctrl)
		{
			DockHostController dhc = this.GetDockHostController(ctrl);
			if (dhc == null)
			{
				Debug.Assert(false, "Error: Control has not been enabled for docking.\n");
				return;
			}

			try
			{
				DockingMgrSerializationWrapperAdv dmgrwppr = new DockingMgrSerializationWrapperAdv(this);
				String strpersist = String.Concat(this.PersistenceID, this.PersistKey);
				serializer.SerializeObject(strpersist, dmgrwppr);
			}
			catch (Exception e)
			{
				Debug.Assert(false, "SaveDockState Failed.", e.Message);
			}
		}

		/// <summary>
		/// Overloaded. Saves the current dockstate to Isolated Storage.
		/// </summary>
		public void SaveDockState()
		{
			this.SaveDockState(AppStateSerializer.GetSingleton());
		}

		/// <summary>
		/// Saves the current dockstate information to the specified persistence medium.
		/// </summary>
		/// <param name="mode"> A <see cref="SerializeMode"/> value.</param>
		/// <param name="persistpath"> Specifies the name of an IsolatedStorage/INI/XML file or a registry key to
		/// which the persistence information will be written.</param>
		/// <remarks>
		/// Writes the docking windows information to the persistence medium specified by the
		/// <paramref name="mode"/> parameter and at the path specified by the <paramref name="persistpath"/> object.
		/// This method has been provided only to allow a higher degree of control over the
		/// serialization process. For normal state storage and retrieval it is advisable to
		/// use the <see cref="DockingManager.SaveDockState()"/> and <see cref="DockingManager.LoadDockState()"/>
		/// methods.
		/// <para>
		/// This method will be removed in a future version. Please use the more flexible SaveDockState(AppStateSerializer) variant, instead.
		/// </para>
		/// </remarks>
		[Obsolete("This method will be removed in a future version. Please use the more flexible SaveDockState(AppStateSerializer) variant, instead.", false)]
		public virtual void SaveDockState(SerializeMode mode, Object persistpath)
		{
			// Save the current dockinfo for all dockhostcontrollers in the alController list
			try
			{
				DockingMgrSerializationWrapperAdv dmgrwppr = new DockingMgrSerializationWrapperAdv(this);
				String strpersist = String.Empty;
				if (this.DesignProcess == false)
					strpersist = String.Concat(this.PersistenceID, this.PersistKey);	// Runtime
				else
					strpersist = DockingManager.strPersistKey;
				if ((mode == SerializeMode.IsolatedStorage) && (persistpath == null)) // Default serialization to Isolated Storage
				{
					AppStateSerializer serializer = AppStateSerializer.GetSingleton();
					serializer.SerializeObject(strpersist, dmgrwppr);
				}
				else
				{
					AppStateSerializer.SerializeIsolatedObject(mode, persistpath, strpersist, dmgrwppr);
				}
			}
			catch (Exception e)
			{
				Debug.Assert(false, "SaveDockState Failed.", e.Message);
			}
		}

		/// <summary>
		/// Saves the current dockstate information to the specified <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/>.
		/// </summary>
		/// <param name="serializer">A reference to the <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/> instance.</param>
		/// <remarks>
		/// Writes the docking windows information to the persistence medium.
		/// This method has been provided only to allow a higher degree of control over the
		/// serialization process. For normal state storage and retrieval it is advisable to
		/// use the <see cref="DockingManager.SaveDockState()"/> and <see cref="DockingManager.LoadDockState()"/>
		/// methods.
		/// </remarks>
		/// <example>
		/// <code lang="C#">
		/// //Saving DockState to IsolatedStorage
		/// AppStateSErializer appstser=new AppStateSerializer(SerializeMode.IsolatedStorage, null);
		/// dockingManager1.SaveDockState(appstser);
		/// appstser.PersistNow();
		/// //Saving DockState to xml file(DockState.xml located in Application folder)
		/// AppStateSerializer appstser =new AppStateSerializer(SerializeMode.XMLFile, "DockState");
		/// dockingManager1.SaveDockState(appstser);
		/// appstser.PersistNow();
		/// </code>
		/// <code lang="VB">
		/// 'Saving DockState to IsolatedStorage
		/// Dim appstser As New AppStateSerializer(SerializeMode.IsolatedStorage, Nothing)
		/// dockingManager1.SaveDockState(appstser)
		/// appstser.PersistNow()
		/// 'Saving DockState to xml file(DockState.xml located in Application folder)
		/// Dim appstser As New AppStateSerializer(SerializeMode.XMLFile, "DockState")
		/// dockingManager1.SaveDockState(appstser)
		/// appstser.PersistNow()
		/// </code>
		/// </example>
		public virtual void SaveDockState(AppStateSerializer serializer)
		{
			if (serializer == null)
				return;

			// Save the current dockinfo for all dockhostcontrollers in the alController list
			try
			{
				DockingMgrSerializationWrapperAdv dmgrwppr = new DockingMgrSerializationWrapperAdv(this);
				String strpersist = String.Empty;
				if (this.DesignProcess == false)
					strpersist = String.Concat(this.PersistenceID, this.PersistKey);	// Runtime
				else
					strpersist = DockingManager.strPersistKey;

				serializer.SerializeObject(strpersist, dmgrwppr);
			}
			catch (Exception e)
			{
				Debug.Assert(false, "SaveDockState Failed.", e.Message);
			}
		}

		/// <summary>
		/// Initializes the control as a docking window and sets it to be in the autohide mode.
		/// </summary>
		/// <param name="ctrl">The control instance.</param>
		/// <param name="edge">The host container edge along which the control will be autohidden.</param>
		/// <param name="size">The autohide window size.</param>
		public void DockControlInAutoHideMode(Control ctrl, DockingStyle edge, int size)
		{
			if (!alEnableDocking.Contains(ctrl))
				SetEnableDocking(ctrl, true);
			DockHostController dhc = GetDockHostController(ctrl);
			if (dhc != null)
			{
				if ((dhc.Floating == false) && (dhc.bInMDIMode == false) && (dhc.DockVisibility == true))
				{
					if (dhc.DICurrent.dStyle != edge)
						DockControl(ctrl, HostControl, edge, size);
					this.SetAutoHideMode(ctrl, true);
					dhc.bAutoHideSizing = false;
				}
			}
		}
        /// <summary>
        /// Sets the control as a separate floating window.
        /// </summary>
        /// <remarks>
        /// Floats the control as a resizable frame using the coordinates and bounds specified by the rcscreen parameter.
        /// </remarks>
        /// <param name="ctrl"> The control to be floated.</param>
        /// <param name="rcscreen"> The bounds for the floating parent frame.</param>
        /// <example>
        /// <code lang="C#">
        /// //Float control panel1 in specified manner.
        /// dockingManager1.FloatControl(panel1,new Rectangle(1,1,200,200));
        /// </code>
        /// <code lang="VB">
        /// 'Float control panel1 in specified manner.
        /// DockingManager1.FloatControl(Panel1,new Rectangle(1,1,200,200))
        /// </code>
        /// </example>
        public virtual void FloatControl(Control ctrl, Rectangle rcscreen)
        {
            FloatControl(ctrl, rcscreen, false);
        }

		/// <summary>
		/// Sets the control as a separate floating window.
		/// </summary>
		/// <remarks>
		/// Floats the control as a resizable frame using the coordinates and bounds specified by the rcscreen parameter.
		/// </remarks>
		/// <param name="ctrl"> The control to be floated.</param>
		/// <param name="rcscreen"> The bounds for the floating parent frame.</param>
        /// <param name="bTabFloating"> When control is on DockTabPage, make entire DockTabControl floating if true.</param>
		/// <example>
		/// <code lang="C#">
		/// //Float control panel1 in specified manner.
		/// dockingManager1.FloatControl(panel1,new Rectangle(1,1,200,200),true);
		/// </code>
		/// <code lang="VB">
		/// 'Float control panel1 in specified manner.
		/// DockingManager1.FloatControl(Panel1,new Rectangle(1,1,200,200),true)
		/// </code>
		/// </example>
		public virtual void FloatControl(Control ctrl, Rectangle rcscreen, bool bTabFloating)
		{
            Control tempCtrl = ctrl;
            while (tempCtrl.Parent != null)
            {
                if (tempCtrl.Parent is DockHost)
                {
                    tempCtrl = tempCtrl.Parent;
                }
                else if (tempCtrl.Parent is Form)
                {
                    this.PreviousParentForm = tempCtrl.Parent as Form;
                    this.PreviousParentVisibility = tempCtrl.Parent.Visible;
                    break;
                }
                else
                    tempCtrl = tempCtrl.Parent;
            }
			// Pad the rcscreen value for the floating form's caption height and border widths
			Size bordersize = SystemInformation.FrameBorderSize;
			rcscreen.Height += SystemInformation.ToolWindowCaptionHeight + (bordersize.Height * 2) + 2;
			rcscreen.Width += (bordersize.Width * 2) + 2; // The DockHost borders add on 2 pixels

            Control[] ctrls = { ctrl };
            DockStateChangeEventArgs args = new DockStateChangeEventArgs(ctrls);

			DockHostController dhc = this.GetDockHostController(ctrl);
			if (dhc == null)
			{
				if (this.alEnableDocking.Contains(ctrl) == false)
					this.alEnableDocking.Add(ctrl);
				// Create a new dockhost for this control and set the initial pos to be floating
				DockHost dhost = this.CreateDockHost(ctrl);
				Debug.Assert(dhost != null);

				if( !htCustomCaptionButtons.ContainsKey( ctrl ) )
					htCustomCaptionButtons.Add( ctrl, new CaptionButtonsCollection() );
				SynchronizeCaptionButtons();

				dhc = dhost.InternalController as DockHostController;
				dhc.DINew = new DockInfo(null, Syncfusion.Windows.Forms.Tools.DockingStyle.Fill, -1, 0, DockPreference.None, rcscreen);
				dhost.DragRectangle = rcscreen;
                this.FireDockStateChangeEvent( "DockStateChanging", args );
				// Fix: FloatControl does not show the control when called between BeginInit and EndInit
				dhc.CreateFloatingFrame(rcscreen.Location, true);
				dhc.Floating = true;
				//Workaround for context menu not showing when the floated by FloatControl method until move or dock the window for first time. 
				dhc.HostControl.Focus();

				if (this.alEnableDocking.Count > 0)
				{
					if (DesignerHooks.GetMsgProcListContains(this) == false)
						m_curentThreadId = DesignerHooks.AddGetMsgProcListener(this);
				}
                this.FireDockStateChangeEvent("DockStateChanged", args);
			}
			else
			{
				bool hidden = false;
				DockHostController sibling = null;
				if (dhc.DockVisibility == false)
					return;
                this.bFreezeDockStateChangeEvents = true;
				if( dhc.bInMDIMode )
					this.SetAsMdiChild(dhc, false, Rectangle.Empty);
				if( dhc.bInAutoHide )
				{
					dhc.ToggleAutoHideMode();
					hidden = true;
				}
				DockTabController dtc = dhc.ParentController as DockTabController;
				if( dhc.dockInfoCurrent.DP == DockPreference.Tabbed && !bTabFloating)
				{
					dtc = dhc.ParentController as DockTabController;
					Debug.Assert(( dtc != null ), "Error: Invalid Parent Controller.\n");

					if( hidden )
						foreach( DockTabPage dtp in dtc.TabControl.TabPages )
						{
							if( dtp.dhcClient != dhc )
								sibling = dtp.dhcClient;
						}

					dtc.RemoveDockHostFromTab(dhc, !dhc.Floating);

                    //For floating each panels individualy.
                    //1)Removing it from the tab
                    //2)Ensuring the focus remains in the order

                    DockTabController dtcparent = dhc.ParentController as DockTabController;
                    if (dtcparent != null)
                    {
                        
                        dtcparent.RemoveDockHostFromTab(dhc, false);
                        
                        if (dhc.HostControl.Controls.Count > 0)
                        {
                            NeedFocusControls.Add(dhc.HostControl.Controls[0]);

                            if (this.NeedFocusControls != null)
                            {
                                foreach (Control fCtrl in this.NeedFocusControls)
                                {
                                    fCtrl.Focus();
                                }
                            }
                            this.ActivateControl(dhc.HostControl.Controls[0]);
                        }
                            
                    }

				}

                if (dtc != null && bTabFloating)
                {
                    if (dtc.Floating)
                    {
						FloatingFormController ffc = dtc.ParentController as FloatingFormController;
						if (ffc == null)
						{
							this.bFreezeDockStateChangeEvents = false;
							this.FireDockStateChangeEvent("DockStateChanging", args);
						}
						else
						{
							ffc.HostControl.Bounds = rcscreen;
							dtc.SelectedController = dhc;
							return;
						}
                    }
                    else
                    {
                        this.bFreezeDockStateChangeEvents = false;
                        this.FireDockStateChangeEvent("DockStateChanging", args);
                    }

					dtc.TransitToPrevFloat();
					FloatingForm fform = dtc.ParentController.HostControl as FloatingForm;
					if (fform != null)
						fform.Bounds = rcscreen;
                    dtc.SelectedController = dhc;
                }
                else
                {
                    if (dhc.Floating)
                    {
                        if (dhc.ParentController is FloatingFormController == false)
                        {
                            this.bFreezeDockStateChangeEvents = false;
                            this.FireDockStateChangeEvent("DockStateChanging", args);
                        }
                        dhc.ExitFloatingFrame();
                    }
                    else
                    {
                        this.bFreezeDockStateChangeEvents = false;
                        this.FireDockStateChangeEvent("DockStateChanging", args);
                        this.UndockFromController(dhc);
                    }

                    dhc.DINew = new DockInfo(null, Syncfusion.Windows.Forms.Tools.DockingStyle.Fill, -1, 0, DockPreference.None, rcscreen);
                    (dhc.HostControl as DockHost).DragRectangle = rcscreen;
                    dhc.CreateFloatingFrame(rcscreen.Location, true);

					if( sibling != null )
					{
						Rectangle rctransient = sibling.DITransient.rcDockArea;
						this.UndockFromController(sibling);
						sibling.LayoutRect = rctransient;
						sibling.ToggleAutoHideMode(false);
						sibling.DINew.rcDockArea = rctransient;
						sibling.DITransient.rcDockArea = rctransient;
					}
                }

				//Workaround for context menu not showing when the floated by FloatControl method until move or dock the window for first time. 
				dhc.HostControl.Focus();
                this.FireDockStateChangeEvent("DockStateChanged", args);
                this.bFreezeDockStateChangeEvents = false;
			}

            //Restoring the parent Forms visibility
            if (ctrl.Parent != null && this.PreviousParentForm != null && !this.PreviousParentForm.IsDisposed)
            {
                this.PreviousParentForm.Visible = this.PreviousParentVisibility;
                this.PreviousParentForm = null;
            }
		}

		/// <summary>
		/// Overloaded. Docks the control to the specified dock-enabled parent control.
		/// </summary>
		/// <remarks>
		/// The <see cref="Syncfusion.Windows.Forms.Tools.DockingStyle"/> value provides the docking information and size.
		/// The interpretation of the dockstyle and nsize values depends upon the context of
		/// the dock operation.
		/// </remarks>
		/// <param name="ctrl"> The control to be docked. </param>
		/// <param name="parent"> The parent control that will host the new control. This can be
		/// the <see cref="DockingManager.HostForm"/> or any other dock-enabled control. </param>
		/// <param name="dockstyle"> A <see cref="Syncfusion.Windows.Forms.Tools.DockingStyle"/> value that specifies the dock type position.</param>
		/// <param name="nsize"> Specifies the docked bounds for the control.</param>
		/// <example>
		/// <code lang="C#">
		/// //Code to dock a control to left of HostForm with width 100
		/// dockingManager1.DockControl(panel1,this,Syncfusion.Windows.Forms.Tools.DockingStyle.Left,100);
		/// //Code to dock a control(panel1) to top of another docked control(panel2).
		/// dockingManager1.DockControl(panel1,panel2,Syncfusion.Windows.Forms.Tools.DockingStyle.Top,100);//panel1 will take space from panel2 at the top
		/// //Code to dock a control(panel1) into another docked control(panel2) in tabbed style
		/// dockingManager1.DockControl(panel1,panel2,Syncfusion.Windows.Forms.Tools.DockingStyle.Tabbed,100);
		/// </code>
		/// <code lang="VB">
		/// 'Code to dock a control to left side of HostForm with width 100
		/// DockingManager1.DockControl(Panel1,this,Syncfusion.Windows.Forms.Tools.DockingStyle.Left,100);
		/// 'Code to dock a control(Panel1) to top of another docked control(Panel2).
		/// DockingManager1.DockControl(panel1,panel2,Syncfusion.Windows.Forms.Tools.DockingStyle.Top,100);//Panel1 will take space from Panel2 at the top
		/// 'Code to dock a control(Panel1) into another docked control(Panel2) in tabbed style
		/// DockingManager1.DockControl(Panel1,Panel2,Syncfusion.Windows.Forms.Tools.DockingStyle.Tabbed,100);
		/// </code>
		///</example>
		public virtual void DockControl(Control ctrl, Control parent, Syncfusion.Windows.Forms.Tools.DockingStyle dockstyle, int nsize )
		{
            if (parent != null && parent is Form)
            {                
                this.PreviousParentVisibility = parent.Visible;
            }

			DockControl( ctrl, parent, dockstyle, nsize, false );

            if (parent != null && parent is Form)
            {
                if(parent.Visible != this.PreviousParentVisibility)
                    parent.Visible = this.PreviousParentVisibility;
            }
		}
			
		/// <summary>
		/// Docks the control to the specified dock-enabled parent control.
		/// </summary>
		/// <remarks>
		/// The <see cref="Syncfusion.Windows.Forms.Tools.DockingStyle"/> value provides the docking information and size.
		/// The interpretation of the dockstyle and nsize values depends upon the context of
		/// the dock operation.
		/// </remarks>
		/// <param name="ctrl"> The control to be docked. </param>
		/// <param name="parent"> The parent control that will host the new control. This can be
		/// the <see cref="DockingManager.HostForm"/> or any other dock-enabled control. </param>
		/// <param name="dockstyle"> A <see cref="Syncfusion.Windows.Forms.Tools.DockingStyle"/> value that specifies the dock type\position.</param>
		/// <param name="nsize"> Specifies the docked bounds for the control.</param>
		/// <param name="tabGroup"> Indicates whether to dock whole tab group 
		/// or specified control only. If control is not part of tab group this parameter is ignored. </param>
		/// <example>
		/// <code lang="C#">
		/// //Code to dock a control to left of HostForm with width 100
		/// dockingManager1.DockControl(panel1,this,Syncfusion.Windows.Forms.Tools.DockingStyle.Left,100);
		/// //Code to dock a control(panel1) to top of another docked control(panel2).
		/// dockingManager1.DockControl(panel1,panel2,Syncfusion.Windows.Forms.Tools.DockingStyle.Top,100);//panel1 will take space from panel2 at the top
		/// //Code to dock a control(panel1) into another docked control(panel2) in tabbed style
		/// dockingManager1.DockControl(panel1,panel2,Syncfusion.Windows.Forms.Tools.DockingStyle.Tabbed,100);
		/// //Code to Dock a control(panel1) into another docked control(panel2) in tabbed style with whole tab group
		/// dockingManager1.DockControl(panel1,panel2,DockingStyle.Tabbed,150,false);
		/// </code>
		/// <code lang="VB">
		/// 'Code to dock a control to left side of HostForm with width 100
		/// DockingManager1.DockControl(Panel1,this,Syncfusion.Windows.Forms.Tools.DockingStyle.Left,100);
		/// 'Code to dock a control(Panel1) to top of another docked control(Panel2).
		/// DockingManager1.DockControl(panel1,panel2,Syncfusion.Windows.Forms.Tools.DockingStyle.Top,100);//Panel1 will take space from Panel2 at the top
		/// 'Code to dock a control(Panel1) into another docked control(Panel2) in tabbed style
		/// DockingManager1.DockControl(Panel1,Panel2,Syncfusion.Windows.Forms.Tools.DockingStyle.Tabbed,100);
		/// 'Code to Dock a control(Panel1) into another docked control(Panel2) in tabbed style with whole tab group
		/// DockingManager1.DockControl(Panel1,Panel2,DockingStyle.Tabbed,150,false);
		/// </code>
		///</example>
        
		public virtual void DockControl(Control ctrl, Control parent, Syncfusion.Windows.Forms.Tools.DockingStyle dockstyle, int nsize, 
			bool tabGroup )
		{

            if (this.PreviousParentForm != null && this.PreviousParentForm.Controls.Count > 0)
                this.PreviousParentVisibility = this.PreviousParentForm.Visible;

			LockActivationEvents++;
			if( this.AHInViewTab != null )
			{
				AHInViewTab.StopAllAnimations();
			}
			bool bPrevForbidFreeze = ForbidFreeze;
			ForbidFreeze = true;
            RightToLeft parentRTF = parent.RightToLeft;
			try
			{
				if( ctrl == parent )
				{
					throw new DockingManagerException("Docking control inside itself not allowed.");
				}
				if( dockstyle == DockingStyle.Fill && parent == HostControl )
				{
					throw new DockingManagerException("Docking control with DockingStyle.Fill to DockingManager's host control not allowed.");
				}
				if (dockstyle == DockingStyle.Tabbed && parent == HostControl)
				{
					throw new DockingManagerException("Docking control cannot be tabbed with host form");
				}
				
				Control control = ActiveControl;

				if( GetFloatOnly( ctrl ) || GetFloatOnly( parent ) )
					return;

				DockHostController hostController = this.GetDockHostController( ctrl );
				DockHostController dhcparent = this.GetDockHostController( parent );

				if( dhcparent != null )
				{
					if( dhcparent.bInMDIMode )
						throw new DockingManagerException("Docking to control in mdi child state is not allowed");
					if( !dhcparent.DockVisibility )
						throw new DockingManagerException("Docking to invisible control is not allowed");
				}

                if (!reduceFlicker)
                {
                    if (parent.RightToLeft != RightToLeft.No)
                        parent.RightToLeft = RightToLeft.No;
                }

				MainFormController mfc = alDockAreaControllers[0] as MainFormController;
				bool bHideParent = false;
                this.bFreezeDockStateChangeEvents = true;
				if( dhcparent != null && dhcparent.AutoHideMode && mfc != null )
				{
					if( dockstyle == DockingStyle.Tabbed )
					{
						DockingStyle border = ( dhcparent.ParentController is DockTabController )?
							dhcparent.ParentController.DICurrent.dStyle: 
							dhcparent.DICurrent.dStyle;
						mfc.GetAHTabControl( border ).ShowController( dhcparent, false );
						dhcparent.ToggleAutoHideMode();
						bHideParent = true;
					}
					else
					{
						throw new DockingManagerException( "Only tabbed docking is allowed when the parent control is autohidden" );
					}
				}

				if( hostController != null )
				{
					if( hostController.bInAutoHide )
						hostController.ToggleAutoHideMode(false);

					hostController.DockEdge = dockstyle;
					if( hostController.LayoutRect.Size == Size.Empty )
						hostController.LayoutRect = hostController.DINew.rcDockArea;
				}

                this.bFreezeDockStateChangeEvents = false;

#if SyncfusionFramework2_0

				if (HostForm != null &&
					HostForm.RightToLeftLayout &&
					HostForm.RightToLeft == RightToLeft.Yes && !reduceFlicker)
				{
					if (dockstyle == DockingStyle.Left)
						dockstyle = DockingStyle.Right;
					else if (dockstyle == DockingStyle.Right)
						dockstyle = DockingStyle.Left;
				}

#endif

				if( tabGroup && ( hostController != null ) &&
					( hostController.ParentController is DockTabController ) )
				{
					InternalDockTabGroup( ctrl, parent, dockstyle, nsize );
				}
				else
				{
					// Get the controller representing the specified parent control.
					DockControllerBase dcparent = null;
					if(parent == this.HostControl)
					{
						dcparent = this.dcHostForm;
					}
					else
					{
						if(dhcparent == null)
						{
                            if (parent.RightToLeft != parentRTF)
                            {
                                parent.RightToLeft = parentRTF;
                            }
							Debug.Assert(false, "Parent control has not been enabled for docking.\n");
							return;
						}
						// If the parent controller is a DockTabController then ensure that it is the selected tab.
						if((dhcparent.ParentController != null) && (dhcparent.ParentController is DockTabController))
						{
							DockTabController dtc = dhcparent.ParentController as DockTabController;
							if(dtc.SelectedController != dhcparent)
								dtc.SelectedController = dhcparent;
						}
						dcparent = dhcparent;
					}

					if(this.alEnableDocking.Contains(ctrl) == false)
						this.SetEnableDocking( ctrl, true );
					this.InternalDockControl(ctrl, dcparent, dockstyle, nsize);

                    if (this.PreviousParentForm != null && this.PreviousParentForm.Controls.Count > 0)
                        this.PreviousParentForm.Visible = this.PreviousParentVisibility;
				}

				if( dhcparent != null && bHideParent )
					dhcparent.ToggleAutoHideMode();
				if( ActiveControl != control )
					ActivateControl( control );
			}
			finally
			{
				LockActivationEvents--;
				ForbidFreeze = bPrevForbidFreeze;
                if (parent.RightToLeft != parentRTF)
                {
                    RibbonForm form = parent as RibbonForm;
                    if (form == null)
                    {
                        parent.RightToLeft = parentRTF;
                    }       
                }   
			}
		}

		#endregion

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal DockHostController DHCInFocus
		{
			get { return this.controllerInFocus; }
			set
			{
				if(this.controllerInFocus != value && !this.StopActivationEvents)
				{
					DockHostController dhccurrent = this.controllerInFocus;
					if((this.controllerInFocus != null) && (value != null))
					{
						DockHost dhostcurrent = dhccurrent.HostControl as DockHost;

						if( dhostcurrent != null )
						{
							CaptionPainter captionPainter = dhostcurrent.TitleBar;

							if( null != captionPainter )
							{
								Rectangle rect = captionPainter.CaptionRect;
								if( Renderer.VisualStyle != VisualStyle.Default )
								{
									rect.Location = new Point( Renderer.ThinBorderWidth, Renderer.ThinBorderWidth );
									rect.Size = new Size( Renderer.ControlBounds.Width - 2 * Renderer.ThinBorderWidth,
										Renderer.CaptionWidth );
								}
								dhostcurrent.Invalidate( rect, false );
							}
						}
					}
					this.controllerInFocus = value;
					if( this.controllerInFocus != null )
					{
						if( this.controllerInFocus.HostControl.Controls.Count > 0 )
						{
							Control ctrl = this.controllerInFocus.HostControl.Controls[0];
							if( ctrl != this.m_activeControl )
								SetActiveControl( ctrl );
						}
					}
                    else
                    {
                        SetActiveControl(null);
                    }
						
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal AHTabControl AHInViewTab
		{
			get { return this.autoHideTabInView; }
			set
			{
				if(this.autoHideTabInView != value)
				{
					if( (value != null)&&(this.autoHideTabInView != null) )
						this.autoHideTabInView.HideController(null, false);
					this.autoHideTabInView = value;
				}
			}
		}

		internal ArrayList HiddenFFControllers
		{
			get
			{
				return this.alHiddenFFControllers;
			}
		}

		/// <summary>
		/// Returns the key used for serializing the <see cref="DockingManager"/> state information.
		/// </summary>
		/// <value>A String value.</value>
		/// <remarks>
		/// This method can be overridden to provide a custom serialization key.
		/// </remarks>
		[Syncfusion.Documentation.DocumentationExclude()]
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		protected virtual String PersistKey
		{
			get
			{
				// To preserve backward compatibility use the HostControl.Name property only for non-Form types
				if(this.HostControl is Form)
					return String.Concat(this.HostControl.GetType().ToString(), DockingManager.strPersistKey);
				else
					return String.Concat(this.HostControl.GetType(), this.HostControl.Name, DockingManager.strPersistKey);
			}
		}

		/// <summary>
		/// Raises the ProvidePersistenceID event.
		/// </summary>
		/// <param name="e">
		/// An ProvidePersistenceIDEventArgs object containing data pertaining to this event.
		/// </param>
		/// <remarks>
		/// The OnProvidePresistenceID method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.
		/// <para>Notes to Inheritors:  When overriding OnProvidePresistenceID in a derived
		/// class, be sure to call the base class's OnProvidePresistenceID method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnProvidePresistenceID(ProvidePersistenceIDEventArgs e)
		{
			if(this.ProvidePersistenceID != null)
				this.ProvidePersistenceID(this, e);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual string PersistenceID
		{
			get
			{
				ProvidePersistenceIDEventArgs e = new ProvidePersistenceIDEventArgs(String.Empty);
				this.OnProvidePresistenceID(e);
				if(e.PersistenceID != String.Empty)
					return String.Concat(e.PersistenceID, ":");
				return String.Empty;
			}
		}

		private MainWindowSubclass subclass = null;
        		
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void subclass_OnDeactivate(object sender, Message m)
		{
			if( this.AHInViewTab != null )
			{
				this.ForceHideActiveControl = true;
				this.AHInViewTab.MFMouseMessageHandler(Point.Empty);
			}
		}

        protected void HostForm_OnClose( object sender, Message m )
        {
           bHostFormClosing = true;
           Form hostControlForm = this.HostControl as Form;
           TabbedMDIManager tabbedMDI = TabbedMDIManager.GetManagerForForm(hostControlForm);

           if (tabbedMDI != null)
               if (tabbedMDI.AttachedTo != null)
                   bHostFormClosing = false;
        }

		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Syncfusion.Documentation.DocumentationExclude()
		]
		protected internal IDragProvider DragProvider
		{
			get { return m_dragProvider; }
        }

        
        internal void SaveToStream()
		{
			this.stmLayout.Seek(0, SeekOrigin.Begin);
			this.SaveToStream(this.stmLayout);
		}

		internal void UpdateDesigner()
		{
			Debug.Assert(base.DesignMode == true);
			if(this.stmLayout != null)
			{
				this.stmLayout.Seek(0, SeekOrigin.Begin);
				this.SaveToStream(this.stmLayout);
				IDesigner idsgnr = (this.GetService(typeof(IDesignerHost)) as IDesignerHost).GetDesigner(this);
				if(idsgnr != null)
				{
					IDockingManagerDesignerComponentInvoke iinvoke = idsgnr as IDockingManagerDesignerComponentInvoke;
					Debug.Assert(iinvoke != null);

                    if (AppDomain.CurrentDomain == null || AppDomain.CurrentDomain.DomainManager == null ||
                        AppDomain.CurrentDomain.DomainManager.ApplicationActivator == null)
                    {
                        iinvoke.RaiseComponentChanged();
                    }
				}
			}
            this.SaveDockState();
		}

		internal Object GetPrimarySelection()
		{
			Debug.Assert(base.DesignMode == true);
			ISelectionService iss = this.GetService(typeof(ISelectionService)) as ISelectionService;
			return iss.PrimarySelection;
		}

		internal void SetSelectedComponents(ICollection clln, SelectionTypes seltype)
		{
			Debug.Assert(base.DesignMode == true);
			ISelectionService iss = this.GetService(typeof(ISelectionService)) as ISelectionService;
			iss.SetSelectedComponents(clln, seltype);
		}

		internal object GetServiceFromMgr(Type service)
		{
			Debug.Assert(base.DesignMode == true);
			return this.GetService(service);
		}

		internal void AddController(DockControllerBase dc)
		{
			if(alDockAreaControllers.Contains(dc) == false)
				alDockAreaControllers.Add(dc);
		}

		internal void RemoveController(DockControllerBase dc)
		{
			try
			{
				if(alDockAreaControllers.Contains(dc) == true)
					alDockAreaControllers.Remove(dc);
				DockHostController dhc = dc as DockHostController;
				if( dhc != null )
				{
					dhc.InternalDockWrapper = null;
					dhc.InternalFloatWrapper = null;
					dhc.InternalForm = null;
					dhc.TempWrapper = null;
				}
			}
			catch(ArgumentException e)
			{
				Trace.Write(e.Message);
			}
		}

		internal void AddFFController(FloatingFormController ffc)
		{
			if(this.alFFControllers.Contains(ffc) == false)
			{
				this.alFFControllers.Add(ffc);
				this.alDockAreaControllers.Add(ffc);
			}
		}

		internal void RemoveFFController(FloatingFormController ffc)
		{
			try
			{
				if(this.alFFControllers.Contains(ffc) == true)
				{
					this.alFFControllers.Remove(ffc);
					this.alDockAreaControllers.Remove(ffc);
				}
			}
			catch(ArgumentException e)
			{
				Trace.Write(e.Message);
			}
		}

		internal IntPtr GetFloatingWindow(Control control, Point pt)
		{
			IntPtr result = IntPtr.Zero;
			
			foreach(FloatingFormController controller in alFFControllers)
			{
				if( controller.HostControl != control )
				{
					Rectangle rectScreen = ((FloatingForm)controller.HostControl).DragRectangle;
					if( rectScreen.Contains(pt) && controller.HostControl.Controls.Count != 0 )
					{
						result = controller.HostControl.Controls[0].Handle;
						break;
					}
				}
			}
			
			return result;
		}

		protected void RestoreMdiZOrder( ArrayList list )
		{
			if( list != null && list.Count > 0 )
			{
				foreach( string name in list )
				{
					DockHostController dhc = dcHostForm.QueryController(name) as DockHostController;

					if( dhc != null && dhc.bInMDIMode )
						dhc.ctrlReference.Parent.BringToFront();
				}
			}
		}

		// Run through the list of controllers and if the point lies over any subscribed control,
		// return the dockcontroller that lies at the top of the Z order
		internal DockControllerBase GetDockController(Point pt)
		{
			Control ptctrl = m_dragProvider.GetUnderlyingControl( pt );
			if(ptctrl == null)
				return null;
			DockControllerBase controller = null;

			while( controller == null && ptctrl != null )
			{
				controller = GetController(ptctrl, pt);

				if( controller == null )
					ptctrl = ptctrl.Parent;
			}

			if( controller == null && ptctrl is ContainerControl )
			{
				if( ptctrl.Parent != null )
					controller = GetController(ptctrl.Parent, pt);
			}

			return controller;
		}
		internal DockingManager GetManagerFromPoint(Point pt)
		{
			Rectangle bounds = Rectangle.Empty;
			DockingManager dockMan = null;
			ArrayList managers = new ArrayList( this.alTargetManagers );
			managers.Add( this );

			foreach( DockingManager dm in managers )
			{
				if (dm.HostControl != null)
				{
					if (!(dm.HostControl is Form) && dm.HostControl.ParentForm != null)
						bounds = new Rectangle(dm.HostControl.ParentForm.PointToScreen(dm.HostControl.Location), dm.HostControl.Size);
					else
						bounds = dm.HostControl.Bounds;

					if (bounds.Contains(pt))
					{
						dockMan = dm;
						break;
					}
				}
			}

			return dockMan;
		}

		internal DockingManager GetVisibleDockingManager()
		{
			DockingManager dockMan = null;
			ArrayList managers = new ArrayList( this.alTargetManagers );
			managers.Add( this );

			foreach( DockingManager dm in managers )
			{
				if( dm.dcHostForm.HostControl.Visible )
					dockMan = dm;
			}

			return dockMan;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected DockControllerBase GetDockControllerFromManager(DockingManager dockingmgr, Control ptctrl, Point pt)
		{
			// Get a top level form or the HostControl over which the cursor is currently present
			ContainerControl ptcntrctrl = ptctrl.GetContainerControl() as ContainerControl;
			if(ptcntrctrl == null)
				return null;
			Control ptparentctrl = ptcntrctrl.ParentForm;
			if((ptparentctrl == null) && (ptcntrctrl is FloatingForm))
				ptparentctrl = ptcntrctrl as FloatingForm;	// Is ptcntrctrl a FloatingForm?
			if(ptparentctrl == null && !ptctrl.InvokeRequired)
			{
				if( dockingmgr.HostControl != null && ((ptctrl == dockingmgr.HostControl) 
					|| (Syncfusion.Runtime.InteropServices.NativeMethods.IsChild(dockingmgr.HostControl.Handle, ptctrl.Handle) == true)))
					ptparentctrl = dockingmgr.HostControl;
				else
					return null;
			}

			ArrayList alcontrollers = dockingmgr.DockAreaControllers;
			foreach(DockControllerBase dc in alcontrollers)
			{
				if(dc.HostControl.Visible == false)	// Ignore hidden controllers
					continue;

				ContainerControl dccntrctrl = dc.HostControl.GetContainerControl() as ContainerControl;
				Control dcparentctrl = dccntrctrl.ParentForm;
				if((dcparentctrl == null) && (dccntrctrl is FloatingForm))
					dcparentctrl = dccntrctrl as FloatingForm;
				if(dcparentctrl == null)
				{
					if((dc.HostControl == dockingmgr.HostControl) || (Syncfusion.Runtime.InteropServices.NativeMethods.IsChild(dockingmgr.HostControl.Handle, dc.HostControl.Handle) == true))
						dcparentctrl = dockingmgr.HostControl;
				}

				if(dcparentctrl != ptparentctrl)
					continue;

				if(dc.IsTargetController(pt) == true)
				{
					DockControllerBase dcchild = this.RecurseChildController(dc, pt);
					return (dcchild != null) ? dcchild : dc;
				}
			}

			return null;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected DockControllerBase RecurseChildController(DockControllerBase dc, Point pt)
		{
			if(dc == null)
				return null;

			if(dc.ChildCount <= 0)
			{
				if(dc.IsTargetController(pt) == true)
					return dc;
				return null;
			}

			if(dc.IsTargetController(pt) == false)
				return null;

			for(int i = 0; i < dc.ChildCount; i++)
			{
				DockControllerBase dcchild = this.RecurseChildController(dc.GetChildAt(i), pt);
				if(dcchild != null)
					return dcchild;
			}
			return null;
		}

		internal DockControllerBase GetDockController(ContainerControl ctrl)
		{
			foreach(DockControllerBase dc in this.alDockAreaControllers)
			{
				if(dc.IsTargetController(ctrl) == true)
					return dc;
			}
			return null;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal DockHostController GetDockHostController(Control ctrl)
		{
			// If the control is present in the alcontroller list, return true
			foreach(DockControllerBase dcbase in this.alDockAreaControllers)
			{
				if(dcbase is DockHostController)
				{
					DockHostController dhc = dcbase as DockHostController;
                    if (dhc.HostControl != null)
                    {
                        if (((dhc.HostControl.Controls.Count > 0) && (dhc.HostControl.Controls[0].Equals(ctrl) == true))
                            || (dhc.ctrlReference == ctrl))
                            return dhc;
                    }
				}
			}
			return null;
		}

        /// <summary>
        /// Generate DockHostController of Floating Form in VS 2010 DockBehavior
        /// </summary>
        /// <param name="ctrl">Floating Form</param>
        /// <returns></returns>
        protected internal DockHostController GetFloatingDockHostController(Control ctrl)
        {
            // If the control is present in the alcontroller list, return true
            foreach (DockControllerBase dcbase in this.alDockAreaControllers)
            {
                if (dcbase is DockHostController)
                {
                    DockHostController dhc = dcbase as DockHostController;
                    if (dhc.HostControl != null)
                    {
                        if (((dhc.HostControl.Controls.Count > 0) && (dhc.HostControl.Controls[0].Equals(ctrl) == true)) || dhc.Floating || (dhc.ctrlReference == ctrl))
                            return dhc;
                    }
                }
            }
            return null;
        }

		internal void ApplyDHCInitialSettings()
		{
			foreach(DockControllerBase dcbase in this.alDockAreaControllers)
			{
				if(dcbase is DockHostController)
				{
					DockHostController dhc = dcbase as DockHostController;
					Control hostCtl = dhc.HostControl;

					if( hostCtl.HasChildren )
					{
						Control ctrl = hostCtl.Controls[0];
						ApplyDHCInitialSettings(ctrl);
					}
				}
			}
		}

		private void ApplyDHCInitialSettings( Control ctrl )
		{
            try
            {
                if (this.htText.Contains(ctrl) == true)
                    this.SetDockLabel(ctrl, (String)this.htText[ctrl]);
                if (this.htIcon.Contains(ctrl) == true)
                    this.SetDockIcon(ctrl, (int)this.htIcon[ctrl]);
                if (this.htmIcon.Contains(ctrl) == true)
                    this.SetMDIChildIcon(ctrl, (int)this.htmIcon[ctrl]);
                if (this.htDockAbility.Contains(ctrl) == true)
                    this.SetDockAbility(ctrl, (DockAbility)htDockAbility[ctrl]);
                if (this.htOuterDockAbility.Contains(ctrl) == true)
                    this.SetOuterDockAbility(ctrl, (DockAbility)htOuterDockAbility[ctrl]);
                if (this.htAllowFloating.Contains(ctrl) == true)
                    this.SetAllowFloating(ctrl, false);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error in ApplyDHInitialSettings Method. \n Details:\n"+ exc.Message);
            }
		}

		internal void ApplyDHCAHSettings()
		{
			foreach(Control ctrl in this.listAHOnLoad)
			{
				DockHostController dhc = this.GetDockHostController(ctrl);
				Debug.Assert(dhc != null);
				if( dhc.AutoHideMode == false && dhc.Floating == false
					&& dhc.DockVisibility )
				{
					dhc.AutoHideMode = true;
					DockingStyle dStyle = dhc.ParentController.DICurrent.dStyle;
					AHTabControl ahbordertab = this.dcHostForm.GetAHTabControl(dStyle);
					ahbordertab.HideController(dhc, false);
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void ApplyDHCFloatOnlySettings()
		{
			foreach(Control ctrl in this.htFloatOnly.Keys)
			{
				if((bool)this.htFloatOnly[ctrl] == true)
				{
					DockHostController dhc = this.GetDockHostController(ctrl);
					Debug.Assert(dhc != null);
					dhc.FloatOnly = true;
				}
			}
		}

		internal void ApplyDHCHiddenOnLoadSettings()
		{
			foreach(Control ctrl in this.htHiddenOnLoad.Keys)
			{
				if((bool)this.htHiddenOnLoad[ctrl] == true)
				{
					DockHostController dhc = this.GetDockHostController(ctrl);
					Debug.Assert(dhc != null);
					dhc.DockVisibility = false;
				}
			}
        }

		[Syncfusion.Documentation.DocumentationExclude()]
		private void InternalDockTabGroup( Control ctrl, Control parent, DockingStyle dockstyle, int nsize )
		{
			this.LockHostFormUpdate();
			DockHostController hostController = GetDockHostController( ctrl );
			DockTabController tabController = hostController.ParentController as DockTabController;
			DockHostController selectedController = tabController.SelectedController;
			TabControlAdv  tabControl = tabController.TabControl as TabControlAdv;
			int pageCount = tabControl.TabPages.Count;
			DockTabPage tabPage = tabControl.TabPages[ pageCount - 1 ] as DockTabPage;
			Control lastControl = tabPage.dhcClient.HostControl.Controls[0];

			ArrayList controllers = new ArrayList();
			for( int i = pageCount - 2; i >= 0; i-- )
			{
				DockTabPage page = tabControl.TabPages[i] as DockTabPage;
				controllers.Add( page.dhcClient );
			}
			//Controls are already docked.
			if( lastControl == parent )
				return;

			RedockDockTabControl( tabController, parent, dockstyle, nsize );

			DockTabController newTabController = hostController.ParentController as DockTabController;
			if( newTabController.SelectedController != selectedController )
				newTabController.SelectedController = selectedController;
			this.UnlockHostFormUpdate();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void RedockDockTabControl( DockTabController dtc, Control parent, DockingStyle dockstyle, int nsize )
		{
			if( dtc != null )
			{
				DockControllerBase dhcparent = this.GetDockHostController( parent );
				if( dhcparent == null && parent == this.HostControl )
					dhcparent = this.dcHostForm;

				DockHostController dhc = null;
				if( dtc.ChildControllers.Count > 0 )
					dhc = dtc.ChildControllers[0] as DockHostController;
				if( dhc != null )
				{
					this.InitializeDHCNew( dhc, dhcparent, dockstyle, nsize );

					dtc.DINew = dhc.DINew;
					dtc.DockEdge = dtc.DINew.dStyle;

					if( dhc.Floating == true )
					{
						if( dhc.DINew.dController.Floating == true )
						{
							dtc.TransitFloatToDockInFloat();
						}
						else
						{
							dtc.TransitFloatToDock();
						}
					}
					else
					{
						if( dhc.DINew.dController.Floating == true )
						{
							dtc.TransitDockToDockInFloat();
						}
						else
						{
							dtc.TransitDockToDock();
						}
					}
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void InternalDockControl(Control ctrl, DockControllerBase dcbparent, Syncfusion.Windows.Forms.Tools.DockingStyle dockstyle, int nsize)
		{
			// If the dsDockFillAHBorder autohide border value for the DockToFill mode has been specified and the
			// mainformcontroller's first(and in the case of DockToFill, only) sizingcontroller is yet to be created, then
			// use dsDockFillAHBorder for the Control's dockstyle value. This restricts autohide tabs to the specified border.
			if((this.bDockToFill == true) && (this.dcHostForm.ChildCount == 0) && (this.dsDockFillAHBorder != DockingStyle.Fill))
				dockstyle = this.dsDockFillAHBorder;

			DockHostController dhc = this.GetDockHostController(ctrl);
			if(dhc == null)
			{
				DockHost dhost = this.CreateDockHost(ctrl);
				Debug.Assert(dhost != null);
				dhc = dhost.InternalController as DockHostController;
				if( dcbparent.Floating == true )
				{
					dhc.Floating = true;
				}
				this.InitializeDHCNew(dhc, dcbparent, dockstyle, nsize);
				dhc.DINew.dController.InvokeDocking(dhc);

				if (this.alEnableDocking.Count > 0)
				{					
					if (DesignerHooks.GetMsgProcListContains(this) == false)
						m_curentThreadId = DesignerHooks.AddGetMsgProcListener(this);
				}
			}
			else
			{
				if(dhc.DockVisibility == false)
					return;
				DockTabController tabControl = dhc.ParentController as DockTabController;
				DockHostController sibling = null;

				// Remove from autohide mode, before initializing the newdhc
				if( dhc.AutoHideMode == true )
				{
					if( tabControl != null )
						foreach( DockTabPage page in tabControl.TabControl.TabPages )
						{
							if( page.dhcClient != dhc )
							{
								sibling = page.dhcClient;
								break;
							}
						}

					dhc.AutoHideMode = false;
				}
				// If hosted within a tabgroup, remove from the group
				if( ( dhc.ParentController != null ) && ( dhc.ParentController is DockTabController ) )
					( dhc.ParentController as DockTabController ).RemoveDockHostFromTab( dhc, false );

				this.InitializeDHCNew(dhc, dcbparent, dockstyle, nsize);
				SizingController parentSc = null;

				if( sibling != null )
				{
					UndockFromController(sibling);
					sibling.ToggleAutoHideMode(false);
					DockingStyle border = this.dcHostForm.GetControllerBorder(sibling);
					AHTabControl tabCtrl = this.dcHostForm.GetAHTabControl(border);
					if( tabCtrl != null )
						tabCtrl.HideController(sibling, false, true);
				}

				if( dhc.DINew.dStyle != DockingStyle.Tabbed )
				{
					if( dhc.DINew.dController != null )
					{
						if( dhc.DINew.dController is DockStateControllerBase )
						{
							parentSc = dhc.DINew.dController.ParentController as SizingController;
							if (parentSc != null && parentSc.ChildCount > 2)
								parentSc = dhc.DINew.dController as SizingController;
						}
						else
							parentSc = dhc.DINew.dController as SizingController;
					}

					if( parentSc != null )
					{
						DockPreference dockPref = DockPreference.Horizontal;
						if( dhc.DINew.dStyle == DockingStyle.Top
							|| dhc.DINew.dStyle == DockingStyle.Bottom )
							dockPref = DockPreference.Vertical;

						dhc.PerformSizeCorrection( parentSc, dockPref );
					}
				}
				if(dhc.Floating == true)
				{
					if( dhc.DINew.dController.Floating == true )
						dhc.TransitFloatToDockInFloat();
					else
					{
						dhc.TransitFloatToDock();
						this.dcHostForm.AdjustLayout();
					}
				}
				else
				{
					if(dhc.DINew.dController.Floating == true)
						dhc.TransitDockToDockInFloat();
					else
						dhc.TransitDockToDock();
				}
			}
			dhc.DockEdge = dockstyle;
		}

		
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void InitializeDHCNew(DockHostController dhc, DockControllerBase dcbparent, Syncfusion.Windows.Forms.Tools.DockingStyle dockstyle, int nsize)
		{
			Control ctrl = dhc.HostControl.Controls[0];
			Rectangle rcdock = Rectangle.Empty;
			DockPreference dp = DockPreference.None;
			if((dockstyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Left) || (dockstyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Right))
			{
				rcdock = (nsize > dcbparent.LayoutRect.Width - SplitterWidth)?
					new Rectangle( 0, 0, dcbparent.LayoutRect.Width - SplitterWidth, ctrl.Height ):
					new Rectangle( 0, 0, nsize, ctrl.Height );
				dp = DockPreference.Horizontal;
			}
			else if((dockstyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Top) || (dockstyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Bottom))
			{
				rcdock = (nsize > dcbparent.LayoutRect.Height - SplitterWidth)?
					new Rectangle( 0, 0, ctrl.Width, dcbparent.LayoutRect.Height - SplitterWidth ):
					new Rectangle( 0, 0, ctrl.Width, nsize );
				dp = DockPreference.Vertical;
			}
			else if(dockstyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Tabbed)
			{
				rcdock = new Rectangle(0,0,ctrl.Width,ctrl.Height);	// rcdock ignored for tabbed docking
				dp = DockPreference.Tabbed;
			}
			else if( dockstyle == DockingStyle.Fill )
			{
				rcdock = (dcbparent.DICurrent.DP == DockPreference.Vertical )?
					new Rectangle( 0, 0, dcbparent.LayoutRect.Width, dcbparent.LayoutRect.Height - SplitterWidth ):
					new Rectangle( 0, 0, dcbparent.LayoutRect.Width - SplitterWidth, dcbparent.LayoutRect.Height );
				dp = dcbparent.DICurrent.DP;
				dockstyle = dcbparent.DICurrent.dStyle;
			}
			dhc.DINew = new DockInfo(dcbparent, dockstyle, -1, 0, dp, rcdock);
		}

		// Used by the designer
		internal void SaveToStream(Stream file)
		{
			try
			{
                if( this.DesignMode && this.bInBeginEndInit ) return;

				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
				BinaryFormatter fmtr = new BinaryFormatter();
				fmtr.AssemblyFormat = FormatterAssemblyStyle.Simple;
				fmtr.Binder = AppStateSerializer.CustomBinder;
				fmtr.Serialize(file, new DockingMgrSerializationWrapperAdv(this));
			}
			catch(Exception e)
			{
				Debug.Assert(false, "Failed to save the DockState.", e.Message);
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
		}
		}
	
		// Used by the designer
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool LoadFromStream(Stream file)
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
				BinaryFormatter fmtr = new BinaryFormatter();
				fmtr.AssemblyFormat = FormatterAssemblyStyle.Simple;
				fmtr.Binder = AppStateSerializer.CustomBinder;
				DockingMgrSerializationWrapper dmgrserializer = null;
				try
				{
					Object deserializedobject = fmtr.Deserialize(file);
					dmgrserializer = deserializedobject as DockingMgrSerializationWrapper;
					if((dmgrserializer == null) && (deserializedobject != null))
					{
						// This is a workaround that is used when the Designer fails to deserialize the docking information
						// from the application's resource. This happens when a docking windows project is loaded into a
						// running instance of DEVENV. DEVENV needs to be restarted for the resource to load correctly.
						dmgrserializer = DockingMgrSerializationWrapper.CloneByReflection(deserializedobject);
					}
				}
				catch(Exception)
				{
					return false;
				}

				if(dmgrserializer == null)
					return false;

				if( dmgrserializer is DockingMgrSerializationWrapperAdv )
					this.ApplyDeserializedState(dmgrserializer as DockingMgrSerializationWrapperAdv);
				else
					this.ApplyDeserializedState(dmgrserializer);
			}
			catch(Exception e)
			{
				Debug.Assert(false, "Failed to load the DockState.", e.Message);
				return false;
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			}
			return true;
		}

		protected DockControllerBase ReArrangeControls( DockControllerBase parent, ArrayList controls )
		{
			foreach( LayoutControllerWrapper lcw in controls )
			{
				SizingControllerWrapper scw = lcw as SizingControllerWrapper;
				DockControllerBase dhc = null;

				if( scw != null )
				{
					Control hostControl = null;
					hostControl = parent.ToplevelController.HostControl;
					SizingController sc = new SizingController(this, hostControl
						, scw.Orientation);
					sc.ParentController = parent;
					sc.LayoutRect = scw.LayoutRect;
					sc.DICurrent.dStyle = scw.Style;
					ArrayList child = lcw.Children;

					ReArrangeControls(sc, child);

					sc.LayoutRect = scw.LayoutRect;

					if( sc.ChildCount != 0 )
					{
						RefreshEmptySize( sc );
						parent.InsertChild(sc, parent.ChildCount, scw.Style);
						if( scw.TransientRect.Width > 0 && scw.TransientRect.Height > 0 )
							sc.DITransient.rcDockArea = scw.TransientRect;
					}
				}
				else
				{
					if( lcw is DockHostControllerWrapper )
					{
						DockHostControllerWrapper dhcw = lcw as DockHostControllerWrapper;
						dhc = this.dcHostForm.QueryController(dhcw.UniqueName);

						if( dhc != null )
						{
							DockControllerBase tempParent = dhc.ParentController;
							if( tempParent != null )
								dhc.ParentController.RemoveChild(dhc);

							parent.InsertChild(dhc, parent.ChildCount, parent.DICurrent.dStyle);
							dhc.HostControl.Visible = true;

							if( tempParent != null )
							{
								if( tempParent.ToplevelController != parent.ToplevelController )
									parent.ToplevelController.HostControl.Controls.Add(dhc.HostControl);

								tempParent.Refresh();
							}
						}
					}
					else if( lcw is DockStateWrapper )
					{
						DockStateWrapper dsw = lcw as DockStateWrapper;
						DockStateControllerWrapper wrapper = null;
						ArrayList controllers = new ArrayList();

						if( dsw.Relations.Count < 2 )
						{
                            if (dsw.InternalController.Count > 0)
                            {
                                string uniqueName = dsw.InternalController[0] as string;
                                dhc = this.dcHostForm.QueryController(uniqueName);

                                if (dhc != null)
                                {
                                    if (alautohide.Contains(dhc))
                                    {
                                        DockHostController hostCtrl = dhc as DockHostController;
                                        SizingController parentSizing = parent as SizingController;

                                        if (hostCtrl != null && parentSizing != null)
                                            hostCtrl.PerformSizeCorrection(parentSizing);

                                        DockControllerBase currParent = dhc.ParentController;
                                        currParent.RemoveChild(dhc);
                                        parent.InsertChild(dhc, parent.ChildCount, parent.DICurrent.dStyle);
                                    }
                                    else
                                    {
                                        controllers.Add(dhc);
                                        wrapper = new DockStateControllerWrapper(this, dhc as DockHostController);
                                    }
                                }
                            }
						}
						else
						{
							foreach( string uniqueName in dsw.InternalController )
							{
								DockHostController tempDhc = this.dcHostForm.QueryController(uniqueName) as DockHostController;

								if( tempDhc != null )
								{
									if( wrapper == null )
										wrapper = new DockStateControllerWrapper( this, tempDhc );

									controllers.Add( tempDhc );

									if( tempDhc.DockVisibility && !tempDhc.bInMDIMode && !tempDhc.FloatOnly )
									{
										if( alautohide.Contains( tempDhc ) || dhc == null )
											dhc = tempDhc;
									}
								}
							}
							if( alautohide.Contains(dhc) )
							{
								DockTabController tabController = AssembleTabControl( controllers );

								DockControllerBase tabParent = tabController.ParentController;
												
								tabParent.RemoveChild(tabController);

								if( tabParent is FloatingFormController )
								{
									FloatingForm hostForm = tabParent.HostControl as FloatingForm;

									parent.ToplevelController.HostControl.Controls.Add( tabController.HostControl );
									hostForm.Disable();
								}

								parent.InsertChild(tabController, parent.ChildCount, parent.DICurrent.dStyle);
								tabParent.Refresh();
								wrapper = null;
							}
							else
							{

								foreach( DockRelation dockRelation in dsw.Relations.Values )
								{
									DockInfo info = dockRelation.Relation;

									dhc = this.dcHostForm.QueryController(info.ControlleName);
									info.dController = dhc;
									dockRelation.RelatedControllers = controllers;
								}

								if( wrapper != null )
								{
									wrapper.DockRelationControllers = controllers;
									wrapper.Relations = dsw.Relations;
								}
							}
						}

						if( wrapper != null )
						{
							wrapper.ControlSize = dsw.StoredLayoutSize;

							if( alautohide.Contains(wrapper.InternalControl) )
							{
								wrapper.InternalControl.ParentController.RemoveChild(wrapper.InternalControl);
								parent.InsertChild(wrapper.InternalControl, parent.ChildCount, parent.DICurrent.dStyle);
							}
							else
							{
								parent.AddChild(wrapper, wrapper.DICurrent.dStyle);
								SizingController parentSc = parent as SizingController;
								if( parentSc != null )
									parentSc.HideRelatedSplitter( wrapper );

								foreach( DockStateControllerBase ctrlBase in controllers )
								{
									if( ctrlBase != null )
									{
										if( wrapper.Floating )
										{
											ctrlBase.InternalFloatWrapper = wrapper;
											FloatingForm hostForm = wrapper.ToplevelController.HostControl as FloatingForm;
											if( hostForm != null )
											{
												ctrlBase.SharedForm = hostForm;
												ctrlBase.SharedForm.FormOwner = ctrlBase;
											}
										}
										else
											ctrlBase.InternalDockWrapper = wrapper;
									}
								}
							}
							wrapper.RefreshSize();
						}
					}
					else if( lcw is DockTabControllerWrapper )
					{
						DockHostController leadHost = null;
						DockTabControllerWrapper dockWrapper = lcw as DockTabControllerWrapper;
						ArrayList tabPages = new ArrayList(dockWrapper.Controls);

						foreach( string name in tabPages )
						{
							DockHostController hostController = this.dcHostForm.QueryController( name ) as DockHostController;						

							if( hostController != null )
							{
								if( leadHost == null )
								{
									leadHost = hostController;
								}
								else
								{
									DockControllerBase currentParent = hostController.ParentController;
									currentParent.RemoveChild(hostController);

									if( currentParent is FloatingFormController )
									{
										FloatingForm hostForm = currentParent.HostControl as FloatingForm;
										hostForm.Disable();
									}
									hostController.DINew.nDockIndex = tabPages.IndexOf(name);
									leadHost.InvokeTabbedDocking(hostController);
									leadHost = hostController;
								}
								hostController.HostControl.Visible = true;
							}
						}

						if( leadHost != null )
						{
							DockControllerBase tabControl = leadHost.ParentController;
							DockControllerBase tabParent = leadHost.ParentController.ParentController;

							if( !( tabControl is DockTabController ) )
							{
								tabParent = leadHost.ParentController;
								tabControl = leadHost;
							}

							tabParent.RemoveChild(tabControl);

							if( tabParent.ToplevelController != parent.ToplevelController )
								parent.ToplevelController.HostControl.Controls.Add(leadHost.ParentController.HostControl);

							if( tabParent is FloatingFormController )
							{
								FloatingForm hostForm = tabParent.HostControl as FloatingForm;
								hostForm.Disable();
							}

							parent.InsertChild(tabControl, parent.ChildCount, parent.DICurrent.dStyle);
							tabControl.LayoutRect = dockWrapper.LayoutRect;
							tabParent.Refresh();
						}
					}
				}
			}

			return parent;
		}
		
		protected bool ReArrangeControl( DockControllerBase parent, ArrayList controls, DockHostController dhc )
		{
			ArrayList collectedControls = new ArrayList();
			DockHostController targetController = null;
			LayoutControllerWrapper topParent = GetTopParentWrapper(controls, dhc);

			if( topParent == null )
				return false;

			collectedControls = CollectControllers(topParent.Children);

			int index = -1;
			int innerIndex = -1;

			foreach( ArrayList names in collectedControls )
			{
				int i = names.IndexOf( dhc.UniqueName );

				if( i != -1 )
				{
					innerIndex = i;
					index = collectedControls.IndexOf(names);
					break;
				}
			}

			if( collectedControls.Count == 0 )
			{
				DockHostControllerWrapper seekWrapper = topParent as DockHostControllerWrapper;

				if( seekWrapper != null && seekWrapper.UniqueName == dhc.UniqueName )
					return true;
			}

			DockControllerBase currParent = dhc.ParentController;
			FloatingForm ff = null;

			if( dhc.Floating )
			{
				ff = dhc.HostControl.Parent as FloatingForm;
				dhc.ExitFloatingFrame();
			}
			else
				UndockFromController(dhc);

			if( ff != null )
				( ff.InternalController as FloatingFormController ).RefreshFormCaption();

			dhc.ParentController.RemoveChild(dhc);
			currParent.Refresh();

			if( innerIndex == -1 )
				return false;
			else
			{
				ArrayList siblings = collectedControls[index] as ArrayList;

				if( siblings.Count > 1 )
				{
					foreach( string name in siblings )
					{
						DockHostController host = this.dcHostForm.QueryController(name) as DockHostController;

						if( host != null && host != dhc )
						{
							if( (host.ParentController is DockTabController
								? host.DockTab != null : true) )
							{
								targetController = host;
								break;
							}
						}
					}
				}

				if( targetController != null )
				{
					targetController.InvokeTabbedDocking(dhc);
				}
				else
				{
					SizingControllerWrapper scw = LocateParentWrapper(controls, dhc) as SizingControllerWrapper;

					foreach( LayoutControllerWrapper lcw in scw.Children )
					{
						DockHostControllerWrapper dhcw = lcw as DockHostControllerWrapper;

						if( dhcw != null && dhcw.UniqueName == dhc.UniqueName )
							innerIndex = scw.Children.IndexOf(dhcw);
						else if( lcw is DockTabControllerWrapper )
						{
							DockTabControllerWrapper dtcw = lcw as DockTabControllerWrapper;

							foreach( string name in dtcw.Controls )
							{
								if( name == dhc.UniqueName )
									innerIndex = scw.Children.IndexOf(dtcw);
							}
						}
					}

					if( innerIndex != 0 )
						index--;

					int next = index;
					while( next < collectedControls.Count )
					{
						ArrayList list = collectedControls[next] as ArrayList;

						foreach( string name in list )
						{
							if( name != dhc.UniqueName )
							{
								DockHostController host = this.dcHostForm.QueryController(name) as DockHostController;

								if( host != null && host.Floating == parent.Floating)
									targetController = host;
							}
						}
						if( targetController != null )
							break;

						next++;
					}
					
					if( targetController == null )
					{
						int prev = index - 1;
						while( prev != -1 )
						{
							ArrayList list = collectedControls[prev] as ArrayList;

							foreach( string name in list )
							{
								if( name != dhc.UniqueName )
								{
									DockHostController host = this.dcHostForm.QueryController(name) as DockHostController;

									if( host != null && host.Floating == parent.Floating )
										targetController = host;
								}
							}
							if( targetController != null )
								break;

							prev--;
						}
					}

					if( targetController != null )
					{
                        LayoutControllerWrapper lcw = LocateParentWrapper(controls, targetController);
                        try
                        {   
                            
                            bool isDocked = false;

                            SizingController targetParent = targetController.ParentController as SizingController;
                            SizingControllerWrapper parentWrapper = lcw as SizingControllerWrapper;

                            if (parentWrapper != null && targetParent != null && parentWrapper.Orientation != targetParent.DICurrent.DP)
                            {
                                isDocked = InsertControl(targetController, targetParent, parentWrapper, dhc);
                            }
                            if(!isDocked)
                                InsertControl(targetController, lcw.Children, dhc);
                        }
                        catch
                        {
                            MessageBox.Show("Error while loading state");
                        }
					}
					else
					{
						if( !parent.Floating )
						{
							scw = GetTopParentWrapper(controls, dhc)
								as SizingControllerWrapper;

							if( scw != null )
							{
								Control hostControl = parent.ToplevelController.HostControl;
								hostControl.Controls.Add(dhc.HostControl);

								SizingController sc = new SizingController(this, hostControl
									, scw.Orientation);
								sc.ParentController = parent;
								sc.LayoutRect = scw.LayoutRect;
								sc.DICurrent.dStyle = scw.Style;
								sc.DICurrent.nPriority = scw.Priority;
								sc.AddChild(dhc, DockingStyle.Fill);

								if( scw.Priority == -1 )
									parent.AddChild(sc, scw.Style);
								else
									parent.InsertChild(sc, scw.Priority, scw.Style);
							}
						}
						else
						{	
							dhc.CreateFloatingFrame(dhc.HostControl.PointToScreen(new Point(0, 0)), true);
							this.dcHostForm.AdjustLayout();
						}
					}
				}
			}

			this.dcHostForm.AdjustLayout();
			return true;
		}

        private bool InsertControl(DockHostController targetController, SizingController targetParent, SizingControllerWrapper parentWrapper, DockHostController dhc)
        {
            bool isInserted = false;
            try
            {
                DockHostControllerWrapper dhcWrapper = GetDHCWrapper(parentWrapper, dhc);
                if (dhcWrapper != null)
                {
                    int nSize = dhc.LayoutRect.Height;
                    if (dhcWrapper.DockEdge == DockingStyle.Top || dhcWrapper.DockEdge == DockingStyle.Bottom)
                        nSize = dhc.LayoutRect.Height;
                    else if (dhcWrapper.DockEdge == DockingStyle.Left || dhcWrapper.DockEdge == DockingStyle.Right)
                        nSize = dhc.LayoutRect.Width;

                    this.InternalDockControl(dhc.HostControl.Controls[0], targetController, dhcWrapper.DockEdge, nSize);
                    isInserted = true;
                }
            }
            catch { }
            return isInserted;
        }

        private DockHostControllerWrapper GetDHCWrapper(SizingControllerWrapper parentWrapper, DockHostController dhc)
        {
            DockHostControllerWrapper dhcWrapper = null;
            if (parentWrapper != null && dhc != null)
            {
                foreach (LayoutControllerWrapper dsc in parentWrapper.Children)
                {
                    if (dsc is DockHostControllerWrapper)
                    {
                        if ((dsc as DockHostControllerWrapper).UniqueName == dhc.UniqueName)
                        {
                            dhcWrapper = dsc as DockHostControllerWrapper;
                            dhc.LayoutRect = (dsc as DockHostControllerWrapper).LayoutRect;
                            break;
                        }
                    }
                }
            }
            return dhcWrapper;
        }
        
		private void InsertControl( DockHostController target, ArrayList children, DockHostController dhc )
		{
			DockStateControllerBase dockTarget = target;
			DockTabController dtc = target.ParentController as DockTabController;

			if( dtc != null )
				dockTarget = dtc;

			DockControllerBase parent = target.ParentController;

			if( parent != null )
			{
				int index = GetHostIndex(target, children);

				Size size = target.LayoutRect.Size;
				parent.ToplevelController.HostControl.Controls.Add(dhc.HostControl);

				if( parent.ChildCount < 3 && !( parent is FloatingFormController ) )
				{
                    if (this.m_bLoadingDockState)
                    {
                        parent.InsertChild(dhc, index, dhc.DockEdge);
                    }
                    else
                        parent.InsertChild(dhc, index, target.DICurrent.dStyle);
					dhc.PerformSizeCorrection(parent as SizingController);
				}
				else
				{
					SizingController newSizing = new SizingController(this
						, dockTarget.ToplevelController.HostControl, dockTarget.DICurrent.DP);
					DockControllerBase targetParent = target.ParentController;

					if( targetParent is DockTabController )
						targetParent = targetParent.ParentController;

					targetParent.ReplaceChild(dockTarget, newSizing);
					newSizing.AddChild(dockTarget, dockTarget.DICurrent.dStyle);
					newSizing.LayoutRect = new Rectangle(newSizing.LayoutRect.Location, size);

					newSizing.InsertChild(dhc, index, dockTarget.DICurrent.dStyle);
					dhc.PerformSizeCorrection(newSizing);
				}
			}
		}

		private int GetHostIndex( DockHostController target, ArrayList children )
		{
			int index = -1;

			foreach( LayoutControllerWrapper lcw in children )
			{
				DockHostControllerWrapper dhcw = lcw as DockHostControllerWrapper;

				if( dhcw != null && dhcw.UniqueName == target.UniqueName )
				{
					index = children.IndexOf(dhcw);
				}
				else if( lcw is DockTabControllerWrapper )
				{
					DockTabControllerWrapper dtcw = lcw as DockTabControllerWrapper;

					foreach( string name in dtcw.Controls )
					{
						if( name == target.UniqueName )
						{
							index = children.IndexOf(dtcw);
							break;
						}
					}
				}

				if( index != -1 )
					break;
			}

			if( index != -1 )
				if( index == 0 )
					index = 2;
				else
					index = 0;
			return index;
		}

		private LayoutControllerWrapper GetTopParentWrapper( ArrayList controls, DockHostController dhc )
		{
			LayoutControllerWrapper scwParent = null;

			foreach( LayoutControllerWrapper lcw in controls )
			{
				if( lcw is SizingControllerWrapper )
				{
					SizingControllerWrapper scw = lcw as SizingControllerWrapper;
					scwParent = LocateParentWrapper(scw.Children, dhc);

					if( scwParent != null )
					{
						return scw;
					}
				}
				else if( lcw is DockHostControllerWrapper )
				{
					if( ( lcw as DockHostControllerWrapper ).UniqueName == dhc.UniqueName )
						return lcw;
				}
				else if( lcw is DockTabControllerWrapper )
				{
					DockTabControllerWrapper dtcw = lcw as DockTabControllerWrapper;

					foreach( string hostName in dtcw.Controls )
						if( hostName == dhc.UniqueName )
							return dtcw;
				}
				else if( alautohide.Contains(dhc) && lcw is DockStateWrapper )
				{ 
					DockStateWrapper dsw = lcw as DockStateWrapper;

					foreach( string name in dsw.InternalController )
						if( name == dhc.UniqueName )
							return dsw;
				}
			}

			return null;
		}

		private LayoutControllerWrapper LocateParentWrapper( ArrayList controls, DockHostController dhc )
		{
			LayoutControllerWrapper scwParent = null;

			foreach( LayoutControllerWrapper lcw in controls )
			{
				if( lcw is SizingControllerWrapper )
				{
					SizingControllerWrapper scw = lcw as SizingControllerWrapper;
					scwParent = LocateParentWrapper(scw.Children, dhc);

					if( scwParent != null )
					{
						if( scwParent is SizingControllerWrapper )
							return scwParent;
						else
							return scw;
					}
				}
				else if( lcw is DockHostControllerWrapper )
				{
					if( ( lcw as DockHostControllerWrapper ).UniqueName == dhc.UniqueName )
						return lcw;
				}
				else if( lcw is DockTabControllerWrapper )
				{
					DockTabControllerWrapper dtcw = lcw as DockTabControllerWrapper;

					foreach( string hostName in dtcw.Controls )
						if( hostName == dhc.UniqueName )
							return dtcw;
				}
				else if( alautohide.Contains(dhc) && lcw is DockStateWrapper )
				{
					DockStateWrapper dsw = lcw as DockStateWrapper;

					foreach( string name in dsw.InternalController )
						if( name == dhc.UniqueName )
							return dsw;
				}
			}

			return null;
		}

		protected ArrayList CollectControllers( ArrayList controls )
		{
			ArrayList collectedControllers = new ArrayList();
			foreach( LayoutControllerWrapper lcw in controls )
			{
				if( lcw is SizingControllerWrapper )
				{
					SizingControllerWrapper scw = lcw as SizingControllerWrapper;
					ArrayList childs = CollectControllers(scw.Children);
					collectedControllers.AddRange(childs);
				}
				else if( lcw is DockHostControllerWrapper )
				{
					ArrayList names = new ArrayList();
					names.Add(( lcw as DockHostControllerWrapper ).UniqueName);
					collectedControllers.Add(names);
				}
				else if( lcw is DockTabControllerWrapper )
				{
					DockTabControllerWrapper dtcw = lcw as DockTabControllerWrapper;
					ArrayList names = new ArrayList();

					foreach( string hostName in dtcw.Controls )
						names.Add(hostName);

					collectedControllers.Add(names);
				}
				else if( lcw is DockStateWrapper )
				{
					DockStateWrapper dsw = lcw as DockStateWrapper;
					ArrayList list = new ArrayList();

					foreach( string name in dsw.InternalController )
						foreach( DockHostController dhc in alautohide )
							if( name == dhc.UniqueName )
								list.Add(name);

					collectedControllers.Add(list);
				}
			}

			return collectedControllers;
		}

		internal DockStateControllerWrapper AssembleTabWrapper( InternalDockStateWrapper wrapper )
		{
			ArrayList controllers = new ArrayList();
			DockHostController currHost = null;
			DockStateControllerWrapper assemblie = new DockStateControllerWrapper(this, null);

			foreach( string name in wrapper.Relations.Keys )
			{
				currHost = this.dcHostForm.QueryController( name )
					as DockHostController;

				if( currHost != null )
				{
					controllers.Add(currHost);
				}
			}

            assemblie.DockRelationControllers = controllers;
			assemblie.Relations = wrapper.Relations;

			return assemblie;
		}

		internal protected DockTabController AssembleTabControl( ArrayList hostControllers )
		{
			DockHostController leadHost = null;

			if( hostControllers != null )
			{
				ArrayList bufferCtrls = new ArrayList( hostControllers );
				bufferCtrls.Reverse();

				foreach( DockHostController hostController in bufferCtrls )
				{
					if( hostController != null && hostController.DockVisibility )
					{
						if( leadHost == null )
						{
							leadHost = hostController;
						}
						else
						{
							DockControllerBase currentParent = hostController.ParentController;
							currentParent.RemoveChild( hostController );

							if( currentParent is FloatingFormController )
							{
								FloatingForm hostForm = currentParent.HostControl as FloatingForm;
								hostForm.Disable();
							}
							hostController.DINew.nDockIndex = 0;
							leadHost.InvokeTabbedDocking( hostController );
							leadHost = hostController;
						}
					}
				}
			}

			return leadHost.ParentController as DockTabController;
		}

		internal DockHostController LocateDockController( ControllerWrapper cw )
		{
			DockHostController dockController = null;

			if( cw is FloatingFormControllerWrapper )
			{
				foreach( ControllerWrapper wrapBase in cw.Children )
				{
					dockController = LocateDockController(wrapBase);

					if( dockController != null )
						break;
				}
			}
			else if( cw is SizingControllerWrapper )
			{
				foreach( ControllerWrapper wrapBase in cw.Children )
				{
					dockController = LocateDockController(wrapBase);

					if( dockController != null )
						break;
				}
			}
			else if( cw is DockTabControllerWrapper )
			{
				DockTabControllerWrapper tabWrapper = cw as DockTabControllerWrapper;

				foreach( string hostName in tabWrapper.Controls )
				{
					dockController = this.dcHostForm.QueryController(hostName) as DockHostController;

					if( dockController != null )
						break;
				}
			}
			else if( cw is DockHostControllerWrapper )
			{
				string uniqName = ( cw as DockHostControllerWrapper ).UniqueName;

				dockController = this.dcHostForm.QueryController(uniqName) as DockHostController;
			}

			return dockController;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void ApplyDeserializedState(DockHostController dhc, DHCSerializationWrapper dhcwrapper)
		{
			StopAutoHideAnimation();

			this.dcHostForm.bOverlapSizing = false;	// Do not allow the MainFormController to do the overlap avoidance size reduction.

			// DockHostControllers need to be visible and have unique dock/float states when applying the persisted layout state.
			// Unautohide, turn on visibility, remove from tabbed groups.
			if(dhc.bInMDIMode == true)
			{
				if(dhc.ctrlReference != null)
					this.SetAsMDIChild(dhc.ctrlReference, false);
				else
					dhc.bInMDIMode = false;
			}
			if(dhc.FloatOnly == true)
				dhc.FloatOnly = false;
			if(dhc.AutoHideMode == true)
				dhc.ToggleAutoHideMode();
			if(dhc.DockVisibility == false)
				dhc.DockVisibility = true;
			if( dhc.ParentController is DockTabController )
			{
				DockTabController dtc = dhc.ParentController as DockTabController;
				dtc.RemoveDockHostFromTab(dhc, false);
			}

			dhc.FloatDCRList.Clear();
			dhc.DockDCRList.Clear();

			if(dhc.Floating == true)
				dhc.HostControl.Parent.Visible = false;
			else dhc.HostControl.Visible = false;

			// If a currently docked controller is also docked in the persisted info, then undock and redock it
			// using the deserialized dockinfo.	Similarly, for floating controllers, dock and refloat them using
			// the deserialzed float dockinfo. This toggling of states allows the deserialized docking info to
			// be applied on the dockhosts while using their float/dock relationships.

			if( !dhcwrapper.bInMDIMode && ((dhc.Floating == false)&&(dhcwrapper.dockInfoCurrent.DP != DockPreference.All))
				|| ((dhc.Floating == true)&&(dhcwrapper.dockInfoCurrent.DP == DockPreference.All)) )
			{
				dhc.InvokePrevDockFloatTransition( dhc.Floating );
			}

			// Assign the depersisted lists to the dockhostcontroller
			dhc.DockDCRList = dhcwrapper.alDockDCR;
			dhc.FloatDCRList = dhcwrapper.alFloatDCR;

			// Set the mainformcontroller reference
			if(dhcwrapper.dockInfoCurrent.DP != DockPreference.All)
				dhcwrapper.dockInfoCurrent.dController = this.dcHostForm;
			else
			{
				// The DragRectangle size is used while creating a new floating frame
				(dhc.HostControl as DockHost).DragRectangle = dhcwrapper.dockInfoCurrent.rcDockArea;
			}
			if(dhcwrapper.dockInfoPrevious.DP != DockPreference.All)
				dhcwrapper.dockInfoPrevious.dController = this.dcHostForm;
			dhc.DIPrevious = new DockInfo(dhcwrapper.dockInfoCurrent);

			if( !dhcwrapper.bInMDIMode )
			dhc.InvokePrevDockFloatTransition( true );
			dhc.DIPrevious = new DockInfo(dhcwrapper.dockInfoPrevious);
			dhc.DockVisibility = dhcwrapper.bDockVisibility;
			dhc.FloatOnly = dhcwrapper.bFloatOnly;
			dhc.bInMDIMode = dhcwrapper.bInMDIMode;

			dhc.UniqueName = dhcwrapper.uniqueName;

			// After depersistence, set floatbounds to be empty so that the
			// first floating frame is always created with the actual control bounds.
			if(dhc.Floating == false)
			{
				dhc.DIPrevious.rcDockArea = Rectangle.Empty;
				(dhc.HostControl as DockHost).DragRectangle = Rectangle.Empty;
			}
			if(dhc.DockVisibility == true)
			{
				// Ignore if the control is a non-active tab
				if(((dhc.ParentController is DockTabController) && (dhc.DockTab == null)) == false)
				{
					// If the During application startup floatingForms are displayed from the HostControl's Paint event handler as displaying toplevel forms
					// before the application's main form is displayed causes problems such as failure to apply the FormWindowState.Maximized state,
					// the Form.CreateDialog() method failing due to circular ownership etc.,
					if(dhc.Floating == true)
					{
						if((this.HostControl.IsHandleCreated == true) && (Syncfusion.Runtime.InteropServices.NativeMethods.IsWindowVisible(this.HostControl.Handle) == true))
							dhc.HostControl.Parent.Visible = true;
					}
					dhc.HostControl.Visible = true;
				}
			}

			// Apply the autohide settings
			if((dhcwrapper.bAutoHideMode == true) && (dhc.AutoHideMode == false))
			{
				dhc.ToggleAutoHideMode();
				this.dcHostForm.GetAHTabControl(dhc.DINew.dStyle).HideController(dhc, false);
			}

			// Apply the MDIChild mode settings
			if((dhc.bInMDIMode == true) && (dhc.DockVisibility == true) && (dhc.FloatOnly == false))
			{
				this.SetAsMDIChild(dhc.HostControl.Controls[0], true);
			}

			// Set the control bounds
			if((dhc.bInMDIMode == false) && (dhcwrapper.ctrlSize != Size.Empty))
			{
				DockControllerBase dcblayout;
				if(dhc.ParentController is DockTabController)
					dcblayout = dhc.ParentController.ParentController;
				else
					dcblayout = dhc.ParentController;
				Size sznewsize = Size.Empty;
				if(dhcwrapper.layoutSize == Size.Empty)
					sznewsize = dhcwrapper.ctrlSize;
				else
					sznewsize = new Size( (int)(((float)dhcwrapper.ctrlSize.Width/(float)dhcwrapper.layoutSize.Width)*(float)dcblayout.LayoutRect.Width), (int)(((float)dhcwrapper.ctrlSize.Height/(float)dhcwrapper.layoutSize.Height)*(float)dcblayout.LayoutRect.Height) );
				this.SetControlSize(dhc.HostControl.Controls[0], sznewsize);
			}

			this.dcHostForm.bOverlapSizing = true;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void ApplyDeserializedState( DockHostController dhc, DockingMgrSerializationWrapperAdv dmgrwrapper )
		{
			StopAutoHideAnimation();
			dhc.HostControl.Visible = false;

			this.dcHostForm.bOverlapSizing = false;	// Do not allow the MainFormController to do the overlap avoidance size reduction.

			// DockHostControllers need to be visible and have unique dock/float states when applying the persisted layout state.
			// Unautohide, turn on visibility, remove from tabbed groups.
			if( dhc.bInMDIMode == true )
			{
				if( dhc.ctrlReference != null )
					this.SetAsMDIChild(dhc.ctrlReference, false);
				else
					dhc.bInMDIMode = false;
			}
			if( dhc.FloatOnly == true )
				dhc.FloatOnly = false;
			if( dhc.AutoHideMode == true )
				dhc.ToggleAutoHideMode();
			if( dhc.DockVisibility == false )
				dhc.DockVisibility = true;
			if( dhc.ParentController is DockTabController )
			{
				DockTabController dtc = dhc.ParentController as DockTabController;
				dtc.RemoveDockHostFromTab(dhc, false);
			}
			if( dhc.Floating )
			{
				dhc.InvokePrevDockFloatTransition(false);
				dhc.SharedForm = null;
			}

			dhc.FloatDCRList.Clear();
			dhc.DockDCRList.Clear();

			DHCSerializationWrapper dhcwrapper = dmgrwrapper.GetDHCSerializationWrapper(dhc);
			ArrayList floatWrappers = dmgrwrapper.FloatingControllerWrappers;
			ControllerWrapper cw = dmgrwrapper.controllerWrapper;
			alautohide = new ArrayList();

			if( dhcwrapper.bAutoHideMode || !dhcwrapper.bDockVisibility )
				alautohide.Add(dhc);
			FloatingFormControllerWrapper targetFloatWrapper = null;

			dhc.UniqueName = dhcwrapper.uniqueName;
			dhc.LayoutRect = new Rectangle(dhc.LayoutRect.Location , dhcwrapper.layoutSize);

			foreach( FloatingFormControllerWrapper ffcw in floatWrappers )
			{
				if( ffcw.Children.Count > 0 )
				{
					if( SearchWrappers(ffcw.Children , dhc) )
					{
						targetFloatWrapper = ffcw;
						break;
					}
				}
			}

			if( LocateParentWrapper(cw.Children , dhc) != null )
			{
				ReArrangeControl(this.dcHostForm , cw.Children , dhc);
			}
			else
			{
				bool docked = false;
				if( targetFloatWrapper != null )
				{
					foreach( FloatingFormController ffc in this.alFFControllers )
					{
						if( ReArrangeControl(ffc , targetFloatWrapper.Children , dhc) )
						{
							docked = true;
							break;
						}
					}

					if( !docked )
					{
						dhc.InternalForm = null;
						dhc.SharedForm = null;
						DockControllerBase parent = dhc.ParentController;
						parent.RemoveChild(dhc);
						dhc.DINew.rcDockArea.Height = 0;
						dhc.InternalFloatWrapper = null;
						parent.Refresh();

						FloatingForm ff = dhc.CreateFloatingFrame(targetFloatWrapper.LayoutRect.Location , true);
						ff.Size = targetFloatWrapper.LayoutRect.Size;

						ReArrangeControls(ff.InternalController , targetFloatWrapper.Children);
						ff.Enable(true);
						ff.Used = true;

						( ff.InternalController as FloatingFormController ).RefreshFormCaption();
						ff.InternalController.AdjustLayout();
					}

				}

				FloatingFormController formController = dhc.ToplevelController as FloatingFormController;

				if( formController != null )
					formController.RefreshFormCaption();
			}

			if( dhcwrapper.bAutoHideMode )
				dhc.EnterAutoHideMode();

			dhc.HostControl.Visible = true;

			dhc.DockVisibility = dhcwrapper.bDockVisibility;
		}

		protected bool SearchWrappers(ArrayList wr , DockHostController dhc)
		{
			bool istargetFloatWrapper = false;

			for( int i = 0 ;i < wr.Count ;i++ )
			{
				DockHostControllerWrapper searchedWrapper = wr[i] as DockHostControllerWrapper;

				if( searchedWrapper != null )
				{
					if( searchedWrapper.UniqueName == dhc.UniqueName )
					{
						istargetFloatWrapper = true;
						break;
					}
					else
					{
						istargetFloatWrapper = SearchWrappers( searchedWrapper.Children , dhc );

						if( istargetFloatWrapper )
							break;
					}
				}
			}

			return istargetFloatWrapper;
		}

        /// <summary>
        /// Transfers a dockable control to or from an MDI child window state.
        /// </summary>
        /// <param name="ctrl">The dockable window to be transferred.</param>
        /// <param name="bsetmdi">TRUE to set the control as an MDI child; FALSE to restore to the dockable state.</param>
        /// <param name="layout">The size and position of the MDI Child window</param>
        /// <remarks>
        /// This method is valid only when the form hosting the <see cref="DockingManager"/> is an MDIContainer.
        /// </remarks>
       internal ArrayList GetFloatingWrapper()
		{
			ArrayList fw = new ArrayList();

			foreach( FloatingFormController ffc in this.alFFControllers )
			{
				fw.Add(ffc.GetWrapper());
			}
			foreach( FloatingFormController ffc in this.alHiddenFFControllers )
			{
				fw.Add(ffc.GetWrapper());
			}

			return fw;
		}

		internal ArrayList GetAhOnLoadList()
		{
			ArrayList ahonload = new ArrayList();

			foreach( Control ctrl in listAHOnLoad )
			{
				DockHostController dhc = GetDockHostController( ctrl );

				if( dhc != null )
					ahonload.Add(dhc.UniqueName);
			}

			return ahonload;
		}

		protected internal void SetActiveControl(Control ctrl)
		{
			if( this.m_activeControl != ctrl && !this.StopActivationEvents )
			{
				if( this.m_activeControl != null && bHoldEvents == false )
				{
					this.OnDockControlDeactivated( new DockActivationChangedEventArgs( this.ctrlLastActive ) );
				}

				if( ctrl != null ) 
					this.ctrlLastActive = ctrl;

				this.m_activeControl = ctrl;

				if( ctrl != null && bHoldEvents == false )
				{
                    dactivatedFlag = 1;
					this.OnDockControlActivated( new DockActivationChangedEventArgs( ctrl ) );
				}
			}
		}
              
		// Iterate up the controller hierarchy and get the first vertical/horizontal
		// splittercontroller that is an adjacent sibling of the dcthis controller. The control
		// is sized to it's new bounds by repositioning this parent/sibling splitter.
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool IterSetSplitterPosition(DockControllerBase dcthis, bool bvertsplitter, int ndelta)
		{
			if(dcthis == null)
				return false;
			DockControllerBase dcparent = dcthis.ParentController;
			if(dcparent == null)
				return false;
			IEnumerator ienum = dcparent.ChildEnumerator;
			if((ienum != null) && (dcparent.ChildCount > 1))
			{
				DockControllerBase[] dcbarray = new DockControllerBase[dcparent.ChildCount];
				ienum.Reset();
				int i = 0;
				while(ienum.MoveNext())
					dcbarray[i++] = ienum.Current as DockControllerBase;
				int nthis = Array.IndexOf(dcbarray, dcthis, 0);
				int nprev = nthis-1;
				int nnext = nthis+1;
				if(nnext < dcbarray.GetLength(0))
				{
					DockControllerBase dcbnext = dcbarray[nnext];
					if( (dcbnext is DragSplitterController) &&
						( ((bvertsplitter == true) && (dcbnext.HostControl.Cursor == Cursors.VSplit)) ||
						  ((bvertsplitter == false) && (dcbnext.HostControl.Cursor == Cursors.HSplit)) ) )
					{
						DragSplitterController dcsplitter = dcbnext as DragSplitterController;
						if( (dcparent.ParentController == this.dcHostForm) &&
							((dcparent.DICurrent.dStyle == DockingStyle.Right) || (dcparent.DICurrent.dStyle == DockingStyle.Bottom)) )
						{
							// Order of Controllers is reversed, and +ve delta will reduce the size and vice versa.
							// So use inverse of the delta polarity.
							if(bvertsplitter)
								(dcsplitter.HostControl as DragSplitter).DragDockInfo.rcDockArea.X -= ndelta;
							else
								(dcsplitter.HostControl as DragSplitter).DragDockInfo.rcDockArea.Y -= ndelta;
						}
						else
						{
                            if (bvertsplitter)
                            {
                                bool isOnPrimaryMonitor = Screen.FromControl(dcsplitter.HostControl).Primary;
                                if (isOnPrimaryMonitor && Screen.PrimaryScreen.Bounds.Width < (dcsplitter.HostControl as DragSplitter).DragDockInfo.rcDockArea.X)
                                    (dcsplitter.HostControl as DragSplitter).DragDockInfo.rcDockArea.X = ((dcparent.HostControl) as Form).DesktopBounds.Width;
                                (dcsplitter.HostControl as DragSplitter).DragDockInfo.rcDockArea.X += ndelta;
                            }
                            else
                                (dcsplitter.HostControl as DragSplitter).DragDockInfo.rcDockArea.Y += ndelta;
						}
						dcsplitter.ResizeTargetPanes();
						dcsplitter.ParentController.AdjustLayout();
						return true;
					}
				}
				if(nprev >= 0)
				{
					DockControllerBase dcbprev = dcbarray[nprev];
					if( (dcbprev is DragSplitterController) &&
						( ((bvertsplitter == true) && (dcbprev.HostControl.Cursor == Cursors.VSplit)) ||
						((bvertsplitter == false) && (dcbprev.HostControl.Cursor == Cursors.HSplit)) ) )
					{
						DragSplitterController dcsplitter = dcbprev as DragSplitterController;
						if( (dcparent.ParentController == this.dcHostForm) &&
							((dcparent.DICurrent.dStyle == DockingStyle.Right) || (dcparent.DICurrent.dStyle == DockingStyle.Bottom)) )
						{
							// Order of Controllers is reversed, and +ve delta will reduce the size and vice versa.
							// So use inverse of the delta polarity.
							if(bvertsplitter)
								(dcsplitter.HostControl as DragSplitter).DragDockInfo.rcDockArea.X += ndelta;
							else
								(dcsplitter.HostControl as DragSplitter).DragDockInfo.rcDockArea.Y += ndelta;
						}
						else
						{
							if(bvertsplitter)
								(dcsplitter.HostControl as DragSplitter).DragDockInfo.rcDockArea.X -= ndelta;
							else
								(dcsplitter.HostControl as DragSplitter).DragDockInfo.rcDockArea.Y -= ndelta;
						}
						dcsplitter.ResizeTargetPanes();
						dcsplitter.ParentController.AdjustLayout();
						return true;
					}
				}
			}
			return this.IterSetSplitterPosition(dcparent, bvertsplitter, ndelta);
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void StopAutoHideAnimation()
		{
			MainFormController mfc = alDockAreaControllers[0] as MainFormController;
			if( mfc != null )
			{
				mfc.ahTabCtrlB.StopAllAnimations();
				mfc.ahTabCtrlL.StopAllAnimations();
				mfc.ahTabCtrlR.StopAllAnimations();
				mfc.ahTabCtrlT.StopAllAnimations();
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void DockToFormController(DockStateControllerBase ctrl)
		{
			DockInfo di = ctrl.DINew;
			MainFormController frmcntlr = di.dController as MainFormController;
			if(frmcntlr == null)
			{
				Debug.Assert(false, "Invalid Cast.\n");
				return;
			}

			ctrl.HostControl.Visible = false;

			// Make the dockhost a child of the form that is hosting the feedback controller
			if( frmcntlr.HostControl.Controls.Contains(ctrl.HostControl) == false )
				frmcntlr.HostControl.Controls.Add(ctrl.HostControl);

			ctrl.LayoutRect = ToDockHostRectangle( di.rcDockArea );

			// Create a new sizingcontroller and add this to the mainformcontroller. The sizingcontroller will
			// host the new dockhost
			SizingController sc = new SizingController(this, frmcntlr.HostControl, di.DP);

			//Calculating initial size for sizing controller
			Size delta = Size.Empty;
			switch( di.DP )
			{
				case DockPreference.Horizontal :
					delta.Width = splitterWidth;
					break;
				case DockPreference.Vertical :
					delta.Height = splitterWidth;
					break;
			}
			
			if( this.bDockToFill )
			{
				sc.LayoutRect = di.rcDockArea;
			}
			else
			{
				sc.LayoutRect = ToDockHostRectangle( di.rcDockArea.X, di.rcDockArea.Y,
					di.rcDockArea.Width + delta.Width, di.rcDockArea.Height + delta.Height );
			}

			sc.DICurrent = new DockInfo(di);

			// Insert the controller into the mainform controller list at the priority position.
			// For -1 priority, however, add the controller to the end of the list
			if(di.nPriority == -1)
				frmcntlr.AddChild(sc, di.dStyle);
			else
				frmcntlr.InsertChild(sc, di.nPriority, di.dStyle);

			sc.AddChild(ctrl, di.dStyle);

			frmcntlr.AdjustLayout();

            //Reset the data bindings if temp binding context was created for lazy binding.
            if (ctrl.HostControl.DataBindings.Count == 0 && ctrl.HostControl.BindingContext != null && (ctrl.HostControl.Controls[0] is TextBox ))
                ctrl.HostControl.Controls[0].BindingContext = null;

			ctrl.HostControl.Visible = true;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void DockToNewSizingController(DockStateControllerBase ctrl)
		{
            if (ctrl == null)
                return;

			DockInfo difeedback = ctrl.DINew;
			DockInfo difbcurrent = difeedback.dController.DICurrent;
            int nDockIndex = difbcurrent.nDockIndex;

			// Hiding window and restoring it, makes the transition seamless and prevents flashing
            if(ctrl.HostControl != null)
			    ctrl.HostControl.Visible = false;

			// Make the dockhost a child of the control that is hosting the feedback controller
			ContainerControl cntrctrl = difeedback.dController.HostControl as ContainerControl;

            if (cntrctrl == null)
                return;

			Control parentctrl = null;
			if((cntrctrl == this.HostControl) || (cntrctrl.Parent == this.HostControl))
				parentctrl = this.HostControl;
			if(parentctrl == null)
			{
				if(cntrctrl is Form)
					parentctrl = cntrctrl;
				else
					parentctrl = cntrctrl.ParentForm;
			}

            if (parentctrl == null)
                return;

			if( parentctrl.Controls.Contains(ctrl.HostControl) == false )
				parentctrl.Controls.Add(ctrl.HostControl);

			// Set the control's internal controller layout rect to be equal to the drag rect
			ctrl.LayoutRect = new Rectangle(0,0,difeedback.rcDockArea.Width,difeedback.rcDockArea.Height);

			// The feedback controller should be removed from it's parent controller. The parent controller will
			// now serve as the sizing controller's parent.
			DockControllerBase parent = difeedback.dController.ParentController;

            if (parent == null)
                return;

			parent.RemoveChild(difeedback.dController);

			// Create the new SizingController
			// The controller for the dockhost currently being docked will be the priority child of the new sizing controller.
			// The controller for the dockhost providing visual feedback for the current dockhost, retrievable through
			// the DINew.dController, will the other child of the sizing controller.
			DockPreference dpsc = ((difeedback.dStyle==Syncfusion.Windows.Forms.Tools.DockingStyle.Left)||(difeedback.dStyle==Syncfusion.Windows.Forms.Tools.DockingStyle.Right)) ? DockPreference.Horizontal : DockPreference.Vertical;
			SizingController sc = new SizingController(this, parent.HostControl, dpsc);

			sc.LayoutRect = difeedback.dController.LayoutRect;
			sc.DICurrent = new DockInfo(parent, difbcurrent.dStyle, 0, difbcurrent.nDockIndex, dpsc, sc.LayoutRect);
			sc.ParentController = parent;

			// Now add the feedback controller to the sizing controller and insert the dockhost controller as the
			// other child of the sizingcontroller
			sc.AddChild(difeedback.dController, Syncfusion.Windows.Forms.Tools.DockingStyle.Fill);
			difeedback.dController.DICurrent.dStyle = difbcurrent.dStyle;	// Restore original border

			int nchildindex = 0;
			if( (difeedback.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Right) || (difeedback.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Bottom) )
				nchildindex = 1;
			sc.InsertChild(ctrl, nchildindex, difeedback.dStyle);
            parent.InsertChild(sc, nDockIndex, difbcurrent.dStyle); 
			ctrl.DICurrent.dStyle = difbcurrent.dStyle;		// Reset to feedback controller's border

			// Setting this dock host as the priority control gives it preferential resizing.
			sc.PriorityController = ctrl;
			parent.AdjustLayout();
			sc.PriorityController = null;

			// Restore window visibility
            if(ctrl.HostControl != null)
			    ctrl.HostControl.Visible = true;
		}

		internal void DockToBlankSizingController(DockStateControllerBase ctrl)
		{
			if( this.DockToFill )
			{
				ctrl.DINew.dController = this.dcHostForm;
				DockToNewSizingControllerFillMode(ctrl);
			}
			else
			{
				SizingController sc = new SizingController(this, this.HostControl, ctrl.DICurrent.DP);
				DockControllerBase parent = ctrl.ParentController;
				this.dcHostForm.HostControl.Controls.Add(ctrl.HostControl);

				if( parent != null )
				{
					parent.RemoveChild(ctrl);
					parent.Refresh();
				}

				sc.ParentController = this.dcHostForm;
				sc.AddChild( ctrl , DockingStyle.Left );
				this.dcHostForm.AddChild( sc , DockingStyle.Left );				
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void DockToSizingController(DockStateControllerBase ctrl)
		{
			if( ctrl.DINew.rcDockArea.Size == Size.Empty )
				ctrl.DINew.rcDockArea.Size = new Size( ctrl.LayoutRect.Width, ctrl.LayoutRect.Height );
			
			DockInfo difeedback = ctrl.DINew;
			int nindex = difeedback.nDockIndex;

			// Hiding window and restoring it, makes the transition seamless and prevents flashing
			ctrl.HostControl.Visible = false;

			// Make the dockhost a child of the form that is hosting the feedback controller
			ContainerControl cntrctrl = difeedback.dController.HostControl as ContainerControl;
			Control parentctrl = null;
			if((cntrctrl == this.HostControl) || (cntrctrl.Parent == this.HostControl))
				parentctrl = this.HostControl;
			if(parentctrl == null)
			{
				if(cntrctrl is Form)
					parentctrl = cntrctrl;
				else
					parentctrl = cntrctrl.ParentForm;
			}

			if(parentctrl.Controls.Contains(ctrl.HostControl) == false)
				parentctrl.Controls.Add(ctrl.HostControl);

			// The feedback is provided by the sizing controller. Insert the dockhost into this sizing controller.
			SizingController sc = difeedback.dController as SizingController;

			// If the sizingcontroller is docked within the mainform, then increase it's width/height by the control dimension
			if((sc.ParentController.MainFormController == true) && (this.bDockToFill == false))
			{
				if(sc.DICurrent.DP == DockPreference.Horizontal)
					sc.LayoutRect = new Rectangle(sc.LayoutRect.Left, sc.LayoutRect.Top,
						sc.LayoutRect.Width+difeedback.rcDockArea.Width, sc.LayoutRect.Height);
				else
					sc.LayoutRect = new Rectangle(sc.LayoutRect.Left, sc.LayoutRect.Top,
						sc.LayoutRect.Width, sc.LayoutRect.Height+difeedback.rcDockArea.Height);
			}
			sc.InsertChild(ctrl, nindex, sc.DICurrent.dStyle);
			ctrl.LayoutRect = difeedback.rcDockArea;

			// Set the current dock host as the sizing controller's priority control and invoke adjustlayout
			sc.PriorityController = ctrl;
			sc.ParentController.AdjustLayout();
			sc.PriorityController = null;

			// Restore window visibility
			ctrl.HostControl.Visible = true;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void DockToNewSizingControllerFillMode(DockStateControllerBase ctrl)
		{
			DockInfo di = ctrl.DINew;
			MainFormController frmcntlr = di.dController as MainFormController;
			if(frmcntlr == null)
			{
				Debug.Assert(false, "Invalid Cast.\n");
				return;
			}

			ctrl.HostControl.Visible = false;

			// Make the dockhost a child of the form that is hosting the feedback controller
			if( frmcntlr.HostControl.Controls.Contains(ctrl.HostControl) == false )
				frmcntlr.HostControl.Controls.Add(ctrl.HostControl);

			// Set the control's internal controller layout rect to be equal to the drag rect
			ctrl.LayoutRect = di.rcDockArea;

			// In DockToFill mode, if the HostControl has been specified as dcbparent and the MainFormController
			// already has a child SizingController, then create a new SizingController for the given dockstyle
			// and add this as a child of the MainFormController's child SizingController.
			DockControllerBase mainformsizing = frmcntlr.GetChildAt(0);
			Debug.Assert((mainformsizing is SizingController) && (mainformsizing.ChildCount > 0));
			DockControllerBase existingchild = mainformsizing.GetChildAt(0);
			DockInfo diexistingchild = existingchild.DICurrent;
			mainformsizing.RemoveChild(existingchild);

			// Create the new SizingController
			// The controller for the dockhost currently being docked will be the priority child of the new sizing controller.
			// The existingchild controller will be the other child of the new sizing controller.
			DockPreference dpsc = ((di.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Left)||(di.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Right)) ? DockPreference.Horizontal : DockPreference.Vertical;
			SizingController sc = new SizingController(this, mainformsizing.HostControl, dpsc);
			Rectangle layoutRect = ctrl.LayoutRect;
			bool mustLayout = false;

			if( !(existingchild is DockStateControllerWrapper) )
				layoutRect = existingchild.LayoutRect;

			if (existingchild is SizingController)
				if ((existingchild as SizingController).IsEmpty)
				{
					layoutRect = ctrl.LayoutRect;
					mustLayout = true;
				}

			sc.LayoutRect = layoutRect; 
			sc.DICurrent = new DockInfo(mainformsizing, diexistingchild.dStyle, 0, diexistingchild.nDockIndex, dpsc, sc.LayoutRect);
			mainformsizing.InsertChild(sc, diexistingchild.nDockIndex, diexistingchild.dStyle);

			// Now add the existingchild to the sizing controller and insert the new dockhost controller as the
			// other child of the sizingcontroller
			sc.AddChild(existingchild, Syncfusion.Windows.Forms.Tools.DockingStyle.Fill);
			existingchild.DICurrent.dStyle = diexistingchild.dStyle;	// Restore original border

			int nchildindex = 0;
			if((di.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Right) || (di.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Bottom))
				nchildindex = 1;
			sc.InsertChild(ctrl, nchildindex, di.dStyle);
			ctrl.DICurrent.dStyle = diexistingchild.dStyle;		// Reset to feedback controller's border

			// Add this relationship to the DCRelationship lists of the controllers involved
			DCRelationship dcrexistingchild = existingchild.DCRCurrent;
			int nrelation = sc.GetHashCode();
			sc.DCRCurrent = new DCRelationship(nrelation, false, sc.DICurrent.DP, -1);			
			ctrl.AddToDCR( new DCRelationship(nrelation, true, sc.DICurrent.DP, sc.GetChildHostIndex(ctrl)));
			
			if(existingchild is SizingController)
			{
				SizingController sizingchild = existingchild as SizingController;
				sizingchild.AddToDCRSharedList(new DCRelationship(nrelation, true, sc.DICurrent.DP, sc.GetChildHostIndex(sizingchild)), true);
			}
			else
			{
				existingchild.DCRCurrent = new DCRelationship(nrelation, true, sc.DICurrent.DP, sc.GetChildHostIndex(existingchild));
			}

			// Setting this dock host as the priority control gives it preferential resizing.
            if( sc.GetDockControllers().Count > 1 && this.NeedDTFPriorityController)
                sc.PriorityController = ctrl;
			mainformsizing.AdjustLayout();
			sc.PriorityController = null;

			ctrl.HostControl.Visible = true;

			if (mustLayout)
				sc.ParentController.AdjustLayout();
		}
		private bool m_bForbidFreeze = false;

		internal bool ForbidFreeze
		{
			get
			{
				return m_bForbidFreeze;
			}
			set 
			{
				if( m_bForbidFreeze != value )
				{
					m_bForbidFreeze = value;
				}
			}
		}

		protected internal void InitiateFormDrag( FloatingForm floatForm, Point ptscreen, Point offset )
		{
			Point htCaptionPoint = floatForm.Location;
			if( !SystemInformation.DragFullWindows )
				floatForm.DragOffset = offset;
			htCaptionPoint.Offset( offset.X, offset.Y );
            
            if (SystemInformation.DragFullWindows && !System.Windows.Forms.SystemInformation.TerminalServerSession)
			{
				DockHost dhost = floatForm.Controls[0] as DockHost;
				Control dockedCtrl = dhost.Controls[0];

                NativeMethods.SendMessage(floatForm.Handle, NativeMethods.WM_SYSCOMMAND, 0xf012
                    , NativeMethods.MAKELPARAM(Cursor.Position.X, Cursor.Position.Y));

				if( dockedCtrl != null && !dockedCtrl.Focused 
					&& !(dockedCtrl.Parent is DockingWrapperForm) )
					dockedCtrl.Focus();
			}
			else
			{
				IntPtr result = NativeMethods.SendMessage( floatForm.Handle, NativeMethods.WM_NCHITTEST, 0
				, NativeMethods.MAKELPARAM( htCaptionPoint.X, htCaptionPoint.Y ) );
				if( result.ToInt32() == NativeMethods.HTTOPRIGHT )
					while( result.ToInt32() != NativeMethods.HTCAPTION )
					{
						htCaptionPoint.X -= offset.X;
						result = NativeMethods.SendMessage( floatForm.Handle, NativeMethods.WM_NCHITTEST, 0
							, NativeMethods.MAKELPARAM( htCaptionPoint.X, htCaptionPoint.Y ) );
					}

				NativeMethods.SendMessage( floatForm.Handle, NativeMethods.WM_SETCURSOR, floatForm.Handle,
					NativeMethods.MAKELPARAM( NativeMethods.HTCAPTION, NativeMethods.WM_LBUTTONDOWN ) );
				NativeMethods.PostMessage( floatForm.Handle, NativeMethods.WM_NCLBUTTONDOWN
					, new IntPtr( NativeMethods.HTCAPTION ), new IntPtr( NativeMethods.MAKELPARAM( htCaptionPoint.X, htCaptionPoint.Y ) ) );
				Point newLoc = new Point( Cursor.Position.X - offset.X, Cursor.Position.Y - offset.Y );
				floatForm.Location = newLoc;
			}
		}

		protected internal void ImportControl( DockControllerBase dc )
		{
			DockingManager dmgrprevious = dc.DockingManager;
            dc.PreviousDockingMgr = dc.DockingManager;
			ArrayList dcchildlist = new ArrayList();
			DockUtilities.RecGetChildControllers( dc , dcchildlist );

			foreach( DockHostController dockhostctrl in dcchildlist )
			{
				if (dockhostctrl.ImageIndex > -1 && dmgrprevious.ImageList != null && this.ImageList != null && dockhostctrl.ImageIndex < dmgrprevious.ImageList.Images.Count
					&& dmgrprevious.ImageList != this.ImageList )
				{
					Image icon = dmgrprevious.ImageList.Images[dockhostctrl.ImageIndex];

					if( icon != null )
					{
                        //dmgrprevious.ImageList.Images.RemoveAt( dockhostctrl.ImageIndex );
						string hashCode = dc.HostControl.Controls[0].GetHashCode().ToString();
						this.ImageList.Images.Add(hashCode, icon);
						dockhostctrl.ImageIndex = this.ImageList.Images.IndexOfKey(hashCode);
					}
				}

				CaptionButtonsCollection captionButtons = dockhostctrl.DockingManager.GetCustomCaptionButtons( dockhostctrl.HostControl.Controls[0] );

				if( captionButtons != null )
					this.SetCustomCaptionButtons( dockhostctrl.HostControl.Controls[0] , captionButtons );

				dmgrprevious.RemoveControllerFromDockingManager(dockhostctrl);
				this.AddControllerToDockingManager(dockhostctrl);
			}

            if (dmgrprevious != null)
                dmgrprevious.controllerInFocus = null;

			if( dc.DockingManager != this )
				dc.DockingManager = this;
		}

		// When a control is being undocked, get hold of the parent sizing controller for this control and dispose it.
		// Also set the sizing controller's parent to be the parent controller for this control's sibling controller
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void UndockFromController(DockControllerBase ctrl)
		{
			SetRelativeSize(ctrl);
			ctrl.Deleting = true;
			SizingController sc = ctrl.ParentController as SizingController;
			FloatingFormController floatParent = ctrl.ToplevelController as FloatingFormController;

			PerformFormsExchange( ctrl );
			bool freezed = m_bForbidFreeze;
			m_bForbidFreeze = true;
			if(sc != null)
			{
				DockControllerBase dcparent = sc.ParentController;
				if(sc.ParentController.MainFormController == false)
				{

					// Notify all subscribers that this controller is being undocked. Set this controller's parent sizingcontroller
					// as the new dock target controller for the subscribed controllers.
					DockInfo dievent = new DockInfo(ctrl.ParentController, ctrl.DICurrent.dStyle, ctrl.DICurrent.nPriority,
						-1, ctrl.DICurrent.DP, ctrl.HostControl == null? Rectangle.Empty
						: ctrl.HostControl.RectangleToScreen(ctrl.HostControl.ClientRectangle));
					// Formulate the DCR for the undocking controller's relationship with it's siblings

					DCRelationship dcr = null;
					DockPreference dp = sc.DICurrent.DP;
					int index = sc.GetChildHostIndex(ctrl);
					if (sc.DCRCurrent != null)
						dcr = new DCRelationship(sc.DCRCurrent.nRelation, true, dp, index);
					else
						dcr = new DCRelationship(0, true, dp, index);
					ctrl.FireControllerChanged(dievent, dcr);

					// Get hold of a sibling controller that shares a splitter with this dockhost					
					int nindex = ctrl.DICurrent.nDockIndex;
					DockControllerBase topLevel = ctrl.ToplevelController;

					DockStateControllerBase controller = ctrl as DockStateControllerBase;
					DockStateControllerWrapper wrap = null;

					if( controller != null && !ForbidWrapperLogic )
					{
						if( controller.TempWrapper != null && controller.TempWrapper.ParentController == sc)
							wrap = controller.TempWrapper;
						else
							wrap = new DockStateControllerWrapper(this, controller);

						sc.ReplaceChild(controller, wrap);

						if( sc.Floating )
							controller.InternalFloatWrapper = wrap;
						else
							controller.InternalDockWrapper = wrap;

						wrap.ParentController.AdjustLayout();
						sc = wrap.ParentController as SizingController;
					}
					else
						sc.RemoveChild(ctrl);

					// Reduce the sizingcontroller's width/height by the dimension of the control being undocked.					
					if( topLevel.Floating )
					{
						topLevel.AdjustLayout();						
					}
					else
					{
						ReduceParentSize(ctrl, sc);
					}
				}
				else	// Hosted within the MainFormController
				{
					// Notify all subscribers that this controller is being undocked. Set this controller's parent sizingcontroller
					// as the new dock target controller for the subscribed controllers.
					DockInfo dievent = new DockInfo(ctrl.ParentController, ctrl.DICurrent.dStyle, ctrl.DICurrent.nPriority,
						-1, ctrl.DICurrent.DP, ctrl.HostControl == null ? Rectangle.Empty 
						: ctrl.HostControl.RectangleToScreen(ctrl.HostControl.ClientRectangle));
					// Formulate the DCR for the undocking controller's relationship with it's siblings
					DCRelationship dcr = null;
					if(sc.DCRCurrent != null)
						dcr = new DCRelationship(sc.DCRCurrent.nRelation, true, sc.DICurrent.DP, sc.GetChildHostIndex(ctrl));
					ctrl.FireControllerChanged(dievent, dcr);

					DockStateControllerBase controller = ctrl as DockStateControllerBase;
					DockStateControllerWrapper wrap = null;

					if( controller != null && !ForbidWrapperLogic)
					{
						if( controller.TempWrapper != null && controller.TempWrapper.ParentController == sc)
							wrap = controller.TempWrapper;
						else
							wrap = new DockStateControllerWrapper(this, controller);

						sc.ReplaceChild(controller, wrap);

						if( sc.Floating )
							controller.InternalFloatWrapper = wrap;
						else
							controller.InternalDockWrapper = wrap;

						wrap.ParentController.AdjustLayout();
						sc = wrap.ParentController as SizingController;
					}
					else
						sc.RemoveChild(ctrl);

					if( sc.ChildCount == 0 )
					{
						dievent = new DockInfo(dcparent, sc.DICurrent.dStyle, sc.ParentController.GetChildHostIndex(sc),
							-1, sc.DICurrent.DP, new Rectangle(0,0,sc.LayoutRect.Width,sc.LayoutRect.Height));
						dcparent.RemoveChild(sc);
						sc.FireControllerChanged(dievent, null);
					}
					else
					{
						ReduceParentSize(ctrl, sc);						
					}
				}

				this.dcHostForm.AdjustLayout();
			}
			else if( (ctrl.ParentController != null) &&
				(ctrl.ParentController is FloatingFormController) )	// Control housed in a floating form controller
			{
				// Notify all subscribers that this floating controller is being docked and is no longer a viable docktarget.
				// Update subscribers of the location/size taken up by this controller. This screen rect will be used
				// while refloating the dockhsotcontrollers.

				// If the ctrl's hostcontrol is null, as in the case of a docktab transition, use the parent form's rect
				Control hostcontrol = ctrl.ToplevelController.HostControl;
				DockInfo dievent = new DockInfo(null, Syncfusion.Windows.Forms.Tools.DockingStyle.Fill, 0, -1, DockPreference.All, hostcontrol.Bounds);
				ctrl.FireControllerChanged(dievent, null);

				DockControllerBase dcfloating = ctrl.ParentController;
				
				DockStateControllerBase controller = ctrl as DockStateControllerBase;
				DockStateControllerWrapper wrap = null;

				if( controller != null && !ForbidWrapperLogic)
				{
					if( controller.TempWrapper != null 
						&& controller.TempWrapper.ParentController == dcfloating )
						wrap = controller.TempWrapper;
					else
						wrap = new DockStateControllerWrapper(this, controller);

					dcfloating.ReplaceChild(controller, wrap);
					controller.InternalFloatWrapper = wrap;
				}
				else
					dcfloating.RemoveChild(ctrl);
				
				// Notifying the floating controller subscribers
				dcfloating.FireControllerChanged(dievent, null);
			}

			DockStateControllerBase dcb = ctrl as DockStateControllerBase;

			if( dcb != null )
				dcb.ExitMaxMinState();

			// Remove the control from the parent form's control collection
			ContainerControl cntrctrl = ctrl.HostControl as ContainerControl;
			if( (cntrctrl != null) && (cntrctrl.Parent != null) && (cntrctrl.Parent.Controls.Contains(cntrctrl) == true) )
			{
				if((this.DesignProcess == false) && (cntrctrl.Parent == this.HostControl))
				{
					// Workaround - Removing a control that has the focus from the Controls collection causes the host form
					// to not close when the 'Close' button is pressed. Temporarily transfer focus to the focusholder control.
					try
					{
						DockHostController dhc = ctrl as DockHostController;

						if( dhc != null && !dhc.DockVisibility )
							LockActivationEvents = 0;
						else
							LockActivationEvents++;

						if (cntrctrl.ContainsFocus)
						{
							this.dcHostForm.FocusHolderControl.Visible = true;
							this.dcHostForm.FocusHolderControl.Focus();
							cntrctrl.Parent.Controls.Remove(cntrctrl);
							this.dcHostForm.FocusHolderControl.Visible = false;
						}
					}
					finally
					{
						LockActivationEvents--;
					}
				}
				else
				{
					cntrctrl.Parent.Controls.Remove(cntrctrl);
				}
			}
			if( floatParent != null )
				floatParent.RefreshFormCaption();

			m_bForbidFreeze = freezed;
			ctrl.Deleting = false;
		}
		/// <summary>
		/// Calculates and sets size relations with siblings.
		/// </summary>
		/// <param name="ctrl">controller to calculate relations for.</param>
		internal void SetRelativeSize( DockControllerBase ctrl)
		{
			ArrayList sizes;
			string undockName = string.Empty;
			if( ctrl is DockHostController )
				undockName = ( ctrl as DockHostController ).UniqueName;
			else
				undockName = ctrl.GetHashCode().ToString();
			DockStateControllerBase dscb = ctrl as DockStateControllerBase;
			DockControllerBase prevChild = ctrl;
			if( ctrl.ParentController == null )
				return;

			FloatingFormController floatParent = ctrl.ToplevelController as FloatingFormController;
			if( floatParent == null )
				dscb.PreviousDockSize = dscb.LayoutRect.Size;
			else
				dscb.PreviousFloatSize = dscb.LayoutRect.Size;

			bool floating = dscb.Floating;

			if( dscb != null )
			{
				string siblingName = string.Empty;
				if( floatParent == null )
					sizes = dscb.StoredDockSizes;
				else
					sizes = dscb.StoredFloatSizes;

				DockTabController dtc = ctrl.ParentController as DockTabController;
				if( dtc != null )
					dscb = dtc;

				SizingController sc = dscb.ParentController as SizingController;
				while( sc != null )
				{
					float relation = 0;

					if( sc.ChildCount == 3 )
					{
						int index = sc.GetChildHostIndex(prevChild);
						DockControllerBase sibling = null;

						if( index == 0 )
							sibling = sc.GetChildAt(2);
						else
							sibling = sc.GetChildAt(0);
						if( sibling is DockHostController )
							siblingName = ( sibling as DockHostController ).UniqueName;
						else
							siblingName = sibling.GetHashCode().ToString();
						if( sibling.DITransient.rcDockArea == Rectangle.Empty )
							sibling.DITransient.rcDockArea = sibling.LayoutRect;
						if( prevChild.DITransient.rcDockArea == Rectangle.Empty )
							prevChild.DITransient.rcDockArea = prevChild.LayoutRect;

						if( sc.DockingOrder == DockPreference.Vertical )
						{
							relation = ( float )sibling.DITransient.rcDockArea.Height / ( float )prevChild.DITransient.rcDockArea.Height;
						}
						else
						{
							relation = ( float )sibling.DITransient.rcDockArea.Width / ( float )prevChild.DITransient.rcDockArea.Width;
						}
						DockStateControllerBase siblingController = sibling as DockStateControllerBase;
						string mainName = string.Empty;

						if( prevChild is DockHostController )
							mainName = ( prevChild as DockHostController ).UniqueName;
						else
							mainName = prevChild.GetHashCode().ToString();

						if( siblingController != null )
						{
							if( floating )
								siblingController.StoredFloatSizes.Insert(0, new RelationNamePair(mainName, 1 / relation));
							else
								siblingController.StoredDockSizes.Insert(0, new RelationNamePair(mainName, 1 / relation));
						}
					}
					else
						siblingName = string.Empty;

					ArrayList buffer = new ArrayList();
					foreach( RelationNamePair relationPair in sizes )
						if( relationPair.Name == siblingName && siblingName != string.Empty )
							buffer.Add(relationPair);

					foreach( RelationNamePair relationPair in buffer )
						sizes.Remove(relationPair);

					sizes.Add( new RelationNamePair( siblingName, relation));
					prevChild = sc;
					sc = sc.ParentController as SizingController;
				}
			}
		}

		private ArrayList GetSiblings( DockControllerBase ctrl )
		{
			SizingController scParent = ctrl.ParentController as SizingController;
			ArrayList siblings = new ArrayList();

			if( scParent != null )
			{
				DockControllerBase sibling = null;
				int index = scParent.GetChildHostIndex(ctrl);

				if( scParent.ChildCount == 3 )
				{
					if( index == 0 )
						sibling = scParent.GetChildAt(2);
					else
						sibling = scParent.GetChildAt(0);
				}

				if( sibling != null && !( sibling is DockStateControllerWrapper ) )
				{
					if( sibling is SizingController )
					{
						SizingController siblingSc = sibling as SizingController;
						siblings = siblingSc.GetDockControllers();

						if( index == 2 )
							siblings.Reverse();
					}
					else if( sibling is DockHostController )
					{
						siblings.Add(sibling);
					}
					else if( sibling is DockTabController )
					{
						siblings.Add(( sibling as DockTabController ).HostController);
					}
				}
			}

			return siblings;
		}
		
		// Runs through the list of floating controllers and checks whether any floating controller has a child
		// with a positive relationship with this dockhost
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool AttemptFloatingFormDCRDocking(DockHostController ctrl, IEnumerator iedcr)
		{
			// Iterate the enumerator list and dock onto the floatingform that provides a matching relationship dockhost.
			while(iedcr.MoveNext() == true)
			{
				IEnumerator iedcrnext = new IEnumWrapper(iedcr.Current);
				foreach(FloatingFormController ffc in this.alFFControllers)
				{
					if(ffc.AttemptDCRDocking(ctrl, iedcrnext) == true)
						return true;
				}
			}
			return false;
		}

		// Creates a controller for the main form
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual MainFormController CreateMainFormController(ContainerControl hostcontrol)
		{
			return new MainFormController(this, hostcontrol);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual DockHost CreateDockHost(Control ctrl)
		{
			return new DockHost(this, ctrl);
		}

		/// <summary>
		/// Creates an instance of FloatingForm.
		/// </summary>
		/// <returns> A <see cref = "FloatingForm"/> that has been created. </returns>
		protected internal virtual FloatingForm CreateFloatingForm()
		{
			return new FloatingForm(this);
		}
		ArrayList subscribed = new ArrayList();

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void RecSubscribeHostControlEvents(Control ctrl, bool subscribe)
		{
			if (ctrl != null)
			{
				if (subscribe == true)
				{
					// If in subscribed ArrayList there is a chain(ArrayList of controls where next control is parent of previous) ending with control that is a child of "ctrl", add "ctrl" to the end of this chain.
					// Also add a new chain starting with "ctrl" to "subscribed" ArrayList.
					foreach (ArrayList chain in subscribed)
					{
						if (chain.Count > 0)
						{
							Control ctrlLast = chain[chain.Count - 1] as Control;
							if (ctrlLast.Parent == ctrl)
								chain.Add(ctrl);
						}
					}
					ArrayList alChain = new ArrayList();
					alChain.Add(ctrl);
					subscribed.Add(alChain);

					if (ctrl != this.HostControl)
						ctrl.ControlRemoved += new ControlEventHandler(this.HostControl_ControlRemoved);
					ctrl.ParentChanged += new EventHandler(this.HostControl_ParentChanged);
					ctrl.VisibleChanged += new EventHandler(this.HostControl_VisibleChanged);

					if (ctrl.Parent != null)
						this.RecSubscribeHostControlEvents(ctrl.Parent, subscribe);
				}
				else
				{
					for (int i = 0; i < subscribed.Count; i++)
					{
						ArrayList chain = subscribed[i] as ArrayList;
						if (chain != null && chain.Count > 0)
						{
							Control ctrlHead = chain[0] as Control;
							if (ctrlHead == ctrl)
							{
								foreach (Control c in chain)
								{
									if (c != HostControl)
										c.ControlRemoved -= new ControlEventHandler(this.HostControl_ControlRemoved);
									c.ParentChanged -= new EventHandler(this.HostControl_ParentChanged);
									c.VisibleChanged -= new EventHandler(this.HostControl_VisibleChanged);
								}
								subscribed.Remove(chain);
							}
						}
					}
				}
			}			
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void HostControl_HandleCreated(object sender, EventArgs e)
		{
			if(this.bWaitOnLayoutEvent == true)
				this.HostControl.Layout += new LayoutEventHandler(this.HostControl_Layout);

			if( subclass == null )
			{
				subclass = new MainWindowSubclass();
				subclass.OnDeactivate += new MainWindowSubclass.MessageEventHandler(subclass_OnDeactivate);
				subclass.OnMdiActivate += new Syncfusion.Windows.Forms.Tools.MainWindowSubclass.MessageEventHandler(this.ProcessMDIActivate);
                subclass.OnClose += new MainWindowSubclass.MessageEventHandler(HostForm_OnClose);
			}
			else
				subclass.ReleaseHandle();

			subclass.AssignHandle(HostControl.Handle);
			using(Graphics g = Graphics.FromHwnd( HostControl.Handle ))
			UpdateFonts( g.DpiY );

			AssignNotifyWindow();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void HostControl_SystemColorsChanged(object sender, EventArgs e)
		{
			m_dragProvider.UpdateColors();

			using(Graphics g = Graphics.FromHwnd(HostControl.Handle))
			UpdateFonts(g.DpiY);

			if( this.VisualStyle != VisualStyle.Default )
			{
				this.Renderer.RefreshColors();
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void HostControl_Paint(object sender, PaintEventArgs e)
		{
			// Workaround for defect #19
			Form host = this.HostForm;
			if( host != null )
			{
				foreach( Form child in host.OwnedForms )
				{
					if( child.Modal && child.Visible && this.alFFControllers.Count != 0 )
					{
						return;
					}
				}
			}

			this.HostControl.Paint -= new PaintEventHandler(this.HostControl_Paint);

			// Name the AutoHide TabControls - Required for certain QA test systems
			this.dcHostForm.GetAHTabControl(DockingStyle.Left).Name = String.Concat("AHTabControl_Left", this.HostControl.Name);
			this.dcHostForm.GetAHTabControl(DockingStyle.Top).Name = String.Concat("AHTabControl_Top", this.HostControl.Name);
			this.dcHostForm.GetAHTabControl(DockingStyle.Right).Name = String.Concat("AHTabControl_Right", this.HostControl.Name);
			this.dcHostForm.GetAHTabControl(DockingStyle.Bottom).Name = String.Concat("AHTabControl_Bottom", this.HostControl.Name);

			if(this.bMDIActivatedVisibility == true)
			{
				// If the DockingManager is hosted in a form that is an MDI child then display floatingforms only if
				// the particular form is the active mdi child
				Form owner = null;
				if(this.HostControl is Form)
					owner = this.HostControl as Form;
				else
					owner =	this.HostControl.ParentForm;
				if((owner != null) && (owner.IsMdiChild == true) && (owner.MdiParent.ActiveMdiChild != owner))
					return;
			}

			// Display the FloatingForms
			this.bFloatingVisibility = true;

			foreach(FloatingFormController ffc in this.alFFControllers)
			{
				if( !ffc.HostControl.Visible )
				{
					ffc.HostControl.Visible = true;
			}
		}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void HostForm_Closed( object sender, EventArgs e )
		{
			if((this.DesignMode == false) && (this.bPersistState == true))
			{
				this.SaveDockState();
			}

			foreach( FloatingFormController ffc in this.alFFControllers )
			{
				if( ffc.HostControl.Visible )
				{
					ffc.HostControl.Visible = false;
				}
			}
		}

		internal void ProcessMDIActivate( object sender, Message msg)
		{
			if (this.bMDIActivatedVisibility == true)
			{
				// If either the HostControl or the Form hosting the HostControl is an MDI child form and this form
				// is being activated or deactivated then show or hideItem the floating forms associated with that
				// particular form.
				Form ownrfrm = null;
				if ((this.HostControl is Form) && ((this.HostControl as Form).IsMdiChild == true))
					ownrfrm = this.HostControl as Form;
				if ((ownrfrm == null) && (this.HostControl.ParentForm != null) && (this.HostControl.ParentForm.IsMdiChild == true))
					ownrfrm = this.HostControl.ParentForm;
				if (ownrfrm != null)
				{
					if (msg.LParam == ownrfrm.Handle)	// The child form is being activated
					{
						bool chainVisible = true;
						Control parent = this.HostControl;
						while( parent != null )
						{
							if( !parent.Visible )
							{
								chainVisible = false;
								break;
							}
							parent = parent.Parent;
						}

						if (chainVisible)
						{
							if (this.bFloatingVisibility == false)
							{
								this.dcHostForm.FocusHolderControl.Enable();
								this.dcHostForm.FocusHolderControl.Visible = true;
								this.dcHostForm.FocusHolderControl.Focus();
								this.dcHostForm.FocusHolderControl.LockFocus = true;
								this.bFloatingVisibility = true;
								foreach (FloatingFormController ffc in this.alFFControllers)
								{
									if (ffc.HostControl.Visible == false)
										ffc.HostControl.Visible = true;
								}
								this.dcHostForm.FocusHolderControl.LockFocus = false;
								this.dcHostForm.FocusHolderControl.Visible = false;
								this.dcHostForm.FocusHolderControl.Disable();
							}
						}
					}
					if( msg.WParam == ownrfrm.Handle )
					{
						if (this.bFloatingVisibility == true)
						{
							this.bFloatingVisibility = false;
							foreach( FloatingFormController ffc in this.alFFControllers )
							{
								if( ffc.HostControl.Visible == true )
									ffc.HostControl.Visible = false;
							}
						}
					}
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void HostControl_ParentChanged(object sender, EventArgs e)
		{
			// A control from the HostControl's hierarchy has had a new Parent assigned to it. Travel up the hierarchy and
			// subscribe to the ControlRemoved/ParentChanged event chain.
			Control ctrl = sender as Control;
			if( ctrl.Parent != null || this.HostControl != null )
			{
				this.RecSubscribeHostControlEvents(ctrl.Parent, true);

				// Get the new parent Form and make this the Owner for all the FloatingForm instances
				Form newowner = this.HostControl.ParentForm;
				if( newowner != null ) 
				{
					if( newowner != this.frmOwner )
					{
						if(this.frmOwner != null)
							this.frmOwner.HandleCreated -= new EventHandler(this.OwnerForm_HandleCreated);
						this.frmOwner = newowner;
						this.frmOwner.HandleCreated += new EventHandler(this.OwnerForm_HandleCreated);
					}
					foreach(FloatingFormController ffc in this.alFFControllers)
					{
						Form floatingfrm = ffc.HostControl as Form;
						floatingfrm.Owner = null;
						floatingfrm.Owner = newowner;
						floatingfrm.BringToFront();
					}
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void OwnerForm_HandleCreated(object sender, EventArgs e)
		{
			Trace.Assert(this.frmOwner == sender);

			foreach(FloatingFormController ffc in this.alFFControllers)
			{
				Form floatingfrm = ffc.HostControl as Form;
				floatingfrm.Owner = null;
				floatingfrm.Owner = this.frmOwner;
				floatingfrm.BringToFront();
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void HostControl_ControlRemoved(object sender, ControlEventArgs e)
		{
			// A control from the HostControl's hierarchy has had a child removed. If the removed control is a direct/indirect
			// parent of the HostControl, then unsubscribe the control ie., the event sender from the
			// ControlRemoved/Parentchanged event chain.
			Control parent = this.HostControl;
			while(parent != null)
			{
				if(parent == e.Control)
				{
					Control sendercontrol = sender as Control;
					if(sendercontrol is Form)
					{
						// If the sendercontrol is a Form, then it's likely that this Form could be serving as the
						// Owner of the FloatingForms belonging to this DockingManager. So disassociate all FloatingForms
						// from the owner form. The FloatingForms will be re-assigned a new Owner later on when the HostControl
						// gets a new parent.
						Form senderform = sendercontrol as Form;
						foreach(FloatingFormController ffc in this.alFFControllers)
						{
							FloatingForm fltform = ffc.HostControl as FloatingForm;
							if(fltform.Owner == senderform)
								senderform.RemoveOwnedForm(fltform);
						}
					}
					this.RecSubscribeHostControlEvents(sendercontrol, false);
					break;
				}
				parent = parent.Parent;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void HostControl_VisibleChanged(object sender, EventArgs e)
		{
			// The VisibleChanged event is used for showing/hiding floatingforms when the HostControl is a non-Form type.
			Control ctrl = sender as Control;
			if( this.HostControl == ctrl )
			{
				if( ctrl.Visible == true )
				{
					if( Syncfusion.Runtime.InteropServices.NativeMethods.IsWindowVisible(this.HostControl.Handle) == true )
					{
						if( this.bFloatingVisibility == false )
						{
							this.bFloatingVisibility = true;
							foreach( FloatingFormController ffc in this.alFFControllers )
							{
								if( ffc.HostControl.Visible == false )
									ffc.HostControl.Visible = true;
							}
						}
					}
				}
				else if( this.bHostActivatedVisibility == true )
				{
					if( Syncfusion.Runtime.InteropServices.NativeMethods.IsWindowVisible(this.HostControl.Handle) == false )
					{
						if( this.bFloatingVisibility == true )
						{
							this.bFloatingVisibility = false;
							foreach( FloatingFormController ffc in this.alFFControllers )
							{
								if( ffc.HostControl.Visible == true )
									ffc.HostControl.Visible = false;
							}
						}
					}
				}
			}
		}
		

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void HostControl_Layout(object sender, LayoutEventArgs levent)
		{
			if( this.DesignDockStateLoad )
				return;
			this.HostControl.Layout -= new LayoutEventHandler(this.HostControl_Layout);
			if( this.dcHostForm != null &&
				this.dcHostForm.LayoutRect != this.HostControl.DisplayRectangle )
				this.dcHostForm.LayoutRect = this.HostControl.DisplayRectangle;
			this.bWaitOnLayoutEvent = false;
			this.LockHostFormUpdate();
			//Syncfusion.Runtime.InteropServices.NativeMethodsHelper.SuspendRedrawWindow(this.HostControl.Handle);
			//Syncfusion.Runtime.InteropServices.NativeMethods.LockWindowUpdate(this.HostControl.Handle);

			bool brestoreminmax = this.bApplyMinMaxExtents;
			this.bApplyMinMaxExtents = false;
            bool bretval=false;
			// Invoke EnableDocking for any controls that may have been added before the
			// hostform assignment was done

			foreach(Control ctrl in this.alEnableDocking)
				this.SetEnableDocking(ctrl, true);

			this.ApplyDHCInitialSettings();
			bool bnoapplydesigner = this.bPersistState;
			if(this.bPersistState == true)
			{
				if(this.DesignProcess == false)
					bnoapplydesigner = this.LoadDockState();
				else
					bnoapplydesigner = false;
			}
			if((bnoapplydesigner == false) && (this.stmLayout != null) && (this.stmLayout.Length > 0))
			{
                try
                {
                    this.OnNewDockStateBeginLoad(EventArgs.Empty);
                    this.stmLayout.Seek(0, SeekOrigin.Begin);
                    this.m_isInitializing = true;                    
                    this.LoadFromStream(this.stmLayout);                    
                    if (this.DesignProcess == false)
                        this.ApplyDHCAHSettings();
                    this.ApplyDHCFloatOnlySettings();
                    if (this.DesignProcess == false)
                        this.ApplyDHCHiddenOnLoadSettings();
                    bretval = true;
                }
                catch (Exception e)
                {
                    Debug.Assert(false, "LoadDockState Failed.", e.Message);
                }
                finally
                {
                    DockStateLoadEventArgs args = new DockStateLoadEventArgs(bretval);
                    this.OnNewDockStateEndLoad(args);
                    this.m_isInitializing = false;
                }
                
			}
			//Syncfusion.Runtime.InteropServices.NativeMethodsHelper.ResumeRedrawWindow(this.HostControl.Handle, true);
			//Syncfusion.Runtime.InteropServices.NativeMethods.LockWindowUpdate(IntPtr.Zero);

			// Set the TabIndex for the DockHost controls to be equal to the TabIndex of the client control.
			// This is needed for maintaining the focus/tabbing order
			foreach(Control ctrl in this.alEnableDocking)
			{
				if(ctrl.Parent != null)
					ctrl.Parent.TabIndex = ctrl.TabIndex;
			}

			// For a hosted design-time environment initially hideItem all floating controllers. Floating forms are displayed
			// when the control is clicked.
			if((this.DesignProcess == true) && (this.DesignMode == false))
			{
				if((this.HostControl is Form) == false)
				{
					foreach(DockControllerBase dcb in this.alFFControllers)
						dcb.HostControl.Visible = false;
				}
			}

			this.bApplyMinMaxExtents = brestoreminmax;
			this.UnlockHostFormUpdate();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void HostControl_RightToLeftChanged( object objSender, EventArgs eaArgs )
		{
			Control ctrlSender = objSender as Control;
			if (null != ctrlSender)
			{
				this.UpdateRightToLeftProperty( ctrlSender.RightToLeft );
				RestoreFloatingForms();
			}
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.ImageListChanged"/> event.
		/// </summary>
		/// <param name="arg">An EventArgs that contains the event data.</param>
		protected virtual void OnImageListChanged(EventArgs arg)
		{
			if(ImageListChanged != null)
				ImageListChanged(this, arg);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void FireDragAllowEvent(DragAllowEventArgs arg)
		{
			if(this.bHoldEvents == false)
				this.OnDragAllow(arg);
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.DragAllow"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="DragAllowEventArgs"/> that contains the event data.</param>
		protected virtual void OnDragAllow(DragAllowEventArgs arg)
		{
			if(this.DragAllow != null)
				this.DragAllow(this, arg);
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void FireDockAllowEvent(DockAllowEventArgs arg)
		{
			if(this.bHoldEvents == false)
				this.OnDockAllow(arg);
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.DockAllow"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="DockAllowEventArgs"/> that contains the event data.</param>
		protected virtual void OnDockAllow(DockAllowEventArgs arg)
		{
			if(this.DockAllow != null)
				this.DockAllow(this, arg);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void FireDockStateChangeEvent(String strevent, DockStateChangeEventArgs arg)
		{
			if(this.bHoldEvents == false && this.bFreezeDockStateChangeEvents == false)
			{
				if(strevent == "DockStateChanging")
					this.OnDockStateChanging(arg);
				else if(strevent == "DockStateChanged")
					this.OnDockStateChanged(arg);
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void FireControlSizeStateChanged( ControlSizeStates newState, ControlSizeStateChangedEventArgs args )
		{
			switch( newState )
			{ 
				case ControlSizeStates.Maximized:
					OnControlMaximized(args as ControlMaximizedEventArgs);
					break;

				case ControlSizeStates.Maximize:
					OnControlMaximizing(args as ControlMaximizeEventArgs);
					break;

				case ControlSizeStates.Minimize:
					OnControlMinimized(args as ControlMinimizedEventArgs);
					break;

				case ControlSizeStates.Restore:
					OnControlRestored(args as ControlRestoredEventArgs);
					break;
			}
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.ControlMinimized"/> event.
		/// </summary>
		/// <param name="args">A <see cref="ControlMinimizedEventArgs"/> that contains the event data.</param>
		protected virtual void OnControlMinimized( ControlMinimizedEventArgs args )
		{
			if( this.ControlMinimized != null )
				this.ControlMinimized(this, args);
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.ControlMaximizing"/> event.
		/// </summary>
		/// <param name="args">A <see cref="ControlMaximizeEventArgs"/> that contains the event data.</param>
		protected virtual void OnControlMaximizing( ControlMaximizeEventArgs args )
		{
			if( this.ControlMaximizing != null )
				this.ControlMaximizing(this, args);
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.ControlMaximized"/> event.
		/// </summary>
		/// <param name="args">A <see cref="ControlMaximizeEventArgs"/> that contains the event data.</param>
		protected virtual void OnControlMaximized( ControlMaximizedEventArgs args )
		{
			if( this.ControlMaximized != null )
				this.ControlMaximized(this, args);
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.ControlRestored"/> event.
		/// </summary>
		/// <param name="args">A <see cref="ControlRestoredEventArgs"/> that contains the event data.</param>
		protected virtual void OnControlRestored( ControlRestoredEventArgs args )
		{
			if( this.ControlRestored != null )
				this.ControlRestored(this, args);
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.DockStateChanging"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="DockStateChangeEventArgs"/> that contains the event data.</param>
		protected virtual void OnDockStateChanging(DockStateChangeEventArgs arg)
		{
			if(this.DockStateChanging != null)
				this.DockStateChanging(this, arg);
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.DockStateChanged"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="DockStateChangeEventArgs"/> that contains the event data.</param>
		protected virtual void OnDockStateChanged(DockStateChangeEventArgs arg)
		{
			if(this.DockStateChanged != null)
				this.DockStateChanged(this, arg);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void FireDockVisibilityChangedEvent(DockVisibilityChangedEventArgs arg)
		{
			if(this.bHoldEvents == false)
				this.OnDockVisibilityChanged(arg);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void FireDockVisibilityChangingEvent (DockVisibilityChangingEventArgs arg)
		{
			if (this.bHoldEvents == false)
				this.OnDockVisibilityChanging(arg);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void FireDockMenuClickEvent(Control dockControl, DockingStyle ds)
		{
			if( this.bHoldEvents == false )
			{
				DockMenuClickEventArgs arg = new DockMenuClickEventArgs(dockControl, ds);
				this.OnDockMenuClick(arg);
			}
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.DockVisibilityChanged"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="DockVisibilityChangedEventArgs"/> that contains the event data.</param>
		protected virtual void OnDockVisibilityChanged(DockVisibilityChangedEventArgs arg)
		{
			if(this.DockVisibilityChanged != null)
				this.DockVisibilityChanged(this, arg);
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.DockVisibilityChanging"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="DockVisibilityChangingEventArgs"/> that contains the event data. </param>
		protected virtual bool OnDockVisibilityChanging(DockVisibilityChangingEventArgs arg)
		{
			if (this.DockVisibilityChanging != null)
			this.DockVisibilityChanging(this, arg);
			return (!arg.Cancel);
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.DockMenuClick"/> event.
		/// </summary>
		/// <param name="arg"> A <see cref="DockMenuClickEventArgs"/> that contains the event data. </param>
		protected virtual void OnDockMenuClick( DockMenuClickEventArgs arg )
		{
			if( this.DockMenuClick != null )
				this.DockMenuClick( this, arg );
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.DockControlActivated"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="DockActivationChangedEventArgs"/> that contains the event data.</param>
		protected virtual void OnDockControlActivated(DockActivationChangedEventArgs arg)
		{
			if(this.DockControlActivated != null && bAllowActivationEvents)
			{
				if( mouseActivatedControl == null || mouseActivatedControl == arg.Control )
				{
					lastActivated = arg.Control;
					mouseActivatedControl = null;

					if( LockActivationEvents <= 0 )
						this.DockControlActivated(this, arg);
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.DockControlDeactivated"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="DockActivationChangedEventArgs"/> that contains the event data.</param>
		protected virtual void OnDockControlDeactivated(DockActivationChangedEventArgs arg)
		{
			if(this.DockControlDeactivated != null && LockActivationEvents <= 0 &&
				lastActivated == arg.Control && bAllowActivationEvents )
			{
                dactivatedFlag = 1;
				this.DockControlDeactivated(this, arg);
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void FireAutoHideAnimationEvent(String strevent, AutoHideAnimationEventArgs arg)
		{
			if(this.bHoldEvents == false)
			{
				if(strevent == "AutoHideAnimationStart")
					this.OnAutoHideAnimationStart(arg);
				else if(strevent == "AutoHideAnimationStop")
					this.OnAutoHideAnimationStop(arg);
			}
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.AutoHideAnimationStart"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="AutoHideAnimationEventArgs"/> that contains the event data.</param>
		protected virtual void OnAutoHideAnimationStart(AutoHideAnimationEventArgs arg)
		{
			if(this.AutoHideAnimationStart != null)
				this.AutoHideAnimationStart(this, arg);
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.AutoHideAnimationStop"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="AutoHideAnimationEventArgs"/> that contains the event data.</param>
		protected virtual void OnAutoHideAnimationStop(AutoHideAnimationEventArgs arg)
		{
            DockHost dh = arg.Control.Parent as DockHost;
            if (dh.DisplayRectangle != Rectangle.Empty)
                arg.RollState = AutoHideRollState.RolledOut;
            else
                arg.RollState = AutoHideRollState.RolledIn;
			this.ForceHideActiveControl = false;
			if(this.AutoHideAnimationStop != null)
				this.AutoHideAnimationStop(this, arg);
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.NewDockStateBeginLoad"/> event.
		/// </summary>
		/// <param name="arg">An <see cref="System.EventArgs"/> that contains the event data.</param>
		protected virtual void OnNewDockStateBeginLoad(EventArgs arg)
		{
			if(this.NewDockStateBeginLoad != null)
				this.NewDockStateBeginLoad(this, arg);
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.NewDockStateEndLoad"/> event.
		/// </summary>
		/// <param name="arg">An <see cref="System.EventArgs"/> that contains the event data.</param>
		protected virtual void OnNewDockStateEndLoad(EventArgs arg)
		{
			if(this.NewDockStateEndLoad != null)
				this.NewDockStateEndLoad(this, arg);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void FireDragFeedbackEvent(String strevent)
		{
			if(this.bHoldEvents == false)
			{
				if(strevent == "DragFeedbackStart")
					this.OnDragFeedbackStart(EventArgs.Empty);
				else if(strevent == "DragFeedbackStop")
					this.OnDragFeedbackStop(EventArgs.Empty);
			}
		}

        private bool isHostFormTopMost = false;
		/// <summary>
		/// Raises the <see cref="DockingManager.DragFeedbackStart"/> event.
		/// </summary>
		/// <param name="arg">An <see cref="System.EventArgs"/> that contains the event data.</param>
		protected virtual void OnDragFeedbackStart(EventArgs arg)
		{
			if(this.DragFeedbackStart != null)
				this.DragFeedbackStart(this, arg);
            if (this.HostForm != null)
            {
                if (this.HostForm.TopMost)
                {
                    isHostFormTopMost = true;
                    this.HostForm.TopMost = false;
                }
                else
                {
                    isHostFormTopMost = false;
                }
            }
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.DragFeedbackStop"/> event.
		/// </summary>
		/// <param name="arg">An <see cref="System.EventArgs"/> that contains the event data.</param>
		protected virtual void OnDragFeedbackStop(EventArgs arg)
		{
            if (isHostFormTopMost)
            {
                isHostFormTopMost = false;
                this.HostForm.TopMost = true;
            }
			if(this.DragFeedbackStop != null)
				this.DragFeedbackStop(this, arg);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void FireProvideGraphicsItemsEvent(ProvideGraphicsItemsEventArgs arg)
		{
			if(this.bHoldEvents == false && arg != null)
				this.OnProvideGraphicsItems(arg);
		}
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal void FireAutoHideTabContextMenuEvent(AutoHideTabContextMenuEventArgs ahcmenuargs)
        {
                OnAutoHideTabContextMenu(ahcmenuargs);
        }
        /// <summary>
        /// Raises the <see cref="DockingManager.AutoHideTabContextMenu"/> event.
        /// </summary>
        /// <param name="ahcmenuargs">A <see cref="DockingManager.AutoHideTabContextMenuEventArgs"/> that contains the event data.</param>
		
        protected virtual void OnAutoHideTabContextMenu(AutoHideTabContextMenuEventArgs ahcmenuargs)
        {
            if (AutoHideTabContextMenu != null)
                AutoHideTabContextMenu(null, ahcmenuargs);
        }
		/// <summary>
		/// Raises the <see cref="DockingManager.ProvideGraphicsItems"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="DockingManager.ProvideGraphicsItemsEventArgs"/> that contains the event data.</param>
		protected virtual void OnProvideGraphicsItems(ProvideGraphicsItemsEventArgs arg)
		{
			if(this.ProvideGraphicsItems != null)
            {
				try
				{
					this.ProvideGraphicsItems(this, arg);
				}
				catch(Exception)
				{
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.DockStateUnavailable"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="DockStateUnavailableEventArgs"/> that contains the event data.</param>
		protected virtual void OnDockStateUnavailable(DockStateUnavailableEventArgs arg)
		{
			if(this.DockStateUnavailable != null)
				this.DockStateUnavailable(this, arg);
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.InitializeControlOnLoad"/> event.
		/// </summary>
		/// <param name="args">A <see cref="InitializeControlOnLoadEventArgs"/> value that contains the event data.</param>
		protected void OnInitializeControlOnLoad(InitializeControlOnLoadEventArgs args)
		{
			if(this.InitializeControlOnLoad != null)
				this.InitializeControlOnLoad(this, args);
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.TransferringFromManager"/> event.
		/// </summary>
		/// <param name="args">A <see cref="TransferManagerEventArgs"/> that contains the event data.</param>
		protected void OnTransferringFromManager(TransferManagerEventArgs args)
		{
			if(this.TransferringFromManager != null)
				this.TransferringFromManager(this, args);
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.TransferredToManager"/> event.
		/// </summary>
		/// <param name="args">A <see cref="TransferManagerEventArgs"/> that contains the event data.</param>
		protected void OnTransferredToManager(TransferManagerEventArgs args)
		{
			if(this.TransferredToManager != null)
				this.TransferredToManager(this, args);
		}

		/// <summary>
		/// 
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void InvalidateDockControllers()
		{
			foreach(DockControllerBase dcb in alDockAreaControllers)
			{
				DockHostController hostController = dcb as DockHostController;
				if( hostController != null )
				{
					Control control = hostController.HostControl;
					if( hostController.ParentController is DockTabController )
						hostController.ParentController.AdjustLayout();
					// need to repaint panel's header.
					// workaroud. Do the same as in DockHostController.HideCaption
					control.Invalidate();
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void InitializeDefaultMenu(DockHostController dhc, PopupMenu menu)
		{
			BarItem dockableitem = null;
			Form hostControlForm = HostControl as Form;
            if (hostControlForm != null || HostControl != null)
			{
				dockableitem = new BarItem(SR.GetString(SR.DockableMenuItemText, dhc.HostControl.Controls[0]), new EventHandler(this.OnMenuDockable_Click));
                if (this.DockBehavior == Tools.DockBehavior.VS2010)
                {
                    dockableitem.Text = "Dock";
                    if (!dhc.Floating && !dhc.bInAutoHide)
                        dockableitem.Enabled = false;
                    else
                    {
                        dockableitem.Enabled = true;
                        dockableitem.Checked = false;
                    }
                }
				dockableitem.Tag = dhc.HostControl.Controls[0];
                if ((dhc.FloatOnly == false) && (dhc.bInMDIMode == false))
                {
                    if(this.VisualStyle != Forms.VisualStyle.Metro)
                        dockableitem.Checked = true;
                }
                
				menu.ParentBarItem.Items.Add(dockableitem);
			}

			BarItem mdichilditem = null;
			menu.ParentBarItem.Style = this.VisualStyle;
            if (this.VisualStyle == Forms.VisualStyle.Metro)
            {
                menu.ParentBarItem.BarItemBackColor = Color.FromArgb(231, 232, 236);
                menu.ParentBarItem.MetroColor = Color.FromArgb(248, 249, 250);
            }
			bool suitable = false;

			if(( this.GetOuterDockAbility(dhc.HostControl.Controls[0]) & DockAbility.Tabbed) != 0 )
				suitable = true;

			if( MenuStyle == DockMenuStyle.VS2005 && suitable )
			{
				bool bTabbedMDI = false;
				if( hostControlForm != null )
				{
					TabbedMDIManager tabbedMDI = TabbedMDIManager.GetManagerForForm( hostControlForm );
					if( tabbedMDI != null )
					{
						bTabbedMDI = true;
					}
				}
				if( bTabbedMDI )
				{
					mdichilditem = new BarItem(SR.GetString(SR.TabbedMDIMenuItemText, dhc.HostControl.Controls[0]), new EventHandler(this.OnMenuMDIChild_Click));
				}
				else
				{
					mdichilditem = new BarItem(SR.GetString(SR.MDIChildMenuItemText, dhc.HostControl.Controls[0]), new EventHandler(this.OnMenuMDIChild_Click));
				}
				mdichilditem.Tag = dhc.HostControl.Controls[0];
				mdichilditem.Enabled = ( hostControlForm != null && hostControlForm.IsMdiContainer 
					&& !dhc.AutoHideMode );
				mdichilditem.Checked = dhc.bInMDIMode;
				menu.ParentBarItem.Items.Add(mdichilditem);
			}

			BarItem hideitem = null;
			if(this.bCloseEnabled == true)
			{
				if(dhc.CloseButtonVisibility == true)
				{
					if( dhc.AutoHideMode == false || this.DockBehavior == Tools.DockBehavior.VS2010)
					{
						hideitem = new BarItem(SR.GetString(SR.HideMenuItemText, dhc.HostControl.Controls[0]), new EventHandler(this.OnMenuHide_Click));
						hideitem.Tag = dhc.HostControl.Controls[0];
                        if (this.VisualStyle == Forms.VisualStyle.Metro && this.DockBehavior == Tools.DockBehavior.VS2010)
                        {
                            Bitmap bmp = new Bitmap(typeof(DockingManager).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.FrameworkComponents.DockingWindows.Images.HelpBarItemImage.dock.png"));
                            if (bmp != null)
                                hideitem.Image = bmp;
                        }
                        else
                            hideitem.Image = null;
						menu.ParentBarItem.Items.Add(hideitem);
					}
				}
			}
			bool allowFloating = dhc.AllowFloating;
			if( dhc.ParentController is DockTabController && !m_singleTabOperate )
				allowFloating = dhc.ParentController.AllowFloating;
			BarItem floatitem = null;
			if( this.bDisallowFloating == false && allowFloating )
			{
				floatitem = new BarItem(SR.GetString(SR.FloatingMenuItemText, dhc.HostControl.Controls[0]), new EventHandler(this.OnMenuFloat_Click));
				floatitem.Tag = dhc.HostControl.Controls[0];
                if (dhc.FloatOnly == true)
                {
                    if (this.VisualStyle != Forms.VisualStyle.Metro)
                        floatitem.Checked = true;
                }
                if (this.DockBehavior == Tools.DockBehavior.VS2010)
                {
                    floatitem.Text = "Float";
                    if (dhc.Floating)
                    {
                        floatitem.Enabled = false;
                        if (this.VisualStyle != Forms.VisualStyle.Metro)
                            floatitem.Checked = true;
                    }
                    else if (!dhc.Floating)
                    {
                        floatitem.Enabled = true;
                        floatitem.Checked = false;
                    }
                }

				if( MenuStyle == DockMenuStyle.VS2003 )
				{
					menu.ParentBarItem.Items.Add(floatitem);
				}
				else
				{
					menu.ParentBarItem.Items.Insert( 0, floatitem );
				}
			}

			if(dhc.Floating == false || MenuStyle == DockMenuStyle.VS2005)
			{
				BarItem autohideitem = null;
				autohideitem = new BarItem(SR.GetString(SR.AutoHideMenuItemText, dhc.HostControl.Controls[0]), new EventHandler(this.OnMenuAutoHide_Click));
				autohideitem.Tag = dhc.HostControl.Controls[0];
				autohideitem.Enabled = (dhc.AutoHideButtonVisibility && !dhc.Floating);
                if (dhc.AutoHideMode == true)
				{
					if(dockableitem != null)
					{
						dockableitem.Checked = false;
                        if(this.DockBehavior == Tools.DockBehavior.VS2008)
						    dockableitem.Enabled = false;
					}
                    if (hideitem != null && this.DockBehavior == Tools.DockBehavior.VS2008)
						hideitem.Enabled = false;
					if(floatitem != null && this.DockBehavior == Tools.DockBehavior.VS2008)
						floatitem.Enabled = false;
					if(autohideitem != null)
                    {
                        if (this.VisualStyle != Forms.VisualStyle.Metro)
                            autohideitem.Checked = true;
                        if (dhc.bInAutoHide && this.DockBehavior == Tools.DockBehavior.VS2010)
                            autohideitem.Enabled = false;
                    }
				}
				if(autohideitem != null)
				{
					if( MenuStyle == DockMenuStyle.VS2003 )
					{
						menu.ParentBarItem.Items.Add(autohideitem);
					}
					else
					{
						int insertionIndex = Math.Min(3, menu.ParentBarItem.Items.Count);
						menu.ParentBarItem.Items.Insert(insertionIndex, autohideitem);
					}
				}
			}

			if( floatitem == null || !floatitem.Checked )
			{
				BarItem docktoleftitem = new BarItem( SR.GetString(SR.MenuItemDockToLeft, dhc.HostControl.Controls[0]), new EventHandler(this.OnMenuDockToLeft_Click));
				docktoleftitem.Tag = dhc.HostControl.Controls[0];

				BarItem docktorightitem = new BarItem( SR.GetString(SR.MenuItemDockToRight, dhc.HostControl.Controls[0]), new EventHandler(this.OnMenuDockToRight_Click));
				docktorightitem.Tag = dhc.HostControl.Controls[0];

				BarItem docktotopitem = new BarItem( SR.GetString(SR.MenuItemDockToTop, dhc.HostControl.Controls[0]), new EventHandler(this.OnMenuDockToTop_Click));
				docktotopitem.Tag = dhc.HostControl.Controls[0];

				BarItem docktobottomitem = new BarItem( SR.GetString(SR.MenuItemDockToBottom, dhc.HostControl.Controls[0]), new EventHandler(this.OnMenuDockToBottom_Click));
				docktobottomitem.Tag = dhc.HostControl.Controls[0];

				ParentBarItem dockto = new ParentBarItem( SR.GetString(SR.MenuItemDockTo, dhc.HostControl.Controls[0]));
				dockto.Items.Add(docktoleftitem);
				dockto.Items.Add(docktorightitem);
				dockto.Items.Add(docktotopitem);
				dockto.Items.Add(docktobottomitem);
                if(this.DockBehavior == Tools.DockBehavior.VS2008)
				    menu.ParentBarItem.Items.Add(dockto);
			}
		}

		/// <summary>
		/// Raises the <see cref="DockingManager.DockContextMenu"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="DockContextMenuEventArgs"/> that contains the event data.</param>
		protected internal virtual void OnDockContextMenu(DockContextMenuEventArgs arg)
		{
			if(this.DockContextMenu != null)
				this.DockContextMenu(this, arg);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void OnMenuDockable_Click(Object sender, EventArgs arg)
		{
			BarItem item = sender as BarItem;

            if (this.DockBehavior == Tools.DockBehavior.VS2010)
            {
                Control ctrl = item.Tag as Control;
                DockHostController dhc = this.GetDockHostController(ctrl);
                if (dhc != null && dhc.Floating)
                {
                    dhc.FloatOnly = false;
                    if(ctrl != null && (ctrl.TopLevelControl is FloatingForm))
                        (ctrl.TopLevelControl as FloatingForm).InvokeToggleDockState(this.DockBehavior);
                }
                if(dhc != null && dhc.bInAutoHide)
                {
                    this.SetAutoHideMode(ctrl, false);
                }
            }

			else if(item.Tag is Control)
			{
				Control ctrl = item.Tag as Control;
				DockHostController dhc = this.GetDockHostController(ctrl);
				if(dhc.FloatOnly == true)
				{
					if( dhc.bInMDIMode == true )
						dhc.bInMDIMode = false;
					dhc.FloatOnly = false;
				}
				else if( (HostControl is Form) && (HostControl as Form).IsMdiContainer && MenuStyle != DockMenuStyle.VS2005 )
				{
					if(dhc.bInMDIMode == false)
						this.SetAsMDIChild(ctrl, true);
					else
						dhc.bInMDIMode = false;
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void OnMenuHide_Click(Object sender, EventArgs arg)
		{
			BarItem item = sender as BarItem;
			if(item.Tag is Control)
			{
				Control ctrl = item.Tag as Control;
				if(item.Checked == false)
					this.SetDockVisibility(ctrl, false);
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void OnMenuFloat_Click(Object sender, EventArgs arg)
		{
				BarItem item = sender as BarItem;
				if(item.Tag is Control)
				{
					Control ctrl = item.Tag as Control;
					if (item.Checked == false)
					{
						this.SetFloatOnly(ctrl, true);
						ctrl.TopLevelControl.Focus();
						ctrl.Focus();
					}
					else
					{
						if( MenuStyle == DockMenuStyle.VS2003 )
						{
							this.SetFloatOnly(ctrl, false);
						}
					}
					item.Checked = !item.Checked;
				}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void OnMenuDockToLeft_Click( object sender, EventArgs arg )
		{
			ProcessDockToClick( sender, DockingStyle.Left );
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void OnMenuDockToRight_Click( object sender, EventArgs arg )
		{
			ProcessDockToClick( sender, DockingStyle.Right );
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void OnMenuDockToTop_Click( object sender, EventArgs arg )
		{
			ProcessDockToClick( sender, DockingStyle.Top );
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void OnMenuDockToBottom_Click( object sender, EventArgs arg )
		{
			ProcessDockToClick( sender, DockingStyle.Bottom );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void OnMenuAutoHide_Click(Object sender, EventArgs arg)
		{
			BarItem item = sender as BarItem;
			if(item.Tag is Control)
			{
				Control ctrl = item.Tag as Control;
				ToggleAHState( ctrl, !item.Checked, m_singleTabOperate, true );
				item.Checked = !item.Checked;
			}
		}
		/// <summary>
		/// Toggles autohide state for specified DockHostController.
		/// </summary>
		/// <param name="dhc">DockHostController to toggle state for.</param>
		/// <param name="ahState">If true - enter AH state, else exit AH state</param>
		/// <param name="singleTab">IF parent is DockTabControl specifies if to toggle AH state for single dhc or for entire DockTabControl.</param>
		/// <param name="animate">If to animate autohiding.</param>
		protected internal void ToggleAHState( DockHostController dhc, bool ahState, bool singleTab, bool animate )
		{
			if( null != dhc )
			{
				DockTabController dtc = dhc.ParentController as DockTabController;
				DockingStyle dStyle = this.dcHostForm.GetControllerBorder(dhc);

				if( dtc != null && singleTab )
				{
					DockStateControllerWrapper stateWrapper = new DockStateControllerWrapper(this, dtc);
					stateWrapper.ParentController = dtc.ParentController;
					dtc.SetChildWrapper( stateWrapper );

					DockTabPage toRemove = null;
					foreach( DockTabPage tabPage in dtc.TabControl.TabPages )
					{
						if( tabPage.dhcClient == dhc )
							toRemove = tabPage; 
					}

					if( toRemove != null )
						dtc.TabControl.TabPages.Remove( toRemove );
				}

				if( ahState )
				{
					if( dtc != null && singleTab )
					{
						dhc.DINew.dStyle = dStyle;
						dhc.bInAutoHide = true;
						this.dcHostForm.EnterAutoHideMode( dhc, false );
						if( !animate )
							this.dcHostForm.GetAHTabControl( dStyle ).HideController( dhc, false );
					}
					else
						dhc.AutoHideMode = true;
				}
				else
					dhc.AutoHideMode = false;

				if( dtc != null && singleTab && ahState )
					dtc.UpdateTabPages();

				this.dcHostForm.AdjustLayout();
			}
		}
		/// <summary>
		/// Toggles autohide state for specified Control.
		/// </summary>
		/// <param name="dhc">Dock enabled control to toggle state for.</param>
		/// <param name="ahState">If true - enter AH state, else exit AH state</param>
		/// <param name="singleTab">IF parent is TabControl specifies if to toggle AH state for single Control or for entire TabControl.</param>
		/// <param name="animate">If to animate autohiding.</param>
		protected internal void ToggleAHState( Control ctrl, bool ahState, bool singleTab, bool animate )
		{
			if( ctrl != null )
			{
				DockHostController dhc = GetDockHostController( ctrl );
				ToggleAHState( dhc, ahState, singleTab, animate );
			}
		}

		protected void OnMenuMDIChild_Click( object sender, EventArgs arg )
		{
			BarItem item = sender as BarItem;
			if(item.Tag is Control)
			{
				Control ctrl = item.Tag as Control;
				DockHostController dhc = this.GetDockHostController(ctrl);
				if( item.Checked == false )
				{
					SetAsMDIChild( ctrl, true );
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void MDIFormDockableItem_Click(object sender, EventArgs e)
		{
			BarItem item = sender as BarItem;
			DockingWrapperForm mdiform = item.Tag as DockingWrapperForm;
			this.SetAsMDIChild(mdiform.ctrlChildRef, false);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void MDIFormHideItem_Click(object sender, EventArgs e)
		{
            BarItem item = sender as BarItem;
            DockingWrapperForm mdiform = item.Tag as DockingWrapperForm;
            Control childctrl = mdiform.ctrlChildRef;

            DockVisibilityChangingEventArgs arg = 
                new DockVisibilityChangingEventArgs(childctrl);
            if (!bFiredVisibilityChangingEvent)
            {
                FireDockVisibilityChangingEvent( arg );
                bFiredVisibilityChangingEvent = true;
            }

            if (!arg.Cancel)
            {
				DockHostController dhc = this.GetDockHostController( childctrl );
				dhc.Closing = true;
                this.SetDockVisibility(childctrl, false);
				dhc.Closing = false;
            }

            bFiredVisibilityChangingEvent = false;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void MDIFormFloatingItem_Click(object sender, EventArgs e)
		{
			BarItem item = sender as BarItem;
			DockingWrapperForm mdiform = item.Tag as DockingWrapperForm;
			Control childctrl = mdiform.ctrlChildRef;

			if( this.GetAllowFloating( childctrl ) )
			{
				bFreezeDockStateChangeEvents = true;
				this.SetAsMDIChild( childctrl, false );
				bFreezeDockStateChangeEvents = false;

				this.SetFloatOnly( childctrl, true );
				childctrl.TopLevelControl.Focus();
				childctrl.Focus();
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void ProcessDockToClick( object sender, DockingStyle dockstyle )
		{
			BarItem item = sender as BarItem;
			Control control = item.Tag as Control;
			DockHostController hostController = GetDockHostController( control );
			MainFormController mainController = alDockAreaControllers[0] as MainFormController;
			int nsize = SplitterWidth;
			// when DockHostController is in autohide mode and is hidden or hiding then restore its unhidden size
			if( hostController.bInAutoHide )
			{
				if( AHInViewTab != null )
				{
					AHInViewTab.StopAllAnimations();
				}
				hostController.LayoutRect = hostController.DINew.rcDockArea;
			}
			Rectangle rectangle = hostController.LayoutRect;
			switch( dockstyle )
			{
				case DockingStyle.Top:
				case DockingStyle.Bottom:
				{
					nsize += (mainController.LayoutRect.Height > 2 * rectangle.Height) ?
						rectangle.Height : mainController.LayoutRect.Height / 2;
                    if (this.VisualStyle == VisualStyle.VS2005 || this.VisualStyle == VisualStyle.Office2003 || this.VisualStyle == VisualStyle.Default)
                        nsize -= Renderer.CaptionWidth + (Renderer.BorderWidth * 6);
                    else if (this.VisualStyle == VisualStyle.Office2007 || this.VisualStyle == VisualStyle.Office2007Outlook)
                        nsize -= Renderer.CaptionWidth + (Renderer.BorderWidth * 3) + 1;
					break;
				}
				case DockingStyle.Left:
				case DockingStyle.Right:
				{
					nsize += (mainController.LayoutRect.Width > 2 * rectangle.Width) ?
						rectangle.Width : mainController.LayoutRect.Width / 2;
                    if(this.VisualStyle == VisualStyle.VS2005 || this.VisualStyle == VisualStyle.Office2003 || this.VisualStyle == VisualStyle.Default)
                        nsize -= SplitterWidth + (Renderer.BorderWidth * 2);
                    else if(this.VisualStyle == VisualStyle.Office2007 || this.VisualStyle == VisualStyle.Office2007Outlook)
                        nsize -= SplitterWidth + (Renderer.BorderWidth * 2);
					break;
				}
			}
			
			//DockAllow event should call before the DockMenuClickEvent.
			//So that we can cancel the event if we dont need to dock to the particular side.
            DockAllowEventArgs args = new DockAllowEventArgs(control, this.HostControl, dockstyle);
			this.FireDockAllowEvent(args);
			if(!args.Cancel)
			{
				DockControl( control, HostControl, dockstyle, nsize, (hostController.ParentController is DockTabController) && !m_singleTabOperate);
   
				FireDockMenuClickEvent( control, dockstyle );
			}

		}

        

		/// <summary>
		/// Overridden. See <see cref="System.ComponentModel.Component.Dispose"/>.
		/// </summary>
		protected override void Dispose(bool bdisposing)
		{
			if( m_nativeWindow != null )
			{
				m_nativeWindow.DestroyHandle();
				m_nativeWindow = null;
			}

            if (CaptionButtons != null)
            {
                CaptionButtons.CollectionChanged -= new EventHandler(CaptionButtons_CollectionChanged);
                CaptionButtons.CollectionItemChanged -= new EventHandler(CaptionButtons_CollectionItemChanged);
                CaptionButtons.Dispose();
            }

			if (DesignerHooks.GetMsgProcListContains(this, m_curentThreadId))
				DesignerHooks.RemoveGetMsgProcListener(this, m_curentThreadId);

			if((bdisposing == true) && (this.dcHostForm != null))
			{
				this.RecSubscribeHostControlEvents( this.HostControl.Parent, false );
				this.m_dragProvider.Dispose();
				this.m_dragProvider = null;
				this.controllerInFocus = null;
				// Re-unsubscribe just to be safe for sometimes the form could be created but not displayed.
				this.HostControl.Paint -= new PaintEventHandler( this.HostControl_Paint );
				this.HostControl.Layout -= new System.Windows.Forms.LayoutEventHandler(this.HostControl_Layout);
				this.HostControl.HandleCreated -= new EventHandler(this.HostControl_HandleCreated);
				this.HostControl.SystemColorsChanged -= new EventHandler(this.HostControl_SystemColorsChanged);
				this.HostControl.RightToLeftChanged -= new EventHandler(HostControl_RightToLeftChanged);
				if(this.DesignMode == false)
				{
					if( subclass != null )
					{
						subclass.OnMdiActivate -= new MainWindowSubclass.MessageEventHandler(this.ProcessMDIActivate);
						subclass.OnDeactivate -= new MainWindowSubclass.MessageEventHandler(subclass_OnDeactivate);
                        subclass.OnClose -= new MainWindowSubclass.MessageEventHandler(HostForm_OnClose);
						subclass.ReleaseHandle();
                        subclass = null;
					}
					if(this.DesignProcess == false)
					{
						this.HostControl.Paint -= new PaintEventHandler(this.HostControl_Paint);
						if( this.frmOwner != null )
						{
							this.frmOwner.HandleCreated -= new EventHandler( this.OwnerForm_HandleCreated );
							this.frmOwner = null;
						}

						if(this.HostControl is Form)
						{
							(this.HostControl as Form).Closed -= new EventHandler( this.HostForm_Closed );
							this.HostControl.ParentChanged -= new EventHandler(this.HostControl_ParentChanged);
						}
						else
							this.RecSubscribeHostControlEvents(this.HostControl, false);
					}
				}

				Array ffcarray = this.alFFControllers.ToArray(typeof(FloatingFormController));
				foreach(FloatingFormController ffc in ffcarray)
					ffc.Dispose();
				foreach( FloatingFormController ffc in alHiddenFFControllers )
					ffc.Dispose();
				this.dcHostForm.Dispose();	// Disposes all the docked controllers
                this.dcHostForm = null;
				this.AHInViewTab = null;
				this.controllers.Clear();
				this.controllerInFocus = null;
				// Clear all collections
				this.alFFControllers.Clear();
				this.alHiddenFFControllers.Clear();
				this.alDockAreaControllers.Clear();
				this.alEnableDocking.Clear();
				this.htText.Clear();
				this.htIcon.Clear();
				this.listAHOnLoad.Clear();
				this.htFloatOnly.Clear();
				this.htHiddenOnLoad.Clear();
				this.htDockAbility.Clear();
				this.htOuterDockAbility.Clear();
				this.alInheritedControls.Clear();
				this.alTargetManagers.Clear();
				this.m_freezeResizeCtrls.Clear();
                this.htCustomCaptionButtons.Clear();
                this.htAllowFloating.Clear();
                this.htmIcon.Clear();
                this.m_CaptionButtons.Clear();
                this.m_mdiZOrder.Clear();
                this.m_CustomizeControlCollection.Clear();
                this.alTargetManagers.Clear();
                this.controllers.Clear();
                
                    
				if((this.bControlScopeImages == true) && (this.ilDockTabs != null))
				{
					this.ilDockTabs.Images.Clear();
					this.ilDockTabs.Dispose();
					this.ilDockTabs = null;
				}

				if(this.stmLayout != null)
					this.stmLayout.Close();

                
                this.htText = null;
                this.htIcon = null;
                this.listAHOnLoad = null;
                this.htFloatOnly = null;
                this.htHiddenOnLoad = null;
                this.htDockAbility = null;
                this.htOuterDockAbility = null;
                this.m_freezeResizeCtrls = null;
                this.htCustomCaptionButtons = null;
                this.htAllowFloating = null;
                this.htmIcon = null;
                this.m_CaptionButtons = null;
                this.m_mdiZOrder = null;
                this.m_CustomizeControlCollection = null;
                this.controllers = null;

				this.ctrlLastActive = null;
				this.ctrlLastPainted = null;
				this.dhLastActive = null;
				this.lastActivated = null;

                if (this.m_container != null)
                {
                    this.m_container.Remove(this);
                    this.m_container = null;
                }

                if (DockingManagersList != null)
                {
                    DockingManagersList.Clear();
                    DockingManagersList = null;
                }

				this.mouseActivatedControl = null;
				this.dcHostForm = null;
			}
			
			base.Dispose(bdisposing);
		}
	
		internal delegate void EnabledDockingEventHandler( Control ctrl, bool enabledDocking );
		internal event EnabledDockingEventHandler OnEnabledDocking;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void RaiseEnabledDocking( Control ctrl, bool enabledDocking )
		{
			if( OnEnabledDocking != null )
				OnEnabledDocking(ctrl, enabledDocking);
		}

        /// <summary>
        /// Enable user to detect once double click occurs on docked windows caption
        /// </summary>
        public event DockMouseSelectionEventHandler OnCaptionDoubleClick;

        /// <summary>
        /// To raise OnCaptionDoubleClick event
        /// </summary>
        /// <param name="ctrl">Active docked child</param>
        internal void RaiseCaptionDoubleClick(Control ctrl)
        {
            if (OnCaptionDoubleClick != null)
            {
                this.OnCaptionDoubleClick(this, new DockControlMouseSelection(ctrl));
            }
        }

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void RemoveDockHostImageFromList(DockHostController dhcontroller)
		{
			if((this.bControlScopeImages == true) && (this.ilDockTabs != null) &&
				(dhcontroller.ImageIndex >= 0) && (dhcontroller.ImageIndex < this.ilDockTabs.Images.Count))
			{
				this.ilDockTabs.Images.RemoveAt(dhcontroller.ImageIndex);
				// Iterate the list of DockHostController and reassign the index for all controllers that
				// have an ImageIndex greater than dhc.ImageIndex;
				foreach(DockControllerBase dcbase in this.alDockAreaControllers)
				{
					if(dcbase is DockHostController)
					{
						DockHostController dhc = dcbase as DockHostController;
						if(dhc.ImageIndex > dhcontroller.ImageIndex)
							dhc.ImageIndex--;
					}
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual DockHostController InternalEnableDocking(Control ctrl, Syncfusion.Windows.Forms.Tools.DockingStyle border, int npriority)
		{
			// Create a dockhost wrapper and assign the controller to it
			DockHost dhost = this.CreateDockHost(ctrl);
			Debug.Assert(dhost != null);

			// Initially, dock the control to the main form with the Left Syncfusion.Windows.Forms.Tools.DockingStyle and Horizontal DockPreference.
			DockHostController dhc = dhost.InternalController as DockHostController;
			dhc.ControlLocation = ctrl.Location;
			dhc.ControlSize = ctrl.Size;
			int nwidth = (ctrl.Size.Width >= this.dcHostForm.HostControl.Width/2) ? this.dcHostForm.HostControl.Width/2 : ctrl.Size.Width;
			DockPreference dp = ((border == Syncfusion.Windows.Forms.Tools.DockingStyle.Left)||(border == Syncfusion.Windows.Forms.Tools.DockingStyle.Right)) ? DockPreference.Horizontal : DockPreference.Vertical;
			dhc.DINew = new DockInfo(this.dcHostForm, border, npriority, 0, dp, new Rectangle(0,0,nwidth,ctrl.Size.Height));
			dhc.DICurrent = new DockInfo(dhc.DINew);
			dhc.DINew.dController.InvokeDocking(dhc);

			return dhc;
		}

		private void UpdateRightToLeftProperty( RightToLeft rtlNewRightToLeft )
		{
			bool bIsMirrored = false;

			if (RightToLeft.Inherit == rtlNewRightToLeft && null != this.HostControl)
			{
				RightToLeft rtlHost = this.HostControl.RightToLeft;
				bIsMirrored = (rtlHost == RightToLeft.Yes);
			}
			else
			{
				bIsMirrored = (rtlNewRightToLeft == RightToLeft.Yes);
			}

			if (m_bIsMirrored != bIsMirrored)
			{
				m_bIsMirrored = bIsMirrored;

			if( VisualStyle != VisualStyle.Default )
			{
				Renderer.IsMirrored = this.IsMirrored;
			}

				foreach(DockControllerBase dcbase in this.alDockAreaControllers)
				{
					if (dcbase is DockHostController)
					{
						DockHost dhost = dcbase.HostControl as DockHost;

						// After RightToLeft change DockHost stops receiving WM_MOUSELEAVE and WM_MOUSEHOVER messages.
						// Call TrackMouseEvent function in order that DockHost receives these messages again.
						NativeMethods.TRACKMOUSEEVENT tme = new NativeMethods.TRACKMOUSEEVENT();
						tme.dwFlags = NativeMethods.TME_HOVER | NativeMethods.TME_LEAVE;
						tme.dwHoverTime = 0;	// hover time-out in milliseconds
						tme.hwndTrack = dhost.Handle;
						tme.cbSize = Marshal.SizeOf(tme);
						NativeMethods.TrackMouseEvent( ref tme );

						if( dhost.Controls.Count > 0 )
						{
							SetIsMirrored( dhost.Controls[0], bIsMirrored );
							dhost.Controls[0].RightToLeft = ( bIsMirrored ) ?
								RightToLeft.Yes :
								RightToLeft.No;
							if( dcbase.ParentController is DockTabController )
							{
								dcbase.ParentController.HostControl.RightToLeft = bIsMirrored ? System.Windows.Forms.RightToLeft.Yes : System.Windows.Forms.RightToLeft.No;
							}
						}
					}
				}
				foreach(FloatingFormController ffc in this.alFFControllers)
				{
					// Dont set RightToLeft for form due to incorrect alignment of caption text
					//					ff.RightToLeft = bIsMirrored ? RightToLeft.Yes : RightToLeft.No;
					RightToLeft rtlEffective = bIsMirrored ? RightToLeft.Yes : RightToLeft.No;

					FloatingForm ffForm = ffc.HostControl as FloatingForm;
					if (null != ffForm)
					{
						ffForm.UpdateChildsRightToLeft( rtlEffective );
					}

					ffForm.UpdateControlBoxVisibility();
				}

				DockingStyle[] adsDtyles =
					{ DockingStyle.Left, DockingStyle.Right, DockingStyle.Top, DockingStyle.Bottom };

				foreach( DockingStyle ds in adsDtyles )
				{
					AHTabControl ahtcTabCtrl =  dcHostForm.GetAHTabControl( ds );
					if (null != ahtcTabCtrl)
					{
						RightToLeft rtlNewVal = bIsMirrored ?
							System.Windows.Forms.RightToLeft.Yes :
							System.Windows.Forms.RightToLeft.No;
						if (rtlNewVal != ahtcTabCtrl.RightToLeft)
						{
							ahtcTabCtrl.RightToLeft = rtlNewVal;
						}
					}
				}
			}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void UpdateFloatingFormsImages()
		{
			for( int i = 0; i < alDockAreaControllers.Count; i++ )
			{
				DockHostController dhc = alDockAreaControllers[i] as DockHostController;
				if( dhc != null && dhc.HostControl != null )
				{
					if( dhc.Floating )
					{
						DockTabController dtc = dhc.ParentController as DockTabController;
						FloatingFormController ffc = null;
						if( dtc != null )
						{
							ffc = dtc.ParentController as FloatingFormController;
							if( ffc != null && dtc.TabControl != null && dtc.TabControl.SelectedIndex>=0 )
							{
								DockTabPage page = dtc.TabControl.TabPages[dtc.TabControl.SelectedIndex] as DockTabPage;
								if( page.dhcClient!=null&& ffc.ImageIndex != page.dhcClient.ImageIndex )
                                {
                                    ffc.ImageIndex = page.dhcClient.ImageIndex;
                                    NativeMethods.RedrawWindow( ffc.HostControl.Handle, IntPtr.Zero, IntPtr.Zero, 
										NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE );
								}
							}
						}
						else
						{
							ffc = dhc.ParentController as FloatingFormController;
							if( ffc != null )
							{
								if( ffc.ImageIndex != dhc.ImageIndex )
								{
                                    ffc.ImageIndex = dhc.ImageIndex;
									NativeMethods.RedrawWindow(ffc.HostControl.Handle, IntPtr.Zero, IntPtr.Zero,
										NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE);
								}
							}
							else
							{
								ffc = dhc.ToplevelController as FloatingFormController;

								if( ffc != null )
								{
									SizingController sc = ffc.dcChild as SizingController;

									if( sc != null && sc.GetDockControllers().Count == 1 )
									{
                                        ffc.ImageIndex = dhc.ImageIndex;
                                        NativeMethods.RedrawWindow(ffc.HostControl.Handle, IntPtr.Zero, IntPtr.Zero,
											NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE);
									}
								}
							}
						}
					}
				}
				else
				{
					FloatingFormController ffcntlr = alDockAreaControllers[i] as FloatingFormController;
					if( ffcntlr != null )
					{
						SizingController sc = ffcntlr.dcChild as SizingController;
						if( sc != null && sc.GetDockControllers().Count != 1 && ffcntlr.ImageIndex >= 0 )
						{
							ffcntlr.ImageIndex = -1;
							NativeMethods.RedrawWindow( ffcntlr.HostControl.Handle, IntPtr.Zero, IntPtr.Zero, 
								NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE );
						}
					}
				}
			}
		}

		
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void RestoreFloatingForms()
		{
			foreach ( FloatingFormController ffc in this.alFFControllers )
			{
				if (ffc.Floating)
				{
					FloatingForm ffForm = ffc.HostControl as FloatingForm;
					if (null != ffForm)
					{
						ffForm.RestoreStateOnRightToLeftUpdate();
					}
				}
			}
        }

		protected internal bool HasCaptionButtonWithName(string name)
		{
			if (this.CaptionButtons.ContainsName(name))
			{
				return true;
			}
			else
			{
				foreach (Control ctrl in alEnableDocking)
				{
					CaptionButtonsCollection collection = htCustomCaptionButtons[ctrl] as CaptionButtonsCollection;
					if (collection != null && collection.ContainsName(name))
					{
						return true;
					}
				}
			}
			return false;
		}

		protected internal string GetCaptionButtonDefaultName()
		{
			int num = 1;
			while(HasCaptionButtonWithName( "CaptionButton" + num.ToString() ))
			{
				num++;
			}
			return "CaptionButton" + num.ToString();
		}

# region Implementation of IDockingManagerDesignerInvoke
        // Private implementation of the IDockingManagerDesignerInvoke interface
		MainFormController IDockingManagerDesignerInvoke.GetHostFormController()
		{
			return this.dcHostForm;
		}

		ArrayList IDockingManagerDesignerInvoke.GetEnableDockingList()
		{
			return this.alEnableDocking;
		}

		ArrayList IDockingManagerDesignerInvoke.GetFFControllerList()
		{
			return this.alFFControllers;
		}

		ArrayList IDockingManagerDesignerInvoke.GetControllerList()
		{
			return this.alDockAreaControllers;
		}

		ArrayList IDockingManagerDesignerInvoke.GetFreezeResizeControllers()
		{
			return this.m_freezeResizeCtrls;
		}

		Hashtable IDockingManagerDesignerInvoke.GetTextTable()
		{
			return this.htText;
		}

        Hashtable IDockingManagerDesignerInvoke.GetIconTable()
        {
            return this.htIcon;
        }
        
        Hashtable IDockingManagerDesignerInvoke.GetmIconTable()
        {
            return this.htmIcon;
        }       

		Hashtable IDockingManagerDesignerInvoke.GetDockAbilityTable()
		{
			return this.htDockAbility;
		}

		Hashtable IDockingManagerDesignerInvoke.GetOuterDockAbilityTable()
		{
			return this.htOuterDockAbility;
		}

		ArrayList IDockingManagerDesignerInvoke.GetInheritedControlsList()
		{
			return this.alInheritedControls;
		}

		DockHostController IDockingManagerDesignerInvoke.GetDHCInFocus()
		{
			return this.controllerInFocus;
		}

		void IDockingManagerDesignerInvoke.SetDHCInFocus(DockHostController dhc)
		{
			this.DHCInFocus = dhc;
		}

		bool IDockingManagerDesignerInvoke.LoadFromStream(Stream file)
		{
			bool brestoreminmax = this.bApplyMinMaxExtents;
			this.bApplyMinMaxExtents = false;

			bool bretval = this.LoadFromStream(file);

			this.bApplyMinMaxExtents = brestoreminmax;

			return bretval;
		}

		void IDockingManagerDesignerInvoke.ApplyDHCFloatOnlySettings()
		{
			this.ApplyDHCFloatOnlySettings();
		}

		Object IDockingManagerDesignerInvoke.GetPrimarySelection()
		{
			Debug.Assert(base.DesignMode == true);
			ISelectionService iss = this.GetService(typeof(ISelectionService)) as ISelectionService;
			return iss.PrimarySelection;
		}

		void IDockingManagerDesignerInvoke.SetSelectedComponents(ICollection clln, SelectionTypes seltype)
		{
			Debug.Assert(base.DesignMode == true);
			ISelectionService iss = this.GetService(typeof(ISelectionService)) as ISelectionService;
			iss.SetSelectedComponents(clln, seltype);
        }

        #endregion
        internal ControllerWrapper GetWrapper()
		{
			MainFormController mfc = (MainFormController) alDockAreaControllers[0];
			return mfc.GetWrapper();
		}

		internal DockControllerBase LocateController(ControllerWrapper cw)
		{
			foreach(DockControllerBase dcb in controllers)
			{
				if( dcb.IsEqual(cw) )
				{
					controllers.Remove(dcb);
					return dcb;
				}
			}

			if( cw is SizingControllerWrapper )
			{
				SizingController sc = new SizingController(this, this.HostControl,
					(cw as SizingControllerWrapper).Orientation);
				return sc;
			}
			return null;
		}

		private void InitializeDragProvider(DragProviderStyle dragProviderStyle)
		{
			providerStyle = dragProviderStyle;

            if( m_dragProvider != null )
            {
                m_dragProvider.Dispose();
                m_dragProvider = null;
            }

			if( bDesignProcess )
				m_dragProvider = new BorderDragProvider( this );
			else
			{
				switch( providerStyle )
				{ 
					case DragProviderStyle.Standard:
						m_dragProvider = new BorderDragProvider( this );
						break;

					case DragProviderStyle.Whidbey:
						m_dragProvider = new WhidbeyDragProvider( this );
						break;
					case DragProviderStyle.VS2012:
						m_dragProvider = new VS2012DragProvider(this);
						break;
                    case DragProviderStyle.VS2010:
                        m_dragProvider = new VS2010DragProvider(this);
                        break;
					case DragProviderStyle.VS2005:
						m_dragProvider = new VS2005DragProvider( this );
						break;

					case DragProviderStyle.VS2008:
						m_dragProvider = new VS2008DragProvider( this );
						break;

					default:
						m_dragProvider = new BorderDragProvider( this );
						break;
				}
			}
		}

		internal bool DesignerInitializing
		{
			get
			{
				if( DesignMode )
				{
					IDesignerHost designerHost = GetService( typeof(IDesignerHost) ) as IDesignerHost;
					DockingManagerDesigner designer = designerHost.GetDesigner( this ) as DockingManagerDesigner;
					return designerHost.Loading || designer.bReloading;
				}

				return false;
			}
		}

		internal Size ToDockHostSize( int width, int height )
		{
			return new Size( width + 2 * Renderer.BorderWidth,
				height + 2 * Renderer.BorderWidth + Renderer.CaptionWidth );
		}
		
		internal Size ToDockHostSize( Size size )
		{
			return ToDockHostSize( size.Width, size.Height );
		}

		internal Rectangle ToDockHostRectangle( int x, int y, int width, int height )
		{
			return new Rectangle( new Point( x ,y ), ToDockHostSize( width, height ) );
		}
		
		internal Rectangle ToDockHostRectangle( Rectangle rectangle )
		{
			return new Rectangle( rectangle.Location, ToDockHostSize( rectangle.Size ) );
		}

		internal Control lastActivated = null;
		internal Control mouseActivatedControl = null;
		
		internal Control GetMdiClient()
		{
			Control mdiClient = null;

			if (this.HostControl != null && this.HostControl is Form && (this.HostControl as Form).IsMdiContainer)
				foreach (Control c in this.HostControl.Controls)
				{
					if (c is MdiClient)
					{
						mdiClient = c;
						break;
					}
				}

			return mdiClient;
		}

		private CaptionButtonsCollection m_prevCaptionButtons = new CaptionButtonsCollection();
        private CaptionButtonsCollection cbRemovedButtons;
        private CaptionButtonsCollection collection;
        private CaptionButtonsCollection newCollection;
		private void SynchronizeCaptionButtons()
		{
			cbRemovedButtons = new CaptionButtonsCollection();
			if (m_prevCaptionButtons.Count > m_CaptionButtons.Count)
			{
				cbRemovedButtons = m_prevCaptionButtons.Clone();
				cbRemovedButtons.ExcludeCommonButtonsWith(m_CaptionButtons);
			}
			for (int i = 0; i < alEnableDocking.Count; i++)
			{
				Control ctrl = alEnableDocking[i] as Control;
				if (ctrl != null)
				{
					collection = GetCustomCaptionButtons(ctrl);
					if (collection != null)
					{
						newCollection = new CaptionButtonsCollection();
						newCollection.MergeWith(CaptionButtons, false);
						for (int j = 0; j < cbRemovedButtons.Count; j++)
						{
							if (collection.ContainsName(cbRemovedButtons[j].Name))
								collection.Remove(collection[cbRemovedButtons[j].Name]);
						}
						newCollection.MergeWith(collection, false);
						collection.Clear();
						collection.MergeWith(newCollection, false);
					}
				}
			}
			m_prevCaptionButtons = m_CaptionButtons.Clone();
		}

		private void CaptionButtons_CollectionChanged(object sender, EventArgs e)
		{
			SynchronizeCaptionButtons();
			if(DesignMode)
				UpdateDesigner();
			UpdateControllers();
		}

		private void CaptionButtons_CollectionItemChanged(object sender, EventArgs e)
		{
			if(DesignMode)
				UpdateDesigner();
			UpdateControllers();
		}

		// Indicates if the the Default/VS2005 style is set with Vista Aero Theme is enabled.
		internal bool IsDefaultRendering()
		{
			if( ( NativeMethods.IsCompositionEnabled() )
				&& ( this.VisualStyle == VisualStyle.VS2005 || this.VisualStyle == VisualStyle.Default ) )
				return true;

			return false;
		}
	}
	
	internal class NotifyNativeWindow: NativeWindow
	{
		private DockingManager m_dockingManager = null;
		private Control m_hostControl = null;

		public NotifyNativeWindow( DockingManager dockingManager, Control hostControl )
		{
			m_dockingManager = dockingManager;
			m_hostControl = hostControl;
		}

		private void ProcessLButtonDown( Message m )
		{
			if( (int) m.WParam == NativeMethods.WM_LBUTTONDOWN )
			{
				Control control = null;
				Point point = new Point( NativeMethods.LOWORD(m.LParam), NativeMethods.HIWORD(m.LParam) );
				DockHost dockHost = m_hostControl.GetChildAtPoint( point ) as DockHost;
				if( dockHost != null )
				{
					DockHostController hostController = dockHost.InternalController as DockHostController;
					DockTabController tabController =
						hostController.ParentController as DockTabController;
					if( tabController != null )
					{
						Point screenPoint = m_hostControl.PointToScreen( point );
						Point clientPoint = tabController.TabControl.PointToClient( screenPoint );
						int tabPressed =  tabController.TabControl.HitTestTabs( clientPoint );
						if( tabPressed != -1 )
						{
							DockTabPage tabPage = tabController.TabControl.TabPages[ tabPressed ] as DockTabPage;
							control = tabPage.dhcClient.HostControl.Controls[0];
						}
					}
					else
					{
						control = dockHost.Controls[0];
					}
				}
				else
				{
					control = null;
				}

				m_dockingManager.mouseActivatedControl = control;
			}
		}

		private void TryFloatingFormPaint( Message m )
		{
			FloatingForm ffrm = Control.FromHandle( m.HWnd ) as FloatingForm;
			if( ffrm != null )
			{
				if( m_dockingManager.VisualStyle == VisualStyle.Default
					|| m_dockingManager.VisualStyle == VisualStyle.VS2005 )
				{
                    if( m.Msg == NativeMethods.WM_NCPAINT )
                        base.WndProc(ref m);
                    if( !ffrm.IsDefaultRendering() )
                        if( ffrm.Text == " " )
                            ffrm.DrawWindowTextAndImage(ref m);
				}
				else
				{
                    if( NativeMethods.IsDwmNCRenderingEnabled(ffrm.Handle) )
                        base.WndProc(ref m);
                    else
                        ffrm.PaintNCArea();
				}
			}
		}

		private void TryFloatingFormCalcSize( Message m )
		{
			if( m_dockingManager.VisualStyle != VisualStyle.Default
				&& m_dockingManager.VisualStyle != VisualStyle.VS2005 )
			{
				FloatingForm ffrm = Control.FromHandle( m.HWnd ) as FloatingForm;
				if( ffrm != null )
				{
					ffrm.ProcessNCCalcSize( m );
				}
			}
		}

		protected override void WndProc(ref Message m)
		{
            if( m.Msg == NativeMethods.WM_PARENTNOTIFY )
			{
				ProcessLButtonDown( m );
			}

			if( m_dockingManager.DesignMode && m.Msg == NativeMethods.WM_NCPAINT )
			{
				TryFloatingFormPaint( m );
				return;
			}

			base.WndProc(ref m);

			if( m_dockingManager.DesignMode && m.Msg == NativeMethods.WM_NCCALCSIZE )
			{
				TryFloatingFormCalcSize( m );
			}

			if( m_dockingManager.DesignMode && m.Msg == NativeMethods.WM_NCACTIVATE )
			{
				TryFloatingFormPaint( m );
			}
		}
	}

	internal class FrameBorderPainterFactory
	{
		public static IFrameBorderPainter GetPainter( FramePainter framePainter )
		{
			IFrameBorderPainter painter = framePainter.Painter;
			PainterType painterType = framePainter.PainterType;
			switch( painterType )
			{
				case PainterType.Halftone:
				{
					if( !(painter is HalftoneBorderPainter) )
					{
						painter = new HalftoneBorderPainter();
					}
					break;
				}

				case PainterType.Colored:
				{
					ColorBorderPainter coloredPainter = painter as ColorBorderPainter;
					if( coloredPainter != null )
					{
						if( framePainter.Brush != null )
						{
							coloredPainter.Brush = framePainter.Brush;
						}
						else
						{
							coloredPainter.BrushColor = framePainter.Color;
						}
						
						if( coloredPainter.Opacity != framePainter.Opacity )
						{
							coloredPainter.Opacity = framePainter.Opacity;
						}
					}
					else
					{
						if( framePainter.Brush != null )
						{
							painter = new ColorBorderPainter( framePainter.Brush );
						}
						else
						{
							painter = new ColorBorderPainter( framePainter.Color );
						}
					}
					break;
				}

				case PainterType.Custom:
				{
					if( framePainter.Painter != null )
						painter = framePainter.Painter;
					else
						painter = new HalftoneBorderPainter();
					break;
				}
			}
			return painter;
		}
	}
	
	[Syncfusion.Documentation.DocumentationExclude()]
	public class FramePainter
	{
		private IFrameBorderPainter m_Painter = null;
		private Brush m_Brush = null;
		private PainterType m_PainterType = PainterType.Halftone;
		private Color m_Color = Color.Silver;
		private float m_Opacity = 1.0f;

		public FramePainter()
		{
			m_Painter = FrameBorderPainterFactory.GetPainter( this );
		}
		
		public IFrameBorderPainter Painter
		{
			get { return m_Painter; }
			set 
			{ 
				m_Painter = value; 
				FrameBorderPainterFactory.GetPainter( this );
			}
		}

		public Brush Brush
		{
			get { return m_Brush; }
			set 
			{ 
				m_Brush = value; 
				FrameBorderPainterFactory.GetPainter( this );
			}
		}

		public PainterType PainterType
		{
			get { return m_PainterType; }
			set 
			{ 
				m_PainterType = value; 
				m_Painter = FrameBorderPainterFactory.GetPainter( this );
			}
		}

		public Color Color
		{
			get { return m_Color; }
			set 
			{ 
				m_Color = value; 
				FrameBorderPainterFactory.GetPainter( this );
			}
		}
		

		public float Opacity
		{
			get { return m_Opacity; }
			set 
			{ 
				m_Opacity = value; 
				FrameBorderPainterFactory.GetPainter( this );
			}
		}
	}
	
	[Serializable]
	internal class SplitterWidthException: ApplicationException
	{
		private const string ErrorMessage = "Incorrect value for SplitterWidth. " + 
			"Please, specify value between 0 and 30.";

		public SplitterWidthException()
			: this( ErrorMessage )
		{
		}

		public SplitterWidthException( string message )
			: base(message)
		{
		}

		public SplitterWidthException( Exception innerExc )
			: this( ErrorMessage, innerExc )
		{
		}
		
		public SplitterWidthException( string message, Exception innerException )
			: base(message, innerException)
		{
		}
	}

	interface IDockable
	{
		DockControllerBase GetController();
	}
}
