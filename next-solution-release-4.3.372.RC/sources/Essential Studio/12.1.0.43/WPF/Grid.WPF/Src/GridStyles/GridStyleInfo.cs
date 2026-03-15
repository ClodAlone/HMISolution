#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.Styles;
using Syncfusion.Windows.Shared;
using System.Diagnostics;
using System.Windows.Interop;
using System.Collections.Generic;
using Syncfusion.Windows.Controls.Scroll;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// GridStyleInfo holds all information stored for a cell.
    /// </summary>
    /// <remarks>
    /// GridStyleInfo provides user-friendly access to all properties stored in GridStyleInfoStore.
    /// A cell's behavior and appearance can be customized with these properties. It also has 
    /// Identity information and can inherit properties from base styles (row styles, column styles,
    /// table style).
    /// </remarks>
    public class GridStyleInfo : StyleInfoBase, IRenderCellInfo , IDisposable
    {
        #region Fields
        // Static Fields.
        private static GridStyleInfo defaultStyle = null;

        //internal bool Locked = false;

        /// <summary>
        /// An empty style object.
        /// </summary>
        public static readonly GridStyleInfo Empty = new GridStyleInfo();
        #endregion
        #region Ctor
        // Constructors.
        static GridStyleInfo()
        {
        }

        /// <summary>
        /// Initalizes a new style object.
        /// </summary>
        public GridStyleInfo()
            : base(new GridStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new style object and copies all data from an existing style object.
        /// </summary>
        /// <param name="style">The style object that contains the original data.</param>
        public GridStyleInfo(GridStyleInfo style)
            : base(style.Store)
        {
        }

        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="GridStyleInfoStore"/>.
        /// </summary>
        /// <param name="store">A <see cref="GridStyleInfoStore"/> that holds data for this <see cref="GridStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridStyleInfoStore"/> object.</param>
        public GridStyleInfo(GridStyleInfoStore store)
            : base(store)
        {
        }

        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="GridStyleInfoIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoIdentity"/> that holds the indentity for this <see cref="GridStyleInfo"/>.
        /// </param>
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
        public GridStyleInfo(StyleInfoIdentityBase identity, GridStyleInfoStore store)
            : base(identity, store)
        {
        }
        #endregion
        #region Identity
        /// <summary>
        /// Holds identity information such as row and column index for the current <see cref="GridStyleInfo"/>.
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
        /// Returns the grid model.
        /// </summary>
        public GridModel GridModel
        {
            get
            {
                GridStyleInfoIdentity cellIdentity = CellIdentity;
                if (cellIdentity != null)
                    return CellIdentity.GridModel;
                return null;
            }
        }

        /// <summary>
        /// Returns the cell row column index.
        /// </summary>
        public RowColumnIndex CellRowColumnIndex
        {
            get
            {
                GridStyleInfoIdentity cellIdentity = CellIdentity;
                if (cellIdentity != null)
                    return CellIdentity.CellRowColumnIndex;
                return RowColumnIndex.Empty;
            }
        }

        /// <summary>
        /// Gets the row index.
        /// </summary>
        public int RowIndex
        {
            get
            {
                return CellRowColumnIndex.RowIndex;
            }
        }

        /// <summary>
        /// Gets the column index.
        /// </summary>
        public int ColumnIndex
        {
            get
            {
                return CellRowColumnIndex.ColumnIndex;
            }
        }

        /// <summary>
        /// The <see cref="GridStyleInfoStore"/> object that holds all the data for this style object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new GridStyleInfoStore Store
        {
            get { return (GridStyleInfoStore)base.Store; }
        }

        ///// <override/>
        //public override void ModifyStyle(IStyleInfo istyle, StyleModifyType mt)
        //{
        //    base.ModifyStyle(istyle, mt);

        //    GridStyleInfoIdentity cellId = base.Identity as GridStyleInfoIdentity;
        //    {
        //        GridStyleInfo style = istyle as GridStyleInfo;
        //        if (style == null)
        //        {
        //            GridStyleInfoStore store = istyle as GridStyleInfoStore;
        //            if (store != null)
        //                style = new GridStyleInfo(store);
        //        }

        //        OnStyleChanged(null);
        //    }
        //}

        /// <override/>
        /// <summary>
        /// Creates a new <see cref="StyleInfoSubObjectIdentity"/> object and associate it with
        /// this <see cref="GridStyleInfo"/> object.
        /// </summary>
        /// <param name="sip">The StyleInfoProperty descriptor for this subobject.</param>
        /// <returns>The <see cref="StyleInfoSubObjectIdentity"/> object that this method creates.</returns>
        public override StyleInfoSubObjectIdentity CreateSubObjectIdentity(StyleInfoProperty sip)
        {
            return new CachedStyleInfoSubObjectIdentity(this, sip);
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
        public GridStyleInfo GetOffLineCopy()
        {
            return new GridStyleInfo(((GridStyleInfoIdentity)Identity).MakeOfflineIdentity(), (GridStyleInfoStore)Store.Clone());
        }
        #endregion
        #region Default
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
                    defaultStyle.Background = null;
                    defaultStyle.BaseStyle = "";
                    defaultStyle.BorderMargins = CellMarginsInfo.Empty;
                    defaultStyle.Padding = CellMarginsInfo.Empty;
                    defaultStyle.TextMargins = CellMarginsInfo.Empty;
                    defaultStyle.Borders = CellBordersInfo.Default;
                    defaultStyle.Font = GridFontInfo.Default;
                    defaultStyle.CellItemTemplate = null;
                    defaultStyle.CellItemTemplateKey = "";
                    defaultStyle.CellEditTemplate = null;
                    defaultStyle.CellEditTemplateKey = "";
                    defaultStyle.CellType = "";
                    defaultStyle.CellValue = "";
                    defaultStyle.CellValueType = null;
                    defaultStyle.CultureInfo = null;
                    defaultStyle.Description = "";
                    defaultStyle.Enabled = true;
                    defaultStyle.Error = "";
                    defaultStyle.Exception = null;
                    defaultStyle.Format = "";
                    defaultStyle.ParseFormats = null;
                    defaultStyle.PropertyDescriptor = null;
                    defaultStyle.ReadOnly = false;
                    defaultStyle.StrictValueType = false;
                    defaultStyle.ItemsSource = null;
                    defaultStyle.DisplayMember = "";
                    defaultStyle.ValueMember = "";
                    defaultStyle.AutoPopulateDropDownColumns = true;
                    defaultStyle.DropDownColumnSizer = GridControlLengthUnitType.Auto;
                    defaultStyle.DropDownVisibleColumns = null;
                    defaultStyle.ChoiceList = null;
                    defaultStyle.IsEditable = false;
                    defaultStyle.IsThreeState = true;
                    defaultStyle.Tag = null;
                    defaultStyle.Foreground = SystemColors.ControlTextBrush;
                    defaultStyle.NegativeForeground = SystemColors.WindowTextBrush;// Brushes.Red;
                    defaultStyle.AcceptsReturn = false;
                    defaultStyle.EnableFloatingCell = false;
                    defaultStyle.FloatCellMode = GridFloatCellsMode.None;
                    defaultStyle.AutoWordSelection = true;
                    defaultStyle.CharacterCasing = CharacterCasing.Normal;
                    defaultStyle.ToolTip = null;
                    //defaultStyle.TextAlignment = TextAlignment.Left;
                    defaultStyle.HorizontalAlignment = HorizontalAlignment.Left;
                    defaultStyle.TextWrapping = TextWrapping.Wrap;
                    defaultStyle.TextTrimming = TextTrimming.CharacterEllipsis;
                    defaultStyle.FlowDirection = FlowDirection.LeftToRight;
                    defaultStyle.MaxLength = 0;
                    defaultStyle.VerticalAlignment = VerticalAlignment.Top;
                    defaultStyle.IsThemed = true;
                    defaultStyle.ShowTooltip = false;
                    defaultStyle.ShowDataValidationTooltip = false;
                    defaultStyle.DataValidationTooltipLocation = new Point();
                    defaultStyle.DataValidationTooltip = string.Empty;
                    defaultStyle.ImageContentAlignment = ImageContentAlignment.Left;
                    defaultStyle.ImageWidth = new GridLength(0.2d, GridUnitType.Star);
                    defaultStyle.ImageHeight = new GridLength(1d, GridUnitType.Star);
                    defaultStyle.ImageMargins = new CellMarginsInfo(0, 0, 0, 0);
                    defaultStyle.ImageContentStretch = ImageContentStretch.Fill;
                    defaultStyle.IncrementalFilter = IncrementalFilter.Disable;
                    defaultStyle.CommentAlignment = CommentAlignment.TopRight;
                    defaultStyle.IsMouseTrackingEnabled = true;

#if SyncfusionFramework4_0
                    defaultStyle.SelectionBrush = GridUtil.GetXamlConvertedValue<Brush>("#FF3399FF");
                    defaultStyle.SelectionOpacity = 0.4;
                    defaultStyle.CaretBrush = Brushes.Black;
#endif
                }

                return GridStyleInfo.defaultStyle;
            }
        }

        /// <override/>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }
        #endregion

        bool IsRowOrColumnStyle(GridStyleInfo info)
        {
            if (info.CellIdentity != null
                && (info.CellIdentity.ColumnIndex == -1 && info.CellIdentity.RowIndex >= 0
                || info.CellIdentity.ColumnIndex >= 0 && info.CellIdentity.RowIndex == -1)
                )
                return true;

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

        // StyleInfoStore properties
        #region Background
        /// <summary>
        /// Gets or sets a background brush for the cell.
        /// </summary>
        [
        Description(""),
        Browsable(true),
        Category("Appearance"),
        ]
        public Brush Background
        {

            get
            {
                return (Brush)GetValue(GridStyleInfoStore.BackgroundProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.BackgroundProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.Background"/>.
        /// </summary>

        public void ResetBackground()
        {
            ResetValue(GridStyleInfoStore.BackgroundProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBackground()
        {
            return HasValue(GridStyleInfoStore.BackgroundProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.Background"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBackground
        {

            get
            {
                return HasValue(GridStyleInfoStore.BackgroundProperty);
            }
        }
        #endregion

        #region Foreground
        /// <summary>
        /// Gets or sets a foreground brush for the cell.
        /// </summary>
        [
        Description("The brush that paints the foreground of the control."),
        Browsable(true),
        Category("Appearance")
        ]
        public Brush Foreground
        {
            get
            {
                return (Brush)GetValue(GridStyleInfoStore.ForegroundProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.ForegroundProperty, value);
            }
        }
        /// <summary>
        /// Resets text color information.
        /// </summary>
        public void ResetForeground()
        {
            ResetValue(GridStyleInfoStore.ForegroundProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeForeground()
        {
            return HasValue(GridStyleInfoStore.ForegroundProperty);
        }

        /// <summary>
        /// Determines if text color has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasForeground
        {
            get
            {
                return HasValue(GridStyleInfoStore.ForegroundProperty);
            }
        }
        #endregion

        // Behavior
        #region AcceptsReturn
        /// <summary>
        /// Gets or sets a value indicating whether pressing the &lt;Enter&gt;-Key should insert a new line into the edited text.
        /// </summary>
        [
        Description(""),
        Browsable(true)
        ]
        public bool AcceptsReturn
        {
            get
            {
                return GetShortValue(GridStyleInfoStore.AcceptsReturnProperty) != 0;
            }
            set
            {
                SetValue(GridStyleInfoStore.AcceptsReturnProperty, value ? 1 : 0);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.AcceptsReturn"/>.
        /// </summary>
        public void ResetAcceptsReturn()
        {
            ResetValue(GridStyleInfoStore.AcceptsReturnProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeAcceptsReturn()
        {
            return HasValue(GridStyleInfoStore.AcceptsReturnProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.AcceptsReturn"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasAcceptsReturn
        {
            get
            {
                return HasValue(GridStyleInfoStore.AcceptsReturnProperty);
            }
        }
        #endregion

        #region FloatingCells
        /// <summary>
        /// Gets or sets a value indicating whether pressing the &lt;Enter&gt;-Key should insert a new line into the edited text.
        /// </summary>
        [
        Description(""),
        Browsable(true)
        ]
        public bool EnableFloatingCell
        {
            get
            {
                return GetShortValue(GridStyleInfoStore.EnableFloatingCellProperty) != 0;
            }
            set
            {
                SetValue(GridStyleInfoStore.EnableFloatingCellProperty, value ? 1 : 0);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.AcceptsReturn"/>.
        /// </summary>
        public void ResetEnableFloatingCell()
        {
            ResetValue(GridStyleInfoStore.EnableFloatingCellProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeEnableFloatingCell()
        {
            return HasValue(GridStyleInfoStore.EnableFloatingCellProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.AcceptsReturn"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasEnableFloatingCell
        {
            get
            {
                return HasValue(GridStyleInfoStore.EnableFloatingCellProperty);
            }
        }
        #endregion

        #region Float Cell Mode
        /// <summary>
        /// Gets or sets a value indicating whether cell has to be floated on static mode.
        /// </summary>
        [
        Description(""),
        Browsable(true)
        ]
        public GridFloatCellsMode FloatCellMode
        {
            get
            {
                return (GridFloatCellsMode)GetShortValue(GridStyleInfoStore.FloatCellModeProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.FloatCellModeProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.FloatCellMode"/>.
        /// </summary>
        public void ResetFloatCellMode()
        {
            ResetValue(GridStyleInfoStore.FloatCellModeProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeFloatCellMode()
        {
            return HasValue(GridStyleInfoStore.FloatCellModeProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.FloatCellMode"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasFloatCellMode
        {
            get
            {
                return HasValue(GridStyleInfoStore.FloatCellModeProperty);
            }
        }
        #endregion

        #region Flood Cell
        /// <summary>
        /// Gets or sets a value indicating whether can be flood from the previous column cell that is floated.
        /// </summary>
        [
        Description(""),
        Browsable(true)
        ]
        public bool FloodCell
        {
            get
            {
                return GetShortValue(GridStyleInfoStore.FloodCellProperty) != 0;
            }
            set
            {
                SetValue(GridStyleInfoStore.FloodCellProperty, value ? 1 : 0);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.FloodCell"/>.
        /// </summary>
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
        /// Determines if <see cref="GridStyleInfo.FloodCell"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasFloodCell
        {
            get
            {
                return HasValue(GridStyleInfoStore.FloodCellProperty);
            }
        }
        #endregion
        #region AutoWordSelection
        /// <summary>
        /// Gets or sets a value that determines whether when a user selects part of a word
        /// by dragging across it with the mouse, the rest of the word is selected.
        /// </summary>
        [
        Description(""),
        Browsable(true)
        ]
        public bool AutoWordSelection
        {
            get
            {
                return GetShortValue(GridStyleInfoStore.AutoWordSelectionProperty) != 0;
            }
            set
            {
                SetValue(GridStyleInfoStore.AutoWordSelectionProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.AutoWordSelection"/>.
        /// </summary>
        public void ResetAutoWordSelection()
        {
            ResetValue(GridStyleInfoStore.AutoWordSelectionProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeAutoWordSelection()
        {
            return HasValue(GridStyleInfoStore.AutoWordSelectionProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.AutoWordSelection"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasAutoWordSelection
        {
            get
            {
                return HasValue(GridStyleInfoStore.AutoWordSelectionProperty);
            }
        }
        #endregion
        #region CharacterCasing
        /// <summary>
        /// Gets or sets if cell control modifies the case of characters as they are typed.
        /// </summary>
        [
        Description(""),
        Browsable(true)
        ]
        public CharacterCasing CharacterCasing
        {
            get
            {
                return (CharacterCasing)GetValue(GridStyleInfoStore.CharacterCasingProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.CharacterCasingProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.CharacterCasing"/>.
        /// </summary>
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
        /// Determines if <see cref="GridStyleInfo.CharacterCasing"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCharacterCasing
        {
            get
            {
                return HasValue(GridStyleInfoStore.CharacterCasingProperty);
            }
        }
        #endregion

        #region TooltipTemplateKey
        /// <summary>
        /// Gets or sets the template for cell tooltip.
        /// </summary>
        public string TooltipTemplateKey
        {
            get
            {
                return (string)this.GetValue(GridStyleInfoStore.TooltipTemplateKeyProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.TooltipTemplateKeyProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.TooltipTemplateKey"/>.
        /// </summary>
        public void ResetTooltipTemplateKey()
        {
            this.ResetValue(GridStyleInfoStore.TooltipTemplateKeyProperty);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeTooltipTemplateKey()
        {
            return this.HasValue(GridStyleInfoStore.TooltipTemplateKeyProperty);
        }


        public DataTemplate TooltipTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(GridStyleInfoStore.TooltipTemplateProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.TooltipTemplateProperty, value);
            }
        }



        /// <summary>
        /// Resets <see cref="GridStyleInfo.TooltipTemplateKey"/>.
        /// </summary>
        public void ResetTooltipTemplate()
        {
            this.ResetValue(GridStyleInfoStore.TooltipTemplateProperty);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeTooltipTemplate()
        {
            return this.HasValue(GridStyleInfoStore.TooltipTemplateProperty);
        }
        /// <summary>
        /// Gets or sets the template(key) for the cell comment tip display.
        /// </summary>
        public string CommentTemplateKey
        {
            get
            {
                return (string)this.GetValue(GridStyleInfoStore.CommentTemplateKeyProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.CommentTemplateKeyProperty, value);
            }
        }

        /// <summary>
        /// Determines the Color of Comment Service Triangle
        /// </summary>

        public Brush CommentBrush
        {
            get
            {
                if (this.GetValue(GridStyleInfoStore.CommentBrushProperty) != null)
                {
                    return (Brush)this.GetValue(GridStyleInfoStore.CommentBrushProperty);
                }
                else
                {
                    return Brushes.Red;
                }
            }
            set
            {
                this.SetValue(GridStyleInfoStore.CommentBrushProperty, value);
            }
        }
        /// <summary>
        /// Determines where the cell comment tip has to be displayed.
        /// </summary>
        public CommentAlignment CommentAlignment
        {
            get
            {
                if (this.GetValue(GridStyleInfoStore.CommentAlignmentProperty) != null)
                {
                    return (CommentAlignment)this.GetValue(GridStyleInfoStore.CommentAlignmentProperty);
                }

                return defaultStyle.CommentAlignment;
            }

            set
            {
                this.SetValue(GridStyleInfoStore.CommentAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text that should appear in the cell comment tip.
        /// </summary>
        public string Comment
        {
            get
            {
                return (string)this.GetValue(GridStyleInfoStore.CommentProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.CommentProperty, value);
            }
        }

        public GridCommentStyleInfo GridCommentStyleInfo
        {
            get { return (GridCommentStyleInfo)this.GetValue(GridStyleInfoStore.GridCommentStyleInfoProperty); }
            set { this.SetValue(GridStyleInfoStore.GridCommentStyleInfoProperty, value); }
        }

        public bool HasGridCommentStyleInfo
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.GridCommentStyleInfoProperty);
            }
        }

#if !SILVERLIGHT

        public IEnumerable ContextMenuItems
        {
            get { return (IEnumerable)this.GetValue(GridStyleInfoStore.ContextMenuItemsProperty); }
            set { this.SetValue(GridStyleInfoStore.ContextMenuItemsProperty, value); }
        }

        public IEnumerable ContextMenuItemSource
        {
            get { return (IEnumerable)this.GetValue(GridStyleInfoStore.ContextMenuItemSourceProperty); }
            set { this.SetValue(GridStyleInfoStore.ContextMenuItemSourceProperty, value); }
        }

        public DataTemplate ContextMenuTemplate
        {
            get { return (DataTemplate)this.GetValue(GridStyleInfoStore.ContextMenuTemplateProperty); }
            set { this.SetValue(GridStyleInfoStore.ContextMenuTemplateProperty, value); }
        }
#endif
        /// <summary>
        /// Determines if <see cref="GridStyleInfo.TooltipTemplateKey"/> has been initialized for the current object.
        /// </summary>
        public bool HasTooltipTemplateKey
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.TooltipTemplateKeyProperty);
            }
        }

        #endregion

        #region ShowTooltip
        /// <summary>
        /// Gets or sets whether to show cell tooltip.
        /// </summary>
        public bool ShowTooltip
        {
            get
            {
                return (bool)this.GetValue(GridStyleInfoStore.ShowTooltipProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.ShowTooltipProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.ShowTooltip"/>.
        /// </summary>
        public void ResetShowTooltip()
        {
            this.ResetValue(GridStyleInfoStore.ShowTooltipProperty);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowTooltip()
        {
            return this.HasValue(GridStyleInfoStore.ShowTooltipProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.ShowTooltip"/> has been initialized for the current object.
        /// </summary>
        public bool HasShowTooltip
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.ShowTooltipProperty);
            }
        }

        #endregion

        #region ToolTip
        /// <summary>
        /// Gets or sets the tooltip for the cell.
        /// </summary>
        [
        Description(""),
        Browsable(true)
        ]
        public object ToolTip
        {
            get
            {
                return GetValue(GridStyleInfoStore.ToolTipProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.ToolTipProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.ToolTip"/>.
        /// </summary>
        public void ResetToolTip()
        {
            ResetValue(GridStyleInfoStore.ToolTipProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeToolTip()
        {
            return HasValue(GridStyleInfoStore.ToolTipProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.ToolTip"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasToolTip
        {
            get
            {
                return HasValue(GridStyleInfoStore.ToolTipProperty);
            }
        }
        #endregion

        #region BackGroundAnimation

        ///<summary>
        ///The From color for the background animation
        ///</summary>
        private Color backgroundAnimationFromColor;// = Colors.Yellow;

        /// <summary>
        /// Gets or Sets the BackgroundAnimationFromColor property
        /// </summary>
        public Color BackgroundAnimationFromColor
        {
            get
            {
                return this.backgroundAnimationFromColor;
            }
            set
            {
                this.backgroundAnimationFromColor = value;
            }
        }
      
        ///<summary>
        ///The To color for the background animation
        ///</summary>
        private Color backgroundAnimationToColor;// = Colors.White;

        /// <summary>
        /// Gets or Sets the BackgroundAnimationFromColor property
        /// </summary>
        public Color BackgroundAnimationToColor
        {
            get
            {
                return this.backgroundAnimationToColor;
            }
            set
            {
                this.backgroundAnimationToColor = value;
            }
        }

        ///<summary>
        ///Initialize the isBackGroundAnimationEnabled as false
        ///</summary>
        private bool isBackGroundAnimationEnabled = false;

        /// <summary>
        /// Gets or Sets a value indicating whether BackGroundAnimatioinEnabled is Enabled or not
        /// </summary>
        public bool IsBackGroundAnimationEnabled
        {
            get
            {
                return this.isBackGroundAnimationEnabled;
            }
            set
            {
                this.isBackGroundAnimationEnabled = value;
            }
        }

        /// <summary>
        /// Autoreverse property for the background animation
        /// </summary>
        private bool isAutoReverseEnabled = false;

        /// <summary>
        /// Gets or Sets the AutoReverse property
        /// </summary>
        public bool IsAutoReverseEnabled
        {
            get
            {
                return this.isAutoReverseEnabled;
            }
            set
            {
                this.isAutoReverseEnabled = value;
            }
        }

        /// <summary>
        /// Setting the default value for the animation duration
        /// </summary>
        private Duration animationDuration;// = new Duration(TimeSpan.FromMilliseconds(500));

        /// <summary>
        /// Gets or Sets the AnimationDuration property
        /// </summary>
        public Duration AnimationDuration
        {
            get
            {
                return this.animationDuration;
            }
            set
            {
                this.animationDuration = value;
            }
        }

        #endregion

        // Alignment
#if false
        #region TextAlignment
        [
        Description(""),
        Browsable(true)
        ]
        public TextAlignment TextAlignment
        {
            get
            {
                return (TextAlignment) GetValue(GridStyleInfoStore.TextAlignmentProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.TextAlignmentProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.TextAlignment"/>.
        /// </summary>
        public void ResetTextAlignment()
        {
            ResetValue(GridStyleInfoStore.TextAlignmentProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeTextAlignment()
        {
            return HasValue(GridStyleInfoStore.TextAlignmentProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.TextAlignment"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTextAlignment
        {
            get
            {
                return HasValue(GridStyleInfoStore.TextAlignmentProperty);
            }
        }
        #endregion
#endif
        #region HorizontalAlignment
        /// <summary>
        /// Gets or sets horizontal alignment of text in the cell.
        /// </summary>
        [
        Description(""),
        Browsable(true)
        ]
        public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                return (HorizontalAlignment)GetValue(GridStyleInfoStore.HorizontalAlignmentProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.HorizontalAlignmentProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.HorizontalAlignment"/>.
        /// </summary>
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
        /// Determines if <see cref="GridStyleInfo.HorizontalAlignment"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasHorizontalAlignment
        {
            get
            {
                return HasValue(GridStyleInfoStore.HorizontalAlignmentProperty);
            }
        }
        #endregion
        #region TextWrapping
        /// <summary>
        /// Gets or sets a value indicating whether text should be wrapped when it does not fit into a single line.
        /// </summary>
        [
        Description(""),
        Browsable(true)
        ]
        public TextWrapping TextWrapping
        {
            get
            {
                return (TextWrapping)GetValue(GridStyleInfoStore.TextWrappingProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.TextWrappingProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.TextWrapping"/>.
        /// </summary>
        public void ResetTextWrapping()
        {
            ResetValue(GridStyleInfoStore.TextWrappingProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeTextWrapping()
        {
            return HasValue(GridStyleInfoStore.TextWrappingProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.TextWrapping"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTextWrapping
        {
            get
            {
                return HasValue(GridStyleInfoStore.TextWrappingProperty);
            }
        }
        #endregion
        #region MaxLength
        /// <summary>
        /// Gets or sets the limits the number of characters the user can type into the cell.
        /// </summary>
        [
        Description(""),
        Browsable(true)
        ]
        public int MaxLength
        {
            get
            {
                return (int)GetValue(GridStyleInfoStore.MaxLengthProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.MaxLengthProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.MaxLength"/>.
        /// </summary>
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
        /// Determines if <see cref="GridStyleInfo.MaxLength"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasMaxLength
        {
            get
            {
                return HasValue(GridStyleInfoStore.MaxLengthProperty);
            }
        }
        #endregion
        #region VerticalAlignment
        /// <summary>
        /// Gets or sets vertical alignment of text in the cell.
        /// </summary>
        [
        Description(""),
        Browsable(true)
        ]
        public VerticalAlignment VerticalAlignment
        {
            get
            {
                return (VerticalAlignment)GetValue(GridStyleInfoStore.VerticalAlignmentProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.VerticalAlignmentProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.VerticalAlignment"/>.
        /// </summary>
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
        /// Determines if <see cref="GridStyleInfo.VerticalAlignment"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasVerticalAlignment
        {
            get
            {
                return HasValue(GridStyleInfoStore.VerticalAlignmentProperty);
            }
        }
        #endregion
        #region HeaderColors
        private bool hasHeaderBackGround = false;
        public bool HasHeaderBackGround
        {
            get
            {
                return this.hasHeaderBackGround; 
            }
            set
            {
                this.hasHeaderBackGround = value; 
            }
        }

        private bool hasHeaderForeGround = false;
        public bool HasHeaderForeGround
        {
            get
            {
                return this.hasHeaderForeGround; 
            }
            set
            {
                this.hasHeaderForeGround = value; 
            }
        }
        #endregion
        #region FlowDirection
        /// <summary>
        /// Gets or sets the content flow direction for the cell text and ui elements.
        /// </summary>
        [
        Description(""),
        Browsable(true)
        ]
        public FlowDirection FlowDirection
        {
            get
            {
                return (FlowDirection)GetValue(GridStyleInfoStore.FlowDirectionProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.FlowDirectionProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.FlowDirection"/>.
        /// </summary>
        public void ResetFlowDirection()
        {
            ResetValue(GridStyleInfoStore.FlowDirectionProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeFlowDirection()
        {
            return HasValue(GridStyleInfoStore.FlowDirectionProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.FlowDirection"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasFlowDirection
        {
            get
            {
                return HasValue(GridStyleInfoStore.FlowDirectionProperty);
            }
        }
        #endregion
        #region MaskEditInfo
        /// <summary>
        /// Gets or sets MaskedEdit state. MaskedEdit is itself an expandable object
        /// with several properties that can be set individually and participate
        /// in style inheritance mechanism.
        /// </summary>
        public GridMaskEditInfo MaskEdit
        {
            get
            {
                return (GridMaskEditInfo)GetValue(GridStyleInfoStore.MaskEditInfoProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.MaskEditInfoProperty, value);
            }
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.MaskEdit"/> has been initialized for the current object.
        /// </summary>
        public bool HasMaskEdit
        {
            get { return HasValue(GridStyleInfoStore.MaskEditInfoProperty); }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.MaskEdit"/> property.
        /// </summary>
        public void ResetMaskEditInfo()
        {
            ResetValue(GridStyleInfoStore.MaskEditInfoProperty);
        }

        /// <summary>
        /// Determines whether the <see cref="GridStyleInfo.MaskEdit"/> property is serializable.
        /// </summary>
        /// <returns>True if it is serializable; False otherwise.</returns>
        public bool ShouldSerializeMaskEditInfo()
        {
            return HasValue(GridStyleInfoStore.MaskEditInfoProperty);
        }

        #endregion

        /// <summary>
        /// Gets or sets IntegerEdit state. IntegerEdit is itself an expandable object
        /// with several properties that can be set individually and participate
        /// in style inheritance mechanism.
        /// </summary>
        public GridIntegerEditStyleInfo IntegerEdit
        {
            get
            {
                return (GridIntegerEditStyleInfo)this.GetValue(GridStyleInfoStore.IntegerEditStyleInfoProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.IntegerEditStyleInfoProperty, value);
            }
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.IntegerEdit"/> has been initialized for the current object.
        /// </summary>
        public bool HasIntegerEdit
        {
            get { return HasValue(GridStyleInfoStore.IntegerEditStyleInfoProperty); }
        }

        /// <summary>
        /// Gets or sets DoubleEdit state. DoubleEdit is itself an expandable object
        /// with several properties that can be set individually and participate
        /// in style inheritance mechanism.
        /// </summary>
        public GridDoubleEditStyleInfo DoubleEdit
        {
            get
            {
                return (GridDoubleEditStyleInfo)this.GetValue(GridStyleInfoStore.DoubleEditStyleInfoProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.DoubleEditStyleInfoProperty, value);
            }
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.DoubleEdit"/> has been initialized for the current object.
        /// </summary>
        public bool HasDoubleEdit
        {
            get { return HasValue(GridStyleInfoStore.DoubleEditStyleInfoProperty); }
        }

        #region PercentMode
        /// <summary>
        /// Gets or sets the way of editing in the percent text cells.
        /// </summary>
        [
        Description(""),
        Browsable(true)
        ]
        public PercentEditMode PercentEditMode
        {
            get
            {
                if (this.GetValue(GridStyleInfoStore.PercentEditModeProperty) == null)
                {
                    return PercentEditMode.PercentMode;
                }
                return (PercentEditMode)this.GetValue(GridStyleInfoStore.PercentEditModeProperty);
            }
            set
            {
                this.SetValue(GridStyleInfoStore.PercentEditModeProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.PercentEditMode"/> property.
        /// </summary>
        public void ResetPercentEditMode()
        {
            this.ResetValue(GridStyleInfoStore.PercentEditModeProperty);
        }

        /// <summary>
        /// Determines whether the <see cref="GridStyleInfo.PercentEditMode"/> property is serializable.
        /// </summary>
        /// <returns>True if it is serializable; False otherwise.</returns>
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public bool ShouldSerializePercentEditMode()
        {
            return this.HasValue(GridStyleInfoStore.PercentEditModeProperty);
        }

        #endregion
        #region CurrencyEdit
        /// <summary>
        /// Gets or sets CurrencyEdit state. CurrencyEdit is itself an expandable object
        /// with several properties that can be set individually and participate
        /// in style inheritance mechanism.
        /// </summary>
        public GridCurrencyEditStyleInfo CurrencyEdit
        {
            get
            {
                return (GridCurrencyEditStyleInfo)this.GetValue(GridStyleInfoStore.CurrencyEditStyleInfoProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.CurrencyEditStyleInfoProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.CurrencyEdit"/> property.
        /// </summary>
        public void ResetCurrencyEdit()
        {
            this.ResetValue(GridStyleInfoStore.CurrencyEditStyleInfoProperty);
        }
        /// <summary>
        /// Determines whether the <see cref="GridStyleInfo.CurrencyEdit"/> property is serializable.
        /// </summary>
        /// <returns>True if it is serializable; False otherwise.</returns>
        public bool ShouldSerializeCurrencyEdit()
        {
            return this.HasValue(GridStyleInfoStore.CurrencyEditStyleInfoProperty);
        }
        /// <summary>
        /// Determines if <see cref="GridStyleInfo.CurrencyEdit"/> has been initialized for the current object.
        /// </summary>
        public bool HasCurrencyEdit
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.CurrencyEditStyleInfoProperty);
            }
        }

        #endregion

        #region DropdownEdit
        /// <summary>
        /// Gets or sets DropdownEdit state. DropdownEdit is itself an expandable object
        /// with several properties that can be set individually and participate
        /// in style inheritance mechanism.
        /// </summary>
        public GridDropdownEditStyleInfo DropdownEdit
        {
            get
            {
                return (GridDropdownEditStyleInfo)this.GetValue(GridStyleInfoStore.DropdownEditStyleInfoProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.DropdownEditStyleInfoProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.DropdownEdit"/> property.
        /// </summary>
        public void ResetDropdownEdit()
        {
            this.ResetValue(GridStyleInfoStore.DropdownEditStyleInfoProperty);
        }
        /// <summary>
        /// Determines whether the <see cref="GridStyleInfo.DropdownEdit"/> property is serializable.
        /// </summary>
        /// <returns>True if it is serializable; False otherwise.</returns>
        public bool ShouldSerializeDropdownEdit()
        {
            return this.HasValue(GridStyleInfoStore.DropdownEditStyleInfoProperty);
        }
        /// <summary>
        /// Determines if <see cref="GridStyleInfo.DropdownEdit"/> has been initialized for the current object.
        /// </summary>
        public bool HasDropdownEdit
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.DropdownEditStyleInfoProperty);
            }
        }

        #endregion

        #region PercentEdit
        /// <summary>
        /// Gets or sets CurrencyEdit state. CurrencyEdit is itself an expandable object
        /// with several properties that can be set individually and participate
        /// in style inheritance mechanism.
        /// </summary>
        public GridPercentEditStyleInfo PercentEdit
        {
            get
            {
                return (GridPercentEditStyleInfo)this.GetValue(GridStyleInfoStore.PercentEditStyleInfoProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.PercentEditStyleInfoProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.CurrencyEdit"/> property.
        /// </summary>
        public void ResetPercentEdit()
        {
            this.ResetValue(GridStyleInfoStore.PercentEditStyleInfoProperty);
        }
        /// <summary>
        /// Determines whether the <see cref="GridStyleInfo.CurrencyEdit"/> property is serializable.
        /// </summary>
        /// <returns>True if it is serializable; False otherwise.</returns>
        public bool ShouldSerializePercentEdit()
        {
            return this.HasValue(GridStyleInfoStore.PercentEditStyleInfoProperty);
        }
        /// <summary>
        /// Determines if <see cref="GridStyleInfo.CurrencyEdit"/> has been initialized for the current object.
        /// </summary>
        public bool HasPercentEdit
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.PercentEditStyleInfoProperty);
            }
        }

        #endregion
        #region DateTimeEdit
        /// <summary>
        /// Gets or sets DateTimeEdit state. DateTimeEdit is itself an expandable object
        /// with several properties that can be set individually and participate
        /// in style inheritance mechanism.
        /// </summary>
        [
        Description(""),
        Browsable(true)
        ]
        public GridDateTimeEditStyleInfo DateTimeEdit
        {
            get
            {
                return (GridDateTimeEditStyleInfo)this.GetValue(GridStyleInfoStore.DateTimeEditStyleInfoProperty);
            }
            set
            {
                this.SetValue(GridStyleInfoStore.DateTimeEditStyleInfoProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.DateTimeEdit"/> property.
        /// </summary>
        public void ResetDateTimeEdit()
        {
            this.ResetValue(GridStyleInfoStore.DateTimeEditStyleInfoProperty);
        }
        /// <summary>
        /// Determines whether the <see cref="GridStyleInfo.DateTimeEdit"/> property is serializable.
        /// </summary>
        /// <returns>True if it is serializable; False otherwise.</returns>
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public bool ShouldSerializeDateTimeEdit()
        {
            return this.HasValue(GridStyleInfoStore.DateTimeEditStyleInfoProperty);
        }
        /// <summary>
        /// Determines if <see cref="GridStyleInfo.DateTimeEdit"/> has been initialized for the current object.
        /// </summary>
        public bool HasDateTimeEdit
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.DateTimeEditStyleInfoProperty);
            }
        }
        #endregion
        #region TimeSpanEdit
        /// <summary>
        /// Gets or sets TimeSpanEdit state. TimeSpanEdit is itself an expandable object
        /// with several properties that can be set individually and participate
        /// in style inheritance mechanism.
        /// </summary>
        public GridTimeSpanEditStyleInfo TimeSpanEdit
        {
            get
            {
                return (GridTimeSpanEditStyleInfo)this.GetValue(GridStyleInfoStore.TimeSpanEditStyleInfoProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.TimeSpanEditStyleInfoProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.TimeSpanEdit"/> property.
        /// </summary>
        public void ResetTimeSpanEdit()
        {
            this.ResetValue(GridStyleInfoStore.TimeSpanEditStyleInfoProperty);
        }
        /// <summary>
        /// Determines whether the <see cref="GridStyleInfo.TimeSpanEdit"/> property is serializable.
        /// </summary>
        /// <returns>True if it is serializable; False otherwise.</returns>
        public bool ShouldSerializeTimeSpanEdit()
        {
            return this.HasValue(GridStyleInfoStore.TimeSpanEditStyleInfoProperty);
        }
        /// <summary>
        /// Determines if <see cref="GridStyleInfo.TimeSpanEdit"/> has been initialized for the current object.
        /// </summary>
        public bool HasTimeSpanEdit
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.TimeSpanEditStyleInfoProperty);
            }
        }

        #endregion


        #region NegativeForeground
        /// <summary>
        /// Gets/Sets the foreground when the current value is negative.
        /// </summary>
        public Brush NegativeForeground
        {
            get
            {
                return (Brush)this.GetValue(GridStyleInfoStore.NegativeForegroundProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.NegativeForegroundProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.NegativeForeground"/> property.
        /// </summary>
        public void ResetNegativeForeground()
        {
            this.ResetValue(GridStyleInfoStore.NegativeForegroundProperty);
        }
        /// <summary>
        /// Determines whether the <see cref="GridStyleInfo.NegativeForeground"/> property is serializable.
        /// </summary>
        /// <returns>True if it is serializable; False otherwise.</returns>
        public bool ShouldSerializeNegativeForeground()
        {
            return this.HasValue(GridStyleInfoStore.NegativeForegroundProperty);
        }
        /// <summary>
        /// Determines if <see cref="GridStyleInfo.NegativeForeground"/> has been initialized for the current object.
        /// </summary>
        public bool HasNegativeForeground
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.NegativeForegroundProperty);
            }
        }

        #endregion

        #region NumberFormat
        /// <summary>
        /// Gets or sets the <see cref="NumberFormatInfo"/> for displaying numbers, currency values and percentages.
        /// </summary>
        [
        Description(""),
        Browsable(true)
        ]
        public NumberFormatInfo NumberFormat
        {
            get
            {
                return (NumberFormatInfo)this.GetValue(GridStyleInfoStore.NumberFormatProperty);
            }
            set
            {
                this.SetValue(GridStyleInfoStore.NumberFormatProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.NumberFormat"/> property.
        /// </summary>
        public void ResetNumberFormat()
        {
            this.ResetValue(GridStyleInfoStore.NumberFormatProperty);
        }

        /// <summary>
        /// Determines whether the <see cref="GridStyleInfo.NumberFormat"/> property is serializable.
        /// </summary>
        /// <returns>True if it is serializable; False otherwise.</returns>
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public bool ShouldSerializeNumberFormat()
        {
            return this.HasValue(GridStyleInfoStore.NumberFormatProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.NumberFormat"/> has been initialized for the current object.
        /// </summary>
        public bool HasNumberFormat
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.NumberFormatProperty);
            }
        }
        #endregion

        #region TextTrimming
        /// <summary>
        /// Gets or sets how text is trimmed when it exceeds the edges of the cell text rectangle.
        /// </summary>
        [
        Description(""),
        Browsable(true)
        ]
        public TextTrimming TextTrimming
        {
            get
            {
                return (TextTrimming)GetValue(GridStyleInfoStore.TextTrimmingProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.TextTrimmingProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.TextTrimming"/>.
        /// </summary>
        public void ResetTextTrimming()
        {
            ResetValue(GridStyleInfoStore.TextTrimmingProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeTextTrimming()
        {
            return HasValue(GridStyleInfoStore.TextTrimmingProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.TextTrimming"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTextTrimming
        {
            get
            {
                return HasValue(GridStyleInfoStore.TextTrimmingProperty);
            }
        }
        #endregion

        #region CaretBrush
        /// <summary>
        /// Gets or sets CaretBrush for Caret in TextBox.
        /// </summary>
        [
        Description(""),
        Browsable(true)
        ]
        public Brush CaretBrush
        {
            get
            {
                return (Brush)GetValue(GridStyleInfoStore.CaretBrushProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.CaretBrushProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.CaretBrush"/>.
        /// </summary>
        public void ResetCaretBrush()
        {
            ResetValue(GridStyleInfoStore.CaretBrushProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCaretBrush()
        {
            return HasValue(GridStyleInfoStore.CaretBrushProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.CaretBrush"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCaretBrush
        {
            get
            {
                return HasValue(GridStyleInfoStore.CaretBrushProperty);
            }
        }
        #endregion

        #region SelectionBrush
        /// <summary>
        /// Gets or sets SelectionBrush for Text.
        /// </summary>
        [
        Description("Selection Brush for Editor cell text"),
        Browsable(true)
        ]
        public Brush SelectionBrush
        {
            get
            {
                return (Brush)GetValue(GridStyleInfoStore.SelectionBrushProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.SelectionBrushProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.SelectionBrush"/>.
        /// </summary>
        public void ResetSelectionBrush()
        {
            ResetValue(GridStyleInfoStore.SelectionBrushProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeSelectionBrush()
        {
            return HasValue(GridStyleInfoStore.SelectionBrushProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.SelectionBrush"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasSelectionBrush
        {
            get
            {
                return HasValue(GridStyleInfoStore.SelectionBrushProperty);
            }
        }
        #endregion

        #region SelectionOpacity
        /// <summary>
        /// Gets or sets SelectionOpacity for Text.
        /// </summary>
        [
        Description("SelectionOpacity for Editor cell text"),
        Browsable(true)
        ]
        public double SelectionOpacity
        {
            get
            {
                return (double)GetValue(GridStyleInfoStore.SelectionOpacityProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.SelectionOpacityProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.SelectionOpacity"/>.
        /// </summary>
        public void ResetSelectionOpacity()
        {
            ResetValue(GridStyleInfoStore.SelectionOpacityProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeSelectionOpacity()
        {
            return HasValue(GridStyleInfoStore.SelectionOpacityProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.SelectionOpacity"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasSelectionOpacity
        {
            get
            {
                return HasValue(GridStyleInfoStore.SelectionOpacityProperty);
            }
        }
        #endregion

        #region BaseStyle
        /// <summary>
        /// Gets or sets the base style for this style instance with default values for properties that are not initialized for this style object.
        /// </summary>
        [
        Description("The base style for this style instance with default values for properties that are not initialized for this style object."),
        Browsable(true),
        Category("Style")
        ]
        public string BaseStyle
        {
            get
            {
                return (string)GetValue(GridStyleInfoStore.BaseStyleProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.BaseStyleProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.BaseStyle"/>.
        /// </summary>
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
        /// Determines if <see cref="GridStyleInfo.BaseStyle"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBaseStyle
        {
            get
            {
                return HasValue(GridStyleInfoStore.BaseStyleProperty);
            }
        }
        #endregion
        #region BorderMargins
        /// <summary>
        /// Holds extra border margins in pixels. When drawing a cell, this specifies the area between the
        /// cell rectangle without border and the inner rectangle of the cell with cell buttons. 
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the BorderMargins property is CellMarginsInfo.Empty.<para/>
        /// The property affects the behavior or appearance of call cell types.<para/>
        /// </remarks>
        [
        Description(""),
        Browsable(true),
        Category("Appearance")
        ]
        public CellMarginsInfo BorderMargins
        {

            get
            {
                return (CellMarginsInfo)GetValue(GridStyleInfoStore.BorderMarginsProperty);
            }

            set
            {
                SetValue(GridStyleInfoStore.BorderMarginsProperty, value);
            }
        }

        /// <internalonly/>
        /// <summary>For internal use.</summary>
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
                    GridStyleInfo marginsInfo = this.Identity.GetBaseStyleNotEmptyExpandable(this, GridStyleInfoStore.BorderMarginsProperty) as GridStyleInfo;
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
        /// Resets <see cref="GridStyleInfo.BorderMargins"/>.
        /// </summary>
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
        /// Determines if <see cref="GridStyleInfo.BorderMargins"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBorderMargins
        {

            get
            {
                return HasValue(GridStyleInfoStore.BorderMarginsProperty);
            }
        }
        #endregion
        #region TextMargins
        /// <summary>
        /// Holds extra border margins in pixels. When drawing a cell, this specifies the area between the
        /// cell rectangle without border and the inner rectangle of the cell with cell buttons. 
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the TextMargins property is CellMarginsInfo.Empty.<para/>
        /// The property affects the behavior or appearance of call cell types:<para/>
        /// </remarks>
        [
        Description(""),
        Browsable(true),
        Category("Appearance")
        ]
        public CellMarginsInfo TextMargins
        {

            get
            {
                return (CellMarginsInfo)GetValue(GridStyleInfoStore.TextMarginsProperty);
            }

            set
            {
                SetValue(GridStyleInfoStore.TextMarginsProperty, value);
            }
        }

        /// <internalonly/>
        /// <summary>For internal use.</summary>
        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CellMarginsInfo ReadOnlyTextMargins
        {
            //
            get
            {
                if (HasTextMargins && !TextMargins.IsEmpty)
                    return TextMargins;

                if (Identity != null)
                {
                    GridStyleInfo marginsInfo = this.Identity.GetBaseStyleNotEmptyExpandable(this, GridStyleInfoStore.TextMarginsProperty) as GridStyleInfo;
                    if (marginsInfo != null)
                    {
                        //if (fixSubObjectsDerivedFromRowandColStyle && IsRowOrColumnStyle(marginsInfo))
                        //    return TextMargins;

                        return marginsInfo.TextMargins;
                    }
                }
                return Default.TextMargins;
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.TextMargins"/>.
        /// </summary>
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
        /// Determines if <see cref="GridStyleInfo.TextMargins"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTextMargins
        {

            get
            {
                return HasValue(GridStyleInfoStore.TextMarginsProperty);
            }
        }
        #endregion
        #region Padding
        /// <summary>
        /// Gets or sets cell padding.
        /// </summary>
        [
        Description(""),
        Browsable(true),
        Category("Appearance")
        ]
        public CellMarginsInfo Padding
        {

            get
            {
                return (CellMarginsInfo)GetValue(GridStyleInfoStore.PaddingProperty);
            }

            set
            {
                SetValue(GridStyleInfoStore.PaddingProperty, value);
            }
        }

        /// <internalonly/>
        /// <summary>For internal use.</summary>
        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CellMarginsInfo ReadOnlyPadding
        {
            //
            get
            {
                if (HasPadding && !Padding.IsEmpty)
                    return Padding;

                if (Identity != null)
                {
                    GridStyleInfo marginsInfo = this.Identity.GetBaseStyleNotEmptyExpandable(this, GridStyleInfoStore.PaddingProperty) as GridStyleInfo;
                    if (marginsInfo != null)
                    {
                        //if (fixSubObjectsDerivedFromRowandColStyle && IsRowOrColumnStyle(marginsInfo))
                        //    return Padding;

                        return marginsInfo.Padding;
                    }
                }
                return Default.Padding;
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.Padding"/>.
        /// </summary>
        public void ResetPadding()
        {
            ResetValue(GridStyleInfoStore.PaddingProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializePadding()
        {
            return HasValue(GridStyleInfoStore.PaddingProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.Padding"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasPadding
        {

            get
            {
                return HasValue(GridStyleInfoStore.PaddingProperty);
            }
        }
        #endregion
        #region Borders
        /// <summary>
        /// Gets or sets cell borders.
        /// </summary>
        [
        Description("Top, left, bottom, and right border settings."),
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Category("Appearance"),
        ]
        public CellBordersInfo Borders
        {
            get
            {
                return (CellBordersInfo)GetValue(GridStyleInfoStore.BordersProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.BordersProperty, value);
            }
        }

        /// <internalonly/>
        /// <summary>For internal use.</summary>
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
                    GridStyleInfo bordersInfo = this.Identity.GetBaseStyleNotEmptyExpandable(this, GridStyleInfoStore.BordersProperty) as GridStyleInfo;
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
            ResetValue(GridStyleInfoStore.BordersProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBorders()
        {
            return HasValue(GridStyleInfoStore.BordersProperty);
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
                return HasValue(GridStyleInfoStore.BordersProperty);
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool IsLeftBorderChanged
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool IsRightBorderChanged
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool IsTopBorderChanged
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool IsBottomBorderChanged
        {
            get;
            set;
        }

        #endregion

        #region IsThemed
        /// <summary>
        /// For internal use.
        /// </summary>
        public bool ReadOnlyIsThemed
        {
            get
            {
                if (this.HasUseThemes)
                {
                    return this.IsThemed;
                }

                return Default.IsThemed;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether cell types should be drawn themed.
        /// </summary>
        public bool IsThemed
        {
            get
            {
                return (bool)this.GetValue(GridStyleInfoStore.IsThemedProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.IsThemedProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.IsThemed"/> property.
        /// </summary>
        public void ResetUseThemes()
        {
            this.ResetValue(GridStyleInfoStore.IsThemedProperty);
        }

        private bool ShouldSerializeUseThemes()
        {
            return this.HasValue(GridStyleInfoStore.IsThemedProperty);
        }
        /// <summary>
        /// Determines if <see cref="GridStyleInfo.IsThemed"/> property has been initialized for the current object.
        /// </summary>
        public bool HasUseThemes
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.IsThemedProperty);
            }
        }

        #endregion
        #region Font
        /// <summary>
        /// Creates or returns a cached GDI+ font generated from font information of
        /// the <see cref="GridStyleInfo.Font"/> object.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [ReadOnly(true)]
        public Typeface Typeface
        {
            get
            {
                if (HasFont && !Font.IsEmpty)
                    return Font.Typeface;

                if (Identity != null)
                {
                    GridStyleInfo fontInfo = this.Identity.GetBaseStyleNotEmptyExpandable(this, GridStyleInfoStore.FontProperty) as GridStyleInfo;
                    if (fontInfo != null)
                        return fontInfo.Typeface;
                }
                return Default.Typeface;
            }
        }

        /// <internalonly/>
        /// <summary>For internal use.</summary>
        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridFontInfo ReadOnlyFont
        {
            get
            {
                if (HasFont && !Font.IsEmpty)
                    return Font;

                if (Identity != null)
                {
                    GridStyleInfo fontInfo = this.Identity.GetBaseStyleNotEmptyExpandable(this, GridStyleInfoStore.FontProperty) as GridStyleInfo;
                    if (fontInfo != null)
                    {
                        if (fixSubObjectsDerivedFromRowandColStyle && IsRowOrColumnStyle(fontInfo))
                            return Font;

                        return fontInfo.Font;
                    }
                }
                return Default.Font;
            }
        }

        /// <summary>
        /// Gets or sets the font for drawing text.
        /// </summary>
        [
        Description("The font for drawing text."),
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Category("Appearance"),
        ]
        public GridFontInfo Font
        {
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
        /// Determines if font information has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasFont
        {
            get
            {
                return HasValue(GridStyleInfoStore.FontProperty);
            }
        }
        #endregion


        #region CellItemTemplate
        [
       Description("The Item DataTemplate for the cell."),
       Browsable(true),
       Category("ItemTemplate"),
       DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
       ]
        public DataTemplate CellItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(GridStyleInfoStore.CellItemTemplateProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.CellItemTemplateProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.CellTemplate"/>.
        /// </summary>
        public void ResetCellItemTemplate()
        {
            ResetValue(GridStyleInfoStore.CellItemTemplateProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCellItemTemplate()
        {
            return HasValue(GridStyleInfoStore.CellItemTemplateProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.CellTemplate"/> has been initialized for the current object.
        /// </summary>
        public bool HasCellItemTemplate
        {
            get
            {
                return HasValue(GridStyleInfoStore.CellItemTemplateProperty);
            }
        }
        #endregion
        #region CellEditTemplate
        [
       Description("The Edit DataTemplate for the cell."),
       Browsable(true),
       Category("Template"),
       DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
       ]
        public DataTemplate CellEditTemplate
        {
            get
            {
                return (DataTemplate)GetValue(GridStyleInfoStore.CellEditTemplateProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.CellEditTemplateProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.CellTemplate"/>.
        /// </summary>
        public void ResetCellEditTemplate()
        {
            ResetValue(GridStyleInfoStore.CellEditTemplateProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCellEditTemplate()
        {
            return HasValue(GridStyleInfoStore.CellEditTemplateProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.CellTemplate"/> has been initialized for the current object.
        /// </summary>
        public bool HasCellEditTemplate
        {
            get
            {
                return HasValue(GridStyleInfoStore.CellEditTemplateProperty);
            }
        }
        #endregion
        #region CellItemTemplateKey
        [
       Description("The Item DataTemplate Key for the cell."),
       Browsable(true),
       Category("Template"),
       DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
       ]
        public string CellItemTemplateKey
        {
            get
            {
                return (string)GetValue(GridStyleInfoStore.CellItemTemplateKeyProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.CellItemTemplateKeyProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.CellTemplateKey"/>.
        /// </summary>
        public void ResetCellItemTemplateKey()
        {
            ResetValue(GridStyleInfoStore.CellItemTemplateKeyProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCellItemTemplateKey()
        {
            return HasValue(GridStyleInfoStore.CellItemTemplateKeyProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.CellTemplateKey"/> has been initialized for the current object.
        /// </summary>
        public bool HasCellItemTemplateKey
        {
            get
            {
                return HasValue(GridStyleInfoStore.CellItemTemplateKeyProperty);
            }
        }
        #endregion
        #region CellEditTemplateKey
        [
       Description("The Edit DataTemplate Key for the cell."),
       Browsable(true),
       Category("Template"),
       DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
       ]
        public string CellEditTemplateKey
        {
            get
            {
                return (string)GetValue(GridStyleInfoStore.CellEditTemplateKeyProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.CellEditTemplateKeyProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.CellTemplateKey"/>.
        /// </summary>
        public void ResetCellEditTemplateKey()
        {
            ResetValue(GridStyleInfoStore.CellEditTemplateKeyProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCellEditTemplateKey()
        {
            return HasValue(GridStyleInfoStore.CellEditTemplateKeyProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.CellTemplateKey"/> has been initialized for the current object.
        /// </summary>
        public bool HasCellEditTemplateKey
        {
            get
            {
                return HasValue(GridStyleInfoStore.CellEditTemplateKeyProperty);
            }
        }
        #endregion


        #region CellType
        /// <summary>
        /// Gets or sets the cell type information for the cell.
        /// </summary>
        [
        Description("Contains cell type information of a cell."),
        Browsable(true),
        TypeConverter(typeof(GridCellTypeNameConverter)),
        Category("Data")
        ]
        public string CellType
        {
            get
            {
                return (string)GetValue(GridStyleInfoStore.CellTypeProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.CellTypeProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.CellType"/>.
        /// </summary>
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
        /// Determines if <see cref="GridStyleInfo.CellType"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCellType
        {
            get
            {
                return HasValue(GridStyleInfoStore.CellTypeProperty);
            }
        }
        #endregion
        #region CellValue
        /// <summary>
        /// Gets or sets the cell value information for the cell.
        /// </summary>
        [
        Description("Contains cell value information of a cell."),
        Browsable(false),
        Category("Data"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public object CellValue
        {
            get
            {
                return GetValue(GridStyleInfoStore.CellValueProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.CellValueProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.CellValue"/>.
        /// </summary>
        public void ResetCellValue()
        {
            ResetValue(GridStyleInfoStore.CellValueProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCellValue()
        {
            return HasValue(GridStyleInfoStore.CellValueProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.CellValue"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCellValue
        {
            get
            {
                return HasValue(GridStyleInfoStore.CellValueProperty);
            }
        }
        #endregion
        #region CellValueType
        /// <summary>
        /// Gets or sets the cell value type information for the cell.
        /// </summary>
        [
        Description("Contains cell value type information of a cell."),
        Browsable(true),
        Category("Data"),
        ]
        public Type CellValueType
        {
            get
            {
                return (Type)GetValue(GridStyleInfoStore.CellValueTypeProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.CellValueTypeProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.CellValueType"/>.
        /// </summary>
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
        /// Determines if <see cref="GridStyleInfo.CellValueType"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCellValueType
        {
            get
            {
                return HasValue(GridStyleInfoStore.CellValueTypeProperty);
            }
        }
        #endregion
        #region CultureInfo
        /// <summary>
        /// Gets or sets the culture information holds rules for parsing and formatting the cell's value.
        /// </summary>
        [
        Description("The culture information holds rules for parsing and formatting the cell's value."),
        Browsable(true),
        TypeConverter(typeof(CultureInfoConverter)),
        ImmutableObject(true)
        ]
        [CloneableProperty(false), DisposeableProperty(false)]
        public CultureInfo CultureInfo
        {
            get
            {
                return (CultureInfo)GetValue(GridStyleInfoStore.CultureInfoProperty);
            }
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
                culture = CultureInfo.CurrentCulture.Clone() as CultureInfo;
            //else
            //    if (!CurrencyEdit.UseCultureInfo)
            //        culture = CultureInfo.CurrentCulture;

            return culture;
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.CultureInfo"/>.
        /// </summary>
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
        /// Determines if <see cref="GridStyleInfo.CultureInfo"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCultureInfo
        {
            get
            {
                return HasValue(GridStyleInfoStore.CultureInfoProperty);
            }
        }
        #endregion
        #region Description
        /// <summary>
        /// Gets / sets the text that is shown in check box or pushbuttons.
        /// </summary>
        [
        Description("Gets / sets the text that is shown in check box or pushbuttons."),
        Browsable(true),
        Category("Data")
        ]
        public string Description
        {
            get
            {
                return (string)GetValue(GridStyleInfoStore.DescriptionProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.DescriptionProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.Description"/>.
        /// </summary>
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
        /// Determines if <see cref="GridStyleInfo.Description"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasDescription
        {
            get
            {
                return HasValue(GridStyleInfoStore.DescriptionProperty);
            }
        }
        #endregion
        #region Enabled
        /// <summary>
        /// Gets or sets a value indicating whether the cell can be activated as current cell or if the cell should be skipped when moving the current cell.
        /// </summary>
        [
        Description("Specifies if the cell can be activated as current cell or if the cell should be skipped when moving the current cell."),
        Browsable(true)
        ]
        public bool Enabled
        {
            get
            {
                return GetShortValue(GridStyleInfoStore.EnabledProperty) != 0;
            }
            set
            {
                SetValue(GridStyleInfoStore.EnabledProperty, value ? 1 : 0);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.Enabled"/>.
        /// </summary>
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
        /// Determines if <see cref="GridStyleInfo.Enabled"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasEnabled
        {
            get
            {
                return HasValue(GridStyleInfoStore.EnabledProperty);
            }
        }
        #endregion
        #region Error
        /// <summary>
        /// Gets or sets the error information if a text could not be converted to the Type specified with CellValueType.
        /// </summary>
        [
        Description("Holds error information if a text could not be converted to the Type specified with CellValueType."),
        Browsable(false),
        Category("Data")
        ]
        public string Error
        {
            get
            {
                return (string)GetValue(GridStyleInfoStore.ErrorProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.ErrorProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.Error"/>.
        /// </summary>
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
        /// Determines if <see cref="GridStyleInfo.Error"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasError
        {
            get
            {
                return HasValue(GridStyleInfoStore.ErrorProperty);
            }
        }
        #endregion
        #region ErrorInfo
        /// <summary>
        /// Gets or sets the <see cref="GridErrorStyleInfo"/> for the cell.
        /// </summary>
        public GridErrorStyleInfo ErrorInfo
        {
            get
            {
                return (GridErrorStyleInfo)this.GetValue(GridStyleInfoStore.ErrorInfoProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.ErrorInfoProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.ErrorInfo"/>.
        /// </summary>
        public void ResetErrorInfo()
        {
            this.ResetValue(GridStyleInfoStore.ErrorInfoProperty);
        }

        private bool ShouldSerializeErrorInfo()
        {
            return this.HasValue(GridStyleInfoStore.ErrorInfoProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.ErrorInfo"/> has been initialized for the current object.
        /// </summary>
        public bool HasErrorInfo
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.ErrorInfoProperty);
            }
        }
        #endregion
        #region Exception
        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        [
        Description("Holds exception."),
        Browsable(false),
        Category("Data")
        ]
        public Exception Exception
        {
            get
            {
                return (Exception)GetValue(GridStyleInfoStore.ExceptionProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.ExceptionProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.Exception"/>.
        /// </summary>
        public void ResetException()
        {
            ResetValue(GridStyleInfoStore.ExceptionProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeException()
        {
            return HasValue(GridStyleInfoStore.ExceptionProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.Exception"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasException
        {
            get
            {
                return HasValue(GridStyleInfoStore.ExceptionProperty);
            }
        }
        #endregion
        #region Format
        /// <summary>
        /// Gets or sets the cell format.
        /// </summary>
        [
        Description("Contains format information of a cell."),
        Browsable(true),
        Category("Appearance"),
        ]
        public string Format
        {
            get
            {
                return (string)GetValue(GridStyleInfoStore.FormatProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.FormatProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.Format"/>.
        /// </summary>
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
        /// Determines if <see cref="GridStyleInfo.Format"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasFormat
        {
            get
            {
                return HasValue(GridStyleInfoStore.FormatProperty);
            }
        }
        #endregion
        #region FormatProvider

        /// <summary>
        /// Gets or sets the cell FormatProvider.
        /// </summary>
        [
        Description("Contains format information of a cell."),
        Browsable(true),
        Category("Appearance"),
        ]
        public IFormatProvider FormatProvider
        {
            get
            {
                return (IFormatProvider)GetValue(GridStyleInfoStore.FormatProviderProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.FormatProviderProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.FormatProvider"/>.
        /// </summary>
        public void ResetFormatProvider()
        {
            ResetValue(GridStyleInfoStore.FormatProviderProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeFormatProvider()
        {
            return HasValue(GridStyleInfoStore.FormatProviderProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.HasFormatProvider"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasFormatProvider
        {
            get
            {
                return HasValue(GridStyleInfoStore.FormatProviderProperty);
            }
        }
        #endregion
        #region ParseFormats
        /// <summary>
        /// Specifies the permissable formats used to parse user entries of cell values.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the ParseFormats property is NULL.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// It can be used to specify various DateTime formats that are allowed when the user enters a DateTime cell value.
        /// <para/>
        /// </remarks>
        [
        Description("Specifies the permissable formats used to parse a user entry."),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Category("Appearance")
        ]
        [NotifyParentProperty(true)]
        public string[] ParseFormats
        {
            get
            {
                string[] c = (string[])GetValue(GridStyleInfoStore.ParseFormatsProperty);
                // TODO: ChoiceList Colledtion Editor-
                //SetValue(GridStyleInfoStore.ParseFormatstProperty, c);
                return c;
            }
            set
            {
                SetValue(GridStyleInfoStore.ParseFormatsProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.ParseFormats"/>.
        /// </summary>
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
        /// Determines if <see cref="GridStyleInfo.ParseFormats"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasParseFormats
        {
            get
            {
                return HasValue(GridStyleInfoStore.ParseFormatsProperty);
            }
        }
        #endregion
        #region ReadOnly
        /// <summary>
        /// Gets or sets a value indicating whether cell contents can be modified by the user.
        /// </summary>
        [
        Description("Specifies if cell contents can be modified by the user. You can programmatically change Read-only cells by setting IgnoreReadOnly to True."),
        Browsable(true),
        Category("Data")
        ]
        public bool ReadOnly
        {
            get
            {
                return GetShortValue(GridStyleInfoStore.ReadOnlyProperty) != 0;
            }
            set
            {
                SetValue(GridStyleInfoStore.ReadOnlyProperty, value ? 1 : 0);
            }
        }
        /// <summary>
        /// Resets Read-only information.
        /// </summary>
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
        /// Determines if Read-only information has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasReadOnly
        {
            get
            {
                return HasValue(GridStyleInfoStore.ReadOnlyProperty);
            }
        }
        #endregion
        #region StrictValueType
        /// <summary>
        /// Gets or sets a value indicating whether an exception should be thrown in the <see cref="ApplyFormattedText"/> method
        /// if the formatted text can not be parsed and converted to the type specified with
        /// <see cref="CellValueType"/>.
        /// </summary>
        [
        Browsable(true),
        Category("Data"),
        ]
        public bool StrictValueType
        {
            get
            {
                return GetShortValue(GridStyleInfoStore.StrictValueTypeProperty) != 0;
            }
            set
            {
                SetValue(GridStyleInfoStore.StrictValueTypeProperty, value ? 1 : 0);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.StrictValueType"/>.
        /// </summary>
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
        /// Determines if <see cref="GridStyleInfo.StrictValueType"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasStrictValueType
        {
            get
            {
                return HasValue(GridStyleInfoStore.StrictValueTypeProperty);
            }
        }
        #endregion

        // Dropdown, choicelist, databinding
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
        ///         <term><see langword="ComboBox"/>  (<see cref="GridCellComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownList"/>  (<see cref="GridCellGridListControlDropDownCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [
        Category(@"Data"),
        Editor(@"System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", @"System.Drawing.Design.UITypeEditor, System.Drawing"),
            //DefaultValue(@""),
            //TypeConverter(@"System.Windows.Forms.Design.DataMemberFieldConverter, System.Design"),
        Description(@"Indicates the property to display for the items in this control.")
        ]
        [NotifyParentProperty(true)]
        public string DisplayMember
        {
            get
            {
                return (string)GetValue(GridStyleInfoStore.DisplayMemberProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.DisplayMemberProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.DisplayMember"/>.
        /// </summary>
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
        /// Determines if <see cref="GridStyleInfo.DisplayMember"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasDisplayMember
        {
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
        ///         <term><see langword="ComboBox"/>  (<see cref="GridCellComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownList"/>  (<see cref="GridCellGridListControlDropDownCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [
        Description(@"Indicates the property to use as the actual value for the items in the control."),
        Category(@"Data"),
            //		DefaultValue(@""),
        Editor(@"System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", @"System.Drawing.Design.UITypeEditor, System.Drawing")
        ]
        [NotifyParentProperty(true)]
        public string ValueMember
        {
            get
            {
                return (string)GetValue(GridStyleInfoStore.ValueMemberProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.ValueMemberProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.ValueMember"/>.
        /// </summary>
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
        /// Determines if <see cref="GridStyleInfo.ValueMember"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasValueMember
        {
            get
            {
                return HasValue(GridStyleInfoStore.ValueMemberProperty);
            }
        }
        #endregion
        #region AutoPopulateDropDownColumns
        // need to provide some attribute................
        /// <summary>
        /// Gets or sets a value indicating whether [auto populate drop down columns].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [auto populate drop down columns]; otherwise, <c>false</c>.
        /// </value>
        [NotifyParentProperty(true)]
        public bool AutoPopulateDropDownColumns
        {
            get 
            { 
                return (bool)GetValue(GridStyleInfoStore.AutoPopulateDropDownColumnsProperty); 
            }
            set
            {
                SetValue(GridStyleInfoStore.AutoPopulateDropDownColumnsProperty, value);
            }
        }
        public void ResetAutoPopulateDropDownColumns()
        {
            ResetValue(GridStyleInfoStore.AutoPopulateDropDownColumnsProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeAutoPopulateDropDownColumns()
        {
            return HasValue(GridStyleInfoStore.AutoPopulateDropDownColumnsProperty);
        }
        /// <summary>
        /// Gets a value indicating whether this instance has auto populate drop down columns.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has auto populate drop down columns; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasAutoPopulateDropDownColumns
        {
            get
            {
                return HasValue(GridStyleInfoStore.AutoPopulateDropDownColumnsProperty);
            }
        }
        #endregion
        #region DropDownColumnSizer
        /// <summary>
        /// Gets or sets the drop down column sizer.
        /// </summary>
        /// <value>The drop down column sizer.</value>
        [NotifyParentProperty(true)]
        public GridControlLengthUnitType DropDownColumnSizer
        {
            get
            {
                return (GridControlLengthUnitType)GetValue(GridStyleInfoStore.DropDownColumnSizerProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.DropDownColumnSizerProperty, value);
            }
        }
        public void ResetDropDownColumnSizer()
        {
            ResetValue(GridStyleInfoStore.DropDownColumnSizerProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeDropDownColumnSizer()
        {
            return HasValue(GridStyleInfoStore.DropDownColumnSizerProperty);
        }
        /// <summary>
        /// Gets a value indicating whether this instance has drop down column sizer.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has drop down column sizer; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasDropDownColumnSizer
        {
            get
            {
                return HasValue(GridStyleInfoStore.DropDownColumnSizerProperty);
            }
        }
        #endregion
        #region DropDownVisibleColumns
        /// <summary>
        /// Gets or sets the drop down visible columns.
        /// </summary>
        /// <value>The drop down visible columns.</value>
        [NotifyParentProperty(true)]
        public GridListColumnsCollection DropDownVisibleColumns
        {
            get
            {
                return (GridListColumnsCollection)GetValue(GridStyleInfoStore.DropDownVisibleColumnsProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.DropDownVisibleColumnsProperty, value);
            }
        }
        public void ResetDropDownVisibleColumns()
        {
            ResetValue(GridStyleInfoStore.DropDownVisibleColumnsProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeDropDownVisibleColumns()
        {
            return HasValue(GridStyleInfoStore.DropDownVisibleColumnsProperty);
        }
        /// <summary>
        /// Gets a value indicating whether this instance has drop down visible columns.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has drop down visible columns; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasDropDownVisibleColumns
        {
            get
            {
                return HasValue(GridStyleInfoStore.DropDownVisibleColumnsProperty);
            }
        }
        #endregion
        #region ItemsSource
        /// <summary>
        /// Gets or sets a data source that holds items to be displayed in a drop-down list. A datasource can be specified instead of manually filling the choicelist with string entries.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the DataSource property is null.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridCellComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownList"/>  (<see cref="GridCellGridListControlDropDownCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [
        Description(@"Indicates the list that this cell will use to get its items."),
        Category(@"Data"),
        ]
        [CloneableProperty(false), DisposeableProperty(false)] // REVIEW:
        [NotifyParentProperty(true)]
        public object ItemsSource
        {
            get
            {
                return (object)GetValue(GridStyleInfoStore.ItemsSourceProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.ItemsSourceProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.ItemsSource"/>.
        /// </summary>
        public void ResetItemsSource()
        {
            ResetValue(GridStyleInfoStore.ItemsSourceProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeItemsSource()
        {
            return HasValue(GridStyleInfoStore.ItemsSourceProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.ItemsSource"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasItemsSource
        {
            get
            {
                return HasValue(GridStyleInfoStore.ItemsSourceProperty);
            }
        }
        #endregion
        #region ChildRelationalColumn
        /// <summary>
        /// Gets or sets the Mapping name of child relational column
        /// </summary>
        [
        Description(@"Indicates the Mapping name of child relational column"),
        Category(@"Relation")
        ]
        public string ChildRelationalColumn
        {
            get
            {
                return (string)GetValue(GridStyleInfoStore.ChildRelationalColumnProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.ChildRelationalColumnProperty, value);
            }
        }
        #endregion
        #region CellValue2
        /// <summary>
        /// Gets or sets the cell value 2 information of the cell.
        /// </summary>
        [
        Description("Contains cell value 2 information of the cell."),
        Browsable(false),
        Category("Data"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public object CellValue2
        {
            get
            {
                return GetValue(GridStyleInfoStore.CellValue2Property);
            }
            set
            {
                SetValue(GridStyleInfoStore.CellValue2Property, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.CellValue2"/>.
        /// </summary>
        public void ResetCellValue2()
        {
            ResetValue(GridStyleInfoStore.CellValue2Property);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCellValue2()
        {
            return HasCellValue2;
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.CellValue2"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCellValue2
        {
            get
            {
                return HasValue(GridStyleInfoStore.CellValue2Property);
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
        ///         <term><see langword="ComboBox"/>  (<see cref="GridCellComboBoxCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        [
        Description("Specifies items to be displayed in a drop-down list."),
        Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design",
            "System.Drawing.Design.UITypeEditor, System.Drawing"),
        DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content),
        Browsable(false),
        RefreshProperties(RefreshProperties.Repaint),
        Category("StyleCategoryValue")
        ]
        [NotifyParentProperty(true)]
        public StringCollection ChoiceList
        {
            get
            {
                StringCollection c = (StringCollection)GetValue(GridStyleInfoStore.ChoiceListProperty);
                // TODO: ChoiceList Colledtion Editor-
                //SetValue(GridStyleInfoStore.ChoiceListProperty, c);
                return c;
            }
            set
            {
                SetValue(GridStyleInfoStore.ChoiceListProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.ChoiceList"/>.
        /// </summary>
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
        /// Determines if <see cref="GridStyleInfo.ChoiceList"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasChoiceList
        {
            get
            {
                return HasValue(GridStyleInfoStore.ChoiceListProperty);
            }
        }
        #endregion
        #region IsEditable
        /// <summary>
        /// Gets or sets a value indicating whether user input is restricted to items from the ChoiceList or ItemsSource.
        /// </summary>
        [
        Description("Specifies if user input is restricted to items from the ChoiceList or ItemsSource"),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Category("StyleCategoryValue")
        ]
        [NotifyParentProperty(true)]
        public bool IsEditable
        {
            get
            {
                return GetShortValue(GridStyleInfoStore.IsEditableProperty) != 0;
            }
            set
            {
                SetValue(GridStyleInfoStore.IsEditableProperty, value ? 1 : 0);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.IsEditable"/>.
        /// </summary>
        public void ResetIsEditable()
        {
            ResetValue(GridStyleInfoStore.IsEditableProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeIsEditable()
        {
            return HasValue(GridStyleInfoStore.IsEditableProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.IsEditable"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasIsEditable
        {
            get
            {
                return HasValue(GridStyleInfoStore.IsEditableProperty);
            }
        }
        #endregion


        #region IsThreeState
        /// <summary>
        /// Gets or sets  the Tristate value for the CheckBox. 
        /// </summary>
        [
        Description("Specifies if the Checkbox should allow Tristate value. "),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Category("StyleCategoryValue")
        ]
        [NotifyParentProperty(true)]
        public bool IsThreeState
            {
            get
                {
                return GetShortValue(GridStyleInfoStore.IsThreeStateProperty) != 0;
                }
            set
                {
                SetValue(GridStyleInfoStore.IsThreeStateProperty, value ? 1 : 0);
                }
            }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.IsThreeState"/>.
        /// </summary>
        public void ResetIsThreeState()
            {
            ResetValue(GridStyleInfoStore.IsThreeStateProperty);
            }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeIsThreeState()
            {
            return HasValue(GridStyleInfoStore.IsThreeStateProperty);
            }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.IsThreeState"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasIsThreeState
            {
            get
                {
                return HasValue(GridStyleInfoStore.IsThreeStateProperty);
                }
            }
        #endregion



        #region PropertyDescriptor
        /// <summary>
        /// Specifies a property descriptor that can be used by UITypeEditCell, PropertyGridCell, and StandardValuesCell cell types.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PropertyDescriptor PropertyDescriptor
        {
            get
            {
                return (PropertyDescriptor)GetValue(GridStyleInfoStore.PropertyDescriptorProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.PropertyDescriptorProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.PropertyDescriptor"/>.
        /// </summary>
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
        /// Determines if <see cref="GridStyleInfo.PropertyDescriptor"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasPropertyDescriptor
        {
            get
            {
                return HasValue(GridStyleInfoStore.PropertyDescriptorProperty);
            }
        }
        #endregion

        // Text, Formatted Text
        #region Formatted Text
        /// <summary>
        /// Returns a formatted text for the default value for a specified <see cref="CellValueType"/>.
        /// </summary>
        [Description("Returns a formatted text for the default value for a specified Cell"),
        Browsable(true),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public string FormatPreview
        {
            get
            {
                object obj = ValueConvert.GetDefaultValue(this.CellValueType);
                if (obj != null)
                    return GetFormattedText(obj);
                return "";
            }
        }

        /// <summary>
        /// Gets the value formatted with the <see cref="GridStyleInfo.Format"/> mask and custom formatting of the <see cref="GridCellModelBase.GetFormattedText"/> method of the associated <see cref="GridCellModelBase"/> or sets the value by calling the <see cref="GridCellModelBase.ApplyFormattedText"/> of the associated <see cref="GridCellModelBase"/>.
        /// </summary>
        [
        Browsable(true),
        RefreshProperties(RefreshProperties.Repaint),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Gets the formatted text value")
        ]
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
                    if (!BrowserInteropHelper.IsBrowserHosted)
                    {
                        TraceUtil.TraceExceptionCatched(ex);
                    }
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        throw;

                    return value != null ? value.ToString() : "";
                }
            }
            set
            {
                if (!ApplyFormattedText(value, GridCellBaseTextInfo.DisplayText))
                    throw new ArgumentException();
            }
        }
        #endregion
        #region Text
        /// <summary>
        /// Gets / sets the value as a string.
        /// </summary>
        [
        Description("Gets / sets the value as a string."),
        Browsable(true),
        Category("Appearance"),
        ]
        public string Text
        {
            get
            {
                object value = CellValue;
                return GetText(CellValue);
            }
            set
            {
                ApplyText(value);
            }
        }
        /// <summary>
        /// Resets the <see cref="Text"/> property.
        /// </summary>
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
        /// Determines if <see cref="GridStyleInfo.Text"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasText
        {
            get
            {
                return HasValue(GridStyleInfoStore.CellValueProperty);
            }
        }
        #endregion
        #region Text Conversion

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
                    return ValueConvert.FormatValue(value, CellValueType, Format, ci, nfi, FormatProvider);
                }
                return cellModel.GetFormattedText(this, value, GridCellBaseTextInfo.DisplayText);
            }
            catch (Exception ex)
            {
                if (!BrowserInteropHelper.IsBrowserHosted)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                }
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    throw;

                return value != null ? value.ToString() : "";
            }
        }


        internal string GetFormulaValue(string formula, GridFormulaTag tag)
        {
            GridCellModelBase cellModel = this.CellModel;
            if (cellModel == null)
            {
                return (string)ValueConvert.ChangeType(formula, typeof(string), CultureInfo.CurrentCulture);
            }
            return cellModel.GetFormulaValue(this, formula, tag);
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
                    CellValue = ValueConvert.Parse(text, CellValueType, nfi, Format);
                    ResetError();
                }
                catch (Exception ex)
                {
                    Error = ex.Message;
                    if (StrictValueType)
                        throw;

                    else if (ex is FormatException || ex.InnerException is FormatException)
                    {
                        CellValue = text;
                        // possibly could also change CellValueType here based on input string
                        // e.Style.CellValueType = typeof(string);
                    }
                    else
                        throw;
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
                return ValueConvert.FormatValue(value, CellValueType, Format, ci, nfi, FormatProvider);
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
                CellValue = ValueConvert.Parse(text, CellValueType, nfi, Format);
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
                        return "";
                    return (string)ValueConvert.ChangeType(value, typeof(string), CultureInfo.CurrentCulture);
                }
                return cellModel.GetText(this, value);
            }
            catch (Exception ex)
            {
                if (!BrowserInteropHelper.IsBrowserHosted)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                }
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    throw;

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
            GridCellModelBase cellModel = this.CellModel;
            if (cellModel == null)
            {
                CultureInfo ci = CultureInfo.CurrentCulture;
                NumberFormatInfo nfi = ci != null ? ci.NumberFormat : null;
                try
                {
                    CellValue = ValueConvert.Parse(text, CellValueType, nfi, "");
                    ResetError();
                }
                catch (Exception ex)
                {
                    Error = ex.Message;
                    if (StrictValueType)
                        throw;

                    else if (ex is FormatException || ex.InnerException is FormatException)
                    {
                        CellValue = text;
                        // possibly could also change CellValueType here based on input string
                        // e.Style.CellValueType = typeof(string);
                    }
                    else
                        throw;
                }
                return true;
            }
            return cellModel.ApplyText(this, text);
        }
        #endregion

        // cell images
        #region ImageList

        /// <summary>
        /// Gets or Sets an image to the underlying ImageList collection. This is a wrapper property that provides a way to easily set one image for this style.
        /// </summary>
        public Image Image
        {
            get
            {
                var hasImage = this.HasImageList;
                if (hasImage && this.ImageList != null)
                {
                    this.ImageIndex = 0;
                    return this.ImageList[0];
                }
                return null;
            }

            set
            {
                if (!this.HasImageList)
                {
                    this.ImageList = new ObservableCollection<Image>();
                    this.ImageIndex = 0;
                }
                if (this.ImageList != null)
                {
                    this.ImageList.Insert(0, value);
                }
            }
        }

        /// <summary>
        /// Gets or Sets a list of Images. This would have higher preference than ErrorInfo image.
        /// </summary>
        public ObservableCollection<Image> ImageList
        {
            get
            {
                return (ObservableCollection<Image>)this.GetValue(GridStyleInfoStore.ImageListProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.ImageListProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.ImageList"/>.
        /// </summary>
        public void ResetImageList()
        {
            this.ResetValue(GridStyleInfoStore.ImageListProperty);
        }

        /// <summary>
        /// Determines whether <see cref="GridStyleInfo.ImageList"/> is initialized for current object.
        /// </summary>
        public bool HasImageList
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.ImageListProperty);
            }
        }

        #endregion

        #region ImageIndex

        /// <summary>
        /// Gets or Sets the ImageIndex that is referred from style.ImageList property.
        /// </summary>
        public int ImageIndex
        {
            get
            {
                if (this.GetValue(GridStyleInfoStore.ImageIndexProperty) == null)
                {
                    return -1;
                }

                return (int)this.GetValue(GridStyleInfoStore.ImageIndexProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.ImageIndexProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.ImageIndex"/>.
        /// </summary>
        public void ResetImageIndex()
        {
            this.ResetValue(GridStyleInfoStore.ImageIndexProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridStyleInfo.ImageIndex"/> is initialized for current object.
        /// </summary>
        public bool HasImageIndex
        {
            get
            {
                var hasValue = this.HasValue(GridStyleInfoStore.ImageIndexProperty);
                if (hasValue && this.HasImageList)
                {
                    hasValue = this.ImageIndex < this.ImageList.Count ? true : false;
                }
                return hasValue;
            }
        }

        #endregion

        #region ImageContentStretch

        /// <summary>
        /// Used internally.
        /// </summary>
        public ImageContentStretch ReadOnlyImageContentStretch
        {
            get
            {
                if (this.HasImageContentStretch)
                {
                    return this.ImageContentStretch;
                }

                return Default.ImageContentStretch;
            }
        }

        /// <summary>
        /// Specifies whether to stretch the image or retain the image size when a row or column is resized.
        /// </summary>
        public ImageContentStretch ImageContentStretch
        {
            get
            {
                return (ImageContentStretch)this.GetValue(GridStyleInfoStore.ImageContentStretchProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.ImageContentStretchProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.ImageContentStretch"/>.
        /// </summary>
        public void ResetImageContentStretch()
        {
            this.ResetValue(GridStyleInfoStore.ImageContentStretchProperty);
        }

        /// <summary>
        /// Determines whether <see cref="GridStyleInfo.ImageContentStretch"/> is initialized for current object.
        /// </summary>
        public bool HasImageContentStretch
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.ImageContentStretchProperty);
            }
        }

        #endregion

        #region ImageContentAlignment
        /// <summary>
        /// For internal use.
        /// </summary>
        public ImageContentAlignment ReadOnlyImageContentAlignment
        {
            get
            {
                if (this.HasImageContentAlignment)
                {
                    return this.ImageContentAlignment;
                }

                return Default.ImageContentAlignment;
            }
        }
        /// <summary>
        /// Defines the IncrementalFilter Value for Combo Box
        /// </summary>
        public IncrementalFilter IncrementalFilter
        {
            get
            {
                return (IncrementalFilter)this.GetValue(GridStyleInfoStore.IncrementalFilterProperty);
            }
            set
            {
                this.SetValue(GridStyleInfoStore.IncrementalFilterProperty, value);
            }
        }

        /// <summary>
        /// Defines the alignment options for text in text-image cells.
        /// </summary>

        public ImageContentAlignment ImageContentAlignment
        {
            get
            {
                return (ImageContentAlignment)this.GetValue(GridStyleInfoStore.ImageContentAlignmentProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.ImageContentAlignmentProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.ImageContentAlignment"/>.
        /// </summary>
        public void ResetImageContentAlignment()
        {
            this.ResetValue(GridStyleInfoStore.ImageContentAlignmentProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridStyleInfo.ImageContentAlignment"/> is initialized for current object.
        /// </summary>
        public bool HasImageContentAlignment
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.ImageContentAlignmentProperty);
            }
        }

        #endregion

        #region ImageMargins
        /// <summary>
        /// Adjusts image margins according to client rectangle.
        /// </summary>
        /// <param name="rectangle">Client rectangle.</param>
        /// <returns>Corrected margins.</returns>
        public Rect AdjustImageMargins(Rect rectangle)
        {
            var right = this.ReadOnlyImageMargins.Right;
            var bottom = this.ReadOnlyImageMargins.Bottom;
            if (right < rectangle.Width && bottom < rectangle.Height)
            {
                return new Rect(rectangle.X + this.ReadOnlyImageMargins.Left, rectangle.Y + this.ReadOnlyImageMargins.Top, rectangle.Width - right, rectangle.Height - bottom);
            }
            return new Rect(rectangle.X + this.ReadOnlyImageMargins.Left, rectangle.Y + this.ReadOnlyImageMargins.Top, rectangle.Width, rectangle.Height);
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        public CellMarginsInfo ReadOnlyImageMargins
        {
            get
            {
                if (this.HasImageMargins)
                {
                    return this.ImageMargins;
                }

                return Default.ImageMargins;
            }
        }

        /// <summary>
        /// Gets or sets the image margins.
        /// </summary>
        public CellMarginsInfo ImageMargins
        {
            get
            {
                return (CellMarginsInfo)this.GetValue(GridStyleInfoStore.ImageMarginsProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.ImageMarginsProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.ImageMargins"/>.
        /// </summary>
        public void ResetImageMargins()
        {
            this.ResetValue(GridStyleInfoStore.ImageMarginsProperty);
        }

        /// <summary>
        /// Determines whether <see cref="GridStyleInfo.ImageMargins"/> is initialized for current object.
        /// </summary>
        public bool HasImageMargins
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.ImageMarginsProperty);
            }
        }

        #endregion

        #region ImageWidth
        /// <summary>
        /// Adjusts the margins accroding to the width and height of the image.
        /// </summary>
        /// <param name="defaultMargin">Default margins.</param>
        /// <param name="clientSize">Client rectangle.</param>
        /// <returns>Adjusted margins.</returns>
        public Thickness AdjustImageWidthAndHeightToMargin(Thickness defaultMargin, Size clientSize)
        {
            if (this.HasImageIndex && !clientSize.IsEmpty)
            {
                var imageContentStretch = this.ReadOnlyImageContentStretch;

                var width = 0.0;
                switch (imageContentStretch)
                {
                    case ImageContentStretch.Fill:
                        width = (this.GetImageWidth() * clientSize.Width) + 5d;
                        break;
                    case ImageContentStretch.Uniform:
                        width = (this.GetImageWidth() * this.GridModel.ColumnWidths.GetDefaultLineSize()) + 5d;
                        break;
                    case ImageContentStretch.Absolute:
                        width = this.GetImageWidth() + 5d;
                        break;
                }

                if (this.ReadOnlyImageContentAlignment == ImageContentAlignment.Left)
                {
                    return new Thickness(defaultMargin.Left + width, defaultMargin.Top, defaultMargin.Right, defaultMargin.Bottom);
                }
                else
                {
                    return new Thickness(defaultMargin.Left, defaultMargin.Top, defaultMargin.Right + width, defaultMargin.Bottom);
                }
            }
            return defaultMargin;
        }

        /// <summary>
        /// Adjusts the margins accroding to the width and height of the image.
        /// </summary>
        /// <param name="defaultMargin">Default margins.</param>
        /// <param name="grid">The grid.</param>
        /// <returns>Adjusted margins.</returns>
        public Thickness AdjustImageWidthAndHeightToMargin(Thickness defaultMargin, GridControlBase grid)
        {
            var rect = grid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, GridRangeInfo.Cell(this.RowIndex, this.ColumnIndex), false, false);
            return this.AdjustImageWidthAndHeightToMargin(defaultMargin, rect.Size);
        }

        /// <summary>
        /// Returns the image width.
        /// </summary>
        /// <returns>Width of the image.</returns>
        public double GetImageWidth()
        {
            var value = 0d;
            if (this.ReadOnlyImageWidth.IsStar || this.ReadOnlyImageWidth.IsAbsolute)
            {
                value = this.ReadOnlyImageWidth.Value;
            }
            return value;
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        public GridLength ReadOnlyImageWidth
        {
            get
            {
                if (this.HasImageWidth)
                {
                    return this.ImageWidth;
                }

                return Default.ImageWidth;
            }
        }

        /// <summary>
        /// Gets or sets the image width.
        /// </summary>
        public GridLength ImageWidth
        {
            get
            {
                return (GridLength)this.GetValue(GridStyleInfoStore.ImageWidthProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.ImageWidthProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.ImageWidth"/>.
        /// </summary>
        public void ResetImageWidth()
        {
            this.ResetValue(GridStyleInfoStore.ImageWidthProperty);
        }

        /// <summary>
        /// Determines whether <see cref="GridStyleInfo.ImageWidth"/> is initialized for current object.
        /// </summary>
        public bool HasImageWidth
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.ImageWidthProperty);
            }
        }

        #endregion

        #region ImageHeight

        /// <summary>
        /// Gets the image height.
        /// </summary>
        /// <returns>Height of the image.</returns>
        public double GetImageHeight()
        {
            var value = 0d;
            if (this.ReadOnlyImageHeight.IsStar || this.ReadOnlyImageHeight.IsAbsolute)
            {
                value = this.ReadOnlyImageHeight.Value;
            }
            return value;
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        public GridLength ReadOnlyImageHeight
        {
            get
            {
                if (this.HasImageHeight)
                {
                    return this.ImageHeight;
                }

                return Default.ImageHeight;
            }
        }

        /// <summary>
        /// Gets or sets the image height.
        /// </summary>
        public GridLength ImageHeight
        {
            get
            {
                return (GridLength)this.GetValue(GridStyleInfoStore.ImageHeightProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.ImageHeightProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.ImageHeight"/>.
        /// </summary>
        public void ResetImageHeight()
        {
            this.ResetValue(GridStyleInfoStore.ImageHeightProperty);
        }

        /// <summary>
        /// Determines whether <see cref="GridStyleInfo.ImageHeight"/> is initialized for current object.
        /// </summary>
        public bool HasImageHeight
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.ImageHeightProperty);
            }
        }

        #endregion

        // CellModel determined through CellType
        #region CellModel

        /// <summary>
        /// Returns the associated <see cref="GridCellModelBase"/> for this style object.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridCellModelBase CellModel
        {
            get
            {
                GridStyleInfoIdentity gsid = Identity as GridStyleInfoIdentity;
                if (gsid != null)
                    return gsid.LookupCellModel(this.CellType);
                return null;
            }
        }


        #endregion

        #region IRenderCellInfo implementation

        object IRenderCellInfo.GetCellBackground()
        {
            return Background;
        }

        bool IRenderCellInfo.CanCombineCellBackground(IRenderCellInfo other)
        {
            GridStyleInfo otherStyle = other as GridStyleInfo;
            if (otherStyle == null)
                return false;

            if (Background is SolidColorBrush && otherStyle.Background is SolidColorBrush && (Background as SolidColorBrush).Color == (otherStyle.Background as SolidColorBrush).Color)
            {
                if ((otherStyle.Borders.Bottom != null && otherStyle.Borders.Bottom.Thickness == 0.0) || (otherStyle.Borders.Right != null && otherStyle.Borders.Right.Thickness == 0.0) ||
                    (otherStyle.Borders.Left != null && otherStyle.Borders.Left.Thickness == 0.0) || (otherStyle.Borders.Top != null && otherStyle.Borders.Top.Thickness == 0.0))
                {
                    return true;
                }
            }

            Thickness t = Padding.ToThickness(otherStyle.Padding);
            if (t.Right + t.Left + t.Bottom + t.Top > 0)
                return false;

            return Background is SolidColorBrush && Background.Equals(other.GetCellBackground());
        }

        object IRenderCellInfo.GetCellBorder(CellBorderSide side)
        {
            return Borders[side] == null ? Borders[side] : Borders[side].Clone();
        }

        bool IRenderCellInfo.CanCombineCellBorder(CellBorderSide side, IRenderCellInfo other)
        {
            GridStyleInfo otherStyle = other as GridStyleInfo;
            if (otherStyle == null)
                return false;

            return Borders[side] == other.GetCellBorder(side);
        }

        System.Windows.Thickness IRenderCellInfo.GetBorderMargins()
        {
            return BorderMargins.ToThickness(Padding);
        }

        System.Windows.Thickness IRenderCellInfo.GetPadding()
        {
            return Padding.ToThickness();
        }

        //ICellRenderer IRenderCellInfo.GetCellRenderer()
        //{
        //    return CellRenderer;
        //}

        #endregion

        #region FormulaTag
        /// <summary>
        /// Gets or sets the formula information for the cell.
        /// </summary>
        [
        Description("Contains formula information of a cell."),
        Browsable(false)
        ]
        public GridFormulaTag FormulaTag
        {
            get
            {
                return (GridFormulaTag)GetValue(GridStyleInfoStore.FormulaTagProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.FormulaTagProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.FormulaTag"/>.
        /// </summary>
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
        /// Determines if <see cref="GridStyleInfo.FormulaTag"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasFormulaTag
        {
            get
            {
                return HasValue(GridStyleInfoStore.FormulaTagProperty);
            }
        }
        #endregion
        #region Tag
        /// <summary>
        /// Gets or sets the custom tag you can associate with a cell.
        /// </summary>
        [
        Description("A custom tag you can associate with a cell."),
        Browsable(false),
        Category("Data"),
        ]
        public object Tag
        {
            get
            {
                return GetValue(GridStyleInfoStore.TagProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.TagProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.Tag"/>.
        /// </summary>
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
        /// Determines if <see cref="Tag"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTag
        {
            get
            {
                return HasValue(GridStyleInfoStore.TagProperty);
            }
        }
        #endregion
        #region DropDownStyle
        /// <summary>
        /// Gets or sets if user input is restricted to items from the <see cref="GridStyleInfo.ChoiceList"/> or <see cref="GridStyleInfo.DataSource"/>.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the DropDownStyle property is GridDropDownStyle.Editable.<para/>
        /// The property affects the behavior or appearance of the following cell types:<para/>
        /// <list type="bullet">
        ///     <item>
        ///         <term><see langword="ComboBox"/>  (<see cref="GridCellComboBoxCellRenderer"/>)</term>
        ///     </item>
        ///     <item>
        ///         <term><see langword="DropDownList"/>  (<see cref="GridCellGridListControlDropDownCellRenderer"/>)</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// </remarks>
        public GridDropDownStyle DropDownStyle
        {
            [DebuggerStepThrough()]
            get
            {
                GridDropDownStyle ds = GridDropDownStyle.Editable;
                if (GetShortValue(GridStyleInfoStore.ExclusiveChoiceListProperty) == 1)
                    ds |= GridDropDownStyle.Exclusive;
                if (GetShortValue(GridStyleInfoStore.AutoCompleteProperty) == 1)
                    ds |= GridDropDownStyle.AutoComplete;
                return ds;
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridStyleInfoStore.ExclusiveChoiceListProperty, ((value & GridDropDownStyle.Exclusive) == GridDropDownStyle.Exclusive) ? 1 : 0);
                SetValue(GridStyleInfoStore.AutoCompleteProperty, ((value & GridDropDownStyle.AutoComplete) == GridDropDownStyle.AutoComplete) ? 1 : 0);
            }
        }

        #region IsMouseTrackingEnabled

        /// <summary>
        /// Gets or Sets a value that Enable or Disable the Mouse Tracking in ComboBox or DropDownList <see cref="GridStyleInfo.IsMouseTrackingEnabled"/>.
        /// </summary>
        public bool IsMouseTrackingEnabled
        {
            get
            {
                return (bool)this.GetValue(GridStyleInfoStore.IsMouseTrackingEnabledProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.IsMouseTrackingEnabledProperty, value);
            }
        }


        #endregion
        /// <summary>
        /// Resets <see cref="GridStyleInfo.DropDownStyle"/>.
        /// </summary>
        public void ResetDropDownStyle()
        {
            ResetValue(GridStyleInfoStore.AutoCompleteProperty);
            ResetValue(GridStyleInfoStore.ExclusiveChoiceListProperty);
        }

        /// <summary>
        /// Determines whether <see cref="GridStyleInfo.DropDownStyle"/> is serializable.
        /// </summary>
        /// <returns>True if it is serializable; False otherwise.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeDropDownStyle()
        {
            return HasValue(GridStyleInfoStore.AutoCompleteProperty) || HasValue(GridStyleInfoStore.ExclusiveChoiceListProperty);
        }

        /// <summary>
        /// Determines whether <see cref="GridStyleInfo.DropDownStyle"/> is initialized for the current object.
        /// </summary>
        public bool HasDropDownStyle
        {
            get
            {
                return HasValue(GridStyleInfoStore.AutoCompleteProperty) || HasValue(GridStyleInfoStore.ExclusiveChoiceListProperty);
            }
        }
        #endregion
        #region ShowDropDownHeaders
        public bool ShowDropDownHeaders
        {
            get
            {
                return (bool)this.GetValue(GridStyleInfoStore.ShowDropDownHeadersProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.ShowDropDownHeadersProperty, value);
            }
        }

        public void ResetShowDropDownHeaders()
        {
            this.ResetValue(GridStyleInfoStore.ShowDropDownHeadersProperty);
        }

        public bool HasShowDropDownHeaders
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.ShowDropDownHeadersProperty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeShowDropDownHeaders()
        {
            return this.HasValue(GridStyleInfoStore.ShowDropDownHeadersProperty);
        }
        #endregion
        #region UpDownEdit
        /// <summary>
        /// Gets or sets UpDownEdit state. UpDownEdit is itself an expandable object
        /// with several properties that can be set individually and participate
        /// in style inheritance mechanism.
        /// </summary>
        public GridUpDownEditStyleInfo UpDownEdit
        {
            get
            {
                return (GridUpDownEditStyleInfo)this.GetValue(GridStyleInfoStore.UpDownEditInfoProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.UpDownEditInfoProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.UpDownEdit"/>.
        /// </summary>
        public void ResetUpDownEdit()
        {
            this.ResetValue(GridStyleInfoStore.UpDownEditInfoProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridStyleInfo.UpDownEdit"/> is serializable.
        /// </summary>
        /// <returns>True if it is serializable; False otherwise.</returns>
        public bool ShouldSerializeUpDownEdit()
        {
            return this.HasValue(GridStyleInfoStore.UpDownEditInfoProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridStyleInfo.UpDownEdit"/> is initialized for the current object.
        /// </summary>
        public bool HasUpDownEdit
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.UpDownEditInfoProperty);
            }
        }

        #endregion
        #region StaysOpenOnEdit
        /// <summary>
        ///Gets or sets a value indicating whether the drop-down host stays open during edit.
        /// </summary>
        public bool StaysOpenOnEdit
        {
            get
            {
                if (this.GetValue(GridStyleInfoStore.StaysOpenOnEditProperty) == null)
                {
                    return false;
                }

                return (bool)this.GetValue(GridStyleInfoStore.StaysOpenOnEditProperty);
            }
            set
            {
                this.SetValue(GridStyleInfoStore.StaysOpenOnEditProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.StaysOpenOnEdit"/>.
        /// </summary>
        public void ResetStaysOpenOnEdit()
        {
            this.ResetValue(GridStyleInfoStore.StaysOpenOnEditProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridStyleInfo.StaysOpenOnEdit"/> is serializable.
        /// </summary>
        /// <returns>True if it is serializable; False otherwise.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeStaysOpenOnEdit()
        {
            return this.HasValue(GridStyleInfoStore.StaysOpenOnEditProperty);
        }
        /// <summary>
        /// Determines whether <see cref="GridStyleInfo.StaysOpenOnEdit"/> is initialized for the current object.
        /// </summary>
        public bool HasStaysOpenOnEdit
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.StaysOpenOnEditProperty);
            }
        }
        #endregion
        #region GridConditionalFormat
        public GridConditionalFormat ConditionalFormat
        {
            get
            {
                return (GridConditionalFormat)GetValue(GridStyleInfoStore.ConditionalFormatProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.ConditionalFormatProperty, value);
            }
        }

        public void ResetConditionalFormat()
        {
            ResetValue(GridStyleInfoStore.ConditionalFormatProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeConditionalFormat()
        {
            return HasValue(GridStyleInfoStore.ConditionalFormatProperty);
        }

        public bool HasConditionalFormat
        {
            get
            {
                return HasValue(GridStyleInfoStore.ConditionalFormatProperty);
            }
        }

        [Obsolete]
        public ApplyConditionalBasdOn ApplyConditionalFormatBasdOn
        {
            get
            {
                var applyTo = GetValue(GridStyleInfoStore.ApplyConditionalFormatBasedOnProperty);
                if (applyTo == null)
                    return ApplyConditionalBasdOn.CellValue;
                return (ApplyConditionalBasdOn)applyTo;
            }
            set
            {
                SetValue(GridStyleInfoStore.ApplyConditionalFormatBasedOnProperty, value);
            }
        }

        public ApplyConditionalBasedOn ApplyConditionalFormatBasedOn
        {
            get
            {
                var applyTo = GetValue(GridStyleInfoStore.ApplyConditionalFormatBasedOnProperty);
                if (applyTo == null)
                    return ApplyConditionalBasedOn.CellValue;
                return (ApplyConditionalBasedOn)applyTo;
            }
            set
            {
                SetValue(GridStyleInfoStore.ApplyConditionalFormatBasedOnProperty, value);
            }
        }

        // private bool FormulaConditionalFormat = true; The variable is assigned but it is never used

        #endregion
		
		#region DataValidation Tooltip

        /// <summary>
        /// Gets or sets the Data Validation Tooltip.
        /// </summary>
        /// <value>The data validation tooltip.</value>
        public string DataValidationTooltip
        {
            get
            {
                return (string)GetValue(GridStyleInfoStore.DataValidationTooltipProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.DataValidationTooltipProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has data validation tooltip.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has data validation tooltip; otherwise, <c>false</c>.
        /// </value>
        public bool HasDataValidationTooltip
        {
            get
            {
                return HasValue(GridStyleInfoStore.DataValidationTooltipProperty);
            }
        }

        /// <summary>
        /// Resets the data validation tooltip.
        /// </summary>
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public void ResetDataValidationTooltip()
        {
            ResetValue(GridStyleInfoStore.DataValidationTooltipProperty);
        }

        /// <summary>
        /// Shoulds the serialize data validation tooltip.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeDataValidationTooltip()
        {
            return HasValue(GridStyleInfoStore.DataValidationTooltipProperty);
        }

        /// <summary>
        /// Gets or sets whether to show data validation tooltip.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if show data validation tooltip; otherwise, <c>false</c>.
        /// </value>
        public bool ShowDataValidationTooltip
        {
            get
            {
                return (bool)GetValue(GridStyleInfoStore.ShowDataValidationTooltipProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.ShowDataValidationTooltipProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has show data validation tooltip.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has show data validation tooltip; otherwise, <c>false</c>.
        /// </value>
        public bool HasShowDataValidationTooltip
        {
            get
            {
                return HasValue(GridStyleInfoStore.ShowDataValidationTooltipProperty);
            }
        }

        /// <summary>
        /// Resets the show data validation tooltip.
        /// </summary>
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public void ResetShowDataValidationTooltip()
        {
            ResetValue(GridStyleInfoStore.ShowDataValidationTooltipProperty);
        }

        /// <summary>
        /// Shoulds the serialize show data validation tooltip.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeShowDataValidationTooltip()
        {
            return HasValue(GridStyleInfoStore.ShowDataValidationTooltipProperty);
        }

        /// <summary>
        /// Gets or sets the data validation tooltip template key.
        /// </summary>
        /// By using this we can customize the data validation tooltip.
        /// <value>The data validation tooltip template key.</value>
        public string DataValidationTooltipTemplateKey
        {
            get
            {
                return (string)GetValue(GridStyleInfoStore.DataValidationTooltipTemplateKeyProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.DataValidationTooltipTemplateKeyProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has data validation tooltip template key.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has data validation tooltip template key; otherwise, <c>false</c>.
        /// </value>
        public bool HasDataValidationTooltipTemplateKey
        {
            get
            {
                return HasValue(GridStyleInfoStore.DataValidationTooltipTemplateKeyProperty);
            }
        }

        /// <summary>
        /// Resets the data validation tooltip template key.
        /// </summary>
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public void ResetDataValidationTooltipTemplateKey()
        {
            ResetValue(GridStyleInfoStore.DataValidationTooltipTemplateKeyProperty);
        }

        /// <summary>
        /// Shoulds the serialize data validation tooltip template key.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeDataValidationTooltipTemplateKey()
        {
            return HasValue(GridStyleInfoStore.DataValidationTooltipTemplateKeyProperty);
        }

        /// <summary>
        /// Gets or sets the data validation tooltip location.
        /// </summary>
        /// By using this property, we can specify the data validation tooltip location.
        /// <value>The data validation tooltip location.</value>
        public Point DataValidationTooltipLocation
        {
            [DebuggerStepThrough()]
            get
            {
                return (Point)GetValue(GridStyleInfoStore.DataValidationTooltipLocationProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.DataValidationTooltipLocationProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has data validation tooltip location.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has data validation tooltip location; otherwise, <c>false</c>.
        /// </value>
        public bool HasDataValidationTooltipLocation
        {
            get
            {
                return HasValue(GridStyleInfoStore.DataValidationTooltipLocationProperty);
            }
        }

        /// <summary>
        /// Resets the data validation tooltip location.
        /// </summary>
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public void ResetDataValidationTooltipLocation()
        {
            ResetValue(GridStyleInfoStore.DataValidationTooltipLocationProperty);
        }

        /// <summary>
        /// Shoulds the serialize data validation tooltip location.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeDataValidationTooltipLocation()
        {
            return HasValue(GridStyleInfoStore.DataValidationTooltipLocationProperty);
        }

        #endregion

        #region ErrorAlert

        /// <summary>
        /// Gets or sets the error alert title.
        /// Used to hold the error alert tile, while import the excel file to grid.
        /// </summary>
        /// <value>The error alert title.</value>
        public string ErrorAlertTitle
        {
            get
            {
                return (string)GetValue(GridStyleInfoStore.ErrorAlertTitleProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.ErrorAlertTitleProperty, value);
            }
        }

        [Obsolete]
        /// <summary>
        /// Gets or sets the error alert title.
        /// Used to hold the error alert tile, while import the excel file to grid.
        /// </summary>
        /// <value>The error alert title.</value>
        public string ErrorAlartTitle
        {
            get
            {
                return (string)GetValue(GridStyleInfoStore.ErrorAlartTitleProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.ErrorAlartTitleProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has error alert title.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has error alert title; otherwise, <c>false</c>.
        /// </value>
        public bool HasErrorAlertTitle
        {
            get
            {
                return HasValue(GridStyleInfoStore.ErrorAlertTitleProperty);
            }
        }

        [Obsolete]
        /// <summary>
        /// Gets a value indicating whether this instance has error alert title.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has error alert title; otherwise, <c>false</c>.
        /// </value>
        public bool HasErrorAlartTitle
        {
            get
            {
                return HasValue(GridStyleInfoStore.ErrorAlartTitleProperty);
            }
        }

        /// <summary>
        /// Resets the error alert title.
        /// </summary>
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public void ResetErrorAlertTitle()
        {
            ResetValue(GridStyleInfoStore.ErrorAlertTitleProperty);
        }

        [Obsolete]
        /// <summary>
        /// Resets the error alert title.
        /// </summary>
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public void ResetErrorAlartTitle()
        {
            ResetValue(GridStyleInfoStore.ErrorAlartTitleProperty);
        }

        /// <summary>
        /// Shoulds the serialize error alert title.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeErrorAlertTitle()
        {
            return HasValue(GridStyleInfoStore.ErrorAlertTitleProperty);
        }

        [Obsolete]
        /// <summary>
        /// Shoulds the serialize error alert title.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeErrorAlartTitle()
        {
            return HasValue(GridStyleInfoStore.ErrorAlartTitleProperty);
        }

        /// <summary>
        /// Gets or sets the error alert text.
        /// Used to hold the error alert text, while import the excel file to grid.
        /// </summary>
        /// <value>The error alert text.</value>
        public string ErrorAlertText
        {
            get
            {
                return (string)GetValue(GridStyleInfoStore.ErrorAlertTextProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.ErrorAlertTextProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has error alert text.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has error alert text; otherwise, <c>false</c>.
        /// </value>
        public bool HasErrorAlertText
        {
            get
            {
                return HasValue(GridStyleInfoStore.ErrorAlertTextProperty);
            }
        }

        /// <summary>
        /// Resets the error alert text.
        /// </summary>
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public void ResetErrorAlertText()
        {
            ResetValue(GridStyleInfoStore.ErrorAlertTextProperty);
        }

        public bool ShouldSerializeErrorAlertText()
        {
            return HasValue(GridStyleInfoStore.ErrorAlertTextProperty);
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose();
            defaultStyle = null;
        }

        public void Dispose(bool disposing)
        {
            this.Dispose();
            if (disposing)
            {
                this.Store.Clear();
                this.Store.Dispose();
                if (this.CellIdentity != null)
                {
                    this.CellIdentity.Dispose();
                    this.CellIdentity = null;
                }
            }
        }
    }

    /// <summary>
    /// Defines behavior of comobo boxes and drop-down list in a cell.
    /// </summary>
    public enum GridDropDownStyle
    {
        /// <summary>
        /// The user can edit the text box contents and is not limited to values existing choices.
        /// </summary>
        Editable = 0,
        /// <summary>
        /// User input is restricted to items from the ChoiceList or DataSource.
        /// </summary>
        Exclusive = 1,
        /// <summary>
        /// The user input is restricted to items from the ChoiceList or DataSource but
        /// the user can type text into the text box and the text box will be filled
        /// with a matching choice.
        /// </summary>
        AutoComplete = 3,
    }

    /// <summary>
    /// Defines the behaviour of Autocomplete Combo Boxes in a Cell
    /// </summary>

    public enum IncrementalFilter
    {
        /// <summary>
        /// The End User can show Items only related to the entered contents
        /// </summary>
        Enable=1,
        /// <summary>
        /// Default behaviour of Auto-Complete Combo box.
        /// </summary>
        Disable=0
    }

    /// <summary>
    /// Defines the alignment options for text in text image cells.
    /// </summary>
    public enum ImageContentAlignment
    {
        /// <summary>
        /// Image appears before the cell content.
        /// </summary>
        Left = 1,
        /// <summary>
        /// Image appears after the cell content.
        /// </summary>
        Right = 2
    }

    /// <summary>
    /// Defines options for stretching the image in the image cell.
    /// </summary>
    public enum ImageContentStretch
    {
        /// <summary>
        /// Stretches or shrinks the image to fit the cell size.
        /// </summary>
        Fill = 1,
        /// <summary>
        /// Retains the image size.
        /// </summary>
        Uniform = 2,
        /// <summary>
        /// Render the image in specified size(Pixel).
        /// </summary>
        Absolute = 3
    }

    /// <summary>
    /// This enumeration specifies floating cell's behavior in a <see cref="GridModel"/>.
    /// </summary>
    /// <remarks>
    /// See <see cref="GridModelOptions.FloatCellsMode"/>.
    /// </remarks>
    public enum GridFloatCellsMode
    {
        /// <summary>
        /// Floating cell's behavior is disabled.
        /// </summary>
        None = 0,

        /// <summary>
        /// Floating cells are calculated before they are displayed and results are saved. Floating cells will
        /// only be recalculated if the width or contents of cells change.
        /// </summary>
        OnDemandCalculation = 1
    }
	
	public enum ApplyConditionalBasedOn
    {
        CellValue,
        FormulaValue
    }

    [Obsolete]
    public enum ApplyConditionalBasdOn
    {
        CellValue,
        FormulaValue
    }
}
