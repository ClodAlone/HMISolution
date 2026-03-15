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
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Runtime.InteropServices;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms.Localization;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// A RecordNavigationControl is a splitter frame with a <see cref="RecordNavigationBar"/> on the bottom left scrollbar.
	/// </summary>
	[
	ToolboxItem(true),
	Designer(typeof(Syncfusion.Windows.Forms.RecordNavigationControlDesigner),
		typeof(System.ComponentModel.Design.IDesigner))
	]
	[System.Drawing.ToolboxBitmap(typeof(Syncfusion.Windows.Forms.RecordNavigationControl), "ToolboxIcons.RecordNavigationControl.bmp")]
	[Description("Represents a control with splitter frame and NavigationBar on the bottom left scrollbar.")]
	public class RecordNavigationControl : SplitterControl
	{
		RecordNavigationScrollBar recordNavigationBar = null;

		/// <summary>
		/// Occurs when the current record is changed.
		/// </summary>
		[Description("Occurs when the current record is changed.")]
		public event CurrentRecordChangedEventHandler CurrentRecordChanged;

		/// <summary>
		/// Occurs when the current record is changing.
		/// </summary>
		[Description("Occurs when the current record is changing.")]
		public event CurrentRecordChangedEventHandler CurrentRecordChanging;

		/// <summary>
		/// Occurs when the user clicks an arrow button.
		/// </summary>
		[Description("Occurs when the user clicks an arrow button.")]
		public event ArrowButtonEventHandler ArrowButtonClicked;

		/// <summary>
		/// Raises the <see cref="ArrowButtonClicked"/> event.
		/// </summary>
		/// <param name="e">An <see cref="ArrowButtonEventArgs" /> that contains the event data.</param>
		protected virtual void OnArrowButtonClicked(ArrowButtonEventArgs e)
		{
#if DEBUG
			if (Switches.RecordNavigationControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name, e);
#endif

			if (ArrowButtonClicked != null)
				ArrowButtonClicked(this, e);
		}

		void NavigationBarArrowButtonClicked(object sender, ArrowButtonEventArgs e)
		{
			OnArrowButtonClicked(e);
		}


        /// <summary>
        /// Raises the <see cref="CurrentRecordChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CurrentRecordEventArgs" /> that contains the event data.</param>
		protected virtual void OnCurrentRecordChanged(CurrentRecordEventArgs e)
		{
#if DEBUG
			if (Switches.RecordNavigationControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name, e);
#endif

			if (CurrentRecordChanged != null)
				CurrentRecordChanged(this, e);
		}

        /// <summary>
        /// Raises the <see cref="CurrentRecordChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CurrentRecordEventArgs" /> that contains the event data.</param>
		protected virtual void OnCurrentRecordChanging(CurrentRecordEventArgs e)
		{
#if DEBUG
			if (Switches.RecordNavigationControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(Name, e);
#endif

			if (CurrentRecordChanging != null)
				CurrentRecordChanging(this, e);
		}

		/// <summary>
		/// Initializes a new <see cref="RecordNavigationControl"/>.
		/// </summary>
		public RecordNavigationControl()
		{
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(RecordNavigationControl));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
			Font = new System.Drawing.Font ("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Bold);
		}

		/// <override/>
		public override bool ThemesEnabled
		{
			get
			{
				return this.NavigationBar.ThemesEnabled;
			}
			set
			{
				this.NavigationBar.ThemesEnabled = value;
				base.ThemesEnabled = value;
			}
		}

		/// <override/>
		protected override void OnFontChanged(EventArgs e)
		{
#if DEBUG
			if (Switches.RecordNavigationControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(this.Font);
#endif

			base.OnFontChanged(e);
			if (recordNavigationBar != null)
			{
				recordNavigationBar.Font = Font;
				PerformLayout();
			}
		}

		/// <override/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (recordNavigationBar != null)
				{
					recordNavigationBar.CurrentRecordChanged -= new CurrentRecordChangedEventHandler(NavigationBarCurrentRecordChanged);
					recordNavigationBar.CurrentRecordChanging -= new CurrentRecordChangedEventHandler(NavigationBarCurrentRecordChanging);
					recordNavigationBar.ArrowButtonClicked -= new ArrowButtonEventHandler(NavigationBarArrowButtonClicked);
					recordNavigationBar.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		/// <override/>
		protected override Control CreateScrollBarContainer(ScrollBars sbType, int index)
		{
			if (sbType == ScrollBars.Horizontal && index == 0)
			{
				return NavigationBar;
			}
			return base.CreateScrollBarContainer(sbType, index);
		}

		/// <summary>
		/// Returns the <see cref="RecordNavigationScrollBar"/> with record information and scroll buttons.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RecordNavigationScrollBar NavigationBar
		{
			get
			{
				if (recordNavigationBar == null)
				{
					SuspendLayout();
					Control sb = CreateScrollBar(ScrollBars.Horizontal, 0);
					recordNavigationBar = new RecordNavigationScrollBar(sb);
					recordNavigationBar.SuspendLayout();
					recordNavigationBar.BeginUpdate();
					recordNavigationBar.DisabledColor = System.Drawing.SystemColors.GrayText;
					recordNavigationBar.EnabledColor = System.Drawing.SystemColors.WindowText;
					recordNavigationBar.ButtonLook = this.ButtonLook;
					recordNavigationBar.MaxRecord = 1000;
					recordNavigationBar.Font = this.Font;
					recordNavigationBar.DisplayArrowButtons = Syncfusion.Windows.Forms.DisplayArrowButtons.All;
					recordNavigationBar.ShowToolTips = this.ShowToolTips;
					recordNavigationBar.MaxLabel = ""; //"of 1000";
					recordNavigationBar.BackColor = System.Drawing.SystemColors.Window;
					recordNavigationBar.AllowAddNew = false;
					recordNavigationBar.CurrentRecordChanged += new CurrentRecordChangedEventHandler(NavigationBarCurrentRecordChanged);
					recordNavigationBar.CurrentRecordChanging += new CurrentRecordChangedEventHandler(NavigationBarCurrentRecordChanging);
					recordNavigationBar.ArrowButtonClicked -= new ArrowButtonEventHandler(NavigationBarArrowButtonClicked);
					recordNavigationBar.Click += new EventHandler(NavigationBarClick);
					recordNavigationBar.Dock = DockStyle.None;
					recordNavigationBar.Anchor = AnchorStyles.None;
					recordNavigationBar.TabStop = false;
					hScrollBars[0] = recordNavigationBar;
					recordNavigationBar.EndUpdate(false);
					recordNavigationBar.ResumeLayout(false);
					ResumeLayout(false);
				}
				return recordNavigationBar;
			}
		}

		/// <override/>
		protected override void OnControlGotFocus()
		{
			base.OnControlGotFocus();
			if (recordNavigationBar != null && ActiveControl != null && recordNavigationBar.Contains(ActiveControl))
				recordNavigationBar.ShowTextBox(true);
		}

		/// <override/>
		protected override void OnValidatingLostFocus()
		{
			if (recordNavigationBar != null)
				recordNavigationBar.NotifyCancelMode();
			base.OnValidatingLostFocus();
		}


		void NavigationBarClick(object sender, EventArgs e)
		{
			OnClick(e);
		}

		/// <summary>
		/// Forces the control to invalidate its client area and immediately redraw itself and any child controls.
		/// </summary>
		public override void Refresh()
		{
			PerformLayout();
			NavigationBar.PerformLayout();
			base.Refresh();
		}

		void NavigationBarCurrentRecordChanged(object sender, CurrentRecordEventArgs e)
		{
			OnCurrentRecordChanged(e);
		}

		void NavigationBarCurrentRecordChanging(object sender, CurrentRecordEventArgs e)
		{
			OnCurrentRecordChanging(e);
		}


		/// <summary>
		/// Gets / sets the current record position.
		/// </summary>
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false)
		]
		public int CurrentRecord
		{
			get
			{
				return NavigationBar.CurrentRecord;
			}
			set
			{
				NavigationBar.CurrentRecord = value;
			}
		}

		/// <summary>
		/// Gets or sets the minimum record position.
		/// </summary>
		[Description("Gets or sets the minimum record position.")]
        public virtual int MinRecord
		{
			get
			{
				return NavigationBar.MinRecord;
			}
			set
			{
				NavigationBar.MinRecord = value;
			}
		}
		private bool ShouldSerializeMinRecord()
		{
			return NavigationBar.RnbData == null && MinRecord != 1;
		}
		/// <summary>
		/// Resets <see cref="MinRecord"/> to default.
		/// </summary>
		public void ResetMinRecord()
		{
			MinRecord = 1;
		}

		/// <summary>
		///  Gets or sets the maximum record position.
		/// </summary>
		[Description("Gets or sets the maximum record position.")]
        public virtual int MaxRecord
		{
			get
			{
				return NavigationBar.MaxRecord;
			}
			set
			{
				NavigationBar.MaxRecord = value;
			}
		}
		private bool ShouldSerializeMaxRecord()
		{
			return NavigationBar.RnbData == null && MaxRecord != -1;
		}
		/// <summary>
		/// Resets <see cref="MaxRecord"/> to default.
		/// </summary>
		public void ResetMaxRecord()
		{
			MaxRecord = -1;
		}


		/// <summary>
		/// Indicates whether adding new records is enabled.
		/// </summary>
        [Description("Indicates whether adding new records is enabled.")]
		public virtual bool AllowAddNew
		{
			get
			{
				return NavigationBar.AllowAddNew;
			}
			set
			{
				NavigationBar.AllowAddNew = value;
			}
		}
		private bool ShouldSerializeAllowAddNew()
		{
			return NavigationBar.RnbData == null && !AllowAddNew;
		}
		/// <summary>
		/// Resets <see cref="AllowAddNew"/> to default.
		/// </summary>
		public void ResetAllowAddNew()
		{
			AllowAddNew = true;
		}

		/// <summary>
		/// Gets or sets the label to be displayed before the record field textbox.
		/// </summary>
		[
		DefaultValue("Record"),
        Description("Gets or sets the label to be displayed before the record field textbox.")
		]
		public virtual string Label
		{
			get
			{
				return NavigationBar.Label;
			}
			set
			{
				NavigationBar.Label = value;
			}
		}

		/// <summary>
		/// Gets or sets an optional maximum label (e.g. "of 1000").
		/// </summary>
		[DefaultValue("")]
        [Description("Gets or sets an optional maximum label (e.g. of 1000).")]
		public virtual string MaxLabel
		{
			get
			{
				return NavigationBar.MaxLabel;
			}
			set
			{
				NavigationBar.MaxLabel = value;
			}
		}

		/// <summary>
		/// Gets / sets the number of Milliseconds to wait before repeatedly firing scroll event.
		/// </summary>
		[
		DefaultValue(200),
		Description("Milliseconds to wait before repeatedly firing scroll event.")
		]
        public virtual int RepeatClickDelay
		{
			get
			{
				return NavigationBar.RepeatClickDelay;
			}
			set
			{
				NavigationBar.RepeatClickDelay = value;
			}
		}

		/// <summary>
		/// Gets / sets the shortest interval for firing scroll event.
		/// </summary>
		[
		DefaultValue(20),
		Description("Shortest interval for firing scroll event.")
		]
        public virtual int MinRepeatClickDelay
		{
			get
			{
				return NavigationBar.MinRepeatClickDelay;
			}
			set
			{
				NavigationBar.MinRepeatClickDelay = value;
			}
		}


		/// <summary>
		/// Gets or sets the arrow button that should be shown in an arrow bar.
		/// </summary>
		[
		DefaultValue(DisplayArrowButtons.All),
        Description("Gets or sets the arrow button that should be shown in an arrow bar.")
		]
        public virtual DisplayArrowButtons NavigationButtons
		{
			get
			{
				return NavigationBar.DisplayArrowButtons;
			}
			set
			{
				NavigationBar.DisplayArrowButtons = value;
			}
		}

		/// <summary>
		/// Gets or sets the backcolor of the navigation bar.
		/// </summary>
		[Description("Gets or sets the backcolor of the navigation bar.")]
		public virtual Color NavigationBarBackColor
		{
			get
			{
				return NavigationBar.BackColor;
			}
			set
			{
				NavigationBar.BackColor = value;
			}
		}
		/// <summary>
		/// Resets the <see cref="NavigationBarBackColor"/> property to its default value.
		/// </summary>
		public void ResetNavigationBarBackColor()
		{
			NavigationBar.BackColor = SystemColors.Window;
		}
		bool ShouldSerializeNavigationBarBackColor()
		{
			return NavigationBar.BackColor != SystemColors.Window;
		}

		/// <summary>
		/// Gets or sets the width of the navigation bar.
		/// </summary>
		[DefaultValue(216)]
        [Description("Gets or sets the width of the navigation bar.")]
        public virtual int NavigationBarWidth
		{
			get
			{
				return NavigationBar.PrefWidth;
			}
			set
			{
				NavigationBar.PrefWidth = value;
				PerformLayout();
				Refresh();
			}
		}


		/// <summary>
		/// Gets or sets the color of arrows in enabled buttons.
		/// </summary>
        [Description("Gets or sets the color of arrows in enabled buttons.")]
		public virtual Color EnabledArrowColor
		{
			get
			{
				return NavigationBar.EnabledColor;
			}
			set
			{
				NavigationBar.EnabledColor = value;
			}
		}
		/// <summary>
		/// Resets the <see cref="EnabledArrowColor"/> property to its default value.
		/// </summary>
		public void ResetEnabledArrowColor()
		{
			EnabledArrowColor = SystemColors.WindowText;
		}
		bool ShouldSerializeEnabledArrowColor()
		{
			return EnabledArrowColor != SystemColors.WindowText;
		}

		/// <summary>
		/// Gets or sets the color of arrows in disabled buttons.
		/// </summary>
        [Description("Gets or sets the color of arrows in disabled buttons.")]
		public virtual Color DisabledArrowColor
		{
			get
			{
				return NavigationBar.DisabledColor;
			}
			set
			{
				NavigationBar.DisabledColor = value;
			}
		}
		/// <summary>
		/// Resets the <see cref="DisabledArrowColor"/> property to its default value.
		/// </summary>
		public void ResetDisabledArrowColor()
		{
			DisabledArrowColor = SystemColors.GrayText;
		}
		bool ShouldSerializeDisabledArrowColor()
		{
			return DisabledArrowColor != SystemColors.GrayText;
		}

		/// <override/>
		protected override void OnButtonLookChanged(EventArgs e)
		{
			if (this.recordNavigationBar != null)
				this.recordNavigationBar.ButtonLook = this.ButtonLook;
#if DEBUG
			if (Switches.RecordNavigationControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(this.ButtonLook);
#endif

			base.OnButtonLookChanged(e);
		}

		/// <override/>
		protected override void OnShowToolTipsChanged(EventArgs e)
		{
			if (this.recordNavigationBar != null)
				this.recordNavigationBar.ShowToolTips = ShowToolTips;
#if DEBUG
			if (Switches.RecordNavigationControlEvents.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(this.ShowToolTips);
#endif

			base.OnShowToolTipsChanged(e);
		}

		/// <summary>
		/// Occurs when the user drags the splitter bar.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="x">The current horizontal position in pixels.</param>
		/// <param name="y">The current vertical position in pixels.</param>
		public override void OnMoveSplitter(object sender, int x, int y)
		{
			this.NavigationBar.HideTextBox();
			base.OnMoveSplitter(sender, x, y);
		}

	}
}