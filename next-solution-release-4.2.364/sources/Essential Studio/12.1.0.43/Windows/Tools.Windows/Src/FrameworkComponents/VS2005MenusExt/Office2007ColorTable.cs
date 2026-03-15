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
using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools
{
	#region Office12ColorTable
	public class Office12ColorTable
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		:ProfessionalColorTable
#endif
	{
		#region Constants
		protected enum OFFICECOLOR
		{
			launcherBackground = 0,
			launcherBorder,
			launcherText,
			launcherTextSelected,
			menuButtonNormalHighlight,
			menuButtonSelected,
			menuButtonSelectedHighlight,
			menuButtonPressed,
			menuButtonPressedHighlight,
			gripGradientEnd,
			selectedButtonInActiveBegin,
			selectedButtonInActiveEnd,
			scrollButtonArrowDisabled,
			ribbonPanelGroupedBorderEnd,

			/// <summary>
			/// First themes color
			/// </summary>
			themesColorFirst,
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			toolStripGradientBegin = themesColorFirst,
#endif
			toolStripGradientEnd,
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			toolStripHighlightGradientBegin,
			toolStripHighlightGradientEnd,
			toolStripSeparatorDark,
			toolStripSeparatorLight,
#endif
			imageMarginGradientBegin,
			imageMarginGradientEnd,
			captionGradientBegin,
			captionGradientEnd,
			captionHighlightGradientBegin,
			captionHighlightGradientEnd,
			captionGroupedGradientBegin,
			captionGroupedGradientEnd,
			captionGroupedHighlightGradientBegin,
			captionGroupedHighlightGradientEnd,
			captionText,
			checkBorder,
			groupGradientBegin,
			groupGradientEnd,
			groupBorder,
			ribbonBorder,
			ribbonBorderInactive,
			ribbonText,
			ribbonTabText,
			ribbonTabInactiveText,
			ribbonTitleText,

			#region QuickPanel background colors
			quickPanelGradientBegin,
			quickPanelGradientEnd,
			quickPanelGradientBeginInactive,
			quickPanelGradientEndInactive,
			#endregion

			#region SystemButton colors
			systemButtonGradientBegin,
			systemButtonGradientEnd,
			systemButtonBorder,
			sysButtonSelectedGradientBegin,
			sysButtonSelectedGradientEnd,
			sysButtonSelectedHighlight,
			sysButtonBorderSelected,
			sysButtonPressedGradientBegin,
			sysButtonPressedGradientEnd,
			sysButtonPressedHighlight,
			sysButtonForeground,
			sysButtonForegroundSelected,
			sysButtonBorderPressed,
			#endregion

			#region MenuButton colors
			menuButtonNormal,
			menuButtonDropDownGradientBegin,
			menuButtonDropDownGradientEnd,
			menuButtomDropDownBorder,
			#endregion
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			#region ToolStrip collapsed colors
			collapsedToolstripGradientBegin,
			collapsedToolstripGradientEnd,
			collapsedToolstripSelectedGradientBegin,
			collapsedToolstripSelectedGradientEnd,
			collapsedToolstripPressedGradientBegin,
			collapsedToolstripPressedGradientEnd,
			collapsedImageGradientBegin,
			collapsedImageGradientEnd,
			collapsedImagePressedGradientBegin,
			collapsedImagePressedGradientEnd,
			#endregion
#endif
			#region Title colors
			activeTitleGradientBegin,
			activeTitleGradientEnd,
			inactiveTitleGradientBegin,
			inactiveTitleGradientEnd,
			#endregion

			#region StatusStripEx colors
			separatorBegin,
			separatorEnd,
			separatorGradientBegin,
			separatorGradientMiddle,
			separatorGradientEnd,
			separatorStatusControlsAreaBegin,
			separatorStatusControlsAreaEnd,
			#endregion

			contextMenuTitle,
			bottomToolstrip,
			panelBackground,
			officeArrowGradientBegin,
			officeArrowGradientEnd,
			scrollButtonGradientBegin,
			scrollButtonGradientEnd,
			scrollButtonArrow,
			standardScrollButtonHighlightedGradientBegin,
			standardScrollButtonHighlightedGradientEnd,
			standardScrollButtonSelectedGradientBegin,
			standardScrollButtonSelectedGradientEnd,
			standardScrollButtonPressedGradientBegin,
			standardScrollButtonPressedGradientEnd,
			scrollerNormalGradientBegin,
			scrollerNormalGradientEnd,
			scrollerNormalBorder,
			scrollerSelectedGradientBegin,
			scrollerSelectedGradientEnd,
			scrollerSelectedBorder,
			scrollerPressedGradientBegin,
			scrollerPressedGradientEnd,
			scrollerPressedBorder,
			ribbonPanelBorderBegin,
			ribbonPanelBorderEnd,
			tabItemSeparator,

			#region ToolStripGallery colors
			galleryScrollBarBackground,
			scrollButtonLargeArrow,
			scrollButtonLargeArrowDisabled,
			disabledButtonGradientBegin,
			disabledButtonGradientEnd,
			#endregion

			headerSeparatorLight,
			headerSeparatorDark,
			headerSeparatorLightInActive,
			headerSeparatorDarkInActive,
			checkBoxBorder,
			checkBoxBorderSelected,
			radioButtonBorderSelected,

			/// <summary>
			/// Last theme color
			/// </summary>
			themesColorLast,
			MAX = themesColorLast,
		}
		#endregion

		#region Constructors
		static Office12ColorTable()
		{
			Office2007Colors.ManagedColorsApplied +=new Office2007Colors.ManagedColorsAppliedEventHandler(ManagedColorsApplied);
		}

		/// <summary>
		/// 
		/// </summary>
		public Office12ColorTable():this(Office2007Theme.Silver)
		{
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="colorScheme"></param>
		internal Office12ColorTable(Office2007Theme colorScheme)
		{
			m_officeColorScheme = colorScheme;
		}
		/// <summary>
		/// 
		/// </summary>
		~Office12ColorTable()
		{
			Clear();
            //Managed colors are not getting applied if we use the below set of code. So commenting this.
            //m_officeColors = null;
            //m_managedColors = null;
            //Office2007Colors.ManagedColorsApplied -= new Office2007Colors.ManagedColorsAppliedEventHandler(ManagedColorsApplied);
		}
		#endregion

		#region Properties

		#region *** Public
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		#region *** Obsolete colors
		/// <summary>
		/// 
		/// </summary>
		public override Color StatusStripGradientBegin
		{
			get { return Color.FromArgb(231, 237, 249); }
		}
		/// <summary>
		/// 
		/// </summary>
		public override Color StatusStripGradientEnd
		{
			get { return Color.FromArgb(193, 197, 222); }
		}
		#endregion
#endif
		#region *** Common colors
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// 
		/// </summary>
		public override Color MenuBorder
		{
			get { return Color.FromArgb(134, 137, 142); }
		}
		/// <summary>
		/// 
		/// </summary>
		public override Color MenuItemBorder
		{
			get { return Color.FromArgb(203, 208, 214); }
		}
		/// <summary>
		/// 
		/// </summary>
		public override Color MenuStripGradientBegin
		{
			get { return Color.FromArgb(192, 192, 192); }
		}
		/// <summary>
		/// 
		/// </summary>
		public override Color MenuStripGradientEnd
		{
			get { return Color.FromArgb(236, 236, 236); }
		}
		/// <summary>
		/// Gets the color that is the border color to use on a MenuStrip.
		/// </summary>
		public virtual Color MenuStripGradientMiddle
		{
			get { return GetAlphaBlendedColor(this.MenuStripGradientBegin, this.MenuStripGradientEnd, 128); }
		}
		/// <summary>
		/// 
		/// </summary>
		public override Color MenuItemSelectedGradientBegin
		{
			get { return Color.FromArgb(250, 251, 255); }
		}
		/// <summary>
		/// 
		/// </summary>
		public override Color MenuItemPressedGradientBegin
		{
			get { return Color.FromArgb(255, 255, 231); }
		}
		/// <summary>
		/// 
		/// </summary>
		public override Color MenuItemPressedGradientEnd
		{
			get { return Color.FromArgb(255, 215, 99); }
		}
		/// <summary>
		/// 
		/// </summary>
		public override Color MenuItemSelectedGradientEnd
		{
			get { return Color.FromArgb(201, 203, 208); }
		}
#endif
		/// <summary>
		/// 
		/// </summary>
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		public override Color ButtonSelectedGradientBegin
#else
		public Color ButtonSelectedGradientBegin
#endif
		{
			get { return Color.FromArgb(255, 252, 217); }
		}
		/// <summary>
		/// 
		/// </summary>
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		public override Color ButtonSelectedGradientEnd
#else
		public Color ButtonSelectedGradientEnd
#endif		
		{
			get { return Color.FromArgb(255, 214, 70); }
		}
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// 
		/// </summary>
		public override Color ButtonSelectedHighlight
		{
			get { return Color.FromArgb(255, 235, 174); }
		}
		/// <summary>
		/// 
		/// </summary>
		public override Color ButtonCheckedGradientBegin
		{
			get { return Color.FromArgb(253, 210, 168); }
		}
		/// <summary>
		/// 
		/// </summary>
		public override Color ButtonCheckedGradientEnd
		{
			get { return Color.FromArgb(249, 147, 47); }
		}
		/// <summary>
		/// 
		/// </summary>
		public override Color ButtonCheckedHighlight
		{
			get { return Color.FromArgb(253, 241, 176); }
		}
#endif
		/// <summary>
		/// 
		/// </summary>
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		public override Color ButtonPressedGradientBegin
#else
		public Color ButtonPressedGradientBegin
#endif
		{
			get { return Color.FromArgb(255, 197, 108); }
		}
		/// <summary>
		/// 
		/// </summary>
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		public override Color ButtonPressedGradientEnd
#else
		public Color ButtonPressedGradientEnd
#endif
		{
			get { return Color.FromArgb(251, 138, 59); }
		}
		
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// 
		/// </summary>
		public override Color ButtonPressedHighlight
		{
			get { return Color.FromArgb(255, 208, 134); }
		}
		/// <summary>
		/// 
		/// </summary>
		public override Color OverflowButtonGradientBegin
		{
			get { return Color.FromArgb(0xCA, 0xD0, 0xDE); }
		}
		/// <summary>
		/// 
		/// </summary>
		public override Color OverflowButtonGradientEnd
		{
			get { return Color.FromArgb(224, 240, 255); }
		}
#endif
		/// <summary>
		/// 
		/// </summary>
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		public override Color ButtonSelectedBorder
#else
		public Color ButtonSelectedBorder
#endif
		{
			get { return Color.FromArgb(185, 160, 116); }
		}

		/// <summary>
		/// 
		/// </summary>
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		public override Color ButtonPressedBorder
#else
		public Color ButtonPressedBorder
#endif
		{
			get { return Color.FromArgb(139, 118, 84); }
		}
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// 
		/// </summary>
		public override Color CheckBackground
		{
			get { return Color.FromArgb(255, 226, 149); }
		}
#endif
		/// <summary>
		/// Gets the color used when the image of a selected ToolStripItem is rendered.
		/// </summary>
		public virtual Color CheckBorder
		{
			get { return OfficeColors[(int)OFFICECOLOR.checkBorder]; }
		}
		/// <summary>
		/// Gets the solid color used in the Launcher background.
		/// </summary>
		public virtual Color LauncherBackground
		{
			get { return OfficeColors[(int)OFFICECOLOR.launcherBackground]; }
		}
		/// <summary>
		/// Gets the color that is the border color of a Launcher.
		/// </summary>
		public virtual Color LauncherBorder
		{
			get { return OfficeColors[(int)OFFICECOLOR.launcherBorder]; }
		}
		/// <summary>
		/// Gets the color that is the text color of a Launcher.
		/// </summary>
		public virtual Color LauncherText
		{
			get { return OfficeColors[(int)OFFICECOLOR.launcherText]; }
		}
		/// <summary>
		///  Gets the color that is the text color of a Launcher, when Launcher is selected.
		/// </summary>
		public virtual Color LauncherTextSelected
		{
			get { return OfficeColors[(int)OFFICECOLOR.launcherTextSelected]; }
		}
		/// <summary>
		/// Get the end color used when the image of a MenuButton is rendered.
		/// </summary>
		public virtual Color MenuButtonNormalHighlight
		{
			get { return OfficeColors[(int)OFFICECOLOR.menuButtonNormalHighlight]; }
		}
		/// <summary>
		/// Get the starting color used when the image of a MenuButton is rendered, 
		/// when MenuButton is selected.
		/// </summary>
		public virtual Color MenuButtonSelected
		{
			get { return OfficeColors[(int)OFFICECOLOR.menuButtonSelected]; }
		}
		/// <summary>
		/// Get the end color used when the image of a MenuButton is rendered, 
		/// when MenuButton is selected.
		/// </summary>
		public virtual Color MenuButtonSelectedHighlight
		{
			get { return OfficeColors[(int)OFFICECOLOR.menuButtonSelectedHighlight]; }
		}
		/// <summary>
		/// Get the starting color used when the image of a MenuButton is rendered, 
		/// when MenuButton is pressed.
		/// </summary>
		public virtual Color MenuButtonPressed
		{
			get { return OfficeColors[(int)OFFICECOLOR.menuButtonPressed]; }
		}
		/// <summary>
		/// Get the end color used when the image of a MenuButton is rendered, 
		/// when MenuButton is pressed.
		/// </summary>
		public virtual Color MenuButtonPressedHighlight
		{
			get { return OfficeColors[(int)OFFICECOLOR.menuButtonPressedHighlight]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used for the ToolStripGalleryDropDown's grip.
		/// </summary>
		public virtual Color GripGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.gripGradientEnd]; }
		}
		/// <summary>
		/// Gets the starting color of a gradient used in ToolStripButton background,
		/// when ToolStripButton is in inactive state.
		/// </summary>
		public virtual Color SelectedButtonInActiveBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.selectedButtonInActiveBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used in ToolStripButton background,
		/// when ToolStripButton is in inactive state.
		/// </summary>
		public virtual Color SelectedButtonInActiveEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.selectedButtonInActiveEnd]; }
		}
		/// <summary>
		/// Get the color used when image for a ScrollButton arrow is rendered,
		/// when ScrollButton is disabled.
		/// </summary>
		public virtual Color ScrollButtonArrowDisabled
		{
			get { return OfficeColors[(int)OFFICECOLOR.scrollButtonArrowDisabled]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used when RibbonPanel border is rendered,
		/// and RibbonPanel is part of a group.
		/// </summary>
		public virtual Color RibbonPanelGroupedBorderEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.ribbonPanelGroupedBorderEnd]; }
		}
		/// <summary>
		/// Gets the color which is the border color of ToolStripRadioButton,
		/// when it is selected.
		/// </summary>
		public virtual Color RadioButtonBorderSelected
		{
			get { return OfficeColors[(int)OFFICECOLOR.radioButtonBorderSelected]; }
		}

		#endregion

		#region *** Themes colors

		#region ToolStrip colors
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// 
		/// </summary>
		public override Color ToolStripGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.toolStripGradientBegin]; }
		}
#endif
		/// <summary>
		/// 
		/// </summary>
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		public override Color ToolStripGradientEnd
#else
		public Color ToolStripGradientEnd
#endif
		{
			get { return OfficeColors[(int)OFFICECOLOR.toolStripGradientEnd]; }
		}
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// Gets the starting color of a gradient used in the ToolStrip background, when ToolStrip is highlighted.
		/// </summary>
		public virtual Color ToolStripHighlightGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.toolStripHighlightGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used in the ToolStrip background, when ToolStrip is highlighted.
		/// </summary>
		public virtual Color ToolStripHighlightGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.toolStripHighlightGradientEnd]; }
		}
		/// <summary>
		/// 
		/// </summary>
		public override Color SeparatorDark
		{
			get { return OfficeColors[(int)OFFICECOLOR.toolStripSeparatorDark]; }
		}
		/// <summary>
		/// 
		/// </summary>
		public override Color SeparatorLight
		{
			get { return OfficeColors[(int)OFFICECOLOR.toolStripSeparatorLight]; }
		}
		/// <summary>
		/// 
		/// </summary>
		public override Color ImageMarginGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.imageMarginGradientBegin]; }
		}
		/// <summary>
		/// 
		/// </summary>
		public override Color ImageMarginGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.imageMarginGradientEnd]; }
		}
#endif
		#endregion

		/// <summary>
		/// Gets the starting color of the gradient used to draw the caption background on a ToolStrip.
		/// </summary>
		public virtual Color CaptionGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.captionGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of the gradient used to draw the caption background on a ToolStrip.
		/// </summary>
		public virtual Color CaptionGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.captionGradientEnd]; }
		}
		/// <summary>
		/// Gets the starting color of the gradient used to draw the caption background on a ToolStrip,
		/// when the caption is highlighted. 
		/// </summary>
		public virtual Color CaptionHighlightGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.captionHighlightGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of the gradient used to draw the caption background on a ToolStrip,
		/// when the caption is highlighted. 
		/// </summary>
		public virtual Color CaptionHighlightGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.captionHighlightGradientEnd]; }
		}
		/// <summary>
		/// Gets the starting color of the gradient used to draw the caption background on a ToolStrip,
		/// when ToolStrip lays out on a RibbonPanel, which is part of a group.
		/// </summary>
		public virtual Color CaptionGroupedGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.captionGroupedGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of the gradient used to draw the caption background on a ToolStrip,
		/// when ToolStrip lays out on a RibbonPanel, which is part of a group.
		/// </summary>
		public virtual Color CaptionGroupedGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.captionGroupedGradientEnd]; }
		}
		/// <summary>
		/// Gets the starting color of the gradient used to draw the caption background on a ToolStrip,
		/// when ToolStrip lays out on a RibbonPanel, which is part of a group, when the caption is highlighted.
		/// </summary>
		public virtual Color CaptionGroupedHighlightGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.captionGroupedHighlightGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of the gradient used to draw the caption background on a ToolStrip,
		/// when ToolStrip lays out on a RibbonPanel, which is part of a group, when the caption is highlighted.
		/// </summary>
		public virtual Color CaptionGroupedHighlightGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.captionGroupedHighlightGradientEnd]; }
		}
		/// <summary>
		/// Gets the color that is the text color of a caption.
		/// </summary>
		public virtual Color CaptionText
		{
			get { return OfficeColors[(int)OFFICECOLOR.captionText]; }
		}
		/// <summary>
		/// Gets the starting color of a gradient used in the ToolStripItem background,
		/// when ToolStripItem is grouped.
		/// </summary>
		public virtual Color GroupGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.groupGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used in the ToolStripItem background,
		/// when ToolStripItem is grouped.
		/// </summary>
		public virtual Color GroupGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.groupGradientEnd]; }
		}
		/// <summary>
		/// Gets the color that is the border color of the ToolStripItem, when it is grouped.
		/// </summary>
		public virtual Color GroupBorder
		{
			get { return OfficeColors[(int)OFFICECOLOR.groupBorder]; }
		}
		/// <summary>
		/// Gets the starting color of a gradient used in the RibbonControlAdvHeader title background.
		/// </summary>
		public virtual Color ActiveTitleGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.activeTitleGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used in the RibbonControlAdvHeader title background.
		/// </summary>
		public virtual Color ActiveTitleGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.activeTitleGradientEnd]; }
		}
		/// <summary>
		/// Gets the starting color of a gradient used in the RibbonControlAdvHeader title background,
		/// when RibbonControlAdv is in inactive state.
		/// </summary>
		public virtual Color InActiveTitleGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.inactiveTitleGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used in the RibbonControlAdvHeader title background,
		/// when RibbonControlAdv is in inactive state.
		/// </summary>
		public virtual Color InActiveTitleGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.inactiveTitleGradientEnd]; }
		}
		/// <summary>
		/// Gets the color that is border color of a RibbonForm and RibbonControlAdv.
		/// </summary>
		public virtual Color RibbonBorder
		{
			get { return OfficeColors[(int)OFFICECOLOR.ribbonBorder]; }
		}
		/// <summary>
		/// Gets the color that is border color of a RibbonForm and RibbonControlAdv,
		/// when RibbonForm or RibbonControlAdv are in inactive state.
		/// </summary>
		public virtual Color RibbonBorderInactive
		{
			get { return OfficeColors[(int)OFFICECOLOR.ribbonBorderInactive]; }
		}
		/// <summary>
		/// Gets the color that if ForeColor of a RibbonControlAdv.
		/// </summary>
		public virtual Color RibbonText
		{
			get { return OfficeColors[(int)OFFICECOLOR.ribbonText]; }
		}
		/// <summary>
		/// Gets the color that is text color of a ToolStripTabItem.
		/// </summary>
		public virtual Color RibbonTabText
		{
			get { return OfficeColors[(int)OFFICECOLOR.ribbonTabText]; }
		}
		/// <summary>
		/// Gets the color that is text color of a ToolStripTabItem, when it is inactive.
		/// </summary>
		public virtual Color RibbonTabInactiveText
		{
			get { return OfficeColors[(int)OFFICECOLOR.ribbonTabInactiveText]; }
		}
		/// <summary>
		/// Gets the color that is text color of a RibbonControlADvHeader title.
		/// </summary>
		public virtual Color RibbonTitleText
		{
			get { return OfficeColors[(int)OFFICECOLOR.ribbonTitleText]; }
		}

		#region QuickPanel background colors
		/// <summary>
		/// Gets the starting color of a gradient used in the QuickPanel background.
		/// </summary>
		public virtual Color QuickPanelGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.quickPanelGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used in the QuickPanel background. 
		/// </summary>
		public virtual Color QuickPanelGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.quickPanelGradientEnd]; }
		}
		/// <summary>
		/// 
		/// </summary>
		public virtual Color InactiveQuickPanelGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.quickPanelGradientBeginInactive]; }
		}
		/// <summary>
		/// 
		/// </summary>
		public virtual Color InactiveQuickPanelGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.quickPanelGradientEndInactive]; }
		}
		#endregion

		#region SystemButton colors
		/// <summary>
		/// Gets the starting color of a gradient used in the SystemButton background.
		/// </summary>
		public virtual Color SystemButtonGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.systemButtonGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used in the SystemButton background.
		/// </summary>
		public virtual Color SystemButtonGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.systemButtonGradientEnd]; }
		}
		/// <summary>
		/// Gets the color that is the border color of SystemButton.
		/// </summary>
		public virtual Color SystemButtonBorder
		{
			get { return OfficeColors[(int)OFFICECOLOR.systemButtonBorder]; }
		}
		/// <summary>
		/// Gets the starting color of a gradient used in the SystemButton background,
		/// when it is selected.
		/// </summary>
		public virtual Color SystemButtonSelectedGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.sysButtonSelectedGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used in the SystemButton background,
		/// when it is selected.
		/// </summary>
		public virtual Color SystemButtonSelectedGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.sysButtonSelectedGradientEnd]; }
		}
		/// <summary>
		/// Gets the color used in a flash image is rendered, when SystemButton is selected.
		/// </summary>
		public virtual Color SystemButtonSelectedHighlight
		{
			get { return OfficeColors[(int)OFFICECOLOR.sysButtonSelectedHighlight]; }
		}
		/// <summary>
		/// Gets the color that is border color of a SystemButton, when it is selected.
		/// </summary>
		public virtual Color SystemButtonBorderSelected
		{
			get { return OfficeColors[(int)OFFICECOLOR.sysButtonBorderSelected]; }
		}
		/// <summary>
		/// Gets the starting color of a gradient used in the SystemButton background,
		/// when it is pressed.
		/// </summary>
		public virtual Color SystemButtonPressedGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.sysButtonPressedGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used in the SystemButton background,
		/// when it is pressed.
		/// </summary>
		public virtual Color SystemButtonPressedGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.sysButtonPressedGradientEnd]; }
		}
		/// <summary>
		/// Gets the solid color used in a flash image is rendered, when SystemButton is pressed.
		/// </summary>
		public virtual Color SystemButtonPressedHighlight
		{
			get { return OfficeColors[(int)OFFICECOLOR.sysButtonPressedHighlight]; }
		}
		/// <summary>
		/// Gets the color that is border color of a SystemButton, when it is pressed.
		/// </summary>
		public virtual Color SystemButtonBorderPressed
		{
			get { return OfficeColors[(int)OFFICECOLOR.sysButtonBorderPressed]; }
		}
		/// <summary>
		/// Gets the solid color used in SystemButton foreground.
		/// </summary>
		public virtual Color SystemButtonForeground
		{
			get { return OfficeColors[(int)OFFICECOLOR.sysButtonForeground]; }
		}
		/// <summary>
		/// Gets the solid color used in SystemButton foreground, when it is selected.
		/// </summary>
		public virtual Color SystemButtonForegroundSelected
		{
			get { return OfficeColors[(int)OFFICECOLOR.sysButtonForegroundSelected]; }
		}
		#endregion

		#region MenuButton colors
		/// <summary>
		/// Get the starting color used when the image of a MenuButton is rendered.
		/// </summary>
		public virtual Color MenuButtonNormal
		{
			get { return OfficeColors[(int)OFFICECOLOR.menuButtonNormal]; }
		}
		/// <summary>
		/// Gets the starting color of a gradient used in the MenuButtonDropDown background.
		/// </summary>
		public virtual Color MenuButtomDropDownGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.menuButtonDropDownGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used in MenuButtonDropDown background.
		/// </summary>
		public virtual Color MenuButtomDropDownGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.menuButtonDropDownGradientEnd]; }
		}
		/// <summary>
		/// Gets the color that is border color of a MenuButtonDropDown.
		/// </summary>
		public virtual Color MenuButtomDropDownBorder
		{
			get { return OfficeColors[(int)OFFICECOLOR.menuButtomDropDownBorder]; }
		}
		#endregion
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		#region ToolStrip collapsed colors
		/// <summary>
		/// Gets the starting color of a gradient, used in the ToolStrip background,
		/// when ToolStrip is in the collapsed state.
		/// </summary>
		public virtual Color CollapsedToolstripGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.collapsedToolstripGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient, used in the ToolStrip background,
		/// when ToolStrip is in the collapsed state.
		/// </summary>
		public virtual Color CollapsedToolstripGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.collapsedToolstripGradientEnd]; }
		}
		/// <summary>
		/// Gets the starting color of a gradient, used in the ToolStrip background,
		/// when ToolStrip is in the collapsed state and is selected.
		/// </summary>
		public virtual Color CollapsedToolstripSelectedGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.collapsedToolstripSelectedGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient, used in the ToolStrip background,
		/// when ToolStrip is in the collapsed state and is selected.
		/// </summary>
		public virtual Color CollapsedToolstripSelectedGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.collapsedToolstripSelectedGradientEnd]; }
		}
		/// <summary>
		/// Gets the starting color of a gradient, used in the ToolStrip background,
		/// when ToolStrip is in the collapsed state and is pressed.
		/// </summary>
		public virtual Color CollapsedToolstripPressedGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.collapsedToolstripPressedGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient, used in the ToolStrip background,
		/// when ToolStrip is in the collapsed state and is pressed.
		/// </summary>
		public virtual Color CollapsedToolstripPressedGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.collapsedToolstripPressedGradientEnd]; }
		}
		/// <summary>
		/// Gets the starting color used when the image of a ToolStrip rendered,
		/// and ToolStrip is in the collapsed state.
		/// </summary>
		public virtual Color CollapsedImageGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.collapsedImageGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color used when the image of a ToolStrip rendered,
		/// and ToolStrip is in the collapsed state.
		/// </summary>
		public virtual Color CollapsedImageGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.collapsedImageGradientEnd]; }
		}
		/// <summary>
		/// Gets the starting color used when the image of a ToolStrip rendered,
		/// and ToolStrip is in the collapsed state and is pressed.
		/// </summary>
		public virtual Color CollapsedImagePressedGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.collapsedImagePressedGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color used when the image of a ToolStrip rendered,
		/// and ToolStrip is in the collapsed state and is pressed.
		/// </summary>
		public virtual Color CollapsedImagePressedGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.collapsedImagePressedGradientEnd]; }
		}
		#endregion
#endif
		#region StatusStripEx colors
		/// <summary>
		/// Gets the starting color of a gradient used when separator of StatusStripEx is rendered.
		/// </summary>
		public virtual Color SeparatorGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.separatorGradientBegin]; }
		}
		/// <summary>
		/// Gets the middle color of a gradient used when separator of StatusStripEx is rendered.
		/// </summary>
		public virtual Color SeparatorGradientMiddle
		{
			get { return OfficeColors[(int)OFFICECOLOR.separatorGradientMiddle]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used when separator of StatusStripEx is rendered.
		/// </summary>
		public virtual Color SeparatorGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.separatorGradientEnd]; }
		}
		/// <summary>
		/// Gets the starting color of a gradient used in the StatusStripEx's StatusControls background.
		/// </summary>
		public virtual Color SeparatorStatusControlsAreaBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.separatorStatusControlsAreaBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used in the StatusStripEx's StatusControls background.
		/// </summary>
		public virtual Color SeparatorStatusControlsAreaEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.separatorStatusControlsAreaEnd]; }
		}
		/// <summary>
		/// Gets the starting color of a gradient used when separator of StatusStripEx's item is rendered.
		/// </summary>
		public virtual Color SeparatorBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.separatorBegin]; }
		}
		/// <summary>
		/// Gets the starting color of a gradient used when separator of StatusStripEx's item is rendered.
		/// </summary>
		public virtual Color SeparatoraEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.separatorEnd]; }
		}
		#endregion

		/// <summary>
		/// Gets the color that is text color of a ContextMenu title.
		/// </summary>
		public virtual Color ContextMenuTitle
		{
			get { return OfficeColors[(int)OFFICECOLOR.contextMenuTitle]; }
		}
		/// <summary>
		/// Get the solid color used in the BottomToolStrip background.
		/// </summary>
		public virtual Color BottomToolstrip
		{
			get { return OfficeColors[(int)OFFICECOLOR.bottomToolstrip]; }
		}
		/// <summary>
		/// Gets the solid color used in the RibbonPanel background.
		/// </summary>
		public virtual Color PanelBackground
		{
			get { return OfficeColors[(int)OFFICECOLOR.panelBackground]; }
		}
		/// <summary>
		/// Gets the starting color of a gradient used when image for OfficeButton arrow is rendered.
		/// </summary>
		public virtual Color OfficeArrowGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.officeArrowGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used when image for OfficeButton arrow is rendered. 
		/// </summary>
		public virtual Color OfficeArrowGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.officeArrowGradientEnd]; }
		}
		/// <summary>
		/// Gets the starting color of a gradient used in the ScrollButton.
		/// </summary>
		public virtual Color ScrollButtonGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.scrollButtonGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used in the ScrollButton.
		/// </summary>
		public virtual Color ScrollButtonGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.scrollButtonGradientEnd]; }
		}
		/// <summary>
		/// Gets the color used when image for a ScrollButton arrow is rendered.
		/// </summary>
		public virtual Color ScrollButtonArrow
		{
			get { return OfficeColors[(int)OFFICECOLOR.scrollButtonArrow]; }
		}
		/// <summary>
		/// Gets the starting color of a gradient used in the standard ScrollButton background,
		/// when ScrollButton is highlighted.
		/// </summary>
		public virtual Color StandardScrollButtonHighlightedGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.standardScrollButtonHighlightedGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used in the standard ScrollButton background,
		/// when ScrollButton is highlighted.
		/// </summary>
		public virtual Color StandardScrollButtonHighlightedGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.standardScrollButtonHighlightedGradientEnd]; }
		}
		/// <summary>
		/// Gets the starting color of a gradient used in the standard ScrollButton background,
		/// when ScrollButton is selected.
		/// </summary>
		public virtual Color StandardScrollButtonSelectedGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.standardScrollButtonSelectedGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used in the standard ScrollButton background,
		/// when ScrollButton is selected.
		/// </summary>
		public virtual Color StandardScrollButtonSelectedGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.standardScrollButtonSelectedGradientEnd]; }
		}
		/// <summary>
		/// Gets the starting color of a gradient used in the standard ScrollButton background,
		/// when ScrollButton is pressed.
		/// </summary>
		public virtual Color StandardScrollButtonPressedGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.standardScrollButtonPressedGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used in the standard ScrollButton background,
		/// when ScrollButton is pressed.
		/// </summary>
		public virtual Color StandardScrollButtonPressedGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.standardScrollButtonPressedGradientEnd]; }
		}
		/// <summary>
		/// Gets the starting color of a gradient used in the ToolStripGallery's Scroller background.
		/// </summary>
		public virtual Color ScrollerNormalGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.scrollerNormalGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used in the ToolStripGallery's Scroller background.
		/// </summary>
		public virtual Color ScrollerNormalGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.scrollerNormalGradientEnd]; }
		}
		/// <summary>
		/// Gets the color that is the border color of a ToolStripGallery's Scroller.
		/// </summary>
		public virtual Color ScrollerNormalBorder
		{
			get { return OfficeColors[(int)OFFICECOLOR.scrollerNormalBorder]; }
		}
		/// <summary>
		/// Gets the starting color of a gradient used in the ToolStripGallery's Scroller background,
		/// when Scroller is selected.
		/// </summary>
		public virtual Color ScrollerSelectedGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.scrollerSelectedGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used in the ToolStripGallery's Scroller background,
		/// when Scroller is selected.
		/// </summary>
		public virtual Color ScrollerSelectedGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.scrollerSelectedGradientEnd]; }
		}
		/// <summary>
		/// Gets the color that is the border color of a ToolStripGallery's Scroller,
		/// when Scroller is selected.
		/// </summary>
		public virtual Color ScrollerSelectedBorder
		{
			get { return OfficeColors[(int)OFFICECOLOR.scrollerSelectedBorder]; }
		}
		/// <summary>
		/// Gets the starting color of a gradient used in the ToolStripGallery's Scroller background,
		/// when Scroller is pressed.
		/// </summary>
		public virtual Color ScrollerPressedGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.scrollerPressedGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used in the ToolStripGallery's Scroller background,
		/// when Scroller is pressed.
		/// </summary>
		public virtual Color ScrollerPressedGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.scrollerPressedGradientEnd]; }
		}
		/// <summary>
		/// Gets the color that is the border color of a ToolStripGallery's Scroller,
		/// when Scroller is pressed.
		/// </summary>
		public virtual Color ScrollerPressedBorder
		{
			get { return OfficeColors[(int)OFFICECOLOR.scrollerPressedBorder]; }
		}
		/// <summary>
		/// Gets the starting color of a gradient used when RibbonPanel border is rendered.
		/// </summary>
		public virtual Color RibbonPanelBorderBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.ribbonPanelBorderBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used when RibbonPanel border is rendered.
		/// </summary>
		public virtual Color RibbonPanelBorderEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.ribbonPanelBorderEnd]; }
		}
		/// <summary>
		/// Gets the color used when ToolStripTabItem separator is drawn.
		/// </summary>
		public virtual Color TabItemSeparator
		{
			get { return OfficeColors[(int)OFFICECOLOR.tabItemSeparator]; }
		}

		#region ToolStripGallery colors
		/// <summary>
		/// 
		/// </summary>
		public virtual Color GalleryScrollBarBackground
		{
			get { return OfficeColors[(int)OFFICECOLOR.galleryScrollBarBackground]; }
		}
		/// <summary>
		/// Gets the color used when image for a ScrollButton large arrow is rendered.
		/// </summary>
		public virtual Color ScrollButtonLargeArrow
		{
			get { return OfficeColors[(int)OFFICECOLOR.scrollButtonLargeArrow]; }
		}
		/// <summary>
		/// Gets the color used when image for a ScrollButton large arrow is rendered,
		/// when it is disabled.
		/// </summary>
		public virtual Color ScrollButtonLargeArrowDisabled
		{
			get { return OfficeColors[(int)OFFICECOLOR.scrollButtonLargeArrowDisabled]; }
		}
		/// <summary>
		/// Gets the starting color of a gradient used in the ScrollButton,
		/// when ScrollButton is disabled.
		/// </summary>
		public virtual Color DisabledButtonGradientBegin
		{
			get { return OfficeColors[(int)OFFICECOLOR.disabledButtonGradientBegin]; }
		}
		/// <summary>
		/// Gets the end color of a gradient used in the ScrollButton,
		/// when ScrollButton is disabled.
		/// </summary>
		public virtual Color DisabledButtonGradientEnd
		{
			get { return OfficeColors[(int)OFFICECOLOR.disabledButtonGradientEnd]; }
		}
		#endregion

		/// <summary>
		/// Gets the starting color used when RibbonControlAdvHeader separator is drawn.
		/// </summary>
		internal virtual Color HeaderSeparatorLight
		{
			get { return OfficeColors[(int)OFFICECOLOR.headerSeparatorLight]; }
		}
		/// <summary>
		/// Gets the end color used when RibbonControlAdvHeader separator is drawn.
		/// </summary>
		internal virtual Color HeaderSeparatorDark
		{
			get { return OfficeColors[(int)OFFICECOLOR.headerSeparatorDark]; }
		}
		/// <summary>
		/// Gets the starting color used when RibbonControlAdvHeader separator is drawn 
		/// and RibbonControlAdvHeader is in inactive state.
		/// </summary>
		internal virtual Color HeaderSeparatorLightInActive
		{
			get { return OfficeColors[(int)OFFICECOLOR.headerSeparatorLightInActive]; }
		}
		/// <summary>
		/// Gets the end color used when RibbonControlAdvHeader separator is drawn 
		/// and RibbonControlAdvHeader is in inactive state.
		/// </summary>
		internal virtual Color HeaderSeparatorDarkInActive
		{
			get { return OfficeColors[(int)OFFICECOLOR.headerSeparatorDarkInActive]; }
		}
		/// <summary>
		/// Gets the color which is the border color of ToolStripCheckBox.
		/// </summary>
		internal virtual Color CheckBoxBorder
		{
			get { return OfficeColors[(int)OFFICECOLOR.checkBoxBorder]; }
		}
		/// <summary>
		/// Gets the color which is the border color of ToolStripCheckBox,
		/// when it is selected.
		/// </summary>
		internal virtual Color CheckBoxBorderSelected
		{
			get { return OfficeColors[(int)OFFICECOLOR.checkBoxBorderSelected]; }
		}
		#endregion

		#region *** Derived colors
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// 
		/// </summary>
		public override Color ToolStripBorder
		{
			get { return GetAlphaBlendedColor(Color.Black, this.ToolStripGradientBegin, 32); }
		}
#endif
		/// <summary>
		/// 
		/// </summary>
		internal Color RadioButtonExternalBorder
		{
			get
			{
				if (m_clExternalBorder == Color.Empty)
				{
					m_clExternalBorder = GetConvertedColor(0.4F, 1.1F);
				}

				return m_clExternalBorder;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Color RadioButtonInternalBorder
		{
			get
			{
				if (m_clInternalBorder == Color.Empty)
				{
					m_clInternalBorder = GetConvertedColor(0.2f, 1.8f);
				}

				return m_clInternalBorder;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Color RadioButtonInternalBorderSelected
		{
			get
			{
				if (m_clInternalBorderSelected == Color.Empty)
				{
					m_clInternalBorderSelected = GetConvertedColor(2.5f, 1.8f);
				}

				return m_clInternalBorderSelected;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Color RadioButtonInternalBorderPressed
		{
			get
			{
				if (m_clInternalBorderPressed == Color.Empty)
				{
					m_clInternalBorderPressed = GetConvertedColor(2.2f, 1.6f);
				}

				return m_clInternalBorderPressed;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Color RadioButtonUncheckedGradientBegin
		{
			get
			{
				if (m_clUncheckedGradientBegin == Color.Empty)
				{
					m_clUncheckedGradientBegin = GetConvertedColor(1f, 1.4f);
				}
				return m_clUncheckedGradientBegin;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Color RadioButtonUncheckedSelectedGradientBegin
		{
			get
			{
				if (m_clUncheckedSelectedGradientBegin == Color.Empty)
				{
					m_clUncheckedSelectedGradientBegin = GetConvertedColor(2f, 1.4f);
				}
				return m_clUncheckedSelectedGradientBegin;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Color RadioButtonUncheckedSelectedGradientEnd
		{
			get
			{
				if (m_clUncheckedSelectedGradientEnd == Color.Empty)
				{
					m_clUncheckedSelectedGradientEnd = GetConvertedColor(1.6f, 2.4f);
				}

				return m_clUncheckedSelectedGradientEnd;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Color RadioButtonUncheckedPressedGradientBegin
		{
			get
			{
				if (m_clUncheckedPressedGradientBegin == Color.Empty)
				{
					m_clUncheckedPressedGradientBegin = GetConvertedColor(2f, 1f);
				}
				return m_clUncheckedPressedGradientBegin;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Color RadioButtonCheckedBackgroundGradientEnd
		{
			get
			{
				if (m_clCheckedBackgroundGradientEnd == Color.Empty)
				{
					m_clCheckedBackgroundGradientEnd = GetConvertedColor(3f, 0.8f);
				}
				return m_clCheckedBackgroundGradientEnd;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Color RadioButtonCheckedSelectedBackgroundGradientEnd
		{
			get
			{
				if (m_clCheckedSelectedBackgroundGradientEnd == Color.Empty)
				{
					m_clCheckedSelectedBackgroundGradientEnd = GetConvertedColor(3.3f, 1f);
				}
				return m_clCheckedSelectedBackgroundGradientEnd;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Color RadioButtonCheckedPressedBackgroundGradientEnd
		{
			get
			{
				if (m_clCheckedPressedBackgroundGradientEnd == Color.Empty)
				{
					m_clCheckedPressedBackgroundGradientEnd = GetConvertedColor(3f, 0.5f);
				}
				return m_clCheckedPressedBackgroundGradientEnd;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Color RadioButtonCheckedInsideBorder
		{
			get
			{
				if (m_clCheckedInsideBorder == Color.Empty)
				{
					m_clCheckedInsideBorder = GetConvertedColor(1.7f, 0.4f);
				}

				return m_clCheckedInsideBorder;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Color ComboBoxBackgroundColor
		{
			get
			{
				return Office12ColorTable.GetAlphaBlendedColor(this.ToolStripGradientEnd, Color.White, 192);
			}
		}
		#endregion

		#region *** Brushes
		/// <summary>
		/// 
		/// </summary>
		internal IntPtr ComboBoxBackgroundBrush
		{
			get
			{
				if (m_ComboBoxBackgroundBrush == IntPtr.Zero)
				{
					Color c = this.ComboBoxBackgroundColor;
					m_ComboBoxBackgroundBrush = WindowsAPI.CreateSolidBrush((uint)(c.R | (c.G << 8) | (c.B << 16)));
				}
				return m_ComboBoxBackgroundBrush;
			}
		}
		#endregion

		#endregion

		#region *** Protected
		/// <summary>
		/// 
		/// </summary>
		protected Color[] OfficeColors
		{
			get
			{
				if (m_officeColors == null)
				{
					m_officeColors = new Color[(int)OFFICECOLOR.MAX];
					InitColors(ref m_officeColors);
				}
				return m_officeColors;
			}
		}

		#endregion

		#region *** Static
		/// <summary>
		/// 
		/// </summary>
		internal static Office12ColorTable ManagedColors
		{
            get
			{
				if (m_managedColors == null)
				{
					m_managedColors = new OfficeBlue();
				}
				return m_managedColors;
			}
		}
		#endregion

		#endregion

		#region Methods

		/// <summary>
		/// Applies managed color scheme based on default scheme.
		/// </summary>
		/// <param name="form">Container form.</param>
		/// <param name="colorScheme">Office2007 color scheme.</param>
		public static void ApplyManagedColorScheme( Form form, Office2007Theme colorScheme )
		{
			ManagedColors.UpdateColorScheme( colorScheme );
			Office2007Colors.ApplyManagedScheme( form, colorScheme );

			form.Invalidate( true );
		}

		/// <summary>
		/// Applies managed color scheme based on color.
		/// </summary>
		/// <param name="form">Container form.</param>
		/// <param name="color">Color for managed scheme.</param>
		public static void ApplyManagedColors( Form form, Color basicColor )
		{
			Office2007Colors.ApplyManagedColors( form, basicColor );

			form.Invalidate( true );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="src"></param>
		/// <param name="dest"></param>
		/// <param name="alpha"></param>
		/// <returns></returns>
		public static Color GetAlphaBlendedColor(Color src, Color dest, int alpha)
		{
			int R = ((src.R * alpha) + ((0xff - alpha) * dest.R)) / 0xff;
			int G = ((src.G * alpha) + ((0xff - alpha) * dest.G)) / 0xff;
			int B = ((src.B * alpha) + ((0xff - alpha) * dest.B)) / 0xff;
			int A = ((src.A * alpha) + ((0xff - alpha) * dest.A)) / 0xff;

			return Color.FromArgb(A, R, G, B);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="colorScheme"></param>
		internal void UpdateColorScheme(Office2007Theme colorScheme)
		{
			m_officeColorScheme = colorScheme;
			m_officeColors = null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="basicColor"></param>
		internal void UpdateColors(Color basicColor)
		{
			UpdateColorScheme(Office2007Theme.Silver);

			int R = basicColor.R;
			int G = basicColor.G;
			int B = basicColor.B;

			for (int i = (int)OFFICECOLOR.themesColorFirst; i < (int)OFFICECOLOR.themesColorLast; i++)
			{
				Color c = this.OfficeColors[i];

				int r = MergeChannels(c.R, R);
				int g = MergeChannels(c.G, G);
				int b = MergeChannels(c.B, B);

				this.OfficeColors[i] = Color.FromArgb(r, g, b);
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="colors"></param>
		protected virtual void InitColors(ref Color[] colors)
		{
			switch (m_officeColorScheme)
			{
				case Office2007Theme.Blue:
					InitBlueColors(ref colors);
					break;
				case Office2007Theme.Silver:
					InitSilverColors(ref colors);
					break;
				case Office2007Theme.Black:
					InitBlackColors(ref colors);
					break;
			}
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		private void Clear()
		{
			m_clExternalBorder = Color.Empty;
			m_clExternalBorderSelected = Color.Empty;
			m_clInternalBorder = Color.Empty;
			m_clInternalBorderSelected = Color.Empty;
			m_clInternalBorderPressed = Color.Empty;
			m_clUncheckedGradientBegin = Color.Empty;
			m_clUncheckedSelectedGradientBegin = Color.Empty;
			m_clUncheckedPressedGradientBegin = Color.Empty;
			m_clUncheckedSelectedGradientEnd = Color.Empty;
			m_clCheckedBackgroundGradientEnd = Color.Empty;
			m_clCheckedSelectedBackgroundGradientEnd = Color.Empty;
			m_clCheckedPressedBackgroundGradientEnd = Color.Empty;
			m_clCheckedInsideBorder = Color.Empty;

			if (m_ComboBoxBackgroundBrush != IntPtr.Zero)
			{
				WindowsAPI.DeleteObject(m_ComboBoxBackgroundBrush);
				m_ComboBoxBackgroundBrush = IntPtr.Zero;
			}            
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="colors"></param>
		private void InitSilverColors(ref Color[] colors)
		{
			Clear();

			colors[(int)OFFICECOLOR.launcherBackground] = Color.FromArgb(255, 228, 123);
			colors[(int)OFFICECOLOR.launcherBorder] = Color.FromArgb(181, 186, 206);
			colors[(int)OFFICECOLOR.launcherText] = Color.FromArgb(101, 104, 112);
			colors[(int)OFFICECOLOR.launcherTextSelected] = Color.FromArgb(49, 69, 125);
			colors[(int)OFFICECOLOR.menuButtonNormalHighlight] = Color.FromArgb(255, 255, 255);
			colors[(int)OFFICECOLOR.menuButtonSelected] = Color.FromArgb(224, 160, 16);
			colors[(int)OFFICECOLOR.menuButtonSelectedHighlight] = Color.FromArgb(255, 255, 0);
			colors[(int)OFFICECOLOR.menuButtonPressed] = Color.FromArgb(181, 105, 8);
			colors[(int)OFFICECOLOR.menuButtonPressedHighlight] = Color.FromArgb(255, 112, 32);

			colors[(int)OFFICECOLOR.gripGradientEnd] = Color.FromArgb(168, 168, 168);
			colors[(int)OFFICECOLOR.selectedButtonInActiveBegin] = Color.FromArgb(254, 245, 219);
			colors[(int)OFFICECOLOR.selectedButtonInActiveEnd] = Color.FromArgb(255, 239, 194);
			colors[(int)OFFICECOLOR.scrollButtonArrowDisabled] = Color.FromArgb(183, 183, 183);
			colors[(int)OFFICECOLOR.ribbonPanelGroupedBorderEnd] = Color.FromArgb(194, 195, 197);
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			colors[(int)OFFICECOLOR.toolStripGradientBegin] = Color.FromArgb(213, 219, 231);
#endif
			colors[(int)OFFICECOLOR.toolStripGradientEnd] = Color.FromArgb(242, 244, 248);
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			colors[(int)OFFICECOLOR.toolStripHighlightGradientBegin] = Color.FromArgb(228, 232, 239);
			colors[(int)OFFICECOLOR.toolStripHighlightGradientEnd] = Color.FromArgb(245, 247, 250);

			colors[(int)OFFICECOLOR.toolStripSeparatorDark] = base.SeparatorDark;
			colors[(int)OFFICECOLOR.toolStripSeparatorLight] = base.SeparatorLight;
#endif
			colors[(int)OFFICECOLOR.imageMarginGradientBegin] = Color.FromArgb(239, 239, 239);
			colors[(int)OFFICECOLOR.imageMarginGradientEnd] = Color.FromArgb(239, 239, 239);

			#region Caption colors
			colors[(int)OFFICECOLOR.captionGradientBegin] = Color.FromArgb(223, 227, 239);
			colors[(int)OFFICECOLOR.captionGradientEnd] = Color.FromArgb(195, 199, 209);
			colors[(int)OFFICECOLOR.captionHighlightGradientBegin] = Color.FromArgb(234, 237, 244);
			colors[(int)OFFICECOLOR.captionHighlightGradientEnd] = Color.FromArgb(209, 213, 220);
			colors[(int)OFFICECOLOR.captionGroupedGradientBegin] = Color.FromArgb(223, 227, 239);
			colors[(int)OFFICECOLOR.captionGroupedGradientEnd] = Color.FromArgb(195, 199, 209);
			colors[(int)OFFICECOLOR.captionGroupedHighlightGradientBegin] = Color.FromArgb(222, 226, 238);
			colors[(int)OFFICECOLOR.captionGroupedHighlightGradientEnd] = Color.FromArgb(179, 185, 199);
			colors[(int)OFFICECOLOR.captionText] = Color.FromArgb(83, 84, 89);
			#endregion

			colors[(int)OFFICECOLOR.checkBorder] = Color.FromArgb(247, 150, 49);
			
			#region Groupped colors
			colors[(int)OFFICECOLOR.groupGradientBegin] = Color.FromArgb(241, 243, 243);
			colors[(int)OFFICECOLOR.groupGradientEnd] = Color.FromArgb(231, 234, 238);
			colors[(int)OFFICECOLOR.groupBorder] = Color.FromArgb(196, 198, 198);
			#endregion

			#region Ribbon colors
			colors[(int)OFFICECOLOR.ribbonBorder] = Color.FromArgb(208, 212, 221);
			colors[(int)OFFICECOLOR.ribbonBorderInactive] = Color.FromArgb(230, 229, 229);
			colors[(int)OFFICECOLOR.ribbonText] = Color.FromArgb(16, 16, 16);
			colors[(int)OFFICECOLOR.ribbonTabText] = Color.FromArgb(76, 83, 92);
			colors[(int)OFFICECOLOR.ribbonTabInactiveText] = Color.FromArgb(76, 83, 92);
			colors[(int)OFFICECOLOR.ribbonTitleText] = Color.FromArgb(92, 98, 106);
			#endregion

			#region QuickPanel background colors
			colors[(int)OFFICECOLOR.quickPanelGradientBegin] = Color.FromArgb(231, 232, 235);
			colors[(int)OFFICECOLOR.quickPanelGradientEnd] = Color.FromArgb(186, 193, 202);
			colors[(int)OFFICECOLOR.quickPanelGradientBeginInactive] = Color.FromArgb(0xF7, 0xEF, 0xF7);
			colors[(int)OFFICECOLOR.quickPanelGradientEndInactive] = Color.FromArgb(0xDE, 0xDB, 0xDE);
			#endregion

			#region SystemButton colors
			colors[(int)OFFICECOLOR.systemButtonGradientBegin] = Color.FromArgb(235, 240, 245);
			colors[(int)OFFICECOLOR.systemButtonGradientEnd] = Color.FromArgb(203, 210, 219);
			colors[(int)OFFICECOLOR.systemButtonBorder] = Color.FromArgb(141, 148, 157);
			colors[(int)OFFICECOLOR.sysButtonSelectedGradientBegin] = Color.FromArgb(252, 253, 254);
			colors[(int)OFFICECOLOR.sysButtonSelectedGradientEnd] = Color.FromArgb(222, 230, 242);
			colors[(int)OFFICECOLOR.sysButtonSelectedHighlight] = Color.FromArgb(233, 238, 244);
			colors[(int)OFFICECOLOR.sysButtonBorderSelected] = Color.FromArgb(200, 205, 212);
			colors[(int)OFFICECOLOR.sysButtonPressedGradientBegin] = Color.FromArgb(195, 199, 204);
			colors[(int)OFFICECOLOR.sysButtonPressedGradientEnd] = Color.FromArgb(125, 131, 140);
			colors[(int)OFFICECOLOR.sysButtonPressedHighlight] = Color.FromArgb(213, 226, 233);
			colors[(int)OFFICECOLOR.sysButtonForeground] = Color.FromArgb(69, 69, 69);
			colors[(int)OFFICECOLOR.sysButtonForegroundSelected] = Color.FromArgb(69, 69, 69);
			colors[(int)OFFICECOLOR.sysButtonBorderPressed] = Color.FromArgb(151, 156, 160);
			#endregion

			#region MenuButton colors
			colors[(int)OFFICECOLOR.menuButtonNormal] = Color.FromArgb(160, 160, 160);
			colors[(int)OFFICECOLOR.menuButtonDropDownGradientBegin] = Color.FromArgb(222, 227, 231);
			colors[(int)OFFICECOLOR.menuButtonDropDownGradientEnd] = Color.FromArgb(198, 203, 214);
			colors[(int)OFFICECOLOR.menuButtomDropDownBorder] = Color.FromArgb(173, 174, 181);
			#endregion
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			#region ToolStrip collapsed colors
			colors[(int)OFFICECOLOR.collapsedToolstripGradientBegin] = Color.FromArgb(198, 207, 214);
			colors[(int)OFFICECOLOR.collapsedToolstripGradientEnd] = Color.FromArgb(247, 243, 247);
			colors[(int)OFFICECOLOR.collapsedToolstripSelectedGradientBegin] = Color.FromArgb(239, 243, 247);
			colors[(int)OFFICECOLOR.collapsedToolstripSelectedGradientEnd] = Color.FromArgb(247, 251, 255);
			colors[(int)OFFICECOLOR.collapsedToolstripPressedGradientBegin] = Color.FromArgb(181, 186, 198);
			colors[(int)OFFICECOLOR.collapsedToolstripPressedGradientEnd] = Color.FromArgb(231, 231, 231);
			colors[(int)OFFICECOLOR.collapsedImageGradientBegin] = Color.FromArgb(239, 239, 239);
			colors[(int)OFFICECOLOR.collapsedImageGradientEnd] = Color.FromArgb(214, 211, 214);
			colors[(int)OFFICECOLOR.collapsedImagePressedGradientBegin] = Color.FromArgb(231, 231, 239);
			colors[(int)OFFICECOLOR.collapsedImagePressedGradientEnd] = Color.FromArgb(214, 211, 214);
			#endregion
#endif
			#region Title colors
			colors[(int)OFFICECOLOR.activeTitleGradientBegin] = Color.FromArgb(232, 236, 240);
			colors[(int)OFFICECOLOR.activeTitleGradientEnd] = Color.FromArgb(192, 198, 207);
			colors[(int)OFFICECOLOR.inactiveTitleGradientBegin] = Color.FromArgb(247, 247, 247);
			colors[(int)OFFICECOLOR.inactiveTitleGradientEnd] = Color.FromArgb(225, 225, 225);
			#endregion

			#region StatusStripEx colors
			colors[(int)OFFICECOLOR.separatorBegin] = Color.FromArgb(168, 168, 168);
			colors[(int)OFFICECOLOR.separatorEnd] = Color.FromArgb(225, 225, 225);
			colors[(int)OFFICECOLOR.separatorGradientBegin] = Color.FromArgb(225, 237, 250);
			colors[(int)OFFICECOLOR.separatorGradientMiddle] = Color.FromArgb(140, 140, 140);
			colors[(int)OFFICECOLOR.separatorGradientEnd] = Color.FromArgb(225, 225, 225);
			colors[(int)OFFICECOLOR.separatorStatusControlsAreaBegin] = Color.FromArgb(231, 232, 235);
			colors[(int)OFFICECOLOR.separatorStatusControlsAreaEnd] = Color.FromArgb(174, 180, 186);
			#endregion

			colors[(int)OFFICECOLOR.contextMenuTitle] = Color.FromArgb(235, 235, 235);
			colors[(int)OFFICECOLOR.bottomToolstrip] = Color.FromArgb(217, 221, 225);

			colors[(int)OFFICECOLOR.panelBackground] = Color.FromArgb(241, 242, 245);

			colors[(int)OFFICECOLOR.officeArrowGradientBegin] = Color.FromArgb(158, 158, 158);
			colors[(int)OFFICECOLOR.officeArrowGradientEnd] = Color.FromArgb(124, 124, 124);

			colors[(int)OFFICECOLOR.scrollButtonGradientBegin] = Color.FromArgb(221, 225, 229);
			colors[(int)OFFICECOLOR.scrollButtonGradientEnd] = Color.FromArgb(184, 188, 192);

			colors[(int)OFFICECOLOR.scrollButtonArrow] = Color.FromArgb(112, 112, 112);

			colors[(int)OFFICECOLOR.standardScrollButtonHighlightedGradientBegin] = Color.FromArgb(249, 249, 249);
			colors[(int)OFFICECOLOR.standardScrollButtonHighlightedGradientEnd] = Color.FromArgb(194, 194, 194);

			colors[(int)OFFICECOLOR.standardScrollButtonSelectedGradientBegin] = Color.FromArgb(225, 225, 225);
			colors[(int)OFFICECOLOR.standardScrollButtonSelectedGradientEnd] = Color.FromArgb(176, 176, 176);

			colors[(int)OFFICECOLOR.standardScrollButtonPressedGradientBegin] = Color.FromArgb(192, 192, 192);
			colors[(int)OFFICECOLOR.standardScrollButtonPressedGradientEnd] = Color.FromArgb(144, 144, 144);

			colors[(int)OFFICECOLOR.scrollerNormalGradientBegin] = Color.FromArgb(0xED, 0xED, 0xED);
			colors[(int)OFFICECOLOR.scrollerNormalGradientEnd] = Color.FromArgb(0xC9, 0xC9, 0xC9);
			colors[(int)OFFICECOLOR.scrollerNormalBorder] = Color.FromArgb(164, 164, 164);

			colors[(int)OFFICECOLOR.scrollerSelectedGradientBegin] = Color.FromArgb(0xDD, 0xDD, 0xDD);
			colors[(int)OFFICECOLOR.scrollerSelectedGradientEnd] = Color.FromArgb(0xC4, 0xC4, 0xC4);
			colors[(int)OFFICECOLOR.scrollerSelectedBorder] = Color.FromArgb(96, 96, 96);

			colors[(int)OFFICECOLOR.scrollerPressedGradientBegin] = Color.FromArgb(0xC9, 0xC9, 0xC9);
			colors[(int)OFFICECOLOR.scrollerPressedGradientEnd] = Color.FromArgb(0x9B, 0x9B, 0x9B);
			colors[(int)OFFICECOLOR.scrollerPressedBorder] = Color.FromArgb(96, 96, 96);

			colors[(int)OFFICECOLOR.ribbonPanelBorderBegin] = Color.FromArgb(245, 245, 245);
			colors[(int)OFFICECOLOR.ribbonPanelBorderEnd] = Color.FromArgb(192, 192, 192);
			colors[(int)OFFICECOLOR.tabItemSeparator] = Color.FromArgb(176, 176, 176);

			#region ToolStripGallery colors
			colors[(int)OFFICECOLOR.galleryScrollBarBackground] = Color.LightGray;
			colors[(int)OFFICECOLOR.scrollButtonLargeArrow] = Color.FromArgb(68, 81, 113);
			colors[(int)OFFICECOLOR.scrollButtonLargeArrowDisabled] = Color.FromArgb(154, 166, 195);
			colors[(int)OFFICECOLOR.disabledButtonGradientBegin] = Color.FromArgb(243, 243, 243);
			colors[(int)OFFICECOLOR.disabledButtonGradientEnd] = Color.FromArgb(228, 228, 228);
			#endregion

			colors[(int)OFFICECOLOR.headerSeparatorLight] = Color.FromArgb(235, 239, 243);
			colors[(int)OFFICECOLOR.headerSeparatorDark] = Color.FromArgb(172, 176, 180);
			colors[(int)OFFICECOLOR.headerSeparatorLightInActive] = Color.FromArgb(247, 247, 247);
			colors[(int)OFFICECOLOR.headerSeparatorDarkInActive] = Color.FromArgb(181, 181, 181);

			colors[(int)OFFICECOLOR.checkBoxBorder] = Color.FromArgb(155, 157, 160);
			colors[(int)OFFICECOLOR.checkBoxBorderSelected] = Color.FromArgb(155, 157, 160);
			colors[(int)OFFICECOLOR.radioButtonBorderSelected] = Color.FromArgb(0x80, 0x80, 0x80);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="colors"></param>
		private void InitBlueColors(ref Color[] colors)
		{
			Clear();

			colors[(int)OFFICECOLOR.launcherBackground] = Color.FromArgb(255, 228, 123);
			colors[(int)OFFICECOLOR.launcherBorder] = Color.FromArgb(181, 186, 206);
			colors[(int)OFFICECOLOR.launcherText] = Color.FromArgb(101, 104, 112);
			colors[(int)OFFICECOLOR.launcherTextSelected] = Color.FromArgb(49, 69, 125);

			colors[(int)OFFICECOLOR.menuButtonNormalHighlight] = Color.FromArgb(255, 255, 255);
			colors[(int)OFFICECOLOR.menuButtonSelected] = Color.FromArgb(224, 160, 16);
			colors[(int)OFFICECOLOR.menuButtonSelectedHighlight] = Color.FromArgb(255, 255, 0);
			colors[(int)OFFICECOLOR.menuButtonPressed] = Color.FromArgb(181, 105, 8);
			colors[(int)OFFICECOLOR.menuButtonPressedHighlight] = Color.FromArgb(255, 112, 32);

			colors[(int)OFFICECOLOR.gripGradientEnd] = Color.FromArgb(168, 168, 168);

			colors[(int)OFFICECOLOR.selectedButtonInActiveBegin] = Color.FromArgb(254, 245, 219);
			colors[(int)OFFICECOLOR.selectedButtonInActiveEnd] = Color.FromArgb(255, 239, 194);

			colors[(int)OFFICECOLOR.scrollButtonArrowDisabled] = Color.FromArgb(183, 183, 183);

			colors[(int)OFFICECOLOR.ribbonPanelGroupedBorderEnd] = Color.FromArgb(194, 195, 197);
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			colors[(int)OFFICECOLOR.toolStripGradientBegin] = Color.FromArgb(199, 216, 237);
			colors[(int)OFFICECOLOR.toolStripGradientEnd] = Color.FromArgb(231, 242, 255);
			colors[(int)OFFICECOLOR.toolStripHighlightGradientBegin] = Color.FromArgb(210, 224, 240);
			colors[(int)OFFICECOLOR.toolStripHighlightGradientEnd] = Color.FromArgb(242, 248, 255);

			colors[(int)OFFICECOLOR.toolStripSeparatorDark] = base.SeparatorDark;
			colors[(int)OFFICECOLOR.toolStripSeparatorLight] = base.SeparatorLight;
#endif
			colors[(int)OFFICECOLOR.imageMarginGradientBegin] = Color.FromArgb(233, 238, 238);
			colors[(int)OFFICECOLOR.imageMarginGradientEnd] = Color.FromArgb(233, 238, 238);

			#region Caption colors
			colors[(int)OFFICECOLOR.captionGradientBegin] = Color.FromArgb(194, 217, 240);
			colors[(int)OFFICECOLOR.captionGradientEnd] = Color.FromArgb(194, 217, 240);
			colors[(int)OFFICECOLOR.captionHighlightGradientBegin] = Color.FromArgb(200, 224, 255);
			colors[(int)OFFICECOLOR.captionHighlightGradientEnd] = Color.FromArgb(214, 237, 255);
			colors[(int)OFFICECOLOR.captionGroupedGradientBegin] = Color.FromArgb(223, 233, 245);
			colors[(int)OFFICECOLOR.captionGroupedGradientEnd] = Color.FromArgb(210, 221, 242);
			colors[(int)OFFICECOLOR.captionGroupedHighlightGradientBegin] = Color.FromArgb(200, 224, 255);
			colors[(int)OFFICECOLOR.captionGroupedHighlightGradientEnd] = Color.FromArgb(214, 237, 255);
			colors[(int)OFFICECOLOR.captionText] = Color.FromArgb(21, 66, 139);
			#endregion

			colors[(int)OFFICECOLOR.checkBorder] = Color.FromArgb(247, 150, 49);

			#region Groupped colors
			colors[(int)OFFICECOLOR.groupGradientBegin] = Color.FromArgb(201, 221, 246);
			colors[(int)OFFICECOLOR.groupGradientEnd] = Color.FromArgb(160, 189, 224);
			colors[(int)OFFICECOLOR.groupBorder] = Color.FromArgb(138, 173, 219);
			#endregion

			#region Ribbon colors
			colors[(int)OFFICECOLOR.ribbonBorder] = Color.FromArgb(191, 219, 254);
			colors[(int)OFFICECOLOR.ribbonBorderInactive] = Color.FromArgb(204, 216, 232);
			colors[(int)OFFICECOLOR.ribbonText] = Color.FromArgb(32, 24, 32);
			colors[(int)OFFICECOLOR.ribbonTabText] = Color.FromArgb(21, 66, 139);
			colors[(int)OFFICECOLOR.ribbonTabInactiveText] = Color.FromArgb(21, 66, 139);
			colors[(int)OFFICECOLOR.ribbonTitleText] = Color.FromArgb(105, 112, 121);
			#endregion

			#region QuickPanel background colors
			colors[(int)OFFICECOLOR.quickPanelGradientBegin] = Color.FromArgb(226, 238, 253);
			colors[(int)OFFICECOLOR.quickPanelGradientEnd] = Color.FromArgb(156, 189, 231);
			colors[(int)OFFICECOLOR.quickPanelGradientBeginInactive] = Color.FromArgb(0xEF, 0xEF, 0xF7);
			colors[(int)OFFICECOLOR.quickPanelGradientEndInactive] = Color.FromArgb(0xCE, 0xD7, 0xE7);
			#endregion

			#region SystemButton colors
			colors[(int)OFFICECOLOR.systemButtonGradientBegin] = Color.FromArgb(232, 241, 252);
			colors[(int)OFFICECOLOR.systemButtonGradientEnd] = Color.FromArgb(210, 225, 244);
			colors[(int)OFFICECOLOR.systemButtonBorder] = Color.FromArgb(119, 147, 185);
			colors[(int)OFFICECOLOR.sysButtonSelectedGradientBegin] = Color.FromArgb(251, 253, 255);
			colors[(int)OFFICECOLOR.sysButtonSelectedGradientEnd] = Color.FromArgb(210, 228, 254);
			colors[(int)OFFICECOLOR.sysButtonSelectedHighlight] = Color.FromArgb(227, 237, 251);
			colors[(int)OFFICECOLOR.sysButtonBorderSelected] = Color.FromArgb(192, 212, 237);
			colors[(int)OFFICECOLOR.sysButtonPressedGradientBegin] = Color.FromArgb(182, 205, 231);
			colors[(int)OFFICECOLOR.sysButtonPressedGradientEnd] = Color.FromArgb(132, 178, 233);
			colors[(int)OFFICECOLOR.sysButtonPressedHighlight] = Color.FromArgb(192, 231, 252);
			colors[(int)OFFICECOLOR.sysButtonForeground] = Color.FromArgb(101, 101, 101);
			colors[(int)OFFICECOLOR.sysButtonForegroundSelected] = Color.FromArgb(101, 101, 101);
			colors[(int)OFFICECOLOR.sysButtonBorderPressed] = Color.FromArgb(161, 190, 228);
			#endregion

			#region MenuButton colors
			colors[(int)OFFICECOLOR.menuButtonNormal] = Color.FromArgb(156, 170, 198);
			colors[(int)OFFICECOLOR.menuButtonDropDownGradientBegin] = Color.FromArgb(206, 227, 247);
			colors[(int)OFFICECOLOR.menuButtonDropDownGradientEnd] = Color.FromArgb(181, 207, 239);
			colors[(int)OFFICECOLOR.menuButtomDropDownBorder] = Color.FromArgb(156, 174, 206);
			#endregion
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			#region ToolStrip collapsed colors
			colors[(int)OFFICECOLOR.collapsedToolstripGradientBegin] = Color.FromArgb(189, 211, 247);
			colors[(int)OFFICECOLOR.collapsedToolstripGradientEnd] = Color.FromArgb(222, 235, 255);
			colors[(int)OFFICECOLOR.collapsedToolstripSelectedGradientBegin] = Color.FromArgb(222, 231, 247);
			colors[(int)OFFICECOLOR.collapsedToolstripSelectedGradientEnd] = Color.FromArgb(247, 247, 255);
			colors[(int)OFFICECOLOR.collapsedToolstripPressedGradientBegin] = Color.FromArgb(115, 154, 206);
			colors[(int)OFFICECOLOR.collapsedToolstripPressedGradientEnd] = Color.FromArgb(173, 211, 255);
			colors[(int)OFFICECOLOR.collapsedImageGradientBegin] = Color.FromArgb(247, 247, 255);
			colors[(int)OFFICECOLOR.collapsedImageGradientEnd] = Color.FromArgb(198, 219, 247);
			colors[(int)OFFICECOLOR.collapsedImagePressedGradientBegin] = Color.FromArgb(198, 211, 239);
			colors[(int)OFFICECOLOR.collapsedImagePressedGradientEnd] = Color.FromArgb(198, 219, 247);
			#endregion
#endif
			#region Title colors
			colors[(int)OFFICECOLOR.activeTitleGradientBegin] = Color.FromArgb(231, 239, 255);
			colors[(int)OFFICECOLOR.activeTitleGradientEnd] = Color.FromArgb(203, 223, 244);
			colors[(int)OFFICECOLOR.inactiveTitleGradientBegin] = Color.FromArgb(229, 233, 237);
			colors[(int)OFFICECOLOR.inactiveTitleGradientEnd] = Color.FromArgb(217, 225, 233);
			#endregion

			#region StatusStripEx colors
			colors[(int)OFFICECOLOR.separatorBegin] = Color.FromArgb(142, 173, 213);
			colors[(int)OFFICECOLOR.separatorEnd] = Color.FromArgb(231, 240, 251);
			colors[(int)OFFICECOLOR.separatorGradientBegin] = Color.FromArgb(225, 237, 250);
			colors[(int)OFFICECOLOR.separatorGradientMiddle] = Color.FromArgb(101, 138, 186);
			colors[(int)OFFICECOLOR.separatorGradientEnd] = Color.FromArgb(204, 222, 245);
			colors[(int)OFFICECOLOR.separatorStatusControlsAreaBegin] = Color.FromArgb(197, 220, 248);
			colors[(int)OFFICECOLOR.separatorStatusControlsAreaEnd] = Color.FromArgb(120, 154, 200);
			#endregion

			colors[(int)OFFICECOLOR.contextMenuTitle] = Color.FromArgb(221, 231, 238);
			colors[(int)OFFICECOLOR.bottomToolstrip] = Color.FromArgb(174, 203, 232);
			colors[(int)OFFICECOLOR.panelBackground] = Color.FromArgb(233, 234, 238);

			colors[(int)OFFICECOLOR.officeArrowGradientBegin] = Color.FromArgb(106, 126, 197);
			colors[(int)OFFICECOLOR.officeArrowGradientEnd] = Color.FromArgb(64, 70, 90);

			colors[(int)OFFICECOLOR.scrollButtonGradientBegin] = Color.FromArgb(226, 238, 251);
			colors[(int)OFFICECOLOR.scrollButtonGradientEnd] = Color.FromArgb(195, 215, 236);

			colors[(int)OFFICECOLOR.scrollButtonArrow] = Color.FromArgb(83, 128, 173);
			
			colors[(int)OFFICECOLOR.standardScrollButtonHighlightedGradientBegin] = Color.FromArgb(245, 249, 253);
			colors[(int)OFFICECOLOR.standardScrollButtonHighlightedGradientEnd] = Color.FromArgb(206, 226, 247);
			colors[(int)OFFICECOLOR.standardScrollButtonSelectedGradientBegin] = Color.FromArgb(242, 246, 250);
			colors[(int)OFFICECOLOR.standardScrollButtonSelectedGradientEnd] = Color.FromArgb(185, 209, 234);
			colors[(int)OFFICECOLOR.standardScrollButtonPressedGradientBegin] = Color.FromArgb(233, 233, 233);
			colors[(int)OFFICECOLOR.standardScrollButtonPressedGradientEnd] = Color.FromArgb(198, 202, 206);

			colors[(int)OFFICECOLOR.scrollerNormalGradientBegin] = Color.FromArgb(0xED, 0xF0, 0xF3);
			colors[(int)OFFICECOLOR.scrollerNormalGradientEnd] = Color.FromArgb(0xBE, 0xCA, 0xDB);
			colors[(int)OFFICECOLOR.scrollerNormalBorder] = Color.FromArgb(96, 112, 145);

			colors[(int)OFFICECOLOR.scrollerSelectedGradientBegin] = Color.FromArgb(0xD3, 0xE4, 0xFA);
			colors[(int)OFFICECOLOR.scrollerSelectedGradientEnd] = Color.FromArgb(0xAA, 0xCB, 0xF6);
			colors[(int)OFFICECOLOR.scrollerSelectedBorder] = Color.FromArgb(57, 114, 172);

			colors[(int)OFFICECOLOR.scrollerPressedGradientBegin] = Color.FromArgb(0xB4, 0xD1, 0xF7);
			colors[(int)OFFICECOLOR.scrollerPressedGradientEnd] = Color.FromArgb(0x6E, 0xA6, 0xF0);
			colors[(int)OFFICECOLOR.scrollerPressedBorder] = Color.FromArgb(19, 76, 134);

			colors[(int)OFFICECOLOR.ribbonPanelBorderBegin] = Color.FromArgb(231, 239, 247);
			colors[(int)OFFICECOLOR.ribbonPanelBorderEnd] = Color.FromArgb(141, 178, 227);
			colors[(int)OFFICECOLOR.tabItemSeparator] = Color.FromArgb(115, 156, 197);


			#region ToolStripGallery colors
			colors[(int)OFFICECOLOR.galleryScrollBarBackground] = Color.FromArgb(216, 224, 240);
			colors[(int)OFFICECOLOR.scrollButtonLargeArrow] = Color.FromArgb(68, 81, 113);
			colors[(int)OFFICECOLOR.scrollButtonLargeArrowDisabled] = Color.FromArgb(154, 166, 195);
			colors[(int)OFFICECOLOR.disabledButtonGradientBegin] = Color.FromArgb(224, 232, 255);
			colors[(int)OFFICECOLOR.disabledButtonGradientEnd] = Color.FromArgb(216, 224, 240);
			#endregion

			colors[(int)OFFICECOLOR.headerSeparatorLight] = Color.FromArgb(176, 209, 242);
			colors[(int)OFFICECOLOR.headerSeparatorDark] = Color.FromArgb(221, 233, 246);
			colors[(int)OFFICECOLOR.headerSeparatorLightInActive] = Color.FromArgb(207, 219, 232);
			colors[(int)OFFICECOLOR.headerSeparatorDarkInActive] = Color.FromArgb(226, 233, 234);

			colors[(int)OFFICECOLOR.checkBoxBorder] = Color.FromArgb(171, 193, 222);
			colors[(int)OFFICECOLOR.checkBoxBorderSelected] = Color.FromArgb(89, 119, 163);
			colors[(int)OFFICECOLOR.radioButtonBorderSelected] = Color.FromArgb(93, 140, 167);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="colors"></param>
		private void InitBlackColors(ref Color[] colors)
		{
			Clear();

			colors[(int)OFFICECOLOR.launcherBackground] = Color.FromArgb(255, 228, 123);
			colors[(int)OFFICECOLOR.launcherBorder] = Color.FromArgb(181, 186, 206);
			colors[(int)OFFICECOLOR.launcherText] = Color.FromArgb(101, 104, 112);
			colors[(int)OFFICECOLOR.launcherTextSelected] = Color.FromArgb(49, 69, 125);

			colors[(int)OFFICECOLOR.menuButtonNormalHighlight] = Color.FromArgb(255, 255, 255);
			colors[(int)OFFICECOLOR.menuButtonSelected] = Color.FromArgb(224, 160, 16);
			colors[(int)OFFICECOLOR.menuButtonSelectedHighlight] = Color.FromArgb(255, 255, 0);
			colors[(int)OFFICECOLOR.menuButtonPressed] = Color.FromArgb(181, 105, 8);
			colors[(int)OFFICECOLOR.menuButtonPressedHighlight] = Color.FromArgb(255, 112, 32);

			colors[(int)OFFICECOLOR.gripGradientEnd] = Color.FromArgb(168, 168, 168);

			colors[(int)OFFICECOLOR.selectedButtonInActiveBegin] = Color.FromArgb(254, 245, 219);
			colors[(int)OFFICECOLOR.selectedButtonInActiveEnd] = Color.FromArgb(255, 239, 194);

			colors[(int)OFFICECOLOR.scrollButtonArrowDisabled] = Color.FromArgb(183, 183, 183);
			colors[(int)OFFICECOLOR.ribbonPanelGroupedBorderEnd] = Color.FromArgb(194, 195, 197);
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			colors[(int)OFFICECOLOR.toolStripGradientBegin] = Color.FromArgb(180, 187, 197);
			colors[(int)OFFICECOLOR.toolStripGradientEnd] = Color.FromArgb(231, 240, 240);
			colors[(int)OFFICECOLOR.toolStripHighlightGradientBegin] = Color.FromArgb(189, 196, 204);
			colors[(int)OFFICECOLOR.toolStripHighlightGradientEnd] = Color.FromArgb(234, 248, 248);

			colors[(int)OFFICECOLOR.toolStripSeparatorDark] = base.SeparatorDark;
			colors[(int)OFFICECOLOR.toolStripSeparatorLight] = base.SeparatorLight;
#endif
			colors[(int)OFFICECOLOR.imageMarginGradientBegin] = Color.FromArgb(239, 239, 239);
			colors[(int)OFFICECOLOR.imageMarginGradientEnd] = Color.FromArgb(239, 239, 239);

			#region Caption colors
			colors[(int)OFFICECOLOR.captionGradientBegin] = Color.FromArgb(182, 184, 184);
			colors[(int)OFFICECOLOR.captionGradientEnd] = Color.FromArgb(156, 158, 158);
			colors[(int)OFFICECOLOR.captionHighlightGradientBegin] = Color.FromArgb(196, 198, 198);
			colors[(int)OFFICECOLOR.captionHighlightGradientEnd] = Color.FromArgb(175, 177, 177);
			colors[(int)OFFICECOLOR.captionGroupedGradientBegin] = Color.FromArgb(190, 190, 190);
			colors[(int)OFFICECOLOR.captionGroupedGradientEnd] = Color.FromArgb(161, 161, 161);
			colors[(int)OFFICECOLOR.captionGroupedHighlightGradientBegin] = Color.FromArgb(174, 174, 174);
			colors[(int)OFFICECOLOR.captionGroupedHighlightGradientEnd] = Color.FromArgb(110, 110, 110);
			colors[(int)OFFICECOLOR.captionText] = Color.White;
			#endregion

			colors[(int)OFFICECOLOR.checkBorder] = Color.FromArgb(247, 150, 49);

			#region Groupped colors
			colors[(int)OFFICECOLOR.groupGradientBegin] = Color.FromArgb(214, 222, 223);
			colors[(int)OFFICECOLOR.groupGradientEnd] = Color.FromArgb(206, 213, 215);
			colors[(int)OFFICECOLOR.groupBorder] = Color.FromArgb(179, 188, 191);
			#endregion

			#region Ribbon colors
			colors[(int)OFFICECOLOR.ribbonBorder] = Color.FromArgb(83, 83, 83);
			colors[(int)OFFICECOLOR.ribbonBorderInactive] = Color.FromArgb(153, 153, 153);
			colors[(int)OFFICECOLOR.ribbonText] = Color.FromArgb(16, 8, 16);
			colors[(int)OFFICECOLOR.ribbonTabText] = Color.FromArgb(0, 0, 0);
			colors[(int)OFFICECOLOR.ribbonTabInactiveText] = Color.FromArgb(255, 255, 255);
			colors[(int)OFFICECOLOR.ribbonTitleText] = Color.FromArgb(255, 255, 255);
			#endregion

			#region QuickPanel background colors
			colors[(int)OFFICECOLOR.quickPanelGradientBegin] = Color.FromArgb(150, 152, 156);
			colors[(int)OFFICECOLOR.quickPanelGradientEnd] = Color.FromArgb(92, 95, 99);
			colors[(int)OFFICECOLOR.quickPanelGradientBeginInactive] = Color.FromArgb(0xB5, 0xB2, 0xB5);
			colors[(int)OFFICECOLOR.quickPanelGradientEndInactive] = Color.FromArgb(0x8C, 0x8A, 0x8C);
			#endregion

			#region SystemButton colors
			colors[(int)OFFICECOLOR.systemButtonGradientBegin] = Color.FromArgb(255, 255, 255);
			colors[(int)OFFICECOLOR.systemButtonGradientEnd] = Color.FromArgb(203, 213, 223);
			colors[(int)OFFICECOLOR.systemButtonBorder] = Color.FromArgb(137, 135, 133);

			colors[(int)OFFICECOLOR.sysButtonSelectedGradientBegin] = Color.FromArgb(162, 171, 180);
			colors[(int)OFFICECOLOR.sysButtonSelectedGradientEnd] = Color.FromArgb(91, 105, 123);
			colors[(int)OFFICECOLOR.sysButtonSelectedHighlight] = Color.FromArgb(173, 199, 214);
			colors[(int)OFFICECOLOR.sysButtonBorderSelected] = Color.FromArgb(86, 96, 109);
			
			colors[(int)OFFICECOLOR.sysButtonPressedGradientBegin] = Color.FromArgb(43, 43, 43);
			colors[(int)OFFICECOLOR.sysButtonPressedGradientEnd] = Color.FromArgb(0, 0, 0);
			colors[(int)OFFICECOLOR.sysButtonPressedHighlight] = Color.FromArgb(65, 83, 102);
			
			colors[(int)OFFICECOLOR.sysButtonForeground] = Color.FromArgb(147, 156, 168);
			colors[(int)OFFICECOLOR.sysButtonForegroundSelected] = Color.FromArgb(54, 53, 53);
			colors[(int)OFFICECOLOR.sysButtonBorderPressed] = Color.FromArgb(34, 36, 41);
			#endregion

			#region MenuButton colors
			colors[(int)OFFICECOLOR.menuButtonNormal] = Color.FromArgb(156, 156, 156);
			colors[(int)OFFICECOLOR.menuButtonDropDownGradientBegin] = Color.FromArgb(99, 99, 99);
			colors[(int)OFFICECOLOR.menuButtonDropDownGradientEnd] = Color.FromArgb(41, 44, 41);
			colors[(int)OFFICECOLOR.menuButtomDropDownBorder] = Color.FromArgb(66, 66, 66);
			#endregion
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			#region ToolStrip collapsed colors
			colors[(int)OFFICECOLOR.collapsedToolstripGradientBegin] = Color.FromArgb(181, 190, 198);
			colors[(int)OFFICECOLOR.collapsedToolstripGradientEnd] = Color.FromArgb(239, 235, 239);
			colors[(int)OFFICECOLOR.collapsedToolstripSelectedGradientBegin] = Color.FromArgb(198, 207, 222);
			colors[(int)OFFICECOLOR.collapsedToolstripSelectedGradientEnd] = Color.FromArgb(239, 235, 239);
			colors[(int)OFFICECOLOR.collapsedToolstripPressedGradientBegin] = Color.FromArgb(156, 162, 173);
			colors[(int)OFFICECOLOR.collapsedToolstripPressedGradientEnd] = Color.FromArgb(214, 219, 222);
			colors[(int)OFFICECOLOR.collapsedImageGradientBegin] = Color.FromArgb(239, 239, 239);
			colors[(int)OFFICECOLOR.collapsedImageGradientEnd] = Color.FromArgb(198, 195, 198);
			colors[(int)OFFICECOLOR.collapsedImagePressedGradientBegin] = Color.FromArgb(222, 219, 222);
			colors[(int)OFFICECOLOR.collapsedImagePressedGradientEnd] = Color.FromArgb(198, 195, 198);
			#endregion
#endif
			#region Title colors
			colors[(int)OFFICECOLOR.activeTitleGradientBegin] = Color.FromArgb(74, 74, 74);
			colors[(int)OFFICECOLOR.activeTitleGradientEnd] = Color.FromArgb(47, 47, 47);
			colors[(int)OFFICECOLOR.inactiveTitleGradientBegin] = Color.FromArgb(157, 157, 157);
			colors[(int)OFFICECOLOR.inactiveTitleGradientEnd] = Color.FromArgb(146, 146, 146);
			#endregion

			#region StatusStripEx colors
			colors[(int)OFFICECOLOR.separatorBegin] = Color.FromArgb(50, 52, 50);
			colors[(int)OFFICECOLOR.separatorEnd] = Color.FromArgb(200, 203, 200);
			colors[(int)OFFICECOLOR.separatorGradientBegin] = Color.FromArgb(200, 200, 200);
			colors[(int)OFFICECOLOR.separatorGradientMiddle] = Color.FromArgb(41, 44, 41);
			colors[(int)OFFICECOLOR.separatorGradientEnd] = Color.FromArgb(180, 180, 180);
			colors[(int)OFFICECOLOR.separatorStatusControlsAreaBegin] = Color.FromArgb(216, 215, 216);
			colors[(int)OFFICECOLOR.separatorStatusControlsAreaEnd] = Color.FromArgb(84, 92, 102);
			#endregion

			colors[(int)OFFICECOLOR.contextMenuTitle] = Color.FromArgb(235, 235, 235);
			colors[(int)OFFICECOLOR.bottomToolstrip] = Color.FromArgb(139, 139, 139);

			colors[(int)OFFICECOLOR.panelBackground] = Color.FromArgb(233, 234, 238);

			colors[(int)OFFICECOLOR.officeArrowGradientBegin] = Color.FromArgb(78, 78, 79);
			colors[(int)OFFICECOLOR.officeArrowGradientEnd] = Color.FromArgb(40, 40, 40);

			colors[(int)OFFICECOLOR.scrollButtonGradientBegin] = Color.FromArgb(221, 225, 229);
			colors[(int)OFFICECOLOR.scrollButtonGradientEnd] = Color.FromArgb(184, 188, 192);

			colors[(int)OFFICECOLOR.scrollButtonArrow] = Color.FromArgb(112, 112, 112);

			colors[(int)OFFICECOLOR.standardScrollButtonHighlightedGradientBegin] = Color.FromArgb(249, 249, 249);
			colors[(int)OFFICECOLOR.standardScrollButtonHighlightedGradientEnd] = Color.FromArgb(194, 194, 194);

			colors[(int)OFFICECOLOR.standardScrollButtonSelectedGradientBegin] = Color.FromArgb(225, 225, 225);
			colors[(int)OFFICECOLOR.standardScrollButtonSelectedGradientEnd] = Color.FromArgb(176, 176, 176);

			colors[(int)OFFICECOLOR.standardScrollButtonPressedGradientBegin] = Color.FromArgb(192, 192, 192);
			colors[(int)OFFICECOLOR.standardScrollButtonPressedGradientEnd] = Color.FromArgb(144, 144, 144);

			colors[(int)OFFICECOLOR.scrollerNormalGradientBegin] = Color.FromArgb(0xED, 0xED, 0xED);
			colors[(int)OFFICECOLOR.scrollerNormalGradientEnd] = Color.FromArgb(0xC9, 0xC9, 0xC9);
			colors[(int)OFFICECOLOR.scrollerNormalBorder] = Color.FromArgb(164, 164, 164);

			colors[(int)OFFICECOLOR.scrollerSelectedGradientBegin] = Color.FromArgb(0xDD, 0xDD, 0xDD);
			colors[(int)OFFICECOLOR.scrollerSelectedGradientEnd] = Color.FromArgb(0xC4, 0xC4, 0xC4);
			colors[(int)OFFICECOLOR.scrollerSelectedBorder] = Color.FromArgb(96, 96, 96);

			colors[(int)OFFICECOLOR.scrollerPressedGradientBegin] = Color.FromArgb(0xC9, 0xC9, 0xC9);
			colors[(int)OFFICECOLOR.scrollerPressedGradientEnd] = Color.FromArgb(0x9B, 0x9B, 0x9B);
			colors[(int)OFFICECOLOR.scrollerPressedBorder] = Color.FromArgb(96, 96, 96);

			colors[(int)OFFICECOLOR.ribbonPanelBorderBegin] = Color.FromArgb(215, 219, 233);
			colors[(int)OFFICECOLOR.ribbonPanelBorderEnd] = Color.FromArgb(190, 190, 190);
			
			colors[(int)OFFICECOLOR.tabItemSeparator] = Color.FromArgb(54, 54, 54);

			#region ToolStripGallery colors
			colors[(int)OFFICECOLOR.galleryScrollBarBackground] = Color.LightGray;
			colors[(int)OFFICECOLOR.scrollButtonLargeArrow] = Color.FromArgb(68, 81, 113);
			colors[(int)OFFICECOLOR.scrollButtonLargeArrowDisabled] = Color.FromArgb(154, 166, 195);
			colors[(int)OFFICECOLOR.disabledButtonGradientBegin] = Color.FromArgb(243, 243, 243);
			colors[(int)OFFICECOLOR.disabledButtonGradientEnd] = Color.FromArgb(228, 228, 228);
			#endregion

			colors[(int)OFFICECOLOR.headerSeparatorLight] = Color.FromArgb(74, 74, 74);
			colors[(int)OFFICECOLOR.headerSeparatorDark] = Color.FromArgb(65, 65, 65);
			colors[(int)OFFICECOLOR.headerSeparatorLightInActive] = Color.FromArgb(158, 158, 158);
			colors[(int)OFFICECOLOR.headerSeparatorDarkInActive] = Color.FromArgb(154, 154, 154);

			colors[(int)OFFICECOLOR.checkBoxBorder] = Color.FromArgb(132, 132, 132);
			colors[(int)OFFICECOLOR.checkBoxBorderSelected] = Color.FromArgb(132, 132, 132);
			colors[(int)OFFICECOLOR.radioButtonBorderSelected] = Color.FromArgb(0x80, 0x80, 0x80);
		}
		/// <summary>
		/// Gets converted color by adding some values to HSL values.
		/// </summary>
		/// <param name="h"></param>
		/// <param name="s"></param>
		/// <param name="l"></param>
		/// <returns></returns>
		private Color GetConvertedColor(float s, float l)
		{
			ColorHSL hsl = new ColorHSL(OfficeColors[(int)OFFICECOLOR.radioButtonBorderSelected]);

			hsl.S = (int)(hsl.S * s);
			hsl.L = (int)(hsl.L * l);

			return hsl.ToRGB();
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
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void ManagedColorsApplied( Office2007Colors.ManagedColorsAppliedEventArgs args )
		{
			ManagedColors.UpdateColors( args.BaseColor );
		}
		#endregion

		#region Fields
		private Office2007Theme m_officeColorScheme;
		private Color[] m_officeColors;

		private Color m_clExternalBorder = Color.Empty;
		private Color m_clExternalBorderSelected = Color.Empty;
		private Color m_clInternalBorder = Color.Empty;
		private Color m_clInternalBorderSelected = Color.Empty;
		private Color m_clInternalBorderPressed = Color.Empty;
		private Color m_clUncheckedGradientBegin = Color.Empty;
		private Color m_clUncheckedSelectedGradientBegin = Color.Empty;
		private Color m_clUncheckedPressedGradientBegin = Color.Empty;
		private Color m_clUncheckedSelectedGradientEnd = Color.Empty;
		private Color m_clCheckedBackgroundGradientEnd = Color.Empty;
		private Color m_clCheckedSelectedBackgroundGradientEnd = Color.Empty;
		private Color m_clCheckedPressedBackgroundGradientEnd = Color.Empty;
		private Color m_clCheckedInsideBorder = Color.Empty;

		private IntPtr m_ComboBoxBackgroundBrush = IntPtr.Zero;

		private static Office12ColorTable m_managedColors = null;
		#endregion
	}
	#endregion

	#region OfficeBlue
	public class OfficeBlue : Office12ColorTable
	{
		public OfficeBlue():base(Office2007Theme.Blue) {}
	}
	#endregion

	#region OfficeBlack
	public class OfficeBlack : Office12ColorTable
	{
		public OfficeBlack():base(Office2007Theme.Black) {}
	}
	#endregion

    #region Office2010ColorTable

    public class Office2010ColorTable 
        #if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		        :ProfessionalColorTable
        #endif
    {
        #region Fileds
        private Color[] colors = null;
        private Office2010ColorScheme office2010ColorScheme = Office2010ColorScheme.Blue;
        IntPtr m_ComboBoxBackgroundBrush = IntPtr.Zero;
        #endregion

        #region Ctor
        public Office2010ColorTable(Office2010ColorScheme colorScheme)
            : base()
        {
            this.office2010ColorScheme = colorScheme;
        }

        public Office2010ColorTable():this(Office2010ColorScheme.Blue)
        {
        }
        #endregion

        #region Properties

        public Color[] Colors
        {
            get 
            {
                if (colors == null)
                {
                    colors = new Color[(int)Office2010Colors.LastColor];
                    InitColors(ref colors);

                    return colors;
                }

                return colors;
            }
        }

        public virtual Color ToolstripButtonSelectedBottomSurroundColors
        {
            get { return Colors[(int)Office2010Colors.ToolstripButtonSelectedBottomSurroundColors]; }
        }

        public virtual Color ToolstripButtonSelectedBottomCenterColor
        {
            get { return Colors[(int)Office2010Colors.ToolstripButtonSelectedBottomCenterColor]; }
        }

        public virtual Color ToolstripButtonSelectedBorder
        {
            get { return Colors[(int)Office2010Colors.ToolstripButtonSelectedBorder]; }
        }

        public virtual Color ToolstripButtonSelectedBackground
        {
            get { return Colors[(int)Office2010Colors.ToolstripButtonSelectedBackground]; }
        }

        public virtual Color SystemCloseButtonSelectedGradientEnd
        {
            get { return Colors[(int)Office2010Colors.SystemCloseButtonSelectedGradientEnd]; }
        }

        public virtual Color SystemButtonPressedBackground
        {
            get { return Colors[(int)Office2010Colors.SystemButtonPressedBackground]; }
        }

        public virtual Color SystemCloseButtonPressedBackground
        {
            get { return Colors[(int)Office2010Colors.SystemCloseButtonPressedBackground]; }
        }

        public virtual Color SystemCloseButtonSelectedGradientBegin
        {
            get { return Colors[(int)Office2010Colors.SystemCloseButtonSelectedGradientBegin]; }
        }

        public virtual Color SystemCloseButtonBorder
        {
            get { return Colors[(int)Office2010Colors.SystemCloseButtonBorder]; }
        }

        public virtual Color ToolstripButtonPressedBorder
        {
            get { return Colors[(int)Office2010Colors.ToolstripButtonPressedBorder]; }
        }

        public virtual Color ToolstripButtonPressedBackground
        {
            get { return Colors[(int)Office2010Colors.ToolstripButtonPressedBackground]; }
        }

        public virtual Color ToolstripButtonCheckedBackground
        {
            get { return Colors[(int)Office2010Colors.ToolstripButtonCheckedBackground]; }
        }

        public virtual Color ActiveHeaderBackground
        {
            get { return Colors[(int)Office2010Colors.ActiveHeaderBackground]; }
        }

        public virtual Color InActiveHeaderBackground
        {
            get { return Colors[(int)Office2010Colors.InActiveHeaderBackground]; }
        }

        public virtual Color RibbonHeaderBorder
        {
            get { return Colors[(int)Office2010Colors.RibbonHeaderBorder]; }
        }

        public virtual Color SystemButtonBorder
        {
            get { return Colors[(int)Office2010Colors.SystemButtonBorder]; }
        }

        public virtual Color SystemButtonSelectedGradientEnd
        {
            get { return Colors[(int)Office2010Colors.SystemButtonSelectedGradientEnd]; }
        }

        public virtual Color SytemButtonSelectedGradientBegin
        {
            get { return Colors[(int)Office2010Colors.SytemButtonSelectedGradientBegin]; }
        }

        public virtual Color QuickItemSeparatorBorder
        {
            get { return Colors[(int)Office2010Colors.QuickItemSeparatorBorder]; }
        }

        public virtual Color QuickItemSeparatorBackground
        {
            get { return Colors[(int)Office2010Colors.QuickItemSeparatorBackground]; }
        }

        public virtual Color ToolstripTabItemBackgournd
        {
            get { return Colors[(int)Office2010Colors.ToolstripTabItemBackgournd]; }
        }

        public virtual Color ToolstripTabItemBorder
        {
            get { return Colors[(int)Office2010Colors.ToolstripTabItemBorder]; }
        }

        public virtual Color ToolstripTabItemAntialiasing
        {
            get { return Colors[(int)Office2010Colors.ToolstripTabItemAntialiasing]; }
        }

        public virtual Color ToolstripTabItemSelectedGradientBegin
        {
            get { return Colors[(int)Office2010Colors.ToolstripTabItemSelectedGradientBegin]; }
        }

        public virtual Color ToolstripTabItemSelectedGradientEnd
        {
            get { return Colors[(int)Office2010Colors.ToolstripTabItemSelectedGradientEnd]; }
        }

        public virtual Color ToolstripTabItemCheckedGradientEnd
        {
            get { return Colors[(int)Office2010Colors.ToolstripTabItemCheckedGradientEnd]; }
        }

        public virtual Color ToolstripTabItemCheckedGradientBegin
        {
            get { return Colors[(int)Office2010Colors.ToolstripTabItemCheckedGradientBegin]; }
        }

        public virtual Color ToolstripTabItemCheckedForeColor
        {
            get { return Colors[(int)Office2010Colors.ToolstripTabItemCheckedForeColor]; }
        }

        public virtual Color ToolstripTabItemForeColor
        {
            get { return Colors[(int)Office2010Colors.ToolstripTabItemForeColor]; }
        }

        public virtual Color FormBorderInActive
        {
            get { return Colors[(int)Office2010Colors.FormBorderInActive]; }
        }

        public virtual Color FormBorderActive
        {
            get { return Colors[(int)Office2010Colors.FormBorderActive]; }
        }

        public virtual Color TabItemSeparatorGradientBegin
        {
            get { return Colors[(int)Office2010Colors.TabItemSeparatorGradientBegin]; }
        }

        public virtual Color TabScrollButtonBorder
        {
            get { return Colors[(int)Office2010Colors.TabScrollButtonBorder]; }
        }

        public virtual Color TabScrollButtonGradientBegin
        {
            get { return Colors[(int)Office2010Colors.TabScrollButtonGradientBegin]; }
        }

        public virtual Color TabScrollButtonGradientEnd
        {
            get { return Colors[(int)Office2010Colors.TabScrollButtonGradientEnd]; }
        }

        public virtual Color RibbonPanelBackgroundGradientBegin
        {
            get { return Colors[(int)Office2010Colors.RibbonPanelBackgroundGradientBegin]; }
        }

        public virtual Color RibbonPanelBackgroundGradientEnd
        {
            get { return Colors[(int)Office2010Colors.RibbonPanelBackgroundGradientEnd]; }
        }

        public virtual Color ComboBoxBackgroundColor
        {
            get { return Colors[(int)Office2010Colors.ComboBoxBackgroundColor]; }
        }

        public virtual Color ComboBoxBorderColor
        {
            get { return Colors[(int)Office2010Colors.ComboBoxBorderColor]; }
        }

        public virtual Color ComboDropDownSelectedGradientBegin
        {
            get { return Colors[(int)Office2010Colors.ComboDropDownSelectedGradientBegin]; }
        }

        public virtual Color ComboDropDownSelectedGradientEnd
        {
            get { return Colors[(int)Office2010Colors.ComboDropDownSelectedGradientEnd]; }
        }

        public virtual Color CaptionText
        {
            get { return Colors[(int)Office2010Colors.CaptionText]; }
        }

        public virtual Color ContextMenuTitleBackground
        {
            get { return Colors[(int)Office2010Colors.ContextMenuTitleBackground]; }
        }

        public virtual Color GripGradientEnd
        {
            get { return Colors[(int)Office2010Colors.GripGradientEnd]; }
        }

        public virtual Color BackStageBackgroundGradientBegin
        {
            get { return Colors[(int)Office2010Colors.BackStageBackgroundGradientBegin]; }
        }

        public virtual Color BackStageBackgroundGradientEnd
        {
            get { return Colors[(int)Office2010Colors.BackStageBackgroundGradientEnd]; }
        }

        public virtual Color BackStagePageAdornerBackgroundGradient
        {
            get { return Colors[(int)Office2010Colors.BackStagePageAdornerBackgroundGradient]; }
        }

        public virtual Color BackStagePageTriangleClip
        {
            get { return Colors[(int)Office2010Colors.BackStagePageTriangleClip]; }
        }

        public virtual Color BackStageHotTrackColor
        {
            get { return Colors[(int)Office2010Colors.BackStageHotTrackColor]; }
        }

        public virtual Color BackStageButtonBackgroundGradientBegin
        {
            get { return Colors[(int)Office2010Colors.BackStageButtonBackgroundGradientBegin]; }
        }

        public virtual Color BackStageButtonBackgroundGradientEnd
        {
            get { return Colors[(int)Office2010Colors.BackStageButtonBackgroundGradientEnd]; }
        }

        public virtual Color BackStagePageSurroundColor
        {
            get { return Colors[(int)Office2010Colors.BackStagePageSurroundColor]; }
        }

        public virtual Color MenuButtonAntializing
        {
            get { return Colors[(int)Office2010Colors.MenuButtonAntializing]; }
        }

        public virtual Color MenuButtonBorder
        {
            get { return Colors[(int)Office2010Colors.MenuButtonBorder]; }
        }
        public virtual Color MenuButtonGlowCenterColor
        {
            get { return Colors[(int)Office2010Colors.MenuButtonGlowCenterColor]; }
        }

        public virtual Color MenuButtonGlowSurroundColor
        {
            get { return Colors[(int)Office2010Colors.MenuButtonGlowSurroundColor]; }
        }

        public virtual Color MenuButtonLowerBackground
        {
            get { return Colors[(int)Office2010Colors.MenuButtonLowerBackground]; }
        }

        public virtual Color MenuButtonSelectedBorder
        {
            get { return Colors[(int)Office2010Colors.MenuButtonSelectedBorder]; }
        }

        public virtual Color MenuButtonUpperGradientBegin
        {
            get { return Colors[(int)Office2010Colors.MenuButtonUpperGradientBegin]; }
        }

        public virtual Color MenuButtonUpperGradientEnd
        {
            get { return Colors[(int)Office2010Colors.MenuButtonUpperGradientEnd]; }
        }

        public virtual Color BackStagePanelGradientBegin
        {
            get { return Colors[(int)Office2010Colors.BackStagePanelGradientBegin]; }
        }

        public virtual Color BackStagePanelGradientEnd
        {
            get { return Colors[(int)Office2010Colors.BackStagePanelGradientEnd]; }
        }

        public virtual Color HeaderAeroBackground
        {
            get { return Colors[(int)Office2010Colors.HeaderAeroBackground]; }
        }

        public virtual Color ContextMenuImageMargin
        {
            get { return Colors[(int)Office2010Colors.ContextMenuImageMargin]; }
        }

        #endregion

        #region *** Brushes
        /// <summary>
        /// 
        /// </summary>
        internal IntPtr ComboBoxBackgroundBrush
        {
            get
            {
                if (m_ComboBoxBackgroundBrush == IntPtr.Zero)
                {
                    Color c = this.ComboBoxBackgroundColor;
                    m_ComboBoxBackgroundBrush = WindowsAPI.CreateSolidBrush((uint)(c.R | (c.G << 8) | (c.B << 16)));
                }
                return m_ComboBoxBackgroundBrush;
            }
        }
        #endregion

        #region Implementations
        
        private void InitColors(ref Color[] colors)
        {
            switch (this.office2010ColorScheme)
            {
                case Office2010ColorScheme.Black:
                    InitBlackColors(ref colors);
                    break;
                case Office2010ColorScheme.Silver:
                    InitSilverColors(ref colors);
                    break;
                default :/*Office2010ColorScheme.Blue*/
                    InitBlueColors(ref colors);
                    break;
            }
        }

        private void InitBlackColors(ref Color[] colors)
        {
            colors[(int)Office2010Colors.InActiveHeaderBackground] =  Color.FromArgb(158,158,158);
            colors[(int)Office2010Colors.ActiveHeaderBackground] = Color.FromArgb(113, 113, 113);
            colors[(int)Office2010Colors.RibbonHeaderBorder] = Color.FromArgb(119, 119, 119);
            colors[(int)Office2010Colors.SystemButtonSelectedGradientEnd] = Color.FromArgb(119, 119, 119);
            colors[(int)Office2010Colors.SytemButtonSelectedGradientBegin] = Color.FromArgb(148, 148, 148);
            colors[(int)Office2010Colors.SystemButtonBorder] = Color.FromArgb(81, 81, 81);
            colors[(int)Office2010Colors.SystemCloseButtonBorder] = Color.FromArgb(155, 61, 61);
            colors[(int)Office2010Colors.SystemCloseButtonSelectedGradientEnd] = Color.FromArgb(227, 97, 98);
            colors[(int)Office2010Colors.SystemCloseButtonSelectedGradientBegin] = Color.FromArgb(255, 132, 130);
            colors[(int)Office2010Colors.ToolstripButtonSelectedBottomCenterColor] = Color.FromArgb(255, 250, 230);
            colors[(int)Office2010Colors.ToolstripButtonSelectedBottomSurroundColors] = Color.FromArgb(12, 255, 250, 230);
            colors[(int)Office2010Colors.ToolstripButtonSelectedBackground] = Color.FromArgb(236, 211, 121);
            colors[(int)Office2010Colors.ToolstripButtonSelectedBorder] = Color.FromArgb(247, 200, 64);
            colors[(int)Office2010Colors.ToolstripButtonPressedBorder] = Color.FromArgb(194, 136, 56);
            colors[(int)Office2010Colors.ToolstripButtonPressedBackground] = Color.FromArgb(255, 228, 138);
            colors[(int)Office2010Colors.ToolstripButtonCheckedBackground] = Color.FromArgb(255, 217, 108);
            colors[(int)Office2010Colors.SystemCloseButtonPressedBackground] = Color.FromArgb(242, 119, 118);
            colors[(int)Office2010Colors.SystemButtonPressedBackground] = Color.FromArgb(113, 113, 113);
            colors[(int)Office2010Colors.QuickItemSeparatorBackground] = Color.FromArgb(86, 86, 86);
            colors[(int)Office2010Colors.QuickItemSeparatorBorder] = Color.FromArgb(171, 171, 171);
            colors[(int)Office2010Colors.ToolstripTabItemBorder] = Color.FromArgb(94,94,94);
            colors[(int)Office2010Colors.ToolstripTabItemAntialiasing] = Color.FromArgb(184,184,184);
            colors[(int)Office2010Colors.ToolstripTabItemSelectedGradientBegin] = Color.FromArgb(148,149,151);
            colors[(int)Office2010Colors.ToolstripTabItemSelectedGradientEnd] = Color.FromArgb(124,124,124);
            colors[(int)Office2010Colors.ToolstripTabItemBackgournd] = Color.FromArgb(126, 126, 126);
            colors[(int)Office2010Colors.ToolstripTabItemCheckedGradientBegin] = Color.FromArgb(207, 207, 207);
            colors[(int)Office2010Colors.ToolstripTabItemCheckedGradientEnd] = Color.FromArgb(201, 201, 201);
            colors[(int)Office2010Colors.ToolstripTabItemForeColor] = Color.White;
            colors[(int)Office2010Colors.ToolstripTabItemCheckedForeColor] = Color.Black;
            colors[(int)Office2010Colors.FormBorderActive] = Color.FromArgb(99, 99, 99);
            colors[(int)Office2010Colors.FormBorderInActive] = Color.FromArgb(119, 119, 119);
            colors[(int)Office2010Colors.TabItemSeparatorGradientBegin] = Color.FromArgb(182, 186, 191);
            colors[(int)Office2010Colors.TabScrollButtonGradientBegin] = Color.FromArgb(207, 207, 207);
            colors[(int)Office2010Colors.TabScrollButtonGradientEnd] = Color.FromArgb(164, 164, 164);
            colors[(int)Office2010Colors.TabScrollButtonBorder] = Color.FromArgb(97, 97, 97);
            colors[(int)Office2010Colors.RibbonPanelBackgroundGradientBegin] = Color.FromArgb(201, 201, 201);
            colors[(int)Office2010Colors.RibbonPanelBackgroundGradientEnd] = Color.FromArgb(165, 165, 165);
            colors[(int)Office2010Colors.ComboBoxBackgroundColor] = Color.FromArgb(198, 198, 198);
            colors[(int)Office2010Colors.ComboBoxBorderColor] = Color.FromArgb(145, 145, 145);
            colors[(int)Office2010Colors.ComboDropDownSelectedGradientBegin] = Color.FromArgb(226, 226, 226);
            colors[(int)Office2010Colors.ComboDropDownSelectedGradientEnd] = Color.FromArgb(196, 196, 196);
            colors[(int)Office2010Colors.CaptionText] = ColorTranslator.FromHtml("#242424");
            colors[(int)Office2010Colors.ContextMenuTitleBackground] = Color.FromArgb(240, 242, 245);
            colors[(int)Office2010Colors.GripGradientEnd] = Color.FromArgb(168, 168, 168);
            colors[(int)Office2010Colors.BackStageBackgroundGradientBegin] = Color.FromArgb(255, 213, 66);
            colors[(int)Office2010Colors.BackStageBackgroundGradientEnd] = Color.FromArgb(252, 169, 38);
            colors[(int)Office2010Colors.BackStagePageAdornerBackgroundGradient] = ColorTranslator.FromHtml("#323131");
            colors[(int)Office2010Colors.BackStagePageSurroundColor] = Color.FromArgb(230, 129, 26);
            colors[(int)Office2010Colors.BackStagePageTriangleClip] = Color.White;
            colors[(int)Office2010Colors.BackStageHotTrackColor] = Color.FromArgb(218, 180, 45);
            colors[(int)Office2010Colors.BackStageButtonBackgroundGradientBegin] = Color.FromArgb(113, 113, 113);
            colors[(int)Office2010Colors.BackStageButtonBackgroundGradientEnd] = Color.FromArgb(58, 58, 58);
            colors[(int)Office2010Colors.MenuButtonAntializing] = Color.FromArgb(252, 191, 49);
            colors[(int)Office2010Colors.MenuButtonBorder] = Color.FromArgb(225, 116, 19);
            colors[(int)Office2010Colors.MenuButtonGlowCenterColor] = Color.White;
            colors[(int)Office2010Colors.MenuButtonGlowSurroundColor] = Color.Transparent;
            colors[(int)Office2010Colors.MenuButtonLowerBackground] = Color.FromArgb(230, 129, 26);
            colors[(int)Office2010Colors.MenuButtonSelectedBorder] = Color.FromArgb(254, 212, 112);
            colors[(int)Office2010Colors.MenuButtonUpperGradientBegin] = Color.FromArgb(250, 195, 69);
            colors[(int)Office2010Colors.MenuButtonUpperGradientEnd] = Color.FromArgb(230, 129, 26);
            colors[(int)Office2010Colors.BackStagePanelGradientBegin] = Color.FromArgb(113, 113, 113);
            colors[(int)Office2010Colors.BackStagePanelGradientEnd] = Color.FromArgb(58, 58, 58);
            colors[(int)Office2010Colors.HeaderAeroBackground] = Color.FromArgb(81, 81, 81);
            colors[(int)Office2010Colors.ContextMenuImageMargin] = Color.FromArgb(167, 171, 176);
        }

        private void InitSilverColors(ref Color[] colors)
        {
            colors[(int)Office2010Colors.InActiveHeaderBackground] = Color.FromArgb(252, 252, 252);
            colors[(int)Office2010Colors.ActiveHeaderBackground] = Color.FromArgb(229, 231, 233);
            colors[(int)Office2010Colors.RibbonHeaderBorder] = Color.FromArgb(101, 109, 117);
            colors[(int)Office2010Colors.SystemButtonSelectedGradientEnd] = Color.FromArgb(231, 232, 233);
            colors[(int)Office2010Colors.SytemButtonSelectedGradientBegin] = Color.White;
            colors[(int)Office2010Colors.SystemButtonBorder] = Color.FromArgb(166, 172, 179);
            colors[(int)Office2010Colors.SystemCloseButtonBorder] = Color.FromArgb(155, 61, 61);
            colors[(int)Office2010Colors.SystemCloseButtonSelectedGradientEnd] = Color.FromArgb(227, 97, 98);
            colors[(int)Office2010Colors.SystemCloseButtonSelectedGradientBegin] = Color.FromArgb(255, 132, 130);
            colors[(int)Office2010Colors.ToolstripButtonSelectedBottomCenterColor] = Color.FromArgb(255, 250, 230);
            colors[(int)Office2010Colors.ToolstripButtonSelectedBottomSurroundColors] = Color.FromArgb(12, 255, 250, 230);
            colors[(int)Office2010Colors.ToolstripButtonSelectedBackground] = Color.FromArgb(236, 211, 121);
            colors[(int)Office2010Colors.ToolstripButtonSelectedBorder] = Color.FromArgb(247, 200, 64);
            colors[(int)Office2010Colors.ToolstripButtonPressedBorder] = Color.FromArgb(194, 136, 56);
            colors[(int)Office2010Colors.ToolstripButtonPressedBackground] = Color.FromArgb(255, 228, 138);
            colors[(int)Office2010Colors.ToolstripButtonCheckedBackground] = Color.FromArgb(255, 217, 108);
            colors[(int)Office2010Colors.SystemCloseButtonPressedBackground] = Color.FromArgb(242, 119, 118);
            colors[(int)Office2010Colors.SystemButtonPressedBackground] = Color.FromArgb(194, 199, 206);
            colors[(int)Office2010Colors.QuickItemSeparatorBackground] = Color.FromArgb(150, 152, 153);
            colors[(int)Office2010Colors.QuickItemSeparatorBorder] = Color.FromArgb(215, 226, 239);
            colors[(int)Office2010Colors.ToolstripTabItemBorder] = Color.FromArgb(177, 181, 186);
            colors[(int)Office2010Colors.ToolstripTabItemAntialiasing] = Color.FromArgb(249, 250, 250);
            colors[(int)Office2010Colors.ToolstripTabItemSelectedGradientBegin] = Color.FromArgb(229, 232, 235);
            colors[(int)Office2010Colors.ToolstripTabItemSelectedGradientEnd] = Color.FromArgb(231, 234, 235);
            colors[(int)Office2010Colors.ToolstripTabItemBackgournd] = Color.FromArgb(231, 233, 235);
            colors[(int)Office2010Colors.ToolstripTabItemCheckedGradientBegin] = Color.White;
            colors[(int)Office2010Colors.ToolstripTabItemCheckedGradientEnd] = Color.White;
            colors[(int)Office2010Colors.ToolstripTabItemForeColor] = Color.FromArgb(59, 59, 59);
            colors[(int)Office2010Colors.ToolstripTabItemCheckedForeColor] = Color.FromArgb(76, 83, 92);
            colors[(int)Office2010Colors.FormBorderActive] = Color.FromArgb(101, 109, 117);
            colors[(int)Office2010Colors.FormBorderInActive] = Color.FromArgb(134, 139, 145);
            colors[(int)Office2010Colors.TabItemSeparatorGradientBegin] = Color.FromArgb(182, 186, 191);
            colors[(int)Office2010Colors.TabScrollButtonGradientBegin] = Color.White;
            colors[(int)Office2010Colors.TabScrollButtonGradientEnd] = Color.FromArgb(232, 235, 239);
            colors[(int)Office2010Colors.TabScrollButtonBorder] = Color.FromArgb(119, 131, 139);
            colors[(int)Office2010Colors.RibbonPanelBackgroundGradientBegin] = Color.White;
            colors[(int) Office2010Colors.RibbonPanelBackgroundGradientEnd] = Color.FromArgb(229, 233, 238);
            colors[(int)Office2010Colors.ComboBoxBackgroundColor] = Color.White;
            colors[(int)Office2010Colors.ComboBoxBorderColor] = Color.FromArgb(187, 191, 196);
            colors[(int)Office2010Colors.ComboDropDownSelectedGradientBegin] = Color.White;
            colors[(int)Office2010Colors.ComboDropDownSelectedGradientEnd] = Color.FromArgb(241, 243, 244);
            colors[(int)Office2010Colors.CaptionText] = ColorTranslator .FromHtml ("#666D7C");
            colors[(int)Office2010Colors.ContextMenuTitleBackground] = Color.FromArgb(240, 242, 245);
            colors[(int)Office2010Colors.GripGradientEnd] = Color.FromArgb(181, 190, 199);
            colors[(int)Office2010Colors.BackStageBackgroundGradientBegin] = Color.FromArgb(218, 99, 203);
            colors[(int) Office2010Colors.BackStageBackgroundGradientEnd] = Color.FromArgb(164, 48, 158);
            colors[(int)Office2010Colors.BackStagePageAdornerBackgroundGradient] = ColorTranslator.FromHtml("#878C92");
            colors[(int)Office2010Colors.BackStagePageSurroundColor] = Color.FromArgb(131, 24, 132);
            colors[(int)Office2010Colors.BackStagePageTriangleClip] = Color.White;
            colors[(int)Office2010Colors.BackStageHotTrackColor] = Color.FromArgb(197, 70, 194);
            colors[(int)Office2010Colors.BackStageButtonBackgroundGradientBegin] = Color.FromArgb(113, 113, 113);
            colors[(int)Office2010Colors.BackStageButtonBackgroundGradientEnd] = Color.FromArgb(58, 58, 58);
            colors[(int)Office2010Colors.MenuButtonAntializing] = Color.FromArgb(198, 94, 189);
            colors[(int)Office2010Colors.MenuButtonBorder] = Color.FromArgb(87, 29, 112);
            colors[(int)Office2010Colors.MenuButtonGlowCenterColor] = Color.White;
            colors[(int)Office2010Colors.MenuButtonGlowSurroundColor] = Color.Transparent;
            colors[(int)Office2010Colors.MenuButtonLowerBackground] = Color.FromArgb(151, 56, 145);
            colors[(int)Office2010Colors.MenuButtonSelectedBorder] = Color.FromArgb(198,140,232);
            colors[(int)Office2010Colors.MenuButtonUpperGradientBegin] = Color.FromArgb(187, 77, 179);
            colors[(int)Office2010Colors.MenuButtonUpperGradientEnd] = Color.FromArgb(151, 56, 145);
            colors[(int)Office2010Colors.BackStagePanelGradientBegin] = Color.FromArgb(251, 252, 253);
            colors[(int)Office2010Colors.BackStagePanelGradientEnd] = Color.FromArgb(224, 227, 231);
            colors[(int) Office2010Colors.HeaderAeroBackground] = Color.White;
            colors[(int)Office2010Colors.ContextMenuImageMargin] = Color.FromArgb(167, 171, 176);
        }

        private void InitBlueColors(ref Color[] colors)
        {
            colors[(int)Office2010Colors.InActiveHeaderBackground] = Color.FromArgb(223, 235, 247);
            colors[(int)Office2010Colors.ActiveHeaderBackground] = Color.FromArgb(187, 206, 230);
            colors[(int)Office2010Colors.RibbonHeaderBorder] = Color.FromArgb(144, 154, 166);
            colors[(int)Office2010Colors.SystemButtonSelectedGradientEnd] = Color.FromArgb(191, 210, 234);
            colors[(int)Office2010Colors.SytemButtonSelectedGradientBegin] = Color.FromArgb(212, 232, 255);
            colors[(int)Office2010Colors.SystemButtonBorder] = Color.FromArgb(143, 165, 191);
            colors[(int)Office2010Colors.SystemCloseButtonBorder] = Color.FromArgb(155, 61, 61);
            colors[(int)Office2010Colors.SystemCloseButtonSelectedGradientEnd] = Color.FromArgb(227, 97, 98);
            colors[(int)Office2010Colors.SystemCloseButtonSelectedGradientBegin] = Color.FromArgb(255, 132, 130);
            colors[(int)Office2010Colors.ToolstripButtonSelectedBottomCenterColor] = Color.FromArgb(255, 250, 230);
            colors[(int)Office2010Colors.ToolstripButtonSelectedBottomSurroundColors] = Color.FromArgb(12, 255, 250, 230);
            colors[(int)Office2010Colors.ToolstripButtonSelectedBackground] = Color.FromArgb(236, 211, 121);
            colors[(int)Office2010Colors.ToolstripButtonSelectedBorder] = Color.FromArgb(247, 200, 64);
            colors[(int)Office2010Colors.ToolstripButtonPressedBorder] = Color.FromArgb(194, 136, 56);
            colors[(int)Office2010Colors.ToolstripButtonPressedBackground] = Color.FromArgb(255, 228, 138);
            colors[(int)Office2010Colors.ToolstripButtonCheckedBackground] = Color.FromArgb(255, 217, 108);
            colors[(int)Office2010Colors.SystemCloseButtonPressedBackground] = Color.FromArgb(242, 119, 118);
            colors[(int)Office2010Colors.SystemButtonPressedBackground] = Color.FromArgb(173, 193, 219);
            colors[(int)Office2010Colors.QuickItemSeparatorBackground] = Color.FromArgb(86, 86, 86);
            colors[(int)Office2010Colors.QuickItemSeparatorBorder] = Color.FromArgb(215, 226, 239);
            colors[(int)Office2010Colors.ToolstripTabItemBorder] = Color.FromArgb(177, 181, 186);
            colors[(int)Office2010Colors.ToolstripTabItemAntialiasing] = Color.FromArgb(215, 224, 234);
            colors[(int)Office2010Colors.ToolstripTabItemSelectedGradientBegin] = Color.FromArgb(214, 223, 234);
            colors[(int)Office2010Colors.ToolstripTabItemSelectedGradientEnd] = Color.FromArgb(201, 216, 234);
            colors[(int)Office2010Colors.ToolstripTabItemBackgournd] = Color.FromArgb(200, 215, 233);
            colors[(int)Office2010Colors.ToolstripTabItemCheckedGradientBegin] = Color.FromArgb(245, 250, 255);
            colors[(int)Office2010Colors.ToolstripTabItemCheckedGradientEnd] = Color.FromArgb(239, 246, 253);
            colors[(int)Office2010Colors.ToolstripTabItemForeColor] = Color.FromArgb(30,57,91);
            colors[(int)Office2010Colors.ToolstripTabItemCheckedForeColor] = Color.FromArgb(30, 57, 91);
            colors[(int)Office2010Colors.FormBorderActive] = Color.FromArgb(144, 154, 166);
            colors[(int)Office2010Colors.FormBorderInActive] = Color.FromArgb(162, 173, 185);
            colors[(int)Office2010Colors.TabItemSeparatorGradientBegin] = Color.FromArgb(182, 186, 191);
            colors[(int)Office2010Colors.TabScrollButtonGradientBegin] = Color.FromArgb(225, 235, 247);
            colors[(int)Office2010Colors.TabScrollButtonGradientEnd] = Color.FromArgb(222, 233, 245);
            colors[(int)Office2010Colors.TabScrollButtonBorder] = Color.FromArgb(148, 168, 194);
            colors[(int)Office2010Colors.RibbonPanelBackgroundGradientBegin] = Color.FromArgb(239, 246, 253);
            colors[(int)Office2010Colors.RibbonPanelBackgroundGradientEnd] = Color.FromArgb(216, 228, 242);
            colors[(int)Office2010Colors.ComboBoxBackgroundColor] = Color.FromArgb(237, 245, 253);
            colors[(int)Office2010Colors.ComboBoxBorderColor] = Color.FromArgb(177, 192, 214);
            colors[(int)Office2010Colors.ComboDropDownSelectedGradientBegin] = Color.FromArgb(236, 245, 253);
            colors[(int)Office2010Colors.ComboDropDownSelectedGradientEnd] = Color.FromArgb(212, 229, 246);
            colors[(int)Office2010Colors.CaptionText] =  ColorTranslator.FromHtml("#384E73");
            colors[(int)Office2010Colors.ContextMenuTitleBackground] = Color.FromArgb(240, 242, 245);
            colors[(int)Office2010Colors.GripGradientEnd] = Color.FromArgb(188, 198, 209);
            colors[(int)Office2010Colors.BackStageBackgroundGradientBegin] = Color.FromArgb(237, 245, 252);
            colors[(int)Office2010Colors.BackStageBackgroundGradientEnd] = Color.FromArgb(195, 212, 235);
            colors[(int)Office2010Colors.BackStagePageAdornerBackgroundGradient] = ColorTranslator.FromHtml("#728EAD");
            colors[(int)Office2010Colors.BackStagePageSurroundColor] = Color.FromArgb(30, 70, 160);
            colors[(int)Office2010Colors.BackStagePageTriangleClip] = Color.White;
            colors[(int)Office2010Colors.BackStageHotTrackColor] = Color.FromArgb(73, 145, 245);
            colors[(int)Office2010Colors.BackStageButtonBackgroundGradientBegin] = Color.FromArgb(113, 113, 113);
            colors[(int)Office2010Colors.BackStageButtonBackgroundGradientEnd] = Color.FromArgb(58, 58, 58);
            colors[(int)Office2010Colors.MenuButtonAntializing] = Color.FromArgb(68, 136, 229);
            colors[(int)Office2010Colors.MenuButtonBorder] = Color.FromArgb(31, 72, 161);
            colors[(int)Office2010Colors.MenuButtonGlowCenterColor] = Color.White;
            colors[(int)Office2010Colors.MenuButtonGlowSurroundColor] = Color.FromArgb(38, 96, 179);
            colors[(int)Office2010Colors.MenuButtonLowerBackground] = Color.FromArgb(38, 96, 179);
            colors[(int)Office2010Colors.MenuButtonSelectedBorder] = Color.FromArgb(100, Color.SkyBlue);
            colors[(int)Office2010Colors.MenuButtonUpperGradientBegin] = Color.FromArgb(67, 134, 226);
            colors[(int)Office2010Colors.MenuButtonUpperGradientEnd] = Color.FromArgb(41, 95, 182);
            colors[(int)Office2010Colors.BackStagePanelGradientBegin] = Color.FromArgb(237, 245, 252);
            colors[(int)Office2010Colors.BackStagePanelGradientEnd] = Color.FromArgb(195, 212, 235);
            colors[(int) Office2010Colors.HeaderAeroBackground] = Color.White;
            colors[(int)Office2010Colors.ContextMenuImageMargin] = Color.FromArgb(167, 171, 176);
        }
        
        #endregion

        #region Enum
        protected enum Office2010Colors
        {
            InActiveHeaderBackground = 0,
            ActiveHeaderBackground,
            RibbonHeaderBorder,
            SytemButtonSelectedGradientBegin,
            SystemButtonSelectedGradientEnd,
            SystemButtonBorder,
            SystemCloseButtonSelectedGradientBegin,
            SystemCloseButtonSelectedGradientEnd,
            SystemCloseButtonBorder,
            SystemCloseButtonPressedBackground,
            SystemButtonPressedBackground,
            ToolstripButtonSelectedBackground,
            ToolstripButtonSelectedBorder,
            ToolstripButtonSelectedBottomCenterColor,
            ToolstripButtonSelectedBottomSurroundColors,
            ToolstripButtonPressedBorder,
            ToolstripButtonPressedBackground,
            ToolstripButtonCheckedBackground,
            QuickItemSeparatorBorder,
            QuickItemSeparatorBackground,
            ToolstripTabItemBorder,
            ToolstripTabItemSelectedGradientBegin,
            ToolstripTabItemSelectedGradientEnd,
            ToolstripTabItemBackgournd,
            ToolstripTabItemAntialiasing,
            ToolstripTabItemCheckedGradientBegin,
            ToolstripTabItemCheckedGradientEnd,
            ToolstripTabItemCheckedForeColor,
            ToolstripTabItemForeColor,
            FormBorderActive,
            FormBorderInActive,
            TabItemSeparatorGradientBegin,
            TabScrollButtonGradientBegin,
            TabScrollButtonGradientEnd,
            TabScrollButtonBorder,
            RibbonPanelBackgroundGradientBegin,
            RibbonPanelBackgroundGradientEnd,
            ComboBoxBackgroundColor,
            ComboBoxBorderColor,
            ComboDropDownSelectedGradientBegin,
            ComboDropDownSelectedGradientEnd,
            CaptionText,
            ContextMenuTitleBackground,
            GripGradientEnd,
            BackStageBackgroundGradientBegin,
            BackStageBackgroundGradientEnd,
            BackStagePageAdornerBackgroundGradient,
            BackStagePageSurroundColor,
            BackStagePageTriangleClip,
            BackStageHotTrackColor,
            BackStageButtonBackgroundGradientEnd,
            BackStageButtonBackgroundGradientBegin,
            MenuButtonUpperGradientBegin,
            MenuButtonUpperGradientEnd,
            MenuButtonLowerBackground,
            MenuButtonGlowCenterColor,
            MenuButtonGlowSurroundColor,
            MenuButtonAntializing,
            MenuButtonSelectedBorder,
            MenuButtonBorder,
            BackStagePanelGradientBegin,
            BackStagePanelGradientEnd,
            HeaderAeroBackground,
            ContextMenuImageMargin,

            LastColor
        }
        #endregion

    }
    #endregion
}