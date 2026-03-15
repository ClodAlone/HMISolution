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
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#if !WinRT
using System.Windows.Controls;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.Styles;
using System.Windows.Data;
using Syncfusion.Windows.Controls.Scroll;
namespace Syncfusion.Windows.Controls.Grid

#else
using Syncfusion.WinRT.Controls.Cells;
using Syncfusion.WinRT.Controls.Scroll;
using Syncfusion.WinRT.GridCommon;
using Syncfusion.WinRT.Styles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridStyleInfo : StyleInfoBase, IRenderCellInfo
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

        public int RowIndex
        {
            get
            {
                return CellRowColumnIndex.RowIndex;
            }
        }

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
        public static GridStyleInfo Default
        {
            get
            {
                if (GridStyleInfo.defaultStyle == null)
                {
                    defaultStyle = new GridStyleInfo();
#if!WinRT
                    defaultStyle.Background = null;
#else
                    defaultStyle.Background = null;
#endif
                    defaultStyle.BaseStyle = "";
                    defaultStyle.BorderMargins = CellMarginsInfo.Empty;
                    defaultStyle.TextMargins = CellMarginsInfo.Empty;
                    defaultStyle.Borders = CellBordersInfo.Default;
                    defaultStyle.Font = GridFontInfo.Default;
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
                    defaultStyle.FloatCellsMode = GridFloatCellsMode.None;
                    defaultStyle.FloodCell = true;
                    defaultStyle.ParseFormats = null;
#if !WinRT
                    defaultStyle.PropertyDescriptor = null;
#endif
                    defaultStyle.ReadOnly = false;
                    defaultStyle.StrictValueType = false;
                    defaultStyle.ItemsSource = null;
                    defaultStyle.DisplayMember = "";
                    defaultStyle.ValueMember = "";
                    defaultStyle.ChoiceList = null;
                    defaultStyle.IsEditable = false;
                    defaultStyle.Tag = null;
#if WinRT
                    defaultStyle.Foreground = new SolidColorBrush(Colors.White);
#else
                    defaultStyle.Foreground = new SolidColorBrush(Colors.Black);// SystemColors.ControlTextBrush;
#endif
                    defaultStyle.NegativeForeground = new SolidColorBrush(Colors.Red);
                    defaultStyle.AcceptsReturn = true;
                    defaultStyle.AutoWordSelection = true;
                    defaultStyle.AllowRowResize = true;
                    //defaultStyle.CharacterCasing = CharacterCasing.Normal;
                    defaultStyle.ToolTip = null;
                    //defaultStyle.TextAlignment = TextAlignment.Left;
                    defaultStyle.HorizontalAlignment = HorizontalAlignment.Left;
                    defaultStyle.TextWrapping = TextWrapping.Wrap;
                    defaultStyle.TextTrimming = TextTrimming.WordEllipsis;
                    //defaultStyle.TextTrimming = TextTrimming.CharacterEllipsis;
                    //defaultStyle.FlowDirection = FlowDirection.LeftToRight;
                    defaultStyle.MaxLength = 0;
                    defaultStyle.VerticalAlignment = VerticalAlignment.Top;
                    defaultStyle.ShowDataValidationTooltip = false;
                    defaultStyle.DataValidationTooltip = string.Empty;
                    defaultStyle.ImageContentAlignment = ImageContentAlignment.Left;
                    defaultStyle.ImageWidth = new GridLength(0.2d, GridUnitType.Star);
                    defaultStyle.ImageHeight = new GridLength(1d, GridUnitType.Star);
                    defaultStyle.ImageMargins = new CellMarginsInfo(0, 0, 0, 0);
                    defaultStyle.ImageContentStretch = ImageContentStretch.Fill;
#if !WinRT
                    defaultStyle.CommentAlignment = CommentAlignment.TopRight;
#endif
                    defaultStyle.IsThreeState = false;
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
        public bool HasBackground
        {

            get
            {
                return HasValue(GridStyleInfoStore.BackgroundProperty);
            }
        }
        #endregion
        #region Foreground
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
        public bool HasAcceptsReturn
        {
            get
            {
                return HasValue(GridStyleInfoStore.AcceptsReturnProperty);
            }
        }
        #endregion
        #region AutoWordSelection
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
        public bool HasAutoWordSelection
        {
            get
            {
                return HasValue(GridStyleInfoStore.AutoWordSelectionProperty);
            }
        }
        #endregion

        #region AllowRowResize
        public bool AllowRowResize
        {
            get
            {
                return GetShortValue(GridStyleInfoStore.AllowRowResizeProperty) != 0;
            }
            set
            {
                SetValue(GridStyleInfoStore.AllowRowResizeProperty, value ? 1 : 0);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.AllowRowResize"/>.
        /// </summary>
        public void ResetAllowRowResize()
        {
            ResetValue(GridStyleInfoStore.AllowRowResizeProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeAllowRowResize()
        {
            return HasValue(GridStyleInfoStore.AllowRowResizeProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridStyleInfo.AllowRowResize"/> has been initialized for the current object.
        /// </summary>
        public bool HasAllowRowResize
        {
            get
            {
                return HasValue(GridStyleInfoStore.AllowRowResizeProperty);
            }
        }
        #endregion

        #region HorizontalAlignment
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
        public bool HasHorizontalAlignment
        {
            get
            {
                return HasValue(GridStyleInfoStore.HorizontalAlignmentProperty);
            }
        }
        #endregion
        #region TextWrapping
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
        public bool HasTextWrapping
        {
            get
            {
                return HasValue(GridStyleInfoStore.TextWrappingProperty);
            }
        }
        #endregion

        #region TextTrimming
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
        public bool HasTextTrimming
        {
            get
            {
                return HasValue(GridStyleInfoStore.TextTrimmingProperty);
            }
        }
        #endregion

        #region MaxLength
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
        public bool HasMaxLength
        {
            get
            {
                return HasValue(GridStyleInfoStore.MaxLengthProperty);
            }
        }
        #endregion
        #region VerticalAlignment
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
        public bool HasVerticalAlignment
        {
            get
            {
                return HasValue(GridStyleInfoStore.VerticalAlignmentProperty);
            }
        }
        #endregion
        #region NegativeForeground
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

        public void ResetNegativeForeground()
        {
            this.ResetValue(GridStyleInfoStore.NegativeForegroundProperty);
        }

        private bool ShouldSerializeNegativeForeground()
        {
            return this.HasValue(GridStyleInfoStore.NegativeForegroundProperty);
        }

        public bool HasNegativeForeground
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.NegativeForegroundProperty);
            }
        }

        #endregion

        #region NumberFormat
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
        public void ResetNumberFormat()
        {
            this.ResetValue(GridStyleInfoStore.NumberFormatProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeNumberFormat()
        {
            return this.HasValue(GridStyleInfoStore.NumberFormatProperty);
        }

        public bool HasNumberFormat
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.NumberFormatProperty);
            }
        }
        #endregion
        #region TextTrimming
        //public TextTrimming TextTrimming
        //{
        //    get
        //    {
        //        return (TextTrimming)GetValue(GridStyleInfoStore.TextTrimmingProperty);
        //    }
        //    set
        //    {
        //        SetValue(GridStyleInfoStore.TextTrimmingProperty, value);
        //    }
        //}
        ///// <summary>
        ///// Resets <see cref="GridStyleInfo.TextTrimming"/>.
        ///// </summary>
        //public void ResetTextTrimming()
        //{
        //    ResetValue(GridStyleInfoStore.TextTrimmingProperty);
        //}
        //[EditorBrowsableAttribute(EditorBrowsableState.Never)]
        //private bool ShouldSerializeTextTrimming()
        //{
        //    return HasValue(GridStyleInfoStore.TextTrimmingProperty);
        //}

        ///// <summary>
        ///// Determines if <see cref="GridStyleInfo.TextTrimming"/> has been initialized for the current object.
        ///// </summary>
        //public bool HasTextTrimming
        //{
        //    get
        //    {
        //        return HasValue(GridStyleInfoStore.TextTrimmingProperty);
        //    }
        //}
        #endregion
        #region BaseStyle
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
        /// The property affects the behavior or appearance of call cell types:<para/>
        /// </remarks>
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
        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
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
        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
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
#if !WinRT
        [
        Description(""),
        Browsable(true),
        Category("")
        ]
#endif
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
#if !WinRT
        [Browsable(false)]
#endif
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
#if !WinRT
        [Browsable(false)]
#endif
        public bool HasPadding
        {

            get
            {
                return HasValue(GridStyleInfoStore.PaddingProperty);
            }
        }
        #endregion
        #region Borders
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
        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        [System.Xml.Serialization.XmlIgnore]
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
        public bool HasBorders
        {

            get
            {
                return HasValue(GridStyleInfoStore.BordersProperty);
            }
        }
        #endregion
        #region Font
        /// <internalonly/>
        [EditorBrowsableAttribute(EditorBrowsableState.Advanced)]
        [System.Xml.Serialization.XmlIgnore]
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
        public bool HasFont
        {
            get
            {
                return HasValue(GridStyleInfoStore.FontProperty);
            }
        }
        #endregion
        #region CellItemTemplate
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
#if (SILVERLIGHT || WinRT)

        public bool EnableFloatCell
        {
            get
            {
                return (bool)this.GetValue(GridStyleInfoStore.EnableFloatingCellProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.EnableFloatingCellProperty, value);
            }
        }

        public GridFloatCellsMode FloatCellsMode
        {
            get
            {
                return (GridFloatCellsMode)this.GetValue(GridStyleInfoStore.FloatCellModeProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.FloatCellModeProperty, value);
            }
        }

        public bool FloodCell
        {
            get
            {
                return (bool)this.GetValue(GridStyleInfoStore.FloodCellProperty);
            }
            set
            {
                this.SetValue(GridStyleInfoStore.FloodCellProperty, value);
            }
        }
#endif
        #region CellType
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
        public bool HasCellType
        {
            get
            {
                return HasValue(GridStyleInfoStore.CellTypeProperty);
            }
        }
        #endregion
        #region CellValue
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
        public bool HasCellValue
        {
            get
            {
                return HasValue(GridStyleInfoStore.CellValueProperty);
            }
        }
        #endregion
        #region CellValue2
        /// <summary>
        /// Gets or sets the cell value 2 information of the cell.
        /// </summary>
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
        public bool HasCellValue2
        {
            get
            {
                return HasValue(GridStyleInfoStore.CellValue2Property);
            }
        }
        #endregion

        #region CellValueType
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
        public bool HasCellValueType
        {
            get
            {
                return HasValue(GridStyleInfoStore.CellValueTypeProperty);
            }
        }
        #endregion
        #region CultureInfo
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
                culture = CultureInfo.CurrentCulture;
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
        public bool HasCultureInfo
        {
            get
            {
                return HasValue(GridStyleInfoStore.CultureInfoProperty);
            }
        }
        #endregion
        #region Description
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
        public bool HasDescription
        {
            get
            {
                return HasValue(GridStyleInfoStore.DescriptionProperty);
            }
        }
        #endregion
        #region Enabled
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
        public bool HasEnabled
        {
            get
            {
                return HasValue(GridStyleInfoStore.EnabledProperty);
            }
        }
        #endregion

        #region Error
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
        public bool HasException
        {
            get
            {
                return HasValue(GridStyleInfoStore.ExceptionProperty);
            }
        }
        #endregion
        #region Format
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
        public bool HasParseFormats
        {
            get
            {
                return HasValue(GridStyleInfoStore.ParseFormatsProperty);
            }
        }
        #endregion
        #region ReadOnly
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
        public bool HasReadOnly
        {
            get
            {
                return HasValue(GridStyleInfoStore.ReadOnlyProperty);
            }
        }
        #endregion
        #region StrictValueType
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
        public bool HasDisplayMember
        {
            get
            {
                return HasValue(GridStyleInfoStore.DisplayMemberProperty);
            }
        }
        #endregion
        #region ValueMember
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
        public bool HasValueMember
        {
            get
            {
                return HasValue(GridStyleInfoStore.ValueMemberProperty);
            }
        }
        #endregion
        #region ItemsSource

        [CloneableProperty(false), DisposeableProperty(false)] // REVIEW:
        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(GridStyleInfoStore.ItemsSourceProperty);
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
        public bool HasItemsSource
        {
            get
            {
                return HasValue(GridStyleInfoStore.ItemsSourceProperty);
            }
        }
        #endregion
        #region ChoiceList

        public List<string> ChoiceList
        {
            get
            {
                List<string> c = (List<string>)GetValue(GridStyleInfoStore.ChoiceListProperty);
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
        public bool HasChoiceList
        {
            get
            {
                return HasValue(GridStyleInfoStore.ChoiceListProperty);
            }
        }
        #endregion
        #region IsEditable

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
        public bool HasIsEditable
        {
            get
            {
                return HasValue(GridStyleInfoStore.IsEditableProperty);
            }
        }
        #endregion
#if !WinRT
        #region PropertyDescriptor
        /// <summary>
        /// Specifies a property descriptor that can be used by UITypeEditCell, PropertyGridCell, and StandardValuesCell cell types.
        /// </summary>
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
        public bool HasPropertyDescriptor
        {
            get
            {
                return HasValue(GridStyleInfoStore.PropertyDescriptorProperty);
            }
        }
        #endregion
#endif
        // Text, Formatted Text
        #region Formatted Text
        /// <summary>
        /// Returns a formatted text for the default value for a specified <see cref="CellValueType"/>.
        /// </summary>
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
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    throw;

                return value != null ? value.ToString() : "";
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
        internal string GetFormulaValue(string formula, GridFormulaTag tag)
        {
            GridCellModelBase cellModel = this.CellModel;
            if (cellModel == null)
            {
                return (string)ValueConvert.ChangeType(formula, typeof(string), CultureInfo.CurrentCulture);
            }
            return cellModel.GetFormulaValue(this, formula, tag);
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

        // CellModel determined through CellType
        #region CellModel

        /// <summary>
        /// Returns the associated <see cref="GridCellModelBase"/> for this style object.
        /// </summary>
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

            //if (Background is SolidColorBrush && otherStyle.Background is SolidColorBrush && (Background as SolidColorBrush).Color == (otherStyle.Background as SolidColorBrush).Color)
            //{
            //    if ((otherStyle.Borders.Bottom != null && otherStyle.Borders.Bottom.Style == BorderStyle.None) || (otherStyle.Borders.Right != null && otherStyle.Borders.Right.Style == BorderStyle.None) ||
            //        (otherStyle.Borders.Left != null && otherStyle.Borders.Left.Style == BorderStyle.None) || (otherStyle.Borders.Top != null && otherStyle.Borders.Top.Style == BorderStyle.None))
            //    {
            //        return true;
            //    }
            //}
#if!WinRT
            Thickness t = Padding.ToThickness(otherStyle.Padding);
            if (t.Right + t.Left + t.Bottom + t.Top > 0)
                return false;

            return Background is SolidColorBrush && Background.Equals(other.GetCellBackground());
#endif
            if (Background is SolidColorBrush)
            {
                SolidColorBrush cellBackground = Background as SolidColorBrush;
                SolidColorBrush otherCellBackground = other.GetCellBackground() as SolidColorBrush;
                if (otherCellBackground != null && cellBackground.Color.Equals(otherCellBackground.Color))
                {
                    return true;
                }
            }
            return false;
        }

        object IRenderCellInfo.GetCellBorder(CellBorderSide side)
        {
            return Borders[side];
        }

        bool IRenderCellInfo.CanCombineCellBorder(CellBorderSide side, IRenderCellInfo other)
        {
            GridStyleInfo otherStyle = other as GridStyleInfo;
            if (otherStyle == null)
                return false;

            return Borders[side] == other.GetCellBorder(side);
        }

        Thickness IRenderCellInfo.GetBorderMargins()
        {
            return BorderMargins.ToThickness();
        }

        Thickness IRenderCellInfo.GetPadding()
        {
            return Padding.ToThickness();
        }

        //ICellRenderer IRenderCellInfo.GetCellRenderer()
        //{
        //    return CellRenderer;
        //}

        #endregion
        #region FormulaTag
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
        public bool HasFormulaTag
        {
            get
            {
                return HasValue(GridStyleInfoStore.FormulaTagProperty);
            }
        }
        #endregion
        #region Tag

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
        public bool HasTag
        {
            get
            {
                return HasValue(GridStyleInfoStore.TagProperty);
            }
        }
        #endregion
        #region DropDownStyle
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

        public void ResetDropDownStyle()
        {
            ResetValue(GridStyleInfoStore.AutoCompleteProperty);
            ResetValue(GridStyleInfoStore.ExclusiveChoiceListProperty);
        }

        private bool ShouldSerializeDropDownStyle()
        {
            return HasValue(GridStyleInfoStore.AutoCompleteProperty) || HasValue(GridStyleInfoStore.ExclusiveChoiceListProperty);
        }

        public bool HasDropDownStyle
        {
            get
            {
                return HasValue(GridStyleInfoStore.AutoCompleteProperty) || HasValue(GridStyleInfoStore.ExclusiveChoiceListProperty);
            }
        }
        #endregion
        #region UpDownEdit
        //public GridUpDownEditStyleInfo UpDownEdit
        //{
        //    get
        //    {
        //        return (GridUpDownEditStyleInfo)this.GetValue(GridStyleInfoStore.UpDownEditInfoProperty);
        //    }

        //    set
        //    {
        //        this.SetValue(GridStyleInfoStore.UpDownEditInfoProperty, value);
        //    }
        //}

        //public void ResetUpDownEdit()
        //{
        //    this.ResetValue(GridStyleInfoStore.UpDownEditInfoProperty);
        //}

        //public bool ShouldSerializeUpDownEdit()
        //{
        //    return this.HasValue(GridStyleInfoStore.UpDownEditInfoProperty);
        //}

        //public bool HasUpDownEdit
        //{
        //    get
        //    {
        //        return this.HasValue(GridStyleInfoStore.UpDownEditInfoProperty);
        //    }
        //}

        #endregion
        #region StaysOpenOnEdit
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

        public void ResetStaysOpenOnEdit()
        {
            this.ResetValue(GridStyleInfoStore.StaysOpenOnEditProperty);
        }

        private bool ShouldSerializeStaysOpenOnEdit()
        {
            return this.HasValue(GridStyleInfoStore.StaysOpenOnEditProperty);
        }

        public bool HasStaysOpenOnEdit
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.StaysOpenOnEditProperty);
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
#if !WinRT
        [Description(""),Browsable(true)]
#endif
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
        public bool HasToolTip
        {
            get
            {
                return HasValue(GridStyleInfoStore.ToolTipProperty);
            }
        }
        #endregion

        #region Comment Servivce

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

        public void ResetCommentTemplateKey()
        {
            this.ResetValue(GridStyleInfoStore.CommentTemplateKeyProperty);
        }

        private bool ShouldSerializeCommentTemplateKey()
        {
            return this.HasValue(GridStyleInfoStore.CommentTemplateKeyProperty);
        }

        public bool HasCommentTemplateKey
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.CommentTemplateKeyProperty);
            }
        }
#if !WinRT
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

        public void ResetCommentAlignment()
        {
            this.ResetValue(GridStyleInfoStore.CommentAlignmentProperty);
        }

        public bool HasCommentAlignment
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.CommentAlignmentProperty);
            }
        }

        private bool ShouldSerializeCommentAlignment()
        {
            return this.HasValue(GridStyleInfoStore.CommentAlignmentProperty);
        }
#endif
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

        public void ResetGridCommentStyleInfo()
        {
            this.ResetValue(GridStyleInfoStore.GridCommentStyleInfoProperty);
        }

        private bool ShouldSerializeGridCommentStyleInfo()
        {
            return this.HasValue(GridStyleInfoStore.GridCommentStyleInfoProperty);
        }

        public void ResetComment()
        {
            this.ResetValue(GridStyleInfoStore.CommentProperty);
        }

        public bool HasComment
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.CommentProperty);
            }
        }

        private bool ShouldSerializeComment()
        {
            return this.HasValue(GridStyleInfoStore.CommentProperty);
        }

        #endregion

        #region IsThreeState
        public bool IsThreeState
        {
            get
            {
                return (bool)this.GetValue(GridStyleInfoStore.IsThreeStateStyleInfoProperty);
            }
            set
            {
                this.SetValue(GridStyleInfoStore.IsThreeStateStyleInfoProperty, value);
            }
        }

        public void ResetIsThreeState()
        {
            this.ResetValue(GridStyleInfoStore.IsThreeStateStyleInfoProperty);
        }

        private bool ShouldSerializeIsThreeState()
        {
            return this.HasValue(GridStyleInfoStore.IsThreeStateStyleInfoProperty);
        }

        public bool HasIsThreeState
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.IsThreeStateStyleInfoProperty);
            }
        }

        #endregion

        public GridImageCellStyleInfo ImageCell
        {
            get { return (GridImageCellStyleInfo)this.GetValue(GridStyleInfoStore.ImageCellStyleInfoProperty); }
            set { this.SetValue(GridStyleInfoStore.ImageCellStyleInfoProperty, value); }
        }
#if !WinRT
        #region CurrencyEdit
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

        

        public void ResetCurrencyEdit()
        {
            this.ResetValue(GridStyleInfoStore.CurrencyEditStyleInfoProperty);
        }

        private bool ShouldSerializeCurrencyEdit()
        {
            return this.HasValue(GridStyleInfoStore.CurrencyEditStyleInfoProperty);
        }

        public bool HasCurrencyEdit
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.CurrencyEditStyleInfoProperty);
            }
        }

        #endregion

        #region DoubleEdit


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

        public void ResetDoubleEdit()
        {
            this.ResetValue(GridStyleInfoStore.DoubleEditStyleInfoProperty);
        }

        private bool ShouldSerializeDoubleEdit()
        {
            return this.HasValue(GridStyleInfoStore.DoubleEditStyleInfoProperty);
        }

        public bool HasDoubleEdit
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.DoubleEditStyleInfoProperty);
            }
        }

        #endregion

        #region DateTimeEdit
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

        public void ResetDateTimeEdit()
        {
            this.ResetValue(GridStyleInfoStore.DateTimeEditStyleInfoProperty);
        }

        private bool ShouldSerializeDateTimeEdit()
        {
            return this.HasValue(GridStyleInfoStore.DateTimeEditStyleInfoProperty);
        }

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

        #region IntEdit


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

        public void ResetIntegerEdit()
        {
            this.ResetValue(GridStyleInfoStore.IntegerEditStyleInfoProperty);
        }

        private bool ShouldSerializeIntegerEdit()
        {
            return this.HasValue(GridStyleInfoStore.IntegerEditStyleInfoProperty);
        }

        public bool HasIntegerEdit
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.IntegerEditStyleInfoProperty);
            }
        }

        #endregion

        #region MaskEdit
        public GridMaskEditStyleInfo MaskEdit
        {
            get
            {
                return (GridMaskEditStyleInfo)this.GetValue(GridStyleInfoStore.MaskEditInfoProperty);
            }

            set
            {
                this.SetValue(GridStyleInfoStore.MaskEditInfoProperty, value);
            }
        }

        public void ResetMaskEdit()
        {
            this.ResetValue(GridStyleInfoStore.MaskEditInfoProperty);
        }

        public bool HasMaskEdit
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.MaskEditInfoProperty);
            }
        }

        private bool ShouldSerializeMaskEdit()
        {
            return this.HasValue(GridStyleInfoStore.MaskEditInfoProperty);
        }

        #endregion

        #region PercentEdit

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

        public void ResetPercentEdit()
        {
            this.ResetValue(GridStyleInfoStore.PercentEditStyleInfoProperty);
        }

        private bool ShouldSerializePercentEdit()
        {
            return this.HasValue(GridStyleInfoStore.PercentEditStyleInfoProperty);
        }

        public bool HasPercentEdit
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.PercentEditStyleInfoProperty);
            }
        }

        #endregion

        #region NumericUpDownEdit

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

        public void ResetUpDownEdit()
        {
            this.ResetValue(GridStyleInfoStore.UpDownEditInfoProperty);
        }

        public bool HasUpDownEdit
        {
            get
            {
                return this.HasValue(GridStyleInfoStore.UpDownEditInfoProperty);
            }
        }

        private bool ShouldSerializeUpDownEdit()
        {
            return this.HasValue(GridStyleInfoStore.UpDownEditInfoProperty);
        }

        #endregion
#endif
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
                var adjustableWidth = imageContentStretch == ImageContentStretch.Fill ? this.GetImageWidth() * clientSize.Width : this.GetImageWidth() * this.GridModel.ColumnWidths.GetDefaultLineSize();
                var width = adjustableWidth + 5d;
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
            return this.AdjustImageWidthAndHeightToMargin(defaultMargin, new Size(rect.Width, rect.Height));

        }

        /// <summary>
        /// Returns the image width.
        /// </summary>
        /// <returns>Width of the image.</returns>
        public double GetImageWidth()
        {
            var value = 0d;
            if (this.ReadOnlyImageWidth.IsStar)
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
            if (this.ReadOnlyImageHeight.IsStar)
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

        [Obsolete]
        public ApplyConditionalBasdOn ApplyConditionalFormatBasdOn
        {
            get
            {
                var applyTo = GetValue(GridStyleInfoStore.ApplyConditionalFormatBasdOnProperty);
                if (applyTo == null)
                    return ApplyConditionalBasdOn.CellValue;
                return (ApplyConditionalBasdOn)applyTo;
            }
            set
            {
                SetValue(GridStyleInfoStore.ApplyConditionalFormatBasdOnProperty, value);
            }
        }

        // private bool FormulaConditionalFormat = true;

        #endregion
		#region DataValidation Tooltip

        /// <summary>
        /// Gets or sets the error alart title.
        /// Used to hold the error alert tile, while import the excel file to grid.
        /// </summary>
        /// <value>The error alart title.</value>
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
        /// Gets or sets the error alart title.
        /// Used to hold the error alert tile, while import the excel file to grid.
        /// </summary>
        /// <value>The error alart title.</value>
        public string ErrorAlartTitle
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

        /// <summary>
        /// Gets a value indicating whether this instance has error alart title.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has error alart title; otherwise, <c>false</c>.
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
        /// Gets a value indicating whether this instance has error alart title.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has error alart title; otherwise, <c>false</c>.
        /// </value>
        public bool HasErrorAlartTitle
        {
            get
            {
                return HasValue(GridStyleInfoStore.ErrorAlertTitleProperty);
            }
        }

        /// <summary>
        /// Resets the error alart title.
        /// </summary>
        public void ResetErrorAlertTitle()
        {
            ResetValue(GridStyleInfoStore.ErrorAlertTitleProperty);
        }

        [Obsolete]
        /// <summary>
        /// Resets the error alart title.
        /// </summary>
        public void ResetErrorAlartTitle()
        {
            ResetValue(GridStyleInfoStore.ErrorAlertTitleProperty);
        }


        /// <summary>
        /// Shoulds the serialize error alart title.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeErrorAlertTitle()
        {
            return HasValue(GridStyleInfoStore.ErrorAlertTitleProperty);
        }

        [Obsolete]
        /// <summary>
        /// Shoulds the serialize error alart title.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeErrorAlartTitle()
        {
            return HasValue(GridStyleInfoStore.ErrorAlertTitleProperty);
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
                return (string)GetValue(GridStyleInfoStore.ErrorAlertProperty);
            }
            set
            {
                SetValue(GridStyleInfoStore.ErrorAlertProperty, value);
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
                return HasValue(GridStyleInfoStore.ErrorAlertProperty);
            }
        }

        /// <summary>
        /// Resets the error alert text.
        /// </summary>
        public void ResetErrorAlertText()
        {
            ResetValue(GridStyleInfoStore.ErrorAlertProperty);
        }

        /// <summary>
        /// Shoulds the serialize error alert text.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeErrorAlertText()
        {
            return HasValue(GridStyleInfoStore.ErrorAlertProperty);
        }

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
        /// Gets or sets a value indicating whether show data validation tooltip.
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

    }

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCommentStyleInfo
    {
        public GridCommentStyleInfo()
        {
        }

        public string BottomLeftComment
        {
            set;
            get;
        }

        public string BottomRightComment
        {
            set;
            get;
        }

        public string TopLeftComment
        {
            set;
            get;
        }

        public string TopRightComment
        {
            set;
            get;
        }

        public string BottomLeftCommentTemplateKey
        {
            get;
            set;
        }

        public string BottomRightCommentTemplateKey
        {
            get;
            set;
        }

        public string TopLeftCommentTemplateKey
        {
            get;
            set;
        }

        public string TopRightCommentTemplateKey
        {
            get;
            set;
        }
    }

    // Summary:
    //     Defines behavior of comobo boxes and drop-down list in a cell.
    public enum GridDropDownStyle
    {
        // Summary:
        //     The user can edit the text box contents and is not limited to values existing
        //     choices.
        Editable = 0,
        //
        // Summary:
        //     User input is restricted to items from the ChoiceList or DataSource.
        Exclusive = 1,
        //
        // Summary:
        //     The user input is restricted to items from the ChoiceList or DataSource but
        //     the user can type text into the text box and the text box will be filled
        //     with a matching choice.
        AutoComplete = 3,
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
        Uniform = 2
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
