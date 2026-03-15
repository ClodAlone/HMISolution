#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Syncfusion.Documentation;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
    #region enum ChartLegendStyle
    /// <summary>
    /// Specifies the information that is to be displayed on double clicking the legend.
    /// </summary>
    public enum ChartLegendStyle
    {
        /// <summary>
        /// Nothing will be displayed. User can drag the legend.
        /// </summary>
        Empty = 0,

        /// <summary>
        /// Enables the user to edit the series styles.
        /// </summary>
        SupportItemStyleEdit = 1,

        /// <summary>
        /// Enables the user to edit the Legend properties.
        /// </summary>
        SupportEditProperties = 2,

        /// <summary>
        /// Enables the user to edit both the series styles and legend properties.
        /// </summary>
        All = SupportEditProperties | SupportItemStyleEdit
    }
    #endregion

    /// <summary>
    /// The ChartLegend control represents the rectangular region that in turn contains 1 or more legend items, each of them
    /// usually representing a series in the chart.
    /// </summary>
    [ToolboxItem(false)]
    [Designer(typeof(ComponentDesigner)), DesignTimeVisible(false)]
    public class ChartLegend : ChartDockControl, IChartLegend
    {
        #region Constants
        /// <summary>
        /// The name of default legend.
        /// </summary>
        public const string DefaultName = "";

        private const int c_divCursorCoef = 4;
        private const string c_emptyText = "Empty";
        private const int c_checkBoxSize = 13;
        private static readonly BrushInfo c_defBackInterior = new BrushInfo(Color.Transparent);
        private static readonly StringFormat c_stringFormat = new StringFormat(StringFormatFlags.MeasureTrailingSpaces);
        #endregion

        #region Members

        private bool m_isWindowLess = false;

        private ChartLegendItemStyle m_baseItemStyle = new ChartLegendItemStyle();

        private bool mouseCatch = false;

        private bool m_floatingAutoSize = true;
        private bool m_showBorder = false;
        private int m_columnsCount = 1;
        private int m_rowsCount = 1;
        private int m_spacing = 4;

        private Cursor m_tempCursor = Cursors.Default;
        private bool m_canCursorChange = true;
        private MouseButtons button;
        private Point pointCatch = Point.Empty;
        private Size m_measuredSize = Size.Empty;

        private LineInfo m_border;
        private StringAlignment m_itemsAlignment = StringAlignment.Near;
        private StringAlignment m_textAlignment = StringAlignment.Center;
        private Color m_textColor = Color.Black;        
        private ChartLegendItem[] m_customItems = new ChartLegendItem[0];
        private ChartLegendItem[] m_allItems = null;

        private ChartControl m_chart;
        private ChartLegendPropertiesDialog m_propertiesEditDialog = null;
        private ChartLegendRepresentationType m_representationType = ChartLegendRepresentationType.SeriesType;
        private bool m_olnyColumnsForFloating = true;
        private ChartLegendStyle m_style = ChartLegendStyle.All;
        private bool m_needRefreshItems = true;
        private BrushInfo m_backInterior = new BrushInfo(c_defBackInterior);

        private bool m_isResizing = false;
        private Dictionary<ChartLegendItem, CheckBox> m_checkBoxes = new Dictionary<ChartLegendItem, CheckBox>();
        #endregion

        #region Events
        /// <summary>
        /// Fired when a legend item needs to draw. Handle this event to change the drawing of items.
        /// </summary>
        public event LegendDrawItemEventHandler DrawItem;

        /// <summary>
        /// Fired when the legend items need to be filtered. Handle this event to change the collection of LegendItems that the legend contains.
        /// </summary>
        public event LegendFilterItemsEventHandler FilterItems;

		/// <summary>
        /// Fired when a legend item text needs to draw. Handle this event to change the drawing of items text
       /// </summary>
         public event LegendDrawItemTextEventHandler DrawItemText;
		 
        /// <summary>
        /// Fired when the legend's minimum size is to be fixed.
        /// </summary>
        public event ChartLegendMinSizeEventHandler MinSize;
        #endregion

        #region Properties

        #region Appearance
        /// <summary>
        /// Gets or sets the spacing between the element borders and the control
        /// </summary>
        [Category("Appearance"), DefaultValue(4), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the spacing between the element borders and the control.")]
        public int Spacing
        {
            get
            {
                return m_spacing;
            }

            set
            {
                if (m_spacing != value)
                {
                    m_spacing = value;
                    OnLegentSizeChange();
                }
            }
        }

        /// <summary>
        /// Gets the line style of the border used by the legend.
        /// </summary>
        [Category("Appearance"), Description("Indicates the border style of border."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), ChartTemplate(ChartTemplateSet.Content)]
        public LineInfo Border
        {
            get
            {
                return this.m_border;
            }
        }

        /// <summary>
        /// Gets or sets RepresentationType which specifies how the icon in each legend item should be represented. By default, it renders an icon based on the series type.
        /// </summary>
        [DefaultValue(ChartLegendRepresentationType.SeriesType), Category("Appearance"), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the representation type of the legend items icon.")]
        public ChartLegendRepresentationType RepresentationType
        {
            get
            {
                return m_representationType;
            }

            set
            {
                if (m_representationType != value)
                {
                    m_representationType = value;
                    SetRepresentationType(value);
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show the legend border.
        /// </summary>
        [DefaultValue(false), Category("Appearance"), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the visibility of the legend border.")]
        public bool ShowBorder
        {
            get
            {
                return m_showBorder;
            }

            set
            {
                if (m_showBorder != value)
                {
                    m_showBorder = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show symbol]. If true, the exact symbol rendered in the series datapoints will be used to render the icon in the legend as well. 
        /// This overrides most other settings. Default is false.
        /// </summary>
        [DefaultValue(false), Category("Appearance"), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the visibility of the items symbol.")]
        public bool ShowSymbol
        {
            get
            {
                return m_baseItemStyle.ShowSymbol;
            }

            set
            {
                if (m_baseItemStyle.ShowSymbol != value)
                {
                    m_baseItemStyle.ShowSymbol = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the alignment of legend. Default is Center.
        /// </summary>
        [DefaultValue(ChartAlignment.Center), Category("Appearance"), ChartTemplate(ChartTemplateSet.Simple), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Obsolete("Use Alignment property.")]
        public ChartAlignment LegendAlignment
        {
            get
            {
                return base.Alignment;
            }

            set
            {
                base.Alignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the Legend items. Default is Near.
        /// </summary>
        [DefaultValue(StringAlignment.Near), Category("Appearance"), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the alignment of the legend items.")]
        public StringAlignment ItemsAlignment
        {
            get
            {
                return m_itemsAlignment;
            }

            set
            {
                if (m_itemsAlignment != value)
                {
                    m_itemsAlignment = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the text. Default is Center.
        /// </summary>
        [DefaultValue(StringAlignment.Center), Category("Appearance"), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the alignment of the text.")]
        public StringAlignment TextAlignment
        {
            get
            {
                return m_textAlignment;
            }

            set
            {
                if (m_textAlignment != value)
                {
                    m_textAlignment = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the text alignment of the legend item. Default is Center.
        /// </summary>
        [DefaultValue(VerticalAlignment.Center), Category("Appearance"), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the text alignment of the legend item.")]
        public VerticalAlignment ItemsTextAligment
        {
            get
            {
                return m_baseItemStyle.TextAlignment;
            }

            set
            {
                if (m_baseItemStyle.TextAlignment != value)
                {
                    m_baseItemStyle.TextAlignment = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show items shadow].
        /// </summary>
        /// <value><c>true</c> if [show items shadow]; otherwise, <c>false</c>.</value>
        [DefaultValue(false), Category("Appearance"), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the visibility of items shadow.")]
        public bool ShowItemsShadow
        {
            get
            {
                return m_baseItemStyle.ShowShadow;
            }

            set
            {
                if (m_baseItemStyle.ShowShadow != value)
                {
                    m_baseItemStyle.ShowShadow = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the offset of shadow for items on the legend.
        /// </summary>
        [DefaultValue(typeof(Size), "2, 2"), Category("Appearance")]
        [Description("Indicates the offset of shadow for items of legend.")]
        public Size ItemsShadowOffset
        {
            get
            {
                return m_baseItemStyle.ShadowOffset;
            }

            set
            {
                if (m_baseItemStyle.ShadowOffset != value)
                {
                    m_baseItemStyle.ShadowOffset = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the color for the legend's shadow.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple), Category("Appearance"), DefaultValue(typeof(Color), "Gray")]
        [Description("Indicates the color for the legend's shadow.")]
        public Color ItemsShadowColor
        {
            get
            {
                return m_baseItemStyle.ShadowColor;
            }

            set
            {
                if (m_baseItemStyle.ShadowColor != value)
                {
                    m_baseItemStyle.ShadowColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>The color of the text.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), Obsolete("Use ForeColor property.")]
        public Color TextColor
        {
            get
            {
                return this.ForeColor;
            }

            set
            {
                this.ForeColor = value;
            }
        }

        /// <summary>
        /// Gets or sets back interior of the chart legend.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple), Category("Appearance")]
        [Description("Indicates the background interior of the chart legend.")]
        public BrushInfo BackInterior
        {
            get
            {
                return m_backInterior != null ? m_backInterior : (m_chart == null ? c_defBackInterior : m_chart.BackInterior);
            }

            set
            {
                if (m_backInterior != value)
                {
                    m_backInterior = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color for the control.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Drawing.Color"></see> that represents the background color of the control. The default is the value of the <see cref="P:System.Windows.Forms.Control.DefaultBackColor"></see> property.</returns>
        /// <PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]       
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }

            set
            {
                base.BackColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the font of the text displayed by the control.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.Drawing.Font"></see> to apply to the text displayed by the control. The default is the value of the <see cref="P:System.Windows.Forms.Control.DefaultFont"></see> property.</returns>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [DefaultValue(typeof(Font), "Verdana, 8pt")]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public override Font Font
        {
            get
            {
                return base.Font;
            }

            set
            {
                base.Font = value;
            }
        }

        /// <summary>
        ///  Gets or sets the text associated with this control.
        /// </summary>
        [NotifyParentProperty(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Gets or sets the text associated with this control.")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public override string Text
        {
            get
            {
                {
                } 

                return base.Text;
            }

            set
            {
                base.Text = value;
            }
        }
        #endregion

        #region Layout
        /// <summary>
        /// Gets or sets the legend position.
        /// </summary>
        [DefaultValue(ChartDock.Right), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the docking position of the legend.")]
        public override ChartDock Position
        {
            get
            {
                return base.Position;
            }

            set
            {
                if (base.Position != value)
                {
                    base.Position = value;
                    SetOrientationByPosition();
                    OnRecalculateSizes();
                }
            }
        }

        /// <summary>
        /// Gets or sets the edges of the container to which a control is bound and determines how a control is resized with its parent.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// A bitwise combination of the <see cref="T:System.Windows.Forms.AnchorStyles"/> values. The default is Top and Left.
        /// </returns>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override AnchorStyles Anchor
        {
            get
            {
                return base.Anchor;
            }

            set
            {
                base.Anchor = value;
            }
        }

        /// <summary>
        /// Gets or sets which control borders are docked to its parent control and determines how a control is resized with its parent.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// One of the <see cref="T:System.Windows.Forms.DockStyle"/> values. The default is <see cref="F:System.Windows.Forms.DockStyle.None"/>.
        /// </returns>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">
        /// The value assigned is not one of the <see cref="T:System.Windows.Forms.DockStyle"/> values.
        /// </exception>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override DockStyle Dock
        {
            get
            {
                return base.Dock;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the default size of items.
        /// </summary>
        [Description("Indicates the size of the items icon.")]
        public Size ItemsSize
        {
            get
            {
                return m_baseItemStyle.RepresentationSize;
            }

            set
            {
                if (m_baseItemStyle.RepresentationSize != value)
                {
                    m_baseItemStyle.RepresentationSize = value;
                    OnRecalculateSizes();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether [set def size for custom]. This property is obsolete.
        /// </summary>
        /// <value>
        ///    <c>true</c> if [set def size for custom]; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), Obsolete("This property is obsolete and not used anymore."), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SetDefSizeForCustom
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the orientation of items on the legend. Default is Vertical.
        /// </summary>
        [DefaultValue(ChartOrientation.Vertical), Category("Layout"), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the orientation of the legend")]
        public override ChartOrientation Orientation
        {
            get
            {
                return base.Orientation;
            }

            set
            {
                if (base.Orientation != value)
                {
                    base.Orientation = value;

                    if (base.Orientation == value)
                    {
                        OnRecalculateSizes();
                    }
                }
            }
        }

        /// <summary>
        /// Gets default size of legend.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The default <see cref="T:System.Drawing.Size"/> of the control.
        /// </returns>
        protected override Size DefaultSize
        {
            get
            {
                return m_floatingAutoSize ? Size : base.DefaultSize;
            }
        }

        /// <summary>
        /// Controls the alignment of the Docked Control inside the ChartArea
        /// </summary>
        /// <value></value>
        [DefaultValue(ChartAlignment.Near)]
        [Description("Indicates the alignment of the legend inside the Chart")]
        public override ChartAlignment Alignment
        {
            get
            {
                return base.Alignment;
            }

            set
            {
                base.Alignment = value;
            }
        }
        #endregion

        #region Behaviour
        /// <summary>
        /// Gets or sets a value indicating whether floating the legend's auto size. It indicates whether the legend will automatically position and size itself when floating. Default is true.
        /// </summary>
        /// <value><c>true</c> if [floating auto size]; otherwise, <c>false</c>.</value>
        [DefaultValue(true), ChartTemplate(ChartTemplateSet.Simple), Description("Indicates whether the legend will automatically position and size itself when floating.")]
        public bool FloatingAutoSize
        {
            get
            {
                return m_floatingAutoSize;
            }

            set
            {
                if (m_floatingAutoSize != value)
                {
                    m_floatingAutoSize = value;
                    OnLegentSizeChange();
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of the columns to be used in the legend. Default is 1.
        /// </summary>
        [DefaultValue(1), ChartTemplate(ChartTemplateSet.Simple), Description("Indicates the number of columns in the Legend.")]
        public int ColumnsCount
        {
            get
            {
                return m_columnsCount;
            }

            set
            {
                if ((m_columnsCount != value) && (value > 0))
                {
                    m_columnsCount = value;
                    OnLegentSizeChange();
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of rows to be used in the legend. Default is 1.
        /// </summary>
        [DefaultValue(1), ChartTemplate(ChartTemplateSet.Simple), Description("Indicates the number of rows in the Legend.")]
        public int RowsCount
        {
            get
            {
                return m_rowsCount;
            }

            set
            {
                if ((m_rowsCount != value) && (value > 0))
                {
                    m_rowsCount = value;
                    OnLegentSizeChange();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [only columns for floating]. Specifies if the legend should only layout the items in columns while floating. Default is true.
        /// </summary>
        /// <value>
        ///     <c>true</c> if [only columns for floating]; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Specifies if the legend should only layout the items in columns while floating.")]
        public bool OnlyColumnsForFloating
        {
            get
            {
                return m_olnyColumnsForFloating;
            }

            set
            {
                if (m_olnyColumnsForFloating != value)
                {
                    m_olnyColumnsForFloating = value;
                    OnLegentSizeChange();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the checkbox of the legend is visible. If visible, the user can use the checkbox
        /// to show/hide the whole series.
        /// </summary>
        [DefaultValue(false), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates whether the checkbox of the legend is visible")]
        public bool VisibleCheckBox
        {
            get
            {
                return m_baseItemStyle.VisibleCheckBox;
            }

            set
            {
                if (m_baseItemStyle.VisibleCheckBox != value)
                {
                    m_baseItemStyle.VisibleCheckBox = value;
                    OnRecalculateSizes();
                }
            }
        }

        /// <summary>
        /// Gets or sets the style of the ChartLegend.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartLegendStyle Style
        {
            get
            {
                return m_style;
            }

            set
            {
                if (m_style != value)
                {
                    m_style = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the chart control is working as simple object.
        /// If it's true then chart don't uses functional of the control.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsWindowLess
        {
            get
            {
                return m_isWindowLess;
            }

            set
            {
                if (m_isWindowLess != value)
                {
                    m_isWindowLess = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the tab order of the control within its container.
        /// </summary>
        /// <value></value>
        /// <returns>The index value of the control within the set of controls within its container. The controls in the container are included in the tab order.</returns>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        new public int TabIndex
        {
            get
            {
                return base.TabIndex;
            }

            set
            {
                base.TabIndex = value;
            }
        }
        #endregion

        #region Data
        /// <summary>
        /// Gets or sets the list of custom LegendItems.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartLegendItem[] CustomItems
        {
            get
            {
                return m_customItems;
            }

            set
            {
                if (m_customItems != value)
                {
                    m_customItems = value;
                    m_needRefreshItems = true;
                    OnLegentSizeChange();
                }
            }
        }

        /// <summary>
        /// Gets the items of the legend.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartLegendItem[] Items
        {
            get
            {
                if (m_allItems == null || m_needRefreshItems)
                {
                    if (m_allItems != null)
                    {
                        foreach (ChartLegendItem item in m_allItems)
                        {
                            if (item != null)
                            {
                                item.SetLegend(null);
                            }
                        }
                    }

                    m_allItems = GetAllItems();

                    if (m_allItems != null)
                    {
                        foreach (ChartLegendItem item in m_allItems)
                        {
                            if (item != null)
                            {
                                item.SetLegend(this);
                            }
                        }
                    }

                    m_needRefreshItems = false;
                }

                return m_allItems;
            }
        }

        /// <summary>
        /// Gets the number of visible items.
        /// </summary>
        /// <value>The number of visible items.</value>
        private int VisibleItemsCount
        {
            get
            {
                int count = 0;

                foreach (ChartLegendItem item in this.Items)
                {
                    if (item.Visible)
                    {
                        count++;
                    }
                }

                return count;
            }
        }
        #endregion

        #region ShouldSerialize
        /// <summary>
        /// Should the serialize location.
        /// </summary>
        /// <returns>Returns true if the element should be serialized otherwise false.</returns>
        protected bool ShouldSerializeLocation()
        {
            return m_position == ChartDock.Floating;
        }

        /// <summary>
        /// Should the serialize items shadow offset.
        /// </summary>
        /// <returns>Returns true if the element should be serialized otherwise false.</returns>
        protected bool ShouldSerializeItemsShadowOffset()
        {
            return (ItemsShadowOffset != new SizeF(2, 2));
        }

        /// <summary>
        /// Should the color of the serialize items shadow.
        /// </summary>
        /// <returns>Returns true if the element should be serialized otherwise false.</returns>
        protected bool ShouldSerializeItemsShadowColor()
        {
            return (ItemsShadowColor != Color.FromArgb(100, Color.Gray));
        }

        /// <summary>
        /// Should the size of the serialize items.
        /// </summary>
        /// <returns>Returns true if the element should be serialized otherwise false.</returns>
        protected bool ShouldSerializeItemsSize()
        {
            return ItemsSize != new Size(20, 20);
        }

        /// <summary>
        /// Should the serialize back interior.
        /// </summary>
        /// <returns>Returns true if the element should be serialized otherwise false.</returns>
        protected bool ShouldSerializeBackInterior()
        {
            return !object.Equals(m_backInterior, c_defBackInterior);
        }

        /// <summary>
        /// Resets the back interior.
        /// </summary>
        protected void ResetBackInterior()
        {
            m_backInterior = new BrushInfo(c_defBackInterior);
        }
        #endregion

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartLegend"/> class. For internal use. Do not use this constructor directly.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="isWindowLess">if set to <c>true</c> [is window less].</param>
        public ChartLegend(ChartControl chart, bool isWindowLess)
        {
            m_chart = chart;
            m_isWindowLess = isWindowLess;

            this.SetStyle(ControlStyles.DoubleBuffer |
              ControlStyles.SupportsTransparentBackColor |
              ControlStyles.UserPaint |
              ControlStyles.AllPaintingInWmPaint |
              ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, false);

            base.Alignment = ChartAlignment.Near;
            base.Position = ChartDock.Right;
            base.BackColor = Color.Transparent;
            base.Font = new Font("Verdana", 8f);

            m_border = new LineInfo();
            m_border.SettingsChanged += new EventHandler(BorderSettingsChanged);

            if (m_chart != null)
            {
                this.WireSeriesCollection();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartLegend"/> class. For internal use. Do not use this constructor directly.
        /// </summary>
        /// <param name="chart">The chart control aggregating this legend.</param>
        public ChartLegend(ChartControl chart)
            : this(chart, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartLegend"/> class.
        /// </summary>
        public ChartLegend()
            : this(null)
        {
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control"></see> and its child controls and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_chart != null)
                {
                    this.UnWireSeriesCollection();

                    if (m_chart.Legends != null)
                    {
                        m_chart.Legends.Remove(this);
                    }

                    m_chart = null;
                }

                if (m_border != null)
                {
                    m_border.SettingsChanged -= new EventHandler(this.BorderSettingsChanged);
                    m_border = null;
                }

                if (m_baseItemStyle != null)
                {
                    m_baseItemStyle.Clear();
                    m_baseItemStyle = null;
                }

                if (m_customItems != null)
                {
                    for (int i = 0; i < m_customItems.Length; i++)
                    {
                        m_customItems[i].Dispose();
                        m_customItems[i] = null;
                    }

                    m_customItems = null;
                }

                if (m_allItems != null)
                {
                    for (int i = 0; i < m_allItems.Length; i++)
                    {
                        m_allItems[i].Dispose();
                        m_allItems[i] = null;
                    }

                    m_allItems = null;
                }
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Overrides the <see cref="Control.Refresh"/> method.
        /// </summary>
        public override void Refresh()
        {
            //// WireSeriesCollection();
            this.m_needRefreshItems = true;
            base.Refresh();
        }

        /// <summary>
        /// Calculates and returns the minimum size required by this legend to fit all display items.
        /// </summary>
        /// <param name="g">Graphics object to be used for metrics.</param>
        /// <returns>Size required.</returns>
        public Size GetMinSize(Graphics g)
        {
            float minWidth = 0, minHeight = 0;
            ChartLegendItem[] items = Items;

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].Visible)
                {
                    SizeF sz = items[i].Measure(g);
                    minHeight = Math.Max(minHeight, sz.Height);
                    minWidth = Math.Max(minWidth, sz.Width);
                }
            }

            Size minSize = new SizeF(minWidth, minHeight).ToSize();

            ChartLegendMinSizeEventArgs sizeArgs = new ChartLegendMinSizeEventArgs(minSize);
            this.RaiseMinSize(sizeArgs);

            if (sizeArgs.Handled == true)
            {
                minSize = sizeArgs.Size;
            }

            return minSize;
        }

        /// <summary>
        /// Gets the legend item at the specified point.
        /// </summary>
        /// <param name="pt">The point.</param>
        /// <returns>Returns ChartLegendItem.</returns>
        public ChartLegendItem GetItemBy(Point pt)
        {
            return this.GetItemBy(pt.X, pt.Y);
        }

        /// <summary>
        /// Gets the legend item at the specified coordinates.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>Returns ChartLegendItem.</returns>
        public ChartLegendItem GetItemBy(int x, int y)
        {
            ChartLegendItem res = null;
            ChartLegendItem[] items = Items;

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].IsHit(x, y))
                {
                    res = items[i];
                    break;
                }
            }

            return res;
        }

        /// <summary>
        /// Sets the callback handler to filter the legend items.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void SetEventHandler(LegendFilterItemsEventHandler handler)
        {
            this.FilterItems = handler;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Chooses the necessary <see cref="Graphics"/> object.
        /// </summary>
        /// <returns>Returns the Graphics object.</returns>
        protected Graphics GetGraphics()
        {
            if (m_isWindowLess)
                return m_chart.GetGraphics();
            else
                return this.CreateGraphics();
        }

        /// <summary>
        /// Converts the specified <see cref="ChartLegendRepresentationType"/> to <see cref="ChartLegendItemType"/> and sets for all items.
        /// </summary>
        /// <param name="type">The type.</param>
        private void SetRepresentationType(ChartLegendRepresentationType type)
        {
            ChartLegendItem[] items = this.Items;

            foreach (ChartLegendItem item in items)
            {
                ChartSeriesLegendItem seriesItem = item as ChartSeriesLegendItem;

                if (seriesItem != null)
                {
                    seriesItem.DrawSeriesIcon = (type == ChartLegendRepresentationType.SeriesType);
                }
            }

            switch (type)
            {
                case ChartLegendRepresentationType.None:
                    {
                        m_baseItemStyle.Type = ChartLegendItemType.None;
                        break;
                    }

                case ChartLegendRepresentationType.SeriesImage:
                    {
                        m_baseItemStyle.Type = ChartLegendItemType.Image;
                        break;
                    }

                case ChartLegendRepresentationType.SeriesType:
                case ChartLegendRepresentationType.Rectangle:
                    {
                        m_baseItemStyle.Type = ChartLegendItemType.Rectangle;
                        break;
                    }

                case ChartLegendRepresentationType.Circle:
                    {
                        m_baseItemStyle.Type = ChartLegendItemType.Circle;
                        break;
                    }

                case ChartLegendRepresentationType.Diamond:
                    {
                        m_baseItemStyle.Type = ChartLegendItemType.Diamond;
                        break;
                    }

                case ChartLegendRepresentationType.Hexagon:
                    {
                        m_baseItemStyle.Type = ChartLegendItemType.Hexagon;
                        break;
                    }

                case ChartLegendRepresentationType.InvertedTriangle:
                    {
                        m_baseItemStyle.Type = ChartLegendItemType.InvertedTriangle;
                        break;
                    }

                case ChartLegendRepresentationType.Line:
                    {
                        m_baseItemStyle.Type = ChartLegendItemType.Line;
                        break;
                    }

                case ChartLegendRepresentationType.StraightLine:
                    {
                        m_baseItemStyle.Type = ChartLegendItemType.StraightLine;
                        break;
                    }

                case ChartLegendRepresentationType.Pentagon:
                    {
                        m_baseItemStyle.Type = ChartLegendItemType.Pentagon;
                        break;
                    }

                case ChartLegendRepresentationType.Triangle:
                    {
                        m_baseItemStyle.Type = ChartLegendItemType.Triangle;
                        break;
                    }

                case ChartLegendRepresentationType.Cross:
                    {
                        m_baseItemStyle.Type = ChartLegendItemType.Cross;
                        break;
                    }
            }
        }

        /// <summary>
        /// Internal method. Please do not call from your code.
        /// </summary>
        /// <param name="size">The Size.</param>
        /// <returns>Returns SizeF.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override SizeF Measure(SizeF size)
        {
            m_measuredSize = Size.Ceiling(this.MeasureItems(this.GetGraphics(), size.ToSize()));

            this.Dock = DockStyle.None;

            if (m_floatingAutoSize || this.Position != ChartDock.Floating)
            {
                this.Size = m_measuredSize;
            }
            else
            {
                this.Size = new Size(Math.Max(m_measuredSize.Width, this.Size.Width),
                    Math.Max(m_measuredSize.Height, this.Size.Height));
            }

            return this.Size;
        }

        /// <summary>
        /// Draws the legend by the specified <see cref="PaintEventArgs"/>.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        internal void Draw(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int visibleItemsCout = this.VisibleItemsCount;

            if (this.BackgroundImage == null)
            {
                BrushPaint.FillRectangle(g, this.ClientRectangle, this.BackInterior);
            }

            if (m_chart != null)
            {
                g.SmoothingMode = m_chart.SmoothingMode;
                g.TextRenderingHint = m_chart.TextRenderingHint;
            }

            SizeF textSize = e.Graphics.MeasureString(this.Text, this.Font, this.Width);
            RectangleF titleRect = new RectangleF(0, 0, this.Width, textSize.Height);
            RectangleF contentRect = new RectangleF(0, titleRect.Bottom, this.Width, this.Height - textSize.Height);
            ChartLegendItem[] items = Items;

            ////this.Controls.Clear();

            if (visibleItemsCout > 0)
            {
                int index = 0;
                bool onlyColumns = (this.Position == ChartDock.Floating) && m_olnyColumnsForFloating;
                bool isVO = Orientation == ChartOrientation.Vertical;
                int cols = Math.Min(visibleItemsCout, m_columnsCount);
                int rows = Math.Min(visibleItemsCout, (onlyColumns ? m_columnsCount : m_rowsCount));

                if (isVO)
                {
                    rows = (int)Math.Ceiling((float)visibleItemsCout / cols);
                }
                else
                {
                    cols = (int)Math.Ceiling((float)visibleItemsCout / rows);
                }

                contentRect.Inflate(-0.5f * m_spacing, -0.5f * m_spacing);
                SizeF cellSize = new SizeF(contentRect.Width / cols, contentRect.Height / rows);

                foreach (ChartLegendItem item in items)
                {
                    if (item.Visible)
                    {
                        int ci = isVO ? index / rows : index % cols;
                        int ri = isVO ? index % rows : index / cols;

                        SizeF itemSize = item.Measure(g);
                        RectangleF itemRect = new RectangleF(contentRect.Left + ci * cellSize.Width,
                          contentRect.Top + ri * cellSize.Height, cellSize.Width, cellSize.Height);

                        itemRect.Inflate(-0.5f * m_spacing, -0.5f * m_spacing);

                        #region Add checkbox
                        if (item.VisibleCheckBox && !this.IsWindowLess)
                        {
                            Point loc = new Point((int)itemRect.X, (int)(itemRect.Y + (itemRect.Height - c_checkBoxSize) / 2));

                            if (m_checkBoxes.ContainsKey(item))
                            {
                                m_checkBoxes[item].Location = loc;
                                m_checkBoxes[item].Visible = true;
                                m_checkBoxes[item].Checked = item.IsChecked;
                            }
                            else
                            {
                                m_checkBoxes.Add(item, this.AddCheckBox(loc, item));
                            }

                            itemRect.X += c_checkBoxSize + m_spacing;
                            itemRect.Width -= c_checkBoxSize + m_spacing;
                        }
                        else
                        {
                            if (m_checkBoxes.ContainsKey(item))
                            {
                                m_checkBoxes[item].Visible = false;
                            }
                        }
                        #endregion

                        switch (m_itemsAlignment)
                        {
                            case StringAlignment.Center:
                                itemRect.X += (itemRect.Width - itemSize.Width) / 2;
                                break;
                            case StringAlignment.Far:
                                itemRect.X += itemRect.Width - itemSize.Width;
                                break;
                        }

                        item.Arrange(itemRect);

                        ChartLegendDrawItemEventArgs drawArgs = new ChartLegendDrawItemEventArgs(e.Graphics, item, Rectangle.Round(itemRect), index);
                        this.RaiseDrawItem(drawArgs);
						
						if (this.DrawItemText  != null )
                           item.m_textHandler = this.DrawItemText;

                        if (!drawArgs.Handled)
                        {
                            item.Draw(g);
                        }

                        index++;
                    }
                    else
                    {
                        if (m_checkBoxes.ContainsKey(item))
                        {
                            m_checkBoxes[item].Visible = false;
                        }
                    }
                }
            }
            else
            {
                #region Draw empty legend
                StringFormat emptyFormat = new StringFormat();

                emptyFormat.LineAlignment = StringAlignment.Center;
                emptyFormat.Alignment = StringAlignment.Center;

                using (SolidBrush sb = new SolidBrush(this.ForeColor))
                {
                    g.DrawString(c_emptyText, this.Font, sb, contentRect, emptyFormat);
                }
                #endregion
            }

            #region Draw title
            if (this.Text != null && this.Text != string.Empty)
            {
                StringFormat titleFormat = new StringFormat(StringFormatFlags.NoClip);
                titleFormat.Alignment = m_textAlignment;

                using (SolidBrush sb = new SolidBrush(this.ForeColor))
                {
                    g.DrawString(this.Text, this.Font, sb, titleRect, titleFormat);
                }

                if (m_showBorder)
                {
                    g.DrawLine(this.Border.Pen, 0, textSize.Height, this.Width, textSize.Height);
                }
            }
            #endregion

            #region Draw border of legend
            if (m_showBorder)
            {
                g.DrawRectangle(this.Border.Pen, 0, 0, this.Width - 1, this.Height - 1);
            }
            #endregion
        }

        /// <summary>
        /// Measures necessary size of legend.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="maxSize">Size of the max.</param>
        /// <returns>Return the SizeF type element.</returns>
        private SizeF MeasureItems(Graphics g, Size maxSize)
        {
            SizeF itemSize = SizeF.Empty;
            SizeF textSize = g.MeasureString(this.Text, this.Font, maxSize.Width, c_stringFormat);
            int visibleItemsCout = this.VisibleItemsCount;

            #region Get items size
            foreach (ChartLegendItem item in this.Items)
            {
                if (item.Visible)
                {
                    SizeF sz = item.Measure(g);

                    if (item.VisibleCheckBox && !this.IsWindowLess)
                    {
                        sz = new SizeF(sz.Width + m_spacing + c_checkBoxSize, Math.Max(sz.Height, c_checkBoxSize));
                    }

                    itemSize.Width = Math.Max(sz.Width, itemSize.Width);
                    itemSize.Height = Math.Max(sz.Height, itemSize.Height);
                }
            }

            ChartLegendMinSizeEventArgs minSizeArgs = new ChartLegendMinSizeEventArgs(itemSize.ToSize());
            this.RaiseMinSize(minSizeArgs);

            if (minSizeArgs.Handled)
            {
                itemSize = new SizeF(minSizeArgs.Size.Width, minSizeArgs.Size.Height);
            }
            #endregion

            #region Compute the necessary size
            if (visibleItemsCout > 0)
            {
                bool onlyColumns = (this.Position == ChartDock.Floating) && m_olnyColumnsForFloating;
                int cols = Math.Min(visibleItemsCout, m_columnsCount);
                int rows = Math.Min(visibleItemsCout, (onlyColumns ? m_columnsCount : m_rowsCount));

                if (Orientation == ChartOrientation.Vertical)
                {
                    rows = (int)Math.Ceiling((float)visibleItemsCout / cols);
                }
                else
                {
                    cols = (int)Math.Ceiling((float)visibleItemsCout / rows);
                }

                itemSize = new SizeF((itemSize.Width + m_spacing) * cols + m_spacing,
                  (itemSize.Height + m_spacing) * rows + m_spacing);
            }
            else
            {
                itemSize = g.MeasureString(c_emptyText, this.Font);
                itemSize = new SizeF(itemSize.Width + 2 * m_spacing, itemSize.Height + 2 * m_spacing);
            }
            #endregion

            return new SizeF(Math.Min(maxSize.Width, Math.Max(textSize.Width, itemSize.Width)),
              Math.Min(maxSize.Height, itemSize.Height + textSize.Height));
        }

        /// <summary>
        /// Override on the standard method of class control.
        /// Event handler to event Paint.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void OnPaint(PaintEventArgs e)
        {
            this.Draw(e);
            base.OnPaint(e);
        }

        /// <summary>
        /// Computes all items of this legend (Custom items + series items).
        /// </summary>
        /// <returns>Returns ChartLegendItem.</returns>
        private ChartLegendItem[] GetAllItems()
        {
            ChartLegendItemsCollection items = new ChartLegendItemsCollection();

            this.Controls.Clear();
            m_checkBoxes.Clear();

            if (m_chart != null)
            {
                for (int i = 0; i < m_chart.Series.Count; i++)
                {
                    ChartSeries series = m_chart.Series[i];

                    if ((series.LegendName == this.Name) && series.Compatible)
                    {                        
                        if (series.BaseType == ChartSeriesBaseType.Single && !m_chart.ChartArea.DivideArea && !m_chart.ChartArea.MultiplePies)
                        {
													if (series.LegendItem.Children.Count > 0)
													{
                            foreach (ChartSeriesLegendItem cli in series.LegendItem.Children)
                            {
                                cli.ItemStyle.BaseStyle = m_baseItemStyle;
                                cli.DrawSeriesIcon = m_representationType == ChartLegendRepresentationType.SeriesType;
                                items.Add(cli);
                            }
													}
													else
													{
														series.LegendItem.ItemStyle.BaseStyle = m_baseItemStyle;
														series.LegendItem.DrawSeriesIcon = m_representationType == ChartLegendRepresentationType.SeriesType;
														items.Add(series.LegendItem);
													}

                            break;
                        }
                        else
                        {
                            series.LegendItem.ItemStyle.BaseStyle = m_baseItemStyle;
                            series.LegendItem.DrawSeriesIcon = m_representationType == ChartLegendRepresentationType.SeriesType;
                            items.Add(series.LegendItem);
                        }
                    }
                }
            }

            if (m_customItems != null)
            {
                for (int i = 0; i < m_customItems.Length; i++)
                {
                    m_customItems[i].ItemStyle.BaseStyle = m_baseItemStyle;
                    items.Add(m_customItems[i]);
                }
            }

            ChartLegendFilterItemsEventArgs filterArgs = new ChartLegendFilterItemsEventArgs(items);

            this.RaiseFilterItems(filterArgs);

            ChartLegendItem[] result = filterArgs.Items.ToArray();

            foreach (ChartLegendItem cli in result)
            {
                cli.ItemStyle.BaseStyle = m_baseItemStyle;
            }

            return result;
        }

        #region Mouse handlers
        /// <summary>
        /// Override the <see cref="OnDoubleClick"/> method.
        /// </summary>
        /// <param name="e">The EventArgument type args.</param>
        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            if (button == MouseButtons.Left)
            {
                bool isItemClick = false;

                if ((m_style & ChartLegendStyle.SupportItemStyleEdit) == ChartLegendStyle.SupportItemStyleEdit)
                {
                    isItemClick = OnSeriesItemDoubleClick();
                }

                if (!isItemClick &&
                  (m_style & ChartLegendStyle.SupportEditProperties) == ChartLegendStyle.SupportEditProperties)
                {
                    if (m_propertiesEditDialog == null)
                    {
                        m_propertiesEditDialog = new ChartLegendPropertiesDialog();
                    }

                    m_propertiesEditDialog.SetLegendInfo(this);
                    m_propertiesEditDialog.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Override the <see cref="OnMouseDown"/> method.
        /// </summary>
        /// <param name="e">The EventArgument type args.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            button = e.Button;

            if (button == MouseButtons.Left)
            {
                mouseCatch = true;
                pointCatch = new Point(e.X, e.Y);
            }

            base.OnMouseDown(e);
        }

        /// <summary>
        /// Override the <see cref="OnMouseMove"/> method.
        /// </summary>
        /// <param name="e">The EventArgument type args.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {                                    
            Point mousePt = PointToClient(Control.MousePosition);
            ChartSeriesLegendItem li = this.GetItemBy(mousePt) as ChartSeriesLegendItem;
            int seriesIndex = -1;

            if (m_chart.SeriesHighlight && m_chart.CalcRegions)
            {
                if (li != null && li.Series != null)
                {
                    seriesIndex = m_chart.Series.IndexOf(li.Series);                    
                    m_chart.SeriesHighlightIndex = seriesIndex;                    
                }
            }            

            if (mouseCatch)
            {
                if (m_isResizing)
                {
                    Size size = Size.Empty;

                    #region Legend size change
                    if (Cursor == Cursors.SizeNWSE)
                    {
                        size = new Size(e.X, e.Y);
                    }
                    else if (Cursor == Cursors.SizeWE)
                    {
                        size = new Size(e.X > m_baseItemStyle.RepresentationSize.Width ? e.X : m_baseItemStyle.RepresentationSize.Width, Size.Height);
                    }
                    else if (Cursor == Cursors.SizeNS)
                    {
                        size = new Size(Size.Width, e.Y > m_baseItemStyle.RepresentationSize.Height ? e.Y : m_baseItemStyle.RepresentationSize.Height);
                    }
                    #endregion

                    this.Size = new Size(Math.Max(m_measuredSize.Width, size.Width),
                        Math.Max(m_measuredSize.Height, size.Height));
                }
            }
            else
            {
                #region Set cursors
                if ((!m_floatingAutoSize) && (this.Position == ChartDock.Floating))
                {
                    bool hResizing = e.X > Size.Width - Cursor.Size.Width / c_divCursorCoef;
                    bool vResizing = e.Y > Size.Height - Cursor.Size.Height / c_divCursorCoef;

                    m_isResizing = hResizing || vResizing;
                    m_canCursorChange = !(hResizing || vResizing);

                    if (hResizing && vResizing)
                    {
                        this.Cursor = Cursors.SizeNWSE;
                    }
                    else if (hResizing)
                    {
                        this.Cursor = Cursors.SizeWE;
                    }
                    else if (vResizing)
                    {
                        this.Cursor = Cursors.SizeNS;
                    }
                    else
                    {
                        this.Cursor = m_tempCursor;
                    }
                }
                #endregion
            }

            if (!m_isResizing)
            {
                base.OnMouseMove(e);
            }
        }

        /// <summary>
        /// Override the <see cref="OnMouseLeave"/> method.
        /// </summary>
        /// <param name="e">The EventArgument type args.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            m_chart.SeriesHighlightIndex = -1;
            base.OnMouseLeave(e);
        }
        /// <summary>
        /// Override the <see cref="OnMouseUp"/> method.
        /// </summary>
        /// <param name="e">The EventArgument type args.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            mouseCatch = false;
            base.OnMouseUp(e);
        }
        #endregion

        #region Properties changing handlers
        /// <summary>
        /// Override the <see cref="Control.OnTextChanged"/> method.
        /// </summary>
        /// <param name="e">The EventArgument type args.</param>
        protected override void OnTextChanged(EventArgs e)
        {
            OnRecalculateSizes();
            base.OnTextChanged(e);
        }

        /// <summary>
        /// Override the <see cref="Control.OnCursorChanged"/> method.
        /// </summary>
        /// <param name="e">The EventArgument type args.</param>
        protected override void OnCursorChanged(EventArgs e)
        {
            if (m_canCursorChange)
            {
                m_tempCursor = Cursor;
            }

            base.OnCursorChanged(e);
        }

        /// <summary>
        /// Override the <see cref="Control.OnLocationChanged"/> method.
        /// </summary>
        /// <param name="e">The EventArgument type args.</param>
        protected override void OnLocationChanged(EventArgs e)
        {
            if (this.BackColor.A != 255)
            {
                this.Invalidate();
            }

            if (this.Position == ChartDock.Floating)
            {
                base.OnLocationChanged(e);
            }
        }

        /// <summary>
        /// Override the <see cref="Control.OnParentChanged"/> method.
        /// </summary>
        /// <param name="e">The EventArgument type args.</param>
        protected override void OnParentChanged(EventArgs e)
        {
            if (m_chart != null)
            {
                this.UnWireSeriesCollection();
            }

            m_chart = this.Parent as ChartControl;

            if (m_chart != null)
            {
                this.WireSeriesCollection();
            }

            m_needRefreshItems = true;
            base.OnParentChanged(e);
        }
        #endregion

        #region Event raisers
        /// <summary>
        /// This method is the calling method of OnDrawItem event.
        /// It is called in the method ChartLegend.GetMinSize().
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Forms.Chart.ChartLegendDrawItemEventArgs"/> instance containing the event data.</param>
        private void RaiseDrawItem(ChartLegendDrawItemEventArgs e)
        {
            if (this.DrawItem != null)
            {
                this.DrawItem(this, e);
            }
        }

        /// <summary>
        /// This method is the calling method of OnFilterItems event.
        /// It is called in the method ChartLegend.GetMinSize().
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Forms.Chart.ChartLegendFilterItemsEventArgs"/> instance containing the event data.</param>
        private void RaiseFilterItems(ChartLegendFilterItemsEventArgs e)
        {
            if (this.FilterItems != null)
            {
                FilterItems(this, e);
            }
        }

        /// <summary>
        /// This method is the calling method of OnMinSize event.
        /// It is called in the method ChartLegend.GetMinSize().
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Forms.Chart.ChartLegendMinSizeEventArgs"/> instance containing the event data.</param>
        private void RaiseMinSize(ChartLegendMinSizeEventArgs e)
        {
            if (this.MinSize != null)
            {
                MinSize(this, e);
            }
        }
        #endregion

        /// <summary>
        /// This method is called when size of legend was changed.
        /// </summary>
        private void OnLegentSizeChange()
        {
            if (m_position != ChartDock.Floating)
            {
                if (m_chart != null)
                {
                    this.m_chart.Redraw(true);
                }
            }
            else
            {
                if (this.Parent != null)
                {
                    this.Measure(this.Parent.Size);
                }

                this.Invalidate();
            }
        }

        /// <summary>
        /// This methods is called when size of legend was changed.
        /// </summary>
        private void OnRecalculateSizes()
        {
            if (m_chart != null)
            {
                this.m_chart.Redraw(true);
            }
        }

        /// <summary>
        /// This methods is called when settings of border was changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void BorderSettingsChanged(object sender, EventArgs e)
        {
            this.m_chart.Refresh();
        }

        /// <summary>
        /// This method is called when the series item in the legend is double clicked.
        /// </summary>
        /// <returns>Returns true when the item is clicked otherwise false.</returns>
        protected bool OnSeriesItemDoubleClick()
        {
            bool isOnItemClick = false;
            Point mousePt = PointToClient(Control.MousePosition);

            ChartSeriesLegendItem li = this.GetItemBy(mousePt) as ChartSeriesLegendItem;

            if (li != null && li.Series != null)
            {
                m_chart.DisplayUserEditStylesDialog(m_chart.Series.IndexOf(li.Series));
                isOnItemClick = true;
            }

            return isOnItemClick;
        }

        /// <summary>
        /// Adds checkbox to legend by specified <see cref="ChartLegendItem"/>.
        /// </summary>
        /// <param name="loc">The location.</param>
        /// <param name="li">The ChartLegendItem.</param>
        /// <returns>Returns the CheckBox.</returns>
        private CheckBox AddCheckBox(Point loc, ChartLegendItem li)
        {
            CheckBox cb = new CheckBox();

            cb.Location = loc;
            cb.Checked = li.IsChecked;
            cb.BackColor = Color.White;

            cb.Size = new Size(c_checkBoxSize, c_checkBoxSize);
            cb.Tag = li;
            cb.CheckedChanged += new EventHandler(CheckBoxCheckedChanged);

            this.Controls.Add(cb);

            return cb;
        }

        /// <summary>
        /// Called when user click on the checkbox.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CheckBoxCheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            ChartLegendItem cli = checkBox.Tag as ChartLegendItem;

            if (cli != null)
            {
                cli.IsChecked = checkBox.Checked;
            }

            this.OnRecalculateSizes();
        }

        /// <summary>
        /// Called when collection of series was changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Forms.Chart.ChartSeriesCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void SeriesChanged(object sender, ChartSeriesCollectionChangedEventArgs e)
        {
            m_needRefreshItems = true;
        }

        /// <summary>
        /// Wires series collection to the legend.
        /// </summary>
        private void WireSeriesCollection()
        {
            this.m_chart.Series.Changed += new ChartSeriesCollectionChangedEventHandler(SeriesChanged);
        }

        /// <summary>
        /// Unwires series collection from the legend.
        /// </summary>
        private void UnWireSeriesCollection()
        {
            if (this.m_chart != null)
            {
                this.m_chart.Series.Changed -= new ChartSeriesCollectionChangedEventHandler(SeriesChanged);
            }
        }
        #endregion
    }
}
