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

namespace Syncfusion.Windows.Forms.Tools.Renderers
{
	/// <summary>
	/// Provides the interface for <see cref="Syncfusion.Windows.Forms.Tools.Renderers.DockingManagerRenderer"/> for rendering 
	/// the docking manager with the different themed color.
	/// </summary>
	public interface IDockingManagerRenderer
	{
		/// <summary>
		/// Gets/Sets the VisualStyle for docking windows.
		/// </summary>
		VisualStyle VisualStyle { get; set; }

		/// <summary>
		/// Draws the docking windows.
		/// </summary>
		void PaintDockedControl( Graphics g, Rectangle rectangle, PaintDockControlArgs args );
		/// <summary>
		/// Draws the splitter control.
		/// </summary>
		void PaintSplitter( Graphics g, Rectangle rectangle, Orientation orientation );

		/// <summary>
		/// Draws the AutoHide panels.
		/// </summary>
		void PaintAutoHidePanels( Graphics g, Rectangle rectangle, AutoHideSide side );

		/// <summary>
		/// Initializes color scheme in accordance with current theme.
		/// </summary>
		void RefreshColors();

		/// <summary>
		/// Initializes Office2007 colors according to given theme. 
		/// </summary>
		void RefreshOffice2007Theme(Office2007Theme newTheme);

        /// <summary>
        /// Initializes Office2010 colors according to given theme. 
        /// </summary>
        void RefreshOffice2010Theme(Office2010Theme newTheme);

		/// <summary>
		/// Refreshes renderer's info about painted docked control.
		/// </summary>
		void RefreshPaintInfo( Rectangle rectangle, PaintDockControlArgs args );

		/// <summary>
		/// Returns the HitTest area on the docking caption.
		/// </summary>
		HitTestArea HitTest( MouseButtons button, Point point );

		/// <summary>
		/// Returns index of button for HitTestArea.Button hit test result.
		/// </summary>
		int GetHitButtonIndex();

        /// <summary>
        /// Returns index of button that is currently highlighted.
        /// </summary>
        /// <returns></returns>
        int GetHighlightedButtonIndex();

		/// <summary>
		/// /// Returns CaptionButton for HitTestArea.Button hit test result.
		/// </summary>
		CaptionButton GetHitButton();

		/// <summary>
		/// Resets all data related to last performed hit test.
		/// </summary>
		void ResetButtonsHitTest();

		/// <summary>
		/// Get/sets the bounds of the control.
		/// </summary>
		Rectangle ControlBounds { get; set; }

        /// <summary>
        /// Gets the bounds of caption.
        /// </summary>
        Rectangle CaptionBounds { get; }

		/// <summary>
		/// Returns the width of the caption.
		/// </summary>
		int CaptionWidth { get; }
		/// <summary>
		/// Returns the width of the Border.
		/// </summary>
		int BorderWidth { get; }

		/// <summary>
		/// Indicates the whether the caption to be painted from RTL or not.
		/// </summary>
		bool IsMirrored { get; set;  }
	}

	/// <summary>
	/// Caption bar of the docking windows.
	/// </summary>
	public class Caption
	{
		/// <summary>
		/// Creates the instance of the Caption bar.
		/// </summary>
		public Caption()
		{}

		/// <summary>
		/// Overloaded Constructor.Creates with the CaptionState.
		/// </summary>
		public Caption( CaptionState state )
		{
			m_CaptionState = state;
		}

		/// <summary>
		/// Overloaded Constructor. Creates with Enabled state.
		/// </summary>
		public Caption( bool enabled )
		{
			m_Enabled = enabled;  
		}

		/// <summary>
		/// Overloaded Constructor. Creates with the Label text.
		/// </summary>
		public Caption( string text )
		{
			m_Text = text;
		}

		/// <summary>
		/// Overloaded Constructor. Creates with Caption state, docking label, 
		/// Docking label alignment and with the customized font.
		/// </summary>
		public Caption( CaptionState state, string text, 
			DockLabelAlignmentStyle alignment, Font font )
		{
			m_CaptionState = state;
			m_Text = text;
			m_Alignment = alignment;
			m_Font = font;
		}

		/// <summary>
		/// Gets/sets the Caption state of the docking windows.
		/// </summary>
		public CaptionState CaptionState
		{
			get { return m_CaptionState; }
			set { m_CaptionState = value; }
		}

		/// <summary>
		/// Returns the enabled state of the docking windows.
		/// </summary>
		public bool Enabled
		{
			get { return m_Enabled; }
		}

		/// <summary>
		/// Gets/sets the Docking Label of the docking windows.
		/// </summary>
		public string Text
		{
			get { return m_Text; }
			set { m_Text = value; }
		}

		/// <summary>
		/// Gets/sets the Dock label alignment style of the docking windows.
		/// </summary>
		public DockLabelAlignmentStyle TextAlignment
		{
			get { return m_Alignment; }
			set { m_Alignment = value; }
		}

		/// <summary>
		/// Gets/sets the font for the docking windows caption.
		/// </summary>
		public Font Font
		{
			get{ return m_Font; }
			set { m_Font = value; }
		}
		
		private CaptionState m_CaptionState = CaptionState.Normal;
		private bool m_Enabled = true;
		private string m_Text;
		private DockLabelAlignmentStyle m_Alignment;
		private Font m_Font;
	}


	/// <summary>
	/// Rendering the docking manager with specified visual style.
	/// </summary>
	public class DockingManagerRenderer : IDockingManagerRenderer
	{
		/// <summary>
		/// Creates the instances of the DockingManagerRenderer.
		/// </summary>
		public DockingManagerRenderer()
		{
			//if( VisualStyle != Renderers.VisualStyle.Default )
            StyleRenderer = new RendererVS2005();
		}
        internal Color m_MetroColor = ColorTranslator.FromHtml("#119EDA");
        private Color metroCaption = Color.White;
        private Color metroButton = Color.FromArgb(255, 255, 255);
        private Color metroSplitter = Color.FromArgb(255, 255, 255);
        private Color metrobordercolor = Color.FromArgb(255, 255, 255);

        internal Color MetroCaptionColor
        {
            get
            {
                return metroCaption;
            }
            set
            {
                if (metroCaption != value)
                {
                    metroCaption = value;
                    if ((StyleRenderer is RendererMetro))
                        (StyleRenderer as RendererMetro).ActiveCaption = this.MetroCaptionColor;
                }
            }
        }      

        internal Color MetroColor
        {
            get
            {
                return m_MetroColor;
            }
            set
            {
                if (m_MetroColor != value)
                {
                    m_MetroColor = value;
                    if ((StyleRenderer is RendererMetro))
                        (StyleRenderer as RendererMetro).MetroColor = this.m_MetroColor;
                }
            }
        }     

        internal Color MetroSplitterColor
        {
            get
            {
                return metroSplitter;
            }
            set
            {
                if (metroSplitter != value)
                {
                    metroSplitter = value;
                    if ((StyleRenderer is RendererMetro))
                        (StyleRenderer as RendererMetro).SplitterColor = this.metroSplitter;
                }
            }
        }

        internal Color MetroBorderColor
        {
            get
            {
                return metrobordercolor;
            }
            set
            {
                if (metrobordercolor != value)
                {
                    metrobordercolor = value;
                    if ((StyleRenderer is RendererMetro))
                        (StyleRenderer as RendererMetro).BorderColor = this.metrobordercolor;
                }
            }
        }

        internal Color MetroButtonColor
        {
            get
            {
                return metroButton;
            }
            set
            {
                if (metroButton != value)
                {
                    metroButton = value;
                    if ((StyleRenderer is RendererMetro))
                        (StyleRenderer as RendererMetro).ButtonColor = this.MetroButtonColor;
                }
            }
        }
		/// <summary>
		/// Gets/sets the Visual Style of the docking windows.
		/// </summary>
		public VisualStyle VisualStyle
		{
			get { return m_VisualStyle; }
			set
			{
				if( m_VisualStyle != value )
				{
					m_VisualStyle = value;
					StyleRenderer = RendererFactory.GetRenderer( m_VisualStyle );
					if (VisualStyle == VisualStyle.Metro)
					{
						(StyleRenderer as RendererMetro).MetroColor = MetroColor;
                        (StyleRenderer as RendererMetro).ButtonColor = MetroButtonColor;
                        (StyleRenderer as RendererMetro).ActiveCaption = MetroCaptionColor;
					}
				}
			}
		}

		/// <summary>
		/// Draws the docking windows.
		/// </summary>
		public virtual void PaintDockedControl(  Graphics g, Rectangle rectangle, 
			PaintDockControlArgs args )
		{
            //Check added for Defect #3058
            if (StyleRenderer != null) 
			    StyleRenderer.PaintDockedControl( g, rectangle, args );
		}

		/// <summary>
		/// Refreshes renderer's info about painted docked control.
		/// </summary>
		public virtual void RefreshPaintInfo( Rectangle rectangle, PaintDockControlArgs args )
		{
			if( StyleRenderer != null )
				StyleRenderer.RefreshPaintInfo( rectangle, args );
		}

		/// <summary>
		/// Draws the splitter control.
		/// </summary>
		public virtual void PaintSplitter( Graphics g, Rectangle rectangle, 
			Orientation orientation )
		{
			StyleRenderer.PaintSplitter( g, rectangle, orientation );
		}

		/// <summary>
		/// Draws the AutoHide panels.
		/// </summary>
		public virtual void PaintAutoHidePanels(  Graphics g, Rectangle rectangle, 
			AutoHideSide side )
		{
			StyleRenderer.PaintAutoHidePanels( g, rectangle, side );
		}

		/// <summary>
		/// Returns the HitTest area on the docking caption.
		/// </summary>
		public virtual HitTestArea HitTest( MouseButtons button, Point point )
		{
			return StyleRenderer.HitTest( button, point );
		}

		/// <summary>
		/// Returns index of caption button at the specified point.
		/// </summary>
		public virtual int GetHitButtonIndex()
		{
			return StyleRenderer.GetHitButtonIndex();
		}

        /// <summary>
        /// Returns index of button that is currently highlighted.
        /// </summary>
        public virtual int GetHighlightedButtonIndex()
        {
            return StyleRenderer.GetHighlightedButtonIndex();
        }

		/// <summary>
		/// /// Returns CaptionButton for HitTestArea.Button hit test result.
		/// </summary>
		public virtual CaptionButton GetHitButton()
		{
			return StyleRenderer.GetHitButton();
		}

		/// <summary>
		/// Initializes color scheme in accordance with current theme.
		/// </summary>
		public virtual void RefreshColors()
		{
			StyleRenderer.RefreshColors();
		}

		/// <summary>
		/// Initializes Office2007 colors according to given theme. 
		/// </summary>
		public virtual void RefreshOffice2007Theme(Office2007Theme newTheme)
		{
			StyleRenderer.RefreshOffice2007Theme(newTheme);
		}

        /// <summary>
        /// Initializes Office2010 colors according to given theme. 
        /// </summary>
        public virtual void RefreshOffice2010Theme(Office2010Theme newTheme)
        {
            StyleRenderer.RefreshOffice2010Theme(newTheme);
        }

		/// <summary>
		/// Resets all data related to last performed hit test.
		/// </summary>
		public void ResetButtonsHitTest()
		{
			StyleRenderer.ResetCaptionButtonsIndices();
		}

		/// <summary>
		/// Returns the width of the caption.
		/// </summary>
		public virtual int CaptionWidth
		{
			get
			{
				if( StyleRenderer != null )
				{
					return StyleRenderer.CaptionWidth;
				}
				else
				{
					return SystemInformation.MenuFont.Height + 3;
				}
			}
		}

		/// <summary>
		/// Returns the width of the Border.
		/// </summary>
		public virtual int BorderWidth
		{
			get
			{
				if( StyleRenderer != null )
				{
					return StyleRenderer.BorderWidth;
				}
				else
				{
					return 1;
				}
			}
		}

		/// <summary>
		/// Returns the width of the thick border.
		/// </summary>
		public virtual int ThickBorderWidth
		{
			get { return StyleRenderer.ThickBorderWidth; }
		}

		/// <summary>
		/// Returns the width of the thin border.
		/// </summary>
		public virtual int ThinBorderWidth
		{
			get { return StyleRenderer.ThinBorderWidth; }
		}

		/// <summary>
		/// Get/sets the bounds of the control.
		/// </summary>
		public Rectangle ControlBounds
		{
			get { return StyleRenderer.ControlBounds; }
			set { StyleRenderer.ControlBounds = value; }
		}

        /// <summary>
        /// Gets the bounds of caption.
        /// </summary>
        public Rectangle CaptionBounds
        {
            get { return StyleRenderer.CaptionBounds; }
        }

		/// <summary>
		/// Indicates the whether the caption to be painted from RTL or not.
		/// </summary>
		public bool IsMirrored
		{
			get { return StyleRenderer.IsMirrored; }
			set { StyleRenderer.IsMirrored = value; }
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected IStyleRenderer StyleRenderer;

		private VisualStyle m_VisualStyle = VisualStyle.Default;
	}

	/// <summary>
	/// Collection of the docking windows different part 
	/// of the classes such as Caption, Close Button and Pin Button.
	/// </summary>
	public class PaintDockControlArgs
	{
		#region Constructors

		/// <summary>
		/// Creates the instance of the class.
		/// </summary>
		public PaintDockControlArgs()
		{
			m_Caption = new Caption();
			m_PaintBorders = true;
			m_Floating = false;
			m_CaptionImageIndex = -1;
			m_pgargs = null;
			m_Table = null;
			m_ImageList = null;
		}

		/// <summary>
		/// Overloaded constructor.
		/// </summary>
		
		public PaintDockControlArgs( Caption caption, CaptionButtonOptionsTable table, bool bPaintBorders, bool bFloating, int captionImageIndex, ImageList imageList )
		{
			m_Caption = caption;
			m_Table = table;
			m_PaintBorders = bPaintBorders;
			m_Floating = bFloating;
			m_CaptionImageIndex = captionImageIndex;
      m_pgargs = null;
			m_ImageList = imageList;
		}

        /// <summary>
        /// Overloaded constructor.
        /// </summary>
        public PaintDockControlArgs(Caption caption, CaptionButtonOptionsTable table, bool bPaintBorders, bool bFloating, int captionImageIndex, ProvideGraphicsItemsEventArgs pgargs, ImageList imageList)
        {
          m_Caption = caption;
          m_Table = table;
          m_PaintBorders = bPaintBorders;
          m_Floating = bFloating;
          m_CaptionImageIndex = captionImageIndex;
          m_pgargs = pgargs;
					m_ImageList = imageList;
        }
				
		#endregion

		#region Class properties

		/// <summary>
		/// Gets/sets the Caption of the docking windows.
		/// </summary>
		public Caption Caption
		{
			get { return m_Caption; }
			set { m_Caption = value; }
		}

		/// <summary>
		/// Gets/Sets the borders of the docking windows.
		/// </summary>
		public bool PaintBorders
		{
			get { return m_PaintBorders; }
			set { m_PaintBorders = value; }
		}

		/// <summary>
		/// Determines whether to paint FloatingForm or docked window: 
		/// TRUE - paint FloatingForm;
		/// FALSE - paint docked control.
		/// </summary>
		public bool Floating
		{
			get { return m_Floating; }
			set { m_Floating = value; }
		}

		/// <summary>
		/// Gets/Sets the image that should be painted in caption.
		/// </summary>
		public int CaptionImageIndex
		{
			get { return m_CaptionImageIndex; }
			set { m_CaptionImageIndex = value; }
		}

      /// <summary>
      /// Gets/Sets additional painting arguments.
      /// </summary>
      public ProvideGraphicsItemsEventArgs ProvideGraphicsItemsArgs
      {
          get { return m_pgargs; }
          set { m_pgargs = value; }
      }

		/// <summary>
		/// Gets caption buttons and options associated with them.
		/// </summary>
		public CaptionButtonOptionsTable Table
		{
			get { return m_Table; }
		}

		/// <summary>
		/// Gets ImageList that contains images used to paint CaptionButtons.
		/// </summary>
		public ImageList ImageList
		{
			get { return m_ImageList; }
		}

		/// <summary>
		/// Used to indicate whether painted control is in design mode.
		/// </summary>
		public bool DesignMode
		{
			get { return m_DesignMode; }
			set
			{
				if( m_DesignMode != value )
				{
					m_DesignMode = value;
				}
			}
		}

		#endregion

		#region Class members
		private Caption m_Caption;
		private CaptionButtonOptionsTable m_Table;
		private bool m_PaintBorders;
		private bool m_Floating;
		private int m_CaptionImageIndex;
		private ProvideGraphicsItemsEventArgs m_pgargs;
		private ImageList m_ImageList;
		private bool m_DesignMode = false;
		#endregion
	}

	internal class RendererFactory
	{ 
		public static IStyleRenderer GetRenderer( VisualStyle style )
		{ 
			switch( style )
			{
				case VisualStyle.Default:
				{
					return null;
				}
				case VisualStyle.VS2005:
				{
					return new RendererVS2005();
				}
				case VisualStyle.Office2007:
				{
					return new RendererOffice2007();
				}
				case VisualStyle.Office2007Outlook:
				{
					return new RendererOffice2007Outlook(); 
				}
                case VisualStyle.Metro:
                {
                    return new RendererMetro();
                }
                case VisualStyle.Office2010:
                {
                    return new RendererOffice2010();
                }
                case VisualStyle.VS2010:
                {
                    return new RendererVS2010();
                }
				case VisualStyle.Office2003:
				default:
				{
					return new RendererOffice2003();
				}
			}
		}
	}
	
	/// <summary>
	/// Specifies the edges of the host form to autohide.
	/// </summary>
	[ Flags ]
	public enum AutoHideSide { 
		/// <summary>
		/// For Internal use.
		/// </summary>
		None = 0, 
		/// <summary>
		/// Left edge of the host form.
		/// </summary>
		Left = 1, 
		/// <summary>
		/// Top edge of the host form.
		/// </summary>
		Top = 2, 
		/// <summary>
		/// Right edge of the host form.
		/// </summary>
		Right = 4, 
		/// <summary>
		/// Bottom edge of the host form.
		/// </summary>
		Bottom = 8 };

	/// <summary>
	/// Specifies the area which is hit by mouse pointer.
	/// </summary>
	public enum HitTestArea { 
		/// <summary>
		/// None of the area.
		/// </summary>
		None, 
		/// <summary>
		/// Border area of the docking window.
		/// </summary>
		Border, 
		/// <summary>
		/// Caption part of the docking window.
		/// </summary>
		Caption, 
		/// <summary>
		/// Caption button of docking window.
		/// </summary>
		Button
	};

	/// <summary>
	/// Specifies the Caption of the docking windows.
	/// </summary>
	public enum CaptionState { 
		/// <summary>
		/// Normal state of Caption.
		/// </summary>
		Normal, 
		/// <summary>
		/// Active state of Caption.
		/// </summary>
		Active };

	/// <summary>
	/// Specifies the different XP OS themes.
	/// </summary>
	public enum Theme { 
		/// <summary>
		/// Classic theme of XP OS.
		/// </summary>
		WindowsClassic, 
		/// <summary>
		/// XP Blue theme of XP OS.
		/// </summary>
		XPBlue, 
		/// <summary>
		/// XPOlive theme of XP OS.
		/// </summary>
		XPOlive, 
		/// <summary>
		/// XPSilver theme of XP OS.
		/// </summary>
		XPSilver,
        /// <summary>
        /// Zune theme of XP OS.
        /// </summary>
        XPZune }
}
