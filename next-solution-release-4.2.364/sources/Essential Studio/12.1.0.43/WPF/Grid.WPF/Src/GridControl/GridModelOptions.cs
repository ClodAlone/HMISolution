#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System.ComponentModel;
    using System.Windows.Controls;
    using Syncfusion.Windows.Collections;
    using Syncfusion.Windows.ComponentModel;
    using Syncfusion.Windows.Diagnostics;
    using Syncfusion.Windows.Styles;
    using System.Windows.Media;
    using System.Windows;
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines possible selection options.
    /// </summary>
    public enum GridDrawSelectionOptions
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,
        /// <summary>
        /// Alphablend selection.
        /// </summary>
        AlphaBlend = 1,
        /// <summary>
        /// Replaces text color of the cells being selected.
        /// </summary>
        ReplaceTextColor = 2,
        /// <summary>
        /// Replaces background of the cells being selected.
        /// </summary>
        ReplaceBackground = 4,
        /// <summary>
        /// Draws darkened alphablend color when selections overlap.
        /// </summary>
        OverlapWithDarkenedAlphaBlend = 8,
        /// <summary>
        /// Displays excel like selection marker.
        /// </summary>
        ExcelLikeSelectionMarker = 16
    }

    public enum EnterKeyBehaviour
    {
        MouseDown,
        MouseRight
    }

    /// <summary>
    /// Defines Copy-Paste options.
    /// </summary>
    [Flags]
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
        IncludeStyle = 128,

        /// <summary>
        /// Includes all Header Text also while copy and cut operation.
        /// </summary>
        IncludeHeaders= 256,

        /// <summary>
        /// Cut/copy/paste the data in Xml format. 
        /// </summary>
        XmlCopyPaste = 512,

        /// <summary>
        /// Includes the Empty cell during clipboard operations.
        /// </summary>
        IncludeEmptyCells = 1024,

        /// <summary>
        /// Includes the Summaries during clipboard copy operation.
        /// </summary>
        CopySummaries = 2048,

        /// <summary>
        /// Includes the Captions during clipboard copy operation.
        /// </summary>
        CopyCaptions = 4096
    }

    /// <summary>
    /// Provides properties that allow you to adjust behavior and appearance of the grid.
    /// </summary>
    [Serializable]
    public class GridModelOptions : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        GridWrapCellBehavior wrapCellBehavior = GridWrapCellBehavior.None;
        bool mulitExtendedArrowKeySelect = true;
        GridSelectionMode listboxMode = GridSelectionMode.None;
        bool allowUIElementClick = false;
        GridSelectionFlags allowSelection = GridSelectionFlags.Any;
        CopyPaste CopypasteOption = CopyPaste.CopyText | CopyPaste.PasteText | CopyPaste.CutText;
        bool excelLikeCurrentCell = true;
        bool excelLikeSelection = false;
        GridControlLengthUnitType columnSizer = GridControlLengthUnitType.None;
        bool excelLikeSelectionFrame = false;
        bool excelLikeFreezePane = false;
        bool enableFloatingCell = false;
        bool excelLikeTabNavigation = false;
        EnterKeyBehaviour enterKeyBehavior = EnterKeyBehaviour.MouseRight;
        GridFloatCellsMode floatCellMode = GridFloatCellsMode.None;
        bool floodCell = true;
        GridCellActivateAction activateCurrentCellBehavior = GridCellActivateAction.ClickOnCell;
        bool scrollFrozenLikeExcel = false;
        bool showCurrentCell = true;
        bool allowSelectionOnShiftTab = true;
        bool allowSelectionOnMouseUp = false;        
        GridShowFormulaBehavior formulaDisplayBehavior = GridShowFormulaBehavior.WhenEditing;
        GridDrawSelectionOptions drawSelectionOptions = GridDrawSelectionOptions.AlphaBlend;
        int maxLength = 1000;


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

        public bool EnableFloatingCell
        {
            get
            {
                return enableFloatingCell;
            }
            set
            {
                enableFloatingCell = value;
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

        private bool allowTextSelectionOnReadOnly = false;

        public bool AllowTextSelectionOnReadOnly
        {
            get
            {
                return allowTextSelectionOnReadOnly;
            }
            set
            {
                allowTextSelectionOnReadOnly = value;
                this.NotifyPropertyChanged("AllowTextSelectionOnReadOnly");
            }
        }

        private bool allowExcelLikeResizing = false;

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

        private bool showErrorIConAtEditing = false;

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


        /// <summary>
        /// Specifies the thickness of the line to be drawn at the border to indicate any hidden columns/rows
        /// </summary>
        public double HiddenBorderThickness { get; set; }

        /// <summary>
        /// Specifies the brush of the line to be drawn at the border to indicate any hidden columns/rows
        /// </summary>
        [XmlIgnore]
        public Brush HiddenBorderBrush { get; set; }


        /// <summary>
        /// Specifies the alphablend brush used to highlight the selection.
        /// </summary>
        [XmlIgnore]
        public Brush HighlightSelectionAlphaBlend { get; set; }

        /// <summary>
        /// Specifies a background brush for highlighting the selection.
        /// </summary>
        [XmlIgnore]
        public Brush HighlightSelectionBackground { get; set; }

        /// <summary>
        /// Specifies wheather SelectionBackground changed or not.
        /// </summary>
        public bool highlightSelectionBackgroundChanged { get; set; }

        /// <summary>
        /// Specifies a foreground brush for highlighting the selection.
        /// </summary>
        [XmlIgnore]
        public Brush HighlightSelectionForeground { get; set; }

        /// <summary>
        /// Specifies wheather HighlightSelectionForeground was changed or not.
        /// </summary>
        public bool highlightSelectionForegroundChanged { get; set; }

        /// <summary>
        /// Specifies a border brush for highlighting the selection.
        /// </summary>
        [XmlIgnore]
        public Brush HighlightSelectionBorder { get; set; }

        /// <summary>
        /// Specifies a width for highlight selection border.
        /// </summary>

        public double HighlightSelectionBorderWidth { get; set; }

        /// <summary>
        /// Specifies a brush for current cell border.
        /// </summary>
        [XmlIgnore]
        public Brush CurrentCellBorder { get; set; }

        /// <summary>
        /// Specifies a width for current cell border.
        /// </summary>

        public double CurrentCellBorderWidth { get; set; }

        /// <summary>
        /// Intializes a new <see cref="GridModelOptions"/>.
        /// </summary>
        public GridModelOptions()
        {
            Color c = SystemColors.HighlightColor;
            HighlightSelectionAlphaBlend = new SolidColorBrush(Color.FromArgb(96, c.R, c.G, c.B));
            HighlightSelectionBorder = Brushes.Black;
            HighlightSelectionBackground = SystemColors.HighlightBrush;
            HighlightSelectionForeground = SystemColors.HighlightTextBrush;
            HighlightSelectionBorderWidth = GridControlConstants.HighlightSelectionBorderWidth;
            CurrentCellBorder = GridControlConstants.CurrentCellBorder;
            CurrentCellBorderWidth = GridControlConstants.CurrentCellBorderWidth;
            HiddenBorderThickness = GridControlConstants.HiddenBorderThickness;
            HiddenBorderBrush = Brushes.Black;
            AllowTextSelectionOnReadOnly = false;
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
        /// Controls the appearance of the selection drawing.
        /// </summary>
        //[DefaultValue(GridDrawSelectionOptions.AlphaBlend)]
        public GridDrawSelectionOptions DrawSelectionOptions
        {
            get { return drawSelectionOptions; }
            set { drawSelectionOptions = value; 
			if (value == GridDrawSelectionOptions.ReplaceBackground || value == GridDrawSelectionOptions.ReplaceTextColor)
                this.showCurrentCell = false;}
        }

        /// <summary>
        /// User Choice for how the clipboard operation is performed 
        /// </summary>
        [XmlIgnore]
        public CopyPaste CopyPasteOption
        {
            get
            {
                return this.CopypasteOption;
            }
            set
            {
                this.CopypasteOption = value;
            }
        }

        internal bool AllowSelectionOnShiftTab
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
        /// Gets / Sets the max length for computing auto sizing range. Default value is 1000.
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
        /// Defines Excel-like current cell behavior. When the user moves the current cell out of a selected
        /// range, the range will be cleared. If the user moves the current cell inside a selected range, the
        /// range will stay.
        /// </summary>
        [
        Browsable(true)//,
            //DefaultValue(false)
        ]
        [Description("Defines Excel-like current cell behavior. When the user moves the current cell out of a selected range, the range will be cleared.")]
        [Category("Excel-Emulation")]
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
        /// Defines Excel-like selection behavior. Prevents the user from doing multi selection when the current cell in Edit Mode.
        /// </summary>
        [
        Browsable(true)//,
            //DefaultValue(false)
        ]
        [Description("Defines Excel-like current cell behavior. When the user moves the current cell out of a selected range, the range will be cleared.")]
        [Category("Excel-Emulation")]
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
        [
        Browsable(true)//,
            //DefaultValue(false)
        ]
        [Description("Specifies whether the active selection should be outlined with a selection frame.")]
        [Category("Excel-Emulation")]
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
            get { return excelLikeTabNavigation; }
            set { excelLikeTabNavigation = value; }
        }

        public bool AllowSelectionOnMouseUp  
        {          
 		    get        
  		    {              
 			    return allowSelectionOnMouseUp;    
       	    }
            set 
            {    
                 allowSelectionOnMouseUp = value;    
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
        [
        Browsable(true)//,
            //DefaultValue(GridSelectionFlags.Any)
        ]
        [Description("Defines selection behavior of the grid.")]
        [Category("Behavior-Selection")]
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
        /// Go to first column in next row or last column in previous row when
        /// at end or beginning of a row and moving left or right. For more options, 
        /// use <see cref="WrapCellBehavior"/> instead.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)//,
            //DefaultValue(false)
        ]
        [Description("Controls what the grid does when at last column in a row.")]
        [Category("Behavior-Keyboard")]
        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
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
        [
        Browsable(true)//,
            //DefaultValue(GridWrapCellBehavior.None)
        ]
        [Description("Controls what the grid does when at last column in a row.")]
        [Category("Behavior-Keyboard")]
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
        [
        Category(@"Behavior"),
            //DefaultValue(GridSelectionMode.None),
        Description(@"Grid can emulated list boxes. In such mode, indicates if the list box is to be single-select, multi-select, or unselectable.")
        ]
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
        [
        Category(@"Behavior"),
            //DefaultValue(GridSelectionMode.None),
        Description(@"Grid can emulated list boxes. In such mode, indicates if the list box is to be single-select, multi-select, or unselectable.")
        ]
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
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        ]
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
        [
        Browsable(true)//,
            //DefaultValue(GridCellActivateAction.ClickOnCell)
        ]
        [Description("Specifies current cell activation behavior when moving the current cell or clicking inside a cell.")]
        [Category("Behavior")]
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

        /// <summary>
        /// Defines scroll behavior when user moves current cell with arrow keys into the frozen cells area.
        /// </summary>
        /// <remarks>
        /// True, if current cell is at the topmost nonfrozen row, scroll the view.
        /// False, move current cell into frozen cells. If current cell is at the top row, 
        /// scroll the view.
        /// </remarks>
        [
        Browsable(true)//,
            //DefaultValue(true),
        ]
        [Description("Defines scroll behavior when user moves current cell with arrow keys into frozen cells area.")]
        [Category("Scrolling")]
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

        /// <summary>
        /// Specifies whether to hide or show the current cell.
        /// </summary>
        /// <value>
        /// <c>True to outline the current cell; false otherwise.</c>
        /// </value>
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
        [
        Browsable(true)//,
            //DefaultValue(GridShowFormulaBehavior.WhenEditing)
        ]
        [Description("Defines when formulas are visible in a formula cell.")]
        [Category("Excel-Emulation")]
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

        public void Dispose()
        {
            this.PropertyChanged = null;
            this.CurrentCellBorder = null;
            this.HiddenBorderBrush = null;
            this.HighlightSelectionAlphaBlend = null;
            this.HighlightSelectionBackground = null;
            this.HighlightSelectionBorder = null;
            this.HighlightSelectionForeground = null;
            this.HighlightSelectionAlphaBlend = null;
        }
    }
}
