//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableCellStyleInfo.cs" company="syncfusion">
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
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Design;
using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Grouping;

#if ASPNET
using Syncfusion.Web.UI.WebControls.Grid.Grouping.Design;
using System.Web.UI.Design;
using Syncfusion.Web.Design.UI;
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// The type converter for <see cref="GridTableCellStyleInfo"/> objects. <see cref="GridTableCellStyleInfoConverter"/>
    /// is a <see cref="StyleInfoBaseConverter"/>. It overrides the <see cref="ConvertTo"/> method and returns a string with
    /// descriptive information about the properties that were set in the <see cref="GridTableCellStyleInfo"/> object.
    /// </summary>
    public class GridTableCellStyleInfoConverter: StyleInfoBaseConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridTableCellStyleInfoConverter()
            : base()
        {
        }

        /// <summary>
        /// <para>Converts the given value object to
        /// the specified destination type using the specified context and arguments.</para>
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="culture">Current culture information.</param>
        /// <param name="value">Value to convert.</param>
        /// <param name="destinationType">Target type.</param>
        /// <returns>Converted object.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return value.ToString().TrimEnd('\r', '\n');
        } //// end of method ConvertTo
    }

    /// <summary>
    /// Holds the plain data for a style object excluding identity information.
    /// </summary>
    [Serializable,
    StaticDataField("sd")]
    [DebuggerStepThrough()]
    public class GridTableCellStyleInfoStore : GridStyleInfoStore
    {
        static StaticData sd = new StaticData(typeof(GridTableCellStyleInfoStore), typeof(GridTableCellStyleInfo), false);

        internal static StaticData StaticData
        {
            get
            {
                return sd;
            }
        }

#if ASPNET
        /// <summary>
        /// Provides information about the <see cref="GridStyleInfo.ImageUrl"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ImageUrlProperty = sd.CreateStyleInfoProperty(typeof(string), "ImageUrl");

        /// <summary>
        /// Provides information about the <see cref="GridTableCellStyleInfo.ImageSize"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ImageSizeProperty = sd.CreateStyleInfoProperty(typeof(Size), "ImageSize");

        /// <summary>
        /// Provides information about the <see cref="GridTableCellStyleInfo.CssClass"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CssClassProperty = sd.CreateStyleInfoProperty(typeof(string), "CssClass");
#endif

        /// <overload>
        /// Initializes a <see cref="GridStyleInfoStore"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridStyleInfoStore"/>.
        /// </summary>
        public GridTableCellStyleInfoStore()
        {
            if (sd.IsEmpty)
            {
                new GridTableCellStyleInfo();
            }
        }

        /// <summary>
        /// Initializes a new <see cref="GridStyleInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridTableCellStyleInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);

            if (sd.IsEmpty)
            {
                new GridTableCellStyleInfo();
            }
        }

        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        /// <override/>
        /// <summary>Returns a duplicate of the current object.</summary>
        /// <returns>A duplicate of the current object.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new GridTableCellStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }

    /// <summary>
    /// Lets you control almost any aspect of the appearance of the grouping grid like cell backcolor,
    /// font, or the cell type. Objects of this class are accessible through the <see cref="GridTableCellAppearance"/>
    /// class, e.g. the <see cref="GridTableCellAppearance.AnyCell"/> property.
    /// </summary>
    /// <remarks>
    /// The <see cref="GridTableCellAppearance"/> class stores <see cref="GridTableCellStyleInfo"/>
    /// information for all cell elements in a grouping grid. GridTableCellAppearance has
    /// an inheritance mechanism that allows child elements to inherit default settings from
    /// parent elements. GridTableCellAppearance lets you control almost any aspect of
    /// the appearance of the grouping grid like cell backcolor, font or the cell type.
    /// <para/>
    /// GridTableCellAppearance supports both inheritance of style properties from parent elements and
    /// also inheritance of style settings within cell elements of one appearance object.
    /// <para/>
    /// Inheritance of style properties is defined by the <see cref="GridTableCellStyleInfoIdentity"/>
    /// object. It has a <see cref="GridTableCellStyleInfoIdentity.GetBaseStyles"/> method that returns
    /// the <see cref="GridStyleInfo"/> objects that form a inheritance chain. Check the
    /// <see cref="GridTableCellStyleInfoIdentity.GetBaseStyleNames"/> method to get a string/debug
    /// information about the inheritance chain for a specific element. Also, the designer will show
    /// this debug information about inheritance chain in a tooltip when you hover the mouse over
    /// a cell within the "Preview and Edit" window.
    /// </remarks>
    [TypeConverter(typeof(GridTableCellStyleInfoConverter))]
    public class GridTableCellStyleInfo : GridStyleInfo
    {
        /// <summary>
        /// An empty style object.
        /// </summary>
        public static new readonly GridTableCellStyleInfo Empty = new GridTableCellStyleInfo();
                
         ////Constructors
        static GridTableCellStyleInfo()
        {
#if ASPNET
            // Hide base class properties from PropertyGrid by setting IsBrowsable = false.
            // StyleInfoBaseConverter will check this setting in its GetProperties method.
            // GridTableCellStyleInfoConverter is derived from StyleInfoBaseConverter.
            GridStyleInfoStore.AllowEnterProperty.IsBrowsable = false;
            GridStyleInfoStore.AutoSizeProperty.IsBrowsable = false;
            GridStyleInfoStore.CellAppearanceProperty.IsBrowsable = false;
            GridStyleInfoStore.AutoSizeProperty.IsBrowsable = false;
            GridStyleInfoStore.ClickableProperty.IsBrowsable = false;
            GridStyleInfoStore.ImageIndexProperty.IsBrowsable = false;
            GridStyleInfoStore.ImageListProperty.IsBrowsable = false;
            GridStyleInfoStore.ShowButtonsProperty.IsBrowsable = false;
            GridStyleInfoStore.VerticalScrollbarProperty.IsBrowsable = false;
#endif
        }

        /// <summary>
        /// Initalizes a new style object.
        /// </summary>
        [DebuggerStepThrough()] public GridTableCellStyleInfo()
            : base(new GridTableCellStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new style object and copies all data from an existing style object.
        /// </summary>
        /// <param name="style">The style object that contains the original data.</param>
        [DebuggerStepThrough()] 
        public GridTableCellStyleInfo(GridTableCellStyleInfo style)
            : base(style.Store)
        {
        }

        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="GridStyleInfoStore"/>.
        /// </summary>
        /// <param name="store">A <see cref="GridStyleInfoStore"/> that holds data for this <see cref="GridTableCellStyleInfo"/>.
        /// All changes in this style object will saved in the <see cref="GridStyleInfoStore"/> object.</param>
        [DebuggerStepThrough()] 
        public GridTableCellStyleInfo(GridStyleInfoStore store)
            : base(store)
        {
        }

        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="GridTableCellStyleInfoIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridTableCellStyleInfoIdentity"/> that holds the identity for this <see cref="GridTableCellStyleInfo"/>.
        /// </param>
        [DebuggerStepThrough()] 
        public GridTableCellStyleInfo(StyleInfoIdentityBase identity)
            : base(identity, new GridTableCellStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="GridTableCellStyleInfoIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridTableCellStyleInfoIdentity"/> that holds the identity for this <see cref="GridTableCellStyleInfo"/>.
        /// </param>
        /// <param name="store">A <see cref="GridStyleInfoStore"/> that holds data for this <see cref="GridTableCellStyleInfo"/>.
        /// All changes in this style object will saved in the <see cref="GridStyleInfoStore"/> object.
        /// </param>
        [DebuggerStepThrough()] 
        public GridTableCellStyleInfo(StyleInfoIdentityBase identity, GridStyleInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Checks whether this style object has a GridTableCellStyleInfoIdentity attached.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsTableCell
        {
            get
            {
                return Identity is GridTableCellStyleInfoIdentity;
            }
        }

        /// <summary>
        /// Returns the GridTableCellStyleInfoIdentity object with Identity information about this style object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridTableCellStyleInfoIdentity TableCellIdentity
        {
            get
            {
                GridTableCellStyleInfoIdentity identity = Identity as GridTableCellStyleInfoIdentity;
                return identity;
            }
        }

        /// <override/>
        /// <summary>Specifies the BaseStyle for this style instance with default values for properties that are not initialized for this style object.</summary>
        [Description("The BaseStyle for this style instance with default values for properties that are not initialized for this style object."),
        Browsable(true),
        TypeConverter(typeof(GridTableBaseStyleNameConverter)),
        RefreshProperties(RefreshProperties.Repaint),
        Category("Style")]
        public new string BaseStyle
        {
            [DebuggerStepThrough()]
            get
            {
                return base.BaseStyle;
            }
            
            [DebuggerStepThrough()]
            set
            {
                base.BaseStyle = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sip"></param>
        protected override void OnStyleChanged(StyleInfoProperty sip)
        {
            if (sip == GridStyleInfoStore.AutoSizeProperty && !this.HasAllowEnter && this.TableCellIdentity != null && this.TableCellIdentity.GetEngine() != null && !this.TableCellIdentity.GetEngine().UseOldListChangedHandler)
            {
                if (this.GetValue(sip).Equals(true) && this.GetValue(GridStyleInfoStore.WrapTextProperty).Equals(true))
                {
                    if (this.TableCellIdentity != null && this.TableCellIdentity.TableCellType == GridTableCellType.AnyCell)
                    {
                       GridRangeInfo rowRange = this.TableCellIdentity.Table.GetElementRangeInfo(this.TableCellIdentity.DisplayElement);
                       this.TableCellIdentity.GetEngine().TableModel.RowHeights.ResizeToFit(rowRange,GridResizeToFitOptions.NoShrinkSize | GridResizeToFitOptions.ResizeCoveredCells);
                    }
                }

                else if (this.GetValue(sip).Equals(true) && this.GetValue(GridStyleInfoStore.WrapTextProperty).Equals(false))
                {
                    if (this.TableCellIdentity != null && this.TableCellIdentity.TableCellType == GridTableCellType.AnyCell)
                    {
                        int field = this.TableCellIdentity.Table.TableModel.FieldToColIndex(this.TableCellIdentity.ColIndex);
                        this.TableCellIdentity.GetEngine().TableModel.ColWidths.ResizeToFit(GridRangeInfo.Col(field), GridResizeToFitOptions.NoShrinkSize | GridResizeToFitOptions.ResizeCoveredCells);
                    }
                }
            }
            base.OnStyleChanged(sip);
        }
        
        /// <summary>
        /// Resets <see cref="GridStyleInfo.BaseStyle"/>.
        /// </summary>
        [DebuggerStepThrough()] 
        public new void ResetBaseStyle()
        {
            ResetValue(GridStyleInfoStore.BaseStyleProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBaseStyle()
        {
            return HasValue(GridStyleInfoStore.BaseStyleProperty);
        }

        /// <summary>
        /// Gets the GridTableCellStyleInfoIdentity for a style, checks also the InnerIdentity of the
        /// Identity.
        /// </summary>
        /// <param name="style">The style object.</param>
        /// <returns>The TableCellIdentity if found; NULL otherwise.</returns>
        public static GridTableCellStyleInfoIdentity GetTableCellIdentity(GridStyleInfo style)
        {
            if (style.Identity.InnerIdentity != null)
            {
                return style.Identity.InnerIdentity as GridTableCellStyleInfoIdentity;
            }

            GridTableCellStyleInfo tableStyleInfo = style as GridTableCellStyleInfo;
            if (tableStyleInfo == null)
            {
                return null;
            }

            return tableStyleInfo.TableCellIdentity;
        }
        /// <summary>
        /// 
        /// </summary>
#if SyncfusionFramework4_0
        [Browsable(false), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public GroupingGridCellUIAProvider GroupingGridProvider
        {
            get
            {
                return new GroupingGridCellUIAProvider(this.GetActiveGridView() as Control, this);
            }
        }
#elif SyncfusionFramework3_5
        [Browsable(false), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public GroupingGridCellUIAProvider GroupingGridProvider
        {
            get
            {
                return new GroupingGridCellUIAProvider(this.GetActiveGridView() as Control, this);
            }
        }

#endif
        /// <override/>
        /// <summary>Contains cell type information of a cell.</summary>
        [Description("Contains cell type information of a cell."),
        Browsable(true),
        TypeConverter(typeof(GridTableCellTypeNameConverter)),
        RefreshProperties(RefreshProperties.Repaint)]
        [NotifyParentProperty(true)]
        public new string CellType
        {
            get
            {
                return base.CellType;
            }
           
            set
            {
                base.CellType = value;
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.CellType"/>.
        /// </summary>
        [DebuggerStepThrough()] 
        public new void ResetCellType()
        {
            ResetValue(GridTableCellStyleInfoStore.CellTypeProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCellType()
        {
            return HasValue(GridTableCellStyleInfoStore.CellTypeProperty);
        }

#if ASPNET
        public static new GridTableCellStyleInfo Default
        {
            get
            {
                GridTableCellStyleInfo style = new GridTableCellStyleInfo();

                style.ModifyStyle(GridStyleInfo.Default,StyleModifyType.Copy);
                style.ImageSize = Size.Empty;
                style.CssClass = "";
                style.ImageUrl = "";

                return style;
            }
        }
        /// <override/>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }

        #region CssClass
        /// <summary>
        /// Specifies a custom css style that will define the appearance of an element.
        /// </summary>
        /// <remarks>Specifying a custom style will preclude all other style information specified in this instance. 
        /// The css style can be specified in an external .css file, for example.</remarks>
        [
        Description("Specifies a custom css style that will define the appearance of an element."),
        Browsable(true),
        RefreshProperties(RefreshProperties.Repaint),
        Category("Appearance"),
        ]
        [NotifyParentProperty(true)]
        public string CssClass
        {
            [DebuggerStepThrough()]
            get
            {
                return (string) GetValue(GridTableCellStyleInfoStore.CssClassProperty);
            }[DebuggerStepThrough()]
            set
            {
                SetValue(GridTableCellStyleInfoStore.CssClassProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.CssClass"/>.
        /// </summary>
        [DebuggerStepThrough() ] public void ResetCssClass()
        {
            ResetValue(GridTableCellStyleInfoStore.CssClassProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCssClass()
        {
            return HasValue(GridTableCellStyleInfoStore.CssClassProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableCellStyleInfo.CssClass"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCssClass
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableCellStyleInfoStore.CssClassProperty);
            }
        }
        #endregion

        #region ImageSize
        [
        Description("Specifies the dimensions for the image in a image cell."),
        Browsable(true),
        RefreshProperties(RefreshProperties.Repaint),
        Category("Image"),
        ]
        [NotifyParentProperty(true)]
        public Size ImageSize
        {
            [DebuggerStepThrough()]
            get
            {
                return (Size) GetValue(GridTableCellStyleInfoStore.ImageSizeProperty);
            }[DebuggerStepThrough()]
            set
            {
                SetValue(GridTableCellStyleInfoStore.ImageSizeProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridStyleInfo.ImageSize"/>.
        /// </summary>
        [DebuggerStepThrough() ] public void ResetImageSize()
        {
            ResetValue(GridTableCellStyleInfoStore.ImageSizeProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeImageSize()
        {
            return HasValue(GridTableCellStyleInfoStore.ImageSizeProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableCellStyleInfo.ImageSize"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasImageSize
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableCellStyleInfoStore.ImageSizeProperty);
            }
        }
        #endregion
        #region ImageUrl
        [
        Description("The url of an image file that will be displayed within the cell."),
        Browsable(true),
        RefreshProperties(RefreshProperties.Repaint),
        Category("Image")
        ]
        [NotifyParentProperty(true)]
        public string ImageUrl
        {
            [DebuggerStepThrough()]
            get
            {
                return (string) GetValue(GridTableCellStyleInfoStore.ImageUrlProperty);
            }[DebuggerStepThrough()]
            set
            {
                SetValue(GridTableCellStyleInfoStore.ImageUrlProperty, value);
            }
        }
        /// <summary>
        /// Resets <see cref="GridStyleInfo.ImageUrl"/>.
        /// </summary>
        [DebuggerStepThrough() ] public void ResetImageUrl()
        {
            ResetValue(GridTableCellStyleInfoStore.ImageUrlProperty);
        }
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeImageUrl()
        {
            return HasValue(GridTableCellStyleInfoStore.ImageUrlProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridTableCellStyleInfo.ImageUrl"/> has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasImageUrl
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridTableCellStyleInfoStore.ImageUrlProperty);
            }
        }
        #endregion
#endif
    }
}

