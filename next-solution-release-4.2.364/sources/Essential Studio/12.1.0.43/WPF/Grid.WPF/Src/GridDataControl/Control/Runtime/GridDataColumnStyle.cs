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
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Styles;

    public class GridDataColumnStyle : GridDataStyleInfo
    {
        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="GridDataColumnStyleStore"/>.
        /// </summary>
        /// <param name="store">A <see cref="GridDataColumnStyleStore"/> that holds data for this <see cref="GridDataColumnStyle"/>.
        /// All changes in this style object will be saved in the <see cref="GridDataColumnStyleStore"/> object.</param>
        public GridDataColumnStyle(GridStyleInfoStore store)
            : base(store)
        {
        }

        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="GridDataColumnStyleIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridDataColumnStyleIdentity"/> that holds the indentity for this <see cref="GridDataColumnStyle"/>.
        /// </param>
        public GridDataColumnStyle(StyleInfoIdentityBase identity)
            : base(identity, new GridStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="GridDataColumnStyleIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridDataColumnStyleIdentity"/> that holds the indentity for this <see cref="GridDataColumnStyle"/>.
        /// </param>
        /// <param name="store">A <see cref="GridDataColumnStyleStore"/> that holds data for this <see cref="GridDataColumnStyle"/>.
        /// All changes in this style object will be saved in the <see cref="GridDataColumnStyleStore"/> object.
        /// </param>
        public GridDataColumnStyle(StyleInfoIdentityBase identity, GridStyleInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Initalizes a new style object.
        /// </summary>
        public GridDataColumnStyle()
            : base(new GridStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new style object and copies all data from an existing style object.
        /// </summary>
        /// <param name="style">The style object that contains the original data.</param>
        public GridDataColumnStyle(GridDataColumnStyle style)
            : base(style.Store)
        {
        }

        /// <summary>
        /// The <see cref="GridDataColumnStyleStore"/> object that holds all the data for this style object.
        /// </summary>
        public new GridDataColumnStyleInfoStore Store
        {
            get { return (GridDataColumnStyleInfoStore)base.Store; }
        }

        /// <override/>
        public override StyleInfoSubObjectIdentity CreateSubObjectIdentity(StyleInfoProperty sip)
        {
            return new CachedStyleInfoSubObjectIdentity(this, sip);
        }

        /// <summary>
        /// Creates a new <see cref="GridDataColumnStyle"/> and copies its cell and identity information from the current object. The new
        /// instance will be made offline so that changes in this style object are not be stored in the GridData
        /// </summary>
        /// <returns>A new <see cref="GridDataColumnStyle"/> instance.</returns>
        /// <remarks>
        /// Lets a style object load base styles and default values but disables
        /// saving changes back to the grid. (see OnStyleChanged below)
        /// </remarks>
        public new GridDataColumnStyle GetOffLineCopy()
        {
            return new GridDataColumnStyle(((GridDataTableStyleInfoIdentity)Identity).MakeOfflineIdentity(), (GridStyleInfoStore)this.Store.Clone());
        }

        #region CellTypeEnum

        /// <summary>
        /// Gets or sets the ENUM value for a CellType.
        /// </summary>
        /// <value>The cell type enum.</value>
        public GridDataCellType CellTypeEnum
        {
            get
            {
                if (this.GetValue(GridDataColumnStyleInfoStore.CellTypeEnumProperty) == null)
                {
                    return GridDataCellType.TextBox;
                }
                return (GridDataCellType)this.GetValue(GridDataColumnStyleInfoStore.CellTypeEnumProperty);
            }

            set
            {
                this.CellType = value.ToString();
                this.SetValue(GridDataColumnStyleInfoStore.CellTypeEnumProperty, value);
            }
        }

        /// <summary>
        /// Resets the cell type enum.
        /// </summary>
        public void ResetCellTypeEnum()
        {
            this.ResetValue(GridDataColumnStyleInfoStore.CellTypeEnumProperty);
        }

        /// <summary>
        /// Gets a value indicating whether this instance has cell type enum.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has cell type enum; otherwise, <c>false</c>.
        /// </value>
        public bool HasCellTypeEnum
        {
            get
            {
                return this.HasValue(GridDataColumnStyleInfoStore.CellTypeEnumProperty);
            }
        }

        private bool ShouldSerializeCellTypeEnum()
        {
            return this.HasValue(GridDataColumnStyleInfoStore.CellTypeEnumProperty);
        }
        
        #endregion
    }

#if !SILVERLIGHT
    [Serializable]
#endif
    [StaticDataField("sd")]
    public class GridDataColumnStyleInfoStore : GridStyleInfoStore
    {
        private static StaticData sd = new StaticData(typeof(GridDataColumnStyleInfoStore), typeof(GridDataStyleInfo), false);

        public static readonly StyleInfoProperty CellTypeEnumProperty = sd.CreateStyleInfoProperty(typeof(GridDataCellType), "CellTypeEnum");

        /// <overload>
        /// Initializes a <see cref="GridDataColumnStyleInfoStore"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridDataColumnStyleInfoStore"/>.
        /// </summary>
        public GridDataColumnStyleInfoStore()
        {
            if (sd.IsEmpty)
            {
                new GridDataStyleInfo();
            }
        }

#if !SyncfusionFramework4_0 && !SILVERLIGHT
        /// <summary>
        /// Initializes a new <see cref="GridDataColumnStyleInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridDataColumnStyleInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (sd.IsEmpty)
            {
                new GridDataStyleInfo();
            }
        }
#endif

        //internal new static StaticData StaticData
        //{
        //    get
        //    {
        //        return sd;
        //    }
        //}

        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        /// <override/>
        public override object Clone()
        {
            StyleInfoStore target = new GridDataColumnStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }

    public enum GridDataCellType
    {
        /// <summary>
        /// Header Cell Type
        /// </summary>
        Header = 1,

        /// <summary>
        /// Static Cell Type
        /// </summary>
        Static = 2,

        /// <summary>
        /// TextBox Cell Type
        /// </summary>
        TextBox = 3,

        /// <summary>
        /// TextBlock Cell Type
        /// </summary>
        TextBlock = 4,

        /// <summary>
        /// CheckBox Cell Type
        /// </summary>
        CheckBox = 5,

        /// <summary>
        /// DataTemplate Cell Type 
        /// </summary>
        DataTemplate = 6,

        /// <summary>
        /// DataBoundTemplate Cell Type
        /// </summary>
        DataBoundTemplate = 7,

        /// <summary>
        /// Button Cell Type
        /// </summary>
        Button = 8,

        /// <summary>
        /// FormulaCell Cell Type
        /// </summary>
        FormulaCell = 9,

        /// <summary>
        /// MadkEdid Cell Type
        /// </summary>
        MaskEdit = 10,

        /// <summary>
        /// PercentEdit Cell Type
        /// </summary>
        PercentEdit = 11,

        /// <summary>
        /// DoubleEdit Cell Type
        /// </summary>
        DoubleEdit = 12,

        /// <summary>
        /// IntegerEdit Cell Type
        /// </summary>
        IntegerEdit = 13,

        /// <summary>
        /// CurrencyEdit Cell Type
        /// </summary>
        CurrencyEdit = 14,

        /// <summary>
        /// DateTimeEdit Cell Type
        /// </summary>
        DateTimeEdit = 15,

        /// <summary>
        /// UpdownEdit Cell Type
        /// </summary>
        UpDownEdit = 16,

        /// <summary>
        /// ComboBox Cell Type
        /// </summary>
        ComboBox = 17,

        /// <summary>
        /// DropDownList Cell Type
        /// </summary>
        DropDownList = 18,

        /// <summary>
        /// ImageCell Cell Type
        /// </summary>
        ImageCell = 19,

        /// <summary>
        /// ImageContent Cell Type
        /// </summary>
        ImageContent = 20,

        /// <summary>
        /// TimeSpanEdit Cell Type
        /// </summary>
        TimeSpanEdit = 21
    }
}