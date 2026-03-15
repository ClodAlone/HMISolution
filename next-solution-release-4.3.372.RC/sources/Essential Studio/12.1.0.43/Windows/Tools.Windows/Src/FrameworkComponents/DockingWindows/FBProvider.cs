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
using System.Diagnostics;
using System.Reflection;
using Syncfusion.Runtime.InteropServices;
using Microsoft.Win32;

namespace Syncfusion.Windows.Forms.Tools
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public class DragDockInfo
	{
		private DockPreference m_DockPreference;
		private DockingStyle m_DockingStyle;
		private Rectangle m_DockArea;
		private int m_Priority;
		private DockControllerBase m_Controller;

		public DockPreference DockPreference
		{
			get { return m_DockPreference; }
			set { m_DockPreference = value; }
		}

		public DockingStyle DockingStyle
		{
			get { return m_DockingStyle; }
			set { m_DockingStyle = value; }
		}

		public Rectangle DockArea
		{
			get { return m_DockArea; }
			set { m_DockArea = value; }
		}

		public int Priority
		{
			get { return m_Priority; }
			set { m_Priority = value; }
		}

		public DockControllerBase Controller
		{
			get { return m_Controller; }
			set { m_Controller = value; }
		}
	}
	
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IFrameBorderPainter
	{
		void StartPaint();
		void EndPaint();

		void DrawFrame( Rectangle rectangle );
		void DrawTabFrame( Rectangle rectangle, DockTabAlignmentStyle alignment );
		void DrawRectangle( Rectangle rectangle );
        void DrawRectangle(Rectangle rectangle,VisualStyle style);
	}

	abstract class CustomBorderPainter: IFrameBorderPainter
	{
		private int tabHeight = 22;
		private bool isMirrored = false;

		public int TabHeight
		{
			get { return tabHeight; }
			set { tabHeight = value; }
		}

		public bool IsMirrored
		{
			get { return isMirrored; }
			set { isMirrored = value; }
		}

		abstract public void StartPaint();
		abstract public void EndPaint();

		abstract public void DrawFrame( Rectangle rectangle );
		abstract public void DrawTabFrame( Rectangle rectangle, DockTabAlignmentStyle alignment  );
		abstract public void DrawRectangle( Rectangle rectangle  );
        abstract public void DrawRectangle(Rectangle rectangle,VisualStyle style);
	}

	internal class HalftoneBorderPainter: CustomBorderPainter
	{
		public override void StartPaint()
		{
		}

		public override void EndPaint()
		{
		}

		public override void DrawFrame( Rectangle rectangle )
		{
			InternalDrawRectangle(new Rectangle(rectangle.Left, rectangle.Top, 4, rectangle.Height));
			InternalDrawRectangle(new Rectangle(rectangle.Left+4, rectangle.Top, rectangle.Width-8, 4));
			InternalDrawRectangle(new Rectangle(rectangle.Right-4, rectangle.Top, 4, rectangle.Height)); 
			InternalDrawRectangle(new Rectangle(rectangle.Left+4, rectangle.Bottom-4, rectangle.Width-8, 4));
		}

		public override void DrawTabFrame( Rectangle rectangle, DockTabAlignmentStyle alignment )
		{				
			int ntabwt = 0;
			int nTabHeight = 22; //this.dockingMgr.DockTabHeight;
			int nTabLeft = 0;
			if( alignment == DockTabAlignmentStyle.Top ||
				alignment == DockTabAlignmentStyle.Bottom )
			{
				ntabwt = ( rectangle.Width > 100 ) ? 45 : rectangle.Width / 2;
				nTabLeft = rectangle.Left + 8;
			}
			else
			{
				ntabwt = ( rectangle.Height > 100 ) ? 45 : rectangle.Height / 2;
				nTabLeft = rectangle.Top + 8;
			}
			Rectangle rctab = Rectangle.Empty;
								
			switch( alignment )
			{
				case DockTabAlignmentStyle.Bottom:
					rctab = new Rectangle(nTabLeft, rectangle.Bottom - nTabHeight - 4, ntabwt, nTabHeight + 4);
					rectangle.Size = new Size(rectangle.Width, rectangle.Height - nTabHeight);
					break;

				case DockTabAlignmentStyle.Top:
					rctab = new Rectangle(nTabLeft, rectangle.Y, ntabwt, nTabHeight + 4);
					rectangle.Size = new Size(rectangle.Width, rectangle.Height - nTabHeight);
					rectangle.Location = new Point(rectangle.Location.X, rectangle.Location.Y + nTabHeight);
					break;

				case DockTabAlignmentStyle.Left:
					rctab = new Rectangle(rectangle.X, nTabLeft, nTabHeight + 4, ntabwt);
					rectangle.Size = new Size(rectangle.Width - nTabHeight, rectangle.Height);
					rectangle.Location = new Point(rectangle.Location.X + nTabHeight, rectangle.Location.Y);
					break;

				case DockTabAlignmentStyle.Right:
					rectangle.Size = new Size(rectangle.Width - nTabHeight, rectangle.Height);
					rctab = new Rectangle(rectangle.X + rectangle.Width-4, nTabLeft, nTabHeight+4, ntabwt);
					break;
			}
			DrawFrame(rectangle);
			InternalDrawTabFrameHelper(rctab, alignment);
		}

		public override void DrawRectangle( Rectangle rectangle )
		{
			InternalDrawRectangle( rectangle );
		}
        public override void DrawRectangle(Rectangle rectangle,VisualStyle style)
        {
            InternalDrawRectangle(rectangle,style);
        }
		protected IntPtr CreateHalftoneHBRUSH()
		{
			int n1 = 0;
			short[] arrclrs = new System.Int16[8];
			while(n1 < 8)
			{
				arrclrs[n1] = ((short) (0x5555 << ((n1 & 1) & 31)));
				n1 = (n1 + 1);
			}
			IntPtr hbmp = Syncfusion.Runtime.InteropServices.NativeMethods.CreateBitmap(8, 8, 1, 1, arrclrs);
			NativeMethods.LOGBRUSH lb = new NativeMethods.LOGBRUSH();
			lb.lbStyle = 3;
			lb.lbColor = ColorTranslator.ToWin32(Color.Black);
			lb.lbHatch = hbmp;
			IntPtr hbrush  = NativeMethods.CreateBrushIndirect(ref lb);
			Syncfusion.Runtime.InteropServices.NativeMethods.DeleteObject(hbmp);
			return hbrush;
		}
        protected void InternalDrawRectangle(Rectangle rcdrag,VisualStyle style)
        {
            IntPtr hdc = NativeMethods.GetWindowDC(IntPtr.Zero);
            try
            {
                //IntPtr hbrush = this.CreateHalftoneHBRUSH();
                IntPtr hbrush = NativeMethods.CreateSolidBrush((int)ColorTranslator.ToWin32(ColorTranslator.FromHtml("#969696")));
                IntPtr hgdiobj = NativeMethods.SelectObject(hdc, hbrush);
                NativeMethods.PatBlt(hdc, rcdrag.X, rcdrag.Y, rcdrag.Width, rcdrag.Height, 0x5a0049/*PATINVERT*/);
                NativeMethods.SelectObject(hdc, hgdiobj);
                NativeMethods.DeleteObject(hbrush);
            }
            finally
            {
                NativeMethods.ReleaseDC(IntPtr.Zero, hdc);
            }
        }
		protected void InternalDrawRectangle(Rectangle rcdrag)
		{
			IntPtr hdc = NativeMethods.GetWindowDC( IntPtr.Zero );
            try
            {
                IntPtr hbrush = this.CreateHalftoneHBRUSH();
                IntPtr hgdiobj = NativeMethods.SelectObject(hdc, hbrush);
                NativeMethods.PatBlt(hdc, rcdrag.X, rcdrag.Y, rcdrag.Width, rcdrag.Height, 0x5a0049/*PATINVERT*/);
                NativeMethods.SelectObject(hdc, hgdiobj);
                NativeMethods.DeleteObject(hbrush);
            }
            finally
            {
                NativeMethods.ReleaseDC(IntPtr.Zero, hdc);
            }
		}

		protected void InternalDrawTabFrameHelper( Rectangle rectangle, DockTabAlignmentStyle alignment )
		{				
			Rectangle left = Rectangle.Empty;
			Rectangle top = Rectangle.Empty;
			Rectangle right = Rectangle.Empty;
			Rectangle bottom = Rectangle.Empty;
								
			switch( alignment )
			{
				case DockTabAlignmentStyle.Bottom:
					left = new Rectangle(rectangle.Left, rectangle.Top + 4, 4, rectangle.Height - 4);
					top = new Rectangle(rectangle.Left + 4, rectangle.Top, rectangle.Width - 8, 4);
					right = new Rectangle(rectangle.Right - 4, rectangle.Top + 4, 4, rectangle.Height - 4);
					bottom = new Rectangle(rectangle.Left + 4, rectangle.Bottom - 4, rectangle.Width - 8, 4);
					break;

				case DockTabAlignmentStyle.Top:
					left = new Rectangle(rectangle.Left, rectangle.Top, 4, rectangle.Height-4);
					top = new Rectangle(rectangle.Left + 4, rectangle.Top, rectangle.Width - 8, 4);
					right = new Rectangle(rectangle.Right - 4, rectangle.Top, 4, rectangle.Height-4);
					bottom = new Rectangle(rectangle.Left + 4, rectangle.Bottom - 4, rectangle.Width - 8, 4);
					break;

				case DockTabAlignmentStyle.Right:
					left = new Rectangle(rectangle.Left, rectangle.Top, 4, rectangle.Height-4);
					top = new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width - 4, 4);
					right = new Rectangle(rectangle.Right - 4, rectangle.Top, 4, rectangle.Height-4);
					bottom = new Rectangle(rectangle.Left + 4, rectangle.Bottom - 4, rectangle.Width - 4, 4);
					break;

				case DockTabAlignmentStyle.Left:
					left = new Rectangle(rectangle.Left, rectangle.Top + 4, 4, rectangle.Height - 4);
					top = new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width - 4, 4);
					right = new Rectangle(rectangle.Right - 4, rectangle.Top + 4, 4, rectangle.Height - 4);
					bottom = new Rectangle(rectangle.Left + 4, rectangle.Bottom - 4, rectangle.Width - 4, 4);
					break;
			}

			InternalDrawRectangle(left); // Left Border
			InternalDrawRectangle(top); // Top Border
			InternalDrawRectangle(right); // Right Border
			InternalDrawRectangle(bottom); // Bottom Border
		}
	}
		
	[ToolboxItem(false)]
	internal class FloatingControl: Control
	{
		public void ShowFloating()
		{
			if (this.Handle == IntPtr.Zero)
			{
				base.CreateControl();
			}

			NativeMethods.SetParent(base.Handle, IntPtr.Zero);
			NativeMethods.ShowWindow(base.Handle, 1);
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams p = base.CreateParams;
				p.ExStyle |= ( NativeMethods.WS_EX_NOACTIVATE | 
					NativeMethods.WS_EX_TOOLWINDOW | 
					NativeMethods.WS_EX_TOPMOST);
				p.Parent = IntPtr.Zero;
				Screen screen = Screen.AllScreens[0];
				p.Width = screen.Bounds.Width;
				p.Height = screen.Bounds.Height;
				return p;
			}
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == NativeMethods.WM_NCHITTEST)
			{
				m.Result = (IntPtr) NativeMethods.HTTRANSPARENT;
			}
			else
			{
				base.WndProc(ref m);
			}
		}
	}

	internal class ColorBorderPainter: CustomBorderPainter
	{
		private FloatingControl borderControl;
		private Brush brush;
		private Graphics graphics;
		private PaintInfo paintInfo = new PaintInfo();
		private bool paintStarted = false;
		private bool startupSettings = false;
		private float opacity = 1;

		protected ColorBorderPainter()
		{
			CreateForm();
		}
			
		public ColorBorderPainter( Brush brush ) : this()
		{
			this.brush = brush;
		}

		public ColorBorderPainter( Color color ) : this()
		{
            using(Brush br=new SolidBrush( color ))
                brush = br;
		}

		public float Opacity
		{
			get { return opacity; }
			set 
			{
				if( opacity != value )
				{
					opacity = value;
					UpdateLayeredWindow();
				}
			}
		}

		public Color BrushColor
		{
			set 
			{
                using (Brush br = new SolidBrush(value))
                    brush = br;
			}
		}

		public Brush Brush
		{
			set 
			{ 
				brush = value;
			}
		}
 
		public override void StartPaint()
		{
			if( !paintStarted )
			{
				borderControl.ShowFloating();

				if( !startupSettings )
				{
					UpdateLayeredWindow();
					startupSettings = true;
				}

				paintStarted = true;
			}
		}

		public override void EndPaint()
		{
			if( paintStarted )
			{
				borderControl.Hide();
				paintStarted = false;
			}
		}

		private void CreateForm( )
		{
			borderControl = new FloatingControl( );
			//borderControl.BackColor = Color.Black;
				
			graphics = Graphics.FromHwnd( borderControl.Handle );
			borderControl.Paint += new PaintEventHandler( borderForm_Paint );

			// prevent flicking on first show
			borderControl.Size = new Size( 0, 0 );
			borderControl.Update();
		}

		public override void DrawFrame( Rectangle rectangle )
		{
			if( rectangle != Rectangle.Empty )
			{
				SetFormPosition( rectangle );
				SetPaintInfo( PaintFigure.Frame, rectangle );
				ForceRepaint();
			}
		}

		public override void DrawTabFrame( Rectangle rectangle, DockTabAlignmentStyle alignment )
		{
			if( rectangle != Rectangle.Empty )
			{
				SetFormPosition( rectangle );
				SetPaintInfo( PaintFigure.TabFrame, rectangle );
				ForceRepaint();
			}
		}

		public override void DrawRectangle( Rectangle rectangle )
		{
			if( rectangle != Rectangle.Empty )
			{
				SetFormPosition( rectangle );
				SetPaintInfo( PaintFigure.Rectangle, rectangle );
				ForceRepaint();
			}
		}
        public override void DrawRectangle(Rectangle rectangle,VisualStyle style)
        {
            if (rectangle != Rectangle.Empty)
            {
                SetFormPosition(rectangle);
                SetPaintInfo(PaintFigure.Rectangle, rectangle);
                ForceRepaint();
            }
        }

		protected void borderForm_Paint( object Sender, PaintEventArgs e )
		{
			graphics = e.Graphics;
			Rectangle rectangle = paintInfo.BorderRectangle;
			switch( paintInfo.PaintFigure )
			{
				case PaintFigure.Rectangle:
				{
					graphics.FillRectangle( brush, 0, 0, rectangle.Width, rectangle.Height );
					break;
				}
				case PaintFigure.Frame:
				{
					//top
					graphics.FillRectangle( brush, 0, 0, rectangle.Width - 4, 4);
					//right
					graphics.FillRectangle( brush, rectangle.Width - 4, 0, 4, rectangle.Height - 4);
					//bottom
					graphics.FillRectangle( brush, 4, rectangle.Height - 4, rectangle.Width - 4, 4);
					//left
					graphics.FillRectangle( brush, 0, 4, 4, rectangle.Height - 4);
					break;
				}
				case PaintFigure.TabFrame:
				{
					//top
					graphics.FillRectangle( brush, 0, 0, rectangle.Width - 4, 4);
					//left
					graphics.FillRectangle( brush, 0, 4, 4, rectangle.Height - 4 - TabHeight );
					//right
					graphics.FillRectangle( brush, rectangle.Width - 4, 0, 
						4, rectangle.Height - TabHeight );

					//tab helper
					int tabWidth = ( rectangle.Width > 100 ) ? 45 : rectangle.Width / 2 ;
					int tabLeft = IsMirrored ? rectangle.Width - tabWidth - 8 :	8;

					graphics.FillRectangle( brush, tabLeft, rectangle.Height - TabHeight,
						4, TabHeight);
					graphics.FillRectangle( brush, tabLeft + tabWidth, 
						rectangle.Height - TabHeight, 4, TabHeight);
						
					graphics.FillRectangle( brush, tabLeft, rectangle.Height - 4, tabWidth, 4);
						
					graphics.FillRectangle( brush, 0, rectangle.Height - TabHeight - 4, tabLeft + 4, 4 );
					graphics.FillRectangle( brush, tabLeft + tabWidth, 
						rectangle.Height - TabHeight - 4, rectangle.Width - tabLeft, 4 );
					break;
				}
			}

		}

		private void SetPaintInfo( PaintFigure paintFigure, Rectangle rectangle )
		{
			paintInfo.PaintFigure = paintFigure;
			paintInfo.BorderRectangle = rectangle;
		}

		private void SetFormPosition( Rectangle rectangle )
		{
			borderControl.Location = rectangle.Location;
			borderControl.Size = rectangle.Size;
		}
		
		private void ForceRepaint()
		{
			NativeMethods.SendMessage( borderControl.Handle, NativeMethods.WM_PAINT, IntPtr.Zero,
				IntPtr.Zero );
		}

		private void UpdateLayeredWindow()
		{
			IntPtr hwnd = borderControl.Handle;
			NativeMethods.SetWindowLong( hwnd, NativeMethods.GWL_EXSTYLE,
				(IntPtr) ((int) NativeMethods.GetWindowLong(hwnd, 
				NativeMethods.GWL_EXSTYLE ) | NativeMethods.WS_EX_LAYERED ) );
			Color color = borderControl.BackColor;
			int i = NativeMethods.RGBToCOLORREF( 256 * 256 * color.R + 256 * color.G + color.B );
			NativeMethods.SetLayeredWindowAttributes(hwnd, (uint) i, (byte) (255 * opacity), 3);
		}
	}

	internal struct PaintInfo
	{
		public PaintFigure PaintFigure;
		public Rectangle BorderRectangle;
	}

	internal enum PaintFigure { Frame, TabFrame, Rectangle }

	internal class DragControlConsts
	{
		public static int DEF_INNER_CONTROL_COUNT = 5;
		
		#region Margins

		public static int DEF_TARGET_MARGIN = 16;
		public static int DEF_INNER_CONTROL_MARGIN = 37;

		#endregion

		#region Image names

		public static string DEF_IMAGES_PATH = "FrameworkComponents.DockingWindows.Images.WhidbeyDragProvider.";
		public static string DEF_ACTIVE_TARGET_IMAGE_NAME = "active.gif";
		public static string DEF_INACTIVE_TARGET_IMAGE_NAME = "inactive.gif";
		public static string DEF_ACTIVE_TAB_IMAGE_NAME = "active_tab.gif";
		public static string DEF_INACTIVE_TAB_IMAGE_NAME = "inactive_tab.gif";

		#endregion

		#region Image indexes

		public static int DEF_LEFT_INACTIVE_TARGET = 0;
		public static int DEF_TOP_INACTIVE_TARGET = 1;
		public static int DEF_RIGHT_INACTIVE_TARGET = 2;
		public static int DEF_BOTTOM_INACTIVE_TARGET = 3;
		public static int DEF_INACTIVE_TAB = 4;
		public static int DEF_LEFT_ACTIVE_TARGET = 5;
		public static int DEF_TOP_ACTIVE_TARGET = 6;
		public static int DEF_RIGHT_ACTIVE_TARGET = 7;
		public static int DEF_BOTTOM_ACTIVE_TARGET = 8;
		public static int DEF_ACTIVE_TAB = 9;

		#endregion
	}

	/// <summary>
	/// Specifies the different target position of the docking window.
	/// </summary>
	[Syncfusion.Documentation.DocumentationExclude()]
	public enum DragTarget 
	{ 
		/// <summary>
		/// Inner Left of the target window.
		/// </summary>
		InnerLeft = 0,
		/// <summary>
		/// Inner Top of the target window.
		/// </summary>
		InnerTop, 
		/// <summary>
		/// Inner Right of the target window.
		/// </summary>
		InnerRight, 
		/// <summary>
		/// Inner Bottom of the target window.
		/// </summary>
		InnerBottom,
		/// <summary>
		/// Inner Tab of the target window.
		/// </summary>
		InnerTab, 
		/// <summary>
		/// Outer Left of the target window.
		/// </summary>
		OuterLeft, 
		/// <summary>
		/// Outer Top of the target window.
		/// </summary>
		OuterTop, 
		/// <summary>
		/// Outer Right of the target window.
		/// </summary>
		OuterRight, 
		/// <summary>
		/// Outer Bottom of the target window.
		/// </summary>
		OuterBottom, 
		/// <summary>
		/// None of the target position in a window.
		/// </summary>
		None = -1 
	} 

	/// <summary>
	/// Contains the details about the currently dragging control and target position of the docking control.
	/// </summary>
	[Syncfusion.Documentation.DocumentationExclude()]
	public class DragControlEventArgs: EventArgs
	{
		private DragTarget m_DragTarget = DragTarget.None;
		private Rectangle m_HostDockArea = Rectangle.Empty;

		/// <summary>
		/// Creates a new instances of a class.
		/// </summary>
		public DragControlEventArgs()
		{
		}
		/// <summary>
		/// Overloaded constructor.
		/// </summary>
		public DragControlEventArgs(DragTarget dragTarget, Rectangle hostDockArea)
		{
			m_DragTarget = dragTarget;
			m_HostDockArea = hostDockArea;
		}
		
		/// <summary>
		/// Returns the Target position of the docking windows.
		/// </summary>
		public DragTarget DragTarget
		{
			get { return m_DragTarget; }
		}
		
		/// <summary>
		/// Returns the area of Host form where the docked window is to be docked. 
		/// </summary>
		public Rectangle HostDockArea
		{
			get { return m_HostDockArea; }
		}
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public class AllowHighlightEventArgs: DragControlEventArgs
	{
		private bool m_AllowHighlight = true;

		public bool AllowHighlight
		{
			get { return m_AllowHighlight; }
			set { m_AllowHighlight = value; }
		}

		public AllowHighlightEventArgs( DragTarget dragTarget )
			: base( dragTarget, Rectangle.Empty )
		{
		}
	}

	interface IDragControl
	{
		void ShowControl();
		void HideControl();
		void Dispose();
		bool ProcessMouseMove(Point point);
		bool ProcessMouseUp(Point point);
	}
	interface IArrowDragControl
		: IDragControl
	{
		event MouseEnterHandler OnMouseEnter;
		event MouseLeaveHandler OnMouseLeave;
		event MouseUpHandler OnMouseUp;
		event MouseMoveHandler OnMouseMove;
		event AllowHighlightHandler OnAllowHighlight;
		Rectangle ControllerRect{ get; set; }
		DockAbility DockAbility { get; set; }
		DockAbility OuterDockAbility { get; set; }
	}
	
	interface IDragTargetControl: IDragControl
	{
		Form InternalControl
		{ 
			get; 
		}

		void OnPaint(object sender, PaintEventArgs e);
		void SetBounds(Rectangle bounds);
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public delegate void MouseEnterHandler(object sender, DragControlEventArgs e);
	[Syncfusion.Documentation.DocumentationExclude()]
	public delegate void MouseLeaveHandler(object sender, DragControlEventArgs e);
	[Syncfusion.Documentation.DocumentationExclude()]
	public delegate void MouseUpHandler(object sender, DragControlEventArgs e);
	[Syncfusion.Documentation.DocumentationExclude()]
	public delegate void AllowHighlightHandler(object sender, AllowHighlightEventArgs e);
	[Syncfusion.Documentation.DocumentationExclude()]
	public delegate void MouseMoveHandler(object sender, DragControlEventArgs e);

	internal class DragTargetForm: Form
	{
		#region Class initialize/finalize methods
		public DragTargetForm()
		{
			this.BackColor = SystemColors.Control ;
		}
		~DragTargetForm()
		{
			SystemEvents.UserPreferenceChanged -= (UserPreferenceChangedEventHandler)Delegate.CreateDelegate(
typeof(UserPreferenceChangedEventHandler), this, "UserPreferenceChanged");
		}
		#endregion

		#region Class overrides
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cParams = base.CreateParams;
				
				cParams.ExStyle |= NativeMethods.WS_EX_TOOLWINDOW;

				return cParams;
			}
		}

		protected override void WndProc( ref Message m )
		{
			base.WndProc(ref m);

			if( m.Msg == NativeMethods.WM_NCHITTEST )
			{
				m.Result = (IntPtr)NativeMethods.HTTRANSPARENT;
			}
		}
		#endregion
	}

	internal class DragTargetControl: IDragTargetControl
	{
		protected Form m_internalControl = null;
		protected bool m_mouseIn = false;
		private DragTarget m_DragTarget = DragTarget.None;
		private IArrowDragControl m_ParentControl = null;

		protected Image activeImage = null;
		protected Image inactiveImage = null;
		
		public event MouseEnterHandler OnMouseEnter;
		public event MouseLeaveHandler OnMouseLeave;
		public event MouseUpHandler OnMouseUp;
		public event MouseMoveHandler OnMouseMove;

		public DragTargetControl()
		{
			m_internalControl = new DragTargetForm();
			m_internalControl.ShowInTaskbar = false;
			m_internalControl.FormBorderStyle = FormBorderStyle.None;
			m_internalControl.TransparencyKey = m_internalControl.BackColor;
			m_internalControl.StartPosition = FormStartPosition.Manual;
			m_internalControl.TopMost = true;
			m_internalControl.Paint += new PaintEventHandler(OnPaint);
		}
		
		public IArrowDragControl ParentControl
		{
			get { return m_ParentControl; }
			set { m_ParentControl = value; }
		}
		
		public Image ActiveImage
		{
			get { return activeImage; }
			set { activeImage = value; }
		}

		public Image InactiveImage
		{
			get { return inactiveImage; }
			set { inactiveImage = value; }
		}

		public bool Visible
		{
			get
			{
				if( m_internalControl != null )
				{
					return m_internalControl.Visible;
				}
				else
				{
					return false;
				}
			}
		}

		public virtual Image CurrentImage
		{
			get
			{
				if( m_mouseIn )
				{
					return activeImage;
				}
				else
				{
					return inactiveImage;
				}
			}
		}
		
		public DragTarget DragTarget
		{
			get { return m_DragTarget; }
			set { m_DragTarget = value; }
		}
		
		public virtual Form InternalControl
		{
			get { return m_internalControl; }
		}

		public virtual void ShowControl()
		{
			m_internalControl.Show();
		}

		public virtual void HideControl()
		{
			m_internalControl.Hide();
		}

		public virtual void OnPaint(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawImage(m_mouseIn ? activeImage : inactiveImage, 
				m_internalControl.ClientRectangle);
		}

		public virtual void SetBounds(Rectangle bounds)
		{
			m_internalControl.Bounds = bounds;
		}

		public virtual bool ProcessMouseMove(Point point)
		{
			// Do not process mouse moving when control is invisible
			if( Visible == false )
			{
				return false;
			}

			bool mouseIn = false;
			if( m_internalControl.Bounds.Contains(point) )
			{
				Point pt = m_internalControl.PointToClient(point);
				Bitmap bitmap = CurrentImage as Bitmap;
				if( bitmap != null )
				{
					Color pointColor = bitmap.GetPixel(pt.X, pt.Y);
					mouseIn = pointColor.ToArgb() != 0;
				}
			}
						
			if( mouseIn )
				RaiseMouseMoveEvent();

			if( m_mouseIn != mouseIn )
			{
				if( m_mouseIn )
				{
					RaiseMouseLeaveEvent();
				}
				else
				{
					RaiseMouseEnterEvent();
				}
				
				m_mouseIn = !m_mouseIn;
				m_internalControl.Invalidate();
				return true;
			}
			return false;
		}

		protected void RaiseMouseEnterEvent()
		{
			if( OnMouseEnter != null )
			{
				OnMouseEnter( this, new DragControlEventArgs( m_DragTarget, ControllerRect ) );
			}
		}

		protected void RaiseMouseMoveEvent()
		{
			if( OnMouseMove != null )
			{
				OnMouseMove( this, new DragControlEventArgs( m_DragTarget, ControllerRect ) );
			}
		}

		protected void RaiseMouseLeaveEvent()
		{
			if( OnMouseLeave != null )
			{
				OnMouseLeave( this, new DragControlEventArgs( m_DragTarget, ControllerRect ) );
			}
		}

		public virtual bool ProcessMouseUp(Point point)
		{
			if( m_mouseIn == true )
			{
				RaiseMouseUpEvent();
				m_mouseIn = false;
				return true;
			}
			return false;
		}

		protected void RaiseMouseUpEvent()
		{
			if( OnMouseUp != null )
				OnMouseUp( this, new DragControlEventArgs( m_DragTarget, ControllerRect ) );
		}

		public Rectangle ControllerRect
		{
			get { return m_ParentControl.ControllerRect; }
		}

		public virtual void Dispose()
		{
			this.activeImage = null;
			this.inactiveImage = null;
			if (m_internalControl != null)
			{
				m_internalControl.Paint -= new PaintEventHandler(OnPaint);
				this.m_internalControl.Dispose();
				this.m_internalControl = null;
			}
			this.m_ParentControl = null;
		}
	}

	internal class DragControl: IDragControl
	{
		protected IArrowDragControl m_OuterControl = null;
		protected IArrowDragControl m_InnerControl = null;
		protected Image[] m_Images = new Image[10];
		
		private Rectangle m_OuterControllerRect = Rectangle.Empty;
		private Rectangle m_InnerControllerRect = Rectangle.Empty;
		protected bool m_bVisible = false;
		protected bool m_bInnerVisible = false;
		protected bool m_bOuterVisible = false;

		protected DockAbility m_DockAbility;
		protected DockAbility m_OuterDockAbility;

		public event MouseEnterHandler OnMouseEnter;
		public event MouseLeaveHandler OnMouseLeave;
		public event MouseUpHandler OnMouseUp;
		public event AllowHighlightHandler OnAllowHighlight;

		public DragControl()
		{
			InitializeImages();

			m_OuterControl = new DragOuterControl(this);
			m_OuterControl.OnMouseEnter += new MouseEnterHandler(MouseEnterEvent);
			m_OuterControl.OnMouseLeave += new MouseLeaveHandler(MouseLeaveEvent);
			m_OuterControl.OnMouseUp += new MouseUpHandler(MouseUpEvent);

			m_InnerControl = new DragInnerControl(this);
			m_InnerControl.OnMouseEnter += new MouseEnterHandler(MouseEnterEvent);
			m_InnerControl.OnMouseLeave += new MouseLeaveHandler(MouseLeaveEvent);
			m_InnerControl.OnMouseUp += new MouseUpHandler(MouseUpEvent);
			m_InnerControl.OnAllowHighlight += new AllowHighlightHandler(AllowHighlightEvent);
		}
		
		protected virtual void InitializeImages()
		{
			string path = DragControlConsts.DEF_IMAGES_PATH;
			int index = 0;
			Bitmap bitmap = null;
			bitmap = (Bitmap) ImageLoader.Load(path+DragControlConsts.DEF_INACTIVE_TARGET_IMAGE_NAME);
			m_Images[index++] = new Bitmap(bitmap);
			bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
			m_Images[index++] = new Bitmap(bitmap);
			bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
			m_Images[index++] = new Bitmap(bitmap);
			bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
			m_Images[index++] = new Bitmap(bitmap);
			bitmap = (Bitmap) ImageLoader.Load(path+DragControlConsts.DEF_INACTIVE_TAB_IMAGE_NAME);
			m_Images[index++] = new Bitmap(bitmap);

			bitmap = (Bitmap) ImageLoader.Load(path+DragControlConsts.DEF_ACTIVE_TARGET_IMAGE_NAME);
			m_Images[index++] = new Bitmap(bitmap);
			bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
			m_Images[index++] = new Bitmap(bitmap);
			bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
			m_Images[index++] = new Bitmap(bitmap);
			bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
			m_Images[index++] = new Bitmap(bitmap);
			bitmap = (Bitmap) ImageLoader.Load(path+DragControlConsts.DEF_ACTIVE_TAB_IMAGE_NAME);
			m_Images[index] = new Bitmap(bitmap);

		}
		
		public Bitmap GetImage(int index)
		{
			return m_Images[index] as Bitmap;
		}
		
		protected void MouseEnterEvent(object sender, DragControlEventArgs e)
		{
			if( OnMouseEnter != null )
			{
				if( sender == m_InnerControl && m_bInnerVisible )
					OnMouseEnter(sender, e);
				else if( m_bOuterVisible )
					OnMouseEnter(sender, e);
			}
		}

		protected void MouseLeaveEvent(object sender, DragControlEventArgs e)
		{
			if( OnMouseLeave != null )
			{
				OnMouseLeave(sender, e);
			}
		}

		protected void MouseUpEvent(object sender, DragControlEventArgs e)
		{
			if( OnMouseUp != null )
			{
				if( sender == m_InnerControl && m_bInnerVisible )
					OnMouseUp(sender, e);
				else if( m_bOuterVisible )
					OnMouseUp( sender, e );
			}
		}

		protected void OuterMouseMoveEvent(object sender, DragControlEventArgs e)
		{ 
			if( OnMouseEnter != null )
			{
				OnMouseEnter(sender, e);
			}
		}

		protected void AllowHighlightEvent(object sender, AllowHighlightEventArgs e)
		{
			if( OnAllowHighlight != null )
			{
				OnAllowHighlight(sender, e);
			}
		}

		public Rectangle OuterControllerRect
		{
			get { return m_OuterControllerRect; }
			set 
			{ 
				m_OuterControllerRect = value; 
				m_OuterControl.ControllerRect = value;
			}
		}

		public Rectangle InnerControllerRect
		{
			get { return m_InnerControllerRect; }
			set
			{
				if( m_InnerControllerRect != value )
				{
					m_InnerControllerRect = value;
					m_InnerControl.ControllerRect = value;
				}
			}
		}

		public bool InnerVisible
		{
			get
			{
				return m_bInnerVisible;
			}
		}

		public bool OuterVisible
		{
			get
			{
				return m_bOuterVisible;
			}
		}

		public bool Visible		
		{
			get
			{
				return m_bInnerVisible || m_bOuterVisible;
			}			
		}

		public virtual DockAbility DockAbility
		{
			get
			{
				return m_DockAbility;
			}
			set
			{
				if( m_DockAbility != value )
				{
					this.m_DockAbility = value;
					m_InnerControl.DockAbility = this.m_DockAbility;
                    // In order to DockAbility changes take effect, hide and show inner drag control.
                    if (InnerVisible)
                    {
                        m_InnerControl.HideControl();
                        m_InnerControl.ShowControl();
                    }
				}
			}
		}

		public DockAbility OuterDockAbility
		{
			get 
			{
				return m_OuterDockAbility;
			}
			set
			{
				if( m_OuterDockAbility != value )
				{
					m_OuterDockAbility = value;
                    // In order to OuterDockAbility changes take effect, hide and show outer drag control.
                    if (Visible)
                    {
                        m_OuterControl.HideControl();
                        m_OuterControl.ShowControl();
                    }
				}
			}
		}
		
		public virtual void ShowControl()
		{
			if( !m_bOuterVisible )
				ShowOuterControl();
			if( !m_bInnerVisible )
				ShowInnerControl();

			m_bVisible = true;
		}

		public virtual void HideControl()
		{
			if( m_bOuterVisible )
				HideOuterControl();
			if( m_bInnerVisible )
				HideInnerControl();

			m_bVisible = false;
		}

		public virtual void ShowOuterControl()
		{
			m_OuterControl.ShowControl();
			m_bOuterVisible = true;
		}

		public virtual void HideOuterControl()
		{
			m_OuterControl.HideControl();
			m_bOuterVisible = false;
		}

		public virtual void ShowInnerControl()
		{
			m_InnerControl.ShowControl();
			m_bInnerVisible = true;
		}

		public virtual void HideInnerControl()
		{
			m_InnerControl.HideControl();
			m_bInnerVisible = false;
		}

		public virtual bool ProcessMouseMove(Point point)
		{
			if( !m_InnerControl.ProcessMouseMove(point) )
				return m_OuterControl.ProcessMouseMove(point);
			return true;
		}

		public virtual bool ProcessMouseUp(Point point)
		{
			if( !m_InnerControl.ProcessMouseUp(point) )
				return m_OuterControl.ProcessMouseUp(point);
			return true;
		}

		public Rectangle ControllerRect
		{
			get { return Rectangle.Empty; }
		}

		public virtual void Dispose()
		{
			this.m_Images = null;

			if (m_OuterControl != null)
			{
				m_OuterControl.OnMouseEnter -= new MouseEnterHandler(MouseEnterEvent);
				m_OuterControl.OnMouseLeave -= new MouseLeaveHandler(MouseLeaveEvent);
				m_OuterControl.OnMouseUp -= new MouseUpHandler(MouseUpEvent);
				this.m_OuterControl.Dispose();
				this.m_OuterControl = null;
			}
			if (m_InnerControl != null)
			{
				m_InnerControl.OnMouseEnter -= new MouseEnterHandler(MouseEnterEvent);
				m_InnerControl.OnMouseLeave -= new MouseLeaveHandler(MouseLeaveEvent);
				m_InnerControl.OnMouseUp -= new MouseUpHandler(MouseUpEvent);
				m_InnerControl.OnAllowHighlight -= new AllowHighlightHandler(AllowHighlightEvent);
				this.m_InnerControl.Dispose();
				this.m_InnerControl = null;
			}
		}

	}

	internal class ImageLoader
	{
		public static Image Load(string path)
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			System.IO.Stream streamImg = asm.GetManifestResourceStream(
				typeof(DockHost), path);

            
			return Image.FromStream(streamImg);
    
		}
	}

	internal class DragOuterControl: IArrowDragControl
	{
		protected ArrayList m_Controls = new ArrayList(4);
		protected DragControl m_ParentControl = null;

		protected const int DEF_LEFT_CONTROL_INDEX = 0;
		protected const int DEF_TOP_CONTROL_INDEX = 1;
		protected const int DEF_RIGHT_CONTROL_INDEX = 2;
		protected const int DEF_BOTTOM_CONTROL_INDEX = 3;

		public event MouseEnterHandler OnMouseEnter;
		public event MouseLeaveHandler OnMouseLeave;
		public event MouseUpHandler OnMouseUp;
		public event MouseMoveHandler OnMouseMove;
		public event AllowHighlightHandler OnAllowHighlight;
		protected Rectangle m_ControllerRect = Rectangle.Empty;

		protected DragTargetControl getControl(int index)
		{
			return m_Controls[index] as DragTargetControl;
		}

		protected DragTargetControl leftControl
		{
			get { return m_Controls[DEF_LEFT_CONTROL_INDEX] as DragTargetControl; }
			set { m_Controls[DEF_LEFT_CONTROL_INDEX] = value; }
		}

		protected DragTargetControl topControl
		{
			get { return m_Controls[DEF_TOP_CONTROL_INDEX] as DragTargetControl; }
			set { m_Controls[DEF_TOP_CONTROL_INDEX] = value; }
		}

		protected DragTargetControl rightControl
		{
			get { return m_Controls[DEF_RIGHT_CONTROL_INDEX] as DragTargetControl; }
			set { m_Controls[DEF_RIGHT_CONTROL_INDEX] = value; }
		}

		protected DragTargetControl bottomControl
		{
			get { return m_Controls[DEF_BOTTOM_CONTROL_INDEX] as DragTargetControl; }
			set { m_Controls[DEF_BOTTOM_CONTROL_INDEX] = value; }
		}

		public virtual DockAbility DockAbility
		{
			get { return DockAbility.All; }
			set {}
		}

		public DockAbility OuterDockAbility
		{
			get
			{
				if( ParentControl != null )
				{
					return ParentControl.OuterDockAbility;
				}
				else
				{
					return DockAbility.All;
				}
			}
			set
			{}
		}

		protected virtual void MouseEnterEvent(object sender, DragControlEventArgs e)
		{
			RaiseOnMouseEnter( sender, e );
		}

		protected void RaiseOnMouseEnter( object sender, DragControlEventArgs e )
		{
			if( OnMouseEnter != null )
			{
				OnMouseEnter( sender, e );
			}
		}

		protected virtual void MouseLeaveEvent(object sender, DragControlEventArgs e)
		{
			RaiseOnMouseLeave( sender, e );
		}

		protected void RaiseOnMouseLeave( object sender, DragControlEventArgs e )
		{
			if( OnMouseLeave != null )
			{
				OnMouseLeave( sender, e );
			}
		}
		
		protected void MouseUpEvent(object sender, DragControlEventArgs e)
		{
			if( OnMouseUp != null ) 
			{
				OnMouseUp(sender, e);
			}
		}

		protected void MouseMoveEvent(object sender, DragControlEventArgs e)		
		{
			if (OnMouseMove != null)
			{
				OnMouseMove(sender, e);
			}
		}

		protected void AllowHighlightEvent(object sender, AllowHighlightEventArgs e)
		{
			if (OnAllowHighlight != null)
			{
				OnAllowHighlight(sender, e);
			}
		}

		public DragOuterControl(DragControl parentControl)
		{
			m_ParentControl = parentControl;

			InitializeProvider();
		}

		protected virtual void InitializeProvider()
		{
			for(int i = 0; i < 4; i++ )
			{
				DragTargetControl control = new DragTargetControl();
				m_Controls.Add(control);
				control.OnMouseEnter += new MouseEnterHandler(MouseEnterEvent);
				control.OnMouseLeave += new MouseLeaveHandler(MouseLeaveEvent);
				control.OnMouseUp += new MouseUpHandler(MouseUpEvent);				
				control.ParentControl = this;
			}

			leftControl.DragTarget = DragTarget.OuterLeft;
			leftControl.ActiveImage = ParentControl.GetImage(DragControlConsts.DEF_LEFT_ACTIVE_TARGET);
			leftControl.InactiveImage = ParentControl.GetImage(DragControlConsts.DEF_LEFT_INACTIVE_TARGET);

			topControl.DragTarget = DragTarget.OuterTop;
			topControl.ActiveImage = ParentControl.GetImage(DragControlConsts.DEF_TOP_ACTIVE_TARGET);
			topControl.InactiveImage = ParentControl.GetImage(DragControlConsts.DEF_TOP_INACTIVE_TARGET);

			rightControl.DragTarget = DragTarget.OuterRight;
			rightControl.ActiveImage = ParentControl.GetImage(DragControlConsts.DEF_RIGHT_ACTIVE_TARGET);
			rightControl.InactiveImage = ParentControl.GetImage(DragControlConsts.DEF_RIGHT_INACTIVE_TARGET);

			bottomControl.DragTarget = DragTarget.OuterBottom;
			bottomControl.ActiveImage = ParentControl.GetImage(DragControlConsts.DEF_BOTTOM_ACTIVE_TARGET);
			bottomControl.InactiveImage = ParentControl.GetImage(DragControlConsts.DEF_BOTTOM_INACTIVE_TARGET);
		}
		
		public Image GetImage(int index)
		{
			return ParentControl.GetImage(index);
		}
		
		public DragControl ParentControl
		{
			get { return m_ParentControl; }
			set { m_ParentControl = value; }
		}
		
		public virtual Rectangle ControllerRect
		{
			get { return m_ControllerRect; }
			set 
			{
				m_ControllerRect = value; 
			
				Rectangle rcLeftBounds = new Rectangle();
				rcLeftBounds.X = value.Left + DragControlConsts.DEF_TARGET_MARGIN;
				rcLeftBounds.Y = value.Top + (value.Height - leftControl.InactiveImage.Height) / 2;
				rcLeftBounds.Width = leftControl.InactiveImage.Width;
				rcLeftBounds.Height = leftControl.InactiveImage.Height;

				Rectangle rcTopBounds = new Rectangle();
				rcTopBounds.X = value.Left + (value.Width - topControl.InactiveImage.Width) / 2;
				rcTopBounds.Y = value.Top + DragControlConsts.DEF_TARGET_MARGIN;
				rcTopBounds.Width = topControl.InactiveImage.Width;
				rcTopBounds.Height = topControl.InactiveImage.Height;

				Rectangle rcRightBounds = new Rectangle();
				rcRightBounds.X = value.Left + value.Width - rightControl.InactiveImage.Width 
					- DragControlConsts.DEF_TARGET_MARGIN;
				rcRightBounds.Y = value.Top + (value.Height - rightControl.InactiveImage.Height) / 2;
				rcRightBounds.Width = rightControl.InactiveImage.Width;
				rcRightBounds.Height = rightControl.InactiveImage.Height;

				Rectangle rcBottomBounds = new Rectangle();
				rcBottomBounds.X = value.Left + (value.Width - bottomControl.InactiveImage.Width) / 2;
				rcBottomBounds.Y = value.Top + value.Height - bottomControl.InactiveImage.Height
					- DragControlConsts.DEF_TARGET_MARGIN;
				rcBottomBounds.Width = bottomControl.InactiveImage.Width;
				rcBottomBounds.Height = bottomControl.InactiveImage.Height;

				// When bounds of controller are too small for arrows then don't show them.
				if (rcLeftBounds.Right > rcRightBounds.Left)
				{
				    leftControl.SetBounds(Rectangle.Empty);
				    rightControl.SetBounds(Rectangle.Empty);
				}
				else
				{
					leftControl.SetBounds(rcLeftBounds);
					rightControl.SetBounds(rcRightBounds);
				}

				if (rcTopBounds.Bottom > rcBottomBounds.Top)
				{
				    topControl.SetBounds(Rectangle.Empty);
				    bottomControl.SetBounds(Rectangle.Empty);
				}
				else
				{
					topControl.SetBounds(rcTopBounds);
					bottomControl.SetBounds(rcBottomBounds);
				}
			}
		}
		
		public virtual void ShowControl()
		{
			if( OuterDockAbility == DockAbility.All )
			{
				for( int i = 0; i < m_Controls.Count; i++ )
				{
					getControl(i).ShowControl();
				}
			}
			else
			{
				if( (OuterDockAbility | DockAbility.Left) == OuterDockAbility )
				{
					getControl(DEF_LEFT_CONTROL_INDEX).ShowControl();
				}
				if( (OuterDockAbility | DockAbility.Top) == OuterDockAbility )
				{
					getControl(DEF_TOP_CONTROL_INDEX).ShowControl();
				}
				if( (OuterDockAbility | DockAbility.Right) == OuterDockAbility )
				{
					getControl(DEF_RIGHT_CONTROL_INDEX).ShowControl();
				}
				if( (OuterDockAbility | DockAbility.Bottom) == OuterDockAbility )
				{
					getControl(DEF_BOTTOM_CONTROL_INDEX).ShowControl();
				}
			}
		}

		public virtual void HideControl()
		{
			for( int i = 0; i < m_Controls.Count; i++ )
			{
				getControl(i).HideControl();
			}
		}

		public virtual bool	ProcessMouseDown(Point point)
		{
			return true;
		}
		
		public virtual bool ProcessMouseMove(Point point)
		{
			for( int i = 0; i < m_Controls.Count; i++ )
			{
				if( getControl(i).ProcessMouseMove(point) )
					return true;
			}
			return false;
		}

		public virtual bool ProcessMouseUp(Point point)
		{
			for( int i = 0; i < m_Controls.Count; i++ )
			{
				if( getControl(i).ProcessMouseUp(point) )
					return true;
			}
			return false;
		}

		public virtual void Dispose()
		{
			for (int i = 0; i < 4; i++)
			{
				DragTargetControl control = m_Controls[i] as DragTargetControl;
				if (control != null)
				{
					control.OnMouseEnter -= new MouseEnterHandler(MouseEnterEvent);
					control.OnMouseLeave -= new MouseLeaveHandler(MouseLeaveEvent);
					control.OnMouseUp -= new MouseUpHandler(MouseUpEvent);
					control.ParentControl = null;
					control.Dispose();
				}
			}
			m_Controls.Clear();
			m_Controls = null;
			this.m_ParentControl = null;
		}
	}
	
	internal class DragInnerControl
		: DragTargetControl
		, IArrowDragControl
	{
		private Rectangle m_ControllerRect = Rectangle.Empty;
		private Size m_ControlSize = new Size(121, 121);
		protected DragControl m_ParentControl = null;
		protected Point m_Center = Point.Empty;

		protected Bitmap[] m_Bitmaps = new Bitmap[DragControlConsts.DEF_INNER_CONTROL_COUNT];

		protected Rectangle[] m_Rectangles = new Rectangle[DragControlConsts.DEF_INNER_CONTROL_COUNT];

		protected DockAbility m_DockAbility;

		public new event MouseEnterHandler OnMouseEnter;
		public new event MouseLeaveHandler OnMouseLeave;
		public new event MouseUpHandler OnMouseUp;
		public event AllowHighlightHandler OnAllowHighlight;

		public new DragControl ParentControl
		{
			get { return m_ParentControl; }
			set { m_ParentControl = value; }
		}

		public Point Center 
		{
			get { return m_Center; }
		}
		
		public new Rectangle ControllerRect 
		{
			get { return m_ControllerRect; }
			set
			{
				m_ControllerRect = value;
				UpdateControlPosition();
				if( m_internalControl.Visible )
					m_internalControl.BringToFront();
			}
		}

		public DockAbility DockAbility
		{
			get
			{
				return m_DockAbility;
			}
			set
			{
				if( m_DockAbility != value )
				{
					m_DockAbility = value;
					for( int i = 0; i < DragControlConsts.DEF_INNER_CONTROL_COUNT; i++ )
					{
						InvalidateImage(i);
					}
				}
			}
		}

		public DockAbility OuterDockAbility
		{
			get
			{
				if( ParentControl != null )
				{
					return ParentControl.OuterDockAbility;
				}
				else
				{
					return DockAbility.All;
				}
			}
			set
			{}
		}

		public DragInnerControl(DragControl parentControl)
		{
			m_ParentControl = parentControl;

			InitializeControl();

			CalculateControlsPosition();
		}

		protected virtual void InitializeControl()
		{
			m_internalControl = new DragTargetForm();
			m_internalControl.ShowInTaskbar = false;
			m_internalControl.FormBorderStyle = FormBorderStyle.None;
			m_internalControl.TransparencyKey = m_internalControl.BackColor;
			m_internalControl.StartPosition = FormStartPosition.Manual;
			m_internalControl.TopMost = true;
			m_internalControl.Paint += new PaintEventHandler(OnPaint);
	
			for( int i = 0; i < DragControlConsts.DEF_INNER_CONTROL_COUNT; i++ )
			{
				m_Bitmaps[i] = new Bitmap(m_ParentControl.GetImage(i));
			}
		}

		protected virtual void CalculateControlsPosition()
		{
			m_Center.X = (m_ControlSize.Width + 1) / 2;
			m_Center.Y = (m_ControlSize.Height + 1) / 2;

			m_Rectangles[4] = new Rectangle(
				m_Center.X - (m_TabBitmap.Width - 1) / 2,
				m_Center.Y - (m_TabBitmap.Height - 1) / 2,
				m_TabBitmap.Width, m_TabBitmap.Height);

			m_Rectangles[0] = new Rectangle(
				m_TabRectangle.X - m_LeftBitmap.Width,	
				m_Center.Y - (m_LeftBitmap.Height - 1) / 2,
				m_LeftBitmap.Width, m_LeftBitmap.Height);

			m_Rectangles[1] = new Rectangle(
				m_Center.X  - (m_TopBitmap.Width - 1) / 2, 
				m_TabRectangle.Y - m_TopBitmap.Height, 
				m_TopBitmap.Width, m_TopBitmap.Height);

			m_Rectangles[2] = new Rectangle(
				m_TabRectangle.X + m_TabBitmap.Width,
				m_Center.Y - (m_RightBitmap.Height - 1) / 2,
				m_RightBitmap.Width, m_RightBitmap.Height);

			m_Rectangles[3] = new Rectangle(
				m_Center.X - (m_BottomBitmap.Width - 1) / 2,
				m_TabRectangle.Y + m_TabBitmap.Height,
				m_BottomBitmap.Width, m_BottomBitmap.Height);
		}
		
		private Bitmap m_LeftBitmap
		{
			get { return m_Bitmaps[0]; }
		}

		private Bitmap m_TopBitmap
		{
			get { return m_Bitmaps[1]; }
		}

		private Bitmap m_RightBitmap
		{
			get { return m_Bitmaps[2]; }
		}

		private Bitmap m_BottomBitmap
		{
			get { return m_Bitmaps[3]; }
		}

		private Bitmap m_TabBitmap
		{
			get { return m_Bitmaps[4]; }
		}
		
		protected Rectangle m_LeftRectangle
		{
			get { return m_Rectangles[0]; }
		}

		protected Rectangle m_TopRectangle
		{
			get { return m_Rectangles[1]; }
		}

		protected Rectangle m_RightRectangle
		{
			get { return m_Rectangles[2]; }
		}

		protected Rectangle m_BottomRectangle
		{
			get { return m_Rectangles[3]; }
		}

		protected Rectangle m_TabRectangle
		{
			get { return m_Rectangles[4]; }
		}

		public override void ShowControl()
		{
			m_internalControl.Show();
		}

		public override void HideControl()
		{
			m_internalControl.Hide();
		}

		public override void OnPaint(object sender, PaintEventArgs e)
		{
			if( DockAbility == DockAbility.None )
			{
				return;
			}

			Graphics graphics = e.Graphics;
			Pen pen = new Pen(Color.FromArgb(177, 177, 177), 1);
			Point center = Center;

			Point[] borderPoints = new Point[] 
			{
				new Point(center.X - DragControlConsts.DEF_INNER_CONTROL_MARGIN - 1, center.Y),
				new Point(center.X, center.Y - DragControlConsts.DEF_INNER_CONTROL_MARGIN - 1),
				new Point(center.X + DragControlConsts.DEF_INNER_CONTROL_MARGIN + 1, center.Y),
				new Point(center.X, center.Y + DragControlConsts.DEF_INNER_CONTROL_MARGIN + 1)
			};

			graphics.DrawPolygon(pen, borderPoints);

			Point[] polygonPoints = new Point[] 
			{
				new Point(center.X - DragControlConsts.DEF_INNER_CONTROL_MARGIN, center.Y),
				new Point(center.X, center.Y - DragControlConsts.DEF_INNER_CONTROL_MARGIN),
				new Point(center.X + DragControlConsts.DEF_INNER_CONTROL_MARGIN, center.Y),
				new Point(center.X, center.Y + DragControlConsts.DEF_INNER_CONTROL_MARGIN)
			};

			pen.Color = Color.FromArgb(245, 238, 238);
			graphics.FillPolygon(pen.Brush, polygonPoints);
			if( (DockAbility | DockAbility.Left) == DockAbility )
			{
				graphics.DrawImage( m_Bitmaps[0], m_Rectangles[0] );
			}
			if( (DockAbility | DockAbility.Top) == DockAbility )
			{
				graphics.DrawImage( m_Bitmaps[1], m_Rectangles[1] );
			}
			if( (DockAbility | DockAbility.Right) == DockAbility )
			{
				graphics.DrawImage( m_Bitmaps[2], m_Rectangles[2] );
			}
			if( (DockAbility | DockAbility.Bottom) == DockAbility )
			{
				graphics.DrawImage( m_Bitmaps[3], m_Rectangles[3] );
			}
			if( (DockAbility | DockAbility.Tabbed) == DockAbility )
			{
				graphics.DrawImage( m_Bitmaps[4], m_Rectangles[4] );
			}
		}

		protected int m_ActiveControlIndex = -1;
		
		public override bool ProcessMouseMove(Point point)
		{
			bool result = false;
			point = m_internalControl.PointToClient(point);
			int activeControlIndex = -1;
			for( int i = 0; i < DragControlConsts.DEF_INNER_CONTROL_COUNT; i++ )
			{
				if( m_Rectangles[i].Contains(point) )
				{
					point.X -= m_Rectangles[i].Location.X;
					point.Y -= m_Rectangles[i].Location.Y;
					Color pointColor = m_Bitmaps[i].GetPixel(point.X, point.Y);
					if( pointColor.ToArgb() != 0 )
					{
						switch( i )
						{
							case 0: 
								if( (DockAbility | DockAbility.Left) == DockAbility )
								{
									activeControlIndex = i;
								}
								break;
							case 1: 
								if( (DockAbility | DockAbility.Top) == DockAbility )
								{
									activeControlIndex = i;
								}
								break;
							case 2: 
								if( (DockAbility | DockAbility.Right) == DockAbility )
								{
									activeControlIndex = i;
								}
								break;
							case 3: 
								if( (DockAbility | DockAbility.Bottom) == DockAbility )
								{
									activeControlIndex = i;
								}
								break;
							case 4: 
								if( (DockAbility | DockAbility.Tabbed) == DockAbility )
								{
									activeControlIndex = i;
								}
								break;
						}
						result = true;
						break;
					}
				}
			}
			
			if( m_ActiveControlIndex != activeControlIndex )
			{
				if( m_ActiveControlIndex != -1 )
				{
					m_Bitmaps[m_ActiveControlIndex] = m_ParentControl.GetImage(
						m_ActiveControlIndex);
					if( this.OnMouseLeave != null )
					{
						string currentTargetStr = Enum.GetName(DragTarget.GetType(), m_ActiveControlIndex);
						DragTarget currentDragTarget = (DragTarget) Enum.Parse(DragTarget.GetType(), currentTargetStr);
						OnMouseLeave(this, new DragControlEventArgs(currentDragTarget, ControllerRect));
					}
					InvalidateImage( m_ActiveControlIndex );
				}

				bool allowHighlight = false;
				string targetStr = Enum.GetName(DragTarget.GetType(), activeControlIndex);
				DragTarget dragTarget = (DragTarget) Enum.Parse(DragTarget.GetType(), targetStr);
				if( OnAllowHighlight != null )
				{
					AllowHighlightEventArgs args = new AllowHighlightEventArgs( dragTarget );
					OnAllowHighlight( this, args );
					allowHighlight = args.AllowHighlight;
				}

				if( allowHighlight )
				{
					if( activeControlIndex != -1 )
					{
						m_Bitmaps[activeControlIndex] = m_ParentControl.GetImage(
							activeControlIndex + DragControlConsts.DEF_INNER_CONTROL_COUNT);
						if( OnMouseEnter != null )
						{
							OnMouseEnter(this, new DragControlEventArgs(dragTarget, ControllerRect));
						}
						InvalidateImage( activeControlIndex );
					}
					m_ActiveControlIndex = activeControlIndex;
				}
			}

			return result;
		}

		public override bool ProcessMouseUp(Point point)
		{
			if( (m_ActiveControlIndex != -1) && (OnMouseUp != null) )
			{
				string targetStr = Enum.GetName(DragTarget.GetType(), m_ActiveControlIndex);
				DragTarget dragTarget = (DragTarget) Enum.Parse(DragTarget.GetType(), targetStr);
				OnMouseUp(this, new DragControlEventArgs(dragTarget, ControllerRect));
			}
			return m_ActiveControlIndex != -1;
		}
		
		private void InvalidateImage(int index)
		{
			m_internalControl.Invalidate( m_Rectangles[index] ) ;
		}

		private void UpdateControlPosition()
		{
			if (this is VS2012InnerDragControl)
			{
				m_ControlSize = new Size(131, 131);
			}
			m_internalControl.Size = m_ControlSize;
			int x = m_ControllerRect.X + (m_ControllerRect.Width - m_ControlSize.Width) / 2;
			int y = m_ControllerRect.Y + (m_ControllerRect.Height - m_ControlSize.Height) / 2;
			m_internalControl.Location = new Point(x, y);
		}

		public override void Dispose()
		{
			base.Dispose();

			this.m_Bitmaps = null;
			if (m_internalControl != null)
			{
				m_internalControl.Paint -= new PaintEventHandler(OnPaint);
				this.m_internalControl.Dispose();
				this.m_internalControl = null;
			}
			this.m_ParentControl = null;
		}
	}

	/// <summary>
	/// Contains the details of the mouse and painting messages for internal use.
	/// </summary>
	[Syncfusion.Documentation.DocumentationExclude()]
	public class DragMessageFilter : IMessageFilter
	{
		internal ArrayList Provider = new ArrayList();
		/// <summary>
		/// Returns the true if the messages are either painting or mouse. Otherwise it will return false. 
		/// </summary>
		public bool PreFilterMessage(ref Message m)
		{
			if( Provider.Count == 0 )
				return false;

			// Ignore all non-WM_LBUTTONXXX mouse and paint messages
			if( (m.Msg >= 0x0204/*WM_RBUTTONDOWN*/ && m.Msg <= 0x0209/*WM_MBUTTONDBLCLK*/) 
				//||(m.Msg == 0x000F/*WM_PAINT*/) 
				|| (m.Msg == 0x0014/*WM_ERASEBKGND*/) )
			{
				foreach( CustomDragProvider dragp in Provider )
				{
					if( dragp.DraggingControl != null )
						return true;
				}
				return false;
			}

			if( m.Msg == 0x0100/*WM_KEYDOWN*/ ) 
			{
				if((int)m.WParam == 0x1B/*VK_ESCAPE*/)
				{
					foreach( CustomDragProvider dragp in Provider )
					{
						if( dragp.DraggingControl != null )
						{
							if( !(dragp is BorderDragProvider)
								|| !dragp.DraggingControl.InternalController.DockingManager.DesignMode )
								dragp.DraggingControl.AbortDrag();
						}
						else
						{
							dragp.AllowDrag = false;
						}
					}
				}
				if((int)m.WParam == 0x11/*VK_CONTROL*/)
				{
					foreach( CustomDragProvider dragp in Provider )
						dragp.ProcessCtrlKeyDown();
				}
			}

			if (m.Msg == 0x0101 /*WM_KEYUP*/)
			{
				if ((int)m.WParam == 0x1B/*VK_ESCAPE*/ )
				{
					foreach( CustomDragProvider dragp in Provider )
						dragp.AllowDrag = true;
				}
				if((int)m.WParam == 0x11/*VK_CONTROL*/)
				{
					foreach( CustomDragProvider dragp in Provider )
						dragp.ProcessCtrlKeyUp();
				}
			}

			return false;
		}
	}
	
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IDragProvider
	{
		IDraggable DraggingControl
		{
			get; set;
		}
		bool CanFloatWhenDisallowFloating{ get; }
		bool SingleTabOperate { get; set; }

		void ProcessMouseDown( DockControllerBase controller, IDraggable ctrl, Point ptscreen );
		void ProcessMouseUp( DockControllerBase controller, IDraggable ctrl, Point ptscreen );
		void ProcessMouseMove( DockControllerBase controller, IDraggable ctrl, Point ptscreen );
		void ProcessDoubleClick();
		void TerminateDrag( IDraggable ctrl, Point ptscreen );
		Control GetUnderlyingControl(Point point);
		void UpdateColors();
		void ForceStopDrag();
		void Dispose();
		bool AllowDrag { get; set; }
		void ProcessCtrlKeyDown();
		void ProcessCtrlKeyUp();
	}
	/// <summary>
	/// Custom Drag Provider class will provide the option to change the default drag provider style. 
	/// </summary>
	[Syncfusion.Documentation.DocumentationExclude()]
	public class CustomDragProvider: IDragProvider
	{
		/// <summary>
		/// Static field to get or set the bool value whether the DisallowFloating cursor should be displayed or not.
		/// </summary>
		static public bool ShowDisallowFloatCursor = true;
		protected bool m_singleTabOperate = false;

		internal IFrameBorderPainter Painter
		{
			get { return DockingManager.FramePainter.Painter; }
		}
		public virtual bool CanFloatWhenDisallowFloating
		{
			get
			{
				return false;
			}
		}

		public virtual bool SingleTabOperate
		{
			get
			{
				return m_singleTabOperate;
			}
			set
			{
				if( m_singleTabOperate != value )
				{
					m_singleTabOperate = value;
				}
			}
		}
		
		/// <summary>
		/// Indicates the flag to Initiate the drag or not
		/// </summary>
		protected bool bInitiateDrag = false;
		
		/// <summary>
		/// Specifies the current point of the dragging window.
		/// </summary>
		protected Point ptMoveStart = Point.Empty;

		/// <summary>
		/// Specifies the initial position of the dragging point.
		/// </summary>
		protected Point ptDelta = Point.Empty;

		/// <summary>
		/// Instance of the DragMessageFilter class to specify the messages of Mouse and Paint messages.
		/// </summary>
		protected static DragMessageFilter MessageFilter = new DragMessageFilter();

		private IDraggable m_DraggingControl = null;
		protected DockingManager m_DockingManager = null;
		protected int m_borderWidth = 0;
		protected bool m_bAllowDrag = true;

		/// <summary>
		/// Specifies whether dragging of controls is allowed.
		/// </summary>
		public bool AllowDrag
		{
			get 
			{
				return m_bAllowDrag;
			}
			set
			{
				if (m_bAllowDrag != value)
				{
					m_bAllowDrag = value;
				}
			}
		}
		
		/// <summary>
		/// Specifies the currently being dragged control.
		/// </summary>
		public IDraggable DraggingControl
		{
			get { return m_DraggingControl; }
			set { m_DraggingControl = value; }
		}

		public virtual void ProcessCtrlKeyDown()
		{}

		public virtual void ProcessCtrlKeyUp()
		{}

		public virtual void UpdateColors()
		{}
		/// <summary>
		/// Gets/sets the docking manager instance.
		/// </summary>
		public DockingManager DockingManager
		{
			get { return m_DockingManager; }
			set { m_DockingManager = value; }
		}

		public virtual void Dispose()
		{
			MessageFilter.Provider.Remove( this );
			this.m_DockingManager = null;
			this.m_DraggingControl = null;
			if( MessageFilter.Provider.Count == 0 )
				Application.RemoveMessageFilter( MessageFilter );
		}
		/// <summary>
		/// Returns if it is allowed do dock IDraggable control to specified target controller.
		/// </summary>
		/// <param name="ctrl">Dragging control.</param>
		/// <param name="targetCtrl">Dock target control.</param>
		/// <returns>Is dock operation allowed.</returns>
		protected virtual bool IsDockAllowed( IDraggable ctrl, DockControllerBase targetCtrl )
		{
			bool allowDock = true;

			if( null != ctrl )
			{
				if( ( null == targetCtrl || targetCtrl.Floating )
					&& ( !CanFloat( ctrl.InternalController ) || this.DockingManager.DisallowFloating ) )
					allowDock = false;
			}

			return allowDock;
		}
		/// <summary>
		/// Returns if it is allowed do dock IDraggable control to internal target controller.
		/// </summary>
		/// <param name="ctrl">Dragging control.</param>
		/// <returns>Is dock operation allowed.</returns>
		protected virtual bool IsDockAllowed( IDraggable ctrl )
		{
			if( ctrl == null )
				return false;

			return IsDockAllowed(ctrl, ctrl.DragDockInfo.dController);
		}

		protected virtual Point CalculateFormOffset( IDraggable ctrl, Point ptClient )
		{
			Point offset = Point.Empty;
			int borderHWidth;
			double captionHeight;
			if( DockingManager.VisualStyle == VisualStyle.Default
				|| DockingManager.VisualStyle == VisualStyle.VS2005 )
				captionHeight = SystemInformation.ToolWindowCaptionHeight;
			else
				captionHeight = DockingManager.Renderer.CaptionWidth;

			GetYOffset(out borderHWidth, ref ptClient );

			Size prevSize = Size.Empty;
			if( ctrl is DockHost )
				prevSize = ((DockHostController)ctrl.InternalController).DIPrevious.rcDockArea.Size;
			int currWidth = ctrl.InternalController.HostControl.Width;

			if( (ctrl is DockHost) 
				&& (ctrl as DockHost).InternalController.ParentController is DockTabController 
				&& DockingManager.DesignMode == false
				&& ((ctrl as DockHost).InternalController.ParentController as DockTabController).dragTabPage != null )
			{
				if( prevSize != Size.Empty && !ctrl.InternalController.Floating )
				{
					offset = new Point( (int)( ( (float)ptClient.X / (float)currWidth ) * prevSize.Width ),
						(int)captionHeight/2);
				}
				else
				{
					offset = new Point( (int)( ( (float)ptClient.X / (float)currWidth ) * ctrl.DragRectangle.Width ),
						(int)captionHeight/2);
				}
			}
			else
			{
				if( ctrl.InternalController.Floating && !(ctrl.InternalController.ParentController is SizingController) )
				{
					offset = new Point(ptClient.X ,ptClient.Y );
				}
				else
				{
					offset = new Point( (int)( ( (float)ptClient.X / ( (float)currWidth ) ) * ctrl.DragRectangle.Width ),
						ptClient.Y );
				}
			}
			if( ctrl is DockHost )
			{
				if( offset.X < borderHWidth )
					offset.X += borderHWidth;
				if( offset.Y < borderHWidth )
					offset.Y += borderHWidth;
			}
			int width = prevSize.Width == 0 ? currWidth	: prevSize.Width;

			if( ctrl is DockHost )
				if( offset.X > ( width - captionHeight ) )
					offset.X -= (int)captionHeight;

			return offset;
		}

		protected virtual DockHostController GetUnderlayingController( Control underlaying )
		{
			Control targetCtrl = underlaying;
			DockHost targetHost = targetCtrl as DockHost;
			DockHostController target = null;

			while( targetHost == null )
			{
				targetCtrl = targetCtrl.Parent;

				if( targetCtrl == null )
					break;

				targetHost = targetCtrl as DockHost;
			}

			if( targetHost != null )
				target = targetHost.InternalController as DockHostController;

			return target;
		}

		protected void GetYOffset(out int borderHWidth, ref Point ptClient)
		{
			if ( this.DockingManager.VisualStyle == VisualStyle.Default 
				|| this.DockingManager.VisualStyle == VisualStyle.VS2005 )
			{
				borderHWidth = m_borderWidth = 4;
			}
			else
			{
				borderHWidth = m_borderWidth = DockingManager.Renderer.BorderWidth;
			}              

			if(ptClient.Y < 0)	// ctrl is a floating form with a single DockHost or tabbed DockHost child.
			{
				if( DockingManager.VisualStyle == VisualStyle.Default
					|| DockingManager.VisualStyle == VisualStyle.VS2005 )
				{
					ptClient.Y = SystemInformation.ToolWindowCaptionHeight - Math.Abs(ptClient.Y)+1;
					borderHWidth++;
				}
				else
				{
					ptClient.Y = DockingManager.Renderer.CaptionWidth - Math.Abs(ptClient.Y);
				}
			}
		}

		/// <summary>
		/// </summary>
		static CustomDragProvider()
		{
			Application.AddMessageFilter(MessageFilter);
		}

		/// <summary>
		/// Creates a new instances of a class.
		/// </summary>
		public CustomDragProvider()	{}

		/// <summary>
		/// Overloaded constructor.
		/// </summary>
		public CustomDragProvider( DockingManager dockingManager )
		{
			this.m_DockingManager = dockingManager;
			MessageFilter.Provider.Add( this );
		}
		
		/// <summary>
		/// Overridable method. It gets current the Docking Information
		/// </summary>
		protected void GetNewDockInfo(IDraggable ctrl, Point ptscreen)
		{
			// Call into the docking manager to determine whether the mouse is over a dock controller
			DockControllerBase dc = this.DockingManager.GetDockController(ptscreen);
			if(dc != null)
			{
				if(ctrl.IsSuitableDockTarget(dc) == true)
				{
					// The dock controller uses the existing dockinfo.rcdockArea rect to get the control's size.
					// This circuitous approach allows us to specify a drag rect that is different than the control's
					// current size. Comes in handy when dragging off a docked host.
					ctrl.DragDockInfo.rcDockArea = ctrl.DragRectangle;
					Control dragctrl = ctrl as Control;
					dc.GetDockInfo(dragctrl, ptscreen, ctrl.DragDockInfo);

					// Check with the drag control whether to proceed with the dock
					if(ctrl.QueryDragProceedWithDock() == false)
						dc = null;

					// Boot back to controller to notify user and get the ok for the dock to proceed.
					if((dc != null) && (dc.QueryDropProceedWithDock(dragctrl, ctrl.DragDockInfo.dStyle) == false))
						dc = null;
				}
				else
					dc = null;
			}

			// calculate control location
			Point ptdrag = new Point(ptscreen.X-this.ptDelta.X, ptscreen.Y-this.ptDelta.Y);
			Point location = ctrl.DragDockInfo.rcDockArea.Location;
			switch(ctrl.AllowedDragAxis(ref ptdrag, new Point(ptscreen.X-this.ptMoveStart.X, ptscreen.Y-this.ptMoveStart.Y)))
			{
				case DragAxis.XY:
					location = ptdrag;
					break;
				case DragAxis.X:
					Control ctrlx = ctrl as Control;
					location = new Point(ptdrag.X, ctrlx.PointToScreen(ctrlx.ClientRectangle.Location).Y);
					break;
				case DragAxis.Y:
					Control ctrly = ctrl as Control;
					location = new Point(ctrly.PointToScreen(ctrly.ClientRectangle.Location).X, ptdrag.Y);
					break;
			}
			
			// If a null controller is returned in the dock info, then restore the existing dock info
			if( (dc == null) || (dc.Equals(ctrl.InternalController) == true) || (ctrl.DragDockInfo.dController == null) )
			{
				if(ctrl.InternalController.Floating == true)
				{
					if((ctrl.InternalController is DockHostController) && ((ctrl.InternalController as DockHostController).HideCaption == true))
						ctrl.DragDockInfo.rcDockArea.Size = (ctrl as Control).Parent.Size;
					else
						ctrl.DragDockInfo.rcDockArea.Size = (ctrl as Control).Bounds.Size;
				}
				else	// When dragged out of the main frame
				{
					Size prevSize = Size.Empty;
					
					if( ctrl is DockHost )
						prevSize = ((DockHostController)ctrl.InternalController).DIPrevious.rcDockArea.Size;
						
					if( prevSize != Size.Empty && !(ctrl.InternalController.ParentController is DockTabController))
						ctrl.DragDockInfo.rcDockArea.Size = prevSize;
					else
						ctrl.DragDockInfo.rcDockArea.Size = ctrl.DragRectangle.Size;
				}

				if(ctrl.DragDockInfo.dController != null)
				{
					ctrl.DragDockInfo.dController = null;
					if( ctrl.DragDockInfo.dStyle != DockingStyle.Tabbed )
						ctrl.DragDockInfo.nDockIndex = -1;
				}
				ctrl.DragDockInfo.rcDockArea.Location = location;
			}

			if(ctrl.InternalController.Floating == true)
			{
				if((ctrl.InternalController is DockHostController) && ((ctrl.InternalController as DockHostController).HideCaption == true))
					ctrl.DragDockInfo.rcControlArea.Size = (ctrl as Control).Parent.Size;
				else
					ctrl.DragDockInfo.rcControlArea.Size = (ctrl as Control).Bounds.Size;
			}
			else	// When dragged out of the main frame
				ctrl.DragDockInfo.rcControlArea.Size = ctrl.DragRectangle.Size;

			ctrl.DragDockInfo.rcControlArea.Location = location;
		}

		public virtual void ForceStopDrag()
		{
			IDraggable draggingControl = this.DraggingControl;

			if( draggingControl != null )
			{
				TerminateDrag( draggingControl, Cursor.Position );

				if( ( this.DockingManager.DisallowFloating || !CanFloat( draggingControl ) )
					&& draggingControl.InternalController.Floating )
					TransitToPrevDock( draggingControl );
			}
		}

		protected bool CanFloat( IDraggable draggingControl )
		{
			bool allowFloating = true;

			if( draggingControl != null )
				allowFloating = CanFloat( draggingControl.InternalController );

			return allowFloating;
		}

		protected bool CanFloat( DockControllerBase controller )
		{
			bool allowFloating = true;

			if( controller != null )
			{
				if( !this.SingleTabOperate && controller.ParentController is DockTabController )
					allowFloating = controller.ParentController.AllowFloating;
				else
					allowFloating = controller.AllowFloating;
			}

			return allowFloating;
		}

		protected virtual void TransitToPrevDock( IDraggable ctrl )
		{}

		/// <summary>
		/// Overridable method.
		/// </summary>
		public virtual void ProcessMouseDown(DockControllerBase controller, IDraggable ctrl, Point ptscreen )
		{
		}

		/// <summary>
		/// Overridable method.
		/// </summary>
		public virtual void ProcessMouseUp( DockControllerBase controller, IDraggable ctrl, Point ptscreen )
		{
		}
		/// <summary>
		/// Overridable method.
		/// </summary>
		public virtual void ProcessMouseMove( DockControllerBase controller, IDraggable ctrl, Point ptscreen )
		{
		}
		/// <summary>
		/// Overridable method.
		/// </summary>
		public virtual void ProcessDoubleClick()
		{
		}
		/// <summary>
		/// Overridable method.
		/// </summary>
		public virtual void TerminateDrag( IDraggable ctrl, Point ptscreen )
		{
		}
		/// <summary>
		/// Overridable method.
		/// </summary>
		protected virtual void InitiateDrag( IDraggable ctrl, Point ptscreen )
		{
		}
		/// <summary>
		/// Overridable method. Returns the control being dragged currently.
		/// </summary>
		public virtual Control GetUnderlyingControl( Point point )
		{
			IntPtr handle = NativeMethods.WindowFromPoint(point.X, point.Y);
			return Control.FromHandle( handle );
		}

		/// <summary>
		/// Correct docked window location, after it has been dragged outside of
		/// working area, so it becomes visible.
		/// </summary>
		/// <param name="ctrl"></param>
		protected void CorrectDockedLocation( IDraggable ctrl )
		{
			if( ctrl == null )
				throw new ArgumentNullException( "ctrl" );

			Screen screen = Screen.FromRectangle( ctrl.DragDockInfo.rcDockArea );
			if( ctrl.DragDockInfo.rcDockArea.IntersectsWith( screen.WorkingArea ) ) return;
			
			Rectangle bounds = ctrl.DragDockInfo.rcDockArea;

			// correct x-coordinates
			if( bounds.Left > screen.WorkingArea.Right )
			{
				bounds.X = screen.WorkingArea.Right - bounds.Width;
			}
			if( bounds.Right < screen.WorkingArea.Left )
			{
				bounds.X = screen.WorkingArea.Left;
			}

			// correct y-coordinates
			if( bounds.Y < screen.WorkingArea.Top )
			{
				bounds.Y = screen.WorkingArea.Top;
			}
			if( bounds.Y > screen.WorkingArea.Bottom )
			{
				bounds.Y = screen.WorkingArea.Bottom - bounds.Height;
			}

			//Workaround to fix the broken issue #173 -  works with DragProviderStyle Standard style also.
			if (this.DockingManager.DragProviderStyle == Syncfusion.Windows.Forms.Tools.DragProviderStyle.Standard)
				ctrl.DragDockInfo.rcDockArea = bounds;
			else
			{
				DockHost dockHost = ctrl as DockHost;
				if (dockHost != null)
				{
					FloatingForm floatingForm = dockHost.Parent as FloatingForm;
					if (floatingForm != null)
					{
						floatingForm.Location = bounds.Location;

					}
				}
			}
		}
	}

	internal class BorderDragProvider: CustomDragProvider
	{
		public BorderDragProvider( DockingManager dockingManager ): 
			base( dockingManager )
		{
			//HalftoneBorderPainter.SetDockManager(dockingManager);
		}

		public override void ProcessMouseDown(DockControllerBase controller, 
			IDraggable ctrl, Point ptscreen)
		{
			base.ProcessMouseDown (controller, ctrl, ptscreen);

			this.bInitiateDrag = false;
			this.ptMoveStart = Point.Empty;
			
			if(ctrl.InitiateDrag(MouseAction.LBtnDown, ptscreen) == true)
			{
				this.bInitiateDrag = true;
				this.ptMoveStart = ptscreen;
				// If the control is a splitter, then draw a drag feedback rect
				if(ctrl.DrawHollow() == false)
					InitiateDrag(ctrl, ptscreen);
			}
		}
		public override Control GetUnderlyingControl(Point point)
		{
			Control underlaying = null;
			IntPtr ptr = IntPtr.Zero;

			if( DraggingControl != null )
			{
				FloatingForm floatController = null;				
				DockHostController hostController = DraggingControl.InternalController as DockHostController;

				if( hostController != null )
				{
					floatController = hostController.ParentController.HostControl as FloatingForm;
				}
				ptr = DockingManager.GetFloatingWindow( floatController, point );
				underlaying = Control.FromHandle( ptr );
			}
			if (underlaying == null)
			{
				underlaying = DockingManager.HostControl;
				DockingManager manager = DockingManager.GetManagerFromPoint(point);

				if (manager != null
					&& manager != this.DockingManager)
				{
					ptr = NativeMethods.WindowFromPoint(point.X, point.Y);
					underlaying = Control.FromHandle(ptr);
					DockHostController hitControl = DockingManager.GetDockHostController(underlaying);
					
					if (hitControl != null && manager != null)
					{						
						if (hitControl.ToplevelController == DraggingControl.InternalController.ToplevelController)
						{	
							underlaying = manager.HostControl;
						}
					}
				}
			}
           
			return underlaying;	
		}


		public override void ProcessMouseMove(DockControllerBase controller, IDraggable ctrl, Point ptscreen)
		{
			base.ProcessMouseMove ( controller, ctrl, ptscreen);

			if( this.bInitiateDrag == true )
			{
				if(	DraggingControl == null )
				{
					if( ( ptscreen.X > this.ptMoveStart.X || ptscreen.X < this.ptMoveStart.X )
						|| ( ptscreen.Y > this.ptMoveStart.Y || ptscreen.Y < this.ptMoveStart.Y ) )
						InitiateDrag( ctrl, ptscreen );
				}

				else // this.iDrag == valid IDraggable	// Provide move feedback
				{
					DockInfo diprevious = new DockInfo(ctrl.DragDockInfo);
					GetNewDockInfo(ctrl, ptscreen);

					if( ShowDisallowFloatCursor )
					{
						if( !IsDockAllowed(ctrl) && ctrl.DrawHollow() )	// DrawHollow is true for a splitter
						{
							if(Cursor.Current != Cursors.No)
								Cursor.Current = Cursors.No;
						}
						else if(Cursor.Current == Cursors.No)
							Cursor.Current = Cursors.Default;
					}

					// If the drag rect has changed, then erase the previous rect and draw the new one
					if( (diprevious.rcDockArea != ctrl.DragDockInfo.rcDockArea)
						|| ((ctrl.DragDockInfo.rcDockArea.Width<=0 || ctrl.DragDockInfo.rcDockArea.Height<=0) == true)
						|| ( (diprevious.DP == DockPreference.Tabbed) && (ctrl.DragDockInfo.nDockIndex != diprevious.nDockIndex) )
						|| (diprevious.DP != ctrl.DragDockInfo.DP) )
					{
						if(ctrl.DrawHollow() == true)
						{
							// Erase the previous drag rect
							if( (diprevious.DP == DockPreference.Tabbed) && (diprevious.dController != null) )
							{
								ITabFeedback itf = diprevious.dController.HostControl as ITabFeedback;
								Debug.Assert((itf != null), "Error: Invalid Cast.\n");
								// If the control provides tabfeedback, then set rcdockarea to empty. Failing to do this will
								// make the subsequent call to draw the new drag rect, render an inverted drag feedback rect
								// causing a trail to be evident during the next cycle of mousemove - dragfeedback drawing.
								if(itf.ProvideTabFeedback(ctrl, MouseAction.MouseLeave) == true)
									ctrl.DragDockInfo.rcDockArea = Rectangle.Empty;
								else
									Painter.DrawTabFrame(diprevious.rcDockArea, DockingManager.DockTabAlignment);
							}
							else
								Painter.DrawFrame(diprevious.rcDockArea);

							// Draw the new drag rect
							if((ctrl.DragDockInfo.DP == DockPreference.Tabbed) && (ctrl.DragDockInfo.dController != null) )
							{
								// Give the feedback controller, the first chance at providing the tab feedback.
								// If the controller already has a tabcontrol associated with it, invoking ProvideTabFeedback
								// will show the insertion point for the new tab. If no tabcontrol is available, then
								// the function returns a false. It is now upto the feedback provider to render the
								// drag outline.
								ITabFeedback itf = ctrl.DragDockInfo.dController.HostControl as ITabFeedback;
								Debug.Assert((itf != null), "Error: Invalid Cast.\n");
								bool btabexists = itf.ProvideTabFeedback(ctrl, MouseAction.MouseMove);
								if(btabexists == false)
									Painter.DrawTabFrame(ctrl.DragDockInfo.rcDockArea, DockingManager.DockTabAlignment);
							}
							else
							{
								Painter.DrawFrame(ctrl.DragDockInfo.rcDockArea);
							}
						}
						else
						{
							Painter.DrawRectangle(diprevious.rcDockArea);
							Painter.DrawRectangle(ctrl.DragDockInfo.rcDockArea);
						}
					}
				}
			}
		}


		public override void ProcessMouseUp(DockControllerBase controller, IDraggable ctrl, Point ptscreen)
		{
			if( DraggingControl != null)
			{
				TerminateDrag(ctrl, ptscreen);

				if( IsDockAllowed(ctrl) )
					controller.ApplyDockInfo();
			}
			else
			{
				this.bInitiateDrag = false;
				this.ptMoveStart = Point.Empty;
			}
		}

		public override void ProcessDoubleClick()
		{
			if(this.bInitiateDrag == true)
			{
				this.bInitiateDrag = false;
				this.ptMoveStart = Point.Empty;
			}
		}
		protected override Point CalculateFormOffset(IDraggable ctrl, Point ptClient)
		{
			Point ptOffset = Point.Empty;
			int borderHWidth;

			GetYOffset( out borderHWidth, ref ptClient );

			Size prevSize = Size.Empty;
			if( ctrl is DockHost )
				prevSize = ((DockHostController)ctrl.InternalController).DIPrevious.rcDockArea.Size;

			if( prevSize != Size.Empty && !ctrl.InternalController.Floating )
			{
				ptOffset = new Point( (int)(((float)ptClient.X/(float)ctrl.InternalController.HostControl.Width)*prevSize.Width),
					(int)(((float)ptClient.Y/(float)ctrl.InternalController.HostControl.Height)*prevSize.Height) );
			}
			else
			{
				ptOffset = new Point( (int)(((float)ptClient.X/(float)ctrl.InternalController.HostControl.Width)*ctrl.DragRectangle.Width),
					(int)(((float)ptClient.Y/(float)ctrl.InternalController.HostControl.Height)*ctrl.DragRectangle.Height) );
			}

			return ptOffset;
		}

		protected override void InitiateDrag( IDraggable ctrl, Point ptscreen )
		{
			if (AllowDrag == false)
				return;
			// Fire the DragFeedbackStart event
			if(!(ctrl is DragSplitter && ! DockingManager.DragFeedbackEventsOnSplitters)) 
				DockingManager.FireDragFeedbackEvent("DragFeedbackStart");

			Point ptmoveclient = ctrl.InternalController.HostControl.PointToClient(this.ptMoveStart);
			this.ptDelta = CalculateFormOffset( ctrl, ptmoveclient );

			Form parentform = null;
			if(ctrl is DockHost)
				parentform = (ctrl as DockHost).ParentForm;
			else if(ctrl is FloatingForm)
				parentform = ctrl as FloatingForm;
			else // DragSplitter
			{
				ContainerControl cntrctrl = (ctrl as Control).GetContainerControl() as ContainerControl;
				if(cntrctrl is Form)
					parentform = cntrctrl as Form;
				else
					parentform = cntrctrl.ParentForm;
			}
			if(parentform != null)
				parentform.Deactivate += new EventHandler(this.ParentForm_Deactivate);

			Painter.StartPaint();
			( ctrl as Control ).Update();
			GetNewDockInfo(ctrl, ptscreen);

			if(ctrl.DrawHollow() == true)	// DockHosts and FloatingFrames
			{
				if( (ctrl.DragDockInfo.dController != null) && (ctrl.DragDockInfo.DP == DockPreference.Tabbed) )
				{
					ITabFeedback itf = ctrl.DragDockInfo.dController.HostControl as ITabFeedback;
					Debug.Assert((itf != null), "Error: Invalid Cast.\n");
					bool btabexists = itf.ProvideTabFeedback(ctrl, MouseAction.LBtnUp);
					if(btabexists == false)
						Painter.DrawTabFrame(ctrl.DragDockInfo.rcDockArea, this.DockingManager.DockTabAlignment);
				}
				else
					Painter.DrawFrame(ctrl.DragDockInfo.rcDockArea);
			}
			else	// For splitters
				Painter.DrawRectangle(ctrl.DragDockInfo.rcDockArea);
			this.DraggingControl = ctrl;
		}

		public override void TerminateDrag(IDraggable ctrl, Point ptscreen)
		{
			base.TerminateDrag (ctrl, ptscreen);

			if(this.bInitiateDrag == true)
			{
				this.bInitiateDrag = false;
				this.ptMoveStart = Point.Empty;
			}

			Form parentform = null;
			if(ctrl is DockHost)
				parentform = (ctrl as DockHost).ParentForm;
			else if(ctrl is FloatingForm)
				parentform = ctrl as FloatingForm;
			else // DragSplitter
			{
				ContainerControl cntrctrl = (ctrl as Control).GetContainerControl() as ContainerControl;
				if(cntrctrl is Form)
					parentform = cntrctrl as Form;
				else
					parentform = cntrctrl.ParentForm;
			}
			if(parentform != null)
				parentform.Deactivate -= new EventHandler(this.ParentForm_Deactivate);

			// Erase the current drag rect and do the transition
			if( this.DraggingControl != null )
			{
				if( ctrl.DrawHollow() == true )
				{
					if( ( ctrl.DragDockInfo.dController != null ) && ( ctrl.DragDockInfo.DP == DockPreference.Tabbed ) )
					{
						ITabFeedback itf = ctrl.DragDockInfo.dController.HostControl as ITabFeedback;
						Debug.Assert( ( itf != null ), "Error: Invalid Cast.\n" );
						bool btabexists = itf.ProvideTabFeedback( ctrl, MouseAction.LBtnUp );
						if( btabexists == false )
							Painter.DrawTabFrame( ctrl.DragDockInfo.rcDockArea, this.DockingManager.DockTabAlignment );
					}
					else
						Painter.DrawFrame( ctrl.DragDockInfo.rcDockArea );
				}
				else
					Painter.DrawRectangle( ctrl.DragDockInfo.rcDockArea );
			}
			DraggingControl = null;
			Painter.EndPaint();

			CorrectDockedLocation( ctrl );
			// Fire the DragFeedbackStop event
			if (!(ctrl is DragSplitter && !DockingManager.DragFeedbackEventsOnSplitters)) 
				DockingManager.FireDragFeedbackEvent("DragFeedbackStop");

		}


		protected void ParentForm_Deactivate(Object sender, EventArgs e)
		{
			(sender as Form).Deactivate -= new System.EventHandler(this.ParentForm_Deactivate);

			if( DraggingControl != null)
				DraggingControl.AbortDrag();
		}
	}
	
	internal class HelperForm : Form
	{
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cParams = base.CreateParams;
				cParams.ExStyle |= NativeMethods.SWP_HIDEWINDOW;

				return cParams;
			}
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == NativeMethods.WM_NCHITTEST)
			{
				m.Result = (IntPtr) NativeMethods.HTTRANSPARENT;
			}
			else
			{
				base.WndProc(ref m);
			}
		}
		
		private Color m_borderColor;
		private int m_borderWidth;

		public Color BorderColor
		{
			get
			{
				return m_borderColor;
			}
			set
			{
				if( m_borderColor != value )
				{
					m_borderColor = value;
				}
			}
		}

		public int BorderWidth
		{
			get
			{
				return m_borderWidth;
			}
			set
			{
				if( m_borderWidth != value )
				{
					m_borderWidth = value;
				}
			}
		}
    
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
		
			if( this.Region != null )
			{
				PaintRegionBorders( g );
			}
			else
			{
				Rectangle rect = new Rectangle( 0,0,this.Width, this.Height	 );
				rect.Inflate(-m_borderWidth,-m_borderWidth);
                using (Brush brush = new SolidBrush(this.BorderColor))
                    g.FillRectangle(brush, rect);
			}
			
			base.OnPaint (e);
		}
		private RectangleF[] m_boundsScans;
		internal RectangleF[] BoundsScans
		{
			get
			{
				return m_boundsScans;
			}
			set			
			{
				if( m_boundsScans != value )
				{
					m_boundsScans = value;
				}
			}
		}
		protected virtual void PaintRegionBorders( Graphics g )
		{
			foreach( RectangleF rect in m_boundsScans )
			{
                using(Brush brush =new SolidBrush(this.BorderColor))
                    g.FillRectangle(brush, Rectangle.Round(rect));
			}			
		}
	}
	
	internal class WhidbeyDragProvider: CustomDragProvider
	{
		protected class TargetForm : IDisposable
		{
			private DockingManager m_DockingManager = null;
			public TargetForm( DockingManager dockingManager )
			{
				this.m_DockingManager = dockingManager;
				InternalForm = new HelperForm();
				InternalForm.StartPosition = FormStartPosition.Manual;
				InternalForm.ShowInTaskbar = false;
				InternalForm.FormBorderStyle = FormBorderStyle.None;
				InternalForm.BackColor = Color.Silver;
				InternalForm.Opacity = 0.4;
			}
			public HelperForm InternalForm = null;

			public Color BackColor
			{
				get
				{
					return InternalForm.BorderColor;
				}
				set
				{
					InternalForm.BorderColor = value;
				}
			}

			public Color BorderColor
			{
				get
				{
					return InternalForm.BackColor;
				}
				set
				{
					InternalForm.BackColor = value;
				}
			}

			public int BorderWidth
			{
				get
				{
					return InternalForm.BorderWidth;
				}
				set
				{
					if( InternalForm.BorderWidth != value )
					{
						InternalForm.BorderWidth = value;
					}
				}
			}

			public double Opacity
			{
				get
				{
					return InternalForm.Opacity;
				}
				set
				{
					InternalForm.Opacity = value;
				}
			}

			public Rectangle Bounds
			{
				get { return InternalForm.Bounds; }
				set { InternalForm.Bounds = value; }
			}

			public Point Location
			{
				get { return InternalForm.Location; }
				set { InternalForm.Location = value; }
			}
			protected virtual void CalculateBoundsScans(DockTabAlignmentStyle alignment, Region region)
			{
				Region rez = new Region();
				Matrix rotationMatrix = new Matrix(1, 0, 0, 1, 1, 1);
				PointF rotationPoint = new PointF(this.Bounds.Width / 2, this.Bounds.Height / 2);
				switch( alignment )
				{ 
					case DockTabAlignmentStyle.Top:
						rotationMatrix.RotateAt(180, rotationPoint);
						break;

					case DockTabAlignmentStyle.Right:
						rotationMatrix.RotateAt(90, rotationPoint);
						break;

					case DockTabAlignmentStyle.Left:
						rotationMatrix.RotateAt(270, rotationPoint);
						break;
				}				

				RectangleF[] bounds = region.GetRegionScans(rotationMatrix);				
				bounds[0].Inflate(-BorderWidth, -BorderWidth);

				if( bounds.Length > 1 )
				{
					bounds[1].Y -= BorderWidth;
					bounds[1].Inflate(-BorderWidth, 0);		
					rez = new Region(bounds[0]);					
					rez.Union(bounds[1]);
					rez.Translate(-1, -1);
					rotationMatrix.Reset();

					switch( alignment )
					{
						case DockTabAlignmentStyle.Top:							
							rotationMatrix.RotateAt(180, rotationPoint);
							break;

						case DockTabAlignmentStyle.Right:							
							rotationMatrix.RotateAt(270, rotationPoint);
							break;

						case DockTabAlignmentStyle.Left:							
							rotationMatrix.RotateAt(90, rotationPoint);
							break;
					}				
					
				}								
				InternalForm.BoundsScans = rez.GetRegionScans(rotationMatrix);
			}

			public void ShowRegion( DockTabAlignmentStyle alignment)
			{
				Rectangle rectangle = new Rectangle(this.Bounds.Location, this.Bounds.Size);
				int ntabwt = 0;
				int nTabHeight = m_DockingManager.DockTabHeight;
				int nTabLeft = 0;
				if (alignment == DockTabAlignmentStyle.Top ||
					alignment == DockTabAlignmentStyle.Bottom)
				{
					ntabwt = (rectangle.Width > 100) ? 45 : rectangle.Width / 2;
					nTabLeft = m_DockingManager.IsMirrored ?
						rectangle.Right - ntabwt - 8 :
						rectangle.Left + 8;
				}
				else
				{
					ntabwt = ( rectangle.Height > 100 ) ? 45 : rectangle.Height / 2;
					nTabLeft = m_DockingManager.IsMirrored ?
						rectangle.Bottom - ntabwt - 8 :
						rectangle.Top + 8;
				}
				Rectangle rctab = Rectangle.Empty;
				switch( alignment )
				{ 
					case DockTabAlignmentStyle.Bottom:
						rctab = new Rectangle(nTabLeft, rectangle.Bottom - nTabHeight - 4, ntabwt, nTabHeight + 4);
						rectangle.Size = new Size(rectangle.Width, rectangle.Height - nTabHeight);
						break;

					case DockTabAlignmentStyle.Top:
						rctab = new Rectangle(nTabLeft, rectangle.Y, ntabwt, nTabHeight + 4);
						rectangle.Size = new Size(rectangle.Width, rectangle.Height - nTabHeight);
						rectangle.Location = new Point(rectangle.Location.X, rectangle.Location.Y + nTabHeight);
						break;

					case DockTabAlignmentStyle.Left:
						rctab = new Rectangle(rectangle.X,nTabLeft, nTabHeight + 4, ntabwt);
						rectangle.Size = new Size(rectangle.Width - nTabHeight, rectangle.Height);
						rectangle.Location = new Point(rectangle.Location.X + nTabHeight, rectangle.Location.Y);						
						break;

					case DockTabAlignmentStyle.Right:						
						rectangle.Size = new Size(rectangle.Width - nTabHeight, rectangle.Height);
						rctab = new Rectangle(rectangle.X+rectangle.Width, nTabLeft, nTabHeight, ntabwt);						
						break;
				}				
				
				Region rgn = new Region(InternalForm.RectangleToClient(rectangle));
				rgn.Union(InternalForm.RectangleToClient(rctab));
				CalculateBoundsScans(alignment, rgn);
				InternalForm.Region = rgn;				
				InternalForm.Show();
			}

			public void ShowRectangle( Rectangle rectangle )
			{
				InternalForm.Region = null;
				InternalForm.Show();
				InternalForm.Bounds = rectangle;
			}

			public void Hide()
			{
				InternalForm.Hide();
			}

			public bool Visible
			{
				get { return InternalForm.Visible; }
			}

			#region IDisposable Members

			public void Dispose()
			{
				this.m_DockingManager = null;
				InternalForm.Dispose();
				InternalForm = null;
			}

			#endregion
		}

		protected DragControl dragControl = null;
		protected TargetForm targetForm = null;
		private DockControllerBase m_Controller = null;
		private bool processedDragControl = false;
		private bool tabbedMDI = false;
		private bool subscribedToMoveEvents = false;
		private DockControllerBase m_DragController = null;
		private DockControllerBase dcbUnderlaying = null;
        private bool bProvideTabFeedback = false;
		protected bool bCtrlKeyPressed = false;

		public override void ForceStopDrag()
		{
			base.ForceStopDrag();
			if (dragControl.Visible)
			{
				dragControl.HideControl();
			}

			if(targetForm.Visible)
			{
				targetForm.Hide();
			}
		}

		protected virtual FocusHolder FocusHolder
		{ 
			get
			{
				return this.DockingManager.dcHostForm.FocusHolderControl;
			}
		}

		protected virtual void CreateTargetForm( DockingManager dockingManager )
		{
			targetForm = new TargetForm( dockingManager);
		}
		
		public WhidbeyDragProvider( DockingManager dockingManager ): 
			base( dockingManager )
		{
			InitializeProvider();
			this.m_DockingManager = dockingManager;
		}

		public override void Dispose()
		{
			base.Dispose();
			dragControl.OnMouseEnter -= new MouseEnterHandler(dragControl_OnMouseEnter);
			dragControl.OnMouseLeave -= new MouseLeaveHandler(dragControl_OnMouseLeave);
			dragControl.OnMouseUp -= new MouseUpHandler(dragControl_OnMouseUp);
			dragControl.OnAllowHighlight -= new AllowHighlightHandler(dragControl_OnAllowHighlight);
			this.dragControl.Dispose();
			targetForm.Dispose();
			targetForm = null;
			this.m_mainFormController = null;
			this.m_Controller = null;
			this.m_prevUnderlaying = null;
			this.underlaying = null;
			this.dcbUnderlaying = null;
			this.dragControl = null;
		}

		protected virtual void InitializeProvider()
		{
			this.dragControl = new DragControl();
	
			dragControl.OnMouseEnter += new MouseEnterHandler(dragControl_OnMouseEnter);
			dragControl.OnMouseLeave += new MouseLeaveHandler(dragControl_OnMouseLeave);
			dragControl.OnMouseUp += new MouseUpHandler(dragControl_OnMouseUp);
			dragControl.OnAllowHighlight +=new AllowHighlightHandler(dragControl_OnAllowHighlight);
	
			CreateTargetForm( DockingManager );
		}

		public override void ProcessCtrlKeyDown()
		{
			base.ProcessCtrlKeyDown();
			if( !bCtrlKeyPressed )
			{
				bCtrlKeyPressed = true;
				dragControl.HideControl();
				if( targetForm.Visible )
				{
					targetForm.Hide();
				}
			}
		}

		public override void ProcessCtrlKeyUp()
		{
			base.ProcessCtrlKeyUp();
			bCtrlKeyPressed = false;
		}

		public override void ProcessMouseDown(DockControllerBase controller, 
			IDraggable ctrl, Point ptscreen)
		{
			base.ProcessMouseDown (controller, ctrl, ptscreen);

			if( !subscribedToMoveEvents )
			{
				m_DockingManager.HostControl.Move +=new EventHandler(HostControl_Move);
				m_DockingManager.HostControl.Resize +=new EventHandler(HostControl_Resize);
				subscribedToMoveEvents = true;
			}

			processedDragControl = false;
			tabbedMDI = false;
			this.bInitiateDrag = false;
			this.ptMoveStart = Point.Empty;
			if(ctrl.InitiateDrag(MouseAction.LBtnDown, ptscreen) == true)
			{
				this.bInitiateDrag = true;
				this.ptMoveStart = ptscreen;
				// If the control is a splitter, then draw a drag feedback rect
				m_DragController = controller;
				if(ctrl.DrawHollow() == false)
					InitiateDrag(ctrl, ptscreen);
			}
		}

		protected void UpdateDockAbility( DockControllerBase controller )
		{
			if (dcbUnderlaying != m_mainFormController && controller.HostControl.Controls.Count > 0)
			{
				dragControl.DockAbility = DockingManager.GetDockAbility( controller.HostControl.Controls[0] );
			}
			else
			{
				dragControl.DockAbility = dragControl.OuterDockAbility;
			}
		}

		protected void UpdateOuterDockAbility( DockControllerBase controller )
		{
			if( controller is DockHostController )
			{
				dragControl.OuterDockAbility = DockingManager.GetOuterDockAbility( controller.HostControl.Controls[0] );
			}
			else
			{
				dragControl.OuterDockAbility = DockAbility.All;
			}
		}

		protected void UpdateDragControlVisibility( Point ptscreen, IDraggable ctrl )
		{
			bool showOuter = false;
			bool showInner = false;

			if( underlaying != null )
			{
				showInner = UpdateInnerControlVisibility( underlaying );
				showOuter = true;
				DockControllerBase controller = ctrl.InternalController;
				DockHostController target = GetUnderlayingController(underlaying);
				// Target is float only control.
				if( target != null && target.FloatOnly )
				{
					MainFormController mfc = GetMainFormController( ptscreen );
					Rectangle bounds = mfc.HostControl.DisplayRectangle;
					bounds = mfc.HostControl.RectangleToScreen( bounds );
					if( !bounds.Contains( ptscreen ) )
					{
						showInner = false;
						showOuter = false;
					}
				}
				// No target dock control.
				if( underlaying == null || m_noControl )
				{
					showInner = false;

					if( !(underlaying is DragSplitter) )
						showOuter = false;
				}

				// Fix: Exact VS 2005 Tabbed Docking behavior
				// If dock target is DockHost caption
				if( ( ctrl.DragDockInfo.DP == DockPreference.Tabbed ) && ( ctrl.DragDockInfo.dController != null )
					&& ( ( dragControl.DockAbility | DockAbility.Tabbed ) == dragControl.DockAbility ) )
				{
					showInner = false;
					showOuter = false;
				}
				else
				{
					if( ( !dragControl.Visible || !dragControl.InnerVisible && DockingManager.GetDockAbility( underlaying ) != DockAbility.None )
						&& !( DraggingControl is DragSplitter ) )
					{
						// Dragging control is not float only control.
						if( DockingManager.GetFloatOnly( controller.HostControl.Controls[0] ) == false )
						{
							UpdateDockAbility( controller );
							if( !bCtrlKeyPressed )
							{
								showOuter &= true;
								if( ShowOuterControlOnly( ptscreen ) )
									showInner = false;
							}
						}
						else
						{
							showInner = false;
							showOuter = false;
						}
					}
				}

				if( bCtrlKeyPressed || DraggingControl is DragSplitter )
                {
                    bCtrlKeyPressed = NativeMethods.GetKeyState( 0x11 ) < 0;  //if Ctrl pressed
                    
                    showInner = false;
					showOuter = false;
				}
			}
			else
			{
				MainFormController mfc = GetMainFormController( ptscreen );
				Rectangle bounds = mfc.HostControl.DisplayRectangle;
				bounds = mfc.HostControl.RectangleToScreen( bounds );
				DockHost dh = ctrl as DockHost;
                if (!((ctrl.DragDockInfo.DP == DockPreference.Tabbed) && (ctrl.DragDockInfo.dController != null)
                                            && ((dragControl.DockAbility | DockAbility.Tabbed) == dragControl.DockAbility)))
				if( ctrl.InternalController.HostControl.Controls.Count > 0 && dh != null )
					if( ( dh.Parent is FloatingForm ) && bounds.Contains( ptscreen )
						&& !DockingManager.GetFloatOnly( ctrl.InternalController.HostControl.Controls[0] ) )
					{
						showInner = true;
						showOuter = true;
					}
			}

			if( showInner )
			{
				if( !dragControl.InnerVisible )
					dragControl.ShowInnerControl();
			}
			else
			{
                if (dragControl.InnerVisible)
                {
                    bool flag = false;
                    Form form = null;
                    FormBorderStyle borderStyle = FormBorderStyle.SizableToolWindow;
                    if (CanChangeFormBorderStyle(ctrl))
                    {
                        //SD2 960
                        //When the main form's border style is FixedToolWindow, the floating form loses mouse capture while it is being dragged. 
                        //Following code ensures that FloatingForm does not loses mouse capture and will be dragged seemlessly

                        form = (ctrl.InternalController.ToplevelController as FloatingFormController).HostControl as Form;
                        if (form != null)
                        {
                            borderStyle = form.FormBorderStyle;
                            form.FormBorderStyle = FormBorderStyle.Sizable;
                            flag = true;
                        }  
                    }

                    dragControl.HideInnerControl();

                    if (flag)
                    {
                        form.FormBorderStyle = borderStyle;
                        form = null;
                    }
                }
			}

			if( showOuter )
			{
				if( !dragControl.OuterVisible )
					dragControl.ShowOuterControl();
			}
			else
			{
                if (dragControl.OuterVisible)
                {
                    bool flag = false;
                    Form form = null;
                    FormBorderStyle borderStyle = FormBorderStyle.SizableToolWindow;
                    if (CanChangeFormBorderStyle(ctrl))
                    {
                        //SD2 960
                        //When the main form's border style is FixedToolWindow, the floating form loses mouse capture while it is being dragged. 
                        //Following code ensures that FloatingForm does not loses mouse capture and will be dragged seemlessly

                        form = (ctrl.InternalController.ToplevelController as FloatingFormController).HostControl as Form;
                        if (form != null)
                        {
                            borderStyle = form.FormBorderStyle;
                            form.FormBorderStyle = FormBorderStyle.Sizable;
                            flag = true;
                        }
                    }

                    dragControl.HideOuterControl();

                    if (flag)
                    {
                        form.FormBorderStyle = borderStyle;
                        form = null;
                    }
                }
			}
		}

        bool CanChangeFormBorderStyle(IDraggable ctrl)
        {
            try
            {
                return (ctrl.InternalController != null && ctrl.InternalController.DockingManager.HostControl is Form
                        && ((ctrl.InternalController.DockingManager.HostControl as Form).FormBorderStyle == FormBorderStyle.FixedToolWindow
                        || (ctrl.InternalController.DockingManager.HostControl as Form).FormBorderStyle == FormBorderStyle.SizableToolWindow) && ctrl.InternalController.Floating
                            && ctrl.InternalController.ToplevelController is FloatingFormController);
            }
            catch
            {

            }
            return false;
        }

		protected bool UpdateInnerControlVisibility( Control underlaying )
		{
			bool show = false;
			if( underlaying != null )
			{
				Point ptscreen = Cursor.Position;
				show = true;
				DockHostController target = GetUnderlayingController(underlaying);

				if( target != null && target.FloatOnly )
				{
					MainFormController mfc = GetMainFormController( ptscreen );
					Rectangle bounds = mfc.HostControl.ClientRectangle;
					bounds = mfc.HostControl.RectangleToScreen( bounds );
					if( !bounds.Contains( ptscreen ) )
						show = false;
				}

				if( DockingManager.GetDockAbility( underlaying ) != DockAbility.None && show )
				{
					bool canShow = true;
					DockHostController dhc = DraggingControl.InternalController as DockHostController;

					if( dhc != null && dhc.FloatOnly )
						canShow = false;
					if( !( underlaying is DragSplitter ) && !( DraggingControl is DragSplitter ) && canShow && DockingManager.GetEnableDocking( underlaying ) )
					{
						if( dragControl.InnerVisible == false && bProvideTabFeedback == false && bCtrlKeyPressed == false )
						{
							show = true;
						}
					}
				}
				else
					show = false;
			}

			return show;
		}

        protected bool ShowOuterControlOnly( Point ptscreen )
        {
			FloatingForm ff = ( DraggingControl as Control ).TopLevelControl as FloatingForm;
			IntPtr handle = IntPtr.Zero;
			if( ff != null )
			{
				ff.PassbyHitTest = true;
				handle = NativeMethods.WindowFromPoint( new NativeMethods.POINT( ptscreen.X, ptscreen.Y ) );
				ff.PassbyHitTest = false;
			}

			Control ctrl = Control.FromHandle(handle);
			if( ctrl is DragSplitter )
				return true;

            return false;
        }

		public override void ProcessMouseMove(DockControllerBase controller, IDraggable ctrl, Point ptscreen)
		{
			base.ProcessMouseMove ( controller, ctrl, ptscreen);
			
			if( this.bInitiateDrag == true )
			{
				if(	DraggingControl == null )
				{
					if( ( ptscreen.X > this.ptMoveStart.X || ptscreen.X < this.ptMoveStart.X )
						|| ( ptscreen.Y > this.ptMoveStart.Y || ptscreen.Y < this.ptMoveStart.Y )
						|| controller is FloatingFormController )
						InitiateDrag( ctrl, ptscreen );
				}

				else // this.iDrag == valid IDraggable	// Provide move feedback
				{
					DockInfo diprevious = new DockInfo(ctrl.DragDockInfo);
					GetNewDockInfo(ctrl, ptscreen);
					m_mainFormController = GetMainFormController( ptscreen );
					dcbUnderlaying = GetUnderlayingController( m_mainFormController, ptscreen );
					if( m_prevUnderlaying != dcbUnderlaying )
					{
						m_prevUnderlaying = dcbUnderlaying;
						UpdateDockAbility( controller );
					}

					if( controller.Floating )
					{
						bool redockFloatControl = true;
						FloatingFormController floatParent = controller.ToplevelController as FloatingFormController;
						if( (controller.ParentController == floatParent &&
							!(controller is DockTabController)) || controller is DragSplitterController )
							redockFloatControl = false;
						else
						{
							SizingController sc = floatParent.dcChild as SizingController;
							if( sc != null && sc.GetDockControllers().Count == 1 && !(controller is DockTabController) )
							{
								redockFloatControl = false;
							}
						}
						if( redockFloatControl )
						{
							DockTabController dtc = controller as DockTabController;
							if( dtc != null && dtc.dragTabPage != null )
							{
								if( dtc.dragTabPage.dhcClient.DINew.dController == null )
								{
									controller.ApplyDockInfo();
									this.InitiateFormDragging( controller, ctrl, ptscreen );
									if( !this.bInitiateDrag && DraggingControl == null )
										return;
								}
							}
							else
							{
								SizingController sc = controller.ParentController as SizingController;
								if( sc != null && ctrl.DragDockInfo.dController == null )
								{
									controller.ApplyDockInfo();
									this.InitiateFormDragging( controller, ctrl, ptscreen );
									if( !this.bInitiateDrag && DraggingControl == null )
										return;
								}
							}

							dtc = controller.ParentController as DockTabController;
							if( dtc != null && ctrl.DragDockInfo.dController == null &&
								controller.ParentController.ParentController is SizingController )
							{
								controller.ApplyDockInfo();
								this.InitiateFormDragging( controller, ctrl, ptscreen );
								if( !this.bInitiateDrag && DraggingControl == null )
									return;
							}
						}
					}
					else
					{
						if( ctrl.DragDockInfo.dController == null )
						{
							Point ptclient = controller.HostControl.PointToClient(ptscreen);
							controller.ApplyDockInfo();
							this.InitiateFormDragging( controller, ctrl, ptscreen );
							if( !this.bInitiateDrag && DraggingControl == null )
								return;
							this.SetDragCursor();
						}
					}

					if( !( ctrl is DragSplitter ) )
						UpdateDragControlPosition( ptscreen );

					UpdateOuterDockAbility( controller );
					UpdateDragControlVisibility( ptscreen, ctrl );

					// If the drag rect has changed, then erase the previous rect and draw the new one
					if( (diprevious.rcDockArea != ctrl.DragDockInfo.rcDockArea)
						|| ((ctrl.DragDockInfo.rcDockArea.Width<0 || ctrl.DragDockInfo.rcDockArea.Height<0) == true)
						|| (diprevious.DP != ctrl.DragDockInfo.DP) )
					{
						if(ctrl.DrawHollow() == true)
						{
							// Erase the previous drag rect
							if( (diprevious.DP == DockPreference.Tabbed) && (diprevious.dController != null) )
							{
								ITabFeedback itf = diprevious.dController.HostControl as ITabFeedback;
								if( itf != null )
								{
									// If the control provides tabfeedback, then set rcdockarea to empty. Failing to do this will
									// make the subsequent call to draw the new drag rect, render an inverted drag feedback rect
									// causing a trail to be evident during the next cycle of mousemove - dragfeedback drawing.
									if( itf.ProvideTabFeedback( ctrl, MouseAction.MouseLeave ) == true )
										ctrl.DragDockInfo.rcDockArea = Rectangle.Empty;
									targetForm.Hide();
									processedDragControl = false;
									this.bProvideTabFeedback = false;
								}
							}

							// Draw the new drag rect
							if((ctrl.DragDockInfo.DP == DockPreference.Tabbed) && (ctrl.DragDockInfo.dController != null)
								&& ((dragControl.DockAbility | DockAbility.Tabbed) == dragControl.DockAbility) )
							{
                                // Implement exact VS2005 behavior - hide drag control.
                                dragControl.HideControl();
                                this.bProvideTabFeedback = true;

								// Give the feedback controller, the first chance at providing the tab feedback.
								// If the controller already has a tabcontrol associated with it, invoking ProvideTabFeedback
								// will show the insertion point for the new tab. If no tabcontrol is available, then
								// the function returns a false. It is now upto the feedback provider to render the
								// drag outline.
								ITabFeedback itf = ctrl.DragDockInfo.dController.HostControl as ITabFeedback;
								Debug.Assert((itf != null), "Error: Invalid Cast.\n");
								itf.ProvideTabFeedback(ctrl, MouseAction.MouseMove);
								targetForm.Bounds = ctrl.DragDockInfo.rcDockArea;
								
								targetForm.ShowRegion(DockingManager.DockTabAlignment);
								processedDragControl = true;
								SetDragCursor();
							}
						}
						else
						{
							if( controller is DragSplitterController)
							{
                                if (DockingManager.VisualStyle == VisualStyle.Metro)
                                {
                                    Painter.DrawRectangle(diprevious.rcDockArea, DockingManager.VisualStyle);
                                    Painter.DrawRectangle(ctrl.DragDockInfo.rcDockArea, DockingManager.VisualStyle);
                                }
                                else
                                {
                                    Painter.DrawRectangle(diprevious.rcDockArea);
                                    Painter.DrawRectangle(ctrl.DragDockInfo.rcDockArea);
                                }
							}
						}
					}
					if( !processedDragControl && ctrl.DragDockInfo.dController != null )
					{
						ctrl.DragDockInfo.dController = null;
						SetDragCursor();
					}
					bool ctrlFloatOnly = false;
					DockHost dhDrag = ctrl as DockHost;
					if( dhDrag != null && dhDrag.Controls[0] != null )
					{
						ctrlFloatOnly = DockingManager.GetFloatOnly( dhDrag.Controls[0] );
					}
					if( !(controller is DragSplitterController) )
					{
						if( !ctrlFloatOnly )
							dragControl.ProcessMouseMove(ptscreen);
						else
							dragControl.ProcessMouseMove(Point.Empty);
					}
				}
			}
		}

		protected void InitiateFormDragging( DockControllerBase controller, IDraggable ctrl, Point ptscreen )
		{
			if( !( controller is DragSplitterController ) )
			{
				DockTabController tabController = controller as DockTabController;

				if( tabController == null || tabController.TabControl == null )
					( ctrl as Control ).Capture = false;
				else
					tabController.TabControl.Capture = false;
			}

			FloatingFormController ffc = ctrl.InternalController.ToplevelController as FloatingFormController;
			if( ffc != null )
			{
				FloatingForm form = ffc.HostControl as FloatingForm;
				this.DockingManager.InitiateFormDrag( form, ptscreen, ptDelta );
			}
		}

		public override void ProcessMouseUp(DockControllerBase controller, IDraggable ctrl, Point ptscreen)
		{
			m_DragController = null;
			if( DraggingControl != null)
			{
				dragControl.ProcessMouseUp(ptscreen);
				TerminateDrag(ctrl, ptscreen);

				bool tabbedDocking = false;
				DockHost host = ctrl as DockHost;
				if( host != null )
				{
					DockHostController hostController = host.InternalController as DockHostController;
					if( hostController != null )
					{
						tabbedDocking = hostController.DINew.DP == DockPreference.Tabbed;
					}
				}

				if( !(ctrl is DragSplitter) )
				{
					bool allowDock = true;
					DockHostController hostController = controller as DockHostController;
					if( hostController != null )
					{
						DockControllerBase baseController = hostController.DINew.dController;
						Control targetControl = null;
						if( baseController != null )
						{
							if( baseController is DockHostController )
							{
								targetControl = baseController.HostControl.Controls[0];
							}
							else
							{
								//Added condition for DockTabController for returns the targetcontrol as null value when we docked as tabbed group
								if (baseController is MainFormController || baseController is DockTabController)
								{
									targetControl = baseController.HostControl;
								}
                                
							}

							Control draggedControl = null;
							if( host != null )
							{
								draggedControl = host.Controls[0];
							}
							else
							{
								FloatingForm floatingForm = ctrl as FloatingForm;
								if( floatingForm != null )
								{
									DockControllerBase floatingController = floatingForm.InternalController;
									draggedControl = floatingController.HostControl.Controls[0];
								}
							}
							
							DockingStyle dStyle = hostController.DINew.dStyle;
#if SyncfusionFramework2_0
							if ( DockingManager.HostForm != null &&
								DockingManager.HostForm.RightToLeftLayout &&
								DockingManager.HostForm.RightToLeft == RightToLeft.Yes)
							{
								if (dStyle == DockingStyle.Left)
									dStyle = DockingStyle.Right;
								else if (dStyle == DockingStyle.Right)
									dStyle = DockingStyle.Left;
							}
#endif
							DockAllowEventArgs args = new DockAllowEventArgs( draggedControl, 
								targetControl, dStyle );

							this.DockingManager.FireDockAllowEvent( args );
							allowDock = !args.Cancel;
							if( !IsDockAllowed(ctrl) )
								allowDock = false;
						}
					}

					if( allowDock )
					{
						if( !ctrl.InternalController.IsFloatOnly() )
						{
							if( processedDragControl )
							{
								if( tabbedMDI )
								{
									FloatingFormController ffc = controller as FloatingFormController;
									if( ffc != null )
									{
										SizingController sc = ffc.dcChild as SizingController;
										if( sc != null )
										{
											ArrayList alDockHosts = new ArrayList();
											sc.GetChildDockHosts( ref alDockHosts );
											foreach( DockHost dh in alDockHosts )
											{
												DockingManager.SetAsMDIChild( dh.Controls[0], true );
											}
										}
									}
									else if( tabbedDocking )
									{
										controller.DockAsMDIChild( ((DockStateControllerBase)controller).DINew );
									}
									else
									{
										controller.ApplyDockInfo();
									}
								}
								else
								{
									controller.ApplyDockInfo();
								}
							}
							else
							{
								if( !IsDockAllowed( ctrl ) && controller.Floating )
								{
									TransitToPrevDock(ctrl);
								}
							}
						}
						else
						{
							DockHostController dockHostController =
								ctrl.InternalController as DockHostController;
							if( dockHostController != null )
							{
								dockHostController.DINew.rcDockArea = 
									dockHostController.DINew.rcControlArea;
							}
						}
					}
					else
					{
						if( !IsDockAllowed(ctrl) && controller.Floating )
							TransitToPrevDock( ctrl );
					}

					dragControl.HideControl();
					targetForm.Hide();
				}
			}
			else
			{
				this.bInitiateDrag = false;
				this.ptMoveStart = Point.Empty;
			}
		}

		protected override void TransitToPrevDock(IDraggable ctrl)
		{
			DockHost host = ctrl as DockHost;
			if (host != null)
			{
				DockHostController hostController = host.InternalController as DockHostController;
				if (hostController != null)
				{
					DockTabController tabController = hostController.ParentController
						as DockTabController;
					if (tabController != null)
					{
						tabController.TransitToPrevDock();
					}
					else
					{
						hostController.TransitToPrevDock();
					}
				}
			}
		}

		public override void TerminateDrag(IDraggable ctrl, Point ptscreen)
		{
			base.TerminateDrag (ctrl, ptscreen);

			if (Cursor.Current != Cursors.Default)
				Cursor.Current = Cursors.Default;

			DraggingControl = null;
			if(this.bInitiateDrag == true)
			{
				this.bInitiateDrag = false;
				this.ptMoveStart = Point.Empty;
			}

			Form parentform = null;
			if(ctrl is DockHost)
				parentform = (ctrl as DockHost).ParentForm;
			else if(ctrl is FloatingForm)
				parentform = ctrl as FloatingForm;
			else // DragSplitter
			{
				ContainerControl cntrctrl = (ctrl as Control).GetContainerControl() as ContainerControl;
				if(cntrctrl is Form)
					parentform = cntrctrl as Form;
				else
					parentform = cntrctrl.ParentForm;
			}
			if(parentform != null)
				parentform.Deactivate -= new EventHandler(this.ParentForm_Deactivate);

			// Erase the current drag rect and do the transition
			if(ctrl.DrawHollow() == true)
			{
				if( (ctrl.DragDockInfo.dController != null) && (ctrl.DragDockInfo.DP == DockPreference.Tabbed) )
				{
					ITabFeedback itf = ctrl.DragDockInfo.dController.HostControl as ITabFeedback;
					//Debug.Assert((itf != null), "Error: Invalid Cast.\n");
					if( itf != null)
					{
						itf.ProvideTabFeedback(ctrl, MouseAction.LBtnUp);
					}
				}
			}
			else
			{
				if (DockingManager.VisualStyle == VisualStyle.Metro)
                {
                    Painter.DrawRectangle(ctrl.DragDockInfo.rcDockArea, DockingManager.VisualStyle);
                }
                else
                {
                    Painter.DrawRectangle(ctrl.DragDockInfo.rcDockArea);
                }
			}

			CorrectDockedLocation( ctrl );
			// Fire the DragFeedbackStop event
			if (!(ctrl is DragSplitter && !DockingManager.DragFeedbackEventsOnSplitters)) 
				DockingManager.FireDragFeedbackEvent("DragFeedbackStop");
			if( FocusHolder.LastActiveControl != null )
			{	
				if(FocusHolder.LastActiveControl.Parent != null)
                    FocusHolder.LastActiveControl.Parent.Focus();
				FocusHolder.LastActiveControl = null;
				FocusHolder.Visible = false;
			}
			underlaying = null;
			this.DockingManager.StopActivationEvents = false;
		}

		protected void ParentForm_Deactivate(Object sender, EventArgs e)
		{
			(sender as Form).Deactivate -= new System.EventHandler(this.ParentForm_Deactivate);
		}

		protected override void InitiateDrag( IDraggable ctrl, Point ptscreen )
		{
			if (AllowDrag == false)
				return;
			// Fire the DragFeedbackStart event
			if (!(ctrl is DragSplitter && !DockingManager.DragFeedbackEventsOnSplitters)) 
				DockingManager.FireDragFeedbackEvent("DragFeedbackStart");

			Point ptmoveclient = ctrl.InternalController.HostControl.PointToClient(this.ptMoveStart);

			this.ptDelta = CalculateFormOffset( ctrl, ptmoveclient );
			this.DockingManager.StopActivationEvents = true;

			Form parentform = null;
			if(ctrl is DockHost)
				parentform = (ctrl as DockHost).ParentForm;
			else if(ctrl is FloatingForm)
				parentform = ctrl as FloatingForm;
			else // DragSplitter
			{
				ContainerControl cntrctrl = (ctrl as Control).GetContainerControl() as ContainerControl;
				if(cntrctrl is Form)
					parentform = cntrctrl as Form;
				else
					parentform = cntrctrl.ParentForm;
			}
			if(parentform != null)
				parentform.Deactivate += new EventHandler(this.ParentForm_Deactivate);

			GetNewDockInfo(ctrl, ptscreen);

			if(ctrl.DrawHollow() == true)	// DockHosts and FloatingFrames
			{
				if( (ctrl.DragDockInfo.dController != null) && (ctrl.DragDockInfo.DP == DockPreference.Tabbed) )
				{
					ITabFeedback itf = ctrl.DragDockInfo.dController.HostControl as ITabFeedback;
					Debug.Assert((itf != null), "Error: Invalid Cast.\n");
					itf.ProvideTabFeedback(ctrl, MouseAction.LBtnUp);
				}
			}
			else
			{	// For splitters
                if (DockingManager.VisualStyle ==VisualStyle.Metro )
                    DockingManager.FramePainter.Painter.DrawRectangle(ctrl.DragDockInfo.rcDockArea, DockingManager.VisualStyle);
                else
                    DockingManager.FramePainter.Painter.DrawRectangle(ctrl.DragDockInfo.rcDockArea);
			}
			this.DraggingControl = ctrl;

			if( !(ctrl is DragSplitter) )
			{
				UpdateDragControlPosition( ptscreen );
				FocusHolder.Visible = true;
				FocusHolder.LastActiveControl = this.DockingManager.ActiveControl;
				FocusHolder.Focus();
			}

			SetDragCursor();
		}

		private void SetDragCursor()
		{
			if( !IsDockAllowed( DraggingControl ) && ( DraggingControl != null 
				? this.DraggingControl.DrawHollow() : true))
			{
				if (Cursor.Current != Cursors.No)
				{
					Cursor.Current = Cursors.No;
				}
			}
			else
			{
				if (Cursor.Current != Cursors.Default)
				{
					Cursor.Current = Cursors.Default;
				}
			}
		}
		public override bool CanFloatWhenDisallowFloating
		{
			get
			{
				return true;
			}
		}
		private Control underlaying;

		public override Control GetUnderlyingControl(Point point)
		{
			IntPtr dragWindowHandle = NativeMethods.WindowFromPoint(new NativeMethods.POINT( point.X, point.Y));
			Control dragControl = Control.FromHandle(dragWindowHandle);
			if( dragControl != null )
			{
				DockHostController hostController = DockingManager.GetDockHostController( dragControl );
				if( (dragControl is FloatingForm)
					|| (dragControl is DockHost)
					|| ( (hostController != null) && (hostController.Floating) ) )
				{
					IntPtr handle = IntPtr.Zero;
					Control hostControl = DockingManager.dcHostForm.HostControl;
					Point ptscreen = hostControl.PointToScreen(point);
					
					if( dragControl is FloatingForm )
					{
						((FloatingForm)dragControl).PassbyHitTest = true;

						if(this.DockingManager.IsDefaultRendering())
							handle = NativeMethods.WindowFromPoint(new NativeMethods.POINT(point.X, point.Y - 20));
						else
							handle = NativeMethods.WindowFromPoint(new NativeMethods.POINT(point.X, point.Y));

						((FloatingForm)dragControl).PassbyHitTest = false;
					}
					else
					{
						handle = DockingManager.GetFloatingWindow(dragControl, point);
					}
					
					if( handle == IntPtr.Zero )
					{
						Point ptclient = hostControl.PointToClient(point);
						handle = NativeMethods.ChildWindowFromPoint( hostControl.Handle, new NativeMethods.POINT( ptclient.X, ptclient.Y));
					}
					Control ctrl = Control.FromHandle(handle);

					// Workaround for defect 2194: 
					// when we can't get a control from current handle
					// search for appropriate parent control in the hierarchy
					while( ctrl == null && handle != IntPtr.Zero )
					{
						handle = NativeMethods.GetParent( handle );
						ctrl = Control.FromHandle( handle );
					}

					if( ctrl != DraggingControl )
						underlaying = ctrl is FloatingForm ? null : ctrl;

					return ctrl;
				}
			}

			return base.GetUnderlyingControl(point);
		}
		private MainFormController m_mainFormController = null;		

		private void UpdateDragControlPosition( Point ptscreen )
		{
			try
			{
				if( m_mainFormController == null )
				{
					m_mainFormController = GetMainFormController( ptscreen );
				}

				Control control = m_mainFormController.HostControl;

				if( control != null )
				{
					Point pt;

#if SyncfusionFramework2_0
					if( DockingManager.HostForm != null &&
						DockingManager.HostForm.RightToLeft == RightToLeft.Yes &&
						DockingManager.HostForm.RightToLeftLayout )
					{
						pt = control.PointToScreen( new Point( m_mainFormController.LayoutRect.Width, 0 ) );
					}
					else
#endif
						pt = control.PointToScreen( new Point( 0, 0 ) );

					Rectangle outerRect = m_mainFormController.LayoutRect;
					outerRect.Offset( pt );
					dragControl.OuterControllerRect = outerRect;
				}
				DockControllerBase controllerBase = DockingManager.GetDockController(ptscreen);
				if( ( controllerBase != null ) && !( controllerBase is DragSplitterController ) )
				{
					DockHostController hostController = controllerBase as DockHostController;
					if( ( hostController != null ) && ( hostController.AutoHideMode ) )
					{
						m_Controller = m_DockingManager.dcHostForm;
					}
					else
					{
						m_Controller = controllerBase;
					}

					if( this.DraggingControl == null || m_Controller == this.DraggingControl.InternalController
						|| m_Controller.ParentController == this.DraggingControl.InternalController )
						return;

					// When underlaying control is FloatingForm in FloatOnly mode then get first control in the 
					// collection of docked controls laying under this FloatingForm.
					if( m_Controller != null && DockingManager.GetFloatOnly( m_Controller.HostControl.Controls[0] ) )
					{
						m_Controller = ( dcbUnderlaying == null )? m_mainFormController : dcbUnderlaying;
					}

					Control hostControl = m_Controller.HostControl;
					Point point;
#if SyncfusionFramework2_0

					if (DockingManager.HostForm != null &&
						DockingManager.HostForm.RightToLeftLayout &&
						DockingManager.HostForm.RightToLeft == RightToLeft.Yes)
					{
						point = hostControl.PointToScreen(new Point(hostControl.Width, 0));
					}
					else
#endif
						point = hostControl.PointToScreen(new Point(0, 0));

					if( m_Controller is MainFormController )
					{
						dragControl.InnerControllerRect = control.RectangleToScreen(m_mainFormController.rcClientPool);
						if( dragControl.DockAbility != dragControl.OuterDockAbility )
						{
							dragControl.DockAbility = dragControl.OuterDockAbility;
						}
					}
					else
					{
						if( ( m_Controller.ToplevelController != this.DraggingControl.InternalController && m_Controller.ChildControllers == null )
                            || (m_Controller.ToplevelController != this.DraggingControl.InternalController && (m_Controller.ChildControllers[0] != null) && !(m_Controller.ChildControllers[0] as DockHostController).FloatOnly))
						{
							if( m_Controller is FloatingFormController && m_Controller.ChildControllers.Count > 0 )
								m_Controller = m_Controller.ChildControllers[0] as DockControllerBase;

							ContainerControl container = m_Controller.HostControl as ContainerControl;
							point = container.Controls[0].PointToScreen( new Point( 0, 0 ) );
							dragControl.DockAbility = DockingManager.GetDockAbility( container.Controls[0] );
							dragControl.InnerControllerRect = new Rectangle( point, container.Controls[0].Size );
						}
					}
					m_noControl = false;
				}
				else
					m_noControl = true;
			}
			catch (Exception ex)
			{
				Trace.WriteLine( ex.Message + Environment.NewLine + ex.StackTrace, "Exception" );
				return;
			}
		}
		private bool m_noControl = true;
		private DockControllerBase m_prevUnderlaying = null;

		private DockControllerBase GetUnderlayingController( MainFormController mfc, Point ptscreen )
		{
			DockControllerBase underTarget = null;

			if( mfc != null && mfc.HostControl != null )
			{
				Control mainform = mfc.HostControl;

				for( int i = 0; i < DockingManager.ControlsArray.Length; i++ )
				{
					DockHostController dhc = DockingManager.GetDockHostController( DockingManager.ControlsArray[i] );

					if( dhc != null && dhc.DockVisibility )
					{
						Rectangle rcScreen = ( dhc.Floating ) ?
							dhc.HostControl.TopLevelControl.RectangleToScreen( dhc.HostControl.Bounds ) :
							mainform.RectangleToScreen( dhc.HostControl.Bounds );
						if( rcScreen.Contains( ptscreen ) && dhc != m_DragController
							&& DockingManager.GetFloatOnly( DockingManager.ControlsArray[i] ) == false )
						{
							underTarget = dhc;
							break;
						}
					}
				}
				if( underTarget == null && mainform.Bounds.Contains( ptscreen ) )
					underTarget = mfc;
			}

			return underTarget;
		}

		private MainFormController GetMainFormController( Point ptscreen )
		{
			DockingManager tempDm = DockingManager.GetManagerFromPoint(ptscreen);
			MainFormController mfc = null;

			if (m_Controller is MainFormController)
				mfc = m_Controller as MainFormController;
			else
			{
				if (m_Controller != null)
				{
					mfc = m_Controller.DockingManager.dcHostForm as MainFormController;
				}
				else
				{
					if (tempDm != null)
						mfc = tempDm.dcHostForm;
					else
						mfc = DockingManager.dcHostForm;
				}
			}

			if (mfc.HostControl == null)
				return null;

			if( !mfc.HostControl.Visible )
			{
				DockingManager visibleDm = DockingManager.GetVisibleDockingManager();

				if( visibleDm != null )
					mfc = visibleDm.dcHostForm;
			}

			return mfc;
		}
		
		protected void dragControl_OnAllowHighlight( object sender, AllowHighlightEventArgs e )
		{
			Form hostForm = DockingManager.HostForm;
			e.AllowHighlight = IsDockAllowed( e.DragTarget );
		}

		private bool IsDockAllowed( DragTarget dragTarget )
		{
			DockHostController dhc = m_Controller as DockHostController;
			if( dhc != null && dhc.HostControl != null )
			{
				if( DockingManager.GetFloatOnly( dhc.HostControl.Controls[0] ) )
					return false;
			}
			DockHost dhDrag = DraggingControl as DockHost;
			if( dhDrag != null )
			{
				if( DockingManager.GetFloatOnly( dhDrag.Controls[0] ) )
					return false;
			}
			
			Control hostControl = DockingManager.HostControl;
			bool canApplyTabDock = true;
			if( !m_Controller.Floating & m_Controller.ToplevelController.HostControl is ContainerControl &
				m_Controller is MainFormController)
			{
				Form targetForm = m_Controller.ToplevelController.HostControl as Form;
				if( targetForm != null )
					canApplyTabDock = ((Form)m_Controller.ToplevelController.HostControl).IsMdiContainer;
				else
					canApplyTabDock = false;
			}
					
				
			return 	((hostControl != null) && canApplyTabDock)
				|| (dragTarget != DragTarget.InnerTab);
		}

		private void HostControl_Move(object sender, EventArgs e)
		{
			//The Whidbey DragProvider throws Null reference exception on closing the MDI window.

			if (m_DockingManager != null && m_DockingManager.HostControl != null)
				UpdateDragControlPosition(Cursor.Position);
		}

		private void HostControl_Resize(object sender, EventArgs e)
		{
			//The Whidbey DragProvider throws Null reference exception on closing the MDI window.

			if (m_DockingManager != null && m_DockingManager.HostControl != null)
				UpdateDragControlPosition(Cursor.Position);
		}

		protected void dragControl_OnMouseEnter( object sender, DragControlEventArgs e )
		{
			IDockable draggedControl = DraggingControl as IDockable;
			if( draggedControl != null )
			{
				processedDragControl = true;
				Rectangle hostArea = Rectangle.Empty;
				DockControllerBase controller = draggedControl.GetController();
				DockHost dh = draggedControl as DockHost;
				if( dh != null && DockingManager.GetFloatOnly( dh.Controls[0] ) )
					return;
				if( e.DragTarget == DragTarget.InnerTab && m_Controller is DockHostController )
					hostArea = ( m_Controller as DockHostController ).GetTabDockTargetRectangle();
				if( hostArea == Rectangle.Empty )
					hostArea = e.HostDockArea;

				DragDockInfo dragDockInfo = GetDragDockInfo(hostArea,
					controller, e.DragTarget);
				if( IsDockAllowed( DraggingControl, dragDockInfo.Controller ) )
					Cursor.Current = Cursors.Default;
					
				targetForm.Bounds = dragDockInfo.DockArea;
				if( e.DragTarget == DragTarget.InnerTab )
				{
					DockHostController hostController = dragDockInfo.Controller as DockHostController;
					if( hostController != null )
					{
						targetForm.ShowRegion( DockingManager.DockTabAlignment );
					}
				}
				else
				{
					targetForm.ShowRectangle( dragDockInfo.DockArea );
				}
			}
		}

		protected void dragControl_OnMouseLeave( object sender, DragControlEventArgs e )
		{
			SetDragCursor();
			targetForm.Hide();
			processedDragControl = false;
		}

		protected void dragControl_OnMouseUp( object sender, DragControlEventArgs e )
		{
			if( DraggingControl != null )
			{
				DockControllerBase dockController = null;
				DockHost dockHost = DraggingControl as DockHost;
				if( dockHost != null )
				{
					dockController = dockHost.InternalController; 
				}

				FloatingForm floatingForm = DraggingControl as FloatingForm;
				if( floatingForm != null )
				{
					dockController = floatingForm.InternalController; 
				}

				if( dockController != null )
				{
					
					DragDockInfo dragDockInfo = GetDragDockInfo(e.HostDockArea,
						dockController, e.DragTarget);
					Form dockForm = dragDockInfo.Controller.HostControl as Form;

					if( (dockForm != null) && (e.DragTarget == DragTarget.InnerTab) &&
						dockForm.IsMdiContainer )
					{
						tabbedMDI = true;
					}

					DockingStyle dstyle = dragDockInfo.DockingStyle;

#if SyncfusionFramework2_0
					bool targetFloating = ( dragDockInfo.Controller != null && dragDockInfo.Controller.Floating)? true: false;
					if ( !targetFloating &&
						DockingManager.HostForm != null &&
						DockingManager.HostForm.RightToLeftLayout &&
						DockingManager.HostForm.RightToLeft == RightToLeft.Yes)
					{
						if (dstyle == DockingStyle.Left)
							dstyle = DockingStyle.Right;
						else if (dstyle == DockingStyle.Right)
							dstyle = DockingStyle.Left;
					}
#endif

					if( dockHost != null )
					{
						DockHostController hostController = dockHost.InternalController as DockHostController;
						hostController.DINew.dController = dragDockInfo.Controller;
						hostController.DINew.dStyle = dstyle;
						hostController.DINew.DP = dragDockInfo.DockPreference;
						hostController.DINew.nPriority = dragDockInfo.Priority;
						hostController.DINew.rcDockArea = dragDockInfo.DockArea;
						if( dragDockInfo.DockingStyle == DockingStyle.Tabbed )
							hostController.DINew.nDockIndex = 0;
					}

					if( floatingForm != null )
					{
						FloatingFormController formController = floatingForm.InternalController as FloatingFormController;
						formController.DICurrent.dController = dragDockInfo.Controller;
						formController.DICurrent.dStyle = dstyle;
						formController.DICurrent.DP = dragDockInfo.DockPreference;
						formController.DICurrent.nPriority = dragDockInfo.Priority;
						formController.DICurrent.rcDockArea = dragDockInfo.DockArea;
						if( dragDockInfo.DockingStyle == DockingStyle.Tabbed )
							formController.DICurrent.nDockIndex = 0;
					}
				}
				targetForm.Hide();
			}
		}

		public DragDockInfo GetDragDockInfo(Rectangle hostRectangle, 
			DockControllerBase dockController, DragTarget dragTarget)
		{
			DragDockInfo dragDockInfo = new DragDockInfo();
			int x = 0, y = 0, width = 0, height = 0;			

			Point pt = m_mainFormController.HostControl.PointToClient(hostRectangle.Location);
			x = pt.X;
			y = pt.Y;
			int prefferedHeight;
			DockStateControllerBase controller = dockController as DockStateControllerBase;
			if( controller != null )
			{
				prefferedHeight = controller.DINew.rcDockArea.Height;
			}
			else
			{
				prefferedHeight = dockController.LayoutRect.Height + CaptionPainter.CaptionHeight;
			}					

			switch( dragTarget )
			{
				case DragTarget.OuterLeft:
				{
					dragDockInfo.DockPreference = DockPreference.Horizontal;
					dragDockInfo.DockingStyle = DockingStyle.Left;
					dragDockInfo.Priority = 0;
					dragDockInfo.Controller = m_mainFormController;
					width = EstimateLength(hostRectangle.Width, 
						dockController.LayoutRect.Width);
					height = hostRectangle.Height;
					break;
				}

				case DragTarget.OuterTop:
				{
					dragDockInfo.DockPreference = DockPreference.Vertical;
					dragDockInfo.DockingStyle = DockingStyle.Top;
					dragDockInfo.Priority = 0;
					dragDockInfo.Controller = m_mainFormController;
					width = hostRectangle.Width;
					height = EstimateLength(hostRectangle.Height,
						prefferedHeight);
					break;
				}

				case DragTarget.OuterRight:
				{
					dragDockInfo.DockPreference = DockPreference.Horizontal;
					dragDockInfo.DockingStyle = DockingStyle.Right;
					dragDockInfo.Priority = 0;
					dragDockInfo.Controller = m_mainFormController;
					width = EstimateLength(hostRectangle.Width,
						dockController.LayoutRect.Width);
					x += hostRectangle.Width - width;
					height = hostRectangle.Height;
					break;
				}

				case DragTarget.OuterBottom:
				{
					dragDockInfo.DockPreference = DockPreference.Vertical;
					dragDockInfo.DockingStyle = DockingStyle.Bottom;
					dragDockInfo.Priority = 0;
					dragDockInfo.Controller = m_mainFormController;
					width = hostRectangle.Width;
					height = EstimateLength(hostRectangle.Height,
						prefferedHeight);
					y += hostRectangle.Height - height;
					break;
				}

				case DragTarget.InnerLeft:
				{
					dragDockInfo.DockPreference = DockPreference.Horizontal;
					dragDockInfo.DockingStyle = DockingStyle.Left;
					dragDockInfo.Priority = -1;
					dragDockInfo.Controller = m_Controller;

					width = EstimateLength(hostRectangle.Width, 
						dockController.LayoutRect.Width);
					height = hostRectangle.Height;

					break;
				}

				case DragTarget.InnerTop:
				{
					dragDockInfo.DockPreference = DockPreference.Vertical;
					dragDockInfo.DockingStyle = DockingStyle.Top;
					dragDockInfo.Priority = -1;
					dragDockInfo.Controller = m_Controller;
					width = hostRectangle.Width;
					height = EstimateLength(hostRectangle.Height, 
						prefferedHeight);
					break;
				}

				case DragTarget.InnerRight:
				{
					dragDockInfo.DockPreference = DockPreference.Horizontal;
					dragDockInfo.DockingStyle = DockingStyle.Right;
					dragDockInfo.Priority = -1;
					dragDockInfo.Controller = m_Controller;
					width = EstimateLength(hostRectangle.Width,
						dockController.LayoutRect.Width);
					x += hostRectangle.Width - width;
					height = hostRectangle.Height;
					break;
				}

				case DragTarget.InnerBottom:
				{
					dragDockInfo.DockPreference = DockPreference.Vertical;
					dragDockInfo.DockingStyle = DockingStyle.Bottom;
					dragDockInfo.Priority = -1;
					dragDockInfo.Controller = m_Controller;
					width = hostRectangle.Width;
					height = EstimateLength(hostRectangle.Height, 
						prefferedHeight);
					y += hostRectangle.Height - height;
					break;
				}

				case DragTarget.InnerTab:
				{
					dragDockInfo.DockPreference = DockPreference.Tabbed;
					dragDockInfo.DockingStyle = DockingStyle.Tabbed;
					dragDockInfo.Priority = 0;
					dragDockInfo.Controller = m_Controller;
					width = hostRectangle.Width;
					height = hostRectangle.Height;
					break;
				}
			}

			Control hostControl = m_mainFormController.HostControl as Control;
			Rectangle rect = new Rectangle(x, y, width, height);
			rect = hostControl.RectangleToScreen(rect);

#if SyncfusionFramework2_0
			if (DockingManager.HostForm != null &&
				DockingManager.HostForm.RightToLeft == RightToLeft.Yes &&
				DockingManager.HostForm.RightToLeftLayout == true)
			{
				rect.X = rect.Right + 2 * (hostRectangle.X - rect.Right);
			}
#endif

			dragDockInfo.DockArea = rect;
			return dragDockInfo;
		}

		private int EstimateLength(int parentLength, int childLength)
		{
			return 2 * childLength > parentLength ? parentLength / 2 : childLength;
		}
	}
    /// <summary>
    /// DragProvider in Whidbey style.
    /// </summary>
    internal class VS2012DragProvider
        : WhidbeyDragProvider
    {
        #region Class members
        private Color m_targetBorderColor = Color.Empty;
        private Color m_targetBackColor = Color.Empty;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dockingManager">see the <see cref="Syncfusion.Windows.Forms.Tools.DockingManager"/></param>
        public VS2012DragProvider(DockingManager dockingManager)
            : base(dockingManager)
        { }

        protected override void InitializeProvider()
        {
            this.dragControl = new VS2012DragControl();

            dragControl.OnMouseEnter += new MouseEnterHandler(dragControl_OnMouseEnter);
            dragControl.OnMouseLeave += new MouseLeaveHandler(dragControl_OnMouseLeave);
            dragControl.OnMouseUp += new MouseUpHandler(dragControl_OnMouseUp);
            dragControl.OnAllowHighlight += new AllowHighlightHandler(dragControl_OnAllowHighlight);

            CreateTargetForm(DockingManager);
        }
        ~VS2012DragProvider()
        {
            if (dragControl != null)
            {
                dragControl.OnMouseEnter -= new MouseEnterHandler(dragControl_OnMouseEnter);
                dragControl.OnMouseLeave -= new MouseLeaveHandler(dragControl_OnMouseLeave);
                dragControl.OnMouseUp -= new MouseUpHandler(dragControl_OnMouseUp);
                dragControl.OnAllowHighlight -= new AllowHighlightHandler(dragControl_OnAllowHighlight);
            }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Updates Colors.
        /// </summary>
        public override void UpdateColors()
        {
            m_targetBackColor = SystemColors.ActiveCaption;
            m_targetBorderColor = SystemColors.ControlDark;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Creates target form with specific parameters.
        /// </summary>
        /// <param name="dockingManager">The <see cref="Syncfusion.Windows.Forms.Tools.DockingManager"/></param>
        protected override void CreateTargetForm(DockingManager dockingManager)
        {
            UpdateColors();
            targetForm = new TargetForm(dockingManager);
            targetForm.BackColor = m_targetBackColor;
            targetForm.BorderColor = m_targetBorderColor;
            targetForm.BorderWidth = 2;
            targetForm.Opacity = 0.25;
        }
        #endregion
    }
    /// <summary>
    /// DragProvider in Whidbey style.
    /// </summary>
    internal class VS2010DragProvider
        : WhidbeyDragProvider
    {
        #region Class members
        private Color m_targetBorderColor = Color.Empty;
        private Color m_targetBackColor = Color.Empty;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dockingManager">see the <see cref="Syncfusion.Windows.Forms.Tools.DockingManager"/></param>
        public VS2010DragProvider(DockingManager dockingManager)
            : base(dockingManager)
        { }

        protected override void InitializeProvider()
        {
            this.dragControl = new VS2010DragControl();

            dragControl.OnMouseEnter += new MouseEnterHandler(dragControl_OnMouseEnter);
            dragControl.OnMouseLeave += new MouseLeaveHandler(dragControl_OnMouseLeave);
            dragControl.OnMouseUp += new MouseUpHandler(dragControl_OnMouseUp);
            dragControl.OnAllowHighlight += new AllowHighlightHandler(dragControl_OnAllowHighlight);

            CreateTargetForm(DockingManager);
        }
        ~VS2010DragProvider()
        {
            if (dragControl != null)
            {
                dragControl.OnMouseEnter -= new MouseEnterHandler(dragControl_OnMouseEnter);
                dragControl.OnMouseLeave -= new MouseLeaveHandler(dragControl_OnMouseLeave);
                dragControl.OnMouseUp -= new MouseUpHandler(dragControl_OnMouseUp);
                dragControl.OnAllowHighlight -= new AllowHighlightHandler(dragControl_OnAllowHighlight);
            }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Updates Colors.
        /// </summary>
        public override void UpdateColors()
        {
            m_targetBackColor = Color.FromArgb(173, 214, 255);
            m_targetBorderColor = Color.FromArgb(225, 225, 225);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Creates target form with specific parameters.
        /// </summary>
        /// <param name="dockingManager">The <see cref="Syncfusion.Windows.Forms.Tools.DockingManager"/></param>
        protected override void CreateTargetForm(DockingManager dockingManager)
        {
            UpdateColors();
            targetForm = new TargetForm(dockingManager);
            targetForm.BackColor = m_targetBackColor;
            targetForm.BorderColor = m_targetBorderColor;
            targetForm.BorderWidth = 2;
            targetForm.Opacity = 0.25;
        }
        #endregion
    }
	/// <summary>
	/// DragProvider in Whidbey style.
	/// </summary>
	internal class VS2005DragProvider
		: WhidbeyDragProvider
	{
		#region Class members
		private Color m_targetBorderColor = Color.Empty;
		private Color m_targetBackColor = Color.Empty;
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dockingManager">see the <see cref="Syncfusion.Windows.Forms.Tools.DockingManager"/></param>
		public VS2005DragProvider( DockingManager dockingManager )
			: base( dockingManager )
		{}

		protected override void InitializeProvider()
		{
			this.dragControl = new VS2005DragControl();

			dragControl.OnMouseEnter += new MouseEnterHandler(dragControl_OnMouseEnter);
			dragControl.OnMouseLeave += new MouseLeaveHandler(dragControl_OnMouseLeave);
			dragControl.OnMouseUp += new MouseUpHandler(dragControl_OnMouseUp);
			dragControl.OnAllowHighlight +=new AllowHighlightHandler(dragControl_OnAllowHighlight);

			CreateTargetForm( DockingManager );
		}
        ~VS2005DragProvider()
        {
            if (dragControl != null)
            {
                dragControl.OnMouseEnter -= new MouseEnterHandler(dragControl_OnMouseEnter);
                dragControl.OnMouseLeave -= new MouseLeaveHandler(dragControl_OnMouseLeave);
                dragControl.OnMouseUp -= new MouseUpHandler(dragControl_OnMouseUp);
                dragControl.OnAllowHighlight -= new AllowHighlightHandler(dragControl_OnAllowHighlight);
            }
        }
		#endregion

		#region Class public methods
		/// <summary>
		/// Updates Colors.
		/// </summary>
		public override void UpdateColors()
		{
			m_targetBackColor = SystemColors.ActiveCaption;
			m_targetBorderColor = SystemColors.ControlDark;
		}
		#endregion

		#region Class overrides
		/// <summary>
		/// Creates target form with specific parameters.
		/// </summary>
		/// <param name="dockingManager">The <see cref="Syncfusion.Windows.Forms.Tools.DockingManager"/></param>
		protected override void CreateTargetForm(DockingManager dockingManager)
		{
			UpdateColors();
			targetForm = new TargetForm( dockingManager);
			targetForm.BackColor = m_targetBackColor;
			targetForm.BorderColor = m_targetBorderColor;
			targetForm.BorderWidth = 2;
			targetForm.Opacity = 0.25;
		}
		#endregion
	}

	internal class VS2005DragControlConsts
	{
		#region Image names

		public static string DEF_IMAGES_PATH = "FrameworkComponents.DockingWindows.Images.VS2005DragProvider.";
		public static string DEF_BOTTOM_TARGET_IMAGE_NAME = "BottomArrow.bmp";
		public static string DEF_BOTTOM_TARGET_IMAGE_HIGHLIGHT = "BottomArrowHighlight.bmp";
		public static string DEF_TOP_TARGET_IMAGE_NAME = "TopArrow.bmp";
		public static string DEF_TOP_TARGET_IMAGE_HIGHLIGHT = "TopArrowHighlight.bmp";
		public static string DEF_LEFT_TARGET_IMAGE_NAME = "LeftArrow.bmp";
		public static string DEF_LEFT_TARGET_IMAGE_HIGHLIGHT = "LeftArrowHighlight.bmp";
		public static string DEF_RIGHT_TARGET_IMAGE_NAME = "RightArrow.bmp";
		public static string DEF_RIGHT_TARGET_IMAGE_HIGHLIGHT = "RightArrowHighlight.bmp";
		public static string DEF_INNER_TARGET_IMAGE_NAME = "Background.bmp";
		public static string DEF_BOTTOM_INNER_TARGET_IMAGE_NAME = "BottomInnerArrow.bmp";
		public static string DEF_BOTTOM_INNER_TARGET_IMAGE_HIGHLIGHT = "BottomInnerArrowHighlight.bmp";
		public static string DEF_TOP_INNER_TARGET_IMAGE_NAME = "TopInnerArrow.bmp";
		public static string DEF_TOP_INNER_TARGET_IMAGE_HIGHLIGHT = "TopInnerArrowHighlight.bmp";
		public static string DEF_LEFT_INNER_TARGET_IMAGE_NAME = "LeftInnerArrow.bmp";
		public static string DEF_LEFT_INNER_TARGET_IMAGE_HIGHLIGHT = "LeftInnerArrowHighlight.bmp";
		public static string DEF_RIGHT_INNER_TARGET_IMAGE_NAME = "RightInnerArrow.bmp";
		public static string DEF_RIGHT_INNER_TARGET_IMAGE_HIGHLIGHT = "RightInnerArrowHighlight.bmp";
		public static string DEF_TAB_INNER_TARGET_IMAGE_NAME = "TabInner.bmp";
		public static string DEF_TAB_INNER_TARGET_IMAGE_HIGHLIGHT = "TabInnerHighlight.bmp";

		#endregion

		#region Image indexes
		public static int DEF_LEFT_INACTIVE_TARGET = 0;
		public static int DEF_TOP_INACTIVE_TARGET = 1;
		public static int DEF_RIGHT_INACTIVE_TARGET = 2;
		public static int DEF_BOTTOM_INACTIVE_TARGET = 3;
		public static int DEF_INACTIVE_TAB = 4;
		public static int DEF_INNER_TARGET = 5;
		public static int DEF_LEFT_INNER_INACTIVE_TARGET = 6;
		public static int DEF_TOP_INNER_INACTIVE_TARGET = 7;
		public static int DEF_RIGHT_INNER_INACTIVE_TARGET = 8;
		public static int DEF_BOTTOM_INNER_INACTIVE_TARGET = 9;
		public static int DEF_LEFT_ACTIVE_TARGET = 10;
		public static int DEF_TOP_ACTIVE_TARGET = 11;
		public static int DEF_RIGHT_ACTIVE_TARGET = 12;
		public static int DEF_BOTTOM_ACTIVE_TARGET = 13;
		public static int DEF_ACTIVE_TAB = 14;
		public static int DEF_LEFT_INNER_ACTIVE_TARGET = 15;
		public static int DEF_TOP_INNER_ACTIVE_TARGET = 16;
		public static int DEF_RIGHT_INNER_ACTIVE_TARGET = 17;
		public static int DEF_BOTTOM_INNER_ACTIVE_TARGET = 18;
		#endregion
	}

	/// <summary>
	/// Drag control for Whidbey drag provider style.
	/// </summary>
	internal class VS2005DragControl: DragControl
	{
		#region Class initialize/finalize methods
		/// <summary>
		/// Constructor.
		/// </summary>
		public VS2005DragControl()
		{
			m_OuterControl = new VS2005OuterDragControl(this);
			m_OuterControl.OnMouseEnter += new MouseEnterHandler(MouseEnterEvent);
			m_OuterControl.OnMouseLeave += new MouseLeaveHandler(MouseLeaveEvent);
			m_OuterControl.OnMouseUp += new MouseUpHandler(MouseUpEvent);
			m_OuterControl.OnMouseMove += new MouseMoveHandler(OuterMouseMoveEvent);

			m_InnerControl = new VS2005InnerDragControl(this);
			m_InnerControl.OnMouseEnter += new MouseEnterHandler(MouseEnterEvent);
			m_InnerControl.OnMouseLeave += new MouseLeaveHandler(MouseLeaveEvent);
			m_InnerControl.OnMouseUp += new MouseUpHandler(MouseUpEvent);
			m_InnerControl.OnAllowHighlight += new AllowHighlightHandler(AllowHighlightEvent);
		}
		#endregion

		#region Class overrides
		/// <summary>
		/// Loads images.
		/// </summary>
		protected override void InitializeImages()
		{
			string path = VS2005DragControlConsts.DEF_IMAGES_PATH;
			int index = 0;
			Bitmap bitmap = null;
			bitmap = (Bitmap) ImageLoader.Load(path+VS2005DragControlConsts.DEF_LEFT_TARGET_IMAGE_NAME);
			m_Images[index++] = new Bitmap(bitmap);
			bitmap = (Bitmap) ImageLoader.Load(path+VS2005DragControlConsts.DEF_TOP_TARGET_IMAGE_NAME);
			m_Images[index++] = new Bitmap(bitmap);
			bitmap = (Bitmap) ImageLoader.Load(path+VS2005DragControlConsts.DEF_RIGHT_TARGET_IMAGE_NAME);
			m_Images[index++] = new Bitmap(bitmap);
			bitmap = (Bitmap) ImageLoader.Load(path+VS2005DragControlConsts.DEF_BOTTOM_TARGET_IMAGE_NAME);
			m_Images[index++] = new Bitmap(bitmap);
			bitmap = (Bitmap) ImageLoader.Load(path+VS2005DragControlConsts.DEF_INNER_TARGET_IMAGE_NAME);
			bitmap.MakeTransparent(Color.FromArgb(0, 255, 0));
			m_Images[index++] = new Bitmap(bitmap);
			bitmap = (Bitmap) ImageLoader.Load(path+VS2005DragControlConsts.DEF_LEFT_INNER_TARGET_IMAGE_NAME);
			m_Images[index++] = new Bitmap(bitmap);
			bitmap = (Bitmap) ImageLoader.Load(path+VS2005DragControlConsts.DEF_TOP_INNER_TARGET_IMAGE_NAME);
			m_Images[index++] = new Bitmap(bitmap);
			bitmap = (Bitmap) ImageLoader.Load(path+VS2005DragControlConsts.DEF_RIGHT_INNER_TARGET_IMAGE_NAME);
			m_Images[index++] = new Bitmap(bitmap);
			bitmap = (Bitmap) ImageLoader.Load(path+VS2005DragControlConsts.DEF_BOTTOM_INNER_TARGET_IMAGE_NAME);
			m_Images[index++] = new Bitmap(bitmap);
			bitmap = (Bitmap) ImageLoader.Load(path+VS2005DragControlConsts.DEF_TAB_INNER_TARGET_IMAGE_NAME);
			m_Images[index++] = new Bitmap(bitmap);
		}

		public override void Dispose()
		{
			if (m_OuterControl != null)
			{
				m_OuterControl.OnMouseEnter -= new MouseEnterHandler(MouseEnterEvent);
				m_OuterControl.OnMouseLeave -= new MouseLeaveHandler(MouseLeaveEvent);
				m_OuterControl.OnMouseUp -= new MouseUpHandler(MouseUpEvent);
				m_OuterControl.OnMouseMove -= new MouseMoveHandler(OuterMouseMoveEvent);
				m_OuterControl.Dispose();
				m_OuterControl = null;
			}

			if (m_InnerControl != null)
			{
				m_InnerControl.OnMouseEnter -= new MouseEnterHandler(MouseEnterEvent);
				m_InnerControl.OnMouseLeave -= new MouseLeaveHandler(MouseLeaveEvent);
				m_InnerControl.OnMouseUp -= new MouseUpHandler(MouseUpEvent);
				m_InnerControl.OnAllowHighlight -= new AllowHighlightHandler(AllowHighlightEvent);
				m_InnerControl.Dispose();
				m_InnerControl = null;
			}

			base.Dispose();
		}
		#endregion
	}
	/// <summary>
	/// WhidbeyDragTargetControl.
	/// </summary>
	internal class VS2005DragTargetControl
		: DragTargetControl
	{
		#region Class constants
		/// <summary>
		/// Defines constatn hilight color.
		/// </summary>
		private static readonly Color DEF_HILIGHT_COLOR = Color.FromArgb( 65, 112, 202);
		#endregion

		#region Class overrides
		/// <summary>
		/// Overrider for its special hilight behaviour.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public override void OnPaint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			e.Graphics.DrawImage( inactiveImage, m_internalControl.ClientRectangle );
			if( m_mouseIn )
			{
				Rectangle hilightRectangle = new Rectangle( 0,0, inactiveImage.Width-1,inactiveImage.Height - 1 );
				e.Graphics.DrawRectangle( new Pen(DEF_HILIGHT_COLOR), hilightRectangle );
			}
		}

		public override void HideControl()
		{
			if( m_mouseIn )
			{
				RaiseMouseLeaveEvent();
				m_mouseIn = false;
			}

			base.HideControl();
		}
		#endregion

		#region Class public methods
		/// <summary>
		/// Get current image.
		/// </summary>
		public override Image CurrentImage
		{
			get
			{
				return inactiveImage;
			}
		}
		#endregion
	}
	/// <summary>
	/// InnerDragControl fo Whidbey drag provider.
	/// </summary>
	internal class VS2005InnerDragControl
		: DragInnerControl
		, IArrowDragControl
	{
		#region Class members
		private Size m_ControlSize = new Size(121, 121);
		private Bitmap m_innerCross;
		private Point m_crossLocation;
		private Size m_tabSize = new Size(29,29);
		private Point[] m_hilightPath = null;
		private Size m_hitSize = new Size( 29, 29 );
		private bool m_bTabbedSelection = false;		
		#endregion

		#region Class events
		public new event MouseEnterHandler OnMouseEnter;
		public new event MouseLeaveHandler OnMouseLeave;
		public new event AllowHighlightHandler OnAllowHighlight;
		#endregion

		#region Class constants
		private const int DEF_LEFT_TARGET = 0;
		private const int DEF_TOP_TARGET = 1;
		private const int DEF_RIGHT_TARGET = 2;
		private const int DEF_BOTTOM_TARGET = 3;
		private const int DEF_TAB_TARGET = 4;
		private const int DEF_INNER_TARGET_START_INDEX = 5;
		private const int DEF_BORDER_WIDTH = 1;
		private static readonly Color DEF_SELECT_COLOR = Color.FromArgb( 65, 112, 202);
		#endregion

		#region Class initialize/finalize methods
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parentControl">Parent controller</param>
		public VS2005InnerDragControl(DragControl parentControl)
			: base( parentControl )
		{
			m_ParentControl = parentControl;

			CalculateControlsPosition();
		}
		#endregion

		#region Class overrides
		/// <summary>
		/// Initializes components.
		/// </summary>
		protected override void InitializeControl()
		{
			m_internalControl = new DragTargetForm();
			m_internalControl.ShowInTaskbar = false;
			m_internalControl.FormBorderStyle = FormBorderStyle.None;
			m_internalControl.TransparencyKey = m_internalControl.BackColor;
			m_internalControl.StartPosition = FormStartPosition.Manual;
			m_internalControl.TopMost = true;
			m_internalControl.Paint += new PaintEventHandler(OnPaint);
			
			m_innerCross = m_ParentControl.GetImage(4);
		}
		/// <summary>
		/// Calculates hit areas.
		/// </summary>
		protected override void CalculateControlsPosition()
		{
			m_Center.X = (m_ControlSize.Width+1) / 2;
			m_Center.Y = (m_ControlSize.Height+1) / 2;
			Size crossSize = m_innerCross.Size;
			m_crossLocation = new Point( Center.X - (crossSize.Width)/2,
				Center.Y - (crossSize.Height)/2);

			m_Rectangles[DEF_TAB_TARGET] = new Rectangle(
				m_Center.X - (m_tabSize.Width +1) / 2,
				m_Center.Y - (m_tabSize.Height+1) / 2,
				m_tabSize.Width, m_tabSize.Height);

			m_Rectangles[DEF_LEFT_TARGET] = new Rectangle(
				m_TabRectangle.X - m_hitSize.Width-1,	
				m_TabRectangle.Y,
				m_hitSize.Width, m_hitSize.Height);

			m_Rectangles[DEF_TOP_TARGET] = new Rectangle(
				m_TabRectangle.X, 
				m_TabRectangle.Y - m_hitSize.Height-1, 
				m_hitSize.Width, m_hitSize.Height);

			m_Rectangles[DEF_RIGHT_TARGET] = new Rectangle(
				m_TabRectangle.X + m_tabSize.Width+1,
				m_TabRectangle.Y,
				m_hitSize.Width, m_hitSize.Height);

			m_Rectangles[DEF_BOTTOM_TARGET] = new Rectangle(
				m_TabRectangle.X,
				m_TabRectangle.Y + m_tabSize.Height+1,
				m_hitSize.Width, m_hitSize.Height);
		}
		/// <summary>
		/// Paints control area.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public override void OnPaint(object sender, PaintEventArgs e)
		{
			if( DockAbility == DockAbility.None )
			{
				return;
			}

			Graphics graphics = e.Graphics;
			Point center = Center;
			Size crossSize = m_innerCross.Size;
			m_crossLocation = new Point( center.X - (crossSize.Width)/2,
				center.Y - (crossSize.Height)/2);
            using (Brush br = new SolidBrush(Color.Transparent))
                graphics.FillRectangle(br, new Rectangle(m_crossLocation, crossSize));
			
			Pen borderPen = new Pen(DEF_SELECT_COLOR, DEF_BORDER_WIDTH );

			graphics.DrawImage( m_innerCross, m_crossLocation );

			if( (DockAbility | DockAbility.Left) == DockAbility )
			{
				graphics.DrawImage( m_ParentControl.GetImage(DEF_INNER_TARGET_START_INDEX + DEF_LEFT_TARGET), 
					m_Rectangles[DEF_LEFT_TARGET] );
			}
			if( (DockAbility | DockAbility.Top) == DockAbility )
			{
				graphics.DrawImage( m_ParentControl.GetImage(DEF_INNER_TARGET_START_INDEX + DEF_TOP_TARGET), 
					m_Rectangles[DEF_TOP_TARGET] );
			}
			if( (DockAbility | DockAbility.Right) == DockAbility )
			{
				graphics.DrawImage( m_ParentControl.GetImage(DEF_INNER_TARGET_START_INDEX + DEF_RIGHT_TARGET), 
					m_Rectangles[DEF_RIGHT_TARGET] );
			}
			if( (DockAbility | DockAbility.Bottom) == DockAbility )
			{
				graphics.DrawImage( m_ParentControl.GetImage(DEF_INNER_TARGET_START_INDEX + DEF_BOTTOM_TARGET), 
					m_Rectangles[DEF_BOTTOM_TARGET] );
			}
			if( (DockAbility | DockAbility.Tabbed) == DockAbility )
			{
				graphics.DrawImage( m_ParentControl.GetImage(DEF_INNER_TARGET_START_INDEX + DEF_TAB_TARGET), 
					m_Rectangles[DEF_TAB_TARGET] );
			}

			if( m_ActiveControlIndex != -1 && m_hilightPath != null)
			{
				if( m_bTabbedSelection )
				{
					graphics.DrawLine( borderPen, m_hilightPath[1], m_hilightPath[2] );
					graphics.DrawLine( borderPen, m_hilightPath[3], m_hilightPath[4] );
					graphics.DrawLine( borderPen, m_hilightPath[5], m_hilightPath[6] );
					graphics.DrawLine( borderPen, m_hilightPath[7], m_hilightPath[0] );
				}
				else
				{
					graphics.DrawLines( borderPen, m_hilightPath );
				}
			}
		}

		protected bool IsControlEnabled( int index )
		{
			switch( index )
			{
				case 0: 
					if( (DockAbility | DockAbility.Left) == DockAbility )
					{
						return true;
					}
					break;
				case 1: 
					if( (DockAbility | DockAbility.Top) == DockAbility )
					{
						return true;
					}
					break;
				case 2: 
					if( (DockAbility | DockAbility.Right) == DockAbility )
					{
						return true;
					}
					break;
				case 3: 
					if( (DockAbility | DockAbility.Bottom) == DockAbility )
					{
						return true;
					}
					break;
				case 4: 
					if( (DockAbility | DockAbility.Tabbed) == DockAbility )
					{
						return true;
					}
					break;
			}
			return false;
		}

		/// <summary>
		/// Processes mouse move.
		/// </summary>
		/// <param name="point">Mouse position.</param>
		/// <returns>if processed.</returns>
		public override bool ProcessMouseMove(Point point)
		{
            if( !this.Visible ) 
                return false;

			bool result = false;
			point = m_internalControl.PointToClient(point);
			int activeControlIndex = -1;
			for( int i = 0; i < DragControlConsts.DEF_INNER_CONTROL_COUNT; i++ )
			{
				if( m_Rectangles[i].Contains(point) )
				{
					point.X -= m_Rectangles[i].Location.X;
					point.Y -= m_Rectangles[i].Location.Y;
					if( IsControlEnabled(i) )
					{
						activeControlIndex = i;
						result = true;
						break;
					}
				}
			}
			
			if( m_ActiveControlIndex != activeControlIndex )
			{
				m_hilightPath = null;
				if( m_ActiveControlIndex != -1 )
				{
					if( this.OnMouseLeave != null )
					{
						string currentTargetStr = Enum.GetName(DragTarget.GetType(), m_ActiveControlIndex);
						DragTarget currentDragTarget = (DragTarget) Enum.Parse(DragTarget.GetType(), currentTargetStr);
						OnMouseLeave(this, new DragControlEventArgs(currentDragTarget, ControllerRect));
					}
					m_internalControl.Invalidate();
				}

				bool allowHighlight = false;
				string targetStr = Enum.GetName(DragTarget.GetType(), activeControlIndex);
				DragTarget dragTarget = (DragTarget) Enum.Parse(DragTarget.GetType(), targetStr);
				if( OnAllowHighlight != null )
				{
					AllowHighlightEventArgs args = new AllowHighlightEventArgs( dragTarget );
					OnAllowHighlight( this, args );
					allowHighlight = args.AllowHighlight;
				}

				if( allowHighlight )
				{
					if( activeControlIndex != -1 )
					{
						m_hilightPath = GenerateGraphicsPath( activeControlIndex );
						
						if( OnMouseEnter != null )
						{
							OnMouseEnter(this, new DragControlEventArgs(dragTarget, ControllerRect));
						}
						m_internalControl.Invalidate();
					}
					m_ActiveControlIndex = activeControlIndex;
				}
				else
					m_ActiveControlIndex = -1;

			}

			return result;
		}

		public override void Dispose()
		{
			base.Dispose();

			if (m_internalControl != null)
			{
                m_internalControl.Paint -= new PaintEventHandler(OnPaint);
				m_internalControl.Dispose();
				m_internalControl = null;
			}
			m_innerCross.Dispose();
			m_innerCross = null;
		}

		#endregion

		#region Class helper methods
		/// <summary>
		/// Generates hilighting graphics path .
		/// </summary>
		/// <param name="controlIndex">index of docking(top, left...)</param>
		/// <returns>Graphics path.</returns>
		protected virtual Point[] GenerateGraphicsPath( int controlIndex )
		{
			Point[] hilightPath;
			Size hilightSize = new Size( 22, 28 );
			m_bTabbedSelection = false;

			switch( controlIndex )
			{
				case DEF_LEFT_TARGET:
					hilightPath = new Point[4];
					hilightPath[2] = new Point( m_LeftRectangle.X, m_LeftRectangle.Y );
					hilightPath[3] = new Point( m_LeftRectangle.X + hilightSize.Width, hilightPath[2].Y );
					hilightPath[1] = new Point( m_LeftRectangle.X, hilightPath[2].Y + hilightSize.Height );
					hilightPath[0] = new Point( hilightPath[3].X, hilightPath[1].Y );
					break;

				case DEF_TOP_TARGET:
					hilightPath = new Point[4];
					hilightPath[1] = new Point( m_TopRectangle.X, m_TopRectangle.Y );
					hilightPath[0] = new Point( m_TopRectangle.X, m_TopRectangle.Y + hilightSize.Width );
					hilightPath[2] = new Point( m_TopRectangle.X + hilightSize.Height, m_TopRectangle.Y );
					hilightPath[3] = new Point( hilightPath[2].X, hilightPath[0].Y );
					break;

				case DEF_RIGHT_TARGET:
					hilightPath = new Point[4];
					hilightPath[1] = new Point( m_RightRectangle.X + m_RightRectangle.Width, m_RightRectangle.Y );
					hilightPath[0] = new Point( hilightPath[1].X - hilightSize.Width, m_RightRectangle.Y );
					hilightPath[2] = new Point( hilightPath[1].X, hilightPath[1].Y + hilightSize.Height );
					hilightPath[3] = new Point( hilightPath[0].X ,hilightPath[2].Y );
					break;

				case DEF_BOTTOM_TARGET:
					hilightPath = new Point[4];
					hilightPath[1] = new Point( m_BottomRectangle.X, m_BottomRectangle.Y + m_BottomRectangle.Height );
					hilightPath[0] = new Point( m_BottomRectangle.X, hilightPath[1].Y - hilightSize.Width );
					hilightPath[2] = new Point( hilightPath[1].X + hilightSize.Height, hilightPath[1].Y );
					hilightPath[3] = new Point( hilightPath[2].X, hilightPath[0].Y );
					break;

				case DEF_TAB_TARGET:
					hilightPath = new Point[8];
					hilightPath[1] = new Point( m_TopRectangle.X, m_TopRectangle.Y + hilightSize.Width );
					hilightPath[0] = new Point( hilightPath[1].X + hilightSize.Height, hilightPath[1].Y );
					hilightPath[2] = new Point( m_LeftRectangle.X + hilightSize.Width, m_LeftRectangle.Y );
					hilightPath[3] = new Point( hilightPath[2].X, m_LeftRectangle.Y + hilightSize.Height );
					hilightPath[4] = new Point( m_BottomRectangle.X, m_BottomRectangle.Y + m_hitSize.Height - hilightSize.Width -1 );
					hilightPath[5] = new Point( m_BottomRectangle.X + hilightSize.Height, hilightPath[4].Y );
					hilightPath[7] = new Point( m_RightRectangle.X + m_hitSize.Width - hilightSize.Width - 2, m_LeftRectangle.Y - 1 );
					hilightPath[6] = new Point( hilightPath[7].X, m_RightRectangle.Y + m_RightRectangle.Height );
					m_bTabbedSelection = true;
					break;

				default:
					hilightPath = null;
					break;
			}

			return hilightPath;
		}
		#endregion
	}
	/// <summary>
	/// OuterDragControl for Whidbey drag provider.
	/// </summary>
	internal class VS2005OuterDragControl
		: DragOuterControl
		, IArrowDragControl
	{
		#region Class initialize/finalize methods
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="parentControl">Parent Controller. See the <see cref="Syncfusion.Windows.Forms.Tools.DragControl"/></param>
		public VS2005OuterDragControl(DragControl parentControl)
			: base( parentControl )
		{}
		#endregion

		#region Class overrides
		/// <summary>
		/// Initializes provider with specific parameters.
		/// </summary>
		protected override void InitializeProvider()
		{
			for(int i = 0; i < 4; i++ )
			{
				VS2012DragTargetControl control = new VS2012DragTargetControl();
				m_Controls.Add(control);
				control.OnMouseEnter += new MouseEnterHandler(MouseEnterEvent);
				control.OnMouseLeave += new MouseLeaveHandler(MouseLeaveEvent);
				control.OnMouseUp += new MouseUpHandler(MouseUpEvent);
				control.OnMouseMove += new MouseMoveHandler(MouseMoveEvent);
				control.ParentControl = this;
			}

			leftControl.DragTarget = DragTarget.OuterLeft;
			leftControl.InactiveImage = GetImage(DragControlConsts.DEF_LEFT_INACTIVE_TARGET);

			topControl.DragTarget = DragTarget.OuterTop;
			topControl.InactiveImage = GetImage(DragControlConsts.DEF_TOP_INACTIVE_TARGET);

			rightControl.DragTarget = DragTarget.OuterRight;
			rightControl.InactiveImage = GetImage(DragControlConsts.DEF_RIGHT_INACTIVE_TARGET);

			bottomControl.DragTarget = DragTarget.OuterBottom;
			bottomControl.InactiveImage = GetImage(DragControlConsts.DEF_BOTTOM_INACTIVE_TARGET);
		}

		public override void Dispose()
		{
			for (int i = 0; i < 4; i++)
			{
				DragTargetControl control = m_Controls[i] as DragTargetControl;
				if (control != null)
				{
					control.OnMouseMove -= new MouseMoveHandler(MouseMoveEvent);
				}
			}
			base.Dispose();
		}
		#endregion
	}
    internal class VS2012DragControlConsts
    {
        #region Image names

        public static string DEF_IMAGES_PATH = "FrameworkComponents.DockingWindows.Images.VS2012DragProvider.";
        public static string DEF_BOTTOM_TARGET_IMAGE_NAME = "BottomArrow.bmp";
        public static string DEF_BOTTOM_TARGET_IMAGE_HIGHLIGHT = "BottomArrowHighlight.bmp";
        public static string DEF_TOP_TARGET_IMAGE_NAME = "TopArrow.bmp";
        public static string DEF_TOP_TARGET_IMAGE_HIGHLIGHT = "TopArrowHighlight.bmp";
        public static string DEF_LEFT_TARGET_IMAGE_NAME = "LeftArrow.bmp";
        public static string DEF_LEFT_TARGET_IMAGE_HIGHLIGHT = "LeftArrowHighlight.bmp";
        public static string DEF_RIGHT_TARGET_IMAGE_NAME = "RightArrow.bmp";
        public static string DEF_RIGHT_TARGET_IMAGE_HIGHLIGHT = "RightArrowHighlight.bmp";
        public static string DEF_INNER_TARGET_IMAGE_NAME = "Background.bmp";
        public static string DEF_BOTTOM_INNER_TARGET_IMAGE_NAME = "BottomInnerArrow.bmp";
        public static string DEF_BOTTOM_INNER_TARGET_IMAGE_HIGHLIGHT = "BottomInnerArrowHighlight.bmp";
        public static string DEF_TOP_INNER_TARGET_IMAGE_NAME = "TopInnerArrow.bmp";
        public static string DEF_TOP_INNER_TARGET_IMAGE_HIGHLIGHT = "TopInnerArrowHighlight.bmp";
        public static string DEF_LEFT_INNER_TARGET_IMAGE_NAME = "LeftInnerArrow.bmp";
        public static string DEF_LEFT_INNER_TARGET_IMAGE_HIGHLIGHT = "LeftInnerArrowHighlight.bmp";
        public static string DEF_RIGHT_INNER_TARGET_IMAGE_NAME = "RightInnerArrow.bmp";
        public static string DEF_RIGHT_INNER_TARGET_IMAGE_HIGHLIGHT = "RightInnerArrowHighlight.bmp";
        public static string DEF_TAB_INNER_TARGET_IMAGE_NAME = "TabInner.bmp";
        public static string DEF_TAB_INNER_TARGET_IMAGE_HIGHLIGHT = "TabInnerHighlight.bmp";

        #endregion

        #region Image indexes
        public static int DEF_LEFT_INACTIVE_TARGET = 0;
        public static int DEF_TOP_INACTIVE_TARGET = 1;
        public static int DEF_RIGHT_INACTIVE_TARGET = 2;
        public static int DEF_BOTTOM_INACTIVE_TARGET = 3;
        public static int DEF_INACTIVE_TAB = 4;
        public static int DEF_INNER_TARGET = 5;
        public static int DEF_LEFT_INNER_INACTIVE_TARGET = 6;
        public static int DEF_TOP_INNER_INACTIVE_TARGET = 7;
        public static int DEF_RIGHT_INNER_INACTIVE_TARGET = 8;
        public static int DEF_BOTTOM_INNER_INACTIVE_TARGET = 9;
        public static int DEF_LEFT_ACTIVE_TARGET = 10;
        public static int DEF_TOP_ACTIVE_TARGET = 11;
        public static int DEF_RIGHT_ACTIVE_TARGET = 12;
        public static int DEF_BOTTOM_ACTIVE_TARGET = 13;
        public static int DEF_ACTIVE_TAB = 14;
        public static int DEF_LEFT_INNER_ACTIVE_TARGET = 15;
        public static int DEF_TOP_INNER_ACTIVE_TARGET = 16;
        public static int DEF_RIGHT_INNER_ACTIVE_TARGET = 17;
        public static int DEF_BOTTOM_INNER_ACTIVE_TARGET = 18;
        #endregion
    }
    /// <summary>
    /// InnerDragControl fo Whidbey drag provider.
    /// </summary>
    ///  /// <summary>
    /// WhidbeyDragTargetControl.
    /// </summary>
    /// /// <summary>
    /// Drag control for Whidbey drag provider style.
    /// </summary>
    internal class VS2012DragControl : DragControl
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Constructor.
        /// </summary>
        public VS2012DragControl()
        {
            m_OuterControl = new VS2012OuterDragControl(this);
            m_OuterControl.OnMouseEnter += new MouseEnterHandler(MouseEnterEvent);
            m_OuterControl.OnMouseLeave += new MouseLeaveHandler(MouseLeaveEvent);
            m_OuterControl.OnMouseUp += new MouseUpHandler(MouseUpEvent);
            m_OuterControl.OnMouseMove += new MouseMoveHandler(OuterMouseMoveEvent);

            m_InnerControl = new VS2012InnerDragControl(this);
            m_InnerControl.OnMouseEnter += new MouseEnterHandler(MouseEnterEvent);
            m_InnerControl.OnMouseLeave += new MouseLeaveHandler(MouseLeaveEvent);
            m_InnerControl.OnMouseUp += new MouseUpHandler(MouseUpEvent);
            m_InnerControl.OnAllowHighlight += new AllowHighlightHandler(AllowHighlightEvent);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Loads images.
        /// </summary>
        protected override void InitializeImages()
        {
            string path = VS2012DragControlConsts.DEF_IMAGES_PATH;
            int index = 0;
            Bitmap bitmap = null;
            bitmap = (Bitmap)ImageLoader.Load(path + VS2012DragControlConsts.DEF_LEFT_TARGET_IMAGE_NAME);
            m_Images[index++] = new Bitmap(bitmap);
            bitmap = (Bitmap)ImageLoader.Load(path + VS2012DragControlConsts.DEF_TOP_TARGET_IMAGE_NAME);
            m_Images[index++] = new Bitmap(bitmap);
            bitmap = (Bitmap)ImageLoader.Load(path + VS2012DragControlConsts.DEF_RIGHT_TARGET_IMAGE_NAME);
            m_Images[index++] = new Bitmap(bitmap);
            bitmap = (Bitmap)ImageLoader.Load(path + VS2012DragControlConsts.DEF_BOTTOM_TARGET_IMAGE_NAME);
            m_Images[index++] = new Bitmap(bitmap);
            bitmap = (Bitmap)ImageLoader.Load(path + VS2012DragControlConsts.DEF_INNER_TARGET_IMAGE_NAME);
            bitmap.MakeTransparent(Color.FromArgb(0, 255, 0));
            m_Images[index++] = new Bitmap(bitmap);
            bitmap = (Bitmap)ImageLoader.Load(path + VS2012DragControlConsts.DEF_LEFT_INNER_TARGET_IMAGE_NAME);
            m_Images[index++] = new Bitmap(bitmap);
            bitmap = (Bitmap)ImageLoader.Load(path + VS2012DragControlConsts.DEF_TOP_INNER_TARGET_IMAGE_NAME);
            m_Images[index++] = new Bitmap(bitmap);
            bitmap = (Bitmap)ImageLoader.Load(path + VS2012DragControlConsts.DEF_RIGHT_INNER_TARGET_IMAGE_NAME);
            m_Images[index++] = new Bitmap(bitmap);
            bitmap = (Bitmap)ImageLoader.Load(path + VS2012DragControlConsts.DEF_BOTTOM_INNER_TARGET_IMAGE_NAME);
            m_Images[index++] = new Bitmap(bitmap);
            bitmap = (Bitmap)ImageLoader.Load(path + VS2012DragControlConsts.DEF_TAB_INNER_TARGET_IMAGE_NAME);
            m_Images[index++] = new Bitmap(bitmap);
        }

        public override void Dispose()
        {
            if (m_OuterControl != null)
            {
                m_OuterControl.OnMouseEnter -= new MouseEnterHandler(MouseEnterEvent);
                m_OuterControl.OnMouseLeave -= new MouseLeaveHandler(MouseLeaveEvent);
                m_OuterControl.OnMouseUp -= new MouseUpHandler(MouseUpEvent);
                m_OuterControl.OnMouseMove -= new MouseMoveHandler(OuterMouseMoveEvent);
                m_OuterControl.Dispose();
                m_OuterControl = null;
            }

            if (m_InnerControl != null)
            {
                m_InnerControl.OnMouseEnter -= new MouseEnterHandler(MouseEnterEvent);
                m_InnerControl.OnMouseLeave -= new MouseLeaveHandler(MouseLeaveEvent);
                m_InnerControl.OnMouseUp -= new MouseUpHandler(MouseUpEvent);
                m_InnerControl.OnAllowHighlight -= new AllowHighlightHandler(AllowHighlightEvent);
                m_InnerControl.Dispose();
                m_InnerControl = null;
            }

            base.Dispose();
        }
        #endregion
    }
    internal class VS2012DragTargetControl
        : DragTargetControl
    {
        #region Class constants
        /// <summary>
        /// Defines constatn hilight color.
        /// </summary>
        private static readonly Color DEF_HILIGHT_COLOR = Color.FromArgb(65, 112, 202);
        #endregion

        #region Class overrides
        /// <summary>
        /// Overrider for its special hilight behaviour.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnPaint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            e.Graphics.DrawImage(inactiveImage, m_internalControl.ClientRectangle);
            if (m_mouseIn)
            {
                Rectangle hilightRectangle = new Rectangle(0, 0, inactiveImage.Width - 1, inactiveImage.Height - 1);
                e.Graphics.DrawRectangle(new Pen(DEF_HILIGHT_COLOR), hilightRectangle);
            }
        }

        public override void HideControl()
        {
            if (m_mouseIn)
            {
                RaiseMouseLeaveEvent();
                m_mouseIn = false;
            }

            base.HideControl();
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Get current image.
        /// </summary>
        public override Image CurrentImage
        {
            get
            {
                return inactiveImage;
            }
        }
        #endregion
    }
    internal class VS2012InnerDragControl
        : DragInnerControl
        , IArrowDragControl
    {
        #region Class members
        private Size m_ControlSize = new Size(131, 131);
        private Bitmap m_innerCross;
        private Point m_crossLocation;
        private Size m_tabSize = new Size(35, 35);
        private Point[] m_hilightPath = null;
        private Size m_hitSize = new Size(35, 35);
        private bool m_bTabbedSelection = false;
        #endregion

        #region Class events
        public new event MouseEnterHandler OnMouseEnter;
        public new event MouseLeaveHandler OnMouseLeave;
        public new event AllowHighlightHandler OnAllowHighlight;
        #endregion

        #region Class constants
        private const int DEF_LEFT_TARGET = 0;
        private const int DEF_TOP_TARGET = 1;
        private const int DEF_RIGHT_TARGET = 2;
        private const int DEF_BOTTOM_TARGET = 3;
        private const int DEF_TAB_TARGET = 4;
        private const int DEF_INNER_TARGET_START_INDEX = 5;
        private const int DEF_BORDER_WIDTH = 1;
        private static readonly Color DEF_SELECT_COLOR = Color.FromArgb(65, 112, 202);
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parentControl">Parent controller</param>
        public VS2012InnerDragControl(DragControl parentControl)
            : base(parentControl)
        {
            m_ParentControl = parentControl;

            CalculateControlsPosition();
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Initializes components.
        /// </summary>
        protected override void InitializeControl()
        {
            m_internalControl = new DragTargetForm();
            m_internalControl.ShowInTaskbar = false;
            m_internalControl.FormBorderStyle = FormBorderStyle.None;
            m_internalControl.TransparencyKey = m_internalControl.BackColor;
            m_internalControl.StartPosition = FormStartPosition.Manual;
            m_internalControl.TopMost = true;
            m_internalControl.Paint += new PaintEventHandler(OnPaint);

            m_innerCross = m_ParentControl.GetImage(4);
           
        }
        /// <summary>
        /// Calculates hit areas.
        /// </summary>
        protected override void CalculateControlsPosition()
        {
            m_Center.X = (m_ControlSize.Width + 1) / 2;
            m_Center.Y = (m_ControlSize.Height + 1) / 2;
            Size crossSize = m_innerCross.Size;
            m_crossLocation = new Point(Center.X - (crossSize.Width) / 2,
                Center.Y - (crossSize.Height) / 2);

            m_Rectangles[DEF_TAB_TARGET] = new Rectangle(
                m_Center.X - (m_tabSize.Width + 1) / 2,
                m_Center.Y - (m_tabSize.Height + 1) / 2,
                m_tabSize.Width, m_tabSize.Height);

            m_Rectangles[DEF_LEFT_TARGET] = new Rectangle(
                m_TabRectangle.X - m_hitSize.Width - 5,
                m_TabRectangle.Y,
                m_hitSize.Width, m_hitSize.Height);

            m_Rectangles[DEF_TOP_TARGET] = new Rectangle(
                m_TabRectangle.X,
                m_TabRectangle.Y - m_hitSize.Height - 5,
                m_hitSize.Width, m_hitSize.Height);

            m_Rectangles[DEF_RIGHT_TARGET] = new Rectangle(
                m_TabRectangle.X + m_tabSize.Width + 5,
                m_TabRectangle.Y,
                m_hitSize.Width, m_hitSize.Height);

            m_Rectangles[DEF_BOTTOM_TARGET] = new Rectangle(
                m_TabRectangle.X,
                m_TabRectangle.Y + m_tabSize.Height + 5,
                m_hitSize.Width, m_hitSize.Height);
        }
        /// <summary>
        /// Paints control area.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnPaint(object sender, PaintEventArgs e)
        {
            if (DockAbility == DockAbility.None)
            {
                return;
            }

            Graphics graphics = e.Graphics;
            Point center = Center;
            Size crossSize = m_innerCross.Size;
            m_crossLocation = new Point(center.X -((crossSize.Width-2) / 2),
                center.Y - ((crossSize.Height-2) / 2));
            using (Brush br = new SolidBrush(Color.Transparent))
            graphics.FillRectangle(br, new Rectangle(m_crossLocation, crossSize));

            Pen borderPen = new Pen(DEF_SELECT_COLOR, DEF_BORDER_WIDTH);
            Bitmap bmp = new Bitmap(typeof(DockingManager).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.FrameworkComponents.DockingWindows.Images.VS2012DragProvider.Background.png"));
            graphics.DrawImage(bmp, 2, 2);

            if ((DockAbility | DockAbility.Left) == DockAbility)
            {
                graphics.DrawImage(m_ParentControl.GetImage(DEF_INNER_TARGET_START_INDEX + DEF_LEFT_TARGET),
                    m_Rectangles[DEF_LEFT_TARGET]);
            }
            if ((DockAbility | DockAbility.Top) == DockAbility)
            {
                graphics.DrawImage(m_ParentControl.GetImage(1),
                    m_Rectangles[DEF_TOP_TARGET]);
            }
            if ((DockAbility | DockAbility.Right) == DockAbility)
            {
                graphics.DrawImage(m_ParentControl.GetImage(2),
                    m_Rectangles[DEF_RIGHT_TARGET]);
            }
            if ((DockAbility | DockAbility.Bottom) == DockAbility)
            {
                graphics.DrawImage(m_ParentControl.GetImage(3),
                    m_Rectangles[DEF_BOTTOM_TARGET]);
            }
            if ((DockAbility | DockAbility.Tabbed) == DockAbility)
            {
                graphics.DrawImage(m_ParentControl.GetImage(DEF_INNER_TARGET_START_INDEX + DEF_TAB_TARGET),
                    m_Rectangles[DEF_TAB_TARGET]);
            }

            if (m_ActiveControlIndex != -1 && m_hilightPath != null)
            {
                if (m_bTabbedSelection)
                {
                    //graphics.DrawLine(borderPen, m_hilightPath[1], m_hilightPath[2]);
                    //graphics.DrawLine(borderPen, m_hilightPath[3], m_hilightPath[4]);
                    //graphics.DrawLine(borderPen, m_hilightPath[5], m_hilightPath[6]);
                    //graphics.DrawLine(borderPen, m_hilightPath[7], m_hilightPath[0]);
                }
                else
                {
                   // graphics.DrawLines(borderPen, m_hilightPath);
                }
            }
        }

        protected bool IsControlEnabled(int index)
        {
            switch (index)
            {
                case 0:
                    if ((DockAbility | DockAbility.Left) == DockAbility)
                    {
                        return true;
                    }
                    break;
                case 1:
                    if ((DockAbility | DockAbility.Top) == DockAbility)
                    {
                        return true;
                    }
                    break;
                case 2:
                    if ((DockAbility | DockAbility.Right) == DockAbility)
                    {
                        return true;
                    }
                    break;
                case 3:
                    if ((DockAbility | DockAbility.Bottom) == DockAbility)
                    {
                        return true;
                    }
                    break;
                case 4:
                    if ((DockAbility | DockAbility.Tabbed) == DockAbility)
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// Processes mouse move.
        /// </summary>
        /// <param name="point">Mouse position.</param>
        /// <returns>if processed.</returns>
        public override bool ProcessMouseMove(Point point)
        {
            if (!this.Visible)
                return false;

            bool result = false;
            point = m_internalControl.PointToClient(point);
            int activeControlIndex = -1;
            for (int i = 0; i < DragControlConsts.DEF_INNER_CONTROL_COUNT; i++)
            {
                if (m_Rectangles[i].Contains(point))
                {
                    point.X -= m_Rectangles[i].Location.X;
                    point.Y -= m_Rectangles[i].Location.Y;
                    if (IsControlEnabled(i))
                    {
                        activeControlIndex = i;
                        result = true;
                        break;
                    }
                }
            }

            if (m_ActiveControlIndex != activeControlIndex)
            {
                m_hilightPath = null;
                if (m_ActiveControlIndex != -1)
                {
                    if (this.OnMouseLeave != null)
                    {
                        string currentTargetStr = Enum.GetName(DragTarget.GetType(), m_ActiveControlIndex);
                        DragTarget currentDragTarget = (DragTarget)Enum.Parse(DragTarget.GetType(), currentTargetStr);
                        OnMouseLeave(this, new DragControlEventArgs(currentDragTarget, ControllerRect));
                    }
                    m_internalControl.Invalidate();
                }

                bool allowHighlight = false;
                string targetStr = Enum.GetName(DragTarget.GetType(), activeControlIndex);
                DragTarget dragTarget = (DragTarget)Enum.Parse(DragTarget.GetType(), targetStr);
                if (OnAllowHighlight != null)
                {
                    AllowHighlightEventArgs args = new AllowHighlightEventArgs(dragTarget);
                    OnAllowHighlight(this, args);
                    allowHighlight = args.AllowHighlight;
                }

                if (allowHighlight)
                {
                    if (activeControlIndex != -1)
                    {
                        m_hilightPath = GenerateGraphicsPath(activeControlIndex);

                        if (OnMouseEnter != null)
                        {
                            OnMouseEnter(this, new DragControlEventArgs(dragTarget, ControllerRect));
                        }
                        m_internalControl.Invalidate();
                    }
                    m_ActiveControlIndex = activeControlIndex;
                }
                else
                    m_ActiveControlIndex = -1;

            }

            return result;
        }

        public override void Dispose()
        {
            base.Dispose();

            if (m_internalControl != null)
            {
                m_internalControl.Paint -= new PaintEventHandler(OnPaint);
                m_internalControl.Dispose();
                m_internalControl = null;
            }
            m_innerCross.Dispose();
            m_innerCross = null;
        }

        #endregion

        #region Class helper methods
        /// <summary>
        /// Generates hilighting graphics path .
        /// </summary>
        /// <param name="controlIndex">index of docking(top, left...)</param>
        /// <returns>Graphics path.</returns>
        protected virtual Point[] GenerateGraphicsPath(int controlIndex)
        {
            Point[] hilightPath;
            Size hilightSize = new Size(22, 28);
            m_bTabbedSelection = false;

            switch (controlIndex)
            {
                case DEF_LEFT_TARGET:
                    hilightPath = new Point[4];
                    hilightPath[2] = new Point(m_LeftRectangle.X, m_LeftRectangle.Y);
                    hilightPath[3] = new Point(m_LeftRectangle.X + hilightSize.Width, hilightPath[2].Y);
                    hilightPath[1] = new Point(m_LeftRectangle.X, hilightPath[2].Y + hilightSize.Height);
                    hilightPath[0] = new Point(hilightPath[3].X, hilightPath[1].Y);
                    break;

                case DEF_TOP_TARGET:
                    hilightPath = new Point[4];
                    hilightPath[1] = new Point(m_TopRectangle.X, m_TopRectangle.Y);
                    hilightPath[0] = new Point(m_TopRectangle.X, m_TopRectangle.Y + hilightSize.Width);
                    hilightPath[2] = new Point(m_TopRectangle.X + hilightSize.Height, m_TopRectangle.Y);
                    hilightPath[3] = new Point(hilightPath[2].X, hilightPath[0].Y);
                    break;

                case DEF_RIGHT_TARGET:
                    hilightPath = new Point[4];
                    hilightPath[1] = new Point(m_RightRectangle.X + m_RightRectangle.Width, m_RightRectangle.Y);
                    hilightPath[0] = new Point(hilightPath[1].X - hilightSize.Width, m_RightRectangle.Y);
                    hilightPath[2] = new Point(hilightPath[1].X, hilightPath[1].Y + hilightSize.Height);
                    hilightPath[3] = new Point(hilightPath[0].X, hilightPath[2].Y);
                    break;

                case DEF_BOTTOM_TARGET:
                    hilightPath = new Point[4];
                    hilightPath[1] = new Point(m_BottomRectangle.X, m_BottomRectangle.Y + m_BottomRectangle.Height);
                    hilightPath[0] = new Point(m_BottomRectangle.X, hilightPath[1].Y - hilightSize.Width);
                    hilightPath[2] = new Point(hilightPath[1].X + hilightSize.Height, hilightPath[1].Y);
                    hilightPath[3] = new Point(hilightPath[2].X, hilightPath[0].Y);
                    break;

                case DEF_TAB_TARGET:
                    hilightPath = new Point[8];
                    hilightPath[1] = new Point(m_TopRectangle.X, m_TopRectangle.Y + hilightSize.Width);
                    hilightPath[0] = new Point(hilightPath[1].X + hilightSize.Height, hilightPath[1].Y);
                    hilightPath[2] = new Point(m_LeftRectangle.X + hilightSize.Width, m_LeftRectangle.Y);
                    hilightPath[3] = new Point(hilightPath[2].X, m_LeftRectangle.Y + hilightSize.Height);
                    hilightPath[4] = new Point(m_BottomRectangle.X, m_BottomRectangle.Y + m_hitSize.Height - hilightSize.Width - 1);
                    hilightPath[5] = new Point(m_BottomRectangle.X + hilightSize.Height, hilightPath[4].Y);
                    hilightPath[7] = new Point(m_RightRectangle.X + m_hitSize.Width - hilightSize.Width - 2, m_LeftRectangle.Y - 1);
                    hilightPath[6] = new Point(hilightPath[7].X, m_RightRectangle.Y + m_RightRectangle.Height);
                    m_bTabbedSelection = true;
                    break;

                default:
                    hilightPath = null;
                    break;
            }

            return hilightPath;
        }
        #endregion
    }
    /// <summary>
    /// OuterDragControl for Whidbey drag provider.
    /// </summary>
    internal class VS2012OuterDragControl
        : DragOuterControl
        , IArrowDragControl
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="parentControl">Parent Controller. See the <see cref="Syncfusion.Windows.Forms.Tools.DragControl"/></param>
        public VS2012OuterDragControl(DragControl parentControl)
            : base(parentControl)
        { }
        #endregion

        #region Class overrides
        /// <summary>
        /// Initializes provider with specific parameters.
        /// </summary>
        protected override void InitializeProvider()
        {
            for (int i = 0; i < 4; i++)
            {
                VS2012DragTargetControl control = new VS2012DragTargetControl();
                m_Controls.Add(control);
                control.OnMouseEnter += new MouseEnterHandler(MouseEnterEvent);
                control.OnMouseLeave += new MouseLeaveHandler(MouseLeaveEvent);
                control.OnMouseUp += new MouseUpHandler(MouseUpEvent);
                control.OnMouseMove += new MouseMoveHandler(MouseMoveEvent);
                control.ParentControl = this;
            }

            leftControl.DragTarget = DragTarget.OuterLeft;
            leftControl.InactiveImage = GetImage(DragControlConsts.DEF_LEFT_INACTIVE_TARGET);

            topControl.DragTarget = DragTarget.OuterTop;
            topControl.InactiveImage = GetImage(DragControlConsts.DEF_TOP_INACTIVE_TARGET);

            rightControl.DragTarget = DragTarget.OuterRight;
            rightControl.InactiveImage = GetImage(DragControlConsts.DEF_RIGHT_INACTIVE_TARGET);

            bottomControl.DragTarget = DragTarget.OuterBottom;
            bottomControl.InactiveImage = GetImage(DragControlConsts.DEF_BOTTOM_INACTIVE_TARGET);
        }

        public override void Dispose()
        {
            for (int i = 0; i < 4; i++)
            {
                DragTargetControl control = m_Controls[i] as DragTargetControl;
                if (control != null)
                {
                    control.OnMouseMove -= new MouseMoveHandler(MouseMoveEvent);
                }
            }
            base.Dispose();
        }
        #endregion
    }


    #region VS 2010 Drag providere style

    internal class VS2010DragControlConsts
    {
        #region Image names

        public static string DEF_IMAGES_PATH = "FrameworkComponents.DockingWindows.Images.VS2010DragProvider.";
        public static string DEF_BOTTOM_TARGET_IMAGE_NAME = "BottomArrow.bmp";
        public static string DEF_BOTTOM_TARGET_IMAGE_HIGHLIGHT = "BottomArrowHighlight.bmp";
        public static string DEF_TOP_TARGET_IMAGE_NAME = "TopArrow.bmp";
        public static string DEF_TOP_TARGET_IMAGE_HIGHLIGHT = "TopArrowHighlight.bmp";
        public static string DEF_LEFT_TARGET_IMAGE_NAME = "LeftArrow.bmp";
        public static string DEF_LEFT_TARGET_IMAGE_HIGHLIGHT = "LeftArrowHighlight.bmp";
        public static string DEF_RIGHT_TARGET_IMAGE_NAME = "RightArrow.bmp";
        public static string DEF_RIGHT_TARGET_IMAGE_HIGHLIGHT = "RightArrowHighlight.bmp";
        public static string DEF_INNER_TARGET_IMAGE_NAME = "Background.bmp";
        public static string DEF_BOTTOM_INNER_TARGET_IMAGE_NAME = "BottomInnerArrow.bmp";
        public static string DEF_BOTTOM_INNER_TARGET_IMAGE_HIGHLIGHT = "BottomInnerArrowHighlight.bmp";
        public static string DEF_TOP_INNER_TARGET_IMAGE_NAME = "TopInnerArrow.bmp";
        public static string DEF_TOP_INNER_TARGET_IMAGE_HIGHLIGHT = "TopInnerArrowHighlight.bmp";
        public static string DEF_LEFT_INNER_TARGET_IMAGE_NAME = "LeftInnerArrow.bmp";
        public static string DEF_LEFT_INNER_TARGET_IMAGE_HIGHLIGHT = "LeftInnerArrowHighlight.bmp";
        public static string DEF_RIGHT_INNER_TARGET_IMAGE_NAME = "RightInnerArrow.bmp";
        public static string DEF_RIGHT_INNER_TARGET_IMAGE_HIGHLIGHT = "RightInnerArrowHighlight.bmp";
        public static string DEF_TAB_INNER_TARGET_IMAGE_NAME = "TabInner.bmp";
        public static string DEF_TAB_INNER_TARGET_IMAGE_HIGHLIGHT = "TabInnerHighlight.bmp";

        #endregion

        #region Image indexes
        public static int DEF_LEFT_INACTIVE_TARGET = 0;
        public static int DEF_TOP_INACTIVE_TARGET = 1;
        public static int DEF_RIGHT_INACTIVE_TARGET = 2;
        public static int DEF_BOTTOM_INACTIVE_TARGET = 3;
        public static int DEF_INACTIVE_TAB = 4;
        public static int DEF_INNER_TARGET = 5;
        public static int DEF_LEFT_INNER_INACTIVE_TARGET = 6;
        public static int DEF_TOP_INNER_INACTIVE_TARGET = 7;
        public static int DEF_RIGHT_INNER_INACTIVE_TARGET = 8;
        public static int DEF_BOTTOM_INNER_INACTIVE_TARGET = 9;
        public static int DEF_LEFT_ACTIVE_TARGET = 10;
        public static int DEF_TOP_ACTIVE_TARGET = 11;
        public static int DEF_RIGHT_ACTIVE_TARGET = 12;
        public static int DEF_BOTTOM_ACTIVE_TARGET = 13;
        public static int DEF_ACTIVE_TAB = 14;
        public static int DEF_LEFT_INNER_ACTIVE_TARGET = 15;
        public static int DEF_TOP_INNER_ACTIVE_TARGET = 16;
        public static int DEF_RIGHT_INNER_ACTIVE_TARGET = 17;
        public static int DEF_BOTTOM_INNER_ACTIVE_TARGET = 18;
        #endregion
    }
    /// <summary>
    /// InnerDragControl fo Whidbey drag provider.
    /// </summary>
    ///  /// <summary>
    /// WhidbeyDragTargetControl.
    /// </summary>
    /// /// <summary>
    /// Drag control for Whidbey drag provider style.
    /// </summary>
    internal class VS2010DragControl : DragControl
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Constructor.
        /// </summary>
        public VS2010DragControl()
        {
            m_OuterControl = new VS2010OuterDragControl(this);
            m_OuterControl.OnMouseEnter += new MouseEnterHandler(MouseEnterEvent);
            m_OuterControl.OnMouseLeave += new MouseLeaveHandler(MouseLeaveEvent);
            m_OuterControl.OnMouseUp += new MouseUpHandler(MouseUpEvent);
            m_OuterControl.OnMouseMove += new MouseMoveHandler(OuterMouseMoveEvent);

            m_InnerControl = new VS2012InnerDragControl(this);
            m_InnerControl.OnMouseEnter += new MouseEnterHandler(MouseEnterEvent);
            m_InnerControl.OnMouseLeave += new MouseLeaveHandler(MouseLeaveEvent);
            m_InnerControl.OnMouseUp += new MouseUpHandler(MouseUpEvent);
            m_InnerControl.OnAllowHighlight += new AllowHighlightHandler(AllowHighlightEvent);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Loads images.
        /// </summary>
        protected override void InitializeImages()
        {
            string path = VS2010DragControlConsts.DEF_IMAGES_PATH;
            int index = 0;
            Bitmap bitmap = null;
            bitmap = (Bitmap)ImageLoader.Load(path + VS2010DragControlConsts.DEF_LEFT_TARGET_IMAGE_NAME);
            m_Images[index++] = new Bitmap(bitmap);
            bitmap = (Bitmap)ImageLoader.Load(path + VS2010DragControlConsts.DEF_TOP_TARGET_IMAGE_NAME);
            m_Images[index++] = new Bitmap(bitmap);
            bitmap = (Bitmap)ImageLoader.Load(path + VS2010DragControlConsts.DEF_RIGHT_TARGET_IMAGE_NAME);
            m_Images[index++] = new Bitmap(bitmap);
            bitmap = (Bitmap)ImageLoader.Load(path + VS2010DragControlConsts.DEF_BOTTOM_TARGET_IMAGE_NAME);
            m_Images[index++] = new Bitmap(bitmap);
            bitmap = (Bitmap)ImageLoader.Load(path + VS2010DragControlConsts.DEF_INNER_TARGET_IMAGE_NAME);
            bitmap.MakeTransparent(Color.FromArgb(0, 255, 0));
            m_Images[index++] = new Bitmap(bitmap);
            bitmap = (Bitmap)ImageLoader.Load(path + VS2010DragControlConsts.DEF_LEFT_INNER_TARGET_IMAGE_NAME);
            m_Images[index++] = new Bitmap(bitmap);
            bitmap = (Bitmap)ImageLoader.Load(path + VS2010DragControlConsts.DEF_TOP_INNER_TARGET_IMAGE_NAME);
            m_Images[index++] = new Bitmap(bitmap);
            bitmap = (Bitmap)ImageLoader.Load(path + VS2010DragControlConsts.DEF_RIGHT_INNER_TARGET_IMAGE_NAME);
            m_Images[index++] = new Bitmap(bitmap);
            bitmap = (Bitmap)ImageLoader.Load(path + VS2010DragControlConsts.DEF_BOTTOM_INNER_TARGET_IMAGE_NAME);
            m_Images[index++] = new Bitmap(bitmap);
            bitmap = (Bitmap)ImageLoader.Load(path + VS2010DragControlConsts.DEF_TAB_INNER_TARGET_IMAGE_NAME);
            m_Images[index++] = new Bitmap(bitmap);
        }

        public override void Dispose()
        {
            if (m_OuterControl != null)
            {
                m_OuterControl.OnMouseEnter -= new MouseEnterHandler(MouseEnterEvent);
                m_OuterControl.OnMouseLeave -= new MouseLeaveHandler(MouseLeaveEvent);
                m_OuterControl.OnMouseUp -= new MouseUpHandler(MouseUpEvent);
                m_OuterControl.OnMouseMove -= new MouseMoveHandler(OuterMouseMoveEvent);
                m_OuterControl.Dispose();
                m_OuterControl = null;
            }

            if (m_InnerControl != null)
            {
                m_InnerControl.OnMouseEnter -= new MouseEnterHandler(MouseEnterEvent);
                m_InnerControl.OnMouseLeave -= new MouseLeaveHandler(MouseLeaveEvent);
                m_InnerControl.OnMouseUp -= new MouseUpHandler(MouseUpEvent);
                m_InnerControl.OnAllowHighlight -= new AllowHighlightHandler(AllowHighlightEvent);
                m_InnerControl.Dispose();
                m_InnerControl = null;
            }

            base.Dispose();
        }
        #endregion
    }
    internal class VS2010DragTargetControl
        : DragTargetControl
    {
        #region Class constants
        /// <summary>
        /// Defines constatn hilight color.
        /// </summary>
        private static readonly Color DEF_HILIGHT_COLOR = Color.FromArgb(65, 112, 202);
        #endregion

        #region Class overrides
        /// <summary>
        /// Overrider for its special hilight behaviour.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnPaint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            e.Graphics.DrawImage(inactiveImage, m_internalControl.ClientRectangle);
            if (m_mouseIn)
            {
                Rectangle hilightRectangle = new Rectangle(0, 0, inactiveImage.Width - 1, inactiveImage.Height - 1);
                e.Graphics.DrawRectangle(new Pen(DEF_HILIGHT_COLOR), hilightRectangle);
            }
        }

        public override void HideControl()
        {
            if (m_mouseIn)
            {
                RaiseMouseLeaveEvent();
                m_mouseIn = false;
            }

            base.HideControl();
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Get current image.
        /// </summary>
        public override Image CurrentImage
        {
            get
            {
                return inactiveImage;
            }
        }
        #endregion
    }
    internal class VS2010InnerDragControl
        : DragInnerControl
        , IArrowDragControl
    {
        #region Class members
        private Size m_ControlSize = new Size(131, 131);
        private Bitmap m_innerCross;
        private Point m_crossLocation;
        private Size m_tabSize = new Size(35, 35);
        private Point[] m_hilightPath = null;
        private Size m_hitSize = new Size(35, 35);
        private bool m_bTabbedSelection = false;
        #endregion

        #region Class events
        public new event MouseEnterHandler OnMouseEnter;
        public new event MouseLeaveHandler OnMouseLeave;
        public new event AllowHighlightHandler OnAllowHighlight;
        #endregion

        #region Class constants
        private const int DEF_LEFT_TARGET = 0;
        private const int DEF_TOP_TARGET = 1;
        private const int DEF_RIGHT_TARGET = 2;
        private const int DEF_BOTTOM_TARGET = 3;
        private const int DEF_TAB_TARGET = 4;
        private const int DEF_INNER_TARGET_START_INDEX = 5;
        private const int DEF_BORDER_WIDTH = 1;
        private static readonly Color DEF_SELECT_COLOR = Color.FromArgb(65, 112, 202);
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parentControl">Parent controller</param>
        public VS2010InnerDragControl(DragControl parentControl)
            : base(parentControl)
        {
            m_ParentControl = parentControl;

            CalculateControlsPosition();
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Initializes components.
        /// </summary>
        protected override void InitializeControl()
        {
            m_internalControl = new DragTargetForm();
            m_internalControl.ShowInTaskbar = false;
            m_internalControl.FormBorderStyle = FormBorderStyle.None;
            m_internalControl.TransparencyKey = m_internalControl.BackColor;
            m_internalControl.StartPosition = FormStartPosition.Manual;
            m_internalControl.TopMost = true;
            m_internalControl.Paint += new PaintEventHandler(OnPaint);

            m_innerCross = m_ParentControl.GetImage(4);

        }
        /// <summary>
        /// Calculates hit areas.
        /// </summary>
        protected override void CalculateControlsPosition()
        {
            m_Center.X = (m_ControlSize.Width + 1) / 2;
            m_Center.Y = (m_ControlSize.Height + 1) / 2;
            Size crossSize = m_innerCross.Size;
            m_crossLocation = new Point(Center.X - (crossSize.Width) / 2,
                Center.Y - (crossSize.Height) / 2);

            m_Rectangles[DEF_TAB_TARGET] = new Rectangle(
                m_Center.X - (m_tabSize.Width + 1) / 2,
                m_Center.Y - (m_tabSize.Height + 1) / 2,
                m_tabSize.Width, m_tabSize.Height);

            m_Rectangles[DEF_LEFT_TARGET] = new Rectangle(
                m_TabRectangle.X - m_hitSize.Width - 5,
                m_TabRectangle.Y,
                m_hitSize.Width, m_hitSize.Height);

            m_Rectangles[DEF_TOP_TARGET] = new Rectangle(
                m_TabRectangle.X,
                m_TabRectangle.Y - m_hitSize.Height - 5,
                m_hitSize.Width, m_hitSize.Height);

            m_Rectangles[DEF_RIGHT_TARGET] = new Rectangle(
                m_TabRectangle.X + m_tabSize.Width + 5,
                m_TabRectangle.Y,
                m_hitSize.Width, m_hitSize.Height);

            m_Rectangles[DEF_BOTTOM_TARGET] = new Rectangle(
                m_TabRectangle.X,
                m_TabRectangle.Y + m_tabSize.Height + 5,
                m_hitSize.Width, m_hitSize.Height);
        }
        /// <summary>
        /// Paints control area.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnPaint(object sender, PaintEventArgs e)
        {
            if (DockAbility == DockAbility.None)
            {
                return;
            }

            Graphics graphics = e.Graphics;
            Point center = Center;
            Size crossSize = m_innerCross.Size;
            m_crossLocation = new Point(center.X - ((crossSize.Width - 2) / 2),
                center.Y - ((crossSize.Height - 2) / 2));
            using (Brush br = new SolidBrush(Color.Transparent))
                graphics.FillRectangle(br, new Rectangle(m_crossLocation, crossSize));

            Pen borderPen = new Pen(DEF_SELECT_COLOR, DEF_BORDER_WIDTH);
            Bitmap bmp = new Bitmap(typeof(DockingManager).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.FrameworkComponents.DockingWindows.Images.VS2010DragProvider.Background.png"));
            graphics.DrawImage(bmp, 2, 2);

            if ((DockAbility | DockAbility.Left) == DockAbility)
            {
                graphics.DrawImage(m_ParentControl.GetImage(DEF_INNER_TARGET_START_INDEX + DEF_LEFT_TARGET),
                    m_Rectangles[DEF_LEFT_TARGET]);
            }
            if ((DockAbility | DockAbility.Top) == DockAbility)
            {
                graphics.DrawImage(m_ParentControl.GetImage(1),
                    m_Rectangles[DEF_TOP_TARGET]);
            }
            if ((DockAbility | DockAbility.Right) == DockAbility)
            {
                graphics.DrawImage(m_ParentControl.GetImage(2),
                    m_Rectangles[DEF_RIGHT_TARGET]);
            }
            if ((DockAbility | DockAbility.Bottom) == DockAbility)
            {
                graphics.DrawImage(m_ParentControl.GetImage(3),
                    m_Rectangles[DEF_BOTTOM_TARGET]);
            }
            if ((DockAbility | DockAbility.Tabbed) == DockAbility)
            {
                graphics.DrawImage(m_ParentControl.GetImage(DEF_INNER_TARGET_START_INDEX + DEF_TAB_TARGET),
                    m_Rectangles[DEF_TAB_TARGET]);
            }

            if (m_ActiveControlIndex != -1 && m_hilightPath != null)
            {
                if (m_bTabbedSelection)
                {
                    //graphics.DrawLine(borderPen, m_hilightPath[1], m_hilightPath[2]);
                    //graphics.DrawLine(borderPen, m_hilightPath[3], m_hilightPath[4]);
                    //graphics.DrawLine(borderPen, m_hilightPath[5], m_hilightPath[6]);
                    //graphics.DrawLine(borderPen, m_hilightPath[7], m_hilightPath[0]);
                }
                else
                {
                    // graphics.DrawLines(borderPen, m_hilightPath);
                }
            }
        }

        protected bool IsControlEnabled(int index)
        {
            switch (index)
            {
                case 0:
                    if ((DockAbility | DockAbility.Left) == DockAbility)
                    {
                        return true;
                    }
                    break;
                case 1:
                    if ((DockAbility | DockAbility.Top) == DockAbility)
                    {
                        return true;
                    }
                    break;
                case 2:
                    if ((DockAbility | DockAbility.Right) == DockAbility)
                    {
                        return true;
                    }
                    break;
                case 3:
                    if ((DockAbility | DockAbility.Bottom) == DockAbility)
                    {
                        return true;
                    }
                    break;
                case 4:
                    if ((DockAbility | DockAbility.Tabbed) == DockAbility)
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// Processes mouse move.
        /// </summary>
        /// <param name="point">Mouse position.</param>
        /// <returns>if processed.</returns>
        public override bool ProcessMouseMove(Point point)
        {
            if (!this.Visible)
                return false;

            bool result = false;
            point = m_internalControl.PointToClient(point);
            int activeControlIndex = -1;
            for (int i = 0; i < DragControlConsts.DEF_INNER_CONTROL_COUNT; i++)
            {
                if (m_Rectangles[i].Contains(point))
                {
                    point.X -= m_Rectangles[i].Location.X;
                    point.Y -= m_Rectangles[i].Location.Y;
                    if (IsControlEnabled(i))
                    {
                        activeControlIndex = i;
                        result = true;
                        break;
                    }
                }
            }

            if (m_ActiveControlIndex != activeControlIndex)
            {
                m_hilightPath = null;
                if (m_ActiveControlIndex != -1)
                {
                    if (this.OnMouseLeave != null)
                    {
                        string currentTargetStr = Enum.GetName(DragTarget.GetType(), m_ActiveControlIndex);
                        DragTarget currentDragTarget = (DragTarget)Enum.Parse(DragTarget.GetType(), currentTargetStr);
                        OnMouseLeave(this, new DragControlEventArgs(currentDragTarget, ControllerRect));
                    }
                    m_internalControl.Invalidate();
                }

                bool allowHighlight = false;
                string targetStr = Enum.GetName(DragTarget.GetType(), activeControlIndex);
                DragTarget dragTarget = (DragTarget)Enum.Parse(DragTarget.GetType(), targetStr);
                if (OnAllowHighlight != null)
                {
                    AllowHighlightEventArgs args = new AllowHighlightEventArgs(dragTarget);
                    OnAllowHighlight(this, args);
                    allowHighlight = args.AllowHighlight;
                }

                if (allowHighlight)
                {
                    if (activeControlIndex != -1)
                    {
                        m_hilightPath = GenerateGraphicsPath(activeControlIndex);

                        if (OnMouseEnter != null)
                        {
                            OnMouseEnter(this, new DragControlEventArgs(dragTarget, ControllerRect));
                        }
                        m_internalControl.Invalidate();
                    }
                    m_ActiveControlIndex = activeControlIndex;
                }
                else
                    m_ActiveControlIndex = -1;

            }

            return result;
        }

        public override void Dispose()
        {
            base.Dispose();

            if (m_internalControl != null)
            {
                m_internalControl.Paint -= new PaintEventHandler(OnPaint);
                m_internalControl.Dispose();
                m_internalControl = null;
            }
            m_innerCross.Dispose();
            m_innerCross = null;
        }

        #endregion

        #region Class helper methods
        /// <summary>
        /// Generates hilighting graphics path .
        /// </summary>
        /// <param name="controlIndex">index of docking(top, left...)</param>
        /// <returns>Graphics path.</returns>
        protected virtual Point[] GenerateGraphicsPath(int controlIndex)
        {
            Point[] hilightPath;
            Size hilightSize = new Size(22, 28);
            m_bTabbedSelection = false;

            switch (controlIndex)
            {
                case DEF_LEFT_TARGET:
                    hilightPath = new Point[4];
                    hilightPath[2] = new Point(m_LeftRectangle.X, m_LeftRectangle.Y);
                    hilightPath[3] = new Point(m_LeftRectangle.X + hilightSize.Width, hilightPath[2].Y);
                    hilightPath[1] = new Point(m_LeftRectangle.X, hilightPath[2].Y + hilightSize.Height);
                    hilightPath[0] = new Point(hilightPath[3].X, hilightPath[1].Y);
                    break;

                case DEF_TOP_TARGET:
                    hilightPath = new Point[4];
                    hilightPath[1] = new Point(m_TopRectangle.X, m_TopRectangle.Y);
                    hilightPath[0] = new Point(m_TopRectangle.X, m_TopRectangle.Y + hilightSize.Width);
                    hilightPath[2] = new Point(m_TopRectangle.X + hilightSize.Height, m_TopRectangle.Y);
                    hilightPath[3] = new Point(hilightPath[2].X, hilightPath[0].Y);
                    break;

                case DEF_RIGHT_TARGET:
                    hilightPath = new Point[4];
                    hilightPath[1] = new Point(m_RightRectangle.X + m_RightRectangle.Width, m_RightRectangle.Y);
                    hilightPath[0] = new Point(hilightPath[1].X - hilightSize.Width, m_RightRectangle.Y);
                    hilightPath[2] = new Point(hilightPath[1].X, hilightPath[1].Y + hilightSize.Height);
                    hilightPath[3] = new Point(hilightPath[0].X, hilightPath[2].Y);
                    break;

                case DEF_BOTTOM_TARGET:
                    hilightPath = new Point[4];
                    hilightPath[1] = new Point(m_BottomRectangle.X, m_BottomRectangle.Y + m_BottomRectangle.Height);
                    hilightPath[0] = new Point(m_BottomRectangle.X, hilightPath[1].Y - hilightSize.Width);
                    hilightPath[2] = new Point(hilightPath[1].X + hilightSize.Height, hilightPath[1].Y);
                    hilightPath[3] = new Point(hilightPath[2].X, hilightPath[0].Y);
                    break;

                case DEF_TAB_TARGET:
                    hilightPath = new Point[8];
                    hilightPath[1] = new Point(m_TopRectangle.X, m_TopRectangle.Y + hilightSize.Width);
                    hilightPath[0] = new Point(hilightPath[1].X + hilightSize.Height, hilightPath[1].Y);
                    hilightPath[2] = new Point(m_LeftRectangle.X + hilightSize.Width, m_LeftRectangle.Y);
                    hilightPath[3] = new Point(hilightPath[2].X, m_LeftRectangle.Y + hilightSize.Height);
                    hilightPath[4] = new Point(m_BottomRectangle.X, m_BottomRectangle.Y + m_hitSize.Height - hilightSize.Width - 1);
                    hilightPath[5] = new Point(m_BottomRectangle.X + hilightSize.Height, hilightPath[4].Y);
                    hilightPath[7] = new Point(m_RightRectangle.X + m_hitSize.Width - hilightSize.Width - 2, m_LeftRectangle.Y - 1);
                    hilightPath[6] = new Point(hilightPath[7].X, m_RightRectangle.Y + m_RightRectangle.Height);
                    m_bTabbedSelection = true;
                    break;

                default:
                    hilightPath = null;
                    break;
            }

            return hilightPath;
        }
        #endregion
    }
    /// <summary>
    /// OuterDragControl for Whidbey drag provider.
    /// </summary>
    internal class VS2010OuterDragControl
        : DragOuterControl
        , IArrowDragControl
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="parentControl">Parent Controller. See the <see cref="Syncfusion.Windows.Forms.Tools.DragControl"/></param>
        public VS2010OuterDragControl(DragControl parentControl)
            : base(parentControl)
        { }
        #endregion

        #region Class overrides
        /// <summary>
        /// Initializes provider with specific parameters.
        /// </summary>
        protected override void InitializeProvider()
        {
            for (int i = 0; i < 4; i++)
            {
                VS2010DragTargetControl control = new VS2010DragTargetControl();
                m_Controls.Add(control);
                control.OnMouseEnter += new MouseEnterHandler(MouseEnterEvent);
                control.OnMouseLeave += new MouseLeaveHandler(MouseLeaveEvent);
                control.OnMouseUp += new MouseUpHandler(MouseUpEvent);
                control.OnMouseMove += new MouseMoveHandler(MouseMoveEvent);
                control.ParentControl = this;
            }

            leftControl.DragTarget = DragTarget.OuterLeft;
            leftControl.InactiveImage = GetImage(DragControlConsts.DEF_LEFT_INACTIVE_TARGET);

            topControl.DragTarget = DragTarget.OuterTop;
            topControl.InactiveImage = GetImage(DragControlConsts.DEF_TOP_INACTIVE_TARGET);

            rightControl.DragTarget = DragTarget.OuterRight;
            rightControl.InactiveImage = GetImage(DragControlConsts.DEF_RIGHT_INACTIVE_TARGET);

            bottomControl.DragTarget = DragTarget.OuterBottom;
            bottomControl.InactiveImage = GetImage(DragControlConsts.DEF_BOTTOM_INACTIVE_TARGET);
        }

        public override void Dispose()
        {
            for (int i = 0; i < 4; i++)
            {
                DragTargetControl control = m_Controls[i] as DragTargetControl;
                if (control != null)
                {
                    control.OnMouseMove -= new MouseMoveHandler(MouseMoveEvent);
                }
            }
            base.Dispose();
        }
        #endregion
    }





    #endregion // VS 2010 Drag provider Style

    /// <summary>
	/// VS2008DragProvider is used to provide docking in VS2008 style.
	/// </summary>
	internal class VS2008DragProvider
		: WhidbeyDragProvider
	{
		#region Class members
		private Color m_targetBorderColor = Color.Empty;
		private Color m_targetBackColor = Color.Empty;
		private readonly Color TARGET_FORM_COLOR = Color.FromArgb( 21, 132, 255 );
		private const int DEF_BORDER_WIDTH = 3;
		private const double DEF_TARGET_OPACITY = 0.3;
		#endregion

		#region class initialize/finalize methods
		/// <summary>
		/// Class constructor.
		/// </summary>
		/// <param name="manager">Parent DockingManager class.</param>
		public VS2008DragProvider( DockingManager manager )
			: base( manager )
		{}
		#endregion

		#region class overrides
		/// <summary>
		/// Initializes DragProvider class.
		/// </summary>
		protected override void InitializeProvider()
		{
			this.dragControl = new VS2008DragControl();

			dragControl.OnMouseEnter += new MouseEnterHandler( dragControl_OnMouseEnter );
			dragControl.OnMouseLeave += new MouseLeaveHandler( dragControl_OnMouseLeave );
			dragControl.OnMouseUp += new MouseUpHandler( dragControl_OnMouseUp );
			dragControl.OnAllowHighlight += new AllowHighlightHandler( dragControl_OnAllowHighlight );

			CreateTargetForm( DockingManager );
		}

		/// <summary>
		/// Refreshes colors used to paint DropTargetForm.
		/// </summary>
		public override void UpdateColors()
		{
			m_targetBackColor = TARGET_FORM_COLOR;
			m_targetBorderColor = SystemColors.ControlDark;
		}

		/// <summary>
		/// Creates target form to point the drop location.
		/// </summary>
		/// <param name="dockingManager">Parent DockingManager</param>
		protected override void CreateTargetForm( DockingManager dockingManager )
		{
			UpdateColors();
			targetForm = new TargetForm( dockingManager );
			targetForm.BackColor = m_targetBackColor;
			targetForm.BorderColor = m_targetBorderColor;
			targetForm.BorderWidth = DEF_BORDER_WIDTH;
			targetForm.Opacity = DEF_TARGET_OPACITY;
		}
		#endregion
	}

	/// <summary>
	/// Constants used by Vs2008 DragProvider
	/// </summary>
	public class VS2008DragControlConsts
	{
		public static string DEF_IMAGES_PATH = "FrameworkComponents.DockingWindows.Images.VS2008DragProvider.";
		public static double DEF_TARGET_OPACITY = 0.7;
	}

	/// <summary>
	/// Event args used to force opacity change.
	/// </summary>
	internal struct FadeEventArgs
	{
		public object Sender;
		public double Opacity;

		public FadeEventArgs( Object evSender, double opacity )
		{
			Sender = evSender;
			Opacity = opacity;
		}
	}

	/// <summary>
	/// Drag control class is used to layout and manage drop arrows.
	/// </summary>
	internal class VS2008DragControl
		: DragControl
	{
		#region class initialize/finalize methods
		public VS2008DragControl()
		{
			m_OuterControl = new VS2008OuterDragControl( this );
			m_OuterControl.OnMouseEnter += new MouseEnterHandler(MouseEnterEvent);
			m_OuterControl.OnMouseLeave += new MouseLeaveHandler(MouseLeaveEvent);
			m_OuterControl.OnMouseUp += new MouseUpHandler(MouseUpEvent);
			m_OuterControl.OnMouseMove += new MouseMoveHandler(OuterMouseMoveEvent);

			m_InnerControl = new VS2008InnerDragControl( this );
			m_InnerControl.OnMouseEnter += new MouseEnterHandler(MouseEnterEvent);
			m_InnerControl.OnMouseLeave += new MouseLeaveHandler(MouseLeaveEvent);
			m_InnerControl.OnMouseUp += new MouseUpHandler(MouseUpEvent);
			m_InnerControl.OnMouseMove += new MouseMoveHandler( OuterMouseMoveEvent );
			m_InnerControl.OnAllowHighlight += new AllowHighlightHandler(AllowHighlightEvent);
		}
		#endregion

		#region class overrides
		public override DockAbility DockAbility
		{
			get
			{
				return m_DockAbility;
			}
			set
			{
				if( m_DockAbility != value )
				{
					bool visible = InnerVisible;

					if( visible )
						this.HideInnerControl();

					this.m_DockAbility = value;
					m_InnerControl.DockAbility = this.m_DockAbility;
					// In order to DockAbility changes take effect, hide and show inner drag control.

					if( visible )
						this.ShowInnerControl();
				}
			}
		}

		public override bool ProcessMouseMove( Point point )
		{
			bool processed = m_OuterControl.ProcessMouseMove( point );
			if( !processed && !( m_OuterControl as VS2008OuterDragControl ).MouseIn )
				processed = m_InnerControl.ProcessMouseMove( point );
			return processed;
		}

		/// <summary>
		/// VS2008 drag provider system uses VS2005 image names.
		/// </summary>
		protected override void InitializeImages()
		{
			int index = 0;
			m_Images = new Image[19];

			LoadImage( VS2005DragControlConsts.DEF_LEFT_TARGET_IMAGE_NAME, index++ );
			LoadImage( VS2005DragControlConsts.DEF_TOP_TARGET_IMAGE_NAME, index++ );
			LoadImage( VS2005DragControlConsts.DEF_RIGHT_TARGET_IMAGE_NAME, index++ );
			LoadImage( VS2005DragControlConsts.DEF_BOTTOM_TARGET_IMAGE_NAME, index++ );
			LoadImage( VS2005DragControlConsts.DEF_TAB_INNER_TARGET_IMAGE_NAME, index++ );
			LoadImage( VS2005DragControlConsts.DEF_INNER_TARGET_IMAGE_NAME, index++ );
			LoadImage( VS2005DragControlConsts.DEF_LEFT_INNER_TARGET_IMAGE_NAME, index++ );
			LoadImage( VS2005DragControlConsts.DEF_TOP_INNER_TARGET_IMAGE_NAME, index++ );
			LoadImage( VS2005DragControlConsts.DEF_RIGHT_INNER_TARGET_IMAGE_NAME, index++ );
			LoadImage( VS2005DragControlConsts.DEF_BOTTOM_INNER_TARGET_IMAGE_NAME, index++ );
			LoadImage( VS2005DragControlConsts.DEF_LEFT_TARGET_IMAGE_HIGHLIGHT, index++ );
			LoadImage( VS2005DragControlConsts.DEF_TOP_TARGET_IMAGE_HIGHLIGHT, index++ );
			LoadImage( VS2005DragControlConsts.DEF_RIGHT_TARGET_IMAGE_HIGHLIGHT, index++ );
			LoadImage( VS2005DragControlConsts.DEF_BOTTOM_TARGET_IMAGE_HIGHLIGHT, index++ );
			LoadImage( VS2005DragControlConsts.DEF_TAB_INNER_TARGET_IMAGE_HIGHLIGHT, index++ );
			LoadImage( VS2005DragControlConsts.DEF_LEFT_INNER_TARGET_IMAGE_HIGHLIGHT, index++ );
			LoadImage( VS2005DragControlConsts.DEF_TOP_INNER_TARGET_IMAGE_HIGHLIGHT, index++ );
			LoadImage( VS2005DragControlConsts.DEF_RIGHT_INNER_TARGET_IMAGE_HIGHLIGHT, index++ );
			LoadImage( VS2005DragControlConsts.DEF_BOTTOM_INNER_TARGET_IMAGE_HIGHLIGHT, index++ );
		}

		protected void LoadImage( string fileName, int index )
		{
			Bitmap bitmap = ( Bitmap )ImageLoader.Load( VS2008DragControlConsts.DEF_IMAGES_PATH + fileName);
			bitmap.MakeTransparent( Color.FromArgb( 0, 255, 0 ) );
			m_Images[index] = new Bitmap( bitmap );
		}
		#endregion

		#region class helper methods
		/// <summary>
		/// Method is used to refresh look of VS2008DragProvider.
		/// </summary>
		internal void Refresh()
		{
			this.HideControl();
			this.ShowControl();
		}
		#endregion
	}

	/// <summary>
	/// VS2008OuterDragControl class is used to layout and
	/// manage drag arrows used to dock control to host form.
	/// </summary>
	internal class VS2008OuterDragControl
		: DragOuterControl
	{
		#region class members
		protected DragTargetControl m_currentTarget = null;
		protected VS2008FadeTimer m_fadeTimer = null;
		#endregion

		#region class properties
		new protected VS2008DragTargetControl leftControl
		{
			get { return m_Controls[DEF_LEFT_CONTROL_INDEX] as VS2008DragTargetControl; }
			set { m_Controls[DEF_LEFT_CONTROL_INDEX] = value; }
		}

		new protected VS2008DragTargetControl topControl
		{
			get { return m_Controls[DEF_TOP_CONTROL_INDEX] as VS2008DragTargetControl; }
			set { m_Controls[DEF_TOP_CONTROL_INDEX] = value; }
		}

		new protected VS2008DragTargetControl rightControl
		{
			get { return m_Controls[DEF_RIGHT_CONTROL_INDEX] as VS2008DragTargetControl; }
			set { m_Controls[DEF_RIGHT_CONTROL_INDEX] = value; }
		}

		new protected VS2008DragTargetControl bottomControl
		{
			get { return m_Controls[DEF_BOTTOM_CONTROL_INDEX] as VS2008DragTargetControl; }
			set { m_Controls[DEF_BOTTOM_CONTROL_INDEX] = value; }
		}

		/// <summary>
		/// Gets if mouse cursor is above DragControl.
		/// </summary>
		internal bool MouseIn
		{ 
			get
			{
				bool mouseIn = false;

				foreach( VS2008DragTargetControl dtc in this.m_Controls )
				{
					if( dtc.MouseIn )
					{
						mouseIn = true;
						break;
					}
				}

				return mouseIn;
			}
		}
		#endregion

		#region class initialize/finalize methods
		/// <summary>
		/// Creates new instance of VS2008OuterDragControl class.
		/// </summary>
		/// <param name="parentControl">Parent DragControl.</param>
		public VS2008OuterDragControl( VS2008DragControl parentControl )
			: base( parentControl )
		{
			m_fadeTimer = new VS2008FadeTimer();
		}
		#endregion

		#region class overrides
		protected override void InitializeProvider()
		{
			for( int i = 0; i < 4; i++ )
			{
				DragTargetControl control = new VS2008DragTargetControl();
				m_Controls.Add( control );
				control.OnMouseEnter += new MouseEnterHandler( MouseEnterEvent );
				control.OnMouseLeave += new MouseLeaveHandler( MouseLeaveEvent );
				control.OnMouseUp += new MouseUpHandler( MouseUpEvent );
				control.OnMouseMove += new MouseMoveHandler( MouseMoveEvent );
				control.InternalControl.Opacity = VS2008DragControlConsts.DEF_TARGET_OPACITY;
				control.ParentControl = this;
			}

			leftControl.DragTarget = DragTarget.OuterLeft;
			leftControl.ActiveImage = ParentControl.GetImage( VS2005DragControlConsts.DEF_LEFT_ACTIVE_TARGET );
			leftControl.InactiveImage = ParentControl.GetImage( VS2005DragControlConsts.DEF_LEFT_INACTIVE_TARGET );

			topControl.DragTarget = DragTarget.OuterTop;
			topControl.ActiveImage = ParentControl.GetImage( VS2005DragControlConsts.DEF_TOP_ACTIVE_TARGET );
			topControl.InactiveImage = ParentControl.GetImage( VS2005DragControlConsts.DEF_TOP_INACTIVE_TARGET );

			rightControl.DragTarget = DragTarget.OuterRight;
			rightControl.ActiveImage = ParentControl.GetImage( VS2005DragControlConsts.DEF_RIGHT_ACTIVE_TARGET );
			rightControl.InactiveImage = ParentControl.GetImage( VS2005DragControlConsts.DEF_RIGHT_INACTIVE_TARGET );

			bottomControl.DragTarget = DragTarget.OuterBottom;
			bottomControl.ActiveImage = ParentControl.GetImage( VS2005DragControlConsts.DEF_BOTTOM_ACTIVE_TARGET );
			bottomControl.InactiveImage = ParentControl.GetImage( VS2005DragControlConsts.DEF_BOTTOM_INACTIVE_TARGET );
		}

		protected override void MouseEnterEvent( object sender, DragControlEventArgs e )
		{
			DragTargetControl dtc = sender as DragTargetControl;

			if( dtc != null )
			{
				Form targetArrow = dtc.InternalControl;

				if( targetArrow != null )
				{
					this.m_fadeTimer.StopAnimation();
					targetArrow.Opacity = 1;
					m_currentTarget = dtc;
				}
			}

			base.MouseEnterEvent( sender, e );
		}

		protected override void MouseLeaveEvent( object sender, DragControlEventArgs e )
		{
			DragTargetControl dtc = sender as DragTargetControl;

			if( dtc != null )
			{
				Form targetArrow = dtc.InternalControl;

				if( targetArrow != null )
				{
					targetArrow.Opacity = VS2008DragControlConsts.DEF_TARGET_OPACITY;
					m_currentTarget = null;
				}
			}

			base.MouseLeaveEvent( sender, e );
		}

		public override void HideControl()
		{
			base.HideControl();

			foreach( VS2008DragTargetControl dtc in m_Controls )
				dtc.MouseIn = false;
		}

		/// <summary>
		/// Shows drag arrows with fade effect.
		/// </summary>
		public override void ShowControl()
		{
			if( OuterDockAbility == DockAbility.All )
			{
				for( int i = 0; i < m_Controls.Count; i++ )
				{
					getControl( i ).InternalControl.Opacity = 0;
					getControl( i ).ShowControl();
				}
			}
			else
			{
				if( ( OuterDockAbility | DockAbility.Left ) == OuterDockAbility )
				{
					getControl( DEF_LEFT_CONTROL_INDEX ).InternalControl.Opacity = 0; ;
					getControl( DEF_LEFT_CONTROL_INDEX ).ShowControl();
				}
				if( ( OuterDockAbility | DockAbility.Top ) == OuterDockAbility )
				{
					getControl( DEF_TOP_CONTROL_INDEX ).InternalControl.Opacity = 0;
					getControl( DEF_TOP_CONTROL_INDEX ).ShowControl();
				}
				if( ( OuterDockAbility | DockAbility.Right ) == OuterDockAbility )
				{
					getControl( DEF_RIGHT_CONTROL_INDEX ).InternalControl.Opacity = 0;
					getControl( DEF_RIGHT_CONTROL_INDEX ).ShowControl();
				}
				if( ( OuterDockAbility | DockAbility.Bottom ) == OuterDockAbility )
				{
					getControl( DEF_BOTTOM_CONTROL_INDEX ).InternalControl.Opacity = 0;
					getControl( DEF_BOTTOM_CONTROL_INDEX ).ShowControl();
				}
			}

			m_fadeTimer.FadeChange += new VS2008FadeTimer.FadeChangeEventHandler( drag2008_FadeChange );
			m_fadeTimer.InitAnimation();
		}
		#endregion

		#region class helper methods
		private void drag2008_FadeChange( FadeEventArgs args )
		{
			foreach( DragTargetControl dtc in m_Controls )
			{
				if( dtc.Visible )
					dtc.InternalControl.Opacity = args.Opacity;
			}

			if( args.Opacity == VS2008DragControlConsts.DEF_TARGET_OPACITY )
			{
				m_fadeTimer.FadeChange -= new VS2008FadeTimer.FadeChangeEventHandler( drag2008_FadeChange );
			}
		}
		#endregion
	}

	/// <summary>
	/// VS2008InnerDragControl class is used to layout and manage dreag arrwos used to dock
	/// control to another dock enabled control.
	/// </summary>
	internal class VS2008InnerDragControl
		: VS2008OuterDragControl
	{
		#region class members
		protected const int DEF_INNER_CONTROL_INDEX = 4;
		protected const int DEF_INNER_TAB_OFFSET = 15;
		protected readonly Size DEF_TARGET_SIZE = new Size( 107, 105 );
		protected readonly Size DEF_TAB_SIZE = new Size( 77, 75 );
		protected DockAbility m_DockAbility = DockAbility.All;
		protected DragControlEventArgs m_tabArgs = null;
		protected bool m_bTabControlHighLight = true;
		protected bool m_bVisible = false;
		protected Hashtable m_disableArrows = null;
		#endregion

		#region class properties
		protected VS2008DragTargetControl tabControl
		{
			get { return m_Controls[DEF_INNER_CONTROL_INDEX] as VS2008DragTargetControl; }
			set { m_Controls[DEF_INNER_CONTROL_INDEX] = value; }
		}
		#endregion

		#region class initialize/finalize methods
		/// <summary>
		/// Creates new instance of VS2008InnerDragControl class.
		/// </summary>
		/// <param name="parent"></param>
		public VS2008InnerDragControl( VS2008DragControl parent )
			: base( parent )
		{
			m_disableArrows = new Hashtable();
		}
		#endregion

		#region class overrides
		public override bool ProcessMouseMove( Point point )
		{
			for( int i = 0; i < m_Controls.Count; i++ )
			{
				if( getControl( i ).ProcessMouseMove( point ) )
					return true;
			}
			return false;
		}

		protected override void InitializeProvider()
		{
			for( int i = 0; i < 5; i++ )
			{
				DragTargetControl control = new VS2008DragTargetControl();
				m_Controls.Add( control );
				control.OnMouseEnter += new MouseEnterHandler( MouseEnterEvent );
				control.OnMouseLeave += new MouseLeaveHandler( MouseLeaveEvent );
				control.OnMouseUp += new MouseUpHandler( MouseUpEvent );
				control.OnMouseMove += new MouseMoveHandler( MouseMoveEvent );
				control.InternalControl.Opacity = VS2008DragControlConsts.DEF_TARGET_OPACITY;
				control.ParentControl = this;
			}

			leftControl.DragTarget = DragTarget.InnerLeft;
			leftControl.ActiveImage = ParentControl.GetImage( VS2005DragControlConsts.DEF_LEFT_INNER_ACTIVE_TARGET );
			leftControl.InactiveImage = ParentControl.GetImage( VS2005DragControlConsts.DEF_LEFT_INNER_INACTIVE_TARGET );

			topControl.DragTarget = DragTarget.InnerTop;
			topControl.ActiveImage = ParentControl.GetImage( VS2005DragControlConsts.DEF_TOP_INNER_ACTIVE_TARGET );
			topControl.InactiveImage = ParentControl.GetImage( VS2005DragControlConsts.DEF_TOP_INNER_INACTIVE_TARGET );

			rightControl.DragTarget = DragTarget.InnerRight;
			rightControl.ActiveImage = ParentControl.GetImage( VS2005DragControlConsts.DEF_RIGHT_INNER_ACTIVE_TARGET );
			rightControl.InactiveImage = ParentControl.GetImage( VS2005DragControlConsts.DEF_RIGHT_INNER_INACTIVE_TARGET );

			bottomControl.DragTarget = DragTarget.InnerBottom;
			bottomControl.ActiveImage = ParentControl.GetImage( VS2005DragControlConsts.DEF_BOTTOM_INNER_ACTIVE_TARGET );
			bottomControl.InactiveImage = ParentControl.GetImage( VS2005DragControlConsts.DEF_BOTTOM_INNER_INACTIVE_TARGET );

			tabControl.DragTarget = DragTarget.InnerTab;
			tabControl.ActiveImage = ParentControl.GetImage( VS2005DragControlConsts.DEF_ACTIVE_TAB );
			tabControl.InactiveImage = ParentControl.GetImage( VS2005DragControlConsts.DEF_INACTIVE_TAB );

			foreach( DragTargetControl dtc in m_Controls )
			{
				if( dtc != tabControl )
					dtc.InternalControl.Owner = tabControl.InternalControl;
			}
		}

		protected override void MouseEnterEvent( object sender, DragControlEventArgs e )
		{
			VS2008DragTargetControl dtc = sender as VS2008DragTargetControl;

			if( dtc != null )
			{
				Form targetArrow = dtc.InternalControl;
				AllowHighlightEventArgs args = new AllowHighlightEventArgs( dtc.DragTarget );
				AllowHighlightEvent( this, args );

				if( m_disableArrows.ContainsKey( dtc ) )
					m_disableArrows.Remove( dtc );

				m_disableArrows.Add( dtc, dtc.AllowHighLight );
				dtc.AllowHighLight = args.AllowHighlight;

				if( dtc.AllowHighLight )
				{
					if( targetArrow != null )
					{
						if( dtc != tabControl )
						{
							m_bTabControlHighLight = tabControl.AllowHighLight;
							tabControl.AllowHighLight = false;

							if( tabControl == m_currentTarget )
							{
								tabControl.InternalControl.Opacity = VS2008DragControlConsts.DEF_TARGET_OPACITY;
								RaiseOnMouseLeave( tabControl, m_tabArgs );
								tabControl.MouseIn = false;
							}
						}
						else
							m_tabArgs = e;

						m_fadeTimer.StopAnimation();
						targetArrow.Opacity = 1;
						m_currentTarget = dtc;
						RaiseOnMouseEnter( sender, e );
					}
				}
			}
		}

		protected override void MouseLeaveEvent( object sender, DragControlEventArgs e )
		{
			VS2008DragTargetControl dtc = sender as VS2008DragTargetControl;

			if( dtc != null )
			{
				Form targetArrow = dtc.InternalControl;

				if( targetArrow != null )
				{
					targetArrow.Opacity = VS2008DragControlConsts.DEF_TARGET_OPACITY;

					if( dtc != tabControl )
						tabControl.AllowHighLight = m_bTabControlHighLight;

					RaiseOnMouseLeave( sender, e );
				}
			}
		}

		/// <summary>
		/// Gets/sets layout rectangle for this control.
		/// </summary>
		public override Rectangle ControllerRect
		{
			get
			{
				return base.ControllerRect;
			}
			set
			{
				if( m_ControllerRect != value )
				{
					m_ControllerRect = value;
					CalculateControlsBounds( value );

					if( m_bVisible )
					{
						this.HideControl();
						this.ShowControl();
					}
				}
			}
		}

		/// <summary>
		/// Gets/sets current DockAbility.
		/// </summary>
		public override DockAbility DockAbility
		{
			get
			{
				return m_DockAbility;
			}
			set
			{
				if( m_DockAbility != value )
				{
					m_DockAbility = value;

					if( ( value | DockAbility.Tabbed ) != value )
					{
						tabControl.ActiveImage = ParentControl.GetImage( VS2005DragControlConsts.DEF_INNER_TARGET );
						tabControl.InactiveImage = ParentControl.GetImage( VS2005DragControlConsts.DEF_INNER_TARGET );
						tabControl.AllowHighLight = false;
					}
					else
					{
						tabControl.ActiveImage = ParentControl.GetImage( VS2005DragControlConsts.DEF_ACTIVE_TAB );
						tabControl.InactiveImage = ParentControl.GetImage( VS2005DragControlConsts.DEF_INACTIVE_TAB );
						tabControl.AllowHighLight = true;
					}
				}
			}
		}

		private void CalculateControlsBounds( Rectangle area )
		{
			Rectangle rect = new Rectangle(Point.Empty, DEF_TARGET_SIZE);
			Point center = new Point( area.Width/2, area.Height/2 );
			center.Offset( area.Location.X, area.Location.Y );
			center.Offset( -DEF_TARGET_SIZE.Width / 2, -DEF_TARGET_SIZE.Height / 2 );
			rect.Location = center;

			Rectangle rcLeftBounds = new Rectangle();
			rcLeftBounds.X = rect.Left;
			rcLeftBounds.Y = rect.Top + ( rect.Height - leftControl.InactiveImage.Height ) / 2;
			rcLeftBounds.Width = leftControl.InactiveImage.Width;
			rcLeftBounds.Height = leftControl.InactiveImage.Height;

			Rectangle rcTopBounds = new Rectangle();
			rcTopBounds.X = rect.Left + ( rect.Width - topControl.InactiveImage.Width ) / 2;
			rcTopBounds.Y = rect.Top;
			rcTopBounds.Width = topControl.InactiveImage.Width;
			rcTopBounds.Height = topControl.InactiveImage.Height;

			Rectangle rcRightBounds = new Rectangle();
			rcRightBounds.X = rect.Left + rect.Width - rightControl.InactiveImage.Width;
			rcRightBounds.Y = rect.Top + ( rect.Height - rightControl.InactiveImage.Height ) / 2;
			rcRightBounds.Width = rightControl.InactiveImage.Width;
			rcRightBounds.Height = rightControl.InactiveImage.Height;

			Rectangle rcBottomBounds = new Rectangle();
			rcBottomBounds.X = rect.Left + ( rect.Width - bottomControl.InactiveImage.Width ) / 2;
			rcBottomBounds.Y = rect.Top + rect.Height - bottomControl.InactiveImage.Height;
			rcBottomBounds.Width = bottomControl.InactiveImage.Width;
			rcBottomBounds.Height = bottomControl.InactiveImage.Height;

			Rectangle rcInnerTab = new Rectangle( Point.Empty, DEF_TAB_SIZE );
			rcInnerTab.Location = new Point( rcLeftBounds.X + DEF_INNER_TAB_OFFSET, rcTopBounds.Y + DEF_INNER_TAB_OFFSET );

			leftControl.SetBounds( rcLeftBounds );
			rightControl.SetBounds( rcRightBounds );
			topControl.SetBounds( rcTopBounds );
			bottomControl.SetBounds( rcBottomBounds );
			tabControl.SetBounds( rcInnerTab );
		}

		/// <summary>
		/// Shows control with fade effect.
		/// </summary>
		public override void ShowControl()
		{
			if( DockAbility == DockAbility.All )
			{
				for( int i = 0; i < m_Controls.Count; i++ )
				{
					getControl( i ).InternalControl.Opacity = 0;
					getControl( i ).ShowControl();
				}
			}
			else
			{
				if( ( DockAbility | DockAbility.Left ) == DockAbility )
				{
					getControl( DEF_LEFT_CONTROL_INDEX ).InternalControl.Opacity = 0; ;
					getControl( DEF_LEFT_CONTROL_INDEX ).ShowControl();
				}
				if( ( DockAbility | DockAbility.Top ) == DockAbility )
				{
					getControl( DEF_TOP_CONTROL_INDEX ).InternalControl.Opacity = 0;
					getControl( DEF_TOP_CONTROL_INDEX ).ShowControl();
				}
				if( ( DockAbility | DockAbility.Right ) == DockAbility )
				{
					getControl( DEF_RIGHT_CONTROL_INDEX ).InternalControl.Opacity = 0;
					getControl( DEF_RIGHT_CONTROL_INDEX ).ShowControl();
				}
				if( ( DockAbility | DockAbility.Bottom ) == DockAbility )
				{
					getControl( DEF_BOTTOM_CONTROL_INDEX ).InternalControl.Opacity = 0;
					getControl( DEF_BOTTOM_CONTROL_INDEX ).ShowControl();
				}

				getControl( DEF_INNER_CONTROL_INDEX ).InternalControl.Opacity = 0;
				getControl( DEF_INNER_CONTROL_INDEX ).ShowControl();
			}

			m_bVisible = true;
			m_fadeTimer.FadeChange += new VS2008FadeTimer.FadeChangeEventHandler( drag2008_FadeChange );
			m_fadeTimer.InitAnimation();
		}

		/// <summary>
		/// Immediately hides control.
		/// </summary>
		public override void HideControl()
		{
			foreach( VS2008DragTargetControl control in m_Controls )
			{
				control.HideControl();

				if( control.MouseIn )
				{
					if( control != tabControl )
						tabControl.AllowHighLight = m_bTabControlHighLight;

					control.MouseIn = false;
				}
			}

			foreach( VS2008DragTargetControl dtc in m_disableArrows.Keys )
				dtc.AllowHighLight = (bool)m_disableArrows[dtc];

			if( m_tabArgs != null )
				tabControl.AllowHighLight = m_bTabControlHighLight;

			m_disableArrows.Clear();
			m_bVisible = false;
			m_bTabControlHighLight = true;
			m_tabArgs = null;
			m_currentTarget = null;
		}
		#endregion

		#region class helper methods
		private void drag2008_FadeChange( FadeEventArgs args )
		{
			foreach( DragTargetControl dtc in this.m_Controls )
			{
				if( dtc.Visible )
					dtc.InternalControl.Opacity = args.Opacity;
			}

			if( args.Opacity == VS2008DragControlConsts.DEF_TARGET_OPACITY )
				m_fadeTimer.FadeChange -= new VS2008FadeTimer.FadeChangeEventHandler( drag2008_FadeChange );
		}
		#endregion
	}

	/// <summary>
	/// VS2008DragTargetControl class is used to show drop arrows.
	/// </summary>
	internal class VS2008DragTargetControl
		: DragTargetControl
	{
		#region class members
		private bool m_bAllowHighlight = true;
		#endregion

		#region class properties
		internal bool AllowHighLight
		{
			get
			{
				return m_bAllowHighlight;
			}
			set
			{
				if( m_bAllowHighlight != value )
				{
					m_bAllowHighlight = value;
					this.InternalControl.Invalidate();
					this.InternalControl.Update();

					if( !AllowHighLight )
						MouseIn = false;
				}
			}
		}

		/// <summary>
		/// Indicates if mouse pointer is above this control.
		/// </summary>
		internal bool MouseIn
		{
			get
			{
				return m_mouseIn;
			}
			set
			{
				if( m_mouseIn != value )
				{
					if( this.AllowHighLight || !value )
						m_mouseIn = value;
				}
			}
		}
		#endregion

		#region class overrides
		/// <summary>
		/// Paints control.
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="e">Paint event args</param>
		public override void OnPaint( object sender, PaintEventArgs e )
		{
			e.Graphics.DrawImage( (m_mouseIn && AllowHighLight) ? activeImage : inactiveImage,
				m_internalControl.ClientRectangle );
		}

		public override bool ProcessMouseUp( Point point )
		{
			if( m_mouseIn == true )
			{
				RaiseMouseUpEvent();
				return true;
			}
			return false;
		}

		public override bool ProcessMouseMove( Point point )
		{
			bool result = false;

			if( this.AllowHighLight && this.Visible )
			{
				bool mouseIn = false;
				if( m_internalControl.Bounds.Contains( point ) )
				{
					Point pt = m_internalControl.PointToClient( point );
					Bitmap bitmap = CurrentImage as Bitmap;
					if( bitmap != null )
					{
						Color pointColor = bitmap.GetPixel( pt.X, pt.Y );
						mouseIn = pointColor.ToArgb() != 0;
					}
				}

				if( MouseIn != mouseIn )
				{
					if( MouseIn )
					{
						RaiseMouseLeaveEvent();
					}
					else
					{
						RaiseMouseEnterEvent();
					}

					MouseIn = !m_mouseIn;
					m_internalControl.Invalidate();
					result = true;
				}
				else
					if( mouseIn )
						RaiseMouseMoveEvent();
			}

			return result;
		}

		public override void HideControl()
		{
			if( this.MouseIn )
				RaiseMouseLeaveEvent();
				
			base.HideControl();
		}
		#endregion
	}

	/// <summary>
	/// Timer used to calculate and perform fade effect.
	/// </summary>
	internal class VS2008FadeTimer
	{
		#region class members
		private Timer m_fadeTimer;
		private const int m_cAnimationTime = 200;
		private const int m_cAnimationSteps = 10;
		private float tempOpacity = 0;
		#endregion

		#region class events
		internal delegate void FadeChangeEventHandler( FadeEventArgs args );
		internal event FadeChangeEventHandler FadeChange;
		#endregion

		#region class initialize/finalize events
		/// <summary>
		/// Initializes new instance of VS2008FadeTimer class.
		/// </summary>
		public VS2008FadeTimer()
		{
			m_fadeTimer = new Timer();
		}
		#endregion

		#region class utility methods
		/// <summary>
		/// Fires fade change event.
		/// </summary>
		/// <param name="args">Fade change args.</param>
		protected void OnFadeChange( FadeEventArgs args )
		{
			if( FadeChange != null )
				FadeChange( args );
		}

		/// <summary>
		/// Initiates animation.
		/// </summary>
		internal void InitAnimation()
		{
			if( !m_fadeTimer.Enabled )
			{
				tempOpacity = 0;
				m_fadeTimer.Interval = m_cAnimationTime / m_cAnimationSteps;
				m_fadeTimer.Tick += new EventHandler( m_fadeTimer_Tick );
				m_fadeTimer.Start();
			}
		}

		protected void m_fadeTimer_Tick( object sender, EventArgs e )
		{
			if( tempOpacity < VS2008DragControlConsts.DEF_TARGET_OPACITY )
			{
				float delta = ( float )VS2008DragControlConsts.DEF_TARGET_OPACITY / ( float )m_cAnimationSteps;
				tempOpacity += delta;
				OnFadeChange( new FadeEventArgs( this, tempOpacity ) );
			}
			else
			{
				StopAnimation();
			}
		}

		/// <summary>
		/// Forces timer to stop.
		/// </summary>
		internal void StopAnimation()
		{
			OnFadeChange( new FadeEventArgs( this, VS2008DragControlConsts.DEF_TARGET_OPACITY ) );
			tempOpacity = 0;
			m_fadeTimer.Stop();
			m_fadeTimer.Tick -= new EventHandler( m_fadeTimer_Tick );
		}
		#endregion
	}
}