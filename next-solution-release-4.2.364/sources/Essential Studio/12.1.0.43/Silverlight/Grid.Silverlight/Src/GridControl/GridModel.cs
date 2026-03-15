#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
//#define TestDrawTextPerformance
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

#if !WinRT
using System;

using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.Styles;
using System.Globalization;

using Hashtable = System.Collections.Generic.Dictionary<object, object>;
using ArrayList = System.Collections.Generic.List<object>;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
using System.Windows.Controls;
using Syncfusion.Windows.Data;

namespace Syncfusion.Windows.Controls.Grid
#else
using Syncfusion.WinRT.ComponentModel;
using Syncfusion.WinRT.Controls.Cells;
using Syncfusion.WinRT.Controls.Scroll;
using Syncfusion.WinRT.GridCommon;
using Syncfusion.WinRT.Styles;
using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.Storage.Pickers;
using System.IO;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using System.Runtime.Serialization;
using System.Text;
using Windows.UI.Xaml;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridModel : Disposable, IGridVolatileCellStylesHost, IOperationFeedbackProvider
    {
        #region Fields
        GridStyleInfo tableStyle = new GridStyleInfo();
        GridStyleInfo headerStyle = new GridStyleInfo();
        GridStyleInfo footerStyle = new GridStyleInfo();
        IEditableLineSizeHost rowHeights = new LineSizeCollection();
        IEditableLineSizeHost columnWidths = new LineSizeCollection();
        GridCoveredCellInfoCollection coveredCells;
        GridOverlappingCellInfoCollection overlappingCells;
        GridCellSpanBackgroundInfoCollection cellSpanBackgrounds;
        GridCellData data = new GridCellData();
        GridVolatileCellStyles volatileCellStyles;
        GridStyleInfoIndexer rowStyles;
        GridStyleInfoIndexer colStyles;
        int headerRows = 1;
        int headerColumns = 1;
        GridBaseStylesMap styleInfoMap = null;

        internal bool isLoaded = false;
        bool ignoreReadOnly;
#if TestDrawTextPerformance
        internal bool SupportsCoveredCells = false;
#else
        internal bool SupportsCoveredCells = true;
#endif
        #endregion
        // Cache scroll values for grid in cell scenarios only.
        double vScrollValue = 0.0;

        public double CachedVScrollValue
        {
            get { return vScrollValue; }
            set { vScrollValue = value; }
        }

        double hScrollValue = 0.0;

        public double CachedHScrollValue
        {
            get { return hScrollValue; }
            set { hScrollValue = value; }
        }

        bool enableFormulaCalculations = true;

        internal bool EnableFormulaCalculations
        {
            get { return enableFormulaCalculations; }
            set { enableFormulaCalculations = value; }
        }
        [XmlIgnore]
        public GridVolatileCellStyles VolatileCellStyles
        {
            get { return volatileCellStyles; }
        }

        private bool enableMultiline;

        [XmlIgnore]
        public bool EnableMultiline
        {
            get
            {
                return enableMultiline;
            }
            set
            {
                enableMultiline = value;
            }
        }
        private GridFormulaEngine formulaEngine;

        [XmlIgnore]
        public GridFormulaEngine FormulaEngine
        {
            get
            {
                if (formulaEngine == null)
                    this.formulaEngine = ((GridCellFormulaModel)this.CellModels["FormulaCell"]).Engine;
                return formulaEngine;
            }
            set
            {
                formulaEngine = value;
            }
        }

        protected virtual void SuspendFormattedTextCalculation()
        {
            SuspendFormulaParsingAndCalculation = true;
        }

        protected virtual void ResumeFormattedTextCalculation()
        {
            SuspendFormulaParsingAndCalculation = false;
        }

        /// <summary>
        /// Suspend Formaula parsing and calculation when we read the formatted from the GridStyleInfo
        /// </summary>
        internal bool SuspendFormulaParsingAndCalculation
        {
            get;
            set;
        }

        public bool IgnoreReadOnly
        {
            get { return ignoreReadOnly; }
            set { ignoreReadOnly = value; }
        }
        #region Ctor
        public GridModel()
        {
            volatileCellStyles = CreateVolatileCellStyles();
            coveredCells = new GridCoveredCellInfoCollection(this);
            overlappingCells = new GridOverlappingCellInfoCollection(this);
            cellSpanBackgrounds = new GridCellSpanBackgroundInfoCollection(this);
            selections = new GridModelSelections(this);
            this.rowStyles = new GridStyleInfoIndexer();
            this.colStyles = new GridStyleInfoIndexer();
#if!WinRT
            this.Options.CopyPasteOption |= CopyPaste.CopyText;
            this.Options.CopyPasteOption |= CopyPaste.CutText;
            this.Options.CopyPasteOption |= CopyPaste.PasteText;
#endif
            RowHeights.DefaultLineSize = 24;
            ColumnWidths.DefaultLineSize = 80;
            RowCount = 1;
            ColumnCount = 1;

            HeaderRows = 1;
            FrozenRows = 1;
            HeaderColumns = 1;
            FrozenColumns = 1;

            var gridLinePen = new Pen(new SolidColorBrush(Colors.DarkGray), 0.5d);
            //gridLinePen.Freeze();

            tableStyle.CellType = "TextBox";
            tableStyle.BorderMargins.Top = gridLinePen.Thickness;
            tableStyle.BorderMargins.Left = gridLinePen.Thickness;
            tableStyle.BorderMargins.Right = gridLinePen.Thickness / 2;
            tableStyle.BorderMargins.Bottom = gridLinePen.Thickness / 2;
#if!WinRT
            tableStyle.Borders.Right = new Pen(new SolidColorBrush(Colors.DarkGray), 0.5d);
            tableStyle.Borders.Bottom = new Pen(new SolidColorBrush(Colors.DarkGray), 0.5d);
            tableStyle.Background = new SolidColorBrush(Colors.White);
            headerStyle.Background = new SolidColorBrush(Colors.LightGray);
#else
            tableStyle.Borders.Right = new Pen(new SolidColorBrush(Color.FromArgb(255, 212, 212, 212)), 0.5d);
            tableStyle.Borders.Bottom = new Pen(new SolidColorBrush(Color.FromArgb(255, 212, 212, 212)), 0.5d);
            tableStyle.Background = new SolidColorBrush(Colors.White);
            tableStyle.Foreground = new SolidColorBrush(Colors.Black);
            headerStyle.Background = new SolidColorBrush(Colors.White);
            headerStyle.Foreground = new SolidColorBrush(Colors.Black);

#endif
            headerStyle.CellType = "Static";
            this.ColumnWidths.LineHiddenChanged += new HiddenRangeChangedEventHandler(ColumnWidths_LineHiddenChanged);
            this.RowHeights.LineHiddenChanged += new HiddenRangeChangedEventHandler(RowHeights_LineHiddenChanged);
            this.rowHeights.DefaultLineSizeChanged += new DefaultLineSizeChangedEventHandler(rowHeights_DefaultLineSizeChanged);
            this.columnWidths.DefaultLineSizeChanged += new DefaultLineSizeChangedEventHandler(columnWidths_DefaultLineSizeChanged);
            this.rowHeights.LineSizeChanged += new RangeChangedEventHandler(rowHeights_LineSizeChanged);
            this.columnWidths.LineSizeChanged += new RangeChangedEventHandler(columnWidths_LineSizeChanged);
            isLoaded = true;
            gridLinePen = null;

        }

        void columnWidths_LineSizeChanged(object sender, RangeChangedEventArgs e)
        {
#if !WinRT
            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new GridModelSetColumnSizeCommand(this, e.From, e.To, e.OldSize));
            }
#endif
        }

        void rowHeights_LineSizeChanged(object sender, RangeChangedEventArgs e)
        {
#if !WinRT
            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new GridModelSetRowSizeCommand(this, e.From, e.To, e.OldSize));
            }
#endif
        }

        void columnWidths_DefaultLineSizeChanged(object sender, DefaultLineSizeChangedEventArgs e)
        {
#if !WinRT
            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new GridModelSetDefaultColumnSizeCommand(this, e.OldValue));
            }
#endif
        }

        void rowHeights_DefaultLineSizeChanged(object sender, DefaultLineSizeChangedEventArgs e)
        {
#if !WinRT
            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new GridModelSetDefaultRowSizeCommand(this, e.OldValue));
            }
#endif
        }

        /// <summary>
        /// Read only collection of hidden columns.
        /// </summary>
        [XmlIgnore]
        public ReadOnlyObservableCollection<GridRangeInfo> HiddenColRanges { get; protected internal set; }
        /// <summary>
        /// Read only collection of hidden rows.
        /// </summary>
        [XmlIgnore]
        public ReadOnlyObservableCollection<GridRangeInfo> HiddenRowRanges { get; protected internal set; }



        void ColumnWidths_LineHiddenChanged(object sender, HiddenRangeChangedEventArgs e)
        {
            var hiddenColRanges = new ObservableCollection<GridRangeInfo>();

            if (this.ColumnWidths.LineCount > 1)
            {
                for (int i = 0; i < this.ColumnWidths.LineCount; i++)
                {
                    if (this.ColumnWidths[i] == 0 && this.ShouldHideColumn(i, false))
                    {
                        hiddenColRanges.Add(GridRangeInfo.Col(i));
                    }
                }
            }
            this.HiddenColRanges = new ReadOnlyObservableCollection<GridRangeInfo>(hiddenColRanges);
#if !WinRT
            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new GridModelSetColumnHideCommand(this, e.From, e.To, !e.Hide));
            }
#endif

        }

        void RowHeights_LineHiddenChanged(object sender, HiddenRangeChangedEventArgs e)
        {
            var hiddenRowRanges = new ObservableCollection<GridRangeInfo>();

            if (this.RowHeights.LineCount > 1)
            {
                for (int i = 0; i < this.RowHeights.LineCount; i++)
                {
                    if (this.RowHeights[i] == 0 && this.ShouldHideRow(i, false))
                    {
                        hiddenRowRanges.Add(GridRangeInfo.Row(i));
                    }
                }
            }
            this.HiddenRowRanges = new ReadOnlyObservableCollection<GridRangeInfo>(hiddenRowRanges);
#if!WinRT

            if (CommandStack.ShouldGenerateUndoInfo)
            {
                if (!(e.From == 0 && e.To == this.RowHeights.LineCount))
                    CommandStack.Push(new GridModelSetRowHideCommand(this, e.From, e.To, !e.Hide));
            }
#endif
        }

        internal bool IsRowHidden(int rowIndex, bool hide)
        {
            return this.ShouldHideRow(rowIndex, hide);
        }

        protected virtual bool ShouldHideRow(int rowIndex, bool hide)
        {
            return true;
        }

        protected virtual bool ShouldHideColumn(int columnIndex, bool hide)
        {
            return true;
        }
#if !WinRT
        #region Column Autosizer
        private GridColumnAutoSizer sizer;
        public GridColumnAutoSizer Sizer
        {
            get
            {
                if (this.sizer == null)
                {
                    this.sizer = this.CreateAutoSizer();
                }

                return this.sizer;
            }
        }

        protected virtual GridColumnAutoSizer CreateAutoSizer()
        {
            return new GridColumnAutoSizer(this);
        }

        internal void UpdateAutoSizer(bool applySizes, bool columnSizerChanged)
        {
            this.OnUpdateAutoSizer(applySizes, columnSizerChanged);
        }

        protected virtual void OnUpdateAutoSizer(bool applySizes, bool columnSizerChanged)
        {
            if (this.Options.ColumnSizer != GridControlLengthUnitType.None)
            {
                if (applySizes)
                {
                    this.Sizer.ApplySizes();
                }
            }
        }
        #endregion
#endif

        protected virtual GridVolatileCellStyles CreateVolatileCellStyles()
        {
            return new GridVolatileCellStyles(this);
        }

        public event EventHandler Disposing;
        internal bool canDisposeGrid = false;
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.ColumnWidths.LineHiddenChanged -= new HiddenRangeChangedEventHandler(ColumnWidths_LineHiddenChanged);
                this.RowHeights.LineHiddenChanged -= new HiddenRangeChangedEventHandler(RowHeights_LineHiddenChanged);

                foreach (GridControlBase grid in this.Views)
                {
                    IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                    if (eventsHost != null)
                    {
                        eventsHost.OnDisposing(disposing);
                    }
                }

                if (Disposing != null)
                {
                    Disposing(this, EventArgs.Empty);
                }


                if (BaseStylesMap != null)
                {
                    this.styleInfoMap.Dispose();
                    this.styleInfoMap = null;
                }

                if (Data != null)
                {
                    this.data.Dispose();
                    this.data = null;
                }

                if (CellModels != null)
                {
                    this.cellModels.Dispose();
                    this.cellModels = null;
                }

                if (OverlappingCells != null)
                {
                    overlappingCells.Dispose();
                    overlappingCells = null;
                }

                if (this.cellSpanBackgrounds.Count > 0)
                {
                    this.cellSpanBackgrounds.Dispose();
                    this.cellSpanBackgrounds = null;
                }

                if (ColStyles != null)
                {
                    this.colStyles = null;
                }

                if (TableStyle != null)
                {
                    this.tableStyle.ClearCache();
                    this.tableStyle.Dispose();
                    this.tableStyle = null;
                }

                if (RowStyles != null)
                {
                    this.rowStyles = null;
                }

                if (ColumnWidths != null)
                {
                    this.columnWidths = null;
                }

                if (RowHeights != null)
                {
                    this.rowHeights = null;
                }

                if (VolatileCellStyles != null)
                {
                    this.volatileCellStyles.Dispose();
                    this.volatileCellStyles = null;
                }

                if (this.coveredCells.Count > 0)
                {
                    this.coveredCells.Dispose();
                    this.coveredCells = null;
                }
                if (this.footerStyle != null)
                    this.footerStyle.Dispose();
#if !WinRT
                if (this.gridCutPaste != null)
                {
                    this.gridCutPaste = null;
                }
#endif
                if (this.headerStyle != null)
                {
                    this.headerStyle.Dispose();
                }
                if (this.HiddenColRanges != null)
                {
                    this.HiddenColRanges = null;
                }
                if (this.HiddenRowRanges != null)
                {
                    this.HiddenRowRanges = null;
                }
                this.options = null;
                if (this.SelectedCells != null)
                {
                    this.SelectedCells.Dispose();
                }
                if (this.selectedRanges != null)
                {
                    this.selectedRanges.Clear();
                    this.selectedRanges = null;
                }
                if (this.selections != null)
                {
                    this.selections.Clear(false);
                    this.selections = null;
                }
#if !WinRT
                if (this.sizer != null)
                {
                    this.sizer.Dispose();
                }
#endif
                if (this.textDataExchange != null)
                    this.textDataExchange = null;
                if (this.userData != null)
                {
                    this.userData.Clear();
                    this.userData = null;
                }
                if (this.CoveredRanges.Count > 0)
                {
                    this.CoveredRanges.Clear();
                }
                if (FormulaEngine != null)
                {
                    this.formulaEngine.Dispose();
                    this.formulaEngine = null;
                }
                if (HeaderStyle != null)
                {
                    this.headerStyle.ClearCache();
                    this.headerStyle.Dispose();
                    this.headerStyle = null;
                }
                if (store != null)
                {
                    store.Dispose();
                }
#if !WinRT
                if (this.GraphicModel != null)
                {
                    this.GraphicModel.Dispose();
                    this.graphicModel = null;
                }
#endif
            }
            base.Dispose(disposing);
        }
        #endregion

        #region ShortCut Properties
        public int RowCount
        {
            get
            {
                return RowHeights.LineCount;
            }

            set
            {
                if (value > RowCount)
                    InsertRows(RowCount, value - RowCount);
                else if (value < RowCount)
                    RemoveRows(value, RowCount - value);
                //RowHeights.LineCount = value;
            }
        }

        public int ColumnCount
        {
            get
            {
                if (ColumnWidths != null)
                    return ColumnWidths.LineCount;
                else
                    return 0;
            }

            set
            {
                if (value > ColumnCount)
                    InsertColumns(ColumnCount, value - ColumnCount);
                else if (value < ColumnCount)
                    RemoveColumns(value, ColumnCount - value);
            }
        }
        #endregion

        #region Options
        GridModelOptions options = new GridModelOptions();

        /// <summary>
        /// A <see cref="GridModelOptions"/> that allows you to adjust behavior and appearance of the grid.
        /// </summary>
        public GridModelOptions Options
        {
            get
            {
                return options;
            }
            set
            {
                options = value;
            }
        }
        #endregion

        #region Cell Height, Content, Spans, VolatileCellStyles
        [XmlIgnore]
        public IEditableLineSizeHost RowHeights
        {
            get { return rowHeights; }
        }

        [XmlIgnore]
        public IEditableLineSizeHost ColumnWidths
        {
            get { return columnWidths; }
            set { columnWidths = value; }
        }

        [XmlIgnore]
        public GridCoveredCellInfoCollection CoveredCells
        {
            get { return coveredCells; }
        }
#if !WinRT
        GraphicModel graphicModel;
        [XmlIgnore]
        public GraphicModel GraphicModel
        {
            get
            {
                if (graphicModel == null)
                {
                    graphicModel = OnGraphicModelCreated();
                }
                return graphicModel;
            }
            set
            {
                graphicModel = value;
            }
        }

        protected virtual GraphicModel OnGraphicModelCreated()
        {
            return new GraphicModel();
        }
#endif
        [XmlIgnore]
        public GridOverlappingCellInfoCollection OverlappingCells
        {
            get { return overlappingCells; }
        }
        [XmlIgnore]
        public GridCellSpanBackgroundInfoCollection CellSpanBackgrounds
        {
            get { return cellSpanBackgrounds; }
        }
        public GridCellData Data
        {
            get { return data; }
        }

        #endregion

        #region Header and Footer
        public int FrozenRows
        {
            get
            {
                return RowHeights.HeaderLineCount;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Negative values not allowed.");
                int savedValue = RowHeights.HeaderLineCount;
                RowHeights.HeaderLineCount = value;
#if !WinRT
                if (CommandStack.ShouldGenerateUndoInfo)
                {
                    CommandStack.Push(new GridModelSetFrozenRowsCountCommand(this, savedValue));
                }
#endif
            }
        }

        public int FrozenColumns
        {
            get
            {
                return ColumnWidths.HeaderLineCount;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Negative values not allowed.");
                int savedValue = ColumnWidths.HeaderLineCount;
                ColumnWidths.HeaderLineCount = value;
#if !WinRT
                if (CommandStack.ShouldGenerateUndoInfo)
                {
                    CommandStack.Push(new GridModelSetFrozenColumnsCountCommand(this, savedValue));
                }
#endif
            }
        }

        public int FooterRows
        {
            get
            {
                return RowHeights.FooterLineCount;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Negative values not allowed.");
                int savedValue = RowHeights.FooterLineCount;
                RowHeights.FooterLineCount = value;
#if !WinRT
                if (CommandStack.ShouldGenerateUndoInfo)
                {
                    CommandStack.Push(new GridModelSetFooterRowsCountCommand(this, savedValue));
                }
#endif
            }
        }

        public int FooterColumns
        {
            get
            {
                return ColumnWidths.FooterLineCount;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Negative values not allowed.");
                int savedValue = ColumnWidths.FooterLineCount;
                ColumnWidths.FooterLineCount = value;
#if !WinRT
                if (CommandStack.ShouldGenerateUndoInfo)
                {
                    CommandStack.Push(new GridModelSetFooterColumnsCountCommand(this, savedValue));
                }
#endif
            }
        }

        public int HeaderRows
        {
            get { return headerRows; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Negative values not allowed.");
                int savedValue = headerRows;
                headerRows = value;
#if !WinRT
                if (CommandStack.ShouldGenerateUndoInfo)
                {
                    CommandStack.Push(new GridModelSetHeaderRowsCountCommand(this, savedValue));
                }
#endif
            }
        }

        public int HeaderColumns
        {
            get { return headerColumns; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Negative values not allowed.");
                int savedValue = headerColumns;
                headerColumns = value;
#if !WinRT
                if (CommandStack.ShouldGenerateUndoInfo)
                {
                    CommandStack.Push(new GridModelSetHeaderColumnsCountCommand(this, savedValue));
                }
#endif
            }
        }

        #endregion

        #region Base Styles
        public GridStyleInfo TableStyle
        {
            get { return tableStyle; }
        }

        public GridStyleInfo HeaderStyle
        {
            get { return headerStyle; }
        }

        public GridStyleInfo FooterStyle
        {
            get { return footerStyle; }
        }

        /// <summary>
        /// Gives you access to the row style information of a row.
        /// </summary>
        /// <value>The row styles.</value>
        /// <overload>
        /// Gives you access to the row style information of a row.
        /// </overload>
        /// <remarks>
        /// The indexer provides you with a very simple way to query and change row style contents.
        /// </remarks>
        /// <example>
        /// The following example make some changes to the grid using the indexer:
        /// <code lang="C#">
        /// model.RowStyles[2].Font.Bold = true;
        /// model.RowStyles[2].Font.Size = 16;
        /// model.RowStyles[2].HorizontalAlignment = GridHorizontalAlignment.Center;
        /// model.RowStyles[2].VerticalAlignment = GridVerticalAlignment.Middle;
        /// model.RowStyles[2].CellType = "Static";
        /// </code>
        /// If you query for specific attributes in a cell and these attributes have not been explicitly set for the cell,
        /// the <see cref="GridStyleInfo"/> object that is returned by the indexer is smart enough to query base styles for
        /// queried information.
        /// <code lang="C#">
        /// model.RowStyles[1].Background = Brushes.Red;
        /// Brush color = model[1, 1].Background;
        /// // model[1, 1].TextColor will return Brushes.Red
        /// </code>
        /// </example>
#if !WinRT
        [Browsable(false)]
#endif
        public GridStyleInfoIndexer RowStyles
        {
            get
            {
                return this.rowStyles;
            }
        }

        /// <overload>
        /// Gives you access to the column style information of a column.
        /// </overload>
        /// <summary>
        /// Gives you access to the column style information of a column.
        /// </summary>
        /// <remarks>
        /// The indexer provides you with a very simple way to query and change column style contents.
        /// </remarks>
        /// <example>
        /// The following example make some changes to the grid using the indexer:
        /// <code lang="C#">
        ///             model.ColStyles[2].Font.Bold = true;
        ///             model.ColStyles[2].Font.Size = 16;
        ///             model.ColStyles[2].HorizontalAlignment = GridHorizontalAlignment.Center;
        ///             model.ColStyles[2].VerticalAlignment = GridVerticalAlignment.Middle;
        ///             model.ColStyles[2].CellType = "Static";
        /// </code>
        /// If you query for specific attributes in a cell and these attributes have not been explicitly set,
        /// the <see cref="GridStyleInfo"/> object that is return by the indexer is smart enough to query base styles for
        /// queried information.
        /// <code lang="C#">
        ///             model.ColStyles[1].Background = Brushes.Red;
        ///                 Brush color = model[1, 1].Background;
        ///                 // model[1, 1].TextColor will return Brushes.Red
        /// </code>
        /// </example>
#if !WinRT
        [Browsable(false)]
#endif
        public GridStyleInfoIndexer ColStyles
        {
            get
            {
                return this.colStyles;
            }
        }

        #endregion

        #region Cell Styles and Events

        protected virtual void OnQueryBaseStyles(GridQueryBaseStylesEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnQueryBaseStyles(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnQueryBaseStyles(e);
            }

            if (QueryBaseStyles != null)
                QueryBaseStyles(this, e);
        }

        protected virtual void OnCommitCellInfo(GridCommitCellInfoEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnCommitCellInfo(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnCommitCellInfo(e);
            }

            if (CommitCellInfo != null)
            {
                CommitCellInfo(this, e);
            }
        }

        protected virtual void OnCommittedCellInfo(GridCommitCellInfoEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnCommittedCellInfo(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnCommittedCellInfo(e);
            }

            if (CommittedCellInfo != null)
                CommittedCellInfo(this, e);
        }

        protected virtual void OnQueryCellInfo(GridQueryCellInfoEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnQueryCellInfo(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnQueryCellInfo(e);
            }

            if (QueryCellInfo != null)
                QueryCellInfo(this, e);
#if !WinRT
            if (e.Style.HasConditionalFormat)
            {
                ApplyConditionalFormating(e.Style);
                e.Handled = true;
            }
#endif
        }

        bool overrideSytle = false;
#if !WinRT
        /// <summary>
        /// Applies the conditional formating.
        /// </summary>
        /// <param name="style">The style.</param>
        private void ApplyConditionalFormating(GridStyleInfo style)
        {
            if (style.HasConditionalFormat)
            {
                string value;
                if (style.ApplyConditionalFormatBasedOn == ApplyConditionalBasedOn.FormulaValue)
                {
                    var condition = style.ConditionalFormat;
                    value = style.GetFormulaValue(condition.FormulaText, condition.FormulaTag);

                    if (style.CellType != "FormulaCell")
                    {
                        FormulaEngine.isFormulainConditionalFormat = true;
                        string formula = condition.FormulaText.Substring(1);
                        FormulaEngine.FormulaContextCell = GridRangeInfo.GetAlphaLabel(style.ColumnIndex) + style.RowIndex.ToString();
                        GridFormulaTag formulaTag = new GridFormulaTag(FormulaEngine.Parse(formula), null, style.RowIndex, style.ColumnIndex);

                        value = FormulaEngine.ComputedValue(formulaTag.Formula);
                        FormulaEngine.isFormulainConditionalFormat = false;
                    }
                }
                else
                {
                    if (style.CellType == "FormulaCell" || style.CellType == "ComboBox")
                    {
                        if (style.FormulaTag != null)
                            value = this.FormulaEngine.ComputedValue(style.FormulaTag.Formula);
                        else
                            value = style.CellValue != null ? style.CellValue.ToString() : string.Empty;
                        
                        if (style.HasFormat)
                        {
                            var zeroFormatedText = 0.ToString(style.Format);
                            if (value == zeroFormatedText || value == string.Empty)
                                value = "0";
                            else
                                value = value.Trim(zeroFormatedText.ToCharArray());
                        }
                    }
                    else
                    {
                        value = style.CellValue != null ? style.CellValue.ToString() : string.Empty;
                    }
                }
                object record;
                int r1;
                double r2;
                bool r3;
                DateTime r4;
                if (int.TryParse(value, out r1))
                {
                    record = r1;
                }
                else if (double.TryParse(value, out r2))
                {
                    record = r2;
                }
                else if (bool.TryParse(value, out r3))
                {
                    record = r3;
                }
                else if (DateTime.TryParse(value, out r4))
                {
                    r2 = (r4.ToOADate());
                    if (r2 < 61)
                    {
                        r2 = (r4.ToOADate() - 1);
                    }
                    record = r2;
                }
                else
                {
                    record = value.ToString();
                }
                var formating = style.ConditionalFormat;
                try
                {
                    if (record.GetType() != typeof(string))
                    {
                        foreach (var condition in formating.Conditions)
                        {
                            if (condition.Value != null)
                            {
                                if (string.IsNullOrWhiteSpace(condition.Value.ToString()) ||
                                    string.IsNullOrEmpty(condition.Value.ToString()))
                                {
                                    record = record.ToString();
                                    break;
                                }
                            }
                        }
                    }
                    else if ((string)record == string.Empty)
                    {
                        foreach (var condition in formating.Conditions)
                        {
                            if (condition.ConditionType == GridConditionType.Equals ||
                                 condition.ConditionType == GridConditionType.NotEquals) continue;
                            record = 0;
                            break;
                        }
                    }
                    var delg = formating.GetCompiledDelegate(style, record.GetType());
                    if (delg != null)
                        overrideSytle = (bool)delg.DynamicInvoke(new object[] { record });
                    else
                        overrideSytle = false;
                }
                catch (Exception ex)
                {
                    overrideSytle = false;
                    throw new InvalidOperationException(ex.Message);
                }
                if (overrideSytle)
                {
                    //If we import excel sheet with conditional formatting then the formatting style was initially stored in the grid model
                    //if we are change the particular cell value then the default style was not applied to that cell
                    //So that the default style was copied and applied to the again when the particular condition failed
                    if (style.ConditionalFormat.defaultStyleStore.IsEmpty)
                    {
                        CopyStyleFromDefaultStyle(style.ConditionalFormat.defaultStyleStore, style.Store, style.ConditionalFormat.Style.Store);
                    }
                    style.ModifyStyle(style.ConditionalFormat.Style, StyleModifyType.Override);
                }
                else if (!style.ConditionalFormat.defaultStyleStore.IsEmpty)
                {
                    style.ModifyStyle(style.ConditionalFormat.defaultStyleStore as GridStyleInfoStore, StyleModifyType.Override);
                }
            }
        }
#endif
        private void CopyStyleFromDefaultStyle(GridStyleInfoStore targetStyleInfoStore, GridStyleInfoStore sourceStyleInfoStore, GridStyleInfoStore basedOnStyle)
        {
            if (basedOnStyle.IsEmpty)
                return;
            foreach (StyleInfoProperty sip in basedOnStyle.StyleInfoProperties)
            {
                if (basedOnStyle.HasValue(sip))
                {
                    var value = sourceStyleInfoStore.GetValue(sip);
                    targetStyleInfoStore.SetValue(sip, value);
                }
            }
        }

        public event GridQueryCellInfoEventHandler QueryCellInfo;
        public event GridCommitCellInfoEventHandler CommitCellInfo;
        public event GridCommitCellInfoEventHandler CommittedCellInfo;
        public event GridQueryBaseStylesEventHandler QueryBaseStyles;

        public GridStyleInfo this[int rowIndex, int columnIndex]
        {
            get
            {
                return volatileCellStyles[rowIndex, columnIndex];
            }
            set
            {
                volatileCellStyles[rowIndex, columnIndex].CopyFrom(value);
            }
        }


        #endregion

        #region VolatileCellStylesHost Members

        //void IGridVolatileCellStylesHost.SaveCellInfo(RowColumnIndex cell, GridStyleInfo style)
        //{
        //    SaveCellInfoEventArgs e = new SaveCellInfoEventArgs(cell, style);
        //    OnSaveCellInfo(e);
        //    if (!e.Handled)
        //    {
        //        GridStyleInfoStore store = data[cell.RowIndex, cell.ColumnIndex];
        //        if (store != null)
        //            store.ModifyStyle(style.Store, StyleModifyType.Copy);
        //        else
        //        {
        //            store = new GridStyleInfoStore();
        //            data[cell.RowIndex, cell.ColumnIndex] = store;
        //            style.Store.CopyTo(store);
        //        }
        //    }
        //}
        GridStyleInfoStore store;
        void IGridVolatileCellStylesHost.QueryCellInfo(RowColumnIndex cell, GridStyleInfo style)
        {
            store = data[cell.RowIndex, cell.ColumnIndex];
            if (store != null)
                style.ModifyStyle(store, StyleModifyType.Override);
            GridQueryCellInfoEventArgs e = new GridQueryCellInfoEventArgs(cell, style);
            OnQueryCellInfo(e);
            if (!e.Handled)
            {
            }
        }

        IStyleInfo[] IGridVolatileCellStylesHost.QueryBaseStyles(RowColumnIndex cell, GridStyleInfo style)
        {
            GridQueryBaseStylesEventArgs e = new GridQueryBaseStylesEventArgs(cell, style);
            OnQueryBaseStyles(e);
            if (!e.Handled)
            {
                if (cell.ColumnIndex < HeaderColumns
                    || cell.RowIndex < HeaderRows)
                    e.BaseStyles.Add(headerStyle);

                GridStyleInfo rowStyle;
                this.rowStyles.TryGetValue(e.Cell.RowIndex, out rowStyle);
                GridStyleInfo colStyle;
                this.colStyles.TryGetValue(e.Cell.ColumnIndex, out colStyle);
                if (this.options.RowColStylePrecedence == PrecedenceStyle.Row)
                {
                    if (rowStyle != null)
                        e.BaseStyles.Add(rowStyle);
                    else if (colStyle != null)
                        e.BaseStyles.Add(colStyle);
                }
                else if (this.options.RowColStylePrecedence == PrecedenceStyle.Column)
                {
                    if (colStyle != null)
                        e.BaseStyles.Add(colStyle);
                    if (rowStyle != null)
                        e.BaseStyles.Add(rowStyle);
                }
                if (cell.ColumnIndex >= ColumnWidths.LineCount - FooterColumns
                    || cell.RowIndex >= RowHeights.LineCount - FooterRows)
                    e.BaseStyles.Add(footerStyle);

                e.BaseStyles.Add(tableStyle);
            }


            // Base style.
            string baseStyleName = style.HasBaseStyle ? style.BaseStyle : "";

            if (baseStyleName.Length == 0)
            {
                foreach (GridStyleInfo si in e.BaseStyles)
                {
                    if (si.HasBaseStyle)
                    {
                        baseStyleName = si.BaseStyle;
                        if (baseStyleName.Length > 0)
                            break;
                    }
                }
            }

            // Row or column header.
            //if (baseStyleName.Length == 0)
            //{
            //    if (cell.RowIndex >= 0 && cell.ColumnIndex >= 0)
            //    {
            //        if (cell.RowIndex <= RowHeights.HeaderLineCount && cell.ColumnIndex <= ColumnWidths.HeaderLineCount)
            //            baseStyleName = "Header";
            //        else if (cell.RowIndex <= RowHeights.HeaderLineCount)
            //            baseStyleName = "Column Header";
            //        else if (cell.ColumnIndex <= ColumnWidths.HeaderLineCount)
            //            baseStyleName = "Row Header";
            //    }
            //}

            // Load all parent base styles (including standard style).
            int level;
            GridStyleInfo[] infoMapStyles = BaseStylesMap.GetBaseStylesMapStyles(baseStyleName, out level);

            int cellStyleCount = e.BaseStyles.Count;
            // Combine the two arrays.
            GridStyleInfo[] baseStyles = new GridStyleInfo[cellStyleCount + level];
            for (int n = 0; n < cellStyleCount; n++)
                baseStyles[n + level] = e.BaseStyles[n];
            if (infoMapStyles != null)
                Array.Copy(infoMapStyles, 0, baseStyles, 0, level);

            // Each GridStyleInfoIdentity will cache the baseStyles.
            return baseStyles;
        }

        void IGridVolatileCellStylesHost.CommitCellInfo(RowColumnIndex cell, GridStyleInfo style, StyleInfoProperty sip)
        {
            GridCommitCellInfoEventArgs e = new GridCommitCellInfoEventArgs(cell, style, sip);
            OnCommitCellInfo(e);
            if (!e.Handled)
            {
                store = data[cell.RowIndex, cell.ColumnIndex];
                if (store == null)
                {
                    store = new GridStyleInfoStore();
                    if (cell.RowIndex > -1 && cell.ColumnIndex > -1)
                        data[cell.RowIndex, cell.ColumnIndex] = store;
                }
                if (sip != null)
                    store.SetValue(sip, style.Store.GetValue(sip));
                else
                    store.ModifyStyle(style.Store, StyleModifyType.Changes);

                if (sip != null && sip.PropertyName != "FormulaTag")
                {
                    GridRangeInfo range = GridRangeInfo.Auto(cell.RowIndex, cell.ColumnIndex);
                    ChangeCells(range, new GridStyleInfo[] { style }, StyleModifyType.Changes);
                }
            }
            OnCommittedCellInfo(e);            
        }
        #endregion

        #region Insert and Remove Rows

        public virtual void InsertRows(int insertAtRowIndex, int count)
        {
            InsertRowsCore(insertAtRowIndex, count, null);

            GridRangeInsertedEventArgs e = new GridRangeInsertedEventArgs(insertAtRowIndex, count);
            OnRowsInserted(e);
#if !WinRT
            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new GridModelRemoveRowsCommand(this, insertAtRowIndex, count));
            }
#endif
        }

        public event GridRangeInsertedEventHandler RowsInserted;

        protected virtual void OnRowsInserted(GridRangeInsertedEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnRowsInserted(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnRowsInserted(e);
            }

            if (RowsInserted != null)
                RowsInserted(this, e);
        }

        protected virtual void InsertRowsCore(int insertAtRowIndex, int count, GridMoveCellsState moveCellsState)
        {
            if (moveCellsState == null) moveCellsState = GridMoveCellsState.Empty;

            VolatileCellStyles.InsertRows(insertAtRowIndex, count, moveCellsState.VolatileCellStyles);
            if (RowHeights.SupportsInsertRemove)
                RowHeights.InsertLines(insertAtRowIndex, count, moveCellsState.LineSizes);
            Data.InsertRows(insertAtRowIndex, count, moveCellsState.Data);
            CoveredCells.InsertRows(insertAtRowIndex, count, moveCellsState.CoveredCells);
            CellSpanBackgrounds.InsertRows(insertAtRowIndex, count, moveCellsState.CellSpanBackgrounds);

            foreach (GridControlBase gridView in views)
            {
                if (moveCellsState.IsEmpty)
                    gridView.ModelInsertRows(insertAtRowIndex, count, null);
                else
                    gridView.ModelInsertRows(insertAtRowIndex, count, moveCellsState.GridViews[gridView]);
            }

            Selections.InsertRows(insertAtRowIndex, count, moveCellsState.Selections);
        }

        public virtual void RemoveRows(int removeAtRowIndex, int count)
        {
            RemoveRowsCore(removeAtRowIndex, count, null);

            GridRangeRemovedEventArgs e = new GridRangeRemovedEventArgs(removeAtRowIndex, count);
            OnRowsRemoved(e);
#if !WinRT
            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new GridModelInsertRowsCommand(this, removeAtRowIndex, count));
            }
#endif
        }

        public event GridRangeRemovedEventHandler RowsRemoved;

        protected virtual void OnRowsRemoved(GridRangeRemovedEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnRowsRemoved(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnRowsRemoved(e);
            }

            if (RowsRemoved != null)
                RowsRemoved(this, e);
        }

        protected virtual void RemoveRowsCore(int removeAtRowIndex, int count, GridMoveCellsState moveCellsState)
        {
            if (moveCellsState == null) moveCellsState = GridMoveCellsState.Empty;

            VolatileCellStyles.RemoveRows(removeAtRowIndex, count, moveCellsState.VolatileCellStyles);
            if (RowHeights.SupportsInsertRemove)
                RowHeights.RemoveLines(removeAtRowIndex, count, moveCellsState.LineSizes);
            Data.RemoveRows(removeAtRowIndex, count, moveCellsState.Data);
            CoveredCells.RemoveRows(removeAtRowIndex, count, moveCellsState.CoveredCells);
            CellSpanBackgrounds.RemoveRows(removeAtRowIndex, count, moveCellsState.CellSpanBackgrounds);
            GridControlBase grid = null;

            foreach (GridControlBase gridView in views)
            {
                grid = gridView;
                if (moveCellsState.IsEmpty)
                    gridView.ModelRemoveRows(removeAtRowIndex, count, null);
                else
                {
                    GridViewMoveCellsState viewState = gridView.CreateGridViewMoveCellsState();
                    gridView.ModelRemoveRows(removeAtRowIndex, count, viewState);
                    moveCellsState.GridViews[gridView] = viewState;
                }
            }
#if !WinRT
            var suspendSelection = moveCellsState != null ? moveCellsState.SuspendSelections : false;
            if (!suspendSelection)
            {
                GridDataControl dataGrid = null;
                CollectionViewAdv view = null;
                if (grid != null)
                {
                    dataGrid = grid.FindParentElementOfType<GridDataControl>();
                    if (dataGrid != null)
                        view = dataGrid.Model.View as CollectionViewAdv;
                }
                if (dataGrid != null && dataGrid.Model.IsInDeteteRecord && view != null && !view.IsInPropertyChange && dataGrid.Model.options.ListBoxSelectionMode != GridSelectionMode.None && this.SelectedRanges.Count > 0)
                {
                    if (removeAtRowIndex == this.RowCount)
                    {
                        removeAtRowIndex = removeAtRowIndex - 1;//If deleted row is the last row then selection should remain in the previous row.
                        grid.CurrentCell.MoveTo(removeAtRowIndex, grid.CurrentCell.ColumnIndex);
                    }

                    // Selections.RemoveRows(removeAtRowIndex, count, moveCellsState.Selections);//Previous Code
                    //Previously while deleting the Row, Deleted row selection remove here. But newly selected item doesn't set here
                    // Now It will achieve through the RaiseSelectionChanged event
                    GridSelectionChangedEventArgs e = new GridSelectionChangedEventArgs(GridRangeInfo.Row(removeAtRowIndex), SelectedRanges, GridSelectionReason.DeleteRow);
                    this.RaiseSelectionChanged(e);
                }
                else
                {
                    if (dataGrid != null)
                    {
                        if (dataGrid.Model.TableProperties.AutoFocusCurrentItem)
                            Selections.RemoveRows(removeAtRowIndex, count, moveCellsState.Selections);
                    }
                    else
                        Selections.RemoveRows(removeAtRowIndex, count, moveCellsState.Selections);
                }
            }
#endif

        }

        #endregion

        #region Insert and Remove Columns

        public virtual void InsertColumns(int insertAtColumnIndex, int count)
        {
            InsertColumnsCore(insertAtColumnIndex, count, null);

            GridRangeInsertedEventArgs e = new GridRangeInsertedEventArgs(insertAtColumnIndex, count);
            OnColumnsInserted(e);
#if !WinRT
            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new GridModelRemoveColumnsCommand(this, insertAtColumnIndex, count));
            }
#endif
        }

        public event GridRangeInsertedEventHandler ColumnsInserted;

        protected virtual void OnColumnsInserted(GridRangeInsertedEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnColumnsInserted(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnColumnsInserted(e);
            }

            if (ColumnsInserted != null)
                ColumnsInserted(this, e);
        }

        protected virtual void InsertColumnsCore(int insertAtColumnIndex, int count, GridMoveCellsState moveCellsState)
        {
            if (moveCellsState == null) moveCellsState = GridMoveCellsState.Empty;

            VolatileCellStyles.InsertColumns(insertAtColumnIndex, count, moveCellsState.VolatileCellStyles);
            if (ColumnWidths.SupportsInsertRemove)
                ColumnWidths.InsertLines(insertAtColumnIndex, count, moveCellsState.LineSizes);
            Data.InsertColumns(insertAtColumnIndex, count, moveCellsState.Data);
            CoveredCells.InsertColumns(insertAtColumnIndex, count, moveCellsState.CoveredCells);
            CellSpanBackgrounds.InsertColumns(insertAtColumnIndex, count, moveCellsState.CellSpanBackgrounds);

            foreach (GridControlBase gridView in views)
            {
                if (moveCellsState.IsEmpty)
                    gridView.ModelInsertColumns(insertAtColumnIndex, count, null);
                else
                    gridView.ModelInsertColumns(insertAtColumnIndex, count, moveCellsState.GridViews[gridView]);
            }
            Selections.InsertColumns(insertAtColumnIndex, count, moveCellsState.Selections);
        }

        public virtual void RemoveColumns(int removeAtColumnIndex, int count)
        {
            RemoveColumnsCore(removeAtColumnIndex, count, null);

            GridRangeRemovedEventArgs e = new GridRangeRemovedEventArgs(removeAtColumnIndex, count);
            OnColumnsRemoved(e);
#if !WinRT
            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new GridModelInsertColumnsCommand(this, removeAtColumnIndex, count));
            }
#endif
        }

        public event GridRangeRemovedEventHandler ColumnsRemoved;

        protected virtual void OnColumnsRemoved(GridRangeRemovedEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnColumnsRemoved(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnColumnsRemoved(e);
            }

            if (ColumnsRemoved != null)
                ColumnsRemoved(this, e);
        }

        protected virtual void RemoveColumnsCore(int removeAtColumnIndex, int count, GridMoveCellsState moveCellsState)
        {
            if (moveCellsState == null) moveCellsState = GridMoveCellsState.Empty;

            VolatileCellStyles.RemoveColumns(removeAtColumnIndex, count, moveCellsState.VolatileCellStyles);
            if (ColumnWidths.SupportsInsertRemove)
                ColumnWidths.RemoveLines(removeAtColumnIndex, count, moveCellsState.LineSizes);
            Data.RemoveColumns(removeAtColumnIndex, count, moveCellsState.Data);
            CoveredCells.RemoveColumns(removeAtColumnIndex, count, moveCellsState.CoveredCells);
            CellSpanBackgrounds.RemoveColumns(removeAtColumnIndex, count, moveCellsState.CellSpanBackgrounds);

            foreach (GridControlBase gridView in views)
            {
                if (moveCellsState.IsEmpty)
                    gridView.ModelRemoveColumns(removeAtColumnIndex, count, null);
                else
                {
                    GridViewMoveCellsState viewState = gridView.CreateGridViewMoveCellsState();
                    gridView.ModelRemoveColumns(removeAtColumnIndex, count, viewState);
                    moveCellsState.GridViews[gridView] = viewState;
                }
            }

            Selections.RemoveColumns(removeAtColumnIndex, count, moveCellsState.Selections);
        }

        #endregion

        #region Move Rows or Columns

        public void MoveRows(int removeAtRowIndex, int count, int insertAtRowIndex)
        {
            GridMoveCellsState moveCellsState = CreateGridMoveCellsState(RowHeights.CreateMoveLines());
            RemoveRowsCore(removeAtRowIndex, count, moveCellsState);
            InsertRowsCore(insertAtRowIndex, count, moveCellsState);

            GridRangeMovedEventArgs e = new GridRangeMovedEventArgs(removeAtRowIndex, count, insertAtRowIndex);
            OnRowsMoved(e);
#if !WinRT
            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new GridModelMoveRowsCommand(this, removeAtRowIndex, count, insertAtRowIndex));
            }
#endif
        }

        public event GridRangeMovedEventHandler RowsMoved;

        protected virtual void OnRowsMoved(GridRangeMovedEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnRowsMoved(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnRowsMoved(e);
            }

            if (RowsMoved != null)
                RowsMoved(this, e);
        }

        public void MoveColumns(int removeAtColumnIndex, int count, int insertAtColumnIndex)
        {
            GridMoveCellsState moveCellsState = CreateGridMoveCellsState(ColumnWidths.CreateMoveLines());
            RemoveColumnsCore(removeAtColumnIndex, count, moveCellsState);
            InsertColumnsCore(insertAtColumnIndex, count, moveCellsState);

            GridRangeMovedEventArgs e = new GridRangeMovedEventArgs(removeAtColumnIndex, count, insertAtColumnIndex);
            OnColumnsMoved(e);
#if !WinRT
            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new GridModelMoveColumnsCommand(this, removeAtColumnIndex, count, insertAtColumnIndex));
            }
#endif
        }

        public event GridRangeMovedEventHandler ColumnsMoved;

        protected virtual void OnColumnsMoved(GridRangeMovedEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnColumnsMoved(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnColumnsMoved(e);
            }

            if (ColumnsMoved != null)
                ColumnsMoved(this, e);
        }

        protected virtual GridMoveCellsState CreateGridMoveCellsState(IEditableLineSizeHost lineSizes)
        {
            return new GridMoveCellsState(lineSizes);
        }
        #endregion

        #region BaseStylesMap
        /// <summary>
        /// The <see cref="GridBaseStylesMap"/> that is associated with this <see cref="GridModel"/>.
        /// </summary>
        [XmlIgnore]
        public GridBaseStylesMap BaseStylesMap
        {
            get
            {
                if (styleInfoMap == null)
                {
                    styleInfoMap = OnCreateBaseStylesMap();
                    OnBaseStylesMapChanged(EventArgs.Empty);
                }
                return styleInfoMap;
            }
            set
            {
                styleInfoMap = value;
                OnBaseStylesMapChanged(EventArgs.Empty);
            }
        }
        /// <summary>
        /// Determines whether <see cref="GridModel.BaseStylesMap"/> has been associated with this <see cref="GridModel"/>.
        /// </summary>
        public bool HasBaseStylesMap
        {
            get
            {
                return styleInfoMap != null;
            }
        }
        /// <summary>
        /// This method is called the first time <see cref="GridModel.BaseStylesMap"/> and no
        /// <see cref="GridBaseStylesMap"/> has been associated with the <see cref="GridModel"/> before.
        /// </summary>
        /// <returns>A <see cref="GridBaseStylesMap"/> object.</returns>
        protected virtual GridBaseStylesMap OnCreateBaseStylesMap()
        {
            GridBaseStylesMap styleInfoMap = new GridBaseStylesMap();
            styleInfoMap.RegisterStandardStyles();
            return styleInfoMap;
        }

        /// <summary>
        /// Raises the BaseStylesMapChanged event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data. </param>
        protected virtual void OnBaseStylesMapChanged(EventArgs e)
        {
            try
            {
                foreach (GridControlBase grid in this.Views)
                {
                    IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                    if (eventsHost != null)
                    {
                        eventsHost.OnBaseStyleMapsChanged(e);
                    }
                }

                if (this.EventsHost != null)
                    this.EventsHost.OnBaseStyleMapsChanged(e);

                if (BaseStylesMapChanged != null)
                    BaseStylesMapChanged(this, e);
            }
            catch (Exception ex)
            {
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    throw;
            }
        }

        internal void RaiseBaseStylesMapChanged(EventArgs e)
        {
            OnBaseStylesMapChanged(e);
        }

        /// <summary>
        /// Occurs when the reference for <see cref="GridModel.BaseStylesMap"/> in <see cref="GridModel"/> has changed.
        /// </summary>
#if !WinRT
        [
        Description("Occurs when the reference to the BaseStylesMap has changed."),
        Category("Behavior")
        ]
#endif
        public event EventHandler BaseStylesMapChanged;


        #endregion

        #region CellModels
        GridCellModelCollection cellModels = null;

        /// <summary>
        /// Manages cell types for the grid.
        /// </summary>
        [XmlIgnore]
        public GridCellModelCollection CellModels
        {
            get
            {
                if (cellModels == null)
                    cellModels = new GridCellModelCollection(this);
                return cellModels;
            }
        }

        GridCellModelBase IGridVolatileCellStylesHost.LookupCellModel(string id)
        {
            return CellModels[id];
        }


        ///// <summary>
        ///// Occurs when the CellModels collection is changed.
        ///// </summary>
        //[
        //Description("Occurs when the CellModels collection is changed"),
        //Category("Behavior")
        //]
        //public event CollectionChangeEventHandler CellModelsChanged;

        ///// <summary>
        ///// Raises the <see cref="GridModel.CellModelsChanged"/> event.
        ///// </summary>
        ///// <param name="e">A <see cref="CollectionChangeEventArgs" /> that contains the event data.</param>// Events
        //protected virtual void OnCellModelsChanged(CollectionChangeEventArgs e)
        //{
        //    foreach (GridControlBase grid in this.Views)
        //    {
        //        IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
        //        if (eventsHost != null)
        //        {
        //            eventsHost.OnCellModelsChanged(e);
        //        }
        //    }

        //    if (this.EventsHost != null)
        //    {
        //        this.EventsHost.OnCellModelsChanged(e);
        //    }

        //    if (CellModelsChanged != null)
        //    {
        //        CellModelsChanged(this, e);
        //    }
        //}

        internal bool ignoreCellModelsChanged = false;

        //internal void RaiseCellModelsChanged(CollectionChangeEventArgs e)
        //{
        //    if (!ignoreCellModelsChanged)
        //        OnCellModelsChanged(e);
        //}



        /// <summary>
        /// Occurs when the <see cref="GridModel.QueryCellModel"/> is querying for the <see cref="GridCellModelBase"/>
        /// and the cell type is not found in the GridCellModelCollection.
        /// </summary>
        /// <remarks>
        /// The GridModel has a table with all cell types used in the grid. Whenever the grid encounters
        /// a new cell type that it cannot find in the table it will raise a <see cref="GridModel.QueryCellModel"/> event.
        /// The <see cref="GridStyleInfo.CellType"/> identifies the name of the cell type. The
        /// <see cref="GridQueryCellModelEventArgs.CellModel"/> should receive the new instance of the
        /// associated cell object. This object will be stored in the table together with its name and
        /// reused among cells with the same <see cref="GridStyleInfo.CellType"/>.
        /// <para/>
        /// You should process this event if you want to add custom cell types and initialize these
        /// cell types on demand when associated cells are accessed the first time.
        /// </remarks>
        /// <seealso cref="GridQueryCellModelEventHandler"/>
#if !WinRT
        [
        Description("Occurs is querying for a cell type."),
        Category("Behavior")
        ]
#endif
        public event GridQueryCellModelEventHandler QueryCellModel;

        /// <summary>
        /// Raises the <see cref="GridModel.QueryCellModel"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryCellModelEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCellModel(GridQueryCellModelEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnQueryCellModel(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnQueryCellModel(e);
            }

            if (QueryCellModel != null)
                QueryCellModel(this, e);
        }

        /// <internalonly/>
        public void RaiseQueryCellModel(GridQueryCellModelEventArgs e)
        {
            OnQueryCellModel(e);

            if (e.CellModel == null)
            {
                IGridCellModelFactory pGridCellObjectFactory = GridFactoryProvider.CellModelFactory;

                if (pGridCellObjectFactory == null)
                {
                    pGridCellObjectFactory = new GridBaseCellModelFactory(true);
                    GridFactoryProvider.Init(pGridCellObjectFactory);
                }
                e.CellModel = pGridCellObjectFactory.CreateCellModel(e.CellType, this);
            }
        }

        #endregion

        #region CellText
#if !WinRT
        [
        Description("Occurs each time the GridStyleInfo.Text is called to get the string that represents the underlying cell's value"),
        Category("Data")
        ]
#endif
        public event GridCellTextEventHandler QueryCellFormattedText;
#if !WinRT
        [
        Description("Occurs each time the GridStyleInfo.FormattedText is called to set the raw string that represents the underlying cell's value."),
        Category("Data")
        ]
#endif
        public event GridCellTextEventHandler SaveCellFormattedText;
#if !WinRT
        [
        Description("Handle this event to provide support for parsing the formatted string and convert it into the the underlying cell's value."),
        Category("Data")
        ]
#endif
        public event GridCellTextEventHandler ParseCommonFormats;
#if!WinRT
        [
Description("Occurs each time the GridStyleInfo.Text is called to get the string that represents the underlying cell's value"),
Category("Data")
]
#endif
        public event GridCellTextEventHandler QueryCellText;

        protected virtual void OnQueryCellText(GridCellTextEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnQueryCellText(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnQueryCellText(e);
            }

            if (QueryCellText != null)
                QueryCellText(this, e);
        }

        public void RaiseQueryCellText(GridCellTextEventArgs e)
        {
            OnQueryCellText(e);
        }
#if !WinRT
        [
        Description("Occurs each time the GridStyleInfo.Text is called to set the raw string that represents the underlying cell's value."),
        Category("Data")
        ]
#endif
        public event GridCellTextEventHandler SaveCellText;

        protected virtual void OnSaveCellText(GridCellTextEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnSaveCellText(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnSaveCellText(e);
            }

            if (SaveCellText != null)
                SaveCellText(this, e);
        }

        public void RaiseSaveCellText(GridCellTextEventArgs e)
        {
            OnSaveCellText(e);
        }

        protected virtual void OnQueryCellFormattedText(GridCellTextEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnQueryCellFormattedText(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnQueryCellFormattedText(e);
            }

            if (QueryCellFormattedText != null)
                QueryCellFormattedText(this, e);
        }

        public void RaiseQueryCellFormattedText(GridCellTextEventArgs e)
        {
            OnQueryCellFormattedText(e);
        }

        protected virtual void OnSaveCellFormattedText(GridCellTextEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnSaveCellFormattedText(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnSaveCellFormattedText(e);
            }

            if (SaveCellFormattedText != null)
                SaveCellFormattedText(this, e);
        }

        public void RaiseSaveCellFormattedText(GridCellTextEventArgs e)
        {
            OnSaveCellFormattedText(e);
        }

        protected virtual void OnParseCommonFormats(GridCellTextEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnParseCommonFormats(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnParseCommonFormats(e);
            }

            if (ParseCommonFormats != null)
                ParseCommonFormats(this, e);
        }

        public void RaiseParseCommonFormats(GridCellTextEventArgs e)
        {
            OnParseCommonFormats(e);

            DefaultParseCommonFormats(e);
        }

        internal void DefaultParseCommonFormats(GridCellTextEventArgs e)
        {
            if (!e.Handled)
            {
                if (e.Text == "" || e.Text == null)
                    return;
                //If this is a numeric type with a common format that is not automatically handled by the Parse
                //routine, we'll take care of it here.

                switch (e.Style.Format)
                {
                    case "P":
                        if (!e.Text.EndsWith("%"))
                            return;
                        string s = e.Text;
                        s = s.TrimEnd('%');
                        decimal d = decimal.Parse(s, System.Globalization.NumberStyles.Any, e.Style.GetCulture(true)) * (decimal)0.01;
                        e.Style.CellValue = Convert.ChangeType(d, e.Style.CellValueType, null);
                        e.Handled = true;
                        break;
                    case "X":
                        Int64 i = Int64.Parse(e.Text, System.Globalization.NumberStyles.AllowHexSpecifier, e.Style.GetCulture(true));
                        e.Style.CellValue = Convert.ChangeType(i, e.Style.CellValueType, null);
                        e.Handled = true;
                        break;
                    default:
                        return;
                }
            }
        }

        #endregion

        #region QueryCoveredRange
        internal void RaiseQueryCoveredRange(GridQueryCoveredRangeEventArgs e)
        {
            OnQueryCoveredRange(e);
        }

        protected virtual void OnQueryCoveredRange(GridQueryCoveredRangeEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnQueryCoveredRange(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnQueryCoveredRange(e);
            }

            if (QueryCoveredRange != null)
                QueryCoveredRange(this, e);
        }

        public event GridQueryCoveredRangeEventHandler QueryCoveredRange;
        #endregion

        #region QueryCellSpanBackgrounds
        internal void RaiseQueryCellSpanBackgrounds(GridQueryCellSpanBackgroundsEventArgs e)
        {
            OnQueryCellSpanBackgrounds(e);
        }

        protected virtual void OnQueryCellSpanBackgrounds(GridQueryCellSpanBackgroundsEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnQueryCellSpansBackground(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnQueryCellSpansBackground(e);
            }

            if (QueryCellSpanBackgrounds != null)
                QueryCellSpanBackgrounds(this, e);
        }

        public event GridQueryCellSpanBackgroundsEventHandler QueryCellSpanBackgrounds;
        #endregion

        #region Views

        List<GridControlBase> views = new List<GridControlBase>();

        public IEnumerable<GridControlBase> Views
        {
            get { return views; }
        }

        internal void RemoveView(GridControlBase gridControlBase)
        {
            views.Remove(gridControlBase);
            if (CurrentCellState.GridControl == gridControlBase)
                currentCellState.GridControl = null;
            foreach (GridCellModelBase cellModel in CellModels.Values)
            {
                if (cellModel.ActiveRenderer == gridControlBase)
                    cellModel.ActiveRenderer = null;
            }
        }

        internal void AddView(GridControlBase gridControlBase)
        {
            views.Add(gridControlBase);
            if (CurrentCellState.GridControl == null && !CurrentCellState.IsEmpty)
                currentCellState.GridControl = gridControlBase;
#if !WinRT
            if (GraphicModel != null && GraphicModel.GridControl == null)
                graphicModel.SetGridControl(gridControlBase);
#endif
        }

        #endregion

        #region InvalidateCell

        /// <summary>
        /// Calls GridControlBase.InvalidateCell for each GridControlBase object
        /// associated with this GridModel.
        /// </summary>
        /// <param name="cellRowColumnIndex"></param>
        public void InvalidateCell(RowColumnIndex cellRowColumnIndex)
        {
            foreach (GridControlBase g in views)
            {
                g.InvalidateCell(cellRowColumnIndex);
            }
        }

        public void InvalidateCell(CellSpanInfoBase span)
        {
            foreach (GridControlBase g in views)
            {
                g.InvalidateCell(span);
            }
        }

        public void InvalidateCell(GridRangeInfo gridRangeInfo)
        {
            CellSpanInfoBase span = gridRangeInfo.ToCellSpan(this);
            InvalidateCell(span);
        }

        public void InvalidateVisual()
        {
            InvalidateVisual(true);
        }

        public void InvalidateVisual(bool setArrangeDirty)
        {
            foreach (GridControlBase g in views)
            {
                g.InvalidateVisual(setArrangeDirty);
            }
        }

        #endregion

        #region Selected Cells

        GridModelSelections selections;

        /// <summary>
        /// Manages selected ranges in the grid. Allows you to add and remove selections, determines
        /// selection state of a specific cell and more.
        /// </summary>
        [XmlIgnore]
        public GridModelSelections Selections
        {
            get
            {
                return selections;
            }
        }

        public GridRangeInfo SelectedCells
        {
            get
            {
                if (SelectedRanges.ActiveRange != null)
                    return SelectedRanges.ActiveRange;
                return GridRangeInfo.Empty;
            }
            set
            {
                Selections.SelectRange(SelectedCells, false);
                Selections.SelectRange(value, true);
            }
        }

        #endregion
        #region CurrentCellState, SelectedRanges
        GridModelCurrentCellState currentCellState = GridModelCurrentCellState.Empty;

        /// <summary>
        /// GridCurrentCell.Activate and GridCurrentCell.Deactivate
        /// set and reset this state.
        /// </summary>
        public GridModelCurrentCellState CurrentCellState
        {
            get { return currentCellState; }
            set { currentCellState = value; }
        }

        GridRangeInfoList selectedRanges = null;

        public virtual GridRangeInfoList SelectedRanges
        {
            get
            {
                if (selectedRanges == null)
                    selectedRanges = new GridRangeInfoList();
                return selectedRanges;
            }
        }
        #endregion

        #region Selection Events
        #region SelectionChanged
        internal void RaiseSelectionChanged(GridSelectionChangedEventArgs e)
        {
            OnSelectionChanged(e);
            selectionStateChanged = true;
        }

        /// <summary>
        /// Raises the <see cref="GridModel.SelectionChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridSelectionChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnSelectionChanged(GridSelectionChangedEventArgs e)
        {
            if (!CanGridRaiseEvents) return;
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnSelectionChanged(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnSelectionChanged(e);
            }

            if (SelectionChanged != null)
                SelectionChanged(this, e);
        }

        /// <summary>
        /// Occurs after the model updates its internal data structures when the model in the process of selecting
        /// a range of cells as a result of a <see cref="GridModelSelections.SelectRange"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridModel"/> will raise a  <see cref="GridModel.SelectionChanging"/> event before
        /// it updates its internal data structures and a  <see cref="GridModel.SelectionChanged"/> event after
        /// afterwards. A <see cref="GridControlBase"/> grid listens to this event and outline
        /// the selected range of cells.
        /// </remarks>
#if !WinRT
        [
        Description("Occurs after internal data structures were updated with new selection state from a SelectRange command."),
        Category("Behavior")
        ]
#endif
        public event GridSelectionChangedEventHandler SelectionChanged;
        #endregion
        #region SelectionChanging

        internal void RaiseSelectionChanging(GridSelectionChangingEventArgs e)
        {
            // Be aware that when GridOptions.ExcelLikeCurrentCell is set and you cancel the current cell 
            // activation in CurrentCellActivating event that RaiseSelectionChanging already was already 
            // called earlier from ProcessSetCurrentCell. If you do not want this you should also handle 
            // the SelectionChanging event.
            if (e.Range != null && e.Range.IsCells && CoveredRanges.Count != 0)
            {
                this.CoveredRanges.InvalidateRanges();
                e.Range = CoveredRanges.Ranges.GetOuterRange(e.Range);
            }

            try
            {
                OnSelectionChanging(e);
            }
            catch
            {
                e.Cancel = true;
                //if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                throw;
            }
        }

        /// <summary>
        /// Raises the <see cref="GridModel.SelectionChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridSelectionChangingEventArgs"/> that contains the event data.</param>
        protected virtual void OnSelectionChanging(GridSelectionChangingEventArgs e)
        {
            if (!CanGridRaiseEvents) return;
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnSelectionChanging(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnSelectionChanging(e);
            }

            if (SelectionChanging != null)
                SelectionChanging(this, e);
        }

        /// <summary>
        /// Occurs before the model updates internal data structures when the model in the process of selecting
        /// a range of cells as a result of a <see cref="GridModelSelections.SelectRange"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridModel"/> will raise a  <see cref="GridModel.SelectionChanging"/> event before
        /// it updates its internal data structures and a  <see cref="GridModel.SelectionChanged"/> event
        /// afterwards. A <see cref="GridControlBase"/> grid listens to this event and outlines
        /// the selected range of cells.
        /// <para/>
        /// You can disallow the selection of specific cells at run-time when
        /// you assign true to <see cref="CancelEventArgs.Cancel"/>.<para/>
        /// You can also modify the <see cref="GridSelectionChangingEventArgs.Range"/> to include additional cells.
        /// </remarks>
        /// <seealso cref="GridSelectionChangingEventHandler"/>
        /// <seealso cref="GridSelectionChangedEventArgs"/>
#if !WinRT
        [
        Description("Occurs before internal data structures are updated with new selection state from a SelectRange command."),
        Category("Behavior")
        ]
#endif
        public event GridSelectionChangingEventHandler SelectionChanging;

        #endregion
        #endregion

        #region UserData
        private IDictionary userData;
        [XmlIgnore]
        public IDictionary UserData
        {
            get
            {
                if (this.userData == null)
                {
                    this.userData = new Dictionary<string, object>();
                }

                return this.userData;
            }
        }
        #endregion


        #region ExcelLikeDragDrop
#if !WinRT
        [Description("Occurs when the user releases the mouse over a cell at the end of an OLE drag-and-drop operation and before the data are applied to the grid."),
        Category("Behavior")]
#endif
        public event GridOleDropAtRowColEventHandler OleDropAtRowCol;

        /// <summary>
        /// Raises the  <see cref="OleDropAtRowCol"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridOleDropAtRowColEventArgs" /> that contains the event data.</param>
        protected virtual void OnOleDropAtRowCol(GridOleDropAtRowColEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.GridControlBaseEvents.TraceVerbose, this, e);
            if (OleDropAtRowCol != null)
            {
                OleDropAtRowCol(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public void RaiseOleDropAtRowCol(GridOleDropAtRowColEventArgs e)
        {
            OnOleDropAtRowCol(e);
        }

        /// <summary>
        /// Occurs after the user releases the mouse over a cell at the end of an OLE drag-and-drop operation and
        /// the data were applied to the grid.
        /// </summary>
#if !WinRT
        [Description("Occurs after the user releases the mouse over a cell at the end of an OLE drag-and-drop operation and the data were applied to the grid."),
        Category("Behavior")]
#endif
        public event EventHandler OleDroppedData;

        /// <summary>
        /// Raises the <see cref="OleDroppedData"/> event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnOleDroppedData(EventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.GridControlBaseEvents.TraceVerbose, Name, e);
            if (OleDroppedData != null)
            {
                OleDroppedData(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public void RaiseOleDroppedData(EventArgs e)
        {
            OnOleDroppedData(e);
        }

        public event GridQueryOleDataSourceDataEventHandler QueryOleDataSourceData;

        protected virtual void OnQueryOleDataSourceData(GridQueryOleDataSourceDataEventArgs e)
        {

            if (!CanGridRaiseEvents)
            {
                return;
            }

            if (QueryOleDataSourceData != null)
            {
                QueryOleDataSourceData(this, e);
            }
        }

        public void RaiseQueryOleDataSourceData(GridQueryOleDataSourceDataEventArgs e)
        {
            OnQueryOleDataSourceData(e);
        }

        /// <summary>
        /// Occurs when the user drops data onto another control using OLE drag-and-drop
        /// and does not press the Control Key. Set e.Cancel = True for this event if you do not
        /// want the grid to clear cell contents of the dragged cells.
        /// </summary>
#if !WinRT
        [Description("Occurs when a user drops data onto another control using OLE drag-and-drop and does not press the Control Key."),
        Category("Behavior")]
#endif
        public event EventHandler<CancelEventArgs> QueryDragDropMoveClearCells;

        /// <summary>
        /// Raises the <see cref="QueryDragDropMoveClearCells"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryDragDropMoveClearCells(CancelEventArgs e)
        {
            if (QueryDragDropMoveClearCells != null)
            {
                QueryDragDropMoveClearCells(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public void RaiseQueryDragDropMoveClearCells(CancelEventArgs e)
        {
            OnQueryDragDropMoveClearCells(e);
        }

        public class InternalGridDragDropData
        {
            public bool dndSource = false;

            public int dndStartRow = 0;

            public int dndStartCol = 0;

            public GridRangeInfoList dndSelList = null;

            public int dndForceDropCol = 0;

            public int dndForceDropRow = 0;

            public int dndRowOffset = 0;

            public int dndColOffset = 0;

            public bool dndCurrentCellText = false;       //// True if selected text from current cell is dragged

            public IGridCellRenderer dndCurrentCellControl = null;

            public static bool dndGridSource = false;   //// True if grid is a data source.

            public static int dndRowsCopied = 0;   ///// Number of rows / cols copied in OnDndCacheGlobalData.

            public static int dndColsCopied = 0;

            public static bool dndGridTargetStyle = false;   //// True if grid was a target and style info was copied.

            public bool directDragDrop = false;
        }

        InternalGridDragDropData dragDropModel;

        public InternalGridDragDropData DragDropData
        {
            get
            {
                if (dragDropModel == null)
                {
                    dragDropModel = new InternalGridDragDropData();
                }

                return dragDropModel;
            }
        }


        #region HyperlinkCellEventHandler

        public event CellRequestNavigateEventHandler CellRequestNavigate;

        protected virtual void OnCellRequestNavigate(CellRequestNavigateEventArgs e)
        {
            if (CellRequestNavigate != null)
                CellRequestNavigate(this, e);
        }

        internal void RaiseCellRequestNavigate(CellRequestNavigateEventArgs e)
        {
            OnCellRequestNavigate(e);
        }

        #endregion

        #endregion
        /// <summary>
        /// Gets or sets the active grid view.
        /// </summary>
        /// <remarks>
        /// If there are several views associated with this model, only one <see cref="GridControlBase"/> can be active.
        /// <para/>
        /// Changing the active view will result in calls to <see cref="GridCurrentCell.Deactivate"/>
        /// and <see cref="GridCurrentCell.Activate"/> for the involved controls.
        /// <para/>
        /// </remarks>
        [XmlIgnore]
        public GridControlBase ActiveGridView
        {
            get
            {
                return activeGridView;
            }

            set
            {
                if (activeGridView != value)
                    activeGridView = value;
            }
        }

        [XmlIgnore]
        internal GridControlBase activeGridView = null;

        [XmlIgnore]
        internal bool selectionStateChanged = false;
#if !WinRT
        #region UndoRedo
        [XmlIgnore]
        internal bool inInit = false;

        [XmlIgnore]
        internal string updateCommand = null;

        [XmlIgnore]
        GridModelCommandManager commandStack = null;

        [XmlIgnore]
        int suspendRecordUndo = 0;

        /// <summary>
        /// Suspend logging undo information.
        /// </summary>
        public void SuspendRecordUndo()
        {
            suspendRecordUndo++;
        }

        /// <summary>
        /// Resume logging undo information.
        /// </summary>
        public void ResumeRecordUndo()
        {
            if (suspendRecordUndo > 0)
            {
                suspendRecordUndo--;
            }
        }

        /// <summary>
        /// Gets a value indicating whether undo information should be logged.
        /// </summary>
        [Browsable(false)]
        public virtual bool ShouldRecordUndo
        {
            get
            {
                return !inInit && suspendRecordUndo == 0;
            }
        }

        /// <summary>
        /// Gets undo and redo in the grid.
        /// </summary>
        [Browsable(false)]
        public GridModelCommandManager CommandStack
        {
            get
            {
                if (commandStack == null)
                {
                    commandStack = new GridModelCommandManager(this);
                }

                return commandStack;
            }
        }

        protected virtual void SetGridModelCommandManager(GridModel gridModel)
        {
            commandStack = new GridModelCommandManager(gridModel);
        }        
        #endregion
#endif

        /// <summary>
        /// Records current selection state - current cell and selected ranges. Will be used for restoring selections when performing undo / redo,
        /// </summary>
        /// <param name="currentRow">The row index of current cell.</param>
        /// <param name="currentCol">The column index of current cell.</param>
        /// <param name="ranges">The current list of selected ranges.</param>
        public void ChangeSelectionState(int currentRow, int currentCol, GridRangeInfo[] ranges)
        {
            // called from CommandStack when doing an undo or redo
            this.Selections.Clear(true);
            if (ActiveGridView != null)
            {
                ActiveGridView.CurrentCell.MoveTo(currentRow, currentCol);
                ActiveGridView.CurrentCell.ScrollInView();
            }

            foreach (GridRangeInfo range in ranges)
            {
                if (range != null && !range.IsEmpty)
                {
                    this.Selections.Add(range);
                }
            }
        }
        /// <overload>
        /// Applies a text to the specified range of cells.
        /// </overload>
        /// <summary>
        /// Applies an array of styles to the specified range of cells.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <param name="cellsInfo">The array of <see cref="GridStyleInfo"/> objects that holds cell information.</param>
        /// <param name="modifyType">A <see cref="StyleModifyType"/> that specifies the style operation to be performed.</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if the operation was successful.</returns>
        /// <remarks>
        /// <see cref="GridModel.ChangeCells"/> will reset volatile data cache,
        /// generate undo information, force recalculation of floating cells, and
        /// raise <see cref="GridModel.CellsChanging"/> and <see cref="GridModel.CellsChanged"/> method.
        /// <para/>
        /// When you change cells directly with an indexer, this results in a call to <see cref="GridModel.ChangeCells"/>
        /// with modifyType set to <see cref="StyleModifyType.Changes"/>.
        /// <para/>
        /// </remarks>
        /// <example>
        /// The following example assigns a previously create style with a bold font to a cell:
        /// <code lang="C#">
        ///             GridStyleInfo boldFontStyle = new GridStyleInfo();
        ///             boldFontStyle.TextColor =  Color.FromArgb(238, 122, 3);
        ///             boldFontStyle.Font = boldFont;
        ///             model[rowIndex, 1].Text = "Interior";
        ///             model.ChangeCells(GridRangeInfo.Cell(rowIndex, 1), boldFontStyle);
        ///             </code>
        /// </example>
        public virtual bool ChangeCells(GridRangeInfo range, GridStyleInfo[] cellsInfo, StyleModifyType modifyType)
        {
            bool success = false;
            GridStyleInfo[] savedCellsInfo = null;    // will be filled with style setting
            //// start op, generate undo info
            OperationFeedback op = new OperationFeedback(this);
            op.Name = "ChangeCells";
#if !WinRT
            op.Description = Syncfusion.Windows.Controls.Grid.Resources.SR.GetString("DescriptionChangeCells", range);
            op.AllowCancel = CommandStack.IsRecording || !CommandStack.Enabled;
            op.AllowRollback = CommandStack.IsRecording;

            try
            {
                //// bool bIsMouseAction = GetHitState() > 0;  // op has a AllowProgress setting
                GridRangeInfo intRange = range.ExpandRange(-1, -1, -1, -1);
                int dwSize = intRange.Width * intRange.Height;

                if (CommandStack.ShouldGenerateUndoInfo)
                {
                    try
                    {
                        savedCellsInfo = GetCellsInfo(intRange);
                    }
                    catch (Exception ex)
                    {
                        if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        {
                            throw;
                        }

                        return false;
                    }
                }

                GridRangeInfo restoreRange = GridRangeInfo.Empty;
                //ResetVolatileData();
                try
                {
                    int cellIndex = 0;
                    int counter = 0;
                    for (int rowIndex = intRange.Top; rowIndex <= intRange.Bottom; rowIndex++)
                    {
                        for (int colIndex = intRange.Left; colIndex <= intRange.Right; colIndex++)
                        {
                            if (cellsInfo.Length > 1)
                            {
                                cellIndex = ((rowIndex - intRange.Top) * intRange.Width) + (colIndex - intRange.Left);
                            }

                            if (cellIndex >= cellsInfo.Length)
                            {
                                break;
                            }

                            GridStyleInfo cellInfo = cellsInfo[cellIndex];
                            if (cellInfo != null)
                            {
                                success |= SetCellInfo(rowIndex, colIndex, cellInfo, modifyType);
                            }
                            else
                            {
                                success |= SetCellInfo(rowIndex, colIndex, null, StyleModifyType.Remove);
                            }
                            op.PercentComplete = (int)((++counter) * 100 / dwSize);
                            if (op.ShouldCancel)
                            {
                                if (rowIndex == range.Top)
                                {
                                    restoreRange = GridRangeInfo.Cells(rowIndex, range.Left, rowIndex, colIndex);
                                }
                                else
                                {
                                    restoreRange = GridRangeInfo.Cells(range.Top, range.Left, rowIndex, range.Right);
                                }

                                throw new ArgumentException();
                            }
                        }
                    }
                }
                catch (ArgumentException ucex)
                {
                    if (!ExceptionManager.RaiseExceptionCatched(this, ucex))
                    {
                        throw;
                    }

                    if (success && savedCellsInfo != null)
                    {
                        if (CommandStack.IsRecording)
                        {
                            CommandStack.Mode = GridCommandMode.Rollback;
                            ChangeCells(restoreRange, savedCellsInfo, StyleModifyType.Copy);
                            CommandStack.Mode = GridCommandMode.Recording;
                            savedCellsInfo = null;
                        }

                        success = false;
                    }
                }
                if (CommandStack.ShouldGenerateUndoInfo && savedCellsInfo != null)
                {
                    CommandStack.Push(new GridChangeCellsCommand(this, range, savedCellsInfo, StyleModifyType.Copy));
                }
            }
            finally
            {
                op.Close();
            }
#endif
            return success;
        }

        public GridStyleInfo[] GetCellsInfo(GridRangeInfo range)
        {
            try
            {
                int size = range.Width * range.Height;
                GridStyleInfo[] cellsInfo = new GridStyleInfo[size];
                for (int rowIndex = range.Top; rowIndex <= range.Bottom; rowIndex++)
                {
                    for (int colIndex = range.Left; colIndex <= range.Right; colIndex++)
                    {
                        GridStyleInfo styleInfo = new GridStyleInfo();
                        int dwIndex = ((rowIndex - range.Top) * range.Width) + (colIndex - range.Left);
                        if (GetCellInfo(rowIndex, colIndex, styleInfo))
                        {
                            cellsInfo[dwIndex] = styleInfo;
                        }
                    }
                }

                return cellsInfo;
            }
            catch (Exception ex)
            {
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                return null;
            }
        }

        public bool GetCellInfo(int rowIndex, int colIndex, GridStyleInfo style)
        {
            if (ActiveGridView != null)
            {
                var renderstyle = ActiveGridView.GetRenderStyleInfo(rowIndex, colIndex);
                if (renderstyle != null)
                {
                    style.Store.ModifyStyle(renderstyle.Store, StyleModifyType.ApplyNew);
                }
                return true;
            }
            return false;
        }

        public bool SetCellInfo(int rowIndex, int colIndex, GridStyleInfo style, StyleModifyType modifyType)
        {
            if (style != null)
            {
                GridStyleInfoIdentity identity = style.Identity as GridStyleInfoIdentity;

                if (identity == null || identity.Data == null || !identity.OffLine)
                {
                    // Avoid recursive calls when style is changed within OnSaveCellInfo.
                    // Otherwise, if identity is not offline, changing a property will
                    // result in OnStyleChanged firing again and a call to ChangeCells.
                    // GridStyleInfo.OnStyleChanged checks for offline flag and will not
                    // call SetCellInfo if the style is offline.
                    identity = new GridStyleInfoIdentity(this.VolatileCellStyles, rowIndex, colIndex, true);
                    GridStyleInfoStore store = (GridStyleInfoStore)style.Store;
                    if (style.Identity == null)
                    {
                        style.Identity = identity;
                    }
                    else
                    {
                        style = new GridStyleInfo(identity, store);
                    }
                }
            }
            try
            {
                int r = rowIndex;
                int c = colIndex;

                GridStyleInfoStore store = null;
                if (modifyType == StyleModifyType.Remove || (style == null && modifyType == StyleModifyType.Copy))
                {
                    store = Data[r, c];
                    if (store == null || 0 == store.GetShortValue(GridStyleInfoStore.ReadOnlyProperty))
                    {
                        Data[r, c] = null;
                    }
                }
                else
                {
                    if (style == null)
                    {
                        throw new ArgumentNullException("style");
                    }

                    if (modifyType != StyleModifyType.Copy)
                    {
                        store = Data[r, c];
                    }

                    if (store != null)
                    {
                        if (0 == store.GetShortValue(GridStyleInfoStore.ReadOnlyProperty))
                        {
                            store.ModifyStyle(style.Store, modifyType);
                        }
                    }
                    else
                    {
                        GridStyleInfoStore store2 = (GridStyleInfoStore)style.Store;
                        if (r <= this.RowCount && c <= this.ColumnCount)
                        {
                            if (store2 == null)
                            {
                                Data[r, c] = store2;
                            }
                            else
                            {
                                Data[r, c] = (GridStyleInfoStore)store2.Clone();
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
            // return false; Unreachable code
        }

        /// <summary>
        /// Applies a text to the specified range of cells.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <param name="textValue">The text to be saved in cells.</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if the operation was successful.</returns>
        /// <genoverload/>
        public bool ChangeCells(GridRangeInfo range, string textValue)
        {
            GridStyleInfo cellInfo = new GridStyleInfo();
            cellInfo.Text = textValue;
            return ChangeCells(range, new GridStyleInfo[] { cellInfo }, StyleModifyType.Changes);
        }

        /// <summary>
        /// Applies a style to the specified range of cells.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <param name="cellInfo">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if the operation was successful.</returns>
        /// <genoverload/>
        public bool ChangeCells(GridRangeInfo range, GridStyleInfo cellInfo)
        {
            return ChangeCells(range, new GridStyleInfo[] { cellInfo }, StyleModifyType.Changes);
        }

        /// <summary>
        /// Applies a style to the specified range of cells.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <param name="cellInfo">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="modifyType">A <see cref="StyleModifyType"/> that specifies the style operation to be performed.</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if the operation was successful.</returns>
        /// <genoverload/>
        public bool ChangeCells(GridRangeInfo range, GridStyleInfo cellInfo, StyleModifyType modifyType)
        {
            return ChangeCells(range, new GridStyleInfo[] { cellInfo }, modifyType);
        }

        /// <summary>
        /// Applies an array of styles to the specified range of cells.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <param name="cellsInfo">The array of <see cref="GridStyleInfo"/> objects that holds cell information.</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if the operation was successful.</returns>
        /// <genoverload/>
        public bool ChangeCells(GridRangeInfo range, GridStyleInfo[] cellsInfo)
        {
            return ChangeCells(range, cellsInfo, StyleModifyType.Changes);
        }

        /// <summary>
        /// Clears cell styles for a given list
        /// </summary>
        /// <param name="gridRangeInfoList"></param>
        /// <param name="clearStyles"></param>
        /// <returns></returns>
        internal bool ClearCells(GridRangeInfoList gridRangeInfoList, bool clearStyles)
        {
            foreach (GridRangeInfo range in gridRangeInfoList)
            {
                for (int i = range.Left; i <= range.Right; i++)
                {
                    for (int j = range.Top; j <= range.Bottom; j++)
                    {
                        if (clearStyles)
                        {
                            var gridStyle = this[j, i];
                            if (gridStyle != null)
                            {
                                gridStyle.Dispose();
                                gridStyle = null;
                                this.volatileCellStyles.Clear(new RowColumnIndex(j, i));
                            }
                            this[j, i] = new GridStyleInfo();
                        }
                        else
                        {
                            GridStyleInfo style = this[j, i];
                            style.ApplyFormattedText(string.Empty);
                        }
                    }
                }
            }

            return true;
        }


#if SyncfusionFramework4_0 || WinRT
        #region CutPaste Coding
        /// <summary>
        /// User set the object which implements the IGridCutPaste
        /// </summary>
        private IGridCopyPaste gridCutPaste;

        /// <summary>
        /// For Copy or paste the formatted text
        /// </summary>
        private GridModelTextDataExchange textDataExchange;

        /// <summary>
        /// User call the CanCopy(), Copy() using this object
        /// </summary>
        private GridModelCutPaste cutPaste;

        /// <summary>
        /// Occurs when the <see cref="GridModelCutPaste.CanPaste"/> is called on the <see cref="CutPaste"/> component of a <see cref="GridModel"/>.
        /// </summary>
        public event GridCutPasteEventHandler ClipboardCanPaste;

        /// <summary>
        /// Occurs when the <see cref="GridModelCutPaste.Copy"/> is called on the <see cref="CutPaste"/> component of a <see cref="GridModel"/>.
        /// </summary>
        public event GridCutPasteEventHandler ClipboardCanCopy;

        /// <summary>
        /// Occurs when the <see cref="GridModelCutPaste.Cut"/> is called on the <see cref="CutPaste"/> component of a <see cref="GridModel"/>.
        /// </summary>
        public event GridCutPasteEventHandler ClipboardCanCut;

        /// <summary>
        /// Occurs when the <see cref="GridModelCutPaste.Paste"/> is called on the <see cref="CutPaste"/> component of a <see cref="GridModel"/>.
        /// </summary>
        public event GridCutPasteEventHandler ClipboardPaste;

        /// <summary>
        /// Occurs after the <see cref="GridModelCutPaste.Paste"/> is called on the <see cref="CutPaste"/> component of a <see cref="GridModel"/>.
        /// </summary>
        public event GridCutPasteEventHandler ClipboardPasted;

        /// <summary>
        /// Occurs when the <see cref="GridModelCutPaste.Copy"/> is called on the <see cref="CutPaste"/> component of a <see cref="GridModel"/>.
        /// </summary>
        public event GridCutPasteEventHandler ClipboardCopy;

        /// <summary>
        /// Occurs when the <see cref="GridModelCutPaste.CanCut"/> is called on the <see cref="CutPaste"/> component of a <see cref="GridModel"/>.
        /// </summary>
        public event GridCutPasteEventHandler ClipboardCut;

        /// <summary>
        /// Gets or sets the GridCutPaste
        /// </summary>
        [XmlIgnore]
        public IGridCopyPaste GridCopyPaste
        {
            get
            {
                if (cutPaste == null)
                {
                    cutPaste = new GridModelCutPaste(this);

                }

                return this.gridCutPaste;
            }

            set
            {
                this.gridCutPaste = value;
            }
        }

        /// <summary>
        /// Gets text data exchange for the grid. Lets you copy cell text to a stream or clipboard and recreate the
        /// cell text at a later time.
        /// </summary>
        [XmlIgnore]
        public virtual GridModelTextDataExchange TextDataExchange
        {
            get
            {
                if (this.textDataExchange == null)
                {
                    this.textDataExchange = new GridModelTextDataExchange(this);
                }

                return this.textDataExchange;
            }
            set
            {
                this.textDataExchange = value;
            }
        }

        /// <summary>
        /// Gets clipboard operations for the grid.
        /// </summary>
        public GridModelCutPaste CutPaste
        {
            get
            {
                if (this.cutPaste == null)
                {
                    this.cutPaste = new GridModelCutPaste(this);
                }

                return this.cutPaste;
            }
        }

        /// <summary>
        /// Used Internally.
        /// </summary>
        /// <param name="e">GridCutPasteEventArgs</param>
        internal void RaiseClipboardCanPaste(GridCutPasteEventArgs e)
        {
            this.OnClipboardCanPaste(e);
        }

        internal void RaiseClipboardCanCopy(GridCutPasteEventArgs e)
        {
            this.OnClipboardCanCopy(e);
        }

        /// <summary>
        /// used internally.
        /// </summary>
        internal void RaiseClipboardCanCut(GridCutPasteEventArgs e)
        {
            this.OnClipboardCanCut(e);
        }

        /// <summary>
        /// Used Internally
        /// </summary>
        /// <param name="e"></param>
        internal void RaiseClipboardPaste(GridCutPasteEventArgs e)
        {
            this.OnClipboardPaste(e);
        }

        /// <summary>
        /// Used Internally
        /// </summary>
        /// <param name="e"></param>
        internal void RaiseClipboardPasted(GridCutPasteEventArgs e)
        {
            this.OnClipboardPasted(e);
        }

        internal void RaiseClipboardCopy(GridCutPasteEventArgs e)
        {
            this.OnClipboardCopy(e);
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        internal void RaiseClipboardCut(GridCutPasteEventArgs e)
        {
            this.OnClipboardCut(e);
        }


        /// <summary>
        /// Raises the <see cref="ClipboardCanPaste"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs" /> that contains the event data.</param>
        protected virtual void OnClipboardCanPaste(GridCutPasteEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnClipboardCanPaste(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnClipboardCanPaste(e);
            }

            if (this.ClipboardCanPaste != null)
            {
                this.ClipboardCanPaste(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ClipboardCanCopy"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs" /> that contains the event data.</param>
        protected virtual void OnClipboardCanCopy(GridCutPasteEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnClipboardCanCopy(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnClipboardCanCopy(e);
            }

            if (this.ClipboardCanCopy != null)
            {
                this.ClipboardCanCopy(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ClipboardCanCut"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs" /> that contains the event data.</param>
        protected virtual void OnClipboardCanCut(GridCutPasteEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnClipboardCanCut(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnClipboardCanCut(e);
            }

            if (this.ClipboardCanCut != null)
            {
                this.ClipboardCanCut(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ClipboardPaste"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs" /> that contains the event data.</param>
        protected virtual void OnClipboardPaste(GridCutPasteEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnClipboardPaste(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnClipboardPaste(e);
            }

            if (this.ClipboardPaste != null)
            {
                this.ClipboardPaste(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ClipboardPasted"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs" /> that contains the event data.</param>
        protected virtual void OnClipboardPasted(GridCutPasteEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnClipboardPasted(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnClipboardPasted(e);
            }

            if (this.ClipboardPasted != null)
            {
                this.ClipboardPasted(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ClipboardCopy"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs" /> that contains the event data.</param>
        protected virtual void OnClipboardCopy(GridCutPasteEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnClipboardCopy(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnClipboardCopy(e);
            }

            if (this.ClipboardCopy != null)
            {
                this.ClipboardCopy(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ClipboardCut"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs" /> that contains the event data.</param>
        protected virtual void OnClipboardCut(GridCutPasteEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnClipboardCut(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnClipboardCut(e);
            }

            if (this.ClipboardCut != null)
            {
                this.ClipboardCut(this, e);
            }
        }
        #endregion
#endif
        [XmlIgnore]
        public IGridModelEventsHost EventsHost
        {
            get;
            set;
        }

        /// <summary>
        /// This is called from GridDropDownGridListControlCellModel to initialize datasource on demand.
        /// Override this method to calculate the datasource on demand
        /// only when it is needed and not every time in QueryStyleInfo. Default behavior is to return
        /// style.ChoiceList if not empty. If style.ChoiceList is empty, style.DataSource is returned.
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public virtual IEnumerable GetStyleDataSource(GridStyleInfo style)
        {
            IEnumerable dataSource = null;
            Type valueType = style.CellValueType;

            if (style.ChoiceList != null && style.ChoiceList.Count > 0)
                dataSource = style.ChoiceList;
            else
                dataSource = style.ItemsSource;

            //if (dataSource == null)
            //{
            //    PropertyDescriptor pd = GetPropertyDescriptor(style);
            //    TypeConverter tc = GetTypeConverter(style);
            //    if (tc != null && tc.CanConvertTo(typeof(string)) && tc.GetStandardValuesSupported())
            //    {
            //        return this.GetCachedStandardValues(tc, pd != null ? pd.PropertyType : style.CellValueType);
            //    }
            //}

            return dataSource;

        }
#if !WinRT
        /// <summary>
        /// Returns GridStyleInfo.PropertyDescriptor.
        /// </summary>
        /// <param name="style">The style object</param>
        /// <returns>A PropertyDescriptor</returns>
        internal PropertyDescriptor GetPropertyDescriptor(GridStyleInfo style)
        {
            return style.PropertyDescriptor;
        }
#endif
        /// <summary>
        /// Returns a TypeConverter with type information about the style.CellValue.
        /// </summary>
        /// <param name="style">The style object</param>
        /// <returns>A TypeConverter</returns>
        //internal TypeConverter GetTypeConverter(GridStyleInfo style)
        //{
        //    PropertyDescriptor pd = GetPropertyDescriptor(style);
        //    if (pd != null)
        //        return pd.Converter;

        //    Type type = style.CellValueType;
        //    if (type != null)
        //        return TypeDescriptor.GetConverter(type);

        //    return null;
        //}



        #region To Rename

        public GridCoveredCellInfoCollection CoveredRanges
        {
            get { return CoveredCells; }
        }

        bool CanGridRaiseEvents
        {
            get { return true; }
        }


        #endregion


        /// <summary>
        /// Returns true if all the GridRangeInfo in this list is of Type Col, otherwise returns false.
        /// </summary>
        public bool IsCols(GridRangeInfoList rangeList)
        {
            foreach (GridRangeInfo range in rangeList)
            {
                if (range.IsCols)
                    continue;
                else
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if all the GridRangeInfo in this list is of Type Row, otherwise returns false.
        /// </summary>
        public bool IsRows(GridRangeInfoList rangeList)
        {
            foreach (GridRangeInfo range in rangeList)
            {
                if (range.IsRows)
                    continue;
                else
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Resizes a range of columns to optimally fit contents of the
        /// specified range of cells and given options.
        /// </summary>
        /// <param name="range">The range of cells to be analyzed.</param>
        /// <param name="options">Specifies whether row or column headers should be included; if size can be reduced and if covered cells should be considered.</param>
        /// <returns>True if any changes were made; False if all sizes were already optimal.</returns>
        internal virtual bool ResizeColumnsToFit(GridRangeInfo range, GridResizeToFitOptions options, GridControlLengthUnitType gridControlLengthUnitType)
        {
            if (range.IsEmpty)
                return false;
            //this.Options.ColumnSizer;
            bool includeCellsWithinCoveredRange = (options & GridResizeToFitOptions.IncludeCellsWithinCoveredRange) != 0;
            bool resizeCoveredCells = (options & GridResizeToFitOptions.ResizeCoveredCells) != 0 || includeCellsWithinCoveredRange;
            bool noShrinkSize = (options & GridResizeToFitOptions.NoShrinkSize) != 0;
            bool includeHeaders = (options & GridResizeToFitOptions.IncludeHeaders) != 0;
            // bool includeHiddenRows = (options & GridResizeToFitOptions.IncludeHiddenCells) != 0;
            range = range.ExpandRange(0, 0, this.RowCount, this.ColumnCount);
            //Model.FloatingCells.EvaluateFloatingCells(range);

            //switch()
            using (Disposable op = new Disposable()) //OperationFeedback op = new OperationFeedback(Model))
            {
                //op.Description = SR.GetString("GRID_IDM_RESIZECOLS");

                double maxWidth = double.MaxValue;

                try
                {

                    //bool bAbort = false;
                    CoveredCellInfo coveredRange;
                    double[] newWidths = LineSizeUtil.GetRange(ColumnWidths, range.Left, range.Right);
                    double[] oldWidths = (double[])newWidths.Clone();
                    if (!noShrinkSize)
                    {
                        for (int n = 0; n < newWidths.Length; n++)
                            newWidths[n] = -1;
                    }

                    bool doHeader = includeHeaders && range.Top > 0;

                    for (int rowIndex = range.Top; rowIndex <= range.Bottom; rowIndex++)
                    {
                        if (doHeader)
                            rowIndex = 0;

                        double width = 0;
                        for (int colIndex = range.Left; colIndex <= range.Right; colIndex++)
                        {
                            coveredRange = CoveredCells.GetCoveredCell(rowIndex, colIndex);
                            //  CoveredCells.GetCellSpan
                            bool isCovered = coveredRange != null;

                            // Skip invisible rows.
                            bool canResize = false;

                            // Covered cells.
                            if (isCovered && !includeCellsWithinCoveredRange)
                            {
                                if (resizeCoveredCells || coveredRange.Width == 1)
                                {
                                    canResize =
                                        rowIndex == coveredRange.Top // Must be the first covered row.
                                        && colIndex == coveredRange.Right // And the last covered col.
                                        // All cells of covered cell must be with in range to be resized.
                                        && range.Left <= coveredRange.Left && range.Bottom >= coveredRange.Bottom;
                                }
                            }
                            else
                                // Skip invisible rows.
                                canResize = RowHeights[rowIndex] > 0;

                            if (canResize)
                            {
                                GridStyleInfo styleInfo = null;

                                Size size;

                                if (isCovered)
                                {
                                    styleInfo = this[coveredRange.Top, coveredRange.Left];
                                    GridCellModelBase cellModel = styleInfo.CellModel;
                                    //cellModel.LoadStyle(coveredRange.Left, coveredRange.Top, styleInfo);

                                    size = cellModel.CalculatePreferredCellSize(coveredRange.Top, coveredRange.Left, styleInfo, GridQueryBounds.Width);

                                    // Subtract col heights of previous cols (only the last col can be resized).
                                    if (colIndex > coveredRange.Left)
                                        size.Width -= (int)LineSizeUtil.GetTotal(this.ColumnWidths, coveredRange.Left, colIndex - 1);
                                }
                                else
                                {
                                    styleInfo = this[rowIndex, colIndex];
                                    GridCellModelBase cellModel = styleInfo.CellModel;
                                    //cellModel.LoadStyle(colIndex, rowIndex, styleInfo);

                                    size = cellModel.CalculatePreferredCellSize(rowIndex, colIndex, styleInfo, GridQueryBounds.Width);
                                }

                                width = size.Width + 2;
                            }
                            else
                                width = this.ColumnWidths[colIndex];

                            //if (op.ShouldCancel)
                            //    throw new GridUserCanceledException();

                            if (isCovered && canResize)
                            {
                                if (coveredRange.Right <= range.Right)
                                    colIndex = coveredRange.Right;
                                else
                                    continue;
                            }

                            if (canResize && width > newWidths[colIndex - range.Left])
                            {
                                newWidths[colIndex - range.Left] = width;
                            }
                        }

                        //op.PercentComplete = (rowIndex - range.Top) * 100 / range.Height;

                        width = Math.Min(maxWidth, width);

                        if (doHeader)
                        {
                            doHeader = false;
                            rowIndex = range.Top - 1;
                        }
                    }

                    bool equal = true;
                    for (int n = 0; n < newWidths.Length; n++)
                        equal &= newWidths[n] == oldWidths[n];

                    if (!equal)
                        LineSizeUtil.SetRange(ColumnWidths, range.Left, range.Right, newWidths);
                }
                catch
                {
                }

                this.InvalidateVisual();

                return true;
            }
        }

        public bool ResizeColumnsToFit(GridRangeInfo range, GridResizeToFitOptions options)
        {
            if (range.IsEmpty)
                return false;

            bool includeCellsWithinCoveredRange = (options & GridResizeToFitOptions.IncludeCellsWithinCoveredRange) != 0;
            bool resizeCoveredCells = (options & GridResizeToFitOptions.ResizeCoveredCells) != 0 || includeCellsWithinCoveredRange;
            bool noShrinkSize = (options & GridResizeToFitOptions.NoShrinkSize) != 0;
            bool includeHeaders = (options & GridResizeToFitOptions.IncludeHeaders) != 0;
            range = range.ExpandRange(0, 0, this.RowCount, this.ColumnCount);
            //Model.FloatingCells.EvaluateFloatingCells(range);

            using (Disposable op = new Disposable()) //OperationFeedback op = new OperationFeedback(Model))
            {
                //op.Description = SR.GetString("GRID_IDM_RESIZECOLS");

                double maxWidth = double.MaxValue;

                try
                {
                    //bool bAbort = false;
                    CoveredCellInfo coveredRange;
                    double[] newWidths = LineSizeUtil.GetRange(ColumnWidths, range.Left, range.Right);
                    double[] oldWidths = (double[])newWidths.Clone();
                    if (!noShrinkSize)
                    {
                        for (int n = 0; n < newWidths.Length; n++)
                            newWidths[n] = -1;
                    }

                    bool doHeader = includeHeaders && range.Top > 0;

                    for (int rowIndex = range.Top; rowIndex <= range.Bottom; rowIndex++)
                    {
                        if (doHeader)
                            rowIndex = 0;

                        double width = 0;
                        for (int colIndex = range.Left; colIndex <= range.Right; colIndex++)
                        {
                            coveredRange = CoveredCells.GetCoveredCell(rowIndex, colIndex);
                            bool isCovered = coveredRange != null;

                            // Skip invisible rows.
                            bool canResize = false;

                            // Covered cells.
                            if (isCovered && !includeCellsWithinCoveredRange)
                            {
                                if (resizeCoveredCells || coveredRange.Width == 1)
                                {
                                    canResize =
                                        rowIndex == coveredRange.Top // Must be the first covered row.
                                        && colIndex == coveredRange.Right // And the last covered col.
                                        // All cells of covered cell must be with in range to be resized.
                                        && range.Left <= coveredRange.Left && range.Bottom >= coveredRange.Bottom;
                                }
                            }
                            else
                                // Skip invisible rows.
                                canResize = RowHeights[rowIndex] > 0;

                            if (canResize)
                            {
                                GridStyleInfo styleInfo = null;

                                Size size;

                                if (isCovered)
                                {
                                    styleInfo = this[coveredRange.Top, coveredRange.Left];
                                    GridCellModelBase cellModel = styleInfo.CellModel;
                                    //cellModel.LoadStyle(coveredRange.Left, coveredRange.Top, styleInfo);

                                    size = cellModel.CalculatePreferredCellSize(coveredRange.Top, coveredRange.Left, styleInfo, GridQueryBounds.Width);

                                    // Subtract col heights of previous cols (only the last col can be resized).
                                    if (colIndex > coveredRange.Left)
                                        size.Width -= (int)LineSizeUtil.GetTotal(this.ColumnWidths, coveredRange.Left, colIndex - 1);
                                }
                                else
                                {
                                    styleInfo = this[rowIndex, colIndex];
                                    GridCellModelBase cellModel = styleInfo.CellModel;
                                    //cellModel.LoadStyle(colIndex, rowIndex, styleInfo);

                                    size = cellModel.CalculatePreferredCellSize(rowIndex, colIndex, styleInfo, GridQueryBounds.Width);
                                }

                                width = size.Width + 2;
                            }
                            else
                                width = this.ColumnWidths[colIndex];

                            //if (op.ShouldCancel)
                            //    throw new GridUserCanceledException();

                            if (isCovered && canResize)
                            {
                                if (coveredRange.Right <= range.Right)
                                    colIndex = coveredRange.Right;
                                else
                                    continue;
                            }

                            if (width > newWidths[colIndex - range.Left])
                                newWidths[colIndex - range.Left] = width;
                        }

                        //op.PercentComplete = (rowIndex - range.Top) * 100 / range.Height;

                        width = Math.Min(maxWidth, width);

                        if (doHeader)
                        {
                            doHeader = false;
                            rowIndex = range.Top - 1;
                        }
                    }

                    bool equal = true;
                    for (int n = 0; n < newWidths.Length; n++)
                        equal &= newWidths[n] == oldWidths[n];

                    if (!equal)
                        LineSizeUtil.SetRange(ColumnWidths, range.Left, range.Right, newWidths);
                }
                catch
                {
                }

                this.InvalidateVisual();

                return true;
            }
        }

        public bool ResizeRowsToFit(GridRangeInfo range, GridResizeToFitOptions options)
        {
            if (range.IsEmpty)
                return false;

            bool includeCellsWithinCoveredRange = (options & GridResizeToFitOptions.IncludeCellsWithinCoveredRange) != 0;
            bool resizeCoveredCells = (options & GridResizeToFitOptions.ResizeCoveredCells) != 0 || includeCellsWithinCoveredRange;
            bool noShrinkSize = (options & GridResizeToFitOptions.NoShrinkSize) != 0;
            bool includeHeaders = (options & GridResizeToFitOptions.IncludeHeaders) != 0;
            range = range.ExpandRange(0, 0, this.RowCount, this.ColumnCount);
            //Model.FloatingCells.EvaluateFloatingCells(range);

            using (Disposable op = new Disposable()) //OperationFeedback op = new OperationFeedback(Model))
            {
                //op.Description = SR.GetString("GRID_IDM_RESIZECOLS");

                double maxHeight = double.MaxValue;

                try
                {
                    //bool bAbort = false;
                    CoveredCellInfo coveredRange;
                    double[] newHeights = LineSizeUtil.GetRange(RowHeights, range.Top, range.Bottom);
                    double[] oldHeights = (double[])newHeights.Clone();
                    if (!noShrinkSize)
                    {
                        for (int n = 0; n < newHeights.Length; n++)
                        {
                            newHeights[n] = -1;
                        }
                    }

                    bool doHeader = includeHeaders && range.Left > 0;

                    for (int colIndex = range.Left; colIndex <= range.Right; colIndex++)
                    {
                        if (doHeader)
                        {
                            colIndex = 0;
                        }

                        double height = 0;
                        for (int rowIndex = range.Top; rowIndex <= range.Bottom; rowIndex++)
                        {
                            coveredRange = CoveredCells.GetCoveredCell(rowIndex, colIndex);
                            bool isCovered = coveredRange != null;

                            // Skip invisible columns.
                            bool canResize = false;

                            // Covered cells.
                            if (isCovered && !includeCellsWithinCoveredRange)
                            {
                                if (resizeCoveredCells || coveredRange.Height == 1)
                                {
                                    canResize =
                                        rowIndex == coveredRange.Top // Must be the first covered row.
                                        && colIndex == coveredRange.Right // And the last covered col.
                                        // All cells of covered cell must be with in range to be resized.
                                        && range.Left <= coveredRange.Left && range.Bottom >= coveredRange.Bottom;
                                }
                            }
                            else
                                // Skip invisible columns.
                                canResize = ColumnWidths[colIndex] > 0;

                            if (canResize)
                            {
                                GridStyleInfo styleInfo = null;

                                Size size;

                                if (isCovered)
                                {
                                    styleInfo = this[coveredRange.Top, coveredRange.Left];
                                    GridCellModelBase cellModel = styleInfo.CellModel;
                                    //cellModel.LoadStyle(coveredRange.Left, coveredRange.Top, styleInfo);

                                    size = cellModel.CalculatePreferredCellSize(coveredRange.Top, coveredRange.Left, styleInfo, GridQueryBounds.Height);

                                    // Subtract col heights of previous cols (only the last col can be resized).
                                    if (rowIndex > coveredRange.Top)
                                        size.Height -= (int)LineSizeUtil.GetTotal(this.RowHeights, coveredRange.Top, rowIndex - 1);
                                }
                                else
                                {
                                    styleInfo = this[rowIndex, colIndex];
                                    GridCellModelBase cellModel = styleInfo.CellModel;
                                    //cellModel.LoadStyle(colIndex, rowIndex, styleInfo);

                                    size = cellModel.CalculatePreferredCellSize(rowIndex, colIndex, styleInfo, GridQueryBounds.Height);
                                }

                                height = size.Height + 2;
                            }
                            else
                            {
                                height = this.RowHeights[rowIndex];
                            }

                            //if (op.ShouldCancel)
                            //    throw new GridUserCanceledException();

                            if (isCovered && canResize)
                            {
                                if (coveredRange.Bottom <= range.Bottom)
                                {
                                    rowIndex = coveredRange.Bottom;
                                }
                                else
                                {
                                    continue;
                                }
                            }

                            if (height > newHeights[rowIndex - range.Top])
                            {
                                newHeights[rowIndex - range.Top] = height;
                            }
                        }

                        //op.PercentComplete = (rowIndex - range.Top) * 100 / range.Height;

                        height = Math.Min(maxHeight, height);

                        if (doHeader)
                        {
                            doHeader = false;
                            colIndex = range.Left - 1;
                        }
                    }

                    bool equal = true;
                    for (int n = 0; n < newHeights.Length; n++)
                        equal &= newHeights[n] == oldHeights[n];

                    if (!equal)
                        LineSizeUtil.SetRange(RowHeights, range.Top, range.Bottom, newHeights);
                }
                catch
                {
                }

                this.InvalidateVisual();

                return true;
            }
        }
        #region Serialization
#if !WinRT
#if WPF
        /// <summary>
        /// Serializes the specified properties in the <see cref="Syncfusion.Windows.Controls.Grid.GridModel"/>.
        /// </summary>
        /// <param name="model">The file name.</param>
        public virtual void Serialize(string fileName)
#endif
#if SILVERLIGHT
        /// <summary>
        /// Serializes the specified properties in the <see cref="Syncfusion.Windows.Controls.Grid.GridModel"/>.
        /// Opens up a SaveFileDialog and saves the serialized data in XML.
        /// </summary>
        public virtual void Serialize()
#endif
#if WinRT
        /// <summary>
        /// Serializes the specified properties in the <see cref="Syncfusion.Windows.Controls.Grid.GridModel"/>.
        /// Opens up a SaveFileDialog and saves the serialized data in XML.
        /// </summary>
        public async void Serialize()
#endif
        {
            try
            {

#if WPF
                var xs = new XmlSerializer(typeof(GridModel));
                using (var sw = new XmlTextWriter(fileName, Encoding.Default))
                {
                    xs.Serialize(sw.BaseStream, this);
                }
#else
#if SILVERLIGHT
                SaveFileDialog sfd = new SaveFileDialog() { Filter = "XML Files (*.xml)|*.xml", FilterIndex = 1 };
                if (sfd.ShowDialog() == true)
                {
                    var stream = sfd.OpenFile();
                    if (stream != null)
                    {
                        var xs = new XmlSerializer(typeof(GridModel));
                        using (var sw = new StreamWriter(stream))
                        {
                            xs.Serialize(sw.BaseStream, this);
                        }
                    }
                }
#endif
#if WinRT
                //SaveXMLfile();
                FileSavePicker fsd = new FileSavePicker();
                fsd.SuggestedStartLocation = PickerLocationId.ComputerFolder;
                fsd.DefaultFileExtension = ".xml";
                fsd.SuggestedFileName = "serialize";
                fsd.FileTypeChoices.Add("XML File", new List<string>() { ".xml" });
                StorageFile file = await fsd.PickSaveFileAsync();
                Stream fileStream = await file.OpenStreamForWriteAsync();
                var serializer = new XmlSerializer(typeof(GridModel));
                using (var sw = new StreamWriter(fileStream))
                {
                    serializer.Serialize(sw.BaseStream, this);
                }
                fileStream.Dispose();
#endif
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Serializes to stream.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
#if WPF
        public virtual void SerializeToStream(TextWriter textWriter)
#else
        public virtual void SerializeToStream(Stream stream)
#endif
        {
            try
            {
                var xs = new XmlSerializer(typeof(GridModel));
#if WPF
                using (var sw = new XmlTextWriter(textWriter))
                {
                    xs.Serialize(sw.BaseStream, this);
                }
#else
                using (var sw = new StreamWriter(stream))
                {
                    xs.Serialize(sw.BaseStream, this);
                }
#endif
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Serializes the <see cref="Syncfusion.Windows.Controls.Grid.GridModel"/> properties as string.
        /// </summary>
        /// <returns>Serialized data as String</returns>
        public virtual string SerializeAsString()
        {
            var result = string.Empty;
            try
            {
                var xs = new XmlSerializer(typeof(GridModel));
                using (var sWriter = new StringWriter())
                {
#if WPF
                    using (var sw = new XmlTextWriter(sWriter))
                    {
                        xs.Serialize(sw, this);
                    }
#else
                    using (var sw = XmlWriter.Create(sWriter))
                    {
                        xs.Serialize(sw, this);
                    }
#endif
                    result = sWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
#endif
        /// <summary>
        /// Deserializes the specified <see cref="Syncfusion.Windows.Controls.Grid.GridModel"/>.
        /// </summary>
#if WPF
        public virtual void Deserialize(string fileName)
        {
        /// <summary>
        /// Deserializes the specified <see cref="Syncfusion.Windows.Controls.Grid.GridModel"/> 
        /// from an XML file read with an OpenFileDialog.
        /// </summary>
#endif
#if SILVERLIGHT
        public virtual void Deserialize()
        {
#endif
#if WinRT
        public async void Deserialize()
        {
#endif
            try
            {
                var xs = new XmlSerializer(typeof(GridModel));
#if WPF
                using (var sr = new XmlTextReader(fileName))
                {
                    var newModel = xs.Deserialize(sr) as GridModel;
                    Apply(newModel);
                }
#endif
#if SILVERLIGHT
                OpenFileDialog ofd = new OpenFileDialog() { Filter = "XML Files (*.xml)|*.xml", FilterIndex = 1 };

                if (ofd.ShowDialog() == true)
                {
                    var stream = ofd.File.OpenRead();
                    using (var sr = new StreamReader(stream, Encoding.UTF8))
                    {
                        var newModel = xs.Deserialize(sr) as GridModel;
                        Apply(newModel);
                    }
                }
#endif
#if WinRT
                FileOpenPicker file = new FileOpenPicker();
                file.FileTypeFilter.Add(".xml");
                file.SuggestedStartLocation = PickerLocationId.ComputerFolder;
                file.ViewMode = PickerViewMode.Thumbnail;

                StorageFile openfile =await file.PickSingleFileAsync();
                if(null !=openfile)
                {
                    Stream stream =await openfile.OpenStreamForReadAsync();
                    using (var sr = new StreamReader(stream))
                    {
                        var newModel = xs.Deserialize(sr) as GridModel;
                        Apply(newModel);
                    }
                }
#endif
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

#if !WinRT
        /// <summary>
        /// Deserializes from stream.
        /// </summary>
        /// <param name="textReader">The text reader.</param>
#if WPF
        public virtual void DeserializeFromStream(TextReader textReader)
#else
        public virtual void DeserializeFromStream(Stream stream)
#endif
        {
            try
            {
                var xs = new XmlSerializer(typeof(GridModel));
#if WPF
                using (var sr = new XmlTextReader(textReader))
                {
                    var newModel = xs.Deserialize(sr) as GridModel;
                    Apply(newModel);
                }
#else
                using (var sr = new StreamReader(stream, Encoding.UTF8))
                {
                    var newModel = xs.Deserialize(sr) as GridModel;
                    Apply(newModel);
                }
#endif
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
#endif
#if SILVERLIGHT
        /// <summary>
        /// Deserializes the specified <see cref="Syncfusion.Windows.Controls.Grid.GridModel"/> from a FileStream. 
        /// The File can be read from an Isolated Storage with the <see cref="System.IO.FileStream"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="System.IO.FileStream"/> has to be closed or used inside a closure when calling this method.
        /// </remarks>
        /// <param name="fileStream">The file stream.</param>
        public virtual void Deserialize(FileStream fileStream)
        {
            var xs = new XmlSerializer(typeof(GridModel));
            try
            {
                using (var sr = new StreamReader(fileStream))
                {
                    var newModel = xs.Deserialize(sr) as GridModel;
                    Apply(newModel);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
#endif
#if !WinRT
        /// <summary>
        /// Deserializes <see cref="Syncfusion.Windows.Controls.Grid.GridModel"/> properties from string.
        /// </summary>
        /// <param name="content">The content.</param>
        public virtual void DeserializeFromString(string content)
        {
            var xs = new XmlSerializer(typeof(GridDataTableProperties));
            using (var sReader = new StringReader(content))
            {
#if WPF
                using (var sr = new XmlTextReader(sReader))
                {
                    var newModel = xs.Deserialize(sr) as GridModel;
                    Apply(newModel);
                }
#else
                using (var sr = XmlReader.Create(sReader))
                {
                    var newModel = xs.Deserialize(sr) as GridModel;
                    Apply(newModel);
                }
#endif
            }
        }
#endif
#endif
        #endregion

        private void Apply(GridModel newModel)
        {
            for (int i = 0; i < this.views.Count; i++)
            {
                var grid = this.views[i];
                grid.Model = newModel;
                grid.InvalidateCells();
            }
        }


        #region OperationFeedback

        /// <summary>
        /// Occurs when an operation takes a longer time and the user should be notified
        /// about its status and have a chance to abort.
        /// </summary>
        /// <remarks>
        /// See <see cref="OperationFeedbackEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
#if !WinRT
        [Description("Occurs when an operation takes a longer time and the user should be notified about its status."),
        Category("Behavior")]
#endif
        public event OperationFeedbackEventHandler OperationFeedback;

        [XmlIgnore]
        Stack<OperationFeedback> feedbackStack = new Stack<OperationFeedback>();
        void IOperationFeedbackProvider.RaiseOperationFeedbackEvent(OperationFeedbackEventArgs e)
        {
            if (OperationFeedback != null)
            {
                OperationFeedback(this, e);
            }
        }

        Stack<OperationFeedback> IOperationFeedbackProvider.FeedbackStack
        {
            get { return feedbackStack; }
        }
        #endregion;
    }


#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public struct GridModelCurrentCellState : IDisposable
    {
        GridControlBase grid;
        RowColumnIndex cellRowColumnIndex;

        public static GridModelCurrentCellState Empty = new GridModelCurrentCellState(null, RowColumnIndex.Empty);

        public GridModelCurrentCellState(GridControlBase grid, RowColumnIndex cellRowColumnIndex)
        {
            this.grid = grid;
            this.cellRowColumnIndex = cellRowColumnIndex;
        }

        public bool IsEmpty
        {
            get { return grid == null; }
        }

        [XmlIgnore]
        public GridControlBase GridControl
        {
            get { return grid; }
            internal set { grid = value; }
        }

        public RowColumnIndex CellRowColumnIndex
        {
            get { return cellRowColumnIndex; }
        }

        public int RowIndex
        {
            get { return cellRowColumnIndex.RowIndex; }
        }

        public int ColumnIndex
        {
            get { return cellRowColumnIndex.ColumnIndex; }
        }

        public void Dispose()
        {

        }
    }
}