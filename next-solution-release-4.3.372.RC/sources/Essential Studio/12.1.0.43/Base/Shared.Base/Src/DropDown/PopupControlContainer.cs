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
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;

using System.ComponentModel.Design.Serialization;
using System.CodeDom;
using Syncfusion.ComponentModel;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms;
using System.Reflection;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Provides information, whether popup is ignoring
	/// working area of the display before showing.
	/// This interface should be implemented by classes,
	/// that contain ot use popups.
	/// </summary>
	public interface IIgnoreWorkingArea
	{
		/// <summary>
		/// Indicates whether derived class ignores working area of the display before showing popup windows.
		/// </summary>
		bool IgnoreWorkingArea{ get; set; }
	}
	/// <summary>
	/// Provides a FindParentForm method that returns the parent form. Use this interface
	/// instead of Control.FindForm when you want to support nested windowless grid as used
	/// in GridGroupingControl. See also <see cref="FindFormHelper.FindForm"/> of the <see cref="FindFormHelper"/>
	/// class.
	/// </summary>
	public interface IFindParentForm
	{
		/// <summary>
		/// Returns the parent form of the control. Use this
		/// instead of Control.FindForm when you want to support nested windowless grid as used
		/// in GridGroupingControl. See also <see cref="FindFormHelper.FindForm"/> of the <see cref="FindFormHelper"/>
		/// class.
		/// </summary>
		/// <returns>The parent form.</returns>
		Form FindParentForm();
	}

	/// <summary>
	/// Provides a FindFormHelper.FindForm(Control) method that supports the IFindParentForm interface. Use this instead of calling
	/// Control.FindForm.
	/// </summary>
	public class FindFormHelper
	{
		/// <summary>
		/// Provides a FindFormHelper.FindForm(Control) method that you can use instead of calling
		/// Control.FindForm.
		/// </summary>
		public static Form FindForm(Control c)
		{
			if (c is IFindParentForm)
				return ((IFindParentForm) c).FindParentForm();
			return c.FindForm();
		}
	}

	/// <summary>
	/// Defines an interface that a <see cref="PopupHost"/> can take and parent itself to.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="PopupControlContainer"/> provides a default implementation that should be useful for all
	/// practical purposes.
	/// </para>
	/// </remarks>
	public interface IPopupControlContainer : IPopupChild
	{
		/// <summary>
		/// Gets / sets the <see cref="PopupHost"/> this container is parented to.
		/// </summary>
		/// <value>
		/// An instance of the <see cref="PopupHost"/> class.
		/// </value>
		/// <remarks>
		/// If this is a control, you would set it to be a child of
		/// the <see cref="PopupHost"/> and position it appropriately in the set property.
		/// </remarks>
		PopupHost PopupHost {get; set;}
		/// <summary>
		/// Provides the appropriate location to the popup given the alignment preferences.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method is usually called to determine whether the open popup should be closed
		/// due to some action in a different control. If this different control is a "related" control,
		/// the popup framework will not close the popup.
		/// </para>
		/// </remarks>
		Point GetPreferredLocation(PopupRelativeAlignment prevRelativeAlignment,
			out PopupRelativeAlignment newRelativeAlignment);
	}


	// Draws a shadow in its right and bottom sides, X pixels wide (within its bounds).
	[Syncfusion.Documentation.DocumentationExclude()]
	public class ShadowWindow : DropDownWindow
	{
		public static int ShadowWidth = 4;
		public static int ShadowHeight = 4;
		static ShadowWindow()
		{
			if(Environment.OSVersion.Platform != PlatformID.Win32NT ||
				Environment.OSVersion.Version.Major < 5)
			{
				ShadowWidth = 2;
				ShadowHeight = 2;
			}

		}
		public ShadowWindow()
			:base()
		{
			this.SuspendLayout();
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.Size = new Size(0,0);
			this.Location = new Point(-1000, -1000);

			this.AllowTransparency = true;
			this.Opacity = 0.2f;
			this.TransparencyKey = Color.LightGray;
			this.ResumeLayout(false);
			//this.BackColor = Color.FromArgb(20, Color.Red);
			//this.ShowWindowTopMost();
		}
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.TransparencyKey = Color.Empty;
        }

		internal Point[] borderOverlapLine;
		public Point[] BorderOverlapLine
		{
			get{return this.borderOverlapLine;}
			set
			{
				if(value == null || value.Length == 2)
					this.borderOverlapLine = value;
				else
					this.borderOverlapLine = null;
			}
		}

		protected virtual Rectangle GetRectToExclude()
		{
			if(borderOverlapLine != null)
			{
				Point[] clientLine = new Point[2];
				clientLine[0] = this.PointToClient(borderOverlapLine[0]);
				clientLine[1] = this.PointToClient(borderOverlapLine[1]);

				bool horizontal = false;
				if(clientLine[0].Y == clientLine[1].Y)
					horizontal = true;

				if(horizontal)
				{
					// Only if at the bottom border.
					if(clientLine[0].Y > this.Height - clientLine[0].Y)
					{
						return new Rectangle(clientLine[0].X, 0, clientLine[1].X - clientLine[0].X,
							this.Height);
					}
				}
				else
				{
					// Only if at the right border.
					if(clientLine[0].X > this.Width - clientLine[0].X)
					{
						return new Rectangle(0, clientLine[0].Y, this.Width,
							clientLine[1].Y - clientLine[0].Y);
					}
				}
			}

			return Rectangle.Empty;
		}

		/// <override/>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			// Clipping technique will not work since this is partly transparent,
			// using Region.Exclude instead.
			Rectangle rectExclude = this.GetRectToExclude();
			Region drawRegion = new Region(this.ClientRectangle);
			drawRegion.Exclude(rectExclude);

            // Draw the valid regions
            try
            {                
                e.Graphics.FillRegion(new SolidBrush(SystemColors.ControlDark), drawRegion);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
            }

			// then the region to exclude with the transparent color.
			e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), rectExclude);
		}
	}
	class PopupCloseRequestListener:
		IDisposable
	{
		private PopupControlContainer popup;
		private Control parentControl;
		private Form popupHost;
		protected internal int ignoreDeactivateTick = int.MinValue;

		public PopupCloseRequestListener(PopupControlContainer popup)
		{
			this.popup = popup;
		}

		public Form PopupHost
		{
			get{return this.popupHost;}
			set
			{
				if(this.popupHost != value)
				{
					if(this.popupHost != null)
						this.popupHost.Deactivate -= new EventHandler(this.FormDeactivate);

					this.popupHost = value;

					if(this.popupHost != null)
						this.popupHost.Deactivate += new EventHandler(this.FormDeactivate);
				}
			}
		}

		public Control ParentControl
		{
			get{return this.parentControl;}
			set
			{
				if(this.parentControl != value)
				{
					if(this.parentControl != null)
						Release();
					this.parentControl = value;
					if(this.parentControl != null)
						Attach();
				}
			}
		}
		private void Attach()
		{
			if(this.parentControl != null)
			{
				this.parentControl.LostFocus += new EventHandler(this.ParentLostFocus);
				Form parentForm = FindFormHelper.FindForm(this.parentControl);

				if (parentForm != null)
				{
					parentForm.Resize += new EventHandler(this.MenuCloseEvent);
					parentForm.Move += new EventHandler(this.MenuCloseEvent);
					parentForm.Deactivate += new EventHandler(this.FormDeactivate);
					if (parentForm.ParentForm != null)
					{
						parentForm.ParentForm.Move += new EventHandler(this.MenuCloseEvent);
						parentForm.ParentForm.Deactivate += new EventHandler(this.FormDeactivate);
					}
				}
			}
		}
		private void Release()
		{
			if(this.parentControl != null)
			{
				this.parentControl.LostFocus -= new EventHandler(this.ParentLostFocus);

				Form parentForm = FindFormHelper.FindForm(this.parentControl);

				if (parentForm != null)
				{
					parentForm.Resize -= new EventHandler(this.MenuCloseEvent);
					parentForm.Move -= new EventHandler(this.MenuCloseEvent);
					parentForm.Deactivate -= new EventHandler(this.FormDeactivate);
					if (parentForm.ParentForm != null)
					{
						parentForm.ParentForm.Move -= new EventHandler(this.MenuCloseEvent);
						parentForm.ParentForm.Deactivate -= new EventHandler(this.FormDeactivate);
					}
				}
			}
		}
		private void ParentLostFocus(object sender, EventArgs e)
		{
			Control curFocusControl = Control.FromHandle(NativeMethods.GetFocus());
			if(curFocusControl == null)
				curFocusControl = PopupUtils.GetADotNetParentControl(NativeMethods.GetFocus());
			if (this.popup != null)
			{
				if ((this.popup != curFocusControl && !this.popup.Contains(curFocusControl))
					&& curFocusControl != this.PopupHost
					&&
					(this.ParentControl == null ||
					(this.ParentControl != curFocusControl && !this.ParentControl.Contains(curFocusControl))
					)
					)
					this.popup.HidePopup(PopupCloseType.Deactivated);
			}
		}

		private void MenuCloseEvent(object sender, EventArgs e)
		{
			if( this.popup.IgnoreMouseMessages ) return;

			this.popup.HidePopup(PopupCloseType.Deactivated);
		}

		private void FormDeactivate(object sender, EventArgs e)
		{
		if (this.popup != null)
			{
				Form activeForm = Form.ActiveForm;
				if (!this.popup.IsShowing() ||
					this.ignoreDeactivateTick > Environment.TickCount ||
					activeForm == sender ||
					(activeForm != null &&
					this.popup.IsRelatedControl(activeForm, true)))
					return;
				else
					this.popup.HidePopup(PopupCloseType.Deactivated);
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Release();
			this.popup = null;
		}

		#endregion
	}

	/// <summary>
	/// A generic window that can be used for drop-down behavior, with WS_EX_TOOLWINDOW and CS_SAVEBITS styles.
	/// </summary>
	/// <remarks>
	/// Internally used by the PopupHost class.
	/// </remarks>
	public class DropDownWindow : TopLevelWindow
	{
		public DropDownWindow()
		{
			this.SuspendLayout();
			this.SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, false);
			this.SetStyle(ControlStyles.Opaque, true);
			this.SetStyle(ControlStyles.Selectable, false);
			this.TabStop = false;
			this.HScroll = false;
			this.VScroll = false;
			this.Visible = false;
			this.ResumeLayout(false);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override CreateParams CreateParams
		{
			[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
			get
			{
				CreateParams cp;
				cp = base.CreateParams;

				cp.ExStyle = (cp.ExStyle | 0x80/*WS_EX_TOOLWINDOW*/);

				// Otherwise has trouble createing window in NT4.0
				if(Environment.OSVersion.Platform != PlatformID.Win32NT ||
					Environment.OSVersion.Version.Major > 5)
					cp.ExStyle |= 0x08000000/*WS_EX_NOACTIVATE*/;

				cp.ClassStyle = cp.ClassStyle | 0x0800 /*CS_SAVEBITS*/;
				return cp;
			}
		}
	}

	/// <summary>
	/// The top level form-derived class that hosts a <see cref="PopupControlContainer"/>
	/// when it is dropped-down.
	/// </summary>
	/// <remarks>
	/// <para>You will normally not have to use this class or refer to
	/// an instance of this class. An instance of this class will
	/// be automatically generated by the <see cref="PopupControlContainer"/> which
	/// will then set this as its parent when <see cref="Syncfusion.Windows.Forms.PopupControlContainer.ShowPopup"/> is called on it.</para>
	/// <para>
	/// However, you can for example access an instance of this
	/// class from the PopupControlContainer and change certain properties.
	/// </para>
	/// <para>
	/// You can include a shadow in your popups by setting the NeedShadow property.
	/// </para>
	/// </remarks>
	/// <example>
	/// Take a look at PopupControlContainer.PopupHost property reference for an example
	/// on how to access and modify the PopupHost's properties during run-time.
	/// </example>
	public class PopupHost : DropDownWindow, IIgnoreWorkingArea
	{
		#region PRIVATE_MEMBERS
		internal int borderGap = 0;
		bool needShadow = false;
		internal ShadowWindow shadow = null;
		internal IPopupControlContainer popupControlContainer;
		internal Control popupControl;
		private PopupRelativeAlignment rAlign = PopupRelativeAlignment.Default;
		private bool aboutToShow = false;
		private Color overlapBorderColor = Color.Empty;
		#endregion PRIVATE_MEMBERS

		#region INIT
		/// <summary>
		/// Creates a new instance of the PopupHost class.
		/// </summary>
		public PopupHost() : base()
		{
			this.SetStyle(ControlStyles.Selectable, false);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void CreateShadow()
		{
			this.shadow = new ShadowWindow();
		}

		/// <summary>
		/// Overridden. See <see cref="M:System.Windows.Forms.Control.WndProc"/>.
		/// </summary>
		/// <param name="m"></param>
		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 0x21/*WM_MOUSEACTIVATE*/)
			{
				m.Result = (IntPtr)3; //MA_NOACTIVATE
				return;
			}

			if (m.Msg == NativeMethods.WM_CLOSE)
			{
				if(this.ControlBox == false)
					// Don't process close messages, unless the control box and borders are shown.
					return;
				else if(this.popupControlContainer != null)
				{
					// Process this if ControlBox is visible.
					this.popupControlContainer.HidePopup(PopupCloseType.Done);
					return;
				}
			}

            if( m.Msg == NativeMethods.WM_ACTIVATE )
            {
                if( ( int ) m.WParam == NativeMethods.WA_INACTIVE )
                {
                    // Comments: Fix for #7913 is reverted.
                    // this.popupControlContainer.HidePopup( PopupCloseType.Deactivated );
                }
            }

			base.WndProc(ref m);
		}
		#endregion INIT

		#region PROPERTIES

		[Syncfusion.Documentation.DocumentationExclude()]
		protected int BorderGap
		{
			get
			{
				return borderGap;
			}
			set
			{
				borderGap = value;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected ShadowWindow Shadow
		{
			get
			{
				return shadow;
			}
//			set
//			{
//				shadow = value;
//			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected Control PopupControl
		{
			get
			{
				return popupControl;
			}
//			set
//			{
//				popupControl = value;
//			}
		}

		/// <summary>
		/// Indicates whether a shadow should
		/// be drawn around the popup window.
		/// </summary>
		/// <value>True if a shadow is needed; False otherwise.</value>
		[DefaultValue(false)]
		public virtual bool NeedShadow
		{
			get
			{
				return needShadow;
			}
			set
			{
				if(this.needShadow != value)
				{
					this.needShadow = value;
					if(this.Visible && this.needShadow)
						UtilFuncs.SetVisibleNoActivate(this.shadow, true);
					else if(!this.Visible && !this.needShadow)
						UtilFuncs.SetVisibleNoActivate(this.shadow, false);
				}
			}
		}
		/// <summary>
		/// Gets / sets the PopupControlContainer that this PopupHost
		/// will host.
		/// </summary>
		/// <value>
		/// An implementation of the <see cref="IPopupControlContainer"/> interface.
		/// </value>
		public IPopupControlContainer PopupControlContainer
		{
			get{return this.popupControlContainer;}
			set
			{
				if(this.popupControlContainer != value)
				{
					UtilFuncs.SetVisibleNoActivate(this, false);

					this.DetachPopup();

					IPopupControlContainer oldContainer = this.popupControlContainer;

					this.popupControlContainer = value;
					this.popupControl = (Control)value;

					// Update circular reference in PopupControlContainer
					if(oldContainer != null)
						oldContainer.PopupHost = null;

					if(this.popupControlContainer != null &&
						this.popupControlContainer.PopupHost != this)
						this.popupControlContainer.PopupHost = this;

					this.AttachPopup();
				}
			}
		}
		/// <summary>
		/// Returns a <see cref="PopupRelativeAlignment"/> value indicating
		/// the current alignment of the popup window.
		/// </summary>
		/// <value>
		/// A <see cref="PopupRelativeAlignment"/> value.
		/// </value>
		public PopupRelativeAlignment CurrentRAlign
		{
			get{return this.rAlign;}
		}

		/// <summary>
		/// Gets / sets the overlap border color with which this top-level form should be drawn.
		/// </summary>
		/// <value>A color value.</value>
		/// <remarks>
		/// <para>This color will be used by this form only when the <see cref="FormBorderStyle"/> is set to None.</para>
		/// <para>The form will draw a custom single-line border with this color and also draw
		/// the overlapped look, if overlapped borders are available.</para>
		/// </remarks>
		public Color OverlapBorderColor
		{
			get{return this.overlapBorderColor;}
			set
			{
				if(this.overlapBorderColor != value)
				{
					this.overlapBorderColor = value;
					if(value != Color.Empty)
						this.borderGap = 2;
					else
						this.borderGap = 0;
				}
			}
		}
		#endregion PROPERTIES

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void DetachPopup()
		{
			if(this.popupControl != null)
			{
				this.Controls.Remove(this.popupControl);
				this.popupControl = null;
			}
			this.popupControlContainer = null;
		}

		[
		Syncfusion.Documentation.DocumentationExclude()
		]
		protected virtual void AttachPopup()
		{
			if (this.popupControl != null)
				this.Controls.Add(this.popupControl);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void ComputeMySize()
		{
            if (this.popupControl != null)
            {
                Size mySize = this.popupControl.Size;
                mySize += new Size(borderGap, borderGap);

                // Need to remove DockStyle.Fill before setting the new size,
                // or else the new size didn't get set in some cases!
                if (this.ClientSize != mySize)
                    this.ClientSize = mySize;
            }
		}

		public new void SuspendLayout()
		{
			base.SuspendLayout();
			if(this.NeedShadow && this.shadow != null)
				this.shadow.SuspendLayout();
		}

		/// <summary>
		/// Ignore working area when menu begin popup.
		/// </summary>
		private bool m_bIgnoreWorkingArea = false;

		/// <summary>
		/// Gets or sets ignore working area when menu begin popup.
		/// </summary>
		[ DefaultValue( false ) ]
		public bool IgnoreWorkingArea
		{
			get
			{
				return m_bIgnoreWorkingArea;
			}
			set
			{
				if( m_bIgnoreWorkingArea != value )
				{
					m_bIgnoreWorkingArea = value;
				}
			}
		}

		public new void ResumeLayout(bool preformLayout)
		{
			base.ResumeLayout(preformLayout);
			if(this.NeedShadow && this.shadow != null)
				this.shadow.ResumeLayout(preformLayout);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void ComputeControlLocation()
		{
			this.popupControl.Location = new Point(borderGap/2, borderGap/2); // Depends on borderGap
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void AdjustLocationForAlignment(ref Point loc, PopupRelativeAlignment align)
		{
			switch(align)
			{
				case PopupRelativeAlignment.BottomRight:
				case PopupRelativeAlignment.LeftTop:
					loc = loc - new Size(this.Width - 1, 0);
					break;
				case PopupRelativeAlignment.RightBottom:
				case PopupRelativeAlignment.TopLeft:
					loc = loc - new Size(0, this.Height - 1);
					break;
				case PopupRelativeAlignment.LeftBottom:
				case PopupRelativeAlignment.TopRight:
					loc = loc - new Size(this.Width - 1, this.Height - 1);
					break;
			}

		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void ComputeMyLocation()
		{
			// Determine the best alignment (with respect to the Parent borders, if any).
			Point prefLocation = Point.Empty, adjustedLocation = new Point(0, 0);
			PopupRelativeAlignment newAlign = PopupRelativeAlignment.Default;

			int i = 0;
			while(true)
			{
				if(++i > 10)
					break;
				prefLocation = this.popupControlContainer.GetPreferredLocation(newAlign, out newAlign);

				this.AdjustLocationForAlignment(ref prefLocation, newAlign);
				adjustedLocation = this.GetAdjustedLocation(prefLocation);
				if(newAlign == PopupRelativeAlignment.Default)
					break;
				else if(adjustedLocation == prefLocation)
					break;
			}

			this.rAlign = newAlign;

			// We have the preferred-adjusted location now.
			if(this.Location != adjustedLocation)
				this.Location = adjustedLocation;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual Point GetAdjustedLocation(Point loc)
		{
			// Trying to avoid calling Screen.GetWorkingArea as far as possible.
			// Calling the Screen method will sometime cause the drop-down menu to go blank!
			// Possibly because the method throws an exception?
			bool multiMonitor = NativeMethods.GetSystemMetrics(80/*SM_CMONITORS*/) > 1;
			Rectangle workingArea = Rectangle.Empty;
			
			if( this.IgnoreWorkingArea )
			{
				workingArea = Screen.GetBounds( loc );
			}
			else
			{
				if( multiMonitor )
				{
					workingArea = Screen.GetWorkingArea(loc);
				}
				else
				{
					workingArea = SystemInformation.WorkingArea;
				}
			}

			if((loc.Y + this.Height) >= workingArea.Bottom)
				loc.Y = workingArea.Bottom - this.Height;
			if(loc.Y < workingArea.Top)
				loc.Y = workingArea.Top;

			if((loc.X + this.Width) >= workingArea.Right)
				loc.X = workingArea.Right - this.Width;
			if(loc.X < workingArea.Left)
				loc.X = workingArea.Left;

			return loc;
		}

		/// <summary>
		/// Overridden. See <see cref="M:System.Windows.Forms.Control.OnVisibleChanged"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			// Change the shadow window's state appropriately.
			if(this.Visible)
			{
				if(this.NeedShadow && this.shadow != null)
				{
					UtilFuncs.SetVisibleNoActivate(this.shadow, true);
				}
			}
			else
			{
				if(this.shadow != null)
					UtilFuncs.SetVisibleNoActivate(this.shadow, false);
			}
		}
		public bool IsShowing()
		{
			return this.Visible || aboutToShow;
			
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected override void OnLocationChanged(EventArgs e)
		{
			ComputeShadowLocation();

			base.OnLocationChanged(e);
		}

		private void ComputeShadowLocation()
		{
			if( this.NeedShadow && this.shadow != null )
			{
				Point loc = this.Location;
				int nShadowWidth = ShadowWindow.ShadowWidth;

				loc.Offset( GetIsMirrored() ? -nShadowWidth : nShadowWidth, ShadowWindow.ShadowHeight );

				this.shadow.Location = loc;
			}
		}

		/// <summary>
		/// Overridden. See <see cref="M:System.Windows.Forms.Control.OnSizeChanged"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSizeChanged(EventArgs e)
		{
			ComputeShadowSize();
			
			base.OnSizeChanged(e);
		}

		private void ComputeShadowSize()
		{
			if( this.NeedShadow && this.shadow != null )
			{
				this.shadow.Size = this.Size;
			}
		}

		/// <summary>
		/// Overridden. See <see cref="M:System.Windows.Forms.Control.OnPaint"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if(this.OverlapBorderColor != Color.Empty && this.FormBorderStyle == FormBorderStyle.None)
			{
				Rectangle border = this.ClientRectangle;
				border.Width -= 1;
				border.Height -= 1;
				e.Graphics.DrawRectangle(new Pen(this.OverlapBorderColor), border);
				if(this.PopupControlContainer.PopupParent != null)
				{
					Point[] borderCue = this.PopupControlContainer.PopupParent.GetBorderOverlapCue(this.CurrentRAlign);
					if(borderCue != null)
					{
						Point left = this.PointToClient(borderCue[0]);
						Point right = this.PointToClient(borderCue[1]);

						e.Graphics.DrawLine(SystemPens.Control, left, right);
					}
				}
			}
		}

		/// <summary>
		/// Shows the popup.
		/// </summary>
		public virtual void ShowPopup()
		{
			if(this.popupControl != null)
			{
                if (this.IsShowing())
                {
                    return;
                }
                else
                {
                    // Need the aboutToShow flag to enable Pumping the Paint messages (which might call into one of the parents)
                    this.aboutToShow = true;
                    this.PumpPaintMessages();
                }

                this.shadow = null;
                if (this.NeedShadow && this.shadow == null) 
					this.CreateShadow();

                ComputeLayout();
                UpdateVisibility();

				this.aboutToShow = false;
			}
		}

		/// <summary>
		/// Updates popup host visibility and z-order.
		/// </summary>
		protected void UpdateVisibility()
		{
			if( this.NeedShadow && this.shadow != null )
				UtilFuncs.SetVisibleNoActivate( this.shadow, true );

			if( !this.Visible )
				UtilFuncs.SetVisibleNoActivate( this, true );
			else
				this.Invalidate( false );

			if( !this.popupControl.Visible )
				UtilFuncs.SetVisibleNoActivate( this.popupControl, true );

			// So that this will stay above shadow window.
			UtilFuncs.ShowWindowTopMost( this.Handle, this.Location, this.Size );
		}

		/// <summary>
		/// Computes size and position of popup.
		/// </summary>
		protected internal void ComputeLayout()
		{
			this.SuspendLayout();

			this.ComputeMySize();
			this.ComputeControlLocation();
			this.ComputeMyLocation();

			this.ResumeLayout( true );

			if( this.NeedShadow )
			{
				ComputeShadowLocation();
				ComputeShadowSize();
			}
		}

		/// <summary>
		/// Hides the popup.
		/// </summary>
		public virtual void HidePopup()
		{
			this.Visible = false;
			this.Location = new Point(-1000, -1000);
            this.ComputeMySize();

			this.PumpPaintMessages();
            if(this.shadow != null)
            {
                this.shadow.Dispose();
                this.shadow = null;
            }
		}
		// Necessary since CS_SAVEBITS for this top-level is not good-enough to ensure that the window
		// underneath is refreshed. Important when navigating menu using keyboard.
		private void PumpPaintMessages()
		{
			NativeMethods.MSG msg = new NativeMethods.MSG();
			int n = 0;
			// Don't loop more than 25 times. (This used to be 10, customer recommended to make this 25).
			while(n < 25 && NativeMethods.PeekMessage(ref msg,IntPtr.Zero,0x000F/*WM_PAINT*/,0x000F/*WM_PAINT*/,1))
			{
				n++;
				NativeMethods.TranslateMessage(ref msg);
				NativeMethods.DispatchMessage(ref msg);
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override void OnDeactivate(EventArgs e)
		{
			base.OnDeactivate(e);

			// Close popup if focus moved to some other window.
			IntPtr focusedWindow = NativeMethods.GetFocus();
			if(focusedWindow != IntPtr.Zero)
			{
				Control focusedControl = Control.FromHandle(focusedWindow);
				if(focusedControl == null)
					focusedControl = PopupUtils.GetADotNetParentControl(focusedWindow);
				if(popupControlContainer != null && (focusedControl == null || !this.popupControlContainer.IsRelatedControl(focusedControl, true)))
					this.popupControlContainer.HidePopup(PopupCloseType.Deactivated);
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool GetIsMirrored()
		{
			return RightToLeft.Yes == RightToLeft;
		}

        protected override void Dispose(bool disposing)
		{
			if( disposing )
			{
				if( this.shadow != null )
				{
					this.shadow.Dispose();
					this.shadow = null;
				}
                
                popupControlContainer = null ;
                popupControl = null;
			}

			base.Dispose( disposing );
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			Init();
		}

		private void Init()
		{
			if( this.shadow == null )
			{
				this.CreateShadow();

				// Otherwise the shadow window causes a flicker when the menu is shown the first time.
				// Make sure not to activate the window while we do this.
				UtilFuncs.SetVisibleNoActivate( this.shadow, this.needShadow );

				if( !this.Visible && !this.needShadow )
				{
					UtilFuncs.SetVisibleNoActivate( this.shadow, false );
				}
			}
		}
	}

	public class PopupControlContainerSerializer: CodeDomSerializer
	{
		public override object Serialize(IDesignerSerializationManager manager, object value)
		{
			CodeDomSerializer serializer = 
				manager.GetSerializer( typeof( PopupControlContainer ).BaseType, typeof( CodeDomSerializer ) ) as CodeDomSerializer;
            
			object serializedObj = null;

			if( serializer != null )
			{
				serializedObj = serializer.Serialize( manager, value );
                
				CodeStatementCollection statements = serializedObj as CodeStatementCollection;
				if( statements != null && statements.Count > 0 )
				{
					CodeAssignStatement statement = statements[ 0 ] as CodeAssignStatement;
					if( statement != null )
					{
						// If the first one is the constructor, 'clear' it, if it's not default.
						CodeObjectCreateExpression createExpr = statement.Right as CodeObjectCreateExpression;
						if( createExpr != null )
						{
							createExpr.Parameters.Clear();
						}
					}
				}
			}

			return serializedObj;
		}

		public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
		{
			CodeDomSerializer serializer = 
				manager.GetSerializer( typeof( PopupControlContainer ).BaseType, typeof( CodeDomSerializer ) ) as CodeDomSerializer;

			object deserializedObj = null;

			if( serializer != null )
			{
				deserializedObj = serializer.Deserialize( manager, codeObject );
			}

			return deserializedObj;
		}

	}

	/// <summary>
	/// A panel-derived class that will let you design custom popups
	/// within a form's designer.
	/// </summary>
	/// <remarks>
	/// <para>To design a custom popup, drag-and-drop it off the toolbox
	/// into a form during design-time. Then populate it with
	/// appropriate controls just like you would any other panel.
	/// You can mark it as invisible (Visible = false) so that it
	/// will not interfere with the form's layout mechanism. It's also recommended that
	/// you unparent it from the design-time parent in your form constructor code, as shown in the sample code below.</para>
	/// <para>
	/// When you are ready to popup, call this class's <see cref="ShowPopup"/> method. This will show
	/// the popup at the specified location.
	/// </para>
	/// <para>
	/// There are different configurations in which you can use this
	/// PopupControlContainer.
	/// <list type="bullet">
	/// <item><description>When you specify a Parent Control using
	/// <see cref="ParentControl"/> and pass a Point.Empty location to
	/// <see cref="ShowPopup"/>, the popup location will be dynamically determined
	/// based on the <see cref="ParentControl"/>'s bounds and the screen area.</description>
	/// </item>
	/// <item>
	/// For even more control over the alignment and positioning
	/// of the popup, you should implement an <see cref="IPopupParent"/> interface
	/// and assign that to the <see cref="PopupParent"/> property (this you would
	/// do instead of the above PopupControl-based alignment.).
	/// </item>
	/// </list>
	/// </para>
	/// <para>When the popup is showing, the PopupControlContainer,
	/// in the ProcessDialogKey override will look for Alt, Enter, Tab, Esc, F4,
	/// and F2 keys and either cancel or close the popup. If you want
	/// to prevent this, then set <see cref="IgnoreDialogKey"/> to False. You should
	/// then make sure to close the popup manually whenever appropriate using
	/// <see cref="HidePopup(PopupCloseType)"/>. </para>
	/// <para>The PopupControlContainer also fires the <see cref="BeforePopup"/>, <see cref="Popup"/>(after popup)
	/// and <see cref="CloseUp"/> events that you can handle. You could for example
	/// set the focus on a control within the PopupControlContainer in the
	/// popup event handler.</para>
	/// </remarks>
	/// <example>
	/// The following example has a PopupControlContainer that gets shown when the
	/// user does an Alt+DownArrow on a text box.
	/// <coderef file="Tools\Samples\Editors Package\PopupControlContainer\PopupControlContainerDemo\cs\Form1.cs" name="PopupControlContainer Initialization" lang="C#"><code lang="C#">
	///			public Form1()
	///			{
	///				InitializeComponent();
	///				// It's recommended that you keep the PopupControlContainer unparented by
	///				// any control on the form (which will be the default case as set up during design-time).
	///				// Otherwise, the Form won't close sometimes after dropping down this popup!
	///				this.popupControlContainer1.Parent.Controls.Remove(this.popupControlContainer1);
	///			}
	///         #region Windows Form Designer generated code
	///         private void InitializeComponent()
	///         {
	///             System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form1));
	///             this.popupControlContainer1 = new Syncfusion.Windows.Forms.PopupControlContainer();
	///             this.cancel = new System.Windows.Forms.Button();
	///             this.OK = new System.Windows.Forms.Button();
	///             this.popupTextBox = new System.Windows.Forms.TextBox();
	///             this.label1 = new System.Windows.Forms.Label();
	///             this.sourceTextBox = new System.Windows.Forms.TextBox();
	///             this.groupBox1 = new System.Windows.Forms.GroupBox();
	///             this.dropDownBtn = new System.Windows.Forms.Button();
	///             this.mainMenu1 = new System.Windows.Forms.MainMenu();
	///             this.menuItem1 = new System.Windows.Forms.MenuItem();
	///             this.menuItem2 = new System.Windows.Forms.MenuItem();
	///             this.popupControlContainer1.SuspendLayout();
	///             this.groupBox1.SuspendLayout();
	///             this.SuspendLayout();
	///             //
	///             // popupControlContainer1
	///             //
	///             this.popupControlContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
	///             this.popupControlContainer1.Controls.AddRange(new System.Windows.Forms.Control[] {
	///                                                                                                  this.cancel,
	///                                                                                                  this.OK,
	///                                                                                                  this.popupTextBox});
	///             this.popupControlContainer1.Location = new System.Drawing.Point(80, 128);
	///             this.popupControlContainer1.Name = "popupControlContainer1";
	///             this.popupControlContainer1.Size = new System.Drawing.Size(120, 128);
	///             this.popupControlContainer1.TabIndex = 0;
	///             this.popupControlContainer1.Visible = false;
	///             this.popupControlContainer1.Popup += new System.EventHandler(this.popupControlContainer1_Popup);
	///             this.popupControlContainer1.CloseUp += new Syncfusion.Windows.Forms.PopupClosedEventHandler(this.popupControlContainer1_CloseUp);
	///             this.popupControlContainer1.BeforePopup += new System.ComponentModel.CancelEventHandler(this.popupControlContainer1_BeforePopup);
	///             //
	///             // cancel
	///             //
	///             this.cancel.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
	///                 | System.Windows.Forms.AnchorStyles.Left)
	///                 | System.Windows.Forms.AnchorStyles.Right);
	///             this.cancel.Location = new System.Drawing.Point(64, 96);
	///             this.cancel.Name = "cancel";
	///             this.cancel.Size = new System.Drawing.Size(48, 24);
	///             this.cancel.TabIndex = 2;
	///             this.cancel.Text = "Cancel";
	///             this.cancel.Click += new System.EventHandler(this.cancelButton_Click);
	///             //
	///             // OK
	///             //
	///             this.OK.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
	///                 | System.Windows.Forms.AnchorStyles.Left)
	///                 | System.Windows.Forms.AnchorStyles.Right);
	///             this.OK.Location = new System.Drawing.Point(8, 96);
	///             this.OK.Name = "OK";
	///             this.OK.Size = new System.Drawing.Size(48, 24);
	///             this.OK.TabIndex = 1;
	///             this.OK.Text = "OK";
	///             this.OK.Click += new System.EventHandler(this.OK_Click);
	///             //
	///             // popupTextBox
	///             //
	///             this.popupTextBox.Multiline = true;
	///             this.popupTextBox.Name = "popupTextBox";
	///             this.popupTextBox.Size = new System.Drawing.Size(118, 90);
	///             this.popupTextBox.TabIndex = 0;
	///             this.popupTextBox.Text = "";
	///             //
	///             // label1
	///             //
	///             this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
	///             this.label1.Location = new System.Drawing.Point(16, 56);
	///             this.label1.Name = "label1";
	///             this.label1.Size = new System.Drawing.Size(256, 64);
	///             this.label1.TabIndex = 1;
	///             this.label1.Text = "Associate a PopupControlContainer with this TextBox. And also transfer data back " +
	///                 "and forth between the popup and the TextBox.";
	///             //
	///             // sourceTextBox
	///             //
	///             this.sourceTextBox.Location = new System.Drawing.Point(40, 128);
	///             this.sourceTextBox.Name = "sourceTextBox";
	///             this.sourceTextBox.Size = new System.Drawing.Size(200, 20);
	///             this.sourceTextBox.TabIndex = 2;
	///             this.sourceTextBox.Text = "Alt+DownArrow for popup";
	///             this.sourceTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxPopupParent_KeyDown);
	///             //
	///             // groupBox1
	///             //
	///             this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
	///                                                                                     this.dropDownBtn});
	///             this.groupBox1.Location = new System.Drawing.Point(8, 32);
	///             this.groupBox1.Name = "groupBox1";
	///             this.groupBox1.Size = new System.Drawing.Size(280, 128);
	///             this.groupBox1.TabIndex = 3;
	///             this.groupBox1.TabStop = false;
	///             this.groupBox1.Text = "PopupControlContainer demo";
	///             //
	///             // dropDownBtn
	///             //
	///             this.dropDownBtn.Image = ((System.Drawing.Bitmap)(resources.GetObject("dropDownBtn.Image")));
	///             this.dropDownBtn.Location = new System.Drawing.Point(240, 96);
	///             this.dropDownBtn.Name = "dropDownBtn";
	///             this.dropDownBtn.Size = new System.Drawing.Size(26, 20);
	///             this.dropDownBtn.TabIndex = 0;
	///             this.dropDownBtn.Click += new System.EventHandler(this.dropDownBtn_Click);
	///             //
	///             // mainMenu1
	///             //
	///             this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
	///                                                                                       this.menuItem1});
	///             //
	///             // menuItem1
	///             //
	///             this.menuItem1.Index = 0;
	///             this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
	///                                                                                       this.menuItem2});
	///             this.menuItem1.Text = "Help";
	///             //
	///             // menuItem2
	///             //
	///             this.menuItem2.Index = 0;
	///             this.menuItem2.Text = "About Syncfusion";
	///             this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
	///             //
	///             // Form1
	///             //
	///             this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
	///             this.ClientSize = new System.Drawing.Size(292, 273);
	///             this.Controls.AddRange(new System.Windows.Forms.Control[] {
	///                                                                           this.sourceTextBox,
	///                                                                           this.label1,
	///                                                                           this.popupControlContainer1,
	///                                                                           this.groupBox1});
	///             this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
	///             this.Menu = this.mainMenu1;
	///             this.Name = "Form1";
	///             this.Text = "Custom Popups Dialog";
	///             this.popupControlContainer1.ResumeLayout(false);
	///             this.groupBox1.ResumeLayout(false);
	///             this.ResumeLayout(false);
	///
	///         }
	///         #endregion
	///
	///         [STAThread]
	///         public static void Main()
	///         {
	///             Application.Run(new Form1());
	///         }
	///
	///         #region OpenClosePopup
	///         private void textBoxPopupParent_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
	///         {
	///             // Using this unconventional if statement syntax to avoid "and" symbol (documentation restriction, please ignore).
	///
	///             // If user pressed key down, then show the popup.
	///             if(e.Alt)
	///                 if(e.KeyCode == Keys.Down)
	///                     if(!this.popupControlContainer1.IsShowing())
	///                     {
	///                         // Let the popup align around the source textBox.
	///                         this.popupControlContainer1.ParentControl = this.sourceTextBox;
	///                         // Passing Point.Empty will align it automatically around the above ParentControl.
	///                         this.popupControlContainer1.ShowPopup(Point.Empty);
	///
	///                         e.Handled = true;
	///                     }
	///             // Escape should close the popup.
	///             if(e.KeyCode == Keys.Escape)
	///                 if(this.popupControlContainer1.IsShowing())
	///                     this.popupControlContainer1.HidePopup(PopupCloseType.Canceled);
	///         }
	///
	///         private void OK_Click(object sender, System.EventArgs e)
	///         {
	///             this.popupControlContainer1.HidePopup(PopupCloseType.Done);
	///         }
	///
	///         private void cancelButton_Click(object sender, System.EventArgs e)
	///         {
	///             this.popupControlContainer1.HidePopup(PopupCloseType.Canceled);
	///         }
	///         #endregion OpenClosePopup
	///
	///         #region PopupEvents
	///         private void popupControlContainer1_BeforePopup(object sender, System.ComponentModel.CancelEventArgs e)
	///         {
	///             // Set the text to be edited with the text in the form text box.
	///             this.popupTextBox.Text = this.sourceTextBox.Text;
	///         }
	///
	///         private void popupControlContainer1_Popup(object sender, System.EventArgs e)
	///         {
	///             // Set the focus on the text box inside the popup after it is open.
	///             this.popupTextBox.Focus();
	///             this.popupTextBox.SelectionStart = 0;
	///             this.popupTextBox.SelectionLength = 0;
	///         }
	///
	///         private void popupControlContainer1_CloseUp(object sender, Syncfusion.Windows.Forms.PopupClosedEventArgs args)
	///         {
	///             // Transfer data from the popup.
	///             if(args.PopupCloseType == PopupCloseType.Done)
	///             {
	///                 this.sourceTextBox.Text = this.popupTextBox.Text;
	///             }
	///             // Set focus back to textbox.
	///             if(args.PopupCloseType == PopupCloseType.Done
	///                 || args.PopupCloseType == PopupCloseType.Canceled)
	///                 this.sourceTextBox.Focus();
	///         }
	///         #endregion PopupEvents</code></coderef>
	/// <coderef file="Tools\Samples\Editors Package\PopupControlContainer\PopupControlContainerDemo\vb\Form1.vb" name="PopupControlContainer Initialization" lang="VB"><code lang="VB">
	///			Public Sub New()
	///				MyBase.New()
	///				InitializeComponent()
	///				' It's recommended that you keep the PopupControlContainer unparented by
	///				' any Control on the Form (which will be the default case as set up during design-time).
	///				' Otherwise, the Form wouldn't close sometimes, after dropping down this popup!
	///				Me.popupControlContainer1.Parent.Controls.Remove(Me.popupControlContainer1)
	///			End Sub
	///
	///        Private Sub InitializeComponent()
	///
	///            Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(Form1))
	///            Me.popupControlContainer1 = New Syncfusion.Windows.Forms.PopupControlContainer()
	///            Me.cancel = New System.Windows.Forms.Button()
	///            Me.OK = New System.Windows.Forms.Button()
	///            Me.popupTextBox = New System.Windows.Forms.TextBox()
	///            Me.label1 = New System.Windows.Forms.Label()
	///            Me.sourceTextBox = New System.Windows.Forms.TextBox()
	///            Me.groupBox1 = New System.Windows.Forms.GroupBox()
	///            Me.dropDownBtn = New System.Windows.Forms.Button()
	///            Me.mainMenu1 = New System.Windows.Forms.MainMenu()
	///            Me.menuItem1 = New System.Windows.Forms.MenuItem()
	///            Me.menuItem2 = New System.Windows.Forms.MenuItem()
	///            Me.popupControlContainer1.SuspendLayout()
	///            Me.groupBox1.SuspendLayout()
	///            Me.SuspendLayout()
	///            '
	///            ' popupControlContainer1
	///            '
	///            Me.popupControlContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
	///            Me.popupControlContainer1.Controls.AddRange(New System.Windows.Forms.Control() {Me.cancel, Me.OK, Me.popupTextBox})
	///            Me.popupControlContainer1.Location = New System.Drawing.Point(80, 128)
	///            Me.popupControlContainer1.Name = "popupControlContainer1"
	///            Me.popupControlContainer1.Size = New System.Drawing.Size(120, 128)
	///            Me.popupControlContainer1.TabIndex = 0
	///            Me.popupControlContainer1.Visible = False
	///            AddHandler Me.popupControlContainer1.Popup, New System.EventHandler(AddressOf popupControlContainer1_Popup)
	///            AddHandler Me.popupControlContainer1.CloseUp, New Syncfusion.Windows.Forms.PopupClosedEventHandler(AddressOf popupControlContainer1_CloseUp)
	///            AddHandler Me.popupControlContainer1.BeforePopup, New System.ComponentModel.CancelEventHandler(AddressOf popupControlContainer1_BeforePopup)
	///            '
	///            ' cancel
	///            '
	///            Me.cancel.Anchor = (((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
	///                        Or System.Windows.Forms.AnchorStyles.Left) _
	///                        Or System.Windows.Forms.AnchorStyles.Right)
	///            Me.cancel.Location = New System.Drawing.Point(64, 96)
	///            Me.cancel.Name = "cancel"
	///            Me.cancel.Size = New System.Drawing.Size(48, 24)
	///            Me.cancel.TabIndex = 2
	///            Me.cancel.Text = "Cancel"
	///            AddHandler Me.cancel.Click, New System.EventHandler(AddressOf cancelButton_Click)
	///            '
	///            ' OK
	///            '
	///            Me.OK.Anchor = (((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
	///                        Or System.Windows.Forms.AnchorStyles.Left) _
	///                        Or System.Windows.Forms.AnchorStyles.Right)
	///            Me.OK.Location = New System.Drawing.Point(8, 96)
	///            Me.OK.Name = "OK"
	///            Me.OK.Size = New System.Drawing.Size(48, 24)
	///            Me.OK.TabIndex = 1
	///            Me.OK.Text = "OK"
	///            AddHandler Me.OK.Click, New System.EventHandler(AddressOf OK_Click)
	///            '
	///            ' popupTextBox
	///            '
	///            Me.popupTextBox.Multiline = True
	///            Me.popupTextBox.Name = "popupTextBox"
	///            Me.popupTextBox.Size = New System.Drawing.Size(118, 90)
	///            Me.popupTextBox.TabIndex = 0
	///            Me.popupTextBox.Text = ""
	///            '
	///            ' label1
	///            '
	///            Me.label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
	///            Me.label1.Location = New System.Drawing.Point(16, 56)
	///            Me.label1.Name = "label1"
	///            Me.label1.Size = New System.Drawing.Size(256, 64)
	///            Me.label1.TabIndex = 1
	///            Me.label1.Text = ("Associate a PopupControlContainer with this TextBox. And also transfer data back " + "and forth between the popup and the TextBox.")
	///            '
	///            ' sourceTextBox
	///            '
	///            Me.sourceTextBox.Location = New System.Drawing.Point(40, 128)
	///            Me.sourceTextBox.Name = "sourceTextBox"
	///            Me.sourceTextBox.Size = New System.Drawing.Size(200, 20)
	///            Me.sourceTextBox.TabIndex = 2
	///            Me.sourceTextBox.Text = "Alt+DownArrow for popup"
	///            AddHandler Me.sourceTextBox.KeyDown, New System.Windows.Forms.KeyEventHandler(AddressOf textBoxPopupParent_KeyDown)
	///            '
	///            ' groupBox1
	///            '
	///            Me.groupBox1.Controls.AddRange(New System.Windows.Forms.Control() {Me.dropDownBtn})
	///            Me.groupBox1.Location = New System.Drawing.Point(8, 32)
	///            Me.groupBox1.Name = "groupBox1"
	///            Me.groupBox1.Size = New System.Drawing.Size(280, 128)
	///            Me.groupBox1.TabIndex = 3
	///            Me.groupBox1.TabStop = False
	///            Me.groupBox1.Text = "PopupControlContainer demo"
	///            '
	///            ' dropDownBtn
	///            '
	///            Me.dropDownBtn.Image = CType(resources.GetObject("dropDownBtn.Image"), System.Drawing.Bitmap)
	///            Me.dropDownBtn.Location = New System.Drawing.Point(240, 96)
	///            Me.dropDownBtn.Name = "dropDownBtn"
	///            Me.dropDownBtn.Size = New System.Drawing.Size(26, 20)
	///            Me.dropDownBtn.TabIndex = 0
	///            AddHandler Me.dropDownBtn.Click, New System.EventHandler(AddressOf dropDownBtn_Click)
	///            '
	///            ' mainMenu1
	///            '
	///            Me.mainMenu1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuItem1})
	///            '
	///            ' menuItem1
	///            '
	///            Me.menuItem1.Index = 0
	///            Me.menuItem1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuItem2})
	///            Me.menuItem1.Text = "Help"
	///            '
	///            ' menuItem2
	///            '
	///            Me.menuItem2.Index = 0
	///            Me.menuItem2.Text = "About Syncfusion"
	///            AddHandler Me.menuItem2.Click, New System.EventHandler(AddressOf menuItem2_Click)
	///            '
	///            ' Form1
	///            '
	///            Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
	///            Me.ClientSize = New System.Drawing.Size(292, 273)
	///            Me.Controls.AddRange(New System.Windows.Forms.Control() {Me.sourceTextBox, Me.label1, Me.popupControlContainer1, Me.groupBox1})
	///            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
	///            Me.Menu = Me.mainMenu1
	///            Me.Name = "Form1"
	///            Me.Text = "Custom Popups Dialog"
	///            Me.popupControlContainer1.ResumeLayout(False)
	///            Me.groupBox1.ResumeLayout(False)
	///            Me.ResumeLayout(False)
	///
	///        End Sub
	///
	///        Private Sub textBoxPopupParent_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)
	///
	///            ' Using this unconventional if statement syntax to avoid "and" symbol (documentation restriction, please ignore).
	///            ' If user pressed key down, then show the popup.
	///            ' Escape should close the popup.
	///            If e.Alt Then
	///                If (e.KeyCode = Keys.Down) Then
	///                    If Not (Me.popupControlContainer1.IsShowing) Then
	///                        ' Let the popup align around the source textBox.
	///                        Me.popupControlContainer1.ParentControl = Me.sourceTextBox
	///                        ' Passing Point.Empty will align it automatically around the above ParentControl.
	///                        Me.popupControlContainer1.ShowPopup(Point.Empty)
	///                        e.Handled = True
	///                    End If
	///                End If
	///            End If
	///            If (e.KeyCode = Keys.Escape) Then
	///                If Me.popupControlContainer1.IsShowing Then
	///                    Me.popupControlContainer1.HidePopup(PopupCloseType.Canceled)
	///                End If
	///            End If
	///
	///        End Sub
	///        Private Sub OK_Click(ByVal sender As Object, ByVal e As EventArgs)
	///
	///            Me.popupControlContainer1.HidePopup(PopupCloseType.Done)
	///
	///        End Sub
	///        Private Sub cancelButton_Click(ByVal sender As Object, ByVal e As EventArgs)
	///
	///            Me.popupControlContainer1.HidePopup(PopupCloseType.Canceled)
	///
	///        End Sub
	///        Private Sub popupControlContainer1_BeforePopup(ByVal sender As Object, ByVal e As CancelEventArgs)
	///
	///            ' Set the text to be edited with the text in the form text box.
	///            Me.popupTextBox.Text = Me.sourceTextBox.Text
	///
	///        End Sub
	///        Private Sub popupControlContainer1_Popup(ByVal sender As Object, ByVal e As EventArgs)
	///
	///            ' Set the focus on the text box inside the popup after its open.
	///            Me.popupTextBox.Focus()
	///            Me.popupTextBox.SelectionStart = 0
	///            Me.popupTextBox.SelectionLength = 0
	///
	///        End Sub
	///        Private Sub popupControlContainer1_CloseUp(ByVal sender As Object, ByVal args As PopupClosedEventArgs)
	///
	///            ' Transfer data from the popup.
	///            ' Set focus back to textbox.
	///            If (args.PopupCloseType = PopupCloseType.Done) Then
	///                Me.sourceTextBox.Text = Me.popupTextBox.Text
	///            End If
	///            If ((args.PopupCloseType = PopupCloseType.Done) _
	///                        OrElse (args.PopupCloseType = PopupCloseType.Canceled)) Then
	///                Me.sourceTextBox.Focus()
	///            End If
	///
	///        End Sub</code></coderef>
	/// </example>
	[System.Drawing.ToolboxBitmap(typeof(Syncfusion.Windows.Forms.PopupControlContainer), "ToolboxIcons.PopupControlContainer.bmp")]
	[ DesignerSerializer( typeof( PopupControlContainerSerializer ), typeof( CodeDomSerializer ) ) ] 
	[Description("A panel-derived class that will let you design custom popups within a designer.")]
	public class PopupControlContainer : Panel, IPopupControlContainer, IPopupParent
	{
		#region PRIVATE
		private Control parentControl;
		private IPopupParent popupParent;
		private Point discreetLocation;
		private PopupHost popupHost;
		private PopupCloseRequestListener listener;
		private MouseProcHookerUtil mouseHooker = null;
		private bool fakeFocus = false;
		internal bool bIgnoreDialogKey = false;
		private bool bIgnoreKeys = false;
		private bool bIgnoreMouseMessages = false;
		private IPopupChild currentPopupChild = null;
        private bool m_bNeedDisposeHost = true;
		private FormWindowSubclss _formSubclass;
        private bool m_bCloseOnTab = true;
        private Form parentForm = null;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);


		internal MouseProcHookerUtil MouseHooker
		{
			get
			{
				return mouseHooker;
			}
			set
			{
				mouseHooker = value;
			}
		}
		#endregion PRIVATE

		#region PROPERTIES
		/// <summary>
		/// Indicates whether the control is in design-mode.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool DesignMode
		{
			get{return base.DesignMode;}
		}

		/// <override/>
		protected override Size DefaultSize
		{
			get
			{
				return ((Size)(new Size(200, 100)));
			}
		}

		protected override Padding DefaultMargin
		{
			get
			{
				return new Padding(0);
			}
		}

		/// <summary>
		/// Indicates whether the popup should send a kill focus message
		/// to the control with focus when the popup was shown.
		/// </summary>
		/// <value>True to send a KillFocus message; False otherwise. Default value
		/// is False.</value>
		/// <remarks>
		/// <para>Faking focus is only necessary when the <see cref="PopupControlContainer"/>
		/// doesn't take the focus but you want it to look like it
		/// took the focus.</para>
		/// <para>
		/// When this property is True, a KillFocus message will be
		/// sent to the control that currently has the focus after the
		/// PopupControlContainer is dropped down and a SetFocus message
		/// will be sent to the control with focus when the popup is
		/// closed.
		/// </para>
		/// <para>
		/// Again, you will probably not need this functionality in
		/// most of the cases, since you will probably set the focus
		/// on some control with this PopupControlContainer as soon
		/// as it pops up (from within the popup event handler).
		/// </para>
		/// </remarks>
		[
		DefaultValue(false),
		Browsable(false),
		Description("Specifies whether the popup should send a KillFocus message to the control with focus when the popup was shown.")
		]
		public bool FakeFocus
		{
			get{return this.fakeFocus;}
			set{this.fakeFocus = value;}
		}

		/// <summary>
		/// Ensures that the <see cref="PopupHost"/> property returns a valid PopupHost.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Normally the <see cref="PopupHost"/> property will return a non-NULL value only when
		/// the popup was displayed at least once. Calling this method will ensure that the property
		/// returns a non-NULL value.
		/// </para>
		/// </remarks>
		public void EnsurePopupHost()
		{
			if( this.popupHost == null )
			{
				this.PopupHost = this.CreatePopupHost();
				m_bNeedDisposeHost = true;
			}
		}

        protected virtual PopupHost CreatePopupHost()
        {
            return new PopupHost();
        }

		/// <summary>
		/// Gets or sets a reference to the PopupHost that will be
		/// used to host this PopupControlContainer when dropped down.
		/// </summary>
		/// <value>The PopupHost object that will host this PopupControlContainer.</value>
		/// <remarks>
		/// <para>PopupHost is the top level form-based control that hosts
		/// this PopupControlContainer when dropped down.</para>
		/// <para>
		/// The PopupControlContainer usually creates a custom
		/// PopupHost when it is asked to drop down. However, you
		/// can provide your own PopupHost if you have a customized
		/// version.
		/// </para>
		/// <para>
		/// You can also get a reference to the <see cref="PopupHost"/>
		/// that the PopupControlContainer uses by default and make
		/// changes to it. The PopupControlContainer creates a
		/// default PopupHost when there is no PopupHost supplied
		/// to it, but <see cref="ShowPopup"/> gets called to show the popup.
		/// Hence, the best place to get the default PopupHost
		/// associated with this PopupControlContainer is in the
		/// <see cref="BeforePopup"/> handler. Or call <see cref="EnsurePopupHost"/> to
		/// create the default PopupHost if it is not yet created.
		/// </para>
		/// </remarks>
		/// <example>
		/// <para>The following example shows how to make the PopupHost's border style resizable to create a resizable popup.</para>
		/// <coderef file="\Tools\Editors Package\PopupControlContainer\PopupControlContainerDemo\CS\Form1.cs" name="Resizable Popup sample" lang="C#"><code lang="C#">
		///         // The PopupControlContainer's BeforePopup event handler
		///         private void popupControlContainer1_BeforePopup(object sender, System.ComponentModel.CancelEventArgs e)
		///         {
        ///             // Create a Popup, that can be resized.
		///
		///             // Make the popup host's border style resizable.
		///             this.popupControlContainer1.PopupHost.FormBorderStyle = FormBorderStyle.SizableToolWindow;
		///             this.popupControlContainer1.PopupHost.BackColor = this.BackColor;
		///
		///             // Necessary to set the host's client size every time, especially since the
		///             // popup's Dock style is set to DockStyle.Fill.
		///             if(!(this.popupControlContainer1.PopupHost.Size.Width >= 140))
		///                 this.popupControlContainer1.PopupHost.Size = new System.Drawing.Size(140, 150);
		///
		///             // So that the popup container will fill the entire popup host when resized.
		///             this.popupControlContainer1.Dock = DockStyle.Fill;
		///         }</code></coderef>
		/// <coderef file="\Tools\Editors Package\PopupControlContainer\PopupControlContainerDemo\VB\Form1.vb" name="Resizable Popup sample" lang="VB"><code lang="VB">
		///        ' The PopupControlContainer's BeforePopup event handler
		///        Private Sub popupControlContainer1_BeforePopup(ByVal sender As Object, ByVal e As CancelEventArgs)
		///
        ///            ' Create a popup that can be resized.
		///            ' Make the popup host's border style resizable.
		///            Me.popupControlContainer1.PopupHost.FormBorderStyle = FormBorderStyle.SizableToolWindow
		///            Me.popupControlContainer1.PopupHost.BackColor = Me.BackColor
		///            ' Necessary to set the host's client size every time, especially since the
		///            ' popup's dock style is set to DockStyle.Fill.
		///            ' So that the popup container will fill the entire popup host when resized.
		///            If Not ((Me.popupControlContainer1.PopupHost.Size.Width >= 140)) Then
		///                Me.popupControlContainer1.PopupHost.Size = New System.Drawing.Size(140, 150)
		///            End If
		///            Me.popupControlContainer1.Dock = DockStyle.Fill
		///
		///        End Sub</code></coderef>
		/// </example>
		[DefaultValue(null),
		Browsable(false),
		Description("Gets or sets a reference to the PopupHost that will be used to host this PopupControlContainer when dropped down.")
		]
		public PopupHost PopupHost
		{
			get{return this.popupHost;}
			set
			{
				if(this.popupHost != value)
				{
					PopupHost oldHost = this.popupHost;
					this.popupHost = value;

					// Update circular reference in PopupHost.
					if(oldHost != null)
						oldHost.PopupControlContainer = null;

					if(this.popupHost != null && this.popupHost.PopupControlContainer != this)
						this.popupHost.PopupControlContainer = this;

                    m_bNeedDisposeHost = false;
				}
			}
		}

        /// <summary>
        /// Gets or sets the discreet location.
        /// </summary>
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public Point DiscreetLocation
		{
			get
			{
				return this.discreetLocation;
			}

			set
			{
				this.discreetLocation = value;
			}
		}

		/// <summary>
		/// Specifies the PopupControlContainer's control parent.
		/// </summary>
		/// <value>A control instance.</value>
		/// <remarks>
		/// <para>The Parent-Child relationship in this case is NOT similar
		/// to the one in the control hierarchy.</para>
		/// <para>
		/// When you specify a Parent Control via
		/// ParentControl and pass a Point.Empty location to
		/// ShowPopup, the popup location will be dynamically determined
		/// based on the ParentControl bounds and the screen area.</para>
		/// </remarks>
		[DefaultValue(null),
		Category("Popup"),
		Description("Specifies the PopupControlContainer's Control Parent.")
		]
		public Control ParentControl
		{
			get{return this.parentControl;}
			set
			{
				if (this.parentControl != value)
				{
					DetachDelegates();
				}

				this.parentControl = value;

				if (null != this.parentControl)
				{
					AttachDelegates();
					UpdateRightToLeftFromParentControl();
				}

				this.discreetLocation = Point.Empty;
			}
		}

		class FormWindowSubclss:
			NativeWindow
		{
			private PopupControlContainer _popupContainer;

			public FormWindowSubclss( PopupControlContainer popupContainer )
			{
				_popupContainer = popupContainer;
			}

			protected override void WndProc( ref Message m )
			{
				if( _popupContainer != null && m.Msg == NativeMethods.WM_ACTIVATEAPP && m.WParam == IntPtr.Zero )
				{
					_popupContainer.HidePopup( PopupCloseType.Deactivated );
				}

				base.WndProc( ref m );
			}

			public override void DestroyHandle()
			{
				base.DestroyHandle();

				_popupContainer = null;
			}
		}

		void AttachDelegates()
		{
			if( null != this.parentControl )
			{
				this.parentControl.RightToLeftChanged += new EventHandler( ParentControl_RightToLeftChanged );
                //this.parentForm = this.parentControl.FindForm();
                if(this.parentForm!= null)
                    this.parentForm.Disposed += new EventHandler(ParentControl_Disposed);
			}
		}

		private void DetachDelegates()
		{
			if( null != this.parentControl )
			{
				this.parentControl.RightToLeftChanged -= new EventHandler( ParentControl_RightToLeftChanged );
                if(this.parentForm != null)
                    this.parentForm.Disposed -= new EventHandler(ParentControl_Disposed);
			}
		}

        private void ParentControl_Disposed(object sender, EventArgs e)
        {
            if (this.popupHost != null)
            {
                this.popupHost.Dispose();
                this.popupHost = null;
            }
        }

		void UpdateRightToLeftFromParentControl()
		{
			ParentControl_RightToLeftChanged( null, null );
		}

		void ParentControl_RightToLeftChanged(object sender, EventArgs e)
		{
			if( this.parentControl != null )
			{
				this.RightToLeft = this.parentControl.RightToLeft;
			}			
		}

		private bool GetIsMirrored()
		{
			return RightToLeft.Yes == RightToLeft;
		}

		/// <summary>
		/// Specifies the <see cref="IPopupParent"/> parent.
		/// </summary>
		/// <value>An instance that implements <see cref="IPopupParent"/>.</value>
		/// <remarks>
		/// <para>When you associate an IPopupParent interface with the PopupControlContainer
		/// you get even more control over the alignment and positioning
		/// of the Popup, (this you would
		/// do instead of the PopupControl based parenting).</para>
		/// <para>When you provide this interface, the alignment and
		/// positioning logic is delegated to this interface.</para>
		/// </remarks>
		/// <example>
		/// Take a look at our PopupsInDepth sample under the Tools/Samples/In Depth folder
		/// for sample usage.
		/// </example>
		[DefaultValue(null),
		Browsable(false),
		Description("Specifies the IPopupParent parent.")
		]
		public IPopupParent PopupParent
		{
			get{return this.popupParent;}
			set{this.popupParent = value;}
		}
		#endregion PROPERTIES

		/// <summary>
		/// Creates a new instance of the <see cref="Syncfusion.Windows.Forms.PopupControlContainer"/>.
		/// </summary>
		public PopupControlContainer()
		{
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(PopupControlContainer));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
			// Track any mouse clicks outside the popup.
			//this.mouseHooker = new MouseProcHookerUtil(Handle, this);

			this.listener = new PopupCloseRequestListener( this );
			_formSubclass = new FormWindowSubclss( this );
            CTRLSIZE = this.Size;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Syncfusion.Windows.Forms.PopupControlContainer"/> class with a specified container.
		/// </summary>
		/// <param name="container">A <see cref="System.ComponentModel.IContainer"/> that represents the container of the <see cref="Syncfusion.Windows.Forms.PopupControlContainer"/>.</param>
		/// <remarks>
		/// <para>Containers are objects that encapsulate and track zero or more components. In this context, containment refers to logical containment, not visual containment. 
        /// You can use components and containers in a variety of scenarios, including scenarios that are both visual and not visual.</para>
		/// <para><bold>Note to Implementers:</bold>  To be a container, the class must implement the IContainer interface, which supports methods for adding, removing and retrieving components.</para>
		/// <para>Unlike other controls, a <see cref="Syncfusion.Windows.Forms.PopupControlContainer"/> is not a direct child of your form (though this is the case during design-time).
		/// This requires you to explicitly dispose of this control before the form gets destroyed.
		/// With this constructor override, the <see cref="Syncfusion.Windows.Forms.PopupControlContainer"/> automatically plugs itself into a form's
		/// default component-containment pattern (through the IContainer member) during design-time. If you create this class in code, then you have to set it up manually.</para>
		/// <para>The implementation simply adds the <see cref="Syncfusion.Windows.Forms.PopupControlContainer"/> instance into the container.</para>
		/// </remarks>
		[ Obsolete( "This constructor is equal to default constructor. Use default constructor instead of this, because PopupControlContainer is Control, not component." ) ]
		public PopupControlContainer(IContainer container)
			:this()
		{
			//container.Add(this);
		}

		/// <override/>
		protected override void Dispose(bool disposing)
		{
			if( disposing )
			{
				if( this.IsShowing() )
					this.HidePopup();

				DetachDelegates();

				if( MouseHooker != null )
				{
					mouseHooker.Dispose();
					mouseHooker = null;
				}
				if( this.listener != null )
				{
					this.listener.Dispose();
					this.listener = null;
				}

				if( this.PopupHost != null )
				{
					PopupHost host = this.PopupHost;
					bool bNeedDisposeHost = m_bNeedDisposeHost;

					this.PopupHost = null;

					if( bNeedDisposeHost )
					{
						host.Dispose();
                        host = null;
					}					
				}

				if( _formSubclass != null )
				{
                    _formSubclass.ReleaseHandle();
                    _formSubclass.DestroyHandle();
					_formSubclass = null;
				}
                this.popupParent = null;
                parentControl = null;

				// Not disposing of the PopupHost because it could be used by someone else.
				// The default usage pattern will Dispose the PopupHost through its Destructor.
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Indicates whether to ignore dialog keys.
		/// </summary>
		/// <value>True to ignore dialog keys; False if not to. Default is False.</value>
		/// <remarks>
		/// When the popup is showing, the PopupControlContainer
		/// in the ProcessDialogKey override will look for Alt, Enter, Tab, Esc, F4,
		/// and F2 keys and either cancel or close the popup. If you want
		/// to prevent this, set IgnoreDialogKey = False. You should
		/// then listen for the above keys and make sure to close the popup manually whenever appropriate using
		/// HidePopup.
		/// </remarks>
		[DefaultValue(false),
		Category("Popup"),
		Description("Specifies whether or not to ignore dialog keys.")
		]
		public bool IgnoreDialogKey
		{
			get
			{
				return bIgnoreDialogKey;
			}
			set
			{
				bIgnoreDialogKey = value;
			}
		}
		/// <summary>
		/// Indicates whether to ignore all mouse messages.
		/// </summary>
		/// <value>True to ignore all mouse messages; False otherwise. Default is False.</value>
		/// <remarks>
		/// <para>When the popup is showing it will "swallow" all the mouse messages that are sent to
		/// controls not in the popup-hierarchy. When showing, the popup will also listen to
		/// mouse messages to determine if the popup should be closed (for mouse down
		/// outside the popup-hierarchy, for example).</para>
		/// <para>
		/// To prevent this behavior, set this property to True.</para>
		/// <para>When set to True, the popup will close only when you call
        /// the <see cref="HidePopup(PopupCloseType)"/> method or when a new popup gets shown.</para>
		/// </remarks>
		[DefaultValue(false),
		Category("Popup"),
		Description("Specifies whether or not to ignore all mouse messages.")
		]
		public bool IgnoreMouseMessages
		{
			get
			{
				return this.bIgnoreMouseMessages;
			}
			set
			{
				bIgnoreMouseMessages = value;
			}
		}
		/// <summary>
		/// Indicates whether to ignore all keys.
		/// </summary>
		/// <value>True to ignore all keys; False otherwise. Default is False.</value>
		/// <remarks>
		/// When the popup is showing, it will "swallow" all the WM_KEYDOWN and WM_CHAR
		/// messages. To prevent it, set this property to True.
		/// </remarks>
		[DefaultValue(false),
		Category("Popup"),
		Description("Specifies whether or not to ignore all keys.")
		]
		public bool IgnoreKeys
		{
			get
			{
				return this.bIgnoreKeys;
			}
			set
			{
				bIgnoreKeys = value;
			}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		bool INeedMouseMoveMessages.MouseMessage(ref Message m)
		{
			Control destination = Control.FromHandle(m.HWnd);
			if(destination == null)
				destination = PopupUtils.GetADotNetParentControl(m.HWnd);

			return this.ProcessMouseMessage(destination, m.Msg, m.LParam, m.WParam );
		}
		bool IMouseHookHLProcClient.MouseHookProc(int msg, Point point, IntPtr hwnd, int wHitTestCode, int dwExtraInfo)
		{
			Control destinationControl = Control.FromHandle(hwnd);

			return this.ProcessMouseMessage(destinationControl, msg, IntPtr.Zero, IntPtr.Zero);
		}
		protected virtual bool ProcessMouseMessage(Control destination, int msg, IntPtr lParam, IntPtr wParam )
		{
			if(!this.IgnoreMouseMessages)
			{
				if(!this.DesignMode
					&& (msg == 0x200 /*WM_MOUSEMOVE*/|| msg == 0x2A3/*WM_MOUSELEAVE*/
					|| msg == 0x2A1/*WM_MOUSEHOVER*/))
				{
					// If the destination is in the parent chain, let it pass through
					if(destination != null && this.IsRelatedControl(destination, true))
						return false;
						// else don't send it to the destination.
                    else
                        return true;
				}
				else
					this.VeryifyMouseBasedDeactivation(destination, msg);
			}
			return false;
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		bool INeedKeyboardMessages.KeyboardMessage(ref Message m)
		{
			bool bProceed = false;
			if(!this.bIgnoreKeys)
			{
				Control curFocusControl = Control.FromHandle(NativeMethods.GetFocus());
				if(curFocusControl == null)
					curFocusControl = PopupUtils.GetADotNetParentControl(NativeMethods.GetFocus());

				// Process keydown and char messages, if its not being sent to any of the related controls.
				if(!this.IsRelatedControl(curFocusControl, true))
				{
					if(m.Msg == 0x0100 || m.Msg == 0x0102)
					{
						Keys keys = (Keys)m.WParam.ToInt32();

						bool bisDialogKeyProcessed = this.ProcessDialogKey( keys );
						if( !bisDialogKeyProcessed )
						{
							if(this.popupParent != null && this.popupParent is IPopupChild)
							{
								bProceed = ((IPopupChild)this.popupParent).KeyboardMessage(ref m);
							}
						}
					}
				}
			}
			return bProceed;
		}
		bool IKeyboardProcHookClient.KeyboardHookProc(int wParam, int lParam)
		{
			if(!this.bIgnoreKeys)
			{
				Control curFocusControl = Control.FromHandle(NativeMethods.GetFocus());
				if(curFocusControl == null)
					curFocusControl = PopupUtils.GetADotNetParentControl(NativeMethods.GetFocus());

				// Process keydown and char messages, if its not being sent to any of the related controls.
				if(!this.IsRelatedControl(curFocusControl, true))
				{
					if((lParam & 0x80000000) == 0)
					{
						Keys keys = (Keys)wParam;
						if(!this.ProcessDialogKey(keys))
							if(this.popupParent != null && this.popupParent is IPopupChild)
								((IPopupChild)this.popupParent).KeyboardHookProc(wParam, lParam);
					}
					return true;
				}
			}
			return false;
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void VeryifyMouseBasedDeactivation(Control destinationControl, int msg)
		{
			switch(msg)
			{
				case  NativeMethods.WM_MOUSEACTIVATE: // 0x0021
				case  NativeMethods.WM_LBUTTONDOWN: // 0x0201
				case  NativeMethods.WM_RBUTTONDOWN: // 0x0204
				case  NativeMethods.WM_MBUTTONDOWN: // 0x0207
					if (this.IsRelatedControl(destinationControl,true))
					{
						break;
					}
					goto case NativeMethods.WM_NCLBUTTONDOWN;

				case  NativeMethods.WM_NCLBUTTONDOWN: // 0x00a1
				case  NativeMethods.WM_NCRBUTTONDOWN: // 0x00a4
				case  NativeMethods.WM_NCMBUTTONDOWN: // 0x00a7
					if( null != destinationControl
						&& !this.IsRelatedControl(destinationControl, !this.DesignMode))// true if runtime, false if designtime.
					{
						// click outside
						this.HidePopup(PopupCloseType.Deactivated);
					}
					break;
			}
		}

        /// <summary>
        /// Gets preferred location.
        /// </summary>
        /// <param name="prevAlignment"></param>
        /// <param name="newAlignment"></param>
        /// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual Point GetPreferredLocation(PopupRelativeAlignment prevAlignment, out PopupRelativeAlignment newAlignment)
		{
			if(this.popupParent != null)
			{
				Point loc = this.popupParent.GetLocationForPopupAlignment(prevAlignment, out newAlignment);
				if(loc != Point.Empty)
					return loc;
			}

			if(this.discreetLocation != Point.Empty)
			{
				newAlignment = PopupRelativeAlignment.Default;
				return this.discreetLocation;
			}

			Point location = Point.Empty;

			if(this.parentControl != null)
			{
				Rectangle parentBounds = this.parentControl.Bounds;

                location = PopupUtils.ComputeDefaultTopBottomAlignmentExt(prevAlignment,
					out newAlignment, parentControl, parentBounds, GetIsMirrored() );

				if(this.parentControl.Parent != null)
					location = this.parentControl.Parent.PointToScreen(location);
			}
			else
			{
				newAlignment = PopupRelativeAlignment.Default;
				location = new Point(1, 1);
			}

			return location;
		}
		/// <summary>
		/// Shows the popup at the specified location.
		/// </summary>
		/// <param name="location">A point in screen coordinates.
		/// Can be Point.Empty.</param>
		/// <remarks>
		/// <para>A popup's position is determined as follows.</para>
		/// <para>First, if an <see cref="IPopupParent"/> interface is associated (using
		/// the <see cref="PopupParent"/> property), then it is queried for the preferred
		/// position. If there is no such interface, the location
		/// specified in the <see cref="ShowPopup"/> method call is used. If ShowPopup
		/// was called with Point.Empty, then the <see cref="ParentControl"/>'s bounds are used
		/// to determine an appropriate position to drop-down.</para>
		/// <para>This will throw a <see cref="BeforePopup"/> followed by a <see cref="Popup"/> event.</para>
		/// <para>Use <see cref="HidePopup(PopupCloseType)"/> to hide the popup in code.</para>
		/// <para>
        /// When the popup is closed either programmatically or by the user,
		/// the <see cref="CloseUp"/> event will be thrown that will indicate
		/// whether the popup was closed or canceled.
		/// </para>
		/// </remarks>
		internal CancelEventArgs args = new CancelEventArgs(false);
        private bool PreventEventsForToolTip = false;
		public virtual void ShowPopup(Point location)
		{
            if (this is ToolTipAdv)
            {
                PreventEventsForToolTip = true;
            }
            EnsurePopupHost();
            if(!PreventEventsForToolTip)
            this.OnBeforePopup(args);
            if (args.Cancel)
                return;

            this.discreetLocation = location;

            this.popupHost.ShowPopup();
            if (!PreventEventsForToolTip)
            {
                this.OnPopup(EventArgs.Empty);
            }
            PreventEventsForToolTip = false;
		}

		/// <summary>
		/// Hides a popup that is open.
		/// </summary>
		/// <remarks>
		/// <para>This method will hide the popup with the <see cref="PopupCloseType.Canceled"/> mode.</para>
		/// </remarks>
		public virtual void HidePopup()
		{
			this.HidePopup(PopupCloseType.Canceled);
		}

        /// <summary>
        /// This method is called from the BarManager class when the parent form is deactivated.
        /// It checks whether the user clicked inside a child control of this PopupControlContainer.
        /// If this is not the case (e.g. user clicked on another window on the desktop) then the popup gets hidden.
        /// </summary>
        public virtual void ConfirmDeactivate()
        {
			bool childAtAPoint = false;
			Point screenPosition = MousePosition;

			foreach( Control control in this.Controls )
			{
				Point clientPosition = control.PointToClient( screenPosition );

				if( control.Bounds.Contains( clientPosition ) )
				{
					childAtAPoint = true;
					break;
				}
			}

            // ignoreDeactivateTick gets set at the time the user clicks on the child control inside the popup and
            // a WM_MOUSEACTIVATE message is received. At that time the old form is still active. Then when the Form.Deactivate
            // method is called we can check the ignoreDeactivateTick value. That way we know if the click was inside the
            // popup or outside on another control and the popup should be hidden.
            if (this.listener.ignoreDeactivateTick > Environment.TickCount || this.inOnPopup || childAtAPoint )
                return;
            else
                this.HidePopup(PopupCloseType.Deactivated);
        }
        /// <summary>
        /// Occurs before a popup is closed.
        /// </summary>
        [Description("Occurs before a popup is closed.")
		]
		public event CancelEventHandler BeforeCloseUp;

		/// <summary>
		/// Hides a popup with the specified <see cref="PopupCloseType"/> mode.
		/// </summary>
		/// <param name="popupCloseType">A PopupCloseType value.</param>
		public virtual void HidePopup(PopupCloseType popupCloseType)
		{
			if(!this.IsShowing())
				return;

			CancelEventArgs args = new CancelEventArgs();

			if( BeforeCloseUp != null )
			{
                BeforeCloseUp( this, args );				
			}

			if( !args.Cancel && this.popupHost.IsShowing() )
			{
				this.popupHost.HidePopup();
				this.OnCloseUp(new PopupClosedEventArgs(popupCloseType));
			}
		}

		/// <summary>
		/// Overridden. See <see cref="M:System.Windows.Forms.Control.ProcessDialogKey"/>.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		protected override bool ProcessDialogKey(Keys key)
		{
			if (bIgnoreDialogKey)
				return base.ProcessDialogKey(key);

			Keys keyCode = key & Keys.KeyCode;
			bool bAlt = (Control.ModifierKeys & Keys.Alt) != Keys.None;

			switch (keyCode)
			{
				case Keys.Down:
					if (bAlt)
						goto case Keys.F4;
					break;

				case Keys.F4:
					HidePopup(PopupCloseType.Canceled);
					FocusParent();
					return true;

				case Keys.F2:
				case Keys.Enter:
				case Keys.Tab:
                    if (CloseOnTab)
                    {
						HidePopup(PopupCloseType.Done);
						FocusParent();
						return true;
                    }
                    return false;

				case Keys.Escape:
					HidePopup(PopupCloseType.Canceled);
					FocusParent();
					return true;
			}

			return false;
		}
        /// <summary>
        /// Specifies whether the drop down need to be closed on pressing tab on the last item and shift tab on the first item in the drop down.
        /// </summary>
        [DefaultValue(true),
        Browsable(true),
        Description("Specifies whether the drop down need to be closed on pressing tab on the last item and shift tab on the first item in the drop down.")
        ]
        public bool CloseOnTab
        {
            get
            {
                return this.m_bCloseOnTab;
            }
            set
            {
                if(this.m_bCloseOnTab != value)
                    this.m_bCloseOnTab = value;
            }
        }
		/// <summary>
		/// Overridden. <see cref="M:System.Windows.Forms.Control.WndProc"/>.
		/// </summary>
		/// <param name="m"></param>
		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 0x21/*WM_MOUSEACTIVATE*/)
			{
				this.listener.ignoreDeactivateTick = Environment.TickCount + 100;
				m.Result = (IntPtr)3;
				return;
			}

			base.WndProc(ref m);
		}

		bool inSetFocus = false;

		/// <summary>
		/// Sets focus on the popup parent control.
		/// </summary>
		public virtual void FocusParent()
		{
			inSetFocus = true;
			try
			{
				if (ParentControl != null && ParentControl.Visible)
					ParentControl.Focus();
			}
			finally
			{
				inSetFocus = false;
			}
		}

        /// <summary>
        /// Focuses first visible parent.
        /// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual void FocusFirstVisibleParent()
		{
			Control parent = ParentControl;
			while (parent != null)
			{
				if (parent.Visible)
				{
					inSetFocus = true;
					try
					{
						parent.Focus();
					}
					finally
					{
						inSetFocus = false;
					}
					return;
				}
				parent = parent.Parent;
			}
		}

        /// <summary>
        /// Specifies whether the control is in set focus.
        /// </summary>
        /// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public bool IsInSetFocus()
		{
			return inSetFocus;
		}

        #region For Touch

        bool isScaling = false;

        bool _touchMode = false;
        /// <summary>
        /// gets or sets the touchmode
        /// </summary>
        [DefaultValue(false)]
        public bool EnableTouchMode
        {
            get
            {
                return _touchMode;
            }
            set
            {
                if (_touchMode != value)
                {
                    _touchMode = value;
                    if (_touchMode)
                        ApplyScaleToControl(1.5F);
                    else
                        ApplyScaleToControl(1);
                }
            }
        }
        /// <summary>
        ///
        /// </summary>
        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetEnableTouchMode()
        {
            EnableTouchMode = false;
        }
        /// <summary>
        ///applies the scaling
        /// </summary>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            foreach (Control ctrl in this.Controls)
            {
                PropertyInfo fi = ctrl.GetType().GetProperty("EnableTouchMode");//,
                fi.SetValue(ctrl, this.EnableTouchMode, null);
            }
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }
        /// <summary>
        ///font changed
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }
        /// <summary></summary>
        /// <param name="e"/>
        protected override void OnSizeChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnSizeChanged(e);
            if (!EnableTouchMode && CTRLSIZE != this.Size)
            {
                CTRLSIZE = this.Size;
            }
        }
        #endregion

		#region IPopupParent_Interface
		protected virtual Point GetLocationForPopupAlignment(PopupRelativeAlignment prevAlignment,
			out PopupRelativeAlignment newAlignment)
		{
			newAlignment = PopupRelativeAlignment.Default;
			return Point.Empty;
		}
		// Dummy imp.
		Point IPopupParent.GetLocationForPopupAlignment(PopupRelativeAlignment prevAlignment,
			out PopupRelativeAlignment newAlignment)
		{
			return this.GetLocationForPopupAlignment(prevAlignment, out newAlignment);
		}

		protected virtual Point[] GetBorderOverlapCue(PopupRelativeAlignment relativeAlignment)
		{
			return null;
		}
		// Dummy imp.
		Point[] IPopupParent.GetBorderOverlapCue(PopupRelativeAlignment relativeAlignment)
		{
			return null;
		}

		bool IPopupParent.IsRightToLeft
		{
			get
			{
				return RightToLeft.Yes == this.RightToLeft;
			}
		}

		protected virtual void ChildClosing(IPopupChild childUI, PopupCloseType popupCloseType)
		{
			if(childUI == this.currentPopupChild && popupCloseType != PopupCloseType.Deactivated)
			{
				PopupManager.SetCurrentPopupClient(this, true, false);
			}
			else
				this.HidePopup(PopupCloseType.Deactivated);
			this.currentPopupChild = null;
		}

		void IPopupParent.ChildClosing(IPopupChild childUI, PopupCloseType popupCloseType)
		{
			this.ChildClosing(childUI, popupCloseType);
		}
		#endregion

		#region POPUP_CHILDREN
		/// <summary>
		/// Gets or sets the current popup child in the popup hierarchy.
		/// </summary>
		/// <value>An instance of <see cref="IPopupChild"/> interface.</value>
		/// <remarks>When you want to show a parent-child hierarchy of popups,
		/// call this property on the parent popup before showing the child popup.
		/// </remarks>
		/// <example>
		/// For example:
		/// <code lang="C#">
		/// // While the parent PopupControlContainer is showing, you might want to show another child
		/// // PopupControlContainer. You can do so as follows:
		/// // Set up parent-child relationship.
		/// parentPopupControlContainer.CurrentPopupChild = childPopupControlContainer;
		/// childPopupControlContainer.PopupParent = parentPopupControlContainer;
		///
		/// // Now show the child popup.
		/// childPopupControlContainer.ShowPopup();
		/// </code>
		/// <code lang="VB">
		/// ' While the parent PopupControlContainer is showing, you might want to show another child
		///	' PopupControlContainer. You can do so as follows:
		/// ' Set up parent-child relationship
		/// parentPopupControlContainer.CurrentPopupChild = childPopupControlContainer
		/// childPopupControlContainer.PopupParent = parentPopupControlContainer
		///
		/// ' Now show the child popup.
		/// childPopupControlContainer.ShowPopup()
		/// </code>
		/// </example>
		[
		DefaultValue(null), Browsable(false), EditorBrowsable(EditorBrowsableState.Always),
		Description("Specifies the current popup child in the popup hierarchy.")
        ]
		public IPopupChild CurrentPopupChild
		{
			get
			{
				return this.currentPopupChild;
			}
			set
			{
				if(this.currentPopupChild != value)
				{
					if(this.currentPopupChild != null)
					{
						currentPopupChild.HidePopup(PopupCloseType.Canceled);
						currentPopupChild = null;
					}
					this.currentPopupChild = value;
				}
			}
		}
		#endregion

		#region IPopupChild_Interface
		Control IPopupItem.GetPopupParentControl()
		{
			if(this.popupParent != null)
				return this.popupParent.GetPopupParentControl();

			return this.parentControl;
		}

        /// <summary>
        /// Checks whether the control is a related control.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="askPopupParent"></param>
        /// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual bool IsRelatedControl(Control control, bool askPopupParent)
		{
			if( !this.IsShowing() )
				return false;

			if( control == this || this.Contains( control ) || control == this.Parent )
			{
				return true;
			}
			else if( askPopupParent )
			{
				if( this.popupParent != null )
				{
					return this.popupParent.IsRelatedControl( control, askPopupParent );
				}
				else if( control != null && this.parentControl != null &&
					(control == this.parentControl || this.parentControl.Contains( control )) )
				{
					return true;
				}
			}

			ScrollBarCustomDraw scroll = control as ScrollBarCustomDraw;

			if( scroll != null )
			{
				foreach( Control ctl in this.Controls )
				{
					if( ScrollersFrame.IsRelated( ctl, scroll ) )
					{
						return true;
					}
				}
			}

            if (this.ParentControl != null && this.ParentControl is ColorPickerButton && scroll != null)
            {
                return (scroll.VisualStyle == ScrollBarCustomDrawStyles.Metro && scroll is VScrollBarCustomDraw);
            }

			return false;
		}
		/// <summary>
		/// Indicates whether the popup is currently dropped down.
		/// </summary>
		/// <returns>True indicates popup is dropped down; False otherwise.</returns>
		public bool IsShowing()
		{
			if(this.popupHost != null)
				return this.popupHost.IsShowing();
			else
				return false;
		}
		#endregion IPopupChild_Interface
		#region EVENTS
		// Events
		/// <summary>
		/// Occurs when a popup is closed.
		/// </summary>
		/// <remarks>
		/// Handling this event will tell you whether the popup was
		/// closed or canceled by the user. This, in some cases, will then let you
		/// know whether or not you should accept changes in the popup.
		/// </remarks>
		[Description("Occurs when a popup is closed.")]
		public event PopupClosedEventHandler CloseUp;
		/// <summary>
		/// Occurs when the popup is about to be shown.
		/// </summary>
		/// <remarks>
		/// You may choose to cancel drop-down in this handler.
		/// This is also a good place, for example, to access the PopupControlContainer's
		/// PopupHost and make changes to it.
		/// </remarks>
		[Description("Occurs when the popup is about to be shown.")]
		public event CancelEventHandler BeforePopup;
		/// <summary>
		/// Occurs after the popup has been dropped down and made visible.
		/// </summary>
		/// <remarks>This is a good place, for example, to set the
		/// focus on a control in the popup.</remarks>
		[Description("Occurs after the popup has beedn dropped down and made visible.")]
		public event EventHandler Popup;

		/// <summary>
		/// Raises the BeforePopup event.
		/// </summary>
		/// <param name="args">A CancelEventArgs that contains the event data.</param>
		/// <remarks>
		/// <para>The OnBeforePopup method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class.</para>
		/// <para>Note to Inheritors:  When overriding OnBeforePopup in a derived
		/// class, be sure to call the base class's OnBeforePopup method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnBeforePopup(CancelEventArgs args)
		{
			Form form = this.parentControl as Form;

			if( form != null && _formSubclass != null )
			{
				_formSubclass.ReleaseHandle();
				_formSubclass.AssignHandle( form.Handle );
			}

			if( BeforePopup != null )
			{
				BeforePopup( this, args );
			}
		}

		/// <summary>
		/// Raises the <see cref="CloseUp"/> event.
		/// </summary>
		/// <param name="args">A <see cref="PopupClosedEventArgs"/> instance containing
		/// data pertaining to this event.</param>
		/// <remarks>
		/// <para>The OnCloseUp method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class. </para>
		/// <para>Note to Inheritors:  When overriding OnCloseUp in a derived
		/// class, be sure to call the base class's OnCloseUp method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnCloseUp(PopupClosedEventArgs args)
		{
			if( _formSubclass != null )
			{
				_formSubclass.ReleaseHandle();
			}

			this.CurrentPopupChild = null;

			IPopupParent parent = this.popupParent;

			this.listener.ParentControl = null;
			this.listener.PopupHost = null;

			PopupManager.SetCurrentPopupClient( this, false, this.fakeFocus );

			if( this.CloseUp != null )
				this.CloseUp( this, args );

			// Inform the parent as well.
			if( parent != null )
			{
				parent.ChildClosing( this, args.PopupCloseType );
			}
		}

        bool inOnPopup = false;

		/// <summary>
		/// Raises the <see cref="Popup"/> event.
		/// </summary>
		/// <param name="args">An EventArgs instance containing
		/// data pertaining to this event.</param>
		/// <remarks>
		/// <para>The OnPopup method also allows derived classes to handle the event
		/// without attaching a delegate. This is the preferred technique for
		/// handling the event in a derived class. </para>
		/// <para>Note to Inheritors:  When overriding OnPopup in a derived
		/// class, be sure to call the base class's OnPopup method so that
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnPopup(EventArgs args)
		{
            inOnPopup = true;
            try
            {
                if (this.popupParent != null)
                    this.listener.ParentControl = this.popupParent.GetPopupParentControl();
                else
                    this.listener.ParentControl = this.ParentControl;

                this.listener.PopupHost = this.PopupHost;

                PopupManager.SetCurrentPopupClient(this, true, !this.DesignMode && this.fakeFocus);

                if (this.Popup != null)
                    this.Popup(this, args);
            }
            finally
            {
                inOnPopup = false;

                // in case Control.Focus gets called after ShowPopup(), e.g. in DateTimePickerAdv:
                //this.popupWindow.ShowPopup(pt);
                //this.monthCalendar.Focus();
                this.listener.ignoreDeactivateTick = Environment.TickCount + 100;
            }
		}
		#endregion EVENTS
	}
}
