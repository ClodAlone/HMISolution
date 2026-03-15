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
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Provides static properties to access the colors used by different components in the XPMenus framework.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The primary objective of this class is to let you specify custom colors for the different
	/// regions of the XPMenus components. When no colors are provided, this class returns the default
	/// colors usually synthesized from the System Colors. If you have to reset a property that you set before,
	/// simply set it to be Color.Empty; the next time the property is queried, it will return the default color.
	/// </para>
	/// <para>
	/// If you are a consumer of the colors in this class, then note that the colors returned
	/// by properties in this class need to be reinitialized when the system color changes.
	/// This class doesn't listen to the system color change event, instead it expects the consumer to notify it
	/// when the system color changes. To avoid redundant updates by multiple consumers, a unique notification pattern 
	/// is recommended.
	/// </para>
	/// <para>
	/// To notify system color changes, you should listen to the <see cref="System.Windows.Forms.Control.SystemColorsChanged"/>
	/// event and call the <see cref="SysColorsChanged"/> method. The SysColorsChanged method will let you specify whether to
	/// update the colors immediately or later with a call to <see cref="UpdateMenuColors"/>. You should
	/// choose to update immediately if you will use the new colors to update certain properties in your control (Backcolor, for example).
	/// On the other hand, if you use the colors within and only within your Paint event, then update the colors later in
	/// the beginning of your Paint method with a call to <b>UpdateMenuColors</b>. Note that the 
	/// <b>UpdateMenuColors</b> method will actually update the colors only if necessary, letting you call it multiple times
	/// without performance hit.
	/// </para>
	/// <para>
	/// There is also a <see cref="MenuColorsChanged"/> event that gets fired whenever colors
	/// are updated, either due to change in System Colors (notified using the above pattern) or when a custom color is set on any of the properties.
	/// </para>
	/// </remarks>
	public class MenuColors
	{
		static Color selColor = Color.Empty;
		static Color selTextColor = Color.Empty;
		static Color selBorderColor = Color.Empty;
		static Color bgColor = Color.Empty;
		static Color imageColColor = Color.Empty;
		static Color commandBarBackColor = Color.Empty;

		static Color customSelColor = Color.Empty;
		static Color customSelTextColor = Color.Empty;
		static Color customCheckedColor = Color.Empty;
		static Color customPressedSelColor = Color.Empty;
		static Color customSelBorderColor = Color.Empty;
		static Color customBGColor = Color.Empty;
		static Color customImageColColor = Color.Empty;
		static Color customDropDownBorderColor = Color.Empty;
		static Color customCommandBarBackColor = Color.Empty;
		static Color customMainMenuBackColor = Color.Empty;
		static Color customStatusBarBackColor = Color.Empty;
		static Color customFloatingCommandBarCaptionColor = Color.Empty;
		static Color customExpandedMenuStripBackColor = Color.Empty;
		static int customInactiveItemAlphaBlendFactor = -1;
		static Color customMenuTextColor = Color.Empty;
		static Color customActiveMenuTextColor = Color.Empty;
		static Color customDisabledMenuTextColorBase = Color.Empty;
		static Color customDisabledToolbarItemTextColorBase = Color.Empty;

		static bool needToUpdateColors = true;

		/// <summary>
		/// Initializes default colors based on SystemColors.
		/// </summary>
		/// <remarks>
		/// Calling this will not affect the custom colors set using the properties.
		/// </remarks>
		public static void UpdateMenuColors()
		{
			if(!needToUpdateColors)
				return;

			needToUpdateColors = false;

			bgColor = CalculateColor(SystemColors.Window,
				SystemColors.Control, 220);
			selColor = CalculateColor(SystemColors.Highlight, 
				SystemColors.Window, 70);
			imageColColor = CalculateColor(SystemColors.Control,
				bgColor, 195);
			selBorderColor = Color.FromArgb(255, SystemColors.Highlight);
			selTextColor = Color.Black;

			Color clr = SystemColors.Control;
			commandBarBackColor = ControlPaint.Light(Color.FromArgb(clr.A,clr.R,clr.G,clr.B), 0.3f);

			OnMenuColorsChanged(EventArgs.Empty);
		}

		static void OnMenuColorsChanged(EventArgs e)
		{
			if(MenuColorsChanged != null)
			{
				MenuColorsChanged(null, e);
			}
		}

		/// <summary>
		/// Fired when the colors have changed either because of change in system colors or
		/// when a custom color is specified using one of the properties.
		/// </summary>
		/// <remarks>
		/// Take a look at the class reference for this class for information on how to notify
		/// this class regarding system color changes.
		/// </remarks>
		public static event EventHandler MenuColorsChanged;

		/// <summary>
		/// Initializes the default colors.
		/// </summary>
		static MenuColors()
		{
			UpdateMenuColors();
		}

		/// <summary>
		/// Call this method to indicate that the system colors have changed.
		/// </summary>
		/// <param name="updateColorsNow">Indicates whether to update colors immediately or later with a call to <see cref="UpdateMenuColors"/>.</param>
		/// <remarks>
		/// Take a look at the MenuColors class reference for the recommended system color change notification pattern.
		/// </remarks>
		public static void SysColorsChanged(bool updateColorsNow)
		{
			if(updateColorsNow)
				UpdateMenuColors();
			else
				needToUpdateColors = true;
		}

		internal static Color CalculateColor(Color front, Color back, decimal alpha)
		{
			// Getting an opaque color off an alpha blended color. So that we can
			// draw over it.

			Color frontColor = Color.FromArgb(255, front);
			Color backColor = Color.FromArgb(255, back);
									
			decimal alphaPercent = alpha/255;
			decimal alphaPercent2 = 1 - alphaPercent;

			decimal frontRed = frontColor.R;
			decimal frontGreen = frontColor.G;
			decimal frontBlue = frontColor.B;
			decimal backRed = backColor.R;
			decimal backGreen = backColor.G;
			decimal backBlue = backColor.B;
			
			byte fRed = (byte)(frontRed*alphaPercent + backRed*alphaPercent2);
			byte fGreen = (byte)(frontGreen*alphaPercent + backGreen*alphaPercent2);
			byte fBlue = (byte)(frontBlue*alphaPercent + backBlue*alphaPercent2);

			return  Color.FromArgb(255, fRed, fGreen, fBlue);
		}

		/// <summary>
		/// Gets / sets the selected color for a menu item in a toolbar.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color SelColor
		{
			get
			{
				if(customSelColor == Color.Empty)
					return selColor;
				else
					return customSelColor;
			}
			set
			{
				if(customSelColor != value)
				{
					customSelColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets / sets the selected text color for an item in a toolbar.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color SelTextColor
		{
			get
			{
				if( customSelTextColor == Color.Empty )
					return selTextColor;
				else
					return customSelTextColor;
			}
			set
			{
				if( customSelTextColor != value )
				{
					customSelTextColor = value;
					OnMenuColorsChanged( EventArgs.Empty );
				}
			}
		}

		/// <summary>
		/// Gets / sets the background color of a drop-down menu.
		/// </summary>
		/// <seealso cref="MenuLeftStripColor"/>
		/// <value>The default value is derived from a System Color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color MenuBGColor
		{
			get
			{
				if(customBGColor == Color.Empty)
					return bgColor;
				else
					return customBGColor;
			}
			set
			{
				if(customBGColor != value)
				{
					customBGColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets / sets the color for the left aligned strip in a drop-down menu where images and check boxes are shown.
		/// </summary>
		/// <seealso cref="MenuBGColor"/>
		/// <value>The default value is derived from a System Color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color MenuLeftStripColor
		{
			get
			{
				if(customImageColColor == Color.Empty)
					return imageColColor;
				else
					return customImageColColor;
			}
			set
			{
				if(customImageColColor != value)
				{
					customImageColColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets / sets the selected-pressed color for a menu item in a toolbar.
		/// </summary>
		/// <value>The default value is derived from a System Color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color PressedSelColor
		{
			get
			{
				if(customPressedSelColor == Color.Empty)
				{
					if(SystemColors.Window.R == 255
						&& SystemColors.Window.G == 255
						&& SystemColors.Window.B == 255)
						return Color.FromArgb(132, 146, 181);
					else
						return ControlPaint.Dark(
							ControlPaint.LightLight(ControlPaint.Dark(SystemColors.Window)));
				}
				else
					return customPressedSelColor;
			}
			set
			{
				if(customPressedSelColor != value)
				{
					customPressedSelColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets / sets the selected color for a checked menu item in a toolbar.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color CheckedSelColor
		{
			get
			{
				if(customCheckedColor == Color.Empty)
					return SelColor; // This has to be slightly darker than SelColor.
				else
					return customCheckedColor;
			}
			set
			{
				if(customCheckedColor != value)
				{
					customCheckedColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets / sets the border color for a selected menu item in a toolbar.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color SelBorderColor
		{
			get
			{
				if(customSelBorderColor == Color.Empty)
					return selBorderColor;
				else
					return customSelBorderColor;
			}
			set
			{
				if(customSelBorderColor != value)
				{
					customSelBorderColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets / sets the border color for a drop-down menu.
		/// </summary>
		/// <seealso cref="MenuBGColor"/>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color DropDownBorderColor
		{
			get
			{
				if(customDropDownBorderColor == Color.Empty)
					return SystemColors.ControlDarkDark;
				else
					return customDropDownBorderColor;
			}
			set
			{
				if(customDropDownBorderColor != value)
				{
					customDropDownBorderColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets / sets the background color for a toolbar / commandbar.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color CommandBarBackColor
		{
			get
			{
				if(customCommandBarBackColor == Color.Empty)
					return commandBarBackColor;
				else
					return customCommandBarBackColor;
			}
			set
			{
				if(customCommandBarBackColor != value)
				{
					customCommandBarBackColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets / sets the background color for the main-menu bar.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color MainMenuBackColor
		{
			get
			{
				if(customMainMenuBackColor == Color.Empty)
					return SystemColors.Control;
				else
					return customMainMenuBackColor;
			}
			set
			{
				if(customMainMenuBackColor != value)
				{
					customMainMenuBackColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets / sets the background color for the Status Bar.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color StatusBarBackColor
		{
			get
			{
				if(customStatusBarBackColor == Color.Empty)
					return SystemColors.Control;
				else
					return customStatusBarBackColor;
			}
			set
			{
				if(customStatusBarBackColor != value)
				{
					customStatusBarBackColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets / sets the Caption background color for a floating toolbar / commandbar.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color FloatingCommandBarCaptionColor
		{
			get
			{
				if(customFloatingCommandBarCaptionColor == Color.Empty)
					return SystemColors.ControlDark;
				else
					return customFloatingCommandBarCaptionColor;
			}
			set
			{
				if(customFloatingCommandBarCaptionColor != value)
				{
					customFloatingCommandBarCaptionColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets / sets the Backcolor for the expanded,  
		/// left-aligned menu strip region. This is the region you see when a partial menu
		/// gets expanded to show all the menu items.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color ExpandedMenuStripBackColor
		{
			get
			{
				if(customExpandedMenuStripBackColor == Color.Empty)
					return ControlPaint.LightLight(SystemColors.ControlDark);
				else
					return customExpandedMenuStripBackColor;
			}
			set
			{
				if(customExpandedMenuStripBackColor != value)
				{
					customExpandedMenuStripBackColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets / sets the alpha-blend factor to use to shade the inactive menu item's icons. (255 for no alpha-blending; 0 will completely hide the item)
		/// </summary>
		/// <value>A value in the range 1 to 255; -1 will reset to default value. Default is 175.</value>
		/// <remarks>
		/// 255 will draw the icon without any alpha blending; 1 will almost hide the icons. This setting will be ignored when XPThemes is turned on.
		/// </remarks>
		public static int InactiveItemAlphaBlendFactor
		{
			get
			{
				if(customInactiveItemAlphaBlendFactor == -1)
					return 175;
				else
					return customInactiveItemAlphaBlendFactor;
			}
			set
			{
				if(customInactiveItemAlphaBlendFactor != value
					&& ((value > 0 && value <= 255) || value == -1))
				{
					customInactiveItemAlphaBlendFactor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets / sets the text color base for the text in the disabled menu items.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// When you specify a custom color, ControlPaint.LightLight will be applied on it before using that color.
		/// </remarks>
		public static Color DisabledMenuTextColorBase
		{
			get
			{
				if(customDisabledMenuTextColorBase == Color.Empty)
					// VS.Net draws a LightLight version of this color when disabled.
					return SystemColors.ControlDark;
				else
					return customDisabledMenuTextColorBase;
			}
			set
			{
				if(customDisabledMenuTextColorBase != value)
				{
					customDisabledMenuTextColorBase = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets / sets the text color base for the text in the disabled toolbar items.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		public static Color DisabledToolbarItemTextColorBase
		{
			get
			{
				if(customDisabledToolbarItemTextColorBase == Color.Empty)
					// VS.Net draws a LightLight version of this color when disabled.
					return SystemColors.GrayText;
				else
					return customDisabledToolbarItemTextColorBase;
			}
			set
			{
				if(customDisabledToolbarItemTextColorBase != value)
				{
					customDisabledToolbarItemTextColorBase = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets / sets the text color of the menu and toolbar items.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color, the next time it is queried.
		/// </remarks>
		public static Color MenuTextColor
		{
			get
			{
				if(customMenuTextColor == Color.Empty)
					return SystemColors.MenuText;
				else
					return customMenuTextColor;
			}
			set
			{
				if(customMenuTextColor != value)
				{
					customMenuTextColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets / sets the active text color of the menu and toolbar items.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color MenuActiveTextColor
		{
			get
			{
				if(customActiveMenuTextColor == Color.Empty)
				{
					if(SystemColors.Window.R == 255
						&& SystemColors.Window.G == 255
						&& SystemColors.Window.B == 255)
						return SystemColors.ActiveCaptionText;
					else
						return SystemColors.MenuText;
				}
				else
					return customActiveMenuTextColor;
			}
			set
			{
				if(customActiveMenuTextColor != value)
				{
					customActiveMenuTextColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}
	}

    public class VistaMenuColors
    {
        static bool needToUpdateColors = true;

        static Color bgColor = Color.Empty;
        static Color selBorderColor = Color.Empty;
        static Color selBGColorLight = Color.Empty;
        static Color selBGColorDark = Color.Empty;
        static Color borderColor = Color.Empty;
        static Color selImageBorderColor = Color.Empty;
        static Color selImageBGColor = Color.Empty;
        static Color selCheckedMenuBorderColor = Color.Empty;
        static Color selCheckedMenuBGColor = Color.Empty;
        static Color selCheckedMenuColor = Color.Empty;
        static Color separatorColor = Color.Empty;

        static Color customBgColor = Color.Empty;        
        static Color customSelBorderColor = Color.Empty;
        static Color customSelBGColorLight = Color.Empty;
        static Color customSelBGColorDark = Color.Empty;
        static Color customBorderColor = Color.Empty;
        static Color customSelImageBorderColor = Color.Empty;
        static Color customSelImageBGColor = Color.Empty;
        static Color customSelCheckedMenuBorderColor = Color.Empty;
        static Color customSelCheckedMenuBGColor = Color.Empty;
        static Color customSelCheckedMenuColor = Color.Empty;
        static Color customSeparatorColor = Color.Empty;

        public static Color BackgroundColor
        {
            get
            {
                if (customBgColor == Color.Empty)
                    return bgColor;
                else
                    return customBgColor;
            }
            set
            {
                if (customBgColor != value)
                {
                    customBgColor = value;
                    OnMenuColorsChanged(EventArgs.Empty);
                }
            }
        }

        public static Color SelBorderColor
        {
            get
            {
                if (customSelBorderColor == Color.Empty)
                    return selBorderColor;
                else
                    return customSelBorderColor;
            }
            set 
            {
                if (customSelBorderColor != value)
                {
                    customSelBorderColor = value;
                    OnMenuColorsChanged(EventArgs.Empty);
                }
            }
        }

        public static Color SelBGColorLight
        {
            get
            {
                if (customSelBGColorLight == Color.Empty)
                    return selBGColorLight;
                else
                    return customSelBGColorLight;
            }
            set
            {
                if (customSelBGColorLight != value)
                {
                    customSelBGColorLight = value;
                    OnMenuColorsChanged(EventArgs.Empty);
                }
            }
        }

        public static Color SelBGColorDark
        {
            get
            {
                if (customSelBGColorDark == Color.Empty)
                    return selBGColorDark;
                else
                    return customSelBGColorDark;
            }
            set
            {
                if (customSelBGColorDark != value)
                {
                    customSelBGColorDark = value;
                    OnMenuColorsChanged(EventArgs.Empty);
                }
            }
        }

        public static Color BorderColor
        {
            get
            {
                if (customBorderColor == Color.Empty)
                    return borderColor;
                else
                    return customBorderColor;
            }
            set
            {
                if (customBorderColor != value)
                {
                    customBorderColor = value;
                    OnMenuColorsChanged(EventArgs.Empty);
                }
            }
        }

        public static Color SelImageBorderColor
        {
            get
            {
                if (customSelImageBorderColor == Color.Empty)
                    return selImageBorderColor;
                else
                    return customSelImageBorderColor;
            }
            set
            {
                if (customSelImageBorderColor != value)
                {
                    customSelImageBorderColor = value;
                    OnMenuColorsChanged(EventArgs.Empty);
                }
            }
        }

        public static Color SelImageBGColor
        {
            get
            {
                if (customSelImageBGColor == Color.Empty)
                    return selImageBGColor;
                else
                    return customSelImageBGColor;
            }
            set
            {
                if (customSelImageBGColor != value)
                {
                    customSelImageBGColor = value;
                    OnMenuColorsChanged(EventArgs.Empty);
                }
            }
        }

        public static Color SelCheckedMenuBorderColor
        {
            get
            {
                if (customSelCheckedMenuBorderColor == Color.Empty)
                    return selCheckedMenuBorderColor;
                else
                    return customSelCheckedMenuBorderColor;
            }
            set
            {
                if (customSelCheckedMenuBorderColor != value)
                {
                    customSelCheckedMenuBorderColor = value;
                    OnMenuColorsChanged(EventArgs.Empty);
                }
            }
        }

        public static Color SelCheckedMenuBGColor
        {
            get
            {
                if (customSelCheckedMenuBGColor == Color.Empty)
                    return selCheckedMenuBGColor;
                else
                    return customSelCheckedMenuBGColor;
            }
            set
            {
                if (customSelCheckedMenuBGColor != value)
                {
                    customSelCheckedMenuBGColor = value;
                    OnMenuColorsChanged(EventArgs.Empty);
                }
            }
        }

        public static Color SelCheckedMenuColor
        {
            get
            {
                if (customSelCheckedMenuColor == Color.Empty)
                    return selCheckedMenuColor;
                else
                    return customSelCheckedMenuColor;
            }
            set
            {
                if (customSelCheckedMenuColor != value)
                {
                    customSelCheckedMenuColor = value;
                    OnMenuColorsChanged(EventArgs.Empty);
                }
            }
        }

        public static Color SeparatorColor
        {
            get
            {
                if (customSeparatorColor != Color.Empty)
                    return customSeparatorColor;
                else
                    return separatorColor;
            }
            set
            {
                if (customSeparatorColor != value)
                {
                    customSeparatorColor = value;
                    OnMenuColorsChanged(EventArgs.Empty);
                }
            }
        }

		/// <summary>
		/// Initializes default colors based on SystemColors.
		/// </summary>
		/// <remarks>
		/// Calling this will not affect the custom colors set using the properties.
		/// </remarks>
		public static void UpdateMenuColors()
		{
			if(!needToUpdateColors)
				return;

			needToUpdateColors = false;

            bgColor = Color.FromArgb(240, 240, 240);
            selBorderColor = Color.FromArgb(173, 219, 239);
            selBGColorLight = Color.FromArgb(235, 241, 245);
            selBGColorDark = Color.FromArgb(218, 235, 243);
            borderColor = Color.FromArgb(224, 224, 224);
            selImageBorderColor = Color.FromArgb(115, 125, 255);
            selImageBGColor = Color.FromArgb(206, 239, 255);
            selCheckedMenuBorderColor = Color.FromArgb(206, 211, 231);
            selCheckedMenuBGColor = Color.FromArgb(231, 239, 247);
            selCheckedMenuColor = Color.FromArgb(8, 16, 165);
            separatorColor = Color.FromArgb(231, 227, 231);

			OnMenuColorsChanged(EventArgs.Empty);
		}

		static void OnMenuColorsChanged(EventArgs e)
		{
			if(MenuColorsChanged != null)
			{
				MenuColorsChanged(null, e);
			}
		}

		/// <summary>
		/// Fired when the colors have changed either because of change in system colors or
		/// when a custom color is specified using one of the properties.
		/// </summary>
		/// <remarks>
		/// Take a look at the class reference for this class for information on how to notify
		/// this class regarding system color changes.
		/// </remarks>
		public static event EventHandler MenuColorsChanged;

		/// <summary>
		/// Initializes the default colors.
		/// </summary>
		static VistaMenuColors()
		{
			UpdateMenuColors();
		}
    }

	public class VS2005Colors
	{
		#region Class members
		private static Color m_panelColor = Color.Empty;
		private static Color m_borderColor = Color.Empty;
		private static Color m_tabItemColor = Color.Empty;
		private static Color m_innerBorderColor = Color.Empty;
		private static Color m_leftAHPanelColor = Color.Empty;
		private static Color m_rightAHPanelColor = Color.Empty;

		private static Color m_MenuSelectedItemColor = Color.Empty;
		private static Color m_MenuSelectedItemBorderColor = Color.Empty;
		private static Color m_MenuBorderColor = Color.Empty;
		private static Color m_MenuSeparatorColor = Color.Empty;
		private static Color m_MenuColumnStyleDarkColor = Color.Empty;
		private static Color m_MenuColumnStyleLightColor = Color.Empty;
		private static Color m_MenuCheckMarkColor = Color.Empty;
		private static Color m_MenuSelectedCheckMarkColor = Color.Empty;
		private static Color m_MenuBackground = Color.Empty;

		private static Color m_BarItemHighlightBorderColor = Color.Empty;
		private static Color m_BarItemPressBorderColor = Color.Empty;
		private static Color m_BarItemCheckBorderColor = Color.Empty;
		private static Color m_BarItemHighlightLightColor = Color.Empty;
		private static Color m_BarItemHighlightDarkColor = Color.Empty;
		private static Color m_BarItemPressLightColor = Color.Empty;
		private static Color m_BarItemPressDarkColor = Color.Empty;
		private static Color m_BarItemSeparatorColor = Color.Empty;
		private static Color m_BarItemCheckLightColor = Color.Empty;
		private static Color m_BarItemCheckDarkColor = Color.Empty;
		
		private static Color m_dDBarItemBorderColor = Color.Empty;
		private static Color m_dDBarItemLightColor = Color.Empty;
		private static Color m_dDBarItemDarkColor = Color.Empty;

		private static Color m_dockBarLightColor = Color.Empty;
		private static Color m_dockBarDarkColor = Color.Empty;
		private static Color m_CommandBarDropDownLightColor = Color.Empty;
		private static Color m_CommandBarDropDownDarkColor = Color.Empty;
		private static Color m_CommandBarDarkColor = Color.Empty;
		private static Color m_CommandBarLightColor = Color.Empty;
		private static Color m_CommandBarBorderColor = Color.Empty;
		private static Color m_DropDownHighlightLightColor = Color.Empty;
		private static Color m_DropDownHighlightDarkColor = Color.Empty;
		private static Color m_DropDownPressedLightColor = Color.Empty;
		private static Color m_DropDownPressedDarkColor = Color.Empty;
		private static Color m_FloatPressButtonColor = Color.Empty;
		private static Color m_FloatPressButtonBorderColor = Color.Empty;
		private static Color m_FloatCommandBarLightColor = Color.Empty;
		private static Color m_FloatCommandBarDarkColor = Color.Empty;
		private static Color m_FloatLightBorderColor = Color.Empty;
		private static Color m_FloatBackgroundColor = Color.Empty;
		private static Color m_FloatBorderColor = Color.Empty;
		private static Color m_FloatCaptionColor = Color.Empty;

        private static bool needToUpdateColors = true;
        private static bool needToUpdateStyleColors = true;
		#endregion

		#region Class properties
		/// <summary>
		/// Gets a value indicating whether themed colors are used.
		/// </summary>
		/// <value><c>true</c> if themed colors are used, <c>false</c> otherwise.</value>
		public static bool UseThemedColors
		{
			get
			{
				bool themedcolors = false;
				if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed)
				{
					if (XPThemes.IsDefaultBlueThemeOn || XPThemes.IsOliveGreenThemeOn || XPThemes.IsSilverThemeOn)
						themedcolors = true;
				}
				return themedcolors;
			}
		}

		/// <summary>
		/// gets/sets color of right auto hide panel.
		/// </summary>
		public static Color RightAHPanelColor
		{
			get
			{
				return m_rightAHPanelColor;
			}
			set
			{
				if( m_rightAHPanelColor != value )
				{
					m_rightAHPanelColor = value;
				}
			}
		}

		/// <summary>
		/// Gets/sets color of left AH panel.
		/// </summary>
		public static Color LeftAHPanelColor
		{
			get
			{
				return m_leftAHPanelColor;
			}
			set
			{
				if( m_leftAHPanelColor != value )
				{
					m_leftAHPanelColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color of the panel.
		/// </summary>
		public static Color PanelColor
		{
			get
			{
				return m_panelColor;
			}
			set
			{
				if (m_panelColor != value)
				{
					m_panelColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color of the border.
		/// </summary>
		public static Color BorderColor
		{
			get
			{
				return m_borderColor;
			}
			set
			{
				if (m_borderColor != value)
				{
					m_borderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color of the inner border.
		/// </summary>
		public static Color InnerBorderColor
		{
			get
			{
				return m_innerBorderColor;
			}
			set
			{
				if (m_innerBorderColor != value)
				{
					m_innerBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color of the tab item.
		/// </summary>
		public static Color TabItemColor
		{
			get
			{
				return m_tabItemColor;
			}
			set
			{
				if ( m_tabItemColor != value )
				{
					m_tabItemColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for border of the DropDownBarItem.
		/// </summary>
		public static Color DDBarItemBorderColor
		{
			get
			{
				return m_dDBarItemBorderColor;
			}
			set
			{
				if( m_dDBarItemBorderColor != value )
				{
					m_dDBarItemBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the light color of the DropDownBarItem.
		/// </summary>
		public static Color DDBarItemLightColor
		{
			get
			{
				return m_dDBarItemLightColor;
			}
			set
			{
				if( m_dDBarItemLightColor != value )
				{
					m_dDBarItemLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the dark color of the DropDownBarItem.
		/// </summary>
		public static Color DDBarItemDarkColor
		{
			get
			{
				return m_dDBarItemDarkColor;
			}
			set
			{
				if( m_dDBarItemDarkColor != value )
				{
					m_dDBarItemDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for border of the menu.
		/// </summary>
		public static Color MenuBorderColor
		{
			get
			{
				return m_MenuBorderColor;
			}
			set
			{
				if( m_MenuBorderColor != value )
				{
					m_MenuBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for separator of the menu.
		/// </summary>
		public static Color MenuSeparatorColor
		{
			get
			{
				return m_MenuSeparatorColor;
			}
			set
			{
				if( m_MenuSeparatorColor != value )
				{
					m_MenuSeparatorColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for border selected item of the menu.
		/// </summary>
		public static Color MenuSelectedItemBorderColor
		{
			get
			{
				return m_MenuSelectedItemBorderColor;
			}
			set
			{
				if( m_MenuSelectedItemBorderColor != value )
				{
					m_MenuSelectedItemBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for selected item of the menu.
		/// </summary>
		public static Color MenuSelectedItemColor
		{
			get
			{
				return m_MenuSelectedItemColor;
			}
			set
			{
				if( m_MenuSelectedItemColor != value )
				{
					m_MenuSelectedItemColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the dark color for column of the menu.
		/// </summary>
		public static Color MenuColumnStyleDarkColor
		{
			get
			{
				return m_MenuColumnStyleDarkColor;
			}
			set
			{
				if( m_MenuColumnStyleDarkColor != value )
				{
					m_MenuColumnStyleDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the light color for column of the menu.
		/// </summary>
		public static Color MenuColumnStyleLightColor
		{
			get
			{
				return m_MenuColumnStyleLightColor;
			}
			set
			{
				if( m_MenuColumnStyleLightColor != value )
				{
					m_MenuColumnStyleLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for check mark of the menu.
		/// </summary>
		public static Color MenuCheckMarkColor
		{
			get
			{
				return m_MenuCheckMarkColor;
			}
			set
			{
				if( m_MenuCheckMarkColor != value )
				{
					m_MenuCheckMarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for selected check mark of the menu.
		/// </summary>
		public static Color MenuSelectedCheckMarkColor
		{
			get
			{
				return m_MenuSelectedCheckMarkColor;
			}
			set
			{
				if( m_MenuSelectedCheckMarkColor != value )
				{
					m_MenuSelectedCheckMarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the background color of the menu.
		/// </summary>
		public static Color MenuBackground
		{
			get
			{
				return m_MenuBackground;
			}
			set
			{
				if( m_MenuBackground != value )
				{
					m_MenuBackground = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets border color of the BarItem.
		/// </summary>
		public static Color BarItemHighlightBorderColor
		{
			get
			{
				return m_BarItemHighlightBorderColor;
			}
			set
			{
				if( m_BarItemHighlightBorderColor != value )
				{
					m_BarItemHighlightBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets border color of the pressed BarItem.
		/// </summary>
		public static Color BarItemPressBorderColor
		{
			get
			{
				return m_BarItemPressBorderColor;
			}
			set
			{
				if( m_BarItemPressBorderColor != value )
				{
					m_BarItemPressBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets border color of the checked BarItem.
		/// </summary>
		public static Color BarItemCheckBorderColor
		{
			get
			{
				return m_BarItemCheckBorderColor;
			}
			set
			{
				if( m_BarItemCheckBorderColor != value )
				{
					m_BarItemCheckBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color of the checked BarItem.
		/// </summary>
		public static Color BarItemCheckLightColor
		{
			get
			{
				return m_BarItemCheckLightColor;
			}
			set
			{
				if( m_BarItemCheckLightColor != value )
				{
					m_BarItemCheckLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets dark color of the checked BarItem.
		/// </summary>
		public static Color BarItemCheckDarkColor
		{
			get
			{
				return m_BarItemCheckDarkColor;
			}
			set
			{
				if( m_BarItemCheckDarkColor != value )
				{
					m_BarItemCheckDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color of the BarItem.
		/// </summary>
		public static Color BarItemHighlightLightColor
		{
			get
			{
				return m_BarItemHighlightLightColor;
			}
			set
			{
				if( m_BarItemHighlightLightColor != value )
				{
					m_BarItemHighlightLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets dark color of the BarItem.
		/// </summary>
		public static Color BarItemHighlightDarkColor
		{
			get
			{
				return m_BarItemHighlightDarkColor;
			}
			set
			{
				if( m_BarItemHighlightDarkColor != value )
				{
					m_BarItemHighlightDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color of the pressed BarItem.
		/// </summary>
		public static Color BarItemPressLightColor
		{
			get
			{
				return m_BarItemPressLightColor;
			}
			set
			{
				if( m_BarItemPressLightColor != value )
				{
					m_BarItemPressLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets dark color of the pressed BarItem.
		/// </summary>
		public static Color BarItemPressDarkColor
		{
			get
			{
				return m_BarItemPressDarkColor;
			}
			set
			{
				if( m_BarItemPressDarkColor != value )
				{
					m_BarItemPressDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets color of the separator BarItem.
		/// </summary>
		public static Color BarItemSeparatorColor
		{
			get
			{
				return m_BarItemSeparatorColor;
			}
			set
			{
				if( m_BarItemSeparatorColor != value )
				{
					m_BarItemSeparatorColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color of the DockBar.
		/// </summary>
		public static Color DockBarLightColor
		{
			get
			{
				return m_dockBarLightColor;
			}
			set
			{
				if( m_dockBarLightColor != value )
				{
					m_dockBarLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets dark color of the DockBar.
		/// </summary>
		public static Color DockBarDarkColor
		{
			get
			{
				return m_dockBarDarkColor;
			}
			set
			{
				if( m_dockBarDarkColor != value )
				{
					m_dockBarDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color for dropdown button of the CommandBar.
		/// </summary>
		public static Color CommandBarDropDownLightColor
		{
			get
			{
				return m_CommandBarDropDownLightColor;
			}
			set
			{
				if( m_CommandBarDropDownLightColor != value )
				{
					m_CommandBarDropDownLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets dark color for dropdown button of the CommandBar.
		/// </summary>
		public static Color CommandBarDropDownDarkColor
		{
			get
			{
				return m_CommandBarDropDownDarkColor;
			}
			set
			{
				if( m_CommandBarDropDownDarkColor != value )
				{
					m_CommandBarDropDownDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets dark color of the CommandBar.
		/// </summary>
		public static Color CommandBarDarkColor
		{
			get
			{
				return m_CommandBarDarkColor;
			}
			set
			{
				if( m_CommandBarDarkColor != value )
				{
					m_CommandBarDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color of the CommandBar.
		/// </summary>
		public static Color CommandBarLightColor
		{
			get
			{
				return m_CommandBarLightColor;
			}
			set
			{
				if( m_CommandBarLightColor != value )
				{
					m_CommandBarLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets color for border of the CommandBar.
		/// </summary>
		public static Color CommandBarBorderColor
		{
			get
			{
				return m_CommandBarBorderColor;
			}
			set
			{
				if( m_CommandBarBorderColor != value )
				{
					m_CommandBarBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color for highlight dropdown button of the CommandBar.
		/// </summary>
		public static Color DropDownHighlightLightColor
		{
			get
			{
				return m_DropDownHighlightLightColor;
			}
			set
			{
				if( m_DropDownHighlightLightColor != value )
				{
					m_DropDownHighlightLightColor = value;
				}
			}
		}
		
		/// <summary>
		/// Gets or sets dark color for highlight dropdown button of the CommandBar.
		/// </summary>
		public static Color DropDownHighlightDarkColor
		{
			get
			{
				return m_DropDownHighlightDarkColor;
			}
			set
			{
				if( m_DropDownHighlightDarkColor != value )
				{
					m_DropDownHighlightDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color for pressed dropdown button of the CommandBar.
		/// </summary>
		public static Color DropDownPressedLightColor
		{
			get
			{
				return m_DropDownPressedLightColor;
			}
			set
			{
				if( m_DropDownPressedLightColor != value )
				{
					m_DropDownPressedLightColor = value;
				}
			}
		}
		
		/// <summary>
		/// Gets or sets dark color for pressed dropdown button of the CommandBar.
		/// </summary>
		public static Color DropDownPressedDarkColor
		{
			get
			{
				return m_DropDownPressedDarkColor;
			}
			set
			{
				if( m_DropDownPressedDarkColor != value )
				{
					m_DropDownPressedDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets color for pressed dropdown button of the floating CommandBar.
		/// </summary>
		public static Color FloatPressButtonColor
		{
			get
			{
				return m_FloatPressButtonColor;
			}
			set
			{
				if( m_FloatPressButtonColor != value )
				{
					m_FloatPressButtonColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets border color for pressed dropdown button of the floating CommandBar.
		/// </summary>
		public static Color FloatPressButtonBorderColor
		{
			get
			{
				return m_FloatPressButtonBorderColor;
			}
			set
			{
				if( m_FloatPressButtonBorderColor != value )
				{
					m_FloatPressButtonBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color of the floating CommandBar.
		/// </summary>
		public static Color FloatCommandBarLightColor
		{
			get
			{
				return m_FloatCommandBarLightColor;
			}
			set
			{
				if( m_FloatCommandBarLightColor != value )
				{
					m_FloatCommandBarLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets dark color of the floating CommandBar.
		/// </summary>
		public static Color FloatCommandBarDarkColor
		{
			get
			{
				return m_FloatCommandBarDarkColor;
			}
			set
			{
				if( m_FloatCommandBarDarkColor != value )
				{
					m_FloatCommandBarDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets color for light border of the floating CommandBar.
		/// </summary>
		public static Color FloatLightBorderColor
		{
			get
			{
				return m_FloatLightBorderColor;
			}
			set
			{
				if( m_FloatLightBorderColor != value )
				{
					m_FloatLightBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets background color of the floating CommandBar.
		/// </summary>
		public static Color FloatBackgroundColor
		{
			get
			{
				return m_FloatBackgroundColor;
			}
			set
			{
				if( m_FloatBackgroundColor != value )
				{
					m_FloatBackgroundColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets color for border of the floating CommandBar.
		/// </summary>
		public static Color FloatBorderColor
		{
			get
			{
				return m_FloatBorderColor;
			}
			set
			{
				if( m_FloatBorderColor != value )
				{
					m_FloatBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets color for caption text of the floating CommandBar.
		/// </summary>
		public static Color FloatCaptionColor
		{
			get
			{
				return m_FloatCaptionColor;
			}
			set
			{
				if( m_FloatCaptionColor != value )
				{
					m_FloatCaptionColor = value;
				}
			}
		}

		#endregion

		#region Class static methods

		/// <summary>
		/// Initializes default colors based on SystemColors.
		/// </summary>
		/// <remarks>
		/// Calling this will not affect the custom colors set using the properties.
		/// </remarks>
		public static void UpdateStyleColors()
		{
            if (!needToUpdateStyleColors)
                return;

            needToUpdateStyleColors = false;

			if ( XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed )
			{
				if (XPThemes.IsDefaultBlueThemeOn || XPThemes.IsOliveGreenThemeOn)
				{
					m_panelColor = Color.FromArgb(229, 229, 215);
					m_borderColor = Color.FromArgb(172, 168, 153);
					m_innerBorderColor = Color.FromArgb(226, 222, 197);
					m_tabItemColor = Color.FromArgb(236, 232, 215);
					m_leftAHPanelColor = Color.FromArgb(229, 229, 215);
					m_rightAHPanelColor = Color.FromArgb(244, 242, 232);
				}
				else
					if (XPThemes.IsSilverThemeOn)
					{
						m_panelColor = Color.FromArgb(215, 215, 229);
						m_borderColor = Color.FromArgb(145, 155, 156);
						m_innerBorderColor = Color.FromArgb(190, 190, 216);
						m_tabItemColor = Color.FromArgb(243, 243, 247);
						m_leftAHPanelColor = Color.FromArgb(215, 215, 229);
						m_rightAHPanelColor = Color.FromArgb(247, 243, 247);
                    }
                    else
                    {
                        m_borderColor = Color.FromArgb(128, 128, 128);
                        m_panelColor = Color.FromArgb(212, 208, 200);
                        m_tabItemColor = Color.FromArgb(212, 208, 200);
                        m_innerBorderColor = Color.FromArgb(128, 128, 128);
                        m_rightAHPanelColor = Color.FromArgb(246, 245, 244);
                        m_leftAHPanelColor = Color.FromArgb(212, 208, 200);
                    }
			}
			else
			{
				m_borderColor = Color.FromArgb(128, 128, 128);
				m_panelColor = Color.FromArgb(212, 208, 200);
				m_tabItemColor = Color.FromArgb(212, 208, 200);
				m_innerBorderColor = Color.FromArgb(128, 128, 128);
				m_rightAHPanelColor = Color.FromArgb(246, 245, 244);
				m_leftAHPanelColor = Color.FromArgb(212, 208, 200);
			}

			OnMenuColorsChanged(EventArgs.Empty);
		}


		public static void UpdateMenuColors()
		{
			bool unthemed = false;

            if (!needToUpdateColors)
                return;

            needToUpdateColors = false;
			
			if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed )
			{
				if( XPThemes.IsDefaultBlueThemeOn )
				{
					m_MenuCheckMarkColor = Color.FromArgb( 225, 230, 232 );
					m_MenuSelectedCheckMarkColor = Color.FromArgb( 49, 106, 197 );
					m_MenuColumnStyleDarkColor = Color.FromArgb( 196, 195, 172 );
					m_MenuColumnStyleLightColor = Color.FromArgb( 254, 254, 251 );
					m_MenuSelectedItemBorderColor = Color.FromArgb( 49, 106, 197 );
					m_MenuSelectedItemColor = Color.FromArgb( 193, 210, 238 );
					m_MenuSeparatorColor = Color.FromArgb( 197, 194, 184 );
					m_MenuBorderColor = Color.FromArgb( 138, 134, 122 );
					m_MenuBackground = Color.FromArgb( 252, 252, 249 );

					m_BarItemHighlightBorderColor = Color.FromArgb( 49, 106, 197 );
					m_BarItemPressBorderColor = Color.FromArgb( 75, 75, 111 );
					m_BarItemCheckBorderColor = Color.FromArgb( 75, 75, 111 );
					m_BarItemHighlightLightColor = Color.FromArgb( 193, 210, 238 );
					m_BarItemHighlightDarkColor = Color.FromArgb( 193, 210, 238 );
					m_BarItemPressLightColor = Color.FromArgb( 152, 181, 226 );
					m_BarItemPressDarkColor = Color.FromArgb( 152, 181, 226 );
					m_BarItemSeparatorColor = Color.FromArgb( 197, 194, 184 );
					m_BarItemCheckLightColor = Color.FromArgb( 225, 230, 232 );
					m_BarItemCheckDarkColor = Color.FromArgb( 225, 230, 232 );

					m_dDBarItemBorderColor = Color.FromArgb( 138, 134, 122 );
					m_dDBarItemLightColor = Color.FromArgb( 251, 251, 249 );
					m_dDBarItemDarkColor = Color.FromArgb( 247, 245, 239 );

					m_dockBarDarkColor = Color.FromArgb( 229, 229, 215 );
					m_dockBarLightColor = Color.FromArgb( 243, 242, 231 );
					m_CommandBarDropDownDarkColor = Color.FromArgb( 172, 168, 153 );
					m_CommandBarDropDownLightColor = Color.FromArgb( 239, 238, 235 );
					m_CommandBarDarkColor = Color.FromArgb( 192, 192, 168 );
					m_CommandBarLightColor = Color.FromArgb( 250, 249, 245 );
					m_CommandBarBorderColor = Color.FromArgb( 163, 163, 124 );
					m_DropDownHighlightLightColor = Color.FromArgb( 193, 210, 238 );
					m_DropDownHighlightDarkColor = Color.FromArgb( 193, 210, 238 );
					m_DropDownPressedLightColor = Color.FromArgb( 225, 230, 232 );
					m_DropDownPressedDarkColor = Color.FromArgb( 225, 230, 232 );
					m_FloatPressButtonColor = Color.FromArgb( 239, 237, 222 );
					m_FloatPressButtonBorderColor = Color.FromArgb( 138, 134, 122 );
					m_FloatCommandBarLightColor = Color.FromArgb( 251, 250, 247 );
					m_FloatCommandBarDarkColor = Color.FromArgb( 200, 199, 178 );
					m_FloatLightBorderColor = Color.FromArgb( 239, 237, 222 );
					m_FloatBackgroundColor = Color.FromArgb( 172, 168, 153 );
					m_FloatBorderColor = Color.FromArgb( 146, 143, 130 );
					m_FloatCaptionColor = Color.White;
					
				}
				else if( XPThemes.IsOliveGreenThemeOn )
				{
					m_MenuCheckMarkColor = Color.FromArgb( 194, 207, 158 );
					m_MenuSelectedCheckMarkColor = Color.FromArgb( 147, 160, 112 );
					m_MenuColumnStyleDarkColor = Color.FromArgb( 196, 195, 172 );
					m_MenuColumnStyleLightColor = Color.FromArgb( 254, 254, 251 );
					m_MenuSelectedItemBorderColor = Color.FromArgb( 147, 160, 112 );
					m_MenuSelectedItemColor = Color.FromArgb( 182, 198, 141 );
					m_MenuSeparatorColor = Color.FromArgb( 197, 194, 184 );
					m_MenuBorderColor = Color.FromArgb( 138, 134, 122 );
					m_MenuBackground = Color.FromArgb( 252, 252, 249 );

					m_BarItemHighlightBorderColor = Color.FromArgb( 147, 160, 112 );
					m_BarItemPressBorderColor = Color.FromArgb( 147, 160, 112 );
					m_BarItemCheckBorderColor = Color.FromArgb( 147, 160, 112 );
					m_BarItemHighlightLightColor = Color.FromArgb( 182, 198, 141 );
					m_BarItemHighlightDarkColor = Color.FromArgb( 182, 198, 141 );
					m_BarItemPressLightColor = Color.FromArgb( 147, 160, 112 );
					m_BarItemPressDarkColor = Color.FromArgb( 147, 160, 112 );
					m_BarItemSeparatorColor = Color.FromArgb( 197, 194, 184 );
					m_BarItemCheckLightColor = Color.FromArgb( 182, 198, 141 );
					m_BarItemCheckDarkColor = Color.FromArgb( 182, 198, 141 );

					m_dDBarItemBorderColor = Color.FromArgb( 138, 134, 122 );
					m_dDBarItemLightColor = Color.FromArgb( 251, 251, 249 );
					m_dDBarItemDarkColor = Color.FromArgb( 247, 245, 239 );

					m_dockBarDarkColor = Color.FromArgb( 229, 229, 215 );
					m_dockBarLightColor = Color.FromArgb( 243, 242, 231 );
					m_CommandBarDropDownDarkColor = Color.FromArgb( 172, 168, 153 );
					m_CommandBarDropDownLightColor = Color.FromArgb( 239, 238, 235 );
					m_CommandBarDarkColor = Color.FromArgb( 192, 192, 168 );
					m_CommandBarLightColor = Color.FromArgb( 250, 249, 245 );
					m_CommandBarBorderColor = Color.FromArgb( 163, 163, 124 );
					m_DropDownHighlightLightColor = Color.FromArgb( 223, 227, 212 );
					m_DropDownHighlightDarkColor = Color.FromArgb( 223, 227, 212 );
					m_DropDownPressedLightColor = Color.FromArgb( 194, 207, 158 );
					m_DropDownPressedDarkColor = Color.FromArgb( 194, 207, 158 );
					m_FloatPressButtonColor = Color.FromArgb( 239, 237, 222 );
					m_FloatPressButtonBorderColor = Color.FromArgb( 138, 134, 122 );
					m_FloatCommandBarLightColor = Color.FromArgb( 251, 250, 247 );
					m_FloatCommandBarDarkColor = Color.FromArgb( 200, 199, 178 );
					m_FloatLightBorderColor = Color.FromArgb( 239, 237, 222 );
					m_FloatBackgroundColor = Color.FromArgb( 172, 168, 153 );
					m_FloatBorderColor = Color.FromArgb( 146, 143, 130 );
					m_FloatCaptionColor = Color.White;
					
				}
				else if( XPThemes.IsSilverThemeOn )
				{
					m_MenuCheckMarkColor = Color.FromArgb( 255, 192, 111 );
					m_MenuSelectedCheckMarkColor = Color.FromArgb( 254, 128, 62 );
					m_MenuColumnStyleDarkColor = Color.FromArgb( 159, 157, 185 );
					m_MenuColumnStyleLightColor = Color.FromArgb( 249, 249, 254 );
					m_MenuSelectedItemBorderColor = Color.FromArgb( 75, 75, 111 );
					m_MenuSelectedItemColor = Color.FromArgb( 254, 238, 194 );
					m_MenuSeparatorColor = Color.FromArgb( 110, 109, 143 );
					m_MenuBorderColor = Color.FromArgb( 124, 124, 148 );
					m_MenuBackground = Color.FromArgb( 253, 250, 255 );

					m_BarItemHighlightBorderColor = Color.FromArgb( 75, 75, 111 );
					m_BarItemPressBorderColor = Color.FromArgb( 75, 75, 111 );
					m_BarItemCheckBorderColor = Color.FromArgb( 75, 75, 111 );
					m_BarItemHighlightLightColor = Color.FromArgb( 255, 244, 204 );
					m_BarItemHighlightDarkColor = Color.FromArgb( 255, 208, 145 );
					m_BarItemPressLightColor = Color.FromArgb( 254, 145, 78 );
					m_BarItemPressDarkColor = Color.FromArgb( 255, 211, 142 );
					m_BarItemSeparatorColor = Color.FromArgb( 110, 109, 143 );
					
					m_BarItemCheckLightColor = Color.FromArgb( 255, 213, 140 );
					m_BarItemCheckDarkColor = Color.FromArgb( 255, 173, 85 );

					m_dDBarItemBorderColor = Color.FromArgb( 124, 124, 148 );
					m_dDBarItemLightColor = Color.FromArgb( 232, 233, 241 );
					m_dDBarItemDarkColor = Color.FromArgb( 186, 185, 205 );

					m_dockBarDarkColor = Color.FromArgb( 215, 215, 229 );
					m_dockBarLightColor = Color.FromArgb( 243, 243, 247 );
					m_CommandBarDropDownDarkColor = Color.FromArgb( 118, 116, 146 );
					m_CommandBarDropDownLightColor = Color.FromArgb( 179, 178, 200 );
					m_CommandBarDarkColor = Color.FromArgb( 153, 151, 181 );
					m_CommandBarLightColor = Color.FromArgb( 243, 244, 250 );
					m_CommandBarBorderColor = Color.FromArgb( 124, 124, 148 );
					m_DropDownHighlightLightColor = Color.FromArgb( 255, 248, 211 );
					m_DropDownHighlightDarkColor = Color.FromArgb( 255, 193, 118 );
					m_DropDownPressedLightColor = Color.FromArgb( 254, 149, 82 );
					m_DropDownPressedDarkColor = Color.FromArgb( 255, 217, 149 );
					m_FloatPressButtonColor = Color.FromArgb( 214, 211, 231 );
					m_FloatPressButtonBorderColor = Color.FromArgb( 124, 124, 148 );
					m_FloatCommandBarLightColor = Color.FromArgb( 245, 245, 252 );
					m_FloatCommandBarDarkColor = Color.FromArgb( 166, 165, 191 );
					m_FloatLightBorderColor = Color.FromArgb( 219, 218, 228 );
					m_FloatBackgroundColor = Color.FromArgb( 122, 121, 153 );
					m_FloatBorderColor = Color.FromArgb( 122, 121, 153 );
					m_FloatCaptionColor = Color.White;
				}
				else
				{
					unthemed = true;
				}
			}
			else
			{
				unthemed = true;
			}

			if( unthemed )
			{
				MenuColors.UpdateMenuColors();

				m_MenuBackground = Color.FromArgb( 252, 252, 249 );
				m_MenuCheckMarkColor = MenuColors.CheckedSelColor;
				m_MenuSelectedCheckMarkColor = MenuColors.CheckedSelColor;
				m_MenuColumnStyleDarkColor = SystemColors.Control;
				m_MenuColumnStyleLightColor = SystemColors.Window;
				m_MenuSelectedItemBorderColor = MenuColors.SelBorderColor;
				m_MenuSelectedItemColor = MenuColors.SelColor;
				m_MenuSeparatorColor = SystemColors.ControlDarkDark;
				m_MenuBorderColor = MenuColors.DropDownBorderColor;

				m_BarItemHighlightBorderColor = MenuColors.SelBorderColor;
				m_BarItemPressBorderColor = MenuColors.SelBorderColor;
				m_BarItemCheckBorderColor = MenuColors.SelBorderColor;
				m_BarItemHighlightLightColor = MenuColors.SelColor;
				m_BarItemHighlightDarkColor = MenuColors.SelColor;
				m_BarItemPressLightColor = ControlPaint.Dark( MenuColors.SelColor, -0.3f );
				m_BarItemPressDarkColor = m_BarItemPressLightColor;
				m_BarItemSeparatorColor = SystemColors.ControlDark;
				m_BarItemCheckLightColor = ControlPaint.Dark( MenuColors.SelColor, -0.3f );
				m_BarItemCheckDarkColor = m_BarItemCheckLightColor;

				m_dDBarItemBorderColor = MenuColors.DropDownBorderColor;
				m_dDBarItemLightColor = SystemColors.ControlLightLight;
				m_dDBarItemDarkColor = SystemColors.ControlLightLight;

				m_dockBarDarkColor = SystemColors.Control;
				m_dockBarLightColor = MenuColors.MenuBGColor;
				m_CommandBarDropDownDarkColor = SystemColors.ControlDark;
				m_CommandBarDropDownLightColor = ControlPaint.Light( SystemColors.ControlDark, 1.7f );
				m_CommandBarDarkColor = SystemColors.Control;
				m_CommandBarLightColor = SystemColors.Window;
				m_CommandBarBorderColor = SystemColors.Control;
				m_DropDownHighlightLightColor = MenuColors.SelColor;
				m_DropDownHighlightDarkColor = MenuColors.SelColor;
				m_DropDownPressedLightColor = ControlPaint.Light( MenuColors.SelColor, 0.7f );
				m_DropDownPressedDarkColor = ControlPaint.Light( MenuColors.SelColor, 0.85f );
				m_FloatPressButtonColor = SystemColors.Control;
				m_FloatPressButtonBorderColor = MenuColors.FloatingCommandBarCaptionColor;
				m_FloatCommandBarLightColor = ControlPaint.Light( SystemColors.Control, 1f );
				m_FloatCommandBarDarkColor = SystemColors.Control;
				m_FloatLightBorderColor = SystemColors.InactiveBorder;
				m_FloatBorderColor = ControlPaint.Dark( SystemColors.ControlDark, -0.2f );
				m_FloatBackgroundColor = SystemColors.ControlDark;
				m_FloatCaptionColor = Color.White;
			}

			OnMenuColorsChanged( EventArgs.Empty );
		}

		static void OnMenuColorsChanged(EventArgs e)
		{
			if (MenuColorsChanged != null)
			{
				MenuColorsChanged(null, e);
			}
		}

		/// <summary>
		/// Fired when the colors have changed either because of change in system colors or
		/// when a custom color is specified using one of the properties.
		/// </summary>
		/// <remarks>
		/// Take a look at the class reference for this class for information on how to notify
		/// this class regarding system color changes.
		/// </remarks>
		public static event EventHandler MenuColorsChanged;
		#endregion

		#region Initialization
		/// <summary>
		/// 
		/// </summary>
		static VS2005Colors()
		{
			UpdateStyleColors();
			UpdateMenuColors();
		}
		#endregion
	}

	public class Office2003Colors
	{
		#region Class members
		private static Color selColor = Color.Empty;
		private static Color pressedSelColor = Color.Empty;

		private static Color checkedColor = Color.Empty;
		private static Color checkedSelColor = Color.Empty;

		private static Color selBorderColor = Color.Empty;

		private static Color menuMarginColorLeft = Color.Empty;
		private static Color menuMarginColorRight = Color.Empty;

		private static Color menuExpandedItemsMarginColorLeft = Color.Empty;
		private static Color menuExpandedItemsMarginColorRight = Color.Empty;
		private static Color dropDownBorderColor = Color.Empty;

		private static Color dockBarColorLight = Color.Empty;
		private static Color dockBarColorDark = Color.Empty;
		private static Color controlGripperColor = Color.Empty;
		private static Color controlBorderColorLight = Color.Empty;
		private static Color controlBorderColorDark = Color.Empty;
		private static Color menuItemHotColorLight = Color.Empty;
		private static Color menuItemHotColorDark = Color.Empty;
		private static Color menuItemPressedColorLight = Color.Empty;
		private static Color menuItemPressedColorDark = Color.Empty;
		private static Color floatingCommandBarCaptionColor = Color.Empty;
		private static Color floatingCommandBarItemPressedColor = Color.Empty;
		private static Color commandBarDropDownColorLight = Color.Empty;
		private static Color commandBarDropDownColorDark = Color.Empty;
		private static Color groupBarHighlightColorLight = Color.Empty;
		private static Color groupBarHighlightColorDark = Color.Empty;
		private static Color groupBarSelectedColorLight = Color.Empty;
		private static Color groupBarSelectedColorDark = Color.Empty;
		private static Color groupBarSelectedHighlightColorLight = Color.Empty;
		private static Color groupBarSelectedHighlightColorDark = Color.Empty;
		private static Color groupBarHeaderColorLight = Color.Empty;
		private static Color groupBarHeaderColorDark = Color.Empty;
		private static Color groupBarItemTextColor = Color.Empty;
		private static Color groupBarItemTextSelectedHighlightColor = Color.Empty;
		private static Color separatorColor = Color.Empty;
        
		// Custom colors:
		private static Color customSelColor = Color.Empty;
		private static Color customPressedSelColor = Color.Empty;

		private static Color customCheckedColor = Color.Empty;
		private	static Color customCheckedSelColor = Color.Empty;
		private static Color customMenuCheckBoxSelBGColor = Color.Empty;

		private static Color customSelBorderColor = Color.Empty;

		private static Color customMenuMarginColorLeft = Color.Empty;
		private static Color customMenuMarginColorRight = Color.Empty;

		private static Color customMenuExpandedItemsMarginColorLeft = Color.Empty;
		private static Color customMenuExpandedItemsMarginColorRight = Color.Empty;
		private static Color customDropDownBorderColor = Color.Empty;

		private static Color customDockBarColorLight = Color.Empty;
		private static Color customDockBarColorDark = Color.Empty;
		private static Color customControlGripperColor = Color.Empty;
		private static Color customControlBorderColorLight = Color.Empty;
		private static Color customControlBorderColorDark = Color.Empty;
		private static Color customMenuItemHotColorLight = Color.Empty;
		private static Color customMenuItemHotColorDark = Color.Empty;
		private static Color customMenuItemPressedColorLight = Color.Empty;
		private static Color customMenuItemPressedColorDark = Color.Empty;
		private static Color customFloatingCommandBarCaptionColor = Color.Empty;
		private static Color customFloatingCommandBarItemPressedColor = Color.Empty;
		private static Color customCommandBarDropDownColorLight = Color.Empty;
		private static Color customCommandBarDropDownColorDark = Color.Empty;
		private static Color customGroupBarHighlightColorLight = Color.Empty;
		private static Color customGroupBarHighlightColorDark = Color.Empty;
		private static Color customGroupBarSelectedColorLight = Color.Empty;
		private static Color customGroupBarSelectedColorDark = Color.Empty;
		private static Color customGroupBarSelectedHighlightColorLight = Color.Empty;
		private static Color customGroupBarSelectedHighlightColorDark = Color.Empty;
		private static Color customGroupBarHeaderColorLight = Color.Empty;
		private static Color customGroupBarHeaderColorDark = Color.Empty;
		private static Color customGroupBarItemTextColor = Color.Empty;
		private static Color customGroupBarItemTextSelectedHighlightColor = Color.Empty;
		private static Color customSeparatorColor = Color.Empty;

		static bool needToUpdateColors = true;
		#endregion

		#region Class Properties
		public static bool UseThemedColors
		{
			get
			{
				bool themedcolors = false;
				if(XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed)
				{
					if(XPThemes.IsDefaultBlueThemeOn || XPThemes.IsOliveGreenThemeOn || XPThemes.IsSilverThemeOn)
						themedcolors = true;
				}
				return themedcolors;
			}
		}
		#endregion

		#region COLOR_INIT
		public static void UpdateMenuColors()
		{
			if(!needToUpdateColors)
				return;

			needToUpdateColors = false;

			bool unthemed = false;
			if(XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed)
			{			
				pressedSelColor = Color.FromArgb(254, 128, 62);

				if(XPThemes.IsDefaultBlueThemeOn || Environment.OSVersion.Version.Major > 5 /* For Vista and above versions apply default blue theme*/)
				{
					selColor = Color.FromArgb(255, 238, 194);

					selBorderColor = Color.FromArgb(0, 0, 128);

					menuMarginColorLeft = Color.FromArgb(222, 236, 254);
					menuMarginColorRight = Color.FromArgb(147, 182, 232);

					menuExpandedItemsMarginColorLeft = Color.FromArgb(203, 221, 246);
					menuExpandedItemsMarginColorRight = Color.FromArgb(141, 178, 230);

					checkedColor = Color.FromArgb(255, 192, 111);
					checkedSelColor = Color.FromArgb(254, 128, 62);

					dropDownBorderColor = Color.FromArgb(0, 45, 150);

					dockBarColorLight = Color.FromArgb(196, 217, 249);
					dockBarColorDark = Color.FromArgb(158, 190, 245);
					controlGripperColor = Color.FromArgb(39, 65, 118);
					controlBorderColorLight = Color.FromArgb(117, 166, 241);
					controlBorderColorDark = Color.FromArgb(59, 97, 156);
					menuItemHotColorLight = Color.FromArgb(255, 248, 211);
					menuItemHotColorDark = Color.FromArgb(255, 193, 118);
					menuItemPressedColorLight = Color.FromArgb(255, 217, 149);
					menuItemPressedColorDark = Color.FromArgb(254, 149, 82);
					floatingCommandBarCaptionColor = Color.FromArgb(42, 102, 201);
					floatingCommandBarItemPressedColor = Color.FromArgb(203, 225, 252);
					commandBarDropDownColorLight = Color.FromArgb(117, 166, 241);
					commandBarDropDownColorDark = Color.FromArgb(59, 97, 156);

					groupBarHighlightColorLight = Color.FromArgb(255, 255, 220);
					groupBarHighlightColorDark = Color.FromArgb(247, 192, 91);
					groupBarSelectedColorLight = Color.FromArgb(251, 230, 148);
					groupBarSelectedColorDark = Color.FromArgb(238, 149, 21);
					groupBarSelectedHighlightColorLight = Color.FromArgb(247, 218, 124);
					groupBarSelectedHighlightColorDark = Color.FromArgb(232, 127, 8);
					groupBarHeaderColorLight = Color.FromArgb(89, 135, 214);
					groupBarHeaderColorDark = Color.FromArgb(3, 56, 147);
					groupBarItemTextColor = SystemColors.ControlText;
					groupBarItemTextSelectedHighlightColor = SystemColors.ControlText;
					separatorColor = Color.FromArgb(106, 140, 203);
				}
				else if(XPThemes.IsOliveGreenThemeOn)
				{
					selColor = Color.FromArgb(255, 238, 194);

					selBorderColor = Color.FromArgb(63, 93, 56);

					menuMarginColorLeft = Color.FromArgb(255, 255, 237);
					menuMarginColorRight = Color.FromArgb(186, 201, 148);

					menuExpandedItemsMarginColorLeft = Color.FromArgb(230, 230, 209);
					menuExpandedItemsMarginColorRight = Color.FromArgb(168, 184, 124);

					checkedColor = Color.FromArgb(255, 192, 111);
					checkedSelColor = Color.FromArgb(254, 128, 62);

					dropDownBorderColor = Color.FromArgb(117, 141, 94);

					dockBarColorLight = Color.FromArgb(241, 240, 227);
					dockBarColorDark = Color.FromArgb(217, 217, 167);
					controlGripperColor = Color.FromArgb(81, 94, 51);
					controlBorderColorLight = Color.FromArgb(176, 194, 140);
					controlBorderColorDark = Color.FromArgb(96, 128, 88);
					menuItemHotColorLight = Color.FromArgb(255, 248, 211);
					menuItemHotColorDark = Color.FromArgb(255, 193, 118);
					menuItemPressedColorLight = Color.FromArgb(255, 217, 149);
					menuItemPressedColorDark = Color.FromArgb(254, 149, 82);
					floatingCommandBarCaptionColor = Color.FromArgb(116, 134, 94);
					floatingCommandBarItemPressedColor = Color.FromArgb(216, 227, 182);
					commandBarDropDownColorLight = Color.FromArgb(176, 194, 140);
					commandBarDropDownColorDark = Color.FromArgb(96, 128, 88);

					groupBarHighlightColorLight = Color.FromArgb(255, 255, 220);
					groupBarHighlightColorDark = Color.FromArgb(247, 192, 91);
					groupBarSelectedColorLight = Color.FromArgb(251, 230, 148);
					groupBarSelectedColorDark = Color.FromArgb(238, 149, 21);
					groupBarSelectedHighlightColorLight = Color.FromArgb(247, 218, 124);
					groupBarSelectedHighlightColorDark = Color.FromArgb(232, 127, 8);
					groupBarHeaderColorLight = Color.FromArgb(176, 194, 140);
					groupBarHeaderColorDark = Color.FromArgb(96, 128, 88);
					groupBarItemTextColor = SystemColors.ControlText;
					groupBarItemTextSelectedHighlightColor = SystemColors.ControlText;
					separatorColor = Color.FromArgb(96, 128, 88);
				}
				else if(XPThemes.IsSilverThemeOn)
				{
					selColor = Color.FromArgb(255, 238, 194);

					selBorderColor = Color.FromArgb(75, 75, 111);

					menuMarginColorLeft = Color.FromArgb(249, 249, 255);
					menuMarginColorRight = Color.FromArgb(165, 163, 189);

					menuExpandedItemsMarginColorLeft = Color.FromArgb(215, 215, 226);
					menuExpandedItemsMarginColorRight = Color.FromArgb(133, 131, 162);

					checkedColor = Color.FromArgb(255, 192, 111);
					checkedSelColor = Color.FromArgb(254, 128, 62);

					dropDownBorderColor = Color.FromArgb(124, 124, 148);

					dockBarColorLight = Color.FromArgb(242, 242, 247);
					dockBarColorDark = Color.FromArgb(215, 215, 229);
					controlGripperColor = Color.FromArgb(84, 84, 117);
					controlBorderColorLight = Color.FromArgb(179, 178, 200);
					controlBorderColorDark = Color.FromArgb(124, 124, 148);
					menuItemHotColorLight = Color.FromArgb(255, 248, 211);
					menuItemHotColorDark = Color.FromArgb(255, 193, 118);
					menuItemPressedColorLight = Color.FromArgb(255, 217, 149);
					menuItemPressedColorDark = Color.FromArgb(254, 149, 82);
					floatingCommandBarCaptionColor = Color.FromArgb(122, 121, 153);
					floatingCommandBarItemPressedColor = Color.FromArgb(214, 211, 231);
					commandBarDropDownColorLight = Color.FromArgb(179, 178, 200);
					commandBarDropDownColorDark = Color.FromArgb(124, 124, 148);

					groupBarHighlightColorLight = Color.FromArgb(255, 255, 220);
					groupBarHighlightColorDark = Color.FromArgb(247, 192, 91);
					groupBarSelectedColorLight = Color.FromArgb(251, 230, 148);
					groupBarSelectedColorDark = Color.FromArgb(238, 149, 21);
					groupBarSelectedHighlightColorLight = Color.FromArgb(247, 218, 124);
					groupBarSelectedHighlightColorDark = Color.FromArgb(232, 127, 8);
					groupBarHeaderColorLight = Color.FromArgb(179, 178, 200);
					groupBarHeaderColorDark = Color.FromArgb(124, 124, 148);
					groupBarItemTextColor = SystemColors.ControlText;
					groupBarItemTextSelectedHighlightColor = SystemColors.ControlText;
					separatorColor = Color.FromArgb(110, 109, 143);
				}
				else 
					unthemed = true;
			}
			else
				unthemed = true;
			
			if(unthemed)
			{
				MenuColors.UpdateMenuColors();

				selColor = MenuColors.SelColor;
				selBorderColor = MenuColors.SelBorderColor;
				pressedSelColor = MenuColors.PressedSelColor;

				checkedColor = MenuColors.CheckedSelColor;
				checkedSelColor = MenuColors.CheckedSelColor;

				menuMarginColorLeft = SystemColors.Window;
				menuMarginColorRight = SystemColors.Control;

				menuExpandedItemsMarginColorLeft = SystemColors.Control;
				menuExpandedItemsMarginColorRight = SystemColors.Control;

				dropDownBorderColor = MenuColors.DropDownBorderColor;

				dockBarColorLight = MenuColors.MenuBGColor;
				dockBarColorDark = SystemColors.Control;
				controlGripperColor = SystemColors.ControlDark;
				controlBorderColorLight = ControlPaint.Light(SystemColors.Control, 0.18f);
				controlBorderColorDark = SystemColors.Control;
				menuItemHotColorLight = MenuColors.SelColor;
				menuItemHotColorDark = MenuColors.SelColor;
				menuItemPressedColorLight = ControlPaint.Light(MenuColors.SelColor, 0.7f);
				menuItemPressedColorDark = ControlPaint.Light(MenuColors.SelColor, 0.85f);
				floatingCommandBarCaptionColor = MenuColors.FloatingCommandBarCaptionColor;
				floatingCommandBarItemPressedColor = SystemColors.Control;
				commandBarDropDownColorLight = ControlPaint.Light(SystemColors.ControlDark, 0.5f);
				commandBarDropDownColorDark = SystemColors.ControlDark;

				groupBarHighlightColorLight = MenuColors.SelColor;
				groupBarHighlightColorDark = MenuColors.SelColor;
				groupBarSelectedColorLight = ControlPaint.Light(MenuColors.SelColor, 0.85f);
				groupBarSelectedColorDark = ControlPaint.Light(MenuColors.SelColor, 0.85f);
				groupBarSelectedHighlightColorLight = MenuColors.PressedSelColor;
				groupBarSelectedHighlightColorDark = MenuColors.PressedSelColor;
				groupBarHeaderColorLight = SystemColors.ControlDark;
				groupBarHeaderColorDark = SystemColors.ControlDark;
				groupBarItemTextColor = SystemColors.ControlText;
				// If the GroupBar's SelectedHighlight color has a HSB(the brightness) value less than .65F set the text color to be a light shade.
				if(groupBarSelectedHighlightColorLight.GetBrightness() < 0.65F)
					groupBarItemTextSelectedHighlightColor = Color.White;
				else
					groupBarItemTextSelectedHighlightColor = SystemColors.ControlText;
				separatorColor = SystemColors.ControlDarkDark;
			}
			
			OnMenuColorsChanged(EventArgs.Empty);
		}

		static void OnMenuColorsChanged(EventArgs e)
		{
			if(MenuColorsChanged != null)
			{
				MenuColorsChanged(null, e);
			}
		}

        /// <summary>
        /// 
        /// </summary>
        static Office2003Colors()
        {
            //UpdateMenuColors();    
        }

		/// <summary>
		/// Fired when the colors have changed either because of change in system colors or
		/// when a custom color is specified using one of the properties.
		/// </summary>
		/// <remarks>
		/// Take a look at the class reference for this class for information on how to notify
		/// this class regarding system color changes.
		/// </remarks>
		public static event EventHandler MenuColorsChanged;

		/// <summary>
		/// Call this method to indicate that the system colors have changed.
		/// </summary>
		/// <param name="updateColorsNow">Indicates whether to update colors immediately or later with a call to <see cref="UpdateMenuColors"/>.</param>
		/// <remarks>
		/// Follow the same system color change notification pattern as explained in the <see cref="MenuColors"/> class reference.
		/// </remarks>
		public static void SysColorsChanged(bool updateColorsNow)
		{
			if(updateColorsNow)
				UpdateMenuColors();
			else
				needToUpdateColors = true;
		}
		#endregion COLOR_INIT

		#region PROPERTIES
		/// <summary>
		/// Gets / sets the selected color for a menu item in a drop-down menu.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color SelColor
		{
			get
			{
				if(customSelColor == Color.Empty)
					return selColor;
				else
					return customSelColor;
			}
			set
			{
				if(customSelColor != value)
				{
					customSelColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets / sets the pressed-selected color for a menu item in a toolbar.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color PressedSelColor
		{
			get
			{
				if(customPressedSelColor == Color.Empty)
					return pressedSelColor;
				else
					return customPressedSelColor;
			}
			set
			{
				if(customPressedSelColor != value)
				{
					customPressedSelColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets / sets the background color of a selected check box in the drop-down menu margin or a checked item in the toolbar.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color CheckedSelColor
		{
			get
			{
				if(customCheckedSelColor == Color.Empty)
					return checkedSelColor;
				else
					return customCheckedSelColor;
			}
			set
			{
				if(customCheckedSelColor != value)
				{
					customCheckedSelColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets / sets the background color of a check box in the drop-down menu margin or a checked item in the toolbar.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color CheckedColor
		{
			get
			{
				if(customCheckedColor == Color.Empty)
					return checkedColor;
				else
					return customCheckedColor;
			}
			set
			{
				if(customCheckedColor != value)
				{
					customCheckedColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets / sets the border color of a menu item selection in the drop-down menus and toolbars.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color SelBorderColor
		{
			get
			{
				if(customSelBorderColor == Color.Empty)
					return selBorderColor;
				else
					return customSelBorderColor;
			}
			set
			{
				if(customSelBorderColor != value)
				{
					customSelBorderColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets / sets the border color of a drop-down menu.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color DropdownBorderColor
		{
			get
			{
				if(customDropDownBorderColor == Color.Empty)
					return dropDownBorderColor;
				else
					return customDropDownBorderColor;
			}
			set
			{
				if(customDropDownBorderColor != value)
				{
					customDropDownBorderColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets / sets the left-gradient color of the drop-down menu margin.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color MenuMarginColorLight
		{
			get
			{
				if(customMenuMarginColorLeft == Color.Empty)
					return menuMarginColorLeft;
				else
					return customMenuMarginColorLeft;
			}
			set
			{
				if(customMenuMarginColorLeft != value)
				{
					customMenuMarginColorLeft = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets / sets the right-gradient color of the drop-down menu margin.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color MenuMarginColorDark
		{
			get
			{
				if(customMenuMarginColorRight == Color.Empty)
					return menuMarginColorRight;
				else
					return customMenuMarginColorRight;
			}
			set
			{
				if(customMenuMarginColorRight != value)
				{
					customMenuMarginColorRight = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets / sets the left-gradient color of the drop-down menu margin of the expanded menu items.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color MenuExpandedItemsMarginColorLeft
		{
			get
			{
				if(customMenuExpandedItemsMarginColorLeft == Color.Empty)
					return menuExpandedItemsMarginColorLeft;
				else
					return customMenuExpandedItemsMarginColorLeft;
			}
			set
			{
				if(customMenuExpandedItemsMarginColorLeft != value)
				{
					customMenuExpandedItemsMarginColorLeft = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets / sets the right-gradient color of the drop-down menu margin of the expanded menu items.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color MenuExpandedItemsMarginColorRight
		{
			get
			{
				if(customMenuExpandedItemsMarginColorRight == Color.Empty)
					return menuExpandedItemsMarginColorRight;
				else
					return customMenuExpandedItemsMarginColorRight;
			}
			set
			{
				if(customMenuExpandedItemsMarginColorRight != value)
				{
					customMenuExpandedItemsMarginColorRight = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the right-gradient color of docked bars.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color DockBarColorLight
		{
			get
			{
				if(customDockBarColorLight == Color.Empty)
					return dockBarColorLight;
				else
					return customDockBarColorLight;
			}
			set
			{
				if(customDockBarColorLight != value)
				{
					customDockBarColorLight = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the left-gradient color of docked bars.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color DockBarColorDark
		{
			get
			{
				if(customDockBarColorDark == Color.Empty)
					return dockBarColorDark;
				else
					return customDockBarColorDark;
			}
			set
			{
				if(customDockBarColorDark != value)
				{
					customDockBarColorDark = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the color of the gripper.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color ControlGripperColor
		{
			get
			{
				if(customControlGripperColor == Color.Empty)
					return controlGripperColor;
				else
					return customControlGripperColor;
			}
			set
			{
				if(customControlGripperColor != value)
				{
					customControlGripperColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the light-gradient border color of bars.
		/// </summary>
		public static Color ControlBorderColorLight
		{
			get
			{
				if(customControlBorderColorLight == Color.Empty)
					return controlBorderColorLight;
				else
					return customControlBorderColorLight;
			}
			set
			{
				if(customControlBorderColorLight != value)
				{
					customControlBorderColorLight = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the dark-gradient border color of bars.
		/// </summary>
		public static Color ControlBorderColorDark
		{
			get
			{
				if(customControlBorderColorDark == Color.Empty)
					return controlBorderColorDark;
				else
					return customControlBorderColorDark;
			}
			set
			{
				if(customControlBorderColorDark != value)
				{
					customControlBorderColorDark = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the light-gradient color of menu item for hot-tracking.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color MenuItemHotColorLight
		{
			get
			{
				if(customMenuItemHotColorLight == Color.Empty)
					return menuItemHotColorLight;
				else
					return customMenuItemHotColorLight;
			}
			set
			{
				if(customMenuItemHotColorLight != value)
				{
					customMenuItemHotColorLight = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the dark-gradient color of menu item for hot-tracking.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color MenuItemHotColorDark
		{
			get
			{
				if(customMenuItemHotColorDark == Color.Empty)
					return menuItemHotColorDark;
				else
					return customMenuItemHotColorDark;
			}
			set
			{
				if(customMenuItemHotColorDark != value)
				{
					customMenuItemHotColorDark = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the light-gradient color of quick customize button when it is pressed.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color MenuItemPressedColorLight
		{
			get
			{
				if(customMenuItemPressedColorLight == Color.Empty)
					return menuItemPressedColorLight;
				else
					return customMenuItemPressedColorLight;
			}
			set
			{
				if(customMenuItemPressedColorLight != value)
				{
					customMenuItemPressedColorLight = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the dark-gradient color of quick customize button when it is pressed.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color MenuItemPressedColorDark
		{
			get
			{
				if(customMenuItemPressedColorDark == Color.Empty)
					return menuItemPressedColorDark;
				else
					return customMenuItemPressedColorDark;
			}
			set
			{
				if(customMenuItemPressedColorDark != value)
				{
					customMenuItemPressedColorDark = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the caption background color of floating bars.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color FloatingCommandBarCaptionColor
		{
			get
			{
				if(customFloatingCommandBarCaptionColor == Color.Empty)
					return floatingCommandBarCaptionColor;
				else
					return customFloatingCommandBarCaptionColor;
			}
			set
			{
				if(customFloatingCommandBarCaptionColor != value)
				{
					customFloatingCommandBarCaptionColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for the floating command bar item which is pressed.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color FloatingCommandBarItemPressedColor
		{
			get
			{
				if(customFloatingCommandBarItemPressedColor == Color.Empty)
					return floatingCommandBarItemPressedColor;
				else
					return customFloatingCommandBarItemPressedColor;
			}
			set
			{
				if(customFloatingCommandBarItemPressedColor != value)
				{
					floatingCommandBarItemPressedColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the light-gradient color of quick customize dropdown button.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color CommandBarDropDownColorLight
		{
			get
			{
				if(customCommandBarDropDownColorLight == Color.Empty)
					return commandBarDropDownColorLight;
				else
					return customCommandBarDropDownColorLight;
			}
			set
			{
				if(customCommandBarDropDownColorLight != value)
				{
					customCommandBarDropDownColorLight = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the dark-gradient color of quick customize dropdown button.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color CommandBarDropDownColorDark
		{
			get
			{
				if(customCommandBarDropDownColorDark == Color.Empty)
					return commandBarDropDownColorDark;
				else
					return customCommandBarDropDownColorDark;
			}
			set
			{
				if(customCommandBarDropDownColorDark != value)
				{
					customCommandBarDropDownColorDark = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the light-gradient highlight color of groupBarItem.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color GroupBarHighlightColorLight
		{
			get
			{
				if(customGroupBarHighlightColorLight == Color.Empty)
					return groupBarHighlightColorLight;
				else
					return customGroupBarHighlightColorLight;
			}
			set
			{
				if(customGroupBarHighlightColorLight != value)
				{
					customGroupBarHighlightColorLight = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the dark-gradient highlight color of groupBarItem.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color GroupBarHighlightColorDark
		{
			get
			{
				if(customGroupBarHighlightColorDark == Color.Empty)
					return groupBarHighlightColorDark;
				else
					return customGroupBarHighlightColorDark;
			}
			set
			{
				if(customGroupBarHighlightColorDark != value)
				{
					customGroupBarHighlightColorDark = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the light-gradient color of selected groupBarItem.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color GroupBarSelectedColorLight
		{
			get
			{
				if(customGroupBarSelectedColorLight == Color.Empty)
					return groupBarSelectedColorLight;
				else
					return customGroupBarSelectedColorLight;
			}
			set
			{
				if(customGroupBarSelectedColorLight != value)
				{
					customGroupBarSelectedColorLight = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the dark-gradient color of selected groupBarItem.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color GroupBarSelectedColorDark
		{
			get
			{
				if(customGroupBarSelectedColorDark == Color.Empty)
					return groupBarSelectedColorDark;
				else
					return customGroupBarSelectedColorDark;
			}
			set
			{
				if(customGroupBarSelectedColorDark != value)
				{
					customGroupBarSelectedColorDark = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the light-gradient highlight color of selected groupBarItem.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color GroupBarSelectedHighlightColorLight
		{
			get
			{
				if(customGroupBarSelectedHighlightColorLight == Color.Empty)
					return groupBarSelectedHighlightColorLight;
				else
					return customGroupBarSelectedHighlightColorLight;
			}
			set
			{
				if(customGroupBarSelectedHighlightColorLight != value)
				{
					customGroupBarSelectedHighlightColorLight = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the dark-gradient highlight color of selected groupBarItem.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color GroupBarSelectedHighlightColorDark
		{
			get
			{
				if(customGroupBarSelectedHighlightColorDark == Color.Empty)
					return groupBarSelectedHighlightColorDark;
				else
					return customGroupBarSelectedHighlightColorDark;
			}
			set
			{
				if(customGroupBarSelectedHighlightColorDark != value)
				{
					customGroupBarSelectedHighlightColorDark = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the light-gradient color of groupBar header.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color GroupBarHeaderColorLight
		{
			get
			{
				if(customGroupBarHeaderColorLight == Color.Empty)
					return groupBarHeaderColorLight;
				else
					return customGroupBarHeaderColorLight;
			}
			set
			{
				if(customGroupBarHeaderColorLight != value)
				{
					customGroupBarHeaderColorLight = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the dark-gradient color of groupBar header.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color GroupBarHeaderColorDark
		{
			get
			{
				if(customGroupBarHeaderColorDark == Color.Empty)
					return groupBarHeaderColorDark;
				else
					return customGroupBarHeaderColorDark;
			}
			set
			{
				if(customGroupBarHeaderColorDark != value)
				{
					customGroupBarHeaderColorDark = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets / sets the color of the text in a GroupBar item.
		/// </summary>
        public static Color GroupBarItemTextColor
		{
			get
			{
				if(customGroupBarItemTextColor == Color.Empty)
					return groupBarItemTextColor;
				else
					return customGroupBarItemTextColor;
			}
			set
			{
				if(customGroupBarItemTextColor != value)
				{
					customGroupBarItemTextColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets / sets the highlight color to be used for the selected text of the GroupBar item.
		/// </summary>
        public static Color GroupBarItemTextSelectedHighlightColor
		{
			get
			{
				if(customGroupBarItemTextSelectedHighlightColor == Color.Empty)
					return groupBarItemTextSelectedHighlightColor;
				else
					return customGroupBarItemTextSelectedHighlightColor;
			}
			set
			{
				if(customGroupBarItemTextSelectedHighlightColor != value)
				{
					customGroupBarItemTextSelectedHighlightColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets / sets the color of the separator line between the bar items.
		/// </summary>
		/// <value>The default value is derived from a system color.</value>
		/// <remarks>
		/// Setting this property to Color.Empty will actually reset the previous set color and 
		/// make this property return the default color the next time it is queried.
		/// </remarks>
		public static Color SeparatorColor
		{
			get
			{
				if(customSeparatorColor == Color.Empty)
					return separatorColor;
				else
					return customSeparatorColor;
			}
			set
			{
				if(customSeparatorColor != value)
				{
					customSeparatorColor = value;
					OnMenuColorsChanged(EventArgs.Empty);
				}
			}
		}

		#endregion PROPERTIES
	}

    public class WindowsXPThemeColors
    {
        #region Class members
        private static Color m_tabControlAdvTabPanelBackGroundColor = Color.Empty;
        private static Color m_tabControlAdvActiveTopTabColor = Color.Empty;
        private static Color m_tabControlAdvActiveBottomTabColor = Color.Empty;
        private static Color m_tabControlAdvInactiveTopTabColor = Color.Empty;
        private static Color m_tabControlAdvInactiveBottomTabColor = Color.Empty;
        private static Color m_tabControlAdvHighLightedTopTabColor = Color.Empty;
        private static Color m_tabControlAdvHighLightedBottomTabColor = Color.Empty;
        private static Color m_tabControlAdvActiveBorderColor = Color.Empty;
        private static Color m_tabControlAdvInactiveBorderColor = Color.Empty;
        private static Color m_tabControlAdvLightBorderColor = Color.Empty;
        #endregion

        #region Class initialize
        static WindowsXPThemeColors()
        {
            UpdateColors();
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Used for drawing the tabControlAdv tabPanel background.
        /// </summary>
        public static Color TabControlAdvTabPanelBackGroundColor
		{
			get
			{
                return m_tabControlAdvTabPanelBackGroundColor;
			}
			set
			{
                if( m_tabControlAdvTabPanelBackGroundColor != value )
				{
                    m_tabControlAdvTabPanelBackGroundColor = value;
				}
			}
		}
        /// <summary>
        /// Used for drawing the active tabPage.
        /// </summary>
        public static Color TabControlAdvActiveTopTabColor
		{
			get
			{
                return m_tabControlAdvActiveTopTabColor;
			}
			set
			{
                if( m_tabControlAdvActiveTopTabColor != value )
				{
                    m_tabControlAdvActiveTopTabColor = value;
				}
			}
		}
        /// <summary>
        /// Used for drawing the active tabPage.
        /// </summary>
        public static Color TabControlAdvActiveBottomTabColor
        {
            get
            {
                return m_tabControlAdvActiveBottomTabColor;
            }
            set
            {
                if( m_tabControlAdvActiveBottomTabColor != value )
                {
                    m_tabControlAdvActiveBottomTabColor = value;
                }
            }
        }
        /// <summary>
        /// Used for drawing the inactive tabPage.
        /// </summary>
        public static Color TabControlAdvInactiveTopTabColor
		{
			get
			{
                return m_tabControlAdvInactiveTopTabColor;
			}
			set
			{
                if( m_tabControlAdvInactiveTopTabColor != value )
				{
                    m_tabControlAdvInactiveTopTabColor = value;
				}
			}
		}
        /// <summary>
        /// Used for drawing the inactive tabPage.
        /// </summary>
        public static Color TabControlAdvInactiveBottomTabColor
        {
            get
            {
                return m_tabControlAdvInactiveBottomTabColor;
            }
            set
            {
                if( m_tabControlAdvInactiveBottomTabColor != value )
                {
                    m_tabControlAdvInactiveBottomTabColor = value;
                }
            }
        }
        /// <summary>
        /// Used for drawing the highlighted tabPage.
        /// </summary>
        public static Color TabControlAdvHighLightedTopTabColor
        {
            get
            {
                return m_tabControlAdvHighLightedTopTabColor;
            }
            set
            {
                if( m_tabControlAdvHighLightedTopTabColor != value )
                {
                    m_tabControlAdvHighLightedTopTabColor = value;
                }
            }
        }
        /// <summary>
        /// Used for drawing the highlighted tabPage.
        /// </summary>
        public static Color TabControlAdvHighLightedBottomTabColor
        {
            get
            {
                return m_tabControlAdvHighLightedBottomTabColor;
            }
            set
            {
                if( m_tabControlAdvHighLightedBottomTabColor != value )
                {
                    m_tabControlAdvHighLightedBottomTabColor = value;
                }
            }
        }
        /// <summary>
        /// Used for drawing the borders of tabPages.
        /// </summary>
        public static Color TabControlAdvActiveBorderColor
        {
            get
            {
                return m_tabControlAdvActiveBorderColor;
            }
            set
            {
                if( m_tabControlAdvActiveBorderColor != value )
                {
                    m_tabControlAdvActiveBorderColor = value;
                }
            }
        }
        /// <summary>
        /// Used for drawing the borders of tabPages.
        /// </summary>
        public static Color TabControlAdvInactiveBorderColor
        {
            get
            {
                return m_tabControlAdvInactiveBorderColor;
            }
            set
            {
                if( m_tabControlAdvInactiveBorderColor != value )
                {
                    m_tabControlAdvInactiveBorderColor = value;
                }
            }
        }
        /// <summary>
        /// Used for drawing the borders of tabPages.
        /// </summary>
        public static Color TabControlAdvLightBorderColor
        {
            get
            {
                return m_tabControlAdvLightBorderColor;
            }
            set
            {
                if( m_tabControlAdvLightBorderColor != value )
                {
                    m_tabControlAdvLightBorderColor = value;
                }
            }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Initializes colors based on Windows XP color scheme.
        /// </summary>
        public static void UpdateColors()
        {
            if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed )
            {
                if( XPThemes.IsDefaultBlueThemeOn )
                {
                    m_tabControlAdvTabPanelBackGroundColor = Color.FromArgb( 232, 232, 219 );
                    m_tabControlAdvActiveTopTabColor = Color.FromArgb( 251, 252, 253 );
                    m_tabControlAdvActiveBottomTabColor = Color.FromArgb( 193, 210, 238 );
                    m_tabControlAdvInactiveTopTabColor = Color.FromArgb( 253, 253, 252 );
                    m_tabControlAdvInactiveBottomTabColor = Color.FromArgb( 238, 236, 221 );
                    m_tabControlAdvHighLightedTopTabColor = Color.FromArgb( 220, 226, 231 );
                    m_tabControlAdvHighLightedBottomTabColor = Color.FromArgb( 162, 187, 226 );
                    m_tabControlAdvActiveBorderColor = Color.FromArgb( 152, 181, 226 );   
                    m_tabControlAdvInactiveBorderColor = Color.FromArgb( 172, 168, 153 );
                    m_tabControlAdvLightBorderColor = Color.FromArgb( 225, 233, 246 ); 
                }
                else if( XPThemes.IsSilverThemeOn )
                {
                    m_tabControlAdvTabPanelBackGroundColor = Color.FromArgb( 223, 223, 234 );
                    m_tabControlAdvActiveTopTabColor = Color.FromArgb( 250, 251, 247 );
                    m_tabControlAdvActiveBottomTabColor = Color.FromArgb( 225, 226, 236 );
                    m_tabControlAdvInactiveTopTabColor = Color.FromArgb( 253, 253, 252 );
                    m_tabControlAdvInactiveBottomTabColor = Color.FromArgb( 244, 244, 248 );
                    m_tabControlAdvHighLightedTopTabColor = Color.FromArgb( 248, 248, 253 );
                    m_tabControlAdvHighLightedBottomTabColor = Color.FromArgb( 255, 198, 122 );
                    m_tabControlAdvActiveBorderColor = Color.FromArgb( 147, 145, 176 );
                    m_tabControlAdvInactiveBorderColor = Color.FromArgb( 172, 168, 153 ); 
                    m_tabControlAdvLightBorderColor = Color.FromArgb( 249, 249, 255 );
                }
                else if( XPThemes.IsOliveGreenThemeOn )
                {
                    m_tabControlAdvTabPanelBackGroundColor = Color.FromArgb( 233, 232, 219 );
                    m_tabControlAdvActiveTopTabColor = Color.FromArgb( 246, 246, 252 );
                    m_tabControlAdvActiveBottomTabColor = Color.FromArgb( 182, 198, 141 );
                    m_tabControlAdvInactiveTopTabColor = Color.FromArgb( 254, 254, 254 );
                    m_tabControlAdvInactiveBottomTabColor = Color.FromArgb( 238, 236, 221 );
                    m_tabControlAdvHighLightedTopTabColor = Color.FromArgb( 248, 248, 243 );
                    m_tabControlAdvHighLightedBottomTabColor = Color.FromArgb( 175, 175, 142 );
                    m_tabControlAdvActiveBorderColor = Color.FromArgb( 147, 160, 112 );
                    m_tabControlAdvInactiveBorderColor = Color.FromArgb( 157, 157, 161 );
                    m_tabControlAdvLightBorderColor = Color.FromArgb( 227, 233, 212 );
                }
            }
            else
            {
                m_tabControlAdvTabPanelBackGroundColor = Color.FromArgb( 212, 212, 212 );
                m_tabControlAdvActiveTopTabColor = Color.FromArgb( 251, 251, 251 );
                m_tabControlAdvActiveBottomTabColor = Color.FromArgb( 192, 192, 192 );
                m_tabControlAdvInactiveTopTabColor = Color.FromArgb( 192, 192, 192 );
                m_tabControlAdvInactiveBottomTabColor = Color.FromArgb( 192, 192, 192 );
                m_tabControlAdvHighLightedTopTabColor = Color.White;
                m_tabControlAdvHighLightedBottomTabColor = Color.White;
                m_tabControlAdvActiveBorderColor = Color.FromArgb( 128, 128, 128 ); 
                m_tabControlAdvInactiveBorderColor = Color.FromArgb( 128, 128, 128 );
                m_tabControlAdvLightBorderColor = Color.FromArgb( 217, 217, 217 );
            }
        }
        #endregion
    }
	
	/// <summary>
	/// Provides colors for Office2007 visual style.
	/// </summary>
	public class Office2007Colors:
		ICloneable
	{
		#region Class Static Members
		/// <summary>
		/// Colors for blue colorscheme of the Office2007 visual style.
		/// </summary>
        private static WeakReference s_blueColors = null;
		/// <summary>
		/// Colors for silver colorscheme of the Office2007 visual style.
		/// </summary>
        private static WeakReference s_silverColors = null;
		/// <summary>
		/// Colors for black colorscheme of the Office2007 visual style.
		/// </summary>
        private static WeakReference s_blackColors = null;
		/// <summary>
		/// 
		/// </summary>
        private static WeakReference s_managedColors = null;
		/// <summary>
		/// Default colorscheme for office2007 visual style.
		/// </summary>
		private static Office2007Theme s_defaultTheme = Office2007Theme.Blue;
		/// <summary>
		/// Base color for managed scheme.
		/// </summary>
		private static Color s_managedBaseColor = Color.Empty;
		#endregion

		#region Class Static Properties
		/// <summary>
		/// Gets or sets default colors for Office2007 visual style.
		/// </summary>
		public static Office2007Colors Default
		{
			get
			{
				return GetColorTable( s_defaultTheme );
			}
		}
		/// <summary>
		/// Gets or sets default colorscheme for office2007 visual style.
		/// </summary>
		public static Office2007Theme DefaultTheme
		{
			get
			{
				return s_defaultTheme;
			}
			set
			{
				if( value != s_defaultTheme )
				{
					s_defaultTheme = value;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal static Office2007Colors ManagedColors
		{
            get
            {
                if (s_managedColors.IsAlive)
                    return s_managedColors.Target as Office2007Colors;
                else
                {
                    Office2007Colors colors = new Office2007BlueColors();
                    s_managedColors = new WeakReference(colors);
                    return colors;
                }
            }
		}
		/// <summary>
		/// 
		/// </summary>
		internal static Color ManagedBaseColor
		{
			get
			{
				return s_managedBaseColor;
			}
		}

		#endregion

		#region Class static events

		/// <summary>
		/// Arguments class for <see cref="Office2007Colors.ManagedColorsApplied"/> event.
		/// </summary>
		public class ManagedColorsAppliedEventArgs:
			EventArgs
		{
			/// <summary>
			/// Initializes <see cref="ManagedColorsAppliedEventArgs"/> instance.
			/// </summary>
			/// <param name="form">Container form.</param>
            /// <param name="baseColor">Base color for the managed theme.</param>
			public ManagedColorsAppliedEventArgs( Form form, Color baseColor )
			{
				this.Form = form;
				this.BaseColor = baseColor;				
			}

			/// <summary>
			/// Container form.
			/// </summary>
			public Form Form;

			/// <summary>
            /// Base color for the managed theme.
			/// </summary>
			public Color BaseColor;
		}

		public delegate void ManagedColorsAppliedEventHandler( ManagedColorsAppliedEventArgs args );

		public static event ManagedColorsAppliedEventHandler ManagedColorsApplied;

		#endregion

		#region Class Static Public Methods
		/// <summary>
		/// Gets color table for Office2007 visual style.
		/// </summary>
		public static Office2007Colors GetColorTable( Office2007Theme theme )
		{
			Office2007Colors colorTable = null;

			switch( theme )
			{
				case Office2007Theme.Black:
				{
                    if ( s_blackColors.IsAlive)
                        colorTable = s_blackColors.Target as Office2007Colors;
                    else
                    {
                        colorTable = new Office2007BlackColors();
                        s_blackColors = new WeakReference(colorTable);
                    }
					break;
				}
				case Office2007Theme.Silver :
				{
                    if (s_silverColors.IsAlive)
                        colorTable = s_silverColors.Target as Office2007Colors;
                    else
                    {
                        colorTable = new Office2007SilverColors();
                        s_silverColors = new WeakReference(colorTable);
                    } 
                    break;
				}
				case Office2007Theme.Blue:
				{
                    if (s_blueColors.IsAlive)
                        colorTable = s_blueColors.Target as Office2007Colors;
                    else
                    {
                        colorTable = new Office2007BlueColors();
                        s_blueColors = new WeakReference(colorTable);
                    } 
                    break;
				}
				case Office2007Theme.Managed:
				{
                    colorTable = ManagedColors;
					break;
				}
				default:
				{
					throw new ArgumentException( "Unknown theme." );
				}
			}
			return colorTable;
		}

		/// <summary>
		/// Applies colors for managed scheme.
		/// </summary>
		/// <param name="form">Container form.</param>
		/// <param name="baseColor">Base color for the managed theme.</param>
		public static void ApplyManagedColors( Form form, Color baseColor )
		{
			s_managedBaseColor = baseColor;

			ManagedColors.UpdateColors( baseColor );
			OnManagedColorApplied( form, baseColor );

			if( form.IsHandleCreated )
			{
				NativeMethods.RedrawWindow( form.Handle, IntPtr.Zero, IntPtr.Zero, NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE );

				form.Invalidate( true );
                form.Update();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="form"></param>
		/// <param name="scheme"></param>
		public static void ApplyManagedScheme( Form form, Office2007Theme scheme )
		{
			s_managedBaseColor = Color.Empty;

			ManagedColors.UpdateScheme( scheme );

			if( form.IsHandleCreated )
			{
				NativeMethods.RedrawWindow( form.Handle, IntPtr.Zero, IntPtr.Zero, NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE );

				form.Invalidate( true );
			}
		}

		protected static void OnManagedColorApplied( Form form, Color baseColor )
		{
			if( Office2007Colors.ManagedColorsApplied != null )
			{
				Office2007Colors.ManagedColorsApplied( new ManagedColorsAppliedEventArgs( form, baseColor ) );
			}
		}

		#endregion

		#region Class Initialize/Finalize Methods
		static Office2007Colors()
		{
            s_managedColors = new WeakReference(new Office2007BlueColors());
            s_blueColors = new WeakReference(new Office2007BlueColors());
            s_silverColors = new WeakReference(new Office2007SilverColors());
            s_blackColors = new WeakReference(new Office2007BlackColors());
		}

		protected Office2007Colors()
		{
			// Initialize colors
			InitializeColors();
		}
		#endregion

		#region Class Members
        
        // Tab item colors
		protected Color m_TabItemBorderColor = Color.Empty;
		protected Color m_TabItemInnerBorderColor = Color.Empty;
		protected Color m_TabItemOuterBorderColor = Color.Empty;
		protected Color m_TabItemTextColor = Color.Empty;
		protected Color m_TabItemActiveBottomColor = Color.Empty;
		protected Color m_TabItemTopGradientColor = Color.Empty;
		protected Color m_TabItemInActiveBottomColor = Color.Empty;
		protected Color m_TabItemMiddleLineColor = Color.Empty;
		protected Color m_TabPanelColor = Color.Empty;
		protected Color m_TabPanelBorderColor = Color.Empty;
		protected Color m_TabPanelBackColor = Color.Empty;

        //DataTimePickerAdv colors
        protected Color m_DataTimePickerBorderColor = Color.Empty;
		protected Color m_DataTimePickerHighLightedBorderColor = Color.Empty;
        protected Color m_DataTimePickerSelectedBorderColor = Color.Empty;

        protected Color m_DataTimePickerDropDownArrowColor = Color.Empty;
        protected Color m_DataTimePickerDropDownLightColor = Color.Empty;
        protected Color m_DataTimePickerDropDownDarkColor = Color.Empty;
        protected Color m_DataTimePickerDropDownHighLightLightColor = Color.Empty;
        protected Color m_DataTimePickerDropDownHighLightDarkColor = Color.Empty;
        protected Color m_DataTimePickerDropDownSelectedLightColor = Color.Empty;
        protected Color m_DataTimePickerDropDownSelectedDarkColor = Color.Empty;

        protected Color m_DataTimePickerCheckBoxNormalColor = Color.Empty;
        protected Color m_DataTimePickerCheckBoxSelectedColor = Color.Empty;
        protected Color m_DataTimePickerCheckBoxBorderPushedColor = Color.Empty;
        protected Color m_DataTimePickerCheckBoxBorderNormalColor = Color.Empty;
        protected Color m_DataTimePickerCheckBoxInnerRectBorderNormalColor = Color.Empty;
        protected Color m_DataTimePickerCheckBoxInnerRectBorderSelectedColor = Color.Empty;
        protected Color m_DataTimePickerCheckBoxInnerRectBorderPushedColor = Color.Empty;
        protected Color m_DataTimePickerCheckBoxInnerRectFillNormalColor = Color.Empty;
        protected Color m_DataTimePickerCheckBoxInnerRectFillSelectedColor = Color.Empty;
        protected Color m_DataTimePickerCheckBoxInnerRectFillPushedColor = Color.Empty;

        protected Color m_DataTimePickerHighLightedForeColor = Color.Empty;

        //MonthCalendarAdv colors
        protected Color m_MonthCalendarHeaderStartColor = Color.Empty;
        protected Color m_MonthCalendarHeaderEndColor = Color.Empty;
        protected Color m_MonthCalendarForeColor = Color.Empty;
        
        // GroupBar colors
		protected Color m_GroupBarBorderColor = Color.Empty;
		protected Color m_GroupBarItemColorDark = Color.Empty;
		protected Color m_GroupBarItemColorLight = Color.Empty;
		protected Color m_GroupBarHighlightColorLight = Color.Empty;
		protected Color m_GroupBarHighlightColorDark = Color.Empty;
		protected Color m_GroupBarSelectedColorLight = Color.Empty;
		protected Color m_GroupBarSelectedColorDark = Color.Empty;
        protected Color m_GroupBarSelectedTopColorLight = Color.Empty;
        protected Color m_GroupBarSelectedTopColorDark = Color.Empty;
		protected Color m_GroupBarSelectedHighlightColorLight = Color.Empty;
		protected Color m_GroupBarSelectedHighlightColorDark = Color.Empty;
		protected Color m_GroupBarHeaderColorLight = Color.Empty;
		protected Color m_GroupBarHeaderColorDark = Color.Empty;
		protected Color m_GroupBarItemTextColor = Color.Empty;
		protected Color m_GroupBarHeaderTextColor = Color.Empty;
		protected Color m_GroupBarSplitterColorDark = Color.Empty;
		protected Color m_GroupBarSplitterColorLight = Color.Empty;
		protected Color m_GroupBarClientAreaBackground = Color.Empty;

        //XpTaskPane colors
        protected Color m_XPTaskPaneInternalBorderColor = Color.Empty;
        protected Color m_XPTaskPaneBorderColor = Color.Empty;
        protected Color m_XPTaskPageBackColor = Color.Empty;

		// menu colors
		protected Color m_MenuBorderColor = Color.Empty;
		protected Color m_MenuSeparatorColor = Color.Empty;
		protected Color m_MenuColumnColor = Color.Empty;
		protected Color m_MenuColumnSeparatorColor = Color.Empty;
		protected Color m_MenuBackground = Color.Empty;
		protected Color m_MenuItemBorderColor = Color.Empty;
		protected Color m_MenuItemDarkColor = Color.Empty;
		protected Color m_MenuItemLightColor = Color.Empty;
		protected Color m_MenuItemArrowLightColor = Color.Empty;
		protected Color m_MenuItemArrowDarkColor = Color.Empty;
		protected Color m_MenuCheckedColor = Color.Empty;
		protected Color m_MenuCheckedFillColor = Color.Empty;
		protected Color m_MenuCheckedBorderColor = Color.Empty;
		protected Color m_MenuTextBoxBorderColor = Color.Empty;
		protected Color m_MenuTextBoxBackColor = Color.Empty;
		protected Color m_MenuComboButtonPushed1Color = Color.Empty;
		protected Color m_MenuComboButtonPushed2Color = Color.Empty;
		protected Color m_MenuComboButtonPushed3Color = Color.Empty;
		protected Color m_MenuComboButtonPushed4Color = Color.Empty;
		protected Color m_MenuComboButtonHighlightLightColor = Color.Empty;
		protected Color m_MenuComboButtonHighlightDarkColor = Color.Empty;
		protected Color m_MenuComboButtonArrowColor = Color.Empty;

		// CommandBar colors
		protected Color m_DropDownLightColor = Color.Empty;
		protected Color m_DropDownDarkColor = Color.Empty;
		protected Color m_CommandBarDarkColor = Color.Empty;
		protected Color m_CommandBarLightColor = Color.Empty;
		protected Color m_CommandBarBorderColor = Color.Empty;
		protected Color m_DockBarBackColor = Color.Empty;
		protected Color m_DropDownHighlightLightColor = Color.Empty;
		protected Color m_DropDownHighlightDarkColor = Color.Empty;
		protected Color m_DropDownPressedLightColor = Color.Empty;
		protected Color m_DropDownPressedDarkColor = Color.Empty;
		
		// floating CommandBar colors
		protected Color m_FloatHighlightButtonColor = Color.Empty;
		protected Color m_FloatHighlightButtonBorderColor = Color.Empty;
		protected Color m_FloatPressButtonColor = Color.Empty;
		protected Color m_FloatPressButtonBorderColor = Color.Empty;
		protected Color m_FloatPressCloseButtonBorderColor = Color.Empty;
		protected Color m_FloatPressCloseButtonColor = Color.Empty;
		protected Color m_FloatCommandBarLightColor = Color.Empty;
		protected Color m_FloatCommandBarDarkColor = Color.Empty;
		protected Color m_FloatLightBorderColor = Color.Empty;
		protected Color m_FloatBackgroundColor = Color.Empty;
		protected Color m_FloatBorderColor = Color.Empty;
		protected Color m_FloatCaptionColor = Color.Empty;

		// BarItems colors
		protected Color m_BarItemSeparatorColor = Color.Empty;
		protected Color m_BarItemPressBorderColor = Color.Empty;
		protected Color m_BarItemHighlightBorderColor = Color.Empty;
		protected Color m_BarItemPressLightColor = Color.Empty;
		protected Color m_BarItemPressDarkColor = Color.Empty;
		protected Color m_DropDownBarItemLightColor = Color.Empty;
		protected Color m_DropDownBarItemDarkColor = Color.Empty;
		protected Color m_DropDownBarItemBorderColor = Color.Empty;
		protected Color m_BarItemCheckLightColor = Color.Empty;
		protected Color m_BarItemCheckDarkColor = Color.Empty;
		protected Color m_BarItemCheckBorderColor = Color.Empty;
		protected Color m_BarItemCheckFlashColor = Color.Empty;
		protected Color m_BarItemPressFlashColor = Color.Empty;
		protected Color m_BarItemSelectFlashColor = Color.Empty;
        protected Color m_TextBarItemBackColor = Color.Empty;
        protected Color m_TextBarItemBorderColor = Color.Empty;
        protected Color m_TextBarItemBorderHighlightColor = Color.Empty;
		
		// colors for ComboButton
		protected Color m_ComboButtonLightColor = Color.Empty;
		protected Color m_ComboButtonDarkColor = Color.Empty;
		protected Color m_ComboButtonPressLightColor = Color.Empty;
		protected Color m_ComboButtonPressDarkColor = Color.Empty;
		protected Color m_ComboButtonHighlightLightColor = Color.Empty;
		protected Color m_ComboButtonHighlightDarkColor = Color.Empty;
		protected Color m_ComboButtonBorder = Color.Empty;
		protected Color m_ComboButtonPressBorder = Color.Empty;
		protected Color m_ComboButtonHighlightBorder = Color.Empty;

		//ButtonAdvColors
		protected Color m_ButtonPressedTopColor = Color.Empty;
		protected Color m_ButtonPressedBottomColor = Color.Empty;
		protected Color m_ButtonSelectedTopColor = Color.Empty;
		protected Color m_ButtonSelectedBottomColor = Color.Empty;
		protected Color m_ButtonDisabledTopColor = Color.Empty;
		protected Color m_ButtonDisabledBottomColor = Color.Empty;
		protected Color m_ButtonPressedBorderColor = Color.Empty;
		protected Color m_ButtonSelectedBorderColor = Color.Empty;
		protected Color m_ButtonDisabledBorderColor = Color.Empty;
		protected Color m_ButtonDefaultTopColor = Color.Empty;
		protected Color m_ButtonDefaultBottomColor = Color.Empty;
		protected Color m_ButtonDefaultBorderColor = Color.Empty;
		protected Color m_ButtonDefaultInternalBorderColor = Color.Empty;
		protected Color m_ButtonPressedInternalBorderColor = Color.Empty;
		protected Color m_ButtonSelectedInternalBorderColor = Color.Empty;

		#region Obsolete
		protected Color m_BlueButtonDefaultTopColor = Color.Empty;
		protected Color m_BlueButtonDefaultBottomColor = Color.Empty;
		protected Color m_BlueButtonDefaultBorderColor = Color.Empty;
		protected Color m_BlueButtonDefaultInternalBorderColor = Color.Empty;
		protected Color m_BlueButtonPressedInternalBorderColor = Color.Empty;
		protected Color m_BlueButtonSelectedInternalBorderColor = Color.Empty;
		protected Color m_SilverButtonDefaultTopColor = Color.Empty;
		protected Color m_SilverButtonDefaultBottomColor = Color.Empty;
		protected Color m_SilverButtonDefaultBorderColor = Color.Empty;
		protected Color m_SilverButtonDefaultInternalBorderColor = Color.Empty;
		protected Color m_SilverButtonPressedInternalBorderColor = Color.Empty;
		protected Color m_SilverButtonSelectedInternalBorderColor = Color.Empty;
		protected Color m_BlackButtonDefaultTopColor = Color.Empty;
		protected Color m_BlackButtonDefaultBottomColor = Color.Empty;
		protected Color m_BlackButtonDefaultBorderColor = Color.Empty;
		protected Color m_BlackButtonDefaultInternalBorderColor = Color.Empty;
		protected Color m_BlackButtonPressedInternalBorderColor = Color.Empty;
		protected Color m_BlackButtonSelectedInternalBorderColor = Color.Empty;
		#endregion

		//NumericUpDownExt colors
        protected Color m_NumericUpDownBorderColor = Color.Empty;
        protected Color m_NumericUpDownHighLightedBorderColor = Color.Empty;
        protected Color m_NumericUpDownSelectedBorderColor = Color.Empty;
        protected Color m_NumericUpDownArrowLightColor = Color.Empty;
        protected Color m_NumericUpDownArrowDarkColor = Color.Empty;

		// TabControlAdv colors
		protected Color m_TabDefaultBorderColor = Color.Empty;
		protected Color m_TabHotLightBottomBorderLineColor = Color.Empty;
		protected Color m_TabHotLightGradientTopBeginColor = Color.Empty;
		protected Color m_TabHotLightGradientTopEndColor = Color.Empty;
		protected Color m_TabHotLightGradientBottomBeginColor = Color.Empty;
		protected Color m_TabHotLightGradientBottomEndColor = Color.Empty;
		protected Color m_TabHotLightGradientCircleColor = Color.Empty;
		protected Color m_TabSelectedGradientTopColor = Color.Empty;
		protected Color m_TabSelectedGradientBottomColor = Color.Empty;
		protected Color m_TabSelectedInnerBorderColor = Color.Empty;
		protected Color m_TabHighlightInnerBorderColor = Color.Empty;
		protected Color m_TabSelectedHotLightBorderColor = Color.Empty;
		protected Color m_TabSelectedHotLightInnerBorderColor = Color.Empty;
		protected Color m_TabForeColor = Color.Empty;
		protected Color m_ActiveTabForeColor = Color.Empty;
		protected Color m_TabBackgroundColor = Color.Empty;
        protected Color m_TabScrollArrowColor = Color.Empty;

		// DockTabControl colors
		protected Color m_DockTabForeColor = Color.Empty;
		protected Color m_DockTabBackgroundColor = Color.Empty;

        //Treeview 
        protected Color m_SelectedNodeBackground = Color.Empty;
        protected Color m_TreeNodeArrowColor = Color.Empty;
        protected Color m_TreeviewBackColor = Color.Empty;
        protected Color m_TreeViewFontColor = Color.Empty;

        //TextBoxExt
        protected Color m_ActiveTextBoxBorderColor = Color.Empty;
        protected Color m_InactiveTextBoxBorderColor = Color.Empty;
        protected Color m_ActiveTextBoxBackColor = Color.Empty;
        protected Color m_InactiveTextBoxBackColor = Color.Empty;

		// Form colors
		protected Color m_ActiveFormBorderColor = Color.Empty;
		protected Color m_InactiveFormBorderColor = Color.Empty;
		protected Color m_FormTextColor = Color.Empty;

		protected Color m_ActiveTitleGradientBegin = Color.Empty;
		protected Color m_ActiveTitleGradientEnd = Color.Empty;
		protected Color m_InactiveTitleGradientBegin = Color.Empty;
		protected Color m_InactiveTitleGradientEnd = Color.Empty;

		protected Color m_SystemButtonSelectedGradientBegin = Color.Empty;
		protected Color m_SystemButtonSelectedGradientEnd = Color.Empty;
		protected Color m_SystemButtonPressedGradientBegin = Color.Empty;
		protected Color m_SystemButtonPressedGradientEnd = Color.Empty;
		protected Color m_SystemButtonBorderSelected = Color.Empty;
		protected Color m_SystemButtonBorderPressed = Color.Empty;
		protected Color m_FormBackground = Color.Empty;

		// UpDown colors
		protected Color m_UpDownArrowStartColor = Color.Empty;
		protected Color m_UpDownArrowEndColor = Color.Empty;

		protected Color m_UpDownBorderNormalColor = Color.Empty;
		protected Color m_UpDownBackgroundNormalColor = Color.Empty;
		protected Color m_UpDownBackgroundNormalStartColor = Color.Empty;
		protected Color m_UpDownBackgroundNormalEndColor = Color.Empty;
		
		protected Color m_UpDownBorderHotColor = Color.Empty;
		protected Color m_UpDownInnerBorderHotStartColor = Color.Empty;
		protected Color m_UpDownInnerBorderHotEndColor = Color.Empty;

		protected Color m_UpDownBorderPressedColor = Color.Empty;
		protected Color m_UpDownInnerBorderPressedStartColor = Color.Empty;
		protected Color m_UpDownInnerBorderPressedEndColor = Color.Empty;

		protected Color m_UpDownBackgroundDisabledStartColor = Color.Empty;
		protected Color m_UpDownBackgroundDisabledEndColor = Color.Empty;
		protected Color m_UpDownBorderDisabledColor = Color.Empty;
		
		protected Color m_UpDownBackgroundHotTopStartColor = Color.Empty;
		protected Color m_UpDownBackgroundHotTopEndColor = Color.Empty;
		protected Color m_UpDownBackgroundHotBottomStartColor = Color.Empty;
		protected Color m_UpDownBackgroundHotBottomEndColor = Color.Empty;
		
		protected Color m_UpDownBackgroundPressedTopStartColor = Color.Empty;
		protected Color m_UpDownBackgroundPressedTopEndColor = Color.Empty;
		protected Color m_UpDownBackgroundPressedBottomStartColor = Color.Empty;
		protected Color m_UpDownBackgroundPressedBottomEndColor = Color.Empty;

        // ComboBoxAdv colors
        protected Color m_ComboBoxAdvNormalBackColor = Color.Empty;
        protected Color m_ComboBoxAdvHotBackColor = Color.Empty;
        protected Color m_ComboBoxAdvNormalBorderColor = Color.Empty;
        protected Color m_ComboBoxAdvHotBorderColor = Color.Empty;
        protected Color m_ComboBoxAdvPushedBorderColor = Color.Empty;
        protected Color m_ComboBoxAdvButtonUpperLineColor = Color.Empty;
        protected Color m_ComboBoxAdvArrowColor = Color.Empty;
        protected Color m_ComboBoxAdvLowerArrowLineColor = Color.Empty;
        protected Color m_ComboBoxAdvHotBackgroundButtonColor1 = Color.Empty;
        protected Color m_ComboBoxAdvHotBackgroundButtonColor2 = Color.Empty;
        protected Color m_ComboBoxAdvHotBackgroundButtonColor3 = Color.Empty;
        protected Color m_ComboBoxAdvHotBackgroundButtonColor4 = Color.Empty;
        protected Color m_ComboBoxAdvNormalBackgroundButtonColor1 = Color.Empty;
        protected Color m_ComboBoxAdvNormalBackgroundButtonColor2 = Color.Empty;
        protected Color m_ComboBoxAdvNormalBackgroundButtonColor3 = Color.Empty;
        protected Color m_ComboBoxAdvNormalBackgroundButtonColor4 = Color.Empty;
        protected Color m_ComboBoxAdvPushedBackgroundButtonColor1 = Color.Empty;
        protected Color m_ComboBoxAdvPushedBackgroundButtonColor2 = Color.Empty;
        protected Color m_ComboBoxAdvPushedBackgroundButtonColor3 = Color.Empty;
        protected Color m_ComboBoxAdvPushedBackgroundButtonColor4 = Color.Empty;

        // CheckBoxAdv colors
        protected Color m_CheckBoxAdvNormalBackColor = Color.Empty;
        protected Color m_CheckBoxAdvSelectedBackColor = Color.Empty;
        protected Color m_CheckBoxAdvPushedBackColor = Color.Empty;
        protected Color m_CheckBoxAdvNormalBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvSelectedBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvPushedBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvNormalInternalBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvSelectedInternalBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvPushedInternalBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvNormalInternalRectangleBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvSelectedInternalRectangleBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvPushedInternalRectangleBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvNormalInternalRectangleColor = Color.Empty;
        protected Color m_CheckBoxAdvSelectedInternalRectangleColor = Color.Empty;
        protected Color m_CheckBoxAdvPushedInternalRectangleColor = Color.Empty;
        protected Color m_CheckBoxAdvNormalTickColor = Color.Empty;
        protected Color m_CheckBoxAdvSelectedTickColor = Color.Empty;
        protected Color m_CheckBoxAdvPushedTickColor = Color.Empty;
        protected Color m_CheckBoxAdvDisabledTickColor = Color.Empty;
        protected Color m_CheckBoxAdvIndeterminateRectangleColor = Color.Empty;
        protected Color m_CheckBoxAdvDisabledBackColor = Color.Empty;
        protected Color m_CheckBoxAdvDisabledBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvDisabledInternalBorderColor = Color.Empty;

        // RadioButtonAdv colors
        protected Color m_RadioButtonAdvNormalBackColor = Color.Empty;
        protected Color m_RadioButtonAdvNormalBorderColor = Color.Empty;
        protected Color m_RadioButtonAdvNormalInternalBorderColor = Color.Empty;
        protected Color m_RadioButtonAdvSelectedBackColor = Color.Empty;
        protected Color m_RadioButtonAdvSelectedBorderColor = Color.Empty;
        protected Color m_RadioButtonAdvSelectedInternalBorderColor = Color.Empty;
        protected Color m_RadioButtonAdvPushedBackColor = Color.Empty;
        protected Color m_RadioButtonAdvPushedBorderColor = Color.Empty;
        protected Color m_RadioButtonAdvPushedInternalBorderColor = Color.Empty;
        protected Color m_RadioButtonAdvCheckMarkBorderColor = Color.Empty;
        protected Color m_RadioButtonAdvCheckMarkNormalBottomColor = Color.Empty;
        protected Color m_RadioButtonAdvCheckMarkSelectedBottomColor = Color.Empty;
        protected Color m_RadioButtonAdvCheckMarkPushedBottomColor = Color.Empty;

        // TabBarSplitterControl colors
        protected Color m_TabBarSplitterBackColor = Color.Empty;
        protected Color m_TabBarSplitterBorderColor = Color.Empty;
        protected Color m_TabBarSplitterTextColor = Color.Empty;
        protected Color m_TabBarSplitterTabStartColor = Color.Empty;
        protected Color m_TabBarSplitterTabEndColor = Color.Empty;
        protected Color m_TabBarSplitterTabBarStartColor = Color.Empty;
        protected Color m_TabBarSplitterTabBarEndColor = Color.Empty;
        protected Color m_TabBarSplitterButtonHoveredStartColor = Color.Empty;
        protected Color m_TabBarSplitterButtonHoveredEndColor = Color.Empty;
        protected Color m_TabBarSplitterButtonPushedStartColor = Color.Empty;
        protected Color m_TabBarSplitterButtonPushedEndColor = Color.Empty;
        protected Color m_TabBarSplitterSizeGripperColor = Color.Empty;
        protected Color m_TabBarSplitterSizeGripperLightColor = Color.Empty;
        protected Color m_TabBarSplitterSizeGripperDarkColor = Color.Empty;

        // XPTaskBar colors
        protected Color m_XPTaskBarBorderColor = Color.Empty;
        protected Color m_XPTaskBarBoxBackColor = Color.Empty;
        protected Color m_XPTaskBarBoxForeColor = Color.Empty;
        protected Color m_XPTaskBarBoxHeaderUpperLineColor = Color.Empty;
        protected Color m_XPTaskBarBoxHeaderLowerLineColor = Color.Empty;
        protected Color m_XPTaskBarBoxArrowColor = Color.Empty;
        protected Color m_XPTaskBarBoxActiveHighlightedItemColor = Color.Empty;
        protected Color m_XPTaskBarBoxInactiveHighlightedItemColor = Color.Empty;

		// ColorUIAdv colors
		protected Color m_ColorUIAdvBackColor = Color.Empty;
		protected Color m_ColorUIAdvTextColor = Color.Empty;
		protected Color m_ColorUIAdvItemBorderColor = Color.Empty;
		protected Color m_ColorUIAdvHighlightedBorderColor = Color.Empty;
		protected Color m_ColorUIAdvSelectedBorderColor = Color.Empty;
		protected Color m_ColorUIAdvSelectedHighlightedBorderColor = Color.Empty;
		protected Color m_ColorUIAdvGroupHeaderBackColor = Color.Empty;

        //StatusBarExt
        protected Color m_StatusBarExtTopGradient = Color.Empty;
        protected Color m_StatusBarExtBottomGradient = Color.Empty;
        protected Color m_StatusBarExtFillColor = Color.Empty;

        #endregion

        #region Class Properties

        #region TabItem Colors
        public Color TabPanelBorderColor
		{
			get
			{
				return m_TabPanelBorderColor;
			}
			set
			{
				if( m_TabPanelBorderColor != value )
				{
					m_TabPanelBorderColor = value;
				}
			}
		}

		public Color TabPanelBackColor
		{
			get
			{
				return m_TabPanelBackColor;
			}
			set
			{
				if ( m_TabPanelBackColor != value )
				{
					m_TabPanelBackColor = value;
				}
			}
		}

		public Color TabPanelColor
		{
			get
			{
				return m_TabPanelColor;
			}
			set
			{
				if( m_TabPanelColor != value )
				{
					m_TabPanelColor = value;
				}
			}
		}

		public Color TabItemBorderColor
		{
			get 
			{
				return m_TabItemBorderColor;
			}
			set
			{
				if( m_TabItemBorderColor != value )
				{
					m_TabItemBorderColor = value;
				}
			}
		}

		public Color TabItemInnerBorderColor
		{
			get
			{
				return m_TabItemInnerBorderColor;
			}
			set
			{
				if( m_TabItemInnerBorderColor != value )
				{
					m_TabItemInnerBorderColor = value;
				}
			}
		}

		public Color TabItemOuterBorderColor
		{
			get
			{
				return m_TabItemOuterBorderColor;
			}
			set
			{
				if( m_TabItemOuterBorderColor != value )
				{
					m_TabItemOuterBorderColor = value;
				}
			}
		}

		public Color TabItemTextColor
		{
			get
			{
				return m_TabItemTextColor;
			}
			set
			{
				if( m_TabItemTextColor != value )
				{
					m_TabItemTextColor = value;
				}
			}
		}

		public Color TabItemActiveBottomColor
		{
			get
			{
				return m_TabItemActiveBottomColor;
			}
			set
			{
				if( m_TabItemActiveBottomColor != value )
				{
					m_TabItemActiveBottomColor = value;
				}
			}
		}

		[Obsolete( "Use TabItemTopGradientColor property instead." )]
		public Color TopGradientColor
		{
			get
			{
				return this.TabItemTopGradientColor;
			}
			set
			{
				this.TabItemTopGradientColor = value;
			}
		}

		public Color TabItemTopGradientColor
		{
			get
			{
				return m_TabItemTopGradientColor;
			}
			set
			{
				if( m_TabItemTopGradientColor != value )
				{
					m_TabItemTopGradientColor = value;
				}
			}
		}

		public Color TabItemInActiveBottomColor
		{
			get
			{
				return m_TabItemInActiveBottomColor;
			}
			set			
			{ 
				if( m_TabItemInActiveBottomColor != value )
				{
					m_TabItemInActiveBottomColor = value;
				}
			}
		}

		public Color TabItemMiddleLineColor
		{
			get
			{
				return m_TabItemMiddleLineColor;
			}
			set
			{
				if( m_TabItemMiddleLineColor != value )
				{
					m_TabItemMiddleLineColor = value;
				}
			}
		}
        #endregion

        #region DataTimePickerAdv Colors
        /// <summary>
        /// Gets or sets border color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerBorderColor
        {
            get { return m_DataTimePickerBorderColor; }
            set
            {
                if (m_DataTimePickerBorderColor != value)
                    m_DataTimePickerBorderColor = value;
            }
        }

        /// <summary>
        /// Gets or sets highlighted border color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerHighLightedBorderColor
        {
            get { return m_DataTimePickerHighLightedBorderColor; }
            set
            {
                if (m_DataTimePickerHighLightedBorderColor != value)
                    m_DataTimePickerHighLightedBorderColor = value;
            }
        }

        /// <summary>
        /// Gets or sets selected border color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerSelectedBorderColor
        {
            get { return m_DataTimePickerSelectedBorderColor; }
            set
            {
                if (m_DataTimePickerSelectedBorderColor != value)
                    m_DataTimePickerSelectedBorderColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a dropdown arrow color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerDropDownArrowColor
        {
            get { return m_DataTimePickerDropDownArrowColor; }
            set
            {
                if (m_DataTimePickerDropDownArrowColor != value)
                    m_DataTimePickerDropDownArrowColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a dropdown gradient light color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerDropDownLightColor
        {
            get { return m_DataTimePickerDropDownLightColor; }
            set 
            {
                if (m_DataTimePickerDropDownLightColor != value)
                    m_DataTimePickerDropDownLightColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a dropdown gradient dark color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerDropDownDarkColor
        {
            get { return m_DataTimePickerDropDownDarkColor; }
            set
            {
                if (m_DataTimePickerDropDownDarkColor != value)
                    m_DataTimePickerDropDownDarkColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a dropdown highlighted gradient light color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerDropDownHighLightLightColor
        {
            get { return m_DataTimePickerDropDownHighLightLightColor; }
            set
            {
                if (m_DataTimePickerDropDownHighLightLightColor != value)
                    m_DataTimePickerDropDownHighLightLightColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a dropdown highlighted gradient dark color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerDropDownHighLightDarkColor
        {
            get { return m_DataTimePickerDropDownHighLightDarkColor; }
            set
            {
                if (m_DataTimePickerDropDownHighLightDarkColor != value)
                    m_DataTimePickerDropDownHighLightDarkColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a dropdown selected gradient light color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerDropDownSelectedLightColor
        {
            get { return m_DataTimePickerDropDownSelectedLightColor; }
            set
            {
                if (m_DataTimePickerDropDownSelectedLightColor != value)
                    m_DataTimePickerDropDownSelectedLightColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a dropdown selected gradient dark color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerDropDownSelectedDarkColor
        {
            get { return m_DataTimePickerDropDownSelectedDarkColor; }
            set
            {
                if (m_DataTimePickerDropDownSelectedDarkColor != value)
                    m_DataTimePickerDropDownSelectedDarkColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a checkbox color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerCheckBoxNormalColor
        {
            get { return m_DataTimePickerCheckBoxNormalColor; }
            set
            {
                if (m_DataTimePickerCheckBoxNormalColor != value)
                    m_DataTimePickerCheckBoxNormalColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a checkbox selected color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerCheckBoxSelectedColor
        {
            get { return m_DataTimePickerCheckBoxSelectedColor; }
            set
            {
                if (m_DataTimePickerCheckBoxSelectedColor != value)
                    m_DataTimePickerCheckBoxSelectedColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a checkbox pushed border color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerCheckBoxBorderPushedColor
        {
            get { return m_DataTimePickerCheckBoxBorderPushedColor; }
            set
            {
                if (m_DataTimePickerCheckBoxBorderPushedColor != value)
                    m_DataTimePickerCheckBoxBorderPushedColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a checkbox border color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerCheckBoxBorderNormalColor
        {
            get { return m_DataTimePickerCheckBoxBorderNormalColor; }
            set
            {
                if (m_DataTimePickerCheckBoxBorderNormalColor != value)
                    m_DataTimePickerCheckBoxBorderNormalColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a checkbox inner rectangle border color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerCheckBoxInnerRectBorderNormalColor
        {
            get { return m_DataTimePickerCheckBoxInnerRectBorderNormalColor; }
            set
            {
                if (m_DataTimePickerCheckBoxInnerRectBorderNormalColor != value)
                    m_DataTimePickerCheckBoxInnerRectBorderNormalColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a checkbox inner rectangle selected border color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerCheckBoxInnerRectBorderSelectedColor
        {
            get { return m_DataTimePickerCheckBoxInnerRectBorderSelectedColor; }
            set
            {
                if (m_DataTimePickerCheckBoxInnerRectBorderSelectedColor != value)
                    m_DataTimePickerCheckBoxInnerRectBorderSelectedColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a checkbox inner rectangle pushed border color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerCheckBoxInnerRectBorderPushedColor
        {
            get { return m_DataTimePickerCheckBoxInnerRectBorderPushedColor; }
            set
            {
                if (m_DataTimePickerCheckBoxInnerRectBorderPushedColor != value)
                    m_DataTimePickerCheckBoxInnerRectBorderPushedColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a checkbox inner rectangle filling color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerCheckBoxInnerRectFillNormalColor
        {
            get { return m_DataTimePickerCheckBoxInnerRectFillNormalColor; }
            set
            {
                if (m_DataTimePickerCheckBoxInnerRectFillNormalColor != value)
                    m_DataTimePickerCheckBoxInnerRectFillNormalColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a checkbox inner rectangle filling selected color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerCheckBoxInnerRectFillSelectedColor
        {
            get { return m_DataTimePickerCheckBoxInnerRectFillSelectedColor; }
            set
            {
                if (m_DataTimePickerCheckBoxInnerRectFillSelectedColor != value)
                    m_DataTimePickerCheckBoxInnerRectFillSelectedColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a checkbox inner rectangle filling pushed color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerCheckBoxInnerRectFillPushedColor
        {
            get { return m_DataTimePickerCheckBoxInnerRectFillPushedColor; }
            set
            {
                if (m_DataTimePickerCheckBoxInnerRectFillPushedColor != value)
                    m_DataTimePickerCheckBoxInnerRectFillPushedColor = value;
            }
        }

        /// <summary>
        /// Gets or sets highlighted fore color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerHighLightedForeColor
        {
            get { return m_DataTimePickerHighLightedForeColor; }
            set
            {
                if (m_DataTimePickerHighLightedForeColor != value)
                    m_DataTimePickerHighLightedForeColor = value;
            }
        }

        /// <summary>
        /// Gets or sets border color for <see cref="NumericUpDownExt">.
        /// </summary>
        public Color NumericUpDownBorderColor
        {
            get { return m_NumericUpDownBorderColor; }
            set
            {
                if (m_NumericUpDownBorderColor != value)
                    m_NumericUpDownBorderColor = value;
            }
        }

        /// <summary>
        /// Gets or sets highlighted border color for <see cref="NumericUpDownExt">.
        /// </summary>
        public Color NumericUpDownHighLightedBorderColor
        {
            get { return m_NumericUpDownHighLightedBorderColor; }
            set
            {
                if (m_NumericUpDownHighLightedBorderColor != value)
                    m_NumericUpDownHighLightedBorderColor = value;
            }
        }

        /// <summary>
        /// Gets or sets selected border color for <see cref="NumericUpDownExt">.
        /// </summary>
        public Color NumericUpDownSelectedBorderColor
        {
            get { return m_NumericUpDownSelectedBorderColor; }
            set
            {
                if( m_NumericUpDownSelectedBorderColor != value )
                    m_NumericUpDownSelectedBorderColor = value;
            }
        }

        /// <summary>
        /// Gets or sets dropdown arrow gradient light color for <see cref="NumericUpDownExt">.
        /// </summary>
        public Color NumericUpDownArrowLightColor
        {
            get { return m_NumericUpDownArrowLightColor; }
            set
            {
                if (m_NumericUpDownArrowLightColor != value)
                    m_NumericUpDownArrowLightColor = value;
            }
        }

        /// <summary>
        /// Gets or sets dropdown arrow gradient dark color for <see cref="NumericUpDownExt">.
        /// </summary>
        public Color NumericUpDownArrowDarkColor
        {
            get { return m_NumericUpDownArrowDarkColor; }
            set
            {
                if (m_NumericUpDownArrowDarkColor != value)
                    m_NumericUpDownArrowDarkColor = value;
            }
        }

        /// <summary>
        /// Gets or sets gradient start color for <see cref="MonthCalendarAdv">.
        /// </summary>
        public Color MonthCalendarHeaderStartColor
        {
            get { return m_MonthCalendarHeaderStartColor; }
            set
            {
                if (m_MonthCalendarHeaderStartColor != value)
                    m_MonthCalendarHeaderStartColor = value;
            }
        }

        /// <summary>
        /// Gets or sets gradient end color for <see cref="MonthCalendarAdv">.
        /// </summary>
        public Color MonthCalendarHeaderEndColor
        {
            get { return m_MonthCalendarHeaderEndColor; }
            set
            {
                if (m_MonthCalendarHeaderEndColor != value)
                    m_MonthCalendarHeaderEndColor = value;
            }
        }

        /// <summary>
        /// Gets or sets fore color for <see cref="MonthCalendarAdv">.
        /// </summary>
        public Color MonthCalendarForeColor
        {
            get { return m_MonthCalendarForeColor; }
            set
            {
                if (m_MonthCalendarForeColor != value)
                    m_MonthCalendarForeColor = value;
            }
        }

		#endregion

		#region GroupBar Colors

        /// <summary>
        /// Gets or sets border color for <see cref="GroupBar">.
        /// </summary>
		public Color GroupBarBorderColor
		{
			get
			{
				return m_GroupBarBorderColor;
			}
			set
			{
				if (m_GroupBarBorderColor != value)
				{
					m_GroupBarBorderColor = value;
				}
			}
		}

        /// <summary>
        /// Gets or sets gradient dark color for <see cref="GroupBarItem">.
        /// </summary>
		public Color GroupBarItemColorDark
		{
			get
			{
				return m_GroupBarItemColorDark;
			}
			set
			{
				if (m_GroupBarItemColorDark != value)
				{
					m_GroupBarItemColorDark = value;
				}
			}
		}

        /// <summary>
        /// Gets or sets gradient light color for <see cref="GroupBarItem">.
        /// </summary>
		public Color GroupBarItemColorLight
		{
			get
			{
				return m_GroupBarItemColorLight;
			}
			set
			{
				if (m_GroupBarItemColorLight != value)
				{
					m_GroupBarItemColorLight = value;
				}
			}
		}

        /// <summary>
        /// Gets or sets <see cref="GroupBar">'s header gradient color dark.
        /// </summary>
		public Color GroupBarHeaderColorDark
		{
			get
			{
				return m_GroupBarHeaderColorDark;
			}
			set
			{
				if (m_GroupBarHeaderColorDark != value)
				{
					m_GroupBarHeaderColorDark = value;
				}
			}
		}

        /// <summary>
        /// Gets or sets <see cref="GroupBar">'s header gradient color light.
        /// </summary>
		public Color GroupBarHeaderColorLight
		{
			get
			{
				return m_GroupBarHeaderColorLight;
			}
			set
			{
				if (m_GroupBarHeaderColorLight != value)
				{
					m_GroupBarHeaderColorLight = value;
				}
			}
		}

        /// <summary>
        /// Gets or sets gradient highlight light color for <see cref="GroupBar">.
        /// </summary>
		public Color GroupBarHighlightColorLight
		{
			get
			{
				return m_GroupBarHighlightColorLight;
			}
			set
			{
				if (m_GroupBarHighlightColorLight != value)
				{
					m_GroupBarHighlightColorLight = value;
				}
			}
		}

        /// <summary>
        /// Gets or sets gradient highlight dark color for <see cref="GroupBar">.
        /// </summary>
		public Color GroupBarHighlightColorDark
		{
			get
			{
				return m_GroupBarHighlightColorDark;
			}
			set
			{
				if (m_GroupBarHighlightColorDark != value)
				{
					m_GroupBarHighlightColorDark = value;
				}
			}
		}

        /// <summary>
        /// Gets or sets gradient selected dark color for <see cref="GroupBar">.
        /// </summary>
		public Color GroupBarSelectedColorDark
		{
			get
			{
				return m_GroupBarSelectedColorDark;
			}
			set
			{
				if (m_GroupBarSelectedColorDark != value)
				{
					m_GroupBarSelectedColorDark = value;
				}
			}
		}

        /// <summary>
        /// Gets or sets gradient selected light color for <see cref="GroupBar">.
        /// </summary>
		public Color GroupBarSelectedColorLight
		{
			get
			{
				return m_GroupBarSelectedColorLight;
			}
			set
			{
				if (m_GroupBarSelectedColorLight != value)
				{
					m_GroupBarSelectedColorLight = value;
				}
			}
		}

        /// <summary>
        /// Gets or sets gradient selected dark color for the upper part of <see cref="GroupBarItem">.
        /// </summary>
        public Color GroupBarSelectedTopColorDark
		{
			get
			{
				return m_GroupBarSelectedTopColorDark;
			}
			set
			{
                if (m_GroupBarSelectedTopColorDark != value)
				{
                    m_GroupBarSelectedTopColorDark = value;
				}
			}
		}

        /// <summary>
        /// Gets or sets gradient selected light color for the upper part of <see cref="GroupBarItem">.
        /// </summary>
		public Color GroupBarSelectedTopColorLight
		{
			get
			{
				return m_GroupBarSelectedTopColorLight;
			}
			set
			{
                if (m_GroupBarSelectedTopColorLight != value)
				{
                    m_GroupBarSelectedTopColorLight = value;
				}
			}
		}

        /// <summary>
        /// Gets or sets gradient highlighted and selected dark color for <see cref="GroupBar">.
        /// </summary>
		public Color GroupBarSelectedHighlightColorDark
		{
			get
			{
				return m_GroupBarSelectedHighlightColorDark;
			}
			set
			{
				if (m_GroupBarSelectedHighlightColorDark != value)
				{
					m_GroupBarSelectedHighlightColorDark = value;
				}
			}
		}

        /// <summary>
        /// Gets or sets gradient highlighted and selected light color for <see cref="GroupBar">.
        /// </summary>
		public Color GroupBarSelectedHighlightColorLight
		{
			get
			{
				return m_GroupBarSelectedHighlightColorLight;
			}
			set
			{
				if (m_GroupBarSelectedHighlightColorLight != value)
				{
					m_GroupBarSelectedHighlightColorLight = value;
				}
			}
		}

        /// <summary>
        /// Gets or sets text color for <see cref="GroupBarItem">.
        /// </summary>
		public Color GroupBarItemTextColor
		{
			get
			{
				return m_GroupBarItemTextColor;
			}
			set
			{
				if (m_GroupBarItemTextColor != value)
				{
					m_GroupBarItemTextColor = value;
				}
			}
		}

        /// <summary>
        /// Gets or sets <see cref="GroupBar">'s header text color.
        /// </summary>
		public Color GroupBarHeaderTextColor
		{
			get
			{
				return m_GroupBarHeaderTextColor;
			}
			set
			{
				if (m_GroupBarHeaderTextColor != value)
				{
					m_GroupBarHeaderTextColor = value;
				}
			}
		}

        /// <summary>
        /// Gets or sets gradient dark color for splitter of <see cref="GroupBar">.
        /// </summary>
		public Color GroupBarSplitterColorDark
		{
			get
			{
				return m_GroupBarSplitterColorDark;
			}
			set
			{
				if (m_GroupBarSplitterColorDark != value)
				{
					m_GroupBarSplitterColorDark = value;
				}
			}
		}

        /// <summary>
        /// Gets or sets gradient light color for splitter of <see cref="GroupBar">.
        /// </summary>
		public Color GroupBarSplitterColorLight
		{
			get
			{
				return m_GroupBarSplitterColorLight;
			}
			set
			{
				if (m_GroupBarSplitterColorLight != value)
				{
					m_GroupBarSplitterColorLight = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the client area background color of <see cref="GroupBar">.
		/// </summary>
		public Color GroupBarClientAreaBackground
		{
			get
			{
				return m_GroupBarClientAreaBackground;
			}
			set
			{
				m_GroupBarClientAreaBackground = value;
			}
		}

		#endregion

        #region XPTaskPane Colors
        /// <summary>
        /// Gets or sets the color for the internal border of <see cref="XPTaskPane">.
        /// </summary>
        public Color XPTaskPaneInternalBorderColor
        {
            get
            {
                return m_XPTaskPaneInternalBorderColor;
            }
            set
            {
                if( m_XPTaskPaneInternalBorderColor != value )
                {
                    m_XPTaskPaneInternalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for the border of <see cref="XPTaskPane">.
        /// </summary>
        public Color XPTaskPaneBorderColor
        {
            get
            {
                return m_XPTaskPaneBorderColor;
            }
            set
            {
                if( m_XPTaskPaneBorderColor != value )
                {
                    m_XPTaskPaneBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the back color for  <see cref="XPTaskPage">.
        /// </summary>
        public Color XPTaskPageBackColor
        {
            get
            {
                return m_XPTaskPageBackColor;
            }
            set
            {
                if( m_XPTaskPageBackColor != value )
                {
                    m_XPTaskPageBackColor = value;
                }
            }
        }
        #endregion

        #region Menu Colors
        /// <summary>
		/// Gets or sets the color for border of the menu.
		/// </summary>
		public Color MenuBorderColor
		{
			get
			{
				return m_MenuBorderColor;
			}
			set
			{
				if( m_MenuBorderColor != value )
				{
					m_MenuBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for separator of the menu.
		/// </summary>
		public Color MenuSeparatorColor
		{
			get
			{
				return m_MenuSeparatorColor;
			}
			set
			{
				if( m_MenuSeparatorColor != value )
				{
					m_MenuSeparatorColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the dark color for highlighted item of the menu.
		/// </summary>
		public Color MenuItemDarkColor
		{
			get
			{
				return m_MenuItemDarkColor;
			}
			set
			{
				if( m_MenuItemDarkColor != value )
				{
					m_MenuItemDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the light color for highlighted item of the menu.
		/// </summary>
		public Color MenuItemLightColor
		{
			get
			{
				return m_MenuItemLightColor;
			}
			set
			{
				if( m_MenuItemLightColor != value )
				{
					m_MenuItemLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the border color for highlighted item of the menu.
		/// </summary>
		public Color MenuItemBorderColor
		{
			get
			{
				return m_MenuItemBorderColor;
			}
			set
			{
				if( m_MenuItemBorderColor != value )
				{
					m_MenuItemBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the dark color for column of the menu.
		/// </summary>
		public Color MenuColumnColor
		{
			get
			{
				return m_MenuColumnColor;
			}
			set
			{
				if( m_MenuColumnColor != value )
				{
					m_MenuColumnColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the separator color for column of the menu.
		/// </summary>
		public Color MenuColumnSeparatorColor
		{
			get
			{
				return m_MenuColumnSeparatorColor;
			}
			set
			{
				if( m_MenuColumnSeparatorColor != value )
				{
					m_MenuColumnSeparatorColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the light color for arrow of the menu item.
		/// </summary>
		public Color MenuItemArrowLightColor
		{
			get
			{
				return m_MenuItemArrowLightColor;
			}
			set
			{
				if( m_MenuItemArrowLightColor != value )
				{
					m_MenuItemArrowLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the dark color for arrow of the menu item.
		/// </summary>
		public Color MenuItemArrowDarkColor
		{
			get
			{
				return m_MenuItemArrowDarkColor;
			}
			set
			{
				if( m_MenuItemArrowDarkColor != value )
				{
					m_MenuItemArrowDarkColor = value;
				}
			}
		}
        
		/// <summary>
		/// Gets or sets the color for check mark of the menu.
		/// </summary>
		public Color MenuCheckedColor
		{
			get
			{
				return m_MenuCheckedColor;
			}
			set
			{
				if( m_MenuCheckedColor != value )
				{
					m_MenuCheckedColor = value;
				}
			}
		}
        
		/// <summary>
		/// Gets or sets the background color for check mark of the menu.
		/// </summary>
		public Color MenuCheckedFillColor
		{
			get
			{
				return m_MenuCheckedFillColor;
			}
			set
			{
				if( m_MenuCheckedFillColor != value )
				{
					m_MenuCheckedFillColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for border check mark of the menu.
		/// </summary>
		public Color MenuCheckedBorderColor
		{
			get
			{
				return m_MenuCheckedBorderColor;
			}
			set
			{
				if( m_MenuCheckedBorderColor != value )
				{
					m_MenuCheckedBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the border color for TextBox item of the menu.
		/// </summary>
		public Color MenuTextBoxBorderColor
		{
			get
			{
				return m_MenuTextBoxBorderColor;
			}
			set
			{
				if( m_MenuTextBoxBorderColor != value )
				{
					m_MenuTextBoxBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the background color for TextBox item of the menu.
		/// </summary>
		public Color MenuTextBoxBackColor
		{
			get
			{
				return m_MenuTextBoxBackColor;
			}
			set
			{
				if( m_MenuTextBoxBackColor != value )
				{
					m_MenuTextBoxBackColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for ComboButton of the menu.
		/// </summary>
		public Color MenuComboButtonPushed1Color
		{
			get
			{
				return m_MenuComboButtonPushed1Color;
			}
			set
			{
				if( m_MenuComboButtonPushed1Color != value )
				{
					m_MenuComboButtonPushed1Color = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for ComboButton of the menu.
		/// </summary>
		public Color MenuComboButtonPushed2Color
		{
			get
			{
				return m_MenuComboButtonPushed2Color;
			}
			set
			{
				if( m_MenuComboButtonPushed2Color != value )
				{
					m_MenuComboButtonPushed2Color = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for ComboButton of the menu.
		/// </summary>
		public Color MenuComboButtonPushed3Color
		{
			get
			{
				return m_MenuComboButtonPushed3Color;
			}
			set
			{
				if( m_MenuComboButtonPushed3Color != value )
				{
					m_MenuComboButtonPushed3Color = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for ComboButton of the menu.
		/// </summary>
		public Color MenuComboButtonPushed4Color
		{
			get
			{
				return m_MenuComboButtonPushed4Color;
			}
			set
			{
				if( m_MenuComboButtonPushed4Color != value )
				{
					m_MenuComboButtonPushed4Color = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the light color for highlighted ComboButton of the menu.
		/// </summary>
		public Color MenuComboButtonHighlightLightColor
		{
			get
			{
				return m_MenuComboButtonHighlightLightColor;
			}
			set
			{
				if( m_MenuComboButtonHighlightLightColor != value )
				{
					m_MenuComboButtonHighlightLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the dark color for highlighted ComboButton of the menu.
		/// </summary>
		public Color MenuComboButtonHighlightDarkColor
		{
			get
			{
				return m_MenuComboButtonHighlightDarkColor;
			}
			set
			{
				if( m_MenuComboButtonHighlightDarkColor != value )
				{
					m_MenuComboButtonHighlightDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for arrow ComboButton of the menu.
		/// </summary>
		public Color MenuComboButtonArrowColor
		{
			get
			{
				return m_MenuComboButtonArrowColor;
			}
			set
			{
				if( m_MenuComboButtonArrowColor != value )
				{
					m_MenuComboButtonArrowColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the background color of the menu.
		/// </summary>
		public Color MenuBackground
		{
			get
			{
				return m_MenuBackground;
			}
			set
			{
				if( m_MenuBackground != value )
				{
					m_MenuBackground = value;
				}
			}
		}

		#endregion

		#region CommandBar Colors
		/// <summary>
		/// Gets or sets light color for dropdown button of the CommandBar.
		/// </summary>
		public Color DropDownLightColor
		{
			get
			{
				return m_DropDownLightColor;
			}
			set
			{
				if( m_DropDownLightColor != value )
				{
					m_DropDownLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets dark color for dropdown button of the CommandBar.
		/// </summary>
		public Color DropDownDarkColor
		{
			get
			{
				return m_DropDownDarkColor;
			}
			set
			{
				if( m_DropDownDarkColor != value )
				{
					m_DropDownDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets dark color of the CommandBar.
		/// </summary>
		public Color CommandBarDarkColor
		{
			get
			{
				return m_CommandBarDarkColor;
			}
			set
			{
				if( m_CommandBarDarkColor != value )
				{
					m_CommandBarDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color of the CommandBar.
		/// </summary>
		public Color CommandBarLightColor
		{
			get
			{
				return m_CommandBarLightColor;
			}
			set
			{
				if( m_CommandBarLightColor != value )
				{
					m_CommandBarLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets color for border of the CommandBar.
		/// </summary>
		public Color CommandBarBorderColor
		{
			get
			{
				return m_CommandBarBorderColor;
			}
			set
			{
				if( m_CommandBarBorderColor != value )
				{
					m_CommandBarBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets background color of the DockBar.
		/// </summary>
		public Color DockBarBackColor
		{
			get
			{
				return m_DockBarBackColor;
			}
			set
			{
				if( m_DockBarBackColor != value )
				{
					m_DockBarBackColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color for highlight dropdown button of the CommandBar.
		/// </summary>
		public Color DropDownHighlightLightColor
		{
			get
			{
				return m_DropDownHighlightLightColor;
			}
			set
			{
				if( m_DropDownHighlightLightColor != value )
				{
					m_DropDownHighlightLightColor = value;
				}
			}
		}
		
		/// <summary>
		/// Gets or sets dark color for highlight dropdown button of the CommandBar.
		/// </summary>
		public Color DropDownHighlightDarkColor
		{
			get
			{
				return m_DropDownHighlightDarkColor;
			}
			set
			{
				if( m_DropDownHighlightDarkColor != value )
				{
					m_DropDownHighlightDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color for pressed dropdown button of the CommandBar.
		/// </summary>
		public Color DropDownPressedLightColor
		{
			get
			{
				return m_DropDownPressedLightColor;
			}
			set
			{
				if( m_DropDownPressedLightColor != value )
				{
					m_DropDownPressedLightColor = value;
				}
			}
		}
		
		/// <summary>
		/// Gets or sets dark color for pressed dropdown button of the CommandBar.
		/// </summary>
		public Color DropDownPressedDarkColor
		{
			get
			{
				return m_DropDownPressedDarkColor;
			}
			set
			{
				if( m_DropDownPressedDarkColor != value )
				{
					m_DropDownPressedDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets color for highlighted dropdown button of the floating CommandBar.
		/// </summary>
		public Color FloatHighlightButtonColor
		{
			get
			{
				return m_FloatHighlightButtonColor;
			}
			set
			{
				if( m_FloatHighlightButtonColor != value )
				{
					m_FloatHighlightButtonColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets border color for highlighted dropdown button of the floating CommandBar.
		/// </summary>
		public Color FloatHighlightButtonBorderColor
		{
			get
			{
				return m_FloatHighlightButtonBorderColor;
			}
			set
			{
				if( m_FloatHighlightButtonBorderColor != value )
				{
					m_FloatHighlightButtonBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets color for pressed dropdown button of the floating CommandBar.
		/// </summary>
		public Color FloatPressButtonColor
		{
			get
			{
				return m_FloatPressButtonColor;
			}
			set
			{
				if( m_FloatPressButtonColor != value )
				{
					m_FloatPressButtonColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets border color for pressed dropdown button of the floating CommandBar.
		/// </summary>
		public Color FloatPressButtonBorderColor
		{
			get
			{
				return m_FloatPressButtonBorderColor;
			}
			set
			{
				if( m_FloatPressButtonBorderColor != value )
				{
					m_FloatPressButtonBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets border color for pressed close button of the floating CommandBar.
		/// </summary>
		public Color FloatPressCloseButtonBorderColor
		{
			get
			{
				return m_FloatPressCloseButtonBorderColor;
			}
			set
			{
				if( m_FloatPressCloseButtonBorderColor != value )
				{
					m_FloatPressCloseButtonBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets color for pressed close button of the floating CommandBar.
		/// </summary>
		public Color FloatPressCloseButtonColor
		{
			get
			{
				return m_FloatPressCloseButtonColor;
			}
			set
			{
				if( m_FloatPressCloseButtonColor != value )
				{
					m_FloatPressCloseButtonColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color of the floating CommandBar.
		/// </summary>
		public Color FloatCommandBarLightColor
		{
			get
			{
				return m_FloatCommandBarLightColor;
			}
			set
			{
				if( m_FloatCommandBarLightColor != value )
				{
					m_FloatCommandBarLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets dark color of the floating CommandBar.
		/// </summary>
		public Color FloatCommandBarDarkColor
		{
			get
			{
				return m_FloatCommandBarDarkColor;
			}
			set
			{
				if( m_FloatCommandBarDarkColor != value )
				{
					m_FloatCommandBarDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets color for light border of the floating CommandBar.
		/// </summary>
		public Color FloatLightBorderColor
		{
			get
			{
				return m_FloatLightBorderColor;
			}
			set
			{
				if( m_FloatLightBorderColor != value )
				{
					m_FloatLightBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets background color of the floating CommandBar.
		/// </summary>
		public Color FloatBackgroundColor
		{
			get
			{
				return m_FloatBackgroundColor;
			}
			set
			{
				if( m_FloatBackgroundColor != value )
				{
					m_FloatBackgroundColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets color for border of the floating CommandBar.
		/// </summary>
		public Color FloatBorderColor
		{
			get
			{
				return m_FloatBorderColor;
			}
			set
			{
				if( m_FloatBorderColor != value )
				{
					m_FloatBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets color for caption text of the floating CommandBar.
		/// </summary>
		public Color FloatCaptionColor
		{
			get
			{
				return m_FloatCaptionColor;
			}
			set
			{
				if( m_FloatCaptionColor != value )
				{
					m_FloatCaptionColor = value;
				}
			}
		}

		#endregion

		#region BarItem Colors
		/// <summary>
		/// Gets or sets color for separator line of the CommandBar.
		/// </summary>
		public Color BarItemSeparatorColor
		{
			get
			{
				return m_BarItemSeparatorColor;
			}
			set
			{
				if( m_BarItemSeparatorColor != value )
				{
					m_BarItemSeparatorColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets color for pressed border of the BarItem.
		/// </summary>
		public Color BarItemPressBorderColor
		{
			get
			{
				return m_BarItemPressBorderColor;
			}
			set
			{
				if( m_BarItemPressBorderColor != value )
				{
					m_BarItemPressBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets color for highlighted border of the BarItem.
		/// </summary>
		public Color BarItemHighlightBorderColor
		{
			get
			{
				return m_BarItemHighlightBorderColor;
			}
			set
			{
				if( m_BarItemHighlightBorderColor != value )
				{
					m_BarItemHighlightBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color for background of the BarItem.
		/// </summary>
		public Color BarItemPressLightColor
		{
			get
			{
				return m_BarItemPressLightColor;
			}
			set
			{
				if( m_BarItemPressLightColor != value )
				{
					m_BarItemPressLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets dark color for background of the BarItem.
		/// </summary>
		public Color BarItemPressDarkColor
		{
			get
			{
				return m_BarItemPressDarkColor;
			}
			set
			{
				if( m_BarItemPressDarkColor != value )
				{
					m_BarItemPressDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color for background of the DropDownBarItem.
		/// </summary>
		public Color DropDownBarItemLightColor
		{
			get
			{
				return m_DropDownBarItemLightColor;
			}
			set
			{
				if( m_DropDownBarItemLightColor != value )
				{
					m_DropDownBarItemLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets dark color for background of the DropDownBarItem.
		/// </summary>
		public Color DropDownBarItemDarkColor
		{
			get
			{
				return m_DropDownBarItemDarkColor;
			}
			set
			{
				if( m_DropDownBarItemDarkColor != value )
				{
					m_DropDownBarItemDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets color for border of the DropDownBarItem.
		/// </summary>
		public Color DropDownBarItemBorderColor
		{
			get
			{
				return m_DropDownBarItemBorderColor;
			}
			set
			{
				if( m_DropDownBarItemBorderColor != value )
				{
					m_DropDownBarItemBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color for background of the checked BarItem.
		/// </summary>
		public Color BarItemCheckLightColor
		{
			get
			{
				return m_BarItemCheckLightColor;
			}
			set
			{
				if( m_BarItemCheckLightColor != value )
				{
					m_BarItemCheckLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets dark color for background of the checked BarItem.
		/// </summary>
		public Color BarItemCheckDarkColor
		{
			get
			{
				return m_BarItemCheckDarkColor;
			}
			set
			{
				if( m_BarItemCheckDarkColor != value )
				{
					m_BarItemCheckDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets color for border of the checked BarItem.
		/// </summary>
		public Color BarItemCheckBorderColor
		{
			get
			{
				return m_BarItemCheckBorderColor;
			}
			set
			{
				if( m_BarItemCheckBorderColor != value )
				{
					m_BarItemCheckBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets color for flash of the checked BarItem.
		/// </summary>
		public Color BarItemCheckFlashColor
		{
			get
			{
				return m_BarItemCheckFlashColor;
			}
			set
			{
				if( m_BarItemCheckFlashColor != value )
				{
					m_BarItemCheckFlashColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets color for flash of the pressed BarItem.
		/// </summary>
		public Color BarItemPressFlashColor
		{
			get
			{
				return m_BarItemPressFlashColor;
			}
			set
			{
				if( m_BarItemPressFlashColor != value )
				{
					m_BarItemPressFlashColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets color for flash of the selected BarItem.
		/// </summary>
		public Color BarItemSelectFlashColor
		{
			get
			{
				return m_BarItemSelectFlashColor;
			}
			set
			{
				if( m_BarItemSelectFlashColor != value )
				{
					m_BarItemSelectFlashColor = value;
				}
			}
		}

        /// <summary>
        /// Gets or sets back color for the TextBoxBarItem.
        /// </summary>
        public Color TextBarItemBackColor
        {
            get
            {
                return m_TextBarItemBackColor;
            }
            set
            {
                if (m_TextBarItemBackColor != value)
                {
                    m_TextBarItemBackColor = value;
                }
            }
        }
        
        /// <summary>
        /// Gets or sets color for border of the TextBoxBarItem.
        /// </summary>
        public Color TextBarItemBorderColor
        {
            get
            {
                return m_TextBarItemBorderColor;
            }
            set
            {
                if (m_TextBarItemBorderColor != value)
                {
                    m_TextBarItemBorderColor = value;
                }
            }
        }

		/// <summary>
		/// Gets or sets color for border of the highlight TextBoxBarItem.
		/// </summary>
		public Color TextBarItemBorderHighlightColor
		{
			get
			{
				return m_TextBarItemBorderHighlightColor;
			}
			set
			{
				if( m_TextBarItemBorderHighlightColor != value )
				{
					m_TextBarItemBorderHighlightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color for ComboButton of the ComboBoxBarItem.
		/// </summary>
		public Color ComboButtonLightColor
		{
			get
			{
				return m_ComboButtonLightColor;
			}
			set
			{
				if( m_ComboButtonLightColor != value )
				{
					m_ComboButtonLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets dark color for ComboButton of the ComboBoxBarItem.
		/// </summary>
		public Color ComboButtonDarkColor
		{
			get
			{
				return m_ComboButtonDarkColor;
			}
			set
			{
				if( m_ComboButtonDarkColor != value )
				{
					m_ComboButtonDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color for ComboButton of the pressed ComboBoxBarItem.
		/// </summary>
		public Color ComboButtonPressLightColor
		{
			get
			{
				return m_ComboButtonPressLightColor;
			}
			set
			{
				if( m_ComboButtonPressLightColor != value )
				{
					m_ComboButtonPressLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets dark color for ComboButton of the pressed ComboBoxBarItem.
		/// </summary>
		public Color ComboButtonPressDarkColor
		{
			get
			{
				return m_ComboButtonPressDarkColor;
			}
			set
			{
				if( m_ComboButtonPressDarkColor != value )
				{
					m_ComboButtonPressDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color for ComboButton of the highlighted ComboBoxBarItem.
		/// </summary>
		public Color ComboButtonHighlightLightColor
		{
			get
			{
				return m_ComboButtonHighlightLightColor;
			}
			set
			{
				if( m_ComboButtonHighlightLightColor != value )
				{
					m_ComboButtonHighlightLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets dark color for ComboButton of the highlighted ComboBoxBarItem.
		/// </summary>
		public Color ComboButtonHighlightDarkColor
		{
			get
			{
				return m_ComboButtonHighlightDarkColor;
			}
			set
			{
				if( m_ComboButtonHighlightDarkColor != value )
				{
					m_ComboButtonHighlightDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets border color for ComboButton of the ComboBoxBarItem.
		/// </summary>
		public Color ComboButtonBorder
		{
			get
			{
				return m_ComboButtonBorder;
			}
			set
			{
				if( m_ComboButtonBorder != value )
				{
					m_ComboButtonBorder = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets border color for ComboButton of the pressed ComboBoxBarItem.
		/// </summary>
		public Color ComboButtonPressBorder
		{
			get
			{
				return m_ComboButtonPressBorder;
			}
			set
			{
				if( m_ComboButtonPressBorder != value )
				{
					m_ComboButtonPressBorder = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets border color for ComboButton of the highlighted ComboBoxBarItem.
		/// </summary>
		public Color ComboButtonHighlightBorder
		{
			get
			{
				return m_ComboButtonHighlightBorder;
			}
			set
			{
				if( m_ComboButtonHighlightBorder != value )
				{
					m_ComboButtonHighlightBorder = value;
				}
			}
		}

		#endregion

		#region Button Colors
		/// <summary></summary>
		public Color ButtonPressedTopColor
		{
			get
			{
				return m_ButtonPressedTopColor;
			}
		}
		/// <summary></summary>
		public Color ButtonPressedBottomColor
		{
			get
			{
				return m_ButtonPressedBottomColor;
			}
		}
		/// <summary></summary>
		public Color ButtonSelectedTopColor
		{
			get
			{
				return m_ButtonSelectedTopColor;
			}
		}
		/// <summary></summary>
		public Color ButtonSelectedBottomColor
		{
			get
			{
				return m_ButtonSelectedBottomColor;
			}
		}
		/// <summary></summary>
		public Color ButtonDisabledTopColor
		{
			get
			{
				return m_ButtonDisabledTopColor;
			}
		}
		/// <summary></summary>
		public Color ButtonDisabledBottomColor
		{
			get
			{
				return m_ButtonDisabledBottomColor;
			}
		}
		/// <summary></summary>
		public Color ButtonPressedBorderColor
		{
			get
			{
				return m_ButtonPressedBorderColor;
			}
		}
		/// <summary></summary>
		public Color ButtonSelectedBorderColor
		{
			get
			{
				return m_ButtonSelectedBorderColor;
			}
		}
		/// <summary></summary>
		public Color ButtonDisabledBorderColor
		{
			get
			{
				return m_ButtonDisabledBorderColor;
			}
		}
		/// <summary></summary>
		public Color ButtonDefaultTopColor
		{
			get
			{
				if( m_ButtonDefaultTopColor != Color.Empty )
				{
					return m_ButtonDefaultTopColor;
				}
				else
				{					
					return m_ButtonDefaultTopColor;
				}
			}
		}
		/// <summary></summary>
		public Color ButtonDefaultBottomColor
		{
			get
			{
				if( m_ButtonDefaultBottomColor != Color.Empty )
				{
					return m_ButtonDefaultBottomColor;
				}
				else
				{					
					return m_ButtonDefaultBottomColor;
				}
			}
		}
		/// <summary></summary>
		public Color ButtonDefaultBorderColor
		{
			get
			{
				if( m_ButtonDefaultBorderColor != Color.Empty )
				{
					return m_ButtonDefaultBorderColor;
				}
				else
				{					
					return m_ButtonDefaultBorderColor;
				}
			}
		}
		/// <summary></summary>
		public Color ButtonDefaultInternalBorderColor
		{
			get
			{
				if( m_ButtonDefaultInternalBorderColor != Color.Empty )
				{
					return m_ButtonDefaultInternalBorderColor;
				}
				else
				{					
					return m_ButtonDefaultInternalBorderColor;
				}
			}
		}
		/// <summary></summary>
		public Color ButtonPressedInternalBorderColor
		{
			get
			{
				if( m_ButtonPressedInternalBorderColor != Color.Empty )
				{
					return m_ButtonPressedInternalBorderColor;
				}
				else
				{					
					return m_ButtonPressedInternalBorderColor;
				}
			}
		}
		/// <summary></summary>
		public Color ButtonSelectedInternalBorderColor
		{
			get
			{
				if( m_ButtonSelectedInternalBorderColor != Color.Empty )
				{
					return m_ButtonSelectedInternalBorderColor;
				}
				else
				{
					return m_ButtonSelectedInternalBorderColor;
				}
			}
		}

		#region Obsolete properties
		[Obsolete]
		public Color BlueButtonDefaultTopColor
		{
			get
			{
				if ( m_BlueButtonDefaultTopColor != Color.Empty )
				{
					return m_BlueButtonDefaultTopColor;
				}
				else
				{
					m_BlueButtonDefaultTopColor = Color.FromArgb(231, 242, 255);
					return m_BlueButtonDefaultTopColor;
				}
			}
		}
		[Obsolete]
		public Color BlueButtonDefaultBottomColor
		{
			get
			{
				if ( m_BlueButtonDefaultBottomColor != Color.Empty )
				{
					return m_BlueButtonDefaultBottomColor;
				}
				else
				{
					m_BlueButtonDefaultBottomColor = Color.FromArgb(179, 209, 252);
					return m_BlueButtonDefaultBottomColor;
				}
			}
		}
		[Obsolete]
		public Color BlueButtonDefaultBorderColor
		{
			get
			{
				if ( m_BlueButtonDefaultBorderColor != Color.Empty )
				{
					return m_BlueButtonDefaultBorderColor;
				}
				else
				{
					m_BlueButtonDefaultBorderColor = Color.FromArgb(176, 208, 255);
					return m_BlueButtonDefaultBorderColor;
				}
			}
		}
		[Obsolete]
		public Color BlueButtonDefaultInternalBorderColor
		{
			get
			{
				if ( m_BlueButtonDefaultInternalBorderColor != Color.Empty )
				{
					return m_BlueButtonDefaultInternalBorderColor;
				}
				else
				{
					m_BlueButtonDefaultInternalBorderColor = Color.FromArgb(128, Color.FromArgb(250, 251, 255));
					return m_BlueButtonDefaultInternalBorderColor;
				}
			}
		}
		[Obsolete]
		public Color BlueButtonPressedInternalBorderColor
		{
			get
			{
				if ( m_BlueButtonPressedInternalBorderColor != Color.Empty )
				{
					return m_BlueButtonPressedInternalBorderColor;
				}
				else
				{
					m_BlueButtonPressedInternalBorderColor = Color.FromArgb(70, Color.FromArgb(176, 132, 92));
					return m_BlueButtonPressedInternalBorderColor;
				}
			}
		}
		[Obsolete]
		public Color BlueButtonSelectedInternalBorderColor
		{
			get
			{
				if ( m_BlueButtonSelectedInternalBorderColor != Color.Empty )
				{
					return m_BlueButtonSelectedInternalBorderColor;
				}
				else
				{
					m_BlueButtonSelectedInternalBorderColor = Color.FromArgb(128, Color.FromArgb(250, 251, 255));
					return m_BlueButtonSelectedInternalBorderColor;
				}
			}
		}
		[Obsolete]
		public Color SilverButtonDefaultTopColor
		{
			get
			{
				if ( m_SilverButtonDefaultTopColor != Color.Empty )
				{
					return m_SilverButtonDefaultTopColor;
				}
				else
				{
					m_SilverButtonDefaultTopColor = Color.FromArgb(223, 227, 231);
					return m_SilverButtonDefaultTopColor;
				}
			}
		}
		[Obsolete]
		public Color SilverButtonDefaultBottomColor
		{
			get
			{
				if ( m_SilverButtonDefaultBottomColor != Color.Empty )
				{
					return m_SilverButtonDefaultBottomColor;
				}
				else
				{
					m_SilverButtonDefaultBottomColor = Color.FromArgb(208, 212, 221);
					return m_SilverButtonDefaultBottomColor;
				}
			}
		}
		[Obsolete]
		public Color SilverButtonDefaultBorderColor
		{
			get
			{
				if ( m_SilverButtonDefaultBorderColor != Color.Empty )
				{
					return m_SilverButtonDefaultBorderColor;
				}
				else
				{
					m_SilverButtonDefaultBorderColor = Color.FromArgb(208, 212, 221);
					return m_SilverButtonDefaultBorderColor;
				}
			}
		}
		[Obsolete]
		public Color SilverButtonDefaultInternalBorderColor
		{
			get
			{
				if ( m_SilverButtonDefaultInternalBorderColor != Color.Empty )
				{
					return m_SilverButtonDefaultInternalBorderColor;
				}
				else
				{
					m_SilverButtonDefaultInternalBorderColor = Color.FromArgb(128, Color.FromArgb(250, 251, 255));
					return m_SilverButtonDefaultInternalBorderColor;
				}
			}
		}
		[Obsolete]
		public Color SilverButtonPressedInternalBorderColor
		{
			get
			{
				if ( m_SilverButtonPressedInternalBorderColor != Color.Empty )
				{
					return m_SilverButtonPressedInternalBorderColor;
				}
				else
				{
					m_SilverButtonPressedInternalBorderColor = Color.FromArgb(70, Color.FromArgb(176, 132, 92));
					return m_SilverButtonPressedInternalBorderColor;
				}
			}
		}
		[Obsolete]
		public Color SilverButtonSelectedInternalBorderColor
		{
			get
			{
				if ( m_SilverButtonSelectedInternalBorderColor != Color.Empty )
				{
					return m_SilverButtonSelectedInternalBorderColor;
				}
				else
				{
					m_SilverButtonSelectedInternalBorderColor = Color.FromArgb(128, Color.FromArgb(250, 251, 255));
					return m_SilverButtonSelectedInternalBorderColor;
				}
			}
		}
		[Obsolete]
		public Color BlackButtonDefaultTopColor
		{
			get
			{
				if ( m_BlackButtonDefaultTopColor != Color.Empty )
				{
					return m_BlackButtonDefaultTopColor;
				}
				else
				{
					m_BlackButtonDefaultTopColor = Color.FromArgb(146, 146, 146);
					return m_BlackButtonDefaultTopColor;
				}
			}
		}
		[Obsolete]
		public Color BlackButtonDefaultBottomColor
		{
			get
			{
				if ( m_BlackButtonDefaultBottomColor != Color.Empty )
				{
					return m_BlackButtonDefaultBottomColor;
				}
				else
				{
					m_BlackButtonDefaultBottomColor = Color.FromArgb(83, 83, 83);
					return m_BlackButtonDefaultBottomColor;
				}
			}
		}
		[Obsolete]
		public Color BlackButtonDefaultBorderColor
		{
			get
			{
				if ( m_BlackButtonDefaultBorderColor != Color.Empty )
				{
					return m_BlackButtonDefaultBorderColor;
				}
				else
				{
					m_BlackButtonDefaultBorderColor = Color.FromArgb(153, 153, 153);
					return m_BlackButtonDefaultBorderColor;
				}
			}
		}
		[Obsolete]
		public Color BlackButtonDefaultInternalBorderColor
		{
			get
			{
				if ( m_BlackButtonDefaultInternalBorderColor != Color.Empty )
				{
					return m_BlackButtonDefaultInternalBorderColor;
				}
				else
				{
					m_BlackButtonDefaultInternalBorderColor = Color.FromArgb(128, Color.FromArgb(250, 251, 255));
					return m_BlackButtonDefaultInternalBorderColor;
				}
			}
		}
		[Obsolete]
		public Color BlackButtonPressedInternalBorderColor
		{
			get
			{
				if ( m_BlackButtonPressedInternalBorderColor != Color.Empty )
				{
					return m_BlackButtonPressedInternalBorderColor;
				}
				else
				{
					m_BlackButtonPressedInternalBorderColor = Color.FromArgb(70, Color.FromArgb(176, 132, 92));
					return m_BlackButtonPressedInternalBorderColor;
				}
			}
		}
		[Obsolete]
		public Color BlackButtonSelectedInternalBorderColor
		{
			get
			{
				if ( m_BlackButtonSelectedInternalBorderColor != Color.Empty )
				{
					return m_BlackButtonSelectedInternalBorderColor;
				}
				else
				{
					m_BlackButtonSelectedInternalBorderColor = Color.FromArgb(128, Color.FromArgb(250, 251, 255));
					return m_BlackButtonSelectedInternalBorderColor;
				}
			}
		}
		#endregion

		#endregion

		#region TabControlAdv colors

		public Color TabDefaultBorderColor
		{
			get { return m_TabDefaultBorderColor; }
		}

		public Color TabHotLightBottomBorderLineColor
		{
			get { return m_TabHotLightBottomBorderLineColor; }
		}

		public Color TabHotLightGradientTopBeginColor
		{
			get { return m_TabHotLightGradientTopBeginColor; }
		}

		public Color TabHotLightGradientTopEndColor
		{
			get { return m_TabHotLightGradientTopEndColor; }
		}

		public Color TabHotLightGradientBottomBeginColor
		{
			get { return m_TabHotLightGradientBottomBeginColor; }
		}

		public Color TabHotLightGradientBottomEndColor
		{
			get { return m_TabHotLightGradientBottomEndColor; }
		}

		public Color TabHotLightGradientCircleColor
		{
			get { return m_TabHotLightGradientCircleColor; }
		}

		public Color TabSelectedGradientTopColor
		{
			get { return m_TabSelectedGradientTopColor; }
		}

		public Color TabSelectedGradientBottomColor
		{
			get { return m_TabSelectedGradientBottomColor; }
		}

		public Color TabSelectedInnerBorderColor
		{
			get { return m_TabSelectedInnerBorderColor; }
		}

		public Color TabHighlightInnerBorderColor
		{
			get { return m_TabHighlightInnerBorderColor; }
		}

		public Color TabSelectedHotLightBorderColor
		{
			get { return m_TabSelectedHotLightBorderColor; }
		}

		public Color TabSelectedHotLightInnerBorderColor
		{
			get { return m_TabSelectedHotLightInnerBorderColor; }
		}

		public Color TabForeColor
		{
			get { return m_TabForeColor; }
		}

		public Color ActiveTabForeColor
		{
			get { return m_ActiveTabForeColor; }
		}

		public Color TabBackgroundColor
		{
			get { return m_TabBackgroundColor; }
		}

        public Color TabScrollArrowColor
        {
            get { return m_TabScrollArrowColor; }
        }

		#endregion

		#region DockTabControl colors

		public Color DockTabForeColor
		{
			get { return m_DockTabForeColor; }
		}

		public Color DockTabBackgroundColor
		{
			get { return m_DockTabBackgroundColor; }
		}

		#endregion

		#region Form colors

		public Color ActiveFormBorderColor
		{
			get { return m_ActiveFormBorderColor; }
		}
		public Color InactiveFormBorderColor
		{
			get { return m_InactiveFormBorderColor; }
		}
         public Color ActiveTextBoxBorderColor
        {
            get { return m_ActiveTextBoxBorderColor; }
        }
        public Color InactiveTextBoxBorderColor
        {
            get { return m_InactiveTextBoxBorderColor; }
        }
        public Color ActiveTextBoxBackColor
        {
            get { return m_ActiveTextBoxBackColor; }
        }
        public Color InactiveTextBoxBackColor
        {
            get { return m_InactiveTextBoxBackColor; }
        }
        public Color SelectedNodeBackground
        {
            get { return m_SelectedNodeBackground; }
        }
        public Color TreeviewBackColor
        {
            get { return m_TreeviewBackColor; }
        }
        public Color TreeNodeArrowColor
        {
            get { return m_TreeNodeArrowColor; }
        }
        public Color TreeViewFontColor
        {
            get { return m_TreeViewFontColor; }
        }
		public Color FormTextColor
		{
			get { return m_FormTextColor; }
		}
		public Color ActiveTitleGradientBegin
		{
			get { return m_ActiveTitleGradientBegin; }
		}
		public Color ActiveTitleGradientEnd
		{
			get { return m_ActiveTitleGradientEnd; }
		}
		public Color InactiveTitleGradientBegin
		{
			get { return m_InactiveTitleGradientBegin; }
		}
		public Color InactiveTitleGradientEnd
		{
			get { return m_InactiveTitleGradientEnd; }
		}
		public Color SystemButtonSelectedGradientBegin
		{
			get { return m_SystemButtonSelectedGradientBegin;}
		}
		public Color SystemButtonSelectedGradientEnd
		{
			get { return m_SystemButtonSelectedGradientEnd; }
		}
		public Color SystemButtonPressedGradientBegin
		{
			get { return m_SystemButtonPressedGradientBegin; }
		}
		public Color SystemButtonPressedGradientEnd
		{
			get { return m_SystemButtonPressedGradientEnd; }
		}

		public Color SystemButtonBorderSelected
		{
			get { return m_SystemButtonBorderSelected; }
		}
		public Color SystemButtonBorderPressed
		{
			get { return m_SystemButtonBorderPressed; }
		}
		public Color FormBackground
		{
			get { return m_FormBackground; }
		}
		#endregion

		#region UpDown colors
		/// <summary>
		/// Gets the arrow start color for UpDownButtons.
		/// </summary>
		public Color UpDownArrowStartColor 
		{
			get
			{
				return m_UpDownArrowStartColor;
			}
		}
		/// <summary>
		/// Gets the arrow end color for UpDownButtons.
		/// </summary>
		public Color UpDownArrowEndColor
		{
			get
			{
				return m_UpDownArrowEndColor;
			}
		}
		/// <summary>
		/// Gets the border color for UpDownButtons in normal state.
		/// </summary>
		public Color UpDownBorderNormalColor
		{
			get
			{
				return m_UpDownBorderNormalColor;
			}
		}
		/// <summary>
		/// Gets the background color for UpDownButtons in normal state. 
		/// </summary>
		public Color UpDownBackgroundNormalColor
		{
			get
			{
				return m_UpDownBackgroundNormalColor;
			}
		}
		/// <summary>
		/// Gets the background start color for UpDownButtons in normal state.
		/// </summary>
		public Color UpDownBackgroundNormalStartColor
		{
			get
			{
				return m_UpDownBackgroundNormalStartColor;
			}
		}
		/// <summary>
		/// Gets the background end color for UpDownButtons in normal state.
		/// </summary>
		public Color UpDownBackgroundNormalEndColor
		{
			get
			{
				return m_UpDownBackgroundNormalEndColor;
			}
		}
		/// <summary>
		/// Gets the border color for UpDownButtons in hot state.
		/// </summary>
		public Color UpDownBorderHotColor
		{
			get
			{
				return m_UpDownBorderHotColor;
			}
		}
		/// <summary>
		/// Gets the inner border start color for UpDownButtons in hot state.
		/// </summary>
		public Color UpDownInnerBorderHotStartColor
		{
			get
			{
				return m_UpDownInnerBorderHotStartColor;
			}
		}
		/// <summary>
		/// Gets the inner border end color for UpDownButtons in hot state.
		/// </summary>
		public Color UpDownInnerBorderHotEndColor
		{
			get
			{
				return m_UpDownInnerBorderHotEndColor;
			}
		}
		/// <summary>
		/// Gets the border color for UpDownButtons in pressed state.
		/// </summary>
		public Color UpDownBorderPressedColor
		{
			get
			{
				return m_UpDownBorderPressedColor;
			}
		}
		/// <summary>
		/// Gets the inner border start color for UpDownButtons in pressed state.
		/// </summary>
		public Color UpDownInnerBorderPressedStartColor
		{
			get
			{
				return m_UpDownInnerBorderPressedStartColor;
			}
		}
		/// <summary>
		/// Gets the inner border end color for UpDownButtons in pressed state.
		/// </summary>
		public Color UpDownInnerBorderPressedEndColor
		{
			get
			{
				return m_UpDownInnerBorderPressedEndColor;
			}
		}
		/// <summary>
		/// Gets the background start color for UpDownButtons in disabled state.
		/// </summary>
		public Color UpDownBackgroundDisabledStartColor
		{
			get
			{
				return m_UpDownBackgroundDisabledStartColor;
			}
		}
		/// <summary>
		/// Gets the background end color for UpDownButtons in disabled state.
		/// </summary>
		public Color UpDownBackgroundDisabledEndColor
		{
			get
			{
				return m_UpDownBackgroundDisabledEndColor;
			}
		}
		/// <summary>
		/// Gets the border color for UpDownButtons in disabled state.
		/// </summary>
		public Color UpDownBorderDisabledColor
		{
			get
			{
				return m_UpDownBorderDisabledColor;
			}
		}
		/// <summary>
		/// Gets the background top start color for UpDownButtons in hot state.
		/// </summary>
		public Color UpDownBackgroundHotTopStartColor
		{
			get
			{
				return m_UpDownBackgroundHotTopStartColor;
			}
		}
		/// <summary>
		/// Gets the background top end color for UpDownButtons in hot state.
		/// </summary>
		public Color UpDownBackgroundHotTopEndColor
		{
			get
			{
				return m_UpDownBackgroundHotTopEndColor;
			}
		}
		/// <summary>
		/// Gets the background bottom start color for UpDownButtons in hot state.
		/// </summary>
		public Color UpDownBackgroundHotBottomStartColor
		{
			get
			{
				return m_UpDownBackgroundHotBottomStartColor;
			}
		}
		/// <summary>
		/// Gets the background bottom end color for UpDownButtons in hot state.
		/// </summary>
		public Color UpDownBackgroundHotBottomEndColor
		{
			get
			{
				return m_UpDownBackgroundHotBottomEndColor;
			}
		}
		/// <summary>
		/// Gets the background top start color for UpDownButtons in pressed state.
		/// </summary>
		public Color UpDownBackgroundPressedTopStartColor
		{
			get
			{
				return m_UpDownBackgroundPressedTopStartColor;
			}
		}
		/// <summary>
		/// Gets the background top end color for UpDownButtons in pressed state.
		/// </summary>
		public Color UpDownBackgroundPressedTopEndColor
		{
			get
			{
				return m_UpDownBackgroundPressedTopEndColor;
			}
		}
		/// <summary>
		/// Gets the background bottom start color for UpDownButtons in pressed state.
		/// </summary>
		public Color UpDownBackgroundPressedBottomStartColor
		{
			get
			{
				return m_UpDownBackgroundPressedBottomStartColor;
			}
		}
		/// <summary>
		/// Gets the background bottom end color for UpDownButtons in pressed state.
		/// </summary>
		public Color UpDownBackgroundPressedBottomEndColor
		{
			get
			{
				return m_UpDownBackgroundPressedBottomEndColor;
			}
		}
		#endregion

        #region ComboBoxAdv colors
        /// <summary>
        /// Gets or sets the back color for <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvNormalBackColor
        {
            get
            {
                return m_ComboBoxAdvNormalBackColor;
            }
            set
            {
                if( m_ComboBoxAdvNormalBackColor != value )
                {
                    m_ComboBoxAdvNormalBackColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the back color for the selected <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvHotBackColor
        {
            get
            {
                return m_ComboBoxAdvHotBackColor;
            }
            set
            {
                if( m_ComboBoxAdvHotBackColor != value )
                {
                    m_ComboBoxAdvHotBackColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the border color for <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvNormalBorderColor
        {
            get
            {
                return m_ComboBoxAdvNormalBorderColor;
            }
            set
            {
                if( m_ComboBoxAdvNormalBorderColor != value )
                {
                    m_ComboBoxAdvNormalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the border color for the selected <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvHotBorderColor
        {
            get
            {
                return m_ComboBoxAdvHotBorderColor;
            }
            set
            {
                if( m_ComboBoxAdvHotBorderColor != value )
                {
                    m_ComboBoxAdvHotBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the border color for the pushed <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvPushedBorderColor
        {
            get
            {
                return m_ComboBoxAdvPushedBorderColor;
            }
            set
            {
                if( m_ComboBoxAdvPushedBorderColor != value )
                {
                    m_ComboBoxAdvPushedBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for upper line of the dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvButtonUpperLineColor
        {
            get
            {
                return m_ComboBoxAdvButtonUpperLineColor;
            }
            set
            {
                if( m_ComboBoxAdvButtonUpperLineColor != value )
                {
                    m_ComboBoxAdvButtonUpperLineColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for the arrow of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvArrowColor
        {
            get
            {
                return m_ComboBoxAdvArrowColor;
            }
            set
            {
                if( m_ComboBoxAdvArrowColor != value )
                {
                    m_ComboBoxAdvArrowColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for lower line of the arrow of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvLowerArrowLineColor
        {
            get
            {
                return m_ComboBoxAdvLowerArrowLineColor;
            }
            set
            {
                if( m_ComboBoxAdvLowerArrowLineColor != value )
                {
                    m_ComboBoxAdvLowerArrowLineColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the hot background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvHotBackgroundButtonColor1
        {
            get
            {
                return m_ComboBoxAdvHotBackgroundButtonColor1;
            }
            set
            {
                if( m_ComboBoxAdvHotBackgroundButtonColor1 != value )
                {
                    m_ComboBoxAdvHotBackgroundButtonColor1 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the hot background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvHotBackgroundButtonColor2
        {
            get
            {
                return m_ComboBoxAdvHotBackgroundButtonColor2;
            }
            set
            {
                if( m_ComboBoxAdvHotBackgroundButtonColor2 != value )
                {
                    m_ComboBoxAdvHotBackgroundButtonColor2 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the hot background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvHotBackgroundButtonColor3
        {
            get
            {
                return m_ComboBoxAdvHotBackgroundButtonColor3;
            }
            set
            {
                if( m_ComboBoxAdvHotBackgroundButtonColor3 != value )
                {
                    m_ComboBoxAdvHotBackgroundButtonColor3 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the hot background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvHotBackgroundButtonColor4
        {
            get
            {
                return m_ComboBoxAdvHotBackgroundButtonColor4;
            }
            set
            {
                if( m_ComboBoxAdvHotBackgroundButtonColor4 != value )
                {
                    m_ComboBoxAdvHotBackgroundButtonColor4 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvNormalBackgroundButtonColor1
        {
            get
            {
                return m_ComboBoxAdvNormalBackgroundButtonColor1;
            }
            set
            {
                if( m_ComboBoxAdvNormalBackgroundButtonColor1 != value )
                {
                    m_ComboBoxAdvNormalBackgroundButtonColor1 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvNormalBackgroundButtonColor2
        {
            get
            {
                return m_ComboBoxAdvNormalBackgroundButtonColor2;
            }
            set
            {
                if( m_ComboBoxAdvNormalBackgroundButtonColor2 != value )
                {
                    m_ComboBoxAdvNormalBackgroundButtonColor2 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvNormalBackgroundButtonColor3
        {
            get
            {
                return m_ComboBoxAdvNormalBackgroundButtonColor3;
            }
            set
            {
                if( m_ComboBoxAdvNormalBackgroundButtonColor3 != value )
                {
                    m_ComboBoxAdvNormalBackgroundButtonColor3 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvNormalBackgroundButtonColor4
        {
            get
            {
                return m_ComboBoxAdvNormalBackgroundButtonColor4;
            }
            set
            {
                if( m_ComboBoxAdvNormalBackgroundButtonColor4 != value )
                {
                    m_ComboBoxAdvNormalBackgroundButtonColor4 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvPushedBackgroundButtonColor1
        {
            get
            {
                return m_ComboBoxAdvPushedBackgroundButtonColor1;
            }
            set
            {
                if( m_ComboBoxAdvPushedBackgroundButtonColor1 != value )
                {
                    m_ComboBoxAdvPushedBackgroundButtonColor1 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvPushedBackgroundButtonColor2
        {
            get
            {
                return m_ComboBoxAdvPushedBackgroundButtonColor2;
            }
            set
            {
                if( m_ComboBoxAdvPushedBackgroundButtonColor2 != value )
                {
                    m_ComboBoxAdvPushedBackgroundButtonColor2 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvPushedBackgroundButtonColor3
        {
            get
            {
                return m_ComboBoxAdvPushedBackgroundButtonColor3;
            }
            set
            {
                if( m_ComboBoxAdvPushedBackgroundButtonColor3 != value )
                {
                    m_ComboBoxAdvPushedBackgroundButtonColor3 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvPushedBackgroundButtonColor4
        {
            get
            {
                return m_ComboBoxAdvPushedBackgroundButtonColor4;
            }
            set
            {
                if( m_ComboBoxAdvPushedBackgroundButtonColor4 != value )
                {
                    m_ComboBoxAdvPushedBackgroundButtonColor4 = value;
                }
            }
        }

        #endregion

        #region CheckBoxAdv colors
        /// <summary>
        /// Used in drawing of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvNormalBackColor
        {
            get
            {
                return m_CheckBoxAdvNormalBackColor;
            }
            set
            {
                if( m_CheckBoxAdvNormalBackColor != value )
                {
                    m_CheckBoxAdvNormalBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvSelectedBackColor
        {
            get
            {
                return m_CheckBoxAdvSelectedBackColor;
            }
            set
            {
                if( m_CheckBoxAdvSelectedBackColor != value )
                {
                    m_CheckBoxAdvSelectedBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvPushedBackColor
        {
            get
            {
                return m_CheckBoxAdvPushedBackColor;
            }
            set
            {
                if( m_CheckBoxAdvPushedBackColor != value )
                {
                    m_CheckBoxAdvPushedBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvNormalBorderColor
        {
            get
            {
                return m_CheckBoxAdvNormalBorderColor;
            }
            set
            {
                if( m_CheckBoxAdvNormalBorderColor != value )
                {
                    m_CheckBoxAdvNormalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the selected border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvSelectedBorderColor
        {
            get
            {
                return m_CheckBoxAdvSelectedBorderColor;
            }
            set
            {
                if( m_CheckBoxAdvSelectedBorderColor != value )
                {
                    m_CheckBoxAdvSelectedBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvPushedBorderColor
        {
            get
            {
                return m_CheckBoxAdvPushedBorderColor;
            }
            set
            {
                if( m_CheckBoxAdvPushedBorderColor != value )
                {
                    m_CheckBoxAdvPushedBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal internal border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvNormalInternalBorderColor
        {
            get
            {
                return m_CheckBoxAdvNormalInternalBorderColor;
            }
            set
            {
                if( m_CheckBoxAdvNormalInternalBorderColor != value )
                {
                    m_CheckBoxAdvNormalInternalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the selected internal border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvSelectedInternalBorderColor
        {
            get
            {
                return m_CheckBoxAdvSelectedInternalBorderColor;
            }
            set
            {
                if( m_CheckBoxAdvSelectedInternalBorderColor != value )
                {
                    m_CheckBoxAdvSelectedInternalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed internal border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvPushedInternalBorderColor
        {
            get
            {
                return m_CheckBoxAdvPushedInternalBorderColor;
            }
            set
            {
                if( m_CheckBoxAdvPushedInternalBorderColor != value )
                {
                    m_CheckBoxAdvPushedInternalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal internal rectangle border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvNormalInternalRectangleBorderColor
        {
            get
            {
                return m_CheckBoxAdvNormalInternalRectangleBorderColor;
            }
            set
            {
                if( m_CheckBoxAdvNormalInternalRectangleBorderColor != value )
                {
                    m_CheckBoxAdvNormalInternalRectangleBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the selected internal rectangle border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvSelectedInternalRectangleBorderColor
        {
            get
            {
                return m_CheckBoxAdvSelectedInternalRectangleBorderColor;
            }
            set
            {
                if( m_CheckBoxAdvSelectedInternalRectangleBorderColor != value )
                {
                    m_CheckBoxAdvSelectedInternalRectangleBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed internal rectangle border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvPushedInternalRectangleBorderColor
        {
            get
            {
                return m_CheckBoxAdvPushedInternalRectangleBorderColor;
            }
            set
            {
                if( m_CheckBoxAdvPushedInternalRectangleBorderColor != value )
                {
                    m_CheckBoxAdvPushedInternalRectangleBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal internal rectangle of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvNormalInternalRectangleColor
        {
            get
            {
                return m_CheckBoxAdvNormalInternalRectangleColor;
            }
            set
            {
                if( m_CheckBoxAdvNormalInternalRectangleColor != value )
                {
                    m_CheckBoxAdvNormalInternalRectangleColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the selected internal rectangle of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvSelectedInternalRectangleColor
        {
            get
            {
                return m_CheckBoxAdvSelectedInternalRectangleColor;
            }
            set
            {
                if( m_CheckBoxAdvSelectedInternalRectangleColor != value )
                {
                    m_CheckBoxAdvSelectedInternalRectangleColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed internal rectangle of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvPushedInternalRectangleColor
        {
            get
            {
                return m_CheckBoxAdvPushedInternalRectangleColor;
            }
            set
            {
                if( m_CheckBoxAdvPushedInternalRectangleColor != value )
                {
                    m_CheckBoxAdvPushedInternalRectangleColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal tick of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvNormalTickColor
        {
            get
            {
                return m_CheckBoxAdvNormalTickColor;
            }
            set
            {
                if( m_CheckBoxAdvNormalTickColor != value )
                {
                    m_CheckBoxAdvNormalTickColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the selected tick of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvSelectedTickColor
        {
            get
            {
                return m_CheckBoxAdvSelectedTickColor;
            }
            set
            {
                if( m_CheckBoxAdvSelectedTickColor != value )
                {
                    m_CheckBoxAdvSelectedTickColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed tick of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvPushedTickColor
        {
            get
            {
                return m_CheckBoxAdvPushedTickColor;
            }
            set
            {
                if( m_CheckBoxAdvPushedTickColor != value )
                {
                    m_CheckBoxAdvPushedTickColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the disabled tick of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvDisabledTickColor
        {
            get
            {
                return m_CheckBoxAdvDisabledTickColor;
            }
            set
            {
                if( m_CheckBoxAdvDisabledTickColor != value )
                {
                    m_CheckBoxAdvDisabledTickColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the indeterminate rectangle of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvIndeterminateRectangleColor
        {
            get
            {
                return m_CheckBoxAdvIndeterminateRectangleColor;
            }
            set
            {
                if( m_CheckBoxAdvIndeterminateRectangleColor != value )
                {
                    m_CheckBoxAdvIndeterminateRectangleColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the disabled back color <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvDisabledBackColor
        {
            get
            {
                return m_CheckBoxAdvDisabledBackColor;
            }
            set
            {
                if( m_CheckBoxAdvDisabledBackColor != value )
                {
                    m_CheckBoxAdvDisabledBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the disabled border <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvDisabledBorderColor
        {
            get
            {
                return m_CheckBoxAdvDisabledBorderColor;
            }
            set
            {
                if( m_CheckBoxAdvDisabledBorderColor != value )
                {
                    m_CheckBoxAdvDisabledBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the disabled internal border <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvDisabledInternalBorderColor
        {
            get
            {
                return m_CheckBoxAdvDisabledInternalBorderColor;
            }
            set
            {
                if( m_CheckBoxAdvDisabledInternalBorderColor != value )
                {
                    m_CheckBoxAdvDisabledInternalBorderColor = value;
                }
            }
        }

        #endregion

        #region RadioButtonAdv colors
        /// <summary>
        /// Used in drawing of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvNormalBackColor
        {
            get
            {
                return m_RadioButtonAdvNormalBackColor;
            }
            set
            {
                if( m_RadioButtonAdvNormalBackColor != value )
                {
                   m_RadioButtonAdvNormalBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the border of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvNormalBorderColor
        {
            get
            {
                return m_RadioButtonAdvNormalBorderColor;
            }
            set
            {
                if( m_RadioButtonAdvNormalBorderColor != value )
                {
                    m_RadioButtonAdvNormalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the internal border of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvNormalInternalBorderColor
        {
            get
            {
                return m_RadioButtonAdvNormalInternalBorderColor;
            }
            set
            {
                if( m_RadioButtonAdvNormalInternalBorderColor != value )
                {
                    m_RadioButtonAdvNormalInternalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvSelectedBackColor
        {
            get
            {
                return m_RadioButtonAdvSelectedBackColor;
            }
            set
            {
                if( m_RadioButtonAdvSelectedBackColor != value )
                {
                   m_RadioButtonAdvSelectedBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the border of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvSelectedBorderColor
        {
            get
            {
                return m_RadioButtonAdvSelectedBorderColor;
            }
            set
            {
                if( m_RadioButtonAdvSelectedBorderColor != value )
                {
                    m_RadioButtonAdvSelectedBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the internal border of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvSelectedInternalBorderColor
        {
            get
            {
                return m_RadioButtonAdvSelectedInternalBorderColor;
            }
            set
            {
                if( m_RadioButtonAdvSelectedInternalBorderColor != value )
                {
                    m_RadioButtonAdvSelectedInternalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvPushedBackColor
        {
            get
            {
                return m_RadioButtonAdvPushedBackColor;
            }
            set
            {
                if( m_RadioButtonAdvPushedBackColor != value )
                {
                   m_RadioButtonAdvPushedBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the border of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvPushedBorderColor
        {
            get
            {
                return m_RadioButtonAdvPushedBorderColor;
            }
            set
            {
                if( m_RadioButtonAdvPushedBorderColor != value )
                {
                    m_RadioButtonAdvPushedBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the internal border of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvPushedInternalBorderColor
        {
            get
            {
                return m_RadioButtonAdvPushedInternalBorderColor;
            }
            set
            {
                if( m_RadioButtonAdvPushedInternalBorderColor != value )
                {
                    m_RadioButtonAdvPushedInternalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the border of check mark of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvCheckMarkBorderColor
        {
            get
            {
                return m_RadioButtonAdvCheckMarkBorderColor;
            }
            set
            {
                if( m_RadioButtonAdvCheckMarkBorderColor != value )
                {
                    m_RadioButtonAdvCheckMarkBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of check mark of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvCheckMarkNormalBottomColor
        {
            get
            {
                return m_RadioButtonAdvCheckMarkNormalBottomColor;
            }
            set
            {
                if( m_RadioButtonAdvCheckMarkNormalBottomColor != value )
                {
                    m_RadioButtonAdvCheckMarkNormalBottomColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of check mark of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvCheckMarkSelectedBottomColor
        {
            get
            {
                return m_RadioButtonAdvCheckMarkSelectedBottomColor;
            }
            set
            {
                if( m_RadioButtonAdvCheckMarkSelectedBottomColor != value )
                {
                    m_RadioButtonAdvCheckMarkSelectedBottomColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of check mark of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvCheckMarkPushedBottomColor
        {
            get
            {
                return m_RadioButtonAdvCheckMarkPushedBottomColor;
            }
            set
            {
                if( m_RadioButtonAdvCheckMarkPushedBottomColor != value )
                {
                    m_RadioButtonAdvCheckMarkPushedBottomColor = value;
                }
            }
        }

        #endregion

        #region TabBarSplitterControl colors
        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterBackColor
        {
            get
            {
                return m_TabBarSplitterBackColor;
            }
            set
            {
                if( m_TabBarSplitterBackColor != value )
                {
                    m_TabBarSplitterBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterBorderColor
        {
            get
            {
                return m_TabBarSplitterBorderColor;
            }
            set
            {
                if( m_TabBarSplitterBorderColor != value )
                {
                    m_TabBarSplitterBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterTextColor
        {
            get
            {
                return m_TabBarSplitterTextColor;
            }
            set
            {
                if( m_TabBarSplitterTextColor != value )
                {
                    m_TabBarSplitterTextColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterTabStartColor
        {
            get
            {
                return m_TabBarSplitterTabStartColor;
            }
            set
            {
                if( m_TabBarSplitterTabStartColor != value )
                {
                    m_TabBarSplitterTabStartColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterTabEndColor
        {
            get
            {
                return m_TabBarSplitterTabEndColor;
            }
            set
            {
                if( m_TabBarSplitterTabEndColor != value )
                {
                    m_TabBarSplitterTabEndColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterTabBarStartColor
        {
            get
            {
                return m_TabBarSplitterTabBarStartColor;
            }
            set
            {
                if( m_TabBarSplitterTabBarStartColor != value )
                {
                    m_TabBarSplitterTabBarStartColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterTabBarEndColor
        {
            get
            {
                return m_TabBarSplitterTabBarEndColor;
            }
            set
            {
                if( m_TabBarSplitterTabBarEndColor != value )
                {
                    m_TabBarSplitterTabBarEndColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterButtonHoveredStartColor
        {
            get
            {
                return m_TabBarSplitterButtonHoveredStartColor;
            }
            set
            {
                if( m_TabBarSplitterButtonHoveredStartColor != value )
                {
                    m_TabBarSplitterButtonHoveredStartColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterButtonHoveredEndColor
        {
            get
            {
                return m_TabBarSplitterButtonHoveredEndColor;
            }
            set
            {
                if( m_TabBarSplitterButtonHoveredEndColor != value )
                {
                    m_TabBarSplitterButtonHoveredEndColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterButtonPushedStartColor
        {
            get
            {
                return m_TabBarSplitterButtonPushedStartColor;
            }
            set
            {
                if( m_TabBarSplitterButtonPushedStartColor != value )
                {
                    m_TabBarSplitterButtonPushedStartColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterButtonPushedEndColor
        {
            get
            {
                return m_TabBarSplitterButtonPushedEndColor;
            }
            set
            {
                if( m_TabBarSplitterButtonPushedEndColor != value )
                {
                    m_TabBarSplitterButtonPushedEndColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterSizeGripperColor
        {
            get
            {
                return m_TabBarSplitterSizeGripperColor;
            }
            set
            {
                if( m_TabBarSplitterSizeGripperColor != value )
                {
                    m_TabBarSplitterSizeGripperColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterSizeGripperLightColor
        {
            get
            {
                return m_TabBarSplitterSizeGripperLightColor;
            }
            set
            {
                if( m_TabBarSplitterSizeGripperLightColor != value )
                {
                    m_TabBarSplitterSizeGripperLightColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterSizeGripperDarkColor
        {
            get
            {
                return m_TabBarSplitterSizeGripperDarkColor;
            }
            set
            {
                if( m_TabBarSplitterSizeGripperDarkColor != value )
                {
                    m_TabBarSplitterSizeGripperDarkColor = value;
                }
            }
        }
        
        #endregion

        #region XPTaskBar colors
        /// <summary>
        /// Used in drawing of <see cref="XPTaskBar">.
        /// </summary>
        public Color XPTaskBarBorderColor
        {
            get
            {
                return m_XPTaskBarBorderColor;
            }
            set
            {
                if( m_XPTaskBarBorderColor != value )
                {
                    m_XPTaskBarBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="XPTaskBar">.
        /// </summary>
        public Color XPTaskBarBoxBackColor
        {
            get
            {
                return m_XPTaskBarBoxBackColor;
            }
            set
            {
                if( m_XPTaskBarBoxBackColor != value )
                {
                    m_XPTaskBarBoxBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="XPTaskBar">.
        /// </summary>
        public Color XPTaskBarBoxForeColor
        {
            get
            {
                return m_XPTaskBarBoxForeColor;
            }
            set
            {
                if( m_XPTaskBarBoxForeColor != value )
                {
                    m_XPTaskBarBoxForeColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="XPTaskBar">.
        /// </summary>
        public Color XPTaskBarBoxHeaderUpperLineColor
        {
            get
            {
                return m_XPTaskBarBoxHeaderUpperLineColor;
            }
            set
            {
                if( m_XPTaskBarBoxHeaderUpperLineColor != value )
                {
                    m_XPTaskBarBoxHeaderUpperLineColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="XPTaskBar">.
        /// </summary>
        public Color XPTaskBarBoxHeaderLowerLineColor
        {
            get
            {
                return m_XPTaskBarBoxHeaderLowerLineColor;
            }
            set
            {
                if( m_XPTaskBarBoxHeaderLowerLineColor != value )
                {
                    m_XPTaskBarBoxHeaderLowerLineColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="XPTaskBar">.
        /// </summary>
        public Color XPTaskBarBoxArrowColor
        {
            get
            {
                return m_XPTaskBarBoxArrowColor;
            }
            set
            {
                if( m_XPTaskBarBoxArrowColor != value )
                {
                    m_XPTaskBarBoxArrowColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="XPTaskBar">.
        /// </summary>
        public Color XPTaskBarBoxActiveHighlightedItemColor
        {
            get
            {
                return m_XPTaskBarBoxActiveHighlightedItemColor;
            }
            set
            {
                if( m_XPTaskBarBoxActiveHighlightedItemColor != value )
                {
                    m_XPTaskBarBoxActiveHighlightedItemColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="XPTaskBar">.
        /// </summary>
        public Color XPTaskBarBoxInactiveHighlightedItemColor
        {
            get
            {
                return m_XPTaskBarBoxInactiveHighlightedItemColor;
            }
            set
            {
                if( m_XPTaskBarBoxInactiveHighlightedItemColor != value )
                {
                    m_XPTaskBarBoxInactiveHighlightedItemColor = value;
                }
            }
        }
        #endregion

		#region ColorUIAdv properties

		/// <summary>
		/// Used in drawing of <see cref="ColorUIAdv">.
		/// </summary>
		public Color ColorUIAdvBackColor
		{
			get
			{
				return m_ColorUIAdvBackColor;
			}
			set
			{
				if( m_ColorUIAdvBackColor != value )
				{
					m_ColorUIAdvBackColor = value;
				}
			}
		}

		/// <summary>
		/// Used in drawing of <see cref="ColorUIAdv">.
		/// </summary>
		public Color ColorUIAdvTextColor
		{
			get
			{
				return m_ColorUIAdvTextColor;
			}
			set
			{
				if( m_ColorUIAdvTextColor != value )
				{
					m_ColorUIAdvTextColor = value;
				}
			}
		}

		/// <summary>
		/// Used in drawing of <see cref="ColorUIAdv">.
		/// </summary>
		public Color ColorUIAdvItemBorderColor
		{
			get
			{
				return m_ColorUIAdvItemBorderColor;
			}
			set
			{
				if( m_ColorUIAdvItemBorderColor != value )
				{
					m_ColorUIAdvItemBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Used in drawing of <see cref="ColorUIAdv">.
		/// </summary>
		public Color ColorUIAdvHighlightedBorderColor
		{
			get
			{
				return m_ColorUIAdvHighlightedBorderColor;
			}
			set
			{
				if( m_ColorUIAdvHighlightedBorderColor != value )
				{
					m_ColorUIAdvHighlightedBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Used in drawing of <see cref="ColorUIAdv">.
		/// </summary>
		public Color ColorUIAdvSelectedBorderColor
		{
			get
			{
				return m_ColorUIAdvSelectedBorderColor;
			}
			set
			{
				if( m_ColorUIAdvSelectedBorderColor != value )
				{
					m_ColorUIAdvSelectedBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Used in drawing of <see cref="ColorUIAdv">.
		/// </summary>
		public Color ColorUIAdvSelectedHighlightedBorderColor
		{
			get
			{
				return m_ColorUIAdvSelectedHighlightedBorderColor;
			}
			set
			{
				if( m_ColorUIAdvSelectedHighlightedBorderColor != value )
				{
					m_ColorUIAdvSelectedHighlightedBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Used in drawing of <see cref="ColorUIAdv">.
		/// </summary>
		public Color ColorUIAdvGroupHeaderBackColor
		{
			get
			{
				return m_ColorUIAdvGroupHeaderBackColor;
			}
			set
			{
				if( m_ColorUIAdvGroupHeaderBackColor != value )
				{
					m_ColorUIAdvGroupHeaderBackColor = value;
				}
			}
		}

        public Color StatusBarExtTopGradient
        {
            get
            {
                return this.m_StatusBarExtTopGradient;
            }
            set
            {
                if (!(this.m_StatusBarExtTopGradient != value))
                    return;
                this.m_StatusBarExtTopGradient = value;
            }
        }

        public Color StatusBarExtBottomGradient
        {
            get
            {
                return this.m_StatusBarExtBottomGradient;
            }
            set
            {
                if (!(this.m_StatusBarExtBottomGradient != value))
                    return;
                this.m_StatusBarExtBottomGradient = value;
            }
        }

        public Color StatusBarExtFillColor
        {
            get
            {
                return this.m_StatusBarExtFillColor;
            }
            set
            {
                if (!(this.m_StatusBarExtFillColor != value))
                    return;
                this.m_StatusBarExtFillColor = value;
            }
        }
		#endregion

		#endregion

		#region Class Utility Methods
		/// <summary>
		/// Initialize colors general for all colorscheme of the Office2007 visual style.
		/// </summary>
		protected virtual void InitializeColors()
		{
			// colors for menu
			m_MenuSeparatorColor = Color.FromArgb(197, 197, 197);
			m_MenuBorderColor = Color.FromArgb(134, 134, 134);
			m_MenuColumnColor = Color.FromArgb(233, 238, 238);
			m_MenuColumnSeparatorColor = Color.FromArgb(197, 197, 197);
			m_MenuItemBorderColor = Color.FromArgb(150, 175, 142, 80);
			m_MenuItemDarkColor = Color.FromArgb(255, 213, 92);
			m_MenuItemLightColor = Color.FromArgb(255, 253, 233);
			m_MenuCheckedColor = Color.FromArgb(52, 55, 124);
			m_MenuCheckedFillColor = Color.FromArgb(255, 227, 149);
			m_MenuCheckedBorderColor = Color.FromArgb(242, 149, 54);
			m_MenuComboButtonPushed1Color = Color.FromArgb(234, 224, 191);
			m_MenuComboButtonPushed2Color = Color.FromArgb(239, 189, 119);
			m_MenuComboButtonPushed3Color = Color.FromArgb(255, 168, 56);
			m_MenuComboButtonPushed4Color = Color.FromArgb(255, 212, 86);

			// colors for BarItem in CommandBar
			m_BarItemPressBorderColor = Color.FromArgb(139, 118, 84);
			m_BarItemHighlightBorderColor = Color.FromArgb(255, 185, 160, 116);
			m_BarItemPressLightColor = Color.FromArgb(255, 197, 108);
			m_BarItemPressDarkColor = Color.FromArgb(251, 138, 59);
			m_DropDownBarItemLightColor = Color.FromArgb(234, 207, 178);
			m_DropDownBarItemDarkColor = Color.FromArgb(248, 171, 72);
			m_DropDownBarItemBorderColor = Color.FromArgb(158, 130, 85);
			m_BarItemCheckLightColor = Color.FromArgb(253, 210, 168);
			m_BarItemCheckDarkColor = Color.FromArgb(249, 147, 47);
			m_BarItemCheckBorderColor = Color.FromArgb(160, 131, 85);
			m_BarItemCheckFlashColor = Color.FromArgb(255, 253, 241, 176);
			m_BarItemPressFlashColor = Color.FromArgb(255, 255, 208, 134);
			m_BarItemSelectFlashColor = Color.FromArgb(255, 255, 235, 174);
			m_TextBarItemBackColor = SystemColors.Window;
			m_TextBarItemBorderColor = SystemColors.Window;
			m_TextBarItemBorderHighlightColor = Color.FromArgb(139, 118, 84);

			// colors for ComboButton of the ComboBoxBarItem in CommandBar.
			m_ComboButtonPressLightColor = Color.FromArgb(255, 197, 108);
			m_ComboButtonPressDarkColor = Color.FromArgb(251, 138, 59);
			m_ComboButtonHighlightLightColor = Color.FromArgb(255, 252, 217);
			m_ComboButtonHighlightDarkColor = Color.FromArgb(255, 214, 70);
			m_ComboButtonPressBorder = Color.FromArgb(139, 118, 84);
			m_ComboButtonHighlightBorder = Color.FromArgb(128, 185, 160, 116);

			// colors for tab group
			m_TabItemBorderColor = Color.FromArgb(153, 187, 232);
			m_TabItemInnerBorderColor = Color.FromArgb(239, 246, 255);
			m_TabItemOuterBorderColor = Color.FromArgb(209, 229, 254);
			m_TabItemTextColor = Color.FromArgb(21, 66, 139);
			m_TabItemActiveBottomColor = Color.FromArgb(225, 210, 163);
			m_TabItemTopGradientColor = Color.FromArgb(196, 221, 254);
			m_TabItemInActiveBottomColor = Color.FromArgb(235, 243, 253);
			m_TabItemMiddleLineColor = Color.FromArgb(215, 226, 232);
			m_TabPanelColor = Color.FromArgb(199, 216, 237);
			m_TabPanelBorderColor = Color.FromArgb(219, 232, 249);
			m_TabPanelBackColor = Color.FromArgb(199, 216, 237);

			// colors for GroupBar
			m_GroupBarHighlightColorLight = Color.FromArgb(255, 255, 231);
			m_GroupBarHighlightColorDark = Color.FromArgb(255, 215, 107);
			m_GroupBarSelectedColorLight = Color.FromArgb(255, 227, 123);
			m_GroupBarSelectedColorDark = Color.FromArgb(255, 170, 57);
			m_GroupBarSelectedTopColorLight = Color.FromArgb(255, 219, 173);
			m_GroupBarSelectedTopColorDark = Color.FromArgb(255, 190, 115);
			m_GroupBarSelectedHighlightColorLight = Color.FromArgb(255, 211, 99);
			m_GroupBarSelectedHighlightColorDark = Color.FromArgb(255, 142, 66);

			// colors for DataTimePicker
			m_DataTimePickerHighLightedBorderColor = Color.FromArgb(219, 206, 153);
			m_DataTimePickerSelectedBorderColor = Color.FromArgb(166, 155, 107);
			
			m_DataTimePickerDropDownHighLightLightColor = Color.FromArgb(255, 252, 217);
			m_DataTimePickerDropDownHighLightDarkColor = Color.FromArgb(255, 214, 70);
			m_DataTimePickerDropDownSelectedLightColor = Color.FromArgb(255, 250, 243);
			m_DataTimePickerDropDownSelectedDarkColor = Color.FromArgb(255, 168, 56);

			m_DataTimePickerCheckBoxNormalColor = Color.FromArgb(74, 93, 148);
			m_DataTimePickerCheckBoxSelectedColor = Color.FromArgb(0, 32, 115);
			m_DataTimePickerCheckBoxInnerRectBorderNormalColor = Color.FromArgb(173, 178, 189);
            m_DataTimePickerCheckBoxInnerRectBorderSelectedColor = Color.FromArgb(253, 203, 87);
			m_DataTimePickerCheckBoxInnerRectBorderPushedColor = Color.FromArgb(241, 138, 35);
			m_DataTimePickerCheckBoxInnerRectFillNormalColor = Color.FromArgb(206, 207, 214);
            m_DataTimePickerCheckBoxInnerRectFillSelectedColor = Color.FromArgb(250, 221, 143);
            m_DataTimePickerCheckBoxInnerRectFillPushedColor = Color.FromArgb(255, 206, 103);

			m_DataTimePickerHighLightedForeColor = Color.FromArgb(0, 101, 206);

			//NumericUpDownExt colors
			m_NumericUpDownBorderColor = Color.FromArgb(173, 174, 181);
			m_NumericUpDownHighLightedBorderColor = Color.FromArgb(57, 125, 181);
			m_NumericUpDownSelectedBorderColor = Color.FromArgb(41, 97, 140);
			m_NumericUpDownArrowLightColor = Color.FromArgb(115, 134, 214);
			m_NumericUpDownArrowDarkColor = Color.FromArgb(74, 85, 123);

			// colors for TabControlAdv
			m_TabDefaultBorderColor = Color.FromArgb(141, 178, 227);
			m_TabHotLightBottomBorderLineColor = Color.FromArgb(208, 195, 146);
			m_TabHotLightGradientTopBeginColor = Color.FromArgb(205, 217, 224);
			m_TabHotLightGradientTopEndColor = Color.FromArgb(228, 230, 222);
			m_TabHotLightGradientBottomBeginColor = Color.FromArgb(221, 221, 208);
			m_TabHotLightGradientBottomEndColor = Color.FromArgb(223, 213, 177);
			m_TabHotLightGradientCircleColor = Color.FromArgb(196, 221, 254);
			m_TabSelectedGradientTopColor = Color.FromArgb(240, 246, 254);
			m_TabSelectedGradientBottomColor = Color.FromArgb(225, 235, 246);
			m_TabSelectedInnerBorderColor = Color.FromArgb(235, 243, 252);
			m_TabHighlightInnerBorderColor = Color.FromArgb(234, 237, 253);
			m_TabSelectedHotLightBorderColor = Color.FromArgb(255, 208, 48);
			m_TabSelectedHotLightInnerBorderColor = Color.FromArgb(255, 240, 187);
			m_TabForeColor = Color.FromArgb(21, 66, 139);
			m_ActiveTabForeColor = m_TabForeColor;
			m_TabBackgroundColor = Color.FromArgb(199, 216, 237);

			// colors for DockTabControl 
			m_DockTabForeColor = Color.FromArgb(21, 66, 139);
			m_DockTabBackgroundColor = Color.FromArgb(199, 216, 237);

			// UpDown colors
			m_UpDownBorderHotColor = Color.FromArgb(221, 204, 155);
			m_UpDownInnerBorderHotStartColor = Color.FromArgb(254, 254, 254);
			m_UpDownInnerBorderHotEndColor = Color.FromArgb(253, 249, 245);

			m_UpDownBorderPressedColor = Color.FromArgb(152, 143, 103);
			m_UpDownInnerBorderPressedStartColor = Color.FromArgb(193, 180, 168);
			m_UpDownInnerBorderPressedEndColor = Color.FromArgb(255, 223, 141);

			m_UpDownBackgroundDisabledStartColor = Color.FromArgb(244, 244, 244);
			m_UpDownBackgroundDisabledEndColor = Color.FromArgb(201, 201, 201);
			m_UpDownBorderDisabledColor = Color.FromArgb(200, 200, 200);

			m_UpDownBackgroundHotTopStartColor = Color.FromArgb(255, 248, 224);
			m_UpDownBackgroundHotTopEndColor = Color.FromArgb(254, 242, 180);
			m_UpDownBackgroundHotBottomStartColor = Color.FromArgb(255, 214, 119);
			m_UpDownBackgroundHotBottomEndColor = Color.FromArgb(254, 221, 139);

			m_UpDownBackgroundPressedTopStartColor = Color.FromArgb(193, 180, 168);
			m_UpDownBackgroundPressedTopEndColor = Color.FromArgb(231, 214, 182);
			m_UpDownBackgroundPressedBottomStartColor = Color.FromArgb(255, 165, 59);
			m_UpDownBackgroundPressedBottomEndColor = Color.FromArgb(255, 190, 71);

			// ColorUIAdv colors
			m_ColorUIAdvBackColor = Color.FromArgb( 250, 250, 250 );
			m_ColorUIAdvTextColor = Color.FromArgb( 2, 22, 109 );
			m_ColorUIAdvItemBorderColor = Color.FromArgb( 197, 197, 197 );
			m_ColorUIAdvHighlightedBorderColor = Color.FromArgb( 243, 148, 54 );
			m_ColorUIAdvSelectedBorderColor = Color.FromArgb( 235, 75, 13 );
			m_ColorUIAdvSelectedHighlightedBorderColor = Color.FromArgb( 255, 226, 148 );
			m_ColorUIAdvGroupHeaderBackColor = Color.FromArgb( 235, 235, 235 );

			// colors for ButtonAdv
			m_ButtonPressedTopColor = Color.FromArgb(255, 197, 108);
			m_ButtonPressedBottomColor = Color.FromArgb(251, 138, 59);
			m_ButtonSelectedTopColor = Color.FromArgb(255, 252, 217);
			m_ButtonSelectedBottomColor = Color.FromArgb(255, 214, 70);
			m_ButtonDisabledTopColor = Color.FromArgb(244, 244, 244);
			m_ButtonDisabledBottomColor = Color.FromArgb(201, 201, 201);
			m_ButtonPressedBorderColor = Color.FromArgb(139, 118, 84);
			m_ButtonSelectedBorderColor = Color.FromArgb(185, 160, 116);
			m_ButtonDisabledBorderColor = Color.FromArgb(156, 164, 173);

            //StatusBarExt
            this.m_StatusBarExtTopGradient = Color.FromArgb(182, 209, 245);
            this.m_StatusBarExtBottomGradient = Color.FromArgb(64, 77, 140);
            this.m_StatusBarExtFillColor = Color.FromArgb(180, 205, 240);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="basicColor"></param>
		internal void UpdateColors( Color basicColor )
		{
			InitializeColors();

			Office2007Colors silverColors = GetColorTable( Office2007Theme.Silver );
			Type type = silverColors.GetType();
			PropertyInfo[] propInfoes = type.GetProperties( BindingFlags.Instance | BindingFlags.Public );

			foreach( PropertyInfo pi in propInfoes )
			{
				if( pi.PropertyType == typeof( Color ) )
				{
					string sFieldName = "m_" + pi.Name;
					FieldInfo fi = type.GetField( sFieldName, BindingFlags.Instance | BindingFlags.NonPublic );

					if( fi != null )
					{
						fi.SetValue( this, MergeColors( (Color)pi.GetValue( silverColors, null ), basicColor ) );
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="scheme"></param>
		internal void UpdateScheme(Office2007Theme scheme)
		{
			switch (scheme)
			{
				case Office2007Theme.Blue:
					s_managedColors = new WeakReference(new Office2007BlueColors());
					break;
				case Office2007Theme.Silver:
					s_managedColors = new WeakReference(new Office2007SilverColors());
					break;
				case Office2007Theme.Black:
					s_managedColors = new WeakReference(new Office2007BlackColors());
					break;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="baseColor"></param>
		/// <param name="blendColor"></param>
		/// <returns></returns>
		internal static Color MergeColors(Color baseColor, Color blendColor)
		{
			int r = MergeChannels(baseColor.R, blendColor.R);
			int g = MergeChannels(baseColor.G, blendColor.G);
			int b = MergeChannels(baseColor.B, blendColor.B);
			
			return Color.FromArgb(r,g,b);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="baseChannel"></param>
		/// <param name="blendChannel"></param>
		/// <returns></returns>
		private static int MergeChannels(int baseChannel, int blendChannel)
		{
			const int MAX = 255;

			int min = (baseChannel * blendChannel) / MAX;
			int max = MAX - ((MAX - baseChannel) * (MAX - blendChannel)) / MAX;

			return (byte)(min + (baseChannel * (max - min)) / MAX);
		}
		#endregion

		#region ICloneable

		public virtual object Clone()
		{
			return (Office2007Colors)this.MemberwiseClone();
		}

		#endregion
	}

	/// <summary>
	/// Provides colors for blue colorscheme of the Office2007 visual style.
	/// </summary>
	public class Office2007BlueColors : Office2007Colors
	{
		#region Class Overrides
		/// <summary>
		/// Initialize colors for Blue colorscheme of the Office2007 visual style.
		/// </summary>
		protected override void InitializeColors()
		{
			base.InitializeColors();

            // colors for TextBoxExt 
            m_ActiveTextBoxBorderColor = Color.FromArgb(255, 230, 158);
            m_InactiveTextBoxBorderColor = Color.FromArgb(188, 208, 229);
            m_ActiveTextBoxBackColor = Color.FromArgb(255, 255, 255);
            m_InactiveTextBoxBackColor = Color.FromArgb(234, 242, 251);

            //TreeviewAdv
            m_SelectedNodeBackground = Color.FromArgb(222, 235, 251);
            m_TreeNodeArrowColor = Color.FromArgb(178, 208, 252);
            m_TreeviewBackColor = Color.FromArgb(255, 255, 255);
            m_TreeViewFontColor = Color.FromArgb(30, 57, 91);

			// colors for menu
			m_MenuTextBoxBorderColor = Color.FromArgb(179, 199, 225);
			m_MenuTextBoxBackColor = Color.FromArgb(234, 242, 251);
			m_MenuComboButtonHighlightLightColor = Color.FromArgb(232, 241, 253);
			m_MenuComboButtonHighlightDarkColor = Color.FromArgb(207, 223, 243);
			m_MenuComboButtonArrowColor = Color.FromArgb(86, 125, 177);
			m_MenuItemArrowLightColor = Color.FromArgb(106, 126, 197);
			m_MenuItemArrowDarkColor = Color.FromArgb(64, 70, 90);

			// colors for CommandBar
			m_CommandBarDarkColor = Color.FromArgb(177, 211, 255);
			m_CommandBarLightColor = Color.FromArgb(227, 239, 255);
			m_CommandBarBorderColor = Color.FromArgb(111, 157, 217);

			// colors for DropDown button of the CommandBar
			m_DockBarBackColor = Color.FromArgb(191, 219, 255);
			m_DropDownDarkColor = Color.FromArgb(111, 157, 217);
			m_DropDownLightColor = Color.FromArgb(215, 232, 255);
			m_DropDownHighlightLightColor = Color.FromArgb(255, 248, 237);
			m_DropDownHighlightDarkColor = Color.FromArgb(255, 193, 118);
			m_DropDownPressedLightColor = Color.FromArgb(254, 149, 82);
			m_DropDownPressedDarkColor = Color.FromArgb(255, 217, 149);

			// colors for floating CommandBar
			m_FloatHighlightButtonColor = Color.FromArgb(255, 231, 162);
			m_FloatHighlightButtonBorderColor = Color.FromArgb(255, 189, 105);
			m_FloatPressButtonColor = Color.FromArgb(214, 232, 255);
			m_FloatPressButtonBorderColor = Color.FromArgb(101, 147, 207);
			m_FloatPressCloseButtonBorderColor = Color.FromArgb(251, 140, 60);
			m_FloatPressCloseButtonColor = Color.FromArgb(251, 140, 60);
			m_FloatCommandBarLightColor = Color.FromArgb(227, 239, 255);
			m_FloatCommandBarDarkColor = Color.FromArgb(186, 216, 255);
			m_FloatLightBorderColor = Color.FromArgb(194, 220, 255);
			m_FloatBackgroundColor = Color.FromArgb(55, 100, 160);
			m_FloatBorderColor = Color.FromArgb(55, 100, 160);
			m_FloatCaptionColor = Color.White;

			// color for separator in a CommandBar
			m_BarItemSeparatorColor = Color.FromArgb(154, 198, 255);

			// colors for ComboButton of the ComboBoxBarItem in CommandBar
			m_ComboButtonLightColor = Color.FromArgb(201, 221, 246);
			m_ComboButtonDarkColor = Color.FromArgb(160, 189, 224);
			m_ComboButtonBorder = Color.FromArgb(138, 173, 219);

			// colors for tab group
			m_TabItemBorderColor = Color.FromArgb(153, 187, 232);
			m_TabItemInnerBorderColor = Color.FromArgb(239, 246, 255);
			m_TabItemOuterBorderColor = Color.FromArgb(209, 229, 254);
			m_TabItemTextColor = Color.FromArgb(21, 66, 139);
			m_TabItemActiveBottomColor = Color.FromArgb(239, 211, 156);
			m_TabItemTopGradientColor = Color.FromArgb(196, 221, 254);
			m_TabItemInActiveBottomColor = Color.FromArgb(235, 243, 253);
			m_TabItemMiddleLineColor = Color.FromArgb(215, 226, 232);
			m_TabPanelColor = Color.FromArgb(199, 216, 237);
			m_TabPanelBorderColor = Color.FromArgb(219, 232, 249);
			m_TabPanelBackColor = Color.FromArgb(199, 216, 237);

			// colors for GroupBar
			m_GroupBarBorderColor = Color.FromArgb(99, 146, 206);
			m_GroupBarHeaderColorLight = Color.FromArgb(231, 243, 255);
			m_GroupBarHeaderColorDark = Color.FromArgb(181, 215, 255);
			m_GroupBarItemTextColor = Color.FromArgb(33, 77, 140);
			m_GroupBarHeaderTextColor = Color.FromArgb(16, 65, 140);
			m_GroupBarItemColorLight = m_GroupBarHeaderColorLight;
			m_GroupBarItemColorDark = m_GroupBarHeaderColorDark;
			m_GroupBarSplitterColorDark = Color.FromArgb(189, 219, 255);
			m_GroupBarSplitterColorLight = Color.FromArgb(239, 243, 255);
			m_GroupBarClientAreaBackground = Color.FromArgb( 213, 228, 242 );

            // colors for DataTimePicker
            m_DataTimePickerBorderColor = Color.FromArgb(171, 193, 222);

            m_DataTimePickerDropDownArrowColor = Color.FromArgb(86, 125, 177);
            m_DataTimePickerDropDownLightColor = Color.FromArgb(252, 253, 254);
            m_DataTimePickerDropDownDarkColor = Color.FromArgb(181, 203, 232);

            m_DataTimePickerCheckBoxBorderNormalColor = Color.FromArgb(171, 193, 222);
            m_DataTimePickerCheckBoxBorderPushedColor = Color.FromArgb(85, 119, 163);           
            
            //colors for MonthCalendarAdv
            m_MonthCalendarHeaderStartColor = Color.FromArgb(179, 209, 252);
            m_MonthCalendarHeaderEndColor = Color.FromArgb(231, 242, 255);
            m_MonthCalendarForeColor = SystemColors.ControlText;

            // colors for XPTaskPane
            m_XPTaskPaneInternalBorderColor = Color.FromArgb(221, 237, 253);
            m_XPTaskPaneBorderColor = Color.FromArgb(145, 183, 249);
            m_XPTaskPageBackColor = Color.FromArgb(221, 237, 253);
			
			// colors for TabControlAdv
			m_TabDefaultBorderColor = Color.FromArgb(141, 178, 227);
			m_TabHotLightBottomBorderLineColor = Color.FromArgb(208, 195, 146);
			m_TabHotLightGradientTopBeginColor = Color.FromArgb(205, 217, 224);
			m_TabHotLightGradientTopEndColor = Color.FromArgb(228, 230, 222);
			m_TabHotLightGradientBottomBeginColor = Color.FromArgb(221, 221, 208);
			m_TabHotLightGradientBottomEndColor = Color.FromArgb(223, 213, 177);
			m_TabHotLightGradientCircleColor = Color.FromArgb(196, 221, 254);
			m_TabSelectedGradientTopColor = Color.FromArgb(240, 246, 254);
			m_TabSelectedGradientBottomColor = Color.FromArgb(225, 235, 246);
			m_TabSelectedInnerBorderColor = Color.FromArgb(235, 243, 252);
			m_TabHighlightInnerBorderColor = Color.FromArgb(234, 237, 253);
			m_TabSelectedHotLightBorderColor = Color.FromArgb(255, 208, 48);
			m_TabSelectedHotLightInnerBorderColor = Color.FromArgb(255, 240, 187);
			m_TabForeColor = Color.FromArgb(21, 66, 139);
			m_ActiveTabForeColor = m_TabForeColor;
			m_TabBackgroundColor = Color.FromArgb(199, 216, 237);
			m_TabScrollArrowColor = Color.FromArgb(86, 125, 177);

			// colors for DockTabControl 
			m_DockTabForeColor = Color.FromArgb(21, 66, 139);
			m_DockTabBackgroundColor = Color.FromArgb(199, 216, 237);

			// form's colors
			m_ActiveFormBorderColor = Color.FromArgb(191, 219, 254);
			m_InactiveFormBorderColor = Color.FromArgb(204, 216, 232);

			m_FormTextColor = Color.FromArgb(57, 105, 173);

			m_ActiveTitleGradientBegin = Color.FromArgb(231, 239, 255);
			m_ActiveTitleGradientEnd = Color.FromArgb(203, 223, 244);
			m_InactiveTitleGradientBegin = Color.FromArgb(229, 233, 237);
			m_InactiveTitleGradientEnd = Color.FromArgb(217, 225, 233);

			m_SystemButtonSelectedGradientBegin = Color.FromArgb(251, 253, 255);
			m_SystemButtonSelectedGradientEnd = Color.FromArgb(210, 228, 254);
			m_SystemButtonPressedGradientBegin = Color.FromArgb(182, 205, 231);
			m_SystemButtonPressedGradientEnd = Color.FromArgb(132, 178, 233);

			m_SystemButtonBorderSelected = Color.FromArgb(192, 212, 237);
			m_SystemButtonBorderPressed = Color.FromArgb(161, 190, 228);
			m_FormBackground = Color.FromArgb( 187, 212, 246 );

			// upDown colors
			m_UpDownArrowStartColor = Color.FromArgb(24, 82, 172);
			m_UpDownArrowEndColor = Color.FromArgb(13, 50, 103);

			m_UpDownBorderNormalColor = Color.FromArgb(177, 197, 218);

			m_UpDownBackgroundNormalColor = Color.FromArgb(234, 242, 251);
			m_UpDownBackgroundNormalStartColor = Color.FromArgb(207, 223, 243);
			m_UpDownBackgroundNormalEndColor = Color.FromArgb(231, 241, 253);

            // colors for comboBoxAdv
            m_ComboBoxAdvNormalBackColor = SystemColors.Window;
            m_ComboBoxAdvHotBackColor = Color.FromArgb(234, 242, 251);
            m_ComboBoxAdvNormalBorderColor = Color.FromArgb(171, 193, 222);
            m_ComboBoxAdvHotBorderColor = Color.FromArgb(219, 206, 153);
            m_ComboBoxAdvPushedBorderColor = Color.FromArgb(157, 146, 102);
            m_ComboBoxAdvButtonUpperLineColor = Color.FromArgb(192, 185, 165);
            m_ComboBoxAdvArrowColor = Color.FromArgb(86, 125, 177);
            m_ComboBoxAdvLowerArrowLineColor = Color.FromArgb(255, 248, 203);
            m_ComboBoxAdvHotBackgroundButtonColor1 = Color.FromArgb(255, 252, 226);
            m_ComboBoxAdvHotBackgroundButtonColor2 = Color.FromArgb(255, 232, 150);
            m_ComboBoxAdvHotBackgroundButtonColor3 = Color.FromArgb(255, 217, 117);
            m_ComboBoxAdvHotBackgroundButtonColor4 = Color.FromArgb(255, 231, 165);
            m_ComboBoxAdvNormalBackgroundButtonColor1 = Color.FromArgb(228, 240, 254);
            m_ComboBoxAdvNormalBackgroundButtonColor2 = Color.FromArgb(218, 231, 249);
            m_ComboBoxAdvNormalBackgroundButtonColor3 = Color.FromArgb(207, 223, 243);
            m_ComboBoxAdvNormalBackgroundButtonColor4 = Color.FromArgb(232, 241, 253);
            m_ComboBoxAdvPushedBackgroundButtonColor1 = Color.FromArgb(224, 212, 178);
            m_ComboBoxAdvPushedBackgroundButtonColor2 = Color.FromArgb(239, 189, 119);
            m_ComboBoxAdvPushedBackgroundButtonColor3 = Color.FromArgb(255, 168, 56);
            m_ComboBoxAdvPushedBackgroundButtonColor4 = Color.FromArgb(255, 230, 148);

            // colors for CheckBoxAdv
            m_CheckBoxAdvNormalBackColor = Color.FromArgb(248, 248, 248);
            m_CheckBoxAdvSelectedBackColor = Color.FromArgb(254, 248, 232);
            m_CheckBoxAdvPushedBackColor = Color.FromArgb(255, 244, 213);
            m_CheckBoxAdvNormalBorderColor = Color.FromArgb(171, 193, 222);
            m_CheckBoxAdvSelectedBorderColor = Color.FromArgb(85, 119, 163);
            m_CheckBoxAdvPushedBorderColor = Color.FromArgb(85, 119, 163);
            m_CheckBoxAdvNormalInternalBorderColor = Color.FromArgb(244, 244, 244);
            m_CheckBoxAdvSelectedInternalBorderColor = Color.FromArgb(222, 234, 250);
            m_CheckBoxAdvPushedInternalBorderColor = Color.FromArgb(193, 216, 245);
            m_CheckBoxAdvNormalInternalRectangleBorderColor = Color.FromArgb(162, 172, 185);
            m_CheckBoxAdvSelectedInternalRectangleBorderColor = Color.FromArgb(250, 213, 122);
            m_CheckBoxAdvPushedInternalRectangleBorderColor = Color.FromArgb(242, 137, 38);
            m_CheckBoxAdvNormalInternalRectangleColor = Color.FromArgb(202, 207, 213);
            m_CheckBoxAdvSelectedInternalRectangleColor = Color.FromArgb(252, 231, 175);
            m_CheckBoxAdvPushedInternalRectangleColor = Color.FromArgb(255, 208, 103);
            m_CheckBoxAdvNormalTickColor = Color.FromArgb(74, 107, 150);
            m_CheckBoxAdvSelectedTickColor = Color.FromArgb(78, 108, 141);
            m_CheckBoxAdvPushedTickColor = Color.FromArgb(79, 105, 130);
            m_CheckBoxAdvDisabledTickColor = Color.FromArgb(176, 190, 208);
            m_CheckBoxAdvIndeterminateRectangleColor = Color.FromArgb(158, 168, 178);
            m_CheckBoxAdvDisabledBackColor = Color.FromArgb(238, 240, 242);
            m_CheckBoxAdvDisabledBorderColor = Color.FromArgb(174, 177, 181);
            m_CheckBoxAdvDisabledInternalBorderColor = Color.FromArgb(224, 226, 229);

            // colors for RadioButtonAdv
            m_RadioButtonAdvNormalBackColor = SystemColors.Window;
            m_RadioButtonAdvNormalBorderColor = Color.FromArgb(148, 175, 214);
            m_RadioButtonAdvNormalInternalBorderColor = Color.FromArgb(162, 172, 185);
            m_RadioButtonAdvSelectedBackColor = Color.FromArgb(227, 252, 255);
            m_RadioButtonAdvSelectedBorderColor = Color.FromArgb(85, 119, 163);
            m_RadioButtonAdvSelectedInternalBorderColor = Color.FromArgb(250, 205, 101);
            m_RadioButtonAdvPushedBackColor = Color.FromArgb(205, 242, 255);
            m_RadioButtonAdvPushedBorderColor = Color.FromArgb(85, 119, 163);
            m_RadioButtonAdvPushedInternalBorderColor = Color.FromArgb(244, 171, 14);
            m_RadioButtonAdvCheckMarkBorderColor = Color.FromArgb(17, 69, 103);
            m_RadioButtonAdvCheckMarkNormalBottomColor = Color.FromArgb(11, 130, 199);
            m_RadioButtonAdvCheckMarkSelectedBottomColor = Color.FromArgb(13, 160, 243);
            m_RadioButtonAdvCheckMarkPushedBottomColor = Color.FromArgb(7, 84, 131);

            // colors for TabBarSplitterControl
            m_TabBarSplitterBackColor = Color.FromArgb( 231, 243, 255 );
            m_TabBarSplitterBorderColor = Color.FromArgb( 148, 166, 198 );
            m_TabBarSplitterTextColor = Color.FromArgb( 16, 65, 140 );
            m_TabBarSplitterTabStartColor = Color.FromArgb( 222, 231, 255 );
            m_TabBarSplitterTabEndColor = Color.FromArgb( 189, 211, 247 );
            m_TabBarSplitterTabBarStartColor = Color.FromArgb( 165, 190, 231 );
            m_TabBarSplitterTabBarEndColor = Color.FromArgb( 132, 166, 214 );
            m_TabBarSplitterButtonHoveredStartColor = Color.White;
            m_TabBarSplitterButtonHoveredEndColor = Color.FromArgb( 222, 231, 247 );
            m_TabBarSplitterButtonPushedStartColor = Color.FromArgb( 206, 223, 255 );
            m_TabBarSplitterButtonPushedEndColor = Color.FromArgb( 148, 182, 239 );
            m_TabBarSplitterSizeGripperColor = Color.FromArgb( 127, 163, 211 );
            m_TabBarSplitterSizeGripperLightColor = Color.FromArgb( 177, 201, 232 );
            m_TabBarSplitterSizeGripperDarkColor = Color.FromArgb( 69, 93, 128 );

            //colors for XPTaskBar
            m_XPTaskBarBorderColor = Color.FromArgb(101, 147, 207);
            m_XPTaskBarBoxBackColor = Color.FromArgb(214, 232, 255);
            m_XPTaskBarBoxForeColor = Color.FromArgb(21, 66, 139);
            m_XPTaskBarBoxHeaderLowerLineColor = Color.FromArgb(173, 209, 255);
            m_XPTaskBarBoxHeaderUpperLineColor = Color.FromArgb(101, 147, 207);
            m_XPTaskBarBoxArrowColor = Color.FromArgb(85, 125, 177);
            m_XPTaskBarBoxActiveHighlightedItemColor = Color.FromArgb(255, 232, 156);
            m_XPTaskBarBoxInactiveHighlightedItemColor = Color.FromArgb( 229, 229, 229 );

			// colors for ColorUIAdv
			m_ColorUIAdvGroupHeaderBackColor = Color.FromArgb( 222, 230, 238 );

			// colors for ButtonAdv
			m_ButtonDefaultTopColor = Color.FromArgb( 231, 242, 255 );
			m_ButtonDefaultBottomColor = Color.FromArgb( 179, 209, 252 );
			m_ButtonDefaultBorderColor = Color.FromArgb( 176, 208, 255 );
			m_ButtonDefaultInternalBorderColor = Color.FromArgb( 128, Color.FromArgb( 250, 251, 255 ) );
			m_ButtonPressedInternalBorderColor = Color.FromArgb( 70, Color.FromArgb( 176, 132, 92 ) );
			m_ButtonSelectedInternalBorderColor = Color.FromArgb( 128, Color.FromArgb( 250, 251, 255 ) );
		}
		#endregion
	}

	/// <summary>
	/// Provides colors for silver colorscheme of the Office2007 visual style.
	/// </summary>
	public class Office2007SilverColors : Office2007Colors
	{
		#region Class Overrides
		/// <summary>
		/// Initialize colors for Silver colorscheme of the Office2007 visual style.
		/// </summary>
		protected override void InitializeColors()
		{
			base.InitializeColors ();

            // colors for TextBoxExt 
            m_ActiveTextBoxBorderColor = Color.FromArgb(255, 230, 158);
            m_InactiveTextBoxBorderColor = Color.FromArgb(164, 171, 178);
            m_ActiveTextBoxBackColor = Color.FromArgb(255, 255, 255);
            m_InactiveTextBoxBackColor = Color.FromArgb(232, 234, 236);

            //TreeviewAdv
            m_SelectedNodeBackground = Color.FromArgb(221, 225, 230);
            m_TreeNodeArrowColor = Color.FromArgb(129, 129, 129);
            m_TreeviewBackColor = Color.FromArgb(255, 255, 255);
            m_TreeViewFontColor = Color.FromArgb(33, 39, 35);

			// colors for menu
			m_MenuTextBoxBorderColor = Color.FromArgb( 169, 177, 184 );
			m_MenuTextBoxBackColor = Color.FromArgb( 232, 234, 236 );
			m_MenuComboButtonHighlightLightColor = Color.FromArgb( 239, 242, 246 );
			m_MenuComboButtonHighlightDarkColor = Color.FromArgb( 218, 224, 231 );
			m_MenuComboButtonArrowColor = Color.FromArgb( 124, 124, 124 );
			m_MenuItemArrowLightColor = Color.FromArgb( 158, 158, 158 );
			m_MenuItemArrowDarkColor = Color.FromArgb( 124, 124, 124 );

			// colors for CommandBar
			m_CommandBarDarkColor = Color.FromArgb( 153, 151, 181 );
			m_CommandBarLightColor = Color.FromArgb( 243, 244, 250 );
			m_CommandBarBorderColor = Color.FromArgb( 124, 124, 148 );

			// colors for DropDown button of the CommandBar
			m_DockBarBackColor = Color.FromArgb( 228, 228, 237 );
			m_DropDownDarkColor = Color.FromArgb( 118, 116, 146 );
			m_DropDownLightColor = Color.FromArgb( 179, 178, 200 );
			m_DropDownHighlightLightColor = Color.FromArgb( 255, 248, 211 );
			m_DropDownHighlightDarkColor = Color.FromArgb( 255, 193, 118 );
			m_DropDownPressedLightColor = Color.FromArgb( 254, 149, 82 );
			m_DropDownPressedDarkColor = Color.FromArgb( 255, 217, 149 );

			// colors for floating CommandBar
			m_FloatHighlightButtonColor = Color.FromArgb( 255, 231, 162 );
			m_FloatHighlightButtonBorderColor = Color.FromArgb( 255, 189, 105 );
			m_FloatPressButtonColor = Color.FromArgb( 208, 212, 217 );
			m_FloatPressButtonBorderColor = Color.FromArgb( 124, 124, 148 );
			m_FloatPressCloseButtonBorderColor = Color.FromArgb( 251, 140, 60 );
			m_FloatPressCloseButtonColor = Color.FromArgb( 251, 140, 60 );
			m_FloatCommandBarLightColor = Color.FromArgb( 245, 245, 252 );
			m_FloatCommandBarDarkColor = Color.FromArgb( 166, 165, 191 );
			m_FloatLightBorderColor = Color.FromArgb( 219, 218, 228 );
			m_FloatBackgroundColor = Color.FromArgb( 122, 121, 153 );
			m_FloatBorderColor = Color.FromArgb( 122, 121, 153 );
			m_FloatCaptionColor = Color.White;

			// color for separator in a CommandBar
			m_BarItemSeparatorColor = Color.FromArgb( 110, 109, 143);

			// colors for ComboButton of the ComboBoxBarItem in CommandBar
			m_ComboButtonLightColor = Color.FromArgb( 241, 243, 243 );
			m_ComboButtonDarkColor = Color.FromArgb( 231, 234, 238 );
			m_ComboButtonBorder = Color.FromArgb(  196, 198, 198 );

			// colors for tab group
			m_TabItemBorderColor = Color.FromArgb(189, 190, 198);
			m_TabItemInnerBorderColor = Color.FromArgb(239, 235, 239);
			m_TabItemOuterBorderColor = Color.FromArgb(231, 229, 239);
			m_TabItemTextColor = Color.FromArgb(74, 81, 90);
			m_TabItemActiveBottomColor = Color.FromArgb(239, 211, 156);
			m_TabItemTopGradientColor = Color.FromArgb(222, 219, 231);
			m_TabItemInActiveBottomColor = Color.FromArgb(231, 231, 239);
			m_TabItemMiddleLineColor = Color.FromArgb(198, 190, 198);
			m_TabPanelColor = Color.FromArgb(214, 215, 222);
			m_TabPanelBorderColor = Color.FromArgb(189, 190, 189);
			m_TabPanelBackColor = Color.FromArgb(214, 215, 222);

			// colors for GroupBar
			m_GroupBarBorderColor = Color.FromArgb(107, 113, 115);
			m_GroupBarHeaderColorLight = Color.FromArgb(247, 247, 255);
			m_GroupBarHeaderColorDark = Color.FromArgb(222, 227, 239);
			m_GroupBarItemTextColor = Color.FromArgb(74, 81, 90);
			m_GroupBarHeaderTextColor = Color.FromArgb(16, 65, 140);
			m_GroupBarItemColorLight = Color.FromArgb(239, 243, 255);
			m_GroupBarItemColorDark = Color.FromArgb(198, 199, 214);
			m_GroupBarSplitterColorDark = Color.FromArgb(115, 117, 148);
			m_GroupBarSplitterColorLight = Color.FromArgb(173, 170, 198);
			m_GroupBarClientAreaBackground = Color.FromArgb( 238, 238, 244 );

            // colors for DataTimePicker
            m_DataTimePickerBorderColor = Color.FromArgb(169, 177, 184);

            m_DataTimePickerDropDownArrowColor = Color.FromArgb(124, 124, 124);
            m_DataTimePickerDropDownLightColor = Color.FromArgb(223, 227, 231);
            m_DataTimePickerDropDownDarkColor = Color.FromArgb(208, 212, 221);

            m_DataTimePickerCheckBoxBorderNormalColor = Color.FromArgb(155, 157, 160);
            m_DataTimePickerCheckBoxBorderPushedColor = Color.FromArgb(107, 113, 115);

			m_DataTimePickerHighLightedForeColor = Color.FromArgb(107, 113, 115);
            
            //colors for MonthCalendarAdv
            m_MonthCalendarHeaderStartColor = Color.FromArgb(208, 212, 221);
            m_MonthCalendarHeaderEndColor = Color.FromArgb(223, 227, 231);
            m_MonthCalendarForeColor = SystemColors.ControlText;

            // colors for XPTaskPane
            m_XPTaskPaneInternalBorderColor = Color.FromArgb(232, 235, 248);
            m_XPTaskPaneBorderColor = Color.FromArgb(158, 160, 160);
            m_XPTaskPageBackColor = Color.FromArgb(232, 235, 248);

			// colors for TabControlAdv
			m_TabDefaultBorderColor = Color.FromArgb(189, 190, 189);
			m_TabHotLightBottomBorderLineColor = Color.FromArgb(239, 186, 116);
			m_TabHotLightGradientTopBeginColor = Color.FromArgb(222, 219, 222);
			m_TabHotLightGradientTopEndColor = Color.FromArgb(231, 227, 231);
			m_TabHotLightGradientBottomBeginColor = Color.FromArgb(231, 215, 173);
			m_TabHotLightGradientBottomEndColor = Color.FromArgb(239, 211, 140);
			m_TabHotLightGradientCircleColor = Color.FromArgb(222, 219, 231);
			m_TabSelectedGradientTopColor = Color.FromArgb(247, 243, 247);
			m_TabSelectedGradientBottomColor = Color.FromArgb(231, 231, 239);
			m_TabSelectedInnerBorderColor = Color.FromArgb(235, 243, 252);
			m_TabHighlightInnerBorderColor = Color.FromArgb(234, 237, 253);
			m_TabSelectedHotLightBorderColor = Color.FromArgb(255, 208, 48);
			m_TabSelectedHotLightInnerBorderColor = Color.FromArgb(255, 240, 187);
			m_TabForeColor = Color.FromArgb(74, 81, 90);
			m_ActiveTabForeColor = m_TabForeColor;
			m_TabBackgroundColor = Color.FromArgb(214, 215, 222);
            m_TabScrollArrowColor = Color.FromArgb(109, 114, 123);

			// colors for DockTabControl 
			m_DockTabForeColor = Color.FromArgb(74, 81, 90);
			m_DockTabBackgroundColor = Color.FromArgb(214, 215, 222);

			// form's colors
			m_ActiveFormBorderColor = Color.FromArgb(208, 212, 221);
			m_InactiveFormBorderColor = Color.FromArgb(230, 229, 229);

			m_FormTextColor = Color.FromArgb( 83, 84, 89 );

			m_ActiveTitleGradientBegin = Color.FromArgb(232, 236, 240);
			m_ActiveTitleGradientEnd = Color.FromArgb(192, 198, 207);
			m_InactiveTitleGradientBegin = Color.FromArgb(247, 247, 247);
			m_InactiveTitleGradientEnd = Color.FromArgb(225, 225, 225);

			m_SystemButtonSelectedGradientBegin = Color.FromArgb(252, 253, 254); ;
			m_SystemButtonSelectedGradientEnd = Color.FromArgb(222, 230, 242);
			m_SystemButtonPressedGradientBegin = Color.FromArgb(195, 199, 204);
			m_SystemButtonPressedGradientEnd = Color.FromArgb(125, 131, 140);

			m_SystemButtonBorderSelected = Color.FromArgb(200, 205, 212);
			m_SystemButtonBorderPressed = Color.FromArgb(151, 156, 160);
			m_FormBackground = Color.FromArgb( 202, 207, 217 );
			
			// upDown colors
			m_UpDownArrowStartColor = Color.FromArgb(96, 104, 112);
			m_UpDownArrowEndColor = Color.FromArgb(58, 62, 66);

			m_UpDownBorderNormalColor = Color.FromArgb(169, 177, 184);
			m_UpDownBackgroundNormalColor = Color.FromArgb(232, 234, 236);
			m_UpDownBackgroundNormalStartColor = Color.FromArgb(232, 234, 236);
			m_UpDownBackgroundNormalEndColor = Color.FromArgb(232, 234, 236);

            // colors for comboBoxAdv
            m_ComboBoxAdvNormalBackColor = SystemColors.Window;
            m_ComboBoxAdvHotBackColor = Color.FromArgb(232, 234, 236);
            m_ComboBoxAdvNormalBorderColor = Color.FromArgb(169, 177, 184);
            m_ComboBoxAdvHotBorderColor = Color.FromArgb(219, 206, 153);
            m_ComboBoxAdvPushedBorderColor = Color.FromArgb(157, 146, 102);
            m_ComboBoxAdvButtonUpperLineColor = Color.FromArgb(192, 185, 165);
            m_ComboBoxAdvArrowColor = Color.FromArgb(124, 124, 124);
            m_ComboBoxAdvLowerArrowLineColor = Color.FromArgb(255, 248, 203);
            m_ComboBoxAdvHotBackgroundButtonColor1 = Color.FromArgb(255, 252, 226);
            m_ComboBoxAdvHotBackgroundButtonColor2 = Color.FromArgb(255, 232, 150);
            m_ComboBoxAdvHotBackgroundButtonColor3 = Color.FromArgb(255, 217, 117);
            m_ComboBoxAdvHotBackgroundButtonColor4 = Color.FromArgb(255, 231, 165);
            m_ComboBoxAdvNormalBackgroundButtonColor1 = Color.FromArgb(223, 227, 231);
            m_ComboBoxAdvNormalBackgroundButtonColor2 = Color.FromArgb(216, 220, 226);
            m_ComboBoxAdvNormalBackgroundButtonColor3 = Color.FromArgb(212, 216, 224);
            m_ComboBoxAdvNormalBackgroundButtonColor4 = Color.FromArgb(221, 225, 230);
            m_ComboBoxAdvPushedBackgroundButtonColor1 = Color.FromArgb(224, 212, 178);
            m_ComboBoxAdvPushedBackgroundButtonColor2 = Color.FromArgb(239, 189, 119);
            m_ComboBoxAdvPushedBackgroundButtonColor3 = Color.FromArgb(255, 168, 56);
            m_ComboBoxAdvPushedBackgroundButtonColor4 = Color.FromArgb(255, 230, 148);

            // colors for CheckBoxAdv
            m_CheckBoxAdvNormalBackColor = Color.FromArgb(248, 248, 248);
            m_CheckBoxAdvSelectedBackColor = Color.FromArgb(254, 248, 232);
            m_CheckBoxAdvPushedBackColor = Color.FromArgb(255, 244, 213);
            m_CheckBoxAdvNormalBorderColor = Color.FromArgb(155, 157, 160);
            m_CheckBoxAdvSelectedBorderColor = Color.FromArgb(155, 157, 160);
            m_CheckBoxAdvPushedBorderColor = Color.FromArgb(155, 157, 160);
            m_CheckBoxAdvNormalInternalBorderColor = Color.FromArgb(244, 244, 244);
            m_CheckBoxAdvSelectedInternalBorderColor = Color.FromArgb(244, 244, 244);
            m_CheckBoxAdvPushedInternalBorderColor = Color.FromArgb(229, 236, 247);
            m_CheckBoxAdvNormalInternalRectangleBorderColor = Color.FromArgb(162, 172, 185);
            m_CheckBoxAdvSelectedInternalRectangleBorderColor = Color.FromArgb(250, 213, 122);
            m_CheckBoxAdvPushedInternalRectangleBorderColor = Color.FromArgb(242, 137, 38);
            m_CheckBoxAdvNormalInternalRectangleColor = Color.FromArgb(202, 207, 213);
            m_CheckBoxAdvSelectedInternalRectangleColor = Color.FromArgb(252, 231, 175);
            m_CheckBoxAdvPushedInternalRectangleColor = Color.FromArgb(255, 208, 103);
            m_CheckBoxAdvNormalTickColor = Color.FromArgb(74, 107, 150);
            m_CheckBoxAdvSelectedTickColor = Color.FromArgb(78, 108, 143);
            m_CheckBoxAdvPushedTickColor = Color.FromArgb(79, 108, 139);
            m_CheckBoxAdvDisabledTickColor = Color.FromArgb(176, 190, 208);
            m_CheckBoxAdvIndeterminateRectangleColor = Color.FromArgb(158, 168, 178);
            m_CheckBoxAdvDisabledBackColor = Color.FromArgb(238, 240, 242);
            m_CheckBoxAdvDisabledBorderColor = Color.FromArgb(174, 177, 181);
            m_CheckBoxAdvDisabledInternalBorderColor = Color.FromArgb(224, 226, 229);

            // colors for RadioButtonAdv
            m_RadioButtonAdvNormalBackColor = SystemColors.Window;
            m_RadioButtonAdvNormalBorderColor = Color.FromArgb(155, 157, 160);
            m_RadioButtonAdvNormalInternalBorderColor = Color.FromArgb(162, 172, 185);
            m_RadioButtonAdvSelectedBackColor = Color.FromArgb(244, 244, 244);
            m_RadioButtonAdvSelectedBorderColor = Color.FromArgb(155, 157, 160);
            m_RadioButtonAdvSelectedInternalBorderColor = Color.FromArgb(250, 205, 101);
            m_RadioButtonAdvPushedBackColor = Color.FromArgb(229, 236, 247);
            m_RadioButtonAdvPushedBorderColor = Color.FromArgb(155, 157, 160);
            m_RadioButtonAdvPushedInternalBorderColor = Color.FromArgb(244, 171, 14);
            m_RadioButtonAdvCheckMarkBorderColor = Color.FromArgb(17, 69, 103);
            m_RadioButtonAdvCheckMarkNormalBottomColor = Color.FromArgb(11, 130, 199);
            m_RadioButtonAdvCheckMarkSelectedBottomColor = Color.FromArgb(13, 160, 243);
            m_RadioButtonAdvCheckMarkPushedBottomColor = Color.FromArgb(7, 84, 131);

            // colors for TabBarSplitterControl
            m_TabBarSplitterBackColor = Color.FromArgb( 239, 243, 255 );
            m_TabBarSplitterBorderColor = Color.FromArgb( 107, 109, 107 );
            m_TabBarSplitterTextColor = Color.FromArgb( 49, 52, 49 );
            m_TabBarSplitterTabStartColor = Color.FromArgb( 222, 223, 231 );
            m_TabBarSplitterTabEndColor = Color.FromArgb( 181, 190, 206 );
            m_TabBarSplitterTabBarStartColor = Color.FromArgb(123, 121, 123);
            m_TabBarSplitterTabBarEndColor = Color.FromArgb( 74, 73, 74 );
            m_TabBarSplitterButtonHoveredStartColor = Color.White;
            m_TabBarSplitterButtonHoveredEndColor = Color.FromArgb( 222, 231, 247 );
            m_TabBarSplitterButtonPushedStartColor = Color.FromArgb( 206, 223, 255 );
            m_TabBarSplitterButtonPushedEndColor = Color.FromArgb( 148, 182, 239 );
            m_TabBarSplitterSizeGripperColor = Color.FromArgb( 183, 186, 194 );
            m_TabBarSplitterSizeGripperLightColor = Color.FromArgb( 205, 209, 213 );
            m_TabBarSplitterSizeGripperDarkColor = Color.FromArgb( 114, 118, 122 );

            //colors for XPTaskBar
            m_XPTaskBarBorderColor = Color.FromArgb(111, 112, 116);
            m_XPTaskBarBoxBackColor = Color.FromArgb(213, 219, 231);
            m_XPTaskBarBoxForeColor = Color.FromArgb(21, 66, 139);
            m_XPTaskBarBoxHeaderLowerLineColor = Color.FromArgb(197, 199, 199);
            m_XPTaskBarBoxHeaderUpperLineColor = Color.FromArgb(111, 112, 116);
            m_XPTaskBarBoxArrowColor = Color.FromArgb(101, 104, 112);
            m_XPTaskBarBoxActiveHighlightedItemColor = Color.FromArgb(255, 232, 156);
            m_XPTaskBarBoxInactiveHighlightedItemColor = Color.FromArgb(229, 229, 229);

			// colors for ButtonAdv
			m_ButtonDefaultTopColor = Color.FromArgb( 223, 227, 231 );
			m_ButtonDefaultBottomColor = Color.FromArgb( 208, 212, 221 );
			m_ButtonDefaultBorderColor = Color.FromArgb( 208, 212, 221 );
			m_ButtonDefaultInternalBorderColor = Color.FromArgb( 128, Color.FromArgb( 250, 251, 255 ) );
			m_ButtonPressedInternalBorderColor = Color.FromArgb( 70, Color.FromArgb( 176, 132, 92 ) );
			m_ButtonSelectedInternalBorderColor = Color.FromArgb( 128, Color.FromArgb( 250, 251, 255 ) );

			// ColorUIAdv colors
			m_ColorUIAdvTextColor = Color.FromArgb( 79, 83, 87 );

            //StatusBarExt Colors
            this.m_StatusBarExtTopGradient = Color.FromArgb(232, 236, 240);
            this.m_StatusBarExtFillColor = Color.FromArgb(192, 198, 207);
            this.m_StatusBarExtBottomGradient = Color.FromArgb(232, 236, 240);
		}
		#endregion
	}

	/// <summary>
	/// Provides colors for black colorscheme of the Office2007 visual style.
	/// </summary>
	public class Office2007BlackColors : Office2007Colors
	{
		#region Class Overrides
		/// <summary>
		/// Initialize colors for Black colorscheme of the Office2007 visual style.
		/// </summary>
		protected override void InitializeColors()
		{
			base.InitializeColors ();

            // colors for TextBoxExt 
            m_ActiveTextBoxBorderColor = Color.FromArgb(255, 230, 158);
            m_InactiveTextBoxBorderColor = Color.FromArgb(164, 171, 178);
            m_ActiveTextBoxBackColor = Color.FromArgb(255, 255, 255);
            m_InactiveTextBoxBackColor = Color.FromArgb(232, 232, 232);

            //TreeviewAdv
            m_SelectedNodeBackground = Color.FromArgb(191, 191, 191);
            m_TreeNodeArrowColor = Color.FromArgb(143, 143, 143);
            m_TreeviewBackColor = Color.FromArgb(255, 255, 255);
            m_TreeViewFontColor = Color.FromArgb(0, 0, 0);

			// colors for menu
			m_MenuTextBoxBorderColor = Color.FromArgb( 137, 137, 137 );
			m_MenuTextBoxBackColor = Color.FromArgb( 232, 232, 232 );
			m_MenuComboButtonHighlightLightColor = Color.FromArgb( 239, 242, 246 );
			m_MenuComboButtonHighlightDarkColor = Color.FromArgb( 218, 224, 231 );
			m_MenuComboButtonArrowColor = Color.FromArgb( 124, 124, 124 );
			m_MenuItemArrowLightColor = Color.FromArgb( 78, 78, 78 );
			m_MenuItemArrowDarkColor = Color.FromArgb( 40, 40, 40 );

			// colors for CommandBar
			m_CommandBarDarkColor = Color.FromArgb( 148, 156, 166 );
			m_CommandBarLightColor = Color.FromArgb( 205, 208, 213 );
			m_CommandBarBorderColor = Color.FromArgb( 76, 83, 92 );

			// colors for DropDown button of the CommandBar
			m_DockBarBackColor = Color.FromArgb( 83, 83, 83 );
			m_DropDownDarkColor = Color.FromArgb( 76, 83, 92 );
			m_DropDownLightColor = Color.FromArgb( 178, 183, 191 );
			m_DropDownHighlightLightColor = Color.FromArgb( 255, 248, 211 );
			m_DropDownHighlightDarkColor = Color.FromArgb( 255, 193, 118 );
			m_DropDownPressedLightColor = Color.FromArgb( 254, 149, 82 );
			m_DropDownPressedDarkColor = Color.FromArgb( 255, 217, 149 );

			// colors for floating CommandBar
			m_FloatHighlightButtonColor = Color.FromArgb( 255, 231, 162 );
			m_FloatHighlightButtonBorderColor = Color.FromArgb( 255, 189, 105 );
			m_FloatPressButtonColor = Color.FromArgb( 221, 224, 227 );
			m_FloatPressButtonBorderColor = Color.FromArgb( 145, 153, 164 );
			m_FloatPressCloseButtonBorderColor = Color.FromArgb( 251, 140, 60 );
			m_FloatPressCloseButtonColor = Color.FromArgb( 251, 140, 60 );
			m_FloatCommandBarLightColor = Color.FromArgb( 206, 210, 215 );
			m_FloatCommandBarDarkColor = Color.FromArgb( 155, 162, 172 );
			m_FloatLightBorderColor = Color.FromArgb( 118, 128, 142 );
			m_FloatBackgroundColor = Color.FromArgb( 83, 83, 83 );
			m_FloatBorderColor = Color.FromArgb( 55, 60, 67 );
			m_FloatCaptionColor = Color.White;

			// color for separator in a CommandBar
			m_BarItemSeparatorColor = Color.FromArgb( 145, 153, 164);

			// colors for ComboButton of the ComboBoxBarItem in CommandBar
			m_ComboButtonLightColor = Color.FromArgb( 214, 222, 223 );
			m_ComboButtonDarkColor = Color.FromArgb( 206, 213, 215 );
			m_ComboButtonBorder = Color.FromArgb(  179, 188, 191  );

			// colors for tab group
			m_TabItemBorderColor = Color.FromArgb(66, 65, 66);
			m_TabItemInnerBorderColor = Color.FromArgb(189, 190, 189);
			m_TabItemOuterBorderColor = Color.FromArgb(74, 73, 74);
			m_TabItemTextColor = Color.FromArgb(255, 255, 255);
			m_TabItemActiveBottomColor = Color.FromArgb(189, 154, 57);
			m_TabItemTopGradientColor = Color.FromArgb(107, 105, 99);
			m_TabItemInActiveBottomColor = Color.FromArgb(148, 150, 148);
			m_TabItemMiddleLineColor = Color.FromArgb(101, 106, 102);
			m_TabPanelColor = Color.FromArgb(82, 81, 82);
			m_TabPanelBorderColor = Color.FromArgb(57, 56, 57);
			m_TabPanelBackColor = Color.FromArgb(82, 81, 82);

			// colors for GroupBar
			m_GroupBarBorderColor = Color.FromArgb(74, 81, 90);
			m_GroupBarHeaderColorLight = Color.FromArgb(247, 243, 247);
			m_GroupBarHeaderColorDark = Color.FromArgb(189, 195, 206);
			m_GroupBarItemTextColor = Color.FromArgb(49, 60, 66);
			m_GroupBarHeaderTextColor = Color.Black;
			m_GroupBarItemColorLight = Color.FromArgb(255, 251, 255);
			m_GroupBarItemColorDark = Color.FromArgb(206, 207,214);
			m_GroupBarSplitterColorDark = Color.FromArgb(198, 203, 214);
			m_GroupBarSplitterColorLight = Color.FromArgb(247, 247, 255);
			m_GroupBarClientAreaBackground = Color.FromArgb( 235, 235, 235 );

            // colors for DataTimePicker
            m_DataTimePickerBorderColor = Color.FromArgb(137, 137, 137);

            m_DataTimePickerDropDownArrowColor = Color.FromArgb(79, 86, 96);
            m_DataTimePickerDropDownLightColor = Color.FromArgb(146, 146, 146);
            m_DataTimePickerDropDownDarkColor = Color.FromArgb(83, 83, 83);

            m_DataTimePickerCheckBoxBorderNormalColor = Color.FromArgb(132, 132, 132);
            m_DataTimePickerCheckBoxBorderPushedColor = Color.FromArgb(74, 81, 90);
            
            m_DataTimePickerHighLightedForeColor = Color.Black;

            //colors for MonthCalendarAdv
            m_MonthCalendarHeaderStartColor = Color.FromArgb(83, 83, 83);
            m_MonthCalendarHeaderEndColor = Color.FromArgb(146, 146, 146);
            m_MonthCalendarForeColor = Color.White;

            // colors for XPTaskPane
            m_XPTaskPaneInternalBorderColor = Color.FromArgb(246, 243, 248);
            m_XPTaskPaneBorderColor = Color.FromArgb(170, 170, 170);
            m_XPTaskPageBackColor = Color.FromArgb(246, 243, 248);

			// colors for TabControlAdv
			m_TabDefaultBorderColor = Color.FromArgb(151, 151, 151);
            m_TabHotLightBottomBorderLineColor = Color.FromArgb(195, 164, 54);
			m_TabHotLightGradientTopBeginColor = Color.FromArgb(151, 151, 151);
			m_TabHotLightGradientTopEndColor = Color.FromArgb(122, 109, 97);
			m_TabHotLightGradientBottomBeginColor = Color.FromArgb(126, 118, 74);
			m_TabHotLightGradientBottomEndColor = Color.FromArgb(233, 190, 38);
            m_TabHotLightGradientCircleColor = Color.FromArgb(122, 109, 97);
            m_TabSelectedGradientTopColor = Color.FromArgb(237, 241, 245);
            m_TabSelectedGradientBottomColor = Color.FromArgb(201, 202, 209);
			m_TabSelectedInnerBorderColor = Color.FromArgb(235, 243, 252);
			m_TabHighlightInnerBorderColor = Color.FromArgb(234, 237, 253);
			m_TabSelectedHotLightBorderColor = Color.FromArgb(255, 208, 48);
			m_TabSelectedHotLightInnerBorderColor = Color.FromArgb(255, 240, 187);
			m_TabForeColor = Color.White;
			m_ActiveTabForeColor = Color.Black;
			m_TabBackgroundColor = Color.FromArgb(82, 81, 82);
            m_TabScrollArrowColor = Color.WhiteSmoke;

			// colors for DockTabControl 
			m_DockTabForeColor = Color.FromArgb(176, 178, 176);
			m_DockTabBackgroundColor = Color.FromArgb(82, 81, 82);

			// form's colors
			m_ActiveFormBorderColor = Color.FromArgb(83, 83, 83);
			m_InactiveFormBorderColor = Color.FromArgb(153, 153, 153);

			m_FormTextColor = Color.White;

			m_ActiveTitleGradientBegin = Color.FromArgb( 74, 74, 74 );
			m_ActiveTitleGradientEnd = Color.FromArgb( 47, 47, 47 );
			m_InactiveTitleGradientBegin = Color.FromArgb( 157, 157, 157 );
			m_InactiveTitleGradientEnd = Color.FromArgb(146, 146, 146);

			m_SystemButtonSelectedGradientBegin = Color.FromArgb(162, 171, 180);
			m_SystemButtonSelectedGradientEnd = Color.FromArgb(91, 105, 123);
			m_SystemButtonPressedGradientBegin = Color.FromArgb(43, 43, 43);
			m_SystemButtonPressedGradientEnd = Color.FromArgb(0, 0, 0);

			m_SystemButtonBorderSelected = Color.FromArgb(86, 96, 109);
			m_SystemButtonBorderPressed = Color.FromArgb(34, 36, 41);
			m_FormBackground = Color.FromArgb( 113, 113, 113 );

			// upDown colors
			m_UpDownArrowStartColor = Color.FromArgb(87, 87, 87);
			m_UpDownArrowEndColor = Color.FromArgb(52, 52, 52);

			m_UpDownBorderNormalColor = Color.FromArgb(137, 137, 137);
			m_UpDownBackgroundNormalColor = Color.FromArgb(232, 232, 232);
			m_UpDownBackgroundNormalStartColor = Color.FromArgb(218, 224, 231);
			m_UpDownBackgroundNormalEndColor = Color.FromArgb(238, 242, 246);

            // colors for comboBoxAdv
            m_ComboBoxAdvNormalBackColor = SystemColors.Window;
            m_ComboBoxAdvHotBackColor = Color.FromArgb(232, 232, 232);
            m_ComboBoxAdvNormalBorderColor = Color.FromArgb(137, 137, 137);
            m_ComboBoxAdvHotBorderColor = Color.FromArgb(219, 206, 153);
            m_ComboBoxAdvPushedBorderColor = Color.FromArgb(157, 146, 102);
            m_ComboBoxAdvButtonUpperLineColor = Color.FromArgb(192, 185, 165);
            m_ComboBoxAdvArrowColor = Color.FromArgb(124, 124, 124);
            m_ComboBoxAdvLowerArrowLineColor = Color.FromArgb(255, 248, 203);
            m_ComboBoxAdvHotBackgroundButtonColor1 = Color.FromArgb(255, 252, 226);
            m_ComboBoxAdvHotBackgroundButtonColor2 = Color.FromArgb(255, 232, 150);
            m_ComboBoxAdvHotBackgroundButtonColor3 = Color.FromArgb(255, 217, 117);
            m_ComboBoxAdvHotBackgroundButtonColor4 = Color.FromArgb(255, 231, 165);
            m_ComboBoxAdvNormalBackgroundButtonColor1 = Color.FromArgb(146, 146, 146);
            m_ComboBoxAdvNormalBackgroundButtonColor2 = Color.FromArgb(117, 117, 117);
            m_ComboBoxAdvNormalBackgroundButtonColor3 = Color.FromArgb(101, 101, 101);
            m_ComboBoxAdvNormalBackgroundButtonColor4 = Color.FromArgb(139, 139, 139);
            m_ComboBoxAdvPushedBackgroundButtonColor1 = Color.FromArgb(224, 212, 178);
            m_ComboBoxAdvPushedBackgroundButtonColor2 = Color.FromArgb(239, 189, 119);
            m_ComboBoxAdvPushedBackgroundButtonColor3 = Color.FromArgb(255, 168, 56);
            m_ComboBoxAdvPushedBackgroundButtonColor4 = Color.FromArgb(255, 230, 148);

            // colors for CheckBoxAdv
            m_CheckBoxAdvNormalBackColor = Color.FromArgb(248, 248, 248);
            m_CheckBoxAdvSelectedBackColor = Color.FromArgb(254, 248, 232);
            m_CheckBoxAdvPushedBackColor = Color.FromArgb(255, 244, 213);
            m_CheckBoxAdvNormalBorderColor = Color.FromArgb(132, 132, 132);
            m_CheckBoxAdvSelectedBorderColor = Color.FromArgb(132, 132, 132);
            m_CheckBoxAdvPushedBorderColor = Color.FromArgb(132, 132, 132);
            m_CheckBoxAdvNormalInternalBorderColor = Color.FromArgb(244, 244, 244);
            m_CheckBoxAdvSelectedInternalBorderColor = Color.FromArgb(244, 244, 244);
            m_CheckBoxAdvPushedInternalBorderColor = Color.FromArgb(229, 236, 247);
            m_CheckBoxAdvNormalInternalRectangleBorderColor = Color.FromArgb(162, 172, 185);
            m_CheckBoxAdvSelectedInternalRectangleBorderColor = Color.FromArgb(250, 213, 122);
            m_CheckBoxAdvPushedInternalRectangleBorderColor = Color.FromArgb(242, 137, 38);
            m_CheckBoxAdvNormalInternalRectangleColor = Color.FromArgb(202, 207, 213);
            m_CheckBoxAdvSelectedInternalRectangleColor = Color.FromArgb(252, 231, 175);
            m_CheckBoxAdvPushedInternalRectangleColor = Color.FromArgb(255, 208, 103);
            m_CheckBoxAdvNormalTickColor = Color.FromArgb(74, 107, 150);
            m_CheckBoxAdvSelectedTickColor = Color.FromArgb(78, 108, 143);
            m_CheckBoxAdvPushedTickColor = Color.FromArgb(79, 105, 130);
            m_CheckBoxAdvDisabledTickColor = Color.FromArgb(176, 190, 208);
            m_CheckBoxAdvIndeterminateRectangleColor = Color.FromArgb(158, 168, 178);
            m_CheckBoxAdvDisabledBackColor = Color.FromArgb(238, 240, 242);
            m_CheckBoxAdvDisabledBorderColor = Color.FromArgb(174, 177, 181);
            m_CheckBoxAdvDisabledInternalBorderColor = Color.FromArgb(224, 226, 229);

            // colors for RadioButtonAdv
            m_RadioButtonAdvNormalBackColor = SystemColors.Window;
            m_RadioButtonAdvNormalBorderColor = Color.FromArgb(132, 132, 132);
            m_RadioButtonAdvNormalInternalBorderColor = Color.FromArgb(162, 172, 185);
            m_RadioButtonAdvSelectedBackColor = Color.FromArgb(244, 244, 244);
            m_RadioButtonAdvSelectedBorderColor = Color.FromArgb(132, 132, 132);
            m_RadioButtonAdvSelectedInternalBorderColor = Color.FromArgb(250, 205, 101);
            m_RadioButtonAdvPushedBackColor = Color.FromArgb(244, 244, 244);
            m_RadioButtonAdvPushedBorderColor = Color.FromArgb(132, 132, 132);
            m_RadioButtonAdvPushedInternalBorderColor = Color.FromArgb(244, 171, 14);
            m_RadioButtonAdvCheckMarkBorderColor = Color.FromArgb(17, 69, 103);
            m_RadioButtonAdvCheckMarkNormalBottomColor = Color.FromArgb(11, 130, 199);
            m_RadioButtonAdvCheckMarkSelectedBottomColor = Color.FromArgb(13, 160, 243);
            m_RadioButtonAdvCheckMarkPushedBottomColor = Color.FromArgb(7, 84, 131);

            // colors for TabBarSplitterControl
            m_TabBarSplitterBackColor = Color.FromArgb( 255, 251, 255 );
            m_TabBarSplitterBorderColor = Color.FromArgb( 107, 109, 107 );
            m_TabBarSplitterTextColor = Color.FromArgb( 49, 52, 49 );
            m_TabBarSplitterTabStartColor = Color.FromArgb( 222, 223, 231 );
            m_TabBarSplitterTabEndColor = Color.FromArgb( 181, 190, 206 );
            m_TabBarSplitterTabBarStartColor = Color.FromArgb(123, 121, 123);
            m_TabBarSplitterTabBarEndColor = Color.FromArgb( 74, 73, 74 );
            m_TabBarSplitterButtonHoveredStartColor = Color.White;
            m_TabBarSplitterButtonHoveredEndColor = Color.FromArgb( 222, 231, 247 );
            m_TabBarSplitterButtonPushedStartColor = Color.FromArgb( 206, 223, 255 );
            m_TabBarSplitterButtonPushedEndColor = Color.FromArgb( 148, 182, 239 );
            m_TabBarSplitterSizeGripperColor = Color.FromArgb( 112, 112, 112 );
            m_TabBarSplitterSizeGripperLightColor = Color.FromArgb( 204, 204, 204 );
            m_TabBarSplitterSizeGripperDarkColor = Color.FromArgb( 37, 37, 37 );

            //colors for XPTaskBar
            m_XPTaskBarBorderColor = Color.FromArgb(76, 83, 92);
            m_XPTaskBarBoxBackColor = Color.FromArgb(221, 224, 227);
            m_XPTaskBarBoxForeColor = Color.Black;
            m_XPTaskBarBoxHeaderLowerLineColor = Color.FromArgb(199, 203, 209);
            m_XPTaskBarBoxHeaderUpperLineColor = Color.FromArgb(76, 83, 92);
            m_XPTaskBarBoxArrowColor = Color.FromArgb(49, 52, 49);
            m_XPTaskBarBoxActiveHighlightedItemColor = Color.FromArgb(255, 232, 156);
            m_XPTaskBarBoxInactiveHighlightedItemColor = Color.FromArgb(229, 229, 229);

			// colors for ButtonAdv
			m_ButtonDefaultTopColor = Color.FromArgb( 146, 146, 146 );
			m_ButtonDefaultBottomColor = Color.FromArgb( 83, 83, 83 );
			m_ButtonDefaultBorderColor = Color.FromArgb( 153, 153, 153 );
			m_ButtonDefaultInternalBorderColor = Color.FromArgb( 128, Color.FromArgb( 250, 251, 255 ) );
			m_ButtonPressedInternalBorderColor = Color.FromArgb( 70, Color.FromArgb( 176, 132, 92 ) );
			m_ButtonSelectedInternalBorderColor = Color.FromArgb( 128, Color.FromArgb( 250, 251, 255 ) );

			// colors for ColorUIAdv
			m_ColorUIAdvTextColor = Color.FromArgb( 70, 70, 70 );
		}
		#endregion
	}

	public class Office2007OutlookColors
	{
		#region Class Members
		private static Color m_panelColor = Color.Empty;
		private static Color m_borderColor = Color.Empty;
		private static Color m_tabItemColor = Color.Empty;
		private static Color m_innerBorderColor = Color.Empty;
		private static Color m_leftAHPanelColor = Color.Empty;
		private static Color m_rightAHPanelColor = Color.Empty;

		private static Color m_MenuSelectedItemColor = Color.Empty;
		private static Color m_MenuSelectedItemBorderColor = Color.Empty;
		private static Color m_MenuBorderColor = Color.Empty;
		private static Color m_MenuSeparatorColor = Color.Empty;
		private static Color m_MenuColumnStyleColor = Color.Empty;
		private static Color m_MenuCheckMarkColor = Color.Empty;
		private static Color m_MenuSelectedCheckMarkColor = Color.Empty;
		private static Color m_MenuCheckMarkBorderColor = Color.Empty;
		private static Color m_MenuSelectedCheckMarkBorderColor = Color.Empty;
		private static Color m_MenuBackground = Color.Empty;

		private static Color m_BarItemHighlightBorderColor = Color.Empty;
		private static Color m_BarItemPressBorderColor = Color.Empty;
		private static Color m_BarItemCheckBorderColor = Color.Empty;
		private static Color m_BarItemHighlightLightColor = Color.Empty;
		private static Color m_BarItemHighlightDarkColor = Color.Empty;
		private static Color m_BarItemPressLightColor = Color.Empty;
		private static Color m_BarItemPressDarkColor = Color.Empty;
		private static Color m_BarItemCheckLightColor = Color.Empty;
		private static Color m_BarItemCheckDarkColor = Color.Empty;
		
		private static Color m_dDBarItemBorderColor = Color.Empty;
		private static Color m_dDBarItemLightColor = Color.Empty;
		private static Color m_dDBarItemDarkColor = Color.Empty;
		private static Color m_DropDownHighlightLightColor = Color.Empty;
		private static Color m_DropDownHighlightDarkColor = Color.Empty;
		private static Color m_DropDownPressedLightColor = Color.Empty;
		private static Color m_DropDownPressedDarkColor = Color.Empty;

		private static Color m_ComboButtonLightColor = Color.Empty;
		private static Color m_ComboButtonDarkColor = Color.Empty;
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets a value indicating whether themed colors are used.
		/// </summary>
		/// <value><c>true</c> if themed colors are used, <c>false</c> otherwise.</value>
		public static bool UseThemedColors
		{
			get
			{
				bool themedcolors = false;
				if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed)
				{
					if (XPThemes.IsDefaultBlueThemeOn || XPThemes.IsOliveGreenThemeOn || XPThemes.IsSilverThemeOn)
						themedcolors = true;
				}
				return themedcolors;
			}
		}

		/// <summary>
		/// gets/sets color of right auto hide panel.
		/// </summary>
		public static Color RightAHPanelColor
		{
			get
			{
				return m_rightAHPanelColor;
			}
			set
			{
				if( m_rightAHPanelColor != value )
				{
					m_rightAHPanelColor = value;
				}
			}
		}

		/// <summary>
		/// Gets/sets color of left AH panel.
		/// </summary>
		public static Color LeftAHPanelColor
		{
			get
			{
				return m_leftAHPanelColor;
			}
			set
			{
				if( m_leftAHPanelColor != value )
				{
					m_leftAHPanelColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color of the panel.
		/// </summary>
		public static Color PanelColor
		{
			get
			{
				return m_panelColor;
			}
			set
			{
				if (m_panelColor != value)
				{
					m_panelColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color of the border.
		/// </summary>
		public static Color BorderColor
		{
			get
			{
				return m_borderColor;
			}
			set
			{
				if (m_borderColor != value)
				{
					m_borderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color of the inner border.
		/// </summary>
		public static Color InnerBorderColor
		{
			get
			{
				return m_innerBorderColor;
			}
			set
			{
				if (m_innerBorderColor != value)
				{
					m_innerBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color of the tab item.
		/// </summary>
		public static Color TabItemColor
		{
			get
			{
				return m_tabItemColor;
			}
			set
			{
				if ( m_tabItemColor != value )
				{
					m_tabItemColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for border of the DropDownBarItem.
		/// </summary>
		public static Color DDBarItemBorderColor
		{
			get
			{
				return m_dDBarItemBorderColor;
			}
			set
			{
				if( m_dDBarItemBorderColor != value )
				{
					m_dDBarItemBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the light color of the DropDownBarItem.
		/// </summary>
		public static Color DDBarItemLightColor
		{
			get
			{
				return m_dDBarItemLightColor;
			}
			set
			{
				if( m_dDBarItemLightColor != value )
				{
					m_dDBarItemLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the dark color of the DropDownBarItem.
		/// </summary>
		public static Color DDBarItemDarkColor
		{
			get
			{
				return m_dDBarItemDarkColor;
			}
			set
			{
				if( m_dDBarItemDarkColor != value )
				{
					m_dDBarItemDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for border of the menu.
		/// </summary>
		public static Color MenuBorderColor
		{
			get
			{
				return m_MenuBorderColor;
			}
			set
			{
				if( m_MenuBorderColor != value )
				{
					m_MenuBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for separator of the menu.
		/// </summary>
		public static Color MenuSeparatorColor
		{
			get
			{
				return m_MenuSeparatorColor;
			}
			set
			{
				if( m_MenuSeparatorColor != value )
				{
					m_MenuSeparatorColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for border selected item of the menu.
		/// </summary>
		public static Color MenuSelectedItemBorderColor
		{
			get
			{
				return m_MenuSelectedItemBorderColor;
			}
			set
			{
				if( m_MenuSelectedItemBorderColor != value )
				{
					m_MenuSelectedItemBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for selected item of the menu.
		/// </summary>
		public static Color MenuSelectedItemColor
		{
			get
			{
				return m_MenuSelectedItemColor;
			}
			set
			{
				if( m_MenuSelectedItemColor != value )
				{
					m_MenuSelectedItemColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for column of the menu.
		/// </summary>
		public static Color MenuColumnStyleColor
		{
			get
			{
				return m_MenuColumnStyleColor;
			}
			set
			{
				if( m_MenuColumnStyleColor != value )
				{
					m_MenuColumnStyleColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for check mark of the menu.
		/// </summary>
		public static Color MenuCheckMarkColor
		{
			get
			{
				return m_MenuCheckMarkColor;
			}
			set
			{
				if( m_MenuCheckMarkColor != value )
				{
					m_MenuCheckMarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for selected check mark of the menu.
		/// </summary>
		public static Color MenuSelectedCheckMarkColor
		{
			get
			{
				return m_MenuSelectedCheckMarkColor;
			}
			set
			{
				if( m_MenuSelectedCheckMarkColor != value )
				{
					m_MenuSelectedCheckMarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for check mark border of the menu.
		/// </summary>
		public static Color MenuCheckMarkBorderColor
		{
			get
			{
				return m_MenuCheckMarkBorderColor;
			}
			set
			{
				if( m_MenuCheckMarkBorderColor != value )
				{
					m_MenuCheckMarkBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the color for selected check mark border of the menu.
		/// </summary>
		public static Color MenuSelectedCheckMarkBorderColor
		{
			get
			{
				return m_MenuSelectedCheckMarkBorderColor;
			}
			set
			{
				if( m_MenuSelectedCheckMarkBorderColor != value )
				{
					m_MenuSelectedCheckMarkBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the background color of the menu.
		/// </summary>
		public static Color MenuBackground
		{
			get
			{
				return m_MenuBackground;
			}
			set
			{
				if( m_MenuBackground != value )
				{
					m_MenuBackground = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets border color of the BarItem.
		/// </summary>
		public static Color BarItemHighlightBorderColor
		{
			get
			{
				return m_BarItemHighlightBorderColor;
			}
			set
			{
				if( m_BarItemHighlightBorderColor != value )
				{
					m_BarItemHighlightBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets border color of the pressed BarItem.
		/// </summary>
		public static Color BarItemPressBorderColor
		{
			get
			{
				return m_BarItemPressBorderColor;
			}
			set
			{
				if( m_BarItemPressBorderColor != value )
				{
					m_BarItemPressBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets border color of the checked BarItem.
		/// </summary>
		public static Color BarItemCheckBorderColor
		{
			get
			{
				return m_BarItemCheckBorderColor;
			}
			set
			{
				if( m_BarItemCheckBorderColor != value )
				{
					m_BarItemCheckBorderColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color of the checked BarItem.
		/// </summary>
		public static Color BarItemCheckLightColor
		{
			get
			{
				return m_BarItemCheckLightColor;
			}
			set
			{
				if( m_BarItemCheckLightColor != value )
				{
					m_BarItemCheckLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets dark color of the checked BarItem.
		/// </summary>
		public static Color BarItemCheckDarkColor
		{
			get
			{
				return m_BarItemCheckDarkColor;
			}
			set
			{
				if( m_BarItemCheckDarkColor != value )
				{
					m_BarItemCheckDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color of the BarItem.
		/// </summary>
		public static Color BarItemHighlightLightColor
		{
			get
			{
				return m_BarItemHighlightLightColor;
			}
			set
			{
				if( m_BarItemHighlightLightColor != value )
				{
					m_BarItemHighlightLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets dark color of the BarItem.
		/// </summary>
		public static Color BarItemHighlightDarkColor
		{
			get
			{
				return m_BarItemHighlightDarkColor;
			}
			set
			{
				if( m_BarItemHighlightDarkColor != value )
				{
					m_BarItemHighlightDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color of the pressed BarItem.
		/// </summary>
		public static Color BarItemPressLightColor
		{
			get
			{
				return m_BarItemPressLightColor;
			}
			set
			{
				if( m_BarItemPressLightColor != value )
				{
					m_BarItemPressLightColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets dark color of the pressed BarItem.
		/// </summary>
		public static Color BarItemPressDarkColor
		{
			get
			{
				return m_BarItemPressDarkColor;
			}
			set
			{
				if( m_BarItemPressDarkColor != value )
				{
					m_BarItemPressDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color for highlight dropdown button of the CommandBar.
		/// </summary>
		public static Color DropDownHighlightLightColor
		{
			get
			{
				return m_DropDownHighlightLightColor;
			}
			set
			{
				if( m_DropDownHighlightLightColor != value )
				{
					m_DropDownHighlightLightColor = value;
				}
			}
		}
		
		/// <summary>
		/// Gets or sets dark color for highlight dropdown button of the CommandBar.
		/// </summary>
		public static Color DropDownHighlightDarkColor
		{
			get
			{
				return m_DropDownHighlightDarkColor;
			}
			set
			{
				if( m_DropDownHighlightDarkColor != value )
				{
					m_DropDownHighlightDarkColor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets light color for pressed dropdown button of the CommandBar.
		/// </summary>
		public static Color DropDownPressedLightColor
		{
			get
			{
				return m_DropDownPressedLightColor;
			}
			set
			{
				if( m_DropDownPressedLightColor != value )
				{
					m_DropDownPressedLightColor = value;
				}
			}
		}
		
		/// <summary>
		/// Gets or sets dark color for pressed dropdown button of the CommandBar.
		/// </summary>
		public static Color DropDownPressedDarkColor
		{
			get
			{
				return m_DropDownPressedDarkColor;
			}
			set
			{
				if( m_DropDownPressedDarkColor != value )
				{
					m_DropDownPressedDarkColor = value;
				}
			}
		}


		/// <summary>
		/// Gets or sets light color for CombobBox button.
		/// </summary>
		public static Color ComboButtonLightColor
		{
			get
			{
				return m_ComboButtonLightColor;
			}
			set
			{
				if( m_ComboButtonLightColor != value )
				{
					m_ComboButtonLightColor = value;
				}
			}
		}


		/// <summary>
		/// Gets or sets dark color for CombobBox button.
		/// </summary>
		public static Color ComboButtonDarkColor
		{
			get
			{
				return m_ComboButtonDarkColor;
			}
			set
			{
				if( m_ComboButtonDarkColor != value )
				{
					m_ComboButtonDarkColor = value;
				}
			}
		}


		#endregion

		#region Class Static Methods
		public static void UpdateMenuColors( Office2007Theme theme )
		{
			// initialize colors for Office2007Outlook visual style
			m_BarItemHighlightBorderColor = Color.FromArgb( 255, 189, 105 );
			m_BarItemPressBorderColor = Color.FromArgb( 251, 140, 60 );
			m_BarItemCheckBorderColor = Color.FromArgb( 255, 171, 63 );
			m_BarItemHighlightLightColor = Color.FromArgb( 255, 245, 204 );
			m_BarItemHighlightDarkColor = Color.FromArgb( 255, 220, 122 );
			m_BarItemPressLightColor = Color.FromArgb( 252, 151, 61 );
			m_BarItemPressDarkColor = Color.FromArgb( 255, 184, 94 );
			m_BarItemCheckLightColor = Color.FromArgb( 255, 207, 146 );
			m_BarItemCheckDarkColor = Color.FromArgb( 255, 175, 73 );

			switch( theme )
			{
				case Office2007Theme.Black : // initialize colors for Black colorscheme
				{
					m_MenuCheckMarkColor = Color.FromArgb( 255, 189, 105 );
					m_MenuSelectedCheckMarkColor = Color.FromArgb( 255, 171, 63 );
					m_MenuCheckMarkBorderColor = Color.FromArgb( 255, 171, 63 );
					m_MenuSelectedCheckMarkBorderColor = Color.FromArgb( 251, 140, 60 );
					m_MenuColumnStyleColor = Color.FromArgb( 239, 239, 239 );
					m_MenuSelectedItemBorderColor = Color.FromArgb( 255, 189, 105 );
					m_MenuSelectedItemColor = Color.FromArgb( 255, 231, 162 );
					m_MenuSeparatorColor = Color.FromArgb( 145, 153, 164 );
					m_MenuBorderColor = Color.FromArgb( 145, 153, 164 );
					m_MenuBackground = Color.FromArgb( 246, 246, 246 );

					m_dDBarItemBorderColor = Color.FromArgb( 145, 153, 164 );
					m_dDBarItemLightColor = Color.FromArgb( 145, 153, 164 );
					m_dDBarItemDarkColor = Color.FromArgb( 103, 112, 124 );
					m_DropDownHighlightLightColor = Color.FromArgb( 255, 245, 204 );
					m_DropDownHighlightDarkColor = Color.FromArgb( 255, 219, 117 );
					m_DropDownPressedLightColor = Color.FromArgb( 252, 151, 61 );
					m_DropDownPressedDarkColor = Color.FromArgb( 255, 184, 94 );

					m_ComboButtonLightColor = Color.FromArgb( 200, 204, 209 );
					m_ComboButtonDarkColor = Color.FromArgb( 159, 166, 175 );
					break;
				}
				case Office2007Theme.Silver : // initialize colors for Silver colorscheme
				{
					m_MenuCheckMarkColor = Color.FromArgb( 255, 189, 105 );
					m_MenuSelectedCheckMarkColor = Color.FromArgb( 255, 171, 63 );
					m_MenuCheckMarkBorderColor = Color.FromArgb( 255, 171, 63 );
					m_MenuSelectedCheckMarkBorderColor = Color.FromArgb( 251, 140, 60 );
					m_MenuColumnStyleColor = Color.FromArgb( 239, 239, 239 );
					m_MenuSelectedItemBorderColor = Color.FromArgb( 255, 189, 105 );
					m_MenuSelectedItemColor = Color.FromArgb( 255, 231, 162 );
					m_MenuSeparatorColor = Color.FromArgb( 110, 109, 143 );
					m_MenuBorderColor = Color.FromArgb( 124, 124, 148 );
					m_MenuBackground = Color.FromArgb( 253, 250, 255 );

					m_dDBarItemBorderColor = Color.FromArgb( 124, 124, 148 );
					m_dDBarItemLightColor = Color.FromArgb( 232, 233, 241 );
					m_dDBarItemDarkColor = Color.FromArgb( 180, 179, 200 );
					m_DropDownHighlightLightColor = Color.FromArgb( 255, 245, 204 );
					m_DropDownHighlightDarkColor = Color.FromArgb( 255, 219, 117 );
					m_DropDownPressedLightColor = Color.FromArgb( 252, 151, 61 );
					m_DropDownPressedDarkColor = Color.FromArgb( 255, 184, 94 );

					m_ComboButtonLightColor = Color.FromArgb( 238, 239, 246 );
					m_ComboButtonDarkColor = Color.FromArgb( 173, 172, 196 );
					break;
				}
				default : // initialize colors for default (Blue) colorscheme
				{
					m_MenuCheckMarkColor = Color.FromArgb( 255, 189, 105 );
					m_MenuSelectedCheckMarkColor = Color.FromArgb( 255, 171, 63 );
					m_MenuCheckMarkBorderColor = Color.FromArgb( 255, 171, 63 );
					m_MenuSelectedCheckMarkBorderColor = Color.FromArgb( 251, 140, 60 );
					m_MenuColumnStyleColor = Color.FromArgb( 233, 238, 238 );
					m_MenuSelectedItemBorderColor = Color.FromArgb( 255, 189, 105 );
					m_MenuSelectedItemColor = Color.FromArgb( 255, 231, 162 );
					m_MenuSeparatorColor = Color.FromArgb( 154, 198, 255 );
					m_MenuBorderColor = Color.FromArgb( 101, 147, 207 );
					m_MenuBackground = Color.FromArgb( 246, 246, 246 );

					m_dDBarItemBorderColor = Color.FromArgb( 101, 147, 207 );
					m_dDBarItemLightColor = Color.FromArgb( 227, 239, 254 );
					m_dDBarItemDarkColor = Color.FromArgb( 144, 185, 238 );
					m_DropDownHighlightLightColor = Color.FromArgb( 255, 245, 204 );
					m_DropDownHighlightDarkColor = Color.FromArgb( 255, 219, 117 );
					m_DropDownPressedLightColor = Color.FromArgb( 252, 151, 61 );
					m_DropDownPressedDarkColor = Color.FromArgb( 255, 184, 94 );

					m_ComboButtonLightColor = Color.FromArgb( 227, 239, 255 );
					m_ComboButtonDarkColor = Color.FromArgb( 191, 219, 255 );
					break;
				}
			}
		}

		#endregion

		#region Class Initialize/Finalize Methods
		static Office2007OutlookColors()
		{
			UpdateMenuColors( Office2007Theme.Blue );
		}
		#endregion
	}
    public class Office2010OutlookColors
    {
        #region Class Members
        private static Color m_panelColor = Color.Empty;
        private static Color m_borderColor = Color.Empty;
        private static Color m_tabItemColor = Color.Empty;
        private static Color m_innerBorderColor = Color.Empty;
        private static Color m_leftAHPanelColor = Color.Empty;
        private static Color m_rightAHPanelColor = Color.Empty;

        private static Color m_MenuSelectedItemColor = Color.Empty;
        private static Color m_MenuSelectedItemBorderColor = Color.Empty;
        private static Color m_MenuBorderColor = Color.Empty;
        private static Color m_MenuSeparatorColor = Color.Empty;
        private static Color m_MenuColumnStyleColor = Color.Empty;
        private static Color m_MenuCheckMarkColor = Color.Empty;
        private static Color m_MenuSelectedCheckMarkColor = Color.Empty;
        private static Color m_MenuCheckMarkBorderColor = Color.Empty;
        private static Color m_MenuSelectedCheckMarkBorderColor = Color.Empty;
        private static Color m_MenuBackground = Color.Empty;

        private static Color m_BarItemHighlightBorderColor = Color.Empty;
        private static Color m_BarItemPressBorderColor = Color.Empty;
        private static Color m_BarItemCheckBorderColor = Color.Empty;
        private static Color m_BarItemHighlightLightColor = Color.Empty;
        private static Color m_BarItemHighlightDarkColor = Color.Empty;
        private static Color m_BarItemPressLightColor = Color.Empty;
        private static Color m_BarItemPressDarkColor = Color.Empty;
        private static Color m_BarItemCheckLightColor = Color.Empty;
        private static Color m_BarItemCheckDarkColor = Color.Empty;

        private static Color m_dDBarItemBorderColor = Color.Empty;
        private static Color m_dDBarItemLightColor = Color.Empty;
        private static Color m_dDBarItemDarkColor = Color.Empty;
        private static Color m_DropDownHighlightLightColor = Color.Empty;
        private static Color m_DropDownHighlightDarkColor = Color.Empty;
        private static Color m_DropDownPressedLightColor = Color.Empty;
        private static Color m_DropDownPressedDarkColor = Color.Empty;

        private static Color m_ComboButtonLightColor = Color.Empty;
        private static Color m_ComboButtonDarkColor = Color.Empty;
        #endregion

        #region Class Properties
        /// <summary>
        /// Gets a value indicating whether themed colors are used.
        /// </summary>
        /// <value><c>true</c> if themed colors are used, <c>false</c> otherwise.</value>
        public static bool UseThemedColors
        {
            get
            {
                bool themedcolors = false;
                if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed)
                {
                    if (XPThemes.IsDefaultBlueThemeOn || XPThemes.IsOliveGreenThemeOn || XPThemes.IsSilverThemeOn)
                        themedcolors = true;
                }
                return themedcolors;
            }
        }

        /// <summary>
        /// gets/sets color of right auto hide panel.
        /// </summary>
        public static Color RightAHPanelColor
        {
            get
            {
                return m_rightAHPanelColor;
            }
            set
            {
                if (m_rightAHPanelColor != value)
                {
                    m_rightAHPanelColor = value;
                }
            }
        }

        /// <summary>
        /// Gets/sets color of left AH panel.
        /// </summary>
        public static Color LeftAHPanelColor
        {
            get
            {
                return m_leftAHPanelColor;
            }
            set
            {
                if (m_leftAHPanelColor != value)
                {
                    m_leftAHPanelColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the panel.
        /// </summary>
        public static Color PanelColor
        {
            get
            {
                return m_panelColor;
            }
            set
            {
                if (m_panelColor != value)
                {
                    m_panelColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        public static Color BorderColor
        {
            get
            {
                return m_borderColor;
            }
            set
            {
                if (m_borderColor != value)
                {
                    m_borderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the inner border.
        /// </summary>
        public static Color InnerBorderColor
        {
            get
            {
                return m_innerBorderColor;
            }
            set
            {
                if (m_innerBorderColor != value)
                {
                    m_innerBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the tab item.
        /// </summary>
        public static Color TabItemColor
        {
            get
            {
                return m_tabItemColor;
            }
            set
            {
                if (m_tabItemColor != value)
                {
                    m_tabItemColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for border of the DropDownBarItem.
        /// </summary>
        public static Color DDBarItemBorderColor
        {
            get
            {
                return m_dDBarItemBorderColor;
            }
            set
            {
                if (m_dDBarItemBorderColor != value)
                {
                    m_dDBarItemBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the light color of the DropDownBarItem.
        /// </summary>
        public static Color DDBarItemLightColor
        {
            get
            {
                return m_dDBarItemLightColor;
            }
            set
            {
                if (m_dDBarItemLightColor != value)
                {
                    m_dDBarItemLightColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the dark color of the DropDownBarItem.
        /// </summary>
        public static Color DDBarItemDarkColor
        {
            get
            {
                return m_dDBarItemDarkColor;
            }
            set
            {
                if (m_dDBarItemDarkColor != value)
                {
                    m_dDBarItemDarkColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for border of the menu.
        /// </summary>
        public static Color MenuBorderColor
        {
            get
            {
                return m_MenuBorderColor;
            }
            set
            {
                if (m_MenuBorderColor != value)
                {
                    m_MenuBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for separator of the menu.
        /// </summary>
        public static Color MenuSeparatorColor
        {
            get
            {
                return m_MenuSeparatorColor;
            }
            set
            {
                if (m_MenuSeparatorColor != value)
                {
                    m_MenuSeparatorColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for border selected item of the menu.
        /// </summary>
        public static Color MenuSelectedItemBorderColor
        {
            get
            {
                return m_MenuSelectedItemBorderColor;
            }
            set
            {
                if (m_MenuSelectedItemBorderColor != value)
                {
                    m_MenuSelectedItemBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for selected item of the menu.
        /// </summary>
        public static Color MenuSelectedItemColor
        {
            get
            {
                return m_MenuSelectedItemColor;
            }
            set
            {
                if (m_MenuSelectedItemColor != value)
                {
                    m_MenuSelectedItemColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for column of the menu.
        /// </summary>
        public static Color MenuColumnStyleColor
        {
            get
            {
                return m_MenuColumnStyleColor;
            }
            set
            {
                if (m_MenuColumnStyleColor != value)
                {
                    m_MenuColumnStyleColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for check mark of the menu.
        /// </summary>
        public static Color MenuCheckMarkColor
        {
            get
            {
                return m_MenuCheckMarkColor;
            }
            set
            {
                if (m_MenuCheckMarkColor != value)
                {
                    m_MenuCheckMarkColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for selected check mark of the menu.
        /// </summary>
        public static Color MenuSelectedCheckMarkColor
        {
            get
            {
                return m_MenuSelectedCheckMarkColor;
            }
            set
            {
                if (m_MenuSelectedCheckMarkColor != value)
                {
                    m_MenuSelectedCheckMarkColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for check mark border of the menu.
        /// </summary>
        public static Color MenuCheckMarkBorderColor
        {
            get
            {
                return m_MenuCheckMarkBorderColor;
            }
            set
            {
                if (m_MenuCheckMarkBorderColor != value)
                {
                    m_MenuCheckMarkBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for selected check mark border of the menu.
        /// </summary>
        public static Color MenuSelectedCheckMarkBorderColor
        {
            get
            {
                return m_MenuSelectedCheckMarkBorderColor;
            }
            set
            {
                if (m_MenuSelectedCheckMarkBorderColor != value)
                {
                    m_MenuSelectedCheckMarkBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color of the menu.
        /// </summary>
        public static Color MenuBackground
        {
            get
            {
                return m_MenuBackground;
            }
            set
            {
                if (m_MenuBackground != value)
                {
                    m_MenuBackground = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets border color of the BarItem.
        /// </summary>
        public static Color BarItemHighlightBorderColor
        {
            get
            {
                return m_BarItemHighlightBorderColor;
            }
            set
            {
                if (m_BarItemHighlightBorderColor != value)
                {
                    m_BarItemHighlightBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets border color of the pressed BarItem.
        /// </summary>
        public static Color BarItemPressBorderColor
        {
            get
            {
                return m_BarItemPressBorderColor;
            }
            set
            {
                if (m_BarItemPressBorderColor != value)
                {
                    m_BarItemPressBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets border color of the checked BarItem.
        /// </summary>
        public static Color BarItemCheckBorderColor
        {
            get
            {
                return m_BarItemCheckBorderColor;
            }
            set
            {
                if (m_BarItemCheckBorderColor != value)
                {
                    m_BarItemCheckBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets light color of the checked BarItem.
        /// </summary>
        public static Color BarItemCheckLightColor
        {
            get
            {
                return m_BarItemCheckLightColor;
            }
            set
            {
                if (m_BarItemCheckLightColor != value)
                {
                    m_BarItemCheckLightColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets dark color of the checked BarItem.
        /// </summary>
        public static Color BarItemCheckDarkColor
        {
            get
            {
                return m_BarItemCheckDarkColor;
            }
            set
            {
                if (m_BarItemCheckDarkColor != value)
                {
                    m_BarItemCheckDarkColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets light color of the BarItem.
        /// </summary>
        public static Color BarItemHighlightLightColor
        {
            get
            {
                return m_BarItemHighlightLightColor;
            }
            set
            {
                if (m_BarItemHighlightLightColor != value)
                {
                    m_BarItemHighlightLightColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets dark color of the BarItem.
        /// </summary>
        public static Color BarItemHighlightDarkColor
        {
            get
            {
                return m_BarItemHighlightDarkColor;
            }
            set
            {
                if (m_BarItemHighlightDarkColor != value)
                {
                    m_BarItemHighlightDarkColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets light color of the pressed BarItem.
        /// </summary>
        public static Color BarItemPressLightColor
        {
            get
            {
                return m_BarItemPressLightColor;
            }
            set
            {
                if (m_BarItemPressLightColor != value)
                {
                    m_BarItemPressLightColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets dark color of the pressed BarItem.
        /// </summary>
        public static Color BarItemPressDarkColor
        {
            get
            {
                return m_BarItemPressDarkColor;
            }
            set
            {
                if (m_BarItemPressDarkColor != value)
                {
                    m_BarItemPressDarkColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets light color for highlight dropdown button of the CommandBar.
        /// </summary>
        public static Color DropDownHighlightLightColor
        {
            get
            {
                return m_DropDownHighlightLightColor;
            }
            set
            {
                if (m_DropDownHighlightLightColor != value)
                {
                    m_DropDownHighlightLightColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets dark color for highlight dropdown button of the CommandBar.
        /// </summary>
        public static Color DropDownHighlightDarkColor
        {
            get
            {
                return m_DropDownHighlightDarkColor;
            }
            set
            {
                if (m_DropDownHighlightDarkColor != value)
                {
                    m_DropDownHighlightDarkColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets light color for pressed dropdown button of the CommandBar.
        /// </summary>
        public static Color DropDownPressedLightColor
        {
            get
            {
                return m_DropDownPressedLightColor;
            }
            set
            {
                if (m_DropDownPressedLightColor != value)
                {
                    m_DropDownPressedLightColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets dark color for pressed dropdown button of the CommandBar.
        /// </summary>
        public static Color DropDownPressedDarkColor
        {
            get
            {
                return m_DropDownPressedDarkColor;
            }
            set
            {
                if (m_DropDownPressedDarkColor != value)
                {
                    m_DropDownPressedDarkColor = value;
                }
            }
        }


        /// <summary>
        /// Gets or sets light color for CombobBox button.
        /// </summary>
        public static Color ComboButtonLightColor
        {
            get
            {
                return m_ComboButtonLightColor;
            }
            set
            {
                if (m_ComboButtonLightColor != value)
                {
                    m_ComboButtonLightColor = value;
                }
            }
        }


        /// <summary>
        /// Gets or sets dark color for CombobBox button.
        /// </summary>
        public static Color ComboButtonDarkColor
        {
            get
            {
                return m_ComboButtonDarkColor;
            }
            set
            {
                if (m_ComboButtonDarkColor != value)
                {
                    m_ComboButtonDarkColor = value;
                }
            }
        }


        #endregion

        #region Class Static Methods
        public static void UpdateMenuColors(Office2010Theme theme)
        {
            // initialize colors for Office2010Outlook visual style
            m_BarItemHighlightBorderColor = Color.FromArgb(255, 189, 105);
            m_BarItemPressBorderColor = Color.FromArgb(251, 140, 60);
            m_BarItemCheckBorderColor = Color.FromArgb(255, 171, 63);
            m_BarItemHighlightLightColor = Color.FromArgb(255, 245, 204);
            m_BarItemHighlightDarkColor = Color.FromArgb(255, 220, 122);
            m_BarItemPressLightColor = Color.FromArgb(252, 151, 61);
            m_BarItemPressDarkColor = Color.FromArgb(255, 184, 94);
            m_BarItemCheckLightColor = Color.FromArgb(255, 207, 146);
            m_BarItemCheckDarkColor = Color.FromArgb(255, 175, 73);

            switch (theme)
            {
                case Office2010Theme.Black: // initialize colors for Black colorscheme
                    {
                        m_MenuCheckMarkColor = Color.FromArgb(255, 189, 105);
                        m_MenuSelectedCheckMarkColor = Color.FromArgb(255, 171, 63);
                        m_MenuCheckMarkBorderColor = Color.FromArgb(255, 171, 63);
                        m_MenuSelectedCheckMarkBorderColor = Color.FromArgb(251, 140, 60);
                        m_MenuColumnStyleColor = Color.FromArgb(239, 239, 239);
                        m_MenuSelectedItemBorderColor = Color.FromArgb(255, 189, 105);
                        m_MenuSelectedItemColor = Color.FromArgb(255, 231, 162);
                        m_MenuSeparatorColor = Color.FromArgb(145, 153, 164);
                        m_MenuBorderColor = Color.FromArgb(145, 153, 164);
                        m_MenuBackground = Color.FromArgb(246, 246, 246);

                        m_dDBarItemBorderColor = Color.FromArgb(145, 153, 164);
                        m_dDBarItemLightColor = Color.FromArgb(145, 153, 164);
                        m_dDBarItemDarkColor = Color.FromArgb(103, 112, 124);
                        m_DropDownHighlightLightColor = Color.FromArgb(255, 245, 204);
                        m_DropDownHighlightDarkColor = Color.FromArgb(255, 219, 117);
                        m_DropDownPressedLightColor = Color.FromArgb(252, 151, 61);
                        m_DropDownPressedDarkColor = Color.FromArgb(255, 184, 94);

                        m_ComboButtonLightColor = Color.FromArgb(200, 204, 209);
                        m_ComboButtonDarkColor = Color.FromArgb(159, 166, 175);
                        break;
                    }
                case Office2010Theme.Silver: // initialize colors for Silver colorscheme
                    {
                        m_MenuCheckMarkColor = Color.FromArgb(255, 189, 105);
                        m_MenuSelectedCheckMarkColor = Color.FromArgb(255, 171, 63);
                        m_MenuCheckMarkBorderColor = Color.FromArgb(255, 171, 63);
                        m_MenuSelectedCheckMarkBorderColor = Color.FromArgb(251, 140, 60);
                        m_MenuColumnStyleColor = Color.FromArgb(239, 239, 239);
                        m_MenuSelectedItemBorderColor = Color.FromArgb(255, 189, 105);
                        m_MenuSelectedItemColor = Color.FromArgb(255, 231, 162);
                        m_MenuSeparatorColor = Color.FromArgb(110, 109, 143);
                        m_MenuBorderColor = Color.FromArgb(124, 124, 148);
                        m_MenuBackground = Color.FromArgb(253, 250, 255);

                        m_dDBarItemBorderColor = Color.FromArgb(124, 124, 148);
                        m_dDBarItemLightColor = Color.FromArgb(232, 233, 241);
                        m_dDBarItemDarkColor = Color.FromArgb(180, 179, 200);
                        m_DropDownHighlightLightColor = Color.FromArgb(255, 245, 204);
                        m_DropDownHighlightDarkColor = Color.FromArgb(255, 219, 117);
                        m_DropDownPressedLightColor = Color.FromArgb(252, 151, 61);
                        m_DropDownPressedDarkColor = Color.FromArgb(255, 184, 94);

                        m_ComboButtonLightColor = Color.FromArgb(238, 239, 246);
                        m_ComboButtonDarkColor = Color.FromArgb(173, 172, 196);
                        break;
                    }
                default: // initialize colors for default (Blue) colorscheme
                    {
                        m_MenuCheckMarkColor = Color.FromArgb(255, 189, 105);
                        m_MenuSelectedCheckMarkColor = Color.FromArgb(255, 171, 63);
                        m_MenuCheckMarkBorderColor = Color.FromArgb(255, 171, 63);
                        m_MenuSelectedCheckMarkBorderColor = Color.FromArgb(251, 140, 60);
                        m_MenuColumnStyleColor = Color.FromArgb(233, 238, 238);
                        m_MenuSelectedItemBorderColor = Color.FromArgb(255, 189, 105);
                        m_MenuSelectedItemColor = Color.FromArgb(255, 231, 162);
                        m_MenuSeparatorColor = Color.FromArgb(154, 198, 255);
                        m_MenuBorderColor = Color.FromArgb(101, 147, 207);
                        m_MenuBackground = Color.FromArgb(246, 246, 246);

                        m_dDBarItemBorderColor = Color.FromArgb(101, 147, 207);
                        m_dDBarItemLightColor = Color.FromArgb(227, 239, 254);
                        m_dDBarItemDarkColor = Color.FromArgb(144, 185, 238);
                        m_DropDownHighlightLightColor = Color.FromArgb(255, 245, 204);
                        m_DropDownHighlightDarkColor = Color.FromArgb(255, 219, 117);
                        m_DropDownPressedLightColor = Color.FromArgb(252, 151, 61);
                        m_DropDownPressedDarkColor = Color.FromArgb(255, 184, 94);

                        m_ComboButtonLightColor = Color.FromArgb(227, 239, 255);
                        m_ComboButtonDarkColor = Color.FromArgb(191, 219, 255);
                        break;
                    }
            }
        }

        #endregion

        #region Class Initialize/Finalize Methods
        static Office2010OutlookColors()
        {
            UpdateMenuColors(Office2010Theme.Blue);
        }
        #endregion
    }
    /// <summary>
    /// Provides colors for blue colorscheme of the Office2010 visual style.
    /// </summary>
    public class Office2010BlueColors : Office2010Colors
    {
        #region Class Overrides
        /// <summary>
        /// Initialize colors for Blue colorscheme of the Office2010 visual style.
        /// </summary>
        protected override void InitializeColors()
        {
            base.InitializeColors();

            // colors for menu
            m_MenuTextBoxBorderColor = Color.FromArgb(179, 199, 225);
            m_MenuTextBoxBackColor = Color.FromArgb(234, 242, 251);
            m_MenuComboButtonHighlightLightColor = Color.FromArgb(232, 241, 253);
            m_MenuComboButtonHighlightDarkColor = Color.FromArgb(207, 223, 243);
            m_MenuComboButtonArrowColor = Color.FromArgb(86, 125, 177);
            m_MenuItemArrowLightColor = Color.FromArgb(106, 126, 197);
            m_MenuItemArrowDarkColor = Color.FromArgb(64, 70, 90);

            // colors for CommandBar
            m_CommandBarDarkColor = Color.FromArgb(166, 194, 225);
            m_CommandBarLightColor = Color.FromArgb(221, 233, 246);
            m_CommandBarBorderColor = Color.FromArgb(221, 233, 246);

            // colors for DropDown button of the CommandBar
            m_DockBarBackColor = Color.FromArgb(187, 209, 233);
            m_DropDownDarkColor = Color.FromArgb(204, 215, 233);
            m_DropDownLightColor = Color.FromArgb(244, 248, 251);
            m_DropDownHighlightLightColor = Color.FromArgb(255, 248, 237);
            m_DropDownHighlightDarkColor = Color.FromArgb(255, 193, 118);
            m_DropDownPressedLightColor = Color.FromArgb(254, 149, 82);
            m_DropDownPressedDarkColor = Color.FromArgb(255, 217, 149);

            // colors for floating CommandBar
            m_FloatHighlightButtonColor = Color.FromArgb(255, 231, 162);
            m_FloatHighlightButtonBorderColor = Color.FromArgb(255, 189, 105);
            m_FloatPressButtonColor = Color.FromArgb(214, 232, 255);
            m_FloatPressButtonBorderColor = Color.FromArgb(101, 147, 207);
            m_FloatPressCloseButtonBorderColor = Color.FromArgb(251, 140, 60);
            m_FloatPressCloseButtonColor = Color.FromArgb(251, 140, 60);
            m_FloatCommandBarLightColor = Color.White;
            m_FloatCommandBarDarkColor = Color.White;
            m_FloatLightBorderColor = Color.FromArgb(194, 220, 255);
            m_FloatBackgroundColor = Color.FromArgb(187, 206, 230);
            m_FloatBorderColor = Color.FromArgb(187, 206, 230);
            m_FloatCaptionColor = Color.FromArgb(64, 22, 157);

            // color for separator in a CommandBar
            m_BarItemSeparatorColor = Color.FromArgb(154, 198, 255);

            // colors for ComboButton of the ComboBoxBarItem in CommandBar
            m_ComboButtonLightColor = Color.FromArgb(201, 221, 246);
            m_ComboButtonDarkColor = Color.FromArgb(160, 189, 224);
            m_ComboButtonBorder = Color.FromArgb(138, 173, 219);

            // colors for tab group
            m_TabItemBorderColor = Color.FromArgb(153, 187, 232);
            m_TabItemInnerBorderColor = Color.FromArgb(239, 246, 255);
            m_TabItemOuterBorderColor = Color.FromArgb(209, 229, 254);
            m_TabItemTextColor = Color.FromArgb(21, 66, 139);
            m_TabItemActiveBottomColor = Color.FromArgb(239, 211, 156);
            m_TabItemTopGradientColor = Color.FromArgb(196, 221, 254);
            m_TabItemInActiveBottomColor = Color.FromArgb(235, 243, 253);
            m_TabItemMiddleLineColor = Color.FromArgb(215, 226, 232);
            m_TabPanelColor = Color.FromArgb(188, 208, 232);
            m_TabPanelBorderColor = Color.FromArgb(219, 232, 249);
            m_TabPanelBackColor = Color.FromArgb(187, 206, 230);

            // colors for GroupBar
            m_GroupBarBorderColor = Color.FromArgb(99, 146, 206);
            m_GroupBarHeaderColorLight = Color.FromArgb(219, 233, 246);
            m_GroupBarHeaderColorDark = Color.FromArgb(186, 208, 232);
            m_GroupBarItemTextColor = Color.FromArgb(33, 77, 140);
            m_GroupBarBackColor = Color.FromArgb(198, 215, 234);
            m_GroupBarHeaderTextColor = Color.FromArgb(16, 65, 140);
            m_GroupBarItemColorLight = m_GroupBarHeaderColorLight;
            m_GroupBarItemColorDark = m_GroupBarHeaderColorDark;
            m_GroupBarSplitterColorDark = Color.FromArgb(189, 219, 255);
            m_GroupBarSplitterColorLight = Color.FromArgb(239, 243, 255);
            m_GroupBarClientAreaBackground = Color.FromArgb(213, 228, 242);
            m_GroupBarHighlightColorLight = Color.FromArgb(229, 241, 252);
            m_GroupBarHighlightColorDark = Color.FromArgb(195, 219, 241);
            m_GroupBarSelectedColorLight = Color.FromArgb(184, 206, 231);
            m_GroupBarSelectedColorDark = Color.FromArgb(229, 241, 252);
            m_GroupBarSelectedTopColorLight = Color.FromArgb(199, 219, 239);
            m_GroupBarSelectedTopColorDark = Color.FromArgb(184, 206, 231);
            m_GroupBarSelectedHighlightColorLight = Color.FromArgb(229, 241, 252);
            m_GroupBarSelectedHighlightColorDark = Color.FromArgb(195, 219, 241); 

            //// colors for DataTimePicker
            //m_DataTimePickerBorderColor = Color.FromArgb(139, 160, 188);

            //m_DataTimePickerDropDownArrowColor = Color.FromArgb(88, 101, 133);
            //m_DataTimePickerDropDownLightColor =  Color.FromArgb(222, 234, 248);
            //m_DataTimePickerDropDownDarkColor = Color.Red; 

            //m_DataTimePickerCheckBoxBorderNormalColor = Color.Red; 
            //m_DataTimePickerCheckBoxBorderPushedColor = Color.Red; Color.FromArgb(194, 129, 51);
            // colors for DataTimePicker
            m_DataTimePickerBorderColor =  Color.FromArgb(139, 160, 188);

            m_DataTimePickerDropDownArrowColor =  Color.FromArgb(86, 125, 177);
            m_DataTimePickerDropDownLightColor = Color.FromArgb(252, 253, 254);
            m_DataTimePickerDropDownDarkColor = Color.FromArgb(181, 203, 232);

            m_DataTimePickerCheckBoxBorderNormalColor = Color.FromArgb(171, 193, 222);
            m_DataTimePickerCheckBoxBorderPushedColor = Color.FromArgb(85, 119, 163);

            

            //colors for MonthCalendarAdv
            m_MonthCalendarHeaderStartColor = Color.FromArgb(208, 221, 238);
            m_MonthCalendarHeaderEndColor = Color.FromArgb(208, 221, 238);
            m_MonthCalendarForeColor = SystemColors.ControlText;
            m_MonthCalendarBackgroundColor = Color.FromArgb(208, 221, 238);

            // colors for XPTaskPane
            m_XPTaskPaneInternalBorderColor = Color.FromArgb(221, 237, 253);
            m_XPTaskPaneBorderColor = Color.FromArgb(145, 183, 249);
            m_XPTaskPageBackColor = Color.FromArgb(221, 237, 253);

            // colors for TabControlAdv
            m_TabDefaultBorderColor = Color.FromArgb(184, 201, 219);
            m_TabHotLightBottomBorderLineColor = Color.Red;
            m_TabHotLightGradientTopBeginColor = Color.Red;
            m_TabHotLightGradientTopEndColor = Color.Red;
            m_TabHotLightGradientBottomBeginColor = Color.Red;
            m_TabHotLightGradientBottomEndColor = Color.Red;
            m_TabHotLightGradientCircleColor = Color.FromArgb(196, 221, 254);
            m_TabSelectedGradientTopColor = Color.FromArgb(242, 249, 255);
            m_TabSelectedGradientBottomColor = Color.FromArgb(240, 246, 253);
            m_TabSelectedInnerBorderColor = Color.FromArgb(184, 201, 219);
            m_TabHighlightInnerBorderColor = Color.FromArgb(234, 237, 253);
            m_TabSelectedHotLightBorderColor = Color.FromArgb(255, 208, 48);
            m_TabSelectedHotLightInnerBorderColor = Color.FromArgb(255, 240, 187);
            m_TabForeColor = Color.FromArgb(21, 66, 139);
            m_ActiveTabForeColor = m_TabForeColor;
            m_TabBackgroundColor = Color.FromArgb(224, 249, 254);
            m_TabScrollArrowColor = Color.FromArgb(86, 125, 177);

            // colors for DockTabControl 
            m_DockTabForeColor = Color.FromArgb(21, 66, 139);
            m_DockTabBackgroundColor = Color.FromArgb(199, 216, 237);

            // colors for TextBoxExt 
            m_ActiveTextBoxBorderColor = Color.FromArgb(235, 137, 0);
            m_InactiveTextBoxBorderColor = Color.FromArgb(185, 199, 220);
            m_ActiveTextBoxBackColor = Color.FromArgb(255, 255, 255);
            m_InactiveTextBoxBackColor = Color.FromArgb(255, 255, 255);
            
            //TreeviewAdv
            m_SelectedNodeBackground = Color.FromArgb(169, 192, 223);
            m_TreeNodeArrowColor = Color.FromArgb(129, 129, 129);
            m_TreeviewBackColor = Color.FromArgb(207, 221, 238);
            m_TreeViewFontColor = Color.FromArgb(30, 57, 91);

            // form's colors
            m_ActiveFormBorderColor = Color.FromArgb(186,209,229);
            m_InactiveFormBorderColor = Color.FromArgb(214,227,239);

            m_FormTextColor = Color.FromArgb(57, 105, 173);

            m_ActiveTitleGradientBegin = Color.FromArgb(220,233,246);
            m_ActiveTitleGradientEnd = Color.FromArgb(188,207,233);
            m_InactiveTitleGradientBegin = Color.FromArgb(234,242,250);
            m_InactiveTitleGradientEnd = Color.FromArgb(215,226,242);

            m_SystemButtonSelectedGradientBegin = Color.FromArgb(247,248,249);
            m_SystemButtonSelectedGradientEnd = Color.FromArgb(220,228,234);
            m_SystemButtonPressedGradientBegin = Color.FromArgb(216, 226, 234);
            m_SystemButtonPressedGradientEnd = Color.FromArgb(142, 173, 193);

            m_SystemButtonBorderSelected = Color.FromArgb(169,195,224);
            m_SystemButtonBorderPressed = Color.FromArgb(131, 161, 183);
            m_FormBackground = Color.FromArgb(206,220,237);

            // upDown colors
            m_UpDownArrowStartColor = Color.FromArgb(24, 82, 172);
            m_UpDownArrowEndColor = Color.FromArgb(13, 50, 103);

            m_UpDownBorderNormalColor = Color.FromArgb(177, 197, 218);

            m_UpDownBackgroundNormalColor = Color.FromArgb(237, 245, 253);
            m_UpDownBackgroundNormalStartColor = Color.FromArgb(207, 223, 243);
            m_UpDownBackgroundNormalEndColor = Color.FromArgb(231, 241, 253);

            // colors for comboBoxAdv
            m_ComboBoxAdvNormalBackColor = SystemColors.Window;
            m_ComboBoxAdvHotBackColor =  Color.FromArgb(234, 242, 251);
            m_ComboBoxAdvNormalBorderColor = Color.FromArgb(171, 186, 208);
            m_ComboBoxAdvHotBorderColor =  Color.FromArgb(194, 130, 51);
            m_ComboBoxAdvPushedBorderColor = Color.FromArgb(157, 146, 102);
            m_ComboBoxAdvButtonUpperLineColor = Color.FromArgb(192, 185, 165);
            m_ComboBoxAdvArrowColor = Color.FromArgb(75, 72, 56);
            m_ComboBoxAdvLowerArrowLineColor = Color.FromArgb(255, 248, 203);
            m_ComboBoxAdvHotBackgroundButtonColor1 = Color.FromArgb(244, 225, 153);
            m_ComboBoxAdvHotBackgroundButtonColor2 = Color.FromArgb(245, 219, 124);
            m_ComboBoxAdvHotBackgroundButtonColor3 = Color.FromArgb(245, 210, 130);
            m_ComboBoxAdvHotBackgroundButtonColor4 = Color.FromArgb(246, 200, 138);
            m_ComboBoxAdvNormalBackgroundButtonColor1 = Color.FromArgb(226, 226, 226);
            m_ComboBoxAdvNormalBackgroundButtonColor2 = Color.FromArgb(220, 220, 220);
            m_ComboBoxAdvNormalBackgroundButtonColor3 = Color.FromArgb(209, 209, 209);
            m_ComboBoxAdvNormalBackgroundButtonColor4 = Color.FromArgb(196, 196, 196);
            m_ComboBoxAdvPushedBackgroundButtonColor1 = Color.FromArgb(250, 211, 115);
            m_ComboBoxAdvPushedBackgroundButtonColor2 = Color.FromArgb(253, 228, 124);
            m_ComboBoxAdvPushedBackgroundButtonColor3 = Color.FromArgb(254, 220, 130);
            m_ComboBoxAdvPushedBackgroundButtonColor4 = Color.FromArgb(255, 228, 138);

            // colors for CheckBoxAdv
            m_CheckBoxAdvNormalBackColor = Color.FromArgb(248, 248, 248);
            m_CheckBoxAdvSelectedBackColor = Color.FromArgb(254, 248, 232);
            m_CheckBoxAdvPushedBackColor = Color.FromArgb(255, 244, 213);
            m_CheckBoxAdvNormalBorderColor = Color.FromArgb(171, 193, 222);
            m_CheckBoxAdvSelectedBorderColor = Color.FromArgb(85, 119, 163);
            m_CheckBoxAdvPushedBorderColor = Color.FromArgb(85, 119, 163);
            m_CheckBoxAdvNormalInternalBorderColor = Color.FromArgb(244, 244, 244);
            m_CheckBoxAdvSelectedInternalBorderColor = Color.FromArgb(222, 234, 250);
            m_CheckBoxAdvPushedInternalBorderColor = Color.FromArgb(193, 216, 245);
            m_CheckBoxAdvNormalInternalRectangleBorderColor = Color.FromArgb(162, 172, 185);
            m_CheckBoxAdvSelectedInternalRectangleBorderColor = Color.FromArgb(250, 213, 122);
            m_CheckBoxAdvPushedInternalRectangleBorderColor = Color.FromArgb(242, 137, 38);
            m_CheckBoxAdvNormalInternalRectangleColor = Color.FromArgb(202, 207, 213);
            m_CheckBoxAdvSelectedInternalRectangleColor = Color.FromArgb(252, 231, 175);
            m_CheckBoxAdvPushedInternalRectangleColor = Color.FromArgb(255, 208, 103);
            m_CheckBoxAdvNormalTickColor = Color.FromArgb(74, 107, 150);
            m_CheckBoxAdvSelectedTickColor = Color.FromArgb(78, 108, 141);
            m_CheckBoxAdvPushedTickColor = Color.FromArgb(79, 105, 130);
            m_CheckBoxAdvDisabledTickColor = Color.FromArgb(176, 190, 208);
            m_CheckBoxAdvIndeterminateRectangleColor = Color.FromArgb(158, 168, 178);
            m_CheckBoxAdvDisabledBackColor = Color.FromArgb(238, 240, 242);
            m_CheckBoxAdvDisabledBorderColor = Color.FromArgb(174, 177, 181);
            m_CheckBoxAdvDisabledInternalBorderColor = Color.FromArgb(224, 226, 229);

            // colors for RadioButtonAdv
            m_RadioButtonAdvNormalBackColor = SystemColors.Window;
            m_RadioButtonAdvNormalBorderColor =  Color.FromArgb(148, 175, 214);
            m_RadioButtonAdvNormalInternalBorderColor = Color.FromArgb(162, 172, 185);
            m_RadioButtonAdvSelectedBackColor = Color.FromArgb(227, 252, 255);
            m_RadioButtonAdvSelectedBorderColor =  Color.FromArgb(85, 119, 163);
            m_RadioButtonAdvSelectedInternalBorderColor = Color.FromArgb(250, 205, 101);
            m_RadioButtonAdvPushedBackColor = Color.FromArgb(205, 242, 255);
            m_RadioButtonAdvPushedBorderColor = Color.FromArgb(85, 119, 163);
            m_RadioButtonAdvPushedInternalBorderColor = Color.FromArgb(244, 171, 14);
            m_RadioButtonAdvCheckMarkBorderColor = Color.FromArgb(17, 69, 103);
            m_RadioButtonAdvCheckMarkNormalBottomColor = Color.FromArgb(11, 130, 199);
            m_RadioButtonAdvCheckMarkSelectedBottomColor = Color.FromArgb(13, 160, 243);
            m_RadioButtonAdvCheckMarkPushedBottomColor = Color.FromArgb(7, 84, 131);

            // colors for TabBarSplitterControl
            m_TabBarSplitterBackColor = Color.FromArgb(231, 243, 255);
            m_TabBarSplitterBorderColor = Color.FromArgb(148, 166, 198);
            m_TabBarSplitterTextColor = Color.FromArgb(16, 65, 140);
            m_TabBarSplitterTabStartColor = Color.FromArgb(222, 231, 255);
            m_TabBarSplitterTabEndColor = Color.FromArgb(189, 211, 247);
            m_TabBarSplitterTabBarStartColor = Color.FromArgb(165, 190, 231);
            m_TabBarSplitterTabBarEndColor = Color.FromArgb(132, 166, 214);
            m_TabBarSplitterButtonHoveredStartColor = Color.White;
            m_TabBarSplitterButtonHoveredEndColor = Color.FromArgb(222, 231, 247);
            m_TabBarSplitterButtonPushedStartColor = Color.FromArgb(206, 223, 255);
            m_TabBarSplitterButtonPushedEndColor = Color.FromArgb(148, 182, 239);
            m_TabBarSplitterSizeGripperColor = Color.FromArgb(127, 163, 211);
            m_TabBarSplitterSizeGripperLightColor = Color.FromArgb(177, 201, 232);
            m_TabBarSplitterSizeGripperDarkColor = Color.FromArgb(69, 93, 128);

            //colors for XPTaskBar
            m_XPTaskBarBorderColor = Color.FromArgb(162, 184, 212);
            m_XPTaskBarBoxBackColor = Color.FromArgb(184, 206, 231);
            m_XPTaskBarBoxForeColor = Color.FromArgb(0, 0, 0);
            m_XPTaskBarBoxHeaderLowerLineColor = Color.FromArgb(133, 158, 191);
            m_XPTaskBarBoxHeaderUpperLineColor = Color.FromArgb(162, 184, 212);
            m_XPTaskBarBoxArrowColor = Color.FromArgb(62, 62, 71);
            m_XPTaskBarBoxActiveHighlightedItemColor = Color.FromArgb(255, 232, 156);
            m_XPTaskBarBoxInactiveHighlightedItemColor = Color.FromArgb( 229, 229, 229 );

            // colors for ColorUIAdv
            m_ColorUIAdvGroupHeaderBackColor = Color.FromArgb(222, 230, 238);

            // colors for ButtonAdv
            m_ButtonDefaultTopColor = Color.FromArgb(231, 242, 255);
            m_ButtonDefaultBottomColor = ColorTranslator.FromHtml("#bed1ea");
            m_ButtonDefaultBorderColor = Color.FromArgb(160, 178, 200);
            m_ButtonDefaultInternalBorderColor = Color.FromArgb(128, Color.FromArgb(250, 251, 255));
            m_ButtonPressedInternalBorderColor = Color.FromArgb(70, Color.FromArgb(176, 132, 92));
            m_ButtonSelectedInternalBorderColor = Color.FromArgb(128, Color.FromArgb(250, 251, 255));
        }
        #endregion
    }

    /// <summary>
    /// Provides colors for silver colorscheme of the Office2010 visual style.
    /// </summary>
    public class Office2010SilverColors : Office2010Colors
    {
        #region Class Overrides
        /// <summary>
        /// Initialize colors for Silver colorscheme of the Office2010 visual style.
        /// </summary>
        protected override void InitializeColors()
        {
            base.InitializeColors();

            // colors for menu
            m_MenuTextBoxBorderColor = Color.FromArgb(169, 177, 184);
            m_MenuTextBoxBackColor = Color.FromArgb(232, 234, 236);
            m_MenuComboButtonHighlightLightColor = Color.FromArgb(239, 242, 246);
            m_MenuComboButtonHighlightDarkColor = Color.FromArgb(218, 224, 231);
            m_MenuComboButtonArrowColor = Color.FromArgb(124, 124, 124);
            m_MenuItemArrowLightColor = Color.FromArgb(158, 158, 158);
            m_MenuItemArrowDarkColor = Color.FromArgb(124, 124, 124);

            // colors for CommandBar
            m_CommandBarDarkColor = Color.FromArgb(255, 255, 255);
            m_CommandBarLightColor = Color.FromArgb(241, 243, 244);
            m_CommandBarBorderColor = Color.FromArgb(241, 243, 244);

            // colors for DropDown button of the CommandBar
            m_DockBarBackColor = Color.FromArgb(249, 250, 251);
            m_DropDownDarkColor = Color.FromArgb(220, 221, 221);
            m_DropDownLightColor = Color.FromArgb(243, 245, 248); 
            m_DropDownHighlightLightColor = Color.FromArgb(255, 248, 211);
            m_DropDownHighlightDarkColor = Color.FromArgb(255, 193, 118);
            m_DropDownPressedLightColor = Color.FromArgb(254, 149, 82);
            m_DropDownPressedDarkColor = Color.FromArgb(255, 217, 149);

            // colors for floating CommandBar
            m_FloatHighlightButtonColor = Color.FromArgb(255, 231, 162);
            m_FloatHighlightButtonBorderColor = Color.FromArgb(255, 189, 105);
            m_FloatPressButtonColor = Color.FromArgb(208, 212, 217);
            m_FloatPressButtonBorderColor = Color.FromArgb(124, 124, 148);
            m_FloatPressCloseButtonBorderColor = Color.FromArgb(251, 140, 60);
            m_FloatPressCloseButtonColor = Color.FromArgb(251, 140, 60);
            m_FloatCommandBarLightColor = Color.White;
            m_FloatCommandBarDarkColor = Color.White;
            m_FloatLightBorderColor = Color.FromArgb(219, 218, 228);
            m_FloatBackgroundColor = Color.FromArgb(151, 151, 151);
            m_FloatBorderColor = Color.FromArgb(151, 151, 115);
            m_FloatCaptionColor = Color.White;

            // color for separator in a CommandBar
            m_BarItemSeparatorColor = Color.FromArgb(110, 109, 143);

            // colors for ComboButton of the ComboBoxBarItem in CommandBar
            m_ComboButtonLightColor = Color.FromArgb(243, 244, 245);
            m_ComboButtonDarkColor = Color.FromArgb(251, 251, 251);
            m_ComboButtonBorder = Color.FromArgb(212, 214, 217);

            // colors for tab group
            m_TabItemBorderColor = Color.FromArgb(189, 190, 198);
            m_TabItemInnerBorderColor = Color.FromArgb(239, 235, 239);
            m_TabItemOuterBorderColor = Color.FromArgb(231, 229, 239);
            m_TabItemTextColor = Color.FromArgb(74, 81, 90);
            m_TabItemActiveBottomColor = Color.FromArgb(239, 211, 156);
            m_TabItemTopGradientColor = Color.FromArgb(222, 219, 231);
            m_TabItemInActiveBottomColor = Color.FromArgb(231, 231, 239);
            m_TabItemMiddleLineColor = Color.FromArgb(198, 190, 198);
            m_TabPanelColor = Color.Red;// Color.FromArgb(214, 215, 222);
            m_TabPanelBorderColor = Color.FromArgb(189, 190, 189);
            m_TabPanelBackColor = Color.FromArgb(227, 230, 232);

            // colors for GroupBar
            m_GroupBarBorderColor = Color.FromArgb(161, 169, 180);
            m_GroupBarHeaderColorLight = Color.FromArgb(237, 241, 244);
            m_GroupBarHeaderColorDark = Color.FromArgb(221, 227, 231);
            m_GroupBarItemTextColor = Color.FromArgb(74, 81, 90);
            m_GroupBarBackColor = Color.FromArgb(222, 226, 234);
            m_GroupBarHeaderTextColor = Color.FromArgb(74, 81, 90);
            m_GroupBarItemColorLight = Color.FromArgb(241, 237, 241);
            m_GroupBarItemColorDark = m_GroupBarHeaderColorDark;
            m_GroupBarSplitterColorDark = Color.FromArgb(222, 227, 228); 
            m_GroupBarSplitterColorLight = Color.FromArgb(220, 226, 230); 
            m_GroupBarClientAreaBackground = Color.FromArgb(238, 238, 244);
            m_GroupBarHighlightColorLight = Color.FromArgb(239, 242, 247);
            m_GroupBarHighlightColorDark = Color.FromArgb(231, 232, 237);
            m_GroupBarSelectedColorLight = Color.FromArgb(225, 230, 234);
            m_GroupBarSelectedColorDark = Color.FromArgb(237, 238, 244);
            m_GroupBarSelectedTopColorLight = Color.FromArgb(222, 227, 228);
            m_GroupBarSelectedTopColorDark = Color.FromArgb(220, 226, 230);
            m_GroupBarSelectedHighlightColorLight = Color.FromArgb(218, 218, 214);
            m_GroupBarSelectedHighlightColorDark = Color.FromArgb(231, 237, 241); 
            // colors for DataTimePicker
            m_DataTimePickerBorderColor = Color.FromArgb(169, 177, 184);

            m_DataTimePickerDropDownArrowColor = Color.FromArgb(124, 124, 124);
            m_DataTimePickerDropDownLightColor = Color.FromArgb(223, 227, 231);
            m_DataTimePickerDropDownDarkColor = Color.FromArgb(208, 212, 221);

            m_DataTimePickerCheckBoxBorderNormalColor = Color.FromArgb(155, 157, 160);
            m_DataTimePickerCheckBoxBorderPushedColor = Color.FromArgb(107, 113, 115);

            m_DataTimePickerHighLightedForeColor = Color.FromArgb(107, 113, 115);

           
            //colors for MonthCalendarAdv
            m_MonthCalendarHeaderStartColor = Color.FromArgb(233, 236, 241);
            m_MonthCalendarHeaderEndColor = Color.FromArgb(233, 236, 241);
            m_MonthCalendarForeColor = SystemColors.ControlText;
            m_MonthCalendarBackgroundColor = Color.FromArgb(233, 236, 241);

            // colors for XPTaskPane
            m_XPTaskPaneInternalBorderColor = Color.FromArgb(232, 235, 248);
            m_XPTaskPaneBorderColor = Color.FromArgb(158, 160, 160);
            m_XPTaskPageBackColor = Color.Red;// Color.FromArgb(232, 235, 248);

            // colors for TabControlAdv
            m_TabDefaultBorderColor = Color.FromArgb(189, 190, 189);
            m_TabHotLightBottomBorderLineColor = Color.FromArgb(239, 186, 116);
            m_TabHotLightGradientTopBeginColor = Color.FromArgb(222, 219, 222);
            m_TabHotLightGradientTopEndColor = Color.FromArgb(231, 227, 231);
            m_TabHotLightGradientBottomBeginColor = Color.FromArgb(231, 215, 173);
            m_TabHotLightGradientBottomEndColor = Color.FromArgb(239, 211, 140);
            m_TabHotLightGradientCircleColor = Color.FromArgb(222, 219, 231);
            m_TabSelectedGradientTopColor = Color.FromArgb(254, 254, 254);
            m_TabSelectedGradientBottomColor = Color.FromArgb(255, 255, 255);
            m_TabSelectedInnerBorderColor = Color.FromArgb(235, 243, 252);
            m_TabHighlightInnerBorderColor = Color.FromArgb(234, 237, 253);
            m_TabSelectedHotLightBorderColor = Color.FromArgb(255, 208, 48);
            m_TabSelectedHotLightInnerBorderColor = Color.FromArgb(255, 240, 187);
            m_TabForeColor = Color.FromArgb(74, 81, 90);
            m_ActiveTabForeColor = m_TabForeColor;
            m_TabBackgroundColor = Color.FromArgb(227, 230, 232);
            m_TabScrollArrowColor = Color.FromArgb(109, 114, 123);

            // colors for DockTabControl 
            m_DockTabForeColor = Color.FromArgb(74, 81, 90);
            m_DockTabBackgroundColor = Color.FromArgb(214, 215, 222);

            // colors for TextBoxExt 
            m_ActiveTextBoxBorderColor = Color.FromArgb(235, 137, 0);
            m_ActiveTextBoxBackColor = Color.FromArgb(255, 255, 255);
            m_InactiveTextBoxBorderColor = Color.FromArgb(164, 171, 178);
            m_InactiveTextBoxBackColor = Color.FromArgb(255, 255, 255);

            //TreeviewAdv
            m_SelectedNodeBackground = Color.FromArgb(196, 196, 196);
            m_TreeNodeArrowColor = Color.FromArgb(129, 129, 129);
            m_TreeviewBackColor = Color.FromArgb(231, 234, 239);
            m_TreeViewFontColor = Color.FromArgb(33, 39, 35);

            // form's colors
            m_ActiveFormBorderColor = Color.FromArgb(209, 211, 212);
            m_InactiveFormBorderColor = Color.FromArgb(214,217,220);

            m_FormTextColor = Color.FromArgb(83, 84, 89);

            m_ActiveTitleGradientBegin = Color.FromArgb(246,248,251);
            m_ActiveTitleGradientEnd = Color.FromArgb(209,210,211);
            m_InactiveTitleGradientBegin = Color.FromArgb(250,251,253);
            m_InactiveTitleGradientEnd = Color.FromArgb(227,228,229);

            m_SystemButtonSelectedGradientBegin = Color.FromArgb(245,246,247);
            m_SystemButtonSelectedGradientEnd = Color.FromArgb(232,235,239);
            m_SystemButtonPressedGradientBegin = Color.FromArgb(202, 204, 206);
            m_SystemButtonPressedGradientEnd = Color.FromArgb(172, 177, 183);

            m_SystemButtonBorderSelected = Color.FromArgb(191,192,196);
            m_SystemButtonBorderPressed = Color.FromArgb(142, 143, 145);
            m_FormBackground = Color.FromArgb(232,236,240);

            // upDown colors
            m_UpDownArrowStartColor = Color.FromArgb(96, 104, 112);
            m_UpDownArrowEndColor = Color.FromArgb(58, 62, 66);

            m_UpDownBorderNormalColor = Color.FromArgb(169, 177, 184);
            m_UpDownBackgroundNormalColor = Color.FromArgb(250, 250, 250);
            m_UpDownBackgroundNormalStartColor = Color.FromArgb(250, 250, 250);
            m_UpDownBackgroundNormalEndColor = Color.FromArgb(250, 250, 250);

            // colors for comboBoxAdv
            m_ComboBoxAdvNormalBackColor = SystemColors.Window;
            m_ComboBoxAdvHotBackColor = Color.FromArgb(250, 250, 250);
            m_ComboBoxAdvNormalBorderColor = Color.FromArgb(213, 214, 218);
            m_ComboBoxAdvHotBorderColor = Color.FromArgb(187, 192, 196);
            m_ComboBoxAdvPushedBorderColor = Color.FromArgb(157, 146, 102);
            m_ComboBoxAdvButtonUpperLineColor = Color.FromArgb(192, 185, 165);
            m_ComboBoxAdvArrowColor = Color.FromArgb(124, 124, 124);
            m_ComboBoxAdvLowerArrowLineColor = Color.FromArgb(255, 248, 203);
            m_ComboBoxAdvHotBackgroundButtonColor1 = Color.FromArgb(244, 225, 153);
            m_ComboBoxAdvHotBackgroundButtonColor2 = Color.FromArgb(245, 219, 124);
            m_ComboBoxAdvHotBackgroundButtonColor3 = Color.FromArgb(245, 210, 130);
            m_ComboBoxAdvHotBackgroundButtonColor4 = Color.FromArgb(246, 200, 138);
            m_ComboBoxAdvNormalBackgroundButtonColor1 = Color.FromArgb(226, 226, 226);
            m_ComboBoxAdvNormalBackgroundButtonColor2 = Color.FromArgb(220, 220, 220);
            m_ComboBoxAdvNormalBackgroundButtonColor3 = Color.FromArgb(209, 209, 209);
            m_ComboBoxAdvNormalBackgroundButtonColor4 = Color.FromArgb(196, 196, 196);
            m_ComboBoxAdvPushedBackgroundButtonColor1 = Color.FromArgb(250, 211, 115);
            m_ComboBoxAdvPushedBackgroundButtonColor2 = Color.FromArgb(253, 228, 124);
            m_ComboBoxAdvPushedBackgroundButtonColor3 = Color.FromArgb(254, 220, 130);
            m_ComboBoxAdvPushedBackgroundButtonColor4 = Color.FromArgb(255, 228, 138);

            // colors for CheckBoxAdv
            m_CheckBoxAdvNormalBackColor =  Color.FromArgb(248, 248, 248);
            m_CheckBoxAdvSelectedBackColor = Color.FromArgb(254, 248, 232);
            m_CheckBoxAdvPushedBackColor = Color.FromArgb(255, 244, 213);
            m_CheckBoxAdvNormalBorderColor = Color.FromArgb(155, 157, 160);
            m_CheckBoxAdvSelectedBorderColor = Color.FromArgb(155, 157, 160);
            m_CheckBoxAdvPushedBorderColor = Color.FromArgb(155, 157, 160);
            m_CheckBoxAdvNormalInternalBorderColor = Color.FromArgb(244, 244, 244);
            m_CheckBoxAdvSelectedInternalBorderColor = Color.FromArgb(244, 244, 244);
            m_CheckBoxAdvPushedInternalBorderColor = Color.FromArgb(229, 236, 247);
            m_CheckBoxAdvNormalInternalRectangleBorderColor = Color.FromArgb(162, 172, 185);
            m_CheckBoxAdvSelectedInternalRectangleBorderColor = Color.FromArgb(250, 213, 122);
            m_CheckBoxAdvPushedInternalRectangleBorderColor = Color.FromArgb(242, 137, 38);
            m_CheckBoxAdvNormalInternalRectangleColor = Color.FromArgb(202, 207, 213);
            m_CheckBoxAdvSelectedInternalRectangleColor = Color.FromArgb(252, 231, 175);
            m_CheckBoxAdvPushedInternalRectangleColor = Color.FromArgb(255, 208, 103);
            m_CheckBoxAdvNormalTickColor = Color.FromArgb(74, 107, 150);
            m_CheckBoxAdvSelectedTickColor = Color.FromArgb(78, 108, 143);
            m_CheckBoxAdvPushedTickColor = Color.FromArgb(79, 108, 139);
            m_CheckBoxAdvDisabledTickColor = Color.FromArgb(176, 190, 208);
            m_CheckBoxAdvIndeterminateRectangleColor = Color.FromArgb(158, 168, 178);
            m_CheckBoxAdvDisabledBackColor = Color.FromArgb(238, 240, 242);
            m_CheckBoxAdvDisabledBorderColor = Color.FromArgb(174, 177, 181);
            m_CheckBoxAdvDisabledInternalBorderColor = Color.FromArgb(224, 226, 229);

            // colors for RadioButtonAdv
            m_RadioButtonAdvNormalBackColor = SystemColors.Window;
            m_RadioButtonAdvNormalBorderColor = Color.FromArgb(155, 157, 160);
            m_RadioButtonAdvNormalInternalBorderColor = Color.FromArgb(162, 172, 185);
            m_RadioButtonAdvSelectedBackColor = Color.FromArgb(244, 244, 244);
            m_RadioButtonAdvSelectedBorderColor = Color.FromArgb(165, 167, 170);
            m_RadioButtonAdvSelectedInternalBorderColor = Color.FromArgb(203, 209, 216);
            m_RadioButtonAdvPushedBackColor = Color.FromArgb(229, 236, 247);
            m_RadioButtonAdvPushedBorderColor = Color.FromArgb(165, 167, 160);
            m_RadioButtonAdvPushedInternalBorderColor = Color.FromArgb(203, 209, 216);
            m_RadioButtonAdvCheckMarkBorderColor =  Color.FromArgb(160, 160, 160);
            m_RadioButtonAdvCheckMarkNormalBottomColor = Color.FromArgb(128, 128, 128);
            m_RadioButtonAdvCheckMarkSelectedBottomColor = Color.FromArgb(155, 156, 167);
            m_RadioButtonAdvCheckMarkPushedBottomColor = Color.FromArgb(107, 107, 107);

            // colors for TabBarSplitterControl
            m_TabBarSplitterBackColor = Color.FromArgb(239, 243, 255);
            m_TabBarSplitterBorderColor = Color.FromArgb(107, 109, 107);
            m_TabBarSplitterTextColor = Color.FromArgb(49, 52, 49);
            m_TabBarSplitterTabStartColor = Color.FromArgb(222, 223, 231);
            m_TabBarSplitterTabEndColor = Color.FromArgb(181, 190, 206);
            m_TabBarSplitterTabBarStartColor = Color.FromArgb(123, 121, 123);
            m_TabBarSplitterTabBarEndColor = Color.FromArgb(74, 73, 74);
            m_TabBarSplitterButtonHoveredStartColor = Color.White;
            m_TabBarSplitterButtonHoveredEndColor = Color.FromArgb(222, 231, 247);
            m_TabBarSplitterButtonPushedStartColor = Color.FromArgb(206, 223, 255);
            m_TabBarSplitterButtonPushedEndColor = Color.FromArgb(148, 182, 239);
            m_TabBarSplitterSizeGripperColor = Color.FromArgb(183, 186, 194);
            m_TabBarSplitterSizeGripperLightColor = Color.FromArgb(205, 209, 213);
            m_TabBarSplitterSizeGripperDarkColor = Color.FromArgb(114, 118, 122);

            //colors for XPTaskBar
            m_XPTaskBarBorderColor = Color.FromArgb(161, 169, 179);
            m_XPTaskBarBoxBackColor = Color.FromArgb(220, 226, 231);
            m_XPTaskBarBoxForeColor = Color.FromArgb(0, 0, 0);
            m_XPTaskBarBoxHeaderLowerLineColor = Color.FromArgb(161, 169, 169);
            m_XPTaskBarBoxHeaderUpperLineColor = Color.FromArgb(179, 186, 195);
            m_XPTaskBarBoxArrowColor = Color.FromArgb(66, 69, 71);
            m_XPTaskBarBoxActiveHighlightedItemColor = Color.FromArgb(255, 232, 156);
            m_XPTaskBarBoxInactiveHighlightedItemColor = Color.FromArgb(229, 229, 229);

            // colors for ButtonAdv
            m_ButtonDefaultTopColor = Color.FromArgb(253, 253, 253);// Color.FromArgb(223, 227, 231);
            m_ButtonDefaultBottomColor = Color.FromArgb(208, 212, 221);
            m_ButtonDefaultBorderColor = Color.FromArgb(182, 185, 190);
            m_ButtonDefaultInternalBorderColor = Color.FromArgb(128, Color.FromArgb(250, 251, 255));
            m_ButtonPressedInternalBorderColor = Color.FromArgb(70, Color.FromArgb(176, 132, 92));
            m_ButtonSelectedInternalBorderColor = Color.FromArgb(128, Color.FromArgb(250, 251, 255));

            // ColorUIAdv colors
            m_ColorUIAdvTextColor = Color.FromArgb(79, 83, 87);

            //StatusBarExt
            this.m_StatusBarExtTopGradient = Color.FromArgb(232, 236, 240);
            this.m_StatusBarExtBottomGradient = Color.FromArgb(232, 236, 240);
            this.m_StatusBarExtFillColor = Color.FromArgb(192, 198, 207);
        }
        #endregion
    }

    /// <summary>
    /// Provides colors for black colorscheme of the Office2010 visual style.
    /// </summary>
    public class Office2010BlackColors : Office2010Colors
    {
        #region Class Overrides
        /// <summary>
        /// Initialize colors for Black colorscheme of the Office2010 visual style.
        /// </summary>
        protected override void InitializeColors()
        {
            base.InitializeColors();

            // colors for menu
            m_MenuTextBoxBorderColor = Color.FromArgb(137, 137, 137);
            m_MenuTextBoxBackColor = Color.FromArgb(232, 232, 232);
            m_MenuComboButtonHighlightLightColor = Color.FromArgb(239, 242, 246);
            m_MenuComboButtonHighlightDarkColor = Color.FromArgb(218, 224, 231);
            m_MenuComboButtonArrowColor = Color.FromArgb(124, 124, 124);
            m_MenuItemArrowLightColor = Color.FromArgb(78, 78, 78);
            m_MenuItemArrowDarkColor = Color.FromArgb(40, 40, 40);

            // colors for CommandBar
            m_CommandBarDarkColor = Color.FromArgb(148, 156, 166);
            m_CommandBarLightColor = Color.FromArgb(205, 208, 213);
            m_CommandBarLightColor = Color.FromArgb(205, 208, 213);

            // colors for DropDown button of the CommandBar
            m_DockBarBackColor = Color.FromArgb(193, 193, 193);
            m_DropDownDarkColor = Color.FromArgb(139, 139, 139);
            m_DropDownLightColor = Color.FromArgb(207, 207, 207);
            m_DropDownHighlightLightColor = Color.FromArgb(255, 248, 211);
            m_DropDownHighlightDarkColor = Color.FromArgb(255, 193, 118);
            m_DropDownPressedLightColor = Color.FromArgb(254, 149, 82);
            m_DropDownPressedDarkColor = Color.FromArgb(255, 217, 149);

            // colors for floating CommandBar
            m_FloatHighlightButtonColor = Color.FromArgb(255, 231, 162);
            m_FloatHighlightButtonBorderColor = Color.FromArgb(255, 189, 105);
            m_FloatPressButtonColor = Color.FromArgb(221, 224, 227);
            m_FloatPressButtonBorderColor = Color.FromArgb(145, 153, 164);
            m_FloatPressCloseButtonBorderColor = Color.FromArgb(251, 140, 60);
            m_FloatPressCloseButtonColor = Color.FromArgb(251, 140, 60);
            m_FloatCommandBarLightColor = Color.White;
            m_FloatCommandBarDarkColor = Color.White;
            m_FloatLightBorderColor = Color.White;
            m_FloatBackgroundColor = Color.FromArgb(128, 128, 128);
            m_FloatBorderColor = Color.FromArgb(113, 113, 113);
            m_FloatCaptionColor = Color.White;

            // color for separator in a CommandBar
            m_BarItemSeparatorColor = Color.FromArgb(145, 153, 164);

            // colors for ComboButton of the ComboBoxBarItem in CommandBar
            m_ComboButtonLightColor = Color.FromArgb(225, 225, 225);
            m_ComboButtonDarkColor = Color.FromArgb(203, 203, 203);
            m_ComboButtonBorder = Color.FromArgb(145, 145, 145);

            // colors for tab group
            m_TabItemBorderColor = Color.FromArgb(66, 65, 66);
            m_TabItemInnerBorderColor = Color.FromArgb(189, 190, 189);
            m_TabItemOuterBorderColor = Color.FromArgb(74, 73, 74);
            m_TabItemTextColor = Color.FromArgb(255, 255, 255);
            m_TabItemActiveBottomColor = Color.FromArgb(189, 154, 57);
            m_TabItemTopGradientColor = Color.FromArgb(107, 105, 99);
            m_TabItemInActiveBottomColor = Color.FromArgb(148, 150, 148);
            m_TabItemMiddleLineColor = Color.FromArgb(101, 106, 102);
            m_TabPanelColor = Color.FromArgb(116, 116, 116);
            m_TabPanelBorderColor = Color.FromArgb(57, 56, 57);
            m_TabPanelBackColor = Color.FromArgb(116, 116, 116);

            // colors for GroupBar
            m_GroupBarBorderColor = Color.FromArgb(45, 45, 45);
            m_GroupBarHeaderColorLight = Color.FromArgb(121, 121, 121); 
            m_GroupBarHeaderColorDark = Color.FromArgb(92, 92, 92); 
            m_GroupBarItemTextColor = Color.FromArgb(49, 60, 66);
            m_GroupBarBackColor = Color.FromArgb(138, 138, 138);
            m_GroupBarHeaderTextColor = Color.White;
            m_GroupBarItemColorLight = Color.FromArgb(91, 91, 91);
            m_GroupBarItemColorDark = Color.FromArgb(125, 125, 125);
            m_GroupBarSplitterColorDark = Color.FromArgb(89, 89, 89); 
            m_GroupBarSplitterColorLight = Color.FromArgb(150, 150, 150); 
            m_GroupBarClientAreaBackground = Color.FromArgb(138, 138, 138);
            //Arrow in groupbar
            m_GroupBarHighlightColorLight = Color.FromArgb(180, 180, 180);
            m_GroupBarHighlightColorDark = Color.FromArgb(100, 100, 100); 

            m_GroupBarSelectedColorLight = Color.FromArgb(150, 150, 150);
            m_GroupBarSelectedColorDark =  Color.FromArgb(203, 203, 203);
            m_GroupBarSelectedTopColorLight = Color.FromArgb(89, 89, 89);
            m_GroupBarSelectedTopColorDark = Color.FromArgb(150, 150, 150);
            m_GroupBarSelectedHighlightColorLight = Color.FromArgb(180, 180, 180);
            m_GroupBarSelectedHighlightColorDark = Color.FromArgb(100, 100, 100);

            // colors for DataTimePicker
            m_DataTimePickerBorderColor = Color.FromArgb(137, 137, 137);

            m_DataTimePickerDropDownArrowColor = Color.FromArgb(79, 86, 96);
            m_DataTimePickerDropDownLightColor = Color.FromArgb(146, 146, 146);
            m_DataTimePickerDropDownDarkColor = Color.FromArgb(83, 83, 83);

            m_DataTimePickerCheckBoxBorderNormalColor = Color.FromArgb(132, 132, 132);
            m_DataTimePickerCheckBoxBorderPushedColor = Color.FromArgb(74, 81, 90);

            m_DataTimePickerHighLightedForeColor = Color.Black;

           
            //colors for MonthCalendarAdv
            m_MonthCalendarHeaderStartColor = Color.FromArgb(163, 171, 177);
            m_MonthCalendarHeaderEndColor = Color.FromArgb(163, 171, 177);
            m_MonthCalendarForeColor = Color.White;
            m_MonthCalendarBackgroundColor = Color.FromArgb(163, 171, 177);

            // colors for XPTaskPane
            m_XPTaskPaneInternalBorderColor = Color.FromArgb(246, 243, 248);
            m_XPTaskPaneBorderColor = Color.FromArgb(170, 170, 170);
            m_XPTaskPageBackColor = Color.FromArgb(246, 243, 248);

            // colors for TabControlAdv
            m_TabDefaultBorderColor = Color.FromArgb(94, 94, 94);
            m_TabHotLightBottomBorderLineColor = Color.Green;//Color.FromArgb(195, 164, 54);
            m_TabHotLightGradientTopBeginColor = Color.Green;//Color.FromArgb(151, 151, 151);
            m_TabHotLightGradientTopEndColor = Color.Green;//Color.FromArgb(122, 109, 97);
            m_TabHotLightGradientBottomBeginColor = Color.Green;//Color.FromArgb(126, 118, 74);
            m_TabHotLightGradientBottomEndColor = Color.Green;//Color.FromArgb(233, 190, 38);
            m_TabHotLightGradientCircleColor = Color.Yellow;//;//Color.FromArgb(122, 109, 97);
            m_TabSelectedGradientTopColor = Color.FromArgb(166, 166, 166);
            m_TabSelectedGradientBottomColor = Color.FromArgb(200, 200, 200);
            m_TabSelectedInnerBorderColor = Color.FromArgb(94, 94, 94);
            m_TabHighlightInnerBorderColor = Color.Red;// Color.FromArgb(234, 237, 253);
            m_TabSelectedHotLightBorderColor = Color.Red;//Color.FromArgb(255, 208, 48);
            m_TabSelectedHotLightInnerBorderColor = Color.Red;//Color.FromArgb(255, 240, 187);
            m_TabForeColor = Color.White;
            m_ActiveTabForeColor = Color.Black;
            m_TabBackgroundColor = Color.FromArgb(116, 116, 116);
            m_TabScrollArrowColor = Color.WhiteSmoke;

            // colors for DockTabControl 
            m_DockTabForeColor = Color.FromArgb(176, 178, 176);
            m_DockTabBackgroundColor = Color.FromArgb(82, 81, 82);

            // colors for TextBoxExt 
            m_ActiveTextBoxBorderColor = Color.FromArgb(235, 137, 0);
            m_ActiveTextBoxBackColor = Color.FromArgb(255, 255, 255);
            m_InactiveTextBoxBorderColor = Color.FromArgb(145, 145, 145);
            m_InactiveTextBoxBackColor = Color.FromArgb(255, 255, 255);

            //TreeviewAdv
            m_SelectedNodeBackground = Color.FromArgb(59, 59, 59);            
            m_TreeNodeArrowColor = Color.FromArgb(255, 255, 255);
            m_TreeviewBackColor = Color.FromArgb(149, 151, 153);
            m_TreeViewFontColor = Color.FromArgb(255, 255, 255);


            // form's colors
            m_ActiveFormBorderColor = Color.FromArgb(106,106,106);
            m_InactiveFormBorderColor = Color.FromArgb(166,166,166);

            m_FormTextColor = Color.White;

            m_ActiveTitleGradientBegin = Color.FromArgb(125,125,125);
            m_ActiveTitleGradientEnd = Color.FromArgb(96,96,96);
            m_InactiveTitleGradientBegin = Color.FromArgb(177,177,177);
            m_InactiveTitleGradientEnd = Color.FromArgb(160,160,160);

            m_SystemButtonSelectedGradientBegin = Color.FromArgb(221,217,221);
            m_SystemButtonSelectedGradientEnd = Color.FromArgb(111,120,130);
            m_SystemButtonPressedGradientBegin = Color.FromArgb(133, 138, 140);
            m_SystemButtonPressedGradientEnd = Color.FromArgb(90, 96, 102);

            m_SystemButtonBorderSelected = Color.FromArgb(178,181,183);
            m_SystemButtonBorderPressed = Color.FromArgb(82, 85, 86);
            m_FormBackground = Color.FromArgb(137,137,137);

            // upDown colors
            m_UpDownArrowStartColor = Color.FromArgb(87, 87, 87);
            m_UpDownArrowEndColor = Color.FromArgb(52, 52, 52);

            m_UpDownBorderNormalColor = Color.FromArgb(145, 145, 145);
            m_UpDownBackgroundNormalColor = Color.FromArgb(232, 232, 232);
            m_UpDownBackgroundNormalStartColor = Color.FromArgb(218, 224, 231);
            m_UpDownBackgroundNormalEndColor = Color.FromArgb(238, 242, 246);

            // colors for comboBoxAdv
            m_ComboBoxAdvNormalBackColor = SystemColors.Window;
            m_ComboBoxAdvHotBackColor = Color.FromArgb(232, 232, 232);
            m_ComboBoxAdvNormalBorderColor =  Color.FromArgb(160, 160, 160);
            m_ComboBoxAdvHotBorderColor = Color.FromArgb(235, 201, 70);
            m_ComboBoxAdvPushedBorderColor = Color.FromArgb(194, 130, 51);
            m_ComboBoxAdvButtonUpperLineColor = Color.FromArgb(192, 185, 165);
            m_ComboBoxAdvArrowColor = Color.FromArgb(124, 124, 124);
            m_ComboBoxAdvLowerArrowLineColor = Color.FromArgb(255, 248, 203);
            m_ComboBoxAdvHotBackgroundButtonColor1 = Color.FromArgb(244, 225, 153);
            m_ComboBoxAdvHotBackgroundButtonColor2 = Color.FromArgb(245, 219, 124);
            m_ComboBoxAdvHotBackgroundButtonColor3 = Color.FromArgb(245, 210, 130);
            m_ComboBoxAdvHotBackgroundButtonColor4 = Color.FromArgb(246, 200, 138);
            m_ComboBoxAdvNormalBackgroundButtonColor1 = Color.FromArgb(226, 226, 226);
            m_ComboBoxAdvNormalBackgroundButtonColor2 = Color.FromArgb(220, 220, 220);
            m_ComboBoxAdvNormalBackgroundButtonColor3 = Color.FromArgb(209, 209, 209);
            m_ComboBoxAdvNormalBackgroundButtonColor4 = Color.FromArgb(196, 196, 196);
            m_ComboBoxAdvPushedBackgroundButtonColor1 = Color.FromArgb(250, 211, 115);
            m_ComboBoxAdvPushedBackgroundButtonColor2 = Color.FromArgb(253, 228, 124);
            m_ComboBoxAdvPushedBackgroundButtonColor3 = Color.FromArgb(254, 220, 130);
            m_ComboBoxAdvPushedBackgroundButtonColor4 = Color.FromArgb(255, 228, 138);

            // colors for CheckBoxAdv
            m_CheckBoxAdvNormalBackColor = Color.FromArgb(248, 248, 248);
            m_CheckBoxAdvSelectedBackColor = Color.FromArgb(254, 248, 232);
            m_CheckBoxAdvPushedBackColor = Color.FromArgb(255, 244, 213);
            m_CheckBoxAdvNormalBorderColor = Color.FromArgb(132, 132, 132);
            m_CheckBoxAdvSelectedBorderColor = Color.FromArgb(132, 132, 132);
            m_CheckBoxAdvPushedBorderColor = Color.FromArgb(132, 132, 132);
            m_CheckBoxAdvNormalInternalBorderColor = Color.FromArgb(244, 244, 244);
            m_CheckBoxAdvSelectedInternalBorderColor = Color.FromArgb(244, 244, 244);
            m_CheckBoxAdvPushedInternalBorderColor = Color.FromArgb(229, 236, 247);
            m_CheckBoxAdvNormalInternalRectangleBorderColor = Color.FromArgb(162, 172, 185);
            m_CheckBoxAdvSelectedInternalRectangleBorderColor = Color.FromArgb(250, 213, 122);
            m_CheckBoxAdvPushedInternalRectangleBorderColor = Color.FromArgb(242, 137, 38);
            m_CheckBoxAdvNormalInternalRectangleColor = Color.FromArgb(202, 207, 213);
            m_CheckBoxAdvSelectedInternalRectangleColor = Color.FromArgb(252, 231, 175);
            m_CheckBoxAdvPushedInternalRectangleColor = Color.FromArgb(255, 208, 103);
            m_CheckBoxAdvNormalTickColor = Color.FromArgb(74, 107, 150);
            m_CheckBoxAdvSelectedTickColor = Color.FromArgb(78, 108, 143);
            m_CheckBoxAdvPushedTickColor = Color.FromArgb(79, 105, 130);
            m_CheckBoxAdvDisabledTickColor = Color.FromArgb(176, 190, 208);
            m_CheckBoxAdvIndeterminateRectangleColor = Color.FromArgb(158, 168, 178);
            m_CheckBoxAdvDisabledBackColor = Color.FromArgb(238, 240, 242);
            m_CheckBoxAdvDisabledBorderColor = Color.FromArgb(174, 177, 181);
            m_CheckBoxAdvDisabledInternalBorderColor = Color.FromArgb(224, 226, 229);

            // colors for RadioButtonAdv
            m_RadioButtonAdvNormalBackColor = SystemColors.Window;
            m_RadioButtonAdvNormalBorderColor = Color.FromArgb(132, 132, 132);
            m_RadioButtonAdvNormalInternalBorderColor = Color.FromArgb(162, 172, 185);
            m_RadioButtonAdvSelectedBackColor = Color.FromArgb(244, 244, 244);
            m_RadioButtonAdvSelectedBorderColor = Color.FromArgb(132, 132, 132);
            m_RadioButtonAdvSelectedInternalBorderColor = Color.FromArgb(203, 209, 216);
            m_RadioButtonAdvPushedBackColor = Color.FromArgb(244, 244, 244);
            m_RadioButtonAdvPushedBorderColor = Color.FromArgb(132, 132, 132);
            m_RadioButtonAdvPushedInternalBorderColor = Color.FromArgb(203, 209, 216);
            m_RadioButtonAdvCheckMarkBorderColor = Color.FromArgb(125, 125, 125);
            m_RadioButtonAdvCheckMarkNormalBottomColor = Color.FromArgb(118, 118, 118);
            m_RadioButtonAdvCheckMarkSelectedBottomColor = Color.FromArgb(155, 156, 157);
            m_RadioButtonAdvCheckMarkPushedBottomColor = Color.FromArgb(95, 95, 95);

            // colors for TabBarSplitterControl
            m_TabBarSplitterBackColor = Color.FromArgb(255, 251, 255);
            m_TabBarSplitterBorderColor = Color.FromArgb(107, 109, 107);
            m_TabBarSplitterTextColor = Color.FromArgb(49, 52, 49);
            m_TabBarSplitterTabStartColor = Color.FromArgb(222, 223, 231);
            m_TabBarSplitterTabEndColor = Color.FromArgb(181, 190, 206);
            m_TabBarSplitterTabBarStartColor = Color.FromArgb(123, 121, 123);
            m_TabBarSplitterTabBarEndColor = Color.FromArgb(74, 73, 74);
            m_TabBarSplitterButtonHoveredStartColor = Color.White;
            m_TabBarSplitterButtonHoveredEndColor = Color.FromArgb(222, 231, 247);
            m_TabBarSplitterButtonPushedStartColor = Color.FromArgb(206, 223, 255);
            m_TabBarSplitterButtonPushedEndColor = Color.FromArgb(148, 182, 239);
            m_TabBarSplitterSizeGripperColor = Color.FromArgb(112, 112, 112);
            m_TabBarSplitterSizeGripperLightColor = Color.FromArgb(204, 204, 204);
            m_TabBarSplitterSizeGripperDarkColor = Color.FromArgb(37, 37, 37);

            //colors for XPTaskBar
            m_XPTaskBarBorderColor = Color.FromArgb(46, 46, 46);
            m_XPTaskBarBoxBackColor = Color.FromArgb(83, 83, 83);
            m_XPTaskBarBoxForeColor = Color.White;
            m_XPTaskBarBoxHeaderLowerLineColor = Color.FromArgb(46, 46, 46);
            m_XPTaskBarBoxHeaderUpperLineColor = Color.FromArgb(65, 65, 65);
            m_XPTaskBarBoxArrowColor = Color.White;
            m_XPTaskBarBoxActiveHighlightedItemColor = Color.FromArgb(255, 232, 156);
            m_XPTaskBarBoxInactiveHighlightedItemColor = Color.FromArgb(229, 229, 229);

            // colors for ButtonAdv
            m_ButtonDefaultTopColor = Color.FromArgb(146, 146, 146);
            m_ButtonDefaultBottomColor = Color.FromArgb(83, 83, 83);
            m_ButtonDefaultBorderColor = Color.FromArgb(169, 169, 169);
            m_ButtonDefaultInternalBorderColor = Color.FromArgb(128, Color.FromArgb(250, 251, 255));
            m_ButtonPressedInternalBorderColor = Color.FromArgb(70, Color.FromArgb(176, 132, 92));
            m_ButtonSelectedInternalBorderColor = Color.FromArgb(128, Color.FromArgb(250, 251, 255));

            // colors for ColorUIAdv
            m_ColorUIAdvTextColor = Color.FromArgb(70, 70, 70);
        }
        #endregion
    }

    /// <summary>
    /// Provides colors for Office2010 visual style.
    /// </summary>
    public class Office2010Colors :
        ICloneable
    {
        #region Class Static Members
        /// <summary>
        /// Colors for blue colorscheme of the Office2010 visual style.
        /// </summary>
        private static WeakReference s_blueColors = null;
        /// <summary>
        /// Colors for silver colorscheme of the Office2010 visual style.
        /// </summary>
        private static WeakReference s_silverColors = null;
        /// <summary>
        /// Colors for black colorscheme of the Office2010 visual style.
        /// </summary>
        private static WeakReference s_blackColors = null;
        /// <summary>
        /// 
        /// </summary>
        private static WeakReference s_managedColors = null;
        /// <summary>
        /// Default colorscheme for office2010 visual style.
        /// </summary>
        private static Office2010Theme s_defaultTheme = Office2010Theme.Blue;
        /// <summary>
        /// Base color for managed scheme.
        /// </summary>
        private static Color s_managedBaseColor = Color.Empty;
        #endregion

        #region Class Static Properties
        /// <summary>
        /// Gets or sets default colors for Office2010 visual style.
        /// </summary>
        public static Office2010Colors Default
        {
            get
            {
                return GetColorTable(s_defaultTheme);
            }
        }
        /// <summary>
        /// Gets or sets default colorscheme for office2010 visual style.
        /// </summary>
        public static Office2010Theme DefaultTheme
        {
            get
            {
                return s_defaultTheme;
            }
            set
            {
                if (value != s_defaultTheme)
                {
                    s_defaultTheme = value;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal static Office2010Colors ManagedColors
        {
            get
            {
                if (s_managedColors.IsAlive)
                    return s_managedColors.Target as Office2010Colors;
                else
                {
                    Office2010Colors colors = new Office2010BlueColors();
                    s_managedColors = new WeakReference(colors);
                    return colors;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal static Color ManagedBaseColor
        {
            get
            {
                return s_managedBaseColor;
            }
        }

        #endregion

        #region Class static events

        /// <summary>
        /// Arguments class for <see cref="Office2010Colors.ManagedColorsApplied"/> event.
        /// </summary>
        public class ManagedColorsAppliedEventArgs :
            EventArgs
        {
            /// <summary>
            /// Initializes <see cref="ManagedColorsAppliedEventArgs"/> instance.
            /// </summary>
            /// <param name="form">Container form.</param>
            /// <param name="baseColor">Base color for the managed theme.</param>
            public ManagedColorsAppliedEventArgs(Form form, Color baseColor)
            {
                this.Form = form;
                this.BaseColor = baseColor;
            }

            /// <summary>
            /// Container form.
            /// </summary>
            public Form Form;

            /// <summary>
            /// Base color for the managed theme.
            /// </summary>
            public Color BaseColor;
        }

        public delegate void ManagedColorsAppliedEventHandler(ManagedColorsAppliedEventArgs args);

        public static event ManagedColorsAppliedEventHandler ManagedColorsApplied;

        #endregion

        #region Class Static Public Methods
        /// <summary>
        /// Gets color table for Office2010 visual style.
        /// </summary>
        public static Office2010Colors GetColorTable(Office2010Theme theme)
        {
            Office2010Colors colorTable = null;

            switch (theme)
            {
                case Office2010Theme.Black:
                    {
                        if (s_blackColors.IsAlive)
                            colorTable = s_blackColors.Target as Office2010Colors;
                        else
                        {
                            colorTable = new Office2010BlackColors();
                            s_blackColors = new WeakReference(colorTable);
                        }
                        break;
                    }
                case Office2010Theme.Silver:
                    {
                        if (s_silverColors.IsAlive)
                            colorTable = s_silverColors.Target as Office2010Colors;
                        else
                        {
                            colorTable = new Office2010SilverColors();
                            s_silverColors = new WeakReference(colorTable);
                        }
                        break;
                    }
                case Office2010Theme.Blue:
                    {
                        if (s_blueColors.IsAlive)
                            colorTable = s_blueColors.Target as Office2010Colors;
                        else
                        {
                            colorTable = new Office2010BlueColors();
                            s_blueColors = new WeakReference(colorTable);
                        }
                        break;
                    }
                case Office2010Theme.Managed:
                    {
                        colorTable = ManagedColors;
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("Unknown theme.");
                    }
            }
            return colorTable;
        }

        /// <summary>
        /// Applies colors for managed scheme.
        /// </summary>
        /// <param name="form">Container form.</param>
        /// <param name="baseColor">Base color for the managed theme.</param>
        public static void ApplyManagedColors(Form form, Color baseColor)
        {
            s_managedBaseColor = baseColor;

            ManagedColors.UpdateColors(baseColor);
            OnManagedColorApplied(form, baseColor);

            if (form.IsHandleCreated)
            {
                NativeMethods.RedrawWindow(form.Handle, IntPtr.Zero, IntPtr.Zero, NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE);

                form.Invalidate(true);
                form.Update();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <param name="scheme"></param>
        public static void ApplyManagedScheme(Form form, Office2010Theme scheme)
        {
            s_managedBaseColor = Color.Empty;

            ManagedColors.UpdateScheme(scheme);

            if (form.IsHandleCreated)
            {
                NativeMethods.RedrawWindow(form.Handle, IntPtr.Zero, IntPtr.Zero, NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE);

                form.Invalidate(true);
            }
        }

        protected static void OnManagedColorApplied(Form form, Color baseColor)
        {
            if (Office2010Colors.ManagedColorsApplied != null)
            {
                Office2010Colors.ManagedColorsApplied(new ManagedColorsAppliedEventArgs(form, baseColor));
            }
        }

        #endregion

        #region Class Initialize/Finalize Methods
        static Office2010Colors()
        {
            s_managedColors = new WeakReference(new Office2010BlueColors());
            s_blueColors = new WeakReference(new Office2010BlueColors());
            s_silverColors = new WeakReference(new Office2010SilverColors());
            s_blackColors = new WeakReference(new Office2010BlackColors());
        }

        protected Office2010Colors()
        {
            // Initialize colors
            InitializeColors();
        }
        #endregion

        #region Class Members

        // Tab item colors
        protected Color m_TabItemBorderColor = Color.Empty;
        protected Color m_TabItemInnerBorderColor = Color.Empty;
        protected Color m_TabItemOuterBorderColor = Color.Empty;
        protected Color m_TabItemTextColor = Color.Empty;
        protected Color m_TabItemActiveBottomColor = Color.Empty;
        protected Color m_TabItemTopGradientColor = Color.Empty;
        protected Color m_TabItemInActiveBottomColor = Color.Empty;
        protected Color m_TabItemMiddleLineColor = Color.Empty;
        protected Color m_TabPanelColor = Color.Empty;
        protected Color m_TabPanelBorderColor = Color.Empty;
        protected Color m_TabPanelBackColor = Color.Empty;

        //DataTimePickerAdv colors
        protected Color m_DataTimePickerBorderColor = Color.Empty;
        protected Color m_DataTimePickerHighLightedBorderColor = Color.Empty;
        protected Color m_DataTimePickerSelectedBorderColor = Color.Empty;

        protected Color m_DataTimePickerDropDownArrowColor = Color.Empty;
        protected Color m_DataTimePickerDropDownLightColor = Color.Empty;
        protected Color m_DataTimePickerDropDownDarkColor = Color.Empty;
        protected Color m_DataTimePickerDropDownHighLightLightColor = Color.Empty;
        protected Color m_DataTimePickerDropDownHighLightDarkColor = Color.Empty;
        protected Color m_DataTimePickerDropDownSelectedLightColor = Color.Empty;
        protected Color m_DataTimePickerDropDownSelectedDarkColor = Color.Empty;

        protected Color m_DataTimePickerCheckBoxNormalColor = Color.Empty;
        protected Color m_DataTimePickerCheckBoxSelectedColor = Color.Empty;
        protected Color m_DataTimePickerCheckBoxBorderPushedColor = Color.Empty;
        protected Color m_DataTimePickerCheckBoxBorderNormalColor = Color.Empty;
        protected Color m_DataTimePickerCheckBoxInnerRectBorderNormalColor = Color.Empty;
        protected Color m_DataTimePickerCheckBoxInnerRectBorderSelectedColor = Color.Empty;
        protected Color m_DataTimePickerCheckBoxInnerRectBorderPushedColor = Color.Empty;
        protected Color m_DataTimePickerCheckBoxInnerRectFillNormalColor = Color.Empty;
        protected Color m_DataTimePickerCheckBoxInnerRectFillSelectedColor = Color.Empty;
        protected Color m_DataTimePickerCheckBoxInnerRectFillPushedColor = Color.Empty;

        protected Color m_DataTimePickerHighLightedForeColor = Color.Empty;

        //MonthCalendarAdv colors
        protected Color m_MonthCalendarHeaderStartColor = Color.Empty;
        protected Color m_MonthCalendarHeaderEndColor = Color.Empty;
        protected Color m_MonthCalendarForeColor = Color.Empty;
        protected Color m_MonthCalendarBackgroundColor = Color.Empty;

        // GroupBar colors
        protected Color m_GroupBarBorderColor = Color.Empty;
        protected Color m_GroupBarItemColorDark = Color.Empty;
        protected Color m_GroupBarItemColorLight = Color.Empty;
        protected Color m_GroupBarHighlightColorLight = Color.Empty;
        protected Color m_GroupBarHighlightColorDark = Color.Empty;
        protected Color m_GroupBarSelectedColorLight = Color.Empty;
        protected Color m_GroupBarSelectedColorDark = Color.Empty;
        protected Color m_GroupBarSelectedTopColorLight = Color.Empty;
        protected Color m_GroupBarSelectedTopColorDark = Color.Empty;
        protected Color m_GroupBarSelectedHighlightColorLight = Color.Empty;
        protected Color m_GroupBarSelectedHighlightColorDark = Color.Empty;
        protected Color m_GroupBarHeaderColorLight = Color.Empty;
        protected Color m_GroupBarHeaderColorDark = Color.Empty;
        protected Color m_GroupBarItemTextColor = Color.Empty;
        protected Color m_GroupBarBackColor = Color.Empty;        
        protected Color m_GroupBarHeaderTextColor = Color.Empty;
        protected Color m_GroupBarSplitterColorDark = Color.Empty;
        protected Color m_GroupBarSplitterColorLight = Color.Empty;
        protected Color m_GroupBarClientAreaBackground = Color.Empty;

        //XpTaskPane colors
        protected Color m_XPTaskPaneInternalBorderColor = Color.Empty;
        protected Color m_XPTaskPaneBorderColor = Color.Empty;
        protected Color m_XPTaskPageBackColor = Color.Empty;

        // menu colors
        protected Color m_MenuBorderColor = Color.Empty;
        protected Color m_MenuSeparatorColor = Color.Empty;
        protected Color m_MenuColumnColor = Color.Empty;
        protected Color m_MenuColumnSeparatorColor = Color.Empty;
        protected Color m_MenuBackground = Color.Empty;
        protected Color m_MenuItemBorderColor = Color.Empty;
        protected Color m_MenuItemDarkColor = Color.Empty;
        protected Color m_MenuItemLightColor = Color.Empty;
        protected Color m_MenuItemArrowLightColor = Color.Empty;
        protected Color m_MenuItemArrowDarkColor = Color.Empty;
        protected Color m_MenuCheckedColor = Color.Empty;
        protected Color m_MenuCheckedFillColor = Color.Empty;
        protected Color m_MenuCheckedBorderColor = Color.Empty;
        protected Color m_MenuTextBoxBorderColor = Color.Empty;
        protected Color m_MenuTextBoxBackColor = Color.Empty;
        protected Color m_MenuComboButtonPushed1Color = Color.Empty;
        protected Color m_MenuComboButtonPushed2Color = Color.Empty;
        protected Color m_MenuComboButtonPushed3Color = Color.Empty;
        protected Color m_MenuComboButtonPushed4Color = Color.Empty;
        protected Color m_MenuComboButtonHighlightLightColor = Color.Empty;
        protected Color m_MenuComboButtonHighlightDarkColor = Color.Empty;
        protected Color m_MenuComboButtonArrowColor = Color.Empty;

        // CommandBar colors
        protected Color m_DropDownLightColor = Color.Empty;
        protected Color m_DropDownDarkColor = Color.Empty;
        protected Color m_CommandBarDarkColor = Color.Empty;
        protected Color m_CommandBarLightColor = Color.Empty;
        protected Color m_CommandBarBorderColor = Color.Empty;
        protected Color m_DockBarBackColor = Color.Empty;
        protected Color m_DropDownHighlightLightColor = Color.Empty;
        protected Color m_DropDownHighlightDarkColor = Color.Empty;
        protected Color m_DropDownPressedLightColor = Color.Empty;
        protected Color m_DropDownPressedDarkColor = Color.Empty;

        // floating CommandBar colors
        protected Color m_FloatHighlightButtonColor = Color.Empty;
        protected Color m_FloatHighlightButtonBorderColor = Color.Empty;
        protected Color m_FloatPressButtonColor = Color.Empty;
        protected Color m_FloatPressButtonBorderColor = Color.Empty;
        protected Color m_FloatPressCloseButtonBorderColor = Color.Empty;
        protected Color m_FloatPressCloseButtonColor = Color.Empty;
        protected Color m_FloatCommandBarLightColor = Color.Empty;
        protected Color m_FloatCommandBarDarkColor = Color.Empty;
        protected Color m_FloatLightBorderColor = Color.Empty;
        protected Color m_FloatBackgroundColor = Color.Empty;
        protected Color m_FloatBorderColor = Color.Empty;
        protected Color m_FloatCaptionColor = Color.Empty;

        // BarItems colors
        protected Color m_BarItemSeparatorColor = Color.Empty;
        protected Color m_BarItemPressBorderColor = Color.Empty;
        protected Color m_BarItemHighlightBorderColor = Color.Empty;
        protected Color m_BarItemPressLightColor = Color.Empty;
        protected Color m_BarItemPressDarkColor = Color.Empty;
        protected Color m_DropDownBarItemLightColor = Color.Empty;
        protected Color m_DropDownBarItemDarkColor = Color.Empty;
        protected Color m_DropDownBarItemBorderColor = Color.Empty;
        protected Color m_BarItemCheckLightColor = Color.Empty;
        protected Color m_BarItemCheckDarkColor = Color.Empty;
        protected Color m_BarItemCheckBorderColor = Color.Empty;
        protected Color m_BarItemCheckFlashColor = Color.Empty;
        protected Color m_BarItemPressFlashColor = Color.Empty;
        protected Color m_BarItemSelectFlashColor = Color.Empty;
        protected Color m_TextBarItemBackColor = Color.Empty;
        protected Color m_TextBarItemBorderColor = Color.Empty;
        protected Color m_TextBarItemBorderHighlightColor = Color.Empty;

        // colors for ComboButton
        protected Color m_ComboButtonLightColor = Color.Empty;
        protected Color m_ComboButtonDarkColor = Color.Empty;
        protected Color m_ComboButtonPressLightColor = Color.Empty;
        protected Color m_ComboButtonPressDarkColor = Color.Empty;
        protected Color m_ComboButtonHighlightLightColor = Color.Empty;
        protected Color m_ComboButtonHighlightDarkColor = Color.Empty;
        protected Color m_ComboButtonBorder = Color.Empty;
        protected Color m_ComboButtonPressBorder = Color.Empty;
        protected Color m_ComboButtonHighlightBorder = Color.Empty;

        //ButtonAdvColors
        protected Color m_ButtonPressedTopColor = Color.Empty;
        protected Color m_ButtonPressedBottomColor = Color.Empty;
        protected Color m_ButtonSelectedTopColor = Color.Empty;
        protected Color m_ButtonSelectedBottomColor = Color.Empty;
        protected Color m_ButtonDisabledTopColor = Color.Empty;
        protected Color m_ButtonDisabledBottomColor = Color.Empty;
        protected Color m_ButtonPressedBorderColor = Color.Empty;
        protected Color m_ButtonSelectedBorderColor = Color.Empty;
        protected Color m_ButtonDisabledBorderColor = Color.Empty;
        protected Color m_ButtonDefaultTopColor = Color.Empty;
        protected Color m_ButtonDefaultBottomColor = Color.Empty;
        protected Color m_ButtonDefaultBorderColor = Color.Empty;
        protected Color m_ButtonDefaultInternalBorderColor = Color.Empty;
        protected Color m_ButtonPressedInternalBorderColor = Color.Empty;
        protected Color m_ButtonSelectedInternalBorderColor = Color.Empty;

        #region Obsolete
        protected Color m_BlueButtonDefaultTopColor = Color.Empty;
        protected Color m_BlueButtonDefaultBottomColor = Color.Empty;
        protected Color m_BlueButtonDefaultBorderColor = Color.Empty;
        protected Color m_BlueButtonDefaultInternalBorderColor = Color.Empty;
        protected Color m_BlueButtonPressedInternalBorderColor = Color.Empty;
        protected Color m_BlueButtonSelectedInternalBorderColor = Color.Empty;
        protected Color m_SilverButtonDefaultTopColor = Color.Empty;
        protected Color m_SilverButtonDefaultBottomColor = Color.Empty;
        protected Color m_SilverButtonDefaultBorderColor = Color.Empty;
        protected Color m_SilverButtonDefaultInternalBorderColor = Color.Empty;
        protected Color m_SilverButtonPressedInternalBorderColor = Color.Empty;
        protected Color m_SilverButtonSelectedInternalBorderColor = Color.Empty;
        protected Color m_BlackButtonDefaultTopColor = Color.Empty;
        protected Color m_BlackButtonDefaultBottomColor = Color.Empty;
        protected Color m_BlackButtonDefaultBorderColor = Color.Empty;
        protected Color m_BlackButtonDefaultInternalBorderColor = Color.Empty;
        protected Color m_BlackButtonPressedInternalBorderColor = Color.Empty;
        protected Color m_BlackButtonSelectedInternalBorderColor = Color.Empty;
        #endregion

        //NumericUpDownExt colors
        protected Color m_NumericUpDownBorderColor = Color.Empty;
        protected Color m_NumericUpDownHighLightedBorderColor = Color.Empty;
        protected Color m_NumericUpDownSelectedBorderColor = Color.Empty;
        protected Color m_NumericUpDownArrowLightColor = Color.Empty;
        protected Color m_NumericUpDownArrowDarkColor = Color.Empty;

        // TabControlAdv colors
        protected Color m_TabDefaultBorderColor = Color.Empty;
        protected Color m_TabHotLightBottomBorderLineColor = Color.Empty;
        protected Color m_TabHotLightGradientTopBeginColor = Color.Empty;
        protected Color m_TabHotLightGradientTopEndColor = Color.Empty;
        protected Color m_TabHotLightGradientBottomBeginColor = Color.Empty;
        protected Color m_TabHotLightGradientBottomEndColor = Color.Empty;
        protected Color m_TabHotLightGradientCircleColor = Color.Empty;
        protected Color m_TabSelectedGradientTopColor = Color.Empty;
        protected Color m_TabSelectedGradientBottomColor = Color.Empty;
        protected Color m_TabSelectedInnerBorderColor = Color.Empty;
        protected Color m_TabHighlightInnerBorderColor = Color.Empty;
        protected Color m_TabSelectedHotLightBorderColor = Color.Empty;
        protected Color m_TabSelectedHotLightInnerBorderColor = Color.Empty;
        protected Color m_TabForeColor = Color.Empty;
        protected Color m_ActiveTabForeColor = Color.Empty;
        protected Color m_TabBackgroundColor = Color.Empty;
        protected Color m_TabScrollArrowColor = Color.Empty;

        // DockTabControl colors
        protected Color m_DockTabForeColor = Color.Empty;
        protected Color m_DockTabBackgroundColor = Color.Empty;

        //Treeview 
        protected Color m_SelectedNodeBackground = Color.Empty;
        protected Color m_TreeNodeArrowColor = Color.Empty;
        protected Color m_TreeviewBackColor = Color.Empty;
        protected Color m_TreeViewFontColor = Color.Empty;

        //TextBoxExt
        protected Color m_ActiveTextBoxBorderColor = Color.Empty;
        protected Color m_InactiveTextBoxBorderColor = Color.Empty;
        protected Color m_ActiveTextBoxBackColor = Color.Empty;
        protected Color m_InactiveTextBoxBackColor = Color.Empty;
        // Form colors
        protected Color m_ActiveFormBorderColor = Color.Empty;
        protected Color m_InactiveFormBorderColor = Color.Empty;
        protected Color m_FormTextColor = Color.Empty;

        protected Color m_ActiveTitleGradientBegin = Color.Empty;
        protected Color m_ActiveTitleGradientEnd = Color.Empty;
        protected Color m_InactiveTitleGradientBegin = Color.Empty;
        protected Color m_InactiveTitleGradientEnd = Color.Empty;

        protected Color m_SystemButtonSelectedGradientBegin = Color.Empty;
        protected Color m_SystemButtonSelectedGradientEnd = Color.Empty;
        protected Color m_SystemButtonPressedGradientBegin = Color.Empty;
        protected Color m_SystemButtonPressedGradientEnd = Color.Empty;
        protected Color m_SystemButtonBorderSelected = Color.Empty;
        protected Color m_SystemButtonBorderPressed = Color.Empty;
        protected Color m_FormBackground = Color.Empty;

        // UpDown colors
        protected Color m_UpDownArrowStartColor = Color.Empty;
        protected Color m_UpDownArrowEndColor = Color.Empty;

        protected Color m_UpDownBorderNormalColor = Color.Empty;
        protected Color m_UpDownBackgroundNormalColor = Color.Empty;
        protected Color m_UpDownBackgroundNormalStartColor = Color.Empty;
        protected Color m_UpDownBackgroundNormalEndColor = Color.Empty;

        protected Color m_UpDownBorderHotColor = Color.Empty;
        protected Color m_UpDownInnerBorderHotStartColor = Color.Empty;
        protected Color m_UpDownInnerBorderHotEndColor = Color.Empty;

        protected Color m_UpDownBorderPressedColor = Color.Empty;
        protected Color m_UpDownInnerBorderPressedStartColor = Color.Empty;
        protected Color m_UpDownInnerBorderPressedEndColor = Color.Empty;

        protected Color m_UpDownBackgroundDisabledStartColor = Color.Empty;
        protected Color m_UpDownBackgroundDisabledEndColor = Color.Empty;
        protected Color m_UpDownBorderDisabledColor = Color.Empty;

        protected Color m_UpDownBackgroundHotTopStartColor = Color.Empty;
        protected Color m_UpDownBackgroundHotTopEndColor = Color.Empty;
        protected Color m_UpDownBackgroundHotBottomStartColor = Color.Empty;
        protected Color m_UpDownBackgroundHotBottomEndColor = Color.Empty;

        protected Color m_UpDownBackgroundPressedTopStartColor = Color.Empty;
        protected Color m_UpDownBackgroundPressedTopEndColor = Color.Empty;
        protected Color m_UpDownBackgroundPressedBottomStartColor = Color.Empty;
        protected Color m_UpDownBackgroundPressedBottomEndColor = Color.Empty;

        // ComboBoxAdv colors
        protected Color m_ComboBoxAdvNormalBackColor = Color.Empty;
        protected Color m_ComboBoxAdvHotBackColor = Color.Empty;
        protected Color m_ComboBoxAdvNormalBorderColor = Color.Empty;
        protected Color m_ComboBoxAdvHotBorderColor = Color.Empty;
        protected Color m_ComboBoxAdvPushedBorderColor = Color.Empty;
        protected Color m_ComboBoxAdvButtonUpperLineColor = Color.Empty;
        protected Color m_ComboBoxAdvArrowColor = Color.Empty;
        protected Color m_ComboBoxAdvLowerArrowLineColor = Color.Empty;
        protected Color m_ComboBoxAdvHotBackgroundButtonColor1 = Color.Empty;
        protected Color m_ComboBoxAdvHotBackgroundButtonColor2 = Color.Empty;
        protected Color m_ComboBoxAdvHotBackgroundButtonColor3 = Color.Empty;
        protected Color m_ComboBoxAdvHotBackgroundButtonColor4 = Color.Empty;
        protected Color m_ComboBoxAdvNormalBackgroundButtonColor1 = Color.Empty;
        protected Color m_ComboBoxAdvNormalBackgroundButtonColor2 = Color.Empty;
        protected Color m_ComboBoxAdvNormalBackgroundButtonColor3 = Color.Empty;
        protected Color m_ComboBoxAdvNormalBackgroundButtonColor4 = Color.Empty;
        protected Color m_ComboBoxAdvPushedBackgroundButtonColor1 = Color.Empty;
        protected Color m_ComboBoxAdvPushedBackgroundButtonColor2 = Color.Empty;
        protected Color m_ComboBoxAdvPushedBackgroundButtonColor3 = Color.Empty;
        protected Color m_ComboBoxAdvPushedBackgroundButtonColor4 = Color.Empty;

        // CheckBoxAdv colors
        protected Color m_CheckBoxAdvNormalBackColor = Color.Empty;
        protected Color m_CheckBoxAdvSelectedBackColor = Color.Empty;
        protected Color m_CheckBoxAdvPushedBackColor = Color.Empty;
        protected Color m_CheckBoxAdvNormalBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvSelectedBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvPushedBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvNormalInternalBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvSelectedInternalBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvPushedInternalBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvNormalInternalRectangleBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvSelectedInternalRectangleBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvPushedInternalRectangleBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvNormalInternalRectangleColor = Color.Empty;
        protected Color m_CheckBoxAdvSelectedInternalRectangleColor = Color.Empty;
        protected Color m_CheckBoxAdvPushedInternalRectangleColor = Color.Empty;
        protected Color m_CheckBoxAdvNormalTickColor = Color.Empty;
        protected Color m_CheckBoxAdvSelectedTickColor = Color.Empty;
        protected Color m_CheckBoxAdvPushedTickColor = Color.Empty;
        protected Color m_CheckBoxAdvDisabledTickColor = Color.Empty;
        protected Color m_CheckBoxAdvIndeterminateRectangleColor = Color.Empty;
        protected Color m_CheckBoxAdvDisabledBackColor = Color.Empty;
        protected Color m_CheckBoxAdvDisabledBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvDisabledInternalBorderColor = Color.Empty;

        // RadioButtonAdv colors
        protected Color m_RadioButtonAdvNormalBackColor = Color.Empty;
        protected Color m_RadioButtonAdvNormalBorderColor = Color.Empty;
        protected Color m_RadioButtonAdvNormalInternalBorderColor = Color.Empty;
        protected Color m_RadioButtonAdvSelectedBackColor = Color.Empty;
        protected Color m_RadioButtonAdvSelectedBorderColor = Color.Empty;
        protected Color m_RadioButtonAdvSelectedInternalBorderColor = Color.Empty;
        protected Color m_RadioButtonAdvPushedBackColor = Color.Empty;
        protected Color m_RadioButtonAdvPushedBorderColor = Color.Empty;
        protected Color m_RadioButtonAdvPushedInternalBorderColor = Color.Empty;
        protected Color m_RadioButtonAdvCheckMarkBorderColor = Color.Empty;
        protected Color m_RadioButtonAdvCheckMarkNormalBottomColor = Color.Empty;
        protected Color m_RadioButtonAdvCheckMarkSelectedBottomColor = Color.Empty;
        protected Color m_RadioButtonAdvCheckMarkPushedBottomColor = Color.Empty;

        // TabBarSplitterControl colors
        protected Color m_TabBarSplitterBackColor = Color.Empty;
        protected Color m_TabBarSplitterBorderColor = Color.Empty;
        protected Color m_TabBarSplitterTextColor = Color.Empty;
        protected Color m_TabBarSplitterTabStartColor = Color.Empty;
        protected Color m_TabBarSplitterTabEndColor = Color.Empty;
        protected Color m_TabBarSplitterTabBarStartColor = Color.Empty;
        protected Color m_TabBarSplitterTabBarEndColor = Color.Empty;
        protected Color m_TabBarSplitterButtonHoveredStartColor = Color.Empty;
        protected Color m_TabBarSplitterButtonHoveredEndColor = Color.Empty;
        protected Color m_TabBarSplitterButtonPushedStartColor = Color.Empty;
        protected Color m_TabBarSplitterButtonPushedEndColor = Color.Empty;
        protected Color m_TabBarSplitterSizeGripperColor = Color.Empty;
        protected Color m_TabBarSplitterSizeGripperLightColor = Color.Empty;
        protected Color m_TabBarSplitterSizeGripperDarkColor = Color.Empty;

        // XPTaskBar colors
        protected Color m_XPTaskBarBorderColor = Color.Empty;
        protected Color m_XPTaskBarBoxBackColor = Color.Empty;
        protected Color m_XPTaskBarBoxForeColor = Color.Empty;
        protected Color m_XPTaskBarBoxHeaderUpperLineColor = Color.Empty;
        protected Color m_XPTaskBarBoxHeaderLowerLineColor = Color.Empty;
        protected Color m_XPTaskBarBoxArrowColor = Color.Empty;
        protected Color m_XPTaskBarBoxActiveHighlightedItemColor = Color.Empty;
        protected Color m_XPTaskBarBoxInactiveHighlightedItemColor = Color.Empty;

        // ColorUIAdv colors
        protected Color m_ColorUIAdvBackColor = Color.Empty;
        protected Color m_ColorUIAdvTextColor = Color.Empty;
        protected Color m_ColorUIAdvItemBorderColor = Color.Empty;
        protected Color m_ColorUIAdvHighlightedBorderColor = Color.Empty;
        protected Color m_ColorUIAdvSelectedBorderColor = Color.Empty;
        protected Color m_ColorUIAdvSelectedHighlightedBorderColor = Color.Empty;
        protected Color m_ColorUIAdvGroupHeaderBackColor = Color.Empty;

        //StatusBarExt
        protected Color m_StatusBarExtTopGradient = Color.Empty;
        protected Color m_StatusBarExtBottomGradient = Color.Empty;
        protected Color m_StatusBarExtFillColor = Color.Empty;
        #endregion

        #region Class Properties

        #region TabItem Colors
        public Color TabPanelBorderColor
        {
            get
            {
                return m_TabPanelBorderColor;
            }
            set
            {
                if (m_TabPanelBorderColor != value)
                {
                    m_TabPanelBorderColor = value;
                }
            }
        }

        public Color TabPanelBackColor
        {
            get
            {
                return m_TabPanelBackColor;
            }
            set
            {
                if (m_TabPanelBackColor != value)
                {
                    m_TabPanelBackColor = value;
                }
            }
        }

        public Color TabPanelColor
        {
            get
            {
                return m_TabPanelColor;
            }
            set
            {
                if (m_TabPanelColor != value)
                {
                    m_TabPanelColor = value;
                }
            }
        }

        public Color TabItemBorderColor
        {
            get
            {
                return m_TabItemBorderColor;
            }
            set
            {
                if (m_TabItemBorderColor != value)
                {
                    m_TabItemBorderColor = value;
                }
            }
        }

        public Color TabItemInnerBorderColor
        {
            get
            {
                return m_TabItemInnerBorderColor;
            }
            set
            {
                if (m_TabItemInnerBorderColor != value)
                {
                    m_TabItemInnerBorderColor = value;
                }
            }
        }

        public Color TabItemOuterBorderColor
        {
            get
            {
                return m_TabItemOuterBorderColor;
            }
            set
            {
                if (m_TabItemOuterBorderColor != value)
                {
                    m_TabItemOuterBorderColor = value;
                }
            }
        }

        public Color TabItemTextColor
        {
            get
            {
                return m_TabItemTextColor;
            }
            set
            {
                if (m_TabItemTextColor != value)
                {
                    m_TabItemTextColor = value;
                }
            }
        }

        public Color TabItemActiveBottomColor
        {
            get
            {
                return m_TabItemActiveBottomColor;
            }
            set
            {
                if (m_TabItemActiveBottomColor != value)
                {
                    m_TabItemActiveBottomColor = value;
                }
            }
        }

        [Obsolete("Use TabItemTopGradientColor property instead.")]
        public Color TopGradientColor
        {
            get
            {
                return this.TabItemTopGradientColor;
            }
            set
            {
                this.TabItemTopGradientColor = value;
            }
        }

        public Color TabItemTopGradientColor
        {
            get
            {
                return m_TabItemTopGradientColor;
            }
            set
            {
                if (m_TabItemTopGradientColor != value)
                {
                    m_TabItemTopGradientColor = value;
                }
            }
        }

        public Color TabItemInActiveBottomColor
        {
            get
            {
                return m_TabItemInActiveBottomColor;
            }
            set
            {
                if (m_TabItemInActiveBottomColor != value)
                {
                    m_TabItemInActiveBottomColor = value;
                }
            }
        }

        public Color TabItemMiddleLineColor
        {
            get
            {
                return m_TabItemMiddleLineColor;
            }
            set
            {
                if (m_TabItemMiddleLineColor != value)
                {
                    m_TabItemMiddleLineColor = value;
                }
            }
        }
        #endregion

        #region DataTimePickerAdv Colors
        /// <summary>
        /// Gets or sets border color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerBorderColor
        {
            get { return m_DataTimePickerBorderColor; }
            set
            {
                if (m_DataTimePickerBorderColor != value)
                    m_DataTimePickerBorderColor = value;
            }
        }

        /// <summary>
        /// Gets or sets highlighted border color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerHighLightedBorderColor
        {
            get { return m_DataTimePickerHighLightedBorderColor; }
            set
            {
                if (m_DataTimePickerHighLightedBorderColor != value)
                    m_DataTimePickerHighLightedBorderColor = value;
            }
        }

        /// <summary>
        /// Gets or sets selected border color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerSelectedBorderColor
        {
            get { return m_DataTimePickerSelectedBorderColor; }
            set
            {
                if (m_DataTimePickerSelectedBorderColor != value)
                    m_DataTimePickerSelectedBorderColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a dropdown arrow color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerDropDownArrowColor
        {
            get { return m_DataTimePickerDropDownArrowColor; }
            set
            {
                if (m_DataTimePickerDropDownArrowColor != value)
                    m_DataTimePickerDropDownArrowColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a dropdown gradient light color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerDropDownLightColor
        {
            get { return m_DataTimePickerDropDownLightColor; }
            set
            {
                if (m_DataTimePickerDropDownLightColor != value)
                    m_DataTimePickerDropDownLightColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a dropdown gradient dark color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerDropDownDarkColor
        {
            get { return m_DataTimePickerDropDownDarkColor; }
            set
            {
                if (m_DataTimePickerDropDownDarkColor != value)
                    m_DataTimePickerDropDownDarkColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a dropdown highlighted gradient light color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerDropDownHighLightLightColor
        {
            get { return m_DataTimePickerDropDownHighLightLightColor; }
            set
            {
                if (m_DataTimePickerDropDownHighLightLightColor != value)
                    m_DataTimePickerDropDownHighLightLightColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a dropdown highlighted gradient dark color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerDropDownHighLightDarkColor
        {
            get { return m_DataTimePickerDropDownHighLightDarkColor; }
            set
            {
                if (m_DataTimePickerDropDownHighLightDarkColor != value)
                    m_DataTimePickerDropDownHighLightDarkColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a dropdown selected gradient light color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerDropDownSelectedLightColor
        {
            get { return m_DataTimePickerDropDownSelectedLightColor; }
            set
            {
                if (m_DataTimePickerDropDownSelectedLightColor != value)
                    m_DataTimePickerDropDownSelectedLightColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a dropdown selected gradient dark color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerDropDownSelectedDarkColor
        {
            get { return m_DataTimePickerDropDownSelectedDarkColor; }
            set
            {
                if (m_DataTimePickerDropDownSelectedDarkColor != value)
                    m_DataTimePickerDropDownSelectedDarkColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a checkbox color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerCheckBoxNormalColor
        {
            get { return m_DataTimePickerCheckBoxNormalColor; }
            set
            {
                if (m_DataTimePickerCheckBoxNormalColor != value)
                    m_DataTimePickerCheckBoxNormalColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a checkbox selected color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerCheckBoxSelectedColor
        {
            get { return m_DataTimePickerCheckBoxSelectedColor; }
            set
            {
                if (m_DataTimePickerCheckBoxSelectedColor != value)
                    m_DataTimePickerCheckBoxSelectedColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a checkbox pushed border color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerCheckBoxBorderPushedColor
        {
            get { return m_DataTimePickerCheckBoxBorderPushedColor; }
            set
            {
                if (m_DataTimePickerCheckBoxBorderPushedColor != value)
                    m_DataTimePickerCheckBoxBorderPushedColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a checkbox border color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerCheckBoxBorderNormalColor
        {
            get { return m_DataTimePickerCheckBoxBorderNormalColor; }
            set
            {
                if (m_DataTimePickerCheckBoxBorderNormalColor != value)
                    m_DataTimePickerCheckBoxBorderNormalColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a checkbox inner rectangle border color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerCheckBoxInnerRectBorderNormalColor
        {
            get { return m_DataTimePickerCheckBoxInnerRectBorderNormalColor; }
            set
            {
                if (m_DataTimePickerCheckBoxInnerRectBorderNormalColor != value)
                    m_DataTimePickerCheckBoxInnerRectBorderNormalColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a checkbox inner rectangle selected border color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerCheckBoxInnerRectBorderSelectedColor
        {
            get { return m_DataTimePickerCheckBoxInnerRectBorderSelectedColor; }
            set
            {
                if (m_DataTimePickerCheckBoxInnerRectBorderSelectedColor != value)
                    m_DataTimePickerCheckBoxInnerRectBorderSelectedColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a checkbox inner rectangle pushed border color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerCheckBoxInnerRectBorderPushedColor
        {
            get { return m_DataTimePickerCheckBoxInnerRectBorderPushedColor; }
            set
            {
                if (m_DataTimePickerCheckBoxInnerRectBorderPushedColor != value)
                    m_DataTimePickerCheckBoxInnerRectBorderPushedColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a checkbox inner rectangle filling color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerCheckBoxInnerRectFillNormalColor
        {
            get { return m_DataTimePickerCheckBoxInnerRectFillNormalColor; }
            set
            {
                if (m_DataTimePickerCheckBoxInnerRectFillNormalColor != value)
                    m_DataTimePickerCheckBoxInnerRectFillNormalColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a checkbox inner rectangle filling selected color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerCheckBoxInnerRectFillSelectedColor
        {
            get { return m_DataTimePickerCheckBoxInnerRectFillSelectedColor; }
            set
            {
                if (m_DataTimePickerCheckBoxInnerRectFillSelectedColor != value)
                    m_DataTimePickerCheckBoxInnerRectFillSelectedColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a checkbox inner rectangle filling pushed color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerCheckBoxInnerRectFillPushedColor
        {
            get { return m_DataTimePickerCheckBoxInnerRectFillPushedColor; }
            set
            {
                if (m_DataTimePickerCheckBoxInnerRectFillPushedColor != value)
                    m_DataTimePickerCheckBoxInnerRectFillPushedColor = value;
            }
        }

        /// <summary>
        /// Gets or sets highlighted fore color for <see cref="DateTimePickerAdv">.
        /// </summary>
        public Color DataTimePickerHighLightedForeColor
        {
            get { return m_DataTimePickerHighLightedForeColor; }
            set
            {
                if (m_DataTimePickerHighLightedForeColor != value)
                    m_DataTimePickerHighLightedForeColor = value;
            }
        }

        /// <summary>
        /// Gets or sets border color for <see cref="NumericUpDownExt">.
        /// </summary>
        public Color NumericUpDownBorderColor
        {
            get { return m_NumericUpDownBorderColor; }
            set
            {
                if (m_NumericUpDownBorderColor != value)
                    m_NumericUpDownBorderColor = value;
            }
        }

        /// <summary>
        /// Gets or sets highlighted border color for <see cref="NumericUpDownExt">.
        /// </summary>
        public Color NumericUpDownHighLightedBorderColor
        {
            get { return m_NumericUpDownHighLightedBorderColor; }
            set
            {
                if (m_NumericUpDownHighLightedBorderColor != value)
                    m_NumericUpDownHighLightedBorderColor = value;
            }
        }

        /// <summary>
        /// Gets or sets selected border color for <see cref="NumericUpDownExt">.
        /// </summary>
        public Color NumericUpDownSelectedBorderColor
        {
            get { return m_NumericUpDownSelectedBorderColor; }
            set
            {
                if (m_NumericUpDownSelectedBorderColor != value)
                    m_NumericUpDownSelectedBorderColor = value;
            }
        }

        /// <summary>
        /// Gets or sets dropdown arrow gradient light color for <see cref="NumericUpDownExt">.
        /// </summary>
        public Color NumericUpDownArrowLightColor
        {
            get { return m_NumericUpDownArrowLightColor; }
            set
            {
                if (m_NumericUpDownArrowLightColor != value)
                    m_NumericUpDownArrowLightColor = value;
            }
        }

        /// <summary>
        /// Gets or sets dropdown arrow gradient dark color for <see cref="NumericUpDownExt">.
        /// </summary>
        public Color NumericUpDownArrowDarkColor
        {
            get { return m_NumericUpDownArrowDarkColor; }
            set
            {
                if (m_NumericUpDownArrowDarkColor != value)
                    m_NumericUpDownArrowDarkColor = value;
            }
        }

        /// <summary>
        /// Gets or sets gradient start color for <see cref="MonthCalendarAdv">.
        /// </summary>
        public Color MonthCalendarHeaderStartColor
        {
            get { return m_MonthCalendarHeaderStartColor; }
            set
            {
                if (m_MonthCalendarHeaderStartColor != value)
                    m_MonthCalendarHeaderStartColor = value;
            }
        }

        /// <summary>
        /// Gets or sets gradient end color for <see cref="MonthCalendarAdv">.
        /// </summary>
        public Color MonthCalendarHeaderEndColor
        {
            get { return m_MonthCalendarHeaderEndColor; }
            set
            {
                if (m_MonthCalendarHeaderEndColor != value)
                    m_MonthCalendarHeaderEndColor = value;
            }
        }

        /// <summary>
        /// Gets or sets gradient end color for <see cref="MonthCalendarAdv">.
        /// </summary>
        public Color MonthCalendarBackgroundColor
        {
            get { return m_MonthCalendarBackgroundColor; }
            set
            {
                if (m_MonthCalendarBackgroundColor != value)
                    m_MonthCalendarBackgroundColor = value;
            }
        }

        /// <summary>
        /// Gets or sets fore color for <see cref="MonthCalendarAdv">.
        /// </summary>
        public Color MonthCalendarForeColor
        {
            get { return m_MonthCalendarForeColor; }
            set
            {
                if (m_MonthCalendarForeColor != value)
                    m_MonthCalendarForeColor = value;
            }
        }

        #endregion

        #region GroupBar Colors

        /// <summary>
        /// Gets or sets border color for <see cref="GroupBar">.
        /// </summary>
        public Color GroupBarBorderColor
        {
            get
            {
                return m_GroupBarBorderColor;
            }
            set
            {
                if (m_GroupBarBorderColor != value)
                {
                    m_GroupBarBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets gradient dark color for <see cref="GroupBarItem">.
        /// </summary>
        public Color GroupBarItemColorDark
        {
            get
            {
                return m_GroupBarItemColorDark;
            }
            set
            {
                if (m_GroupBarItemColorDark != value)
                {
                    m_GroupBarItemColorDark = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets gradient light color for <see cref="GroupBarItem">.
        /// </summary>
        public Color GroupBarItemColorLight
        {
            get
            {
                return m_GroupBarItemColorLight;
            }
            set
            {
                if (m_GroupBarItemColorLight != value)
                {
                    m_GroupBarItemColorLight = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets <see cref="GroupBar">'s header gradient color dark.
        /// </summary>
        public Color GroupBarHeaderColorDark
        {
            get
            {
                return m_GroupBarHeaderColorDark;
            }
            set
            {
                if (m_GroupBarHeaderColorDark != value)
                {
                    m_GroupBarHeaderColorDark = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets <see cref="GroupBar">'s header gradient color light.
        /// </summary>
        public Color GroupBarHeaderColorLight
        {
            get
            {
                return m_GroupBarHeaderColorLight;
            }
            set
            {
                if (m_GroupBarHeaderColorLight != value)
                {
                    m_GroupBarHeaderColorLight = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets gradient highlight light color for <see cref="GroupBar">.
        /// </summary>
        public Color GroupBarHighlightColorLight
        {
            get
            {
                return m_GroupBarHighlightColorLight;
            }
            set
            {
                if (m_GroupBarHighlightColorLight != value)
                {
                    m_GroupBarHighlightColorLight = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets gradient highlight dark color for <see cref="GroupBar">.
        /// </summary>
        public Color GroupBarHighlightColorDark
        {
            get
            {
                return m_GroupBarHighlightColorDark;
            }
            set
            {
                if (m_GroupBarHighlightColorDark != value)
                {
                    m_GroupBarHighlightColorDark = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets gradient selected dark color for <see cref="GroupBar">.
        /// </summary>
        public Color GroupBarSelectedColorDark
        {
            get
            {
                return m_GroupBarSelectedColorDark;
            }
            set
            {
                if (m_GroupBarSelectedColorDark != value)
                {
                    m_GroupBarSelectedColorDark = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets gradient selected light color for <see cref="GroupBar">.
        /// </summary>
        public Color GroupBarSelectedColorLight
        {
            get
            {
                return m_GroupBarSelectedColorLight;
            }
            set
            {
                if (m_GroupBarSelectedColorLight != value)
                {
                    m_GroupBarSelectedColorLight = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets gradient selected dark color for the upper part of <see cref="GroupBarItem">.
        /// </summary>
        public Color GroupBarSelectedTopColorDark
        {
            get
            {
                return m_GroupBarSelectedTopColorDark;
            }
            set
            {
                if (m_GroupBarSelectedTopColorDark != value)
                {
                    m_GroupBarSelectedTopColorDark = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets gradient selected light color for the upper part of <see cref="GroupBarItem">.
        /// </summary>
        public Color GroupBarSelectedTopColorLight
        {
            get
            {
                return m_GroupBarSelectedTopColorLight;
            }
            set
            {
                if (m_GroupBarSelectedTopColorLight != value)
                {
                    m_GroupBarSelectedTopColorLight = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets gradient highlighted and selected dark color for <see cref="GroupBar">.
        /// </summary>
        public Color GroupBarSelectedHighlightColorDark
        {
            get
            {
                return m_GroupBarSelectedHighlightColorDark;
            }
            set
            {
                if (m_GroupBarSelectedHighlightColorDark != value)
                {
                    m_GroupBarSelectedHighlightColorDark = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets gradient highlighted and selected light color for <see cref="GroupBar">.
        /// </summary>
        public Color GroupBarSelectedHighlightColorLight
        {
            get
            {
                return m_GroupBarSelectedHighlightColorLight;
            }
            set
            {
                if (m_GroupBarSelectedHighlightColorLight != value)
                {
                    m_GroupBarSelectedHighlightColorLight = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets text color for <see cref="GroupBarItem">.
        /// </summary>
        public Color GroupBarItemTextColor
        {
            get
            {
                return m_GroupBarItemTextColor;
            }
            set
            {
                if (m_GroupBarItemTextColor != value)
                {
                    m_GroupBarItemTextColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets Back color for <see cref="GroupBarItem">.
        /// </summary>
        public Color GroupBarBackColor
        {
            get
            {
                return m_GroupBarBackColor;
            }
            set
            {
                if (m_GroupBarBackColor != value)
                {
                    m_GroupBarBackColor = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets <see cref="GroupBar">'s header text color.
        /// </summary>
        public Color GroupBarHeaderTextColor
        {
            get
            {
                return m_GroupBarHeaderTextColor;
            }
            set
            {
                if (m_GroupBarHeaderTextColor != value)
                {
                    m_GroupBarHeaderTextColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets gradient dark color for splitter of <see cref="GroupBar">.
        /// </summary>
        public Color GroupBarSplitterColorDark
        {
            get
            {
                return m_GroupBarSplitterColorDark;
            }
            set
            {
                if (m_GroupBarSplitterColorDark != value)
                {
                    m_GroupBarSplitterColorDark = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets gradient light color for splitter of <see cref="GroupBar">.
        /// </summary>
        public Color GroupBarSplitterColorLight
        {
            get
            {
                return m_GroupBarSplitterColorLight;
            }
            set
            {
                if (m_GroupBarSplitterColorLight != value)
                {
                    m_GroupBarSplitterColorLight = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the client area background color of <see cref="GroupBar">.
        /// </summary>
        public Color GroupBarClientAreaBackground
        {
            get
            {
                return m_GroupBarClientAreaBackground;
            }
            set
            {
                m_GroupBarClientAreaBackground = value;
            }
        }

        #endregion

        #region XPTaskPane Colors
        /// <summary>
        /// Gets or sets the color for the internal border of <see cref="XPTaskPane">.
        /// </summary>
        public Color XPTaskPaneInternalBorderColor
        {
            get
            {
                return m_XPTaskPaneInternalBorderColor;
            }
            set
            {
                if (m_XPTaskPaneInternalBorderColor != value)
                {
                    m_XPTaskPaneInternalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for the border of <see cref="XPTaskPane">.
        /// </summary>
        public Color XPTaskPaneBorderColor
        {
            get
            {
                return m_XPTaskPaneBorderColor;
            }
            set
            {
                if (m_XPTaskPaneBorderColor != value)
                {
                    m_XPTaskPaneBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the back color for  <see cref="XPTaskPage">.
        /// </summary>
        public Color XPTaskPageBackColor
        {
            get
            {
                return m_XPTaskPageBackColor;
            }
            set
            {
                if (m_XPTaskPageBackColor != value)
                {
                    m_XPTaskPageBackColor = value;
                }
            }
        }
        #endregion

        #region Menu Colors
        /// <summary>
        /// Gets or sets the color for border of the menu.
        /// </summary>
        public Color MenuBorderColor
        {
            get
            {
                return m_MenuBorderColor;
            }
            set
            {
                if (m_MenuBorderColor != value)
                {
                    m_MenuBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for separator of the menu.
        /// </summary>
        public Color MenuSeparatorColor
        {
            get
            {
                return m_MenuSeparatorColor;
            }
            set
            {
                if (m_MenuSeparatorColor != value)
                {
                    m_MenuSeparatorColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the dark color for highlighted item of the menu.
        /// </summary>
        public Color MenuItemDarkColor
        {
            get
            {
                return m_MenuItemDarkColor;
            }
            set
            {
                if (m_MenuItemDarkColor != value)
                {
                    m_MenuItemDarkColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the light color for highlighted item of the menu.
        /// </summary>
        public Color MenuItemLightColor
        {
            get
            {
                return m_MenuItemLightColor;
            }
            set
            {
                if (m_MenuItemLightColor != value)
                {
                    m_MenuItemLightColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the border color for highlighted item of the menu.
        /// </summary>
        public Color MenuItemBorderColor
        {
            get
            {
                return m_MenuItemBorderColor;
            }
            set
            {
                if (m_MenuItemBorderColor != value)
                {
                    m_MenuItemBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the dark color for column of the menu.
        /// </summary>
        public Color MenuColumnColor
        {
            get
            {
                return m_MenuColumnColor;
            }
            set
            {
                if (m_MenuColumnColor != value)
                {
                    m_MenuColumnColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the separator color for column of the menu.
        /// </summary>
        public Color MenuColumnSeparatorColor
        {
            get
            {
                return m_MenuColumnSeparatorColor;
            }
            set
            {
                if (m_MenuColumnSeparatorColor != value)
                {
                    m_MenuColumnSeparatorColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the light color for arrow of the menu item.
        /// </summary>
        public Color MenuItemArrowLightColor
        {
            get
            {
                return m_MenuItemArrowLightColor;
            }
            set
            {
                if (m_MenuItemArrowLightColor != value)
                {
                    m_MenuItemArrowLightColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the dark color for arrow of the menu item.
        /// </summary>
        public Color MenuItemArrowDarkColor
        {
            get
            {
                return m_MenuItemArrowDarkColor;
            }
            set
            {
                if (m_MenuItemArrowDarkColor != value)
                {
                    m_MenuItemArrowDarkColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for check mark of the menu.
        /// </summary>
        public Color MenuCheckedColor
        {
            get
            {
                return m_MenuCheckedColor;
            }
            set
            {
                if (m_MenuCheckedColor != value)
                {
                    m_MenuCheckedColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color for check mark of the menu.
        /// </summary>
        public Color MenuCheckedFillColor
        {
            get
            {
                return m_MenuCheckedFillColor;
            }
            set
            {
                if (m_MenuCheckedFillColor != value)
                {
                    m_MenuCheckedFillColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for border check mark of the menu.
        /// </summary>
        public Color MenuCheckedBorderColor
        {
            get
            {
                return m_MenuCheckedBorderColor;
            }
            set
            {
                if (m_MenuCheckedBorderColor != value)
                {
                    m_MenuCheckedBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the border color for TextBox item of the menu.
        /// </summary>
        public Color MenuTextBoxBorderColor
        {
            get
            {
                return m_MenuTextBoxBorderColor;
            }
            set
            {
                if (m_MenuTextBoxBorderColor != value)
                {
                    m_MenuTextBoxBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color for TextBox item of the menu.
        /// </summary>
        public Color MenuTextBoxBackColor
        {
            get
            {
                return m_MenuTextBoxBackColor;
            }
            set
            {
                if (m_MenuTextBoxBackColor != value)
                {
                    m_MenuTextBoxBackColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for ComboButton of the menu.
        /// </summary>
        public Color MenuComboButtonPushed1Color
        {
            get
            {
                return m_MenuComboButtonPushed1Color;
            }
            set
            {
                if (m_MenuComboButtonPushed1Color != value)
                {
                    m_MenuComboButtonPushed1Color = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for ComboButton of the menu.
        /// </summary>
        public Color MenuComboButtonPushed2Color
        {
            get
            {
                return m_MenuComboButtonPushed2Color;
            }
            set
            {
                if (m_MenuComboButtonPushed2Color != value)
                {
                    m_MenuComboButtonPushed2Color = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for ComboButton of the menu.
        /// </summary>
        public Color MenuComboButtonPushed3Color
        {
            get
            {
                return m_MenuComboButtonPushed3Color;
            }
            set
            {
                if (m_MenuComboButtonPushed3Color != value)
                {
                    m_MenuComboButtonPushed3Color = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for ComboButton of the menu.
        /// </summary>
        public Color MenuComboButtonPushed4Color
        {
            get
            {
                return m_MenuComboButtonPushed4Color;
            }
            set
            {
                if (m_MenuComboButtonPushed4Color != value)
                {
                    m_MenuComboButtonPushed4Color = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the light color for highlighted ComboButton of the menu.
        /// </summary>
        public Color MenuComboButtonHighlightLightColor
        {
            get
            {
                return m_MenuComboButtonHighlightLightColor;
            }
            set
            {
                if (m_MenuComboButtonHighlightLightColor != value)
                {
                    m_MenuComboButtonHighlightLightColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the dark color for highlighted ComboButton of the menu.
        /// </summary>
        public Color MenuComboButtonHighlightDarkColor
        {
            get
            {
                return m_MenuComboButtonHighlightDarkColor;
            }
            set
            {
                if (m_MenuComboButtonHighlightDarkColor != value)
                {
                    m_MenuComboButtonHighlightDarkColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for arrow ComboButton of the menu.
        /// </summary>
        public Color MenuComboButtonArrowColor
        {
            get
            {
                return m_MenuComboButtonArrowColor;
            }
            set
            {
                if (m_MenuComboButtonArrowColor != value)
                {
                    m_MenuComboButtonArrowColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color of the menu.
        /// </summary>
        public Color MenuBackground
        {
            get
            {
                return m_MenuBackground;
            }
            set
            {
                if (m_MenuBackground != value)
                {
                    m_MenuBackground = value;
                }
            }
        }

        #endregion

        #region CommandBar Colors
        /// <summary>
        /// Gets or sets light color for dropdown button of the CommandBar.
        /// </summary>
        public Color DropDownLightColor
        {
            get
            {
                return m_DropDownLightColor;
            }
            set
            {
                if (m_DropDownLightColor != value)
                {
                    m_DropDownLightColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets dark color for dropdown button of the CommandBar.
        /// </summary>
        public Color DropDownDarkColor
        {
            get
            {
                return m_DropDownDarkColor;
            }
            set
            {
                if (m_DropDownDarkColor != value)
                {
                    m_DropDownDarkColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets dark color of the CommandBar.
        /// </summary>
        public Color CommandBarDarkColor
        {
            get
            {
                return m_CommandBarDarkColor;
            }
            set
            {
                if (m_CommandBarDarkColor != value)
                {
                    m_CommandBarDarkColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets light color of the CommandBar.
        /// </summary>
        public Color CommandBarLightColor
        {
            get
            {
                return m_CommandBarLightColor;
            }
            set
            {
                if (m_CommandBarLightColor != value)
                {
                    m_CommandBarLightColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets color for border of the CommandBar.
        /// </summary>
        public Color CommandBarBorderColor
        {
            get
            {
                return m_CommandBarBorderColor;
            }
            set
            {
                if (m_CommandBarBorderColor != value)
                {
                    m_CommandBarBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets background color of the DockBar.
        /// </summary>
        public Color DockBarBackColor
        {
            get
            {
                return m_DockBarBackColor;
            }
            set
            {
                if (m_DockBarBackColor != value)
                {
                    m_DockBarBackColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets light color for highlight dropdown button of the CommandBar.
        /// </summary>
        public Color DropDownHighlightLightColor
        {
            get
            {
                return m_DropDownHighlightLightColor;
            }
            set
            {
                if (m_DropDownHighlightLightColor != value)
                {
                    m_DropDownHighlightLightColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets dark color for highlight dropdown button of the CommandBar.
        /// </summary>
        public Color DropDownHighlightDarkColor
        {
            get
            {
                return m_DropDownHighlightDarkColor;
            }
            set
            {
                if (m_DropDownHighlightDarkColor != value)
                {
                    m_DropDownHighlightDarkColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets light color for pressed dropdown button of the CommandBar.
        /// </summary>
        public Color DropDownPressedLightColor
        {
            get
            {
                return m_DropDownPressedLightColor;
            }
            set
            {
                if (m_DropDownPressedLightColor != value)
                {
                    m_DropDownPressedLightColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets dark color for pressed dropdown button of the CommandBar.
        /// </summary>
        public Color DropDownPressedDarkColor
        {
            get
            {
                return m_DropDownPressedDarkColor;
            }
            set
            {
                if (m_DropDownPressedDarkColor != value)
                {
                    m_DropDownPressedDarkColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets color for highlighted dropdown button of the floating CommandBar.
        /// </summary>
        public Color FloatHighlightButtonColor
        {
            get
            {
                return m_FloatHighlightButtonColor;
            }
            set
            {
                if (m_FloatHighlightButtonColor != value)
                {
                    m_FloatHighlightButtonColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets border color for highlighted dropdown button of the floating CommandBar.
        /// </summary>
        public Color FloatHighlightButtonBorderColor
        {
            get
            {
                return m_FloatHighlightButtonBorderColor;
            }
            set
            {
                if (m_FloatHighlightButtonBorderColor != value)
                {
                    m_FloatHighlightButtonBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets color for pressed dropdown button of the floating CommandBar.
        /// </summary>
        public Color FloatPressButtonColor
        {
            get
            {
                return m_FloatPressButtonColor;
            }
            set
            {
                if (m_FloatPressButtonColor != value)
                {
                    m_FloatPressButtonColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets border color for pressed dropdown button of the floating CommandBar.
        /// </summary>
        public Color FloatPressButtonBorderColor
        {
            get
            {
                return m_FloatPressButtonBorderColor;
            }
            set
            {
                if (m_FloatPressButtonBorderColor != value)
                {
                    m_FloatPressButtonBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets border color for pressed close button of the floating CommandBar.
        /// </summary>
        public Color FloatPressCloseButtonBorderColor
        {
            get
            {
                return m_FloatPressCloseButtonBorderColor;
            }
            set
            {
                if (m_FloatPressCloseButtonBorderColor != value)
                {
                    m_FloatPressCloseButtonBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets color for pressed close button of the floating CommandBar.
        /// </summary>
        public Color FloatPressCloseButtonColor
        {
            get
            {
                return m_FloatPressCloseButtonColor;
            }
            set
            {
                if (m_FloatPressCloseButtonColor != value)
                {
                    m_FloatPressCloseButtonColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets light color of the floating CommandBar.
        /// </summary>
        public Color FloatCommandBarLightColor
        {
            get
            {
                return m_FloatCommandBarLightColor;
            }
            set
            {
                if (m_FloatCommandBarLightColor != value)
                {
                    m_FloatCommandBarLightColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets dark color of the floating CommandBar.
        /// </summary>
        public Color FloatCommandBarDarkColor
        {
            get
            {
                return m_FloatCommandBarDarkColor;
            }
            set
            {
                if (m_FloatCommandBarDarkColor != value)
                {
                    m_FloatCommandBarDarkColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets color for light border of the floating CommandBar.
        /// </summary>
        public Color FloatLightBorderColor
        {
            get
            {
                return m_FloatLightBorderColor;
            }
            set
            {
                if (m_FloatLightBorderColor != value)
                {
                    m_FloatLightBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets background color of the floating CommandBar.
        /// </summary>
        public Color FloatBackgroundColor
        {
            get
            {
                return m_FloatBackgroundColor;
            }
            set
            {
                if (m_FloatBackgroundColor != value)
                {
                    m_FloatBackgroundColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets color for border of the floating CommandBar.
        /// </summary>
        public Color FloatBorderColor
        {
            get
            {
                return m_FloatBorderColor;
            }
            set
            {
                if (m_FloatBorderColor != value)
                {
                    m_FloatBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets color for caption text of the floating CommandBar.
        /// </summary>
        public Color FloatCaptionColor
        {
            get
            {
                return m_FloatCaptionColor;
            }
            set
            {
                if (m_FloatCaptionColor != value)
                {
                    m_FloatCaptionColor = value;
                }
            }
        }

        #endregion

        #region BarItem Colors
        /// <summary>
        /// Gets or sets color for separator line of the CommandBar.
        /// </summary>
        public Color BarItemSeparatorColor
        {
            get
            {
                return m_BarItemSeparatorColor;
            }
            set
            {
                if (m_BarItemSeparatorColor != value)
                {
                    m_BarItemSeparatorColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets color for pressed border of the BarItem.
        /// </summary>
        public Color BarItemPressBorderColor
        {
            get
            {
                return m_BarItemPressBorderColor;
            }
            set
            {
                if (m_BarItemPressBorderColor != value)
                {
                    m_BarItemPressBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets color for highlighted border of the BarItem.
        /// </summary>
        public Color BarItemHighlightBorderColor
        {
            get
            {
                return m_BarItemHighlightBorderColor;
            }
            set
            {
                if (m_BarItemHighlightBorderColor != value)
                {
                    m_BarItemHighlightBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets light color for background of the BarItem.
        /// </summary>
        public Color BarItemPressLightColor
        {
            get
            {
                return m_BarItemPressLightColor;
            }
            set
            {
                if (m_BarItemPressLightColor != value)
                {
                    m_BarItemPressLightColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets dark color for background of the BarItem.
        /// </summary>
        public Color BarItemPressDarkColor
        {
            get
            {
                return m_BarItemPressDarkColor;
            }
            set
            {
                if (m_BarItemPressDarkColor != value)
                {
                    m_BarItemPressDarkColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets light color for background of the DropDownBarItem.
        /// </summary>
        public Color DropDownBarItemLightColor
        {
            get
            {
                return m_DropDownBarItemLightColor;
            }
            set
            {
                if (m_DropDownBarItemLightColor != value)
                {
                    m_DropDownBarItemLightColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets dark color for background of the DropDownBarItem.
        /// </summary>
        public Color DropDownBarItemDarkColor
        {
            get
            {
                return m_DropDownBarItemDarkColor;
            }
            set
            {
                if (m_DropDownBarItemDarkColor != value)
                {
                    m_DropDownBarItemDarkColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets color for border of the DropDownBarItem.
        /// </summary>
        public Color DropDownBarItemBorderColor
        {
            get
            {
                return m_DropDownBarItemBorderColor;
            }
            set
            {
                if (m_DropDownBarItemBorderColor != value)
                {
                    m_DropDownBarItemBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets light color for background of the checked BarItem.
        /// </summary>
        public Color BarItemCheckLightColor
        {
            get
            {
                return m_BarItemCheckLightColor;
            }
            set
            {
                if (m_BarItemCheckLightColor != value)
                {
                    m_BarItemCheckLightColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets dark color for background of the checked BarItem.
        /// </summary>
        public Color BarItemCheckDarkColor
        {
            get
            {
                return m_BarItemCheckDarkColor;
            }
            set
            {
                if (m_BarItemCheckDarkColor != value)
                {
                    m_BarItemCheckDarkColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets color for border of the checked BarItem.
        /// </summary>
        public Color BarItemCheckBorderColor
        {
            get
            {
                return m_BarItemCheckBorderColor;
            }
            set
            {
                if (m_BarItemCheckBorderColor != value)
                {
                    m_BarItemCheckBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets color for flash of the checked BarItem.
        /// </summary>
        public Color BarItemCheckFlashColor
        {
            get
            {
                return m_BarItemCheckFlashColor;
            }
            set
            {
                if (m_BarItemCheckFlashColor != value)
                {
                    m_BarItemCheckFlashColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets color for flash of the pressed BarItem.
        /// </summary>
        public Color BarItemPressFlashColor
        {
            get
            {
                return m_BarItemPressFlashColor;
            }
            set
            {
                if (m_BarItemPressFlashColor != value)
                {
                    m_BarItemPressFlashColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets color for flash of the selected BarItem.
        /// </summary>
        public Color BarItemSelectFlashColor
        {
            get
            {
                return m_BarItemSelectFlashColor;
            }
            set
            {
                if (m_BarItemSelectFlashColor != value)
                {
                    m_BarItemSelectFlashColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets back color for the TextBoxBarItem.
        /// </summary>
        public Color TextBarItemBackColor
        {
            get
            {
                return m_TextBarItemBackColor;
            }
            set
            {
                if (m_TextBarItemBackColor != value)
                {
                    m_TextBarItemBackColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets color for border of the TextBoxBarItem.
        /// </summary>
        public Color TextBarItemBorderColor
        {
            get
            {
                return m_TextBarItemBorderColor;
            }
            set
            {
                if (m_TextBarItemBorderColor != value)
                {
                    m_TextBarItemBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets color for border of the highlight TextBoxBarItem.
        /// </summary>
        public Color TextBarItemBorderHighlightColor
        {
            get
            {
                return m_TextBarItemBorderHighlightColor;
            }
            set
            {
                if (m_TextBarItemBorderHighlightColor != value)
                {
                    m_TextBarItemBorderHighlightColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets light color for ComboButton of the ComboBoxBarItem.
        /// </summary>
        public Color ComboButtonLightColor
        {
            get
            {
                return m_ComboButtonLightColor;
            }
            set
            {
                if (m_ComboButtonLightColor != value)
                {
                    m_ComboButtonLightColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets dark color for ComboButton of the ComboBoxBarItem.
        /// </summary>
        public Color ComboButtonDarkColor
        {
            get
            {
                return m_ComboButtonDarkColor;
            }
            set
            {
                if (m_ComboButtonDarkColor != value)
                {
                    m_ComboButtonDarkColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets light color for ComboButton of the pressed ComboBoxBarItem.
        /// </summary>
        public Color ComboButtonPressLightColor
        {
            get
            {
                return m_ComboButtonPressLightColor;
            }
            set
            {
                if (m_ComboButtonPressLightColor != value)
                {
                    m_ComboButtonPressLightColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets dark color for ComboButton of the pressed ComboBoxBarItem.
        /// </summary>
        public Color ComboButtonPressDarkColor
        {
            get
            {
                return m_ComboButtonPressDarkColor;
            }
            set
            {
                if (m_ComboButtonPressDarkColor != value)
                {
                    m_ComboButtonPressDarkColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets light color for ComboButton of the highlighted ComboBoxBarItem.
        /// </summary>
        public Color ComboButtonHighlightLightColor
        {
            get
            {
                return m_ComboButtonHighlightLightColor;
            }
            set
            {
                if (m_ComboButtonHighlightLightColor != value)
                {
                    m_ComboButtonHighlightLightColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets dark color for ComboButton of the highlighted ComboBoxBarItem.
        /// </summary>
        public Color ComboButtonHighlightDarkColor
        {
            get
            {
                return m_ComboButtonHighlightDarkColor;
            }
            set
            {
                if (m_ComboButtonHighlightDarkColor != value)
                {
                    m_ComboButtonHighlightDarkColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets border color for ComboButton of the ComboBoxBarItem.
        /// </summary>
        public Color ComboButtonBorder
        {
            get
            {
                return m_ComboButtonBorder;
            }
            set
            {
                if (m_ComboButtonBorder != value)
                {
                    m_ComboButtonBorder = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets border color for ComboButton of the pressed ComboBoxBarItem.
        /// </summary>
        public Color ComboButtonPressBorder
        {
            get
            {
                return m_ComboButtonPressBorder;
            }
            set
            {
                if (m_ComboButtonPressBorder != value)
                {
                    m_ComboButtonPressBorder = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets border color for ComboButton of the highlighted ComboBoxBarItem.
        /// </summary>
        public Color ComboButtonHighlightBorder
        {
            get
            {
                return m_ComboButtonHighlightBorder;
            }
            set
            {
                if (m_ComboButtonHighlightBorder != value)
                {
                    m_ComboButtonHighlightBorder = value;
                }
            }
        }

        #endregion

        #region Button Colors
        /// <summary></summary>
        public Color ButtonPressedTopColor
        {
            get
            {
                return m_ButtonPressedTopColor;
            }
        }
        /// <summary></summary>
        public Color ButtonPressedBottomColor
        {
            get
            {
                return m_ButtonPressedBottomColor;
            }
        }
        /// <summary></summary>
        public Color ButtonSelectedTopColor
        {
            get
            {
                return m_ButtonSelectedTopColor;
            }
        }
        /// <summary></summary>
        public Color ButtonSelectedBottomColor
        {
            get
            {
                return m_ButtonSelectedBottomColor;
            }
        }
        /// <summary></summary>
        public Color ButtonDisabledTopColor
        {
            get
            {
                return m_ButtonDisabledTopColor;
            }
        }
        /// <summary></summary>
        public Color ButtonDisabledBottomColor
        {
            get
            {
                return m_ButtonDisabledBottomColor;
            }
        }
        /// <summary></summary>
        public Color ButtonPressedBorderColor
        {
            get
            {
                return m_ButtonPressedBorderColor;
            }
        }
        /// <summary></summary>
        public Color ButtonSelectedBorderColor
        {
            get
            {
                return m_ButtonSelectedBorderColor;
            }
        }
        /// <summary></summary>
        public Color ButtonDisabledBorderColor
        {
            get
            {
                return m_ButtonDisabledBorderColor;
            }
        }
        /// <summary></summary>
        public Color ButtonDefaultTopColor
        {
            get
            {
                if (m_ButtonDefaultTopColor != Color.Empty)
                {
                    return m_ButtonDefaultTopColor;
                }
                else
                {
                    return m_ButtonDefaultTopColor;
                }
            }
        }
        /// <summary></summary>
        public Color ButtonDefaultBottomColor
        {
            get
            {
                if (m_ButtonDefaultBottomColor != Color.Empty)
                {
                    return m_ButtonDefaultBottomColor;
                }
                else
                {
                    return m_ButtonDefaultBottomColor;
                }
            }
        }
        /// <summary></summary>
        public Color ButtonDefaultBorderColor
        {
            get
            {
                if (m_ButtonDefaultBorderColor != Color.Empty)
                {
                    return m_ButtonDefaultBorderColor;
                }
                else
                {
                    return m_ButtonDefaultBorderColor;
                }
            }
        }
        /// <summary></summary>
        public Color ButtonDefaultInternalBorderColor
        {
            get
            {
                if (m_ButtonDefaultInternalBorderColor != Color.Empty)
                {
                    return m_ButtonDefaultInternalBorderColor;
                }
                else
                {
                    return m_ButtonDefaultInternalBorderColor;
                }
            }
        }
        /// <summary></summary>
        public Color ButtonPressedInternalBorderColor
        {
            get
            {
                if (m_ButtonPressedInternalBorderColor != Color.Empty)
                {
                    return m_ButtonPressedInternalBorderColor;
                }
                else
                {
                    return m_ButtonPressedInternalBorderColor;
                }
            }
        }
        /// <summary></summary>
        public Color ButtonSelectedInternalBorderColor
        {
            get
            {
                if (m_ButtonSelectedInternalBorderColor != Color.Empty)
                {
                    return m_ButtonSelectedInternalBorderColor;
                }
                else
                {
                    return m_ButtonSelectedInternalBorderColor;
                }
            }
        }

        #region Obsolete properties
        [Obsolete]
        public Color BlueButtonDefaultTopColor
        {
            get
            {
                if (m_BlueButtonDefaultTopColor != Color.Empty)
                {
                    return m_BlueButtonDefaultTopColor;
                }
                else
                {
                    m_BlueButtonDefaultTopColor = Color.FromArgb(231, 242, 255);
                    return m_BlueButtonDefaultTopColor;
                }
            }
        }
        [Obsolete]
        public Color BlueButtonDefaultBottomColor
        {
            get
            {
                if (m_BlueButtonDefaultBottomColor != Color.Empty)
                {
                    return m_BlueButtonDefaultBottomColor;
                }
                else
                {
                    m_BlueButtonDefaultBottomColor = Color.FromArgb(179, 209, 252);
                    return m_BlueButtonDefaultBottomColor;
                }
            }
        }
        [Obsolete]
        public Color BlueButtonDefaultBorderColor
        {
            get
            {
                if (m_BlueButtonDefaultBorderColor != Color.Empty)
                {
                    return m_BlueButtonDefaultBorderColor;
                }
                else
                {
                    m_BlueButtonDefaultBorderColor = Color.FromArgb(176, 208, 255);
                    return m_BlueButtonDefaultBorderColor;
                }
            }
        }
        [Obsolete]
        public Color BlueButtonDefaultInternalBorderColor
        {
            get
            {
                if (m_BlueButtonDefaultInternalBorderColor != Color.Empty)
                {
                    return m_BlueButtonDefaultInternalBorderColor;
                }
                else
                {
                    m_BlueButtonDefaultInternalBorderColor = Color.FromArgb(128, Color.FromArgb(250, 251, 255));
                    return m_BlueButtonDefaultInternalBorderColor;
                }
            }
        }
        [Obsolete]
        public Color BlueButtonPressedInternalBorderColor
        {
            get
            {
                if (m_BlueButtonPressedInternalBorderColor != Color.Empty)
                {
                    return m_BlueButtonPressedInternalBorderColor;
                }
                else
                {
                    m_BlueButtonPressedInternalBorderColor = Color.FromArgb(70, Color.FromArgb(176, 132, 92));
                    return m_BlueButtonPressedInternalBorderColor;
                }
            }
        }
        [Obsolete]
        public Color BlueButtonSelectedInternalBorderColor
        {
            get
            {
                if (m_BlueButtonSelectedInternalBorderColor != Color.Empty)
                {
                    return m_BlueButtonSelectedInternalBorderColor;
                }
                else
                {
                    m_BlueButtonSelectedInternalBorderColor = Color.FromArgb(128, Color.FromArgb(250, 251, 255));
                    return m_BlueButtonSelectedInternalBorderColor;
                }
            }
        }
        [Obsolete]
        public Color SilverButtonDefaultTopColor
        {
            get
            {
                if (m_SilverButtonDefaultTopColor != Color.Empty)
                {
                    return m_SilverButtonDefaultTopColor;
                }
                else
                {
                    m_SilverButtonDefaultTopColor = Color.FromArgb(223, 227, 231);
                    return m_SilverButtonDefaultTopColor;
                }
            }
        }
        [Obsolete]
        public Color SilverButtonDefaultBottomColor
        {
            get
            {
                if (m_SilverButtonDefaultBottomColor != Color.Empty)
                {
                    return m_SilverButtonDefaultBottomColor;
                }
                else
                {
                    m_SilverButtonDefaultBottomColor = Color.FromArgb(208, 212, 221);
                    return m_SilverButtonDefaultBottomColor;
                }
            }
        }
        [Obsolete]
        public Color SilverButtonDefaultBorderColor
        {
            get
            {
                if (m_SilverButtonDefaultBorderColor != Color.Empty)
                {
                    return m_SilverButtonDefaultBorderColor;
                }
                else
                {
                    m_SilverButtonDefaultBorderColor = Color.FromArgb(208, 212, 221);
                    return m_SilverButtonDefaultBorderColor;
                }
            }
        }
        [Obsolete]
        public Color SilverButtonDefaultInternalBorderColor
        {
            get
            {
                if (m_SilverButtonDefaultInternalBorderColor != Color.Empty)
                {
                    return m_SilverButtonDefaultInternalBorderColor;
                }
                else
                {
                    m_SilverButtonDefaultInternalBorderColor = Color.FromArgb(128, Color.FromArgb(250, 251, 255));
                    return m_SilverButtonDefaultInternalBorderColor;
                }
            }
        }
        [Obsolete]
        public Color SilverButtonPressedInternalBorderColor
        {
            get
            {
                if (m_SilverButtonPressedInternalBorderColor != Color.Empty)
                {
                    return m_SilverButtonPressedInternalBorderColor;
                }
                else
                {
                    m_SilverButtonPressedInternalBorderColor = Color.FromArgb(70, Color.FromArgb(176, 132, 92));
                    return m_SilverButtonPressedInternalBorderColor;
                }
            }
        }
        [Obsolete]
        public Color SilverButtonSelectedInternalBorderColor
        {
            get
            {
                if (m_SilverButtonSelectedInternalBorderColor != Color.Empty)
                {
                    return m_SilverButtonSelectedInternalBorderColor;
                }
                else
                {
                    m_SilverButtonSelectedInternalBorderColor = Color.FromArgb(128, Color.FromArgb(250, 251, 255));
                    return m_SilverButtonSelectedInternalBorderColor;
                }
            }
        }
        [Obsolete]
        public Color BlackButtonDefaultTopColor
        {
            get
            {
                if (m_BlackButtonDefaultTopColor != Color.Empty)
                {
                    return m_BlackButtonDefaultTopColor;
                }
                else
                {
                    m_BlackButtonDefaultTopColor = Color.FromArgb(146, 146, 146);
                    return m_BlackButtonDefaultTopColor;
                }
            }
        }
        [Obsolete]
        public Color BlackButtonDefaultBottomColor
        {
            get
            {
                if (m_BlackButtonDefaultBottomColor != Color.Empty)
                {
                    return m_BlackButtonDefaultBottomColor;
                }
                else
                {
                    m_BlackButtonDefaultBottomColor = Color.FromArgb(83, 83, 83);
                    return m_BlackButtonDefaultBottomColor;
                }
            }
        }
        [Obsolete]
        public Color BlackButtonDefaultBorderColor
        {
            get
            {
                if (m_BlackButtonDefaultBorderColor != Color.Empty)
                {
                    return m_BlackButtonDefaultBorderColor;
                }
                else
                {
                    m_BlackButtonDefaultBorderColor = Color.FromArgb(153, 153, 153);
                    return m_BlackButtonDefaultBorderColor;
                }
            }
        }
        [Obsolete]
        public Color BlackButtonDefaultInternalBorderColor
        {
            get
            {
                if (m_BlackButtonDefaultInternalBorderColor != Color.Empty)
                {
                    return m_BlackButtonDefaultInternalBorderColor;
                }
                else
                {
                    m_BlackButtonDefaultInternalBorderColor = Color.FromArgb(128, Color.FromArgb(250, 251, 255));
                    return m_BlackButtonDefaultInternalBorderColor;
                }
            }
        }
        [Obsolete]
        public Color BlackButtonPressedInternalBorderColor
        {
            get
            {
                if (m_BlackButtonPressedInternalBorderColor != Color.Empty)
                {
                    return m_BlackButtonPressedInternalBorderColor;
                }
                else
                {
                    m_BlackButtonPressedInternalBorderColor = Color.FromArgb(70, Color.FromArgb(176, 132, 92));
                    return m_BlackButtonPressedInternalBorderColor;
                }
            }
        }
        [Obsolete]
        public Color BlackButtonSelectedInternalBorderColor
        {
            get
            {
                if (m_BlackButtonSelectedInternalBorderColor != Color.Empty)
                {
                    return m_BlackButtonSelectedInternalBorderColor;
                }
                else
                {
                    m_BlackButtonSelectedInternalBorderColor = Color.FromArgb(128, Color.FromArgb(250, 251, 255));
                    return m_BlackButtonSelectedInternalBorderColor;
                }
            }
        }
        #endregion

        #endregion

        #region TabControlAdv colors

        public Color TabDefaultBorderColor
        {
            get { return m_TabDefaultBorderColor; }
        }

        public Color TabHotLightBottomBorderLineColor
        {
            get { return m_TabHotLightBottomBorderLineColor; }
        }

        public Color TabHotLightGradientTopBeginColor
        {
            get { return m_TabHotLightGradientTopBeginColor; }
        }

        public Color TabHotLightGradientTopEndColor
        {
            get { return m_TabHotLightGradientTopEndColor; }
        }

        public Color TabHotLightGradientBottomBeginColor
        {
            get { return m_TabHotLightGradientBottomBeginColor; }
        }

        public Color TabHotLightGradientBottomEndColor
        {
            get { return m_TabHotLightGradientBottomEndColor; }
        }

        public Color TabHotLightGradientCircleColor
        {
            get { return m_TabHotLightGradientCircleColor; }
        }

        public Color TabSelectedGradientTopColor
        {
            get { return m_TabSelectedGradientTopColor; }
        }

        public Color TabSelectedGradientBottomColor
        {
            get { return m_TabSelectedGradientBottomColor; }
        }

        public Color TabSelectedInnerBorderColor
        {
            get { return m_TabSelectedInnerBorderColor; }
        }

        public Color TabHighlightInnerBorderColor
        {
            get { return m_TabHighlightInnerBorderColor; }
        }

        public Color TabSelectedHotLightBorderColor
        {
            get { return m_TabSelectedHotLightBorderColor; }
        }

        public Color TabSelectedHotLightInnerBorderColor
        {
            get { return m_TabSelectedHotLightInnerBorderColor; }
        }

        public Color TabForeColor
        {
            get { return m_TabForeColor; }
        }

        public Color ActiveTabForeColor
        {
            get { return m_ActiveTabForeColor; }
        }

        public Color TabBackgroundColor
        {
            get { return m_TabBackgroundColor; }
        }

        public Color TabScrollArrowColor
        {
            get { return m_TabScrollArrowColor; }
        }

        #endregion

        #region DockTabControl colors

        public Color DockTabForeColor
        {
            get { return m_DockTabForeColor; }
        }

        public Color DockTabBackgroundColor
        {
            get { return m_DockTabBackgroundColor; }
        }

        #endregion

        #region Form colors

        public Color ActiveFormBorderColor
        {
            get { return m_ActiveFormBorderColor; }
        }
        public Color SelectedNodeBackground
        {
            get { return m_SelectedNodeBackground; }
        }
        public Color TreeviewBackColor
        {
            get { return m_TreeviewBackColor; }
        }
        public Color TreeNodeArrowColor
        {
            get { return m_TreeNodeArrowColor; }
        }
        public Color TreeViewFontColor
        {
            get { return m_TreeViewFontColor; }
        }
        public Color InactiveFormBorderColor
        {
            get { return m_InactiveFormBorderColor; }
        }
        public Color ActiveTextBoxBorderColor
        {
            get { return m_ActiveTextBoxBorderColor; }
        }
        public Color InactiveTextBoxBorderColor
        {
            get { return m_InactiveTextBoxBorderColor; }
        }
        public Color ActiveTextBoxBackColor
        {
            get { return m_ActiveTextBoxBackColor; }
        }
        public Color InactiveTextBoxBackColor
        {
            get { return m_InactiveTextBoxBackColor; }
        }
        public Color FormTextColor
        {
            get { return m_FormTextColor; }
        }
        public Color ActiveTitleGradientBegin
        {
            get { return m_ActiveTitleGradientBegin; }
        }
        public Color ActiveTitleGradientEnd
        {
            get { return m_ActiveTitleGradientEnd; }
        }
        public Color InactiveTitleGradientBegin
        {
            get { return m_InactiveTitleGradientBegin; }
        }
        public Color InactiveTitleGradientEnd
        {
            get { return m_InactiveTitleGradientEnd; }
        }
        public Color SystemButtonSelectedGradientBegin
        {
            get { return m_SystemButtonSelectedGradientBegin; }
        }
        public Color SystemButtonSelectedGradientEnd
        {
            get { return m_SystemButtonSelectedGradientEnd; }
        }
        public Color SystemButtonPressedGradientBegin
        {
            get { return m_SystemButtonPressedGradientBegin; }
        }
        public Color SystemButtonPressedGradientEnd
        {
            get { return m_SystemButtonPressedGradientEnd; }
        }

        public Color SystemButtonBorderSelected
        {
            get { return m_SystemButtonBorderSelected; }
        }
        public Color SystemButtonBorderPressed
        {
            get { return m_SystemButtonBorderPressed; }
        }
        public Color FormBackground
        {
            get { return m_FormBackground; }
        }
        #endregion

        #region UpDown colors
        /// <summary>
        /// Gets the arrow start color for UpDownButtons.
        /// </summary>
        public Color UpDownArrowStartColor
        {
            get
            {
                return m_UpDownArrowStartColor;
            }
        }
        /// <summary>
        /// Gets the arrow end color for UpDownButtons.
        /// </summary>
        public Color UpDownArrowEndColor
        {
            get
            {
                return m_UpDownArrowEndColor;
            }
        }
        /// <summary>
        /// Gets the border color for UpDownButtons in normal state.
        /// </summary>
        public Color UpDownBorderNormalColor
        {
            get
            {
                return m_UpDownBorderNormalColor;
            }
        }
        /// <summary>
        /// Gets the background color for UpDownButtons in normal state. 
        /// </summary>
        public Color UpDownBackgroundNormalColor
        {
            get
            {
                return m_UpDownBackgroundNormalColor;
            }
        }
        /// <summary>
        /// Gets the background start color for UpDownButtons in normal state.
        /// </summary>
        public Color UpDownBackgroundNormalStartColor
        {
            get
            {
                return m_UpDownBackgroundNormalStartColor;
            }
        }
        /// <summary>
        /// Gets the background end color for UpDownButtons in normal state.
        /// </summary>
        public Color UpDownBackgroundNormalEndColor
        {
            get
            {
                return m_UpDownBackgroundNormalEndColor;
            }
        }
        /// <summary>
        /// Gets the border color for UpDownButtons in hot state.
        /// </summary>
        public Color UpDownBorderHotColor
        {
            get
            {
                return m_UpDownBorderHotColor;
            }
        }
        /// <summary>
        /// Gets the inner border start color for UpDownButtons in hot state.
        /// </summary>
        public Color UpDownInnerBorderHotStartColor
        {
            get
            {
                return m_UpDownInnerBorderHotStartColor;
            }
        }
        /// <summary>
        /// Gets the inner border end color for UpDownButtons in hot state.
        /// </summary>
        public Color UpDownInnerBorderHotEndColor
        {
            get
            {
                return m_UpDownInnerBorderHotEndColor;
            }
        }
        /// <summary>
        /// Gets the border color for UpDownButtons in pressed state.
        /// </summary>
        public Color UpDownBorderPressedColor
        {
            get
            {
                return m_UpDownBorderPressedColor;
            }
        }
        /// <summary>
        /// Gets the inner border start color for UpDownButtons in pressed state.
        /// </summary>
        public Color UpDownInnerBorderPressedStartColor
        {
            get
            {
                return m_UpDownInnerBorderPressedStartColor;
            }
        }
        /// <summary>
        /// Gets the inner border end color for UpDownButtons in pressed state.
        /// </summary>
        public Color UpDownInnerBorderPressedEndColor
        {
            get
            {
                return m_UpDownInnerBorderPressedEndColor;
            }
        }
        /// <summary>
        /// Gets the background start color for UpDownButtons in disabled state.
        /// </summary>
        public Color UpDownBackgroundDisabledStartColor
        {
            get
            {
                return m_UpDownBackgroundDisabledStartColor;
            }
        }
        /// <summary>
        /// Gets the background end color for UpDownButtons in disabled state.
        /// </summary>
        public Color UpDownBackgroundDisabledEndColor
        {
            get
            {
                return m_UpDownBackgroundDisabledEndColor;
            }
        }
        /// <summary>
        /// Gets the border color for UpDownButtons in disabled state.
        /// </summary>
        public Color UpDownBorderDisabledColor
        {
            get
            {
                return m_UpDownBorderDisabledColor;
            }
        }
        /// <summary>
        /// Gets the background top start color for UpDownButtons in hot state.
        /// </summary>
        public Color UpDownBackgroundHotTopStartColor
        {
            get
            {
                return m_UpDownBackgroundHotTopStartColor;
            }
        }
        /// <summary>
        /// Gets the background top end color for UpDownButtons in hot state.
        /// </summary>
        public Color UpDownBackgroundHotTopEndColor
        {
            get
            {
                return m_UpDownBackgroundHotTopEndColor;
            }
        }
        /// <summary>
        /// Gets the background bottom start color for UpDownButtons in hot state.
        /// </summary>
        public Color UpDownBackgroundHotBottomStartColor
        {
            get
            {
                return m_UpDownBackgroundHotBottomStartColor;
            }
        }
        /// <summary>
        /// Gets the background bottom end color for UpDownButtons in hot state.
        /// </summary>
        public Color UpDownBackgroundHotBottomEndColor
        {
            get
            {
                return m_UpDownBackgroundHotBottomEndColor;
            }
        }
        /// <summary>
        /// Gets the background top start color for UpDownButtons in pressed state.
        /// </summary>
        public Color UpDownBackgroundPressedTopStartColor
        {
            get
            {
                return m_UpDownBackgroundPressedTopStartColor;
            }
        }
        /// <summary>
        /// Gets the background top end color for UpDownButtons in pressed state.
        /// </summary>
        public Color UpDownBackgroundPressedTopEndColor
        {
            get
            {
                return m_UpDownBackgroundPressedTopEndColor;
            }
        }
        /// <summary>
        /// Gets the background bottom start color for UpDownButtons in pressed state.
        /// </summary>
        public Color UpDownBackgroundPressedBottomStartColor
        {
            get
            {
                return m_UpDownBackgroundPressedBottomStartColor;
            }
        }
        /// <summary>
        /// Gets the background bottom end color for UpDownButtons in pressed state.
        /// </summary>
        public Color UpDownBackgroundPressedBottomEndColor
        {
            get
            {
                return m_UpDownBackgroundPressedBottomEndColor;
            }
        }
        #endregion

        #region ComboBoxAdv colors
        /// <summary>
        /// Gets or sets the back color for <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvNormalBackColor
        {
            get
            {
                return m_ComboBoxAdvNormalBackColor;
            }
            set
            {
                if (m_ComboBoxAdvNormalBackColor != value)
                {
                    m_ComboBoxAdvNormalBackColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the back color for the selected <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvHotBackColor
        {
            get
            {
                return m_ComboBoxAdvHotBackColor;
            }
            set
            {
                if (m_ComboBoxAdvHotBackColor != value)
                {
                    m_ComboBoxAdvHotBackColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the border color for <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvNormalBorderColor
        {
            get
            {
                return m_ComboBoxAdvNormalBorderColor;
            }
            set
            {
                if (m_ComboBoxAdvNormalBorderColor != value)
                {
                    m_ComboBoxAdvNormalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the border color for the selected <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvHotBorderColor
        {
            get
            {
                return m_ComboBoxAdvHotBorderColor;
            }
            set
            {
                if (m_ComboBoxAdvHotBorderColor != value)
                {
                    m_ComboBoxAdvHotBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the border color for the pushed <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvPushedBorderColor
        {
            get
            {
                return m_ComboBoxAdvPushedBorderColor;
            }
            set
            {
                if (m_ComboBoxAdvPushedBorderColor != value)
                {
                    m_ComboBoxAdvPushedBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for upper line of the dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvButtonUpperLineColor
        {
            get
            {
                return m_ComboBoxAdvButtonUpperLineColor;
            }
            set
            {
                if (m_ComboBoxAdvButtonUpperLineColor != value)
                {
                    m_ComboBoxAdvButtonUpperLineColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for the arrow of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvArrowColor
        {
            get
            {
                return m_ComboBoxAdvArrowColor;
            }
            set
            {
                if (m_ComboBoxAdvArrowColor != value)
                {
                    m_ComboBoxAdvArrowColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for lower line of the arrow of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvLowerArrowLineColor
        {
            get
            {
                return m_ComboBoxAdvLowerArrowLineColor;
            }
            set
            {
                if (m_ComboBoxAdvLowerArrowLineColor != value)
                {
                    m_ComboBoxAdvLowerArrowLineColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the hot background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvHotBackgroundButtonColor1
        {
            get
            {
                return m_ComboBoxAdvHotBackgroundButtonColor1;
            }
            set
            {
                if (m_ComboBoxAdvHotBackgroundButtonColor1 != value)
                {
                    m_ComboBoxAdvHotBackgroundButtonColor1 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the hot background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvHotBackgroundButtonColor2
        {
            get
            {
                return m_ComboBoxAdvHotBackgroundButtonColor2;
            }
            set
            {
                if (m_ComboBoxAdvHotBackgroundButtonColor2 != value)
                {
                    m_ComboBoxAdvHotBackgroundButtonColor2 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the hot background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvHotBackgroundButtonColor3
        {
            get
            {
                return m_ComboBoxAdvHotBackgroundButtonColor3;
            }
            set
            {
                if (m_ComboBoxAdvHotBackgroundButtonColor3 != value)
                {
                    m_ComboBoxAdvHotBackgroundButtonColor3 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the hot background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvHotBackgroundButtonColor4
        {
            get
            {
                return m_ComboBoxAdvHotBackgroundButtonColor4;
            }
            set
            {
                if (m_ComboBoxAdvHotBackgroundButtonColor4 != value)
                {
                    m_ComboBoxAdvHotBackgroundButtonColor4 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvNormalBackgroundButtonColor1
        {
            get
            {
                return m_ComboBoxAdvNormalBackgroundButtonColor1;
            }
            set
            {
                if (m_ComboBoxAdvNormalBackgroundButtonColor1 != value)
                {
                    m_ComboBoxAdvNormalBackgroundButtonColor1 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvNormalBackgroundButtonColor2
        {
            get
            {
                return m_ComboBoxAdvNormalBackgroundButtonColor2;
            }
            set
            {
                if (m_ComboBoxAdvNormalBackgroundButtonColor2 != value)
                {
                    m_ComboBoxAdvNormalBackgroundButtonColor2 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvNormalBackgroundButtonColor3
        {
            get
            {
                return m_ComboBoxAdvNormalBackgroundButtonColor3;
            }
            set
            {
                if (m_ComboBoxAdvNormalBackgroundButtonColor3 != value)
                {
                    m_ComboBoxAdvNormalBackgroundButtonColor3 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvNormalBackgroundButtonColor4
        {
            get
            {
                return m_ComboBoxAdvNormalBackgroundButtonColor4;
            }
            set
            {
                if (m_ComboBoxAdvNormalBackgroundButtonColor4 != value)
                {
                    m_ComboBoxAdvNormalBackgroundButtonColor4 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvPushedBackgroundButtonColor1
        {
            get
            {
                return m_ComboBoxAdvPushedBackgroundButtonColor1;
            }
            set
            {
                if (m_ComboBoxAdvPushedBackgroundButtonColor1 != value)
                {
                    m_ComboBoxAdvPushedBackgroundButtonColor1 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvPushedBackgroundButtonColor2
        {
            get
            {
                return m_ComboBoxAdvPushedBackgroundButtonColor2;
            }
            set
            {
                if (m_ComboBoxAdvPushedBackgroundButtonColor2 != value)
                {
                    m_ComboBoxAdvPushedBackgroundButtonColor2 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvPushedBackgroundButtonColor3
        {
            get
            {
                return m_ComboBoxAdvPushedBackgroundButtonColor3;
            }
            set
            {
                if (m_ComboBoxAdvPushedBackgroundButtonColor3 != value)
                {
                    m_ComboBoxAdvPushedBackgroundButtonColor3 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvPushedBackgroundButtonColor4
        {
            get
            {
                return m_ComboBoxAdvPushedBackgroundButtonColor4;
            }
            set
            {
                if (m_ComboBoxAdvPushedBackgroundButtonColor4 != value)
                {
                    m_ComboBoxAdvPushedBackgroundButtonColor4 = value;
                }
            }
        }

        #endregion

        #region CheckBoxAdv colors
        /// <summary>
        /// Used in drawing of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvNormalBackColor
        {
            get
            {
                return m_CheckBoxAdvNormalBackColor;
            }
            set
            {
                if (m_CheckBoxAdvNormalBackColor != value)
                {
                    m_CheckBoxAdvNormalBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvSelectedBackColor
        {
            get
            {
                return m_CheckBoxAdvSelectedBackColor;
            }
            set
            {
                if (m_CheckBoxAdvSelectedBackColor != value)
                {
                    m_CheckBoxAdvSelectedBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvPushedBackColor
        {
            get
            {
                return m_CheckBoxAdvPushedBackColor;
            }
            set
            {
                if (m_CheckBoxAdvPushedBackColor != value)
                {
                    m_CheckBoxAdvPushedBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvNormalBorderColor
        {
            get
            {
                return m_CheckBoxAdvNormalBorderColor;
            }
            set
            {
                if (m_CheckBoxAdvNormalBorderColor != value)
                {
                    m_CheckBoxAdvNormalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the selected border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvSelectedBorderColor
        {
            get
            {
                return m_CheckBoxAdvSelectedBorderColor;
            }
            set
            {
                if (m_CheckBoxAdvSelectedBorderColor != value)
                {
                    m_CheckBoxAdvSelectedBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvPushedBorderColor
        {
            get
            {
                return m_CheckBoxAdvPushedBorderColor;
            }
            set
            {
                if (m_CheckBoxAdvPushedBorderColor != value)
                {
                    m_CheckBoxAdvPushedBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal internal border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvNormalInternalBorderColor
        {
            get
            {
                return m_CheckBoxAdvNormalInternalBorderColor;
            }
            set
            {
                if (m_CheckBoxAdvNormalInternalBorderColor != value)
                {
                    m_CheckBoxAdvNormalInternalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the selected internal border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvSelectedInternalBorderColor
        {
            get
            {
                return m_CheckBoxAdvSelectedInternalBorderColor;
            }
            set
            {
                if (m_CheckBoxAdvSelectedInternalBorderColor != value)
                {
                    m_CheckBoxAdvSelectedInternalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed internal border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvPushedInternalBorderColor
        {
            get
            {
                return m_CheckBoxAdvPushedInternalBorderColor;
            }
            set
            {
                if (m_CheckBoxAdvPushedInternalBorderColor != value)
                {
                    m_CheckBoxAdvPushedInternalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal internal rectangle border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvNormalInternalRectangleBorderColor
        {
            get
            {
                return m_CheckBoxAdvNormalInternalRectangleBorderColor;
            }
            set
            {
                if (m_CheckBoxAdvNormalInternalRectangleBorderColor != value)
                {
                    m_CheckBoxAdvNormalInternalRectangleBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the selected internal rectangle border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvSelectedInternalRectangleBorderColor
        {
            get
            {
                return m_CheckBoxAdvSelectedInternalRectangleBorderColor;
            }
            set
            {
                if (m_CheckBoxAdvSelectedInternalRectangleBorderColor != value)
                {
                    m_CheckBoxAdvSelectedInternalRectangleBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed internal rectangle border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvPushedInternalRectangleBorderColor
        {
            get
            {
                return m_CheckBoxAdvPushedInternalRectangleBorderColor;
            }
            set
            {
                if (m_CheckBoxAdvPushedInternalRectangleBorderColor != value)
                {
                    m_CheckBoxAdvPushedInternalRectangleBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal internal rectangle of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvNormalInternalRectangleColor
        {
            get
            {
                return m_CheckBoxAdvNormalInternalRectangleColor;
            }
            set
            {
                if (m_CheckBoxAdvNormalInternalRectangleColor != value)
                {
                    m_CheckBoxAdvNormalInternalRectangleColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the selected internal rectangle of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvSelectedInternalRectangleColor
        {
            get
            {
                return m_CheckBoxAdvSelectedInternalRectangleColor;
            }
            set
            {
                if (m_CheckBoxAdvSelectedInternalRectangleColor != value)
                {
                    m_CheckBoxAdvSelectedInternalRectangleColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed internal rectangle of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvPushedInternalRectangleColor
        {
            get
            {
                return m_CheckBoxAdvPushedInternalRectangleColor;
            }
            set
            {
                if (m_CheckBoxAdvPushedInternalRectangleColor != value)
                {
                    m_CheckBoxAdvPushedInternalRectangleColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal tick of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvNormalTickColor
        {
            get
            {
                return m_CheckBoxAdvNormalTickColor;
            }
            set
            {
                if (m_CheckBoxAdvNormalTickColor != value)
                {
                    m_CheckBoxAdvNormalTickColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the selected tick of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvSelectedTickColor
        {
            get
            {
                return m_CheckBoxAdvSelectedTickColor;
            }
            set
            {
                if (m_CheckBoxAdvSelectedTickColor != value)
                {
                    m_CheckBoxAdvSelectedTickColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed tick of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvPushedTickColor
        {
            get
            {
                return m_CheckBoxAdvPushedTickColor;
            }
            set
            {
                if (m_CheckBoxAdvPushedTickColor != value)
                {
                    m_CheckBoxAdvPushedTickColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the disabled tick of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvDisabledTickColor
        {
            get
            {
                return m_CheckBoxAdvDisabledTickColor;
            }
            set
            {
                if (m_CheckBoxAdvDisabledTickColor != value)
                {
                    m_CheckBoxAdvDisabledTickColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the indeterminate rectangle of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvIndeterminateRectangleColor
        {
            get
            {
                return m_CheckBoxAdvIndeterminateRectangleColor;
            }
            set
            {
                if (m_CheckBoxAdvIndeterminateRectangleColor != value)
                {
                    m_CheckBoxAdvIndeterminateRectangleColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the disabled back color <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvDisabledBackColor
        {
            get
            {
                return m_CheckBoxAdvDisabledBackColor;
            }
            set
            {
                if (m_CheckBoxAdvDisabledBackColor != value)
                {
                    m_CheckBoxAdvDisabledBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the disabled border <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvDisabledBorderColor
        {
            get
            {
                return m_CheckBoxAdvDisabledBorderColor;
            }
            set
            {
                if (m_CheckBoxAdvDisabledBorderColor != value)
                {
                    m_CheckBoxAdvDisabledBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the disabled internal border <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvDisabledInternalBorderColor
        {
            get
            {
                return m_CheckBoxAdvDisabledInternalBorderColor;
            }
            set
            {
                if (m_CheckBoxAdvDisabledInternalBorderColor != value)
                {
                    m_CheckBoxAdvDisabledInternalBorderColor = value;
                }
            }
        }

        #endregion

        #region RadioButtonAdv colors
        /// <summary>
        /// Used in drawing of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvNormalBackColor
        {
            get
            {
                return m_RadioButtonAdvNormalBackColor;
            }
            set
            {
                if (m_RadioButtonAdvNormalBackColor != value)
                {
                    m_RadioButtonAdvNormalBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the border of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvNormalBorderColor
        {
            get
            {
                return m_RadioButtonAdvNormalBorderColor;
            }
            set
            {
                if (m_RadioButtonAdvNormalBorderColor != value)
                {
                    m_RadioButtonAdvNormalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the internal border of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvNormalInternalBorderColor
        {
            get
            {
                return m_RadioButtonAdvNormalInternalBorderColor;
            }
            set
            {
                if (m_RadioButtonAdvNormalInternalBorderColor != value)
                {
                    m_RadioButtonAdvNormalInternalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvSelectedBackColor
        {
            get
            {
                return m_RadioButtonAdvSelectedBackColor;
            }
            set
            {
                if (m_RadioButtonAdvSelectedBackColor != value)
                {
                    m_RadioButtonAdvSelectedBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the border of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvSelectedBorderColor
        {
            get
            {
                return m_RadioButtonAdvSelectedBorderColor;
            }
            set
            {
                if (m_RadioButtonAdvSelectedBorderColor != value)
                {
                    m_RadioButtonAdvSelectedBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the internal border of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvSelectedInternalBorderColor
        {
            get
            {
                return m_RadioButtonAdvSelectedInternalBorderColor;
            }
            set
            {
                if (m_RadioButtonAdvSelectedInternalBorderColor != value)
                {
                    m_RadioButtonAdvSelectedInternalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvPushedBackColor
        {
            get
            {
                return m_RadioButtonAdvPushedBackColor;
            }
            set
            {
                if (m_RadioButtonAdvPushedBackColor != value)
                {
                    m_RadioButtonAdvPushedBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the border of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvPushedBorderColor
        {
            get
            {
                return m_RadioButtonAdvPushedBorderColor;
            }
            set
            {
                if (m_RadioButtonAdvPushedBorderColor != value)
                {
                    m_RadioButtonAdvPushedBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the internal border of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvPushedInternalBorderColor
        {
            get
            {
                return m_RadioButtonAdvPushedInternalBorderColor;
            }
            set
            {
                if (m_RadioButtonAdvPushedInternalBorderColor != value)
                {
                    m_RadioButtonAdvPushedInternalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the border of check mark of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvCheckMarkBorderColor
        {
            get
            {
                return m_RadioButtonAdvCheckMarkBorderColor;
            }
            set
            {
                if (m_RadioButtonAdvCheckMarkBorderColor != value)
                {
                    m_RadioButtonAdvCheckMarkBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of check mark of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvCheckMarkNormalBottomColor
        {
            get
            {
                return m_RadioButtonAdvCheckMarkNormalBottomColor;
            }
            set
            {
                if (m_RadioButtonAdvCheckMarkNormalBottomColor != value)
                {
                    m_RadioButtonAdvCheckMarkNormalBottomColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of check mark of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvCheckMarkSelectedBottomColor
        {
            get
            {
                return m_RadioButtonAdvCheckMarkSelectedBottomColor;
            }
            set
            {
                if (m_RadioButtonAdvCheckMarkSelectedBottomColor != value)
                {
                    m_RadioButtonAdvCheckMarkSelectedBottomColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of check mark of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvCheckMarkPushedBottomColor
        {
            get
            {
                return m_RadioButtonAdvCheckMarkPushedBottomColor;
            }
            set
            {
                if (m_RadioButtonAdvCheckMarkPushedBottomColor != value)
                {
                    m_RadioButtonAdvCheckMarkPushedBottomColor = value;
                }
            }
        }

        #endregion

        #region TabBarSplitterControl colors
        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterBackColor
        {
            get
            {
                return m_TabBarSplitterBackColor;
            }
            set
            {
                if (m_TabBarSplitterBackColor != value)
                {
                    m_TabBarSplitterBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterBorderColor
        {
            get
            {
                return m_TabBarSplitterBorderColor;
            }
            set
            {
                if (m_TabBarSplitterBorderColor != value)
                {
                    m_TabBarSplitterBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterTextColor
        {
            get
            {
                return m_TabBarSplitterTextColor;
            }
            set
            {
                if (m_TabBarSplitterTextColor != value)
                {
                    m_TabBarSplitterTextColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterTabStartColor
        {
            get
            {
                return m_TabBarSplitterTabStartColor;
            }
            set
            {
                if (m_TabBarSplitterTabStartColor != value)
                {
                    m_TabBarSplitterTabStartColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterTabEndColor
        {
            get
            {
                return m_TabBarSplitterTabEndColor;
            }
            set
            {
                if (m_TabBarSplitterTabEndColor != value)
                {
                    m_TabBarSplitterTabEndColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterTabBarStartColor
        {
            get
            {
                return m_TabBarSplitterTabBarStartColor;
            }
            set
            {
                if (m_TabBarSplitterTabBarStartColor != value)
                {
                    m_TabBarSplitterTabBarStartColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterTabBarEndColor
        {
            get
            {
                return m_TabBarSplitterTabBarEndColor;
            }
            set
            {
                if (m_TabBarSplitterTabBarEndColor != value)
                {
                    m_TabBarSplitterTabBarEndColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterButtonHoveredStartColor
        {
            get
            {
                return m_TabBarSplitterButtonHoveredStartColor;
            }
            set
            {
                if (m_TabBarSplitterButtonHoveredStartColor != value)
                {
                    m_TabBarSplitterButtonHoveredStartColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterButtonHoveredEndColor
        {
            get
            {
                return m_TabBarSplitterButtonHoveredEndColor;
            }
            set
            {
                if (m_TabBarSplitterButtonHoveredEndColor != value)
                {
                    m_TabBarSplitterButtonHoveredEndColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterButtonPushedStartColor
        {
            get
            {
                return m_TabBarSplitterButtonPushedStartColor;
            }
            set
            {
                if (m_TabBarSplitterButtonPushedStartColor != value)
                {
                    m_TabBarSplitterButtonPushedStartColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterButtonPushedEndColor
        {
            get
            {
                return m_TabBarSplitterButtonPushedEndColor;
            }
            set
            {
                if (m_TabBarSplitterButtonPushedEndColor != value)
                {
                    m_TabBarSplitterButtonPushedEndColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterSizeGripperColor
        {
            get
            {
                return m_TabBarSplitterSizeGripperColor;
            }
            set
            {
                if (m_TabBarSplitterSizeGripperColor != value)
                {
                    m_TabBarSplitterSizeGripperColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterSizeGripperLightColor
        {
            get
            {
                return m_TabBarSplitterSizeGripperLightColor;
            }
            set
            {
                if (m_TabBarSplitterSizeGripperLightColor != value)
                {
                    m_TabBarSplitterSizeGripperLightColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="TabBarSplitterControl">.
        /// </summary>
        public Color TabBarSplitterSizeGripperDarkColor
        {
            get
            {
                return m_TabBarSplitterSizeGripperDarkColor;
            }
            set
            {
                if (m_TabBarSplitterSizeGripperDarkColor != value)
                {
                    m_TabBarSplitterSizeGripperDarkColor = value;
                }
            }
        }

        #endregion

        #region XPTaskBar colors
        /// <summary>
        /// Used in drawing of <see cref="XPTaskBar">.
        /// </summary>
        public Color XPTaskBarBorderColor
        {
            get
            {
                return m_XPTaskBarBorderColor;
            }
            set
            {
                if (m_XPTaskBarBorderColor != value)
                {
                    m_XPTaskBarBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="XPTaskBar">.
        /// </summary>
        public Color XPTaskBarBoxBackColor
        {
            get
            {
                return m_XPTaskBarBoxBackColor;
            }
            set
            {
                if (m_XPTaskBarBoxBackColor != value)
                {
                    m_XPTaskBarBoxBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="XPTaskBar">.
        /// </summary>
        public Color XPTaskBarBoxForeColor
        {
            get
            {
                return m_XPTaskBarBoxForeColor;
            }
            set
            {
                if (m_XPTaskBarBoxForeColor != value)
                {
                    m_XPTaskBarBoxForeColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="XPTaskBar">.
        /// </summary>
        public Color XPTaskBarBoxHeaderUpperLineColor
        {
            get
            {
                return m_XPTaskBarBoxHeaderUpperLineColor;
            }
            set
            {
                if (m_XPTaskBarBoxHeaderUpperLineColor != value)
                {
                    m_XPTaskBarBoxHeaderUpperLineColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="XPTaskBar">.
        /// </summary>
        public Color XPTaskBarBoxHeaderLowerLineColor
        {
            get
            {
                return m_XPTaskBarBoxHeaderLowerLineColor;
            }
            set
            {
                if (m_XPTaskBarBoxHeaderLowerLineColor != value)
                {
                    m_XPTaskBarBoxHeaderLowerLineColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="XPTaskBar">.
        /// </summary>
        public Color XPTaskBarBoxArrowColor
        {
            get
            {
                return m_XPTaskBarBoxArrowColor;
            }
            set
            {
                if (m_XPTaskBarBoxArrowColor != value)
                {
                    m_XPTaskBarBoxArrowColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="XPTaskBar">.
        /// </summary>
        public Color XPTaskBarBoxActiveHighlightedItemColor
        {
            get
            {
                return m_XPTaskBarBoxActiveHighlightedItemColor;
            }
            set
            {
                if (m_XPTaskBarBoxActiveHighlightedItemColor != value)
                {
                    m_XPTaskBarBoxActiveHighlightedItemColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="XPTaskBar">.
        /// </summary>
        public Color XPTaskBarBoxInactiveHighlightedItemColor
        {
            get
            {
                return m_XPTaskBarBoxInactiveHighlightedItemColor;
            }
            set
            {
                if (m_XPTaskBarBoxInactiveHighlightedItemColor != value)
                {
                    m_XPTaskBarBoxInactiveHighlightedItemColor = value;
                }
            }
        }
        #endregion

        #region ColorUIAdv properties

        /// <summary>
        /// Used in drawing of <see cref="ColorUIAdv">.
        /// </summary>
        public Color ColorUIAdvBackColor
        {
            get
            {
                return m_ColorUIAdvBackColor;
            }
            set
            {
                if (m_ColorUIAdvBackColor != value)
                {
                    m_ColorUIAdvBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="ColorUIAdv">.
        /// </summary>
        public Color ColorUIAdvTextColor
        {
            get
            {
                return m_ColorUIAdvTextColor;
            }
            set
            {
                if (m_ColorUIAdvTextColor != value)
                {
                    m_ColorUIAdvTextColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="ColorUIAdv">.
        /// </summary>
        public Color ColorUIAdvItemBorderColor
        {
            get
            {
                return m_ColorUIAdvItemBorderColor;
            }
            set
            {
                if (m_ColorUIAdvItemBorderColor != value)
                {
                    m_ColorUIAdvItemBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="ColorUIAdv">.
        /// </summary>
        public Color ColorUIAdvHighlightedBorderColor
        {
            get
            {
                return m_ColorUIAdvHighlightedBorderColor;
            }
            set
            {
                if (m_ColorUIAdvHighlightedBorderColor != value)
                {
                    m_ColorUIAdvHighlightedBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="ColorUIAdv">.
        /// </summary>
        public Color ColorUIAdvSelectedBorderColor
        {
            get
            {
                return m_ColorUIAdvSelectedBorderColor;
            }
            set
            {
                if (m_ColorUIAdvSelectedBorderColor != value)
                {
                    m_ColorUIAdvSelectedBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="ColorUIAdv">.
        /// </summary>
        public Color ColorUIAdvSelectedHighlightedBorderColor
        {
            get
            {
                return m_ColorUIAdvSelectedHighlightedBorderColor;
            }
            set
            {
                if (m_ColorUIAdvSelectedHighlightedBorderColor != value)
                {
                    m_ColorUIAdvSelectedHighlightedBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="ColorUIAdv">.
        /// </summary>
        public Color ColorUIAdvGroupHeaderBackColor
        {
            get
            {
                return m_ColorUIAdvGroupHeaderBackColor;
            }
            set
            {
                if (m_ColorUIAdvGroupHeaderBackColor != value)
                {
                    m_ColorUIAdvGroupHeaderBackColor = value;
                }
            }
        }

        public Color StatusBarExtTopGradient
        {
            get
            {
                return this.m_StatusBarExtTopGradient;
            }
            set
            {
                if (!(this.m_StatusBarExtTopGradient != value))
                    return;
                this.m_StatusBarExtTopGradient = value;
            }
        }

        public Color StatusBarExtBottomGradient
        {
            get
            {
                return this.m_StatusBarExtBottomGradient;
            }
            set
            {
                if (!(this.m_StatusBarExtBottomGradient != value))
                    return;
                this.m_StatusBarExtBottomGradient = value;
            }
        }

        public Color StatusBarExtFillColor
        {
            get
            {
                return this.m_StatusBarExtFillColor;
            }
            set
            {
                if (!(this.m_StatusBarExtFillColor != value))
                    return;
                this.m_StatusBarExtFillColor = value;
            }
        }
        #endregion

        #endregion

        #region Class Utility Methods
        /// <summary>
        /// Initialize colors general for all colorscheme of the Office2010 visual style.
        /// </summary>
        protected virtual void InitializeColors()
        {
            // colors for menu
            m_MenuSeparatorColor = Color.FromArgb(197, 197, 197);
            m_MenuBorderColor = Color.FromArgb(134, 134, 134);
            m_MenuColumnColor = Color.FromArgb(233, 238, 238);
            m_MenuColumnSeparatorColor = Color.FromArgb(197, 197, 197);
            m_MenuItemBorderColor = Color.FromArgb(150, 175, 142, 80);
            m_MenuItemDarkColor = Color.FromArgb(255, 244, 197);
            m_MenuItemLightColor = Color.FromArgb(255, 232, 146);
            m_MenuCheckedColor = Color.FromArgb(52, 55, 124);
            m_MenuCheckedFillColor = Color.FromArgb(255, 227, 149);
            m_MenuCheckedBorderColor = Color.FromArgb(247, 212, 110);
            m_MenuComboButtonPushed1Color = Color.FromArgb(234, 224, 191);
            m_MenuComboButtonPushed2Color = Color.FromArgb(239, 189, 119);
            m_MenuComboButtonPushed3Color = Color.FromArgb(255, 168, 56);
            m_MenuComboButtonPushed4Color = Color.FromArgb(255, 212, 86);

            // colors for BarItem in CommandBar
            m_BarItemPressBorderColor = Color.FromArgb(194, 138, 48);
            m_BarItemHighlightBorderColor = Color.FromArgb(236, 199, 87);
            m_BarItemPressLightColor = Color.FromArgb(255, 223, 113);
            m_BarItemPressDarkColor = Color.FromArgb(255, 242, 114);
            m_DropDownBarItemLightColor = Color.FromArgb(255, 223, 113);
            m_DropDownBarItemDarkColor = Color.FromArgb(255, 242, 114);
            m_DropDownBarItemBorderColor = Color.FromArgb(194, 138, 48);
            m_BarItemCheckLightColor = Color.FromArgb(253, 210, 168);
            m_BarItemCheckDarkColor = Color.FromArgb(249, 147, 47);
            m_BarItemCheckBorderColor = Color.FromArgb(160, 131, 85);
            m_BarItemCheckFlashColor = Color.FromArgb(255, 253, 241, 176);
            m_BarItemPressFlashColor = Color.FromArgb(255, 255, 208, 134);
            m_BarItemSelectFlashColor = Color.FromArgb(255, 255, 235, 174);
            m_TextBarItemBackColor = SystemColors.Window;
            m_TextBarItemBorderColor = SystemColors.Window;
            m_TextBarItemBorderHighlightColor = Color.FromArgb(139, 118, 84);

            // colors for ComboButton of the ComboBoxBarItem in CommandBar.
            m_ComboButtonPressLightColor = Color.FromArgb(255, 197, 108);
            m_ComboButtonPressDarkColor = Color.FromArgb(251, 138, 59);
            m_ComboButtonHighlightLightColor = Color.FromArgb(255, 252, 217);
            m_ComboButtonHighlightDarkColor = Color.FromArgb(255, 214, 70);
            m_ComboButtonPressBorder = Color.FromArgb(139, 118, 84);
            m_ComboButtonHighlightBorder = Color.FromArgb(128, 185, 160, 116);

            // colors for tab group
            m_TabItemBorderColor = Color.FromArgb(153, 187, 232);
            m_TabItemInnerBorderColor = Color.FromArgb(239, 246, 255);
            m_TabItemOuterBorderColor = Color.FromArgb(209, 229, 254);
            m_TabItemTextColor = Color.FromArgb(21, 66, 139);
            m_TabItemActiveBottomColor = Color.FromArgb(225, 210, 163);
            m_TabItemTopGradientColor = Color.FromArgb(196, 221, 254);
            m_TabItemInActiveBottomColor = Color.FromArgb(235, 243, 253);
            m_TabItemMiddleLineColor = Color.FromArgb(215, 226, 232);
            m_TabPanelColor = Color.FromArgb(199, 216, 237);
            m_TabPanelBorderColor = Color.FromArgb(219, 232, 249);
            m_TabPanelBackColor = Color.FromArgb(199, 216, 237);

            // colors for GroupBar
            //m_GroupBarHighlightColorLight = Color.FromArgb(229, 241, 252);
            //m_GroupBarHighlightColorDark = Color.FromArgb(195, 219, 241);
            //m_GroupBarSelectedColorLight =  Color.FromArgb(255, 227, 123);
            //m_GroupBarSelectedColorDark = Color.FromArgb(255, 170, 57);
            //m_GroupBarSelectedTopColorLight = Color.FromArgb(199, 219, 239);
            //m_GroupBarSelectedTopColorDark = Color.FromArgb(184, 206, 231);
            //m_GroupBarSelectedHighlightColorLight = Color.FromArgb(200, 219, 239);
            //m_GroupBarSelectedHighlightColorDark = Color.FromArgb(230, 242, 251); 

            // colors for DataTimePicker
            m_DataTimePickerHighLightedBorderColor = Color.FromArgb(222, 183, 69);
            m_DataTimePickerSelectedBorderColor = Color.FromArgb(194, 129, 51);

            m_DataTimePickerDropDownHighLightLightColor = Color.FromArgb(245, 241, 211);
            m_DataTimePickerDropDownHighLightDarkColor = Color.FromArgb(236, 216, 141);
            m_DataTimePickerDropDownSelectedLightColor = Color.FromArgb(254, 223, 130);
            m_DataTimePickerDropDownSelectedDarkColor = Color.FromArgb(253, 226, 135);

            m_DataTimePickerCheckBoxNormalColor = Color.FromArgb(74, 93, 148);
            m_DataTimePickerCheckBoxSelectedColor =  Color.FromArgb(0, 32, 115);
            m_DataTimePickerCheckBoxInnerRectBorderNormalColor = Color.FromArgb(173, 178, 189);
            m_DataTimePickerCheckBoxInnerRectBorderSelectedColor =  Color.FromArgb(253, 203, 87);
            m_DataTimePickerCheckBoxInnerRectBorderPushedColor =  Color.FromArgb(241, 138, 35);
            m_DataTimePickerCheckBoxInnerRectFillNormalColor = Color.FromArgb(206, 207, 214);
            m_DataTimePickerCheckBoxInnerRectFillSelectedColor =  Color.FromArgb(250, 221, 143);
            m_DataTimePickerCheckBoxInnerRectFillPushedColor = Color.FromArgb(255, 206, 103);

            m_DataTimePickerHighLightedForeColor = Color.FromArgb(0, 101, 206);

            //NumericUpDownExt colors
            m_NumericUpDownBorderColor = Color.FromArgb(173, 174, 181);
            m_NumericUpDownHighLightedBorderColor = Color.FromArgb(57, 125, 181);
            m_NumericUpDownSelectedBorderColor = Color.FromArgb(41, 97, 140);
            m_NumericUpDownArrowLightColor = Color.FromArgb(115, 134, 214);
            m_NumericUpDownArrowDarkColor = Color.FromArgb(74, 85, 123);

            // colors for TabControlAdv
            m_TabDefaultBorderColor =  Color.FromArgb(141, 178, 227);
            m_TabHotLightBottomBorderLineColor =  Color.FromArgb(208, 195, 146);
            m_TabHotLightGradientTopBeginColor = Color.FromArgb(205, 217, 224);
            m_TabHotLightGradientTopEndColor = Color.FromArgb(228, 230, 222);
            m_TabHotLightGradientBottomBeginColor = Color.FromArgb(221, 221, 208);
            m_TabHotLightGradientBottomEndColor = Color.FromArgb(223, 213, 177);
            m_TabHotLightGradientCircleColor = Color.FromArgb(196, 221, 254);
            m_TabSelectedGradientTopColor = Color.FromArgb(240, 246, 254);
            m_TabSelectedGradientBottomColor = Color.FromArgb(225, 235, 246);
            m_TabSelectedInnerBorderColor = Color.FromArgb(235, 243, 252);
            m_TabHighlightInnerBorderColor = Color.FromArgb(234, 237, 253);
            m_TabSelectedHotLightBorderColor = Color.FromArgb(255, 208, 48);
            m_TabSelectedHotLightInnerBorderColor = Color.FromArgb(255, 240, 187);
            m_TabForeColor = Color.FromArgb(21, 66, 139);
            m_ActiveTabForeColor = m_TabForeColor;
            m_TabBackgroundColor = Color.FromArgb(199, 216, 237);

            // colors for DockTabControl 
            m_DockTabForeColor = Color.FromArgb(21, 66, 139);
            m_DockTabBackgroundColor = Color.FromArgb(199, 216, 237);

            // UpDown colors
            m_UpDownBorderHotColor = Color.FromArgb(221, 204, 155);
            m_UpDownInnerBorderHotStartColor = Color.FromArgb(254, 254, 254);
            m_UpDownInnerBorderHotEndColor = Color.FromArgb(253, 249, 245);

            m_UpDownBorderPressedColor = Color.FromArgb(152, 143, 103);
            m_UpDownInnerBorderPressedStartColor = Color.FromArgb(193, 180, 168);
            m_UpDownInnerBorderPressedEndColor = Color.FromArgb(255, 223, 141);

            m_UpDownBackgroundDisabledStartColor = Color.FromArgb(244, 244, 244);
            m_UpDownBackgroundDisabledEndColor = Color.FromArgb(201, 201, 201);
            m_UpDownBorderDisabledColor = Color.FromArgb(200, 200, 200);

            m_UpDownBackgroundHotTopStartColor = Color.FromArgb(255, 231, 114);
            m_UpDownBackgroundHotTopEndColor = Color.FromArgb(254, 242, 180);
            m_UpDownBackgroundHotBottomStartColor = Color.FromArgb(255, 214, 119);
            m_UpDownBackgroundHotBottomEndColor = Color.FromArgb(254, 221, 139);

            m_UpDownBackgroundPressedTopStartColor = Color.FromArgb(253, 223, 131);
            m_UpDownBackgroundPressedTopEndColor = Color.FromArgb(194, 153, 67);
            m_UpDownBackgroundPressedBottomStartColor = Color.FromArgb(255, 165, 59);
            m_UpDownBackgroundPressedBottomEndColor = Color.FromArgb(255, 190, 71);

            // ColorUIAdv colors
            m_ColorUIAdvBackColor = Color.FromArgb(250, 250, 250);
            m_ColorUIAdvTextColor = Color.FromArgb(2, 22, 109);
            m_ColorUIAdvItemBorderColor = Color.FromArgb(197, 197, 197);
            m_ColorUIAdvHighlightedBorderColor = Color.FromArgb(243, 148, 54);
            m_ColorUIAdvSelectedBorderColor = Color.FromArgb(235, 75, 13);
            m_ColorUIAdvSelectedHighlightedBorderColor = Color.FromArgb(255, 226, 148);
            m_ColorUIAdvGroupHeaderBackColor = Color.FromArgb(235, 235, 235);

            // colors for ButtonAdv
            m_ButtonPressedTopColor = Color.FromArgb(255, 197, 108);
            m_ButtonPressedBottomColor = Color.FromArgb(251, 138, 59);
            m_ButtonSelectedTopColor = ColorTranslator.FromHtml("#FCF9E0");
            m_ButtonSelectedBottomColor =ColorTranslator.FromHtml("#FAEAA8") ;
            m_ButtonDisabledTopColor = Color.FromArgb(244, 244, 244);
            m_ButtonDisabledBottomColor = Color.FromArgb(201, 201, 201);
            m_ButtonPressedBorderColor = Color.FromArgb(139, 118, 84);
            m_ButtonSelectedBorderColor = Color.FromArgb(185, 160, 116);
            m_ButtonDisabledBorderColor = Color.FromArgb(156, 164, 173);

            //StatusBarExt
            this.m_StatusBarExtTopGradient = Color.FromArgb(182, 209, 245);
            this.m_StatusBarExtBottomGradient = Color.FromArgb(64, 77, 140);
            this.m_StatusBarExtFillColor = Color.FromArgb(180, 205, 240);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="basicColor"></param>
        internal void UpdateColors(Color basicColor)
        {
            InitializeColors();

            Office2010Colors silverColors = GetColorTable(Office2010Theme.Silver);
            Type type = silverColors.GetType();
            PropertyInfo[] propInfoes = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (PropertyInfo pi in propInfoes)
            {
                if (pi.PropertyType == typeof(Color))
                {
                    string sFieldName = "m_" + pi.Name;
                    FieldInfo fi = type.GetField(sFieldName, BindingFlags.Instance | BindingFlags.NonPublic);

                    if (fi != null)
                    {
                        fi.SetValue(this, MergeColors((Color)pi.GetValue(silverColors, null), basicColor));
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheme"></param>
        internal void UpdateScheme(Office2010Theme scheme)
        {
            switch (scheme)
            {
                case Office2010Theme.Blue:
                    s_managedColors = new WeakReference(new Office2010BlueColors());
                    break;
                case Office2010Theme.Silver:
                    s_managedColors = new WeakReference(new Office2010SilverColors());
                    break;
                case Office2010Theme.Black:
                    s_managedColors = new WeakReference(new Office2010BlackColors());
                    break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseColor"></param>
        /// <param name="blendColor"></param>
        /// <returns></returns>
        internal static Color MergeColors(Color baseColor, Color blendColor)
        {
            int r = MergeChannels(baseColor.R, blendColor.R);
            int g = MergeChannels(baseColor.G, blendColor.G);
            int b = MergeChannels(baseColor.B, blendColor.B);

            return Color.FromArgb(r, g, b);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseChannel"></param>
        /// <param name="blendChannel"></param>
        /// <returns></returns>
        private static int MergeChannels(int baseChannel, int blendChannel)
        {
            const int MAX = 255;

            int min = (baseChannel * blendChannel) / MAX;
            int max = MAX - ((MAX - baseChannel) * (MAX - blendChannel)) / MAX;

            return (byte)(min + (baseChannel * (max - min)) / MAX);
        }
        #endregion

        #region ICloneable

        public virtual object Clone()
        {
            return (Office2010Colors)this.MemberwiseClone();
        }

        #endregion
    }
    public class MetroColors : ICloneable
    {
        #region Class Static Members
        private static WeakReference s_magentaColors = null;
        /// <summary>
        /// Colors for silver colorscheme of the metro visual style.
        /// </summary>
        private static WeakReference s_purpleColors = null;
        /// <summary>
        /// Colors for black colorscheme of the metro visual style.
        /// </summary>
        private static WeakReference s_tealColors = null;
        /// <summary>
        /// Colors for blue colorscheme of the metro visual style.
        /// </summary>
        private static WeakReference s_limeColors = null;
        /// <summary>
        /// Colors for blue colorscheme of the metro visual style.
        /// </summary>
        private static WeakReference s_brownColors = null;
        /// /// <summary>
        /// Colors for blue colorscheme of the metro visual style.
        /// </summary>
        private static WeakReference s_pinkColors = null;
        /// <summary>
        /// Colors for blue colorscheme of the metro visual style.
        /// </summary>
        private static WeakReference s_orangeColors = null;
        /// <summary>
        /// Colors for blue colorscheme of the metro visual style.
        /// </summary>
        private static WeakReference s_blueColors = null;
        /// <summary>
        /// Colors for blue colorscheme of the metro visual style.
        /// </summary>
        private static WeakReference s_redColors = null;
        /// <summary>
        /// Colors for blue colorscheme of the metro visual style.
        /// </summary>
        private static WeakReference s_greenColors = null;
        //<summary>
        /// </summary>
        private static WeakReference s_managedColors = null;
        /// <summary>
        //Default colorscheme for metro visual style.
        /// </summary>		
        private static MetroTheme s_defaultMetroTheme = MetroTheme.Managed;
        /// <summary>
        /// Base color for managed scheme.
        /// </summary>
        private static Color s_managedBaseColor = Color.Empty;
        #endregion

        #region Class Static Properties
        /// <summary>
        /// Gets or sets default colors for metro visual style.
        /// </summary>
        public static MetroColors Default
        {
            get
            {
                return GetColorTable(s_defaultMetroTheme);
            }
        }
        /// <summary>
        /// Gets or sets default colorscheme for metro visual style.
        /// </summary>
        public static MetroTheme DefaultTheme
        {
            get
            {
                return s_defaultMetroTheme;
            }
            set
            {
                if (value != s_defaultMetroTheme)
                {
                    s_defaultMetroTheme = value;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal static MetroColors ManagedColors
        {
            get
            {
                if (s_managedColors.IsAlive)
                    return s_managedColors.Target as MetroColors;
                else
                {
                    MetroColors colors = new MetroColors();
                    s_managedColors = new WeakReference(colors);
                    return colors;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal static Color ManagedBaseColor
        {
            get
            {
                return s_managedBaseColor;
            }
        }

        #endregion

        #region Class static events

        /// <summary>
        /// Arguments class for <see cref="MetroColors.ManagedColorsApplied"/> event.
        /// </summary>
        public class ManagedColorsAppliedEventArgs :
            EventArgs
        {
            /// <summary>
            /// Initializes <see cref="ManagedColorsAppliedEventArgs"/> instance.
            /// </summary>
            /// <param name="form">Container form.</param>
            /// <param name="baseColor">Base color for the managed theme.</param>
            public ManagedColorsAppliedEventArgs(Form form, Color baseColor)
            {
                this.Form = form;
                this.BaseColor = baseColor;
            }

            /// <summary>
            /// Container form.
            /// </summary>
            public Form Form;

            /// <summary>
            /// Base color for the managed theme.
            /// </summary>
            public Color BaseColor;
        }

        public delegate void ManagedColorsAppliedEventHandler(ManagedColorsAppliedEventArgs args);

        public static event ManagedColorsAppliedEventHandler ManagedColorsApplied;

        #endregion

        #region Class Static Public Methods
        /// <summary>
        /// Gets color table for Metro Color.
        /// </summary>
        public static MetroColors GetColorTable(MetroTheme theme)
        {
            MetroColors colorTable = null;

            switch (theme)
            {
                case MetroTheme.Magenta:
                    {
                        if (s_magentaColors.IsAlive)
                            colorTable = s_magentaColors.Target as MetroColors;
                        else
                        {
                            colorTable = new MetroMagentaColors();
                            s_magentaColors = new WeakReference(colorTable);
                        }
                        break;
                    }
                case MetroTheme.Purple:
                    {
                        if (s_purpleColors.IsAlive)
                            colorTable = s_purpleColors.Target as MetroColors;
                        else
                        {
                            colorTable = new MetroPurpleColors();
                            s_purpleColors = new WeakReference(colorTable);
                        }
                        break;
                    }
                case MetroTheme.Teal:
                    {
                        if (s_tealColors.IsAlive)
                            colorTable = s_tealColors.Target as MetroColors;
                        else
                        {
                            colorTable = new MetroTealColors();
                            s_tealColors = new WeakReference(colorTable);
                        }
                        break;
                    }
                case MetroTheme.Lime:
                    {
                        if (s_limeColors.IsAlive)
                            colorTable = s_limeColors.Target as MetroColors;
                        else
                        {
                            colorTable = new MetroLimeColors();
                            s_limeColors = new WeakReference(colorTable);
                        }
                        break;
                    }
                case MetroTheme.Brown:
                    {
                        if (s_brownColors.IsAlive)
                            colorTable = s_brownColors.Target as MetroColors;
                        else
                        {
                            colorTable = new MetroBrownColors();
                            s_brownColors = new WeakReference(colorTable);
                        }
                        break;
                    }
                case MetroTheme.Pink:
                    {
                        if (s_pinkColors.IsAlive)
                            colorTable = s_pinkColors.Target as MetroColors;
                        else
                        {
                            colorTable = new MetroPinkColors();
                            s_pinkColors = new WeakReference(colorTable);
                        }
                        break;
                    }
                case MetroTheme.Orange:
                    {
                        if (s_orangeColors.IsAlive)
                            colorTable = s_orangeColors.Target as MetroColors;
                        else
                        {
                            colorTable = new MetroOrangeColors();
                            s_orangeColors = new WeakReference(colorTable);
                        }
                        break;
                    }
                case MetroTheme.Blue:
                    {
                        if (s_blueColors.IsAlive)
                            colorTable = s_blueColors.Target as MetroColors;
                        else
                        {
                            colorTable = new MetroBlueColors();
                            s_blueColors = new WeakReference(colorTable);
                        }
                        break;
                    }
                case MetroTheme.Red:
                    {
                        if (s_redColors.IsAlive)
                            colorTable = s_redColors.Target as MetroColors;
                        else
                        {
                            colorTable = new MetroRedColors();
                            s_redColors = new WeakReference(colorTable);
                        }
                        break;
                    }
                case MetroTheme.Green:
                    {
                        if (s_greenColors.IsAlive)
                            colorTable = s_greenColors.Target as MetroColors;
                        else
                        {
                            colorTable = new MetroGreenColors();
                            s_greenColors = new WeakReference(colorTable);
                        }
                        break;
                    }
                case MetroTheme.Managed:
                    {
                        colorTable = ManagedColors;
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("Unknown theme.");
                    }
            }
            return colorTable;
        }
        /// <summary>
        /// Applies colors for managed scheme.
        /// </summary>
        /// <param name="form">Container form.</param>
        /// <param name="baseColor">Base color for the managed theme.</param>
        public static void ApplyManagedColors(Form form, Color baseColor)
        {
            s_managedBaseColor = baseColor;

            ManagedColors.UpdateColors(baseColor);
            OnManagedColorApplied(form, baseColor);

            if (form.IsHandleCreated)
            {
                NativeMethods.RedrawWindow(form.Handle, IntPtr.Zero, IntPtr.Zero, NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE);

                form.Invalidate(true);
                form.Update();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <param name="scheme"></param>
        public static void ApplyManagedScheme(Form form, MetroTheme scheme)
        {
            s_managedBaseColor = Color.Empty;

            ManagedColors.UpdateScheme(scheme);

            if (form.IsHandleCreated)
            {
                NativeMethods.RedrawWindow(form.Handle, IntPtr.Zero, IntPtr.Zero, NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE);

                form.Invalidate(true);
            }
        }

        protected static void OnManagedColorApplied(Form form, Color baseColor)
        {
            if (MetroColors.ManagedColorsApplied != null)
            {
                MetroColors.ManagedColorsApplied(new ManagedColorsAppliedEventArgs(form, baseColor));
            }
        }
        #endregion

        #region Initialize
        protected MetroColors()
        {
            InitializeColors();
        }
        #endregion

        #region Class Members

        // colors for ComboButton
        protected Color m_ComboButtonLightColor = Color.Empty;
        protected Color m_ComboButtonDarkColor = Color.Empty;
        protected Color m_ComboButtonPressLightColor = Color.Empty;
        protected Color m_ComboButtonPressDarkColor = Color.Empty;
        protected Color m_ComboButtonHighlightLightColor = Color.Empty;
        protected Color m_ComboButtonHighlightDarkColor = Color.Empty;
        protected Color m_ComboButtonBorder = Color.Empty;
        protected Color m_ComboButtonPressBorder = Color.Empty;
        protected Color m_ComboButtonHighlightBorder = Color.Empty;

        //ButtonAdvColors
        protected Color m_ButtonPressedTopColor = Color.Empty;
        protected Color m_ButtonPressedBottomColor = Color.Empty;
        protected Color m_ButtonSelectedTopColor = Color.Empty;
        protected Color m_ButtonSelectedBottomColor = Color.Empty;
        protected Color m_ButtonDisabledTopColor = Color.Empty;
        protected Color m_ButtonDisabledBottomColor = Color.Empty;
        protected Color m_ButtonPressedBorderColor = Color.Empty;
        protected Color m_ButtonSelectedBorderColor = Color.Empty;
        protected Color m_ButtonDisabledBorderColor = Color.Empty;
        protected Color m_ButtonDefaultTopColor = Color.Empty;
        protected Color m_ButtonDefaultBottomColor = Color.Empty;
        protected Color m_ButtonDefaultBorderColor = Color.Empty;
        protected Color m_ButtonDefaultInternalBorderColor = Color.Empty;
        protected Color m_ButtonPressedInternalBorderColor = Color.Empty;
        protected Color m_ButtonSelectedInternalBorderColor = Color.Empty;

        // ComboBoxAdv colors
        protected Color m_ComboBoxAdvNormalBackColor = Color.Empty;
        protected Color m_ComboBoxAdvHotBackColor = Color.Empty;
        protected Color m_ComboBoxAdvNormalBorderColor = Color.Empty;
        protected Color m_ComboBoxAdvHotBorderColor = Color.Empty;
        protected Color m_ComboBoxAdvPushedBorderColor = Color.Empty;
        protected Color m_ComboBoxAdvButtonUpperLineColor = Color.Empty;
        protected Color m_ComboBoxAdvArrowColor = Color.Empty;
        protected Color m_ComboBoxAdvLowerArrowLineColor = Color.Empty;
        protected Color m_ComboBoxAdvHotBackgroundButtonColor1 = Color.Empty;
        protected Color m_ComboBoxAdvHotBackgroundButtonColor2 = Color.Empty;
        protected Color m_ComboBoxAdvHotBackgroundButtonColor3 = Color.Empty;
        protected Color m_ComboBoxAdvHotBackgroundButtonColor4 = Color.Empty;
        protected Color m_ComboBoxAdvNormalBackgroundButtonColor1 = Color.Empty;
        protected Color m_ComboBoxAdvNormalBackgroundButtonColor2 = Color.Empty;
        protected Color m_ComboBoxAdvNormalBackgroundButtonColor3 = Color.Empty;
        protected Color m_ComboBoxAdvNormalBackgroundButtonColor4 = Color.Empty;
        protected Color m_ComboBoxAdvPushedBackgroundButtonColor1 = Color.Empty;
        protected Color m_ComboBoxAdvPushedBackgroundButtonColor2 = Color.Empty;
        protected Color m_ComboBoxAdvPushedBackgroundButtonColor3 = Color.Empty;
        protected Color m_ComboBoxAdvPushedBackgroundButtonColor4 = Color.Empty;

        //ComboBackground
        protected Color m_borderColorMetro = Color.Empty;
        protected Color m_foreColorTopLineMetro = Color.Empty;
        protected Color m_foreColorTopFirstMetro =Color.Empty;
        protected Color m_foreColorTopLastMetro = Color.Empty;
        protected Color m_foreColorBottomFirstMetro = Color.Empty;
        protected Color m_foreColorBottomLastMetro = Color.Empty;
        protected Color m_foreColorBottomLineMetro =Color.Empty;

        // CheckBoxAdv colors
        protected Color m_CheckBoxAdvNormalBackColor = Color.Empty;
        protected Color m_CheckBoxAdvSelectedBackColor = Color.Empty;
        protected Color m_CheckBoxAdvPushedBackColor = Color.Empty;
        protected Color m_CheckBoxAdvNormalBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvSelectedBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvPushedBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvNormalInternalBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvSelectedInternalBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvPushedInternalBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvNormalInternalRectangleBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvSelectedInternalRectangleBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvPushedInternalRectangleBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvNormalInternalRectangleColor = Color.Empty;
        protected Color m_CheckBoxAdvSelectedInternalRectangleColor = Color.Empty;
        protected Color m_CheckBoxAdvPushedInternalRectangleColor = Color.Empty;
        protected Color m_CheckBoxAdvNormalTickColor = Color.Empty;
        protected Color m_CheckBoxAdvSelectedTickColor = Color.Empty;
        protected Color m_CheckBoxAdvPushedTickColor = Color.Empty;
        protected Color m_CheckBoxAdvDisabledTickColor = Color.Empty;
        protected Color m_CheckBoxAdvIndeterminateRectangleColor = Color.Empty;
        protected Color m_CheckBoxAdvDisabledBackColor = Color.Empty;
        protected Color m_CheckBoxAdvDisabledBorderColor = Color.Empty;
        protected Color m_CheckBoxAdvDisabledInternalBorderColor = Color.Empty;

        // RadioButtonAdv colors
        protected Color m_RadioButtonAdvNormalBackColor = Color.Empty;
        protected Color m_RadioButtonAdvNormalBorderColor = Color.Empty;
        protected Color m_RadioButtonAdvNormalInternalBorderColor = Color.Empty;
        protected Color m_RadioButtonAdvSelectedBackColor = Color.Empty;
        protected Color m_RadioButtonAdvSelectedBorderColor = Color.Empty;
        protected Color m_RadioButtonAdvSelectedInternalBorderColor = Color.Empty;
        protected Color m_RadioButtonAdvPushedBackColor = Color.Empty;
        protected Color m_RadioButtonAdvPushedBorderColor = Color.Empty;
        protected Color m_RadioButtonAdvPushedInternalBorderColor = Color.Empty;
        protected Color m_RadioButtonAdvCheckMarkBorderColor = Color.Empty;
        protected Color m_RadioButtonAdvCheckMarkNormalBottomColor = Color.Empty;
        protected Color m_RadioButtonAdvCheckMarkSelectedBottomColor = Color.Empty;
        protected Color m_RadioButtonAdvCheckMarkPushedBottomColor = Color.Empty;
               
        #endregion

        #region Class Properties

        #region Button Colors
        /// <summary></summary>
        public Color ButtonPressedTopColor
        {
            get
            {
                return m_ButtonPressedTopColor;
            }
        }
        /// <summary></summary>
        public Color ButtonPressedBottomColor
        {
            get
            {
                return m_ButtonPressedBottomColor;
            }
        }
        /// <summary></summary>
        public Color ButtonSelectedTopColor
        {
            get
            {
                return m_ButtonSelectedTopColor;
            }
        }
        /// <summary></summary>
        public Color ButtonSelectedBottomColor
        {
            get
            {
                return m_ButtonSelectedBottomColor;
            }
        }
        /// <summary></summary>
        public Color ButtonDisabledTopColor
        {
            get
            {
                return m_ButtonDisabledTopColor;
            }
        }
        /// <summary></summary>
        public Color ButtonDisabledBottomColor
        {
            get
            {
                return m_ButtonDisabledBottomColor;
            }
        }
        /// <summary></summary>
        public Color ButtonPressedBorderColor
        {
            get
            {
                return m_ButtonPressedBorderColor;
            }
        }
        /// <summary></summary>
        public Color ButtonSelectedBorderColor
        {
            get
            {
                return m_ButtonSelectedBorderColor;
            }
        }
        /// <summary></summary>
        public Color ButtonDisabledBorderColor
        {
            get
            {
                return m_ButtonDisabledBorderColor;
            }
        }
        /// <summary></summary>
        public Color ButtonDefaultTopColor
        {
            get
            {
                if (m_ButtonDefaultTopColor != Color.Empty)
                {
                    return m_ButtonDefaultTopColor;
                }
                else
                {
                    return m_ButtonDefaultTopColor;
                }
            }
        }
        /// <summary></summary>
        public Color ButtonDefaultBottomColor
        {
            get
            {
                if (m_ButtonDefaultBottomColor != Color.Empty)
                {
                    return m_ButtonDefaultBottomColor;
                }
                else
                {
                    return m_ButtonDefaultBottomColor;
                }
            }
        }
        /// <summary></summary>
        public Color ButtonDefaultBorderColor
        {
            get
            {
                if (m_ButtonDefaultBorderColor != Color.Empty)
                {
                    return m_ButtonDefaultBorderColor;
                }
                else
                {
                    return m_ButtonDefaultBorderColor;
                }
            }
        }
        /// <summary></summary>
        public Color ButtonDefaultInternalBorderColor
        {
            get
            {
                if (m_ButtonDefaultInternalBorderColor != Color.Empty)
                {
                    return m_ButtonDefaultInternalBorderColor;
                }
                else
                {
                    return m_ButtonDefaultInternalBorderColor;
                }
            }
        }
        /// <summary></summary>
        public Color ButtonPressedInternalBorderColor
        {
            get
            {
                if (m_ButtonPressedInternalBorderColor != Color.Empty)
                {
                    return m_ButtonPressedInternalBorderColor;
                }
                else
                {
                    return m_ButtonPressedInternalBorderColor;
                }
            }
        }
        /// <summary></summary>
        public Color ButtonSelectedInternalBorderColor
        {
            get
            {
                if (m_ButtonSelectedInternalBorderColor != Color.Empty)
                {
                    return m_ButtonSelectedInternalBorderColor;
                }
                else
                {
                    return m_ButtonSelectedInternalBorderColor;
                }
            }
        }

        #endregion

        #region ComboBoxAdv colors
        /// <summary>
        /// Gets or sets the back color for <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvNormalBackColor
        {
            get
            {
                return m_ComboBoxAdvNormalBackColor;
            }
            set
            {
                if (m_ComboBoxAdvNormalBackColor != value)
                {
                    m_ComboBoxAdvNormalBackColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the back color for the selected <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvHotBackColor
        {
            get
            {
                return m_ComboBoxAdvHotBackColor;
            }
            set
            {
                if (m_ComboBoxAdvHotBackColor != value)
                {
                    m_ComboBoxAdvHotBackColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the border color for <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvNormalBorderColor
        {
            get
            {
                return m_ComboBoxAdvNormalBorderColor;
            }
            set
            {
                if (m_ComboBoxAdvNormalBorderColor != value)
                {
                    m_ComboBoxAdvNormalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the border color for the selected <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvHotBorderColor
        {
            get
            {
                return m_ComboBoxAdvHotBorderColor;
            }
            set
            {
                if (m_ComboBoxAdvHotBorderColor != value)
                {
                    m_ComboBoxAdvHotBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the border color for the pushed <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvPushedBorderColor
        {
            get
            {
                return m_ComboBoxAdvPushedBorderColor;
            }
            set
            {
                if (m_ComboBoxAdvPushedBorderColor != value)
                {
                    m_ComboBoxAdvPushedBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for upper line of the dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvButtonUpperLineColor
        {
            get
            {
                return m_ComboBoxAdvButtonUpperLineColor;
            }
            set
            {
                if (m_ComboBoxAdvButtonUpperLineColor != value)
                {
                    m_ComboBoxAdvButtonUpperLineColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for the arrow of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvArrowColor
        {
            get
            {
                return m_ComboBoxAdvArrowColor;
            }
            set
            {
                if (m_ComboBoxAdvArrowColor != value)
                {
                    m_ComboBoxAdvArrowColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for lower line of the arrow of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvLowerArrowLineColor
        {
            get
            {
                return m_ComboBoxAdvLowerArrowLineColor;
            }
            set
            {
                if (m_ComboBoxAdvLowerArrowLineColor != value)
                {
                    m_ComboBoxAdvLowerArrowLineColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the hot background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvHotBackgroundButtonColor1
        {
            get
            {
                return m_ComboBoxAdvHotBackgroundButtonColor1;
            }
            set
            {
                if (m_ComboBoxAdvHotBackgroundButtonColor1 != value)
                {
                    m_ComboBoxAdvHotBackgroundButtonColor1 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the hot background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvHotBackgroundButtonColor2
        {
            get
            {
                return m_ComboBoxAdvHotBackgroundButtonColor2;
            }
            set
            {
                if (m_ComboBoxAdvHotBackgroundButtonColor2 != value)
                {
                    m_ComboBoxAdvHotBackgroundButtonColor2 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the hot background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvHotBackgroundButtonColor3
        {
            get
            {
                return m_ComboBoxAdvHotBackgroundButtonColor3;
            }
            set
            {
                if (m_ComboBoxAdvHotBackgroundButtonColor3 != value)
                {
                    m_ComboBoxAdvHotBackgroundButtonColor3 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the hot background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvHotBackgroundButtonColor4
        {
            get
            {
                return m_ComboBoxAdvHotBackgroundButtonColor4;
            }
            set
            {
                if (m_ComboBoxAdvHotBackgroundButtonColor4 != value)
                {
                    m_ComboBoxAdvHotBackgroundButtonColor4 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvNormalBackgroundButtonColor1
        {
            get
            {
                return m_ComboBoxAdvNormalBackgroundButtonColor1;
            }
            set
            {
                if (m_ComboBoxAdvNormalBackgroundButtonColor1 != value)
                {
                    m_ComboBoxAdvNormalBackgroundButtonColor1 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvNormalBackgroundButtonColor2
        {
            get
            {
                return m_ComboBoxAdvNormalBackgroundButtonColor2;
            }
            set
            {
                if (m_ComboBoxAdvNormalBackgroundButtonColor2 != value)
                {
                    m_ComboBoxAdvNormalBackgroundButtonColor2 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvNormalBackgroundButtonColor3
        {
            get
            {
                return m_ComboBoxAdvNormalBackgroundButtonColor3;
            }
            set
            {
                if (m_ComboBoxAdvNormalBackgroundButtonColor3 != value)
                {
                    m_ComboBoxAdvNormalBackgroundButtonColor3 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvNormalBackgroundButtonColor4
        {
            get
            {
                return m_ComboBoxAdvNormalBackgroundButtonColor4;
            }
            set
            {
                if (m_ComboBoxAdvNormalBackgroundButtonColor4 != value)
                {
                    m_ComboBoxAdvNormalBackgroundButtonColor4 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvPushedBackgroundButtonColor1
        {
            get
            {
                return m_ComboBoxAdvPushedBackgroundButtonColor1;
            }
            set
            {
                if (m_ComboBoxAdvPushedBackgroundButtonColor1 != value)
                {
                    m_ComboBoxAdvPushedBackgroundButtonColor1 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvPushedBackgroundButtonColor2
        {
            get
            {
                return m_ComboBoxAdvPushedBackgroundButtonColor2;
            }
            set
            {
                if (m_ComboBoxAdvPushedBackgroundButtonColor2 != value)
                {
                    m_ComboBoxAdvPushedBackgroundButtonColor2 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvPushedBackgroundButtonColor3
        {
            get
            {
                return m_ComboBoxAdvPushedBackgroundButtonColor3;
            }
            set
            {
                if (m_ComboBoxAdvPushedBackgroundButtonColor3 != value)
                {
                    m_ComboBoxAdvPushedBackgroundButtonColor3 = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed background of dropdown button of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvPushedBackgroundButtonColor4
        {
            get
            {
                return m_ComboBoxAdvPushedBackgroundButtonColor4;
            }
            set
            {
                if (m_ComboBoxAdvPushedBackgroundButtonColor4 != value)
                {
                    m_ComboBoxAdvPushedBackgroundButtonColor4 = value;
                }
            }
        }
        /// <summary>
        /// Used in drawing of the background of dropdown list item of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvPushedBackgroundListItemBorder
        {
            get
            {
                return m_borderColorMetro;
            }
            set
            {
                if (m_borderColorMetro != value)
                {
                    m_borderColorMetro = value;
                }
            }
        }
        /// <summary>
        /// Used in drawing of the background of dropdown list item Top of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvPushedBackgroundListItemTop
        {
            get
            {
                return m_foreColorTopLineMetro;
            }
            set
            {
                if (m_foreColorTopLineMetro != value)
                {
                    m_foreColorTopLineMetro = value;
                }
            }
        }
        /// <summary>
        /// Used in drawing of the background of dropdown list item of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvPushedBackgroundListItemTopFirst
        {
            get
            {
                return m_foreColorTopFirstMetro;
            }
            set
            {
                if (m_foreColorTopFirstMetro != value)
                {
                    m_foreColorTopFirstMetro = value;
                }
            }
        }
        /// <summary>
        /// Used in drawing of the background of dropdown list item of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvPushedBackgroundListItemTopLast
        {
            get
            {
                return m_foreColorTopLastMetro;
            }
            set
            {
                if (m_foreColorTopLastMetro != value)
                {
                    m_foreColorTopLastMetro = value;
                }
            }
        }
        /// <summary>
        /// Used in drawing of the background of dropdown list item of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvPushedBackgroundListItemBottomFirst
        {
            get
            {
                return m_foreColorBottomFirstMetro;
            }
            set
            {
                if (m_foreColorBottomFirstMetro != value)
                {
                    m_foreColorBottomFirstMetro = value;
                }
            }
        }
        /// <summary>
        /// Used in drawing of the background of dropdown list item of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvPushedBackgroundListItemBottomLast
        {
            get
            {
                return m_foreColorBottomLastMetro;
            }
            set
            {
                if (m_foreColorBottomLastMetro != value)
                {
                    m_foreColorBottomLastMetro = value;
                }
            }
        }
        /// <summary>
        /// Used in drawing of the background of dropdown list item of <see cref="ComboBoxAdv">.
        /// </summary>
        public Color ComboBoxAdvPushedBackgroundListItemBottom
        {
            get
            {
                return m_foreColorBottomLineMetro;
            }
            set
            {
                if (m_foreColorBottomLineMetro != value)
                {
                    m_foreColorBottomLineMetro = value;
                }
            }
        }
        #endregion

        #region CheckBoxAdv colors
        /// <summary>
        /// Used in drawing of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvNormalBackColor
        {
            get
            {
                return m_CheckBoxAdvNormalBackColor;
            }
            set
            {
                if (m_CheckBoxAdvNormalBackColor != value)
                {
                    m_CheckBoxAdvNormalBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvSelectedBackColor
        {
            get
            {
                return m_CheckBoxAdvSelectedBackColor;
            }
            set
            {
                if (m_CheckBoxAdvSelectedBackColor != value)
                {
                    m_CheckBoxAdvSelectedBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvPushedBackColor
        {
            get
            {
                return m_CheckBoxAdvPushedBackColor;
            }
            set
            {
                if (m_CheckBoxAdvPushedBackColor != value)
                {
                    m_CheckBoxAdvPushedBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvNormalBorderColor
        {
            get
            {
                return m_CheckBoxAdvNormalBorderColor;
            }
            set
            {
                if (m_CheckBoxAdvNormalBorderColor != value)
                {
                    m_CheckBoxAdvNormalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the selected border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvSelectedBorderColor
        {
            get
            {
                return m_CheckBoxAdvSelectedBorderColor;
            }
            set
            {
                if (m_CheckBoxAdvSelectedBorderColor != value)
                {
                    m_CheckBoxAdvSelectedBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvPushedBorderColor
        {
            get
            {
                return m_CheckBoxAdvPushedBorderColor;
            }
            set
            {
                if (m_CheckBoxAdvPushedBorderColor != value)
                {
                    m_CheckBoxAdvPushedBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal internal border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvNormalInternalBorderColor
        {
            get
            {
                return m_CheckBoxAdvNormalInternalBorderColor;
            }
            set
            {
                if (m_CheckBoxAdvNormalInternalBorderColor != value)
                {
                    m_CheckBoxAdvNormalInternalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the selected internal border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvSelectedInternalBorderColor
        {
            get
            {
                return m_CheckBoxAdvSelectedInternalBorderColor;
            }
            set
            {
                if (m_CheckBoxAdvSelectedInternalBorderColor != value)
                {
                    m_CheckBoxAdvSelectedInternalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed internal border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvPushedInternalBorderColor
        {
            get
            {
                return m_CheckBoxAdvPushedInternalBorderColor;
            }
            set
            {
                if (m_CheckBoxAdvPushedInternalBorderColor != value)
                {
                    m_CheckBoxAdvPushedInternalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal internal rectangle border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvNormalInternalRectangleBorderColor
        {
            get
            {
                return m_CheckBoxAdvNormalInternalRectangleBorderColor;
            }
            set
            {
                if (m_CheckBoxAdvNormalInternalRectangleBorderColor != value)
                {
                    m_CheckBoxAdvNormalInternalRectangleBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the selected internal rectangle border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvSelectedInternalRectangleBorderColor
        {
            get
            {
                return m_CheckBoxAdvSelectedInternalRectangleBorderColor;
            }
            set
            {
                if (m_CheckBoxAdvSelectedInternalRectangleBorderColor != value)
                {
                    m_CheckBoxAdvSelectedInternalRectangleBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed internal rectangle border of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvPushedInternalRectangleBorderColor
        {
            get
            {
                return m_CheckBoxAdvPushedInternalRectangleBorderColor;
            }
            set
            {
                if (m_CheckBoxAdvPushedInternalRectangleBorderColor != value)
                {
                    m_CheckBoxAdvPushedInternalRectangleBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal internal rectangle of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvNormalInternalRectangleColor
        {
            get
            {
                return m_CheckBoxAdvNormalInternalRectangleColor;
            }
            set
            {
                if (m_CheckBoxAdvNormalInternalRectangleColor != value)
                {
                    m_CheckBoxAdvNormalInternalRectangleColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the selected internal rectangle of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvSelectedInternalRectangleColor
        {
            get
            {
                return m_CheckBoxAdvSelectedInternalRectangleColor;
            }
            set
            {
                if (m_CheckBoxAdvSelectedInternalRectangleColor != value)
                {
                    m_CheckBoxAdvSelectedInternalRectangleColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed internal rectangle of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvPushedInternalRectangleColor
        {
            get
            {
                return m_CheckBoxAdvPushedInternalRectangleColor;
            }
            set
            {
                if (m_CheckBoxAdvPushedInternalRectangleColor != value)
                {
                    m_CheckBoxAdvPushedInternalRectangleColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the normal tick of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvNormalTickColor
        {
            get
            {
                return m_CheckBoxAdvNormalTickColor;
            }
            set
            {
                if (m_CheckBoxAdvNormalTickColor != value)
                {
                    m_CheckBoxAdvNormalTickColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the selected tick of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvSelectedTickColor
        {
            get
            {
                return m_CheckBoxAdvSelectedTickColor;
            }
            set
            {
                if (m_CheckBoxAdvSelectedTickColor != value)
                {
                    m_CheckBoxAdvSelectedTickColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the pushed tick of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvPushedTickColor
        {
            get
            {
                return m_CheckBoxAdvPushedTickColor;
            }
            set
            {
                if (m_CheckBoxAdvPushedTickColor != value)
                {
                    m_CheckBoxAdvPushedTickColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the disabled tick of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvDisabledTickColor
        {
            get
            {
                return m_CheckBoxAdvDisabledTickColor;
            }
            set
            {
                if (m_CheckBoxAdvDisabledTickColor != value)
                {
                    m_CheckBoxAdvDisabledTickColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the indeterminate rectangle of <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvIndeterminateRectangleColor
        {
            get
            {
                return m_CheckBoxAdvIndeterminateRectangleColor;
            }
            set
            {
                if (m_CheckBoxAdvIndeterminateRectangleColor != value)
                {
                    m_CheckBoxAdvIndeterminateRectangleColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the disabled back color <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvDisabledBackColor
        {
            get
            {
                return m_CheckBoxAdvDisabledBackColor;
            }
            set
            {
                if (m_CheckBoxAdvDisabledBackColor != value)
                {
                    m_CheckBoxAdvDisabledBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the disabled border <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvDisabledBorderColor
        {
            get
            {
                return m_CheckBoxAdvDisabledBorderColor;
            }
            set
            {
                if (m_CheckBoxAdvDisabledBorderColor != value)
                {
                    m_CheckBoxAdvDisabledBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the disabled internal border <see cref="CheckBoxAdv">.
        /// </summary>
        public Color CheckBoxAdvDisabledInternalBorderColor
        {
            get
            {
                return m_CheckBoxAdvDisabledInternalBorderColor;
            }
            set
            {
                if (m_CheckBoxAdvDisabledInternalBorderColor != value)
                {
                    m_CheckBoxAdvDisabledInternalBorderColor = value;
                }
            }
        }

        #endregion

        #region RadioButtonAdv colors
        /// <summary>
        /// Used in drawing of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvNormalBackColor
        {
            get
            {
                return m_RadioButtonAdvNormalBackColor;
            }
            set
            {
                if (m_RadioButtonAdvNormalBackColor != value)
                {
                    m_RadioButtonAdvNormalBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the border of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvNormalBorderColor
        {
            get
            {
                return m_RadioButtonAdvNormalBorderColor;
            }
            set
            {
                if (m_RadioButtonAdvNormalBorderColor != value)
                {
                    m_RadioButtonAdvNormalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the internal border of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvNormalInternalBorderColor
        {
            get
            {
                return m_RadioButtonAdvNormalInternalBorderColor;
            }
            set
            {
                if (m_RadioButtonAdvNormalInternalBorderColor != value)
                {
                    m_RadioButtonAdvNormalInternalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvSelectedBackColor
        {
            get
            {
                return m_RadioButtonAdvSelectedBackColor;
            }
            set
            {
                if (m_RadioButtonAdvSelectedBackColor != value)
                {
                    m_RadioButtonAdvSelectedBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the border of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvSelectedBorderColor
        {
            get
            {
                return m_RadioButtonAdvSelectedBorderColor;
            }
            set
            {
                if (m_RadioButtonAdvSelectedBorderColor != value)
                {
                    m_RadioButtonAdvSelectedBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the internal border of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvSelectedInternalBorderColor
        {
            get
            {
                return m_RadioButtonAdvSelectedInternalBorderColor;
            }
            set
            {
                if (m_RadioButtonAdvSelectedInternalBorderColor != value)
                {
                    m_RadioButtonAdvSelectedInternalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvPushedBackColor
        {
            get
            {
                return m_RadioButtonAdvPushedBackColor;
            }
            set
            {
                if (m_RadioButtonAdvPushedBackColor != value)
                {
                    m_RadioButtonAdvPushedBackColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the border of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvPushedBorderColor
        {
            get
            {
                return m_RadioButtonAdvPushedBorderColor;
            }
            set
            {
                if (m_RadioButtonAdvPushedBorderColor != value)
                {
                    m_RadioButtonAdvPushedBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the internal border of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvPushedInternalBorderColor
        {
            get
            {
                return m_RadioButtonAdvPushedInternalBorderColor;
            }
            set
            {
                if (m_RadioButtonAdvPushedInternalBorderColor != value)
                {
                    m_RadioButtonAdvPushedInternalBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of the border of check mark of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvCheckMarkBorderColor
        {
            get
            {
                return m_RadioButtonAdvCheckMarkBorderColor;
            }
            set
            {
                if (m_RadioButtonAdvCheckMarkBorderColor != value)
                {
                    m_RadioButtonAdvCheckMarkBorderColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of check mark of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvCheckMarkNormalBottomColor
        {
            get
            {
                return m_RadioButtonAdvCheckMarkNormalBottomColor;
            }
            set
            {
                if (m_RadioButtonAdvCheckMarkNormalBottomColor != value)
                {
                    m_RadioButtonAdvCheckMarkNormalBottomColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of check mark of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvCheckMarkSelectedBottomColor
        {
            get
            {
                return m_RadioButtonAdvCheckMarkSelectedBottomColor;
            }
            set
            {
                if (m_RadioButtonAdvCheckMarkSelectedBottomColor != value)
                {
                    m_RadioButtonAdvCheckMarkSelectedBottomColor = value;
                }
            }
        }

        /// <summary>
        /// Used in drawing of check mark of <see cref="RadioButtonAdv">.
        /// </summary>
        public Color RadioButtonAdvCheckMarkPushedBottomColor
        {
            get
            {
                return m_RadioButtonAdvCheckMarkPushedBottomColor;
            }
            set
            {
                if (m_RadioButtonAdvCheckMarkPushedBottomColor != value)
                {
                    m_RadioButtonAdvCheckMarkPushedBottomColor = value;
                }
            }
        }

        #endregion

        #region Class Utility Methods
        /// <summary>
        /// Initialize colors general for all colorscheme of the Metro visual style.
        /// </summary>
        protected virtual void InitializeColors()
        {
            // colors for ButtonAdv 
            m_ButtonPressedTopColor = Color.FromArgb(17, 158, 218);
            m_ButtonPressedBottomColor = Color.FromArgb(17, 158, 218);
            m_ButtonPressedBorderColor = Color.FromArgb(17, 158, 218);

            m_ButtonSelectedTopColor = Color.FromArgb(216, 216, 217);
            m_ButtonSelectedBottomColor = Color.FromArgb(216, 216, 217);
            m_ButtonSelectedBorderColor = Color.FromArgb(216, 216, 217);

            m_ButtonDisabledTopColor = Color.FromArgb(245, 245, 245);
            m_ButtonDisabledBottomColor = Color.FromArgb(245, 245, 245);
            m_ButtonDisabledBorderColor = Color.FromArgb(245, 245, 245);

            m_ButtonDefaultTopColor = Color.FromArgb(235, 235, 235);
            m_ButtonDefaultBottomColor = Color.FromArgb(235, 235, 235);
            m_ButtonDefaultBorderColor = Color.FromArgb(235, 235, 235);

            m_ButtonDefaultInternalBorderColor = Color.FromArgb(235, 235, 235);
            m_ButtonPressedInternalBorderColor = Color.FromArgb(17, 158, 218);
            m_ButtonSelectedInternalBorderColor = Color.FromArgb(216, 216, 217);

            //// colors for comboBoxAdv
            m_ComboBoxAdvNormalBackColor = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvNormalBackgroundButtonColor1 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvNormalBackgroundButtonColor2 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvNormalBackgroundButtonColor3 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvNormalBackgroundButtonColor4 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvNormalBorderColor = Color.FromArgb(204, 204, 204);

            m_ComboBoxAdvHotBorderColor = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvHotBackColor = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor1 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor2 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor3 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor4 = Color.FromArgb(255, 255, 255);

            m_ComboBoxAdvButtonUpperLineColor = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvArrowColor = Color.FromArgb(51, 51, 51);
            m_ComboBoxAdvLowerArrowLineColor = Color.FromArgb(204, 204, 204);

            m_ComboBoxAdvPushedBackgroundButtonColor1 = Color.FromArgb(17, 158, 218);
            m_ComboBoxAdvPushedBackgroundButtonColor2 = Color.FromArgb(17, 158, 218);
            m_ComboBoxAdvPushedBackgroundButtonColor3 = Color.FromArgb(17, 158, 218);
            m_ComboBoxAdvPushedBackgroundButtonColor4 = Color.FromArgb(17, 158, 218);
            m_ComboBoxAdvPushedBorderColor = Color.FromArgb(204, 204, 204);

            //Combackbackground dropdown
            m_borderColorMetro = Color.FromArgb(17, 158, 218);
            m_foreColorTopLineMetro = Color.FromArgb(17, 158, 218);
            m_foreColorTopFirstMetro = Color.FromArgb(17, 158, 218);
            m_foreColorTopLastMetro = Color.FromArgb(17, 158, 218);
            m_foreColorBottomFirstMetro = Color.FromArgb(17, 158, 218);
            m_foreColorBottomLastMetro = Color.FromArgb(17, 158, 218);
            m_foreColorBottomLineMetro = Color.FromArgb(17, 158, 218);

            // colors for CheckBoxAdv
            m_CheckBoxAdvNormalBackColor = Color.FromArgb(253, 253, 253);
            m_CheckBoxAdvSelectedBackColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedBackColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvSelectedBorderColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvPushedBorderColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvNormalInternalBorderColor = Color.FromArgb(253, 253, 253);
            m_CheckBoxAdvSelectedInternalBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalBorderColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalInternalRectangleBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvSelectedInternalRectangleBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalRectangleBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvNormalInternalRectangleColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvSelectedInternalRectangleColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalRectangleColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvSelectedTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvPushedTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvDisabledTickColor = Color.FromArgb(51, 51, 51);

            m_CheckBoxAdvIndeterminateRectangleColor = Color.FromArgb(238, 238, 238);
            m_CheckBoxAdvDisabledBackColor = Color.FromArgb(238, 238, 238);
            m_CheckBoxAdvDisabledBorderColor = Color.FromArgb(238, 238, 238);
            m_CheckBoxAdvDisabledInternalBorderColor = Color.FromArgb(238, 238, 238);

            // colors for RadioButtonAdv
            m_RadioButtonAdvNormalBackColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvNormalBorderColor = Color.FromArgb(173, 173, 173);
            m_RadioButtonAdvNormalInternalBorderColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvSelectedBackColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvSelectedBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvSelectedInternalBorderColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvPushedBackColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvPushedBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvPushedInternalBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkNormalBottomColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkSelectedBottomColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkPushedBottomColor = Color.FromArgb(0, 0, 0);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="basicColor"></param>
        internal void UpdateColors(Color basicColor)
        {
            InitializeColors();

            MetroColors megentaColors = GetColorTable(MetroTheme.Magenta);
            Type type = megentaColors.GetType();
            PropertyInfo[] propInfoes = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (PropertyInfo pi in propInfoes)
            {
                if (pi.PropertyType == typeof(Color))
                {
                    string sFieldName = "m_" + pi.Name;
                    FieldInfo fi = type.GetField(sFieldName, BindingFlags.Instance | BindingFlags.NonPublic);

                    if (fi != null)
                    {
                        fi.SetValue(this, MergeColors((Color)pi.GetValue(megentaColors, null), basicColor));
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheme"></param>
        /// 
        internal void UpdateScheme(MetroTheme scheme)
        {
            switch (scheme)
            {
                case MetroTheme.Magenta:
                    s_magentaColors = new WeakReference(new MetroMagentaColors());
                    break;
                case MetroTheme.Purple:
                    s_purpleColors = new WeakReference(new MetroPurpleColors());
                    break;
                case MetroTheme.Teal:
                    s_tealColors = new WeakReference(new MetroTealColors());
                    break;
                case MetroTheme.Lime:
                    s_limeColors = new WeakReference(new MetroLimeColors());
                    break;
                case MetroTheme.Brown:
                    s_brownColors = new WeakReference(new MetroBrownColors());
                    break;
                case MetroTheme.Pink:
                    s_pinkColors = new WeakReference(new MetroPinkColors());
                    break;
                case MetroTheme.Orange:
                    s_orangeColors = new WeakReference(new MetroOrangeColors());
                    break;
                case MetroTheme.Blue:
                    s_blueColors = new WeakReference(new MetroBlueColors());
                    break;
                case MetroTheme.Red:
                    s_redColors = new WeakReference(new MetroRedColors());
                    break;
                case MetroTheme.Green:
                    s_greenColors = new WeakReference(new MetroGreenColors());
                    break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseColor"></param>
        /// <param name="blendColor"></param>
        /// <returns></returns>
        internal static Color MergeColors(Color baseColor, Color blendColor)
        {
            int r = MergeChannels(baseColor.R, blendColor.R);
            int g = MergeChannels(baseColor.G, blendColor.G);
            int b = MergeChannels(baseColor.B, blendColor.B);

            return Color.FromArgb(r, g, b);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseChannel"></param>
        /// <param name="blendChannel"></param>
        /// <returns></returns>
        private static int MergeChannels(int baseChannel, int blendChannel)
        {
            const int MAX = 255;

            int min = (baseChannel * blendChannel) / MAX;
            int max = MAX - ((MAX - baseChannel) * (MAX - blendChannel)) / MAX;

            return (byte)(min + (baseChannel * (max - min)) / MAX);
        }
        #endregion
        static MetroColors()
        {
            s_managedColors = new WeakReference(new MetroColors());
            s_magentaColors = new WeakReference(new MetroMagentaColors());
            s_purpleColors = new WeakReference(new MetroPurpleColors());
            s_tealColors = new WeakReference(new MetroTealColors());
            s_brownColors = new WeakReference(new MetroBrownColors());
            s_limeColors = new WeakReference(new MetroLimeColors());
            s_orangeColors = new WeakReference(new MetroOrangeColors());
            s_pinkColors = new WeakReference(new MetroPinkColors());
            s_blueColors = new WeakReference(new MetroBlueColors());
            s_greenColors = new WeakReference(new MetroGreenColors());
            s_redColors = new WeakReference(new MetroRedColors());
        }

        #region ICloneable

        public virtual object Clone()
        {
            return (MetroColors)this.MemberwiseClone();
        }

        #endregion
    }
    /// <summary>
    /// Provides colors for Magenta colorscheme of the Metro visual style.
    /// </summary>
    public class MetroMagentaColors : MetroColors
    {
        #region Class Overrides
        /// <summary>
        /// Initialize colors for Magenta colorscheme of the Metro visual style.
        /// </summary>
        protected override void InitializeColors()
        {
            base.InitializeColors();
            // colors for ButtonAdv 
            m_ButtonPressedTopColor = Color.FromArgb(17, 158, 218);
            m_ButtonPressedBottomColor = Color.FromArgb(17, 158, 218);
            m_ButtonPressedBorderColor = Color.FromArgb(17, 158, 218);

            m_ButtonSelectedTopColor = Color.FromArgb(216, 216, 217);
            m_ButtonSelectedBottomColor = Color.FromArgb(216, 216, 217);
            m_ButtonSelectedBorderColor = Color.FromArgb(216, 216, 217);

            m_ButtonDisabledTopColor = Color.FromArgb(245, 245, 245);
            m_ButtonDisabledBottomColor = Color.FromArgb(245, 245, 245);
            m_ButtonDisabledBorderColor = Color.FromArgb(245, 245, 245);

            m_ButtonDefaultTopColor = Color.Magenta;
            m_ButtonDefaultBottomColor = Color.Magenta;
            m_ButtonDefaultBorderColor = Color.Magenta;

            m_ButtonDefaultInternalBorderColor = Color.Magenta;
            m_ButtonPressedInternalBorderColor = Color.FromArgb(17, 158, 218);
            m_ButtonSelectedInternalBorderColor = Color.FromArgb(216, 216, 217);

            //// colors for comboBoxAdv
            m_ComboBoxAdvNormalBackColor = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvNormalBackgroundButtonColor1 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvNormalBackgroundButtonColor2 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvNormalBackgroundButtonColor3 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvNormalBackgroundButtonColor4 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvNormalBorderColor = Color.FromArgb(204, 204, 204);

            m_ComboBoxAdvHotBorderColor = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvHotBackColor = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor1 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor2 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor3 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor4 = Color.FromArgb(255, 255, 255);

            m_ComboBoxAdvButtonUpperLineColor = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvArrowColor = Color.FromArgb(51, 51, 51);
            m_ComboBoxAdvLowerArrowLineColor = Color.FromArgb(204, 204, 204);

            m_ComboBoxAdvPushedBackgroundButtonColor1 = Color.Magenta;
            m_ComboBoxAdvPushedBackgroundButtonColor2 = Color.Magenta;
            m_ComboBoxAdvPushedBackgroundButtonColor3 = Color.Magenta;
            m_ComboBoxAdvPushedBackgroundButtonColor4 = Color.Magenta;
            m_ComboBoxAdvPushedBorderColor = Color.FromArgb(204, 204, 204);

            //combo background dropdown
            m_borderColorMetro = Color.Magenta;
            m_foreColorTopLineMetro = Color.Magenta;
            m_foreColorTopFirstMetro = Color.Magenta;
            m_foreColorTopLastMetro = Color.Magenta;
            m_foreColorBottomFirstMetro = Color.Magenta;
            m_foreColorBottomLastMetro = Color.Magenta;
            m_foreColorBottomLineMetro = Color.Magenta;

            // colors for CheckBoxAdv
            m_CheckBoxAdvNormalBackColor = Color.Magenta;
            m_CheckBoxAdvSelectedBackColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedBackColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalBorderColor = Color.Magenta;
            m_CheckBoxAdvSelectedBorderColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvPushedBorderColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvNormalInternalBorderColor = Color.FromArgb(253, 253, 253);
            m_CheckBoxAdvSelectedInternalBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalBorderColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalInternalRectangleBorderColor = Color.Magenta;
            m_CheckBoxAdvSelectedInternalRectangleBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalRectangleBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvNormalInternalRectangleColor = Color.Magenta;
            m_CheckBoxAdvSelectedInternalRectangleColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalRectangleColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvSelectedTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvPushedTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvDisabledTickColor = Color.FromArgb(51, 51, 51);

            m_CheckBoxAdvIndeterminateRectangleColor = Color.Magenta;
            m_CheckBoxAdvDisabledBackColor = Color.FromArgb(238, 238, 238);
            m_CheckBoxAdvDisabledBorderColor = Color.FromArgb(238, 238, 238);
            m_CheckBoxAdvDisabledInternalBorderColor = Color.FromArgb(238, 238, 238);

            // colors for RadioButtonAdv
            m_RadioButtonAdvNormalBackColor = Color.Magenta;
            m_RadioButtonAdvNormalBorderColor = Color.Magenta;
            m_RadioButtonAdvNormalInternalBorderColor = Color.Magenta;
            m_RadioButtonAdvSelectedBackColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvSelectedBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvSelectedInternalBorderColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvPushedBackColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvPushedBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvPushedInternalBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkNormalBottomColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkSelectedBottomColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkPushedBottomColor = Color.FromArgb(0, 0, 0);
        }
        #endregion
    }
    /// <summary>
    /// Provides colors for Orange colorscheme of the Metro visual style.
    /// </summary>
    public class MetroOrangeColors : MetroColors
    {
        #region Class Overrides
        /// <summary>
        /// Initialize colors for Blue colorscheme of the Metro visual style.
        /// </summary>
        protected override void InitializeColors()
        {
            base.InitializeColors();
            // colors for ButtonAdv 
            m_ButtonPressedTopColor = Color.FromArgb(17, 158, 218);
            m_ButtonPressedBottomColor = Color.FromArgb(17, 158, 218);
            m_ButtonPressedBorderColor = Color.FromArgb(17, 158, 218);

            m_ButtonSelectedTopColor = Color.FromArgb(216, 216, 217);
            m_ButtonSelectedBottomColor = Color.FromArgb(216, 216, 217);
            m_ButtonSelectedBorderColor = Color.FromArgb(216, 216, 217);

            m_ButtonDisabledTopColor = Color.FromArgb(245, 245, 245);
            m_ButtonDisabledBottomColor = Color.FromArgb(245, 245, 245);
            m_ButtonDisabledBorderColor = Color.FromArgb(245, 245, 245);

            m_ButtonDefaultTopColor = Color.Orange;
            m_ButtonDefaultBottomColor = Color.Orange;
            m_ButtonDefaultBorderColor = Color.Orange;

            m_ButtonDefaultInternalBorderColor = Color.Magenta;
            m_ButtonPressedInternalBorderColor = Color.Orange;
            m_ButtonSelectedInternalBorderColor = Color.FromArgb(216, 216, 217);

            //// colors for comboBoxAdv
            m_ComboBoxAdvNormalBackColor = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvNormalBackgroundButtonColor1 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor2 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor3 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor4 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBorderColor = Color.FromArgb(204, 204, 204);

            m_ComboBoxAdvHotBorderColor = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvHotBackColor = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor1 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor2 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor3 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor4 = Color.FromArgb(255, 255, 255);

            m_ComboBoxAdvButtonUpperLineColor = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvArrowColor = Color.FromArgb(51, 51, 51);
            m_ComboBoxAdvLowerArrowLineColor = Color.FromArgb(204, 204, 204);

            m_ComboBoxAdvPushedBackgroundButtonColor1 = Color.Orange;
            m_ComboBoxAdvPushedBackgroundButtonColor2 = Color.Orange;
            m_ComboBoxAdvPushedBackgroundButtonColor3 = Color.Orange;
            m_ComboBoxAdvPushedBackgroundButtonColor4 = Color.Orange;
            m_ComboBoxAdvPushedBorderColor = Color.FromArgb(204, 204, 204);

            //combo background dropdown
            m_borderColorMetro = Color.Orange;
            m_foreColorTopLineMetro = Color.Orange;
            m_foreColorTopFirstMetro = Color.Orange;
            m_foreColorTopLastMetro = Color.Orange;
            m_foreColorBottomFirstMetro = Color.Orange;
            m_foreColorBottomLastMetro = Color.Orange;
            m_foreColorBottomLineMetro = Color.Orange;

            // colors for CheckBoxAdv
            m_CheckBoxAdvNormalBackColor = Color.Orange;
            m_CheckBoxAdvSelectedBackColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedBackColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalBorderColor = Color.Orange;
            m_CheckBoxAdvSelectedBorderColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvPushedBorderColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvNormalInternalBorderColor = Color.FromArgb(253, 253, 253);
            m_CheckBoxAdvSelectedInternalBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalBorderColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalInternalRectangleBorderColor = Color.Orange;
            m_CheckBoxAdvSelectedInternalRectangleBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalRectangleBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvNormalInternalRectangleColor = Color.Orange;
            m_CheckBoxAdvSelectedInternalRectangleColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalRectangleColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvSelectedTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvPushedTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvDisabledTickColor = Color.FromArgb(51, 51, 51);

            m_CheckBoxAdvIndeterminateRectangleColor = Color.Orange;
            m_CheckBoxAdvDisabledBackColor = Color.FromArgb(238, 238, 238);
            m_CheckBoxAdvDisabledBorderColor = Color.FromArgb(238, 238, 238);
            m_CheckBoxAdvDisabledInternalBorderColor = Color.FromArgb(238, 238, 238);

            // colors for RadioButtonAdv
            m_RadioButtonAdvNormalBackColor = Color.Orange;
            m_RadioButtonAdvNormalBorderColor = Color.Orange;
            m_RadioButtonAdvNormalInternalBorderColor = Color.Orange;
            m_RadioButtonAdvSelectedBackColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvSelectedBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvSelectedInternalBorderColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvPushedBackColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvPushedBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvPushedInternalBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkNormalBottomColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkSelectedBottomColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkPushedBottomColor = Color.FromArgb(0, 0, 0);
        }
        #endregion
    }
    /// <summary>
    /// Provides colors for Teal colorscheme of the Metro visual style.
    /// </summary>
    public class MetroTealColors : MetroColors
    {
        #region Class Overrides
        /// <summary>
        /// Initialize colors for Teal colorscheme of the Metro visual style.
        /// </summary>
        protected override void InitializeColors()
        {
            base.InitializeColors();
            // colors for ButtonAdv 
            m_ButtonPressedTopColor = Color.FromArgb(17, 158, 218);
            m_ButtonPressedBottomColor = Color.FromArgb(17, 158, 218);
            m_ButtonPressedBorderColor = Color.FromArgb(17, 158, 218);

            m_ButtonSelectedTopColor = Color.FromArgb(216, 216, 217);
            m_ButtonSelectedBottomColor = Color.FromArgb(216, 216, 217);
            m_ButtonSelectedBorderColor = Color.FromArgb(216, 216, 217);

            m_ButtonDisabledTopColor = Color.FromArgb(245, 245, 245);
            m_ButtonDisabledBottomColor = Color.FromArgb(245, 245, 245);
            m_ButtonDisabledBorderColor = Color.FromArgb(245, 245, 245);

            m_ButtonDefaultTopColor = Color.Teal;
            m_ButtonDefaultBottomColor = Color.Teal;
            m_ButtonDefaultBorderColor = Color.Teal;

            m_ButtonDefaultInternalBorderColor = Color.Teal;
            m_ButtonPressedInternalBorderColor = Color.FromArgb(17, 158, 218);
            m_ButtonSelectedInternalBorderColor = Color.FromArgb(216, 216, 217);

            //// colors for comboBoxAdv
            m_ComboBoxAdvNormalBackColor = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvNormalBackgroundButtonColor1 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor2 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor3 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor4 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBorderColor = Color.FromArgb(204, 204, 204);

            m_ComboBoxAdvHotBorderColor = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvHotBackColor = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor1 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor2 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor3 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor4 = Color.FromArgb(255, 255, 255);

            m_ComboBoxAdvButtonUpperLineColor = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvArrowColor = Color.FromArgb(51, 51, 51);
            m_ComboBoxAdvLowerArrowLineColor = Color.FromArgb(204, 204, 204);

            m_ComboBoxAdvPushedBackgroundButtonColor1 = Color.Teal;
            m_ComboBoxAdvPushedBackgroundButtonColor2 = Color.Teal;
            m_ComboBoxAdvPushedBackgroundButtonColor3 = Color.Teal;
            m_ComboBoxAdvPushedBackgroundButtonColor4 = Color.Teal;
            m_ComboBoxAdvPushedBorderColor = Color.FromArgb(204, 204, 204);

            //combo background dropdown
            m_borderColorMetro = Color.Teal;
            m_foreColorTopLineMetro = Color.Teal;
            m_foreColorTopFirstMetro = Color.Teal;
            m_foreColorTopLastMetro = Color.Teal;
            m_foreColorBottomFirstMetro = Color.Teal;
            m_foreColorBottomLastMetro = Color.Teal;
            m_foreColorBottomLineMetro = Color.Teal;

            // colors for CheckBoxAdv
            m_CheckBoxAdvNormalBackColor = Color.Teal;
            m_CheckBoxAdvSelectedBackColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedBackColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalBorderColor = Color.Teal;
            m_CheckBoxAdvSelectedBorderColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvPushedBorderColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvNormalInternalBorderColor = Color.FromArgb(253, 253, 253);
            m_CheckBoxAdvSelectedInternalBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalBorderColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalInternalRectangleBorderColor = Color.Teal;
            m_CheckBoxAdvSelectedInternalRectangleBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalRectangleBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvNormalInternalRectangleColor = Color.Teal;
            m_CheckBoxAdvSelectedInternalRectangleColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalRectangleColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvSelectedTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvPushedTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvDisabledTickColor = Color.FromArgb(51, 51, 51);

            m_CheckBoxAdvIndeterminateRectangleColor = Color.Teal;
            m_CheckBoxAdvDisabledBackColor = Color.FromArgb(238, 238, 238);
            m_CheckBoxAdvDisabledBorderColor = Color.FromArgb(238, 238, 238);
            m_CheckBoxAdvDisabledInternalBorderColor = Color.FromArgb(238, 238, 238);

            // colors for RadioButtonAdv
            m_RadioButtonAdvNormalBackColor = Color.Teal;
            m_RadioButtonAdvNormalBorderColor = Color.Teal;
            m_RadioButtonAdvNormalInternalBorderColor = Color.Teal;
            m_RadioButtonAdvSelectedBackColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvSelectedBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvSelectedInternalBorderColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvPushedBackColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvPushedBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvPushedInternalBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkNormalBottomColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkSelectedBottomColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkPushedBottomColor = Color.FromArgb(0, 0, 0);
        }
        #endregion
    }
    /// <summary>
    /// Provides colors for Brown colorscheme of the Metro visual style.
    /// </summary>
    public class MetroBrownColors : MetroColors
    {
        #region Class Overrides
        /// <summary>
        /// Initialize colors for Brown colorscheme of the Metro visual style.
        /// </summary>
        protected override void InitializeColors()
        {
            base.InitializeColors();
            // colors for ButtonAdv 
            m_ButtonPressedTopColor = Color.FromArgb(17, 158, 218);
            m_ButtonPressedBottomColor = Color.FromArgb(17, 158, 218);
            m_ButtonPressedBorderColor = Color.FromArgb(17, 158, 218);

            m_ButtonSelectedTopColor = Color.FromArgb(216, 216, 217);
            m_ButtonSelectedBottomColor = Color.FromArgb(216, 216, 217);
            m_ButtonSelectedBorderColor = Color.FromArgb(216, 216, 217);

            m_ButtonDisabledTopColor = Color.FromArgb(245, 245, 245);
            m_ButtonDisabledBottomColor = Color.FromArgb(245, 245, 245);
            m_ButtonDisabledBorderColor = Color.FromArgb(245, 245, 245);

            m_ButtonDefaultTopColor = Color.Brown;
            m_ButtonDefaultBottomColor = Color.Brown;
            m_ButtonDefaultBorderColor = Color.Brown;

            m_ButtonDefaultInternalBorderColor = Color.Brown;
            m_ButtonPressedInternalBorderColor = Color.FromArgb(17, 158, 218);
            m_ButtonSelectedInternalBorderColor = Color.FromArgb(216, 216, 217);

            //// colors for comboBoxAdv
            m_ComboBoxAdvNormalBackColor = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvNormalBackgroundButtonColor1 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor2 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor3 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor4 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBorderColor = Color.FromArgb(204, 204, 204);

            m_ComboBoxAdvHotBorderColor = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvHotBackColor = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor1 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor2 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor3 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor4 = Color.FromArgb(255, 255, 255);

            m_ComboBoxAdvButtonUpperLineColor = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvArrowColor = Color.FromArgb(51, 51, 51);
            m_ComboBoxAdvLowerArrowLineColor = Color.FromArgb(204, 204, 204);

            m_ComboBoxAdvPushedBackgroundButtonColor1 = Color.Brown;
            m_ComboBoxAdvPushedBackgroundButtonColor2 = Color.Brown;
            m_ComboBoxAdvPushedBackgroundButtonColor3 = Color.Brown;
            m_ComboBoxAdvPushedBackgroundButtonColor4 = Color.Brown;
            m_ComboBoxAdvPushedBorderColor = Color.FromArgb(204, 204, 204);

            //combo background dropdown
            m_borderColorMetro = Color.Brown;
            m_foreColorTopLineMetro = Color.Brown;
            m_foreColorTopFirstMetro = Color.Brown;
            m_foreColorTopLastMetro = Color.Brown;
            m_foreColorBottomFirstMetro = Color.Brown;
            m_foreColorBottomLastMetro = Color.Brown;
            m_foreColorBottomLineMetro = Color.Brown;

            // colors for CheckBoxAdv
            m_CheckBoxAdvNormalBackColor = Color.Brown;
            m_CheckBoxAdvSelectedBackColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedBackColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalBorderColor = Color.Brown;
            m_CheckBoxAdvSelectedBorderColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvPushedBorderColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvNormalInternalBorderColor = Color.FromArgb(253, 253, 253);
            m_CheckBoxAdvSelectedInternalBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalBorderColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalInternalRectangleBorderColor = Color.Brown;
            m_CheckBoxAdvSelectedInternalRectangleBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalRectangleBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvNormalInternalRectangleColor = Color.Brown;
            m_CheckBoxAdvSelectedInternalRectangleColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalRectangleColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvSelectedTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvPushedTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvDisabledTickColor = Color.FromArgb(51, 51, 51);

            m_CheckBoxAdvIndeterminateRectangleColor = Color.Brown;
            m_CheckBoxAdvDisabledBackColor = Color.FromArgb(238, 238, 238);
            m_CheckBoxAdvDisabledBorderColor = Color.FromArgb(238, 238, 238);
            m_CheckBoxAdvDisabledInternalBorderColor = Color.FromArgb(238, 238, 238);

            // colors for RadioButtonAdv
            m_RadioButtonAdvNormalBackColor = Color.Brown;
            m_RadioButtonAdvNormalBorderColor = Color.Brown;
            m_RadioButtonAdvNormalInternalBorderColor = Color.Brown;
            m_RadioButtonAdvSelectedBackColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvSelectedBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvSelectedInternalBorderColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvPushedBackColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvPushedBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvPushedInternalBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkNormalBottomColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkSelectedBottomColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkPushedBottomColor = Color.FromArgb(0, 0, 0);
        }
        #endregion
    }
    /// <summary>
    /// Provides colors for Lime colorscheme of the Metro visual style.
    /// </summary>
    public class MetroLimeColors : MetroColors
    {
        #region Class Overrides
        /// <summary>
        /// Initialize colors for Lime colorscheme of the Metro visual style.
        /// </summary>
        protected override void InitializeColors()
        {
            base.InitializeColors();
            // colors for ButtonAdv 
            m_ButtonPressedTopColor = Color.FromArgb(17, 158, 218);
            m_ButtonPressedBottomColor = Color.FromArgb(17, 158, 218);
            m_ButtonPressedBorderColor = Color.FromArgb(17, 158, 218);

            m_ButtonSelectedTopColor = Color.FromArgb(216, 216, 217);
            m_ButtonSelectedBottomColor = Color.FromArgb(216, 216, 217);
            m_ButtonSelectedBorderColor = Color.FromArgb(216, 216, 217);

            m_ButtonDisabledTopColor = Color.FromArgb(245, 245, 245);
            m_ButtonDisabledBottomColor = Color.FromArgb(245, 245, 245);
            m_ButtonDisabledBorderColor = Color.FromArgb(245, 245, 245);

            m_ButtonDefaultTopColor = Color.Lime;
            m_ButtonDefaultBottomColor = Color.Lime;
            m_ButtonDefaultBorderColor = Color.Lime;

            m_ButtonDefaultInternalBorderColor = Color.Lime;
            m_ButtonPressedInternalBorderColor = Color.FromArgb(17, 158, 218);
            m_ButtonSelectedInternalBorderColor = Color.FromArgb(216, 216, 217);

            //// colors for comboBoxAdv
            m_ComboBoxAdvNormalBackColor = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvNormalBackgroundButtonColor1 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor2 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor3 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor4 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBorderColor = Color.FromArgb(204, 204, 204);

            m_ComboBoxAdvHotBorderColor = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvHotBackColor = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor1 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor2 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor3 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor4 = Color.FromArgb(255, 255, 255);

            m_ComboBoxAdvButtonUpperLineColor = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvArrowColor = Color.FromArgb(51, 51, 51);
            m_ComboBoxAdvLowerArrowLineColor = Color.FromArgb(204, 204, 204);

            m_ComboBoxAdvPushedBackgroundButtonColor1 = Color.Lime;
            m_ComboBoxAdvPushedBackgroundButtonColor2 = Color.Lime;
            m_ComboBoxAdvPushedBackgroundButtonColor3 = Color.Lime;
            m_ComboBoxAdvPushedBackgroundButtonColor4 = Color.Lime;
            m_ComboBoxAdvPushedBorderColor = Color.FromArgb(204, 204, 204);

            //combo background dropdown
            m_borderColorMetro = Color.Lime;
            m_foreColorTopLineMetro = Color.Lime;
            m_foreColorTopFirstMetro = Color.Lime;
            m_foreColorTopLastMetro = Color.Lime;
            m_foreColorBottomFirstMetro = Color.Lime;
            m_foreColorBottomLastMetro = Color.Lime;
            m_foreColorBottomLineMetro = Color.Lime;

            // colors for CheckBoxAdv
            m_CheckBoxAdvNormalBackColor = Color.Lime;
            m_CheckBoxAdvSelectedBackColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedBackColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalBorderColor = Color.Lime;
            m_CheckBoxAdvSelectedBorderColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvPushedBorderColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvNormalInternalBorderColor = Color.FromArgb(253, 253, 253);
            m_CheckBoxAdvSelectedInternalBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalBorderColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalInternalRectangleBorderColor = Color.Lime;
            m_CheckBoxAdvSelectedInternalRectangleBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalRectangleBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvNormalInternalRectangleColor = Color.Lime;
            m_CheckBoxAdvSelectedInternalRectangleColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalRectangleColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvSelectedTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvPushedTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvDisabledTickColor = Color.FromArgb(51, 51, 51);

            m_CheckBoxAdvIndeterminateRectangleColor = Color.Lime;
            m_CheckBoxAdvDisabledBackColor = Color.FromArgb(238, 238, 238);
            m_CheckBoxAdvDisabledBorderColor = Color.FromArgb(238, 238, 238);
            m_CheckBoxAdvDisabledInternalBorderColor = Color.FromArgb(238, 238, 238);

            // colors for RadioButtonAdv
            m_RadioButtonAdvNormalBackColor = Color.Lime;
            m_RadioButtonAdvNormalBorderColor = Color.Lime;
            m_RadioButtonAdvNormalInternalBorderColor = Color.Lime;
            m_RadioButtonAdvSelectedBackColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvSelectedBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvSelectedInternalBorderColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvPushedBackColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvPushedBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvPushedInternalBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkNormalBottomColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkSelectedBottomColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkPushedBottomColor = Color.FromArgb(0, 0, 0);
        }
        #endregion
    }
    /// <summary>
    /// Provides colors for Purple colorscheme of the Metro visual style.
    /// </summary>
    public class MetroPurpleColors : MetroColors
    {
        #region Class Overrides
        /// <summary>
        /// Initialize colors for Purple colorscheme of the Metro visual style.
        /// </summary>
        protected override void InitializeColors()
        {
            base.InitializeColors();
            // colors for ButtonAdv 
            m_ButtonPressedTopColor = Color.FromArgb(17, 158, 218);
            m_ButtonPressedBottomColor = Color.FromArgb(17, 158, 218);
            m_ButtonPressedBorderColor = Color.FromArgb(17, 158, 218);

            m_ButtonSelectedTopColor = Color.FromArgb(216, 216, 217);
            m_ButtonSelectedBottomColor = Color.FromArgb(216, 216, 217);
            m_ButtonSelectedBorderColor = Color.FromArgb(216, 216, 217);

            m_ButtonDisabledTopColor = Color.FromArgb(245, 245, 245);
            m_ButtonDisabledBottomColor = Color.FromArgb(245, 245, 245);
            m_ButtonDisabledBorderColor = Color.FromArgb(245, 245, 245);

            m_ButtonDefaultTopColor = Color.Purple;
            m_ButtonDefaultBottomColor = Color.Purple;
            m_ButtonDefaultBorderColor = Color.Purple;

            m_ButtonDefaultInternalBorderColor = Color.Purple;
            m_ButtonPressedInternalBorderColor = Color.FromArgb(17, 158, 218);
            m_ButtonSelectedInternalBorderColor = Color.FromArgb(216, 216, 217);

            //// colors for comboBoxAdv
            m_ComboBoxAdvNormalBackColor = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvNormalBackgroundButtonColor1 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor2 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor3 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor4 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBorderColor = Color.FromArgb(204, 204, 204);

            m_ComboBoxAdvHotBorderColor = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvHotBackColor = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor1 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor2 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor3 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor4 = Color.FromArgb(255, 255, 255);

            m_ComboBoxAdvButtonUpperLineColor = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvArrowColor = Color.FromArgb(51, 51, 51);
            m_ComboBoxAdvLowerArrowLineColor = Color.FromArgb(204, 204, 204);

            m_ComboBoxAdvPushedBackgroundButtonColor1 = Color.Purple;
            m_ComboBoxAdvPushedBackgroundButtonColor2 = Color.Purple;
            m_ComboBoxAdvPushedBackgroundButtonColor3 = Color.Purple;
            m_ComboBoxAdvPushedBackgroundButtonColor4 = Color.Purple;
            m_ComboBoxAdvPushedBorderColor = Color.FromArgb(204, 204, 204);

            //combo background dropdown
            m_borderColorMetro = Color.Purple;
            m_foreColorTopLineMetro = Color.Purple;
            m_foreColorTopFirstMetro = Color.Purple;
            m_foreColorTopLastMetro = Color.Purple;
            m_foreColorBottomFirstMetro = Color.Purple;
            m_foreColorBottomLastMetro = Color.Purple;
            m_foreColorBottomLineMetro = Color.Purple;

            // colors for CheckBoxAdv
            m_CheckBoxAdvNormalBackColor = Color.Purple;
            m_CheckBoxAdvSelectedBackColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedBackColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalBorderColor = Color.Purple;
            m_CheckBoxAdvSelectedBorderColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvPushedBorderColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvNormalInternalBorderColor = Color.FromArgb(253, 253, 253);
            m_CheckBoxAdvSelectedInternalBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalBorderColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalInternalRectangleBorderColor = Color.Purple;
            m_CheckBoxAdvSelectedInternalRectangleBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalRectangleBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvNormalInternalRectangleColor = Color.Purple;
            m_CheckBoxAdvSelectedInternalRectangleColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalRectangleColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvSelectedTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvPushedTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvDisabledTickColor = Color.FromArgb(51, 51, 51);

            m_CheckBoxAdvIndeterminateRectangleColor = Color.Purple;
            m_CheckBoxAdvDisabledBackColor = Color.FromArgb(238, 238, 238);
            m_CheckBoxAdvDisabledBorderColor = Color.FromArgb(238, 238, 238);
            m_CheckBoxAdvDisabledInternalBorderColor = Color.FromArgb(238, 238, 238);

            // colors for RadioButtonAdv
            m_RadioButtonAdvNormalBackColor = Color.Purple;
            m_RadioButtonAdvNormalBorderColor = Color.Purple;
            m_RadioButtonAdvNormalInternalBorderColor = Color.Purple;
            m_RadioButtonAdvSelectedBackColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvSelectedBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvSelectedInternalBorderColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvPushedBackColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvPushedBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvPushedInternalBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkNormalBottomColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkSelectedBottomColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkPushedBottomColor = Color.FromArgb(0, 0, 0);
        }
        #endregion
    }
    /// <summary>
    /// Provides colors for Pink colorscheme of the Metro visual style.
    /// </summary>
    public class MetroPinkColors : MetroColors
    {
        #region Class Overrides
        /// <summary>
        /// Initialize colors for Pink colorscheme of the Metro visual style.
        /// </summary>
        protected override void InitializeColors()
        {
            base.InitializeColors();
            // colors for ButtonAdv 
            m_ButtonPressedTopColor = Color.FromArgb(17, 158, 218);
            m_ButtonPressedBottomColor = Color.FromArgb(17, 158, 218);
            m_ButtonPressedBorderColor = Color.FromArgb(17, 158, 218);

            m_ButtonSelectedTopColor = Color.FromArgb(216, 216, 217);
            m_ButtonSelectedBottomColor = Color.FromArgb(216, 216, 217);
            m_ButtonSelectedBorderColor = Color.FromArgb(216, 216, 217);

            m_ButtonDisabledTopColor = Color.FromArgb(245, 245, 245);
            m_ButtonDisabledBottomColor = Color.FromArgb(245, 245, 245);
            m_ButtonDisabledBorderColor = Color.FromArgb(245, 245, 245);

            m_ButtonDefaultTopColor = Color.Pink;
            m_ButtonDefaultBottomColor = Color.Pink;
            m_ButtonDefaultBorderColor = Color.Pink;

            m_ButtonDefaultInternalBorderColor = Color.Pink;
            m_ButtonPressedInternalBorderColor = Color.FromArgb(17, 158, 218);
            m_ButtonSelectedInternalBorderColor = Color.FromArgb(216, 216, 217);

            //// colors for comboBoxAdv
            m_ComboBoxAdvNormalBackColor = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvNormalBackgroundButtonColor1 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor2 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor3 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor4 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBorderColor = Color.FromArgb(204, 204, 204);

            m_ComboBoxAdvHotBorderColor = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvHotBackColor = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor1 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor2 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor3 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor4 = Color.FromArgb(255, 255, 255);

            m_ComboBoxAdvButtonUpperLineColor = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvArrowColor = Color.FromArgb(51, 51, 51);
            m_ComboBoxAdvLowerArrowLineColor = Color.FromArgb(204, 204, 204);

            m_ComboBoxAdvPushedBackgroundButtonColor1 = Color.Pink;
            m_ComboBoxAdvPushedBackgroundButtonColor2 = Color.Pink;
            m_ComboBoxAdvPushedBackgroundButtonColor3 = Color.Pink;
            m_ComboBoxAdvPushedBackgroundButtonColor4 = Color.Pink;
            m_ComboBoxAdvPushedBorderColor = Color.FromArgb(204, 204, 204);

            //combo background dropdown
            m_borderColorMetro = Color.Pink;
            m_foreColorTopLineMetro = Color.Pink;
            m_foreColorTopFirstMetro = Color.Pink;
            m_foreColorTopLastMetro = Color.Pink;
            m_foreColorBottomFirstMetro = Color.Pink;
            m_foreColorBottomLastMetro = Color.Pink;
            m_foreColorBottomLineMetro = Color.Pink;

            // colors for CheckBoxAdv
            m_CheckBoxAdvNormalBackColor = Color.Pink;
            m_CheckBoxAdvSelectedBackColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedBackColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalBorderColor = Color.Pink;
            m_CheckBoxAdvSelectedBorderColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvPushedBorderColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvNormalInternalBorderColor = Color.FromArgb(253, 253, 253);
            m_CheckBoxAdvSelectedInternalBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalBorderColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalInternalRectangleBorderColor = Color.Pink;
            m_CheckBoxAdvSelectedInternalRectangleBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalRectangleBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvNormalInternalRectangleColor = Color.Pink;
            m_CheckBoxAdvSelectedInternalRectangleColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalRectangleColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvSelectedTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvPushedTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvDisabledTickColor = Color.FromArgb(51, 51, 51);

            m_CheckBoxAdvIndeterminateRectangleColor = Color.Pink;
            m_CheckBoxAdvDisabledBackColor = Color.FromArgb(238, 238, 238);
            m_CheckBoxAdvDisabledBorderColor = Color.FromArgb(238, 238, 238);
            m_CheckBoxAdvDisabledInternalBorderColor = Color.FromArgb(238, 238, 238);

            // colors for RadioButtonAdv
            m_RadioButtonAdvNormalBackColor = Color.Pink;
            m_RadioButtonAdvNormalBorderColor = Color.Pink;
            m_RadioButtonAdvNormalInternalBorderColor = Color.Pink;
            m_RadioButtonAdvSelectedBackColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvSelectedBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvSelectedInternalBorderColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvPushedBackColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvPushedBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvPushedInternalBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkNormalBottomColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkSelectedBottomColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkPushedBottomColor = Color.FromArgb(0, 0, 0);
        }
        #endregion
    }
    /// <summary>
    /// Provides colors for Blue colorscheme of the Metro visual style.
    /// </summary>
    public class MetroBlueColors : MetroColors
    {
        #region Class Overrides
        /// <summary>
        /// Initialize colors for Blue colorscheme of the Metro visual style.
        /// </summary>
        protected override void InitializeColors()
        {
            base.InitializeColors();
            // colors for ButtonAdv 
            m_ButtonPressedTopColor = Color.FromArgb(17, 158, 218);
            m_ButtonPressedBottomColor = Color.FromArgb(17, 158, 218);
            m_ButtonPressedBorderColor = Color.FromArgb(17, 158, 218);

            m_ButtonSelectedTopColor = Color.FromArgb(216, 216, 217);
            m_ButtonSelectedBottomColor = Color.FromArgb(216, 216, 217);
            m_ButtonSelectedBorderColor = Color.FromArgb(216, 216, 217);

            m_ButtonDisabledTopColor = Color.FromArgb(245, 245, 245);
            m_ButtonDisabledBottomColor = Color.FromArgb(245, 245, 245);
            m_ButtonDisabledBorderColor = Color.FromArgb(245, 245, 245);

            m_ButtonDefaultTopColor = Color.Blue;
            m_ButtonDefaultBottomColor = Color.Blue;
            m_ButtonDefaultBorderColor = Color.Blue;

            m_ButtonDefaultInternalBorderColor = Color.Blue;
            m_ButtonPressedInternalBorderColor = Color.FromArgb(17, 158, 218);
            m_ButtonSelectedInternalBorderColor = Color.FromArgb(216, 216, 217);

            //// colors for comboBoxAdv
            m_ComboBoxAdvNormalBackColor = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvNormalBackgroundButtonColor1 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor2 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor3 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor4 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBorderColor = Color.FromArgb(204, 204, 204);

            m_ComboBoxAdvHotBorderColor = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvHotBackColor = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor1 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor2 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor3 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor4 = Color.FromArgb(255, 255, 255);

            m_ComboBoxAdvButtonUpperLineColor = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvArrowColor = Color.FromArgb(51, 51, 51);
            m_ComboBoxAdvLowerArrowLineColor = Color.FromArgb(204, 204, 204);

            m_ComboBoxAdvPushedBackgroundButtonColor1 = Color.Blue;
            m_ComboBoxAdvPushedBackgroundButtonColor2 = Color.Blue;
            m_ComboBoxAdvPushedBackgroundButtonColor3 = Color.Blue;
            m_ComboBoxAdvPushedBackgroundButtonColor4 = Color.Blue;
            m_ComboBoxAdvPushedBorderColor = Color.FromArgb(204, 204, 204);

            //combo background dropdown
            m_borderColorMetro = Color.Blue;
            m_foreColorTopLineMetro = Color.Blue;
            m_foreColorTopFirstMetro = Color.Blue;
            m_foreColorTopLastMetro = Color.Blue;
            m_foreColorBottomFirstMetro = Color.Blue;
            m_foreColorBottomLastMetro = Color.Blue;
            m_foreColorBottomLineMetro = Color.Blue;

            // colors for CheckBoxAdv
            m_CheckBoxAdvNormalBackColor = Color.Blue;
            m_CheckBoxAdvSelectedBackColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedBackColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalBorderColor = Color.Blue;
            m_CheckBoxAdvSelectedBorderColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvPushedBorderColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvNormalInternalBorderColor = Color.FromArgb(253, 253, 253);
            m_CheckBoxAdvSelectedInternalBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalBorderColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalInternalRectangleBorderColor = Color.Blue;
            m_CheckBoxAdvSelectedInternalRectangleBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalRectangleBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvNormalInternalRectangleColor = Color.Blue;
            m_CheckBoxAdvSelectedInternalRectangleColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalRectangleColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvSelectedTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvPushedTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvDisabledTickColor = Color.FromArgb(51, 51, 51);

            m_CheckBoxAdvIndeterminateRectangleColor = Color.Blue;
            m_CheckBoxAdvDisabledBackColor = Color.FromArgb(238, 238, 238);
            m_CheckBoxAdvDisabledBorderColor = Color.FromArgb(238, 238, 238);
            m_CheckBoxAdvDisabledInternalBorderColor = Color.FromArgb(238, 238, 238);

            // colors for RadioButtonAdv
            m_RadioButtonAdvNormalBackColor = Color.Blue;
            m_RadioButtonAdvNormalBorderColor = Color.Blue;
            m_RadioButtonAdvNormalInternalBorderColor = Color.Blue;
            m_RadioButtonAdvSelectedBackColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvSelectedBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvSelectedInternalBorderColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvPushedBackColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvPushedBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvPushedInternalBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkNormalBottomColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkSelectedBottomColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkPushedBottomColor = Color.FromArgb(0, 0, 0);
        }
        #endregion
    }
    /// <summary>
    /// Provides colors for Red colorscheme of the Metro visual style.
    /// </summary>
    public class MetroRedColors : MetroColors
    {
        #region Class Overrides
        /// <summary>
        /// Initialize colors for Red colorscheme of the Metro visual style.
        /// </summary>
        protected override void InitializeColors()
        {
            base.InitializeColors();
            // colors for ButtonAdv 
            m_ButtonPressedTopColor = Color.FromArgb(17, 158, 218);
            m_ButtonPressedBottomColor = Color.FromArgb(17, 158, 218);
            m_ButtonPressedBorderColor = Color.FromArgb(17, 158, 218);

            m_ButtonSelectedTopColor = Color.FromArgb(216, 216, 217);
            m_ButtonSelectedBottomColor = Color.FromArgb(216, 216, 217);
            m_ButtonSelectedBorderColor = Color.FromArgb(216, 216, 217);

            m_ButtonDisabledTopColor = Color.FromArgb(245, 245, 245);
            m_ButtonDisabledBottomColor = Color.FromArgb(245, 245, 245);
            m_ButtonDisabledBorderColor = Color.FromArgb(245, 245, 245);

            m_ButtonDefaultTopColor = Color.Red;
            m_ButtonDefaultBottomColor = Color.Red;
            m_ButtonDefaultBorderColor = Color.Red;

            m_ButtonDefaultInternalBorderColor = Color.Red;
            m_ButtonPressedInternalBorderColor = Color.FromArgb(17, 158, 218);
            m_ButtonSelectedInternalBorderColor = Color.FromArgb(216, 216, 217);

            //// colors for comboBoxAdv
            m_ComboBoxAdvNormalBackColor = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvNormalBackgroundButtonColor1 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor2 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor3 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor4 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBorderColor = Color.FromArgb(204, 204, 204);

            m_ComboBoxAdvHotBorderColor = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvHotBackColor = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor1 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor2 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor3 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor4 = Color.FromArgb(255, 255, 255);

            m_ComboBoxAdvButtonUpperLineColor = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvArrowColor = Color.FromArgb(51, 51, 51);
            m_ComboBoxAdvLowerArrowLineColor = Color.FromArgb(204, 204, 204);

            m_ComboBoxAdvPushedBackgroundButtonColor1 = Color.Red;
            m_ComboBoxAdvPushedBackgroundButtonColor2 = Color.Red;
            m_ComboBoxAdvPushedBackgroundButtonColor3 = Color.Red;
            m_ComboBoxAdvPushedBackgroundButtonColor4 = Color.Red;
            m_ComboBoxAdvPushedBorderColor = Color.FromArgb(204, 204, 204);

            //combo background dropdown
            m_borderColorMetro = Color.Red;
            m_foreColorTopLineMetro = Color.Red;
            m_foreColorTopFirstMetro = Color.Red;
            m_foreColorTopLastMetro = Color.Red;
            m_foreColorBottomFirstMetro = Color.Red;
            m_foreColorBottomLastMetro = Color.Red;
            m_foreColorBottomLineMetro = Color.Red;

            // colors for CheckBoxAdv
            m_CheckBoxAdvNormalBackColor = Color.Red;
            m_CheckBoxAdvSelectedBackColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedBackColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalBorderColor = Color.Red;
            m_CheckBoxAdvSelectedBorderColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvPushedBorderColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvNormalInternalBorderColor = Color.FromArgb(253, 253, 253);
            m_CheckBoxAdvSelectedInternalBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalBorderColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalInternalRectangleBorderColor = Color.Red;
            m_CheckBoxAdvSelectedInternalRectangleBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalRectangleBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvNormalInternalRectangleColor = Color.Red;
            m_CheckBoxAdvSelectedInternalRectangleColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalRectangleColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvSelectedTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvPushedTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvDisabledTickColor = Color.FromArgb(51, 51, 51);

            m_CheckBoxAdvIndeterminateRectangleColor = Color.Red;
            m_CheckBoxAdvDisabledBackColor = Color.FromArgb(238, 238, 238);
            m_CheckBoxAdvDisabledBorderColor = Color.FromArgb(238, 238, 238);
            m_CheckBoxAdvDisabledInternalBorderColor = Color.FromArgb(238, 238, 238);

            // colors for RadioButtonAdv
            m_RadioButtonAdvNormalBackColor = Color.Red;
            m_RadioButtonAdvNormalBorderColor = Color.Red;
            m_RadioButtonAdvNormalInternalBorderColor = Color.Red;
            m_RadioButtonAdvSelectedBackColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvSelectedBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvSelectedInternalBorderColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvPushedBackColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvPushedBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvPushedInternalBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkNormalBottomColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkSelectedBottomColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkPushedBottomColor = Color.FromArgb(0, 0, 0);
        }
        #endregion
    }
    /// <summary>
    /// Provides colors for Green colorscheme of the Metro visual style.
    /// </summary>
    public class MetroGreenColors : MetroColors
    {
        #region Class Overrides
        /// <summary>
        /// Initialize colors for Green colorscheme of the Metro visual style.
        /// </summary>
        protected override void InitializeColors()
        {
            base.InitializeColors();
            // colors for ButtonAdv 
            m_ButtonPressedTopColor = Color.FromArgb(17, 158, 218);
            m_ButtonPressedBottomColor = Color.FromArgb(17, 158, 218);
            m_ButtonPressedBorderColor = Color.FromArgb(17, 158, 218);

            m_ButtonSelectedTopColor = Color.FromArgb(216, 216, 217);
            m_ButtonSelectedBottomColor = Color.FromArgb(216, 216, 217);
            m_ButtonSelectedBorderColor = Color.FromArgb(216, 216, 217);

            m_ButtonDisabledTopColor = Color.FromArgb(245, 245, 245);
            m_ButtonDisabledBottomColor = Color.FromArgb(245, 245, 245);
            m_ButtonDisabledBorderColor = Color.FromArgb(245, 245, 245);

            m_ButtonDefaultTopColor = Color.Green;
            m_ButtonDefaultBottomColor = Color.Green;
            m_ButtonDefaultBorderColor = Color.Green;

            m_ButtonDefaultInternalBorderColor = Color.Green;
            m_ButtonPressedInternalBorderColor = Color.FromArgb(17, 158, 218);
            m_ButtonSelectedInternalBorderColor = Color.FromArgb(216, 216, 217);

            //// colors for comboBoxAdv
            m_ComboBoxAdvNormalBackColor = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvNormalBackgroundButtonColor1 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor2 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor3 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBackgroundButtonColor4 = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvNormalBorderColor = Color.FromArgb(204, 204, 204);

            m_ComboBoxAdvHotBorderColor = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvHotBackColor = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor1 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor2 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor3 = Color.FromArgb(255, 255, 255);
            m_ComboBoxAdvHotBackgroundButtonColor4 = Color.FromArgb(255, 255, 255);

            m_ComboBoxAdvButtonUpperLineColor = Color.FromArgb(204, 204, 204);
            m_ComboBoxAdvArrowColor = Color.FromArgb(51, 51, 51);
            m_ComboBoxAdvLowerArrowLineColor = Color.FromArgb(204, 204, 204);

            m_ComboBoxAdvPushedBackgroundButtonColor1 = Color.Green;
            m_ComboBoxAdvPushedBackgroundButtonColor2 = Color.Green;
            m_ComboBoxAdvPushedBackgroundButtonColor3 = Color.Green;
            m_ComboBoxAdvPushedBackgroundButtonColor4 = Color.Green;
            m_ComboBoxAdvPushedBorderColor = Color.FromArgb(204, 204, 204);

            //combo background dropdown
            m_borderColorMetro = Color.Green;
            m_foreColorTopLineMetro = Color.Green;
            m_foreColorTopFirstMetro = Color.Green;
            m_foreColorTopLastMetro = Color.Green;
            m_foreColorBottomFirstMetro = Color.Green;
            m_foreColorBottomLastMetro = Color.Green;
            m_foreColorBottomLineMetro = Color.Green;

            // colors for CheckBoxAdv
            m_CheckBoxAdvNormalBackColor = Color.Green;
            m_CheckBoxAdvSelectedBackColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedBackColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalBorderColor = Color.Green;
            m_CheckBoxAdvSelectedBorderColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvPushedBorderColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvNormalInternalBorderColor = Color.FromArgb(253, 253, 253);
            m_CheckBoxAdvSelectedInternalBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalBorderColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalInternalRectangleBorderColor = Color.Green;
            m_CheckBoxAdvSelectedInternalRectangleBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalRectangleBorderColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvNormalInternalRectangleColor = Color.Green;
            m_CheckBoxAdvSelectedInternalRectangleColor = Color.FromArgb(236, 236, 236);
            m_CheckBoxAdvPushedInternalRectangleColor = Color.FromArgb(236, 236, 236);

            m_CheckBoxAdvNormalTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvSelectedTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvPushedTickColor = Color.FromArgb(51, 51, 51);
            m_CheckBoxAdvDisabledTickColor = Color.FromArgb(51, 51, 51);

            m_CheckBoxAdvIndeterminateRectangleColor = Color.Green;
            m_CheckBoxAdvDisabledBackColor = Color.FromArgb(238, 238, 238);
            m_CheckBoxAdvDisabledBorderColor = Color.FromArgb(238, 238, 238);
            m_CheckBoxAdvDisabledInternalBorderColor = Color.FromArgb(238, 238, 238);

            // colors for RadioButtonAdv
            m_RadioButtonAdvNormalBackColor = Color.Green;
            m_RadioButtonAdvNormalBorderColor = Color.Green;
            m_RadioButtonAdvNormalInternalBorderColor = Color.Green;
            m_RadioButtonAdvSelectedBackColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvSelectedBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvSelectedInternalBorderColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvPushedBackColor = Color.FromArgb(243, 243, 243);
            m_RadioButtonAdvPushedBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvPushedInternalBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkBorderColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkNormalBottomColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkSelectedBottomColor = Color.FromArgb(0, 0, 0);
            m_RadioButtonAdvCheckMarkPushedBottomColor = Color.FromArgb(0, 0, 0);
        }
        #endregion
    }
        #endregion
    public enum Office2007Theme
	{
		Blue = 0,
		Silver,
		Black,
		Managed = -1
	}
    public enum Office2010Theme
    {
        Blue = 0,
        Silver,
        Black,
        Managed = -1
    }
    public enum MetroTheme
    {
        Magenta = 0,
        Purple,
        Teal,
        Lime,
        Brown,
        Pink,
        Orange,
        Blue,
        Red,
        Green,
        Managed = -1
    }
	/// <summary>
	/// Specifies that this object supports <see cref="Office2007Theme"/>.
	/// </summary>
	public interface ISupportOffice2007Theme
	{
		/// <summary>
		/// Specifies <see cref="Office2007Theme"/> to use.
		/// </summary>
		Office2007Theme Office2007ColorTheme { get; set; }

		/// <summary>
		/// Enables rendering with <see cref="Office2007Theme"/>.
		/// </summary>
		void EnableOffice2007Style();
	}
    /// <summary>
    /// Specifies that this object supports <see cref="Office2007Theme"/>.
    /// </summary>
    public interface ISupportOffice2010Theme
    {
        /// <summary>
        /// Specifies <see cref="Office2007Theme"/> to use.
        /// </summary>
        Office2010Theme Office2010ColorTheme { get; set; }

        /// <summary>
        /// Enables rendering with <see cref="Office2007Theme"/>.
        /// </summary>
        void EnableOffice2010Style();
    }
    /// <summary>
    /// Specifies that this object supports <see cref="MetroTheme"/>.
    /// </summary>
    public interface ISupportMetroTheme
    {
        /// <summary>
        /// Specifies <see cref="MetroTheme"/> to use.
        /// </summary>
        MetroTheme MetroColorTheme { get; set; }

        /// <summary>
        /// Enables rendering with <see cref="MetroTheme"/>.
        /// </summary>
        void EnableMetroStyle();
    }
	public enum VS2005Theme
	{
		Blue,
		Silver,
		Olive
	}
}
