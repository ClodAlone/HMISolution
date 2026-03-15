#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Resources;
using System.Globalization;
using System.Diagnostics;
using System.Collections;
using System.Threading;

// Tip:
// To simplify rebuilding this class while working on your project
// you should add the following External Tool in Developer Studio:
// 
//     Title: Resgen SR.TXT
//     Command: C:\WINNT\system32\CMD.EXE
//     Arguments: /c resgen SR.txt & srgen @srgen.ini & echo Done.
//     Initial Directory: $(ProjectDir)
//     Please enable "X Use Output for Window."
// 
// Additionally you should add a file srgen.ini to project that 
// contains command line arguments for your project.
//
// Example: 
// SRGen.ini
// sr.txt namespace:Samples.SRGenSample classname:SR

namespace Syncfusion.Windows.Forms.Localization
{
    /// <summary>
    ///    SR provides localized access to string resources specific 
    ///    from the assembly manifest Syncfusion.Windows.Forms.Localization.SR.resources
    /// </summary>
	sealed class SR 
	{
		// Fields
		private ResourceManager resources;
		private static SR loader = null;

		// Strings 
		internal const string Bottom = "Bottom";
		internal const string CategoryAction = "CategoryAction";
		internal const string CategoryAppearance = "CategoryAppearance";
		internal const string CategoryBehavior = "CategoryBehavior";
		internal const string CategoryData = "CategoryData";
		internal const string ColorEditorPaletteTab = "ColorEditorPaletteTab";
		internal const string ColorEditorStandardTab = "ColorEditorStandardTab";
		internal const string ColorEditorSystemTab = "ColorEditorSystemTab";
		internal const string Copy = "Copy";
		internal const string Cut = "Cut";
		internal const string Delete = "Delete";
		internal const string DescriptionMenuItemText = "DescriptionMenuItemText";
		internal const string Error = "Error";
		internal const string FewerButtonsItemsText = "FewerButtonsItemsText";
		internal const string GroupBarAddRemoveButton = "GroupBarAddRemoveButton";
		internal const string GroupBarDropDownToolTip = "GroupBarDropDownToolTip";
		internal const string GroupBarExpandButtonToolTip = "GroupBarExpandButtonToolTip";
		internal const string GroupBarMinimizeButtonToolTip = "GroupBarMinimizeButtonToolTip";
		internal const string GroupBarNavigationPaneTooltip = "GroupBarNavigationPaneTooltip";
		internal const string InvalidArgument = "InvalidArgument";
		internal const string InvalidLowBoundArgumentEx = "InvalidLowBoundArgumentEx";
		internal const string InvalidLowBoundArgumentEx2 = "InvalidLowBoundArgumentEx2";
		internal const string LeftEdge = "LeftEdge";
		internal const string ModName = "ModName";
		internal const string MoreButtonsItemsText = "MoreButtonsItemsText";
		internal const string PageDown = "PageDown";
		internal const string PageLeft = "PageLeft";
		internal const string PageRight = "PageRight";
		internal const string PageUp = "PageUp";
		internal const string Paste = "Paste";
		internal const string RecordNavigationBarAddNewRecord = "RecordNavigationBarAddNewRecord";
		internal const string RecordNavigationBarCurrentRecord = "RecordNavigationBarCurrentRecord";
		internal const string RecordNavigationBarFirstRecord = "RecordNavigationBarFirstRecord";
		internal const string RecordNavigationBarLastRecord = "RecordNavigationBarLastRecord";
		internal const string RecordNavigationBarNextRecord = "RecordNavigationBarNextRecord";
		internal const string RecordNavigationBarPreviousRecord = "RecordNavigationBarPreviousRecord";
		internal const string RecordNavigationBarRecord = "RecordNavigationBarRecord";
		internal const string RecordNavigationBarRecordCount = "RecordNavigationBarRecordCount";
		internal const string ResetMenuItemText = "ResetMenuItemText";
		internal const string RightEdge = "RightEdge";
		internal const string ScrollDown = "ScrollDown";
		internal const string ScrollHere = "ScrollHere";
		internal const string ScrollLeft = "ScrollLeft";
		internal const string ScrollRight = "ScrollRight";
		internal const string ScrollUp = "ScrollUp";
		internal const string SelectAll = "SelectAll";
		internal const string StyleCategoryAppearance = "StyleCategoryAppearance";
		internal const string TabBarControlAdd = "TabBarControlAdd";
		internal const string TabBarControlRemove = "TabBarControlRemove";
		internal const string TabBarPageOnTabBarPage = "TabBarPageOnTabBarPage";
		internal const string TabBarSplitterControlAdd = "TabBarSplitterControlAdd";
		internal const string TabBarSplitterControlRemove = "TabBarSplitterControlRemove";
		internal const string TabBarToolTipEmptyIcon = "TabBarToolTipEmptyIcon";
		internal const string Test = "Test";
		internal const string TextParseFailedFormat = "TextParseFailedFormat";
		internal const string Top = "Top";
		internal const string TopLeftBottomRight = "TopLeftBottomRight";
		internal const string Undo = "Undo";
        internal const string OK = "OK";
        internal const string Cancel = "Cancel";
        internal const string Abort = "Abort";
        internal const string Retry ="Retry";
        internal const string Ignore= "Ignore";
        internal const string Yes = "Yes";
        internal const string No = "No";
        internal const string Close = "Close";
        internal const string Help = "Help";
        internal const string TryAgain = "TryAgain";
        internal const string Continue = "Continue";
        internal const string ActiveBorder = "ActiveBorder";
        internal const string ActiveCaption  = "ActiveCaption";
        internal const string ActiveCaptionText  = "ActiveCaptionText";
        internal const string AppWorkspace  = "AppWorkspace";
        internal const string ButtonFace  = "ButtonFace";
        internal const string ButtonHighlight  = "ButtonHighlight";
        internal const string ButtonShadow  = "ButtonShadow";
        internal const string Control  = "Control";
        internal const string ControlDark  = "ControlDark";
        internal const string ControlDarkDark  = "ControlDarkDark";
        internal const string ControlLight  = "ControlLight";
        internal const string ControlLightLight  = "ControlLightLight";
        internal const string ControlText  = "ControlText";
        internal const string Desktop  = "Desktop";
        internal const string GradientActiveCaption  = "GradientActiveCaption";
        internal const string GradientInactiveCaption  = "GradientInactiveCaption";
        internal const string GrayText  = "GrayText";
        internal const string Highlight  = "Highlight";
        internal const string HighlightText  = "HighlightText";
        internal const string HotTrack  = "HotTrack";
        internal const string InactiveBorder  = "InactiveBorder";
        internal const string InactiveCaption  = "InactiveCaption";
        internal const string InactiveCaptionText  = "InactiveCaptionText";
        internal const string Info  = "Info";
        internal const string InfoText  = "InfoText";
        internal const string Menu  = "Menu";
        internal const string MenuBar  = "MenuBar";
        internal const string MenuHighlight  = "MenuHighlight";
        internal const string MenuText  = "MenuText";
        internal const string ScrollBar  = "ScrollBar";
        internal const string Window  = "Window";
        internal const string WindowFrame  = "WindowFrame";
        internal const string WindowText  = "WindowText";
        internal const string Transparent = "Transparent";  
        internal const string AliceBlue  = "AliceBlue";
        internal const string AntiqueWhite  = "AntiqueWhite";
        internal const string Aqua  = "Aqua";
        internal const string Aquamarine  = "Aquamarine";
        internal const string Azure  = "Azure";
        internal const string Beige  = "Beige";
        internal const string Bisque  = "Bisque";
        internal const string Black  = "Black";
        internal const string BlanchedAlmond  = "BlanchedAlmond";
        internal const string Blue  = "Blue";
        internal const string BlueViolet  = "BlueViolet";
        internal const string Brown  = "Brown";
        internal const string BurlyWood  = "BurlyWood";
        internal const string CadetBlue  = "CadetBlue";
        internal const string Chartreuse  = "Chartreuse";
        internal const string Chocolate  = "Chocolate";
        internal const string Coral  = "Coral";
        internal const string CornflowerBlue = "CornflowerBlue";
        internal const string Cornsilk  = "Cornsilk";
        internal const string Crimson  = "Crimson";
        internal const string Cyan  = "Cyan";
        internal const string DarkBlue  = "DarkBlue";
        internal const string DarkCyan  = "DarkCyan";
        internal const string DarkGoldenrod  = "DarkGoldenrod";
        internal const string DarkGray  = "DarkGray";
        internal const string DarkGreen  = "DarkGreen";
        internal const string DarkKhaki  = "DarkKhaki";
        internal const string DarkMagenta  = "DarkMagenta";
        internal const string DarkOliveGreen  = "DarkOliveGreen";
        internal const string DarkOrange  = "DarkOrange";
        internal const string DarkOrchid  = "DarkOrchid";
        internal const string DarkRed  = "DarkRed";
        internal const string DarkSalmon  = "DarkSalmon";
        internal const string DarkSeaGreen  = "DarkSeaGreen";
        internal const string DarkSlateBlue  = "DarkSlateBlue";
        internal const string DarkSlateGray  = "DarkSlateGray";
        internal const string DarkTurquoise  = "DarkTurquoise";
        internal const string DarkViolet  = "DarkViolet";
        internal const string DeepPink  = "DeepPink";
        internal const string DeepSkyBlue  = "DeepSkyBlue";
        internal const string DimGray  = "DimGray";
        internal const string DodgerBlue  = "DodgerBlue";
        internal const string Firebrick  = "Firebrick";
        internal const string FloralWhite  = "FloralWhite";
        internal const string ForestGreen  = "ForestGreen";
        internal const string Fuchsia  = "Fuchsia";
        internal const string Gainsboro  = "Gainsboro";
        internal const string GhostWhite  = "GhostWhite";
        internal const string Gold  = "Gold";
        internal const string Goldenrod  = "Goldenrod";
        internal const string Gray  = "Gray";
        internal const string Green  = "Green";
        internal const string GreenYellow  = "GreenYellow";
        internal const string Honeydew  = "Honeydew";
        internal const string HotPink  = "HotPink";
        internal const string IndianRed  = "IndianRed";
        internal const string Indigo  = "Indigo";
        internal const string Ivory  = "Ivory";
        internal const string Khaki  = "Khaki";
        internal const string Lavender  = "Lavender";
        internal const string LavenderBlush  = "LavenderBlush";
        internal const string LawnGreen  = "LawnGreen";
        internal const string LemonChiffon  = "LemonChiffon";
        internal const string LightBlue  = "LightBlue";
        internal const string LightCoral  = "LightCoral";
        internal const string LightCyan  = "LightCyan";
        internal const string LightGoldenrodYellow  = "LightGoldenrodYellow";
        internal const string LightGreen  = "LightGreen";
        internal const string LightGray  = "LightGray";
        internal const string LightPink  = "LightPink";
        internal const string LightSalmon  = "LightSalmon";
        internal const string LightSeaGreen  = "LightSeaGreen";
        internal const string LightSkyBlue  = "LightSkyBlue";
        internal const string LightSlateGray  = "LightSlateGray";
        internal const string LightSteelBlue  = "LightSteelBlue";
        internal const string LightYellow  = "LightYellow";
        internal const string Lime  = "Lime";
        internal const string LimeGreen  = "LimeGreen";
        internal const string Linen  = "Linen";
        internal const string Magenta  = "Magenta";
        internal const string Maroon  = "Maroon";
        internal const string MediumAquamarine  = "MediumAquamarine";
        internal const string MediumBlue  = "MediumBlue";
        internal const string MediumOrchid  = "MediumOrchid";
        internal const string MediumPurple  = "MediumPurple";
        internal const string MediumSeaGreen  = "MediumSeaGreen";
        internal const string MediumSlateBlue  = "MediumSlateBlue";
        internal const string MediumSpringGreen  = "MediumSpringGreen";
        internal const string MediumTurquoise  = "MediumTurquoise";
        internal const string MediumVioletRed  = "MediumVioletRed";
        internal const string MidnightBlue  = "MidnightBlue";
        internal const string MintCream  = "MintCream";
        internal const string MistyRose  = "MistyRose";
        internal const string Moccasin  = "Moccasin";
        internal const string NavajoWhite  = "NavajoWhite";
        internal const string Navy  = "Navy";
        internal const string OldLace  = "OldLace";
        internal const string Olive  = "Olive";
        internal const string OliveDrab  = "OliveDrab";
        internal const string Orange  = "Orange";
        internal const string OrangeRed  = "OrangeRed";
        internal const string Orchid  = "Orchid";
        internal const string PaleGoldenrod  = "PaleGoldenrod";
        internal const string PaleGreen  = "PaleGreen";
        internal const string PaleTurquoise  = "PaleTurquoise";
        internal const string PaleVioletRed  = "PaleVioletRed";
        internal const string PapayaWhip  = "PapayaWhip";
        internal const string PeachPuff  = "PeachPuff";
        internal const string Peru  = "Peru";
        internal const string Pink  = "Pink";
        internal const string Plum  = "Plum";
        internal const string PowderBlue  = "PowderBlue";
        internal const string Purple  = "Purple";
        internal const string Red  = "Red";
        internal const string RosyBrown  = "RosyBrown";
        internal const string RoyalBlue  = "RoyalBlue";
        internal const string SaddleBrown  = "SaddleBrown";
        internal const string Salmon  = "Salmon";
        internal const string SandyBrown  = "SandyBrown";
        internal const string SeaGreen  = "SeaGreen";
        internal const string SeaShell  = "SeaShell";
        internal const string Sienna  = "Sienna";
        internal const string Silver  = "Silver";
        internal const string SkyBlue  = "SkyBlue";
        internal const string SlateBlue  = "SlateBlue";
        internal const string SlateGray  = "SlateGray";
        internal const string Snow  = "Snow";
        internal const string SpringGreen  = "SpringGreen";
        internal const string SteelBlue  = "SteelBlue";
        internal const string Tan  = "Tan";
        internal const string Teal  = "Teal";
        internal const string Thistle  = "Thistle";
        internal const string Tomato  = "Tomato";
        internal const string Turquoise  = "Turquoise";
        internal const string Violet  = "Violet";
        internal const string Wheat  = "Wheat";
        internal const string White  = "White";
        internal const string WhiteSmoke  = "WhiteSmoke";
        internal const string Yellow  = "Yellow";
        internal const string YellowGreen = "YellowGreen";


		private SR()  
		{
			this.resources = new ResourceManager(this.GetType());
		}

		// Methods
		private static SR GetLoader()  
		{
			lock(typeof(SR))
			{
				if (SR.loader == null)
					SR.loader = new SR();
				return SR.loader;
			}
		}

		// Methods

		public static string GetString(CultureInfo culture, string name, object args)
		{
			SR sr = SR.GetLoader();
			
			if (sr == null) 
				return null;
			try
			{
				if (LocalizationProvider.Provider != null)
				{
					String result = string.Empty;

					result = LocalizationProvider.Provider.GetLocalizedString(culture, name, args);

					if (result != string.Empty)
						return result;
				}
				return sr.resources.GetString(name, culture);
			}
			catch
			{
				return name;
			}
		}


		public static string GetString(string name)  
		{
			return SR.GetString(null, name);
		}


		public static string GetString(string name, params object[] args)  
		{
			return SR.GetString(null, name,args);
		}

		public static string GetString(string name, object ctrl)
		{
			return SR.GetString(null, name, ctrl);
		}

		public static string GetString(CultureInfo culture, string name)  
		{
			SR sr = SR.GetLoader();
			if (sr == null) 
				return null;
            try
            {
                if (LocalizationProvider.Provider != null)
                {
                    String result = string.Empty;

                    result = LocalizationProvider.Provider.GetLocalizedString(culture, name, null );

                    if (result != string.Empty)
                        return result;
                }
                return sr.resources.GetString(name, culture);
            }
            catch
            {
                return name;
            }
		}


		public static object GetObject(CultureInfo culture, string name)  
		{
			SR sr = SR.GetLoader();
			if (sr == null) 
				return null;
			return sr.resources.GetObject(name, culture);
		}


		public static object GetObject(string name)  
		{
			return SR.GetObject(null, name);
		}



		public static bool GetBoolean(CultureInfo culture, string name)  
		{
			bool value;
			SR sr = SR.GetLoader();
			object obj;
			value = false;
			if (sr != null) 
			{
				obj = sr.resources.GetObject(name, culture);
				if (obj is System.Boolean) 
					value = ((bool) obj);
			}
			return value;
		}


		public static bool GetBoolean(string name)  
		{
			return SR.GetBoolean(name);
		}


		public static byte GetByte(CultureInfo culture, string name)  
		{
			byte value;
			SR sr = SR.GetLoader();
			object obj;
			value = (byte)0;
			if (sr != null) 
			{
				obj = sr.resources.GetObject(name, culture);
				if (obj is System.Byte) 
					value = ((byte) obj);
			}
			return value;
		}


		public static byte GetByte(string name)  
		{
			return SR.GetByte(null, name);
		}


		public static char GetChar(CultureInfo culture, string name)  
		{
			char value;
			SR sr = SR.GetLoader();
			object obj;
			value = (char)0;
			if (sr != null) 
			{
				obj = sr.resources.GetObject(name, culture);
				if (obj is System.Char)
					value = (char) obj;
			}
			return value;
		}


		public static char GetChar(string name)  
		{
			return SR.GetChar(null, name);
		}


		public static double GetDouble(CultureInfo culture, string name)  
		{
			double value;
			SR sr = SR.GetLoader();
			object obj;
			value = 0.0;
			if (sr != null) 
			{
				obj = sr.resources.GetObject(name, culture);
				if (obj is System.Double) 
					value = ((double) obj);
			}
			return value;
		}


		public static double GetDouble(string name)  
		{
			return SR.GetDouble(null, name);
		}


		public static float GetFloat(CultureInfo culture, string name)  
		{
			float value;
			SR sr = SR.GetLoader();
			object obj;
			value = 0.0f;
			if (sr != null) 
			{
				obj = sr.resources.GetObject(name, culture);
				if (obj is System.Single) 
					value = ((float)obj);
			}
			return value;
		}


		public static float GetFloat(string name)  
		{
			return SR.GetFloat(null, name);
		}


		public static int GetInt(string name)  
		{
			return SR.GetInt(null, name);
		}


		public static int GetInt(CultureInfo culture, string name)  
		{
			int value;
			SR sr = SR.GetLoader();
			object obj;
			value = 0;
			if (sr != null) 
			{
				obj = sr.resources.GetObject(name, culture);
				if (obj is System.Int32) 
					value = ((int) obj);
			}
			return value;
		}


		public static long GetLong(string name)  
		{
			return SR.GetLong(null, name);
		}


		public static long GetLong(CultureInfo culture, string name)  
		{
			Int64 value;
			SR sr = SR.GetLoader();
			object obj;
			value = ((Int64) 0);
			if (sr != null) 
			{
				obj = sr.resources.GetObject(name, culture);
				if (obj is System.Int64)
					value = ((Int64) obj);
			}
			return value;
		}

		public static short GetShort(CultureInfo culture, string name)  
		{
			short value;
			SR sr = SR.GetLoader();
			object obj;
			value = (short)0;
			if (sr != null)
			{
				obj = sr.resources.GetObject(name, culture);
				if (obj is System.Int16)
					value = ((short) obj);
			}
			return value;
		}


		public static short GetShort(string name)  
		{
			return SR.GetShort(null, name);
		}
	}

	/// <summary>
	/// Specifies the category in which the property or event will be displayed in a visual designer.
	/// </summary>
	/// <remarks>
	/// This is a localized version of CategoryAttribute. The localized string will be loaded from the 
	/// assembly manifest Syncfusion.Windows.Forms.Localization.SR.resources
	/// </remarks>
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)] 
	sealed class SRCategoryAttribute : CategoryAttribute
	{
		public SRCategoryAttribute(string category)
			: base(category)
		{
		} 
        
		protected override string GetLocalizedString(string value)
		{
			return SR.GetString(value);
		} 
	} 

	/// <summary>
	/// Specifies a description for a property or event.
	/// </summary>
	/// <remarks>
	/// This is a localized version of DescriptionAttribute. The localized string will be loaded from the 
	/// assembly manifest Syncfusion.Windows.Forms.Localization.SR.resources
	/// </remarks>
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)] 
	sealed class SRDescriptionAttribute : DescriptionAttribute
	{
		private bool replaced = false;
        
		public SRDescriptionAttribute(string description)
			: base(description)
		{
		} 
        
		public override string Description 
		{ 
			get
			{
				if (!this.replaced) 
				{
					this.replaced = true;
					this.DescriptionValue = SR.GetString(base.Description);
				}
				return base.Description;
			} 
		}
	}	
} // end of namespace Syncfusion.Windows.Forms.Localization

namespace Syncfusion.Windows.Forms
{
    /// <summary>
    /// Defines a <see cref="ILocalizationProvider"/> interface.
    /// </summary>
    public interface ILocalizationProvider
    {
        string GetLocalizedString(CultureInfo culture, string name,object  ctrl);
    }

    /// <summary>
    /// Provides static methods and properties to localize the UI.
    /// </summary>
    public sealed class LocalizationProvider
    {
        #region Fields
        private static ILocalizationProvider provider;
        #endregion

        #region LocalizationProvider
        public static ILocalizationProvider Provider
        {
            get
            {
                return provider;
            }
            set
            {
                provider = value;
            }
        }
        #endregion
    }

    /// <summary>
    /// ResourceIdentifiers contains resource Ids specific to the Syncfusion.Shared.Base assembly. 
    /// </summary>
    public sealed class ResourceIdentifiers
    {
        #region String Constants
        public const string Bottom = "Bottom";
        public const string CategoryAction = "CategoryAction";
        public const string CategoryAppearance = "CategoryAppearance";
        public const string CategoryBehavior = "CategoryBehavior";
        public const string CategoryData = "CategoryData";
        public const string ColorEditorPaletteTab = "ColorEditorPaletteTab";
        public const string ColorEditorStandardTab = "ColorEditorStandardTab";
        public const string ColorEditorSystemTab = "ColorEditorSystemTab";
        public const string Copy = "Copy";
        public const string Cut = "Cut";
        public const string Delete = "Delete";
        public const string DescriptionMenuItemText = "DescriptionMenuItemText";
        public const string Error = "Error";
        public const string FewerButtonsItemsText = "FewerButtonsItemsText";
        public const string GroupBarAddRemoveButton = "GroupBarAddRemoveButton";
        public const string GroupBarDropDownToolTip = "GroupBarDropDownToolTip";
        public const string GroupBarExpandButtonToolTip = "GroupBarExpandButtonToolTip";
        public const string GroupBarMinimizeButtonToolTip = "GroupBarMinimizeButtonToolTip";
        public const string GroupBarNavigationPaneTooltip = "GroupBarNavigationPaneTooltip";
        public const string InvalidArgument = "InvalidArgument";
        public const string InvalidLowBoundArgumentEx = "InvalidLowBoundArgumentEx";
        public const string InvalidLowBoundArgumentEx2 = "InvalidLowBoundArgumentEx2";
        public const string LeftEdge = "LeftEdge";
        public const string ModName = "ModName";
        public const string MoreButtonsItemsText = "MoreButtonsItemsText";
        public const string PageDown = "PageDown";
        public const string PageLeft = "PageLeft";
        public const string PageRight = "PageRight";
        public const string PageUp = "PageUp";
        public const string Paste = "Paste";
        public const string RecordNavigationBarAddNewRecord = "RecordNavigationBarAddNewRecord";
        public const string RecordNavigationBarCurrentRecord = "RecordNavigationBarCurrentRecord";
        public const string RecordNavigationBarFirstRecord = "RecordNavigationBarFirstRecord";
        public const string RecordNavigationBarLastRecord = "RecordNavigationBarLastRecord";
        public const string RecordNavigationBarNextRecord = "RecordNavigationBarNextRecord";
        public const string RecordNavigationBarPreviousRecord = "RecordNavigationBarPreviousRecord";
        public const string RecordNavigationBarRecord = "RecordNavigationBarRecord";
        public const string RecordNavigationBarRecordCount = "RecordNavigationBarRecordCount";
        public const string ResetMenuItemText = "ResetMenuItemText";
        public const string RightEdge = "RightEdge";
        public const string ScrollDown = "ScrollDown";
        public const string ScrollHere = "ScrollHere";
        public const string ScrollLeft = "ScrollLeft";
        public const string ScrollRight = "ScrollRight";
        public const string ScrollUp = "ScrollUp";
        public const string SelectAll = "SelectAll";
        public const string StyleCategoryAppearance = "StyleCategoryAppearance";
        public const string TabBarControlAdd = "TabBarControlAdd";
        public const string TabBarControlRemove = "TabBarControlRemove";
        public const string TabBarPageOnTabBarPage = "TabBarPageOnTabBarPage";
        public const string TabBarSplitterControlAdd = "TabBarSplitterControlAdd";
        public const string TabBarSplitterControlRemove = "TabBarSplitterControlRemove";
        public const string TabBarToolTipEmptyIcon = "TabBarToolTipEmptyIcon";
        public const string Test = "Test";
        public const string TextParseFailedFormat = "TextParseFailedFormat";
        public const string Top = "Top";
        public const string TopLeftBottomRight = "TopLeftBottomRight";
        public const string Undo = "Undo";
        public const string OK = "OK";
        public const string Cancel = "Cancel";
        public const string Abort = "Abort";
        public const string Retry = "Retry";
        public const string Ignore = "Ignore";
        public const string Yes = "Yes";
        public const string No = "No";
        public const string Close = "Close";
        public const string Help = "Help";
        public const string TryAgain = "TryAgain";
        public const string Continue = "Continue";
        public const string IgnoreOnce = "IgnoreOnce";
        public const string IgnoreAll = "IgnoreAll";
        public const string Update = "Update";
        public const string Change = "Change";
        public const string ChangeAll = "ChangeAll";
        public const string Options = "Options";
        public const string Add = "Add";
        public const string DeleteAll = "DeleteAll";
        public const string SpellCheckerButtonCustomDictionary = "SpellCheckerButtonCustomDictionary";
        public const string SpellCheckerButtonAddToDictionary = "SpellCheckerButtonAddToDictionary";
        public const string SpellCheckerButtonNew = "SpellCheckerButtonNew";
        public const string SpellCheckerLabelNotInDictionary = "SpellCheckerLabelNotInDictionary";
        public const string SpellCheckerLabelSuggestions = "SpellCheckerLabelSuggestions";
        public const string SpellCheckerLabelOptions = "SpellCheckerLabelOptions";
        public const string SpellCheckerDialogCaption = "SpellCheckerDialogCaption";
        public const string SpellCheckCompletedAlert = "SpellCheckCompletedAlert";
        public const string SpellCheckerDictioanryEditorCaption = "SpellCheckerDictioanryEditorCaption";
        public const string SpellCheckerOptionsDialogCaption = "SpellCheckerOptionsDialogCaption";
        public const string SpellCheckerLabelDictionaryPath = "SpellCheckerLabelDictionaryPath";
        public const string SpellCheckerLabelWords = "SpellCheckerLabelWords";
        public const string SpellCheckerLabelDictionary = "SpellCheckerLabelDictionary";
        public const string SpellCheckerIgnoreUpperCase = "SpellCheckerIgnoreUpperCase";
        public const string SpellCheckerIgnoreMixedCase = "SpellCheckerIgnoreMixedCase";
        public const string SpellCheckerIgnoreWordsWithNumbers = "SpellCheckerIgnoreWordsWithNumbers";
        public const string SpellCheckerIgnoreSpecialCharacters = "SpellCheckerIgnoreSpecialCharacters";
        public const string SpellCheckerIgnoreFileNames = "SpellCheckerIgnoreFileNames";
        public const string SpellCheckerIgnoreEmailAddress = "SpellCheckerIgnoreEmailAddress";
        public const string SpellCheckerIgnoreInternetAddress = "SpellCheckerIgnoreInternetAddress";
        public const string SpellCheckerIgnoreHTMLTags = "SpellCheckerIgnoreHTMLTags";
        public const string ActiveBorder = "ActiveBorder";
        public const string ActiveCaption = "ActiveCaption";
        public const string ActiveCaptionText = "ActiveCaptionText";
        public const string AppWorkspace = "AppWorkspace";
        public const string ButtonFace = "ButtonFace";
        public const string ButtonHighlight = "ButtonHighlight";
        public const string ButtonShadow = "ButtonShadow";
        public const string Control = "Control";
        public const string ControlDark = "ControlDark";
        public const string ControlDarkDark = "ControlDarkDark";
        public const string ControlLight = "ControlLight";
        public const string ControlLightLight = "ControlLightLight";
        public const string ControlText = "ControlText";
        public const string Desktop = "Desktop";
        public const string GradientActiveCaption = "GradientActiveCaption";
        public const string GradientInactiveCaption = "GradientInactiveCaption";
        public const string GrayText = "GrayText";
        public const string Highlight = "Highlight";
        public const string HighlightText = "HighlightText";
        public const string HotTrack = "HotTrack";
        public const string InactiveBorder = "InactiveBorder";
        public const string InactiveCaption = "InactiveCaption";
        public const string InactiveCaptionText = "InactiveCaptionText";
        public const string Info = "Info";
        public const string InfoText = "InfoText";
        public const string Menu = "Menu";
        public const string MenuBar = "MenuBar";
        public const string MenuHighlight = "MenuHighlight";
        public const string MenuText = "MenuText";
        public const string ScrollBar = "ScrollBar";
        public const string Window = "Window";
        public const string WindowFrame = "WindowFrame";
        public const string WindowText = "WindowText";
        public const string Transparent = "Transparent";
        public const string AliceBlue = "AliceBlue";
        public const string AntiqueWhite = "AntiqueWhite";
        public const string Aqua = "Aqua";
        public const string Aquamarine = "Aquamarine";
        public const string Azure = "Azure";
        public const string Beige = "Beige";
        public const string Bisque = "Bisque";
        public const string Black = "Black";
        public const string BlanchedAlmond = "BlanchedAlmond";
        public const string Blue = "Blue";
        public const string BlueViolet = "BlueViolet";
        public const string Brown = "Brown";
        public const string BurlyWood = "BurlyWood";
        public const string CadetBlue = "CadetBlue";
        public const string Chartreuse = "Chartreuse";
        public const string Chocolate = "Chocolate";
        public const string Coral = "Coral";
        public const string CornflowerBlue = "CornflowerBlue";
        public const string Cornsilk = "Cornsilk";
        public const string Crimson = "Crimson";
        public const string Cyan = "Cyan";
        public const string DarkBlue = "DarkBlue";
        public const string DarkCyan = "DarkCyan";
        public const string DarkGoldenrod = "DarkGoldenrod";
        public const string DarkGray = "DarkGray";
        public const string DarkGreen = "DarkGreen";
        public const string DarkKhaki = "DarkKhaki";
        public const string DarkMagenta = "DarkMagenta";
        public const string DarkOliveGreen = "DarkOliveGreen";
        public const string DarkOrange = "DarkOrange";
        public const string DarkOrchid = "DarkOrchid";
        public const string DarkRed = "DarkRed";
        public const string DarkSalmon = "DarkSalmon";
        public const string DarkSeaGreen = "DarkSeaGreen";
        public const string DarkSlateBlue = "DarkSlateBlue";
        public const string DarkSlateGray = "DarkSlateGray";
        public const string DarkTurquoise = "DarkTurquoise";
        public const string DarkViolet = "DarkViolet";
        public const string DeepPink = "DeepPink";
        public const string DeepSkyBlue = "DeepSkyBlue";
        public const string DimGray = "DimGray";
        public const string DodgerBlue = "DodgerBlue";
        public const string Firebrick = "Firebrick";
        public const string FloralWhite = "FloralWhite";
        public const string ForestGreen = "ForestGreen";
        public const string Fuchsia = "Fuchsia";
        public const string Gainsboro = "Gainsboro";
        public const string GhostWhite = "GhostWhite";
        public const string Gold = "Gold";
        public const string Goldenrod = "Goldenrod";
        public const string Gray = "Gray";
        public const string Green = "Green";
        public const string GreenYellow = "GreenYellow";
        public const string Honeydew = "Honeydew";
        public const string HotPink = "HotPink";
        public const string IndianRed = "IndianRed";
        public const string Indigo = "Indigo";
        public const string Ivory = "Ivory";
        public const string Khaki = "Khaki";
        public const string Lavender = "Lavender";
        public const string LavenderBlush = "LavenderBlush";
        public const string LawnGreen = "LawnGreen";
        public const string LemonChiffon = "LemonChiffon";
        public const string LightBlue = "LightBlue";
        public const string LightCoral = "LightCoral";
        public const string LightCyan = "LightCyan";
        public const string LightGoldenrodYellow = "LightGoldenrodYellow";
        public const string LightGreen = "LightGreen";
        public const string LightGray = "LightGray";
        public const string LightPink = "LightPink";
        public const string LightSalmon = "LightSalmon";
        public const string LightSeaGreen = "LightSeaGreen";
        public const string LightSkyBlue = "LightSkyBlue";
        public const string LightSlateGray = "LightSlateGray";
        public const string LightSteelBlue = "LightSteelBlue";
        public const string LightYellow = "LightYellow";
        public const string Lime = "Lime";
        public const string LimeGreen = "LimeGreen";
        public const string Linen = "Linen";
        public const string Magenta = "Magenta";
        public const string Maroon = "Maroon";
        public const string MediumAquamarine = "MediumAquamarine";
        public const string MediumBlue = "MediumBlue";
        public const string MediumOrchid = "MediumOrchid";
        public const string MediumPurple = "MediumPurple";
        public const string MediumSeaGreen = "MediumSeaGreen";
        public const string MediumSlateBlue = "MediumSlateBlue";
        public const string MediumSpringGreen = "MediumSpringGreen";
        public const string MediumTurquoise = "MediumTurquoise";
        public const string MediumVioletRed = "MediumVioletRed";
        public const string MidnightBlue = "MidnightBlue";
        public const string MintCream = "MintCream";
        public const string MistyRose = "MistyRose";
        public const string Moccasin = "Moccasin";
        public const string NavajoWhite = "NavajoWhite";
        public const string Navy = "Navy";
        public const string OldLace = "OldLace";
        public const string Olive = "Olive";
        public const string OliveDrab = "OliveDrab";
        public const string Orange = "Orange";
        public const string OrangeRed = "OrangeRed";
        public const string Orchid = "Orchid";
        public const string PaleGoldenrod = "PaleGoldenrod";
        public const string PaleGreen = "PaleGreen";
        public const string PaleTurquoise = "PaleTurquoise";
        public const string PaleVioletRed = "PaleVioletRed";
        public const string PapayaWhip = "PapayaWhip";
        public const string PeachPuff = "PeachPuff";
        public const string Peru = "Peru";
        public const string Pink = "Pink";
        public const string Plum = "Plum";
        public const string PowderBlue = "PowderBlue";
        public const string Purple = "Purple";
        public const string Red = "Red";
        public const string RosyBrown = "RosyBrown";
        public const string RoyalBlue = "RoyalBlue";
        public const string SaddleBrown = "SaddleBrown";
        public const string Salmon = "Salmon";
        public const string SandyBrown = "SandyBrown";
        public const string SeaGreen = "SeaGreen";
        public const string SeaShell = "SeaShell";
        public const string Sienna = "Sienna";
        public const string Silver = "Silver";
        public const string SkyBlue = "SkyBlue";
        public const string SlateBlue = "SlateBlue";
        public const string SlateGray = "SlateGray";
        public const string Snow = "Snow";
        public const string SpringGreen = "SpringGreen";
        public const string SteelBlue = "SteelBlue";
        public const string Tan = "Tan";
        public const string Teal = "Teal";
        public const string Thistle = "Thistle";
        public const string Tomato = "Tomato";
        public const string Turquoise = "Turquoise";
        public const string Violet = "Violet";
        public const string Wheat = "Wheat";
        public const string White = "White";
        public const string WhiteSmoke = "WhiteSmoke";
        public const string Yellow = "Yellow";
        public const string YellowGreen = "YellowGreen";

        #endregion
    }
}