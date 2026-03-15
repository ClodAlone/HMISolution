#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.ComponentModel;
using System.Windows;
using System;
using System.Xml.Serialization;

#if !WinRT
using System.Windows.Controls;
using Syncfusion.Windows.Collections;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Styles;
using System.Windows.Media;
namespace Syncfusion.Windows.Controls.Grid
#else
using Windows.UI.Xaml.Media;
using Windows.UI;
using Syncfusion.WinRT.Controls.Cells;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{
    public enum GridDrawSelectionOptions
    {
        None = 0,
        AlphaBlend = 1,
        ReplaceTextColor = 2,
        ReplaceBackground = 4,
        OverlapWithDarkenedAlphaBlend = 8,
        ExcelLikeSelectionMarker = 16
    }

    /// <summary>
    /// Defines Copy-Paste options.
    /// </summary>
    public enum CopyPaste
    {
        /// <summary>
        /// Set the CopyText option
        /// </summary>
        CopyText = 1,

        /// <summary>
        /// Set the CopyCell option
        /// </summary>
        CopyCellData = 2,

        /// <summary>
        /// Set the PasteText option
        /// </summary>
        PasteText = 4,

        /// <summary>
        /// Set the PasteCell option
        /// </summary>
        PasteCell = 8,

        /// <summary>
        /// Set the CutText option
        /// </summary>
        CutText = 16,

        /// <summary>
        /// Set the CutCell option
        /// </summary>
        CutCell = 32,

        /// <summary>
        /// Omits the current cell during clipboard operations.
        /// </summary>
        ExcludeCurrentCell = 64,

        /// <summary>
        /// Includes all style info while copy, cut and paste using IDataObject.
        /// </summary>
        IncludeStyle =128,

        
        /// <summary>
        /// Includes all Header Text also while copy and cut operation.
        /// </summary>
        IncludeHeaders= 256,

        /// <summary>
        /// Includes the Empty cell during clipboard operations.
        /// </summary>
        IncludeEmptyCells = 512,

        /// <summary>
        /// Includes the Summaries during clipboard copy operation.
        /// </summary>
        CopySummaries = 1024,

        /// <summary>
        /// Includes the Captions during clipboard copy operation.
        /// </summary>
        CopyCaptions = 2048
    }

    public enum EnterKeyBehaviour
    {
        MouseDown,
        MouseRight
    }
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridModelOptions : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        GridWrapCellBehavior wrapCellBehavior = GridWrapCellBehavior.None;
        bool mulitExtendedArrowKeySelect = true;
        GridSelectionMode listboxMode = GridSelectionMode.None;
        bool allowUIElementClick = false;
        GridSelectionFlags allowSelection = GridSelectionFlags.Any;
        bool excelLikeCurrentCell = true;
        bool excelLikeSelection = false;
        bool excelLikeSelectionFrame = false;
        bool excelLikeFreezePane = false;
        bool excelLikeTabNavigation = false;
        GridCellActivateAction activateCurrentCellBehavior = GridCellActivateAction.ClickOnCell;
        bool enableFloatCell = false;
        EnterKeyBehaviour enterKeyBehavior = EnterKeyBehaviour.MouseRight;
        GridFloatCellsMode floatCellMode = GridFloatCellsMode.None;
        bool floodCell = true;
        bool scrollFrozenLikeExcel = false;
        bool showCurrentCell = true;
        GridDrawSelectionOptions drawSelectionOptions = GridDrawSelectionOptions.AlphaBlend;


        int maxLength = 1000;
        bool allowSelectionOnShiftTab = false;
        GridControlLengthUnitType columnSizer = GridControlLengthUnitType.None;

        GridShowFormulaBehavior formulaDisplayBehavior = GridShowFormulaBehavior.WhenEditing;
        //DragCellsOptions
        GridDataObjectConsumerOptions dataObjectConsumerOptions = GridDataObjectConsumerOptions.All;
        internal GridDragDropFlags dragDropDropTargetFlags;

        public GridDragDropFlags DragDropDropTargetFlags
        {
            get
            {
                return dragDropDropTargetFlags;
            }

            set
            {
                dragDropDropTargetFlags = value;
                this.NotifyPropertyChanged("DragDropDropTargetFlags");
            }
        }

        public GridDataObjectConsumerOptions DataObjectConsumerOptions
        {
            get
            {
                return dataObjectConsumerOptions;
            }

            set
            {
                if (dataObjectConsumerOptions != value)
                {
                    dataObjectConsumerOptions = value;
                    this.NotifyPropertyChanged("DataObjectConsumerOptions");
                }
            }
        }
        [XmlIgnore]
        public Brush HighlightSelectionAlphaBlend { get; set; }

        [XmlIgnore]
        public Brush HighlightSelectionBackground { get; set; }

        [XmlIgnore]
        public Brush HighlightSelectionForeground { get; set; }

        [XmlIgnore]
        public Brush HighlightSelectionBorder { get; set; }

        public double HighlightSelectionBorderWidth { get; set; }

        [XmlIgnore]
        public Brush CurrentCellBorder { get; set; }

        public double CurrentCellBorderWidth { get; set; }


        private bool allowExcelLikeResizing = true;

        /// <summary>
        /// Allows user to resize Hidden columns and hidden rows as in Excel.
        /// </summary>
        public bool AllowExcelLikeResizing
        {
            get { return allowExcelLikeResizing; }
            set
            {
                if (allowExcelLikeResizing != value)
                {
                    allowExcelLikeResizing = value;
                    this.NotifyPropertyChanged("AllowExcelLikeResizing");
                }
            }
        }
        private bool showErrorIConAtEditing = true;

        /// <summary>
        /// Gets / sets ErrorIcon at Editing time.
        /// </summary>
        public bool ShowErrorIconOnEditing
        {
            get
            {
                return showErrorIConAtEditing;
            }
            set
            {
                if (showErrorIConAtEditing != value)
                {
                    showErrorIConAtEditing = value;
                    this.NotifyPropertyChanged("ShowErrorIconAtEditing");
                }
            }
        }

        public GridModelOptions()
        {
#if !WinRT
            Color c = SystemColors.HighlightColor;
            HighlightSelectionAlphaBlend = new SolidColorBrush(Color.FromArgb(96, c.R, c.G, c.B));
            HighlightSelectionBackground = new SolidColorBrush(SystemColors.ControlLightColor);
            HighlightSelectionForeground = new SolidColorBrush(SystemColors.ControlTextColor);
#else
            Color c = Colors.Blue;
            HighlightSelectionAlphaBlend = new SolidColorBrush(Color.FromArgb(96, c.R, c.G, c.B));
            HighlightSelectionBackground = new SolidColorBrush(Colors.Blue);
            HighlightSelectionForeground = new SolidColorBrush(Colors.Wheat);
#endif
            HighlightSelectionBorder = new SolidColorBrush(Colors.Black);
            HighlightSelectionBorderWidth = 2;
            CurrentCellBorder = new SolidColorBrush(Colors.Black);
            CurrentCellBorderWidth = 1;
            HiddenBorderBrush = new SolidColorBrush(Colors.Black);
            HiddenBorderThickness = 2;
        }

        /// <summary>
        /// Specifies the thickness of the line drawn at the border to indicate any hidden columns/rows
        /// </summary>
        public double HiddenBorderThickness { get; set; }

        /// <summary>
        /// Specifies the brush of the line drawn at the border to indicate any hidden columns/rows
        /// </summary>
        [XmlIgnore]
        public Brush HiddenBorderBrush { get; set; }

        //[DefaultValue(GridDrawSelectionOptions.AlphaBlend)]
        public GridDrawSelectionOptions DrawSelectionOptions
        {
            get { return drawSelectionOptions; }
            set { drawSelectionOptions = value; }
        }

        /// <summary>
        /// Defines Excel-like current cell behavior. When the user moves the current cell out of a selected
        /// range, the range will be cleared. If the user moves the current cell inside a selected range, the
        /// range will stay.
        /// </summary>
        public bool ExcelLikeCurrentCell
        {
            get
            {
                return excelLikeCurrentCell;
            }
            set
            {
                excelLikeCurrentCell = value;
                //OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// User Choice for how the clipboard operation is performed 
        /// </summary>
        [XmlIgnore]
        public CopyPaste CopyPasteOption
        {
            get;
            set;
        }


        public bool AllowSelectionOnShiftTab
        {
            get
            {
                return this.allowSelectionOnShiftTab;
            }
            set
            {
                this.allowSelectionOnShiftTab = value;
            }
        }

        /// <summary>
        /// Property Change Notifier
        /// </summary>
        /// <param name="info"></param>
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        /// <summary>
        /// User Choice for how the column can be sized
        /// </summary>
        public GridControlLengthUnitType ColumnSizer
        {
            get
            {
                return columnSizer;
            }
            set
            {
                columnSizer = value;
                NotifyPropertyChanged("ColumnSizer");
            }
        }

        /// <summary>
        /// User Choice for specifying how many rows to take into account for sizing
        /// </summary>
        public int MaxLength
        {
            get
            {
                return maxLength;
            }
            set
            {
                maxLength = value;
                NotifyPropertyChanged("MaxLength");
            }
        }

        /// <summary>
        /// <summary>
        /// Defines Excel-like selection behavior. Prevents the user from doing multi selection when the current cell in Edit Mode.
        /// </summary>
#if !WinRT
        [
        Browsable(true)//,
            //DefaultValue(false)
        ]
        [Description("Defines Excel-like current cell behavior. When the user moves the current cell out of a selected range, the range will be cleared.")]
        [Category("Excel-Emulation")]
#endif
        public bool ExcelLikeSelection
        {
            get
            {
                return excelLikeSelection;
            }
            set
            {
                excelLikeSelection = value;
                //OnOptionsChanged(EventArgs.Empty);
            }
        }
        /// <summary>
        /// Specifies whether the active selection should be outline with a selection frame.
        /// </summary>
        public bool ExcelLikeSelectionFrame
        {
            get
            {
                return excelLikeSelectionFrame;
            }
            set
            {
                excelLikeSelectionFrame = value;
                //OnOptionsChanged(EventArgs.Empty);
            }
        }

        public bool ExcelLikeFreezePane
        {
            get
            {
                return excelLikeFreezePane;
            }
            set
            {
                excelLikeFreezePane = value;
            }
        }

        /// <summary>
        /// It will enable or disable the Excel like tab key navigation
        /// </summary>
        /// <value><see langword="true"/> if set to true then skips the read only cells move to the next enabled cell;
        /// otherwise, move to the next cell <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool ExcelLikeTabNavigation
        {
            get
            {
                return excelLikeTabNavigation;
            }
            set
            {
                excelLikeTabNavigation = value;
            }
        }

        /// <summary>
        /// Defines selection behavior of the grid.
        /// </summary>
        /// <value>
        /// A <see cref="GridSelectionFlags"/> that specifies options to be applied.
        /// </value>
        /// <remarks>
        /// You can customize the mouse controller's behavior at run-time while the user is performing the action 
        /// by subscribing to the <see cref="GridModel.SelectionChanging"/> and 
        /// <see cref="GridModel.SelectionChanged"/> events.<para/>
        /// The <see cref="SelectCellsMouseButtonsMask"/> property lets you decide which mouse buttons 
        /// can be used for selecting cells.
        /// </remarks>
        public GridSelectionFlags AllowSelection
        {
            get
            {
                return allowSelection;
            }
            set
            {
                allowSelection = value;
            }
        }

                 /// <summary>
        /// set procedence style for either row or column
        /// </summary>
        private PrecedenceStyle _RowColStylePrecedence = PrecedenceStyle.Row;
        public PrecedenceStyle RowColStylePrecedence
        {
            get
            {
                return _RowColStylePrecedence;
            }
            set
            {
                _RowColStylePrecedence = value;
            }
        }

        /// <summary>
        /// Go to first column in next row or last column in previous row when
        /// at end or beginning of a row and moving left or right. For more options, 
        /// use <see cref="WrapCellBehavior"/> instead.
        /// </summary>
        public bool WrapCell
        {
            get
            {
                return wrapCellBehavior != GridWrapCellBehavior.None;
            }
            set
            {
                if (value)
                    wrapCellBehavior = GridWrapCellBehavior.WrapRow;
                else
                    wrapCellBehavior = GridWrapCellBehavior.None;
            }
        }

        /// <summary>
        /// Go to first column in next row or last column in previous row when
        /// at end or beginning of a row and moving left or right.
        /// </summary>
        public GridWrapCellBehavior WrapCellBehavior
        {
            get
            {
                return wrapCellBehavior;
            }
            set
            {
                wrapCellBehavior = value;
            }
        }

        /// <summary>
        /// Enables list box-like selection behavior for the grid when the user moves the current cell.
        /// </summary>
        /// <value>
        /// A <see cref="GridSelectionMode"/> that defines the list box-like selection behavior of the grid.
        /// </value>
        public GridSelectionMode ListBoxSelectionMode
        {
            get
            {
                return listboxMode;
            }
            set
            {
                if (listboxMode != value)
                {
                    listboxMode = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether child UIElements in cells should receive mouse events when
        /// list box-like selection behavior is enabled for the grid. Set this true
        /// if you want to place buttons or checkboxes in cells.
        /// </summary>
        public bool ListBoxModeAllowUIElementClick
        {
            get
            {
                return allowUIElementClick;
            }
            set
            {
                if (allowUIElementClick != value)
                {
                    allowUIElementClick = value;
                }
            }
        }

        /// <summary>
        /// When you select GridSelectionMode.MultiExtended, this flag defines if the rows selection
        /// should be cleared and moved with the new current cell or if only the current cell
        /// should be moved without clearing selections.
        /// </summary>
        internal bool MulitExtendedArrowKeySelect
        {
            get
            {
                return mulitExtendedArrowKeySelect;
            }
            set
            {
                // TODO: Add code in GridSelectCellsMouseCountroler to extend selection without moving current cell.
                mulitExtendedArrowKeySelect = value;
            }
        }

        /// <summary>
        /// Specifies current cell activation behavior when moving the current cell or clicking inside a cell.
        /// </summary>
        /// <value>
        /// A <see cref="GridCellActivateAction"/> enumeration that defines when to set the focus / toggle edit mode for the current cell.
        /// </value>
        public GridCellActivateAction ActivateCurrentCellBehavior
        {
            get
            {
                return activateCurrentCellBehavior;
            }
            set
            {
                activateCurrentCellBehavior = value;
                //OnOptionsChanged(EventArgs.Empty);
            }
        }

        public bool EnableFloatCell
        {
            get
            {
                return enableFloatCell;
            }
            set
            {
                enableFloatCell = value;
                //OnOptionsChanged(EventArgs.Empty);
            }
        }

        public EnterKeyBehaviour EnterKeyBehaviour
        {
            get
            {
                return enterKeyBehavior;
            }
            set
            {
                enterKeyBehavior = value;
                //OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Get or set the value indicating whether to allow floating for the cell and its mode
        /// </summary>
        /// <value>
        /// A <see cref="GridFloatCellMode"/> enumeration that defines the mode of floating.
        /// </value>
        public GridFloatCellsMode FloatCellMode
        {
            get
            {
                return floatCellMode;
            }
            set
            {
                floatCellMode = value;
                //OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Get or set the value indicationg whether the neghbouring cell float over the corresponding cell.
        /// </summary>
        /// <value>
        /// The boolean value indicating to allow the behavior
        /// </value>
        public bool FloodCell
        {
            get
            {
                return floodCell;
            }
            set
            {
                floodCell = value;
                //OnOptionsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Defines scroll behavior when user moves current cell with arrow keys into the frozen cells area.
        /// </summary>
        /// <remarks>
        /// True, if current cell is at the topmost nonfrozen row, scroll the view.
        /// False, move current cell into frozen cells. If current cell is at the top row, 
        /// scroll the view.
        /// </remarks>
        public bool ScrollFrozen
        {
            get
            {
                return scrollFrozenLikeExcel;
            }
            set
            {
                scrollFrozenLikeExcel = value;
                //                OnOptionsChanged(EventArgs.Empty);
            }
        }

        public bool ShowCurrentCell
        {
            get { return showCurrentCell; }
            set { showCurrentCell = value; }
        }
        /// <summary>
        /// Defines when formulas are visible in a formula cell.
        /// </summary>
        /// <remarks>
        /// You can use this property to always hide formulas from your user, to always show
        /// formulas to your user, or only show them in a particular cell when it is either current
        /// or being edited.
        /// </remarks>
        public GridShowFormulaBehavior FormulaDisplayBehavior
        {
            get
            {
                return formulaDisplayBehavior;
            }
            set
            {
                formulaDisplayBehavior = value;
                //                OnOptionsChanged(EventArgs.Empty);
            }
        }
    }
}
