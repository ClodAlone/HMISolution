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
    using System.Collections.ObjectModel;
    using Syncfusion.Linq;
    using System.ComponentModel;
    using System.Collections;
    using System.Windows;
    using System.Windows.Media;
#if !SILVERLIGHT
    using System.Data;
#endif
    using Syncfusion.Windows.Data;
    using System.Xml.Serialization;
using System.Windows.Data;

    /// <summary>
    /// Pre-defined summary types to be used in <see cref="GridDataSummaryColumn"/>.
    /// </summary>
    //public enum GridDataSummaryType
    //{
    //    CountAggregate,
    //    DoubleAggregate,
    //    Int32Aggregate,
    //    Custom
    //}

#if !SILVERLIGHT
    [Serializable]
#endif
    public class GridDataSummaryColumn : DependencyObject, ISummaryColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataSummaryColumn"/> class.
        /// </summary>
        public GridDataSummaryColumn()
        {
        }

        /// <summary>
        /// DependencyProperty for <see cref="SummaryType"/> property.
        /// </summary>
        public static readonly DependencyProperty SummaryTypeProperty = DependencyProperty.Register("SummaryType", typeof(SummaryType), typeof(GridDataSummaryColumn), new PropertyMetadata(SummaryType.CountAggregate));

        /// <summary>
        /// Gets or sets the type of the summary. Specify SummaryType.Custom to include custom summary aggregation.
        /// </summary>
        /// <value>The type of the summary.</value>
        public SummaryType SummaryType
        {
            get
            {
                return (SummaryType)this.GetValue(GridDataSummaryColumn.SummaryTypeProperty);
            }

            set
            {
                this.SetValue(GridDataSummaryColumn.SummaryTypeProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="MappingName"/> property.
        /// </summary>
        public static readonly DependencyProperty MappingNameProperty = DependencyProperty.Register("MappingName", typeof(string), typeof(GridDataSummaryColumn), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the MappingName for the underlying source that is bound to <see cref="GridDataControl"/>.
        /// </summary>
        /// <value>The name of the mapping.</value>
        public string MappingName
        {
            get
            {
                return (string)this.GetValue(GridDataSummaryColumn.MappingNameProperty);
            }

            set
            {
                this.SetValue(GridDataSummaryColumn.MappingNameProperty, value);
            }
        }

        #region AggregateColumn (DependencyProperty)

        /// <summary>
        /// Gets / Sets the Aggregate column for computing aggregates. This property would take first precedence from the MappingName.
        /// </summary>
        public string AggregateColumn
        {
            get { return (string)GetValue(AggregateColumnProperty); }
            set { SetValue(AggregateColumnProperty, value); }
        }

        public static readonly DependencyProperty AggregateColumnProperty = DependencyProperty.Register("AggregateColumn", typeof(string), typeof(GridDataSummaryColumn), new PropertyMetadata(string.Empty));

        #endregion


#if !SILVERLIGHT

        /// <summary>
        /// DependencyProperty for <see cref="GridDataSummaryColumn.Name"/> property.
        /// </summary>
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(GridDataSummaryColumn), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the name. This should be a unique identity for the <see cref="GridDataSummaryColumn"/>.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return (string)this.GetValue(GridDataSummaryColumn.NameProperty);
            }

            set
            {
                this.SetValue(GridDataSummaryColumn.NameProperty, value);
            }
        }
#else

        #region SummaryColumnName (DependencyProperty)

        /// <summary>
        /// Gets / Sets the Name for the SummaryColumn
        /// </summary>
        public string SummaryColumnName
        {
            get { return (string)GetValue(SummaryColumnNameProperty); }
            set { SetValue(SummaryColumnNameProperty, value); }
        }

        public static readonly DependencyProperty SummaryColumnNameProperty = DependencyProperty.Register("SummaryColumnName", typeof(string), typeof(GridDataSummaryColumn), new PropertyMetadata(string.Empty, OnSummaryColumnNameChanged));

        private static void OnSummaryColumnNameChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var summaryColumn = dpo as GridDataSummaryColumn;
            if (args.NewValue != null)
            {
                var iSummaryCol = summaryColumn as ISummaryColumn;
                iSummaryCol.Name = args.NewValue.ToString();
            }
        }

        #endregion

        private string name = string.Empty;
        string ISummaryColumn.Name
        {
            get { return this.name; }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                }
            }
        }
#endif

        /// <summary>
        /// DependencyProperty for <see cref="ColumnStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty ColumnStyleProperty = DependencyProperty.Register("ColumnStyle", typeof(GridDataStyleInfo), typeof(GridDataSummaryColumn), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the column style.
        /// </summary>
        /// <value>The column style.</value>
        public GridDataStyleInfo ColumnStyle
        {
            get
            {
                return (GridDataStyleInfo)this.GetValue(GridDataSummaryColumn.ColumnStyleProperty);
            }

            set
            {
                this.SetValue(GridDataSummaryColumn.ColumnStyleProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataSummaryColumn.CustomAggregate"/>.
        /// </summary>
        public static readonly DependencyProperty CustomAggregateProperty = DependencyProperty.Register("CustomAggregate", typeof(ISummaryAggregate), typeof(GridDataSummaryColumn), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the custom aggregate. Should implement <see cref="ISummaryAggregate"/> interface to delegate the custom summaries.
        /// </summary>
        /// <value>The custom aggregate.</value>
        [XmlIgnore]
        public ISummaryAggregate CustomAggregate
        {
            get
            {
                return (ISummaryAggregate)this.GetValue(GridDataSummaryColumn.CustomAggregateProperty);
            }

            set
            {
                this.SetValue(GridDataSummaryColumn.CustomAggregateProperty, value);
            }
        }

        private string format = string.Empty;
        /// <summary>
        /// Gets or sets the Format property. Format should be set within single quotes,
        /// <para></para>
        /// <para><code lang="C#">column.Format = '{Sum:##.00}'</code></para>
        /// </summary>
        [TypeConverter(typeof(GridDataFormatConverter))]
        public string Format
        {
            get
            {
                return this.format;
            }

            set
            {
#if SILVERLIGHT
                var formatConv = new GridDataFormatConverter();
                var newValue = formatConv.ConvertFrom(null, System.Threading.Thread.CurrentThread.CurrentCulture, value);
#endif
#if !SILVERLIGHT
                this.format = value;
#else
                if (newValue != null)
                {
                    this.format = newValue.ToString();
                }
#endif
            }
        }
    }

    /// <summary>
    /// Provides data to hold properties for the SummaryRow.
    /// </summary>
    /// <remarks>
    /// <code language="XAML">
    ///            &lt;syncfusion:GridDataControl.SummaryRows&gt;
    ///                &lt;syncfusion:GridDataSummaryRow ShowSummaryInRow="True" Title="'Charges - {FreightSummary}$'"&gt;
    ///                    &lt;syncfusion:GridDataSummaryRow.RowStyle&gt;
    ///                        &lt;syncfusion:GridDataStyleInfo Background="PeachPuff" /&gt;
    ///                    &lt;/syncfusion:GridDataSummaryRow.RowStyle&gt;
    ///                    &lt;syncfusion:GridDataSummaryRow.SummaryColumns&gt;
    ///                        &lt;syncfusion:GridDataSummaryColumn Name="FreightSummary"  MappingName="Freight" SummaryType="Int32Aggregate" Format="'{Sum:##.00}'" /&gt;
    ///                    &lt;/syncfusion:GridDataSummaryRow.SummaryColumns&gt;
    ///                &lt;/syncfusion:GridDataSummaryRow&gt;
    ///                &lt;syncfusion:GridDataSummaryRow ShowSummaryInRow="False" Title="'Total - {ShipCount} Items'"&gt;
    ///                    &lt;syncfusion:GridDataSummaryRow.RowStyle&gt;
    ///                        &lt;syncfusion:GridDataStyleInfo Background="Yellow" /&gt;
    ///                    &lt;/syncfusion:GridDataSummaryRow.RowStyle&gt;
    ///                    &lt;syncfusion:GridDataSummaryRow.SummaryColumns&gt;
    ///                        &lt;syncfusion:GridDataSummaryColumn Name="ShipCount" MappingName="ShipName" SummaryType="CountAggregate" Format="'{Count}'" /&gt;
    ///                    &lt;/syncfusion:GridDataSummaryRow.SummaryColumns&gt;
    ///                &lt;/syncfusion:GridDataSummaryRow&gt;
    ///            &lt;/syncfusion:GridDataControl.SummaryRows&gt;
    /// </code>
    /// </remarks>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class GridDataSummaryRow : DependencyObject, ISummaryRow, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataSummaryRow"/> class.
        /// </summary>
        public GridDataSummaryRow()
        {
            this.summaryColumns = new ObservableCollection<ISummaryColumn>();
            this.serializableColumns = new ObservableCollection<GridDataSummaryColumn>();
            this.summaryColumns.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(summaryColumns_CollectionChanged);
        }

        void summaryColumns_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.isInSuspend)
            {
                return;
            }

            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    {
                        foreach (GridDataSummaryColumn col in e.NewItems)
                        {
                            this.serializableColumns.Add(col);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (GridDataSummaryColumn col in e.OldItems)
                    {
                        this.serializableColumns.Remove(col);
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    this.serializableColumns.Clear();
                    break;
            }
        }

        private bool isInSuspend = false;

        /// <summary>
        /// Initializes a <see cref="GridDataSummaryRow"/> from another instance.
        /// </summary>
        /// <param name="other">The other.</param>
        public void InitializeForm(GridDataSummaryRow other)
        {
            if (other.SerializableColumns.Count > 0)
            {
                this.isInSuspend = true;
                foreach (var col in other.SerializableColumns)
                {
                    this.summaryColumns.Add((ISummaryColumn)col);
                    this.serializableColumns.Add(col);
                }
                this.isInSuspend = false;
            }
            else
            {
                this.summaryColumns.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(summaryColumns_CollectionChanged);
                this.summaryColumns = other.SummaryColumns;
                this.summaryColumns.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(summaryColumns_CollectionChanged);
            }

            this.Title = other.Title;
            this.TitleColumnCount = other.TitleColumnCount;
            this.ShowSummaryInRow = other.ShowSummaryInRow;
            this.Name = other.Name;
            this.IsVisible = other.IsVisible;
            this.RowStyle = other.RowStyle;
        }

        private ObservableCollection<ISummaryColumn> summaryColumns;
        /// <summary>
        /// Gets the summary columns.
        /// </summary>
        /// <value>The summary columns.</value>
        [XmlIgnore]
        public ObservableCollection<ISummaryColumn> SummaryColumns
        {
            get
            {
                return this.summaryColumns;
            }
        }

        private ObservableCollection<GridDataSummaryColumn> serializableColumns;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ObservableCollection<GridDataSummaryColumn> SerializableColumns
        {
            get
            {
                return this.serializableColumns;
            }
        }

        internal void SetTableModel(GridDataTableModel tableModel)
        {
            this.Model = tableModel;
        }

        [XmlIgnore]
        public GridDataTableModel Model
        {
            get;
            private set;
        }

        /// <summary>
        /// DependencyProperty for <see cref="TitleColumnCount"/> property.
        /// </summary>
        public static readonly DependencyProperty TitleColumnCountProperty = DependencyProperty.Register("TitleColumnCount", typeof(int), typeof(GridDataSummaryRow), new PropertyMetadata(1, OnTitleColumnCountChanged));

        private static void OnTitleColumnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var summaryRow = d as GridDataSummaryRow;
            if (summaryRow.Model == null)
            {
                return;
            }

            if (summaryRow.Model.IsLoaded)
            {
                summaryRow.Model.InvalidateDisplay();
            }
        }

        /// <summary>
        /// Gets or sets the title column count. Specify this property to extend the covered cell range for this particular summary row.
        /// </summary>
        /// <value>The title column count.</value>
        public int TitleColumnCount
        {
            get
            {
                return (int)this.GetValue(GridDataSummaryRow.TitleColumnCountProperty);
            }

            set
            {
                this.SetValue(GridDataSummaryRow.TitleColumnCountProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataSummaryRow.IsVisible"/>.
        /// </summary>
        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.Register("IsVisible", typeof(bool), typeof(GridDataSummaryRow), new PropertyMetadata(true, OnIsVisibleChanged));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsVisible
        {
            get
            {
                return (bool)this.GetValue(GridDataSummaryRow.IsVisibleProperty);
            }

            set
            {
                this.SetValue(GridDataSummaryRow.IsVisibleProperty, value);
            }
        }

        private static void OnIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var summaryRow = d as GridDataSummaryRow;
            if (summaryRow.Model == null)
            {
                return;
            }

            if (summaryRow.Model.IsLoaded)
            {
                summaryRow.Model.InvalidateDisplay();
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataSummaryRow.ShowSummaryInRow" />.
        /// </summary>
        public static readonly DependencyProperty ShowSummaryInRowProperty = DependencyProperty.Register("ShowSummaryInRow", typeof(bool), typeof(GridDataSummaryRow), new PropertyMetadata(OnShowSummaryInRow));

        private static void OnShowSummaryInRow(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var summaryRow = d as GridDataSummaryRow;
            if (summaryRow.Model == null)
            {
                return;
            }

            if (summaryRow.Model.IsLoaded)
            {
                summaryRow.Model.InvalidateDisplay();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show summary in row].
        /// </summary>
        /// <value><c>true</c> if [show summary in row]; otherwise, <c>false</c>.</value>
        public bool ShowSummaryInRow
        {
            get
            {
                return (bool)this.GetValue(GridDataSummaryRow.ShowSummaryInRowProperty);
            }

            set
            {
                this.SetValue(GridDataSummaryRow.ShowSummaryInRowProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataSummaryRow.Name"/> property.
        /// </summary>
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(GridDataSummaryRow), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return (string)this.GetValue(GridDataSummaryRow.NameProperty);
            }

            set
            {
                this.SetValue(GridDataSummaryRow.NameProperty, value);
            }
        }

        private IValueConverter _Converter = null;
        [XmlIgnore]
        public IValueConverter Converter
        {
            get { return _Converter; }
            set { _Converter = value; }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataSummaryRow.Title"/> property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(GridDataSummaryRow), new PropertyMetadata(string.Empty, OnTitleChanged));

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var summaryRow = d as GridDataSummaryRow;
            if (summaryRow.Model == null)
            {
                return;
            }

            if (summaryRow.Model.IsLoaded)
            {
                summaryRow.Model.InvalidateDisplay();
            }
        }

        /// <summary>
        /// Gets or sets the Title property. Define the format that the summary row should
        /// display.
        /// <para></para>
        /// <code lang="C#">&lt;syncfusion:GridDataSummaryRow Background=&quot;Yellow&quot;
        /// ShowSummaryInRow=&quot;True&quot; Title=&quot;'{Name} Charges -
        /// {FreightSummary}$ for {OrderCount} Items'&quot;&gt;
        ///                     &lt;syncfusion:GridDataSummaryRow.SummaryColumns&gt;
        ///                         &lt;syncfusion:GridDataSummaryColumn
        /// Name=&quot;FreightSummary&quot;  MappingName=&quot;Freight&quot;
        /// SummaryType=&quot;Int32Aggregate&quot; Format=&quot;'{Sum:##}'&quot; /&gt;
        ///                         &lt;syncfusion:GridDataSummaryColumn
        /// Name=&quot;OrderCount&quot; MappingName=&quot;OrderDate&quot;
        /// SummaryType=&quot;CountAggregate&quot; Format=&quot;'{Count}'&quot; /&gt;
        ///                     &lt;/syncfusion:GridDataSummaryRow.SummaryColumns&gt;
        ///                 &lt;/syncfusion:GridDataSummaryRow&gt;</code>
        /// </summary>
        [TypeConverter(typeof(GridDataFormatConverter))]
        public string Title
        {
            get
            {
                return (string)this.GetValue(GridDataSummaryRow.TitleProperty);
            }

            set
            {
                this.SetValue(GridDataSummaryRow.TitleProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataSummaryRow.RowStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty RowStyleProperty = DependencyProperty.Register("RowStyle", typeof(GridDataStyleInfo), typeof(GridDataSummaryRow), new PropertyMetadata(OnRowStyleChanged));

        private static void OnRowStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var summaryRow = d as GridDataSummaryRow;
            if (summaryRow.Model == null)
            {
                return;
            }

            if (summaryRow.Model.IsLoaded)
            {
                summaryRow.Model.InvalidateDisplay();
            }
        }

        /// <summary>
        /// Gets or sets the row style.
        /// </summary>
        /// <value>The row style.</value>
        public GridDataStyleInfo RowStyle
        {
            get
            {
                return (GridDataStyleInfo)this.GetValue(GridDataSummaryRow.RowStyleProperty);
            }

            set
            {
                this.SetValue(GridDataSummaryRow.RowStyleProperty, value);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.summaryColumns.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(summaryColumns_CollectionChanged);
        }

        #endregion
    }

    class GridDataFormatConverter : TypeConverter
    {
        public GridDataFormatConverter()
        {
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            var formatString = value.ToString();
            if (formatString.Length > 1)
            {
                var sb = new StringBuilder();
                var startIdx = formatString.IndexOf("\'") + 1;
                var endIdx = formatString.LastIndexOf("\'");
                if (startIdx > 0)
                {
                    sb.Append(formatString.Substring(0, startIdx - 1));
                }
                for (int i = startIdx;i < endIdx;i++)
                {
                    char c = formatString[i];
                    sb.Append(c);
                }
                if (sb.Length > 0)
                {
                    return sb.ToString();
                }
            }

            return formatString;
        }
    }
}
