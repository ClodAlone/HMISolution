//-------------------------------------------------------------------------------------------------
// <copyright file="GridStyleInfo.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Grid
{
    ////    [Syncfusion.Documentation.DocumentationExclude()]
    ////    [DebuggerStepThrough()]
    ////    internal class GridTemporaryStyleInfoIdentity: GridStyleInfoIdentity
    ////    {
    ////        GridControlBase gridView;
    ////        GridStyleInfo styleInfo;
    ////        protected IStyleInfo[] savedBaseStyles = null;
    ////
    ////        //// Cache
    ////        public GridTemporaryStyleInfoIdentity(GridStyleInfo styleInfo, GridControlBase gridView)
    ////            : base((GridStyleInfoIdentity) styleInfo.Identity)
    ////        {
    ////            this.styleInfo = styleInfo;
    ////            this.gridView = gridView;
    ////
    ////            //// What about offLine, GC.SuppressFinalize(this); ?
    ////        }
    ////
    ////        public override void Dispose()
    ////        {
    ////            styleInfo = null;
    ////            savedBaseStyles = null;
    ////            base.Dispose();
    ////        }
    ////
    ////        public GridStyleInfo StyleInfo
    ////        {
    ////            get
    ////            {
    ////                return styleInfo;
    ////            }
    ////        }
    ////
    ////        ///// <summary>
    ////        ///// Returns the active <see cref="GridControlBase"/> for the <see cref="GridModel"/> this style belongs to or NULL
    ////        ///// if the style is used outside a grid model.
    ////        ///// </summary>
    ////        ///// <returns>A reference to the grid control base or NULL if the style is used outside a grid model.</returns>
    ////        public override GridControlBase GetActiveGridView()
    ////        {
    ////            return gridView;
    ////        }
    ////
    ////
    ////        ///// <override/>
    ////        public override IStyleInfo[] GetBaseStyles(IStyleInfo thisStyleInfo)
    ////        {
    ////            if (savedBaseStyles == null && this.Data != null)
    ////            {
    ////                IStyleInfo[] baseStyles = base.GetBaseStyles(thisStyleInfo);
    ////                savedBaseStyles = new IStyleInfo[baseStyles.Length+1];
    ////                Array.Copy(baseStyles, 0, savedBaseStyles, 1, baseStyles.Length);
    ////                savedBaseStyles[0] = styleInfo;
    ////            }
    ////            return savedBaseStyles;
    ////        }
    ////
    ////        ///// <override/>
    ////        public override bool OffLine
    ////        {
    ////            get
    ////            {
    ////                return true;
    ////            }
    ////        }
    ////
    ////
    ////        ///// <override/>
    ////        public override void OnStyleChanged(StyleInfoBase style, StyleInfoProperty sip)
    ////        {
    ////        }
    ////    }

    [Syncfusion.Documentation.DocumentationExclude()]
    [DebuggerStepThrough()]
    internal class GridViewStyleInfoIdentity : GridStyleInfoIdentity
    {
        GridControlBase gridView;

        //// Cache
        public GridViewStyleInfoIdentity(GridControlBase gridView, IGridData data, int rowIndex, int colIndex)
            : base(data, rowIndex, colIndex, true)
        {
            this.gridView = gridView;
        }

        /// <override/>
        public override void Dispose()
        {
            gridView = null;
            base.Dispose();
        }

        /// <summary>
        /// Returns the active <see cref="GridControlBase"/> for the <see cref="GridModel"/> this style belongs to or NULL
        /// if the style is used outside a grid model.
        /// </summary>
        /// <returns>
        /// A reference to the grid control base or NULL if the style is used outside a grid model.
        /// </returns>
        /// <override/>
        public override GridControlBase GetActiveGridView()
        {
            return gridView;
        }
    }

    /// <summary>
    /// Identity is the reference back to the cell the style belongs to.
    /// </summary>
    /// <remarks>
    /// GridStyleInfo will ask for base styles through GridStyleInfoIdentity.GetBaseStyles
    /// when the user accesses a property that is not initialized in the style.
    /// GridStyleInfoIdentity also ensures that changes are made permanent in GridData.
    /// </remarks>
    [DebuggerStepThrough()]
    public class GridStyleInfoIdentity : StyleInfoIdentityBase, IGridModelSource
    {
        IGridData data;
        bool offLine;
        GridCellPos cellPos;

        // Cache.
        IStyleInfo[] cachedBaseStyles = null;

        /// <summary>
        /// Finalizes an instance of the <see cref="GridStyleInfoIdentity"/> class. Removes the associated cell cache object from the volatile data store.
        /// </summary>
        ~GridStyleInfoIdentity()
        {
            if (!offLine && data != null)
            {
                ((IGridVolatileData)data).ResetItem(cellPos);
            }
        }

        /// <override/>
        /// <summary>
        /// Releases all resources used by the GridStyleInfoIdentity.
        /// </summary>
        public override void Dispose()
        {
            if (!offLine && data != null)
            {
                ((IGridVolatileData)data).ResetItem(cellPos);
            }

            data = null;
            cachedBaseStyles = null;
            base.Dispose(); // will call GC.SupressFinalize
        }

        /// <overload>
        /// Initializes a <see cref="GridStyleInfoIdentity"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridStyleInfoIdentity"/> with a reference to <see cref="IGridData"/>, row, and column index.
        /// </summary>
        /// <param name="data">A reference to <see cref="IGridData"/>.</param>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        public GridStyleInfoIdentity(IGridData data, int rowIndex, int colIndex)
        {
            this.data = data;
            this.cellPos = new GridCellPos(rowIndex, colIndex);
            this.offLine = false;
        }

        /// <summary>
        /// Initializes a new <see cref="GridStyleInfoIdentity"/> with a reference to <see cref="IGridData"/>, row and column index, and offline state.
        /// </summary>
        /// <param name="data">A reference to <see cref="IGridData"/>.</param>
        /// <param name="pos">Cell coordinates.</param>
        public GridStyleInfoIdentity(IGridData data, GridCellPos pos)
        {
            this.data = data;
            this.cellPos = pos;
            this.offLine = false;
        }

        /// <summary>
        /// Gets a value indicating whether changes in the style object should stored. True if changes in this style object should not be stored in the associated <see cref="IGridData"/>.
        /// </summary>
        public virtual bool OffLine
        {
            get
            {
                return offLine;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="GridStyleInfoIdentity"/> with a reference to <see cref="IGridData"/>, row and column index, and offline state.
        /// </summary>
        /// <param name="data">A reference to <see cref="IGridData"/></param>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <param name="offLine">True if changes in this style object should not be stored in the associated <see cref="IGridData"/>.</param>
        public GridStyleInfoIdentity(IGridData data, int rowIndex, int colIndex, bool offLine)
        {
            this.data = data;
            this.cellPos = new GridCellPos(rowIndex, colIndex);
            this.offLine = offLine;

            // GridStyleInfoIdentity implements a finalizer that is not needed when object is offLine.
            // Therefore, we call GC.SupressFinalize to
            // immediately take this object off the finalization queue
            // and prevent finalization code for this object.
            if (offLine)
            {
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="GridStyleInfoIdentity"/> with a reference to <see cref="IGridData"/>, row and column index, and offline state.
        /// </summary>
        /// <param name="data">A reference to <see cref="IGridData"/>.</param>
        /// <param name="pos">Cell coordinates.</param>
        /// <param name="offLine">True if changes in this style object should not be stored in the associated <see cref="IGridData"/>.</param>
        public GridStyleInfoIdentity(IGridData data, GridCellPos pos, bool offLine)
        {
            this.data = data;
            this.cellPos = pos;
            this.offLine = offLine;

            //// GridStyleInfoIdentity implements a finalizer that is not needed when an object is offLine.
            //// Therefore, we call GC.SupressFinalize to
            //// immediately take this object off the finalization queue
            //// and prevent finalization code for this object.
            if (offLine)
            {
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Initializes a new <see cref="GridStyleInfoIdentity"/> and copies its data from an existing object.
        /// </summary>
        /// <param name="other">The existing object to copy data from.</param>
        protected GridStyleInfoIdentity(GridStyleInfoIdentity other)
        {
            this.data = other.data;
            this.cellPos = other.cellPos;

            //// GridStyleInfoIdentity implements a finalizer that is not needed when object is offLine.
            //// Therefore, we call GC.SupressFinalize to
            //// immediately take this object off the finalization queue
            //// and prevent finalization code for this object.
            if (offLine)
            {
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Creates a new <see cref="GridStyleInfoIdentity"/> and copies its identity information from the current object. The new
        /// instance will be detached from <see cref="IGridData"/> so that changes in this style object are not be stored in the associated <see cref="IGridData"/>.
        /// </summary>
        /// <returns>A new <see cref="GridStyleInfoIdentity"/> instance.</returns>
        /// <remarks>
        /// Lets a style object load base styles and default values but disables
        /// saving changes back to the grid. (see OnStyleChanged below)
        /// </remarks>
        public GridStyleInfoIdentity MakeOfflineIdentity()
        {
            return new GridStyleInfoIdentity(data, cellPos.RowNumber, cellPos.ColumnNumber/*rowIndex, colIndex*/, true);
        }

        /// <summary>
        /// Gets to <see cref="IGridData"/>.
        /// </summary>
        public IGridData Data
        {
            [DebuggerStepThrough()]
            get { return data; }
        }

        /// <summary>
        /// Gets the row index.
        /// </summary>
        public int RowIndex
        {
            [DebuggerStepThrough()]
            get { return cellPos._rowNumber; }
        }

        /// <summary>
        /// Gets the column index.
        /// </summary>
        public int ColIndex
        {
            [DebuggerStepThrough()]
            get { return cellPos._columnNumber; }
        }

        /// <summary>
        /// Gets or sets the cell coordinates.
        /// </summary>
        public GridCellPos CellPos
        {
            [DebuggerStepThrough()]
            get
            {
                return cellPos;
            }

            set
            {
                cellPos = value;
            }
        }

        /// <summary>
        /// Overriden. Returns base styles from <see cref="IGridData"/> by calling <see cref="IGridData.GetBaseStyles"/>.
        /// </summary>
        /// <param name="thisStyleInfo">A reference to a <see cref="IStyleInfo"/>.</param>
        /// <returns>An array of base styles.</returns>
        public override IStyleInfo[] GetBaseStyles(IStyleInfo thisStyleInfo)
        {
            if (InnerIdentity != null)
            {
                return InnerIdentity.GetBaseStyles(thisStyleInfo);
            }

            if (cachedBaseStyles == null && data != null)
            {
                cachedBaseStyles = data.GetBaseStyles(thisStyleInfo as GridStyleInfo, RowIndex, ColIndex);
            }

            return cachedBaseStyles;
        }

        /// <exclude/>
        public void ResetBaseStylesCache()
        {
            cachedBaseStyles = null;
        }

        /// <summary>
        /// Returns a <see cref="GridCellModelBase"/> for the specified id / cell type name.
        /// </summary>
        /// <param name="id">Cell type name.</param>
        /// <returns>The <see cref="GridCellModelBase"/> for the given id.</returns>
        /// <remarks>
        /// Calls <see cref="IGridData.LookupCellModel"/>.
        /// </remarks>
        public virtual GridCellModelBase LookupCellModel(string id)
        {
            return data.LookupCellModel(id);
        }

        /// <summary>
        /// Returns the <see cref="GridModel"/> this style belongs to or NULL if the style is used outside a grid model.
        /// </summary>
        /// <returns>A reference to the grid model or NULL if the style is used outside a grid model.</returns>
        public virtual GridModel GetGridModel()
        {
            GridVolatileData gdata = this.data as GridVolatileData;
            GridModel grid = (gdata != null) ? gdata.Grid : null;
            return grid;
        }

        /// <summary>
        /// Returns the active <see cref="GridControlBase"/> for the <see cref="GridModel"/> this style belongs to or NULL
        /// if the style is used outside a grid model.
        /// </summary>
        /// <returns>A reference to the grid control base or NULL if the style is used outside a grid model.</returns>
        public virtual GridControlBase GetActiveGridView()
        {
            GridModel grid = GetGridModel();
            return grid != null ? grid.ActiveGridView : null;
        }

        GridModel IGridModelSource.Model
        {
            get
            {
                return GetGridModel();
            }
        }

        /// <override/>
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        [DebuggerStepThrough()]
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append(" {");
            sb.Append("rowIndex = ");
            sb.Append(RowIndex.ToString());
            sb.Append(", colIndex = ");
            sb.Append(ColIndex.ToString());
            sb.Append(" }");
            return sb.ToString();
        }

        /// <summary>
        /// Overridden. If the style is not offline, saves its changes in the <see cref="IGridData"/>.
        /// </summary>
        /// <param name="style">A reference to the <see cref="GridStyleInfo"/> object.</param>
        /// <param name="sip">The <see cref="StyleInfoProperty"/> that identifies the changed style property.</param>
        public override void OnStyleChanged(StyleInfoBase style, StyleInfoProperty sip)
        {
            //// Make style permanent in GridData.
            if (!offLine && data != null)
            {
                //// GridModel.SetCellInfo uses following code to avoid recursion.
                ////GridStyleInfoIdentity identity = (GridStyleInfoIdentity) style.Identity;
                ////identity = new GridStyleInfoIdentity(identity.Data, identity.RowIndex, identity.ColIndex, true);
                ////GridStyleInfoStore store = (GridStyleInfoStore) style.Store;
                ////data[RowIndex, ColIndex] = new GridStyleInfo(identity, store);
                data[RowIndex, ColIndex] = (GridStyleInfo)style;
            }

            cachedBaseStyles = null;
        }

        /// <summary>
        /// Gets results of ToString method.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string Info
        {
            get
            {
                return ToString();
            }
        }

        ///// <override/>
        ////public override StyleInfoBase GetBaseStyleNotEmptyExpandable(IStyleInfo thisStyleInfo, StyleInfoProperty sip)
        ////{
        ////    StyleInfoBase style = base.GetBaseStyleNotEmptyExpandable(thisStyleInfo, sip);

        ////    if (thisStyleInfo is StyleInfoBase && style != null)
        ////    {
        ////        //// Special handling of RowStyles and ColStyles for those case
        ////        //// where a cell could inherit settings from both of them. Such
        ////        //// information would get lost in the optimized style.
        ////        GridStyleInfoIdentity id = style.Identity as GridStyleInfoIdentity;
        ////        if (id != null)
        ////        {
        ////            if (id.RowIndex == -1 || id.ColIndex == -1)
        ////                return (StyleInfoBase) thisStyleInfo;
        ////        }
        ////    }

        ////    return style;
        ////}

        /// <override/>
        /// <summary>
        /// Returns the style that has the specific property initialized.
        /// </summary>
        /// <param name="thisStyleInfo">The style information.</param>
        /// <param name="sip">Identifier for the property to operate on.</param>
        /// <returns>A Syncfusion.Styles.StyleInfoBase that has the property initialized.</returns>
        public override StyleInfoBase GetBaseStyle(IStyleInfo thisStyleInfo, StyleInfoProperty sip)
        {
            //// I override this method here in Grid.Windows to revert back
            //// to the old behavior for GetBaseStyle for grid windows product.
            //// In version 5.1 the base class version of this method was changed
            //// to use/call FindStyleInfoProperty which slows things down.

            if (InnerIdentity != null)
            {
                return InnerIdentity.GetBaseStyle(thisStyleInfo, sip);
            }

            IStyleInfo[] baseStyles = GetBaseStyles(thisStyleInfo);
            if (baseStyles != null)
            {
                foreach (StyleInfoBase style in baseStyles)
                {
                    if (style != null && style.Store != null && style.HasValue(sip))
                    {
                        return style;
                    }
                }
            }

            return null;
        }
    }

    /// <summary>
    /// GridStyleInfoStore holds the plain data for a style object excluding indentity information.
    /// </summary>
    /// <remarks>
    /// When persisting grid cells, <see cref="GridStyleInfoStore"/> is the object that should be
    /// saved. Identity information can be recreated at run-time when loading cell information but the
    /// cell information must be saved.
    /// <para/>
    /// GridStyleInfoStore also holds the static "layout" information for the style.
    /// StaticData contains static variables with the information to access data
    /// in BitVector32 and StyleInfoObjectStore. This information can be shared
    /// among style objects of the same type but collision must be avoided between
    /// style types of different products. Having GridStyleInfoStore and ChartStyleInfoStore
    /// types solves that collision problem.
    /// </remarks>
    [Serializable,
    StaticDataField("sd")]
    [DebuggerStepThrough()]
    public class GridStyleInfoStore : StyleInfoStore
    {
        static StaticData sd = new StaticData(typeof(GridStyleInfoStore), typeof(GridStyleInfo), false);

        internal static StaticData StaticData
        {
            get
            {
                return sd;
            }
        }

        // Objects - more frequently used fields should come first because of memory performance reasons
        // Data will be allocated per style object on a slot basis, 4 object references at a time.

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.CellType"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CellTypeProperty = sd.CreateStyleInfoProperty(typeof(string), "CellType");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.CellValue"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CellValueProperty = sd.CreateStyleInfoProperty(typeof(object), "CellValue");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Interior"/> property.
        /// </summary>
        public readonly static StyleInfoProperty InteriorProperty = sd.CreateStyleInfoProperty(typeof(BrushInfo), "Interior");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.TextColor"/> property.
        /// </summary>
        public readonly static StyleInfoProperty TextColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "TextColor");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Font"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FontProperty = sd.CreateStyleInfoProperty(typeof(GridFontInfo), "Font");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.CellValueType"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CellValueTypeProperty = sd.CreateStyleInfoProperty(typeof(Type), "CellValueType");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Format"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FormatProperty = sd.CreateStyleInfoProperty(typeof(string), "Format");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.BaseStyle"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BaseStyleProperty = sd.CreateStyleInfoProperty(typeof(string), "BaseStyle");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Description"/> property.
        /// </summary>
        public readonly static StyleInfoProperty DescriptionProperty = sd.CreateStyleInfoProperty(typeof(string), "Description");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ValueMember"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ValueMemberProperty = sd.CreateStyleInfoProperty(typeof(string), "ValueMember");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.DisplayMember"/> property.
        /// </summary>
        public readonly static StyleInfoProperty DisplayMemberProperty = sd.CreateStyleInfoProperty(typeof(string), "DisplayMember");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.DataSource"/> property.
        /// </summary>
        public readonly static StyleInfoProperty DataSourceProperty = sd.CreateStyleInfoProperty(typeof(object), "DataSource", StyleInfoPropertyOptions.Serializable);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.PropertyDescriptor"/> property.
        /// </summary>
        public readonly static StyleInfoProperty PropertyDescriptorProperty = sd.CreateStyleInfoProperty(typeof(PropertyDescriptor), "PropertyDescriptor", StyleInfoPropertyOptions.None);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.FormulaTag"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FormulaTagProperty = sd.CreateStyleInfoProperty(typeof(GridFormulaTag), "FormulaTag");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Tag"/> property.
        /// </summary>
        public readonly static StyleInfoProperty TagProperty = sd.CreateStyleInfoProperty(typeof(object), "Tag");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Borders"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BordersProperty = sd.CreateStyleInfoProperty(typeof(GridBordersInfo), "Borders");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.TextMargins"/> property.
        /// </summary>
        public readonly static StyleInfoProperty TextMarginsProperty = sd.CreateStyleInfoProperty(typeof(GridMarginsInfo), "TextMargins");
        
        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ImageList"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ImageListProperty = sd.CreateStyleInfoProperty(typeof(ImageList), "ImageList", StyleInfoPropertyOptions.Serializable);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ImageIndex"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ImageIndexProperty = sd.CreateStyleInfoProperty(typeof(int), "ImageIndex");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Error"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ErrorProperty = sd.CreateStyleInfoProperty(typeof(string), "Error");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.MaxLength"/> property.
        /// </summary>
        public readonly static StyleInfoProperty MaxLengthProperty = sd.CreateStyleInfoProperty(typeof(int), "MaxLength");
        
        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.CultureInfo"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CultureInfoProperty = sd.CreateStyleInfoProperty(typeof(CultureInfo), "CultureInfo", StyleInfoPropertyOptions.Serializable);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.CheckBoxOptions"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CheckBoxOptionsProperty = sd.CreateStyleInfoProperty(typeof(GridCheckBoxCellInfo), "CheckBoxOptions");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ValidateValue"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ValidateValueProperty = sd.CreateStyleInfoProperty(typeof(GridCellValidateValueInfo), "ValidateValue");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.NumericUpDown"/> property.
        /// </summary>
        public readonly static StyleInfoProperty NumericUpDownProperty = sd.CreateStyleInfoProperty(typeof(GridNumericUpDownCellInfo), "NumericUpDown");
        
        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ChoiceList"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ChoiceListProperty = sd.CreateStyleInfoProperty(typeof(StringCollection), "ChoiceList");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.MaskEdit"/> property.
        /// </summary>
        public readonly static StyleInfoProperty MaskEditProperty = sd.CreateStyleInfoProperty(typeof(GridMaskEditInfo), "MaskEdit");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ProgressBar"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ProgressBarProperty = sd.CreateStyleInfoProperty(typeof(GridProgressBarInfo), "ProgressBar");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.CurrencyEdit"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CurrencyEditProperty = sd.CreateStyleInfoProperty(typeof(GridCurrencyEditInfo), "CurrencyEdit");
        
        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.PasswordChar"/> property.
        /// </summary>
        public readonly static StyleInfoProperty PasswordCharProperty = sd.CreateStyleInfoProperty(typeof(char), "PasswordChar");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.CellTipText"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CellTipTextProperty = sd.CreateStyleInfoProperty(typeof(string), "CellTipText");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Control"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ControlProperty = sd.CreateStyleInfoProperty(typeof(Control), "Control", StyleInfoPropertyOptions.Serializable);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.BackgroundImageID"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BackgroundImageIDProperty = sd.CreateStyleInfoProperty(typeof(string), "BackgroundImageID");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.BackgroundImage"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BackgroundImageProperty = sd.CreateStyleInfoProperty(typeof(Image), "BackgroundImage");

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.BorderMargins"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BorderMarginsProperty = sd.CreateStyleInfoProperty(typeof(GridMarginsInfo), "BorderMargins");

        // BitArray member - maximum number of bits should not be more than 64 Bits.

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.HorizontalAlignment"/> property.
        /// </summary>
        public readonly static StyleInfoProperty HorizontalAlignmentProperty = sd.CreateStyleInfoProperty(typeof(GridHorizontalAlignment), "HorizontalAlignment", 3, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.VerticalAlignment"/> property.
        /// </summary>
        public readonly static StyleInfoProperty VerticalAlignmentProperty = sd.CreateStyleInfoProperty(typeof(GridVerticalAlignment), "VerticalAlignment", 3, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ReadOnly"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ReadOnlyProperty = sd.CreateStyleInfoProperty(typeof(bool), "ReadOnly", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.RightToLeft"/> property.
        /// </summary>
        public readonly static StyleInfoProperty RightToLeftProperty = sd.CreateStyleInfoProperty(typeof(RightToLeft), "RightToLeft", 2, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.WrapText"/> property.
        /// </summary>
        public readonly static StyleInfoProperty WrapTextProperty = sd.CreateStyleInfoProperty(typeof(bool), "WrapText", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.WrapRotatedText"/> property.
        /// </summary>
        public readonly static StyleInfoProperty WrapRotatedTextProperty = sd.CreateStyleInfoProperty(typeof(bool), "WrapRotatedText", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Trimming"/> property.
        /// </summary>
        public readonly static StyleInfoProperty TrimmingProperty = sd.CreateStyleInfoProperty(typeof(StringTrimming), "Trimming", 5, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Enabled"/> property.
        /// </summary>
        public readonly static StyleInfoProperty EnabledProperty = sd.CreateStyleInfoProperty(typeof(bool), "Enabled", 1, true);
        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.AutoCompleteInEditMode"/> property.
        /// </summary>
        public readonly static StyleInfoProperty AutoCompleteInEditModeProperty = sd.CreateStyleInfoProperty(typeof(GridComboSelectionOptions), "AutoCompleteInEditMode", 3, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.TriState"/> property.
        /// </summary>
        public readonly static StyleInfoProperty TriStateProperty = sd.CreateStyleInfoProperty(typeof(bool), "TriState", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Clickable"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ClickableProperty = sd.CreateStyleInfoProperty(typeof(bool), "Clickable", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.HotkeyPrefix"/> property.
        /// </summary>
        public readonly static StyleInfoProperty HotkeyPrefixProperty = sd.CreateStyleInfoProperty(typeof(HotkeyPrefix), "HotkeyPrefix", 2, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.VerticalScrollbar"/> property.
        /// </summary>
        public readonly static StyleInfoProperty VerticalScrollbarProperty = sd.CreateStyleInfoProperty(typeof(bool), "VerticalScrollbar", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.AutoSize"/> property.
        /// </summary>
        public readonly static StyleInfoProperty AutoSizeProperty = sd.CreateStyleInfoProperty(typeof(bool), "AutoSize", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.AllowEnter"/> property.
        /// </summary>
        public readonly static StyleInfoProperty AllowEnterProperty = sd.CreateStyleInfoProperty(typeof(bool), "AllowEnter", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.StrictValueType"/> property.
        /// </summary>
        public readonly static StyleInfoProperty StrictValueTypeProperty = sd.CreateStyleInfoProperty(typeof(bool), "StrictValueType", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ShowButtons"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowButtonsProperty = sd.CreateStyleInfoProperty(typeof(GridShowButtons), "ShowButtons", 5, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.CellAppearance"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CellAppearanceProperty = sd.CreateStyleInfoProperty(typeof(GridCellAppearance), "CellAppearance", 3, true);
        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.AutoFit"/> property.
        /// </summary>
        public readonly static StyleInfoProperty AutoFitProperty = sd.CreateStyleInfoProperty(typeof(AutoFitOptions), "AutoFit", 4, true);
        /// <summary>
        ///Provides information about the <see cref="GridStyleInfo.AutoFitChar"/> property.
        ///</summary>
        public readonly static StyleInfoProperty AutoFitCharProperty = sd.CreateStyleInfoProperty(typeof(char), "AutoFitChar", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.FloatCell"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FloatCellProperty = sd.CreateStyleInfoProperty(typeof(bool), "FloatCell", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.FloodCell"/> property.
        /// </summary>
        public readonly static StyleInfoProperty FloodCellProperty = sd.CreateStyleInfoProperty(typeof(bool), "FloodCell", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.MergeCell"/> property.
        /// </summary>
        public readonly static StyleInfoProperty MergeCellProperty = sd.CreateStyleInfoProperty(typeof(GridMergeCellDirection), "MergeCell", 2, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ExclusiveChoiceList"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ExclusiveChoiceListProperty = sd.CreateStyleInfoProperty(typeof(bool), "ExclusiveChoiceList", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.DropDownStyle"/> property.
        /// </summary>
        public readonly static StyleInfoProperty AutoCompleteProperty = sd.CreateStyleInfoProperty(typeof(bool), "AutoComplete", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.TextAlign"/> property.
        /// </summary>
        public readonly static StyleInfoProperty TextAlignProperty = sd.CreateStyleInfoProperty(typeof(GridTextAlign), "TextAlign", 2, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.RadioButtonAlignment"/> property.
        /// </summary>
        public readonly static StyleInfoProperty RadioButtonAlignmentProperty = sd.CreateStyleInfoProperty(typeof(ButtonAlignment), "RadioButtonAlignment", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.Themed"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ThemedProperty = sd.CreateStyleInfoProperty(typeof(bool), "Themed", 1, true);

        ////        /// <summary>
        ////        /// Provides information about the <see cref="GridStyleInfo.Locked"/> property.
        ////        /// </summary>
        ////        public readonly static StyleInfoProperty LockedProperty = sd.CreateStyleInfoProperty(typeof(bool), "Locked", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.CharacterCasing"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CharacterCasingProperty = sd.CreateStyleInfoProperty(typeof(CharacterCasing), "CharacterCasing", 2, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.BackgroundImageMode"/> property.
        /// </summary>
        public readonly static StyleInfoProperty BackgroundImageModeProperty = sd.CreateStyleInfoProperty(typeof(GridBackgroundImageMode), "BackgroundImageMode", 2, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ImageSizeMode"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ImageSizeModeProperty = sd.CreateStyleInfoProperty(typeof(GridImageSizeMode), "ImageSizeMode", 5, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ImageFromByteArray"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ImageFromByteArrayProperty = sd.CreateStyleInfoProperty(typeof(bool), "ImageFromByteArray", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ParseFormats"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ParseFormatsProperty = sd.CreateStyleInfoProperty(typeof(string[]), "ParseFormats");
        
        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }
        
        static GridStyleInfoStore()
        {
            // Static factory methods for subobjects.
            BordersProperty.CreateObject = new CreateSubObjectHandler(GridBordersInfo.CreateObject);
            FontProperty.CreateObject = new CreateSubObjectHandler(GridFontInfo.CreateObject);
            CheckBoxOptionsProperty.CreateObject = new CreateSubObjectHandler(GridCheckBoxCellInfo.CreateObject);
            ValidateValueProperty.CreateObject = new CreateSubObjectHandler(GridCellValidateValueInfo.CreateObject);
            NumericUpDownProperty.CreateObject = new CreateSubObjectHandler(GridNumericUpDownCellInfo.CreateObject);
            TextMarginsProperty.CreateObject = new CreateSubObjectHandler(GridMarginsInfo.CreateObject);
            BorderMarginsProperty.CreateObject = new CreateSubObjectHandler(GridMarginsInfo.CreateObject);
            MaskEditProperty.CreateObject = new CreateSubObjectHandler(GridMaskEditInfo.CreateObject);
            ProgressBarProperty.CreateObject = new CreateSubObjectHandler(GridProgressBarInfo.CreateObject);
            CurrencyEditProperty.CreateObject = new CreateSubObjectHandler(GridCurrencyEditInfo.CreateObject);

            CultureInfoProperty.SerializeXmlBehavior = SerializeXmlBehavior.SerializeAsString;
            ImageListProperty.SerializeXmlBehavior = SerializeXmlBehavior.Skip;
            InteriorProperty.SerializeXmlBehavior = SerializeXmlBehavior.SerializeAsString;

            GridStyleInfoStore.CellValueProperty.IsAnyObject = true;
            // maybe later - don't want to modify existing style.ToString() behavior.
            // GridStyleInfoStore.CellValueProperty.IsConvertibleToBase64 = true;
        }

        /// <overload>
        /// Initializes a <see cref="GridStyleInfoStore"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridStyleInfoStore"/>.
        /// </summary>
        public GridStyleInfoStore()
        {
            if (sd.IsEmpty)
            {
                new GridStyleInfo();
            }
        }

        /// <summary>
        /// Initializes a new <see cref="GridStyleInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridStyleInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            if (sd.IsEmpty)
            {
                new GridStyleInfo();
            }
        }

        /// <override/>
        /// <summary>
        /// Creates a copy of the current object.
        /// </summary>
        /// <returns>A duplicate of the current object.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new GridStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }

    /// <summary>
    /// GridStyleInfo holds all information stored for a cell.
    /// </summary>
    /// <remarks>
    /// GridStyleInfo provides user-friendly access to all properties stored
    /// in GridStyleInfoStore. It also has Identity information and can inherit
    /// properties from base styles (row styles, column styles, table style).
    /// <para/>
    /// <see cref="GridModel"/> provides a very simple way to query and change cell contents
    /// using the indexer.
    /// <para/>
    /// A cell's behavior and appearance can be customized with the following properties of the <see cref="GridStyleInfo"/> class:
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>PropertyName</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.AllowEnter"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Gets / sets if pressing the &lt;Enter&gt;-Key should insert a new line into the edited text. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.AutoSize"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Gets / sets if the cell height should automatically increase when the edited text does not fit into the cell and <see cref="GridStyleInfo.WrapText"/> is True. If <see cref="GridStyleInfo.WrapText"/> is False, <see cref="GridStyleInfo.AutoSize"/> will affect the column width. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.BackgroundImage"/> (<see cref="System.Drawing.Image"/>)</term>
    ///         <description>Gets / sets the image that the cell displays as background. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.BackgroundImageMode"/> (<see cref="GridBackgroundImageMode"/>)</term>
    ///         <description>Indicates how the background image is displayed. (Default: GridBackgroundImageMode)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.BaseStyle"/> (<see cref="System.String"/>)</term>
    ///         <description>The base style for this style instance with default values for properties that are not initialized for this style object. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Borders"/> (<see cref="GridBordersInfo"/>)</term>
    ///         <description>Top, left, bottom, and right border settings. (Default: GridBordersInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellAppearance"/> (<see cref="GridCellAppearance"/>)</term>
    ///         <description>Specifies if cell edges shall be drawn raised, sunken, or flat (default). (Default: GridCellAppearance.Flat)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellTipText"/> (<see cref="System.String"/>)</term>
    ///         <description>ToolTip text to be displayed when user hovers mouse over cell. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellType"/> (<see cref="System.String"/>)</term>
    ///         <description>The cell type for this style instance. Cell types are accessed in the grid through the <see cref="GridModel.CellModels"/> property of a <see cref="GridModel"/> which returns a <see cref="GridCellModelBase"/> object. To access cell renderers, use the <see cref="GridControlBase.CellRenderers"/> property of a <see cref="GridControlBase"/> instance. (Default: Text Box)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellValue"/> (<see cref="System.Object"/>)</term>
    ///         <description>This property holds the cell value. Although the cell value is typically a string, it can also be any other primitive type such as int, byte, enum, or any custom type that is derived from <see cref="System.Object"/>. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellValueType"/> (<see cref="System.Type"/>)</term>
    ///         <description>Specifies the preferred <see cref="System.Type"/> for cell values. When you assign a value to the <see cref="GridStyleInfo"/> object, the value will be converted to this type. If the value cannot be converted, <see cref="GridStyleInfo.Error"/> will contain error information. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CharacterCasing"/> (<see cref="System.Windows.Forms.CharacterCasing"/>)</term>
    ///         <description>Specifies if cell control modifies the case of characters as they are typed when the cell's <see cref="GridStyleInfo.CellType"/> is "OriginalTextBox". (Default: CharacterCasing.Normal)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CheckBoxOptions"/> (<see cref="GridCheckBoxCellInfo"/>)</term>
    ///         <description>Gets / sets flat look and values that represent checked, unchecked, and indeterminated state of the check box. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ChoiceList"/> (<see cref="System.Collections.Specialized.StringCollection"/>)</term>
    ///         <description>Specifies items to be displayed in a drop-down list. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Clickable"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if the user can click on any cell button elements in this renderer. (Default: true)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Control"/> (<see cref="System.Windows.Forms.Control"/>)</term>
    ///         <description>A custom control you can associate with a cell. (Default: null)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CultureInfo"/> (<see cref="System.Globalization.CultureInfo"/>)</term>
    ///         <description>The culture information holds rules for parsing and formatting the cells value. (Default: null)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CurrencyEdit"/> (<see cref="GridCurrencyEditInfo"/>)</term>
    ///         <description>A nested object with currency text box properties for a cell.  (Default: GridCurrencyEditInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.DataSource"/> (<see cref="System.Object"/>)</term>
    ///         <description>Specifies a data source that holds items to be displayed in a drop-down list. A datasource can be specified instead of manually filling the choicelist with string entries. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Description"/> (<see cref="System.String"/>)</term>
    ///         <description>Gets / sets the text that is shown in check box or pushbuttons. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.DisplayMember"/> (<see cref="System.String"/>)</term>
    ///         <description>Names the property in the <see cref="GridStyleInfo.DataSource"/> that holds the text to be displayed in a cell that depends on a <see cref="GridStyleInfo.ValueMember"/>. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.DropDownStyle"/> (<see cref="GridDropDownStyle"/>)</term>
    ///         <description>Specifies if user input is restricted to items from the <see cref="GridStyleInfo.ChoiceList"/> or <see cref="GridStyleInfo.DataSource"/>. (Default: GridDropDownStyle.Editable)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Enabled"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if the cell can be activated as current cell or if the cell should be skipped when moving the current cell. (Default: True)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Error"/> (<see cref="System.String"/>)</term>
    ///         <description>Holds error information if a value could not be converted to the <see cref="System.Type"/> specified with <see cref="GridStyleInfo.CellValueType"/>. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ExclusiveChoiceList"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if user input is restricted to items from the <see cref="GridStyleInfo.ChoiceList"/> or <see cref="GridStyleInfo.DataSource"/>.  (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.FloatCell"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Gets / sets if text can float into the boundaries of a neighboring cell. (Default: True)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.FloodCell"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Gets / sets if this cell can be flooded by a previous cell. (Default: True)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Font"/> (<see cref="GridFontInfo"/>)</term>
    ///         <description>The font for drawing text. (Default: GridFontInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Format"/> (<see cref="System.String"/>)</term>
    ///         <description>Gets / sets the format mask for formatting the cell value. You can specify numeric format strings,
    /// date format strings, or enumeration format strings as discussed in the section "Format Specifiers and Format Providers" of the .NET Framework Developers Guide (see ms-help://MS.VSCC/MS.MSDNVS/cpguide/html/cpconformatspecifiersformatproviders.htm) (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.FormulaTag"/> (<see cref="GridFormulaTag"/>)</term>
    ///         <description>A formula tag that is associated with a cell. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.HorizontalAlignment"/> (<see cref="GridHorizontalAlignment"/>)</term>
    ///         <description>Specifies horizontal alignment of text in the cell. (Default: GridHorizontalAlignment.Left)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.HotkeyPrefix"/> (<see cref="System.Drawing.Text.HotkeyPrefix"/>)</term>
    ///         <description>Specifies how hot-key prefixes should be displayed. Hot-keys are indicated in text with an '&amp;' (ampersand). When you enable hot-key prefix, the specific characters can be displayed underlined or regular. The '&amp;' will not be displayed. (Default: HotkeyPrefix.Show)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ImageIndex"/> (<see cref="System.Int32"/>)</term>
    ///         <description>Specifies an index for an image in the <see cref="GridStyleInfo.ImageList"/> of a <see cref="GridStyleInfo"/>
    /// instance. (Default: -1)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ImageList"/> (<see cref="System.Windows.Forms.ImageList"/>)</term>
    ///         <description>The <see cref="GridStyleInfo.ImageList"/> that holds a collection of images. Cells can choose images with the <see cref="GridStyleInfo.ImageIndex"/> property in a <see cref="GridStyleInfo"/>
    /// instance. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Interior"/> (<see cref="Syncfusion.Drawing.BrushInfo"/>)</term>
    ///         <description> Lets you specify a solid backcolor, gradient, or pattern style with both back and forecolor for a cell's
    ///  background.  (Default: SystemColors.Window)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.MaskEdit"/> (<see cref="GridMaskEditInfo"/>)</term>
    ///         <description>A nested object with masked edit properties for a cell. (Default: GridMaskEditInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.MaxLength"/> (<see cref="System.Int32"/>)</term>
    ///         <description>Limits the number of characters the user can type into the cell. Note: When selecting text from a choice list or when pasting text, the text can be longer. Additional validation is necessary on your side. (Default: 0)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.MergeCell"/> (<see cref="GridMergeCellDirection"/>)</term>
    ///         <description>Specifies merge behavior for an individual cell when merging cells feature has been enabled in a <see cref="GridModel"/> with <see cref="GridModelOptions.MergeCellsMode"/>. (Default: GridMergeCellDirection.None)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.NumericUpDown"/> (<see cref="GridNumericUpDownCellInfo"/>)</term>
    ///         <description><see cref="GridStyleInfo.NumericUpDown"/> lets you specify the step, minimum, and maximum value
    /// and if the value should start over when you reach the maximum value. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.PasswordChar"/> (<see cref="System.Char"/>)</term>
    ///         <description>The character used to mask characters of a password in a password-entry cell. The cell's <see cref="GridStyleInfo.CellType"/> must be "OriginalTextBox". (Default: Blank)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ProgressBar"/> (<see cref="GridProgressBarInfo"/>)</term>
    ///         <description>A nested object with ProgressBar properties for a cell. Default: GridProgressBarInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ReadOnly"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if cell contents can be modified by the user. You can programmatically change Read-only cells by setting <see cref="GridModel.DiscardReadOnly"/> to True. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ShowButtons"/> (<see cref="GridShowButtons"/>)</term>
    ///         <description>Specifies when to show or display the cell buttons. Possible choices are: show the button only for the current cell, always show buttons, or never show buttons. (Default: GridShowButtons.Show)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.StrictValueType"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Indicates whether an exception should be thrown in the <see cref="ApplyFormattedText(string)"/> method if the formatted text can not be parsed and converted to the type specified with <see cref="GridStyleInfo.CellValueType"/>. (Default: True)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Tag"/> (<see cref="System.Object"/>)</term>
    ///         <description>A custom tag you can associate with a cell. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Text"/> (<see cref="System.String"/>)</term>
    ///         <description>Gets / sets the value as a string. If a <see cref="GridStyleInfo.CellValueType"/>
    /// is specified, the text will be parsed and converted to the type specified with
    /// <see cref="GridStyleInfo.CellValueType"/> using any <see cref="GridStyleInfo.CultureInfo"/>
    /// information.
    ///  (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.TextAlign"/> (<see cref="GridTextAlign"/>)</term>
    ///         <description>Align text left of button elements (which is typical for combo boxes). Or align text right of button elements. (Default: GridTextAlign.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.TextColor"/> (<see cref="System.Drawing.Color"/>)</term>
    ///         <description>Lets you specify the color for drawing the cell text. (Default: SystemColors.WindowText)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.TextMargins"/> (<see cref="GridMarginsInfo"/>)</term>
    ///         <description>Holds text margins in pixels. When drawing a cell, this specifies the empty area between the
    /// text rectangle and the client rectangle of the cell without borders and cell buttons. (Default: GridMarginsInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Themed"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if cell should be drawn using Windows XP themes when <see cref="GridControlBase.ThemesEnabled"/> has been set. (Default: True)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.TriState"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if this is a Tristate check box that has an additional indeterminated state. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Trimming"/> (<see cref="System.Drawing.StringTrimming"/>)</term>
    ///         <description>Indicates how text is trimmed when it exceeds the edges of the cell text rectangle. (Default: StringTrimming.Character)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ValidateValue"/> (<see cref="GridCellValidateValueInfo"/>)</term>
    ///         <description>Holds validation rules for the cell value that are being checked before any user changes are committed to the grid cells style object. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ValueMember"/> (<see cref="System.String"/>)</term>
    ///         <description>Gets / sets a string that specifies the property of the data source from which to draw the value. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.VerticalAlignment"/> (<see cref="GridVerticalAlignment"/>)</term>
    ///         <description>Specifies vertical alignment of text in the cell. (Default: GridVerticalAlignment.Top)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.VerticalScrollbar"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if text box should show a vertical scrollbar when text is being edited and does not fit in cell. WrapText must be initialized to True. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.WrapText"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if text should be wrapped when it does not fit into a single line. (Default: True)</description>
    ///     </item>
    /// </list>
    /// <para/>
    /// <para/>
    /// <para/>
    /// </remarks>
    /// <para/>
    /// <example>
    /// The following example makes some changes to the grid using the indexer:
    /// <code lang="C#">
    ///             model[2, 2].Text = "Grid Demo";
    ///             model[2, 2].Font.Bold = True;
    ///             model[2, 2].Font.Size = 16;
    ///             model[2, 2].HorizontalAlignment = GridHorizontalAlignment.Center;
    ///             model[2, 2].VerticalAlignment = GridVerticalAlignment.Middle;
    ///             model[2, 2].CellType = "Static";
    ///             model[2, 2].Borders.All = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(100, 238, 122, 3));
    ///             model[2, 2].Interior = new BrushInfo(GradientStyle.PathEllipse, Color.FromArgb(100, 57, 73, 122), Color.FromArgb(237, 240, 247));
    /// </code>
    /// If you query for specific attributes in a cell and these attributes have not been explicitly set for the cell,
    /// the <see cref="GridStyleInfo"/> object that is return by the indexer is smart enough to query base styles for
    /// queried information.
    /// <code lang="C#">
    ///             GridStyleInfo standard = model.BaseStylesMap["Standard"].StyleInfo;
    ///             standard.TextColor = Color.FromArgb(0, 21, 84);
    ///                 Color color = model[1, 1].TextColor;
    ///                 // model[1, 1].TextColor will return Color.FromArgb(0, 21, 84));
    /// </code>
    /// </example>
    [TypeConverter(typeof(GridStyleInfoConverter))]
    public class GridStyleInfo : StyleInfoBase
    {
        // Static Fields.
        private static GridStyleInfo defaultStyle = null;

        internal bool Locked = false;

        /// <summary>
        /// An empty style object.
        /// </summary>
        public static readonly GridStyleInfo Empty = new GridStyleInfo();

        // Constructors.
        static GridStyleInfo()
        {
        }

        /// <summary>
        /// Initalizes a new style object.
        /// </summary>
        [DebuggerStepThrough()]
        public GridStyleInfo()
            : base(new GridStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new style object and copies all data from an existing style object.
        /// </summary>
        /// <param name="style">The style object that contains the original data.</param>
        [DebuggerStepThrough()]
        public GridStyleInfo(GridStyleInfo style)
            : base(style.Store)
        {
        }

        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="GridStyleInfoStore"/>.
        /// </summary>
        /// <param name="store">A <see cref="GridStyleInfoStore"/> that holds data for this <see cref="GridStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridStyleInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public GridStyleInfo(GridStyleInfoStore store)
            : base(store)
        {
        }

        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="GridStyleInfoIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoIdentity"/> that holds the indentity for this <see cref="GridStyleInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public GridStyleInfo(StyleInfoIdentityBase identity)
            : base(identity, new GridStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="GridStyleInfoIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoIdentity"/> that holds the indentity for this <see cref="GridStyleInfo"/>.
        /// </param>
        /// <param name="store">A <see cref="GridStyleInfoStore"/> that holds data for this <see cref="GridStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridStyleInfoStore"/> object.
        /// </param>
        [DebuggerStepThrough()]
        public GridStyleInfo(StyleInfoIdentityBase identity, GridStyleInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="GridStyleInfoIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoIdentity"/> that holds the indentity for this <see cref="GridStyleInfo"/>.
        /// </param>
        /// <param name="store">A <see cref="GridStyleInfoStore"/> that holds data for this <see cref="GridStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridStyleInfoStore"/> object.
        /// </param>
        /// <param name="cacheValues"> set the bool value for cacheValues </param>
        [DebuggerStepThrough()]
        public GridStyleInfo(StyleInfoIdentityBase identity, GridStyleInfoStore store, bool cacheValues)
            : base(identity, store, cacheValues)
        {
        }

        /// <summary>
        /// Gets or sets identity information such as row and column index for the current <see cref="GridStyleInfo"/>.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridStyleInfoIdentity CellIdentity
        {
            get
            {
                return base.Identity as GridStyleInfoIdentity;
            }

            set
            {
                base.Identity = value;
            }
        }

        /// <summary>
        /// Returns the <see cref="GridModel"/> for this style or NULL if style is used outside a grid model.
        /// </summary>
        /// <returns>The <see cref="GridModel"/> this style belongs to or NULL.</returns>
        public GridModel GetGridModel()
        {
            GridStyleInfoIdentity cellId = base.Identity as GridStyleInfoIdentity;
            GridModel grid = (cellId != null) ? cellId.GetGridModel() : null;
            return grid;
        }

        /// <summary>
        /// Returns the <see cref="GridControlBase"/> for this style or NULL if style is used outside a grid model.
        /// </summary>
        /// <returns>The active <see cref="GridControlBase"/> for the grid model this style belongs to or NULL.</returns>
        public GridControlBase GetActiveGridView()
        {
            GridStyleInfoIdentity cellId = base.Identity as GridStyleInfoIdentity;
            GridControlBase grid = (cellId != null) ? cellId.GetActiveGridView() : null;
            return grid;
        }

        /// <summary>
        /// Gets the <see cref="GridStyleInfoStore"/> object that holds all the data for this style object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new GridStyleInfoStore Store
        {
            get { return (GridStyleInfoStore)base.Store; }
        }

        /// <override/>
        /// <summary>
        /// Applies the specified changes to the style object.
        /// </summary>
        /// <param name="istyle">The style object to modify.</param>
        /// <param name="mt">Defines the style operations for the given style object.</param>
        public override void ModifyStyle(IStyleInfo istyle, StyleModifyType mt)
        {
            GridStyleInfoIdentity cellId = base.Identity as GridStyleInfoIdentity;
            if (cellId == null || cellId.OffLine || this.Updating)
            {
                base.ModifyStyle(istyle, mt);
            }
            else
            {
                GridStyleInfo style = istyle as GridStyleInfo;
                if (style == null)
                {
                    GridStyleInfoStore store = istyle as GridStyleInfoStore;
                    if (store != null)
                    {
                        style = new GridStyleInfo(store);
                    }
                }

                cellId.GetGridModel().SetCellInfo(cellId.RowIndex, cellId.ColIndex, style, mt);
            }
        }

        ////        ///// <summary>
        ////        ///// Holds identity information such as base style row and column index for the current <see cref="GridStyleInfo"/>.
        ////        ///// </summary>
        ////        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ////        public GridBaseStyleIdentity BaseStyleIdentity
        ////        {
        ////            get
        ////            {
        ////                return base.Identity as GridBaseStyleIdentity;
        ////            }
        ////            set
        ////            {
        ////                base.Identity = value;
        ////            }
        ////        }
        ////

        /// <override/>
        /// <summary>
        /// Creates a product-specific identity object for the sub object.
        /// </summary>
        /// <param name="sip">StyleInfoProperty descriptor for this sub object.</param>
        /// <returns>Identity for the sub object.</returns>
        [DebuggerStepThrough()]
        public override StyleInfoSubObjectIdentity CreateSubObjectIdentity(StyleInfoProperty sip)
        {
            return new GridStyleInfoSubObjectIdentity(this, sip);
        }

        /// <summary>
        /// Creates a new <see cref="GridStyleInfo"/> and copies its cell and identity information from the current object. The new
        /// instance will be made offline so that changes in this style object are not be stored in the GridData
        /// </summary>
        /// <returns>A new <see cref="GridStyleInfo"/> instance.</returns>
        /// <remarks>
        /// Lets a style object load base styles and default values but disables
        /// saving changes back to the grid. (see OnStyleChanged below)
        /// </remarks>
        [DebuggerStepThrough()]
        public GridStyleInfo GetOffLineCopy()
        {
            return new GridStyleInfo(((GridStyleInfoIdentity)Identity).MakeOfflineIdentity(), (GridStyleInfoStore)Store.Clone());
        }

        GridStyleInfoCustomPropertiesCollection cpl = null;

        /// <summary>
        /// Gets a collection of custom property objects that have
        /// at least one initialized value. The primary purpose of this
        /// collection is to support design-time code serialization of
        /// custom properties.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridStyleInfoCustomPropertiesCollection CustomProperties
        {
            get
            {
                if (cpl == null)
                {
                    cpl = new GridStyleInfoCustomPropertiesCollection(this);
                }

                return cpl;
            }
        }

        bool ShouldSerializeCustomProperties()
        {
            return CustomProperties.Count > 0;
        }

        /// <override/>
        protected override void OnStyleChanged(StyleInfoProperty sip)
        {
            if (Locked)
            {
                return;
            }

            ////throw new InvalidOperationException("This style has been Locked and can not be changed. Set GridControlBase.SupportsPrepareViewStyleInfo = true to support this kind of operation.");
            cpl = null;
            base.OnStyleChanged(sip);
        }

        // Default

        /// <summary>
        /// Gets a <see cref="GridStyleInfo"/> with default settings.
        /// </summary>
        public static GridStyleInfo Default
        {
            get
            {
                if (GridStyleInfo.defaultStyle == null)
                {
                    defaultStyle = new GridStyleInfo();
                    defaultStyle.TextColor = SystemColors.WindowText;
                    defaultStyle.ReadOnly = false;
                    defaultStyle.Themed = true;
                    ////defaultStyle.Locked = false;
                    defaultStyle.Font = GridFontInfo.Default;
                    defaultStyle.Borders = GridBordersInfo.Default;

                    defaultStyle.BackgroundImageID = string.Empty;
                    defaultStyle.BackgroundImage = null;
                    defaultStyle.BackgroundImageMode = GridBackgroundImageMode.Normal;

                    defaultStyle.HorizontalAlignment = GridHorizontalAlignment.Left;
                    defaultStyle.CharacterCasing = CharacterCasing.Normal;
                    defaultStyle.PasswordChar = ' ';
                    defaultStyle.CellTipText = string.Empty;
                    defaultStyle.VerticalAlignment = GridVerticalAlignment.Top;
                    defaultStyle.CellType = "TextBox";
                    defaultStyle.Error = string.Empty;
                    defaultStyle.Interior = new BrushInfo(SystemColors.Window);
                    ////                    defaultStyle.Borders.Top = new GridBorder(GridBorderStyle.Standard, SystemColors.WindowFrame, GridBorderWeight.Thin);
                    ////                    defaultStyle.Borders.Left = new GridBorder(GridBorderStyle.Standard, SystemColors.WindowFrame, GridBorderWeight.Thin);
                    ////                    defaultStyle.Borders.Bottom = new GridBorder(GridBorderStyle.Standard, SystemColors.WindowFrame, GridBorderWeight.Thin);
                    ////                    defaultStyle.Borders.Right = new GridBorder(GridBorderStyle.Standard, SystemColors.WindowFrame, GridBorderWeight.Thin);
                    defaultStyle.VerticalScrollbar = false;
                    defaultStyle.MaxLength = 0;
                    defaultStyle.CellAppearance = GridCellAppearance.Flat;

                    defaultStyle.TextColor = SystemColors.WindowText;
                    defaultStyle.WrapText = true;
                    defaultStyle.WrapRotatedText = false;
                    defaultStyle.Trimming = StringTrimming.Character;
                    defaultStyle.AutoSize = false;
                    defaultStyle.AllowEnter = false;
                    defaultStyle.Enabled = true;
                    defaultStyle.TriState = false;
                    ////defaultStyle.ExclusiveChoiceList = false;
                    ////defaultStyle.AutoComplete = false;
                    defaultStyle.DropDownStyle = GridDropDownStyle.Editable;
                    defaultStyle.Clickable = true;
                    defaultStyle.BaseStyle = string.Empty;

                    defaultStyle.Description = string.Empty;
                    ////defaultStyle.ChoiceList = new StringCollection();
                    defaultStyle.ImageList = null;
                    defaultStyle.ImageIndex = -1;
                    defaultStyle.DataSource = null;
                    defaultStyle.DisplayMember = string.Empty;
                    defaultStyle.ValueMember = string.Empty;
                    defaultStyle.PropertyDescriptor = null;

                    defaultStyle.FloatCell = true;
                    defaultStyle.FloodCell = true;
                    defaultStyle.MergeCell = GridMergeCellDirection.None;

                    defaultStyle.RightToLeft = RightToLeft.Inherit;
                    defaultStyle.ImageFromByteArray = true;

                    defaultStyle.Format = string.Empty;
                    defaultStyle.FormulaTag = null;
                    defaultStyle.CellValueType = null;
                    defaultStyle.StrictValueType = true;
                    defaultStyle.CultureInfo = null;
                    defaultStyle.CellValue = string.Empty;
                    defaultStyle.ShowButtons = GridShowButtons.Show;
                    defaultStyle.TextAlign = GridTextAlign.Default;
                    defaultStyle.HotkeyPrefix = HotkeyPrefix.None;
                    defaultStyle.CheckBoxOptions = GridCheckBoxCellInfo.Default;
                    defaultStyle.ValidateValue = GridCellValidateValueInfo.Default;
                    defaultStyle.NumericUpDown = GridNumericUpDownCellInfo.Default;
                    defaultStyle.TextMargins = GridMarginsInfo.Default;
                    defaultStyle.BorderMargins = GridMarginsInfo.Empty;
                    defaultStyle.MaskEdit = GridMaskEditInfo.Default;
                    defaultStyle.ProgressBar = GridProgressBarInfo.Default;
                    defaultStyle.CurrencyEdit = GridCurrencyEditInfo.Default;
                    defaultStyle.ImageSizeMode = GridImageSizeMode.CenterImage;
                    defaultStyle.AutoCompleteInEditMode = GridComboSelectionOptions.Both;
                    defaultStyle.AutoFitChar = '#';
                    defaultStyle.AutoFit = AutoFitOptions.None;
                }

                return GridStyleInfo.defaultStyle;
            }
        }

        /// <summary>
        /// Override this method to return a default style object for your derived class.
        /// </summary>
        /// <returns>A default style object.</returns>
        /// <override/>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }

        /// <summary>
        /// Gets the associated <see cref="GridCellModelBase"/> for this style object.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridCellModelBase CellModel
        {
            get
            {
                GridStyleInfoIdentity gsid = Identity as GridStyleInfoIdentity;
                if (gsid != null)
                {
                    return gsid.LookupCellModel(this.CellType);
                }

                return null;
            }
        }

        // Properties.
        #region Font
        /// <summary>
        /// Gets returns or creates a cached GDI+ font generated from font information of
        /// the <see cref="GridStyleInfo.Font"/> object.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [ReadOnly(true)]
        public Font GdipFont
        {
            [DebuggerStepThrough()]
            get
            {
                if (HasFont && !Font.IsEmpty)
                {
                    return Font.GdipFont;
                }

                if (Identity != null)
                {
                    GridStyleInfo fontInfo = this.Identity.GetBaseStyleNotEmptyExpandable(this, GridStyleInfoStore.FontProperty) as GridStyleInfo;
                    if (fontInfo != null)
                    {
                        return fontInfo.GdipFont;
                    }
                }

                return Default.GdipFont;
            }
        }

        /// <internalonly/>
        /// <summary>Gets readonly font. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridFontInfo ReadOnlyFont
        {
            ////[DebuggerStepThrough()]
            get
            {
                if (HasFont && !Font.IsEmpty)
                {
                    return Font;
                }

                if (Identity != null)
                {
                    GridStyleInfo fontInfo = this.Identity.GetBaseStyleNotEmptyExpandable(this, GridStyleInfoStore.FontProperty) as GridStyleInfo;
                    if (fontInfo != null)
                    {
                        if (fixSubObjectsDerivedFromRowandColStyle && IsRowOrColumnStyle(fontInfo))
                        {
                            return Font;
                        }

                        return fontInfo.Font;
                    }
                }

                return Default.Font;
            }
        }

        /// <summary>
        /// Gets or sets the font for drawing text.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the font property is GridFontInfo.Default.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="PushButton"/>  (<see cref="GridPushButtonCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("The font for drawing text."),
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        SRCategory("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public GridFontInfo Font
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridFontInfo)GetValue(GridStyleInfoStore.FontProperty);
            }

            set
            {
                SetValue(GridStyleInfoStore.FontProperty, value);
            }
        }

        /// <summary>
        /// Resets font information.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetFont()
        {
            ResetValue(GridStyleInfoStore.FontProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeFont()
        {
            return HasValue(GridStyleInfoStore.FontProperty);
        }

        /// <summary>
        /// Gets a value indicating whether font information has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasFont
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.FontProperty);
            }
        }

        #endregion
        #region Borders
        /// <summary>
        /// Gets or sets Top, left, bottom, and right border settings.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the Borders property is GridBordersInfo.Default.
        /// <para/>
        /// The property affects the behavior or appearance of the following cell types:
        /// <para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DataBoundRowHeader"/>  (<see cref="GridDataBoundRowHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="PushButton"/>  (<see cref="GridPushButtonCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Top, left, bottom, and right border settings."),
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        SRCategory("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public GridBordersInfo Borders
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridBordersInfo)GetValue(GridStyleInfoStore.BordersProperty);
            }

            set
            {
                SetValue(GridStyleInfoStore.BordersProperty, value);
            }
        }

        /// <internalonly/>
        /// <summary>Gets readonly readonly borders. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridBordersInfo ReadOnlyBorders
        {
            ////[DebuggerStepThrough()]
            get
            {
                if (HasBorders && !Borders.IsEmpty)
                {
                    return Borders;
                }

                if (Identity != null)
                {
                    GridStyleInfo bordersInfo = this.Identity.GetBaseStyleNotEmptyExpandable(this, GridStyleInfoStore.BordersProperty) as GridStyleInfo;
                    if (bordersInfo != null)
                    {
                        if (fixSubObjectsDerivedFromRowandColStyle && IsRowOrColumnStyle(bordersInfo))
                        {
                            return Borders;
                        }

                        return bordersInfo.Borders;
                    }
                }

                return Default.Borders;
            }
        }

        bool IsRowOrColumnStyle(GridStyleInfo info)
        {
            if (info.CellIdentity != null
                && ((info.CellIdentity.ColIndex == -1 && info.CellIdentity.RowIndex >= 0)
                || (info.CellIdentity.ColIndex >= 0 && info.CellIdentity.RowIndex == -1)))
            {
                return true;
            }

            return false;
        }

        static bool fixSubObjectsDerivedFromRowandColStyle = true;

        /// <summary>
        /// Gets or sets a value indicating whether the cells are derived from row and column styles. In the case that the base style that implements
        /// a specific subobject (e.g. Borders, Font, ...) is a row style
        /// the column style would be ignored if we return here the subobject row of the row style
        /// in the optimized ReadOnlyBorders property. Returning the normal Borders objects instead
        /// fixes this problem.
        /// </summary>
        /// <remarks>
        /// Default value is true. You can set it false if you relied on previous fault behavior.
        /// </remarks>
        /// <example>
        /// <code lang="C#">
        /// this.gridControl1.Model.RowStyles[row - 1].Borders.Bottom = new GridBorder(GridBorderStyle.Solid, c, GridBorderWeight.ExtraExtraThick);
        /// this.gridControl1.Model.ColStyles[col - 1].Borders.Right = new GridBorder(GridBorderStyle.Solid, c, GridBorderWeight.ExtraExtraThick);
        /// GridBordersInfo b = this.gridControl1.Model[row - 1, col - 1].ReadOnlyBorders;
        /// Console.WriteLine(b.Right); // will return Default since ReadOnlyBorders is RowStyles[row - 1].Borders which
        /// has no knowledge about ColStyles if FixSubObjectsDerivedFromRowandColStyle = false.
        ///  <para/>
        /// Setting FixSubObjectsDerivedFromRowandColStyle = true fixes the problem.
        /// </code>
        /// </example>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), System.Xml.Serialization.XmlIgnore]
        public static bool FixSubObjectsDerivedFromRowandColStyle
        {
            get
            {
                return fixSubObjectsDerivedFromRowandColStyle;
            }

            set
            {
                fixSubObjectsDerivedFromRowandColStyle = value;
            }
        }

        /// <summary>
        /// Resets the borders information.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetBorders()
        {
            ResetValue(GridStyleInfoStore.BordersProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBorders()
        {
            return HasValue(GridStyleInfoStore.BordersProperty);
        }

        /// <summary>
        /// Gets a value indicating whether border information has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBorders
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.BordersProperty);
            }
        }

        #endregion
        #region Interior
        /// <summary>
        /// Gets or sets a solid backcolor, gradient, or pattern style with both back and forecolor for a cell's background.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the Interior property is SystemColors.Window.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DataBoundRowHeader"/>  (<see cref="GridDataBoundRowHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="PushButton"/>  (<see cref="GridPushButtonCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Lets you specify a solid backcolor, gradient, or pattern style with both back and forecolor for a cell's background."),
        Browsable(true),
        SRCategory("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public BrushInfo Interior
        {
            [DebuggerStepThrough()]
            get
            {
                return (BrushInfo)GetValue(GridStyleInfoStore.InteriorProperty);
            }

            set
            {
                SetValue(GridStyleInfoStore.InteriorProperty, value);
            }
        }

        /// <summary>
        /// Resets interior information.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetInterior()
        {
            ResetValue(GridStyleInfoStore.InteriorProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeInterior()
        {
            return HasValue(GridStyleInfoStore.InteriorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether interior information has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasInterior
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.InteriorProperty);
            }
        }

        /// <summary>
        /// Gets or sets a shortcut to get the backcolor of a cell instead of using <see cref="GridStyleInfo.Interior"/>.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Provides a shortcut to get the backcolor of a cell instead of using Interior.")]
        [SRCategory("StyleCategoryAppearance")]
        [NotifyParentProperty(true)]
        public Color BackColor
        {
            [DebuggerStepThrough()]
            get
            {
                return Interior.BackColor;
            }

            [DebuggerStepThrough()]
            set
            {
                Interior = new BrushInfo(value);
            }
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBackColor()
        {
            return ShouldSerializeInterior();
        }

        /// <summary>
        /// Resets the <see cref="Interior"/> back to default.
        /// </summary>
        public void ResetBackColor()
        {
            ResetInterior();
        }

        #endregion
        #region TextColor
        /// <summary>
        /// Gets or sets the color for drawing the cell text.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the TextColor property is SystemColors.WindowText.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DataBoundRowHeader"/>  (<see cref="GridDataBoundRowHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Contains text color of a cell."),
        Browsable(true),
        SRCategory("StyleCategoryAppearance")]
        [NotifyParentProperty(true)]
        public Color TextColor
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color)GetValue(GridStyleInfoStore.TextColorProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.TextColorProperty, value);
            }
        }

        /// <summary>
        /// Resets text color information.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetTextColor()
        {
            ResetValue(GridStyleInfoStore.TextColorProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeTextColor()
        {
            return HasValue(GridStyleInfoStore.TextColorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether text color has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTextColor
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.TextColorProperty);
            }
        }

        #endregion
        #region ReadOnly
        /// <summary>
        /// Gets or sets a value indicating whether cell contents can be modified by the user. You can programmatically change Read-only cells by setting <see cref="GridModel.IgnoreReadOnly"/> to True.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the Read-Only property is False.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Specifies if cell contents can be modified by the user. You can programmatically change Read-only cells by setting IgnoreReadOnly to True."),
        Browsable(true),
        SRCategory("StyleCategoryBehavior")]
        [NotifyParentProperty(true)]
        public bool ReadOnly
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridStyleInfoStore.ReadOnlyProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.ReadOnlyProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets Read-only information.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetReadOnly()
        {
            ResetValue(GridStyleInfoStore.ReadOnlyProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeReadOnly()
        {
            return HasValue(GridStyleInfoStore.ReadOnlyProperty);
        }

        /// <summary>
        /// Gets a value indicating whether Read-only information has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasReadOnly
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.ReadOnlyProperty);
            }
        }
        #endregion
        #region RightToLeft
        /// <summary>
        /// Gets or sets if cell contents read from right to left.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the RightToLeft property is False.<para/>
        /// <para/>
        /// </remarks>
        [Description("Specifies if cell content reads from right to left."),
        Browsable(true),
        SRCategory("StyleCategoryBehavior")]
        [NotifyParentProperty(true)]
        public RightToLeft RightToLeft
        {
            get
            {
                return (RightToLeft)GetShortValue(GridStyleInfoStore.RightToLeftProperty);
            }

            set
            {
                SetValue(GridStyleInfoStore.RightToLeftProperty, value);
            }
        }

        /// <summary>
        /// Resets Read-only information.
        /// </summary>
        public void ResetRightToLeft()
        {
            ResetValue(GridStyleInfoStore.RightToLeftProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeRightToLeft()
        {
            return HasRightToLeft;
        }

        /// <summary>
        /// Gets a value indicating whether Read-only information has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasRightToLeft
        {
            get
            {
                return HasValue(GridStyleInfoStore.RightToLeftProperty);
            }
        }
        #endregion
        #region Themed
        /// <summary>
        /// Gets or sets a value indicating whether cell types that support Windows XP themes should be drawn themed.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the Themed property is True.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DataBoundRowHeader"/>  (<see cref="GridDataBoundRowHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="PushButton"/>  (<see cref="GridPushButtonCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="RichText"/>  (<see cref="GridRichTextBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ProgressBar"/>  (<see cref="GridProgressBarCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Specifies if cell types that support Windows XP themes should be drawn themed."),
        Browsable(true),
        SRCategory("StyleCategoryAppearance")]
        [NotifyParentProperty(true)]
        public bool Themed
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridStyleInfoStore.ThemedProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.ThemedProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets Read-only information.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetThemed()
        {
            ResetValue(GridStyleInfoStore.ThemedProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeThemed()
        {
            return HasValue(GridStyleInfoStore.ThemedProperty);
        }

        /// <summary>
        /// Gets a value indicating whether Read-only information has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasThemed
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.ThemedProperty);
            }
        }

        #endregion
        ////        #region Locked
        ////        /// <summary>
        ////        /// Specifies if cells can be dragged or cut.
        ////        /// </summary>
        ////        [
        ////        Description("Specifies if cell types that support Windows XP themes should be drawn themed."),
        ////        Browsable(true),
        ////        SRCategory("StyleCategoryAppearance")
        ////        ]
        ////        public bool Locked
        ////        {
        ////            [DebuggerStepThrough()]
        ////            get
        ////            {
        ////                return GetShortValue(GridStyleInfoStore.LockedProperty) != 0;
        ////            }[DebuggerStepThrough()]
        ////            set
        ////            {
        ////                SetValue(GridStyleInfoStore.LockedProperty, value ? 1 : 0);
        ////            }
        ////        }
        ////        ///// <summary>
        ////        ///// Resets Read-only information.
        ////        ///// </summary>
        ////        [DebuggerStepThrough() ] public void ResetLocked()
        ////        {
        ////            ResetValue(GridStyleInfoStore.LockedProperty);
        ////        }
        ////        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        ////        private bool ShouldSerializeLocked()
        ////        {
        ////            return HasValue(GridStyleInfoStore.LockedProperty);
        ////        }
        ////
        ////        ///// <summary>
        ////        ///// Determines if Read-only information has been initialized for the current object.
        ////        ///// </summary>
        ////        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ////        public bool HasLocked
        ////        {
        ////            [DebuggerStepThrough()]
        ////            get
        ////            {
        ////                return HasValue(GridStyleInfoStore.LockedProperty);
        ////            }
        ////        }
        ////        #endregion
        #region Clickable
        /// <summary>
        /// Gets or sets a value indicating whether the user can click on any cell button elements in this renderer.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the Clickable property is True.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="PushButton"/>  (<see cref="GridPushButtonCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Specifies if the user can click on any cell button elements in this renderer."),
        Browsable(true),
        SRCategory("StyleCategoryBehavior")]
        [NotifyParentProperty(true)]
        public bool Clickable
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridStyleInfoStore.ClickableProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.ClickableProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets the clickable information.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetClickable()
        {
            ResetValue(GridStyleInfoStore.ClickableProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeClickable()
        {
            return HasValue(GridStyleInfoStore.ClickableProperty);
        }

        /// <summary>
        /// Gets a value indicating whether clickable information has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasClickable
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.ClickableProperty);
            }
        }
        #endregion
        #region HotkeyPrefix
        /// <summary>
        /// Gets or sets how hot-key prefixes should be displayed. Hot-keys are indicated in text with an '&amp;' (ampersand).
        /// When you enable the hot-key prefix, the specific characters can be displayed underlined or regular. The '&amp;' will not
        /// be displayed.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the HotkeyPrefix property is HotkeyPrefix.None.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="PushButton"/>  (<see cref="GridPushButtonCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Contains hotkey prefix information of a cell."),
        Browsable(true),
        SRCategory("StyleCategoryAppearance")]
        [NotifyParentProperty(true)]
        public HotkeyPrefix HotkeyPrefix
        {
            [DebuggerStepThrough()]
            get
            {
                return (HotkeyPrefix)GetShortValue(GridStyleInfoStore.HotkeyPrefixProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.HotkeyPrefixProperty, (short)value);
            }
        }

        /// <summary>
        /// Resets the <see cref="HotkeyPrefix"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetHotkeyPrefix()
        {
            ResetValue(GridStyleInfoStore.HotkeyPrefixProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeHotkeyPrefix()
        {
            return HasValue(GridStyleInfoStore.HotkeyPrefixProperty);
        }
        
        /// <summary>
        /// Gets a value indicating whether the <see cref="HotkeyPrefix"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasHotkeyPrefix
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.HotkeyPrefixProperty);
            }
        }
        #endregion
        #region Trimming
        /// <summary>
        /// Gets or sets how text is trimmed when it exceeds the edges of the cell text rectangle.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the Trimming property is StringTrimming.Character.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="PushButton"/>  (<see cref="GridPushButtonCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Indicates how text is trimmed when it exceeds the edges of the cell text rectangle."),
        Browsable(true),
        SRCategory("StyleCategoryAppearance")]
        [NotifyParentProperty(true)]
        public StringTrimming Trimming
        {
            [DebuggerStepThrough()]
            get
            {
                return (StringTrimming)GetShortValue(GridStyleInfoStore.TrimmingProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.TrimmingProperty, (short)value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.Trimming"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetTrimming()
        {
            ResetValue(GridStyleInfoStore.TrimmingProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeTrimming()
        {
            return HasValue(GridStyleInfoStore.TrimmingProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.Trimming"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTrimming
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.TrimmingProperty);
            }
        }
        #endregion
        #region HorizontalAlignment
        /// <summary>
        /// Gets or sets horizontal alignment of text in the cell.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the HorizontalAlignment property is GridHorizontalAlignment.Left.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Specifies horizontal alignment of text in the cell."),
        Browsable(true),
        SRCategory("StyleCategoryAppearance")]
        [NotifyParentProperty(true)]
        public GridHorizontalAlignment HorizontalAlignment
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridHorizontalAlignment)GetShortValue(GridStyleInfoStore.HorizontalAlignmentProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.HorizontalAlignmentProperty, (short)value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.HorizontalAlignment"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetHorizontalAlignment()
        {
            ResetValue(GridStyleInfoStore.HorizontalAlignmentProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeHorizontalAlignment()
        {
            return HasValue(GridStyleInfoStore.HorizontalAlignmentProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.HorizontalAlignment"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasHorizontalAlignment
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.HorizontalAlignmentProperty);
            }
        }

        #endregion
        #region CharacterCasing
        /// <summary>
        /// Gets or sets if cell control modifies the case of characters as they are typed when
        /// the cell's <see cref="GridStyleInfo.CellType"/> is "OriginalTextBox".
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the CharacterCasing property is CharacterCasing.Normal.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="OriginalTextBox"/>  (<see cref="GridOriginalTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Specifies if cell control modifies the case of characters as they are typed."),
        Browsable(true),
        SRCategory("StyleCategoryAppearance")]
        [NotifyParentProperty(true)]
        public CharacterCasing CharacterCasing
        {
            [DebuggerStepThrough()]
            get
            {
                return (CharacterCasing)GetShortValue(GridStyleInfoStore.CharacterCasingProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.CharacterCasingProperty, (short)value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.CharacterCasing"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetCharacterCasing()
        {
            ResetValue(GridStyleInfoStore.CharacterCasingProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCharacterCasing()
        {
            return HasValue(GridStyleInfoStore.CharacterCasingProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.CharacterCasing"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCharacterCasing
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.CharacterCasingProperty);
            }
        }
        #endregion
        #region VerticalAlignment
        /// <summary>
        /// Gets or sets vertical alignment of text in the cell.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the VerticalAlignment property is GridVerticalAlignment.Top.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Specifies vertical alignment of text in the cell."),
        Browsable(true),
        SRCategory("StyleCategoryAppearance")]
        [NotifyParentProperty(true)]
        public GridVerticalAlignment VerticalAlignment
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridVerticalAlignment)GetShortValue(GridStyleInfoStore.VerticalAlignmentProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.VerticalAlignmentProperty, (short)value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.VerticalAlignment"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetVerticalAlignment()
        {
            ResetValue(GridStyleInfoStore.VerticalAlignmentProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeVerticalAlignment()
        {
            return HasValue(GridStyleInfoStore.VerticalAlignmentProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.VerticalAlignment"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasVerticalAlignment
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.VerticalAlignmentProperty);
            }
        }
        #endregion

        #region RadioButtonAlignment
        /// <summary>
        /// Gets or sets the Alignment of the radio button elements inside the cell rectangle.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the RadioButtonAlignment property is ButtonAlignment.Horizontal.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="RadioButton"/>  (<see cref="GridRadioButtonCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Gets or sets the Alignment of the radio button elements inside the cell rectangle"),
        Browsable(true),
        SRCategory("StyleCategoryAppearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [NotifyParentProperty(true)]
        public ButtonAlignment RadioButtonAlignment
        {
            [DebuggerStepThrough()]
            get
            {
                return (ButtonAlignment)GetShortValue(GridStyleInfoStore.RadioButtonAlignmentProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.RadioButtonAlignmentProperty, (short)value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.ButtonAlignment"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetRadioButtonAlignment()
        {
            ResetValue(GridStyleInfoStore.RadioButtonAlignmentProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeRadioButtonAlignment()
        {
            return HasValue(GridStyleInfoStore.RadioButtonAlignmentProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.ButtonAlignment"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasRadioButtonAlignment
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.RadioButtonAlignmentProperty);
            }
        }
        #endregion
        #region TextAlign
        /// <summary>
        /// Gets or sets Align text left of button elements (which is typical for combo boxes). Or align text right of button elements.
        /// See <see cref="HorizontalAlignment"/> how to align text left, centered, and right inside a cell rectangle.
        /// </summary>
        /// <remarks>
        /// Don't confuse this with <see cref="HorizontalAlignment"/>. <see cref="TextAlign"/> specifies if text should be
        /// displayed left or right of any cell button elements.
        /// <para/>
        /// <para/>
        /// The default value for the TextAlign property is GridTextAlign.Default.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Align text left of button elements (which is typical for combo boxes). Or align text right of button elements."),
        Browsable(true),
        SRCategory("StyleCategoryAppearance")]
        [NotifyParentProperty(true)]
        public GridTextAlign TextAlign
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridTextAlign)GetShortValue(GridStyleInfoStore.TextAlignProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.TextAlignProperty, (short)value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.TextAlign"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetTextAlign()
        {
            ResetValue(GridStyleInfoStore.TextAlignProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeTextAlign()
        {
            return HasValue(GridStyleInfoStore.TextAlignProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.TextAlign"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTextAlign
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.TextAlignProperty);
            }
        }
        #endregion
        #region BaseStyle
        /// <summary>
        /// Gets or sets the base style for this style instance with default values for properties that are not initialized for this style object.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the BaseStyle property is String.Empty.
        /// <para/>
        /// The property affects the behavior or appearance of the following cell types:
        /// <para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DataBoundRowHeader"/>  (<see cref="GridDataBoundRowHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="PushButton"/>  (<see cref="GridPushButtonCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("The base style for this style instance with default values for properties that are not initialized for this style object."),
        Browsable(true),
        TypeConverter(typeof(GridBaseStyleNameConverter)),
        RefreshProperties(RefreshProperties.Repaint),
        SRCategory("StyleCategoryStyle")]
        [NotifyParentProperty(true)]
        public string BaseStyle
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridStyleInfoStore.BaseStyleProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.BaseStyleProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.BaseStyle"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetBaseStyle()
        {
            ResetValue(GridStyleInfoStore.BaseStyleProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBaseStyle()
        {
            return HasValue(GridStyleInfoStore.BaseStyleProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.BaseStyle"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBaseStyle
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.BaseStyleProperty);
            }
        }
        #endregion
        #region WrapText
        /// <summary>
        /// Gets or sets a value indicating whether text should be wrapped when it does not fit into a single line.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the WrapText property is True.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="PushButton"/>  (<see cref="GridPushButtonCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Specifies if text should be wrapped when it does not fit into a single line."),
        Browsable(true),
        SRCategory("StyleCategoryBehavior")]
        [NotifyParentProperty(true)]
        public bool WrapText
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridStyleInfoStore.WrapTextProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.WrapTextProperty, value ? 1 : 0);
            }
        }
                
        /// <summary>
        /// Resets <see cref="GridStyleInfo.WrapText"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetWrapText()
        {
            ResetValue(GridStyleInfoStore.WrapTextProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeWrapText()
        {
            return HasValue(GridStyleInfoStore.WrapTextProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.WrapText"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasWrapText
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.WrapTextProperty);
            }
        }
        #endregion


        #region WrapRotatedText
        [Description("Specifies if Rotated text should be wrapped when it does not fit into a single line."),
        Browsable(true),
        SRCategory("StyleCategoryBehavior")]
        [NotifyParentProperty(true)]
        public bool WrapRotatedText
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridStyleInfoStore.WrapRotatedTextProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.WrapRotatedTextProperty, value ? 1 : 0);
            }
        }


        /// <summary>
        /// Resets <see cref="GridStyleInfo.WrapRotatedText"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetWrapRotatedText()
        {
            ResetValue(GridStyleInfoStore.WrapRotatedTextProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeWrapRotatedText()
        {
            return HasValue(GridStyleInfoStore.WrapRotatedTextProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.WrapRotatedText"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasWrapRotatedText
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.WrapRotatedTextProperty);
            }
        }
        #endregion
        #region VerticalScrollbar
        /// <summary>
        /// Gets or sets a value indicating whether text box should show a vertical scrollbar when text is being edited and does not fit in cell. WrapText must be initialized to True.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the VerticalScrollbar property is False.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Specifies if text box should show a vertical scrollbar when text is being edited and does not fit in cell. WrapText must be initialized to True."),
        Browsable(true),
        SRCategory("StyleCategoryBehavior")]
        [NotifyParentProperty(true)]
        public bool VerticalScrollbar
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridStyleInfoStore.VerticalScrollbarProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.VerticalScrollbarProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.VerticalScrollbar"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetVerticalScrollbar()
        {
            ResetValue(GridStyleInfoStore.VerticalScrollbarProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeVerticalScrollbar()
        {
            return HasValue(GridStyleInfoStore.VerticalScrollbarProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.VerticalScrollbar"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasVerticalScrollbar
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.VerticalScrollbarProperty);
            }
        }
        #endregion
        #region AutoSize
        /// <summary>
        /// Gets or sets a value indicating whether the cell height should automatically increase when the edited text does not fit into the cell and <see cref="GridStyleInfo.WrapText"/> is True. If <see cref="GridStyleInfo.WrapText"/> is False, <see cref="GridStyleInfo.AutoSize"/> will affect the column width.
        /// </summary>
        /// <para/>
        /// <remarks>
        /// <para/>
        /// The default value for the AutoSize property is False.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Specifies if the cell height should automatically increase when the edited text does not fit into the cell and WrapText is True. If WrapText is False, AutoSize will affect the column width."),
        Browsable(true),
        SRCategory("StyleCategoryBehavior")]
        [NotifyParentProperty(true)]
        public bool AutoSize
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridStyleInfoStore.AutoSizeProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.AutoSizeProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.AutoSize"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetAutoSize()
        {
            ResetValue(GridStyleInfoStore.AutoSizeProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeAutoSize()
        {
            return HasValue(GridStyleInfoStore.AutoSizeProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.AutoSize"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasAutoSize
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.AutoSizeProperty);
            }
        }
        #endregion
        #region AllowEnter
        /// <summary>
        /// Gets or sets a value indicating whether pressing the &lt;Enter&gt;-Key should insert a new line into the edited text.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the AllowEnter property is False.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Specifies if pressing the <Enter>-Key should insert a new line into the edited text."),
        Browsable(true),
        SRCategory("StyleCategoryBehavior")]
        [NotifyParentProperty(true)]
        public bool AllowEnter
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridStyleInfoStore.AllowEnterProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.AllowEnterProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.AllowEnter"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetAllowEnter()
        {
            ResetValue(GridStyleInfoStore.AllowEnterProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeAllowEnter()
        {
            return HasValue(GridStyleInfoStore.AllowEnterProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.AllowEnter"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasAllowEnter
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.AllowEnterProperty);
            }
        }
        #endregion
        #region Enabled
        /// <summary>
        /// Gets or sets a value indicating whether the cell can be activated as current cell or if the cell should be skipped when moving the current cell.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the Enabled property is True.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DataBoundRowHeader"/>  (<see cref="GridDataBoundRowHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="PushButton"/>  (<see cref="GridPushButtonCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Specifies if the cell can be activated as current cell or if the cell should be skipped when moving the current cell."),
        Browsable(true),
        SRCategory("StyleCategoryBehavior")]
        [NotifyParentProperty(true)]
        public bool Enabled
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridStyleInfoStore.EnabledProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.EnabledProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.Enabled"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetEnabled()
        {
            ResetValue(GridStyleInfoStore.EnabledProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeEnabled()
        {
            return HasValue(GridStyleInfoStore.EnabledProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.Enabled"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasEnabled
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.EnabledProperty);
            }
        }
        #endregion
        #region TriState
        /// <summary>
        /// Gets or sets a value indicating whether this checkbox is a Tristate check box that has an additional indeterminated state.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the TriState property is False.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Specifies if this checkbox is a Tristate check box that has an additional indeterminated state."),
        Browsable(true),
        SRCategory("StyleCategoryBehavior")]
        [NotifyParentProperty(true)]
        public bool TriState
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridStyleInfoStore.TriStateProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.TriStateProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.TriState"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetTriState()
        {
            ResetValue(GridStyleInfoStore.TriStateProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeTriState()
        {
            return HasValue(GridStyleInfoStore.TriStateProperty);
        }

        /// <summary>
        /// Gets a value indicating whetherf <see cref="GridStyleInfo.TriState"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTriState
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.TriStateProperty);
            }
        }
        #endregion
        #region ExclusiveChoiceList
        /// <summary>
        /// Gets or sets a value indicating whether user input is restricted to items from the <see cref="GridStyleInfo.ChoiceList"/> or <see cref="GridStyleInfo.DataSource"/>.
        /// Use <see cref="DropDownStyle"/> instead.<para/>
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the ExclusiveChoiceList property is False.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Specifies if user input is restricted to items from the ChoiceList or DataSource"),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRCategory("StyleCategoryValue")]
        [NotifyParentProperty(true)]
        public bool ExclusiveChoiceList
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridStyleInfoStore.ExclusiveChoiceListProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.ExclusiveChoiceListProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.ExclusiveChoiceList"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetExclusiveChoiceList()
        {
            ResetValue(GridStyleInfoStore.ExclusiveChoiceListProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeExclusiveChoiceList()
        {
            return HasValue(GridStyleInfoStore.ExclusiveChoiceListProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.ExclusiveChoiceList"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasExclusiveChoiceList
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.ExclusiveChoiceListProperty);
            }
        }
        #endregion
        #region AutoComplete

        /// <summary>
        /// Gets or sets if user input is restricted to items from the <see cref="GridStyleInfo.ChoiceList"/> or <see cref="GridStyleInfo.DataSource"/>.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the DropDownStyle property is GridDropDownStyle.Editable.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Specifies if user input is restricted to items from the ChoiceList or DataSource."),
        Browsable(true),
        SRCategory("StyleCategoryValue")]
        [NotifyParentProperty(true)]
        public GridDropDownStyle DropDownStyle
        {
            [DebuggerStepThrough()]
            get
            {
                GridDropDownStyle ds = GridDropDownStyle.Editable;
                if (GetShortValue(GridStyleInfoStore.ExclusiveChoiceListProperty) == 1)
                {
                    ds |= GridDropDownStyle.Exclusive;
                }

                if (GetShortValue(GridStyleInfoStore.AutoCompleteProperty) == 1)
                {
                    ds |= GridDropDownStyle.AutoComplete;
                }

                return ds;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.ExclusiveChoiceListProperty, ((value & GridDropDownStyle.Exclusive) == GridDropDownStyle.Exclusive) ? 1 : 0);
                SetValue(GridStyleInfoStore.AutoCompleteProperty, ((value & GridDropDownStyle.AutoComplete) == GridDropDownStyle.AutoComplete) ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.DropDownStyle"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetDropDownStyle()
        {
            ResetValue(GridStyleInfoStore.AutoCompleteProperty);
            ResetValue(GridStyleInfoStore.ExclusiveChoiceListProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeDropDownStyle()
        {
            return HasValue(GridStyleInfoStore.AutoCompleteProperty) || HasValue(GridStyleInfoStore.ExclusiveChoiceListProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.DropDownStyle"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasDropDownStyle
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.AutoCompleteProperty) || HasValue(GridStyleInfoStore.ExclusiveChoiceListProperty);
            }
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>
        /// <c>true</c> if [is auto complete]; otherwise, <c>false</c>.
        /// </returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool IsAutoComplete()
        {
            return GetShortValue(GridStyleInfoStore.AutoCompleteProperty) != 0;
        }

        /// <summary>
        /// Gets or sets a value indicating whether user input is restricted to items from the <see cref="GridStyleInfo.ChoiceList"/> or <see cref="GridStyleInfo.DataSource"/>.
        /// </summary>
        [Description("Specifies if user input is restricted to items from the ChoiceList or DataSource."),
        Browsable(true),
        SRCategory("StyleCategoryValue")]
        internal bool AutoComplete
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridStyleInfoStore.AutoCompleteProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.AutoCompleteProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.AutoComplete"/>.
        /// </summary>
        [DebuggerStepThrough()]
        internal void ResetAutoComplete()
        {
            ResetValue(GridStyleInfoStore.AutoCompleteProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeAutoComplete()
        {
            return HasValue(GridStyleInfoStore.AutoCompleteProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.AutoComplete"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool HasAutoComplete
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.AutoCompleteProperty);
            }
        }

        /// <summary>
        /// Gets or sets the value whether the cell can have AutoComplete, AutoSuggest, Both and None behavior in Editable mode.
        /// </summary>
        [Description("Gets or sets the value whether the cell can have AutoComplete behavior in Editable mode."),
        Browsable(true),
        SRCategory("StyleCategoryValue")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [SerializeProperty(true)]
        public GridComboSelectionOptions AutoCompleteInEditMode
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridComboSelectionOptions)GetShortValue(GridStyleInfoStore.AutoCompleteInEditModeProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.AutoCompleteInEditModeProperty, (short)value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.AutoComplete"/>.
        /// </summary>
        [DebuggerStepThrough()]
        internal void ResetAutoCompleteInEditMode()
        {
            ResetValue(GridStyleInfoStore.AutoCompleteInEditModeProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeAutoCompleteInEditMode()
        {
            return HasValue(GridStyleInfoStore.AutoCompleteInEditModeProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.AutoCompleteInEditMode"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool HasAutoCompleteInEditMode
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.AutoCompleteInEditModeProperty);
            }
        }

        #endregion
        #region FloatCell
        /// <summary>
        /// Gets or sets a value indicating whether text can float into the boundaries of a neighboring cell.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the FloatCell property is True.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Gets / sets if text can float into the boundaries of a neighboring cell."),
        Browsable(true),
        SRCategory("StyleCategoryBehavior")]
        [NotifyParentProperty(true)]
        public bool FloatCell
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridStyleInfoStore.FloatCellProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.FloatCellProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.FloatCell"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetFloatCell()
        {
            ResetValue(GridStyleInfoStore.FloatCellProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeFloatCell()
        {
            return HasValue(GridStyleInfoStore.FloatCellProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.FloatCell"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasFloatCell
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.FloatCellProperty);
            }
        }
        #endregion
        #region FloodCell
        /// <summary>
        /// Gets or sets a value indicating whether this cell can be flooded by a previous cell.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the FloodCell property is True.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Gets / sets if this cell can be flooded by a previous cell."),
        Browsable(true),
        SRCategory("StyleCategoryBehavior")]
        [NotifyParentProperty(true)]
        public bool FloodCell
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridStyleInfoStore.FloodCellProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.FloodCellProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.FloodCell"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetFloodCell()
        {
            ResetValue(GridStyleInfoStore.FloodCellProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeFloodCell()
        {
            return HasValue(GridStyleInfoStore.FloodCellProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.FloodCell"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasFloodCell
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.FloodCellProperty);
            }
        }
        #endregion
        #region MergeCell
        /// <summary>
        /// Gets or sets if cell edges shall be drawn raised, sunken, or flat (default).
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the MergeCell property is GridMergeCellDirection.Flat.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DataBoundRowHeader"/>  (<see cref="GridDataBoundRowHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="PushButton"/>  (<see cref="GridPushButtonCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Specifies if cell edges shall be drawn raised, sunken, or flat (default)."),
        Browsable(true),
        SRCategory("StyleCategoryAppearance")]
        [NotifyParentProperty(true)]
        public GridMergeCellDirection MergeCell
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridMergeCellDirection)GetShortValue(GridStyleInfoStore.MergeCellProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.MergeCellProperty, (short)value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.MergeCell"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetMergeCell()
        {
            ResetValue(GridStyleInfoStore.MergeCellProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeMergeCell()
        {
            return HasValue(GridStyleInfoStore.MergeCellProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.MergeCell"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasMergeCell
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.MergeCellProperty);
            }
        }
        #endregion
        #region CellAppearance
        /// <summary>
        /// Gets or sets if cell edges shall be drawn raised, sunken, or flat (default).
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the CellAppearance property is GridCellAppearance.Flat.
        /// <para/>
        /// The property affects the behavior or appearance of the following cell types:
        /// <para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DataBoundRowHeader"/>  (<see cref="GridDataBoundRowHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="PushButton"/>  (<see cref="GridPushButtonCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Specifies if cell edges shall be drawn raised, sunken, or flat (default)."),
        Browsable(true),
        SRCategory("StyleCategoryAppearance")]
        [NotifyParentProperty(true)]
        public GridCellAppearance CellAppearance
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridCellAppearance)GetShortValue(GridStyleInfoStore.CellAppearanceProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.CellAppearanceProperty, (short)value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.CellAppearance"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetCellAppearance()
        {
            ResetValue(GridStyleInfoStore.CellAppearanceProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCellAppearance()
        {
            return HasValue(GridStyleInfoStore.CellAppearanceProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.CellAppearance"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCellAppearance
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.CellAppearanceProperty);
            }
        }
        /// <summary>
        /// Provides an option for displaying the cell with symbols when the contents in cell exceeds it's width.
        /// </summary>
        public AutoFitOptions AutoFit
        {
            [DebuggerStepThrough()]
            get
            {
                return (AutoFitOptions)GetShortValue(GridStyleInfoStore.AutoFitProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.AutoFitProperty, (short)value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.AutoFit"/>
        /// </summary>
        public void ResetAutoFit()
        {
            ResetValue(GridStyleInfoStore.AutoFitProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeAutoFit()
        {
            return HasValue(GridStyleInfoStore.AutoFitProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.AutoFit"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasAutoFit
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.AutoFitProperty);
            }
        }
        
        /// <summary>
        /// Obtains the desired character to be displayed in cell when the contents in cell exceeds the width.
        /// </summary>
        public Char AutoFitChar
        {
            [DebuggerStepThrough()]
            get
            {
                return (Char)GetValue(GridStyleInfoStore.AutoFitCharProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.AutoFitCharProperty, (Char)value);
            }
        }
        
        /// <summary>
        /// Resets <see cref="GridStyleInfo.AutoFitChar"/>
        /// </summary>
        public void ResetAutoFitChar()
        {
            ResetValue(GridStyleInfoStore.AutoFitCharProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeAutoFitChar()
        {
            return HasValue(GridStyleInfoStore.AutoFitCharProperty);
        }
        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.AutoFitChar"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasAutoFitChar
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.AutoFitCharProperty);
            }
        }
        #endregion
        #region MaxLength
        /// <summary>
        /// Gets or sets Limits the number of characters the user can type into the cell. Note: When selecting a text from a choice list or when pasting text, the text can be longer. Additional validation is necessary on your side.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the MaxLength property is 0.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Limits the number of characters the user can type into the cell. Note: When selecting a text from a choice list or when pasting text, the text can be longer. Additional validation is necessary on your side."),
        Browsable(true),
        SRCategory("StyleCategoryBehavior")]
        [NotifyParentProperty(true)]
        public int MaxLength
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridStyleInfoStore.MaxLengthProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.MaxLengthProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.MaxLength"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetMaxLength()
        {
            ResetValue(GridStyleInfoStore.MaxLengthProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeMaxLength()
        {
            return HasValue(GridStyleInfoStore.MaxLengthProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.MaxLength"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasMaxLength
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.MaxLengthProperty);
            }
        }
        #endregion
        #region CellType
        /// <summary>
        /// Gets or sets the cell type for this style instance. Cell types are accessed in the grid through the <see cref="GridModel.CellModels"/>
        /// property of a <see cref="GridModel"/> which returns a <see cref="GridCellModelBase"/> object. To access cell renderers
        /// use the <see cref="GridControlBase.CellRenderers"/> property of a <see cref="GridControlBase"/> instance.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the CellType property is Text Box.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DataBoundRowHeader"/>  (<see cref="GridDataBoundRowHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="PushButton"/>  (<see cref="GridPushButtonCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Currency"/>  (<see cref="GridCurrencyTextBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="MaskEdit"/>  (<see cref="GridMaskEditCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="FormulaCell"/>  (<see cref="GridFormulaCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="RichText"/>  (<see cref="GridRichTextBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Control"/>  (<see cref="GridGenericControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="OriginalTextBox"/>  (<see cref="GridOriginalTextBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ProgressBar"/>  (<see cref="GridProgressBarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="RadioButton"/>  (<see cref="GridRadioButtonCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Contains cell type information of a cell."),
        Browsable(true),
        TypeConverter(typeof(GridCellTypeNameConverter)),
        RefreshProperties(RefreshProperties.Repaint),
        SRCategory("StyleCategoryAppearance")]
        [NotifyParentProperty(true)]
        public string CellType
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridStyleInfoStore.CellTypeProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.CellTypeProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.CellType"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetCellType()
        {
            ResetValue(GridStyleInfoStore.CellTypeProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCellType()
        {
            return HasValue(GridStyleInfoStore.CellTypeProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.CellType"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCellType
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.CellTypeProperty);
            }
        }
        #endregion
        #region ChoiceList
        /// <summary>
        /// Gets or sets items to be displayed in a drop-down list.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the ChoiceList property is NULL.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Specifies items to be displayed in a drop-down list."),
        Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design",
            "System.Drawing.Design.UITypeEditor, System.Drawing"),
        DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content),
        Browsable(false),
        RefreshProperties(RefreshProperties.Repaint),
        SRCategory("StyleCategoryValue")]
        [NotifyParentProperty(true)]
        public StringCollection ChoiceList
        {
            [DebuggerStepThrough()]
            get
            {
                StringCollection c = (StringCollection)GetValue(GridStyleInfoStore.ChoiceListProperty);
                //// TODO: ChoiceList Colledtion Editor-
                ////SetValue(GridStyleInfoStore.ChoiceListProperty, c);
                return c;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.ChoiceListProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.ChoiceList"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetChoiceList()
        {
            ResetValue(GridStyleInfoStore.ChoiceListProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeChoiceList()
        {
            return HasValue(GridStyleInfoStore.ChoiceListProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.ChoiceList"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasChoiceList
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.ChoiceListProperty);
            }
        }
        #endregion
        #region Description
        /// <summary>
        /// Gets or sets the text that is shown in check box or pushbuttons.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the Description property is String.Empty.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="PushButton"/>  (<see cref="GridPushButtonCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Gets / sets the text that is shown in check box or pushbuttons."),
        Browsable(true),
        SRCategory("StyleCategoryValue")]
        [NotifyParentProperty(true)]
        public string Description
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridStyleInfoStore.DescriptionProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.DescriptionProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.Description"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetDescription()
        {
            ResetValue(GridStyleInfoStore.DescriptionProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeDescription()
        {
            return HasValue(GridStyleInfoStore.DescriptionProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.Description"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasDescription
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.DescriptionProperty);
            }
        }
        #endregion
        #region Format
        /// <summary>
        /// Gets or sets the format mask for formatting the cell value. You can specify numeric format strings,
        /// date format strings, or enumeration format strings as discussed in the section "Format Specifiers and Format Providers"
        /// of the .NET Framework Developers Guide (see ms-help://MS.VSCC/MS.MSDNVS/cpguide/html/cpconformatspecifiersformatproviders.htm)
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the Format property is String.Empty.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Contains format information of a cell."),
        Browsable(true),
        Editor(typeof(GridCellFormatEditor), typeof(System.Drawing.Design.UITypeEditor)),
        RefreshProperties(RefreshProperties.Repaint),
        SRCategory("StyleCategoryValue")]
        [NotifyParentProperty(true)]
        public string Format
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridStyleInfoStore.FormatProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.FormatProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.Format"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetFormat()
        {
            ResetValue(GridStyleInfoStore.FormatProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeFormat()
        {
            return HasValue(GridStyleInfoStore.FormatProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.Format"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasFormat
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.FormatProperty);
            }
        }
        #endregion
        #region DisplayMember
        /// <summary>
        ///  Gets or sets the Name the property in the <see cref="GridStyleInfo.DataSource"/> that holds the text to be displayed in a cell that depends on a <see cref="GridStyleInfo.ValueMember"/>.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the DisplayMember property is String.Empty.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Category(@"Data"),
        Editor(@"System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", @"System.Drawing.Design.UITypeEditor, System.Drawing"),
            ////        DefaultValue(@""),
        TypeConverter(@"System.Windows.Forms.Design.DataMemberFieldConverter, System.Design"),
        Description(@"Indicates the property to display for the items in this control.")]
        [NotifyParentProperty(true)]
        public string DisplayMember
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridStyleInfoStore.DisplayMemberProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.DisplayMemberProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.DisplayMember"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetDisplayMember()
        {
            ResetValue(GridStyleInfoStore.DisplayMemberProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeDisplayMember()
        {
            return HasValue(GridStyleInfoStore.DisplayMemberProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.DisplayMember"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasDisplayMember
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.DisplayMemberProperty);
            }
        }
        #endregion

        #region ValueMember
        /// <summary>
        ///   <para>Gets or sets a string that
        /// specifies the property of the data source from which to draw
        /// the value.</para>
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the ValueMember property is String.Empty.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description(@"Indicates the property to use as the actual value for the items in the control."),
        Category(@"Data"),
            ////        DefaultValue(@""),
        Editor(@"System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", @"System.Drawing.Design.UITypeEditor, System.Drawing")]
        [NotifyParentProperty(true)]
        public string ValueMember
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridStyleInfoStore.ValueMemberProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.ValueMemberProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.ValueMember"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetValueMember()
        {
            ResetValue(GridStyleInfoStore.ValueMemberProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeValueMember()
        {
            return HasValue(GridStyleInfoStore.ValueMemberProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.ValueMember"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasValueMember
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.ValueMemberProperty);
            }
        }
        #endregion
        #region DataSource
        /// <summary>
        /// Gets or sets a data source that holds items to be displayed in a drop-down list. A datasource can be specified instead of manually filling the choicelist with string entries.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the DataSource property is null.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description(@"Indicates the list that this cell will use to get its items."),
        Category(@"Data"),
            ////        DefaultValue(null),
        RefreshProperties(RefreshProperties.Repaint),
        TypeConverter(@"System.Windows.Forms.Design.DataSourceConverter, System.Design")]
        [CloneableProperty(false), DisposeableProperty(false)] // REVIEW:
        [NotifyParentProperty(true)]
        public object DataSource
        {
            [DebuggerStepThrough()]
            get
            {
                return (object)GetValue(GridStyleInfoStore.DataSourceProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.DataSourceProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.DataSource"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetDataSource()
        {
            ResetValue(GridStyleInfoStore.DataSourceProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeDataSource()
        {
            return HasValue(GridStyleInfoStore.DataSourceProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.DataSource"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasDataSource
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.DataSourceProperty);
            }
        }
        #endregion
        #region PropertyDescriptor
        /// <summary>
        /// Gets or sets a property descriptor that can be used by UITypeEditCell, PropertyGridCell, and StandardValuesCell cell types.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PropertyDescriptor PropertyDescriptor
        {
            [DebuggerStepThrough()]
            get
            {
                return (PropertyDescriptor)GetValue(GridStyleInfoStore.PropertyDescriptorProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.PropertyDescriptorProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.PropertyDescriptor"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetPropertyDescriptor()
        {
            ResetValue(GridStyleInfoStore.PropertyDescriptorProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializePropertyDescriptor()
        {
            return HasValue(GridStyleInfoStore.PropertyDescriptorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.PropertyDescriptor"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasPropertyDescriptor
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.PropertyDescriptorProperty);
            }
        }
        #endregion
        #region Text
        /// <summary>
        /// Gets or sets the value as a string. If a <see cref="GridStyleInfo.CellValueType"/>
        /// is specified, the text will be parsed and converted to the type specified with
        /// <see cref="GridStyleInfo.CellValueType"/> using CultureInfo.CurrentCulture
        /// information.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the Text property is String.Empty.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Gets / sets the value as a string."),
        Browsable(true),
        RefreshProperties(RefreshProperties.Repaint),
        SRCategory("StyleCategoryValue")]
        [NotifyParentProperty(true)]
        public string Text
        {
            [DebuggerStepThrough()]
            get
            {
                object value = CellValue;
                return GetText(CellValue);
            }

            [DebuggerStepThrough()]
            set
            {
                ApplyText(value);
            }
        }

        /// <summary>
        /// Resets the <see cref="Text"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetText()
        {
            ResetValue(GridStyleInfoStore.CellValueProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeText()
        {
            return HasValue(GridStyleInfoStore.CellValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.Text"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasText
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.CellValueProperty);
            }
        }
        #endregion
        #region CellValue
        /// <summary>
        /// Gets or sets the cell value. Although the cell value is typically a string, it
        /// can also be any other primitive type such as int, byte, enum, or any custom type that
        /// is derived from <see cref="System.Object"/>.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the CellValue property is String.Empty.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// TODO: Explain how to set StylePropertyInfo.IsCloneable = false and StylePropertyInfo.IsDisposable = false
        /// or implement IStyleCloneable to avoid cloning and disposing of objects assigned to Tag.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Contains cell value information of a cell."),
        Browsable(false),
        RefreshProperties(RefreshProperties.Repaint),
        SRCategory("StyleCategoryValue"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ////[CloneableProperty(false), DisposeableProperty(false)]  // REVIEW:
        public object CellValue
        {
            [DebuggerStepThrough()]
            get
            {
                object value = GetValue(GridStyleInfoStore.CellValueProperty);
                ////                if (this.StrictValueType && !HasError)
                ////                {
                ////                    Type type = this.CellValueType;
                ////                    if (type != null && type != value.GetType())
                ////                    {
                ////                        try
                ////                        {
                ////                            value = GridCellValueConvert.ChangeType(value, type, null);
                ////                        }
                ////                        catch (Exception ex)
                ////                        {
                ////                            value = null;
                ////                        }
                ////                    }
                ////                }

                return value;
            }

            [DebuggerStepThrough()]
            set
            {
                ////                if (value != null && this.StrictValueType)
                ////                {
                ////                    Type type = this.CellValueType;
                ////                    if (type != null && type != value.GetType())
                ////                    {
                ////                        try
                ////                        {
                ////                            value = GridCellValueConvert.ChangeType(value, type, null);
                ////                        }
                ////                        catch (Exception ex)
                ////                        {
                ////                            Error = ex.Message;
                ////                            value = null;
                SetValue(GridStyleInfoStore.CellValueProperty, value);
                ////                            return;
                ////                        }
                ////                    }
                ////                }
                if (HasError)
                {
                    ResetError();
                }
                ////                SetValue(GridStyleInfoStore.CellValueProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.CellValue"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetCellValue()
        {
            ResetValue(GridStyleInfoStore.CellValueProperty);
            ResetError();
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCellValue()
        {
            return HasValue(GridStyleInfoStore.CellValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.CellValue"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCellValue
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.CellValueProperty);
            }
        }
        #endregion
        #region CellValueType
        /// <summary>
        /// Gets or sets the preferred <see cref="System.Type"/> for cell values. When you assign a value
        /// to the <see cref="GridStyleInfo"/> object, the value will be converted to this type. If the
        /// value cannot be converted, <see cref="GridStyleInfo.Error"/> will contain error information.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the CellValueType property is NULL.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Contains cell value type information of a cell."),
        Browsable(true),
        TypeConverter(typeof(GridCellValueTypeConverter)),
        RefreshProperties(RefreshProperties.Repaint),
        SRCategory("StyleCategoryValue")]
        [NotifyParentProperty(true)]
        public Type CellValueType
        {
            [DebuggerStepThrough()]
            get
            {
                return (Type)GetValue(GridStyleInfoStore.CellValueTypeProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.CellValueTypeProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.CellValueType"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetCellValueType()
        {
            ResetValue(GridStyleInfoStore.CellValueTypeProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCellValueType()
        {
            return HasValue(GridStyleInfoStore.CellValueTypeProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.CellValueType"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCellValueType
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.CellValueTypeProperty);
            }
        }

        #endregion
        #region ImageFromByteArray
        /// <summary>
        /// Gets or sets a value indicating whether cells should check whether a byte array can be
        /// converted to a image.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the ImageFromByteArray property is True.<para/>
        /// <para/>
        /// </remarks>
        [Description("Specifies if cells should check whether a byte array can be converted to a image."),
        Browsable(true),
        SRCategory("StyleCategoryValue")]
        [NotifyParentProperty(true)]
        public bool ImageFromByteArray
        {
            get
            {
                return GetShortValue(GridStyleInfoStore.ImageFromByteArrayProperty) != 0;
            }

            set
            {
                SetValue(GridStyleInfoStore.ImageFromByteArrayProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets Read-only information.
        /// </summary>
        public void ResetImageFromByteArray()
        {
            ResetValue(GridStyleInfoStore.ImageFromByteArrayProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeImageFromByteArray()
        {
            return HasImageFromByteArray;
        }

        /// <summary>
        /// Gets a value indicating whether Read-only information has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasImageFromByteArray
        {
            get
            {
                return HasValue(GridStyleInfoStore.ImageFromByteArrayProperty);
            }
        }
        #endregion
        #region Error
        /// <summary>
        /// Gets or sets error information if a text could not be converted to the <see cref="System.Type"/> specified
        ///  with <see cref="GridStyleInfo.CellValueType"/> in
        /// <see cref="FormattedText"/> or <see cref="Text"/> setter.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the Error property is String.Empty.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DataBoundRowHeader"/>  (<see cref="GridDataBoundRowHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Holds error information if a text could not be converted to the Type specified with CellValueType."),
        Browsable(false),
        SRCategory("StyleCategoryValue")]
        public string Error
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridStyleInfoStore.ErrorProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.ErrorProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.Error"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetError()
        {
            ResetValue(GridStyleInfoStore.ErrorProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeError()
        {
            return HasValue(GridStyleInfoStore.ErrorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.Error"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasError
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.ErrorProperty);
            }
        }

        #endregion
        #region StrictValueType
        /// <summary>
        /// Gets or sets a value indicating whether an exception should be thrown in the <see cref="ApplyFormattedText(string)"/> method
        /// if the formatted text can not be parsed and converted to the type specified with
        /// <see cref="CellValueType"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="ApplyFormattedText(string)"/> method will be called when the user enters text into
        /// a text box. The method checks if there are event handlers for <see cref="GridModel.SaveCellFormattedText"/>.
        /// If not, it will continue with its default behavior and try to convert the input text into
        /// the type specified with <see cref="CellValueType"/>.
        /// <para/>
        /// If this conversion fails, <see cref="ApplyFormattedText(string)"/> will check <see cref="StrictValueType"/>. If it
        /// is True, an exception is thrown which itself results in a warning message displayed to the user at the
        /// time from <see cref="GridControlBase.CurrentCellValidating"/>.
        /// <para/>
        /// If you set <see cref="StrictValueType"/> to False, <see cref="ApplyFormattedText(string)"/> will not throw
        /// an exception; it will simply store the text as <see cref="CellValue"/>.
        /// <para/>
        /// If you need a more specialized customization of this behavior, you should handle the
        /// <see cref="GridModel.SaveCellFormattedText"/> event. This lets you parse the text input
        /// and change the cell's <see cref="CellValueType"/> at run-time. See <see cref="GridModel.SaveCellFormattedText"/>
        /// for an example how to do this.
        /// <para/>
        /// The default value for the StrictValueType property is True.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DataBoundRowHeader"/>  (<see cref="GridDataBoundRowHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="PushButton"/>  (<see cref="GridPushButtonCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Currency"/>  (<see cref="GridCurrencyTextBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="MaskEdit"/>  (<see cref="GridMaskEditCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="FormulaCell"/>  (<see cref="GridFormulaCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="RichText"/>  (<see cref="GridRichTextBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Control"/>  (<see cref="GridGenericControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="OriginalTextBox"/>  (<see cref="GridOriginalTextBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ProgressBar"/>  (<see cref="GridProgressBarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="RadioButton"/>  (<see cref="GridRadioButtonCellRenderer"/>)</term>
        ///     </item>
        ///     </list>
        /// <para/>
        /// </remarks>
        [Browsable(true),
        RefreshProperties(RefreshProperties.Repaint),
        SRCategory("StyleCategoryValue"), Description("Gets/sets a value indicating whether an exception should be thrown in the ApplyFormattedText method,if the formatted text can not be parsed and converted to the type specified with CellType")]
        [NotifyParentProperty(true)]
        public bool StrictValueType
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridStyleInfoStore.StrictValueTypeProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.StrictValueTypeProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.StrictValueType"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetStrictValueType()
        {
            ResetValue(GridStyleInfoStore.StrictValueTypeProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeStrictValueType()
        {
            return HasValue(GridStyleInfoStore.StrictValueTypeProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.StrictValueType"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasStrictValueType
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.StrictValueTypeProperty);
            }
        }
        #endregion
        #region Tag
        /// <summary>
        /// Gets or sets a custom tag you can associate with a cell.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the Tag property is NULL.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// TODO: Explain how to set StylePropertyInfo.IsCloneable = false and StylePropertyInfo.IsDisposable = false
        /// or implement IStyleCloneable to avoid cloning and disposing of objects assigned to Tag.
        /// </remarks>
        [Description("A custom tag you can associate with a cell."),
        Browsable(false),
        RefreshProperties(RefreshProperties.Repaint),
        SRCategory("StyleCategoryValue")]
        ////[CloneableProperty(false), DisposeableProperty(false)] // REVIEW:
        public object Tag
        {
            [DebuggerStepThrough()]
            get
            {
                return GetValue(GridStyleInfoStore.TagProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.TagProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.Tag"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetTag()
        {
            ResetValue(GridStyleInfoStore.TagProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeTag()
        {
            return HasValue(GridStyleInfoStore.TagProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="Tag"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTag
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.TagProperty);
            }
        }
        #endregion
        #region Control
        /// <summary>
        /// Gets or sets a custom control you can associate with a cell.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the Control property is NULL.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="Control"/>  (<see cref="GridGenericControlCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("A custom control you can associate with a cell."),
        Browsable(false),
        RefreshProperties(RefreshProperties.Repaint),
        SRCategory("StyleCategoryValue")]
        [CloneableProperty(false), DisposeableProperty(false)]
        public Control Control
        {
            [DebuggerStepThrough()]
            get
            {
                return (Control)GetValue(GridStyleInfoStore.ControlProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.ControlProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.Control"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetControl()
        {
            ResetValue(GridStyleInfoStore.ControlProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeControl()
        {
            return HasValue(GridStyleInfoStore.ControlProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="Control"/> has been initialized for the current Control.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasControl
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.ControlProperty);
            }
        }
        #endregion
        #region FormulaTag
        /// <summary>
        /// Gets or sets a formula tag that is associated with a cell.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the FormulaTag property is NULL.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="FormulaCell"/>  (<see cref="GridFormulaCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("A custom tag you can associate with a cell."),
        Browsable(false),
        SRCategory("StyleCategoryValue"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SerializeProperty(true)]
        public GridFormulaTag FormulaTag
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridFormulaTag)GetValue(GridStyleInfoStore.FormulaTagProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.FormulaTagProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.FormulaTag"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetFormulaTag()
        {
            ResetValue(GridStyleInfoStore.FormulaTagProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeFormulaTag()
        {
            return HasValue(GridStyleInfoStore.FormulaTagProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="FormulaTag"/> has been initialized for the current FormulaTag.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasFormulaTag
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.FormulaTagProperty);
            }
        }
        #endregion
        #region CultureInfo
        /// <summary>
        /// Gets or sets the culture information holds rules for parsing and formatting the cell's value.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the CultureInfo property is NULL.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("The culture information holds rules for parsing and formatting the cell's value."),
        Browsable(true),
        TypeConverter(typeof(CultureInfoConverter)),
        RefreshProperties(RefreshProperties.Repaint),
        SRCategory("StyleCategoryValue"),
        ImmutableObject(true)]
        [NotifyParentProperty(true)]
        [CloneableProperty(false), DisposeableProperty(false)]
        public CultureInfo CultureInfo
        {
            [DebuggerStepThrough()]
            get
            {
                return (CultureInfo)GetValue(GridStyleInfoStore.CultureInfoProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.CultureInfoProperty, value);
            }
        }

        /// <summary>
        /// Gets the culture information from the style object or returns CultureInfo.CurrentCulture
        /// if <see cref="GridStyleInfo.CultureInfo"/> is NULL.
        /// </summary>
        /// <param name="useCurrentCultureIfNull">True if CultureInfo.CurrentUICulture should be returned
        /// when <see cref="GridStyleInfo.CultureInfo"/> is NULL.</param>
        /// <returns>The culture information with rules for parsing and formatting the cell's value.</returns>
        public CultureInfo GetCulture(bool useCurrentCultureIfNull)
        {
            CultureInfo culture = CultureInfo;
            if (culture == null)
            {
                culture = CultureInfo.CurrentCulture;
            }
            else
            {
                if (!CurrencyEdit.UseCultureInfo && CellType == GridCellTypeName.Currency)
                {
                    culture = CultureInfo.CurrentCulture;
                }
            }

            return culture;
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.CultureInfo"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetCultureInfo()
        {
            ResetValue(GridStyleInfoStore.CultureInfoProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCultureInfo()
        {
            return HasValue(GridStyleInfoStore.CultureInfoProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.CultureInfo"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCultureInfo
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.CultureInfoProperty);
            }
        }
        #endregion
        #region ValidateValue
        /// <summary>
        /// Gets or sets validation rules for the cell value that are being checked before any user changes are committed to the grid cell's style object.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the ValidateValue property is NULL.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Holds validation rules for the cell values that are being checked before any user changes are committed to the grid cells style object.."),
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        RefreshProperties(RefreshProperties.Repaint),
        SRCategory("StyleCategoryValue"),
        NotifyParentProperty(true)]
        public GridCellValidateValueInfo ValidateValue
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridCellValidateValueInfo)GetValue(GridStyleInfoStore.ValidateValueProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.ValidateValueProperty, value);
            }
        }

        /// <internalonly/>
        /// <summary>Gets readonly validate value. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [NotifyParentProperty(true)]
        public GridCellValidateValueInfo ReadOnlyValidateValue
        {
            ////[DebuggerStepThrough()]
            get
            {
                if (HasValidateValue && !ValidateValue.IsEmpty)
                {
                    return ValidateValue;
                }

                if (Identity != null)
                {
                    GridStyleInfo validateInfo = this.Identity.GetBaseStyleNotEmptyExpandable(this, GridStyleInfoStore.ValidateValueProperty) as GridStyleInfo;
                    if (validateInfo != null)
                    {
                        if (fixSubObjectsDerivedFromRowandColStyle && IsRowOrColumnStyle(validateInfo))
                        {
                            return ValidateValue;
                        }

                        return validateInfo.ValidateValue;
                    }
                }

                return Default.ValidateValue;
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.ValidateValue"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetValidateValue()
        {
            ResetValue(GridStyleInfoStore.ValidateValueProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeValidateValue()
        {
            return HasValue(GridStyleInfoStore.ValidateValueProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.ValidateValue"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasValidateValue
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.ValidateValueProperty);
            }
        }
        #endregion
        #region CurrencyEdit
        /// <summary>
        /// Gets or sets CurrencyEdit state. CurrencyEdit is itself an expandable object
        /// with several properties that can be set individually and participate
        /// in style inheritance mechanism.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the CurrencyEdit property is GridCurrencyEditInfo.Default.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="Currency"/>  (<see cref="GridCurrencyTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("CurrencyEdit state."),
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Category("Custom"),
        NotifyParentProperty(true)]
        public GridCurrencyEditInfo CurrencyEdit
        {
            get
            {
                return (GridCurrencyEditInfo)this.GetValue(GridStyleInfoStore.CurrencyEditProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.CurrencyEditProperty, value);
            }
        }

        /// <summary>
        /// Resets CurrencyEdit state.
        /// </summary>
        public void ResetCurrencyEdit()
        {
            this.ResetValue(GridStyleInfoStore.CurrencyEditProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCurrencyEdit()
        {
            return this.HasValue(GridStyleInfoStore.CurrencyEditProperty);
        }

        /// <summary>
        /// Gets a value indicating whether CurrencyEdit state has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCurrencyEdit
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.CurrencyEditProperty);
            }
        }
        #endregion
        #region MaskEdit
        /// <summary>
        /// Gets or sets MaskedEdit state. MaskedEdit is itself an expandable object
        /// with several properties that can be set individually and participate
        /// in style inheritance mechanism.
        /// </summary>
        [Description("MaskedEdit state."),
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Category("Custom"),
        NotifyParentProperty(true)]
        public GridMaskEditInfo MaskEdit
        {
            get
            {
                return (GridMaskEditInfo)this.GetValue(GridStyleInfoStore.MaskEditProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.MaskEditProperty, value);
            }
        }

        /// <summary>
        /// Resets MaskedEdit state.
        /// </summary>
        public void ResetMaskEdit()
        {
            this.ResetValue(GridStyleInfoStore.MaskEditProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeMaskEdit()
        {
            return this.HasValue(GridStyleInfoStore.MaskEditProperty);
        }

        /// <summary>
        /// Gets a value indicating whether MaskedEdit state has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasMaskEdit
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.MaskEditProperty);
            }
        }
        #endregion
        #region ProgressBar
        /// <summary>
        /// Gets or sets ProgressBar state. ProgressBar is itself an expandable object
        /// with several properties that can be set individually and participate
        /// in style inheritance mechanism.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the ProgressBar property is GridProgressBarInfo.Default.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="ProgressBar"/>  (<see cref="GridProgressBarCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("ProgressBar state."),
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Category("Custom"),
        NotifyParentProperty(true)]
        public GridProgressBarInfo ProgressBar
        {
            get
            {
                return (GridProgressBarInfo)this.GetValue(GridStyleInfoStore.ProgressBarProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.ProgressBarProperty, value);
            }
        }

        /// <summary>
        /// Resets ProgressBar state.
        /// </summary>
        public void ResetProgressBar()
        {
            this.ResetValue(GridStyleInfoStore.ProgressBarProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeProgressBar()
        {
            return this.HasValue(GridStyleInfoStore.ProgressBarProperty);
        }

        /// <summary>
        /// Gets a value indicating whether ProgressBar state has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasProgressBar
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.ProgressBarProperty);
            }
        }
        #endregion
        #region NumericUpDown
        /// <summary>
        /// Gets or sets <see cref="GridStyleInfo.NumericUpDown"/> to specify the step, minimum, and maximum value
        /// and if the value should start over when you reach the maximum value.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the NumericUpDown property is NULL.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Contains numeric up and down information for a cell."),
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        RefreshProperties(RefreshProperties.Repaint),
        SRCategory("StyleCategoryValue"),
        NotifyParentProperty(true)]
        public GridNumericUpDownCellInfo NumericUpDown
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridNumericUpDownCellInfo)GetValue(GridStyleInfoStore.NumericUpDownProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.NumericUpDownProperty, value);
            }
        }

        /// <internalonly/>
        /// <summary>Gets readonly numeric updown. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridNumericUpDownCellInfo ReadOnlyNumericUpDown
        {
            ////[DebuggerStepThrough()]
            get
            {
                if (HasNumericUpDown && !NumericUpDown.IsEmpty)
                {
                    return NumericUpDown;
                }

                if (Identity != null)
                {
                    GridStyleInfo updownInfo = this.Identity.GetBaseStyleNotEmptyExpandable(this, GridStyleInfoStore.NumericUpDownProperty) as GridStyleInfo;
                    if (updownInfo != null)
                    {
                        if (fixSubObjectsDerivedFromRowandColStyle && IsRowOrColumnStyle(updownInfo))
                        {
                            return NumericUpDown;
                        }

                        return updownInfo.NumericUpDown;
                    }
                }

                return Default.NumericUpDown;
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.NumericUpDown"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetNumericUpDown()
        {
            ResetValue(GridStyleInfoStore.NumericUpDownProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeNumericUpDown()
        {
            return HasValue(GridStyleInfoStore.NumericUpDownProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.NumericUpDown"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasNumericUpDown
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.NumericUpDownProperty);
            }
        }
        #endregion
        #region CheckBoxOptions
        /// <summary>
        /// Gets or sets flat look and values that represent checked, unchecked, and indeterminated state of the check box.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the CheckBoxOptions property is NULL.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Contains check box options of a cell."),
        Browsable(true),
        RefreshProperties(RefreshProperties.Repaint),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        SRCategory("StyleCategoryValue"),
        NotifyParentProperty(true)]
        public GridCheckBoxCellInfo CheckBoxOptions
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridCheckBoxCellInfo)GetValue(GridStyleInfoStore.CheckBoxOptionsProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.CheckBoxOptionsProperty, value);
            }
        }

        /// <internalonly/>
        /// <summary>Gets readonly Checkbox options. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridCheckBoxCellInfo ReadOnlyCheckBoxOptions
        {
            ////[DebuggerStepThrough()]
            get
            {
                if (HasCheckBoxOptions && !CheckBoxOptions.IsEmpty)
                {
                    return CheckBoxOptions;
                }

                if (Identity != null)
                {
                    GridStyleInfo checkBoxInfo = this.Identity.GetBaseStyleNotEmptyExpandable(this, GridStyleInfoStore.CheckBoxOptionsProperty) as GridStyleInfo;
                    if (checkBoxInfo != null)
                    {
                        if (fixSubObjectsDerivedFromRowandColStyle && IsRowOrColumnStyle(checkBoxInfo))
                        {
                            return CheckBoxOptions;
                        }

                        return checkBoxInfo.CheckBoxOptions;
                    }
                }

                return Default.CheckBoxOptions;
            }
        }
        
        /// <summary>
        /// Resets <see cref="GridStyleInfo.CheckBoxOptions"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetCheckBoxOptions()
        {
            ResetValue(GridStyleInfoStore.CheckBoxOptionsProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCheckBoxOptions()
        {
            return HasValue(GridStyleInfoStore.CheckBoxOptionsProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.CheckBoxOptions"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCheckBoxOptions
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.CheckBoxOptionsProperty);
            }
        }
        #endregion
        #region TextMargins
        /// <summary>
        /// Gets or sets text margins in pixels. When drawing a cell, this specifies the empty area between the
        /// text rectangle and the client rectangle of the cell without borders and cell buttons.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the TextMargins property is GridMarginsInfo.Default.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DataBoundRowHeader"/>  (<see cref="GridDataBoundRowHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Holds text margins in pixels. When drawing a cell, this specifies the empty area between the text rectangle and the client rectangle of the cell without borders and cell buttons."),
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        SRCategory("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public GridMarginsInfo TextMargins
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridMarginsInfo)GetValue(GridStyleInfoStore.TextMarginsProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.TextMarginsProperty, value);
            }
        }

        /// <internalonly/>
        /// <summary>Gets readonly text margins. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridMarginsInfo ReadOnlyTextMargins
        {
            ////[DebuggerStepThrough()]
            get
            {
                if (HasTextMargins && !TextMargins.IsEmpty)
                {
                    return TextMargins;
                }

                if (Identity != null)
                {
                    GridStyleInfo marginsInfo = this.Identity.GetBaseStyleNotEmptyExpandable(this, GridStyleInfoStore.TextMarginsProperty) as GridStyleInfo;
                    if (marginsInfo != null)
                    {
                        if (fixSubObjectsDerivedFromRowandColStyle && IsRowOrColumnStyle(marginsInfo))
                        {
                            return TextMargins;
                        }

                        return marginsInfo.TextMargins;
                    }
                }

                return Default.TextMargins;
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.TextMargins"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetTextMargins()
        {
            ResetValue(GridStyleInfoStore.TextMarginsProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeTextMargins()
        {
            return HasValue(GridStyleInfoStore.TextMarginsProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.TextMargins"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTextMargins
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.TextMarginsProperty);
            }
        }
        #endregion
        #region BorderMargins
        /// <summary>
        /// Gets or sets extra border margins in pixels. When drawing a cell, this specifies the area between the
        /// cell rectangle without border and the inner rectangle of the cell with cell buttons. Is most
        /// useful if you want to customize <see cref="GridControlBase.DrawCellFrameAppearance"/>
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the BorderMargins property is GridMarginsInfo.Empty.<para/>
        /// The property affects the behavior or appearance of call cell types:<para/>
        /// </remarks>
        [Description("Holds extra border margins in pixels. "),
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        SRCategory("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public GridMarginsInfo BorderMargins
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridMarginsInfo)GetValue(GridStyleInfoStore.BorderMarginsProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.BorderMarginsProperty, value);
            }
        }

        /// <internalonly/>
        /// <summary>Gets readonly border margins. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridMarginsInfo ReadOnlyBorderMargins
        {
            ////[DebuggerStepThrough()]
            get
            {
                if (HasBorderMargins && !BorderMargins.IsEmpty)
                {
                    return BorderMargins;
                }

                if (Identity != null)
                {
                    GridStyleInfo marginsInfo = this.Identity.GetBaseStyleNotEmptyExpandable(this, GridStyleInfoStore.BorderMarginsProperty) as GridStyleInfo;
                    if (marginsInfo != null)
                    {
                        if (fixSubObjectsDerivedFromRowandColStyle && IsRowOrColumnStyle(marginsInfo))
                        {
                            return BorderMargins;
                        }
                        
                        return marginsInfo.BorderMargins;
                    }
                }

                return Default.BorderMargins;
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.BorderMargins"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetBorderMargins()
        {
            ResetValue(GridStyleInfoStore.BorderMarginsProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBorderMargins()
        {
            return HasValue(GridStyleInfoStore.BorderMarginsProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.BorderMargins"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBorderMargins
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.BorderMarginsProperty);
            }
        }
        #endregion
        #region ShowButtons
        /// <summary>
        /// Gets or sets when to show or display the cell buttons. Possible choices are: show the button only for the current cell, always show buttons, or never show buttons.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the ShowButtons property is GridShowButtons.Show.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Specifies when to show or display the cell buttons. Possible choices are: show the button only for the current cell, always show buttons, or never show buttons."),
        Browsable(true),
        SRCategory("StyleCategoryAppearance")]
        [NotifyParentProperty(true)]
        public GridShowButtons ShowButtons
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridShowButtons)GetShortValue(GridStyleInfoStore.ShowButtonsProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.ShowButtonsProperty, (short)value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.ShowButtons"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetShowButtons()
        {
            ResetValue(GridStyleInfoStore.ShowButtonsProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowButtons()
        {
            return HasValue(GridStyleInfoStore.ShowButtonsProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.ShowButtons"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShowButtons
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.ShowButtonsProperty);
            }
        }
        #endregion
        #region PasswordChar
        /// <summary>
        /// Gets or sets the character used to mask characters of a password in a password-entry cell. The
        /// cells <see cref="GridStyleInfo.CellType"/> must be "OriginalTextBox".
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the PasswordChar property is blank.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="OriginalTextBox"/>  (<see cref="GridOriginalTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("The character used to mask characters of a password in a password-entry cell."),
        Browsable(true),
        SRCategory("StyleCategoryAppearance")]
            ////        DefaultValue(' ')        
        [NotifyParentProperty(true)]
        public char PasswordChar
        {
            [DebuggerStepThrough()]
            get
            {
                return (char)GetValue(GridStyleInfoStore.PasswordCharProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                if (value.Equals('\0'))
                {
                    ResetPasswordChar();
                }
                else
                {
                    SetValue(GridStyleInfoStore.PasswordCharProperty, value);
                }
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.PasswordChar"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetPasswordChar()
        {
            ResetValue(GridStyleInfoStore.PasswordCharProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializePasswordChar()
        {
            return HasValue(GridStyleInfoStore.PasswordCharProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.PasswordChar"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasPasswordChar
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.PasswordCharProperty);
            }
        }
        #endregion
        #region CellTipText
        /// <summary>
        /// Gets or sets ToolTip text to be displayed when user hovers mouse over cell.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the CellTipText property is String.Empty.
        /// <para/>
        /// The property affects the behavior or appearance of the following cell types:
        /// <para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DataBoundRowHeader"/>  (<see cref="GridDataBoundRowHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="PushButton"/>  (<see cref="GridPushButtonCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("ToolTip text to be displayed when user hovers mouse over cell."),
        Browsable(true),
        SRCategory("StyleCategoryAppearance")]
            ////        DefaultValue("")       
        [NotifyParentProperty(true)]
        public string CellTipText
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridStyleInfoStore.CellTipTextProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.CellTipTextProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.CellTipText"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetCellTipText()
        {
            ResetValue(GridStyleInfoStore.CellTipTextProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCellTipText()
        {
            return HasValue(GridStyleInfoStore.CellTipTextProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.CellTipText"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCellTipText
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.CellTipTextProperty);
            }
        }
        #endregion
        #region ImageIndex
        /// <summary>
        /// Gets or sets an image index that specifies an image inside a <see cref="GridStyleInfo.ImageList"/>.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the ImageIndex property is -1.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Holds an image index that specifies an image inside an image list."),
        Browsable(true),
        Category("Image"),
        TypeConverter(typeof(ImageIndexConverter)),
        System.ComponentModel.Editor(typeof(Syncfusion.Windows.Forms.Design.ImageIndexEditor), typeof(System.Drawing.Design.UITypeEditor))]
            ////        DefaultValue(-1)        
        [NotifyParentProperty(true)]
        public int ImageIndex
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(GridStyleInfoStore.ImageIndexProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.ImageIndexProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.ImageIndex"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetImageIndex()
        {
            ResetValue(GridStyleInfoStore.ImageIndexProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeImageIndex()
        {
            return HasValue(GridStyleInfoStore.ImageIndexProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.ImageIndex"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasImageIndex
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.ImageIndexProperty);
            }
        }
        #endregion
        #region ImageSizeMode
        /// <summary>
        /// Gets or sets how the image is displayed.
        /// </summary>
        /// <remarks>
        /// Valid values for this property are taken from the PictureBoxSizeMode
        /// enumeration. By default, in PictureBoxSizeMode.Normal mode, the Image
        /// is placed in the upper left corner of the PictureBox, and any part of
        /// the image too big for the PictureBox is clipped. Using the
        /// PictureBoxSizeMode.StretchImage value causes the image to stretch to
        /// fit the PictureBox.
        /// <para/>
        /// Using the PictureBoxSizeMode.AutoSize value causes the control to resize
        /// to always fit the image. Using the PictureBoxSizeMode.CenterImage value
        /// causes the image to be centered in the client area.
        /// <para/>
        /// </remarks>
        [Description("Indicates how the image is displayed."),
        Browsable(true),
        Category("Image")]
        [NotifyParentProperty(true)]
        public GridImageSizeMode ImageSizeMode
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridImageSizeMode)GetValue(GridStyleInfoStore.ImageSizeModeProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.ImageSizeModeProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.ImageSizeMode"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetImageSizeMode()
        {
            ResetValue(GridStyleInfoStore.ImageSizeModeProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeImageSizeMode()
        {
            return HasValue(GridStyleInfoStore.ImageSizeModeProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.ImageSizeMode"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasImageSizeMode
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.ImageSizeModeProperty);
            }
        }
        #endregion
        #region ImageList
        /// <summary>
        /// Gets or sets an <see cref="ImageList"/> that holds a collection of images. Cells can choose images with the <see cref="GridStyleInfo.ImageIndex"/> property in a <see cref="GridStyleInfo"/> instance.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the ImageList property is NULL.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("A list of images."),
        Browsable(true),
        RefreshProperties(RefreshProperties.Repaint),
        Category("Image")]
        [CloneableProperty(false), DisposeableProperty(false)] // REVIEW:
        [NotifyParentProperty(true)]
        public ImageList ImageList
        {
            [DebuggerStepThrough()]
            get
            {
                return (ImageList)GetValue(GridStyleInfoStore.ImageListProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.ImageListProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.ImageList"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetImageList()
        {
            ResetValue(GridStyleInfoStore.ImageListProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeImageList()
        {
            return HasValue(GridStyleInfoStore.ImageListProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.ImageList"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasImageList
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.ImageListProperty);
            }
        }
        #endregion
        #region BackgroundImageMode
        /// <summary>
        /// Gets or sets how the background image is displayed.
        /// </summary>
        /// <remarks>
        /// Valid values for this property are taken from the GridBackgroundImageMode
        /// enumeration. By default, in BackgroundImageModeMode.Normal mode, the Image
        /// is placed in the upper left corner of the cell(s), and any part of
        /// the image too big for the cell(s) is clipped. Using the
        /// GridBackgroundImageModeMode.StretchImage value causes the image to stretch to
        /// fit the cell(s).
        /// <para/>
        /// The default value for the BackgroundImageMode property is GridBackgroundImageMode.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DataBoundRowHeader"/>  (<see cref="GridDataBoundRowHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Currency"/>  (<see cref="GridCurrencyTextBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="MaskEdit"/>  (<see cref="GridMaskEditCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="FormulaCell"/>  (<see cref="GridFormulaCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="OriginalTextBox"/>  (<see cref="GridOriginalTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Indicates how the image is displayed."),
        Browsable(true),
        Category("Image")]
        [NotifyParentProperty(true)]
        public GridBackgroundImageMode BackgroundImageMode
        {
            get
            {
                ////TraceUtil.TraceCurrentMethodInfo();
                return (GridBackgroundImageMode)GetShortValue(GridStyleInfoStore.BackgroundImageModeProperty);
            }

            set
            {
                ////TraceUtil.TraceCurrentMethodInfo(value);
                SetValue(GridStyleInfoStore.BackgroundImageModeProperty, (short)value);
            }
        }

        /// <summary>
        /// Resets BackgroundImageMode state
        /// </summary>
        public void ResetBackgroundImageMode()
        {
            ResetValue(GridStyleInfoStore.BackgroundImageModeProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBackgroundImageMode()
        {
            return HasValue(GridStyleInfoStore.BackgroundImageModeProperty);
        }

        /// <summary>
        /// Gets a value indicating whether BackgroundImageMode state has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBackgroundImageMode
        {
            get
            {
                return HasValue(GridStyleInfoStore.BackgroundImageModeProperty);
            }
        }
        #endregion
        #region BackgroundImageID
        /// <summary>
        /// Gets or sets the Namespace ID that contains the cell's background information id.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the BackgroundImageID property is "".<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DataBoundRowHeader"/>  (<see cref="GridDataBoundRowHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Currency"/>  (<see cref="GridCurrencyTextBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="MaskEdit"/>  (<see cref="GridMaskEditCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="FormulaCell"/>  (<see cref="GridFormulaCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="OriginalTextBox"/>  (<see cref="GridOriginalTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Gets / sets the namespace id for the image to be displayed."),
        Browsable(false),
        Category("Image")]
            ////        DefaultValue("")        
        public string BackgroundImageID
        {
            get
            {
                ////TraceUtil.TraceCurrentMethodInfo();
                return (string)GetValue(GridStyleInfoStore.BackgroundImageIDProperty);
            }

            set
            {
                ////TraceUtil.TraceCurrentMethodInfo(value);
                SetValue(GridStyleInfoStore.BackgroundImageIDProperty, value);
            }
        }

        /// <summary>
        /// Resets BackgroundImageID state.
        /// </summary>
        public void ResetBackgroundImageID()
        {
            ResetValue(GridStyleInfoStore.BackgroundImageIDProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBackgroundImageID()
        {
            return HasValue(GridStyleInfoStore.BackgroundImageIDProperty);
        }

        /// <summary>
        /// Gets a value indicating whether BackgroundImage state has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBackgroundImageID
        {
            get
            {
                return HasValue(GridStyleInfoStore.BackgroundImageIDProperty);
            }
        }
        #endregion

        #region BackgroundImage
        /// <summary>
        /// Gets or sets the image that the cell displays as background.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the BackgroundImage property is NULL.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="CheckBox"/>  (<see cref="GridCheckBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DataBoundRowHeader"/>  (<see cref="GridDataBoundRowHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDown"/>  (<see cref="GridDropDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownColorUI"/>  (<see cref="GridDropDownColorUICellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGrid"/>  (<see cref="GridDropDownGridCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownGridListControl"/>  (<see cref="GridDropDownGridListControlCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownMonthCalendar"/>  (<see cref="GridDropDownMonthCalendarCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Header"/>  (<see cref="GridHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="NumericUpDown"/>  (<see cref="GridNumericUpDownCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="SortColumnHeader"/>  (<see cref="GridSortColumnHeaderCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Static"/>  (<see cref="GridStaticCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="TextBox"/>  (<see cref="GridTextBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="Currency"/>  (<see cref="GridCurrencyTextBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="MaskEdit"/>  (<see cref="GridMaskEditCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="FormulaCell"/>  (<see cref="GridFormulaCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="OriginalTextBox"/>  (<see cref="GridOriginalTextBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [Description("Gets / sets the image that the PictureBox displays."),
        Browsable(true),
        Category("Image")]
        [NotifyParentProperty(true)]
        public Image BackgroundImage
        {
            get
            {
                ////TraceUtil.TraceCurrentMethodInfo();
                return (Image)GetValue(GridStyleInfoStore.BackgroundImageProperty);
            }

            set
            {
                ////TraceUtil.TraceCurrentMethodInfo(value);
                SetValue(GridStyleInfoStore.BackgroundImageProperty, value);
            }
        }

        /// <summary>
        /// Resets BackgroundImage state.
        /// </summary>
        public void ResetBackgroundImage()
        {
            ResetValue(GridStyleInfoStore.BackgroundImageProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBackgroundImage()
        {
            return HasValue(GridStyleInfoStore.BackgroundImageProperty);
        }

        /// <summary>
        /// Gets a value indicating whether BackgroundImage state has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBackgroundImage
        {
            get
            {
                return HasValue(GridStyleInfoStore.BackgroundImageProperty);
            }
        }
        #endregion
        
        #region ParseFormats
        /// <summary>
        /// Gets or sets the permissible formats used to parse user entries of cell values.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the ParseFormats property is NULL.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// It can be used to specify various DateTime formats that are allowed when the user enters a DateTime cell value.
        /// <para/>
        /// </remarks>
        [Description("Specifies the permissible formats used to parse a user entry."),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRCategory("StyleCategoryValue")]
        [NotifyParentProperty(true)]
        public string[] ParseFormats
        {
            [DebuggerStepThrough()]
            get
            {
                string[] c = (string[])GetValue(GridStyleInfoStore.ParseFormatsProperty);
                //// TODO: ChoiceList Colledtion Editor-
                ////SetValue(GridStyleInfoStore.ParseFormatstProperty, c);
                return c;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.ParseFormatsProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.ParseFormats"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetParseFormats()
        {
            ResetValue(GridStyleInfoStore.ParseFormatsProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeParseFormats()
        {
            return HasValue(GridStyleInfoStore.ParseFormatsProperty);
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridStyleInfo.ParseFormats"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasParseFormats
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridStyleInfoStore.ParseFormatsProperty);
            }
        }
        #endregion

        ////        GridNumberFormatInfoAttribute
        ////        GridNumberFormatInfoAttribute ognfi = (GridNumberFormatInfoAttribute) style.CustomStyleProperties[typeof(GridNumberFormatInfoAttribute)];
        ////        if (ognfi != null)
        ////            nfi = ognfi.NumberFormatInfo;
        ////        GridCurrencyFormatInfoAttribute ogcfi = (GridCurrencyFormatInfoAttribute) style.CustomStyleProperties[typeof(GridCurrencyFormatInfoAttribute)];
        ////                                            if (ogcfi != null)
        ////                                                ogcfi.ApplyInfo(nfi);
        ////        NumberFormatInfo nfi = System.Globalization.CultureInfo.GetCultures(CultureTypes.AllCultures

#if SyncfusionFramework4_0
        /// <summary>
        /// Get the provider for the GridCell UI autommation
        /// </summary>
        [Browsable(false), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public GridCellUIAProvider Provider
        {
            get
            {
                return new GridCellUIAProvider(this.GetActiveGridView() as Control, this);
            }
        }
#elif SyncfusionFramework3_5
        /// <summary>
        /// Get the provider for the GridCell UI autommation
        /// </summary>
        [Browsable(false), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public GridCellUIAProvider Provider
        {
            get
            {
                return new GridCellUIAProvider(this.GetActiveGridView() as Control, this);
            }
        }

#endif
        /// <summary>
        /// Gets a formatted text for the default value for a specified <see cref="CellValueType"/>.
        /// </summary>
        [Description("Returns a formatted text for the default value for a specified Cell"),
        Browsable(false),
        RefreshProperties(RefreshProperties.Repaint),
        SRCategory("StyleCategoryValue"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FormatPreview
        {
            [DebuggerStepThrough()]
            get
            {
                object obj = GridCellValueConvert.GetDefaultValue(this.CellValueType);
                if (obj != null)
                {
                    return GetFormattedText(obj);
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the value formatted with the <see cref="GridStyleInfo.Format"/> mask and custom formatting of the <see cref="GridCellModelBase.GetFormattedText"/> method of the associated <see cref="GridCellModelBase"/> or sets the value by calling the <see cref="GridCellModelBase.ApplyFormattedText"/> of the associated <see cref="GridCellModelBase"/>.
        /// </summary>
        [Browsable(true),
        RefreshProperties(RefreshProperties.Repaint),
        SRCategory("StyleCategoryValue"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Gets the formatted text value")]
        [NotifyParentProperty(true)]
        public string FormattedText
        {
            get
            {
                object value = this.CellValue;
                try
                {
                    return GetFormattedText(value, GridCellBaseTextInfo.DisplayText);
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }

                    return value != null ? value.ToString() : string.Empty;
                }
            }

            set
            {
                if (!ApplyFormattedText(value, GridCellBaseTextInfo.DisplayText))
                {
                    throw new ArgumentException();
                }
            }
        }

        /// <overload>
        /// Return formatted text for the specified value.
        /// GridStyleInfo.CultureInfo is used for conversion to string.
        /// </overload>
        /// <summary>
        /// Return formatted text for the specified value.
        /// </summary>
        /// <param name="value">The value to be formatted.</param>
        /// <returns>A string that holds the formatted text.</returns>
        public string GetFormattedText(object value)
        {
            try
            {
                GridCellModelBase cellModel = this.CellModel;
                if (cellModel == null)
                {
                    CultureInfo ci = CultureInfo;
                    NumberFormatInfo nfi = ci != null ? ci.NumberFormat : null;
                    return GridCellValueConvert.FormatValue(value, CellValueType, Format, ci, nfi);
                }

                return cellModel.GetFormattedText(this, value, GridCellBaseTextInfo.DisplayText);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                return value != null ? value.ToString() : string.Empty;
            }
        }

        /// <overload>
        /// Parses the formatted text using Format and cell value type information stored in the current style object.
        /// The text is parsed using GridStyleInfo.CultureInfo information.
        /// </overload>
        /// <summary>
        /// Parses the formatted text using Format and cell value type information stored in the current style object.
        /// The text is parsed using GridStyleInfo.CultureInfo information.
        /// </summary>
        /// <param name="text">The formatted text.</param>
        /// <returns>True if the text could be parsed correctly and converted to a cell value.</returns>
        public bool ApplyFormattedText(string text)
        {
            GridCellModelBase cellModel = this.CellModel;
            if (cellModel == null)
            {
                CultureInfo ci = CultureInfo;
                NumberFormatInfo nfi = ci != null ? ci.NumberFormat : null;
                try
                {
                    CellValue = GridCellValueConvert.Parse(text, CellValueType, nfi, Format);
                    ResetError();
                }
                catch (Exception ex)
                {
                    Error = ex.Message;
                    if (StrictValueType)
                    {
                        throw;
                    }
                    else if (ex is FormatException || ex.InnerException is FormatException)
                    {
                        CellValue = text;
                        // possibly could also change CellValueType here based on input string
                        // e.Style.CellValueType = typeof(string);
                    }
                    else
                    {
                        throw;
                    }
                }

                return true;
            }

            return cellModel.ApplyFormattedText(this, text, GridCellBaseTextInfo.DisplayText);
        }

        /// <summary>
        /// Return formatted text for the specified value.
        /// GridStyleInfo.CultureInfo is used for conversion to string.
        /// </summary>
        /// <param name="value">The value to be formatted.</param>
        /// <param name="textInfo">A hint that specifies the current action why the text is formatted.</param>
        /// <returns>A string that holds the formatted text.</returns>
        public string GetFormattedText(object value, int textInfo)
        {
            GridCellModelBase cellModel = this.CellModel;
            if (cellModel == null)
            {
                CultureInfo ci = CultureInfo;
                NumberFormatInfo nfi = ci != null ? ci.NumberFormat : null;
                return GridCellValueConvert.FormatValue(value, CellValueType, Format, ci, nfi);
            }

            return cellModel.GetFormattedText(this, value, textInfo);
        }

        /// <summary>
        /// Parses the formatted text using Format and cell value type information stored in the current style object.
        /// The text is parsed using GridStyleInfo.CultureInfo information.
        /// </summary>
        /// <param name="text">The formatted text.</param>
        /// <param name="textInfo">A hint that specifies the current action why the text is parsed.</param>
        /// <returns>True if the text could be parsed correctly and converted to a cell value.</returns>
        public bool ApplyFormattedText(string text, int textInfo)
        {
            GridCellModelBase cellModel = this.CellModel;
            if (cellModel == null)
            {
                CultureInfo ci = CultureInfo;
                NumberFormatInfo nfi = ci != null ? ci.NumberFormat : null;
                CellValue = GridCellValueConvert.Parse(text, CellValueType, nfi, Format);
                return true;
            }

            return cellModel.ApplyFormattedText(this, text, textInfo);
        }

        /// <summary>
        /// Returns text for the specified value (ignoring any <see cref="Format"/> settings).
        /// CultureInfo.CurrentCulture is used for conversion to string.
        /// </summary>
        /// <param name="value">The value to be converted to string.</param>
        /// <returns>A string that represents the value.</returns>
        public string GetText(object value)
        {
            try
            {
                GridCellModelBase cellModel = this.CellModel;
                if (cellModel == null)
                {
                    if (value == null)
                    {
                        return string.Empty;
                    }

                    return (string)GridCellValueConvert.ChangeType(value, typeof(string), CultureInfo.CurrentCulture);
                }

                return cellModel.GetText(this, value);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                return value != null ? value.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Parses the text (ignoring any <see cref="Format"/> settings) and assigns it to CellValue.
        /// The text is parsed using CultureInfo.CurrentCulture information.
        /// </summary>
        /// <param name="text">The text to be parsed.</param>
        /// <returns>True if the text could be parsed correctly and converted to a cell value.</returns>
        public bool ApplyText(string text)
        {
            GridCellModelBase cellModel = this.CellModel;
            if (cellModel == null)
            {
                CultureInfo ci = CultureInfo.CurrentCulture;
                NumberFormatInfo nfi = ci != null ? ci.NumberFormat : null;
                try
                {
                    CellValue = GridCellValueConvert.Parse(text, CellValueType, nfi, string.Empty);
                    ResetError();
                }
                catch (Exception ex)
                {
                    Error = ex.Message;
                    if (StrictValueType)
                    {
                        throw;
                    }
                    else if (ex is FormatException || ex.InnerException is FormatException)
                    {
                        CellValue = text;
                        // possibly could also change CellValueType here based on input string
                        // e.Style.CellValueType = typeof(string);
                    }
                    else
                    {
                        throw;
                    }
                }

                return true;
            }

            return cellModel.ApplyText(this, text);
        }
    }

    /// <summary>
    ///    <para>Provides
    ///       a type converter to convert expandable objects to and from various
    ///       other representations.</para>
    /// </summary>
    internal class GridStyleInfoCustomPropertiesConverter :
        ExpandableObjectConverter
    {
        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <override/>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        } // end of method CanConvertTo

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <override/>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != null && destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
            {
                System.Reflection.ConstructorInfo constructorInfo = value.GetType().GetConstructor(new Type[] { });
                if (constructorInfo != null)
                {
                    return new System.ComponentModel.Design.Serialization.InstanceDescriptor(constructorInfo, new object[] { }, false);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        } // end of method ConvertTo
    }

    /// <summary>
    /// Provides a base class that you should derive from if you want to register additional
    /// custom properties with <see cref="GridStyleInfo"/>. Custom properties will be shown
    /// in the property grid for a style just like all other regular properties. You can
    /// also add expandable objects such as a font. Custom properties participate
    /// in the style inheritance mechanism similar to regular properties.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <example>
    /// See the following code samples how to get / set custom properties and
    /// a derived class with custom properties:
    /// <code lang="C#">
    /// GridControl grid = new GridControl();
    /// GridStyleInfo style = new GridStyleInfo();
    /// <para/>
    /// // using ctor with existing style object and caching the object (both C# and VB)
    /// MyCustomStyleProperties mcs = new MyCustomStyleProperties(style);
    /// mcs.TheLocked = true;
    /// mcs.TheFont.Bold = true;
    /// <para/>
    /// // design time code (both C# and VB)
    /// MyCustomStyleProperties myCustomStyleProperties1 = new MyCustomStyleProperties();
    /// myCustomStyleProperties1.TheLocked = true;
    /// myCustomStyleProperties1.TheFont.Bold = true;
    /// style.CustomProperties.Add(myCustomStyleProperties1);
    /// <para/>
    /// // using ctor with indexer (C# only)
    /// (new MyCustomStyleProperties(grid[1,1])).TheLocked = true;
    /// (new MyCustomStyleProperties(grid[1,1])).TheFont.Bold = true;
    /// <para/>
    /// // using ctor with existing style object (C# only)
    /// style.Text = "bla";
    /// new MyCustomStyleProperties(style).TheLocked = true;
    /// new MyCustomStyleProperties(style).TheFont.Bold = true;
    /// <para/>
    /// // explicit case (C# only)
    /// ((MyCustomStyleProperties) style).TheLocked = true;
    /// ((MyCustomStyleProperties) style).TheFont.Bold = true;
    /// </code>
    /// <code lang="VB">
    /// Dim style As New GridStyleInfo()
    /// Dim grid As New GridControl()
    /// <para/>
    /// ' using ctor with existing style object and caching the object (VB and C#)
    /// Dim mcs As New MyCustomStyleProperties(style)
    /// mcs.TheLocked = True
    /// mcs.TheFont.Bold = True
    /// <para/>
    /// ' design time code (VB and C#)
    /// Dim myCustomStyleProperties1 As New MyCustomStyleProperties()
    /// myCustomStyleProperties1.TheLocked = True
    /// myCustomStyleProperties1.TheFont.Bold = True
    /// style.CustomProperties.Add(myCustomStyleProperties1)
    /// <para/>
    /// ' with operator (Visual Basic only)
    /// With New MyCustomStyleProperties(style)
    ///     .TheLocked = True
    ///     .TheFont.Bold = True
    /// End With
    /// <para/>
    /// ' with operator (Visual Basic only)
    /// With New MyCustomStyleProperties(grid(1, 1))
    ///     .TheLocked = True
    ///     .TheFont.Bold = True
    /// End With
    /// </code>
    /// <code lang="C#">
    /// using System;
    /// using System.ComponentModel;
    /// <para/>
    /// using Syncfusion.Diagnostics;
    /// using Syncfusion.Styles;
    /// using Syncfusion.Windows.Forms;
    /// using Syncfusion.Windows.Forms.Grid;
    /// <para/>
    /// namespace WindowsApplication1
    /// {
    ///     public class MyCustomStyleProperties : GridStyleInfoCustomProperties
    ///     {
    ///         // static initialization of property descriptors
    ///         static Type t = typeof(MyCustomStyleProperties);
    /// <para/>
    ///         readonly static StyleInfoProperty LockedProperty = CreateStyleInfoProperty(t, "TheLocked");
    ///         readonly static StyleInfoProperty TheFontProperty = CreateStyleInfoProperty(t, "TheFont");
    /// <para/>
    ///         // default settings for all properties this object holds
    ///         static MyCustomStyleProperties defaultObject;
    /// <para/>
    ///         // initialize default settings for all properties in static ctor
    ///         static MyCustomStyleProperties ()
    ///         {
    ///             // all properties must be initialized for the Default property
    ///             defaultObject = new MyCustomStyleProperties(GridStyleInfo.Default);
    ///             defaultObject.TheLocked = true;
    ///             defaultObject.TheFont = GridFontInfo.Default;
    ///         }
    /// <para/>
    ///         /// <summary>
    ///         /// Provides access to default values for this type.
    ///         /// </summary>
    ///         public static MyCustomStyleProperties Default
    ///         {
    ///             get
    ///             {
    ///                 return defaultObject;
    ///             }
    ///         }
    /// <para/>
    ///         /// <summary>
    ///         /// Force static ctor being called at least once.
    ///         /// </summary>
    ///         public static void Initialize()
    ///         {
    ///         }
    /// <para/>
    ///         // Explicit cast from GridStyleInfo to MyCustomStyleProperties.
    ///         // (Note: this will only work for C#, Visual Basic does not support dynamic casts.)
    /// <para/>
    ///         /// <summary>
    ///         /// Explicit cast from GridStyleInfo to this custom property object.
    ///         /// </summary>
    ///         /// <returns>A new custom properties object.</returns>
    ///         public static explicit operator MyCustomStyleProperties(GridStyleInfo style)
    ///         {
    ///             return new MyCustomStyleProperties(style);
    ///         }
    /// <para/>
    ///         /// <summary>
    ///         /// Initializes a MyCustomStyleProperties object with a style object that holds all data.
    ///         /// </summary>
    ///         public MyCustomStyleProperties(GridStyleInfo style)
    ///             : base(style)
    ///         {
    ///         }
    /// <para/>
    ///         /// <summary>
    ///         /// Initializes a MyCustomStyleProperties object with an empty style object. Design-
    ///         /// time environment will use this ctor and later copy the values to a style object
    ///         /// by calling style.CustomProperties.Add(otherCustomStyleProperties1).
    ///         /// </summary>
    ///         public MyCustomStyleProperties()
    ///             : base()
    ///         {
    ///         }
    /// <para/>
    ///         /// <summary>
    ///         /// Gets / sets TheLocked state.
    ///         /// </summary>
    ///         [
    ///         Description("Specifies if ..."),
    ///         Browsable(true),
    ///         Category("StyleCategoryBehavior")
    ///         ]
    ///         public bool TheLocked
    ///         {
    ///             get
    ///             {
    ///                 TraceUtil.TraceCurrentMethodInfo();
    ///                 return (bool) style.GetValue(LockedProperty);
    ///             }
    ///             set
    ///             {
    ///                 TraceUtil.TraceCurrentMethodInfo(value);
    ///                 style.SetValue(LockedProperty, value);
    ///             }
    ///         }
    ///         /// <summary>
    ///         /// Resets TheLocked state.
    ///         /// </summary>
    ///         public void ResetTheLocked()
    ///         {
    ///             style.ResetValue(LockedProperty);
    ///         }
    ///         [EditorBrowsableAttribute(EditorBrowsableState.Never)]
    ///         private bool ShouldSerializeTheLocked()
    ///         {
    ///             return style.HasValue(LockedProperty);
    ///         }
    ///         /// <summary>
    ///         /// Gets if TheLocked state has been initialized for the current object.
    ///         /// </summary>
    ///         [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    ///         public bool HasTheLocked
    ///         {
    ///             get
    ///             {
    ///                 return style.HasValue(LockedProperty);
    ///             }
    ///         }
    /// <para/>
    ///         /// <summary>
    ///         /// Gets / sets TheFont state. TheFont is itself an expandable object
    ///         /// with several properties that can be set individually and participate
    ///         /// in style inheritance mechanism.
    ///         /// </summary>
    ///         [
    ///         Description("The font for drawing text."),
    ///         Browsable(true),
    ///         DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
    ///         Category("StyleCategoryAppearance")
    ///         ]
    ///         public GridFontInfo TheFont
    ///         {
    ///             get
    ///             {
    ///                 return (GridFontInfo) style.GetValue(TheFontProperty);
    ///             }
    ///             set
    ///             {
    ///                 style.SetValue(TheFontProperty, value);
    ///             }
    ///         }
    /// <para/>
    ///         /// <summary>
    ///         /// Resets TheFont state.
    ///         /// </summary>
    ///         public void ResetTheFont()
    ///         {
    ///             style.ResetValue(TheFontProperty);
    ///         }
    ///         [EditorBrowsableAttribute(EditorBrowsableState.Never)]
    ///         private bool ShouldSerializeTheFont()
    ///         {
    ///             return style.HasValue(TheFontProperty);
    ///         }
    ///         /// <summary>
    ///         /// Determines if TheFont state has been initialized for the current object.
    ///         /// </summary>
    ///         [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    ///         public bool HasTheFont
    ///         {
    ///             get
    ///             {
    ///                 return style.HasValue(TheFontProperty);
    ///             }
    ///         }
    /// <para/>
    ///     }
    /// <para/>
    ///     public class MyGridControl : GridControl
    ///     {
    ///         public MyGridControl()
    ///         {
    ///             // force static ctor of MyCustomStyleProperties being called at least once
    ///             MyCustomStyleProperties.Initialize();
    ///         }
    ///     }
    /// }
    /// </code>
    /// <code lang="VB">
    /// Public Class MyCustomStyleProperties
    ///     Inherits GridStyleInfoCustomProperties
    ///     ' static initialization of property descriptors
    ///     Private Shared t As Type = GetType(MyCustomStyleProperties)
    /// <para/>
    ///     Private Shared LockedProperty As StyleInfoProperty = CreateStyleInfoProperty(t, "TheLocked")
    ///     Private Shared TheFontProperty As StyleInfoProperty = CreateStyleInfoProperty(t, "TheFont")
    /// <para/>
    ///     ' default settings for all properties this object holds
    ///     Private Shared defaultObject As MyCustomStyleProperties
    /// <para/>
    /// <para/>
    ///     ' initialize default settings for all properties in static ctor
    ///     Shared Sub New()
    ///         ' all properties must be initialized for the Default property
    ///         defaultObject = New MyCustomStyleProperties(GridStyleInfo.Default)
    ///         defaultObject.TheLocked = True
    ///         defaultObject.TheFont = GridFontInfo.Default
    ///     End Sub 'New
    /// <para/>
    ///     '/ <summary>
    ///     '/ Provides access to default values for this type
    ///     '/ </summary>
    /// <para/>
    ///     Public Shared ReadOnly Property [Default]() As MyCustomStyleProperties
    ///         Get
    ///             Return defaultObject
    ///         End Get
    ///     End Property
    /// <para/>
    /// <para/>
    ///     '/ <summary>
    ///     '/ Force static ctor being called at least once.
    ///     '/ </summary>
    ///     Public Shared Sub Initialize()
    ///     End Sub 'Initialize
    /// <para/>
    ///     '/ <summary>
    ///     '/ Initializes a MyCustomStyleProperties object with a style object that holds all data.
    ///     '/ </summary>
    ///     Public Sub New(ByVal style As GridStyleInfo)
    ///         MyBase.New(style)
    ///     End Sub 'New
    /// <para/>
    /// <para/>
    ///     '/ <summary>
    ///     '/ Initializes a MyCustomStyleProperties object with an empty style object. Design-
    ///     '/ time environment will use this ctor and later copy the values to a style object
    ///     '/ by calling style.CustomProperties.Add(otherCustomStyleProperties1).
    ///     '/ </summary>
    ///     Public Sub New()
    ///     End Sub 'New
    /// <para/>
    ///     '/ <summary>
    ///     '/ Gets / sets TheLocked state.
    ///     '/ </summary>
    /// <para/>
    ///     'Description("Specifies if ..."), Browsable(True), Category("StyleCategoryBehavior")> _
    ///     Public Property TheLocked() As Boolean
    ///         Get
    ///             TraceUtil.TraceCurrentMethodInfo()
    ///             Return CBool(style.GetValue(LockedProperty))
    ///         End Get
    ///         Set(ByVal Value As Boolean)
    ///             TraceUtil.TraceCurrentMethodInfo(Value)
    ///             style.SetValue(LockedProperty, Value)
    ///         End Set
    ///     End Property
    /// <para/>
    ///     '/ <summary>
    ///     '/ Resets TheLocked state.
    ///     '/ </summary>
    ///     Public Sub ResetTheLocked()
    ///         style.ResetValue(LockedProperty)
    ///     End Sub 'ResetTheLocked
    /// <para/>
    ///     'EditorBrowsableAttribute(EditorBrowsableState.Never)> _
    ///     Private Function ShouldSerializeTheLocked() As Boolean
    ///         Return style.HasValue(LockedProperty)
    ///     End Function 'ShouldSerializeTheLocked
    ///     '/ <summary>
    ///     '/ Gets if TheLocked state has been initialized for the current object.
    ///     '/ </summary>
    /// <para/>
    ///     'Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    ///     Public ReadOnly Property HasTheLocked() As Boolean
    ///         Get
    ///             Return style.HasValue(LockedProperty)
    ///         End Get
    ///     End Property
    /// <para/>
    ///     '/ <summary>
    ///     '/ Gets / sets TheFont state. TheFont is itself an expandable object
    ///     '/ with several properties that can be set individually and participate
    ///     '/ in style inheritance mechanism.
    ///     '/ </summary>
    /// <para/>
    ///     'Description("The font for drawing text."), Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("StyleCategoryAppearance")> _
    ///     Public Property TheFont() As GridFontInfo
    ///         Get
    ///             Return CType(style.GetValue(TheFontProperty), GridFontInfo)
    ///         End Get
    ///         Set(ByVal Value As GridFontInfo)
    ///             style.SetValue(TheFontProperty, Value)
    ///         End Set
    ///     End Property
    /// <para/>
    /// <para/>
    ///     '/ <summary>
    ///     '/ Resets TheFont state.
    ///     '/ </summary>
    ///     Public Sub ResetTheFont()
    ///         style.ResetValue(TheFontProperty)
    ///     End Sub 'ResetTheFont
    /// <para/>
    ///     'EditorBrowsableAttribute(EditorBrowsableState.Never)> _
    ///     Private Function ShouldSerializeTheFont() As Boolean
    ///         Return style.HasValue(TheFontProperty)
    ///     End Function 'ShouldSerializeTheFont
    ///     '/ <summary>
    ///     '/ Determines if TheFont state has been initialized for the current object.
    ///     '/ </summary>
    /// <para/>
    ///     'Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    ///     Public ReadOnly Property HasTheFont() As Boolean
    ///         Get
    ///             Return style.HasValue(TheFontProperty)
    ///         End Get
    ///     End Property
    /// End Class 'MyCustomStyleProperties
    /// </code>
    /// </example>
    [TypeConverter(typeof(GridStyleInfoCustomPropertiesConverter))]
    public class GridStyleInfoCustomProperties
    {
        // instance data for type safe access to style information

        /// <summary>
        /// The <see cref="GridStyleInfo"/> that holds and
        /// gets the data for this custom property object.
        /// </summary>
        protected internal GridStyleInfo style;

        /// <summary>
        /// Initializes the <see cref="GridStyleInfoCustomPropertiesCollection"/> object
        /// with a <see cref="GridStyleInfo"/> that the properties of this
        /// class will belong to.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> that holds and
        /// gets the data for this custom property object.</param>
        protected GridStyleInfoCustomProperties(GridStyleInfo style)
        {
            this.style = style;
        }

        /// <summary>
        /// Initializes the <see cref="GridStyleInfoCustomPropertiesCollection"/> object
        /// with an empty <see cref="GridStyleInfo"/> object. When you later
        /// set the <see cref="StyleInfo"/> property, the changes in this object
        /// will be copied over to the new <see cref="GridStyleInfo"/> object.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridStyleInfo.CustomProperties"/> collection adds
        /// design-time support for custom properties by adding empty custom property
        /// objects and later calling <see cref="GridStyleInfoCustomPropertiesCollection.Add"/>,
        /// which will result in changing the <see cref="StyleInfo"/> property for this object
        /// and forces copying all properties of this object to the style object.
        /// </remarks>
        protected GridStyleInfoCustomProperties()
        {
            this.style = new GridStyleInfo();
        }

        /// <summary>
        /// Gets or sets the <see cref="GridStyleInfo"/> that holds and
        /// gets the data for this custom property object. When you
        /// set the <see cref="StyleInfo"/> property, all prior changes in this object
        /// will be copied over to the new <see cref="GridStyleInfo"/> object.
        /// </summary>
        public GridStyleInfo StyleInfo
        {
            get
            {
                return style;
            }

            set
            {
                if (value != style && style != null)
                {
                    value.ModifyStyle(style, Syncfusion.Styles.StyleModifyType.Override);
                }

                style = value;
            }
        }

        static StyleInfoProperty _CreateStyleInfoProperty(Type componentType, StaticData sd, Type type, string propertyName, StyleInfoPropertyOptions propertyOptions)
        {
            return sd.CreateStyleInfoProperty(type, propertyName, 0, false, componentType, propertyOptions);
        }

        /// <overload>
        /// Registers a new custom property.
        /// </overload>>
        /// <summary>
        /// Registers a new custom property.
        /// </summary>
        /// <param name="componentType">The type of your derived custom property class.</param>
        /// <param name="type">The type of the property.</param>
        /// <param name="propertyName">The name of the property. This must match a property member in your class.</param>
        /// <returns>A <see cref="StyleInfoProperty"/> object that you should use for getting and setting
        /// values.</returns>
        protected static StyleInfoProperty CreateStyleInfoProperty(Type componentType, Type type, string propertyName)
        {
            return _CreateStyleInfoProperty(componentType, GridStyleInfoStore.StaticData, type, propertyName, StyleInfoPropertyOptions.All);
        }

        /// <summary>
        /// Registers a new custom property.
        /// </summary>
        /// <param name="componentType">The type of your derived custom property class.</param>
        /// <param name="propertyName">The name of the property. This must match a property member in your class.</param>
        /// <returns>A <see cref="StyleInfoProperty"/> object that you should use for getting and setting
        /// values.</returns>
        protected static StyleInfoProperty CreateStyleInfoProperty(Type componentType, string propertyName)
        {
            System.Reflection.PropertyInfo pi = componentType.GetProperty(propertyName);
            Type type = pi.PropertyType;
            return _CreateStyleInfoProperty(componentType, GridStyleInfoStore.StaticData, type, propertyName, StyleInfoPropertyOptions.All);
        }

        /// <summary>
        /// Registers a new custom property.
        /// </summary>
        /// <param name="componentType">The type of your derived custom property class.</param>
        /// <param name="propertyName">The name of the property. This must match a property member in your class.</param>
        /// <param name="propertyOptions">Specifies attributes for the property.</param>
        /// <returns>A <see cref="StyleInfoProperty"/> object that you should use for getting and setting
        /// values.</returns>
        protected static StyleInfoProperty CreateStyleInfoProperty(Type componentType, string propertyName, StyleInfoPropertyOptions propertyOptions)
        {
            System.Reflection.PropertyInfo pi = componentType.GetProperty(propertyName);
            Type type = pi.PropertyType;
            return _CreateStyleInfoProperty(componentType, GridStyleInfoStore.StaticData, type, propertyName, propertyOptions);
        }

        /// <summary>
        /// Registers a new custom property.
        /// </summary>
        /// <param name="componentType">The type of your derived custom property class.</param>
        /// <param name="type">The type of the property.</param>
        /// <param name="propertyName">The name of the property. This must match a property member in your class.</param>
        /// <param name="propertyOptions">Specifies attributes for the property.</param>
        /// <returns>A <see cref="StyleInfoProperty"/> object that you should use for getting and setting
        /// values.</returns>
        protected static StyleInfoProperty CreateStyleInfoProperty(Type componentType, Type type, string propertyName, StyleInfoPropertyOptions propertyOptions)
        {
            return _CreateStyleInfoProperty(componentType, GridStyleInfoStore.StaticData, type, propertyName, propertyOptions);
        }
    }

    /// <summary>
    /// Implements a collection of custom property objects that have
    /// at least one initialized value. The primary purpose of this
    /// collection is to support design-time code serialization of
    /// custom properties.
    /// </summary>
    public class GridStyleInfoCustomPropertiesCollection : ICollection
    {
        StyleInfoBase styleInfo;
        Hashtable types = new Hashtable();

        /// <summary>
        /// Initializes a <see cref="GridStyleInfoCustomPropertiesCollection"/> with a reference
        /// to the parent style object.
        /// </summary>
        /// <param name="styleInfo">The style info</param>
        internal GridStyleInfoCustomPropertiesCollection(StyleInfoBase styleInfo)
        {
            this.styleInfo = styleInfo;
            ICollection sipsc = styleInfo.Store.StyleInfoProperties;
            Type styleInfoType = styleInfo.GetType();

            foreach (StyleInfoProperty sip in sipsc)
            {
                //// Check if ComponentType is a custom property types and if property is initialized.
                if (!sip.ComponentType.IsAssignableFrom(styleInfoType) && styleInfo.HasValue(sip))
                {
                    //// Only add one object even and ignore subsequent properties
                    if (!types.ContainsKey(sip.ComponentType))
                    {
                        types.Add(sip.ComponentType, Activator.CreateInstance(sip.ComponentType, new object[] { styleInfo }));
                    }
                    ////System.Diagnostics.Trace.WriteLine(sip.PropertyName + ":" + sip.ComponentType.Name + " != " + styleInfoType.Name);
                }
            }
        }

        /// <summary>
        /// Copies the initialized properties of the specified custom property
        /// to the parent style object and attaches the custom property object
        /// with the parent style object.
        /// </summary>
        /// <param name="value">A GridStyleInfoCustomProperties with
        /// custom properties.</param>
        public void Add(GridStyleInfoCustomProperties value)
        {
            value.StyleInfo = (GridStyleInfo)this.styleInfo;
        }

        #region ICollection Members
        bool ICollection.IsSynchronized
        {
            get
            {
                return types.Values.IsSynchronized;
            }
        }

        /// <summary>
        /// Gets the number of objects in this collection.
        /// </summary>
        public int Count
        {
            get
            {
                return types.Count;
            }
        }

        /// <summary>
        ///   <para>Copies the <see cref="GridStyleInfoCustomPropertiesCollection" /> elements to a one-dimensional <see cref="System.Array" /> at the specified index.</para>
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="System.Array" /> that is the destination of the object's from instance. The <see cref="System.Array" /> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(GridStyleInfoCustomProperties[] array, int index)
        {
            this.types.Values.CopyTo(array, index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((GridStyleInfoCustomProperties[])array, index);
        }

        object ICollection.SyncRoot
        {
            get
            {
                return types.Values.SyncRoot;
            }
        }
        #endregion
        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return types.Values.GetEnumerator();
        }
        #endregion
    }

    /// <summary>
    /// <see cref="GridStyleInfoSubObject"/> is an abstract base class for classes
    /// to be used as sub-objects in a <see cref="GridStyleInfo"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="GridStyleInfoSubObject"/> is derived from <see cref="StyleInfoBase"/>
    /// and thus provides the same easy way to provide properties that can inherit values
    /// from base styles at run-time.<para/>
    /// The difference is that <see cref="GridStyleInfoSubObject"/> supports this inheritance
    /// mechanism as a sub-object from a <see cref="GridStyleInfo"/>. A sub-object needs to
    /// have knowledge about its parent object and be able to walk the base styles from the
    /// parent object.<para/>
    /// Examples for implementation of <see cref="GridStyleInfoSubObject"/> are the font and border
    /// classes in Essential Grid.<para/>
    /// Programmers can derive their own style classes from <see cref="GridStyleInfoSubObject"/>
    /// and add type-safe (and intelli-sense)
    /// supported custom properties to the style class. If you write your own
    /// SpinButton class that needs individual properties, simply add a CellSpinButtonInfo
    /// class as subobject. If you derive CellSpinButtonInfo from GridStyleInfoSubObject,
    /// your new object will support property inheritance from base styles.
    /// <para/>
    /// See the overview for <see cref="StyleInfoBase"/> for further discussion about style objects.
    /// </remarks>
    /// <example>The following example shows how you can use the GridFontInfo class in Essential Grid:
    /// <code lang="C#">
    ///         standard.Font.Facename = "Helvetica";
    ///         model[1, 3].Font.Bold = true;
    ///         string faceName = model[1, 3].Font.Facename; // any cell inherits standard style
    ///         Console.WriteLIne(faceName); // will output "Helvetica"
    ///         Console.WriteLIne(model[1, 3].Font.Bold); // will output "true"
    ///         Console.WriteLIne(model[1, 3].Font.HasFaceName); // will output "False"
    /// </code>
    /// </example>
    public abstract class GridStyleInfoSubObject : StyleInfoSubObjectBase
    {
        /// <summary>
        /// Initalizes a new <see cref="GridStyleInfoSubObject"/> object and associates it with an existing <see cref="StyleInfoStore"/>.
        /// </summary>
        /// <param name="identity">A <see cref="StyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="StyleInfoBase"/>.
        /// <param name="store">A <see cref="StyleInfoStore"/> that holds data for this object.</param>
        /// All changes in this style object will saved in the <see cref="StyleInfoStore"/> object.
        /// </param>
        [DebuggerStepThrough()]
        public GridStyleInfoSubObject(StyleInfoSubObjectIdentity identity, StyleInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridStyleInfoSubObject"/> object and associates it with an existing <see cref="StyleInfoStore"/>.
        /// </summary>
        /// <param name="store">A <see cref="StyleInfoStore"/> that holds data for this object.
        /// All changes in this style object will be saved in the <see cref="StyleInfoStore"/> object.
        /// </param>
        [DebuggerStepThrough()]
        public GridStyleInfoSubObject(StyleInfoStore store)
            : base(store)
        {
        }

        /// <summary>
        /// Returns the <see cref="GridStyleInfo"/> this subobject belongs to.
        /// </summary>
        /// <returns>The parent style object.</returns>
        public GridStyleInfo GetGridStyleInfo()
        {
            StyleInfoSubObjectIdentity id = this.Identity as StyleInfoSubObjectIdentity;
            return (id != null) ? id.Owner as GridStyleInfo : null;
        }

        /// <summary>
        /// Return the <see cref="GridStyleInfoIdentity"/> with identity information about the parent style.
        /// </summary>
        /// <returns>The parent style's identity object.</returns>
        public GridStyleInfoIdentity GetCellIdentity()
        {
            GridStyleInfo cell = GetGridStyleInfo();
            GridStyleInfoIdentity cellId = (cell != null) ? cell.CellIdentity as GridStyleInfoIdentity : null;
            return cellId;
        }

        /// <summary>
        /// Returns the <see cref="GridModel"/> this style belongs to or NULL if the style is used outside a grid model.
        /// </summary>
        /// <returns>A reference to the grid model or NULL if the style is used outside a grid model.</returns>
        public GridModel GetGridModel()
        {
            GridStyleInfo cell = GetGridStyleInfo();
            return cell != null ? cell.GetGridModel() : null;
        }

        /// <summary>
        /// Returns the active <see cref="GridControlBase"/> for the <see cref="GridModel"/> this style belongs to or NULL
        /// if the style is used outside a grid model.
        /// </summary>
        /// <returns>A reference to the grid control base or NULL if the style is used outside a grid model.</returns>
        public GridControlBase GetActiveGridView()
        {
            GridStyleInfo cell = GetGridStyleInfo();
            return cell != null ? cell.GetActiveGridView() : null;
        }

        /// <summary>
        /// Gets results of ToString method.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Info
        {
            get
            {
                return ToString();
            }
        }

        /// <override/>
        /// <summary>
        /// Returns a string holding the current object.
        /// </summary>
        /// <returns>String representation of the current object.</returns>
        public override string ToString()
        {
            GridStyleInfo cell = GetGridStyleInfo();
            if (cell != null)
            {
                return cell.ToString();
            }

            return base.ToString();
        }
    }
}

