#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !SILVERLIGHT
using Syncfusion.Data;
#endif
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
#if WinRT
using Windows.UI.Xaml;
#else
using System.Windows;
using System.ComponentModel;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    [ClassReference(IsReviewed = false)]
    public class SortColumnDescriptions : ObservableCollection<SortColumnDescription>
    {
        public SortColumnDescriptions()
        {

        }
        #region Property

        public SortColumnDescription this[string columnName]
        {
            get
            {
                var column = this.FirstOrDefault(col => col.ColumnName == columnName);
                return column;
            }
        }

        #endregion
    }

    [ClassReference(IsReviewed = false)]
    public class SortColumnDescription : DependencyObject
    {
        public static readonly DependencyProperty ColumnNameProperty = DependencyProperty.Register("ColumnName", typeof(string), typeof(SortColumnDescription), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName
        {
            get
            {
                return (string)this.GetValue(ColumnNameProperty);
            }

            set
            {
                this.SetValue(ColumnNameProperty, value);
            }
        }


        /// <summary>
        /// DependencyProperty for <see cref="GridDataSortColumn.SortDirection"/> property.
        /// </summary>
        public static readonly DependencyProperty SortDirectionProperty = DependencyProperty.Register("SortDirection", typeof(ListSortDirection), typeof(SortColumnDescription), new PropertyMetadata(ListSortDirection.Ascending));

        /// <summary>
        /// Gets or sets the sort direction.
        /// </summary>
        /// <value>The direction.</value>
        public ListSortDirection SortDirection
        {
            get
            {
                return (ListSortDirection)this.GetValue(SortDirectionProperty);
            }

            set
            {
                this.SetValue(SortDirectionProperty, value);
            }
        }

        //public IComparer<object> CustomComparer
        //{
        //    get { return (IComparer<object>)GetValue(CustomComparerProperty); }
        //    set { SetValue(CustomComparerProperty, value); }
        //}

        //public static readonly DependencyProperty CustomComparerProperty = DependencyProperty.Register("CustomComparer", typeof(IComparer<object>), typeof(GridDataSortColumn), new PropertyMetadata(null));

    }
}
