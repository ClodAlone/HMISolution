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
using System.Drawing;

using Syncfusion.ComponentModel;
using Syncfusion.Documentation;
#endregion

namespace Syncfusion.Windows.Forms
{

    /// <summary>
    /// Defines the different colors that will be used to define the 3 Windows XP
    /// color schemes supported.
    /// </summary>
    [DocumentationExclude()]
    public class WindowsVistaColors
    {
        #region Class members
        /// <summary></summary>
        private static Color buttonPressedTopColor = Color.Empty;
        /// <summary></summary>
        private static Color buttonPressedBottomColor = Color.Empty;
        /// <summary></summary>
        private static Color buttonSelectedTopColor = Color.Empty;
        /// <summary></summary>
        private static Color buttonSelectedBottomColor = Color.Empty;
        /// <summary></summary>
        private static Color buttonDisabledTopColor = Color.Empty;
        /// <summary></summary>
        private static Color buttonDisabledBottomColor = Color.Empty;
        /// <summary></summary>
        private static Color buttonPressedBorderColor = Color.Empty;
        /// <summary></summary>
        private static Color buttonSelectedBorderColor = Color.Empty;
        /// <summary></summary>
        private static Color buttonDisabledBorderColor = Color.Empty;


        /// <summary></summary>
        private static Color blueButtonDefaultTopColor = Color.Empty;
        /// <summary></summary>
        private static Color blueButtonDefaultBottomColor = Color.Empty;
        /// <summary></summary>
        private static Color blueButtonDefaultBorderColor = Color.Empty;
        /// <summary></summary>
        private static Color blueButtonDefaultInternalBorderColor = Color.Empty;
        /// <summary></summary>
        private static Color blueButtonPressedInternalBorderColor = Color.Empty;
        /// <summary></summary>
        private static Color blueButtonSelectedInternalBorderColor = Color.Empty;

        /// <summary></summary>
        private static Color silverButtonDefaultTopColor = Color.Empty;
        /// <summary></summary>
        private static Color silverButtonDefaultBottomColor = Color.Empty;
        /// <summary></summary>
        private static Color silverButtonDefaultBorderColor = Color.Empty;
        /// <summary></summary>
        private static Color silverButtonDefaultInternalBorderColor = Color.Empty;
        /// <summary></summary>
        private static Color silverButtonPressedInternalBorderColor = Color.Empty;
        /// <summary></summary>
        private static Color silverButtonSelectedInternalBorderColor = Color.Empty;

        /// <summary></summary>
        private static Color blackButtonDefaultTopColor = Color.Empty;
        /// <summary></summary>
        private static Color blackButtonDefaultBottomColor = Color.Empty;
        /// <summary></summary>
        private static Color blackButtonDefaultBorderColor = Color.Empty;
        /// <summary></summary>
        private static Color blackButtonDefaultInternalBorderColor = Color.Empty;
        /// <summary></summary>
        private static Color blackButtonPressedInternalBorderColor = Color.Empty;
        /// <summary></summary>
        private static Color blackButtonSelectedInternalBorderColor = Color.Empty;

        #endregion

        #region Class properties
        /// <summary></summary>
        public static Color ButtonPressedTopColor
        {
            get
            {
                if (buttonPressedTopColor != Color.Empty)
                {
                    return buttonPressedTopColor;
                }
                else
                {
                    buttonPressedTopColor = Color.FromArgb(255, 197, 108);
                    return buttonPressedTopColor;
                }
            }
        }
        /// <summary></summary>
        public static Color ButtonPressedBottomColor
        {
            get
            {
                if (buttonPressedBottomColor != Color.Empty)
                {
                    return buttonPressedBottomColor;
                }
                else
                {
                    buttonPressedBottomColor = Color.FromArgb(251, 138, 59);
                    return buttonPressedBottomColor;
                }
            }
        }
        /// <summary></summary>
        public static Color ButtonSelectedTopColor
        {
            get
            {
                if (buttonSelectedTopColor != Color.Empty)
                {
                    return buttonSelectedTopColor;
                }
                else
                {
                    buttonSelectedTopColor = Color.FromArgb(255, 252, 217);
                    return buttonSelectedTopColor;
                }
            }
        }
        /// <summary></summary>
        public static Color ButtonSelectedBottomColor
        {
            get
            {
                if (buttonSelectedBottomColor != Color.Empty)
                {
                    return buttonSelectedBottomColor;
                }
                else
                {
                    buttonSelectedBottomColor = Color.FromArgb(255, 214, 70);
                    return buttonSelectedBottomColor;
                }
            }
        }
        /// <summary></summary>
        public static Color ButtonDisabledTopColor
        {
            get
            {
                if (buttonDisabledTopColor != Color.Empty)
                {
                    return buttonDisabledTopColor;
                }
                else
                {
                    buttonDisabledTopColor = Color.FromArgb(244, 244, 244);
                    return buttonDisabledTopColor;
                }
            }
        }
        /// <summary></summary>
        public static Color ButtonDisabledBottomColor
        {
            get
            {
                if (buttonDisabledBottomColor != Color.Empty)
                {
                    return buttonDisabledBottomColor;
                }
                else
                {
                    buttonDisabledBottomColor = Color.FromArgb(201, 201, 201);
                    return buttonDisabledBottomColor;
                }
            }
        }
        /// <summary></summary>
        public static Color ButtonPressedBorderColor
        {
            get
            {
                if (buttonPressedBorderColor != Color.Empty)
                {
                    return buttonPressedBorderColor;
                }
                else
                {
                    buttonPressedBorderColor = Color.FromArgb(139, 118, 84);
                    return buttonPressedBorderColor;
                }
            }
        }
        /// <summary></summary>
        public static Color ButtonSelectedBorderColor
        {
            get
            {
                if (buttonSelectedBorderColor != Color.Empty)
                {
                    return buttonSelectedBorderColor;
                }
                else
                {
                    buttonSelectedBorderColor = Color.FromArgb(185, 160, 116);
                    return buttonSelectedBorderColor;
                }
            }
        }
        /// <summary></summary>
        public static Color ButtonDisabledBorderColor
        {
            get
            {
                if (buttonDisabledBorderColor != Color.Empty)
                {
                    return buttonDisabledBorderColor;
                }
                else
                {
                    buttonDisabledBorderColor = Color.FromArgb(156, 164, 173);
                    return buttonDisabledBorderColor;
                }
            }
        }
        /// <summary></summary>
        public static Color BlueButtonDefaultTopColor
        {
            get
            {
                if ( blueButtonDefaultTopColor != Color.Empty )
                {
                    return blueButtonDefaultTopColor;
                }
                else
                {
                    blueButtonDefaultTopColor = Color.FromArgb( 231, 242, 255 );
                    return blueButtonDefaultTopColor;
                }
            }
        }
        /// <summary></summary>
        public static Color BlueButtonDefaultBottomColor
        {
            get
            {
                if (blueButtonDefaultBottomColor != Color.Empty)
                {
                    return blueButtonDefaultBottomColor;
                }
                else
                {
                    blueButtonDefaultBottomColor = Color.FromArgb( 179, 209, 252 );
                    return blueButtonDefaultBottomColor;
                }
            }
        }
        /// <summary></summary>
        public static Color BlueButtonDefaultBorderColor
        {
            get
            {
                if ( blueButtonDefaultBorderColor != Color.Empty )
                {
                    return blueButtonDefaultBorderColor;
                }
                else
                {
                    blueButtonDefaultBorderColor = Color.FromArgb( 176, 208, 255 );
                    return blueButtonDefaultBorderColor;
                }
            }
        }
        /// <summary></summary>
        public static Color BlueButtonDefaultInternalBorderColor
        {
            get
            {
                if ( blueButtonDefaultInternalBorderColor != Color.Empty )
                {
                    return blueButtonDefaultInternalBorderColor;
                }
                else
                {
                    blueButtonDefaultInternalBorderColor = Color.FromArgb( 128, Color.FromArgb( 250, 251, 255 ) );
                    return blueButtonDefaultInternalBorderColor;
                }
            }
        }
        /// <summary></summary>
        public static Color BlueButtonPressedInternalBorderColor
        {
            get
            {
                if (blueButtonPressedInternalBorderColor != Color.Empty)
                {
                    return blueButtonPressedInternalBorderColor;
                }
                else
                {
                    blueButtonPressedInternalBorderColor = Color.FromArgb( 70, Color.FromArgb( 176, 132, 92 ) );
                    return blueButtonPressedInternalBorderColor;
                }
            }
        }
        /// <summary></summary>
        public static Color BlueButtonSelectedInternalBorderColor
        {
            get
            {
                if ( blueButtonSelectedInternalBorderColor != Color.Empty )
                {
                    return blueButtonSelectedInternalBorderColor;
                }
                else
                {
                    blueButtonSelectedInternalBorderColor = Color.FromArgb(128, Color.FromArgb(250, 251, 255));
                    return blueButtonSelectedInternalBorderColor;
                }
            }
        }
        /// <summary></summary>
        public static Color SilverButtonDefaultTopColor
        {
            get
            {
                if ( silverButtonDefaultTopColor != Color.Empty )
                {
                    return silverButtonDefaultTopColor;
                }
                else
                {
                    silverButtonDefaultTopColor = Color.FromArgb(223, 227, 231);
                    return silverButtonDefaultTopColor;
                }
            }
        }
        /// <summary></summary>
        public static Color SilverButtonDefaultBottomColor
        {
            get
            {
                if ( silverButtonDefaultBottomColor != Color.Empty )
                {
                    return silverButtonDefaultBottomColor;
                }
                else
                {
                    silverButtonDefaultBottomColor = Color.FromArgb(208, 212, 221);                  
                    return silverButtonDefaultBottomColor;
                }
            }
        }
        /// <summary></summary>
        public static Color SilverButtonDefaultBorderColor
        {
            get
            {
                if ( silverButtonDefaultBorderColor != Color.Empty )
                {
                    return silverButtonDefaultBorderColor;
                }
                else
                {
                    silverButtonDefaultBorderColor = Color.FromArgb(208, 212, 221);
                    return silverButtonDefaultBorderColor;
                }
            }
        }
        /// <summary></summary>
        public static Color SilverButtonDefaultInternalBorderColor
        {
            get
            {
                if ( silverButtonDefaultInternalBorderColor != Color.Empty )
                {
                    return silverButtonDefaultInternalBorderColor;
                }
                else
                {
                    silverButtonDefaultInternalBorderColor = Color.FromArgb(128, Color.FromArgb(250, 251, 255));
                    return silverButtonDefaultInternalBorderColor;
                }
            }
        }
        /// <summary></summary>
        public static Color SilverButtonPressedInternalBorderColor
        {
            get
            {
                if ( silverButtonPressedInternalBorderColor != Color.Empty )
                {
                    return silverButtonPressedInternalBorderColor;
                }
                else
                {
                    silverButtonPressedInternalBorderColor = Color.FromArgb(70, Color.FromArgb(176, 132, 92));
                    return silverButtonPressedInternalBorderColor;
                }
            }
        }
        /// <summary></summary>
        public static Color SilverButtonSelectedInternalBorderColor
        {
            get
            {
                if ( silverButtonSelectedInternalBorderColor != Color.Empty )
                {
                    return silverButtonSelectedInternalBorderColor;
                }
                else
                {
                    silverButtonSelectedInternalBorderColor = Color.FromArgb(128, Color.FromArgb(250, 251, 255));
                    return silverButtonSelectedInternalBorderColor;
                }
            }
        }
        /// <summary></summary>
        public static Color BlackButtonDefaultTopColor
        {
            get
            {
                if ( blackButtonDefaultTopColor != Color.Empty )
                {
                    return blackButtonDefaultTopColor;
                }
                else
                {
                    blackButtonDefaultTopColor = Color.FromArgb(146, 146, 146);
                    return blackButtonDefaultTopColor;
                }
            }
        }
        /// <summary></summary>
        public static Color BlackButtonDefaultBottomColor
        {
            get
            {
                if ( blackButtonDefaultBottomColor != Color.Empty )
                {
                    return blackButtonDefaultBottomColor;
                }
                else
                {
                    blackButtonDefaultBottomColor = Color.FromArgb(83, 83, 83);
                    return blackButtonDefaultBottomColor;
                }
            }
        }
        /// <summary></summary>
        public static Color BlackButtonDefaultBorderColor
        {
            get
            {
                if ( blackButtonDefaultBorderColor != Color.Empty )
                {
                    return blackButtonDefaultBorderColor;
                }
                else
                {
                    blackButtonDefaultBorderColor = Color.FromArgb(153, 153, 153);
                    return blackButtonDefaultBorderColor;
                }
            }
        }
        /// <summary></summary>
        public static Color BlackButtonDefaultInternalBorderColor
        {
            get
            {
                if ( blackButtonDefaultInternalBorderColor != Color.Empty )
                {
                    return blackButtonDefaultInternalBorderColor;
                }
                else
                {
                    blackButtonDefaultInternalBorderColor = Color.FromArgb(128, Color.FromArgb(250, 251, 255));
                    return blackButtonDefaultInternalBorderColor;
                }
            }
        }
        /// <summary></summary>
        public static Color BlackButtonPressedInternalBorderColor
        {
            get
            {
                if ( blackButtonPressedInternalBorderColor != Color.Empty )
                {
                    return blackButtonPressedInternalBorderColor;
                }
                else
                {
                    blackButtonPressedInternalBorderColor = Color.FromArgb(70, Color.FromArgb(176, 132, 92));
                    return blackButtonPressedInternalBorderColor;
                }
            }
        }
        /// <summary></summary>
        public static Color BlackButtonSelectedInternalBorderColor
        {
            get
            {
                if ( blackButtonSelectedInternalBorderColor != Color.Empty )
                {
                    return blackButtonSelectedInternalBorderColor;
                }
                else
                {
                    blackButtonSelectedInternalBorderColor = Color.FromArgb(128, Color.FromArgb(250, 251, 255));
                    return blackButtonSelectedInternalBorderColor;
                }
            }
        }


        #endregion
    }
	
}