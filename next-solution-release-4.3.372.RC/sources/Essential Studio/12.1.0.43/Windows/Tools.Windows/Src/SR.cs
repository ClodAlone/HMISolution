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
using System.ComponentModel;
using System.Resources;
using System.Globalization;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using System.Windows.Forms;

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

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    ///    SR provides localized access to string resources specific 
    ///    from the assembly manifest Syncfusion.Windows.Forms.Tools.SR.resources
    /// </summary>
	sealed class SR 
	{
		#region Fields
		private ResourceManager resources;
		private static SR loader = null;
		#endregion

		#region Strings
		internal const string Add = "Add";
		internal const string AddOrRemoveButtons = "AddOrRemoveButtons";
		internal const string AutoHideMenuItemText = "AutoHideMenuItemText";
		internal const string BeginAGroup = "BeginAGroup";
		internal const string CancelMenuItemText = "CancelMenuItemText";
		internal const string CategoryAction = "CategoryAction";
		internal const string CategoryAppearance = "CategoryAppearance";
		internal const string CategoryBehavior = "CategoryBehavior";
		internal const string CategoryData = "CategoryData";
		internal const string CloseMenuItemText = "CloseMenuItemText";
		internal const string Copy = "Copy";
        internal const string ColorEditorStandardTab = "ColorEditorStandardTab";
        internal const string ColorEditorCustomTab = "ColorEditorCustomTab";
        internal const string ColorEditorOKButton = "ColorEditorOKButton";
        internal const string ColorEditorCancelButton = "ColorEditorCancelButton";
        internal const string ColorEditorColorsLabel = "ColorEditorColorsLabel";
        internal const string ColorEditorNewLabel = "ColorEditorNewLabel";
        internal const string ColorEditorCurrentLabel = "ColorEditorCurrentLabel";
        internal const string ColorEditorColorModelLabel = "ColorEditorColorModelLabel";
        internal const string ColorEditorRedLabel = "ColorEditorRedLabel";
        internal const string ColorEditorGreenLabel = "ColorEditorGreenLabel";
        internal const string ColorEditorBlueLabel = "ColorEditorBlueLabel";
        internal const string ColorEditorHueLabel = "ColorEditorHueLabel";
        internal const string ColorEditorSatLabel = "ColorEditorSatLabel";
        internal const string ColorEditorLumLabel = "ColorEditorLumLabel";
        internal const string ColorPickerStateButton = "ColorPickerStateButton";
        internal const string ColorPickerMoreColorsButton = "ColorPickerMoreColorsButton";
        internal const string ColorPickerThemeColorsGroup = "ColorPickerThemeColorsGroup";
        internal const string ColorPickerStandardColorsGroup = "ColorPickerStandardColorsGroup";
        internal const string ColorPickerRecentColorsGroup = "ColorPickerRecentColorsGroup";
        internal const string ColorPickerCustomColorsGroup = "ColorPickerCustomColorsGroup";
		internal const string CustomizationResetConfirm = "CustomizationResetConfirm";
		internal const string CustomizationResetConfirmTitle = "CustomizationResetConfirmTitle";
		internal const string CustomizeMenu = "CustomizeMenu";
		internal const string CustomMenu = "CustomMenu";
		internal const string Cut = "Cut";
		internal const string DefaultMenuItemText = "DefaultMenuItemText";
		internal const string Delete = "Delete";
		internal const string DeleteAll = "DeleteAll";
		internal const string DeleteConfirmDlgCaption = "DeleteConfirmDlgCaption";
		internal const string DeleteMenuItemText = "DeleteMenuItemText";
		internal const string DescriptionBarItemText = "DescriptionBarItemText";
		internal const string DockableMenuItemText = "DockableMenuItemText";
		internal const string DuplicateNameWarning = "DuplicateNameWarning";
		internal const string FewerButtonsItemsText = "FewerButtonsItemsText";
		internal const string FloatingMenuItemText = "FloatingMenuItemText";
		internal const string GenericCancelButtonText = "GenericCancelButtonText";
		internal const string GenericOKButtonText = "GenericOKButtonText";
		internal const string GroupBarDropDownToolTip = "GroupBarDropDownToolTip";
		internal const string HideMenuItemText = "HideMenuItemText";
		internal const string ImageAndText = "ImageAndText";
		internal const string InvalidToolbarDeleteWarning = "InvalidToolbarDeleteWarning";
		internal const string MainMenu = "MainMenu";
		internal const string MDIChildMenuItemText = "MDIChildMenuItemText";
		internal const string TabbedMDIMenuItemText = "TabbedMDIMenuItemText";
		internal const string MdiListMenuItem = "MdiListMenuItem";
		internal const string MergedBarNamePrefix = "MergedBarNamePrefix";
		internal const string MoreButtonsItemsText = "MoreButtonsItemsText";
		internal const string MoreWindowsCaptionInMenu = "MoreWindowsCaptionInMenu";
		internal const string MoveNextMenuItemText = "MoveNextMenuItemText";
		internal const string MovePrevMenuItemText = "MovePrevMenuItemText";
		internal const string MoveToNextGroupMenuItemText = "MoveToNextGroupMenuItemText";
		internal const string MoveToPrevGroupMenuItemText = "MoveToPrevGroupMenuItemText";
		internal const string NameCaption = "NameCaption";
		internal const string NewHorzGroupMenuItemText = "NewHorzGroupMenuItemText";
		internal const string NewMenu = "NewMenu";
		internal const string NewToolbarName = "NewToolbarName";
		internal const string NewVertGroupMenuItemText = "NewVertGroupMenuItemText";
		internal const string NotifyCustomizationReset = "NotifyCustomizationReset";
		internal const string NotifyCustomizationResetTitle = "NotifyCustomizationResetTitle";
		internal const string NotifyRecentlyUsedItemsReset = "NotifyRecentlyUsedItemsReset";
		internal const string Paste = "Paste";
		internal const string RecentlyUsedItemsResetConfirm = "RecentlyUsedItemsResetConfirm";
		internal const string RecentlyUsedItemsResetConfirmTitle = "RecentlyUsedItemsResetConfirmTitle";
		internal const string Rename = "Rename";
		internal const string ResetBarItem = "ResetBarItem";
		internal const string ResetToolBarMenu = "ResetToolBarMenu";
		internal const string ShortMiscelaneousText = "ShortMiscelaneousText";
		internal const string SuccesfulResetMessageBoxTitle = "SuccesfulResetMessageBoxTitle";
		internal const string SuccesfulToolbarResetMessage = "SuccesfulToolbarResetMessage";
		internal const string TextOnlyAlways = "TextOnlyAlways";
		internal const string TextOnlyInMenus = "TextOnlyInMenus";
		internal const string ToolbarDeleteConfirmation = "ToolbarDeleteConfirmation";
		internal const string ToolbarNameEntryDialogCaption = "ToolbarNameEntryDialogCaption";
		internal const string Undo = "Undo";
		internal const string DockPanelAutoHideButtonToolTip = "DockPanelAutoHideButtonToolTip";
		internal const string DockPanelCloseButtonToolTip = "DockPanelCloseButtonToolTip";
		internal const string DockPanelMenuButtonToolTip = "DockPanelMenuButtonToolTip";
		internal const string DockPanelMaximizeButtonToolTip = "DockPanelMaximizeButtonToolTip";
		internal const string DockPanelRestoreButtonToolTip = "DockPanelRestoreButtonToolTip";
		internal const string MenuItemDockToLeft = "MenuItemDockToLeft";
		internal const string MenuItemDockToRight = "MenuItemDockToRight";
		internal const string MenuItemDockToTop = "MenuItemDockToTop";
		internal const string MenuItemDockToBottom = "MenuItemDockToBottom";
		internal const string MenuItemDockTo = "MenuItemDockTo";
		internal const string MdiListActivateButton = "MdiListActivateButton";
		internal const string MdiListCancelButton = "MdiListCancelButton";
        internal const string ToolTipCaptionButtonClose = "ToolTipCaptionButtonClose";
        internal const string ToolTipCaptionButtonPin = "ToolTipCaptionButtonPin";
        internal const string ToolTipCaptionButtonMenu = "ToolTipCaptionButtonMenu";
        internal const string ToolTipCaptionButtonMaximize = "ToolTipCaptionButtonMaximize";
        internal const string ToolTipCaptionButtonRestore = "ToolTipCaptionButtonRestore";
        internal const string ToolStripItemHelpButton = "ToolStripItemHelpButton";
        internal const string ToolTipCaptionButtonMinimize = "ToolTipCaptionButtonMinimize";
        internal const string QuickAccessAddItem = "QuickAccessAddItem";
        internal const string QuickAccessRemoveItem = "QuickAccessRemoveItem";
        internal const string QuickAccessCustomizeMenu = "QuickAccessCustomizeMenu";
        internal const string QuickAccessCustomizeCaption = "QuickAccessCustomizeCaption";
        internal const string QuickAccessPlaceBelowRibbon = "QuickAccessPlaceBelowRibbon";
        internal const string QuickAccessMinimizeTheRibbon = "QuickAccessMinimizeTheRibbon";
        internal const string QuickAccessPlaceAboveRibbon = "QuickAccessPlaceAboveRibbon";
        internal const string QuickAccessDialogCommands = "QuickAccessDialogCommands";
        internal const string QuickAccessDialogButtonAdd = "QuickAccessDialogButtonAdd";
        internal const string QuickAccessDialogButtonRemove = "QuickAccessDialogButtonRemove";
        internal const string QuickAccessDialogButtonOk = "QuickAccessDialogButtonOk";
        internal const string QuickAccessDialogButtonCancel = "QuickAccessDialogButtonCancel";
        internal const string QuickAccessDialogButtonReset = "QuickAccessDialogButtonReset";
        internal const string QuickAccessDialogDropDownName = "QuickAccessDialogDropDownName";
        internal const string BarCustomizationDialogCaption = "BarCustomizationDialogCaption";
        internal const string BarCustomizationDialogTabOptions = "BarCustomizationDialogTabOptions";
        internal const string BarCustomizationDialogTabCommands = "BarCustomizationDialogTabCommands";
        internal const string BarCustomizationDialogTabToolbars = "BarCustomizationDialogTabToolbars";
        internal const string BarCustomizationDialogResetPartialMenus = "BarCustomizationDialogResetPartialMenus";
        internal const string BarCustomizationDialogResetCustomization = "BarCustomizationDialogResetCustomization";
        internal const string BarCustomizationDialogButtonReset = "BarCustomizationDialogButtonReset";
        internal const string BarCustomizationDialogLargeIcons = "BarCustomizationDialogLargeIcons";
        internal const string BarCustomizationDialogOther = "BarCustomizationDialogOther";
        internal const string BarCustomizationDialogPersonalizedMenus = "BarCustomizationDialogPersonalizedMenus";
        internal const string BarCustomizationDialogCommands = "BarCustomizationDialogCommands";
        internal const string BarCustomizationDialogCategories = "BarCustomizationDialogCategories";
        internal const string BarCustomizationDialogToolbars = "BarCustomizationDialogToolbars";
        internal const string BarCustomizationDialogExpandAfterDelay = "BarCustomizationDialogExpandAfterDelay";
        internal const string BarCustomizationDialogDelete = "BarCustomizationDialogDelete";
        internal const string BarCustomizationDialogModify = "BarCustomizationDialogModify";
        internal const string BarCustomizationDialogClose = "BarCustomizationDialogClose";
        internal const string BarCustomizationDialogNew = "BarCustomizationDialogNew";
        internal const string BarCustomizationDialogAlwaysFullMenu = "BarCustomizationDialogAlwaysFullMenu";
        internal const string MdiWindowDialogCaption = "MdiWindowDialogCaption";
        internal const string GotoToday = "GotoToday";
        internal const string DateTimePickerNoDate = "DateTimePickerNoDate";
        internal const string ColorDialog = "ColorDialog";
        internal const string Today = "Today";
        internal const string None = "None";
        internal const string CustomizeQuickAccessLabel = "CustomizeQuickAccessLabel";
        internal const string QuickAccessToolBarLabel = "QuickAccessToolBarLabel";
        internal const string CustomizationLabel = "CustomizationLabel";
        #endregion

        #region Constructor and Destructor
		private SR()  
		{
			this.resources = new ResourceManager(this.GetType());
		}

        public static void ReleaseResources()
        {
            if(SR.loader!=null)
                SR.loader.resources.ReleaseAllResources();
        }
        #endregion

		#region Implementations
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
			if (LocalizationProvider.Provider != null)
			{
				String result = string.Empty;
				result = LocalizationProvider.Provider.GetLocalizedString(culture, name, args);

				if (result != string.Empty)
					return result;
			}
			SR sr = SR.GetLoader();
			string value;
			
			if (sr == null) 
				return null;

			try
			{
				value = sr.resources.GetString(name, culture);
				if (value != null && args != null ) 
					return String.Format(value,args);
			
				return value;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				return name;
			}
		}


		public static string GetString(string name)  
		{
			return SR.GetString(null, name, null);
		}


		public static string GetString(string name, object args)  
		{
			return SR.GetString(null, name,args);
		}


		public static string GetString(CultureInfo culture, string name)  
		{
            if (LocalizationProvider.Provider != null)
            {
                String result = string.Empty;

                result = LocalizationProvider.Provider.GetLocalizedString(culture, name,null);

                if (result != string.Empty)
                    return result;
            }

			SR sr = SR.GetLoader();
			if (sr == null) 
				return null;
			string value =  sr.resources.GetString(name, culture);

			return value;
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
		#endregion
	}

	/// <summary>
	/// Specifies the category in which the property or event will be displayed in a visual designer.
	/// </summary>
	/// <remarks>
	/// This is a localized version of CategoryAttribute. The localized string will be loaded from the 
	/// assembly manifest Syncfusion.Windows.Forms.Tools.SR.resources
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
	/// assembly manifest Syncfusion.Windows.Forms.Tools.SR.resources
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
    /// <summary>
    /// ToolsResourceIdentifiers contains Ids specific to the Syncfusion.Windows.Forms.Tools namespace. 
    /// </summary>
    public sealed class ToolsResourceIdentifiers
    {
        #region Strings Constants
        public const string Add = "Add";
        public const string AddOrRemoveButtons = "AddOrRemoveButtons";
        public const string AutoHideMenuItemText = "AutoHideMenuItemText";
        public const string BeginAGroup = "BeginAGroup";
        public const string CancelMenuItemText = "CancelMenuItemText";
        public const string CategoryAction = "CategoryAction";
        public const string CategoryAppearance = "CategoryAppearance";
        public const string CategoryBehavior = "CategoryBehavior";
        public const string CategoryData = "CategoryData";
        public const string CloseMenuItemText = "CloseMenuItemText";
        public const string Copy = "Copy";
        public const string ColorEditorStandardTab = "ColorEditorStandardTab";
        public const string ColorEditorCustomTab = "ColorEditorCustomTab";
        public const string ColorEditorOKButton = "ColorEditorOKButton";
        public const string ColorEditorCancelButton = "ColorEditorCancelButton";
        public const string ColorEditorColorsLabel = "ColorEditorColorsLabel";
        public const string ColorEditorNewLabel = "ColorEditorNewLabel";
        public const string ColorEditorCurrentLabel = "ColorEditorCurrentLabel";
        public const string ColorEditorColorModelLabel = "ColorEditorColorModelLabel";
        public const string ColorEditorRedLabel = "ColorEditorRedLabel";
        public const string ColorEditorGreenLabel = "ColorEditorGreenLabel";
        public const string ColorEditorBlueLabel = "ColorEditorBlueLabel";
        public const string ColorEditorHueLabel = "ColorEditorHueLabel";
        public const string ColorEditorSatLabel = "ColorEditorSatLabel";
        public const string ColorEditorLumLabel = "ColorEditorLumLabel";
        public const string ColorPickerStateButton = "ColorPickerStateButton";
        public const string ColorPickerMoreColorsButton = "ColorPickerMoreColorsButton";
        public const string ColorPickerThemeColorsGroup = "ColorPickerThemeColorsGroup";
        public const string ColorPickerStandardColorsGroup = "ColorPickerStandardColorsGroup";
        public const string ColorPickerRecentColorsGroup = "ColorPickerRecentColorsGroup";
        public const string ColorPickerCustomColorsGroup = "ColorPickerCustomColorsGroup";
        public const string CustomizationResetConfirm = "CustomizationResetConfirm";
        public const string CustomizationResetConfirmTitle = "CustomizationResetConfirmTitle";
        public const string CustomizeMenu = "CustomizeMenu";
        public const string CustomMenu = "CustomMenu";
        public const string Cut = "Cut";
        public const string DefaultMenuItemText = "DefaultMenuItemText";
        public const string Delete = "Delete";
        public const string DeleteAll = "DeleteAll";
        public const string DeleteConfirmDlgCaption = "DeleteConfirmDlgCaption";
        public const string DeleteMenuItemText = "DeleteMenuItemText";
        public const string DescriptionBarItemText = "DescriptionBarItemText";
        public const string DockableMenuItemText = "DockableMenuItemText";
        public const string DuplicateNameWarning = "DuplicateNameWarning";
        public const string FewerButtonsItemsText = "FewerButtonsItemsText";
        public const string FloatingMenuItemText = "FloatingMenuItemText";
        public const string GenericCancelButtonText = "GenericCancelButtonText";
        public const string GenericOKButtonText = "GenericOKButtonText";
        public const string GroupBarDropDownToolTip = "GroupBarDropDownToolTip";
        public const string HideMenuItemText = "HideMenuItemText";
        public const string ImageAndText = "ImageAndText";
        public const string InvalidToolbarDeleteWarning = "InvalidToolbarDeleteWarning";
        public const string MainMenu = "MainMenu";
        public const string MDIChildMenuItemText = "MDIChildMenuItemText";
        public const string TabbedMDIMenuItemText = "TabbedMDIMenuItemText";
        public const string MdiListMenuItem = "MdiListMenuItem";
        public const string MergedBarNamePrefix = "MergedBarNamePrefix";
        public const string MoreButtonsItemsText = "MoreButtonsItemsText";
        public const string MoreWindowsCaptionInMenu = "MoreWindowsCaptionInMenu";
        public const string MoveNextMenuItemText = "MoveNextMenuItemText";
        public const string MovePrevMenuItemText = "MovePrevMenuItemText";
        public const string MoveToNextGroupMenuItemText = "MoveToNextGroupMenuItemText";
        public const string MoveToPrevGroupMenuItemText = "MoveToPrevGroupMenuItemText";
        public const string NameCaption = "NameCaption";
        public const string NewHorzGroupMenuItemText = "NewHorzGroupMenuItemText";
        public const string NewMenu = "NewMenu";
        public const string NewToolbarName = "NewToolbarName";
        public const string NewVertGroupMenuItemText = "NewVertGroupMenuItemText";
        public const string NotifyCustomizationReset = "NotifyCustomizationReset";
        public const string NotifyCustomizationResetTitle = "NotifyCustomizationResetTitle";
        public const string NotifyRecentlyUsedItemsReset = "NotifyRecentlyUsedItemsReset";
        public const string Paste = "Paste";
        public const string RecentlyUsedItemsResetConfirm = "RecentlyUsedItemsResetConfirm";
        public const string RecentlyUsedItemsResetConfirmTitle = "RecentlyUsedItemsResetConfirmTitle";
        public const string Rename = "Rename";
        public const string ResetBarItem = "ResetBarItem";
        public const string ResetToolBarMenu = "ResetToolBarMenu";
        public const string ShortMiscelaneousText = "ShortMiscelaneousText";
        public const string SuccesfulResetMessageBoxTitle = "SuccesfulResetMessageBoxTitle";
        public const string SuccesfulToolbarResetMessage = "SuccesfulToolbarResetMessage";
        public const string TextOnlyAlways = "TextOnlyAlways";
        public const string TextOnlyInMenus = "TextOnlyInMenus";
        public const string ToolbarDeleteConfirmation = "ToolbarDeleteConfirmation";
        public const string ToolbarNameEntryDialogCaption = "ToolbarNameEntryDialogCaption";
        public const string Undo = "Undo";
        public const string DockPanelAutoHideButtonToolTip = "DockPanelAutoHideButtonToolTip";
        public const string DockPanelCloseButtonToolTip = "DockPanelCloseButtonToolTip";
        public const string DockPanelMenuButtonToolTip = "DockPanelMenuButtonToolTip";
        public const string DockPanelMaximizeButtonToolTip = "DockPanelMaximizeButtonToolTip";
        public const string DockPanelRestoreButtonToolTip = "DockPanelRestoreButtonToolTip";
        public const string MenuItemDockToLeft = "MenuItemDockToLeft";
        public const string MenuItemDockToRight = "MenuItemDockToRight";
        public const string MenuItemDockToTop = "MenuItemDockToTop";
        public const string MenuItemDockToBottom = "MenuItemDockToBottom";
        public const string MenuItemDockTo = "MenuItemDockTo";
        public const string MdiListActivateButton = "MdiListActivateButton";
        public const string MdiListCancelButton = "MdiListCancelButton";
        public const string ToolTipCaptionButtonClose = "ToolTipCaptionButtonClose";
        public const string ToolTipCaptionButtonPin = "ToolTipCaptionButtonPin";
        public const string ToolTipCaptionButtonMenu = "ToolTipCaptionButtonMenu";
        public const string ToolTipCaptionButtonMaximize = "ToolTipCaptionButtonMaximize";
        public const string ToolTipCaptionButtonRestore = "ToolTipCaptionButtonRestore";
        public const string ToolStripItemHelpButton = "ToolStripItemHelpButton";
        public const string ToolTipCaptionButtonMinimize = "ToolTipCaptionButtonMinimize";
        public const string QuickAccessAddItem = "QuickAccessAddItem";
        public const string QuickAccessRemoveItem = "QuickAccessRemoveItem";
        public const string QuickAccessCustomizeMenu = "QuickAccessCustomizeMenu";
        public const string QuickAccessCustomizeCaption = "QuickAccessCustomizeCaption";
        public const string QuickAccessPlaceBelowRibbon = "QuickAccessPlaceBelowRibbon";
        public const string QuickAccessMinimizeTheRibbon = "QuickAccessMinimizeTheRibbon";
        public const string QuickAccessPlaceAboveRibbon = "QuickAccessPlaceAboveRibbon";
        public const string QuickAccessDialogCommands = "QuickAccessDialogCommands";
        public const string QuickAccessDialogButtonAdd = "QuickAccessDialogButtonAdd";
        public const string QuickAccessDialogButtonRemove = "QuickAccessDialogButtonRemove";
        public const string QuickAccessDialogButtonOk = "QuickAccessDialogButtonOk";
        public const string QuickAccessDialogButtonCancel = "QuickAccessDialogButtonCancel";
        public const string QuickAccessDialogButtonReset = "QuickAccessDialogButtonReset";
        public const string QuickAccessDialogDropDownName = "QuickAccessDialogDropDownName";
        public const string BarCustomizationDialogCaption = "BarCustomizationDialogCaption";
        public const string BarCustomizationDialogTabOptions = "BarCustomizationDialogTabOptions";
        public const string BarCustomizationDialogTabCommands = "BarCustomizationDialogTabCommands";
        public const string BarCustomizationDialogTabToolbars = "BarCustomizationDialogTabToolbars";
        public const string BarCustomizationDialogResetPartialMenus = "BarCustomizationDialogResetPartialMenus";
        public const string BarCustomizationDialogResetCustomization = "BarCustomizationDialogResetCustomization";
        public const string BarCustomizationDialogButtonReset = "BarCustomizationDialogButtonReset";
        public const string BarCustomizationDialogLargeIcons = "BarCustomizationDialogLargeIcons";
        public const string BarCustomizationDialogOther = "BarCustomizationDialogOther";
        public const string BarCustomizationDialogPersonalizedMenus = "BarCustomizationDialogPersonalizedMenus";
        public const string BarCustomizationDialogCommands = "BarCustomizationDialogCommands";
        public const string BarCustomizationDialogCategories = "BarCustomizationDialogCategories";
        public const string BarCustomizationDialogToolbars = "BarCustomizationDialogToolbars";
        public const string BarCustomizationDialogExpandAfterDelay = "BarCustomizationDialogExpandAfterDelay";
        public const string BarCustomizationDialogDelete = "BarCustomizationDialogDelete";
        public const string BarCustomizationDialogModify = "BarCustomizationDialogModify";
        public const string BarCustomizationDialogClose = "BarCustomizationDialogClose";
        public const string BarCustomizationDialogNew = "BarCustomizationDialogNew";
        public const string BarCustomizationDialogAlwaysFullMenu = "BarCustomizationDialogAlwaysFullMenu";
        public const string MdiWindowDialogCaption = "MdiWindowDialogCaption";
        public const string GotoToday = "GotoToday";
        public const string DateTimePickerNoDate = "DateTimePickerNoDate";
        public const string ColorDialog = "ColorDialog";
        public const string Today = "Today";
        public const string None = "None";
        public const string CustomizeQuickAccessLabel = "CustomizeQuickAccessLabel";
        public const string QuickAccessToolBarLabel = "QuickAccessToolBarLabel";
        public const string CustomizationLabel = "CustomizationLabel";
        #endregion
    }
    

} // end of namespace Syncfusion.Windows.Forms.Tools