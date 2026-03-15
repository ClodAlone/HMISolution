//-------------------------------------------------------------------------------------------------
// <copyright file="GridBoundColumn.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
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
using System.Windows.Forms;
using System.Data;
using System.Text;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms;
using Syncfusion.Styles;
using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// GridBoundColumn provides information for a column to be used in a <see cref="GridDataBoundGrid"/>. GridBoundColumn
    /// can be customized with the visual studio designer where you can change header text, mapping name, and formatting
    /// of a column.
    /// </summary>
    /// <seealso cref="GridDataBoundGrid"/>
    /// <seealso cref="GridBoundColumnsCollection"/>
    [DesignTimeVisibleAttribute(false)]
    [ToolboxItemAttribute(false)]
    [DefaultPropertyAttribute("Header")]
    public class GridBoundColumn : Component, ICurrencyManagerSource, ICloneable
    {
        GridBoundColumnStyleInfo styleInfo;
        private PropertyDescriptor propertyDescriptor = null;
        private ICurrencyManagerSource owner = null;
        private string mappingName = string.Empty;
        private string headerName = string.Empty;
        ////        private bool invalid = false;
        ////        private bool updating = false;
        private bool readOnly = false;
        internal int width = -1;
        private bool isDefault = false;

        /// <summary>
        /// Occurs when the <see cref="PropertyDescriptor"/> property has changed.
        /// </summary>
        public event EventHandler PropertyDescriptorChanged;

        /// <summary>
        /// Occurs when the <see cref="HeaderText"/> property has changed.
        /// </summary>
        public event EventHandler HeaderTextChanged;
        
        /// <summary>
        /// Occurs when the <see cref="MappingName"/> property has changed.
        /// </summary>
        public event EventHandler MappingNameChanged;

        /// <summary>
        /// Occurs when the <see cref="ReadOnly"/> property has changed.
        /// </summary>
        public event EventHandler ReadOnlyChanged;

        /// <summary>
        /// Initializes an empty <see cref="GridBoundColumn"/>.
        /// </summary>
        public GridBoundColumn()
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (styleInfo != null)
                {
                    this.styleInfo.Dispose();
                    this.styleInfo = null;
                }

                this.owner = null;
                if (tag is IDisposable)
                {
                    ((IDisposable)tag).Dispose();
                }

                tag = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Returns a copy of this object.
        /// </summary>
        /// <returns>A <see cref="GridBoundColumn"/> object.</returns>
        public object Clone()
        {
            GridBoundColumn b = this.MemberwiseClone() as GridBoundColumn;
            if (styleInfo != null)
            {
                b.styleInfo = new GridBoundColumnStyleInfo(this, (GridStyleInfoStore)this.styleInfo.Store.Clone());
                if (tag is ICloneable)
                {
                    b.tag = ((ICloneable)tag).Clone();
                }
            }

            return b;
        }

        /// <overload>
        /// Initializes a <see cref="GridBoundColumn"/> with a <see cref="PropertyDescriptor"/> this column
        /// should be associated with.
        /// </overload>
        /// <summary>
        /// Initializes a <see cref="GridBoundColumn"/> with a <see cref="PropertyDescriptor"/> this column
        /// should be associated with.
        /// </summary>
        /// <param name="prop">The <see cref="PropertyDescriptor"/> that defines the data to be displayed in the column.</param>
        public GridBoundColumn(PropertyDescriptor prop)
        {
            PropertyDescriptor = prop;
            if (prop != null)
            {
                readOnly = prop.IsReadOnly;
                headerName = prop.Name;
                mappingName = prop.Name;
            }
        }

        internal GridBoundColumn(PropertyDescriptor prop, bool isDefault)
            : this(prop)
        {
            this.isDefault = isDefault;
            if (isDefault)
            {
                headerName = prop.Name;
                mappingName = prop.Name;
            }
        }

        CurrencyManager ICurrencyManagerSource.GetCurrencyManager()
        {
            if (this.owner != null)
            {
                return owner.GetCurrencyManager();
            }

            return null;
        }

        /// <summary>
        /// Gets or sets information how to format cells in the column.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridStyleInfo StyleInfo
        {
            get
            {
                if (styleInfo == null)
                {
                    styleInfo = new GridBoundColumnStyleInfo(this);
                }

                return styleInfo;
            }

            set
            {
                styleInfo = new GridBoundColumnStyleInfo(this, (GridStyleInfoStore)value.Store);
                ////styleInfo = value;
            }
        }

        bool ShouldSerializeStyleInfo()
        {
            return styleInfo != null && styleInfo.Store != null && !styleInfo.IsEmpty;
        }
        
        /// <summary>
        /// Gets or sets the <see cref="PropertyDescriptor"/> that defines the data to be displayed in the column.
        /// </summary>
        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        [DefaultValueAttribute(null)]
        [BrowsableAttribute(false)]
        public virtual PropertyDescriptor PropertyDescriptor
        {
            get
            {
                return propertyDescriptor;
            }

            set
            {
                if (propertyDescriptor != value)
                {
                    propertyDescriptor = value;
                    OnPropertyDescriptorChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="GridModelDataBinder"/> that this bound column is associated with.
        /// </summary>
        /// <remarks>
        /// Each <see cref="GridDataBoundGrid"/> has a <see cref="GridModelDataBinder"/>. The
        /// <see cref="GridModelDataBinder"/> provides loading and saving data from an external data source
        /// and supports BeginEdit, EndEdit, and AddNew functionality.
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual GridModelDataBinder Owner
        {
            get
            {
                return owner as GridModelDataBinder;
            }

            set
            {
                owner = value;
                ////                if (owner != null)
                ////                    owner.IsDesignMode = this.DesignMode;
            }
        }
        
        /// <summary>
        /// Sets the <see cref="ICurrencyManagerSource"/> this bound column is associated with.
        /// </summary>
        /// <param name="owner">The ICurrencyManagerSource object.</param>
        public void SetOwner(ICurrencyManagerSource owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Gets or sets the header text to be displayed in the column header.
        /// </summary>
        [CategoryAttribute("Display")]
        [LocalizableAttribute(true)]
        public virtual string HeaderText
        {
            get
            {
                return headerName;
            }

            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }

                if (headerName.Equals(value))
                {
                    return;
                }

                headerName = value;
                OnHeaderTextChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Resets the header text.
        /// </summary>
        public void ResetHeaderText()
        {
            HeaderText = string.Empty;
        }

        /// <summary>
        /// Gets or sets the mapping for this column. You should specify which column of a <see cref="DataTable"/>
        /// you want to display in the grid at this column.
        /// </summary>
        [TypeConverter(typeof(CurrencyManagerMappingNameConverter))]
        [DefaultValue("")]
        public string MappingName
        {
            get
            {
                return mappingName;
            }

            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }

                if (mappingName.Equals(value))
                {
                    return;
                }

                string str = mappingName;
                mappingName = value;
                try
                {
                    if (this.Owner != null)
                    {
                        Owner.GridBoundColumns.CheckForMappingNameDuplicates(this);
                    }
                }
                catch
                {
                    mappingName = str;
                    throw;
                }

                OnMappingNameChanged(EventArgs.Empty);
            }
        }

        string Name
        {
            get
            {
                if (this.PropertyDescriptor != null)
                {
                    return this.PropertyDescriptor.Name;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private bool ShouldSerializeHeaderText()
        {
            return headerName.Length != 0;
        }

        object tag = null;

        /// <summary>
        /// Gets or sets a tag that is associated with this object.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public object Tag
        {
            get
            {
                return tag;
            }

            set
            {
                tag = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Read-only state of the column.
        /// </summary>
        [DefaultValueAttribute(false)]
        public virtual bool ReadOnly
        {
            get
            {
                return readOnly;
            }

            set
            {
                if (readOnly != value)
                {
                    readOnly = value;
                    OnReadOnlyChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the backcolor of the <see cref='GridBoundColumn'/>
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color BackColor
        {
            get
            {
                return this.StyleInfo.BackColor;
            }
            set
            {
                if (this.StyleInfo.BackColor != value)
                    this.StyleInfo.BackColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the <see cref='GridBoundColumn'/>
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Width
        {
            get
            {
                return this.Owner.gridModel.ColWidths[this.MappingName];
            }
            set
            {
                if (this.Owner.gridModel.ColWidths[this.MappingName] != value)
                    this.Owner.gridModel.ColWidths[this.MappingName] = value;
            }
        }

        /// <summary>
        /// Gets or sets the hidden state of the <see cref='GridBoundColumn'/>
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Hidden
        {
            get
            {
                return this.Owner.gridModel.ColWidths[this.MappingName] == 0;
            }
            set
            {
                if (this.Owner.gridModel.HideCols[this.MappingName] != value)
                    this.Owner.gridModel.HideCols[this.MappingName] = value;
            }
        }

        /// <summary>
        /// Gets or sets the position of the <see cref='GridBoundColumn'/>
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Position
        {
            get
            {
                return this.Owner.gridModel.NameToColIndex(this.MappingName);
            }
            set
            {
                this.Owner.gridModel.Cols.MoveRange(this.Owner.gridModel.NameToColIndex(this.MappingName), value);
            }
        }

        private void OnReadOnlyChanged(EventArgs e)
        {
#if DEBUG
            if (Switches.GridBoundColumnEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.Name, this.ReadOnly);
            }
#else
            ;
#endif

            if (ReadOnlyChanged != null)
            {
                ReadOnlyChanged(this, e);
            }
        }
        ////
        ////        internal void BeginUpdate()
        ////        {
        ////            updating = true;
        ////        }
        ////
        ////        internal void EndUpdate()
        ////        {
        ////            updating = false;
        ////            if (invalid)
        ////            {
        ////                invalid = false;
        ////                //Invalidate();
        ////            }
        ////        }

        private void OnPropertyDescriptorChanged(EventArgs e)
        {
#if DEBUG
            if (Switches.GridBoundColumnEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.Name);
            }
#else
            ;
#endif
            if (PropertyDescriptorChanged != null)
            {
                PropertyDescriptorChanged(this, e);
            }
        }

        private void OnHeaderTextChanged(EventArgs e)
        {
#if DEBUG
            if (Switches.GridBoundColumnEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.Name, this.HeaderText);
            }
#else
            ;
#endif
            if (HeaderTextChanged != null)
            {
                HeaderTextChanged(this, e);
            }
        }

        private void OnMappingNameChanged(EventArgs e)
        {
#if DEBUG
            if (Switches.GridBoundColumnEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.Name, this.mappingName);
            }
#else
            ;
#endif
            if (MappingNameChanged != null)
            {
                MappingNameChanged(this, e);
            }
        }

        internal object value = null;
        internal bool dirty = false;
        internal object savedValue = null;
    }

    [Syncfusion.Documentation.DocumentationExclude()]
    [DebuggerStepThrough()]
    internal class GridBoundColumnStyleInfoIdentity : StyleInfoIdentityBase
    {
        GridBoundColumn boundColumn;
        protected IStyleInfo[] savedBaseStyles = null;

        // Cache
        public GridBoundColumnStyleInfoIdentity(GridBoundColumn col)
        {
            this.boundColumn = col;
        }

        public override void Dispose()
        {
            savedBaseStyles = null;
            boundColumn = null;
            base.Dispose();
        }

        /// <summary>
        /// Returns an array with base styles for the specified style object.
        /// </summary>
        /// <param name="thisStyleInfo">The style object.</param>
        /// <returns>
        /// An array of style objects that are base styles for the current style object.
        /// </returns>
        /// <override/>
        public override IStyleInfo[] GetBaseStyles(IStyleInfo thisStyleInfo)
        {
            if (savedBaseStyles == null && this.boundColumn != null)
            {
                int level;
                GridStyleInfo style = thisStyleInfo as GridStyleInfo;
                string baseStyleName = style.HasBaseStyle ? style.BaseStyle : string.Empty;
                if (boundColumn.Owner == null || boundColumn.Owner.gridModel == null)
                {
                    savedBaseStyles = new GridStyleInfo[0];
                }
                else
                {
                    GridStyleInfo[] infoMapStyles = boundColumn.Owner.gridModel.BaseStylesMap.GetBaseStylesMapStyles(baseStyleName, out level);
                    savedBaseStyles = new GridStyleInfo[level];
                    if (infoMapStyles != null)
                    {
                        Array.Copy(infoMapStyles, 0, savedBaseStyles, 0, level);
                    }
                }
            }

            return savedBaseStyles;
        }

        /// <override/>
        public override void OnStyleChanged(StyleInfoBase style, StyleInfoProperty sip)
        {
            savedBaseStyles = null;
        }
    }

    [TypeConverter(typeof(StyleInfoBaseConverter))]
    class GridBoundColumnStyleInfo : GridStyleInfo
    {
        public GridBoundColumnStyleInfo(GridBoundColumn boundColumn, GridStyleInfoStore store)
            : base((StyleInfoIdentityBase)new GridBoundColumnStyleInfoIdentity(boundColumn), store)
        {
        }

        public GridBoundColumnStyleInfo(GridBoundColumn boundColumn)
            : base(new GridBoundColumnStyleInfoIdentity(boundColumn))
        {
        }
    }
}
