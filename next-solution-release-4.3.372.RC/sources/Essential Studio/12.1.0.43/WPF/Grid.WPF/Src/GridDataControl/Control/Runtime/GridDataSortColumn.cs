#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Data;
    using System.Xml.Serialization;

    /// <summary>
    /// Provides the data for Sorting columns in <see cref="GridDataControl"/>.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class GridDataSortColumn : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataSortColumn"/> class.
        /// </summary>
        public GridDataSortColumn()
        {
        }

        /// <summary>
        /// Initializes from another instance of <see cref="GridDataSortColumn"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        public void InitializeFrom(GridDataSortColumn other)
        {
            this.ColumnName = other.ColumnName;
            this.SortDirection = other.SortDirection;
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataSortColumn.ColumnName"/> property.
        /// </summary>
        public static readonly DependencyProperty ColumnNameProperty = DependencyProperty.Register("ColumnName", typeof(string), typeof(GridDataSortColumn), new
PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName
        {
            get
            {
                return (string)this.GetValue(GridDataSortColumn.ColumnNameProperty);
            }

            set
            {
                this.SetValue(GridDataSortColumn.ColumnNameProperty, value);
            }
        }


        /// <summary>
        /// DependencyProperty for <see cref="GridDataSortColumn.SortDirection"/> property.
        /// </summary>
        public static readonly DependencyProperty SortDirectionProperty = DependencyProperty.Register("SortDirection", typeof(ListSortDirection), typeof(GridDataSortColumn), new PropertyMetadata(ListSortDirection.Ascending));

        /// <summary>
        /// Gets or sets the sort direction.
        /// </summary>
        /// <value>The direction.</value>
        public ListSortDirection SortDirection
        {
            get
            {
                return (ListSortDirection)this.GetValue(GridDataSortColumn.SortDirectionProperty);
            }

            set
            {
                this.SetValue(GridDataSortColumn.SortDirectionProperty, value);
            }
        }

        #region CustomComparer (DependencyProperty)

        /// <summary>
        /// Gets / sets the custom comparer.
        /// </summary>
        [XmlIgnore]
        public IComparer<object> CustomComparer
        {
            get { return (IComparer<object>)GetValue(CustomComparerProperty); }
            set { SetValue(CustomComparerProperty, value); }
        }

        public static readonly DependencyProperty CustomComparerProperty = DependencyProperty.Register("CustomComparer", typeof(IComparer<object>), typeof(GridDataSortColumn), new PropertyMetadata(null));

        #endregion


    }

    /// <summary>
    /// Provides the data for specifying group columns.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class GridDataGroupColumn : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataGroupColumn"/> class.
        /// </summary>
        public GridDataGroupColumn()
        {
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataGroupColumn.ColumnName"/>.
        /// </summary>
        public static readonly DependencyProperty ColumnNameProperty = DependencyProperty.Register("ColumnName", typeof(string), typeof(GridDataGroupColumn), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName
        {
            get
            {
                return (string)this.GetValue(GridDataGroupColumn.ColumnNameProperty);
            }

            set
            {
                this.SetValue(GridDataGroupColumn.ColumnNameProperty, value);
            }
        }

        /// <summary>
        /// Initializes from another instance of <see cref="GridDataGroupColumn"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        public void InitializeFrom(GridDataGroupColumn other)
        {
            this.ColumnName = other.ColumnName;
            this.Converter = other.Converter;
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataGroupColumn.Converter"/> property.
        /// </summary>
        public static readonly DependencyProperty ConverterProperty = DependencyProperty.Register("Converter", typeof(IValueConverter), typeof(GridDataGroupColumn), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the converter.
        /// </summary>
        /// <value>The converter.</value>
        [XmlIgnore]
        public IValueConverter Converter
        {
            get
            {
                return (IValueConverter)this.GetValue(GridDataGroupColumn.ConverterProperty);
            }

            set
            {
                this.SetValue(GridDataGroupColumn.ConverterProperty, value);
            }
        }

        /*
        /// <summary>
        /// DependencyProperty for <see cref="GridDataGroupColumn.Converter"/> property.
        /// </summary>
        public static readonly DependencyProperty ComparerProperty = DependencyProperty.Register("Comparer", typeof(IEqualityComparer<object>), typeof(GridDataGroupColumn), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the comparer.
        /// </summary>
        /// <value>The comparer.</value>
        [XmlIgnore]
        public IEqualityComparer<object> Comparer
        {
            get
            {
                return (IEqualityComparer<object>)this.GetValue(GridDataGroupColumn.ComparerProperty);
            }

            set
            {
                this.SetValue(GridDataGroupColumn.ComparerProperty, value);
            }
        }*/
    }

    internal class GridDataSortColumnComparer
    {
        internal static int _Compare(GridDataSortColumn columnDescriptor, object x, object y)
        {
            int cmp = -1;
            //if (columnDescriptor.SortComparer != null)
            //{
            //    cmp = columnDescriptor.SortComparer.Compare(x, y);
            //}
            //else
            {
                cmp = _Compare(x, y);
            }

            if (columnDescriptor.SortDirection == ListSortDirection.Descending)
            {
                return -cmp;
            }

            return cmp;
        }

        internal static int _Compare(object x, object y)
        {
            int cmp = 0;
            bool xIsNull = (x == null || x is DBNull);
            bool yIsNull = (y == null || y is DBNull);

            if (yIsNull && xIsNull)
            {
                cmp = 0;
            }
            else if (xIsNull)
            {
                cmp = -1;
            }
            else if (yIsNull)
            {
                cmp = 1;
            }
            else if (x is IComparable)
            {
                cmp = ((IComparable)x).CompareTo(y);
            }

            return cmp;
        }
    }
}
