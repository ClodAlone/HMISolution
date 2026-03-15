//-------------------------------------------------------------------------------------------------
// <copyright file="GridGroupOptionsStyleInfo.cs" company="syncfusion">
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
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

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
using Syncfusion.Web.Design.UI;
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// The type converter for <see cref="GridGroupOptionsStyleInfo"/> objects. <see cref="GridGroupOptionsStyleInfoConverter"/>
    /// is a <see cref="StyleInfoBaseConverter"/>. It overrides the <see cref="ConvertTo"/> method and returns a string with
    /// descriptive information about the properties that were set in the <see cref="GridGroupOptionsStyleInfo"/> object.
    /// </summary>
    public class GridGroupOptionsStyleInfoConverter: StyleInfoBaseConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridGroupOptionsStyleInfoConverter()
            : base()
        {
        }

        /// <summary>
        ///    <para>Converts the given value object to
        ///       the specified destination type using the specified context and arguments.</para>
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="culture">Current culture information.</param>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert to.</param>
        /// <returns>Converted object.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return value.ToString().TrimEnd('\r', '\n');
        } // end of method ConvertTo
    }

    /// <summary>
    /// Properties in this class let you control the look and behavior of the top level group, child groups and nested child tables.
    /// You can control the caption text, where and if AddNewRow will be displayed, or whether captions, footers, previews and summaries are displayed.
    /// </summary>
    /// <remarks>
    /// GridGroupOptionsStyleInfo supports inheritance properties from parent elements.
    /// <para/>
    /// Examples for inheritance of properties from parent elements are:<para/>
    /// <list type="bullet">
    /// <item><term>Group inherits from GridColumnDescriptor.GroupByOptions.</term></item>
    /// <item><term>GridColumnDescriptor.GroupByOptions inherits from GridTableDescriptor.GroupOptions.</term></item>
    /// <item><term>GridTableDescriptor.GroupOptions inherits from GridGroupingControl.GroupOptions</term></item>
    /// </list>
    /// <para/>
    /// A <see cref="GridGroupingControl"/> distinguishes between three different kinds of group options:
    /// <list type="bullet">
    /// <item><term>TopLevelGroup: lets you control the look and behavior of the top level group.</term></item>
    /// <item><term>ChildGroupOptions: lets you control the look and behavior of the child groups.</term></item>
    /// <item><term>NestedTableGroupOptions: lets you control the look and behavior of the nested child relations</term></item>
    /// </list>
    /// </remarks>
    [TypeConverter(typeof(GridGroupOptionsStyleInfoConverter))]
    public class GridGroupOptionsStyleInfo : StyleInfoBase ////, IXmlSerializable
    {
    /*    #region IXmlSerializable Members

        /// <summary>
        /// Serializes the contents of this object into a Xml stream.
        /// </summary>
        /// <param name="writer">Represents the xml stream.</param>
        public new void WriteXml(XmlWriter writer)
        {
            Store.WriteXml(writer);
        }

        /// <summary>
        /// Not implemented and returns null.
        /// </summary>
        /// <returns></returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            // TODO:  Add GetSchema implementation
            return null;
        }

        /// <summary>
        /// Deserializes the contents of this object from a Xml stream.
        /// </summary>
        /// <param name="reader">Represents the xml stream.</param>
        public new void ReadXml(XmlReader reader)
        {
            Store.ReadXml(reader);
        }

        #endregion*/

        /// <summary>
        /// An empty style object.
        /// </summary>
        public static readonly GridGroupOptionsStyleInfo Empty = new GridGroupOptionsStyleInfo();

        // Constructors
        static GridGroupOptionsStyleInfo()
        {
        }

        /// <summary>
        /// Initalizes a new style object.
        /// </summary>
        [DebuggerStepThrough()] public GridGroupOptionsStyleInfo()
            : base(new GridGroupOptionsStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new style object and copies all data from an existing style object.
        /// </summary>
        /// <param name="style">The style object that contains the original data.</param>
        [DebuggerStepThrough()] public GridGroupOptionsStyleInfo(GridGroupOptionsStyleInfo style)
            : base(style.Store)
        {
        }

        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="GridGroupOptionsStyleInfoStore"/>.
        /// </summary>
        /// <param name="store">A <see cref="GridGroupOptionsStyleInfoStore"/> that holds data for this <see cref="GridGroupOptionsStyleInfo"/>.
        /// All changes in this style object will saved in the <see cref="GridGroupOptionsStyleInfoStore"/> object.</param>
        [DebuggerStepThrough()] public GridGroupOptionsStyleInfo(GridGroupOptionsStyleInfoStore store)
            : base(store)
        {
        }

        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="GridGroupOptionsStyleInfoIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridGroupOptionsStyleInfoIdentity"/> that holds the identity for this <see cref="GridGroupOptionsStyleInfo"/>.
        /// </param>
        [DebuggerStepThrough()] public GridGroupOptionsStyleInfo(StyleInfoIdentityBase identity)
            : base(identity, new GridGroupOptionsStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new style object and associates it with an existing <see cref="GridGroupOptionsStyleInfoIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridGroupOptionsStyleInfoIdentity"/> that holds the identity for this <see cref="GridGroupOptionsStyleInfo"/>.
        /// </param>
        /// <param name="store">A <see cref="GridGroupOptionsStyleInfoStore"/> that holds data for this <see cref="GridGroupOptionsStyleInfo"/>.
        /// All changes in this style object will saved in the <see cref="GridGroupOptionsStyleInfoStore"/> object.
        /// </param>
        [DebuggerStepThrough()] public GridGroupOptionsStyleInfo(StyleInfoIdentityBase identity, GridGroupOptionsStyleInfoStore store)
            : base(identity, store)
        {
        }

        /// <override/>
        /// <summary>
        /// Returns a string holding the current object.
        /// </summary>
        /// <returns>String representation of the current object.</returns>
        public override string ToString()
        {
            return Store.ToString();
        }

        /// <override/>
        protected override void OnStyleChanged(StyleInfoProperty sip)
        {
            cpl = null;
            base.OnStyleChanged(sip);
        }

        // Default

        // Static Fields
        [ThreadStatic]
        private static GridGroupOptionsStyleInfo defaultStyle = null;

        /// <summary>
        /// Returns a <see cref="GridGroupOptionsStyleInfo"/> with default settings.
        /// </summary>
        public static GridGroupOptionsStyleInfo Default
        {
            get
            {
                if (GridGroupOptionsStyleInfo.defaultStyle == null)
                {
                    defaultStyle = new GridGroupOptionsStyleInfo();
                    defaultStyle.ShowAddNewRecordBeforeDetails = true;
                    defaultStyle.ShowCaption = true;
                    defaultStyle.CaptionText = "{CategoryCaption}: {Category} - {RecordCount} Items";
                    defaultStyle.ShowCaptionPlusMinus = true;
                    defaultStyle.ShowCaptionSummaryCells = false;
                    defaultStyle.ShowColumnHeaders = false;
                    defaultStyle.ShowStackedHeaders = false;
                    defaultStyle.ShowFilterBar = false;
                    defaultStyle.ShowGroupFooter = false;
                    defaultStyle.ShowGroupHeader = false;
                    defaultStyle.ShowGroupPreview = false;
                    defaultStyle.ShowEmptyGroups = false;
                    defaultStyle.RepaintCaptionWhenItemsChanged = true;
                    defaultStyle.IsExpandedInitialValue = false;
                    defaultStyle.ShowSummaries = true;
                    defaultStyle.SummaryRowPlacement = GridSummaryRowPlacement.AfterDetails;
                    defaultStyle.ShowGroupSummaryWhenCollapsed = false;
                    defaultStyle.ShowGroupIndentAsCoveredRange = false;
                    defaultStyle.AllowScrollCaptionBar = true;
                }

                return GridGroupOptionsStyleInfo.defaultStyle;
            }
        }

        [ThreadStatic]
        private static GridGroupOptionsStyleInfo defaultTopLevelStyle = null;

        /// <summary>
        /// Returns a <see cref="GridGroupOptionsStyleInfo"/> with default settings for look and behavior of the top level group.
        /// </summary>
        public static GridGroupOptionsStyleInfo DefaultTopLevelGroupOptions
        {
            get
            {
                if (GridGroupOptionsStyleInfo.defaultTopLevelStyle == null)
                {
                    defaultTopLevelStyle = new GridGroupOptionsStyleInfo();
#if ASPNET
                    defaultTopLevelStyle.CaptionText = "{TableName}: {RecordCount} Items ({PagingInfo})";
#else
                    defaultTopLevelStyle.CaptionText = "{TableName}: {RecordCount} Items";
#endif
                    defaultTopLevelStyle.ShowAddNewRecordBeforeDetails = true;
                    defaultTopLevelStyle.ShowCaptionSummaryCells = false;
                    defaultTopLevelStyle.ShowCaption = true;
                    defaultTopLevelStyle.ShowCaptionPlusMinus = false;
                    defaultTopLevelStyle.ShowColumnHeaders = true;
                    defaultTopLevelStyle.ShowStackedHeaders = true;
                    defaultTopLevelStyle.IsExpandedInitialValue = true;
                    defaultTopLevelStyle.ShowFilterBar = false;
                }

                return GridGroupOptionsStyleInfo.defaultTopLevelStyle;
            }
        }

        [ThreadStatic]
        private static GridGroupOptionsStyleInfo defaultForeignKeyTableGroupOptions = null;

        /// <summary>
        /// Returns a <see cref="GridGroupOptionsStyleInfo"/> with default settings for look and behavior of the top level group.
        /// </summary>
        public static GridGroupOptionsStyleInfo DefaultForeignKeyTableGroupOptions
        {
            get
            {
                if (GridGroupOptionsStyleInfo.defaultForeignKeyTableGroupOptions == null)
                {
                    defaultForeignKeyTableGroupOptions = new GridGroupOptionsStyleInfo();
                    defaultForeignKeyTableGroupOptions.ShowAddNewRecordBeforeDetails = true;
                    defaultForeignKeyTableGroupOptions.ShowCaptionSummaryCells = false;
                    defaultForeignKeyTableGroupOptions.ShowCaption = false;
                    defaultForeignKeyTableGroupOptions.ShowCaptionPlusMinus = false;
                    defaultForeignKeyTableGroupOptions.ShowColumnHeaders = true;
                    defaultForeignKeyTableGroupOptions.ShowStackedHeaders = true;
                    defaultForeignKeyTableGroupOptions.ShowFilterBar = false;
                    defaultForeignKeyTableGroupOptions.ShowSummaries = false;
                    defaultForeignKeyTableGroupOptions.SummaryRowPlacement = GridSummaryRowPlacement.AfterDetails;
                    defaultForeignKeyTableGroupOptions.ShowAddNewRecordAfterDetails = false;
                }

                return GridGroupOptionsStyleInfo.defaultForeignKeyTableGroupOptions;
            }
        }

        [ThreadStatic]
        private static GridGroupOptionsStyleInfo defaultChildTableStyle = null;

        /// <summary>
        /// Returns a <see cref="GridGroupOptionsStyleInfo"/> with default settings for the look and behavior of the nested child relations
        /// </summary>
        public static GridGroupOptionsStyleInfo DefaultNestedTableGroupOptions
        {
            get
            {
                if (GridGroupOptionsStyleInfo.defaultChildTableStyle == null)
                {
                    defaultChildTableStyle = new GridGroupOptionsStyleInfo();
#if ASPNET
                    defaultChildTableStyle.CaptionText = "{TableName}: {RecordCount} Items ({PagingInfo})";
#else
                    defaultChildTableStyle.CaptionText = "{TableName}: {RecordCount} Items";
#endif
                    defaultChildTableStyle.ShowCaption = true;
                    defaultChildTableStyle.ShowCaptionSummaryCells = false;
                    defaultChildTableStyle.ShowCaptionPlusMinus = true;
                    defaultChildTableStyle.ShowAddNewRecordBeforeDetails = true;
                    defaultChildTableStyle.ShowColumnHeaders = true;
                    defaultChildTableStyle.ShowStackedHeaders = true;
                    defaultChildTableStyle.ShowFilterBar = false;
                }

                return GridGroupOptionsStyleInfo.defaultChildTableStyle;
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
        
        #region CaptionText
        /// <summary>
        /// Lets you control the caption text displayed. See the remarks for allowed tokens.
        /// </summary>
        /// <remarks>
        /// Caption Format Tokens are: <para/>
        /// <list type="table">
        /// <listheader><term>Token</term><description>Description</description></listheader>
        /// <item><term>{TableName}</term><description>Displays the CaptionSection.ParentTableDescriptor.Name</description></item>
        /// <item><term>{CategoryName}</term><description>Displays the CaptionSection.ParentGroup.Name.</description></item>
        /// <item><term>{CategoryCaption}</term><description>Displays the HeaderText of the column that this group belongs to.</description></item>
        /// <item><term>{Category}</term><description>Displays the CaptionSection.ParentGroup.Category.</description></item>
        /// <item><term>{RecordCount}</term><description>Displays the CaptionSection.ParentGroup.GetFilteredRecordCount().</description></item>
        /// <item><term>summary tokens</term><description>Allows you to display any item you enter as a Summary Column. See discussion below: <para/>
        /// </description></item>
        /// </list>
        /// <para/>
        /// Custom Summary Tokens: <para/>
        /// Any summary item you add can be included in the CaptionText. You have the option of hiding summaries, so it is possible to add summaries only for the purpose of displaying values in the CaptionText. If you have added a summary row named Row1, and a Summary columns named Column1, then you can also use the value of this summary item in the caption with the token {Row1.Column1}.
        /// <para>When used in ASP.Net you can also use the {PageCount} and {CurrentPage} tokens.</para>
        /// </remarks>
        [NotifyParentProperty(true),
        Description("Specifies the caption text displayed. {TableName}, {CategoryName}, {CategoryCaption}, {Category}, {RecordCount} and custom summary tokens are tokens that could be used.")]
        public string CaptionText
        {
            [DebuggerStepThrough()]
            get
            {
                return (string) GetValue(GridGroupOptionsStyleInfoStore.CaptionTextProperty);
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridGroupOptionsStyleInfoStore.CaptionTextProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridGroupOptionsStyleInfo.CaptionText"/>.
        /// </summary>
        [DebuggerStepThrough()] 
        public void ResetCaptionText()
        {
            ResetValue(GridGroupOptionsStyleInfoStore.CaptionTextProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCaptionText()
        {
            return HasValue(GridGroupOptionsStyleInfoStore.CaptionTextProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridGroupOptionsStyleInfo.CaptionText"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCaptionText
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridGroupOptionsStyleInfoStore.CaptionTextProperty);
            }
        }
        #endregion
        #region CaptionSummaryRow
        /// <summary>
        /// Lets you specify a summary row that should be displayed inside CaptionSummaryCells
        /// when ShowCaptionSummaryCells has been set to true.
        /// </summary>
        [Description("Specifies a caption that should be displayed inside CaptionSummaryCells when ShowCaptionSummaryCells has been set to true."),
        Category("Look and Feel")]
        [NotifyParentProperty(true)]
        public string CaptionSummaryRow
        {
            [DebuggerStepThrough()]
            get
            {
                return (string) GetValue(GridGroupOptionsStyleInfoStore.CaptionSummaryRowProperty);
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridGroupOptionsStyleInfoStore.CaptionSummaryRowProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="GridGroupOptionsStyleInfo.CaptionSummaryRow"/>.
        /// </summary>
        [DebuggerStepThrough()] 
        public void ResetCaptionSummaryRow()
        {
            ResetValue(GridGroupOptionsStyleInfoStore.CaptionSummaryRowProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeCaptionSummaryRow()
        {
            return HasValue(GridGroupOptionsStyleInfoStore.CaptionSummaryRowProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridGroupOptionsStyleInfo.CaptionSummaryRow"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCaptionSummaryRow
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridGroupOptionsStyleInfoStore.CaptionSummaryRowProperty);
            }
        }
        #endregion
        #region ShowAddNewRecordBeforeDetails
        /// <summary>
        /// If true, AddNewRecord shown at top of group.
        /// </summary>
        [NotifyParentProperty(true)]
        [Description("If true, AddNewRecord shown at top of group."),
        Category("Look and Feel")]
        public bool ShowAddNewRecordBeforeDetails
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridGroupOptionsStyleInfoStore.ShowAddNewRecordBeforeDetailsProperty) != 0;
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridGroupOptionsStyleInfoStore.ShowAddNewRecordBeforeDetailsProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridGroupOptionsStyleInfo.ShowAddNewRecordBeforeDetails"/>.
        /// </summary>
        [DebuggerStepThrough()] 
        public void ResetShowAddNewRecordBeforeDetails()
        {
            ResetValue(GridGroupOptionsStyleInfoStore.ShowAddNewRecordBeforeDetailsProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowAddNewRecordBeforeDetails()
        {
            return HasValue(GridGroupOptionsStyleInfoStore.ShowAddNewRecordBeforeDetailsProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridGroupOptionsStyleInfo.ShowAddNewRecordBeforeDetails"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShowAddNewRecordBeforeDetails
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridGroupOptionsStyleInfoStore.ShowAddNewRecordBeforeDetailsProperty);
            }
        }
        #endregion
        #region ShowAddNewRecordAfterDetails
        /// <summary>
        /// If true, AddNewRecord shown at bottom of group.
        /// </summary>
        [NotifyParentProperty(true)]
        [Description("If true, AddNewRecord shown at bottom of group."),
        Category("Look and Feel")]
        public bool ShowAddNewRecordAfterDetails
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridGroupOptionsStyleInfoStore.ShowAddNewRecordAfterDetailsProperty) != 0;
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridGroupOptionsStyleInfoStore.ShowAddNewRecordAfterDetailsProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridGroupOptionsStyleInfo.ShowAddNewRecordAfterDetails"/>.
        /// </summary>
        [DebuggerStepThrough()] 
        public void ResetShowAddNewRecordAfterDetails()
        {
            ResetValue(GridGroupOptionsStyleInfoStore.ShowAddNewRecordAfterDetailsProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowAddNewRecordAfterDetails()
        {
            return HasValue(GridGroupOptionsStyleInfoStore.ShowAddNewRecordAfterDetailsProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridGroupOptionsStyleInfo.ShowAddNewRecordAfterDetails"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShowAddNewRecordAfterDetails
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridGroupOptionsStyleInfoStore.ShowAddNewRecordAfterDetailsProperty);
            }
        }
        #endregion

        #region ShowCaption

        /// <summary>
        /// Indicates whether the Caption row is visible.
        /// </summary>
        [NotifyParentProperty(true)]
        [Description("Indicates whether the Caption row is visible."),
        Category("Look and Feel")]
        public bool ShowCaption
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridGroupOptionsStyleInfoStore.ShowCaptionProperty) != 0;
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridGroupOptionsStyleInfoStore.ShowCaptionProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridGroupOptionsStyleInfo.ShowCaption"/>.
        /// </summary>
        [DebuggerStepThrough()] 
        public void ResetShowCaption()
        {
            ResetValue(GridGroupOptionsStyleInfoStore.ShowCaptionProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowCaption()
        {
            return HasValue(GridGroupOptionsStyleInfoStore.ShowCaptionProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridGroupOptionsStyleInfo.ShowCaption"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShowCaption
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridGroupOptionsStyleInfoStore.ShowCaptionProperty);
            }
        }
        #endregion
        #region ShowCaptionPlusMinus

        /// <summary>
        /// Indicates whether there is a PlusMinus cell next to the Caption.
        /// </summary>
        [NotifyParentProperty(true)]
        [Description("Indicates whether there is a PlusMinus cell next to the Caption."),
        Category("Look and Feel")]
        public bool ShowCaptionPlusMinus
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridGroupOptionsStyleInfoStore.ShowCaptionPlusMinusProperty) != 0;
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridGroupOptionsStyleInfoStore.ShowCaptionPlusMinusProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridGroupOptionsStyleInfo.ShowCaptionPlusMinus"/>.
        /// </summary>
        [DebuggerStepThrough()] 
        public void ResetShowCaptionPlusMinus()
        {
            ResetValue(GridGroupOptionsStyleInfoStore.ShowCaptionPlusMinusProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowCaptionPlusMinus()
        {
            return HasValue(GridGroupOptionsStyleInfoStore.ShowCaptionPlusMinusProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridGroupOptionsStyleInfo.ShowCaptionPlusMinus"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShowCaptionPlusMinus
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridGroupOptionsStyleInfoStore.ShowCaptionPlusMinusProperty);
            }
        }
        #endregion
        #region ShowCaptionSummaryCells
        /// <summary>
        /// Indicates whether the group caption should display summaries in columns instead
        /// of only one large caption bar. The CaptionSummaryRow then lets you specify
        /// a SummaryRow with Summary column information.
        /// </summary>
        [NotifyParentProperty(true)]
        [Description("Indicates whether the group caption should display summaries in columns instead of only one large caption bar."),
        Category("Look and Feel")]
        public bool ShowCaptionSummaryCells
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridGroupOptionsStyleInfoStore.ShowCaptionSummaryCellsProperty) != 0;
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridGroupOptionsStyleInfoStore.ShowCaptionSummaryCellsProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridGroupOptionsStyleInfo.ShowCaptionSummaryCells"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetShowCaptionSummaryCells()
        {
            ResetValue(GridGroupOptionsStyleInfoStore.ShowCaptionSummaryCellsProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowCaptionSummaryCells()
        {
            return HasValue(GridGroupOptionsStyleInfoStore.ShowCaptionSummaryCellsProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridGroupOptionsStyleInfo.ShowCaptionSummaryCells"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShowCaptionSummaryCells
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridGroupOptionsStyleInfoStore.ShowCaptionSummaryCellsProperty);
            }
        }
        #endregion
        #region ShowGroupHeader

        /// <summary>
        /// Indicates whether a header is visible.
        /// </summary>
        [NotifyParentProperty(true)]
        [Description("Indicates whether a group header is visible."),
        Category("Look and Feel")]
        public bool ShowGroupHeader
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridGroupOptionsStyleInfoStore.ShowGroupHeaderProperty) != 0;
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridGroupOptionsStyleInfoStore.ShowGroupHeaderProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridGroupOptionsStyleInfo.ShowGroupHeader"/>.
        /// </summary>
        [DebuggerStepThrough()] 
        public void ResetShowGroupHeader()
        {
            ResetValue(GridGroupOptionsStyleInfoStore.ShowGroupHeaderProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowGroupHeader()
        {
            return HasValue(GridGroupOptionsStyleInfoStore.ShowGroupHeaderProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridGroupOptionsStyleInfo.ShowGroupHeader"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShowGroupHeader
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridGroupOptionsStyleInfoStore.ShowGroupHeaderProperty);
            }
        }
        #endregion
        #region ShowGroupFooter
        /// <summary>
        /// Indicates whether a footer is visible.
        /// </summary>
        [Description("Indicates whether a group footer is visible."),
        Category("Look and Feel")]
        [NotifyParentProperty(true)]
        public bool ShowGroupFooter
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridGroupOptionsStyleInfoStore.ShowGroupFooterProperty) != 0;
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridGroupOptionsStyleInfoStore.ShowGroupFooterProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridGroupOptionsStyleInfo.ShowGroupFooter"/>.
        /// </summary>
        [DebuggerStepThrough()] 
        public void ResetShowGroupFooter()
        {
            ResetValue(GridGroupOptionsStyleInfoStore.ShowGroupFooterProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowGroupFooter()
        {
            return HasValue(GridGroupOptionsStyleInfoStore.ShowGroupFooterProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridGroupOptionsStyleInfo.ShowGroupFooter"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShowGroupFooter
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridGroupOptionsStyleInfoStore.ShowGroupFooterProperty);
            }
        }
        #endregion
        #region ShowSummaries
        /// <summary>
        /// Indicates whether summaries are visible.
        /// </summary>
        [Description("Indicates whether summaries are visible."),
        Category("Look and Feel")]
        [NotifyParentProperty(true)]
        public bool ShowSummaries
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridGroupOptionsStyleInfoStore.ShowSummariesProperty) != 0;
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridGroupOptionsStyleInfoStore.ShowSummariesProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridGroupOptionsStyleInfo.ShowSummaries"/>.
        /// </summary>
        [DebuggerStepThrough()] 
        public void ResetShowSummaries()
        {
            ResetValue(GridGroupOptionsStyleInfoStore.ShowSummariesProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowSummaries()
        {
            return HasValue(GridGroupOptionsStyleInfoStore.ShowSummariesProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridGroupOptionsStyleInfo.ShowSummaries"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShowSummaries
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridGroupOptionsStyleInfoStore.ShowSummariesProperty);
            }
        }
        #endregion
        #region SummaryRowPlacement
        /// <summary>
        /// Summaries location.
        /// </summary>
        [NotifyParentProperty(true)]
        [Description("Summaries location."),
        Category("Look and Feel"),
        DefaultValue(typeof(GridSummaryRowPlacement), "AfterDetails")]
        public GridSummaryRowPlacement SummaryRowPlacement
        {
            [DebuggerStepThrough()]
            get
            {
                return (GridSummaryRowPlacement)GetValue(GridGroupOptionsStyleInfoStore.SummaryRowPlacementProperty);
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridGroupOptionsStyleInfoStore.SummaryRowPlacementProperty, value);
            }
        }

        /// <summary>
        /// Resets <see cref="SummaryRowPlacement"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetSummaryRowPlacement()
        {
            ResetValue(GridGroupOptionsStyleInfoStore.SummaryRowPlacementProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeSummaryRowPlacement()
        {
            return HasValue(GridGroupOptionsStyleInfoStore.SummaryRowPlacementProperty);
        }

        /// <summary>
        /// Determines if <see cref="SummaryRowPlacement"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasSummaryRowPlacement
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridGroupOptionsStyleInfoStore.SummaryRowPlacementProperty);
            }
        }
                #endregion
        #region ShowColumnHeaders
        /// <summary>
        /// Indicates whether the column headers are visible.
        /// </summary>
        [Description("Indicates whether the column headers are visible."),
        Category("Look and Feel")]
        [NotifyParentProperty(true)]
        public bool ShowColumnHeaders
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridGroupOptionsStyleInfoStore.ShowColumnHeadersProperty) != 0;
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridGroupOptionsStyleInfoStore.ShowColumnHeadersProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridGroupOptionsStyleInfo.ShowColumnHeaders"/>.
        /// </summary>
        [DebuggerStepThrough()] 
        public void ResetShowColumnHeaders()
        {
            ResetValue(GridGroupOptionsStyleInfoStore.ShowColumnHeadersProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowColumnHeaders()
        {
            return HasValue(GridGroupOptionsStyleInfoStore.ShowColumnHeadersProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridGroupOptionsStyleInfo.ShowColumnHeaders"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShowColumnHeaders
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridGroupOptionsStyleInfoStore.ShowColumnHeadersProperty);
            }
        }
        #endregion
        #region ShowStackedHeaders

        /// <summary>
        /// Indicates whether the stacked headers are visible.
        /// </summary>
        [Description("Indicates whether the column headers are visible."),
        Category("Look and Feel")]
        [NotifyParentProperty(true)]
        public bool ShowStackedHeaders
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridGroupOptionsStyleInfoStore.ShowStackedHeadersProperty) != 0;
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridGroupOptionsStyleInfoStore.ShowStackedHeadersProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridGroupOptionsStyleInfo.ShowStackedHeaders"/>.
        /// </summary>
        [DebuggerStepThrough()] 
        public void ResetShowStackedHeaders()
        {
            ResetValue(GridGroupOptionsStyleInfoStore.ShowStackedHeadersProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowStackedHeaders()
        {
            return HasValue(GridGroupOptionsStyleInfoStore.ShowStackedHeadersProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridGroupOptionsStyleInfo.ShowStackedHeaders"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShowStackedHeaders
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridGroupOptionsStyleInfoStore.ShowStackedHeadersProperty);
            }
        }
        #endregion
        #region ShowFilterBar
        /// <summary>
        /// Indicates whether the FilterBar are visible.
        /// </summary>
        [Description("Indicates whether FilterBar is visible."),
        Category("Look and Feel")]
        [NotifyParentProperty(true)]
        public bool ShowFilterBar
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridGroupOptionsStyleInfoStore.ShowFilterBarProperty) != 0;
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridGroupOptionsStyleInfoStore.ShowFilterBarProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridGroupOptionsStyleInfo.ShowFilterBar"/>.
        /// </summary>
        [DebuggerStepThrough()] 
        public void ResetShowFilterBar()
        {
            ResetValue(GridGroupOptionsStyleInfoStore.ShowFilterBarProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowFilterBar()
        {
            return HasValue(GridGroupOptionsStyleInfoStore.ShowFilterBarProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridGroupOptionsStyleInfo.ShowFilterBar"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShowFilterBar
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridGroupOptionsStyleInfoStore.ShowFilterBarProperty);
            }
        }
        #endregion
        #region ShowGroupSummaryWhenCollapsed

        /// <summary>
        /// Indicates whether summary items are visible when the group is collapsed.
        /// </summary>
        [Description("Indicates whether summary items are visible when the group is collapsed."),
        Category("Look and Feel")]
        [NotifyParentProperty(true)]
        public bool ShowGroupSummaryWhenCollapsed
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridGroupOptionsStyleInfoStore.ShowGroupSummaryWhenCollapsedProperty) != 0;
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridGroupOptionsStyleInfoStore.ShowGroupSummaryWhenCollapsedProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridGroupOptionsStyleInfo.ShowGroupSummaryWhenCollapsed"/>.
        /// </summary>
        [DebuggerStepThrough()] 
        public void ResetShowGroupSummaryWhenCollapsed()
        {
            ResetValue(GridGroupOptionsStyleInfoStore.ShowGroupSummaryWhenCollapsedProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowGroupSummaryWhenCollapsed()
        {
            return HasValue(GridGroupOptionsStyleInfoStore.ShowGroupSummaryWhenCollapsedProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridGroupOptionsStyleInfo.ShowGroupSummaryWhenCollapsed"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShowGroupSummaryWhenCollapsed
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridGroupOptionsStyleInfoStore.ShowGroupSummaryWhenCollapsedProperty);
            }
        }
        #endregion
        #region ShowGroupIndentAsCoveredRange
        /// <summary>
        /// Indicates whether to treat all indent cells for the group as a single covered cell.
        /// </summary>
        [Description("Indicates whether to treat all indent cells for the group as a single covered cell."),
        Category("Look and Feel")]
        [NotifyParentProperty(true)]
        public bool ShowGroupIndentAsCoveredRange
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridGroupOptionsStyleInfoStore.ShowGroupIndentAsCoveredRangeProperty) != 0;
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridGroupOptionsStyleInfoStore.ShowGroupIndentAsCoveredRangeProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridGroupOptionsStyleInfo.ShowGroupIndentAsCoveredRange"/>.
        /// </summary>
        [DebuggerStepThrough()] 
        public void ResetShowGroupIndentAsCoveredRange()
        {
            ResetValue(GridGroupOptionsStyleInfoStore.ShowGroupIndentAsCoveredRangeProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowGroupIndentAsCoveredRange()
        {
            return HasValue(GridGroupOptionsStyleInfoStore.ShowGroupIndentAsCoveredRangeProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridGroupOptionsStyleInfo.ShowGroupIndentAsCoveredRange"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShowGroupIndentAsCoveredRange
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridGroupOptionsStyleInfoStore.ShowGroupIndentAsCoveredRangeProperty);
            }
        }
        #endregion
        #region ShowGroupPreview
        /// <summary>
        /// Indicates whether a preview is visible when the group is collapsed.
        /// </summary>
        [Description("Indicates whether a preview is visible when the group is collapsed."),
        Category("Look and Feel")]
        [NotifyParentProperty(true)]
        public bool ShowGroupPreview
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridGroupOptionsStyleInfoStore.ShowGroupPreviewProperty) != 0;
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridGroupOptionsStyleInfoStore.ShowGroupPreviewProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridGroupOptionsStyleInfo.ShowGroupPreview"/>.
        /// </summary>
        [DebuggerStepThrough()] 
        public void ResetShowGroupPreview()
        {
            ResetValue(GridGroupOptionsStyleInfoStore.ShowGroupPreviewProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowGroupPreview()
        {
            return HasValue(GridGroupOptionsStyleInfoStore.ShowGroupPreviewProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridGroupOptionsStyleInfo.ShowGroupPreview"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShowGroupPreview
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridGroupOptionsStyleInfoStore.ShowGroupPreviewProperty);
            }
        }
        #endregion
        #region RepaintCaptionWhenItemsChanged
        /// <summary>
        /// Indicates whether the caption row should be repainted when a record that belongs to
        /// the group was changed. The setting only has an effect when InvalidateAllWhenListChanged = false.
        /// </summary>
        [Description("Indicates whether the caption row should be repainted when a record that belongs to"+
        "the group was changed.  The setting only has an effect when InvalidateAllWhenListChanged = false."),
        Category("Look and Feel")]
        [NotifyParentProperty(true)]
        public bool RepaintCaptionWhenItemsChanged
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridGroupOptionsStyleInfoStore.RepaintCaptionWhenItemsChangedProperty) != 0;
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridGroupOptionsStyleInfoStore.RepaintCaptionWhenItemsChangedProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridGroupOptionsStyleInfo.RepaintCaptionWhenItemsChanged"/>.
        /// </summary>
        [DebuggerStepThrough()] 
        public void ResetRepaintCaptionWhenItemsChanged()
        {
            ResetValue(GridGroupOptionsStyleInfoStore.RepaintCaptionWhenItemsChangedProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeRepaintCaptionWhenItemsChanged()
        {
            return HasValue(GridGroupOptionsStyleInfoStore.RepaintCaptionWhenItemsChangedProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridGroupOptionsStyleInfo.RepaintCaptionWhenItemsChanged"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasRepaintCaptionWhenItemsChanged
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridGroupOptionsStyleInfoStore.RepaintCaptionWhenItemsChangedProperty);
            }
        }
        #endregion
        #region IsExpandedInitialValue
        /// <summary>
        /// Gets or sets whether groups should be shown expanded initially. IsExpandedInitialValue
        /// will be assigned to groups when the table is categorized, e.g. when GroupedColumns were changed.
        /// This value must be set before the GroupedColumns are changed.
        /// </summary>
        [Description("Gets or sets whether groups should be shown expanded initially.  IsExpandedInitialValue will be assigned to groups when the table is categorized"),
        Category("Look and Feel")]
        [NotifyParentProperty(true)]
        public bool IsExpandedInitialValue
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridGroupOptionsStyleInfoStore.IsExpandedInitialValueProperty) != 0;
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridGroupOptionsStyleInfoStore.IsExpandedInitialValueProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridGroupOptionsStyleInfo.IsExpandedInitialValue"/>.
        /// </summary>
        [DebuggerStepThrough()] 
        public void ResetIsExpandedInitialValue()
        {
            ResetValue(GridGroupOptionsStyleInfoStore.IsExpandedInitialValueProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeIsExpandedInitialValue()
        {
            return HasValue(GridGroupOptionsStyleInfoStore.IsExpandedInitialValueProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridGroupOptionsStyleInfo.IsExpandedInitialValue"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasIsExpandedInitialValue
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridGroupOptionsStyleInfoStore.IsExpandedInitialValueProperty);
            }
        }
        #endregion
        #region ShowEmptyGroups
        /// <summary>
        /// Indicates whether a preview is visible when group is collapsed.
        /// </summary>
        [Description("Indicates whether empty groups are visible when the group is collapsed."),
        Category("Look and Feel")]
        [NotifyParentProperty(true)]
        public bool ShowEmptyGroups
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridGroupOptionsStyleInfoStore.ShowEmptyGroupsProperty) != 0;
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridGroupOptionsStyleInfoStore.ShowEmptyGroupsProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridGroupOptionsStyleInfo.ShowEmptyGroups"/>.
        /// </summary>
        [DebuggerStepThrough()] 
        public void ResetShowEmptyGroups()
        {
            ResetValue(GridGroupOptionsStyleInfoStore.ShowEmptyGroupsProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeShowEmptyGroups()
        {
            return HasValue(GridGroupOptionsStyleInfoStore.ShowEmptyGroupsProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridGroupOptionsStyleInfo.ShowEmptyGroups"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShowEmptyGroups
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridGroupOptionsStyleInfoStore.ShowEmptyGroupsProperty);
            }
        }
        #endregion
        #region AllowScrollCaptionBar
        /// <summary>
        /// Indicates whether the caption bar should be scrolled horizontally or if
        /// it should stay fixed in the view.
        /// </summary>
        [Description("Indicates whether the caption bar should be scrolled horizontally or if it should stay fixed in the view.")]
        [NotifyParentProperty(true)]
        internal bool AllowScrollCaptionBar
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridGroupOptionsStyleInfoStore.AllowScrollCaptionBarProperty) != 0;
            }
            
            [DebuggerStepThrough()]
            set
            {
                SetValue(GridGroupOptionsStyleInfoStore.AllowScrollCaptionBarProperty, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Resets <see cref="GridGroupOptionsStyleInfo.AllowScrollCaptionBar"/>.
        /// </summary>
        [DebuggerStepThrough()] 
        public void ResetAllowScrollCaptionBar()
        {
            ResetValue(GridGroupOptionsStyleInfoStore.AllowScrollCaptionBarProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        [Obsolete("AllowScrollCaptionBar is not yet implemented")]
        private bool ShouldSerializeAllowScrollCaptionBar()
        {
            return HasValue(GridGroupOptionsStyleInfoStore.AllowScrollCaptionBarProperty);
        }

        /// <summary>
        /// Determines if <see cref="GridGroupOptionsStyleInfo.AllowScrollCaptionBar"/> has been initialized for the current object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasAllowScrollCaptionBar
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridGroupOptionsStyleInfoStore.AllowScrollCaptionBarProperty);
            }
        }
        #endregion

        #region CustomProperties
        GridGroupOptionsStyleInfoCustomPropertiesCollection cpl = null;

        /// <summary>
        /// Returns a collection of custom property objects that have
        /// at least one initialized value. The primary purpose of this
        /// collection is to support design-time code serialization of
        /// custom properties.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridGroupOptionsStyleInfoCustomPropertiesCollection CustomProperties
        {
            get
            {
                if (cpl == null)
                {
                    cpl = new GridGroupOptionsStyleInfoCustomPropertiesCollection(this);
                }

                return cpl;
            }
        }

        private bool ShouldSerializeCustomProperties()
        {
            return CustomProperties.Count > 0;
        }
        #endregion
     }
    
    /// <summary>
    /// GridGroupOptionsStyleInfoStore holds the plain data for a style object excluding identity information.
    /// </summary>
    /// <remarks>
    /// When persisting style information, <see cref="GridGroupOptionsStyleInfoStore"/> are the objects that should be
    /// saved. Identity information can be recreated at runtime when loading cell information but the
    /// cell information must be saved.
    /// <para/>
    /// GridGroupOptionsStyleInfoStore also holds the static "layout" information for the style.
    /// StaticData contains static variables with the information to access data
    /// in BitVector32 and StyleInfoObjectStore. This information can be shared
    /// among style objects of the same type but collision must be avoided between
    /// style types of different products. Having GridStyleInfoStore and ChartStyleInfoStore
    /// types solves that collision problem.
    /// </remarks>
    [Serializable,
    StaticDataField("sd")]
    [DebuggerStepThrough()]
    public class GridGroupOptionsStyleInfoStore: StyleInfoStore
    {
        static StaticData sd = new StaticData(typeof(GridGroupOptionsStyleInfoStore), typeof(GridGroupOptionsStyleInfo), false);

        internal static StaticData StaticData
        {
            get
            {
                return sd;
            }
        }

        /// <summary>
        /// Provides information about the <see cref="GridGroupOptionsStyleInfo.CaptionText"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CaptionTextProperty = sd.CreateStyleInfoProperty(typeof(string), "CaptionText");

        /// <summary>
        /// Provides information about the <see cref="GridGroupOptionsStyleInfo.ShowAddNewRecordBeforeDetails"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowAddNewRecordBeforeDetailsProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowAddNewRecordBeforeDetails", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridGroupOptionsStyleInfo.ShowAddNewRecordAfterDetails"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowAddNewRecordAfterDetailsProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowAddNewRecordAfterDetails", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridGroupOptionsStyleInfo.ShowCaption"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowCaptionProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowCaption", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridGroupOptionsStyleInfo.ShowCaptionPlusMinus"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowCaptionPlusMinusProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowCaptionPlusMinus", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridGroupOptionsStyleInfo.ShowGroupHeader"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowGroupHeaderProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowGroupHeader", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridGroupOptionsStyleInfo.ShowGroupFooter"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowGroupFooterProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowGroupFooter", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridGroupOptionsStyleInfo.ShowSummaries"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowSummariesProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowSummaries", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridGroupOptionsStyleInfo.SummaryRowPlacement"/> property.
        /// </summary>
        public readonly static StyleInfoProperty SummaryRowPlacementProperty = sd.CreateStyleInfoProperty(typeof(GridSummaryRowPlacement), "SummaryRowPlacement");

        /// <summary>
        /// Provides information about the <see cref="GridGroupOptionsStyleInfo.ShowColumnHeaders"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowColumnHeadersProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowColumnHeaders", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridGroupOptionsStyleInfo.ShowStackedHeaders"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowStackedHeadersProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowStackedHeaders", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridGroupOptionsStyleInfo.ShowFilterBar"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowFilterBarProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowFilterBar", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridGroupOptionsStyleInfo.ShowGroupSummaryWhenCollapsed"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowGroupSummaryWhenCollapsedProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowGroupSummaryWhenCollapsed", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridGroupOptionsStyleInfo.ShowGroupIndentAsCoveredRange"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowGroupIndentAsCoveredRangeProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowGroupIndentAsCoveredRange", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridGroupOptionsStyleInfo.ShowGroupPreview"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowGroupPreviewProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowGroupPreview", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridGroupOptionsStyleInfo.ShowCaptionSummaryCells"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowCaptionSummaryCellsProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowCaptionSummaryCells", 1, true);
        
        /// <summary>
        /// Provides information about the <see cref="GridGroupOptionsStyleInfo.CaptionSummaryRow"/> property.
        /// </summary>
        public readonly static StyleInfoProperty CaptionSummaryRowProperty = sd.CreateStyleInfoProperty(typeof(string), "CaptionSummaryRow");

        /// <summary>
        /// Provides information about the <see cref="GridGroupOptionsStyleInfo.RepaintCaptionWhenItemsChanged"/> property.
        /// </summary>
        public readonly static StyleInfoProperty RepaintCaptionWhenItemsChangedProperty = sd.CreateStyleInfoProperty(typeof(bool), "RepaintCaptionWhenItemsChanged", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridGroupOptionsStyleInfo.IsExpandedInitialValue"/> property.
        /// </summary>
        public readonly static StyleInfoProperty IsExpandedInitialValueProperty = sd.CreateStyleInfoProperty(typeof(bool), "IsExpandedInitialValue", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridGroupOptionsStyleInfo.ShowEmptyGroups"/> property.
        /// </summary>
        public readonly static StyleInfoProperty ShowEmptyGroupsProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowEmptyGroups", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridGroupOptionsStyleInfo.AllowScrollCaptionBar"/> property.
        /// </summary>
        public readonly static StyleInfoProperty AllowScrollCaptionBarProperty = sd.CreateStyleInfoProperty(typeof(bool), "AllowScrollCaptionBar", 1, true);

        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        static GridGroupOptionsStyleInfoStore()
        {
            //// static factory methods for subobjects
            ////BordersProperty.CreateObject = new CreateSubObjectHandler(GridBordersInfo.CreateObject);
        }
        
        /// <overload>
        /// Initializes a <see cref="GridGroupOptionsStyleInfoStore"/>
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridGroupOptionsStyleInfoStore"/>
        /// </summary>
        public GridGroupOptionsStyleInfoStore()
        {
            if (sd.IsEmpty)
            {
                new GridGroupOptionsStyleInfo();
            }
        }

        /// <summary>
        /// Initializes a new <see cref="GridGroupOptionsStyleInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridGroupOptionsStyleInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (sd.IsEmpty)
            {
                new GridGroupOptionsStyleInfo();
            }
        }

        /// <override/>
        /// <summary>
        /// Returns a copy of the current object.
        /// </summary>
        /// <returns>A duplicate of the current object.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new GridGroupOptionsStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }

    /// <summary>
    /// Interface for hosting a <see cref="GridGroupOptionsStyleInfo"/>
    /// </summary>
    public interface IGridGroupOptionsSource
    {
        /// <summary>
        /// Returns a reference to the <see cref="GridEngine"/> this object belongs to.
        /// </summary>
        GridEngine Engine { get; }

        /// <summary>
        /// Determines whether the GroupOptions object has been initialized.
        /// </summary>
        bool HasGroupOptions { get; }

        /// <summary>
        /// Gets the group options.
        /// </summary>
        GridGroupOptionsStyleInfo GroupOptions { get; }

        /// <summary>
        /// Notifies the host that properties in the GroupOptions object were changed.
        /// </summary>
        void RaiseGroupOptionsChanged(GridGroupOptionsChangedEventArgs e);

        /// <summary>
        /// Notifies the host that properties in the GroupOptions object are about to be changed.
        /// </summary>
        void RaiseGroupOptionsChanging(GridGroupOptionsChangedEventArgs e);

        /// <summary>
        /// Returns a <see cref="IGridGroupOptionsSource"/> of the first parent element with group options in the hierarchy.
        /// </summary>
        /// <returns>returns IGridGroupOptionsSource</returns>
        IGridGroupOptionsSource GetParentGroupOptionsSource();
    }

    /// <summary>
    /// The type of group options: TopLevelGroup, ChildTable or inner groups.
    /// </summary>
    public enum GridGroupOptionsType
    {
        /// <summary>
        /// Any type of group.
        /// </summary>
        AnyGroup,

        /// <summary>
        /// Top-level group.
        /// </summary>
        TopLevelGroup,

        /// <summary>
        /// Childtable of a nested relation
        /// </summary>
        ChildTable,

        /// <summary>
        /// Inner groups (grouped by groups)
        /// </summary>
        Groups
    }

    /// <summary>
    /// Summaries locations.
    /// </summary>
    public enum GridSummaryRowPlacement
    {
        /// <summary>
        /// Show Summaries before filter bar.
        /// </summary>
        BeforeFilter,

        /// <summary>
        /// Show Summaries before details.
        /// </summary>
        BeforeDetails,

        /// <summary>
        /// Show Summaries after details.
        /// </summary>
        AfterDetails
    }

    /// <summary>
    /// Provides identity information for <see cref="GridGroupOptionsStyleInfo"/> objects and
    /// methods for inheriting default setting from parent elements.
    /// </summary>
    public class GridGroupOptionsStyleInfoIdentity : StyleInfoIdentityBase
    {
        // Cache
        int version = -1;
#if WEAKREF
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
#else
        IStyleInfo[] cachedBaseStyles;
#endif

        GridGroupOptionsType groupOptionsType = GridGroupOptionsType.Groups;

        // Identity properties
        IGridGroupOptionsSource groupOptionsSource;

        /// <summary>
        /// The host element.
        /// </summary>
        public IGridGroupOptionsSource GroupOptionsSource
        {
            get
            {
                return groupOptionsSource;
            }
           
            set
            {
                groupOptionsSource = value;
            }
        }

////        public override bool Equals(object obj)
////        {
////            if (obj == null)
////                return this == null;
////
////            if (!(obj is GridGroupOptionsStyleInfoIdentity) || this == null)
////                return false;
////
////            GridGroupOptionsStyleInfoIdentity other = (GridGroupOptionsStyleInfoIdentity) obj;
////
////            return groupOptionsType == other.groupOptionsType;
////        }
////
////        public override int GetHashCode()
////        {
////            return base.GetHashCode ();
////        }
        
        ////private GridColumnDescriptor column;

        /// <override/>
        /// <summary>Releases all resources used by this component.</summary>
        public override void Dispose()
        {
            cachedBaseStyles = null;
            this.groupOptionsSource = null;
            base.Dispose();
        }

        /// <overload>
        /// Initializes the identity object with the host.
        /// </overload>
        /// <summary>
        /// Initializes the identity object with the host.
        /// </summary>
        /// <param name="groupOptionsSource">The host for the <see cref="GridGroupOptionsStyleInfo"/> object.</param>
        public GridGroupOptionsStyleInfoIdentity(IGridGroupOptionsSource groupOptionsSource)
        {
            this.groupOptionsSource = groupOptionsSource;
        }

        /// <summary>
        /// Initializes the identity object with the host.
        /// </summary>
        /// <param name="groupOptionsSource">The host for the <see cref="GridGroupOptionsStyleInfo"/> object.</param>
        /// <param name="groupOptionsType">The kind of group options.</param>
        public GridGroupOptionsStyleInfoIdentity(IGridGroupOptionsSource groupOptionsSource, GridGroupOptionsType groupOptionsType)
        {
            this.groupOptionsSource = groupOptionsSource;
            this.groupOptionsType = groupOptionsType;
        }

        /// <summary>
        /// Initializes a new <see cref="GridGroupOptionsStyleInfoIdentity"/> and copies its data from an existing object.
        /// </summary>
        /// <param name="other">The existing object to copy data from.</param>
        protected GridGroupOptionsStyleInfoIdentity(GridGroupOptionsStyleInfoIdentity other)
        {
            this.groupOptionsSource = other.groupOptionsSource;
            this.groupOptionsType = other.groupOptionsType;
        }

        /// <summary>
        /// Overriden. Returns base styles from <see cref="IGridData"/> by calling <see cref="IGridData.GetBaseStyles"/>.
        /// </summary>
        /// <param name="thisStyleInfo">A reference to a <see cref="IStyleInfo"/></param>
        /// <returns>An array of base styles.</returns>
        public override IStyleInfo[] GetBaseStyles(IStyleInfo thisStyleInfo)
        {
            if (cachedBaseStyles == null || version != groupOptionsSource.Engine.Version)
            {
                ArrayList styleList = new ArrayList();

                ////if (groupOptionsSource.GroupOptions != thisStyleInfo)
                ////    styleList.Add(groupOptionsSource.GroupOptions);

                if (groupOptionsSource is ChildTable || groupOptionsSource is TableDescriptor || groupOptionsSource is Engine)
                {
                    IGridGroupOptionsSource parentGroup = groupOptionsSource.GetParentGroupOptionsSource();

                    RelationDescriptor rd = null;

                    if (parentGroup is TableDescriptor)
                    {
                        rd = ((TableDescriptor) parentGroup).ParentRelation;
                    }

                    bool isForeignKeyTable = rd != null && (rd.RelationKind == RelationKind.ForeignKeyReference
                        || rd.RelationKind == RelationKind.ListItemReference
                        || rd.RelationKind == RelationKind.ForeignKeyKeyWords);     //// ForeignListItems

                    bool isTopLevelGroup = (groupOptionsSource is Group && ((Group) groupOptionsSource).IsTopLevelGroup)
                        || this.groupOptionsType == GridGroupOptionsType.TopLevelGroup;

                    bool isNestedTable = (groupOptionsSource is ChildTable && !((Group) groupOptionsSource).IsTopLevelGroup)
                        || this.groupOptionsType == GridGroupOptionsType.ChildTable;
                    
                    while (parentGroup != null && !(parentGroup is Engine))
                    {
                        if (parentGroup is GridTableDescriptor)
                        {
                            styleList.Add(((GridTableDescriptor) parentGroup).TopLevelGroupOptions);
                        }

                        parentGroup = parentGroup.GetParentGroupOptionsSource();
                    }

                    if (isForeignKeyTable)
                    {
                        styleList.Add(GridGroupOptionsStyleInfo.DefaultForeignKeyTableGroupOptions);
                    }
                    else if (isTopLevelGroup)
                    {
                        if (!(groupOptionsSource is Engine) && groupOptionsSource.Engine != null)
                        {
                            styleList.Add(groupOptionsSource.Engine.TopLevelGroupOptions);
                        }

                        styleList.Add(GridGroupOptionsStyleInfo.DefaultTopLevelGroupOptions);
                    }
                    else if (isNestedTable)
                    {
                        if (!(groupOptionsSource is Engine))
                        {
                            styleList.Add(groupOptionsSource.Engine.NestedTableGroupOptions);
                        }

                        styleList.Add(GridGroupOptionsStyleInfo.DefaultNestedTableGroupOptions);
                    }
                    else
                    {
                        if (!(groupOptionsSource is Engine) && groupOptionsSource.Engine != null)
                        {
                            styleList.Add(groupOptionsSource.Engine.ChildGroupOptions);
                        }
                    }
                }
                else
                {
                    IGridGroupOptionsSource parentGroup = groupOptionsSource.GetParentGroupOptionsSource();

                    while (parentGroup != null && !(parentGroup is Engine))
                    {
                        if (parentGroup.HasGroupOptions)
                        {
                            styleList.Add(parentGroup.GroupOptions);
                        }

                        parentGroup = parentGroup.GetParentGroupOptionsSource();
                    }

                    RelationDescriptor rd = parentGroup is ChildTable ? ((Element) parentGroup).ParentTableDescriptor.ParentRelation : null;
                    bool isForeignKeyTable = rd != null &&
                        (rd.RelationKind == RelationKind.ForeignKeyReference
                        || rd.RelationKind == RelationKind.ListItemReference
                        || rd.RelationKind == RelationKind.ForeignKeyKeyWords);            //// ForeignListItems

                    if (isForeignKeyTable)
                    {
                        styleList.Add(GridGroupOptionsStyleInfo.DefaultForeignKeyTableGroupOptions);
                    }
                    else
                    {
                        if (groupOptionsSource.Engine != null)
                            styleList.Add(groupOptionsSource.Engine.ChildGroupOptions);
                    }
                }

                styleList.Add(GridGroupOptionsStyleInfo.Default);

                cachedBaseStyles = new IStyleInfo[styleList.Count];
                styleList.CopyTo(cachedBaseStyles);
                if (groupOptionsSource.Engine != null)
                    version = groupOptionsSource.Engine.Version;
            }

            return cachedBaseStyles;
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
            if (groupOptionsSource != null)
            {
                sb.AppendFormat("GroupOptionsSource = {0}", groupOptionsSource.ToString());
            }
////            if (column != null)
////                sb.AppendFormat(", Column = {0}", column.Name);
            sb.Append(" }");
            return sb.ToString();
        }

        /// <override/>
        /// <summary>
        /// Occurs when a property in the <see cref="StyleInfoBase"/> has changed.
        /// </summary>
        /// <param name="style">A reference to the <see cref="StyleInfoBase"/> that has changed. </param>
        /// <param name="sip">Identity for the property to operate on.</param>
        public override void OnStyleChanged(StyleInfoBase style, StyleInfoProperty sip)
        {
            cachedBaseStyles = null;
            GridGroupOptionsChangedEventArgs e = new GridGroupOptionsChangedEventArgs(this, (GridGroupOptionsStyleInfo) style, sip);
            this.groupOptionsSource.RaiseGroupOptionsChanged(e);
        }

        /// <override/>
        /// <summary>
        /// Occurs before a property in the <see cref="StyleInfoBase"/> is changing.
        /// </summary>
        /// <param name="style">A reference to the <see cref="StyleInfoBase"/> that is changed. </param>
        /// <param name="sip">Identity for the property to operate on.</param>
        public override void OnStyleChanging(StyleInfoBase style, StyleInfoProperty sip)
        {
            GridGroupOptionsChangedEventArgs e = new GridGroupOptionsChangedEventArgs(this, (GridGroupOptionsStyleInfo) style, sip);
            this.groupOptionsSource.RaiseGroupOptionsChanging(e);
        }

        /// <summary>
        /// Results of ToString method.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
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
        /// Returns a base style that has specific property initialized.
        /// </summary>
        /// <param name="thisStyleInfo">Style information.</param>
        /// <param name="sip">Identity for the property to operate on.</param>
        /// <returns>returns StyleInfoBase</returns>
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
}

