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

#region File Using
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Design;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools.XPMenus;
using Syncfusion.Drawing;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// <p>
	/// A <see cref="ControlBar"/> is a specialized implementation of the <see cref="Syncfusion.Windows.Forms.Tools.CommandBar"/> class that 
	/// is used for hosting controls within the Essential Tools XPMenus Framework. A ControlBar can contain two controls - 
	/// the main control that occupies the ControlBar's client region and optionally, a <see cref="ControlBar.CaptionControl"/> that is 
	/// displayed within the ControlBar's caption region. While just about any <see cref="System.Windows.Forms.Control"/> instance 
	/// can be used as the ControlBar's main client, the caption control position is normally occupied by single line controls such as a 
	/// ToolBar, TextBox or ComboBox. ControlBars thus function as full-featured docking windows that can be docked along 
	/// the host form's borders or floated as top-level windows, while following the layout limitations of the XPMenus toolbars.
	/// Examples of the ControlBar concept include the Microsoft Office 2003 TaskPane window.
	/// </p>
	/// <p>
	/// ControlBars are supported through the XPMenus framework and can be added to any form that has been initialized 
	/// with a <see cref="XPMenus.MainFrameBarManager"/>. Invoking the MainFrameBarManager's AddControlBar design-time verb will 
	/// create a new ControlBar and add it to the form. Dropping a control onto the ControlBar sets it as the ControlBar's main client. 
	/// To assign the caption control, drop control onto the ControlBar and set the <see cref="ControlBar.CaptionControl"/> to reference it.
	/// </p>
	/// </summary>
	public class ControlBar : CommandBar
	{
		#region Class Static Members
		protected static bool bDockedSizing = false;
		protected static Point ptSizingStart = Point.Empty;
		/// <summary>
		/// Reference to renderer for ControlBar.
		/// </summary>
		private ControlBarRenderer m_renderer = null;
		#endregion

		#region Class Constants
		protected const int nTBDropDownWidth = 250;
		#endregion

		#region Class Members
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Control cptnControl = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Size minSize = new Size(100,100);
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Size maxSize = new Size(1000,1000);
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nCtrlBarCaptionHt = 0;
		internal ControlBarWeakContainer controlBarWeakContainer = null;
		#endregion

		#region Class Properties

		/// <summary>
		/// Gets / sets the height of the <see cref="ControlBar"/> caption area.
		/// </summary>
		/// <value>An integer value representing the caption height.</value>
		[
		Category("Appearance"),
		Description("The height of the ControlBar caption area."),
		Localizable(true)
		]
		public int ControlBarCaptionHeight
		{
			get { return this.nCtrlBarCaptionHt; }
			set
			{
				if(this.nCtrlBarCaptionHt != value)
				{
					this.nCtrlBarCaptionHt = value;
					if(this.Parent != null)
					{
						if(this.cbarDockState != CommandBarDockState.Float)
							this.bRedockNeeded = true;
						else
							this.bRecalcNeeded = true;
						this.Invalidate();
					}
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal override Rectangle CaptionRect
		{
			get 
			{
				Size szCaption = new Size( this.Width - 4, this.nCtrlBarCaptionHt );
				Point ptLocation = new Point( 2, 2 );

				if( this.IsRTL )
				{
					Rectangle rcClient = this.ClientRectangle;

					ptLocation.X = rcClient.Right - ptLocation.X - szCaption.Width;
				}

				return new Rectangle( ptLocation, szCaption );
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal override Rectangle DragStartRect
		{
			get
			{
				Rectangle rcdrag = new Rectangle(2, 2, this.Width-4, this.nCtrlBarCaptionHt);
				if((this.cbarDockState == CommandBarDockState.Left) || (this.cbarDockState == CommandBarDockState.Right) 
					|| (this.cbarDockState == CommandBarDockState.Float))
				{
					if(this.cptnControl != null)
						rcdrag.Height += this.cptnControl.Height;
				}
				return rcdrag;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal override Rectangle GripperRect
		{
			get
			{
				Rectangle rcgripper = Rectangle.Empty;

				if( !this.bHideGripper )
				{
					Rectangle rcclient = this.ClientRectangle;
					rcgripper = new Rectangle(
						this.IsRTL ? rcclient.Right-2-this.nHeaderOff : rcclient.Left+2,
						rcclient.Top+2, this.nHeaderOff, this.nCtrlBarCaptionHt );
				}

				return rcgripper;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal override Rectangle DropDownRect
		{
			get
			{
				Rectangle rcDD = Rectangle.Empty;

				if( CommandBarDockState.None != this.cbarDockState && !this.bHideDropDown )
				{
					int nleftoffset = this.nHeaderOff+2;
					int ncaptionht = CommandBar.CaptionHeight;
					int ntrailwidth = 0;
					if(this.bHideCloseButton == false)
						ntrailwidth = ncaptionht;
					else
						ntrailwidth = 1;

					Rectangle rccaption  = this.CaptionRect;
					Size szDD;

					if((this.cbarDockState == CommandBarDockState.Top) || (this.cbarDockState == CommandBarDockState.Bottom))
					{
						if(this.cptnControl != null)
							nleftoffset += this.cptnControl.Width+2;
						int nddwidth = rccaption.Width-nleftoffset-ntrailwidth;
						if(nddwidth > ControlBar.nTBDropDownWidth)
							nddwidth = ControlBar.nTBDropDownWidth;

						szDD = new Size( nddwidth, ncaptionht );
					}
					else	// CommandBarDockState.Left||CommandBarDockState.Right||CommandBarDockState.Float
					{
						szDD = new Size( rccaption.Width-nleftoffset-ntrailwidth, ncaptionht );
					}

					Point ptLocation = new Point(
						this.IsRTL ? rccaption.Right-nleftoffset-szDD.Width : rccaption.Left+nleftoffset,
						rccaption.Top+(int)Math.Floor((float)(rccaption.Height-ncaptionht)/2f) );

					rcDD = new Rectangle( ptLocation, szDD );
				}

				return rcDD;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal override Rectangle CloseButtonRect
		{
			get
			{
				Rectangle rcButton = Rectangle.Empty;

				if( !this.bHideCloseButton )
				{
					int ncaptionht = CommandBar.CaptionHeight;
					Rectangle rccaption = this.CaptionRect;

					rcButton = new Rectangle( this.IsRTL ? rccaption.Left : rccaption.Right-ncaptionht,
						rccaption.Top+(int)Math.Floor((float)(rccaption.Height-ncaptionht)/2f),
						ncaptionht, ncaptionht );
				}

				return rcButton;
			}
		}

		/// <summary>
		/// Gets / sets the size of the ControlBar in pixels.
		/// </summary>
        [
		Category("Layout"),
		Description("The size of the ControlBar in pixels."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public new Size Size 
		{
			get { return base.Size; }
			set 
			{
				if(this.Parent != null)
				{
					if((this.cbarDockState == CommandBarDockState.Left) || (this.cbarDockState == CommandBarDockState.Right))
					{
						if((value.Width != this.nCommandBarHt) && (value.Width >= this.minSize.Width))
						{
							// Increase the size of the parent dock bar and displace all CommandBars in the next row by the same distance
							int deltaX = value.Width - this.nCommandBarHt;
							this.cdbParent.Width += deltaX;
							for(int i=this.nRCIndex+1; i<this.cdbParent.nRCCount; i++)
							{
								CommandBar[] cbarray = this.cdbParent.GetRowArray(i);
								foreach(CommandBar cbar in cbarray)
									cbar.Location = new Point(cbar.Location.X+deltaX, cbar.Location.Y);
							}
							this.nCommandBarHt = value.Width;
							this.Width = value.Width;
						}
					}
					else if((this.cbarDockState == CommandBarDockState.Top) || (this.cbarDockState == CommandBarDockState.Bottom))
					{
						if((value.Height != this.nCommandBarHt) && (value.Height >= this.minSize.Height))
						{
							// Increase the size of the parent dock bar and displace all CommandBars in the next row by an equivalent distance
							int deltaY = value.Height - this.nCommandBarHt;
							this.cdbParent.Height += deltaY;
							for(int i=(this.nRCIndex+1); i<this.cdbParent.nRCCount; i++)
							{
								CommandBar[] cbarray = this.cdbParent.GetRowArray(i);
								foreach(CommandBar cbar in cbarray)
									cbar.Location = new Point(cbar.Location.X, cbar.Location.Y+deltaY);
							}
							this.nCommandBarHt = value.Height;
							this.Height = value.Height;
						}
					}
					else if(this.cbarDockState == CommandBarDockState.Float)
					{
						if((value.Width != this.Width) && (value.Width >= this.minSize.Width))
							this.Parent.Width += (value.Width-this.Width);
						if((value.Height != this.Height) && (value.Height >= this.minSize.Height))
							this.Parent.Height += (value.Height-this.Height);
					}
				}
			}
		}


		/// <summary>
		/// Gets / sets the minimum extent to which the ControlBar can be sized.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Size"/> value.</value>
		[
		Category("Layout"),
		Description("The minimum size of the ControlBar in pixels."),
		Localizable(true)
		]
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		public Size MinimumSize
#else
		public new Size MinimumSize
#endif		
		{
			get { return this.minSize; }
			set
			{
				if(this.minSize != value)
				{
					this.minSize = value;
					if(this.Parent != null)
					{
						if((this.cbarDockState == CommandBarDockState.Left) || (this.cbarDockState == CommandBarDockState.Right))
						{
							if(value.Width > this.nCommandBarHt)
							{
								// Increase the size of the parent dock bar and displace all CommandBars in the next row by the same distance
								int deltaX = value.Width - this.nCommandBarHt;
								this.cdbParent.Width += deltaX;
								for(int i=this.nRCIndex+1; i<this.cdbParent.nRCCount; i++)
								{
									CommandBar[] cbarray = this.cdbParent.GetRowArray(i);
									foreach(CommandBar cbar in cbarray)
										cbar.Location = new Point(cbar.Location.X+deltaX, cbar.Location.Y);
								}
								this.nCommandBarHt = value.Width;
								this.Width = value.Width;
							}
						}
						else if((this.cbarDockState == CommandBarDockState.Top) || (this.cbarDockState == CommandBarDockState.Bottom))
						{
							if(value.Height > this.nCommandBarHt)
							{
								// Increase the size of the parent dock bar and displace all CommandBars in the next row by an equivalent distance
								int deltaY = value.Height - this.nCommandBarHt;
								this.cdbParent.Height += deltaY;
								for(int i=(this.nRCIndex+1); i<this.cdbParent.nRCCount; i++)
								{
									CommandBar[] cbarray = this.cdbParent.GetRowArray(i);
									foreach(CommandBar cbar in cbarray)
										cbar.Location = new Point(cbar.Location.X, cbar.Location.Y+deltaY);
								}
								this.nCommandBarHt = value.Height;
								this.Height = value.Height;
							}
						}
						else if(this.cbarDockState == CommandBarDockState.Float)
						{
							if(value.Width > this.Width)
								this.Parent.Width += (value.Width-this.Width);
							if(value.Height > this.Height)
								this.Parent.Height += (value.Height-this.Height);
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets / sets the maximum extent to which the ControlBar can be sized.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Size"/> value.</value>
		[
		Category("Layout"),
		Description("The maximum size of the ControlBar in pixels."),
		Localizable(true)
		]
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		public Size MaximumSize
#else
		public new Size MaximumSize
#endif
		
		{
			get { return this.maxSize; }
			set
			{
				if(this.maxSize != value)
				{
					this.maxSize = value;
				}
			}
		}
		
		[		
		Browsable(false),
		Syncfusion.Documentation.DocumentationExclude(),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override int MinHeight
		{
			get { return base.MinHeight; }
			set { base.MinHeight = value; }
		}

		[
		Browsable(false),
		Syncfusion.Documentation.DocumentationExclude(),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override int MinLength
		{
			get { return base.MinLength; }
			set { base.MinLength = value; }
		}

		[
		Browsable(false),
		Syncfusion.Documentation.DocumentationExclude(),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override int MaxLength
		{
			get { return base.MaxLength; }
			set { base.MaxLength = value; }
		}

		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public override int IntegralHeight
		{
			get { return this.nIntegral; }
			set { /*No imp*/ }
		}

		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public override int RowOffset
		{
			get { return this.nRowOffsetDir; }
			set { /*No imp*/ }
		}

		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public override bool DockModeWrapping
		{
			get { return this.bDockModeWrapping; }
			set { /*No imp*/ }
		}

		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public override bool FloatModeWrapping
		{
			get { return this.bFloatModeWrapping; }
			set { /*No imp*/ }
		}

		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public override bool OccupyFullRow
		{
			get { return this.bFullRow; }
			set { /*No imp*/ }
		}

		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public override bool ShowDockModeText
		{
			get { return this.bShowDockModeText; }
			set { /*No imp*/ }
		}

		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public override bool HideChevron
		{
			get { return this.bHideChevron; }
			set { /*No imp*/ }
		}

		/// <summary>
		/// Gets / sets the control that is displayed in the CommandBar's caption region.
		/// </summary>
		/// <value>A <see cref="System.Windows.Forms.Control"/> instance.</value>
		[
		Category("Behavior"),
		Description("The Control that is displayed in the CommandBar's caption region.")
		]
		public Control CaptionControl
		{
			get { return this.cptnControl; }

			set
			{
				if(this.cptnControl != value)
				{
					this.cptnControl = value;
					if(this.cptnControl != null)
					{
						this.cptnControl.Dock = DockStyle.None;
						this.cptnControl.Anchor = AnchorStyles.Top|AnchorStyles.Left;
						this.cptnControl.Height = this.nCtrlBarCaptionHt-2;
						if(this.Controls.Contains(this.cptnControl) == false)
							this.Controls.Add(this.cptnControl);
						else
							this.SetChildControlBounds();
						this.Invalidate();
					}
				}
			}
		}

		/// <summary>
		/// Gets the client control in the control collection.
		/// </summary>
		protected internal Control ClientControl
		{
			get
			{
				Control clientctrl = null;
				foreach(Control ctrl in this.Controls)
				{
					if(ctrl != this.cptnControl)
					{
						clientctrl = ctrl;
						break;
					}
				}
				return clientctrl;
			}
		}		

		#endregion

		#region Class Initialize/Finalize Methods
		/// <summary>
		/// Creates a new instance of the ControlBar class.
		/// </summary>
		public ControlBar()
		{
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override void InitializeCommandBar()
		{
			this.SetStyle(ControlStyles.Selectable, false);			
			this.SetStyle(ControlStyles.StandardDoubleClick
				|ControlStyles.UserPaint|ControlStyles.SupportsTransparentBackColor
				|ControlStyles.AllPaintingInWmPaint|ControlStyles.DoubleBuffer,
				true);
			this.TabStop = false;

			this.bFullRow = true;
			this.nCommandBarHt = 150;
			this.bFloatModeWrapping = true;
			this.bRestrictedSizing = false;
			this.nCtrlBarCaptionHt = CommandBar.CaptionHeight+8;
			this.cbarDockState = CommandBarDockState.Left;
			
			this.pupMenu = new PopupMenu();
			this.pupMenu.ParentBarItem = new ParentBarItem();

			this.Font = SystemInformation.MenuFont;
			if(CommandBar.ftFloatCaption == null)
				CommandBar.InitializeFloatCaptionFont();
		    
			UpdateColorScheme();

			controlBarWeakContainer = new ControlBarWeakContainer(this);
			MenuColors.MenuColorsChanged += new EventHandler(this.controlBarWeakContainer.MenuColorsChangedWeakEventHandler);
			Office2003Colors.MenuColorsChanged += new EventHandler(this.controlBarWeakContainer.Office2003ColorsChangedWeakEventHandler);
		}


		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.Dispose"/>.
		/// </summary>
		protected override void Dispose(bool bdisposing)
		{
			if(bdisposing == true)
			{
				Office2003Colors.MenuColorsChanged -= new EventHandler(this.Office2003Colors_MenuColorsChanged);
			}
			base.Dispose(bdisposing);
		}

		#endregion

		#region Class Utility Methods
		/// <summary>
		/// Updates color schemes.
		/// </summary>
		private void UpdateColorScheme()
		{
			MenuColors.UpdateMenuColors();
			Office2003Colors.UpdateMenuColors();
			VS2005Colors.UpdateMenuColors();
			
			Office2007Theme theme = ( cbController != null ) ? 
				cbController.Office2007Theme : Office2007Theme.Blue;
			Office2007OutlookColors.UpdateMenuColors( theme );
		}
		/// <summary>
		/// Sets renderer for ControlBar.
		/// </summary>
		protected override void SetRenderer()
		{
			base.SetRenderer();

			VisualStyle style = ( this.cbController == null ) ? VisualStyle.Default : 
				this.cbController.Style;

			if( m_renderer != null )
			{
				m_renderer.Dispose();
			}

			if( ShouldDrawThemed() 
				&& style != VisualStyle.Office2007
				&& style != VisualStyle.Office2007Outlook )
			{
				m_renderer = new ControlBarRendererThemed( this );
			}
			else
			{
				switch( style )
				{
					case VisualStyle.VS2005 :
					{
						m_renderer = new ControlBarRendererVS2005( this );
						break;
					}
					case VisualStyle.Office2003 :
					{
						m_renderer = new ControlBarRendererOffice2003( this );
						break;
					}
					case VisualStyle.Office2007Outlook :
					case VisualStyle.Office2007 :
					{
						m_renderer = new ControlBarRendererOffice2007( this );
						break;
					}
                    case VisualStyle.Office2010:
                    {
                        m_renderer = new ControlBarRendererOffice2010(this);
                        break;
                    }
					default :
					{
						m_renderer = new ControlBarRendererOfficeXP( this );
						break;
					}
				}
			}
		}

		#endregion

		#region Class Event Handlers

		[Syncfusion.Documentation.DocumentationExclude()]
		public void Office2003Colors_MenuColorsChanged(object sender, EventArgs e)
		{
			if((this.ShouldDrawThemed() == false) && (this.cbController.Style == VisualStyle.Office2003))
			{
				base.BackColor = Office2003Colors.DockBarColorLight;
				this.bBackColorSet = false;
			}
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			if (this.cbController.Style != VisualStyle.Default && this.cbController.Style != VisualStyle.OfficeXP && this.cbController.Style != VisualStyle.Metro)
			{
				base.OnPaint (e);
			}

			if( m_renderer != null )
			{
				m_renderer.Draw( e.Graphics );
			}
		}
		#endregion

		#region Class Overrides

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override void SetChildControlBounds()
		{
			CommandBarDockState cbdockstate = this.DockState;
			if((this.cbController != null) && (cbdockstate != CommandBarDockState.None) && (this.Controls.Count > 0))
			{
				bool bRTL = this.IsRTL;
				Rectangle rcclient = this.ClientRectangle;
				int nClientRight = rcclient.Right;
				int nCaptionCtlWidth = 0;

				// If the Caption toolbar has been initialized with Items then display it in the ControlBar's caption area
				if(this.cptnControl != null)
				{
					nCaptionCtlWidth  = this.cptnControl.Width;
					if((cbdockstate == CommandBarDockState.Left) || (cbdockstate == CommandBarDockState.Right))
					{
						this.cptnControl.Location = new Point(
							bRTL ? nClientRight-2-nCaptionCtlWidth : 2, this.nCtrlBarCaptionHt+2);
					}
					else if((cbdockstate == CommandBarDockState.Top) || (cbdockstate == CommandBarDockState.Bottom))	// CommandBarDockState.Top||CommandBarDockstate.Bottom
					{
						int xoffset = this.nHeaderOff;
						// V-center the caption control
						int yoffset = (int)Math.Floor((float)(this.nCtrlBarCaptionHt - this.cptnControl.Height)/2f);
						this.cptnControl.Location = new Point(
							bRTL ? nClientRight-2-xoffset-nCaptionCtlWidth : 2+xoffset, 2+yoffset);
					}
					else	// (this.Floating == true)
					{
						this.cptnControl.Location =
							new Point( bRTL ? nClientRight-1-nCaptionCtlWidth : 1, this.nCtrlBarCaptionHt+1 );
					}
					if(this.cptnControl.Height > (this.nCtrlBarCaptionHt-2))
						this.cptnControl.Height = this.nCtrlBarCaptionHt-2;
				}

				Control clientctrl = this.ClientControl;
				if(clientctrl != null)
				{
					Size clientctrlsize;

					if((cbdockstate == CommandBarDockState.Top) || (cbdockstate == CommandBarDockState.Bottom))
					{
						clientctrlsize = new Size(rcclient.Width-4, rcclient.Height-4-this.nCtrlBarCaptionHt);
						clientctrl.Location = new Point(
							bRTL ? nClientRight-clientctrlsize.Width-2 : rcclient.Left+2,
							rcclient.Top+2+this.nCtrlBarCaptionHt );
					}
					else if((cbdockstate == CommandBarDockState.Left) || (cbdockstate == CommandBarDockState.Right))
					{
						if(this.cptnControl != null)
						{
							clientctrlsize = new Size(rcclient.Width-4, rcclient.Height-4-this.nCtrlBarCaptionHt-this.cptnControl.Height);
							clientctrl.Location = new Point(
								bRTL ? rcclient.Right-clientctrlsize.Width-2 : rcclient.Left+2,
								rcclient.Top+2+this.nCtrlBarCaptionHt+this.cptnControl.Height);
						}
						else
						{
							clientctrlsize = new Size(rcclient.Width-4, rcclient.Height-4-this.nCtrlBarCaptionHt);
							clientctrl.Location = new Point(
								bRTL ? rcclient.Right-clientctrlsize.Width-2 : rcclient.Left+2,
								rcclient.Top+2+this.nCtrlBarCaptionHt );
						}
					}
					else // CommandBarDockState.Float
					{
						if(this.cptnControl != null)
						{
							clientctrlsize = new Size(rcclient.Width-2, rcclient.Height-2-this.nCtrlBarCaptionHt-this.cptnControl.Height);
							clientctrl.Location = new Point(
								bRTL ? rcclient.Right-clientctrlsize.Width-1 : rcclient.Left+1,
								rcclient.Top+1+this.nCtrlBarCaptionHt+this.cptnControl.Height);
							
						}
						else
						{	
							clientctrlsize = new Size(rcclient.Width-2, rcclient.Height-2-this.nCtrlBarCaptionHt);
							clientctrl.Location = new Point(
								bRTL ? rcclient.Right-clientctrlsize.Width-1 : rcclient.Left+1,
								rcclient.Top+1+this.nCtrlBarCaptionHt);
						}
					}
					if((clientctrlsize.Width > 0) && (clientctrlsize.Height > 0))
						clientctrl.Size = clientctrlsize;
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override void ChangeCommandBarOrientation()
		{
			// Do nothing for ControlBar
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal override Size CalculateFloatingSize()
		{
			Size szfloat = Size.Empty;
			Rectangle rcfloat = this.rcFloat;
			if((rcfloat.Width == 0) || (rcfloat.Height == 0))
				szfloat = this.Size;
			else
				szfloat = rcfloat.Size;
			return szfloat;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override PopupRelativeAlignment GetFirstAlignPreference()
		{
			return this.IsRTL ? PopupRelativeAlignment.BottomLeft : PopupRelativeAlignment.BottomRight;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public override void HandleMouseMove(MouseButtons button, Point ptscreen)
		{
			if( CommandBar.bDragging == false )
			{
				Point ptclient = this.PointToClient( ptscreen );
				Rectangle rcclient = this.ClientRectangle;
				if( ControlBar.bDockedSizing == false )
				{
					Rectangle rcdragedge= Rectangle.Empty;
					Cursor sizingcursor = Cursors.Default;
					if( this.cbarDockState == CommandBarDockState.Top )
					{
						rcdragedge = new Rectangle( rcclient.Left, rcclient.Bottom-2, rcclient.Width, 2 );
						sizingcursor = Cursors.SizeNS;
					}
					else if( this.cbarDockState == CommandBarDockState.Bottom )
					{
						rcdragedge = new Rectangle( rcclient.Left, rcclient.Top, rcclient.Width, 2 );
						sizingcursor = Cursors.SizeNS;
					}
					else if( this.cbarDockState == CommandBarDockState.Left )
					{
						rcdragedge = new Rectangle( rcclient.Right-2, rcclient.Top, 2, rcclient.Height );
						sizingcursor = Cursors.SizeWE;
					}
					else if( this.cbarDockState == CommandBarDockState.Right )
					{
						rcdragedge = new Rectangle( rcclient.Left, rcclient.Top, 2, rcclient.Height );
						sizingcursor = Cursors.SizeWE;
					}

					if( rcdragedge.Contains( ptclient ) == true )
					{
						if( CommandBar.crDefault == null )
						{
							if( !this.DesignProcess && (this.Cursor != sizingcursor) )
							{
								CommandBar.crDefault = this.Cursor;
								this.Cursor = sizingcursor;
							}
							return;
						}
					}
					else
					{
						if( this.Cursor == sizingcursor )
						{
							if( CommandBar.crDefault != null )
							{
								this.Cursor = CommandBar.crDefault;
								CommandBar.crDefault = null;
							}
							else
								this.Cursor = Cursors.Default;
						}
					}
				}
				else	// (bDockedSizing == true)
				{
					if( this.cbarDockState == CommandBarDockState.Top )
					{
						int deltaY = ptscreen.Y - ControlBar.ptSizingStart.Y;
						int newheight = this.Height + deltaY;

						if( (newheight > this.minSize.Height) && (newheight < this.maxSize.Height) )
						{
							// Increase the size of the parent dock bar and displace all CommandBars in the next row by an equivalent distance
							this.cdbParent.Height += deltaY;
							for( int i=(this.nRCIndex+1); i<this.cdbParent.nRCCount; i++ )
							{
								CommandBar[] cbarray = this.cdbParent.GetRowArray( i );
								foreach( CommandBar cbar in cbarray )
									cbar.Location = new Point( cbar.Location.X, cbar.Location.Y+deltaY );
							}
							this.nCommandBarHt = newheight;
							this.Height = newheight;
							ControlBar.ptSizingStart = ptscreen;
							this.Refresh();
						}
					}
					else if( this.cbarDockState == CommandBarDockState.Bottom )
					{
						int deltaY = ControlBar.ptSizingStart.Y - ptscreen.Y;
						int newheight = this.Height + deltaY;

						if( (newheight > this.minSize.Height) && (newheight < this.maxSize.Height) )
						{
							// Increase the size of the parent dock bar and displace all CommandBars in the previous row by an equivalent distance
							this.cdbParent.Height += deltaY;
							for( int i=(this.nRCIndex+1); i<this.cdbParent.nRCCount; i++ )
							{
								CommandBar[] cbarray = this.cdbParent.GetRowArray( i );
								foreach( CommandBar cbar in cbarray )
									cbar.Location = new Point( cbar.Location.X, cbar.Location.Y+deltaY );
							}
							this.nCommandBarHt = newheight;
							this.Height = newheight;
							ControlBar.ptSizingStart = ptscreen;
							this.Refresh();
						}
					}
					else if( this.cbarDockState == CommandBarDockState.Left )
					{
						int deltaX = ptscreen.X - ControlBar.ptSizingStart.X;
						int newwidth = this.Width + deltaX;

						if( (newwidth > this.minSize.Width) && (newwidth < this.maxSize.Width) )
						{
							// Increase the size of the parent dock bar and displace all CommandBars in the next row by the same distance
							this.cdbParent.Width += deltaX;
							for( int i=this.nRCIndex+1; i<this.cdbParent.nRCCount; i++ )
							{
								CommandBar[] cbarray = this.cdbParent.GetRowArray( i );
								foreach( CommandBar cbar in cbarray )
									cbar.Location = new Point( cbar.Location.X+deltaX, cbar.Location.Y );
							}
							this.nCommandBarHt = newwidth;
							this.Width = newwidth;
							ControlBar.ptSizingStart = ptscreen;
							this.Refresh();
						}
					}
					else if( this.cbarDockState == CommandBarDockState.Right )
					{
						int deltaX = ControlBar.ptSizingStart.X - ptscreen.X;
						int newwidth = this.Width + deltaX;

						if( (newwidth > this.minSize.Width) && (newwidth < this.maxSize.Width) )
						{
							// Change the size of the parent dock bar and displace all CommandBars in the next row by the same distance
							this.cdbParent.Width += deltaX;
							for( int i=(this.nRCIndex+1); i<this.cdbParent.nRCCount; i++ )
							{
								CommandBar[] cbarray = this.cdbParent.GetRowArray( i );
								foreach( CommandBar cbar in cbarray )
									cbar.Location = new Point( cbar.Location.X+deltaX, cbar.Location.Y );
							}
							this.nCommandBarHt = newwidth;
							this.Width = newwidth;
							ControlBar.ptSizingStart = ptscreen;
							this.Refresh();
						}
					}
					return;
				}
			}
			base.HandleMouseMove( button, ptscreen );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public override void HandleMouseDown(MouseButtons button, Point ptscreen)
		{
			Point ptclient = this.PointToClient(ptscreen);
			Rectangle rcclient = this.ClientRectangle;
			Rectangle rcdragedge= Rectangle.Empty;
			if(this.cbarDockState == CommandBarDockState.Top)
				rcdragedge = new Rectangle(rcclient.Left, rcclient.Bottom-2, rcclient.Width, 2);
			else if(this.cbarDockState == CommandBarDockState.Bottom)
				rcdragedge = new Rectangle(rcclient.Left, rcclient.Top, rcclient.Width, 2);
			else if(this.cbarDockState == CommandBarDockState.Left)
				rcdragedge = new Rectangle(rcclient.Right-2, rcclient.Top, 2, rcclient.Height);
			else if(this.cbarDockState == CommandBarDockState.Right)
				rcdragedge = new Rectangle(rcclient.Left, rcclient.Top, 2, rcclient.Height);

			if(rcdragedge.Contains(ptclient))
			{
				ControlBar.bDockedSizing = true;
				ControlBar.ptSizingStart = ptscreen;
			}
			else
				base.HandleMouseDown(button, ptscreen);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public override void HandleMouseUp(MouseButtons button, Point ptscreen)
		{
			if(ControlBar.bDockedSizing == true)
			{
				ControlBar.bDockedSizing = false;
				ControlBar.ptSizingStart = Point.Empty;
			}
			base.HandleMouseUp(button, ptscreen);
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnControlRemoved"/>.
		/// </summary>
		protected override void OnControlRemoved(ControlEventArgs arg)
		{
			base.OnControlRemoved(arg);

			if(arg.Control == this.cptnControl)
				this.cptnControl = null;
		}

		/// <summary>
		/// Gets rectangle for display text of the ControlBar.
		/// </summary>
		protected internal override Rectangle GetTextRectangle()
		{
			Rectangle textRect = Rectangle.Empty;
			bool bRTL = this.IsRTL;

			int iButtonHeight = CommandBar.CaptionHeight;
			int iTrailWidth = 0;
			
			if( this.bHideDropDown == false )
			{
				iTrailWidth += iButtonHeight;
			}

			if( this.bHideCloseButton == false )
			{
				iTrailWidth += iButtonHeight;
			}

			Rectangle captionRect = this.CaptionRect;

			int iLeftOffset = this.nHeaderOff + 2;
			int iCtrlBarCaptionHt = this.nCtrlBarCaptionHt;
			if( ( this.Floating == false ) && 
				( this.cdbParent.Dock == DockStyle.Top || this.cdbParent.Dock == DockStyle.Bottom ) )
			{
				if( this.cptnControl != null )
				{
					iLeftOffset += this.cptnControl.Width + 2;
				}
			}

			Size textSize = new Size( captionRect.Width - 4 - iLeftOffset - iTrailWidth, 
				captionRect.Height );
			Point ptLocation = new Point( bRTL ? captionRect.Right - textSize.Width - iLeftOffset : 
				captionRect.Left + iLeftOffset, captionRect.Top );
			textRect = new Rectangle( ptLocation, textSize );

			return textRect;
		}

		#endregion	
	}
}
