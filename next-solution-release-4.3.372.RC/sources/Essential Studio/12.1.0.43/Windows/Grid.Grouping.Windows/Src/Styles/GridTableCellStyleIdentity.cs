//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableCellStyleIdentity.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
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

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;

using Syncfusion.Grouping;

#if ASPNET
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// Provides identity information for a <see cref="GridTableCellStyleInfo"/> object that is
    /// created by <see cref="GridTable"/> for each cell in the grid. GridTableCellStyleInfoIdentity
    /// defines the inheritance of style properties from parent elements inside the grid.
    /// </summary>
    /// <remarks>
    /// Inheritance of style properties is defined by the <see cref="GridTableCellStyleInfoIdentity"/>
    /// object. It has a <see cref="GridTableCellStyleInfoIdentity.GetBaseStyles"/> method that returns
    /// the <see cref="GridStyleInfo"/> objects that form an inheritance chain. Check the
    /// <see cref="GridTableCellStyleInfoIdentity.GetBaseStyleNames"/> method to get string / debug
    /// information about the inheritance chain for a specific element. Also, the designer will show
    /// this debug information about the inheritance chain in a ToolTip when you hover the mouse over
    /// a cell within the "Preview and Edit" window.
    /// </remarks>
    public class GridTableCellStyleInfoIdentity : GridStyleInfoIdentity, IGridModelSource
    {
#if WEAKREF
        // Cache
        WeakReference __cachedBaseStyles;

        IStyleInfo[] cachedBaseStyles
        {
            get
            {
                if (__cachedBaseStyles != null)
                    return (IStyleInfo[]) __cachedBaseStyles.Target;
                return null;
            }
            set
            {
                if (value != null)
                    __cachedBaseStyles = new WeakReference(value);
                else
                    __cachedBaseStyles = null;
            }
        }

        ~GridTableCellStyleInfoIdentity()
        {
            if (displayElement is IGridTableCellStyleInfoWeakReferences)
                ((IGridTableCellStyleInfoWeakReferences) displayElement).FinalizingTableCellStyleInfoIdentity();
        }
#else
        IStyleInfo[] cachedBaseStyles;
#endif
        //// Identity properties
        GridTable table;
        GridTableCellType tableCellType;
        Element displayElement;  // Record, Caption etc.
        GridColumnDescriptor column;
        GridStackedHeaderDescriptor stackedHeader;
        GridSummaryColumnDescriptor summaryColumn;
        SummaryDescriptor filterBarSummaryDescriptor;
        // FilterColumnDescriptor filterColumn;
        int version;

        private SortColumnDescriptor groupedColumn;

        ////        public override bool Equals(object obj)
        ////        {
        ////            if (obj == null)
        ////                return this == null;
        ////
        ////            if (!(obj is GridTableCellStyleInfoIdentity) || this == null)
        ////                return false;
        ////
        ////            GridTableCellStyleInfoIdentity other = (GridTableCellStyleInfoIdentity) obj;
        ////
        ////            return tableCellType == other.tableCellType;
        ////                && EqualsObject(table, other.table)
        ////                && EqualsObject(displayElement, other.displayElement)
        ////                && EqualsObject(column, other.column)
        ////                && EqualsObject(summaryColumn, other.summaryColumn)
        ////                && EqualsObject(groupedColumn, other.groupedColumn);
        ////        }
        ////
        ////        public override int GetHashCode()
        ////        {
        ////            return base.GetHashCode ();
        ////        }

        /// <summary>
        /// Compares two objects if they are equal. Works also with NULL references.
        /// </summary>
        /// <param name="obj1">The first object to compare.</param>
        /// <param name="obj2">The second object to compare.</param>
        /// <returns>True if both objects are equal.</returns>
        static bool EqualsObject(object obj1, object obj2)
        {
            return obj1 == obj2
                || (obj1 != null && obj2 != null && obj1.Equals(obj2));
        }

        /// <summary>
        /// The <see cref="SortColumnDescriptor"/> for the grouped column, if this cell is associated with a
        /// column that is grouped by, e.g. if this is a caption cell or an indent cell.
        /// </summary>
        public SortColumnDescriptor GroupedColumn
        {
            get
            {
                return this.groupedColumn;
            }

            set
            {
                this.groupedColumn = value;
            }
        }

        /// <summary>
        /// The <see cref="GridTable"/> that created this object.
        /// </summary>
        public GridTable Table
        {
            get
            {
                return table;
            }

            set
            {
                table = value;
            }
        }

        /// <summary>
        /// The <see cref="GridTableCellType"/> that specifies what kind of cell this is.
        /// </summary>
        public GridTableCellType TableCellType
        {
            get
            {
                return tableCellType;
            }

            set
            {
                tableCellType = value;
            }
        }

        /// <summary>
        /// The display element displayed at the row of this cell.
        /// </summary>
        public Element DisplayElement
        {
            get
            {
                return displayElement;
            }

            set
            {
                displayElement = value;
            }
        }

        /// <summary>
        /// The column this cell displays if it is RecordFieldCell, column header cell, or any cell associated with a
        /// <see cref="GridColumnDescriptor"/>.
        /// </summary>
        public GridColumnDescriptor Column
        {
            get
            {
                return column;
            }

            set
            {
                column = value;
            }
        }

        /// <summary>
        /// The stacked header this cell displays if it is StackedHeaderCell
        /// </summary>
        public GridStackedHeaderDescriptor StackedHeader
        {
            get
            {
                return stackedHeader;
            }

            set
            {
                stackedHeader = value;
            }
        }

        /// <summary>
        /// The summary this cell displays if it is Summary cell or any cell associated with a
        /// <see cref="GridSummaryColumnDescriptor"/>.
        /// </summary>
        public GridSummaryColumnDescriptor SummaryColumn
        {
            get
            {
                return summaryColumn;
            }

            set
            {
                summaryColumn = value;
            }
        }

        /// <summary>
        /// The summary this cell displays if it is Summary cell or any cell associated with a
        /// <see cref="GridSummaryColumnDescriptor"/>.
        /// </summary>
        public SummaryDescriptor FilterBarSummaryDescriptor
        {
            get
            {
                return filterBarSummaryDescriptor;
            }

            set
            {
                filterBarSummaryDescriptor = value;
            }
        }

        /// <override/>
        /// <summary>Disposes the current object.</summary>
        public override void Dispose()
        {
            cachedBaseStyles = null;
            base.Dispose();
        }

        /// <summary>
        /// Initializes the identity object with a reference to the volatile data cache, row, and column index.
        /// </summary>
        /// <param name="data">Reference to the volatile data cache.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public GridTableCellStyleInfoIdentity(IGridData data, int rowIndex, int colIndex)
            : base(data, rowIndex, colIndex, true)
        {
            //// Always set GridStyleInfoIdentity.Identity = true since GridTableModelVolatileData.ResetItem
            //// doesn't do anything and therefore no finalizer is needed.
        }

        /// <summary>
        /// Initializes a new <see cref="GridStyleInfoIdentity"/> and copies its data from an existing object.
        /// </summary>
        /// <param name="other">The existing object to copy data from.</param>
        protected GridTableCellStyleInfoIdentity(GridTableCellStyleInfoIdentity other)
            : base(other)
        {
            this.column = other.column;
            this.displayElement = other.displayElement;
            this.summaryColumn = other.summaryColumn;
            this.table = other.table;
            this.tableCellType = other.tableCellType;
            this.filterBarSummaryDescriptor = other.filterBarSummaryDescriptor;
        }

        void AddAppearanceSourceStyle(ArrayList styleList, IGridTableCellAppearanceSource appearanceSource, GridEngine engine)
        {
            if (appearanceSource != null && appearanceSource.ShouldSerializeAppearance())
            {
                GridTableCellAppearance appearance = appearanceSource.GetAppearance();
                AddAppearanceStyle(styleList, appearance, engine);
            }
        }

        void AddAppearanceStyle(ArrayList styleList, GridTableCellAppearance appearance, GridEngine engine)
        {
            if (appearance.IsModifiedStyle(this.tableCellType))
            {
                AddStyle(appearance.GetStyle(this.tableCellType), styleList, engine);
            }

            foreach (GridTableCellStyleInfo style in appearance.GetBaseStyles(this.tableCellType))
            {
                if (style != null)
                {
                    AddStyle(style, styleList, engine);
                }
            }
        }

        /// <summary>
        /// Returns the <see cref="GridEngine"/> this object belongs to.
        /// </summary>
        /// <returns>The engine object.</returns>
        public GridEngine GetEngine()
        {
            GridTableDescriptor tableDescriptor = null;
            if (table != null)
            {
                tableDescriptor = table.TableDescriptor;
            }
            else if (displayElement != null)
            {
                tableDescriptor = (GridTableDescriptor)displayElement.ParentTableDescriptor;
            }

            GridEngine engine = tableDescriptor != null ? tableDescriptor.Engine : null;
            return engine;
        }

        /// <summary>
        /// Overriden. Returns base styles that define the inheritance of style properties.
        /// </summary>
        /// <remarks>
        /// Inheritance of style properties is defined by the <see cref="GridTableCellStyleInfoIdentity"/>
        /// object. It has a <see cref="GridTableCellStyleInfoIdentity.GetBaseStyles"/> method that returns
        /// the <see cref="GridStyleInfo"/> objects that form an inheritance chain. Check the
        /// <see cref="GridTableCellStyleInfoIdentity.GetBaseStyleNames"/> method to get string / debug
        /// information about the inheritance chain for a specific element. Also, the designer will show
        /// this debug information about the inheritance chain in a ToolTip when you hover the mouse over
        /// a cell within the "Preview and Edit" window.
        /// <para/>
        /// </remarks>
        /// <param name="thisStyleInfo">A reference to a <see cref="IStyleInfo"/>.</param>
        /// <returns>An array of base styles.</returns>
        public override IStyleInfo[] GetBaseStyles(IStyleInfo thisStyleInfo)
        {
            //// GridTableCellStyleInfoIdentity.GetBaseStyles call triggered by:
            //// style.GdipFont
            //// style.CellType
            GridTableDescriptor tableDescriptor = null;
            if (table != null)
            {
                tableDescriptor = table.TableDescriptor;
            }
            else if (displayElement != null)
            {
                tableDescriptor = (GridTableDescriptor)displayElement.ParentTableDescriptor;
            }

            GridEngine engine = tableDescriptor != null ? tableDescriptor.Engine : null;

            if (cachedBaseStyles == null || version != engine.Version)
            {
                ArrayList styleList = new ArrayList();

                //// In case BaseStyle was specified in QueryCellInfo
                AddBaseStyle(thisStyleInfo, styleList, engine);

                //// Current element and parent elements
                Element element = displayElement;
                while (element != null)
                {
                    //// most of the times those are empty because they can only be modified at runtime ...
                    AddAppearanceSourceStyle(styleList, element as IGridTableCellAppearanceSource, engine);
                    element = element.ParentElement;
                }

                //// Column Descriptor
                AddAppearanceSourceStyle(styleList, column as IGridTableCellAppearanceSource, engine);

                //// Conditional Formats
                if (displayElement != null)
                {
                    Record record = Record.GetParentRecord(displayElement);
                    if (tableDescriptor != null && record != null)
                    {
                        foreach (GridConditionalFormatDescriptor conditionalFormat in tableDescriptor.ConditionalFormats)
                        {
                            if (conditionalFormat.CompareRecord(record))
                            {
                                AddAppearanceSourceStyle(styleList, conditionalFormat, engine);
                            }
                        }
                    }
                }

                //// Grouped Column
                if (this.groupedColumn != null && tableDescriptor != null)
                {
                    GridColumnDescriptor col = tableDescriptor.Columns[groupedColumn.Name];
                    if (col != null)
                    {
                        this.AddAppearanceStyle(styleList, col.GroupByAppearance, engine);
                    }
                }

                //// Stacked Header Descriptor
                if (this.stackedHeader != null)
                {
                    AddAppearanceSourceStyle(styleList, this.stackedHeader as IGridTableCellAppearanceSource, engine);
                }

                if (this.displayElement is GridStackedHeaderRow)
                {
                    AddAppearanceSourceStyle(styleList, ((GridStackedHeaderRow)displayElement).StackedHeaderRowDescriptor as IGridTableCellAppearanceSource, engine);
                }

                //// SummaryColumn Descriptor
                if (this.summaryColumn != null)
                {
                    AddAppearanceSourceStyle(styleList, this.summaryColumn as IGridTableCellAppearanceSource, engine);
                }

                if (this.displayElement is GridSummaryRow)
                {
                    AddAppearanceSourceStyle(styleList, ((GridSummaryRow)displayElement).SummaryRowDescriptor, engine);
                }
                else if (this.summaryColumn != null) 
                {
                    //// e.g. GroupCaptionSummaryCell
                    AddAppearanceSourceStyle(styleList, this.summaryColumn.ParentRow as IGridTableCellAppearanceSource, engine);
                }

                //// TableDescriptor
                if (tableDescriptor != null)
                {
                    AddAppearanceSourceStyle(styleList, tableDescriptor as IGridTableCellAppearanceSource, engine);

                    // Parent TableDescriptor
                    GridTableDescriptor parentTableDescriptor = tableDescriptor;
                    while (parentTableDescriptor != null && parentTableDescriptor.InheritAppearanceFomParent)
                    {
                        parentTableDescriptor = (GridTableDescriptor)parentTableDescriptor.ParentTableDescriptor;
                        AddAppearanceSourceStyle(styleList, parentTableDescriptor as IGridTableCellAppearanceSource, engine);
                    }
                }

                //// Engine
                if (engine != null)
                {
                    //// Default Cell Value Type
                    if (column != null && (this.displayElement is RecordRow || this.displayElement is Record))
                    {
                        AddColumnStyles(styleList, column);
                    }

                    AddAppearanceSourceStyle(styleList, engine as IGridTableCellAppearanceSource, engine);

                    //// Optional default style (DefaultAppearance is not serialized...)
                    AddAppearanceSourceStyle(styleList, engine.InternalDefaultAppearanceSource, engine);
                }

                AddStyle(GridTableCellAppearance.Default.GetStyle(this.tableCellType), styleList, engine);
                AddStyles(GridTableCellAppearance.Default.GetBaseStyles(this.tableCellType), styleList, engine);
                if (engine != null)
                {
                    AddStyle(engine.engineDefaultStyle, styleList, engine);
                }

                version = engine.Version;
                cachedBaseStyles = new GridTableCellStyleInfo[styleList.Count];
                styleList.CopyTo(cachedBaseStyles);
            }

            return cachedBaseStyles;
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude]
        public static void AddBaseStyles(ArrayList styleList, GridEngine engine)
        {
            foreach (GridStyleInfo style in styleList)
            {
                if (style.HasBaseStyle)
                {
                    AddBaseStyle(style, styleList, engine);
                    break;
                }
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude]
        public static void AddStyle(IStyleInfo style, ArrayList styleList, GridEngine engine)
        {
            if (style != null)
            {
                styleList.Add(style);
                AddBaseStyle(style, styleList, engine);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude]
        public static void AddStyles(IStyleInfo[] styles, ArrayList styleList, GridEngine engine)
        {
            for (int n = 0; n < styles.Length; n++)
            {
                AddStyle(styles[n], styleList, engine);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude]
        public static void AddBaseStyle(IStyleInfo style, ArrayList styleList, GridEngine engine)
        {
            string baseStyle = string.Empty;

            if (style.HasValue(GridStyleInfoStore.BaseStyleProperty))
            {
                baseStyle = (string)style.GetValue(GridStyleInfoStore.BaseStyleProperty);
            }

            int level = 0;
            if (baseStyle != string.Empty)
            {
                GridTableCellStyleInfo[] baseStyles = engine.BaseStyles.GetBaseStylesMapStyles(baseStyle, out level);
                for (int n = 0; n < level; n++)
                {
                    styleList.Add(baseStyles[n]);
                }

                baseStyles = engine.DefaultBaseStyles.GetBaseStylesMapStyles(baseStyle, out level);
                for (int n = 0; n < level; n++)
                {
                    styleList.Add(baseStyles[n]);
                }
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude]
        public static void AddColumnStyles(ArrayList styleList, GridColumnDescriptor column)
        {
            string fullName = "System.String";
            FieldDescriptor fd = column.FieldDescriptor;
            if (fd != null && fd.GetPropertyType() != null)
            {
                GridEngine engine = column.Engine;

                Type propertyType = null;
                PropertyDescriptor pd = fd.GetPropertyDescriptor();

                if (fd.IsRelatedField() && column.AllowDropDownCell)
                {
                    propertyType = fd.GetNestedRelatedDescriptor().GetPropertyType();
                    pd = fd.GetNestedRelatedDescriptor().GetPropertyDescriptor();
                    AddStyle(engine.PropertyTypeDefaultStyles["ForeignKey"].Style, styleList, engine);
                    AddStyle(GridPropertyTypeDefaultStyleCollection.Default["ForeignKey"].Style, styleList, engine);
                }
                else if (fd.IsRelatedField())
                {
                    propertyType = fd.GetNestedRelatedDescriptor().GetPropertyType();
                    pd = fd.GetNestedRelatedDescriptor().GetPropertyDescriptor();
                }
                else
                {
                    propertyType = fd.GetPropertyType();
                }

                GridPropertyTypeDefaultStyle propertyDefault = engine.PropertyTypeDefaultStyles[propertyType.FullName];
                GridPropertyTypeDefaultStyle propertyDefaultDefault = GridPropertyTypeDefaultStyleCollection.Default[propertyType.FullName];
                if ((propertyDefault == null || propertyDefault.AllowDropDown) &&
                    (propertyDefaultDefault == null || propertyDefaultDefault.AllowDropDown))
                {
                    //// Automatic support for UITypeEditor, PropertyDescriptor.GetStandardValues
                    if (pd != null && pd.PropertyType != typeof(byte[]) && pd.PropertyType != typeof(DateTime))
                    {
                        object editor = pd.GetEditor(typeof(System.Drawing.Design.UITypeEditor));
                        if (editor != null)
                        {
                            if (column.AllowDropDownCell)
                            {
                                AddStyle(engine.PropertyTypeDefaultStyles["UITypeEditor"].Style, styleList, engine);
                                AddStyle(GridPropertyTypeDefaultStyleCollection.Default["UITypeEditor"].Style, styleList, engine);
                            }

                            propertyType = fd.GetPropertyType();
                        }

                        if (pd.Converter != null && pd.Converter.CanConvertTo(typeof(string)) && pd.Converter.GetStandardValuesSupported())
                        {
                            if (column.AllowDropDownCell)
                            {
                                if (ListUtil.GetStandardValuesExclusive(pd))
                                {
                                    AddStyle(engine.PropertyTypeDefaultStyles["AutoCompleteStandardValues"].Style, styleList, engine);
                                    AddStyle(GridPropertyTypeDefaultStyleCollection.Default["AutoCompleteStandardValues"].Style, styleList, engine);
                                }

                                //// TODO: should I check for byte[] / Image here and set exclusive?

                                AddStyle(engine.PropertyTypeDefaultStyles["StandardValues"].Style, styleList, engine);
                                AddStyle(GridPropertyTypeDefaultStyleCollection.Default["StandardValues"].Style, styleList, engine);
                            }

                            propertyType = fd.GetPropertyType();
                        }

                        if (pd.Converter != null && pd.Converter.GetPropertiesSupported())
                        {
                            if (column.AllowDropDownCell)
                            {
                                AddStyle(engine.PropertyTypeDefaultStyles["PropertyGrid"].Style, styleList, engine);
                                AddStyle(GridPropertyTypeDefaultStyleCollection.Default["PropertyGrid"].Style, styleList, engine);
                            }
                        }
                    }
                }

                fullName = propertyType.FullName;
                if (propertyDefault != null)
                {
                    AddStyle(propertyDefault.Style, styleList, engine);
                }

                if (propertyDefaultDefault != null)
                {
                    AddStyle(propertyDefaultDefault.Style, styleList, engine);
                }
            }
        }

        /// <summary>
        /// Debug helper.
        /// </summary>
        /// <param name="styleList">An array of styles.</param>
        public static void DumpStyles(ArrayList styleList)
        {
            for (int n = 0; n < styleList.Count; n++)
            {
                if (styleList[n] == null)
                {
                    Console.WriteLine(n.ToString() + ": null");
                }
                else
                {
                    Console.WriteLine(n.ToString() + ":");
                    GridStyleInfo style = (GridStyleInfo)styleList[n];
                    if (style.Identity != null)
                    {
                        Console.WriteLine(style.Identity.ToString());
                    }

                    Console.WriteLine(style.ToString());
                }
            }
        }

        /// <summary>
        /// Gets a string that indicates parent styles within the appearance object for the specified table cell element.
        /// </summary>
        /// <returns>A string with debug information.</returns>
        public string[] GetBaseStyleNames()
        {
            GridTableDescriptor tableDescriptor = null;
            if (table != null)
            {
                tableDescriptor = table.TableDescriptor;
            }
            else if (displayElement != null)
            {
                tableDescriptor = (GridTableDescriptor)displayElement.ParentTableDescriptor;
            }

            GridEngine engine = tableDescriptor != null ? tableDescriptor.Engine : null;

            ArrayList styleList = new ArrayList();

            //// Current element and parent elements
            ////            Element element = displayElement;
            ////            while (element != null)
            ////            {
            ////                styleList.Add(element.GetType().Name.ToString());
            ////                element = element.ParentElement;
            ////            }

            //// Column Descriptor
            if (column != null)
            {
                styleList.Add(column.Name + ".Appearance");
            }

            //// Conditional Formats
            if (displayElement != null)
            {
                Record record = Record.GetParentRecord(displayElement);
                if (tableDescriptor != null && record != null)
                {
                    foreach (GridConditionalFormatDescriptor conditionalFormat in tableDescriptor.ConditionalFormats)
                    {
                        if (conditionalFormat.CompareRecord(record))
                        {
                            styleList.Add(conditionalFormat.Name + ".Appearance");
                        }
                    }
                }
            }

            //// Grouped Column
            if (this.groupedColumn != null && tableDescriptor != null)
            {
                GridColumnDescriptor col = tableDescriptor.Columns[groupedColumn.Name];
                if (col != null)
                {
                    styleList.Add(col.Name + ".GroupByAppearance");
                }
            }

            //// SummaryColumn Descriptor
            if (this.summaryColumn != null)
            {
                styleList.Add(summaryColumn.Name + ".Appearance");
            }

            if (this.displayElement is GridSummaryRow)
            {
                styleList.Add(((GridSummaryRow)displayElement).SummaryRowDescriptor.Name + ".Appearance");
            }
            else if (this.summaryColumn != null) 
            {
                //// e.g. GroupCaptionSummaryCell
                styleList.Add(summaryColumn.ParentRow.Name + ".Appearance");
            }

            //// TableDescriptor
            if (tableDescriptor != null)
            {
                //// Parent TableDescriptor
                GridTableDescriptor parentTableDescriptor = tableDescriptor;
                while (parentTableDescriptor != null && parentTableDescriptor.InheritAppearanceFomParent)
                {
                    styleList.Add(parentTableDescriptor.Name + ".Appearance");
                    parentTableDescriptor = (GridTableDescriptor)parentTableDescriptor.ParentTableDescriptor;
                }
            }

            //// Engine
            if (engine != null)
            {
#if ASPNET
                styleList.Add(engine.ParentControl.ClientID + ".Appearance");
#else
                styleList.Add(engine.ParentControl.Name + ".Appearance");
#endif
            }

            //// Default Cell Value Type
            if (engine != null && column != null && (this.displayElement is RecordRow || this.displayElement is Record))
            {
                string fullName = "System.String";
                FieldDescriptor fd = column.FieldDescriptor;
                if (fd != null && fd.GetPropertyType() != null)
                {
                    PropertyDescriptor pd = fd.GetPropertyDescriptor();

                    if (fd.IsRelatedField())
                    {
                        styleList.Add("DefaultStyle[" + "ForeignKey" + "]");
                    }
                    else if (pd != null && pd.Converter != null && pd.Converter.CanConvertTo(typeof(string)) && pd.Converter.GetStandardValuesSupported())
                    {
                        if (ListUtil.GetStandardValuesExclusive(pd))
                        {
                            styleList.Add("DefaultStyle[" + "AutoCompleteStandardValues" + "]");
                        }
                        //// TODO: should I check for byte[] / Image here and set exclusive?

                        styleList.Add("DefaultStyle[" + "StandardValues" + "]");
                    }

                    fullName = fd.GetPropertyType().FullName;
                    int indexOf = GridPropertyTypeDefaultStyleCollection.Default.IndexOf(fullName);
                    if (indexOf != -1)
                    {
                        styleList.Add("DefaultStyle[" + fullName + "]");
                    }
                }
            }

            //// BaseStyles
            ////            if (engine != null)
            ////            {
            ////                string baseStyle = string.Empty;
            ////                foreach (GridStyleInfo style in styleList)
            ////                {
            ////                    if (style.HasBaseStyle)
            ////                    {
            ////                        baseStyle = style.BaseStyle;
            ////                        break;
            ////                    }
            ////                }
            ////                int level = 0;
            ////                if (baseStyle != string.Empty)
            ////                    styleList.Add("BaseStylesMap[" + baseStyle + "]");
            ////
            ////            }

            styleList.Add("GridTableCellAppearance.Default");
            return (string[])styleList.ToArray(typeof(string));
        }

        /// <summary>
        /// Returns a <see cref="GridCellModelBase"/> for the specified id / cell type name.
        /// </summary>
        /// <param name="id">Cell type name.</param>
        /// <returns>The <see cref="GridCellModelBase"/> for the given id.</returns>
        /// <remarks>
        /// Calls <see cref="IGridData.LookupCellModel"/>.
        /// </remarks>
        public override GridCellModelBase LookupCellModel(string id)
        {
            if (Table.TableModel != null)
            {
                return Table.TableModel.CellModels[id];
            }

            return null;
        }

        /// <summary>
        /// Returns the <see cref="GridModel"/> this style belongs to or NULL if the style is used outside a grid model.
        /// </summary>
        /// <returns>A reference to the grid model or NULL if the style is used outside a grid model.</returns>
        public override GridModel GetGridModel()
        {
            return Table.TableModel;
        }

#if ASPNET
#else
        /// <summary>
        /// Returns the active <see cref="GridControlBase"/> for the <see cref="GridModel"/> this style belongs to or NULL
        /// if the style is used outside a grid model.
        /// </summary>
        /// <returns>A reference to the grid control base or NULL if the style is used outside a grid model.</returns>
        public override GridControlBase GetActiveGridView()
        {
            GridModel grid = GetGridModel();
            return grid != null ? (GridControlBase)grid.ActiveGridView : null;
        }
#endif

        GridModel IGridModelSource.Model
        {
            get
            {
                return GetGridModel();
            }
        }

        /// <override/>
        /// <summary>
        /// Returns a string holding the current object.
        /// </summary>
        /// <returns>String representation of the current object.</returns>
        [DebuggerStepThrough()]
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append(" {");
            sb.AppendFormat("{0}", this.tableCellType);
            if (column != null)
            {
                sb.AppendFormat(", Column = {0}", this.column.Name);
            }

            if (groupedColumn != null)
            {
                sb.AppendFormat(", GroupedColumn = {0}", this.groupedColumn.Name);
            }

            if (this.summaryColumn != null)
            {
                sb.AppendFormat(", SummaryColumn = {0}", this.summaryColumn.Name);
            }

            if (this.displayElement != null)
            {
                sb.AppendFormat(", DisplayElement = {0}, ", this.displayElement.GetType().Name);
            }

            if (table != null)
            {
                sb.AppendFormat(", Table = {0}", this.table);
            }

            sb.Append(" }");
            return sb.ToString();
        }

        /// <override/>
        /// <summary>Results of ToString method.</summary>
        public override string Info
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendFormat("{0}", this.TableCellType);
                string[] tableCellTypeBasedOnArray = GridTableCellAppearance.GetBaseStyleNames(this.TableCellType);
                if (tableCellTypeBasedOnArray.Length > 0)
                {
                    sb.Append(" based on: { " + tableCellTypeBasedOnArray[0]);
                    for (int n = 1; n < tableCellTypeBasedOnArray.Length; n++)
                    {
                        sb.Append(", " + tableCellTypeBasedOnArray[n]);
                    }

                    sb.Append(" }\r\n");
                }

                if (this.DisplayElement != null)
                {
                    sb.AppendFormat("Element = {0}", this.DisplayElement.GetType().Name);
                    Element element = this.DisplayElement.ParentDisplayElement;
                    if (element != null)
                    {
                        sb.Append(", Child Of: { " + element.GetType().Name.ToString());
                        if (element is ChildTable)
                        {
                            sb.Append(" of " + element.ParentTable.TableDescriptor.Name);
                        }

                        element = element.ParentElement;
                        while (element != null)
                        {
                            sb.Append(", ");
                            if (element is NestedTable)
                            {
                                sb.Append("\r\n    ");
                            }

                            sb.Append(element.GetType().Name.ToString());
                            if (element is ChildTable)
                            {
                                sb.Append(" of " + element.ParentTable.TableDescriptor.Name);
                            }
                            else if (element is Group && ((Group)element).CategoryColumns.Count > 0)
                            {
                                sb.Append(" (" + ((Group)element).CategoryColumns[0].Name + ")");
                            }

                            element = element.ParentDisplayElement;
                        }

                        sb.Append(" }");
                    }
                }

                if (this.Column != null)
                {
                    sb.AppendFormat("\r\nColumn = {0}", this.Column.Name);
                }

                if (this.GroupedColumn != null)
                {
                    sb.AppendFormat("\r\nGroupedColumn = {0}", this.GroupedColumn.Name);
                }

                if (this.SummaryColumn != null)
                {
                    sb.AppendFormat("\r\nSummaryColumn = {0}", this.SummaryColumn.Name);
                }

                string[] tableAppareanceBasedOnArray = this.GetBaseStyleNames();
                if (tableAppareanceBasedOnArray.Length > 0)
                {
                    sb.Append("\r\nBaseAppearance: { " + tableAppareanceBasedOnArray[0]);
                    for (int n = 1; n < tableAppareanceBasedOnArray.Length; n++)
                    {
                        if (tableAppareanceBasedOnArray[n].StartsWith("DefaultStyle"))
                        {
                            sb.Append(",\r\n    " + tableAppareanceBasedOnArray[n]);
                        }
                        else
                        {
                            sb.Append(", " + tableAppareanceBasedOnArray[n]);
                        }
                    }

                    sb.Append(" }");
                }

                return sb.ToString();
            }
        }

        /// <override/>
        /// <summary>Occurs when a property in <see cref="StyleInfoBase"/> is changed.</summary>
        /// <param name="style">The <see cref="StyleInfoBase"/> instance that was changed.</param>
        /// <param name="sip">An identity for the property to operate on.</param>
        public override void OnStyleChanged(StyleInfoBase style, StyleInfoProperty sip)
        {
            cachedBaseStyles = null;
            Element el = displayElement;
            GridTableCellStyleInfoEventArgs e = null;
            while (el != null)
            {
                IGridTableCellStyleChanged tableCellStyleChanged = el as IGridTableCellStyleChanged;
                if (tableCellStyleChanged != null)
                {
                    if (e == null)
                    {
                        e = new GridTableCellStyleInfoEventArgs(this, (GridTableCellStyleInfo)style, sip);
                    }

                    tableCellStyleChanged.RaiseTableCellStyleChanged(e);
                    if (e.Handled)
                    {
                        break;
                    }
                }

                el = el.ParentElement;
            }
        }

        ////        /// <summary>
        ////        /// Results of ToString method.
        ////        /// </summary>
        ////        public string Info
        ////        {
        ////            get
        ////            {
        ////                return ToString();
        ////            }
        ////        }
    }
}
