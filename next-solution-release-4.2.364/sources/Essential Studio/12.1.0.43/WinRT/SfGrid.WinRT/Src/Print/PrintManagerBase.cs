#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using Syncfusion.Data;
using System;
using System.Collections.Generic;
using System.Linq;
#if WinRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Printing;
using Windows.Graphics.Printing;
using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
#if WPF
using System.Windows.Documents;
using System.Windows.Markup;
using System.Printing;
#else
using System.Windows.Printing;
#endif
#endif
using Syncfusion.UI.Xaml.Grid.Utility;

namespace Syncfusion.UI.Xaml.Grid
{
    public class PrintManagerBase : INotifyPropertyChanged, IDisposable
    {
        #region Fields

        private readonly ICollectionViewAdv view;
        internal IPropertyAccessProvider provider;
        private IList source;
        internal int pageCount = 0;
        private double equalcolumnWidth;
        private const string GroupCaptionConstant = "{ColumnName} : {Key} - {ItemsCount} Items";
        private const double IndentWidth = 20d;
        private Dictionary<int, List<RowInfo>> pageDictionary;
        internal Action<bool> InValidate=null;
        internal bool isSuspended;
        internal bool isPagesInitialized;
        private readonly List<PrintPageControl> printControls;
        private int groupCount;
        

#if WinRT
        private IPrintDocumentSource printDocumentSource;
        private PrintDocument printDocument;
#endif

        #endregion

        #region Ctor

        internal PrintManagerBase(ICollectionViewAdv view)
        {
            if (view == null)
                throw new ArgumentNullException("View cannot be as null");
            this.view = view;

            if (printControls == null)
                printControls = new List<PrintPageControl>();
#if WinRT
            RegisterForPrinting();
#endif
        }

        #endregion

        #region Properties

        internal bool AllowColumnWidthFitToPrintPage { get; set; }

        internal bool AllowRepeatHeaders { get; set; }

        private Thickness printPageMargin = new Thickness(96);
        /// <summary>
        /// Gets or sets the Margin for printing pages.
        /// </summary>
        /// <value>Thichness</value>
        /// <remarks></remarks>
        public Thickness PrintPageMargin
        {
            get { return printPageMargin; }
            set
            {
                if (printPageMargin == value) return;
                printPageMargin = value;
                OnPrintPropertyChanged();
                OnPropertyChanged("PrintPageMargin");

            }
        }

        private double printPageHeight = 1122.52;
        /// <summary>
        /// Gets or sets the Height for print page.
        /// </summary>
        /// <value>double</value>
        public double PrintPageHeight
        {
            get { return printPageHeight; }
            set
            {
                if (printPageHeight == value) return;
                printPageHeight = value;
                OnPrintPropertyChanged();
                OnPropertyChanged("PrintPageHeight");
            }
        }

        private double printPageWidth = 793.70;
        /// <summary>
        /// Gets or Sets the Width of printable page.
        /// </summary>
        public double PrintPageWidth
        {
            get { return printPageWidth; }
            set
            {
                if (printPageWidth == value) return;
                printPageWidth = value;
                OnPrintPropertyChanged();
                OnPropertyChanged("PrintPageWidth");
            }
        }

        private double printPageHeaderHeight;
        /// <summary>
        /// Gets or sets the height for the print page header.
        /// </summary>
        /// <value>double</value>
        public double PrintPageHeaderHeight
        {
            get { return printPageHeaderHeight; }
            set
            {
                if (printPageHeaderHeight == value) return;
                printPageHeaderHeight = value;
                OnPrintPropertyChanged();
                OnPropertyChanged("PrintPageHeaderHeight");
            }
        }

        private double printPageFooterHeight;
        /// <summary>
        /// Gets or sets the height of Print page footer.
        /// </summary>
        /// <value> double </value>
        public double PrintPageFooterHeight
        {
            get { return printPageFooterHeight; }
            set
            {
                if (printPageFooterHeight == value) return;
                printPageFooterHeight = value;
                OnPrintPropertyChanged();
                OnPropertyChanged("PrintPageFooterHeight");
            }
        }

        private double printHeaderRowHeight = 28;
        /// <summary>
        /// Gets or Sets the Print Header Row Height
        /// </summary>
        public double PrintHeaderRowHeight
        {
            get { return printHeaderRowHeight; }
            set
            {
                if (printHeaderRowHeight == value) return;
                printHeaderRowHeight = value;
                OnPrintPropertyChanged();
                OnPropertyChanged("PrintHeaderRowHeight");
            }
        }

        private double printRowHeight = 24;
        /// <summary>
        /// Gets or Sets the Print row Height
        /// </summary>
        public double PrintRowHeight
        {
            get { return printRowHeight; }
            set
            {
                if (printRowHeight == value) return;
                printRowHeight = value;
                OnPrintPropertyChanged();
                OnPropertyChanged("PrintRowHeight");
            }
        }

        private double printSummaryRowHeight = 24;
        /// <summary>
        /// Gets or Sets the Print Summary Row's Height
        /// </summary>
        internal double PrintSummaryRowHeight
        {
            get { return printSummaryRowHeight; }
            set
            {
                if (printSummaryRowHeight == value) return;
                printSummaryRowHeight = value;
                OnPrintPropertyChanged();
                OnPropertyChanged("PrintSummaryRowHeight");
            }
        }

        private double printGroupCaptionRowHeight = 24;
        /// <summary>
        /// Gets or Sets the Print Group Caption row's Height.
        /// </summary>
        internal double PrintGroupCaptionRowHeight
        {
            get { return printGroupCaptionRowHeight; }
            set
            {
                if (printGroupCaptionRowHeight == value) return;
                printGroupCaptionRowHeight = value;
                OnPrintPropertyChanged();
                OnPropertyChanged("PrintGroupCaptionRowHeight");
            }
        }

        private DataTemplate printPageHeaderTemplate;
        /// <summary>
        /// Gets or sets the Template for the print page header.
        /// </summary>
        /// <value>DataTemplate</value>
        public DataTemplate PrintPageHeaderTemplate
        {
            get { return printPageHeaderTemplate; }
            set
            {
                if (printPageHeaderTemplate == value) return;
                printPageHeaderTemplate = value;
                OnPrintPropertyChanged();
                OnPropertyChanged("PrintPageHeaderTemplate");
            }
        }

        private DataTemplate printPageFooterTemplate;
        /// <summary>
        /// Gets or sets the Template for Print page footer.
        /// </summary>
        /// <value></value>
        public DataTemplate PrintPageFooterTemplate
        {
            get { return printPageFooterTemplate; }
            set
            {
                if (printPageFooterTemplate == value) return;
                printPageFooterTemplate = value;
                OnPrintPropertyChanged();
                OnPropertyChanged("PrintPageFooterTemplate");
            }
        }

        private PrintOrientation printPageOrientation = PrintOrientation.Portrait;
        /// <summary>
        /// Gets or Sets the Page Orientation of the Print Page
        /// </summary>
        public PrintOrientation PrintPageOrientation
        {
            get { return printPageOrientation; }
            set
            {
                if (printPageOrientation == value) return;
                printPageOrientation = value;
                OnPropertyChanged("PrintPageFooterTemplate");
            }
        }

        private PrintScaleOptions printScaleOption = PrintScaleOptions.NoScaling;
        /// <summary>
        /// Gets or Sets the Print Scale option for the Print page
        /// </summary>
        public PrintScaleOptions PrintScaleOption
        {
            get { return printScaleOption; }
            set
            {
                if (printScaleOption == value) return;
                printScaleOption = value;
                OnPrintPropertyChanged();
                OnPropertyChanged("PrintScaleOption");
            }
        }

        #endregion

        #region Virtual Methods


        /// <summary>
        /// Invokes to Initilize the Print options.
        /// </summary>
        /// <remarks></remarks>
        protected virtual void InitializeProperties()
        {

        }


        /// <summary>
        /// Invokes to Get the Column's List that need to be printed.
        /// </summary>
        /// <returns></returns>
        /// <remarks>List<string></remarks>
        protected virtual List<string> GetColumnNames()
        {
#if WPF
            return (from PropertyDescriptor prop in view.GetItemProperties() select prop.Name).ToList();
#else
            return (from KeyValuePair<string, PropertyInfo> prop in view.GetItemProperties() select prop.Value.Name).ToList();
#endif
        }

        /// <summary>
        /// Invokes to Get the column width for the coresponding Mapping name.
        /// </summary>
        /// <param name="mappingName"></param>
        /// <returns>double</returns>
        protected virtual double GetColumnWidth(string mappingName)
        {
            return equalcolumnWidth;
        }

        /// <summary>
        /// Invokes to get the column's Header Text
        /// </summary>
        /// <param name="mappingName"></param>
        /// <returns>string</returns>
        protected virtual string GetColumnHeaderText(string mappingName)
        {
            return mappingName;
        }

        /// <summary>
        /// Invokes to get the formated Cell value from the column.
        /// </summary>
        /// <param name="cellValue"></param>
        /// <param name="mappingName"></param>
        /// <returns>string</returns>
        protected virtual string GetFormatedCellValue(object cellValue, string mappingName)
        {
            return cellValue != null ? cellValue.ToString() : string.Empty;
        }

        /// <summary>
        /// Invokes to get teh Column's Padding corresponding to the column's Mapping name.
        /// </summary>
        /// <param name="mappingName"></param>
        /// <returns>Thickness</returns>
        protected virtual Thickness GetColumnPadding(string mappingName)
        {
            return new Thickness(5, 0, 0, 0);
        }

        /// <summary>
        /// Invokes to get the Column Text alignment coressponding with the Mapping name.
        /// </summary>
        /// <param name="mappingName"></param>
        /// <returns>TextAlignment</returns>
        protected virtual TextAlignment GetColumnTextAlignment(string mappingName)
        {
            return TextAlignment.Left;
        }

        /// <summary>
        /// Invokes to get the Column's Text Wrapping corresponding to the Mapping name
        /// </summary>
        /// <param name="mappingName"></param>
        /// <returns>TextWrapping</returns>
        protected virtual TextWrapping GetColumnTextWrapping(string mappingName)
        {
            return TextWrapping.NoWrap;
        }

        /// <summary>
        /// Invokes to get the Column's Element to set content as for the Print Grid Cell
        /// </summary>
        /// <param name="record"></param>
        /// <param name="mappingName"></param>
        /// <returns>object</returns>
        protected virtual object GetColumnElement(object record, string mappingName)
        {
            return new TextBlock
            {
                Text =
                    GetFormatedCellValue(
                        provider.GetValue(record, mappingName),
                        mappingName),
                Padding = GetColumnPadding(mappingName),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = GetColumnTextAlignment(mappingName),
                TextWrapping = GetColumnTextWrapping(mappingName)
            };
        }

        /// <summary>
        /// Invokes to Get the Header column Element to set content for the PrintHeaderCell
        /// </summary>
        /// <param name="mappingName"></param>
        /// <returns>UIElement</returns>
        protected virtual UIElement GetColumnHeaderElement(string mappingName)
        {
            return new TextBlock
            {
                Text = GetColumnHeaderText(mappingName),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = GetColumnTextWrapping(mappingName)
            };
        }

        /// <summary>
        /// Invokes to get the Group Caption String Format
        /// </summary>
        /// <returns>string</returns>
        protected virtual string GetGroupCaptionStringFormat()
        {
            return GroupCaptionConstant;
        }

        /// <summary>
        /// Gets the CaptionSummary is in Row.
        /// </summary>
        protected virtual bool IsCaptionSummaryInRow
        {
            get
            {
                return true;
            }
        }

        public virtual PrintGridCell GetPrintGridCell(object record, string mappingName)
        {
            return new PrintGridCell();
        }

        #endregion

        #region Internal Methods


        internal void InitializePrint(bool needToInitProperties)
        {
            isSuspended = true;
            if (needToInitProperties)
                InitializeProperties();
            var columnNames = GetColumnNames();
            if(!columnNames.Any())
                return;

            if (!(view is Syncfusion.Data.PagedCollectionView || view is Syncfusion.Data.VirtualizingCollectionView) && view.GroupDescriptions.Any())
                groupCount = view.GroupDescriptions.Count;
            equalcolumnWidth = (PrintPageWidth -
                                (PrintPageMargin.Left + PrintPageMargin.Right +
                                 (groupCount * IndentWidth))) / columnNames.Count;
            equalcolumnWidth = equalcolumnWidth < 0 ? 0 : equalcolumnWidth;
            provider = view.GetPropertyAccessProvider();

            pageDictionary = new Dictionary<int, List<RowInfo>>();
            var avalHeight = PrintPageHeight -
                             (PrintPageMargin.Top + PrintPageMargin.Bottom + PrintPageHeaderHeight +
                              PrintPageFooterHeight);
            avalHeight = avalHeight < 0 ? 0 : avalHeight;
            var avalWidth = PrintPageWidth - (PrintPageMargin.Left + PrintPageMargin.Right);
            avalWidth = avalWidth < 0 ? 0 : avalWidth;
            if (view is Syncfusion.Data.PagedCollectionView)
                source = (view as Syncfusion.Data.PagedCollectionView).GetInternalList();
            else if (view is Syncfusion.Data.VirtualizingCollectionView)
                source = (view as Syncfusion.Data.VirtualizingCollectionView).GetInternalSource() as IList;
            else if (view.GroupDescriptions.Any())
                source = ToIEnumerable(view.TopLevelGroup.GetEnumerator()).ToList();
            else
                source = view.Records.ToList();

            ComputePages(source, new Size(avalWidth, avalHeight));
            pageCount = pageDictionary.Count;
            isSuspended = false;
            isPagesInitialized = true;
        }


#if WPF

        public void Print()
        {
            if (!isPagesInitialized)
                InitializePrint(true);

            Print(new PrintDialog
            {
                PrintTicket =
                {
                    PageOrientation =
                        PrintPageOrientation == PrintOrientation.Landscape
                            ? PageOrientation.Landscape
                            : PageOrientation.Portrait
                },
            }, true);
        }

        internal void PrintWithDialog()
        {
            var printDialog = new PrintDialog
            {
                UserPageRangeEnabled = true,
                PrintTicket =
                {
                    PageOrientation =
                        PrintPageOrientation == PrintOrientation.Landscape
                            ? PageOrientation.Landscape
                            : PageOrientation.Portrait
                },
            };


            Print(printDialog, printDialog.ShowDialog());
        }

        private void Print(PrintDialog printDialog, bool? canPrint)
        {
            if (printDialog == null)
                printDialog = new PrintDialog();

            printDialog.PrintTicket.PageMediaSize = new PageMediaSize(PrintPageWidth,
                PrintPageHeight);

            if (canPrint != null && (bool)canPrint)
            {
                var fixedDoc = new FixedDocument();

                int start = 1;
                int end = pageCount;
                if (printDialog.PageRange.PageFrom != 0 && printDialog.PageRange.PageTo != 0)
                {
                    start = printDialog.PageRange.PageFrom;
                    end = printDialog.PageRange.PageTo > end ? end : printDialog.PageRange.PageTo;
                }
                for (var i = start; i <= end; i++)
                {
                    var printpageControl = printControls.FirstOrDefault(x => x.PageIndex == i) ?? CreatePage(i);
                    var pageContent = new PageContent();
                    var fixedPage = new FixedPage
                    {
                        Height = PrintPageHeight,
                        Width = PrintPageWidth,
                        PrintTicket = printDialog.PrintTicket
                    };
                    fixedPage.Children.Add(printpageControl);
                    ((IAddChild)pageContent).AddChild(fixedPage);
                    fixedDoc.Pages.Add(pageContent);
                }

                printDialog.PrintDocument(fixedDoc.DocumentPaginator, "Printing");
                foreach (var page in fixedDoc.Pages)
                {
                    page.Child.Children.Clear();
                }
            }
            else
            {
                printControls.Clear();
            }
        }
#elif SILVERLIGHT
        public void Print()
        {
            if (!isPagesInitialized)
                InitializePrint(true);
            var i = 1;
            var printDoc = new PrintDocument();

            printDoc.PrintPage += (sender, args) =>
            {
                args.PageVisual = CreatePage(i);
                i++;
                args.HasMorePages = i <= pageCount;
            };
            printDoc.Print("Print Document");
        }
#endif

        internal PrintPageControl CreatePage(int pageIndex)
        {
            return CreatePage(pageIndex, new PrintPageControl(this));
        }

        internal PrintPageControl CreatePage(int pageIndex, PrintPageControl pageControl)
        {
            if (pageControl.Content is PrintPagePanel)
                (pageControl.Content as PrintPagePanel).Children.Clear();

            if (pageDictionary == null || pageDictionary.Count <= 0)
                return pageControl;

            pageControl.DataContext = this;

            pageControl.TotalPages = pageDictionary.Count;
            pageControl.PageIndex = pageIndex;

            var panel = new PrintPagePanel();
            var rowsInfo = panel.RowsInfoList = pageDictionary[pageIndex];
            foreach (var rowInfo in rowsInfo)
            {
                if (rowInfo.RecordIndex == -1)
                    AddHeaders(panel, rowInfo);
                else
                {
                    object record;
                    if (rowInfo.RecordIndex >= source.Count && view.TableSummaryRows.Any() &&
                        rowInfo.RecordIndex - source.Count < view.TableSummaryRows.Count)
                    {
                        record = view.Records.TableSummaries[rowInfo.RecordIndex - source.Count];
                        AddTableSummaryRecord(record as SummaryRecordEntry, panel, rowInfo);
                    }
                    else
                    {
                        record = source[rowInfo.RecordIndex] is RecordEntry
                                     ? (source[rowInfo.RecordIndex] as RecordEntry).Data
                                     : source[rowInfo.RecordIndex];
                        if (record is Group)
                            AddGroupRecord(record as Group, panel, rowInfo);
                        else if (record is SummaryRecordEntry)
                            AddSummaryRecord(record as SummaryRecordEntry, panel, rowInfo);
                        else
                            AddRecord(record, panel, rowInfo);
                    }
                }
            }

            pageControl.Content = panel;
            ProcessPrintPageScale(pageControl);
            return pageControl;
        }

        internal void ProcessPrintPageScale(PrintPageControl pageControl)
        {
            switch (PrintScaleOption)
            {
                case PrintScaleOptions.NoScaling:
                    pageControl.Scale(1, 1);
                    break;
                case PrintScaleOptions.FitAllColumnsonOnePage:
                    pageControl.Scale(GetScaleXValue(), 1);
                    break;
                case PrintScaleOptions.FitAllRowsonOnePage:
                    pageControl.Scale(1, GetScaleYValue());
                    break;
                case PrintScaleOptions.FitViewonOnePage:
                    pageControl.Scale(GetScaleXValue(), GetScaleYValue());
                    break;
            }
        }

        internal double GetScaleXValue()
        {
            return (PrintPageWidth - (PrintPageMargin.Left + PrintPageMargin.Right)) /
                   GetColumnNames().Sum(columnName => GetColumnWidth(columnName));
        }

        internal double GetScaleYValue()
        {
            return (PrintPageHeight - (PrintPageMargin.Top + PrintPageMargin.Bottom + PrintPageFooterHeight + PrintHeaderRowHeight)) /
                   (PrintPageHeaderHeight + (source.Count * PrintRowHeight) +
                    (view.TableSummaryRows.Count * PrintRowHeight));
        }

        #endregion

        #region Private Methods

        private void OnPrintPropertyChanged()
        {
            if (isSuspended) return;
            if (InValidate != null)
                InValidate(false);
        }

        private void ComputePages(IList source, Size avaliableSize)
        {
            var pageIndex = 1;
            var recordIndex = 0;
            var previoPageStartRecordIndex = 0;
            var columnNames = GetColumnNames();
            var totalColumnsWidth = columnNames.Sum(columnName => GetColumnWidth(columnName));
            var pagesPerRecord = (int)(Math.Round(totalColumnsWidth, 2) / Math.Round(avaliableSize.Width, 2));
            pagesPerRecord = printScaleOption == PrintScaleOptions.NoScaling ||
                             PrintScaleOption == PrintScaleOptions.FitAllRowsonOnePage
                                 ? Math.Round(totalColumnsWidth, 2) % Math.Round(avaliableSize.Width, 2) > 0
                                       ? pagesPerRecord + 1
                                       : pagesPerRecord
                                 : 1;
            int start, end;
            ComputeColumnforPage(pageIndex, columnNames, avaliableSize, pagesPerRecord, out start, out end);

            if (start < 0 || end < 0)
                return;

            var rowHeights = PrintHeaderRowHeight;
            var rowDictionary = new List<RowInfo>();
            InitializeHeadresForPage(rowDictionary, columnNames, pageIndex, pagesPerRecord, start, end);
            do
            {
                object record;
                if (PrintScaleOption == PrintScaleOptions.NoScaling ||
                    PrintScaleOption == PrintScaleOptions.FitAllColumnsonOnePage ||
                    (PrintScaleOption == PrintScaleOptions.FitAllRowsonOnePage &&
                     pageIndex + 1 <= pagesPerRecord && recordIndex == source.Count + view.TableSummaryRows.Count))
                {
                    if (rowHeights + PrintRowHeight > Math.Round(avaliableSize.Height, 2) ||
                        recordIndex == source.Count + view.TableSummaryRows.Count)
                    {
                        pageIndex++;
                        if (pagesPerRecord != 1 && pageIndex % pagesPerRecord != 1)
                            recordIndex = previoPageStartRecordIndex;

                        if (recordIndex > source.Count + view.TableSummaryRows.Count)
                            break;

                        previoPageStartRecordIndex = recordIndex;

                        if (recordIndex < source.Count)
                        {
                            rowDictionary = new List<RowInfo>();
                            ComputeColumnforPage(pageIndex, columnNames, avaliableSize, pagesPerRecord, out start,
                                                 out end);
                            InitializeHeadresForPage(rowDictionary, columnNames, pageIndex, pagesPerRecord, start, end);
                            rowHeights = pageIndex > pagesPerRecord
                                             ? (AllowRepeatHeaders ? PrintHeaderRowHeight : 0)
                                             : PrintHeaderRowHeight;

                            record = source[recordIndex];
                            AddParentRows(source, record, rowDictionary, ref rowHeights, columnNames, start,
                                end);
                            recordIndex++;
                        }
                    }
                }

                var cellRects = new List<CellInfo>();
                if (recordIndex >= source.Count && view.TableSummaryRows.Any() &&
                    recordIndex - source.Count < view.TableSummaryRows.Count)
                    record = view.Records.TableSummaries[recordIndex - source.Count];
                else if (recordIndex < source.Count)
                    record = source[recordIndex];
                else
                    break;

                if (record is Group)
                    AddGroupRecordToDict(record, cellRects, columnNames, rowHeights, start, end);
                else if (record is SummaryRecordEntry)
                    AddSummaryRecordToDict(record, cellRects, columnNames, rowHeights, start, end);
                else
                    AddRecordToDict(record, cellRects, columnNames, rowHeights, start, end);
                var needTopBorder = !rowDictionary.Any();

                if (!needTopBorder)
                {
                    var previousRow = rowDictionary.LastOrDefault();
                    if (previousRow != null && previousRow.CellInfos[0].CellRect.X > cellRects[0].CellRect.X)
                    {
                        needTopBorder = true;
                        previousRow.NeedBottomBorder = false;
                    }
                }

                rowDictionary.Add(new RowInfo
                {
                    CellInfos = cellRects,
                    RecordIndex = recordIndex,
                    NeedBottomBorder = true,
                    NeedTopBorder = needTopBorder
                });
                rowHeights += PrintRowHeight;
                recordIndex++;
            } while (recordIndex <= source.Count + view.TableSummaryRows.Count);

        }

        private void AddSummaryRecordToDict(object record, List<CellInfo> cellRects, List<string> columnNames,
                double rowHeights, int start, int end)
        {
            var summaryRecord = record as SummaryRecordEntry;
            var xPos = summaryRecord.Level >= 0 ? summaryRecord.Level * IndentWidth : 0;
            if (summaryRecord.SummaryRow.ShowSummaryInRow)
            {
                var columnWidths = groupCount * IndentWidth;
                for (var startIndex = start; startIndex <= end; startIndex++)
                    columnWidths += GetColumnWidth(columnNames[startIndex]);
                cellRects.Add(new CellInfo
                {
                    CellRect = new Rect(xPos, rowHeights, columnWidths - xPos, PrintRowHeight),
                    ColumnName = string.Empty
                });
            }
            else
            {
                for (var startindex = start; startindex <= end; startindex++)
                {
                    var name = columnNames[startindex];
                    var width = GetColumnWidth(name);
                    if (startindex == start)
                        width += ((groupCount * IndentWidth) - xPos);
                    cellRects.Add(new CellInfo
                    {
                        CellRect = new Rect(xPos, rowHeights, width, PrintRowHeight),
                        ColumnName = name
                    });
                    xPos += width;
                }
            }
        }

        private void AddParentRows(IList source, object record, List<RowInfo> rowDictionary,
                ref double rowHeights, List<string> columnNames, int start, int end)
        {
            var needTopBorder = true;
            var cellInfos = new List<CellInfo>();
            if (record is Group)
            {
                var gGroup = record as Group;
                if (gGroup.Level > 1)
                    AddParentRows(source, gGroup.Parent, rowDictionary, ref rowHeights, columnNames,
                        start, end);

                AddGroupRecordToDict(record, cellInfos, columnNames, rowHeights, start, end);
                needTopBorder = !rowDictionary.Any();
                if (!needTopBorder)
                {
                    var previousRow = rowDictionary.LastOrDefault();
                    if (previousRow != null && previousRow.CellInfos[0].CellRect.X > cellInfos[0].CellRect.X)
                    {
                        needTopBorder = true;
                        previousRow.NeedBottomBorder = false;
                    }
                }
                rowDictionary.Add(new RowInfo
                {
                    CellInfos = cellInfos,
                    RecordIndex = source.IndexOf(record),
                    NeedBottomBorder = true,
                    NeedTopBorder = needTopBorder
                });

                rowHeights += PrintRowHeight;

            }
            else if (record is RecordEntry)
            {
                var recordEntry = (record as RecordEntry);
                AddParentRows(source, recordEntry.Parent, rowDictionary, ref rowHeights, columnNames,
                    start, end);

                AddRecordToDict(record, cellInfos, columnNames, rowHeights, start, end);
                needTopBorder = !rowDictionary.Any();
                if (!needTopBorder)
                {
                    var previousRow = rowDictionary.LastOrDefault();
                    if (previousRow != null && previousRow.CellInfos[0].CellRect.X > cellInfos[0].CellRect.X)
                    {
                        needTopBorder = true;
                        previousRow.NeedBottomBorder = false;
                    }
                }
                rowDictionary.Add(new RowInfo
                {
                    CellInfos = cellInfos,
                    RecordIndex = source.IndexOf(record),
                    NeedBottomBorder = true,
                    NeedTopBorder = needTopBorder
                });
                rowHeights += PrintRowHeight;
            }
        }

        private void AddRecordToDict(object record, List<CellInfo> cellRects, List<string> columnNames, double rowHeights, int start, int end)
        {
            var xPos = record is RecordEntry && (record as RecordEntry).Level >= 0
                ? (record as RecordEntry).Level * IndentWidth
                : 0;
            for (int startIndex = start; startIndex <= end; startIndex++)
            {
                var columnName = columnNames[startIndex];
                var width = GetColumnWidth(columnName);

                cellRects.Add(new CellInfo
                {
                    CellRect = new Rect(xPos, rowHeights, width, PrintRowHeight),
                    ColumnName = columnName
                });
                xPos += width;
            }
        }

        private void AddGroupRecordToDict(object record, List<CellInfo> cellRects, List<string> columnNames, double rowHeights, int start, int end)
        {
            var gGroup = record as Group;
            var groupDescription =
                view.GroupDescriptions[gGroup.Level - 1] as PropertyGroupDescription;
            var xPos = gGroup.Level > 1 ? ((gGroup.Level - 1) * IndentWidth) : 0;
            if (groupDescription != null)
            {
                var groupName =
                    groupDescription.PropertyName;
                if (IsCaptionSummaryInRow)
                {
                    double columnWidths = groupCount * IndentWidth;
                    for (int startIndex = start; startIndex <= end; startIndex++)
                        columnWidths += GetColumnWidth(columnNames[startIndex]);

                    cellRects.Add(new CellInfo
                    {
                        CellRect = new Rect(xPos, rowHeights, columnWidths - xPos, PrintRowHeight),
                        ColumnName = groupName,
                    });
                }
                else
                {
                    if (gGroup.SummaryDetails.SummaryRow != null)
                    {
                        for (var startindex = start; startindex <= end; startindex++)
                        {
                            var name = columnNames[startindex];
                            var width = GetColumnWidth(name);
                            if (startindex == start)
                                width += ((groupCount * IndentWidth) - xPos);
                            cellRects.Add(new CellInfo
                            {
                                CellRect = new Rect(xPos, rowHeights, width, PrintRowHeight),
                                ColumnName = name
                            });
                            xPos += width;
                        }
                    }
                }

            }
        }

        private void ComputeColumnforPage(int pageIndex, List<string> columnNames, Size avaliableSize, int pagesPerRecord, out int startColumn, out int endColumn)
        {
            startColumn = 0;
            endColumn = columnNames.Count - 1;
            if (pagesPerRecord == 1)
                return;
            if (pageDictionary.Keys.Contains(pageIndex - 1))
            {
                var rowdetail = pageDictionary[pageIndex - 1].FirstOrDefault();
                if (rowdetail != null)
                {
                    var lastColumn = rowdetail.CellInfos.LastOrDefault();
                    if (lastColumn == null)
                    {
                        startColumn = -1;
                        endColumn = -1;
                        return;
                    }
                    var lastColumnofPreviousPage = lastColumn.ColumnName;
                    var lastColIndex = columnNames.IndexOf(lastColumnofPreviousPage);
                    startColumn = lastColIndex >= columnNames.Count - 1 ? 0 : lastColIndex + 1;
                }
            }

            endColumn = columnNames.Count() - 1;
            double columnWidths = groupCount * IndentWidth;
            int columnsCount = columnNames.Count();
            for (var colIndex = startColumn; colIndex < columnsCount; colIndex++)
            {
                columnWidths += GetColumnWidth(columnNames[colIndex]);
                if (Math.Round(columnWidths, 2) > Math.Round(avaliableSize.Width, 2))
                {
                    endColumn = colIndex - 1;
                    break;
                }
                else if (colIndex == columnsCount - 1)
                {
                    endColumn = colIndex;
                    break;
                }
            }
        }

        private void InitializeHeadresForPage(List<RowInfo> rowDictionary, List<string> columnNames, int pageIndex, int pagesPerRecord, int startIndex, int endIndex)
        {

            if (pageIndex > pagesPerRecord)
            {
                if (AllowRepeatHeaders)
                    AddHeaderRowToDict(rowDictionary, columnNames, startIndex, endIndex, 0);
            }
            else
                AddHeaderRowToDict(rowDictionary, columnNames, startIndex, endIndex, 0);
            pageDictionary.Add(pageIndex, rowDictionary);
        }

        private void AddHeaderRowToDict(List<RowInfo> rowDictionary, List<string> columnsList, int startIndex, int endIndex, double yPos)
        {
            var cellRects = new List<CellInfo>();
            double columnWidths = 0;
            for (var start = startIndex; start <= endIndex; start++)
            {
                var columnName = columnsList[start];

                var width = GetColumnWidth(columnName);
                if (start == startIndex)
                    width += groupCount * IndentWidth;
                width = width < 0 ? 0 : width;
                cellRects.Add(new CellInfo
                {
                    CellRect = new Rect(columnWidths, yPos, width, PrintHeaderRowHeight),
                    ColumnName = columnName
                });

                columnWidths += width;
            }
            rowDictionary.Add(new RowInfo
            {
                CellInfos = cellRects,
                RecordIndex = -1,
                NeedTopBorder = true,
                NeedBottomBorder = true
            });
        }

        public static IEnumerable<NodeEntry> ToIEnumerable<NodeEntry>(IEnumerator<NodeEntry> enumerator)
        {
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
            yield break;
        }

        private void AddHeaders(Panel panel, RowInfo rowInfo)
        {
            var topThickNess = rowInfo.NeedTopBorder ? 1 : 0;
            var bottomThickness = rowInfo.NeedBottomBorder ? 1 : 0;
            var i = 0;
            foreach (var cellInfo in rowInfo.CellInfos)
            {
                using (var cell = i == 0
                    ? new PrintHeaderCell
                    {
                        Width = cellInfo.CellRect.Width,
                        BorderThickness = new Thickness(1, topThickNess, 1, bottomThickness)
                    }
                    : new PrintHeaderCell
                    {
                        Width = cellInfo.CellRect.Width,
                        BorderThickness = new Thickness(0, topThickNess, 1, bottomThickness)
                    })
                {
                    cell.Content = GetColumnHeaderElement(cellInfo.ColumnName);
                    panel.Children.Add(cell);
                }
                i++;
            }

        }

        private void AddRecord(object record, Panel panel, RowInfo rowInfo)
        {
            var topThickNess = rowInfo.NeedTopBorder ? 1 : 0;
            var bottomThickness = rowInfo.NeedBottomBorder ? 1 : 0;
            var i = 0;
            foreach (var cellInfo in rowInfo.CellInfos)
            {
                var cell = GetPrintGridCell(record, cellInfo.ColumnName);
                if (i == 0)
                {
                    cell.Width = cellInfo.CellRect.Width;
                    cell.BorderThickness = new Thickness(1, topThickNess, 1, bottomThickness);
                }
                else
                {
                    cell.Width = cellInfo.CellRect.Width;
                    cell.BorderThickness = new Thickness(0, topThickNess, 1, bottomThickness);
                }
                var content = GetColumnElement(record, cellInfo.ColumnName);
                if (content is DataTemplate)
                {
                    cell.ContentTemplate = content as DataTemplate;
                    cell.Content = record;
                }
                else
                    cell.Content = content;
                panel.Children.Add(cell);
                i++;
            }
        }

        private void AddGroupRecord(Group gGroup, Panel panel, RowInfo rowInfo)
        {
            var cellsInfo = rowInfo.CellInfos;
            var topThickNess = rowInfo.NeedTopBorder ? 1 : 0;
            var bottomThickness = rowInfo.NeedBottomBorder ? 1 : 0;
            for (var start = 0; start < cellsInfo.Count; start++)
            {
                var cellInfo = cellsInfo[start];
                if (IsCaptionSummaryInRow)
                {
                    using (
                        var cell = new PrintCaptionSummaryCell
                        {
                            Width = cellInfo.CellRect.Width,
                            Padding = new Thickness(2, 0, 0, 0),
                            BorderThickness = new Thickness(1, topThickNess, 1, bottomThickness),
                            Content = new TextBlock
                            {
                                Text =
                                    view.TopLevelGroup.GetGroupCaptionText(gGroup,
                                        GetGroupCaptionStringFormat
                                            (), cellInfo.ColumnName),
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Center
                            }
                        })
                    {
                        panel.Children.Add(cell);
                    }
                }
                else
                {

                    using (var cell = new PrintCaptionSummaryCell
                    {
                        Width = cellInfo.CellRect.Width,
                        Padding = new Thickness(2, 0, 0, 0),
                        BorderThickness =
                            start == 0
                                ? new Thickness(1, topThickNess, 0, bottomThickness)
                                : start == (cellsInfo.Count - 1)
                                    ? new Thickness(0, topThickNess, 1, bottomThickness)
                                    : new Thickness(0, topThickNess, 0, bottomThickness)
                    })
                    {
                        var summaryColumns = gGroup.SummaryDetails.SummaryRow.SummaryColumns;
                        if (summaryColumns != null && summaryColumns.Any())
                        {
                            if (summaryColumns.Any(x => x.MappingName == cellInfo.ColumnName))
                            {
                                cell.Content = new TextBlock
                                {
                                    Text =
                                        SummaryCreator.GetSummaryDisplayText(gGroup.SummaryDetails,
                                            cellInfo.ColumnName,
                                            view),
                                    Padding = new Thickness(2, 0, 0, 0),
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    VerticalAlignment = VerticalAlignment.Center
                                };
                            }
                        }
                        panel.Children.Add(cell);
                    }
                }
            }
        }

        private void AddSummaryRecord(SummaryRecordEntry summaryRecord, PrintPagePanel panel, RowInfo rowInfo)
        {
            var cellsInfo = rowInfo.CellInfos;
            var topThickNess = rowInfo.NeedTopBorder ? 1 : 0;
            var bottomThickness = rowInfo.NeedBottomBorder ? 1 : 0;
            for (var start = 0; start < cellsInfo.Count; start++)
            {
                var cellInfo = cellsInfo[start];
                if (summaryRecord.SummaryRow.ShowSummaryInRow)
                {
                    using (
                        var cell = new PrintGroupSummaryCell
                        {
                            Width = cellInfo.CellRect.Width,
                            Padding = new Thickness(2, 0, 0, 0),
                            BorderThickness = new Thickness(1, topThickNess, 1, bottomThickness),
                            Content = new TextBlock
                            {
                                Text =
                                    SummaryCreator.GetSummaryDisplayTextForRow(summaryRecord, view),
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Center
                            }
                        })
                    {
                        panel.Children.Add(cell);
                    }
                }
                else
                {

                    using (
                        var cell = new PrintGroupSummaryCell
                        {
                            Width = cellInfo.CellRect.Width,
                            Padding = new Thickness(2, 0, 0, 0),
                            BorderThickness =
                                start == 0
                                    ? new Thickness(1, topThickNess, 0, bottomThickness)
                                    : start == (cellsInfo.Count - 1)
                                        ? new Thickness(0, topThickNess, 1, bottomThickness)
                                        : new Thickness(0, topThickNess, 0, bottomThickness)
                        })
                    {
                        var summaryColumns = summaryRecord.SummaryRow.SummaryColumns;
                        if (summaryColumns != null && summaryColumns.Any())
                        {
                            if (summaryColumns.Any(x => x.MappingName == cellInfo.ColumnName))
                            {
                                cell.Content = new TextBlock
                                {
                                    Text =
                                        SummaryCreator.GetSummaryDisplayText(summaryRecord, cellInfo.ColumnName,
                                            view),
                                    Padding = new Thickness(2, 0, 0, 0),
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    VerticalAlignment = VerticalAlignment.Center
                                };
                            }
                        }
                        panel.Children.Add(cell);
                    }
                }
            }
        }

        private void AddTableSummaryRecord(SummaryRecordEntry summaryRecord, PrintPagePanel panel, RowInfo rowInfo)
        {
            var cellsInfo = rowInfo.CellInfos;
            var topThickNess = rowInfo.NeedTopBorder ? 1 : 0;
            var bottomThickness = rowInfo.NeedBottomBorder ? 1 : 0;
            for (var start = 0; start < cellsInfo.Count; start++)
            {
                var cellInfo = cellsInfo[start];
                if (summaryRecord.SummaryRow.ShowSummaryInRow)
                {
                    using (
                        var cell = new PrintTableSummaryCell
                        {
                            Width = cellInfo.CellRect.Width,
                            Padding = new Thickness(2, 0, 0, 0),
                            BorderThickness = new Thickness(1, topThickNess, 1, bottomThickness),
                            Content = new TextBlock
                            {
                                Text =
                                    SummaryCreator.GetSummaryDisplayTextForRow(summaryRecord, view),
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Center
                            }
                        })
                    {
                        panel.Children.Add(cell);
                    }
                }
                else
                {

                    using (
                        var cell = new PrintTableSummaryCell
                        {
                            Width = cellInfo.CellRect.Width,
                            Padding = new Thickness(2, 0, 0, 0),
                            BorderThickness =
                                start == 0
                                    ? new Thickness(1, topThickNess, 0, bottomThickness)
                                    : start == (cellsInfo.Count - 1)
                                        ? new Thickness(0, topThickNess, 1, bottomThickness)
                                        : new Thickness(0, topThickNess, 0, bottomThickness)
                        })
                    {
                        var summaryColumns = summaryRecord.SummaryRow.SummaryColumns;
                        if (summaryColumns != null && summaryColumns.Any())
                        {
                            if (summaryColumns.Any(x => x.MappingName == cellInfo.ColumnName))
                            {
                                cell.Content = new TextBlock
                                {
                                    Text =
                                        SummaryCreator.GetSummaryDisplayText(summaryRecord, cellInfo.ColumnName,
                                            view),
                                    Padding = new Thickness(2, 0, 0, 0),
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    VerticalAlignment = VerticalAlignment.Center
                                };
                            }
                        }
                        panel.Children.Add(cell);
                    }
                }
            }
        }

        #endregion

        #region Internal Class

        internal class CellInfo
        {
            internal string ColumnName { get; set; }

            internal Rect CellRect { get; set; }
        }

        internal class RowInfo
        {
            internal int RecordIndex { get; set; }

            internal List<CellInfo> CellInfos { get; set; }

            internal bool NeedTopBorder { get; set; }

            internal bool NeedBottomBorder { get; set; }
        }

        #endregion

#if WinRT

        public async void Print()
        {
           await PrintManager.ShowPrintUIAsync();
        }

        private void RegisterForPrinting()
        {
            printDocument = new PrintDocument();
            printDocumentSource = printDocument.DocumentSource;
            printDocument.Paginate += OnCreatePrintPreviewPages;
            printDocument.GetPreviewPage += OnGetPrintPreviewPage;
            printDocument.AddPages += OnAddPrintPages;
            var printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested += OnPrintTaskRequested;
        }

        private void UnRegisterForPrinting()
        {
            printDocument.Paginate -= OnCreatePrintPreviewPages;
            printDocument.GetPreviewPage -= OnGetPrintPreviewPage;
            printDocument.AddPages -= OnAddPrintPages;
            var printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested -= OnPrintTaskRequested;
        }

        private void OnCreatePrintPreviewPages(object sender, PaginateEventArgs e)
        {
            printControls.Clear();
            var printOptions = e.PrintTaskOptions;
            var pageDescrip = printOptions.GetPageDescription(0);
            PrintPageHeight = pageDescrip.PageSize.Height;
            PrintPageWidth = pageDescrip.PageSize.Width;
            PrintPageOrientation = printOptions.Orientation;
            InitializePrint(true);
            printDocument.SetPreviewPageCount(pageCount, PreviewPageCountType.Intermediate);
        }

        private void OnGetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            var printpageControl = printControls.FirstOrDefault(x => x.PageIndex == e.PageNumber);
            if (printpageControl == null)
            {
                printpageControl = CreatePage(e.PageNumber);
                printControls.Add(printpageControl);
            }

            printDocument.SetPreviewPage(e.PageNumber, printpageControl);
        }

        private void OnAddPrintPages(object sender, AddPagesEventArgs e)
        {
            for (var i = 1; i <= pageCount; i++)
            {
                var printpageControl = printControls.FirstOrDefault(x => x.PageIndex == i) ?? CreatePage(i);
                printDocument.AddPage(printpageControl);
            }
            printDocument.AddPagesComplete();
        }

        private void OnPrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            args.Request.CreatePrintTask("Printing", sourceRequested => sourceRequested.SetSource(printDocumentSource));
        }

        ~PrintManagerBase()
        {
            UnRegisterForPrinting();
        }
#endif

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            provider = null;
            source = null;
            printControls.Clear();
            pageDictionary.Clear();
        }

        #endregion

    }

    public class GridPrintManager : PrintManagerBase
    {
        #region Fields

        private readonly SfDataGrid dataGrid;

        #endregion

        #region Ctor

        public GridPrintManager(SfDataGrid dataGrid)
            : base(dataGrid.View)
        {
            this.dataGrid = dataGrid;
        }

        #endregion

        #region Overrides

        protected override void InitializeProperties()
        {
            if (dataGrid.PrintSettings == null) return;
            var printSettings = dataGrid.PrintSettings;
            AllowColumnWidthFitToPrintPage = printSettings.AllowColumnWidthFitToPrintPage;
            PrintPageFooterHeight = printSettings.PrintPageFooterHeight;
            PrintPageFooterTemplate = printSettings.PrintPageFooterTemplate;
            PrintPageHeaderHeight = printSettings.PrintPageHeaderHeight;
            PrintPageHeaderTemplate = printSettings.PrintPageHeaderTemplate;
            PrintPageMargin = printSettings.PrintPageMargin;
            PrintPageWidth = printSettings.PrintPageWidth;
            PrintPageHeight = printSettings.PrintPageHeight;
            AllowRepeatHeaders = printSettings.AllowRepeatHeaders;
            PrintHeaderRowHeight = dataGrid.HeaderRowHeight;
            PrintRowHeight = dataGrid.RowHeight;
        }

        protected override List<string> GetColumnNames()
        {
            return this.dataGrid.Columns.Where(col => !col.IsHidden).Select(x => x.MappingName).ToList();
        }

        protected override string GetColumnHeaderText(string mappingName)
        {
            return !string.IsNullOrEmpty(this.dataGrid.Columns[mappingName].HeaderText)
                       ? this.dataGrid.Columns[mappingName].HeaderText
                       : mappingName;
        }

        protected override double GetColumnWidth(string mappingName)
        {
            if (!AllowColumnWidthFitToPrintPage && !double.IsNaN(this.dataGrid.Columns[mappingName].ActualWidth) && this.dataGrid.Columns[mappingName].ActualWidth < PrintPageWidth)
                return this.dataGrid.Columns[mappingName].ActualWidth;
            else
                return base.GetColumnWidth(mappingName);
        }

        protected override string GetFormatedCellValue(object cellValue, string mappingName)
        {
            if (cellValue != null)
            {
                if (dataGrid.Columns[mappingName].DisplayBinding != null && dataGrid.Columns[mappingName].DisplayBinding.Converter != null)
                {
                    return
                        Convert.ToString(dataGrid.Columns[mappingName].DisplayBinding.Converter.Convert(cellValue, null,
                                                                                                        null,
                                                                                                        null));
                }
            }
            return base.GetFormatedCellValue(cellValue, mappingName);
        }

        protected override Thickness GetColumnPadding(string mappingName)
        {
            return dataGrid.Columns[mappingName].Padding;
        }

        protected override TextAlignment GetColumnTextAlignment(string mappingName)
        {
            return dataGrid.Columns[mappingName].TextAlignment;
        }

        protected override TextWrapping GetColumnTextWrapping(string mappingName)
        {
            var column = dataGrid.Columns[mappingName];
            if (column != null && column is GridTextColumn)
                return (column as GridTextColumn).TextWrapping;

            return TextWrapping.NoWrap;
        }

        protected override object GetColumnElement(object record, string mappingName)
        {
            var column = dataGrid.Columns[mappingName];
            if (column is GridCheckBoxColumn)
            {
                var ckb = new CheckBox
                {
                    IsEnabled = false,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    DataContext = record
                };
#if !WinRT
                var bind = column.ValueBinding.CreateEditBinding(column.UpdateTrigger);
                ckb.SetBinding(ToggleButton.IsCheckedProperty, bind);
#else
                ckb.SetBinding(ToggleButton.IsCheckedProperty, column.ValueBinding);
#endif
                return ckb;
            }
            if (column is GridTemplateColumn)
                return (column as GridTemplateColumn).CellTemplate;
            if (column is GridImageColumn)
            {
                var image = new Image
                {
                    DataContext = record
                };
                image.SetBinding(Image.SourceProperty, column.ValueBinding);

                var stretchBind = new Binding { Path = new PropertyPath("Stretch"), Mode = BindingMode.TwoWay, Source = column };
                image.SetBinding(Image.StretchProperty, stretchBind);
                var imageWidthBinding = new Binding { Path = new PropertyPath("ImageWidth"), Mode = BindingMode.TwoWay, Source = column };
                image.SetBinding(FrameworkElement.WidthProperty, imageWidthBinding);

                var imageHeightBinding = new Binding { Path = new PropertyPath("ImageHeight"), Mode = BindingMode.TwoWay, Source = column };
                image.SetBinding(FrameworkElement.HeightProperty, imageHeightBinding);

#if WPF
                var stretchDirectionBind = new Binding { Path = new PropertyPath("StretchDirection"), Mode = BindingMode.TwoWay, Source = column };
                image.SetBinding(Image.StretchDirectionProperty, stretchDirectionBind);
#endif
                var paddingBind = new Binding { Path = new PropertyPath("Padding"), Mode = BindingMode.TwoWay, Source = column };
                image.SetBinding(FrameworkElement.MarginProperty, paddingBind);
                image.HorizontalAlignment = TextAlignmentToHorizontalAlignment(column.TextAlignment);

                return image;

            }

            var tb = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = GetColumnTextAlignment(mappingName),
                TextWrapping = GetColumnTextWrapping(mappingName),
                DataContext = record
            };
            if (!column.IsUnbound)
                tb.SetBinding(TextBlock.TextProperty, column.DisplayBinding);
            else
                tb.Text = dataGrid.GetUnBoundCellValue(column, record).ToString();
            var padding = column.ReadLocalValue(GridColumn.PaddingProperty);
            tb.Padding = padding != DependencyProperty.UnsetValue
                             ? column.Padding
                             : new Thickness(4, 3, 3, 1);
            return tb;
        }

        protected override string GetGroupCaptionStringFormat()
        {
            return dataGrid.GroupCaptionTextFormat ?? dataGrid.GroupCaptionConstant;
        }

        private HorizontalAlignment TextAlignmentToHorizontalAlignment(TextAlignment textAlignment)
        {
            HorizontalAlignment horizontalAlignment;

            switch (textAlignment)
            {
                case TextAlignment.Right:
                    horizontalAlignment = HorizontalAlignment.Right;
                    break;

                case TextAlignment.Center:
                    horizontalAlignment = HorizontalAlignment.Center;
                    break;

                case TextAlignment.Justify:
                    horizontalAlignment = HorizontalAlignment.Stretch;
                    break;
                default:
                    horizontalAlignment = HorizontalAlignment.Left;
                    break;
            }
            return horizontalAlignment;
        }

        protected override bool IsCaptionSummaryInRow
        {
            get
            {
                if (dataGrid.CaptionSummaryRow != null)
                    return dataGrid.CaptionSummaryRow.ShowSummaryInRow;
                else
                    return true;
            }
        }

        #endregion

    }

#if !WinRT

    /// <summary>
    /// Specifies the orientation options for the printed output.
    /// </summary>
    public enum PrintOrientation
    {
        Portrait,
        Landscape
    }
#endif
}

