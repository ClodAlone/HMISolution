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
using System.Drawing;

using Syncfusion.Documentation;
#endregion

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// This should be called WindowsXPColorAndLayoutScheme.
	/// Specifies the layout and colors for buttons and combo box buttons.
	/// </summary>
	[ DocumentationExclude() ]
	public enum WindowsXPColorScheme
	{
		/// <summary></summary>
		DefaultBlue = 1,
		/// <summary></summary>
		OliveGreen = 2,
		/// <summary></summary>
		Silver = 3,
		/// <summary></summary>
		DefaultBlueCombo = 4,
		/// <summary></summary>
		OliveGreenCombo = 5,
		/// <summary></summary>
		SilverCombo = 6
	}

    /// <summary>
    /// Defines the different colors that will be used to define the 3 Windows XP
    /// color schemes supported.
    /// </summary>
    [DocumentationExclude()]
    public class WindowsXPColors
    {
        #region Class members
        /// <summary></summary>
        private static Color defaultBlueTopColor = Color.Empty;
        /// <summary></summary>
        private static Color defaultBlueBottomColor = Color.Empty;
        /// <summary></summary>
        private static Color defaultBlueGradientStartColor = Color.Empty;
        /// <summary></summary>
        private static Color defaultBlueGradientEndColor = Color.Empty;
        /// <summary></summary>
        private static Color defaultBlueMouseOverColor = Color.Empty;
        /// <summary></summary>
        private static Color defaultBlueBorderColor = Color.Empty;
        /// <summary></summary>
        private static Color defaultBlueComboButtonColor = Color.Empty;
        /// <summary></summary>
        private static Color defaultBlueComboButtonMouseOverColor = Color.Empty;
        /// <summary></summary>
        private static Color defaultBlueComboButtonPressedColor = Color.Empty;
        /// <summary></summary>
        private static Color defaultBlueComboArcColor = Color.Empty;
        /// <summary></summary>
        private static Color defaultBlueComboBorderColor = Color.Empty;
        /// <summary></summary>
        private static Color defaultBlueComboButtonBorderColor = Color.Empty;
        /// <summary></summary>
        private static Color oliveGreenTopColor = Color.Empty;
        /// <summary></summary>
        private static Color oliveGreenBottomColor = Color.Empty;
        /// <summary></summary>
        private static Color oliveGreenGradientStartColor = Color.Empty;
        /// <summary></summary>
        private static Color oliveGreenGradientEndColor = Color.Empty;
        /// <summary></summary>
        private static Color oliveGreenMouseOverColor = Color.Empty;
        /// <summary></summary>
        private static Color oliveGreenBorderColor = Color.Empty;
        /// <summary></summary>
        private static Color oliveGreenComboButtonColor = Color.Empty;
        /// <summary></summary>
        private static Color oliveGreenComboButtonMouseOverColor = Color.Empty;
        /// <summary></summary>
        private static Color oliveGreenComboButtonPressedColor = Color.Empty;
        /// <summary></summary>
        private static Color oliveGreenComboArcColor = Color.Empty;
        /// <summary></summary>
        private static Color oliveGreenComboBorderColor = Color.Empty;
        /// <summary></summary>
        private static Color oliveGreenComboButtonBorderColor = Color.Empty;
        /// <summary></summary>
        private static Color silverTopColor = Color.Empty;
        /// <summary></summary>
        private static Color silverBottomColor = Color.Empty;
        /// <summary></summary>
        private static Color silverGradientStartColor = Color.Empty;
        /// <summary></summary>
        private static Color silverGradientEndColor = Color.Empty;
        /// <summary></summary>
        private static Color silverMouseOverColor = Color.Empty;
        /// <summary></summary>
        private static Color silverBorderColor = Color.Empty;
        /// <summary></summary>
        private static Color silverComboBorderColor = Color.Empty;
        #endregion

        #region Class properties
        /// <summary></summary>
        public static Color DefaultBlueTopColor
        {
            get
            {
                if (defaultBlueTopColor != Color.Empty)
                {
                    return defaultBlueTopColor;
                }
                else
                {
                    defaultBlueTopColor = Color.FromArgb(244, 244, 240);
                    return defaultBlueTopColor;
                }
            }
        }

        /// <summary></summary>
        public static Color DefaultBlueBottomColor
        {
            get
            {
                if (defaultBlueBottomColor != Color.Empty)
                {
                    return defaultBlueBottomColor;
                }
                else
                {
                    defaultBlueBottomColor = Color.FromArgb(214, 208, 197);
                    return defaultBlueBottomColor;
                }
            }
        }

        /// <summary></summary>
        public static Color DefaultBlueGradientStartColor
        {
            get
            {
                if (defaultBlueGradientStartColor != Color.Empty)
                {
                    return defaultBlueGradientStartColor;
                }
                else
                {
                    defaultBlueGradientStartColor = Color.FromArgb(244, 244, 240);
                    return defaultBlueGradientStartColor;
                }
            }
        }

        /// <summary></summary>
        public static Color DefaultBlueGradientEndColor
        {
            get
            {
                if (defaultBlueGradientEndColor != Color.Empty)
                {
                    return defaultBlueGradientEndColor;
                }
                else
                {
                    defaultBlueGradientEndColor = Color.FromArgb(214, 208, 197);
                    return defaultBlueGradientEndColor;
                }
            }
        }

        /// <summary></summary>
        public static Color DefaultBlueMouseOverColor
        {
            get
            {
                if (defaultBlueMouseOverColor != Color.Empty)
                {
                    return defaultBlueMouseOverColor;
                }
                else
                {
                    defaultBlueMouseOverColor = Color.Orange;
                    return defaultBlueMouseOverColor;
                }
            }
        }

        /// <summary></summary>
        public static Color DefaultBlueBorderColor
        {
            get
            {
                if (defaultBlueBorderColor != Color.Empty)
                {
                    return defaultBlueBorderColor;
                }
                else
                {
                    defaultBlueBorderColor = Color.FromArgb(0, 60, 116);
                    return defaultBlueBorderColor;
                }
            }
        }

        /// <summary></summary>
        public static Color DefaultBlueComboButtonColor
        {
            get
            {
                if (defaultBlueComboButtonColor != Color.Empty)
                {
                    return defaultBlueComboButtonColor;
                }
                else
                {
                    defaultBlueComboButtonColor = Color.FromArgb(184, 203, 246);
                    return defaultBlueComboButtonColor;
                }
            }
        }

        /// <summary></summary>
        public static Color DefaultBlueComboButtonMouseOverColor
        {
            get
            {
                if (defaultBlueComboButtonMouseOverColor != Color.Empty)
                {
                    return defaultBlueComboButtonMouseOverColor;
                }
                else
                {
                    defaultBlueComboButtonMouseOverColor = Color.FromArgb(216, 237, 252);
                    return defaultBlueComboButtonMouseOverColor;
                }
            }
        }

        /// <summary></summary>
        public static Color DefaultBlueComboButtonPressedColor
        {
            get
            {
                if (defaultBlueComboButtonPressedColor != Color.Empty)
                {
                    return defaultBlueComboButtonPressedColor;
                }
                else
                {
                    defaultBlueComboButtonPressedColor = Color.FromArgb(131, 167, 240);
                    return defaultBlueComboButtonPressedColor;
                }
            }
        }

        /// <summary></summary>
        public static Color DefaultBlueComboArcColor
        {
            get
            {
                if (defaultBlueComboArcColor != Color.Empty)
                {
                    return defaultBlueComboArcColor;
                }
                else
                {
                    defaultBlueComboArcColor = Color.FromArgb(230, 238, 252);
                    return defaultBlueComboArcColor;
                }
            }
        }

        /// <summary></summary>
        public static Color DefaultBlueComboBorderColor
        {
            get
            {
                if (defaultBlueComboBorderColor != Color.Empty)
                {
                    return defaultBlueComboBorderColor;
                }
                else
                {
                    defaultBlueComboBorderColor = Color.FromArgb(127, 157, 185);
                    return defaultBlueComboBorderColor;
                }
            }
        }

        /// <summary></summary>
        public static Color DefaultBlueComboButtonBorderColor
        {
            get
            {
                if (defaultBlueComboButtonBorderColor != Color.Empty)
                {
                    return defaultBlueComboButtonBorderColor;
                }
                else
                {
                    defaultBlueComboButtonBorderColor = Color.FromArgb(183, 202, 245);
                    return defaultBlueComboButtonBorderColor;
                }
            }
        }

        /// <summary></summary>
        public static Color OliveGreenTopColor
        {
            get
            {
                if (oliveGreenTopColor != Color.Empty)
                {
                    return oliveGreenTopColor;
                }
                else
                {
                    oliveGreenTopColor = Color.FromArgb(254, 253, 239);
                    return oliveGreenTopColor;
                }
            }
        }

        /// <summary></summary>
        public static Color OliveGreenBottomColor
        {
            get
            {
                if (oliveGreenBottomColor != Color.Empty)
                {
                    return oliveGreenBottomColor;
                }
                else
                {
                    oliveGreenBottomColor = Color.FromArgb(236, 233, 216);
                    return oliveGreenBottomColor;
                }
            }
        }

        /// <summary></summary>
        public static Color OliveGreenGradientStartColor
        {
            get
            {
                if (oliveGreenGradientStartColor != Color.Empty)
                {
                    return oliveGreenGradientStartColor;
                }
                else
                {
                    oliveGreenGradientStartColor = Color.FromArgb(254, 253, 239);
                    return oliveGreenGradientStartColor;
                }
            }
        }

        /// <summary></summary>
        public static Color OliveGreenGradientEndColor
        {
            get
            {
                if (oliveGreenGradientEndColor != Color.Empty)
                {
                    return oliveGreenGradientEndColor;
                }
                else
                {
                    oliveGreenGradientEndColor = Color.FromArgb(236, 233, 216);
                    return oliveGreenGradientEndColor;
                }
            }
        }

        /// <summary></summary>
        public static Color OliveGreenMouseOverColor
        {
            get
            {
                if (oliveGreenMouseOverColor != Color.Empty)
                {
                    return oliveGreenMouseOverColor;
                }
                else
                {
                    oliveGreenMouseOverColor = Color.Orange;
                    return oliveGreenMouseOverColor;
                }
            }
        }

        /// <summary></summary>
        public static Color OliveGreenBorderColor
        {
            get
            {
                if (oliveGreenBorderColor != Color.Empty)
                {
                    return oliveGreenBorderColor;
                }
                else
                {
                    oliveGreenBorderColor = Color.FromArgb(55, 98, 6);
                    return oliveGreenBorderColor;
                }
            }
        }

        /// <summary></summary>
        public static Color OliveGreenComboButtonColor
        {
            get
            {
                if (oliveGreenComboButtonColor != Color.Empty)
                {
                    return oliveGreenComboButtonColor;
                }
                else
                {
                    oliveGreenComboButtonColor = Color.FromArgb(155, 173, 124);
                    return oliveGreenComboButtonColor;
                }
            }
        }

        /// <summary></summary>
        public static Color OliveGreenComboButtonMouseOverColor
        {
            get
            {
                if (oliveGreenComboButtonMouseOverColor != Color.Empty)
                {
                    return oliveGreenComboButtonMouseOverColor;
                }
                else
                {
                    oliveGreenComboButtonMouseOverColor = Color.FromArgb(198, 211, 155);
                    return oliveGreenComboButtonMouseOverColor;
                }
            }
        }

        /// <summary></summary>
        public static Color OliveGreenComboButtonPressedColor
        {
            get
            {
                if (oliveGreenComboButtonPressedColor != Color.Empty)
                {
                    return oliveGreenComboButtonPressedColor;
                }
                else
                {
                    oliveGreenComboButtonPressedColor = Color.FromArgb(155, 173, 130);
                    return oliveGreenComboButtonPressedColor;
                }
            }
        }

        /// <summary></summary>
        public static Color OliveGreenComboArcColor
        {
            get
            {
                if (oliveGreenComboArcColor != Color.Empty)
                {
                    return oliveGreenComboArcColor;
                }
                else
                {
                    oliveGreenComboArcColor = Color.FromArgb(195, 200, 184);
                    return oliveGreenComboArcColor;
                }
            }
        }

        /// <summary></summary>
        public static Color OliveGreenComboBorderColor
        {
            get
            {
                if (oliveGreenComboBorderColor != Color.Empty)
                {
                    return oliveGreenComboBorderColor;
                }
                else
                {
                    oliveGreenComboBorderColor = Color.FromArgb(164, 185, 127);
                    return oliveGreenComboBorderColor;
                }
            }
        }

        /// <summary></summary>
        public static Color OliveGreenComboButtonBorderColor
        {
            get
            {
                if (oliveGreenComboButtonBorderColor != Color.Empty)
                {
                    return oliveGreenComboButtonBorderColor;
                }
                else
                {
                    oliveGreenComboButtonBorderColor = Color.FromArgb(157, 171, 119);
                    return oliveGreenComboButtonBorderColor;
                }
            }
        }

        /// <summary></summary>
        public static Color SilverTopColor
        {
            get
            {
                if (silverTopColor != Color.Empty)
                {
                    return silverTopColor;
                }
                else
                {
                    silverTopColor = Color.FromArgb(253, 253, 253);
                    return silverTopColor;
                }
            }
        }

        /// <summary></summary>
        public static Color SilverBottomColor
        {
            get
            {
                if (silverBottomColor != Color.Empty)
                {
                    return silverBottomColor;
                }
                else
                {
                    silverBottomColor = Color.FromArgb(202, 201, 221);
                    return silverBottomColor;
                }
            }
        }

        /// <summary></summary>
        public static Color SilverGradientStartColor
        {
            get
            {
                if (silverGradientStartColor != Color.Empty)
                {
                    return silverGradientStartColor;
                }
                else
                {
                    silverGradientStartColor = Color.FromArgb(253, 253, 253);
                    return silverGradientStartColor;
                }
            }
        }

        /// <summary></summary>
        public static Color SilverGradientEndColor
        {
            get
            {
                if (silverGradientEndColor != Color.Empty)
                {
                    return silverGradientEndColor;
                }
                else
                {
                    silverGradientEndColor = Color.FromArgb(202, 201, 221);
                    return silverGradientEndColor;
                }
            }
        }

        /// <summary></summary>
        public static Color SilverMouseOverColor
        {
            get
            {
                if (silverMouseOverColor != Color.Empty)
                {
                    return silverMouseOverColor;
                }
                else
                {
                    silverMouseOverColor = Color.Orange;
                    return silverMouseOverColor;
                }
            }
        }

        /// <summary></summary>
        public static Color SilverBorderColor
        {
            get
            {
                if (silverBorderColor != Color.Empty)
                {
                    return silverBorderColor;
                }
                else
                {
                    silverBorderColor = Color.FromArgb(0, 60, 116);
                    return silverBorderColor;
                }
            }
        }

        /// <summary></summary>
        public static Color SilverComboBorderColor
        {
            get
            {
                if (silverComboBorderColor != Color.Empty)
                {
                    return silverComboBorderColor;
                }
                else
                {
                    silverComboBorderColor = Color.FromArgb(165, 172, 178);
                    return silverComboBorderColor;
                }
            }
        }
        #endregion
    }

	/// <summary>
	/// Defines the different colors that are used to define the Office 2003 look and feel.
	/// </summary>
	[ DocumentationExclude() ]
	public class ButtonOffice2003Colors
	{
		#region Class members
		/// <summary></summary>
		private static Color defaultColor = Color.Empty;
		/// <summary></summary>
		private static Color mouseOverColor = Color.Empty;
		/// <summary></summary>
		private static Color pressedColor = Color.Empty;
		/// <summary></summary>
		private static Color borderColor = Color.Empty;
		/// <summary></summary>
		private static Color focusBorderColor = Color.Empty;
		#endregion

		#region Class properties
		/// <summary></summary>
		public static Color DefaultColor
		{
			get
			{
				if( defaultColor != Color.Empty )
				{
					return defaultColor;
				}
				else
				{
					defaultColor = Color.FromArgb( 219, 218, 228 );
					return defaultColor;
				}
			}
		}

		/// <summary></summary>
		public static Color MouseOverColor
		{
			get
			{
				if( mouseOverColor != Color.Empty )
				{
					return mouseOverColor;
				}
				else
				{
					mouseOverColor = Color.FromArgb( 255, 238, 194 );
					return mouseOverColor;
				}
			}
		}

		/// <summary></summary>
		public static Color PressedColor
		{
			get
			{
				if( pressedColor != Color.Empty )
				{
					return pressedColor;
				}
				else
				{
					pressedColor = Color.FromArgb( 254, 128, 62 );
					return pressedColor;
				}
			}
		}

		/// <summary></summary>
		public static Color BorderColor
		{
			get
			{
				if( borderColor != Color.Empty )
				{
					return borderColor;
				}
				else
				{
					borderColor = SystemColors.ControlDark;
					return borderColor;
				}
			}
		}

		/// <summary></summary>
		public static Color FocusBorderColor
		{
			get
			{
				if( focusBorderColor != Color.Empty )
				{
					return focusBorderColor;
				}
				else
				{
					focusBorderColor = Color.FromArgb( 0, 0, 128 );
					return focusBorderColor;
				}
			}
		}
		#endregion
	}

	/// <summary>
	/// Defines the different colors that are used to define the Office XP look and feel.
	/// </summary>
	[ DocumentationExclude() ]
	public class ButtonOfficeXPColors
	{
		#region Class members
		/// <summary></summary>
		private static Color borderColor = Color.Empty;
		#endregion

		#region Class properties
		/// <summary></summary>
		public static Color BorderColor
		{
			get
			{
				if( borderColor != Color.Empty )
				{
					return borderColor;
				}
				else
				{
					borderColor = Color.FromArgb( 178, 180, 191 );
					return borderColor;
				}
			}
		}
		#endregion
	}
}