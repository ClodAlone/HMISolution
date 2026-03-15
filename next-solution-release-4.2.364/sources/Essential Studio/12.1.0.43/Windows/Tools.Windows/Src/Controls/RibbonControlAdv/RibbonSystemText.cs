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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Globalization;
using System.ComponentModel;
using Syncfusion.Windows.Forms.Tools;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class RibbonSystemText
	{
		#region Constants
		const string QA_ADDITEM = "QuickAccessAddItem";
		const string QA_REMOVEITEM = "QuickAccessRemoveItem";
		const string QA_CUSTOMIZE_MENU = "QuickAccessCustomizeMenu";
		const string QA_CUSTOMIZE_CAPTION = "QuickAccessCustomizeCaption";
		const string QA_PLACEBELOW = "QuickAccessPlaceBelowRibbon";
		const string QA_MINIMIZE = "QuickAccessMinimizeTheRibbon";
		const string QA_PLACEABOVE = "QuickAccessPlaceAboveRibbon";
		const string QAD_COMMANDS = "QuickAccessDialogCommands";
		const string QAD_ADD = "QuickAccessDialogButtonAdd";
		const string QAD_REMOVE = "QuickAccessDialogButtonRemove";
		const string QAD_OK = "QuickAccessDialogButtonOk";
		const string QAD_CANCEL = "QuickAccessDialogButtonCancel";
		const string QAD_RESET = "QuickAccessDialogButtonReset";
		const string QAD_DROP_DOWN_NAME = "QuickAccessDialogDropDownName";
		Control locControl = null ;
		#endregion

		#region Constructors
		public RibbonSystemText(Control ctrl)
		{
			locControl = ctrl;
			CultureInfo ci = CultureInfo.CurrentCulture;

			m_sqaAddItemText = SR.GetString(ci, QA_ADDITEM, ctrl);
			m_sqaRemoveItemText = SR.GetString(ci, QA_REMOVEITEM, ctrl);
			m_sqaCustomizeMenuText = SR.GetString(ci, QA_CUSTOMIZE_MENU, ctrl);
			m_sqaPlaceBelowText = SR.GetString(ci, QA_PLACEBELOW, ctrl);
			m_sqaPlaceAboveText = SR.GetString(ci, QA_PLACEABOVE, ctrl);
			m_sqaMinimize = SR.GetString(ci, QA_MINIMIZE, ctrl);
			m_sqaCustomizeCaptionText = SR.GetString(ci, QA_CUSTOMIZE_CAPTION, ctrl);
			m_sqadCommandsText = SR.GetString(ci, QAD_COMMANDS, ctrl);
			m_sqadAddText = SR.GetString(ci, QAD_ADD, ctrl);
			m_sqadRemoveText = SR.GetString(ci, QAD_REMOVE, ctrl);
			m_sqadOkText = SR.GetString(ci, QAD_OK, ctrl);
			m_sqadCancelText = SR.GetString(ci, QAD_CANCEL, ctrl);
			m_sqadResetText = SR.GetString(ci, QAD_RESET, ctrl);
			m_sqadDopDownName = SR.GetString(ci, QAD_DROP_DOWN_NAME, ctrl);
		}
		#endregion
		
		#region Properties
		[Description("Gets or sets text of menu command to add new item to Quick access bar")]
		public string QuickAccessAddItemText
		{
			get { return m_sqaAddItemText; }
			set { m_sqaAddItemText = value; }
		}
		[Description("Gets or sets text of menu command to remove item from Quick access bar")]
		public string QuickAccessRemoveItemText
		{
			get { return m_sqaRemoveItemText; }
			set { m_sqaRemoveItemText = value; }
		}
		[Description("Gets or sets text of menu command to customize Quick access bar")]
		public string QuickAccessCustomizeMenuText
		{
			get { return m_sqaCustomizeMenuText; }
			set { m_sqaCustomizeMenuText = value; }
		}
		[Description("Gets or sets text of menu command to place Quick access bar below Ribbon")]
		public string QuickAccessPlaceBelowText
		{
			get { return m_sqaPlaceBelowText; }
			set { m_sqaPlaceBelowText = value; }
		}
		[Description("Gets or sets text of menu command to place Quick access bar above Ribbon")]
		public string QuickAccessPlaceAboveText
		{
			get { return m_sqaPlaceAboveText; }
			set { m_sqaPlaceAboveText = value; }
		}
		[Description("Gets or sets text of menu command to minimize Ribbon")]
		public string QuickAccessMinimizeRibbon
		{
			get { return m_sqaMinimize; }
			set { m_sqaMinimize = value; }
		}
		[Description("Gets or sets title of Quick access customize dialog")]
		public string QuickAccessCustomizeCaptionText
		{
			get { return m_sqaCustomizeCaptionText; }
			set { m_sqaCustomizeCaptionText = value; }
		}
		[Description("Gets or sets label of command selector in Quick access customize dialog")]
		public string QuickAccessDialogCommandsText
		{
			get { return m_sqadCommandsText; }
			set { m_sqadCommandsText = value; }
		}
		[Description("Gets or sets text of 'Add' button in Quick access customize dialog")]
		public string QuickAccessDialogAddText
		{
			get { return m_sqadAddText; }
			set { m_sqadAddText = value; }
		}
		[Description("Gets or sets text of 'Remove' button in Quick access customize dialog")]
		public string QuickAccessDialogRemoveText
		{
			get { return m_sqadRemoveText; }
			set { m_sqadRemoveText = value; }
		}
		[Description("Gets or sets text of 'OK' button in Quick access customize dialog")]
		public string QuickAccessDialogOkText
		{
			get { return m_sqadOkText; }
			set { m_sqadOkText = value; }
		}
		[Description("Gets or sets text of 'Cancel' button in Quick access customize dialog")]
		public string QuickAccessDialogCancelText
		{
			get { return m_sqadCancelText; }
			set { m_sqadCancelText = value; }
		}
		[Description("Gets or sets text of 'Reset' button in Quick access customize dialog")]
		public string QuickAccessDialogResetText
		{
			get { return m_sqadResetText; }
			set { m_sqadResetText = value; }
		}
		[Description( "Gets or sets the name of drop down in Quick access customize dialog." )]
		public string QuickAccessDialogDropDownName
		{
			get { return m_sqadDopDownName; }
			set { m_sqadDopDownName = value; }
		}
		#endregion

		#region ShouldSerialize/Reset methods
		bool ShouldSerializeQuickAccessAddItemText()
		{
			return m_sqaAddItemText != SR.GetString(CultureInfo.CurrentCulture, QA_ADDITEM, locControl);
		}
		void ResetQuickAccessAddItemText()
		{
			m_sqaAddItemText = SR.GetString(CultureInfo.CurrentCulture, QA_ADDITEM, locControl);
		}
		bool ShouldSerializeQuickAccessRemoveItemText()
		{
			return m_sqaRemoveItemText != SR.GetString(CultureInfo.CurrentCulture, QA_REMOVEITEM, locControl);
		}
		void ResetQuickAccessRemoveItemText()
		{
			m_sqaRemoveItemText = SR.GetString(CultureInfo.CurrentCulture, QA_REMOVEITEM, locControl);
		}
		bool ShouldSerializeQuickAccessCustomizeMenuText()
		{
			return m_sqaCustomizeMenuText != SR.GetString(CultureInfo.CurrentCulture, QA_CUSTOMIZE_MENU, locControl);
		}
		void ResetQuickAccessCustomizeMenuText()
		{
			m_sqaCustomizeMenuText = SR.GetString(CultureInfo.CurrentCulture, QA_CUSTOMIZE_MENU, locControl);
		}
		bool ShouldSerializeQuickAccessPlaceBelowText()
		{
			return m_sqaPlaceBelowText != SR.GetString(CultureInfo.CurrentCulture, QA_PLACEBELOW, locControl);
		}
		void ResetQuickAccessPlaceBelowText()
		{
			m_sqaPlaceBelowText = SR.GetString(CultureInfo.CurrentCulture, QA_PLACEBELOW, locControl);
		}
		bool ShouldSerializeQuickAccessMinimizeRibbon()
		{
			return m_sqaMinimize != SR.GetString( CultureInfo.CurrentCulture, QA_MINIMIZE, locControl);
		}
		void ResetQuickAccessMinimizeRibbon()
		{
			m_sqaMinimize = SR.GetString( CultureInfo.CurrentCulture, QA_MINIMIZE, locControl);
		}
		bool ShouldSerializeQuickAccessPlaceAboveText()
		{
			return m_sqaPlaceAboveText != SR.GetString(CultureInfo.CurrentCulture, QA_PLACEABOVE, locControl);
		}
		void ResetQuickAccessPlaceAboveText()
		{
			m_sqaPlaceAboveText = SR.GetString(CultureInfo.CurrentCulture, QA_PLACEABOVE, locControl);
		}
		bool ShouldSerializeQuickAccessCustomizeCaptionText()
		{
			return m_sqaCustomizeCaptionText != SR.GetString(CultureInfo.CurrentCulture, QA_CUSTOMIZE_CAPTION, locControl);
		}
		void ResetQuickAccessCustomizeCaptionText()
		{
			m_sqaCustomizeCaptionText = SR.GetString(CultureInfo.CurrentCulture, QA_CUSTOMIZE_CAPTION, locControl);
		}
		bool ShouldSerializeQuickAccessDialogCommandsText()
		{
			return m_sqadCommandsText != SR.GetString(CultureInfo.CurrentCulture, QAD_COMMANDS, locControl);
		}
		void ResetQuickAccessDialogCommandsText()
		{
			m_sqadCommandsText = SR.GetString(CultureInfo.CurrentCulture, QAD_COMMANDS, locControl);
		}
		bool ShouldSerializeQuickAccessDialogAddText()
		{
			return m_sqadAddText != SR.GetString(CultureInfo.CurrentCulture, QAD_ADD, locControl);
		}
		void ResetQuickAccessDialogAddText()
		{
			m_sqadAddText = SR.GetString(CultureInfo.CurrentCulture, QAD_ADD, locControl);
		}
		bool ShouldSerializeQuickAccessDialogRemoveText()
		{
			return m_sqadRemoveText != SR.GetString(CultureInfo.CurrentCulture, QAD_REMOVE, locControl);
		}
		void ResetQuickAccessDialogRemoveText()
		{
			m_sqadRemoveText = SR.GetString(CultureInfo.CurrentCulture, QAD_REMOVE, locControl);
		}
		bool ShouldSerializeQuickAccessDialogOkText()
		{
			return m_sqadOkText != SR.GetString(CultureInfo.CurrentCulture, QAD_OK, locControl);
		}
		void ResetQuickAccessDialogOkText()
		{
			m_sqadOkText = SR.GetString(CultureInfo.CurrentCulture, QAD_OK, locControl);
		}
		bool ShouldSerializeQuickAccessDialogCancelText()
		{
			return m_sqadCancelText != SR.GetString(CultureInfo.CurrentCulture, QAD_CANCEL, locControl);
		}
		void ResetQuickAccessDialogCancelText()
		{
			m_sqadCancelText = SR.GetString(CultureInfo.CurrentCulture, QAD_CANCEL, locControl);
		}
		bool ShouldSerializeQuickAccessDialogResetText()
		{
			return m_sqadResetText != SR.GetString(CultureInfo.CurrentCulture, QAD_RESET, locControl);
		}
		void ResetQuickAccessDialogResetText()
		{
			m_sqadResetText = SR.GetString(CultureInfo.CurrentCulture, QAD_RESET, locControl);
		}
		#endregion

		#region Fields
		string m_sqaAddItemText;
		string m_sqaRemoveItemText;
		string m_sqaCustomizeMenuText;
		string m_sqaCustomizeCaptionText;
		string m_sqaPlaceBelowText;
		string m_sqaPlaceAboveText;
		string m_sqaMinimize;
		string m_sqadCommandsText;
		string m_sqadAddText;
		string m_sqadRemoveText;
		string m_sqadOkText;
		string m_sqadCancelText;
		string m_sqadResetText;
		string m_sqadDopDownName;
		#endregion
	}
}
#endif
