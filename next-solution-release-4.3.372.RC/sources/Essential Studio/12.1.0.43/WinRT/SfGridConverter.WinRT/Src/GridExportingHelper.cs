#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Data;
using Syncfusion.XlsIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

#if !WinRT
using System.Windows.Data;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
#endif

#if WPF
using System.Data;
#endif

namespace Syncfusion.UI.Xaml.Grid.Converter
{
    internal static class GridExportingHelper
    {

        internal static IEnumerable<GridSummaryRow> GetTopTableSummaries(this SfDataGrid grid)
        {
            return grid.TableSummaryRows.Where(row => row is GridTableSummaryRow && (row as GridTableSummaryRow).Position == TableSummaryRowPosition.Top);
        }

        internal static IEnumerable<GridSummaryRow> GetBottomTableSummaries(this SfDataGrid grid)
        {
            return grid.TableSummaryRows.Except(GetTopTableSummaries(grid));
        }

        internal static int ResolveSummaryColumnIndex(this SfDataGrid dataGrid, string columnName, List<string> excludeColumns)
        {
            if (excludeColumns.Contains(columnName))
                return -1;
            else
            {
                var gridColumns = from column in dataGrid.Columns
                                  where
                                      !excludeColumns.Contains(column.MappingName) && !column.IsHidden
                                  select column.MappingName;
                return gridColumns.ToList<string>().IndexOf(columnName);
            }
        }

        /// <summary>
        /// Creates the new CollectionViewAdv and adds the Summaries, Groups, SortDescriptions.
        /// </summary>
        /// <param name="dataGrid">SfdataGrid</param>
        /// <param name="record">RecordEntry:Parent Record</param>
        /// <param name="relationalColumn">RelationalColumn</param>
        /// <param name="propertyAccessProvider">IPropertyAccesProvider</param>
        /// <returns>ICollectionViewAdv</returns>
        internal static ICollectionViewAdv CreateCollectionView(this SfDataGrid dataGrid, object record, string relationalColumn, IPropertyAccessProvider propertyAccessProvider)
        {
            ICollectionViewAdv collectionViewAdv;
            if (string.IsNullOrEmpty(relationalColumn))
                collectionViewAdv = CreateCollectionViewAdv(record as IEnumerable, dataGrid);
            else
                collectionViewAdv = CreateCollectionViewAdv(propertyAccessProvider.GetValue(record, relationalColumn) as IEnumerable, dataGrid);

            if (collectionViewAdv == null)
                return null;
            if (dataGrid.TableSummaryRows != null)
            {
                foreach (ISummaryRow row in dataGrid.TableSummaryRows)
                {
                    collectionViewAdv.TableSummaryRows.Add(row);
                }
            }
            if (dataGrid.GroupSummaryRows != null)
            {
                foreach (ISummaryRow row in dataGrid.GroupSummaryRows)
                {
                    collectionViewAdv.SummaryRows.Add(row);
                }
            }

            collectionViewAdv.CaptionSummaryRow = dataGrid.CaptionSummaryRow;

            if (dataGrid.GroupColumnDescriptions.Count > 0)
            {
                foreach (GroupColumnDescription groupDescription in dataGrid.GroupColumnDescriptions)
                {
                    collectionViewAdv.GroupDescriptions.Add(new PropertyGroupDescription(groupDescription.ColumnName));
                }
            }

            if (dataGrid.SortColumnDescriptions.Count > 0)
            {
                foreach (SortColumnDescription sortColumnDescription in dataGrid.SortColumnDescriptions)
                {
                    collectionViewAdv.SortDescriptions.Add(new SortDescription(sortColumnDescription.ColumnName, sortColumnDescription.SortDirection));
                }
            }

            return collectionViewAdv;
        }

        //Source copied from dataGrid.cs
        internal static ICollectionViewAdv CreateCollectionViewAdv(IEnumerable source, SfDataGrid dataGrid)
        {
            ICollectionViewAdv view = null;
            if (source != null)
            {
#if WPF
                if (!CheckIsLegacyDataTable(source))
                {
#endif
                    if (source is GridQueryableCollectionViewWrapper)
                        view = source as GridQueryableCollectionViewWrapper;
                    else if (source is Syncfusion.Data.PagedCollectionView)
                        view = source as Syncfusion.Data.PagedCollectionView;
                    else if (dataGrid.View != null && dataGrid.View is PagedCollectionView)
                        view = new PagedCollectionView(source);
                    else
                        view = new GridQueryableCollectionViewWrapper(source, dataGrid);
                    if (dataGrid.SourceType != null)
                        (view as GridQueryableCollectionViewWrapper).SetSourceType(dataGrid.SourceType);

#if !SILVERLIGHT && SyncfusionFramework4_0
                    if (view is GridQueryableCollectionViewWrapper)
                        ((GridQueryableCollectionViewWrapper)view).UsePLINQ = dataGrid.UsePLINQ;
#endif

#if WPF
                }
                else if (source is PagedCollectionView)
                {
                    view = source as PagedCollectionView;
                }
                else
                {
                    view = new GridDataTableCollectionViewWrapper(source, dataGrid);
                    if (dataGrid.SourceType != null)
                        (view as DataTableCollectionView).SetSourceType(dataGrid.SourceType);
                }
#endif
            }
            return view;
        }

#if WPF
        internal static bool CheckIsLegacyDataTable(IEnumerable source)
        {
            return source is DataTable || source is DataView;
        }
#endif

#if WPF
        internal static System.Drawing.Color ConvertColor(this Brush brush)
        {
            Color mediaColor = (brush as SolidColorBrush).Color;
            return System.Drawing.Color.FromArgb(mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B);
        }
#endif

    }

    public enum ExportCellType
    {
        HeaderCell,

        RecordCell,

        GroupCaptionCell,

        GroupSummaryCell,

        TableSummaryCell,

        TopTableSummaryCell,

        IndentCell,

        StackedHeaderCell
    }

    public class ExportCellStyle
    {
        public ExportCellStyle()
        {
            this.FontInfo = new ExportFontInfo();
        }

        public ExportCellStyle(Brush backGroundBrush, Brush foreGroundBrush)
        {
            this.BackGroundBrush = backGroundBrush;
            this.ForeGroundBrush = foreGroundBrush;
            this.FontInfo = new ExportFontInfo();
        }

        public Brush BackGroundBrush { get; set; }
        public Brush ForeGroundBrush { get; set; }
        public ExportFontInfo FontInfo { get; set; }
    }

    public class ExportFontInfo
    {
        public ExportFontInfo()
        {

        }

        private bool _bold;
        public bool Bold
        {
            get
            {
                return _bold;
            }
            set
            {
                _bold = value;
            }
        }

        private bool _italic;
        public bool Italic
        {
            get
            {
                return _italic;
            }
            set
            {
                _italic = value;
            }
        }

        private double _size = 11;
        public double Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
            }
        }

        private ExcelUnderline _underline = ExcelUnderline.None;
        public ExcelUnderline Underline
        {
            get
            {
                return _underline;
            }
            set
            {
                _underline = value;
            }
        }

        private string _fontName = "Calibri";
        public string FontName
        {
            get
            {
                return _fontName;
            }
            set
            {
                _fontName = value;
            }
        }
    }
}
