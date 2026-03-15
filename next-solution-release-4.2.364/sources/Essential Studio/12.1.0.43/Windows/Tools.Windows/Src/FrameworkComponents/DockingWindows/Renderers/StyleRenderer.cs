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
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Collections;
using Syncfusion.Windows.Forms.Renderers;

namespace Syncfusion.Windows.Forms.Tools.Renderers
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IStyleRenderer
	{
		#region Methods

		void PaintDockedControl( Graphics g, Rectangle rectangle, PaintDockControlArgs args );
		void PaintSplitter( Graphics g, Rectangle rectangle, Orientation orientation );
		void PaintAutoHidePanels( Graphics g, Rectangle rectangle, AutoHideSide side);
		bool RefreshColors();
		void RefreshPaintInfo( Rectangle rectangle, PaintDockControlArgs args );
		HitTestArea HitTest( MouseButtons button, Point point );
		int GetHitButtonIndex();
        int GetHighlightedButtonIndex();
		CaptionButton GetHitButton();
		void ResetCaptionButtonsIndices();
        void RefreshOffice2007Theme(Office2007Theme newTheme);
        void RefreshOffice2010Theme(Office2010Theme newTheme);

		#endregion

		#region Properties

		Rectangle ControlBounds { get; set; }
        Rectangle CaptionBounds { get; }
		int CaptionWidth { get; }
		int BorderWidth { get; }
		int ThickBorderWidth { get; }
		int ThinBorderWidth { get; }
		bool IsMirrored { get; set; }

		#endregion
	}

	internal abstract class StyleRenderer : IStyleRenderer
	{
		#region Constructors

		public StyleRenderer()
		{
			ColorTable = new ColorTableBase();
		}


		#endregion

		#region Public virtual/abstract methods

		public virtual void RefreshPaintInfo( Rectangle rectangle, PaintDockControlArgs args )
		{
            if (Layout.Table != null)
            {
                Layout.Table.Dispose();
                Layout.Table = null;
            }

			Layout.ControlBounds = rectangle;
			Layout.CaptionEnabled = ( args.Caption == null )? false: args.Caption.Enabled;
			Layout.Floating = args.Floating;
			Layout.ImageEnabled = 
				( GetImageFromList(args.CaptionImageIndex, args.ImageList) == null )? false: true;
			Layout.Table = args.Table;
		}

		public void ResetCaptionButtonsIndices()
		{
			Layout.ResetCaptionButtonIndices();
		}

		public virtual int CaptionWidth
		{
			get { return m_Layout.CaptionWidth; }
		}

		public virtual int BorderWidth 
		{
			get { return m_Layout.BorderWidth; }
		}

		public virtual int ThickBorderWidth
		{
			get { return m_Layout.ThickBorderWidth; }
		}

		public virtual int ThinBorderWidth
		{
			get { return m_Layout.ThinBorderWidth; }
		}

		public abstract void PaintDockedControl(  Graphics g, Rectangle rectangle, 
			PaintDockControlArgs args );
		public abstract void PaintSplitter( Graphics g, Rectangle rectangle, 
			Orientation orientation );
		public abstract void PaintAutoHidePanels( Graphics g, Rectangle rectangle, 
			AutoHideSide side );

        public abstract int GetHighlightedButtonIndex();

		public virtual bool RefreshColors()
		{
			return ColorTable.RefreshColors();
		}

		public virtual void RefreshOffice2007Theme(Office2007Theme newTheme)
		{ 
		}
        public virtual void RefreshOffice2010Theme(Office2010Theme newTheme)
        {

        }
		protected abstract Image GetImageFromList( int index, ImageList list );


		#endregion

		#region Properties

		protected virtual IControlStyleLayout Layout
		{
			get { return m_Layout; }
			set { m_Layout = value; }
		}

		protected virtual ColorTableBase ColorTable
		{
			get
			{
				return m_ColorTable;
			}
			set
			{
				m_ColorTable = value;
			}
		}

		public virtual HitTestArea HitTest( MouseButtons button, Point point )
		{
			HitTestArea hitTest = m_Layout.HitTest( button, point );
			if( hitTest != HitTestArea.Button )
			{
				m_Layout.ResetCaptionButtonIndices();
			}
			return hitTest;
		}

		public virtual int GetHitButtonIndex()
		{
			return m_Layout.GetHitButtonIndex();
		}

		public virtual CaptionButton GetHitButton()
		{
			return m_Layout.GetHitButton();
		}

		public virtual Rectangle ControlBounds
		{
			get { return Layout.ControlBounds; }
			set { Layout.ControlBounds = value; }
		}

        public virtual Rectangle CaptionBounds
        {
            get { return Layout.CaptionBounds; }
        }

		public bool IsMirrored
		{
			get { return Layout.IsMirrored; }
			set { Layout.IsMirrored = value; }
		}

		protected string ImagesPath
		{
			get
			{
				return DEF_IMAGES_PATH;
			}
		}


		#endregion

		#region Private members

		private IControlStyleLayout m_Layout;
		private ColorTableBase m_ColorTable;

		#endregion

		#region Class constants

		private string DEF_IMAGES_PATH = "Syncfusion.Windows.Forms.Tools.FrameworkComponents.DockingWindows.Images.Renderers.";

		#endregion
	}

	internal abstract class CustomRenderer : StyleRenderer
	{
		#region Methods

		public override bool RefreshColors()
		{
			if( base.RefreshColors() )
			{
				RefreshBrushes();
				return true;
			}
			else
				return false;
		}

		protected override Image GetImageFromList( int index, ImageList list )
		{
			if( index >= 0 &&  list != null 
				&& index < list.Images.Count )
			{
				return list.Images[index];
			}
			else
			{
				return null;
			}
		}

		protected abstract void CreateGraphicObjects( bool recreate );
		protected abstract void CreateCaptionBrushes(bool recreate);
		protected virtual void RefreshBrushes(bool recreate)
		{
			DisposeBrushes();
			CreateGraphicObjects(recreate);
		}
		protected virtual void RefreshBrushes()
		{
			RefreshBrushes(true);
		}
		protected abstract void DisposeBrushes();
		protected virtual void RefreshCaptionBrushes()
		{
			DisposeCaptionBrushes();
			CreateCaptionBrushes(true);
		}

		protected abstract void DisposeCaptionBrushes();

		public override void PaintDockedControl(Graphics g, Rectangle rectangle, 
			PaintDockControlArgs args)
		{
			RefreshPaintInfo( rectangle, args );
			CreateGraphicObjects( false );

			if( args.PaintBorders )
			{
				PaintBorders( g, args );
			}
			if( args.Caption != null && args.Caption.Enabled )
			{
				if( Layout.CaptionBounds.Width != 0 && Layout.CaptionBounds.Height != 0 )
				{
					if( m_bNeedCreateBrushes )
						CreateCaptionBrushes( true );
					PaintCaption( g, args );
				}
			}
		}

		public override void PaintAutoHidePanels(Graphics g, Rectangle rectangle, 
			AutoHideSide side)
		{

		}

		public override void PaintSplitter(Graphics g, Rectangle rectangle, 
			Orientation orientation )
		{
			Brush splitterBrush = GetSplitterBrush();
			RendererPrimitives.PaintBackground( g, rectangle, splitterBrush );
		}


		protected virtual StringAlignment GetTextAlignment( DockLabelAlignmentStyle alignment )
		{
			StringAlignment stringAlignment = StringAlignment.Near;

			switch( alignment )
			{
				case DockLabelAlignmentStyle.Default:
				case DockLabelAlignmentStyle.Left:
				{
					stringAlignment = StringAlignment.Near;
					break;
				}
				case DockLabelAlignmentStyle.Right:
				{
					stringAlignment = StringAlignment.Far;
					break;
				}
				case DockLabelAlignmentStyle.Center:
				{
					stringAlignment = StringAlignment.Center;
					break;
				}
			}
			return stringAlignment;
		}

		protected abstract Brush GetCaptionBrush( CaptionState cpState );
		protected abstract Brush GetTextBrush( CaptionState cpState );
		protected abstract Brush GetBorderBrush( bool innerBorder );
		protected abstract Brush GetSplitterBrush();
		protected abstract Brush GetButtonBrush(CaptionState cpState, CaptionButtonState btnState);
		protected abstract Pen GetButtonPen(CaptionState cpState, CaptionButtonState btnState);
		protected abstract Pen GetButtonImagePen(CaptionState cpState, CaptionButtonState btnState);

		protected virtual void PaintCaption( Graphics g, PaintDockControlArgs args )
		{
			if( m_CaptionHeight != Layout.CaptionWidth || m_PrevFloating != Layout.Floating )
			{
				m_PrevFloating = Layout.Floating;
				m_CaptionHeight = Layout.CaptionWidth;
				RefreshCaptionBrushes();
			}

            Brush captionBrush;
            if (args.ProvideGraphicsItemsArgs != null &&
                args.ProvideGraphicsItemsArgs.CaptionBackground != null )
            {
                captionBrush = args.ProvideGraphicsItemsArgs.CaptionBackground;
            }
            else
            {
                captionBrush = GetCaptionBrush(args.Caption.CaptionState);
            }

			RendererPrimitives.PaintBackground( g, Layout.CaptionBounds, captionBrush );
				
			PaintButtons( g, args );
			DrawText( g, args );
			if( Layout.ImageEnabled )
			{
				g.DrawImage( GetImageFromList(args.CaptionImageIndex, args.ImageList), Layout.ImageBounds );
			}		
		}

		protected virtual void PaintBorders( Graphics g, PaintDockControlArgs args )
		{
			Brush innerBrush; 
			Brush outerBrush;
            if( args.ProvideGraphicsItemsArgs != null &&
                args.ProvideGraphicsItemsArgs.CaptionForeground != Color.Empty )
            {
                innerBrush = new SolidBrush( args.ProvideGraphicsItemsArgs.CaptionForeground );
                outerBrush = new SolidBrush( args.ProvideGraphicsItemsArgs.CaptionForeground );
            }
            else
            {
                innerBrush = GetBorderBrush(true);
                outerBrush = GetBorderBrush(false);
            }

			RendererPrimitives.PaintBorders( g, Layout.ControlBounds,
				outerBrush, Layout.OuterBorderWidth);

			Rectangle innerBorder = Layout.ControlBounds;
			innerBorder.Inflate( -Layout.OuterBorderWidth, -Layout.OuterBorderWidth );
			if( Layout.Floating )
			{
				RendererPrimitives.PaintBorders( g, innerBorder, 
					innerBrush, Layout.BorderWidth - Layout.OuterBorderWidth);
			}
		}
		protected virtual void PaintButtons( Graphics g, PaintDockControlArgs args )
		{
			if( Layout.CaptionEnabled )
			{
			Rectangle bounds = Rectangle.Empty;
				for( int i = 0; i < args.Table.Buttons.Count; i++ )
				{
					bounds = Layout.GetButtonBounds( i );

					if( bounds == Rectangle.Empty )
					{
						continue;
					}

					CaptionButtonState cbState = CaptionButtonState.Normal;
					if( !args.DesignMode )
					{
						if( Layout.ActiveButtonIndex == i )
						{
							cbState = CaptionButtonState.Active;
						}
						else if( Layout.PushedButtonIndex == i )
						{
							cbState = CaptionButtonState.Pushed;
						}

                        m_HighlightButtonIndex = Layout.ActiveButtonIndex;
					}

					bounds.Inflate(-1, -1); // Prepare rectangle for image painting.

                    Rectangle r = new Rectangle();

                    if (Layout.Floating && this is RendererMetro)
                    {
                        r = new Rectangle(bounds.X, bounds.Y - 1, bounds.Width + 1, bounds.Height - 1);
                    }
                    else
                        r = bounds;

                    PaintButtonBackground(g, args.Caption.CaptionState, r, cbState);

                    PaintButtonImage(g, args.Caption.CaptionState, Layout.Table.Buttons[i], cbState, Layout.Table.Options[i], r, args.ImageList);
				}
			}
		}

		protected virtual void PaintButtonBackground( Graphics g, CaptionState cpState, Rectangle rectangle, CaptionButtonState btnState )
		{
			if( btnState != CaptionButtonState.Normal )
			{
				Brush buttonBrush = GetButtonBrush( cpState, btnState );
				Pen buttonBorderPen = GetButtonPen( cpState, btnState );
                using (Brush bgbuttonBrush = new SolidBrush(Color.White))
                {
                    using (Pen bgbuttonBorderPen = new Pen(bgbuttonBrush))
                        RendererPrimitives.PaintButtonBackground(g, rectangle, bgbuttonBorderPen, bgbuttonBrush);
                }
                RendererPrimitives.PaintButtonBackground( g, rectangle, buttonBorderPen, buttonBrush );
			}
		}

		protected virtual void PaintButtonImage( Graphics g, CaptionState cpState, 
			CaptionButton button, CaptionButtonState btnState, CaptionButtonOptions btnOptions, Rectangle rect, ImageList imageList )
		{
			Image buttonImage = GetImageFromList( button.ImageIndex, imageList );			
			if( buttonImage != null )
			{
				Bitmap bmp = buttonImage as Bitmap;
				if( bmp != null && button.TransparentImageColor != Color.Transparent )
				{
					bmp.MakeTransparent( button.TransparentImageColor );
				}
				RendererPrimitives.PaintCustomButtonImage( g, buttonImage, rect );
			}
			else
			{
				Pen imgPen = GetButtonImagePen( cpState, btnState );
				switch( button.Type )
				{
					case CaptionButtonType.Close:
						RendererPrimitives.PaintCloseButtonImage( g, imgPen, rect );
						break;
					case CaptionButtonType.Pin:
						if( btnOptions.ModifiedView )
							RendererPrimitives.PaintPinButtonImageHorizontal( g, imgPen, rect );
						else
							RendererPrimitives.PaintPinButtonImageVertical( g, imgPen, rect );
						break;
					case CaptionButtonType.Menu:
						RendererPrimitives.PaintMenuButtonImage( g, imgPen, rect );
						break;
					case CaptionButtonType.Maximize:
						bool bRestoreButtonExists = Layout.Table.Buttons.ContainsButtonType(CaptionButtonType.Restore);
						if( btnOptions.ModifiedView && !bRestoreButtonExists )
							RendererPrimitives.PaintRestoreButtonImage( g, imgPen, rect );
						else
							RendererPrimitives.PaintMaximizeButtonImage( g, imgPen, rect );
						break;
					case CaptionButtonType.Restore:
						RendererPrimitives.PaintRestoreButtonImage(g, imgPen, rect);
						break;

				}
			}
		}

		protected virtual void DrawText( Graphics g, PaintDockControlArgs args )
		{
			Layout.TextRectHeight = args.Caption.Font.Height + DEF_CAPTION_OFFSET;
			if( Layout.TextBounds != Rectangle.Empty )
			{
				StringFormat format = new StringFormat();
				format.Trimming = StringTrimming.EllipsisCharacter;
				format.Alignment = GetTextAlignment( args.Caption.TextAlignment );
				format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;

				if( this.IsMirrored )
					format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;

                Font ftCaption;
                if (args.ProvideGraphicsItemsArgs != null &&
                    args.ProvideGraphicsItemsArgs.CaptionFont != null)
                {
                    ftCaption = args.ProvideGraphicsItemsArgs.CaptionFont;
                }
                else
                {
                    ftCaption = args.Caption.Font;
                }
                if (this is RendererVS2010)
                {
                    ftCaption = new Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
                }
				Brush textBrush = GetTextBrush( args.Caption.CaptionState );
                if (args.ProvideGraphicsItemsArgs != null && args.ProvideGraphicsItemsArgs.CaptionForeground != Color.Empty)
                    textBrush = new SolidBrush(args.ProvideGraphicsItemsArgs.CaptionForeground);
				RendererPrimitives.DrawText( g, args.Caption.Text, ftCaption,
					textBrush, Layout.TextBounds, format);
			}
		}

        public override int GetHighlightedButtonIndex()
        {
            return m_HighlightButtonIndex;
        }

		#endregion

		#region Private members

		private int m_CaptionHeight = SystemInformation.ToolWindowCaptionHeight;
		private bool m_PrevFloating;
        private int m_HighlightButtonIndex = -1;
		protected Rectangle m_prevCaptionRect = Rectangle.Empty;
		protected bool m_bNeedCreateBrushes = false;

		#endregion


		#region Class constants

		private int DEF_CAPTION_OFFSET = 1; 

		#endregion

		#region Internal properties

		internal int CaptionHeight
		{
			get { return m_CaptionHeight; }
			set
			{
				if (m_CaptionHeight != value)
				{
					m_CaptionHeight = value;
				}
			}
		}

		internal bool PrevFloating
		{
			get { return m_PrevFloating; }
			set
			{
				if (m_PrevFloating != value)
				{
					m_PrevFloating = value;
				}
			}
		}

		#endregion
	}


	internal class RendererOffice2003 : CustomRenderer
	{
		#region Constructors

		public RendererOffice2003 ()
		{
			ColorTable = new ColorTableOffice2003();
			Layout = new LayoutOffice2003();
			CreateGraphicObjects( false );
		}

		#endregion

		#region Methods

		public override void PaintSplitter(Graphics g, Rectangle rectangle, 
			Orientation orientation )
		{
			base.PaintSplitter( g, rectangle, orientation );
			
			int dotCount = 6;
			int gripperWidth = dotCount * 4 - 1;
			Point point = Point.Empty;
			if( orientation == Orientation.Horizontal )
			{
				point.X = rectangle.Width / 2 - gripperWidth / 2;
				point.Y = (rectangle.Height - 3) / 2 + 1;
			}
			else
			{
				point.X = (rectangle.Width - 3) / 2 + 1;
				point.Y = rectangle.Height / 2 - gripperWidth / 2;
			}
			RendererPrimitives.PaintGripper( g, orientation, point,	m_GripperBitmap, dotCount );
		}


		protected override Brush GetCaptionBrush(CaptionState cpState)
		{
			if( cpState == CaptionState.Normal )
			{
				return m_CaptionBrush;
			}
			else
			{
				return m_ActiveCaptionBrush;
			}
		}

		protected override Brush GetTextBrush( CaptionState cpState )
		{
			return m_TextBrush;
		}

		protected override Brush GetBorderBrush( bool innerBorder )
		{
			if( innerBorder )
			{
				return m_InnerBorderBrush;
			}
			else
			{
				return m_OuterBorderBrush;
			}
		}

		protected override Brush GetSplitterBrush()
		{
			return m_SplitterBrush;
		}

		protected override Brush GetButtonBrush(CaptionState cpState, CaptionButtonState btnState)
		{
			if( btnState == CaptionButtonState.Active  )
			{
				return m_ActiveButtonBrush;
			}
			else
			{
				return m_PushedButtonBrush;
			}
		}

		protected override Pen GetButtonPen(CaptionState cpState, CaptionButtonState btnState)
		{
			return m_ButtonBorderPen;
		}

		protected override Pen GetButtonImagePen(CaptionState cpState, CaptionButtonState btnState)
		{
			if( btnState == CaptionButtonState.Pushed )
			{
				return m_PushedButtonImagePen;
			}
			else
			{
				return m_ButtonImagePen;
			}
		}


		protected override void PaintCaption( Graphics g, PaintDockControlArgs args )
		{
			base.PaintCaption( g, args );	
			PaintGripper( g );
		}
		
		protected void PaintGripper( Graphics g )
		{
			if( Layout.GripperRectangle != Rectangle.Empty )
			{
				RendererPrimitives.PaintGripper( g, Orientation.Vertical, 
					Layout.GripperRectangle.Location, m_GripperBitmap, CaptionWidth / 6 );
			}
		}


		protected override void CreateGraphicObjects( bool recreate )
		{
			m_GraphicObjectsCreated = !m_GraphicObjectsCreated || recreate;
			if( m_GraphicObjectsCreated )
			{
				m_OuterBorderBrush = new SolidBrush( ColorTable.OuterBorderColor );
				m_InnerBorderBrush = new SolidBrush( ColorTable.InnerBorderColor );

				CreateGripperBitmap();
				m_ButtonBorderPen = new Pen( ColorTable.ButtonBorderColor );
				m_ButtonImagePen = new Pen( ColorTable.ButtonImageColor );
				m_PushedButtonImagePen = new Pen( ColorTable.PushedButtonImageColor );

				Rectangle buttonBrushRect = ( Layout.Table.Buttons.Count > 0 )?
					Layout.GetButtonBounds(0): 
					Rectangle.Empty;

				if( buttonBrushRect != Rectangle.Empty )
				{
					m_ActiveButtonBrush = new LinearGradientBrush( buttonBrushRect,
						ColorTable.ActiveButtonTopColor, ColorTable.ActiveButtonBottomColor,
						LinearGradientMode.Vertical );

					m_PushedButtonBrush = new LinearGradientBrush( buttonBrushRect,
						ColorTable.PushedButtonTopColor, ColorTable.PushedButtonBottomColor,
						LinearGradientMode.Vertical );
				}

				m_TextBrush = new SolidBrush( ColorTable.TextColor );

				m_SplitterBrush = new SolidBrush( ColorTable.CaptionTopColor );

				m_GraphicObjectsCreated = true;

				CreateCaptionBrushes(recreate);
			}
		}

		protected void CreateGripperBitmap()
		{
			m_GripperBitmap = new Bitmap(4,4);
			m_GripperBitmap.MakeTransparent();
			m_GripperBitmap.SetPixel(0,0,ColorTable.GripperForegroundColor);
			m_GripperBitmap.SetPixel(0,1,ColorTable.GripperForegroundColor);
			m_GripperBitmap.SetPixel(1,0,ColorTable.GripperForegroundColor);
			m_GripperBitmap.SetPixel(1,1,ColorTable.GripperForegroundColor);
			m_GripperBitmap.SetPixel(1,2,ColorTable.GripperBackgroundColor);
			m_GripperBitmap.SetPixel(2,1,ColorTable.GripperBackgroundColor);
			m_GripperBitmap.SetPixel(2,2,ColorTable.GripperBackgroundColor);
		}

		protected override void DisposeBrushes()
		{
			if( m_ActiveButtonBrush != null )
				m_ActiveButtonBrush.Dispose();
			if( m_InnerBorderBrush != null )
				m_InnerBorderBrush.Dispose();
			if( m_OuterBorderBrush != null )
				m_OuterBorderBrush.Dispose();
			if( m_PushedButtonBrush != null )
				m_PushedButtonBrush.Dispose();
			if( m_SplitterBrush != null )
				m_SplitterBrush.Dispose();
			if( m_TextBrush != null )
				m_TextBrush.Dispose();
			if(m_GripperBitmap != null)
				m_GripperBitmap.Dispose();
            
            m_ActiveButtonBrush = null;
            m_OuterBorderBrush = null;
            m_PushedButtonBrush = null;
            m_SplitterBrush = null;
            m_TextBrush = null;
            m_ButtonImagePen = null;
            m_SplitterBrush = null;

			DisposeCaptionBrushes();
			m_GraphicObjectsCreated = false;
		}

		protected override void CreateCaptionBrushes(bool recreate)
		{
			if( Layout.CaptionBounds.Width != 0 && Layout.CaptionBounds.Height != 0 )
			{
				if (m_prevCaptionRect.Location != Layout.CaptionBounds.Location
					|| m_prevCaptionRect.Height != Layout.CaptionBounds.Height || recreate)
				{
					m_CaptionBrush = new LinearGradientBrush(Layout.CaptionBounds,
						ColorTable.CaptionTopColor, ColorTable.CaptionBottomColor,
						LinearGradientMode.Vertical);

					m_ActiveCaptionBrush = new LinearGradientBrush(Layout.CaptionBounds,
						ColorTable.ActiveCaptionTopColor, ColorTable.ActiveCaptionBottomColor,
						LinearGradientMode.Vertical);

					m_prevCaptionRect = Layout.CaptionBounds;
				}
			}
		}

		protected override void DisposeCaptionBrushes()
		{
			if( m_ActiveCaptionBrush != null )
				m_ActiveCaptionBrush.Dispose();
			if( m_CaptionBrush != null )
				m_CaptionBrush.Dispose();
		}


		#endregion

		#region Private members

		private Brush m_ActiveCaptionBrush;
		private Brush m_CaptionBrush;
		private Brush m_OuterBorderBrush;
		private Brush m_InnerBorderBrush;
		private Pen m_ButtonBorderPen;
		private Pen m_ButtonImagePen;
		private Pen m_PushedButtonImagePen;
		private Brush m_ActiveButtonBrush;
		private Brush m_PushedButtonBrush;
		private Brush m_TextBrush;
		private Brush m_SplitterBrush;
		private Bitmap m_GripperBitmap;

		private bool m_GraphicObjectsCreated;

		#endregion

		#region Properties

		protected new LayoutOffice2003 Layout
		{
			get { return base.Layout as LayoutOffice2003; }
			set { base.Layout = value; }
		}

		protected new ColorTableOffice2003 ColorTable
		{
			get
			{
				return base.ColorTable as ColorTableOffice2003;
			}
			set
			{
				base.ColorTable = value;
			}
		}

		#endregion
	}
    internal class RendererMetro : CustomRenderer
    {
        #region Constructors
        #region Private members

        private int m_CaptionHeight = SystemInformation.ToolWindowCaptionHeight;
        private bool m_PrevFloating;      

        #endregion
        public Color MetroColor = ColorTranslator.FromHtml("#119EDA");

        public Color ActiveCaption = Color.White;
        public Color ButtonColor = Color.FromArgb(255, 255, 255);
        public Color SplitterColor = ColorTranslator.FromHtml("#9B9FB7");
        public Color BorderColor = Color.Empty;
        #region Class constants

        private int DEF_CAPTION_OFFSET = 1;

        #endregion

        public RendererMetro()
        {
            ColorTable = new ColorTableVS2012();
            Layout = new LayoutVS2012();
            CreateGraphicObjects(false);
        }


        #endregion

        #region Methods
        
        public override int ThickBorderWidth
        {
            get { return 1; }
        }
        protected override void CreateGraphicObjects(bool recreate)
        {
            m_GraphicObjectsCreated = !m_GraphicObjectsCreated || recreate;
            if (m_GraphicObjectsCreated)
            {
                m_BorderBrush = new SolidBrush(ColorTable.BorderColor);
                m_InnerBorderBrush = new SolidBrush(ColorTable.InnerBorderColor);

                m_ButtonBorderPen = new Pen(ColorTable.ButtonBorderColor);
                m_ActiveButtonBorderPen = new Pen(Color.FromArgb(100,MetroColor ));

                m_ButtonImagePen = new Pen(ColorTable.ButtonImageColor);
                if (ButtonColor != Color.FromArgb(255, 255, 255))
                {
                    m_ActiveButtonImagePen = new Pen(ButtonColor);
                }
                else
                    m_ActiveButtonImagePen = new Pen(Color.FromArgb(255, 255, 255));
                Rectangle buttonBrushRect = (Layout.Table.Buttons.Count > 0) ?
                    Layout.GetButtonBounds(0) :
                    Rectangle.Empty;

                if (buttonBrushRect != Rectangle.Empty)
                {
                    m_ButtonBrush = new SolidBrush(ColorTable.ButtonColor);
                    m_ActiveButtonBrush = new SolidBrush(Color.FromArgb(100, MetroColor));
                    m_PushedButtonBrush = new SolidBrush(Color.FromArgb(14, 97, 152));
                }

                m_TextBrush = new SolidBrush(ColorTable.TextColor);

                m_ActiveTextBrush = new SolidBrush(ColorTable.ActiveTextColor);

                m_SplitterBrush = new SolidBrush(ColorTable.SplitterColor);

                m_GraphicObjectsCreated = true;

                CreateCaptionBrushes(recreate);
            }
        }
        Color m_metroColor = Color.Empty;
        protected override void CreateCaptionBrushes(bool recreate)
        {
            if (Layout.CaptionBounds.Width != 0 && Layout.CaptionBounds.Height != 0)
            {
                if (m_prevCaptionRect.Location != Layout.CaptionBounds.Location
                    || m_prevCaptionRect.Height != Layout.CaptionBounds.Height || (recreate||m_metroColor!=MetroColor))
                {
                    m_metroColor=MetroColor;
                    m_CaptionBrush = new LinearGradientBrush(Layout.CaptionBounds,
                        ColorTable.CaptionTopColor, ColorTable.CaptionBottomColor,
                        LinearGradientMode.Vertical);

                    m_ActiveCaptionBrush = new SolidBrush(m_metroColor);

                    m_prevCaptionRect = Layout.CaptionBounds;
                    m_bNeedCreateBrushes = false;
                }
            }
            else
            {
                //if (m_prevCaptionRect.Location != Layout.CaptionBounds.Location
                //    || m_prevCaptionRect.Height != Layout.CaptionBounds.Height || recreate)
                //{
                //    m_CaptionBrush = new LinearGradientBrush(Layout.CaptionBounds,
                //        ColorTable.CaptionTopColor, ColorTable.CaptionBottomColor,
                //        LinearGradientMode.Vertical);

                //    m_ActiveCaptionBrush = new LinearGradientBrush(Layout.CaptionBounds,
                //        ColorTable.ActiveCaptionTopColor, ColorTable.ActiveCaptionBottomColor,
                //        LinearGradientMode.Vertical);

                //    m_prevCaptionRect = Layout.CaptionBounds;
                //    m_bNeedCreateBrushes = false;
                //}
                if (recreate)
                    m_bNeedCreateBrushes = true;
            }
        }

        protected override void DisposeBrushes()
        {
            if (m_ActiveButtonBrush != null)
                m_ActiveButtonBrush.Dispose();
            if (m_ButtonBrush != null)
                m_ButtonBrush.Dispose();
            if (m_BorderBrush != null)
                m_BorderBrush.Dispose();
            if (m_PushedButtonBrush != null)
                m_PushedButtonBrush.Dispose();
            if (m_SplitterBrush != null)
                m_SplitterBrush.Dispose();
            if (m_ActiveTextBrush != null)
                m_ActiveTextBrush.Dispose();
            if (m_TextBrush != null)
                m_TextBrush.Dispose();
            if (m_ButtonBorderPen != null)
                m_ButtonBorderPen.Dispose();
            if (m_ActiveButtonBorderPen != null)
                m_ActiveButtonBorderPen.Dispose();
            if (m_SplitterBrush != null)
                m_SplitterBrush.Dispose();
            if (m_ButtonImagePen != null)
                m_ButtonImagePen.Dispose();
            if (m_ActiveButtonImagePen != null)
                m_ActiveButtonImagePen.Dispose();

            m_ActiveButtonBrush = null;
            m_ButtonBrush = null;
            m_PushedButtonBrush = null;
            m_SplitterBrush = null;
            m_TextBrush = null;
            m_ButtonImagePen = null;
            m_ActiveButtonImagePen = null;
            m_SplitterBrush = null;

            DisposeCaptionBrushes();
            m_GraphicObjectsCreated = false;
        }
        protected override void DisposeCaptionBrushes()
        {
            if (m_ActiveCaptionBrush != null)
                m_ActiveCaptionBrush.Dispose();
            if (m_CaptionBrush != null)
                m_CaptionBrush.Dispose();
        }

        protected override Brush GetCaptionBrush(CaptionState cpState)
        {
            if (cpState == CaptionState.Normal)
            {
                return m_CaptionBrush;
            }
            else
            {
                return m_ActiveCaptionBrush;

            }
        }

        protected override Brush GetTextBrush(CaptionState cpState)
        {
            if (cpState == CaptionState.Normal)
            {
                return m_TextBrush;
            }
            else
            {
                return m_ActiveTextBrush;
            }
        }

        protected override Brush GetBorderBrush(bool innerBorder)
        {
            if (innerBorder)
            {
                return m_InnerBorderBrush;
            }
            else
            {
                return m_BorderBrush;
            }
        }

        protected override Brush GetSplitterBrush()
        {
            return m_SplitterBrush;
        }

        protected override Brush GetButtonBrush(CaptionState cpState, CaptionButtonState btnState)
        {
            if (cpState == CaptionState.Active)
            {
                if (btnState == CaptionButtonState.Active)
                {
                    return m_ActiveButtonBrush;
                }
                else
                {
                    return m_PushedButtonBrush;
                }
            }
            else
            {
                return m_ButtonBrush;
            }
        }
        protected override Pen GetButtonPen(CaptionState cpState, CaptionButtonState btnState)
        {
            if (cpState == CaptionState.Active)
            {
                return m_ActiveButtonBorderPen;
            }
            else
            {
                return m_ButtonBorderPen;
            }
        }

        protected override Pen GetButtonImagePen(CaptionState cpState, CaptionButtonState btnState)
        {
            if (cpState == CaptionState.Active)
            {
                return m_ActiveButtonImagePen;
            }
            else
            {
                return m_ButtonImagePen;
            }
        }


        #endregion

        #region Private members

        private Brush m_CaptionBrush;
        private Brush m_ActiveCaptionBrush;
        private Brush m_BorderBrush;
        private Brush m_InnerBorderBrush;
        private Pen m_ButtonBorderPen;
        private Pen m_ActiveButtonBorderPen;
        private Pen m_ButtonImagePen;
        private Pen m_ActiveButtonImagePen;
        private Brush m_ButtonBrush;
        private Brush m_ActiveButtonBrush;
        private Brush m_PushedButtonBrush;
        private Brush m_TextBrush;
        private Brush m_ActiveTextBrush;
        private Brush m_SplitterBrush;

        private bool m_GraphicObjectsCreated;

        #endregion

        #region Properties

        protected new LayoutVS2012 Layout
        {
            get { return base.Layout as LayoutVS2012; }
            set { base.Layout = value; }
        }

        protected new ColorTableVS2012 ColorTable
        {
            get
            {
                return base.ColorTable as ColorTableVS2012;
            }
            set
            {
                base.ColorTable = value;
            }
        }

        #endregion
        public override void PaintSplitter(Graphics g, Rectangle rectangle,
            Orientation orientation)
        {
            Brush splitterBrush = GetSplitterBrush();
            if (orientation == Orientation.Vertical)
            {
                Rectangle r1 = new Rectangle(rectangle.X, rectangle.Y, 2, rectangle.Height / 2);
                Rectangle r2 = new Rectangle(rectangle.X, (rectangle.Height / 2), 2, rectangle.Height / 2);
                Rectangle r3 = new Rectangle(rectangle.X, (rectangle.Height / 2) - 1, 2, rectangle.Height / 2);
                if (this.SplitterColor == ColorTranslator.FromHtml("#9B9FB7"))
                {
                    RendererPrimitives.PaintBackground(g, r1, new LinearGradientBrush(r1, SystemColors.Control, ColorTranslator.FromHtml("#9B9FB7"), LinearGradientMode.Vertical));
                    RendererPrimitives.PaintBackground(g, r2, new LinearGradientBrush(r3, ColorTranslator.FromHtml("#9B9FB7"), SystemColors.Control, LinearGradientMode.Vertical));
                }
                else
                    RendererPrimitives.PaintBackground(g, rectangle, new SolidBrush(SplitterColor));
            }
            else
            {
                RendererPrimitives.PaintBackground(g, rectangle, new SolidBrush(SplitterColor));
            }
            // g.DrawLine(Pens.Gray , new Point(rectangle.X, r2.Y), new Point(r2.Width, r2.Y));
        }
        public override void PaintDockedControl(Graphics g, Rectangle rectangle,
        PaintDockControlArgs args)
        {
            RefreshPaintInfo(rectangle, args);
            CreateGraphicObjects(false);

            if (args.PaintBorders)
            {
                PaintBorders(g, args);
            }
            if (args.Caption != null && args.Caption.Enabled)
            {
                if (Layout.CaptionBounds.Width != 0 && Layout.CaptionBounds.Height != 0)
                {
                    if (m_bNeedCreateBrushes)
                        CreateCaptionBrushes(true);
                    PaintCaption(g, args);
                }
            }
        }
        protected override void DrawText(Graphics g, PaintDockControlArgs args)
        {
            Layout.TextRectHeight = args.Caption.Font.Height + DEF_CAPTION_OFFSET;
            if (Layout.TextBounds != Rectangle.Empty)
            {
                StringFormat format = new StringFormat();
                format.Trimming = StringTrimming.EllipsisCharacter;
                format.Alignment = GetTextAlignment(args.Caption.TextAlignment);
                format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;

                if (this.IsMirrored)
                    format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;

                Font ftCaption;
                if (args.ProvideGraphicsItemsArgs != null &&
                    args.ProvideGraphicsItemsArgs.CaptionFont != null)
                {
                    ftCaption = args.ProvideGraphicsItemsArgs.CaptionFont;
                }
                else
                {
                    ftCaption = args.Caption.Font;
                }
                if (Layout is LayoutVS2012)
                {
                    float[] dashValues = { 1, 3, 1, 3 };
                    Pen blackPen = null;

                    if (args.Caption.CaptionState == CaptionState.Active)
                    {
                        if (ActiveCaption != Color.White)
                            blackPen = new Pen(ActiveCaption,1);
						else
                            blackPen = new Pen(Color.White, 1);
                    }
                    else
                        blackPen = new Pen(Color.Gray, 1);
                    SizeF fontsize = g.MeasureString(args.Caption.Text, ftCaption);
                    int myInt = (int)Math.Ceiling(fontsize.Width);
                    blackPen.DashPattern = dashValues;
                    int s = args.Table.Buttons.Count;
                    int v = s * 17;
                    int captionwidth = 0;
                    Rectangle r = new Rectangle();
                    if (Layout.Floating)
                        r = new Rectangle(Layout.TextBounds.X, Layout.TextBounds.Y - 3, Layout.TextBounds.Width, Layout.TextBounds.Height);
                    if (Layout.Floating)
                    {
                        if (this.Layout.TextBounds.Width > fontsize.Width)
                        {
                            captionwidth = (r.Height / 2) + r.Y;
                            g.DrawLine(blackPen, new Point(myInt + r.X, captionwidth - 2), new Point(r.Width, captionwidth - 2));
                            g.DrawLine(blackPen, new Point(myInt + r.X + 2, captionwidth), new Point(r.Width, captionwidth));
                            g.DrawLine(blackPen, new Point(myInt + r.X, captionwidth + 2), new Point(r.Width, captionwidth + 2));
                        }
                    }
                    else
                    {
                        if (this.Layout.TextBounds.Width > fontsize.Width)
                        {
                            captionwidth = (Layout.TextBounds.Height / 2) + Layout.TextBounds.Y;
                            g.DrawLine(blackPen, new Point(myInt + Layout.TextBounds.X, captionwidth - 2), new Point(Layout.TextBounds.Width, captionwidth - 2));
                            g.DrawLine(blackPen, new Point(myInt + Layout.TextBounds.X + 2, captionwidth), new Point(Layout.TextBounds.Width, captionwidth));
                            g.DrawLine(blackPen, new Point(myInt + Layout.TextBounds.X, captionwidth + 2), new Point(Layout.TextBounds.Width, captionwidth + 2));
                        }
                    }
                    blackPen.Dispose();
                }
                Brush textBrush = GetTextBrush(args.Caption.CaptionState);
                if (args.ProvideGraphicsItemsArgs != null && args.ProvideGraphicsItemsArgs.CaptionForeground != Color.Empty)
                    textBrush = new SolidBrush(args.ProvideGraphicsItemsArgs.CaptionForeground);
                Rectangle rect;
                if(Layout.Floating)
                    rect = new Rectangle(Layout.TextBounds.X, Layout.TextBounds.Y - 3, Layout.TextBounds.Width, Layout.TextBounds.Height);
                else
                    rect = new Rectangle(Layout.TextBounds.X, Layout.TextBounds.Y, Layout.TextBounds.Width, Layout.TextBounds.Height);
                RendererPrimitives.DrawText(g, args.Caption.Text, ftCaption,
                    textBrush, rect, format);
            }
        }
        protected override void PaintCaption(Graphics g, PaintDockControlArgs args)
        {
            if (m_CaptionHeight != Layout.CaptionWidth || m_PrevFloating != Layout.Floating)
            {
                m_PrevFloating = Layout.Floating;
                m_CaptionHeight = Layout.CaptionWidth;
                RefreshCaptionBrushes();
            }

            Brush captionBrush;
            if (args.ProvideGraphicsItemsArgs != null &&
                args.ProvideGraphicsItemsArgs.CaptionBackground != null)
            {
                captionBrush = args.ProvideGraphicsItemsArgs.CaptionBackground;
            }
            else
            {
                captionBrush = GetCaptionBrush(args.Caption.CaptionState);
            }
            Rectangle r = new Rectangle(Layout.CaptionBounds.X, Layout.CaptionBounds.Y, Layout.CaptionBounds.Width, Layout.CaptionBounds.Height - Layout.CaptionBounds.Y);
            r.Inflate(+3, +3);
            r.Height -= 2;
            RendererPrimitives.PaintBackground(g, r, captionBrush);
            PaintButtons(g, args);
            DrawText(g, args);
            if (Layout.ImageEnabled)
            {
                g.DrawImage(GetImageFromList(args.CaptionImageIndex, args.ImageList), Layout.ImageBounds);
            }
        }
        protected override void PaintBorders(Graphics g, PaintDockControlArgs args)
        {
            Brush innerBrush;
            Brush outerBrush;
            if (args.Floating)
            {
                if (args.Caption.CaptionState == CaptionState.Active)
                {
                    innerBrush = new SolidBrush(MetroColor);
                    outerBrush = new SolidBrush(MetroColor);
                }
                else
                {
                    innerBrush = new SolidBrush(Color.FromArgb(239, 238, 235));
                    outerBrush = new SolidBrush(Color.FromArgb(114, 114, 114));
                }
            }
            else
            {
                if (args.ProvideGraphicsItemsArgs != null &&
                    args.ProvideGraphicsItemsArgs.CaptionForeground != Color.Empty)
                {
                    innerBrush = new SolidBrush(args.ProvideGraphicsItemsArgs.CaptionForeground);
                    if(BorderColor == Color.Empty)
                        outerBrush = new SolidBrush(args.ProvideGraphicsItemsArgs.CaptionForeground);
                    else
                        outerBrush = new SolidBrush(BorderColor);
                }
                else
                {
                    innerBrush = GetBorderBrush(true);
                    if (BorderColor == Color.Empty)
                        outerBrush = GetBorderBrush(false);
                    else
                        outerBrush = new SolidBrush(BorderColor);
                }
            }

            RendererPrimitives.PaintBorders(g, Layout.ControlBounds,
                outerBrush, Layout.OuterBorderWidth);

        }
    }
	internal class RendererVS2005 : CustomRenderer
	{
		#region Constructors

		public RendererVS2005 ()
		{
			ColorTable = new ColorTableVS2005();
			Layout = new LayoutVS2005();
			CreateGraphicObjects( false );
		}


		#endregion

		#region Methods

		protected override void CreateGraphicObjects(bool recreate)
		{
			m_GraphicObjectsCreated = !m_GraphicObjectsCreated || recreate;
			if( m_GraphicObjectsCreated )
			{
				m_BorderBrush = new SolidBrush( ColorTable.BorderColor );
				m_InnerBorderBrush = new SolidBrush( ColorTable.InnerBorderColor );

				m_ButtonBorderPen = new Pen( ColorTable.ButtonBorderColor );
				m_ActiveButtonBorderPen = new Pen( ColorTable.ActiveButtonBorderColor );
				
				m_ButtonImagePen = new Pen( ColorTable.ButtonImageColor );
				m_ActiveButtonImagePen = new Pen( ColorTable.ActiveButtonImageColor );

				Rectangle buttonBrushRect = ( Layout.Table.Buttons.Count > 0 )?
					Layout.GetButtonBounds(0): 
					Rectangle.Empty;

				if( buttonBrushRect != Rectangle.Empty )
				{
					m_ButtonBrush = new SolidBrush( ColorTable.ButtonColor );
					m_ActiveButtonBrush = new SolidBrush( ColorTable.ActiveButtonColor );
					m_PushedButtonBrush = new SolidBrush( ColorTable.PushedButtonColor );
				}

				m_TextBrush = new SolidBrush( ColorTable.TextColor );

				m_ActiveTextBrush = new SolidBrush( ColorTable.ActiveTextColor );

				m_SplitterBrush = new SolidBrush( ColorTable.SplitterColor );

				m_GraphicObjectsCreated = true;

				CreateCaptionBrushes(recreate);
			}
		}

		protected override void CreateCaptionBrushes(bool recreate)
		{
			if( Layout.CaptionBounds.Width != 0 && Layout.CaptionBounds.Height != 0 )
			{
				if( m_prevCaptionRect.Location != Layout.CaptionBounds.Location
					|| m_prevCaptionRect.Height != Layout.CaptionBounds.Height || recreate )
				{
					m_CaptionBrush = new LinearGradientBrush( Layout.CaptionBounds,
						ColorTable.CaptionTopColor, ColorTable.CaptionBottomColor,
						LinearGradientMode.Vertical );

					m_ActiveCaptionBrush = new LinearGradientBrush( Layout.CaptionBounds,
						ColorTable.ActiveCaptionTopColor, ColorTable.ActiveCaptionBottomColor,
						LinearGradientMode.Vertical );

					m_prevCaptionRect = Layout.CaptionBounds;
					m_bNeedCreateBrushes = false;
				}
			}
			else
				if( recreate )
					m_bNeedCreateBrushes = true;
		}

		protected override void DisposeBrushes()
		{
			if( m_ActiveButtonBrush != null )
				m_ActiveButtonBrush.Dispose();
			if( m_ButtonBrush != null )
				m_ButtonBrush.Dispose();
			if( m_BorderBrush != null )
				m_BorderBrush.Dispose();
			if( m_PushedButtonBrush != null )
				m_PushedButtonBrush.Dispose();
			if( m_SplitterBrush != null )
				m_SplitterBrush.Dispose();
			if( m_ActiveTextBrush != null )
				m_ActiveTextBrush.Dispose();
			if( m_TextBrush != null )
				m_TextBrush.Dispose();
			if( m_ButtonBorderPen != null )
				m_ButtonBorderPen.Dispose();
			if( m_ActiveButtonBorderPen != null )
				m_ActiveButtonBorderPen.Dispose();
			if( m_SplitterBrush != null )
				m_SplitterBrush.Dispose();
			if( m_ButtonImagePen != null )
				m_ButtonImagePen.Dispose();
			if( m_ActiveButtonImagePen != null )
				m_ActiveButtonImagePen.Dispose();

            m_ActiveButtonBrush = null;
            m_ButtonBrush = null;
            m_PushedButtonBrush = null;
            m_SplitterBrush = null;
            m_TextBrush = null;
            m_ButtonImagePen = null;
            m_ActiveButtonImagePen = null;
            m_SplitterBrush = null;

			DisposeCaptionBrushes();
			m_GraphicObjectsCreated = false;
		}
		protected override void DisposeCaptionBrushes()
		{
			if( m_ActiveCaptionBrush != null )
				m_ActiveCaptionBrush.Dispose();
			if( m_CaptionBrush != null )
				m_CaptionBrush.Dispose();
		}

		protected override Brush GetCaptionBrush( CaptionState cpState )
		{
			if( cpState == CaptionState.Normal )
			{
				return m_CaptionBrush;
			}
			else
			{
				return m_ActiveCaptionBrush;

			}
		}

		protected override Brush GetTextBrush(CaptionState cpState)
		{
			if( cpState == CaptionState.Normal )
			{
				return m_TextBrush;
			}
			else
			{
				return m_ActiveTextBrush;
			}
		}

		protected override Brush GetBorderBrush(bool innerBorder)
		{
			if( innerBorder )
			{
				return m_InnerBorderBrush;
			}
			else
			{
				return m_BorderBrush;
			}
		}

		protected override Brush GetSplitterBrush()
		{
			return m_SplitterBrush;
		}

		protected override Brush GetButtonBrush(CaptionState cpState, CaptionButtonState btnState)
		{
			if( cpState == CaptionState.Active )
			{
				if( btnState == CaptionButtonState.Active )
				{
					return m_ActiveButtonBrush;
				}
				else
				{
					return m_PushedButtonBrush;
				}
			}
			else
			{
				return m_ButtonBrush;
			}
		}
		protected override Pen GetButtonPen(CaptionState cpState, CaptionButtonState btnState)
		{
			if( cpState == CaptionState.Active )
			{
				return m_ActiveButtonBorderPen;
			}
			else
			{
				return m_ButtonBorderPen;
			}
		}

		protected override Pen GetButtonImagePen(CaptionState cpState, CaptionButtonState btnState)
		{
			if( cpState == CaptionState.Active )
			{
				return m_ActiveButtonImagePen;
			}
			else
			{
				return m_ButtonImagePen;
			}
		}


		#endregion

		#region Private members

		private Brush m_CaptionBrush;
		private Brush m_ActiveCaptionBrush;
		private Brush m_BorderBrush;
		private Brush m_InnerBorderBrush;
		private Pen m_ButtonBorderPen;
		private Pen m_ActiveButtonBorderPen;
		private Pen m_ButtonImagePen;
		private Pen m_ActiveButtonImagePen;
		private Brush m_ButtonBrush;
		private Brush m_ActiveButtonBrush;
		private Brush m_PushedButtonBrush;
		private Brush m_TextBrush;
		private Brush m_ActiveTextBrush;
		private Brush m_SplitterBrush;

		private bool m_GraphicObjectsCreated;

		#endregion

		#region Properties

		protected new LayoutVS2005 Layout
		{
			get { return base.Layout as LayoutVS2005; }
			set { base.Layout = value; }
		}

		protected new ColorTableVS2005 ColorTable
		{
			get
			{
				return base.ColorTable as ColorTableVS2005;
			}
			set
			{
				base.ColorTable = value;
			}
		}

		#endregion
	}

    /// <summary>
    /// Renderer class for visual studio 2010 theme
    /// </summary>
    internal class RendererVS2010 : CustomRenderer
    {
        #region Constructors

        public RendererVS2010()
        {
            ColorTable = new ColorTableVS2010();
            Layout = new LayoutVS2010();
            CreateGraphicObjects(false);
        }

        #endregion

        #region Methods

        public override void PaintSplitter(Graphics g, Rectangle rectangle,
            Orientation orientation)
        {
            base.PaintSplitter(g, rectangle, orientation);

            int dotCount = 6;
            int gripperWidth = dotCount * 4 - 1;
            Point point = Point.Empty;
            if (orientation == Orientation.Horizontal)
            {
                point.X = rectangle.Width / 2 - gripperWidth / 2;
                point.Y = (rectangle.Height - 3) / 2 + 1;
            }
            else
            {
                point.X = (rectangle.Width - 3) / 2 + 1;
                point.Y = rectangle.Height / 2 - gripperWidth / 2;
            }
            RendererPrimitives.PaintGripper(g, orientation, point, m_GripperBitmap, dotCount);
        }


        protected override Brush GetCaptionBrush(CaptionState cpState)
        {
            if (cpState == CaptionState.Normal)
            {
                return m_CaptionBrush;
            }
            else
            {
                return m_ActiveCaptionBrush;
            }
        }

        protected override Brush GetTextBrush(CaptionState cpState)
        {
            if (cpState == CaptionState.Active)
                return new SolidBrush(this.ColorTable.ActiveTextColor);
            else
                return new SolidBrush(Color.White);
        }

        protected override Brush GetBorderBrush(bool innerBorder)
        {
            if (innerBorder)
            {
                return m_InnerBorderBrush;
            }
            else
            {
                return m_OuterBorderBrush;
            }
        }

        protected override Brush GetSplitterBrush()
        {
            return m_SplitterBrush;
        }

        protected override Brush GetButtonBrush(CaptionState cpState, CaptionButtonState btnState)
        {
            if (btnState == CaptionButtonState.Active)
            {
                return m_ActiveButtonBrush;
            }
            else
            {
                return m_PushedButtonBrush;
            }
        }

        protected override Pen GetButtonPen(CaptionState cpState, CaptionButtonState btnState)
        {
            return m_ButtonBorderPen;
        }

        protected override Pen GetButtonImagePen(CaptionState cpState, CaptionButtonState btnState)
        {
            if (btnState == CaptionButtonState.Pushed)
            {
                return m_PushedButtonImagePen;
            }
            else
            {
                if(cpState == CaptionState.Normal)
                    return m_ButtonImagePen;
                else
                    return m_ButtonImagePen = new Pen(Color.FromArgb(117, 99, 61));
            }
        }


        protected override void PaintCaption(Graphics g, PaintDockControlArgs args)
        {
            base.PaintCaption(g, args);
            PaintGripper(g);
        }

        protected void PaintGripper(Graphics g)
        {
           
        }


        protected override void CreateGraphicObjects(bool recreate)
        {
            m_GraphicObjectsCreated = !m_GraphicObjectsCreated || recreate;
            if (m_GraphicObjectsCreated)
            {
                m_OuterBorderBrush = new SolidBrush(ColorTable.OuterBorderColor);
                m_InnerBorderBrush = new SolidBrush(ColorTable.InnerBorderColor);

                CreateGripperBitmap();
                m_ButtonBorderPen = new Pen(ColorTable.ButtonBorderColor);
                m_ButtonImagePen = new Pen(ColorTable.ButtonImageColor);
                m_PushedButtonImagePen = new Pen(ColorTable.PushedButtonImageColor);

                Rectangle buttonBrushRect = (Layout.Table.Buttons.Count > 0) ?
                    Layout.GetButtonBounds(0) :
                    Rectangle.Empty;

                if (buttonBrushRect != Rectangle.Empty)
                {
                    m_ActiveButtonBrush = new LinearGradientBrush(buttonBrushRect,
                        ColorTable.ActiveButtonTopColor, ColorTable.ActiveButtonBottomColor,
                        LinearGradientMode.Vertical);

                    m_PushedButtonBrush = new LinearGradientBrush(buttonBrushRect,
                        ColorTable.PushedButtonTopColor, ColorTable.PushedButtonBottomColor,
                        LinearGradientMode.Vertical);
                }

                m_TextBrush = new SolidBrush(this.ColorTable.TextColor);

                m_SplitterBrush = new SolidBrush(ColorTable.CaptionTopColor);

                m_GraphicObjectsCreated = true;

                CreateCaptionBrushes(recreate);
            }
        }

        protected void CreateGripperBitmap()
        {
            m_GripperBitmap = new Bitmap(4, 4);
            m_GripperBitmap.MakeTransparent();
            m_GripperBitmap.SetPixel(0, 0, ColorTable.GripperForegroundColor);
            m_GripperBitmap.SetPixel(0, 1, ColorTable.GripperForegroundColor);
            m_GripperBitmap.SetPixel(1, 0, ColorTable.GripperForegroundColor);
            m_GripperBitmap.SetPixel(1, 1, ColorTable.GripperForegroundColor);
            m_GripperBitmap.SetPixel(1, 2, ColorTable.GripperBackgroundColor);
            m_GripperBitmap.SetPixel(2, 1, ColorTable.GripperBackgroundColor);
            m_GripperBitmap.SetPixel(2, 2, ColorTable.GripperBackgroundColor);
        }

        protected override void DisposeBrushes()
        {
            if (m_ActiveButtonBrush != null)
                m_ActiveButtonBrush.Dispose();
            if (m_InnerBorderBrush != null)
                m_InnerBorderBrush.Dispose();
            if (m_OuterBorderBrush != null)
                m_OuterBorderBrush.Dispose();
            if (m_PushedButtonBrush != null)
                m_PushedButtonBrush.Dispose();
            if (m_SplitterBrush != null)
                m_SplitterBrush.Dispose();
            if (m_TextBrush != null)
                m_TextBrush.Dispose();
            if (m_GripperBitmap != null)
                m_GripperBitmap.Dispose();

            m_ActiveButtonBrush = null;
            m_OuterBorderBrush = null;
            m_PushedButtonBrush = null;
            m_SplitterBrush = null;
            m_TextBrush = null;
            m_ButtonImagePen = null;
            m_SplitterBrush = null;

            DisposeCaptionBrushes();
            m_GraphicObjectsCreated = false;
        }

        protected override void CreateCaptionBrushes(bool recreate)
        {
            if (Layout.CaptionBounds.Width != 0 && Layout.CaptionBounds.Height != 0)
            {
                if (m_prevCaptionRect.Location != Layout.CaptionBounds.Location
                    || m_prevCaptionRect.Height != Layout.CaptionBounds.Height || recreate)
                {
                    m_CaptionBrush = new LinearGradientBrush(Layout.CaptionBounds,
                        ColorTable.CaptionTopColor, ColorTable.CaptionBottomColor,
                        LinearGradientMode.Vertical);

                    m_ActiveCaptionBrush = new LinearGradientBrush(Layout.CaptionBounds,
                        ColorTable.ActiveCaptionTopColor, ColorTable.ActiveCaptionBottomColor,
                        LinearGradientMode.Vertical);

                    m_prevCaptionRect = Layout.CaptionBounds;
                }
            }
        }

        protected override void DisposeCaptionBrushes()
        {
            if (m_ActiveCaptionBrush != null)
                m_ActiveCaptionBrush.Dispose();
            if (m_CaptionBrush != null)
                m_CaptionBrush.Dispose();
        }


        #endregion

        #region Private members

        private Brush m_ActiveCaptionBrush;
        private Brush m_CaptionBrush;
        private Brush m_OuterBorderBrush;
        private Brush m_InnerBorderBrush;
        private Pen m_ButtonBorderPen;
        private Pen m_ButtonImagePen;
        private Pen m_PushedButtonImagePen;
        private Brush m_ActiveButtonBrush;
        private Brush m_PushedButtonBrush;
        private Brush m_TextBrush;
        private Brush m_SplitterBrush;
        private Bitmap m_GripperBitmap;

        private bool m_GraphicObjectsCreated;

        #endregion

        #region Properties

        protected new LayoutVS2010 Layout
        {
            get { return base.Layout as LayoutVS2010; }
            set { base.Layout = value; }
        }

        protected new ColorTableVS2010 ColorTable
        {
            get
            {
                return base.ColorTable as ColorTableVS2010;
            }
            set
            {
                base.ColorTable = value;
            }
        }

        #endregion
    }

	internal abstract class RendererOffice2007Base : CustomRenderer
	{
		#region Properties

		protected new virtual LayoutOffice2007Base Layout
		{
			get
			{
				return base.Layout as LayoutOffice2007Base;
			}
			set
			{
				base.Layout = value;
			}
		}
		protected new virtual ColorTableOffice2007Base ColorTable
		{
			get
			{
				return base.ColorTable as ColorTableOffice2007Base;
			}
			set
			{
				base.ColorTable = value;
			}
		}

		#endregion

		#region Methods

		protected override void PaintCaption(Graphics g, PaintDockControlArgs args)
		{
			// Painting captions in Office2007-like renderers differs from others
			// so calling method from base class is undesired.
			if (CaptionHeight != Layout.CaptionWidth || PrevFloating != Layout.Floating)
			{
				PrevFloating = Layout.Floating;
				CaptionHeight = Layout.CaptionWidth;
				RefreshCaptionBrushes();
			}

			Brush captionUpperBrush = null;
			Brush captionLowerBrush = null;

			try
			{
				if( args.ProvideGraphicsItemsArgs != null &&
					( args.ProvideGraphicsItemsArgs.CaptionBackground is SolidBrush	) )
                {
                    Brush brush = new LinearGradientBrush( Layout.CaptionUpperBounds,
						( ( args.ProvideGraphicsItemsArgs.CaptionBackground as System.Drawing.Brush ) as System.Drawing.SolidBrush ).Color,
						( ( args.ProvideGraphicsItemsArgs.CaptionBackground as System.Drawing.Brush ) as System.Drawing.SolidBrush ).Color,
						LinearGradientMode.Vertical );
                   ( brush as LinearGradientBrush ).WrapMode = WrapMode.TileFlipXY;

                   captionUpperBrush = brush;
                   captionLowerBrush = brush;
                }
                else
                {
					if( args.ProvideGraphicsItemsArgs != null &&
						args.ProvideGraphicsItemsArgs.CaptionBackground != null )
					{
						captionUpperBrush = args.ProvideGraphicsItemsArgs.CaptionBackground;
						captionLowerBrush = args.ProvideGraphicsItemsArgs.CaptionBackground;
					}
					else
					{
						captionUpperBrush = GetCaptionUpperBrush( args.Caption.CaptionState );
						captionLowerBrush = GetCaptionLowerBrush( args.Caption.CaptionState );
					}
				}

				RendererPrimitives.PaintBackground( g, Layout.CaptionUpperBounds, captionUpperBrush );
				RendererPrimitives.PaintBackground( g, Layout.CaptionLowerBounds, captionLowerBrush );
			}
			finally
			{
				if( captionLowerBrush != null )
					captionLowerBrush.Dispose();
				if( captionUpperBrush != null )
					captionUpperBrush.Dispose();
			}

			PaintButtons(g, args);
			DrawText(g, args);
			if (Layout.ImageEnabled)
			{
				g.DrawImage(GetImageFromList(args.CaptionImageIndex, args.ImageList), Layout.ImageBounds);
			}
		}

		public override void RefreshOffice2007Theme(Office2007Theme newTheme)
		{
			base.RefreshOffice2007Theme(newTheme);

			if (newTheme != m_prevTheme || newTheme == Office2007Theme.Managed)
			{
				// To ensure creation of new caption brushes, enable caption at that moment.
				bool prevCaptionEnabled = Layout.CaptionEnabled;
				Layout.CaptionEnabled = true;
				ColorTable.Office2007Theme = newTheme;
				RefreshBrushes(true);
				Layout.CaptionEnabled = prevCaptionEnabled;
				m_prevTheme = newTheme;
			}
		}
				
		protected override Brush GetCaptionBrush(CaptionState cpState)
		{
			return GetCaptionUpperBrush(cpState);
		}

		protected virtual Brush GetCaptionUpperBrush( CaptionState cpState )
		{
			if( cpState == CaptionState.Normal )
			{
				Brush brush = new LinearGradientBrush( Layout.CaptionUpperBounds,
					ColorTable.CaptionUpperTopColor, ColorTable.CaptionUpperBottomColor,
					LinearGradientMode.Vertical );
				( brush as LinearGradientBrush ).WrapMode = WrapMode.TileFlipXY;

				return brush;
			}
			else
			{
				Brush brush = new LinearGradientBrush( Layout.CaptionUpperBounds,
					ColorTable.ActiveCaptionUpperTopColor, ColorTable.ActiveCaptionUpperBottomColor,
					LinearGradientMode.Vertical );
				( brush as LinearGradientBrush ).WrapMode = WrapMode.TileFlipXY;

				return brush;
			}
		}

		protected virtual Brush GetCaptionLowerBrush( CaptionState cpState )
		{
			if( cpState == CaptionState.Normal )
			{
				Brush brush = new LinearGradientBrush( Layout.CaptionLowerBounds,
					ColorTable.CaptionLowerTopColor, ColorTable.CaptionLowerBottomColor,
					LinearGradientMode.Vertical );
				( brush as LinearGradientBrush ).WrapMode = WrapMode.TileFlipXY;

				return brush;
			}
			else
			{
				Brush brush = new LinearGradientBrush( Layout.CaptionLowerBounds,
					ColorTable.ActiveCaptionLowerTopColor, ColorTable.ActiveCaptionLowerBottomColor,
					LinearGradientMode.Vertical );
				( brush as LinearGradientBrush ).WrapMode = WrapMode.TileFlipXY;

				return brush;
			}
		}

		protected override void CreateGraphicObjects(bool recreate)
		{
			m_GraphicObjectsCreated = !m_GraphicObjectsCreated || recreate;
			if (m_GraphicObjectsCreated)
			{
				m_OuterBorderBrush = new SolidBrush(ColorTable.OuterBorderColor);
				m_InnerBorderBrush = new SolidBrush(ColorTable.InnerBorderColor);

				m_ButtonImagePen = new Pen(ColorTable.ButtonImageColor);
				m_ActiveButtonImagePen = new Pen(ColorTable.ActiveButtonImageColor);

				Rectangle buttonBrushRect = (Layout.Table.Buttons.Count > 0) ?
					Layout.GetButtonBounds(0) :
					Rectangle.Empty;

				if (buttonBrushRect != Rectangle.Empty)
				{
					m_ButtonBrush = new SolidBrush(ColorTable.ButtonColor);
					m_ActiveButtonBrush = new SolidBrush(ColorTable.ActiveButtonColor);
					m_PushedButtonBrush = new SolidBrush(ColorTable.PushedButtonColor);
				}

				m_TextBrush = new SolidBrush(ColorTable.TextColor);

				m_SplitterBrush = new SolidBrush(ColorTable.SplitterColor);

				m_GraphicObjectsCreated = true;
			}
		}

		protected override void CreateCaptionBrushes(bool recreate)
		{
			if (Layout.CaptionBounds.Width != 0 || Layout.CaptionBounds.Height != 0)
			{
				if (m_prevCaptionRect.Location != Layout.CaptionBounds.Location
					|| m_prevCaptionRect.Height != Layout.CaptionBounds.Height || recreate)
				{
					m_CaptionUpperBrush = new LinearGradientBrush(Layout.CaptionUpperBounds,
						ColorTable.CaptionUpperTopColor, ColorTable.CaptionUpperBottomColor,
						LinearGradientMode.Vertical);
					(m_CaptionUpperBrush as LinearGradientBrush).WrapMode = WrapMode.TileFlipXY;

					m_CaptionLowerBrush = new LinearGradientBrush(Layout.CaptionLowerBounds,
						ColorTable.CaptionLowerTopColor, ColorTable.CaptionLowerBottomColor,
						LinearGradientMode.Vertical);
					(m_CaptionLowerBrush as LinearGradientBrush).WrapMode = WrapMode.TileFlipXY;

					m_ActiveCaptionUpperBrush = new LinearGradientBrush(Layout.CaptionUpperBounds,
						ColorTable.ActiveCaptionUpperTopColor, ColorTable.ActiveCaptionUpperBottomColor,
						LinearGradientMode.Vertical);
					(m_ActiveCaptionUpperBrush as LinearGradientBrush).WrapMode = WrapMode.TileFlipXY;

					m_ActiveCaptionLowerBrush = new LinearGradientBrush(Layout.CaptionLowerBounds,
						ColorTable.ActiveCaptionLowerTopColor, ColorTable.ActiveCaptionLowerBottomColor,
						LinearGradientMode.Vertical);
					(m_ActiveCaptionLowerBrush as LinearGradientBrush).WrapMode = WrapMode.TileFlipXY;

					m_prevCaptionRect = Layout.CaptionBounds;
				}
			}
		}

		protected override void DisposeBrushes()
		{
			if (m_ActiveButtonBrush != null)
				m_ActiveButtonBrush.Dispose();
			if (m_ButtonBrush != null)
				m_ButtonBrush.Dispose();
			if (m_OuterBorderBrush != null)
				m_OuterBorderBrush.Dispose();
			if (m_PushedButtonBrush != null)
				m_PushedButtonBrush.Dispose();
			if (m_SplitterBrush != null)
				m_SplitterBrush.Dispose();
			if (m_TextBrush != null)
				m_TextBrush.Dispose();
			if (m_ButtonImagePen != null)
				m_ButtonImagePen.Dispose();
			if (m_ActiveButtonImagePen != null)
				m_ActiveButtonImagePen.Dispose();
			if (m_SplitterBrush != null)
				m_SplitterBrush.Dispose();

            m_ActiveButtonBrush = null;
            m_ButtonBrush = null;
            m_OuterBorderBrush = null;
            m_PushedButtonBrush = null;
            m_SplitterBrush = null;
            m_TextBrush = null;
            m_ButtonImagePen = null;
            m_ActiveButtonImagePen = null;
            m_SplitterBrush = null;

			DisposeCaptionBrushes();
			m_GraphicObjectsCreated = false;
		}

		protected override void DisposeCaptionBrushes()
		{
		}

		protected override Brush GetTextBrush(CaptionState cpState)
		{
			return m_TextBrush;
		}

		protected override Brush GetBorderBrush(bool innerBorder)
		{
			if (innerBorder)
			{
				return m_InnerBorderBrush;
			}
			else
			{
				return m_OuterBorderBrush;
			}
		}

		protected override Pen GetButtonImagePen(CaptionState cpState, CaptionButtonState btnState)
		{
			if (btnState == CaptionButtonState.Active)
			{
				return m_ActiveButtonImagePen;
			}
			else
			{
				return m_ButtonImagePen;
			}
		}

		protected override void PaintBorders(Graphics g, PaintDockControlArgs args)
		{
			base.PaintBorders(g, args);

			Brush borderBrush;
			if (args.ProvideGraphicsItemsArgs != null &&
				args.ProvideGraphicsItemsArgs.CaptionForeground != Color.Empty)
			{
				borderBrush = new SolidBrush(args.ProvideGraphicsItemsArgs.CaptionForeground);
			}
			else
			{
				borderBrush = GetBorderBrush(false);
			}
			g.FillRectangle(borderBrush, Layout.ControlBounds.Left, Layout.ControlBounds.Top, 3, 3);
			g.FillRectangle(borderBrush, Layout.ControlBounds.Right - 3, Layout.ControlBounds.Top, 3, 3);
			
			Brush outerBrush = null;
			if (args.ProvideGraphicsItemsArgs != null &&
			   args.ProvideGraphicsItemsArgs.CaptionForeground != Color.Empty)
			{
				outerBrush = new SolidBrush(args.ProvideGraphicsItemsArgs.CaptionForeground);
			}
			else
			{
				outerBrush = GetBorderBrush(false);
			}
			
			Rectangle rRectangle = this.Layout.ControlBounds;
			rRectangle.Inflate( -this.Layout.OuterBorderWidth, -this.Layout.OuterBorderWidth );
			
			RendererPrimitives.PaintBorders( g,rRectangle, outerBrush, 1 );
		}

		protected override Brush GetSplitterBrush()
		{
			return m_SplitterBrush;
		}
		protected override Brush GetButtonBrush(CaptionState cpState, CaptionButtonState btnState)
		{
			if (cpState == CaptionState.Active)
			{
				if (btnState == CaptionButtonState.Active)
				{
					return m_ActiveButtonBrush;
				}
				else
				{
					return m_PushedButtonBrush;
				}
			}
			else
			{
				return m_ButtonBrush;
			}
		}

		#endregion

		#region Private/Protected members

		private Office2007Theme m_prevTheme = Office2007Theme.Blue;		

		protected Brush m_CaptionUpperBrush;
		protected Brush m_CaptionLowerBrush;
		protected Brush m_ActiveCaptionUpperBrush;
		protected Brush m_ActiveCaptionLowerBrush;
		protected Brush m_OuterBorderBrush;
		protected Brush m_InnerBorderBrush;
		protected Pen m_ButtonImagePen;
		protected Pen m_ActiveButtonImagePen;
		protected Brush m_ButtonBrush;
		protected Brush m_ActiveButtonBrush;
		protected Brush m_PushedButtonBrush;
		protected Brush m_TextBrush;
		protected Brush m_SplitterBrush;

		protected bool m_GraphicObjectsCreated;

		#endregion
	}

    internal abstract class RendererOffice2010Base : CustomRenderer
    {
        #region Properties

        protected new virtual LayoutOffice2010Base Layout
        {
            get
            {
                return base.Layout as LayoutOffice2010Base;
            }
            set
            {
                base.Layout = value;
            }
        }
        protected new virtual ColorTableOffice2010Base ColorTable
        {
            get
            {
                return base.ColorTable as ColorTableOffice2010Base;
            }
            set
            {
                base.ColorTable = value;
            }
        }

        #endregion

        #region Methods

        protected override void PaintCaption(Graphics g, PaintDockControlArgs args)
        {
            // Painting captions in Office2007-like renderers differs from others
            // so calling method from base class is undesired.
            if (CaptionHeight != Layout.CaptionWidth || PrevFloating != Layout.Floating)
            {
                PrevFloating = Layout.Floating;
                CaptionHeight = Layout.CaptionWidth;
                RefreshCaptionBrushes();
            }

            Brush captionUpperBrush = null;
            Brush captionLowerBrush = null;

            try
            {
                if (args.ProvideGraphicsItemsArgs != null &&
                    (args.ProvideGraphicsItemsArgs.CaptionBackground is SolidBrush))
                {
                    Brush brush = new LinearGradientBrush(Layout.CaptionUpperBounds,
                        ((args.ProvideGraphicsItemsArgs.CaptionBackground as System.Drawing.Brush) as System.Drawing.SolidBrush).Color,
                        ((args.ProvideGraphicsItemsArgs.CaptionBackground as System.Drawing.Brush) as System.Drawing.SolidBrush).Color,
                        LinearGradientMode.Vertical);
                    (brush as LinearGradientBrush).WrapMode = WrapMode.TileFlipXY;

                    captionUpperBrush = brush;
                    captionLowerBrush = brush;
                }
                else
                {
                    if (args.ProvideGraphicsItemsArgs != null &&
                        args.ProvideGraphicsItemsArgs.CaptionBackground != null)
                    {
                        captionUpperBrush = args.ProvideGraphicsItemsArgs.CaptionBackground;
                        captionLowerBrush = args.ProvideGraphicsItemsArgs.CaptionBackground;
                    }
                    else
                    {
                        captionUpperBrush = GetCaptionUpperBrush(args.Caption.CaptionState);
                        captionLowerBrush = GetCaptionLowerBrush(args.Caption.CaptionState);
                    }
                }

                RendererPrimitives.PaintBackground(g, Layout.CaptionUpperBounds, captionUpperBrush);
                RendererPrimitives.PaintBackground(g, Layout.CaptionLowerBounds, captionLowerBrush);
            }
            finally
            {
                if (captionLowerBrush != null)
                    captionLowerBrush.Dispose();
                if (captionUpperBrush != null)
                    captionUpperBrush.Dispose();
            }

            PaintButtons(g, args);
            DrawText(g, args);
            if (Layout.ImageEnabled)
            {
                g.DrawImage(GetImageFromList(args.CaptionImageIndex, args.ImageList), Layout.ImageBounds);
            }
        }

        public override void RefreshOffice2010Theme(Office2010Theme newTheme)
        {
            base.RefreshOffice2010Theme(newTheme);

            if (newTheme != m_prevTheme || newTheme == Office2010Theme.Managed)
            {
                // To ensure creation of new caption brushes, enable caption at that moment.
                bool prevCaptionEnabled = Layout.CaptionEnabled;
                Layout.CaptionEnabled = true;
                ColorTable.Office2010Theme = newTheme;
                RefreshBrushes(true);
                Layout.CaptionEnabled = prevCaptionEnabled;
                m_prevTheme = newTheme;
            }
        }

        protected override Brush GetCaptionBrush(CaptionState cpState)
        {
            return GetCaptionUpperBrush(cpState);
        }

        protected virtual Brush GetCaptionUpperBrush(CaptionState cpState)
        {
            if (cpState == CaptionState.Normal)
            {
                Brush brush = new LinearGradientBrush(Layout.CaptionUpperBounds,
                    ColorTable.CaptionUpperTopColor, ColorTable.CaptionUpperBottomColor,
                    LinearGradientMode.Vertical);
                (brush as LinearGradientBrush).WrapMode = WrapMode.TileFlipXY;

                return brush;
            }
            else
            {
                Brush brush = new LinearGradientBrush(Layout.CaptionUpperBounds,
                    ColorTable.ActiveCaptionUpperTopColor, ColorTable.ActiveCaptionUpperBottomColor,
                    LinearGradientMode.Vertical);
                (brush as LinearGradientBrush).WrapMode = WrapMode.TileFlipXY;

                return brush;
            }
        }

        protected virtual Brush GetCaptionLowerBrush(CaptionState cpState)
        {
            if (cpState == CaptionState.Normal)
            {
                Brush brush = new LinearGradientBrush(Layout.CaptionLowerBounds,
                    ColorTable.CaptionLowerTopColor, ColorTable.CaptionLowerBottomColor,
                    LinearGradientMode.Vertical);
                (brush as LinearGradientBrush).WrapMode = WrapMode.TileFlipXY;

                return brush;
            }
            else
            {
                Brush brush = new LinearGradientBrush(Layout.CaptionLowerBounds,
                    ColorTable.ActiveCaptionLowerTopColor, ColorTable.ActiveCaptionLowerBottomColor,
                    LinearGradientMode.Vertical);
                (brush as LinearGradientBrush).WrapMode = WrapMode.TileFlipXY;

                return brush;
            }
        }

        protected override void CreateGraphicObjects(bool recreate)
        {
            m_GraphicObjectsCreated = !m_GraphicObjectsCreated || recreate;
            if (m_GraphicObjectsCreated)
            {
                m_OuterBorderBrush = new SolidBrush(ColorTable.OuterBorderColor);
                m_InnerBorderBrush = new SolidBrush(ColorTable.InnerBorderColor);

                m_ButtonImagePen = new Pen(ColorTable.ButtonImageColor);
                m_ActiveButtonImagePen = new Pen(ColorTable.ActiveButtonImageColor);

                Rectangle buttonBrushRect = (Layout.Table.Buttons.Count > 0) ?
                    Layout.GetButtonBounds(0) :
                    Rectangle.Empty;

                if (buttonBrushRect != Rectangle.Empty)
                {
                    m_ButtonBrush = new SolidBrush(ColorTable.ButtonColor);
                    m_ActiveButtonBrush = new SolidBrush(ColorTable.ActiveButtonColor);
                    m_PushedButtonBrush = new SolidBrush(ColorTable.PushedButtonColor);
                }

                m_TextBrush = new SolidBrush(ColorTable.TextColor);

                m_SplitterBrush = new SolidBrush(ColorTable.SplitterColor);

                m_GraphicObjectsCreated = true;
            }
        }

        protected override void CreateCaptionBrushes(bool recreate)
        {
            if (Layout.CaptionBounds.Width != 0 || Layout.CaptionBounds.Height != 0)
            {
                if (m_prevCaptionRect.Location != Layout.CaptionBounds.Location
                    || m_prevCaptionRect.Height != Layout.CaptionBounds.Height || recreate)
                {
                    m_CaptionUpperBrush = new LinearGradientBrush(Layout.CaptionUpperBounds,
                        ColorTable.CaptionUpperTopColor, ColorTable.CaptionUpperBottomColor,
                        LinearGradientMode.Vertical);
                    (m_CaptionUpperBrush as LinearGradientBrush).WrapMode = WrapMode.TileFlipXY;

                    m_CaptionLowerBrush = new LinearGradientBrush(Layout.CaptionLowerBounds,
                        ColorTable.CaptionLowerTopColor, ColorTable.CaptionLowerBottomColor,
                        LinearGradientMode.Vertical);
                    (m_CaptionLowerBrush as LinearGradientBrush).WrapMode = WrapMode.TileFlipXY;

                    m_ActiveCaptionUpperBrush = new LinearGradientBrush(Layout.CaptionUpperBounds,
                        ColorTable.ActiveCaptionUpperTopColor, ColorTable.ActiveCaptionUpperBottomColor,
                        LinearGradientMode.Vertical);
                    (m_ActiveCaptionUpperBrush as LinearGradientBrush).WrapMode = WrapMode.TileFlipXY;

                    m_ActiveCaptionLowerBrush = new LinearGradientBrush(Layout.CaptionLowerBounds,
                        ColorTable.ActiveCaptionLowerTopColor, ColorTable.ActiveCaptionLowerBottomColor,
                        LinearGradientMode.Vertical);
                    (m_ActiveCaptionLowerBrush as LinearGradientBrush).WrapMode = WrapMode.TileFlipXY;

                    m_prevCaptionRect = Layout.CaptionBounds;
                }
            }
        }

        protected override void DisposeBrushes()
        {
            if (m_ActiveButtonBrush != null)
                m_ActiveButtonBrush.Dispose();
            if (m_ButtonBrush != null)
                m_ButtonBrush.Dispose();
            if (m_OuterBorderBrush != null)
                m_OuterBorderBrush.Dispose();
            if (m_PushedButtonBrush != null)
                m_PushedButtonBrush.Dispose();
            if (m_SplitterBrush != null)
                m_SplitterBrush.Dispose();
            if (m_TextBrush != null)
                m_TextBrush.Dispose();
            if (m_ButtonImagePen != null)
                m_ButtonImagePen.Dispose();
            if (m_ActiveButtonImagePen != null)
                m_ActiveButtonImagePen.Dispose();
            if (m_SplitterBrush != null)
                m_SplitterBrush.Dispose();

            m_ActiveButtonBrush = null;
            m_ButtonBrush = null;
            m_OuterBorderBrush = null;
            m_PushedButtonBrush = null;
            m_SplitterBrush = null;
            m_TextBrush = null;
            m_ButtonImagePen = null;
            m_ActiveButtonImagePen = null;
            m_SplitterBrush = null;

            DisposeCaptionBrushes();
            m_GraphicObjectsCreated = false;
        }

        protected override Brush GetTextBrush(CaptionState cpState)
        {
            return m_TextBrush;
        }

        protected override Brush GetBorderBrush(bool innerBorder)
        {
            if (innerBorder)
            {
                return m_InnerBorderBrush;
            }
            else
            {
                return m_OuterBorderBrush;
            }
        }

        protected override Pen GetButtonImagePen(CaptionState cpState, CaptionButtonState btnState)
        {
            if (btnState == CaptionButtonState.Active)
            {
                return m_ActiveButtonImagePen;
            }
            else
            {
                return m_ButtonImagePen;
            }
        }

        protected override void PaintBorders(Graphics g, PaintDockControlArgs args)
        {
            base.PaintBorders(g, args);

            Brush borderBrush;
            if (args.ProvideGraphicsItemsArgs != null &&
                args.ProvideGraphicsItemsArgs.CaptionForeground != Color.Empty)
            {
                borderBrush = new SolidBrush(args.ProvideGraphicsItemsArgs.CaptionForeground);
            }
            else
            {
                borderBrush = GetBorderBrush(false);
            }
            g.FillRectangle(borderBrush, Layout.ControlBounds.Left, Layout.ControlBounds.Top, 3, 3);
            g.FillRectangle(borderBrush, Layout.ControlBounds.Right - 3, Layout.ControlBounds.Top, 3, 3);

            Brush outerBrush = null;
            if (args.ProvideGraphicsItemsArgs != null &&
               args.ProvideGraphicsItemsArgs.CaptionForeground != Color.Empty)
            {
                outerBrush = new SolidBrush(args.ProvideGraphicsItemsArgs.CaptionForeground);
            }
            else
            {
                outerBrush = GetBorderBrush(false);
            }

            Rectangle rRectangle = this.Layout.ControlBounds;
            rRectangle.Inflate(-this.Layout.OuterBorderWidth, -this.Layout.OuterBorderWidth);

            RendererPrimitives.PaintBorders(g, rRectangle, outerBrush, 1);
        }

        protected override Brush GetSplitterBrush()
        {
            return m_SplitterBrush;
        }
        protected override Brush GetButtonBrush(CaptionState cpState, CaptionButtonState btnState)
        {
            if (cpState == CaptionState.Active)
            {
                if (btnState == CaptionButtonState.Active)
                {
                    return m_ActiveButtonBrush;
                }
                else
                {
                    return m_PushedButtonBrush;
                }
            }
            else
            {
                return m_ButtonBrush;
            }
        }

        #endregion

        #region Private/Protected members

        private Office2010Theme m_prevTheme = Office2010Theme.Blue;

        protected Brush m_CaptionUpperBrush;
        protected Brush m_CaptionLowerBrush;
        protected Brush m_ActiveCaptionUpperBrush;
        protected Brush m_ActiveCaptionLowerBrush;
        protected Brush m_OuterBorderBrush;
        protected Brush m_InnerBorderBrush;
        protected Pen m_ButtonImagePen;
        protected Pen m_ActiveButtonImagePen;
        protected Brush m_ButtonBrush;
        protected Brush m_ActiveButtonBrush;
        protected Brush m_PushedButtonBrush;
        protected Brush m_TextBrush;
        protected Brush m_SplitterBrush;

        protected bool m_GraphicObjectsCreated;

        #endregion
    }

    internal class RendererOffice2010 : RendererOffice2010Base
    {
        #region Constructors

        public RendererOffice2010()
        {
            ColorTable = new ColorTableOffice2010();
            Layout = new LayoutOffice2010();
            CreateGraphicObjects(false);
        }

        #endregion

        #region Properties

        protected new LayoutOffice2010 Layout
        {
            get { return base.Layout as LayoutOffice2010; }
            set { base.Layout = value; }
        }

        protected new ColorTableOffice2010 ColorTable
        {
            get
            {
                return base.ColorTable as ColorTableOffice2010;
            }
            set
            {
                base.ColorTable = value;
            }
        }

        #endregion

        #region Methods

        protected override void CreateGraphicObjects(bool recreate)
        {
            base.CreateGraphicObjects(recreate);

            m_GraphicObjectsCreated = !m_GraphicObjectsCreated || recreate;
            if (m_GraphicObjectsCreated)
            {
                m_ButtonBorderPen = new Pen(ColorTable.ButtonBorderColor);
                m_ActiveButtonBorderPen = new Pen(ColorTable.ActiveButtonBorderColor);
                m_PushedButtonBorderPen = new Pen(ColorTable.PushedButtonBorderColor);
            }
        }

        protected override void DisposeBrushes()
        {
            base.DisposeBrushes();

            if (m_ButtonBorderPen != null)
                m_ButtonBorderPen.Dispose();
            if (m_ActiveButtonBorderPen != null)
                m_ActiveButtonBorderPen.Dispose();
            if (m_PushedButtonBorderPen != null)
                m_PushedButtonBorderPen.Dispose();
        }

        protected override Pen GetButtonPen(CaptionState cpState, CaptionButtonState btnState)
        {
            if (btnState == CaptionButtonState.Pushed)
            {
                return m_PushedButtonBorderPen;
            }
            else
            {
                if (cpState == CaptionState.Active)
                {
                    return m_ActiveButtonBorderPen;
                }
                else
                {
                    return m_ButtonBorderPen;
                }
            }
        }

        #endregion

        #region Private/Protected members

        private Pen m_ButtonBorderPen;
        private Pen m_ActiveButtonBorderPen;
        private Pen m_PushedButtonBorderPen;

        #endregion

        protected override void DisposeCaptionBrushes()
        {

        }
    }

	internal class RendererOffice2007 : RendererOffice2007Base
	{
		#region Constructors

		public RendererOffice2007 ()
		{
			ColorTable = new ColorTableOffice2007();
			Layout = new LayoutOffice2007();
			CreateGraphicObjects( false );
		}

		#endregion

		#region Properties

		protected new LayoutOffice2007 Layout
		{
			get { return base.Layout as LayoutOffice2007; }
			set { base.Layout = value; }
		}

		protected new ColorTableOffice2007 ColorTable
		{
			get
			{
				return base.ColorTable as ColorTableOffice2007;
			}
			set
			{
				base.ColorTable = value;
			}
		}

		#endregion

		#region Methods

		protected override void CreateGraphicObjects(bool recreate)
		{
			base.CreateGraphicObjects(recreate);

			m_GraphicObjectsCreated = !m_GraphicObjectsCreated || recreate;
			if( m_GraphicObjectsCreated )
			{
				m_ButtonBorderPen = new Pen( ColorTable.ButtonBorderColor );
				m_ActiveButtonBorderPen = new Pen( ColorTable.ActiveButtonBorderColor );
				m_PushedButtonBorderPen = new Pen( ColorTable.PushedButtonBorderColor );				
			}
		}
				
		protected override void DisposeBrushes()
		{
			base.DisposeBrushes();

			if( m_ButtonBorderPen != null )
				m_ButtonBorderPen.Dispose();
			if( m_ActiveButtonBorderPen != null )
				m_ActiveButtonBorderPen.Dispose();
			if( m_PushedButtonBorderPen != null )
				m_PushedButtonBorderPen.Dispose();
		}

		protected override Pen GetButtonPen(CaptionState cpState, CaptionButtonState btnState)
		{
			if( btnState == CaptionButtonState.Pushed )
			{
				return m_PushedButtonBorderPen;
			}
			else
			{
				if( cpState == CaptionState.Active )
				{
					return m_ActiveButtonBorderPen;
				}
				else
				{
					return m_ButtonBorderPen;
				}
			}
		}

		#endregion

		#region Private/Protected members

		private Pen m_ButtonBorderPen;
		private Pen m_ActiveButtonBorderPen;
		private Pen m_PushedButtonBorderPen;

		#endregion
	}

	internal class RendererOffice2007Outlook : RendererOffice2007Base
	{
		#region Constructors

		public RendererOffice2007Outlook()
		{
			ColorTable = new ColorTableOffice2007Outlook();
			Layout = new LayoutOffice2007Outlook();
			CreateGraphicObjects(false);
		}

		#endregion

		#region Properties

		protected new LayoutOffice2007Outlook Layout
		{
			get { return base.Layout as LayoutOffice2007Outlook; }
			set { base.Layout = value; }
		}

		protected new ColorTableOffice2007Outlook ColorTable
		{
			get
			{
				return base.ColorTable as ColorTableOffice2007Outlook;
			}
			set
			{
				base.ColorTable = value;
			}
		}

		#endregion

		#region Methods

		protected override Pen GetButtonPen(CaptionState cpState, CaptionButtonState btnState)
		{
			// No borders around buttons are painted.
			return null;
		}

		#endregion

	}
}
