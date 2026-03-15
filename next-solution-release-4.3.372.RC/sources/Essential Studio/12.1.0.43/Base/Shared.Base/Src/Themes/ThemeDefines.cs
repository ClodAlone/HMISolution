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

namespace Syncfusion.Windows.Forms
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public class DrawTextFormats
	{
		public const int DT_TOP    =        0x00000000;
		public const int DT_LEFT =           0x00000000;
		public const int DT_CENTER =                  0x00000001;
		public const int DT_RIGHT   =                 0x00000002;
		public const int DT_VCENTER  =                0x00000004;
		public const int DT_BOTTOM    =               0x00000008;
		public const int DT_WORDBREAK  =              0x00000010;
		public const int DT_SINGLELINE  =             0x00000020;
		public const int DT_EXPANDTABS   =            0x00000040;
		public const int DT_TABSTOP       =           0x00000080;
		public const int DT_NOCLIP     =              0x00000100;
		public const int DT_EXTERNALLEADING =         0x00000200;
		public const int DT_CALCRECT         =        0x00000400;
		public const int DT_NOPREFIX          =       0x00000800;
		public const int DT_INTERNAL           =      0x00001000;
		//#if(WINVER >= 0x0400)
		public const int DT_EDITCONTROL        =      0x00002000;
		public const int DT_PATH_ELLIPSIS      =      0x00004000;
		public const int DT_END_ELLIPSIS       =      0x00008000;
		public const int DT_MODIFYSTRING       =      0x00010000;
		public const int DT_RTLREADING         =      0x00020000;
		public const int DT_WORD_ELLIPSIS      =      0x00040000;
		//#if(WINVER >= 0x0500)
		public const int DT_NOFULLWIDTHCHARBREAK=     0x00080000;
		public const int DT_HIDEPREFIX          =     0x00100000;
		public const int DT_PREFIXONLY          =     0x00200000;
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public enum PropertyTypes
	{
		dummy = 200,
		STRING,
		INT,
		BOOL,
		COLOR,
		MARGINS,
		FILENAME,
		SIZE,
		POSITION,
		RECT,
		FONT,
		INTLIST
	}
	
	[Syncfusion.Documentation.DocumentationExclude()]
	public enum ThemeProperties
	{
		STRING = 201,
		INT = 202,
		BOOL = 203,
		COLOR = 204,
		MARGINS = 205,
		FILENAME = 206,
		SIZE = 207,
		POSITION = 208,
		RECT = 209,
		FONT = 210,
		INTLIST = 211,
		COLORSCHEMES = 401,
		SIZES = 402,
		CHARSET = 403,
		DISPLAYNAME = 601,
		TOOLTIP = 602,
		COMPANY = 603,
		AUTHOR = 604,
		COPYRIGHT = 605,
		URL = 606,
		VERSION = 607,
		DESCRIPTION = 608,
		CAPTIONFONT = 801,
		SMALLCAPTIONFONT = 802,
		MENUFONT = 803,
		STATUSFONT = 804,
		MSGBOXFONT = 805,
		ICONTITLEFONT = 806,
		FLATMENUS = 1001,
		SIZINGBORDERWIDTH = 1201,
		SCROLLBARWIDTH = 1202,
		SCROLLBARHEIGHT = 1203,
		CAPTIONBARWIDTH = 1204,
		CAPTIONBARHEIGHT = 1205,
		SMCAPTIONBARWIDTH = 1206,
		SMCAPTIONBARHEIGHT = 1207,
		MENUBARWIDTH = 1208,
		MENUBARHEIGHT = 1209,
		MINCOLORDEPTH = 1301,
		CSSNAME = 1401,
		XMLNAME = 1402,
		SCROLLBAR = 1601,
		BACKGROUND = 1602,
		ACTIVECAPTION = 1603,
		INACTIVECAPTION = 1604,
		MENU = 1605,
		WINDOW = 1606,
		WINDOWFRAME = 1607,
		MENUTEXT = 1608,
		WINDOWTEXT = 1609,
		CAPTIONTEXT = 1610,
		ACTIVEBORDER = 1611,
		INACTIVEBORDER = 1612,
		APPWORKSPACE = 1613,
		HIGHLIGHT = 1614,
		HIGHLIGHTTEXT = 1615,
		BTNFACE = 1616,
		BTNSHADOW = 1617,
		GRAYTEXT = 1618,
		BTNTEXT = 1619,
		INACTIVECAPTIONTEXT = 1620,
		BTNHIGHLIGHT = 1621,
		DKSHADOW3D = 1622,
		LIGHT3D = 1623,
		INFOTEXT = 1624,
		INFOBK = 1625,
		BUTTONALTERNATEFACE = 1626,
		HOTTRACKING = 1627,
		GRADIENTACTIVECAPTION = 1628,
		GRADIENTINACTIVECAPTION = 1629,
		MENUHILIGHT = 1630,
		MENUBAR = 1631,
		FROMHUE1 = 1801,
		FROMHUE2 = 1802,
		FROMHUE3 = 1803,
		FROMHUE4 = 1804,
		FROMHUE5 = 1805,
		TOHUE1 = 1806,
		TOHUE2 = 1807,
		TOHUE3 = 1808,
		TOHUE4 = 1809,
		TOHUE5 = 1810,
		FROMCOLOR1 = 2001,
		FROMCOLOR2 = 2002,
		FROMCOLOR3 = 2003,
		FROMCOLOR4 = 2004,
		FROMCOLOR5 = 2005,
		TOCOLOR1 = 2006,
		TOCOLOR2 = 2007,
		TOCOLOR3 = 2008,
		TOCOLOR4 = 2009,
		TOCOLOR5 = 2010,
		TRANSPARENT = 2201,
		AUTOSIZE = 2202,
		BORDERONLY = 2203,
		COMPOSITED = 2204,
		BGFILL = 2205,
		GLYPHTRANSPARENT = 2206,
		GLYPHONLY = 2207,
		ALWAYSSHOWSIZINGBAR = 2208,
		MIRRORIMAGE = 2209,
		UNIFORMSIZING = 2210,
		INTEGRALSIZING = 2211,
		SOURCEGROW = 2212,
		SOURCESHRINK = 2213,
		IMAGECOUNT = 2401,
		ALPHALEVEL = 2402,
		BORDERSIZE = 2403,
		ROUNDCORNERWIDTH = 2404,
		ROUNDCORNERHEIGHT = 2405,
		GRADIENTRATIO1 = 2406,
		GRADIENTRATIO2 = 2407,
		GRADIENTRATIO3 = 2408,
		GRADIENTRATIO4 = 2409,
		GRADIENTRATIO5 = 2410,
		PROGRESSCHUNKSIZE = 2411,
		PROGRESSSPACESIZE = 2412,
		SATURATION = 2413,
		TEXTBORDERSIZE = 2414,
		ALPHATHRESHOLD = 2415,
		WIDTH = 2416,
		HEIGHT = 2417,
		GLYPHINDEX = 2418,
		TRUESIZESTRETCHMARK = 2419,
		MINDPI1 = 2420,
		MINDPI2 = 2421,
		MINDPI3 = 2422,
		MINDPI4 = 2423,
		MINDPI5 = 2424,
		GLYPHFONT = 2601,
		IMAGEFILE = 3001,
		IMAGEFILE1 = 3002,
		IMAGEFILE2 = 3003,
		IMAGEFILE3 = 3004,
		IMAGEFILE4 = 3005,
		IMAGEFILE5 = 3006,
		STOCKIMAGEFILE = 3007,
		GLYPHIMAGEFILE = 3008,
		TEXT = 3201,
		OFFSET = 3401,
		TEXTSHADOWOFFSET = 3402,
		MINSIZE = 3403,
		MINSIZE1 = 3404,
		MINSIZE2 = 3405,
		MINSIZE3 = 3406,
		MINSIZE4 = 3407,
		MINSIZE5 = 3408,
		NORMALSIZE = 3409,
		SIZINGMARGINS = 3601,
		CONTENTMARGINS = 3602,
		CAPTIONMARGINS = 3603,
		BORDERCOLOR = 3801,
		FILLCOLOR = 3802,
		TEXTCOLOR = 3803,
		EDGELIGHTCOLOR = 3804,
		EDGEHIGHLIGHTCOLOR = 3805,
		EDGESHADOWCOLOR = 3806,
		EDGEDKSHADOWCOLOR = 3807,
		EDGEFILLCOLOR = 3808,
		TRANSPARENTCOLOR = 3809,
		GRADIENTCOLOR1 = 3810,
		GRADIENTCOLOR2 = 3811,
		GRADIENTCOLOR3 = 3812,
		GRADIENTCOLOR4 = 3813,
		GRADIENTCOLOR5 = 3814,
		SHADOWCOLOR = 3815,
		GLOWCOLOR = 3816,
		TEXTBORDERCOLOR = 3817,
		TEXTSHADOWCOLOR = 3818,
		GLYPHTEXTCOLOR = 3819,
		GLYPHTRANSPARENTCOLOR = 3820,
		FILLCOLORHINT = 3821,
		BORDERCOLORHINT = 3822,
		ACCENTCOLORHINT = 3823,
		BGTYPE = 4001,
		BORDERTYPE = 4002,
		FILLTYPE = 4003,
		SIZINGTYPE = 4004,
		HALIGN = 4005,
		CONTENTALIGNMENT = 4006,
		VALIGN = 4007,
		OFFSETTYPE = 4008,
		ICONEFFECT = 4009,
		TEXTSHADOWTYPE = 4010,
		IMAGELAYOUT = 4011,
		GLYPHTYPE = 4012,
		IMAGESELECTTYPE = 4013,
		GLYPHFONTSIZINGTYPE = 4014,
		TRUESIZESCALINGTYPE = 4015,
		USERPICTURE = 5001,
		DEFAULTPANESIZE = 5002,
		BLENDCOLOR = 5003
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public enum ThemeColors
	{
		BORDERCOLOR = 3801,		
		FILLCOLOR = 3802,       
		TEXTCOLOR = 3803,       
		EDGELIGHTCOLOR = 3804,  
		EDGEHIGHLIGHTCOLOR = 3805,  
		EDGESHADOWCOLOR = 3806,     
		EDGEDKSHADOWCOLOR = 3807,   
		EDGEFILLCOLOR = 3808,       
		TRANSPARENTCOLOR = 3809,    
		GRADIENTCOLOR1 = 3810,      
		GRADIENTCOLOR2 = 3811,      
		GRADIENTCOLOR3 = 3812,      
		GRADIENTCOLOR4 = 3813,      
		GRADIENTCOLOR5 = 3814,      
		SHADOWCOLOR = 3815,			
		GLOWCOLOR = 3816,			
		TEXTBORDERCOLOR = 3817,     
		TEXTSHADOWCOLOR = 3818,		
		GLYPHTEXTCOLOR = 3819,		
		GLYPHTRANSPARENTCOLOR = 3820,	
		FILLCOLORHINT = 3821,			
		BORDERCOLORHINT = 3822,     
		ACCENTCOLORHINT = 3823,     
	}
	
	/// <summary>
	/// Specifies the theme size type requested using <see cref="ThemedControlDrawing.GetPartSize(System.Drawing.Graphics, int, int, Syncfusion.Windows.Forms.THEMESIZE)"/>.
	/// </summary>
	public enum THEMESIZE 
	{ 
		/// <summary>
		/// Requesting the minimum size.
		/// </summary>
		TS_MIN, 
		/// <summary>
		/// Requesting the size of the theme part that will best fit the available space.
		/// </summary>
		TS_TRUE, 
		/// <summary>
		/// Requesting the size that the theme manager uses to draw a part.
		/// </summary>
		TS_DRAW 
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public class ThemedControls
	{
		public const string WINDOW = "WINDOW";
		public const string CLOCK = "CLOCK";
		public const string BUTTON = "BUTTON";
		public const string GLOBALS = "GLOBALS";
		public const string REBAR = "REBAR";
		public const string REBARPARTS = "REBARPARTS";
		public const string STATUS = "STATUS";
		public const string LISTVIEW = "LISTVIEW";
		public const string MENU = "MENU";
		public const string HEADER = "HEADER";
		public const string PROGRESS = "PROGRESS";
		public const string TAB = "TAB";
		public const string TRACKBAR = "TRACKBAR";
		public const string TRAYNOTIFYPARTS = "TRAYNOTIFYPARTS";
		public const string TOOLBAR = "TOOLBAR";
		public const string TOOLTIP = "TOOLTIP";
		public const string TREEVIEW = "TREEVIEW";
		public const string SPIN = "SPIN";
		public const string SCROLLBAR = "SCROLLBAR";
		public const string EDIT = "EDIT";
		public const string COMBOBOX = "COMBOBOX";
		public const string TASKBAR = "TASKBAR";
		public const string TASKBAND = "TASKBAND";
		public const string STARTPANEL = "STARTPANEL";
		public const string EXPLORERBAR = "EXPLORERBAR";
		public const string PAGE = "PAGE";
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public class ThemeParts
	{
		// Button Parts:
		public const int BP_PUSHBUTTON = 1;
		public const int BP_RADIOBUTTON = 2;
		public const int BP_CHECKBOX = 3;
		public const int BP_GROUPBOX = 4;
		public const int BP_USERBUTTON = 5;

		// Tab Parts:
		public const int TABP_TABITEM = 1;
		public const int TABP_TABITEMLEFTEDGE = 2;
		public const int TABP_TABITEMRIGHTEDGE = 3;
		public const int TABP_TABITEMBOTHEDGE = 4;
		public const int TABP_TOPTABITEM = 5;
		public const int TABP_TOPTABITEMLEFTEDGE = 6;
		public const int TABP_TOPTABITEMRIGHTEDGE = 7;
		public const int TABP_TOPTABITEMBOTHEDGE = 8;
		public const int TABP_PANE = 9;
		public const int TABP_BODY = 10;

		// Explorer Bar Parts:
		public const int EBP_HEADERBACKGROUND = 1;
		public const int EBP_HEADERCLOSE = 2;
		public const int EBP_HEADERPIN = 3;
		public const int EBP_IEBARMENU = 4;
		public const int EBP_NORMALGROUPBACKGROUND = 5;
		public const int EBP_NORMALGROUPCOLLAPSE = 6;
		public const int EBP_NORMALGROUPEXPAND = 7;
		public const int EBP_NORMALGROUPHEAD = 8;
		public const int EBP_SPECIALGROUPBACKGROUND = 9;
		public const int EBP_SPECIALGROUPCOLLAPSE = 10;
		public const int EBP_SPECIALGROUPEXPAND = 11;
		public const int EBP_SPECIALGROUPHEAD = 12;

		// Toolbar Parts:
		public const int TP_BUTTON = 1;
		public const int TP_DROPDOWNBUTTON = 2;
		public const int TP_SPLITBUTTON = 3;
		public const int TP_SPLITBUTTONDROPDOWN = 4;
		public const int TP_SEPARATOR = 5;
		public const int TP_SEPARATORVERT = 6;

		// Scrollbar Parts:
		public const int SBP_ARROWBTN = 1;
		public const int SBP_THUMBBTNHORZ = 2;
		public const int SBP_THUMBBTNVERT = 3;
		public const int SBP_LOWERTRACKHORZ = 4;
		public const int SBP_UPPERTRACKHORZ = 5;
		public const int SBP_LOWERTRACKVERT = 6;
		public const int SBP_UPPERTRACKVERT = 7;
		public const int SBP_GRIPPERHORZ = 8;
		public const int SBP_GRIPPERVERT = 9;
		public const int SBP_SIZEBOX = 10;

		// Window Parts:
		public const int WP_CAPTION = 1;
		public const int WP_SMALLCAPTION = 2;
		public const int WP_MINCAPTION = 3;
		public const int WP_SMALLMINCAPTION = 4;
		public const int WP_MAXCAPTION = 5;
		public const int WP_SMALLMAXCAPTION = 6;
		public const int WP_FRAMELEFT = 7;
		public const int WP_FRAMERIGHT = 8;
		public const int WP_FRAMEBOTTOM = 9;
		public const int WP_SMALLFRAMELEFT = 10;
		public const int WP_SMALLFRAMERIGHT = 11;
		public const int WP_SMALLFRAMEBOTTOM = 12;		
		public const int WP_SYSBUTTON = 13;
		public const int WP_MDISYSBUTTON = 14;
		public const int WP_MINBUTTON = 15;
		public const int WP_MDIMINBUTTON = 16;
		public const int WP_MAXBUTTON = 17;
		public const int WP_CLOSEBUTTON = 18;
		public const int WP_SMALLCLOSEBUTTON = 19;
		public const int WP_MDICLOSEBUTTON = 20;
		public const int WP_RESTOREBUTTON = 21;
		public const int WP_MDIRESTOREBUTTON = 22;
		public const int WP_HELPBUTTON = 23;
		public const int WP_MDIHELPBUTTON = 24;		
		public const int WP_HORZSCROLL = 25;
		public const int WP_HORZTHUMB = 26;
		public const int WP_VERTSCROLL = 27;
		public const int WP_VERTTHUMB = 28;		
		public const int WP_DIALOG = 29;		
		public const int WP_CAPTIONSIZINGTEMPLATE = 30;
		public const int WP_SMALLCAPTIONSIZINGTEMPLATE = 31;
		public const int WP_FRAMELEFTSIZINGTEMPLATE = 32;
		public const int WP_SMALLFRAMELEFTSIZINGTEMPLATE = 33;
		public const int WP_FRAMERIGHTSIZINGTEMPLATE = 34;
		public const int WP_SMALLFRAMERIGHTSIZINGTEMPLATE = 35;
		public const int WP_FRAMEBOTTOMSIZINGTEMPLATE = 36;
		public const int WP_SMALLFRAMEBOTTOMSIZINGTEMPLATE = 37;

		// Combo Box Parts:
		public const int CP_DROPDOWNBUTTON = 1;

		// Spin Button Parts:
		public const int SPNP_UP = 1;
		public const int SPNP_DOWN = 2;
		public const int SPNP_UPHORZ = 3;
		public const int SPNP_DOWNHORZ = 4;

		// Header Parts:
		public const int HP_HEADERITEM = 1;
		public const int HP_HEADERITEMLEFT = 2;
		public const int HP_HEADERITEMRIGHT = 3;
		public const int HP_HEADERSORTARROW = 4;

		// Rebar Parts:
		public const int RP_GRIPPER = 1;
		public const int RP_GRIPPERVERT = 2;
		public const int RP_BAND = 3;
		public const int RP_CHEVRON = 4;
		public const int RP_CHEVRONVERT = 5;

		// Edit Parts:
		public const int EP_EDITTEXT = 1;
		public const int EP_CARET = 2;

		// Status Parts:
		public const int SP_PANE = 1;
		public const int SP_GRIPPERPANE = 2;
		public const int SP_GRIPPER =3;
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public class ThemeStates
	{
		public const int CBS_UNCHECKEDNORMAL = 1;
		public const int CBS_UNCHECKEDHOT = 2;
		public const int CBS_UNCHECKEDPRESSED = 3;
		public const int CBS_UNCHECKEDDISABLED = 4;
		public const int CBS_CHECKEDNORMAL = 5;
		public const int CBS_CHECKEDHOT = 6;
		public const int CBS_CHECKEDPRESSED = 7;
		public const int CBS_CHECKEDDISABLED = 8;
		public const int CBS_MIXEDNORMAL = 9;
		public const int CBS_MIXEDHOT = 10;
		public const int CBS_MIXEDPRESSED = 11;
		public const int CBS_MIXEDDISABLED = 12;

		public const int PBS_NORMAL = 1; 
		public const int PBS_HOT = 2; 
		public const int PBS_PRESSED = 3;
		public const int PBS_DISABLED = 4;
		public const int PBS_DEFAULTED = 5;

		public const int RBS_UNCHECKEDNORMAL = 1;
		public const int RBS_UNCHECKEDHOT = 2;
		public const int RBS_UNCHECKEDPRESSED = 3;
		public const int RBS_UNCHECKEDDISABLED = 4;
		public const int RBS_CHECKEDNORMAL = 5;
		public const int RBS_CHECKEDHOT = 6;
		public const int RBS_CHECKEDPRESSED = 7;
		public const int RBS_CHECKEDDISABLED = 8;

		public const int TIS_NORMAL = 1;
		public const int TIS_HOT = 2;
		public const int TIS_SELECTED = 3;
		public const int TIS_DISABLED = 4;
		public const int TIS_FOCUSED = 5;

		public const int TILES_NORMAL=1;
		public const int TILES_HOT = 2;
		public const int TILES_SELECTED = 3;
		public const int TILES_DISABLED = 4;
		public const int TILES_FOCUSED = 5;

		public const int TIRES_NORMAL = 1;
		public const int TIRES_HOT = 2;
		public const int TIRES_SELECTED = 3;
		public const int TIRES_DISABLED = 4;
		public const int TIRES_FOCUSED = 5;

		public const int TIBES_NORMAL = 1;
		public const int TIBES_HOT = 2;
		public const int TIBES_SELECTED = 3;
		public const int TIBES_DISABLED = 4;
		public const int TIBES_FOCUSED = 5;

		public const int TTIS_NORMAL = 1;
		public const int TTIS_HOT = 2;
		public const int TTIS_SELECTED = 3;
		public const int TTIS_DISABLED = 4;
		public const int TTIS_FOCUSED = 5;

		public const int TTILES_NORMAL = 1;
		public const int TTILES_HOT = 2;
		public const int TTILES_SELECTED = 3;
		public const int TTILES_DISABLED = 4;
		public const int TTILES_FOCUSED = 5;

		public const int TTIRES_NORMAL = 1;
		public const int TTIRES_HOT = 2;
		public const int TTIRES_SELECTED = 3;
		public const int TTIRES_DISABLED = 4;
		public const int TTIRES_FOCUSED = 5;

		public const int TTIBES_NORMAL = 1;
		public const int TTIBES_HOT = 2;
		public const int TTIBES_SELECTED = 3;
		public const int TTIBES_DISABLED = 4;
		public const int TTIBES_FOCUSED = 5;

		// Explorer Bar States:
		public const int EBHC_NORMAL = 1; 
		public const int EBHC_HOT = 2;
		public const int EBHC_PRESSED = 3;
		public const int EBHP_NORMAL = 1;
		public const int EBHP_HOT = 2;
		public const int EBHP_PRESSED = 3;
		public const int EBHP_SELECTEDNORMAL = 4;
		public const int EBHP_SELECTEDHOT = 5;
		public const int EBHP_SELECTEDPRESSED = 6;
		public const int EBM_NORMAL = 1;
		public const int EBM_HOT = 2;
		public const int EBM_PRESSED = 3;
		public const int EBNGC_NORMAL = 1; 
		public const int EBNGC_HOT = 2;
		public const int EBNGC_PRESSED = 3;
		public const int EBNGE_HOT = 1;
		public const int EBNGE_NORMAL = 2; 
		public const int EBNGE_PRESSED = 3;
		public const int EBSGC_NORMAL = 1;
		public const int EBSGC_HOT = 2;
		public const int EBSGC_PRESSED = 3;
		public const int EBSGE_NORMAL = 1;
		public const int EBSGE_HOT = 2;
		public const int EBSGE_PRESSED = 3;

		// Toolbar States:
		public const int TS_NORMAL = 1;
		public const int TS_HOT = 2;
		public const int TS_PRESSED = 3;
		public const int TS_DISABLED = 4;
		public const int TS_CHECKED = 5; 
		public const int TS_HOTCHECKED = 6;

		// Scrollbar States:
		public const int ABS_UPNORMAL = 1; 
		public const int ABS_UPHOT = 2;
		public const int ABS_UPPRESSED = 3; 
		public const int ABS_UPDISABLED = 4;
		public const int ABS_DOWNNORMAL = 5;
		public const int ABS_DOWNHOT = 6;
		public const int ABS_DOWNPRESSED = 7; 
		public const int ABS_DOWNDISABLED = 8;
		public const int ABS_LEFTNORMAL = 9;
		public const int ABS_LEFTHOT = 10;
		public const int ABS_LEFTPRESSED = 11;
		public const int ABS_LEFTDISABLED = 12;
		public const int ABS_RIGHTNORMAL = 13;
		public const int ABS_RIGHTHOT = 14;
		public const int ABS_RIGHTPRESSED = 15;
		public const int ABS_RIGHTDISABLED = 16;

		public const int SCRBS_NORMAL = 1;
		public const int SCRBS_HOT = 2;
		public const int SCRBS_PRESSED = 3;
		public const int SCRBS_DISABLED = 4;

		public const int SZB_RIGHTALIGN = 1;
		public const int SZB_LEFTALIGN = 2;

		// Window States:
		public const int FS_ACTIVE = 1;
		public const int FS_INACTIVE = 2;
		public const int CS_ACTIVE = 1;
		public const int CS_INACTIVE = 2;
		public const int CS_DISABLED = 3;
		public const int MXCS_ACTIVE = 1;
		public const int MXCS_INACTIVE = 2;
		public const int MXCS_DISABLED = 3;
		public const int MNCS_ACTIVE = 1;
		public const int MNCS_INACTIVE = 2;
		public const int MNCS_DISABLED = 3;
		public const int HSS_NORMAL = 1;
		public const int HSS_HOT = 2;
		public const int HSS_PUSHED = 3;
		public const int HSS_DISABLED = 4;
		public const int HTS_NORMAL = 1;
		public const int HTS_HOT = 2;
		public const int HTS_PUSHED = 3;
		public const int HTS_DISABLED = 4;
		public const int VSS_NORMAL = 1;
		public const int VSS_HOT = 2;
		public const int VSS_PUSHED = 3;
		public const int VSS_DISABLED = 4;
		public const int VTS_NORMAL = 1;
		public const int VTS_HOT = 2;
		public const int VTS_PUSHED = 3;
		public const int VTS_DISABLED = 4;
		public const int SBS_NORMAL = 1;
		public const int SBS_HOT = 2;
		public const int SBS_PUSHED = 3;
		public const int SBS_DISABLED = 4;
		public const int MINBS_NORMAL = 1;
		public const int MINBS_HOT = 2;
		public const int MINBS_PUSHED = 3;
		public const int MINBS_DISABLED = 4;
		public const int MAXBS_NORMAL = 1;
		public const int MAXBS_HOT = 2;
		public const int MAXBS_PUSHED = 3;
		public const int MAXBS_DISABLED = 4;
		public const int RBS_NORMAL = 1;
		public const int RBS_HOT = 2;
		public const int RBS_PUSHED = 3;
		public const int RBS_DISABLED = 4;
		public const int HBS_NORMAL = 1;
		public const int HBS_HOT = 2;
		public const int HBS_PUSHED = 3;
		public const int HBS_DISABLED = 4;
		public const int CBS_NORMAL = 1;
		public const int CBS_HOT = 2;
		public const int CBS_PUSHED = 3;
		public const int CBS_DISABLED = 4;		

		// Combo Box states:
		public const int CBXS_NORMAL = 1;
		public const int CBXS_HOT = 2;
		public const int CBXS_PRESSED = 3;
		public const int CBXS_DISABLED = 4;

		// Spin Button States:
		public const int DNS_NORMAL = 1;
		public const int DNS_HOT = 2;
		public const int DNS_PRESSED = 3;
		public const int DNS_DISABLED = 4;
		public const int DNHZS_NORMAL = 1;
		public const int DNHZS_HOT = 2;
		public const int DNHZS_PRESSED = 3;
		public const int DNHZS_DISABLED = 4;
		public const int UPS_NORMAL = 1;
		public const int UPS_HOT = 2;
		public const int UPS_PRESSED = 3;
		public const int UPS_DISABLED = 4;
		public const int UPHZS_NORMAL = 1;
		public const int UPHZS_HOT = 2;
		public const int UPHZS_PRESSED = 3;
		public const int UPHZS_DISABLED = 4;

		// Header States:
		public const int HIS_NORMAL = 1;
		public const int HIS_HOT = 2;
		public const int HIS_PRESSED = 3;

		// Rebar States:
		public const int CHEVS_NORMAL = 1;
		public const int CHEVS_HOT = 2;
		public const int CHEVS_PRESSED = 3;

		// Edit Box States:
		public const int ETS_NORMAL = 1;
		public const int ETS_HOT = 2;
		public const int ETS_SELECTED = 3;
		public const int ETS_DISABLED = 4;
		public const int ETS_FOCUSED = 5;
		public const int ETS_READONLY = 6;
		public const int ETS_ASSIST = 7;
	}
}