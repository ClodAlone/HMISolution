//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableCellAppearance.cs" company="syncfusion">
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
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Grouping;

#if ASPNET
using System.Web.UI;
using Syncfusion.Web.Design.UI;
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// Defines cell types used by <see cref="GridTableCellAppearance"/>. For each GridTableCellType,
    /// the <see cref="GridTableCellStyleInfo"/> can be modified in a <see cref="GridTableCellAppearance"/>.
    /// </summary>
    public enum GridTableCellType
    {
        /// <summary>
        /// Represents None.
        /// </summary>
        None,

        /// <summary>
        /// Any cell in the grid.
        /// </summary>
        AnyCell,

        /// <summary>
        /// Any record cell.
        /// </summary>
        AnyRecordFieldCell,

        /// <summary>
        /// Any nested table cell.
        /// </summary>
        AnyNestedTableCell,

        /// <summary>
        /// Any cell in a group item.
        /// </summary>
        AnyGroupCell,

        /// <summary>
        /// Any summary cell.
        /// </summary>
        AnySummaryCell,

        /// <summary>
        /// Any header cell.
        /// </summary>
        AnyHeaderCell,

        /// <summary>
        /// Any indent cell.
        /// </summary>
        AnyIndentCell,

        /// <summary>
        /// Any preview cell.
        /// </summary>
        AnyPreviewCell,

        /// <summary>
        /// Empty cells.
        /// </summary>
        EmptyCell,

        /// <summary>
        /// The top-left header cell.
        /// </summary>
        TopLeftHeaderCell,

        /// <summary>
        /// Any row header cell.
        /// </summary>
        RowHeaderCell,

        /// <summary>
        /// Column header cell.
        /// </summary>
        ColumnHeaderCell,

        /// <summary>
        /// Filter bar header cell.
        /// </summary>
        ColumnHeaderWithFilterCell,

        /// <summary>
        /// Any header cell in a record row.
        /// </summary>
        RecordRowHeaderCell,

        /// <summary>
        /// Any header cell in a record preview row.
        /// </summary>
        RecordPreviewRowHeaderCell,

        /// <summary>
        /// Any header cell in a group footer.
        /// </summary>
        GroupFooterRowHeaderCell,

        /// <summary>
        /// Any header cell in a group header.
        /// </summary>
        GroupHeaderRowHeaderCell,

        /// <summary>
        /// Any header cell in a group preview section.
        /// </summary>
        GroupPreviewRowHeaderCell,

        /// <summary>
        /// Any header cell in a new record row.
        /// </summary>
        AddNewRecordRowHeaderCell,

        /// <summary>
        /// Any header cell in an alternate record row.
        /// </summary>
        AlternateRecordRowHeaderCell,

        /// <summary>
        /// Any cell in a group caption.
        /// </summary>
        GroupCaptionRowHeaderCell,

        /// <summary>
        /// Any filter bar row header cell.
        /// </summary>
        FilterBarRowHeaderCell,

        /// <summary>
        /// Any row header cell in an empty section.
        /// </summary>
        EmptySectionRowHeaderCell,

        /// <summary>
        /// PlusMinus cell in a record row.
        /// </summary>
        RecordPlusMinusCell,

        /// <summary>
        /// Field cell in a non-alternate record row.
        /// </summary>
        RecordFieldCell,

        /// <summary>
        /// Field cell in an alternate record row.
        /// </summary>
        AlternateRecordFieldCell,

        /// <summary>
        /// Field cell in new record row.
        /// </summary>
        AddNewRecordFieldCell,

        /// <summary>
        /// Cell in record preview row.
        /// </summary>
        RecordPreviewCell,

        /// <summary>
        /// Cell in group preview section.
        /// </summary>
        GroupPreviewCell,

        /// <summary>
        /// Row header cell in parent table for rows with indented nested table.
        /// </summary>
        NestedTableRowHeaderCell,

        /// <summary>
        /// Indent cell in parent table for rows with indented nested table.
        /// </summary>
        NestedTableIndentCell,

        /// <summary>
        /// Indent cell with T-line in parent table for rows with indented nested table.
        /// </summary>
        NestedTableIndentTCell,

        /// <summary>
        /// Indent cell with closing L-line in parent table for rows with indented nested table.
        /// </summary>
        NestedTableIndentLCell,

        /// <summary>
        /// Indent cell with continuous I-line in parent table for rows with indented nested table.
        /// </summary>
        NestedTableIndentICell,

        /// <summary>
        /// The nested table cell in which the nested table is drawn.
        /// </summary>
        NestedTableCell,

        /// <summary>
        /// Indent cell in group.
        /// </summary>
        GroupIndentCell,

        /// <summary>
        /// Indent cell with continuous I-line in group.
        /// </summary>
        GroupIndentICell,

        /// <summary>
        /// Indent cell with T-line in group.
        /// </summary>
        GroupIndentTCell,

        /// <summary>
        /// Indent cell with closing L-line in group.
        /// </summary>
        GroupIndentLCell,

        /// <summary>
        /// PlusMinus cell in group caption.
        /// </summary>
        GroupCaptionPlusMinusCell,

        /// <summary>
        /// Group caption cell.
        /// </summary>
        GroupCaptionCell,

        /// <summary>
        /// Any cell in group header section.
        /// </summary>
        GroupHeaderSectionCell,

        /// <summary>
        /// Any cell in group footer section.
        /// </summary>
        GroupFooterSectionCell,

        /// <summary>
        /// Indent cell in group header section.
        /// </summary>
        GroupHeaderIndentCell,

        /// <summary>
        /// Indent cell in group footer section.
        /// </summary>
        GroupFooterIndentCell,

        /// <summary>
        /// Field cell in filter bar.
        /// </summary>
        FilterBarCell,

        /// <summary>
        /// Field cell in summary row.
        /// </summary>
        SummaryFieldCell,

        /// <summary>
        /// Field cell in summary row with GridSummaryStyle.FillRow.
        /// </summary>
        SummaryFillRowCell,

        /// <summary>
        /// Title cell in summary row.
        /// </summary>
        SummaryTitleCell,

        /// <summary>
        /// Row header cell in summary row.
        /// </summary>
        SummaryRowHeaderCell,

        /// <summary>
        /// Empty cell in summary row.
        /// </summary>
        SummaryEmptyCell,

        /// <summary>
        /// Summary cell in caption bar.
        /// </summary>
        GroupCaptionSummaryCell,

        /// <summary>
        /// Stacked header cells.
        /// </summary>
        StackedHeaderCell,

        //// Note: If you add any values here, you must also update
        ////       GridTableCellAppearance.GridTableCellTypeMaxValue.

        //// see GridTableCellAppearance.GetBaseStyleNames for inheritance rules
    }

    /// <summary>
    /// Interface for hosting a <see cref="GridTableCellAppearance"/>.
    /// </summary>
    public interface IGridTableCellAppearanceSource
    {
        /// <summary>
        /// Returns the <see cref="GridTableCellAppearance"/> of this element.
        /// </summary>
        /// <returns>Returns the <see cref="GridTableCellAppearance"/></returns>
        GridTableCellAppearance GetAppearance();

        /// <summary>
        /// Determines whether the Appearance object has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>returns boolean value to determines whether the Appearance object has been modified and its contents should be serialized at design-time.</returns>
        bool ShouldSerializeAppearance();

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>Returns a <see cref="GridTableCellAppearance"/></returns>
        GridTableCellAppearance GetBaseAppearance();

        /// <summary>
        /// Returns a reference to the <see cref="GridEngine"/> this object belongs to.
        /// </summary>
        GridEngine Engine { get; }

        /// <summary>
        /// Notifies the host that properties in the Appearance object were changed.
        /// </summary>
        void RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e);

        /// <summary>
        /// Notifies the host that properties in the Appearance object will be changed.
        /// </summary>
        void RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e);
    }

    #region TypeConverter
    /// <summary>
    /// The type converter for <see cref="GridTableCellAppearance"/> objects. The <see cref="GridTableCellAppearanceTypeConverter"/>
    /// is an <see cref="ExpandableObjectConverter"/>. It overrides the <see cref="GetProperties"/> method and returns only appearance properties
    /// that were not filtered out with <see cref="GridTableCellAppearance.PropertyFilter"/> of the <see cref="GridTableCellAppearance"/> object.
    /// </summary>
    public class GridTableCellAppearanceTypeConverter : ExpandableObjectConverter
    {
        /// <override/>
        /// <summary>
        /// Gets a collection of properties for the specified type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="value">Value specifying the type.</param>
        /// <param name="attributes">An array of System.Attribute objects.</param>
        /// <returns>A list of properties.</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection pds
                = TypeDescriptor.GetProperties(value.GetType(), attributes);

            GridTableCellAppearance appearance = value as GridTableCellAppearance;
            if (appearance != null && appearance.PropertyFilter != null)
            {
                ArrayList pdc = new ArrayList();
                foreach (string prop in appearance.PropertyFilter)
                {
                    pdc.Add(pds[prop]);
                }

                return new PropertyDescriptorCollection((PropertyDescriptor[])pdc.ToArray(typeof(PropertyDescriptor)));
            }

            return pds;
        }

        /// <override/>
        /// <summary>
        /// Gets a value indicating whether this object supports properties using the
        /// specified context.
        /// </summary>
        /// <param name="context">Fformat
        /// context. </param>
        /// <returns>
        /// true because <see
        /// cref="TypeConverter.GetProperties(System.Object)" />
        /// should be called to find the properties of this object. This method never
        /// returns false.
        /// </returns>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
    #endregion

    /// <summary>
    /// The <see cref="GridTableCellAppearance"/> class stores <see cref="GridTableCellStyleInfo"/>
    /// information for all cell elements in a grouping grid. GridTableCellAppearance has
    /// an inheritance mechanism that allows child elements to inherit default settings from
    /// parent elements. GridTableCellAppearance lets you control almost any aspect of
    /// the appearance of the grouping grid such as cell backcolor, font, or the cell type.
    /// </summary>
    /// <remarks>
    /// GridTableCellAppearance supports both inheritance of style properties from parent elements and
    /// also inheritance of style settings within cell elements of one appearance object.
    /// <para/>
    /// Inheritance of style settings within cell elements of one appearance object has higher precedence
    /// than inheritance of style properties from parent elements.
    /// <para/>
    /// Examples for inheritance of style properties from parent elements are:<para/>
    /// <list type="bullet">
    /// <item><term>RecordField inherits from GridColumnDescriptor.Appearance and GridRecord.Appearance.</term></item>
    /// <item><term>GridRecord.Appearance inherits from ParentGroup.Appearance.</term></item>
    /// <item><term>ParentGroup.Appearance inherits from TableDescriptor.Appearance.</term></item>
    /// </list>
    /// Inheritance of style properties is defined by the <see cref="GridTableCellStyleInfoIdentity"/>
    /// object. It has a <see cref="GridTableCellStyleInfoIdentity.GetBaseStyles"/> method that returns
    /// the <see cref="GridStyleInfo"/> objects that form an inheritance chain. Check the
    /// <see cref="GridTableCellStyleInfoIdentity.GetBaseStyleNames"/> method to get string / debug
    /// information about the inheritance chain for a specific element. Also, the designer will show
    /// this debug information about the inheritance chain in a ToolTip when you hover the mouse over
    /// a cell within the "Preview and Edit" window.
    /// <para/>
    /// Inheritance of style settings within cell elements of one appearance object is defined as follows:
    /// <para/>
    /// <list type="table">
    /// <listheader><term>TableCellType</term><description>Inherits from</description></listheader>
    /// <item><term><see cref="GridTableCellType.None"/></term><description>None</description></item>
    /// <item><term><see cref="GridTableCellType.AnyCell"/></term><description>None</description></item>
    /// <item><term><see cref="GridTableCellType.EmptyCell"/></term><description>AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.EmptySectionRowHeaderCell"/></term><description>AnyHeaderCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.AnyHeaderCell"/></term><description>AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.AnyIndentCell"/></term><description>AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.ColumnHeaderCell"/></term><description>AnyHeaderCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.ColumnHeaderWithFilterCell"/></term><description>ColumnHeaderCell, AnyHeaderCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.RowHeaderCell"/></term><description>AnyHeaderCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.TopLeftHeaderCell"/></term><description>AnyHeaderCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.RecordPlusMinusCell"/></term><description>AnyRecordFieldCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.AnyNestedTableCell"/></term><description>AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.NestedTableRowHeaderCell"/></term><description>AnyNestedTableCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.NestedTableIndentCell"/></term><description>AnyIndentCell, AnyNestedTableCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.NestedTableCell"/></term><description>AnyNestedTableCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.AnyRecordFieldCell"/></term><description>AnyHeaderCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.RecordFieldCell"/></term><description>AnyRecordFieldCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.RecordRowHeaderCell"/></term><description>RowHeaderCell, AnyHeaderCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.AlternateRecordFieldCell"/></term><description>AnyRecordFieldCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.AlternateRecordRowHeaderCell"/></term><description>RowHeaderCell, AnyHeaderCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.AddNewRecordFieldCell"/></term><description>AnyRecordFieldCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.AddNewRecordRowHeaderCell"/></term><description>RowHeaderCell, AnyHeaderCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.AnyGroupCell"/></term><description>AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.GroupIndentCell"/></term><description>AnyIndentCell, AnyGroupCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.GroupIndentTCell"/></term><description>GroupIndentCell, AnyIndentCell, AnyGroupCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.GroupIndentLCell"/></term><description>GroupIndentCell, AnyIndentCell, AnyGroupCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.GroupIndentICell"/></term><description>GroupIndentCell, AnyIndentCell, AnyGroupCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.GroupCaptionCell"/></term><description>AnyGroupCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.GroupCaptionSummaryCell"/></term><description>AnyRecordFieldCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.GroupCaptionPlusMinusCell"/></term><description>AnyGroupCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.GroupCaptionRowHeaderCell"/></term><description>RowHeaderCell, AnyHeaderCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.FilterBarCell"/></term><description>AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.FilterBarRowHeaderCell"/></term><description>RowHeaderCell, AnyHeaderCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.AnySummaryCell"/></term><description>AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.SummaryFieldCell"/></term><description>AnySummaryCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.SummaryFillRowCell"/></term><description>AnySummaryCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.SummaryTitleCell"/></term><description>AnySummaryCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.SummaryRowHeaderCell"/></term><description>RowHeaderCell, AnyHeaderCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.SummaryEmptyCell"/></term><description>AnySummaryCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.GroupHeaderSectionCell"/></term><description>AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.GroupFooterSectionCell"/></term><description>AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.GroupHeaderIndentCell"/></term><description>GroupIndentCell, AnyIndentCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.GroupFooterIndentCell"/></term><description>GroupIndentCell, AnyIndentCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.AnyPreviewCell"/></term><description>AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.RecordPreviewCell"/></term><description>AnyPreviewCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.GroupPreviewCell"/></term><description>AnyPreviewCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.GroupFooterRowHeaderCell"/></term><description>RowHeaderCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.GroupHeaderRowHeaderCell"/></term><description>RowHeaderCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.GroupPreviewRowHeaderCell"/></term><description>RowHeaderCell, AnyCell</description></item>
    /// <item><term><see cref="GridTableCellType.RecordPreviewRowHeaderCell"/></term><description>RowHeaderCell, AnyCell</description></item></list>
    /// </remarks>
    [TypeConverter(typeof(GridTableCellAppearanceTypeConverter))]
    public class GridTableCellAppearance :
        ICustomTypeDescriptor
#if ASPNET
        , IStateManager
#endif
, IDisposable
    {
        const int GridTableCellTypeMaxValue = (int)GridTableCellType.StackedHeaderCell;
        GridTableCellStyleInfo[] styles = null;
        IGridTableCellAppearanceSource owner;
        string[] propertyFilter = null;
#if ASPNET
        private bool trackingVS = false;
#endif
        #region Construct, Owner and Styles
        /// <overload>
        /// Initializes a new appearance object.
        /// </overload>
        /// <summary>
        /// Initializes a new appearance object.
        /// </summary>
        public GridTableCellAppearance()
        {
        }

        /// <summary>
        /// Initializes a new appearance object.
        /// </summary>
        /// <param name="owner">The parent element that hosts this appearance object and receives notification about changes.</param>
        public GridTableCellAppearance(IGridTableCellAppearanceSource owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Sets the owner.
        /// </summary>
        /// <param name="owner">The parent element that hosts this appearance object and receives notification about changes.</param>
        public void SetOwner(IGridTableCellAppearanceSource owner)
        {
            this.owner = owner;
        }

        /// <override/>
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return "Appearance";
        }

        /// <summary>
        /// Gets the parent element that hosts this appearance object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public IGridTableCellAppearanceSource Owner
        {
            get
            {
                return owner;
            }
        }

        GridTableCellStyleInfo[] Styles
        {
            get
            {
                if (styles == null)
                {
                    styles = new GridTableCellStyleInfo[GridTableCellTypeMaxValue + 1];
                }

                return styles;
            }
        }

        /// <summary>
        /// Initializes this object and copies properties from another object. <see cref="Changing"/>
        /// and <see cref="Changed"/> events are raised for every property that is modified. If both
        /// objects are equal, no events are raised.
        /// </summary>
        /// <param name="other">The source object.</param>
        public void InitializeFrom(GridTableCellAppearance other)
        {
            bool isChanged = false;

            if (styles == null && other.styles == null)
            {
                return;
            }

            for (int n = 0; n < Styles.Length; n++)
            {
                if (Styles[n] == null && other.Styles[n] == null)
                {
                    continue;
                }

                if (other.Styles[n] == null)
                {
                    styles[n] = null;
                    isChanged = true;
                }
                else if (this.Styles[n] == null)
                {
                    this.GetStyle((GridTableCellType)n).CopyFrom(other.Styles[n]);
                    isChanged = true;
                }
                else
                {
                    isChanged |= !this.Styles[n].Store.Equals(other.Styles[n].Store);
                    this.Styles[n].CopyFrom(other.Styles[n]);
                }
            }

            if (isChanged)
            {
                RaiseChanged(GridTableCellType.AnyCell, new DescriptorPropertyChangedEventArgs("InitalizeFrom"));
            }
        }

        /// <summary>
        /// Applies properties from another object. <see cref="Changing"/>
        /// and <see cref="Changed"/> events are raised for every property that is modified. If both
        /// objects are equal, no events are raised. All properties that are specified in the other appearance 
        /// will be applied using StyleModifyType.Override.
        /// </summary>
        /// <param name="other">The source object.</param>
        public void ModifyAppearance(GridTableCellAppearance other)
        {
            this.ModifyAppearance(other, StyleModifyType.Override);
        }

        /// <summary>
        /// Applies properties from another object. <see cref="Changing"/>
        /// and <see cref="Changed"/> events are raised for every property that is modified. If both
        /// objects are equal, no events are raised. Properties that are specified in the other appearance 
        /// will be applied using the specified StyleModifyType.
        /// </summary>
        /// <param name="other">The source object.</param>
        /// <param name="modifyType">The source style modify type.</param>
        public void ModifyAppearance(GridTableCellAppearance other, StyleModifyType modifyType)
        {
            bool isChanged = false;

            if (styles == null && other.styles == null)
            {
                return;
            }

            for (int n = 0; n < Styles.Length; n++)
            {
                if (Styles[n] == null && other.Styles[n] == null)
                {
                    continue;
                }

                if (other.Styles[n] == null)
                {
                    ////styles[n] = null;
                    ////isChanged = true;
                }
                else if (this.Styles[n] == null)
                {
                    this.GetStyle((GridTableCellType)n).CopyFrom(other.Styles[n]);
                    isChanged = true;
                }
                else
                {
                    this.Styles[n].ModifyStyle(other.Styles[n], modifyType);
                    isChanged |= this.Styles[n].IsChanged;
                }
            }

            if (isChanged)
            {
                RaiseChanged(GridTableCellType.AnyCell, new DescriptorPropertyChangedEventArgs("InitalizeFrom"));
            }
        }

        #endregion

        #region Generic Methods

        /// <summary>
        /// Determines if the <see cref="GridTableCellStyleInfo"/> for the specified <see cref="GridTableCellType"/>
        /// is modified.
        /// </summary>
        /// <param name="tableCellType">Specifies the cell type.</param>
        /// <returns>True if the <see cref="GridTableCellStyleInfo"/> is modified; False if all properties are default.</returns>
        public virtual bool IsModifiedStyle(GridTableCellType tableCellType)
        {
            int index = (int)tableCellType;
            return this.styles != null && this.styles[index] != null && !this.styles[index].IsEmpty;
        }

        /// <summary>
        /// Resets all properties in the <see cref="GridTableCellStyleInfo"/> for the specified <see cref="GridTableCellType"/>
        /// back to default values.
        /// </summary>
        /// <param name="tableCellType">Specifies the cell type.</param>
        public virtual void ResetStyle(GridTableCellType tableCellType)
        {
            if (IsModifiedStyle(tableCellType))
            {
                RaiseChanging(tableCellType, new DescriptorPropertyChangedEventArgs("Reset"));
                int index = (int)tableCellType;
                this.styles[index] = null;
                RaiseChanged(tableCellType, new DescriptorPropertyChangedEventArgs("Reset"));
            }
        }

        bool getStyleReturnsNullStyleIfEmpty = false;

        private GridTableCellStyleInfo GetStyle(GridTableCellType tableCellType, GridStyleInfoStore store)
        {
            if (getStyleReturnsNullStyleIfEmpty && !this.IsModifiedStyle(tableCellType))
            {
                return null;
            }

            int index = (int)tableCellType;
            GridTableCellStyleInfo style = Styles[index];
            if (style == null)
            {
                if (store == null)
                {
                    style = new GridTableCellStyleInfo(new GridTableCellAppearanceStyleInfoIdentity(owner != null ? owner.Engine : null, this, tableCellType));
                }
                else
                {
                    style = new GridTableCellStyleInfo(new GridTableCellAppearanceStyleInfoIdentity(owner != null ? owner.Engine : null, this, tableCellType), store);
                }
                ////style.Changed += new StyleChangedEventHandler(style_Changed);
                styles[index] = style;
            }

            return style;
        }

        /// <summary>
        /// Returns a reference to the <see cref="GridTableCellStyleInfo"/> for the specified <see cref="GridTableCellType"/>.
        /// </summary>
        /// <param name="tableCellType">Specifies the cell type.</param>
        /// <returns>The <see cref="GridTableCellStyleInfo"/> for the specified <see cref="GridTableCellType"/>.</returns>
        public virtual GridTableCellStyleInfo GetStyle(GridTableCellType tableCellType)
        {
            return this.GetStyle(tableCellType, null);

            ////            switch (tableCellType)
            ////            {
            ////                case GridTableCellType.AnyCell: return AnyCell;
            ////                case GridTableCellType.EmptyCell: return EmptyCell;
            ////                case GridTableCellType.AnyHeaderCell: return AnyHeaderCell;
            ////                case GridTableCellType.ColumnHeaderCell: return ColumnHeaderCell;
            ////                case GridTableCellType.StackedHeaderCell: return StackedHeaderCell;
            ////                case GridTableCellType.RowHeaderCell: return RowHeaderCell;
            ////                case GridTableCellType.RecordPlusMinusCell: return RecordPlusMinusCell;
            ////                case GridTableCellType.AnyNestedTableCell: return AnyNestedTableCell;
            ////                case GridTableCellType.NestedTableRowHeaderCell: return NestedTableRowHeaderCell;
            ////                case GridTableCellType.NestedTableIndentCell: return NestedTableIndentCell;
            ////                case GridTableCellType.NestedTableCell: return NestedTableCell;
            ////                case GridTableCellType.AnyRecordFieldCell: return AnyRecordFieldCell;
            ////                case GridTableCellType.RecordFieldCell: return RecordFieldCell;
            ////                case GridTableCellType.AlternateRecordFieldCell: return AlternateRecordFieldCell;
            ////                case GridTableCellType.AddNewRecordFieldCell: return AddNewRecordFieldCell;
            ////                case GridTableCellType.AnyGroupCell: return AnyGroupCell;
            ////                case GridTableCellType.GroupIndentCell: return GroupIndentCell;
            ////                case GridTableCellType.GroupCaptionCell: return GroupCaptionCell;
            ////                case GridTableCellType.GroupCaptionPlusMinusCell: return GroupCaptionPlusMinusCell;
            ////                case GridTableCellType.FilterBarCell: return FilterBarCell;
            ////                case GridTableCellType.AnySummaryCell: return AnySummaryCell;
            ////                case GridTableCellType.SummaryFieldCell: return SummaryFieldCell;
            ////                case GridTableCellType.SummaryFillRowCell: return SummaryFillRowCell;
            ////                case GridTableCellType.SummaryTitleCell: return SummaryTitleCell;
            ////                case GridTableCellType.SummaryEmptyCell: return SummaryEmptyCell;
            ////                default:
            ////                    throw new ArgumentException("Invalid Enum");
            ////            }
        }

        #endregion

        #region Base Styles
        /// <overload>
        /// Returns an array of style objects representing the inheritance chain of style settings within cell elements of the appearance object for
        /// the specified table cell type.
        /// </overload>
        /// <summary>
        /// Returns an array of style objects representing the inheritance chain of style settings within cell elements of the appearance object for
        /// the specified table cell type.
        /// </summary>
        /// <param name="tableCellType">The table cell type.</param>
        /// <returns>An array of GridTableCellStyleInfo.</returns>
        public GridTableCellStyleInfo[] GetBaseStyles(GridTableCellType tableCellType)
        {
            return GetBaseStyles(tableCellType, false);
        }

        /// <summary>
        /// Returns an array of style objects representing the inheritance chain of style settings within cell elements of the appearance object for
        /// the specified table cell type.
        /// </summary>
        /// <param name="tableCellType">The table cell type.</param>
        /// <param name="nullStyleIfEmpty">Indicates if style objects that were not modified from their default settings should be included. If True, a NULL object
        /// is added to the array for default styles; otherwise the empty style object is added.</param>
        /// <returns>An array of GridTableCellStyleInfo.</returns>
        public virtual GridTableCellStyleInfo[] GetBaseStyles(GridTableCellType tableCellType, bool nullStyleIfEmpty)
        {
            bool savedNullStyleIfEmpty = this.getStyleReturnsNullStyleIfEmpty;
            this.getStyleReturnsNullStyleIfEmpty = nullStyleIfEmpty;
            try
            {
                switch (tableCellType)
                {
                    case GridTableCellType.None: break;
                    case GridTableCellType.AnyCell: break;
                    case GridTableCellType.EmptyCell: return new GridTableCellStyleInfo[] { AnyCell };
                    case GridTableCellType.EmptySectionRowHeaderCell: return new GridTableCellStyleInfo[] { AnyHeaderCell, AnyCell };
                    case GridTableCellType.AnyHeaderCell: return new GridTableCellStyleInfo[] { AnyCell };
                    case GridTableCellType.AnyIndentCell: return new GridTableCellStyleInfo[] { AnyCell };
                    case GridTableCellType.ColumnHeaderCell: return new GridTableCellStyleInfo[] { AnyHeaderCell, AnyCell };
                    case GridTableCellType.StackedHeaderCell: return new GridTableCellStyleInfo[] { ColumnHeaderCell, AnyHeaderCell, AnyCell };
                    case GridTableCellType.ColumnHeaderWithFilterCell: return new GridTableCellStyleInfo[] { ColumnHeaderCell, AnyHeaderCell, AnyCell };
                    case GridTableCellType.RowHeaderCell: return new GridTableCellStyleInfo[] { AnyHeaderCell, AnyCell };
                    case GridTableCellType.TopLeftHeaderCell: return new GridTableCellStyleInfo[] { AnyHeaderCell, AnyCell };
                    case GridTableCellType.RecordPlusMinusCell: return new GridTableCellStyleInfo[] { AnyRecordFieldCell, AnyCell };
                    case GridTableCellType.AnyNestedTableCell: return new GridTableCellStyleInfo[] { AnyCell };
                    case GridTableCellType.NestedTableRowHeaderCell: return new GridTableCellStyleInfo[] { AnyNestedTableCell, AnyCell };
                    case GridTableCellType.NestedTableIndentCell: return new GridTableCellStyleInfo[] { AnyIndentCell, AnyNestedTableCell, AnyCell };
                    case GridTableCellType.NestedTableIndentTCell: return new GridTableCellStyleInfo[] { NestedTableIndentCell, AnyIndentCell, AnyNestedTableCell, AnyCell };
                    case GridTableCellType.NestedTableIndentLCell: return new GridTableCellStyleInfo[] { NestedTableIndentCell, AnyIndentCell, AnyNestedTableCell, AnyCell };
                    case GridTableCellType.NestedTableIndentICell: return new GridTableCellStyleInfo[] { NestedTableIndentCell, AnyIndentCell, AnyNestedTableCell, AnyCell };
                    case GridTableCellType.NestedTableCell: return new GridTableCellStyleInfo[] { AnyNestedTableCell, AnyCell };
                    case GridTableCellType.AnyRecordFieldCell: return new GridTableCellStyleInfo[] { AnyCell };
                    case GridTableCellType.RecordFieldCell: return new GridTableCellStyleInfo[] { AnyRecordFieldCell, AnyCell };
                    case GridTableCellType.RecordRowHeaderCell: return new GridTableCellStyleInfo[] { RowHeaderCell, AnyHeaderCell, AnyCell };
                    case GridTableCellType.AlternateRecordFieldCell: return new GridTableCellStyleInfo[] { AnyRecordFieldCell, AnyCell };
                    case GridTableCellType.AlternateRecordRowHeaderCell: return new GridTableCellStyleInfo[] { RowHeaderCell, AnyHeaderCell, AnyCell };
                    case GridTableCellType.AddNewRecordFieldCell: return new GridTableCellStyleInfo[] { AlternateRecordFieldCell, AnyRecordFieldCell, AnyCell };
                    case GridTableCellType.AddNewRecordRowHeaderCell: return new GridTableCellStyleInfo[] { RowHeaderCell, AnyHeaderCell, AnyCell };
                    case GridTableCellType.AnyGroupCell: return new GridTableCellStyleInfo[] { AnyCell };
                    case GridTableCellType.GroupIndentCell: return new GridTableCellStyleInfo[] { AnyIndentCell, AnyGroupCell, AnyCell };
                    case GridTableCellType.GroupIndentTCell: return new GridTableCellStyleInfo[] { GroupIndentCell, AnyIndentCell, AnyGroupCell, AnyCell };
                    case GridTableCellType.GroupIndentLCell: return new GridTableCellStyleInfo[] { GroupIndentCell, AnyIndentCell, AnyGroupCell, AnyCell };
                    case GridTableCellType.GroupIndentICell: return new GridTableCellStyleInfo[] { GroupIndentCell, AnyIndentCell, AnyGroupCell, AnyCell };
                    case GridTableCellType.GroupCaptionCell: return new GridTableCellStyleInfo[] { AnyGroupCell, AnyCell };
                    case GridTableCellType.GroupCaptionPlusMinusCell: return new GridTableCellStyleInfo[] { AnyGroupCell, AnyCell };
                    case GridTableCellType.GroupCaptionRowHeaderCell: return new GridTableCellStyleInfo[] { RowHeaderCell, AnyHeaderCell, AnyCell };
                    case GridTableCellType.FilterBarCell: return new GridTableCellStyleInfo[] { AnyCell };
                    case GridTableCellType.FilterBarRowHeaderCell: return new GridTableCellStyleInfo[] { RowHeaderCell, AnyHeaderCell, AnyCell };
                    case GridTableCellType.AnySummaryCell: return new GridTableCellStyleInfo[] { AnyCell };
                    case GridTableCellType.SummaryFieldCell: return new GridTableCellStyleInfo[] { AnySummaryCell, AnyCell };
                    case GridTableCellType.SummaryFillRowCell: return new GridTableCellStyleInfo[] { AnySummaryCell, AnyCell };
                    case GridTableCellType.SummaryTitleCell: return new GridTableCellStyleInfo[] { AnySummaryCell, AnyCell };
                    case GridTableCellType.SummaryRowHeaderCell: return new GridTableCellStyleInfo[] { RowHeaderCell, AnyHeaderCell, AnyCell };
                    case GridTableCellType.SummaryEmptyCell: return new GridTableCellStyleInfo[] { AnySummaryCell, AnyCell };
                    case GridTableCellType.GroupCaptionSummaryCell: return new GridTableCellStyleInfo[] { AnyRecordFieldCell, AnyCell };
                    case GridTableCellType.GroupHeaderSectionCell: return new GridTableCellStyleInfo[] { AnyCell };
                    case GridTableCellType.GroupFooterSectionCell: return new GridTableCellStyleInfo[] { AnyCell };
                    case GridTableCellType.GroupHeaderIndentCell: return new GridTableCellStyleInfo[] { GroupIndentCell, AnyIndentCell, AnyCell };
                    case GridTableCellType.GroupFooterIndentCell: return new GridTableCellStyleInfo[] { GroupIndentCell, AnyIndentCell, AnyCell };
                    case GridTableCellType.AnyPreviewCell: return new GridTableCellStyleInfo[] { AnyCell };
                    case GridTableCellType.RecordPreviewCell: return new GridTableCellStyleInfo[] { AnyPreviewCell, AnyCell };
                    case GridTableCellType.GroupPreviewCell: return new GridTableCellStyleInfo[] { AnyPreviewCell, AnyCell };
                    case GridTableCellType.GroupFooterRowHeaderCell: return new GridTableCellStyleInfo[] { RowHeaderCell, AnyCell };
                    case GridTableCellType.GroupHeaderRowHeaderCell: return new GridTableCellStyleInfo[] { RowHeaderCell, AnyCell };
                    case GridTableCellType.GroupPreviewRowHeaderCell: return new GridTableCellStyleInfo[] { RowHeaderCell, AnyCell };
                    case GridTableCellType.RecordPreviewRowHeaderCell: return new GridTableCellStyleInfo[] { RowHeaderCell, AnyCell };
                    default:
                        Debug.Assert(false, "Unhandled GridTableCellType");
                        break;
                }

                return new GridTableCellStyleInfo[0];
            }
            finally
            {
                this.getStyleReturnsNullStyleIfEmpty = savedNullStyleIfEmpty;
            }
        }

        /// <summary>
        /// Gets a string that indicates parent styles within the appearance object for the specified table cell element.
        /// </summary>
        /// <param name="tableCellType">The table cell type.</param>
        /// <returns>A string with debug information.</returns>
        public static string[] GetBaseStyleNames(GridTableCellType tableCellType)
        {
            try
            {
                switch (tableCellType)
                {
                    case GridTableCellType.None: break;
                    case GridTableCellType.AnyCell: break;
                    case GridTableCellType.EmptyCell: return new string[] { "AnyCell" };
                    case GridTableCellType.EmptySectionRowHeaderCell: return new string[] { "AnyHeaderCell", "AnyCell" };
                    case GridTableCellType.AnyHeaderCell: return new string[] { "AnyCell" };
                    case GridTableCellType.AnyIndentCell: return new string[] { "AnyCell" };
                    case GridTableCellType.ColumnHeaderCell: return new string[] { "AnyHeaderCell", "AnyCell" };
                    case GridTableCellType.StackedHeaderCell: return new string[] { "ColumnHeaderCell", "AnyHeaderCell", "AnyCell" };
                    case GridTableCellType.ColumnHeaderWithFilterCell: return new string[] { "ColumnHeaderCell", "AnyHeaderCell", "AnyCell" };
                    case GridTableCellType.RowHeaderCell: return new string[] { "AnyHeaderCell", "AnyCell" };
                    case GridTableCellType.TopLeftHeaderCell: return new string[] { "AnyHeaderCell", "AnyCell" };
                    case GridTableCellType.RecordPlusMinusCell: return new string[] { "AnyRecordFieldCell", "AnyCell" };
                    case GridTableCellType.AnyNestedTableCell: return new string[] { "AnyCell" };
                    case GridTableCellType.NestedTableRowHeaderCell: return new string[] { "AnyNestedTableCell", "AnyCell" };
                    case GridTableCellType.NestedTableIndentCell: return new string[] { "AnyIndentCell", "AnyNestedTableCell", "AnyCell" };
                    case GridTableCellType.NestedTableCell: return new string[] { "AnyNestedTableCell", "AnyCell" };
                    case GridTableCellType.AnyRecordFieldCell: return new string[] { "AnyCell" };
                    case GridTableCellType.RecordFieldCell: return new string[] { "AnyRecordFieldCell", "AnyCell" };
                    case GridTableCellType.RecordRowHeaderCell: return new string[] { "RowHeaderCell", "AnyHeaderCell", "AnyCell" };
                    case GridTableCellType.AlternateRecordFieldCell: return new string[] { "AnyRecordFieldCell", "AnyCell" };
                    case GridTableCellType.AlternateRecordRowHeaderCell: return new string[] { "RowHeaderCell", "AnyHeaderCell", "AnyCell" };
                    case GridTableCellType.AddNewRecordFieldCell: return new string[] { "AnyRecordFieldCell", "AnyCell" };
                    case GridTableCellType.AddNewRecordRowHeaderCell: return new string[] { "RowHeaderCell", "AnyHeaderCell", "AnyCell" };
                    case GridTableCellType.AnyGroupCell: return new string[] { "AnyCell" };
                    case GridTableCellType.GroupIndentCell: return new string[] { "AnyIndentCell", "AnyGroupCell", "AnyCell" };
                    case GridTableCellType.GroupIndentTCell: return new string[] { "GroupIndentCell", "AnyIndentCell", "AnyGroupCell", "AnyCell" };
                    case GridTableCellType.GroupIndentLCell: return new string[] { "GroupIndentCell", "AnyIndentCell", "AnyGroupCell", "AnyCell" };
                    case GridTableCellType.GroupIndentICell: return new string[] { "GroupIndentCell", "AnyIndentCell", "AnyGroupCell", "AnyCell" };
                    case GridTableCellType.GroupCaptionCell: return new string[] { "AnyGroupCell", "AnyCell" };
                    case GridTableCellType.GroupCaptionPlusMinusCell: return new string[] { "AnyGroupCell", "AnyCell" };
                    case GridTableCellType.GroupCaptionSummaryCell: return new string[] { "AnyRecordFieldCell", "AnyCell" };
                    case GridTableCellType.GroupCaptionRowHeaderCell: return new string[] { "RowHeaderCell", "AnyHeaderCell", "AnyCell" };
                    case GridTableCellType.FilterBarCell: return new string[] { "AnyCell" };
                    case GridTableCellType.FilterBarRowHeaderCell: return new string[] { "RowHeaderCell", "AnyHeaderCell", "AnyCell" };
                    case GridTableCellType.AnySummaryCell: return new string[] { "AnyCell" };
                    case GridTableCellType.SummaryFieldCell: return new string[] { "AnySummaryCell", "AnyCell" };
                    case GridTableCellType.SummaryFillRowCell: return new string[] { "AnySummaryCell", "AnyCell" };
                    case GridTableCellType.SummaryTitleCell: return new string[] { "AnySummaryCell", "AnyCell" };
                    case GridTableCellType.SummaryRowHeaderCell: return new string[] { "RowHeaderCell", "AnyHeaderCell", "AnyCell" };
                    case GridTableCellType.SummaryEmptyCell: return new string[] { "AnySummaryCell", "AnyCell" };
                    case GridTableCellType.GroupHeaderSectionCell: return new string[] { "AnyCell" };
                    case GridTableCellType.GroupFooterSectionCell: return new string[] { "AnyCell" };
                    case GridTableCellType.GroupHeaderIndentCell: return new string[] { "GroupIndentCell", "AnyIndentCell", "AnyCell" };
                    case GridTableCellType.GroupFooterIndentCell: return new string[] { "GroupIndentCell", "AnyIndentCell", "AnyCell" };
                    case GridTableCellType.AnyPreviewCell: return new string[] { "AnyCell" };
                    case GridTableCellType.RecordPreviewCell: return new string[] { "AnyPreviewCell", "AnyCell" };
                    case GridTableCellType.GroupPreviewCell: return new string[] { "AnyPreviewCell", "AnyCell" };
                    case GridTableCellType.GroupFooterRowHeaderCell: return new string[] { "RowHeaderCell", "AnyCell" };
                    case GridTableCellType.GroupHeaderRowHeaderCell: return new string[] { "RowHeaderCell", "AnyCell" };
                    case GridTableCellType.GroupPreviewRowHeaderCell: return new string[] { "RowHeaderCell", "AnyCell" };
                    case GridTableCellType.RecordPreviewRowHeaderCell: return new string[] { "RowHeaderCell", "AnyCell" };
                }

                return new string[0];
            }
            finally
            {
            }
        }

        #endregion

        #region Public Properties

        bool isDefault = false;
        bool isReadOnly = false;
        bool isModified = false;

        /// <summary>
        /// Returns True if any style of the appearance object was modified; False if all are default.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsModified
        {
            get
            {
                return isModified;
            }
        }

        /// <summary>
        /// Resets all styles within the appearance object.
        /// </summary>
        public void Reset()
        {
            if (isReadOnly)
            {
                throw new InvalidOperationException("GridTableCellAppearance is Read-only. You must not change the GridCellAppearance.Default object; change the Engine.Appearance object instead.");
            }

            if (this.isModified)
            {
                RaiseChanging(GridTableCellType.AnyCell, new DescriptorPropertyChangedEventArgs("Reset"));
                this.styles = null;
                this.isDefault = false;
                RaiseChanged(GridTableCellType.AnyCell, new DescriptorPropertyChangedEventArgs("Reset"));
                this.isModified = false;
            }
        }

        /// <summary>
        /// Returns True if all styles of the appearance object are default; False if any are modified.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsDefault
        {
            get
            {
                return isDefault;
            }
        }

        /// <summary>
        /// Returns whether the object can be modified.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsReadOnly
        {
            get
            {
                return isReadOnly;
            }
        }

        #endregion

        #region Default

        /// <summary>
        /// Initializes the <see cref="Default"/> appearance. Any appearance will inherit default settings
        /// that were not initialized from the <see cref="Default"/> appearance.
        /// </summary>
        public virtual void InitializeDefault()
        {
            isDefault = true;

            GridTableCellStyleInfo style;

            //// Standard style
            style = AnyCell;
            //// style.Interior = engine.DefaultStyle.Intererior ... // new BrushInfo(SystemColors.Window);
            style.CheckBoxOptions.CheckedValue = "True";
            style.CheckBoxOptions.UncheckedValue = "False";
            //// style.Font.Facename = engine.DefaultStyle.Font "Tahoma";
            //// style.Font.Size = engine.DefaultStyle.Font  8;
#if ASPNET
            style.Font.Size = 0;
            style.Borders.All = new GridBorder(GridBorderStyle.NotSet,Color.Empty,GridBorderWeight.Thin);
#endif
            style.WrapText = true;
            style.AllowEnter = false;

            //// Headers
            style = AnyHeaderCell;
            style.Enabled = false;
            style.Font.Bold = false;
            style.Borders.All = new GridBorder(GridBorderStyle.None, SystemColors.WindowFrame, GridBorderWeight.Thin);

            style.Interior = new BrushInfo(SystemColors.Control);
            style.VerticalAlignment = GridVerticalAlignment.Middle;
            style.CellType = "Header";

            //// TopLeftHeaderCell
            style = this.TopLeftHeaderCell;
            style.Enabled = true;
            style.CellType = "RowHeaderCell";
            style.Enabled = false;

            //// Row Header
            style = RowHeaderCell;
            style.CellType = "RowHeaderCell";
            style.HorizontalAlignment = GridHorizontalAlignment.Left;
            style.Enabled = true;

            //// Column Header
            style = ColumnHeaderCell;
            style.CellType = "ColumnHeaderCell";
            style.HorizontalAlignment = GridHorizontalAlignment.Center;

            //// Stacked Header
            style = StackedHeaderCell;
            style.CellType = "StackedHeaderCell";

            //// Column Header With Filter
            style = ColumnHeaderWithFilterCell;
            style.CellType = "ColumnHeaderWithFilterCell";
            style.HorizontalAlignment = GridHorizontalAlignment.Center;

            //// Summaries
            style = AnySummaryCell;
            style.BackColor = SystemColors.Info;
            style.TextColor = SystemColors.InfoText;
            style.Enabled = false;
            style.CellType = "Static";
            style.Borders.Right = GridBorder.Empty;
            style.Borders.Top = new GridBorder(GridBorderStyle.Solid, Color.Blue, GridBorderWeight.Medium);

            //// Groups
            style = AnyGroupCell;
            style.TextColor = SystemColors.ControlText;

            //// Indent
            style = AnyIndentCell;
            style.Borders.Bottom = GridBorder.Empty;
            style.CellType = "Static";
            style.Enabled = false;

            //// Empty
            style = EmptyCell;
            style.Enabled = false;
            style.Borders.All = GridBorder.Empty;
            style.CellType = "Static";

            //// Group Caption
            style = GroupCaptionCell;
            ////style.Borders.All = GridBorder.Empty;
            style.Borders.Top = new GridBorder(GridBorderStyle.Solid, SystemColors.ControlDarkDark);
            ////style.Font.Bold = true;
            style.BackColor = SystemColors.Control;
            style.CellType = "Header";

            //// Group Caption PlusMinus
            style = GroupCaptionPlusMinusCell;
            style.Borders.Bottom = GridBorder.Empty;
            style.Borders.Top = new GridBorder(GridBorderStyle.Solid, SystemColors.Control);
            style.CellType = "PushButton";
            style.Enabled = true;
            style.HorizontalAlignment = GridHorizontalAlignment.Center;
            style.VerticalAlignment = GridVerticalAlignment.Middle;

            //// GroupCaptionSummaryCell
            style = GroupCaptionSummaryCell;
            style.CellType = "Static";

            //// Group Indent
            style = GroupIndentCell;
            style.CellType = "IndentCell";

            style = GroupIndentICell;
            style.CellValue = "I";

            style = GroupIndentTCell;
            style.CellValue = "T";

            style = GroupIndentLCell;
            style.CellValue = "L";

            //// FilterBar
            style = FilterBarCell;
            ////style.TextColor = Color.Blue;
            style.CellType = "FilterBarCell";

            //// Record PlusMinus
            style = RecordPlusMinusCell;
            style.Borders.Bottom = GridBorder.Empty;
            style.Borders.Right = GridBorder.Empty;
            style.CellType = "PushButton";
            style.Enabled = true;
            style.HorizontalAlignment = GridHorizontalAlignment.Center;
            style.VerticalAlignment = GridVerticalAlignment.Middle;

            //// Record Fields
            style = AnyRecordFieldCell;
            style.Enabled = true;
            style.CellType = "TextBox";
#if ASPNET
            style.TextMargins.Left = 4;
            style.TextMargins.Right = 4;
#endif
            //// Any Preview
            style = AnyPreviewCell;
            style.Font.Italic = true;
            style.TextColor = Color.Blue;
            style.Text = "Preview";
            style.Enabled = false;

            //// Nested Table
            style = NestedTableCell;
            style.Borders.All = GridBorder.Empty;

            //// Nested Table Indent
            style = NestedTableIndentCell;

            style = NestedTableIndentICell;
            style.CellValue = "I";

            style = NestedTableIndentTCell;
            style.CellValue = "T";

            style = NestedTableIndentLCell;
            style.CellValue = "L";

            //// NestedTable Row Header
            style = NestedTableRowHeaderCell;
            style.CellType = "Static";
            style.Borders.Bottom = GridBorder.Empty;
            style.Enabled = false;

            isReadOnly = true;
            isModified = false;
        }

        [ThreadStatic]
        static GridTableCellAppearance defaultCellAppearance;

        /// <summary>
        /// The <see cref="Default"/> appearance. Any appearance will inherit default settings
        /// that were not initialized from the <see cref="Default"/> appearance.
        /// </summary>
        public static GridTableCellAppearance Default
        {
            get
            {
                if (defaultCellAppearance == null)
                {
                    defaultCellAppearance = new GridTableCellAppearance(null);
                    defaultCellAppearance.InitializeDefault();
                }

                return defaultCellAppearance;
            }
        }

        #endregion

        #region Change Events

        internal void RaiseChanging(GridTableCellType tableCellType, EventArgs inner)
        {
            if (isReadOnly)
            {
                throw new InvalidOperationException("GridTableCellAppearance is Read-only. You must not change the GridCellAppearance.Default object; change the Engine.Appearance object instead.");
            }

            GridTableCellStyleInfoChangedEventArgs e = new GridTableCellStyleInfoChangedEventArgs(this, tableCellType, inner);
            OnChanging(e);

            if (owner != null)
            {
                owner.RaiseAppearanceChanging(e);
            }
        }

        internal void RaiseChanged(GridTableCellType tableCellType, EventArgs inner)
        {
            if (isReadOnly)
            {
                throw new InvalidOperationException("GridTableCellAppearance is Read-only. You must not change the GridCellAppearance.Default object; change the Engine.Appearance object instead.");
            }

            isModified = true;

            GridTableCellStyleInfoChangedEventArgs e = new GridTableCellStyleInfoChangedEventArgs(this, tableCellType, inner);
            OnChanged(e);

            if (owner != null)
            {
                owner.RaiseAppearanceChanged(e);
            }
        }

        /// <summary>
        /// Occurs after a style is changed.
        /// </summary>
        [Description("Occurs after a style is changed")]
        public event GridTableCellStyleInfoChangedEventHandler Changed;

        /// <summary>
        /// Occurs before a style is changed.
        /// </summary>
        [Description("Occurs before a style is changed")]
        public event GridTableCellStyleInfoChangedEventHandler Changing;

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridTableCellStyleInfoChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
            if (Changed != null)
            {
                Changed(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="Changing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridTableCellStyleInfoChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
            if (Changing != null)
            {
                Changing(this, e);
            }
        }

        #endregion

        #region ICustomTypeDescriptor Members

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
            return ((ICustomTypeDescriptor)this).GetProperties(null);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection pds = TypeDescriptor.GetProperties(this, attributes, true);
            if (PropertyFilter != null)
            {
                ArrayList pdc = new ArrayList();
                foreach (string prop in PropertyFilter)
                {
                    pdc.Add(pds[prop]);
                }

                return new PropertyDescriptorCollection((PropertyDescriptor[])pdc.ToArray(typeof(PropertyDescriptor)));
            }

            return pds;
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion

        #region Property Filter

        /// <summary>
        /// Defines styles to show in a propertygrid for the Apperance object of a
        /// <see cref="GridSummaryColumnDescriptor"/> and <see cref="GridSummaryRowDescriptor"/>.
        /// </summary>
        public static readonly string[] SummaryDescriptorPropertyFilter = new string[] 
            {
               "AnySummaryCell",
               "SummaryEmptyCell",
               "SummaryFieldCell",
               "SummaryFillRowCell",
               "SummaryRowHeaderCell",
               "SummaryTitleCell",
               "GroupCaptionSummaryCell",
            };

        /// <summary>
        /// Defines styles to show in a propertygrid for the Apperance object of a
        /// <see cref="GridColumnDescriptor"/>.
        /// </summary>
        public static readonly string[] ColumnDescriptorPropertyFilter = new string[] 
        {
                               "ColumnHeaderCell",
                               "ColumnHeaderWithFilterCell",
                               "AnyRecordFieldCell",
                               "RecordFieldCell",
                               "AlternateRecordFieldCell",
                               "AddNewRecordFieldCell",
                               "FilterBarCell",
                               "GroupCaptionSummaryCell",
        };

        /// <summary>
        /// Defines styles to show in a propertygrid for the Apperance object of a
        /// <see cref="GridStackedHeaderDescriptor"/>.
        /// </summary>
        public static readonly string[] StackedHeaderDescriptorPropertyFilter = new string[] 
        {
                               "StackedHeaderCell",
        };

        /// <summary>
        /// Defines styles to show in a propertygrid for the Apperance object of a
        /// <see cref="GridConditionalFormatDescriptor"/>.
        /// </summary>
        public static readonly string[] ConditionalFormatDescriptorPropertyFilter = new string[]
        {
                               "AnyRecordFieldCell",
                               "AnyHeaderCell",
                               "RecordRowHeaderCell",
                               "RecordPlusMinusCell",
                               "RecordPreviewCell",
                               "RecordPreviewRowHeaderCell",
         };

        /// <summary>
        /// Defines styles to show in a propertygrid for the GroupByApperance object of a
        /// <see cref="GridColumnDescriptor"/>.
        /// </summary>
        public static readonly string[] GroupByDescriptorPropertyFilter = new string[]
        {
                               "AnyIndentCell",
                               "ColumnHeaderCell",
                               "GroupCaptionCell",
                               "GroupCaptionPlusMinusCell",
                               "GroupCaptionRowHeaderCell",
                               "GroupFooterIndentCell",
                               "GroupFooterRowHeaderCell",
                               "GroupFooterSectionCell",
                               "GroupHeaderIndentCell",
                               "GroupHeaderRowHeaderCell",
                               "GroupHeaderSectionCell",
                               "GroupIndentCell",
                               "GroupPreviewCell",
                               "GroupPreviewRowHeaderCell",
                               "AnySummaryCell",
                               "StackedHeaderCell", 
                               "SummaryEmptyCell",
                               "SummaryFieldCell",
                               "SummaryFillRowCell",
                               "SummaryRowHeaderCell",
                               "SummaryTitleCell",
                               "GroupCaptionSummaryCell",
          };

        ////        public const string[] TableDescriptorPropertyFilter = {
        ////                                                      };

        /// <summary>
        /// Defines styles to show in a propertygrid for this appearance object at design-time.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public string[] PropertyFilter
        {
            get
            {
                return this.propertyFilter;
            }

            set
            {
                this.propertyFilter = value;
            }
        }

        #endregion

        #region Commented out IXmlSerializable
        /*
                /// <summary>
                /// Serializes the contents of this object into an XML stream.
                /// </summary>
                /// <param name="writer">Represents the XML stream.</param>
                public void WriteXml(XmlWriter writer)
                {
                    if (styles == null)
                        return;

                    for (int n = 0; n < styles.Length; n++)
                    {
                        if (styles[n] != null)
                        {
                            // <PropertyName>
                            writer.WriteStartElement(((GridTableCellType) n).ToString());

                            StyleInfoStore styleInfoStore = styles[n].Store;
                            if (styleInfoStore != null)
                                styleInfoStore.WriteXml(writer);
                            // XmlSerialize(writer, styleInfoStore, styleInfoStore.GetType(), false);

                            // <PropertyName/>
                            writer.WriteEndElement();
                        }
                    }
                }

                /// <summary>
                /// Not implemented and returns NULL.
                /// </summary>
                /// <returns></returns>
                XmlSchema IXmlSerializable.GetSchema()
                {
                    // TODO:  Add GetSchema implementation
                    return null;
                }

                /// <summary>
                /// Deserializes the contents of this object from an XML stream.
                /// </summary>
                /// <param name="reader">Represents the XML stream.</param>
                public void ReadXml(XmlReader reader)
                {
                }

        */
        #endregion

        #region Appearance Properties

        #region AnyCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// any cell in the grid.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of any cell in the grid.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo AnyCell
        {
            get
            {
                return GetStyle(GridTableCellType.AnyCell);
            }

            set
            {
                AnyCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="AnyCell"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAnyCell()
        {
            return IsModifiedStyle(GridTableCellType.AnyCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="AnyCell"/> object.
        /// </summary>
        public void ResetAnyCell()
        {
            ResetStyle(GridTableCellType.AnyCell);
        }
        #endregion

        #region RecordPreviewCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// cells in record preview row.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of cells in record preview row.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo RecordPreviewCell
        {
            get
            {
                return GetStyle(GridTableCellType.RecordPreviewCell);
            }

            set
            {
                RecordPreviewCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="RecordPreviewCell"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeRecordPreviewCell()
        {
            return IsModifiedStyle(GridTableCellType.RecordPreviewCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="RecordPreviewCell"/> object.
        /// </summary>
        public void ResetRecordPreviewCell()
        {
            ResetStyle(GridTableCellType.RecordPreviewCell);
        }
        #endregion

        #region GroupHeaderSectionCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// any cell in group header section.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of any cell in group header section.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo GroupHeaderSectionCell
        {
            get
            {
                return GetStyle(GridTableCellType.GroupHeaderSectionCell);
            }

            set
            {
                GroupHeaderSectionCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="GroupHeaderSectionCell"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeGroupHeaderSectionCell()
        {
            return IsModifiedStyle(GridTableCellType.GroupHeaderSectionCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="GroupHeaderSectionCell"/> object.
        /// </summary>
        public void ResetGroupHeaderSectionCell()
        {
            ResetStyle(GridTableCellType.GroupHeaderSectionCell);
        }
        #endregion

        #region GroupFooterSectionCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// any cell in the group footer section.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of any cell in group footer section")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo GroupFooterSectionCell
        {
            get
            {
                return GetStyle(GridTableCellType.GroupFooterSectionCell);
            }

            set
            {
                GroupFooterSectionCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="GroupFooterSectionCell"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeGroupFooterSectionCell()
        {
            return IsModifiedStyle(GridTableCellType.GroupFooterSectionCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="GroupFooterSectionCell"/> object.
        /// </summary>
        public void ResetGroupFooterSectionCell()
        {
            ResetStyle(GridTableCellType.GroupFooterSectionCell);
        }
        #endregion

        #region GroupHeaderIndentCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// indent cells in the group header section.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of indent cells in the group header section.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo GroupHeaderIndentCell
        {
            get
            {
                return GetStyle(GridTableCellType.GroupHeaderIndentCell);
            }

            set
            {
                GroupHeaderIndentCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="GroupHeaderIndentCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeGroupHeaderIndentCell()
        {
            return IsModifiedStyle(GridTableCellType.GroupHeaderIndentCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="GroupHeaderIndentCell"/> object.
        /// </summary>
        public void ResetGroupHeaderIndentCell()
        {
            ResetStyle(GridTableCellType.GroupHeaderIndentCell);
        }
        #endregion

        #region GroupFooterIndentCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// indent cells in the group footer section.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of indent cells in the group footer section.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo GroupFooterIndentCell
        {
            get
            {
                return GetStyle(GridTableCellType.GroupFooterIndentCell);
            }

            set
            {
                GroupFooterIndentCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="GroupFooterIndentCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeGroupFooterIndentCell()
        {
            return IsModifiedStyle(GridTableCellType.GroupFooterIndentCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="GroupFooterIndentCell"/> object.
        /// </summary>
        public void ResetGroupFooterIndentCell()
        {
            ResetStyle(GridTableCellType.GroupFooterIndentCell);
        }
        #endregion

        #region RecordPreviewRowHeaderCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// any header cell in a record preview row.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See the <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of any header cell in a record preview row.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo RecordPreviewRowHeaderCell
        {
            get
            {
                return GetStyle(GridTableCellType.RecordPreviewRowHeaderCell);
            }

            set
            {
                RecordPreviewRowHeaderCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="RecordPreviewRowHeaderCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeRecordPreviewRowHeaderCell()
        {
            return IsModifiedStyle(GridTableCellType.RecordPreviewRowHeaderCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="RecordPreviewRowHeaderCell"/> object.
        /// </summary>
        public void ResetRecordPreviewRowHeaderCell()
        {
            ResetStyle(GridTableCellType.RecordPreviewRowHeaderCell);
        }
        #endregion

        #region GroupPreviewRowHeaderCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// any header cell in a group preview section.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See the <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance for any header cell in a group preview section.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo GroupPreviewRowHeaderCell
        {
            get
            {
                return GetStyle(GridTableCellType.GroupPreviewRowHeaderCell);
            }

            set
            {
                GroupPreviewRowHeaderCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="GroupPreviewRowHeaderCell"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeGroupPreviewRowHeaderCell()
        {
            return IsModifiedStyle(GridTableCellType.GroupPreviewRowHeaderCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="GroupPreviewRowHeaderCell"/> object.
        /// </summary>
        public void ResetGroupPreviewRowHeaderCell()
        {
            ResetStyle(GridTableCellType.GroupPreviewRowHeaderCell);
        }
        #endregion

        #region GroupFooterRowHeaderCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// any header cell in a group footer.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance for any header cell in a group footer.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo GroupFooterRowHeaderCell
        {
            get
            {
                return GetStyle(GridTableCellType.GroupFooterRowHeaderCell);
            }

            set
            {
                GroupFooterRowHeaderCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="GroupFooterRowHeaderCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeGroupFooterRowHeaderCell()
        {
            return IsModifiedStyle(GridTableCellType.GroupFooterRowHeaderCell);
        }

        /// <summary>        
        /// Discards any changes for the <see cref="GroupFooterRowHeaderCell"/> object.
        /// </summary>
        public void ResetGroupFooterRowHeaderCell()
        {
            ResetStyle(GridTableCellType.GroupFooterRowHeaderCell);
        }
        #endregion

        #region GroupHeaderRowHeaderCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// any header cell in a group header.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance for any header cell in a group header.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo GroupHeaderRowHeaderCell
        {
            get
            {
                return GetStyle(GridTableCellType.GroupHeaderRowHeaderCell);
            }

            set
            {
                GroupHeaderRowHeaderCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="GroupHeaderRowHeaderCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeGroupHeaderRowHeaderCell()
        {
            return IsModifiedStyle(GridTableCellType.GroupHeaderRowHeaderCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="GroupHeaderRowHeaderCell"/> object.
        /// </summary>
        public void ResetGroupHeaderRowHeaderCell()
        {
            ResetStyle(GridTableCellType.GroupHeaderRowHeaderCell);
        }
        #endregion

        #region GroupPreviewCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// cells in group preview section.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance for cells in group preview section")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo GroupPreviewCell
        {
            get
            {
                return GetStyle(GridTableCellType.GroupPreviewCell);
            }

            set
            {
                GroupPreviewCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="GroupHeaderRowHeaderCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeGroupPreviewCell()
        {
            return IsModifiedStyle(GridTableCellType.GroupPreviewCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="GroupPreviewCell"/> object.
        /// </summary>
        public void ResetGroupPreviewCell()
        {
            ResetStyle(GridTableCellType.GroupPreviewCell);
        }
        #endregion

        #region AnyPreviewCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// any preview cell.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of any preview cell")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo AnyPreviewCell
        {
            get
            {
                return GetStyle(GridTableCellType.AnyPreviewCell);
            }

            set
            {
                AnyPreviewCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="AnyPreviewCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAnyPreviewCell()
        {
            return IsModifiedStyle(GridTableCellType.AnyPreviewCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="AnyPreviewCell"/> object.
        /// </summary>
        public void ResetAnyPreviewCell()
        {
            ResetStyle(GridTableCellType.AnyPreviewCell);
        }
        #endregion

        #region AnyHeaderCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// any header cell.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of any header cell")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo AnyHeaderCell
        {
            get
            {
                return GetStyle(GridTableCellType.AnyHeaderCell);
            }

            set
            {
                AnyHeaderCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="AnyHeaderCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAnyHeaderCell()
        {
            return IsModifiedStyle(GridTableCellType.AnyHeaderCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="AnyHeaderCell"/> object.
        /// </summary>
        public void ResetAnyHeaderCell()
        {
            ResetStyle(GridTableCellType.AnyHeaderCell);
        }
        #endregion

        #region AnyIndentCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// any indent cell.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of any indent cell")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo AnyIndentCell
        {
            get
            {
                return GetStyle(GridTableCellType.AnyIndentCell);
            }

            set
            {
                AnyIndentCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="AnyIndentCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>       
        public bool ShouldSerializeAnyIndentCell()
        {
            return IsModifiedStyle(GridTableCellType.AnyIndentCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="AnyIndentCell"/> object.
        /// </summary>
        public void ResetAnyIndentCell()
        {
            ResetStyle(GridTableCellType.AnyIndentCell);
        }
        #endregion

        #region EmptyCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// empty cell in summary rows.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of empty cells in  rows.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo EmptyCell
        {
            get
            {
                return GetStyle(GridTableCellType.EmptyCell);
            }

            set
            {
                EmptyCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="EmptyCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeEmptyCell()
        {
            return IsModifiedStyle(GridTableCellType.EmptyCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="EmptyCell"/> object.
        /// </summary>
        public void ResetEmptyCell()
        {
            ResetStyle(GridTableCellType.EmptyCell);
        }
        #endregion

        #region ColumnHeaderCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// column header cells.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of column header cells.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo ColumnHeaderCell
        {
            get
            {
                return GetStyle(GridTableCellType.ColumnHeaderCell);
            }

            set
            {
                ColumnHeaderCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="ColumnHeaderCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeColumnHeaderCell()
        {
            return IsModifiedStyle(GridTableCellType.ColumnHeaderCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="ColumnHeaderCell"/> object.
        /// </summary>
        public void ResetColumnHeaderCell()
        {
            ResetStyle(GridTableCellType.ColumnHeaderCell);
        }
        #endregion

        #region StackedHeaderCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// stacked header cells.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of column header cells.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo StackedHeaderCell
        {
            get
            {
                return GetStyle(GridTableCellType.StackedHeaderCell);
            }

            set
            {
                StackedHeaderCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="StackedHeaderCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeStackedHeaderCell()
        {
            return IsModifiedStyle(GridTableCellType.StackedHeaderCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="StackedHeaderCell"/> object.
        /// </summary>
        public void ResetStackedHeaderCell()
        {
            ResetStyle(GridTableCellType.StackedHeaderCell);
        }
        #endregion

        #region ColumnHeaderWithFilterCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// filter bar header cells.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of filter bar header cells.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo ColumnHeaderWithFilterCell
        {
            get
            {
                return GetStyle(GridTableCellType.ColumnHeaderWithFilterCell);
            }

            set
            {
                ColumnHeaderWithFilterCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="ColumnHeaderWithFilterCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeColumnHeaderWithFilterCell()
        {
            return IsModifiedStyle(GridTableCellType.ColumnHeaderWithFilterCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="ColumnHeaderWithFilterCell"/> object.
        /// </summary>
        public void ResetColumnHeaderWithFilterCell()
        {
            ResetStyle(GridTableCellType.ColumnHeaderWithFilterCell);
        }
        #endregion

        #region TopLeftHeaderCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// the top-left header cell.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of the top-left header cell.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo TopLeftHeaderCell
        {
            get
            {
                return GetStyle(GridTableCellType.TopLeftHeaderCell);
            }

            set
            {
                TopLeftHeaderCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="TopLeftHeaderCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeTopLeftHeaderCell()
        {
            return IsModifiedStyle(GridTableCellType.TopLeftHeaderCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="TopLeftHeaderCell"/> object.
        /// </summary>
        public void ResetTopLeftHeaderCell()
        {
            ResetStyle(GridTableCellType.TopLeftHeaderCell);
        }
        #endregion

        #region EmptySectionRowHeaderCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// row header cells in an empty section.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of row header cells in an empty section.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo EmptySectionRowHeaderCell
        {
            get
            {
                return GetStyle(GridTableCellType.EmptySectionRowHeaderCell);
            }

            set
            {
                EmptySectionRowHeaderCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="EmptySectionRowHeaderCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeEmptySectionRowHeaderCell()
        {
            return IsModifiedStyle(GridTableCellType.EmptySectionRowHeaderCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="EmptySectionRowHeaderCell"/> object.
        /// </summary>
        public void ResetEmptySectionRowHeaderCell()
        {
            ResetStyle(GridTableCellType.EmptySectionRowHeaderCell);
        }
        #endregion

        #region RecordRowHeaderCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// headers cell in non-alternate record rows.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of headers cell in non-alternate record rows.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo RecordRowHeaderCell
        {
            get
            {
                return GetStyle(GridTableCellType.RecordRowHeaderCell);
            }

            set
            {
                RecordRowHeaderCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="RecordRowHeaderCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeRecordRowHeaderCell()
        {
            return IsModifiedStyle(GridTableCellType.RecordRowHeaderCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="RecordRowHeaderCell"/> object.
        /// </summary>
        public void ResetRecordRowHeaderCell()
        {
            ResetStyle(GridTableCellType.RecordRowHeaderCell);
        }
        #endregion

        #region AddNewRecordRowHeaderCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// header cells in a new record row.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of header cells in a new record row.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo AddNewRecordRowHeaderCell
        {
            get
            {
                return GetStyle(GridTableCellType.AddNewRecordRowHeaderCell);
            }

            set
            {
                AddNewRecordRowHeaderCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="AddNewRecordRowHeaderCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAddNewRecordRowHeaderCell()
        {
            return IsModifiedStyle(GridTableCellType.AddNewRecordRowHeaderCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="AddNewRecordRowHeaderCell"/> object.
        /// </summary>
        public void ResetAddNewRecordRowHeaderCell()
        {
            ResetStyle(GridTableCellType.AddNewRecordRowHeaderCell);
        }
        #endregion

        #region AlternateRecordRowHeaderCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// header cells in alternate record rows.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of header cells in alternate record rows.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo AlternateRecordRowHeaderCell
        {
            get
            {
                return GetStyle(GridTableCellType.AlternateRecordRowHeaderCell);
            }

            set
            {
                AlternateRecordRowHeaderCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="AlternateRecordRowHeaderCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAlternateRecordRowHeaderCell()
        {
            return IsModifiedStyle(GridTableCellType.AlternateRecordRowHeaderCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="AlternateRecordRowHeaderCell"/> object.
        /// </summary>
        public void ResetAlternateRecordRowHeaderCell()
        {
            ResetStyle(GridTableCellType.AlternateRecordRowHeaderCell);
        }
        #endregion

        #region GroupCaptionRowHeaderCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// row header cells in the same row as a group caption.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of row header cells in the same row as a group caption.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo GroupCaptionRowHeaderCell
        {
            get
            {
                return GetStyle(GridTableCellType.GroupCaptionRowHeaderCell);
            }

            set
            {
                GroupCaptionRowHeaderCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="GroupCaptionRowHeaderCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeGroupCaptionRowHeaderCell()
        {
            return IsModifiedStyle(GridTableCellType.GroupCaptionRowHeaderCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="GroupCaptionRowHeaderCell"/> object.
        /// </summary>
        public void ResetGroupCaptionRowHeaderCell()
        {
            ResetStyle(GridTableCellType.GroupCaptionRowHeaderCell);
        }
        #endregion

        #region FilterBarRowHeaderCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// any filter bar row header cell.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of any filter bar row header cell")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo FilterBarRowHeaderCell
        {
            get
            {
                return GetStyle(GridTableCellType.FilterBarRowHeaderCell);
            }

            set
            {
                FilterBarRowHeaderCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="FilterBarRowHeaderCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeFilterBarRowHeaderCell()
        {
            return IsModifiedStyle(GridTableCellType.FilterBarRowHeaderCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="FilterBarRowHeaderCell"/> object.
        /// </summary>
        public void ResetFilterBarRowHeaderCell()
        {
            ResetStyle(GridTableCellType.FilterBarRowHeaderCell);
        }
        #endregion

        #region RowHeaderCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// row header cells in summary row.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of row header cells in summary row.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo RowHeaderCell
        {
            get
            {
                return GetStyle(GridTableCellType.RowHeaderCell);
            }

            set
            {
                RowHeaderCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="RowHeaderCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeRowHeaderCell()
        {
            return IsModifiedStyle(GridTableCellType.RowHeaderCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="RowHeaderCell"/> object.
        /// </summary>
        public void ResetRowHeaderCell()
        {
            ResetStyle(GridTableCellType.RowHeaderCell);
        }
        #endregion

        #region RecordPlusMinusCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// PlusMinus cells in a record row.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of PlusMinus cells in a record row.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo RecordPlusMinusCell
        {
            get
            {
                return GetStyle(GridTableCellType.RecordPlusMinusCell);
            }

            set
            {
                RecordPlusMinusCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="RecordPlusMinusCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeRecordPlusMinusCell()
        {
            return IsModifiedStyle(GridTableCellType.RecordPlusMinusCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="RecordPlusMinusCell"/> object.
        /// </summary>
        public void ResetRecordPlusMinusCell()
        {
            ResetStyle(GridTableCellType.RecordPlusMinusCell);
        }
        #endregion

        #region AnyNestedTableCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// any cells related to nested tables.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of any cells related to nested tables.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo AnyNestedTableCell
        {
            get
            {
                return GetStyle(GridTableCellType.AnyNestedTableCell);
            }

            set
            {
                AnyNestedTableCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="AnyNestedTableCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAnyNestedTableCell()
        {
            return IsModifiedStyle(GridTableCellType.AnyNestedTableCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="AnyNestedTableCell"/> object.
        /// </summary>
        public void ResetAnyNestedTableCell()
        {
            ResetStyle(GridTableCellType.AnyNestedTableCell);
        }
        #endregion

        #region NestedTableRowHeaderCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// row header cells in parent table for rows with indented nested tables.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of row header cells in parent table for rows with indented nested tables.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo NestedTableRowHeaderCell
        {
            get
            {
                return GetStyle(GridTableCellType.NestedTableRowHeaderCell);
            }

            set
            {
                NestedTableRowHeaderCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="NestedTableRowHeaderCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeNestedTableRowHeaderCell()
        {
            return IsModifiedStyle(GridTableCellType.NestedTableRowHeaderCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="NestedTableRowHeaderCell"/> object.
        /// </summary>
        public void ResetNestedTableRowHeaderCell()
        {
            ResetStyle(GridTableCellType.NestedTableRowHeaderCell);
        }
        #endregion

        #region NestedTableIndentCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// indent cells in parent table for rows with indented nested tables.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of indent cells in parent table for rows with indented nested tables.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo NestedTableIndentCell
        {
            get
            {
                return GetStyle(GridTableCellType.NestedTableIndentCell);
            }

            set
            {
                NestedTableIndentCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="NestedTableIndentCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeNestedTableIndentCell()
        {
            return IsModifiedStyle(GridTableCellType.NestedTableIndentCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="NestedTableIndentCell"/> object.
        /// </summary>
        public void ResetNestedTableIndentCell()
        {
            ResetStyle(GridTableCellType.NestedTableIndentCell);
        }
        #endregion

        #region NestedTableIndentLCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// indent cells with closing L-line in parent table for rows with indented nested tables.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance for indent cells with closing L-line parent table for rows with indented nested tables.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo NestedTableIndentLCell
        {
            get
            {
                return GetStyle(GridTableCellType.NestedTableIndentLCell);
            }

            set
            {
                NestedTableIndentLCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="NestedTableIndentLCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeNestedTableIndentLCell()
        {
            return IsModifiedStyle(GridTableCellType.NestedTableIndentLCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="NestedTableIndentLCell"/> object.
        /// </summary>
        public void ResetNestedTableIndentLCell()
        {
            ResetStyle(GridTableCellType.NestedTableIndentLCell);
        }
        #endregion

        #region NestedTableIndentTCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// indent cells with T-lines in parent table for rows with indented nested tables.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of indent cells with T-lines in parent table for rows with indented nested tables")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo NestedTableIndentTCell
        {
            get
            {
                return GetStyle(GridTableCellType.NestedTableIndentTCell);
            }

            set
            {
                NestedTableIndentTCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="NestedTableIndentTCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeNestedTableIndentTCell()
        {
            return IsModifiedStyle(GridTableCellType.NestedTableIndentTCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="NestedTableIndentTCell"/> object.
        /// </summary>
        public void ResetNestedTableIndentTCell()
        {
            ResetStyle(GridTableCellType.NestedTableIndentTCell);
        }
        #endregion

        #region NestedTableIndentICell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// indent cells with continuous I-line in parent table for rows with indented nested tables.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of indent cells with continuous I-line in parent table for rows with indented nested tables.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo NestedTableIndentICell
        {
            get
            {
                return GetStyle(GridTableCellType.NestedTableIndentICell);
            }

            set
            {
                NestedTableIndentICell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="NestedTableIndentICell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeNestedTableIndentICell()
        {
            return IsModifiedStyle(GridTableCellType.NestedTableIndentICell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="NestedTableIndentICell"/> object.
        /// </summary>
        public void ResetNestedTableIndentICell()
        {
            ResetStyle(GridTableCellType.NestedTableIndentICell);
        }
        #endregion

        #region NestedTableCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// nested table cells in which the child table is drawn.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of nested table cells in which the child table is drawn.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo NestedTableCell
        {
            get
            {
                return GetStyle(GridTableCellType.NestedTableCell);
            }

            set
            {
                NestedTableCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="NestedTableCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeNestedTableCell()
        {
            return IsModifiedStyle(GridTableCellType.NestedTableCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="NestedTableCell"/> object.
        /// </summary>
        public void ResetNestedTableCell()
        {
            ResetStyle(GridTableCellType.NestedTableCell);
        }
        #endregion

        #region AnyRecordFieldCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// any record field cell.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of any record field cell.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo AnyRecordFieldCell
        {
            get
            {
                return GetStyle(GridTableCellType.AnyRecordFieldCell);
            }

            set
            {
                AnyRecordFieldCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="AnyRecordFieldCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAnyRecordFieldCell()
        {
            return IsModifiedStyle(GridTableCellType.AnyRecordFieldCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="AnyRecordFieldCell"/> object.
        /// </summary>
        public void ResetAnyRecordFieldCell()
        {
            ResetStyle(GridTableCellType.AnyRecordFieldCell);
        }
        #endregion

        #region RecordFieldCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// field cells in the non-alternate record row.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of field cells in non-alternate record row.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo RecordFieldCell
        {
            get
            {
                return GetStyle(GridTableCellType.RecordFieldCell);
            }

            set
            {
                RecordFieldCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="RecordFieldCell"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeRecordFieldCell()
        {
            return IsModifiedStyle(GridTableCellType.RecordFieldCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="RecordFieldCell"/> object.
        /// </summary>
        public void ResetRecordFieldCell()
        {
            ResetStyle(GridTableCellType.RecordFieldCell);
        }
        #endregion

        #region AlternateRecordFieldCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// field cells in alternate record rows.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of field cells in alternate record rows.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo AlternateRecordFieldCell
        {
            get
            {
                return GetStyle(GridTableCellType.AlternateRecordFieldCell);
            }

            set
            {
                AlternateRecordFieldCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="AlternateRecordFieldCell"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeAlternateRecordFieldCell()
        {
            return IsModifiedStyle(GridTableCellType.AlternateRecordFieldCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="AlternateRecordFieldCell"/> object.
        /// </summary>
        public void ResetAlternateRecordFieldCell()
        {
            ResetStyle(GridTableCellType.AlternateRecordFieldCell);
        }
        #endregion

        #region AddNewRecordFieldCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// field cells in new record row.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
        [Description("Contains options for changing the appearance of field cells in new record row.")]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo AddNewRecordFieldCell
        {
            get
            {
                return GetStyle(GridTableCellType.AddNewRecordFieldCell);
            }

            set
            {
                AddNewRecordFieldCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="AddNewRecordFieldCell"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeAddNewRecordFieldCell()
        {
            return IsModifiedStyle(GridTableCellType.AddNewRecordFieldCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="AddNewRecordFieldCell"/> object.
        /// </summary>
        public void ResetAddNewRecordFieldCell()
        {
            ResetStyle(GridTableCellType.AddNewRecordFieldCell);
        }
        #endregion

        #region AnyGroupCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// any group-related cell.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of any group-related cell.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo AnyGroupCell
        {
            get
            {
                return GetStyle(GridTableCellType.AnyGroupCell);
            }

            set
            {
                AnyGroupCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="AnyGroupCell"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeAnyGroupCell()
        {
            return IsModifiedStyle(GridTableCellType.AnyGroupCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="AnyGroupCell"/> object.
        /// </summary>
        public void ResetAnyGroupCell()
        {
            ResetStyle(GridTableCellType.AnyGroupCell);
        }
        #endregion

        #region GroupIndentCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// indent cells in groups.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of indent cells in groups.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo GroupIndentCell
        {
            get
            {
                return GetStyle(GridTableCellType.GroupIndentCell);
            }

            set
            {
                GroupIndentCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="GroupIndentCell"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeGroupIndentCell()
        {
            return IsModifiedStyle(GridTableCellType.GroupIndentCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="GroupIndentCell"/> object.
        /// </summary>
        public void ResetGroupIndentCell()
        {
            ResetStyle(GridTableCellType.GroupIndentCell);
        }
        #endregion

        #region GroupIndentLCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// indent cells with closing L-line in groups.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of indent cells with closing L-line in groups.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo GroupIndentLCell
        {
            get
            {
                return GetStyle(GridTableCellType.GroupIndentLCell);
            }

            set
            {
                GroupIndentLCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="GroupIndentLCell"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeGroupIndentLCell()
        {
            return IsModifiedStyle(GridTableCellType.GroupIndentLCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="GroupIndentLCell"/> object.
        /// </summary>
        public void ResetGroupIndentLCell()
        {
            ResetStyle(GridTableCellType.GroupIndentLCell);
        }
        #endregion

        #region GroupIndentTCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// indent cells with T-lines in groups.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of indent cells with T-lines in groups.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo GroupIndentTCell
        {
            get
            {
                return GetStyle(GridTableCellType.GroupIndentTCell);
            }

            set
            {
                GroupIndentTCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="GroupIndentTCell"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeGroupIndentTCell()
        {
            return IsModifiedStyle(GridTableCellType.GroupIndentTCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="GroupIndentTCell"/> object.
        /// </summary>
        public void ResetGroupIndentTCell()
        {
            ResetStyle(GridTableCellType.GroupIndentTCell);
        }
        #endregion

        #region GroupIndentICell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// indent cells with continuous I-lines in groups.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of indent cells with continuous I-lines in groups.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo GroupIndentICell
        {
            get
            {
                return GetStyle(GridTableCellType.GroupIndentICell);
            }

            set
            {
                GroupIndentICell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="GroupIndentICell"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeGroupIndentICell()
        {
            return IsModifiedStyle(GridTableCellType.GroupIndentICell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="GroupIndentICell"/> object.
        /// </summary>
        public void ResetGroupIndentICell()
        {
            ResetStyle(GridTableCellType.GroupIndentICell);
        }
        #endregion

        #region GroupCaptionCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// group caption cells.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of group caption cells.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo GroupCaptionCell
        {
            get
            {
                return GetStyle(GridTableCellType.GroupCaptionCell);
            }

            set
            {
                GroupCaptionCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="GroupCaptionCell"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeGroupCaptionCell()
        {
            return IsModifiedStyle(GridTableCellType.GroupCaptionCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="GroupCaptionCell"/> object.
        /// </summary>
        public void ResetGroupCaptionCell()
        {
            ResetStyle(GridTableCellType.GroupCaptionCell);
        }
        #endregion

        #region GroupCaptionPlusMinusCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// the PlusMinus cell in group captions.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See the <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of the PlusMinus cell in group captions.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo GroupCaptionPlusMinusCell
        {
            get
            {
                return GetStyle(GridTableCellType.GroupCaptionPlusMinusCell);
            }

            set
            {
                GroupCaptionPlusMinusCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="GroupCaptionPlusMinusCell"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeGroupCaptionPlusMinusCell()
        {
            return IsModifiedStyle(GridTableCellType.GroupCaptionPlusMinusCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="GroupCaptionPlusMinusCell"/> object.
        /// </summary>
        public void ResetGroupCaptionPlusMinusCell()
        {
            ResetStyle(GridTableCellType.GroupCaptionPlusMinusCell);
        }
        #endregion

        #region FilterBarCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// field cells in filter bars.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of field cells in filter bars.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo FilterBarCell
        {
            get
            {
                return GetStyle(GridTableCellType.FilterBarCell);
            }

            set
            {
                FilterBarCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="FilterBarCell"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeFilterBarCell()
        {
            return IsModifiedStyle(GridTableCellType.FilterBarCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="FilterBarCell"/> object.
        /// </summary>
        public void ResetFilterBarCell()
        {
            ResetStyle(GridTableCellType.FilterBarCell);
        }
        #endregion

        #region AnySummaryCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// any summary cell.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of any summary cell.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo AnySummaryCell
        {
            get
            {
                return GetStyle(GridTableCellType.AnySummaryCell);
            }

            set
            {
                AnySummaryCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="AnySummaryCell"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeAnySummaryCell()
        {
            return IsModifiedStyle(GridTableCellType.AnySummaryCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="AnySummaryCell"/> object.
        /// </summary>
        public void ResetAnySummaryCell()
        {
            ResetStyle(GridTableCellType.AnySummaryCell);
        }
        #endregion

        #region SummaryFieldCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// field cells in summary rows.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of field cells in summary rows.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo SummaryFieldCell
        {
            get
            {
                return GetStyle(GridTableCellType.SummaryFieldCell);
            }

            set
            {
                SummaryFieldCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="SummaryFieldCell"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeSummaryFieldCell()
        {
            return IsModifiedStyle(GridTableCellType.SummaryFieldCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="SummaryFieldCell"/> object.
        /// </summary>
        public void ResetSummaryFieldCell()
        {
            ResetStyle(GridTableCellType.SummaryFieldCell);
        }
        #endregion

        #region SummaryFillRowCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// field cells in summary rows with GridSummaryStyle.FillRows.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of field cells in summary rows with GridSummaryStyle.FillRows.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo SummaryFillRowCell
        {
            get
            {
                return GetStyle(GridTableCellType.SummaryFillRowCell);
            }

            set
            {
                SummaryFillRowCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="SummaryFillRowCell"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeSummaryFillRowCell()
        {
            return IsModifiedStyle(GridTableCellType.SummaryFillRowCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="SummaryFillRowCell"/> object.
        /// </summary>
        public void ResetSummaryFillRowCell()
        {
            ResetStyle(GridTableCellType.SummaryFillRowCell);
        }
        #endregion

        #region SummaryTitleCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// the title cell in summary rows.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of the title cell in summary rows.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo SummaryTitleCell
        {
            get
            {
                return GetStyle(GridTableCellType.SummaryTitleCell);
            }

            set
            {
                SummaryTitleCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="SummaryTitleCell"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeSummaryTitleCell()
        {
            return IsModifiedStyle(GridTableCellType.SummaryTitleCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="SummaryTitleCell"/> object.
        /// </summary>
        public void ResetSummaryTitleCell()
        {
            ResetStyle(GridTableCellType.SummaryTitleCell);
        }
        #endregion

        #region SummaryRowHeaderCell

        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// row header cells in summary rows.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of row header cells in summary rows.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo SummaryRowHeaderCell
        {
            get
            {
                return GetStyle(GridTableCellType.SummaryRowHeaderCell);
            }

            set
            {
                SummaryRowHeaderCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="SummaryRowHeaderCell"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeSummaryRowHeaderCell()
        {
            return IsModifiedStyle(GridTableCellType.SummaryRowHeaderCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="SummaryRowHeaderCell"/> object.
        /// </summary>
        public void ResetSummaryRowHeaderCell()
        {
            ResetStyle(GridTableCellType.SummaryRowHeaderCell);
        }
        #endregion

        #region SummaryEmptyCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// empty cells in summary rows.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of empty cells in summary rows")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo SummaryEmptyCell
        {
            get
            {
                return GetStyle(GridTableCellType.SummaryEmptyCell);
            }

            set
            {
                SummaryEmptyCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="SummaryEmptyCell"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeSummaryEmptyCell()
        {
            return IsModifiedStyle(GridTableCellType.SummaryEmptyCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="SummaryEmptyCell"/> object.
        /// </summary>
        public void ResetSummaryEmptyCell()
        {
            ResetStyle(GridTableCellType.SummaryEmptyCell);
        }
        #endregion
        #region GroupCaptionSummaryCell
        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> with cell settings for
        /// summary cells in the caption bar.
        /// </summary>
        /// <remarks>
        /// When you assign a new <see cref="GridTableCellStyleInfo"/> object, the setter will
        /// not assign a reference to the object. Instead all properties of the new style object
        /// will be copied over to the existing style object.
        /// <para/>
        /// See <see cref="GridTableCellAppearance"/> overview for information on inheritance of
        /// style settings.
        /// </remarks>
        [Description("Contains options for changing the appearance of summary cells in the caption bar.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellStyleInfo GroupCaptionSummaryCell
        {
            get
            {
                return GetStyle(GridTableCellType.GroupCaptionSummaryCell);
            }

            set
            {
                GroupCaptionSummaryCell.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="GroupCaptionSummaryCell"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeGroupCaptionSummaryCell()
        {
            return IsModifiedStyle(GridTableCellType.GroupCaptionSummaryCell);
        }

        /// <summary>
        /// Discards any changes for the <see cref="GroupCaptionSummaryCell"/> object.
        /// </summary>
        public void ResetGroupCaptionSummaryCell()
        {
            ResetStyle(GridTableCellType.GroupCaptionSummaryCell);
        }
        #endregion

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes of the object and inner style object.
        /// </summary>
        public void Dispose()
        {
            owner = null;
            if (styles != null)
            {
                this.ClearStyles(this.styles);
                styles = null;
            }

            GC.SuppressFinalize(this);
        }

        private void ClearStyles(GridTableCellStyleInfo[] styles)
        {
            foreach (GridTableCellStyleInfo style in styles)
            {
                if (style != null)
                {
                    style.Dispose();
                }
            }
        }

        #endregion
#if ASPNET
        #region IStateManager Impl.
        bool IStateManager.IsTrackingViewState
        {
            get{return this.trackingVS;}
        }
        void IStateManager.LoadViewState(object state)
        {
            if(state != null)
            {
                StateBag viewState = new StateBag();
                ((IStateManager)viewState).LoadViewState(state);
                GridStyleInfoStore[] stores = viewState["Styles"] as GridStyleInfoStore[];
                if(this.styles != null)
                    this.ClearStyles(this.styles);
                this.styles = new GridTableCellStyleInfo[GridTableCellTypeMaxValue+1];
                for(int i = 0; i < stores.Length; i++)
                {
                    if(stores[i] != null)
                        this.styles[i] = this.GetStyle((GridTableCellType)i, stores[i]);
                }
                viewState.Clear();
                //TODO_PRAVEEN: Should I fire an event?
                //Changed();
            }
        }
        void IStateManager.TrackViewState()
        {
        }
        object IStateManager.SaveViewState()
        {
            if(this.styles != null)
            {
                StateBag viewState = new StateBag();
                ((IStateManager)viewState).TrackViewState();
                GridStyleInfoStore[] stores = new GridStyleInfoStore[this.styles.Length];
                for(int i = 0; i < this.styles.Length; i++)
                {
                    if(styles[i] != null)
                        stores[i] = styles[i].Store;
                }
                viewState["Styles"] = stores;
                return ((IStateManager)viewState).SaveViewState();
            }
            return null;
        }
        #endregion // IStateManager Impl.
#endif
    }
}