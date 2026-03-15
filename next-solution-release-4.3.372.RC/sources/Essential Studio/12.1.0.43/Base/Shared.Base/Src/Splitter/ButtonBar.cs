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
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Diagnostics;
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms
{
    /// <summary>
    ///   ButtonBar is a base class for displaying several buttons in one bar. It is used by <see cref="RecordNavigationBar"/>
    ///   and <see cref="TabBar"/>.
    /// </summary>
    [
	ToolboxItem(false),
	]
    public class ButtonBar : Control,
        IControlToolTipProvider,
		ICancelModeProvider,
		ISupportUpdating
    {
		// Fields
		private InternalButtonBar bar = null;
        int updateCount = 0;
        bool updatePending = false;
	    internal ControlToolTip toolTipProvider = null;
        bool showToolTips = false;

		// Events
		/// <summary>
		/// Occurs when a WM_CANCELMODE is received.
		/// </summary>
        [Description("Occurs when a WM_CANCELMODE is received.")]
		public event EventHandler CancelMode;

		/// <summary>
		/// Occurs when <see cref="ButtonLook"/> is changed.
		/// </summary>
		[Description("Occurs when ButtonLook is changed.")]
		public event EventHandler ButtonLookChanged;

		/// <summary>
		/// Occurs before the <see cref="Control.MouseDown"/> event is raised.
		/// </summary>
		[Description("Occurs before the Control.MouseDown event is raised.")]
		public event MouseEventHandler ButtonBarMouseDown;

        /// <summary>
        /// Raises the <see cref="ButtonBarMouseDown"/> event.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs" /> that contains the event data.</param>
		protected virtual void OnButtonBarMouseDown(MouseEventArgs e)
		{
#if DEBUG
			if (Switches.ButtonBarEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(this.Name, e.X, e.Y, e.Button, e.Clicks);
#else
			;
#endif

			if (ButtonBarMouseDown != null)
				ButtonBarMouseDown(this, e);
		}

		internal Rectangle ReverseRectangleRTL(Rectangle r)
		{
			if (this.RightToLeft == RightToLeft.Yes)
				return new Rectangle(this.ButtonBarChild.Bounds.Right - r.Right, r.Top, r.Width, r.Height);
			return r;
		}

		/// <summary>
		/// Initializes a new button bar.
		/// </summary>
        public ButtonBar()
        {
			SetStyle(ControlStyles.ResizeRedraw, false);
			SetStyle(ControlStyles.AllPaintingInWmPaint|ControlStyles.UserPaint|WhidbeyCompatibleControlStyles.DoubleBuffer, true);
		}

		/// <override/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				SuspendLayout();
				if (bar != null)
				{
					bar.Dispose();
					bar = null;
				}
				if (toolTipProvider != null)
				{
					toolTipProvider.Dispose();
					toolTipProvider = null;
				}
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Suspends updating the bar.
		/// </summary>
	    public void BeginUpdate()
	    {
            updateCount++;
	    }

		/// <overload>
		/// Resumes updating the bar.
		/// </overload>
		/// <summary>
		/// Resumes updating the bar and refreshes it.
		/// </summary>
		public void EndUpdate()
		{
			EndUpdate(true);
		}

		/// <summary>
		/// Resumes updating the bar and optionally refreshes it.
		/// </summary>
		/// <param name="refresh">Indicates whether button bar should be refreshed; if False button will only be refreshed when it is marked dirty.</param>
		public void EndUpdate(bool refresh)
	    {
            if (updateCount > 0)
            {
                updateCount--;
                if (updateCount == 0 && (refresh || updatePending))
                {
                    Refresh();
                    updatePending = false;
                }
            }
	    }

		/// <summary>
		/// Indicates whether <see cref="BeginUpdate"/> was called.
		/// </summary>
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false)
		]
        public bool Updating
        {
            get
            {
                return updateCount > 0;
            }
        }

		bool ISupportUpdating.Updating
		{
			get
			{
				return Updating;
			}
		}

		/// <override/>
		protected override void OnMouseDown(MouseEventArgs mevent)
	    {
			OnButtonBarMouseDown(mevent);
#if DEBUG
			if (Switches.SplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name, mevent.X, mevent.Y, mevent.Button, mevent.Clicks);
#endif

			base.OnMouseDown(mevent);
            if (toolTipProvider != null)
                toolTipProvider.DeactivateToolTip();
	    }

		/// <override/>
		protected override void OnMouseUp(MouseEventArgs mevent)
	    {
#if DEBUG
			if (Switches.SplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name, mevent.X, mevent.Y, mevent.Button, mevent.Clicks);
#endif

			base.OnMouseUp(mevent);
            if (toolTipProvider != null && this.IsHandleCreated)
                toolTipProvider.ActivateToolTip();
	    }

		internal bool shouldPaintButtonBar = true;

		/// <override/>
		protected override void OnPaint(PaintEventArgs pe)
		{
			base.OnPaint(pe);

			if (shouldPaintButtonBar)
			{
				lock(this)
				{
					InternalButtonBar bar = ButtonBarChild;

					Region clip = pe.Graphics.Clip;
					bar.Paint(pe.Graphics);
					pe.Graphics.ExcludeClip(bar.Bounds);

					Brush br = new SolidBrush(BackColor);
					pe.Graphics.FillRectangle(br, ClientRectangle);
					br.Dispose();

					pe.Graphics.Clip = clip;
				}
			}
        }

		/// <override/>
		protected override void OnFontChanged(EventArgs e)
        {
#if DEBUG
			if (Switches.SplitterControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Font);
#endif

			base.OnFontChanged(e);
			PerformLayout();
        }

		/// <override/>
		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
		protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
				case NativeMethods.WM_PAINT:
					if (Updating && m.HWnd == this.Handle)
					{
						updatePending = true;
						return;
					}
					break;
				case 31/*WM_CANCELMODE*/:
					OnCancelMode(EventArgs.Empty);
					break;
			}
            base.WndProc(ref m);
        }

		internal void NotifyCancelMode()
		{
			OnCancelMode(EventArgs.Empty);
		}

		/// <summary>
		/// Raises the <see cref="CancelMode"/> event.
		/// </summary>
		/// <param name="e">Event data.</param>
        protected virtual void OnCancelMode(EventArgs e)
        {
			if (this.bar != null)
				this.bar.CancelMode();
			try
			{
#if DEBUG
				if (Switches.ButtonBarEvents.TraceVerbose)
				    TraceUtil.TraceCurrentMethodInfo(this.Name, e);
#endif

				if (CancelMode != null)
					CancelMode(this, e);
			}
			catch (Exception ex)
			{
				TraceUtil.TraceExceptionCatched(ex);
				if (!ExceptionManager.RaiseExceptionCatched(this, ex))
					throw;
			}
            if (toolTipProvider != null)
                toolTipProvider.ActivateToolTip();

			while (this.updateCount > 0)
				EndUpdate(false);
        }

		/// <summary>
        /// Forces the control to invalidate its client area and immediately redraw itself and any child controls.
		/// </summary>
		public override void Refresh()
        {
            if (Updating || !IsHandleCreated)
                updatePending = true;
            else
            {
			    InternalButtonBar bar = ButtonBarChild;
                bar.Dirty = true; // force all buttons to redraw
                base.Refresh();
            }
        }

		/// <override/>
		protected override void OnLayout(LayoutEventArgs levent)
		{
			if (Size.IsEmpty)
				return;

			if (ToolTipProvider != null)
                ResetToolTips();
#if DEBUG

			if (Switches.SplitterControlEvents.TraceVerbose)

			    TraceUtil.TraceCurrentMethodInfo(levent.AffectedProperty);
#endif

			base.OnLayout(levent);

			InternalButtonBar bar = ButtonBarChild;
			Rectangle rect = ComputeButtonBarChildBounds();

            bar.Bounds = rect;
            bar.RecalcLayout(true);
            bar.InvalidateIfDirty();
		}

		/// <summary>
		/// Returns the default size of the control.
		/// </summary>
		protected override Size DefaultSize
		{
			get
			{
				return new Size(100, 23);
			}
		}

		/// <summary>
		/// Returns the bounds for the button bar.
		/// </summary>
		/// <returns>A <see cref="Rectangle"/> indicating where the bar should be drawn.</returns>
        protected virtual Rectangle ComputeButtonBarChildBounds()
        {
            return ClientRectangle;
        }

		/// <summary>
		/// Gets or sets a <see cref="InternalButtonBar"/> that holds an array of buttons.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public InternalButtonBar ButtonBarChild
		{
			get
			{
                if (bar == null)
                {
                    bar = OnCreateButtonBarChild();
                    bar.Dirty = true;
					bar.RepeatClickDelay = 200;
					bar.MinRepeatClickDelay = 20;
                }
				return bar;
			}

			set
			{
				if (bar != value)
				{
					bar = value;
                    bar.Dirty = true;
					PerformLayout();
				}
			}
		}

		/// <summary>
		/// Creates an instance of the <see cref="InternalButtonBar"/> and initializes it with the layout information.
		/// </summary>
		/// <returns>The initialized <see cref="InternalButtonBar"/>.</returns>
		protected virtual InternalButtonBar OnCreateButtonBarChild()
		{
			InternalButtonBar bar = new InternalButtonBar(this);

			// testing code ->
			bar.Buttons = new InternalButton[] {
												   new InternalButton(new Size(25, 10)),
												   new InternalButton(),
												   null,
												   new InternalButton(),
												   new InternalButton(new Size(35, 10)),
			};
			bar.Dirty = true;
			// <- testing

			return bar;
		}
		//protected abstract InternalButtonBar OnCreateButtonBarChild();

	    /// <summary>
	    /// Indicates whether ToolTips are being shown for tabs that have ToolTips set on them.
	    /// </summary>
	    [
	        DefaultValue(false),
            Description("Indicates whether ToolTips are being shown for tabs that have ToolTips set on them.")
	        //Category("ScrollButtons"),
	    ]
        public bool ShowToolTips
        {
            get
            {
                return showToolTips;
            }
            set
            {
                if (showToolTips != value)
                {
                    showToolTips = value;
                    if (!value && toolTipProvider != null)
                    {
                        toolTipProvider.Destroy();
                        toolTipProvider = null;
                    }
                    PerformLayout();
                }
            }
        }

        ControlToolTip IControlToolTipProvider.GetControlToolTip()
        {
            if (showToolTips && !DesignMode && toolTipProvider == null)
            {
                toolTipProvider = new ControlToolTip(this);
                toolTipProvider.CreateToolTipHandle();
            }
            return toolTipProvider;
        }

		/// <override/>
		protected override void OnHandleDestroyed(EventArgs e)
        {
            ResetToolTips();
            if (toolTipProvider != null)
            {
                toolTipProvider.Destroy();
                toolTipProvider = null;
            }
			base.OnHandleDestroyed(e);
        }

		/// <summary>
		/// Reinitializes and hides ToolTips.
		/// </summary>
        public virtual void ResetToolTips()
        {
            if (bar != null)
                bar.ResetToolTips();
        }

	    internal ControlToolTip ToolTipProvider
	    {
	        get
            {
	    	   return toolTipProvider;
	        }
	    }

		/// <summary>
		/// Gets or sets the button look for the arrow buttons.
		/// </summary>
		[
		DefaultValue(ButtonLook.Normal),
		Description("Indicates if arrow buttons should be drawn flat or raised.")
        ]
		public ButtonLook ButtonLook
        {
			get
            {
                return ButtonBarChild.FlatLook ?
                    ButtonLook.Flat :
                    ButtonLook.Normal;
            }
			set
			{
				if (value != ButtonLook)
				{
					ButtonBarChild.FlatLook = (value == ButtonLook.Flat);
					ButtonBarChild.Dirty = true;
					Refresh();
					OnButtonLookChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Raises the <see cref="ButtonLookChanged"/> event.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected virtual void OnButtonLookChanged(EventArgs e)
		{
#if DEBUG
			if (Switches.ButtonBarEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(this.Name, e);
#endif

			if (ButtonLookChanged != null)
				ButtonLookChanged(this, e);
		}
    }
}
