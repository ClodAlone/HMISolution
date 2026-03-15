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
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;
using System.Threading;
using System.Security.Permissions;
using System.Security.Cryptography;

using Microsoft.Win32;

using Syncfusion.Shared.Utils.KeyBinding;
using Syncfusion.Shared.Utils.KeyBinding.Implementation;
using Syncfusion.Windows.Forms.Edit.Design;
using Syncfusion.Windows.Forms.Edit.Dialogs;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Implementation;
using Syncfusion.Windows.Forms.Edit.Implementation.Config;
using Syncfusion.Windows.Forms.Edit.Implementation.Formatting;
using Syncfusion.Windows.Forms.Edit.Implementation.IO;
using Syncfusion.Windows.Forms.Edit.Implementation.Parser;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Utils;
using Syncfusion.Windows.Forms.Edit.Utils.AutoFormatting;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Localization;
using Syncfusion.Windows.Forms.Edit.Forms.Popup;
using Syncfusion.IO;
using Syncfusion.Windows.Forms.Edit.Utils.CodeSnippets;
using Syncfusion.Windows.Forms.Edit.Dialogs.Options;

namespace Syncfusion.Windows.Forms.Edit
{
    /// <summary>
    /// Control for editing source files. Loads language configuration from file.
    /// </summary>
    [Designer(typeof(EditControlDesigner))]
    [ToolboxItem(false)]
    public class StreamEditControl
        : IntelliScrollableControl
        , IKeyBinderContainer
        , ILongOperationControllerInternal
        , ISupportInitialize
    {
        #region Classes
        /// <summary>
        /// Saved information about current view.
        /// </summary>
        protected class SavedViewInfo
        {
            #region Internal Classes
            /// <summary>
            /// Structure for storing two text range offsets.
            /// </summary>
            public struct TextRangeInfo
            {
                #region Public Fields
                /// <summary>
                /// Start offset.
                /// </summary>
                public long StartOffset;
                /// <summary>
                /// End offset.
                /// </summary>
                public long EndOffset;
                #endregion
            }
            #endregion

            #region Public Fields
            /// <summary>
            /// Collection of selection ranges.
            /// </summary>
            internal IList SelectionRanges = new ArrayList();
            /// <summary>
            /// Index of the screen top line.
            /// </summary>
            internal long TopLineStart;
            /// <summary>
            /// Offset of the screen top line.
            /// </summary>
            internal float TopLineOffset;
            /// <summary>
            /// Position of cursor.
            /// </summary>
            internal long CursorPosition;
            /// <summary>
            /// Visual location of selection start.
            /// </summary>
            internal VisualLocation SelectionVisualStart;
            /// <summary>
            /// Visual location of selection end.
            /// </summary>
            internal VisualLocation SelectionVisualEnd;
            #endregion
        }

        /// <summary>
        /// Type of intellisense.
        /// </summary>
        protected enum IntellisenseType
        {
            /// <summary>
            /// Context prompt.
            /// </summary>
            ContextPrompt,
            /// <summary>
            /// Context choice.
            /// </summary>
            ContextChoice
        }

        /// <summary>
        /// Info about visual location in a single line.
        /// </summary>
        internal class VisualLocation
        {
            #region Public Fields
            /// <summary>
            /// Index of lexem line.
            /// </summary>
            public int Line = 0;
            /// <summary>
            /// Index of subline of current location.
            /// </summary>
            public int SubLine = 0;
            /// <summary>
            /// Column offset in current subline.
            /// </summary>
            public int Offset = 0;
            #endregion

            #region Static Properties
            /// <summary>
            /// Gets empty visual location.
            /// </summary>
            public static VisualLocation Empty
            {
                get
                {
                    return new VisualLocation();
                }
            }
            #endregion

            #region Initialization
            /// <summary>
            /// Creates new instance of VisualLocation.
            /// </summary>
            public VisualLocation()
            {
            }
            /// <summary>
            /// Creates and initializes new instance of VisualLocation.
            /// </summary>
            /// <param name="line"></param>
            /// <param name="subLine"></param>
            /// <param name="offset"></param>
            public VisualLocation(int line, int subLine, int offset)
            {
                this.Line = line;
                this.SubLine = subLine;
                this.Offset = offset;
            }
            #endregion

            #region Public Methods
            /// <summary>
            /// Checks whether visual location is empty.
            /// </summary>
            /// <returns>true if visual location is empty; otherwise false.</returns>
            public bool IsEmpty()
            {
                return (this.Line == 0 && this.SubLine == 0 && this.Offset == 0);
            }
            #endregion

            #region Overrides
            /// <summary>
            /// Returns true if current and given VisualLocation instances are equal.
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                VisualLocation loc = (VisualLocation)obj;
                return (this.Line == loc.Line && this.SubLine == loc.SubLine && this.Offset == loc.Offset);
            }
            /// <summary>
            /// Gets hash code.
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return (this.Line ^ this.SubLine ^ this.Offset);
            }
            /// <summary>
            /// == operator.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static bool operator ==(VisualLocation x, VisualLocation y)
            {
                return x.Equals(y);
            }
            /// <summary>
            /// != operator.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public static bool operator !=(VisualLocation x, VisualLocation y)
            {
                return !x.Equals(y);
            }
            #endregion
        }

        /// <summary>
        /// Group of the actions that must be undone with one undo operation.
        /// </summary>
        private struct UndoGroup
        {
            #region Public Fields
            /// <summary>
            /// Starting count of actions in undo queue.
            /// </summary>
            public readonly int StartActionsCount;
            /// <summary>
            /// Ending count of actions in undo queue.
            /// </summary>
            public readonly int EndActionsCount;
            #endregion

            #region Properties
            /// <summary>
            /// Gets count of actions in undo queue which belongs to the group.
            /// </summary>
            public int GroupLength
            {
                get
                {
                    return EndActionsCount - StartActionsCount;
                }
            }

            #endregion

            #region Initialization
            /// <summary>
            /// Initializes structure by Start and End values.
            /// </summary>
            /// <param name="iStart">Starting count of actions in undo queue.</param>
            /// <param name="iEnd">Ending count of actions in undo queue.</param>
            public UndoGroup(int iStart, int iEnd)
            {
                if (iStart >= iEnd)
                    throw new ArgumentException(
                        Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_66);

                StartActionsCount = iStart;
                EndActionsCount = iEnd;
            }
            #endregion
        }

        /// <summary>
        /// Information about insert/delete operation.
        /// </summary>
        private struct OperationData
        {
            #region Public Fields
            /// <summary>
            /// True if operation is an insert operation.
            /// </summary>
            public bool Insert;
            /// <summary>
            /// Start line of the changes.
            /// </summary>
            public int StartLine;
            /// <summary>
            /// Start column of the changes.
            /// </summary>
            public int StartColumn;
            /// <summary>
            /// End line of the changes.
            /// </summary>
            public int EndLine;
            /// <summary>
            /// End column of the changes.
            /// </summary>
            public int EndColumn;
            /// <summary>
            /// Text that was inserted.
            /// </summary>
            public string InsertText;
            #endregion
        }

        /// <summary>
        /// Information about Indent Guideline Region.
        /// Graphical and virtual positions of the region start and end lexems.
        /// </summary>
        private class IndentGuidelineRegionInfo
        {
            #region Public Fields
            /// <summary>
            /// Rectangle, ocqupied by the start lexem of the region.
            /// </summary>
            public RectangleF RectLexemStart;
            /// <summary>
            /// Rectangle, ocqupied by the end lexem of the region.
            /// </summary>
            public RectangleF RectLexemEnd;
            /// <summary>
            /// Location of the start lexem.
            /// </summary>
            public CoordinatePoint PointStart;
            /// <summary>
            /// Location of the end lexem.
            /// </summary>
            public CoordinatePoint PointEnd;
            #endregion
        }
        #endregion

        #region Constants
        /// <summary>
        /// Specifies size of the area that precedes the text (on the left).
        /// </summary>
        internal const int DEF_PRE_TEXT_AREA = 3;
        /// <summary>
        /// String that consists only from the tabulation character.
        /// </summary>
        private const string DEF_STR_TAB_CHAR = "\t";
        /// <summary>
        /// Tab character.
        /// </summary>
        private const char DEF_TAB_CHAR = '\t';
        /// <summary>
        /// Maximum count of lexems that can be parsed while looking for guideline indentation block ends in parstial parsing mode.
        /// </summary>
        private const int DEF_MAX_LEXEMS_PARSE_ON_INDENT = 3000;
        /// <summary>
        /// Name of the resource with common movement cursor.
        /// </summary>
        private const string m_CursorSelectLineName = "Syncfusion.Windows.Forms.Edit.Images.LineSelection.cur";
        /// <summary>
        /// Regex conversion pattern.
        /// </summary>
        private const string DEF_CONVERT_REGEX = "(\n\r)|(\r\n)|(\n)|(\r)";
        /// <summary>
        /// Default Graphics object. Used for measuring.
        /// </summary>
        private static readonly Graphics _graphics = Graphics.FromImage(new Bitmap(1, 1));
        /// <summary>
        /// Default lines count which is used when there are no lines in the file.
        /// </summary>
        private const int DEF_NUMBERS_SIZE = 999;
        /// <summary>
        /// Width of the collapsers area
        /// </summary>
        private const int DEF_COLLAPSE_AREA = 12;
        /// <summary>
        /// Width of the selection margin area.
        /// </summary>
        internal const int DEF_SELECTION_MARGIN_WIDTH = 5;
        /// <summary>
        /// Count of lines that will be loaded in one pass when the user is idle.
        /// </summary>
        private const int DEF_IDLE_LOAD_LINES_COUNT = 20;
        /// <summary>
        /// Delay in miliseconds between every idle-processing.
        /// </summary>
        private const int DEF_IDLE_TIMER_DELAY = 400;
        /// <summary>
        /// Default with of user margin
        /// </summary>
        private const int DEF_USER_MARGIN = 100;
        /// <summary>
        /// Name of the resource with XSL transformation for XML representation of the text.
        /// </summary>
        private const string DEF_XSL_TRANSFORM = "Syncfusion.Windows.Forms.Edit.OutPutConvert.xslt";
        /// <summary>
        /// Regular expression for checking selected text.
        /// </summary>
        private const string DEF_SMART_TEXT_CHECK = @"^(\w|\s|\d)+$";
        /// <summary>
        /// Step for scrolling when line by line scrolling is performed; performance has to be improved..
        /// </summary>
        internal const float DEF_LINE_SCROLLING_STEP_SLOW = 0.3f;
        /// <summary>
        /// Step for scrolling when line by line scrolling is performed; performance shouldn't be improved.
        /// </summary>
        internal const float DEF_LINE_SCROLLING_STEP_FAST = 7f;
        /// <summary>
        /// Name of the selection layer in dynamic formatting.
        /// </summary>
        private const string DEF_LAYER_NAME_SELECTION = "Selection";
        /// <summary>
        /// Name of the border layer in dynamic formatting.
        /// </summary>
        private const string DEF_LAYER_NAME_BORDER_DYNAMIC = "Border_Dyn";
        /// <summary>
        /// Name of the border layer in dynamic formatting.
        /// </summary>
        private const string DEF_LAYER_NAME_BORDER_OVER = "Border_Over";
        /// <summary>
        /// Name of the text color layer in dynamic formatting.
        /// </summary>
        private const string DEF_LAYER_NAME_TEXTCOLOR = "TextColor";
        /// <summary>
        /// Name of the readonly layer in dynamic formatting.
        /// </summary>
        private const string DEF_LAYER_NAME_READONLY = "ReadOnly";
        /// <summary>
        /// Name of the dynamic formats layer that contains wavelines.
        /// </summary>
        private const string DEF_LAYER_NAME_WAVELINE = "WaveLines";
        /// <summary>
        /// Name of the dynamic formats layer that contains wavelines.
        /// </summary>
        private const string DEF_LAYER_NAME_LINEBACKCOLOR_DYNAMIC = "LineBackColor_Dynamic";
        /// <summary>
        /// Name of the dynamic formats layer that contains wavelines.
        /// </summary>
        private const string DEF_LAYER_NAME_LINEBACKCOLOR_OVER = "LineBackColor_Over";
        /// <summary>
        /// Name of the dynamic formats layer that contains text strike outs.
        /// </summary>
        private const string DEF_LAYER_NAME_STRIKEOUTS = "StrikeOutsLayer";
        /// <summary>
        /// Maximum count of the unrendered lines that can be shown by the scrollbar.
        /// </summary>
        private const int DEF_MAX_UNRENDERED_LINES = 1000;
        /// <summary>
        /// Sleep interval for idle-processing timer.
        /// </summary>
        private const int DEF_IDLE_SLEEP_INTERVAL = 2000;
        /// <summary>
        /// Maximum count of lines in a tooltip.
        /// </summary>
        private const int DEF_TOOLTIP_MAX_LINES = 50;
        /// <summary>
        /// Offset for the text.
        /// </summary>
        private const int DEF_OFFSET_TEXT = 0;
        /// <summary>
        /// Height of the page header in percents.
        /// </summary>
        private const float DEF_HEADER_HEIGHT = 1.3f;
        /// <summary>
        /// Height of the page footer in percents.
        /// </summary>
        private const float DEF_FOOTER_HEIGHT = 1.3f;
        /// <summary>
        /// Offset between text and header or footer in percents.
        /// </summary>
        private const int DEF_HEADER_FOOTER_TEXT_OFFSET = 1;
        /// <summary>
        /// Line to remeasure before and after the line that was misplaced on rendering.
        /// </summary>
        private const int DEF_LINES_PRERENDER = 50;
        /// <summary>
        /// Default lines wrapping marking image.
        /// </summary>
        private const string DEF_DEFAULT_WRAP_MARK = "Syncfusion.Windows.Forms.Edit.Images.LineWrapMark.bmp";
        /// <summary>
        /// Default lines wrapping marking image.
        /// </summary>
        private const string DEF_DEFAULT_WRAP_MARK_RTL = "Syncfusion.Windows.Forms.Edit.Images.LineWrapMarkRTL.bmp";
        /// <summary>
        /// Default wrapped lines marking image.
        /// </summary>
        private const string DEF_DEFAULT_WRAPPED_LINE_MARK = "Syncfusion.Windows.Forms.Edit.Images.WrappedLineMark.bmp";
        /// <summary>
        /// Default wrapped lines marking image.
        /// </summary>
        private const string DEF_DEFAULT_WRAPPED_LINE_MARK_RTL = "Syncfusion.Windows.Forms.Edit.Images.WrappedLineMarkRTL.bmp";
        /// <summary>
        /// Default context prompt width.
        /// </summary>
        private const int DEF_CONTEXT_PROMPT_DEFAULT_WIDTH = 400;
        /// <summary>
        /// Default context prompt height.
        /// </summary>
        internal const int DEF_CONTEXT_PROMPT_DEFAULT_HEIGHT = 50;
        /// <summary>
        /// Default array of tab stops.
        /// </summary>
        internal static readonly int[] DEF_ARR_TAB_STOPS = new int[] { 8, 16, 24, 32, 40 };
        /// <summary>
        /// Extensions of files able to be dropped into EditControl.
        /// </summary>
        internal static readonly string[] DEF_ARR_FILE_EXTENSIONS =
            new string[] { ".cs", ".vb", ".sql", ".xml", ".java", ".pas", ".html", ".htm", ".vbs", ".js" };
        /// <summary>
        /// Delay of long operation timer (for mouse cursor changing).
        /// </summary>
        private int DEF_LONGOP_TIMER_DELAY = 1000;
        /// <summary>
        /// Dummy for marking snippet end point.
        /// </summary>
        private const string DEF_END_SNIPPET_MEMBER_MARK = "-~-end-~-";
        /// <summary>
        /// "\n".
        /// </summary>
        private const string STR_NEW_LINE = "\n";
        /// <summary>
        /// Number of pixels which are added to selection when ExtendSelectionToFarRight is set to false.
        /// </summary>
        private const int PIXEL_TO_ADD_TO_SELECTION = 8;
        /// <summary>
        /// Default number of pixels between lines.
        /// </summary>
        internal const int SPACE_BETWEEN_LINES = 1;
        /// <summary>
        /// Number of pixels to add to border when it's invalidated.
        /// </summary>
        private const int BORDER_INVALIDATION_PIXELS = 5;
        /// <summary>
        /// If distance from mouse down point and left edge of control is less than this value, control is scrolled horizontally.
        /// </summary>
        private const int OFFSET_TO_SCROLL = 15;
        /// <summary>
        /// Minimum value for tab size.
        /// </summary>
        internal const int MIN_TAB_SIZE = 1;
        /// <summary>
        /// Maximum value for tab size.
        /// </summary>
        internal const int MAX_TAB_SIZE = 60;
        #endregion

        #region Static Fields
        /// <summary>
        /// Common movement cursor.
        /// </summary>
        private static Cursor _cursorLineSelection;
        /// <summary>
        /// For calculating hashes.
        /// </summary>
        private static MD5 _md5;
        #endregion

        #region Fields
        /// <summary>
        /// Search the text like in visual studio editor
        /// </summary>
        private bool m_likeVSStudioSearch = false;
        /// <summary>
        /// Indicates whether text was modified after the last save.
        /// </summary>
        protected bool m_bModified;
        /// <summary>
        /// True when parent form is being closed.
        /// </summary>
        protected bool m_bClosing = false;
        /// <summary>
        /// List of parse points for marking lines that were changed and saved.
        /// </summary>
        protected ArrayList m_savedLinesPoints;
        /// <summary>
        /// Specifies the arguments use last time the outlining tooltip was shown. If last tooltip was not outlinign tooltip, this member is set to null.
        /// </summary>
        private CollapseEventArgs m_argsLastOuliningTooltip;
        /// <summary>
        /// Current configuration.
        /// </summary>
        private Config m_Configuration;
        /// <summary>
        /// Flag of the InsertMode.
        /// InsertedMode is turned on by default.
        /// </summary>
        private bool m_bInsertMode = true;
        /// <summary>
        /// Counter for the update locks.
        /// </summary>
        private int m_updateLocks;
        /// <summary>
        /// Input stream.
        /// </summary>
        private StreamsWrapper m_wrapper;
        /// <summary>
        /// Lexem parser instance.
        /// </summary>
        internal RenderableLexemParser m_parser;
        /// <summary>
        /// StringFormat instance used for drawing and measuring strings, verticaly centered.
        /// </summary>
        private StringFormat m_centFormat = new StringFormat(GraphicsUtils.DefaultFormat.FormatFlags);
        /// <summary>
        /// Flag of line numbers showing.
        /// </summary>
        private bool m_bShowLines;
        /// <summary>
        ///Flag of markers showing.
        /// </summary>
        private bool m_bShowMarkers;
        /// <summary>
        /// Flag of collapsers showing.
        /// </summary>
        private bool m_bShowCollapse = true;
        /// <summary>
        /// Determines whether KeyPress was handled.
        /// </summary>
        private bool m_bKeyPressHandled;
        /// <summary>
        /// Timer based on other thread, used to scan file while user is idle.
        /// </summary>
        private System.Windows.Forms.Timer m_scanTimer_Callback;
        /// <summary>
        /// Manager of dynamic formatting.
        /// </summary>
        private DynamicFormatManager m_formatManager;
        /// <summary>
        /// Selection range.
        /// </summary>
        private ComplexTextRange m_selection = new ComplexTextRange();
        /// <summary>
        /// Flag that indicates whether control currently is in selection mode.
        /// </summary>
        private bool m_bSelecting;
        /// <summary>
        /// Set to true when lexem is selected by double clicking on it. Used for fixing def. OT4736.
        /// </summary>
        private bool m_bDoubleClicked;
        /// <summary>
        /// Point of mouse position in moment of selecting lexem by double click. Used for fixing def. OT4736..
        /// </summary>
        private Point m_doubleClickedPoint;
        /// <summary>
        /// Specifies whether last mouse click was in text.
        /// </summary>
        private bool m_bClickedInText;
        /// <summary>
        /// Specifies whether last mouse click was in selection margin.
        /// </summary>
        private bool m_bClickedInSelectionMargin;
        /// <summary>
        /// Binder of the keyboard.
        /// </summary>
        private KeyProcessor m_keyBinder;
        /// <summary>
        /// Pen used to draw green dots.
        /// </summary>
        private Pen m_GreenDotsPen = new Pen(Color.DarkCyan);
        /// <summary>
        /// Find dialog.
        /// </summary>
        private IFindDialogForm m_findDialog;
        /// <summary>
        /// Goto dialog.
        /// </summary>
        private IGotoDialogForm m_gotoDialog;
        /// <summary>
        /// Replace dialog.
        /// </summary>
        private IReplaceDialogForm m_replaceDialog;
        /// <summary>
        /// ContextChoice controller.
        /// </summary>
        private ContextChoiceController m_controllerContextChoice;
        /// <summary>
        /// Context prompt window.
        /// </summary>
        private ContextPrompt m_contextPrompt;
        /// <summary>
        /// Quality of composite.
        /// </summary>
        private CompositingQuality m_CompositingQuality;
        /// <summary>
        /// Interpolation mode.
        /// </summary>
        private InterpolationMode m_InterpolationMode;
        /// <summary>
        /// Smoothing mode.
        /// </summary>
        private SmoothingMode m_SmoothingMode;
        /// <summary>
        /// Text rendering hint.
        /// </summary>
        private TextRenderingHint m_TextRenderingHint = TextRenderingHint.SystemDefault;
        /// <summary>
        /// Cursor manager.
        /// </summary>
        private CursorManager m_CursorManager;
        /// <summary>
        /// Specifies whether tab symbols must be used.
        /// </summary>
        private bool m_bUseTabs = true;
        /// <summary>
        /// Visibility of the user margin.
        /// </summary>
        private bool m_bShowUserMargin;
        /// <summary>
        /// With of user margin.
        /// </summary>
        private int m_iUserMarginWidth = DEF_USER_MARGIN;
        /// <summary>
        /// Count of long operation currently started.
        /// </summary>
        private int m_operationLevel;
        /// <summary>
        /// Brush used to draw background.
        /// </summary>
        private Brush m_backgroundBrush;
        /// <summary>
        /// Specifies whether removing a read-only region is allowed.
        /// </summary>
        private bool allowDeleteReadOnlyRegion = false;
        /// <summary>
        /// Read-only mode.
        /// </summary>
        private bool m_bReadOnly;
        /// <summary>
        /// Stack of the undo groups (UndoGroup stucture).
        /// </summary>
        private Stack m_undoGroups = new Stack();
        /// <summary>
        /// Stack of the redo groups (UndoGroup stucture).
        /// </summary>
        private Stack m_redoGroups = new Stack();
        /// <summary>
        /// If true, it means that undo group was opened, no undo operations can be done, 
        /// all changes in group can be undone by a single undo operation.
        /// </summary>
        private int m_iUndoGroupOpened;
        /// <summary>
        /// Count of the action in the undo queue, stored when undo group was opened.
        /// </summary>
        private int m_iUndoGroupStart;
        /// <summary>
        /// Specifies whether group undo is on.
        /// </summary>
        private bool m_bGroupUndo = true;
        /// <summary>
        /// Group undo markers.
        /// </summary>
        private Stack m_UndoMarkers = new Stack();
        /// <summary>
        /// Group redo markers.
        /// </summary>
        private Stack m_RedoMarkers = new Stack();
        /// <summary>
        /// Last executed operation.
        /// </summary>
        private OperationData m_lastOperation;
        /// <summary>
        /// WordWrapping mode sign.
        /// </summary>
        private bool m_bWordWrapping;
        /// <summary>
        /// Old width of the control.
        /// </summary>
        private int m_iOldWidth = 0;
        /// <summary>
        /// Tab size.
        /// </summary>
        private int m_iTabSize = 2;
        /// <summary>
        /// Specifies how the control process horizontal scrolling.
        /// </summary>
        private ScrollMode vScrollMode = ScrollMode.Pixel;
        /// <summary>
        /// Whitespacs visibility.
        /// </summary>
        private bool m_bShowWhitespaces = false;
        /// <summary>
        /// Context tooltip.
        /// </summary>
        private ToolTipEx m_tip;
        /// <summary>
        /// Bookmarks tooltip.
        /// </summary>
        private ToolTipEx m_bookmarksTooltip;
        /// <summary>
        /// Context menu manager.
        /// </summary>
        private ContextMenuManager m_menuManager;
        /// <summary>
        /// Single line mode status.
        /// </summary>
        private bool m_bSingleLineEnabled;
        /// <summary>
        /// Number of the page to be printed.
        /// </summary>
        private int m_iPrintPageNumber;
        /// <summary>
        /// Y offset of the page to be printed.
        /// </summary>
        private int m_iPrintPageY;
        /// <summary>
        /// Text Region to be printed.
        /// </summary>
        private GraphicsPath m_regionAreaToPrint;
        /// <summary>
        /// Print document.
        /// </summary>
        private PrintDocument m_printDoc;
        /// <summary>
        /// Specifies whether context choice should be updated 
        /// when user types something.
        /// </summary>
        private bool m_bUpdateContextChoice = true;
        /// <summary>
        /// XP-Style transparent selection.
        /// </summary>
        private bool m_bTransparentSelection = true;
        /// <summary>
        /// List of the fake copies of the control.
        /// </summary>
        private IList m_listFakeCopies = new ArrayList();
        /// <summary>
        /// Specifies whether only current page should be printed.
        /// </summary>
        private bool m_bPrintCurrentPage;
        /// <summary>
        /// Number of the last underline.
        /// </summary>
        private int m_iUnderlineNumber = 0;
        /// <summary>
        /// Number of the last line background.
        /// </summary>
        private int m_iBackcolorNumber = 0;
        /// <summary>
        /// Indicates whether control accepts tabs.
        /// </summary>
        private bool m_bTransferFocusOnTab = false;
        /// <summary>
        /// Specifies whether selection margin is visible.
        /// </summary>
        private bool m_bSelectionMarginVisible = true;
        /// <summary>
        /// Color of the selection margin background.
        /// </summary>
        private Color m_colorSelectionMarginBackground = Color.Empty;
        /// <summary>
        /// Color of the selection margin foreground.
        /// </summary>
        private Color m_colorSelectionMarginForeground = Color.Red;
        /// <summary>
        /// Last line index, selected from selection margin.
        /// </summary>
        private int m_iLastLineInLineSelection = -1;
        /// <summary>
        /// Width of selection margin.
        /// </summary>
        private int m_iSelectionMarginWidth = DEF_SELECTION_MARGIN_WIDTH;
        /// <summary>
        /// List of the new-lines, that were deleted during switching to the singleline mode.
        /// </summary>
        private IList m_listNewLines = new ArrayList();
        /// <summary>
        /// Column index of the word start that was under cursor when context choice was opened last time.
        /// </summary>
        private int m_iContextChoiceLastWordColumn = -1;
        /// <summary>
        /// Column at witch ContextPrompt opening lexem starts.
        /// </summary>
        private int m_iContextPromptOpeningLexemColumn = -1;
        /// <summary>
        /// Specifies whether ContentDividers should be visible.
        /// </summary>
        private bool m_bShowContentDividers = true;
        /// <summary>
        /// Specifies whether Stop search at page end.
        /// </summary>
        private bool m_bWrapAroundSearch = true;
        /// <summary>
        /// Specifies whether IndentationGuidelines should be visible.
        /// </summary>
        private bool m_bShowIndentationGuidelines = true;
        /// <summary>
        /// Information about currently visible indentation Guideline.
        /// </summary>
        private IndentGuidelineRegionInfo m_indentGuidelineInfo = null;
        /// <summary>
        /// Color of the indent Guideline.
        /// </summary>
        private Color m_colorIndentLine = Color.Gray;
        /// <summary>
        /// Color of the text under selection.
        /// </summary>
        private Color m_selectionTextColor = Color.Blue;
        /// <summary>
        /// Color of the indent guid borders.
        /// </summary>
        private Color m_colorIndentBorders = Color.LightBlue;
        /// <summary>
        /// Timer for auto indent Guideline search.
        /// </summary>
        private System.Windows.Forms.Timer m_timerAutoIndent;
        /// <summary>
        /// Specifies whether indent Guidelines should be shown automatically.
        /// </summary>
        private bool m_bIndentAutoShow = true;
        /// <summary>
        /// Brush to be used for drawing background.
        /// </summary>
        private BrushInfo m_brushBackground = BrushInfo.Empty;
        /// <summary>
        /// Cache of the selected text.
        /// </summary>
        private string m_strOldSelectedText;
        /// <summary>
        /// Count of selection changes lock.
        /// </summary>
        private int m_iSelectionLockCount;
        /// <summary>
        /// Specifies whether guidelines can be highlighted or only guideline should be drawn.
        /// </summary>
        private bool m_bOnlyHighlightMatchingBraces;
        /// <summary>
        /// Last X coordinate of the cursor. 
        /// Used for navigation implementation similar with Word in non-virtualspace mode.
        /// </summary>
        private int m_lastCursorX = -1;
        /// <summary>
        /// Virtual space mode state.
        /// </summary>
        private bool m_bVirtualSpaceMode;
        /// <summary>
        /// List of the custom controls.
        /// </summary>
        private ArrayList m_CustomControls = new ArrayList();
        /// <summary>
        /// Value that shows whether user is currently dragging selected text.
        /// </summary>
        private bool m_bDraggingSelectedText;
        /// <summary>
        /// Text range that is currently dragged.
        /// </summary>
        private ComplexTextRange m_rangeDragging;
        /// <summary>
        /// Rectangle, that is drawn to shown the position at which dragged object will be inserted.
        /// </summary>
        private RectangleF m_rectDragOverPosition;
        /// <summary>
        /// Value that specifes whether user left-clicked in selected are.
        /// </summary>
        private bool m_bClickedInSelection;
        /// <summary>
        /// Position of the mouse for the moment when user has clicked in the selected area.
        /// </summary>
        private Point m_pointClickPosition;
        /// <summary>
        /// Value indicating that indicates whether page headers and footers should be printed outside the page margins.
        /// </summary>
        private bool m_bPrintHeaderFooterOutsideMargins = true;
        /// <summary>
        /// Value that indicates whether page header and footer should be printer.
        /// </summary>
        private bool m_bPrintHeaderFooter = true;
        /// <summary>
        /// Specifies whether word wrapping was on before printing.
        /// </summary>
        private bool m_bPrintWordWrapSetting;
        /// <summary>
        /// Specifies the WrapMode used before printing.
        /// </summary>
        private WordWrapMode m_PrintWordWrapModeSetting;
        /// <summary>
        /// Specifies whether control should react on configuration change notification.
        /// </summary>
        private int m_iSuppressConfigurationChangeNotification;
        /// <summary>
        /// Value that indicates whether event handlers are attached to 
        /// Move and Resize events of the parent controls.
        /// </summary>
        private int m_iLayoutEventAttaches;
        /// <summary>
        /// Bookmark management helper object.
        /// </summary>
        private BookmarksHelper m_bookmarkhelper;
        /// <summary>
        /// Properties of White space mode.
        /// </summary>
        private ShowWhiteSpaceProperties m_whitespaceProps = new ShowWhiteSpaceProperties();
        /// <summary>
        /// Style of margin border (in print preview).
        /// </summary>
        private FrameBorderStyle m_marginBorderStyle = FrameBorderStyle.None;
        /// <summary>
        /// Color of margin border (in print preview).
        /// </summary>
        private Color m_clrMarginBorder = Color.Empty;
        /// <summary>
        /// Weight of margin border line (in print preview).
        /// </summary>
        private BorderWeight m_marginBorderWeight = BorderWeight.Thin;
        /// <summary>
        /// Color of user margin border.
        /// </summary>
        private Color m_clrUserMarginBorder = Color.Black;
        /// <summary>
        /// BrushInfo object that is used when user margin is being drawn.
        /// </summary>
        private BrushInfo m_brushUserMargin = new BrushInfo(Color.BurlyWood);
        /// <summary>
        /// Default font of user margin text.
        /// </summary>
        private Font m_fontUserMarginText = (Font)Control.DefaultFont.Clone();
        /// <summary>
        /// Default color of user margin text.
        /// </summary>
        private Color m_clrUserMarginText = Color.Black;
        /// <summary>
        /// Temporary list of the collapsed regions.
        /// </summary>
        private IList m_listCollapsed = new ArrayList();
        /// <summary>
        /// Temporary list of the expanded regions.
        /// </summary>
        private IList m_listNotCollapsed = new ArrayList();
        /// <summary>
        /// Specifies count of the selection locks.
        /// </summary>
        private int m_iSelectionLocks;
        /// <summary>
        /// Pen to draw border of user margin area.
        /// </summary>
        private Pen m_penUserMarginAreaBorder = new Pen(Color.Black);
        /// <summary>
        /// Type of word wrapping.
        /// </summary>
        private WordWrapType m_wrapType = WordWrapType.WrapByWord;
        /// <summary>
        /// Width of text area.
        /// </summary>
        private int m_textAreaWidth = 600;
        /// <summary>
        /// Indicates whether text area should be shown.
        /// </summary>
        private bool m_bShowTextArea = false;
        /// <summary>
        /// Style of line that delimits text area.
        /// </summary>
        private DashStyle m_textAreaLineStyle = DashStyle.Dot;
        /// <summary>
        /// Color of line that delimits text area.
        /// </summary>
        private Color m_clrTextAreaLine = Color.Black;
        /// <summary>
        /// BrushInfo object that is used when area situated after text area is drawn.
        /// </summary>
        private BrushInfo m_brushAfterTextArea = new BrushInfo(Color.BlanchedAlmond);
        /// <summary>
        /// Pen to draw line that delimits text area.
        /// </summary>
        private Pen m_penTextAreaLine = new Pen(Color.Black);
        /// <summary>
        /// Mode of word wrapping.
        /// </summary>
        private WordWrapMode m_wrapMode = WordWrapMode.Control;
        /// <summary>
        /// Width of marker area.
        /// </summary>
        private int m_markerAreaWidth = 16;
        /// <summary>
        /// Array of tab stops.
        /// </summary>
        private int[] m_arrTabStops = DEF_ARR_TAB_STOPS;
        /// <summary>
        /// Indicates whether tab stops should be shown.
        /// </summary>
        private bool m_bUseTabStops = false;
        /// <summary>
        /// Indicates whether XP style should be used.
        /// </summary>
        private bool m_bUseXPStyle = true;
        /// <summary>
        /// Indicates whether XP style Border should be used.
        /// </summary>
        private bool m_bUseXPStyleBorder = true;
        /// <summary>
        /// Brush used to draw collapse icons when XP style is used but there's no XP themes available.
        /// </summary>
        private BrushInfo m_brushCollapseIcons = new BrushInfo(GradientStyle.BackwardDiagonal, new Color[] { Color.White, Color.Gray });
        /// <summary>
        /// Brush used to draw markers area when XP style is used but there's no XP themes available.
        /// </summary>
        private BrushInfo m_brushMarkersArea = new BrushInfo(GradientStyle.Horizontal, new Color[] { Color.White, Color.FromArgb(237, 227, 214) });
        /// <summary>
        /// Mode of auto indentation.
        /// </summary>
        private AutoIndentMode m_autoIndentMode = AutoIndentMode.None;
        /// <summary>
        /// Indicates whether context tooltips are shown.
        /// </summary>
        private bool m_bShowContextTooltip = true;
        /// <summary>
        /// Indicates whether bookmark tooltips are shown.
        /// </summary>
        private bool m_bShowBookmarkTooltips = true;
        /// <summary>
        /// Indicates whether outlining toltips are shown.
        /// </summary>
        private bool m_bShowOutliningTooltip = true;
        /// <summary>
        /// Indicates whether lines wrapping should be marked.
        /// </summary>
        private bool m_bMarkLineWrapping = true;
        /// <summary>
        /// Custom image that marks lines wrapping.
        /// </summary>
        private Image m_lineWrappingMarkingImage;
        /// <summary>
        /// Default image that marks lines wrapping.
        /// </summary>
        private Image m_lineWrappingDefautImage;
        /// <summary>
        /// Default image that marks lines wrapping in RTL.
        /// </summary>
        private Image m_lineWrappingDefautImageRTL;
        /// <summary>
        /// Indicates whether wrapped lines should be marked.
        /// </summary>
        private bool m_bMarkWrappedLines = true;
        /// <summary>
        /// Indicates whether changed lines should be marked.
        /// </summary>
        private bool m_bMarkChangedLines = false;
        /// <summary>
        /// Custom image that marks wrapped lines.
        /// </summary>
        private Image m_wrappedLinesMarkingImage;
        /// <summary>
        /// Default image that marks wrapped lines.
        /// </summary>
        private Image m_wrappedLinesDefautImage;
        /// <summary>
        /// Default image that marks wrapped lines in RTL.
        /// </summary>
        private Image m_wrappedLinesDefautImageRTL;
        /// <summary>
        /// Size of context prompt.
        /// </summary>
        private Size m_contextPromptSize = new Size(DEF_CONTEXT_PROMPT_DEFAULT_WIDTH, DEF_CONTEXT_PROMPT_DEFAULT_HEIGHT);
        /// <summary>
        /// Font of line numbers.
        /// </summary>
        private Font m_lineNumbersFont = (Font)Control.DefaultFont.Clone();
        /// <summary>
        /// Brush for line numbers.
        /// </summary>
        private SolidBrush m_lineNumbersBrush = new SolidBrush(Color.DarkBlue);
        /// <summary>                                                   
        /// Indicates whether custom context prompt size should be used.
        /// </summary>
        private bool m_bUseCustomSizeContextPrompt = false;
        /// <summary>
        /// Color of context prompt form border.
        /// </summary>
        private Color m_clrContextPromptBorder = Color.Black;
        /// <summary>
        /// Color of context tooltip form border.
        /// </summary>
        private Color m_clrContextTooltipBorder = Color.Black;
        /// <summary>
        /// Color of bookmark tooltip form border.
        /// </summary>
        private Color m_clrBookmarkTooltipBorder = Color.Black;
        /// <summary>
        /// Brush for context tooltip background.
        /// </summary>
        private BrushInfo m_contextTooltipBackgroundBrush = new BrushInfo(Color.LemonChiffon);
        /// <summary>
        /// Brush for bookmark tooltip background.
        /// </summary>
        private BrushInfo m_bookmarkTooltipBackgroundBrush = new BrushInfo(Color.LemonChiffon);
        /// <summary>
        /// Brush for context prompt background.
        /// </summary>
        private BrushInfo m_contextPromptBackgroundBrush = new BrushInfo(Color.LemonChiffon);
        /// <summary>
        /// Brush for context prompt background.
        /// </summary>
        private BrushInfo m_indentationBlockBackgroundBrush = new BrushInfo(Color.LightGray);
        /// <summary>
        /// Array of ColumnGuideItem objects.
        /// </summary>
        private ColumnGuideItem[] m_arrColumnGuideItems = new ColumnGuideItem[] { };
        /// <summary>
        /// Bool that indicates whether column guides should be drawn.
        /// </summary>
        private bool m_bShowColumnGuides = true;
        /// <summary>
        /// Font that is used while measuring position of column guides.
        /// </summary>
        private Font m_columnGuidesMeasuringFont = new Font("Courier New", 10);
        /// <summary>
        /// Style of new line of the newly created stream.
        /// </summary>
        private NewLineStyle m_defaultNewLineStyle = NewLineStyle.Control;
        /// <summary>
        /// Indicates whether indentation block borders should be drawn.
        /// </summary>
        private bool m_bShowIndentationBlockBorders = false;
        /// <summary>
        /// Style of indentation block border line.
        /// </summary>
        private FrameBorderStyle m_indentationBlockBorderStyle = FrameBorderStyle.Solid;
        /// <summary>
        /// Color of indentation block border line.
        /// </summary>
        private Color m_clrIndentationBlockBorder = Color.Gray;
        /// <summary>
        /// Brush for changed lines marking line.
        /// </summary>
        private Brush m_changedLinesMarkingLineBrush = new SolidBrush(Color.Yellow);
        /// <summary>
        /// Brush for saved lines marking line.
        /// </summary>
        private Brush m_savedLinesMarkingLineBrush = new SolidBrush(Color.Lime);
        /// <summary>
        /// Indicates whether outer file dragged and dropped into Edit Control
        /// should be inserted into current content.
        /// </summary>
        private bool m_bInsertDroppedFileIntoText = false;
        /// <summary>
        /// Indicates whether context choice list gets autocompleted when single lexem remains in the list.
        /// </summary>
        private bool m_bAutoCompleteSingleLexem = false;
        /// <summary>
        /// Offset of paragraphs.
        /// </summary>
        private int m_iParagraphOffset = 0;
        /// <summary>
        /// Offset of paragraphs.
        /// </summary>
        private int m_iWrappedLinesOffset = 0;
        /// <summary>
        /// Autoformatting manager.
        /// </summary>
        private AutoFormattingManager m_autoFormattingManager = new AutoFormattingManager();
        /// <summary>
        /// Exports text to different format.
        /// </summary>
        private Exporter m_exporter = new Exporter();
        /// <summary>
        /// Controller for work with context choice of code snippets.
        /// </summary>
        private CodeSnippetsPopupController m_codeSnippetsPopupController;
        /// <summary>
        /// Manager for work with code snippets.
        /// </summary>
        private CodeSnippetsManager m_codeSnippetsManager;
        /// <summary>
        /// Form that represents options and can be shown through context menu.
        /// </summary>
        private Form m_contextMenuOptionsForm;
        /// <summary>
        /// Image of the control. When not null, OnPaint should draw it.
        /// </summary>
        private Image m_controlImage;
        /// <summary>
        /// Specifies whether scrollers are disabled.
        /// </summary>
        private bool m_bDisableScrollers = false;
        /// <summary>
        /// Type of intellisense that was shown last time.
        /// </summary>
        private IntellisenseType m_IntellisenseType;
        /// <summary>
        /// Stores invalidation tracer.
        /// </summary>
        private InvalidationStackTracer m_tracerInvalidation;
        /// <summary>
        /// Indicates whether config file should be loaded. If set to false, default language is created from code.
        /// </summary>
        private bool m_bLoadConfigFile = true;
        /// <summary>
        /// Timer for long operations and cursor changing.
        /// </summary>
        private System.Threading.Timer threadingTimer;
        /// <summary>
        /// ID of the main thread.
        /// </summary>
        private uint m_mainThreadId;
        /// <summary>
        /// Extensions of files that can be dropped to EditControl.
        /// </summary>
        private string[] m_arrFileExtensions = DEF_ARR_FILE_EXTENSIONS;
        /// <summary>
        /// Indicates whether all files can be dropped to EditControl.
        /// If set to false, only files with extension contained in m_arrFileExtensions can be dropped.
        /// </summary>
        private bool m_DropAllFiles = false;
        /// <summary>
        /// Indicates whether all saved lines can be flushed.
        /// If set to false, saved lines will not be flushed.
        /// </summary>
        private bool m_FlushSavedLines = false;
        /// <summary>
        /// Indicates whether Files can be Autosaved in EditControl.
        /// If set to false, Files will not be saved automatically.
        /// </summary>
        private bool m_AutoSave = false;
        /// <summary>
        /// Indicates whether selection should be performed when shift button is pressed
        /// </summary>
        private bool m_bAllowShiftSelectionOld = true;
        /// <summary>
        /// Indicates whether autoreplace triggers should be used.
        /// </summary>
        private bool m_bUseAutoreplaceTriggers = true;
        /// <summary>
        /// If set to true, scroll info should be updated on first paint.
        /// </summary>
        private bool m_bUpdateScrollInfo = false;
        /// <summary>
        /// Indicates whether border should be drawn around active code snippets.
        /// </summary>
        private bool m_bDrawCodeSnippetBorder = false;
        /// <summary>
        /// Indicates whether current line should be highlighted.
        /// </summary>
        private bool m_bHighlightCurrentLine = false;
        /// <summary>
        /// Color of current line highlight.
        /// </summary>
        private Color m_clrCurrentLineHighlight = Color.Gray;
        /// <summary>
        /// Hashcode for string that was selected as block of text. In is pasted respectively to each line if hashcodes are equal.
        /// </summary>
        private byte[] m_blockHashcode;
        /// <summary>
        /// Font that is used while measuring position of column guides.
        /// </summary>
        private Font m_wordWrapColumnMeasuringFont = new Font("Courier New", 10);
        /// <summary>
        /// Column for wrapping text. Used when WordWrapMode is set to SpecifiedColumn.
        /// </summary>
        private int m_iWordWrapColumn = 100;
        /// <summary>
        /// Position of word wrap column.
        /// </summary>
        private int m_wordWrapColumnPos = -1;
        /// <summary>
        /// 
        /// </summary>
        internal CodeSnippetsEditBox m_codeSnippetsEditBox;
        /// <summary>
        /// List of formats that end at the point of inserting text. Used in updating end points of dynamic formatting.
        /// </summary>
        private IList m_endingFormats;
        /// <summary>
        /// Indicates whether inserting text should be allowed at the beginning of readonly region at the start of new line.
        /// </summary>
        private bool m_bAllowInsertBeforeReadonlyNewLine = true;
        /// <summary>
        /// Indicates whether insertion of text before readonly region at the beginning of new line is currently performed.
        /// </summary>
        private bool m_bInsertingBeforeReadonlyNewLine = false;
        /// <summary>
        /// Indicates whether scrollbars should be always visible.
        /// </summary>
        private bool m_bAlwaysShowScrollers = false;
        /// <summary>
        /// If more than 0, WM_CHAR is not sent to parent's WndProc.
        /// </summary>
        private int iCatchWmChar = 0;
        /// <summary>
        /// Chached last rendered line for OnCursorManagerBeforeCoordinatesChange. Used for performance improvement.
        /// </summary>
        private RenderedLine m_lastLineForBeforeCoordsChanged;
        /// <summary>
        /// Last area bottom position for OnCursorManagerBeforeCoordinatesChange. Used for performance improvement.
        /// </summary>
        private int m_lastBottomYForBeforeCoordsChanged;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bAllowMouseCursorChange = true;
        /// <summary>
        /// Placement of user margin.
        /// </summary>
        private MarginPlacement m_userMarginPlacement = MarginPlacement.Right;
        /// <summary>
        /// Width of the line numbers area.
        /// </summary>
        private int m_lineNumbersWidth = 10;
        /// <summary>
        /// Virtual line number start value.
        /// </summary>
        private int m_virtualLineNumberOffset = 0;
        /// <summary>
        /// Controls mouse position
        /// </summary>
        private Point m_position = Point.Empty;
        /// <summary>
        /// Selection of the line number area.
        /// </summary>
        private bool m_bSelectOnLineNumberClick = true;
        /// <summary>
        /// Selection of the dragged text.
        /// </summary>
        private bool m_bSelectTextAfterDragDrop = true;
        /// <summary>
        /// Selection of the dragged text.
        /// </summary>
        private bool m_bRespectTabStopsOnInsertingText = false;
        /// <summary>
        /// Selectection of the full line.
        /// </summary>
        private bool m_bExtendSelectionToFarRight = true;
        /// <summary>
        /// Indicates whether native GDI should be used for text output.
        /// </summary>
        private bool m_bNativeDrawing;
        /// <summary>
        /// Space between lines.
        /// </summary>
        private int m_spaceBetweenLines = SPACE_BETWEEN_LINES;
        /// <summary>
        /// Point where mouse left button was pressed.
        /// </summary>
        private Point m_mouseDownPoint;
        /// <summary>
        /// Indicates whether encoding was changed forcibly because of inserting some not supported symbols and user should be prompted on save.
        /// </summary>
        protected bool m_bEncodingForcedlyChanged;
        /// <summary>
        /// Encoding used before forced change.
        /// </summary>
        internal Encoding m_oldEncoding;
        /// <summary>
        /// Indicates whether context menu is being shown or not.
        /// </summary>
        private bool isShowing = false;
        /// <summary> 
        /// Determines whether KeyUp was handled. 
        /// </summary> 
        private bool m_bKeyUpHandled;
        /// <summary>
        /// Indicates whether click from indent menu
        /// </summary>
        private bool fromIndentClick = false;
        /// <summary>
        /// Used in Updating Tooltip Mouse Move
        /// </summary>
        private bool isToolTipOn = false;
        /// <summary>
        /// Used in Drag-Drop.
        /// </summary>
        private DragDropEffects DragResult = DragDropEffects.None;
        /// <summary>
        /// start point of the background selection
        /// </summary>
        private CoordinatePoint backcolor_startpoint;
        #endregion

        #region Static Properties
        /// <summary>
        /// Default Graphics object. Used for measuring.
        /// </summary>
        static protected internal Graphics DefaultGraphics
        {
            get
            {
                return _graphics;
            }
        }
        /// <summary>
        /// Gets cursor that shows four directions all together.
        /// </summary>
        public static Cursor SelectLineCursor
        {
            get
            {
                if (_cursorLineSelection == null)
                {
                    Assembly executing = Assembly.GetExecutingAssembly();
                    Stream stream = executing.GetManifestResourceStream(m_CursorSelectLineName);

                    _cursorLineSelection = new Cursor(stream);
                }

                return _cursorLineSelection;
            }
        }
        #endregion
       
        #region Properties

        /// <summary>
        /// Gets or sets the search the text like in visual studion editor 
        /// </summary>
        public bool FileEditLikeVisualStduioSearch
        {
            get
            {
                return this.m_likeVSStudioSearch;
            }
            set
            {
                if (this.m_likeVSStudioSearch != value)
                {
                    this.m_likeVSStudioSearch = value;
                }
            }
        }

        /// <summary>
        /// Gets bookmark management helper.
        /// </summary>
        public BookmarksHelper Bookmarks
        {
            get
            {
                return m_bookmarkhelper;
            }
        }
        /// <summary>
        /// Gets or sets value indicating whether page header and footer should be printed.
        /// </summary>
        public bool PrintHeaderAndFooter
        {
            get
            {
                return m_bPrintHeaderFooter;
            }
            set
            {
                m_bPrintHeaderFooter = value;
            }
        }
        /// <summary>
        /// Gets or sets value indication whether virtual space mode is enabled.
        /// </summary>
        public bool VirtualSpaceMode
        {
            get
            {
                if (m_CursorManager == null)
                    return m_bVirtualSpaceMode;

                return m_CursorManager.VirtualSpaceMode;
            }
            set
            {
                m_CursorManager.VirtualSpaceMode = value;
                m_bVirtualSpaceMode = value;
            }
        }
        /// <summary>
        /// The background color, gradient, and other styles can be set through this property.
        /// </summary>
        /// <remarks>
        /// The GradientPanel control provides this property to enable specialized custom gradient backgrounds.
        /// </remarks>
        public BrushInfo BackgroundColor
        {
            get
            {
                return m_brushBackground;
            }
            set
            {
                if (m_brushBackground != value)
                {
                    m_brushBackground = value;
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets color of the indent line.
        /// </summary>
        public Color IndentLineColor
        {
            get
            {
                return m_colorIndentLine;
            }
            set
            {
                if (m_colorIndentLine != value)
                {
                    m_colorIndentLine = value;
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets color of the text under selection.
        /// </summary>
        internal Color SelectionTextColor
        {
            get
            {
                return m_selectionTextColor;
            }
            set
            {
                if (m_selectionTextColor != value)
                {
                    m_selectionTextColor = value;
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets color of the indent block start and end.
        /// </summary>
        public Color IndentBlockHighlightingColor
        {
            get
            {
                return m_colorIndentBorders;
            }
            set
            {
                if (m_colorIndentBorders != value)
                {
                    m_colorIndentBorders = value;
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets value that specifies whether indent Guideline should be shown automatically after cursor repositioning.
        /// </summary>
        public bool AutoIndentGuideline
        {
            get
            {
                return m_bIndentAutoShow;
            }
            set
            {
                m_bIndentAutoShow = value;
            }
        }
        /// <summary>
        /// Gets or sets value that specifies whether guidelines can be highlighted or only guideline should be drawn.
        /// </summary>
        public bool OnlyHighlightMatchingBraces
        {
            get
            {
                return m_bOnlyHighlightMatchingBraces;
            }
            set
            {
                if (m_bOnlyHighlightMatchingBraces != value)
                {
                    m_bOnlyHighlightMatchingBraces = value;
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets selected text range.
        /// </summary>
        public IComplexTextRange Selection
        {
            get
            {
                return m_selection;
            }
        }
        /// <summary>
        /// Context menu manager.
        /// </summary>
        public ContextMenuManager ContextMenuManager
        {
            get
            {
                return m_menuManager;
            }
            set
            {
                m_menuManager = value;
            }
        }
        /// <summary>
        /// Gets or sets current column.
        /// </summary>
        public int CurrentColumn
        {
            get
            {
                return m_CursorManager.CursorVirtualCoordinates.Column;
            }
            set
            {
                if (m_CursorManager.CursorVirtualCoordinates.Column != value)
                {
                    m_CursorManager.CursorVirtualCoordinates.Column = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets current virtual column. Virtual column is visual position of character on the screen.
        /// </summary>
        public int VisualColumn
        {
            get
            {
                return GetCurrentVisualColumn();
            }
            set
            {
                SetCurrentVisualColumn(value);
            }
        }
        /// <summary>
        /// Gets or sets current line.
        /// </summary>
        public int CurrentLine
        {
            get
            {
                return m_CursorManager.CursorVirtualCoordinates.Line;
            }
            set
            {
                if (m_CursorManager.CursorVirtualCoordinates.Line != value)
                {
                    m_CursorManager.CursorVirtualCoordinates.Line = value;
                    UpdateScrollInfo();
                    m_CursorManager.Update();
                }
            }
        }
        /// <summary>
        /// Gets or sets current position of the cursor in virtual coordinates.
        /// </summary>
        public Point CurrentPosition
        {
            get
            {
                return m_CursorManager.CursorVirtualCoordinates.Position;
            }
            set
            {
                if (value != m_CursorManager.CursorVirtualCoordinates.Position)
                {
                    m_parser.collapseCollectionUpdate  = false;
                    m_CursorManager.CursorVirtualCoordinates.Position = value;
                    m_parser.collapseCollectionUpdate  = true;
                }
            }
        }
        /// <summary>
        /// Gets or sets insert mode state.
        /// </summary>
        public bool InsertMode
        {
            get
            {
                return m_bInsertMode;
            }
            set
            {
                if (m_bInsertMode != value)
                {
                    m_CursorManager.Visible = false;
                    m_bInsertMode = value;

                    m_CursorManager.CursorGraphicalCoordinates.Size = new Size((m_bInsertMode) ? -1 : 0, 0);

                    m_CursorManager.Visible = true;
                    OnInsertModeChanged();
                }
            }
        }
        /// <summary>
        /// Gets or sets value indicating whether line numbers should be shown.
        /// </summary>
        public bool ShowLineNumbers
        {
            get
            {
                return m_bShowLines && !SingleLineMode;
            }
            set
            {
                if (value != m_bShowLines)
                {
                    m_bShowLines = value;
                    UpdateScrollerVerticalSize();
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets value indicating whether line markers should be shown.
        /// </summary>
        public bool ShowMarkers
        {
            get
            {
                return m_bShowMarkers;
            }
            set
            {
                if (value != m_bShowMarkers)
                {
                    m_bShowMarkers = value;
                    UpdateScrollerVerticalSize();
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets value indicating whether collapsers should be shown.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Description("Obsolete : Use ShowOutliningCollapsers property instead.")]
        public bool ShowCollapse
        {
            get
            {
                return m_bShowCollapse;
            }
            set
            {
                if (value != m_bShowCollapse)
                {
                    m_bShowCollapse = value;
                    m_parser.CollapsingEnabled = value;
                    UpdateScrollerVerticalSize();
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets value indicating whether outlining collapsers should be shown.
        /// </summary>
        public bool ShowOutliningCollapsers
        {
            get
            {
                return ShowCollapse;
            }
            set
            {
                ShowCollapse = value;
            }
        }
        /// <summary>
        /// Gets or sets value indicating whether content dividers should be shown.
        /// </summary>
        public bool ShowContentDividers
        {
            get
            {
                return m_bShowContentDividers;
            }
            set
            {
                if (value != m_bShowContentDividers)
                {
                    m_bShowContentDividers = value;
                    UpdateScrollerVerticalSize();
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or Sets a value indicating whether to stop search at the page end.
        /// </summary>
        public bool WrapAroundSearch
        {
            get
            {
                return m_bWrapAroundSearch;
            }
            set
            {
                if (value != m_bWrapAroundSearch)
                {
                    m_bWrapAroundSearch = value;
                }
            }
        }
        bool bEnableSmartInBlockIndent = false;
        ///<summary>
        /// Gets or sets value indicating whether Auto indent smart mode work in block mode.
        /// </summary>

        public bool EnableSmartInBlockIndent
        {
            get
            {
                return bEnableSmartInBlockIndent;
            }
            set
            {
                if (bEnableSmartInBlockIndent != value)
                {
                    bEnableSmartInBlockIndent = value;
                }
            }
        }
        /// <summary>
        /// Gets or Sets the enableMD5
        /// </summary>
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
                    if (value)
                        _md5 = MD5.Create();
                }
            }
        }
        /// <summary>
        /// Gets or sets value indicating whether indentation guidelines should be shown.
        /// </summary>
        public bool ShowIndentationGuidelines
        {
            get
            {
                return m_bShowIndentationGuidelines;
            }
            set
            {
                if (value != m_bShowIndentationGuidelines)
                {
                    m_bShowIndentationGuidelines = value;
                    UpdateScrollerVerticalSize();
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets visibility of the selection margin.
        /// </summary>
        public bool ShowSelectionMargin
        {
            get
            {
                return m_bSelectionMarginVisible;
            }
            set
            {
                if (m_bSelectionMarginVisible != value)
                {
                    m_bSelectionMarginVisible = value;
                    UpdateScrollerVerticalSize();

                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets background color of the selection margin.
        /// </summary>
        public Color SelectionMarginBackgroundColor
        {
            get
            {
                return m_colorSelectionMarginBackground;
            }
            set
            {
                if (m_colorSelectionMarginBackground != value)
                {
                    m_colorSelectionMarginBackground = value;
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets foreground color of the selection margin.
        /// </summary>
        public Color SelectionMarginForegroundColor
        {
            get
            {
                return m_colorSelectionMarginForeground;
            }
            set
            {
                if (m_colorSelectionMarginForeground != value)
                {
                    m_colorSelectionMarginForeground = value;
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets width of the selection margin.
        /// </summary>
        public int SelectionMarginWidth
        {
            get
            {
                return m_iSelectionMarginWidth;
            }
            set
            {
                if (value != m_iSelectionMarginWidth)
                {
                    m_iSelectionMarginWidth = value;
                    UpdateScrollerVerticalSize();
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets value indicating whether whitespaces should be shown as special symbols.
        /// </summary>
        public bool ShowWhitespaces
        {
            get
            {
                if (m_parser != null)
                {
                    m_bShowWhitespaces = m_parser.Formats.ShowWhiteSpaces;
                }
                return m_bShowWhitespaces;
            }
            set
            {
                if (m_bShowWhitespaces != value)
                {
                    m_bShowWhitespaces = value;
                    if (m_parser != null)
                    {
                        m_parser.Formats.ShowWhiteSpaces = m_bShowWhitespaces;
                        InvalidateAll();
                    }
                }
            }
        }
        /// <summary>
        /// Gets number of lines.
        /// </summary>
        public int VisibleLineCount
        {
            get
            {
                if (m_parser == null) return 0;
                return m_parser.TotalLines;
            }
        }
        /// <summary>
        /// Get number of lines in file.
        /// </summary>
        public int PhysicalLineCount
        {
            get
            {
                if (m_parser == null) return 0;
                return m_parser.BaseStream.LinesCount;
            }
        }
        /// <summary>
        /// Gets or sets language configuration.
        /// </summary>
        public Config Configurator
        {
            get
            {
                return m_Configuration;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("Configurator");

                if (m_Configuration.DefaultLanguage   != value.DefaultLanguage  )
                {
                    string currentLanguage = (m_parser.Formats as IConfigLanguage).Language;
                    m_Configuration.ConfigurationChanged -= new EventHandler(OnConfiguratorChanged);
                    m_Configuration.FormatsChanged -= new EventHandler(OnFormatsConfigurationChanged);
                    m_Configuration = value;
                    m_Configuration.ConfigurationChanged += new EventHandler(OnConfiguratorChanged);
                    m_Configuration.FormatsChanged += new EventHandler(OnFormatsConfigurationChanged);
                    OnConfigurationChanged();

                    if (m_Configuration.KnownLanguageNames.Contains(currentLanguage))
                        ResetColoring(m_Configuration[currentLanguage]);
                    else
                        ResetColoring(m_Configuration.DefaultLanguage);
                }
            }
        }
        /// <summary>
        /// Gets parser for internal usage
        /// </summary>
        public RenderableLexemParser Parser
        {
            get
            {
                return m_parser;
            }
        }
        /// <summary>
        /// Gets or sets state of the word-wrapping mode.
        /// </summary>
        public bool WordWrap
        {
            get
            {
                return m_bWordWrapping && !m_bSingleLineEnabled;
            }
            set
            {
                if (value != m_bWordWrapping)
                {
                    CurrentColumn = 1;
                    VirtualSize = (!DisableScrollers) ? new Size(0, VirtualSize.Height) : Size.Empty;
                    m_bWordWrapping = value;

                    if (WordWrapChanged != null)
                    {
                        WordWrapChanged(this, EventArgs.Empty);
                    }

                    UpdateMeasuringInfo();
                }
            }
        }
        /// <summary>
        /// Gets or sets composite quality.
        /// </summary>
        public CompositingQuality GraphicsCompositingQuality
        {
            get
            {
                return m_CompositingQuality;
            }
            set
            {
                if (value != m_CompositingQuality)
                {
                    m_CompositingQuality = value;
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets interpolation mode.
        /// </summary>
        public InterpolationMode GraphicsInterpolationMode
        {
            get
            {
                return m_InterpolationMode;
            }
            set
            {
                if (value != m_InterpolationMode)
                {
                    m_InterpolationMode = value;
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets smoothing mode.
        /// </summary>
        public SmoothingMode GraphicsSmoothingMode
        {
            get
            {
                return m_SmoothingMode;
            }
            set
            {
                if (value != m_SmoothingMode)
                {
                    m_SmoothingMode = value;
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets text rendering hint.
        /// </summary>
        public TextRenderingHint GraphicsTextRenderingHint
        {
            get
            {
                return m_TextRenderingHint;
            }
            set
            {
                if (value != m_TextRenderingHint)
                {
                    m_TextRenderingHint = value;
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets flag that determines whether undo operation can be done.
        /// </summary>
        public bool CanUndo
        {
            get
            {
                return ((m_parser != null) ? m_parser.CanUndo : false);
            }
        }
        /// <summary>
        /// Gets flag that determines whether redo operation can be done.
        /// </summary>
        public bool CanRedo
        {
            get
            {
                return ((m_parser != null) ? m_parser.CanRedo : false);
            }
        }
        /// <summary>
        /// Gets flag that determines whether copy operation can be done.
        /// </summary>
        public bool CanCopy
        {
            get
            {
                return (m_parser != null && m_wrapper != null && !m_selection.IsEmpty());
            }
        }
        /// <summary>
        /// Gets flag that determines whether paste operation can be done.
        /// </summary>
        public bool CanPaste
        {
            get
            {
                return CheckForSupportedData(Clipboard.GetDataObject());
            }
        }
        /// <summary>
        /// Gets flag that determines whether cut operation can be done.
        /// </summary>
        public bool CanCut
        {
            get
            {
                return CanCopy;
            }
        }
        /// <summary>
        /// Gets or sets selected text.
        /// </summary>
        /// <remarks>
        /// If there is no text selected and you are setting new selected text, it will be inserted in the position of the cursor.
        /// Otherwise, when there is some text selected, it will be deleted and new text will be inserted.
        /// </remarks>
        public string SelectedText
        {
            get
            {
                StringBuilder result = new StringBuilder();
                if (m_parser != null && !m_selection.IsEmpty())
                {
                    for (int i = 0, count = m_selection.Ranges.Count; i < count; i++)
                    {
                        TextRange range = (TextRange)m_selection.Ranges[i];
                        result.Append(m_parser.BaseStream.GetTextInRange(range.Top.PhysicalPoint, range.Bottom.PhysicalPoint, true));
                        if (i < count - 1)
                        {
                            result.Append(STR_NEW_LINE);
                        }
                    }
                }
                return result.ToString();
            }
            set
            {
                if (m_parser != null)
                {
                    LockUpdate();
                    BeginSelectionUpdate();
                    UndoGroupOpen();
                    DeleteSelected();
                    StopSelection();
                    SelectionCancel();
                    StartSelection();
                    TextInsertInternalRespectingTabStops(CurrentLine, CurrentColumn, value);
                    StopSelection();
                    UndoGroupClose();
                    EndSelectionUpdate();
                    UnlockUpdate();
                }
            }
        }
        /// <summary>
        /// Gets or sets count of spaces to be placed instead tabulation symbol.
        /// </summary>
        public int TabSize
        {
            get
            {
                if (m_parser != null)
                {
                    m_iTabSize = m_parser.TabLength;
                }
                return m_iTabSize;
            }
            set
            {
                if (value < MIN_TAB_SIZE || value > MAX_TAB_SIZE)
                {
                    throw new ArgumentOutOfRangeException("TabSize");
                }

                m_iTabSize = value;

                if (m_parser != null)
                {
                    m_parser.TabLength = m_iTabSize;
                    m_parser.DropMeasuringInfo();
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets tabulation replace sign.
        /// </summary>
        public bool UseTabs
        {
            get
            {
                return m_bUseTabs;
            }
            set
            {
                m_bUseTabs = value;
            }
        }
        /// <summary>
        /// Gets or sets plain text representation of the text data, the control is working with.
        /// </summary>
        public new string Text
        {
            get
            {
                CheckControlState();

                return m_wrapper.GetTextInRange(
                    m_parser.BaseStream.GetParsePoint(0), m_parser.BaseStream.GetParsePoint(m_parser.BaseStream.Length), true);
            }
            set
            {
                if (m_parser != null)
                {
                    bool bReadOnly = m_bReadOnly;
                    m_bReadOnly = false;

                    // Editor can be in readonly state just because stream can not be written.
                    if (!ReadOnly && this.Text != value)
                    {
                        LockUpdate();
                        UndoGroupOpen();
                        SelectAll();
                        DeleteSelected();
                        StopSelection();
                        SelectionCancel();
                        TextInsertInternalRespectingTabStops(CurrentLine, CurrentColumn, value);
                        UndoGroupClose();
                        UnlockUpdate();

                        if (this.DesignMode)
                            ResetUndoInfo();
                    }

                    m_bReadOnly = bReadOnly;
                    VirtualSize = Size.Empty;
                    UpdateScrollInfo();
                }
                else throw new Exception(Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_67);
            }
        }
        /// <summary>
        /// Gets text of the current line.
        /// </summary>
        public string CurrentLineText
        {
            get
            {
                CheckControlState();
                return GetLineText(CurrentLine);
            }
        }
        /// <summary>
        /// Gets instance of the current line.
        /// </summary>
        public ILexemLine CurrentLineInstance
        {
            get
            {
                CheckControlState();
                return m_parser.GetLine(CurrentLine) as RenderedLine;
            }
        }
        /// <summary>
        /// Gets or sets visibility of the user margin.
        /// </summary>
        public bool ShowUserMargin
        {
            get
            {
                return m_bShowUserMargin;
            }
            set
            {
                if (value != m_bShowUserMargin)
                {
                    m_bShowUserMargin = value;
                    UpdateScrollerOffsets(this);
                    UpdateScrollBarsSize();
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets width of the user margin.
        /// </summary>
        public int UserMarginWidth
        {
            get
            {
                return m_iUserMarginWidth;
            }
            set
            {
                if (value != m_iUserMarginWidth)
                {
                    m_iUserMarginWidth = value;
                    UpdateScrollerOffsets(this);
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets list of available languages.
        /// </summary>
        public IList Languages
        {
            get
            {
                return new ArrayList(m_Configuration.KnownLanguages);
            }
        }
        /// <summary>
        /// Gets or sets config language currently used.
        /// </summary>
        public IConfigLanguage Language
        {
            get
            {
                return ((m_parser != null) ? (m_parser.Formats as IConfigLanguage) : null);
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                if (Language != value)
                {
                    ResetColoring(value);
                }
            }
        }
        /// <summary>
        /// Gets or sets the value indicating whether changes can be done to the input stream.
        /// </summary>
        public bool ReadOnly
        {
            get
            {
                return m_bReadOnly || (m_parser == null || !m_parser.BaseStream.CanWrite);
            }
            set
            {
                if (m_bReadOnly != value)
                {
                    m_bReadOnly = value;
                    RaiseReadOnlyChangedEvent();
                }
            }
        }
        /// <summary>
        /// Gets list of commands.
        /// </summary>
        public IKeyCommandList Commands
        {
            get
            {
                return m_keyBinder.Commands;
            }
        }
        /// <summary>
        /// Gets root key binder.
        /// </summary>
        public IKeyCommandListBinder KeyBinder
        {
            get
            {
                return m_keyBinder.Binder;
            }
        }
        /// <summary>
        /// Gets or sets key binding processor.
        /// </summary>
        public KeyProcessor KeyBindingProcessor
        {
            get
            {
                return m_keyBinder;
            }
            set
            {
                if (value == null) throw new ArgumentNullException();

                m_keyBinder = value;
            }
        }
        /// <summary>
        /// Get or sets grouping actions for undo/redo.
        /// </summary>
        public bool GroupUndo
        {
            get
            {
                return m_bGroupUndo;
            }
            set
            {
                if (m_bGroupUndo != value)
                {
                    m_UndoMarkers.Clear();
                    m_RedoMarkers.Clear();

                    m_bGroupUndo = value;

                    // We need to set markers for every operation in undo/redo queues.
                    if (value)
                    {
                        for (int i = 0; i < m_parser.UndoQueueLength; i++)
                            m_UndoMarkers.Push(i);

                        for (int i = 0; i < m_parser.RedoQueueLength; i++)
                            m_RedoMarkers.Push(i);
                    }
                }
            }
        }
        /// <summary>
        /// Gets location of the cursors right-bottom position in control coordinates.
        /// </summary>
        public Point CursorGraphicalLocation
        {
            get
            {
                if (m_CursorManager == null) return Point.Empty;

                Point point = m_CursorManager.CursorGraphicalCoordinates.LeftTop;

                point.X -= HScrollBar.Value;
                point.Y -= VScrollBar.Value;
                point.X += ScrollOffsetLeft;
                point.Y += ScrollOffsetTop;

                Rectangle rc = m_CursorManager.CursorGraphicalCoordinates.Rectangle;
                point.Y += rc.Height;
                point.X += rc.Width;

                return point;
            }
        }
        /// <summary>
        /// Gets or sets flag that specifies, whether context menu is enabled.
        /// </summary>
        public bool ContextMenuEnabled
        {
            get
            {
                return m_menuManager.Enabled;
            }
            set
            {
                m_menuManager.Enabled = value;
            }
        }
        /// <summary>
        /// Gets or sets status of the single line mode.
        /// </summary>
        public bool SingleLineMode
        {
            get
            {
                return m_bSingleLineEnabled;
            }
            set
            {
                if ((m_bSingleLineEnabled != value))
                {
                    try
                    {
                        Form form = this.FindForm();

                        if (null != form)
                            WinAPI.LockWindowUpdate(form.Handle);

                        m_bSingleLineEnabled = value;

                        if (m_bWordWrapping & m_bSingleLineEnabled)
                        {
                            UpdateMeasuringInfo();
                        }

                        OnSingeLineChanged();
                    }
                    finally
                    {
                        WinAPI.LockWindowUpdate(IntPtr.Zero);
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether removing a read-only region is allowed.
        /// </summary>
        public bool AllowDeleteReadOnlyRegion
        {
            get 
            { 
                return allowDeleteReadOnlyRegion; 
            }
            set 
            { 
                allowDeleteReadOnlyRegion = value; 
            }
        }

        /// <summary>
        /// Gets value indicating whether content of the file was modified after the last save.
        /// </summary>
        public bool IsModified
        {
            get
            {
                return (CanUndo && m_bModified);
            }
        }
        /// <summary>
        /// Gets print document that can be used to print the contents of the editor.
        /// </summary>
        public PrintDocument PrintDocument
        {
            get
            {
                if (m_printDoc == null)
                {
                    m_printDoc = new PrintDocument();
                    m_printDoc.PrintPage += new PrintPageEventHandler(OnPrintDocumentPage);
                    m_printDoc.BeginPrint += new PrintEventHandler(OnDocBeginPrint);
                    m_printDoc.EndPrint += new PrintEventHandler(OnDocEndPrint);
                    m_printDoc.PrinterSettings.MinimumPage = 1;
                    m_printDoc.PrinterSettings.FromPage = 1;
                    m_printDoc.PrinterSettings.ToPage = 1;
                }
                return m_printDoc;
            }
        }
        /// <summary>
        /// Gets or sets value that specifies whether context choice should be updated when it is active and user types something.
        /// </summary>
        public bool UpdateContextChoiceList
        {
            get
            {
                return m_bUpdateContextChoice;
            }
            set
            {
                m_bUpdateContextChoice = value;
            }
        }
        /// <summary>
        /// Gets or sets transparency of the selection.
        /// </summary>
        public bool TransparentSelection
        {
            get
            {
                return m_bTransparentSelection;
            }
            set
            {
                if (m_bTransparentSelection != value)
                {
                    m_bTransparentSelection = value;

                    SelectionCancel();
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets value indicating whether control accepts tabs.
        /// </summary>
        public bool TransferFocusOnTab
        {
            get
            {
                return m_bTransferFocusOnTab;
            }
            set
            {
                m_bTransferFocusOnTab = value;
            }
        }
        /// <summary>
        /// Gets properties of Show white spaces mode.
        /// </summary>
        public ShowWhiteSpaceProperties ShowWhiteSpaceProperties
        {
            get
            {
                return m_whitespaceProps;
            }
        }
        /// <summary>
        /// Gets or sets color of user margin border.
        /// </summary>
        public Color UserMarginBorderColor
        {
            get
            {
                return m_clrUserMarginBorder;
            }
            set
            {
                if (value != m_clrUserMarginBorder)
                {
                    m_clrUserMarginBorder = value;
                    if (m_bShowUserMargin)
                    {
                        InvalidateAll();
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets BrushInfo object that is used when user margin is being drawn.
        /// </summary>
        public BrushInfo UserMarginBackgroundColor
        {
            get
            {
                return m_brushUserMargin;
            }
            set
            {
                if (value != m_brushUserMargin && value != null)
                {
                    m_brushUserMargin = value;
                    if (m_bShowUserMargin)
                    {
                        InvalidateAll();
                    }
                }
            }
        }
        /// <summary>
        /// Get or set default font of user margin text.
        /// </summary>
        public Font UserMarginTextFont
        {
            get
            {
                return m_fontUserMarginText;
            }
            set
            {
                if (null != value)
                {
                    if (null != m_fontUserMarginText) m_fontUserMarginText.Dispose();
                    m_fontUserMarginText = value;
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Get or set default color of user margin text.
        /// </summary>
        public Color UserMarginTextColor
        {
            get
            {
                return m_clrUserMarginText;
            }
            set
            {
                if (value != Color.Empty && value != m_clrUserMarginText)
                {
                    m_clrUserMarginText = value;
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets type of word wrapping.
        /// </summary>
        public WordWrapType WrapType
        {
            get
            {
                return m_wrapType;
            }
            set
            {
                if (value != m_wrapType)
                {
                    m_wrapType = value;
                    UpdateMeasuringInfo();
                }
            }
        }
        /// <summary>
        /// Gets or sets width of text area.
        /// </summary>
        public int TextAreaWidth
        {
            get
            {
                return m_textAreaWidth;
            }
            set
            {
                if (m_textAreaWidth != value)
                {
                    m_textAreaWidth = value;

                    if (WordWrapMode.WordWrapMargin == m_wrapMode)
                    {
                        RemeasureLinesWrapping();
                    }

                    if (ShowTextArea)
                        InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets the location of marker area on the horizontal axis pays attention to RTL.
        /// </summary>
        private int MarkerAreaOffset
        {
            get
            {
                int offset = (this.ShowMarkers) ? this.MarkerAreaWidth : 0;

                if (this.RightToLeft == RightToLeft.Yes)
                {
                    if (ForPrinting)
                        return (int)this.PrintableArea.Right - offset;
                    else if (ForFakeEdit)
                        return this.FakeEditArea.Right - offset;
                    else
                        return this.ClientRectangle.Right - offset;
                }
                else
                    return 0;
            }
        }

        /// <summary>
        /// Gets or sets width of marker area.
        /// </summary>
        public int MarkerAreaWidth
        {
            get
            {
                return m_markerAreaWidth;
            }
            set
            {
                if (value != m_markerAreaWidth)
                {
                    m_markerAreaWidth = value;
                    if (ShowMarkers)
                    {
                        UpdateScrollerVerticalSize();
                        InvalidateAll();
                        RemeasureLinesWrapping();
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets bool that indicates whether text area should be shown.
        /// </summary>
        public bool ShowTextArea
        {
            get
            {
                return m_bShowTextArea;
            }
            set
            {
                if (m_bShowTextArea != value)
                {
                    m_bShowTextArea = value;
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets style of line that delimits text area.
        /// </summary>
        public DashStyle TextAreaLineStyle
        {
            get
            {
                return m_textAreaLineStyle;
            }
            set
            {
                if (m_textAreaLineStyle != value)
                {
                    m_textAreaLineStyle = value;
                    if (m_bShowTextArea)
                    {
                        InvalidateAll();
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets color of line that delimits text area.
        /// </summary>
        public Color TextAreaLineColor
        {
            get
            {
                return m_clrTextAreaLine;
            }
            set
            {
                if (m_clrTextAreaLine != value && Color.Empty != value)
                {
                    m_clrTextAreaLine = value;
                    if (m_bShowTextArea)
                    {
                        InvalidateAll();
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets BrushInfo object that is used when area situated after text area is drawn.
        /// </summary>
        public BrushInfo AfterTextAreaBrush
        {
            get
            {
                return m_brushAfterTextArea;
            }
            set
            {
                if (m_brushAfterTextArea != value && null != value)
                {
                    m_brushAfterTextArea = value;
                    if (m_bShowTextArea)
                    {
                        InvalidateAll();
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets mode of word wrapping.
        /// </summary>
        public WordWrapMode WrapMode
        {
            get
            {
                return m_wrapMode;
            }
            set
            {
                if (value != m_wrapMode)
                {
                    m_wrapMode = value;
                    UpdateMeasuringInfo();
                }
            }
        }
        /// <summary>
        /// Gets or sets array of tab stops.
        /// </summary>
        public int[] TabStopsArray
        {
            get
            {
                return m_arrTabStops;
            }
            set
            {
                if (null == value) throw new ArgumentNullException("TabStopsArray");

                Array.Sort(value);

                if (value[0] <= 0) throw new ArgumentOutOfRangeException("TabStopsArray");
                for (int i = 0; i < value.Length - 1; i++)
                {
                    if (value[i] == value[i + 1]) throw new ArgumentOutOfRangeException("TabStopsArray");
                }

                m_arrTabStops = value;
            }
        }
        /// <summary>
        /// Gets or sets bool that indicates whether tab stops should be used.
        /// </summary>
        public bool UseTabStops
        {
            get
            {
                return m_bUseTabStops;
            }
            set
            {
                if (value != m_bUseTabStops)
                {
                    m_bUseTabStops = value;
                    InvalidateAll();
                }
            }
        }
        private Color indicatorMarginColor = Color.Empty;
        /// <summary>
        /// Gets or sets value indicating whether XP style should be used.
        /// </summary>
        public Color IndicatorMarginColor
        {
            get
            {
                return indicatorMarginColor;
            }
            set
            {
                if (value != indicatorMarginColor)
                {
                    indicatorMarginColor = value;
                    InvalidateAll();
                }
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether XP style should be used.
        /// </summary>
        public bool UseXPStyle
        {
            get
            {
                return m_bUseXPStyle;
            }
            set
            {
                if (value != m_bUseXPStyle)
                {
                    m_bUseXPStyle = value;
                    InvalidateAll();
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
                return m_bUseXPStyleBorder;
            }
            set
            {
                if (value != m_bUseXPStyleBorder)
                {
                    m_bUseXPStyleBorder = value;
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets mode of auto indentation.
        /// </summary>
        public AutoIndentMode AutoIndentMode
        {
            get
            {
                return m_autoIndentMode;
            }
            set
            {
                m_autoIndentMode = value;
            }
        }
        /// <summary>
        /// Gets or sets bool indicating whether context tooltips are shown.
        /// </summary>
        public bool ShowContextTooltip
        {
            get
            {
                return m_bShowContextTooltip;
            }
            set
            {
                m_bShowContextTooltip = value;
            }
        }
        /// <summary>
        /// Gets or sets bool indicating whether bookmark tooltips are shown.
        /// </summary>
        public bool ShowBookmarkTooltips
        {
            get
            {
                return m_bShowBookmarkTooltips;
            }
            set
            {
                m_bShowBookmarkTooltips = value;
            }
        }
        /// <summary>
        /// Gets or sets bool indicating whether outlining tooltips are shown.
        /// </summary>
        public bool ShowOutliningTooltip
        {
            get
            {
                return m_bShowOutliningTooltip;
            }
            set
            {
                m_bShowOutliningTooltip = value;
            }
        }
        /// <summary>
        /// Gets or sets bool that indicates whether lines wrapping should be marked.
        /// </summary>
        public bool MarkLineWrapping
        {
            get
            {
                return m_bMarkLineWrapping;
            }
            set
            {
                if (value != m_bMarkLineWrapping)
                {
                    m_bMarkLineWrapping = value;
                    RemeasureLinesWrapping();
                }
            }
        }
        /// <summary>
        /// Gets or sets custom image that marks lines wrapping.
        /// </summary>
        public Image CustomLineWrappingMarkingImage
        {
            get
            {
                return m_lineWrappingMarkingImage;
            }
            set
            {
                m_lineWrappingMarkingImage = value;
                if (this.MarkLineWrapping)
                {
                    RemeasureLinesWrapping();
                }
            }
        }
        /// <summary>
        /// Gets lines wrapping marking image.
        /// </summary>
        public Image WrapMarkingImage
        {
            get
            {
                Image result = null;

                if (m_lineWrappingMarkingImage != null)
                {
                    result = m_lineWrappingMarkingImage;
                }
                else if (m_lineWrappingDefautImage != null && m_lineWrappingDefautImageRTL!=null)
                {
                    result =(this.RightToLeft == RightToLeft.Yes)? m_lineWrappingDefautImageRTL : m_lineWrappingDefautImage;
                }
                else
                {
                    Stream imgStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(DEF_DEFAULT_WRAP_MARK);
                    m_lineWrappingDefautImage = Image.FromStream(imgStream);
                    ((Bitmap)m_lineWrappingDefautImage).MakeTransparent(Color.White);

                    imgStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(DEF_DEFAULT_WRAP_MARK_RTL);
                    m_lineWrappingDefautImageRTL = Image.FromStream(imgStream);
                    ((Bitmap)m_lineWrappingDefautImageRTL).MakeTransparent(Color.White);

                    result = (this.RightToLeft == RightToLeft.Yes) ? m_lineWrappingDefautImageRTL : m_lineWrappingDefautImage;
                }

                return result;
            }
        }
        /// <summary>
        /// Gets wrapped lines marking image.
        /// </summary>
        public Image WrappedLinesMarkingImage
        {
            get
            {
                Image result = null;

                if (m_wrappedLinesMarkingImage != null)
                {
                    result = m_wrappedLinesMarkingImage;
                }
                else if (m_wrappedLinesDefautImage != null && m_wrappedLinesDefautImageRTL != null)
                {
                    result = (this.RightToLeft == RightToLeft.Yes) ? m_wrappedLinesDefautImageRTL : m_wrappedLinesDefautImage;
                }
                else
                {
                    Stream imgStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(DEF_DEFAULT_WRAPPED_LINE_MARK);
                    m_wrappedLinesDefautImage = Image.FromStream(imgStream);
                    ((Bitmap)m_wrappedLinesDefautImage).MakeTransparent(Color.White);
                    
                    imgStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(DEF_DEFAULT_WRAPPED_LINE_MARK_RTL);
                    m_wrappedLinesDefautImageRTL = Image.FromStream(imgStream);
                    ((Bitmap)m_wrappedLinesDefautImageRTL).MakeTransparent(Color.White);

                    result = (this.RightToLeft == RightToLeft.Yes) ? m_wrappedLinesDefautImageRTL : m_wrappedLinesDefautImage;
                }

                return result;
            }
        }
        /// <summary>
        /// Gets or sets size of context prompt.
        /// </summary>
        public Size ContextPromptSize
        {
            get
            {
                return m_contextPromptSize;
            }
            set
            {
                if (!value.IsEmpty)
                {
                    m_contextPromptSize = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets value indicating whether custom context prompt size should be used.
        /// </summary>
        public bool UseCustomSizeContextPrompt
        {
            get
            {
                return m_bUseCustomSizeContextPrompt;
            }
            set
            {
                m_bUseCustomSizeContextPrompt = value;
            }
        }
        /// <summary>
        /// Gets or sets font of line numbers.
        /// </summary>
        public Font LineNumbersFont
        {
            get
            {
                return m_lineNumbersFont;
            }
            set
            {
                if (!m_lineNumbersFont.Equals(value))
                {
                    m_lineNumbersFont = value;
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets color of line numbers.
        /// </summary>
        public Color LineNumbersColor
        {
            get
            {
                return m_lineNumbersBrush.Color;
            }
            set
            {
                if (m_lineNumbersBrush.Color != value)
                {
                    m_lineNumbersBrush.Color = value;
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets color of line numbers.
        /// </summary>
        public LineNumberAlignment LineNumbersAlignment
        {
            get
            {
                if (m_centFormat.Alignment == StringAlignment.Near)
                {
                    return LineNumberAlignment.Left;
                }
                else
                {
                    return LineNumberAlignment.Right;
                }
            }
            set
            {
                if (value == LineNumberAlignment.Left)
                {
                    m_centFormat.Alignment = StringAlignment.Near;
                }
                else
                {
                    m_centFormat.Alignment = StringAlignment.Far;
                }
                InvalidateAll();
            }
        }
        /// <summary>
        /// Gets or sets color of context prompt form border.
        /// </summary>
        public Color ContextPromptBorderColor
        {
            get
            {
                return m_clrContextPromptBorder;
            }
            set
            {
                m_clrContextPromptBorder = value;
            }
        }
        /// <summary>
        /// Gets or sets color of context tooltip form border.
        /// </summary>
        public Color ContextTooltipBorderColor
        {
            get
            {
                return m_clrContextTooltipBorder;
            }
            set
            {
                m_clrContextTooltipBorder = value;
            }
        }
        /// <summary>
        /// Gets or sets color of bookmark tooltip form border.
        /// </summary>
        public Color BookmarkTooltipBorderColor
        {
            get
            {
                return m_clrBookmarkTooltipBorder;
            }
            set
            {
                m_clrBookmarkTooltipBorder = value;
            }
        }
        /// <summary>
        /// Gets or sets brush for context tooltip background.
        /// </summary>
        public BrushInfo ContextTooltipBackgroundBrush
        {
            get
            {
                return m_contextTooltipBackgroundBrush;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("ContextTooltipBackgroundBrush");

                m_contextTooltipBackgroundBrush = value;
            }
        }
        /// <summary>
        /// Gets or sets brush for bookmark tooltip background.
        /// </summary>
        public BrushInfo BookmarkTooltipBackgroundBrush
        {
            get
            {
                return m_bookmarkTooltipBackgroundBrush;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("ContextTooltipBackgroundBrush");

                m_bookmarkTooltipBackgroundBrush = value;
            }
        }
        /// <summary>
        /// Gets or sets brush for context prompt background.
        /// </summary>
        public BrushInfo ContextPromptBackgroundBrush
        {
            get
            {
                return m_contextPromptBackgroundBrush;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("ContextPromptBackgroundBrush");

                m_contextPromptBackgroundBrush = value;
                if (m_bShowIndentationBlockBorders)
                {
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets brush for indentation block background..
        /// </summary>
        public BrushInfo IndentationBlockBackgroundBrush
        {
            get
            {
                return m_indentationBlockBackgroundBrush;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("ContextPromptBackgroundBrush");

                m_indentationBlockBackgroundBrush = value;
            }
        }
        /// <summary>
        /// Gets or sets array of ColumnGuideItem objects.
        /// </summary>
        public ColumnGuideItem[] ColumnGuideItems
        {
            get
            {
                return m_arrColumnGuideItems;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("ColumnGuideItems");
                foreach (ColumnGuideItem colGuide in value)
                {
                    if (colGuide.Column <= 0) throw new ArgumentOutOfRangeException("ColumnGuideItems[].Column", colGuide.Column, "ColumnGuideItems");
                }

                m_arrColumnGuideItems = value;
                InvalidateAll();
            }
        }
        /// <summary>
        /// Gets or sets bool that indicates whether whether column guides should be drawn.
        /// </summary>
        public bool ShowColumnGuides
        {
            get
            {
                return m_bShowColumnGuides;
            }
            set
            {
                if (value != m_bShowColumnGuides)
                {
                    m_bShowColumnGuides = value;
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets font that is used while measuring position of column guides.
        /// </summary>
        public Font ColumnGuidesMeasuringFont
        {
            get
            {
                return m_columnGuidesMeasuringFont;
            }
            set
            {
                if (null != value)
                {
                    m_columnGuidesMeasuringFont = value;
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets style of new line of the newly created stream.
        /// </summary>
        public NewLineStyle DefaultNewLineStyle
        {
            get
            {
                return m_defaultNewLineStyle;
            }
            set
            {
                if (!Parser.BaseStream.NewLineStyleDetected)
                {
                    SetNewLineStyle(value);
                }
                m_defaultNewLineStyle = value;
            }
        }
        /// <summary>
        /// Gets or sets style of new line.
        /// </summary>
        public NewLineStyle NewLineStyle
        {
            get
            {
                return m_wrapper.NewLineStyle;
            }
            set
            {
                if (value != m_wrapper.NewLineStyle)
                {
                    SetNewLineStyle(value);
                }
            }
        }
        /// <summary>
        /// Gets or sets bool that indicates whether indentation block borders should be drawn.
        /// </summary>
        public bool ShowIndentationBlockBorders
        {
            get
            {
                return m_bShowIndentationBlockBorders;
            }
            set
            {
                m_bShowIndentationBlockBorders = value;
            }
        }
        /// <summary>
        /// Gets or sets style of indentation block border line.
        /// </summary>
        public FrameBorderStyle IndentationBlockBorderStyle
        {
            get
            {
                return m_indentationBlockBorderStyle;
            }
            set
            {
                m_indentationBlockBorderStyle = value;
            }
        }
        /// <summary>
        /// Gets or sets color of indentation block border line.
        /// </summary>
        public Color IndentationBlockBorderColor
        {
            get
            {
                return m_clrIndentationBlockBorder;
            }
            set
            {
                m_clrIndentationBlockBorder = value;
            }
        }
        /// <summary>
        /// Gets or sets bool that indicates whether outer file dragged and dropped into Edit Control should be inserted into current content.
        /// When set to false, current file is closed, and dropped outer file is opened.
        /// </summary>
        public bool InsertDroppedFileIntoText
        {
            get
            {
                return m_bInsertDroppedFileIntoText;
            }
            set
            {
                m_bInsertDroppedFileIntoText = value;
            }
        }
        /// <summary>
        /// Gets or sets bool that indicates whether context choice list gets autocompleted
        /// when single lexem remains in the list.
        /// </summary>
        public bool AutoCompleteSingleLexem
        {
            get
            {
                return m_bAutoCompleteSingleLexem;
            }
            set
            {
                m_bAutoCompleteSingleLexem = value;
            }
        }
        /// <summary>
        /// Gets or sets bool that indicates whether wrapped lines should be marked.
        /// </summary>
        public bool MarkWrappedLines
        {
            get
            {
                return m_bMarkWrappedLines;
            }
            set
            {
                if (value != m_bMarkWrappedLines)
                {
                    m_bMarkWrappedLines = value;
                    UpdateParserOffsetsInfo();
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets custom image that marks wrapped lines.
        /// </summary>
        public Image CustomWrappedLinesMarkingImage
        {
            get
            {
                return m_wrappedLinesMarkingImage;
            }
            set
            {
                m_wrappedLinesMarkingImage = value;
                UpdateParserOffsetsInfo();
                InvalidateAll();
            }
        }
        /// <summary>
        /// Gets or sets offset of paragraphs.
        /// </summary>
        public int ParagraphOffset
        {
            get
            {
                return m_iParagraphOffset;
            }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("ParagraphOffset");

                if (value != m_iParagraphOffset)
                {
                    m_iParagraphOffset = value;
                    UpdateParserOffsetsInfo();
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets offset of wrapped lines.
        /// </summary>
        public int WrappedLinesOffset
        {
            get
            {
                return m_iWrappedLinesOffset;
            }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("ParagraphOffset");

                if (value != m_iWrappedLinesOffset)
                {
                    m_iWrappedLinesOffset = value;
                    UpdateParserOffsetsInfo();
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets autoformatting manager.
        /// </summary>
        public AutoFormattingManager AutoFormattingManager
        {
            get
            {
                return m_autoFormattingManager;
            }
        }
        /// <summary>
        /// Gets or sets bool that indicates whether changed lines should be marked.
        /// </summary>
        public bool MarkChangedLines
        {
            get
            {
                return m_bMarkChangedLines;
            }
            set
            {
                if (value != m_bMarkChangedLines)
                {
                    m_bMarkChangedLines = value;
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets color of changed lines marking line.
        /// </summary>
        public Color ChangedLinesMarkingLineColor
        {
            get
            {
                return ((SolidBrush)m_changedLinesMarkingLineBrush).Color;
            }
            set
            {
                m_changedLinesMarkingLineBrush.Dispose();
                m_changedLinesMarkingLineBrush = new SolidBrush(value);
            }
        }
        /// <summary>
        /// Gets or sets color of saved lines marking line.
        /// </summary>
        public Color SavedLinesMarkingLineColor
        {
            get
            {
                return ((SolidBrush)m_savedLinesMarkingLineBrush).Color;
            }
            set
            {
                m_savedLinesMarkingLineBrush.Dispose();
                m_savedLinesMarkingLineBrush = new SolidBrush(value);
            }
        }
        /// <summary>
        /// Gets or sets form that represents options and can be shown through context menu.
        /// </summary>
        internal  EditControl contextMenuOptionsFormEditControl = null;
        /// <summary>
        /// Gets or Sets the context menu options form
        /// </summary>
        public Form ContextMenuOptionsForm
        {
            get
            {
                if( m_contextMenuOptionsForm == null)
                    ContextMenuOptionsForm = new ControlOptions(contextMenuOptionsFormEditControl);
                return m_contextMenuOptionsForm;
            }
            set
            {
                m_contextMenuOptionsForm = value;
            }
        }
        /// <summary>
        /// Gets or sets value indicating whether config file should be loaded. If set to false, default language is created from code.
        /// </summary>
        public bool LoadConfigFile
        {
            get
            {
                return m_bLoadConfigFile;
            }
            set
            {
                m_bLoadConfigFile = value;
            }
        }
        /// <summary>
        /// Gets or sets value indicating whether all files can be dropped to EditControl.
        /// If set to false, only files with extension contained in m_arrFileExtensions can be dropped.
        /// </summary>
        public bool DropAllFiles
        {
            get
            {
                return m_DropAllFiles;
            }
            set
            {
                m_DropAllFiles = value;
            }
        }
        /// <summary>
        /// Gets or sets value indicating whether saved lines can be flushed.
        /// If set to false, saved lines will not be flushed.
        /// </summary>
        public bool FlushSavedLines
        {
            get
            {
                return m_FlushSavedLines;
            }
            set
            {
                m_FlushSavedLines = value;
            }
        }
        /// <summary>
        /// Gets or sets whether autosave can be done in EditControl.
        /// </summary>
        internal bool AutoSave
        {
            get
            {
                return m_AutoSave;
            }
            set
            {
                m_AutoSave = value;
            }
        }
        /// <summary>
        /// Gets or sets extensions of files that can be dropped to EditControl.
        /// </summary>
        public string[] FileExtensions
        {
            get
            {
                return m_arrFileExtensions;
            }
            set
            {
                if (null == value) throw new ArgumentNullException("FileExtensionToDrop");

                Array.Sort(value);

                for (int i = 0; i < value.Length - 1; i++)
                {
                    if (value[i] == value[i + 1]) throw new ArgumentOutOfRangeException("FileExtensionToDrop");
                }

                m_arrFileExtensions = value;
            }
        }
        /// <summary>
        /// Gets or sets value indicating whether autoreplace triggers should be used.
        /// </summary>
        public bool UseAutoreplaceTriggers
        {
            get
            {
                return m_bUseAutoreplaceTriggers;
            }
            set
            {
                m_bUseAutoreplaceTriggers = value;
            }
        }
        /// <summary>
        /// Gets or sets value indicating whether border should be drawn around active code snippets.
        /// </summary>
        public bool DrawCodeSnippetBorder
        {
            get
            {
                return m_bDrawCodeSnippetBorder;
            }
            set
            {
                m_bDrawCodeSnippetBorder = value;
            }
        }
        /// <summary>
        /// Gets or sets value indicating whether current line should be highlighted.
        /// </summary>
        public bool HighlightCurrentLine
        {
            get
            {
                return m_bHighlightCurrentLine;
            }
            set
            {
                m_bHighlightCurrentLine = value;
                Invalidate();
            }
        }
        /// <summary>
        /// Gets or sets color of current line highlight.
        /// </summary>
        public Color CurrentLineHighlightColor
        {
            get
            {
                return m_clrCurrentLineHighlight;
            }
            set
            {
                m_clrCurrentLineHighlight = value;
                Invalidate();
            }
        }
        /// <summary>
        /// Gets or sets font that is used while measuring position of column guides.
        /// </summary>
        public Font WordWrapColumnMeasuringFont
        {
            get
            {
                return m_wordWrapColumnMeasuringFont;
            }
            set
            {
                if (value != null)
                {
                    m_wordWrapColumnMeasuringFont = value;
                    m_wordWrapColumnPos = -1;

                    if (m_bWordWrapping && m_wrapMode == WordWrapMode.SpecifiedColumn)
                    {
                        RemeasureLinesWrapping();
                        InvalidateAll();
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets column for wrapping text. Used when WordWrapMode is set to SpecifiedColumn.
        /// </summary>
        public int WordWrapColumn
        {
            get
            {
                return m_iWordWrapColumn;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("WordWrapColumn");
                }

                if (value != m_iWordWrapColumn)
                {
                    m_iWordWrapColumn = value;
                    m_wordWrapColumnPos = -1;

                    if (m_bWordWrapping && m_wrapMode == WordWrapMode.SpecifiedColumn)
                    {
                        RemeasureLinesWrapping();
                        InvalidateAll();
                    }
                }
            }
        }
        Size m_CodeSnipptSize = new Size(100, 100);
        /// <summary>
        /// Gets or Sets the codesnippet size
        /// </summary>
        public Size CodeSnipptSize
        {
            get
            {
                return m_CodeSnipptSize;
            }
            set
            {
                if (!value.IsEmpty)
                {
                    m_CodeSnipptSize = value;
                    m_codeSnippetsEditBox.FormSize = value;
                }
                else throw new ArgumentOutOfRangeException("FormSize");
            }
        }
        /// <summary>
        /// Gets or sets delay for tooltips in milliseconds.
        /// </summary>
        public int ToolTipDelay
        {
            get
            {
                return m_tip.ShowDelay;
            }
            set
            {
                if (value < 1) throw new ArgumentOutOfRangeException("ToolTipDelay");

                m_tip.ShowDelay = value;
                m_bookmarksTooltip.ShowDelay = value;
            }
        }
        /// <summary>
        /// Gets or sets value indicating whether inserting text should be allowed at the beginning of readonly region at the start of new line.
        /// </summary>
        public bool AllowInsertBeforeReadonlyNewLine
        {
            get
            {
                return m_bAllowInsertBeforeReadonlyNewLine;
            }
            set
            {
                m_bAllowInsertBeforeReadonlyNewLine = value;
            }
        }
        /// <summary>
        /// Gets or sets value indicating whether scrollers should be always visible.
        /// </summary>
        public bool AlwaysShowScrollers
        {
            get
            {
                return m_bAlwaysShowScrollers;
            }
            set
            {
                m_bAlwaysShowScrollers = value;
                UpdateScrollBarsVisibility();
            }
        }
        /// <summary>
        /// Gets or sets value indicating whether mouse cursor should be changed by control when needed.
        /// </summary>
        public bool AllowMouseCursorChange
        {
            get
            {
                return m_bAllowMouseCursorChange;
            }
            set
            {
                m_bAllowMouseCursorChange = value;
            }
        }
        /// <summary>
        /// Gets or sets placement of user margin.
        /// </summary>
        public MarginPlacement UserMarginPlacement
        {
            get
            {
                return m_userMarginPlacement;
            }
            set
            {
                if (m_userMarginPlacement != value)
                {
                    m_userMarginPlacement = value;
                    UpdateScrollerOffsets(this);
                    UpdateScrollBarsSize();
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets selection of the line number.
        /// </summary>
        public bool SelectOnLineNumbersClick
        {
            get
            {
                return m_bSelectOnLineNumberClick;
            }
            set
            {
                if (m_bSelectOnLineNumberClick != value)
                {
                    m_bSelectOnLineNumberClick = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets value indicating whether native GDI should be used for text output.
        /// </summary>
        public bool UseNativeDrawing
        {
            get
            {
                return m_bNativeDrawing;
            }
            set
            {
                if (this.UseNativeDrawing != value)
                {
                    m_parser.UseNativeDrawing = value;
                    m_bNativeDrawing = value;
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets or sets space between lines.
        /// </summary>
        public int SpaceBetweenLines
        {
            get
            {
                return m_spaceBetweenLines;
            }
            set
            {
                if (this.m_spaceBetweenLines != value)
                {
                    m_parser.SpaceBetweenLines = value;
                    m_spaceBetweenLines = value;
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Specifies whether line selection should be extended to far right.
        /// </summary>
        public bool ExtendSelectionToFarRight
        {
            get
            {
                return m_bExtendSelectionToFarRight;
            }
            set
            {
                if (this.m_bExtendSelectionToFarRight != value)
                {
                    m_bExtendSelectionToFarRight = value;
                }
            }
        }
        /// <summary>
        /// Specifies selection of dragged text.
        /// </summary>
        public bool SelectTextAfterDragDrop
        {
            get
            {
                return m_bSelectTextAfterDragDrop;
            }
            set
            {
                if (this.m_bSelectTextAfterDragDrop != value)
                {
                    m_bSelectTextAfterDragDrop = value;
                }
            }
        }
        /// <summary>
        /// gets or sets value specifying Specifies whether tab stops should be respected on inserting blocks of text.
        /// </summary>
        public bool RespectTabStopsOnInsertingText
        {
            get
            {
                return m_bRespectTabStopsOnInsertingText;
            }
            set
            {
                m_bRespectTabStopsOnInsertingText = value;
            }
        }

        //To fix SD 56 -Increase indent does not work properly
        /// <summary>
        /// Indicates whether indent button is clicked
        /// </summary>
        [Browsable(false)]
        public bool FromIndentClick
        {
            get
            {
                return this.fromIndentClick;
            }
            set
            {
                this.fromIndentClick = value;
            }
        }
       

        #endregion

        #region Nonpublic Properties
        /// <summary>
        /// Gets or sets value that indicates whether control is currently in text-selection mode.
        /// </summary>
        protected bool IsSelecting
        {
            get
            {
                bool bKeysOK = (Control.ModifierKeys == Keys.Shift ||
                    Control.ModifierKeys == (Keys.Shift | Keys.Control) || Control.ModifierKeys == (Keys.Shift | Keys.Alt));
                bool bSelecting = (m_bSelecting || this.Focused && m_bAllowShiftSelectionOld && bKeysOK);
                return (m_iSelectionLocks == 0 && bSelecting && !m_controllerContextChoice.AutoCompleteStringShown);
            }
            set
            {
                m_bSelecting = value;
            }
        }
        /// <summary>
        /// Gets value that indicates whether control is currently in block text-selection mode.
        /// </summary>
        protected bool IsBlockSelecting
        {
            get
            {
                return false;
                /*Keys modifierKeys = Control.ModifierKeys;
                return ( this.IsSelecting && modifierKeys == ( modifierKeys | Keys.Alt ) );*/
            }
        }
        /// <summary>
        /// Gets or sets rectangle that show position at which dragged object will be inserted.
        /// </summary>
        protected RectangleF DragDropRectangle
        {
            get
            {
                return m_rectDragOverPosition;
            }
            set
            {
                if (value != m_rectDragOverPosition)
                {
                    RectangleF oldRect = m_rectDragOverPosition;
                    m_rectDragOverPosition = value;
                    InvalidateRelativeRect(oldRect);
                    InvalidateRelativeRect(value);
                }
            }
        }
        /// <summary>
        /// Gets or sets the client rectangle of the FakeEdit control.
        /// </summary>
        private Rectangle fakeEditArea;
        /// <summary>
        /// Gets or sets the client rectangle of the FakeEdit control
        /// </summary>
        public Rectangle FakeEditArea
        {
            get { return fakeEditArea; }
            set { fakeEditArea = value; }
        }
        /// <summary>
        /// Gets or Sets the printable area.
        /// </summary>
        private RectangleF printableArea;
        /// <summary>
        /// Gets or Sets the printable area.
        /// </summary>
        internal RectangleF PrintableArea
        {
            get { return printableArea; }
            set { printableArea = value; }
        }

        /// <summary>
        /// gets or sets a value indicating whether drawing is done for FakeEdit control.
        /// </summary>
        private bool forFakeEdit = false;
        /// <summary>
        /// gets or sets a value indicating whether drawing is done for FakeEdit control.
        /// </summary>
        internal bool ForFakeEdit
        {
            get { return forFakeEdit; }
            set { forFakeEdit = value; }
        }
        /// <summary>
        /// Gets or sets whether printing is in progress.
        /// </summary>
        private bool forPrinting = false;
        /// <summary>
        /// Gets or sets whether printing is in progress.
        /// </summary>
        internal bool ForPrinting
        {
            get { return forPrinting; }
            set { forPrinting = value; }
        }

        /// <summary>
        /// Gets or sets offset used for drawing text. It includes offset for LineNumbers, Markers and Collapsing if they are enabled.
        /// </summary>
        internal int TextDrawOffset
        {
            get
            {
                int result = DEF_PRE_TEXT_AREA + ((this.ShowLineNumbers) ? (m_lineNumbersWidth) : (0));
                result += (this.ShowMarkers) ? (m_markerAreaWidth) : (0);
                result += (this.ShowCollapse) ? (DEF_COLLAPSE_AREA) : (0);
                result += (m_bSelectionMarginVisible) ? (m_iSelectionMarginWidth) : (0);
                result += (m_bShowUserMargin && m_userMarginPlacement == MarginPlacement.Left) ? (m_iUserMarginWidth) : (0);

                if (this.RightToLeft == RightToLeft.Yes)
                {
                    if (ForPrinting)
                        return (int)PrintableArea.Right - result;
                    else if (ForFakeEdit)
                        return this.FakeEditArea.Right - result;
                    else
                        return this.ClientRectangle.Right - result;
                }
                else
                    return result;
            }
        }
        /// <summary>
        /// Gets or Sets Start value of virtual line number.
        /// </summary>
        public int VirtualLineNumberOffset
        {
            get { return m_virtualLineNumberOffset; }
            set { m_virtualLineNumberOffset = value; }
        }
        /// <summary>
        /// Gets offset of the line-numbers area.
        /// </summary>
        protected int LineNumbersAreaOffset
        {
            get
            {
                if (this.RightToLeft == RightToLeft.Yes)
                    return this.MarkerAreaOffset - (this.ShowLineNumbers ? m_lineNumbersWidth : 0);
                else
                    return this.ShowMarkers ? this.MarkerAreaWidth : 0 ;
            }
        }
        /// <summary>
        /// Gets offset of the collapsing area.
        /// </summary>
        protected internal int CollapsingAreaOffset
        {
            get
            {
                if(this.RightToLeft == RightToLeft.Yes)
                    return this.LeftUserMarginOffset - ((this.ShowCollapse) ? DEF_COLLAPSE_AREA : 0);
                else
                    return this.LeftUserMarginOffset +
                        ((m_bShowUserMargin && m_userMarginPlacement == MarginPlacement.Left) ? (m_iUserMarginWidth) : (0));
            }
        }
        /// <summary>
        /// Gets offset of the user margin for it's positioning on the left.
        /// </summary>
        protected internal int LeftUserMarginOffset
        {
            get
            {
                if (this.RightToLeft == RightToLeft.Yes)
                    return this.LineNumbersAreaOffset - ((this.ShowUserMargin && UserMarginPlacement == MarginPlacement.Left) ? m_iUserMarginWidth : 0);
                else
                    return this.LineNumbersAreaOffset + ((this.ShowLineNumbers) ? (m_lineNumbersWidth) : (0));
            }
        }
        /// <summary>
        /// Specifies how the control process vertical scrolling.
        /// </summary>
        public ScrollMode VScrollMode
        {
            get
            {
                return this.vScrollMode;
            }
            set
            {
                this.vScrollMode = value;
            }
        }
        /// <summary>
        /// Gets offset of the selection margin area.
        /// </summary>
        protected int SelectionMarginOffset
        {
            get
            {
                if (this.RightToLeft == RightToLeft.Yes)
                    return this.CollapsingAreaOffset - (ShowSelectionMargin ? this.SelectionMarginWidth : 0);
                else
                    return this.CollapsingAreaOffset + (ShowCollapse ? DEF_COLLAPSE_AREA : 0);
            }
        }
        /// <summary>
        /// Gets offset of the selection line number area.
        /// </summary>
        protected int SelectionLineNumberOffset
        {
            get
            {
                return this.LineNumbersAreaOffset;
            }
        }
        /// <summary>
        /// Gets value indicating whether autocomplete dialog is opened and ready for keyboard processing.
        /// </summary>
        protected bool ContextChoiceOn
        {
            get
            {
                return m_controllerContextChoice.IsVisible;
            }
        }
        /// <summary>
        /// Gets value indicating whether ContextPrompt dialog is opened.
        /// </summary>
        protected bool ContextPromptOn
        {
            get
            {
                return (m_contextPrompt != null && m_contextPrompt.Visible);
            }
        }
        /// <summary>
        /// Get pen to draw user margin area border.
        /// </summary>
        internal Pen UserMarginAreaBorderPen
        {
            get
            {
                if (m_clrUserMarginBorder != m_penUserMarginAreaBorder.Color)
                {
                    m_penUserMarginAreaBorder.Color = m_clrUserMarginBorder;
                }
                return m_penUserMarginAreaBorder;
            }
        }
        /// <summary>
        /// Get pen to draw line that delimits text area.
        /// </summary>
        internal Pen TextAreaLinePen
        {
            get
            {
                if (m_textAreaLineStyle != m_penTextAreaLine.DashStyle)
                {
                    m_penTextAreaLine.DashStyle = m_textAreaLineStyle;
                }
                if (m_clrTextAreaLine != m_penTextAreaLine.Color)
                {
                    m_penTextAreaLine.Color = m_clrTextAreaLine;
                }
                return m_penTextAreaLine;
            }
        }
        /// <summary>
        /// Gets DynamicFormatManager.
        /// </summary>
        internal DynamicFormatManager DynamicFormatManager
        {
            get
            {
                return m_formatManager;
            }
        }
        /// <summary>
        /// Gets or sets find dialog form.
        /// </summary>
        internal IFindDialogForm FindDialogWnd
        {
            get
            {
                if (m_findDialog == null)
                {
                    m_findDialog = new FrmFindDialog(this);
                }
                return m_findDialog;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("FindDialogWnd");

                m_findDialog = value;
            }
        }
        /// <summary>
        /// Gets or sets replace dialog form.
        /// </summary>
        internal IReplaceDialogForm ReplaceDialogWnd
        {
            get
            {
                if (m_replaceDialog == null)
                {
                    m_replaceDialog = new frmReplaceDialog(this);
                }
                return m_replaceDialog;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("ReplaceDialogWnd");

                m_replaceDialog = value;
            }
        }
        /// <summary>
        /// Gets or sets goto dialog form.
        /// </summary>
        internal IGotoDialogForm GotoDialogWnd
        {
            get
            {
                if (m_gotoDialog == null)
                {
                    Form owner = this.TopLevelControl as Form;
                    if (owner == null)
                    {
                        m_gotoDialog = new FrmGoDialog();
                    }
                    else
                    {
                        m_gotoDialog = new FrmGoDialog(this.TopLevelControl as Form);
                    }
                }
                return m_gotoDialog;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("GotoDialogWnd");

                m_gotoDialog = value;
            }
        }
        /// <summary>
        /// Gets maximum width of the line. Used for WordWrapping.
        /// </summary>
        internal int MaxWidth
        {
            get
            {
                int result = 0;

                if (!this.WordWrap)
                {
                    result = int.MaxValue;
                }
                else
                {
                    switch (m_wrapMode)
                    {
                        case WordWrapMode.Control:
                            {
                                result = this.ClientRectangle.Width - this.ScrollOffsetLeft - this.ScrollOffsetRight - 4;

                                if (GetMinimalWidth != null)
                                {
                                    ChangeValueEventArgs args = new ChangeValueEventArgs(result);
                                    GetMinimalWidth(this, args);

                                    result = (int)args.Value;
                                }
                                break;
                            }

                        case WordWrapMode.WordWrapMargin:
                            {
                                result = m_textAreaWidth - 1;
                                break;
                            }

                        case WordWrapMode.SpecifiedColumn:
                            {
                                if (m_wordWrapColumnPos == -1)
                                {
                                    using (Graphics g = CreateGraphics())
                                    {
                                        CalculateWordWrapColumnPos(g);
                                    }
                                }

                                result = m_wordWrapColumnPos;
                                break;
                            }
                    }
                }

                if (this.MarkLineWrapping)
                {
                    result -= this.WrapMarkingImage.Width;
                }

                return result;
            }
        }
        /// <summary>
        /// Gets context choice controller.
        /// </summary>
        internal ContextChoiceController ContextChoice
        {
            get
            {
                return m_controllerContextChoice;
            }
        }
        /// <summary>
        /// Gets code snippets manager.
        /// </summary>
        internal CodeSnippetsManager CodeSnippetsManager
        {
            get
            {
                return m_codeSnippetsManager;
            }
        }
        /// <summary>
        /// Gets or sets value indicating whether selection should be performed when shift button is pressed.
        /// </summary>
        protected internal bool AllowShiftSelectionOld
        {
            get
            {
                return m_bAllowShiftSelectionOld;
            }
            set
            {
                m_bAllowShiftSelectionOld = value;
            }
        }
        /// <summary>
        /// Gets or sets value that indicates whether scrollers are disabled.
        /// </summary>
        protected internal bool DisableScrollers
        {
            get
            {
                return m_bDisableScrollers || SingleLineMode;
            }
            set
            {
                if (m_bDisableScrollers != value)
                {
                    m_bDisableScrollers = value;
                    OnDisableScrollersChanged();
                }
            }
        }
        /// <summary>
        /// Gets cursor manager.
        /// </summary>
        protected internal CursorManager CursorManager
        {
            get
            {
                return m_CursorManager;
            }
        }
        /// <summary>
        /// Gets instance of the current line.
        /// </summary>
        protected internal RenderedLine CurrentLineInstanceInternal
        {
            get
            {
                CheckControlState();
                return m_parser.GetLine(CurrentLine) as RenderedLine;
            }
        }
        /// <summary>
        /// Gets or Sets indent Guideline region info.
        /// </summary>
        private IndentGuidelineRegionInfo IndentGuideline
        {
            get
            {
                return m_indentGuidelineInfo;
            }
            set
            {
                if (m_indentGuidelineInfo != value)
                {
                    m_indentGuidelineInfo = value;
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Gets current subline.
        /// </summary>
        private int CurrentSubLine
        {
            get
            {
                RenderedLexem lexem = (RenderedLexem)GetLexemUnderCursor();
                return (lexem == null) ? (((RenderedLine)this.CurrentLineInstance).SubLinesCount - 1) : (lexem.SubLine);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when InsertMode flag has changed
        /// </summary>
        public event EventHandler InsertModeChanged;
        /// <summary>
        /// Event that is raised when current cursor position has changed.
        /// </summary>
        public event EventHandler CursorPositionChanged;
        /// <summary>
        /// Event that is raised when text selection is changed.
        /// </summary>
        public event EventHandler SelectionChanged;
        /// <summary>
        /// Event that is raised when Changed State was updated.
        /// </summary>
        public event EventHandler CanUndoRedoChanged;
        /// <summary>
        /// Event that is raised when user margin have to be painted.
        /// </summary>
        public event PaintEventHandler PaintUserMargin;
        /// <summary>
        /// Event that is raised on the start of the long operation.
        /// </summary>
        public event LongOperationEventHandler OperationStarted;
        /// <summary>
        /// Even that is raised on the end of the long operation.
        /// </summary>
        public event LongOperationEventHandler OperationStopped;
        /// <summary>
        /// Event that is raised when current stream instance is to be changed to some other one.
        /// </summary>
        public virtual event ChangingStreamEventHandler ChangingStream;
        /// <summary>
        /// Event that is raised when ReadOnly mode changes.
        /// </summary>
        public event EventHandler ReadOnlyChanged;
        /// <summary>
        /// Event that is raised when class registers default key bindings.
        /// </summary>
        public event EventHandler RegisteringDefaultKeyBindings;
        /// <summary>
        /// Event that is raised when class registers commands for key-binding.
        /// </summary>
        public event EventHandler RegisteringKeyCommands;
        /// <summary>
        /// Event that is raised after changing configuration.
        /// </summary>
        public event EventHandler ConfigurationChanged;
        /// <summary>
        /// Event that is raised after changing parsers language.
        /// </summary>
        public event EventHandler LanguageChanged;
        /// <summary>
        /// Event that is raised before context prompt should be shown.
        /// </summary>
        public event CancelEventHandler ContextPromptBeforeOpen;
        /// <summary>
        /// Event that is raised when context prompt should be shown.
        /// </summary>
        public event ContextPromptUpdateEventHandler ContextPromptOpen;
        /// <summary>
        /// Event that is raised when context prompt should be shown.
        /// </summary>
        public event ContextPromptUpdateEventHandler ContextPromptUpdate;
        /// <summary>
        /// Event that is raised when context prompt should be shown.
        /// </summary>
        public event ContextPromptCloseEventHandler ContextPromptClose;
        /// <summary>
        /// Event that is raised when text should be updated.
        /// </summary>
        public event UpdateTooltipEventHandler UpdateContextToolTip;
        /// <summary>
        /// Event that is raised when bookmark tooltip text should be updated.
        /// </summary>
        public event UpdateBookmarkTooltipEventHandler UpdateBookmarkToolTip;
        /// <summary>
        /// Event, that is raised, when user should fill menu with menu items.
        /// </summary>
        public event EventHandler MenuFill;
        /// <summary>
        /// Raised, when single line mode has been changed.
        /// </summary>
        public event EventHandler SingleLineChanged;
        /// <summary>
        /// Event, that is raised when text is changed.
        /// </summary>
        public new event EventHandler TextChanged;
        /// <summary>
        /// Event, that is raised once new match is found in FindAndReplaceDialogBox via Find Next button.
        /// </summary>
        public event EventHandler Find;

        /// <summary>
        /// Event, that is raised when text is to be changed.
        /// </summary>
        public event TextChangingEventHandler TextChanging;
        /// <summary>
        /// Event, that is raised when text in line is to be changed.
        /// </summary>
        public event TextChangedEventHandler LineChanged;
        /// <summary>
        /// Event, that is raised when text in line is to be inserted.
        /// </summary>
        public event LineInsertedEventHandler LineInserted;
        /// <summary>
        /// Event, that is raised when text in line is to be inserted.
        /// </summary>
        public event LineDeletedEventHandler LineDeleted;
        /// <summary>
        /// Event, that is raised when context prompt selection is changed.
        /// </summary>
        public event ContextPromptSelectionChangedEventHandler ContextPromptSelectionChanged;
        /// <summary>
        /// Event, that is raised control's area needs to be invalidated.
        /// </summary>
        internal event InvalidateAreaEventHandler InvalidateArea;
        /// <summary>
        /// Event, that is raised when word-wrap mode is changed.
        /// </summary>
        public event EventHandler WordWrapChanged;
        /// <summary>
        /// Event that is raised when editor is about to insert text of the selected context choice item.
        /// </summary>
        public event ContextChoiceTextInsertEventHandler ContextChoiceSelectedTextInsert;
        /// <summary>
        /// Event that is raised when page header should be printed.
        /// </summary>
        public event PrintHeadlineEventHandler PrintHeader;
        /// <summary>
        /// Event that is raised when page footer should be printed.
        /// </summary>
        public event PrintHeadlineEventHandler PrintFooter;
        /// <summary>
        /// Event that is raised when user clicks in the indicator margin area.
        /// </summary>
        public event IndicatorClickEventHandler IndicatorMarginClick;
        /// <summary>
        /// Event that is raised when user double-clicks in the indicator margin area.
        /// </summary>
        public event IndicatorClickEventHandler IndicatorMarginDoubleClick;
        /// <summary>
        /// Event that is raised when control painting should be locked.
        /// </summary>
        internal event EventHandler PaintLockRequest;
        /// <summary>
        /// Event that is raised when control painting should be unlocked.
        /// </summary>
        internal event EventHandler PaintUnlockRequest;
        /// <summary>
        /// Event that is raised when user margin area text is ready to be drawn.
        /// </summary>
        public event DrawUserMarginTextEventHandler DrawUserMarginText;
        /// <summary>
        /// Event that is raised when user tries to save stream with data loosing.
        /// </summary>
        public event SaveWithDataLosingEventHandler SaveStreamWithDataLoss;
        /// <summary>
        /// Event that is raised when parser is created.
        /// </summary>
        public event EventHandler ParserCreated;
        /// <summary>
        /// Event that is raised when parser is destroyed.
        /// </summary>
        public event EventHandler ParserDestroyed;
        /// <summary>
        /// Event that is raised when outlining tooltip is about to be shown.
        /// </summary>
        public event OutliningTooltipBeforePopupEventHandler OutliningTooltipBeforePopup;
        /// <summary>
        /// Event that is raised when outlining tooltip is shown.
        /// </summary>
        public event CollapsedRegionRelatedEventHandler OutliningTooltipPopup;
        /// <summary>
        /// Event that is raised when outlining tooltip is closed.
        /// </summary>
        public event CollapsedRegionRelatedEventHandler OutliningTooltipClose;
        /// <summary>
        /// Event that is raised when encoding is possibly changed.
        /// </summary>
        public event EncodingChangedEventHandler EncodingChanged;
        /// <summary>
        /// Event that is raised before the line number is drawn.
        /// </summary>
        public event OnBeforeLineNumberPaintEventHandler OnBeforeLineNumberPaint;
        /// <summary>
        /// Event that is raised when minimal width of every fake control should be retrieved.
        /// </summary>
        public event ChangeValueEventHandler GetMinimalWidth;
        /// <summary>
        /// Raised when text in hidden block is found and this block can't be expanded due to user's cancelling.
        /// </summary>
        public event UnreachableTextFoundEventHandler UnreachableTextFound;
        /// <summary>
        /// Event that is raised when DisableScrollers property value is changed.
        /// </summary>
        public event EventHandler DisableScrollersChanged;
        /// <summary>
        /// Raised when CollapseAll method is called.
        /// </summary>
        public event CancelEventHandler CollapsingAll;
        /// <summary>
        /// Raised when ExpandeAll method is called.
        /// </summary>
        public event CancelEventHandler ExpandingAll;
        /// <summary>
        /// Raised when CollapseAll method was called.
        /// </summary>
        public event EventHandler CollapsedAll;
        /// <summary>
        /// Raised when ExpandeAll method was called.
        /// </summary>
        public event EventHandler ExpandedAll;
        /// <summary>
        /// Raised when new document is created within editor.
        /// </summary>
        public event EventHandler NewDocCreated;
        #endregion
        bool bEnableMD5 = true ;
        #region Initialization & Finalization
        /// <summary>
        /// Initializes static members.
        /// </summary>
        static StreamEditControl()
        {
            //_md5 = MD5.Create();
        }
        /// <summary>
        /// Creates new instance of StreamEditControl class and initializes it.
        /// </summary>
        public StreamEditControl()
        {
#if !NO_LICENSE
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(StreamEditControl));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
#endif

            m_savedLinesPoints = new ArrayList();

            m_mainThreadId = (uint)Thread.CurrentThread.ManagedThreadId;

            m_tracerInvalidation = new InvalidationStackTracer(this);

            BackColor = SystemColors.Window;
            m_brushBackground = BrushInfo.Empty;

            m_GreenDotsPen.DashStyle = DashStyle.Dot;

            ControlStyles styleTrue = ControlStyles.Selectable |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.UserMouse |
                ControlStyles.StandardClick |
                ControlStyles.StandardDoubleClick |
                ControlStyles.ContainerControl |
                ControlStyles.UserPaint;

            ControlStyles styleFalse = ControlStyles.CacheText;

            SetStyle(styleTrue, true);
            SetStyle(styleFalse, false);

            m_Configuration = new Config(this.DesignMode, false);
            m_Configuration.ConfigurationChanged += new EventHandler(OnConfiguratorChanged);
            m_Configuration.FormatsChanged += new EventHandler(OnFormatsConfigurationChanged);
            m_bookmarkhelper = new BookmarksHelper(this);
            m_controllerContextChoice = new ContextChoiceController(this, true);
            m_controllerContextChoice.FormLoad += new EventHandler(OnControllerContextChoiceFormLoad);
            m_controllerContextChoice.ContextChoiceClose += new ContextChoiceCloseEventHandler(OnContextChoiceClosed);
            m_controllerContextChoice.ContextChoiceAutoComplete += new ContextChoiceEventHandler(OnControllerContextChoiceAutoComplete);

            m_codeSnippetsEditBox = new CodeSnippetsEditBox("Insert snippet:  ", this);

            m_codeSnippetsPopupController = new CodeSnippetsPopupController(this, m_codeSnippetsEditBox);
            m_codeSnippetsPopupController.FormLoad += new EventHandler(OnCodeSnippetsPopupControllerFormLoad);
            m_codeSnippetsPopupController.ContextChoiceOpen += new ContextChoiceEventHandler(OnCodeSnippetsPopupControllerContextChoiceOpen);
            m_codeSnippetsPopupController.ContextChoiceClose += new ContextChoiceCloseEventHandler(OnCodeSnippetsPopupControllerContextChoiceClose);
            m_codeSnippetsEditBox.ContextChoiceController = m_codeSnippetsPopupController;

            // attach to event anf catch any desktop changes
            SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);

            m_centFormat.Alignment = StringAlignment.Near;

            m_scanTimer_Callback = new System.Windows.Forms.Timer();
            m_scanTimer_Callback.Interval = DEF_IDLE_TIMER_DELAY;
            m_scanTimer_Callback.Tick += new EventHandler(OnIdleTextParsing);

            this.ParserCreated += new EventHandler(OnStreamEditControlParserCreated);

            New();

            m_keyBinder = new KeyProcessor();
            m_keyBinder.InitializeClassDefaults(this);
            m_keyBinder.UnprocessedKey += new ProcessCommandsEventHandler(OnUnprocessedKeyPress);

            this.VScrollBar.SmallChange = (int)m_parser.DefaultLineHeight;
            this.HScrollBar.SmallChange = (int)m_parser.DefaultLineHeight;
            m_tip = new ToolTipEx(this);
            m_tip.UpdateTooltip += new UpdateTooltipEventHandler(OnUpdateToolTip);
            m_tip.VisibleChanged += new EventHandler(OnTipVisibleChanged);

            m_bookmarksTooltip = new ToolTipEx(this);
            m_bookmarksTooltip.UpdateTooltip += new UpdateTooltipEventHandler(OnBookmarksTooltipUpdateTooltip);

            m_timerAutoIndent = new System.Windows.Forms.Timer();
            m_timerAutoIndent.Interval = 1000;
            m_timerAutoIndent.Stop();
            m_timerAutoIndent.Tick += new EventHandler(OnTimerIndentGuidelineShowTick);

            m_menuManager = new ContextMenuManager(this);
            m_menuManager.FillMenu += new EventHandler(OnFillMenu);
            
            ShowWhiteSpaceProperties.Change += new EventHandler(OnShowWhiteSpacePropertiesChange);

            m_autoFormattingManager.RegisterFormatter(KnownLanguages.CSharp, new CSFormatter());

            m_codeSnippetsManager = new CodeSnippetsManager(this);
        }

        void ContextMenuProvider_Popup(object sender, EventArgs e)
        {
            isShowing = true;          
        } 

        /// <summary>
        /// Disposes object and frees all used resources and suppresses object's finalization.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            lock (this)
            {
                m_controllerContextChoice.FormLoad -= new EventHandler(OnControllerContextChoiceFormLoad);
                m_controllerContextChoice.ContextChoiceClose -= new ContextChoiceCloseEventHandler(OnContextChoiceClosed);
                m_controllerContextChoice.ContextChoiceAutoComplete -= new ContextChoiceEventHandler(OnControllerContextChoiceAutoComplete);
                m_controllerContextChoice.Dispose();

                m_codeSnippetsPopupController.ContextChoiceClose -= new ContextChoiceCloseEventHandler(OnCodeSnippetsPopupControllerContextChoiceClose);
                m_codeSnippetsPopupController.FormLoad -= new EventHandler(OnCodeSnippetsPopupControllerFormLoad);
                m_codeSnippetsPopupController.ContextChoiceOpen -= new ContextChoiceEventHandler(OnCodeSnippetsPopupControllerContextChoiceOpen);
                m_codeSnippetsPopupController.Dispose();

                m_menuManager.FillMenu -= new EventHandler(OnFillMenu);
                m_menuManager.Dispose();

                m_scanTimer_Callback.Tick -= new EventHandler(OnIdleTextParsing);
                m_timerAutoIndent.Tick -= new EventHandler(OnTimerIndentGuidelineShowTick);

                m_Configuration.ConfigurationChanged -= new EventHandler(OnConfiguratorChanged);
                m_Configuration.FormatsChanged -= new EventHandler(OnFormatsConfigurationChanged);

                ShowWhiteSpaceProperties.Change -= new EventHandler(OnShowWhiteSpacePropertiesChange);

                SystemEvents.DisplaySettingsChanged -= new EventHandler(SystemEvents_DisplaySettingsChanged);

                this.ParserCreated -= new EventHandler(OnStreamEditControlParserCreated);

                if (m_contextPrompt != null)
                {
                    m_contextPrompt.Dispose();
                }

                if (m_Configuration != null) m_Configuration.Dispose();

                m_tip.UpdateTooltip -= new UpdateTooltipEventHandler(OnUpdateToolTip);
                m_tip.VisibleChanged -= new EventHandler(OnTipVisibleChanged);
                m_tip.Dispose();
                m_tip = null;

                m_bookmarksTooltip.UpdateTooltip -= new UpdateTooltipEventHandler(OnBookmarksTooltipUpdateTooltip);
                m_bookmarksTooltip.Dispose();
                m_bookmarksTooltip = null;

                m_bookmarkhelper = null;

                m_keyBinder.UnprocessedKey -= new ProcessCommandsEventHandler(OnUnprocessedKeyPress);
                m_keyBinder = null;
                m_codeSnippetsManager = null;

                base.Dispose(disposing);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Locks processing of the configuration change events.
        /// </summary>
        public void BeginConfigurationChange()
        {
            m_iSuppressConfigurationChangeNotification++;
        }
        /// <summary>
        /// Unlocks processing of the configuration change events.
        /// </summary>
        /// <param name="update">Indicates whether control should be updated.</param>
        public void EndConfigurationChange(bool update)
        {
            if (0 < m_iSuppressConfigurationChangeNotification)
            {
                m_iSuppressConfigurationChangeNotification--;
            }

            if (update && m_iSuppressConfigurationChangeNotification == 0)
            {
                OnConfigurationChanged();
            }
        }
        /// <summary>
        /// Loads configuration from stream.
        /// </summary>
        /// <param name="configStream">Stream with config.</param>
        public void LoadConfig(Stream configStream)
        {
            if (configStream == null) throw new ArgumentNullException("configStream");

            lock (this)
            {
                m_Configuration.Open(configStream);
                OnConfigurationChanged();
            }
        }
        /// <summary>
        /// Reads configuration from file.
        /// </summary>
        /// <param name="fileName">Name of the file with configuration.</param>
        /// <param name="currentFilePath">Name of the file that is currently loaded, or empty string.</param>
        public void LoadConfig(string fileName, string currentFilePath)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");
            if (fileName.Length == 0)
                throw new ArgumentException(Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_68);
            if (currentFilePath == null) throw new ArgumentNullException("currentFilePath");
            if (currentFilePath.Length == 0)
                throw new ArgumentException(Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_69);

            lock (this)
            {
                m_Configuration.Open(fileName);
                OnConfigurationChanged();
                ResetColoring(GetFileLanguage(currentFilePath));
            }
        }
        /// <summary>
        /// Resets parser.
        /// </summary>
        /// <param name="lang">New language configuration to be set.</param>
        public void ResetColoring(IConfigLanguage lang)
        {
            if (lang == null) throw new ArgumentNullException("lang");

            if (m_parser != null && m_parser.BaseStream != null)
            {
                ControlStateStore stateStore = new ControlStateStore();
                stateStore.StoreData(this);
                if (!this.Language.Cached)
                {
                    CreateParser(m_parser.BaseStream, lang);
                    stateStore.RestoreData(this, false);
                }
                else
                {
                    Refresh();
                }
                RaiseConfigurationChangedEvent();
            }
        }
        /// <summary>
        /// Loads file and configuration for it.
        /// </summary>
        /// <param name="stream">Name of the file to load.</param>
        /// <returns>True, if user has not canceled loading, otherwise false.</returns>
        public bool LoadStream(Stream stream)
        {
            lock (this)
            {
                return LoadStream(stream, m_Configuration.DefaultLanguage);
            }
        }
        /// <summary>
        /// Loads file and configuration for it.
        /// </summary>
        /// <param name="stream">Name of the file to load.</param>
        /// <param name="lang">Config language.</param>
        /// <returns>True, if user has not canceled loading, otherwise false.</returns>
        public bool LoadStream(Stream stream, IConfigLanguage lang)
        {
            return LoadStream(stream, lang, null);
        }
        /// <summary>
        /// Loads file and configuration for it.
        /// </summary>
        /// <param name="stream">Name of the file to load.</param>
        /// <param name="lang">Config language.</param>
        /// <param name="encoding">Encoding to use.</param>
        /// <returns>True, if user has not canceled loading, otherwise false.</returns>
        public bool LoadStream(Stream stream, IConfigLanguage lang, Encoding encoding)
        {
            if (stream == null) throw new ArgumentNullException("fileName");

            bool bResult = false;

            lock (this)
            {
                if (CloseStream())
                {
                    using (((ILongOperationController)this).StartOperation("Loading stream"))
                    {
                        m_wrapper = new StreamsWrapper(stream, m_defaultNewLineStyle, encoding);
                        CreateParser(m_wrapper, lang);
                        CurrentPosition = new Point(1, 1);
                        m_CursorManager.Update();
                        RaiseUpdateChangedStateEvent();
                        bResult = true;
                    }
                }
            }

            return bResult;

        }
        /// <summary>
        /// Creates empty stream and makes editor to edit it.
        /// </summary>
        /// <returns>True if operation succeeds.</returns>
        bool count = false;
        /// <summary>
        /// Creates new configuration
        /// </summary>
        /// <returns></returns>
        public virtual bool New()
        {
            count = New(m_Configuration.DefaultLanguage);
            lock (this)
            {
                if (count)
                    ccount++;
                return New(m_Configuration.DefaultLanguage);
            }
        }
        /// <summary>
        /// initiates ccount to be zero
        /// </summary>
        public int ccount = 0;
        /// <summary>
        /// Creates empty stream and makes editor to edit it.
        /// </summary>
        /// <param name="lang">Config language.</param>
        /// <returns>True if operation succeeds.</returns>
        public virtual bool New(IConfigLanguage lang)
        {
            bool bResult = false;

            lock (this)
            {
                if (CloseStream())
                {
                    m_wrapper = new StreamsWrapper(new MemoryStream(0), m_defaultNewLineStyle);
                    CreateParser(m_wrapper, lang);
                    RecalculateSpaces(PhysicalLineCount);
                    CurrentPosition = new Point(1, 1);
                    RaiseUpdateChangedStateEvent();
                    m_CursorManager.Update();

                    if (m_wrapper.NewLineStyle != this.DefaultNewLineStyle)
                    {
                        SetNewLineStyle(this.DefaultNewLineStyle);
                    }
                    if (NewDocCreated != null)
                    {
                        NewDocCreated(this, EventArgs.Empty);
                    }

                    bResult = true;
                }
            }

            return bResult;
        }
        /// <summary>
        /// Flushes changes to the current stream.
        /// </summary>
        public void SaveToStream()
        {
            lock (this)
            {
                if (m_parser != null && !ReadOnly)
                {
                    m_parser.BaseStream.Save();
                    RaiseUpdateChangedStateEvent();
                    if(FlushSavedLines)
                        m_savedLinesPoints.Clear();
                }
            }
        }
        /// <summary>
        /// Converts encoding and new line style of the input stream; returns result stream. Input stream is closed.
        /// </summary>
        /// <param name="stream">Input stream.</param>
        /// <param name="encoding">Encoding to use when saving to stream.</param>
        /// <param name="lineEndString">String representing line end style.</param>
        /// <param name="bDataLost">bool indicating whether data lost happened.</param>
        /// <returns>Resulting stream or null if no recoding is done.</returns>
        public Stream ConvertEncodingAndNewLine(Stream stream, Encoding encoding, string lineEndString, out bool bDataLost)
        {
            Stream result = null;
            bDataLost = false;

            if (encoding != null && lineEndString != null && lineEndString != string.Empty)
            {
                result = ConvertStream(stream, lineEndString, encoding, out bDataLost);
            }
            else if (encoding != null)
            {
                result = ConvertStream(stream, encoding, out bDataLost);
            }
            else if (lineEndString != null && lineEndString != string.Empty)
            {
                result = ConvertStream(stream, lineEndString, out bDataLost);
            }

            return result;
        }
        /// <summary>
        /// Saves data from current stream to the specified one.
        /// </summary>
        /// <param name="stream">Output stream.</param>
        /// <param name="encoding">Encoding to use when saving to stream.</param>
        /// <param name="newLineString">String representing line end style used when saving to stream.</param>
        /// <returns>Bool indicating whether saving succeeded.</returns>
        public bool SaveToStream(Stream stream, Encoding encoding, string newLineString)
        {
            bool bResult = false;

            if (m_parser != null)
            {
                lock (this)
                {
                    MemoryStream memStream = new MemoryStream();
                    m_parser.BaseStream.SaveTo(memStream);

                    bool bDataLost;
                    MemoryStream convertedStream = (MemoryStream)ConvertEncodingAndNewLine(memStream, encoding, newLineString, out bDataLost);
                    bool bUserHandled = false;
                    if (bDataLost)
                    {
                        bool bHandled = false;
                        if (SaveStreamWithDataLoss != null)
                        {
                            SaveWithDataLosingEventArgs args = new SaveWithDataLosingEventArgs();
                            if (SaveStreamWithDataLoss != null)
                            {
                                SaveStreamWithDataLoss(this, args);
                            }
                            if (args.UserHandling)
                            {
                                if (args.SaveWithLoss)
                                {
                                    bHandled = true;
                                }
                                else
                                {
                                    bUserHandled = true;
                                }
                            }
                        }

                        if (!bUserHandled && !bHandled
                            && MessageBox.Show(Localizer.GetString(Localizer.DEF_MSG_SAVE_STREAM_USING_ENCODING), Localizer.GetString(Localizer.DEF_MSG_SAVE_STREAM_USING_ENCODING_CAPTION),
                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.No)
                        {
                            bUserHandled = true;
                        }
                    }

                    if (!bUserHandled)
                    {
                        if (null == convertedStream)
                        {
                            convertedStream = new MemoryStream();
                            m_parser.BaseStream.SaveTo(convertedStream);
                        }
                        convertedStream.WriteTo(stream);
                        bResult = true;
                    }
                }
            }

            return bResult;
        }
        /// <summary>
        /// Saves data from current stream to the specified one.
        /// </summary>
        /// <param name="stream">Output stream.</param>
        public void SaveToStream(Stream stream)
        {
            if (m_parser != null)
            {
                lock (this)
                {
                    m_parser.BaseStream.SaveTo(stream);
                }
            }
        }
        /// <summary>
        /// Discards all unsaved changes.
        /// </summary>
        internal void DiscardChanges()
        {
            if (m_parser != null)
            {
                lock (this)
                {
                    UpdateLinesMeasuring(this.CurrentLine); // for fixing problems related to application crash on exit.
                    m_parser.BaseStream.DiscardChanges();
                    RaiseUpdateChangedStateEvent();
                    CurrentPosition = new Point(1, 1);
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Returns current ParsePoint.
        /// </summary>
        /// <returns>IParsePoint with current position.</returns>
        public IParsePoint GetRealCursorPosition()
        {
            lock (this)
            {
                CheckControlState();
                return GetNearestParsePointRight(CurrentLine, CurrentColumn).PhysicalPoint;
            }
        }
        /// <summary>
        /// Gets text of the specified line.
        /// </summary>
        /// <param name="iLineIndex">Line index.</param>
        /// <returns>Text of the line.</returns>
        public string GetLineText(int iLineIndex)
        {
            lock (this)
            {
                CheckControlState();
                return GetLineTextInternal(iLineIndex);
            }
        }
        /// <summary>
        /// Gets line's instance.
        /// </summary>
        /// <param name="iLineIndex">Index of the line.</param>
        /// <returns>Instance of the line.</returns>
        public ILexemLine GetLine(int iLineIndex)
        {
            lock (this)
            {
                CheckControlState();
                return m_parser.GetLine(iLineIndex);
            }
        }
        /// <summary>
        /// Appends text.
        /// </summary>
        /// <param name="text">The text to be appended.</param>
        public void AppendText(string text)
        {
            CheckControlState();
            int iLine = VisibleLineCount;
            int iColumn = GetLineLength(iLine);
            TextInsertInternalRespectingTabStops(iLine, iColumn, text);
            InvalidateAll();
        }
        /// <summary>
        /// Inserts text in the given position.
        /// </summary>
        /// <param name="line">Line in virtual coordinates where text should be inserted.</param>
        /// <param name="column">Column in virtual coordinates where text should be inserted.</param>
        /// <param name="text">Text to be inserted.</param>
        public void InsertText(int line, int column, string text)
        {
            TextInsertInternalRespectingTabStops(line, column, text);
            InvalidateAll();
        }
        /// <summary>
        /// Gets word under cursor.
        /// </summary>
        /// <returns>Lexem under cursor.</returns>
        public string GetCurrentWord()
        {
            lock (this)
            {
                int index;
                return GetCurrentWord(out index);
            }
        }
        /// <summary>
        /// Gets column where current word starts.
        /// </summary>
        /// <returns>Index of the column of the word start.</returns>
        public int GetCurrentWordColumn()
        {
            lock (this)
            {
                int index;
                GetCurrentWord(out index);
                return index;
            }
        }
        /// <summary>
        /// Looks for specified expression in text.
        /// </summary>
        /// <param name="start">Start position for the search.</param>
        /// <param name="expression">Expression to be found.</param>
        /// <param name="bSearchInCollapsed">Flag, that specifies whether text can be found in collapsed region.</param>
        /// <param name="searchUp">Indicates whether search should be per</param>
        /// <returns>Search results.</returns>
        public FindResult FindRegex(IParsePoint start, Regex expression, bool bSearchInCollapsed, bool searchUp)
        {
            lock (this)
            {
                FindResult res;
                bool bPointsVisible = false;
                bool found = true;
                m_parser.BaseStream.LikeVisualStudioEditorSearch = this.FileEditLikeVisualStduioSearch;

                do
                {
                    if (FileEditLikeVisualStduioSearch)
                    {
                        res = m_parser.BaseStream.FindNext(start, expression, searchUp, m_selection.Exists());
                    }
                    else
                    {
                        res = m_parser.BaseStream.FindNext(start, expression, searchUp);
                    }
                    if (res.Result.Success)
                    {
                        start = (searchUp) ? res.StartPoint : res.EndPoint;
                        bPointsVisible = m_parser.IsPointVisible(res.StartPoint) && m_parser.IsPointVisible(res.EndPoint);

                        if (!bSearchInCollapsed && !bPointsVisible)
                            found = false;
                    }
                }
                while (res.Result.Success && !bSearchInCollapsed && !bPointsVisible && !found);

                if (!res.Result.Success)
                {
                    res.StartPoint = null;
                    res.EndPoint = null;
                }

                return res;
            }
        }
        /// <summary>
        /// Marks search result and sets cursor to the end of the selection.
        /// </summary>
        /// <param name="result">Find result that must be marked.</param>
        public void MarkSearchResult(FindResult result)
        {
            MarkSearchResult(result, false);
        }
        /// <summary>
        /// Marks search result and sets cursor to the end of the selection.
        /// </summary>
        /// <param name="result">Find result that must be marked.</param>
        /// <param name="bSearchUp">Indicates whether search process was being performed bottom-up.
        /// Used for proper cursor positioning if empty string was found.</param>
        public void MarkSearchResult(FindResult result, bool bSearchUp)
        {
            lock (this)
            {
                if (result.Result.Success)
                {
                    bool oldShiftBasedSelection = this.AllowShiftSelectionOld;
                    this.AllowShiftSelectionOld = false;
                    StopSelection();
                    SelectionCancel();
                    m_parser.EnsureVisibility(result.StartPoint);
                    m_parser.EnsureVisibility(result.EndPoint);

                    if (result.StartPoint != result.EndPoint)
                    {
                        m_CursorManager.CursorPhysicalCoordinates.Position = result.StartPoint;
                        StartSelection();
                        m_CursorManager.CursorPhysicalCoordinates.Position = result.EndPoint;
                        StopSelection();
                    }
                    else
                    {
                        if (result.StartPoint.Offset == m_parser.BaseStream.Length && !bSearchUp)
                        {
                            MoveToBeginning();
                        }
                        else if (this.CurrentPosition.X == 1 && this.CurrentPosition.Y == 1 && bSearchUp)
                        {
                            MoveToEnd();
                        }
                        else
                        {
                            if (bSearchUp)
                            {
                                MoveLeft();
                            }
                            else
                            {
                                m_CursorManager.CursorPhysicalCoordinates.Position = result.StartPoint;
                                MoveRight();
                            }
                        }
                    }
                    this.AllowShiftSelectionOld = oldShiftBasedSelection;
                }
            }
        }
        /// <summary>
        /// Opens undo group. All further text changes can be undone with open undo operation.
        /// </summary>
        public void UndoGroupOpen()
        {
            lock (this)
            {
                if (m_iUndoGroupOpened == 0) m_iUndoGroupStart = m_parser.UndoQueueLength;
                m_iUndoGroupOpened++;
            }
        }
        /// <summary>
        /// Saves and closes undo group.
        /// </summary>
        public void UndoGroupClose()
        {
            if (m_iUndoGroupOpened == 0) throw new Exception(Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_72);

            lock (this)
            {
                m_iUndoGroupOpened--;
                int groupLength = m_parser.UndoQueueLength - m_iUndoGroupStart;

                if (m_iUndoGroupOpened == 0 && groupLength > 1)
                {
                    UndoGroup group = new UndoGroup(m_iUndoGroupStart, m_parser.UndoQueueLength);
                    m_undoGroups.Push(group);
                }
            }
        }
        /// <summary>
        /// Cancels undo grouping. 
        /// </summary>
        public void UndoGroupCancel()
        {
            if (m_iUndoGroupOpened == 0) throw new Exception(Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_72);

            lock (this)
            {
                m_iUndoGroupOpened--;
            }
        }
        /// <summary>
        /// Collapses all collapsible regions.
        /// </summary>
        public void CollapseAll()
        {
			m_parser.canUpdate = true;
            if (CollapsingAll != null)
            {
                CancelEventArgs args = new CancelEventArgs();
                CollapsingAll(this, args);

                if (args.Cancel) return;
            }

            lock (this)
            {
                using (((ILongOperationController)this).StartOperation("Collapse All"))
                {
                    HideIndentGuideline();
                    StopSelection();
                    SelectionCancel();
                    ShowCollapse = true;

                    m_parser.SetAllCollapsings(true);
                    UpdateScrollerVerticalSize();
                }
            }

            if (CollapsedAll != null)
            {
                CollapsedAll(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Expands all collapsed regions.
        /// </summary>
        public void ExpandAll()
        {
			m_parser.canUpdate = true;
            CancelEventArgs args = new CancelEventArgs();
            if (ExpandingAll != null)
            {
                ExpandingAll(this, args);
            }

            if (!args.Cancel)
            {
                lock (this)
                {
                    using (((ILongOperationController)this).StartOperation("Expand All"))
                    {
                        HideIndentGuideline();
                        StopSelection();
                        SelectionCancel();
                        // Remember old coordinates of cursor.
                        CoordinatePoint oldCursor = GetNearestParsePointLeft(CurrentLine, CurrentColumn);
                        if (oldCursor == null)
                        {
                            oldCursor = GetNearestParsePointLeft(CurrentLine, CurrentColumn - 1);
                        }
                        m_parser.SetAllCollapsings(false);
                        UpdateScrollerVerticalSize();
                        CoordinatePoint newCursor = m_parser.GetCoordinatePoint(oldCursor.PhysicalPoint, true);
                        CurrentPosition = new Point(newCursor.VirtualColumn , newCursor.VirtualLine);
                        InvalidateAll();
                    }
                }

                if (ExpandedAll != null)
                {
                    ExpandedAll(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Collapses all collapsible regions in currently selected area or in the current line.
        /// </summary>
        public void Collapse()
        {
			m_parser.canUpdate = true;
            lock (this)
            {
                using (((ILongOperationController)this).StartOperation("Collapsings toggle"))
                {
                    m_listNotCollapsed.Clear();
                    m_listCollapsed.Clear();
                    RenderedLine line;

                    if (!m_selection.IsEmpty())
                    {
                        int bottomLine = m_selection.Bottom.VirtualLine;

                        for (int i = m_selection.Top.VirtualLine; i < bottomLine; i++)
                        {
                            line = m_parser.GetLine(i) as RenderedLine;
                            FillLineCollapsers(line, m_listCollapsed, m_listNotCollapsed);
                        }
                    }
                    else
                    {
                        line = m_parser.GetLine(CurrentLine) as RenderedLine;
                        FillLineCollapsers(line, m_listCollapsed, m_listNotCollapsed);
                    }

                    if (m_listNotCollapsed.Count == 0)
                    {
                        IParsePoint point = GetNearestParsePointLeft(CurrentLine, CurrentColumn).PhysicalPoint;
                        CollapsableRegion region = m_parser.GetOuterCollapsableRegion(point, false);

                        if (region != null)
                        {
                            m_listNotCollapsed.Add(region);
                        }
                    }

                    if (m_listNotCollapsed.Count > 0)
                    {
                        StopSelection();
                        SelectionCancel();
                        ProcessCollapsing(m_listNotCollapsed, true);
                    }
                }
            }
        }
        /// <summary>
        /// Expands all collapsed regions in currently selected area or in the current line.
        /// </summary>
        public void Expand()
        {
			m_parser.canUpdate = true;
            lock (this)
            {
                using (((ILongOperationController)this).StartOperation("Collapsings expand"))
                {
                    m_listNotCollapsed.Clear();
                    m_listCollapsed.Clear();
                    RenderedLine line;

                    if (!m_selection.IsEmpty())
                    {
                        int bottomLine = m_selection.Bottom.VirtualLine;

                        for (int i = m_selection.Top.VirtualLine; i < bottomLine; i++)
                        {
                            line = m_parser.GetLine(i) as RenderedLine;
                            FillLineCollapsers(line, m_listCollapsed, m_listNotCollapsed);
                        }
                    }
                    else
                    {
                        line = m_parser.GetLine(CurrentLine) as RenderedLine;
                        FillLineCollapsers(line, m_listCollapsed, m_listNotCollapsed);
                    }

                    if (m_listCollapsed.Count > 0)
                    {
                        StopSelection();
                        SelectionCancel();
                        ProcessCollapsing(m_listCollapsed, false);
                    }
                }
            }
        }
        /// <summary>
        /// Converts point in client coordinates to the virtual position in text.
        /// </summary>
        /// <param name="point">Point in client coordinates.</param>
        /// <returns>Virtual position in the text.</returns>
        public Point PointToVirtualPosition(Point point)
        {
            return PointToVirtualPosition(point, true);
        }
        /// <summary>
        /// Converts point in client coordinates to the virtual position in text.
        /// </summary>
        /// <param name="point">Point in client coordinates.</param>
        /// <param name="bUseScrollers">Specifies whether scrollers information should be used.</param>
        /// <returns>Virtual position in the text.</returns>
        public Point PointToVirtualPosition(Point point, bool bUseScrollers)
        {
            lock (this)
            {
                CheckControlState();

                if (bUseScrollers)
                {
                    point.X -= AutoScrollPosition.X;
                    point.Y -= AutoScrollPosition.Y;
                }

                point.X -= TextDrawOffset;

                return m_parser.GraphicalToVirtual(point, VirtualSpaceMode);
            }
        }
        /// <summary>
        /// Converts point in client coordinates to the physical position in text.
        /// </summary>
        /// <param name="point">Point in client coordinates.</param>
        /// <returns>Physical position in the text.</returns>
        public Point PointToPhysicalPosition(Point point)
        {
            lock (this)
            {
                CheckControlState();
                return ConvertVirtualPositionToPhysical(PointToVirtualPosition(point));
            }
        }
        /// <summary>
        /// Converts virtual coordinates to physical.
        /// </summary>
        /// <param name="point">Point in virtual coordinates.</param>
        /// <returns>Point in physical coordinates or (0,0) if given virtual position is not present in the stream.</returns>
        public Point ConvertVirtualPositionToPhysical(Point point)
        {
            lock (this)
            {
                CheckControlState();

                IParsePoint pointPhysical = m_parser.VirtualToPhysical(point);

                if (pointPhysical != null)
                    return new Point(pointPhysical.Position, pointPhysical.Line);
                else
                    return new Point(0, 0);
            }
        }
        /// <summary>
        /// Converts virtual position in text to the offset in stream.
        /// </summary>
        /// <param name="point">Virtual position.</param>
        /// <returns>Offset in the file or stream or -1 if such virtual position is not present in stream.</returns>
        public long ConvertVirtualPositionToOffset(Point point)
        {
            lock (this)
            {
                CheckControlState();
                IParsePoint pointPhysical = m_parser.VirtualToPhysical(point);

                if (pointPhysical != null)
                    return pointPhysical.Offset;
                else
                    return -1;
            }
        }
        /// <summary>
        /// Converts in-stream offset to virtual coordinates.
        /// </summary>
        /// <param name="offset">In-Stream offset.</param>
        /// <returns>Virtual position.</returns>
        public Point ConvertOffsetToVirtualPosition(long offset)
        {
            lock (this)
            {
                CheckControlState();

                if (offset < 0 || offset > m_parser.BaseStream.Length) throw new ArgumentOutOfRangeException("offset", offset,
                       Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_58);

                IParsePoint point = m_parser.BaseStream.GetParsePoint(offset);
                return m_parser.PhysicalToVirtual(point);
            }
        }
        /// <summary>
        /// Prints current page on default printer.
        /// </summary>
        [Command("Printing.PrintCurrentPage")]
        public void PrintCurrentPage()
        {
            BeforePrinting();
            m_bPrintCurrentPage = true;

            try
            {
                PrintDocument.Print();
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message, "");
            }
            finally
            {
                AfterPrinting();
            }
        }
        /// <summary>
        /// Prints selected area on default printer.
        /// </summary>
        [Command("Printing.PrintSelected")]
        public void PrintSelection()
        {
            BeforePrinting();
            PrintDocument.PrinterSettings.PrintRange = PrintRange.Selection;

            try
            {
                PrintDocument.Print();
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message, "");
            }
            finally
            {
                AfterPrinting();
            }
        }
        /// <summary>
        /// Prints entire document on default printer.
        /// </summary>
        [Command("Printing.PrintNoDialog")]
        public void PrintNoDialog()
        {
            BeforePrinting();
            PrintDocument.PrinterSettings.PrintRange = PrintRange.AllPages;

            try
            {
                PrintDocument.Print();
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message, "");
            }
            finally
            {
                AfterPrinting();
            }
        }
        /// <summary>
        /// Shows print dialog and gives user ability to start printing.
        /// </summary>
        [Command("Printing.Print")]
        [KeysBinding(Keys.P | Keys.Control)]
        public void Print()
        {
            BeforePrinting();
            PrintDialog dialogSettings = new PrintDialog();
            dialogSettings.Document = PrintDocument;
            dialogSettings.AllowSelection = true;
            dialogSettings.AllowSomePages = true;
            dialogSettings.UseEXDialog = true;
            PrintDocument.PrinterSettings.PrintRange = PrintRange.AllPages;

            try
            {
                CloseIntellisense();
                if (dialogSettings.ShowDialog() == DialogResult.OK)
                {
                    PrintDocument.Print();
                }
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message, "");
            }
            finally
            {
                AfterPrinting();
            }
        }
        /// <summary>
        /// Shows print preview dialog.
        /// </summary>
        [Command("Printing.PrintPreview")]
        public void PrintPreview()
        {
            BeforePrinting();
            PrintPreviewDialog dialog = new PrintPreviewDialog();
            PrintDocument.PrinterSettings.PrintRange = PrintRange.Selection;
            dialog.Document = PrintDocument;
            dialog.UseAntiAlias = false;
            dialog.MinimizeBox = true;

            try
            {
                CloseIntellisense();
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message, "");
            }
            finally
            {
                AfterPrinting();
            }
        }
        /// <summary>
        /// Prints pages range.
        /// </summary>
        /// <param name="startPageNumber">Start page in range.</param>
        /// <param name="endPageNumber">End page in range.</param>
        public void PrintPages(int startPageNumber, int endPageNumber)
        {
            BeforePrinting();
            PrintDocument.PrinterSettings.PrintRange = PrintRange.SomePages;
            PrintDocument.PrinterSettings.FromPage = startPageNumber;
            PrintDocument.PrinterSettings.ToPage = endPageNumber;

            try
            {
                PrintDocument.Print();
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message, "");
            }
            finally
            {
                AfterPrinting();
            }
        }
        /// <summary>
        /// Saves document's XML representation to the file.
        /// </summary>
        /// <param name="filename">Name of the file, the document should be saved to.</param>
        public void SaveAsXML(string filename)
        {
            m_exporter.SaveAsXML(filename);
        }
        /// <summary>
        /// Saves document's HTML representation to the file.
        /// </summary>
        /// <param name="filename">Name of the file, the document should be saved to.</param>
        /// <param name="bUseLineBreakTags">Indicates whether line break tags should be used.</param>
        public void SaveAsHTML(string filename, bool bUseLineBreakTags)
        {
            m_exporter.SaveAsHTML(filename, bUseLineBreakTags);
        }
        /// <summary>
        /// Saves document's RTF representation to the file.
        /// </summary>
        /// <param name="filename">Name of the file, the document should be saved to.</param>
        public void SaveAsRTF(string filename)
        {
            m_exporter.SaveAsRTF(filename);
        }
        /// <summary>
        /// Resets undo information.
        /// </summary>
        public void ResetUndoInfo()
        {
            if (CanUndo)
            {
                Parser.BaseStream.FlushChanges();
            }
        }
        /// <summary>
        /// Sets underlining of the specified text region.
        /// </summary>
        /// <param name="pointStart">Starting point.</param>
        /// <param name="pointEnd">End point.</param>
        /// <param name="format">Format to be used.</param>
        public void SetUnderline(CoordinatePoint pointStart, CoordinatePoint pointEnd, ISnippetFormat format)
        {
            IDynamicFormatsLayer layer = m_formatManager[DEF_LAYER_NAME_WAVELINE];

            if (!pointStart.AttachToEvents)
            {
                pointStart = new CoordinatePoint(m_parser, pointStart.PhysicalPoint, pointStart.VirtualLine, pointStart.VirtualColumn, true);
            }
            if (!pointEnd.AttachToEvents)
            {
                pointEnd = new CoordinatePoint(m_parser, pointEnd.PhysicalPoint, pointEnd.VirtualLine, pointEnd.VirtualColumn, true);
            }

            layer.Add(pointStart, pointEnd, format);
            InvalidateAll();
        }
        /// <summary>
        /// Removes underlining in the specified region.
        /// </summary>
        /// <param name="pointStart">Starting point.</param>
        /// <param name="pointEnd">End point.</param>
        public void RemoveUnderline(CoordinatePoint pointStart, CoordinatePoint pointEnd)
        {
            IDynamicFormatsLayer layer = m_formatManager[DEF_LAYER_NAME_WAVELINE];

            if (!pointStart.AttachToEvents)
            {
                pointStart = new CoordinatePoint(m_parser, pointStart.PhysicalPoint, pointStart.VirtualLine, pointStart.VirtualColumn, false);
            }
            if (!pointEnd.AttachToEvents)
            {
                pointEnd = new CoordinatePoint(m_parser, pointEnd.PhysicalPoint, pointEnd.VirtualLine, pointEnd.VirtualColumn, false);
            }

            layer.Remove(pointStart, pointEnd);
            InvalidateAll();
        }
        /// <summary>
        /// Register custom underline format, that can be used when setting region's underlining.
        /// </summary>
        /// <param name="color">Color of the underlining.</param>                                                         
        /// <param name="style">Style of the underlining.</param>
        /// <param name="weight">Weight of the underlining.</param>
        /// <returns>Newly created format.</returns>
        public ISnippetFormat RegisterUnderlineFormat(Color color, UnderlineStyle style, UnderlineWeight weight)
        {
            Format format = new Format("CustomUnderlineStyle_" + m_iUnderlineNumber.ToString());
            format.Parent = (FormatManager)m_parser.Formats;
            m_iUnderlineNumber++;

            format.LineColor = color;
            format.UnderlineStyle = style;
            format.UnderlineWeight = weight;

            return format;
        }
        /// <summary>
        /// Registers line backcolor format.
        /// </summary>
        /// <param name="colorBackGround">Line background color.</param>
        /// <param name="colorBorder">Line border color.</param>
        /// <param name="style">Hatch style of the background.</param>
        /// <param name="useHatchFill">Specifies whether hatchstyle value should be used for drawing background.</param>
        /// <returns>Newly created format.</returns>
        public IBackgroundFormat RegisterBackColorFormat(Color colorBackGround, Color colorBorder, HatchStyle style, bool useHatchFill)
        {
            return RegisterBackColorFormat(colorBackGround, colorBorder, colorBorder, style, useHatchFill);
        }
        /// <summary>
        /// Registers line backcolor format.
        /// </summary>
        /// <param name="colorBackGround">Line background color.</param>
        /// <param name="colorForeGround"></param>
        /// <param name="colorBorder">Line border color.</param>
        /// <param name="style">Hatch style of the background.</param>
        /// <param name="useHatchFill">Specifies whether hatchstyle value should be used for drawing background.</param>
        /// <returns>Newly created format.</returns>
        public IBackgroundFormat RegisterBackColorFormat
            (Color colorBackGround, Color colorForeGround, Color colorBorder, HatchStyle style, bool useHatchFill)
        {
            Format format = new Format("CustomBackcolorStyle_" + m_iBackcolorNumber.ToString());
            format.Parent = (FormatManager)m_parser.Formats;
            m_iBackcolorNumber++;

            format.BackColor = colorBackGround;
            format.BorderColor = colorBorder;
            format.ForeColor = colorForeGround;
            format.BorderStyle = FrameBorderStyle.Solid;
            format.HatchStyle = style;
            format.UseHatchFill = useHatchFill;

            return format;
        }
        /// <summary>
        /// Updates line widths in word-wrap mode.
        /// </summary>
        public void RemeasureLinesWrapping()
        {
            if (m_parser != null && this.WordWrap && this.Visible && this.MaxWidth != m_iOldWidth)
            {
                m_iOldWidth = this.MaxWidth;
                UpdateMeasuringInfo();
            }
        }
        /// <summary>
        /// Sets background color of the line.
        /// </summary>
        /// <param name="iLine">Line number.</param>
        /// <param name="bFullLine">Specifies if full line should be selected or just text.</param>
        /// <param name="format">Format with background color.</param>
        public void SetLineBackColor(int iLine, bool bFullLine, IBackgroundFormat format)
        {
            if (format == null) throw new ArgumentNullException("format");

            RenderedLine line = GetLine(iLine) as RenderedLine;

            if (line == null)
                throw new ArgumentOutOfRangeException("iLine", Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_73);

            IList lineLexems = line.LineLexems;
            int iLineFirstChar = 1;
            if (!bFullLine)
            {
                int iIndex = 0;
                while (iIndex < lineLexems.Count)
                {
                    RenderedLexem lexem = (RenderedLexem)lineLexems[iIndex];
                    if (lexem.Config.Type != FormatType.Whitespace)
                    {
                        iLineFirstChar = lexem.Column;
                        break;
                    }

                    iIndex++;
                }
            }

            CoordinatePoint pointStart = (bFullLine) ? line.GetStartPoint() : m_parser.GetCoordinatePoint(iLine, iLineFirstChar, true);
            CoordinatePoint pointEnd = (bFullLine) ? m_parser.GetCoordinatePoint(line.LineEndPoint, false, true) : line.GetEndPoint();
            SetRangeBackcolor(pointStart, pointEnd, format as ISnippetFormat);
        }
        /// <summary>
        /// Sets background format for the selected area.
        /// </summary>
        /// <param name="format">Formatting to be set.</param>
        public void SetSelectionBackColor(IBackgroundFormat format)
        {
            if (format == null) throw new ArgumentNullException("format");

            if (!m_selection.IsEmpty())
            {
                foreach (TextRange range in m_selection.Ranges)
                {
                    CoordinatePoint pointStart = m_parser.GetCoordinatePoint(range.Top.VirtualLine, range.Top.VirtualColumn, true);
                    pointStart.UpdatePhisicalCoordinates();
                    CoordinatePoint pointEnd = m_parser.GetCoordinatePoint(range.Bottom.VirtualLine, range.Bottom.VirtualColumn, true);
                    pointEnd.UpdatePhisicalCoordinates();

                    SetRangeBackcolor(pointStart, pointEnd, format as ISnippetFormat);
                }
            }
        }
        /// <summary>
        /// Removes line back color.
        /// </summary>
        /// <param name="iLine">Line number.</param>
        public void RemoveLineBackColor(int iLine)
        {
            RenderedLine line = GetLine(iLine) as RenderedLine;

            if (line == null)
                throw new ArgumentOutOfRangeException("iLine", Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_73);

            CoordinatePoint pointStart = line.GetStartPoint();
            CoordinatePoint pointEnd = m_parser.GetCoordinatePoint(line.LineEndPoint, false, true);

            IDynamicFormatsLayer back_layer = m_formatManager[DEF_LAYER_NAME_LINEBACKCOLOR_DYNAMIC];
            IDynamicFormatsLayer back_layer1 = m_formatManager[DEF_LAYER_NAME_LINEBACKCOLOR_OVER];
            back_layer.Remove(pointStart, pointEnd);
            back_layer1.Remove(pointStart, pointEnd);
            InvalidateAll();
        }
        /// <summary>
        /// Removes background coloring from the selected text.
        /// </summary>
        public void RemoveSelectionBackColor()
        {
            if (!m_selection.IsEmpty())
            {
                foreach (TextRange range in m_selection.Ranges)
                {
                    IDynamicFormatsLayer back_layer = m_formatManager[DEF_LAYER_NAME_LINEBACKCOLOR_DYNAMIC];
                    back_layer.Remove(range.Top, range.Bottom);

                    back_layer = m_formatManager[DEF_LAYER_NAME_LINEBACKCOLOR_OVER];
                    back_layer.Remove(range.Top, range.Bottom);
                }
            }
        }
        /// <summary>
        /// Gets line backcolor format, used for the specified line.
        /// </summary>
        /// <param name="iLine">Line number.</param>
        /// <returns>Format, used for drawing background of the line.</returns>
        public IDynamicFormat[] GetLineBackColorFormats(int iLine)
        {
            ICollection list = GetLineBackColors(iLine) as ICollection;

            if (list.Count > 0)
            {
                IDynamicFormat[] formats = new IDynamicFormat[list.Count];
                list.CopyTo(formats, 0);
                return formats;
            }

            return null;
        }
        /// <summary>
        /// Gets copy of the parsers stack at the current position.
        /// </summary>
        /// <returns>Parser stack at the position of the cursor.</returns>
        public ConfigStack GetCurrentStack()
        {
            ILexemLine line = CurrentLineInstance;
            return (line == null) ? (null) : (line.GetStackByColumn(CurrentColumn) as ConfigStack);
        }
        /// <summary>
        /// Gets list of the lexems that are inside current stack.
        /// </summary>
        /// <param name="entireRegion">If true, all lexems will be retrieved, otherwise just those, that are before the cursor.</param>
        /// <returns>List of the lexems.</returns>
        public IList GetLexemsInsideCurrentStack(bool entireRegion)
        {
            ConfigStack stack = GetCurrentStack();
            return (stack == null) ? (null) : (GetLexemsInsideStack(stack, entireRegion));
        }
        /// <summary>
        /// Gets list of the lexems that are inside current stack.
        /// </summary>
        /// <param name="stack">Stack we should use.</param>
        /// <param name="entireRegion">If true, all lexems will be retrieved, otherwise just those, that are before the cursor.</param>
        /// <returns>List of the lexems.</returns>
        public IList GetLexemsInsideStack(ConfigStack stack, bool entireRegion)
        {
            if (stack == null) throw new ArgumentNullException("stack");

            IStackData stackItem = stack.Pop();

            IList result = null;

            if (stackItem.Config.IsComplex)
            {
                IEnumerator enumerator = Parser.GetEnumerator(stack, stackItem.Location);
                ILexemEnumeratorParserInfo infoProvider = enumerator as ILexemEnumeratorParserInfo;
                IParsePoint pointCurrent = GetRealCursorPosition();
                int iStackLength = infoProvider.CurrentStack.Count;
                result = new ArrayList();

                // Skip region start.
                if (!enumerator.MoveNext())
                    throw new ApplicationException(Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_74);

                while (enumerator.MoveNext() && (entireRegion || infoProvider.CurrentPosition <= pointCurrent.Offset)
                    && infoProvider.CurrentStack.Count > iStackLength)
                {
                    RenderedLexem lexem = enumerator.Current as RenderedLexem;
                    result.Add(lexem);
                }
            }

            return result;
        }
        /// <summary>
        /// Hides indentation Guideline.
        /// </summary>
        public void HideIndentGuideline()
        {
            if (m_timerAutoIndent != null)
            {
                m_timerAutoIndent.Stop();
                if (m_bIndentAutoShow)
                {
                    m_timerAutoIndent.Start();
                }
            }

            IndentGuideline = null;
        }
        /// <summary>
        /// Sets selection start at the specified position in text.
        /// </summary>
        /// <param name="column">Column index of the selection start.</param>
        /// <param name="line">Line index of the selection start.</param>
        public void StartSelection(int column, int line)
        {
            SelectionCancel();
            StopSelection();
            SetSelectionStart(GetNearestParsePointLeft(line, column));
        }
        /// <summary>
        /// Sets selection end at the specified position in text.
        /// </summary>
        /// <param name="column">Column index of the selection end, should point to the symbol that is next the last selected symbol.</param>
        /// <param name="line">Line index of the selection end, should point to the symbol that is next the last selected symbol.</param>
        public void StopSelection(int column, int line)
        {
            SetSelectionEnd(GetNearestParsePointLeft(line, column));
        }
        /// <summary>
        /// Sets selected area of the text.
        /// </summary>
        /// <param name="columnStart">Column index of the selection start.</param>
        /// <param name="lineStart">Line index of the selection start.</param>
        /// <param name="columnEnd">Column index of the selection end, should point to the symbol that is next the last selected symbol.</param>
        /// <param name="lineEnd">Line index of the selection end, should point to the symbol that is next the last selected symbol.</param>
        public void SetSelection(int columnStart, int lineStart, int columnEnd, int lineEnd)
        {
            StartSelection(columnStart, lineStart);
            StopSelection(columnEnd, lineEnd);
        }
        /// <summary>
        /// Strikes out text.
        /// </summary>
        /// <param name="start">Start location of the text to strike out.</param>
        /// <param name="end">End location of the text to strike out.</param>
        /// <param name="color">Color of the text strike out. If you set it to Color.Empty, strikeout will be removed.</param>
        public void SetTextStrikeOut(CoordinatePoint start, CoordinatePoint end, Color color)
        {
            IDynamicFormatsLayer strikeout_layer = m_formatManager[DEF_LAYER_NAME_STRIKEOUTS];
            ISnippetFormat format = ((FormatManager)Language).GetStrikeOutFormat(color);
            strikeout_layer.Add(start, end, format);
            InvalidateAll();
        }
        /// <summary>
        /// Sets border around text.
        /// </summary>
        /// <param name="start">Start of text to draw border around.</param>
        /// <param name="end">End of text to draw border around.</param>
        /// <param name="color">Color of border.</param>
        /// <param name="style">Style of border.</param>
        /// <param name="weight">Weight of border line.</param>
        public void SetTextBorder(CoordinatePoint start, CoordinatePoint end, Color color, FrameBorderStyle style, BorderWeight weight)
        {
            FormatManager manager = (FormatManager)Language;
            Format format = (Format)manager.GetBorderFormat(style, color, weight);

            //if( style == FrameBorderStyle.Solid && weight == BorderWeight.Thin )
            //{
            //  IDynamicFormatsLayer border_layer = m_formatManager[ DEF_LAYER_NAME_BORDER_OVER ];
            //  border_layer.Add( start, end, format );
            //}
            //else
            {
                IDynamicFormatsLayer border_layer = m_formatManager[DEF_LAYER_NAME_BORDER_DYNAMIC];
                if (this.ExtendSelectionToFarRight)
                {
                    border_layer.Add(start, end, format);
                }
                else
                {
                    ApplyFormatToEachLine(border_layer, start, end, format);
                }
            }

            InvalidateAll();
        }
        /// <summary>
        /// Removes border around text with given coordinates.
        /// </summary>
        /// <param name="start">Start of the text.</param>
        /// <param name="end">End of the text.</param>
        public void RemoveTextBorder(CoordinatePoint start, CoordinatePoint end)
        {
            IDynamicFormatsLayer border_layer = m_formatManager[DEF_LAYER_NAME_BORDER_DYNAMIC];
            border_layer.Remove(start, end);
            border_layer = m_formatManager[DEF_LAYER_NAME_BORDER_OVER];
            border_layer.Remove(start, end);
            InvalidateAll();
        }
        /// <summary>
        /// Sets parameters of border that's drawing in page preview.
        /// </summary>
        /// <param name="style">Style of border.</param>
        /// <param name="color">Color of border.</param>
        /// <param name="weight">Weight of border line.</param>
        public void SetPageBorder(FrameBorderStyle style, Color color, BorderWeight weight)
        {
            m_marginBorderStyle = style;
            m_clrMarginBorder = color;
            m_marginBorderWeight = weight;
        }
        /// <summary>
        /// Closes stream, makes control readonly.
        /// </summary>
        /// <returns>True if user did not cancel the operation, otherwise false.</returns>
        public bool Close()
        {
            bool result = New();

            if (result)
            {
                InvalidateAll();
            }

            return result;
        }
        /// <summary>
        /// Checks whether control has background that requires redraw.
        /// </summary>
        /// <returns>bool indicating whether redraw is needed.</returns>
        public bool IsRedrawingRequiredBackground()
        {
            bool result = false;

            result |= (BackgroundColor.Style != BrushStyle.None && BackgroundColor.Style != BrushStyle.Solid);
            result |= (m_bShowUserMargin && m_brushUserMargin.Style != BrushStyle.None
                && m_brushUserMargin.Style != BrushStyle.Solid);
            result |= (m_bShowTextArea && m_brushAfterTextArea.Style != BrushStyle.None
                && m_brushAfterTextArea.Style != BrushStyle.Solid);
            result |= (m_bShowTextArea && m_textAreaLineStyle != DashStyle.Solid);
            result |= (m_bShowIndentationBlockBorders && m_indentationBlockBackgroundBrush.Style != BrushStyle.None);

            return result;
        }
        /// <summary>
        /// Deletes text at specified position.
        /// </summary>
        /// <param name="start">Start coordinate point of text that has to be deleted.</param>
        /// <param name="end">End coordinate point of text that has to be deleted.</param>
        public void DeleteText(CoordinatePoint start, CoordinatePoint end)
        {
            if (null == start) throw new ArgumentNullException("start");
            if (null == end) throw new ArgumentNullException("end");
            if ((start.VirtualLine < 1) || (start.VirtualColumn < 1) || (start.VirtualLine > Parser.TotalLines))
                throw new ArgumentOutOfRangeException("start");
            if ((end.VirtualLine < 1) || (end.VirtualColumn < 1) || (end.VirtualLine > Parser.TotalLines))
                throw new ArgumentOutOfRangeException("end");

            LockSelection(); // fix for def. OT7403
            TextDeleteInternal(start.VirtualLine, start.VirtualColumn, end.VirtualLine, end.VirtualColumn);
            UnlockSelection();
        }
        /// <summary>
        /// Deletes all text in document.
        /// </summary>
        public void DeleteAll()
        {
            int lastLineLen = m_parser.GetLine(m_parser.TotalLines).LineLength + 1;
            LockSelection(); // fix for def. OT7403
            TextDeleteInternal(1, 1, m_parser.TotalLines, lastLineLen);
            UnlockSelection();
        }
        /// <summary>
        /// Returns text represented as XML.
        /// </summary>
        /// <returns>String with text represented as XML.</returns>
        public string GetTextAsXML()
        {
            return m_exporter.GetXML();
        }
        /// <summary>
        /// Returns text represented as HTML.
        /// </summary>
        /// <returns>String with text represented as HTML.</returns>
        public string GetTextAsHTML()
        {
            return m_exporter.GetHTML();
        }
        /// <summary>
        /// Returns text represented as RTF.
        /// </summary>
        /// <returns>String with text represented as RTF.</returns>
        public string GetTextAsRTF()
        {
            return m_exporter.GetRTF();
        }
        /// <summary>
        /// Returns text situated between specified coordinate points represented as XML.
        /// </summary>
        /// <param name="start">Point representing start of the text.</param>
        /// <param name="end">Point representing end of the text.</param>
        /// <returns>String with desired text represented as XML.</returns>
        public string GetTextAsXML(CoordinatePoint start, CoordinatePoint end)
        {
            return m_exporter.GetXML(start, end);
        }
        /// <summary>
        /// Returns text situated between specified coordinate points represented as HTML.
        /// </summary>
        /// <param name="start">Point representing start of the text.</param>
        /// <param name="end">Point representing end of the text.</param>
        /// <returns>String with desired text represented as HTML.</returns>
        public string GetTextAsHTML(CoordinatePoint start, CoordinatePoint end)
        {
            return m_exporter.GetHTML(start, end);
        }
        /// <summary>
        /// Returns text situated between specified coordinate points represented as RTF.
        /// </summary>
        /// <param name="start">Point representing start of the text.</param>
        /// <param name="end">Point representing end of the text.</param>
        /// <returns>String with desired text represented as RTF.</returns>
        public string GetTextAsRTF(CoordinatePoint start, CoordinatePoint end)
        {
            return m_exporter.GetRTF(start, end);
        }
        /// <summary>
        /// Set color of text.
        /// </summary>
        /// <param name="start">Start of text to set color.</param>
        /// <param name="end">End of text to set color.</param>
        /// <param name="color">Color to set.</param>
        public void SetTextColor(CoordinatePoint start, CoordinatePoint end, Color color)
        {
            if (null == start) throw new ArgumentNullException("start");
            if (null == end) throw new ArgumentNullException("end");
            if (Color.Empty == color) throw new ArgumentOutOfRangeException("color");

            if (start > end)
            {
                CoordinatePoint temp = start;
                start = end;
                end = temp;
            }

            IDynamicFormatsLayer textColor_layer = m_formatManager[DEF_LAYER_NAME_TEXTCOLOR];
            FormatManager manager = (FormatManager)Language;
            Format format = (Format)manager.GetTextColorFormat(color);
            textColor_layer.Add(start, end, format);
            InvalidateAll();
        }
        /// <summary>
        /// Set color of text background.
        /// </summary>
        /// <param name="start">Start of text to set color.</param>
        /// <param name="end">End of text to set color.</param>
        /// <param name="color">Color to set.</param>
        public void SetBackgroundColor(CoordinatePoint start, CoordinatePoint end, Color color)
        {
            backcolor_startpoint = start;
            if (null == start) throw new ArgumentNullException("start");
            if (null == end) throw new ArgumentNullException("end");
            if (Color.Empty == color) throw new ArgumentOutOfRangeException("color");

            if (start > end)
            {
                CoordinatePoint temp = start;
                start = end;
                end = temp;
            }

            FormatManager manager = (FormatManager)Language;
            Format format = (Format)manager.GetBackgroundColorFormat(color);
            SetRangeBackcolor(start, end, format);
        }
        /// <summary>
        /// Set text as readonly.
        /// </summary>
        /// <param name="start">Start of text to set as readonly.</param>
        /// <param name="end">End of text to set as readonly.</param>
        /// <param name="backColor">Color of text background. Empty if no changes needed.</param>
        /// <param name="textColor">Color of text. Empty if no changes needed.</param>
        public void MarkAsReadOnly(CoordinatePoint start, CoordinatePoint end, Color backColor, Color textColor)
        {
            if (null == start) throw new ArgumentNullException("start");
            if (null == end) throw new ArgumentNullException("end");

            if ((start.VirtualLine > end.VirtualLine) || (start.VirtualLine == end.VirtualLine && start.VirtualColumn > end.VirtualColumn))
            {
                CoordinatePoint temp = start;
                start = end;
                end = temp;
            }

            FormatManager manager = (FormatManager)Language;
            Format format = (Format)manager.GetBackgroundAndTextColorFormat(backColor, textColor);
            IDynamicFormatsLayer readOnly_layer = m_formatManager[DEF_LAYER_NAME_READONLY];
            readOnly_layer.Add(start, end, format);
            InvalidateAll();
        }
        /// <summary>
        /// Removes readonly status of specified region.
        /// </summary>
        /// <param name="start">Start of text to remove readonly status.</param>
        /// <param name="end">End of text to remove readonly status.</param>
        public void RemoveReadOnly(CoordinatePoint start, CoordinatePoint end)
        {
            if (null == start) throw new ArgumentNullException("start");
            if (null == end) throw new ArgumentNullException("end");

            if ((start.VirtualLine > end.VirtualLine) || (start.VirtualLine == end.VirtualLine && start.VirtualColumn > end.VirtualColumn))
            {
                CoordinatePoint temp = start;
                start = end;
                end = temp;
            }

            IDynamicFormatsLayer readOnly_layer = m_formatManager[DEF_LAYER_NAME_READONLY];
            readOnly_layer.Remove(start, end);
            InvalidateAll();
        }
        /// <summary>
        /// Indicates whether mouse pointer is situated over selected text.
        /// </summary>
        /// <returns>True if mouse pointer is situated over selected text; otherwise false.</returns>
        public bool MouseOverSelection()
        {
            return PointBelongsToSelection(PointToClient(MousePosition));
        }
        /// <summary>
        /// Sets color of selected text.
        /// </summary>
        /// <param name="color">Color to set.</param>
        public void SetSelectionTextColor(Color color)
        {
            if (!m_selection.IsEmpty())
            {
                foreach (TextRange range in m_selection.Ranges)
                {
                    CoordinatePoint start = new CoordinatePoint(range.Top, true);
                    CoordinatePoint end = new CoordinatePoint(range.Bottom, true);
                    SetTextColor(start, end, color);
                }
            }
        }
        /// <summary>
        /// Sets color of selected text background.
        /// </summary>
        /// <param name="color">Color to set.</param>
        public void SetSelectionBackColor(Color color)
        {
            if (!m_selection.IsEmpty())
            {
                foreach (TextRange range in m_selection.Ranges)
                {
                    CoordinatePoint start = new CoordinatePoint(range.Top, true);
                    CoordinatePoint end = new CoordinatePoint(range.Bottom, true);

                    SetBackgroundColor(start, end, color);
                }
            }
        }
        /// <summary>
        /// Sets border to selected text.
        /// </summary>
        /// <param name="color">Color of border.</param>
        /// <param name="style">Style of border.</param>
        /// <param name="weight">Weight of border line.</param>
        public void SetSelectionBorder(Color color, FrameBorderStyle style, BorderWeight weight)
        {
            if (!m_selection.IsEmpty())
            {
                foreach (TextRange range in m_selection.Ranges)
                {
                    CoordinatePoint start = new CoordinatePoint(range.Top, true);
                    CoordinatePoint end = new CoordinatePoint(range.Bottom, true);
                    SetTextBorder(start, end, color, style, weight);
                }
            }
        }
        /// <summary>
        /// Sets underline to selection.
        /// </summary>
        /// <param name="color">Color of underline.</param>
        /// <param name="style">Style of underline.</param>
        /// <param name="weight">Weight of underline.</param>
        public void SetSelectionUnderline(Color color, FrameBorderStyle style, BorderWeight weight)
        {
            if (!m_selection.IsEmpty())
            {
                foreach (TextRange range in m_selection.Ranges)
                {
                    CoordinatePoint start = new CoordinatePoint(range.Top, true);
                    CoordinatePoint end = new CoordinatePoint(range.Bottom, true);
                    Format format = new Format();
                    format.LineColor = color;
                    format.UnderlineStyle = UnderlineStyle.Solid;
                    format.UnderlineWeight = UnderlineWeight.Thin;
                    SetUnderline(start, end, format);
                }
            }
        }
        /// <summary>
        /// Strikeout selected text.
        /// </summary>
        /// <param name="color">Color of strikeout line.</param>
        public void SetSelectionStrikeout(Color color)
        {
            if (!m_selection.IsEmpty())
            {
                foreach (TextRange range in m_selection.Ranges)
                {
                    CoordinatePoint start = new CoordinatePoint(range.Top, true);
                    CoordinatePoint end = new CoordinatePoint(range.Bottom, true);
                    SetTextStrikeOut(start, end, color);
                }
            }
        }
        /// <summary>
        /// Sets selection to readonly with default color settings.
        /// </summary>
        public void SetSelectionReadOnly()
        {
            if (!m_selection.IsEmpty())
            {
                foreach (TextRange range in m_selection.Ranges)
                {
                    CoordinatePoint start = new CoordinatePoint(range.Top, true);
                    CoordinatePoint end = new CoordinatePoint(range.Bottom, true);
                    MarkAsReadOnly(start, end, Color.White, Color.Gray);
                }
            }
        }
        /// <summary>
        /// Accepts auto complete string and updates context choice list.
        /// </summary>
        public void AcceptAutoComplete()
        {
            // If there's something to accept.
            if (m_controllerContextChoice.AutoCompleteStringShown && !m_selection.IsEmpty())
            {
                // End point of selected auto complete string. Cursor should be set to this point after all.
                CoordinatePoint newPos = this.Selection.End;
                int pos = Selection.End.VirtualColumn;
                SelectionCancel();
                this.CurrentColumn = pos;
                m_controllerContextChoice.AutoCompleteStringShown = false;
                UpdateContextChoice();
            }
        }
        /// <summary>
        /// Declines auto complete string.
        /// </summary>
        public void DeclineAutoComplete()
        {
            // If there's something to decline.
            if (m_controllerContextChoice.AutoCompleteStringShown && !m_selection.IsEmpty())
            {
                // Delete selected auto complete string.
                TextDeleteInternal(
                    Selection.Start.VirtualLine, Selection.Start.VirtualColumn, Selection.End.VirtualLine, Selection.End.VirtualColumn, false);
            }

            m_controllerContextChoice.AutoCompleteStringShown = false;
        }
        /// <summary>
        /// Sets cursor to specified line and ensures visibility.
        /// </summary>
        /// <param name="iLine">Number of desired line.</param>
        /// <returns>Bool indicating success.</returns>
        public bool GoToLine(int iLine)
        {
            bool bResult = false;

            if (iLine >= 1 && iLine <= this.PhysicalLineCount)
            {
                lock (this)
                {
                    IParsePoint point = m_parser.BaseStream.GetParsePoint(iLine, 1, true);
                    m_parser.EnsureVisibility(point);
                    m_CursorManager.CursorPhysicalCoordinates.Position = point;
                    UpdateScrollInfo();
                    bResult = true;
                }
            }

            return bResult;
        }
        /// <summary>
        /// Goes to specifies position in opened file.
        /// </summary>
        /// <param name="iLineNumber">Number of line to set cursor position to.</param>
        /// <param name="iLinesAbove">Number of lines to leave above cursor.</param>
        /// <returns>Bool indicating success.</returns>
        public bool GoTo(int iLineNumber, int iLinesAbove)
        {
            bool bResult = false;

            if (GoToLine(iLineNumber))
            {
                lock (this)
                {
                    int i = CurrentLineInstance.LineIndex - iLinesAbove;
                    if (i >= 1)
                    {
                        AutoScrollPosition = new Point(0, (int)((RenderedLine)m_parser.GetLine(i)).Y);
                        bResult = true;
                    }
                }
            }

            return bResult;
        }
        /// <summary>
        /// Load file and configuration for it.
        /// </summary>
        /// <param name="strFileName">Name of the file to load.</param>
        /// <returns>True if operation succeeds.</returns>
        public virtual bool LoadFile(string strFileName)
        {
            return false;
        }
        /// <summary>
        /// Indents text in the specified range.
        /// </summary>
        /// <param name="p1">Beginning of range.</param>
        /// <param name="p2">End of rage.</param>
        public void IndentText(CoordinatePoint p1, CoordinatePoint p2)
        {
            TextRange range = new TextRange();
            range.Start = p1;
            range.End = p2;
            AddGuidingTabs(range.Top.VirtualLine, range.Bottom.VirtualLine);
        }
        /// <summary>
        /// Outdents text in the specified range.
        /// </summary>
        /// <param name="p1">Beginning of range.</param>
        /// <param name="p2">End of rage.</param>
        public void OutdentText(CoordinatePoint p1, CoordinatePoint p2)
        {
            TextRange range = new TextRange();
            range.Start = p1;
            range.End = p2;
            RemoveGuidingTabs(range.Top.VirtualLine, range.Bottom.VirtualLine);
        }
        /// <summary>
        /// Autoformats given range of text.
        /// </summary>
        /// <param name="iStartLineIndex">Index of first line of range to autoformat.</param>
        /// <param name="iEndLineIndex">Index of last line of range to autoformat.</param>
        public void AutoFormatText(int iStartLineIndex, int iEndLineIndex)
        {
            int iTotalLines = m_parser.TotalLines;

            if (iStartLineIndex < 1 || iStartLineIndex > iTotalLines) throw new ArgumentOutOfRangeException("iFirstLine");
            if (iEndLineIndex < 1 || iEndLineIndex > iTotalLines) throw new ArgumentOutOfRangeException("iLastLine");
            if (iEndLineIndex < iStartLineIndex) throw new ArgumentException("First line index should be less than last line index");

            this.LockUpdate();

            IParsePoint startPoint = m_parser.GetLine(iStartLineIndex).LineStartPoint;
            IParsePoint endPoint = m_parser.GetLine(iEndLineIndex).LineEndPoint;

            string strFormattedText = m_autoFormattingManager.FormatText(startPoint, endPoint);

            CoordinatePoint startCoorPoint = m_parser.GetCoordinatePoint(startPoint);
            CoordinatePoint endCoorPoint = m_parser.GetCoordinatePoint(endPoint);

            DeleteText(startCoorPoint, endCoorPoint);
            InsertText(startCoorPoint.VirtualLine, startCoorPoint.VirtualColumn, strFormattedText);

            this.UnlockUpdate();
        }
        /// <summary>
        /// Shows the cursor caret.
        /// </summary>
        public void ShowCaret()
        {
            CursorManager.Visible = true;
        }
        /// <summary>
        /// Hides the cursor caret.
        /// </summary>
        public void HideCaret()
        {
            CursorManager.Visible = false;
        }
        /// <summary>
        /// Comments text in the specified range.
        /// </summary>
        /// <param name="p1">Beginning of range.</param>
        /// <param name="p2">End of rage.</param>
        public void CommentText(CoordinatePoint p1, CoordinatePoint p2)
        {
            string start = Language.StartComment;
            string end = Language.EndComment;

            p2.AttachToEvents = true;

            if (start != string.Empty)
            {
                UndoGroupOpen();
                LockUpdate();

                try
                {
                    if (string.Empty != end)
                    {
                        TextInsertInternal(p1.VirtualLine, p1.VirtualColumn, start, false);
                        TextInsertInternal(p2.VirtualLine, p2.VirtualColumn, end, false);
                    }
                    else
                    {
                        TextRange range = new TextRange();
                        range.Start = p1;
                        range.End = p2;

                        int top = range.Top.VirtualLine;
                        int bottom = range.Bottom.VirtualLine - ((range.Bottom.VirtualColumn == 1) ? (1) : (0));
                        for (int i = top; i <= bottom; i++)
                        {
                            TextInsertInternal(i, 1, start, false);
                        }
                    }
                }
                finally
                {
                    UndoGroupClose();
                    UnlockUpdate();
                    InvalidateAll();
                }
            }
        }
        /// <summary>
        /// Uncomments text (if possible) in the specified range.
        /// </summary>
        /// <param name="p1">Beginning of range.</param>
        /// <param name="p2">End of rage.</param>
        public void UncommentText(CoordinatePoint p1, CoordinatePoint p2)
        {
            string start = Language.StartComment;
            string end = Language.EndComment;

            TextRange range = new TextRange();
            range.Start = p1;
            range.End = p2;

            int top = range.Top.VirtualLine;
            int bottom = 0;

            try
            {
                UndoGroupOpen();
                LockUpdate();


                if (start != string.Empty && end == string.Empty)
                {

                    bottom = range.Bottom.VirtualLine - ((range.Bottom.VirtualColumn == 1) ? (1) : (0));
                    for (int i = top; i <= bottom; i++)
                    {
                        ILexemLine line = m_parser.GetLine(i);

                        if (line.LineLexems.Count == 0) continue;

                        ILexem textLex = null;
                        foreach (ILexem lex in line.LineLexems)
                        {
                            if (lex.Config.Type != FormatType.Whitespace)
                            {
                                textLex = lex;
                                break;
                            }
                        }

                        if (textLex != null && textLex.Text.StartsWith(start))
                        {
                            TextDeleteInternal(i, 1, i, start.Length + 1);
                        }
                    }
                    SetSelection(1, top, 1, bottom + 1);
                }

                else if (start != string.Empty && end != string.Empty)
                {
                    bottom = range.Bottom.VirtualLine;

                    for (int i = top; i <= bottom; i++)
                    {
                        ILexemLine line = m_parser.GetLine(i);

                        if (line.LineLexems.Count == 0) continue;

                        ILexem textLex = null;
                        bool isEndCleared = false;
                        string str = string.Empty;

                        foreach (ILexem lex in line.LineLexems)
                        {
                            if (lex.Config.Type != FormatType.Whitespace)
                            {
                                if (lex.Config.Type == FormatType.Comment)
                                {
                                    textLex = lex;
                                    str = str + textLex.Text;
                                }
                                if (isEndCleared == true)
                                {
                                    break;
                                }
                            }
                        }

                        this.SetSelection(range.Top.VirtualColumn, range.Top.VirtualLine, range.Bottom.VirtualColumn, range.Bottom.VirtualLine);

                        if (end != null && this.SelectedText.EndsWith(end))
                        {
                            if (end != String.Empty)
                            {
                                TextDeleteInternal(bottom, range.Bottom.VirtualColumn - end.Length, bottom, range.Bottom.VirtualColumn);
                                isEndCleared = true;
                                this.SetSelection(range.Top.VirtualColumn, range.Top.VirtualLine, range.Bottom.VirtualColumn - end.Length, range.Bottom.VirtualLine);
                            }
                        }

                        if (str != null && this.SelectedText.StartsWith(start))
                        {
                            TextDeleteInternal(top, range.Top.VirtualColumn, top, range.Top.VirtualColumn + start.Length);
                        }
                    }

                    this.SetSelection(range.Top.VirtualColumn, range.Top.VirtualLine, range.Bottom.VirtualColumn, range.Bottom.VirtualLine);
                }
            }
            finally
            {
                UndoGroupClose();
                UnlockUpdate();
                InvalidateAll();
            }
        }

        /// <summary>
        /// Comments single line.
        /// </summary>
        /// <param name="iLineIndex">Index of line to comment.</param>
        public void CommentLine(int iLineIndex)
        {
            if (iLineIndex > m_parser.TotalLines) throw new ArgumentOutOfRangeException("iLineIndex");

            CoordinatePoint start = m_parser.GetCoordinatePoint(GetLine(iLineIndex).LineStartPoint, true);
            CoordinatePoint end = m_parser.GetCoordinatePoint(GetLine(iLineIndex).LineEndPoint, true);
            CommentText(start, end);
        }
        /// <summary>
        /// Uncomments single line.
        /// </summary>
        /// <param name="iLineIndex">Index of line to uncomment.</param>
        public void UnCommentLine(int iLineIndex)
        {
            if (iLineIndex > m_parser.TotalLines) throw new ArgumentOutOfRangeException("iLineIndex");

            CoordinatePoint start = m_parser.GetCoordinatePoint(GetLine(iLineIndex).LineStartPoint, true);
            CoordinatePoint end = m_parser.GetCoordinatePoint(GetLine(iLineIndex).LineEndPoint, true);

            UncommentText(start, end);
        }
        /// <summary>
        /// Calculates desired size of the control.
        /// </summary>
        /// <returns>Desired size.</returns>
        public Size GetDesiredSize()
        {
            SizeF result = Size.Empty;

            for (int i = 1, count = Parser.TotalLines; i <= count; i++)
            {
                RenderedLine line = (RenderedLine)Parser.GetLine(i);

                if (!line.IsMeasured)
                {
                    Parser.MeasureLine(line);
                }

                result.Height += line.Height;

                if (result.Width < line.Width)
                {
                    result.Width = line.Width;
                }
            }

            FixLineRenderingPositions();

            result.Width += DEF_PRE_TEXT_AREA;
            return Size.Round(result);
        }
        /// <summary>
        /// Closes context prompt popup.
        /// </summary>
        public void CloseContextPrompt()
        {
            m_contextPrompt.Close();
        }
        /// <summary>
        /// Closes context tooltip.
        /// </summary>
        public void CloseContextTooltip()
        {
            m_tip.Close();
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
        public ITextRange FindRange(string searchString, CoordinatePoint startLocation, CoordinatePoint endLocation,
            bool matchWholeWord, bool searchHiddenText, bool searchUp, bool useRegex)
        {
            if (searchString == null) throw new ArgumentNullException("searchString");
            if (searchString == string.Empty) throw new ArgumentOutOfRangeException("searchString");
            if (startLocation == null) throw new ArgumentNullException("startLocation");

            if (!useRegex) searchString = Regex.Escape(searchString);

            if (matchWholeWord)
            {
                searchString = string.Format(@"(^|\W)(?<{0}>{1})(\W|$)", StreamsWrapper.DEF_SEARCH_DATA_GROUP, searchString);
            }

            FindResult findResult = FindRegex(((searchUp) ? (endLocation.PhysicalPoint) : (startLocation.PhysicalPoint)),
                new Regex(searchString), searchHiddenText, searchUp);

            TextRange result = null;

            if (findResult.Result.Success)
            {
                bool bOK = true;
                if (!searchUp && endLocation != null)
                {
                    if ((ParsePoint)findResult.EndPoint > (ParsePoint)endLocation.PhysicalPoint) bOK = false; ;
                }
                else
                {
                    if ((ParsePoint)findResult.StartPoint < (ParsePoint)startLocation.PhysicalPoint) bOK = false;
                }

                if (bOK)
                {
                    m_parser.EnsureVisibility(findResult.StartPoint, findResult.EndPoint);
                    if (m_parser.IsPointVisible(findResult.StartPoint) && m_parser.IsPointVisible(findResult.EndPoint))
                    {
                        result = new TextRange();
                        result.Start = m_parser.GetCoordinatePoint(findResult.StartPoint);
                        result.End = m_parser.GetCoordinatePoint(findResult.EndPoint);
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Starts new operation.
        /// </summary>
        /// <param name="name">Name of the operation.</param>
        /// <returns>Operation.</returns>
        public ILongOperation StartOperation(string name)
        {
            return new LongOperation(this, name) as ILongOperation;
        }
        /// <summary>
        /// Removes current selection and sets new with start and end in given point.
        /// Later it can be changed using  <seealso cref="SetSelectionEnd"/> method.
        /// </summary>
        /// <remarks>
        /// Note: If you just use <c>SetSelectionStart</c> method, no selection will be visible, but it will be created,
        /// and all commands will work as if selection is set.
        /// </remarks>
        /// <param name="point">ParsePoint with position of selection start.</param>
        public void SetSelectionStart(CoordinatePoint point)
        {
            if (point != null)
            {
                SelectionCancel();
                TextRange range = new TextRange();
                range.Start = point;
                range.End = point;
                m_selection.Ranges.Add(range);
                if (this.IsSelecting)
                {
                    int y = this.CurrentLine;
                    m_selection.VisualStart = GetVisualLocation(y, this.CurrentColumn);
                }
            }
        }
        /// <summary>
        /// Sets end of the selection.
        /// </summary>
        /// <remarks>
        /// Note: Selection must be already present. <para>Old selection will be simply removed.</para>
        /// </remarks>
        /// <param name="point">ParsePoint of end of selection.</param>
        public void SetSelectionEnd(CoordinatePoint point)
        {
            if (!m_selection.IsEmpty() && point != null)
            {
                IDynamicFormatsLayer selection_layer = null;
                if (!m_bTransparentSelection)
                {
                    selection_layer = m_formatManager[DEF_LAYER_NAME_SELECTION];
                    selection_layer.Remove(m_selection.Top, m_selection.Bottom);
                }

                CoordinatePoint oldCoordinate = m_selection.End;
                if (!this.IsBlockSelecting)
                {
                    m_selection.End = point;
                    m_selection.Start.UpdatePhisicalCoordinates();

                    CoordinatePoint top = m_selection.Top;
                    CoordinatePoint bottom = m_selection.Bottom;
                    ISnippetFormat format = m_parser.Formats[FormatType.SelectedText];

                    if (!m_bTransparentSelection && m_selection.End.PhysicalPoint.Offset != m_selection.Start.PhysicalPoint.Offset)
                    {
                        if (m_bExtendSelectionToFarRight)
                        {
                            selection_layer.Add(top, bottom, format);
                        }
                        else
                        {
                            ApplyFormatToEachLine(selection_layer, top, bottom, format);
                        }
                    }
                    //m_selection.VisualEnd = GetVisualLocation( point.VirtualLine, point.VirtualColumn );					
                }
                else
                {
                    int startX = m_selection.VisualStart.Offset;
                    int currentVisualColumn = GetVisualLocation(this.CurrentLine, this.CurrentColumn).Offset;

                    m_selection.Clear();
                    int x1 = Math.Min(currentVisualColumn, startX);
                    int x2 = Math.Max(currentVisualColumn, startX);
                    int y1, y2, subLine1, subLine2;
                    int curSubLine = this.CurrentSubLine;
                    if (this.CurrentLine < m_selection.VisualStart.Line)
                    {
                        y1 = this.CurrentLine;
                        y2 = m_selection.VisualStart.Line;
                        subLine1 = curSubLine;
                        subLine2 = m_selection.VisualStart.SubLine;
                    }
                    else if (this.CurrentLine > m_selection.VisualStart.Line)
                    {
                        y1 = m_selection.VisualStart.Line;
                        y2 = this.CurrentLine;
                        subLine1 = m_selection.VisualStart.SubLine;
                        subLine2 = curSubLine;
                    }
                    else
                    {
                        y1 = y2 = this.CurrentLine;
                        subLine1 = Math.Min(m_selection.VisualStart.SubLine, curSubLine);
                        subLine2 = Math.Max(m_selection.VisualStart.SubLine, curSubLine);
                    }

                    for (int y = y1; y <= y2; y++)
                    {
                        int startSubLine = (y == y1) ? (subLine1) : (0);
                        int endSubLine2 = (y == y2) ? (subLine2) : (((RenderedLine)GetLine(y)).SubLinesCount - 1);
                        for (int endSubLine = startSubLine; endSubLine <= endSubLine2; endSubLine++)
                        {
                            TextRange range = new TextRange();
                            range.Start = GetNearestParsePointLeft(y, GetVirtualColumn(y, endSubLine, x1));
                            range.End = GetNearestParsePointLeft(y, GetVirtualColumn(y, endSubLine, x2));
                            m_selection.Ranges.Add(range);
                            if (!m_bTransparentSelection && range.End.PhysicalPoint.Offset != range.Start.PhysicalPoint.Offset)
                            {
                                selection_layer.Add(range.Top, range.Bottom, m_parser.Formats[FormatType.SelectedText]);
                            }
                        }
                    }

                    m_selection.VisualEnd = new VisualLocation(this.CurrentLine, this.CurrentSubLine, currentVisualColumn);
                }

                if (oldCoordinate.VirtualLine > point.VirtualLine)
                {
                    CoordinatePoint tempPoint = oldCoordinate;
                    oldCoordinate = point;
                    point = tempPoint;
                }

                RenderedLine firstLine = m_parser.GetLine(Math.Max(oldCoordinate.VirtualLine - 1, 1)) as RenderedLine;
                RenderedLine lastLine = m_parser.GetLine(point.VirtualLine) as RenderedLine;

                int yMin = (int)Math.Floor(firstLine.Y);
                Rectangle drawRect = new Rectangle(0, yMin, int.MaxValue, (int)Math.Ceiling(lastLine.Y - firstLine.Y + lastLine.Height + 1));

                InvalidateAll(drawRect);
                OnSelectionChanged();
            }
        }
        /// <summary>
        /// Sets complex selection ranges.
        /// </summary>
        /// <param name="ranges">Collection of ranges.</param>
        public void SetSelectionRanges(IList ranges)
        {
            SelectionCancel();
            m_selection.Ranges.AddRange(ranges);

            if (!m_bTransparentSelection)
            {
                IDynamicFormatsLayer selection_layer = m_formatManager[DEF_LAYER_NAME_SELECTION];
                foreach (TextRange range in m_selection.Ranges)
                {
                    selection_layer.Add(range.Top, range.Bottom, m_parser.Formats[FormatType.SelectedText]);
                }
            }

            InvalidateAll();
            OnSelectionChanged();
        }
        /// <summary>
        /// Registers default commands.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RegisterKeyCommands()
        {
            m_keyBinder.AppendKeyBindings(m_bookmarkhelper, true, false);
            if (RegisteringKeyCommands != null)
            {
                RegisteringKeyCommands(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Registers default commands.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RegisterDefaultKeyBindings()
        {
            m_keyBinder.AppendKeyBindings(m_bookmarkhelper, false, true);
            if (RegisteringDefaultKeyBindings != null)
            {
                RegisteringDefaultKeyBindings(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Adds tabs at the beginning of selected lines.
        /// </summary>
        /// <param name="bForce">Indicates whether tab should be added if single line is selected.</param>
        public void AddTabsToSelection(bool bForce)
        {
            lock (this)
            {
                StopSelection();
                bool bInsertTab = true;
                if (m_selection.Exists())
                {
                    ComplexTextRange selected_ranges = m_selection.Clone() as ComplexTextRange;

                    foreach (TextRange range in selected_ranges.Ranges)
                    {
                        if (range.Top.VirtualLine != range.Bottom.VirtualLine)
                        {
                            int startLine = range.Top.VirtualLine;
                            int endLine = range.Bottom.VirtualLine;
                            if (range.Bottom.VirtualColumn == 1) endLine--;
                            AddGuidingTabs(startLine, endLine);
                            bInsertTab = false;
                        }
                        else if (bForce)
                        {
                            AddGuidingTabs(this.CurrentLine, this.CurrentLine);
                            bInsertTab = false;
                        }
                    }

                    int start = selected_ranges.Top.VirtualLine;
                    int end = selected_ranges.Bottom.VirtualLine;
                    if (selected_ranges.Bottom.VirtualColumn == 1)
                    {
                        end--;
                    }

                    StopSelection();
                    SelectionCancel();
                    CurrentPosition = new Point(1, start);
                    StartSelection();

                    int x, y;
                    if (m_parser.TotalLines != end)
                    {
                        x = 1;
                        y = end + 1;
                    }
                    else
                    {
                        x = m_parser.GetLine(end).LineLength + 1;
                        y = end;
                    }
                    CurrentPosition = new Point(x, y);
                    StopSelection();
                }

                if (bInsertTab)
                {
                    InsertChar(DEF_TAB_CHAR);
                }
            }
        }
        #endregion

        #region Nonpublic Methods
        /// <summary>
        /// Inserts code snippet into text.
        /// </summary>
        protected internal void InsertCodeSnippet()
        {
            // Get string to replace "\n" in snippet members.
            // It will consist of current new line string and whitespace.

            ILexemLine curLine = this.CurrentLineInstance;
            int curCol = this.CurrentColumn;
            int pos = 0;
            int tabLength = m_parser.TabLength;

            foreach (ILexem lex in curLine.LineLexems)
            {
                if (lex.Column >= curCol) break;

                if (lex.Text == DEF_STR_TAB_CHAR)
                {
                    pos += tabLength * lex.Length;
                }
                else
                {
                    pos += lex.Length;
                }
            }

            int tabs = pos / tabLength;
            int spaces = pos % tabLength;
            string whiteSpace = new string(DEF_TAB_CHAR, tabs) + new string(' ', spaces);

            // Build string to insert.

            StringBuilder textToInsertBuilder = new StringBuilder();
            bool bInsertWhitespace = false;

            foreach (CodeSnippetsManager.SnippetMember snippetMember in m_codeSnippetsManager.SnippetMembers)
            {
                if (snippetMember is CodeSnippetsManager.EndSnippetMember)
                {
                    textToInsertBuilder.Append(DEF_END_SNIPPET_MEMBER_MARK);
                }
                else
                {
                    if (bInsertWhitespace)
                    {
                        snippetMember.Text = whiteSpace + snippetMember.Text;
                        bInsertWhitespace = false;
                    }

                    if (snippetMember.Text.EndsWith(STR_NEW_LINE))
                    {
                        snippetMember.Text = snippetMember.Text.Replace(STR_NEW_LINE, m_parser.BaseStream.NewLineStr);
                        bInsertWhitespace = true;
                    }

                    textToInsertBuilder.Append(snippetMember.Text);
                }

                if (!this.UseTabs)
                {
                    snippetMember.Text = snippetMember.Text.Replace(DEF_STR_TAB_CHAR, (m_parser.Formats as FormatManager).TabReplaceString);
                }
            }

            string textToInsert = textToInsertBuilder.ToString();

            bool oldGroupUndo = m_bGroupUndo;
            m_bGroupUndo = false;
            UndoGroupOpen();
            LockUpdate();
            DeleteSelected();
            IParsePoint curParsePoint = this.Parser.GetCoordinatePoint(this.CurrentLine, this.CurrentColumn).PhysicalPoint;
            Point curPhysicalPos = new Point(curParsePoint.Position, curParsePoint.Line);
            Point curPos = this.CurrentPosition;
            TextInsertInternal(this.CurrentLine, this.CurrentColumn, textToInsert, true);
            curParsePoint = this.Parser.GetCoordinatePoint(this.CurrentLine, this.CurrentColumn).PhysicalPoint;
            Point endPhysicalPos = new Point(curParsePoint.Position, curParsePoint.Line);
            m_codeSnippetsManager.StartPoint = m_parser.BaseStream.GetParsePoint(curPhysicalPos.Y, curPhysicalPos.X, true);
            m_codeSnippetsManager.EndPoint = m_parser.BaseStream.GetParsePoint(endPhysicalPos.Y, endPhysicalPos.X, true);
            this.CurrentPosition = curPos;
            UnlockUpdate();

            // Create parse points for code snippets manager.

            IParsePoint searchPoint = m_parser.BaseStream.GetParsePoint(curPhysicalPos.Y, curPhysicalPos.X, true);
            Regex regex;
            string text;
            FindResult findRes;

            int endMemberIndex = -1;

            foreach (CodeSnippetsManager.SnippetMember snippetMember in m_codeSnippetsManager.SnippetMembers)
            {
                bool bEndMark = (snippetMember is CodeSnippetsManager.EndSnippetMember);
                text = (bEndMark) ? (DEF_END_SNIPPET_MEMBER_MARK) : (snippetMember.Text);
                regex = new Regex(Regex.Escape(text));
                findRes = FindRegex(searchPoint, regex, false, false);

                if (snippetMember.IsTemplateMember)
                {
                    if (bEndMark)
                    {
                        m_codeSnippetsManager.CursorEndPoint = findRes.EndPoint;

                        CoordinatePoint start_point = m_parser.GetCoordinatePoint(findRes.StartPoint);
                        CoordinatePoint end_point = m_parser.GetCoordinatePoint(findRes.EndPoint);

                        TextDeleteInternal(start_point.VirtualLine, start_point.VirtualColumn, end_point.VirtualLine, end_point.VirtualColumn, false);

                        endMemberIndex = m_codeSnippetsManager.SnippetMembers.IndexOf(snippetMember);
                    }
                    else
                    {
                        snippetMember.StartPoint = findRes.StartPoint;
                        snippetMember.EndPoint = findRes.EndPoint;
                    }
                }

                searchPoint = findRes.EndPoint;
            }

            if (endMemberIndex != -1)
            {
                m_codeSnippetsManager.SnippetMembers.RemoveAt(endMemberIndex);
            }

            UnlockUpdate();
            UndoGroupClose();
            m_bGroupUndo = oldGroupUndo;
        }
        /// <summary>
        /// Locks control drawing.
        /// </summary>
        protected internal void LockUpdate()
        {
            if (m_updateLocks++ == 0)
            {
                if (PaintLockRequest != null)
                {
                    PaintLockRequest(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Unlocks control drawing.
        /// </summary>
        protected internal void UnlockUpdate()
        {
            if (m_updateLocks > 0)
            {
                if (--m_updateLocks == 0)
                {
                    if (PaintUnlockRequest != null)
                    {
                        PaintUnlockRequest(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Raise Find event
        /// </summary>
        internal void RaiseFindAndReplaceEvent()
        {
            Find(this, EventArgs.Empty);
        }

        /// <summary>
        /// Puts newly-binded fake edit control to the list and disables word wrapping.
        /// </summary>
        /// <param name="control">Fake edit control, binded to this control.</param>
        protected internal void OnFakeControlBinded(FakeEditControl control)
        {
            m_listFakeCopies.Add(control);

            this.WordWrap = false;
            this.SingleLineMode = false;
        }
        /// <summary>
        /// Removes binded fake edit control from the list.
        /// </summary>
        /// <param name="control"></param>
        protected internal void OnFakeControlUnbinded(FakeEditControl control)
        {
            m_listFakeCopies.Remove(control);
        }
        /// <summary>
        /// Changes encoding and new-line style of the content of the stream.
        /// </summary>
        /// <param name="textStream">Input stream.</param>
        /// <param name="newLine">New new-line style.</param>
        /// <param name="encoding">New encoding.</param>
        /// <param name="bDataLost">Indicates whether some data was lst during recoding.</param>
        /// <returns>Newly created stream, or null if no changes where done.</returns>
        protected internal Stream ConvertStream(Stream textStream, string newLine, Encoding encoding, out bool bDataLost)
        {
            if (newLine == null) throw new ArgumentNullException("newLine");
            if (newLine == string.Empty) throw new ArgumentOutOfRangeException("newLine", newLine,
                                                                           Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_10);
            if (encoding == null) throw new ArgumentNullException("encoding");

            return ConvertStreamInternal(textStream, newLine, encoding, out bDataLost);
        }
        /// <summary>
        /// Changes new-line style of the content of the stream.
        /// </summary>
        /// <param name="textStream">Input stream.</param>
        /// <param name="newLine">New new-line style.</param>
        /// <param name="bDataLost">Indicates whether some data was lst during recoding.</param>
        /// <returns>Newly created stream, or null if no changes where done.</returns>
        protected internal Stream ConvertStream(Stream textStream, string newLine, out bool bDataLost)
        {
            if (newLine == null) throw new ArgumentNullException("newLine");

            if (newLine == string.Empty) throw new ArgumentOutOfRangeException("newLine", newLine,
               Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_10);

            return ConvertStreamInternal(textStream, newLine, null, out bDataLost);
        }
        /// <summary>
        /// Changes encoding and new-line style of the content of the stream.
        /// </summary>
        /// <param name="textStream">Input stream.</param>
        /// <param name="encoding">New encoding, can be null to leave the previous one.</param>
        /// <param name="bDataLost">Indicates whether some data was lst during recoding.</param>
        /// <returns>Newly created stream, or null if no changes where done.</returns>
        protected internal Stream ConvertStream(Stream textStream, Encoding encoding, out bool bDataLost)
        {
            if (encoding == null) throw new ArgumentNullException("encoding");

            return ConvertStreamInternal(textStream, string.Empty, encoding, out bDataLost);
        }
        /// <summary>
        /// Sets the same new-line style for the entire stream.
        /// </summary>
        /// <param name="textStream">Input stream.</param>
        /// <param name="bDataLost">Indicates whether some data was lost during recoding.</param>
        /// <returns>Newly created stream, or null if no changes where done.</returns>
        protected internal Stream ConvertStream(Stream textStream, out bool bDataLost)
        {
            return ConvertStreamInternal(textStream, string.Empty, null, out bDataLost);
        }
        /// <summary>
        /// Changes encoding and new-line style of the content of the stream.
        /// </summary>
        /// <param name="textStream">Input stream will be closed after usage.</param>
        /// <param name="newLine">New new-line style can be empty string for autodetection.</param>
        /// <param name="encoding">New encoding can be null to leave the previous one.</param>
        /// <param name="bDataLost">Indicates whether some data was lst during recoding.</param>
        /// <returns>Newly created stream, or null if no changes where done.</returns>
        protected internal Stream ConvertStreamInternal(Stream textStream, string newLine, Encoding encoding, out bool bDataLost)
        {
            if (textStream == null) throw new ArgumentNullException("textStream");

            bDataLost = false;
            textStream.Position = 0;
            Encoding encodingDetected = Encoding.Default;

            using (StreamReader reader = new StreamReader(textStream, Encoding.Default))
            {
                string text = reader.ReadToEnd();
                encodingDetected = reader.CurrentEncoding;

                string strRegex = DEF_CONVERT_REGEX;
                Regex searchRegex = new Regex(strRegex, Syncfusion.Windows.Forms.Edit.Implementation.Config.Config.DEF_COMPILED_REGEX);


                if (newLine == string.Empty)
                {
                    Match match = searchRegex.Match(text);

                    if (match.Success)
                    {
                        newLine = match.Value;
                    }
                }

                string newText = searchRegex.Replace(text, newLine);

                if (newText == text && encoding == null)
                    return null;

                if (null == encoding)
                {
                    encoding = encodingDetected;
                }

                byte[] byteData = encoding.GetBytes(newText);
                string textAfterEncoding = encoding.GetString(byteData);

                if (newText != textAfterEncoding)
                {
                    bDataLost = true;
                }

                byte[] preamble = encoding.GetPreamble();
                bool bPreambleNeeded = (preamble != null && preamble.Length > 0);
                Stream result = new MemoryStream((bPreambleNeeded) ? (preamble.Length) : (0) + byteData.Length);

                if (bPreambleNeeded)
                {
                    result.Write(preamble, 0, preamble.Length);
                }

                result.Write(byteData, 0, byteData.Length);
                result.Position = 0;

                if (null != EncodingChanged)
                {
                    EncodingChanged(this, EventArgs.Empty);
                }

                return result;
            }
        }
        /// <summary>
        /// Updates scrollbars positions to ensure that caret is visible.
        /// </summary>
        protected internal virtual void UpdateScrollInfo()
        {
            //			try
            //			{
            //				Update();
            //			}
            //			catch( Exception e )
            //			{
            //				Debug.WriteLine( e.Message, "Exception" );
            //			}

            //InvalidateAll();
            base.resetCached = this.Language.Cached;
            m_CursorManager.Update();

            UpdateScrollerVerticalSize();
            int iAutoScrollPositionX = this.AutoScrollPosition.X;
            int newY = -this.AutoScrollPosition.Y;
            int newX = -iAutoScrollPositionX;

            Rectangle cursorRect = m_CursorManager.CursorGraphicalCoordinates.Rectangle;
            if (this.RightToLeft == RightToLeft.Yes)
                cursorRect.X = this.TextDrawOffset - cursorRect.X - cursorRect.Width;
            else
                cursorRect.X += this.TextDrawOffset;

            if (!this.DisableScrollers)
            {
                if (cursorRect.Y < -this.AutoScrollPosition.Y)
                {
                    newY = cursorRect.Y;
                }
                if ((cursorRect.Y + cursorRect.Height) > (-this.AutoScrollPosition.Y + this.ClientRectangle.Height))
                {
                    if((this.VScrollMode == ScrollMode.Pixel))
                        newY = cursorRect.Y - this.ClientRectangle.Height + cursorRect.Height;
                    else
                        newY = cursorRect.Y;
                }
            }
            else
            {
                newY = 0;
            }

            if ((cursorRect.X < (-iAutoScrollPositionX + this.TextDrawOffset))
                || (cursorRect.X > (-iAutoScrollPositionX + this.ClientRectangle.Width - OFFSET_TO_SCROLL)))
            {
                // Center scroller on cursor
                newX = cursorRect.X - this.ClientRectangle.Width / 2 + this.ScrollOffsetLeft;
                int textOffset = 15;  //text will start from 15 points later
                if (SingleLineMode && RightToLeft == System.Windows.Forms.RightToLeft.Yes)
                    newX = cursorRect.X - this.ClientRectangle.Width + this.ScrollOffsetLeft + this.MarkerAreaWidth + DEF_COLLAPSE_AREA + textOffset;
                // Normalize cursor position
                newX = Math.Max(newX, 0);

                int maximum = Math.Max(0, this.HScrollBar.Maximum - this.HScrollBar.LargeChange);
                if (maximum < newX && !this.WordWrap)
                {
                    this.HScrollBar.Maximum = newX + this.HScrollBar.LargeChange;
                    UpdateScrollBarsVisibility();
                    UpdateScrollBarsSize();
                }
                else
                {
                    newX = Math.Min(newX, maximum);
                }
            }

            // If there is no scrolling now, update scrollbars.
            if ((newY != -this.AutoScrollPosition.Y || newX != -iAutoScrollPositionX) && (!this.IsAutoScrolling))
            {
                this.AutoScrollPosition = new Point(newX, newY);
            }

            if (this.RightToLeft == RightToLeft.Yes && !this.IsSelecting && !this.IsAutoScrolling
                && (cursorRect.X < (iAutoScrollPositionX + this.ScrollOffsetLeft + OFFSET_TO_SCROLL) || (cursorRect.X > (this.TextDrawOffset - OFFSET_TO_SCROLL))))
            {
                this.ScrollToCaret();
            }
        }
        /// <summary>
        /// Destroys parser.
        /// </summary>
        protected internal void DestroyParser()
        {
            if (m_parser == null)
                throw new NullReferenceException(Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_82);

            if (null != ParserDestroyed)
            {
                ParserDestroyed(this, EventArgs.Empty);
            }

            this.Bookmarks.BookmarkClear(true);
            m_listNewLines.Clear();
            foreach (IDynamicFormatsLayer layer in m_formatManager.Layers)
            {
                layer.DataChanged -= new EventHandler(OnDynamicFormatLayerChanged);
            }

            m_parser.BaseStream.AfterTextChange -= new EventHandler(OnBaseStreamAfterTextChange);
            m_parser.LineInstanceDeleted -= new EventHandler(OnParserLineInstanceDeleted);
            m_parser.LinesCountChanged -= new ValueChangedEventHandler(OnLinesCountChanged);
            m_parser.TextDeleted -= new TextChangedEventHandler(OnTextDeleted);
            m_parser.LineInserted -= new LineInsertedEventHandler(OnLineInserted);
            m_parser.LineDeleted -= new LineDeletedEventHandler(OnLineDeleted);
            m_parser.TextInserted -= new TextChangedEventHandler(OnTextInserted);
            m_parser.TextInserting -= new TextChangingEventHandler(OnTextInserting);
            m_parser.TextDeleting -= new TextChangingEventHandler(OnTextDeleting);
            m_parser.OperationStarted -= new LongOperationEventHandler(ProcessOperationStart);
            m_parser.OperationStopped -= new LongOperationEventHandler(ProcessOperationEnd);
            m_parser.BaseStream.UndoBufferFlushed -= new EventHandler(OnBaseStreamUndoBufferFlush);
            m_parser.BaseStream.RedoBufferFlushed -= new EventHandler(OnBaseStreamRedoBufferFlush);
            m_parser.OutliningStateChanged -= new EventHandler(OnParserOutliningStateChanged);
            m_parser.LineIndexChanged -= new EventHandler(OnParserLineIndexChanged);
            m_parser.Dispose();

            if (null != ParserDestroyed)
            {
                ParserDestroyed(this, EventArgs.Empty);
            }

            m_parser = null;
            if (m_CursorManager != null)
            {
                m_CursorManager.Dispose();
                m_CursorManager = null;
            }
        }
        /// <summary>
        /// Gets lexem under cursor.
        /// </summary>
        /// <returns>IRenderedLexem instance.</returns>
        protected internal IRenderedLexem GetLexemUnderCursor()
        {
            return GetLexemUnderCursor(true);
        }
        /// <summary>
        /// Gets line by y coordinate.
        /// </summary>
        /// <param name="y">Y in control's coordinates.</param>
        /// <returns>Line instance or null.</returns>
        protected internal RenderedLine InternalGetLineByVirtualY(int y)
        {
            return InternalGetLineByAbsoluteY(y - this.AutoScrollPosition.Y);
        }
        /// <summary>
        /// Gets line by y coordinate.
        /// </summary>
        /// <param name="y">Y in virtual coordinates.</param>
        /// <returns>Line instance or null.</returns>
        protected internal RenderedLine InternalGetLineByAbsoluteY(int y)
        {
            CheckControlState();
            RenderedLine line = m_parser.GetLineByY(y);
            if (line != null)
            {
                m_parser.MeasureLine(line);
            }
            return line;
        }
        /// <summary>
        /// Gets line by index.
        /// </summary>
        /// <param name="iLine">Line index.</param>
        /// <param name="correctY">Indicates whether line y position should be corrected.</param>
        /// <returns>Line instance.</returns>
        protected internal RenderedLine InternalGetLine(int iLine, bool correctY)
        {
            CheckControlState();
            RenderedLine line = m_parser.GetLine(iLine) as RenderedLine;
            line.Parsed = true;

            if (correctY)
            {
                m_parser.MeasureLine(line);
            }

            return line;
        }
        /// <summary>
        /// Changes encoding of the underlying stream.
        /// </summary>
        /// <param name="newEncoding">New encoding.</param>
        /// <param name="bForced">Indicates whether encoding is being changed forcibly due to inserting unsupported symbols.</param>
        protected internal virtual void ChangeEncoding(Encoding newEncoding, bool bForced)
        {
            if (newEncoding == null) throw new ArgumentNullException("newEncoding");

            if (bForced)
            {
                m_bEncodingForcedlyChanged = true;
                m_oldEncoding = this.Parser.BaseStream.Encoding;
            }

            IConfigLanguage langCurrent = this.Language;
            MemoryStream stream = new MemoryStream();
            SaveToStream(stream);

            ControlStateStore stateStore = new ControlStateStore();
            stateStore.StoreData(this);

            bool bLost;
            Stream streamConverted = ConvertStream(stream, newEncoding, out bLost);
            if (streamConverted != null)
            {
                Point currentCursorPosition = CurrentPosition;
                DiscardChanges();
                LoadStream(streamConverted, langCurrent);

                if (!bLost && stateStore.CanApply(this))
                {
                    stateStore.RestoreData(this, false);
                }
                CurrentPosition = currentCursorPosition;
            }
        }
        /// <summary>
        /// Updates vertical scroller size.
        /// </summary>
        protected internal void UpdateScrollerVerticalSize()
        {
            UpdateScrollerVerticalSize(this, true);
        }
        /// <summary>
        /// Updates vertical scroller size.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="bUpdateCursor">If true, cursor and it's position will be updated.</param>
        protected internal void UpdateScrollerVerticalSize(IntelliScrollableControl control, bool bUpdateCursor)
        {
            UpdateScrollerOffsets(control);
            CheckControlState();

            float lastRenderedLineY = 0;
            RenderedLine line = m_parser.GetLastRegisteredLine() as RenderedLine;

            if (line != null)
            {
                lastRenderedLineY = line.Y + line.Height;
            }

            int iUnrenderedLinesCount = m_parser.TotalLines - ((line != null) ? line.LineIndex : 0);

            // Limit count of the scrollable lines.
            if (Parser.ParsingMode == TextParsingMode.FullParsing)
            {
                iUnrenderedLinesCount = Math.Min(DEF_MAX_UNRENDERED_LINES, iUnrenderedLinesCount);
            }

            lastRenderedLineY += iUnrenderedLinesCount * m_parser.DefaultLineHeight;
            Size newSize = new Size(control.VirtualSize.Width, (int)Math.Ceiling(lastRenderedLineY));

            if (newSize != control.VirtualSize)
            {
                if (!m_bDisableScrollers)
                {
                    control.VirtualSize = newSize;
                }
                else
                {
                    control.VirtualSize = Size.Empty;
                }
            }

            if (bUpdateCursor)
            {
                m_CursorManager.CursorVirtualCoordinates.Line = Math.Min(m_CursorManager.CursorVirtualCoordinates.Line, m_parser.TotalLines);
                m_CursorManager.Update();
            }
        }
        /// <summary>
        /// Checks whether there is some supported data format in given data object.
        /// </summary>
        /// <param name="data">Object, that keeps data in different formats.</param>
        /// <returns>True if given data object contains data in one of the supported formats, otherwise false.</returns>
        protected internal bool CheckForSupportedData(IDataObject data)
        {
            if (data == null) throw new ArgumentNullException("data");

            bool bUnicodePresent = data.GetDataPresent(DataFormats.UnicodeText, true);
            bool bFilePresent = data.GetDataPresent(DataFormats.FileDrop);
            bool bTextPresent = data.GetDataPresent(DataFormats.Text, true);

            if (bFilePresent)
            {
                string[] fileNames = (string[])data.GetData(DataFormats.FileDrop);
                bTextPresent &= (bFilePresent = (fileNames.Length == 1));
                bUnicodePresent &= bTextPresent;
            }

            return (bUnicodePresent || bFilePresent || bTextPresent);
        }
        /// <summary>
        /// Inserts data from data object if it contains any supported format.
        /// </summary>
        /// <param name="data">Object, that keeps data in different formats.</param>
        protected internal void PasteData(IDataObject data)
        {
            if (data == null) throw new ArgumentNullException("data");
            originalText = this.Text;
            bool bIsSelecting = IsSelecting;
            bool bUnicodePresent = data.GetDataPresent(DataFormats.UnicodeText, true);
            bool bTextPresent = data.GetDataPresent(DataFormats.Text, true);
            bool bFilePresent = data.GetDataPresent(DataFormats.FileDrop);

            string text = string.Empty;
            if (bFilePresent)
            {
                string[] fileNames = (string[])data.GetData(DataFormats.FileDrop);
                if (fileNames.Length == 1)
                {
                    if (m_bInsertDroppedFileIntoText)
                    {
                        FileStream streamDragFile = new FileStream(fileNames[0], FileMode.Open, FileAccess.Read, FileShare.Read);
                        StreamReader reader = new StreamReader(streamDragFile);
                        text = reader.ReadToEnd();
                        reader.Close();
                        streamDragFile.Close();
                    }
                    else
                    {
                        bool bLoad = true;
                        string extension = Path.GetExtension(fileNames[0]);
                        if (!m_DropAllFiles)
                        {
                            bool bFound = false;
                            foreach (string ext in m_arrFileExtensions)
                            {
                                if (string.Equals(ext, extension))
                                {
                                    bFound = true;
                                    break;
                                }
                            }
                            bLoad = bFound;
                        }

                        if (!bLoad)
                        {
                            MessageBox.Show(
                                "File with " + extension + " extension cannot be dropped", "File Drop Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                        else
                        {
                            LoadFile(fileNames[0]);
                        }
                    }
                }
                else
                {
                    throw new NotSupportedException(Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_84);
                }
            }
            else if (bUnicodePresent || bTextPresent)
            {
                text = (string)data.GetData((bUnicodePresent) ? DataFormats.UnicodeText : DataFormats.Text, true);
            }

            if (text != string.Empty && text!=null)
            {
                UndoGroupOpen();
                StopSelection();
                DeleteSelected();
                bool bHashEqual=true ;
                if (EnableMD5)
                {
                    if (_md5 == null)
                        _md5 = MD5.Create();
                byte[] hashCode = _md5.ComputeHash(Encoding.UTF8.GetBytes(text));
                bHashEqual = (m_blockHashcode != null && hashCode.Length == m_blockHashcode.Length);
                if (bHashEqual)
                {
                    for (int i = 0, count = hashCode.Length; i < count; i++)
                    {
                        if (hashCode[i] != m_blockHashcode[i])
                        {
                            bHashEqual = false;
                            break;
                        }
                    }
                }
                }
                text = text.Replace("\r", string.Empty);
                string[] strings = text.Split('\n');
                int linesCount = 0;
                if (strings != null)
                    linesCount = strings.Length;
                int curLine = this.CurrentLine;
                int curCol = this.CurrentColumn;
                VisualLocation curVisualLoc = GetVisualLocation(curLine, curCol);
                int curVisualCol = curVisualLoc.Offset;
                int curSubLine = curVisualLoc.SubLine;
                int curLineSubLinesCount = ((RenderedLine)this.CurrentLineInstance).SubLinesCount;
                if (strings.Length > 1)
                {
                    undoOrRedo = string.Empty;
                    if (linesCount > 1)
                    {
                        if (curCol == 1)
                        {
                            if (text.EndsWith("\n"))
                            {
                                linesCount -= 1;
                            }
                        }
                        else
                        {
                            curLine += 1;
                            linesCount -= 1;
                        }
                    }
                    LinesEventArgs e = new LinesEventArgs(strings, curLine, curCol, linesCount, TextChange.Inserted);
                    OnLineInserted(this, e);
                }
                if (bHashEqual)
                {
                    LockUpdate();

                    TextInsertInternalRespectingTabStops(curLine, curCol, strings[0]);
                    int lastLexemInSublineIndex = 0;

                    for (int i = 1, count = strings.Length; i < count; i++)
                    {
                        if (curSubLine == curLineSubLinesCount - 1)
                        {
                            if (curLine == m_parser.TotalLines)
                            {
                                TextInsertInternal(curLine, this.CurrentLineInstance.LineLength + 1, STR_NEW_LINE);
                            }

                            RenderedLine nextLine = (RenderedLine)m_parser.GetLine(curLine + 1);
                            int nextLineLen = nextLine.LineLength;
                            int nextLineVisualLength = GetVisualLocation(curLine + 1, nextLineLen).Offset;
                            curLineSubLinesCount = nextLine.SubLinesCount;
                            curSubLine = 0;
                            lastLexemInSublineIndex = 0;
                            if (curVisualCol > nextLineVisualLength)
                            {
                                TextInsertInternal(curLine + 1, nextLine.LineLength, new string(' ', curVisualCol - nextLineVisualLength));
                            }
                            this.CurrentPosition = new Point(GetVirtualColumn(curLine + 1, 0, curVisualCol), curLine + 1);
                            curLine = this.CurrentLine;
                            curCol = this.CurrentColumn;
                        }
                        else
                        {
                            IList lexems = this.CurrentLineInstance.LineLexems;
                            int lastLexemInNextSubline;
                            for (lastLexemInNextSubline = lastLexemInSublineIndex; lastLexemInNextSubline < lexems.Count - 1; lastLexemInNextSubline++)
                            {
                                if (((RenderedLexem)lexems[lastLexemInNextSubline + 1]).SubLine == curSubLine + 2)
                                {
                                    break;
                                }
                            }

                            RenderedLexem lex = (RenderedLexem)lexems[lastLexemInNextSubline];
                            // -1 & +1 for proper calculation of visual location at the edge of sublines.
                            int nextLineVisualLength = GetVisualLocation(curLine, lex.Column + lex.Length - 1).Offset + 1;
                            curSubLine++;
                            lastLexemInSublineIndex = lastLexemInNextSubline;
                            if (curVisualCol > nextLineVisualLength)
                            {
                                TextInsertInternal(curLine, lex.Column + lex.Length, new string(' ', curVisualCol - nextLineVisualLength));
                            }
                            this.CurrentPosition = new Point(GetVirtualColumn(curLine, curSubLine, curVisualCol), curLine);
                            curLine = this.CurrentLine;
                            curCol = this.CurrentColumn;
                        }

                        TextInsertInternalRespectingTabStops(curLine, curCol, strings[i]);
                    }

                    UnlockUpdate();
                }
                else
                {
                    int curStartLine = this.CurrentLine;
                    int curStartCol = this.CurrentColumn;

                    TextInsertInternalRespectingTabStops(this.CurrentLine, this.CurrentColumn, text);

                    int curEndLine = this.CurrentLine;
                    int curEndCol = this.CurrentColumn;

                    if (m_bSelectTextAfterDragDrop && m_bDraggingSelectedText)
                    {
                        CoordinatePoint start = m_parser.GetCoordinatePoint(curStartLine, curStartCol);
                        CoordinatePoint end = m_parser.GetCoordinatePoint(curEndLine, curEndCol);

                        SetSelectionStart(start);
                        SetSelectionEnd(end);
                    }
                }

                UndoGroupClose();
            }

            this.IsSelecting = bIsSelecting;
        }
        /// <summary>
        /// Calculates region that is used to display text within specified range.
        /// </summary>
        /// <param name="startPoint">Start of the range.</param>
        /// <param name="endPoint">End of the range.</param>
        /// <returns>Region that is used to display text within specified range.</returns>
        protected internal GraphicsPath GetTextDrawPath(CoordinatePoint startPoint, CoordinatePoint endPoint)
        {
            return GetTextDrawPath(startPoint, endPoint, true);
        }
        /// <summary>
        /// Calculates region that is used to display text within specified range.
        /// </summary>
        /// <param name="startPoint">Start of the range.</param>
        /// <param name="endPoint">End of the range.</param>
        /// <param name="bInfinityRightLimit">Indicates whether right limit should be set to infinity.</param>
        /// <returns>Region that is used to display text within specified range.</returns>
        protected internal GraphicsPath GetTextDrawPath(CoordinatePoint startPoint, CoordinatePoint endPoint, bool bInfinityRightLimit)
        {
            if (startPoint.VirtualLine == endPoint.VirtualLine && startPoint.VirtualColumn == endPoint.VirtualColumn) return null;

            RectangleF rectStart = m_parser.VirtualToGraphical(new Point(startPoint.VirtualColumn, startPoint.VirtualLine));
            RectangleF rectEnd = m_parser.VirtualToGraphical(new Point(endPoint.VirtualColumn, endPoint.VirtualLine));

            if (m_bExtendSelectionToFarRight)
                return GetTextDrawPath(rectStart, rectEnd, bInfinityRightLimit);
            else
                return GetTextDrawPathWithoutRightExtending(rectStart, rectEnd, bInfinityRightLimit);
        }
        /// <summary>
        /// Invalidates client area of control and all attached fake edit controls.
        /// </summary>
        protected internal void InvalidateAll()
        {
            InvalidateAll(new Rectangle(0, 0, int.MaxValue, int.MaxValue));

        }
        /// <summary>
        /// RectangleToRTL method returns result
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        protected internal RectangleF RectangleToRTL(RectangleF rect)
        {
            RectangleF result = RectangleF.Empty;
            result.X = this.TextDrawOffset - rect.X;
            result.Y = result.Y;
            result.Width = rect.Width;
            result.Height = rect.Height;
            return result;
        }

        /// <summary>
        /// Invalidates specified area of control and all attached fake edit controls.
        /// </summary>
        /// <param name="rect">Rectangle to invalidate.</param>
        protected internal void InvalidateAll(Rectangle rect)
        {
            if (InvalidateArea != null)
            {
                InvalidateArea(this, rect);
            }

            rect.X += AutoScrollPosition.X;
            rect.Y += AutoScrollPosition.Y;

            if (rect.Width == int.MaxValue)
            {
                rect.Width -= rect.X + 1;
            }
            if (rect.Height == int.MaxValue)
            {
                rect.Height -= rect.Y + 1;
            }

            rect.Intersect(ClientRectangle);
            if (!rect.IsEmpty)
            {
                m_tracerInvalidation.Invalidate(rect);
            }
        }
        /// <summary>
        /// Checks whether gripper should be drawn.
        /// </summary>
        /// <param name="control">True if gripper should be drawn; otherwise false.</param>
        protected internal bool CheckForGripper(Control control)
        {
            if (control == null) throw new ArgumentNullException("control");

            Form parentForm = control.FindForm();
            if (parentForm == null || !parentForm.IsHandleCreated) return false;
            bool bShowGripper = false;

            if (this.RightToLeft != RightToLeft.Yes)
            {
                Point pointRightBottom = control.PointToScreen(new Point(control.Width, control.Height));
                Point pointFormRightBottom = parentForm.PointToScreen(new Point(parentForm.ClientRectangle.Width, parentForm.ClientRectangle.Height));

                bShowGripper = (pointRightBottom == pointFormRightBottom) && (parentForm.SizeGripStyle != SizeGripStyle.Hide);
            }
            else
            {
                Point pointLeftBottom = control.PointToScreen(new Point(control.Left, control.Height));
                Point pointFormLeftBottom = parentForm.PointToScreen(new Point(parentForm.Left, parentForm.ClientRectangle.Height));

                bShowGripper = (pointLeftBottom == pointFormLeftBottom) && (parentForm.SizeGripStyle != SizeGripStyle.Hide);
            }
            return bShowGripper;
        }
        /// <summary>
        /// Gets rectangle, the line's indicator margin is drawn to.
        /// </summary>
        /// <param name="line">Line to get the indicator margin rectangle for.</param>
        /// <returns>Rectangle for indicator margin or Rectangle.Empty if ShowMerkers is set to false.</returns>
        protected internal Rectangle GetLineIndicatorRectangle(RenderedLine line)
        {
            if (null == line) throw new ArgumentNullException("line");

            if (!ShowMarkers)
            {
                return Rectangle.Empty;
            }

            m_parser.MeasureLine(line);
            Rectangle markRect = new Rectangle(this.MarkerAreaOffset, (int)(line.Y), m_markerAreaWidth, (int)(line.Height));
            return markRect;
        }
        /// <summary>
        /// Processes changes of the DisableScrollers property value.
        /// </summary>
        protected void OnDisableScrollersChanged()
        {
            AutoScrollPosition = Point.Empty;
            VScroll = HScroll = !DisableScrollers;

            if (DisableScrollersChanged != null)
            {
                DisableScrollersChanged(this, EventArgs.Empty);
            }

            InvalidateAll();
        }
        /// <summary>
        /// Updates line rendering positions, updates indentation guideline info.
        /// </summary>
        protected void FixLineRenderingPositions()
        {
            Parser.FixLineRenderingPositions();
            UpdateIndentationGuideLineEnd();
        }
        /// <summary>
        /// This methods is called before any printing operation starts. Control update is locked, word wrapping is turned on if needed.
        /// </summary>
        protected void BeforePrinting()
        {
            LockPaint();
            //LockUpdate();
            CheckControlState();
            m_bPrintCurrentPage = false;

            m_bPrintWordWrapSetting = this.WordWrap;
            m_PrintWordWrapModeSetting = this.WrapMode;

            if (!this.WordWrap)
            {
                ComplexTextRange selection = (ComplexTextRange)m_selection.Clone();
                this.WordWrap = true;
                this.WrapMode = WordWrapMode.Control;
                m_selection = selection;
            }
        }
        /// <summary>
        /// This methods is called after any printing operation ends. Control update is unlocked, word wrapping is turned off if needed.
        /// </summary>
        protected void AfterPrinting()
        {
            WordWrap = m_bPrintWordWrapSetting;
            WrapMode = m_PrintWordWrapModeSetting;
            UnlockPaint();
            //UnlockUpdate();
            InvalidateAll();
        }
        /// <summary>
        /// Locks text selection selection.
        /// </summary>
        protected void LockSelection()
        {
            m_iSelectionLocks++;
        }
        /// <summary>
        /// Unlocks text selection.
        /// </summary>
        protected void UnlockSelection()
        {
            if (m_iSelectionLocks > 0)
            {
                m_iSelectionLocks--;
            }
        }
        /// <summary>
        /// Makes OnPaint to draw a bitmap image of the control.
        /// </summary>
        protected void LockPaint()
        {
            m_controlImage = ActiveXSnapshot.PrintWindow(this);
        }
        /// <summary>
        /// Restores normal work of OnPaint.
        /// </summary>
        protected void UnlockPaint()
        {
            m_controlImage = null;
        }
        /// <summary>
        /// Recalculates space, needed for drawing line index.
        /// </summary>
        /// <param name="lineCount">Line count.</param>
        protected void RecalculateSpaces(int lineCount)
        {
            string number = (m_virtualLineNumberOffset + lineCount).ToString();
            //Fix for 496
				m_lineNumbersWidth = (int)Math.Round(DefaultGraphics.MeasureString(number, this.LineNumbersFont).Width, 0) + number.Length;
            // fixed
            if(this.RightToLeft != RightToLeft.Yes)
                ScrollOffsetLeft = this.TextDrawOffset - 2;
        }
        /// <summary>
        /// Disposes current instance members using the specified type information.
        /// </summary>
        /// <param name="typeToDispose">Type info to be used.</param>
        /// <param name="setNull">Value that indicates whether nullable variable should be set to null.</param>
        protected void DisposeType(Type typeToDispose, bool setNull)
        {
            FieldInfo[] fields = typeToDispose.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                object obj = field.GetValue(this);
                IDisposable disp = obj as IDisposable;

                if (null != disp)
                {
#if DEBUG && VERBOSE
          Debug.WriteLine( field.Name, "Field disposing" );
#endif
                    disp.Dispose();
                }
            }

            if (typeToDispose.BaseType != null)
            {
                DisposeType(typeToDispose.BaseType, setNull);
            }

            if (setNull)
            {
                foreach (FieldInfo field in fields)
                {
                    object obj = field.GetValue(this);
                    if (obj is ValueType) continue;

#if DEBUG && VERBOSE
          Debug.WriteLine( field.Name, "Field set to null" );
#endif
                    field.SetValue(this, null);
                }
            }
        }
        /// <summary>
        /// Redraws rectangle specified in coordinates relative to the text start, when TextDrawOffset is not taken into account.
        /// </summary>
        /// <param name="rect">Rectangle to be redrawn.</param>
        protected void InvalidateRelativeRect(RectangleF rect)
        {
            if (!rect.IsEmpty)
            {
                Rectangle rectToRedraw = new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
                InvalidateRelativeRect(rectToRedraw);
            }
        }
        /// <summary>
        /// Redraws rectangle specified in coordinates relative to the text start, when TextDrawOffset is not taken into account.
        /// </summary>
        /// <param name="rect">Rectangle to be redrawn.</param>
        protected void InvalidateRelativeRect(Rectangle rect)
        {
            if (!rect.IsEmpty)
            {
                rect.X += TextDrawOffset;
                InvalidateAll(rect);
            }
        }
        /// <summary>
        /// Saves current information about selection and top-visible line.
        /// </summary>
        /// <returns>Saved info.</returns>
        protected SavedViewInfo SaveCurrentViewInfo()
        {
            SavedViewInfo info = new SavedViewInfo();
            if (m_selection.IsEmpty() && m_selection.Top != m_selection.Bottom)
            {
                foreach (TextRange range in m_selection.Ranges)
                {
                    SavedViewInfo.TextRangeInfo rangeInfo = new SavedViewInfo.TextRangeInfo();
                    rangeInfo.StartOffset = (range.Start != null) ? (range.Start.PhysicalPoint.Offset) : -1;
                    rangeInfo.EndOffset = (range.End != null) ? (range.End.PhysicalPoint.Offset) : -1;
                    info.SelectionRanges.Add(rangeInfo);
                }

                info.SelectionVisualStart = m_selection.VisualStart;
                info.SelectionVisualEnd = m_selection.VisualEnd;
            }

            RenderedLine topLine = (m_parser != null) ? m_parser.GetLineByY(-AutoScrollPosition.Y) : null;
            info.TopLineStart = (null != topLine) ? topLine.LineStartPoint.Offset : -1;
            info.TopLineOffset = (null != topLine) ? -AutoScrollPosition.Y - topLine.Y : 0;
            info.CursorPosition = (m_parser != null) ? GetNearestParsePointLeft(CurrentLine, CurrentColumn).PhysicalPoint.Offset : -1;
            return info;
        }
        /// <summary>
        /// Restores view info.
        /// </summary>
        /// <param name="info">Structure with saved info.</param>
        protected void RestoreViewInfo(SavedViewInfo info)
        {
            if (-1 != info.CursorPosition)
            {
                IParsePoint point = m_parser.BaseStream.GetParsePoint(info.CursorPosition);
                m_parser.EnsureVisibility(point);
                CoordinatePoint coordinateCursor = m_parser.GetCoordinatePoint(point, true, false);

                CurrentPosition = new Point(coordinateCursor.VirtualColumn, coordinateCursor.VirtualLine);
            }

            SelectionCancel();

            if (info.SelectionRanges.Count > 0)
            {
                IList ranges = new ArrayList();
                foreach (SavedViewInfo.TextRangeInfo range in info.SelectionRanges)
                {
                    TextRange textRange = new TextRange();
                    textRange.Start = m_parser.GetCoordinatePoint(m_parser.BaseStream.GetParsePoint(range.StartOffset));
                    textRange.End = m_parser.GetCoordinatePoint(m_parser.BaseStream.GetParsePoint(range.EndOffset));
                    ranges.Add(textRange);
                }
                SetSelectionRanges(ranges);

                m_selection.VisualStart = info.SelectionVisualStart;
                m_selection.VisualEnd = info.SelectionVisualEnd;
            }

            if (-1 != info.TopLineStart)
            {
                IParsePoint point = m_parser.BaseStream.GetParsePoint(info.TopLineStart);

                m_parser.EnsureVisibility(point);
                CoordinatePoint coordinateTop = m_parser.GetCoordinatePoint(point, true, false);

                if (null != coordinateTop)
                {
                    RenderedLine lineTopNew = m_parser.GetLine(coordinateTop.VirtualLine) as RenderedLine;
                    if (null != lineTopNew)
                    {
                        this.AutoScrollOffset = new Point(0, (int)lineTopNew.Y);
                    }
                }
            }
        }
        /// <summary>
        /// Locks SelectionChanged event raising. Increases lock counter by one.
        /// </summary>
        protected void BeginSelectionUpdate()
        {
            m_iSelectionLockCount++;
        }
        /// <summary>
        /// Unlock SelectionChanged event raising. Decreases lock counter by one.
        /// </summary>
        protected void EndSelectionUpdate()
        {
            if (--m_iSelectionLockCount <= 0)
            {
                OnSelectionChanged();
            }

            if (m_iSelectionLockCount < 0)
            {
                m_iSelectionLockCount = 0;
            }
        }
        /// <summary>
        /// Updates information regarding ending of the indentation guideline.
        /// </summary>
        protected void UpdateIndentationGuideLineEnd()
        {
            UpdateIndentationGuideLineEnd(IndentGuideline);
        }
        /// <summary>
        /// Gets stack for current IndentGuideline region.
        /// </summary>
        /// <returns>Stack or null.</returns>
        protected ConfigStack GetCurrentIndentGuidelinedRegionStack()
        {
            ConfigStack stack = null;
            ILexemLine line = CurrentLineInstance;
            IStackData data = null;

            if (line != null)
            {
                int column = CurrentColumn;
                RenderedLexem lexem = line.FindLexemByColumn(column) as RenderedLexem;
                if (lexem != null)
                {
                    stack = line.GetStackByColumn(lexem.Column + lexem.Length) as ConfigStack;

                    data = stack.Peek();
                    IConfigLexem config = (data.FirstConfig != null) ? (data.FirstConfig) : (data.Config);

                    if (!config.IndentationGuideline || data.Config.IsEqualToEnd(lexem.Text) || data.Lexem == null || lexem.Text != data.Lexem.Text)
                    {
                        stack = null;
                    }
                }

                if (stack == null && column != 1)
                {
                    lexem = line.FindLexemByColumn(column - 1) as RenderedLexem;
                    if (lexem != null)
                    {
                        stack = line.GetStackByColumn(lexem.Column) as ConfigStack;

                        data = stack.Peek();
                        IConfigLexem config = (data.FirstConfig != null) ? (data.FirstConfig) : (data.Config);

                        if (!config.IndentationGuideline || data.Config.IsEqualToBegin(lexem.Text) || data.Lexem == null)
                        {
                            stack = null;
                        }
                    }
                }

                if (stack == null)
                {
                    stack = line.GetStackByColumn(column) as ConfigStack;
                }
            }

            if (stack == null) throw new ApplicationException(Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_76);

            while (stack.Count > 0)
            {
                data = stack.Peek();
                IConfigLexem config = (data.FirstConfig != null) ? (data.FirstConfig) : (data.Config);

                if (config.IndentationGuideline)
                {
                    break;
                }

                stack.Pop();
            }

            if (stack.Count == 0)
            {
                stack = null;
            }

            return stack;
        }
        /// <summary>
        /// Adds background formatting for the range and invalidates control.
        /// </summary>
        /// <param name="start">Start point of the range.</param>
        /// <param name="end">End point of the range.</param>
        /// <param name="format">Format of the range.</param>
        protected void SetRangeBackcolor(CoordinatePoint start, CoordinatePoint end, ISnippetFormat format)
        {
            if (format == null) throw new ArgumentNullException("format");
            if (start == null) throw new ArgumentNullException("start");
            if (end == null) throw new ArgumentNullException("end");

            bool bHasBackground = (format.BackColor != Color.Empty);
            bool bHasBorder = (format.BorderColor != Color.Empty && format.BorderStyle != FrameBorderStyle.None);
            bool bBorderThin = (bHasBorder && format.BorderStyle == FrameBorderStyle.Solid && format.BorderWeight == BorderWeight.Thin);

            if (bBorderThin && this.ExtendSelectionToFarRight)
            {
                IDynamicFormatsLayer back_layer = m_formatManager[DEF_LAYER_NAME_LINEBACKCOLOR_OVER];
                back_layer.Add(start, end, format);
            }

            if (bHasBackground)
            {
                if (bBorderThin && this.ExtendSelectionToFarRight)
                {
                    format = (ISnippetFormat)RegisterBackColorFormat(
                      format.BackColor, format.ForeColor, Color.Empty, format.HatchStyle, format.UseHatchFill);
                }

                IDynamicFormatsLayer back_layer = m_formatManager[DEF_LAYER_NAME_LINEBACKCOLOR_DYNAMIC];
                if (!this.ExtendSelectionToFarRight)
                {
                    ApplyFormatToEachLine(back_layer, start, end, format);
                }
                else
                {
                    back_layer.Add(start, end, format);
                }
            }

            InvalidateAll();
        }
        /// <summary>
        /// Gets dynamic formatting range for the line.
        /// </summary>
        /// <param name="iLine">Line number.</param>
        /// <returns>Dynamic formatting range.</returns>
        protected IList GetLineBackColors(int iLine)
        {
            RenderedLine line = GetLine(iLine) as RenderedLine;

            if (line == null)
                throw new ArgumentOutOfRangeException("iLine", Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_73);

            CoordinatePoint pointStart = line.GetStartPoint();
            CoordinatePoint pointEnd = m_parser.GetCoordinatePoint(line.LineEndPoint, false, true);

            IDynamicFormatsLayer back_layer = m_formatManager[DEF_LAYER_NAME_LINEBACKCOLOR_DYNAMIC];
            ArrayList list = (ArrayList)back_layer[pointStart, pointEnd];
            back_layer = m_formatManager[DEF_LAYER_NAME_LINEBACKCOLOR_OVER];
            list.AddRange(back_layer[pointStart, pointEnd]);

            if (list == null)
            {
                list = new ArrayList();
            }

            return list;
        }
        /// <summary>
        /// Raises SingleLineChanged event and updates form size if needed.
        /// </summary>
        protected virtual void OnSingeLineChanged()
        {
            if (SingleLineChanged != null) SingleLineChanged(this, EventArgs.Empty);

            LockUpdate();
            VScroll = HScroll = !DisableScrollers;
            UndoGroupOpen();

            if (SingleLineMode)
            {
                m_listNewLines.Clear();

                for (int i = Parser.TotalLines; i > 1; i--)
                {
                    RenderedLine line = GetLine(i) as RenderedLine;
                    m_listNewLines.Add(line.LineStartPoint);
                    TextDeleteInternal(i, 0, i, 1, false);
                }
            }
            else
            {
                for (int i = 0; i < m_listNewLines.Count; i++)
                {
                    IParsePoint point = m_listNewLines[i] as IParsePoint;
                    if (point.IsValid)
                    {
                        CoordinatePoint pointNewLine = Parser.GetCoordinatePoint(point, true, false);
                        TextInsertInternal(pointNewLine.VirtualLine, pointNewLine.VirtualColumn, Parser.BaseStream.NewLineStr, false);
                        pointNewLine.Dispose();
                    }
                }
            }

            CurrentPosition = new Point(1, 1);
            UndoGroupClose();
            UnlockUpdate();

            if (!DisableScrollers)
            {
                OnDisableScrollersChanged();
            }
            else
            {
                InvalidateAll();
            }
        }
        /// <summary>
        /// Checks whether control is in workable state, if not, throws an exception.
        /// </summary>
        protected void CheckControlState()
        {
            if (m_parser == null)
                throw new NullReferenceException(Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_77);
            if (m_wrapper == null)
                throw new NullReferenceException(Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_78);
            if (m_Configuration == null)
                throw new NullReferenceException(Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_79);
            if (m_CursorManager == null)
                throw new NullReferenceException(Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_80);
        }
        /// <summary>
        /// Expands all collapsing on the current line.
        /// </summary>
        /// <returns>True if user didn't cancel expanding; otherwise false.</returns>
        protected bool ExpandCurrentLine()
        {
            do
            {
                m_listNotCollapsed.Clear();
                m_listCollapsed.Clear();

                FillLineCollapsers(CurrentLineInstanceInternal, m_listCollapsed, m_listNotCollapsed);

                if (m_listCollapsed.Count > 0 && !ProcessCollapsing(m_listCollapsed, false))
                {
                    return false;
                }
            }
            while (m_listCollapsed.Count > 0);

            return true;
        }
        /// <summary>
        /// Gets word under cursor.
        /// </summary>
        /// <param name="iWordColumn">OUT start column of the word.</param>
        /// <returns>Lexem under cursor.</returns>
        protected string GetCurrentWord(out int iWordColumn)
        {
            IRenderedLexem lexem = GetLexemUnderCursor();
            iWordColumn = 0;

            if (lexem == null) return string.Empty;

            iWordColumn = lexem.Column;
            return lexem.Text;
        }
        /// <summary>
        /// Inserts text and sets cursor's position after the text.
        /// </summary>
        /// <param name="iLine">Line where text must be inserted.</param>
        /// <param name="iColumn">Column where text must be inserted.</param>
        /// <param name="str">Text to be inserted.</param>
        protected void TextInsertInternalRespectingTabStops(int iLine, int iColumn, string str)
        {
            if (this.RespectTabStopsOnInsertingText)
            {
                ArrayList strings = new ArrayList();
                int tabpos = str.IndexOf('\t');
                while (tabpos != -1)
                {
                    if (tabpos > 0)
                    {
                        strings.Add(str.Substring(0, tabpos));
                    }
                    strings.Add("\t");
                    str = str.Remove(0, tabpos + 1);
                    tabpos = str.IndexOf('\t');
                }

                if (str != string.Empty)
                {
                    strings.Add(str);
                }

                LockUpdate();

                for (int i = 0, row = iLine, col = iColumn; i < strings.Count; i++)
                {
                    TextInsertInternal(row, col, (string)strings[i], true, true);

                    row = this.CurrentLine;
                    col = this.CurrentColumn;
                }

                UnlockUpdate();
            }
            else
            {
                TextInsertInternal(iLine, iColumn, str, true, true);
            }
        }
        /// <summary>
        /// Inserts text and sets cursor's position after the text.
        /// </summary>
        /// <param name="iLine">Line where text must be inserted.</param>
        /// <param name="iColumn">Column where text must be inserted.</param>
        /// <param name="str">Text to be inserted.</param>
        protected void TextInsertInternal(int iLine, int iColumn, string str)
        {
            TextInsertInternal(iLine, iColumn, str, true, true);
        }
        /// <summary>
        /// Inserts text and set`s cursor`s position after the text.
        /// </summary>
        /// <param name="iLine">Line where text must be inserted.</param>
        /// <param name="iColumn">Column where text must be inserted.</param>
        /// <param name="str">Text to be inserted.</param>
        /// <param name="update">Specifies whether cursor and graphics should be updated.</param>
        protected internal void TextInsertInternal(int iLine, int iColumn, string str, bool update)
        {
            TextInsertInternal(iLine, iColumn, str, update, true);
        }
        /// <summary>
        /// Defines the readOnly_layer
        /// </summary>
        public IDynamicFormatsLayer readOnly_layer;
        /// <summary>
        /// Inserts text and set`s cursor`s position after the text.
        /// </summary>
        /// <param name="iLine">Line where text must be inserted.</param>
        /// <param name="iColumn">Column where text must be inserted.</param>
        /// <param name="str">Text to be inserted.</param>
        /// <param name="update">Specifies whether cursor and graphics should be updated.</param>
        /// <param name="bUseTabStops">Specifies whether tab stops should be used.</param>
        protected void TextInsertInternal(int iLine, int iColumn, string str, bool update, bool bUseTabStops)
        {
            if (str != null)
            {
                if (SingleLineMode)
                {
                    str = str.Replace(STR_NEW_LINE, "").Replace("\r", "");
                }

                if (str != string.Empty && !ReadOnly)
                {
                    if (m_codeSnippetsManager.Activated)
                    {
                        m_codeSnippetsManager.DeleteSnippetMemberText();
                    }

                    // If we don't use any kind of encoding, that supports unicode.
                    // If we are inserting some unicode symbols.
                    // If text can not be saved using current encoding.
                    if ( /*m_wrapper.Encoding.GetMaxByteCount( 1 ) == 1 && */Encoding.UTF8.GetByteCount(str) > str.Length &&
                        m_wrapper.Encoding.GetString(m_wrapper.Encoding.GetBytes(str)) != str)
                    {
                        readOnly_layer = m_formatManager[DEF_LAYER_NAME_READONLY];
                        CoordinatePoint point1;
                        point1 = m_parser.GetCoordinatePoint(iLine, iColumn);
                        if (!readOnly_layer.PointInLayer(point1, !m_bInsertingBeforeReadonlyNewLine, false))
                        ChangeEncoding(Encoding.Unicode, true);
                    }

                    
                    CoordinatePoint point;
                    CoordinatePoint tempPoint=null;
                    if (this.FromIndentClick)
                    {
                        tempPoint = m_parser.GetCoordinatePoint(iLine, iColumn);
                        point = m_parser.GetCoordinatePoint(iLine, 1);
                    }
                    else
                    {
                        point = m_parser.GetCoordinatePoint(iLine, iColumn);
                    }

                    // Maybe it is in virtual space
                    if (point == null)
                    {
                        int endLineColumn = m_parser.GetLine(iLine).LineLength + 1;
                        point = m_parser.GetCoordinatePoint(iLine, endLineColumn);
                        // Extend string.
                        str = new string(' ', iColumn - endLineColumn) + str;

                        if (point == null) throw new Exception(Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_81);
                    }

                    point.UpdatePhisicalCoordinates();
                    Point curCursorPos = this.CurrentPosition;
                    bool bCancel = false;
                    if (!ExpandCurrentLine())
                    {
                        // User has cancelled expanding.
                        this.CurrentPosition = curCursorPos;
                        if (CursorOverCollapsedRegion())
                        {
                            bCancel = true;
                        }
                    }

                    if (!bCancel)
                    {
                        point.UpdateVirtualCoordinates();
                        IDynamicFormatsLayer readOnlyLayer = m_formatManager[DEF_LAYER_NAME_READONLY];

                        if (m_bInsertingBeforeReadonlyNewLine)
                        {
                            m_bInsertingBeforeReadonlyNewLine = m_lastOperation.Insert &&
                                (this.CurrentLine == m_lastOperation.EndLine &&
                                this.CurrentColumn == m_lastOperation.EndColumn);
                        }
                        else
                        {
                            m_bInsertingBeforeReadonlyNewLine = (m_bAllowInsertBeforeReadonlyNewLine && iColumn == 1);
                        }

                        if (!readOnlyLayer.PointInLayer(point, !m_bInsertingBeforeReadonlyNewLine, false))
                        {
                            LockSelection();

                            FormatManager fm = (m_parser.Formats as FormatManager);

                            // If tab stops should be used.
                            if (DEF_STR_TAB_CHAR == str && m_bUseTabStops && bUseTabStops)
                            {
                                // Indicates whether something should be changed.
                                bool bMakeChanges = false;
                                // Number of chars in previous sublines. Used for tab stops in "multisublined" lines.
                                int prevSublinesCharsCount = 0;

                                // Get real column taking into account tabs length.

                                RenderedLine curLine = (RenderedLine)m_parser.GetLine(iLine);
                                // Real column.
                                int curCol = iColumn;
                                RenderedLexem curLex = (RenderedLexem)curLine.FindLexemByColumn(iColumn);
                                int curSubline = (null == curLex) ? (curLine.SubLineHeight.Length - 1) : (curLex.SubLine);

                                for (int k = 0, count = curLine.LineLexems.Count; k < count; k++)
                                {
                                    RenderedLexem lex = (RenderedLexem)curLine.LineLexems[k];

                                    if (lex.Column > iColumn)
                                    {
                                        break;
                                    }

                                    // Look at lexems from current subline only.
                                    if (curSubline != lex.SubLine)
                                    {
                                        prevSublinesCharsCount += lex.Length;
                                        continue;
                                    }

                                    for (int j = 0; j < lex.Text.Length; j++)
                                    {
                                        if (lex.Column + j >= iColumn)
                                        {
                                            break;
                                        }

                                        // correct real column.
                                        if (DEF_TAB_CHAR == lex.Text[j])
                                        {
                                            curCol += fm.SpacesInTab - 1;
                                        }
                                    }
                                }

                                // Index of tab stop that has to be used.
                                int nextTabStop = 0;
                                for (int len = m_arrTabStops.Length; nextTabStop < len; nextTabStop++)
                                {
                                    if (m_arrTabStops[nextTabStop] + prevSublinesCharsCount > curCol)
                                    {
                                        bMakeChanges = true;
                                        break;
                                    }
                                }

                                // If something must be changed.
                                if (bMakeChanges)
                                {
                                    // Length of free space that must be filled.
                                    int length = m_arrTabStops[nextTabStop] + prevSublinesCharsCount - curCol;
                                    int tabLength = fm.SpacesInTab;
                                    if (tabLength > length)
                                    {
                                        // There's no enough space for tab.
                                        str = new string(' ', length);
                                    }
                                    else
                                    {
                                        // More space should be inserted.
                                        int tabs = length / tabLength;
                                        int spaces = length % tabLength;
                                        str = new string(DEF_TAB_CHAR, tabs) + new string(' ', spaces);
                                    }
                                }
                            }

                            if (!m_bUseTabs)
                            {
                                str = str.Replace(DEF_STR_TAB_CHAR, fm.TabReplaceString);
                            }

                            RenderedLine lineTemp = (update) ? (m_parser.GetLine(iLine) as RenderedLine) : (null);
                            int oldSublinesCount = (update) ? (lineTemp.SubLinesCount) : (0);

                            int iUndoQueueLenBeforeInsert = m_parser.UndoQueueLength;
                            m_bModified = true;
                            m_parser.InsertText(str, point);
                            CoordinatePoint newPoint;
                            if (this.FromIndentClick)
                            {
                                newPoint = m_parser.GetCoordinatePoint(tempPoint.PhysicalPoint, true);
                            }
                            else
                            {
                                newPoint = m_parser.GetCoordinatePoint(point.PhysicalPoint, true);
                            }
                            if (m_bGroupUndo && m_parser.UndoQueueLength != iUndoQueueLenBeforeInsert)
                            {
                                bool bNeedMarker = !m_lastOperation.Insert;
                                if (!bNeedMarker)
                                {
                                    // If last operation was insert, check it.
                                    bNeedMarker = !(str == " " && m_lastOperation.InsertText == " ") && !(str != " " && m_lastOperation.InsertText != " ");
                                    if (!bNeedMarker)
                                    {
                                        bNeedMarker = !(point.VirtualLine == m_lastOperation.EndLine && point.VirtualColumn == m_lastOperation.EndColumn)
                                            || m_lastOperation.EndLine != m_lastOperation.StartLine;
                                    }
                                }
                                if (bNeedMarker)
                                {
                                    m_UndoMarkers.Push(m_parser.UndoQueueLength - 1);
                                }
                            }

                            if (update)
                            {
                                RenderedLine startLine = m_parser.GetLine(iLine) as RenderedLine;
                                RenderedLine endLine =
                                    (iLine != newPoint.VirtualLine) ? (m_parser.GetLine(newPoint.VirtualLine) as RenderedLine) : (startLine);

                                m_parser.MeasureLine(endLine); // Fix for def. OT6760.

                                if (startLine.SubLinesCount != oldSublinesCount)
                                {
                                    InvalidateAll();
                                }
                                else
                                {
                                    InvalidateAll(new Rectangle(0, (int)(startLine.Y - BORDER_INVALIDATION_PIXELS),
                                        int.MaxValue, (int)Math.Ceiling(endLine.Height + endLine.Y - startLine.Y + (2 * BORDER_INVALIDATION_PIXELS) + 1)));
                                }
                                CurrentPosition = new Point(newPoint.VirtualColumn, newPoint.VirtualLine);
                            }

                            m_lastOperation.Insert = true;
                            m_lastOperation.InsertText = str;
                            m_lastOperation.StartColumn = point.VirtualColumn;
                            m_lastOperation.StartLine = point.VirtualLine;
                            m_lastOperation.EndColumn = newPoint.VirtualColumn;
                            m_lastOperation.EndLine = newPoint.VirtualLine;

                            RaiseUpdateChangedStateEvent();
                            UnlockSelection();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Deletes text in specified range.
        /// </summary>
        /// <param name="iLineStart">Start line of text to be deleted.</param>
        /// <param name="iColumnStart">Start column of text to be deleted.</param>
        /// <param name="iLineEnd">End line of text to be deleted.</param>
        /// <param name="iColumnEnd">End column of text to be deleted.</param>
        protected void TextDeleteInternal(int iLineStart, int iColumnStart, int iLineEnd, int iColumnEnd)
        {
            originalText = this.Text;
            TextDeleteInternal(iLineStart, iColumnStart, iLineEnd, iColumnEnd, true);
        }
        /// <summary>
        /// Deletes text in specified range.
        /// </summary>
        /// <param name="iLineStart">Start line of text to be deleted.</param>
        /// <param name="iColumnStart">Start column of text to be deleted.</param>
        /// <param name="iLineEnd">End line of text to be deleted.</param>
        /// <param name="iColumnEnd">End column of text to be deleted.</param>
        /// <param name="update">Specifies whether cursor and graphics should be updated.</param>
        protected internal void TextDeleteInternal(int iLineStart, int iColumnStart, int iLineEnd, int iColumnEnd, bool update)
        {
            if (!ReadOnly)
            {
                CoordinatePoint start = m_parser.GetCoordinatePoint(iLineStart, iColumnStart);
                CoordinatePoint end = m_parser.GetCoordinatePoint(iLineEnd, iColumnEnd);
                IDynamicFormatsLayer readOnlyLayer = m_formatManager[DEF_LAYER_NAME_READONLY];

                if (!CanEditSelectedText())
                    return;

                if (!readOnlyLayer.PointInLayer(start, false, false) && !readOnlyLayer.PointInLayer(end, false, false)||AllowDeleteReadOnlyRegion )
                {
                    StopSelection();
                    SelectionCancel();

                    if (iLineStart != iLineEnd || iColumnStart != iColumnEnd)
                    {
                        CoordinatePoint pointStart = GetNearestParsePointLeft(iLineStart, iColumnStart);
                        CoordinatePoint pointEnd = GetNearestParsePointRight(iLineEnd, iColumnEnd);

                        if (pointStart < pointEnd)
                        {
                            m_bModified = true;
                            m_parser.DeleteText(pointStart, pointEnd);

                            if (m_bGroupUndo)
                            {
                                bool bNeedMarker = m_lastOperation.Insert;
                                if (!bNeedMarker)
                                {
                                    bNeedMarker = !(pointStart.VirtualLine == m_lastOperation.StartLine && pointStart.VirtualColumn == m_lastOperation.StartColumn)
                                        && !(pointEnd.VirtualLine == m_lastOperation.StartLine && pointEnd.VirtualColumn == m_lastOperation.StartColumn);
                                }
                                if (bNeedMarker)
                                {
                                    m_UndoMarkers.Push(m_parser.UndoQueueLength - 1);
                                }
                            }

                            if (update)
                            {
                                if (pointStart.VirtualLine != pointEnd.VirtualLine)
                                {
                                    RenderedLine startLine = m_parser.GetLine(pointStart.VirtualLine) as RenderedLine;
                                    int y = (int)startLine.Y;
                                    InvalidateAll(new Rectangle(0, y, int.MaxValue, int.MaxValue));
                                }
                                else
                                {
                                    // Fix for defect OT#5717.

                                    RenderedLine startLine = m_parser.GetLine(pointStart.VirtualLine) as RenderedLine;

                                    CoordinatePoint startCoordinatePoint = m_parser.GetCoordinatePoint(startLine.LineStartPoint);
                                    CoordinatePoint endCoordinatePoint = m_parser.GetCoordinatePoint(startLine.LineEndPoint);

                                    IDynamicFormatsLayer borderLayer = m_formatManager[DEF_LAYER_NAME_BORDER_DYNAMIC];
                                    IList layer = borderLayer[startCoordinatePoint, endCoordinatePoint];

                                    if (layer.Count >= 1)
                                    {
                                        int y = (int)startLine.Y;
                                        InvalidateAll(new Rectangle(0, y - BORDER_INVALIDATION_PIXELS, int.MaxValue,
                                            (int)startLine.Height + (2 * BORDER_INVALIDATION_PIXELS)));
                                    }
                                }

                                CurrentPosition = new Point(pointStart.VirtualColumn, pointStart.VirtualLine);
                            }

                            m_lastOperation.Insert = false;
                            m_lastOperation.InsertText = string.Empty;
                            m_lastOperation.StartColumn = pointStart.VirtualColumn;
                            m_lastOperation.StartLine = pointStart.VirtualLine;
                            m_lastOperation.EndColumn = pointEnd.VirtualColumn;
                            m_lastOperation.EndLine = pointEnd.VirtualLine;

                            RaiseUpdateChangedStateEvent();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Executes some actions after changing configuration.
        /// </summary>
        protected virtual void OnConfigurationChanged()
        {
            RaiseConfigurationChangedEvent();
            ResetColoring(Language);
        }
        /// <summary>
        /// Executes some actions after changing configuration language.
        /// </summary>
        protected virtual void OnLanguageChanged()
        {
            RaiseLanguageChangedEvent();
            m_bUpdateScrollInfo = true;
        }
        /// <summary>
        /// Closes input stream wrapper. Does not close underlying stream.
        /// </summary>
        /// <returns>True if operation succeeds.</returns>
        protected virtual bool CloseStream()
        {
            if (m_codeSnippetsManager != null)
                m_codeSnippetsManager.Activated = false;

            if (m_wrapper != null)
            {
                if (!RaiseChangingStreamEvent()) return false;

                StopSelection();
                SelectionCancel();

                DestroyParser();

                m_wrapper.Close(false);
                m_wrapper = null;
                m_bModified = false;
            }

            return true;
        }
        /// <summary>
        /// Handler of OperationStarted event of parser.
        /// </summary>
        /// <param name="operation">Operation.</param>
        protected void ProcessOperationStart(ILongOperation operation)
        {
            m_operationLevel++;

            if (threadingTimer == null)
            {
                threadingTimer = new System.Threading.Timer(new TimerCallback(SetWaitCursor), operation, DEF_LONGOP_TIMER_DELAY, Timeout.Infinite);
            }

            if (OperationStarted != null)
            {
                OperationStarted(operation);
            }
        }
        /// <summary>
        /// Handler of OperationStopped event of parser.
        /// </summary>
        /// <param name="operation">Operation.</param>
        protected void ProcessOperationEnd(ILongOperation operation)
        {
            if (m_operationLevel != 0)
            {
                m_operationLevel--;
                if (m_operationLevel == 0)
                {
                    Cursor.Current = this.DefaultCursor;
                }
                if (OperationStopped != null)
                {
                    OperationStopped(operation);
                }
            }
        }
        /// <summary>
        /// Gets line that is next to the given one.
        /// </summary>
        /// <param name="line">Current line.</param>
        /// <returns>RenderedLine object.</returns>
        protected RenderedLine GetNextPreRenderedLine(RenderedLine line)
        {
            if (line == null) throw new ArgumentNullException("line");

            int iLine = line.LineIndex + 1;
            if (iLine <= m_parser.TotalLines)
            {
                RenderedLine nextLine = m_parser.GetLine(iLine) as RenderedLine;
                return nextLine;
            }

            return null;
        }
        /// <summary>
        /// Get language configuration by given filename. Filename can include path.
        /// If there is not special configuration for such type of files, then Default configuration will be used.
        /// </summary>
        /// <param name="fileName">Path to the file.</param>
        /// <returns>Language configuration.</returns>
        protected IConfigLanguage GetFileLanguage(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");
            if (fileName.Length == 0)
                throw new ArgumentException(Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_68);

            BeginConfigurationChange();
            IConfigLanguage lang = null;

            string ext = Path.GetExtension(fileName);

            if (ext != null && ext != string.Empty)
            {
                ext = ext.Remove(0, 1);
                lang = m_Configuration.GetLanguage(ext);
            }

            if (lang == null)
            {
                lang = m_Configuration.DefaultLanguage;
            }

            EndConfigurationChange(false);
            return lang;
        }
        /// <summary>
        /// Gets length of the line.
        /// </summary>
        /// <param name="iLine">Index of the line, to be measured.</param>
        /// <returns>Length of the line.</returns>
        protected int GetLineLength(int iLine)
        {
            return (m_parser.GetLine(iLine) as RenderedLine).LineLength;
        }
        /// <summary>
        /// Sets caret to given (x, y) position. Position is in coordinates of client area.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        protected virtual void SetCursorToClientPoint(int x, int y)
        {
            SetCursorToPoint(x + AutoScrollPosition.X, y + AutoScrollPosition.Y);
        }
        /// <summary>
        /// Sets caret to given (x, y) position. Position is in coordinates of entire control area.
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        protected virtual void SetCursorToPoint(int x, int y)
        {
            Point point = Point.Empty;
            if (this.RightToLeft == RightToLeft.Yes)
            {
                point = new Point(Math.Max(Math.Min(x, this.TextDrawOffset - 2), 0) - this.AutoScrollPosition.X,
                                Math.Max(Math.Min(y, ClientRectangle.Bottom - 2), 0) - this.AutoScrollPosition.Y);
            }
            else
            {
                point = new Point(Math.Max(Math.Min(x, ClientRectangle.Right - 2), TextDrawOffset + 3) - this.AutoScrollPosition.X,
                 Math.Max(Math.Min(y, ClientRectangle.Bottom - 2), 0) - this.AutoScrollPosition.Y);

                point.X -= this.TextDrawOffset;
            }

            Point pointVirtual = m_CursorManager.PositionConverter.GraphicalToVirtual(point, VirtualSpaceMode);
            RectangleF rect = m_CursorManager.PositionConverter.VirtualToGraphical(pointVirtual);

            if (point.X >= (rect.X + rect.Width / 2.0) && point.X <= rect.Right)
            {
                pointVirtual.X++;
				// Fix for 508
                if (this.Parser.Formats[FormatType.Text].Font.Size <= 7.0f)
                {
                    pointVirtual.X--;
                }
                //End
            }
            if (pointVirtual.X != CurrentColumn || pointVirtual.Y != CurrentLine)
            {
                CurrentPosition = pointVirtual;
            }
        }
        /// <summary>
        /// Searches for the <see cref="Syncfusion.Windows.Forms.Edit.Interfaces.IParsePoint"/> at the given position.
        /// </summary>
        /// <remarks>
        /// If it can not be found (it is in virtual space), then you will get parse point, pointing to the beginning of the next line.
        /// If it can not be done, ParsePoint, pointing to the end of current line will be returned.
        /// </remarks>
        /// <param name="iLine">Line index, the ParsePoint is needed for.</param>
        /// <param name="iColumn">Column index, the ParsePoint is needed for. Can be 0.</param>
        /// <returns>ParsePoint to the given position.</returns>
        protected CoordinatePoint GetNearestParsePointRight(int iLine, int iColumn)
        {
            return Parser.GetNearestParsePointRight(iLine, iColumn);
        }
        /// <summary>
        /// Searches for the <see cref="Syncfusion.Windows.Forms.Edit.Interfaces.IParsePoint"/> at the given position.
        /// </summary>
        /// <remarks>
        /// If it can not be found ( or column is 0), and  if it is in virtual space, then you will get parse point to the end of given line;
        /// If column is 0, then you will get parse point to the end of the previous line( if it is one ).
        /// </remarks>
        /// <param name="iLine">Line index the ParsePoint is needed for.</param>
        /// <param name="iColumn">Column index the ParsePoint is needed for. Can be 0.</param>
        /// <returns>ParsePoint to the given position.</returns>
        protected CoordinatePoint GetNearestParsePointLeft(int iLine, int iColumn)
        {
            return this.Parser.GetNearestParsePointLeft(iLine, iColumn);
        }

        /// <summary>
        /// Checks whether the line needs collapse.
        /// </summary>
        /// <param name="rLine">Rendered line</param>
        /// <returns>true if should not collpase the specified region, otherwise false</returns>
        private bool NeedCollapse(RenderedLine rLine)
        {
            int position = 0;

            foreach (IRenderedLexem lexem in rLine.LineLexems)
            {
                ILexemLine lastLine = GetLine(+1);

                if (rLine.LineEndPoint.Line == rLine.LineStartPoint.Line)
                {
                    if (GetLexemCollapsingType(lexem) == LexemCollapsingType.RegionStart)
                    {
                        if (lexem.Length + position + 1 == rLine.LineEndPoint.Position)
                        {
                            return false;
                        }
                    }
                }
                position += lexem.Length;
            }
            return true;
        }

        /// <summary>
        /// Processes click on collapse rectangle of some line.
        /// </summary>
        /// <param name="line">Clicked line.</param>
        protected virtual void ProcessClickOnLineCollapse(ILexemLine line)
        {
            if (line == null) throw new ArgumentNullException("line");

            m_listCollapsed.Clear();
            m_listNotCollapsed.Clear();

            RenderedLine rLine = line as RenderedLine;

            if (!this.NeedCollapse(rLine))
            {
                return;
            }
            
            FillLineCollapsers(rLine, m_listCollapsed, m_listNotCollapsed);

            bool bNeedUnCollapse = (m_listCollapsed.Count > 0);
            foreach (ILexem lex in rLine.LineLexems)
            {
                if (lex.Text == "Then " || lex.Text == "then ")
                {
                    multiEndBlock = false;
                    break;
                }
                else
                {
                    multiEndBlock = true;
                }
            }
            ProcessCollapsing((!bNeedUnCollapse) ? (m_listNotCollapsed) : (m_listCollapsed), !bNeedUnCollapse);
            
            //Fix SD3835 
            this.Refresh();
            this.Invalidate(true);
            //End
        }
        /// <summary>
        /// Gets lexem collapsing type. Lexem collapsing type determines lexem's relation to the collapsible regions.
        /// </summary>
        /// <param name="lexem">Lexem to be checked.</param>
        /// <returns>Value of LexemCollapsingType type.</returns>
        protected LexemCollapsingType GetLexemCollapsingType(ILexem lexem)
        {
            ICollapsableConfigLexem collapseConfig = lexem.Config as ICollapsableConfigLexem;
            LexemCollapsingType result = LexemCollapsingType.None;

            bool bCollapsedBlock = (null != lexem.Collapser && lexem.Collapser.Collapsed);
            if (bCollapsedBlock)
            {
                result = LexemCollapsingType.CollapsedLexem;
            }
            else
            {
                if (collapseConfig != null && collapseConfig.IsCollapsable)
                {
                    if (collapseConfig.IsEqualToBegin(lexem.Text))
                    {
                        result = LexemCollapsingType.RegionStart;
                    }
                    else if (collapseConfig.IsEqualToEnd(lexem.Text))
                    {
                        result = LexemCollapsingType.RegionEnd;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Fills lists of collapsed and uncollapsed regions from specified line.
        /// </summary>
        /// <param name="line">Line to be inspected.</param>
        /// <param name="collapsed">List of collapsed regions.(Must be empty)</param>
        /// <param name="notCollapsed">List of uncollapsed regions.(Must be empty)</param>
        protected void FillLineCollapsers(RenderedLine line, IList collapsed, IList notCollapsed)
        {
            if (line == null) throw new ArgumentNullException("line");
            if (collapsed == null) throw new ArgumentNullException("collapsed");
            if (notCollapsed == null) throw new ArgumentNullException("uncollapsed");

            m_parser.MeasureLine(line);
         
            foreach (IRenderedLexem lex in line.LineLexems)
            {
              
                LexemCollapsingType collapse = GetLexemCollapsingType(lex);
              
                switch (collapse)
                {
                    case LexemCollapsingType.CollapsedLexem:
                        collapsed.Add(lex.Collapser);
                        break;
                    case LexemCollapsingType.RegionStart:
                        lex.Collapser.ResetEnd();
                        notCollapsed.Add(lex.Collapser);
                        break;
                                             
                }
               
            }
        }

               /// <summary>
        /// Collapses and expands specified regions in specified line.
        /// </summary>
        /// <param name="toCollapse">List of <see cref="CollapsableRegion"/> to be collapsed.</param>
        /// <param name="bCollapse">If true, regions will be collapsed, otherwise - uncollapsed.</param>
        /// <returns>Bool indicating whether changes were not cancelled by user.</returns>
        protected bool ProcessCollapsing(IList toCollapse, bool bCollapse)
        {
                       
            if (toCollapse == null) throw new ArgumentNullException("toCollapse");

            bool bOK = true;
           
            if (multiEndBlock && toCollapse.Count > 0)
            {
                LockUpdate();
                using (ILongOperation operation = Parser.StartOperation("Collapsing/Expanding text."))
                {
                    Point pointScroll = AutoScrollPosition;
                    // Cancel selection
                    StopSelection();
                    SelectionCancel();

                    // Remember old coordinates of cursor.
                    CoordinatePoint oldCursor = GetNearestParsePointLeft(CurrentLine, CurrentColumn);

                    // Fix for 3765
                    if (oldCursor == null)
                    {
                        oldCursor = GetNearestParsePointLeft(CurrentLine, CurrentColumn - 1);
                    }
                    //end

                    // If cursor is in virtual space, we have to store it's offset from line's last symbol.
                    int iCursorOffset = CurrentColumn - oldCursor.VirtualColumn;

                    m_parser.BeginUpdateRegions();
                    // Process collapsing.
                    
                    foreach (CollapsableRegion collapseRegion in toCollapse)
                    {
                       
                            collapseRegion.End = null;
                            collapseRegion.ResetEnd();
                      
                        collapseRegion.Collapsed = bCollapse;
                        // Changes may be cancelled by user.
                        if (collapseRegion.Collapsed != bCollapse) bOK = false;
                        
                    }
                    m_parser.EndUpdateRegions();
                    

                    // Update scroller`s size
                    UpdateScrollerVerticalSize();
                    // Get new coordinates of the cursor
                    CoordinatePoint newCursor = m_parser.GetCoordinatePoint(oldCursor.PhysicalPoint, true);
                    // If we were moved to start of the collapsed region, then do not use offset.
                    if (oldCursor.PhysicalPoint.Offset != newCursor.PhysicalPoint.Offset)
                    {
                        iCursorOffset = 0;
                    }
                    // Update cursors location.
                    CurrentPosition = new Point(newCursor.VirtualColumn + iCursorOffset, newCursor.VirtualLine);
                    // Updates Y position of all lines.
                    FixLineRenderingPositions();

                    pointScroll.X = -pointScroll.X;
                    pointScroll.Y = -pointScroll.Y;
                    AutoScrollPosition = pointScroll;
                }
                
                UnlockUpdate();
                
            }
            
            return bOK;
        }
        /// <summary>
        /// Searches additional formatting for specified region and converts it to array of <see cref="AdditionalFormatting"/> objects.
        /// </summary>
        /// <param name="startPoint">Start of the region.</param>
        /// <param name="endPoint">End of the region.</param>
        /// <param name="afterTextFormat">OUT format that continues after region end.</param>
        /// <returns>Array of <see cref="AdditionalFormatting"/> objects.</returns>
        protected AdditionalFormatting[] GetAndConvertFormattings
            (CoordinatePoint startPoint, CoordinatePoint endPoint, out ISnippetFormat afterTextFormat)
        {
            IList dfList = m_formatManager[startPoint, endPoint];
            AdditionalFormatting[] result = null;
            afterTextFormat = null;

            if (dfList.Count > 0)
            {
                result = new AdditionalFormatting[dfList.Count];
            }
            if (dfList.Count > 0 && m_formatManager[DEF_LAYER_NAME_LINEBACKCOLOR_DYNAMIC] != null)
            {
                dfList.Clear();
                for (int i = 0; i < m_formatManager[DEF_LAYER_NAME_LINEBACKCOLOR_DYNAMIC][startPoint, endPoint].Count; i++)
                {
                    if (backcolor_startpoint == null || backcolor_startpoint.VirtualLine != startPoint.VirtualLine)
                        dfList.Add(m_formatManager[DEF_LAYER_NAME_LINEBACKCOLOR_DYNAMIC][startPoint, endPoint][i]);
                }
                for (int i = 0; i < m_formatManager[startPoint, endPoint].Count; i++)
                {
                    dfList.Add(m_formatManager[startPoint, endPoint][i]);
                }
                result = new AdditionalFormatting[dfList.Count];
            }
            for (int i = 0; i < dfList.Count; i++)
            {
                IDynamicFormat frm = dfList[i] as IDynamicFormat;
                result[i].Format = frm.Format;

                if (frm.Start.VirtualLine == startPoint.VirtualLine && frm.Start.VirtualColumn > startPoint.VirtualColumn)
                {
                    result[i].StartLetterIndex = frm.Start.VirtualColumn - startPoint.VirtualColumn;
                }
                else
                {
                    result[i].StartLetterIndex = 0;
                }

                int iLastLetter = endPoint.VirtualColumn - startPoint.VirtualColumn - 1;

                if (frm.End.VirtualLine == startPoint.VirtualLine && (frm.End.VirtualColumn - startPoint.VirtualColumn > 0))
                {
                    result[i].EndLetterIndex = Math.Min(iLastLetter, frm.End.VirtualColumn - startPoint.VirtualColumn - 1);
                }
                else
                {
                    result[i].EndLetterIndex = iLastLetter;
                    afterTextFormat = result[i].Format;
                }
            }

            return result;
        }
        /// <summary>
        /// Removes selected text.
        /// </summary>
        protected virtual bool DeleteSelected()
        {
            bool result = false;
            if (!m_selection.IsEmpty() && m_selection.Top != m_selection.Bottom)
            {
                ArrayList rangesToDelete = new ArrayList();
                rangesToDelete.AddRange(m_selection.Ranges);
                LockUpdate();
                LockSelection();
                foreach (TextRange range in rangesToDelete)
                {
                    TextDeleteInternal(range.Top.VirtualLine, range.Top.VirtualColumn, range.Bottom.VirtualLine, range.Bottom.VirtualColumn);
                }
                UnlockSelection();
                UnlockUpdate();
                result = true;
            }
            return result;
        }
        /// <summary>
        /// Creates new lexem parser and disposes old one.
        /// </summary>
        /// <param>StreamsWrapper to be used.</param>
        /// <param>Configuration language.</param>
        /// <summary>
        /// CreateParser method that used to create new parser
        /// </summary>
        /// <param name="newWrapper"></param>
        /// <param name="newLang"></param>
        protected virtual void CreateParser(StreamsWrapper newWrapper, IConfigLanguage newLang)
        {
            if (newWrapper == null) throw new ArgumentNullException("newWrapper");
            if (newLang == null) throw new ArgumentNullException("newLang");
            HideIndentGuideline();
            using (((ILongOperationController)this).StartOperation("Creating parser"))
            {
                bool bSameText = (m_parser != null && newWrapper == m_parser.BaseStream);
                LexemParser.UndoRedoData parserUndo =
                    (bSameText) ? ((LexemParser.UndoRedoData)m_parser.UndoData.Clone()) : (new LexemParser.UndoRedoData());
                if (!bSameText)
                {
                    OnBaseStreamUndoBufferFlush(this, EventArgs.Empty);
                    OnBaseStreamRedoBufferFlush(this, EventArgs.Empty);
                }

                SavedViewInfo viewInfo = SaveCurrentViewInfo();
                if (m_parser != null)
                {
                    DestroyParser();
                }

                m_parser = new RenderableLexemParser(newWrapper, newLang, parserUndo);
                m_parser.MaxWidth = MaxWidth;
                m_parser.LineInstanceDeleted += new EventHandler(OnParserLineInstanceDeleted);
                m_parser.LinesCountChanged += new ValueChangedEventHandler(OnLinesCountChanged);
                m_parser.LineInserted += new LineInsertedEventHandler(OnLineInserted);
                m_parser.LineDeleted += new LineDeletedEventHandler(OnLineDeleted);
                m_parser.TextDeleted += new TextChangedEventHandler(OnTextDeleted);
                m_parser.TextInserted += new TextChangedEventHandler(OnTextInserted);
                m_parser.TextInserting += new TextChangingEventHandler(OnTextInserting);
                m_parser.TextDeleting += new TextChangingEventHandler(OnTextDeleting);
                m_parser.OperationStarted += new LongOperationEventHandler(ProcessOperationStart);
                m_parser.OperationStopped += new LongOperationEventHandler(ProcessOperationEnd);
                m_parser.BaseStream.UndoBufferFlushed += new EventHandler(OnBaseStreamUndoBufferFlush);
                m_parser.BaseStream.RedoBufferFlushed += new EventHandler(OnBaseStreamRedoBufferFlush);
                m_parser.OutliningStateChanged += new EventHandler(OnParserOutliningStateChanged);
                m_parser.LineIndexChanged += new EventHandler(OnParserLineIndexChanged);
                m_parser.Formats.ShowWhiteSpaces = m_bShowWhitespaces;
                m_parser.TabLength = m_iTabSize;
                m_parser.UseNativeDrawing = this.UseNativeDrawing;
                m_parser.SpaceBetweenLines = this.SpaceBetweenLines;
                (m_parser.Formats as FormatManager).ShowWhiteSpaceProperties = m_whitespaceProps;

                if (m_wrapType == WordWrapType.WrapByChar)
                {
                    m_parser.CharWrap = true;
                }
                m_parser.MaxWidth = MaxWidth;
                UpdateParserOffsetsInfo();

                m_parser.BaseStream.AfterTextChange += new EventHandler(OnBaseStreamAfterTextChange);

                if (null != ParserCreated)
                {
                    ParserCreated(this, EventArgs.Empty);
                }
                if (null != EncodingChanged)
                {
                    EncodingChanged(this, EventArgs.Empty);
                }

                m_CursorManager = new ScrollableCursorManager(this, m_parser as IPositionConverter);
                m_CursorManager.CoordinatesChanged += new EventHandler(OnCursorManagerCoordinatesChanged);
                m_CursorManager.LineChanged += new ValueChangedEventHandler(OnCursorManagerLineChanged);
                m_CursorManager.BeforeCoordinatesChange += new CoordinatesChangeEventHandler(OnCursorManagerBeforeCoordinatesChange);
                m_CursorManager.VirtualSpaceMode = m_bVirtualSpaceMode;

                m_formatManager = new DynamicFormatManager();
                m_formatManager.RegisterLayer(DEF_LAYER_NAME_WAVELINE, true).DataChanged += new EventHandler(OnDynamicFormatLayerChanged);
                m_formatManager.RegisterLayer(DEF_LAYER_NAME_LINEBACKCOLOR_DYNAMIC, false);
                m_formatManager.RegisterLayer(DEF_LAYER_NAME_LINEBACKCOLOR_OVER, true).DataChanged += new EventHandler(OnDynamicFormatLayerChanged);
                m_formatManager.RegisterLayer(DEF_LAYER_NAME_STRIKEOUTS);
                m_formatManager.RegisterLayer(DEF_LAYER_NAME_BORDER_DYNAMIC);
                m_formatManager.RegisterLayer(DEF_LAYER_NAME_BORDER_OVER, true).DataChanged += new EventHandler(OnDynamicFormatLayerChanged);
                m_formatManager.RegisterLayer(DEF_LAYER_NAME_TEXTCOLOR);
                m_formatManager.RegisterLayer(DEF_LAYER_NAME_READONLY);
                m_formatManager.RegisterLayer(DEF_LAYER_NAME_SELECTION);

                VirtualSize = (!DisableScrollers) ?
                    (new Size(0, Math.Min(m_parser.TotalLines, DEF_MAX_UNRENDERED_LINES) * m_parser.Formats.MaxLineHeight)) : (Size);

                RecalculateSpaces((m_parser != null) ? (m_parser.TotalLines) : (DEF_NUMBERS_SIZE));
                m_parser.GetLineByY(ClientRectangle.Bottom);

                if (bSameText)
                {
                    RestoreViewInfo(viewInfo);
                }
                else
                {
                    AutoScrollPosition = Point.Empty;
                    CurrentPosition = new Point(1, 1);
                    SelectionCancel();
                }
                m_CursorManager.Update();

                InvalidateAll();
                OnLanguageChanged();
                RaiseUpdateChangedStateEvent();

                GC.Collect();
            }
            if (m_CursorManager!=null)
                m_CursorManager.Visible = true;
        }
        /// <summary>
        /// Gets lexem under cursor.
        /// </summary>
        /// <param name="correct">Specifies whether function should return nearest lexem if there is no lexem under cursor.</param>
        /// <returns>IRenderedLexem instance.</returns>
        protected IRenderedLexem GetLexemUnderCursor(bool correct)
        {
            RenderedLine line = CurrentLineInstanceInternal;

            if (line != null)
            {
                if (!line.IsMeasured)
                {
                    m_parser.MeasureLine(line);
                }

                int column = CurrentColumn;
                IRenderedLexem lexem = line.FindLexemByColumn(column) as IRenderedLexem;
                if (lexem != null && lexem.Length > 0 && lexem.Text.Trim().Length == 0 && column > 0) 
                {
                    lexem = line.FindLexemByColumn(column - 1) as IRenderedLexem;
                }
                if (correct)
                {
                    if (lexem != null && lexem.Config.Type == FormatType.Whitespace && column == lexem.Column && column > 1)
                    {
                        lexem = line.FindLexemByColumn(column - 1) as IRenderedLexem;
                    }
                    if (lexem == null && line.LineLexems.Count > 0)
                    {
                        IRenderedLexem lastLexem = line.LineLexems[line.LineLexems.Count - 1] as IRenderedLexem;
                        if (lastLexem.Column + lastLexem.Length == CurrentColumn)
                        {
                            lexem = lastLexem;
                        }
                    }
                }

                return lexem;
            }

            return null;
        }
        /// <summary>
        /// If there is some text selected, then check whether it matches (\w|\s|\d)+ mask, or if there is no selected text, then just get current lexem.
        /// </summary>
        /// <returns>String.</returns>
        protected string SmartGetCurrentText()
        {
            string result = string.Empty;

            string selectedText = SelectedText;
            if (selectedText == string.Empty)
            {
                IRenderedLexem lexem = GetLexemUnderCursor();

                if (lexem != null)
                {
                    result = lexem.Text;
                }
            }

            if (Regex.IsMatch(selectedText, DEF_SMART_TEXT_CHECK) ||!(Regex.IsMatch(selectedText,"\n")))
            {
                result = selectedText;
            }

            return result;
        }
        /// <summary>
        /// Checks weather cursor position belongs to the word, that was under cursor when context choice was opened.
        /// </summary>
        /// <returns>False if context choice should be closed.</returns>
        protected bool ContextChoiceCheckCursorPosition()
        {
            ContextChoiceController.AutoCompleteStringInfo ac = m_controllerContextChoice.GetAutoCompleteStringInfo();
            int iNeededColumn = ( /*this.m_controllerContextChoice.UseAutocomplete && */!ac.IsEmpty()) ? (ac.Column) : (CurrentColumn);
            return (m_iContextChoiceLastWordColumn == iNeededColumn);
        }
        /// <summary>
        /// Updates auto-complete dialog.
        /// </summary>
        protected void UpdateContextChoice()
        {
            if (ContextChoiceOn)
            {
                if (!ContextChoiceCheckCursorPosition())
                {
                    m_controllerContextChoice.Close();
                }
                else
                {
                    m_controllerContextChoice.Update();
                }
            }
        }
        /// <summary>
        /// Tries to complete word.
        /// </summary>
        protected void TryToCompleteWord()
        {
            // Check whether we can already close the autocomplete.
            if (m_controllerContextChoice.IsVisible)
            {
                IContextChoiceItem[] items = m_controllerContextChoice.GetVisibleItems();

                if (items.Length == 1)
                {
                    m_controllerContextChoice.SelectedItem = items[0];

                    Point pointStart = CurrentPosition;

                    LockSelection();
                    m_controllerContextChoice.Close(false);
                    UnlockSelection();
                }
            }
        }
        /// <summary>
        /// Gets text of the line.
        /// </summary>
        /// <param name="iVirtualLine">Index of the line.</param>
        /// <returns>Text of the line excluding end-line symbols.</returns>
        protected string GetLineTextInternal(int iVirtualLine)
        {
            ILexemLine line = m_parser.GetLine(iVirtualLine);
            StringBuilder result = new StringBuilder();

            foreach (ILexem lex in line.LineLexems)
            {
                result.Append(lex.Text);
            }

            return result.ToString();
        }
        /// <summary>
        /// Gets text of the line.
        /// </summary>
        /// <param name="iLine">Index of line.</param>
        /// <returns>Text of the line excluding end-line symbols.</returns>
        protected string GetPhysicalLineTextInternal(int iLine)
        {
            if (iLine < 1 || iLine > m_parser.BaseStream.LinesCount) throw new ArgumentOutOfRangeException(
               "iLine", iLine, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_83);

            IParsePoint start = m_parser.BaseStream.GetParsePoint(iLine, 1, true);
            IParsePoint end = (iLine < m_parser.BaseStream.LinesCount)
                ? m_parser.BaseStream.GetParsePoint(iLine + 1, 1, true) : m_parser.BaseStream.GetParsePoint(m_parser.BaseStream.Length);

            return m_parser.BaseStream.GetTextInRange(start, end, true);
        }
        /// <summary>
        /// Drops measuring info for lines, if it is incorrect according to the current maxwidth.
        /// </summary>
        protected void UpdateMeasuringInfo()
        {
            if (m_parser != null)
            {
                HideIndentGuideline();
                m_parser.CharWrap = (m_wrapType == WordWrapType.WrapByChar);

                int droppedLines = 0;
                int iMaxWidth = MaxWidth;
                IEnumerator enumerator = m_parser.GetLineEnumerator();

                while (enumerator.MoveNext())
                {
                    RenderedLine line = enumerator.Current as RenderedLine;
                    if (line.IsMeasured)
                    {
                        bool needDropping = line.Width > iMaxWidth;
                        if (!needDropping && (line.SubLinesCount > 1))
                        {
                            int lastLine = 0;
                            foreach (IRenderedLexem lexem in line.LineLexems)
                            {
                                bool bLineChanged = (lexem.SubLine != lastLine);
                                if (bLineChanged)
                                {
                                    lastLine = lexem.SubLine;
                                }
                                if (bLineChanged && lexem.YOffset != 0)
                                {
                                    needDropping = true;
                                    break;
                                }
                            }
                        }
                        if (needDropping)
                        {
                            droppedLines++;
                            line.DropMeasureInfo();
                        }
                    }
                }

                Parser.MaxWidth = iMaxWidth;
                if (droppedLines > 0)
                {
                    VirtualSize = (DisableScrollers) ? (Size.Empty) : (new Size(0, VirtualSize.Height));
                    FixLineRenderingPositions();
                    InvalidateAll();
                    Update();
                    UpdateScrollerVerticalSize(this, true);
                }
            }
        }
        /// <summary>
        /// Updates offsets of the scrollable area.
        /// </summary>
        /// <param name="control">IntelliScrollableControl.</param>
        protected void UpdateScrollerOffsets(IntelliScrollableControl control)
        {
            if (this.RightToLeft == RightToLeft.Yes)
            {
                control.ScrollOffsetLeft = (m_bShowUserMargin && m_userMarginPlacement == MarginPlacement.Right) ? m_iUserMarginWidth : 2;
                control.ScrollOffsetRight = this.ClientRectangle.Width - this.TextDrawOffset;
            }
            else
            {
                control.ScrollOffsetLeft = this.TextDrawOffset - 2;
                control.ScrollOffsetRight = (m_bShowUserMargin && m_userMarginPlacement == MarginPlacement.Right) ? (m_iUserMarginWidth) : (0);
            }
        }
        /// <summary>
        /// Checks, whether given client coordinates belong to the selected area.
        /// </summary>
        /// <param name="mousePoint">Point in client coordinates.</param>
        /// <returns>True, if there is some area selected and given coordinates belong to the selection, otherwise - false.</returns>
        protected bool PointBelongsToSelection(Point mousePoint)
        {
            bool bResult = false;
            Point coordinates = PointToVirtualPosition(mousePoint);

            if (!coordinates.IsEmpty && m_selection.Exists())
            {
                int rangeIndex = 0;
                TextRange range = null;
                int line = coordinates.Y;
                int col = coordinates.X;
                int visualStart = m_selection.VisualTopLeft.Offset;
                int visualEnd = m_selection.VisualBottomRight.Offset;
                int visualCol = GetVisualLocation(line, coordinates.X).Offset;

                if (!m_selection.IsBlock())
                {
                    int topLine = m_selection.Top.VirtualLine;
                    int bottomLine = m_selection.Bottom.VirtualLine;
                    int topCol = m_selection.Top.VirtualColumn;
                    int bottomCol = m_selection.Bottom.VirtualColumn;
                    bResult = (topLine < line && bottomLine > line);
                    if (!bResult)
                    {
                        if (topLine == line)
                        {
                            if (topLine == bottomLine)
                            {
                                bResult = (topCol <= col && bottomCol > col);
                            }
                            else
                            {
                                bResult = (topCol <= col);
                            }
                        }
                        else if (bottomLine == line)
                        {
                            bResult = (bottomCol > col);
                        }
                    }
                }
                else
                {
                    for (; rangeIndex < m_selection.Ranges.Count && !bResult; rangeIndex++)
                    {
                        range = (TextRange)m_selection.Ranges[rangeIndex];
                        bResult = (range.Top.VirtualLine < line && range.Bottom.VirtualLine > line);
                        if (!bResult)
                        {
                            if (line == range.Top.VirtualLine)
                            {
                                bResult = GetVisualLocation(range.Top.VirtualLine, range.Top.VirtualColumn).Offset <= visualCol;
                            }
                            else if (line == range.Bottom.VirtualLine)
                            {
                                bResult = GetVisualLocation(range.Bottom.VirtualLine, range.Bottom.VirtualColumn).Offset > visualCol;
                            }
                        }
                    }

                    CoordinatePoint cursorPoint = new CoordinatePoint(m_parser, null, coordinates.Y, coordinates.X, false);
                    // We should check whether we are really inside the selection
                    if (bResult && range.Bottom == cursorPoint)
                    {
                        RectangleF rect = Parser.VirtualToGraphical(coordinates);

                        rect.X += this.AutoScrollPosition.X + this.TextDrawOffset;
                        rect.Y += this.AutoScrollPosition.Y;

                        bResult = rect.Contains(mousePoint);
                    }
                }
            }
            return bResult;
        }
        /// <summary>
        /// Checks whether the specified point belongs to the text area.
        /// </summary>
        /// <param name="point">The point in the client coordinates.</param>
        /// <returns>True is the point belong the text area, otherwise false.</returns>
        protected bool PointBelongToTextArea(Point point)
        {
            Rectangle rect = Rectangle.Empty;

            if (this.RightToLeft == RightToLeft.Yes)
                rect = new Rectangle(0, 0, this.ClientSize.Width - this.ScrollOffsetRight, this.ClientSize.Height);
            else
                rect = new Rectangle(this.TextDrawOffset, 0, this.ClientSize.Width - this.TextDrawOffset - this.ScrollOffsetRight, this.ClientSize.Height);
            return rect.Contains(point);
        }
        /// <summary>
        /// Calculates region that is used to display text within specified range.
        /// </summary>
        /// <param name="startPoint">Start of the range.</param>
        /// <param name="endPoint">End of the range.</param>
        /// <returns>Region that is used to display text within specified range.</returns>
        protected Region GetTextDrawRegion(CoordinatePoint startPoint, CoordinatePoint endPoint)
        {
            GraphicsPath path = GetTextDrawPath(startPoint, endPoint);
            Region result = null;

            if (path != null)
            {
                result = new Region(path);
                path.Dispose();
            }

            return result;
        }
        /// <summary>
        /// Calculates region that is used to display text within specified range.
        /// </summary>
        /// <param name="rectStart">Start of the range.</param>
        /// <param name="rectEnd">End of the range.</param>
        /// <param name="bInfinityRightLimit">Indicates whether right limit should be set to infinity.</param>
        /// <returns>Region that is used to display text within specified range.</returns>
        protected GraphicsPath GetTextDrawPath(RectangleF rectStart, RectangleF rectEnd, bool bInfinityRightLimit)
        {
            if (rectEnd.Left == DEF_OFFSET_TEXT)
            {
                rectEnd.Height = 0;
                rectEnd.Y--;
            }

            GraphicsPath path = new GraphicsPath(FillMode.Alternate);
            bool bNonSingleLine = (rectStart.Top != rectEnd.Top);

            path.StartFigure();
            path.AddLine(rectStart.Left, rectStart.Bottom, rectStart.Left, rectStart.Top);

            if (bNonSingleLine)
            {
                int rightLimit;
                if (this.WordWrap)
                {
                    rightLimit = m_parser.MaxWidth + 1;

                    if (this.MarkLineWrapping)
                    {
                        rightLimit += this.WrapMarkingImage.Width;
                    }
                }
                else
                {
                    rightLimit = (bInfinityRightLimit) ? (Math.Max(this.Width, this.VirtualSize.Width)) : (this.Width);
                }

                path.AddLine(rectStart.Left, rectStart.Top, rightLimit, rectStart.Top);
                path.AddLine(rightLimit, rectStart.Top, rightLimit, rectEnd.Top);
                path.AddLine(rightLimit, rectEnd.Top, Math.Max(rectEnd.Left, rectStart.Left), rectEnd.Top);
            }
            else
            {
                path.AddLine(rectStart.Left, rectStart.Top, rectEnd.Left, rectEnd.Top);
            }

            if (rectEnd.Left < rectStart.Left && bNonSingleLine && rectEnd.Top <= rectStart.Bottom+1)
            {
                path.CloseFigure();
                path.StartFigure();
            }

            path.AddLine(rectEnd.Left, rectEnd.Top, rectEnd.Left, rectEnd.Bottom);

            if (bNonSingleLine)
            {
                path.AddLine(rectEnd.Left, rectEnd.Bottom, DEF_OFFSET_TEXT, rectEnd.Bottom);
                path.AddLine(DEF_OFFSET_TEXT, rectEnd.Bottom, DEF_OFFSET_TEXT, rectStart.Bottom);
                path.AddLine(DEF_OFFSET_TEXT, rectStart.Bottom, Math.Min(rectStart.Left, rectEnd.Left), rectStart.Bottom);
            }
            else
            {
                path.AddLine(rectEnd.Left, rectEnd.Bottom, rectStart.Left, rectStart.Bottom);
            }

            path.CloseFigure();

            Matrix translateMatrix = new Matrix();
            translateMatrix.Translate(TextDrawOffset, 0);
            path.Transform(translateMatrix);

            return path;
        }

        /// <summary>
        /// Calculates region that is used to display text within specified range.
        /// </summary>
        /// <param name="rectStart">Start of the range.</param>
        /// <param name="rectEnd">End of the range.</param>
        /// <param name="bInfinityRightLimit">Indicates whether right limit should be set to infinity.</param>
        /// <returns>Region that is used to display text within specified range.</returns>
        protected GraphicsPath GetTextDrawPathWithoutRightExtending(RectangleF rectStart, RectangleF rectEnd, bool bInfinityRightLimit)
        {
            if (rectEnd.Left == DEF_OFFSET_TEXT)
            {
                rectEnd.Y--;
            }

            GraphicsPath path = new GraphicsPath(FillMode.Alternate);

            if (rectEnd.Top == rectStart.Top)
            {
                path.AddRectangle(new RectangleF(rectStart.Left, rectStart.Top, rectEnd.Left - rectStart.Left, rectStart.Height));
            }
            else
            {
                RenderedLine startLine = m_parser.GetLineByY(rectStart.Top + rectStart.Height / 2);
                RenderedLine endLine = m_parser.GetLineByY(rectEnd.Top + rectEnd.Height / 2);

                bool bNonSingleLine = (startLine != endLine);

                path.StartFigure();
                path.AddLine(rectStart.Left, rectStart.Bottom, rectStart.Left, rectStart.Top);
                Point p = new Point((int)rectStart.Left, (int)rectStart.Top);

                bool bSecondLineDrawn = false;

                if (bNonSingleLine)
                {
                    // First line select
                    IRenderedLexem lastLexem = startLine.GetLastSublineLexem(rectStart.Top + rectStart.Height / 2 - startLine.Y);

                    Point p2;
                    int subLine;
                    if (lastLexem != null)
                    {
                        p2 = new Point((int)(lastLexem.XOffset + lastLexem.Width + PIXEL_TO_ADD_TO_SELECTION), p.Y);
                        subLine = lastLexem.SubLine;
                    }
                    else
                    {
                        p2 = new Point((int)(PIXEL_TO_ADD_TO_SELECTION), p.Y);
                        subLine = 0;
                    }
                    path.AddLine(p, p2);
                    p = p2;
                    p2.Y += (int)startLine.SubLineHeight[subLine];
                    path.AddLine(p, p2);

                    for (int i = subLine + 1; i < startLine.SubLinesCount; i++)
                    {
                        lastLexem = startLine.GetLastSublineLexem(p2.Y + startLine.SubLineHeight[i] / 2 - startLine.Y);
                        if (lastLexem != null)
                        {
                            p = p2;
                            p2.X = (int)(lastLexem.XOffset + lastLexem.Width + PIXEL_TO_ADD_TO_SELECTION);

                            if (!bSecondLineDrawn && p2.X < rectStart.Left)
                            {
                                path.AddLine(p, new Point((int)rectStart.Left, p2.Y));
                                path.CloseFigure();
                                p.X = DEF_OFFSET_TEXT;
                            }

                            bSecondLineDrawn = true;

                            path.AddLine(p, p2);
                            p = p2;
                            p2.Y += (int)startLine.SubLineHeight[i];
                            path.AddLine(p, p2);
                        }
                    }
                    int startvalue = startLine.LineIndex;
                    if(Parser.ParsingMode != TextParsingMode.FullParsing)
                        startvalue = Parser.parsingIndex;
                    // Middle lines select

                    for (int i = startvalue + 1; i <= endLine.LineIndex - 1; i++)
                    {
                        RenderedLine curLine = GetLine(i) as RenderedLine;
                        p2.Y = (int)curLine.Y;

                        for (int j = 0; j < curLine.SubLinesCount; j++)
                        {
                            lastLexem = curLine.GetLastSublineLexem(p2.Y + curLine.SubLineHeight[j] / 2 - curLine.Y);
                            p = p2;

                            if (lastLexem != null)
                            {
                                p2.X = (int)(lastLexem.XOffset + lastLexem.Width + PIXEL_TO_ADD_TO_SELECTION);
                            }
                            else
                            {
                                p2.X = PIXEL_TO_ADD_TO_SELECTION;
                            }

                            if (!bSecondLineDrawn && p2.X < rectStart.Left)
                            {
                                path.AddLine(p, new Point((int)rectStart.Left, p2.Y));
                                path.CloseFigure();
                                p.X = DEF_OFFSET_TEXT;
                            }

                            bSecondLineDrawn = true;
                            path.AddLine(p, p2);
                            p = p2;
                            p2.Y += (int)curLine.SubLineHeight[j];
                            path.AddLine(p, p2);
                        }
                    }

                    // Last line select		

                    lastLexem = endLine.GetLastSublineLexem(rectEnd.Top + rectEnd.Height / 2 - endLine.Y);

                    subLine = (lastLexem != null) ? (lastLexem.SubLine) : (0);

                    for (int i = 0; i < subLine; i++)
                    {
                        lastLexem = endLine.GetLastSublineLexem(p2.Y + endLine.SubLineHeight[i] / 2 - endLine.Y);

                        if (lastLexem != null)
                        {
                            p = p2;
                            p2.X = (int)(lastLexem.XOffset + lastLexem.Width + PIXEL_TO_ADD_TO_SELECTION);

                            if (!bSecondLineDrawn && p2.X < rectStart.Left)
                            {
                                path.AddLine(p, new Point((int)rectStart.Left, p2.Y));
                                path.CloseFigure();
                                p.X = DEF_OFFSET_TEXT;
                            }

                            bSecondLineDrawn = true;

                            path.AddLine(p, p2);
                            p = p2;
                            p2.Y += (int)endLine.SubLineHeight[i];
                            path.AddLine(p, p2);
                        }
                    }

                    if (!bSecondLineDrawn && rectEnd.Left < rectStart.Left)
                    {
                        path.AddLine(p2, new Point((int)rectStart.Left, p2.Y));
                        path.CloseFigure();
                        p.X = DEF_OFFSET_TEXT;
                    }

                    if (rectEnd.X > 0)
                    {
                        RenderedLine curLine = GetLine(endLine.LineIndex) as RenderedLine;

                        path.AddLine(p.X, rectEnd.Top, rectEnd.Left, rectEnd.Top);
                        path.AddLine(rectEnd.Left, rectEnd.Top, rectEnd.Left, rectEnd.Bottom);
                        path.AddLine(rectEnd.Left, rectEnd.Bottom, DEF_OFFSET_TEXT, rectEnd.Bottom);
                        path.AddLine(DEF_OFFSET_TEXT, rectEnd.Bottom, DEF_OFFSET_TEXT, rectStart.Bottom);
                        path.CloseFigure();
                    }
                    else
                    {
                        path.AddLine(p.X, p2.Y, DEF_OFFSET_TEXT, p2.Y);
                        path.AddLine(DEF_OFFSET_TEXT, p2.Y, DEF_OFFSET_TEXT, rectStart.Bottom);
                    }
                }
                else
                {
                    IRenderedLexem startLineLastLexem = startLine.GetLastSublineLexem(rectStart.Top + rectStart.Height / 2 - startLine.Y);
                    IRenderedLexem endLineLastLexem = endLine.GetLastSublineLexem(rectEnd.Top + rectEnd.Height / 2 - endLine.Y);
                    Point p2 = p;
                    int endSubLine = 0;
                    int startSubLine = 0;

                    startSubLine = (startLineLastLexem != null) ? (startLineLastLexem.SubLine) : (0);

                    endSubLine = (endLineLastLexem != null) ? (endLineLastLexem.SubLine) : (0);

                    p2.X = (int)(startLineLastLexem.XOffset + startLineLastLexem.Width + PIXEL_TO_ADD_TO_SELECTION);
                    path.AddLine(p, p2);
                    p = p2;
                    p2.Y += (int)endLine.SubLineHeight[startSubLine];
                    path.AddLine(p, p2);
                    p = p2;

                    for (int i = startSubLine + 1; i < endSubLine; i++)
                    {
                        endLineLastLexem = endLine.GetLastSublineLexem(p2.Y + endLine.SubLineHeight[i] / 2 - endLine.Y);
                        if (endLineLastLexem != null)
                        {
                            p2.X = (int)(endLineLastLexem.XOffset + endLineLastLexem.Width + PIXEL_TO_ADD_TO_SELECTION);

                            if (!bSecondLineDrawn && p2.X < rectStart.Left)
                            {
                                path.AddLine(p, new Point((int)rectStart.Left, p.Y));
                                path.CloseFigure();
                                p.X = DEF_OFFSET_TEXT;
                            }

                            bSecondLineDrawn = true;
                            path.AddLine(p, p2);
                            p = p2;
                            p2.Y += (int)endLine.SubLineHeight[i];
                            path.AddLine(p, p2);
                            p = p2;
                        }
                    }

                    if (!bSecondLineDrawn && rectEnd.Left < rectStart.Left)
                    {
                        path.AddLine(p, new Point((int)rectStart.Left, p.Y));
                        path.CloseFigure();
                        p2.X = DEF_OFFSET_TEXT;
                        bSecondLineDrawn = true;
                    }

                    if (rectEnd.X > 0)
                    {
                        path.AddLine(p2.X, rectEnd.Top, rectEnd.Left, rectEnd.Top);
                        path.AddLine(rectEnd.Left, rectEnd.Top, rectEnd.Left, rectEnd.Bottom);
                        path.AddLine(rectEnd.Left, rectEnd.Bottom, DEF_OFFSET_TEXT, rectEnd.Bottom);
                        path.AddLine(DEF_OFFSET_TEXT, rectEnd.Bottom, DEF_OFFSET_TEXT, rectStart.Bottom);
                        path.CloseFigure();
                    }
                }
            }

            path.CloseFigure();
            Matrix translateMatrix = new Matrix();
            translateMatrix.Translate(TextDrawOffset, 0);
            path.Transform(translateMatrix);
            return path;
        }

        /// <summary>
        /// Calculates region that is used to display text within specified range.
        /// </summary>
        /// <param name="rectStart">Start of the range.</param>
        /// <param name="rectEnd">End of the range.</param>
        /// <returns>Region that is used to display text within specified range.</returns>
        protected GraphicsPath GetTextDrawPath(RectangleF rectStart, RectangleF rectEnd)
        {
            return GetTextDrawPath(rectStart, rectEnd, true);
        }
        /// <summary>
        /// Calculates region, used to draw selected text.
        /// </summary>
        /// <returns>Region of selected text.</returns>
        protected Region GetSelectedTextDrawRegion()
        {
            Region result = null;
            if (!m_selection.IsEmpty() && m_selection.Top != m_selection.Bottom)
            {
                result = new Region();
                foreach (TextRange range in m_selection.Ranges)
                {
                    result.Union(GetTextDrawRegion(range.Top, range.Bottom));
                }
            }

            return result;
        }
        /// <summary>
        /// Calculates region, used to draw selected text.
        /// </summary>
        /// <returns>Path of selected text.</returns>
        protected GraphicsPath GetSelectedTextDrawPath()
        {
            GraphicsPath result = null;
            if (!m_selection.IsEmpty() && m_selection.Top != m_selection.Bottom)
            {
                if (!m_selection.IsBlock())
                {
                    result = new GraphicsPath();
                    foreach (TextRange range in m_selection.Ranges)
                    {
                        GraphicsPath path = GetTextDrawPath(range.Top, range.Bottom);
                        if (path != null)
                        {
                            result.AddPath(path, true);
                        }
                    }
                }
                else
                {
                    result = new GraphicsPath();
                    VisualLocation topLeft = m_selection.VisualTopLeft;
                    Point pTopLeft = new Point(GetVirtualColumn(topLeft.Line, topLeft.SubLine, topLeft.Offset), topLeft.Line);
                    PointF p1 = m_parser.VirtualToGraphical(pTopLeft).Location;
                    VisualLocation bottomRight = m_selection.VisualBottomRight;
                    Point pBottomRight = new Point(GetVirtualColumn(bottomRight.Line, bottomRight.SubLine, bottomRight.Offset), bottomRight.Line);
                    RectangleF endRect = m_parser.VirtualToGraphical(pBottomRight);
                    PointF p2 = new PointF(endRect.Left, endRect.Bottom);
                    result.AddRectangle(new RectangleF(p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y));
                    result.CloseFigure();

                    using (Matrix translateMatrix = new Matrix())
                    {
                        translateMatrix.Translate(this.TextDrawOffset, 0);
                        result.Transform(translateMatrix);
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Inserts typed character in to the text at the current position.
        /// </summary>
        /// <param name="key">Character to insert.</param>
        protected void InsertChar(char key)
        {
            LockSelection();
            StopSelection();
            string str = key.ToString();

            if (!CanEditSelectedText())
            {
                UnlockSelection();
                return;
            }

            if (CurrentLine >= 1 && CurrentColumn >= 1)
            {
                UndoGroupOpen();
                if (!DeleteSelected() && !m_bInsertMode)
                {
                    ILexemLine line = m_parser.GetLine(CurrentLine);
                    if (CurrentColumn < line.LineLength + 1)
                    {
                        TextDeleteInternal(CurrentLine, CurrentColumn, CurrentLine, CurrentColumn + 1);
                    }
                }

                str = ReplaceNewLineKey(str);

                TextInsertInternal(CurrentLine, CurrentColumn, str);
                UpdateIndentation();
                UndoGroupClose();

                RenderedLine lineRendered = m_parser.GetLine(CurrentLine) as RenderedLine;
                if (lineRendered != null)
                {
                    if (!lineRendered.IsMeasured)
                    {
                        m_parser.MeasureLine(lineRendered);
                    }

                    IRenderedLexem lexem = lineRendered.FindLexemByColumn(CurrentColumn - 1) as IRenderedLexem;
                    if (lexem != null && lexem.Config.DropContextChoiceList && lexem.Text != lexem.Config.EndBlock)
                    {
                        if (m_controllerContextChoice.IsVisible)
                        {
                            m_controllerContextChoice.Close();
                        }

                        ShowContextChoice();
                    }
                    else if (lexem != null && lexem.Config.DropContextPrompt)
                    {
                        ShowContextPrompt();
                    }
                }

                UnlockSelection();
            }
        }
        /// <summary>
        /// Gets a value indication whether the selction does not contains read only layer.
        /// </summary>
        /// <returns>True if no read only layer present in the selection range.</returns>
        private bool CanEditSelectedText()
        {
            if (!this.Selection.IsEmpty() && this.Selection.Top != this.Selection.Bottom)
            {
                IDynamicFormatsLayer readOnlyLayer = m_formatManager[DEF_LAYER_NAME_READONLY];
                CoordinatePoint selStart, selEnd;

                selStart = (Selection.Start > Selection.End) ? Selection.End : Selection.Start;
                selEnd = (Selection.Start > Selection.End) ? Selection.Start : Selection.End;

                foreach (DynamicFormat format in readOnlyLayer)
                {
                    if (((selStart <= format.Start && selEnd >= format.End)
                        || (selStart >= format.Start && selStart <= format.End)) && !AllowDeleteReadOnlyRegion)
                        return false;
                }
            }
            return true;
        }
        private int indentLine = 0;
        /// <summary>
        /// Checks weather given string is equal to "\r" and if it is equal and control is not in single line mode and AutoIndent is enabled,
        /// than it will be replaced with indentation spaces.
        /// </summary>
        /// <param name="str">String to change to indentation spaces.</param>
        /// <returns>Resulting string.</returns>
        protected string ReplaceNewLineKey(string str)
        {
            if (str == null) throw new ArgumentNullException("str");

            // If there's something to do.
            if (m_autoIndentMode != AutoIndentMode.None && this.CurrentColumn != 1 && str == "\r" && !SingleLineMode)
            {
                // Line where return key was pressed.
                RenderedLine line = (RenderedLine)m_parser.GetLine(CurrentLine);

                int tabLen = m_parser.TabLength;
                // Number of spaces to input at the start of the next line.
                int n = 0;

                if (AutoIndentMode.Block == m_autoIndentMode)
                {
                    IList lexems = line.LineLexems;

                    for (int i = 0; i < lexems.Count; i++)
                    {
                        RenderedLexem lex = (RenderedLexem)lexems[i];
                        // If white space finished.
                        if (FormatType.Whitespace != lex.Config.Type)
                        {
                            break;
                        }

                        if (DEF_STR_TAB_CHAR == lex.Text)
                        {
                            n += tabLen;
                        }
                        else
                        {
                            n += lex.Length;
                        }
                    }
                    if (this.EnableSmartInBlockIndent)
                    {
                        object[] stackData = GetCurrentStack().ToArray();

                        IParsePoint p = null;
                        IStackData datum = null;
                        n = 0;
                        for (int i = 0, len = stackData.Length; i < len; i++)
                        {
                            datum = (IStackData)stackData[i];
                            if (datum.Lexem != null)
                            {
                                IConfigLexem config = datum.Config;
                                if (config.Indent)
                                {
                                    if (indentLine == 0)
                                    {
                                        // Location of indent lexem needed.
                                        p = datum.Location;
                                    }
                                }
                                else if (indentLine == 1)
                                {
                                    indentLine = 0;
                                }
                                if (null != p)
                                {
                                    n += tabLen;
                                }
                            }
                        }
                    }
                }
                else // smart auto indentation
                {
                    object[] stackData = GetCurrentStack().ToArray();

                    IParsePoint p = null;
                    IStackData datum = null;

                    for (int i = 0, len = stackData.Length; i < len; i++)
                    {
                        datum = (IStackData)stackData[i];
                        IConfigLexem config = datum.Config;

                        if (config.Indent)
                        {
                            // Location of indent lexem needed.
                            p = datum.Location;
                            break;
                        }
                    }

                    // If indent lexem found.
                    if (null != p)
                    {
                        RenderedLexem lex = (RenderedLexem)datum.Lexem;

                        Point pointVirtual = m_parser.PhysicalToVirtual(p);

                        if (pointVirtual == Point.Empty) throw new InvalidOperationException("Physical point can not be converted to virtual point.");

                        // Line of indent lexem.
                        RenderedLine lexLine = (RenderedLine)m_parser.GetLine(pointVirtual.Y);

                        IList lexems = lexLine.LineLexems;

                        for (int i = 0, count = lexems.Count; i < count; i++)
                        {
                            RenderedLexem curLex = (RenderedLexem)lexems[i];
                            // Don't take into account lexems from other sublines.
                            if (lex.SubLine != curLex.SubLine)
                            {
                                continue;
                            }
                            // Stop space measuring.
                            if (pointVirtual.X <= curLex.Column || FormatType.Whitespace != curLex.Config.Type)
                            {
                                break;
                            }

                            if (DEF_STR_TAB_CHAR == curLex.Text)
                            {
                                n += tabLen;
                            }
                            else
                            {
                                n += curLex.Length;
                            }
                        }

                        n += tabLen;
                    }
                }

                int iTabCount = n / tabLen;
                int iSpacesCount = n % tabLen;

                str += new string(DEF_TAB_CHAR, iTabCount);
                str += new string(' ', iSpacesCount);
            }

            return str;
        }
        /// <summary>
        /// Processes keypress of the intellisense keys.
        /// </summary>
        /// <param name="e">KeyPressEventArgs.</param>
        protected void ProcessIntellisenseKey(KeyPressEventArgs e)
        {
            if (ContextChoiceOn)
            {
                UpdateContextChoice();
            }

            if (ContextPromptOn)
            {
                UpdateContextPrompt();
            }
        }
        /// <summary>
        /// Updates context prompt.
        /// </summary>
        protected void UpdateContextPrompt()
        {
            ConfigStack stack = GetCurrentStack();

            if (stack != null && stack.Count > 0)
            {
                IStackData stackItem = stack.Pop();
                while (stack.Count > 1 && !stackItem.Config.DropContextPrompt)
                {
                    stackItem = stack.Pop();
                }

                if (!stackItem.Config.DropContextPrompt)
                {
                    m_contextPrompt.Close();
                    return;
                }
            }

            if (ContextPromptUpdate != null)
            {
                ContextPromptUpdateEventArgs arg = new ContextPromptUpdateEventArgs(
                    m_contextPrompt.List, m_contextPrompt.Dropper, m_contextPrompt.LexemBeforeDropper);
                ContextPromptUpdate(this, arg);
                if (arg.CloseForm)
                {
                    m_contextPrompt.Close();
                }
            }
        }
        /// <summary>
        /// Sets last cursor X position to the current one.
        /// </summary>
        protected void UpdateLastCursorPosition()
        {
            m_lastCursorX = m_CursorManager.CursorGraphicalCoordinates.LeftTop.X;
        }
        /// <summary>
        /// Calculates distance using dx and dy values.
        /// </summary>
        /// <param name="dx">Dx</param>
        /// <param name="dy">Dy</param>
        /// <returns>Distance.</returns>
        protected float GetDistance(float dx, float dy)
        {
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
        /// <summary>
        /// Replaces text of all lexems of the specified type in selection.
        /// </summary>
        /// <param name="filterType">Type of the lexems to process.</param>
        /// <param name="replacer">Event handler used for lexem text replacement.</param>
        protected void ReplaceLexemsInSelection(FormatType filterType, LexemReplaceEventHandler replacer)
        {
            if (!m_selection.IsEmpty())
            {
                ArrayList ranges = (ArrayList)m_selection.Ranges.Clone();
                StopSelection();
                SelectionCancel();
                for (int i = 0, count = ranges.Count; i < count; i++)
                {
                    TextRange range = (TextRange)ranges[i];
                    CoordinatePoint pointStart = m_parser.GetCoordinatePoint(range.Top.PhysicalPoint, true, true);
                    CoordinatePoint pointEnd = m_parser.GetCoordinatePoint(range.Bottom.PhysicalPoint, true, true);
                    ReplaceLexemsInRegion(pointStart.PhysicalPoint, pointEnd.PhysicalPoint, filterType, replacer);
                    range.Start = GetNearestParsePointLeft(pointStart.VirtualLine, pointStart.VirtualColumn);
                    range.End = GetNearestParsePointLeft(pointEnd.VirtualLine, pointEnd.VirtualColumn);
                    pointStart.Dispose();
                    pointEnd.Dispose();
                }

                TextRange lastRange = (TextRange)ranges[ranges.Count - 1];
                this.CurrentPosition = new Point(lastRange.End.VirtualColumn, lastRange.End.VirtualLine);
                m_selection.Ranges.AddRange(ranges);
            }
        }
        /// <summary>
        /// Replaces text of all lexems of the specified type in the specified region.
        /// </summary>
        /// <param name="start">Start position of the region.</param>
        /// <param name="end">End position of the region.</param>
        /// <param name="filterType">Type of the lexems to process.</param>
        /// <param name="replacer">Event handler used for lexem text replacement.</param>
        protected void ReplaceLexemsInRegion(IParsePoint start, IParsePoint end, FormatType filterType, LexemReplaceEventHandler replacer)
        {
            if (null == start) throw new ArgumentNullException("start");
            if (null == end) throw new ArgumentNullException("end");
            if (null == replacer) throw new ArgumentNullException("replacer");

            LockSelection();
            LockUpdate();
            try
            {
                Point startVirtual = m_parser.PhysicalToVirtual(start);

                if (startVirtual.IsEmpty)
                    throw new ArgumentException(Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_179, "start");

                // Get line
                RenderedLine line = (RenderedLine)GetLine(startVirtual.Y);
                m_parser.MeasureLine(line);

                // Get lexem, we are inside of.
                RenderedLexem lexemStart = line.FindLexemByColumn(startVirtual.X) as RenderedLexem;
                int startColumn = (lexemStart != null) ? (lexemStart.Column) : (line.LineLength);

                start = m_parser.VirtualToPhysical(new Point(startColumn, startVirtual.Y));
                ConfigStack stack = line.GetStackByColumn(startColumn);
                LexemParser.LexemParserEnumerator lexemReader = (LexemParser.LexemParserEnumerator)m_parser.GetEnumerator(stack, start);
                lexemReader.Reset();
                long positionOld = lexemReader.CurrentPosition;
                bool bCanReadNext = true;
                ArrayList listRecords = new ArrayList();

                while (lexemReader.MoveNext() && bCanReadNext)
                {
                    ILexem lexem = lexemReader.Current as ILexem;
                    if (lexem == null)
                    {
                        break;
                    }

                    if (lexem.Config.Type == filterType)
                    {
                        string textNew = replacer(lexem);
                        if (textNew != lexem.Text)
                        {
                            IParsePoint pointLexemStart = m_parser.BaseStream.GetParsePoint(positionOld);
                            IParsePoint pointLexemEnd = m_parser.BaseStream.GetParsePoint(lexemReader.CurrentPosition);
                            TextReplacementRecord record = new TextReplacementRecord(pointLexemStart, pointLexemEnd, textNew);
                            listRecords.Add(record);
                        }
                    }

                    positionOld = lexemReader.CurrentPosition;
                    bCanReadNext = (positionOld < end.Offset);
                }

                UndoGroupOpen();

                try
                {
                    foreach (TextReplacementRecord record in listRecords)
                    {
                        Point pointLexemStartVirtual = m_parser.PhysicalToVirtual(record.PointStart);
                        Point pointLexemEndVirtual = m_parser.PhysicalToVirtual(record.PointEnd);
                        TextDeleteInternal(pointLexemStartVirtual.Y, pointLexemStartVirtual.X, pointLexemEndVirtual.Y, pointLexemEndVirtual.X, false);
                        TextInsertInternal(pointLexemStartVirtual.Y, pointLexemStartVirtual.X, record.NewText, false);
                    }
                }
                finally
                {
                    UndoGroupClose();
                }

                m_CursorManager.Update();
                InvalidateAll();
            }
            finally
            {
                UnlockUpdate();
                UnlockSelection();
                // Fix of bug related to application fail during closing without any cursor movement after this method.
                UpdateLinesMeasuring(this.CurrentLine);
            }
        }
        /// <summary>
        /// Checks whether point belongs to the indicator margin.
        /// </summary>
        /// <param name="point">Point (in client coordinates) to be checked.</param>
        /// <returns>True if the selection margin is visible and point belongs to it, otherwise false.</returns>
        protected bool PointBelongToIndicatorMargin(Point point)
        {
            Rectangle markRect = new Rectangle(this.MarkerAreaOffset, 0, m_markerAreaWidth, ClientRectangle.Height);
            return markRect.Contains(point);
        }
        /// <summary>
        /// Checks whether point belongs to the selection margin.
        /// </summary>
        /// <param name="point">Point (in client coordinates) to be checked.</param>
        /// <returns>True if point belongs to client coordinates.</returns>
        protected bool PointBelongToSelectionArea(Point point)
        {
            if ((!ShowSelectionMargin) && (!ShowLineNumbers || !m_bSelectOnLineNumberClick)) return false;

            Rectangle rectSelectionMargin = (this.ShowSelectionMargin) ?
                (new Rectangle(SelectionMarginOffset, 0, m_iSelectionMarginWidth + 2, ClientSize.Height)) : (Rectangle.Empty);
            Rectangle rectSelectionLineNumber = (this.ShowLineNumbers) ?
                (new Rectangle(SelectionLineNumberOffset, 0, m_lineNumbersWidth + 2, ClientSize.Height)) : (Rectangle.Empty);

            return (rectSelectionMargin.Contains(point) || (rectSelectionLineNumber.Contains(point) && m_bSelectOnLineNumberClick));
        }
        /// <summary>
        /// Attaches event handlers to Move and Resize events.
        /// </summary>
        protected void AttachLayoutEventsToParents()
        {
            m_iLayoutEventAttaches++;
            if (m_iLayoutEventAttaches <= 1)
            {
                Control parentControl = this;
                while (null != parentControl)
                {
                    parentControl.Resize += new EventHandler(ProcessRelayout);
                    parentControl.LocationChanged += new EventHandler(ProcessRelayout);
                    parentControl = parentControl.Parent;
                }
            }
        }
        /// <summary>
        /// Detaches event handlers from Move and Resize events.
        /// </summary>
        protected void DetachLayoutEventsToParents()
        {
            m_iLayoutEventAttaches--;
            if (m_iLayoutEventAttaches <= 0)
            {
                Control parentControl = this;

                while (null != parentControl)
                {
                    parentControl.Resize -= new EventHandler(ProcessRelayout);
                    parentControl.LocationChanged -= new EventHandler(ProcessRelayout);
                    parentControl = parentControl.Parent;
                }
            }
        }
        /// <summary>
        /// Adds guiding tabs at the beginning of lines in the specified range.
        /// </summary>
        /// <param name="iBeginLine">Index of first line to add guiding tab to.</param>
        /// <param name="iEndLine">Index of last line to add guiding tab to.</param>
        protected void AddGuidingTabs(int iBeginLine, int iEndLine)
        {
            if (iBeginLine > this.m_parser.TotalLines) throw new ArgumentOutOfRangeException("iBeginLine");
            if (iEndLine > this.m_parser.TotalLines) throw new ArgumentOutOfRangeException("iEndLine");

            lock (this)
            {
                UndoGroupOpen();
                LockUpdate();

                try
                {
                    for (int i = iBeginLine; i <= iEndLine; i++)
                    {
                        TextInsertInternal(i, 1, DEF_STR_TAB_CHAR, false);
                    }
                }
                finally
                {
                    UndoGroupClose();
                    UnlockUpdate();
                }
            }
        }
        /// <summary>
        /// Removes guiding tabs at the beginning of lines in the specified range.
        /// </summary>
        /// <param name="iBeginLine">Index of first line to remove guiding tab from.</param>
        /// <param name="iEndLine">Index of last line to remove guiding tab from.</param>
        protected void RemoveGuidingTabs(int iBeginLine, int iEndLine)
        {
            if (iBeginLine > this.m_parser.TotalLines) throw new ArgumentOutOfRangeException("iBeginLine");
            if (iEndLine > this.m_parser.TotalLines) throw new ArgumentOutOfRangeException("iEndLine");

            lock (this)
            {
                UndoGroupOpen();
                LockUpdate();

                try
                {
                    for (int i = iBeginLine; i <= iEndLine; i++)
                    {
                        string text = GetLineTextInternal(i);
                        int tabSize = this.TabSize;
                        int toDelete = 0;
                        int toInsert = 0;
                        foreach (char c in text)
                        {
                            if (toDelete >= tabSize)
                            {
                                break;
                            }

                            if (c == ' ')
                            {
                                toDelete++;
                            }
                            else if (c == DEF_TAB_CHAR)
                            {
                                toInsert = toDelete;
                                toDelete++;
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (toDelete > 0)
                        {
                            TextDeleteInternal(i, 1, i, toDelete + 1, false);
                            if (toInsert > 0)
                            {
                                TextInsertInternal(i, 1, new string(' ', toInsert));
                            }
                        }
                    }
                }
                finally
                {
                    UndoGroupClose();
                    UnlockUpdate();
                }
            }
        }
        /// <summary>
        /// Unindents last lexem of block if needed.
        /// </summary>
        protected void UpdateIndentation()
        {
            if (AutoIndentMode.Smart == m_autoIndentMode || EnableSmartInBlockIndent)
            {
                RenderedLine line = (RenderedLine)CurrentLineInstance;
                RenderedLexem lexem = (RenderedLexem)line.FindLexemByColumn(CurrentColumn - 1);

                if (lexem != null)
                {
                    // Indicates whether current lexem is the first nonwhitespace lexem in the line.
                    bool bFirstNonwhitespaceLexem = true;
                    IList lexems = line.LineLexems;
                    for (int i = 0, len = lexems.Count; i < len; i++)
                    {
                        RenderedLexem lex = (RenderedLexem)lexems[i];
                        if (FormatType.Whitespace != lex.Config.Type)
                        {
                            if (lex != lexem) bFirstNonwhitespaceLexem = false;
                            break;
                        }
                    }

                    // If current lexem is first nonwhitespace lexem in the line.
                    if (bFirstNonwhitespaceLexem)
                    {
                        object[] arrStack = line.GetStackByColumn(lexem.Column).ToArray();
                        bool bIndentationLexemFound = false;
                        int iLexIndex = 0;
                        IStackData datum = null;
                        for (; iLexIndex < arrStack.Length; iLexIndex++)
                        {
                            datum = (IStackData)arrStack[iLexIndex];
                            if (datum.Config.Indent)
                            {
                                bIndentationLexemFound = true;
                                break;
                            }
                        }

                        // If current lexem can end the indentation block & it's the end of indentation block.
                        if (bIndentationLexemFound && datum.Config.IsEqualToEnd(lexem.Text))
                        {
                            Point p = m_parser.PhysicalToVirtual(datum.Location);
                            RenderedLine indentLine = (RenderedLine)GetLine(p.Y);

                            int iSpace = 0;
                            int tabLen = m_parser.TabLength;

                            for (int i = 0; i < indentLine.LineLexems.Count; i++)
                            {
                                RenderedLexem lex = (RenderedLexem)indentLine.LineLexems[i];
                                if (lex.Column >= p.X || FormatType.Whitespace != lex.Config.Type)
                                {
                                    break;
                                }

                                if (DEF_STR_TAB_CHAR == lex.Text)
                                {
                                    iSpace += tabLen;
                                }
                                else
                                {
                                    iSpace += lex.Length;
                                }
                            }

                            TextDeleteInternal(CurrentLine, 1, CurrentLine, lexem.Column, false);
                            if (iSpace == 0)
                            {
                                CurrentColumn = lexem.Length + 1;
                            }
                            else
                            {
                                int iTabCount = iSpace / tabLen;
                                int iSpacesCount = iSpace % tabLen;

                                string strWhitespace = new string(DEF_TAB_CHAR, iTabCount);
                                strWhitespace += new string(' ', iSpacesCount);
                                if (!m_bUseTabs)
                                {
                                    strWhitespace = strWhitespace.Replace(DEF_STR_TAB_CHAR, (m_parser.Formats as FormatManager).GetTabString());
                                }

                                TextInsertInternal(CurrentLine, 1, strWhitespace, false, false);
                                CurrentColumn = strWhitespace.Length + lexem.Length + 1;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Gets FindResult structure for regular expression and other parameters.
        /// </summary>
        /// <param name="start">Start position for the search.</param>
        /// <param name="expression">Expression to be found.</param>
        /// <param name="bSearchInCollapsed">Flag, that specifies whether text can be found in collapsed region.</param>
        /// <param name="searchUp">Specifies whether text has to be found above current position.</param>
        /// <param name="textToFind">Text that has to be found.</param>
        /// <returns>FindResult structure with resulting seach data.</returns>
        internal FindResult GetFindResult(IParsePoint start, Regex expression, bool bSearchInCollapsed, bool searchUp, string textToFind)
        {
            bool bContinue = false;

            this.LockSelection();

            FindResult res;
            long firstFoundOffset = -1;

            do
            {
                bContinue = false;

                res = FindRegex(start, expression, bSearchInCollapsed, searchUp);
                if (!res.Result.Success)
                {
                    if (!searchUp && (start.Line != 1 || start.Position != 1) && m_bWrapAroundSearch)
                    {
                        res = FindRegex(Parser.BaseStream.GetParsePoint(1, 1, true), expression, bSearchInCollapsed, searchUp);
                    }
                    else if (searchUp && start.Offset != Parser.BaseStream.Length)
                    {
                        res = FindRegex(Parser.BaseStream.GetParsePoint(Parser.BaseStream.Length), expression, bSearchInCollapsed, searchUp);
                    }
                }

                if (res.Result.Success && bSearchInCollapsed)
                {
                    Parser.EnsureVisibility(res.StartPoint);
                    Parser.EnsureVisibility(res.EndPoint);

                    // If unreachable text is found: user cancels expanding.
                    if (!Parser.IsPointVisible(res.StartPoint) || !Parser.IsPointVisible(res.EndPoint))
                    {
                        if (UnreachableTextFound != null)
                        {
                            UnreachableTextFoundEventArgs args = new UnreachableTextFoundEventArgs(textToFind, res.StartPoint);
                            UnreachableTextFound(this, args);

                            // If user cancelled further search.
                            if (!args.ContinueSearch) return FindResult.Empty;
                        }

                        // Exit if this result was already found in current cycle.
                        if (firstFoundOffset != -1)
                        {
                            if (firstFoundOffset == res.StartPoint.Offset) return FindResult.Empty;
                        }
                        else
                        {
                            firstFoundOffset = res.StartPoint.Offset;
                        }

                        bContinue = true;
                        start = res.EndPoint;
                    }
                }
            }
            while (bContinue);
            this.UnlockSelection();
            return res;
        }
        /// <summary>
        /// Gets information about indent region cursor is currently in.
        /// </summary>
        /// <returns>IndentGuidelineRegionInfo.</returns>
        private IndentGuidelineRegionInfo GetCurrentIndentGuideline()
        {
            return GetCurrentIndentGuideline(false);
        }
        /// <summary>
        /// Gets information about indent region cursor is currently in.
        /// </summary>
        /// <param name="bForced">Indicates whether parsing should be forced.</param>
        /// <returns>IndentGuidelineRegionInfo.</returns>
        private IndentGuidelineRegionInfo GetCurrentIndentGuideline(bool bForced)
        {
            ConfigStack stack = GetCurrentIndentGuidelinedRegionStack();

            IndentGuidelineRegionInfo result = null;

            if (stack != null)
            {
                IStackData stackItem = stack.Pop();
                if (stackItem.Location != null)
                {
                    result = new IndentGuidelineRegionInfo();

                    if (!stackItem.Config.IsComplex)
                        throw new ApplicationException(Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_75);

                    result.PointStart = Parser.GetCoordinatePoint(stackItem.Location, true);
                    result.RectLexemStart = Parser.VirtualToGraphical(new Point(result.PointStart.VirtualColumn, result.PointStart.VirtualLine));
                    RenderedLine lineStart = (RenderedLine)GetLine(result.PointStart.VirtualLine);
                    result.RectLexemStart.Width = ((RenderedLexem)lineStart.FindLexemByColumn(result.PointStart.VirtualColumn)).Width;
                    result.RectLexemStart.Height = lineStart.SubLineHeight[((RenderedLexem)stackItem.Lexem).SubLine];

                    CoordinatePoint coordinatePointLeft = GetNearestParsePointLeft(CurrentLine, CurrentColumn);
                    IParsePoint pointLeft = (coordinatePointLeft != null) ? coordinatePointLeft.PhysicalPoint : null;
                    CollapsableRegion region = (null != stackItem.Lexem) ? stackItem.Lexem.Collapser : null;

                    bool bSameAsCollapser = (null != region && region.Start.Offset == result.PointStart.PhysicalPoint.Offset);
                    long lastPosition = -1;
                    bool bFullParsing = bForced;
                    if (bSameAsCollapser && region.End != null)
                    {
                        lastPosition = (region.End != null) ? region.End.Offset : -1;
                    }
                    else
                    {
                        IEnumerator enumerator = Parser.GetEnumerator(stack, stackItem.Location);
                        ILexemEnumeratorParserInfo infoProvider = enumerator as ILexemEnumeratorParserInfo;
                        int iStackLength = infoProvider.CurrentStack.Count;

                        // Skip region start.
                        if (!enumerator.MoveNext())
                            throw new ApplicationException(Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_74);

                        lastPosition = infoProvider.CurrentPosition;
                        int iLexemsLeft = (bFullParsing) ? (int.MaxValue) : (DEF_MAX_LEXEMS_PARSE_ON_INDENT);

                        while (iLexemsLeft-- > 0 && enumerator.MoveNext() && infoProvider.CurrentStack.Count > iStackLength)
                        {
                            lastPosition = infoProvider.CurrentPosition;
                        }

                        if (iLexemsLeft <= 0 || infoProvider.CurrentStack.Count > iStackLength || enumerator.Current == null)
                        {
                            lastPosition = -1;
                        }
                    }

                    result.RectLexemEnd = result.RectLexemStart;
                    result.PointEnd = result.PointStart;

                    if (lastPosition >= 0)
                    {
                        result.PointEnd = Parser.GetCoordinatePoint(Parser.BaseStream.GetParsePoint(lastPosition), true);
                    }
                    else
                    {
                        result.PointEnd = null;
                    }

                    UpdateIndentationGuideLineEnd(result);
                }
            }

            return result;
        }
        /// <summary>
        /// Updates information regarding ending of the indentation guideline.
        /// </summary>
        /// <param name="info">Indentation guideline information to be updated.</param>
        private void UpdateIndentationGuideLineEnd(IndentGuidelineRegionInfo info)
        {
            if (null != info && null != info.PointEnd)
            {
                Point pointEnd = Parser.PhysicalToVirtual(info.PointEnd.PhysicalPoint);
                info.RectLexemEnd = Parser.VirtualToGraphical(pointEnd);

                RenderedLine lineEnd = (RenderedLine)GetLine(pointEnd.Y);

                RenderedLexem lexemEnd = lineEnd.FindLexemByColumn(pointEnd.X) as RenderedLexem;

                if (null != lexemEnd)
                {
                    info.RectLexemEnd.Width = lexemEnd.Width;
                    info.RectLexemEnd.Height = lineEnd.SubLineHeight[lexemEnd.SubLine];
                }
            }
            else if (info != null)
            {
                info.RectLexemEnd.X = info.RectLexemStart.X;
                info.RectLexemEnd.Y = -1;
                info.RectLexemEnd.Width = 0;
                info.RectLexemEnd.Height = 0;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessRelayout(object sender, EventArgs e)
        {
            if (ContextChoiceOn)
            {
                m_controllerContextChoice.Close();
            }
            if (ContextPromptOn)
            {
                m_contextPrompt.Close();
            }
        }
        /// <summary>
        /// Closes all visible intellisense windows.
        /// </summary>
        private void CloseIntellisense()
        {
            if (ContextChoiceOn)
            {
                ContextChoice.Close();
            }
            if (ContextPromptOn)
            {
                m_contextPrompt.Close();
            }
            if (null != m_tip && m_tip.Visible)
            {
                m_tip.Visible = false;
            }
            if (null != m_bookmarksTooltip && m_bookmarksTooltip.Visible)
            {
                m_bookmarksTooltip.Visible = false;
            }
            m_codeSnippetsPopupController.Close();
        }
        /// <summary>
        /// Gets visual column of current cursor position.
        /// </summary>
        /// <returns>Number of visual column of current cursor position.</returns>
        private int GetCurrentVisualColumn()
        {
            int result = CurrentColumn;
            RenderedLine line = m_parser.GetLine(CurrentLine) as RenderedLine;

            if (line != null)
            {
                IList lexems = line.LineLexems;
                IRenderedLexem lexem;
                for (int i = 0, len = lexems.Count; i < len; i++)
                {
                    lexem = (IRenderedLexem)lexems[i];
                    if (lexem.Column >= CurrentColumn)
                    {
                        break;
                    }
                    if (DEF_STR_TAB_CHAR == lexem.Text)
                    {
                        result += m_parser.TabLength - 1;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Sets current virtual column of cursor position.
        /// </summary>
        /// <param name="col">Number of virtual column.</param>
        private void SetCurrentVisualColumn(int col)
        {
            CheckControlState();
            RenderedLine line = m_parser.GetLine(CurrentLine) as RenderedLine;

            if (line != null)
            {
                IList lexems = line.LineLexems;
                IRenderedLexem lexem = null;
                int curCol = 0;

                for (int i = 0, len = lexems.Count; i < len; i++)
                {
                    lexem = (IRenderedLexem)lexems[i];
                    int length = (DEF_STR_TAB_CHAR == lexem.Text) ? (m_parser.TabLength) : (lexem.Length);

                    if (curCol + length < col)
                    {
                        curCol += length;
                        continue;
                    }

                    if (DEF_STR_TAB_CHAR == lexem.Text)
                    {
                        CurrentColumn = lexem.Column;
                    }
                    else
                    {
                        CurrentColumn = lexem.Column + (col - curCol - 1);
                    }

                    return;
                }

                if (null != lexem)
                {
                    CurrentColumn = lexem.Column + lexem.Length + (col - curCol - 1);
                }
                else
                {
                    CurrentColumn = 1;
                }
            }
        }
        /// <summary>
        /// Sets new line style to the underlying stream.
        /// </summary>
        /// <param name="style">Style of new line.</param>
        private void SetNewLineStyle(NewLineStyle style)
        {
            m_wrapper.NewLineStyle = style;
            IConfigLanguage langCurrent = Language;

            MemoryStream stream = new MemoryStream();
            SaveToStream(stream);

            bool b;
            Stream streamConverted = ConvertStream(stream, RegexTokenizer.GetNewLineString(style), out b);
            if (null != streamConverted)
            {
                StreamReader reader = new StreamReader(streamConverted);
                string s = reader.ReadToEnd();
                streamConverted.Position = 0;
            }

            if (streamConverted != null)
            {
                Point currentCursorPosition = CurrentPosition;
                DiscardChanges();
                LoadStream(streamConverted, langCurrent);
                CurrentPosition = currentCursorPosition;
            }
        }
        /// <summary>
        /// Gets lexem that goes before given lexem and is not whitespqace or new line mark.
        /// </summary>
        /// <param name="lineIndex">Index of line lexem is situated in.</param>
        /// <param name="lexem">Lexem to find previous one.</param>
        /// <returns></returns>
        private IRenderedLexem GetPreviousNotWhitespaceLexem(int lineIndex, IRenderedLexem lexem)
        {
            if (lineIndex < 0) throw new ArgumentOutOfRangeException("lineIndex");
            if (null == lexem) throw new ArgumentNullException("lexem");

            RenderedLine line = (RenderedLine)m_parser.GetLine(lineIndex);

            if (null == line) return null;

            IRenderedLexem prevLexem = null;
            int col = lexem.Column;

            if (1 != col)
            {
                prevLexem = (IRenderedLexem)line.FindLexemByColumn(col - 1);
            }
            else
            {
                IList prevLineLexems = null;

                if (0 == lineIndex)
                {
                    return null;
                }
                else
                {
                    do
                    {
                        lineIndex--;

                        if (0 == lineIndex) return null;

                        prevLineLexems = ((RenderedLine)m_parser.GetLine(lineIndex)).LineLexems;
                    }
                    while (0 == prevLineLexems.Count);
                }

                prevLexem = (IRenderedLexem)prevLineLexems[prevLineLexems.Count - 1];
            }

            if (null == prevLexem) return null;

            string strRegex = DEF_CONVERT_REGEX;
            Regex searchRegex = new Regex(strRegex, RegexOptions.Compiled);
            Match match = searchRegex.Match(prevLexem.Text);

            if (FormatType.Whitespace != prevLexem.Config.Type && !match.Success) return prevLexem;
            else return GetPreviousNotWhitespaceLexem(lineIndex, prevLexem);
        }
        /// <summary>
        /// Updates values of parser's ParagraphOffset and WrappedLinesOffset properties.
        /// </summary>
        private void UpdateParserOffsetsInfo()
        {
            m_parser.ParagraphOffset = m_iParagraphOffset;
            m_parser.WrappedLinesOffset = m_iWrappedLinesOffset;
            if (m_bMarkWrappedLines)
            {
                if (m_parser.WrappedLinesOffset < this.WrappedLinesMarkingImage.Width)
                    m_parser.WrappedLinesOffset = this.WrappedLinesMarkingImage.Width;
            }
        }
        /// <summary>
        /// Checks whether cursor is situated over collapsed region lexem.
        /// </summary>
        /// <returns>True if cursor is situated over collapsed region; otherwise false.</returns>
        private bool CursorOverCollapsedRegion()
        {
            IRenderedLexem lex = GetLexemUnderCursor();
            if (lex != null && lex.Config.Type == FormatType.CollapsedText)
            {
                return true;
            }

            lex = (IRenderedLexem)CurrentLineInstance.FindLexemByColumn(this.CurrentColumn - 1);
            if (lex != null && lex.Config.Type == FormatType.CollapsedText)
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// Sets mouse cursor to waiting state.
        /// </summary>
        /// <param name="operation">ILongOperation instance that caused the cursor change.</param>
        private void SetWaitCursor(object operation)
        {
            ILongOperation op = (ILongOperation)operation;

            if (op.IsRunning && this.TopLevelControl == Form.ActiveForm)
            {
                WinAPI.AttachThreadInput((uint)Thread.CurrentThread.ManagedThreadId, m_mainThreadId, true);
                Cursor.Current = Cursors.WaitCursor;
            }

            threadingTimer.Dispose();
            threadingTimer = null;
        }
        /// <summary>
        /// Processes work with autoreplace triggers.
        /// </summary>
        /// <param name="c">Inserted char.</param>
        private void ProcessAutoReplace(char c)
        {
            if (m_bUseAutoreplaceTriggers)
            {
                IConfigLexem curConfig = ((IStackData)GetCurrentStack().Peek()).Config;

                if (curConfig.AllowTriggers)
                {
                    if (this.Language.TriggersActivatorsString.IndexOf(c) != -1)
                    {
                        IRenderedLexem lex = GetLexemUnderCursor();

                        if (lex != null)
                        {
                            string lexText = lex.Text.Substring(0, this.CurrentColumn - lex.Column);

                            foreach (AutoReplaceTrigger trigger in this.Language.AutoReplaceTriggers)
                            {
                                if (trigger.From == lexText)
                                {
                                    TextDeleteInternal(this.CurrentLine, lex.Column, this.CurrentLine, lex.Column + lexText.Length, false);
                                    TextInsertInternal(this.CurrentLine, lex.Column, trigger.To, true);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Updates height of surrounding lines.
        /// </summary>
        /// <param name="iLine">Line, neighbours of which must be measured.</param>
        private void UpdateLinesMeasuring(int iLine)
        {
            if (iLine >= 1 || iLine <= m_parser.TotalLines)
            {
                int iLineFirst = Math.Max(1, iLine - DEF_LINES_PRERENDER);
                int iLineLast = Math.Min(Parser.TotalLines, iLine + DEF_LINES_PRERENDER);

                for (int indexPrerender = iLineFirst; indexPrerender <= iLineLast; indexPrerender++)
                {
                    RenderedLine linePrerender = Parser.GetLine(indexPrerender) as RenderedLine;
                    Parser.MeasureLine(linePrerender);
                }

                FixLineRenderingPositions();
            }
        }
        /// <summary>
        /// Gets visual location by given virtual column. Visual column counts tabs as several symbols.
        /// </summary>
        /// <param name="lineIndex">Index of line.</param>
        /// <param name="virtualColumn">Virtual column.</param>
        /// <returns>Visual location.</returns>
        private VisualLocation GetVisualLocation(int lineIndex, int virtualColumn)
        {
            VisualLocation result = new VisualLocation(lineIndex, 0, 0);
            result.Offset = 0;
            RenderedLine line = (RenderedLine)m_parser.GetLine(lineIndex);
            IRenderedLexem lexAtCol = (IRenderedLexem)line.FindLexemByColumn(virtualColumn);
            result.SubLine = (lexAtCol == null) ? (line.SubLinesCount - 1) : (lexAtCol.SubLine);

            if (line.LineLexems.Count == 0)
            {
                result.Offset = virtualColumn + GetNumberOfCharsInWidth(m_iParagraphOffset);
            }

            foreach (IRenderedLexem lexem in line.LineLexems)
            {
                if (lexem.SubLine >= result.SubLine)
                {
                    int subLineOffset = (result.SubLine == 0) ? (m_iParagraphOffset) : (m_parser.WrappedLinesOffset);
                    if (result.Offset == 0)
                    {
                        result.Offset = virtualColumn - lexem.Column + 1 + GetNumberOfCharsInWidth(subLineOffset);
                    }

                    if (lexem.Column >= virtualColumn)
                    {
                        break;
                    }

                    if (lexem.Text == DEF_STR_TAB_CHAR)
                    {
                        result.Offset += m_parser.TabLength - 1;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Gets virtual column by given visual column. Virtual column counts tabs as one symbol.
        /// </summary>
        /// <param name="lineIndex">Index of line.</param>
        /// <param name="subLine"></param>
        /// <param name="visualColumn">Visual column.</param>
        /// <returns>Virtual column.</returns>
        private int GetVirtualColumn(int lineIndex, int subLine, int visualColumn)
        {
            int result = 0;
            RenderedLine line = (RenderedLine)m_parser.GetLine(lineIndex);

            int lineOffset = (subLine == 0) ? (m_iParagraphOffset) : (m_parser.WrappedLinesOffset);
            int realColumn = visualColumn - GetNumberOfCharsInWidth(lineOffset);
            if (realColumn < 1)
            {
                realColumn = 1;
            }

            if (line.LineLexems.Count == 0)
            {
                result = realColumn;
            }

            int sublineLen = 1;
            foreach (IRenderedLexem lexem in line.LineLexems)
            {
                if (lexem.SubLine == subLine)
                {
                    if (result == 0)
                    {
                        result = lexem.Column + realColumn - 1;
                    }

                    if (sublineLen >= realColumn)
                    {
                        break;
                    }
                    sublineLen += lexem.Length;

                    if (lexem.Text == DEF_STR_TAB_CHAR)
                    {
                        result -= m_parser.TabLength - 1;
                        realColumn -= m_parser.TabLength - 1;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Resets Background brush.
        /// </summary>
        private void ResetBackgroundColor()
        {
            this.BackgroundColor = BrushInfo.Empty;
        }
        /// <summary>
        /// Parses some lines in stream.
        /// </summary>
        private void IdleStreamProcess()
        {
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        /// <summary>
        /// Truncates and unindents text of the tooltip.
        /// </summary>
        /// <param name="text">Text of the tooltip.</param>
        /// <returns>String with truncated and unindented text of the tooltip.</returns>
        private string TruncateAndUnindentTooltipText(string text)
        {
            if (text == null) throw new ArgumentNullException("text");

            text = text.Replace(DEF_STR_TAB_CHAR, (m_parser.Formats as FormatManager).GetTabString());

            StringBuilder result = new StringBuilder(32);
            StringReader reader = new StringReader(text);
            string strCurrentLine;
            int iLineIndex = 1;
            int iMinimumSpaces = -1;

            // Calculate minimum spaces count and put allowed count of lines to the result.
            while ((strCurrentLine = reader.ReadLine()) != null &&
                iLineIndex++ < DEF_TOOLTIP_MAX_LINES)
            {
                int iFirstNonSpace = -1;
                int iLineLength = strCurrentLine.Length;

                while (++iFirstNonSpace < iLineLength)
                {
                    if (strCurrentLine[iFirstNonSpace] != ' ') break;
                }

                if (iFirstNonSpace < iLineLength)
                {
                    iMinimumSpaces = (iMinimumSpaces >= 0) ?
                        Math.Min(iMinimumSpaces, iFirstNonSpace) : iFirstNonSpace;
                }

                if (iLineIndex == DEF_TOOLTIP_MAX_LINES)
                {
                    strCurrentLine = ((iFirstNonSpace > 0) ?
                        new string(' ', iFirstNonSpace) : string.Empty);

                    strCurrentLine += "...";
                }

                result.Append(strCurrentLine);
                result.Append(STR_NEW_LINE);
            }

            if (iMinimumSpaces > 0)
            {
                reader = new StringReader(result.ToString());
                result.Length = 0;

                while ((strCurrentLine = reader.ReadLine()) != null)
                {
                    result.Append(strCurrentLine.Remove(0,
                        Math.Min(iMinimumSpaces, strCurrentLine.Length)));
                    result.Append(STR_NEW_LINE);
                }
            }

            return result.ToString();
        }
        /// <summary>
        /// Calculates text, header and footer areas of the page for printing and prints header and footer.
        /// </summary>
        /// <param name="marginBounds">Page margin bounds.</param>
        /// <param name="pageBounds">Page bounds.</param>
        /// <param name="g">Graphics, header and footer should be drawn on.</param>
        /// <param name="rectMargins">Returns text area margins.</param>
        /// <param name="rectHeader">Returns header area bounds.</param>
        /// <param name="rectFooter">Returns footer area bounds.</param>
        private void PrintHeaderFooter(
            Rectangle marginBounds, Rectangle pageBounds, Graphics g, out Rectangle rectMargins, out Rectangle rectHeader, out Rectangle rectFooter)
        {
            int entireHeinght = pageBounds.Height;
            int iHeaderHeight = (int)(DEF_HEADER_HEIGHT / 100f * entireHeinght);
            int iFooterHeight = (int)(DEF_FOOTER_HEIGHT / 100f * entireHeinght);
            int iOffset = (int)(DEF_HEADER_FOOTER_TEXT_OFFSET / 100f * entireHeinght);
            rectMargins = marginBounds;

            if (m_bPrintHeaderFooterOutsideMargins)
            {
                rectHeader = new Rectangle(marginBounds.X, marginBounds.Y - iHeaderHeight - iOffset, marginBounds.Width, iHeaderHeight);
                rectFooter = new Rectangle(marginBounds.X, marginBounds.Bottom + iOffset, marginBounds.Width, iFooterHeight);
            }
            else
            {
                rectHeader = new Rectangle(marginBounds.X, marginBounds.Y, marginBounds.Width, iHeaderHeight);
                rectFooter = new Rectangle(marginBounds.X, marginBounds.Bottom - iFooterHeight, marginBounds.Width, iFooterHeight);
            }

            if (m_bPrintHeaderFooter)
            {
                iHeaderHeight = OnPrintHeader(g, rectHeader);
                iFooterHeight = OnPrintFooter(g, rectFooter);
            }
            else
            {
                iHeaderHeight = 0;
                iFooterHeight = 0;
            }

            if (m_bPrintHeaderFooterOutsideMargins)
            {
                rectHeader.Height = iHeaderHeight;
                rectFooter.Y = rectFooter.Bottom - iFooterHeight;
                rectFooter.Height = iFooterHeight;
            }
            else
            {
                rectHeader.Height = iHeaderHeight;
                rectFooter.Y = rectMargins.Bottom - iFooterHeight;
                rectFooter.Height = iFooterHeight;
            }

            rectMargins.Y = rectHeader.Bottom;
            if (iHeaderHeight > 0)
            {
                rectMargins.Y += iOffset;
            }
            rectMargins.Height = rectFooter.Top - rectMargins.Top;
            if (iFooterHeight > 0)
            {
                rectMargins.Height -= iOffset;
            }
        }
        /// <summary>
        /// Remembers that block selection was copied to clipboard. It will be pasted respectively.
        /// </summary>
        /// <param name="text">Text that was copied.</param>
        private void SetBlockClipboardData(string text)
        {
            if (EnableMD5)
            {
                if (_md5==null)
                _md5 = MD5.Create();
                m_blockHashcode = _md5.ComputeHash(Encoding.UTF8.GetBytes(text));
            }
        }
        /// <summary>
        /// Gets number of characters fitting given width. Uses format for Whitespace.
        /// </summary>
        /// <param name="width">Width to get number of chars for.</param>
        /// <returns>number of characters fitting given width.</returns>
        private int GetNumberOfCharsInWidth(int width)
        {
            float charWidth = (m_parser.Formats[FormatType.Whitespace] as Format).MeasureText(
                GraphicsUtils.DefaultGraphics, " ", true, this.UseNativeDrawing, this.SpaceBetweenLines).Width;
            return Convert.ToInt32(width / charWidth);
        }
        /// <summary>
        /// Calculates position of word wrap column position.
        /// </summary>
        /// <param name="g">Graphics object to use while measuring lines.</param>
        private void CalculateWordWrapColumnPos(Graphics g)
        {
            string measuringString = new string('$', m_iWordWrapColumn);
            m_wordWrapColumnPos =
                (int)GraphicsUtils.MeasureString(measuringString, m_wordWrapColumnMeasuringFont, false, g, string.Empty, this.UseNativeDrawing).Width;
        }
        /// <summary>
        /// Returns scale coefficient for different DPIs.
        /// </summary>
        /// <param name="g1"></param>
        /// <param name="g2"></param>
        /// <returns></returns>
        private float GetDPIScale(Graphics g1, Graphics g2)
        {
            float result = g1.DpiX / g2.DpiX;

            // Value 6.25 isn't correct for DPIs 96 and 600 for unknown reasons. This approach possibly needs improvement in the future.
            if (g1.DpiX == 600 && g2.DpiX == 96)
            {
                result = 6;
            } // the same reason
            else if (g1.DpiX == 300 && g2.DpiX == 96)
            {
                result = 3;
            }

            return result;
        }
        /// <summary>
        /// Applies given format in given layer to each line separately.
        /// </summary>
        /// <param>Layer to apply format to.</param>
        /// <param name="top">Start point of range.</param>
        /// <param name="bottom">End point of range.</param>
        /// <param name="format">Format to apply.</param>
        /// <param name="layer">layer.</param>
        private void ApplyFormatToEachLine(IDynamicFormatsLayer layer, CoordinatePoint top, CoordinatePoint bottom, ISnippetFormat format)
        {
            if (top.VirtualLine == bottom.VirtualLine)
            {
                CoordinatePoint p1 = m_parser.GetCoordinatePoint(top.VirtualLine, top.VirtualColumn);
                CoordinatePoint p2 = m_parser.GetCoordinatePoint(bottom.VirtualLine, bottom.VirtualColumn);
                layer.Add(p1, p2, format);
            }
            else
            {
                CoordinatePoint p1 = m_parser.GetCoordinatePoint(top.VirtualLine, top.VirtualColumn);
                CoordinatePoint p2 = m_parser.GetCoordinatePoint(top.VirtualLine, GetLine(top.VirtualLine).LineLength + 1);
                layer.Add(p1, p2, format);

                for (int i = top.VirtualLine + 1; i < bottom.VirtualLine; i++)
                {
                    ILexemLine line = GetLine(i);
                    layer.Add(m_parser.GetCoordinatePoint(i, 1), m_parser.GetCoordinatePoint(i, line.LineLength + 1), format);
                }

                layer.Add(m_parser.GetCoordinatePoint(GetLine(bottom.VirtualLine).LineStartPoint),
                    m_parser.GetCoordinatePoint(bottom.VirtualLine, bottom.VirtualColumn), format);
            }
        }
        /// <summary>
        /// Handles deletion of selected text on drag drop.
        /// /// </summary>
        private void DeteleSelectedTextOnDrop()
        {
            foreach (TextRange range in m_rangeDragging.Ranges)
            {
                if (CurrentLine == range.Top.VirtualLine && range.Top.VirtualLine == range.Bottom.VirtualLine
                       && CurrentColumn > range.Top.VirtualColumn)
                {
                    CurrentPosition = new Point(CurrentColumn - (range.Bottom.VirtualColumn - range.Top.VirtualColumn), CurrentLine);
                }
                TextDeleteInternal(range.Top.VirtualLine, range.Top.VirtualColumn, range.Bottom.VirtualLine, range.Bottom.VirtualColumn, false);
            }
        }
        #endregion

        #region Event Raisers
        /// <summary>
        /// Raises OperationStarted event.
        /// </summary>
        /// <param name="operation">Operation, that is started.</param>
        void ILongOperationControllerInternal.RaiseOperationStart(ILongOperation operation)
        {
            ProcessOperationStart(operation);
        }
        /// <summary>
        /// Raises OperationStopped event.
        /// </summary>
        /// <param name="operation">Operation, that is stopped.</param>
        void ILongOperationControllerInternal.RaiseOperationEnd(ILongOperation operation)
        {
            ProcessOperationEnd(operation);
        }
        /// <summary>
        /// Raises InsertModeChanged event
        /// </summary>
        protected void RaiseInsertModeChangedEvent()
        {
            if (InsertModeChanged != null)
            {
                InsertModeChanged(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Raises CursorPositionChanged event
        /// </summary>
        protected void RaiseCursorPositionChangedEvent()
        {
            if (CursorPositionChanged != null)
            {
                CursorPositionChanged(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Raises CanUndoRedoChanged event.
        /// </summary>
        protected void RaiseUpdateChangedStateEvent()
        {
            if (CanUndoRedoChanged != null)
            {
                CanUndoRedoChanged(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Raises PaintUserMargin event.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="clip"></param>
        protected void RaisePaintUserMarginEvent(Graphics g, Rectangle clip)
        {
            if (PaintUserMargin != null)
            {
                PaintUserMargin(this, new PaintEventArgs(g, clip));
            }
        }
        /// <summary>
        /// Raises ChangingStream event.
        /// </summary>
        protected virtual bool RaiseChangingStreamEvent()
        {
            if (m_parser != null && ChangingStream != null)
            {
                bool processed = true;
                ChangingStream(ref processed);
                return processed;
            }

            return true;
        }
        /// <summary>
        /// Raises ReadOnlyChanged event.
        /// </summary>
        protected void RaiseReadOnlyChangedEvent()
        {
            if (ReadOnlyChanged != null)
            {
                ReadOnlyChanged(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Raises ConfigurationChanged event.
        /// </summary>
        protected void RaiseConfigurationChangedEvent()
        {
            if (ConfigurationChanged != null)
            {
                ConfigurationChanged(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Raises LanguageChanged event.
        /// </summary>
        protected void RaiseLanguageChangedEvent()
        {
            if (LanguageChanged != null)
            {
                LanguageChanged(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Raises SelectionChanged event.
        /// </summary>
        protected virtual void OnSelectionChanged()
        {
            if (m_iSelectionLockCount <= 0)
            {
                if (SelectionChanged != null)
                {
                    string strSelectedText = SelectedText;

                    if (m_strOldSelectedText != strSelectedText)
                    {
                        m_strOldSelectedText = strSelectedText;
                        SelectionChanged(this, EventArgs.Empty);
                    }
                }
            }
        }
        #endregion

        #region Navigation & Actions & Commands
        /// <summary>
        /// Moves cursor left, if possible.
        /// </summary>
        [Command("Navigation.MoveLeft")
         , KeysBinding(Keys.Left)
         , KeysBinding(Keys.Left | Keys.Shift)
         , KeysBinding(Keys.Left | Keys.Shift | Keys.Alt)]
        public virtual void MoveLeft()
        {
            lock (this)
            {
                if (this.CurrentColumn > 1)
                {
                    RenderedLexem lexem = CurrentLineInstanceInternal.FindLexemByColumn(CurrentColumn - 1) as RenderedLexem;

                    if (lexem != null && lexem.Config.Format.UseCustomControl)
                    {
                        this.CurrentColumn = lexem.Column;
                    }
                    else
                    {
                        this.CurrentColumn--;
                    }
                }
                else if (this.CurrentColumn == 1 && this.CurrentLine == 1)
                {
                    this.CurrentColumn--;
                }
                else if (this.CurrentLine > 1)
                {
                    this.CurrentLine--;
                    MoveToLineEnd();
                }
            }
        }
        /// <summary>
        /// Move cursor up, if possible.
        /// </summary>
        [Command("Navigation.MoveUp")
         , KeysBinding(Keys.Up)
         , KeysBinding(Keys.Up | Keys.Shift)
         , KeysBinding(Keys.Up | Keys.Shift | Keys.Alt)]
        public virtual void MoveUp()
        {
            lock (this)
            {
                RenderedLine line = InternalGetLine(CurrentLine, false);
                IRenderedLexem lex = line.FindLexemByColumn(CurrentColumn) as IRenderedLexem;

                if (CurrentLine > 1 || (lex != null && lex.SubLine > 0) || (lex == null && line.SubLinesCount > 1))
                {
                    Point oldPoint = m_CursorManager.CursorGraphicalCoordinates.LeftTop;
                    int oldCursorX = m_lastCursorX;

                    if (!VirtualSpaceMode)
                    {
                        oldPoint.X = m_lastCursorX;
                    }
                    oldPoint.X += 2;
                    bool bProcessed = false;

                    // If word-wrap is enabled, then we have to move inside current line
                    if (line.SubLinesCount > 1)
                    {
                        if ((lex != null && lex.SubLine > 0) || (lex == null))
                        {
                            float lineHeight = line.SubLineHeight[(lex != null) ? lex.SubLine - 1 : line.SubLineHeight.Length - 2];
                            oldPoint.Y -= (int)Math.Round(lineHeight / 2);
                            m_CursorManager.CursorGraphicalCoordinates.LeftTop = oldPoint;
                            bProcessed = true;
                        }
                    }

                    if (!bProcessed)
                    {
                        // Check upper line for sub-lines
                        RenderedLine prevLine = InternalGetLine(CurrentLine - 1, false);
                        float lineHeight = prevLine.SubLineHeight[prevLine.SubLineHeight.Length - 1];
                        float prevY = prevLine.Y;
                        if (prevLine.SubLinesCount > 1)
                        {
                            for (int i = 0, count = prevLine.SubLinesCount; i < count - 1; i++)
                            {
                                prevY += prevLine.SubLineHeight[i];
                            }
                        }

                        oldPoint.Y += (int)Math.Round(lineHeight / 2 - (line.Y - prevY));
                        m_CursorManager.CursorGraphicalCoordinates.LeftTop = oldPoint;
                    }

                    m_lastCursorX = oldCursorX;
                }
            }
        }
        /// <summary>
        /// Moves cursor down if possible.
        /// </summary>
        [Command("Navigation.MoveDown")
         , KeysBinding(Keys.Down)
         , KeysBinding(Keys.Down | Keys.Shift)
         , KeysBinding(Keys.Down | Keys.Shift | Keys.Alt)]
        public virtual void MoveDown()
        {
            lock (this)
            {
                RenderedLine line = m_parser.GetLine(CurrentLine) as RenderedLine;
                IRenderedLexem lex = line.FindLexemByColumn(CurrentColumn) as IRenderedLexem;

                if (CurrentLine < m_parser.TotalLines || (lex != null && lex.SubLine < line.SubLineHeight.Length - 1))
                {
                    Point oldPoint = m_CursorManager.CursorGraphicalCoordinates.LeftTop;
                    int oldCursorX = m_lastCursorX;
                    if (!VirtualSpaceMode)
                    {
                        oldPoint.X = m_lastCursorX;
                    }

                    oldPoint.X += 2;
                    bool bProcessed = false;

                    // If word-wrap is enabled, then we have to move inside current line
                    if (line.SubLinesCount > 1)
                    {
                        if (lex != null && lex.SubLine < line.SubLinesCount - 1)
                        {
                            float lineHeight = line.SubLineHeight[lex.SubLine + 1];
                            oldPoint.Y += (int)Math.Round(lineHeight / 2 + line.SubLineHeight[lex.SubLine]);

                            m_CursorManager.CursorGraphicalCoordinates.LeftTop = oldPoint;
                            bProcessed = true;
                        }
                    }

                    if (!bProcessed)
                    {
                        // Check upper line for sub-lines
                        RenderedLine nextLine = m_parser.GetLine(CurrentLine + 1) as RenderedLine;
                        float lineHeight = nextLine.SubLineHeight[0];

                        float y = line.Y;
                        if (line.SubLinesCount > 1)
                        {
                            for (int i = 0; i < line.SubLinesCount - 1; i++)
                            {
                                y += line.SubLineHeight[i];
                            }
                        }
                        oldPoint.Y += (int)Math.Round(lineHeight / 2 + nextLine.Y - y);
                        m_CursorManager.CursorGraphicalCoordinates.LeftTop = oldPoint;
                    }

                    m_lastCursorX = oldCursorX;
                }
            }
        }
        /// <summary>
        /// Moves cursor right if possible.
        /// </summary>
        [Command("Navigation.MoveRight")
         , KeysBinding(Keys.Right)
         , KeysBinding(Keys.Right | Keys.Shift)
         , KeysBinding(Keys.Right | Keys.Shift | Keys.Alt)]
        public virtual void MoveRight()
        {
            lock (this)
            {
                RenderedLine currentLine = CurrentLineInstanceInternal;
                RenderedLexem lexem = currentLine.FindLexemByColumn(CurrentColumn + 1) as RenderedLexem;

                if (lexem == null)
                {
                    if ((CurrentColumn + 1) == currentLine.LineLength)
                    {
                        CurrentColumn++;
                    }
                    else if (CurrentLine < Parser.TotalLines)
                    {
                        CurrentColumn = 1;
                        CurrentLine++;
                    }
                    else if (CurrentLine == Parser.TotalLines)
                    {
                        CurrentLine++;
                    }
                }
                else if (lexem.Config.Format.UseCustomControl)
                {
                    CurrentColumn = lexem.Column + lexem.Length;
                }
                else
                {
                    CurrentColumn++;
                }
            }
        }
        /// <summary>
        /// Moves caret one page up.
        /// </summary>
        [Command("Navigation.MovePageUp")
         , KeysBinding(Keys.PageUp)
         , KeysBinding(Keys.PageUp | Keys.Shift)]
        public virtual void MovePageUp()
        {
            lock (this)
            {
                int curTopY = -this.AutoScrollPosition.Y;
                RenderedLine curTopLine = m_parser.GetLineByY(curTopY);
                int cursorOffset = this.CurrentLine - curTopLine.LineIndex;
                int height = this.ClientSize.Height - this.ScrollOffsetTop - this.ScrollOffsetBottom;
                int newY = curTopY - height + (int)curTopLine.Height;

                if (newY < 0)
                {
                    RenderedLine line = m_parser.GetLineByY(-newY);
                    if (line != null)
                    {
                        cursorOffset = Math.Max(cursorOffset - line.LineIndex, 0);
                    }
                    else
                    {
                        cursorOffset = 0;
                    }
                    newY = 0;
                }

                RenderedLine newTopLine = m_parser.GetLineByY(newY);
                this.AutoScrollPosition = new Point(this.AutoScrollPosition.X, (int)newTopLine.Y);
                this.CurrentLine = newTopLine.LineIndex + cursorOffset;
            }
        }
        /// <summary>
        /// Moves caret one page down.
        /// </summary>
        [Command("Navigation.MovePageDown")
         , KeysBinding(Keys.PageDown)
         , KeysBinding(Keys.PageDown | Keys.Shift)]
        public virtual void MovePageDown()
        {
            lock (this)
            {
                int curTopY = -this.AutoScrollPosition.Y;
                RenderedLine curTopLine = m_parser.GetLineByY(curTopY);
                int cursorOffset = this.CurrentLine - curTopLine.LineIndex;
                int height = this.ClientSize.Height - this.ScrollOffsetTop - this.ScrollOffsetBottom;
                int newY = curTopY + height;

                RenderedLine newTopLine = m_parser.GetLineByY(newY);
                if (newTopLine == null)
                {
                    newTopLine = (RenderedLine)m_parser.GetLine(m_parser.TotalLines);
                    cursorOffset = 0;
                }

                this.AutoScrollPosition = new Point(this.AutoScrollPosition.X, (int)newTopLine.Y);
                this.CurrentLine = newTopLine.LineIndex;
            }
        }
        /// <summary>
        /// Moves caret to the end of line
        /// </summary>
        [Command("Navigation.MoveToLineEnd")
         , KeysBinding(Keys.End)
         , KeysBinding(Keys.End | Keys.Shift)]
        public virtual void MoveToLineEnd()
        {
            lock (this)
            {
                CurrentColumn = GetLineLength(CurrentLine);
            }
        }
        /// <summary>
        /// Moves caret to the beginning of line. First whitespaces will be skipped.
        /// </summary>
        [Command("Navigation.MoveToLineStart")
         , KeysBinding(Keys.Home)
         , KeysBinding(Keys.Home | Keys.Shift)]
        public virtual void MoveToLineStart()
        {
            lock (this)
            {
                RenderedLine line = m_parser.GetLine(CurrentLine) as RenderedLine;
                IList lexems = line.LineLexems;
                int firstColumn = 1;

                foreach (IRenderedLexem lexem in lexems)
                {
                    firstColumn = lexem.Column;
                    if ((lexem.Config.Format as Format).Name != "Whitespace")
                    {
                        break;
                    }
                }

                if (CurrentColumn != firstColumn)
                {
                    CurrentColumn = firstColumn;
                }
                else
                {
                    CurrentColumn = 1;
                }
            }
        }
        /// <summary>
        /// Moves caret to left by one word, or to the beginning of the current.
        /// </summary>
        [Command("Navigation.MoveLeftWord")
         , KeysBinding(Keys.Control | Keys.Left)
         , KeysBinding(Keys.Control | Keys.Left | Keys.Shift)]
        public virtual void MoveLeftWord()
        {
            lock (this)
            {
                if (CurrentColumn == 1)
                {
                    if (CurrentLine == 1) return;

                    CurrentLine--;
                    CurrentColumn = CurrentLineInstance.LineLength + 1;
                }

                RenderedLine line = m_parser.GetLine(CurrentLine) as RenderedLine;
                IRenderedLexem lexem = line.FindLexemByColumn(CurrentColumn) as IRenderedLexem;
                if (lexem != null && CurrentColumn == lexem.Column)
                {
                    IList lexems = line.LineLexems;
                    int index = lexems.IndexOf(lexem) - 1;

                    while (index >= 0)
                    {
                        lexem = lexems[index] as IRenderedLexem;
                        index--;

                        if ((lexem.Config.Format as Format).Name != "Whitespace")
                        {
                            break;
                        }
                    }
                }
                else if (lexem == null)
                {
                    IList lexems = line.LineLexems;

                    if (lexems.Count > 0)
                    {
                        lexem = lexems[lexems.Count - 1] as IRenderedLexem;
                    }
                    else
                    {
                        CurrentColumn = 1;
                    }
                }

                if (lexem != null)
                    CurrentColumn = lexem.Column;
            }
        }
        /// <summary>
        /// Moves caret to the right by one word.
        /// </summary>
        [Command("Navigation.MoveRightWord")
         , KeysBinding(Keys.Control | Keys.Right)
         , KeysBinding(Keys.Control | Keys.Right | Keys.Shift)]
        public virtual void MoveRightWord()
        {
            lock (this)
            {
                RenderedLine line = m_parser.GetLine(CurrentLine) as RenderedLine;
                if (CurrentColumn == line.LineLength)
                {
                    if (CurrentLine != Parser.TotalLines)
                    {
                        CurrentLine++;
                        CurrentColumn = 1;
                        return;
                    }
                }

                IRenderedLexem lexem = line.FindLexemByColumn(CurrentColumn) as IRenderedLexem;
                if (lexem != null)
                {
                    IList lexems = line.LineLexems;
                    int index = lexems.IndexOf(lexem) + 1;
                    while (index < lexems.Count)
                    {
                        lexem = lexems[index] as IRenderedLexem;
                        if ((lexem.Config.Format as Format).Name != "Whitespace")
                        {
                            break;
                        }
                        index++;
                    }

                    if (index < lexems.Count)
                    {
                        CurrentColumn = lexem.Column;
                    }
                    else
                    {
                        MoveToLineEnd();
                    }
                }
            }
        }
        /// <summary>
        /// Moves caret to the beginning of the file.
        /// </summary>
        [Command("Navigation.DocumentStart")
         , KeysBinding(Keys.Control | Keys.Home)
         , KeysBinding(Keys.Control | Keys.Home | Keys.Shift)]
        public virtual void MoveToBeginning()
        {
            lock (this)
            {
                using (((ILongOperationController)this).StartOperation("Moving to beginning"))
                {
                    CurrentLine = 1;
                    CurrentColumn = 1;
                }
            }
        }
        /// <summary>
        /// Moves caret to the end of file.
        /// </summary>
        [Command("Navigation.DocumentEnd")
         , KeysBinding(Keys.Control | Keys.End)
         , KeysBinding(Keys.Control | Keys.End | Keys.Shift)]
        public virtual void MoveToEnd()
        {
            lock (this)
            {
                using (((ILongOperationController)this).StartOperation("Moving to end"))
                {
                    CurrentLine = m_parser.TotalLines;
                    CurrentColumn = GetLineLength(CurrentLine);
                }
            }
        }
        /// <summary>
        /// Start selection.
        /// </summary>
        public virtual void StartSelection()
        {
            lock (this)
            {
                // Sets start of the selection
                IsSelecting = true;
                CoordinatePoint point = GetNearestParsePointLeft(CurrentLine, CurrentColumn);

                VisualLocation visualLoc = GetVisualLocation(this.CurrentLine, this.CurrentColumn);
                if (m_selection.IsEmpty() || (m_selection.End != point && m_selection.VisualEnd != visualLoc && m_selection.End == m_selection.Start ))
                {
                    SetSelectionStart(point);
                }
            }
        }
        /// <summary>
        /// Scrolls the contents of the control to the current caret position.
        /// </summary>
        public virtual void ScrollToCaret()
        {
            if ( !this.SingleLineMode && this.CursorManager.Caret != null )
            {
                ScrollScroller(this.HScrollBar, this.CursorManager.Caret.Position.X - OFFSET_TO_SCROLL);
                ScrollScroller(this.VScrollBar, -this.CursorManager.Caret.Position.Y - OFFSET_TO_SCROLL);
            }
        }
        
        /// <summary>							
        /// Stops selection.
        /// </summary>
        [Command("Edit.StopSelection")]
        public virtual void StopSelection()
        {
            lock (this)
            {
                IsSelecting = false;
            }
        }
        /// <summary>
        /// Resets selection.
        /// </summary>
        [Command("Edit.ResetSelection")
         , KeysBinding(Keys.Escape | Keys.Shift)]
        public virtual void ResetSelection()
        {
            lock (this)
            {
                StopSelection();
                SelectionCancel();
                StartSelection();
            }
        }
        /// <summary>
        /// Changes insert mode.
        /// </summary>
        [Command("Edit.ToggleInsertMode")
         , KeysBinding(Keys.Insert)]
        public virtual void ToggleInsertMode()
        {
            lock (this)
            {
                InsertMode = !InsertMode;
            }
        }
        /// <summary>
        /// Inserts text from clipboard.
        /// </summary>
        [Command("Clipboard.Paste")
         , KeysBinding(Keys.Shift | Keys.Insert)
         , KeysBinding(Keys.Control | Keys.V)]
        public virtual void Paste()
        {
            lock (this)
            {
                IDataObject data = Clipboard.GetDataObject();
                PasteData(data);
            }
        }
        /// <summary>
        /// Copies selected text to clipboard.
        /// </summary>
        [Command("Clipboard.Copy")
         , KeysBinding(Keys.Control | Keys.Insert)
         , KeysBinding(Keys.Control | Keys.C)]
        public virtual void Copy()
        {
            lock (this)
            {
                if (CanCopy)
                {
                    string text = this.SelectedText;
                    text = text.Replace(STR_NEW_LINE, "\r\n");

                    // Workaround adviced by Microsoft (http://thedotnet.com/nntp/127816/showpost.aspx)
                    // For resolving problems with remote computers
                    bool bOK = false;
                    Exception ex = null;
                    for (int i = 0; i < 15; i++)
                    {
                        try
                        {
                            Clipboard.SetDataObject(text, true);
                            bOK = true;
                            break;
                        }
                        catch (System.Runtime.InteropServices.ExternalException extex)
                        {
                            ex = extex;
                        }
                    }

                    if (!bOK)
                    {
                        throw ex;
                    }

                    if (m_selection.IsBlock())
                    {
                        SetBlockClipboardData(text);
                    }
                }
            }
        }
        /// <summary>
        /// Copies selected text to clipboard and deletes it from text.
        /// </summary>
        [Command("Clipboard.Cut")
         , KeysBinding(Keys.Shift | Keys.Delete)
         , KeysBinding(Keys.Control | Keys.X)]
        public virtual void Cut()
        {
            lock (this)
            {
                LockSelection();
                Copy();
                DeleteSelected();
                UnlockSelection();
            }
        }
        /// <summary>
        /// Cancels selection.
        /// </summary>
        [Command("Edit.SelectionCancel")
         , KeysBinding(Keys.Escape)]
        public void EscapeKeyProcess()
        {
            CloseIntellisense();
            HideIndentGuideline();
            SelectionCancel();
        }
        /// <summary>
        /// Removes selection and causes invalidation of the previously selected area.
        /// </summary>
        public virtual void SelectionCancel()
        {
            lock (this)
            {
                if (!m_selection.IsEmpty())
                {
                    foreach (TextRange range in m_selection.Ranges)
                    {
                        m_formatManager[DEF_LAYER_NAME_SELECTION].Remove(range.Top, range.Bottom);
                    }

                    RenderedLine firstLine = m_parser.GetLine(m_selection.Top.VirtualLine) as RenderedLine;
                    RenderedLine lastLine = m_parser.GetLine(m_selection.Bottom.VirtualLine) as RenderedLine;

                    int yMin = (int)Math.Round(firstLine.Y);
                    Rectangle drawRect = new Rectangle(0, yMin, int.MaxValue, (int)Math.Ceiling(lastLine.Y - firstLine.Y + lastLine.Height + 1));

                    InvalidateAll(drawRect);
                    m_selection.Clear();
                    m_selection.VisualEnd = m_selection.VisualStart = VisualLocation.Empty;
                    OnSelectionChanged();

                    if (IsSelecting)
                    {
                        StartSelection();
                    }
                }
            }
        }
        int phyLineCount = 0;  
        /// <summary>
        /// Deletes one char to the right.
        /// </summary>
        [Command("Edit.DeleteChar")
         , KeysBinding(Keys.Delete)]
        public virtual void DeleteChar()
        {
            lock (this)
            {
                phyLineCount = this.PhysicalLineCount;
                if (!DeleteSelected())
                {
                    if (CursorOverCollapsedRegion())
                    {
                        ExpandCurrentLine();
                        if (CursorOverCollapsedRegion())
                        {
                            return;
                        }
                    }

                    CoordinatePoint leftPoint = GetNearestParsePointLeft(CurrentLine, CurrentColumn);
                    // Insert spaces to the end of line to ensure that next line
                    // will be added after cursor.
                    if (leftPoint.VirtualColumn < CurrentColumn)
                    {
                        TextInsertInternal(CurrentLine, leftPoint.VirtualColumn, new string(' ', CurrentColumn - leftPoint.VirtualColumn));
                    }

                    TextDeleteInternal(leftPoint.VirtualLine, leftPoint.VirtualColumn, leftPoint.VirtualLine, leftPoint.VirtualColumn + 1);
                }
            }
        }
        /// <summary>
        /// Deletes one word to the right.
        /// </summary>
        [Command("Edit.DeleteWord")
         , KeysBinding(Keys.Control | Keys.Delete)]
        public virtual void DeleteWord()
        {
            lock (this)
            {
                if (!DeleteSelected())
                {
                    int oldLine = CurrentLine;
                    int oldColumn = CurrentColumn;
                    MoveRightWord();

                    if (oldLine == CurrentLine && oldColumn == CurrentColumn)
                    {
                        DeleteChar();
                    }
                    else
                    {
                        TextDeleteInternal(oldLine, oldColumn, CurrentLine, CurrentColumn);
                    }
                }
            }
        }
        /// <summary>
        /// Deletes one char to the left.
        /// </summary>
        [Command("Edit.Backspace")
         , KeysBinding(Keys.Back)
         , KeysBinding(Keys.Shift | Keys.Back)]
        public virtual void DeleteCharLeft()
        {
            lock (this)
            {
                LockSelection();

                if (!DeleteSelected())
                {
                    if (CursorOverCollapsedRegion())
                    {
                        ExpandCurrentLine();
                        if (CursorOverCollapsedRegion())
                        {
                            return;
                        }
                    }

                    CoordinatePoint leftPoint = GetNearestParsePointLeft(CurrentLine, CurrentColumn);
                    // If we are going to delete real text, then delete it, otherwise just move caret to the left
                    if (leftPoint.VirtualColumn == CurrentColumn)
                    {
                        TextDeleteInternal(CurrentLine, CurrentColumn - 1, CurrentLine, CurrentColumn);
                    }
                    else
                    {
                        CurrentColumn--;
                    }
                }

                UnlockSelection();
            }
        }
        /// <summary>
        /// Deletes one word to the left.
        /// </summary>
        [Command("Edit.DeleteWordLeft")
         , KeysBinding(Keys.Control | Keys.Back)]
        public virtual void DeleteWordLeft()
        {
            lock (this)
            {
                if (!DeleteSelected())
                {
                    int oldColumn = CurrentColumn;
                    if (oldColumn > 1)
                    {
                        int oldLine = CurrentLine;
                        MoveLeftWord();
                        TextDeleteInternal(CurrentLine, CurrentColumn, oldLine, oldColumn);
                    }
                    else
                    {
                        DeleteCharLeft();
                    }
                }
            }
        }

        private Point m_findDialogLocation = Point.Empty;
        /// <summary>
        /// Gets/Sets Find and Replace dialog display location
        /// </summary>
        public Point FindDialogLocation
        {
            get 
            { 
                return m_findDialogLocation;
            }
            set 
            {
                if (m_findDialogLocation != value)
                {
                    m_findDialogLocation = value;
                    if (this.FindDialogWnd != null)
                        (this.FindDialogWnd as Form).Location = m_findDialogLocation;
                }
            }
        }


        /// <summary>
        /// Shows Find dialog window.
        /// </summary>
        [Command("Edit.Find")
         , KeysBinding(Keys.Control | Keys.F)]
        public virtual void FindDialog()
        {
            lock (this)
            {
                string text = SmartGetCurrentText();
                CloseIntellisense();

                (this.FindDialogWnd as Form).RightToLeft = this.RightToLeft;
                if (this.FindDialogLocation != Point.Empty)
                {
                    (this.FindDialogWnd as Form).StartPosition = FormStartPosition.Manual;
                    (this.FindDialogWnd as Form).Location = this.FindDialogLocation;
                }
                FindDialogWnd.Show();
                if (text != string.Empty && !(Regex.IsMatch(text, "\n")))
                {
                    FindDialogWnd.SearchText = text;
                }
                FindDialogWnd.SelectTextAndFocus();
            }
        }

        public void CloseFindDialog()
        {
            if (m_findDialog != null)
            {
                (this.FindDialogWnd as FrmFindDialog).HideDialog();
            }
        }
        public void CloseReplaceDialog()
        {
            if (m_replaceDialog != null)
            {
                (this.ReplaceDialogWnd as frmReplaceDialog).HideDialog();
            }
        }
        /// <summary>
        /// Searches text under cursor, or selected text.
        /// </summary>
        public virtual bool FindCurrentText()
        {
            lock (this)
            {
                string text = SmartGetCurrentText();
                if (text != string.Empty)
                {
                    FindDialogWnd.SearchText = text;
                    return (FindDialogWnd.FindNext() == FindNextResult.Ok);
                }
                return false;
            }
        }
        /// <summary>
        /// Searches text under cursor, or selected text. Used for key bindings.
        /// </summary>
        [Command("Edit.FindSelected")
         , KeysBinding(Keys.Control | Keys.F3)]
        protected void FindCurrentTextKeyBinder()
        {
            FindCurrentText();
        }
        /// <summary>
        /// Searches text under cursor, or selected text.
        /// </summary>
        public virtual bool FindNext()
        {
            lock (this)
            {
                if (FindDialogWnd == null || FindDialogWnd.SearchText == string.Empty)
                    return false;

                return (FindDialogWnd.FindNext() == FindNextResult.Ok);
            }
        }
        /// <summary>
        /// Searches text under cursor, or selected text.
        /// </summary>
        [Command("Edit.FindNext")
         , KeysBinding(Keys.F3)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void FindNextKeyBinder()
        {
            FindNext();
        }
        /// <summary>
        /// Shows Goto dialog window.
        /// </summary>
        [Command("Edit.GoTo")
         , KeysBinding(Keys.Control | Keys.G)]
        public virtual void GoToDialog()
        {
            GotoDialogWnd.MinLine = 1;
            GotoDialogWnd.MaxLine = PhysicalLineCount;
            CloseIntellisense();

            (GotoDialogWnd as Form).RightToLeft = this.RightToLeft;

            if (GotoDialogWnd.ShowDialog() == DialogResult.OK)
            {
                GoToLine(GotoDialogWnd.LineNumber);
            }
        }
        /// <summary>
        /// Shows Replace dialog window.
        /// </summary>
        [Command("Edit.Replace")
         , KeysBinding(Keys.Control | Keys.H)]
        public virtual void ReplaceDialog()
        {
            lock (this)
            {
                string text = SmartGetCurrentText();
                CloseIntellisense();

                (ReplaceDialogWnd as Form).RightToLeft = this.RightToLeft;

                ReplaceDialogWnd.Show();

                if (text != string.Empty && !(Regex.IsMatch(text, "\n")))
                {
                    ReplaceDialogWnd.SearchText = text;
                }

                ReplaceDialogWnd.SelectTextAndFocus();
            }
        }
        /// <summary>
        /// Selects all text.
        /// </summary>
        [Command("Edit.SelectAll")
         , KeysBinding(Keys.Control | Keys.A)]
        public virtual void SelectAll()
        {
            lock (this)
            {
                using (((ILongOperationController)this).StartOperation("Selecting all"))
                {
                    LockUpdate();
                    m_codeSnippetsManager.Activated = false;
                    StopSelection();
                    SelectionCancel();
                    MoveToEnd();
                    SetSelectionStart(GetNearestParsePointLeft(1, 1));
                    SetSelectionEnd(GetNearestParsePointRight(CurrentLine, CurrentColumn));
                    UnlockUpdate();
                }
            }
        }
        /// <summary>
        /// Binds keyboard.
        /// </summary>
        [Command("Configuration.BindKeyboard")
         , KeysBinding(Keys.Control | Keys.B)]
        public virtual void BindKeyboard()
        {
            lock (this)
            {
                m_keyBinder.BeginEdit();

                try
                {
                    using (frmKeysBinding form = new frmKeysBinding(m_keyBinder, this))
                    {
                        CloseIntellisense();
                        (form as Form).RightToLeft = this.RightToLeft;
                        if (DialogResult.OK == form.ShowDialog(this))
                        {
                            m_keyBinder.EndEdit();
                        }
                        else
                        {
                            m_keyBinder.CancelEdit();
                        }
                    }
                }
                catch
                {
                    m_keyBinder.CancelEdit();
                }
            }
        }
        /// <summary>
        /// Dumps undo/redo data.
        /// </summary>
        [Conditional("DEBUG")]
        private void DumpUndoData()
        {
            Debug.WriteLine("-------------------");
            Debug.WriteLine("Undo Stack:");

            Stack stackUndo = (Stack)Parser.UndoData.UndoStack.Clone();
            Stack stackRedo = (Stack)Parser.UndoData.RedoStack.Clone();
            StringBuilder b = new StringBuilder(16);
            string strMarkerUndo = string.Empty;
            string strMarkerRedo = string.Empty;

            if (m_UndoMarkers.Count > 0)
            {
                int iMarkerUndo = (int)m_UndoMarkers.Peek();
                int iSpaceLengthUndo = (stackUndo.Count - iMarkerUndo) * 3;
                if (iSpaceLengthUndo > 0)
                {
                    strMarkerUndo = new string(' ', iSpaceLengthUndo);
                }

                strMarkerUndo += " ^^";
            }

            if (m_RedoMarkers.Count > 0)
            {
                int iMarkerRedo = (int)m_RedoMarkers.Peek();
                int iSpaceLengthRedo = (stackRedo.Count - iMarkerRedo) * 3;

                if (iSpaceLengthRedo > 0)
                {
                    strMarkerRedo = new string(' ', iSpaceLengthRedo);
                }

                strMarkerRedo += " ^^";
            }

            for (int i = 0; i < stackUndo.Count; i++)
            {
                LexemParser.UndoItem item = (LexemParser.UndoItem)stackUndo.Pop();
                b.AppendFormat("{0,3}", item.StartOffset);
            }

            Debug.WriteLine(b.ToString());
            Debug.WriteLine(strMarkerUndo);

            Debug.WriteLine("Redo Stack:");
            b.Length = 0;

            for (int i = 0; i < stackRedo.Count; i++)
            {
                LexemParser.UndoItem item = (LexemParser.UndoItem)stackRedo.Pop();
                b.AppendFormat("{0,3}", item.StartOffset);
            }

            Debug.WriteLine(b.ToString());
            Debug.WriteLine(strMarkerRedo);
            Debug.WriteLine("-------------------");
            undoOrRedo = string.Empty;
        }
        /// <summary>
        /// Undoes last operation.
        /// </summary>
        [Command("Edit.Undo")
         , KeysBinding(Keys.Control | Keys.Z)]
        public virtual void Undo()
        {
            undoOrRedo = "UnDo";
            lock (this)
            {
                if (m_iUndoGroupOpened == 0)
                {
                    StopSelection();
                    SelectionCancel();

                    if (this.CanUndo)
                    {
                        Point point = Point.Empty;
                        bool bNeedMore = m_bGroupUndo;
                        int iUndoesNumber = 0;
                        int iInitialUndoQueueLength = m_parser.UndoQueueLength;

                        do
                        {
                            int iUndoCount = 1;
                            bool bGroupReplaced = false;
                            if (m_undoGroups.Count > 0)
                            {
                                UndoGroup group = (UndoGroup)m_undoGroups.Peek();

                                if (group.EndActionsCount == m_parser.UndoQueueLength)
                                {
                                    iUndoCount = group.GroupLength;
                                    m_redoGroups.Push(m_undoGroups.Pop());
                                    bGroupReplaced = true;
                                }
                            }

                            while (iUndoCount-- > 0)
                            {
                                point = m_parser.Undo();

                                if (point.IsEmpty)
                                {
                                    if (bGroupReplaced)
                                    {
                                        m_undoGroups.Push(m_redoGroups.Pop());
                                    }

                                    for (int i = 0; i < iUndoesNumber; i++)
                                    {
                                        m_parser.Redo(false);
                                    }

                                    return;
                                }
                                else
                                {
                                    iUndoesNumber++;
                                }
                            }

                            if (m_bGroupUndo)
                            {
                                // We should do this because of the undo-grouping.
                                while (m_UndoMarkers.Count > 0 && (int)m_UndoMarkers.Peek() > m_parser.UndoQueueLength)
                                {
                                    m_UndoMarkers.Pop();
                                }

                                if (m_UndoMarkers.Count > 0)
                                {
                                    int iStopPosition = (int)m_UndoMarkers.Peek();
                                    if (iStopPosition <= m_parser.UndoQueueLength)
                                    {
                                        bNeedMore = false;
                                        int iRedoMarker = m_parser.RedoQueueLength - (iInitialUndoQueueLength - (int)m_UndoMarkers.Pop());
                                        if (iRedoMarker >= 0)
                                        {
                                            m_RedoMarkers.Push(iRedoMarker);
                                        }
                                    }
                                }
                                else
                                {
                                    bNeedMore = false;
                                }
                            }
                        }
                        while (bNeedMore);

                        if (point != Point.Empty)
                        {
                            CurrentPosition = point;
                        }

                        RaiseUpdateChangedStateEvent();
                    }

                    DumpUndoData();
                }
            }
        }
        /// <summary>
        /// Undoes last operation.
        /// </summary>
        [Command("Edit.Redo")
         , KeysBinding(Keys.Control | Keys.Y)]
        public virtual void Redo()
        {
            undoOrRedo = "ReDo";
            lock (this)
            {
                if (m_iUndoGroupOpened == 0)
                {
                    StopSelection();
                    SelectionCancel();

                    if (CanRedo)
                    {
                        Point point;
                        bool bNeedMore = m_bGroupUndo;
                        int iInitialRedoQueueLength = m_parser.RedoQueueLength;

                        do
                        {
                            int iRedoCount = 1;
                            if (m_redoGroups.Count > 0)
                            {
                                UndoGroup group = (UndoGroup)m_redoGroups.Peek();
                                if (group.StartActionsCount == m_parser.UndoQueueLength)
                                {
                                    iRedoCount = group.GroupLength;
                                    m_undoGroups.Push(m_redoGroups.Pop());
                                }
                            }

                            while (iRedoCount-- > 1)
                            {
                                m_parser.Redo();
                            }

                            point = m_parser.Redo();
                            if (m_bGroupUndo)
                            {
                                // We should do this because of the undo-grouping.
                                while (m_RedoMarkers.Count > 0 && (int)m_RedoMarkers.Peek() > m_parser.RedoQueueLength)
                                {
                                    m_RedoMarkers.Pop();
                                }

                                if (m_RedoMarkers.Count > 0)
                                {
                                    int iStopPosition = (int)m_RedoMarkers.Peek();
                                    if (iStopPosition == m_parser.RedoQueueLength)
                                    {
                                        bNeedMore = false;
                                        int iUndoMarker = m_parser.UndoQueueLength - (iInitialRedoQueueLength - (int)m_RedoMarkers.Pop());

                                        if (iUndoMarker >= 0)
                                        {
                                            m_UndoMarkers.Push(iUndoMarker);
                                        }
                                    }
                                }
                                else
                                {
                                    bNeedMore = false;
                                }
                            }
                        }
                        while (bNeedMore);

                        if (point != Point.Empty)
                        {
                            CurrentPosition = point;
                        }

                        RaiseUpdateChangedStateEvent();
                    }
                }
            }

            DumpUndoData();
        }
        /// <summary>
        /// Refreshes screen, frees up memory, deletes a lot of parsepoints.
        /// </summary>
        [Command("Edit.Refresh")
         , KeysBinding(Keys.F5)]
        public override void Refresh()
        {
            lock (this)
            {
                using (((ILongOperationController)this).StartOperation("Refreshing"))
                {
                    m_parser.MaxWidth = MaxWidth;
                    long memAvail = GC.GetTotalMemory(false);

                    IEnumerator enumerator = m_parser.GetLineEnumerator();
                    int iRemovedLines = 0;
                    int iTurnedOffLines = 0;

                    while (enumerator.MoveNext())
                    {
                        (enumerator.Current as RenderedLine).Parsed = false;
                        iTurnedOffLines++;
                    }

                    GC.Collect();

                    memAvail = -GC.GetTotalMemory(true) + memAvail;
                    double KBFreed = memAvail / 1024.0;
                    Debug.WriteLine("Memory freed after resetting ParsePoints: " + KBFreed.ToString("0.000"));
                    Debug.WriteLine(string.Format("Lines removed: {0}, Lines turned off: {1}", iRemovedLines, iTurnedOffLines));
                    GenerateDebugMap();

                    base.Refresh();
                }
            }
        }
        /// <summary>
        /// Turns off collapsing.
        /// </summary>
        [Command("Edit.Collapsing.Off")
         , KeysBinding(Keys.Control | Keys.M, Keys.Control | Keys.P)
         ]
        public virtual void SwitchCollapsingOff()
        {
            lock (this)
            {
                using (((ILongOperationController)this).StartOperation("Collapsings off"))
                {
                    StopSelection();
                    SelectionCancel();
                    ExpandAll();
                }

            }
        }
        /// <summary>
        /// Turns on collapsing and collapses all.
        /// </summary>
        [Command("Edit.Collapsing.On")
         , KeysBinding(Keys.Control | Keys.M, Keys.Control | Keys.O)
         ]
        public virtual void SwitchCollapsingOn()
        {
            lock (this)
            {
                using (((ILongOperationController)this).StartOperation("Collapsings on"))
                {
                    StopSelection();
                    SelectionCancel();
                    ShowCollapse = true;

                    m_parser.SetAllCollapsings(true);
                    UpdateScrollerVerticalSize();
                }
            }
        }
        /// <summary>
        /// Toggles collapsing for current line.
        /// </summary>
        [Command("Edit.Collapsing.Toggle")
         , KeysBinding(Keys.Control | Keys.M, Keys.Control | Keys.M)
         ]
        public virtual void ToggleLineCollapsing()
        {
            
            RenderedLine line;
            bool bNeedUnCollapse;
            bool bNeedCollapse;
            lock (this)
            {
                using (((ILongOperationController)this).StartOperation("Collapsings toggle"))
                {
                    m_codeSnippetsManager.Activated = false;
                    m_listCollapsed.Clear();
                    m_listNotCollapsed.Clear();
                    

                    if (!m_selection.IsEmpty() && m_selection.Start != m_selection.End)
                    {
                        int bottomLine = m_selection.Bottom.VirtualLine;
                        for (int i = m_selection.Top.VirtualLine; i < bottomLine; i++)
                        {
                            line = m_parser.GetLine(i) as RenderedLine;
                            if (!this.NeedCollapse(line))
                            {
                                continue;
                            }
                            FillLineCollapsers(line, m_listCollapsed, m_listNotCollapsed);
                        }
                    }
                    else
                    {
                        line = m_parser.GetLine(CurrentLine) as RenderedLine;
                        if (!this.NeedCollapse(line))
                        {
                            return;
                        }
                        FillLineCollapsers(line, m_listCollapsed, m_listNotCollapsed);
                    }

                    bNeedUnCollapse = (m_listCollapsed.Count > 0);
                    bNeedCollapse = (m_listNotCollapsed.Count > 0);

                 
                    StopSelection();
                    SelectionCancel();
                    HideIndentGuideline();
                    
                    if (!(bNeedUnCollapse || bNeedCollapse))
                    {
                        IParsePoint point = GetNearestParsePointLeft(CurrentLine, CurrentColumn).PhysicalPoint;
                        
                        CollapsableRegion region = m_parser.GetOuterCollapsableRegion(point, false);

                        if (region == null)
                            return;
                        
                      
                        line = m_parser.GetLine(region.Start.Line) as RenderedLine;

                        if (!this.NeedCollapse(line))
                        {
                            return;
                        }

                        FillLineCollapsers(line, m_listCollapsed, m_listNotCollapsed);
                         
                        bNeedUnCollapse = (m_listCollapsed.Count > 0);
                        bNeedCollapse = (m_listNotCollapsed.Count > 0);

                    }

                    m_parser.UpdateLineInformation();
                    
                    ProcessCollapsing((!bNeedUnCollapse) ? m_listNotCollapsed : m_listCollapsed, !bNeedUnCollapse);
                   
                }
               
            }
        }
        /// <summary>
        /// Generates HTML document and puts it to the clipboard.
        /// </summary>
        [Command("Tools.GenerateHTML")
         , KeysBinding(Keys.Control | Keys.Q)]
        public void GenerateHTML()
        {
            lock (this)
            {
                using (((ILongOperationController)this).StartOperation("HTML to clipboard"))
                {
                    string dataStr = m_exporter.GetHTML();

                    DataObject data = new DataObject(DataFormats.Html, ClipboardHTML.GetHTMLForClipboard(dataStr));

                    bool bOK = false;
                    Exception ex = null;

                    // Workaround adviced by Microsoft (http://thedotnet.com/nntp/127816/showpost.aspx)
                    // For problems with remote computers
                    for (int i = 0; i < 15; i++)
                    {
                        try
                        {
                            Clipboard.SetDataObject(data, true);

                            bOK = true;
                            break;
                        }
                        catch (System.Runtime.InteropServices.ExternalException extex)
                        {
                            ex = extex;
                        }
                    }

                    if (!bOK)
                    {
                        throw ex;
                    }
                }
            }
        }
        /// <summary>
        /// Shows Context Prompt dialog.
        /// </summary>
        [Command("Editor.ContextPrompt")
         , KeysBinding(Keys.Control | Keys.Shift | Keys.Space)]
        public virtual void ShowContextPrompt()
        {
            lock (this)
            {
                RenderedLexem dropper = null;
                RenderedLine dropperLine = null;

                ConfigStack stack = GetCurrentStack();
                m_iContextPromptOpeningLexemColumn = -1;
                IStackData stackItem = null;

                if (stack != null)
                {
                    while (stack.Count > 0 && !((IStackData)(stackItem = stack.Pop())).Config.DropContextPrompt)
                    {
                    }

                    if (stackItem.Config.DropContextPrompt)
                    {
                        int iLine = m_parser.GetCoordinatePoint(stackItem.Location).VirtualLine;
                        dropperLine = (RenderedLine)m_parser.GetLine(iLine);
                        dropper = (RenderedLexem)dropperLine.FindLexemByColumn(stackItem.Location.Position);
                        m_iContextPromptOpeningLexemColumn = dropper.Column;

                        if (ContextPromptBeforeOpen != null)
                        {
                            CancelEventArgs argsCancel = new CancelEventArgs();
                            ContextPromptBeforeOpen(this, argsCancel);

                            if (argsCancel.Cancel)
                            {
                                return;
                            }
                        }

                        if (ContextPromptOpen != null)
                        {
                            // create form if needed
                            if (m_contextPrompt == null || m_contextPrompt.IsDisposed)
                            {
                                m_contextPrompt = new ContextPrompt(this);
                            }

                            m_contextPrompt.Dropper = dropper;

                            IRenderedLexem lexemBeforeDropper = GetPreviousNotWhitespaceLexem(dropperLine.LineIndex, dropper);
                            m_contextPrompt.LexemBeforeDropper = lexemBeforeDropper;

                            ContextPromptUpdateEventArgs e = new ContextPromptUpdateEventArgs(m_contextPrompt.List, dropper, lexemBeforeDropper);
                            ContextPromptOpen(this, e);

                            if (e.List.Count == 0 || e.CloseForm)
                            {
                                m_contextPrompt.Dispose();
                                m_contextPrompt = null;
                                return;
                            }

                            m_contextPrompt.Closed += new EventHandler(OnContextPromptClosed);
                            m_contextPrompt.SelectedPromptChanged += new ContextPromptSelectionChangedEventHandler(OnContextPromptSelectedPromptChanged);

                            Point location = PointToScreen(CursorGraphicalLocation);
                            m_contextPrompt.UseXPStyle = m_bUseXPStyle;
                            m_contextPrompt.UseCustomSize = m_bUseCustomSizeContextPrompt;
                            m_contextPrompt.BorderColor = m_clrContextPromptBorder;
                            m_contextPrompt.BackgroundBrush = m_contextPromptBackgroundBrush;
                            m_contextPrompt.ShowContextPrompt(location, m_contextPromptSize);
                            AttachLayoutEventsToParents();

                            m_IntellisenseType = IntellisenseType.ContextPrompt;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Shows auto complete dialog.
        /// </summary>
        [Command("Editor.ContextChoice")
         , KeysBinding(Keys.Control | Keys.Space)]
        public virtual void ShowContextChoice()
        {
            LockUpdate();

            lock (this)
            {
                using (((ILongOperationController)this).StartOperation("Show autocomplete"))
                {
                    ContextChoiceController.AutoCompleteStringInfo ac = m_controllerContextChoice.GetAutoCompleteStringInfo();
                    m_iContextChoiceLastWordColumn = (m_controllerContextChoice.UseAutocomplete && !ac.IsEmpty()) ? (ac.Column) : (CurrentColumn);

                    IRenderedLexem dropper = null;
                    IRenderedLexem lexemBeforeDropper = null;

                    RenderedLine curLine = CurrentLineInstance as RenderedLine;
                    if (null != curLine)
                    {
                        int lexIndex = curLine.LineLexems.IndexOf(GetLexemUnderCursor());
                        for (; lexIndex >= 0; lexIndex--)
                        {
                            IRenderedLexem lex = (IRenderedLexem)curLine.LineLexems[lexIndex];

                            if (lex.Config.DropContextChoiceList)
                            {
                                dropper = lex;
                                lexemBeforeDropper = GetPreviousNotWhitespaceLexem(curLine.LineIndex, dropper);
                                break;
                            }
                        }
                    }

                    m_controllerContextChoice.Dropper = dropper;
                    m_controllerContextChoice.LexemBeforeDropper = lexemBeforeDropper;
                    m_controllerContextChoice.Show();

                    if (m_bAutoCompleteSingleLexem)
                    {
                        TryToCompleteWord();
                    }

                    AttachLayoutEventsToParents();
                    m_IntellisenseType = IntellisenseType.ContextChoice;
                }
            }

            UnlockUpdate();
        }
        /// <summary>
        /// Shows code snippets choice list.
        /// </summary>
        [Command("Editor.CodeSnippets")
         , KeysBinding(Keys.Control | Keys.Oemtilde)]
        public virtual void ShowCodeSnippets()
        {
            if (!Language.SnippetsContainer.IsEmpty)
            {
                Point loc = this.PointToScreen(this.CursorGraphicalLocation);
                loc.Y -= m_codeSnippetsEditBox.Height;
                m_codeSnippetsEditBox.Location = loc;
                m_codeSnippetsEditBox.Visible = true;
                m_codeSnippetsEditBox.SelectNextControl(null, true, true, true, true);
            }
        }
        /// <summary>
        /// Toggles showing of whitespaces.
        /// </summary>
        [Command("View.ShowWhiteSpaces")
         , KeysBinding(Keys.Control | Keys.Shift | Keys.W)]
        public virtual void ToggleShowingWhiteSpaces()
        {
            ShowWhitespaces = !ShowWhitespaces;
        }
        /// <summary>
        /// Adds leading tab symbol to the selected lines, or just inserts tab symbol.
        /// </summary>
        [Command("Edit.AddLeadingTab")
         , KeysBinding(Keys.Tab)]
        public virtual void AddTabsToSelection()
        {
            AddTabsToSelection(false);
        }
        /// <summary>
        /// Removes leading tab symbol (or it's spaces equivalent) from selected lines.
        /// </summary>
        [Command("Edit.RemoveLeadingTab")
         , KeysBinding(Keys.Shift | Keys.Tab)]
        public virtual void RemoveTabsFromSelection()
        {
            if (m_bTransferFocusOnTab)
                return; 

            lock (this)
            {
                int start = (!m_selection.IsEmpty()) ? (m_selection.Top.VirtualLine) : (CurrentLine);
                int end = (!m_selection.IsEmpty()) ? (m_selection.Bottom.VirtualLine) : (CurrentLine);
                CoordinatePoint endPoint = m_selection.Bottom;

                if (!this.Selection.IsEmpty() && this.Selection.Top != this.Selection.Bottom)
                {
                    for (int i = 0, count = m_selection.Ranges.Count; i < count; i++)
                    {
                        TextRange range = (TextRange)m_selection.Ranges[i];
                        int startLine = range.Top.VirtualLine;
                        int endLine = range.Bottom.VirtualLine;
                        if (this.Selection.Bottom.VirtualColumn == 1)
                        {
                            endLine--;
                        }
                        RemoveGuidingTabs(startLine, endLine);
                    }
                }
                else
                {
                    RemoveGuidingTabs(this.CurrentLine, this.CurrentLine);
                }

                if (endPoint != null && endPoint.VirtualColumn == 1 && start != end)
                {
                    end--;
                }

                StopSelection();
                SelectionCancel();
                m_selection.Clear();
                CurrentPosition = new Point(1, start);
                StartSelection();

                int x, y;
                if (m_parser.TotalLines != end)
                {
                    x = 1;
                    y = end + 1;
                }
                else
                {
                    x = m_parser.GetLine(end).LineLength + 1;
                    y = end;
                }
                CurrentPosition = new Point(x, y);
                StopSelection();
            }
        }
        /// <summary>
        /// Proceeds with some tests.
        /// </summary>
        [Command("General.Test")
         , KeysBinding(Keys.Control | Keys.T, Keys.Control | Keys.T)]
        public void TestProc()
        {
            lock (this)
            {
                m_keyBinder.BeginEdit();
                m_keyBinder.Binder.BindToCommand(Keys.Control | Keys.T, "General.Test");
                m_keyBinder.CancelEdit();
            }
        }
        /// <summary>
        /// Proceeds with some tests.
        /// </summary>
        [Command("General.Test.Indent")
         , KeysBinding(Keys.Shift | Keys.Control | Keys.I)]
        public void IndentTest()
        {
            lock (this)
            {
            }
        }
        /// <summary>
        /// If possible, shows indent Guideline of the current region.
        /// </summary>
        [Command("View.ShowIndentationGuideline")
         , KeysBinding(Keys.Control | Keys.K, Keys.Control | Keys.I)]
        public void ShowIndentGuideline()
        {
            IndentGuideline = GetCurrentIndentGuideline(true);
        }
        /// <summary>
        /// Jumps to the start of the block.
        /// </summary>
        [Command("Navigation.JumpBlockStart")]
        [KeysBinding(Keys.Alt | Keys.Up)]
        public void JumpToIndentBlockStart()
        {
            IndentGuidelineRegionInfo info = GetCurrentIndentGuideline(true);

            if (info != null)
            {
                CurrentPosition = new Point(info.PointStart.VirtualColumn, info.PointStart.VirtualLine);
                UpdateScrollInfo();
            }
        }
        /// <summary>
        /// Jumps to the end of the block.
        /// </summary>
        [Command("Navigation.JumpBlockEnd")]
        [KeysBinding(Keys.Alt | Keys.Down)]
        public void JumpToIndentBlockEnd()
        {
            IndentGuidelineRegionInfo info = GetCurrentIndentGuideline(true);

            if (info != null)
            {
                CurrentPosition = new Point(info.PointEnd.VirtualColumn, info.PointEnd.VirtualLine);
                UpdateScrollInfo();
            }
        }
        /// <summary>
        /// Generates parsing map of the file.
        /// </summary>
        [Command("General.Map")
         , KeysBinding(Keys.Control | Keys.Shift | Keys.M)]
        [Conditional("DEBUG")]
        public void GenerateDebugMap()
        {
            using (TextWriter writer_ = new StreamWriter("NonLocalized.txt"))
            {
                Localizer.WriteNonLocalizedItemsReport(writer_);
                Debug.WriteLine(@"List of non-localized string has been written to NonLocalized.txt");
            }

            IEnumerator enumerator = m_parser.GetLineEnumerator();
            int iLastLine = -1;
            int iLastVirtualLine = -1;
            bool bWasError = false;

            StreamWriter writer = new StreamWriter("LineParseMap.txt");

            while (enumerator.MoveNext())
            {
                RenderedLine line = enumerator.Current as RenderedLine;
                int iPhisicalLine = line.LineStartPoint.Line;

                bWasError = (bWasError || (iPhisicalLine < iLastLine) || (iLastVirtualLine >= line.LineIndex));

                if (iLastLine < (iPhisicalLine - 1))
                {
                    writer.WriteLine(string.Format("Not Parsed: {0} - {1}", Math.Max(0, iLastLine), iPhisicalLine));
                }

                writer.WriteLine(string.Format("Line(Phisical): {0}; Line(Virtual): {3}; Parsed: {1, 5}; Measured: {2,5}; Y: {4}; Height: {5}",
                    iPhisicalLine, line.Parsed, line.IsMeasured, line.LineIndex, line.Y, line.Height));

                iLastLine = iPhisicalLine;
                iLastVirtualLine = line.LineIndex;
            }
            writer.Close();

            if (bWasError)
            {
                MessageBox.Show(Localizer.GetString(Localizer.DEF_MSG_LINE_NUMBERING_PROBLEMS));
            }
        }
        /// <summary>
        /// Generates parsing map of the file.
        /// </summary>
        [Command("General.KeyMap")
         , KeysBinding(Keys.Control | Keys.Shift | Keys.K)]
        public void GenerateDebugKeyMap()
        {
            Stream stream = new FileStream("KeyMap.txt", FileMode.Create);
            m_keyBinder.SaveBindingsToXML(stream);
            stream.Close();
        }
        /// <summary>
        /// Underlines selection with wave line.
        /// </summary>
        [
        Command("View.SetWaveline"),
        KeysBinding(Keys.Control | Keys.W)
        ]
        public void SetWaveLines()
        {
            if (!m_selection.IsEmpty())
            {
                SetSelectionUnderline(Color.Black, FrameBorderStyle.Wave, BorderWeight.Thin);
            }
        }
        /// <summary>
        /// Strikes out selection.
        /// </summary>
        [
        Command("View.StrikeOutSelection"),
        KeysBinding(Keys.Shift | Keys.Alt | Keys.S)
        ]
        public void StrikeOutSelectedText()
        {
            if (!m_selection.IsEmpty())
            {
                SetSelectionStrikeout(Color.Gray);
            }
        }
        /// <summary>
        /// Removes wave underlining from selection.
        /// </summary>
        [
        Command("View.RemoveWaveline"),
        KeysBinding(Keys.Control | Keys.Shift | Keys.W)
        ]
        public void RemoveWaveLines()
        {
            if (!m_selection.IsEmpty())
            {
                foreach (TextRange range in m_selection.Ranges)
                {
                    RemoveUnderline(range.Top, range.Bottom);
                }
            }
        }
        /// <summary>
        /// Changes spaces sequences to tabs.
        /// </summary>
        [
        Command("Edit.TabifySelection"),
        KeysBinding(Keys.Control | Keys.T, Keys.Control | Keys.T)
        ]
        public void TabifySelection()
        {
            ReplaceLexemsInSelection(FormatType.Whitespace, new LexemReplaceEventHandler(ReplaceWhiteSpaceWithTabs));
        }
        /// <summary>
        /// Changes tabs sequences to spaces.
        /// </summary>
        [
        Command("Edit.UntabifySelection"),
        KeysBinding(Keys.Control | Keys.T, Keys.Control | Keys.U)
        ]
        public void UntabifySelection()
        {
            ReplaceLexemsInSelection(FormatType.Whitespace, new LexemReplaceEventHandler(ReplaceTabsWithWhiteSpace));
        }
        #endregion

        #region Paint Methods
        /// <summary>
        /// Draws line background.
        /// </summary>
        /// <param name="g">Graphics object.</param>
        /// <param name="xOffset">X coordinate of the line.</param>
        /// <param name="line">Line, the background should be rendered.</param>
        protected virtual void DrawLineBorders(Graphics g, float xOffset, RenderedLine line)
        {
            IDynamicFormatsLayer backGrounds = m_formatManager[DEF_LAYER_NAME_LINEBACKCOLOR_OVER];
            IDynamicFormatsLayer backGrounds_borders = m_formatManager[DEF_LAYER_NAME_BORDER_OVER];

            if (backGrounds.Hidden)
            {

                ArrayList list;
                CoordinatePoint pointStart = line.GetStartPoint();
                CoordinatePoint pointEnd = line.GetEndPoint();
                IList backGroundsList = backGrounds[pointStart, pointEnd];

                if (backGroundsList is ArrayList)
                {
                    list = (ArrayList)backGroundsList;
                }
                else
                {
                    list = new ArrayList(backGroundsList);
                }

                list.AddRange(backGrounds_borders[pointStart, pointEnd]);
                foreach (IDynamicFormat format in backGroundsList)
                {
                    Format snippetFormat = format.Format as Format;
                    if (!snippetFormat.BorderColor.IsEmpty)
                    {
                        snippetFormat.ForeColor = snippetFormat.BorderColor;
                    }

                    if (!snippetFormat.ForeColor.IsEmpty)
                    {
                        Matrix matr = g.Transform;
                        g.TranslateTransform(-xOffset, 0);
                        if (format.Start <= format.End)
                        {
                        GraphicsPath path = GetTextDrawPath(format.Start, format.End);
                        if (null != path)
                        {
                            g.DrawPath(snippetFormat.BackGroundPen, path);
                            path.Dispose();
                        }
                        }

                        g.Transform = matr;
                        matr.Dispose();
                    }
                }
                pointStart.Dispose();
                pointEnd.Dispose();
            }
        }
        /// <summary>
        /// Draws PreRenderedLine on the given graphics object. Line must be previously measured, or it will not be rendered correctly.
        /// </summary>
        /// <param name="g">Graphics object, line must be rendered to.</param>
        /// <param name="line">Line, to be rendered.</param>
        /// <param name="x">X Position of the rendering.</param>
        /// <param name="y">Y Position of the rendering.</param>
        /// <param name="drawDynamicFormatting">Indicates whether dynamic formatting should be drawn.</param>
        /// <param name="autoScrollY">Y autoscroll position. Used for proper native drawing.</param>
        /// <param name="scale">Scale value for output. Used in printing for resolving printing problems related to native methods.</param>
        /// <param name="margins">Margin offsets for text output. Used in printing.</param>
        protected virtual float DrawLexemLine(
            Graphics g, RenderedLine line, float x, float y, bool drawDynamicFormatting, int autoScrollY, float scale, Size margins)
        {
            if (line == null) throw new ArgumentNullException("line");
            if (g == null) throw new ArgumentNullException("g");

            m_parser.SetDPIFromGraphics(g);
            m_parser.MeasureLine(line);

            IList lexems = line.LineLexems;
            float width = 0;
            float lastY = y;
            float lastX = x;

            TextDrawInfo txtDraw = new TextDrawInfo();
            CoordinatePoint startPoint = new CoordinatePoint(m_parser, null, 1, 1, false);
            CoordinatePoint endPoint = new CoordinatePoint(m_parser, null, 1, 1, false);
            ISnippetFormat afterTextFormat = null;
            CoordinatePoint pointLast = null;
            bool bDivider = false;

            RectangleF clipRectangle = g.ClipBounds;
            int lexemsCount = lexems.Count;
            bool bSaved = (m_savedLinesPoints.BinarySearch(line.LineStartPoint, ParsePointManager.DEF_PARSEPOINT_SEARCH) >= 0);
            bool bChanged = line.Changed;

            if (m_bSelectionMarginVisible && m_bMarkChangedLines && (bChanged || bSaved))
            {
                int offset = this.SelectionMarginOffset;
                Rectangle markerRect = new Rectangle(offset, (int)y, m_iSelectionMarginWidth + 1, (int)line.Height);

                Region oldClip = g.Clip.Clone();
                g.SetClip(markerRect, CombineMode.Replace);

                if (bChanged)
                {
                    g.FillRectangle(m_changedLinesMarkingLineBrush, markerRect);
                }
                else if (bSaved)
                {
                    g.FillRectangle(m_savedLinesMarkingLineBrush, markerRect);
                }

                g.SetClip(oldClip, CombineMode.Replace);
            }
            
            // Borders drawing stuff.
            BorderInfo borderInfo = new BorderInfo();
            borderInfo.Rect = new RectangleF(0, 0, 0, 0);
            borderInfo.format = null;
            borderInfo.bFinish = false;

            for (int i = 0; i < lexemsCount; i++)
            {
                IRenderedLexem lex = lexems[i] as IRenderedLexem;
                ISnippetFormat format = lex.Config.Format;

                string lexemGroupText = lex.Text;
                float lexemGroupWidth = lex.Width;
                bDivider |= lex.Config.ContentDivider && (!lex.Config.IsComplex || lex.Config.IsEqualToEnd(lex.Text));

                int lexemLength = lexemGroupText.Length;
                float lexemX = x + lex.XOffset;
                float lexemY = lastY = y + lex.YOffset;
                width = Math.Max(width, lex.XOffset + lexemGroupWidth);

                lastX = lexemX + lexemGroupWidth;

                if (this.RightToLeft == RightToLeft.Yes)
                    lexemX = this.TextDrawOffset + ScrollOffsetRight - lexemX;
                txtDraw.Text = lexemGroupText;
                txtDraw.DrawRectangle.X = (int)Math.Round(lexemX);
                txtDraw.DrawRectangle.Y = (int)Math.Round(lexemY);
                txtDraw.DrawRectangle.Width = (int)Math.Ceiling(lexemGroupWidth);
                txtDraw.DrawRectangle.Height = (int)Math.Ceiling(line.SubLineHeight[lex.SubLine]);
                txtDraw.TextHeight = (int)line.GetSubLineTextHeight(lex.SubLine);
                txtDraw.VerticalAlignment = StringAlignment.Near;

                if (lex.Column < 1 || lexemLength < 1) throw new ApplicationException(
                       Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_89);

                startPoint = new CoordinatePoint(null, null, line.LineIndex, lex.Column, false);
                endPoint = new CoordinatePoint(null, null, line.LineIndex, lex.Column + lexemLength, false);

                if (pointLast == null)
                {
                    pointLast = startPoint;
                }

                if (startPoint < endPoint)
                txtDraw.DynamicFormattings =
                    (drawDynamicFormatting) ? (GetAndConvertFormattings(startPoint, endPoint, out afterTextFormat)) : (null);

                float lexemRight = txtDraw.DrawRectangle.X + txtDraw.DrawRectangle.Width;
                float lexemLeft = txtDraw.DrawRectangle.X;

                bool bIsLeftInClip = clipRectangle.X <= lexemLeft;
                bool bIsRightInClip = clipRectangle.X <= lexemRight && clipRectangle.Right >= lexemRight;
                bool bIsClipInside = clipRectangle.X >= lexemLeft && clipRectangle.Right <= lexemRight;

                // For borders.
                bool bFinish = (i == lexemsCount - 1) || (lexems[i + 1] as IRenderedLexem).SubLine != lex.SubLine;

                borderInfo.bFinish = bFinish && (afterTextFormat == null || afterTextFormat.BorderStyle == FrameBorderStyle.None);

                // Indicates whether current lexem is the first lexem of wrapped line.
                bool bWrappedLine = (i > 0 && (lexems[i - 1] as IRenderedLexem).SubLine != lex.SubLine);

                if (bWrappedLine && m_bMarkWrappedLines)
                {
                    int xOffset = (this.RightToLeft == RightToLeft.Yes) ? (this.TextDrawOffset - this.WrappedLinesMarkingImage.Width - 2) : (int)x;
                    g.DrawImage(this.WrappedLinesMarkingImage,xOffset, line.Y + lex.YOffset);
                }

                if (bFinish)
                {
                    if (afterTextFormat != null)
                    {
                        Rectangle aftertextRect = txtDraw.DrawRectangle;
                        if (this.RightToLeft == RightToLeft.Yes)
                        {
                            aftertextRect.X = this.ClientRectangle.Left + this.ScrollOffsetLeft;
                            aftertextRect.Width = (int)Math.Round(lexemX - lexemGroupWidth);
                        }
                        else
                        {
                            aftertextRect.X = (int)Math.Round(lexemX + lexemGroupWidth);
                            aftertextRect.Width = this.ClientRectangle.Width - this.ScrollOffsetRight - aftertextRect.X;
                        }
                        g.FillRectangle((afterTextFormat as Format).BackGroundBrush, aftertextRect);
                    }
                }

                if (borderInfo.format != null)
                {
                    IRenderedLexem prevLex = null;
                    if (i != 0)
                    {
                        prevLex = (IRenderedLexem)lexems[i - 1];
                    }
                    if (prevLex == null || prevLex.SubLine != lex.SubLine)
                    {
                        borderInfo.Rect.Location = txtDraw.DrawRectangle.Location;
                        borderInfo.Rect.Width = 0;
                    }
                }

                Format formatImpl = (Format)format;

                bool RTL = this.RightToLeft == RightToLeft.Yes;
                if (!RTL)
                    formatImpl.StringFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.DisplayFormatControl;
                else
                    formatImpl.StringFormat.FormatFlags = StringFormatFlags.DirectionRightToLeft | StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.DisplayFormatControl;
                formatImpl.RightToLeft = RTL;
                formatImpl.TextDrawOffset = TextDrawOffset;

                if (bIsLeftInClip || bIsRightInClip || bIsClipInside
                    || (borderInfo.format != null && borderInfo.format.BorderStyle != FrameBorderStyle.None))
                {
                    formatImpl.DrawText(g, ref txtDraw, ref borderInfo, autoScrollY, scale, margins, this.UseNativeDrawing, this.SpaceBetweenLines);
                }

                if (bFinish && !borderInfo.bFinish && afterTextFormat != null)
                {
                    RectangleF aftertextRect = txtDraw.DrawRectangle;

                    aftertextRect.X = lexemX + lexemGroupWidth;
                    aftertextRect.Width = this.ClientRectangle.Width * 2 - this.ScrollOffsetRight - aftertextRect.X;

                    borderInfo.bFinish = true;
                    formatImpl.DrawBorder(g, ref borderInfo, (Format)afterTextFormat, ref aftertextRect);
                }

                if (bFinish && line.SubLinesCount - 1 != lex.SubLine && this.MarkLineWrapping)
                {
                    int xOffset = (RightToLeft == RightToLeft.Yes) ? txtDraw.DrawRectangle.Left - this.WrapMarkingImage.Width - 3 : txtDraw.DrawRectangle.Right + 1;
                    g.DrawImage(this.WrapMarkingImage,xOffset , line.Y + lex.YOffset);
                }

                if (bFinish)
                {
                    DrawWaveLines(g, pointLast, endPoint, x);
                    pointLast = null;
                }
            }

            if (lexems.Count == 0 && drawDynamicFormatting)
            {
                txtDraw.DrawRectangle.X = (int)x;
                txtDraw.DrawRectangle.Y = (int)Math.Round(y);
                txtDraw.DrawRectangle.Width = (int)(ClientRectangle.Width - x - ScrollOffsetRight);
                txtDraw.DrawRectangle.Height = (int)Math.Ceiling(line.Height);

                startPoint = new CoordinatePoint(m_parser, null, line.LineIndex, 1, false);
                endPoint = new CoordinatePoint(m_parser, null, line.LineIndex, 1, false);

                if (startPoint < endPoint)
                txtDraw.DynamicFormattings = GetAndConvertFormattings(startPoint, endPoint, out afterTextFormat);

                if (afterTextFormat != null)
                {
                    g.FillRectangle((afterTextFormat as Format).BackGroundBrush, txtDraw.DrawRectangle);
                }
            }

            if (m_bHighlightCurrentLine && line.LineIndex == this.CurrentLine)
            {
                using (Pen p = new Pen(Color.FromArgb(80, m_clrCurrentLineHighlight)))
                {
                    using (Brush br = new SolidBrush(Color.FromArgb(30, m_clrCurrentLineHighlight)))
                    {
                        int w = Math.Max(this.Width, this.VirtualSize.Width);
                        int left = (this.ShowMarkers) ? this.MarkerAreaOffset : 0;
                        if (this.RightToLeft == RightToLeft.No)
                            left = left + this.MarkerAreaWidth;
                        RectangleF rect = new RectangleF(left, y, w, line.Height + 1);
                        Region oldClip = g.Clip.Clone();
                        g.SetClip(rect, CombineMode.Replace);

                        g.FillRectangle(br, left, y + 1, w, line.Height - 1);
                        g.DrawLine(p, left, y, w, y);
                        g.DrawLine(p, left, y + line.Height, w, y + line.Height);

                        g.SetClip(oldClip, CombineMode.Replace);
                    }
                }
            }

            if (this.ShowWhitespaces && m_parser.TotalLines > line.LineIndex && this.ShowWhiteSpaceProperties.ShowNewLines)
            {
                txtDraw.Text = ShowWhiteSpaceProperties.NewLineString;
                Format whitespaces = (Format)m_parser.Formats[FormatType.Whitespace];
                TextInfo infoParagraph = whitespaces.MeasureText(g, txtDraw.Text, true, this.UseNativeDrawing, this.SpaceBetweenLines);
                txtDraw.DrawRectangle.X = (int)lastX;
                txtDraw.DrawRectangle.Y = (int)lastY;
                txtDraw.DrawRectangle.Width = (int)Math.Ceiling(infoParagraph.Width);
                txtDraw.DrawRectangle.Height = (int)Math.Ceiling(infoParagraph.Height);
                txtDraw.VerticalAlignment = StringAlignment.Near;
                txtDraw.DynamicFormattings = null;
                BorderInfo bi = new BorderInfo();
                whitespaces.DrawText(g, ref txtDraw, ref bi, autoScrollY, scale, margins, this.UseNativeDrawing, this.SpaceBetweenLines);
            }

            startPoint = null;
            endPoint = null;

            if (bDivider && m_bShowContentDividers)
            {
                float fDividerLineY = y + line.Height - 1;
                g.DrawLine(SystemPens.ControlDark, x, fDividerLineY, -x + Int16.MaxValue, fDividerLineY);
            }

            if (m_bShowUserMargin && DrawUserMarginText != null)
            {
                int userMarginoffset = (RightToLeft == RightToLeft.Yes) ? (0) : this.ClientRectangle.Right - m_iUserMarginWidth;

                int userMarginLeft = (m_userMarginPlacement == MarginPlacement.Right) ?
                    (userMarginoffset) : (this.LeftUserMarginOffset);
                Rectangle rectUserMargin = new Rectangle(userMarginLeft, (int)y, m_iUserMarginWidth, (int)line.Height);
                g.SetClip(rectUserMargin, CombineMode.Replace);
                DrawUserMarginTextEventArgs args = new DrawUserMarginTextEventArgs(g, rectUserMargin, line, m_fontUserMarginText, m_clrUserMarginText);
                GraphicsState grState = g.Save();
                DrawUserMarginText(this, args);
                g.Restore(grState);

                if (!args.CustomDraw && string.Empty != args.Text && null != args.Text)
                {
                    SolidBrush br = new SolidBrush(args.Color);
                    g.DrawString(args.Text, args.Font, br, rectUserMargin.X, rectUserMargin.Y);
                    br.Dispose();
                }
            }

            return width;
        }
        /// <summary>
        /// Paints foreground of the control
        /// </summary>
        /// <param name="e">PaintEventArgs.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (m_controlImage != null)
            {
                e.Graphics.DrawImage(m_controlImage, 0, 0);
            }
            else
            {
                float maxWidth = 0;
                m_scanTimer_Callback.Interval = DEF_IDLE_SLEEP_INTERVAL;

                lock (this)
                {
                    base.OnPaint(e);

                    try
                    {
                        if (m_updateLocks == 0 && m_parser != null)
                        {
                            Point autoScrollPosition = this.AutoScrollPosition;
                            Graphics g = e.Graphics;
                            m_parser.SetDPIFromGraphics(g);
                            g.TextRenderingHint = m_TextRenderingHint;
                            g.CompositingQuality = m_CompositingQuality;
                            g.InterpolationMode = m_InterpolationMode;
                            g.SmoothingMode = m_SmoothingMode;
                            g.RenderingOrigin = autoScrollPosition;

                            Rectangle clipRectangle = e.ClipRectangle;
                            Rectangle drawRect = clipRectangle;

                            clipRectangle.Height += 10;
                            clipRectangle.Width += 10;

                            g.SetClip(clipRectangle);

                            if (this.ScrollOffsetRight > 0)
                            {
                                RectangleF utilityRectRight =
                                    new RectangleF(this.ClientRectangle.Width - this.ScrollOffsetRight, 0, this.ScrollOffsetRight, this.ClientRectangle.Height);
                                g.SetClip(utilityRectRight, CombineMode.Exclude);
                            }

                            // Translates drawing according to current scrollers position.
                            g.TranslateTransform(0, autoScrollPosition.Y);

                            drawRect.Y -= autoScrollPosition.Y;

                            RenderedLine lineLastInRegion = m_parser.GetLineByY(drawRect.Bottom);
                            float fYAfterLastLine = 0;

                            if (lineLastInRegion != null)
                            {
                                fYAfterLastLine = lineLastInRegion.Y + lineLastInRegion.Height;
                            }

                            float iLastWidth = 0;

#if VERBOSE && DEBUG
          Debug.WriteLine( drawRect );
#endif

                            DrawAreaBackground(g, this.ClientRectangle, drawRect, false);

                            GraphicsState gs = g.Save();
                            g.ResetTransform();
                            g.SetClip(clipRectangle);
                            Rectangle margRect = DrawUserMarginArea(g, this.ClientRectangle);
                            g.Restore(gs);
                            g.SetClip(margRect, CombineMode.Exclude);

                            DrawTextArea(g, this.ClientRectangle);

                            //g.SetClip( drawRect );
                            RenderedLine lastLine =
                                DrawArea(g, drawRect, -autoScrollPosition.X, out iLastWidth, true, true, false, this.AutoScrollPosition.Y, 1, Size.Empty);

                            DrawColumnGuides(g);

                            maxWidth = Math.Max(iLastWidth, maxWidth);

                            // If line position has been chaged during drawing and remeasuring,
                            // then we should re-invalidate area, that is next to the last drawn line in region.
                            if (lineLastInRegion != null && lastLine != null && fYAfterLastLine != (lineLastInRegion.Y + lineLastInRegion.Height))
                            {
                                float fNextLineY = lastLine.Y + lastLine.Height;
                                Rectangle rectReInvalidate = new Rectangle(0, (int)fNextLineY, int.MaxValue, int.MaxValue);
                                InvalidateAll(rectReInvalidate);
                            }
                        }
                    }
                    //catch( Exception exc )
                    //{
                    //  Debug.WriteLine( "Paint failed, exception thrown :" + exc.Message );
                    //  Debug.WriteLine( "Source:" );
                    //  Debug.WriteLine( exc.Source );
                    //  Debug.WriteLine( "Stack:" );
                    //  Debug.WriteLine( exc.StackTrace );
                    //  throw exc;
                    //}
                    finally
                    {
                        e.Graphics.ResetTransform();
                    }

                    if (!this.WordWrap || m_wrapMode != WordWrapMode.Control)
                    {
                        // Update scrollers.
                        int iMaxWidth = (int)Math.Ceiling(maxWidth) /*- this.AutoScrollPosition.X*/;
                        int newWidth = this.VirtualSize.Width;

                        if (newWidth > this.MaxWidth)
                        {
                            newWidth = this.MaxWidth;
                        }
                        if (iMaxWidth > newWidth)
                        {
                            newWidth = iMaxWidth;
                        }

                        if (this.VirtualSize.Width != newWidth)
                        {
                            if (!m_bDisableScrollers)
                            {
                                this.VirtualSize = new Size(newWidth, this.VirtualSize.Height);
                            }
                            else
                            {
                                this.VirtualSize = Size.Empty;
                            }
                        }
                    }

                    UpdateScrollerVerticalSize(this, false);
                    UpdateScrollBarsSize();
                    UpdateScrollBarsVisibility();

                    if (m_bUpdateScrollInfo)
                    {
                        m_bUpdateScrollInfo = false;
                        UpdateScrollInfo();
                    }
                }
            }
        }
        /// <summary>
        /// Draws column guide lines.
        /// </summary>
        /// <param name="g">Graphics object to draw.</param>
        protected virtual void DrawColumnGuides(Graphics g)
        {
            if (m_bShowColumnGuides)
            {
                for (int i = 0; i < m_arrColumnGuideItems.Length; i++)
                {
                    string measuringString = new string('$', m_arrColumnGuideItems[i].Column);
                    int pos = (int)GraphicsUtils.MeasureString(
                        measuringString, m_columnGuidesMeasuringFont, false, g, string.Empty, this.UseNativeDrawing).Width + ScrollOffsetLeft + 1;
                    pos += this.AutoScrollPosition.X;

                    if(this.RightToLeft == RightToLeft.Yes)
                        pos = this.TextDrawOffset - pos;

                    Pen pen = new Pen(m_arrColumnGuideItems[i].Color);
                    g.DrawLine(pen, pos, -g.Transform.OffsetY, pos, this.Height - g.Transform.OffsetY);
                }
            }
        }
        /// <summary>
        /// Draws text area line and area situated after that line.
        /// </summary>
        /// <param name="g">Graphics object to draw things.</param>
        /// <param name="rectToDraw">Rectangle to draw.</param>
        protected internal virtual void DrawTextArea(Graphics g, Rectangle rectToDraw)
        {
            if (m_bShowTextArea)
            {
                int width = m_textAreaWidth + ScrollOffsetLeft + AutoScrollPosition.X;
                rectToDraw.Y -= (int)g.Transform.OffsetY;

                g.DrawLine(TextAreaLinePen, width, rectToDraw.Top, width, rectToDraw.Bottom);
                BrushPaint.FillRectangle(g, new Rectangle(width + 1, rectToDraw.Top, rectToDraw.Width - width, rectToDraw.Height), AfterTextAreaBrush);
            }
        }
        /// <summary>
        /// Draws user margin area.
        /// </summary>
        /// <param name="g">Graphics object, margin area should be drawn on.</param>
        /// <param name="clientrect">Client rectangle of the control, user margin should be drawn on.</param>
        protected internal virtual Rectangle DrawUserMarginArea(Graphics g, Rectangle clientrect)
        {
            if (m_bShowUserMargin)
            {
                int userMarginoffset = (RightToLeft == RightToLeft.Yes) ? (0) : this.ClientRectangle.Right - m_iUserMarginWidth;

                int x = (m_userMarginPlacement == MarginPlacement.Right) ?
                    (userMarginoffset) : (this.LeftUserMarginOffset);
                Rectangle userMarginRect = new Rectangle(x, 0, m_iUserMarginWidth, clientrect.Height);
                Region userMarginRegion = new Region(userMarginRect);
                userMarginRegion.Intersect(g.Clip);

                g.SetClip(userMarginRegion, CombineMode.Replace);
                userMarginRect.Height -= 1;
                userMarginRect.Width -= 1;

                BrushPaint.FillRectangle(g, userMarginRect, m_brushUserMargin);
                g.DrawRectangle(this.UserMarginAreaBorderPen, userMarginRect);

                RectangleF rectInternalF = userMarginRegion.GetBounds(g);
                Rectangle rectInternal = new Rectangle((int)rectInternalF.X + 1, (int)rectInternalF.Y + 1,
                    (int)rectInternalF.Width - 2, (int)rectInternalF.Height - 2);

                RaisePaintUserMarginEvent(g, rectInternal);
                userMarginRegion.Dispose();
                return userMarginRect;
            }
            else
            {
                return Rectangle.Empty;
            }
        }
        /// <summary>
        /// Used in Owner Drwan Line Numbers.
        /// </summary>
        private int maxLineNumberLength = 0;

        /// <summary>
        /// Draws some part of the text area.
        /// </summary>
        /// <param name="g">Graphics, text should be drawn on.</param>
        /// <param name="rect">Rectangle in the area to be drawn.</param>
        /// <param name="drawDynamicFormatting">Specifies whether dynamic formatting can be drawn.</param>
        /// <param name="drawIncompleteLines">Specifies whether just complete lines should be drawn.</param>
        /// <param name="iMaxLineWidthInArea">Width of the longest line in the drawn area.</param>
        /// <param name="xScroll">Shows how much text should be scrolled to the left.</param>
        /// <param name="bForPrint">Indicates whether g is printer's graphics.</param>
        /// <param name="autoScrollY">Y autoscroll position. Used for proper native drawing.</param>
        /// <param name="scale">Scale value for output. Used in printing for resolving printing problems related to native methods.</param>
        /// <param name="margins">Margin offsets for text output. Used in printing.</param>
        /// <returns>Last rendered line or null if no lines where rendered.</returns>
        /// <remarks>
        /// Given area will be drawn without using information about scrollers. To scroll area use transformation matrices of the Graphics object.
        /// </remarks>
        protected internal virtual RenderedLine DrawArea(Graphics g, RectangleF rect, float xScroll, out float iMaxLineWidthInArea,
            bool drawDynamicFormatting, bool drawIncompleteLines, bool bForPrint, int autoScrollY, float scale, Size margins)
        {
            RenderedLine lineCurrent = m_parser.GetLineByY(rect.Top);
            RenderedLine resultLine = lineCurrent;
            iMaxLineWidthInArea = 0;

            if (lineCurrent == null)
            {
                return null;
            }

            // Y coordinate of the line to be drawn.

            float lineY = lineCurrent.Y;
            // Calculate utility area iUtilityAreaWidth
            int iUtilityAreaWidth = (RightToLeft == RightToLeft.Yes) ? (int)(this.ScrollOffsetRight) : TextDrawOffset;

            // Text region - region, used for drawing text.
            Region textRegion = g.Clip.Clone();
            Region originalRegion = g.Clip.Clone();
            RectangleF rectTextRegion = textRegion.GetBounds(g);

            // Utility region - region used for drawing line numbers, bookmarks and collapsers.
            int utilityRectX = (this.RightToLeft == RightToLeft.Yes) ? (int)(rect.Right - iUtilityAreaWidth) : 0;
            RectangleF utilityRect = new RectangleF(utilityRectX, rectTextRegion.Top, iUtilityAreaWidth, rectTextRegion.Height);
            textRegion.Exclude(utilityRect);
            Region utilityRegion = new Region(utilityRect);

            bool bNeedNextLine = (lineCurrent != null && lineY < rect.Bottom);
            bool bNewLinesMeasured = false;
            float fLineZeroX = iUtilityAreaWidth - xScroll;

            if (bForPrint && RightToLeft == RightToLeft.Yes)
                fLineZeroX += 3;

            int iLinesRendered = 0;

            g.SetClip(textRegion, CombineMode.Replace);

            if (drawDynamicFormatting)
            {
                DrawIndentationBlockBackground(g, fLineZeroX);
            }

            if (!m_bTransparentSelection && !m_selection.IsEmpty() && m_selection.IsBlock())
            {
                using (GraphicsPath path = GetSelectedTextDrawPath())
                {
                    using (Matrix scrollerMatrix = new Matrix())
                    {
                        scrollerMatrix.Translate(-xScroll, 0);
                        path.Transform(scrollerMatrix);

                        using (Brush brush = new SolidBrush(m_parser.Formats[FormatType.SelectedText].BackColor))
                        {
                            g.FillPath(brush, path);
                        }
                        using (Pen pen = new Pen(m_parser.Formats[FormatType.SelectedText].BorderColor))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                }
            }

            while (bNeedNextLine)
            {
                iLinesRendered++;
                lineCurrent.Y = lineY;
                bNewLinesMeasured |= !lineCurrent.IsMeasured;

                g.SetClip(textRegion, CombineMode.Replace);
                float oldLineHeight = lineCurrent.Height;

                // draw line
                float lineWidth = DrawLexemLine(g, lineCurrent, fLineZeroX, lineY, drawDynamicFormatting, autoScrollY, scale, margins);
                g.SetClip(textRegion, CombineMode.Replace);
                DrawLineBorders(g, xScroll, lineCurrent);
                
                iMaxLineWidthInArea = Math.Max(iMaxLineWidthInArea, lineWidth + iUtilityAreaWidth);

                SmoothingMode smoothing = g.SmoothingMode;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.SetClip(utilityRect, CombineMode.Replace);

                // draw line number
                if (this.ShowLineNumbers)
                {
                    RectangleF rectLineNumber = new RectangleF(LineNumbersAreaOffset, lineY, m_lineNumbersWidth, lineCurrent.SubLineHeight[0]);
                    LineNumberPaintEventArgs e = new LineNumberPaintEventArgs(g, (lineCurrent.LineStartPoint.Line + m_virtualLineNumberOffset), m_lineNumbersFont, m_lineNumbersBrush.Color, this.LineNumbersAlignment, rectLineNumber, false);
                    if (OnBeforeLineNumberPaint!=null)
                    {
                        OnBeforeLineNumberPaint(this, e);
                    }

                    if (!e.Handled)
                    {
                        Color oldBrushColor = m_lineNumbersBrush.Color;
                        if (e.ForeColor != m_lineNumbersBrush.Color)
                            m_lineNumbersBrush.Color = e.ForeColor;
                        
                        if (maxLineNumberLength < e.LineNumber.ToString().Length)
                        {
                            maxLineNumberLength = e.LineNumber.ToString().Length;
                            RecalculateSpaces(Convert.ToInt32(e.LineNumber));
                            this.Invalidate();
                        }

                        //if (e.LineNumber.ToString().Length != lineCurrent.LineStartPoint.Line.ToString().Length)
                        //    RecalculateSpaces((int)e.LineNumber);

                        g.DrawString(e.LineNumber.ToString(), e.LineNumbersFont, m_lineNumbersBrush, e.Bounds, e.StringFormat);
                        m_lineNumbersBrush.Color = oldBrushColor;
                    }
                }

                // Draw Collapsing Rectangles
                if (this.ShowCollapse)
                {
                    DrawCollapseRect(lineCurrent, g, bForPrint);
                }

                // Draw bookmarks
                if (this.ShowMarkers)
                {
                    IntPtr handle = (this.IsHandleCreated) ? this.Handle : IntPtr.Zero;
                    Bookmarks.DrawBookmark(lineCurrent, g, handle, this.UseXPStyle, bForPrint);
                }

                g.SmoothingMode = smoothing;

                // Get next line info
                lineY += lineCurrent.Height;
                lineCurrent = GetNextPreRenderedLine(lineCurrent);

                if (lineCurrent != null)
                {
                    resultLine = lineCurrent;
                }

                bNeedNextLine = (lineCurrent != null && lineY < rect.Bottom &&
                    (drawIncompleteLines || (lineY + lineCurrent.GetSubLineTextHeight(0)) <= rect.Bottom));
            }

            g.SetClip(textRegion, CombineMode.Replace);

            if (this.DragDropRectangle != RectangleF.Empty)
            {
                RectangleF rectDragDrop = this.DragDropRectangle;
                g.DrawRectangle(SystemPens.WindowFrame, rectDragDrop.X + this.TextDrawOffset + this.AutoScrollPosition.X, rectDragDrop.Y, rectDragDrop.Width - 1, rectDragDrop.Height - 1);
            }

            if (drawDynamicFormatting)
            {
                DrawIndentGuideline(g, fLineZeroX);
            }
            if (m_codeSnippetsManager.Activated)
            {
                m_codeSnippetsManager.MarkCodeSnippet(g);
            }

#if !NO_TIMING && VERBOSE
      Debug.WriteLine( string.Format( "Lines rendered: {0}", iLinesRendered ) );
#endif

            // Update y coordinate of all lines next to the last one if needed.
            if (lineCurrent != null && Math.Abs(lineY - lineCurrent.Y) >= 0.5f)
            {
                int iLineFirst = Math.Max(1, lineCurrent.LineIndex - DEF_LINES_PRERENDER);
                int iLineLast = Math.Min(Parser.TotalLines, lineCurrent.LineIndex + DEF_LINES_PRERENDER);

                for (int indexPrerender = iLineFirst; indexPrerender <= iLineLast; indexPrerender++)
                {
                    RenderedLine linePrerender = Parser.GetLine(indexPrerender) as RenderedLine;
                    Parser.MeasureLine(linePrerender);
                }

                int offset = 0;
                int scrollTop = -AutoScrollPosition.Y + ScrollOffsetTop;
                RenderedLine line = m_parser.GetLineByY(scrollTop);
                if (line == null)
                {
                    line = this.CurrentLineInstanceInternal;
                }

                offset = (int)(line.Y - scrollTop);
                FixLineRenderingPositions();
                InvalidateAll();
                Point currentScrollPoint = new Point(-AutoScrollPosition.X, (int)(line.Y - offset));
                if (AutoScrollPosition.Y > currentScrollPoint.Y)
                    AutoScrollPosition = new Point(-AutoScrollPosition.X, (int)(line.Y - offset));
            }

            if (drawDynamicFormatting)
            {
                DrawTransparentSelection(g, xScroll, textRegion, utilityRegion);
            }

            g.SetClip(originalRegion, CombineMode.Replace);

            textRegion.Dispose();
            utilityRegion.Dispose();
            originalRegion.Dispose();

            if (bNewLinesMeasured && (null != m_CursorManager))
            {
                m_CursorManager.Update();
            }

            return resultLine;
        }
        /// <summary>
        /// Draw indent Guideline.
        /// </summary>
        /// <param name="g">Graphics object.</param>
        /// <param name="xOffset">xOffset of the Guidelines.</param>
        protected virtual void DrawIndentGuideline(Graphics g, float xOffset)
        {
            if (this.IndentGuideline != null && this.IndentGuideline.RectLexemEnd.Y != -1 && this.ShowIndentationGuidelines)
            {
                Color fillColor = Color.FromArgb(40, m_colorIndentBorders);
                Color borderColor = m_colorIndentBorders;
                Brush brushRect = new SolidBrush(fillColor);
                Brush brushLine = new SolidBrush(m_colorIndentLine);

                Pen penRect = new Pen(borderColor);
                Pen penLine = new Pen(brushLine);
                penLine.DashStyle = DashStyle.Dot;

                RectangleF rectStart = IndentGuideline.RectLexemStart;
                rectStart.X = Math.Max(rectStart.X, 1) + xOffset;
                rectStart = GetRectangleInRTL(rectStart);

                RectangleF rectEnd = IndentGuideline.RectLexemEnd;
                rectEnd.X = Math.Max(rectEnd.X, 1) + xOffset;
                rectEnd = GetRectangleInRTL(rectEnd);

                if (!m_bOnlyHighlightMatchingBraces)
                {
                    RectangleF rectClipBounds = g.ClipBounds;

                    float x, y1, y2;

                    x = (RightToLeft == RightToLeft.Yes) ? (rectEnd.Right - 1) : rectEnd.X;
                    
                    y1 = Math.Min(rectStart.Bottom, rectClipBounds.Bottom);
                    y2 = Math.Max(rectEnd.Top, rectClipBounds.Top);

                    g.DrawLine(penLine,x,y1,x,y2);
                }

                FillRectWithBorder(g, brushRect, penRect, rectStart);
                FillRectWithBorder(g, brushRect, penRect, rectEnd);

                brushRect.Dispose();
                brushLine.Dispose();
                penRect.Dispose();
                penLine.Dispose();

                if (m_bShowIndentationBlockBorders)
                {
                    rectStart = IndentGuideline.RectLexemStart;
                    rectEnd = IndentGuideline.RectLexemEnd;
                    rectEnd.X += rectEnd.Width;

                    GraphicsPath path = GetTextDrawPath(rectStart, rectEnd);
                    Matrix scrollerMatrix = new Matrix();
                    scrollerMatrix.Translate(AutoScrollPosition.X, 0);
                    path.Transform(scrollerMatrix);

                    if (null != path)
                    {
                        GraphicsUtils.DrawPath(g, path, m_indentationBlockBorderStyle, m_clrIndentationBlockBorder);
                    }
                }
            }
        }
        /// <summary>
        /// Draws indentation block background.
        /// </summary>
        /// <param name="g">Graphics.</param>
        /// <param name="xOffset">X offset.</param>
        protected void DrawIndentationBlockBackground(Graphics g, float xOffset)
        {
            if (m_bShowIndentationBlockBorders && IndentGuideline != null)
            {
                RectangleF rectStart = IndentGuideline.RectLexemStart;
                RectangleF rectEnd = IndentGuideline.RectLexemEnd;
                rectEnd.X += rectEnd.Width;

                GraphicsPath path = GetTextDrawPath(rectStart, rectEnd, true);

                Matrix scrollerMatrix = new Matrix();
                
                if (RightToLeft == RightToLeft.Yes)
                {
                    MirrorGraphicsPath(ref path, (this.TextDrawOffset + ScrollOffsetRight) /*Client Rect Width*/);
                    scrollerMatrix.Translate((this.TextDrawOffset - ScrollOffsetRight - 2), 0);
                }

                scrollerMatrix.Translate(AutoScrollPosition.X, 0);
                path.Transform(scrollerMatrix);

                RectangleF pathRect = path.GetBounds();
                Region oldClip = g.Clip.Clone();

                g.SetClip(new Region(path), CombineMode.Replace);
                if (null != path)
                {
                    BrushPaint.FillRectangle(g, pathRect, m_indentationBlockBackgroundBrush);
                }
                g.SetClip(oldClip, CombineMode.Replace);
            }
        }
        /// <summary>
        /// Fills rectangle using specified brush and draws it's border using specified pen.
        /// </summary>
        /// <param name="g">Graphics object.</param>
        /// <param name="pen">Pen to be used for drawing border.</param>
        /// <param name="brush">Brush to be used for filling rectangle.</param>
        /// <param name="rect">Rectangle to be filled and bordered.</param>
        protected virtual void FillRectWithBorder(Graphics g, Brush brush, Pen pen, RectangleF rect)
        {
            g.FillRectangle(brush, rect);
            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
        }
        /// <summary>
        /// Draws background of the control area.
        /// </summary>
        /// <param name="g">Graphics, text should be drawn on.</param>
        /// <param name="rectClient">Client rectangle.</param>
        /// <param name="rect">Rectangle in the area to be drawn.</param>
        /// <param name="bForPrint">Indicates whether drawing isbeing performed for printing.</param>
        protected internal virtual void DrawAreaBackground(Graphics g, Rectangle rectClient, RectangleF rect, bool bForPrint)
        {
            if (g == null) throw new ArgumentNullException("g");

            int iMarkers = (this.ShowMarkers) ? (m_markerAreaWidth) : (0);
            int iLines = (this.ShowLineNumbers) ? (m_lineNumbersWidth) : (0);
            int iCollapse = (this.ShowCollapse) ? (DEF_COLLAPSE_AREA) : (0);
            int iSelectionMargin = (m_bSelectionMarginVisible) ? (m_iSelectionMarginWidth) : (0);

            float height = rect.Height;

            RectangleF rcMarks = new RectangleF(this.MarkerAreaOffset, rect.Top, iMarkers, height);
            RectangleF rcLines = new RectangleF(this.LineNumbersAreaOffset, rect.Top, iLines, height);
            RectangleF rcColl = new RectangleF(this.CollapsingAreaOffset, rect.Top, iCollapse, height);
            RectangleF rcSelMargin = new RectangleF(this.SelectionMarginOffset, rect.Top, iSelectionMargin + 1, height);

            Brush brushSelectionMargin = ((FormatManager)Language).GetBrush(m_colorSelectionMarginBackground);

            if (m_backgroundBrush == null)
            {
                m_backgroundBrush = new SolidBrush(BackColor);
            }

            bool bGradient = BackgroundColor.Style != BrushStyle.None || BackgroundColor.Style != BrushStyle.Solid;
            if (!bForPrint)
            {
                Rectangle rectFill = rectClient;
                rectFill.Y -= (int)g.Transform.OffsetY;
                rectFill.X -= (int)g.Transform.OffsetX;

                if (bGradient)
                {
                    rectFill.X -= iLines + iCollapse;
                    rectFill.Width += iLines + iCollapse;
                }

                BrushPaint.FillRectangle(g, rectFill, BackgroundColor);
            }

            if (ShowLineNumbers && !bGradient)
            {
                g.FillRectangle(m_backgroundBrush, rcLines);
                g.DrawLine(m_GreenDotsPen, rcLines.Right - 1, rect.Top, rcLines.Right - 1, rect.Top + height);
            }

            if (this.UseXPStyle)
            {
                if (XPStyle.XPThemesEnabled() && !bForPrint)
                {
                    Rectangle intRect = new Rectangle((int)(rcMarks.X + g.Transform.OffsetX), (int)(rcMarks.Y + g.Transform.OffsetY),
                        (int)rcMarks.Width, (int)rcMarks.Height);
                    if (this.indicatorMarginColor != Color.Empty)
                    {
                        SolidBrush brush = new SolidBrush(IndicatorMarginColor);
                        g.FillRectangle(brush, rcMarks);
                    }
                    else
                        XPStyle.Draw(this.Handle, g, intRect, "Scrollbar", 6, 3);
                }
                else
                {
                    if (this.indicatorMarginColor != Color.Empty)
                    {
                        SolidBrush brush = new SolidBrush(IndicatorMarginColor);
                        g.FillRectangle(brush, rcMarks);
                    }
                    else
                    {
                        BrushPaint.FillRectangle(g, rcMarks, m_brushMarkersArea);
                        if (ShowMarkers)
                        {
                            g.DrawLine(new Pen(Color.LightGray), rcMarks.Right - 1, rect.Top, rcMarks.Right - 1, rect.Top + height);
                        }
                    }
                }
            }
            else
            {
                if (this.indicatorMarginColor != Color.Empty)
                {
                    SolidBrush brush = new SolidBrush(IndicatorMarginColor);
                    g.FillRectangle(brush, rcMarks);
                }
                else
                {
                    g.FillRectangle(SystemBrushes.Control, rcMarks);
                    if (ShowMarkers)
                    {
                        g.DrawLine(m_GreenDotsPen, rcMarks.Right - 1, rect.Top, rcMarks.Right - 1, rect.Top + height);
                    }
                }
            }

            if (rcColl.Width > 0 && !bGradient)
            {
                g.FillRectangle(m_backgroundBrush, rcColl);
            }
            if (m_bSelectionMarginVisible)
            {
                g.FillRectangle(brushSelectionMargin, rcSelMargin);
            }
        }
        private bool multiEndBlock = true;
        /// <summary>
        /// Draws rectangle with plus or minus sign in the collapsers area if line supports collapsing.
        /// </summary>
        /// <param name="line">Line, to draw sign for.</param>
        /// <param name="g">Graphics object, where sign can be drawn.</param>
        /// <param name="bForPrint">Indicates whether g is printer's graphics.</param>
        protected virtual void DrawCollapseRect(RenderedLine line, Graphics g, bool bForPrint)
        {
            if (line == null) throw new ArgumentNullException("line");
            if (g == null) throw new ArgumentNullException("g");

            if (this.ShowCollapse)
            {
                bool bHaveCollapses = false;
                bool bDrawPlus = false;
                bool bDrawUpperPartOfLine = false;
                object[] startStackData = line.LineStartStack.ToArray();

                for (int i = startStackData.Length - 1; i >= 0; i--)
                {
                    IStackData datum = (IStackData)startStackData[i];
                    ICollapsableConfigLexem config = (datum.FirstConfig == null) ?
                        (datum.Config as ICollapsableConfigLexem) : (datum.FirstConfig as ICollapsableConfigLexem);

                    if (config != null && config.IsCollapsable)
                    {
                        bDrawUpperPartOfLine = true;
                        break;
                    }
                }

                bool bDrawLowerPartOfLine = false;
                object[] endStackData = line.LineEndStack.ToArray();

                for (int i = endStackData.Length - 1; i >= 0; i--)
                {
                    IStackData datum = (IStackData)endStackData[i];
                    ICollapsableConfigLexem config = (null == datum.FirstConfig) ?
                        (datum.Config as ICollapsableConfigLexem) : (datum.FirstConfig as ICollapsableConfigLexem);

                    if (null != config && config.IsCollapsable)
                    {
                        bDrawLowerPartOfLine = true;
                        break;
                    }
                }

                bool bEndOfCollapsing = false;

                foreach (ILexem lex in line.LineLexems)
                {
                    LexemCollapsingType collapsingtype = GetLexemCollapsingType(lex);
                    switch (collapsingtype)
                    {
                        case LexemCollapsingType.CollapsedLexem:
                            bDrawPlus = true;
                            bHaveCollapses = true;
                            break;

                        case LexemCollapsingType.RegionStart:
                            bHaveCollapses = true;
                            break;

                        case LexemCollapsingType.RegionEnd:
                            bEndOfCollapsing = true;
                            break;
                    }
                    if (lex.Config.EndBlock != null)
                    {
                        if (lex.Text == "Then " || lex.Text == "then ")
                        {
                            bHaveCollapses = false;
                            bEndOfCollapsing = false;
                        }
                    }
                }

                if (bDrawUpperPartOfLine && !bDrawLowerPartOfLine)
                {
                    bEndOfCollapsing = true;
                }

                if (bHaveCollapses || bDrawUpperPartOfLine || bDrawLowerPartOfLine || bEndOfCollapsing)
                {
                    Rectangle collapseRect =
                        new Rectangle(this.CollapsingAreaOffset + 2, (int)(line.Y + 4), DEF_COLLAPSE_AREA - 3, DEF_COLLAPSE_AREA - 3);
                    int centerY = collapseRect.Top + collapseRect.Height / 2;
                    int centerX = collapseRect.Left + collapseRect.Width / 2;

                    if (bDrawUpperPartOfLine)
                    {
                        if (!bHaveCollapses)
                        {
                            g.DrawLine(Pens.Gray, centerX, line.Y, centerX, centerY);
                        }
                        else
                        {
                            g.DrawLine(Pens.Gray, centerX, line.Y, centerX, centerY - 4);
                        }
                    }
                    if (bDrawLowerPartOfLine)
                    {
                        if (!bHaveCollapses)
                        {
                            g.DrawLine(Pens.Gray, centerX, centerY, centerX, line.Y + line.Height);
                        }
                        else
                        {
                            g.DrawLine(Pens.Gray, centerX, centerY + 4, centerX, line.Y + line.Height);
                        }
                    }

                    if (bEndOfCollapsing)
                    {
                        int offset =DEF_COLLAPSE_AREA / 2;
                        offset *= (this.RightToLeft == RightToLeft.Yes) ? - 1 : 1;

                        g.DrawLine(Pens.Gray, centerX, centerY, centerX + offset, centerY);
                    }
                    if (bHaveCollapses)
                    {
                        DrawCollapseIcon(g, collapseRect, bDrawPlus, bForPrint);
                    }
                }
            }
        }
        /// <summary>
        /// Draws transparent selection.
        /// </summary>
        /// <param name="g">Graphics.</param>
        /// <param name="xScroll">Shows how much text should be scrolled to the left.</param>
        /// <param name="regionText">Region, used for drawing text.</param>
        /// <param name="regionUtility">Region used for drawing line numbers, bookmarks and collapsers.</param>
        protected virtual void DrawTransparentSelection(Graphics g, float xScroll, Region regionText, Region regionUtility)
        {
            if (g == null) throw new ArgumentNullException("g");
            if (regionText == null) throw new ArgumentNullException("regionText");
            if (regionUtility == null) throw new ArgumentNullException("regionUtility");

            GraphicsPath path = GetSelectedTextDrawPath();

            if (path != null)
            {
                Matrix scrollerMatrix = new Matrix();

                if (this.RightToLeft == RightToLeft.Yes)
                {
                    scrollerMatrix.Translate(this.TextDrawOffset - ScrollOffsetRight - 2 + xScroll, 0);
                    MirrorGraphicsPath(ref path, this.TextDrawOffset + ScrollOffsetRight /*Client Rect With */);
                }
                else
                {
                    scrollerMatrix.Translate(-xScroll, 0);
                }

                if (m_bTransparentSelection)
                {
                    g.SetClip(regionText, CombineMode.Replace);
                    path.Transform(scrollerMatrix);
                    Color color = Color.FromArgb(30, m_selectionTextColor);
                    Color color1 = Color.FromArgb(80, m_selectionTextColor);

                    Pen p = new Pen(color1);
                    Brush b = new SolidBrush(color);
                    g.FillPath(b, path);
                    b.Dispose();
                    g.DrawPath(p, path);
                    p.Dispose();
                }

                if (m_bSelectionMarginVisible)
                {
                    g.SetClip(regionUtility, CombineMode.Replace);
                    RectangleF rect = path.GetBounds(scrollerMatrix);

                    rect.X = SelectionMarginOffset;
                    rect.Width = m_iSelectionMarginWidth;

                    Pen pen = ((FormatManager)Language).GetPen(m_colorSelectionMarginForeground);
                    Brush brush = ((FormatManager)Language).GetBrush(m_colorSelectionMarginForeground);

                    DrawCorner(g, pen, rect, false, false);
                    DrawCorner(g, pen, rect, true, false);
                    DrawCorner(g, pen, rect, false, true);
                    DrawCorner(g, pen, rect, true, true);

                    rect.X += 2;
                    rect.Width -= 3;
                    rect.Y += 2;
                    rect.Height -= 3;

                    g.FillRectangle(brush, rect);
                }

                path.Dispose();
            }
        }
        /// <summary>
        /// Flips the GraphicsPath against X axis.
        /// </summary>
        /// <param name="path">The GraphicsPath to transform</param>
        /// <param name="width">Width of the area to be transformed</param>
        /// <remarks>This will flip the GraphicsPath againts the X axis.
        /// Use ShiftGraphicsPath to relocate the GraphicsPath</remarks>
        protected void MirrorGraphicsPath(ref GraphicsPath path, int width)
        {
            path.Transform(new Matrix(-1, 0, 0, 1, width, 0));
        }
        /// <summary>
        /// Moves the Graphics path by the specified amount.
        /// </summary>
        /// <param name="path">The GraphicsPath to shift</param>
        /// <param name="OffsetX">Offset for the X axis</param>
        /// <param name="OffsetY">Offset for the Y axis</param>
        protected Matrix ShiftGraphicsPath(ref GraphicsPath path, float OffsetX, float OffsetY)
        {
            Matrix locator = new Matrix();
            locator.Translate(OffsetX,OffsetY);
            
            path.Transform(locator);

            return locator;
        }

        /// <summary>
        /// Draws corner of the selection margin.
        /// </summary>
        /// <param name="g">Graphics object.</param>
        /// <param name="pen">Pen to be used for drawing.</param>
        /// <param name="rect">Selection margin rectangle.</param>
        /// <param name="bRight">True of corner is on the right.</param>
        /// <param name="bBottom">True of corner is on the bottom..</param>
        protected virtual void DrawCorner(Graphics g, Pen pen, RectangleF rect, bool bRight, bool bBottom)
        {
            PointF[] points = new PointF[3];
            points[0].X = bRight ? rect.Right : rect.Left;
            points[0].Y = bBottom ? (rect.Bottom - 2) : (rect.Y + 2);
            points[1].X = bRight ? rect.Right : rect.Left;
            points[1].Y = bBottom ? rect.Bottom : rect.Y;
            points[2].X = bRight ? (rect.Right - 1) : (rect.Left + 1);
            points[2].Y = bBottom ? rect.Bottom : rect.Y;
            g.DrawLines(pen, points);
        }
        /// <summary>
        /// Draws WaveLines.
        /// </summary>
        /// <param name="g">Graphics object.</param>
        /// <param name="start">Start coordinate of the line.</param>
        /// <param name="end">End coordinate of the line.</param>
        /// <param name="xScroll">X coordinate of the line start.</param>
        protected virtual void DrawWaveLines(Graphics g, CoordinatePoint start, CoordinatePoint end, float xScroll)
        {
            if (g == null) throw new ArgumentNullException("g");
            if (start == null) throw new ArgumentNullException("start");
            if (end == null) throw new ArgumentNullException("end");
            if (start.VirtualLine != end.VirtualLine) throw new ArgumentException(
                   Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_90);
            if (start.VirtualColumn > end.VirtualColumn) throw new ArgumentException(
                   Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_91);

            IDynamicFormatsLayer wavelines = m_formatManager[DEF_LAYER_NAME_WAVELINE];
            IList list = wavelines[start, end];

            for (int i = 0; i < list.Count; i++)
            {
                IDynamicFormat format = list[i] as IDynamicFormat;
                Format underlineFormat = format.Format as Format;
                if (RightToLeft == RightToLeft.Yes)
                {
                    underlineFormat.RightToLeft = true;
                    underlineFormat.TextDrawOffset = this.TextDrawOffset + this.ScrollOffsetRight;
                }

                if (underlineFormat == null) throw new NotSupportedException(
                       Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_92);

                CoordinatePoint pStart = format.Start;
                CoordinatePoint pEnd = format.End;

                if (pStart.IsValid && pEnd.IsValid)
                {
                    if (pStart < start || pStart.VirtualLine > start.VirtualLine)
                    {
                        pStart = start;
                    }
                    if (pEnd > end)
                    {
                        pEnd = end;
                    }

                    RectangleF rectStart = m_parser.VirtualToGraphical(new Point(pStart.VirtualColumn, pStart.VirtualLine));
                    RectangleF rectEnd = m_parser.VirtualToGraphical(new Point(pEnd.VirtualColumn - 1, pEnd.VirtualLine));
                    RectangleF underLineRect = new RectangleF(rectStart.X + xScroll, rectStart.Y, rectEnd.Right - rectStart.X, rectStart.Height);
                    underlineFormat.DrawUnderlines(g, underLineRect);
                }
            }
        }
        /// <summary>
        /// Draws collapse icon using XP style id possible.
        /// </summary>
        /// <param name="g">Graphics object to draw.</param>
        /// <param name="rect">Rectangle where icon should be drawn.</param>
        /// <param name="bPlus">true if plus should be drawn, false if minus.</param>
        /// <param name="bForPrint">Indicates whether g is printer's graphics.</param>
        protected virtual void DrawCollapseIcon(Graphics g, Rectangle rect, bool bPlus, bool bForPrint)
        {
            int centerY = rect.Top + rect.Height / 2;
            int centerX = rect.Left + rect.Width / 2;

            if (this.UseXPStyle && XPStyle.XPThemesEnabled() && !bForPrint)
            {
                Rectangle transformedRect = new Rectangle(rect.Location, rect.Size);
                transformedRect.Y += (int)g.Transform.OffsetY;
                transformedRect.X += (int)g.Transform.OffsetX;

                if (bPlus)
                {
                    XPStyle.Draw(this.Handle, g, transformedRect, "Treeview", 2, 1);
                }
                else
                {
                    XPStyle.Draw(this.Handle, g, transformedRect, "Treeview", 2, 2);
                }
                return;
            }

            rect.Width--;
            rect.Height--;

            if (this.UseXPStyle)
            {
                BrushPaint.FillRectangle(g, rect, m_brushCollapseIcons);
                g.DrawRectangle(Pens.DarkGray, rect);

                g.DrawLine(Pens.Black, rect.Left + 1, centerY, rect.Right - 1, centerY);
                if (bPlus)
                {
                    g.DrawLine(Pens.Black, centerX, rect.Top + 1, centerX, rect.Bottom - 1);
                }

                return;
            }

            g.DrawRectangle(Pens.Gray, rect);
            g.DrawRectangle(Pens.Gray, rect);

            g.DrawLine(Pens.Gray, rect.Left + 1, centerY, rect.Right - 1, centerY);
            if (bPlus)
            {
                g.DrawLine(Pens.Gray, centerX, rect.Top + 1, centerX, rect.Bottom - 1);
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Updates background brush.
        /// </summary>
        /// <param name="e">EventArgs.</param>
        protected override void OnBackColorChanged(EventArgs e)
        {
            lock (this)
            {
                if (m_backgroundBrush != null)
                {
                    m_backgroundBrush.Dispose();
                }
                base.OnBackColorChanged(e);
                m_backgroundBrush = new SolidBrush(BackColor);
            }
        }
        /// <summary>
        /// Overrides the OnRightToLeftChanged event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);

            if(this.HScrollBar.InnerScrollBar != null)
                this.HScrollBar.InnerScrollBar.RightToLeft = this.RightToLeft;
            if (this.VScrollBar.InnerScrollBar != null)
                this.VScrollBar.InnerScrollBar.RightToLeft = this.RightToLeft;

        }

        /// <summary>
        /// Raised when control gets input focus.
        /// </summary>
        /// <param name="e">EventArgs.</param>
        protected override void OnGotFocus(EventArgs e)
        {
            lock (this)
            {
                base.OnGotFocus(e);
                StopSelection();
            }
        }
        /// <summary>
        /// Raised when control loses input focus.
        /// </summary>
        /// <param name="e">EventArgs.</param>
        protected override void OnLostFocus(EventArgs e)
        {
            lock (this)
            {
                base.OnLostFocus(e);
                StopSelection();
            }
        }
        /// <summary>
        /// Windows Messages Handler.
        /// </summary>
        /// <param name="m">Message.</param>
        [UIPermission(SecurityAction.Assert, Unrestricted = true, Window = UIPermissionWindow.AllWindows)]
        [SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            bool bGoUp = true;

            // preprocessing.
            switch ((Msg)m.Msg)
            {
                //case Msg.WM_PAINT:
                //  {
                //    if( m_controlImage != null )
                //    {
                //      bGoUp = false;
                //    }
                //    break;
                //  }

                case Msg.WM_IME_STARTCOMPOSITION:
                    {
                        IntPtr inputContext = WinAPI.ImmGetContext(this.Handle);
                        tagCOMPOSITIONFORM compWnd = new tagCOMPOSITIONFORM();
                        compWnd.dwStyle = (int)tagCOMPOSITIONFORM_Position.CFS_FORCE_POSITION;
                        compWnd.ptCurrentPos = new POINT(this.CursorGraphicalLocation.X,
                            this.CursorGraphicalLocation.Y - m_CursorManager.CursorGraphicalCoordinates.Rectangle.Height);
                        WinAPI.ImmSetCompositionWindow(inputContext, ref compWnd);
                        break;
                    }

#if SyncfusionFramework2_0
                // Workaround for defect OT#4147: in .NET2.0 WM_CHAR that comes after WM_IME_CHAR calls OnKeyPress() which causes char
                // to be inserted twice, because WM_IME_CHAR calls OnKeyPress() itself. On the level of System.Forms.Control
                // WM_IME_STARTCOMPOSITION resets flag responsible for not calling OnKeyPress() on WM_CHAR, and WM_CHAR comes after it.
                case Msg.WM_IME_CHAR:
                    {
                        iCatchWmChar++;
                        break;
                    }

                case Msg.WM_CHAR:
                    {
                        if (iCatchWmChar > 0)
                        {
                            bGoUp = false;
                            iCatchWmChar--;
                        }
                        break;
                    }
#endif
            }

            if (bGoUp)
            {
                base.WndProc(ref m);
            }

            // postprocessing.
            switch ((Msg)m.Msg)
            {
                case Msg.WM_GETDLGCODE:
                    {
                        int result = (int)DialogCodes.DLGC_WANTCHARS | (int)DialogCodes.DLGC_WANTARROWS | m.Result.ToInt32();
                        if (!m_bTransferFocusOnTab || ContextChoiceOn)
                        {
                            result |= (int)DialogCodes.DLGC_WANTTAB;
                        }
                        result |= (int)DialogCodes.DLGC_WANTALLKEYS;
                        m.Result = new IntPtr(result);
                        break;
                    }
            }
        }
        /// <summary>
        /// Raised when user pushes down some key. All key-presses should be processed by KeyBinder.
        /// </summary>
        /// <param name="e">KeyEventArgs.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            m_position = Control.MousePosition;
            if (m_tip != null)
            {
                m_tip.Hide();
            }
            if (e.KeyData == Keys.Tab && m_bTransferFocusOnTab && !ContextChoiceOn)
            {
                m_bKeyPressHandled = false;
                m_bKeyUpHandled = false;
                base.OnKeyDown(e);

                if (this.GetContainerControl() is EditControl)
                {
                    EditControl ctrl = this.GetContainerControl() as EditControl;
                    this.TopLevelControl.SelectNextControl(ctrl, true, true, true, true);
                }
                else
                {
                    Control ctrl = this.GetContainerControl() as Control;
                    this.TopLevelControl.SelectNextControl(ctrl, true, true, true, true);
                }
                m_bKeyPressHandled = true;
                m_bKeyUpHandled = true;
                e.Handled = true;
            }
            else if (e.KeyData == (Keys.Shift | Keys.Tab) && m_bTransferFocusOnTab && !ContextChoiceOn)
            {
                m_bKeyPressHandled = false;
                m_bKeyUpHandled = false;
                base.OnKeyDown(e);
                this.StopSelection();

                if (this.GetContainerControl() is EditControl)
                {
                    EditControl ctrl = this.GetContainerControl() as EditControl;
                    this.TopLevelControl.SelectNextControl(ctrl, false, true, true, true);
                }
                else
                {
                    Control ctrl = this.GetContainerControl() as Control;
                    this.TopLevelControl.SelectNextControl(ctrl, false, true, true, true);
                }

                m_bKeyPressHandled = true;
                m_bKeyUpHandled = true;
                e.Handled = true;
            }
            else
            {
                lock (this)
                {
                    HideIndentGuideline();

                    bool bContextChoiceVisibleBefore = ContextChoiceOn;
                    DragDropRectangle = RectangleF.Empty;

                    m_bKeyPressHandled = false; // workaround for defect 3948

                    base.OnKeyDown(e);

                    m_bKeyPressHandled = true;
                    if (!e.Handled)
                    {
                        // if auto complete form shown then it must process keys first
                        if (ContextChoiceOn)
                        {
                            if (ContextPromptOn)
                            {
                                switch (m_IntellisenseType)
                                {
                                    case IntellisenseType.ContextPrompt:
                                        e.Handled = m_contextPrompt.ProcessKey(e.KeyData);
                                        break;

                                    case IntellisenseType.ContextChoice:
                                        m_controllerContextChoice.ProccessKeys(e);
                                        break;
                                }
                            }
                            else
                            {
                                m_controllerContextChoice.ProccessKeys(e);
                            }
                        }

                        if (!e.Handled && ContextPromptOn)
                        {
                            e.Handled = m_contextPrompt.ProcessKey(e.KeyData);
                        }

                        if (m_codeSnippetsPopupController.IsVisible)
                        {
                            m_codeSnippetsPopupController.ProccessKeys(e);
                        }

                        if (!e.Handled)
                        {
                            m_codeSnippetsManager.ProcessKeys(e);
                        }

                        // if key not processable then send it to keys binder
                        if (!e.Handled)
                        {
                            if (e.KeyData == Keys.Tab)
                            {
                                //Adjust selection to make tab work in case of selection from bottom to top or right to left.
                                //Also this will work in case of the above selection between different lines.
                                if (this.SelectedText != string.Empty && !this.SelectedText.Contains("\n"))
                                {
                                    if (Selection.Start.VirtualLine == Selection.End.VirtualLine)
                                    {
                                        if (Selection.Start.VirtualColumn > Selection.End.VirtualColumn)
                                        {
                                            this.SetSelection(Selection.End.VirtualColumn, Selection.End.VirtualLine, Selection.Start.VirtualColumn, Selection.Start.VirtualLine);
                                        }
                                    }

                                    if (Selection.Start.VirtualLine > Selection.End.VirtualLine)
                                    {
                                        this.SetSelection(Selection.End.VirtualColumn, Selection.End.VirtualLine, Selection.Start.VirtualColumn, Selection.Start.VirtualLine);
                                    }

                                    ClearSelectedTextForTab();
                                }
                            }

                            if (Control.ModifierKeys != Keys.Alt && Control.ModifierKeys != (Keys.Alt | Keys.Control)) //Fix for def. OT6855.
                            {
                                m_keyBinder.ProcessKey(e.KeyData);
                            }
                            else
                            {
                                m_bKeyPressHandled = false;
                            }

                            if (e.KeyCode == Keys.ShiftKey && !m_controllerContextChoice.AutoCompleteStringShown)
                            {
                                StartSelection();
                            }

                            if (this.ContextChoiceOn && bContextChoiceVisibleBefore &&
                                Keys.Control != e.KeyData && Keys.Alt != e.KeyData && e.KeyCode != Keys.ShiftKey) // For proper work of auto complete.
                            {
                                UpdateContextChoice();
                            }

                            if (this.ContextPromptOn)
                            {
                                UpdateContextPrompt();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clears the Selected text when the user press the tab key.
        /// </summary>
        private void ClearSelectedTextForTab()
        {
            this.DeleteText(this.Selection.Start, this.Selection.End);
            this.TabifySelection();
        }

        /// <summary>
        /// Raised when some pressed key was released.
        /// </summary>
        /// <param name="e">KeyEventArgs.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            lock (this)
            {
                base.OnKeyUp(e);

                if (!e.Handled && !m_bKeyUpHandled)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.ShiftKey:
                            StopSelection();
                            break;
                    }
                }
            }
            isToolTipOn = false;
        }
        /// <summary>
        /// Raised when user presses some key.
        /// </summary>
        /// <param name="e"><see cref="KeyPressEventArgs"/> instance with information about key-press.</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            lock (this)
            {
                m_scanTimer_Callback.Interval = DEF_IDLE_SLEEP_INTERVAL;

                base.OnKeyPress(e);

                if (!e.Handled && m_parser != null && !m_bKeyPressHandled)
                {
                    if (m_codeSnippetsManager.Activated)
                    {
                        m_codeSnippetsManager.ProcessKeys(e);
                    }

                    StopSelection();
                    ProcessIntellisenseKey(e);

                    if (!e.Handled)
                    {
                        ProcessAutoReplace(e.KeyChar);
                        InsertChar(e.KeyChar);
                        ProcessIntellisenseKey(e);
                    }
                }
            }
        }
        /// <summary>
        /// Raised when mouse is moved over the control.
        /// </summary>
        /// <param name="e"><see cref="MouseEventArgs"/> with information about mouse movement.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            isToolTipOn = false;
            if(m_position!=Control.MousePosition)
                isToolTipOn = true;
            lock (this)
            {
                Point point = new Point(e.X, e.Y);
                bool bMouseDown = (e.Button != MouseButtons.None);
                bool bInSelectionMargin = (bMouseDown) ? (m_bClickedInSelectionMargin) : (PointBelongToSelectionArea(point));
                bool bInText = (bMouseDown) ? (m_bClickedInText) : (PointBelongToTextArea(point) && !bInSelectionMargin);

                if (!IsIntellyScrollActive)
                {
                    bool bInSelection = PointBelongsToSelection(point);

                    if (bInText && (!bInSelection || bMouseDown))
                    {
                        SpecialCursor = null;
                    }
                    else if (bInText && bInSelection && !bMouseDown)
                    {
                        SpecialCursor = StreamEditControl.SelectLineCursor;
                    }
                    else if (bInSelectionMargin || m_iLastLineInLineSelection > 0)
                    {
                        SpecialCursor = StreamEditControl.SelectLineCursor;
                    }
                    else if (!bInText && !bInSelection && !bMouseDown)
                    {
                        SpecialCursor = StreamEditControl.SelectLineCursor;
                    }
                    else
                    {
                        SpecialCursor = this.DefaultCursor;
                    }
                }
                base.OnMouseMove(e);

                if (!bMouseDown)
                {
                    if (this.IsSelecting)
                    {
                        StopSelection();
                    }
                }
                else if (m_bClickedInSelection && GetDistance(m_pointClickPosition.X - e.X, m_pointClickPosition.Y - e.Y) > 2)
                {
                    m_bClickedInSelection = false;
                    m_bDraggingSelectedText = true;
                    m_rangeDragging = (ComplexTextRange)m_selection.Clone();

                    foreach (TextRange range in m_rangeDragging.Ranges)
                    {
                        range.End.AttachToEvents = range.Start.AttachToEvents = true;
                    }

                    string text = this.SelectedText;
                    if (this.Selection.IsBlock())
                    {
                        SetBlockClipboardData(text);
                    }

                    DragResult = this.DoDragDrop(this.SelectedText, DragDropEffects.Move | DragDropEffects.Copy);

                    if (m_bDraggingSelectedText && DragResult == DragDropEffects.Move)
                    {
                        UndoGroupOpen();

                        if (m_bDraggingSelectedText)
                        {
                            StopSelection();
                            SelectionCancel();
                        }

                        DeteleSelectedTextOnDrop();

                        UndoGroupClose();
                        InvalidateAll();

                        DragResult = DragDropEffects.None;
                        m_bDraggingSelectedText = false;
                    }
                }
                else if (this.IsSelecting && bInText)
                {
                    Point loc = new Point(e.X, e.Y);
                    if (loc != m_mouseDownPoint) // fix for def. OT6310.
                    {
                        Point pt = GetGraphicalCoordinateInRTL(e.X, e.Y);
                        SetCursorToPoint(pt.X,pt.Y);
						//Invalidate(); //Fix for SD5135
                    }
                }
                else if (m_bDoubleClicked && bInText) // fixing of def. OT4736.
                {
                    Point p = PointToVirtualPosition(point);
                    if (p != m_doubleClickedPoint)
                    {
                        m_bDoubleClicked = false;
                        this.CurrentPosition = m_doubleClickedPoint;
                        StartSelection();
                        m_bSelecting = true;
                        if (p != Point.Empty)
                            this.CurrentPosition = p;
                    }
                }
                else if (m_iLastLineInLineSelection > 0)
                {
                    Point pointVirtual = m_CursorManager.PositionConverter.GraphicalToVirtual(new Point(1, e.Y - AutoScrollPosition.Y), VirtualSpaceMode);

                    if (!pointVirtual.IsEmpty)
                    {
                        bool bSecondLineIsBeforeFirstLine = (pointVirtual.Y < m_iLastLineInLineSelection);                      
                        CoordinatePoint endPoint = GetNearestParsePointRight(pointVirtual.Y, this.GetLineLength(pointVirtual.Y));
                        CoordinatePoint startPoint =
                            GetNearestParsePointLeft(m_iLastLineInLineSelection + ((bSecondLineIsBeforeFirstLine) ? (1) : (0)), 1);

                        if (!m_selection.Exists() || m_selection.Start != startPoint || m_selection.End != endPoint)
                        {
                            StopSelection();
                            SelectionCancel();
                            SetSelectionStart(startPoint);
                            SetSelectionEnd(endPoint);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Called when mouse button is pressed.
        /// </summary>
        /// <param name="e">MouseEventArgs.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_mouseDownPoint = new Point(e.X, e.Y);
            }

            if (e.Clicks > 1)
            {
                DoubleClickFromMouseDown(EventArgs.Empty);
            }
            else
            {
                m_codeSnippetsManager.Activated = false;
                m_bClickedInSelection = false;

                lock (this)
                {
                    if (m_parser == null)
                    {
                        base.OnMouseDown(e);
                    }
                    else
                    {
                        m_controllerContextChoice.Close();

                        if (this.ContextPromptOn)
                        {
                            m_contextPrompt.Close();
                        }

                        Point mousePoint = new Point(e.X, e.Y);

                        bool bInSelectionMargin = PointBelongToSelectionArea(mousePoint);
                        bool bIsInText = PointBelongToTextArea(mousePoint) && !bInSelectionMargin;

                        m_bClickedInSelectionMargin = bInSelectionMargin;
                        m_bClickedInText = bIsInText;

                        if (bIsInText || bInSelectionMargin)
                        {
                            bool bClickInSelection = PointBelongsToSelection(mousePoint);

                            if (!bClickInSelection && bIsInText)
                            {
                                if (!this.IsSelecting)
                                {
                                    SelectionCancel();
                                }

                                Point pt = GetGraphicalCoordinateInRTL(e.X, e.Y);

                                SetCursorToPoint(pt.X,pt.Y);
                                StartSelection();
                            }
                            else if ((e.Button == MouseButtons.Left) && bClickInSelection)
                            {
                                m_bClickedInSelection = true;
                                m_pointClickPosition = new Point(e.X, e.Y);
                            }

                            if ((e.Button == MouseButtons.Left) && bInSelectionMargin)
                            {
                                Point pointVirtual = m_CursorManager.PositionConverter.GraphicalToVirtual(
                                    new Point(1, e.Y - AutoScrollPosition.Y), VirtualSpaceMode);
                                m_iLastLineInLineSelection = pointVirtual.Y;
                                StopSelection();
                                SelectionCancel();
                                StartSelection();
                            }
                        }
                        else
                        {
                            if (this.ShowCollapse && e.X >= CollapsingAreaOffset && e.X < CollapsingAreaOffset + DEF_COLLAPSE_AREA)
                            {
                                // Click on collapser
                                ILexemLine line = m_parser.GetLineByY(e.Y - AutoScrollPosition.Y);
                                if (line != null)
                                {
                                    ProcessClickOnLineCollapse(line);
                                }
                            }
                        }
                    }
                }
            }

            base.OnMouseDown(e);
        }

        private Point GetGraphicalCoordinateInRTL(int x, int y)
        {
            Point pt = new Point(x,y);

            if (this.RightToLeft == RightToLeft.Yes)
            {
                x = Width-ScrollOffsetRight - ScrollOffsetLeft -CursorManager.CursorGraphicalCoordinates.Size.Width - x - 2;
                pt = new Point(x,y);
            }

            return pt;
            
        }
        private bool IsCurrentKeyboardLayoutRTL()
        {
            StringBuilder sbKLID = new StringBuilder(WinAPI.KL_NAMELENGTH);

            if (WinAPI.GetKeyboardLayoutName(sbKLID))
            {
                int klid = int.Parse(
                   sbKLID.ToString().Substring(0, WinAPI.KL_NAMELENGTH - 1),
                   System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.InvariantCulture);

                // strip all but the bottom half of the number
                klid &= 0xffff;

                return new System.Globalization.CultureInfo(klid, false).TextInfo.IsRightToLeft;
            }

            return false;
        }
        private RectangleF GetRectangleInRTL(RectangleF rect)
        {
            float x = rect.X;
            if (this.RightToLeft == RightToLeft.Yes)
            {
                x = (this.TextDrawOffset + this.ScrollOffsetRight) - x - rect.Width;
                rect.X = x;
            }
            return rect;
        }

        /// <summary>
        /// Raised when mouse button is released.
        /// </summary>
        /// <param name="e"><see cref="MouseEventArgs"/> with information about mouse.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            StopSelection();
            m_iLastLineInLineSelection = -1;
            m_bDraggingSelectedText = false;
            m_bDoubleClicked = false;
            m_rangeDragging = null;

            if (m_bClickedInSelection && GetDistance(m_pointClickPosition.X - e.X, m_pointClickPosition.Y - e.Y) <= 2)
            {
                m_bClickedInSelection = false;
                SetCursorToPoint(e.X, e.Y);
            }
        }
        /// <summary>
        /// Processes double-click, selects word.
        /// </summary>
        /// <param name="e">EventArgs.</param>
        protected virtual void DoubleClickFromMouseDown(EventArgs e)
        {
            lock (this)
            {
                Point pointMouse = PointToClient(Control.MousePosition);
                bool bMouseInText = PointBelongToTextArea(pointMouse);
                bool bMouseInIndicatorMargin = PointBelongToIndicatorMargin(pointMouse);


                if (bMouseInText)
                {
                    RenderedLine line = m_parser.GetLine(this.CurrentLine) as RenderedLine;

                    if (line != null)
                    {
                        IRenderedLexem lexem = line.FindLexemByColumn(this.CurrentColumn) as IRenderedLexem;

                        if (lexem != null)
                        {
                            CoordinatePoint start = m_parser.GetCoordinatePoint(this.CurrentLine, lexem.Column);
                            CoordinatePoint end = m_parser.GetCoordinatePoint(this.CurrentLine, lexem.Column + lexem.Length);

                            m_CursorManager.Visible = false;
                            m_CursorManager.CursorVirtualCoordinates.Position = new Point(end.VirtualColumn, end.VirtualLine);
                            StopSelection();
                            SelectionCancel();
                            SetSelectionStart(start);
                            SetSelectionEnd(end);
                            m_CursorManager.Visible = true;
                            m_bDoubleClicked = true;
                            m_doubleClickedPoint = PointToVirtualPosition(pointMouse);
                        }
                    }

                    // Click event should be raised only when user clicks in the text area.
                    //base.OnDoubleClick(e);
                }
                else if (bMouseInIndicatorMargin)
                {
                    pointMouse.X += TextDrawOffset;
                    Point pointVirtual = PointToVirtualPosition(pointMouse);

                    if (!pointVirtual.IsEmpty && null != IndicatorMarginDoubleClick)
                    {
                        IndicatorClickEventArgs args = new IndicatorClickEventArgs(pointVirtual.Y, Bookmarks.GetCustomBookmark(pointVirtual.Y));
                        IndicatorMarginDoubleClick(this, args);
                    }
                }
            }
        }
        /// <summary>
        /// Processes mouse clicks.
        /// </summary>
        /// <param name="e">EventArgs.</param>
        protected override void OnClick(EventArgs e)
        {
            Point pointMouse = PointToClient(MousePosition);
            bool bMouseInText = PointBelongToTextArea(pointMouse);
            bool bMouseInIndicatorMargin = PointBelongToIndicatorMargin(pointMouse);

            if (bMouseInText)
            {
                base.OnClick(e);
            }

            if (bMouseInIndicatorMargin)
            {
                pointMouse.X += TextDrawOffset;
                Point pointVirtual = PointToVirtualPosition(pointMouse);
                this.CurrentLine = pointVirtual.Y;
                RenderedLine line = this.GetLine(pointVirtual.Y) as RenderedLine;
                IParsePoint point = line.LineStartPoint;
                if (!pointVirtual.IsEmpty && IndicatorMarginDoubleClick != null)
                {
                    IndicatorClickEventArgs args = new IndicatorClickEventArgs(point.Line, Bookmarks.GetCustomBookmark(point.Line));
                    IndicatorMarginClick(this, args);
                }
            }
        }
        /// <summary>
        /// Hides and shows cursor on scrolling.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="se">ScrollEventArgs.</param>
        protected override void OnHScrollInternal(object sender, ScrollEventArgs se)
        {
            CloseIntellisense();
            m_CursorManager.Visible = false;
            base.OnHScrollInternal(sender, se);
            m_CursorManager.Visible = true;
        }
        /// <summary>
        /// Hides and shows cursor on scrolling.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="se">ScrollEventArgs.</param>
        protected override void OnVScrollInternal(object sender, ScrollEventArgs se)
        {
            CloseIntellisense();
            m_CursorManager.Visible = false;
            base.OnVScrollInternal(sender, se);
            m_CursorManager.Visible = true;
        }
        /// <summary>
        /// Processes horizontal scroll event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="se">ScrollEventArgs.</param>
        protected override void OnHScroll(object sender, ScrollEventArgs se)
        {
            lock (this)
            {
                if (IsRedrawingRequiredBackground())
                {
                    InvalidateAll();
                }
                else
                {
                    if (this.RightToLeft == RightToLeft.Yes)
                    {
                        m_tracerInvalidation.Invalidate(this.ClientRectangle);
                    }
                }
                base.OnHScroll(sender, se);
            }
        }
        /// <summary>
        /// Processes vertical scroll event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="se">ScrollEventArgs.</param>
        protected override void OnVScroll(object sender, ScrollEventArgs se)
        {
            lock (this)
            {
                if (IsRedrawingRequiredBackground())
                {
                    InvalidateAll();
                }
                else
                {
                    int yAmount = (this.VScrollBar.Value - se.NewValue);

                    if (!this.ShowUserMargin || this.UserMarginPlacement == MarginPlacement.Right)
                    {
                        Rectangle bounds = new Rectangle(0, this.ScrollOffsetTop,
                            this.ScrollOffsetLeft, this.ClientRectangle.Height - this.ScrollOffsetTop - this.ScrollOffsetBottom);
                        ScrollWindow(0, yAmount, bounds, bounds);

                        if (this.ShowUserMargin)
                        {
                            bounds = new Rectangle(this.ClientRectangle.Width - this.ScrollOffsetRight + 1, this.ScrollOffsetTop + 1, this.ScrollOffsetRight - 2,
                                this.ClientRectangle.Height - this.ScrollOffsetTop - this.ScrollOffsetBottom - 2);
                            ScrollWindow(0, yAmount, bounds, bounds);
                        }
                    }
                    else
                    {
                        Rectangle bounds = new Rectangle(0, this.ScrollOffsetTop,
                            this.LeftUserMarginOffset - 1, this.ClientRectangle.Height - this.ScrollOffsetTop - this.ScrollOffsetBottom);
                        ScrollWindow(0, yAmount, bounds, bounds);
                        bounds = new Rectangle(this.CollapsingAreaOffset, this.ScrollOffsetTop,
                            this.ScrollOffsetLeft - this.CollapsingAreaOffset, this.ClientRectangle.Height - this.ScrollOffsetTop - this.ScrollOffsetBottom);
                        ScrollWindow(0, yAmount, bounds, bounds);
                        bounds = new Rectangle(this.LeftUserMarginOffset, this.ScrollOffsetTop + 1,
                            this.UserMarginWidth, this.ClientRectangle.Height - this.ScrollOffsetTop - this.ScrollOffsetBottom - 2);
                        ScrollWindow(0, yAmount, bounds, bounds);
                    }
                }
                base.OnVScroll(sender, se);
            }
        }
        /// <summary>
        /// Performs size changing-related operations.
        /// </summary>
        /// <param name="e">EventArgs.</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            lock (this)
            {
                if (m_bShowUserMargin)
                {
                    InvalidateAll();
                }
            }
        }

        /// <summary>
        /// Serialize all collapsible regions in currently selected area or in the current line.
        /// </summary>
        public void SerializeCollapse()
        {
            RenderedLine line;
            bool bNeedUnCollapse;
            bool bNeedCollapse;
            lock (this)
            {
                using (((ILongOperationController)this).StartOperation("Collapsings toggle"))
                {
                    m_codeSnippetsManager.Activated = false;
                    m_listCollapsed.Clear();
                    m_listNotCollapsed.Clear();


                    if (!m_selection.IsEmpty() && m_selection.Start != m_selection.End)
                    {
                        int bottomLine = m_selection.Bottom.VirtualLine;
                        for (int i = m_selection.Top.VirtualLine; i < bottomLine; i++)
                        {
                            line = m_parser.GetLine(i) as RenderedLine;
                            if (!this.NeedCollapse(line))
                            {
                                continue;
                            }
                            FillLineCollapsers(line, m_listCollapsed, m_listNotCollapsed);
                        }
                    }
                    else
                    {
                        line = m_parser.GetLine(CurrentLine) as RenderedLine;
                        if (!this.NeedCollapse(line))
                        {
                            return;
                        }
                        FillLineCollapsers(line, m_listCollapsed, m_listNotCollapsed);
                    }

                    bNeedUnCollapse = (m_listCollapsed.Count > 0);
                    bNeedCollapse = (m_listNotCollapsed.Count > 0);


                    StopSelection();
                    SelectionCancel();
                    HideIndentGuideline();

                    if (!(bNeedUnCollapse || bNeedCollapse))
                    {
                        IParsePoint point = GetNearestParsePointLeft(CurrentLine, CurrentColumn).PhysicalPoint;

                        CollapsableRegion region = m_parser.GetOuterCollapsableRegion(point, false);

                        if (region == null)
                            return;


                        line = m_parser.GetLine(CurrentLine) as RenderedLine;

                        if (!this.NeedCollapse(line))
                        {
                            return;
                        }

                        FillLineCollapsers(line, m_listCollapsed, m_listNotCollapsed);

                        bNeedUnCollapse = (m_listCollapsed.Count > 0);
                        bNeedCollapse = (m_listNotCollapsed.Count > 0);

                    }

                    m_parser.UpdateLineInformation();

                    ProcessCollapsing(m_listNotCollapsed, true);

                }

            }
        }

        /// <summary>
        /// Scrolls control vertically by specified amount of lines.
        /// </summary>
        /// <param name="fLinesCount">Count of lines to scroll.</param>
        /// <param name="direction">Direction of scrolling.</param>
        protected override void ScrollLines(ScrollDirection direction, float fLinesCount)
        {
            lock (this)
            {
                TimeCounter timeMeasure = new TimeCounter();
                timeMeasure.Start();
                RenderedLine firstLine = InternalGetLineByVirtualY((direction == ScrollDirection.Up) ? (0) : (ClientRectangle.Bottom - 1));

                if (firstLine != null)
                {
                    float fScrollAmount = 0;
                    fLinesCount = Math.Min(fLinesCount, (direction == ScrollDirection.Up) ?
                        (firstLine.LineIndex - 1) : (m_parser.TotalLines - firstLine.LineIndex));
                    int iLinesCount = (int)Math.Floor(fLinesCount);
                    ScrollEventType type = ScrollEventType.SmallDecrement;

                    if (direction == ScrollDirection.Up)
                    {
                        fScrollAmount = (firstLine.Y + this.AutoScrollPosition.Y);

                        if (fLinesCount - iLinesCount > 0)
                        {
                            fScrollAmount -= InternalGetLine(firstLine.LineIndex - iLinesCount - 1, false).Height * (fLinesCount - iLinesCount);
                        }

                        fScrollAmount += InternalGetLine(firstLine.LineIndex - iLinesCount, false).Y - firstLine.Y;
                    }
                    else
                    {
                        fScrollAmount = (firstLine.Y + firstLine.Height + this.AutoScrollPosition.Y - ClientRectangle.Height);

                        if (fLinesCount - iLinesCount > 0 && firstLine.LineIndex - iLinesCount + 1 > 0)
                        {
                            fScrollAmount += (m_parser.GetLine(firstLine.LineIndex - iLinesCount + 1) as RenderedLine).Height * (fLinesCount - iLinesCount);
                        }

                        for (int i = firstLine.LineIndex + 1; i <= firstLine.LineIndex + iLinesCount; i++)
                        {
                            fScrollAmount += InternalGetLine(i, true).Height;
                        }
                    }

                    float iValue = VScrollBar.Value;
                    float newValue = iValue + (int)Math.Ceiling(fScrollAmount);
                    newValue = Math.Max(0, Math.Min(newValue, VScrollBar.Maximum - VScrollBar.LargeChange + 1));
                    ScrollEventArgs args = new ScrollEventArgs(type, 0);
                    int moveSign = Math.Sign(newValue - iValue);

                    if (moveSign != 0)
                    {
                        float scrollingStep;

                        if (IsRedrawingRequiredBackground())
                        {
                            scrollingStep = DEF_LINE_SCROLLING_STEP_SLOW;
                        }
                        else
                        {
                            scrollingStep = DEF_LINE_SCROLLING_STEP_FAST;
                        }

                        float oneMoveSize = 0;

                        if (this.VScrollMode == ScrollMode.Pixel)
                            oneMoveSize = Math.Abs((iValue - newValue) / fLinesCount / (Math.Max(scrollingStep - (fLinesCount - 3) / 3 * 2, 1)));
                        else
                            oneMoveSize = newValue;

                        timeMeasure.Finish();
                        float prepareTime = timeMeasure.Result;
                        timeMeasure.Start();
                        do
                        {
                            iValue += oneMoveSize * moveSign;
                            args.NewValue = (int)Math.Ceiling((moveSign > 0) ? (Math.Min(iValue, newValue)) : (Math.Max(iValue, newValue)));
                            OnVScrollInternal(this, args);
                            Update();
                        }
                        while ((iValue - newValue) * moveSign <= 0);

                        timeMeasure.Finish();
                        float scrollTime = timeMeasure.Result;

#if !NO_TIMING && VERBOSE
      Debug.WriteLine( fLinesCount, "Scrolled lines" );
      Debug.WriteLine( prepareTime, "Preparation time" );
      Debug.WriteLine( scrollTime, "Scrolling time" );
#endif
                    }
                }
            }
        }
        /// <summary>
        /// Called before scrolling by timer on every timer tick. 
        /// </summary>
        protected override void AfterAutoScroll()
        {
            lock (this)
            {
                Point point = PointToClient(MousePosition);
                Rectangle ClientRectangle = new Rectangle(ScrollOffsetLeft, ScrollOffsetTop,
                    this.ClientRectangle.Width - ScrollOffsetLeft - ScrollOffsetRight, this.ClientRectangle.Height - ScrollOffsetTop - ScrollOffsetBottom);

                if (!ClientRectangle.Contains(point))
                {
                    bool bWasInSelection = IsSelecting;
                    IsSelecting &= (m_iLastLineInLineSelection == -1);
                    SetCursorToPoint(point.X, point.Y);
                    IsSelecting = bWasInSelection;

                    if (m_bDraggingSelectedText)
                    {
                        m_selection = m_rangeDragging;
                        IsSelecting = false;
                    }
                    Update();
                }
            }
        }
        /// <summary>
        /// Updates line wrapping info.
        /// </summary>
        /// <param name="levent">LayoutEventArgs.</param>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (ContextChoiceOn)
            {
                m_controllerContextChoice.Close(true);
            }

            base.OnLayout(levent);

            lock (this)
            {
                RemeasureLinesWrapping();
            }
        }
        /// <summary>
        /// Changes drag effect to copy if some file is dragged over the control.
        /// </summary>
        /// <param name="drgevent">DragEventArgs.</param>
        protected override void OnDragOver(DragEventArgs drgevent)
        {
            Point mousePoint = new Point(drgevent.X, drgevent.Y);
            mousePoint = PointToClient(mousePoint);
            bool bInText = PointBelongToTextArea(mousePoint);
            bool bClickInSelection = false;
            if (m_bDraggingSelectedText)
            {
                bClickInSelection = PointBelongsToSelection(mousePoint) || PointToVirtualPosition(mousePoint) == m_selection.Bottom.VirtualPoint;
                drgevent.Effect = (bClickInSelection || !bInText) ? (DragDropEffects.None) : (DragDropEffects.Copy | DragDropEffects.Move);
            }
            else
            {
                if (bInText)
                {
                    if (CheckForSupportedData(drgevent.Data))
                    {
                        drgevent.Effect = DragDropEffects.Copy & drgevent.AllowedEffect;
                    }
                }
                else
                {
                    drgevent.Effect = DragDropEffects.None;
                }
            }
            if (DragDropEffects.None != drgevent.Effect && (m_bInsertDroppedFileIntoText || !drgevent.Data.GetDataPresent(DataFormats.FileDrop)))
            {
                Point point = PointToClient(new Point(drgevent.X, drgevent.Y));
                point = PointToVirtualPosition(point);

                if (Point.Empty != point)
                {
                    RectangleF rect = m_parser.VirtualToGraphical(point);
                    rect.Width = 3;
                    DragDropRectangle = rect;
                }
            }
            else
            {
                DragDropRectangle = RectangleF.Empty;
            }

            if (drgevent.KeyState == 9 && ((drgevent.Effect & DragDropEffects.Copy) == DragDropEffects.Copy))
                drgevent.Effect = DragDropEffects.Copy;
            else if ((drgevent.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move && !bClickInSelection)
                drgevent.Effect = DragDropEffects.Move;
            else if (bClickInSelection)
                drgevent.Effect = DragDropEffects.None;
            base.OnDragOver(drgevent);
        }
        /// <summary>
        /// Inserts text from the dropped to the control file.
        /// </summary>
        /// <param name="drgevent">DragEventArgs.</param>
        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            if (CheckForSupportedData(drgevent.Data))
            {
                Point point = PointToClient(new Point(drgevent.X, drgevent.Y));
                CurrentPosition = PointToVirtualPosition(point);

                UndoGroupOpen();

                if (m_bDraggingSelectedText)
                {
                    StopSelection();
                    SelectionCancel();
                }

                PasteData(drgevent.Data);
                if (m_bDraggingSelectedText && !((Control.ModifierKeys | Keys.Control) == Control.ModifierKeys))
                {
                    DeteleSelectedTextOnDrop();
                }
                UndoGroupClose();
                InvalidateAll();
            }

            base.OnDragDrop(drgevent);
            m_bDraggingSelectedText = false;
            m_rangeDragging = null;
            DragDropRectangle = RectangleF.Empty;
        }
        /// <summary>
        /// Sets m_rectDragOverPosition to empty rectangle.
        /// </summary>
        /// <param name="e">EventArgs.</param>
        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);
            DragDropRectangle = RectangleF.Empty;
        }
        /// <summary>
        /// Checks whether given key can be processed by control.
        /// </summary>
        /// <param name="keyData">Keys to check.</param>
        /// <returns>True if keyData is input key; otherwise false.</returns>
        protected override bool IsInputKey(Keys keyData)
        {
            Keys keyCode = keyData & Keys.KeyCode;

            switch (keyCode)
            {
                case Keys.Tab:
                    if (!TransferFocusOnTab) return true;
                    break;

                case Keys.Return:
                    if (!SingleLineMode) return true;
                    break;
            }

            return base.IsInputKey(keyData);
        }
        /// <summary>
        /// Updates scrollbar sizes.
        /// </summary>
        protected override void UpdateScrollBarsSize()
        {
            if (!DisableScrollers)
                base.UpdateScrollBarsSize();
            else
            {
                VScrollBar.LargeChange = 0;
                HScrollBar.LargeChange = 0;
            }
        }
        /// <summary>
        /// Updates visibility of the ScrollBars.
        /// </summary>
        protected internal override void UpdateScrollBarsVisibility()
        {
            if (!DisableScrollers)
            {
                if (m_bAlwaysShowScrollers)
                {
                    this.VScroll = this.HScroll = true;
                }
                else
                {
                    bool bShowGripper = CheckForGripper(this);

                    if (bShowGripper)
                    {
                        VScroll = !DisableVerticalScroller;
                        HScroll = !DisableHorizontalScroller;
                    }
                    else
                    {
                        base.UpdateScrollBarsVisibility();
                    }
                }
            }
            else
            {
                HScroll = VScroll = false;
            }
        }
        /// <summary>
        /// Raises PrintHeader event.
        /// </summary>
        /// <param name="g">Graphics.</param>
        /// <param name="bounds">Bounds of header.</param>
        /// <returns>Height of header.</returns>
        protected virtual int OnPrintHeader(Graphics g, Rectangle bounds)
        {
            PrintHeadlineEventArgs e = new PrintHeadlineEventArgs(g, bounds, m_iPrintPageNumber, string.Empty);

            if (PrintHeader != null)
            {
                PrintHeader(this, e);
                return e.HeadlineHeight;
            }

            return 0;
        }
        /// <summary>
        /// Raises PrintFooter event.
        /// </summary>
        /// <param name="g">Graphics.</param>
        /// <param name="bounds">Bounds of footer.</param>
        /// <returns>Height of footer.</returns>
        protected virtual int OnPrintFooter(Graphics g, Rectangle bounds)
        {
            PrintHeadlineEventArgs e = new PrintHeadlineEventArgs(g, bounds, m_iPrintPageNumber, string.Empty);

            if (PrintFooter != null)
            {
                PrintFooter(this, e);
                return e.HeadlineHeight;
            }

            return 0;
        }
        //		/// <summary>
        //    /// Checks whether Autoscrolling can be started when user presses mouse button down.
        //    /// </summary>
        //    /// <returns>If return value is true, autoscrolling will be allowed.</returns>
        //protected override bool CheckIfCanStartAutoscroll()
        //{
        //  Point point = PointToClient( MousePosition );
        //  bool result = base.CheckIfCanStartAutoscroll() || PointBelongToSelectionArea( point );

        //  return result;
        //}
        /// <summary>
        /// Changes cursor to IBeam.
        /// </summary>
        /// <param name="e">EventArgs.</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (m_bAllowMouseCursorChange)
            {
                this.Cursor = Cursors.IBeam;
            }
        }
        /// <summary>
        /// Works with autoscroll position in single line mode.
        /// </summary>
        public override Point AutoScrollPosition
        {
            get
            {
                return base.AutoScrollPosition;
            }
            set
            {
                if (!m_bDisableScrollers)
                {
                    base.AutoScrollPosition = value;
                    if (this.SingleLineMode)
                    {
                        m_CursorManager.Update();
                    }
                }
                else
                {
                    base.AutoScrollPosition = Point.Empty;
                }

                // Fix for single line mode.
                if (DisableScrollers)
                    InvalidateAll();
            }
        }
        /// <summary>
        /// Gets autoscroll rectangle.
        /// </summary>
        protected override Rectangle AutoScrollRectangle
        {
            get
            {
                Rectangle result = base.AutoScrollRectangle;

                if (m_bShowUserMargin)
                {
                    result.Width += m_iUserMarginWidth;
                }

                return result;
            }
        }
        /// <summary>
        /// Indicates whether cursor changing is allowed.
        /// </summary>
        protected override bool IsCursorChangingAllowed
        {
            get
            {
                return !isShowing;// !m_menuManager.IsShown);
            }
        }
        #endregion

        #region ISupportInitialize Members
        /// <summary>
        /// Performs actions needed before initialization.
        /// </summary>
        public void BeginInit()
        {
            LockUpdate();
            if (m_keyBinder != null)
            {
                m_keyBinder.UnprocessedKey -= new ProcessCommandsEventHandler(OnUnprocessedKeyPress);
            }
        }
        /// <summary>
        /// Initializes key bindings.
        /// </summary>
        public void EndInit()
        {
            if (m_bLoadConfigFile)
            {
                if (m_Configuration != null)
                {
                    m_Configuration.Dispose();
                }

                //this.Configurator = new Config(this.DesignMode, true);
                //ResetColoring(m_Configuration.DefaultLanguage);
            }

            if (m_keyBinder == null)
            {
                m_keyBinder = new KeyProcessor();
                m_keyBinder.InitializeClassDefaults(this);
            }
            else
            {
                m_keyBinder.InitializeCommandsList(this);
                RegisterDefaultKeyBindings();
            }

            m_keyBinder.UnprocessedKey += new ProcessCommandsEventHandler(OnUnprocessedKeyPress);

            CurrentPosition = new Point(1, 1);
            ResetUndoInfo();
            OnConfigurationChanged();
            UnlockUpdate();
        }
        #endregion

        #region Context Menu Event Handlers
        /// <summary>
        /// Handles Options context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerOptions(object sender, EventArgs e)
        {
            if (m_contextMenuOptionsForm != null)
            {
                CloseIntellisense();

                (m_contextMenuOptionsForm as Form).RightToLeft = this.RightToLeft;

                m_contextMenuOptionsForm.ShowDialog();
            }
            isShowing = false;
        }
        /// <summary>
        /// Handles Edit->Cut context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerCut(object sender, EventArgs e)
        {
            Cut();
            isShowing = false;
        }
        /// <summary>
        /// Handles Edit->Copy context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerCopy(object sender, EventArgs e)
        {
            Copy();
            isShowing = false;
        }
        /// <summary>
        /// Handles Edit->Paste context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerPaste(object sender, EventArgs e)
        {
            Paste();
            isShowing = false;
        }
        /// <summary>
        /// Handles Edit->Delete context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerDelete(object sender, EventArgs e)
        {
            DeleteChar();
            isShowing = false;
        }
        /// <summary>
        /// Handles Edit->Undo context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerUndo(object sender, EventArgs e)
        {
            Undo();
            isShowing = false;
        }
        /// <summary>
        /// Handles Edit->Redo context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerRedo(object sender, EventArgs e)
        {
            Redo();
            isShowing = false;
        }
        /// <summary>
        /// Handles Edit->Find context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerFind(object sender, EventArgs e)
        {
            FindDialog();
            isShowing = false;
        }
        /// <summary>
        /// Handles Edit->Replace context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerReplace(object sender, EventArgs e)
        {
            ReplaceDialog();
            isShowing = false;
        }
        /// <summary>
        /// Handles Edit->Goto context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerGoto(object sender, EventArgs e)
        {
            GoToDialog();
            isShowing = false;
        }
        /// <summary>
        /// Handles Edit->SelectAll context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerSelectAll(object sender, EventArgs e)
        {
            SelectAll();
            isShowing = false;
        }
        /// <summary>
        /// Handles Edit->DeleteAll context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerDeleteAll(object sender, EventArgs e)
        {
            DeleteAll();
            isShowing = false;
        }
        /// <summary>
        /// Handles File->New context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerNew(object sender, EventArgs e)
        {
            New();
            isShowing = false;
        }
        /// <summary>
        /// Handles File->Open context menu item.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">EventArgs.</param>
        protected virtual void MenuHandlerOpen(object sender, EventArgs e)
        {
            // Implemented in FileEditControl.
        }
        /// <summary>
        /// Handles File->Close context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerClose(object sender, EventArgs e)
        {
            Close();
            isShowing = false;
        }
        /// <summary>
        /// Handles File->Save context menu item.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">EventArgs.</param>
        protected virtual void MenuHandlerSave(object sender, EventArgs e)
        {
            // Implemented in FileEditControl.
        }
        /// <summary>
        /// Handles File->SaveAs context menu item.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">EventArgs.</param>
        protected virtual void MenuHandlerSaveAs(object sender, EventArgs e)
        {
            // Implemented in FileEditControl.
        }
        /// <summary>
        /// Handles File->PrintPreview context menu item.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">EventArgs.</param>
        private void MenuHandlerPrintPreview(object sender, EventArgs e)
        {
            PrintPreview();
            isShowing = false;
        }
        /// <summary>
        /// Handles File->Print context menu item.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">EventArgs.</param>
        private void MenuHandlerPrint(object sender, EventArgs e)
        {
            Print();
            isShowing = false;
        }
        /// <summary>
        /// Handles Advanced->TabifySelection context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerTabifySelection(object sender, EventArgs e)
        {
            TabifySelection();
            isShowing = false;
        }
        /// <summary>
        /// Handles Advanced->UntabifySelection context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerUntabifySelection(object sender, EventArgs e)
        {
            UntabifySelection();
            isShowing = false;
        }
        /// <summary>
        /// Handles Advanced->IndentSelection context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerIndentSelection(object sender, EventArgs e)
        {
            AddTabsToSelection(true);
            isShowing = false;
        }
        /// <summary>
        /// Handles Advanced->UnindentSelection context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerUnindentSelection(object sender, EventArgs e)
        {
            m_bTransferFocusOnTab = false;
            RemoveTabsFromSelection();
            isShowing = false;
        }
        /// <summary>
        /// Handles Advanced->CommentSelection context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerCommentSelection(object sender, EventArgs e)
        {
            if (!m_selection.IsEmpty())
            {
                ComplexTextRange selected_ranges = m_selection.Clone() as ComplexTextRange;

                foreach (TextRange range in selected_ranges.Ranges)
                {
                    m_parser.EnsureVisibility(range.Top.PhysicalPoint, range.Bottom.PhysicalPoint);

                    range.Start.UpdateVirtualCoordinates();
                    range.End.UpdateVirtualCoordinates();

                    CommentText(range.Top, range.Bottom);
                }
            }
            isShowing = false;
        }
        /// <summary>
        /// Handles Advanced->UncommentSelection context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerUncommentSelection(object sender, EventArgs e)
        {
            if (!m_selection.IsEmpty())
            {
                for (int i = 0; i < m_selection.Ranges.Count; i++)
                {
                    TextRange range = (TextRange)m_selection.Ranges[i];
                    UncommentText(range.Top, range.Bottom);
                }
            }
            isShowing = false;
        }
        /// <summary>
        /// Handles Advanced->CollapseAll context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerCollapseAll(object sender, EventArgs e)
        {
            CollapseAll();
            isShowing = false;
        }
        /// <summary>
        /// Handles Advanced->ExpandAll context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerExpandAll(object sender, EventArgs e)
        {
            ExpandAll();
            isShowing = false;
        }
        /// <summary>
        /// Handles Bookmarks->ToggleBookmark context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerToggleBookmark(object sender, EventArgs e)
        {
            Bookmarks.BookmarkToggle();
            isShowing = false;
        }
        /// <summary>
        /// Handles Bookmarks->NextBookmark context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerNextBookmark(object sender, EventArgs e)
        {
            Bookmarks.BookmarkNext();
            isShowing = false;
        }
        /// <summary>
        /// Handles Bookmarks->PrevBookmark context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerPrevBookmark(object sender, EventArgs e)
        {
            Bookmarks.BookmarkPrevious();
            isShowing = false;
        }
        /// <summary>
        /// Handles Bookmarks->ClearBookmarks context menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuHandlerClearBookmark(object sender, EventArgs e)
        {
            Bookmarks.BookmarkClear();
            isShowing = false;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Invalidates area if current line highlighing is on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCursorManagerLineChanged(object sender, ValueChangedEventArgs e)
        {
            if (this.HighlightCurrentLine)
            {
                int oldLineIndex = (int)e.oldValue;
                int newLineIndex = (int)e.newValue;
                RenderedLine newLine = (RenderedLine)GetLine(newLineIndex);
                RenderedLine oldLine = (oldLineIndex > this.Parser.TotalLines) ? (newLine) : ((RenderedLine)GetLine(oldLineIndex));
                int top = (int)Math.Min(oldLine.Y, newLine.Y);
                int bottom = (int)Math.Max(oldLine.Y + oldLine.Height, newLine.Y + newLine.Height) + 1;

                InvalidateAll(new Rectangle(0, top, this.ClientRectangle.Width, bottom - top));
            }
        }
        /// <summary>
        /// Handler of the CoordinatesChanged event of the cursor manager.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCursorManagerCoordinatesChanged(object sender, EventArgs e)
        {
            UpdateLastCursorPosition();

            if (this.WordWrap)
            {
                Rectangle cursorRect = m_CursorManager.CursorGraphicalCoordinates.Rectangle;
                cursorRect.X += TextDrawOffset;

                if (cursorRect.Right > VirtualSize.Width && VirtualSize.Width > 0)
                {
                    UpdateScrollInfo();
                    return;
                }
            }

            UpdateScrollInfo();

            if (IsSelecting)
            {
                SetSelectionEnd(GetNearestParsePointLeft(CurrentLine, CurrentColumn));
            }
            else if (m_iLastLineInLineSelection == -1)
            {
                SelectionCancel();
            }

            HideIndentGuideline();
            RaiseCursorPositionChangedEvent();
        }
        /// <summary>
        /// Updates measure of lines surrounding new line.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCursorManagerBeforeCoordinatesChange(object sender, CoordinatesChangeEventArgs e)
        {
            if (!m_bClosing)
            {
                RenderedLine topLine = m_parser.GetLineByY(-this.AutoScrollPosition.Y);
                int bottom = -this.AutoScrollPosition.Y + this.ClientRectangle.Height;
                RenderedLine bottomLine;
                if (m_lastLineForBeforeCoordsChanged != null && bottom == m_lastBottomYForBeforeCoordsChanged)
                {
                    bottomLine = m_lastLineForBeforeCoordsChanged;
                }
                else
                {
                    bottomLine = m_parser.GetLineByY(bottom);
                    m_lastBottomYForBeforeCoordsChanged = bottom;
                    m_lastLineForBeforeCoordsChanged = bottomLine;
                }

                if (this.CurrentLine != e.NewPoint.Y &&
                    (topLine != null && e.NewPoint.Y < topLine.LineIndex || bottomLine != null && e.NewPoint.Y > bottomLine.LineIndex))
                {
                    UpdateLinesMeasuring(e.NewPoint.Y);
                    m_bUpdateScrollInfo = true;
                }
            }
        }
		internal string undoOrRedo = string.Empty;
        /// <summary>
        /// Handler of the Delete and Insert events of the lexem parser.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextChanged != null)
            {
                TextChanged(this, e);
            }
            IndentGuideline = null;
            m_lastLineForBeforeCoordsChanged = null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextDeleted(object sender, TextChangedEventArgs e)
        {
            string text = e.Text;
            text = text.Replace("\r", string.Empty);
            string[] strings = text.Split('\n');
            int linesCount = 0;
            int startline = e.StartLine;
            int column = e.StartColumn;
            if (strings != null)
            {
                if (e.Text.Length.Equals(1) && e.Text.Equals("\n"))
                    linesCount = strings.Length - 1;
                else
                    linesCount = strings.Length;
            }
            if (linesCount >= 1)
            {
                TextChange textupdatetype;
                if (e.Text == "\n")
                {
                    column = 1;
                    linesCount = 1;
                    strings[0] = "Empty Line Deleted";
                }
                if (e.Text == "\r\n" && undoOrRedo != string.Empty)
                {
                    column = 1;
                    linesCount = 1;
                    strings[0] = "Empty Line Deleted by " + undoOrRedo;
                }
                if (undoOrRedo != string.Empty)
                {
                    if (undoOrRedo == "UnDo")
                        textupdatetype = TextChange.Undo;
                    else
                        textupdatetype = TextChange.Redo;
                }
                else
                    textupdatetype = TextChange.Deleted;
                LinesEventArgs eveargs = new LinesEventArgs(strings, startline, column, linesCount, textupdatetype);
                TextChangedEventArgs arg = new TextChangedEventArgs(e.Text, e.StartLine, e.StartColumn, e.Type);
                if (e.Text.Contains("\n"))
                {
                    if (linesCount > 1)
                    {
                        if (column == 1)
                        {
                            if (e.Text.EndsWith("\n"))
                            {
                                linesCount -= 1;
                                 eveargs = new LinesEventArgs(strings, startline, column, linesCount, textupdatetype);
                            }
                            OnLineDeleted(sender, eveargs);
                            if (phyLineCount == linesCount)
                                OnLineInserted(this, new LinesEventArgs(strings, startline, column, 1, TextChange.Inserted));
                            OnLineChanged(arg);
                        }
                        else
                        {
                            startline += 1;
                            linesCount -= 1;
                            eveargs = new LinesEventArgs(strings, startline, column, linesCount, textupdatetype);
                            OnLineDeleted(sender, eveargs);
                            OnLineChanged(arg);
                        }
                    }
                    else
                    {
                        eveargs = new LinesEventArgs(strings, startline + 1, column, linesCount, textupdatetype);
                        OnLineDeleted(sender, eveargs);
                        OnLineChanged(arg);
                    }
                }
                else
                    OnLineChanged(arg);

            }
          
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextInserted(object sender, TextChangedEventArgs e)
        {
            OnTextChanged(sender, e);

            string text = e.Text;
            text = text.Replace("\r", string.Empty);
            string[] strings = text.Split('\n');
            int linesCount = 0;
            int startline = e.StartLine;
            int column = e.StartColumn;
            if (strings != null)
            {
                if (e.Text.Length.Equals(1) && e.Text.Equals("\n"))
                    linesCount = strings.Length - 1;
                else
                    linesCount = strings.Length;
            }
            if (linesCount == 1 && e.Text != "\r" && e.Text != "\r\n" && e.Text != "\n")
            {
                OnLineChanged(e);
            }
            if (e.Text.Contains("\n"))
            {
                TextChange textUpdateType;
                if (e.Text == "\n")
                {
                    column = 1;
                    linesCount = 1;
                    strings[0] = "Create a New Line by Press Enter";
                    
                }
                if (e.Text == "\r\n" && undoOrRedo != string.Empty)
                {
                    column = 1;
                    linesCount = 1;
                    strings[0] = "Empty Line Inserted by " + undoOrRedo;
                }
                if (undoOrRedo != string.Empty)
                {
                    if (undoOrRedo == "UnDo")
                        textUpdateType = TextChange.Undo;
                    else
                        textUpdateType = TextChange.Redo;
                }
                else
                    textUpdateType = TextChange.Inserted;

                if (e.Text == "\n" || e.Text == "\r\n" || e.Text == "\r")
                {
                    OnLineInserted(this, new LinesEventArgs(strings, startline + 1, column, linesCount, TextChange.Inserted));
                    OnLineChanged(e);
                }
                else if ((e.Text.Contains("\n") || e.Text.Contains("\r\n") || e.Text.Contains("\r")) && undoOrRedo != string.Empty)
                {
                    if (linesCount > 1)
                    {
                        if (column == 1)
                        {
                            if (e.Text.EndsWith("\n") || e.Text.EndsWith("\r\n") || e.Text.EndsWith("\r"))
                            {
                                linesCount -= 1;
                            }
                        }
                        else
                        {
                            startline += 1;
                            linesCount -= 1;
                        }
                    }
                    OnLineInserted(this, new LinesEventArgs(strings, startline, column, linesCount, TextChange.Inserted));
                    OnLineChanged(e);
                }
                else
                {
                    OnLineChanged(e);
                }
            }
            if (m_endingFormats != null && m_endingFormats.Count > 0)
            {
                CoordinatePoint point = m_parser.GetCoordinatePoint(e.StartLine, e.StartColumn, true);
                for (int i = 0, len = m_endingFormats.Count; i < len; i++)
                {
                    ((DynamicFormat)m_endingFormats[i]).End = point;
                }
            }
        }
        /// <summary>
        /// Handler of the Deleting and Inserting events of the lexem parser.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextChanging(object sender, TextChangingEventArgs e)
        {
            if (TextChanging != null)
            {
                TextChanging(this, e);
            }
        }
        /// <summary>
        /// Handler of the Line modifying events of the lexem parser.
        /// </summary>
        /// <param></param>
        /// <param name="e"></param>
        private void OnLineChanged(TextChangedEventArgs e)
        {
            if (LineChanged != null)
            {
                LineChanged(this, e);
            }
        }
        /// <summary>
        /// Handler of the Line Inserting events of the lexem parser.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLineInserted(object sender, LinesEventArgs e)
        {
            if (LineInserted != null)
            {
                LineInserted(this, e);
            }
        }
        /// <summary>
        /// Handler of the Line Deleting  events of the lexem parser.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLineDeleted(object sender, LinesEventArgs e)
        {
            if (LineDeleted != null)
            {
                LineDeleted(this, e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextInserting(object sender, TextChangingEventArgs e)
        {
            OnTextChanging(sender, e);

            if (!e.Cancel)
            {
                CoordinatePoint point = m_parser.GetCoordinatePoint(e.StartLine, e.StartColumn);
                m_endingFormats = m_formatManager.GetFormatsWithEndAt(point);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        string originalText = string.Empty;
        private void OnTextDeleting(object sender, TextChangingEventArgs e)
        {
            OnTextChanging(sender, e);
        }
        /// <summary>
        /// Called when Insert Mode changed.
        /// </summary>
        protected virtual void OnInsertModeChanged()
        {
            RaiseInsertModeChangedEvent();
        }
        /// <summary>
        /// Called when system colors changed.
        /// </summary>
        /// <param name="e">EventArgs.</param>
        protected override void OnSystemColorsChanged(EventArgs e)
        {
            RecalculateSpaces((m_parser != null) ? (m_parser.TotalLines) : (DEF_NUMBERS_SIZE));
            base.OnSystemColorsChanged(e);
        }
        /// <summary>
        /// Called when display settings changed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">EventArgs.</param>
        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            RecalculateSpaces((m_parser != null) ? (m_parser.TotalLines) : (DEF_NUMBERS_SIZE));
            InvalidateAll();
        }
        /// <summary>
        /// Handler of the UnprocessedKey event of the KeyCommandBinder.
        /// </summary>
        /// <param name="key">Key, that was pressed.</param>
        protected virtual void OnUnprocessedKeyPress(Keys key)
        {
            int keyFilter = (int)Keys.Control /*| ( int )Keys.Alt*/;
            int filteredKey = (int)key & keyFilter;

            //bool extChar = false;
            //if( ( key | Keys.Alt ) == key )
            //{
            //  Keys num = key & ( ~Keys.Alt );
            //  extChar = ( num >= Keys.NumPad0 && num <= Keys.NumPad9 );
            //}

            m_bKeyPressHandled = (filteredKey != 0 /*&& filteredKey != keyFilter*/ /*&& !extChar */);
        }
        /// <summary>
        /// Handler of the LineInstanceDeleted event.
        /// </summary>
        /// <param name="sender">Line, that was deleted.</param>
        /// <param name="e">Empty params.</param>
        private void OnParserLineInstanceDeleted(object sender, EventArgs e)
        {
            m_parser.canUpdate = true;
            RenderedLine line = sender as RenderedLine;

            int y = (int)(line.Y + this.AutoScrollPosition.Y);

            if (y <= this.ClientRectangle.Height)
            {
                if (line.LineIndex != m_parser.TotalLines)
                {
                    InvalidateAll(new Rectangle(0, y - this.AutoScrollPosition.Y, int.MaxValue, (int)Math.Ceiling(line.Height)));
                }
                else
                {
                    InvalidateAll(new Rectangle(0, y - this.AutoScrollPosition.Y, int.MaxValue, int.MaxValue));
                }
            }
        }
        /// <summary>
        /// Handler of the LinesCountChanged of the LexemParser.
        /// </summary>
        /// <param name="sender">Sender of the event. Can be null.</param>
        /// <param name="e"><see cref="ValueChangedEventArgs"/> instance with parameters. Can be null.</param>
        private void OnLinesCountChanged(object sender, ValueChangedEventArgs e)
        {
            if (!this.SingleLineMode)
            {
                m_parser.FixLineRenderingPositions();

                RenderedLine lastLine = m_parser.GetLastRegisteredLine() as RenderedLine;
                int lastLineIndex = (lastLine != null) ? (lastLine.LineIndex) : (0);
                float lastLineBottom = (lastLine != null) ? (lastLine.Y + lastLine.Height) : (0);
                float realBottom = lastLineBottom + (((int)e.newValue) - lastLineIndex) * m_parser.DefaultLineHeight;
                realBottom += AutoScrollPosition.Y;

                if (this.ClientRectangle.Contains(AutoScrollPosition.X + 1, (int)realBottom))
                {
                    RenderedLine line = m_parser.GetLine((int)e.newValue) as RenderedLine;
                    if (!line.IsMeasured)
                    {
                        m_parser.MeasureLine(line);
                    }

                    int Y = (int)Math.Ceiling(line.Y + line.Height);
                    int oldY = this.VirtualSize.Height;

                    if (Y < oldY)
                    {
                        int temp = oldY;
                        oldY = Y;
                        Y = temp;
                    }

                    InvalidateAll();
                }

                int oldWidth = m_lineNumbersWidth;
                RecalculateSpaces(m_parser.BaseStream.LinesCount);
                if (oldWidth != m_lineNumbersWidth)
                {
                    InvalidateAll();
                }

                UpdateScrollerVerticalSize();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="result"></param>
        private void OnContextChoiceClosed(IContextChoiceController sender, DialogResult result)
        {
            DetachLayoutEventsToParents();

            // paste selected word from autocomplete dialog
            if (result == DialogResult.OK)
            {
                IContextChoiceItem selected = sender.SelectedItem;
                ContextChoiceTextInsertEventArgs args = new ContextChoiceTextInsertEventArgs(selected.Text, selected);

                if (ContextChoiceSelectedTextInsert != null)
                {
                    ContextChoiceSelectedTextInsert(m_controllerContextChoice, args);
                }

                if (!args.Cancel)
                {
                    DeleteSelected();  // fix for d4523
                    ContextChoiceController.AutoCompleteStringInfo ac = m_controllerContextChoice.GetAutoCompleteStringInfo();
                    if (ac.Text != args.InsertText)
                    {
                        int column = (!ac.IsEmpty()) ? (ac.Column) : (this.CurrentColumn);

                        StopSelection();
                        SelectionCancel();
                        if (!ac.IsEmpty() && ac.Column != this.CurrentColumn)
                        {
                            TextDeleteInternal(this.CurrentLine, ac.Column, this.CurrentLine, ac.Column + ac.Text.Length, false);
                        }
                        if (selected != null)
                        {
                            TextInsertInternal(this.CurrentLine, column, args.InsertText, true);
                        }

                        StartSelection();
                    }
                }
            }

            m_iContextChoiceLastWordColumn = -1;

            this.Focus();
            m_CursorManager.Update();
            UndoGroupClose();
            InvalidateAll();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnContextPromptClosed(object sender, EventArgs e)
        {
            DetachLayoutEventsToParents();

            if (ContextPromptClose != null)
            {
                ContextPromptCloseEventArgs arg = new ContextPromptCloseEventArgs(m_contextPrompt.List,
                    m_contextPrompt.DialogResult == DialogResult.Cancel, m_contextPrompt.Dropper, m_contextPrompt.LexemBeforeDropper);
                ContextPromptClose(this, arg);
            }

            this.Focus();
            m_CursorManager.Update();
        }
        /// <summary>
        /// Handler for the UndoBufferFlushed event of the changes stream.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBaseStreamUndoBufferFlush(object sender, EventArgs e)
        {
            m_undoGroups.Clear();
            m_UndoMarkers.Clear();
            Invalidate();
        }
        /// <summary>
        /// Handler for the RedoBufferFlushed event of the changes stream.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBaseStreamRedoBufferFlush(object sender, EventArgs e)
        {
            m_redoGroups.Clear();
            m_RedoMarkers.Clear();
        }
        /// <summary>
        /// Updates text of the tooltip.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUpdateToolTip(object sender, UpdateTooltipEventArgs e)
        {
            m_argsLastOuliningTooltip = null;
            if ( !m_codeSnippetsEditBox.Visible)
            {
                m_tip.UseXPStyle = m_bUseXPStyle;
                m_tip.UseXPStyleBorder = m_bUseXPStyleBorder;
                m_tip.BorderColor = m_clrContextTooltipBorder;
                m_tip.BackgroundBrush = m_contextTooltipBackgroundBrush;

                Point point = new Point(e.X, e.Y);
                Point pointVirtual = PointToVirtualPosition(point);

                if (pointVirtual.Y > 0)
                {
                    RenderedLine line = GetLine(pointVirtual.Y) as RenderedLine;

                    if (line != null)
                    {
                        IRenderedLexem lexem = line.FindLexemByColumn(pointVirtual.X) as IRenderedLexem;
                        if (lexem != null && lexem.Collapser != null && lexem.Collapser.Collapsed && m_bShowOutliningTooltip)
                        {
                            CollapsableRegion collapser = lexem.Collapser;
                            string strCollapsed = m_parser.BaseStream.GetTextInRange(collapser.Start, collapser.End, true);
                            bool bShowOutliningTooltip = true;
                            if (null != OutliningTooltipBeforePopup)
                            {
                                OutliningTooltipBeforePopupEventArgs args = new OutliningTooltipBeforePopupEventArgs(
                                    collapser.CollapseName, collapser, strCollapsed);
                                OutliningTooltipBeforePopup(this, args);
                                if (OutliningTooltipShowMode.Off == args.ShowMode) return;
                                if (OutliningTooltipShowMode.SimpleTooltip == args.ShowMode) bShowOutliningTooltip = false;
                            }

                            if (bShowOutliningTooltip)
                            {
                                int iStartLexemLength = lexem.Collapser.Lexem.Length;
                                strCollapsed = strCollapsed.Remove(0, iStartLexemLength);
                                e.Text = TruncateAndUnindentTooltipText(strCollapsed);
                                e.HintedArea.X = (int)(lexem.XOffset + AutoScrollPosition.X + TextDrawOffset);
                                e.HintedArea.Y = (int)(line.Y + lexem.YOffset + AutoScrollPosition.Y + ScrollOffsetTop);
                                e.HintedArea.Width = (int)(lexem.Width);
                                e.HintedArea.Height = (int)(line.SubLineHeight[lexem.SubLine]);
                                m_argsLastOuliningTooltip = new CollapseEventArgs(collapser.CollapseName, collapser, strCollapsed);

                                if (null != OutliningTooltipPopup)
                                {
                                    CollapseEventArgs args = m_argsLastOuliningTooltip;
                                    OutliningTooltipPopup(this, args);
                                }
                            }
                        }
                    }
                }

                if (m_bShowContextTooltip && UpdateContextToolTip != null && this.Focused && isToolTipOn && !m_parser.TooltipHide)
                {
                    string currenttext = string.Empty;
                    if (pointVirtual.Y > 0)
                    {
                        // Get the current line
                        ILexemLine line1 = GetLine(pointVirtual.Y) as ILexemLine;

                        if (line1 != null)
                        {
                            // Get tokens from the current line
                            ILexem lexem1 = line1.FindLexemByColumn(pointVirtual.X);

                            if (lexem1 != null && e.Text != lexem1.Text )
                            {
                                currenttext = lexem1.Text;
                            }
                        }
                    }
                    if (m_codeSnippetsManager != null && m_codeSnippetsManager.m_curTemplateMembers!=null)
                    {
                            foreach (Syncfusion.Windows.Forms.Edit.Utils.CodeSnippets.CodeSnippetsManager.SnippetMember snippetMember in m_codeSnippetsManager.m_curTemplateMembers)
                            {
                                if (snippetMember.Text == currenttext)
                                {
                                    e.Text = snippetMember.ToolTip;
                                }
                            }
                    }
                    m_tip.UseXPStyleBorder = m_bUseXPStyleBorder;
                    UpdateContextToolTip(this, e);
                }
            }
        }
        /// <summary>
        /// Processes stream.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnIdleTextParsing(object sender, EventArgs e)
        {
            IdleStreamProcess();
        }
        /// <summary>
        /// Calls OnConfigurationChanged() when user changes smth. within active configurator.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConfiguratorChanged(object sender, EventArgs e)
        {
            if (m_iSuppressConfigurationChangeNotification <= 0)
            {
                OnConfigurationChanged();
            }
        }
        /// <summary>
        /// Fills default context menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFillMenu(object sender, EventArgs e)
        {
            isShowing = false;
            // Parent items.
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(Localizer.DEF_MENU_EDIT), null);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(Localizer.DEF_MENU_FILE), null);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(Localizer.DEF_MENU_ADVANCED), null);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(Localizer.DEF_MENU_BOOKMARKS), null);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(Localizer.DEF_MENU_OPTIONS), new EventHandler(MenuHandlerOptions));

            // Edit items.
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(Localizer.DEF_MENU_EDIT), Localizer.GetString(Localizer.DEF_MENU_CUT), new EventHandler(MenuHandlerCut));
            m_menuManager.ContextMenuProvider.SetContextMenuItemEnabled(Localizer.GetString(Localizer.DEF_MENU_CUT), CanCut);
            m_menuManager.ContextMenuProvider.SetContextMenuItemShortcut(Localizer.GetString(Localizer.DEF_MENU_CUT), Shortcut.ShiftDel);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(Localizer.DEF_MENU_EDIT), Localizer.GetString(Localizer.DEF_MENU_COPY), new EventHandler(MenuHandlerCopy));
            m_menuManager.ContextMenuProvider.SetContextMenuItemEnabled(Localizer.GetString(Localizer.DEF_MENU_COPY), CanCopy);
            m_menuManager.ContextMenuProvider.SetContextMenuItemShortcut(Localizer.GetString(Localizer.DEF_MENU_COPY), Shortcut.CtrlIns);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(
                Localizer.DEF_MENU_EDIT), Localizer.GetString(Localizer.DEF_MENU_PASTE), new EventHandler(MenuHandlerPaste));
            m_menuManager.ContextMenuProvider.SetContextMenuItemEnabled(Localizer.GetString(Localizer.DEF_MENU_PASTE), CanPaste);
            m_menuManager.ContextMenuProvider.SetContextMenuItemShortcut(Localizer.GetString(Localizer.DEF_MENU_PASTE), Shortcut.ShiftIns);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(
                Localizer.DEF_MENU_EDIT), Localizer.GetString(Localizer.DEF_MENU_DELETE), new EventHandler(MenuHandlerDelete));
            m_menuManager.ContextMenuProvider.SetContextMenuItemShortcut(Localizer.GetString(Localizer.DEF_MENU_DELETE), Shortcut.Del);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(Localizer.DEF_MENU_EDIT), Localizer.GetString(Localizer.DEF_MENU_UNDO), new EventHandler(MenuHandlerUndo));
            m_menuManager.ContextMenuProvider.SetContextMenuItemEnabled(Localizer.GetString(Localizer.DEF_MENU_UNDO), CanUndo);
            m_menuManager.ContextMenuProvider.SetContextMenuItemShortcut(Localizer.GetString(Localizer.DEF_MENU_UNDO), Shortcut.CtrlZ);
            m_menuManager.ContextMenuProvider.SetContextMenuItemSeparator(Localizer.GetString(Localizer.DEF_MENU_UNDO), true);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(Localizer.DEF_MENU_EDIT), Localizer.GetString(Localizer.DEF_MENU_REDO), new EventHandler(MenuHandlerRedo));
            m_menuManager.ContextMenuProvider.SetContextMenuItemEnabled(Localizer.GetString(Localizer.DEF_MENU_REDO), CanRedo);
            m_menuManager.ContextMenuProvider.SetContextMenuItemShortcut(Localizer.GetString(Localizer.DEF_MENU_REDO), Shortcut.CtrlY);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(Localizer.DEF_MENU_EDIT), Localizer.GetString(Localizer.DEF_MENU_FIND), new EventHandler(MenuHandlerFind));
            m_menuManager.ContextMenuProvider.SetContextMenuItemShortcut(Localizer.GetString(Localizer.DEF_MENU_FIND), Shortcut.CtrlF);
            m_menuManager.ContextMenuProvider.SetContextMenuItemSeparator(Localizer.GetString(Localizer.DEF_MENU_FIND), true);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(
                Localizer.DEF_MENU_EDIT), Localizer.GetString(Localizer.DEF_MENU_REPLACE), new EventHandler(MenuHandlerReplace));
            m_menuManager.ContextMenuProvider.SetContextMenuItemShortcut(Localizer.GetString(Localizer.DEF_MENU_REPLACE), Shortcut.CtrlH);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(Localizer.DEF_MENU_EDIT), Localizer.GetString(Localizer.DEF_MENU_GOTO), new EventHandler(MenuHandlerGoto));
            m_menuManager.ContextMenuProvider.SetContextMenuItemShortcut(Localizer.GetString(Localizer.DEF_MENU_GOTO), Shortcut.CtrlG);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(
                Localizer.DEF_MENU_EDIT), Localizer.GetString(Localizer.DEF_MENU_SELECTALL), new EventHandler(MenuHandlerSelectAll));
            m_menuManager.ContextMenuProvider.SetContextMenuItemShortcut(Localizer.GetString(Localizer.DEF_MENU_SELECTALL), Shortcut.CtrlA);
            m_menuManager.ContextMenuProvider.SetContextMenuItemSeparator(Localizer.GetString(Localizer.DEF_MENU_SELECTALL), true);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(
                Localizer.DEF_MENU_EDIT), Localizer.GetString(Localizer.DEF_MENU_DELETEALL), new EventHandler(MenuHandlerDeleteAll));

            // File items.
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(Localizer.DEF_MENU_FILE), Localizer.GetString(Localizer.DEF_MENU_NEW), new EventHandler(MenuHandlerNew));
            m_menuManager.ContextMenuProvider.SetContextMenuItemShortcut(Localizer.GetString(Localizer.DEF_MENU_NEW), Shortcut.CtrlN);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(Localizer.DEF_MENU_FILE), Localizer.GetString(Localizer.DEF_MENU_OPEN), new EventHandler(MenuHandlerOpen));
            m_menuManager.ContextMenuProvider.SetContextMenuItemShortcut(Localizer.GetString(Localizer.DEF_MENU_OPEN), Shortcut.CtrlO);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(Localizer.DEF_MENU_FILE), Localizer.GetString(Localizer.DEF_MENU_CLOSE), new EventHandler(MenuHandlerClose));
            m_menuManager.ContextMenuProvider.SetContextMenuItemShortcut(Localizer.GetString(Localizer.DEF_MENU_NEW), Shortcut.CtrlN);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(Localizer.DEF_MENU_FILE), Localizer.GetString(Localizer.DEF_MENU_SAVE), new EventHandler(MenuHandlerSave));
            m_menuManager.ContextMenuProvider.SetContextMenuItemShortcut(Localizer.GetString(Localizer.DEF_MENU_SAVE), Shortcut.CtrlS);
            m_menuManager.ContextMenuProvider.SetContextMenuItemSeparator(Localizer.GetString(Localizer.DEF_MENU_SAVE), true);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(
                Localizer.DEF_MENU_FILE), Localizer.GetString(Localizer.DEF_MENU_SAVEAS), new EventHandler(MenuHandlerSaveAs));
            m_menuManager.ContextMenuProvider.SetContextMenuItemShortcut(Localizer.GetString(Localizer.DEF_MENU_SAVEAS), Shortcut.CtrlShiftS);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(
                Localizer.DEF_MENU_FILE), Localizer.GetString(Localizer.DEF_MENU_PRINTPREVIEW), new EventHandler(MenuHandlerPrintPreview));
            m_menuManager.ContextMenuProvider.SetContextMenuItemSeparator(Localizer.GetString(Localizer.DEF_MENU_PRINTPREVIEW), true);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(
                Localizer.DEF_MENU_FILE), Localizer.GetString(Localizer.DEF_MENU_PRINT), new EventHandler(MenuHandlerPrint));
            m_menuManager.ContextMenuProvider.SetContextMenuItemShortcut(Localizer.GetString(Localizer.DEF_MENU_PRINT), Shortcut.CtrlP);

            // Advanced items.
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(
                Localizer.DEF_MENU_ADVANCED), Localizer.GetString(Localizer.DEF_MENU_TABIFYSELECTION), new EventHandler(MenuHandlerTabifySelection));
            m_menuManager.ContextMenuProvider.SetContextMenuItemEnabled(Localizer.GetString(Localizer.DEF_MENU_TABIFYSELECTION), !m_selection.IsEmpty());
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(
                Localizer.DEF_MENU_ADVANCED), Localizer.GetString(Localizer.DEF_MENU_UNTABIFYSELECTION), new EventHandler(MenuHandlerUntabifySelection));
            m_menuManager.ContextMenuProvider.SetContextMenuItemEnabled(Localizer.GetString(Localizer.DEF_MENU_UNTABIFYSELECTION), !m_selection.IsEmpty());
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(
                Localizer.DEF_MENU_ADVANCED), Localizer.GetString(Localizer.DEF_MENU_INDENTSELECTION), new EventHandler(MenuHandlerIndentSelection));
            m_menuManager.ContextMenuProvider.SetContextMenuItemEnabled(Localizer.GetString(Localizer.DEF_MENU_INDENTSELECTION), !m_selection.IsEmpty());
            m_menuManager.ContextMenuProvider.SetContextMenuItemSeparator(Localizer.GetString(Localizer.DEF_MENU_INDENTSELECTION), true);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(
                Localizer.DEF_MENU_ADVANCED), Localizer.GetString(Localizer.DEF_MENU_UNINDENTSELECTION), new EventHandler(MenuHandlerUnindentSelection));
            m_menuManager.ContextMenuProvider.SetContextMenuItemEnabled(Localizer.GetString(Localizer.DEF_MENU_UNINDENTSELECTION), !m_selection.IsEmpty());
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(
                Localizer.DEF_MENU_ADVANCED), Localizer.GetString(Localizer.DEF_MENU_COMMENTSELECTION), new EventHandler(MenuHandlerCommentSelection));
            m_menuManager.ContextMenuProvider.SetContextMenuItemEnabled(Localizer.GetString(Localizer.DEF_MENU_COMMENTSELECTION), !m_selection.IsEmpty());
            m_menuManager.ContextMenuProvider.SetContextMenuItemSeparator(Localizer.GetString(Localizer.DEF_MENU_COMMENTSELECTION), true);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(
                Localizer.DEF_MENU_ADVANCED), Localizer.GetString(Localizer.DEF_MENU_UNCOMMENTSELECTION), new EventHandler(MenuHandlerUncommentSelection));
            m_menuManager.ContextMenuProvider.SetContextMenuItemEnabled(Localizer.GetString(Localizer.DEF_MENU_UNCOMMENTSELECTION), !m_selection.IsEmpty());
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(
                Localizer.DEF_MENU_ADVANCED), Localizer.GetString(Localizer.DEF_MENU_COLLAPSEALL), new EventHandler(MenuHandlerCollapseAll));
            m_menuManager.ContextMenuProvider.SetContextMenuItemSeparator(Localizer.GetString(Localizer.DEF_MENU_COLLAPSEALL), true);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(
                Localizer.DEF_MENU_ADVANCED), Localizer.GetString(Localizer.DEF_MENU_EXPANDALL), new EventHandler(MenuHandlerExpandAll));

            // Bookmarks items.
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(
                Localizer.DEF_MENU_BOOKMARKS), Localizer.GetString(Localizer.DEF_MENU_TOGGLEBOOKMARK), new EventHandler(MenuHandlerToggleBookmark));
            m_menuManager.ContextMenuProvider.SetContextMenuItemShortcut(Localizer.GetString(Localizer.DEF_MENU_TOGGLEBOOKMARK), Shortcut.CtrlF2);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(
                Localizer.DEF_MENU_BOOKMARKS), Localizer.GetString(Localizer.DEF_MENU_NEXTBOOKMARK), new EventHandler(MenuHandlerNextBookmark));
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(
                Localizer.DEF_MENU_BOOKMARKS), Localizer.GetString(Localizer.DEF_MENU_PREVBOOKMARK), new EventHandler(MenuHandlerPrevBookmark));
            m_menuManager.ContextMenuProvider.SetContextMenuItemShortcut(Localizer.GetString(Localizer.DEF_MENU_PREVBOOKMARK), Shortcut.ShiftF2);
            m_menuManager.ContextMenuProvider.AddContextMenuItem(Localizer.GetString(
                Localizer.DEF_MENU_BOOKMARKS), Localizer.GetString(Localizer.DEF_MENU_CLEARBOOKMARKS), new EventHandler(MenuHandlerClearBookmark));

            if (MenuFill != null)
            {
                MenuFill(m_menuManager, EventArgs.Empty);
            }
            
            m_menuManager.ContextMenuProvider.Popup += new EventHandler(ContextMenuProvider_Popup);
            m_menuManager.ContextMenuProvider.Collapse += new EventHandler(ContextMenuProvider_Collapse);
        }

        void ContextMenuProvider_Collapse(object sender, EventArgs e)
        {
            isShowing = false;
        }

        /// <summary>
        /// Initializes data, needed for printing of the document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDocBeginPrint(object sender, PrintEventArgs e)
        {
            m_iPrintPageNumber = 1;
            m_iPrintPageY = 0;
            if (m_bPrintCurrentPage)
            {
                m_iPrintPageY = -this.AutoScrollPosition.Y;
            }
            else if (PrintDocument.PrinterSettings.PrintRange == PrintRange.SomePages)
            {
                Margins marg = PrintDocument.DefaultPageSettings.Margins;
                m_iPrintPageNumber = PrintDocument.PrinterSettings.FromPage;
                int lineTop = (PrintDocument.DefaultPageSettings.PaperSize.Height - marg.Bottom - marg.Top) * (m_iPrintPageNumber - 1);
                RenderedLine line = Parser.GetLineByY(lineTop);

                if (line == null) throw new ArgumentOutOfRangeException("FromPage", m_iPrintPageNumber,
                                                        Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_93);

                m_iPrintPageY = (int)line.Y;
            }
        }
        /// <summary>
        /// Remeasures lines using width op the control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDocEndPrint(object sender, PrintEventArgs e)
        {
            CheckControlState();
            m_parser.MaxWidth = MaxWidth;
            m_parser.RemeasureLines();

            if (m_regionAreaToPrint != null)
            {
                m_regionAreaToPrint.Dispose();
                m_regionAreaToPrint = null;
            }
        }
        /// <summary>
        /// Prints single page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPrintDocumentPage(object sender, PrintPageEventArgs e)
        {
            ForPrinting = true;

            m_parser.MaxWidth = (RightToLeft != RightToLeft.Yes && this.WordWrap) ? (e.MarginBounds.Width - this.TextDrawOffset - DEF_OFFSET_TEXT) : (int.MaxValue);

            m_parser.SetDPIFromGraphics(e.Graphics);

            if (PrintDocument.PrinterSettings.PrintRange == PrintRange.Selection)
            {
                m_regionAreaToPrint = GetSelectedTextDrawPath();
            }
            else
            {
                m_regionAreaToPrint = null;
            }

            Rectangle rectMargins;
            Rectangle rectHeader;
            Rectangle rectFooter;

            PrintHeaderFooter(e.MarginBounds, e.PageBounds, e.Graphics, out rectMargins, out rectHeader, out rectFooter);

            RectangleF printableArea = new Rectangle(0, m_iPrintPageY, rectMargins.Width, rectMargins.Height);
            float lastY = float.MaxValue;
            float offsetY = 0;

            e.Graphics.SetClip(rectMargins);
            e.Graphics.TranslateTransform(rectMargins.X, rectMargins.Y - m_iPrintPageY);

            RectangleF regionBounds = RectangleF.Empty;
            if (m_regionAreaToPrint != null)
            {
                regionBounds = m_regionAreaToPrint.GetBounds();
                e.Graphics.TranslateTransform(0, -regionBounds.Top);
                offsetY = regionBounds.Y;
                lastY = regionBounds.Bottom - regionBounds.Y;
                printableArea.Y += regionBounds.Top;
                printableArea.Intersect(regionBounds);
                printableArea.Height++; // Added to make last line in selection be drawn.
            }

            PrintableArea = printableArea;

            DrawAreaBackground(e.Graphics, e.PageBounds, printableArea, true);
            if (m_regionAreaToPrint != null)
            {
                e.Graphics.SetClip(m_regionAreaToPrint, CombineMode.Intersect);
            }

            float out1;
            RenderedLine nextLine;
            if (this.UseNativeDrawing)
            {
                Graphics screenGraphics = CreateGraphics();
                float scale = GetDPIScale(e.Graphics, screenGraphics);

                screenGraphics.Dispose();
                int yOffset = -m_iPrintPageY;
                if (m_regionAreaToPrint != null)
                {
                    yOffset -= (int)regionBounds.Top;
                }
                nextLine = DrawArea(e.Graphics, printableArea, 0, out out1, false, false, true, yOffset, scale,
                    new Size(rectMargins.Left, rectMargins.Top));
            }
            else
            {
                nextLine = DrawArea(e.Graphics, printableArea, 0, out out1, false, false, true, 0, 1, new Size(rectMargins.Left, rectMargins.Top));
            }

            e.Graphics.ResetTransform();
            e.Graphics.ResetClip();
#if DEBUG
            e.Graphics.DrawRectangle(Pens.Black, rectHeader);
            e.Graphics.DrawRectangle(Pens.Black, rectFooter);
#endif

            if (FrameBorderStyle.None != m_marginBorderStyle)
            {
                RectangleF floatMargRect = new RectangleF(rectMargins.Location, rectMargins.Size);
                GraphicsUtils.DrawBorder(e.Graphics, ref floatMargRect, m_marginBorderStyle, m_clrMarginBorder, m_marginBorderWeight);
            }
#if DEBUG
            else
            {
                e.Graphics.DrawRectangle(Pens.Black, rectMargins);
            }
#endif

            m_iPrintPageNumber++;

            if (nextLine != null)
            {
                m_iPrintPageY = (int)(nextLine.Y - offsetY);
            }
            else
            {
                m_iPrintPageY += rectMargins.Height + 1;
            }

            e.HasMorePages = (!m_bPrintCurrentPage && !printableArea.IsEmpty &&
                (nextLine != null && nextLine.LineIndex < m_parser.TotalLines && lastY > (nextLine.Y + nextLine.Height)));
            if (PrintDocument.PrinterSettings.PrintRange == PrintRange.SomePages)
            {
                e.HasMorePages = e.HasMorePages && (PrintDocument.PrinterSettings.ToPage >= m_iPrintPageNumber);
            }

            ForPrinting = false;
        }
        /// <summary>
        /// Raises ContextPromptSelectionChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnContextPromptSelectedPromptChanged(ContextPrompt sender, ContextPromptSelectionChangedEventArgs e)
        {
            if (ContextPromptOn)
            {
                UpdateContextPrompt();
            }
            if (ContextPromptSelectionChanged != null)
            {
                ContextPromptSelectionChanged(sender, e);
            }
        }
        /// <summary>
        /// Invalidates control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDynamicFormatLayerChanged(object sender, EventArgs e)
        {
            InvalidateAll();
        }
        /// <summary>
        /// Shows indent Guideline.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerIndentGuidelineShowTick(object sender, EventArgs e)
        {
            if (!IsDisposed && !IsSelecting)
            {
                m_timerAutoIndent.Stop();
                IndentGuideline = GetCurrentIndentGuideline(false);
            }
        }
        /// <summary>
        /// Drops measuring info of entire control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFormatsConfigurationChanged(object sender, EventArgs e)
        {
            SavedViewInfo info = SaveCurrentViewInfo();
            IEnumerator enumerator = m_parser.GetLineEnumerator();

            while (enumerator.MoveNext())
            {
                RenderedLine line = enumerator.Current as RenderedLine;
                line.DropMeasureInfo();
            }

            RestoreViewInfo(info);
            InvalidateAll();
        }
        /// <summary>
        /// Updates context choice form location.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnControllerContextChoiceFormLoad(object sender, EventArgs e)
        {
            Form form = (Form)sender;
            form.StartPosition = FormStartPosition.Manual;

            //Form Height adjustment. 
            Point formPoint = PointToScreen(this.CursorGraphicalLocation);
            form.Location = formPoint;

            #region Y Adjustment
            //Code to check the Form Y position when it extends beyond the screen working area. 
            Rectangle screenWorkingRect = Screen.PrimaryScreen.WorkingArea;
            if ((form.Location.Y + form.Height) > screenWorkingRect.Bottom)
            {
                //This is for height when the Form is not maximized and the context choice extends the bounds of the screen.
                formPoint.Y = PointToScreen(m_CursorManager.CursorGraphicalCoordinates.LeftTop).Y - form.Height;
            }

            form.Location = formPoint; 
    
            //This is for height when the Form is maximized after the context choice is used once in not maximized mode. 
            if (form.Bottom > screenWorkingRect.Bottom)
            {
                formPoint = PointToScreen(this.CursorGraphicalLocation);
                Rectangle rect = RectangleToScreen(form.Bounds);
                Rectangle cursorRect = RectangleToScreen(m_CursorManager.CursorGraphicalCoordinates.Rectangle);
                formPoint.Y = formPoint.Y - (rect.Height + cursorRect.Height);
                form.Location = formPoint;
            }

            #endregion

            #region X Adjustment
            int y = form.Location.Y;
            formPoint = PointToScreen(form.Location);

            if ((form.Location.X + form.Width) > screenWorkingRect.Right)
            {
                //When the width crosses the primary screen bounds, the form should be shown within the window. 
                formPoint.X = PointToScreen(this.CursorGraphicalLocation).X - form.Width;
                form.Location = new Point(formPoint.X, y);
            }

            #endregion 
        }
        /// <summary>
        /// Replaces whitespaces with tabs.
        /// </summary>
        /// <param name="lexem">Lexem that contains whitespaces.</param>
        /// <returns>Text where whitespaces are replaced with tabs.</returns>
        private string ReplaceWhiteSpaceWithTabs(ILexem lexem)
        {
            FormatManager manager = (FormatManager)m_parser.Formats;
            string result = lexem.Text.Replace(manager.TabReplaceString, DEF_STR_TAB_CHAR);
            return result;
        }
        /// <summary>
        /// Replaces tabs with whitespaces.
        /// </summary>
        /// <param name="lexem">Lexem that contains tabs.</param>
        /// <returns>Text where tabs are replaced with whitespaces.</returns>
        private string ReplaceTabsWithWhiteSpace(ILexem lexem)
        {
            FormatManager manager = (FormatManager)m_parser.Formats;
            string result = lexem.Text.Replace(DEF_STR_TAB_CHAR, manager.TabReplaceString);
            return result;
        }
        /// <summary>
        /// Manages White space show mode properties.
        /// </summary>
        private void OnShowWhiteSpacePropertiesChange(object sender, System.EventArgs e)
        {
            InvalidateAll();
        }
        /// <summary>
        /// Raises OutliningTooltipClose event if needed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTipVisibleChanged(object sender, EventArgs e)
        {
            bool bHidden = (!m_tip.Disposing && !m_tip.IsDisposed && !m_tip.Visible);
            if (bHidden && m_argsLastOuliningTooltip != null && OutliningTooltipClose != null)
            {
                OutliningTooltipClose(this, m_argsLastOuliningTooltip);
            }
        }
        /// <summary>
        /// Inserts auto complete string.
        /// </summary>
        /// <param name="controller"></param>
        private void OnControllerContextChoiceAutoComplete(IContextChoiceController controller)
        {
            string text = m_controllerContextChoice.CommonPart;
            if (text != string.Empty)
            {
                // For not to paste unnecessary text.
                IRenderedLexem lex = this.GetLexemUnderCursor();
                string lexText = lex.Text;

                // Length of first part of lexem (before cursor).
                int iPrevText = this.CurrentColumn - lex.Column;

                // If we (possibly) don't need to paste text, because it already exists.
                if (iPrevText + text.Length <= lexText.Length)
                {
                    // Part of lexem that can be equal to text that has to be inserted.
                    lexText = lexText.Substring(this.CurrentColumn - lex.Column, text.Length);
                }
                else
                {
                    lexText = String.Empty;
                }

                // If we really need to insert the text.
                if (lexText.ToLower() != text.ToLower())
                {
                    StringBuilder sb = new StringBuilder();
                    IList lexems = this.CurrentLineInstance.LineLexems;

                    for (int i = lexems.IndexOf(lex); i < lexems.Count; i++)
                    {
                        sb.Append(((ILexem)lexems[i]).Text);
                        if (sb.Length >= text.Length)
                        {
                            break;
                        }
                    }

                    sb.Remove(0, this.CurrentColumn - lex.Column);
                    string existingText = sb.ToString();
                    int charsToDelete = 0;
                    int shorterString = (existingText.Length < text.Length) ? (existingText.Length) : (text.Length);

                    for (int i = 0; i < shorterString; i++)
                    {
                        if (text[i] == existingText[i])
                        {
                            charsToDelete++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (charsToDelete > 0)
                    {
                        TextDeleteInternal(this.CurrentLine, this.CurrentColumn, this.CurrentLine, this.CurrentColumn + charsToDelete, false);
                    }

                    TextInsertInternal(this.CurrentLine, this.CurrentColumn, text, false);
                }

                // Show selected auto complete string.
                SetSelection(this.CurrentColumn, this.CurrentLine, this.CurrentColumn + text.Length, this.CurrentLine);
                m_controllerContextChoice.AutoCompleteStringShown = true;
            }
        }
        /// <summary>
        /// Hides current indentation guideline.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnParserOutliningStateChanged(object sender, EventArgs e)
        {
            HideIndentGuideline();
        }
        /// <summary>
        /// Performs actions needed to be done when new parser is created.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStreamEditControlParserCreated(object sender, EventArgs e)
        {
            m_autoFormattingManager.Parser = m_parser;
            m_exporter.Parser = m_parser;
        }
        /// <summary>
        /// Manages code snippets context choice.
        /// </summary>
        /// <param name="controller"></param>
        private void OnCodeSnippetsPopupControllerContextChoiceOpen(IContextChoiceController controller)
        {
            m_codeSnippetsPopupController.Activate(Language.SnippetsContainer);
        }
        /// <summary>
        /// Assignes right location to the code snippets list window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCodeSnippetsPopupControllerFormLoad(object sender, EventArgs e)
        {
            Form form = (Form)sender;
            form.Location = PointToScreen(this.CursorGraphicalLocation);
        }
        /// <summary>
        /// Processes code snippets.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="dialogresult"></param>
        private void OnCodeSnippetsPopupControllerContextChoiceClose(IContextChoiceController controller, DialogResult dialogresult)
        {
            if (dialogresult == DialogResult.OK)
            {
                m_codeSnippetsEditBox.HidePopup();
                Form mainForm = this.TopLevelControl as Form;
                if (mainForm != null)
                {
                    mainForm.Activate();
                }

                string title = m_codeSnippetsPopupController.SelectedItem.Text;
                m_codeSnippetsManager.Activate(m_codeSnippetsPopupController.CurrentContainer.GetSnippetByTitle(title), this.SelectedText);
            }
            if (m_iUndoGroupOpened > 0)
            {
                UndoGroupClose();
            }
        }
        /// <summary>
        /// Updates state of start and end points of each dynamic formatting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBaseStreamAfterTextChange(object sender, EventArgs e)
        {
            m_formatManager.UpdateFormats();
        }
        /// <summary>
        /// Manages bookmark tooltips.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBookmarksTooltipUpdateTooltip(object sender, UpdateTooltipEventArgs e)
        {
            int offset = (RightToLeft == RightToLeft.Yes) ? this.MarkerAreaOffset : this.MarkerAreaWidth;
            if (m_bShowBookmarkTooltips && e.X <= offset && e.Y > 0)
            {
                RenderedLine line = m_parser.GetLineByY(e.Y);
                if (line != null)
                {
                    int iLine = line.LineIndex;
                    IBookmark bookmark = this.Bookmarks.BookmarkGet(iLine);

                    if (bookmark == null)
                    {
                        bookmark = this.Bookmarks.GetCustomBookmark(iLine);
                    }

                    if (bookmark != null)
                    {
                        m_bookmarksTooltip.UseXPStyle = m_bUseXPStyle;
                        m_bookmarksTooltip.BorderColor = m_clrBookmarkTooltipBorder;
                        m_bookmarksTooltip.BackgroundBrush = m_bookmarkTooltipBackgroundBrush;
                        e.HintedArea = new Rectangle(0, (int)line.Y, m_markerAreaWidth, (int)line.Height);

                        if (UpdateBookmarkToolTip != null)
                        {
                            UpdateBookmarkTooltipEventArgs args = new UpdateBookmarkTooltipEventArgs(iLine, bookmark, e);
                            UpdateBookmarkToolTip(this, args);

                            e.HintedArea = args.HintedArea;
                            e.Image = args.Image;
                            e.Text = args.Text;
                            e.X = args.X;
                            e.Y = args.Y;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Updates line on screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnParserLineIndexChanged(object sender, EventArgs e)
        {
            RenderedLine line = (RenderedLine)sender;
            Rectangle rect;

            if (line.IsMeasured)
            {
                rect = new Rectangle(-this.AutoScrollPosition.X, (int)line.Y, ClientSize.Width, (int)line.Height);
            }
            else
            {
                rect = new Rectangle(-this.AutoScrollPosition.X, (int)line.Y, ClientSize.Width, (int)m_parser.DefaultLineHeight);
            }

            InvalidateAll(rect);
        }
        #endregion
    }
    /// <summary>
    /// Specifies how vertical scrolling is processed.
    /// </summary>
    public enum ScrollMode
    {
        /// <summary>
        /// Scrolls the control to the new value immediately.
        /// </summary>
        Immediate,

        /// <summary>
        /// Scrolls the control when thumb track is completed. Not Implemented.
        /// </summary>
        Deferred,

        /// <summary>
        /// Scrolls line by line.
        /// </summary>
        Pixel
    }
}
