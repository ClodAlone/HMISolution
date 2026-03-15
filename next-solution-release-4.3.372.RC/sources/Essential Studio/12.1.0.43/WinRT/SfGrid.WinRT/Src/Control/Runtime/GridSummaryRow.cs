#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.ObjectModel;
using Syncfusion.Data;
using System;
#if WinRT
using Windows.UI.Xaml;
#else
using System.Windows;
using System.ComponentModel;
#endif


namespace Syncfusion.UI.Xaml.Grid
{
    [ClassReference(IsReviewed = false)]
    public class GridSummaryRow : DependencyObject, ISummaryRow
    {
        #region Fields

        private ObservableCollection<ISummaryColumn> summaryColumns;

        #endregion

        #region Dependency Registration

        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.Register("IsVisible", typeof(bool), typeof(GridSummaryRow), new PropertyMetadata(true));

        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(GridSummaryRow), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ShowSummaryInRowProperty = DependencyProperty.Register("ShowSummaryInRow", typeof(bool), typeof(GridSummaryRow), new PropertyMetadata(true));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(GridSummaryRow), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty TitleColumnCountProperty = DependencyProperty.Register("TitleColumnCount", typeof(int), typeof(GridSummaryRow), new PropertyMetadata(0));

        #endregion

        #region Ctor

        public GridSummaryRow()
        {
            this.summaryColumns = new ObservableCollection<ISummaryColumn>();
        }

        #endregion

        #region ISummaryRow Members

        /// <summary>
        /// Gets or sets a value indicating whether this instance .
        /// </summary>
        /// <value><see langword="true"/> if this instance ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        //public bool IsVisible
        //{
        //    get
        //    {
        //        return (bool)this.GetValue(IsVisibleProperty);
        //    }
        //    set
        //    {
        //        this.SetValue(IsVisibleProperty, value);
        //    }
        //}

        /// <summary>
        /// Gets or sets Name of Summary Row
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string Name
        {
            get
            {
                return (string)this.GetValue(NameProperty);
            }
            set
            {
                this.SetValue(NameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Summary Should show in row or column
        /// </summary>
        /// <value><see langword="true"/> if ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool ShowSummaryInRow
        {
            get
            {
                return (bool)this.GetValue(ShowSummaryInRowProperty);
            }
            set
            {
                this.SetValue(ShowSummaryInRowProperty, value);
            }
        }

        /// <summary>
        /// Gets the Summary Column Collection.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public ObservableCollection<ISummaryColumn> SummaryColumns
        {
            get { return this.summaryColumns; }
            set { this.summaryColumns = value; }
        }

        /// <summary>
        /// Gets or sets the Title
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
#if !WinRT
        [TypeConverter(typeof(GridSummaryFormatConverter))]
#endif
        public string Title
        {
            get
            {
                return (string)this.GetValue(TitleProperty);
            }
            set
            {
#if !WPF
                var formattedValue = value.SummaryFormatedString();
                this.SetValue(TitleProperty, formattedValue);
#else
                this.SetValue(TitleProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets TitleColumn Count
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        //public int TitleColumnCount
        //{
        //    get
        //    {
        //        return (int)this.GetValue(TitleColumnCountProperty);
        //    }
        //    set
        //    {
        //        this.SetValue(TitleColumnCountProperty, value);
        //    }
        //}

        #endregion
    }

    public class GridTableSummaryRow : GridSummaryRow
    {
        internal Action<GridTableSummaryRow, TableSummaryRowPosition> TableSummaryPositionChanged;

        public TableSummaryRowPosition Position
        {
            get { return (TableSummaryRowPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(TableSummaryRowPosition), typeof(GridTableSummaryRow), new PropertyMetadata(TableSummaryRowPosition.Bottom, OnTableSummaryPositionChanged));

        private static void OnTableSummaryPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var row = d as GridTableSummaryRow;
            if (row.TableSummaryPositionChanged != null)
                row.TableSummaryPositionChanged(row, (TableSummaryRowPosition)e.NewValue);
        }
    }

}
