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
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Runtime.InteropServices;
using System.Drawing.Design;
using System.ComponentModel;
using System.Diagnostics;
using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Derives from the PopupHost class and adds a gripper label and
	/// makes the form sizable.
	/// </summary>
	[ToolboxItem(false)]
	public class SizablePopupHost : PopupHost
	{
		private const int c_nMinBoundsWidth		= 100;
		private const int c_nMinBoundsHeight	= 100;
        /// <summary>
        /// Used when ShowGripper or ShowCloseButton property is set to true;
        /// </summary>
        private const int c_nOffsetY = 34;

		/// <summary>
		/// Label for the gripper.
		/// </summary>
		private Label gripperLabel;

		/// <summary>
		/// The close button.
		/// </summary>
		private CloseButton closeBtn;

		/// <summary>
		/// The embedded child control.
		/// </summary>
		private Control childControl;

		/// <summary>
		/// The current size - used when resizing.
		/// </summary>
		private Size currentSize;

		/// <summary>
		/// Indicates whether the window is being resized.
		/// </summary>
		private bool isResizing;

		/// <summary>
		/// The host rectangle.
		/// </summary>
		private Rectangle hostRect;

		/// <summary>
		/// Internal initialization state of drop-down window.
		/// </summary>
		private bool isInitialized;

		/// <summary>
		/// The last size of the window.
		/// </summary>
		private Size lastSize;

        /// <summary>
        /// Indicates whether to show gripper.
        /// </summary>
        private bool m_bShowGripper = true;

        /// <summary>
        /// Indicates whether to show close bytton.
        /// </summary>
        private bool m_bShowCloseButton = true;

		/// <summary>
		/// Create an object of type SizablePopupHost.
		/// </summary>
		/// <param name="childControl"></param>
		public SizablePopupHost(Control childControl)
		{
			this.isInitialized = false;
			this.childControl = childControl;

			this.gripperLabel = new Label();
			this.closeBtn = new CloseButton();

			this.hostRect = Rectangle.Empty;
			this.currentSize = Size.Empty;
			this.isResizing = false;
			this.lastSize = Size.Empty;

			this.BackColor = this.childControl.BackColor;

			InitializeComponent();
		}

		~SizablePopupHost()
		{
			Dispose(false);
		}

		/// <override/>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
                if (this.childControl != null)
                {
                    this.childControl.Dispose();
                    this.childControl = null;
                }
                if (this.gripperLabel != null)
                {
                    this.gripperLabel.Dispose();
                    this.gripperLabel = null;
                }

                if (this.closeBtn != null)
                {
                    this.closeBtn.Dispose();
                    this.closeBtn = null;
                }
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Last size property.
		/// </summary>
		/// <remarks>For AutoComplete DropDownSize persistance</remarks>
		public Size LastSize
		{
			get
			{
				return lastSize;
			}
			set
			{
				if( lastSize != value )
					lastSize = value;
			}
		}

        /// <summary>
        /// Returns/sets the visibility of the close button
        /// </summary>
        public bool ShowCloseButton
        {
            get
            {
                return m_bShowCloseButton;
            }
            set
            {
                if (value != m_bShowCloseButton)
                {
                    m_bShowCloseButton = value;
                    this.closeBtn.Visible = m_bShowCloseButton;
                }
            }
        }

        /// <summary>
        /// Indicates whether a gripper will be shown that can used for resizing
        /// </summary>
        public bool ShowGripper
        {
            get
            {
                return m_bShowGripper;
            }
            set
            {
                if (value != m_bShowGripper)
                {
                    m_bShowGripper = value;
                    this.gripperLabel.Visible = m_bShowGripper;
                }
            }
        }

        /// <summary>
		/// Hides the popup.
		/// </summary>
		public override void HidePopup()
		{
			if (this.isResizing)
			{
				ProcessGripperDragRelease( true );
			}

			this.lastSize = this.Size;
			base.HidePopup();
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SuspendLayout();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Overrides PopupHost.ComputeMySize.
		/// </summary>
		protected override void ComputeMySize()
		{
			this.FormBorderStyle = FormBorderStyle.None;
			SizablePopupControlContainer childContainer = (SizablePopupControlContainer)this.PopupControlContainer;

            bool bFirstTime = !this.isInitialized;

            if (bFirstTime || childContainer.FitToChildControlSize)
            {
                this.childControl.Location = Point.Empty;
                int offsetY = ( this.ShowGripper || this.ShowCloseButton ) ? c_nOffsetY : 0;

                childContainer.Dock = DockStyle.None;
                this.childControl.Dock = DockStyle.None;

                childContainer.Height = this.childControl.Height + offsetY;
                                              
                if (!childContainer.Visible)
                    this.childControl.Height = childContainer.Height - offsetY;

				childContainer.Width = childContainer.GetPreferredSize(Size.Empty).Width;
            }
            
            if (bFirstTime)
			{
				int verticalScrollWidth = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXVSCROLL);
                                
                childContainer.BorderStyle = BorderStyle.FixedSingle;

				// The gripper.
				this.gripperLabel.Location = new Point(childContainer.Width - 24 ,childContainer.Height -24);
				childContainer.Controls.Add(this.gripperLabel);
				this.gripperLabel.Font = Syncfusion.Drawing.FontUtil.CreateFont("Marlett", 15.75F, System.Drawing.FontStyle.Regular);
				this.gripperLabel.Text = "p";
				this.gripperLabel.Height = 24;
				this.gripperLabel.Width = 32;
				this.gripperLabel.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
				this.childControl.Anchor = AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Top|AnchorStyles.Right;

				// The close button.
				this.closeBtn.Location  = new Point(2, childContainer.Height - 20);
				this.closeBtn.Width = 16;
				this.closeBtn.Height = 16;
				this.closeBtn.FlatStyle = FlatStyle.Flat;
				this.closeBtn.TextAlign = ContentAlignment.MiddleCenter;
				childContainer.Controls.Add(this.closeBtn);
				this.closeBtn.Text = "x";

				//Events.
				this.closeBtn.MouseEnter += new System.EventHandler(this.HandleCloseBtnMouseEnter);
				this.closeBtn.MouseLeave += new System.EventHandler(this.HandleCloseBtnMouseLeave);
				this.closeBtn.Click += new System.EventHandler(this.HandleCloseBtnClick);
				this.gripperLabel.MouseMove += new MouseEventHandler(this.HandleGripperMouseMove);
				this.gripperLabel.MouseDown += new MouseEventHandler(this.HandleGripperMouseDown);
				this.gripperLabel.MouseUp += new MouseEventHandler(this.HandleGripperMouseUp);

				this.currentSize = this.ClientRectangle.Size;
				this.isInitialized = true;
			}

			base.ComputeMySize();

            int padding = 3;
            Size adjustedSize = new Size(this.Size.Width, this.Size.Height + padding);
            if (childContainer.FitToChildControlSize)
                this.Size = adjustedSize;

            if (this.lastSize != Size.Empty && !childContainer.FitToChildControlSize)
            	this.Size = this.lastSize;
            
            UpdateControlLayout( childContainer, bFirstTime );

            childControl.Dock = DockStyle.Top;
            childContainer.Dock = DockStyle.Fill;
		}

		private void UpdateControlLayout( SizablePopupControlContainer childContainer, bool bFirstTime )
		{
			bool bIsMirrored = GetIsMirrored();

			Size sizeParent = childContainer.Size;
			int nGripperX = 0, nCloseBtnX = 0;
			
			if (bIsMirrored)
			{
				nGripperX = -6;
				this.gripperLabel.Text = "y";
				this.gripperLabel.Cursor = System.Windows.Forms.Cursors.SizeNESW;

				nCloseBtnX = sizeParent.Width - closeBtn.Width - 4;

				this.gripperLabel.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
				this.closeBtn.Anchor = AnchorStyles.Bottom|AnchorStyles.Right;
			}
			else
			{
				nGripperX = sizeParent.Width - 28;
				this.gripperLabel.Text = "p";
				this.gripperLabel.Cursor = System.Windows.Forms.Cursors.SizeNWSE;

				nCloseBtnX = 2;

				this.gripperLabel.Anchor = AnchorStyles.Bottom|AnchorStyles.Right;
				this.closeBtn.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			}
			
			Point ptGripper = new Point( nGripperX, sizeParent.Height - 24 );
			Point ptCloseBtn = new Point( nCloseBtnX, sizeParent.Height - 20 );
			if (bFirstTime)
			{
				ptGripper.Offset( bIsMirrored ? 0 : 2, 2 );
				ptCloseBtn.Offset( bIsMirrored ? 2 : 0, 2 );
			}	
			
			this.gripperLabel.Location = ptGripper;
			this.closeBtn.Location = ptCloseBtn;
		}

		/// <override/>
		[EditorBrowsable(EditorBrowsableState.Advanced)] 
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
            SizablePopupControlContainer childContainer = (SizablePopupControlContainer)this.PopupControlContainer;
            bool fitToChildSize = false;

            if( childContainer != null )
                fitToChildSize = childContainer.FitToChildControlSize;

			if(width < c_nMinBoundsWidth && !fitToChildSize)
			{
				if (GetIsMirrored())
				{
					int nWidthDiff = c_nMinBoundsWidth - width;
                    x -= nWidthDiff;
				}
				width = c_nMinBoundsWidth;
			}

			if(height < c_nMinBoundsHeight && !fitToChildSize)
				height = c_nMinBoundsHeight;

			base.SetBoundsCore(x,y,width,height,BoundsSpecified.All);
		}

		/// <summary>
		/// Handles the MouseDown event of the gripper label.
		/// </summary>
		/// <param name="sender">The gripper label.</param>
		/// <param name="e">The event data.</param>
		private void HandleGripperMouseDown(object sender, MouseEventArgs e)
		{
			this.isResizing = true;

			this.hostRect.Location = new System.Drawing.Point(this.Left,this.Top);
			this.hostRect.Size = Size;
			ControlPaint.DrawReversibleFrame(this.hostRect, Color.Empty, System.Windows.Forms.FrameStyle.Dashed);
		}

		/// <summary>
		/// Handles the MouseUp event of the gripper label.
		/// </summary>
		/// <param name="sender">The gripper label.</param>
		/// <param name="e">The event data.</param>
		private void HandleGripperMouseUp(object sender, MouseEventArgs e)
		{
			ProcessGripperDragRelease( false );
		}

		private void ProcessGripperDragRelease( bool bCanceled )
		{
			if (this.isResizing)
			{
				ControlPaint.DrawReversibleFrame(this.hostRect, Color.Empty, FrameStyle.Dashed);

				if (!bCanceled)
				{
					this.Location = this.hostRect.Location;
					this.Size = this.currentSize;
					this.lastSize = this.currentSize;
                    this.childControl.Height = this.Height - c_nOffsetY;
                    this.childControl.Invalidate();
				}

				this.isResizing = false;
			}
		}

		/// <summary>
		/// Handler for the MouseMove event of the gripper label.
		/// </summary>
		/// <param name="sender">The gripper label.</param>
		/// <param name="e">The event data.</param>
		private void HandleGripperMouseMove(object sender, MouseEventArgs e)
		{
			try
			{
				if (this.isResizing && e.Button == MouseButtons.Left)
				{				
					// Undraw .hostRect.
					ControlPaint.DrawReversibleFrame(this.hostRect, Color.Empty, FrameStyle.Dashed);
					
					bool bIsMirrored = GetIsMirrored();

					// .currentSize
					int nMouseX = MousePosition.X;
					int nFrameWidth = bIsMirrored ?
						this.Location.X - nMouseX + this.Width :
						MousePosition.X - this.Location.X;

					this.currentSize = new Size( nFrameWidth, MousePosition.Y - this.Location.Y);
					
					int nDiffX = nFrameWidth - this.hostRect.Width;
					int nFrameLeft = bIsMirrored ? nMouseX : this.Left ;

					// Calcualate new .hostRect.
					this.hostRect.Location = new System.Drawing.Point( nFrameLeft, this.Top );
					this.hostRect.Size = this.currentSize;

					// Draw new .hostRect.
					ControlPaint.DrawReversibleFrame(this.hostRect , Color.Empty, FrameStyle.Dashed);
				}
			}
			catch(Exception ex)
			{
				TraceUtil.TraceExceptionCatched(ex);
				if (!ExceptionManager.RaiseExceptionCatched(this, ex))
					throw;
			}
		}

		/// <summary>
		/// Handler for the MouseEnter event of the Close Button.
		/// </summary>
		/// <param name="sender">The close button.</param>
		/// <param name="e">The event data.</param>
		private void HandleCloseBtnMouseEnter(object sender, System.EventArgs e)
		{
			this.closeBtn.BackColor = SystemColors.InactiveCaptionText;

		}

		/// <summary>
		/// Handler for the MouseLeave event of the Close Button.
		/// </summary>
		/// <param name="sender">The close button.</param>
		/// <param name="e">The event data.</param>
		private void HandleCloseBtnMouseLeave(object sender, System.EventArgs e)
		{
			this.closeBtn.BackColor = SystemColors.Control;
		}

		/// <summary>
		/// Handler for the Click event of the close button.
		/// Closes the popup window.
		/// </summary>
		/// <param name="sender">The close button.</param>
		/// <param name="e">The event data.</param>
		private void HandleCloseBtnClick(object sender, System.EventArgs e)
		{
			SizablePopupControlContainer childContainer = (SizablePopupControlContainer)this.PopupControlContainer;
			childContainer.HidePopup(PopupCloseType.Canceled);
		}
	}

	/// <summary>
	/// The close button used in SizablePopupHost.
	/// </summary>
	[ToolboxItem(false)]
	public class CloseButton:Button
	{
		public CloseButton()
		{
			this.Size = new Size(16,16);
		}

		/// <summary>
		/// Override OnPaint.
		/// </summary>
		/// <param name="pe">The event data.</param>
		protected override void OnPaint(PaintEventArgs pe)
		{
			base.OnPaint(pe);
			Pen pen = new Pen(SystemColors.ControlText, 2);
			Rectangle rect = this.Bounds/*new Rectangle(0,0, this.Width, this.Height)*/;
			Graphics g = pe.Graphics;

			g.DrawLine(pen,new Point(4,4),new Point(12,12));
			g.DrawLine(pen,new Point(4,12),new Point(12,4));
			pen.Dispose();
		}

		/// <summary>
		/// Gets / sets the text property.
		/// </summary>
		public override string Text
		{
			get
			{
				return base.Text;
			}

			set
			{
				base.Text = String.Empty;
			}
		}
	}
}