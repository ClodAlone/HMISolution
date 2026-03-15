#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Cells;

using Syncfusion.Windows.Styles;

namespace Syncfusion.Windows.Controls.VirtualTreeView
{
    /// <summary>
    /// Maintains appearance and value for a cell in a <see cref="VirtualTreeView"/>. The
    /// style object also provides shortcut properties to access the TreeModel,
    /// TreeNode, Column, Level and CellRowColumnIndex the cell style represents.
    /// </summary>
    public class TreeStyleInfo : StyleInfoBase, IRenderCellInfo
    {
        #region Fields
        // Static Fields.
        private static TreeStyleInfo defaultStyle = null;

        internal bool Locked = false;

        /// <summary>
        /// An empty style object.
        /// </summary>
        public static readonly TreeStyleInfo Empty = new TreeStyleInfo();
        #endregion
        #region Ctor
        // Constructors.
        static TreeStyleInfo()
        {
        }

        /// <summary>
        /// Initalizes a new style object.
        /// </summary>
        public TreeStyleInfo()
            : base(new TreeStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new style object and copies all data from an existing style object.
        /// </summary>
        /// <param name="style">The style object that contains the original data.</param>
        public TreeStyleInfo(TreeStyleInfo style)
            : base(style.Store)
        {
        }

        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="TreeStyleInfoStore"/>.
        /// </summary>
        /// <param name="store">A <see cref="TreeStyleInfoStore"/> that holds data for this <see cref="TreeStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="TreeStyleInfoStore"/> object.</param>
        public TreeStyleInfo(TreeStyleInfoStore store)
            : base(store)
        {
        }

        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="TreeStyleInfoIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="TreeStyleInfoIdentity"/> that holds the indentity for this <see cref="TreeStyleInfo"/>.</param>
        public TreeStyleInfo(StyleInfoIdentityBase identity)
            : base(identity, new TreeStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="TreeStyleInfoIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="TreeStyleInfoIdentity"/> that holds the indentity for this <see cref="TreeStyleInfo"/>.</param>
        /// <param name="store">A <see cref="TreeStyleInfoStore"/> that holds data for this <see cref="TreeStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="TreeStyleInfoStore"/> object.</param>
        public TreeStyleInfo(StyleInfoIdentityBase identity, TreeStyleInfoStore store)
            : base(identity, store)
        {
        }
        #endregion
        #region Identity
        /// <summary>
        /// Holds identity information such as row and column index for the current <see cref="TreeStyleInfo"/>.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TreeStyleInfoIdentity TreeNodeIdentity
        {
            get
            {
                return base.Identity as TreeStyleInfoIdentity;
            }
            set
            {
                base.Identity = value;
            }
        }

        /// <summary>
        /// Gets the tree model.
        /// </summary>
        /// <value>The tree model.</value>
        public TreeModel TreeModel
        {
            get
            {
                TreeStyleInfoIdentity treeNodeIdentity = TreeNodeIdentity;
                if (treeNodeIdentity != null)
                    return treeNodeIdentity.TreeModel;
                return null;
            }
        }

        /// <summary>
        /// Gets the tree node.
        /// </summary>
        /// <value>The tree node.</value>
        public TreeNode TreeNode
        {
            get
            {
                TreeStyleInfoIdentity treeNodeIdentity = TreeNodeIdentity;
                if (treeNodeIdentity != null)
                    return treeNodeIdentity.TreeNode;
                return null;
            }
        }

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <value>The column.</value>
        public TreeColumn Column
        {
            get
            {
                TreeStyleInfoIdentity treeNodeIdentity = TreeNodeIdentity;
                if (treeNodeIdentity != null)
                    return treeNodeIdentity.Column;
                return null;
            }
        }

        /// <summary>
        /// Gets the level.
        /// </summary>
        /// <value>The level.</value>
        public TreeLevel Level
        {
            get
            {
                TreeStyleInfoIdentity treeNodeIdentity = TreeNodeIdentity;
                if (treeNodeIdentity != null)
                    return treeNodeIdentity.Level;
                return null;
            }
        }

        /// <summary>
        /// Gets the index of the cell row column.
        /// </summary>
        /// <value>The index of the cell row column.</value>
        public RowColumnIndex CellRowColumnIndex
        {
            get
            {
                TreeStyleInfoIdentity treeNodeIdentity = TreeNodeIdentity;
                if (treeNodeIdentity != null)
                    return TreeNodeIdentity.CellRowColumnIndex;
                return RowColumnIndex.Empty;
            }
        }

        /// <summary>
        /// Gets the index of the row.
        /// </summary>
        /// <value>The index of the row.</value>
        public int RowIndex
        {
            get
            {
                return CellRowColumnIndex.RowIndex;
            }
        }

        /// <summary>
        /// Gets the index of the column.
        /// </summary>
        /// <value>The index of the column.</value>
        public int ColumnIndex
        {
            get
            {
                return CellRowColumnIndex.ColumnIndex;
            }
        }

        /// <summary>
        /// The <see cref="TreeStyleInfoStore"/> object that holds all the data for this style object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new TreeStyleInfoStore Store
        {
            get { return (TreeStyleInfoStore)base.Store; }
        }

        ///// <override/>
        //public override void ModifyStyle(IStyleInfo istyle, StyleModifyType mt)
        //{
        //    base.ModifyStyle(istyle, mt);

        //    TreeStyleInfoIdentity cellId = base.Identity as TreeStyleInfoIdentity;
        //    {
        //        TreeStyleInfo style = istyle as TreeStyleInfo;
        //        if (style == null)
        //        {
        //            TreeStyleInfoStore store = istyle as TreeStyleInfoStore;
        //            if (store != null)
        //                style = new TreeStyleInfo(store);
        //        }

        //        OnStyleChanged(null);
        //    }
        //}

        /// <summary>
        /// Override this method to create a product-specific identity object for a sub object.
        /// </summary>
        /// <param name="sip"></param>
        /// <returns>
        /// An identity object for a subobject of this style.
        /// </returns>
        /// <example>
        /// The following code is an example how Essential Grid creates GridStyleInfoSubObjectIdentity:
        /// <code lange="C#">
        /// public override StyleInfoSubObjectIdentity CreateSubObjectIdentity(StyleInfoProperty sip)
        /// {
        /// return new GridStyleInfoSubObjectIdentity(this, sip);
        /// }
        /// </code>
        /// </example>
        public override StyleInfoSubObjectIdentity CreateSubObjectIdentity(StyleInfoProperty sip)
        {
            return new CachedStyleInfoSubObjectIdentity(this, sip);
        }

        ///// <summary>
        ///// Creates a new <see cref="TreeStyleInfo"/> and copies its cell and identity information from the current object. The new
        ///// instance will be made offline so that changes in this style object are not be stored in the GridData
        ///// </summary>
        ///// <returns>A new <see cref="TreeStyleInfo"/> instance.</returns>
        ///// <remarks>
        ///// Lets a style object load base styles and default values but disables
        ///// saving changes back to the grid. (see OnStyleChanged below)
        ///// </remarks>
        //public TreeStyleInfo GetOffLineCopy()
        //{
        //    return new TreeStyleInfo(((TreeStyleInfoIdentity)Identity).MakeOfflineIdentity(), (TreeStyleInfoStore)Store.Clone());
        //}
        #endregion
        #region Default
        /// <summary>
        /// Gets the default style settings.
        /// </summary>
        /// <value>The default.</value>
        public static TreeStyleInfo Default
        {
            get
            {
                if (TreeStyleInfo.defaultStyle == null)
                {
                    defaultStyle = new TreeStyleInfo();
                    defaultStyle.Background = null;
                    defaultStyle.BorderMargins = CellMarginsInfo.Empty;
                    defaultStyle.Borders = CellBordersInfo.Default;
                    defaultStyle.CellRenderer = null;// new CellStaticTextRenderer();
                    defaultStyle.CellTemplateKey = "";
                    defaultStyle.CellTemplate = null;
                    defaultStyle.CellValueType = null;
                    defaultStyle.CellValue = "";
                    defaultStyle.CultureInfo = null;
                    defaultStyle.Format = "";
                }

                return TreeStyleInfo.defaultStyle;
            }
        }

        /// <summary>
        /// Override this method to return a default style object for your derived class.
        /// </summary>
        /// <returns>A default style object.</returns>
        /// <remarks>
        /// You should cache the default style object in a static field.
        /// </remarks>
        protected internal override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }

        #endregion

        // Store properties
        #region Background
        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>The background.</value>
        [
        Description(""),
        Browsable(true),
        Category(""),
        ]
        public Brush Background
        {

            get
            {
                return (Brush)GetValue(TreeStyleInfoStore.BackgroundProperty);
            }
            set
            {
                SetValue(TreeStyleInfoStore.BackgroundProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="TreeStyleInfo.Background"/>.
        /// </summary>
        public void ResetBackground()
        {
            ResetValue(TreeStyleInfoStore.BackgroundProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBackground()
        {
            return HasValue(TreeStyleInfoStore.BackgroundProperty);
        }

        /// <summary>
        /// Determines if <see cref="TreeStyleInfo.Background"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBackground
        {

            get
            {
                return HasValue(TreeStyleInfoStore.BackgroundProperty);
            }
        }
        #endregion
        #region BorderMargins
        /// <summary>
        /// Gets or sets the border margins.
        /// </summary>
        /// <value>The border margins.</value>
        [
        Description(""),
        Browsable(true),
        Category("")
        ]
        public CellMarginsInfo BorderMargins
        {

            get
            {
                return (CellMarginsInfo)GetValue(TreeStyleInfoStore.BorderMarginsProperty);
            }

            set
            {
                SetValue(TreeStyleInfoStore.BorderMarginsProperty, value);
            }
        }

        /// <internalonly/>
        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CellMarginsInfo ReadOnlyBorderMargins
        {
            //
            get
            {
                if (HasBorderMargins && !BorderMargins.IsEmpty)
                    return BorderMargins;

                if (Identity != null)
                {
                    TreeStyleInfo marginsInfo = this.Identity.GetBaseStyleNotEmptyExpandable(this, TreeStyleInfoStore.BorderMarginsProperty) as TreeStyleInfo;
                    if (marginsInfo != null)
                    {
                        //if (fixSubObjectsDerivedFromRowandColStyle && IsRowOrColumnStyle(marginsInfo))
                        //    return BorderMargins;

                        return marginsInfo.BorderMargins;
                    }
                }
                return Default.BorderMargins;
            }
        }

        /// <summary>
        /// Resets <see cref="TreeStyleInfo.BorderMargins"/>.
        /// </summary>
        public void ResetBorderMargins()
        {
            ResetValue(TreeStyleInfoStore.BorderMarginsProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBorderMargins()
        {
            return HasValue(TreeStyleInfoStore.BorderMarginsProperty);
        }

        /// <summary>
        /// Determines if <see cref="TreeStyleInfo.BorderMargins"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBorderMargins
        {

            get
            {
                return HasValue(TreeStyleInfoStore.BorderMarginsProperty);
            }
        }
        #endregion
        #region Borders
        /// <summary>
        /// Gets or sets the borders.
        /// </summary>
        /// <value>The borders.</value>
        [
        Description("Top, left, bottom, and right border settings."),
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Category(""),
        ]
        public CellBordersInfo Borders
        {

            get
            {
                return (CellBordersInfo)GetValue(TreeStyleInfoStore.BordersProperty);
            }
            set
            {
                SetValue(TreeStyleInfoStore.BordersProperty, value);
            }
        }

        /// <internalonly/>
        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CellBordersInfo ReadOnlyBorders
        {
            get
            {
                if (HasBorders && !Borders.IsEmpty)
                    return Borders;

                if (Identity != null)
                {
                    TreeStyleInfo bordersInfo = this.Identity.GetBaseStyleNotEmptyExpandable(this, TreeStyleInfoStore.BordersProperty) as TreeStyleInfo;
                    if (bordersInfo != null)
                    {
                        //if (fixSubObjectsDerivedFromRowandColStyle && IsRowOrColumnStyle(bordersInfo))
                        //    return Borders;

                        return bordersInfo.Borders;
                    }
                }

                return Default.Borders;
            }
        }

        /// <summary>
        /// Resets the borders information.
        /// </summary>
        public void ResetBorders()
        {
            ResetValue(TreeStyleInfoStore.BordersProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBorders()
        {
            return HasValue(TreeStyleInfoStore.BordersProperty);
        }
        /// <summary>
        /// Determines if border information has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBorders
        {

            get
            {
                return HasValue(TreeStyleInfoStore.BordersProperty);
            }
        }
        #endregion
        #region CellRenderer
        /// <summary>
        /// Gets or sets the cell renderer.
        /// </summary>
        /// <value>The cell renderer.</value>
        [
        Description("Contains cell type information of a cell."),
        Browsable(true),
        Category("")
        ]
        public TreeCellRenderer CellRenderer
        {
            get
            {
                return (TreeCellRenderer)GetValue(TreeStyleInfoStore.CellRendererProperty);
            }
            set
            {
                SetValue(TreeStyleInfoStore.CellRendererProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="TreeStyleInfo.CellRenderer"/>.
        /// </summary>
        public void ResetCellRenderer()
        {
            ResetValue(TreeStyleInfoStore.CellRendererProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCellRenderer()
        {
            return HasValue(TreeStyleInfoStore.CellRendererProperty);
        }

        /// <summary>
        /// Determines if <see cref="TreeStyleInfo.CellRenderer"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCellRenderer
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(TreeStyleInfoStore.CellRendererProperty);
            }
        }
        #endregion
        #region CellTemplate
        /// <summary>
        /// Gets or sets the cell template.
        /// </summary>
        /// <value>The cell template.</value>
        [
        Description("The DataTemplate used to display an item."),
        Browsable(true),
        Category(""),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public DataTemplate CellTemplate
        {
            get
            {
                return (DataTemplate)GetValue(TreeStyleInfoStore.CellTemplateProperty);
            }
            set
            {
                SetValue(TreeStyleInfoStore.CellTemplateProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="TreeStyleInfo.CellTemplate"/>.
        /// </summary>
        public void ResetCellTemplate()
        {
            ResetValue(TreeStyleInfoStore.CellTemplateProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCellTemplate()
        {
            return HasValue(TreeStyleInfoStore.CellTemplateProperty);
        }

        /// <summary>
        /// Determines if <see cref="TreeStyleInfo.CellTemplate"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCellTemplate
        {
            get
            {
                return HasValue(TreeStyleInfoStore.CellTemplateProperty);
            }
        }
        #endregion
        #region CellTemplateKey
        /// <summary>
        /// Gets or sets the cell template key.
        /// </summary>
        /// <value>The cell template key.</value>
        [
        Description("The DataTemplate used to display an item."),
        Browsable(true),
        Category(""),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public string CellTemplateKey
        {
            get
            {
                return (string)GetValue(TreeStyleInfoStore.CellTemplateKeyProperty);
            }
            set
            {
                SetValue(TreeStyleInfoStore.CellTemplateKeyProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="TreeStyleInfo.CellTemplateKey"/>.
        /// </summary>
        public void ResetCellTemplateKey()
        {
            ResetValue(TreeStyleInfoStore.CellTemplateKeyProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCellTemplateKey()
        {
            return HasValue(TreeStyleInfoStore.CellTemplateKeyProperty);
        }

        /// <summary>
        /// Determines if <see cref="TreeStyleInfo.CellTemplateKey"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCellTemplateKey
        {
            get
            {
                return HasValue(TreeStyleInfoStore.CellTemplateKeyProperty);
            }
        }
        #endregion
        #region CellValue
        /// <summary>
        /// Gets or sets the cell value.
        /// </summary>
        /// <value>The cell value.</value>
        [
        Description("Contains cell value information of a cell."),
        Browsable(false),
        Category(""),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public object CellValue
        {
            get
            {
                return GetValue(TreeStyleInfoStore.CellValueProperty);
            }
            set
            {
                SetValue(TreeStyleInfoStore.CellValueProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="TreeStyleInfo.CellValue"/>.
        /// </summary>
        public void ResetCellValue()
        {
            ResetValue(TreeStyleInfoStore.CellValueProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCellValue()
        {
            return HasValue(TreeStyleInfoStore.CellValueProperty);
        }

        /// <summary>
        /// Determines if <see cref="TreeStyleInfo.CellValue"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCellValue
        {
            get
            {
                return HasValue(TreeStyleInfoStore.CellValueProperty);
            }
        }
        #endregion
        #region CellValueType
        /// <summary>
        /// Gets or sets the type of the cell value.
        /// </summary>
        /// <value>The type of the cell value.</value>
        [
        Description("Contains cell value type information of a cell."),
        Browsable(true),
        Category(""),
        ]
        public Type CellValueType
        {
            get
            {
                return (Type)GetValue(TreeStyleInfoStore.CellValueTypeProperty);
            }
            set
            {
                SetValue(TreeStyleInfoStore.CellValueTypeProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="TreeStyleInfo.CellValueType"/>.
        /// </summary>
        public void ResetCellValueType()
        {
            ResetValue(TreeStyleInfoStore.CellValueTypeProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCellValueType()
        {
            return HasValue(TreeStyleInfoStore.CellValueTypeProperty);
        }

        /// <summary>
        /// Determines if <see cref="TreeStyleInfo.CellValueType"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCellValueType
        {
            get
            {
                return HasValue(TreeStyleInfoStore.CellValueTypeProperty);
            }
        }
        #endregion
        #region CultureInfo
        /// <summary>
        /// Gets or sets the culture info.
        /// </summary>
        /// <value>The culture info.</value>
        [
        Description("The culture information holds rules for parsing and formatting the cell's value."),
        Browsable(true),
        TypeConverter(typeof(CultureInfoConverter)),
        Category(""),
        ImmutableObject(true)
        ]
        [CloneableProperty(false), DisposeableProperty(false)]
        public CultureInfo CultureInfo
        {
            get
            {
                return (CultureInfo)GetValue(TreeStyleInfoStore.CultureInfoProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(TreeStyleInfoStore.CultureInfoProperty, value);
            }
        }
        /// <summary>
        /// Gets the culture information from the style object or returns CultureInfo.CurrentCulture
        /// if <see cref="TreeStyleInfo.CultureInfo"/> is NULL.
        /// </summary>
        /// <param name="useCurrentCultureIfNull">True if CultureInfo.CurrentUICulture should be returned
        /// when <see cref="TreeStyleInfo.CultureInfo"/> is NULL.</param>
        /// <returns>The culture information with rules for parsing and formatting the cell's value.</returns>
        public CultureInfo GetCulture(bool useCurrentCultureIfNull)
        {
            CultureInfo culture = CultureInfo;
            if (culture == null)
                culture = CultureInfo.CurrentCulture;

            return culture;
        }

        /// <summary>
        /// Resets <see cref="TreeStyleInfo.CultureInfo"/>.
        /// </summary>
        public void ResetCultureInfo()
        {
            ResetValue(TreeStyleInfoStore.CultureInfoProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCultureInfo()
        {
            return HasValue(TreeStyleInfoStore.CultureInfoProperty);
        }

        /// <summary>
        /// Determines if <see cref="TreeStyleInfo.CultureInfo"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCultureInfo
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(TreeStyleInfoStore.CultureInfoProperty);
            }
        }
        #endregion
        #region Format
        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>The format.</value>
        [
        Description("Contains format information of a cell."),
        Browsable(true),
        Category(""),
        ]
        [NotifyParentProperty(true)]
        public string Format
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(TreeStyleInfoStore.FormatProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(TreeStyleInfoStore.FormatProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="TreeStyleInfo.Format"/>.
        /// </summary>
        public void ResetFormat()
        {
            ResetValue(TreeStyleInfoStore.FormatProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeFormat()
        {
            return HasValue(TreeStyleInfoStore.FormatProperty);
        }

        /// <summary>
        /// Determines if <see cref="TreeStyleInfo.Format"/> has been initialized for the current object.
        /// </summary>
        public bool HasFormat
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(TreeStyleInfoStore.FormatProperty);
            }
        }
        #endregion

        // Text Conversion
        #region FormattedText
        /// <summary>
        /// Gets or sets the formatted text.
        /// </summary>
        /// <value>The formatted text.</value>
        [
        Browsable(true),
        Category(""),
        Description("Gets the formatted text value")
        ]
        public string FormattedText
        {
            get
            {
                object value = this.CellValue;
                try
                {
                    return GetFormattedText(value);
                }
                catch
                {
                    return value != null ? value.ToString() : "";
                }
            }
            set
            {
                if (!ApplyFormattedText(value))
                    throw new ArgumentException();
            }
        }

        /// <overload>
        /// Return formatted text for the specified value.
        /// TreeStyleInfo.CultureInfo is used for conversion to string.
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
                CultureInfo ci = CultureInfo;
                NumberFormatInfo nfi = ci != null ? ci.NumberFormat : null;
                return ValueConvert.FormatValue(value, CellValueType, Format, ci, nfi, null);
            }
            catch
            {
                return value != null ? value.ToString() : "";
            }
        }

        /// <overload>
        /// Parses the formatted text using Format and cell value type information stored in the current style object.
        /// The text is parsed using TreeStyleInfo.CultureInfo information.
        /// </overload>
        /// <summary>
        /// Parses the formatted text using Format and cell value type information stored in the current style object.
        /// The text is parsed using TreeStyleInfo.CultureInfo information.
        /// </summary>
        /// <param name="text">The formatted text.</param>
        /// <returns>True if the text could be parsed correctly and converted to a cell value.</returns>
        public bool ApplyFormattedText(string text)
        {
            CultureInfo ci = CultureInfo;
            NumberFormatInfo nfi = ci != null ? ci.NumberFormat : null;
            try
            {
                CellValue = ValueConvert.Parse(text, CellValueType, nfi, Format);
            }
            catch
            {
                CellValue = text;
            }
            return true;
        }
        #endregion
        #region Text

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [
        Browsable(true),
        Category(""),
        Description("Gets the formatted text value")
        ]
        public string Text
        {
            get
            {
                object value = this.CellValue;
                try
                {
                    return GetText(value);
                }
                catch
                {
                    return value != null ? value.ToString() : "";
                }
            }
            set
            {
                if (!ApplyText(value))
                    throw new ArgumentException();
            }
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
                if (value == null)
                    return "";
                return (string)ValueConvert.ChangeType(value, typeof(string), CultureInfo.CurrentCulture);
            }
            catch
            {
                return value != null ? value.ToString() : "";
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
            CultureInfo ci = CultureInfo.CurrentCulture;
            NumberFormatInfo nfi = ci != null ? ci.NumberFormat : null;
            try
            {
                CellValue = ValueConvert.Parse(text, CellValueType, nfi, "");
            }
            catch
            {
                CellValue = text;
            }
            return true;
        }
        #endregion

        #region IRenderCellInfo Implementation

        object IRenderCellInfo.GetCellBackground()
        {
            return Background;
        }

        bool IRenderCellInfo.CanCombineCellBackground(IRenderCellInfo other)
        {
            TreeStyleInfo otherStyle = other as TreeStyleInfo;
            if (otherStyle == null)
                return false;

            return Background.Equals(other.GetCellBackground());
        }

        object IRenderCellInfo.GetCellBorder(CellBorderSide side)
        {
            return Borders[side];
        }

        bool IRenderCellInfo.CanCombineCellBorder(CellBorderSide side, IRenderCellInfo other)
        {
            TreeStyleInfo otherStyle = other as TreeStyleInfo;
            if (otherStyle == null)
                return false;

            return Borders[side] == other.GetCellBorder(side);
        }

        System.Windows.Thickness IRenderCellInfo.GetBorderMargins()
        {
            return BorderMargins.ToThickness();
        }

        System.Windows.Thickness IRenderCellInfo.GetPadding()
        {
            return EmptyThickness;
        }

        static readonly Thickness EmptyThickness = new Thickness(0);

        //ICellRenderer IRenderCellInfo.GetCellRenderer()
        //{
        //    return CellRenderer;
        //}

        #endregion

    }
}
