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

#region file using directives
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.ComponentModel;
using Syncfusion.Windows.Forms.Interop;
using Syncfusion.Windows.Forms.Renderers;
using Syncfusion.Windows.Forms.Tools;

#endregion

namespace Syncfusion.Windows.Forms
{
	#region *** ScrollersFrame
	/// <summary>
	/// Custom scrollbars with various VisualStyles and provision to add buttons to it.
	/// </summary>

	[
	ToolboxItem( true ),
	TypeConverter( typeof( ScrollersFrame.VisualStyleTypeConverter ) ),
	Designer( typeof( ScrollersFrameDesigner ) ),
	Description( "Provides custom scrollbars with various VisualStyles" ),
	ToolboxBitmap( typeof( PopupControlContainer ), "ToolboxIcons.ScrollersFrame.bmp" )
	]
	public class ScrollersFrame:
	  Component,
	  IMessageFilter,
      IVisualStyle 
	{
		#region Class constants
		/// <summary></summary>
		private enum ScrollBarComponents
		{
			/// <summary>The scroll bar itself.</summary>
			ScrollBar=0,
			/// <summary>The top or right arrow button.</summary>
			TopRightArrow=1,
			/// <summary>The page up or page right region.</summary>
			TopRightRegion=2,
			/// <summary>The scroll box (thumb).</summary>
			Thumb=3,
			/// <summary>The page down or page left region.</summary>
			BottomLeftRegion=4,
			/// <summary>The bottom or left arrow button.</summary>
			BottomLeftArrow=5
		}

		/// <summary></summary>
		private const string PROPERTY_NAME = "OfficeColorScheme";
		#endregion

		#region Class members
		/// <summary>Subclasser that helps us to monitor control changes.</summary>
		private NativeWindowEx m_nativewnd;
		/// <summary>Reference on control to which we attach scrollers.</summary>
		private Control m_parent;
		/// <summary></summary>
		private NativeMethods.SCROLLINFO m_hScroll;
		/// <summary></summary>
		private NativeMethods.SCROLLINFO m_vScroll;
        /// <summary></summary>
        private NativeMethods.SCROLLBARINFO m_hScrollBar;
        /// <summary></summary>
        private NativeMethods.SCROLLBARINFO m_vScrollBar;
        /// <summary></summary>
        private Control m_oldParentParent;
        /// <summary></summary>
        private ScrollBarCustomDrawStyles m_styles = ScrollBarCustomDrawStyles.Classic;
        /// <summary> Color scheme that used in Rendering. </summary>
        private Office2007ColorScheme m_ColorScheme = Office2007ColorScheme.Blue;
        /// <summary> Color scheme that used in Rendering. </summary>
        private Office2010ColorScheme m_ColorScheme2010 = Office2010ColorScheme.Blue;
        /// <summary> Color scheme that used in Rendering. </summary>
        private MetroColorScheme m_MetroColorScheme = MetroColorScheme.Managed;
        /// <summary>Gripper visibility behavior.</summary>
		private SizeGripperVisibility m_gripperVisibility = SizeGripperVisibility.Auto;
		/// <summary>
		/// Indicates if a Parent control is in dragging mode.
		/// </summary>
		private bool m_bParentInDragging = false;
		/// <summary>
		/// Indicates if left mouse button is pressed.
		/// </summary>
		private bool m_bLeftMouseButtonDown = false;
		/// <summary>
		/// Enables delayed scrollbar updates. Default value is true.
		/// </summary>
		private bool m_bEnableDelayedScrollBarUpdate = true;
		/// <summary></summary>
		private IAsyncResult m_iarSynchronizeScrollbars;
		private MethodInvoker m_miSynchronizeScrollbarsHandler;
		/// <summary>
		/// Collection of all attached controls and corresponding <see cref="ScrollersFrame"/> components.
		/// </summary>
		private static IDictionary s_attachedControls = new HybridDictionary();
		public IRenderer m_customrenderer;
		#endregion

		#region Class properties
		/// <summary>Reference on control to which we assign our custom scrollers.</summary>
		[
		Browsable( true ),
		Category( "Behavior" ),
		DefaultValue( null ),
		Description( "Reference on control to which we assign our custom scrollers." ),
		TypeConverter( typeof( AttachedToControlTypeConverter ) )
		]
		public Control AttachedTo
		{
			get
			{
				return m_parent;
			}
			set
			{
				if( m_parent != value )
				{
					DetachFrame();

					if( value != null )
					{
						AttachFrame( value );
					}
				}
			}
		}

		/// <summary>Reference on configurable Horizontal Scroller control.</summary>
		[
		Browsable( false ),
		Description( "Reference on configurable Horizontal Scroller control." ),
		Category( "Behavior" ),
		]
		public HScrollBarCustomDraw HorizontalScroller
		{
			get
			{
				return hScroller;
			}
		}
		/// <summary>Reference on configurable Vertical Scroller control.</summary>
		[
		Browsable( false ),
		Description( "Reference on configurable Vertical Scroller control." ),
		Category( "Behavior" ),
		]
		public VScrollBarCustomDraw VerticalScroller
		{
			get
			{
				return vScroller;
			}
		}
		/// <summary>
		/// Gets or sets visibility of size gripper.
		/// </summary>
		[
		Browsable( true ),
		Category( "Appearance" ),
		DefaultValue( typeof( SizeGripperVisibility ), "Smart" ),
		Description( "Indicates visibility of size gripper." )
		]
		public SizeGripperVisibility SizeGripperVisibility
		{
			get
			{
				return m_gripperVisibility;
			}
			set
			{
				if( m_gripperVisibility != value )
				{
					m_gripperVisibility = value;
					Update();
				}
			}
		}
		/// <summary>True - horizontal scroller is visible to user, otherwise False.</summary>
		protected bool IsHorizontalScrollVisible
		{
			get
			{
				int style = NativeMethods.GetWindowLong( m_parent.Handle, NativeMethods.GWL_STYLE );
				bool bVisible = ( style & NativeMethods.WS_HSCROLL ) != 0;

				if( bVisible )
				{
					NativeMethods.GetScrollBarInfo( m_parent.Handle, NativeMethods.OBJID_HSCROLL, ref m_hScrollBar );

					int index = (int)ScrollBarComponents.ScrollBar;
					int scrollbarState = m_hScrollBar.rgstate[index];
					if( scrollbarState != 0 )
					{
						bVisible = ( scrollbarState & (int)NativeMethods.StateSystem.STATE_SYSTEM_INVISIBLE ) == 0;
					}
				}

				return bVisible;
			}
		}
		/// <summary>True - vertical scroller is visible to user, otherwise False.</summary>
		protected bool IsVerticalScrollVisible
		{
			get
			{
				int style = NativeMethods.GetWindowLong( m_parent.Handle, NativeMethods.GWL_STYLE );
				bool bVisible = ( style & NativeMethods.WS_VSCROLL ) != 0;

				if( bVisible )
				{
					NativeMethods.GetScrollBarInfo( m_parent.Handle, NativeMethods.OBJID_VSCROLL, ref m_vScrollBar );

					int index = (int)ScrollBarComponents.ScrollBar;
					int scrollbarState = m_vScrollBar.rgstate[index];
					if( scrollbarState != 0 )
					{
						bVisible = ( scrollbarState & (int)NativeMethods.StateSystem.STATE_SYSTEM_INVISIBLE ) == 0;
					}
				}

				return bVisible;
			}
		}
		/// <summary>Specifies the style of appearance.</summary>
		[
		Description( "Specifies the style of appearance." ),
		Category( "Appearance - Styles" ),
		DefaultValue( typeof( ScrollBarCustomDrawStyles ), "Classic" ),
		RefreshProperties( RefreshProperties.All )
		]
		public ScrollBarCustomDrawStyles VisualStyle
		{
			get
			{
				return m_styles;
			}
			set
			{
				if( m_styles != value )
				{
					m_styles = value;

                    bool bMetroColorSchemeUsed = (value == ScrollBarCustomDrawStyles.Metro);
                    bool bOfficeColorSchemeUsed = (value == ScrollBarCustomDrawStyles.Office2007);
                    bool bOffice2010ColorSchemeUsed = (value == ScrollBarCustomDrawStyles.Office2010);

					if( hScroller != null )
					{
						if( bOfficeColorSchemeUsed )
						{
							hScroller.OfficeColorScheme = this.OfficeColorScheme;
						}
                        else if (bOffice2010ColorSchemeUsed)
                        {
                            hScroller.Office2010ColorScheme = this.Office2010ColorScheme;
                        }
                        else if (bMetroColorSchemeUsed)
                            hScroller.MetroColorScheme = this.MetroColorScheme;

						hScroller.VisualStyle = value;
						hScroller.Invalidate();
					}

					if( vScroller != null )
					{
						if( bOfficeColorSchemeUsed )
						{
							vScroller.OfficeColorScheme = this.OfficeColorScheme;
						}
                        else if (bOffice2010ColorSchemeUsed)
                        {
                            vScroller.Office2010ColorScheme = this.Office2010ColorScheme;
                        }
                        else if (bMetroColorSchemeUsed)
                        {
                            vScroller.MetroColorScheme = this.MetroColorScheme;
                        }
                        vScroller.VisualStyle = value;
                        vScroller.Invalidate();
                    }
                }

                m_sizeGripper.Invalidate();
            }
        }
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
                if (VisualStyle == ScrollBarCustomDrawStyles.Office2007 || VisualStyle == ScrollBarCustomDrawStyles.Office2007Generic )
                {
                    if (value == "Office2007Blue")
                       OfficeColorScheme = Office2007ColorScheme.Blue ;
                    else if (value == "Office2007Silver")
                        OfficeColorScheme = Office2007ColorScheme.Silver;
                    else if (value == "Office2007Black")
                        OfficeColorScheme = Office2007ColorScheme.Black;
                    else if (value == "Managed")
                        OfficeColorScheme = Office2007ColorScheme.Managed;
                }
                else if (VisualStyle == ScrollBarCustomDrawStyles.Office2010)
                {
                    if (value == "Office2007Blue")
                        Office2010ColorScheme = Office2010ColorScheme.Blue;
                    else if (value == "Office2007Silver")
                        Office2010ColorScheme = Office2010ColorScheme.Silver;
                    else if (value == "Office2007Black")
                        Office2010ColorScheme = Office2010ColorScheme.Black;
                    else if (value == "Managed")
                        Office2010ColorScheme = Office2010ColorScheme.Managed;
                }
                else if (VisualStyle == ScrollBarCustomDrawStyles.Metro)
                {
                    if (value == "MetroMagenta")
                        MetroColorScheme = MetroColorScheme.Magenta;
                    else if (value == "MetroPurple")
                        MetroColorScheme = MetroColorScheme.Purple;
                    else if (value == "MetroOrange")
                        MetroColorScheme = MetroColorScheme.Orange;
                    else if (value == "MetroPink")
                        MetroColorScheme = MetroColorScheme.Pink;
                    else if (value == "MetroLime")
                        MetroColorScheme = MetroColorScheme.Lime;
                    else if (value == "MetroBrown")
                        MetroColorScheme = MetroColorScheme.Brown;
                    else if (value == "MetroRed")
                        MetroColorScheme = MetroColorScheme.Red;
                    else if (value == "MetroBlue")
                        MetroColorScheme = MetroColorScheme.Blue;
                    else if (value == "MetroGreen")
                        MetroColorScheme = MetroColorScheme.Green;
                    else if (value == "MetroTeal")
                        MetroColorScheme = MetroColorScheme.Teal;
                    else if (value == "MetroManaged")
                        MetroColorScheme = MetroColorScheme.Managed;
                }
            }
        }
        /// <summary>
        /// Gets or sets whether the Metro color scheme should be User defined colors.
        /// </summary>
        [
        Category("Appearance - Styles"),
        Description("Specifies the user defined color scheme")
        ]
        public MetroColorScheme MetroColorScheme
        {
            get
            {
                return m_MetroColorScheme;
            }
            set
            {
                if (m_MetroColorScheme != value)
                {
                    m_MetroColorScheme = value;

                    if (hScroller != null)
                    {
                        hScroller.MetroColorScheme = value;
                        hScroller.Invalidate();
                    }

                    if (vScroller != null)
                    {
                        vScroller.MetroColorScheme = value;
                        vScroller.Invalidate();
                    }

                    m_sizeGripper.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets whether the Office color scheme should be Silver or Blue or Black.
        /// </summary>
        [
        Category("Appearance - Styles"),
        Description("Specifies the color scheme (Silver, Blue, Black).")
        ]
        public Office2007ColorScheme OfficeColorScheme
        {
            get
            {
                return m_ColorScheme;
            }
            set
            {
                if (m_ColorScheme != value)
                {
                    m_ColorScheme = value;

                    if (hScroller != null)
                    {
                        hScroller.OfficeColorScheme = value;
                        hScroller.Invalidate();
                    }

                    if (vScroller != null)
                    {
                        vScroller.OfficeColorScheme = value;
                        vScroller.Invalidate();
                    }

                    m_sizeGripper.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the Office color scheme should be Silver or Blue or Black.
        /// </summary>
        [
        Category("Appearance - Styles"),
        Description("Specifies the office 2010 color scheme (Silver, Blue, Black).")
        ]
        public Office2010ColorScheme Office2010ColorScheme
        {
            get
            {
                return m_ColorScheme2010;
            }
            set
            {
                if (m_ColorScheme2010 != value)
                {
                    m_ColorScheme2010 = value;

                    if (hScroller != null)
                    {
                        hScroller.Office2010ColorScheme = value;
                        hScroller.Invalidate();
                    }

                    if (vScroller != null)
                    {
                        vScroller.Office2010ColorScheme = value;
                        vScroller.Invalidate();
                    }

                    m_sizeGripper.Invalidate();
                }
            }
        }
		/// <summary>
		/// Gets or sets the value to be added to or subtracted from the Value property when the horizontal scroll box is moved a small distance. 
		/// </summary>
		[
		Category( "Behavior" ),
		Description( "Gets or sets the value to be added to or subtracted from the Value property when the horizontal scroll box is moved a small distance." ),
		DefaultValue( 1 )
		]
		public int HorizontalSmallChange
		{
			get
			{
				if( hScroller != null )
				{
					return hScroller.SmallChange;
				}

				return 1;
			}
			set
			{
				if( hScroller != null )
				{
					hScroller.SmallChange = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets the value to be added to or subtracted from the Value property when the vertical scroll box is moved a small distance. 
		/// </summary>
		[
		Category( "Behavior" ),
		Description( "Gets or sets the value to be added to or subtracted from the Value property when the vertical scroll box is moved a small distance." ),
		DefaultValue( 1 )
		]
		public int VerticallSmallChange
		{
			get
			{
				if( vScroller != null )
				{
					return vScroller.SmallChange;
				}

				return 1;
			}
			set
			{
				if( vScroller != null )
				{
					vScroller.SmallChange = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets whether delayed scrollbar updates must be updated.
		/// </summary>
		[DefaultValue( true )]
		[Description( "Gets or sets whether delayed scrollbar updates must be updated." )]
		public virtual bool EnableDelayedScrollBarUpdate
		{
			get { return m_bEnableDelayedScrollBarUpdate; }
			set { m_bEnableDelayedScrollBarUpdate = value; }
		}
		/// <summary>
		/// Gets or sets value indicating whether scrollbar should be refreshed on each value change.
		/// If set to false, scrollbar is invalidated only and therefore is visually refreshed after processing all scrolling messages.
		/// </summary>
		[
		Category( "Behavior" ),
		Description( @"Indicates whether scrollbar should be refreshed on each value change. If set to false, scrollbar is invalidated only and therefore is visually refreshed after processing all scrolling messages." ),
		DefaultValue( false ),
		]
		public bool RefreshOnValueChange
		{
			get
			{
				return vScroller.RefreshOnValueChange;
			}
			set
			{
				vScroller.RefreshOnValueChange = value;
				hScroller.RefreshOnValueChange = value;
			}
		}
		#endregion

		#region Form controls
		/// <summary>Horizontal scroller instance.</summary>
		protected HScrollBarCustomDraw hScroller;
		/// <summary>Vertical scroller instance.</summary>
		protected VScrollBarCustomDraw vScroller;
		/// <summary>Size gripper control instance.</summary>
		protected SizeGripperAdv m_sizeGripper;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private Container components = null;
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>Default constructor.</summary>
		public ScrollersFrame()
		{
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(ScrollersFrame));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// attach internal events for processing scrolling
			hScroller.Scroll += new ScrollEventHandler( OnScrollerScroll );
			vScroller.Scroll += new ScrollEventHandler( OnScrollerScroll );
		}

		/// <summary>Component oriented constructor. Mostly used by Windows Forms
		/// designer.</summary>
		/// <param name="container">reference on container that will control 
		/// class life time.</param>
		public ScrollersFrame( IContainer container )
			: this()
		{
			container.Add( this );
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing"/>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
				{
					components.Dispose();
				}

				DetachFrame();

				if( hScroller != null )
				{
					hScroller.Scroll -= new ScrollEventHandler( OnScrollerScroll );
					hScroller.Dispose();
					hScroller = null;

					vScroller.Scroll -= new ScrollEventHandler( OnScrollerScroll );
					vScroller.Dispose();
					vScroller = null;
				}

				if( m_sizeGripper != null )
				{
					m_sizeGripper.Dispose();
					m_sizeGripper.DropHandle();
					m_sizeGripper = null;
				}

				EndInvokeSynchronizeScrollbars();
				m_miSynchronizeScrollbarsHandler = null;
			}

			base.Dispose( disposing );
		}
		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.hScroller = new Syncfusion.Windows.Forms.HScrollBarCustomDraw( this );
			this.vScroller = new Syncfusion.Windows.Forms.VScrollBarCustomDraw( this );
			this.m_sizeGripper = new Syncfusion.Windows.Forms.ScrollersFrame.SizeGripperAdv( this );
			// 
			// hScroller
			// 
			this.hScroller.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left )
		| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.hScroller.Location = new System.Drawing.Point( 0, 144 );
			this.hScroller.Name = "hScroller";
			this.hScroller.Size = new System.Drawing.Size( 224, 17 );
			this.hScroller.TabIndex = 0;
			this.hScroller.ThemeEnabled = true;
			this.hScroller.Value = 10;
			this.hScroller.VisualStyle = Syncfusion.Windows.Forms.ScrollBarCustomDrawStyles.Classic;
			// 
			// vScroller
			// 
			this.vScroller.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
		| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.vScroller.Location = new System.Drawing.Point( 224, 0 );
			this.vScroller.Name = "vScroller";
			this.vScroller.Size = new System.Drawing.Size( 16, 144 );
			this.vScroller.TabIndex = 1;
			this.vScroller.ThemeEnabled = true;
			this.vScroller.Value = 10;
			this.vScroller.VisualStyle = Syncfusion.Windows.Forms.ScrollBarCustomDrawStyles.Classic;
			// 
			// m_sizeGripper
			// 
			this.m_sizeGripper.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.m_sizeGripper.Location = new System.Drawing.Point( 224, 144 );
			this.m_sizeGripper.Name = "sizeGripper";
			this.m_sizeGripper.Size = new System.Drawing.Size( 17, 17 );
			this.m_sizeGripper.TabIndex = 2;
		}
		#endregion

		#region Class codedom serialization
		/// <summary>
		/// Indicates whether the current value of the OfficeColorScheme property is to be serialized.
		/// </summary>
		/// <returns></returns>
		protected virtual bool ShouldSerializeVisualStyle()
		{
			return ( this.VisualStyle != ScrollBarCustomDrawStyles.Classic );
		}

		/// <summary>
		/// Resets the office color scheme.
		/// </summary>
		protected virtual void ResetVisualStyle()
		{
			this.VisualStyle = ScrollBarCustomDrawStyles.Classic;
		}

        /// <summary>
        /// Indicates whether the current value of the OfficeColorScheme property is to be serialized.
        /// </summary>
        /// <returns></returns>
        protected virtual bool ShouldSerializeOfficeColorScheme()
        {
            return (this.OfficeColorScheme != Office2007ColorScheme.Blue && this.MetroColorScheme != MetroColorScheme.Magenta);
        }

        /// <summary>
		/// Indicates whether the current value of the Office2010ColorScheme property is to be serialized.
		/// </summary>
		/// <returns>a boolean value.</returns>
		protected virtual bool ShouldSerializeOffice2010ColorScheme()
		{
			return ( this.Office2010ColorScheme != Office2010ColorScheme.Blue );
		}

        /// <summary>
        /// Resets the office color scheme.
        /// </summary>
        protected virtual void ResetOfficeColorScheme()
        {
            this.OfficeColorScheme = Office2007ColorScheme.Blue;
            this.MetroColorScheme = MetroColorScheme.Magenta;
        }

        /// <summary>
        /// Resets the office2010 color scheme.
        /// </summary>
        protected virtual void ResetOffice2010ColorScheme()
        {
            this.Office2010ColorScheme = Office2010ColorScheme.Blue;
        }

		#endregion

		#region Class event handlers
		/// <summary>Method called when parent control create own window handle.</summary>
		/// <param name="sender">reference on parent control.</param>
		/// <param name="e">Event arguments.</param>
		private void parent_HandleCreated( object sender, EventArgs e )
		{
			AttachScrollers();
		}

		/// <summary>Method called when control destroy own window handle.</summary>
		/// <param name="sender">reference on parent control.</param>
		/// <param name="e">Event arguments.</param>
		private void parent_HandleDestroyed( object sender, EventArgs e )
		{
			DetachScrollers();
		}

		/// <summary>Method called when detected parent RightToLeft property value changes.</summary>
		/// <param name="sender">reference on parent control.</param>
		/// <param name="e">Event arguments.</param>
		private void parent_RightToLeftChanged( object sender, EventArgs e )
		{
			ReflectRightToLeft();
		}

		void m_parent_MouseDown(object sender, MouseEventArgs e)
		{
			m_bLeftMouseButtonDown = true;
		}

		void m_parent_MouseUp(object sender, MouseEventArgs e)
		{
			m_bParentInDragging = m_bLeftMouseButtonDown = false;
		}

		/// <summary></summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnScrollerScroll( object sender, ScrollEventArgs e )
		{
			if( m_parent != null && m_parent.IsHandleCreated )
			{
				// The most correct way to set scroll info is using SetScrollInfo function, because 
				// it supports 32 bit scroll position values. But there are some controls that don't 
				// use GetScrollInfo, but just use wParam from scroll messages (such as RichTextBox).
				// So the best approach to use here is to call SetScrollInfo and send scroll message after that.
				// Correct behavior for control is to call GetScrollInfo on receiving scroll message.
				// This approach doesn't work with "more than 16 bit" values if control doesn't use GetScrollInfo function.

				NativeMethods.SCROLLINFO scrollInfo = new NativeMethods.SCROLLINFO();

				scrollInfo.cbSize = Marshal.SizeOf( typeof( NativeMethods.SCROLLINFO ) );
				scrollInfo.fMask = NativeMethods.SIF_ALL;
				int fnBar = ( sender == hScroller ) ? NativeMethods.SB_HORZ : NativeMethods.SB_VERT;
				bool res = NativeMethods.GetScrollInfo( m_parent.Handle, fnBar, ref scrollInfo );

				if( res )
				{
					if( m_parent is ListView )
					{
						// ListView uses the LVM_SCROLL message for scrolling instead of WM_HSCROLL/WM_VSCROLL messages.
						OnScrollerScrollListView( e, scrollInfo, fnBar );
					}
					else
					{
						scrollInfo.nPos = e.NewValue;
						NativeMethods.SetScrollInfo( m_parent.Handle, fnBar, ref scrollInfo, true );

						IntPtr wParam = new IntPtr( NativeMethods.SB_THUMBPOSITION + 0x10000 * e.NewValue );
						int wmMsg = ( fnBar == NativeMethods.SB_HORZ ) ? NativeMethods.WM_HSCROLL : NativeMethods.WM_VSCROLL;

						NativeMethods.PostMessage( m_parent.Handle, wmMsg, wParam, IntPtr.Zero );
					}
				}
			}
		}

		private void OnScrollerScrollListView( ScrollEventArgs e, NativeMethods.SCROLLINFO scrollInfo, int fnBar )
		{
			ListView listView = (ListView)m_parent;
			int value = 0, divisor = 1;

			if( fnBar == NativeMethods.SB_HORZ )
			{
				OnScrollerScrollListViewHorz( e, ref scrollInfo, listView, ref value );
			}
			else
			{
				OnScrollerScrollListViewVert( e, ref scrollInfo, listView, ref value, ref divisor );
			}

			scrollInfo.nPos += value / divisor;
			NativeMethods.SetScrollInfo( m_parent.Handle, fnBar, ref scrollInfo, true );
		}

		private void OnScrollerScrollListViewVert( ScrollEventArgs e, ref NativeMethods.SCROLLINFO scrollInfo, ListView listView, ref int value, ref int divisor )
		{
			// When the list-view control is in details view, the control can only be scrolled vertically 
			// in whole line increments. Therefore, the value will be rounded to the nearest number 
			// of pixels that form a whole line increment.                   
			if( listView.View == View.Details )
			{
				if( listView.Items.Count > 0 )
				{
					Rectangle rect = listView.GetItemRect( 0 );

                    if (rect.Height != 0)
						divisor = rect.Height;

					switch( e.Type )
					{
						case ScrollEventType.LargeDecrement:
						value = -scrollInfo.nPage * rect.Height;
						this.vScroller.LargeChange = -value;
						break;

						case ScrollEventType.LargeIncrement:
						value = scrollInfo.nPage * rect.Height;
						this.vScroller.LargeChange = value;
						break;

						case ScrollEventType.SmallDecrement:
						value = -rect.Height;
						this.vScroller.SmallChange = -value;
						break;

						case ScrollEventType.SmallIncrement:
						value = rect.Height;
						this.vScroller.SmallChange = value;
						break;

						default:
						value = ( e.NewValue - scrollInfo.nPos ) * rect.Height;
						break;
					}
				}
			}
			else
			{
				value = e.NewValue - scrollInfo.nPos;
			}

			NativeMethods.SendMessage( m_parent.Handle, NativeMethods.LVM_SCROLL, IntPtr.Zero, (IntPtr)value );
		}

		private void OnScrollerScrollListViewHorz( ScrollEventArgs e, ref NativeMethods.SCROLLINFO scrollInfo, ListView listView, ref int value )
		{
			// When the list-view control is in List view value specifies the number of columns to scroll, 
			// else value specifies the number of pixels to scroll.    
			if( listView.View == View.List )
			{
				switch( e.Type )
				{
					case ScrollEventType.LargeDecrement:
					value = -1;
					// If the list-view control is in list-view, this value specifies the number of columns to scroll.
					this.hScroller.LargeChange = 1;
					break;

					case ScrollEventType.LargeIncrement:
					value = 1;
					// If the list-view control is in list-view, this value specifies the number of columns to scroll.
					this.hScroller.LargeChange = 1;
					break;

					case ScrollEventType.SmallDecrement:
					value = -1;
					this.hScroller.SmallChange = 1;
					break;

					case ScrollEventType.SmallIncrement:
					value = 1;
					this.hScroller.SmallChange = 1;
					break;

					default:
					value = e.NewValue - scrollInfo.nPos;
					break;
				}
			}
			else
			{
				value = e.NewValue - scrollInfo.nPos;
			}

			NativeMethods.SendMessage( m_parent.Handle, NativeMethods.LVM_SCROLL, (IntPtr)value, IntPtr.Zero );
		}


		/// <summary>Method called when detected changes in parent-child hierarchy.</summary>
		/// <param name="sender">reference on parent control.</param>
		/// <param name="e">Event arguments.</param>
		private void parent_ParentChanged( object sender, EventArgs e )
		{
			Control newParent = ( (Control)sender ).Parent;

			if( m_oldParentParent != null )
			{
				DetachScrollers();
			}

			m_oldParentParent = newParent;

			if( m_oldParentParent != null )
			{
				AttachScrollers();
			}
		}
		#endregion

		#region Class Public Methods
		/// <summary>Method attach scroller frame to the specified control. Previously 
		/// attaches to the controls will be released.</summary>
		/// <param name="control">Reference on windows forms control. Can not be NULL.</param>
		public void AttachFrame( Control control )
		{
			if( null == control )
			{
				throw new ArgumentNullException( "control" );
			}

			// if we have reAttaching then detach from old frame first
			if( m_parent != null )
			{
				DetachFrame();
			}

			m_parent = control;
			m_oldParentParent = m_parent.Parent;

			m_parent.HandleCreated += new EventHandler( parent_HandleCreated );
			m_parent.ParentChanged += new EventHandler( parent_ParentChanged );
			m_parent.HandleDestroyed += new EventHandler( parent_HandleDestroyed );
			m_parent.RightToLeftChanged += new EventHandler( parent_RightToLeftChanged );

            m_parent.VisibleChanged += new EventHandler(m_parent_VisibleChanged);
			m_parent.MouseDown += new MouseEventHandler(m_parent_MouseDown);
			m_parent.MouseUp += new MouseEventHandler(m_parent_MouseUp);

			AttachScrollers();

        if (!(s_attachedControls.Contains(control)))
                s_attachedControls.Add(control, this);
		}

        void m_parent_VisibleChanged(object sender, EventArgs e)
        {
            if (!m_parent.Visible)
            {
                this.vScroller.Visible = m_parent.Visible;
                this.hScroller.Visible = m_parent.Visible;
            }
        }
       /// <summary>
       /// Gets or Sets custom renderer to customize the scroll bars
       /// </summary>
        public IRenderer CustomRender
        {
            get
            {
                return m_customrenderer;
            }
            set
            {
                if (null != value && value != m_customrenderer)
                {
                    m_customrenderer = value;
                    this.hScroller.InternalRender = m_customrenderer;
                    this.vScroller.InternalRender = m_customrenderer;
                }
            }
        }
		/// <summary>Detach scrollers frame from previously attached control.</summary>
		public void DetachFrame()
		{
			if( m_parent != null )
			{
				DetachScrollers();

				m_parent.HandleCreated -= new EventHandler( parent_HandleCreated );
				m_parent.HandleDestroyed -= new EventHandler( parent_HandleDestroyed );
				m_parent.ParentChanged -= new EventHandler( parent_ParentChanged );
				m_parent.RightToLeftChanged -= new EventHandler( parent_RightToLeftChanged );

                m_parent.VisibleChanged -= new EventHandler(m_parent_VisibleChanged);
				m_parent.MouseDown -= new MouseEventHandler(m_parent_MouseDown);
				m_parent.MouseUp -= new MouseEventHandler(m_parent_MouseUp);

				s_attachedControls.Remove( m_parent );

				m_parent = null;
			}
		}

		/// <summary>Refresh scroller frames internal settings and repaint.</summary>
		public void Update()
		{
			SynchronizeScrollbarsPosition();
		}
		#endregion

		#region Class utility methods
		/// <summary>Internal scrollers attaching algorithm.</summary>
		private void AttachScrollers()
		{
			if( m_parent.IsHandleCreated && m_oldParentParent != null )
			{
				if( null == m_parent.Parent )
				{
					throw new ArgumentException( "Can't attach to top level or parentless control" );
				}

				ReflectRightToLeft();

				m_nativewnd = new NativeWindowEx( m_parent.Handle );
				m_nativewnd.MessageFilter = this;

				SynchronizeScrollbarsPosition();
			}
		}

		/// <summary>Internal scrollers detaching algorithm.</summary>
		private void DetachScrollers()
		{
			// if we are not disposed then detach all
			if( hScroller != null )
			{
				if( m_nativewnd != null )
				{
					m_nativewnd.MessageFilter = null;
				}

				// hide all controls
				m_sizeGripper.Enabled = m_sizeGripper.Visible = false;
				vScroller.Enabled = vScroller.Visible = false;
				hScroller.Enabled = hScroller.Visible = false;

				// cleanup by Wnd Handles
				vScroller.DropHandle();
				hScroller.DropHandle();
				m_sizeGripper.DropHandle();
			}
		}

		/// <summary>Reflect Right to Left settings from parent control with respect to internal logic.</summary>
		/// <remarks>change RTL before SetParent calls. RTL force re-creation of the scroller handle!!!</remarks>
		protected virtual void ReflectRightToLeft()
		{
			if( null == m_parent )
			{
				throw new ArgumentNullException( "m_parent" );
			}

			// reflect RTL changes in runtime
			hScroller.RightToLeft = vScroller.RightToLeft = m_sizeGripper.RightToLeft = m_parent.RightToLeft;

			UpdateScrollersParent();
		}

		/// <summary>Method reassign parents for scrollers when needed.</summary>
		private void UpdateScrollersParent()
		{
			if( m_parent.IsHandleCreated )
			{
				if( m_parent.Parent == null )
				{
					CreateControlContainer();
				}

				if( m_parent.Parent == null )
				{
					throw new ArgumentNullException( "Control does not have parent window" );
				}

				// assign parent for scrollers
				IntPtr handle = m_parent.Parent.Handle;
				NativeMethods.SetParent( hScroller.Handle, handle );
				NativeMethods.SetParent( vScroller.Handle, handle );
				NativeMethods.SetParent( m_sizeGripper.Handle, handle );
			}
		}

		/// <summary>Method synchronize settings with parent wihdow and scrollers.</summary>
		internal void SynchronizeScrollbarsPosition()
		{
			if( m_parent != null && m_parent.Parent!=null && m_parent.IsHandleCreated )
			{
				SynchronizeScrollbars( true );

				IntPtr hPrev = NativeMethods.GetWindow( m_parent.Handle, NativeMethods.GW_HWNDPREV );
				const int flags = NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE;

				if( this.IsHorizontalScrollVisible )
				{
					NativeMethods.SetWindowPos( hScroller.Handle, hPrev, 0, 0, 0, 0, flags );
				}

				if( this.IsVerticalScrollVisible )
				{
					NativeMethods.SetWindowPos( vScroller.Handle, hPrev, 0, 0, 0, 0, flags );
				}

				if( m_sizeGripper.Visible )
				{
					NativeMethods.SetWindowPos( m_sizeGripper.Handle, hPrev, 0, 0, 0, 0, flags );
				}

				UpdateParentInDragging();

				Rectangle rcHScroll = (Rectangle)m_hScrollBar.rcScrollBar;
				Rectangle rcVScroll = (Rectangle)m_vScrollBar.rcScrollBar;

				int style = NativeMethods.GetWindowLong(m_parent.Handle, NativeMethods.GWL_EXSTYLE);

				if ( (style & NativeMethods.WS_EX_LAYOUTRTL) !=  0)
				{
					NativeMethods.RECT rcParent = new NativeMethods.RECT();
					NativeMethods.GetWindowRect(m_parent.Handle, ref rcParent);

					rcHScroll.X = rcParent.left + rcParent.right - rcHScroll.Right;
					rcVScroll.X = rcParent.left + rcParent.right - rcVScroll.Right;
				}

				Rectangle rcH = m_parent.Parent.RectangleToClient(rcHScroll);
				Rectangle rcV = m_parent.Parent.RectangleToClient(rcVScroll);

				// set position of horizontal scroller
				if( this.IsHorizontalScrollVisible )
				{
					NativeMethods.SetWindowPos( hScroller.Handle, IntPtr.Zero, rcH.Left, rcH.Top,
					  rcH.Width, rcH.Height, NativeMethods.SWP_NOZORDER );
				}

				// set position of vertical scroller
				if( this.IsVerticalScrollVisible )
				{
					NativeMethods.SetWindowPos( vScroller.Handle, IntPtr.Zero, rcV.Left, rcV.Top,
					  rcV.Width, rcV.Height, NativeMethods.SWP_NOZORDER );
				}

				// set position of size grip if it exists
				if( m_sizeGripper.Visible )
				{
					bool bMirrored = ( m_sizeGripper.RightToLeft == RightToLeft.Yes );

					NativeMethods.SetWindowPos( m_sizeGripper.Handle, IntPtr.Zero, ( bMirrored ) ? rcV.Left : rcH.Right,
					  rcV.Bottom, rcV.Width, rcH.Height, NativeMethods.SWP_NOZORDER );
				}
			}
		}
		/// <summary>
		/// If parent is being dragged, scrollers get invisible.
		/// </summary>
		protected virtual void UpdateParentInDragging()
		{
			if( m_bParentInDragging )
			{
				// update visibility
				hScroller.Enabled = hScroller.Visible = false;
				vScroller.Enabled = vScroller.Visible = false;
				m_sizeGripper.Enabled = m_sizeGripper.Visible = false;
			}
			else
			{
				// update visibility
				hScroller.Enabled = hScroller.Visible = this.IsHorizontalScrollVisible;
				vScroller.Enabled = vScroller.Visible = this.IsVerticalScrollVisible;

				UpdateGripperVisibility();
			}
		}
		/// <summary>
		/// Method synchronize our scrollers with parent scollers. By parameter
		/// specified source of data.
		/// </summary>
		/// <param name="bWindow">True - source is window, otherwise False.</param>
		internal void SynchronizeScrollbars( bool bWindow )
		{
			if( m_parent != null && m_parent.IsHandleCreated )
			{
				vScroller.Enabled = m_parent.Enabled;
				hScroller.Enabled = m_parent.Enabled;

				// initialize structures
				m_hScrollBar.cbSize = m_vScrollBar.cbSize = Marshal.SizeOf( typeof( NativeMethods.SCROLLBARINFO ) );

				m_hScroll.cbSize = m_vScroll.cbSize = Marshal.SizeOf( typeof( NativeMethods.SCROLLINFO ) );
				m_hScroll.fMask = m_vScroll.fMask = NativeMethods.SIF_ALL;

				if( bWindow )
				{
					NativeMethods.GetScrollInfo( m_parent.Handle, NativeMethods.SB_HORZ, ref m_hScroll );
					NativeMethods.GetScrollInfo( m_parent.Handle, NativeMethods.SB_VERT, ref m_vScroll );
					NativeMethods.GetScrollBarInfo( m_parent.Handle, NativeMethods.OBJID_HSCROLL, ref m_hScrollBar );
					NativeMethods.GetScrollBarInfo( m_parent.Handle, NativeMethods.OBJID_VSCROLL, ref m_vScrollBar );

					// update controls settings
					UpdateScrollerSettings( hScroller, ref m_hScroll );
					UpdateScrollerSettings( vScroller, ref m_vScroll );
					UpdateScrollerSettings( ref m_hScrollBar, ref m_vScrollBar );
				}
				else
				{
					// update settings by controls
					UpdateSettingsByScroller( ref m_hScroll, hScroller );
					UpdateSettingsByScroller( ref m_vScroll, vScroller );

					NativeMethods.SetScrollInfo( m_parent.Handle, NativeMethods.SB_HORZ, ref m_hScroll, false );
					NativeMethods.SetScrollInfo( m_parent.Handle, NativeMethods.SB_VERT, ref m_hScroll, false );
				}

				if( IsHorizontalScrollVisible )
				{
					hScroller.RecalculateScroll();
				}

				if( IsVerticalScrollVisible )
				{
					vScroller.RecalculateScroll();
				}
			}
		}

		/// <summary>Method updates enable states from structs specified by user.</summary>
		/// <param name="horz">Horizontal scroller struct.</param>
		/// <param name="vert">Vertical scroller struct.</param>
		private void UpdateScrollerSettings( ref NativeMethods.SCROLLBARINFO horz, ref NativeMethods.SCROLLBARINFO vert )
		{
			bool bDisableLeft = horz.rgstate[(int)ScrollBarComponents.BottomLeftArrow] == (int)NativeMethods.StateSystem.STATE_SYSTEM_UNAVAILABLE;
			bool bDisableRight = horz.rgstate[(int)ScrollBarComponents.TopRightArrow] == (int)NativeMethods.StateSystem.STATE_SYSTEM_UNAVAILABLE;
			bool bDisableTop = vert.rgstate[(int)ScrollBarComponents.TopRightArrow] == (int)NativeMethods.StateSystem.STATE_SYSTEM_UNAVAILABLE;
			bool bDisableBottom = vert.rgstate[(int)ScrollBarComponents.BottomLeftArrow] == (int)NativeMethods.StateSystem.STATE_SYSTEM_UNAVAILABLE;

			hScroller.DisableMinimumArrow = bDisableLeft;
			hScroller.DisableMaximumArrow = bDisableRight;
			hScroller.DisableThumb = ( bDisableLeft && bDisableRight );

			vScroller.DisableMinimumArrow = bDisableTop;
			vScroller.DisableMaximumArrow = bDisableBottom;
			vScroller.DisableThumb = ( bDisableTop && bDisableBottom );
		}

		/// <summary>
		/// Utility method. Copy settings value to scrollbar control.
		/// </summary>
		/// <param name="bar">reference on scroller instance.</param>
		/// <param name="settings">Window WIN32 API struct settings of which we reflect.</param>
		private void UpdateScrollerSettings( ScrollBarCustomDraw bar, ref NativeMethods.SCROLLINFO settings )
		{
			bar.Minimum = settings.nMin;
			bar.Maximum = settings.nMax;
			bar.Value = settings.nPos;

			if( m_parent is ListView )
			{
				ListView lv = (ListView)m_parent;

				// If the list-view control is in list-view, large value specifies the number of columns to scroll.
				if( lv.View == View.List )
				{
					bar.LargeChange = 1;
				}
				else
				{
					bar.LargeChange = settings.nPage;
				}

				if( m_parent.RightToLeft == RightToLeft.Yes )
				{
					bar.Value = bar.Minimum + bar.Maximum - bar.Value;
				}
			}
			else
			{
				bar.LargeChange = settings.nPage;
			}
		}

		/// <summary>Vise versa operation to <see cref="UpdateScrollerSettings"/> method.</summary>
		/// <param name="settings">Destination of settings reflection.</param>
		/// <param name="bar">ScrollBra instance which settings we reflect.</param>
		private void UpdateSettingsByScroller( ref NativeMethods.SCROLLINFO settings, ScrollBarCustomDraw bar )
		{
			settings.nMin = bar.Minimum;
			settings.nMax = bar.Maximum;
			settings.nPos = bar.Value;
			settings.nPage = bar.LargeChange;
			settings.nTrackPos = 0;
		}

		/// <summary>
		/// Method create special container windows that will host attached control.
		/// </summary>
		/// <remarks>NOT implemented. Reserved for future enhancements. But can be overrided 
		/// and implemented by user.</remarks>
		protected virtual void CreateControlContainer()
		{
			// TODO: do nothing now. This is reserved for future control enhancing.


		}

		/// <summary>Method process WM_NSCALCSIZE message.</summary>
		/// <param name="m"></param>
		/// <returns></returns>
		protected virtual bool WmNcCalcSize( ref Message m )
		{
			SynchronizeScrollbarsPosition();

			return true;
		}

		/// <summary>Method process WM_WINDOWPOSCHANGED message.</summary>
		/// <param name="m"></param>
		/// <returns></returns>
		protected virtual bool WmWindowPosChanged( ref Message m )
		{
			if( this.EnableDelayedScrollBarUpdate )
			{
				if( this.AttachedTo != null && this.AttachedTo.IsHandleCreated )
				{
					EndInvokeSynchronizeScrollbars();

					if( m_miSynchronizeScrollbarsHandler == null )
					{
						m_miSynchronizeScrollbarsHandler = new MethodInvoker( SynchronizeScrollbarsAsync );
					}

					m_iarSynchronizeScrollbars = this.AttachedTo.BeginInvoke( m_miSynchronizeScrollbarsHandler );
				}
			}

			SynchronizeScrollbarsPosition();

			return true;
		}

		private void EndInvokeSynchronizeScrollbars()
		{
			Control owner = this.AttachedTo;

			if( owner != null && m_iarSynchronizeScrollbars != null && !m_iarSynchronizeScrollbars.IsCompleted )
			{
				owner.EndInvoke( m_iarSynchronizeScrollbars );
			}

			m_iarSynchronizeScrollbars = null;
		}

		/// <summary>Method process WM_STYLECHANGED message.</summary>
		/// <param name="m"></param>
		/// <returns></returns>
		protected virtual bool WmStyleChanged( ref Message m )
		{
			SynchronizeScrollbarsPosition();
			return true;
		}
		/// <summary>
		/// Method process a WM_MOVE message.
		/// </summary>
		/// <returns></returns>
		protected virtual bool WM_Move( ref Message m )
		{
			m_bParentInDragging = m_bLeftMouseButtonDown = false;

			return true;
		}
		/// <summary>
		/// Method process a WM_MOUSEMOVE message.
		/// </summary>
		/// <returns></returns>
		protected virtual bool WM_MouseMove( ref Message m )
		{
			m_bParentInDragging = m_bLeftMouseButtonDown;

			return true;
		}
		/// <summary>Method filter parent control messages and according to 
		/// them set scroller frame settings.</summary>
		/// <param name="m">Windows message.</param>
		/// <returns>True - allow forward of message, otherwise False.</returns>
		public bool PreFilterMessage( ref Message m )
		{
			bool bForward = true;

			switch( m.Msg )
			{
				case NativeMethods.WM_NCCALCSIZE:
				bForward = WmNcCalcSize( ref m );
				break;

				//case WM_WINDOWPOSCHANGING:
				case NativeMethods.WM_WINDOWPOSCHANGED:
				bForward = WmWindowPosChanged( ref m );
				break;

				//case WM_STYLECHANGING:
				case NativeMethods.WM_STYLECHANGED:
				bForward = WmStyleChanged( ref m );
				break;

				case NativeMethods.WM_MOVE:
				bForward = WM_Move( ref m );
				break;

				case NativeMethods.WM_MOUSEMOVE:
				bForward = WM_MouseMove( ref m );
				break;

				case 49693:
				case NativeMethods.WM_MOUSEWHEEL:
				case NativeMethods.WM_KEYDOWN:
				case NativeMethods.WM_REFLECT + NativeMethods.WM_PARENTNOTIFY:
				SynchronizeScrollbars( true );
				break;

				default:
				if( !( m_parent is ListView ) )
				{
					SynchronizeScrollbars( true );
				}
				break;
			}

			return !bForward;
		}

		/// <summary>
		/// Updates visibility of gripper.
		/// </summary>
		protected virtual void UpdateGripperVisibility()
		{
			m_sizeGripper.Enabled = m_sizeGripper.Visible = GetGripperVisibility();
		}
		/// <summary>
		/// Gets visibility of the size gripper.
		/// </summary>
		/// <returns>true if gripper should be shown; otherwise false.</returns>
		protected bool GetGripperVisibility()
		{
			bool bGrip;
			switch( m_gripperVisibility )
			{
				case SizeGripperVisibility.Visible:
				bGrip = m_parent.Visible;
				break;

				case SizeGripperVisibility.Hidden:
				bGrip = false;
				break;

				case SizeGripperVisibility.Auto:
				default:
				bGrip = ( hScroller.Visible && vScroller.Visible ) && m_parent.Visible;
				break;

			}
			return bGrip;
		}
		/// <summary>
		/// 
		/// </summary>
		private void SynchronizeScrollbarsAsync()
		{
			SynchronizeScrollbars( true );
		}

		/// <summary>
		/// Verifies if <see cref="ScrollBarCustomDraw"/> is owned by <see cref="ScroolerFrame"/> attched to control.
		/// </summary>
		/// <param name="control">Control to verify.</param>
		/// <param name="scroll"></param>
		/// <returns></returns>
		internal static bool IsRelated( Control control, ScrollBarCustomDraw scroll )
		{
			bool bResult = false;

			if( s_attachedControls.Contains( control ) )
			{
				ScrollersFrame frame = (ScrollersFrame)s_attachedControls[control];

				bResult = ( frame.hScroller == scroll ) || ( frame.vScroller == scroll );
			}

			return bResult;
		}

		#endregion

		#region Class internal declarations
		/// <summary>Special Size Gripper class that supports RTL.</summary>
		[ToolboxItem( false )]
		public class SizeGripper: Control
		{
			#region Class Public Methods
			/// <summary>Publish for user Handle destroy functionality. Usefull for resource 
			/// cleanup in runtime.</summary>
			public void DropHandle()
			{
				this.DestroyHandle();
			}
			#endregion

			#region Class override
			/// <summary></summary>
			/// <param name="e"></param>
			protected override void OnPaint( PaintEventArgs e )
			{
				base.OnPaint( e );

				bool bMirrored = ( this.RightToLeft == RightToLeft.Yes );

				using( CMirroredDrawer md3DBorder = new CMirroredDrawer( e.Graphics, this.ClientRectangle, bMirrored ) )
				{
					ControlPaint.DrawSizeGrip( md3DBorder.VirtualGfx, this.BackColor, md3DBorder.VirtualBounds );
				}
			}
			#endregion
		}

		/// <summary>
		/// Size gripper advanced.
		/// </summary>
		[ToolboxItem( false )]
		public class SizeGripperAdv: SizeGripper
		{
			#region Class constants
			/// <summary></summary>
			private const int MIN_WIDTH = 15;
			/// <summary></summary>
			private const int GRIP_RECTANGLE_WIDTH = 2;
			#endregion

			#region Fields
			/// <summary>
			/// Instance of ScrollersFrame.
			/// </summary>
			private ScrollersFrame m_scrFrame;
			/// <summary>
			/// Indicates whether grip marking should be drawn. If set to false, just background is filled.
			/// </summary>
			private bool m_bDrawGripMarking =  true;
			#endregion

			#region Properties
			/// <summary>
			/// Gets or sets value indicating whether grip marking should be drawn. If set to false, just background is filled.
			/// </summary>
			public bool DrawGripMarking
			{
				get
				{
					return m_bDrawGripMarking;
				}
				set
				{
					if( m_bDrawGripMarking != value )
					{
						m_bDrawGripMarking = value;
						Invalidate();
					}
				}
			}
			#endregion

			#region Initialization
			/// <summary>Default constructor.</summary>
			/// <param name="frame">reference on scroller frame. Can not be NULL.</param>
			public SizeGripperAdv( ScrollersFrame frame )
				: base()
			{
				if( null == frame )
					throw new ArgumentNullException( "frame" );

				m_scrFrame = frame;
			}
			#endregion

			#region Overrides
			/// <summary>Implemented visual styles support.</summary>
			/// <param name="e">Paint event arguments.</param>
			protected override void OnPaint( PaintEventArgs e )
			{
				if( m_scrFrame != null )
				{
					switch( m_scrFrame.VisualStyle )
					{
						case ScrollBarCustomDrawStyles.Office2007:
						case ScrollBarCustomDrawStyles.Office2007Generic:
						{
							ColorTableOffice2007 colorTable = ColorTableOffice2007.GetColorTable( m_scrFrame.VisualStyle, m_scrFrame.OfficeColorScheme );

							Color clGripDark = colorTable.ScrollerGripDark;
							Color clGripLight = colorTable.ScrollerGripLight;
							Color clGripBackGround = colorTable.ScrollerGripBackGround;

							PaintGripOffice2007( e.Graphics, e.ClipRectangle, clGripLight, clGripDark, clGripBackGround );
						}
						break;
                        case ScrollBarCustomDrawStyles.Office2010:
                        {
                            ColorTableOffice2010 colorTable = ColorTableOffice2010.GetColorTable(m_scrFrame.VisualStyle, m_scrFrame.Office2010ColorScheme);

                            Color clGripDark = colorTable.ScrollerGripDark;
                            Color clGripLight = colorTable.ScrollerGripLight;
                            Color clGripBackGround = colorTable.ScrollerGripBackGround;

                            PaintGripOffice2007(e.Graphics, e.ClipRectangle, clGripLight, clGripDark, clGripBackGround);
                        }
                        break;
						case ScrollBarCustomDrawStyles.WindowsXP:
						{
							Color clGripDark = Color.FromArgb( 184, 181, 161 );
							Color clGripLight = Color.FromArgb( 255, 255, 255 );
							Color clGripBackGround = Color.FromArgb( 224, 223, 227 );

							PaintGripOffice2007( e.Graphics, e.ClipRectangle, clGripLight, clGripDark, clGripBackGround );
						}
						break;

						case ScrollBarCustomDrawStyles.Classic:
						{
							if( m_bDrawGripMarking )
							{
								bool bMirrored = ( this.RightToLeft == RightToLeft.Yes );

								using( CMirroredDrawer md3DBorder = new CMirroredDrawer( e.Graphics, this.ClientRectangle, bMirrored ) )
								{
									ControlPaint.DrawSizeGrip( md3DBorder.VirtualGfx, this.BackColor, md3DBorder.VirtualBounds );
								}
							}
							else
							{
								using( Brush br = new SolidBrush( this.BackColor ) )
								{
									e.Graphics.FillRectangle( br, this.ClientRectangle );
								}
							}
						}
						break;
					}
				}

			}

			/// <summary>
			/// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control"/> and its child controls and optionally releases the managed resources.
			/// </summary>
			/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
			protected override void Dispose( bool disposing )
			{
				if( disposing )
				{
					m_scrFrame = null;
				}

				base.Dispose( disposing );
			}
			#endregion

			#region Implementation
			/// <summary></summary>
			/// <param name="g"></param>
			/// <param name="rc"></param>
			/// <param name="clGripLight"/>
			/// <param name="clGripDark"/>
			/// <param name="clGripBackground"/>
			private void PaintGripOffice2007( Graphics g, Rectangle rc, Color clGripLight, Color clGripDark, Color clGripBackground )
			{
                using (Brush brush = new SolidBrush(clGripBackground))
                    g.FillRectangle(brush, rc);

				if( m_bDrawGripMarking )
				{
					if( rc.Width > MIN_WIDTH && rc.Height > MIN_WIDTH )
					{
						using( Brush brushLight = new SolidBrush( clGripLight ) )
						{
							if( this.RightToLeft == RightToLeft.Yes )
							{
								Rectangle rcLight = new Rectangle( rc.Left + 2, rc.Height - 4, GRIP_RECTANGLE_WIDTH, GRIP_RECTANGLE_WIDTH );
								g.FillRectangle( brushLight, rcLight );
								rcLight.X += 4;
								g.FillRectangle( brushLight, rcLight );
								rcLight.X += 4;
								g.FillRectangle( brushLight, rcLight );
								rcLight.X -= 4;
								rcLight.Y -= 4;
								g.FillRectangle( brushLight, rcLight );
								rcLight.X -= 4;
								g.FillRectangle( brushLight, rcLight );
								rcLight.Y -= 4;
								g.FillRectangle( brushLight, rcLight );
							}
							else
							{
								Rectangle rcLight = new Rectangle( rc.Width - 4, rc.Height - 4, GRIP_RECTANGLE_WIDTH, GRIP_RECTANGLE_WIDTH );
								g.FillRectangle( brushLight, rcLight );
								rcLight.X -= 4;
								g.FillRectangle( brushLight, rcLight );
								rcLight.X -= 4;
								g.FillRectangle( brushLight, rcLight );
								rcLight.X += 4;
								rcLight.Y -= 4;
								g.FillRectangle( brushLight, rcLight );
								rcLight.X += 4;
								g.FillRectangle( brushLight, rcLight );
								rcLight.Y -= 4;
								g.FillRectangle( brushLight, rcLight );
							}
						}

						using( Brush brushDark = new SolidBrush( clGripDark ) )
						{
							if( this.RightToLeft == RightToLeft.Yes )
							{
								Rectangle rcDark = new Rectangle( rc.Left + 3, rc.Height - 5, GRIP_RECTANGLE_WIDTH, GRIP_RECTANGLE_WIDTH );
								g.FillRectangle( brushDark, rcDark );
								rcDark.X += 4;
								g.FillRectangle( brushDark, rcDark );
								rcDark.X += 4;
								g.FillRectangle( brushDark, rcDark );
								rcDark.X -= 4;
								rcDark.Y -= 4;
								g.FillRectangle( brushDark, rcDark );
								rcDark.X -= 4;
								g.FillRectangle( brushDark, rcDark );
								rcDark.Y -= 4;
								g.FillRectangle( brushDark, rcDark );
							}
							else
							{
								Rectangle rcDark = new Rectangle( rc.Width - 5, rc.Height - 5, GRIP_RECTANGLE_WIDTH, GRIP_RECTANGLE_WIDTH );
								g.FillRectangle( brushDark, rcDark );
								rcDark.X -= 4;
								g.FillRectangle( brushDark, rcDark );
								rcDark.X -= 4;
								g.FillRectangle( brushDark, rcDark );
								rcDark.X += 4;
								rcDark.Y -= 4;
								g.FillRectangle( brushDark, rcDark );
								rcDark.X += 4;
								g.FillRectangle( brushDark, rcDark );
								rcDark.Y -= 4;
								g.FillRectangle( brushDark, rcDark );
							}
						}
					}
				}
			}
			#endregion
		}

		/// <summary>Design time change of property visibility. Office 2007 visual style support.</summary>
		internal class VisualStyleTypeConverter: CustomPropertiesTypeConverter
		{
			/// <summary>Show/Hide OfficeColorScheme property for user in runtime.</summary>
			/// <param name="component"></param>
			/// <param name="property"></param>
			/// <returns></returns>
			protected override Attribute[] GetPropertyAttributes( Component component, PropertyDescriptor property )
			{
				ScrollersFrame scrollersFrame = component as ScrollersFrame;

				if( scrollersFrame != null && (property.Name == PROPERTY_NAME || property.Name == "Office2010ColorScheme"))
				{
					switch( scrollersFrame.VisualStyle )
					{
						case ScrollBarCustomDrawStyles.Office2007:
						case ScrollBarCustomDrawStyles.Office2007Generic:
                        case ScrollBarCustomDrawStyles.Office2010:
						return new Attribute[] { BrowsableAttribute.Yes };

						default:
						return new Attribute[] { BrowsableAttribute.No };
					}
				}

				return EmptyAttributes;
			}
		}
		#endregion
	}
	#endregion

	#region *** ScrollersFrameDesigner
	/// <summary>Class Attaching better design time support for FM2.0 and higher.
	/// In older version class have no influences on design time.</summary>
	public class ScrollersFrameDesigner: ComponentDesigner
	{
		/// <summary></summary>
		/// <param name="component"></param>
		public override void Initialize( IComponent component )
		{
			base.Initialize( component );

#if ! ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			INestedContainer nestedContainer = this.GetService( typeof( INestedContainer ) ) as INestedContainer;
			if( nestedContainer != null )
			{
				ScrollersFrame frame = component as ScrollersFrame;

				if( frame != null )
				{
					nestedContainer.Add( frame.VerticalScroller );
					nestedContainer.Add( frame.HorizontalScroller );
				}
			}
#endif
		}
	}
	#endregion

	#region *** AttachedToControlTypeConverter
	/// <summary>Designer helper lass. Do not allow attaching of root 
	/// components by <see cref="ScrollersFrame"/>.</summary>
	internal class AttachedToControlTypeConverter: ReferenceConverter
	{
		/// <summary>Default constructor.</summary>
		/// <param name="type"></param>
		public AttachedToControlTypeConverter( Type type )
			:
			  base( type )
		{
		}
		/// <summary>Method filter that chech is attaching allowed or not.</summary>
		/// <param name="context">filter context.</param>
		/// <param name="value">property value that requesting check operation.</param>
		/// <returns>True - if attaching allowed, otherwise False.</returns>
		protected override bool IsValueAllowed( ITypeDescriptorContext context, object value )
		{
			bool bResult = base.IsValueAllowed( context, value );

			if( bResult )
			{
				IDesignerHost host = context.GetService( typeof( IDesignerHost ) ) as IDesignerHost;

				bResult = ( null != host && value != host.RootComponent );
			}

			return bResult;
		}
	}
	#endregion
    /// <summary>
    /// ProgressBarAdv Designer
    /// </summary>
    public class ScrollerFrameDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        /// <summary>
        /// Designer ActionList collection
        /// </summary>
        private System.ComponentModel.Design.DesignerActionListCollection actionLists;

        /// <summary>
        ///  Initializes a new instance of the CheckBoxAdvDesigner class
        /// </summary>
        public ScrollerFrameDesigner()
            : base()
        {
        }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

        /// <summary>
        /// Gets a value indication the designer action
        /// </summary>
        public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
        {
            get
            {
                if (null == this.actionLists)
                {
                    this.actionLists = new System.ComponentModel.Design.DesignerActionListCollection();
                    this.actionLists.Add(new ScrollerFrameActionList(this.Component));
                }

                return this.actionLists;
            }
        }

#endif
    }
}