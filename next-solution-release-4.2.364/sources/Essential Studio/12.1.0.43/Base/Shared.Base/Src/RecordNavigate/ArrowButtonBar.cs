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
using System.Diagnostics;
using Syncfusion.Diagnostics;
using System.Security;
using System.Security.Permissions;

namespace Syncfusion.Windows.Forms
{
    /// <summary>
    /// Specifies the arrow button that should be shown in an arrow bar.
    /// </summary>
	public enum DisplayArrowButtons
    {
		/// <summary>
		/// Don't show buttons.
		/// </summary>
		None,

		/// <summary>
		/// Show up and down buttons.
		/// </summary>
		Single,

		/// <summary>
		/// Show up, down, move first and move last buttons.
		/// </summary>
		All
	}



    /// <summary>
    /// This control is used by <see cref="TabBarSplitterControl"/> and <see cref="RecordNavigationControl"/>
    /// to display arrow buttons.
    /// </summary>
	[
	ToolboxItem(false),
	]
    public class ArrowButtonBar : ButtonBar,
        IInternalArrowButtonParent,
        IInternalButtonParent
    {
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		internal DisplayArrowButtons displayArrowButtons;

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		internal Color enabledColor = SystemColors.WindowText;

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		internal Color disabledColor = SystemColors.GrayText;

		private ArrowType enableButtonFlags = ArrowType.All;

		private bool themesEnabled = false;

		/// <summary>
		/// Initializes a new <see cref="ArrowButtonBar"/>.
		/// </summary>
        public ArrowButtonBar()
        {
        }

		/// <summary>
		/// Occurs when the user clicks on an arrow button.
		/// </summary>
        [Description("Occurs when the user clicks on an arrow button.")]
		public event ArrowButtonEventHandler ArrowButtonClicked;

		/// <summary>
		/// Raises the <see cref="ArrowButtonClicked"/> event.
		/// </summary>
		/// <param name="e">An <see cref="ArrowButtonEventArgs" /> that contains the event data.</param>
		protected virtual void OnArrowButtonClicked(ArrowButtonEventArgs e)
		{
#if DEBUG
			if (Switches.ArrowButtonBarEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(this.Name, e);
#endif

			if (ArrowButtonClicked != null)
				ArrowButtonClicked(this, e);
		}

		/// <summary>
		/// Indicates whether themes are enabled for this control.
		/// </summary>
        [Description("Indicates whether themes are enabled for this control."), DefaultValue(false)]
		public virtual bool ThemesEnabled
		{
			get{return this.themesEnabled;}
			set
			{
				this.themesEnabled = value;
				this.OnThemeChanged(EventArgs.Empty);
				this.Invalidate();
			}
		}

		/// <summary>
		/// Fired when the ThemesEnabled property changes.
		/// </summary>
        [Description("Fired when the ThemesEnabled property changes.")]
		public event EventHandler ThemeChanged;
		/// <override/>
		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 0x031A/*WM_THEMECHANGED*/)
			{
				this.Invalidate();
			}
			base.WndProc(ref m);
		}

		/// <summary>
		/// Raises the ThemeChanged event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnThemeChanged method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Note to Inheritors: When overriding OnThemeChanged in a derived
		/// class, be sure to call the base class's OnThemeChanged method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnThemeChanged(EventArgs e)
		{
#if DEBUG
			if (Switches.ArrowButtonBarEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(this.ThemesEnabled);
#endif

			if(this.ThemeChanged != null)
			{
				this.ThemeChanged(this, e);
			}
		}
		/// <override/>
		protected override InternalButtonBar OnCreateButtonBarChild()
		{
			InternalButtonBar bar = new InternalButtonBar(this);
			return bar;
		}


		/// <summary>
		/// Occurs when the specified button was clicked or the mouse is pressed down on the button.
		/// </summary>
		/// <param name="button">The source of the event.</param>
		public virtual void /*IInternalButtonParent.*/OnClickedButton(InternalButton button)
		{
			InternalArrowButton arrow = button as InternalArrowButton;
			Debug.WriteLine("IInternalButtonParent.OnClickedButton(" + button.ToString() + ")");

			ArrowButtonEventArgs e = new ArrowButtonEventArgs(arrow.Type);
			OnArrowButtonClicked(e);
			if (e.Cancel)
				return;

			/*
			if (arrow)
			{
				switch (arrow.Kind)
				{
				case ArrowType.First:
				case ArrowType.Last:
				case ArrowType.Next:
				case ArrowType.Previous:
				}
			}
			*/
		}

		/// <summary>
		/// Initializes the the arrow bar.
		/// </summary>
		public virtual void InitBars()
		{
			//initialized = true;

			InternalButtonBar bar = this.ButtonBarChild;
			switch (displayArrowButtons)
			{
			case DisplayArrowButtons.All:
				bar.Buttons = AllArrowButtons;
				break;

			case DisplayArrowButtons.None:
				bar.Buttons = NoArrowButtons;
				break;

			default:
				bar.Buttons = SingleArrowButtons;
				break;
			}

			bar.Dirty = true;

			PerformLayout();
		}

        // Properties

		/// <summary>
		/// Gets or sets the number of Milliseconds to wait before firing scroll event.
		/// </summary>
		[
        ////Category("ScrollButtons"),
		DefaultValue(200),
		Description("Milliseconds to wait before firing scroll event.")
		]
		public int RepeatClickDelay
		{
			get
			{
				return this.ButtonBarChild.RepeatClickDelay;
			}
			set
			{
				this.ButtonBarChild.RepeatClickDelay = value;
			}
		}

		/// <summary>
		/// Gets or sets the Shortest interval for firing scroll event.
		/// </summary>
		[
		////Category("ScrollButtons"),
		DefaultValue(20),
		Description("Shortest interval for firing scroll event.")
		]
		public int MinRepeatClickDelay
		{
			get
			{
				return this.ButtonBarChild.MinRepeatClickDelay;
			}
			set
			{
				this.ButtonBarChild.MinRepeatClickDelay = value;
			}
		}

		/// <summary>
		/// Gets or sets the arrow buttons to be shown in an arrow bar.
		/// </summary>
		[
        //Category("ScrollButtons"),
		DefaultValue(DisplayArrowButtons.None),
		Description("Specifies arrow buttons to be shown in an arrow bar.")
		]
        public virtual DisplayArrowButtons DisplayArrowButtons
        {
			get { return displayArrowButtons; }
			set
			{
				if (value != displayArrowButtons)
				{
					displayArrowButtons = value;
					InitBars();
					Refresh();
				}
			}
		}

		/// <summary>
		/// Gets or sets the buttons to show enabled. Other buttons are disabled.
		/// </summary>
        [DefaultValue(ArrowType.All)]
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ArrowType EnableButtonFlags
        {
			get
            {
                return enableButtonFlags;
            }
			set
			{
				enableButtonFlags = value;
				foreach (InternalButton button in this.ButtonBarChild.Buttons)
				{
                    if (button != null && button.Cookie is ArrowType)
                    {
					    ArrowType kind = (ArrowType) button.Cookie;
					    button.Enabled = ((enableButtonFlags & kind) != 0);
                    }
				}
				if (this.ButtonBarChild.Dirty)
					Invalidate();
			}
		}

        //Category("ScrollButtons"
		/// <summary>
		/// Gets or sets the color of arrows in enabled buttons.
		/// </summary>
		[Description("The color of arrows in enabled buttons."), DefaultValue(typeof(Color), "WindowText")]
		public Color EnabledColor
		{
			get
			{
                return enabledColor;
            }
			set
			{
				enabledColor = value;
                Refresh();
			}
		}
		/// <summary>
		/// Resets <see cref="EnabledColor"/> to default.
		/// </summary>
		public void ResetEnabledColor()
		{
			EnabledColor = SystemColors.WindowText;
		}
		internal bool ShouldSerializeEnabledColor()
		{
			return EnabledColor != SystemColors.GrayText;
		}

        //Category("ScrollButtons")
		/// <summary>
		/// Gets or sets the color of arrows in disabled buttons.
		/// </summary>
		[Description("The color of arrows in disabled buttons.")]
		public Color DisabledColor
		{
			get
			{
                return disabledColor;
            }
			set
			{
				disabledColor = value;
                Refresh();
			}
		}
		/// <summary>
		/// Resets <see cref="DisabledColor"/> to default.
		/// </summary>
		public void ResetDisabledColor()
		{
			DisabledColor = SystemColors.GrayText;
		}
		internal bool ShouldSerializeDisabledColor()
		{
			return DisabledColor != SystemColors.GrayText;
		}


		// Private Properties

		/// <summary>
		/// Returns an array with buttons to show when <see cref="DisplayArrowButtons"/> is <see cref="Syncfusion.Windows.Forms.DisplayArrowButtons.None"/>.
		/// </summary>
		protected virtual InternalButton[] NoArrowButtons
		{
			get
			{
				return new InternalButton[0];
			}
		}

		/// <summary>
		/// Returns an array with buttons to show when <see cref="DisplayArrowButtons"/> is <see cref="Syncfusion.Windows.Forms.DisplayArrowButtons.Single"/>.
		/// </summary>
		protected virtual InternalButton[] SingleArrowButtons
		{
			get
			{
				return new InternalButton[] {
					new InternalArrowButton(this, ArrowType.Previous, "Scroll to previous tab."),
					new InternalArrowButton(this, ArrowType.Next, "Scroll to next tab."),
				};
			}
		}

		/// <summary>
		/// Returns an array with buttons to show when <see cref="DisplayArrowButtons"/> is <see cref="Syncfusion.Windows.Forms.DisplayArrowButtons.All"/>.
		/// </summary>
		protected virtual InternalButton[] AllArrowButtons
		{
			get
			{
				return new InternalButton[] {
					new InternalArrowButton(this, ArrowType.First, "Scroll to first tab."),
					new InternalArrowButton(this, ArrowType.Previous, "Scroll to previous tab."),
					new InternalArrowButton(this, ArrowType.Next, "Scroll to next tab."),
					new InternalArrowButton(this, ArrowType.Last, "Scroll to last tab."),
				};
			}
		}
    }
}
