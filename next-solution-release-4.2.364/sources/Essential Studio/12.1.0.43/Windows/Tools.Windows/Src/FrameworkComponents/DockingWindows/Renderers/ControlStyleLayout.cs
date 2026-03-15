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
using System.Collections;

namespace Syncfusion.Windows.Forms.Tools.Renderers
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IControlStyleLayout
	{
		#region Properties

		bool IsMirrored { get; set; }
		Orientation CaptionOrientation { get; set; }
		bool CaptionEnabled { get; set; }
		bool Floating { get; set; }
		bool ImageEnabled { get; set; }
		CaptionButtonOptionsTable Table { get; set; }

		int BorderWidth { get; }
		int ThickBorderWidth { get; }
		int ThinBorderWidth { get; }
		int OuterBorderWidth { get; }
		int CaptionWidth { get; }

		int ActiveButtonIndex { get; }
		int PushedButtonIndex { get; }

		int TextRectHeight { get; set; }

		Rectangle ControlBounds { get; set; }
		Rectangle CaptionBounds { get; }
		Rectangle ThickBorderCaptionBounds { get; }
		Rectangle ThinBorderCaptionBounds { get; }
		Rectangle TextBounds { get; }
		Rectangle ImageBounds { get; }


		#endregion

		#region Methods

		HitTestArea HitTest( MouseButtons button, Point point );
		int GetHitButtonIndex();
		CaptionButton GetHitButton();
		Rectangle GetButtonBounds(int index);
		Rectangle GetButtonImageBounds();
		void ResetCaptionButtonIndices();

		#endregion
	}

	internal abstract class ControlStyleLayout : IControlStyleLayout
	{
		#region Properties
		public Rectangle ControlBounds
		{
			get { return m_ControlBounds; }
			set { m_ControlBounds = value; }
		}

		public bool IsMirrored
		{
			get { return m_IsMirrored; }
			set { m_IsMirrored = value; }
		}

		public Orientation CaptionOrientation
		{
			get { return m_Orientation; }
			set { m_Orientation = value; }
		}

		public bool CaptionEnabled
		{
			get { return m_CaptionEnabled; }
			set { m_CaptionEnabled = value; }
		}

		public bool Floating
		{
			get { return m_Floating; }
			set { m_Floating = value; }
		}

		public bool ImageEnabled
		{
			get { return m_ImageEnabled; }
			set { m_ImageEnabled = value; }
		}

		public CaptionButtonOptionsTable Table
		{
			get { return m_Table; }
			set 
			{ 
				if(m_Table != value)
					m_Table = value; 
			}
		}

		public int ActiveButtonIndex
		{
			get { return m_activeButtonIndex; }
		}

		public int PushedButtonIndex
		{
			get { return m_pushedButtonIndex; }
		}

		public abstract int BorderWidth { get; }
		public abstract int ThickBorderWidth { get; }
		public abstract int ThinBorderWidth { get; }
		public abstract int OuterBorderWidth { get; }
		public abstract int CaptionWidth { get; }

		public abstract Rectangle CaptionBounds { get; }
		public abstract Rectangle ThinBorderCaptionBounds { get; }
		public abstract Rectangle ThickBorderCaptionBounds { get; }
		public abstract Rectangle TextBounds { get;  }
		public abstract Rectangle ImageBounds { get; }

		public abstract int TextRectHeight { get; set; }
		protected abstract int ButtonSize { get; }
		protected abstract int ButtonMargin { get; }
		protected abstract int ButtonSeparator { get; }

		#endregion

		#region Methods

		public abstract HitTestArea HitTest( MouseButtons button, Point point );

		public virtual Rectangle GetButtonImageBounds()
		{
			return new Rectangle( 0, 0, ButtonSize, ButtonSize );
		}

		public void ResetCaptionButtonIndices()
		{
			this.m_captionButtonIndex = -1;
			this.m_activeButtonIndex = -1;
			this.m_pushedButtonIndex = -1;
		}

		protected virtual void SetCaptionButtonIndex( MouseButtons button, Point pt )
		{
			for( int i = 0; i < Table.Buttons.Count; i++ )
			{
				Rectangle rcButton = GetButtonBounds(i);
				if( rcButton != Rectangle.Empty && rcButton.Contains(pt) )
				{
					if( i >= 0 )
					{
						if( button == MouseButtons.Left )
						{
							m_pushedButtonIndex = i;
							m_activeButtonIndex = -1;
						}
						else
						{
							m_pushedButtonIndex = -1;
							m_activeButtonIndex = i;
						}
					}
					m_captionButtonIndex = i;
					return;
				}
			}
			m_captionButtonIndex = -1;
			m_activeButtonIndex = -1;
			m_pushedButtonIndex = -1;
		}

		public virtual int GetHitButtonIndex()
		{
			return m_captionButtonIndex;
		}

		public virtual CaptionButton GetHitButton()
		{
			if( m_captionButtonIndex >= 0 && m_captionButtonIndex < Table.Buttons.Count )
			{
				return Table.Buttons[m_captionButtonIndex];
			}
			else
			{
				return null;
			}
		}

		public virtual Rectangle GetButtonBounds( int index )
		{
			Rectangle rectangle = Rectangle.Empty;
			int topMargin = ( CaptionWidth - ButtonSize ) / 2;
			int margin = ( ButtonSize + ButtonSeparator ) * index;
			if( IsMirrored )
			{
				rectangle.X = ControlBounds.Left + BorderWidth 
					+ ButtonMargin + margin;
			}
			else
			{
				rectangle.X = ControlBounds.Left + ControlBounds.Width 
					- BorderWidth - ButtonMargin - margin - ButtonSize;
			}
			rectangle.Y = CaptionBounds.Y + topMargin;
			rectangle.Size = new Size ( ButtonSize, ButtonSize) ;

			if( CaptionBounds.Contains( rectangle ) )
				return rectangle;

			return Rectangle.Empty;
		}


		#endregion

		#region Private members

		private Rectangle m_ControlBounds;
		private bool m_IsMirrored;
		private Orientation m_Orientation;
		private bool m_CaptionEnabled = true;
		private bool m_Floating;
		private bool m_ImageEnabled;
		private CaptionButtonOptionsTable m_Table = new CaptionButtonOptionsTable();
		private int m_captionButtonIndex = -1;
		private int m_activeButtonIndex = -1;
		private int m_pushedButtonIndex = -1;

		#endregion
	}

	internal abstract class CustomLayout : ControlStyleLayout
	{
		#region Members

		private int m_TextRectHeight;

		#endregion

		public CustomLayout()
		{
		}

		#region Properties

		public override int BorderWidth
		{
			get 
			{ 
				if( Floating )
				{
					return ThickBorderWidth;
				}
				else
				{
					return ThinBorderWidth; 
				}
			}
		}

		public override int ThickBorderWidth
		{
			get
			{
				return DEF_THICK_BORDER_WIDTH;
			}
		}

		public override int ThinBorderWidth
		{
			get
			{
				return DEF_THIN_BORDER_WIDTH;
			}
		}

		protected override int ButtonMargin
		{
			get
			{
				return DEF_BUTTON_MARGIN;
			}
		}

		protected override int ButtonSeparator
		{
			get
			{
				return DEF_BUTTON_SEPARATOR_WIDTH;
			}
		}

		protected int ButtonCount
		{
			get
			{
				return Table.Buttons.Count;
			}
		}


		public override Rectangle TextBounds
		{
			get
			{
				Rectangle rectangle = Rectangle.Empty;
				rectangle.Y = CaptionBounds.Y + (CaptionWidth - TextRectHeight ) / 2;
				rectangle.Height = TextRectHeight;

				int imageWidth = (ImageEnabled)? ImageBounds.Width: 0;
				if( imageWidth == 0 && ImageEnabled )
					return Rectangle.Empty;

				int width =	 CaptionBounds.Width - TextLeftMargin - TextRightMargin - imageWidth;
				rectangle.Width = width > 0 ? width : 0;
				
				if( IsMirrored )
				{
					rectangle.X = TextRightMargin;
				}
				else
				{
					rectangle.X = TextLeftMargin + BorderWidth + imageWidth;
				}
				
				if( rectangle.Width <= 0 )
					return Rectangle.Empty;

				return rectangle;
			}
		}

		public override Rectangle ImageBounds
		{
			get
			{
				Rectangle rcImage = Rectangle.Empty;
				if( ImageEnabled )
				{
					int top = (CaptionBounds.Height - ImageSize) / 2 + BorderWidth;
					
					if( IsMirrored )
					{
						if( !(Table.Buttons.Count > 0 && GetButtonBounds( Table.Buttons.Count - 1 ).Right > CaptionBounds.Width - ImageSize - ImageLeftMargin) )
						{
							rcImage = new Rectangle(ImageRightMargin, top, ImageSize, ImageSize);
						}
					}
					else
					{
						if( !(Table.Buttons.Count > 0 && GetButtonBounds( Table.Buttons.Count - 1 ).Left < ImageLeftMargin + ImageSize ) )
						{
							rcImage = new Rectangle(ImageLeftMargin, top, ImageSize, ImageSize);
						}
					}
				}

				if (CaptionBounds.Contains(rcImage))
				{
					return rcImage;
				}
				else
				{
					return Rectangle.Empty;
				}
			}
		}

		protected abstract int ImageSize { get; }
		protected abstract int TextLeftMargin { get; }
		protected abstract int TextRightMargin { get; }
		protected abstract int ImageLeftMargin { get; }
		protected abstract int ImageRightMargin { get; }

		public override Rectangle CaptionBounds
		{
			get
			{
				if( CaptionEnabled )
				{
					return new Rectangle ( 
						ControlBounds.Left + BorderWidth,
						ControlBounds.Top + BorderWidth,
						ControlBounds.Left + ControlBounds.Width - 2 * BorderWidth,
						CaptionWidth 
						);
				}
				else
				{
					return Rectangle.Empty;
				}
			}
		}

		public override int TextRectHeight
		{
			get
			{
				return m_TextRectHeight;
			}
			set
			{
				if (m_TextRectHeight != value)
				{
					m_TextRectHeight = value;
				}
			}
		}

		public override Rectangle ThickBorderCaptionBounds
		{
			get
			{
				if( CaptionEnabled )
				{
					return new Rectangle (
						ControlBounds.Left + ThickBorderWidth,
						ControlBounds.Top + ThickBorderWidth,
						ControlBounds.Left + ControlBounds.Width - 2 * ThickBorderWidth,
						CaptionWidth
						);
				}
				else
				{
					return Rectangle.Empty;
				}
			}
		}

		public override Rectangle ThinBorderCaptionBounds
		{
			get
			{
				if( CaptionEnabled )
				{
					return new Rectangle (
						ControlBounds.Left + ThinBorderWidth,
						ControlBounds.Top + ThinBorderWidth,
						ControlBounds.Left + ControlBounds.Width - 2 * ThinBorderWidth,
						CaptionWidth
						);
				}
				else
				{
					return Rectangle.Empty;
				}
			}
		}
		#endregion

		#region Class constants

		private const int DEF_BUTTON_MARGIN = 3;
		private const int DEF_BUTTON_SEPARATOR_WIDTH = 2;
		private const int DEF_THIN_BORDER_WIDTH = 1;
		private const int DEF_THICK_BORDER_WIDTH = 4;

		#endregion

		#region Methods

		public override HitTestArea HitTest(MouseButtons button, Point point)
		{
			Rectangle nonBorderRect = ControlBounds;
			int index = -1;
			nonBorderRect.Inflate( -BorderWidth, -BorderWidth );
			if( ControlBounds.Contains( point ) && ( !nonBorderRect.Contains( point ) ))
			{
				return HitTestArea.Border;
			}
			else
			{
				if( CaptionBounds.Contains( point ) )
				{
					if( button == MouseButtons.Left || button == MouseButtons.None )
					{
						SetCaptionButtonIndex( button, point );
						index = GetHitButtonIndex();
						if( index >= 0 )
						{
							return HitTestArea.Button;
						}
					}
					
					return HitTestArea.Caption;
				}
				else
				{
					return HitTestArea.None;
				}
			}
		}


		#endregion

	}

	internal class LayoutOffice2003 : CustomLayout
	{
		#region Constructors

		public LayoutOffice2003() {}


		#endregion

		#region Properties

		public override int CaptionWidth
		{
			get 
			{ 
				if( DEF_CAPTION_WIDTH > SystemInformation.ToolWindowCaptionHeight )
				{
					return DEF_CAPTION_WIDTH; 
				}
				else
				{
					return SystemInformation.ToolWindowCaptionHeight;
				}
			}
		}

		protected override int ImageSize
		{
			get
			{
				return DEF_ICON_SIZE;
			}
		}
		public virtual Rectangle GripperRectangle
		{
			get
			{
				Rectangle rectangle = Rectangle.Empty;
				rectangle.Y = BorderWidth + DEF_GRIPPER_TOP_MARGIN;
				if( IsMirrored )
				{
					rectangle.X = ControlBounds.Width - BorderWidth 
						- DEF_GRIPPER_LEFT_MARGIN - 3;
				}
				else
				{
					rectangle.X = BorderWidth + DEF_GRIPPER_LEFT_MARGIN;
				}

				int minWidth = DEF_GRIPPER_LEFT_MARGIN + DEF_GRIPPER_WIDTH 
					+ ( ButtonSeparator + ButtonSize ) * ButtonCount
					+ ButtonMargin;
				
				if( minWidth > CaptionBounds.Width )
					return Rectangle.Empty;

				return rectangle;
			}
		}
		

		protected override int ButtonSize
		{
			get
			{
				return DEF_BUTTON_SIZE;
			}
		}

		protected override int TextLeftMargin
		{
			get
			{
				return DEF_GRIPPER_LEFT_MARGIN + DEF_GRIPPER_WIDTH + DEF_TEXT_SEPARATOR_WIDTH;
			}
		}

		protected override int TextRightMargin
		{
			get
			{
				return ButtonMargin + ( ButtonSize + ButtonSeparator ) * ButtonCount
					+ DEF_TEXT_SEPARATOR_WIDTH;
			}
		}

		protected override int ImageLeftMargin
		{
			get
			{
				return DEF_GRIPPER_LEFT_MARGIN + BorderWidth + DEF_TEXT_SEPARATOR_WIDTH + DEF_GRIPPER_WIDTH;
			}
		}

		protected override int ImageRightMargin
		{
			get
			{
				return CaptionBounds.Width - DEF_GRIPPER_LEFT_MARGIN - DEF_GRIPPER_WIDTH - ImageSize;
			}
		}

		public override int OuterBorderWidth
		{
			get 
			{
				if( Floating )
				{
					return DEF_THICK_OUTER_BORDER_WIDTH;
				}
				else
				{
					return DEF_THIN_OUTER_BORDER_WIDTH;
				}
			}
		}


		#endregion

		#region Class constants

		private const int DEF_CAPTION_WIDTH = 25;
		private const int DEF_BUTTON_SIZE = 15;
		private const int DEF_GRIPPER_LEFT_MARGIN = 4;
		private const int DEF_GRIPPER_TOP_MARGIN = 5;
		private const int DEF_GRIPPER_WIDTH = 3;
		private const int DEF_GRIPPER_HEIGHT = 15;
		private const int DEF_ICON_SIZE = 16;
		private const int DEF_TEXT_SEPARATOR_WIDTH = 3;
		private const int DEF_THICK_OUTER_BORDER_WIDTH = 2;
		private const int DEF_THIN_OUTER_BORDER_WIDTH = 1;

		#endregion
	}

	internal class LayoutVS2005 : CustomLayout
	{
		#region Constructors

		public LayoutVS2005() {}

		#endregion

		#region Properties

		public override int CaptionWidth
		{
			get
			{
				return SystemInformation.ToolWindowCaptionHeight;
			}
		}


		protected override int ButtonSize
		{
			get
			{
				return SystemInformation.ToolWindowCaptionButtonSize.Height;
			}
		}

		protected override int ImageSize
		{
			get
			{
				if( CaptionWidth - 2 > SystemInformation.SmallIconSize.Width )
				{
					return SystemInformation.SmallIconSize.Height;
				}
				else
				{
					return CaptionWidth - 2;
				}
			}
		}

		protected override int TextLeftMargin
		{
			get
			{
				return DEF_TEXT_SEPARATOR_WIDTH;
			}
		}

		protected override int TextRightMargin
		{
			get
			{
				return ButtonMargin + ( ButtonSize + ButtonSeparator ) * ButtonCount
					+ DEF_TEXT_SEPARATOR_WIDTH;
			}
		}

		protected override int ImageLeftMargin
		{
			get
			{
				return BorderWidth + DEF_TEXT_SEPARATOR_WIDTH;
			}
		}

		protected override int ImageRightMargin
		{
			get
			{
				return CaptionBounds.Width - ImageSize;
			}
		}

		public override int OuterBorderWidth
		{
			get 
			{
				return DEF_OUTER_BORDER_WIDTH;
			}
		}


		#endregion

		#region Class constants

		private const int DEF_TEXT_SEPARATOR_WIDTH = 3;
		private const int DEF_OUTER_BORDER_WIDTH = 1;

		#endregion
	}

    /// <summary>
    /// VS2010 layout appearance
    /// </summary>
    internal class LayoutVS2010 : CustomLayout
    {
        #region Constructors

        public LayoutVS2010() { }


        #endregion

        #region Properties

        public override int CaptionWidth
        {
            get
            {
                if (DEF_CAPTION_WIDTH > SystemInformation.ToolWindowCaptionHeight)
                {
                    return DEF_CAPTION_WIDTH;
                }
                else
                {
                    return SystemInformation.ToolWindowCaptionHeight;
                }
            }
        }

        protected override int ImageSize
        {
            get
            {
                return DEF_ICON_SIZE;
            }
        }
        public virtual Rectangle GripperRectangle
        {
            get
            {
                Rectangle rectangle = Rectangle.Empty;
                rectangle.Y = BorderWidth + DEF_GRIPPER_TOP_MARGIN;
                if (IsMirrored)
                {
                    rectangle.X = ControlBounds.Width - BorderWidth
                        - DEF_GRIPPER_LEFT_MARGIN - 3;
                }
                else
                {
                    rectangle.X = BorderWidth + DEF_GRIPPER_LEFT_MARGIN;
                }

                int minWidth = DEF_GRIPPER_LEFT_MARGIN + DEF_GRIPPER_WIDTH
                    + (ButtonSeparator + ButtonSize) * ButtonCount
                    + ButtonMargin;

                if (minWidth > CaptionBounds.Width)
                    return Rectangle.Empty;

                return rectangle;
            }
        }


        protected override int ButtonSize
        {
            get
            {
                return DEF_BUTTON_SIZE;
            }
        }

        protected override int TextLeftMargin
        {
            get
            {
                return DEF_GRIPPER_LEFT_MARGIN + DEF_GRIPPER_WIDTH + DEF_TEXT_SEPARATOR_WIDTH;
            }
        }

        protected override int TextRightMargin
        {
            get
            {
                return ButtonMargin + (ButtonSize + ButtonSeparator) * ButtonCount
                    + DEF_TEXT_SEPARATOR_WIDTH;
            }
        }

        protected override int ImageLeftMargin
        {
            get
            {
                return DEF_GRIPPER_LEFT_MARGIN + BorderWidth + DEF_TEXT_SEPARATOR_WIDTH + DEF_GRIPPER_WIDTH;
            }
        }

        protected override int ImageRightMargin
        {
            get
            {
                return CaptionBounds.Width - DEF_GRIPPER_LEFT_MARGIN - DEF_GRIPPER_WIDTH - ImageSize;
            }
        }

        public override int OuterBorderWidth
        {
            get
            {
                if (Floating)
                {
                    return DEF_THICK_OUTER_BORDER_WIDTH;
                }
                else
                {
                    return DEF_THIN_OUTER_BORDER_WIDTH;
                }
            }
        }


        #endregion

        #region Class constants

        private const int DEF_CAPTION_WIDTH = 25;
        private const int DEF_BUTTON_SIZE = 15;
        private const int DEF_GRIPPER_LEFT_MARGIN = 4;
        private const int DEF_GRIPPER_TOP_MARGIN = 5;
        private const int DEF_GRIPPER_WIDTH = 3;
        private const int DEF_GRIPPER_HEIGHT = 15;
        private const int DEF_ICON_SIZE = 16;
        private const int DEF_TEXT_SEPARATOR_WIDTH = 3;
        private const int DEF_THICK_OUTER_BORDER_WIDTH = 2;
        private const int DEF_THIN_OUTER_BORDER_WIDTH = 1;

        #endregion
    }

    internal class LayoutVS2012 : CustomLayout
    {
        #region Constructors

        public LayoutVS2012() { }

        #endregion

        #region Properties

        public override int CaptionWidth
        {
            get
            {
                return SystemInformation.ToolWindowCaptionHeight;
            }
        }


        protected override int ButtonSize
        {
            get
            {
                return SystemInformation.ToolWindowCaptionButtonSize.Height;
            }
        }

        protected override int ImageSize
        {
            get
            {
                if (CaptionWidth - 2 > SystemInformation.SmallIconSize.Width)
                {
                    return SystemInformation.SmallIconSize.Height;
                }
                else
                {
                    return CaptionWidth - 2;
                }
            }
        }

        protected override int TextLeftMargin
        {
            get
            {
                return DEF_TEXT_SEPARATOR_WIDTH;
            }
        }

        protected override int TextRightMargin
        {
            get
            {
                return ButtonMargin + (ButtonSize + ButtonSeparator) * ButtonCount
                    + DEF_TEXT_SEPARATOR_WIDTH;
            }
        }

        protected override int ImageLeftMargin
        {
            get
            {
                return BorderWidth + DEF_TEXT_SEPARATOR_WIDTH;
            }
        }

        protected override int ImageRightMargin
        {
            get
            {
                return CaptionBounds.Width - ImageSize;
            }
        }

        public override int OuterBorderWidth
        {
            get
            {
                return DEF_OUTER_BORDER_WIDTH;
            }
        }


        #endregion

        #region Class constants

        private const int DEF_TEXT_SEPARATOR_WIDTH = 3;
        private const int DEF_OUTER_BORDER_WIDTH = 1;

        #endregion
    }
	internal abstract class LayoutOffice2007Base : CustomLayout
	{
		#region Constructors

		public LayoutOffice2007Base() {}

		#endregion

		#region Properties

		public override int CaptionWidth
		{
			get
			{
				if( DEF_MIN_CAPTION_HEIGHT < SystemInformation.ToolWindowCaptionHeight )
				{
					return SystemInformation.ToolWindowCaptionHeight;
				}
				else
				{
					return DEF_MIN_CAPTION_HEIGHT;
				}
			}
		}


		protected override int ButtonSize
		{
			get
			{
				if( DEF_MIN_BUTTON_SIZE < SystemInformation.ToolWindowCaptionButtonSize.Height )
				{
					return SystemInformation.ToolWindowCaptionButtonSize.Height;
				}
				else
				{
					return DEF_MIN_BUTTON_SIZE;
				}
			}
		}

		protected override int ImageSize
		{
			get
			{
				if( CaptionWidth - 2 > SystemInformation.SmallIconSize.Width )
				{
					return SystemInformation.SmallIconSize.Height;
				}
				else
				{
					return CaptionWidth - 2;
				}
			}
		}

		protected override int TextLeftMargin
		{
			get
			{
				return DEF_TEXT_SEPARATOR_WIDTH;
			}
		}

		protected override int TextRightMargin
		{
			get
			{
				return ButtonMargin + ( ButtonSize + ButtonSeparator ) * ButtonCount
					+ DEF_TEXT_SEPARATOR_WIDTH;
			}
		}

		protected override int ImageLeftMargin
		{
			get
			{
				return BorderWidth + DEF_TEXT_SEPARATOR_WIDTH;
			}
		}

		protected override int ImageRightMargin
		{
			get
			{
				return CaptionBounds.Width - ImageSize;
			}
		}

		public override int ThinBorderWidth
		{
			get
			{
				return DEF_BORDER_WIDTH;
			}
		}

		public override int ThickBorderWidth
		{
			get
			{
				return DEF_BORDER_WIDTH;
			}
		}

		public override int OuterBorderWidth
		{
			get 
			{
				return DEF_OUTER_BORDER_WIDTH;
			}
		}

		public abstract Rectangle CaptionUpperBounds {get; }

		public abstract Rectangle CaptionLowerBounds {get; }
		#endregion

		#region Class constants

		private const int DEF_TEXT_SEPARATOR_WIDTH = 4;
		private const int DEF_MIN_CAPTION_HEIGHT = 20;
		private const int DEF_MIN_BUTTON_SIZE = 17;
		private const int DEF_BORDER_WIDTH = 3;
		private const int DEF_OUTER_BORDER_WIDTH = 3;

		#endregion
	}

	internal class LayoutOffice2007 : LayoutOffice2007Base
	{
		#region Properties

		public override Rectangle CaptionUpperBounds
		{
			get 
			{
				return new Rectangle(
					CaptionBounds.Left,
					CaptionBounds.Top,
					CaptionBounds.Width,
					CaptionWidth * DEF_UPPER_COEFF / (DEF_UPPER_COEFF + DEF_LOWER_COEFF)
					);
			}
		}

		public override Rectangle CaptionLowerBounds
		{
			get
			{
				return new Rectangle(
					CaptionBounds.Left,
					CaptionBounds.Top + CaptionUpperBounds.Height - 1,
					CaptionBounds.Width,
					CaptionWidth - CaptionUpperBounds.Height + 1
					);
			}
		}

		#endregion

		#region Class constants

		private const int DEF_UPPER_COEFF = 1;
		private const int DEF_LOWER_COEFF = 3;

		#endregion
		}

    internal abstract class LayoutOffice2010Base : CustomLayout
    {
        #region Constructors

        public LayoutOffice2010Base() { }

        #endregion

        #region Properties

        public override int CaptionWidth
        {
            get
            {
                if (DEF_MIN_CAPTION_HEIGHT < SystemInformation.ToolWindowCaptionHeight)
                {
                    return SystemInformation.ToolWindowCaptionHeight;
                }
                else
                {
                    return DEF_MIN_CAPTION_HEIGHT;
                }
            }
        }


        protected override int ButtonSize
        {
            get
            {
                if (DEF_MIN_BUTTON_SIZE < SystemInformation.ToolWindowCaptionButtonSize.Height)
                {
                    return SystemInformation.ToolWindowCaptionButtonSize.Height;
                }
                else
                {
                    return DEF_MIN_BUTTON_SIZE;
                }
            }
        }

        protected override int ImageSize
        {
            get
            {
                if (CaptionWidth - 2 > SystemInformation.SmallIconSize.Width)
                {
                    return SystemInformation.SmallIconSize.Height;
                }
                else
                {
                    return CaptionWidth - 2;
                }
            }
        }

        protected override int TextLeftMargin
        {
            get
            {
                return DEF_TEXT_SEPARATOR_WIDTH;
            }
        }

        protected override int TextRightMargin
        {
            get
            {
                return ButtonMargin + (ButtonSize + ButtonSeparator) * ButtonCount
                    + DEF_TEXT_SEPARATOR_WIDTH;
            }
        }

        protected override int ImageLeftMargin
        {
            get
            {
                return BorderWidth + DEF_TEXT_SEPARATOR_WIDTH;
            }
        }

        protected override int ImageRightMargin
        {
            get
            {
                return CaptionBounds.Width - ImageSize;
            }
        }

        public override int ThinBorderWidth
        {
            get
            {
                return DEF_BORDER_WIDTH;
            }
        }

        public override int ThickBorderWidth
        {
            get
            {
                return DEF_BORDER_WIDTH;
            }
        }

        public override int OuterBorderWidth
        {
            get
            {
                return DEF_OUTER_BORDER_WIDTH;
            }
        }

        public abstract Rectangle CaptionUpperBounds { get; }

        public abstract Rectangle CaptionLowerBounds { get; }
        #endregion

        #region Class constants

        private const int DEF_TEXT_SEPARATOR_WIDTH = 4;
        private const int DEF_MIN_CAPTION_HEIGHT = 20;
        private const int DEF_MIN_BUTTON_SIZE = 17;
        private const int DEF_BORDER_WIDTH = 3;
        private const int DEF_OUTER_BORDER_WIDTH = 3;

        #endregion
    }

    internal class LayoutOffice2010 : LayoutOffice2010Base
    {
        #region Properties

        public override Rectangle CaptionUpperBounds
        {
            get
            {
                return new Rectangle(
                    CaptionBounds.Left,
                    CaptionBounds.Top,
                    CaptionBounds.Width,
                    CaptionWidth * DEF_UPPER_COEFF / (DEF_UPPER_COEFF + DEF_LOWER_COEFF)
                    );
            }
        }

        public override Rectangle CaptionLowerBounds
        {
            get
            {
                return new Rectangle(
                    CaptionBounds.Left,
                    CaptionBounds.Top + CaptionUpperBounds.Height - 1,
                    CaptionBounds.Width,
                    CaptionWidth - CaptionUpperBounds.Height + 1
                    );
            }
        }

        #endregion

        #region Class constants

        private const int DEF_UPPER_COEFF = 1;
        private const int DEF_LOWER_COEFF = 3;

        #endregion
    }
	internal class LayoutOffice2007Outlook : LayoutOffice2007Base
	{
		#region Properties

		public override Rectangle CaptionUpperBounds
		{
			get
			{
				return new Rectangle(
					CaptionBounds.Left,
					CaptionBounds.Top,
					CaptionBounds.Width,
					CaptionBounds.Height * DEF_UPPER_COEFF / (DEF_UPPER_COEFF + DEF_LOWER_COEFF)
					);
			}
		}

		public override Rectangle CaptionLowerBounds
		{
			get
			{
				return new Rectangle(
					CaptionBounds.Left,
					CaptionUpperBounds.Top - 1 + CaptionUpperBounds.Height,
					CaptionBounds.Width,
					CaptionBounds.Height - CaptionUpperBounds.Height + 1
					);
			}
		}

		#endregion

		#region Class constants

		private const int DEF_UPPER_COEFF = 1;
		private const int DEF_LOWER_COEFF = 1;

		#endregion
	}
}
