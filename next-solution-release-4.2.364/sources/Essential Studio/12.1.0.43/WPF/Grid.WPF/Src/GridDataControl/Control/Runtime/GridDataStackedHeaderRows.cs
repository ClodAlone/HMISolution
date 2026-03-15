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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    using Syncfusion.Windows.Shared;

    /// <summary>
    /// Provides the data for Stacked header implementation for <see cref="GridDataControl"/>.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class GridDataStackedHeaderRow
#if !SILVERLIGHT
        : Freezable
#else
 : DependencyObject
#endif
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataStackedHeaderRow"/> class.
        /// </summary>
        public GridDataStackedHeaderRow()
        {
#if !SILVERLIGHT
            this.Columns = new FreezableCollection<GridDataStackedHeaderColumn>();
#else
            this.Columns = new ObservableCollection<GridDataStackedHeaderColumn>();
#endif
        }

        /// <summary>
        /// Initializes from another instance of <see cref="GridDataStackedHeaderRow"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        public void InitializeFrom(GridDataStackedHeaderRow other)
        {
            this.Columns = other.Columns;
            this.Name = other.Name;
            this.RowStyle = other.RowStyle;
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataStackedHeaderRow.Name"/> property.
        /// </summary>
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(GridDataStackedHeaderRow), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return (string)this.GetValue(GridDataStackedHeaderRow.NameProperty);
            }

            set
            {
                this.SetValue(GridDataStackedHeaderRow.NameProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataStackedHeaderRow.RowStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty RowStyleProperty = DependencyProperty.Register("RowStyle", typeof(GridDataStyleInfo), typeof(GridDataStackedHeaderRow), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the row style.
        /// </summary>
        /// <value>The row style.</value>
        public GridDataStyleInfo RowStyle
        {
            get
            {
                return (GridDataStyleInfo)this.GetValue(GridDataStackedHeaderRow.RowStyleProperty);
            }

            set
            {
                this.SetValue(GridDataStackedHeaderRow.RowStyleProperty, value);
            }
        }
        
#if !SILVERLIGHT

        /// <summary>
        /// DependencyProperty for <see cref="GridDataStackedHeaderRow.Columns"/> property.
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(FreezableCollection<GridDataStackedHeaderColumn>), typeof(GridDataStackedHeaderRow), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public FreezableCollection<GridDataStackedHeaderColumn> Columns
        {
            get
            {
                return (FreezableCollection<GridDataStackedHeaderColumn>)this.GetValue(GridDataStackedHeaderRow.ColumnsProperty);
            }

            set
            {
                this.SetValue(GridDataStackedHeaderRow.ColumnsProperty, value);
            }
        }
#else 
        /// <summary>
        /// DependencyProperty for <see cref="GridDataStackedHeaderRow.Columns"/> property.
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(ObservableCollection<GridDataStackedHeaderColumn>), typeof(GridDataStackedHeaderRow), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public ObservableCollection<GridDataStackedHeaderColumn> Columns
        {
            get
            {
                return (ObservableCollection<GridDataStackedHeaderColumn>)this.GetValue(GridDataStackedHeaderRow.ColumnsProperty);
            }

            set
            {
                this.SetValue(GridDataStackedHeaderRow.ColumnsProperty, value);
            }
        }

#endif

#if !SILVERLIGHT
        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
#endif

    }

    /// <summary>
    /// Provides the data for <see cref="GridDataStackedHeaderColumn"/> class.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class GridDataStackedHeaderColumn
#if !SILVERLIGHT
        : Freezable
#else
 : DependencyObject
#endif
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataStackedHeaderColumn"/> class.
        /// </summary>
        public GridDataStackedHeaderColumn()
        {
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataStackedHeaderColumn.ColumnStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty ColumnStyleProperty = DependencyProperty.Register("ColumnStyle", typeof(GridDataStyleInfo), typeof(GridDataStackedHeaderColumn), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the column style.
        /// </summary>
        /// <value>The column style.</value>
        public GridDataStyleInfo ColumnStyle
        {
            get
            {
                return (GridDataStyleInfo)this.GetValue(GridDataStackedHeaderColumn.ColumnStyleProperty);
            }

            set
            {
                this.SetValue(GridDataStackedHeaderColumn.ColumnStyleProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataStackedHeaderColumn.Name"/> property.
        /// </summary>
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(GridDataStackedHeaderColumn), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return (string)this.GetValue(GridDataStackedHeaderColumn.NameProperty);
            }

            set
            {
                this.SetValue(GridDataStackedHeaderColumn.NameProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataStackedHeaderColumn.HeaderText"/> property.
        /// </summary>
        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register("HeaderText", typeof(string), typeof(GridDataStackedHeaderColumn), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the header text.
        /// </summary>
        /// <value>The header text.</value>
        public string HeaderText
        {
            get
            {
                return (string)this.GetValue(GridDataStackedHeaderColumn.HeaderTextProperty);
            }

            set
            {
                this.SetValue(GridDataStackedHeaderColumn.HeaderTextProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataStackedHeaderColumn.ColumnSpan"/> property.
        /// </summary>
        public static readonly DependencyProperty ColumnSpanProperty = DependencyProperty.Register("ColumnSpan", typeof(int), typeof(GridDataStackedHeaderColumn), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the column span. Specify this property to merge the cells in the stacked header.
        /// </summary>
        /// <value>The column span.</value>
        public int ColumnSpan
        {
            get
            {
                return (int)this.GetValue(GridDataStackedHeaderColumn.ColumnSpanProperty);
            }

            set
            {
                this.SetValue(GridDataStackedHeaderColumn.ColumnSpanProperty, value);
            }
        }

#if !SILVERLIGHT
        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
#endif
    }
}
