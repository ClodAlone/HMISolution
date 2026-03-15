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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Diagnostics;
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms
{
	[
	ToolboxItem(false),
	Syncfusion.Documentation.DocumentationExclude()
	]
	internal class ReflectScrollBar: ScrollBar, IScrollBar
	{
		private bool supportsThumbTrack;
		private bool supportsScrollTips;
		private bool isThumbTracking = false;

#if CTLCOLOR
		private IntPtr brushHandle = IntPtr.Zero;
#endif

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

		Control reflectParent;
		ScrollBars scrollBarType;

		public ReflectScrollBar(Control reflectParent, ScrollBars scrollBarType)
		{
			this.reflectParent = reflectParent;
			this.scrollBarType = scrollBarType;

			NativeMethods.SCROLLINFO si = new NativeMethods.SCROLLINFO();
			si.cbSize = Marshal.SizeOf(typeof(NativeMethods.SCROLLINFO));
			si.fMask = (int) SIF.ALL;
			if (NativeMethods.GetScrollInfo(this.reflectParent.Handle, this.ScrollInfoBar, ref si)
				&& si.nPos != -1)
			{
				try
				{
					base.Minimum = si.nMin;
					base.Maximum = si.nMax;
					base.Value = Math.Max(Math.Min(si.nPos, si.nMax), si.nMin);
					base.LargeChange = si.nPage;
					base.SmallChange = 1;
				}
				catch (Exception ex)
				{
					TraceUtil.TraceExceptionCatched(ex);
					if (!ExceptionManager.RaiseExceptionCatched(null, ex))
						throw;
				}
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Control ReflectParent
		{
			get
			{
				return reflectParent;
			}
		}


		private int ReflectPosition(int position)  
		{
			if (scrollBarType == ScrollBars.Horizontal)
				return ((this.Minimum + ((this.Maximum - this.LargeChange) + 1)) - position);

			return position;
		}

		/// <summary>
		/// True if scroll bar is currently in thumb drag mode.
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
		/// <see cref="ScrollControl"/> Checks this property to determine if ScrollTips should be displayed.
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


#if CTLCOLOR
	    protected override void OnBackColorChanged(EventArgs e)  
	    {
            this.RecreateBrush();
            this.Invalidate();
            base.OnBackColorChanged(e);
	    }

        private void EnsureBrushCreated()  
	    {
	    	if (this.brushHandle != IntPtr.Zero)
                return;

            Color color = Color.Green; //this.BackColor;
	    	if (ColorTranslator.ToOle(color) == 0)
	    	   this.brushHandle = NativeMethods.GetSysColorBrush((ColorTranslator.ToOle(color) & 0xff));
            else
	    	   this.brushHandle = NativeMethods.CreateSolidBrush(ColorTranslator.ToWin32(color));
	    }

	    private void RecreateBrush()  
	    {
            if (this.brushHandle != IntPtr.Zero) 
                NativeMethods.DeleteObject(this.brushHandle);
            this.brushHandle = IntPtr.Zero;
            this.EnsureBrushCreated();
	    }

		static int HIWORD(int n)  
		{
			return ((n >> 16) & 0xffff);
		}
		
		private void WmReflectCtlColor(ref Message m)
        {
            if (HIWORD((int) m.LParam) ==  0x0005/*CTLCOLOR_SCROLLBAR*/)
            {
                IntPtr hdc = m.WParam;
                NativeMethods.SetTextColor(hdc, ColorTranslator.ToWin32(this.ForeColor));
                NativeMethods.SetBkColor(hdc, ColorTranslator.ToWin32(this.BackColor));
                this.EnsureBrushCreated();
                m.Result = this.brushHandle;
            }
        }
#endif
		public static int LOWORD(int n)  
		{
			return (n & 0xffff);
		}

		public static int LOWORD(IntPtr n)  
		{
			return LOWORD((int) n);
		}

  
		public void ReflectScrollMessage(ref Message m)  
		{
			NativeMethods.SCROLLINFO si;
			ScrollEventArgs sa;
			ScrollEventType saType = (ScrollEventType)LOWORD(m.WParam);
#if SCROLLTIP
            bool relayScrollTip = false;
#endif

			if( this.RightToLeft == RightToLeft.Yes && this.scrollBarType == ScrollBars.Horizontal )
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

			int newValue = this.Value;
			isThumbTracking = false;
			switch(saType) 
			{
				case ScrollEventType.First:
					newValue = this.Minimum;
					break;
				case ScrollEventType.Last:
					newValue = ((this.Maximum - this.LargeChange) + 1);
					break;
				case ScrollEventType.SmallDecrement:
					newValue = Math.Max((this.Value - this.SmallChange), this.Minimum);
					break;
				case ScrollEventType.SmallIncrement:
					newValue = Math.Min((this.Value + this.SmallChange), ((this.Maximum - this.LargeChange) + 1));
					break;
				case ScrollEventType.LargeDecrement:
					newValue = Math.Max((this.Value - this.LargeChange), this.Minimum);
					break;
				case ScrollEventType.LargeIncrement:
					newValue = Math.Min((this.Value + this.LargeChange), ((this.Maximum - this.LargeChange) + 1));
					break;

				case ScrollEventType.ThumbTrack:
//					if (!SupportsThumbTrack)
//						return;
//					else
						isThumbTracking = true;

					goto case ScrollEventType.ThumbPosition;

				case ScrollEventType.ThumbPosition: 
					si = new NativeMethods.SCROLLINFO();
					si.cbSize = Marshal.SizeOf(typeof(NativeMethods.SCROLLINFO));
					si.fMask = (int) SIF.ALL;
					NativeMethods.GetScrollInfo(this.reflectParent.Handle, this.ScrollInfoBar, ref si);

					if (this.RightToLeft == RightToLeft.Yes) 
						newValue = this.ReflectPosition(si.nTrackPos);
					else
						newValue = si.nTrackPos;
					break;
			}

            ScrollOrientation orientation = ScrollOrientation.HorizontalScroll;
            if (m.Msg == NativeMethods.WM_VSCROLL)
            {
                orientation = ScrollOrientation.VerticalScroll;
            }
            else if (m.Msg == NativeMethods.WM_HSCROLL)
            {
                orientation = ScrollOrientation.HorizontalScroll;
            }
            sa = new ScrollEventArgs(saType, newValue, orientation);
		   
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
#else
		{   
#endif
			this.OnScroll(sa);
			base.Value = Math.Max(Math.Min(sa.NewValue, Maximum), Minimum);
		}
		}

		/// <override/>
		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
		protected override void WndProc(ref Message m)  
		{
			//Debug.WriteLine(String.Format("Message {0:x4}", m.Msg));
			switch (m.Msg)
			{
#if CTLCOLOR
            case 0x0019/*WM_CTLCOLOR*/:
            case 0x2019/*WM_CTLCOLOR*/:
                WmReflectCtlColor(ref m);
                break;
#endif
				case 0x2114/*WM_HSCROLL*/:
				case 0x2115/*WM_VSCROLL*/:
				case 0x0114/*WM_HSCROLL*/:
				case 0x0115/*WM_VSCROLL*/:
					ReflectScrollMessage(ref m);
					break;

				default:
					base.WndProc(ref m);
					break;
			}
		}

		/// <override/>
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
		}

		protected new void UpdateScrollInfo()  
		{
			NativeMethods.SCROLLINFO si;
			if (this.IsHandleCreated && this.Enabled)
				base.UpdateScrollInfo();
			else if (this.reflectParent != null)
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
				NativeMethods.SetScrollInfo(this.reflectParent.Handle, this.ScrollInfoBar, ref si, true);

				/*
				if (si.nMax <= si.nMin)
				{
					NativeMethods.FlatSB_EnableScrollBar(reflectParent.Handle, .ScrollInfoBar, (int) ESB.DISABLE_BOTH);
				}*/
			}
		}

		int ScrollInfoBar
		{
			get
			{
				return this.scrollBarType == ScrollBars.Horizontal ? 0 : 1;
			}
		}

		/// <summary>
        /// Gets / sets a value to be added to or subtracted from to the Value property when the scroll box is moved a large distance. 
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
				if (base.LargeChange != value) 
				{
					if( value < 0 )
					{
						value = 0;
					}

					base.LargeChange = value;
					UpdateScrollInfo();
				}
			}
		}

		/// <summary>
        /// Gets / sets the upper limit of values of the scrollable range. 
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
				if (base.Maximum != value) 
				{
					base.Maximum = value;
					UpdateScrollInfo();
				}
			}
		}

		/// <summary>
        /// Gets / sets the lower limit of values of the scrollable range. 
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
				if (base.Minimum != value) 
				{
					base.Minimum = value;
					UpdateScrollInfo();
				}
			}
		}

    
		/// <summary>
        /// Gets / sets a value to be added to or subtracted from to the Value property when the scroll box is moved a small distance. 
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
				if (base.SmallChange != value)
				{
					base.SmallChange = value;
					this.UpdateScrollInfo();
				}
			}
		}
    
		/// <summary>
        /// Gets / sets a numeric value that represents the current position of the scroll box on the scroll bar control. 
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
				if (base.Value != value) 
				{
					base.Value = value;
					UpdateScrollInfo();
				}
			}
		}
	}
}
