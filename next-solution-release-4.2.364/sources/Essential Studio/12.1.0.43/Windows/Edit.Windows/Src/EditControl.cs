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

using System.ComponentModel;
using System.Drawing;
using System;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Collections;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Text;
using System.Xml;
using System.Security.Permissions;
using System.Drawing.Design;
using System.IO.IsolatedStorage;
using System.Reflection;

using Syncfusion.Windows.Forms.Localization;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Edit.Utils;
using Syncfusion.Windows.Forms.Edit.Implementation.Config;
using Syncfusion.Windows.Forms.Edit.Implementation;
using Syncfusion.Windows.Forms.Edit.Implementation.Parser;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Shared.Utils.KeyBinding;
using Syncfusion.Shared.Utils.KeyBinding.Implementation;
using Syncfusion.IO;
using Syncfusion.Windows.Forms.Edit.Utils.AutoFormatting;
using Syncfusion.Windows.Forms.Edit.Implementation.IO;
using Syncfusion.Windows.Forms.Edit.Utils.CodeSnippets;
using Syncfusion.Windows.Forms.Edit.Forms.Popup;
using Syncfusion.Windows.Forms.Edit.Design;
using Syncfusion.Windows.Forms.Edit.Dialogs;
using Syncfusion.Windows.Forms.Edit.Dialogs.Options;
using Syncfusion.Windows.Forms.Collections;
using System.ComponentModel.Design;

namespace Syncfusion.Windows.Forms.Edit
{
	/// <summary>
	/// Summary description for EditControl.
	/// </summary>
	[Designer( typeof( EditControlDesigner ) )]
	[ToolboxItem( true )]
	[ToolboxBitmap( typeof( EditControl ), "ToolboxIcons.Edit.bmp" )]
	public class EditControl
		: BaseLocalizableControl
		, ISupportInitialize
	{
		#region Classes
        /// <summary>
        /// It's an Event Handler for RegexCaseInsenstiveArgs
        /// </summary>
        public event EventHandler<RegexCaseInsenstiveArgs> SearchRegex;

        /// <summary>
        /// This class describes string that will be ignore the case when user using FindRegex() programatically.
        /// </summary>
        public class RegexCaseInsenstiveArgs:EventArgs
        {
            /// <summary>
            /// It indicates caseInsenstive is on or off.
            /// </summary>
            private bool m_biscaseInsensitive = false;

            /// <summary>
            /// Initializes an object of RegexCaseInsenstiveArgs Class
            /// </summary>
            public RegexCaseInsenstiveArgs()
            {

            }

            /// <summary>
            /// Gets or sets whether caseInsenstive is on.
            /// </summary>
            public bool IsCaseInsensitiveOn
            {
                set
                {
                    if (m_biscaseInsensitive != value)
                    {
                        m_biscaseInsensitive = value;
                    }
                }

                get
                {
                    return m_biscaseInsensitive;
                }
            }

        }

		/// <summary>
		/// Attribute for properties that are used in context choice options.
		/// </summary>
		[AttributeUsage( AttributeTargets.Property, AllowMultiple = false )]
		private class ContextOptionsAttribute
			: Attribute
		{
		}
		/// <summary>
		/// Fix for bugs related to scrolling subclassing.
		/// </summary>
		[ToolboxItem( false )]
		private class EditorPanel : Panel
		{
			/// <summary>
			/// 
			/// </summary>
			/// <param name="keyData"></param>
			/// <returns></returns>
			protected override bool IsInputKey( Keys keyData )
			{
				bool result = base.IsInputKey( keyData );
				return true;
			}
		}
		/// <summary>
		/// ScrollersFrame for EditControl.
		/// </summary>
		[DesignTimeVisible( false )]
		internal class EditScrollersFrame
			: ScrollersFrame
		{
			#region Initialization
			/// <summary>
			/// Creates new EditScrollersFrame.
			/// </summary>
			public EditScrollersFrame()
			{
				this.RefreshOnValueChange = true;
			}
			#endregion

			#region Overrides
			/// <summary>
			/// Updates visibility of gripper.
			/// </summary>
			protected override void UpdateGripperVisibility()
			{
				m_sizeGripper.Enabled = m_sizeGripper.Visible = true;
				m_sizeGripper.DrawGripMarking = GetGripperVisibility();
			}
			/// <summary>
			/// 
			/// </summary>
			protected override void UpdateParentInDragging()
			{
				hScroller.Enabled = hScroller.Visible = this.IsHorizontalScrollVisible;
				vScroller.Enabled = vScroller.Visible = this.IsVerticalScrollVisible;
				UpdateGripperVisibility();
			}
			#endregion
		}
		#endregion

		#region Constans
		/// <summary>
		/// Used in RedrawWindow API.
		/// </summary>
		private const int RDW_FRAME = 0x0400;
		/// <summary>
		/// Used in RedrawWindow API.
		/// </summary>
		private const int RDW_UPDATENOW = 0x0100;
		/// <summary>
		/// Used in RedrawWindow API.
		/// </summary>
		private const int RDW_INVALIDATE = 0x0001;
		/// <summary>
		/// Distance of splitters center docking.
		/// </summary>
		private const int DEF_SPLITTERS_DOCK_DISTANCE = 20;
		/// <summary>
		/// Distance between the text and right/bottom borders in autosize mode.
		/// </summary>
		private const int DEF_MIN_BORDER_DIST_AUTOSIZE = 5;
		#endregion

		#region Controls
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private Syncfusion.Windows.Forms.Tools.Controls.StatusBar.StatusBarExt statusBar;
		internal Syncfusion.Windows.Forms.Edit.FileEditControl edtCode;
		private Syncfusion.Windows.Forms.Edit.FakeEditControl fakeEditControl1;
		private EditorPanel pnlCommon;
		private EditorPanel pnlClient;
		private EditorPanel pnlTop;
		private System.Windows.Forms.Splitter splitterCenter;
		private EditorPanel pnlClientClient;
		private EditorPanel pnlClientRight;
		private EditorPanel pnlTopClient;
		private EditorPanel pnlTopRight;
		private System.Windows.Forms.Splitter splitterBottom;
		private System.Windows.Forms.Splitter splitterTop;
		private Syncfusion.Windows.Forms.Edit.FakeEditControl fakeEditControl2;
		private Syncfusion.Windows.Forms.Edit.FakeEditControl fakeEditControl3;
		#endregion

		#region Fields
		/// <summary>
		/// Specifies text parsing mode.
		/// </summary>
		private TextParsingMode m_parsingMode = TextParsingMode.FullParsing;
		/// <summary>
		/// Border style.
		/// </summary>
		private BorderStyle borderStyle = BorderStyle.None;
		/// <summary>
		/// Timer, that is used for remeasuring text in word-wrap mode.
		/// </summary>
		private Timer m_timerUpdateWordWrap;
		/// <summary>
		/// Control, the editor should be exchanged with on timer tick.
		/// </summary>
		private FakeEditControl m_controlExchangeWith;
		/// <summary>
		/// Height of the control, that was set before single line mode was enabled.
		/// </summary>
		private int m_iHeightBeforeSingleLine;
		/// <summary>
		/// Counter for event handling lock.
		/// </summary>
		private int m_iLockEvents;
		/// <summary>
		/// Specifies whether file name should be printed.
		/// </summary>
		private bool m_bPrintFileName;
		/// <summary>
		/// Specifies whether page number should be printed.
		/// </summary>
		private bool m_bPrintPageNumber;
		/// <summary>
		/// Brush used to draw vertical splitter when XP style is used but there's no XP themes available.
		/// </summary>
		private BrushInfo m_brushVertSplitter = new BrushInfo( GradientStyle.Horizontal, new Color[] { Color.White, Color.Gray } );
		/// <summary>
		/// Brush used to draw horizontal splitter when XP style is used but there's no XP themes available.
		/// </summary>
		private BrushInfo m_brushHorSplitter = new BrushInfo( GradientStyle.Vertical, new Color[] { Color.White, Color.Gray } );
		/// <summary>
		/// Settings of status bar.
		/// </summary>
		private StatusBarSettings m_statusBarSettings = null;
		/// <summary>
		/// Size of the control.
		/// </summary>
		private Size m_sizeOld = Size.Empty;
		/// <summary>
		/// Number of paint locks.
		/// </summary>
		private int m_iPaintLocks;
		/// <summary>
		/// Specifies state of the autosizing feature.
		/// </summary>
		private bool m_bAutoSize;
		/// <summary>
		/// Specifies minimum size in autosize mode.
		/// </summary>
		private Size m_minSize;
		/// <summary>
		/// Indicates whether static data should be cleared on dispose. Frees memory, but impairs time of next loading of EditControl.
		/// </summary>
		private bool m_bClearStaticDataOnDispose = true;
		/// <summary>
		/// Custom cursor image.
		/// </summary>
		private Bitmap m_imgCursor;
		/// <summary>
		/// ScrollersFrame for ScrollEditControl.
		/// </summary>
		internal EditScrollersFrame m_scrollersFrame;
		/// <summary>
		/// ScrollersFrame for first fake editor.
		/// </summary>
		internal EditScrollersFrame m_scrollersFrameForFake1;
		/// <summary>
		/// ScrollersFrame for second fake editor.
		/// </summary>
		internal EditScrollersFrame m_scrollersFrameForFake2;
		/// <summary>
		/// ScrollersFrame for third fake editor.
		/// </summary>
		internal EditScrollersFrame m_scrollersFrameForFake3;
		/// <summary>
		/// Specifies whether horizontal splitters enabled.
		/// </summary>
		private bool m_bOldHorizontalSplitters = false;
		/// <summary>
		/// Specifies whether vertical splitters enabled.
		/// </summary>
		private bool m_bOldVerticalSplitters = false;
		/// <summary>
		/// Specifies whether commands are already registered.
		/// </summary>
		private bool m_bCommandsRegistered = false;
		/// <summary>
		/// Specifies whether Escape key should be used.
		/// </summary>
		private bool m_bAcceptsEscape = true;
        /// <summary>
        /// Specifies the scroll bar visual style.
        /// </summary>
        private ScrollBarCustomDrawStyles m_scrollVisualStyle = ScrollBarCustomDrawStyles.WindowsXP;
        /// <summary>
        /// Specifies the Office2007 scroll bar color scheme.
        /// </summary>
        private Office2007ColorScheme m_scrollColorScheme = Office2007ColorScheme.Blue;
		#endregion

		#region Properties
        /// <summary>
        ///  Gets or sets the search the text like in visual studion editor 
        /// </summary>
        /// [Browsable( true )]
        [Category("Search Type")]
        [DefaultValue(false)]
        [Description("Specifies whether searching the text like in visual studio editor.")]
         public bool LikeVisualStudioSearch
        {
            get
            {
                return edtCode.FileEditLikeVisualStduioSearch;
            }
            set
            {
                if (edtCode.FileEditLikeVisualStduioSearch != value)
                {
                    edtCode.FileEditLikeVisualStduioSearch = value;
                }
            }
        }

		/// <summary>
		/// Gets or sets value that indicates whether autoresizing of the control is turned on.
		/// </summary>
		[Browsable( true )]
		[Category( "Behavior" )]
		[DefaultValue( false )]
		[Description( "Specifies whether autoresizing of the control is turned on." )]
		public new bool AutoSize
		{
			get
			{
				return m_bAutoSize;
			}
			set
			{
				if( value != m_bAutoSize )
				{
					m_bAutoSize = value;
					ShowHorizontalScroller = !value;
					ShowVerticalScroller = !value;
					ShowHorizontalSplitters = !value;
					ShowVerticalSplitters = !value;
					edtCode.DisableScrollers = value;

					UpdateSizeInAutoSizeMode();
				}
			}
		}
		/// <summary>
		/// Gets or sets minimum size in autosize mode.
		/// </summary>
		[Browsable( true )]
		[Category( "Behavior" )]
		[DefaultValue( typeof( Size ), "0, 0" )]
		[Description( "Specifies minimum size in autosize mode. If AutoSize mode is off, the value of this property is ignored." )]
		public Size MinSize
		{
			get
			{
				return m_minSize;
			}
			set
			{
				if( value != m_minSize )
				{
					m_minSize = value;
					UpdateSizeInAutoSizeMode();
				}
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether all files can be dropped to EditControl.
		/// If set to false, only files with extension contained in FileExtensions can be dropped.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( false )]
		[Category( "Behavior" )]
		[Description( "If true any file can be dropped in the EditControl. If false only files with certain extension can be dropped." )]
		public bool DropAllFiles
		{
			get
			{
				return edtCode.DropAllFiles;
			}
			set
			{
				edtCode.DropAllFiles = value;
			}
		}
		/// <summary>
		/// Gets or sets extensions of files that can be dropped to EditControl.
		/// </summary>
        [Browsable( true )]
        [DefaultValue(false)]
        [Category("Behavior")]
        [Description("If true saved lines will be flushed. If false saved lines will not be flushed.")]
        public bool FlushSavedLines
        {
            get
            {
                return edtCode.FlushSavedLines;
            }
            set
            {
                edtCode.FlushSavedLines = value;
            }
        }
        /// <summary>
        /// Gets or sets whether saved line can be flushed.
        /// </summary>
        [Browsable( true )]
        [DefaultValue(false)]
        [Category("Behavior")]
        [Description("If true files will be autosaved in EditControl. If false files will not be saved automatically.")]
        public bool AutoSave
        {
            get
            {
                return edtCode.AutoSave;
            }
            set
            {
                edtCode.AutoSave = value;
            }
        }
        /// <summary>
        ///  Gets or sets whether autosave can be done in EditControl.
        /// </summary>
		[Browsable( true )]
		[Category( "Behavior" )]
		[Description( "Extensions of files that can be dropped to EditControl." )]
		public string[] FileExtensions
		{
			get
			{
				return edtCode.FileExtensions;
			}
			set
			{
				edtCode.FileExtensions = value;
			}
		}
		/// <summary>
		/// Gets lexical macros manager.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public MacrosManager LexicalMacrosManager
		{
			get
			{
				return this.Configurator.MacrosManager;
			}
		}
		/// <summary>
		/// Gets or sets text parsing mode. User can select between high parsing speed or high syntax highlighting accuracy.
		/// </summary>
		[Browsable( true )]
		[Category( "Behavior" )]
		[DefaultValue( TextParsingMode.FullParsing )]
		[Description( "Specifies text parsing mode used by control. Parsing can be set to be accurate and slow or non-accurate and fast." )]
		public TextParsingMode ParsingMode
		{
			get
			{
				return m_parsingMode;
			}
			set
			{
				if( m_parsingMode != value )
				{
					m_parsingMode = value;
					OnParsingModeChanged();
				}
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether vertical scroller can be shown.
		/// </summary>
		[Browsable( true )]
		[Category( "Appearance" )]
		[DefaultValue( true )]
		[Description( "Specifies whether vertical scroller can be shown." )]
		[ContextOptions]
		public bool ShowVerticalScroller
		{
			get
			{
				return !edtCode.DisableVerticalScroller;
			}
			set
			{
				edtCode.DisableVerticalScroller = !value;
				fakeEditControl1.DisableVerticalScroller = !value;
				fakeEditControl2.DisableVerticalScroller = !value;
				fakeEditControl3.DisableVerticalScroller = !value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether horizontal scroller can be shown.
		/// </summary>
		[Browsable( true )]
		[Category( "Appearance" )]
		[DefaultValue( true )]
		[Description( "Specifies whether horizontal scroller can be shown." )]
		[ContextOptions]
		public bool ShowHorizontalScroller
		{
			get
			{
				return !edtCode.DisableHorizontalScroller;
			}
			set
			{
				edtCode.DisableHorizontalScroller = !value;
				fakeEditControl1.DisableHorizontalScroller = !value;
				fakeEditControl2.DisableHorizontalScroller = !value;
				fakeEditControl3.DisableHorizontalScroller = !value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether page header and footer should be printed.
		/// </summary>
		[Browsable( true )]
		[Category( "Appearance" )]
		[DefaultValue( true )]
		[Description( "Specifies whether page header and footer should be printed." )]
		public bool PageHeaderAndFooterVisible
		{
			get
			{
				return edtCode.PrintHeaderAndFooter;
			}
			set
			{
				edtCode.PrintHeaderAndFooter = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether document name should be printed.
		/// </summary>
		[Browsable( true )]
		[Category( "Appearance" )]
		[DefaultValue( false )]
		[Description( "Specifies whether document name should be printed when page header is visible." )]
		public bool PrintDocumentName
		{
			get
			{
				return m_bPrintFileName;
			}
			set
			{
				m_bPrintFileName = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether page number should be printed.
		/// </summary>
		[Browsable( true )]
		[Category( "Appearance" )]
		[DefaultValue( false )]
		[Description( "Specifies whether page number should be printed when page footer is visible." )]
		public bool PrintPageNumber
		{
			get
			{
				return m_bPrintPageNumber;
			}
			set
			{
				m_bPrintPageNumber = value;
			}
		}
		/// <summary>
		/// Gets or sets value indication whether virtual space mode is enabled.
		/// </summary>
		[Browsable( true )]
		[Category( "Behavior" )]
		[DefaultValue( false )]
		[Description( "Specifies whether virtual space mode is enabled." )]
		[ContextOptions]
		public bool VirtualSpaceMode
		{
			get
			{
				return edtCode.VirtualSpaceMode;
			}
			set
			{
				edtCode.VirtualSpaceMode = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether save prompt dialog should be displayed before control is closed.
		/// </summary>
		[Browsable( true )]
		[Category( "Behavior" )]
		[DefaultValue( true )]
		[Description( "Specifies whether save prompt dialog should be displayed before control is closed." )]
		public bool SaveOnClose
		{
			get
			{
				return edtCode.SaveOnClose;
			}
			set
			{
				edtCode.SaveOnClose = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether control allows dropping of objects.
		/// </summary>
		[Category( "Behavior" )]
		[DefaultValue( false )]
		[Description( "Specifies whether drag and drop operations are allowed for control." )]
		public new bool AllowDrop
		{
			get
			{
				return base.AllowDrop;
			}
			set
			{
				if( value != base.AllowDrop )
				{
					base.AllowDrop = edtCode.AllowDrop = fakeEditControl1.AllowDrop = fakeEditControl2.AllowDrop = fakeEditControl3.AllowDrop = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets brush for filling EditControl's background.
		/// </summary>
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )]
		[Category( "Appearance" )]
		[Description( "Specifies background fill style and color." )]
		[DefaultValue( typeof( BrushInfo ), "None" )]
		public BrushInfo BackgroundColor
		{
			get
			{
				return edtCode.BackgroundColor;
			}

			set
			{
				edtCode.BackgroundColor = value;
			}
		}
		/// <summary>
		/// Gets or sets color of the indent line.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Specifes color of the block indentation line." )]
		[DefaultValue( typeof( Color ), "Gray" )]
		public Color IndentLineColor
		{
			get
			{
				return edtCode.IndentLineColor;
			}
			set
			{
				edtCode.IndentLineColor = value;
			}
		}
        /// <summary>
        /// Gets or sets color of the text under selection.
        /// </summary>
        [Category("Appearance")]
        [Description("Specifies color of the text under selection.")]
        [DefaultValue(typeof(Color), "Blue")]
        public Color SelectionTextColor
        {
            get
            {
                return edtCode.SelectionTextColor;
            }
            set
            {
                edtCode.SelectionTextColor = value;
            }
        }
		/// <summary>
		/// Gets or sets color of the indent block start and end.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Specifies color used for highlighting indented block start and end." )]
		[DefaultValue( typeof( Color ), "LightBlue" )]
		public Color IndentBlockHighlightingColor
		{
			get
			{
				return edtCode.IndentBlockHighlightingColor;
			}
			set
			{
				edtCode.IndentBlockHighlightingColor = value;
			}
		}
		/// <summary>
		/// Gets or sets value that specifies whether indent guideline should be shown automatically after cursor repositioning.
		/// </summary>
		[Category( "Behavior" )]
		[DefaultValue( true )]
		[Description( "Specifies whether indent guideline should be automatically shown." )]
		public bool AutoIndentGuideline
		{
			get
			{
				return edtCode.AutoIndentGuideline;
			}
			set
			{
				edtCode.AutoIndentGuideline = value;
			}
		}
		/// <summary>
		/// Gets or sets value that specifies whether only start and end of the block should be highlighted or guideline should be drawn either.
		/// </summary>
		[Category( "Behavior" )]
		[DefaultValue( false )]
		[Description( "Specifies whether only start and end of the block should be highlighted or guideline should be drawn either." )]
		public bool OnlyHighlightMatchingBraces
		{
			get
			{
				return edtCode.OnlyHighlightMatchingBraces;
			}
			set
			{
				edtCode.OnlyHighlightMatchingBraces = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether indentation guidelines should be shown.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Specifies whether indentation guidelines and block start/end highlighting should be drawn." )]
		[DefaultValue( true )]
		[ContextOptions]
		public bool ShowIndentationGuidelines
		{
			get
			{
				return edtCode.ShowIndentationGuidelines;
			}
			set
			{
				edtCode.ShowIndentationGuidelines = value;
			}
		}
		/// <summary>
		/// Gets or sets context menu manager.
		/// </summary>
		[Browsable( false )]
		public ContextMenuManager ContextMenuManager
		{
			get
			{
				return edtCode.ContextMenuManager;
			}
			set
			{
				edtCode.ContextMenuManager = value;
			}
		}
		/// <summary>
		/// Gets or sets current column.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public int CurrentColumn
		{
			get
			{
				return edtCode.CurrentColumn;
			}
			set
			{
				edtCode.CurrentColumn = value;
			}
		}
		/// <summary>
		/// Gets or sets current virtual column. Virtual column is visual position of character on the screen.
		/// </summary>
		[Browsable( false )]
		public int VisualColumn
		{
			get
			{
				return edtCode.VisualColumn;
			}
			set
			{
				edtCode.VisualColumn = value;
			}
		}
		/// <summary>
		/// Get or set current line.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public int CurrentLine
		{
			get
			{
				return edtCode.CurrentLine;
			}
			set
			{
				edtCode.CurrentLine = value;
			}
		}
		/// <summary>
		/// Gets or sets current position of the cursor in virtual coordinates.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public Point CurrentPosition
		{
			get
			{
				return edtCode.CurrentPosition;
			}
			set
			{
				edtCode.CurrentPosition = value;
			}
		}
		/// <summary>
		/// Gets or sets insert mode state.
		/// </summary>
		[Category( "Behavior" )]
		[Description( "Specifes insert mode state." )]
		[DefaultValue( true )]
		[ContextOptions]
		public bool InsertMode
		{
			get
			{
				return edtCode.InsertMode;
			}
			set
			{
				edtCode.InsertMode = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether line numbers should be shown.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Specifies whether line numbers should be shown." )]
		[DefaultValue( true )]
		[ContextOptions]
		public bool ShowLineNumbers
		{
			get
			{
				return edtCode.ShowLineNumbers;
			}
			set
			{
				edtCode.ShowLineNumbers = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether bookmarks and indicator margin should be visible.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Specifies whether bookmarks and indicator margin should be visible." )]
		[DefaultValue( true )]
		[ContextOptions]
		public bool ShowIndicatorMargin
		{
			get
			{
				return edtCode.ShowMarkers;
			}
			set
			{
				edtCode.ShowMarkers = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether markers should be visible.
		/// </summary>
		[Browsable( false )]
		[EditorBrowsable( EditorBrowsableState.Never )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public bool ShowMarkers
		{
			get
			{
				return edtCode.ShowMarkers;
			}
			set
			{
				edtCode.ShowMarkers = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether outlining collapsers should be shown.
		/// </summary>
		[Browsable( false )]
		[EditorBrowsable( EditorBrowsableState.Never )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public bool ShowCollapse
		{
			get
			{
				return edtCode.ShowCollapse;
			}
			set
			{
				edtCode.ShowCollapse = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether outlining collapsers should be shown.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Specifies whether outlining collapsers should be visible." )]
		[DefaultValue( true )]
		[ContextOptions]
		public bool ShowOutliningCollapsers
		{
			get
			{
				return edtCode.ShowCollapse;
			}
			set
			{
				edtCode.ShowCollapse = value;
			}
		}
        /// <summary>
        /// Gets or Sets a value indicating whether to stop search at the page end.
        /// </summary>
        [Category("Behavior")]
        [Description("Specifies  whether to stop search at the page end.")]
        [DefaultValue(true)]
        [ContextOptions]
        public bool WrapAroundSearch
        {
            get
            {
                return edtCode.WrapAroundSearch;
            }
            set
            {
                edtCode.WrapAroundSearch = value;
            }
        }
		/// <summary>
		/// Gets or sets visibility of the selection margin.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Specifies whether selection margin area should be visible." )]
		[DefaultValue( true )]
		[ContextOptions]
		public bool ShowSelectionMargin
		{
			get
			{
				return edtCode.ShowSelectionMargin;
			}
			set
			{
				edtCode.ShowSelectionMargin = value;
			}
		}
		/// <summary>
		/// Gets or sets background color of the selection margin.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Specifies selection margin area color." )]
		[DefaultValue( typeof( Color ), "" )]
		public Color SelectionMarginBackgroundColor
		{
			get
			{
				return edtCode.SelectionMarginBackgroundColor;
			}
			set
			{
				edtCode.SelectionMarginBackgroundColor = value;
			}
		}
		/// <summary>
		/// Gets or sets foreground color of the selection margin.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Specifies selection margin color." )]
		[DefaultValue( typeof( Color ), "Red" )]
		public Color SelectionMarginForegroundColor
		{
			get
			{
				return edtCode.SelectionMarginForegroundColor;
			}
			set
			{
				edtCode.SelectionMarginForegroundColor = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether whitespaces should be shown as special symbols.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Specifies whether whitespaces should be shown as special symbols." )]
		[DefaultValue( false )]
		[ContextOptions]
		public bool ShowWhitespaces
		{
			get
			{
				return edtCode.ShowWhitespaces;
			}
			set
			{
				edtCode.ShowWhitespaces = value;
			}
		}
		/// <summary>
		/// Gets count of visible lines on the screen.
		/// </summary>
		[Browsable( false )]
		public int VisibleLineCount
		{
			get
			{
				return edtCode.VisibleLineCount;
			}
		}
        /// <summary>
        /// Gets or Sets the virtual line number start value.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Specifies start value of virtual line number.")]
        [DefaultValue(typeof(int), "0")]
        public int VirtualLineNumberOffset
        {
            get
            {
                return edtCode.VirtualLineNumberOffset;
            }
            set
            {
                edtCode.VirtualLineNumberOffset = value;
            }
        }
		/// <summary>
		/// Get count of lines in file.
		/// </summary>
		[Browsable( false )]
		public int PhysicalLineCount
		{
			get
			{
				return edtCode.PhysicalLineCount;
			}
		}
		/// <summary>
		/// Gets or sets language configurator.
		/// </summary>
		[Browsable( false )]
		public Config Configurator
		{
			get
			{
				return edtCode.Configurator;
			}
			set
			{
				edtCode.Configurator = value;
			}
		}
		/// <summary>
		/// Gets parser for internal usage.
		/// </summary>
		[Browsable( false )]
		public RenderableLexemParser Parser
		{
			get
			{
				return edtCode.Parser;
			}
		}
		/// <summary>
		/// Gets or sets composite quality.
		/// </summary>
		[Browsable( true )]
		[Category( "Render Quality" )]
		[Description( "Specifies image composition qaulity." )]
		[DefaultValue( System.Drawing.Drawing2D.CompositingQuality.Default )]
		public CompositingQuality GraphicsCompositingQuality
		{
			get
			{
				return edtCode.GraphicsCompositingQuality;
			}
			set
			{
				edtCode.GraphicsCompositingQuality = value;
			}
		}
		/// <summary>
		/// Gets or sets interpolation mode.
		/// </summary>
		[Browsable( true )]
		[Category( "Render Quality" )]
		[Description( "Specifies interpolation mode." )]
		[DefaultValue( System.Drawing.Drawing2D.InterpolationMode.Default )]
		public InterpolationMode GraphicsInterpolationMode
		{
			get
			{
				return edtCode.GraphicsInterpolationMode;
			}
			set
			{
				edtCode.GraphicsInterpolationMode = value;
			}
		}
		/// <summary>
		/// Get or sets smoothing mode.
		/// </summary>
		[Browsable( true )]
		[Category( "Render Quality" )]
		[Description( "Specifies smoothing mode." )]
		[DefaultValue( System.Drawing.Drawing2D.SmoothingMode.Default )]
		public SmoothingMode GraphicsSmoothingMode
		{
			get
			{
				return edtCode.GraphicsSmoothingMode;
			}
			set
			{
				edtCode.GraphicsSmoothingMode = value;
			}
		}
		/// <summary>
		/// Get or sets text rendering hint.
		/// </summary>
		[Browsable( true )]
		[Category( "Render Quality" )]
		[Description( "Specifies text hinting mode." )]
		[DefaultValue( System.Drawing.Text.TextRenderingHint.SystemDefault )]
		public TextRenderingHint GraphicsTextRenderingHint
		{
			get
			{
				return edtCode.GraphicsTextRenderingHint;
			}
			set
			{
				edtCode.GraphicsTextRenderingHint = value;
			}
		}
		/// <summary>
		/// Gets flag that determines whether undo operation can be done.
		/// </summary>
		[Browsable( false )]
		public bool CanUndo
		{
			get
			{
				return edtCode.CanUndo;
			}
		}
		/// <summary>
		/// Gets flag that determines whether redo operation can be done.
		/// </summary>
		[Browsable( false )]
		public bool CanRedo
		{
			get
			{
				return edtCode.CanRedo;
			}
		}
		/// <summary>
		/// Gets flag that determines whether copy operation can be done.
		/// </summary>
		[Browsable( false )]
		public bool CanCopy
		{
			get
			{
				return edtCode.CanCopy;
			}
		}
		/// <summary>
		/// Gets flag that determines whether paste operation can be done.
		/// </summary>
		[Browsable( false )]
		public bool CanPaste
		{
			get
			{
				return edtCode.CanPaste;
			}
		}
		/// <summary>
		/// Gets flag that determines whether cut operation can be done.
		/// </summary>
		[Browsable( false )]
		public bool CanCut
		{
			get
			{
				return edtCode.CanCut;
			}
		}
		/// <summary>
		/// Gets or sets selected text.
		/// </summary>
		/// <remarks>
		/// If there is no text selected and you are setting new selected text, it will be inserted in the position of the cursor.
		/// Otherwise, when there is some text selected, it will be deleted and new text will be inserted.
		/// </remarks>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public string SelectedText
		{
			get
			{
				return edtCode.SelectedText;
			}
			set
			{
				edtCode.SelectedText = value;
			}
		}
		/// <summary>
		/// Gets or sets size of tabulation.
		/// </summary>
		[Browsable( true )]
		[Category( "Behavior" )]
		[Description( "Specifies tab size in spaces." )]
		[DefaultValue( 2 )]
		[ContextOptions]
		public int TabSize
		{
			get
			{
				return edtCode.TabSize;
			}
			set
			{
				edtCode.TabSize = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether tab symbols should be used.
		/// </summary>
		[Browsable( true )]
		[Category( "Behavior" )]
		[DefaultValue( true )]
		[Description( "Specifies whether tab symbol is allowed or spaces should be used instead." )]
		[ContextOptions]
		public bool UseTabs
		{
			get
			{
				return edtCode.UseTabs;
			}
			set
			{
				edtCode.UseTabs = value;
			}
		}
		/// <summary>
		/// Gets or sets plain text representation of the text data the control is working with.
		/// </summary>
		[Category( "Data" )]
		[Browsable( true )]
		[Description( "Specifies editable text of the control." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		new public string Text
		{
			get
			{
				return edtCode.Text;
			}
			[UIPermission( SecurityAction.Assert, Unrestricted = true, Window = UIPermissionWindow.AllWindows )]
			[SecurityPermission( SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode )]
			set
			{
				pnlCommon.Visible = false;
                this.ActualText = value;
				edtCode.Text = value;
				pnlCommon.Visible = true;
			}
		}

        private string m_actualText = null;
        /// <summary>
        /// Gets/Sets the actual text with considering \r as well.
        /// </summary>
        /// <remarks>
        /// Used by WM_GETTEXTLENGTH Message.
        /// </remarks>
        protected internal string ActualText
        {
            get
            {
                return this.m_actualText;
            }
            set
            {
                if (this.m_actualText != value)
                    this.m_actualText = value;
            }
        }

		/// <summary>
		/// Gets value indicating whether underlying stream is closed.
		/// </summary>
		[Browsable( false )]
		[Obsolete( "When underlying stream is closed, new stream is opened." )]
		public bool IsClosed
		{
			get
			{
				return false;
			}
		}
		/// <summary>
		/// Gets text of the current line.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public string CurrentLineText
		{
			get
			{
				return edtCode.CurrentLineText;
			}
		}
		/// <summary>
		/// Gets instance of the current line.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public ILexemLine CurrentLineInstance
		{
			get
			{
				return edtCode.CurrentLineInstance;
			}
		}
		/// <summary>
		/// Gets or sets visibility of the user margin.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Appearance" )]
		[Browsable( false )]
		[Obsolete( "This property was renamed to ShowUserMargin." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public bool ShowRightUserMargin
		{
			get
			{
				return edtCode.ShowUserMargin;
			}
			set
			{
				edtCode.ShowUserMargin = value;
			}
		}
		/// <summary>
		/// Gets or sets visibility of the user margin.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Appearance" )]
		[Description( "Specifies whether user margin should be visible." )]
		[ContextOptions]
		public bool ShowUserMargin
		{
			get
			{
				return edtCode.ShowUserMargin;
			}
			set
			{
				edtCode.ShowUserMargin = value;
			}
		}
		/// <summary>
		/// Gets or sets width of the user margin.
		/// </summary>
		[Category( "Appearance" )]
		[Browsable( false )]
		[Obsolete( "Property was renamed. Use UserMarginWidth instead." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public int UserMarginRightWidth
		{
			get
			{
				return edtCode.UserMarginWidth;
			}
			set
			{
				edtCode.UserMarginWidth = value;
			}
		}
		/// <summary>
		/// Gets or sets width of the user margin.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Specifies user margin width." )]
		[DefaultValue( 100 )]
		public int UserMarginWidth
		{
			get
			{
				return edtCode.UserMarginWidth;
			}
			set
			{
				edtCode.UserMarginWidth = value;
			}
		}
		/// <summary>
		/// Gets list of available languages.
		/// </summary>
		[Browsable( false )]
		public IList Languages
		{
			get
			{
				return edtCode.Languages;
			}
		}
		/// <summary>
		/// Gets or sets currently used config language.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public IConfigLanguage Language
		{
			get
			{
				return edtCode.Language;
			}
			set
			{
				edtCode.Language = value;
			}
		}
		/// <summary>
		/// Gets or sets the value indicating whether changes can be done to the input stream.
		/// </summary>
		[Browsable( true )]
		[Category( "Behavior" )]
		[Description( "Specifies whether edit control is in readonly mode." )]
		[DefaultValue( false )]
		public bool ReadOnly
		{
			get
			{
				return edtCode.ReadOnly;
			}
			set
			{
				edtCode.ReadOnly = value;
			}
		}
		/// <summary>
		/// Gets list of commands.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public IKeyCommandList Commands
		{
			get
			{
				return edtCode.Commands;
			}
		}
		/// <summary>
		/// Gets key binder.
		/// </summary>
		[Browsable( false )]
		[ReadOnly( true )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public IKeyCommandListBinder KeyBinder
		{
			get
			{
				return edtCode.KeyBinder;
			}
		}
		/// <summary>
		/// Gets or sets key binding processor.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public KeyProcessor KeyBindingProcessor
		{
			get
			{
				return edtCode.KeyBindingProcessor;
			}
			set
			{
				edtCode.KeyBindingProcessor = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether grouping should be enabled for undo/redo actions.
		/// </summary>
		[Browsable( true )]
		[Category( "Behavior" )]
		[Description( "Specifies whether undo operations should be grouped." )]
		[DefaultValue( true )]
		[ContextOptions]
		public bool GroupUndo
		{
			get
			{
				return edtCode.GroupUndo;
			}
			set
			{
				edtCode.GroupUndo = value;
			}
		}
		/// <summary>
		/// Gets location of cursor's right-bottom position in control coordinates.
		/// </summary>
		[Browsable( false )]
		public Point CursorGraphicalLocation
		{
			get
			{
				return edtCode.CursorGraphicalLocation;
			}
		}
		/// <summary>
		/// Gets or sets flag that specifies, whether context menu is enabled.
		/// </summary>
		[Browsable( true )]
		[Category( "Behavior" )]
		[Description( "Specifies whether context menu can be shown on right-click." )]
		[DefaultValue( true )]
		public bool ContextMenuEnabled
		{
			get
			{
				return edtCode.ContextMenuEnabled;
			}
			set
			{
				edtCode.ContextMenuEnabled = value;
			}
		}
		/// <summary>
		/// Gets or sets status of the single line mode.
		/// </summary>
		[Browsable( true )]
		[Category( "Appearance" )]
		[Description( "Specifies whether single line mode is enabled." )]
		[DefaultValue( false )]
		[RefreshProperties( RefreshProperties.All )]
		public bool SingleLineMode
		{
			get
			{
				return edtCode.SingleLineMode;
			}
			set
			{
				edtCode.SingleLineMode = value;
				statusBar.Visible = false;
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether removing a read-only region is allowed.
		/// </summary>
		[Browsable(true)]
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Gets or sets a value indicating whether removing a read-only region is allowed.")]
		public bool AllowDeleteReadOnlyRegion
		{
			get 
			{ 
				return edtCode.AllowDeleteReadOnlyRegion; 
			}
			set
			{
				edtCode.AllowDeleteReadOnlyRegion = value;
			}
		}
		/// <summary>
		/// Gets value indicating whether content of the file was modified.
		/// </summary>
		[Browsable( false )]
		public bool IsModified
		{
			get
			{
				return edtCode.IsModified;
			}
		}
		/// <summary>
		/// Gets print document, that can be used to print the contents of the editor.
		/// </summary>
		[Browsable( false )]
		public PrintDocument PrintDocument
		{
			get
			{
				return edtCode.PrintDocument;
			}
		}
		/// <summary>
		/// Gets or sets value that specifies whether context choice should be updated when it is active and user types something.
		/// </summary>
		[Browsable( false )]
		[Category( "Behavior" )]
		[DefaultValue( true )]
		[Obsolete( "This property will be removed in next release." )]
		public bool UpdateContextChoiceList
		{
			get
			{
				return edtCode.UpdateContextChoiceList;
			}
			set
			{
				edtCode.UpdateContextChoiceList = value;
			}
		}
		/// <summary>
		/// Gets or sets transparency of the selection.
		/// </summary>
		[Browsable( true )]
		[Category( "Appearance" )]
		[Description( @"Specifies whether transparent selection should (WinXP style selection) 
			be used or invers selection should be used instead (Standard text selection)." )]
		[DefaultValue( true )]
		[ContextOptions]
		public bool TransparentSelection
		{
			get
			{
				return edtCode.TransparentSelection;
			}
			set
			{
				edtCode.TransparentSelection = value;
			}
		}
		/// <summary>
		/// Gets or sets sign, whether file should be converted when loading.
		/// </summary>
		/// <remarks>Such file conversion is needed if file contains different new-line symbols or sequences.</remarks>
		[DefaultValue( true )]
		[Browsable( true )]
		[Category( "Behavior" )]
		[Description( "Specifies whether all new-line symbols in text should be converted to one common new-line symbol on load." )]
		public bool ConvertOnLoad
		{
			get
			{
				return edtCode.ConvertOnLoad;
			}
			set
			{
				edtCode.ConvertOnLoad = value;
			}
		}
		/// <summary>
		/// Gets or sets file stream, that is used as an input.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public FileStream FileOpened
		{
			get
			{
				return edtCode.FileOpened;
			}
			set
			{
				edtCode.FileOpened = value;
			}
		}
		/// <summary>
		/// Gets or sets name of the currently opened file.
		/// </summary>
		[Browsable( false )]
		[DefaultValue( "" )]
		public string FileName
		{
			get
			{
				return edtCode.FileName;
			}
			set
			{
				edtCode.FileName = value;
                StatusBarSettings.FileNamePanel.Panel.Text = Path.GetFileName(value);
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether file should be opened in shared mode.
		/// </summary>
		[DefaultValue( false )]
		[Browsable( true )]
		[Category( "Behavior" )]
		[Description( @"Specifies whether entire file should be loaded into the memory 
			(so file will be loaded in shared access mode), or it should be loaded just partially with exclusive access." )]
		public bool SharedFileMode
		{
			get
			{
				return edtCode.SharedFileMode;
			}
			set
			{
				edtCode.SharedFileMode = value;
			}
		}
		/// <summary>
		/// Gets or sets the border style of the control.
		/// </summary>
		[DefaultValue( BorderStyle.None )]
		[DispId( -504 )]
		[Category( "Appearance" )]
		[Description( "Specifies style of the controls border." )]
		public new BorderStyle BorderStyle
		{
			get
			{
				return this.borderStyle;
			}
			set
			{
				if( this.borderStyle != value )
				{
					if( !Enum.IsDefined( typeof( System.Windows.Forms.BorderStyle ), value ) )
						throw new InvalidEnumArgumentException( "value", ( ( int )( value ) ), typeof( BorderStyle ) );

					this.borderStyle = value;
					UpdateStyles();
				}
			}
		}
		/// <summary>
		/// Left offset.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public int ScrollOffsetLeft
		{
			get
			{
				return 0;
			}
			set
			{ }
		}
		/// <summary>
		/// Right offset.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public int ScrollOffsetRight
		{
			get
			{
				return 0;
			}
			set
			{ }
		}
		/// <summary>
		/// Top offset.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public int ScrollOffsetTop
		{
			get
			{
				return 0;
			}
			set
			{ }
		}
		/// <summary>
		/// Bottom offset.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public int ScrollOffsetBottom
		{
			get
			{
				return 0;
			}
			set
			{ }
		}
		/// <summary>
		/// Control's virtual size.
		/// </summary>
		/// <remarks>
		/// If control's client area is smaller then virtual size, then
		/// scrollers will be visible.
		/// </remarks>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public Size VirtualSize
		{
			get
			{
				return Size.Empty;
			}
			set
			{ }
		}
		/// <summary>
		/// Gets selected text range.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public ITextRange Selection
		{
			get
			{
				IComplexTextRange selection = edtCode.Selection;

				if( selection.IsEmpty() || selection.Top == selection.Bottom )
				{
					return null;
				}

				return selection;
			}
		}
		/// <summary>
		/// Gets or sets value that indicates whether vertical splitter is visible.
		/// </summary>
		[Browsable( true )]
		[Category( "Appearance" )]
		[Description( "Specifies whether vertical splitters should be visible." )]
		[DefaultValue( true )]
		public bool ShowVerticalSplitters
		{
			get
			{
				return splitterTop.Enabled && splitterBottom.Enabled;
			}
			set
			{
				bool bValue = value && !SingleLineMode;

				if( bValue != ShowVerticalSplitters )
				{
					splitterBottom.Visible = splitterTop.Visible = splitterTop.Enabled = splitterBottom.Enabled = bValue;
					if( !bValue )
					{
						pnlClientRight.Width = pnlTopRight.Width = 0;
					}

					ReAlignAll();
				}
				else
				{
					m_bOldVerticalSplitters = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets value that indicates whether horizontal splitters are visible.
		/// </summary>
		[Browsable( true )]
		[Category( "Appearance" )]
		[Description( "Specifies whether horizontal splitters should be visible." )]
		[DefaultValue( true )]
		public bool ShowHorizontalSplitters
		{
			get
			{
				return splitterCenter.Enabled;
			}
			set
			{
				bool bValue = value && !SingleLineMode;

				if( bValue != ShowHorizontalSplitters )
				{
					splitterCenter.Visible = splitterCenter.Enabled = bValue;
					pnlClient.Height = 0;
					ReAlignAll();
				}
				else
				{
					m_bOldHorizontalSplitters = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether control accepts tabs.
		/// </summary>
		[Browsable( true )]
		[Category( "Behavior" )]
		[Description( @"Specifies whether control should transfer focus to the next control when user presses tab key, 
			or it should insert tab symbol instead." )]
		[DefaultValue( false )]
		public bool TransferFocusOnTab
		{
			get
			{
				return edtCode.TransferFocusOnTab;
			}
			set
			{
				edtCode.TransferFocusOnTab = value;
			}
		}
		bool bEnableMD5 = true;
        /// <summary>
		/// Gets or sets value indicating whether control MD5 support.
		/// </summary>
		[Browsable( true )]
		[Category( "Behavior" )]
		[DefaultValue( true )]
        public bool EnableMD5
        {
            get
            {
                return bEnableMD5;
            }
            set
            {
                if (bEnableMD5 != value)
                {
                    bEnableMD5 = value;
                    edtCode.EnableMD5 = value;
                    
                }
            }
        }
        /// <summary>
        /// Gets or sets value indicating whether Auto indent smart mode work in block mode.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool EnableSmartInBlockIndent
        {
            get
            {
                return edtCode.EnableSmartInBlockIndent;
            }
            set
            {
                edtCode.EnableSmartInBlockIndent = value;
            }
        }
        bool bEnableRTL = true;
        /// <summary>
        /// Gets or sets value indicating whether control RTL support.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool EnableRTL
        {
            get
            {
                return bEnableRTL;
            }
            set
            {
                if (bEnableRTL != value)
                {
                    bEnableRTL = value;
                }
            }
        }
		/// <summary>
		/// Gets or sets width of the selection margin.
		/// </summary>
		[Browsable( true )]
		[Category( "Appearance" )]
		[Description( "Specifies width of the selection margin area." )]
		[DefaultValue( StreamEditControl.DEF_SELECTION_MARGIN_WIDTH )]
		public int SelectionMarginWidth
		{
			get
			{
				return edtCode.SelectionMarginWidth;
			}
			set
			{
				edtCode.SelectionMarginWidth = value;
			}
		}
		/// <summary>
		/// Gets readonly copy of the bookmarks collection.
		/// </summary>
		[Browsable( false )]
		public BookmarksCollection Bookmarks
		{
			get
			{
				return edtCode.Bookmarks.Bookmarks;
			}
		}
		/// <summary>
		/// Gets readonly copy of the custom bookmarks collection.
		/// </summary>
		[Browsable( false )]
		public CustomBookmarksCollection CustomBookmarks
		{
			get
			{
				return edtCode.Bookmarks.CustomBookmarks;
			}
		}
		/// <summary>
		/// Gets or sets text lines array.
		/// </summary>
		[Editor( "System.Windows.Forms.Design.StringArrayEditor, System.Design, Culture=neutral", typeof( UITypeEditor ) )]
		[Category( "Data" )]
		[Description( "Specifies the collection of the text lines." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public string[] Lines
		{
			get
			{
				char[] newlineChars = edtCode.Parser.BaseStream.NewLineStr.ToCharArray();
				return Text.Split( newlineChars );
			}
			set
			{
				if( null == value ) throw new ArgumentNullException( "Lines" );

				string newlineStr = edtCode.Parser.BaseStream.NewLineStr;
				string textNew = string.Join( newlineStr, value );
				Text = textNew;
			}
		}
		/// <summary>
		/// Get properties of Show white spaces mode.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Specifies options of White space mode." )]
		[TypeConverter( typeof( ShowWhiteSpaceProperties.ShowWhiteSpacePropertiesConverter ) )]
		[Browsable( true )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		public ShowWhiteSpaceProperties WhiteSpaceIndicators
		{
			get
			{
				return edtCode.ShowWhiteSpaceProperties;
			}
		}
		/// <summary>
		/// Gets or sets color of user margin border.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Specifies color of user margin border." )]
		[DefaultValue( typeof( Color ), "Black" )]
		public Color UserMarginBorderColor
		{
			get
			{
				return edtCode.UserMarginBorderColor;
			}
			set
			{
				edtCode.UserMarginBorderColor = value;
			}
		}
		/// <summary>
		/// Gets or sets BrushInfo object that is used when user margin is being drawn.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Specifies BrushInfo object that is used when user margin is being drawn." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )]
		[DefaultValue( typeof( BrushInfo ), "Solid; BurlyWood" )]
		public BrushInfo UserMarginBackgroundColor
		{
			get
			{
				return edtCode.UserMarginBackgroundColor;
			}
			set
			{
				edtCode.UserMarginBackgroundColor = value;
			}
		}
		/// <summary>
		/// Gets or sets default font of user margin text.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Specifies default font of user margin text." )]
		public Font UserMarginTextFont
		{
			get
			{
				return edtCode.UserMarginTextFont;
			}
			set
			{
				edtCode.UserMarginTextFont = value;
			}
		}
		/// <summary>
		/// Gets or sets default color of user margin text.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Specifies default color of user margin text." )]
		[DefaultValue( typeof( Color ), "Black" )]
		public Color UserMarginTextColor
		{
			get
			{
				return edtCode.UserMarginTextColor;
			}
			set
			{
				edtCode.UserMarginTextColor = value;
			}
		}
		/// <summary>
		/// Gets or sets state of the word-wrapping mode.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( false )]
		[Category( "Word Wrapping" )]
		[Description( "Specifies whether word wrapping is enabled." )]
		[ContextOptions]
		public bool WordWrap
		{
			get
			{
				return edtCode.WordWrap;
			}
			set
			{
				edtCode.WordWrap = value;
			}
		}
		/// <summary>
		/// Gets or sets type of word wrapping.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( WordWrapType.WrapByWord )]
		[Category( "Word Wrapping" )]
		[Description( "Specifies type of word wrapping." )]
		public WordWrapType WordWrapType
		{
			get
			{
				return edtCode.WrapType;
			}
			set
			{
				edtCode.WrapType = value;
				UpdateSizeInAutoSizeMode();
			}
		}
		/// <summary>
		/// Gets or sets width of wordwrap margin. To make word-wrap margin visible you should set WordWrapMarginVisible property to true.
		/// </summary>
		[Browsable( true )]
		[Category( "Word Wrapping" )]
		[Description( @"Specifies width of the word-wrap margin. 
			To make word-wrap margin visible you should set WordWrapMarginVisible property to true." )]
		[DefaultValue( 600 )]
		public int TextAreaWidth
		{
			get
			{
				return edtCode.TextAreaWidth;
			}
			set
			{
				if( value > 0 )
				{
					edtCode.TextAreaWidth = value;
					UpdateSizeInAutoSizeMode();
				}
				else throw new ArgumentOutOfRangeException( "TextAreaWidth" );
			}
		}
		/// <summary>
		/// Gets or sets bool that indicates whether text area should be shown.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( false )]
		[Category( "Word Wrapping" )]
		[Description( "Specifies whether word-wrap margin should be shown. It can be shown even when word-wrap is off." )]
		[ContextOptions]
		public bool WordWrapMarginVisible
		{
			get
			{
				return edtCode.ShowTextArea;
			}
			set
			{
				edtCode.ShowTextArea = value;
			}
		}
		/// <summary>
		/// Gets or sets style of line that delimits text area.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( System.Drawing.Drawing2D.DashStyle.Dot )]
		[Category( "Word Wrapping" )]
		[Description( "Specifies style of line that is drawn at the border of the word-wrap margin." )]
		public DashStyle WordWrapMarginLineStyle
		{
			get
			{
				return edtCode.TextAreaLineStyle;
			}
			set
			{
				edtCode.TextAreaLineStyle = value;
			}
		}
		/// <summary>
		/// Gets or sets color of line that delimits text area.
		/// </summary>
		[Browsable( true )]
		[Category( "Word Wrapping" )]
		[Description( "Specifies color of line that delimits text area." )]
		[DefaultValue( typeof( Color ), "Black" )]
		public Color WordWrapMarginLineColor
		{
			get
			{
				return edtCode.TextAreaLineColor;
			}
			set
			{
				edtCode.TextAreaLineColor = value;
			}
		}
		/// <summary>
		/// Gets or sets BrushInfo object that is used when area situated after text area is drawn.
		/// </summary>
		[Browsable( true )]
		[Category( "Word Wrapping" )]
		[Description( "Specifies BrushInfo object that is used when area situated after text area is drawn." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )]
		[DefaultValue( typeof( BrushInfo ), "Solid; BlanchedAlmond" )]
		public BrushInfo WordWrapMarginBrush
		{
			get
			{
				return edtCode.AfterTextAreaBrush;
			}
			set
			{
				edtCode.AfterTextAreaBrush = value;
			}
		}
		/// <summary>
		/// Gets or sets mode of word wrapping.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( WordWrapMode.Control )]
		[Category( "Word Wrapping" )]
		[Description( "Specifies mode of word wrapping." )]
		public WordWrapMode WordWrapMode
		{
			get
			{
				return edtCode.WrapMode;
			}
			set
			{
				edtCode.WrapMode = value;
				UpdateSizeInAutoSizeMode();
			}
		}
		/// <summary>
		/// Gets or sets width of marker area.
		/// </summary>
		[Browsable( true )]
		[Category( "Appearance" )]
		[Description( "Specifies width of marker area." )]
		[DefaultValue( 16 )]
		public int MarkerAreaWidth
		{
			get
			{
				return edtCode.MarkerAreaWidth;
			}
			set
			{
				if( value > 0 )
				{
					edtCode.MarkerAreaWidth = value;
				}
				else throw new ArgumentOutOfRangeException( "MarkerAreaWidth" );
			}
		}
		/// <summary>
		/// Gets or sets position of the horizontal splitter.
		/// </summary>
		[Browsable( true )]
		[Category( "Layout" )]
		[Description( "Specifies position of the horizontal splitter." )]
		[DefaultValue( 0 )]
		public int HorizontalSplitterPosition
		{
			get
			{
				return splitterCenter.SplitPosition;
			}
			set
			{
				if( value < 0 ) throw new ArgumentOutOfRangeException( "HorizontalSplitterPosition" );

				if( value != splitterCenter.SplitPosition )
				{
					splitterCenter.SplitPosition = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets position of the bottom vertical splitter.
		/// </summary>
		[Browsable( true )]
		[Category( "Layout" )]
		[Description( "Specifies position of the bottom vertical splitter." )]
		[DefaultValue( 0 )]
		public int BottomVerticalSplitterPosition
		{
			get
			{
				return splitterBottom.SplitPosition;
			}
			set
			{
				if( value < 0 ) throw new ArgumentOutOfRangeException( "BottomVerticalSplitterPosition" );

				if( value != splitterBottom.SplitPosition )
				{
					splitterBottom.SplitPosition = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets position of the top vertical splitter.
		/// </summary>
		[Browsable( true )]
		[Category( "Layout" )]
		[Description( "Specifies position of the top vertical splitter." )]
		[DefaultValue( 0 )]
		public int TopVerticalSplitterPosition
		{
			get
			{
				return splitterTop.SplitPosition;
			}
			set
			{
				if( value < 0 ) throw new ArgumentOutOfRangeException( "TopVerticalSplitterPosition" );

				if( value != splitterTop.SplitPosition )
				{
					splitterTop.SplitPosition = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets array of tab stops.
		/// </summary>
		[Browsable( true )]
		[Category( "Behavior" )]
		[Description( "Specifies array of tab stops." )]
		public int[] TabStopsArray
		{
			get
			{
				return edtCode.TabStopsArray;
			}
			set
			{
				edtCode.TabStopsArray = value;
			}
		}
		/// <summary>
		/// Gets or sets bool that indicates whether tab stops should be used.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( false )]
		[Category( "Behavior" )]
		[Description( "Specifies whether tab stops should be used." )]
		[ContextOptions]
		public bool UseTabStops
		{
			get
			{
				return edtCode.UseTabStops;
			}
			set
			{
				edtCode.UseTabStops = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating color of the indicatormargin.
		/// </summary>
		[Browsable(true)]
		[DefaultValue(true)]
		[Category("Appearance")]
		[Description("Specifies backcolor of the indicator margin.")]
		[ContextOptions]
		public Color IndicatorMarginBackColor
		{
			get
			{
				return edtCode.IndicatorMarginColor;
			}
			set
			{
				if (value != edtCode.IndicatorMarginColor)
				{
					edtCode.IndicatorMarginColor = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether XP style should be used.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( true )]
		[Category( "Appearance" )]
		[Description( "Specifies whether XP style should be used." )]
		[ContextOptions]
		public bool UseXPStyle
		{
			get
			{
				return edtCode.UseXPStyle;
			}
			set
			{
				if( value != edtCode.UseXPStyle )
				{
					edtCode.UseXPStyle = value;

					splitterBottom.Invalidate();
					splitterCenter.Invalidate();
					splitterTop.Invalidate();
					InvalidateNonClientArea();

                    ScrollBarCustomDrawStyles style = this.UseXPStyle ? ScrollBarCustomDrawStyles.WindowsXP : ScrollBarCustomDrawStyles.Classic;
                    this.ScrollVisualStyle = style;
					
                    UpdateScrollersVisualStyle();
				}
			}
		}

        /// <summary>
        /// Gets or sets value indicating whether XP style Border should be used.
        /// </summary>
        public bool UseXPStyleBorder
        {
            get
            {
                return edtCode.UseXPStyleBorder;
            }
            set
            {
                if (value != edtCode.UseXPStyleBorder)
                {
                    edtCode.UseXPStyleBorder = value;
                }
            }
        }
		/// <summary>
		/// Gets context choice controller.
		/// </summary>
		[Browsable( false )]
		public IContextChoiceController ContextChoiceController
		{
			get
			{
				return edtCode.ContextChoice;
			}
		}
		/// <summary>
		/// Gets or sets mode of auto indentation.
		/// </summary>
		[Browsable( true )]
		[Category( "Behavior" )]
		[DefaultValue( AutoIndentMode.None )]
		[Description( "Specifies mode of auto indentation." )]
		[ContextOptions]
		public AutoIndentMode AutoIndentMode
		{
			get
			{
				return edtCode.AutoIndentMode;
			}
			set
			{
				edtCode.AutoIndentMode = value;
			}
		}
		/// <summary>
		/// Gets or sets bool indicating whether context tooltips are shown.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( true )]
		[Category( "Behavior" )]
		[Description( "Specifies whether context tooltips are shown." )]
		public bool ShowContextTooltip
		{
			get
			{
				return edtCode.ShowContextTooltip;
			}
			set
			{
				edtCode.ShowContextTooltip = value;
			}
		}
        /// <summary>
        /// Gets or sets bool indicating whether Content Dividers are shown.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("Specifies whether Content Dividers are shown.")]
        public bool ShowContentDividers
        {
            get
            {
                return edtCode.ShowContentDividers;
            }
            set
            {
                edtCode.ShowContentDividers = value;
            }
        }
		/// <summary>
		/// Gets or sets bool indicating whether smart auto indent should be used. When set to true, AutoIndentMode is set to Smart.
		/// When set to false, AutoIndentMode is set to None. Obsolete, use AutoIndentMode now.
		/// </summary>
		[Browsable( false )]
		[DefaultValue( false )]
		[Category( "Behavior" )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		[Obsolete( "Please use AutoIndentMode property instead of this one. This property will be removed in a future releases." )]
		public bool AutoIndent
		{
			get
			{
				return ( AutoIndentMode.None != AutoIndentMode );
			}
			set
			{
				if( value && AutoIndentMode == AutoIndentMode.None )
				{
					AutoIndentMode = AutoIndentMode.Smart;
				}
				if( !value && AutoIndentMode != AutoIndentMode.None )
				{
					AutoIndentMode = AutoIndentMode.None;
				}
			}
		}
		/// <summary>
		/// Gets or sets bool indicating whether collapsed text is shown in tooltip when mouse hovers over collapsed section.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( true )]
		[Category( "Intellisense Appearance" )]
		[Description( "Specifies whether collapsed text is shown in tooltip when mouse hovers over collapsed section." )]
		public bool ShowOutliningTooltip
		{
			get
			{
				return edtCode.ShowOutliningTooltip;
			}
			set
			{
				edtCode.ShowOutliningTooltip = value;
			}
		}
		/// <summary>
		/// Gets settings of status bar.
		/// </summary>
		[Browsable( true )]
		[Category( "Appearance" )]
		[Description( "Specifies settings of status bar." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		public StatusBarSettings StatusBarSettings
		{
			get
			{
				return m_statusBarSettings;
			}
		}
		/// <summary>
		/// Gets or sets bool that indicates whether lines wrapping should be marked.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( true )]
		[Category( "Word Wrapping" )]
		[Description( "Specifies whether lines wrapping should be marked." )]
		[ContextOptions]
		public bool MarkLineWrapping
		{
			get
			{
				return edtCode.MarkLineWrapping;
			}
			set
			{
				edtCode.MarkLineWrapping = value;
			}
		}
		/// <summary>
		/// Gets or sets custom image that marks lines wrapping.
		/// </summary>
		[Browsable( true )]
		[Category( "Word Wrapping" )]
		[Description( "Specifies custom image that marks lines wrapping." )]
		[DefaultValue( null )]
		public Image CustomLineWrappingMarkingImage
		{
			get
			{
				return edtCode.CustomLineWrappingMarkingImage;
			}
			set
			{
				edtCode.CustomLineWrappingMarkingImage = value;
			}
		}
		/// <summary>
		/// Gets or sets size of the context choice form.
		/// </summary>
		[Browsable( true )]
		[Category( "Intellisense Appearance" )]
		[Description( "Specifies size of the context choice form." )]
		[DefaultValue( typeof( Size ), "176, 88" )]
		public Size ContextChoiceSize
		{
			get
			{
				return ContextChoiceController.FormSize;
			}
			set
			{
				ContextChoiceController.FormSize = value;
			}
		}
		/// <summary>
		/// Gets or sets size of context prompt.
		/// </summary>
		[Browsable( true )]
		[Category( "Intellisense Appearance" )]
		[Description( @"Specifies custom size of the context prompt dropdown.
			The specified value is used only if UseCustomSizeContextPrompt is set to true." )]
		[DefaultValue( typeof( Size ), "400, 50" )]
		public Size ContextPromptCustomSize
		{
			get
			{
				return edtCode.ContextPromptSize;
			}
			set
			{
				edtCode.ContextPromptSize = value;
			}
		}
        /// <summary>
        /// Gets or Sets the Codesnippet size
        /// </summary>
		public Size CodeSnipptSize
		{
			get
			{
				return edtCode.CodeSnipptSize;
			}
			set
			{
				if (!value.IsEmpty)
				{
					edtCode.CodeSnipptSize = value;
				}
				else throw new ArgumentOutOfRangeException("FormSize");
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether custom context prompt size should be used.
		/// </summary>
		[Browsable( true )]
		[Category( "Intellisense Appearance" )]
		[Description( "Indicates whether custom context prompt size should be used." )]
		[DefaultValue( false )]
		public bool UseCustomSizeContextPrompt
		{
			get
			{
				return edtCode.UseCustomSizeContextPrompt;
			}
			set
			{
				edtCode.UseCustomSizeContextPrompt = value;
			}
		}
		/// <summary>
		/// Gets or sets font of line numbers.
		/// </summary>
		[Browsable( true )]
		[Category( "Appearance" )]
		[Description( "Specifies font of line numbers." )]
		public Font LineNumbersFont
		{
			get
			{
				return edtCode.LineNumbersFont;
			}
			set
			{
				edtCode.LineNumbersFont = value;
			}
		}
		/// <summary>
		/// Gets or sets color of line numbers.
		/// </summary>
		[Browsable( true )]
		[Category( "Appearance" )]
		[Description( "Specifies color of line numbers." )]
		[DefaultValue( typeof( Color ), "DarkBlue" )]
		public Color LineNumbersColor
		{
			get
			{
				return edtCode.LineNumbersColor;
			}
			set
			{
				edtCode.LineNumbersColor = value;
			}
		}
		/// <summary>
		/// Gets or sets color of context choice form border.
		/// </summary>
		[Browsable( true )]
		[Category( "Intellisense Appearance" )]
		[Description( "Specifies color of context choice form border. Used when UseXPStyle is set to false, otherwise 3D border is drawn." )]
		[DefaultValue( typeof( Color ), "Black" )]
		public Color ContextChoiceBorderColor
		{
			get
			{
				return edtCode.ContextChoice.FormBorderColor;
			}
			set
			{
				edtCode.ContextChoice.FormBorderColor = value;
			}
		}
		/// <summary>
		/// Gets or sets color of context prompt form border.
		/// </summary>
		[Browsable( true )]
		[Category( "Intellisense Appearance" )]
		[Description( "Specifies color of context prompt form border. Used when UseXPStyle is set to false, otherwise 3D border is drawn." )]
		[DefaultValue( typeof( Color ), "Black" )]
		public Color ContextPromptBorderColor
		{
			get
			{
				return edtCode.ContextPromptBorderColor;
			}
			set
			{
				edtCode.ContextPromptBorderColor = value;
			}
		}
		/// <summary>
		/// Gets or sets color of context tooltip form border.
		/// </summary>
		[Browsable( true )]
		[Category( "Intellisense Appearance" )]
		[Description( "Specifies color of context tooltip form border. Used when UseXPStyle is set to false, otherwise 3D border is drawn." )]
		[DefaultValue( typeof( Color ), "Black" )]
		public Color ContextTooltipBorderColor
		{
			get
			{
				return edtCode.ContextTooltipBorderColor;
			}
			set
			{
				edtCode.ContextTooltipBorderColor = value;
			}
		}
		/// <summary>
		/// Gets or sets color of bookmark tooltip form border.
		/// </summary>
		[Browsable( true )]
		[Category( "Intellisense Appearance" )]
		[Description( "Specifies color of bookmark tooltip form border. Used when UseXPStyle is set to false,	otherwise 3D border is drawn." )]
		[DefaultValue( typeof( Color ), "Black" )]
		public Color BookmarkTooltipBorderColor
		{
			get
			{
				return edtCode.BookmarkTooltipBorderColor;
			}
			set
			{
				edtCode.BookmarkTooltipBorderColor = value;
			}
		}
		/// <summary>
		/// Gets or sets brush for context tooltip background.
		/// </summary>
		[Browsable( true )]
		[Category( "Intellisense Appearance" )]
		[Description( "Specifies brush info for context tooltip background." )]
		[DefaultValue( typeof( BrushInfo ), "Solid; LemonChiffon" )]
		public BrushInfo ContextTooltipBackgroundBrush
		{
			get
			{
				return edtCode.ContextTooltipBackgroundBrush;
			}
			set
			{
				edtCode.ContextTooltipBackgroundBrush = value;
			}
		}
		/// <summary>
		/// Gets or sets brush for bookmark tooltip background.
		/// </summary>
		[Browsable( true )]
		[Category( "Intellisense Appearance" )]
		[Description( "Specifies brush info for bookmark tooltip background." )]
		[DefaultValue( typeof( BrushInfo ), "Solid; LemonChiffon" )]
		public BrushInfo BookmarkTooltipBackgroundBrush
		{
			get
			{
				return edtCode.BookmarkTooltipBackgroundBrush;
			}
			set
			{
				edtCode.BookmarkTooltipBackgroundBrush = value;
			}
		}
		/// <summary>
		/// Gets or sets brush for context prompt background.
		/// </summary>
		[Browsable( true )]
		[Category( "Intellisense Appearance" )]
		[Description( "Specifies brush info for context prompt background." )]
		[DefaultValue( typeof( BrushInfo ), "Solid; LemonChiffon" )]
		public BrushInfo ContextPromptBackgroundBrush
		{
			get
			{
				return edtCode.ContextPromptBackgroundBrush;
			}
			set
			{
				edtCode.ContextPromptBackgroundBrush = value;
			}
		}
		/// <summary>
		/// Gets or sets brush for indentation block background.
		/// </summary>
		[Browsable( true )]
		[Category( "Appearance" )]
		[Description( "Specifies brush info for indentation block background." )]
		[DefaultValue( typeof( BrushInfo ), "Solid; LightGray" )]
		public BrushInfo IndentationBlockBackgroundBrush
		{
			get
			{
				return edtCode.IndentationBlockBackgroundBrush;
			}
			set
			{
				edtCode.IndentationBlockBackgroundBrush = value;
			}
		}
		/// <summary>
		/// Gets or sets array of ColumnGuideItem objects.
		/// </summary>
		[Browsable( true )]
		[Category( "Column Guidelines" )]
		[Description( "Specifies array of ColumnGuideItem objects." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		public ColumnGuideItem[] ColumnGuideItems
		{
			get
			{
				return edtCode.ColumnGuideItems;
			}
			set
			{
				edtCode.ColumnGuideItems = value;
			}
		}
		/// <summary>
		/// Gets or sets bool that indicates whether column guides should be drawn.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( true )]
		[Category( "Column Guidelines" )]
		[Description( "Specifies whether column guides should be drawn." )]
		[ContextOptions]
		public bool ShowColumnGuides
		{
			get
			{
				return edtCode.ShowColumnGuides;
			}
			set
			{
				edtCode.ShowColumnGuides = value;
			}
		}
		/// <summary>
		/// Gets or sets font that is used while measuring position of column guides.
		/// </summary>
		[Browsable( true )]
		[Category( "Column Guidelines" )]
		[Description( "Specifies font that is used while measuring position of column guides." )]
		[DefaultValue( typeof( Font ), "Courier New, 10pt" )]
		public Font ColumnGuidesMeasuringFont
		{
			get
			{
				return edtCode.ColumnGuidesMeasuringFont;
			}
			set
			{
				edtCode.ColumnGuidesMeasuringFont = value;
			}
		}
		/// <summary>
		/// Gets or sets style of new line of the newly created stream.
		/// </summary>
		[Browsable( true )]
		[Category( "Behavior" )]
		[Description( "Specifies style of new line of the newly created stream." )]
		[DefaultValue( NewLineStyle.Control )]
		public NewLineStyle DefaultNewLineStyle
		{
			get
			{
				return edtCode.DefaultNewLineStyle;
			}
			set
			{
				edtCode.DefaultNewLineStyle = value;
			}
		}
		/// <summary>
		/// Gets or sets bool that indicates whether indentation block borders should be drawn.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( false )]
		[Category( "Appearance" )]
		[Description( "Specifies whether indentation block borders should be drawn." )]
		[ContextOptions]
		public bool ShowIndentationBlockBorders
		{
			get
			{
				return edtCode.ShowIndentationBlockBorders;
			}
			set
			{
				edtCode.ShowIndentationBlockBorders = value;
			}
		}
		/// <summary>
		/// Gets or sets style of indentation block border line.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( FrameBorderStyle.Solid )]
		[Category( "Appearance" )]
		[Description( "Specifies style of indentation block border line." )]
		public FrameBorderStyle IndentationBlockBorderStyle
		{
			get
			{
				return edtCode.IndentationBlockBorderStyle;
			}
			set
			{
				edtCode.IndentationBlockBorderStyle = value;
			}
		}
		/// <summary>
		/// Gets or sets color of indentation block border line.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( typeof( Color ), "Gray" )]
		[Category( "Appearance" )]
		[Description( "Specifies color of indentation block border line." )]
		public Color IndentationBlockBorderColor
		{
			get
			{
				return edtCode.IndentationBlockBorderColor;
			}
			set
			{
				edtCode.IndentationBlockBorderColor = value;
			}
		}
		/// <summary>
		/// Gets or sets bool that indicates whether outer file dragged and dropped into Edit Control should be inserted into current content.
		/// When set to false, current file is closed, and dropped outer file is opened.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( false )]
		[Category( "Behavior" )]
		[Description( @"Specifies whether outer file dragged & dropped into Edit Control should be inserted into current content. 
			When set to false, current file is closed, and dropped outer file is opened." )]
		public bool InsertDroppedFileIntoText
		{
			get
			{
				return edtCode.InsertDroppedFileIntoText;
			}
			set
			{
				edtCode.InsertDroppedFileIntoText = value;
			}
		}
		/// <summary>
		/// Gets or sets bool that indicates whether context choice list gets autocompleted when single lexem remains in the list.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( false )]
		[Category( "Advanced" )]
		[Description( "Specifies whether context choice list gets autocompleted when single lexem remains in the list." )]
		public bool AutoCompleteSingleLexem
		{
			get
			{
				return edtCode.AutoCompleteSingleLexem;
			}
			set
			{
				edtCode.AutoCompleteSingleLexem = value;
			}
		}
		/// <summary>
		/// Gets or sets bool that indicates whether wrapped lines should be marked.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( true )]
		[Category( "Word Wrapping" )]
		[Description( "Specifies whether wrapped lines should be marked." )]
		[ContextOptions]
		public bool MarkWrappedLines
		{
			get
			{
				return edtCode.MarkWrappedLines;
			}
			set
			{
				edtCode.MarkWrappedLines = value;
			}
		}
		/// <summary>
		/// Gets or sets custom image that marks wrapped lines.
		/// </summary>
		[Browsable( true )]
		[Category( "Word Wrapping" )]
		[Description( "Specifies custom image that marks wrapped lines ." )]
		[DefaultValue( null )]
		public Image CustomWrappedLinesMarkingImage
		{
			get
			{
				return edtCode.CustomWrappedLinesMarkingImage;
			}
			set
			{
				edtCode.CustomWrappedLinesMarkingImage = value;
			}
		}
		/// <summary>
		/// Gets or sets offset of paragraphs.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( 0 )]
		[Category( "Behavior" )]
		[Description( "Specifies offset of paragraphs." )]
		public int ParagraphOffset
		{
			get
			{
				return edtCode.ParagraphOffset;
			}
			set
			{
				edtCode.ParagraphOffset = value;
			}
		}
		/// <summary>
		/// Gets or sets offset of wrapped lines.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( 0 )]
		[Category( "Word Wrapping" )]
		[Description( "Specifies offset of wrapped lines." )]
		public int WrappedLinesOffset
		{
			get
			{
				return edtCode.WrappedLinesOffset;
			}
			set
			{
				edtCode.WrappedLinesOffset = value;
			}
		}
		/// <summary>
		/// Gets autoformatting manager.
		/// </summary>
		[Browsable( false )]
		public AutoFormattingManager AutoFormattingManager
		{
			get
			{
				return edtCode.AutoFormattingManager;
			}
		}
		/// <summary>
		/// Gets or sets bool that indicates whether changed lines should be marked.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( false )]
		[Category( "Appearance" )]
		[Description( "Specifies whether changed lines should be marked." )]
		[ContextOptions]
		public bool MarkChangedLines
		{
			get
			{
				return edtCode.MarkChangedLines;
			}
			set
			{
				edtCode.MarkChangedLines = value;
			}
		}
		/// <summary>
		/// Gets or sets color of changed lines marking line.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( typeof( Color ), "Yellow" )]
		[Category( "Appearance" )]
		[Description( "Specifies color of changed lines marking line." )]
		public Color ChangedLinesMarkingLineColor
		{
			get
			{
				return edtCode.ChangedLinesMarkingLineColor;
			}
			set
			{
				edtCode.ChangedLinesMarkingLineColor = value;
			}
		}
		/// <summary>
		/// Gets or sets color of saved lines marking line.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( typeof( Color ), "Lime" )]
		[Category( "Appearance" )]
		[Description( "Specifies color of saved lines marking line." )]
		public Color SavedLinesMarkingLineColor
		{
			get
			{
				return edtCode.SavedLinesMarkingLineColor;
			}
			set
			{
				edtCode.SavedLinesMarkingLineColor = value;
			}
		}
		/// <summary>
		/// Gets or sets bool indicating whether bookmark tooltips are shown.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( true )]
		[Category( "Behavior" )]
		[Description( "Specifies whether bookmark tooltips are shown." )]
		public bool ShowBookmarkTooltip
		{
			get
			{
				return edtCode.ShowBookmarkTooltips;
			}
			set
			{
				edtCode.ShowBookmarkTooltips = value;
			}
		}
		/// <summary>
		/// Gets or sets find dialog form.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public IFindDialogForm FindControl
		{
			get
			{
				return edtCode.FindDialogWnd;
			}
			set
			{
				edtCode.FindDialogWnd = value;
			}
		}
		/// <summary>
		/// Gets or sets replace dialog form.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public IReplaceDialogForm ReplaceControl
		{
			get
			{
				return edtCode.ReplaceDialogWnd;
			}
			set
			{
				edtCode.ReplaceDialogWnd = value;
			}
		}
		/// <summary>
		/// Gets or sets goto dialog form.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public IGotoDialogForm GotoControl
		{
			get
			{
				return edtCode.GotoDialogWnd;
			}
			set
			{
				edtCode.GotoDialogWnd = value;
			}
		}
		/// <summary>
		/// Gets display name of the file.
		/// </summary>
		[Browsable( false )]
		public string DisplayedFileName
		{
			get
			{
				return edtCode.DisplayFileName;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether config file should be loaded. If set to false, default language is created from code.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( true )]
		[Category( "Behavior" )]
		[Description( "Specifies whether config file should be loaded. If set to false, default language is created from code." )]
		[Obsolete( "Is ignored." )]
		public bool LoadConfigFile
		{
			get
			{
				return edtCode.LoadConfigFile;
			}
			set
			{
				edtCode.LoadConfigFile = value;
			}
		}
		/// <summary>
		/// Gets or sets bool indicating whether static data should be cleared on dispose.
		/// Frees memory, but impairs time of next loading of EditControl.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( true )]
		[Category( "Advanced" )]
		[Description( "Specifies whether static data should be cleared on dispose. Frees memory, but impairs time of next loading of EditControl." )]
		public bool ClearStaticDataOnDispose
		{
			get
			{
				return m_bClearStaticDataOnDispose;
			}
			set
			{
				m_bClearStaticDataOnDispose = value;
			}
		}
		/// <summary>
		/// Gets history of Find dialog.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( true )]
		[Category( "Behavior" )]
		[Description( @"Search history of Find dialog." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		[Editor( "System.Windows.Forms.Design.StringCollectionEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing" )]
		public ArrayList FindHistory
		{
			get
			{
				return edtCode.FindDialogWnd.History;
			}
		}
		/// <summary>
		/// Gets search history of Replace dialog.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( true )]
		[Category( "Behavior" )]
		[Description( @"Search history of Replace dialog." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		[Editor( "System.Windows.Forms.Design.StringCollectionEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing" )]
		public ArrayList ReplaceSearchHistory
		{
			get
			{
				return edtCode.ReplaceDialogWnd.History;
			}
		}
		/// <summary>
		/// Gets history of Replace dialog.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( true )]
		[Category( "Behavior" )]
		[Description( @"Replace History of Replace dialog." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		[Editor( "System.Windows.Forms.Design.StringCollectionEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing" )]
		public ArrayList ReplaceHistory
		{
			get
			{
				return edtCode.ReplaceDialogWnd.ReplaceHistory;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether autoreplace triggers should be used.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( true )]
		[Category( "Behavior" )]
		[Description( @"Specifies whether autoreplace triggers should be used." )]
		public bool UseAutoreplaceTriggers
		{
			get
			{
				return edtCode.UseAutoreplaceTriggers;
			}
			set
			{
				edtCode.UseAutoreplaceTriggers = value;
			}
		}
		/// <summary>
		/// Gets or sets custom bitmap for mouse pointer.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( null )]
		[Category( "Behavior" )]
		[Description( @"Specifies custom bitmap for mouse pointer." )]
		public Bitmap CustomCursor
		{
			get
			{
				return m_imgCursor;
			}
			set
			{
				if( m_imgCursor != value )
				{
					m_imgCursor = value;

					edtCode.DefaultCursor = ( m_imgCursor != null ) ? ( new Cursor( m_imgCursor.GetHicon() ) ) : ( null );
				}
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether border should be drawn around active code snippets.
		/// </summary>
		[Browsable( true )]
		[Description( "Indicates whether border should be drawn around active code snippets." )]
		[DefaultValue( false )]
		[Category( "Appearance" )]
		public bool DrawCodeSnippetBorder
		{
			get
			{
				return edtCode.DrawCodeSnippetBorder;
			}
			set
			{
				edtCode.DrawCodeSnippetBorder = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether current line should be highlighted.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( false )]
		[Category( "Appearance" )]
		[Description( "Specifies whether current line should be highlighted." )]
		public bool HighlightCurrentLine
		{
			get
			{
				return edtCode.HighlightCurrentLine;
			}
			set
			{
				edtCode.HighlightCurrentLine = value;
			}
		}
		/// <summary>
		/// Gets or sets color of current line highlight.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( typeof( Color ), "Gray" )]
		[Category( "Appearance" )]
		[Description( "Specifies color of current line highlight." )]
		public Color CurrentLineHighlightColor
		{
			get
			{
				return edtCode.CurrentLineHighlightColor;
			}
			set
			{
				edtCode.CurrentLineHighlightColor = value;
			}
		}
		/// <summary>
		/// Gets or sets column for wrapping text. Used when WordWrapMode is set to SpecifiedColumn.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( 100 )]
		[Category( "Word Wrapping" )]
		[Description( "Specifies column for wrapping text. Used when WordWrapMode is set to SpecifiedColumn." )]
		public int WordWrapColumn
		{
			get
			{
				return edtCode.WordWrapColumn;
			}
			set
			{
				edtCode.WordWrapColumn = value;
			}
		}
		/// <summary>
		/// Gets or sets font that is used while calculating position of WordWrapColumn.
		/// </summary>
		[Browsable( true )]
		[Category( "Word Wrapping" )]
		[Description( "Specifies font that is used in calculating position of WordWrapColumn." )]
		[DefaultValue( typeof( Font ), "Courier New, 10pt" )]
		public Font WordWrapColumnMeasuringFont
		{
			get
			{
				return edtCode.WordWrapColumnMeasuringFont;
			}
			set
			{
				edtCode.WordWrapColumnMeasuringFont = value;
			}
		}
		/// <summary>
		/// Gets or sets delay for tooltips in milliseconds.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( 1000 )]
		[Category( "Behavior" )]
		[Description( "delay for tooltips in milliseconds." )]
		public int ToolTipDelay
		{
			get
			{
				return edtCode.ToolTipDelay;
			}
			set
			{
				edtCode.ToolTipDelay = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether inserting text should be allowed at the beginning of readonly region at the start of new line.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( true )]
		[Category( "Advanced" )]
		[Description( "Specifies whether inserting text should be allowed at the beginning of readonly region at the start of new line." )]
		public bool AllowInsertBeforeReadonlyNewLine
		{
			get
			{
				return edtCode.AllowInsertBeforeReadonlyNewLine;
			}
			set
			{
				edtCode.AllowInsertBeforeReadonlyNewLine = value;
			}
		}
		/// <summary>
		/// Gets or sets file name to be shown in SaveAs dialog.
		/// </summary>
		[Browsable( false )]
		[DefaultValue( "" )]
		public string PseudoFileName
		{
			get
			{
				return edtCode.PseudoFileName;
			}
			set
			{
				edtCode.PseudoFileName = value;
			}
		}
        /// <summary>
        /// Specifies the scrollbar Visual Style.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Specifies the scrollbar Visual Style.")]
        [DefaultValue(typeof(ScrollBarCustomDrawStyles), "WindowsXP")]
        public ScrollBarCustomDrawStyles ScrollVisualStyle
        {
            get { return m_scrollVisualStyle; }
            set 
            {
                if (m_scrollVisualStyle != value)
                    m_scrollVisualStyle = value;

                UpdateScrollersVisualStyle();
            }
        }
        /// <summary>
        /// Specifies how the control process vertical scrolling.
        /// </summary>
        [Description("Specifies how the control process vertical scrolling."), DefaultValue(ScrollMode.Pixel)]
        public ScrollMode VScrollMode
        {
            get
            {
                return this.edtCode.VScrollMode;
            }
            set
            {
                this.edtCode.VScrollMode = value;
            }
        }
        /// <summary>
        /// Specifies the scrollbar color scheme when Office2007 or Office2007Generic Style is set.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Specifies the scrollbar color scheme when Office2007 or Office2007Generic Style is set.")]
        [DefaultValue(typeof(Office2007ColorScheme), "Blue")]
        public Office2007ColorScheme ScrollColorScheme
        {
            get { return m_scrollColorScheme; }
            set
            {
                if (m_scrollColorScheme != value)
                    m_scrollColorScheme = value;

                UpdateScrollersVisualStyle();
            }
        }
		/// <summary>
		/// Gets buttons on the top of vertical scrollbar.
		/// </summary>
		[Browsable( true )]
		[Category( "ScrollbarButtons" )]
		[Description( "Specifies buttons on the top of vertical scrollbar." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		[Editor( typeof( ScrollbarButtonsCollectionEditor ), typeof( UITypeEditor ) )]
		public ControlsCollection ScrollbarTopButtons
		{
			get
			{
				return m_scrollersFrame.VerticalScroller.ControlsBefore;
			}
		}
		/// <summary>
		/// Gets buttons on the bottom of vertical scrollbar.
		/// </summary>
		[Browsable( true )]
		[Category( "ScrollbarButtons" )]
		[Description( "Specifies buttons on the bottom of vertical scrollbar." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		[Editor( typeof( ScrollbarButtonsCollectionEditor ), typeof( UITypeEditor ) )]
		public ControlsCollection ScrollbarBottomButtons
		{
			get
			{
				return m_scrollersFrame.VerticalScroller.ControlsAfter;
			}
		}
		/// <summary>
		/// Gets buttons on the left of horizontal scrollbar.
		/// </summary>
		[Browsable( true )]
		[Category( "ScrollbarButtons" )]
		[Description( "Specifies buttons on the left of horizontal scrollbar." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		[Editor( typeof( ScrollbarButtonsCollectionEditor ), typeof( UITypeEditor ) )]
		public ControlsCollection ScrollbarLeftButtons
		{
			get
			{
				return m_scrollersFrame.HorizontalScroller.ControlsBefore;
			}
		}
		/// <summary>
		/// Gets buttons on the right of vertical scrollbar.
		/// </summary>
		[Browsable( true )]
		[Category( "ScrollbarButtons" )]
		[Description( "Specifies buttons on the right of vertical scrollbar." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		[Editor( typeof( ScrollbarButtonsCollectionEditor ), typeof( UITypeEditor ) )]
		public ControlsCollection ScrollbarRightButtons
		{
			get
			{
				return m_scrollersFrame.HorizontalScroller.ControlsAfter;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether scrollers should be always visible.
		/// </summary>
		[Browsable( true )]
		[Category( "ScrollbarButtons" )]
		[Description( "Specifies value indicating whether scrollers should be always visible." )]
		[DefaultValue( false )]
		public bool AlwaysShowScrollers
		{
			get
			{
				return edtCode.AlwaysShowScrollers;
			}
			set
			{
				edtCode.AlwaysShowScrollers = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether context choice items should be filtered while typing.
		/// </summary>
		[Browsable( true )]
		[Category( "Behavior" )]
		[Description( "Specifies value indicating whether context choice items should be filtered while typing." )]
		[DefaultValue( true )]
		public bool FilterAutoCompleteItems
		{
			get
			{
				return this.ContextChoiceController.UseAutocomplete;
			}
			set
			{
				this.ContextChoiceController.UseAutocomplete = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether context choice items filtering string should be extended back to the whitespace.
		/// </summary>
		[Browsable( true )]
		[Category( "Advanced" )]
		[Description( "Specifies value indicating whether context choice items filtering string should be extended back to the whitespace." )]
		[DefaultValue( true )]
		public bool ExtendItemsFilteringString
		{
			get
			{
				return this.ContextChoiceController.ExtendItemsFilteringString;
			}
			set
			{
				this.ContextChoiceController.ExtendItemsFilteringString = value;
			}
		}
		/// <summary>
		/// Gets or sets alignment of line numbers.
		/// </summary>
		[Browsable( true )]
		[Category( "Appearance" )]
		[Description( "Specifies alignment of line numbers." )]
		[DefaultValue( typeof( LineNumberAlignment ), "Left" )]
		public LineNumberAlignment LineNumbersAlignment
		{
			get
			{
				return edtCode.LineNumbersAlignment;
			}
			set
			{
				edtCode.LineNumbersAlignment = value;
			}
		}
		/// <summary>
		/// Gets or sets placement of user margin.
		/// </summary>
		[Browsable( true )]
		[Category( "Appearance" )]
		[Description( "Specifies placement of user margin." )]
		[DefaultValue( MarginPlacement.Right )]
		public MarginPlacement UserMarginPlacement
		{
			get
			{
				return edtCode.UserMarginPlacement;
			}
			set
			{
				edtCode.UserMarginPlacement = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether click on line numbers performs selection.
		/// </summary>
		[Browsable( true )]
		[Category( "Advanced" )]
		[Description( "Specifies whether click on line numbers performs selection." )]
		[DefaultValue( true )]
		public bool SelectOnLineNumberClick
		{
			get
			{
				return edtCode.SelectOnLineNumbersClick;
			}
			set
			{
				edtCode.SelectOnLineNumbersClick = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether native GDI should be used for text output.
		/// </summary>
		[Browsable( false )]
		[DefaultValue( false )]
		public bool UseNativeDrawing
		{
			get
			{
				return edtCode.UseNativeDrawing;
			}
			set
			{
				edtCode.UseNativeDrawing = value;
			}
		}
		/// <summary>
		/// Gets or sets space between lines in pixels.
		/// </summary>
		[Browsable( true )]
		[DefaultValue( 1 )]
		[Category( "Appearance" )]
		[Description( "Specifies space between lines." )]
		public int SpaceBetweenLines
		{
			get
			{
				return edtCode.SpaceBetweenLines;
			}
			set
			{
				edtCode.SpaceBetweenLines = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether text should be selected after drag/drop operation.
		/// </summary>
		[Browsable( true )]
		[Category( "Behavior" )]
		[Description( "Specifies whether text should be selected after drag/drop operation." )]
		[DefaultValue( true )]
		public bool SelectTextAfterDragDrop
		{
			get
			{
				return edtCode.SelectTextAfterDragDrop;
			}
			set
			{
				edtCode.SelectTextAfterDragDrop = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether line selection should be extended to far right.
		/// </summary>
		[Browsable( true )]
		[Category( "Behavior" )]
		[Description( "Specifies whether line selection should be extended to far right." )]
		[DefaultValue( true )]
		public bool ExtendSelectionToFarRight
		{
			get
			{
				return edtCode.ExtendSelectionToFarRight;
			}
			set
			{
				edtCode.ExtendSelectionToFarRight = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether tab stops should be respected on inserting blocks of text.
		/// </summary>
		[Browsable( true )]
		[Category( "Advanced" )]
		[Description( "Specifies whether tab stops should be respected on inserting blocks of text." )]
		[DefaultValue( false )]
		public bool RespectTabStopsOnInsertingText
		{
			get
			{
				return edtCode.RespectTabStopsOnInsertingText;
			}
			set
			{
				edtCode.RespectTabStopsOnInsertingText = value;
			}
		}
		/// <summary>
		/// Indicates if the edit control should handle Escape key to close the parent form
		/// Default value is true. Setting it to false will close the parent form.
		/// </summary>
		[Browsable( true )]
		[Category( "Advanced" )]
		[Description( "Specifies whether Escape key should be used by the control." )]
		[DefaultValue( true )]
		public bool AcceptsEscape
		{
			get
			{
				return m_bAcceptsEscape;
			}
			set
			{
				m_bAcceptsEscape = value;
			}
		}              
		/// <summary>
		/// Gets or sets scroll position of edit control.
		/// </summary>
		[Browsable( false )]
		public Point ScrollPosition
		{
			get
			{
				Point autoScrollPos = edtCode.AutoScrollPosition;
				return new Point( -autoScrollPos.X, -autoScrollPos.Y );
			}
			set
			{
				edtCode.AutoScrollPosition = value;
			}
		}
		#endregion

		#region Nonpublic Properties
		/// <summary>
		/// Gets value indicating whether events handling is locked.
		/// </summary>
		private bool EventsHandlingLocked
		{
			get
			{
				return m_iLockEvents > 0;
			}
		}
		/// <summary>
		/// For private usage only. Implemented for applying ContextOptions attribute.
		/// </summary>
		[ContextOptions]
		private bool ShowSatusBar
		{
			get
			{
				return this.StatusBarSettings.Visible;
			}
			set
			{
				this.StatusBarSettings.Visible = value;
			}
		}
		/// <summary>
		/// Hides Background image property.
		/// </summary>
		[Description( "Specifies wheteher Background image property is hide." )]
		private new Image BackgroundImage
		{
			get
			{
				return base.BackgroundImage;
			}
		}
		/// <summary>
		/// Hides Font image property.
		/// </summary>
		[Description( "Specifies wheteher Background Font image property is hide." )]
		private new Font Font
		{
			get
			{
				return base.Font;
			}
		}
		#endregion

		#region Initialization & Finalization
		/// <summary>
		/// Creates and initializes edit control.
		/// </summary>
		[FileIOPermission( SecurityAction.Assert, Unrestricted = true )]
		[UIPermission( SecurityAction.Assert, Unrestricted = true, Window = UIPermissionWindow.AllWindows )]
		[SecurityPermission( SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode )]
		public EditControl()
		{
			SetStyle( ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true );

			InitializeComponent();

			m_scrollersFrame = new EditScrollersFrame();
			m_scrollersFrameForFake1 = new EditScrollersFrame();
			m_scrollersFrameForFake2 = new EditScrollersFrame();
			m_scrollersFrameForFake3 = new EditScrollersFrame();
			UpdateScrollersVisualStyle();

			m_scrollersFrame.HorizontalScroller.SmallChange = edtCode.HScrollBar.SmallChange;
			m_scrollersFrame.VerticalScroller.SmallChange = edtCode.VScrollBar.SmallChange;

			m_scrollersFrame.AttachedTo = edtCode;
			m_scrollersFrameForFake1.AttachedTo = fakeEditControl1;
			m_scrollersFrameForFake1.SizeGripperVisibility = SizeGripperVisibility.Hidden;
			m_scrollersFrameForFake2.AttachedTo = fakeEditControl2;
			m_scrollersFrameForFake2.SizeGripperVisibility = SizeGripperVisibility.Hidden;
			m_scrollersFrameForFake3.AttachedTo = fakeEditControl3;
			m_scrollersFrameForFake3.SizeGripperVisibility = SizeGripperVisibility.Hidden;

			AttachToEvents();
			edtCode.InvalidateAll();
            edtCode.ContextMenuOptionsForm = new ControlOptions(this);
            edtCode.contextMenuOptionsFormEditControl = this;

			m_timerUpdateWordWrap = new Timer();
			m_timerUpdateWordWrap.Tick += new EventHandler( MeasureTimerTick );
			m_timerUpdateWordWrap.Start();

			ReAlignAll();
			ExchangeWithFake( fakeEditControl3 );

			m_statusBarSettings = new StatusBarSettings( statusBar );
			m_statusBarSettings.CheckSmartGripVisibility += new GetBoolEventHandler( m_statusBarSettings_CheckSmartGripVisibility );
			m_statusBarSettings.VisibilityChanged += new ValueChangedEventHandler( m_statusBarSettings_VisibilityChanged );

			StatusBarSettings.FileNamePanel.Panel.Text = Path.GetFileName( edtCode.DisplayFileName );
		}
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				m_brushHorSplitter = null;
				m_brushVertSplitter = null;
                if(edtCode != null)
				    edtCode.Dispose();
				edtCode = null;

				if( components != null )
					components.Dispose();

				if( m_timerUpdateWordWrap != null )
				{
					m_timerUpdateWordWrap.Dispose();
					m_timerUpdateWordWrap = null;
				}

				if( m_bClearStaticDataOnDispose )
				{
					Config.ClearStaticData();
				}

				if( m_scrollersFrame != null )
				{
					m_scrollersFrame.DetachFrame();
					m_scrollersFrame.Dispose();
				}
				if( m_scrollersFrameForFake1 != null )
				{
					m_scrollersFrameForFake1.DetachFrame();
					m_scrollersFrameForFake1.Dispose();
				}
				if( m_scrollersFrameForFake2 != null )
				{
					m_scrollersFrameForFake2.DetachFrame();
					m_scrollersFrameForFake2.Dispose();
				}
				if( m_scrollersFrameForFake3 != null )
				{
					m_scrollersFrameForFake3.DetachFrame();
					m_scrollersFrameForFake3.Dispose();
				}
			}

			GC.GetTotalMemory( true );

			base.Dispose( disposing );
		}
		#endregion

		#region Component Designer Generated Code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.edtCode = new Syncfusion.Windows.Forms.Edit.FileEditControl();
			this.fakeEditControl1 = new Syncfusion.Windows.Forms.Edit.FakeEditControl();
			this.pnlCommon = new EditorPanel();
			this.pnlClient = new EditorPanel();
			this.pnlTopRight = new EditorPanel();
			this.fakeEditControl3 = new Syncfusion.Windows.Forms.Edit.FakeEditControl();
			this.splitterBottom = new System.Windows.Forms.Splitter();
			this.pnlTopClient = new EditorPanel();
			this.splitterCenter = new Splitter();
			this.pnlTop = new EditorPanel();
			this.pnlClientRight = new EditorPanel();
			this.fakeEditControl2 = new Syncfusion.Windows.Forms.Edit.FakeEditControl();
			this.splitterTop = new System.Windows.Forms.Splitter();
			this.pnlClientClient = new EditorPanel();
			this.statusBar = new Syncfusion.Windows.Forms.Tools.Controls.StatusBar.StatusBarExt();
			( ( System.ComponentModel.ISupportInitialize )( this.edtCode ) ).BeginInit();
			this.pnlCommon.SuspendLayout();
			this.pnlClient.SuspendLayout();
			this.pnlTopRight.SuspendLayout();
			this.pnlTopClient.SuspendLayout();
			this.pnlTop.SuspendLayout();
			this.pnlClientRight.SuspendLayout();
			this.pnlClientClient.SuspendLayout();
			this.SuspendLayout();
			// 
			// edtCode
			// 
			this.edtCode.AfterTextAreaBrush = new Syncfusion.Drawing.BrushInfo( System.Drawing.Color.BlanchedAlmond );
			this.edtCode.AutoCompleteSingleLexem = false;
			this.edtCode.AutoIndentGuideline = true;
			this.edtCode.AutoIndentMode = Syncfusion.Windows.Forms.Edit.Enums.AutoIndentMode.None;
			this.edtCode.AutoScrollPosition = new System.Drawing.Point( 0, 0 );
			this.edtCode.BackColor = System.Drawing.SystemColors.Window;
			this.edtCode.ColumnGuideItems = new Syncfusion.Windows.Forms.Edit.Utils.ColumnGuideItem[ 0 ];
			this.edtCode.ColumnGuidesMeasuringFont = new System.Drawing.Font( "Courier New", 10F );
			this.edtCode.ContextPromptBackgroundBrush = new Syncfusion.Drawing.BrushInfo( System.Drawing.Color.LemonChiffon );
			this.edtCode.ContextPromptBorderColor = System.Drawing.Color.Black;
			this.edtCode.ContextPromptSize = new System.Drawing.Size( 400, 50 );
			this.edtCode.ContextTooltipBackgroundBrush = new Syncfusion.Drawing.BrushInfo( System.Drawing.Color.LemonChiffon );
			this.edtCode.ContextTooltipBorderColor = System.Drawing.Color.Black;

			this.edtCode.CustomLineWrappingMarkingImage = null;

			this.edtCode.FastScrollingStep = 10;

			this.edtCode.GraphicsCompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
			this.edtCode.GraphicsInterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
			this.edtCode.GraphicsSmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
			this.edtCode.GraphicsTextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
			this.edtCode.IndentationBlockBorderColor = System.Drawing.Color.Gray;
			this.edtCode.IndentationBlockBorderStyle = Syncfusion.Windows.Forms.Edit.Enums.FrameBorderStyle.Solid;
			this.edtCode.IndentBlockHighlightingColor = System.Drawing.Color.LightBlue;
			this.edtCode.IndentLineColor = System.Drawing.Color.Gray;
            this.edtCode.SelectionTextColor = System.Drawing.Color.Blue;
			this.edtCode.InsertDroppedFileIntoText = false;
			this.edtCode.LineNumbersColor = System.Drawing.Color.DarkBlue;
			this.edtCode.LineNumbersFont = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( ( System.Byte )( 204 ) ) );
			this.edtCode.MarkerAreaWidth = 16;
			this.edtCode.MarkLineWrapping = true;

			this.edtCode.Name = "edtCode";

			this.edtCode.OnlyHighlightMatchingBraces = false;

			this.edtCode.PrintHeaderAndFooter = true;

			this.edtCode.SelectionMarginBackgroundColor = System.Drawing.Color.Empty;
			this.edtCode.SelectionMarginForegroundColor = System.Drawing.Color.Red;
			this.edtCode.SelectionMarginWidth = 5;


			this.edtCode.ShowContentDividers = true;
			this.edtCode.ShowContextTooltip = true;
			this.edtCode.ShowIndentationBlockBorders = false;
			this.edtCode.ShowIndentationGuidelines = true;
			this.edtCode.ShowLineNumbers = true;
			this.edtCode.ShowMarkers = true;
			this.edtCode.ShowOutliningCollapsers = true;
			this.edtCode.ShowOutliningTooltip = true;
			this.edtCode.ShowSelectionMargin = true;
			this.edtCode.ShowTextArea = false;
			this.edtCode.ShowWhitespaces = false;
			this.edtCode.WrapAroundSearch = true;
			this.edtCode.TabIndex = 0;
			this.edtCode.TabSize = 2;
			this.edtCode.TabStopsArray = new int[] {
																							 8,
																							 16,
																							 24,
																							 32,
																							 40};
			this.edtCode.Text = "edtCode";
            this.edtCode.SelectAll();
			this.edtCode.TextAreaLineColor = System.Drawing.Color.Black;
			this.edtCode.TextAreaLineStyle = System.Drawing.Drawing2D.DashStyle.Dot;
			this.edtCode.TextAreaWidth = 600;
			this.edtCode.UseCustomSizeContextPrompt = false;
			this.edtCode.UserMarginBackgroundColor = new Syncfusion.Drawing.BrushInfo( System.Drawing.Color.BurlyWood );
			this.edtCode.UserMarginBorderColor = System.Drawing.Color.Black;
			this.edtCode.UserMarginTextColor = System.Drawing.Color.Black;
			this.edtCode.UserMarginTextFont = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( ( System.Byte )( 204 ) ) );
			this.edtCode.UseTabStops = false;
			this.edtCode.UseXPStyle = true;

			this.edtCode.VirtualSpaceMode = false;
			this.edtCode.WrapMode = Syncfusion.Windows.Forms.Edit.Enums.WordWrapMode.Control;
			this.edtCode.WrappedLinesOffset = 0;
			this.edtCode.WrapType = Syncfusion.Windows.Forms.Edit.Enums.WordWrapType.WrapByWord;
			// 
			// fakeEditControl1
			// 

			this.fakeEditControl1.Control = this.edtCode;



			this.fakeEditControl1.Location = new System.Drawing.Point( 0, 0 );
			this.fakeEditControl1.Name = "fakeEditControl1";




			this.fakeEditControl1.Size = new System.Drawing.Size( 20, 20 );
			this.fakeEditControl1.TabIndex = 25;


			// 
			// pnlCommon
			// 
			this.pnlCommon.BackColor = System.Drawing.SystemColors.Window;
			this.pnlCommon.Controls.Add( this.pnlClient );
			this.pnlCommon.Controls.Add( this.splitterCenter );
			this.pnlCommon.Controls.Add( this.pnlTop );
			this.pnlCommon.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlCommon.Location = new System.Drawing.Point( 0, 0 );
			this.pnlCommon.Name = "pnlCommon";
			this.pnlCommon.Size = new System.Drawing.Size( 528, 336 );
			this.pnlCommon.TabIndex = 26;
			// 
			// pnlClient
			// 
			this.pnlClient.BackColor = System.Drawing.SystemColors.Window;
			this.pnlClient.Controls.Add( this.pnlTopRight );
			this.pnlClient.Controls.Add( this.splitterBottom );
			this.pnlClient.Controls.Add( this.pnlTopClient );
			this.pnlClient.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlClient.Location = new System.Drawing.Point( 0, 5 );
			this.pnlClient.Name = "pnlClient";
			this.pnlClient.Size = new System.Drawing.Size( 528, 331 );
			this.pnlClient.TabIndex = 0;
			// 
			// pnlTopRight
			// 
			this.pnlTopRight.Controls.Add( this.fakeEditControl3 );
			this.pnlTopRight.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlTopRight.Location = new System.Drawing.Point( 6, 0 );
			this.pnlTopRight.Name = "pnlTopRight";
			this.pnlTopRight.Size = new System.Drawing.Size( 522, 331 );
			this.pnlTopRight.TabIndex = 1;
			// 
			// fakeEditControl3
			// 

			this.fakeEditControl3.Control = this.edtCode;

			this.fakeEditControl3.Location = new System.Drawing.Point( 0, 0 );
			this.fakeEditControl3.Name = "fakeEditControl3";

			this.fakeEditControl3.TabIndex = 0;

			// 
			// splitterBottom
			// 
			this.splitterBottom.BackColor = System.Drawing.SystemColors.ControlLight;
			this.splitterBottom.Location = new System.Drawing.Point( 0, 0 );
			this.splitterBottom.MinSize = 0;
			this.splitterBottom.Name = "splitterBottom";
			this.splitterBottom.Size = new System.Drawing.Size( 6, 331 );
			this.splitterBottom.TabIndex = 2;
			this.splitterBottom.TabStop = false;
			this.splitterBottom.SplitterMoved += new System.Windows.Forms.SplitterEventHandler( this.VerticalSplitterMoved );
			this.splitterBottom.Paint += new System.Windows.Forms.PaintEventHandler( this.splitterCenter_Paint );
			this.splitterBottom.DoubleClick += new System.EventHandler( this.splitterVertical_DoubleClick );
			this.splitterBottom.SplitterMoving += new System.Windows.Forms.SplitterEventHandler( this.VerticalSplitterMoving );
			// 
			// pnlTopClient
			// 
			this.pnlTopClient.Controls.Add( this.fakeEditControl1 );
			this.pnlTopClient.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlTopClient.Location = new System.Drawing.Point( 0, 0 );
			this.pnlTopClient.Name = "pnlTopClient";
			this.pnlTopClient.Size = new System.Drawing.Size( 0, 331 );
			this.pnlTopClient.TabIndex = 0;
			// 
			// splitterCenter
			// 
			this.splitterCenter.BackColor = System.Drawing.SystemColors.ControlLight;
			this.splitterCenter.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitterCenter.Location = new System.Drawing.Point( 0, 0 );
			this.splitterCenter.MinSize = 0;
			this.splitterCenter.Name = "splitterCenter";
			this.splitterCenter.Size = new System.Drawing.Size( 528, 5 );
			this.splitterCenter.TabIndex = 2;
			this.splitterCenter.TabStop = false;
			this.splitterCenter.SplitterMoved += new System.Windows.Forms.SplitterEventHandler( this.VerticalSplitterMoved );
			this.splitterCenter.Paint += new System.Windows.Forms.PaintEventHandler( this.splitterCenter_Paint );
			this.splitterCenter.DoubleClick += new System.EventHandler( this.splitterCenter_DoubleClick );
			this.splitterCenter.SplitterMoving += new System.Windows.Forms.SplitterEventHandler( this.splitterCenter_SplitterMoving );
			// 
			// pnlTop
			// 
			this.pnlTop.Controls.Add( this.pnlClientRight );
			this.pnlTop.Controls.Add( this.splitterTop );
			this.pnlTop.Controls.Add( this.pnlClientClient );
			this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlTop.Location = new System.Drawing.Point( 0, 0 );
			this.pnlTop.Name = "pnlTop";
			this.pnlTop.Size = new System.Drawing.Size( 528, 0 );
			this.pnlTop.TabIndex = 1;
			// 
			// pnlClientRight
			// 
			this.pnlClientRight.BackColor = System.Drawing.SystemColors.Window;
			this.pnlClientRight.Controls.Add( this.fakeEditControl2 );
			this.pnlClientRight.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlClientRight.ForeColor = System.Drawing.SystemColors.ActiveBorder;
			this.pnlClientRight.Location = new System.Drawing.Point( 6, 0 );
			this.pnlClientRight.Name = "pnlClientRight";
			this.pnlClientRight.Size = new System.Drawing.Size( 522, 0 );
			this.pnlClientRight.TabIndex = 1;
			// 
			// fakeEditControl2
			// 

			this.fakeEditControl2.Control = this.edtCode;



			this.fakeEditControl2.Location = new System.Drawing.Point( 0, 0 );
			this.fakeEditControl2.Name = "fakeEditControl2";




			this.fakeEditControl2.Size = new System.Drawing.Size( 20, 20 );
			this.fakeEditControl2.TabIndex = 0;


			// 
			// splitterTop
			// 
			this.splitterTop.BackColor = System.Drawing.SystemColors.ControlLight;
			this.splitterTop.Location = new System.Drawing.Point( 0, 0 );
			this.splitterTop.MinSize = 0;
			this.splitterTop.Name = "splitterTop";
			this.splitterTop.Size = new System.Drawing.Size( 6, 0 );
			this.splitterTop.TabIndex = 2;
			this.splitterTop.TabStop = false;
			this.splitterTop.SplitterMoved += new System.Windows.Forms.SplitterEventHandler( this.VerticalSplitterMoved );
			this.splitterTop.Paint += new System.Windows.Forms.PaintEventHandler( this.splitterCenter_Paint );
			this.splitterTop.DoubleClick += new System.EventHandler( this.splitterVertical_DoubleClick );
			this.splitterTop.SplitterMoving += new System.Windows.Forms.SplitterEventHandler( this.VerticalSplitterMoving );
			// 
			// pnlClientClient
			// 
			this.pnlClientClient.BackColor = System.Drawing.SystemColors.Window;
			this.pnlClientClient.Controls.Add( this.edtCode );
			this.pnlClientClient.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlClientClient.ForeColor = System.Drawing.SystemColors.ActiveBorder;
			this.pnlClientClient.Location = new System.Drawing.Point( 0, 0 );
			this.pnlClientClient.Name = "pnlClientClient";
			this.pnlClientClient.Size = new System.Drawing.Size( 0, 0 );
			this.pnlClientClient.TabIndex = 0;
			// 
			// statusBar
			// 
			this.statusBar.Location = new System.Drawing.Point( 0, 336 );
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size( 528, 24 );
			this.statusBar.TabIndex = 27;
			this.statusBar.Visible = false;
			this.statusBar.VisibleChanged += new System.EventHandler( this.statusBar_VisibleChanged );
			// 
			// EditControl
			// 
			this.Controls.Add( this.pnlCommon );
			this.Controls.Add( this.statusBar );
			this.Name = "EditControl";
			this.Size = new System.Drawing.Size( 528, 360 );
			( ( System.ComponentModel.ISupportInitialize )( this.edtCode ) ).EndInit();
			this.pnlCommon.ResumeLayout( false );
			this.pnlClient.ResumeLayout( false );
			this.pnlTopRight.ResumeLayout( false );
			this.pnlTopClient.ResumeLayout( false );
			this.pnlTop.ResumeLayout( false );
			this.pnlClientRight.ResumeLayout( false );
			this.pnlClientClient.ResumeLayout( false );
			this.ResumeLayout( false );
		}
		#endregion

		#region Events
		/// <summary>
		/// Event that is raised when InsertMode flag has changed
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "Occurs when InsertMode property value has been changed." )]
		public event EventHandler InsertModeChanged;
		/// <summary>
		/// Event that is raised when current cursor position has changed.
		/// </summary>
		[Category( "Behavior" )]
		[Description( "Occurs when cursors position has been changed." )]
		public event EventHandler CursorPositionChanged;
		/// <summary>
		/// Event that is raised when text selection has been changed.
		/// </summary>
		[Category( "Behavior" )]
		[Description( "Occurs when selection has been changed." )]
		public event EventHandler SelectionChanged;
		/// <summary>
		/// Event that is raised when Changed State was changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "Occurs when CanUndoRedo state is has been changed." )]
		public event EventHandler CanUndoRedoChanged;
		/// <summary>
		/// Event that is raised when user margin has to be painted.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Occurs when user margin should be drawn." )]
		public event PaintEventHandler PaintUserMargin;
		/// <summary>
		/// Event that is raised on the start of the long operation.
		/// </summary>
		[Category( "Behavior" )]
		[Description( "Occurs when some operation starts." )]
		public event LongOperationEventHandler OperationStarted;
		/// <summary>
		/// Event that is raised on the end of the long operation.
		/// </summary>
		[Category( "Behavior" )]
		[Description( "Occurs when operation ends." )]
		public event LongOperationEventHandler OperationStopped;
		/// <summary>
		/// Event that is raised when current stream instance is to be changed to some other one.
		/// </summary>
		[Category( "Data" )]
		[Description( "Occurs when underlying stream of the control is about to change." )]
		[Obsolete( "Later this event will not be exposed to the user.", false )]
		public virtual event ChangingStreamEventHandler ChangingStream;
		/// <summary>
		/// Event that is raised when ReadOnly mode changes.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "Occurs when ReadOnly property value has been changed. " )]
		public event EventHandler ReadOnlyChanged;
		/// <summary>
		/// Event that is raised when class registers default key bindings.
		/// </summary>
		[Category( "Behavior" )]
		[Description( "Occurs when default key bindings should be added." )]
		public event EventHandler RegisteringDefaultKeyBindings;
		/// <summary>
		/// Event that is raised when class registers commands for key-binding.
		/// </summary>
		[Category( "Behavior" )]
		[Description( "Occurs when custom key-binding command should be registered." )]
		public event EventHandler RegisteringKeyCommands;
		/// <summary>
		/// Event that is raised after changing configuration.
		/// </summary>
		[Category( "Behavior" )]
		[Description( "Occurs when configuration has been changed." )]
		public event EventHandler ConfigurationChanged;
		/// <summary>
		/// Event that is raised after changing parsers language.
		/// </summary>
		[Category( "Behavior" )]
		[Description( "Occurs when current language of the control has been changed." )]
		public event EventHandler LanguageChanged;
		/// <summary>
		/// Event that is raised when line mark should be drawn.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Occurs when custom line mark should be drawn." )]
		public event DrawLineMarkEventHandler DrawLineMark;
		/// <summary>
		/// Event that is raised before the line number is drawn.
		/// </summary>
		public event OnBeforeLineNumberPaintEventHandler BeforeLineNumberPaint;
		/// <summary>
		/// Event that is raised before the ContextChoice dialog is shown to user.
		/// </summary>
		[Category( "Intellisense" )]
		[Description( "Occurs when context choice window is about to open. User can cancel context choice window." )]
		public event CancelEventHandler ContextChoiceBeforeOpen;
		/// <summary>
		/// Event that is raised when auto-complete dialog should be updated.
		/// </summary>
		[Category( "Intellisense" )]
		[Description( "Occurs when context choice list should be updated." )]
		public event ContextChoiceEventHandler ContextChoiceUpdate;
		/// <summary>
		/// Event that is raised when auto-complete dialog has been opened.
		/// </summary>
		[Category( "Intellisense" )]
		[Description( "Occurs when context choice window has been opened." )]
		public event ContextChoiceEventHandler ContextChoiceOpen;
		/// <summary>
		/// Event that is raised when auto-complete dialog has been closed.
		/// </summary>
		[Category( "Intellisense" )]
		[Description( "Occurs when context choice window has been closed." )]
		public event ContextChoiceCloseEventHandler ContextChoiceClose
		{
			add
			{
				edtCode.ContextChoice.ContextChoiceClose += value;
			}
			remove
			{
				edtCode.ContextChoice.ContextChoiceClose -= value;
			}
		}
		/// <summary>
		/// Event that is raised before context prompt should be shown.
		/// </summary>
		[Category( "Intellisense" )]
		[Description( "Occurs when context prompt window is about to open. User can cancel it." )]
		public event CancelEventHandler ContextPromptBeforeOpen;
		/// <summary>
		/// Event that is raised when context prompt is to be opened.
		/// </summary>
		[Category( "Intellisense" )]
		[Description( "Occurs when context prompt has been opened." )]
		public event ContextPromptUpdateEventHandler ContextPromptOpen;
		/// <summary>
		/// Event that is raised when context prompt is updated.
		/// </summary>
		[Category( "Intellisense" )]
		[Description( "Occurs user has moved cursor and context prompt should be updated." )]
		public event ContextPromptUpdateEventHandler ContextPromptUpdate;
		/// <summary>
		/// Event that is raised when context prompt is closed.
		/// </summary>
		[Category( "Intellisense" )]
		[Description( "Occurs when context prompt window has been closed." )]
		public event ContextPromptCloseEventHandler ContextPromptClose;
		/// <summary>
		/// Event that is raised when context tooltip text should be updated.
		/// </summary>
		[Category( "Intellisense" )]
		[Description( "Occurs when context tooltip text should be updated." )]
		public event UpdateTooltipEventHandler UpdateContextToolTip;
		/// <summary>
		/// Event that is raised when bookmark tooltip text should be updated.
		/// </summary>
		[Category( "Intellisense" )]
		[Description( "Occurs when bookmark tooltip text should be updated." )]
		public event UpdateBookmarkTooltipEventHandler UpdateBookmarkToolTip;
		/// <summary>
		/// Event, that is raised, when user should fill menu with menu items.
		/// </summary>
		[Category( "Behavior" )]
		[Description( "Occurs when context menu is about to open. User can handle this event to add custom menu items." )]
		public event EventHandler MenuFill;
		/// <summary>
		/// Raised, when single line mode has been changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "Occurs when SingleLineMode property value has been changed." )]
		public event EventHandler SingleLineChanged;
		/// <summary>
		/// Event, that is raised when text has been changed.
		/// </summary>
		[Category( "Data" )]
		[Description( "Occurs when text has been changed." )]
		new public event EventHandler TextChanged;
        /// <summary>
        /// Event, that is raised once new match is found in FindAndReplaceDialogBox via Find Next Button.
        /// </summary>
        public event EventHandler Find;
		/// <summary>
		/// Event, that is raised when text is to be changed.
		/// </summary>
		[Category( "Data" )]
		[Description( "Occurs when text is about to change." )]
		public event TextChangingEventHandler TextChanging;
        /// <summary>
        /// Event, that is raised when Line is changed
        /// </summary>
        [Category("Data")]
        [Description("Occurs when line changed.")]
        public event TextChangedEventHandler LineChanged;
        /// <summary>
        /// Event, that is raised when Line is inserted
        /// </summary>
        [Category("Data")]
        [Description("Occurs when line inserted.")]
        public event LineInsertedEventHandler LineInserted;
        /// <summary>
        /// Event, that is raised when Line is deleted
        /// </summary>
        [Category("Data")]
        [Description("Occurs when line Deleted.")]
        public event LineDeletedEventHandler LineDeleted;
		/// <summary>
		/// Event, that is raised when some context choice list item gets selected.
		/// </summary>
		[Category( "Intellisense" )]
		[Description( "Occurs when context choice item has been selected." )]
		public event ContextChoiceItemSelectedEventHandler ContextChoiceItemSelected;
		/// <summary>
		/// Event, that is raised when context prompt selection has been changed.
		/// </summary>
		[Category( "Intellisense" )]
		[Description( "Occurs when context prompt item has been selected." )]
		public event ContextPromptSelectionChangedEventHandler ContextPromptSelectionChanged;
		/// <summary>
		/// Event, that is raised when word-wrap mode has been changed.
		/// </summary>
		[Category( "Property Changed" )]
		[Description( "Occurs when WordWrap property value has been changed." )]
		public event EventHandler WordWrapChanged;
		/// <summary>
		/// Event that is raised when editor is about to insert text of the selected context choice item.
		/// </summary>
		[Category( "Intellisense" )]
		[Description( "Occurs when editor is about to insert selected context choice item to the text. Action can be canceled." )]
		public event ContextChoiceTextInsertEventHandler ContextChoiceSelectedTextInsert;
		/// <summary>
		/// Event that is raised when context choice item is right clicked.
		/// </summary>
		[Category( "Intellisense" )]
		[Description( "Occurs when context choice item is right clicked." )]
		public event ContextChoiceItemEventHandler ContextChoiceRightClick;
		/// <summary>
		/// Event that is raised when page header should be printed.
		/// </summary>
		[Category( "Printing" )]
		[Description( "Occurs when document is printed and page header needs to be printed." )]
		public event PrintHeadlineEventHandler PrintHeader;
		/// <summary>
		/// Event that is raised when page footer should be printed.
		/// </summary>
		[Category( "Printing" )]
		[Description( "Occurs when document is printed and page footer needs to be printed." )]
		public event PrintHeadlineEventHandler PrintFooter;
		/// <summary>
		/// Event that is raised when user clicks in the indicator margin area.
		/// </summary>
		[Category( "Action" )]
		[Description( "Occurs when users single clicks on the indicator margin." )]
		public event IndicatorClickEventHandler IndicatorMarginClick;
		/// <summary>
		/// Event that is raised when user double-clicks in the indicator margin area.
		/// </summary>
		[Category( "Action" )]
		[Description( "Occurs when user double clicks on the indicator margin." )]
		public event IndicatorClickEventHandler IndicatorMarginDoubleClick;
		/// <summary>
		/// Event that is raised when user margin area text is ready to be drawn.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Occurs when user margin area text is ready to be drawn." )]
		public event DrawUserMarginTextEventHandler DrawUserMarginText;
		/// <summary>
		/// Event that is raised when user tries to save stream with data loosing.
		/// </summary>
		[Category( "Behavior" )]
		[Description( "Occurs when user tries to save stream with data loosing." )]
		public event SaveWithDataLosingEventHandler SaveStreamWithDataLoss;
		/// <summary>
		/// Event that is raised when user tries to save file with data loosing.
		/// </summary>
		[Category( "Behavior" )]
		[Description( "Occurs when user tries to save file with data loosing." )]
		public event SaveWithDataLosingEventHandler SaveFileWithDataLoss;
		/// <summary>
		/// Event that is raised when underlying stream is about to close and user should decide if he wants to save the changes in file.
		/// </summary>
		[Category( "Data" )]
		[Description( "Occurs when edited stream is about to be closed and changes should be saved." )]
		public event StreamCloseEventHandler Closing;
		/// <summary>
		/// Event that is raised when outlining tooltip is about to be shown.
		/// </summary>
		[Category( "Outlining" )]
		[Description( "Occurs when outlining tooltip is about to be shown." )]
		public event OutliningTooltipBeforePopupEventHandler OutliningTooltipBeforePopup;
		/// <summary>
		/// Event that is raised when outlining tooltip is shown.
		/// </summary>
		[Category( "Outlining" )]
		[Description( "Occurs when outlining tooltip is shown." )]
		public event CollapsedRegionRelatedEventHandler OutliningTooltipPopup;
		/// <summary>
		/// Event that is raised when outlining tooltip is closed.
		/// </summary>
		[Category( "Outlining" )]
		[Description( "Occurs when outlining tooltip is closed." )]
		public event CollapsedRegionRelatedEventHandler OutliningTooltipClose;
		/// <summary>
		/// Event that is raised before region is about to expand.
		/// </summary>
		[Category( "Outlining" )]
		[Description( "Occurs before region is about to expand." )]
		public event OutliningCancellableEventHandler OutliningBeforeExpand;
		/// <summary>
		/// Event that is raised when region expands.
		/// </summary>
		[Category( "Outlining" )]
		[Description( "Occurs when region expands." )]
		public event OutliningEventHandler OutliningExpand;
		/// <summary>
		/// Event that is raised before region is about to collapse.
		/// </summary>
		[Category( "Outlining" )]
		[Description( "Occurs before region is about to collapse." )]
		public event OutliningCancellableEventHandler OutliningBeforeCollapse;
		/// <summary>
		/// Event that is raised when region collapses.
		/// </summary>
		[Category( "Outlining" )]
		[Description( "Occurs when region collapses." )]
		public event OutliningEventHandler OutliningCollapse;
		/// <summary>
		/// Raised when text in hidden block is found and this block can't be expanded due to user's cancelling.
		/// </summary>
		[Category( "Behavior" )]
		[Description( "Occurs when text in hidden block is found and this block can't be expanded due to user's cancelling." )]
		public event UnreachableTextFoundEventHandler UnreachableTextFound
		{
			add
			{
				edtCode.UnreachableTextFound += value;
			}
			remove
			{
				edtCode.UnreachableTextFound -= value;
			}
		}
		/// <summary>
		/// Raised when new code snippet member has to be highlighted.
		/// </summary>
		[Category( "CodeSnippets" )]
		[Description( "Occurs when new snippet member has to be highlighted." )]
		public event NewSnippetMemberHighlightingEventHandler NewSnippetMemberHighlighting;
		/// <summary>
		/// Raised when text of code snippet template member is to be changed.
		/// </summary>
		[Category( "CodeSnippets" )]
		[Description( "Occurs when text of template member is to be changed." )]
		public event CodeSnippetTemplateTextChangingEventHandler CodeSnippetTemplateTextChanging;
		/// <summary>
		/// Raised when code snippet is to be activated.
		/// </summary>
		[Category( "CodeSnippets" )]
		[Description( "Occurs when code snippet is to be activated." )]
		public event CancellableCodeSnippetsEventHandler CodeSnippetActivating;
		/// <summary>
		/// Raised when code snippet is to be deactivated.
		/// </summary>
		[Category( "CodeSnippets" )]
		[Description( "Occurs when code snippet is to be deactivated." )]
		public event CodeSnippetsEventHandler CodeSnippetDeactivating;
		/// <summary>
		/// Raised when CollapseAll method is called.
		/// </summary>
        [Description(" Raised when CollapseAll method is called.")]
		public event CancelEventHandler CollapsingAll
		{
			add
			{
				edtCode.CollapsingAll += value;
			}
			remove
			{
				edtCode.CollapsingAll -= value;
			}
		}
		/// <summary>
		/// Raised when ExpandeAll method is called.
		/// </summary>
        [Description("Raised when ExpandeAll method is called.")]
		public event CancelEventHandler ExpandingAll
		{
			add
			{
				edtCode.ExpandingAll += value;
			}
			remove
			{
				edtCode.ExpandingAll -= value;
			}
		}
		/// <summary>
		/// Raised when CollapseAll method was called.
		/// </summary>
        [Description("Raised when CollapseAll method was called.")]
		public event EventHandler CollapsedAll
		{
			add
			{
				edtCode.CollapsedAll += value;
			}
			remove
			{
				edtCode.CollapsedAll -= value;
			}
		}
		/// <summary>
		/// Raised when ExpandeAll method was called.
		/// </summary>
        [Description("Raised when ExpandeAll method was called.")]
		public event EventHandler ExpandedAll
		{
			add
			{
				edtCode.ExpandedAll += value;
			}
			remove
			{
				edtCode.ExpandedAll -= value;
			}
		}
		/// <summary>
		/// Raised when horizontal scrolling takes place.
		/// </summary>
        [Description("Raised when horizontal scrolling takes place.")]
		public new event ScrollEventHandler HorizontalScroll
		{
			add
			{
				edtCode.HorizontalScroll += value;
			}
			remove
			{
				edtCode.HorizontalScroll -= value;
			}
		}
		/// <summary>
		/// Raised when vertical scrolling takes place.
		/// </summary>
        [Description("Raised when vertical scrolling takes place.")]
		public new event ScrollEventHandler VerticalScroll
		{
			add
			{
				edtCode.VerticalScroll += value;
			}
			remove
			{
				edtCode.VerticalScroll -= value;
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Applies coloring of the specified language to the text.
		/// </summary>
		/// <param name="language">Known language to apply.</param>
		public void ApplyConfiguration( KnownLanguages language )
		{
			if( KnownLanguages.Undefined == language ) throw new ArgumentException( "Undefined language can not be applied.", "language" );

			bool bLanguageFound = false;
			foreach( ConfigLanguage lang in Configurator.KnownLanguages )
			{
				if( lang.KnownLanguage == language )
				{
					bLanguageFound = true;
					ApplyConfiguration( lang );
					break;
				}
			}

			if( !bLanguageFound ) throw new ArgumentException( "The specified language can not be found.", "language" );
		}
		/// <summary>
		/// Applies coloring of the specified language to the text.
		/// </summary>
		/// <param name="languageName">Name of the language to apply.</param>
		public void ApplyConfiguration( string languageName )
		{
			ApplyConfiguration( Configurator[ languageName ] );
		}
		/// <summary>
		/// Applies coloring of the specified language to the text.
		/// </summary>
		/// <param name="lang">New language configuration to be set.</param>
		public void ApplyConfiguration( IConfigLanguage lang )
		{
			edtCode.ResetColoring( lang );
		}
		/// <summary>
		/// Reads configuration from file.
		/// </summary>
		/// <param name="fileName">Name of the file with configuration.</param>
		/// <param name="currentFilePath">Name of the file that is currently loaded, or empty string.</param>
		public void LoadConfig( string fileName, string currentFilePath )
		{
			edtCode.LoadConfig( fileName, currentFilePath );
		}
		/// <summary>
		/// Reads configuration from file.
		/// </summary>
		/// <param name="configStream">Stream to load config from.</param>
		public void LoadConfig( Stream configStream )
		{
			edtCode.LoadConfig( configStream );
		}
		/// <summary>
		/// Resets parser.
		/// </summary>
		/// <param name="lang">New language configuration to be set.</param>
		[Obsolete( @"This method is obsolete, it will be removed in the next release. 
			Please use one of the ApplyConfiguration method overloads instead." )]
		public void ResetColoring( IConfigLanguage lang )
		{
			edtCode.ResetColoring( lang );
		}
		/// <summary>
		/// Loads stream.
		/// </summary>
		/// <param name="stream">Stream to load.</param>
		/// <returns>True, if user has not canceled loading, otherwise false.</returns>
		public bool LoadStream( Stream stream )
		{
			return edtCode.LoadStream( stream );
		}
		/// <summary>
		/// Loads stream and configuration for it.
		/// </summary>
		/// <param name="stream">Stream to load.</param>
		/// <param name="lang">Configuration to apply to loaded stream.</param>
		/// <returns>True, if user has not canceled loading, otherwise false.</returns>
		public bool LoadStream( Stream stream, IConfigLanguage lang )
		{
			return edtCode.LoadStream( stream, lang );
		}
		/// <summary>
		/// Creates empty stream and makes editor to edit it.
		/// </summary>
		/// <returns>True if new empty stream was successfully created and loaded; otherwise false.</returns>
		public virtual bool New()
		{
			return edtCode.New();
		}
		/// <summary>
		/// Creates empty stream and makes editor to edit it.
		/// </summary>
		/// <param name="lang">Language configuration to be set</param>
		/// <returns>True if new empty stream was successfully created and loaded.</returns>
		public virtual bool New( IConfigLanguage lang )
		{
			return edtCode.New( lang );
		}
		/// <summary>
		/// Flushes changes to the current stream.
		/// </summary>
		public void FlushChanges()
		{
			edtCode.SaveToStream();
		}

        /// <summary>
        /// Discards all unsaved changes from current stream.
        /// </summary>
        public void DiscardChanges()
        {
            edtCode.DiscardChanges();
        }

		/// <summary>
		/// Flushes changes to the current stream.
		/// </summary>
		[Obsolete( "This method is obsolete. Please use Flushchanges() method instead." )]
		public void SaveToStream()
		{
			FlushChanges();
		}
		/// <summary>
		/// Saves data from current stream to the specified one.
		/// </summary>
		/// <param name="stream">Output stream.</param>
		public void SaveToStream( Stream stream )
		{
			edtCode.SaveToStream( stream );
		}
		/// <summary>
		/// Returns current ParsePoint.
		/// </summary>
		/// <returns>IParsePoint with current position.</returns>
		public IParsePoint GetRealCursorPosition()
		{
			return edtCode.GetRealCursorPosition();
		}
		/// <summary>
		/// Gets text of the specified line.
		/// </summary>
		/// <param name="iLineIndex">Line index.</param>
		/// <returns>Text of the line.</returns>
		public string GetLineText( int iLineIndex )
		{
			return edtCode.GetLineText( iLineIndex );
		}
		/// <summary>
		/// Gets line's instance.
		/// </summary>
		/// <param name="iLineIndex">Index of the line.</param>
		/// <returns>Instance of the line.</returns>
		public ILexemLine GetLine( int iLineIndex )
		{
			return edtCode.GetLine( iLineIndex );
		}
		/// <summary>
		/// Sets cursor to the beginning of the line with the specified number.
		/// </summary>
		/// <param name="lineNumber">Number of the line the cursor should be set to.</param>
		public void GoTo( int lineNumber )
		{
			edtCode.GoToLine( lineNumber );
		}
		/// <summary>
		/// Goes to specifies position in opened file.
		/// </summary>
		/// <param name="iLineNumber">Number of line to set cursor position to.</param>
		/// <param name="iLinesAbove">Number of lines to leave above cursor.</param>
		/// <returns>Bool indicating success.</returns>
		public bool GoTo( int iLineNumber, int iLinesAbove )
		{
			return edtCode.GoTo( iLineNumber, iLinesAbove );
		}
		/// <summary>
		/// Appends text.
		/// </summary>
		/// <param name="text">The text to be appended.</param>
		public void AppendText( string text )
		{
			edtCode.AppendText( text );
		}
		/// <summary>
		/// Inserts text in the given position.
		/// </summary>
		/// <param name="line">Line in virtual coordinates where text should be inserted.</param>
		/// <param name="column">Column in virtual coordinates where text should be inserted.</param>
		/// <param name="text">Text to be inserted.</param>
		public void InsertText( int line, int column, string text )
		{
			edtCode.InsertText( line, column, text );
		}
		/// <summary>
		/// Gets word under cursor.
		/// </summary>
		/// <returns>Lexem under cursor.</returns>
		public string GetCurrentWord()
		{
			return edtCode.GetCurrentWord();
		}
		/// <summary>
		/// Gets column where current word starts.
		/// </summary>
		/// <returns>Index of the column of the word start.</returns>
		public int GetCurrentWordColumn()
		{
			return edtCode.GetCurrentWordColumn();
		}
		/// <summary>
		/// Looks for specified expression in text.
		/// </summary>
		/// <param name="startLine">Start line for the search.</param>
		/// <param name="startColumn">Start column for the search.</param>
		/// <param name="expression">Expression to be found.</param>
		/// <param name="bSearchInCollapsed">Flag, that specifies whether text can be found in collapsed region.</param>
		/// <param name="searchUp">Value indicating whether search should be performed in bottom-up destination.</param>
		/// <returns>Search result.</returns>
		public FindResult FindRegex( int startLine, int startColumn, string expression, bool bSearchInCollapsed, bool searchUp )
		{
			CoordinatePoint pointStart = Parser.GetNearestParsePointLeft( startLine, startColumn );

			if( pointStart == null )
				throw new ApplicationException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_8 );
            try
            {
                RegexCaseInsenstiveArgs regexArgs = new RegexCaseInsenstiveArgs();
                Regex regex = null;

                OnFindRegex(regexArgs);

                if (regexArgs.IsCaseInsensitiveOn)
                {
                    regex = new Regex(expression,RegexOptions.IgnoreCase);
                }
                else
                {
                    regex = new Regex(expression);
                }

                return FindRegex(pointStart.PhysicalPoint, regex, bSearchInCollapsed, searchUp);
            }
            catch
            {
                MessageBox.Show(this, Localizer.GetString(Localizer.EditResourceIdentifiers.DEF_MSG_REGEX_INCORRECT), Localizer.GetString(Localizer.EditResourceIdentifiers.DEF_MSG_REGEX_INCORRECT_CAPTION), MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return FindResult.Empty;
            }
		}

        /// <summary>
        /// To create the search expression as ignorecase or casesenstive option
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnFindRegex(RegexCaseInsenstiveArgs e)
        {
            if (SearchRegex != null)
            {
                SearchRegex(this,e);
            }
        }
		/// <summary>
		/// Marks search result and sets cursor to the end of the selection.
		/// </summary>
		/// <param name="result">Find result that must be marked.</param>
		public void MarkSearchResult( FindResult result )
		{
			edtCode.MarkSearchResult( result );
		}
		/// <summary>
		/// Searches for given string in the text of control and returns text range of first found occurance.
		/// </summary>
		/// <param name="searchString">Text to find.</param>
		/// <param name="startLocation">Start point of search range.</param>
		/// <param name="endLocation">End point of search range.</param>
		/// <param name="matchWholeWord">Indicates whether whole word should be found.</param>
		/// <param name="searchHiddenText">Indicates whether hidden text should be searched.</param>
		/// <param name="searchUp">Indicates whether search should be performed in up direction.</param>
		/// <param name="useRegex">Indicates whether regex should be used.</param>
		/// <returns>Text range of first found occurance or null if no match was found.</returns>
		public ITextRange FindRange( string searchString, CoordinatePoint startLocation, CoordinatePoint endLocation,
			bool matchWholeWord, bool searchHiddenText, bool searchUp, bool useRegex )
		{
			return edtCode.FindRange( searchString, startLocation, endLocation, matchWholeWord, searchHiddenText, searchUp, useRegex );
		}
		/// <summary>
		/// Searches for given string in the text of control and returns text range of first found occurance.
		/// </summary>
		/// <param name="searchString">Text to find.</param>
		/// <param name="startLocation">Start point of search range.</param>
		/// <param name="endLocation">End point of search range.</param>
		/// <param name="searchUp">Indicates whether search should be performed in up direction.</param>
		/// <returns>Text range of first found occurance or null if no match was found.</returns>
		public ITextRange FindRange( string searchString, CoordinatePoint startLocation, CoordinatePoint endLocation, bool searchUp )
		{
			return edtCode.FindRange( searchString, startLocation, endLocation, false, true, searchUp, false );
		}
		/// <summary>
		/// Searches for given string in the text of control and returns text range of first found occurance.
		/// </summary>
		/// <param name="searchString">Text to find.</param>
		/// <param name="startLocation">Start point of search range.</param>
		/// <param name="searchUp">Indicates whether search should be performed in up direction.</param>
		/// <returns>Text range of first found occurance or null if no match was found.</returns>
		public ITextRange FindRange( string searchString, CoordinatePoint startLocation, bool searchUp )
		{
			return edtCode.FindRange( searchString, startLocation, null, false, true, searchUp, false );
		}
		/// <summary>
		/// Searches for given string in the text of control and returns text range of first found occurance.
		/// </summary>
		/// <param name="searchString">Text to find.</param>
		/// <param name="startLocation">Start point of search range.</param>
		/// <returns>Text range of first found occurance or null if no match was found.</returns>
		public ITextRange FindRange( string searchString, CoordinatePoint startLocation )
		{
			return edtCode.FindRange( searchString, startLocation, null, false, true, false, false );
		}
		/// <summary>
		/// Searches specified string in the text.
		/// </summary>
		/// <param name="text">String to be found in the text.</param>
		/// <param name="caseSensitive">Specifies whether case sensitive search should be performed.</param>
		/// <param name="wholeWord">Specifies whether only whole words should be searched.</param>
		/// <param name="searchInHidden">Specifies whether search should be performed inside collapsed blocks.</param>
		/// <returns>True if text was found, otherwise false.</returns>
		public bool FindText( string text, bool caseSensitive, bool wholeWord, bool searchInHidden )
		{
			return FindText( text, caseSensitive, wholeWord, searchInHidden, false );
		}
		/// <summary>
		/// Searches specified string in the text (including hidden text).
		/// </summary>
		/// <param name="text">String to be found in the text.</param>
		/// <param name="caseSensitive">Specifies whether case sensitive search should be performed.</param>
		/// <param name="wholeWord">Specifies whether only whole words should be searched.</param>
		/// <returns>True if text was found, otherwise false.</returns>
		public bool FindText( string text, bool caseSensitive, bool wholeWord )
		{
			return FindText( text, caseSensitive, wholeWord, true, false );
		}
		/// <summary>
		/// Searches specified string in the text (including hidden text). Just whole words would be found.
		/// </summary>
		/// <param name="text">String to be found in the text.</param>
		/// <param name="caseSensitive">Specifies whether case sensitive search should be performed.</param>
		/// <returns>True if text was found, otherwise false.</returns>
		public bool FindText( string text, bool caseSensitive )
		{
			return FindText( text, caseSensitive, false, true, false );
		}
		/// <summary>
		/// Searches specified string in the text (including hidden text). Just whole words would be found. Search is case-insensitive.
		/// </summary>
		/// <param name="text">String to be found in the text.</param>
		/// <returns>True if text was found, otherwise false.</returns>
		public bool FindText( string text )
		{
			return FindText( text, false, false, true, false );
		}
		/// <summary>
		/// Searches specified string in the text.
		/// </summary>
		/// <param name="text">String to be found in the text.</param>
		/// <param name="caseSensitive">Specifies whether case sensitive search should be performed.</param>
		/// <param name="searchInHidden">Specifies whether search should be performed inside collapsed blocks.</param>
		/// <param name="wholeWord">Specifies whether only whole words should be searched.</param>
		/// <param name="searchUp">Specifies whether search should be performed in the up direction.</param>
		/// <returns>True if text was found, otherwise false.</returns>
		public bool FindText( string text, bool caseSensitive, bool wholeWord, bool searchInHidden, bool searchUp )
		{
			return FindText( text, caseSensitive, wholeWord, searchInHidden, searchUp, null );
		}
		/// <summary>
		/// Searches specified string in the text.
		/// </summary>
		/// <param name="text">String to be found in the text.</param>
		/// <param name="caseSensitive">Specifies whether case sensitive search should be performed.</param>
		/// <param name="searchInHidden">Specifies whether search should be performed inside collapsed blocks.</param>
		/// <param name="wholeWord">Specifies whether only whole words should be searched.</param>
		/// <param name="searchUp">Specifies whether search should be performed in the up direction.</param>
		/// <param name="startPoint">Point to start search from. If null, search is performed from current cursor position.</param>
		/// <returns>True if text was found, otherwise false.</returns>
		public bool FindText( string text, bool caseSensitive, bool wholeWord, bool searchInHidden, bool searchUp, IParsePoint startPoint )
		{
			bool result = false;
			FindResult fResult = GetTextCoords( text, caseSensitive, wholeWord, searchInHidden, searchUp, startPoint );
			if( !fResult.IsEmpty )
			{
				MarkSearchResult( fResult );
				result = fResult.Result.Success;
			}

			return result;
		}
		/// <summary>
		/// Searches the specified string in the text and replaces it.
		/// </summary>
		/// <param name="text">String to be found in the text.</param>
		/// <param name="caseSensitive">Specifies whether case sensitive search should be performed.</param>
		/// <param name="searchInHidden">Specifies whether search should be performed inside collapsed blocks.</param>
		/// <param name="wholeWord">Specifies whether only whole words should be searched.</param>
		/// <param name="replacetext">Specified the string, found text should be replaced to.</param>
		/// <returns>True if text was found, otherwise false.</returns>
		public bool ReplaceText( string text, bool caseSensitive, bool wholeWord, bool searchInHidden, string replacetext )
		{
			return ReplaceText( text, caseSensitive, wholeWord, searchInHidden, false, replacetext );
		}
		/// <summary>
		/// Searches the specified string in the text and replaces it. Search is done inside collapsed blocks.
		/// </summary>
		/// <param name="text">String to be found in the text.</param>
		/// <param name="caseSensitive">Specifies whether case sensitive search should be performed.</param>
		/// <param name="wholeWord">Specifies whether only whole words should be searched.</param>
		/// <param name="replacetext">Specified the string, found text should be replaced to.</param>
		/// <returns>True if text was found, otherwise false.</returns>
		public bool ReplaceText( string text, bool caseSensitive, bool wholeWord, string replacetext )
		{
			return ReplaceText( text, caseSensitive, wholeWord, true, false, replacetext );
		}
		/// <summary>
		/// Searches the specified string in the text and replaces it. Search is done inside collapsed blocks. Only whole words would be found.
		/// </summary>
		/// <param name="text">String to be found in the text.</param>
		/// <param name="caseSensitive">Specifies whether case sensitive search should be performed.</param>
		/// <param name="replacetext">Specified the string, found text should be replaced to.</param>
		/// <returns>True if text was found, otherwise false.</returns>
		public bool ReplaceText( string text, bool caseSensitive, string replacetext )
		{
			return ReplaceText( text, caseSensitive, false, true, false, replacetext );
		}
		/// <summary>
		/// Searches the specified string in the text and replaces it. Search is done inside collapsed blocks. Only whole words would be found. Search is case-insensitive.
		/// </summary>
		/// <param name="text">String to be found in the text.</param>
		/// <param name="replacetext">Specified the string, found text should be replaced to.</param>
		/// <returns>True if text was found, otherwise false.</returns>
		public bool ReplaceText( string text, string replacetext )
		{
			return ReplaceText( text, false, false, true, false, replacetext );
		}
		/// <summary>
		/// Searches the specified string in the text and replaces it.
		/// </summary>
		/// <param name="text">String to be found in the text.</param>
		/// <param name="caseSensitive">Specifies whether case sensitive search should be performed.</param>
		/// <param name="searchInHidden">Specifies whether search should be performed inside collapsed blocks.</param>
		/// <param name="wholeWord">Specifies whether only whole words should be searched.</param>
		/// <param name="replacetext">Specified the string, found text should be replaced to.</param>
		/// <param name="searchUp">Specifies whether search should be performed in the up direction.</param>
		/// <returns>True if text was found, otherwise false.</returns>
		public bool ReplaceText( string text, bool caseSensitive, bool wholeWord, bool searchInHidden, bool searchUp, string replacetext )
		{
			bool result = false;

			if( text != null && text != string.Empty )
			{
				result = FindText( text, caseSensitive, wholeWord, searchInHidden, searchUp );
				if( result )
				{
					SelectedText = replacetext;
				}
			}

			return result;
		}
		/// <summary>
		/// Searches the specified string in the text and replaces it. Search is done inside collapsed blocks.
		/// </summary>
		/// <param name="text">String to be found in the text.</param>
		/// <param name="caseSensitive">Specifies whether case sensitive search should be performed.</param>
		/// <param name="wholeWord">Specifies whether only whole words should be searched.</param>
		/// <param name="replacetext">Specified the string, found text should be replaced to.</param>
		/// <returns>Number of occurances replaced.</returns>
		public int ReplaceAll( string text, bool caseSensitive, bool wholeWord, string replacetext )
		{
			return ReplaceAll( text, caseSensitive, wholeWord, true, replacetext );
		}
		/// <summary>
		/// Searches the specified string in the text and replaces it. Search is done inside collapsed blocks. Only whole words would be found.
		/// </summary>
		/// <param name="text">String to be found in the text.</param>
		/// <param name="caseSensitive">Specifies whether case sensitive search should be performed.</param>
		/// <param name="replacetext">Specified the string, found text should be replaced to.</param>
		/// <returns>Number of occurances replaced.</returns>
		public int ReplaceAll( string text, bool caseSensitive, string replacetext )
		{
			return ReplaceAll( text, caseSensitive, false, true, replacetext );
		}
		/// <summary>
		/// Searches the specified string in the text and replaces it. Search is done inside collapsed blocks. Only whole words would be found. Search is case-insensitive.
		/// </summary>
		/// <param name="text">String to be found in the text.</param>
		/// <param name="replacetext">Specified the string, found text should be replaced to.</param>
		/// <returns>Number of occurances replaced.</returns>
		public int ReplaceAll( string text, string replacetext )
		{
			return ReplaceAll( text, false, false, true, replacetext );
		}
		/// <summary>
		/// Searches the specified string in the text and replaces it.
		/// </summary>
		/// <param name="text">String to be found in the text.</param>
		/// <param name="caseSensitive">Specifies whether case sensitive search should be performed.</param>
		/// <param name="wholeWord">Specifies whether only whole words should be searched.</param>
		/// <param name="searchInHidden">Specifies whether search should be performed inside collapsed blocks.</param>
		/// <param name="replacetext">Specified the string, found text should be replaced to.</param>
		/// <returns>Number of occurances replaced.</returns>
		public int ReplaceAll( string text, bool caseSensitive, bool wholeWord, bool searchInHidden, string replacetext )
		{
			int result = 0;

			if( text != null && text != string.Empty )
			{
				bool bResult = false;
				int iReplaces = 0;

				UndoGroupOpen();

				IParsePoint startPoint = edtCode.Parser.GetParsePoint( 1, 1 );
				long firstFoundOffset = -1;
				FindResult fres = GetTextCoords( text, caseSensitive, wholeWord, searchInHidden, false, startPoint );
				Point lastPoint = Point.Empty;

				while( !fres.IsEmpty && fres.Result.Success )
				{
					if( firstFoundOffset == -1 )
					{
						firstFoundOffset = fres.StartPoint.Offset + GetEncoding().GetBytes( replacetext ).Length;
					}
					else
					{
						if( firstFoundOffset > fres.StartPoint.Offset )
						{
							break;
						}
					}

					startPoint = fres.EndPoint;
					CoordinatePoint start = edtCode.Parser.GetCoordinatePoint( fres.StartPoint );
					CoordinatePoint end = edtCode.Parser.GetCoordinatePoint( fres.EndPoint );
					edtCode.TextDeleteInternal( start.VirtualLine, start.VirtualColumn, end.VirtualLine, end.VirtualColumn, false );
					edtCode.TextInsertInternal( start.VirtualLine, start.VirtualColumn, replacetext, false );
					lastPoint = new Point( start.VirtualColumn, start.VirtualLine );
					bResult = true;
					iReplaces++;
					fres = GetTextCoords( text, caseSensitive, wholeWord, searchInHidden, false, startPoint );
				}

				if( !lastPoint.IsEmpty )
				{
					this.CurrentPosition = lastPoint;
				}

				if( bResult )
				{
					UndoGroupClose();
				}
				else
				{
					UndoGroupCancel();
				}

				result = iReplaces;
			}

			return result;
		}
		/// <summary>
		/// Creates custom bookmark for the specified line.
		/// </summary>
		/// <param name="iLine">Physical line index.</param>
		/// <param name="painter">Paint handler.</param>
		/// <returns>Interface that represents custom bookmark.</returns>
		public ICustomBookmark SetCustomBookmark( int iLine, BookmarkPaintEventHandler painter )
		{
			return edtCode.Bookmarks.SetCustomBookmark( iLine, painter );
		}
		/// <summary>
		/// Removes custom bookmark from the specified line.
		/// </summary>
		/// <param name="iLine">Physical line index.</param>
		/// <param name="painter">Paint handler.</param>
		public void RemoveCustomBookmark( int iLine, BookmarkPaintEventHandler painter )
		{
			edtCode.Bookmarks.RemoveCustomBookmark( iLine, painter );
		}
		/// <summary>
		/// Opens undo group. All further text changes can be undone with open undo operation.
		/// </summary>
		public void UndoGroupOpen()
		{
			edtCode.UndoGroupOpen();
		}
		/// <summary>
		/// Saves and closes undo group.
		/// </summary>
		public void UndoGroupClose()
		{
			edtCode.UndoGroupClose();
		}
		/// <summary>
		/// Cancels undo grouping. 
		/// </summary>
		public void UndoGroupCancel()
		{
			edtCode.UndoGroupCancel();
		}
		/// <summary>
		/// Collapses all collapsible regions.
		/// </summary>
		public void CollapseAll()
		{
			edtCode.CollapseAll();
		}
		/// <summary>
		/// Expands all collapsed regions.
		/// </summary>
		public void ExpandAll()
		{
			edtCode.ExpandAll();
		}
		/// <summary>
		/// Collapses all collapsible regions in currently selected area or in the current line.
		/// </summary>
		public void Collapse()
		{
			edtCode.Collapse();
		}
		/// <summary>
		/// Expands all collapsed regions in currently selected area or in the current line.
		/// </summary>
		public void Expand()
		{
			edtCode.Expand();
		}
		/// <summary>
		/// Converts point in client coordinates to the virtual position in text.
		/// </summary>
		/// <param name="point">Point in client coordinates.</param>
		/// <returns>Virtual position in the text.</returns>
		public Point PointToVirtualPosition( Point point )
		{
			return edtCode.PointToVirtualPosition( point );
		}
		/// <summary>
		/// Converts point in client coordinates to the virtual position in text.
		/// </summary>
		/// <param name="point">Point in client coordinates.</param>
		/// <param name="bUseScrollers">Specifies whether scrollers information should be used.</param>
		/// <returns>Virtual position in the text.</returns>
		public Point PointToVirtualPosition( Point point, bool bUseScrollers )
		{
			return edtCode.PointToVirtualPosition( point, bUseScrollers );
		}
		/// <summary>
		/// Converts point in client coordinates to the physical position in text.
		/// </summary>
		/// <param name="point">Point in client coordinates.</param>
		/// <returns>Physical position in the text.</returns>
		public Point PointToPhysicalPosition( Point point )
		{
			return edtCode.PointToPhysicalPosition( point );
		}
		/// <summary>
		/// Converts virtual coordinates to physical.
		/// </summary>
		/// <param name="point">Point in virtual coordinates.</param>
		/// <returns>Point in physical coordinates or (0,0) if given virtual position is not present in the stream.</returns>
		public Point ConvertVirtualPositionToPhysical( Point point )
		{
			return edtCode.ConvertVirtualPositionToPhysical( point );
		}
		/// <summary>
		/// Converts virtual position in text to the offset in stream.
		/// </summary>
		/// <param name="point">Virtual position.</param>
		/// <returns>Offset in the file or stream or -1 if such virtual position is not present in stream.</returns>
		public long ConvertVirtualPositionToOffset( Point point )
		{
			return edtCode.ConvertVirtualPositionToOffset( point );
		}
		/// <summary>
		/// Converts in-stream offset to virtual coordinates.
		/// </summary>
		/// <param name="offset">In-Stream offset.</param>
		/// <returns>Virtual position.</returns>
		public Point ConvertOffsetToVirtualPosition( long offset )
		{
            //Fix for SD 2658  Unicode takes 2 bytes for each character
            if (this.GetEncoding().Equals(Encoding.Unicode))
            {
                offset = (offset + 1) * 2;
            }
            // End
			return edtCode.ConvertOffsetToVirtualPosition( offset );
		}
		/// <summary>
		/// Converts point in virtual coordinates to coordinate point.
		/// </summary>
		/// <param name="point">Point in virtual coordinates.</param>
		/// <returns>Coordinate point that corresponds to the specified virtual point.</returns>
		public CoordinatePoint ConvertVirtualPointToCoordinatePoint( Point point )
		{
			return Parser.GetCoordinatePoint( point.Y, point.X, true );
		}
		/// <summary>
		/// Converts point in virtual coordinates to coordinate point.
		/// </summary>
		/// <param name="column">Virtual column index.</param>
		/// <param name="line">Virtual line index.</param>
		/// <returns>Coordinate point that corresponds to the specified virtual point.</returns>
		public CoordinatePoint ConvertVirtualPointToCoordinatePoint( int column, int line )
		{
			return ConvertVirtualPointToCoordinatePoint( new Point( column, line ) );
		}
		/// <summary>
		/// Clears clipboard by putting empty object to it.
		/// </summary>
		public void ClearClipboard()
		{
			Clipboard.SetDataObject( new DataObject( Guid.NewGuid().ToString(), new object() ) );
		}
		/// <summary>
		/// Prints current page on default printer.
		/// </summary>
		public void PrintCurrentPage()
		{
			edtCode.PrintCurrentPage();
		}
		/// <summary>
		/// Prints selected area on default printer.
		/// </summary>
		public void PrintSelection()
		{
			edtCode.PrintSelection();
		}
		/// <summary>
		/// Prints entire document on default printer.
		/// </summary>
		public void PrintNoDialog()
		{
			edtCode.PrintNoDialog();
		}
		/// <summary>
		/// Shows print dialog and gives user ability to start printing.
		/// </summary>
		public void Print()
		{
			edtCode.Print();
		}
		/// <summary>
		/// Shows print preview dialog.
		/// </summary>
		public void PrintPreview()
		{
			edtCode.PrintPreview();
		}
		/// <summary>
		/// Prints pages range.
		/// </summary>
		/// <param name="startPageNumber">Start page in range.</param>
		/// <param name="endPageNumber">End page in range.</param>
		public void PrintPages( int startPageNumber, int endPageNumber )
		{
			edtCode.PrintPages( startPageNumber, endPageNumber );
		}
		/// <summary>
		/// Saves document's XML representation to the file.
		/// </summary>
		/// <param name="filename">Name of the file, the document should be saved to.</param>
		public void SaveAsXML( string filename )
		{
			edtCode.SaveAsXML( filename );
		}
		/// <summary>
		/// Saves document's HTML representation to the file.
		/// </summary>
		/// <param name="filename">Name of the file, the document should be saved to.</param>
		/// <param name="bUseLineBreakTags">Indicates whether line break tags should be used.</param>
		public void SaveAsHTML( string filename, bool bUseLineBreakTags )
		{
			edtCode.SaveAsHTML( filename, bUseLineBreakTags );
		}
		/// <summary>
		/// Saves document's HTML representation to the file.
		/// </summary>
		/// <param name="filename">Name of the file, the document should be saved to.</param>
		public void SaveAsHTML( string filename )
		{
			SaveAsHTML( filename, true );
		}
		/// <summary>
		/// Saves document's RTF representation to the file.
		/// </summary>
		/// <param name="filename">Name of the file, the document should be saved to.</param>
		public void SaveAsRTF( string filename )
		{
			edtCode.SaveAsRTF( filename );
		}
		/// <summary>
		/// Loads file and configuration for it.
		/// </summary>
		/// <param name="fileName">Name of the file to load.</param>
		public bool LoadFile( string fileName )
		{
            this.edtCode.FileName = fileName;
			return edtCode.LoadFile( fileName );
		}
		/// <summary>
		/// Loads file using given encoding.
		/// </summary>
		/// <param name="fileName">Name of file to load.</param>
		/// <param name="encoding">Encoding to use for file reading.</param>
		/// <returns>True if file was successfully loaded.</returns>
		public bool LoadFile( string fileName, Encoding encoding )
		{
            this.edtCode.FileName = fileName;
			return edtCode.LoadFile( fileName, encoding );
		}
		/// <summary>
		/// Shows open file dialog to user and opens selected file.
		/// </summary>
		/// <returns>True if file was successfully loaded.</returns>
		public bool LoadFile()
		{
			return edtCode.LoadFile();
		}
		/// <summary>
		/// Saves text to file.
		/// </summary>
		/// <returns>True if file was successfully saved. False is returned only if user has cancelled saving somehow.</returns>
		public bool Save()
		{
			return edtCode.Save();
		}
		/// <summary>
		/// Shows SaveAs dialog and saves data to specified file.
		/// </summary>
		/// <returns>True if file was successfully saved. False is returned only if user has cancelled saving somehow.</returns>
		public bool SaveAs()
		{
			return edtCode.SaveAs();
		}
		/// <summary>
		/// Saves content to the specified file.
		/// </summary>
		/// <param name="fileName">Name of the file to which the text has to be saved.</param>
		public void SaveFile( string fileName )
		{
			edtCode.SaveFile( fileName, null, null );
		}
		/// <summary>
		/// Saves content to the specified file.
		/// </summary>
		/// <param name="fileName">Name of the file to which the text has to be saved.</param>
		/// <param name="newLineStyle">Style of new line in saved file.</param>
		public void SaveFile( string fileName, NewLineStyle newLineStyle )
		{
			edtCode.SaveFile( fileName, null, RegexTokenizer.GetNewLineString( newLineStyle ) );
		}
		/// <summary>
		/// Saves content to the specified file.
		/// </summary>
		/// <param name="fileName">Name of the file to which the text has to be saved.</param>
		/// <param name="encoding">Encoding of saved file.</param>
		/// <param name="newLineStyle">Style of new line in saved file.</param>
		public void SaveFile( string fileName, Encoding encoding, NewLineStyle newLineStyle )
		{
			edtCode.SaveFile( fileName, encoding, RegexTokenizer.GetNewLineString( newLineStyle ) );
            if (encoding != null)
            {
                edtCode.ISSaved = false;
                SetEncoding(encoding);
            }
		}
		/// <summary>
		/// Saves content to the specified stream using specified encoding and line end style.
		/// </summary>
		/// <param name="stream">Stream to save to.</param>
		/// <param name="encoding">Encoding to use when saving to stream.</param>
		/// <param name="newLineStyle">Line end style used when saving to stream.</param>
		public void SaveStream( Stream stream, Encoding encoding, NewLineStyle newLineStyle )
		{
            edtCode.SaveToStream(stream, encoding, RegexTokenizer.GetNewLineString(newLineStyle));
            if (encoding != null)
            {
                edtCode.ISSaved = false;
                SetEncoding(encoding);
            }
		}
		/// <summary>
		/// Prompts the user with a save dialog if the current file was modified and saves file if needed.
		/// </summary>
		/// <returns>False if file was changed but user decided not to save file, otherwise true.</returns>
		public bool SaveModified()
		{
			return edtCode.SaveModified();
		}
		/// <summary>
		/// Creates new empty file with default coloring.
		/// </summary>
		/// <returns>True if file was created, otherwise false.</returns>
		public bool NewFile()
		{
			return edtCode.NewFile();
		}
		/// <summary>
		/// Creates new empty file with specified coloring.
		/// </summary>
		/// <param name="lang">Language to be used for text coloring.</param>
		/// <returns>True if file was created, otherwise false.</returns>
		public bool NewFile( IConfigLanguage lang )
		{
			return edtCode.NewFile( lang );
		}
		/// <summary>
		/// Moves cursor left, if possible.
		/// </summary>
		public void MoveLeft()
		{
			edtCode.MoveLeft();
		}
		/// <summary>
		/// Move cursor up, if possible.
		/// </summary>
		public void MoveUp()
		{
			edtCode.MoveUp();
		}
		/// <summary>
		/// Moves cursor down if possible.
		/// </summary>
		public void MoveDown()
		{
			edtCode.MoveDown();
		}
		/// <summary>
		/// Moves cursor right if possible.
		/// </summary>
		public void MoveRight()
		{
			edtCode.MoveRight();
		}
		/// <summary>
		/// Moves caret one page up.
		/// </summary>
		public void MovePageUp()
		{
			edtCode.MovePageUp();
		}
		/// <summary>
		/// Moves caret one page down.
		/// </summary>
		public void MovePageDown()
		{
			edtCode.MovePageDown();
		}
		/// <summary>
		/// Moves caret to the end of line
		/// </summary>
		public void MoveToLineEnd()
		{
			edtCode.MoveToLineEnd();
		}
		/// <summary>
		/// Moves caret to the beginning of line. First whitespaces will be skipped.
		/// </summary>
		public void MoveToLineStart()
		{
			edtCode.MoveToLineStart();
		}
		/// <summary>
		/// Moves caret to left by one word, or to the beginning of the current.
		/// </summary>
		public void MoveLeftWord()
		{
			edtCode.MoveLeftWord();
		}
		/// <summary>
		/// Moves caret to the right by one word.
		/// </summary>
		public void MoveRightWord()
		{
			edtCode.MoveRightWord();
		}
		/// <summary>
		/// Moves caret to the beginning of the file.
		/// </summary>
		public void MoveToBeginning()
		{
			edtCode.MoveToBeginning();
		}
		/// <summary>
		/// Moves caret to the end of file.
		/// </summary>
		public void MoveToEnd()
		{
			edtCode.MoveToEnd();
		}
		/// <summary>
		/// Starts selection.
		/// </summary>
		public void StartSelection()
		{
			edtCode.StartSelection();
		}
		/// <summary>
		/// Stops selection.
		/// </summary>
		public void StopSelection()
		{
			edtCode.StopSelection();
		}
		/// <summary>
		/// Reset selection.
		/// </summary>
		public void ResetSelection()
		{
			edtCode.ResetSelection();
		}
		/// <summary>
		/// Toggles insert mode.
		/// </summary>
		public void ToggleInsertMode()
		{
			edtCode.ToggleInsertMode();
		}
		/// <summary>
		/// Inserts text from clipboard.
		/// </summary>
		public virtual void Paste()
		{
			edtCode.Paste();
		}
		/// <summary>
		/// Copies selected text to clipboard.
		/// </summary>
		public virtual void Copy()
		{
			edtCode.Copy();
		}
		/// <summary>
		/// Cuts selected text to clipboard.
		/// </summary>
		public virtual void Cut()
		{
			edtCode.Cut();
		}
		/// <summary>
		/// Removes selection and causes invalidation of the area that was selected.
		/// </summary>
		public void SelectionCancel()
		{
			edtCode.SelectionCancel();
		}
		/// <summary>
		/// Deletes one char to the right.
		/// </summary>
		public void DeleteChar()
		{
			edtCode.DeleteChar();
		}
		/// <summary>
		/// Deletes one word to the right.
		/// </summary>
		public void DeleteWord()
		{
			edtCode.DeleteWord();
		}
		/// <summary>
		/// Deletes one char to the left.
		/// </summary>
		public void DeleteCharLeft()
		{
			edtCode.DeleteCharLeft();
		}
		/// <summary>
		/// Deletes one word to the left.
		/// </summary>
		public void DeleteWordLeft()
		{
			edtCode.DeleteWordLeft();
		}
		/// <summary>
		/// Shows Find dialog window.
		/// </summary>
		[Obsolete( "This method is obsolete. Please use ShowFindDialog method instead." )]
		public void FindDialog()
		{
			edtCode.FindDialog();
		}
		/// <summary>
		/// Shows Find dialog window.
		/// </summary>
		public void ShowFindDialog()
		{
			edtCode.FindDialog();
		}
        /// <summary>
        /// Hide Find dialog window.
        /// </summary>
        public void CloseFindDialog()
        {
            edtCode.CloseFindDialog();
        }
        /// <summary>
        /// Hide Replace dialog window.
        /// </summary>
        public void CloseReplaceDialog()
        {
            edtCode.CloseReplaceDialog();
        }
		/// <summary>
		/// Searches text under cursor, or selected text.
		/// </summary>
		/// <returns>True if text was found.</returns>
		public bool FindCurrentText()
		{
			return edtCode.FindCurrentText();
		}
		/// <summary>
		/// Searches text under cursor, or selected text.
		/// </summary>
		/// <returns>True if text was found.</returns>
		public bool FindNext()
		{
			return edtCode.FindCurrentText();
		}
		/// <summary>
		/// Shows Goto dialog window.
		/// </summary>
		[Obsolete( "This method is obsolete. Please use ShowGoToDialog method instead." )]
		public void GoToDialog()
		{
			edtCode.GoToDialog();
		}
		/// <summary>
		/// Shows Goto dialog window.
		/// </summary>
		public void ShowGoToDialog()
		{
			edtCode.GoToDialog();
		}
		/// <summary>
		/// Shows Replace dialog window.
		/// </summary>
		[Obsolete( "This method is obsolete. Please use ShowReplaceDialog method instead." )]
		public void ReplaceDialog()
		{
			edtCode.ReplaceDialog();
		}
		/// <summary>
		/// Shows Replace dialog window.
		/// </summary>
		public void ShowReplaceDialog()
		{
			edtCode.ReplaceDialog();
		}
		/// <summary>
		/// Selects all text.
		/// </summary>
		public void SelectAll()
		{
			edtCode.SelectAll();
		}
		/// <summary>
		/// Shows keys binding form. Obsolete, use ShowKeysBindingEditor method now.
		/// </summary>
		[Obsolete]
		public void BindKeyboard()
		{
			edtCode.BindKeyboard();
		}
		/// <summary>
		/// Shows keys binding form.
		/// </summary>
		public void ShowKeysBindingEditor()
		{
			edtCode.BindKeyboard();
		}
		/// <summary>
		/// Undoes last operation.
		/// </summary>
		public void Undo()
		{
			edtCode.Undo();
		}
		/// <summary>
		/// Redoes last operation.
		/// </summary>
		public void Redo()
		{
			edtCode.Redo();
		}
		/// <summary>
		/// Refreshes screen, frees up memory, deletes a lot of parsepoints.
		/// </summary>
		public override void Refresh()
		{
			edtCode.Refresh();
		}
		/// <summary>
		/// Turns off collapsing.
		/// </summary>
		public void SwitchCollapsingOff()
		{
			edtCode.SwitchCollapsingOff();
		}
		/// <summary>
		/// Turns on collapsing and collapses all.
		/// </summary>
		public void SwitchCollapsingOn()
		{
			edtCode.SwitchCollapsingOn();
		}
		/// <summary>
		/// Toggles collapsing for current line.
		/// </summary>
		public void ToggleLineCollapsing()
		{
			edtCode.ToggleLineCollapsing();
		}
        public void SetRegexAsNone()
        {
            RegexTokenizer.SetRegexAsNone = true;
        }
		/// <summary>
		/// Shows context prompt.
		/// </summary>
		public void ShowContextPrompt()
		{
			edtCode.ShowContextPrompt();
		}
		/// <summary>
		/// Shows context choice.
		/// </summary>
		public void ShowContextChoice()
		{
			edtCode.ShowContextChoice();
		}
		/// <summary>
		/// Sets bookmark to the current line.
		/// </summary>
		public void BookmarkToggle()
		{
			edtCode.Bookmarks.BookmarkToggle();
		}
		/// <summary>
		/// Clears all bookmarks.
		/// </summary>
		public void BookmarkClear()
		{
			edtCode.Bookmarks.BookmarkClear( false );
		}
		/// <summary>
		/// Clears all bookmarks.
		/// </summary>
		/// <param name="clearCustom">Specifies whether custom bookmarks should be cleared, too.</param>
		public void BookmarkClear( bool clearCustom )
		{
			edtCode.Bookmarks.BookmarkClear( clearCustom );
		}
		/// <summary>
		/// Goes to the next bookmark.
		/// </summary>
		public virtual void BookmarkNext()
		{
			edtCode.Bookmarks.BookmarkNext();
		}
		/// <summary>
		/// Goes to the previous bookmark.
		/// </summary>
		public virtual void BookmarkPrevious()
		{
			edtCode.Bookmarks.BookmarkPrevious();
		}
		/// <summary>
		/// Toggles bookmark with index 1.
		/// </summary>
		public virtual void ToggleIndexedBookmark1()
		{
			edtCode.Bookmarks.ToggleIndexedBookmark1();
		}
		/// <summary>
		/// Toggles bookmark with index 2.
		/// </summary>
		public virtual void ToggleIndexedBookmark2()
		{
			edtCode.Bookmarks.ToggleIndexedBookmark2();
		}
		/// <summary>
		/// Toggles bookmark with index 3.
		/// </summary>
		public virtual void ToggleIndexedBookmark3()
		{
			edtCode.Bookmarks.ToggleIndexedBookmark3();
		}
		/// <summary>
		/// Toggles bookmark with index 4.
		/// </summary>
		public virtual void ToggleIndexedBookmark4()
		{
			edtCode.Bookmarks.ToggleIndexedBookmark4();
		}
		/// <summary>
		/// Toggles bookmark with index 5.
		/// </summary>
		public virtual void ToggleIndexedBookmark5()
		{
			edtCode.Bookmarks.ToggleIndexedBookmark5();
		}
		/// <summary>
		/// Toggles bookmark with index 6.
		/// </summary>
		public virtual void ToggleIndexedBookmark6()
		{
			edtCode.Bookmarks.ToggleIndexedBookmark6();
		}
		/// <summary>
		/// Toggles bookmark with index 7.
		/// </summary>
		public virtual void ToggleIndexedBookmark7()
		{
			edtCode.Bookmarks.ToggleIndexedBookmark7();
		}
		/// <summary>
		/// Toggles bookmark with index 8.
		/// </summary>
		public virtual void ToggleIndexedBookmark8()
		{
			edtCode.Bookmarks.ToggleIndexedBookmark8();
		}
		/// <summary>
		/// Toggles bookmark with index 9.
		/// </summary>
		public virtual void ToggleIndexedBookmark9()
		{
			edtCode.Bookmarks.ToggleIndexedBookmark9();
		}
		/// <summary>
		/// Toggles bookmark with index 0.
		/// </summary>
		public virtual void ToggleIndexedBookmark0()
		{
			edtCode.Bookmarks.ToggleIndexedBookmark0();
		}
		/// <summary>
		/// Switches bookmark with index 1.
		/// </summary>
		public virtual void SwitchIndexedBookmark1()
		{
			edtCode.Bookmarks.SwitchIndexedBookmark1();
		}
		/// <summary>
		/// Switches bookmark with index 2.
		/// </summary>
		public virtual void SwitchIndexedBookmark2()
		{
			edtCode.Bookmarks.SwitchIndexedBookmark2();
		}
		/// <summary>
		/// Switches bookmark with index 3.
		/// </summary>
		public virtual void SwitchIndexedBookmark3()
		{
			edtCode.Bookmarks.SwitchIndexedBookmark3();
		}
		/// <summary>
		/// Switches bookmark with index 4.
		/// </summary>
		public virtual void SwitchIndexedBookmark4()
		{
			edtCode.Bookmarks.SwitchIndexedBookmark4();
		}
		/// <summary>
		/// Switches bookmark with index 5.
		/// </summary>
		public virtual void SwitchIndexedBookmark5()
		{
			edtCode.Bookmarks.SwitchIndexedBookmark5();
		}
		/// <summary>
		/// Switches bookmark with index 6.
		/// </summary>
		public virtual void SwitchIndexedBookmark6()
		{
			edtCode.Bookmarks.SwitchIndexedBookmark6();
		}
		/// <summary>
		/// Switches bookmark with index 7.
		/// </summary>
		public virtual void SwitchIndexedBookmark7()
		{
			edtCode.Bookmarks.SwitchIndexedBookmark7();
		}
		/// <summary>
		/// Switches bookmark with index 8.
		/// </summary>
		public virtual void SwitchIndexedBookmark8()
		{
			edtCode.Bookmarks.SwitchIndexedBookmark8();
		}
		/// <summary>
		/// Switches bookmark with index 9.
		/// </summary>
		public virtual void SwitchIndexedBookmark9()
		{
			edtCode.Bookmarks.SwitchIndexedBookmark9();
		}
		/// <summary>
		/// Switches bookmark with index 0.
		/// </summary>
		public virtual void SwitchIndexedBookmark0()
		{
			edtCode.Bookmarks.SwitchIndexedBookmark0();
		}
		/// <summary>
		/// Toggles showing of whitespaces.
		/// </summary>
		public virtual void ToggleShowingWhiteSpaces()
		{
			edtCode.ToggleShowingWhiteSpaces();
		}
		/// <summary>
		/// Adds leading tab symbol to the selected lines, or just inserts tab symbol.
		/// </summary>
		public virtual void AddTabsToSelection()
		{
			edtCode.AddTabsToSelection();
		}
		/// <summary>
		/// Removes leading tab symbol (or its spaces equivalent) from selected lines.
		/// </summary>
		public virtual void RemoveTabsFromSelection()
		{
			edtCode.RemoveTabsFromSelection();
		}
		/// <summary>
		/// Changes spaces sequences to tabs.
		/// </summary>
		public virtual void TabifySelection()
		{
			edtCode.TabifySelection();
		}
		/// <summary>
		/// Changes tabs sequences to spaces.
		/// </summary>
		public virtual void UntabifySelection()
		{
			edtCode.UntabifySelection();
		}
		/// <summary>
		/// Resets undo information.
		/// </summary>
		public void ResetUndoInfo()
		{
			edtCode.ResetUndoInfo();
		}
		/// <summary>
		/// Registers custom underline format, that can be used when setting region's underlining.
		/// </summary>
		/// <param name="color">Color of the underlining.</param>                                                         
		/// <param name="style">Style of the underlining.</param>
		/// <param name="weight">Weight of the underlining.</param>
		/// <returns>Newly created format.</returns>
		public ISnippetFormat RegisterUnderlineFormat( Color color, UnderlineStyle style, UnderlineWeight weight )
		{
			return edtCode.RegisterUnderlineFormat( color, style, weight );
		}
		/// <summary>
		/// Sets underlining of the specified text region.
		/// </summary>
		/// <param name="pointStart">Starting point.</param>
		/// <param name="pointEnd">End point.</param>
		/// <param name="format">Format to be used.</param>
		public void SetUnderline( CoordinatePoint pointStart, CoordinatePoint pointEnd, ISnippetFormat format )
		{
			edtCode.SetUnderline( pointStart, pointEnd, format );
		}
		/// <summary>
		/// Sets underlining of the specified text region.
		/// </summary>
		/// <param name="pointStart">Starting point (physical coordinates).</param>
		/// <param name="pointEnd">End point (physical coordinates).</param>
		/// <param name="format">Format to be used.</param>
		public void SetUnderline( Point pointStart, Point pointEnd, ISnippetFormat format )
		{
			IParsePoint p1 = GetParsePoint( pointStart );
			IParsePoint p2 = GetParsePoint( pointEnd );

			edtCode.SetUnderline( edtCode.Parser.GetCoordinatePoint( p1 ), edtCode.Parser.GetCoordinatePoint( p2 ), format );
		}
		/// <summary>
		/// Removes underlining in the specified region.
		/// </summary>
		/// <param name="pointStart">Starting point (physical coordinates).</param>
		/// <param name="pointEnd">End point (physical coordinates).</param>
		public void RemoveUnderline( Point pointStart, Point pointEnd )
		{
			IParsePoint p1 = GetParsePoint( pointStart );
			IParsePoint p2 = GetParsePoint( pointEnd );

			edtCode.RemoveUnderline( edtCode.Parser.GetCoordinatePoint( p1 ), edtCode.Parser.GetCoordinatePoint( p2 ) );
		}
		/// <summary>
		/// Removes underlining in the specified region.
		/// </summary>
		/// <param name="pointStart">Starting point.</param>
		/// <param name="pointEnd">End point.</param>
		public void RemoveUnderline( CoordinatePoint pointStart, CoordinatePoint pointEnd )
		{
			edtCode.RemoveUnderline( pointStart, pointEnd );
		}
		/// <summary>
		/// Registers format used for filling the background of the selection or line.
		/// </summary>
		/// <param name="colorBackGround">Specifies background color.</param>
		/// <param name="colorForeGround">Specifies foreground color, used only if hatchstyle is used.</param>
		/// <param name="colorBorder">Specifies border color.</param>
		/// <param name="style">Specifies hatch style of the background.</param>
		/// <param name="useHatchFill">Specifies whether hatchstyle value should be used for drawing background.</param>
		/// <returns>Newly created format.</returns>
		public IBackgroundFormat RegisterBackColorFormat(
			Color colorBackGround, Color colorForeGround, Color colorBorder, HatchStyle style, bool useHatchFill )
		{
			return edtCode.RegisterBackColorFormat( colorBackGround, colorForeGround, colorBorder, style, useHatchFill );
		}
		/// <summary>
		/// Registers line backcolor format.
		/// </summary>
		/// <param name="colorBackGround">Line background color.</param>
		/// <param name="colorBorder">Line border color.</param>
		/// <param name="style">Hatch style of the background.</param>
		/// <param name="useHatchFill">Specifies whether hatchstyle value should be used for drawing background.</param>
		/// <returns>Newly created format.</returns>
		public IBackgroundFormat RegisterBackColorFormat( Color colorBackGround, Color colorBorder, HatchStyle style, bool useHatchFill )
		{
			return edtCode.RegisterBackColorFormat( colorBackGround, colorBorder, style, useHatchFill );
		}
		/// <summary>
		/// Registers line backcolor format.
		/// </summary>
		/// <param name="colorBackGround">Line background color.</param>
		/// <param name="colorBorder">Line border color.</param>
		/// <returns>Newly created format.</returns>
		public IBackgroundFormat RegisterBackColorFormat( Color colorBackGround, Color colorBorder )
		{
			return RegisterBackColorFormat( colorBackGround, colorBorder, HatchStyle.Min, false );
		}
		/// <summary>
		/// Sets background color of the line.
		/// </summary>
		/// <param name="iLine">Line number.</param>
		/// <param name="bFullLine">Specifies if full line should be selected or just text.</param>
		/// <param name="format">Format with background color.</param>
		public void SetLineBackColor( int iLine, bool bFullLine, IBackgroundFormat format )
		{
			edtCode.SetLineBackColor( iLine, bFullLine, format );
		}
		/// <summary>
		/// Sets background format for the selected area.
		/// </summary>
		/// <param name="format">Formatting to be set.</param>
		public void SetSelectionBackColor( IBackgroundFormat format )
		{
			edtCode.SetSelectionBackColor( format );
		}
		/// <summary>
		/// Removes line back color.
		/// </summary>
		/// <param name="iLine">Line number.</param>
		public void RemoveLineBackColor( int iLine )
		{
			edtCode.RemoveLineBackColor( iLine );
		}
		/// <summary>
		/// Removes background coloring from the selected text.
		/// </summary>
		public void RemoveSelectionBackColor()
		{
			edtCode.RemoveSelectionBackColor();
		}
		/// <summary>
		/// Gets line backcolor format, used for the specified line.
		/// </summary>
		/// <param name="iLine">Line number.</param>
		/// <returns>Format, used for drawing background of the line.</returns>
		public IDynamicFormat[] GetLineBackColorFormats( int iLine )
		{
			return edtCode.GetLineBackColorFormats( iLine );
		}
		/// <summary>
		/// Gets copy of the parser's stack at the current position.
		/// </summary>
		/// <returns>Parser stack at the position of the cursor.</returns>
		public ConfigStack GetCurrentStack()
		{
			return edtCode.GetCurrentStack();
		}
		/// <summary>
		/// Gets list of the lexems that are inside current stack.
		/// </summary>
		/// <param name="entireRegion">If true, all lexems will be retrieved; otherwise just those, that are before the cursor.</param>
		/// <returns>List of the lexems.</returns>
		public IList GetLexemsInsideCurrentStack( bool entireRegion )
		{
			return edtCode.GetLexemsInsideCurrentStack( entireRegion );
		}
		/// <summary>
		/// Gets character under cursor.
		/// </summary>
		/// <returns>Character.</returns>
		public char GetCurrentCharacter()
		{
			string txt = edtCode.GetCurrentWord();
			int col = edtCode.GetCurrentWordColumn();

			if( edtCode.CurrentColumn - col >= 0 && txt.Length > edtCode.CurrentColumn - col )
			{
				return txt.Substring( edtCode.CurrentColumn - col, 1 )[ 0 ];
			}

			return ' ';
		}
		/// <summary>
		/// If possible, shows indent guideline of the current region.
		/// </summary>
		public void ShowIndentGuideline()
		{
			edtCode.ShowIndentGuideline();
		}
		/// <summary>
		/// Hides indentation guideline.
		/// </summary>
		public void HideIndentGuideline()
		{
			edtCode.HideIndentGuideline();
		}
		/// <summary>
		/// Jumps to the start of the block.
		/// </summary>
		public void JumpToIndentBlockStart()
		{
			edtCode.JumpToIndentBlockStart();
		}
		/// <summary>
		/// Jumps to the end of the block.
		/// </summary>
		public void JumpToIndentBlockEnd()
		{
			edtCode.JumpToIndentBlockEnd();
		}
		/// <summary>
		/// Shows formats customization dialog.
		/// </summary>
		public void ShowFormatsCustomizationDialog()
		{
			frmFormatsConfig dialog = new frmFormatsConfig();
			(dialog as Form).RightToLeft = this.edtCode.RightToLeft;
			dialog.EditControl = this;
			dialog.ShowDialog();
		}
		/// <summary>
		/// Sets selection start at the specified position in text.
		/// </summary>
		/// <param name="column">Column index of the selection start.</param>
		/// <param name="line">Line index of the selection start.</param>
		public void StartSelection( int column, int line )
		{
			edtCode.StartSelection( column, line );
		}
		/// <summary>
		/// Sets selection end at the specified position in text.
		/// </summary>
		/// <param name="column">Column index of the selection end, should point to the symbol that is next the last selected symbol.</param>
		/// <param name="line">Line index of the selection end, should point to the symbol that is next the last selected symbol.</param>
		public void StopSelection( int column, int line )
		{
			edtCode.StopSelection( column, line );
		}
		/// <summary>
		/// Removes current selection and sets new with start and end in given point.
		/// Later it can be changed using <seealso cref="SetSelectionEnd"/> method.
		/// </summary>
		/// <remarks>
		/// Note: If you just use <c>SetSelectionStart</c> method, no selection will be visible, but it will be created,
		/// and all commands will work as if selection is set.
		/// </remarks>
		/// <param name="cPoint">CoordinatePoint with position of selection start.</param>
		public void SetSelectionStart( CoordinatePoint cPoint )
		{
			edtCode.SetSelectionStart( cPoint );
		}
		/// <summary>
		/// Sets end of the selection.
		/// </summary>
		/// <remarks>
		/// Note: Selection must be already present. <para>Old selection will be simply removed.</para>
		/// </remarks>
		/// <param name="cPoint">CoordinatePoint of end of selection.</param>
		public void SetSelectionEnd( CoordinatePoint cPoint )
		{
			edtCode.SetSelectionEnd( cPoint );
		}
		/// <summary>
		/// Sets selected area of the text.
		/// </summary>
		/// <param name="columnStart">Column index of the selection start.</param>
		/// <param name="lineStart">Line index of the selection start.</param>
		/// <param name="columnEnd">Column index of the selection end, should point to the symbol that is next the last selected symbol.</param>
		/// <param name="lineEnd">Line index of the selection end, should point to the symbol that is next the last selected symbol.</param>
		public void SetSelection( int columnStart, int lineStart, int columnEnd, int lineEnd )
		{
			edtCode.SetSelection( columnStart, lineStart, columnEnd, lineEnd );
		}
		/// <summary>
		/// Selects line with specified index.
		/// </summary>
		/// <param name="lineNumber">Index of line to select.</param>
		public void SelectLine( int lineNumber )
		{
            if( lineNumber <= this.Parser.TotalLines )
            {
				ILexemLine lastLine = this.GetLine( lineNumber );
				SetSelection( 1, lineNumber, lastLine.LineLength + 1, lineNumber );
			}
		}
		/// <summary>
		/// Gets bookmark at the specified line.
		/// </summary>
		/// <param name="line">Line index.</param>
		/// <returns>Bookmark at the specified line</returns>
		public IBookmark BookmarkGet( int line )
		{
			return edtCode.Bookmarks.BookmarkGet( line );
		}
		/// <summary>
		/// Sets bookmark at the specified line.
		/// </summary>
		/// <param name="line">Line index.</param>
		/// <param name="brushinfo">BrushInfo object to fill bookmark area.</param>
		/// <param name="borderColor">Color of bookmark border.</param>
		/// <returns>Added bookmark.</returns>
		public IBookmark BookmarkAdd( int line, BrushInfo brushinfo, Color borderColor )
		{
			IBookmark result = BookmarkAdd( line );
			result.BookmarkBrush = brushinfo;
			result.BorderColor = borderColor;
			return result;
		}
		/// <summary>
		/// Sets bookmark at the specified line.
		/// </summary>
		/// <param name="line">Line index.</param>
		/// <param name="brushinfo">BrushInfo object to fill bookmark area.</param>
		/// <returns>Added bookmark.</returns>
		public IBookmark BookmarkAdd( int line, BrushInfo brushinfo )
		{
			IBookmark result = BookmarkAdd( line );
			result.BookmarkBrush = brushinfo;
			result.BorderColor = Color.Empty;
			return result;
		}
		/// <summary>
		/// Sets bookmark at the specified line.
		/// </summary>
		/// <param name="line">Line index.</param>
		/// <returns>Added bookmark.</returns>
		public IBookmark BookmarkAdd( int line )
		{
			return edtCode.Bookmarks.BookmarkAdd( line );
		}
		/// <summary>
		/// Removes bookmark at the specified line.
		/// </summary>
		/// <param name="line">Line index.</param>
		public void BookmarkRemove( int line )
		{
			edtCode.Bookmarks.BookmarkRemove( line );
		}
		/// <summary>
		/// Removes bookmark at the specified line.
		/// </summary>
		/// <param name="bookmark">Bookmark to be removed.</param>
		public void BookmarkRemove( IBookmark bookmark )
		{
			if( bookmark == null ) throw new ArgumentNullException( "bookmark" );

			edtCode.Bookmarks.BookmarkRemove( bookmark.Line );
		}
		/// <summary>
		/// Strikes out text.
		/// </summary>
		/// <param name="start">Start location of the text to strike out.</param>
		/// <param name="end">End location of the text to strike out.</param>
		/// <param name="color">Color of the text strike out. It you set it to Color.Empty, strikeout will be removed.</param>
		public void StrikeThrough( CoordinatePoint start, CoordinatePoint end, Color color )
		{
			edtCode.SetTextStrikeOut( start, end, color );
		}
		/// <summary>
		/// Strikes out text.
		/// </summary>
		/// <param name="pointStart">Start location of the text to strike out (physical coordinates).</param>
		/// <param name="pointEnd">End location of the text to strike out (physical coordinates).</param>
		/// <param name="color">Color of the text strike out. It you set it to Color.Empty, strikeout will be removed.</param>
		public void StrikeThrough( Point pointStart, Point pointEnd, Color color )
		{
			IParsePoint p1 = GetParsePoint( pointStart );
			IParsePoint p2 = GetParsePoint( pointEnd );

			edtCode.SetTextStrikeOut( edtCode.Parser.GetCoordinatePoint( p1 ), edtCode.Parser.GetCoordinatePoint( p2 ), color );
		}
		/// <summary>
		/// Strikes out line.
		/// </summary>
		/// <param name="linenumber">Lumber of the line to strikeout.</param>
		/// <param name="color">Color of the strike out.</param>
		public void StrikeThrough( int linenumber, Color color )
		{
			RenderedLine line = edtCode.GetLine( linenumber ) as RenderedLine;

			if( null == line ) throw new ArgumentException( "Specified line can not be found", "linenumber" );

			CoordinatePoint pointStart = line.GetStartPoint();
			CoordinatePoint pointEnd = null;

			if( linenumber < edtCode.Parser.TotalLines )
			{
				line = edtCode.GetLine( linenumber + 1 ) as RenderedLine;
				pointEnd = line.GetStartPoint();
			}
			else
			{
				pointEnd = line.GetEndPoint();
			}

			edtCode.SetTextStrikeOut( pointStart, pointEnd, color );
		}
		/// <summary>
		/// Sets border around text.
		/// </summary>
		/// <param name="start">Start of text to draw border around.</param>
		/// <param name="end">End of text to draw border around.</param>
		/// <param name="color">Color of border.</param>
		/// <param name="style">Style of border.</param>
		/// <param name="weight">Weight of border line.</param>
		public void SetTextBorder( CoordinatePoint start, CoordinatePoint end, Color color, FrameBorderStyle style, BorderWeight weight )
		{
			edtCode.SetTextBorder( start, end, color, style, weight );
		}
		/// <summary>
		/// Sets border around text.
		/// </summary>
		/// <param name="pointStart">Start of text to draw border around (physical coordinates).</param>
		/// <param name="pointEnd">End of text to draw border around (physical coordinates).</param>
		/// <param name="color">Color of border.</param>
		/// <param name="style">Style of border.</param>
		/// <param name="weight">Weight of border line.</param>
		public void SetTextBorder( Point pointStart, Point pointEnd, Color color, FrameBorderStyle style, BorderWeight weight )
		{
			IParsePoint p1 = GetParsePoint( pointStart );
			IParsePoint p2 = GetParsePoint( pointEnd );

			edtCode.SetTextBorder( edtCode.Parser.GetCoordinatePoint( p1 ), edtCode.Parser.GetCoordinatePoint( p2 ), color, style, weight );
		}
		/// <summary>
		/// Removes border around text with given coordinates.
		/// </summary>
		/// <param name="start">Start of the text.</param>
		/// <param name="end">End of the text.</param>
		public void RemoveTextBorder( CoordinatePoint start, CoordinatePoint end )
		{
			edtCode.RemoveTextBorder( start, end );
		}
		/// <summary>
		/// Removes border around text with given coordinates.
		/// </summary>
		/// <param name="pointStart">Start of text to draw border around (physical coordinates).</param>
		/// <param name="pointEnd">End of text to draw border around (physical coordinates).</param>
		public void RemoveTextBorder( Point pointStart, Point pointEnd )
		{
			IParsePoint p1 = GetParsePoint( pointStart );
			IParsePoint p2 = GetParsePoint( pointEnd );

			edtCode.RemoveTextBorder( edtCode.Parser.GetCoordinatePoint( p1 ), edtCode.Parser.GetCoordinatePoint( p2 ) );
		}
		/// <summary>
		/// Sets parameters of border that's drawing in page preview.
		/// </summary>
		/// <param name="style">Style of border.</param>
		/// <param name="color">Color of border.</param>
		/// <param name="weight">Weight of border line.</param>
		public void SetPageBorder( FrameBorderStyle style, Color color, BorderWeight weight )
		{
			edtCode.SetPageBorder( style, color, weight );
		}
		/// <summary>
		/// Removes border drawing in page preview.
		/// </summary>
		public void RemovePageBorder()
		{
			edtCode.SetPageBorder( FrameBorderStyle.None, Color.Empty, BorderWeight.Thin );
		}
		/// <summary>
		/// Sets parameters of border that's drawing in page preview. Weight is set to Thin.
		/// </summary>
		/// <param name="style">Style of border.</param>
		/// <param name="color">Color of border.</param>
		public void SetPageBorder( FrameBorderStyle style, Color color )
		{
			SetPageBorder( style, color, BorderWeight.Thin );
		}
		/// <summary>
		/// Sets parameters of border that's drawing in page preview. Weight is set to thick, Style is set to Solid.
		/// </summary>
		/// <param name="color">Color of border.</param>
		public void SetPageBorder( Color color )
		{
			SetPageBorder( FrameBorderStyle.Solid, color, BorderWeight.Thin );
		}
		/// <summary>
		/// Closes stream, makes control readonly.
		/// </summary>
		/// <returns>True is user did not canceled the operation; otherwise false.</returns>
		public bool Close()
		{
			return edtCode.Close();
		}
		/// <summary>
		/// Deletes text at specified position.
		/// </summary>
		/// <param name="start">Start coordinate point of text that has to be deleted.</param>
		/// <param name="end">End coordinate point of text that has to be deleted.</param>
		public void DeleteText( CoordinatePoint start, CoordinatePoint end )
		{
			if( null == start ) throw new ArgumentNullException( "start" );
			if( null == end ) throw new ArgumentNullException( "end" );

			edtCode.DeleteText( start, end );
		}
		/// <summary>
		/// Deletes text at specified position.
		/// </summary>
		/// <param name="pointStart">Start coordinate point of text that has to be deleted (physical coordinates).</param>
		/// <param name="pointEnd">End coordinate point of text that has to be deleted (physical coordinates).</param>
		public void DeleteText( Point pointStart, Point pointEnd )
		{
			IParsePoint p1 = GetParsePoint( pointStart );
			IParsePoint p2 = GetParsePoint( pointEnd );

			edtCode.DeleteText( edtCode.Parser.GetCoordinatePoint( p1 ), edtCode.Parser.GetCoordinatePoint( p2 ) );
		}
		/// <summary>
		/// Deletes all text in document.
		/// </summary>
		public void DeleteAll()
		{
			edtCode.DeleteAll();
		}
		/// <summary>
		/// Returns text represented as XML.
		/// </summary>
		/// <returns>String with text represented as XML.</returns>
		public string GetTextAsXML()
		{
			return edtCode.GetTextAsXML();
		}
		/// <summary>
		/// Returns text represented as HTML.
		/// </summary>
		/// <returns>String with text represented as HTML.</returns>
		public string GetTextAsHTML()
		{
			return edtCode.GetTextAsHTML();
		}
		/// <summary>
		/// Returns text represented as RTF.
		/// </summary>
		/// <returns>String with text represented as RTF.</returns>
		public string GetTextAsRTF()
		{
			return edtCode.GetTextAsRTF();
		}
		/// <summary>
		/// Returns text situated between specified coordinate points represented as XML.
		/// </summary>
		/// <param name="start">Point representing start of the text.</param>
		/// <param name="end">Point representing end of the text.</param>
		/// <returns>String with desired text represented as XML.</returns>
		public string GetTextAsXML( CoordinatePoint start, CoordinatePoint end )
		{
			if( null == start ) throw new ArgumentNullException( "start" );
			if( null == end ) throw new ArgumentNullException( "end" );

			return edtCode.GetTextAsXML( start, end );
		}
		/// <summary>
		/// Returns text situated between specified coordinate points represented as HTML.
		/// </summary>
		/// <param name="start">Point representing start of the text.</param>
		/// <param name="end">Point representing end of the text.</param>
		/// <returns>String with desired text represented as HTML.</returns>
		public string GetTextAsHTML( CoordinatePoint start, CoordinatePoint end )
		{
			if( null == start ) throw new ArgumentNullException( "start" );
			if( null == end ) throw new ArgumentNullException( "end" );

			return edtCode.GetTextAsHTML( start, end );
		}
		/// <summary>
		/// Returns text situated between specified coordinate points represented as RTF.
		/// </summary>
		/// <param name="start">Point representing start of the text.</param>
		/// <param name="end">Point representing end of the text.</param>
		/// <returns>String with desired text represented as RTF.</returns>
		public string GetTextAsRTF( CoordinatePoint start, CoordinatePoint end )
		{
			if( null == start ) throw new ArgumentNullException( "start" );
			if( null == end ) throw new ArgumentNullException( "end" );

			return edtCode.GetTextAsRTF( start, end );
		}
		/// <summary>
		/// Returns text situated between specified coordinate points represented as XML.
		/// </summary>
		/// <param name="pointStart">Point representing start of the text (physical coordinates).</param>
		/// <param name="pointEnd">Point representing end of the text (physical coordinates).</param>
		/// <returns>String with desired text represented as XML.</returns>
		public string GetTextAsXML( Point pointStart, Point pointEnd )
		{
			IParsePoint p1 = GetParsePoint( pointStart );
			IParsePoint p2 = GetParsePoint( pointEnd );

			return edtCode.GetTextAsXML( edtCode.Parser.GetCoordinatePoint( p1 ), edtCode.Parser.GetCoordinatePoint( p2 ) );
		}
		/// <summary>
		/// Returns text situated between specified coordinate points represented as HTML.
		/// </summary>
		/// <param name="pointStart">Point representing start of the text (physical coordinates).</param>
		/// <param name="pointEnd">Point representing end of the text (physical coordinates).</param>
		/// <returns>String with desired text represented as HTML.</returns>
		public string GetTextAsHTML( Point pointStart, Point pointEnd )
		{
			IParsePoint p1 = GetParsePoint( pointStart );
			IParsePoint p2 = GetParsePoint( pointEnd );

			return edtCode.GetTextAsHTML( edtCode.Parser.GetCoordinatePoint( p1 ), edtCode.Parser.GetCoordinatePoint( p2 ) );
		}
		/// <summary>
		/// Returns text situated between specified coordinate points represented as RTF.
		/// </summary>
		/// <param name="pointStart">Point representing start of the text (physical coordinates).</param>
		/// <param name="pointEnd">Point representing end of the text (physical coordinates).</param>
		/// <returns>String with desired text represented as RTF.</returns>
		public string GetTextAsRTF( Point pointStart, Point pointEnd )
		{
			IParsePoint p1 = GetParsePoint( pointStart );
			IParsePoint p2 = GetParsePoint( pointEnd );

			return edtCode.GetTextAsRTF( edtCode.Parser.GetCoordinatePoint( p1 ), edtCode.Parser.GetCoordinatePoint( p2 ) );
		}
		/// <summary>
		/// Splits into 2 equal vertical halves.
		/// </summary>
		public void SplitVertically()
		{
			splitterTop.SplitPosition = 0;
			splitterCenter.SplitPosition = 0;
			splitterBottom.SplitPosition = ClientRectangle.Width / 2;
		}
		/// <summary>
		/// Splits into 2 equal horizontal halves.
		/// </summary>
		public void SplitHorizontally()
		{
			splitterTop.SplitPosition = 0;
			splitterBottom.SplitPosition = 0;
			splitterCenter.SplitPosition = ClientRectangle.Height / 2;
		}
		/// <summary>
		/// Splits into 4 equal parts.
		/// </summary>
		public void SplitFourQuadrants()
		{
			splitterCenter.SplitPosition = ClientRectangle.Height / 2;
			splitterTop.SplitPosition = ClientRectangle.Width / 2;
			splitterBottom.SplitPosition = ClientRectangle.Width / 2;
		}
		/// <summary>
		/// Sets color of text.
		/// </summary>
		/// <param name="start">Start of text to set color.</param>
		/// <param name="end">End of text to set color.</param>
		/// <param name="color">Color to set.</param>
		public void SetTextColor( CoordinatePoint start, CoordinatePoint end, Color color )
		{
			edtCode.SetTextColor( start, end, color );
		}
		/// <summary>
		/// Sets color of text background.
		/// </summary>
		/// <param name="start">Start of text to set color.</param>
		/// <param name="end">End of text to set color.</param>
		/// <param name="color">Color to set.</param>
		public void SetBackgroundColor( CoordinatePoint start, CoordinatePoint end, Color color )
		{
			edtCode.SetBackgroundColor( start, end, color );
		}
		/// <summary>
		/// Sets text as readonly.
		/// </summary>
		/// <param name="start">Start of text to set as readonly.</param>
		/// <param name="end">End of text to set as readonly.</param>
		/// <param name="backColor">Color of text background. Empty if no changes needed.</param>
		/// <param name="textColor">Color of text. Empty if no changes needed.</param>
		public void MarkAsReadOnly( CoordinatePoint start, CoordinatePoint end, Color backColor, Color textColor )
		{
			edtCode.MarkAsReadOnly( start, end, backColor, textColor );
		}
		/// <summary>
		/// Removes readonly status of specified region.
		/// </summary>
		/// <param name="start">Start of text to remove readonly status.</param>
		/// <param name="end">End of text to remove readonly status.</param>
		public void RemoveReadOnly( CoordinatePoint start, CoordinatePoint end )
		{
			edtCode.RemoveReadOnly( start, end );
		}
		/// <summary>
		/// Sets color of text.
		/// </summary>
		/// <param name="pointStart">Start of text to set color (physical coordinates).</param>
		/// <param name="pointEnd">End of text to set color (physical coordinates).</param>
		/// <param name="color">Color to set.</param>
		public void SetTextColor( Point pointStart, Point pointEnd, Color color )
		{
			IParsePoint p1 = GetParsePoint( pointStart );
			IParsePoint p2 = GetParsePoint( pointEnd );

			edtCode.SetTextColor( edtCode.Parser.GetCoordinatePoint( p1 ), edtCode.Parser.GetCoordinatePoint( p2 ), color );
		}
		/// <summary>
		/// Sets color of text background.
		/// </summary>
		/// <param name="pointStart">Start of text to set color (physical coordinates).</param>
		/// <param name="pointEnd">End of text to set color (physical coordinates).</param>
		/// <param name="color">Color to set.</param>
		public void SetBackgroundColor( Point pointStart, Point pointEnd, Color color )
		{
			IParsePoint p1 = GetParsePoint( pointStart );
			IParsePoint p2 = GetParsePoint( pointEnd );

			edtCode.SetBackgroundColor( edtCode.Parser.GetCoordinatePoint( p1 ), edtCode.Parser.GetCoordinatePoint( p2 ), color );
		}
		/// <summary>
		/// Sets text as readonly.
		/// </summary>
		/// <param name="pointStart">Start of text to set as readonly (physical coordinates).</param>
		/// <param name="pointEnd">End of text to set as readonly (physical coordinates).</param>
		/// <param name="backColor">Color of text background. Empty if no changes needed.</param>
		/// <param name="textColor">Color of text. Empty if no changes needed.</param>
		public void MarkAsReadOnly( Point pointStart, Point pointEnd, Color backColor, Color textColor )
		{
			IParsePoint p1 = GetParsePoint( pointStart );
			IParsePoint p2 = GetParsePoint( pointEnd );

			edtCode.MarkAsReadOnly( edtCode.Parser.GetCoordinatePoint( p1 ), edtCode.Parser.GetCoordinatePoint( p2 ), backColor, textColor );
		}
		/// <summary>
		/// Removes readonly status of specified region.
		/// </summary>
		/// <param name="pointStart">Start of text to remove readonly status (physical coordinates).</param>
		/// <param name="pointEnd">End of text to remove readonly status (physical coordinates).</param>
		public void RemoveReadOnly( Point pointStart, Point pointEnd )
		{
			IParsePoint p1 = GetParsePoint( pointStart );
			IParsePoint p2 = GetParsePoint( pointEnd );

			edtCode.RemoveReadOnly( edtCode.Parser.GetCoordinatePoint( p1 ), edtCode.Parser.GetCoordinatePoint( p2 ) );
		}
		/// <summary>
		/// Gets style of new line of current stream.
		/// </summary>
		/// <returns>Style of new line of current stream.</returns>
		public NewLineStyle GetNewLineStyle()
		{
			return edtCode.NewLineStyle;
		}
		/// <summary>
		/// Sets style of new line of current stream.
		/// </summary>
		/// <param name="style">New style of new line of current stream.</param>
		public void SetNewLineStyle( NewLineStyle style )
		{
			edtCode.NewLineStyle = style;
		}
		/// <summary>
		/// Indents text in the specified range.
		/// </summary>
		/// <param name="p1">Beginning of range.</param>
		/// <param name="p2">End of rage.</param>
		public void IndentText( CoordinatePoint p1, CoordinatePoint p2 )
		{
			edtCode.IndentText( p1, p2 );
		}
		/// <summary>
		/// Outdents text in the specified range.
		/// </summary>
		/// <param name="p1">Beginning of range.</param>
		/// <param name="p2">End of rage.</param>
		public void OutdentText( CoordinatePoint p1, CoordinatePoint p2 )
		{
			edtCode.OutdentText( p1, p2 );
		}
		/// <summary>
		/// Indents text in the specified range.
		/// </summary>
		/// <param name="pointStart">Beginning of range (physical coordinates).</param>
		/// <param name="pointEnd">End of rage (physical coordinates).</param>
		public void IndentText( Point pointStart, Point pointEnd )
		{
			IParsePoint p1 = GetParsePoint( pointStart );
			IParsePoint p2 = GetParsePoint( pointEnd );

			edtCode.IndentText( edtCode.Parser.GetCoordinatePoint( p1 ), edtCode.Parser.GetCoordinatePoint( p2 ) );
		}
		/// <summary>
		/// Outdents text in the specified range.
		/// </summary>
		/// <param name="pointStart">Beginning of range (physical coordinates).</param>
		/// <param name="pointEnd">End of rage (physical coordinates).</param>
		public void OutdentText( Point pointStart, Point pointEnd )
		{
			IParsePoint p1 = GetParsePoint( pointStart );
			IParsePoint p2 = GetParsePoint( pointEnd );

			edtCode.OutdentText( edtCode.Parser.GetCoordinatePoint( p1 ), edtCode.Parser.GetCoordinatePoint( p2 ) );
		}
		/// <summary>
		/// Indents selected text.
		/// </summary>
		public void IndentSelection()
		{
            edtCode.FromIndentClick = true;
			edtCode.AddTabsToSelection( true );
            edtCode.FromIndentClick = false;
		}
		/// <summary>
		/// Outdents selected text.
		/// </summary>
		public void OutdentSelection()
		{
			edtCode.RemoveTabsFromSelection();
		}
		/// <summary>
		/// Autoformats given range of text.
		/// </summary>
		/// <param name="iStartLineIndex">Index of first line of range to autoformat.</param>
		/// <param name="iEndLineIndex">Index of last line of range to autoformat.</param>
		public void AutoFormatText( int iStartLineIndex, int iEndLineIndex )
		{
			edtCode.AutoFormatText( iStartLineIndex, iEndLineIndex );
		}
		/// <summary>
		/// Creates snapshot of the control.
		/// </summary>
		/// <returns>Bitmap instance with actual snapshot of the control.</returns>
		public Bitmap CreateBitmap()
		{
			return ActiveXSnapshot.PrintWindow( this );
		}
		/// <summary>
		/// Suspends painting of the control.
		/// </summary>
		public void SuspendPainting()
		{
			pnlCommon.Hide();
			m_iPaintLocks++;
		}
		/// <summary>
		/// Resumes painting of the control.
		/// </summary>
		public void ResumePainting()
		{
			if( m_iPaintLocks > 0 )
			{
				m_iPaintLocks--;
			}
			if( m_iPaintLocks == 0 )
			{
				pnlCommon.Show();
			}
		}
		/// <summary>
		/// Shows the cursor caret.
		/// </summary>
		public void ShowCaret()
		{
			edtCode.ShowCaret();
		}
		/// <summary>
		/// Hides the cursor caret.
		/// </summary>
		public void HideCaret()
		{
			edtCode.HideCaret();
		}
		/// <summary>
		/// Gets current text encoding.
		/// </summary>
		/// <returns>Current encoding.</returns>
		public Encoding GetEncoding()
		{
			return edtCode.Parser.BaseStream.Encoding;
		}
		/// <summary>
		/// Sets current text encoding.
		/// </summary>
		/// <param name="encoding"></param>
		public void SetEncoding( Encoding encoding )
		{
			edtCode.ChangeEncoding( encoding, false );
		}
        /// <summary>
        /// Sets Edit Control find and replace dialog box location
        /// </summary>
        /// <param name="location">Display Location</param>
        public void SetFindAndReplaceDialogLocation(Point location)
        {
            edtCode.FindDialogLocation = location;
        }
		/// <summary>
		/// Comments text in the specified range.
		/// </summary>
		/// <param name="pointStart">Beginning of range (physical coordinates).</param>
		/// <param name="pointEnd">End of rage (physical coordinates).</param>
		public void CommentText( Point pointStart, Point pointEnd )
		{
			IParsePoint p1 = GetParsePoint( pointStart );
			IParsePoint p2 = GetParsePoint( pointEnd );

			edtCode.CommentText( edtCode.Parser.GetCoordinatePoint( p1 ), edtCode.Parser.GetCoordinatePoint( p2 ) );
		}
		/// <summary>
		/// Uncomments text in the specified range.
		/// </summary>
		/// <param name="pointStart">Beginning of range (physical coordinates).</param>
		/// <param name="pointEnd">End of rage (physical coordinates).</param>
		public void UncommentText( Point pointStart, Point pointEnd )
		{
			IParsePoint p1 = GetParsePoint( pointStart );
			IParsePoint p2 = GetParsePoint( pointEnd );

			edtCode.UncommentText( edtCode.Parser.GetCoordinatePoint( p1 ), edtCode.Parser.GetCoordinatePoint( p2 ) );
		}
		/// <summary>
		/// Comments selected text.
		/// </summary>
		public void CommentSelection()
		{
			ITextRange sel = this.Selection;
			if( null != sel )
			{
				edtCode.CommentText( sel.Top, sel.Bottom );
			}
		}
		/// <summary>
		/// Uncomments selected text.
		/// </summary>
		public void UncommentSelection()
		{
			ITextRange sel = this.Selection;
			if( null != sel )
			{
				edtCode.UncommentText( sel.Top, sel.Bottom );
			}
		}
		/// <summary>
		/// Creates control state store.
		/// </summary>
		/// <returns>ControlStateStore instance with info about control state.</returns>
		public ControlStateStore GetControlState()
		{
			ControlStateStore stateStore = new ControlStateStore();
			stateStore.StoreData( edtCode );
			return stateStore;
		}
		/// <summary>
		/// Restores control state.
		/// </summary>
		/// <param name="state">Control state store to get data from.</param>
		public void RestoreControlState( ControlStateStore state )
		{
			state.RestoreData( edtCode, true );
		}
		/// <summary>
		/// Restores control state.
		/// </summary>
		/// <param name="state">Control state store to get data from.</param>
		/// <param name="append">Specifies whether settings from the state should be applied without clearing currently used settings.</param>
		public void RestoreControlState( ControlStateStore state, bool append )
		{
			state.RestoreData( edtCode, append );
		}
		/// <summary>
		/// Adds new code snippet to current language.
		/// </summary>
		/// <param name="title">Snippet title.</param>
		/// <param name="literals">List of literals.</param>
		/// <param name="code">Snippet code.</param>
		public void AddCodeSnippet( string title, ArrayList literals, string code )
		{
			if( title == null || title == string.Empty ) throw new ArgumentNullException( "title" );
			if( code == null || code == string.Empty ) throw new ArgumentOutOfRangeException( "code" );

			edtCode.CodeSnippetsManager.AddCodeSnippet( title, literals, code );
		}
		/// <summary>
		/// Gets code snippet by it's title.
		/// </summary>
		/// <param name="title">Title of code snippet that has to be found.</param>
		/// <returns>Needed code snippet or null if there's no snippet with given title.</returns>
		public CodeSnippet GetSnippetByTitle( string title )
		{
			if( title == null || title == string.Empty ) throw new ArgumentNullException( "title" );

			return edtCode.CodeSnippetsManager.GetSnippetByTitle( title );
		}
		/// <summary>
		/// Changes text of all template members with defined name of currently activated code snippet.
		/// </summary>
		/// <param name="memberName">Name of template member.</param>
		/// <param name="newText">New text.</param>
		public void ChangeSnippetTemplateText( string memberName, string newText )
		{
			if( memberName == null ) throw new ArgumentNullException( "memberName" );
			if( newText == null ) throw new ArgumentNullException( "newText" );
			if( memberName == string.Empty ) throw new ArgumentOutOfRangeException( "membername" );

			Point curPos = edtCode.CurrentPosition;
			edtCode.CodeSnippetsManager.ChangeTemplateText( memberName, newText );
			edtCode.CurrentPosition = curPos;
		}
		/// <summary>
		/// Comments current line.
		/// </summary>
		public void CommentLine()
		{
			CommentLine( this.CurrentLine );
		}
		/// <summary>
		/// Comments single line.
		/// </summary>
		/// <param name="iLineIndex">Index of line to comment.</param>
		public void CommentLine( int iLineIndex )
		{
			edtCode.CommentLine( iLineIndex );
		}
		/// <summary>
		/// Uncomments current line.
		/// </summary>
		public void UnCommentLine()
		{
			UnCommentLine( this.CurrentLine );
		}
		/// <summary>
		/// Uncomments single line.
		/// </summary>
		/// <param name="iLineIndex">Index of line to uncomment.</param>
		public void UnCommentLine( int iLineIndex )
		{
			edtCode.UnCommentLine( iLineIndex );
		}
		/// <summary>
		/// Saves settings from context options dialogue to the isolated storage.
		/// </summary>
		public void SaveSettingToIsolatedStorage()
		{
			IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForDomain();
			IsolatedStorageFileStream storeStream = new IsolatedStorageFileStream( "ContextSettings.xml", FileMode.Create, store );

			XmlTextWriter xmlWriter = new XmlTextWriter( storeStream, Encoding.UTF8 );
			xmlWriter.Formatting = Formatting.Indented;
			GenerateContextOptionsXml( xmlWriter );

			xmlWriter.Flush();
			xmlWriter.Close();
		}
		/// <summary>
		/// Applies context options settings from the isolated storage.
		/// </summary>
		public void ApplySettingFromIsolatedStorage()
		{
			IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForDomain();
			string[] files = store.GetFileNames( "ContextSettings.xml" );

			if( files.Length == 0 ) throw new Exception( "Isolated storage file not found." );

			IsolatedStorageFileStream storeStream = new IsolatedStorageFileStream( "ContextSettings.xml", FileMode.Open, store );
			XmlReader xmlReader = new XmlTextReader( storeStream );
			ApplyContextOptionsFromXml( xmlReader );
			xmlReader.Close();
		}
		/// <summary>
		/// Gets parse point by physical coordinates in the stream.
		/// </summary>
		/// <param name="x">X-coordinate.</param>
		/// <param name="y">Y-coordinate.</param>
		/// <returns>Resulting parse point.</returns>
		public IParsePoint GetParsePoint( int x, int y )
		{
			return edtCode.Parser.BaseStream.GetParsePoint( x, y );
		}
		/// <summary>
		/// Gets parse point by physical coordinates in the stream.
		/// </summary>
		/// <param name="p">Point in the stream..</param>
		/// <returns>Resulting parse point.</returns>
		public IParsePoint GetParsePoint( Point p )
		{
			return edtCode.Parser.BaseStream.GetParsePoint( p.X, p.Y );
		}
		/// <summary>
		/// Closes context choice popup.
		/// </summary>
		public void CloseContextChoice()
		{
			edtCode.ContextChoice.Close();
		}
		/// <summary>
		/// Closes context prompt popup.
		/// </summary>
		public void CloseContextPrompt()
		{
			edtCode.CloseContextPrompt();
		}
		/// <summary>
		/// Closes context tooltip popup.
		/// </summary>
		public void CloseContextTooltip()
		{
			edtCode.CloseContextTooltip();
		}
		/// <summary>
		/// Returns text with current new line style.
		/// </summary>
		/// <returns>String containing EditControl text with current new line style.</returns>
		public string GetTextWithNewLineStyle()
		{
			return edtCode.Parser.BaseStream.GetTextInRange(
				edtCode.Parser.BaseStream.GetParsePoint( 0 ), edtCode.Parser.BaseStream.GetParsePoint( edtCode.Parser.BaseStream.Length ), false );
		}
		/// <summary>
		/// Returns text with given new line style.
		/// </summary>
		/// <param name="newLineStyle">New line style for text.</param>
		/// <returns>String containing EditControl text with given new line style.</returns>
		public string GetTextWithNewLineStyle( Syncfusion.IO.NewLineStyle newLineStyle )
		{
			string text = edtCode.Text;
			text = text.Replace( "\n", RegexTokenizer.GetNewLineString( newLineStyle ) );
			return text;
		}
		/// <summary>
		/// Shows code snippets choice list.
		/// </summary>
		public virtual void ShowCodeSnippets()
		{
			edtCode.ShowCodeSnippets();
		}
        /// <summary>
        /// Scrolls the contents of the control to the current caret position.
        /// </summary>
        public virtual void ScrollToCaret()
        {
            edtCode.ScrollToCaret();
        }
		#endregion

		#region Nonpublic Methods
		/// <summary>
		/// Updates control size in autosize mode.
		/// </summary>
		protected void UpdateSizeInAutoSizeMode()
		{
			if( AutoSize != false )
			{
				Size size = edtCode.GetDesiredSize();
				size.Height += DEF_MIN_BORDER_DIST_AUTOSIZE;
				size.Height += edtCode.ScrollOffsetTop + edtCode.ScrollOffsetBottom;

				if( WordWrap )
				{
					if( WordWrapMode == WordWrapMode.Control )
					{
						size.Width = Size.Width;
					}
					else if( WordWrapMode == WordWrapMode.WordWrapMargin )
					{
						size.Width = TextAreaWidth + StreamEditControl.DEF_PRE_TEXT_AREA + edtCode.ScrollOffsetLeft + DEF_MIN_BORDER_DIST_AUTOSIZE;
					}
				}
				else
				{
					size.Width += edtCode.ScrollOffsetLeft + edtCode.ScrollOffsetRight;
					size.Width += DEF_MIN_BORDER_DIST_AUTOSIZE;

					if( edtCode.MarkLineWrapping )
					{
						size.Width += edtCode.WrapMarkingImage.Width;
					}
				}

				if( MinSize != Size.Empty )
				{
					size.Width = Math.Max( size.Width, MinSize.Width );
					size.Height = Math.Max( size.Height, MinSize.Height );
				}

				Size = size;
				edtCode.VirtualSize = Size.Empty;
				edtCode.HScroll = edtCode.VScroll = false;
			}
		}
		/// <summary>
		/// Updates information regarding currently used encoding in the status bar.
		/// </summary>
		protected void UpdateStatusBarEncodingPanel()
		{
			string text = string.Empty;

			if( null != edtCode.Parser )
			{
				text = edtCode.Parser.BaseStream.Encoding.EncodingName;
			}

			StatusBarSettings.EncodingPanel.Panel.Text = text;
		}
		/// <summary>
		/// Updates parser's text parsing mode.
		/// </summary>
		protected void OnParsingModeChanged()
		{
			if( null != edtCode && null != edtCode.Parser )
			{
				edtCode.Parser.ParsingMode = ParsingMode;
			}
		}
		/// <summary>
		/// Attaches event reraisers to edit control.
		/// </summary>
		protected void AttachToEvents()
		{
			edtCode.ContextChoice.ItemSelected += new ContextChoiceItemSelectedEventHandler( edtCode_ContextChoiceItemSelected );
			edtCode.ContextChoice.ContextChoiceRightClick += new ContextChoiceItemEventHandler( OnContextChoiceRightClick );
			edtCode.ContextPromptSelectionChanged += new ContextPromptSelectionChangedEventHandler( edtCode_ContextPromptSelectionChanged );
			edtCode.TextChanging += new TextChangingEventHandler( edtCode_TextChanging );
            edtCode.LineChanged += new TextChangedEventHandler(edtCode_LineChanged);
            edtCode.LineInserted += new LineInsertedEventHandler(edtCode_LineInserted);
            edtCode.LineDeleted += new LineDeletedEventHandler(edtCode_LineDeleted);
            edtCode.TextChanged += new EventHandler( edtCode_TextChanged );
            edtCode.Find += new EventHandler(edtCode_Find);
			edtCode.SingleLineChanged += new EventHandler( edtCode_SingleLineChanged );
			edtCode.MenuFill += new EventHandler( edtCode_MenuFill );
			edtCode.UpdateContextToolTip += new UpdateTooltipEventHandler( edtCode_UpdateContextToolTip );
			edtCode.UpdateBookmarkToolTip += new UpdateBookmarkTooltipEventHandler( edtCode_UpdateBookmarkToolTip );
			edtCode.ContextPromptClose += new ContextPromptCloseEventHandler( edtCode_ContextPromptClose );
			edtCode.ContextPromptOpen += new ContextPromptUpdateEventHandler( edtCode_ContextPromptOpen );
			edtCode.ContextPromptUpdate += new ContextPromptUpdateEventHandler( edtCode_ContextPromptUpdate );
			edtCode.ContextPromptBeforeOpen += new CancelEventHandler( edtCode_ContextPromptBeforeOpen );
			edtCode.ContextChoice.ContextChoiceOpen += new ContextChoiceEventHandler( edtCode_ContextChoiceOpen );
			edtCode.ContextChoice.ContextChoiceUpdate += new ContextChoiceEventHandler( edtCode_ContextChoiceUpdate );
			edtCode.ContextChoice.ContextChoiceBeforeOpen += new CancelEventHandler( edtCode_ContextChoiceBeforeOpen );
			edtCode.Bookmarks.DrawLineMark += new DrawLineMarkEventHandler( edtCode_DrawLineMark );
			edtCode.LanguageChanged += new EventHandler( edtCode_LanguageChanged );
			edtCode.OnBeforeLineNumberPaint += new OnBeforeLineNumberPaintEventHandler(edtCode_OnBeforeLineNumberPaint);
			edtCode.ConfigurationChanged += new EventHandler( edtCode_ConfigurationChanged );
			edtCode.RegisteringKeyCommands += new EventHandler( edtCode_RegisteringKeyCommands );
			edtCode.RegisteringDefaultKeyBindings += new EventHandler( edtCode_RegisteringDefaultKeyBindings );
			edtCode.ReadOnlyChanged += new EventHandler( edtCode_ReadOnlyChanged );
			edtCode.ChangingStream += new ChangingStreamEventHandler( edtCode_ChangingStream );
			edtCode.OperationStopped += new LongOperationEventHandler( edtCode_OperationStopped );
			edtCode.OperationStarted += new LongOperationEventHandler( edtCode_OperationStarted );
			edtCode.PaintUserMargin += new PaintEventHandler( edtCode_PaintUserMargin );
			edtCode.CanUndoRedoChanged += new EventHandler( edtCode_CanUndoRedoChanged );
			edtCode.SelectionChanged += new EventHandler( edtCode_SelectionChanged );
			edtCode.CursorPositionChanged += new EventHandler( edtCode_CursorPositionChanged );
			edtCode.InsertModeChanged += new EventHandler( edtCode_InsertModeChanged );
			edtCode.WordWrapChanged += new EventHandler( edtCode_WordWrapChanged );
			edtCode.DragOver += new DragEventHandler( edtCode_DragOver );
			edtCode.DragDrop += new DragEventHandler( edtCode_DragDrop );
			edtCode.DragEnter += new DragEventHandler( edtCode_DragEnter );
			edtCode.DragLeave += new EventHandler( edtCode_DragLeave );
			edtCode.MouseDown += new MouseEventHandler( edtCode_MouseDown );
			edtCode.MouseEnter += new EventHandler( edtCode_MouseEnter );
			edtCode.MouseLeave += new EventHandler( edtCode_MouseLeave );
			edtCode.MouseMove += new MouseEventHandler( edtCode_MouseMove );
			edtCode.MouseHover += new EventHandler( edtCode_MouseHover );
			edtCode.MouseUp += new MouseEventHandler( edtCode_MouseUp );
			edtCode.MouseWheel += new MouseEventHandler( edtCode_MouseWheel );
			edtCode.KeyDown += new KeyEventHandler( edtCode_KeyDown );
			edtCode.KeyUp += new KeyEventHandler( edtCode_KeyUp );
			edtCode.KeyPress += new KeyPressEventHandler( edtCode_KeyPress );
			edtCode.Click += new EventHandler( edtCode_Click );
			edtCode.DoubleClick += new EventHandler( edtCode_DoubleClick );
			this.CursorChanged += new EventHandler( EditControl_CursorChanged );
			edtCode.ContextChoiceSelectedTextInsert += new ContextChoiceTextInsertEventHandler( edtCode_ContextChoiceSelectedTextInsert );
			edtCode.PrintFooter += new PrintHeadlineEventHandler( edtCode_PrintFooter );
			edtCode.PrintHeader += new PrintHeadlineEventHandler( edtCode_PrintHeader );
			edtCode.IndicatorMarginClick += new IndicatorClickEventHandler( edtCode_IndicatorMarginClick );
			edtCode.IndicatorMarginDoubleClick += new IndicatorClickEventHandler( edtCode_IndicatorMarginDoubleClick );
			edtCode.PaintLockRequest += new EventHandler( edtCode_PaintLockRequest );
			edtCode.PaintUnlockRequest += new EventHandler( edtCode_PaintUnlockRequest );
			edtCode.DrawUserMarginText += new DrawUserMarginTextEventHandler( edtCode_DrawUserMarginText );
			edtCode.SaveStreamWithDataLoss += new SaveWithDataLosingEventHandler( edtCode_SaveStreamWithDataLoss );
			edtCode.SaveFileWithDataLoss += new SaveWithDataLosingEventHandler( edtCode_SaveFileWithDataLoss );
			edtCode.FileNameChanged += new EventHandler( edtCode_FileNameChanged );
			edtCode.StreamClose += new StreamCloseEventHandler( edtCode_StreamClose );
			edtCode.ParserCreated += new EventHandler( edtCode_ParserCreated );
			edtCode.ParserDestroyed += new EventHandler( edtCode_ParserDestroyed );
			edtCode.OutliningTooltipBeforePopup += new OutliningTooltipBeforePopupEventHandler( edtCode_OutliningTooltipBeforePopup );
			edtCode.EncodingChanged += new EncodingChangedEventHandler( edtCode_EncodingChanged );
			edtCode.OutliningTooltipPopup += new CollapsedRegionRelatedEventHandler( edtCode_OutliningTooltipPopup );
			edtCode.OutliningTooltipClose += new CollapsedRegionRelatedEventHandler( edtCode_OutliningTooltipClose );
			edtCode.GetMinimalWidth += new ChangeValueEventHandler( edtCode_GetMinimalWidth );
			edtCode.CodeSnippetsManager.CodeSnippetActivating += new CancellableCodeSnippetsEventHandler( CodeSnippetActivatingReRaiser );
			edtCode.CodeSnippetsManager.CodeSnippetDeactivating += new CodeSnippetsEventHandler( CodeSnippetDeactivatingReRaiser );
			edtCode.CodeSnippetsManager.CodeSnippetTemplateTextChanging += new CodeSnippetTemplateTextChangingEventHandler( CodeSnippetTemplateTextChangingReRaiser );
			edtCode.CodeSnippetsManager.NewSnippetMemberHighlighting += new NewSnippetMemberHighlightingEventHandler( NewSnippetMemberHighlightingReRaiser );
			edtCode.ScrollbarsSizeUpdated += new EventHandler( OnEdtCodeScrollbarsSizeUpdated );

			fakeEditControl1.MouseEnter += new EventHandler( FakeEditorMouseMove );
			fakeEditControl2.MouseEnter += new EventHandler( FakeEditorMouseMove );
			fakeEditControl3.MouseEnter += new EventHandler( FakeEditorMouseMove );
		}
		/// <summary>
		/// Looks for specified expression in text.
		/// </summary>
		/// <param name="start">Start position for the search.</param>
		/// <param name="expression">Expression to be found.</param>
		/// <param name="searchUp">Specifies whether upsearch should be done.</param>
		/// <param name="bSearchInCollapsed">Flag, that specifies whether text can be found in collapsed region.</param>
		/// <returns>Search results.</returns>
		internal FindResult FindRegex( IParsePoint start, Regex expression, bool bSearchInCollapsed, bool searchUp )
		{
			return edtCode.FindRegex( start, expression, bSearchInCollapsed, searchUp );
		}
		/// <summary>
		/// Saves file.
		/// </summary>
		private void SaveCommand()
		{
			edtCode.Save();
		}
		/// <summary>
		/// SaveAs command.
		/// </summary>
		private void SaveAsCommand()
		{
			edtCode.SaveAs();
		}
		/// <summary>
		/// Opens file.
		/// </summary>
		private void OpenCommand()
		{
			edtCode.LoadFile();
		}
		/// <summary>
		/// Creates new file.
		/// </summary>
		private void NewCommand()
		{
			edtCode.NewFile();
		}
		/// <summary>
		/// Exchanges fake editor and real editor.
		/// </summary>
		/// <param name="senderFake"></param>
		private void ExchangeWithFake( FakeEditControl senderFake )
		{
			ExchangeWithFake( senderFake, Point.Empty );
		}
		/// <summary>
		/// Exchanges fake editor and real editor.
		/// </summary>
		/// <param name="senderFake"></param>
		/// <param name="mousePoint"></param>
		private void ExchangeWithFake( FakeEditControl senderFake, Point mousePoint )
		{
			Control oldParent = edtCode.Parent;
			this.pnlCommon.Visible = false;
			Point oldScrollerCode = edtCode.AutoScrollPosition;
			oldScrollerCode.X = -oldScrollerCode.X;
			oldScrollerCode.Y = -oldScrollerCode.Y;

			Point oldScrollerFake = senderFake.AutoScrollPosition;
			oldScrollerFake.X = -oldScrollerFake.X;
			oldScrollerFake.Y = -oldScrollerFake.Y;

			if( mousePoint != Point.Empty )
			{
				Point newVitualCoord = edtCode.PointToVirtualPosition( mousePoint, false );
				edtCode.CurrentPosition = newVitualCoord;
			}

			edtCode.Focus();
			edtCode.Parent = senderFake.Parent;
			edtCode.Focus();
			senderFake.Parent = oldParent;

			edtCode.AutoScrollPosition = oldScrollerFake;
			senderFake.AutoScrollPosition = oldScrollerCode;
			ReAlignAll();
			this.pnlCommon.Visible = true;
			edtCode.Focus();
		}
		/// <summary>
		/// Exchanges fake editor and real editor by timer.
		/// </summary>
		/// <param name="senderFake"></param>
		private void ExchangeWithFakeDelayed( FakeEditControl senderFake )
		{
			if( m_controlExchangeWith != senderFake )
			{
				m_controlExchangeWith = senderFake;
				m_timerUpdateWordWrap.Stop();
				m_timerUpdateWordWrap.Start();
			}
		}
		/// <summary>
		/// Calculates position of the vertical splitter.
		/// </summary>
		/// <param name="iPosition">Original position.</param>
		/// <returns>Resulting position.</returns>
		private int CalculateVerticalSplitterPosition( int iPosition )
		{
			if( WordWrap )
			{
				iPosition = ( iPosition > ( pnlClient.Width * 1 / 3 ) ) ? ( pnlClient.Width / 2 ) : ( 0 );
			}

			return iPosition;
		}
		/// <summary>
		/// Updates sizes of the vertical splitters.
		/// </summary>
		private void UpdateVerticalSplitters()
		{
#if VERBOSE && DEBUG
      Debug.WriteLine( DateTime.Now, "UpdateVerticalSplitters" );
#endif

			if( edtCode != null && !edtCode.IsDisposed && WordWrap )
			{
				int posTop = CalculateVerticalSplitterPosition( splitterTop.SplitPosition );
				int posBottom = CalculateVerticalSplitterPosition( splitterBottom.SplitPosition );

				SetSplitterPosition( splitterTop, posTop );
				SetSplitterPosition( splitterBottom, posBottom );
			}
		}
		/// <summary>
		/// Checks if splitter have different position than the specified one and if not, changes it.
		/// </summary>
		/// <param name="splitter">Splitter to be updated.</param>
		/// <param name="position">Position to be set.</param>
		private void SetSplitterPosition( Splitter splitter, int position )
		{
			if( splitter == null ) throw new ArgumentNullException( "splitter" );

			if( position != splitter.SplitPosition )
			{
				splitter.SplitPosition = position;
			}
		}
		/// <summary>
		/// Updates size of the control, based on the parent size.
		/// </summary>
		/// <param name="control">Control to be updated.</param>
		private void UpdateControlSize( Control control )
		{
			Rectangle parentRect = control.Parent.ClientRectangle;

			bool b = !( parentRect.Width <= 2 || parentRect.Height <= 2 );
			if( !b )
			{
				control.Size = Size.Empty;
			}
			else
			{
				int width = parentRect.Width - 0;
				control.Size = new Size( width, parentRect.Height - 0 );
				control.Location = new Point( 0, 0 );
			}
		}
		/// <summary>
		/// Manually realigns all controls.
		/// </summary>
		private void ReAlignAll()
		{
			LockEvents();
#if VERBOSE && DEBUG
      Debug.WriteLine( DateTime.Now, "ReAlignAll" );
#endif
			this.pnlCommon.Visible = false;

			if( !ShowHorizontalSplitters && splitterCenter.SplitPosition > 0 )
			{
				splitterCenter.SplitPosition = 0;
			}
			if( !ShowVerticalSplitters && splitterBottom.SplitPosition > 0 )
			{
				splitterBottom.SplitPosition = 0;
			}
			if( !ShowVerticalSplitters && splitterTop.SplitPosition > 0 )
			{
				splitterTop.SplitPosition = 0;
			}

			UpdateVerticalSplitters();

			UpdateControlSize( edtCode );
			UpdateControlSize( fakeEditControl1 );
			UpdateControlSize( fakeEditControl2 );
			UpdateControlSize( fakeEditControl3 );

			this.pnlCommon.Visible = true;

			edtCode.RemeasureLinesWrapping();
			edtCode.InvalidateAll();
			UnlockEvents();
		}
		/// <summary>
		/// Sets control heights in singleline mode.
		/// </summary>
		private void UpdateHeightInSingleLineMode()
		{
			if( SingleLineMode )
			{
				Height = Language.MaxLineHeight + 6; // +8 - borders of control itself and inner control.
			}
		}
		/// <summary>
		/// Locks event processing.
		/// </summary>
		private void LockEvents()
		{
			m_iLockEvents++;
		}
		/// <summary>
		/// Unlocks event processing.
		/// </summary>
		private void UnlockEvents()
		{
			if( m_iLockEvents > 0 )
			{
				m_iLockEvents--;
			}
		}
		/// <summary>
		/// Draws string at the center of the specified rectangle.
		/// </summary>
		/// <param name="g">Graphics object.</param>
		/// <param name="rectangle">Output rectangle.</param>
		/// <param name="text">Text to be drawn.</param>
		private void DrawStringCentered( Graphics g, Rectangle rectangle, string text )
		{
			RectangleF rect = new RectangleF( rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height );
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;

			g.DrawString( text, Font, Brushes.Gray, rect, format );
			format.Dispose();
		}
		/// <summary>
		/// Redraws nonclient area of the control.
		/// </summary>
		protected void InvalidateNonClientArea()
		{
			WinAPI.RedrawWindow( this.Handle, IntPtr.Zero, IntPtr.Zero, RDW_FRAME | RDW_UPDATENOW | RDW_INVALIDATE );
		}
		/// <summary>
		/// Gets bool indicating visibility of fake edit control.
		/// </summary>
		/// <param name="fake">Fake edit control to retrieve visibility of.</param>
		/// <returns>Bool indicating visibility of fake edit control</returns>
		internal bool FakeVisible( FakeEditControl fake )
		{
			if( null == fake ) throw new ArgumentNullException( "fake" );

			return ( fake.Parent.Width > 0 && fake.Parent.Height > 0 );
		}
		/// <summary>
		/// Geterates Xml document with context options.
		/// </summary>
		/// <param name="writer">Xml writer.</param>
		private void GenerateContextOptionsXml( XmlWriter writer )
		{
			if( writer == null ) throw new ArgumentNullException( "writer" );

			writer.WriteStartElement( "ContextOptionsSettings" );

			PropertyInfo[] arrProperties = this.GetType().GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
			ContextOptionsAttribute[] attributes = null;

			foreach( PropertyInfo property in arrProperties )
			{
				attributes = ( ContextOptionsAttribute[] )property.GetCustomAttributes( typeof( ContextOptionsAttribute ), false );
				if( attributes.Length == 0 ) continue;

				writer.WriteStartElement( property.Name );
				writer.WriteString( property.GetValue( this, null ).ToString() );
				writer.WriteEndElement();
			}

			writer.WriteEndElement();
		}
		/// <summary>
		/// Applies context options settings from Xml document.
		/// </summary>
		/// <param name="reader">Xml reader.</param>
		private void ApplyContextOptionsFromXml( XmlReader reader )
		{
			if( reader == null ) throw new ArgumentNullException( "reader" );

			IList settings = new ArrayList();
			reader.ReadStartElement( "ContextOptionsSettings" );
			while( true )
			{
				DictionaryEntry entry = new DictionaryEntry();
				reader.Read();

				if( reader.NodeType == XmlNodeType.EndElement )
				{
					break;
				}

				entry.Key = reader.Name; // Name of the property.
				entry.Value = reader.ReadString(); // Value of the property.
				reader.ReadEndElement();

				settings.Add( entry );
			}

			PropertyInfo[] arrProperties = this.GetType().GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
			ContextOptionsAttribute[] attributes = null;

			foreach( PropertyInfo property in arrProperties )
			{
				attributes = ( ContextOptionsAttribute[] )property.GetCustomAttributes( typeof( ContextOptionsAttribute ), false );

				if( attributes.Length == 0 )
				{
					continue;
				}

				foreach( DictionaryEntry entry in settings )
				{
					if( ( string )entry.Key == property.Name )
					{
						Type propType = property.PropertyType;
						object value = null;

						if( propType.IsEnum )
						{
							value = Enum.Parse( propType, ( string )entry.Value );
						}
						else if( propType == typeof( int ) )
						{
							value = int.Parse( ( string )entry.Value );
						}
						else if( propType == typeof( bool ) )
						{
							value = bool.Parse( ( string )entry.Value );
						}

						if( value == null ) throw new Exception( "Invalid Xml." );

						property.SetValue( this, value, null );
					}
				}
			}
		}
		/// <summary>
		/// Updates visual style of scrollers
		/// </summary>
		private void UpdateScrollersVisualStyle()
		{
            ScrollBarCustomDrawStyles style = this.ScrollVisualStyle;
           
            m_scrollersFrame.VisualStyle = style;
            m_scrollersFrameForFake1.VisualStyle = style;
            m_scrollersFrameForFake2.VisualStyle = style;
            m_scrollersFrameForFake3.VisualStyle = style;

            if (style == ScrollBarCustomDrawStyles.Office2007 ||
                style == ScrollBarCustomDrawStyles.Office2007Generic)
            {
                m_scrollersFrame.OfficeColorScheme = ScrollColorScheme;
                m_scrollersFrameForFake1.OfficeColorScheme = ScrollColorScheme;
                m_scrollersFrameForFake2.OfficeColorScheme = ScrollColorScheme;
                m_scrollersFrameForFake3.OfficeColorScheme = ScrollColorScheme;
            }
		}
		/// <summary>
		/// Searches for text and returns it's coordinates if found.
		/// </summary>
		/// <param name="text">String to be found in the text.</param>
		/// <param name="caseSensitive">Specifies whether case sensitive search should be performed.</param>
		/// <param name="searchInHidden">Specifies whether search should be performed inside collapsed blocks.</param>
		/// <param name="wholeWord">Specifies whether only whole words should be searched.</param>
		/// <param name="searchUp">Specifies whether search should be performed in the up direction.</param>
		/// <param name="startPoint">Point to start search from. If null, search is performed from current cursor position.</param>
		/// <returns>FindResult instance.</returns>
		private FindResult GetTextCoords(
			string text, bool caseSensitive, bool wholeWord, bool searchInHidden, bool searchUp, IParsePoint startPoint )
		{
			FindResult result = FindResult.Empty;
			if( text != null && text != string.Empty )
			{
				text = Regex.Escape( text );
				RegexOptions opt = RegexOptions.None;

				if( !caseSensitive )
				{
					opt |= RegexOptions.IgnoreCase;
				}
				if( wholeWord )
				{
					text = string.Format( FrmFindDialog.DEF_STR_WHOLEWORD_REGEX_TEMPLATE, StreamsWrapper.DEF_SEARCH_DATA_GROUP, text );
				}

				Regex regex = new Regex( text, opt );

				IParsePoint pointStart = startPoint;
				if( pointStart == null )
				{
					pointStart = ( edtCode.SelectedText != string.Empty && searchUp ) ? ( edtCode.Selection.Start.PhysicalPoint )
						: ( edtCode.CursorManager.CursorPhysicalCoordinates.Position );
				}

				result = edtCode.GetFindResult( pointStart, regex, searchInHidden, searchUp, text );
			}

			return result;
		}

		/// <summary>
		/// Checks if RTL to change as the results of pressing ctl+shift's
		/// </summary>
		/// <param name="iKeyCode"></param>
		/// <param name="iFlags"></param>
		/// <returns></returns>
		protected bool? IsRTLChangeRequired(int iKeyCode, int iFlags)
		{
			bool? b = null;

			byte[] bCharData = new byte[256];
			Utils.WinAPI.GetKeyboardState(bCharData);
			if (bCharData[Utils.WinAPI.VK_LCONTROL] >= 128 && bCharData[Utils.WinAPI.VK_LSHIFT] >= 128)
			{
				b = false;
			}
			else if (bCharData[Utils.WinAPI.VK_RCONTROL] >= 128 && bCharData[Utils.WinAPI.VK_RSHIFT] >= 128)
			{
				b = true;
			}
			return b;
		}

		#endregion

		#region Overrides
        /// <summary>
        /// Overrides the Processcmdkey
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
		protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
		{
			bool handled = false;
            Form parentForm = this.FindForm();

            if ( !this.AcceptsEscape && keyData == Keys.Escape && parentForm.CancelButton != null)                 
			{
				parentForm.Close();
				handled = true;
			}

			if( !handled )
			{
				handled = base.ProcessCmdKey( ref msg, keyData );
                
			}

			return handled;
		}

		/// <summary>
		/// Overriden. Changes <see cref="System.Windows.Forms.CreateParams.Style"/> to show or hide scrollbars and also consider the controls
		/// <see cref="ScrollControl.BorderStyle"/> setting.
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		protected override CreateParams CreateParams
		{
			[System.Security.Permissions.SecurityPermission( SecurityAction.LinkDemand, UnmanagedCode = true )]
			get
			{
				System.Windows.Forms.CreateParams cp = base.CreateParams;

				switch( this.borderStyle )
				{
					case BorderStyle.Fixed3D:
						cp.ExStyle |= 0x200; // WS_EX_DLGFRAME
						break;
					case BorderStyle.FixedSingle:
						cp.Style |= 0x800000; // WS_BORDER
						break;
				}
				return cp;
			}
		}
		/// <summary>
		/// Hides ScrollControlIntoView method.
		/// </summary>
		/// <param name="ctrl">Control.</param>
		protected new void ScrollControlIntoView( Control ctrl )
		{
		}
		/// <summary>
		/// Changes drag effect to copy if some file is dragged over the control.
		/// </summary>
		/// <param name="drgevent">DragEventArgs.</param>
		protected override void OnDragOver( DragEventArgs drgevent )
		{
			base.OnDragOver( drgevent );
		}
		/// <summary>
		/// Inserts text from the dropped to the control file.
		/// </summary>
		/// <param name="drgevent">DragEventArgs.</param>
		protected override void OnDragDrop( DragEventArgs drgevent )
		{
			base.OnDragDrop( drgevent );
		}
		/// <summary>
		/// Paints the background of the control.
		/// </summary>
		/// <param name="pevent">PaintEventArgs.</param>
		protected override void OnPaintBackground( PaintEventArgs pevent )
		{
		}
		/// <summary>
		/// Realligns controls after relayouting control.
		/// </summary>
		/// <param name="levent">LayoutEventArgs.</param>
		[UIPermission( SecurityAction.Assert, Unrestricted = true, Window = UIPermissionWindow.AllWindows )]
		protected override void OnLayout( LayoutEventArgs levent )
		{
			if( levent.AffectedProperty != "Visible" )
			{
				UpdateHeightInSingleLineMode();

				base.OnLayout( levent );

				if( m_sizeOld != this.Size )
				{
					ReAlignAll();
					UpdateSizeInAutoSizeMode();
					m_sizeOld = Size;
				}
			}
			else
			{
				base.OnLayout( levent );
			}
		}
        
		/// <summary>
		/// Processes Windows messages.
		/// </summary>
		/// <param name="m">The Windows Message to process.</param>
		protected override void WndProc( ref Message m )
		{
			base.WndProc( ref m );
            if ((int)Msg.WM_SETTEXT == m.Msg)
            {
                string strLParam = System.Runtime.InteropServices.Marshal.PtrToStringAuto(m.LParam);                
                this.Text = strLParam;
            }

            if ((int)Msg.EM_REPLACESEL == m.Msg)
            {
                string strLParam = System.Runtime.InteropServices.Marshal.PtrToStringAuto(m.LParam);
                this.SelectedText = strLParam;
            }

            if ((int)Msg.WM_GETTEXT == m.Msg)
            {
                if(this.ActualText != null)
                    m.Result = (IntPtr)this.ActualText.Length;
                else
                    m.Result = (IntPtr)this.Text.Length;
            }

            if ((int)Msg.WM_GETTEXTLENGTH == m.Msg)
            {
                if (this.ActualText != null)
                    m.Result = (IntPtr)this.ActualText.Length;
                else
                    m.Result = (IntPtr)this.Text.Length;
            }

            if ((int)Msg.EM_LINEINDEX == m.Msg)
            {
                int line = (int)m.WParam;

                if (line < this.Lines.Length && line > -1)
                {
                    long pos = this.ConvertVirtualPositionToOffset(new Point(1,line+1));
                    m.Result = (IntPtr)pos;
                }
                else if (line == -1) 
                {
                    line = this.CurrentLine;
                    long pos = this.ConvertVirtualPositionToOffset(new Point(1, line+1));
                    m.Result = (IntPtr)pos;
                }
                else // Beyond the limit
                {
                    m.Result = (IntPtr)(-1);
                }
            }

			if( ( int )Msg.WM_NCPAINT == m.Msg && BorderStyle.FixedSingle == this.borderStyle && this.UseXPStyle )
			{
				IntPtr hDC = GDIAppi.GetDCEx(
							m.HWnd, m.WParam, DeviceContextValues.Window | DeviceContextValues.Cache | DeviceContextValues.IntersectRgn );
                
				if( ( IntPtr )0 != hDC )
				{
					// Bounds are updated after NcPaint. So API must be used.
					RECT wRECT = new RECT();
					WinAPI.GetWindowRect( this.Handle, out wRECT );

					Rectangle bounds = ( Rectangle )wRECT;

					if( XPStyle.XPThemesEnabled() )
					{
						XPStyle.Draw( m.HWnd, hDC, new Rectangle( 0, 0, bounds.Width, bounds.Height ), "ListView", 1, 1 );
					}
					else
					{
						Graphics g = Graphics.FromHdc( hDC );
						Rectangle rect = new Rectangle( 0, 0, bounds.Width - 1, bounds.Height - 1 );
						g.DrawRectangle( Pens.LightBlue, rect );
						g.Dispose();
					}

					GDIAppi.ReleaseDC( m.HWnd, hDC );
				}

				return;
			}
		}
		/// <summary>
		/// Sets the cursor that is displayed when the mouse pointer is over the control.
		/// </summary>
		public override Cursor Cursor
		{
			set
			{
				base.Cursor = value;
				edtCode.AllowMouseCursorChange = ( Object.ReferenceEquals( this.Cursor, Cursors.Arrow ) );
			}
		}

        /// <summary>
        /// Gets or sets a value indicating whether to render the content of the control in RightToLeft layout.
        /// </summary>
        [Description("Gets or sets a value indicating whether to render the content of the control in RightToLeft layout."), Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new bool RenderRightToLeft
        {
            get 
            {
                return edtCode.RightToLeft == System.Windows.Forms.RightToLeft.Yes;
            }
            set
            {
                if(this.RenderRightToLeft != value)
                {
                    edtCode.RightToLeft = value ? RightToLeft.Yes : RightToLeft.No;
                
                    edtCode.HScrollBar.RightToLeft = this.edtCode.VScrollBar.RightToLeft = edtCode.RightToLeft;
                    fakeEditControl1.RightToLeft = edtCode.RightToLeft;
                    fakeEditControl2.RightToLeft = edtCode.RightToLeft;
                    fakeEditControl3.RightToLeft = edtCode.RightToLeft;
                }
            }
        }

		// RTL is not supported. Fix for bug OT#6309.
		/// <summary>
		/// Use RenderRightToLeft instead.
		/// </summary>
		[Browsable( false )]
		public override RightToLeft RightToLeft
		{
			get
			{
				return RightToLeft.No;
			}
		}
		/// <summary>
		/// Overriden OnKeyDown
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if ((e.KeyData & Keys.Control) != Keys.None && (e.Modifiers & Keys.Shift) != Keys.None)
			{
				bool? b = IsRTLChangeRequired((int)e.KeyCode, (int)e.KeyValue);

				if (b.HasValue && EnableRTL)
				{
					if (b.Value)
						this.RenderRightToLeft = true;
					else
						this.RenderRightToLeft = false;
				}
			}

			base.OnKeyDown(e);
		}

		#endregion

		#region Event ReRaisers
		/// <summary>
		/// Raises ContextPromptUpdate event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_ContextPromptUpdate( object sender, ContextPromptUpdateEventArgs e )
		{
			if( ContextPromptUpdate != null )
			{
				ContextPromptUpdate( this, e );
			}
		}
		/// <summary>
		/// Raises when some context choice list item gets selected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_ContextChoiceItemSelected( IContextChoiceController sender, ContextChoiceItemSelectedEventArgs e )
		{
			if( ContextChoiceItemSelected != null )
				ContextChoiceItemSelected( sender, e );
		}
		/// <summary>
		/// Raises when context prompt selection has been changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_ContextPromptSelectionChanged( ContextPrompt sender, ContextPromptSelectionChangedEventArgs e )
		{
			if( ContextPromptSelectionChanged != null )
				ContextPromptSelectionChanged( sender, e );
		}
		/// <summary>
		/// Raises when text is to be changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_TextChanging( object sender, TextChangingEventArgs e )
		{
			if( TextChanging != null )
				TextChanging( this, e );
		}
        /// <summary>
        /// Raises, when line text has been changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void edtCode_LineChanged(object sender, TextChangedEventArgs e)
        {
            if (LineChanged != null)
            {
                LineChanged(this, e);
            }
        }
        /// <summary>
        /// Raises when line is to be inserted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void edtCode_LineInserted(object sender, LinesEventArgs e)
        {
            if (LineInserted != null)
                LineInserted(this, e);
        }
        /// <summary>
        ///  Raises when line is to be deleted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void edtCode_LineDeleted(object sender, LinesEventArgs e)
        {
            if (LineDeleted != null)
                LineDeleted(this, e);
        }
		/// <summary>
		/// Raises, when text has been changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_TextChanged( object sender, EventArgs e )
		{
			if( TextChanged != null )
				TextChanged( this, e );

			UpdateSizeInAutoSizeMode();
		}

        /// <summary>
        /// Raises Find event when word match is found via FindAndReplaceDialog Next button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void edtCode_Find(object sender, EventArgs e)
        {
            if (Find != null)
                Find(this, e);
        }

		/// <summary>
		/// Raises, when single line mode has been changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_SingleLineChanged( object sender, EventArgs e )
		{
			// Control can not be in split-view mode if SingleLineMode is on.

			if( SingleLineMode )
			{
				m_bOldVerticalSplitters = ShowVerticalSplitters;
				m_bOldHorizontalSplitters = ShowHorizontalSplitters;

				ShowVerticalSplitters = false;
				ShowHorizontalSplitters = false;
			}
			else
			{
				ShowVerticalSplitters = m_bOldVerticalSplitters;
				ShowHorizontalSplitters = m_bOldHorizontalSplitters;
			}

			if( SingleLineMode )
			{
				m_iHeightBeforeSingleLine = Height;
				UpdateHeightInSingleLineMode();
			}
			else
			{
				Height = m_iHeightBeforeSingleLine;
			}

			if( SingleLineChanged != null )
				SingleLineChanged( sender, e );
		}
		/// <summary>
		/// Raises, when user should fill menu with menu items.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_MenuFill( object sender, EventArgs e )
		{
			if( MenuFill != null )
				MenuFill( sender, e );
		}
		/// <summary>
		/// Raises when text should be updated.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_UpdateContextToolTip( object sender, UpdateTooltipEventArgs e )
		{
			if( UpdateContextToolTip != null )
				UpdateContextToolTip( sender, e );
		}
		/// <summary>
		/// Raises when bookmark tooltip text should be updated.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_UpdateBookmarkToolTip( object sender, UpdateBookmarkTooltipEventArgs e )
		{
			if( UpdateBookmarkToolTip != null )
				UpdateBookmarkToolTip( sender, e );
		}
		/// <summary>
		/// Raises when context prompt should be shown.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_ContextPromptClose( object sender, ContextPromptCloseEventArgs e )
		{
			if( ContextPromptClose != null )
				ContextPromptClose( sender, e );

		}
		/// <summary>
		/// Raises when context prompt should be shown.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_ContextPromptOpen( object sender, ContextPromptUpdateEventArgs e )
		{
			if( ContextPromptOpen != null )
				ContextPromptOpen( sender, e );
		}
		/// <summary>
		/// Raises before context prompt should be shown.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_ContextPromptBeforeOpen( object sender, CancelEventArgs e )
		{
			if( ContextPromptBeforeOpen != null )
				ContextPromptBeforeOpen( sender, e );
		}
		/// <summary>
		/// Raises when auto-complete dialog has been opened.
		/// </summary>
		/// <param name="controller">"Controller for the context choice"</param>
		private void edtCode_ContextChoiceOpen( IContextChoiceController controller )
		{
			if( ContextChoiceOpen != null )
				ContextChoiceOpen( controller );
		}
		/// <summary>
		/// Raises when auto-complete dialog should be updated.
		/// </summary>
		/// <param name="controller">"Controller for the context choice"</param>
		private void edtCode_ContextChoiceUpdate( IContextChoiceController controller )
		{
			if( ContextChoiceUpdate != null )
				ContextChoiceUpdate( controller );
		}
		/// <summary>
		/// Raises before the ContextChoice dialog is shown to user.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_ContextChoiceBeforeOpen( object sender, CancelEventArgs e )
		{
			if( ContextChoiceBeforeOpen != null )
				ContextChoiceBeforeOpen( sender, e );
		}
		/// <summary>
		/// Raises when line mark should be drawn.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_DrawLineMark( object sender, DrawLineMarkEventArgs e )
		{
			if( DrawLineMark != null )
				DrawLineMark( sender, e );
		}
		/// <summary>
		/// Raises when line number are drawn
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void edtCode_OnBeforeLineNumberPaint(object sender, LineNumberPaintEventArgs e)
		{
			if (this.BeforeLineNumberPaint != null)
				BeforeLineNumberPaint(this, e);
		}
		/// <summary>
		/// Raises after changing parsers language.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_LanguageChanged( object sender, EventArgs e )
		{
			if( LanguageChanged != null )
				LanguageChanged( sender, e );
		}
		/// <summary>
		/// Raises when registers additional commands for key-binder.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_RegisteringKeyCommands( object sender, EventArgs e )
		{
			RegisterCommands();
		}
		/// <summary>
		/// Raises when registers additional key-bindings.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_RegisteringDefaultKeyBindings( object sender, EventArgs e )
		{
			RegisterBindings();
		}
		/// <summary>
		/// Raises when ReadOnly mode changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_ReadOnlyChanged( object sender, EventArgs e )
		{
			if( ReadOnlyChanged != null )
				ReadOnlyChanged( sender, e );

			string text = string.Empty;
			if( edtCode.ReadOnly ) text =Localizer.GetString( Localizer.EditResourceIdentifiers.DEF_STATUSBAR_READONLY);
			else text = Localizer.GetString(Localizer.DEF_STATUSBAR_NOT_READONLY);

			StatusBarSettings.StatusPanel.Panel.Text = text;
		}
		/// <summary>
		/// Raises when current stream instance is to be changed to some other one.
		/// </summary>
		private void edtCode_ChangingStream( ref bool processed )
		{
			if( ChangingStream != null )
				ChangingStream( ref processed );
		}
		/// <summary>
		/// Raises on the end of the long operation.
		/// </summary>
		/// <param name="operation">Long operation</param>
		private void edtCode_OperationStopped( ILongOperation operation )
		{
			if( OperationStopped != null )
				OperationStopped( operation );
		}
		/// <summary>
		/// Raises on the start of the long operation.
		/// </summary>
		/// <param name="operation">"Long operation"</param>
		private void edtCode_OperationStarted( ILongOperation operation )
		{
			if( OperationStarted != null )
				OperationStarted( operation );
		}

		/// <summary>
		/// Raises when user margin have to be painted.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_PaintUserMargin( object sender, PaintEventArgs e )
		{
			if( PaintUserMargin != null )
				PaintUserMargin( sender, e );
		}

		/// <summary>
		/// Raises when Changed State was updated.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_CanUndoRedoChanged( object sender, EventArgs e )
		{
			if( CanUndoRedoChanged != null )
				CanUndoRedoChanged( sender, e );
		}

		/// <summary>
		/// Raises when text selection has been changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_SelectionChanged( object sender, EventArgs e )
		{
			if( SelectionChanged != null )
				SelectionChanged( sender, e );
		}
		/// <summary>
		/// Raises when current cursor position has changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_CursorPositionChanged( object sender, EventArgs e )
		{
			StatusBarSettings.CoordsPanel.Panel.Text = string.Format(
				Localizer.GetString(Localizer.DEF_STATUSBAR_POSITION)
				, edtCode.Parser.GetLine( edtCode.CurrentLine ).LineStartPoint.Line.ToString()
				, edtCode.VisualColumn );
			if( CursorPositionChanged != null )
				CursorPositionChanged( sender, e );
		}
		/// <summary>
		/// Raises when InsertMode flag has changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_InsertModeChanged( object sender, EventArgs e )
		{
			if( InsertModeChanged != null )
				InsertModeChanged( sender, e );

			string text = string.Empty;

			if( edtCode.InsertMode ) text = Localizer.GetString(Localizer.DEF_STATUSBAR_INSERT);
			else text = Localizer.GetString(Localizer.DEF_STATUSBAR_OVERWRITE);

			StatusBarSettings.InsertPanel.Panel.Text = text;
		}
		/// <summary>
		/// Calls OnMouseDown.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_MouseDown( object sender, MouseEventArgs e )
		{
			OnMouseDown( e );
		}
		/// <summary>
		/// Calls OnMouseEnter.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_MouseEnter( object sender, EventArgs e )
		{
			OnMouseEnter( e );
		}
		/// <summary>
		/// Calls OnMouseLeave
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_MouseLeave( object sender, EventArgs e )
		{
			OnMouseLeave( e );
		}
		/// <summary>
		/// Calls OnMouseMove.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_MouseMove( object sender, MouseEventArgs e )
		{
			OnMouseMove( e );
		}
		/// <summary>
		/// Exchanges editor with fake, the mouse is over.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FakeEditorMouseMove( object sender, EventArgs e )
		{
			// Do not switch anywhere if editor is capturing mouse.
			if( !edtCode.Capture && !edtCode.m_codeSnippetsEditBox.Visible )
			{
				ExchangeWithFake( sender as FakeEditControl );
			}
		}
		/// <summary>
		/// Calls OnMouseHover.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_MouseHover( object sender, EventArgs e )
		{
			OnMouseHover( e );
		}
		/// <summary>
		/// Calls OnMouseDown.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_MouseUp( object sender, MouseEventArgs e )
		{
			OnMouseUp( e );
		}
		/// <summary>
		/// Calls OnMouseWheel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_MouseWheel( object sender, MouseEventArgs e )
		{
			OnMouseWheel( e );
		}
		/// <summary>
		/// Sets cursor.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EditControl_CursorChanged( object sender, EventArgs e )
		{
			edtCode.Cursor = this.Cursor;
		}
		/// <summary>
		/// Calls OnClick.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_Click( object sender, EventArgs e )
		{
			OnClick( e );
		}
		/// <summary>
		/// Calls OnDoubleClick.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_DoubleClick( object sender, EventArgs e )
		{
			OnDoubleClick( e );
		}
		/// <summary>
		/// Calls OnKeyDown.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_KeyDown( object sender, KeyEventArgs e )
		{
			OnKeyDown( e );
		}
		/// <summary>
		/// Calls OnKeyUp.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_KeyUp( object sender, KeyEventArgs e )
		{
			OnKeyUp( e );
		}
		/// <summary>
		/// Calls OnKeyPress.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_KeyPress( object sender, KeyPressEventArgs e )
		{
			OnKeyPress( e );
		}
		/// <summary>
		/// Paints splitter.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		[SecurityPermission( SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode )]
		private void splitterCenter_Paint( object sender, System.Windows.Forms.PaintEventArgs e )
		{
			Splitter splt = ( Splitter )sender;
			int centerX = splt.Width / 2;
			int centerY = splt.Height / 2;

			if( this.UseXPStyle )
			{
				if( XPStyle.XPThemesEnabled() )
				{
					if( splt.Width > splt.Height )
					{
						XPStyle.Draw( this.Handle, e.Graphics, splt.ClientRectangle, "TrackBar", 6, 1 );
					}
					else
					{
						XPStyle.Draw( this.Handle, e.Graphics, splt.ClientRectangle, "TrackBar", 3, 1 );
					}
				}
				else
				{
					Rectangle rect = splt.ClientRectangle;

					if( splt.Width > splt.Height )
					{
						BrushPaint.FillRectangle( e.Graphics, rect, m_brushHorSplitter );
						e.Graphics.DrawLine( Pens.Gray, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1 );
					}
					else
					{
						BrushPaint.FillRectangle( e.Graphics, rect, m_brushVertSplitter );
						e.Graphics.DrawLine( Pens.Gray, rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom );
					}
				}
			}
			else
			{
				ControlPaint.DrawBorder( e.Graphics, splt.ClientRectangle,
					Color.Gray, ButtonBorderStyle.Solid );
			}

			if( splt.Width > splt.Height )
			{
				for( int x = centerX - 10; x <= centerX + 10; x += 4 )
				{
					e.Graphics.FillRectangle( Brushes.Gray, x - 1, centerY - 1, 2, 2 );
					e.Graphics.FillRectangle( Brushes.White, x, centerY, 2, 2 );
				}
			}
			else
			{
				for( int y = centerY - 10; y <= centerY + 10; y += 4 )
				{
					e.Graphics.FillRectangle( Brushes.Gray, centerX - 1, y - 1, 2, 2 );
					e.Graphics.FillRectangle( Brushes.White, centerX, y, 2, 2 );
				}
			}
		}
		/// <summary>
		/// Raises ContextChoiceSelectedTextInsert event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_ContextChoiceSelectedTextInsert( IContextChoiceController sender, ContextChoiceTextInsertEventArgs e )
		{
			if( ContextChoiceSelectedTextInsert != null )
			{
				ContextChoiceSelectedTextInsert( sender, e );
			}
		}
		/// <summary>
		/// Raises IndicatorMarginClick 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_IndicatorMarginClick( object sender, IndicatorClickEventArgs e )
		{
			if( null != IndicatorMarginClick )
			{
				IndicatorMarginClick( this, e );
			}
		}
		/// <summary>
		/// Raises IndicatorMarginDoubleClick event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_IndicatorMarginDoubleClick( object sender, IndicatorClickEventArgs e )
		{
			if( null != IndicatorMarginDoubleClick )
			{
				IndicatorMarginDoubleClick( this, e );
			}
		}
		/// <summary>
		/// Raises DrawUserMarginText event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_DrawUserMarginText( object sender, DrawUserMarginTextEventArgs e )
		{
			if( null != DrawUserMarginText )
			{
				DrawUserMarginText( this, e );
			}
		}
		/// <summary>
		/// Raises SaveStreamWithDataLosing event.
		/// </summary>
		private void edtCode_SaveStreamWithDataLoss( object sender, SaveWithDataLosingEventArgs e )
		{
			if( null != SaveStreamWithDataLoss )
			{
				SaveStreamWithDataLoss( this, e );
			}
		}
		/// <summary>
		/// Raises SaveFileWithDataLosing event.
		/// </summary>
		private void edtCode_SaveFileWithDataLoss( object sender, SaveWithDataLosingEventArgs e )
		{
			if( null != SaveFileWithDataLoss )
			{
				SaveFileWithDataLoss( this, e );
			}
		}
		/// <summary>
		/// Raises StreamClose event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_StreamClose( object sender, StreamCloseEventArgs e )
		{
			if( null != Closing )
			{
				Closing( this, e );
			}
		}
		/// <summary>
		/// Raises OutliningTooltipBeforePopup event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_OutliningTooltipBeforePopup( object sender, OutliningTooltipBeforePopupEventArgs e )
		{
			if( null != OutliningTooltipBeforePopup )
			{
				OutliningTooltipBeforePopup( this, e );
			}
		}
		/// <summary>
		/// Raises OutliningTooltipPopup event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_OutliningTooltipPopup( object sender, CollapseEventArgs e )
		{
			if( null != OutliningTooltipPopup )
			{
				OutliningTooltipPopup( this, e );
			}
		}
		/// <summary>
		/// Raises OutliningTooltipClose event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_OutliningTooltipClose( object sender, CollapseEventArgs e )
		{
			if( null != OutliningTooltipClose )
			{
				OutliningTooltipClose( this, e );
			}
		}
		/// <summary>
		/// Raises CodeSnippetActivating event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CodeSnippetActivatingReRaiser( object sender, CancellableCodeSnippetsEventArgs e )
		{
			if( CodeSnippetActivating != null ) CodeSnippetActivating( sender, e );
		}
		/// <summary>
		/// Raises CodeSnippetDeactivating event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CodeSnippetDeactivatingReRaiser( object sender, CodeSnippetsEventArgs e )
		{
			if( CodeSnippetDeactivating != null ) CodeSnippetDeactivating( sender, e );
		}
		/// <summary>
		/// Raises CodeSnippetTemplateTextChanging event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CodeSnippetTemplateTextChangingReRaiser( object sender, CodeSnippetTemplateTextChangingEventArgs e )
		{
			if( CodeSnippetTemplateTextChanging != null ) CodeSnippetTemplateTextChanging( sender, e );
		}
		/// <summary>
		/// Raises NewSnippetMemberHighlighting event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NewSnippetMemberHighlightingReRaiser( object sender, NewSnippetMemberHighlightingEventArgs e )
		{
			if( NewSnippetMemberHighlighting != null ) NewSnippetMemberHighlighting( sender, e );
		}
		#endregion

		#region ShouldSerialize & Reset Methods
		/// <summary>
		/// Tells designer to serialize ColumnGuideItems.
		/// </summary>
		/// <returns></returns>
		[EditorBrowsable( EditorBrowsableState.Never )]
		internal bool ShouldSerializeColumnGuideItems()
		{
			return this.ColumnGuideItems.Length > 0;
		}
		/// <summary>
		/// Resets ColumnGuideItems property value.
		/// </summary>
		/// <returns></returns>
		[EditorBrowsable( EditorBrowsableState.Never )]
		internal void ResetColumnGuideItems()
		{
			this.ColumnGuideItems = new ColumnGuideItem[] { };
		}
		/// <summary>
		/// Tells designer to serialize LineNumbersFont property.
		/// </summary>
		/// <returns></returns>
		[EditorBrowsable( EditorBrowsableState.Never )]
		internal bool ShouldSerializeLineNumbersFont()
		{
			return !( this.LineNumbersFont.Equals( Control.DefaultFont ) );
		}
		/// <summary>
		/// Resets LineNumbersFont property value.
		/// </summary>
		/// <returns></returns>
		[EditorBrowsable( EditorBrowsableState.Never )]
		internal void ResetLineNumbersFont()
		{
			this.LineNumbersFont = ( Font )Control.DefaultFont.Clone();
		}
		/// <summary>
		/// Tells designer to serialize TabStops array.
		/// </summary>
		/// <returns></returns>
		[EditorBrowsable( EditorBrowsableState.Never )]
		internal bool ShouldSerializeTabStopsArray()
		{
			int[] arr = this.TabStopsArray;
			int len = arr.Length;

			if( len != StreamEditControl.DEF_ARR_TAB_STOPS.Length ) return true;

			for( int i = 0; i < len; i++ )
			{
				if( arr[ i ] != StreamEditControl.DEF_ARR_TAB_STOPS[ i ] ) return true;
			}

			return false;
		}
		/// <summary>
		/// Resets TabStopsArray property value.
		/// </summary>
		/// <returns></returns>
		[EditorBrowsable( EditorBrowsableState.Never )]
		internal void ResetTabStopsArray()
		{
			this.TabStopsArray = StreamEditControl.DEF_ARR_TAB_STOPS;
		}
		/// <summary>
		/// Tells designer to serialize UserMarginTextFont property.
		/// </summary>
		/// <returns></returns>
		[EditorBrowsable( EditorBrowsableState.Never )]
		internal bool ShouldSerializeUserMarginTextFont()
		{
			return this.UserMarginTextFont == Control.DefaultFont;
		}
		internal bool ShouldSerializeIndicatorMarginBackColor()
		{
			return this.IndicatorMarginBackColor != Color.Empty;
		}
		/// <summary>
		/// Resets UserMarginTextFont property value.
		/// </summary>
		/// <returns></returns>
		[EditorBrowsable( EditorBrowsableState.Never )]
		internal void ResetUserMarginTextFont()
		{
			this.UserMarginTextFont = ( Font )Control.DefaultFont.Clone();
		}
		/// <summary>
		/// Tells designer to serialize AllowDrop property.
		/// </summary>
		/// <returns></returns>
		[EditorBrowsable( EditorBrowsableState.Never )]
		internal bool ShouldSerializeAllowDrop()
		{
			return true;
		}
		/// <summary>
		/// Resets AllowDrop property value.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		internal void ResetAllowDrop()
		{
			AllowDrop = false;
		}
		/// <summary>
		/// Tells designer to serialize Text property.
		/// </summary>
		/// <returns></returns>
		[EditorBrowsable( EditorBrowsableState.Never )]
		internal bool ShouldSerializeText()
		{
			return true;
		}
		/// <summary>
		/// Resets text property value.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		internal new void ResetText()
		{
			Text = Name;
		}
		/// <summary>
		/// Tells designer to serialize file extensions
		/// </summary>
		/// <returns></returns>
		[EditorBrowsable( EditorBrowsableState.Never )]
		internal bool ShouldSerializeFileExtensions()
		{
			bool result = true;
			string[] exts = this.FileExtensions;
			if( exts.Length == StreamEditControl.DEF_ARR_FILE_EXTENSIONS.Length )
			{
				string[] arr1 = ( string[] )this.FileExtensions.Clone();
				string[] arr2 = ( string[] )StreamEditControl.DEF_ARR_FILE_EXTENSIONS.Clone();
				Array.Sort( arr1 );
				Array.Sort( arr2 );
				bool bDif = false;
				for( int i = 0; i < arr1.Length; i++ )
				{
					if( arr1[ i ] != arr2[ i ] )
					{
						bDif = true;
						break;
					}
				}
				result = bDif;
			}
			return result;
		}
		/// <summary>
		/// Reset file extensions
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		internal void ResetFileExtensions()
		{
			this.FileExtensions = ( string[] )StreamEditControl.DEF_ARR_FILE_EXTENSIONS.Clone();
		}
		#endregion

		#region ISupportInitialize Members
		/// <summary>
		/// Stub for compatibility reasons.
		/// </summary>
		public void BeginInit()
		{
		}
		/// <summary>
		/// Performs operations needed after initialization.
		/// </summary>
		public void EndInit()
		{
			edtCode.ResetUndoInfo();

			if( !m_bCommandsRegistered )
			{
				RegisterCommands();
				RegisterBindings();
				m_bCommandsRegistered = true;
			}
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Registers additional commands for key-binder.
		/// </summary>
		/// <param></param>
		/// <param></param>
		private void RegisterCommands()
		{
			edtCode.Commands.Add( "File.Save" ).ProcessCommand += new ProcessCommandEventHandler( SaveCommand );
			edtCode.Commands.Add( "File.Open" ).ProcessCommand += new ProcessCommandEventHandler( OpenCommand );
			edtCode.Commands.Add( "File.SaveAs" ).ProcessCommand += new ProcessCommandEventHandler( SaveAsCommand );
			edtCode.Commands.Add( "File.New" ).ProcessCommand += new ProcessCommandEventHandler( NewCommand );

			if( RegisteringKeyCommands != null )
			{
				RegisteringKeyCommands( this, EventArgs.Empty );
			}
		}
		/// <summary>
		/// Registers additional key-bindings.
		/// </summary>
		/// <param></param>
		/// <param></param>
		private void RegisterBindings()
		{
			edtCode.KeyBinder.BindToCommand( Keys.Control | Keys.S, "File.Save" );
			edtCode.KeyBinder.BindToCommand( Keys.Control | Keys.S | Keys.Shift, "File.SaveAs" );
			edtCode.KeyBinder.BindToCommand( Keys.Control | Keys.N, "File.New" );
			edtCode.KeyBinder.BindToCommand( Keys.Control | Keys.O, "File.Open" );
			edtCode.KeyBinder.BindToCommand( Keys.F9, "BookMarks.SetCustom" );

			if( RegisteringDefaultKeyBindings != null )
			{
				RegisteringDefaultKeyBindings( this, EventArgs.Empty );
			}
		}
		/// <summary>
		/// Switches between fake editor and real editor.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void fakeEditControl2_MouseDown( object sender, System.Windows.Forms.MouseEventArgs e )
		{
			FakeEditControl senderFake = ( FakeEditControl )sender;
			Point mousePoint = new Point( e.X - senderFake.AutoScrollPosition.X, e.Y - senderFake.AutoScrollPosition.Y );
			ExchangeWithFake( senderFake, mousePoint );
		}
		/// <summary>
		/// Switches to non-zero-size editor.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_Resize( object sender, EventArgs e )
		{
			if( ( edtCode.Width == 0 || edtCode.Height == 0 ) && Visible )
			{
				if( fakeEditControl1.Width != 0 || fakeEditControl1.Height != 0 ) ExchangeWithFake( fakeEditControl1 );
				if( fakeEditControl2.Width != 0 || fakeEditControl2.Height != 0 ) ExchangeWithFake( fakeEditControl2 );
				if( fakeEditControl3.Width != 0 || fakeEditControl3.Height != 0 ) ExchangeWithFake( fakeEditControl3 );
			}
		}
		/// <summary>
		/// Handles changes of WordWrapping.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_WordWrapChanged( object sender, EventArgs e )
		{
			if( WordWrapChanged != null )
			{
				WordWrapChanged( sender, e );
			}

			UpdateVerticalSplitters();
		}
		/// <summary>
		/// Handles dragging over control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FakeEditorDragOver( object sender, DragEventArgs e )
		{
			if( edtCode.CheckForSupportedData( e.Data ) )
			{
				e.Effect = DragDropEffects.Copy & e.AllowedEffect;
			}
		}
		/// <summary>
		/// Handles dropping of the object to control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FakeEditorDragDrop( object sender, DragEventArgs e )
		{
			if( edtCode.CheckForSupportedData( e.Data ) )
			{
				FakeEditControl senderFake = ( FakeEditControl )sender;

				Point point = senderFake.PointToClient( new Point( e.X, e.Y ) );
				Point mousePoint = new Point( point.X - senderFake.AutoScrollPosition.X, point.Y - senderFake.AutoScrollPosition.Y );
				ExchangeWithFake( senderFake, mousePoint );

				edtCode.PasteData( e.Data );
			}
		}
		/// <summary>
		/// Calls OnDragOver method.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_DragOver( object sender, DragEventArgs e )
		{
			OnDragOver( e );
		}
		/// <summary>
		/// Calls OnDragDrop method.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_DragDrop( object sender, DragEventArgs e )
		{
			OnDragDrop( e );
		}
		/// <summary>
		/// Calls OnDragEnter method.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_DragEnter( object sender, DragEventArgs e )
		{
			OnDragEnter( e );
		}
		/// <summary>
		/// Calls OnDragLeave method.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_DragLeave( object sender, EventArgs e )
		{
			OnDragLeave( e );
		}
		/// <summary>
		/// Reallignes all controls.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		[UIPermission( SecurityAction.Assert, Unrestricted = true, Window = UIPermissionWindow.AllWindows )]
		private void VerticalSplitterMoved( object sender, System.Windows.Forms.SplitterEventArgs e )
		{
			if( EventsHandlingLocked ) return;

			LockEvents();

			if( WordWrap )
			{
				int posTop = CalculateVerticalSplitterPosition( splitterTop.SplitPosition );
				int posBottom = CalculateVerticalSplitterPosition( splitterBottom.SplitPosition );

				if( sender == splitterBottom )
				{
					SetSplitterPosition( splitterTop, posBottom );
					SetSplitterPosition( splitterBottom, posBottom );
				}
				else
				{
					SetSplitterPosition( splitterTop, posTop );
					SetSplitterPosition( splitterBottom, posTop );
				}
			}

			ReAlignAll();
			UnlockEvents();
		}
		/// <summary>
		/// Corrects position in wordwrap mode.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void VerticalSplitterMoving( object sender, System.Windows.Forms.SplitterEventArgs e )
		{
			e.SplitX = CalculateVerticalSplitterPosition( e.SplitX );

			if( Math.Abs( e.X - this.Width / 2 ) < DEF_SPLITTERS_DOCK_DISTANCE )
				e.SplitX = this.Width / 2;
		}
		/// <summary>
		/// Stops timer and remeasures text.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MeasureTimerTick( object sender, EventArgs e )
		{
			// To prevent putting more tick event into the queue.
			m_timerUpdateWordWrap.Stop();

			if( m_controlExchangeWith != null )
			{
				ExchangeWithFake( m_controlExchangeWith );
				m_controlExchangeWith = null;
			}

			edtCode.RemeasureLinesWrapping();

			// To ensure that ReAllignAll in ExchangeWithFake did not started timer again.
			m_timerUpdateWordWrap.Stop();
		}
		/// <summary>
		/// Prints page footer and page number on it.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_PrintFooter( object sender, PrintHeadlineEventArgs e )
		{
			if( PrintPageNumber )
			{
				e.Text = Localizer.GetString(Localizer.DEF_PRINT_PAGE_PREFIX) + e.PageNumber.ToString();
			}

			e.Handled = false;

			if( PrintFooter != null )
			{
				PrintFooter( this, e );
			}

			if( !e.Handled )
			{
				if( null != e.Text && string.Empty != e.Text )
				{
					Rectangle rect = e.Rectangle;
					rect.Y = rect.Bottom - e.HeadlineHeight;
					rect.Height = e.HeadlineHeight;

					e.Graphics.DrawLine( Pens.Gray, rect.X, rect.Top, rect.Right, rect.Top );
					DrawStringCentered( e.Graphics, rect, e.Text );
				}
				else
				{
					e.HeadlineHeight = 0;
				}
			}
		}
		/// <summary>
		/// Prints page header and document name on it.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_PrintHeader( object sender, PrintHeadlineEventArgs e )
		{
			e.Handled = false;

			if( PrintDocumentName )
			{
				e.Text = ( string.Empty != edtCode.FileName ) ?
					(Localizer.GetString( Localizer.DEF_PRINT_FILE_PREFIX) + " " + edtCode.FileName ) : ( Localizer.GetString(Localizer.DEF_PRINT_NONAME_DOCUMENT) );
			}
			else
			{
				e.Text = string.Empty;
			}

			if( PrintHeader != null )
			{
				PrintHeader( this, e );
			}

			if( !e.Handled )
			{
				if( null != e.Text && string.Empty != e.Text )
				{
					Rectangle rect = e.Rectangle;
					rect.Height = e.HeadlineHeight;

					e.Graphics.DrawLine( Pens.Gray, rect.X, rect.Bottom, rect.Right, rect.Bottom );
					DrawStringCentered( e.Graphics, rect, e.Text );
				}
				else
				{
					e.HeadlineHeight = 0;
				}
			}
		}
		/// <summary>
		/// Disables control drawing.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_PaintLockRequest( object sender, EventArgs e )
		{
			pnlCommon.Visible = false;
		}
		/// <summary>
		/// Enables control drawing.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_PaintUnlockRequest( object sender, EventArgs e )
		{
			pnlCommon.Visible = true;
		}
		/// <summary>
		/// Subscribes for parser events.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_ParserCreated( object sender, EventArgs e )
		{
			OnParsingModeChanged();
			edtCode.Parser.OutliningBeforeCollapse += new OutliningCancellableEventHandler( Parser_OutliningBeforeCollapse );
			edtCode.Parser.OutliningBeforeExpand += new OutliningCancellableEventHandler( Parser_OutliningBeforeExpand );
			edtCode.Parser.OutliningCollapse += new OutliningEventHandler( Parser_OutliningCollapse );
			edtCode.Parser.OutliningExpand += new OutliningEventHandler( Parser_OutliningExpand );
		}
		/// <summary>
		/// Unsubscribes parser events.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_ParserDestroyed( object sender, EventArgs e )
		{
			OnParsingModeChanged();
			edtCode.Parser.OutliningBeforeCollapse -= new OutliningCancellableEventHandler( Parser_OutliningBeforeCollapse );
			edtCode.Parser.OutliningBeforeExpand -= new OutliningCancellableEventHandler( Parser_OutliningBeforeExpand );
			edtCode.Parser.OutliningCollapse -= new OutliningEventHandler( Parser_OutliningCollapse );
			edtCode.Parser.OutliningExpand -= new OutliningEventHandler( Parser_OutliningExpand );
		}
		/// <summary>
		/// Handler for the OutliningBeforeCollapse event of the collapsible region.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Parser_OutliningBeforeCollapse( object sender, OutliningEventArgs e )
		{
			if( null != OutliningBeforeCollapse ) OutliningBeforeCollapse( this, e );
		}
		/// <summary>
		/// Handler for the OutliningBeforeExpand event of the collapsible region.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Parser_OutliningBeforeExpand( object sender, OutliningEventArgs e )
		{
			if( null != OutliningBeforeExpand ) OutliningBeforeExpand( this, e );
		}
		/// <summary>
		/// Handler for the OutliningCollapse event of the collapsible region.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Parser_OutliningCollapse( object sender, CollapseEventArgs e )
		{
			if( null != OutliningCollapse ) OutliningCollapse( this, e );
		}
		/// <summary>
		/// Handler for the OutliningExpand event of the collapsible region.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Parser_OutliningExpand( object sender, CollapseEventArgs e )
		{
			if( null != OutliningExpand ) OutliningExpand( this, e );
		}
		/// <summary>
		/// Updates encoding info on the statusbar.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_EncodingChanged( object sender, EventArgs e )
		{
			UpdateStatusBarEncodingPanel();
		}
		/// <summary>
		/// Realigns edit control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void statusBar_VisibleChanged( object sender, System.EventArgs e )
		{
			if( !this.Disposing && this.SingleLineMode ) statusBar.Visible = false;
		}
		/// <summary>
		/// Chacks whether gripper should be shown.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_statusBarSettings_CheckSmartGripVisibility( object sender, GetBoolEventArgs e )
		{
			e.Value = edtCode.CheckForGripper( this );
		}
		/// <summary>
		/// Realigns all.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_statusBarSettings_VisibilityChanged( object sender, ValueChangedEventArgs e )
		{
			ReAlignAll();
		}
		/// <summary>
		/// Retrieves minimal width of fake controls.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_GetMinimalWidth( object sender, ChangeValueEventArgs e )
		{
			ArrayList values = new ArrayList();

			values.Add( e.Value );
			if( FakeVisible( fakeEditControl1 ) ) values.Add( fakeEditControl1.MaxWidth );
			if( FakeVisible( fakeEditControl2 ) ) values.Add( fakeEditControl2.MaxWidth );
			if( FakeVisible( fakeEditControl3 ) ) values.Add( fakeEditControl3.MaxWidth );

			int result = int.MaxValue;

			foreach( int i in values )
			{
				if( i < result ) result = i;
			}

			e.Value = result;
		}
		/// <summary>
		/// Puts splitter to the center of the control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void splitterCenter_DoubleClick( object sender, System.EventArgs e )
		{
			splitterCenter.SplitPosition = ( splitterCenter.SplitPosition == 0 )
				? this.Height / 2
				: 0;
		}
		/// <summary>
		/// Puts splitter to the center of the control or hides it.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void splitterVertical_DoubleClick( object sender, System.EventArgs e )
		{
			Splitter splitter = ( Splitter )sender;
			splitter.SplitPosition = ( splitter.SplitPosition == 0 )
				? this.Width / 2
				: 0;
		}
		/// <summary>
		/// Docks splitter to the center.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void splitterCenter_SplitterMoving( object sender, System.Windows.Forms.SplitterEventArgs e )
		{
			if( Math.Abs( e.Y - this.Height / 2 ) < DEF_SPLITTERS_DOCK_DISTANCE )
				e.SplitY = this.Height / 2;
		}
		/// <summary>
		/// Updates filename statusbar panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_FileNameChanged( object sender, EventArgs e )
		{
			if( !edtCode.Disposing )
			{
				StatusBarSettings.FileNamePanel.Panel.Text = Path.GetFileName( edtCode.DisplayFileName );
                if (edtCode.getFileName != null)
                    this.edtCode.FileName = StatusBarSettings.FileNamePanel.Panel.Text;
				statusBar.Invalidate( true );
			}
		}
		/// <summary>
		/// Updates filename statusbar panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void edtCode_ConfigurationChanged( object sender, EventArgs e )
		{
			if( ConfigurationChanged != null )
				ConfigurationChanged( sender, e );

			StatusBarSettings.FileNamePanel.Panel.Text = Path.GetFileName( edtCode.DisplayFileName );
            this.edtCode.FileName = StatusBarSettings.FileNamePanel.Panel.Text;
			statusBar.Invalidate( true );
		}
		/// <summary>
		/// Raises ContextChoiceRightClick event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnContextChoiceRightClick( IContextChoiceController sender, ContextChoiceItemEventArgs e )
		{
			if( ContextChoiceRightClick != null )
			{
				ContextChoiceRightClick( sender, e );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnEdtCodeScrollbarsSizeUpdated( object sender, EventArgs e )
		{
			m_scrollersFrame.SizeGripperVisibility =
				( edtCode.CheckForGripper( edtCode ) ) ? ( SizeGripperVisibility.Visible ) : ( SizeGripperVisibility.Hidden );
			m_scrollersFrame.Update();
			m_scrollersFrameForFake1.SizeGripperVisibility =
				( edtCode.CheckForGripper( fakeEditControl1 ) ) ? ( SizeGripperVisibility.Visible ) : ( SizeGripperVisibility.Hidden );
			m_scrollersFrameForFake1.Update();
			m_scrollersFrameForFake2.SizeGripperVisibility =
				( edtCode.CheckForGripper( fakeEditControl2 ) ) ? ( SizeGripperVisibility.Visible ) : ( SizeGripperVisibility.Hidden );
			m_scrollersFrameForFake2.Update();
			m_scrollersFrameForFake3.SizeGripperVisibility =
				( edtCode.CheckForGripper( fakeEditControl3 ) ) ? ( SizeGripperVisibility.Visible ) : ( SizeGripperVisibility.Hidden );
			m_scrollersFrameForFake3.Update();
		}
		#endregion
	}
	/// <summary>
	/// EventArgs used in painting the LineNumbers in Editcontrol.
	/// </summary>
	public class LineNumberPaintEventArgs : System.EventArgs
	{
		#region Fields
		private Graphics graphics;
		private Int64 lineNumber;
		private RectangleF bounds;
		private Color foreColor = Color.Empty;
		private Font lineNumbersFont;
		private LineNumberAlignment lineNumbersAlignment;
		private bool handled = false;
		private StringFormat stringFormat = new StringFormat(GraphicsUtils.DefaultFormat.FormatFlags);
		#endregion 

		#region Properties
		/// <summary>
		/// Gets the StringAlinment for the LineNumbers
		/// </summary>
		internal StringFormat StringFormat
		{
			get
			{
				return stringFormat;
			}
		}
		/// <summary>
		/// Gets or Sets the Graphics object used for the drawing.
		/// </summary>
		public Graphics Graphics
		{
			get { return graphics; }
			set { graphics = value; }
		}
		/// <summary>
		/// Gets or Sets the current LineNumber to be drawn.
		/// </summary>
		public Int64 LineNumber
		{
			get { return lineNumber; }
			set { lineNumber = value; }
		}
		/// <summary>
		/// Gets or Sets the bounds of the current LineNumber rectangle.
		/// </summary>
		public RectangleF Bounds
		{
			get { return bounds; }
			set { bounds = value; }
		}
		/// <summary>
		/// Gets or Sets the fore color of the current LineNumber.
		/// </summary>
		public Color ForeColor
		{
			get { return foreColor; }
			set { foreColor = value; }
		}
		/// <summary>
		/// Gets or Sets the font of the current LineNumber.
		/// </summary>
		public Font LineNumbersFont
		{
			get { return lineNumbersFont; }
			set { lineNumbersFont = value; }
		}
		/// <summary>
		/// Gets or Sets the LineNumbersAlignment of the current LineNumber.
		/// </summary>
		public LineNumberAlignment LineNumbersAlignment
		{
			get
			{
				return lineNumbersAlignment;
			}
			set
			{
				if (value == LineNumberAlignment.Left)
				{
					stringFormat.Alignment = StringAlignment.Near;
				}
				else
				{
					stringFormat.Alignment = StringAlignment.Far;
				}

				lineNumbersAlignment = value;
			}
		}
		/// <summary>
		/// Gets or Sets a value indicating whether the LineNumber drawing is to be cancelled.
		/// </summary>
		public bool Handled
		{
			get { return handled; }
			set { handled = value; }
		}
		#endregion

		#region Constructor
        /// <summary>
        /// Constructor for LineNumberPaintEventArgs
        /// </summary>
        /// <param name="g"></param>
        /// <param name="lineNumber"></param>
        /// <param name="lineNumbersFont"></param>
        /// <param name="foreColor"></param>
        /// <param name="lineNumbersAlignment"></param>
        /// <param name="bounds"></param>
        /// <param name="handled"></param>
		public LineNumberPaintEventArgs(Graphics g, Int64 lineNumber, Font lineNumbersFont, Color foreColor,
		LineNumberAlignment lineNumbersAlignment,RectangleF bounds, bool handled)
		{
			this.Graphics = g;
			this.LineNumber = lineNumber;
			this.LineNumbersFont = lineNumbersFont;
			this.ForeColor = foreColor;
			this.LineNumbersAlignment = lineNumbersAlignment;
			this.Bounds = bounds;
			this.Handled = handled;
		}
		#endregion
	}
}