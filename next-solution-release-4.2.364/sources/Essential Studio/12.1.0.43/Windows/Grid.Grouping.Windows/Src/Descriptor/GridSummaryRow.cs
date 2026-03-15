//-------------------------------------------------------------------------------------------------
// <copyright file="GridSummaryRow.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.ComponentModel.Design.Serialization;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms;

#if ASPNET
using System.Xml.Serialization;
using System.Web.UI;
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    #region TypeConverter

    /// <summary>
    /// The type converter for <see cref="GridSummaryRowDescriptor"/> objects. <see cref="GridSummaryRowDescriptorTypeConverter"/>
    /// is a <see cref="DescriptorBaseConverter"/>. It overrides the default behavior of the
    /// ConvertTo method and adds support for design-time code serialization.
    /// </summary>
    public class GridSummaryRowDescriptorTypeConverter : DescriptorBaseConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridSummaryRowDescriptorTypeConverter()
            : base()
        {
        }

        /// <override/>
        /// <summary>
        /// Determines whether this object can be converted to the type specified.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="destinationType">The type you want to convert the object to.</param>
        /// <returns>True if this conversion is possible.</returns>
        public override /*TypeConverter*/ bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        /// <override/>
        /// <summary>
        /// Converts the given value to the type specified.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="culture">Current culture information.</param>
        /// <param name="value">Value to convert.</param>
        /// <param name="destinationType">Target type.</param>
        /// <returns>Converted object.</returns>
        public override /*TypeConverter*/ object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor)
                && (value is GridSummaryRowDescriptor))
            {
                GridSummaryRowDescriptor summaryRow = (GridSummaryRowDescriptor) value;
                Type type = typeof(GridSummaryRowDescriptor);

                if (summaryRow.ShouldSerializeAppearance())
                {
                    return new InstanceDescriptor(type.GetConstructor(new Type[0]), null, false);
                }                    
                else
                {
                    //// public GridSummaryRowDescriptor(string name, string title, params GridSummaryColumnDescriptor[] summaries)
                    GridSummaryColumnDescriptor[] summaryColumns = new GridSummaryColumnDescriptor[summaryRow.SummaryColumns.Count];
                    summaryRow.SummaryColumns.CopyTo(summaryColumns, 0);

                    if (summaryRow.ShouldSerializeTitle())
                    {
                        return new InstanceDescriptor(
                            type.GetConstructor(new Type[] { typeof(string), typeof(string), typeof(GridSummaryColumnDescriptor[]) }),
                            new object[] { summaryRow.Name, summaryRow.Title, summaryColumns },
                            true);
                    }
                    else
                    {
                        return new InstanceDescriptor(
                            type.GetConstructor(new Type[] { typeof(string), typeof(GridSummaryColumnDescriptor[]) }),
                            new object[] { summaryRow.Name, summaryColumns },
                            true);
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        } // end of method ConvertTo

        /// <override/>
        /// <summary>
        /// Gets a collection of properties for the specified type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="value">Value specifying type.</param>
        /// <param name="attributes">An array of System.Attribute objects that will be used as a filter.</param>
        /// <returns>A list of properties.</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection pds = TypeDescriptor.GetProperties(value.GetType(), attributes);

            string[] atts = new string[]
            {
                "Name",
                "Visible",
                "Title",
                "TitleColumnCount",
                "SummaryColumns"
            };

            return pds.Sort(atts);
        }
    }

    #endregion

    #region GridSummaryRowDescriptor

    /// <summary>
    /// A GridSummaryRowDescriptor declares a summary row with one or multiple GridSummaryColumnDescriptor elements.
    /// GridSummaryRowDescriptor descriptors are managed by the <see cref="GridSummaryRowDescriptorCollection"/> which
    /// is returned by the <see cref="GridTableDescriptor.SummaryRows"/> property
    /// of a <see cref="GridTableDescriptor"/>.
    /// </summary>
    /// <remarks>
    /// Each group in the GridTable has a summary section. The summary section has as many
    /// rows as there are GridSummaryRowDescriptors that are visible. Each GridSummaryRowDescriptor
    /// has a collection of GridSummaryColumnDescriptor columns. The GridSummaryColumnDescriptor
    /// defines the GridColumnDescriptor to calculate summary information for
    /// the SummaryType and the target column where the summary should be displayed in the grid.
    /// <para/>
    /// See also the Grid\Grouping\CustomSummary example for setting up custom summaries.
    /// </remarks>
    /// <example>
    /// The following example shows how to set up a summary:
    /// <code lang="C#">
    /// ' Setup a integrated summary
    /// Dim sd0 As New GridSummaryColumnDescriptor()
    /// sd0.DataMember = "Quantity"
    /// sd0.DisplayColumn = "Quantity"
    /// sd0.Format = "{Average:#.00}"
    /// sd0.SummaryType = SummaryType.DoubleAggregate
    /// Me.gridGroupingControl1.TableDescriptor.SummaryRows.Add(New GridSummaryRowDescriptor("Row 0", "Average", sd0))
    /// <para/>
    /// </code>
    /// <code lang="VB">
    /// <para/>
    /// // Setup a integrated summary
    /// GridSummaryColumnDescriptor sd0 = new GridSummaryColumnDescriptor();
    /// sd0.DataMember = "Quantity";
    /// sd0.DisplayColumn = "Quantity";
    /// sd0.Format = "{Average:#.00}";
    /// sd0.SummaryType = SummaryType.DoubleAggregate;
    /// this.gridGroupingControl1.TableDescriptor.SummaryRows.Add(new GridSummaryRowDescriptor("Row 0", "Average", sd0));
    /// </code>
    /// </example>
    [TypeConverter(typeof(GridSummaryRowDescriptorTypeConverter))]
    public class GridSummaryRowDescriptor : DescriptorBase, ICloneable, IItemPropertiesSource, IGridTableCellAppearanceSource
    {
        string name = string.Empty;
        string title = string.Empty;
        int titleColumnCount = 1;
        bool titleColumnCountModified = false;
        bool titleModified = false;
        GridSummaryRowDescriptorCollection parentCollection;
        GridSummaryColumnDescriptorCollection summaryColumns;
        GridTableDescriptor tableDescriptor;
        bool isFillRow = false;
        private bool visible;

        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs before a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanging;

        /// <overload>
        /// Initializes a new empty descriptor.
        /// </overload>
        /// <summary>
        /// Initializes a new empty descriptor.
        /// </summary>
        public GridSummaryRowDescriptor()
        {
            summaryColumns = new GridSummaryColumnDescriptorCollection(this);
            this.visible = true;
            WireSummaryColumns();
        }

        /// <override/>
        /// <summary>Gets the name of the descriptor.</summary>
        /// <returns>Descriptor name.</returns>
        public override string GetName()
        {
            return Name;
        }

        /// <override/>
        /// <summary>
        /// Returns a string holding the current object.
        /// </summary>
        /// <returns>String representation of the current object.</returns>
        public override string ToString()
        {
            return GetType().Name + " { " + name + " }";
        }

        void WireSummaryColumns()
        {
            summaryColumns.Changed += new ListPropertyChangedEventHandler(summaryColumns_Changed);
            summaryColumns.Changing += new ListPropertyChangedEventHandler(summaryColumns_Changing);
        }

        /// <summary>
        /// Initializes a new descriptor with a row name.
        /// </summary>
        /// <param name="name">The row name.</param>
        public GridSummaryRowDescriptor(string name)
            : this()
        {
            this.name = name;
        }

        /// <summary>
        /// Initializes a new descriptor with a row name, title, and collection of GridSummaryColumnDescriptors.
        /// </summary>
        /// <param name="name">The row name.</param>
        /// <param name="title">A summary row title. A title is displayed at the left-most cell of that row.</param>
        /// <param name="summaries">A collection of GridSummaryColumnDescriptors.</param>
        public GridSummaryRowDescriptor(string name, string title, GridSummaryColumnDescriptor[] summaries)
            : this()
        {
            this.name = name;
            this.title = title;
            this.titleModified = true;
            this.summaryColumns.AddRange(summaries);
        }

        /// <summary>
        /// Initializes a new descriptor with a row name, title, and collection of GridSummaryColumnDescriptors.
        /// </summary>
        /// <param name="name">The row name.</param>
        /// <param name="title">A summary row title. A title is displayed at the left-most cell of that row.</param>
        /// <param name="summary">A GridSummaryColumnDescriptor.</param>
        public GridSummaryRowDescriptor(string name, string title, GridSummaryColumnDescriptor summary)
            : this()
        {
            this.name = name;
            this.title = title;
            this.titleModified = true;
            this.summaryColumns.Add(summary);
        }

        /// <summary>
        /// Initializes a new descriptor with a row name and a collection of GridSummaryColumnDescriptors.
        /// </summary>
        /// <param name="name">The row name.</param>
        /// <param name="summary">A GridSummaryColumnDescriptor</param>
        public GridSummaryRowDescriptor(string name, GridSummaryColumnDescriptor summary)
            : this()
        {
            this.name = name;
            this.summaryColumns.Add(summary);
        }

        /// <summary>
        /// Initializes a new descriptor with a row name and a collection of GridSummaryColumnDescriptors.
        /// </summary>
        /// <param name="name">The row name.</param>
        /// <param name="summaries">A collection of GridSummaryColumnDescriptor</param>
        public GridSummaryRowDescriptor(string name, GridSummaryColumnDescriptor[] summaries)
            : this()
        {
            this.name = name;
            this.title = string.Empty;
            this.summaryColumns.AddRange(summaries);
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                name = "Disposed";
                title = string.Empty;
                parentCollection = null;
                if (summaryColumns != null)
                {
                    summaryColumns.Dispose();
                }

                summaryColumns = null;
                tableDescriptor = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Initializes this object and copies properties from another object. <see cref="PropertyChanging"/>
        /// and <see cref="PropertyChanged"/> events are raised for every property that is modified. If both
        /// objects are equal, no events are raised.
        /// </summary>
        /// <param name="other">The source object.</param>
        public void InitializeFrom(GridSummaryRowDescriptor other)
        {
            this.Name = other.Name;

            if (other.ShouldSerializeTitle())
            {
                this.Title = other.Title;
            }
            else
            {
                ResetTitle();
            }

            if (other.ShouldSerializeTitleColumnCount())
            {
                this.TitleColumnCount = other.TitleColumnCount;
            }
            else
            {
                ResetTitleColumnCount();
            }

            this.SummaryColumns.InitializeFrom(other.SummaryColumns);
            this.Appearance.InitializeFrom(other.Appearance);
            this.Visible = other.Visible;

            isFillRow = false;
            foreach (GridSummaryColumnDescriptor sd in this.SummaryColumns)
            {
                isFillRow |= sd.Style == GridSummaryStyle.FillRow;
            }
        }

        PropertyDescriptorCollection IItemPropertiesSource.GetItemProperties()
        {
            return tableDescriptor.ItemProperties;
        }

        internal void SetCollection(GridSummaryRowDescriptorCollection parentCollection)
        {
            this.parentCollection = parentCollection;
            if (parentCollection.tableDescriptor != null)
            {
                this.tableDescriptor = parentCollection.tableDescriptor;
            }

            if (this.name == string.Empty)
            {
                this.name = "Row " + this.parentCollection.Count;
            }

            foreach (GridSummaryColumnDescriptor sd in this.SummaryColumns)
            {
                sd.SetCollection(this.SummaryColumns);
                sd.SetParentRow(this);
            }
        }

        /// <summary>
        /// The collection this descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridSummaryRowDescriptorCollection Collection
        {
            get
            {
                return parentCollection;
            }
        }

        internal void SetTableDescriptor(GridTableDescriptor tableDescriptor)
        {
            this.tableDescriptor = tableDescriptor;
        }

        /// <summary>
        /// The TableDescriptor that this descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridTableDescriptor TableDescriptor
        {
            get
            {
                return tableDescriptor;
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="DescriptorPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnPropertyChanging(DescriptorPropertyChangedEventArgs e)
        {
            if (this.Disposing)
            {
                return;
            }

            if (PropertyChanging != null)
            {
                PropertyChanging(this, e);
            }

            if (parentCollection != null)
            {
                parentCollection.RaisePropertyItemChanging(this, e);
            }

            isFillRow = false;
            foreach (GridSummaryColumnDescriptor sd in this.SummaryColumns)
            {
                isFillRow |= sd.Style == GridSummaryStyle.FillRow;
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="DescriptorPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnPropertyChanged(DescriptorPropertyChangedEventArgs e)
        {
            if (this.Disposing)
            {
                return;
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }

            if (parentCollection != null)
            {
                parentCollection.RaisePropertyItemChanged(this, e);
            }

            isFillRow = false;
            foreach (GridSummaryColumnDescriptor sd in this.SummaryColumns)
            {
                isFillRow |= sd.Style == GridSummaryStyle.FillRow;
            }
        }

        /// <summary>
        /// Gets whether this row displays summary columns in grid cells below a certain header column
        /// or if the summary row is one large covered cell that spans the whole row.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsFillRow
        {
            get
            {
                return isFillRow;
            }
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Creates a copy of this descriptor.
        /// </summary>
        /// <returns>A copy of this descriptor.</returns>
        public GridSummaryRowDescriptor Clone()
        {
            GridSummaryRowDescriptor rd = new GridSummaryRowDescriptor();
            rd.InitializeFrom(this);
            rd.WireSummaryColumns();
            foreach (GridSummaryColumnDescriptor cd in rd.SummaryColumns)
            {
                cd.SetParentRow(rd);
            }

            return rd;
        }

        /// <override/>
        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the
        /// current <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current
        /// <see cref="T:System.Object" />. </param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (this == null && obj == null)
            {
                return true;
            }
            else if (this == null)
            {
                return false;
            }
            else if (!(obj is GridSummaryRowDescriptor))
            {
                return false;
            }
            else if (((GridSummaryRowDescriptor)obj).SummaryColumns == null)
            {
                return false;
            }

            return Equals((GridSummaryRowDescriptor) obj);
        }

        bool Equals(GridSummaryRowDescriptor other)
        {
            return other.name == name
                && other.Title == Title
                && other.TitleColumnCount == TitleColumnCount
                && other.SummaryColumns.Equals(this.SummaryColumns)
                && other.Visible == Visible
                && other.Appearance.Equals(this.Appearance);
        }

        /// <override/>
        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// The collection of <see cref="GridSummaryColumnDescriptor"/> elements.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [ListBindableAttribute(false)]
        [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [NotifyParentProperty(true)]
        [Description("The collection of GridSummaryColumnDescriptor elements.")]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridSummaryColumnDescriptorCollection SummaryColumns
        {
            get
            {
                return summaryColumns;
            }
        }

        internal void EnsureDisplayColumns()
        {
            if (!columnsInit)
            {
                this.InitDisplayColumns();
            }
        }

        internal void InitDisplayColumns()
        {
            columnsInit = true;

            GridTableDescriptor td = this.TableDescriptor;
            int colCount = td.GetColumnSetColCount();
            col2SummaryColumnMapping = new GridSummaryColumnDescriptor[colCount];

            foreach (GridSummaryColumnDescriptor scd in this.SummaryColumns)
            {
                string colName = scd.DisplayColumn;
                int columnIndex = td.Columns.IndexOf(colName);
                if (columnIndex != -1)
                {
                    GridColumnDescriptor cd = td.Columns[columnIndex];
                    bool found = false;
                    for (int row = 0; !found && row < td.RowsPerRecord; row++)
                    {
                        for (int col = 0; !found && col < colCount; col++)
                        {
                            if (td.RecordRowColumns[row, col] != null && td.RecordRowColumns[row, col].Name == colName)
                            {
                                found = true;
                                scd.RowInRecord = row;
                                scd.ColInRecord = col;
                                col2SummaryColumnMapping[col] = scd;
                            }
                        }
                    }
                }
            }

            columnsInit = true;
        }

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void ResetDisplayColumns()
        {
            columnsInit = false;
        }

        bool columnsInit = false;
        GridSummaryColumnDescriptor[] col2SummaryColumnMapping;

        /// <summary>
        /// Returns the GridSummaryColumnDescriptor that is displayed at the zero-based grid column position.
        /// </summary>
        /// <param name="col">The column index.</param>
        /// <returns>The GridSummaryColumnDescriptor.</returns>
        /// <remarks>
        /// </remarks>
        /// <example>
        /// The GridTableControl calls this method to determine which summary column to display
        /// at a specific grid column index. Given a grid cell rowIndex and colIndex, the grid
        /// calls
        /// <code lang="C#">
        /// fieldNum = TableDescriptor.ColIndexToField(colIndex);
        /// GridSummaryColumnDescriptor sc = sr.SummaryRowDescriptor.GetSummaryColumnAtCol(fieldNum);
        /// </code>
        /// </example>
        public GridSummaryColumnDescriptor GetSummaryColumnAtCol(int col)
        {
            if (col2SummaryColumnMapping != null && col >= 0 && col < col2SummaryColumnMapping.Length)
            {
                return col2SummaryColumnMapping[col];
            }

            return null;
        }

        /// <summary>
        /// The title text to be displayed in the column title.
        /// </summary>
        [LocalizableAttribute(true)]
        [Description("The title text to be displayed in the column title.")]
        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                if (title != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Title"));
                    title = value;
                    titleModified = true;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Title"));
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Title"/> has been modified
        /// and its value should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeTitle()
        {
            return titleModified;
        }

        /// <summary>
        /// Resets the title text.
        /// </summary>
        public void ResetTitle()
        {
            Title = string.Empty;
            titleModified = false;
        }

        /// <summary>
        /// The number of columns that the Title should span. Specify 0 if no title should be displayed.
        /// </summary>
        [Description("The number of columns that the Title should span.")]
        public int TitleColumnCount
        {
            get
            {
                if (!this.titleColumnCountModified)
                {
                    if (this.title == string.Empty)
                    {
                        return 0;
                    }
                }

                return titleColumnCount;
            }

            set
            {
                if (titleColumnCount != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("TitleColumnCount"));
                    titleColumnCount = value;
                    titleColumnCountModified = true;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("TitleColumnCount"));
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="TitleColumnCount"/> has been modified
        /// and its value should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeTitleColumnCount()
        {
            return titleColumnCountModified;
        }

        /// <summary>
        /// Resets the titleColumnCount text.
        /// </summary>
        public void ResetTitleColumnCount()
        {
            TitleColumnCount = 1;
            titleColumnCountModified = false;
        }

        /// <summary>
        /// Gets / sets whether the row should be visible in the grid. If you set
        /// this property to False, the row will not show up but you can still reference
        /// the summary columns from a caption bar in the <see cref="GridGroupOptionsStyleInfo.CaptionText"/>
        /// of a <see cref="GridGroupOptionsStyleInfo"/> as a custom summary token.
        /// </summary>
        [Description("Gets / Sets whether the row should be visible in the grid.")]
        public bool Visible
        {
            get
            {
                return this.visible;
            }

            set
            {
                if (this.visible != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Visible"));
                    this.visible = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Visible"));
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Visible"/> has been set to False
        /// and its value should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeVisible()
        {
            return this.visible != true;
        }

        /// <summary>
        /// Resets <see cref="Visible"/> property to be True.
        /// </summary>
        public void ResetVisible()
        {
            this.Visible = true;
        }

        /// <summary>
        /// The name of this descriptor. This name is used to look up the summary in the
        /// <see cref="GridSummaryRowDescriptorCollection"/>.
        /// </summary>
        [DefaultValue("")]
        [RefreshProperties(RefreshProperties.All)]
        [Description("The name of this descriptor.")]
        public virtual string Name
        {
            get
            {
                return name;
            }

            set
            {
                if (name != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Name"));
                    name = value;
                    if (!this.titleModified)
                    {
                        title = name;
                    }

                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Name"));
                }
            }
        }
#if ASPNET
        private ITemplate titleTemplate = null;
        /// <summary>
        /// Gets / sets the template used to display the title section of this summary row.
        /// The ContainerType of this template is GridCellTemplated.
        /// </summary>
        /// <remarks>
        /// This template lets you customize the display of the title section of this summary row.
        /// The ContainerType of this template is GridCellTemplated.
        /// </remarks>
        [Description("Gets / sets the template used to display the title section of this summary row."),
        Browsable(false),
        DefaultValue(null),
        PersistenceMode(PersistenceMode.InnerProperty),
        TemplateContainer(typeof(GridCellTemplated)),
        XmlIgnore(),
        NotifyParentProperty(true)
        ]
        public virtual ITemplate TitleTemplate {get {return this.titleTemplate;}set {this.titleTemplate = value;}
        }

        internal bool AreTemplatesAvailable()
        {
            if(this.titleTemplate != null)
                return true;

            foreach(GridSummaryColumnDescriptor gscd in this.SummaryColumns)
                if(gscd.AreTemplatesAvailable())
                    return true;

            return false;
        }
#endif

        private void summaryColumns_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            OnPropertyChanging(new DescriptorPropertyChangedEventArgs("SummaryColumns", e));
        }

        private void summaryColumns_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            ////                foreach (GridSummaryColumnDescriptor summaryColumn in summaryColumns)
            ////                    if (summaryColumn.ParentRow != this)
            ////                        return;
            OnPropertyChanged(new DescriptorPropertyChangedEventArgs("SummaryColumns", e));
            foreach (GridSummaryColumnDescriptor summaryColumn in summaryColumns)
            {
                summaryColumn.SetParentRow(this);
            }

            this.ResetDisplayColumns();
        }

        GridTableCellAppearance appearance;

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with default <see cref="GridTableCellStyleInfo"/>
        /// information for all cell elements that display data of this summary row.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Specifies appearance settings for all cell elements that display data of this summary row.")]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                    appearance.PropertyFilter = GridTableCellAppearance.SummaryDescriptorPropertyFilter;
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

    /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        #region IGridTableCellAppearanceSource Members

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal bool HasApparance
        {
            get { return ShouldSerializeAppearance(); }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetBaseAppearance()
        {
            if (this.TableDescriptor != null)
            {
                return TableDescriptor.Appearance;
            }

            return null;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Appearance", e));
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Appearance", e));
        }

        /// <summary>
        /// Gets the <see cref="GridEngine"/> that this summary column descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridEngine Engine
        {
            get
            {
                if (this.TableDescriptor != null)
                {
                    return this.TableDescriptor.Engine;
                }

                return null;
            }
        }
        #endregion
    }

    #endregion

    #region GridSummaryRowDescriptorCollection

    /// <summary>
    /// A collection from <see cref="GridSummaryRowDescriptor"/> that declares
    /// summary rows each with one or multiple GridSummaryColumnDescriptor elements.
    /// An instance of this collection is returned by the <see cref="GridTableDescriptor.SummaryRows"/> property
    /// of a <see cref="GridTableDescriptor"/>.
    /// </summary>
    /// <remarks>
    /// The SummaryRows collection contains GridSummaryRowDescriptors. GridSummaryRowDescriptors have a name,
    /// title, and collection of summary columns. A GridSummaryRowDescriptor also has a Read-only
    /// IsFillRow property. If this property is True, the summary should fill the
    /// whole row and not be displayed below individual columns. IsFillRow will return True if
    /// any of the GridSummaryColummDescriptors in the SummaryColumns collection is set to
    /// GridSummaryStyle.FillRow.
    /// <para/>
    /// The GridSummaryColummDescriptor defines where to display the column in the row.
    /// Essential properties are the name, format, DisplayColum, DataMember, and
    /// SummaryType. The multiple GridSummaryColumnDescriptor objects
    /// have a name and mapping name that identify the column for which a summary should be
    /// calculated for and a SummaryType property that defines the type of calculations to be performed.
    /// <para/>
    /// Possible SummaryTypes are: Count, BooleanAggregate, ByteAggregate, CharAggregate, DistinctCount,
    /// DoubleAggregate, Int32Aggregate, MaxLength, StringAggregate, Vector, DoubleVector, and Custom.
    /// <para/>
    /// When you specify the SummaryType.Custom type, you need to set the custom method through the
    /// CreateSummaryMethod property of the SummaryDescriptor. It is of type CreateSummaryDelegate and
    /// is called to create an instance of a summary object. You also need to handle the
    /// GridGroupingControl.QueryCustomSummary as demonstrated in the Grid/Grouping/CustomSummaries example.
    /// <para/>
    /// Each group in the GridTable has a summary section. The summary section has as many
    /// rows as there are GridSummaryRowDescriptors that are visible. Each GridSummaryRowDescriptor
    /// has a collection of GridSummaryColumnDescriptor columns. The GridSummaryColumnDescriptor
    /// defines the GridColumnDescriptor to calculate summary information for
    /// the SummaryType and the target column where the summary should be displayed in the grid.
    /// </remarks>
    /// <example>
    /// The following example shows how to set up a summary:
    /// <code lang="C#">
    /// ' Setup a integrated summary
    /// Dim sd0 As New GridSummaryColumnDescriptor()
    /// sd0.DataMember = "Quantity"
    /// sd0.DisplayColumn = "Quantity"
    /// sd0.Format = "{Average:#.00}"
    /// sd0.SummaryType = SummaryType.DoubleAggregate
    /// Me.gridGroupingControl1.TableDescriptor.SummaryRows.Add(New GridSummaryRowDescriptor("Row 0", "Average", sd0))
    /// <para/>
    /// </code>
    /// <code lang="VB">
    /// <para/>
    /// // Setup a integrated summary
    /// GridSummaryColumnDescriptor sd0 = new GridSummaryColumnDescriptor();
    /// sd0.DataMember = "Quantity";
    /// sd0.DisplayColumn = "Quantity";
    /// sd0.Format = "{Average:#.00}";
    /// sd0.SummaryType = SummaryType.DoubleAggregate;
    /// this.gridGroupingControl1.TableDescriptor.SummaryRows.Add(new GridSummaryRowDescriptor("Row 0", "Average", sd0));
    /// </code>
    /// </example>
    [TypeConverter(typeof(CustomTypeDescriptorConverter))]
    [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [ListBindableAttribute(false)]
    public class GridSummaryRowDescriptorCollection : IDisposable, IList, ICloneable, IInsideCollectionEditorProperty, ICustomTypeDescriptor
    {
        ArrayList inner = new ArrayList();
        internal int version;
        internal GridTableDescriptor tableDescriptor;
        internal bool insideCollectionEditor = false;

        /// <summary>
        /// Occurs after a property in a nested element or the collection is changed.
        /// </summary>
        public event ListPropertyChangedEventHandler Changed;

        /// <summary>
        /// Occurs before a property in a nested element or the collection is changed.
        /// </summary>
        public event ListPropertyChangedEventHandler Changing;

        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs before a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanging;

        /// <override/>
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return String.Format("GridSummaryRowDescriptorCollection: Count {0}, InsideColl {1}", Count, insideCollectionEditor);
        }

        /// <summary>
        /// Gets / sets whether the collection is manipulated inside a collection editor.
        /// </summary>
        public bool InsideCollectionEditor
        {
            get
            {
                return insideCollectionEditor;
            }

            set
            {
                if (insideCollectionEditor != value)
                {
                    insideCollectionEditor = value;
                }
#if DEBUG
                if (Switches.GroupingGrid.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this);
                }
#else
                ;
#endif
            }
        }

        void IInsideCollectionEditorProperty.InitializeFrom(object other)
        {
            InitializeFrom((GridSummaryRowDescriptorCollection) other);
        }

        /// <summary>
        /// Copies settings from another collection and raises <see cref="Changing"/> and <see cref="Changed"/>
        /// events if differences to the other collection are detected.
        /// </summary>
        /// <param name="other">The source collection.</param>
        public void InitializeFrom(GridSummaryRowDescriptorCollection other)
        {
            int i;
            int count = Math.Min(Count, other.Count);
            for (i = 0; i < count; i++)
            {
                this[i].InitializeFrom(other[i]);
            }

            for (; i < other.Count; i++)
            {
                Add(other[i].Clone());
            }

            while (Count > other.Count)
            {
                RemoveAt(Count - 1);
            }
        }

        /// <summary>
        /// Initializes a new empty collection.
        /// </summary>
        public GridSummaryRowDescriptorCollection()
        {
        }

        internal void SetTableDescriptor(GridTableDescriptor tableDescriptor)
        {
            this.tableDescriptor = tableDescriptor;
            int count = Count;
            for (int n = 0; n < count; n++)
            {
                this[n].SetCollection(this);
            }
        }

        internal GridSummaryRowDescriptorCollection(GridTableDescriptor tableDescriptor)
        {
            this.tableDescriptor = tableDescriptor;
        }

        /// <summary>
        /// Creates a copy of the collection and all its elements.
        /// </summary>
        /// <returns>A copy of the collection and all its elements.</returns>
        public GridSummaryRowDescriptorCollection Clone()
        {
            GridSummaryRowDescriptorCollection coll = new GridSummaryRowDescriptorCollection(this.tableDescriptor);
            coll.version = this.version+1000;
            int count = Count;
            GridSummaryRowDescriptor[] columnDescriptors = new GridSummaryRowDescriptor[count];
            for (int n = 0; n < count; n++)
            {
                coll.inner.Add(this[n].Clone());
                coll[n].SetCollection(coll);
            }

            return coll;
        }

        /// <override/>
        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the
        /// current <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current
        /// <see cref="T:System.Object" />. </param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (this == null && obj == null)
            {
                return true;
            }
            else if (this == null)
            {
                return false;
            }
            else if (!(obj is GridSummaryRowDescriptorCollection))
            {
                return false;
            }

            return Equals((GridSummaryRowDescriptorCollection) obj);
        }

        /// <override/>
        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// The version number of this collection. The version is increased each time the
        /// collection or an element within the collection was modified.
        /// </summary>
        public int Version
        {
            get
            {
                return version;
            }
        }

        bool Equals(GridSummaryRowDescriptorCollection other)
        {
            int count = Count;
            if (other.Count != count)
            {
                return false;
            }

            for (int n = 0; n < count; n++)
            {
                if (!this[n].Equals(other[n]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets / sets the element at the zero-based index.
        /// </summary>
        public GridSummaryRowDescriptor this[int index]
        {
            get
            {
                return (GridSummaryRowDescriptor) inner[index];
            }

            set
            {
                if (inner[index] != value)
                {
                    OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemChanged, index, value, null));
                    inner[index] = value;
                    value.SetCollection(this);
                    OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemChanged, index, value, null));
                }
            }
        }

        /// <summary>
        /// Gets / sets the element with the specified name.
        /// </summary>
        /// <summary>
        /// Gets / sets the element with the specified name.
        /// </summary>
#if ASPNET
        public GridSummaryRowDescriptor GetSummaryRowDescriptor(string name){return this[name];}
#endif

#if ASPNET
        // Designer ser. doesn't like overloaded accessors.
        internal
#else
        public
#endif
            GridSummaryRowDescriptor this[string name]
        {
            get
            {
                int index = Find(name);
                if (index == -1)
                {
                    return null;
                }

                return (GridSummaryRowDescriptor) inner[index];
            }
            
            set
            {
                int index = Find(name);
                if (index == -1)
                {
                    value.Name = name;
                    Add(value);
                }
                else
                {
                    OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemChanged, index, value, null));
                    inner[index] = value;
                    value.Name = name;
                    value.SetCollection(this);
                    OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemChanged, index, value, null));
                }
            }
        }

        internal int Find(string name)
        {
            for (int n = 0; n < Count; n++)
            {
                if (this[n].Name == name)
                {
                    return n;
                }
            }

            return -1;
        }

        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(GridSummaryRowDescriptor value)
        {
            if (value == null)
            {
                return false;
            }

            return inner.Contains(value);
        }

        /// <summary>
        /// Determines if the element with the specified name belongs to this collection.
        /// </summary>
        /// <param name="name">The name of the element to locate in the collection.</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(string name)
        {
            return Find(name) != -1;
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(GridSummaryRowDescriptor value)
        {
            return inner.IndexOf(value);
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element that matches the name in the collection.
        /// </summary>
        /// <param name="name">The name of the element to locate in the collection. </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(string name)
        {
            return Find(name);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in an array at which copying begins. </param>
        public void CopyTo(GridSummaryRowDescriptor[] array, int index)
        {
            int count = Count;
            for (int n = 0; n < count; n++)
            {
                array[index + n] = this[n];
            }
        }

        GridSummaryRowDescriptorCollection SyncRoot
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Returns an enumerator for the entire collection.
        /// </summary>
        /// <returns>An Enumerator for the entire collection.</returns>
        /// <remarks>Enumerators only allow reading of the data in the collection.
        /// Enumerators cannot be used to modify the underlying collection.</remarks>
        public GridSummaryRowDescriptorCollectionEnumerator GetEnumerator()
        {
            return new GridSummaryRowDescriptorCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts a descriptor element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, GridSummaryRowDescriptor value)
        {
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Insert, index, value, null));
            inner.Insert(index, value);
            value.SetCollection(this);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Insert, index, value, null));
        }

        /// <summary>
        /// Removes the specified descriptor element from the collection.
        /// </summary>
        /// <param name="value">The element to remove from the collection. If the value is NULL or the element is not contained
        /// in the collection, the method will do nothing.</param>
        public void Remove(GridSummaryRowDescriptor value)
        {
            int index = IndexOf(value);
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
            inner.Remove(value);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
        }

        /// <summary>
        /// Adds a SummaryDescriptor to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(GridSummaryRowDescriptor value)
        {
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, -1, value, null));
            int index = inner.Add(value);
            value.SetCollection(this);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, index, value, null));
            return index;
        }

        /// <summary>
        /// Removes the a descriptor element that matches the specified name from the collection.
        /// </summary>
        /// <param name="name">The name of the element to locate in the collection. </param>
        public void Remove(string name)
        {
            Remove(this[name]);
        }

        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. </param>
        public void RemoveAt(int index)
        {
            object value = inner[index];
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
            inner.RemoveAt(index);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
        }

        /// <summary>
        /// Disposes of the object and collection items.
        /// </summary>
        public void Dispose()
        {
            foreach (DescriptorBase db in inner)
            {
                db.Dispose();
            }

            inner.Clear();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            if (inner.Count > 0)
            {
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
                inner.Clear();
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
            }
        }

        /// <summary>
        /// Returns False.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns False since this collection has no fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns False.
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the collection. The property also
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// </summary>
        public int Count
        {
            get
            {
                return inner.Count;
            }
        }

        int visibleRowCount = -1;

        /// <summary>
        /// Gets the number of rows that have the <see cref="GridSummaryRowDescriptor.Visible"/> property set to True. This
        /// is also the number of summary rows that are displayed in the grid for each group.
        /// </summary>
        public int VisibleRowCount
        {
            get
            {
                if (visibleRowCount == -1)
                {
                    visibleRowCount = 0;
                    foreach (GridSummaryRowDescriptor row in this)
                    {
                        if (row.Visible)
                        {
                            visibleRowCount++;
                        }
                    }
                }

                return visibleRowCount;
            }
        }

        /// <summary>
        /// Raises the <see cref="Changing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanging(ListPropertyChangedEventArgs e)
        {
            if (!this.InsideCollectionEditor)
            {
#if DEBUG
                if (Switches.GroupingGrid.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, e.Item, e.Index, version);
                }
#else
                ;
#endif
                if (Changing != null)
                {
                    Changing(this, e);
                }
            }
        }

        internal void RaisePropertyItemChanging(GridSummaryRowDescriptor column, DescriptorPropertyChangedEventArgs e)
        {
            if (!this.InsideCollectionEditor)
            {
#if DEBUG
                if (Switches.GroupingGrid.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, column.Name, e.PropertyName);
                }                  
#else
                ;
#endif
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, IndexOf(column), column, e.PropertyName, e));
            }
        }

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanged(ListPropertyChangedEventArgs e)
        {
            visibleRowCount = -1;
            version++;
            if (!this.InsideCollectionEditor)
            {
#if DEBUG
                if (Switches.GroupingGrid.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, e.Item, e.Index, version);
                }
#else
                ;
#endif
                if (Changed != null)
                {
                    Changed(this, e);
                }
            }
        }

        internal void RaisePropertyItemChanged(GridSummaryRowDescriptor column, DescriptorPropertyChangedEventArgs e)
        {
            visibleRowCount = -1;
            if (!this.InsideCollectionEditor)
            {
#if DEBUG
                if (Switches.GroupingGrid.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, column.Name, e.PropertyName);
                }
#else
                ;
#endif
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, IndexOf(column), column, e.PropertyName, e));
            }
        }

        #region ICloneable Private Members
        object ICloneable.Clone()
        {
            return Clone();
        }
        #endregion

        #region IList Private Members

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                this[index] = (GridSummaryRowDescriptor) value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (GridSummaryRowDescriptor) value);
        }

        void IList.Remove(object value)
        {
            Remove((GridSummaryRowDescriptor) value);
        }

        bool IList.Contains(object value)
        {
            return Contains((GridSummaryRowDescriptor) value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((GridSummaryRowDescriptor) value);
        }

        int IList.Add(object value)
        {
            return Add((GridSummaryRowDescriptor) value);
        }

        #endregion

        #region ICollection Private Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((GridSummaryRowDescriptor[]) array, index);
        }

        object ICollection.SyncRoot
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region IEnumerable Private Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Raises the <see cref="PropertyChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="DescriptorPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnPropertyChanging(DescriptorPropertyChangedEventArgs e)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="DescriptorPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnPropertyChanged(DescriptorPropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        #region ICustomTypeDescriptor
        System.ComponentModel.AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor) this).GetProperties(null);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            ArrayList pds = new ArrayList();
            Attribute[] att = new Attribute[] 
            {
              new BrowsableAttribute(true),
              new System.Xml.Serialization.XmlIgnoreAttribute(),
              new RefreshPropertiesAttribute(RefreshProperties.All),
              new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden),
              new CategoryAttribute("Items")
            };

            ArrayList names = new ArrayList();
            foreach (DescriptorBase descriptor in this)
            {
                pds.Add(new DescriptorBasePropertyDescriptor(descriptor.GetName(), descriptor, att, GetType()));
                names.Add(descriptor.GetName());
            }

            PropertyDescriptorCollection pdc = new PropertyDescriptorCollection((PropertyDescriptor[]) pds.ToArray(typeof(PropertyDescriptor)));
            return pdc.Sort((string[]) names.ToArray(typeof(string)));
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
    }

    /// <summary>
    /// Enumerator class for <see cref="GridSummaryRowDescriptor"/> elements of a <see cref="GridSummaryRowDescriptorCollection"/>.
    /// </summary>
    public class GridSummaryRowDescriptorCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        GridSummaryRowDescriptorCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="parentCollection">The parent collection to enumerate.</param>
        public GridSummaryRowDescriptorCollectionEnumerator(GridSummaryRowDescriptorCollection parentCollection)
        {
            _coll = parentCollection;
            _next = _coll.Count > 0 ? 0 : -1;
        }

        #region IEnumerator Members

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public virtual void Reset()
        {
            _cursor = -1;
            _next = _coll.Count > 0 ? 0 : -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public GridSummaryRowDescriptor Current
        {
            get
            {
                return _coll[_cursor];
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next element; False if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            if (_next == -1)
            {
                return false;
            }

            _cursor = _next;

            _next++;
            if (_next >= _coll.Count)
            {
                _next = -1;
            }

            return _cursor != -1;
        }
        #endregion
    }
    #endregion
}