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
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace Syncfusion.Windows.Forms
{

	[Syncfusion.Documentation.DocumentationExclude()]
	public enum FlatScrollBarStyle
	{
		Odyssey       = 3,
		Flat          = 2,
		Encarta       = 1,
		Regular       = 0
	};

	
	[Syncfusion.Documentation.DocumentationExclude()]
	public enum FlatScrollBarType
	{
		Horizontal = 0,
		Vertical = 1
	};



	/// <summary>
	///    <para>Implements the basic functionality of a scroll bar control.</para>
	/// </summary>
	/// <remarks>
	///    <para>To adjust the value
	///       range of the scroll bar control,
	///       set the <see cref="System.Windows.Forms.ScrollBar.Minimum"/> and <see cref="System.Windows.Forms.ScrollBar.Maximum"/>
	///       properties.
	///       To adjust the distance the scroll box moves, set the <see cref="System.Windows.Forms.ScrollBar.SmallChange"/> and <see cref="System.Windows.Forms.ScrollBar.LargeChange"/> properties. To
	///       adjust the starting point of the scroll box, set the <see cref="System.Windows.Forms.ScrollBar.Value"/> property when the
	///       control is initially displayed.</para>
	///    <note type="note">
	///       The scroll box is sometimes
	///       referred to as the "thumb".
	///    </note>
	/// </remarks>
	/// <seealso cref="FlatVScrollBar"/>
	/// <seealso cref="FlatHScrollBar"/>
	[
	DefaultEvent("Scroll"),
	Designer("System.Windows.Forms.Design.ScrollBarDesigner, System.Design", "System.ComponentModel.Design.IDesigner"),
	DefaultProperty("Value"),
	ToolboxItem(false),
	Syncfusion.Documentation.DocumentationExclude()
	]
	public abstract class FlatScrollBar : ScrollBar, IScrollBar
	{
		private bool supportsThumbTrack;
		private bool supportsScrollTips;
		private bool isThumbTracking = false;

		[Syncfusion.Documentation.DocumentationExclude()]
		internal class NativeMethods 
		{
			public const int WS_HSCROLL = 0x100000;
			public const int WS_VSCROLL = 0x200000;

			[
				StructLayout(LayoutKind.Sequential) 
				]
				[Syncfusion.Documentation.DocumentationExclude()]
				internal struct SCROLLINFO 
			{
				public int cbSize;
				public int fMask;
				public int nMin;
				public int nMax;
				public int nPage;
				public int nPos;
				public int nTrackPos;
			}

			[
				ComVisibleAttribute(true),
				StructLayout(LayoutKind.Sequential, Pack = 1), 
				Syncfusion.Documentation.DocumentationExclude()]
				internal class INITCOMMONCONTROLSEX 
			{
				public int dwSize = Marshal.SizeOf(typeof(INITCOMMONCONTROLSEX));
				public int dwICC;
			}

			[DllImport("comctl32")]
			extern public static void InitCommonControls()  ;

			[DllImport("comctl32")]
			extern public static bool InitCommonControlsEx(INITCOMMONCONTROLSEX icc)  ;

			[DllImport("comctl32")]
			extern public static bool InitializeFlatSB(IntPtr hwnd);

			[DllImport("comctl32")]
			extern public static bool UninitializeFlatSB(IntPtr hwnd);

			[DllImport("comctl32")]
			extern public static int FlatSB_SetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO si, bool redraw)  ;

			[DllImport("comctl32")]
			extern public static bool FlatSB_GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO si);

			[DllImport("comctl32")]
			extern public static bool FlatSB_SetScrollProp(IntPtr hwnd, int index, int NewValue, bool fRedraw);

			[DllImport("comctl32")]
			extern public static bool FlatSB_GetScrollProp(IntPtr hwnd, int index, out int value);

			[DllImport("comctl32")]
			extern public static bool FlatSB_EnableScrollBar(IntPtr hwnd, int wSBflags, int wArrows);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		internal enum SIF
		{
			RANGE           = 0x0001,
			PAGE            = 0x0002,
			POS             = 0x0004,
			DISABLENOSCROLL = 0x0008,
			TRACKPOS        = 0x0010,
			ALL             = SIF.RANGE | SIF.PAGE | SIF.POS | SIF.TRACKPOS
		};

		internal enum WSB
		{
			PROP_CYVSCROLL  = 0x0001,
			PROP_CXHSCROLL  = 0x0002,
			PROP_CYHSCROLL  = 0x0004,
			PROP_CXVSCROLL  = 0x0008,
			PROP_CXHTHUMB   = 0x0010,
			PROP_CYVTHUMB   = 0x0020,
			PROP_VBKGCOLOR  = 0x0040,
			PROP_HBKGCOLOR  = 0x0080,
			PROP_VSTYLE     = 0x0100,
			PROP_HSTYLE     = 0x0200,
			PROP_WINSTYLE   = 0x0400,
			PROP_PALETTE    = 0x0800,
			PROP_MASK       = 0x0FFF
		};            

		internal enum ESB
		{
			ENABLE_BOTH     = 0x0000,
			DISABLE_BOTH    = 0x0003,
			DISABLE_LEFT    = 0x0001,
			DISABLE_RIGHT   = 0x0002,
			DISABLE_UP      = 0x0001,
			DISABLE_DOWN    = 0x0002,
			DISABLE_LTUP    = DISABLE_LEFT,
			DISABLE_RTDN    = DISABLE_RIGHT
		}

		internal FlatScrollBarType scrollBarType = FlatScrollBarType.Horizontal;
		internal FlatScrollBarStyle flatScrollBarStyle = FlatScrollBarStyle.Encarta;

#if SCROLLTIP        
        private bool showScrollTips = false;
        private ScrollTip scrollTip;
#endif

		// Constructors
		public FlatScrollBar() : base()
		{
			NativeMethods.INITCOMMONCONTROLSEX cc;
			cc = new NativeMethods.INITCOMMONCONTROLSEX();
			cc.dwICC = 8;
			NativeMethods.InitCommonControlsEx(cc);
			// BackColor = SystemColors.ScrollBar;
			BackColor = Color.Aquamarine;
		}

		/// <override/>
		protected override CreateParams CreateParams
		{
			[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
			get 
			{
				CreateParams cp;
				cp = base.CreateParams;
				cp.ClassName = null;
				return cp;
			}
		}

		/// <summary>
		///     Creates the handler. Overridden to help set up scrollbar information.
		/// </summary>
		protected override void OnHandleCreated(EventArgs e)  
		{
			NativeMethods.InitializeFlatSB(Handle);
			InitScrollInfo();
			UpdateScrollInfo();
#if SCROLLTIP
            _ShowScrollTips(showScrollTips);
#endif
			base.OnHandleCreated(e);
		}

		void InitScrollInfo()
		{
			int propIndex = (int) ((scrollBarType == FlatScrollBarType.Horizontal) ? WSB.PROP_HSTYLE : WSB.PROP_VSTYLE);
			NativeMethods.FlatSB_SetScrollProp(Handle, propIndex, (int) this.flatScrollBarStyle, false);
			int win32Color = ColorTranslator.ToWin32(this.BackColor);
			propIndex = (int) ((scrollBarType == FlatScrollBarType.Horizontal) ? WSB.PROP_HBKGCOLOR : WSB.PROP_VBKGCOLOR);
			NativeMethods.FlatSB_SetScrollProp(Handle, propIndex, win32Color, true);
		}

		/// <override/>
		public override void Refresh()
		{
			base.Refresh();
		}

		/// <override/>
		protected override void OnHandleDestroyed(EventArgs e)  
		{
			if (IsHandleCreated)
				NativeMethods.UninitializeFlatSB(Handle);

#if SCROLLTIP
            _ShowScrollTips(false);
#endif
			base.OnHandleDestroyed(e);
		}

		private int ReflectPosition(int position)  
		{
			if (scrollBarType == FlatScrollBarType.Horizontal)
				return ((this.Minimum + ((this.Maximum - this.LargeChange) + 1)) - position);

			return position;
		}

		internal static int LOWORD(int n)  
		{
			return (n & 0xffff);
		}

		internal static int HIWORD(int n)  
		{
			return ((n >> 16) & 0xffff);
		}

		internal static int LOWORD(IntPtr n)  
		{
			return LOWORD((int) n);
		}

		internal static int HIWORD(IntPtr n)  
		{
			return HIWORD((int) n);
		}

		/// <summary>
		/// Indicates whether the associated control should scroll while the user is dragging a scrollbar thumb.
		/// </summary>
		[
		Browsable(true), 
		Category("Behavior"),
		Description("Specifies if the associated control should scroll while the user is dragging a scrollbar thumb."),
		DefaultValue(false)
		]
		public bool SupportsThumbTrack
		{
			get
			{
				return supportsThumbTrack;
			}
			set
			{
				supportsThumbTrack = value;
			}
		}

		/// <summary>
		/// Indicates whether the parent control should show ScrollTips while the user is dragging a scrollbar thumb.
		/// </summary>
		/// <remarks>
		/// <see cref="ScrollControl"/> checks this property to determine if ScrollTips should be displayed.
		/// </remarks>
		[
		Browsable(true), 
		Category("Behavior"),
		Description("Specifies if the control should show ScrollTips while the user is dragging a scrollbar thumb."),
		DefaultValue(false)
		]
		public bool SupportsScrollTips
		{
			get
			{
				return supportsScrollTips;
			}
			set
			{
				if (supportsScrollTips != value)
				{
					supportsScrollTips = value;
				}
			}
		}


		/// <override/>
		protected override void OnBackColorChanged(EventArgs e)  
		{
			if (IsHandleCreated)
			{
				int win32Color = ColorTranslator.ToWin32(this.BackColor);
				int propIndex = (int) ((scrollBarType == FlatScrollBarType.Horizontal) ? WSB.PROP_HBKGCOLOR : WSB.PROP_VBKGCOLOR);
				NativeMethods.FlatSB_SetScrollProp(Handle, propIndex, win32Color, true);
			}
			base.OnBackColorChanged(e);
		}

		private void WmReflectScroll(ref Message m)  
		{
			NativeMethods.SCROLLINFO si;
			ScrollEventArgs sa;
			ScrollEventType saType = (ScrollEventType)LOWORD(m.WParam);
#if SCROLLTIP
            bool relayScrollTip = false;
#endif

			if (this.RightToLeft == RightToLeft.Yes)
			{
				switch(saType)
				{
					case ScrollEventType.First:
						saType = ScrollEventType.Last;
						break;
					case ScrollEventType.Last:
						saType = ScrollEventType.First;
						break;
					case ScrollEventType.SmallDecrement:
						saType = ScrollEventType.SmallIncrement;
						break;
					case ScrollEventType.SmallIncrement:
						saType = ScrollEventType.SmallDecrement;
						break;
					case ScrollEventType.LargeDecrement:
						saType = ScrollEventType.LargeIncrement;
						break;
					case ScrollEventType.LargeIncrement:
						saType = ScrollEventType.LargeDecrement;
						break;
				}
			}

			int NewValue = this.Value;
			isThumbTracking = false;
			switch(saType) 
			{
				case ScrollEventType.First:
					NewValue = this.Minimum;
					break;
				case ScrollEventType.Last:
					NewValue = ((this.Maximum - this.LargeChange) + 1);
					break;
				case ScrollEventType.SmallDecrement:
					NewValue = Math.Max((this.Value - this.SmallChange), this.Minimum);
					break;
				case ScrollEventType.SmallIncrement:
					NewValue = Math.Min((this.Value + this.SmallChange), ((this.Maximum - this.LargeChange) + 1));
					break;
				case ScrollEventType.LargeDecrement:
					NewValue = Math.Max((this.Value - this.LargeChange), this.Minimum);
					break;
				case ScrollEventType.LargeIncrement:
					NewValue = Math.Min((this.Value + this.LargeChange), ((this.Maximum - this.LargeChange) + 1));
					break;

				case ScrollEventType.ThumbTrack:
#if SCROLLTIP        
                if (this.scrollTip != null)
                    relayScrollTip = true;
#endif
//					if (!SupportsThumbTrack)
//						return;
//					else
						isThumbTracking = true;

					goto case ScrollEventType.ThumbPosition;

				case ScrollEventType.ThumbPosition: 
					si = new NativeMethods.SCROLLINFO();
					si.cbSize = Marshal.SizeOf(typeof(NativeMethods.SCROLLINFO));
					si.fMask = (int) SIF.ALL;
					NativeMethods.FlatSB_GetScrollInfo(this.Handle, (int) this.scrollBarType, ref si);

					if (this.RightToLeft == RightToLeft.Yes) 
						NewValue = this.ReflectPosition(si.nTrackPos);
					else
						NewValue = si.nTrackPos;
					break;
			}
                
			sa = new ScrollEventArgs(saType, NewValue);
		   
#if SCROLLTIP        
            if (relayScrollTip)
            {
                this.scrollTip.RestoreBackground(this.scrollTip);
                this.Value = sa.NewValue;
                this.scrollTip.HandleScrollMessage(null, saType, 0, "ScrollPosition " + sa.NewValue.ToString(), this);
            }
            else
            {
                if (this.scrollTip != null)
                    this.scrollTip.RestoreBackground(this.scrollTip);
				this.OnScroll(sa);
				this.Value = sa.NewValue;
			}
#else
			this.OnScroll(sa);
			this.Value = sa.NewValue;
#endif
		}

		/// <override/>
		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
		protected override void WndProc(ref Message m)  
		{
			//Debug.WriteLine(String.Format("Message {0:x4}", m.Msg));
			switch (m.Msg)
			{
				case 0x0114/*WM_HSCROLL*/:
				case 0x0115/*WM_VSCROLL*/:
					WmReflectScroll(ref m);
					break;

				default:
					base.WndProc(ref m);
					break;
			}
		}

		/// <summary>
		/// Indicates whether the scroll bar is currently in thumb drag mode.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsThumbTracking
		{
			get
			{
				return isThumbTracking;
			}
		}


		void IScrollBar.UpdateScrollInfo()
		{
			UpdateScrollInfo();
		}

		internal new void UpdateScrollInfo()  
		{
			NativeMethods.SCROLLINFO si;
			if (this.IsHandleCreated && this.Enabled)
			{
				si = new NativeMethods.SCROLLINFO();
				si.cbSize = Marshal.SizeOf(typeof(NativeMethods.SCROLLINFO));
				si.fMask = (int) (SIF.ALL | SIF.DISABLENOSCROLL);
				si.nMin = this.Minimum;
				si.nMax = this.Maximum;
				si.nPage = Math.Min(this.LargeChange,( (this.Maximum - this.Minimum) + 1));
				if (this.RightToLeft == RightToLeft.Yes) 
					si.nPos = this.ReflectPosition(this.Value);
				else
					si.nPos = this.Value;
		   
				si.nTrackPos = 0;
				NativeMethods.FlatSB_SetScrollInfo(this.Handle, (int) this.scrollBarType, ref si, true);

				if (si.nMax <= si.nMin)
				{
					NativeMethods.FlatSB_EnableScrollBar(Handle, (int) this.scrollBarType, (int) ESB.DISABLE_BOTH);
				}
			}
		}

		/// <summary>
        /// Gets / sets the effect of clicking within the scroll bar but outside the scroll box.
		/// </summary>
        [
		DefaultValueAttribute(10),
		CategoryAttribute("Behavior")
		]
		public new int LargeChange
		{
			get 
			{
				return base.LargeChange;
			}
			set 
			{
				if (this.LargeChange != value) 
				{
					base.LargeChange = value;
					UpdateScrollInfo();
				}
			}
		}

		/// <summary>
		/// Gets / sets the maximum range of the scroll bar. Default is 100.
		/// </summary>
        [
		CategoryAttribute("Behavior"),
		DefaultValueAttribute(100)
		]
		public new int Maximum
		{
			get 
			{
				return base.Maximum;
			}
			set 
			{
				if (this.Maximum != value) 
				{
					base.Maximum = value;
					UpdateScrollInfo();
				}
			}
		}

		/// <summary>
		/// Gets / sets the minimum range of the scroll bar. Default is zero.
		/// </summary>
        [
		CategoryAttribute("Behavior"),
		DefaultValueAttribute(0)
		]
		public new int Minimum
		{
			get 
			{
				return base.Minimum;
			}
			set 
			{
				if (this.Minimum != value) 
				{
					base.Minimum = value;
					UpdateScrollInfo();
				}
			}
		}

    
		/// <summary>
        /// Gets / sets the effect of clicking the scroll arrows at each end of the control.
		/// </summary>
        [
		CategoryAttribute("Behavior"),
		DefaultValueAttribute(1),
		]
		public new int SmallChange
		{
			get 
			{
				return base.SmallChange;
			}
			set 
			{
				if (this.SmallChange != value)
				{
					base.SmallChange = value;
					this.UpdateScrollInfo();
				}
			}
		}
    
		/// <summary>
		/// Gets / sets the starting value of the scroll bar.
		/// </summary>
        [
		DefaultValueAttribute(0),
		CategoryAttribute("Behavior"),
		BindableAttribute(true),
		]
		public new int Value
		{
			get 
			{
				return base.Value;
			}
			set 
			{
				if (this.Value != value) 
				{
					base.Value = value;
					UpdateScrollInfo();
				}
			}
		}

		/// <summary>
		///     Gets / sets the background color of this scrollbar. 
		/// </summary>
		[
		Category("Appearance"),
		Browsable(true),
		]
		public override Color BackColor
		{
			get 
			{
				return base.BackColor;
			}
			set 
			{
				if (base.BackColor != value)
				{
					base.BackColor = value;
					if (IsHandleCreated)
					{
						int propIndex = (int) ((scrollBarType == FlatScrollBarType.Horizontal) ? WSB.PROP_HBKGCOLOR : WSB.PROP_VBKGCOLOR);
						NativeMethods.FlatSB_SetScrollProp(Handle, propIndex, value.ToArgb(), true);
					}
				}
			}
		}

		/// <summary>
		/// Resets the backcolor of the scroll bar.
		/// </summary>
        public override void ResetBackColor()
		{
			this.BackColor = SystemColors.ScrollBar;
		}

        /// <summary>
        /// Gets / sets the scroll bar style. Default is Flat style.
        /// </summary>
		[
		DefaultValue(FlatScrollBarStyle.Flat),
		Category("Behavior"),
		]
        public FlatScrollBarStyle Appearance
		{
			get 
			{
				return this.flatScrollBarStyle;
			}
			set 
			{
				if (this.flatScrollBarStyle != value)
				{
					this.flatScrollBarStyle = value;
					if (IsHandleCreated)
					{
						int propIndex = (int) ((scrollBarType == FlatScrollBarType.Horizontal) ? WSB.PROP_HSTYLE : WSB.PROP_VSTYLE);
						NativeMethods.FlatSB_SetScrollProp(Handle, propIndex, (int) this.flatScrollBarStyle, true);
					}
				}
			}
		}

#if SCROLLTIP        
        public bool ShowScrollTips 
        {
            get 
            {
                 return showScrollTips;
            }
            set 
            {
                 if (showScrollTips != value)
                 {
                     showScrollTips = value;
                     _ShowScrollTips(showScrollTips);
                 }
            }
        }

        private void _ShowScrollTips(bool showScrollTips)
        {
            if (!IsHandleCreated)
                return;

            if (showScrollTips)
            {
                if (scrollTip == null)
                {
                    scrollTip = new ScrollTip(this);
                    //scrollTip.CreateToolTipHandle();
                }
            }
            else if (scrollTip != null)
            {
                scrollTip.Dispose();
                scrollTip = null;
            }
        }
#else
		public bool ShowScrollTips 
		{
			get 
			{
				return false;
			}
			set 
			{
			}
		}
#endif    
	}
}

