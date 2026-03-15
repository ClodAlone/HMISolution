#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region File Using
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Represents renderer of the Office2007 visual style for docked CommandBar.
	/// </summary>
	internal class CommandBarRendererOffice2007 : CommandBarRenderer
	{
		#region Class Properties
		/// <summary>
		/// Gets or sets color table for Office2007 visual style.
		/// </summary>
		private Office2007Colors ColorTable
		{
			get
			{
				return this.CmdBar.Office2007ColorTable;
			}
		}
		#endregion

		#region Class Initialize/Finalize Methods
		public CommandBarRendererOffice2007( CommandBar commandBar ) : base( commandBar )
		{
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Draws text of the CommandBar with Office2007 visual style.
		/// </summary>
		protected override void DrawText( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			string text = this.CmdBar.Text;
			Font textFont = this.CmdBar.Font;
			Color foreColor = this.CmdBar.ForeColor;
			CommandBarPainter.PaintDockedText( g, rect, text, textFont, foreColor, bRTL, bVertical );
		}
		/// <summary>
		/// Draws background of the CommandBar with Office2007 visual style.
		/// </summary>
		protected override void DrawBackground( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			if( !IsMainCommandBar() )
			{
				CommandBarPainter.PaintDockedBackground( g, bRTL, bVertical, rect,
					ColorTable.CommandBarLightColor,
					ColorTable.CommandBarDarkColor,
					ColorTable.CommandBarBorderColor );
			}
		}

		/// <summary>
		/// Draws DropBownButton of the CommandBar with Office2007 visual style.
		/// </summary>
		protected override void DrawDropDown( Graphics g, Rectangle rect, bool bRTL, bool bVertical,
			CBButtonState state, bool bShowChevron, bool bShowArrow )
		{
			Color chevronColor = this.CmdBar.ChevronColor;
			Color startColor = Color.Empty;
			Color endColor = Color.Empty;

			switch( state )
			{
				case CBButtonState.Hot :
				{
					startColor = ( bVertical && bRTL ) ? ColorTable.DropDownHighlightDarkColor : 
						ColorTable.DropDownHighlightLightColor;
					endColor = ( bVertical && bRTL ) ? ColorTable.DropDownHighlightLightColor : 
						ColorTable.DropDownHighlightDarkColor;
					break;
				}
				case CBButtonState.Pressed :
				{
					startColor = ( bVertical && bRTL ) ? ColorTable.DropDownPressedDarkColor : 
						ColorTable.DropDownPressedLightColor;
					endColor = ( bVertical && bRTL ) ? ColorTable.DropDownPressedLightColor : 
						ColorTable.DropDownPressedDarkColor;
					break;
				}
				default : 
				{
					startColor = ( bVertical && bRTL ) ? ColorTable.DropDownDarkColor : 
						ColorTable.DropDownLightColor;
					endColor = ( bVertical && bRTL ) ? ColorTable.DropDownLightColor : 
						ColorTable.DropDownDarkColor;
					break;
				}
			}

			CommandBarPainter.PaintDropDownButton( g, rect, bRTL, bVertical, bShowChevron,
				startColor, endColor, chevronColor, bShowArrow );
		}
		/// <summary>
		/// Draws gripper of the CommandBar with Office2007 visual style.
		/// </summary>
		protected override void DrawGripper( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			CommandBarPainter.PaintGripper( g, rect, bRTL, bVertical, 
				Color.White, ColorTable.CommandBarBorderColor );
		}

		#endregion
	}

	/// <summary>
	/// Represents renderer of the Office2007 visual style for floating CommandBar.
	/// </summary>
	internal class CommandBarFloatingRendererOffice2007 : CommandBarFloatingRenderer
	{
		#region Class Properties
		/// <summary>
		/// Gets or sets color table for Office2007 visual style.
		/// </summary>
		private Office2007Colors ColorTable
		{
			get
			{
				return this.CmdBar.Office2007ColorTable;
			}
		}
		#endregion

		#region Class Initialize/Finalize Methods
		public CommandBarFloatingRendererOffice2007( CommandBar commandBar ) : base( commandBar )
		{
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Draws text of the CommandBar with Office2007 visual style.
		/// </summary>
		protected override void DrawText( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			string text = this.CmdBar.Text;
			Color foreColor = Color.White;
			Font textFont = this.CmdBar.GetFloatinCaptionFont();
			CommandBarPainter.PaintFloatingText( g, rect, text, textFont, foreColor, bRTL );
		}
		/// <summary>
		/// Draws background of the CommandBar with Office2007 visual style.
		/// </summary>
		protected override void DrawBackground( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			Color backColor = ColorTable.FloatBackgroundColor;
			Color startColor = ColorTable.FloatCommandBarLightColor;
			Color endColor = ColorTable.FloatCommandBarDarkColor;
			Color borderColor = ColorTable.FloatLightBorderColor;

			Rectangle captionRect = this.CmdBar.CaptionRect;
			Rectangle barRect = new Rectangle( rect.Left + 1, 
				rect.Top + captionRect.Height + 1, 
				rect.Width - 2, rect.Height - 2 );

			CommandBarPainter.PaintFloatingBackground( g, rect, barRect, 
				backColor, borderColor, startColor, endColor );
		}

		/// <summary>
		/// Draws DropBownButton of the CommandBar with Office2007 visual style.
		/// </summary>
		protected override void DrawDropDown( Graphics g, Rectangle rect, bool bRTL, bool bVertical,
			CBButtonState state, bool bShowChevron, bool bShowArrow )
		{
			Color borderColor = Color.Empty;
			Color fillColor = Color.Empty;
			Color buttonColor = Color.Empty;

			switch( state )
			{
				case CBButtonState.Hot :
				{
					borderColor = ColorTable.FloatHighlightButtonBorderColor;
					fillColor = ColorTable.FloatHighlightButtonColor;
					buttonColor = SystemColors.ControlText;
					break;
				}
				case CBButtonState.Pressed :
				{
					borderColor = ColorTable.FloatPressButtonBorderColor;
					fillColor = ColorTable.FloatPressButtonColor;
					buttonColor = SystemColors.ControlText;
					break;
				}
				default :
				{
					borderColor = Color.Empty;
					fillColor = Color.Empty;
					buttonColor = Color.White;
					break;
				}
			}

			CommandBarPainter.PaintFloatingDropDownButton( g, rect, borderColor, fillColor, buttonColor );
		}
		/// <summary>
		/// Draws gripper of the CommandBar with Office2007 visual style.
		/// </summary>
		protected override void DrawGripper( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			// doesn't need draw gripper
		}
		/// <summary>
		/// Draws close button of the floating CommandBar.
		/// </summary>
		protected override void DrawCloseButton( Graphics g, Rectangle rect, CBButtonState state )
		{
			Color borderColor = Color.Empty;
			Color fillColor = Color.Empty;
			Color buttonColor = Color.Empty;

			switch( state )
			{
				case CBButtonState.Hot :
				{
					borderColor = ColorTable.FloatHighlightButtonBorderColor;
					fillColor = ColorTable.FloatHighlightButtonColor;
					buttonColor = SystemColors.ControlText;
					break;
				}
				case CBButtonState.Pressed :
				{
					borderColor = ColorTable.FloatPressCloseButtonBorderColor;
					fillColor = ColorTable.FloatPressCloseButtonColor;
					buttonColor = SystemColors.ControlText;
					break;
				}
				default :
				{
					borderColor = Color.Empty;
					fillColor = Color.Empty;
					buttonColor = Color.White;
					break;
				}
			}

			CommandBarPainter.PaintFloatingCloseButton( g, rect, borderColor, fillColor, buttonColor );
		}
		#endregion
	}

	/// <summary>
	/// Represents renderer of the Office2007 visual style for docked ControlBar.
	/// </summary>
	internal class ControlBarRendererOffice2007 : ControlBarRenderer
	{
		#region Class Properties
		/// <summary>
		/// Gets or sets color table for Office2007 visual style.
		/// </summary>
		private Office2007Colors ColorTable
		{
			get
			{
				return this.CmdBar.Office2007ColorTable;
			}
		}
		#endregion

		#region Class Initialize/Finalize Methods
		public ControlBarRendererOffice2007( ControlBar controlBar ) : base( controlBar )
		{
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Draws text of the ControlBar with Office2007 visual style.
		/// </summary>
		protected override void DrawText(Graphics g, Rectangle rect, bool bRTL, bool bVertical)
		{
			string text = this.ControlBar.Text;
			Color foreColor = this.ControlBar.ForeColor;
			Font textFont = this.ControlBar.Font;
			CommandBarPainter.PaintControlBarText( g, rect, text, textFont, foreColor, bRTL );
		}

		/// <summary>
		/// Draws background of the ControlBar with Office2007 visual style.
		/// </summary>
		protected override void DrawBackground(Graphics g, Rectangle rect, bool bRTL, bool bVerticalt)
		{
			Color borderColor = ColorTable.CommandBarLightColor;
			Color lightColor = ColorTable.CommandBarLightColor;
			Color darkColor = ColorTable.CommandBarDarkColor;
			Color activeLightColor = ColorTable.MenuItemLightColor;
			Color activeDarkColor = ColorTable.MenuItemDarkColor;

			Rectangle captionRect = this.ControlBar.CaptionRect;
			bool bContainFocus = ( this.ControlBar.ClientControl != null ) 
				&& ( this.ControlBar.ClientControl.ContainsFocus == true );

			CommandBarPainter.PaintControlBarBackground( g, rect, captionRect, bRTL, bContainFocus,
				borderColor, lightColor, darkColor, activeLightColor, activeDarkColor );
		}

		/// <summary>
		/// Draws DropBownButton of the ControlBar with Office2007 visual style.
		/// </summary>
		protected override void DrawDropDown( Graphics g, Rectangle rect, bool bRTL, bool bVertical, Syncfusion.Windows.Forms.Tools.CBButtonState state, bool bShowChevron, bool bShowArrow )
		{
			Color borderColor = Color.Empty;
			Color fillColor = Color.Empty;

			if( state == CBButtonState.Hot )
			{
				borderColor = ColorTable.FloatHighlightButtonBorderColor;
				fillColor = ColorTable.FloatHighlightButtonColor;
			}
			else if( state == CBButtonState.Pressed )
			{
				borderColor = ColorTable.FloatPressButtonBorderColor;
				fillColor = ColorTable.FloatPressButtonColor;
			}

			CommandBarPainter.PaintControlBarDropDown( g, rect, bRTL, borderColor, fillColor );
		}

		/// <summary>
		/// Draws close button of the ControlBar.
		/// </summary>
		protected override void DrawCloseButton(Graphics g, Rectangle rect, Syncfusion.Windows.Forms.Tools.CBButtonState state)
		{
			Color borderColor = Color.Empty;
			Color fillColor = Color.Empty;

			switch( state )
			{
				case CBButtonState.Hot :
				{
					borderColor = ColorTable.FloatHighlightButtonBorderColor;
					fillColor = ColorTable.FloatHighlightButtonColor;
					break;
				}
				case CBButtonState.Pressed :
				{
					borderColor = ColorTable.FloatPressCloseButtonBorderColor;
					fillColor = ColorTable.FloatPressCloseButtonColor;
					break;
				}
				default :
				{
					borderColor = Color.Empty;
					fillColor = Color.Empty;
					break;
				}
			}

			CommandBarPainter.PaintControlBarCloseButton( g, rect, borderColor, fillColor );
		}
		/// <summary>
		/// Draws gripper of the ControlBar with Office2007 visual style.
		/// </summary>
		protected override void DrawGripper(Graphics g, Rectangle rect, bool bRTL, bool bVertical)
		{
			CommandBarPainter.PaintGripper( g, rect, bRTL, false, 
				Color.White, ColorTable.CommandBarBorderColor );
		}

		#endregion
	}

    /// <summary>
    /// Represents renderer of the Office2010 visual style for docked CommandBar.
    /// </summary>
    internal class CommandBarRendererOffice2010 : CommandBarRenderer
    {
        #region Class Properties
        /// <summary>
        /// Gets or sets color table for Office2010 visual style.
        /// </summary>
        private Office2010Colors ColorTable
        {
            get
            {
                return this.CmdBar.Office2010ColorTable;
            }
        }
        #endregion

        #region Class Initialize/Finalize Methods
        public CommandBarRendererOffice2010(CommandBar commandBar)
            : base(commandBar)
        {
        }
        #endregion

        #region Class Overrides
        /// <summary>
        /// Draws text of the CommandBar with Office2010 visual style.
        /// </summary>
        protected override void DrawText(Graphics g, Rectangle rect, bool bRTL, bool bVertical)
        {
            string text = this.CmdBar.Text;
            Font textFont = this.CmdBar.Font;
            Color foreColor = this.CmdBar.ForeColor;
            CommandBarPainter.PaintDockedText(g, rect, text, textFont, foreColor, bRTL, bVertical);
        }
        /// <summary>
        /// Draws background of the CommandBar with Office2010 visual style.
        /// </summary>
        protected override void DrawBackground(Graphics g, Rectangle rect, bool bRTL, bool bVertical)
        {
            if (!IsMainCommandBar())
            {
                CommandBarPainter.PaintDockedBackground(g, bRTL, bVertical, rect,
                    ColorTable.CommandBarLightColor,
                    ColorTable.CommandBarDarkColor,
                    ColorTable.CommandBarBorderColor);
            }
        }

        /// <summary>
        /// Draws DropBownButton of the CommandBar with Office2010 visual style.
        /// </summary>
        protected override void DrawDropDown(Graphics g, Rectangle rect, bool bRTL, bool bVertical,
            CBButtonState state, bool bShowChevron, bool bShowArrow)
        {
            Color chevronColor = this.CmdBar.ChevronColor;
            Color startColor = Color.Empty;
            Color endColor = Color.Empty;

            switch (state)
            {
                case CBButtonState.Hot:
                    {
                        startColor = (bVertical && bRTL) ? ColorTable.DropDownHighlightDarkColor :
                            ColorTable.DropDownHighlightLightColor;
                        endColor = (bVertical && bRTL) ? ColorTable.DropDownHighlightLightColor :
                            ColorTable.DropDownHighlightDarkColor;
                        break;
                    }
                case CBButtonState.Pressed:
                    {
                        startColor = (bVertical && bRTL) ? ColorTable.DropDownPressedDarkColor :
                            ColorTable.DropDownPressedLightColor;
                        endColor = (bVertical && bRTL) ? ColorTable.DropDownPressedLightColor :
                            ColorTable.DropDownPressedDarkColor;
                        break;
                    }
                default:
                    {
                        startColor = (bVertical && bRTL) ? ColorTable.DropDownDarkColor :
                            ColorTable.DropDownLightColor;
                        endColor = (bVertical && bRTL) ? ColorTable.DropDownLightColor :
                            ColorTable.DropDownDarkColor;
                        break;
                    }
            }

            CommandBarPainter.PaintDropDownButton(g, rect, bRTL, bVertical, bShowChevron,
                startColor, endColor, chevronColor, bShowArrow);
        }
        /// <summary>
        /// Draws gripper of the CommandBar with Office2010 visual style.
        /// </summary>
        protected override void DrawGripper(Graphics g, Rectangle rect, bool bRTL, bool bVertical)
        {
            CommandBarPainter.PaintGripper(g, rect, bRTL, bVertical,
                Color.White,ControlPaint.Dark( ColorTable.ComboButtonBorder));
        }

        #endregion
    }

    /// <summary>
    /// Represents renderer of the Office2010 visual style for floating CommandBar.
    /// </summary>
    internal class CommandBarFloatingRendererOffice2010 : CommandBarFloatingRenderer
    {
        #region Class Properties
        /// <summary>
        /// Gets or sets color table for Office2010 visual style.
        /// </summary>
        private Office2010Colors ColorTable
        {
            get
            {
                return this.CmdBar.Office2010ColorTable;
            }
        }
        #endregion

        #region Class Initialize/Finalize Methods
        public CommandBarFloatingRendererOffice2010(CommandBar commandBar)
            : base(commandBar)
        {
        }
        #endregion

        #region Class Overrides
        /// <summary>
        /// Draws text of the CommandBar with Office2010 visual style.
        /// </summary>
        protected override void DrawText(Graphics g, Rectangle rect, bool bRTL, bool bVertical)
        {
            string text = this.CmdBar.Text;
            Color foreColor = Color.White;
            Font textFont = this.CmdBar.GetFloatinCaptionFont();
            CommandBarPainter.PaintFloatingText(g, rect, text, textFont, foreColor, bRTL);
        }
        /// <summary>
        /// Draws background of the CommandBar with Office2010 visual style.
        /// </summary>
        protected override void DrawBackground(Graphics g, Rectangle rect, bool bRTL, bool bVertical)
        {
            Color backColor = ColorTable.FloatBackgroundColor;
            Color startColor = ColorTable.FloatCommandBarLightColor;
            Color endColor = ColorTable.FloatCommandBarDarkColor;
            Color borderColor = ColorTable.FloatLightBorderColor;

            Rectangle captionRect = this.CmdBar.CaptionRect;
            Rectangle barRect = new Rectangle(rect.Left + 1,
                rect.Top + captionRect.Height + 1,
                rect.Width - 2, rect.Height - 2);

            CommandBarPainter.PaintFloatingBackground(g, rect, barRect,
                backColor, borderColor, startColor, endColor);
        }

        /// <summary>
        /// Draws DropBownButton of the CommandBar with Office2010 visual style.
        /// </summary>
        protected override void DrawDropDown(Graphics g, Rectangle rect, bool bRTL, bool bVertical,
            CBButtonState state, bool bShowChevron, bool bShowArrow)
        {
            Color borderColor = Color.Empty;
            Color fillColor = Color.Empty;
            Color buttonColor = Color.Empty;

            switch (state)
            {
                case CBButtonState.Hot:
                    {
                        borderColor = ColorTable.FloatHighlightButtonBorderColor;
                        fillColor = ColorTable.FloatHighlightButtonColor;
                        buttonColor = SystemColors.ControlText;
                        break;
                    }
                case CBButtonState.Pressed:
                    {
                        borderColor = ColorTable.FloatPressButtonBorderColor;
                        fillColor = ColorTable.FloatPressButtonColor;
                        buttonColor = SystemColors.ControlText;
                        break;
                    }
                default:
                    {
                        borderColor = Color.Empty;
                        fillColor = Color.Empty;
                        buttonColor = Color.White;
                        break;
                    }
            }

            CommandBarPainter.PaintFloatingDropDownButton(g, rect, borderColor, fillColor, buttonColor);
        }
        /// <summary>
        /// Draws gripper of the CommandBar with Office2010 visual style.
        /// </summary>
        protected override void DrawGripper(Graphics g, Rectangle rect, bool bRTL, bool bVertical)
        {
            // doesn't need draw gripper
        }
        /// <summary>
        /// Draws close button of the floating CommandBar.
        /// </summary>
        protected override void DrawCloseButton(Graphics g, Rectangle rect, CBButtonState state)
        {
            Color borderColor = Color.Empty;
            Color fillColor = Color.Empty;
            Color buttonColor = Color.Empty;

            switch (state)
            {
                case CBButtonState.Hot:
                    {
                        borderColor = ColorTable.FloatHighlightButtonBorderColor;
                        fillColor = ColorTable.FloatHighlightButtonColor;
                        buttonColor = SystemColors.ControlText;
                        break;
                    }
                case CBButtonState.Pressed:
                    {
                        borderColor = ColorTable.FloatPressCloseButtonBorderColor;
                        fillColor = ColorTable.FloatPressCloseButtonColor;
                        buttonColor = SystemColors.ControlText;
                        break;
                    }
                default:
                    {
                        borderColor = Color.Empty;
                        fillColor = Color.Empty;
                        buttonColor = Color.White;
                        break;
                    }
            }

            CommandBarPainter.PaintFloatingCloseButton(g, rect, borderColor, fillColor, buttonColor);
        }
        #endregion
    }

    /// <summary>
    /// Represents renderer of the Office2010 visual style for docked ControlBar.
    /// </summary>
    internal class ControlBarRendererOffice2010 : ControlBarRenderer
    {
        #region Class Properties
        /// <summary>
        /// Gets or sets color table for Office2010 visual style.
        /// </summary>
        private Office2010Colors ColorTable
        {
            get
            {
                return this.CmdBar.Office2010ColorTable;
            }
        }
        #endregion

        #region Class Initialize/Finalize Methods
        public ControlBarRendererOffice2010(ControlBar controlBar)
            : base(controlBar)
        {
        }
        #endregion

        #region Class Overrides
        /// <summary>
        /// Draws text of the ControlBar with Office2010 visual style.
        /// </summary>
        protected override void DrawText(Graphics g, Rectangle rect, bool bRTL, bool bVertical)
        {
            string text = this.ControlBar.Text;
            Color foreColor = this.ControlBar.ForeColor;
            Font textFont = this.ControlBar.Font;
            CommandBarPainter.PaintControlBarText(g, rect, text, textFont, foreColor, bRTL);
        }

        /// <summary>
        /// Draws background of the ControlBar with Office2010 visual style.
        /// </summary>
        protected override void DrawBackground(Graphics g, Rectangle rect, bool bRTL, bool bVerticalt)
        {
            Color borderColor = ColorTable.CommandBarLightColor;
            Color lightColor = ColorTable.CommandBarLightColor;
            Color darkColor = ColorTable.CommandBarDarkColor;
            Color activeLightColor = ColorTable.MenuItemLightColor;
            Color activeDarkColor = ColorTable.MenuItemDarkColor;

            Rectangle captionRect = this.ControlBar.CaptionRect;
            bool bContainFocus = (this.ControlBar.ClientControl != null)
                && (this.ControlBar.ClientControl.ContainsFocus == true);

            CommandBarPainter.PaintControlBarBackground(g, rect, captionRect, bRTL, bContainFocus,
                borderColor, lightColor, darkColor, activeLightColor, activeDarkColor);
        }

        /// <summary>
        /// Draws DropBownButton of the ControlBar with Office2010 visual style.
        /// </summary>
        protected override void DrawDropDown(Graphics g, Rectangle rect, bool bRTL, bool bVertical, Syncfusion.Windows.Forms.Tools.CBButtonState state, bool bShowChevron, bool bShowArrow)
        {
            Color borderColor = Color.Empty;
            Color fillColor = Color.Empty;

            if (state == CBButtonState.Hot)
            {
                borderColor = ColorTable.FloatHighlightButtonBorderColor;
                fillColor = ColorTable.FloatHighlightButtonColor;
            }
            else if (state == CBButtonState.Pressed)
            {
                borderColor = ColorTable.FloatPressButtonBorderColor;
                fillColor = ColorTable.FloatPressButtonColor;
            }

            CommandBarPainter.PaintControlBarDropDown(g, rect, bRTL, borderColor, fillColor);
        }

        /// <summary>
        /// Draws close button of the ControlBar.
        /// </summary>
        protected override void DrawCloseButton(Graphics g, Rectangle rect, Syncfusion.Windows.Forms.Tools.CBButtonState state)
        {
            Color borderColor = Color.Empty;
            Color fillColor = Color.Empty;

            switch (state)
            {
                case CBButtonState.Hot:
                    {
                        borderColor = ColorTable.FloatHighlightButtonBorderColor;
                        fillColor = ColorTable.FloatHighlightButtonColor;
                        break;
                    }
                case CBButtonState.Pressed:
                    {
                        borderColor = ColorTable.FloatPressCloseButtonBorderColor;
                        fillColor = ColorTable.FloatPressCloseButtonColor;
                        break;
                    }
                default:
                    {
                        borderColor = Color.Empty;
                        fillColor = Color.Empty;
                        break;
                    }
            }

            CommandBarPainter.PaintControlBarCloseButton(g, rect, borderColor, fillColor);
        }
        /// <summary>
        /// Draws gripper of the ControlBar with Office2010 visual style.
        /// </summary>
        protected override void DrawGripper(Graphics g, Rectangle rect, bool bRTL, bool bVertical)
        {
            CommandBarPainter.PaintGripper(g, rect, bRTL, false,
                Color.White,ControlPaint.Dark( ColorTable.ComboButtonBorder));
        }

        #endregion
    }
}
