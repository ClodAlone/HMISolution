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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// The ChartToolBarInfo class which stores the information of ToolBar.
    /// </summary>
    /// <internalonly/>
    [DocumentationExclude()]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ChartToolBarInfo
    {
        #region Members
        private ChartToolBar m_toolBar;
        private bool m_isDefaultItems = true;
        private bool m_showDialog = false;
        #endregion

        #region Events

        /// <summary>
        /// Occurs when item is clicked.
        /// </summary>
        public event EventHandler ItemClick
        {
            add
            {
                m_toolBar.ItemClick += value;
            }

            remove
            {
                m_toolBar.ItemClick -= value;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the location of the toolbar.
        /// </summary>
        [Description("Indicates the location of toolbar")]
        public Point Location
        {
            get
            {
                return m_toolBar.Location;
            }

            set
            {
                m_toolBar.Location = value;
            }
        }

        /// <summary>
        /// Gets or sets the button size of the toolbar buttons.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the size of the toolbar items")]
        public Size ButtonSize
        {
            get
            {
                return m_toolBar.ButtonSize;
            }

            set
            {
                m_toolBar.ButtonSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the toolbar.
        /// </summary>
        [DefaultValue(ChartOrientation.Horizontal)]
        [Description("Indicates the orientation of toolbar")]
        public ChartOrientation Orientation
        {
            get
            {
                return m_toolBar.Orientation;
            }

            set
            {
                m_toolBar.Orientation = value;
            }
        }

        /// <summary>
        /// Gets or sets the spacing between items.
        /// </summary>
        [DefaultValue(0)]
        [Description("Indicates the spacing between items.")]
        public int Spacing
        {
            get
            {
                return m_toolBar.Spacing;
            }

            set
            {
                m_toolBar.Spacing = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this element can be resized automatically.
        /// </summary>
        [DefaultValue(true)]
        [Description("Indicates if this element can be resized automatically.")]
        public bool AutoSize
        {
            get
            {
                return m_toolBar.AutoSize;
            }

            set
            {
                m_toolBar.AutoSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the toolbar button.
        /// </summary>
        [Description("Indicates the size of toolbar.")]
        public Size Size
        {
            get
            {
                return m_toolBar.Size;
            }

            set
            {
                if (!m_toolBar.AutoSize)
                {
                    m_toolBar.Size = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the backcolor of the ToolBar.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the backcolor of toolbar.")]
        public Color BackColor
        {
            get
            {
                return m_toolBar.BackColor;
            }

            set
            {
                m_toolBar.BackColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of the header.
        /// </summary>
        [DefaultValue(0)]
        [Description("Indicates the height of the header.")]
        public int Header
        {
            get
            {
                return m_toolBar.Header;
            }

            set
            {
                m_toolBar.Header = value;
            }
        }

        /// <summary>
        /// Gets the information that is to be used for drawing border.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Content), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Contains the information that is to be used for drawing border.")]
        public LineInfo Border
        {
            get
            {
                return m_toolBar.Border;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether show the border.
        /// </summary>
        /// <value><c>true</c> if [show border]; otherwise, <c>false</c>.</value>
        [DefaultValue(true), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the visibility of border.")]
        public bool ShowBorder
        {
            get
            {
                return m_toolBar.ShowBorder;
            }

            set
            {
                m_toolBar.ShowBorder = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the ToolBarPropertyDialog is to be shown or not on double click.
        /// </summary>
        [DefaultValue(false), ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates whether the ToolBarPropertyDialog is to be shown or not on double click.")]
        public bool ShowDialog
        {
            get
            {
                return m_showDialog;
            }

            set
            {
                if (m_showDialog != value)
                {
                    m_showDialog = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the toolbar is to be shown.
        /// </summary>
        [DefaultValue(false)]
        [Description("Indicates the visibility of toolbar.")]
        public bool Visible
        {
            get
            {
                return m_toolBar.Visible;
            }

            set
            {
                m_toolBar.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets the flatstyle appearance for the toolbar button control.
        /// </summary>
        [DefaultValue(FlatStyle.Flat)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        [Obsolete("This property isn't used anymore")]
        public FlatStyle ButtonFlatStyle
        {
            get
            {
                return m_toolBar.ButtonFlatStyle;
            }

            set
            {
                m_toolBar.ButtonFlatStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the BackInterior of the toolBar button.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the background color of toolbar items.")]
        public Color ButtonBackColor
        {
            get
            {
                return m_toolBar.ButtonBackColor;
            }

            set
            {
                m_toolBar.ButtonBackColor = value;
            }
        }

        /// <summary>
        /// Gets the toolbar buttons collection.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Obsolete("Use the Items collection"), EditorBrowsable(EditorBrowsableState.Never)]
        public ChartToolBarButtonCollection Buttons
        {
            get
            {
                return m_toolBar.Buttons;
            }
        }

        /// <summary>
        /// Gets or sets the ForeColor of the toolBar button.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple)]
        [Description("Indicates the foreground color of toolbar items.")]
        public Color ButtonForeColor
        {
            get
            {
                return m_toolBar.ButtonForeColor;
            }

            set
            {
                m_toolBar.ButtonForeColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the docking position of the ToolBar.
        /// </summary>
        [DefaultValue(ChartDock.Top)]
        [Description("Indicates the dock position of toolbar.")]
        public ChartDock Position
        {
            get
            {
                return m_toolBar.Position;
            }

            set
            {
                m_toolBar.Position = value;
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the ToolBar.
        /// </summary>
        [DefaultValue(ChartAlignment.Center)]
        [Description("Indicates the alignment of toolbar.")]
        public ChartAlignment Alignment
        {
            get
            {
                return m_toolBar.Alignment;
            }

            set
            {
                m_toolBar.Alignment = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the ToolBar is to be held docked.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Indicates if the control should be docked inside the Chart")]
        public bool DockingFree
        {
            get
            {
                return m_toolBar.DockingFree;
            }

            set
            {
                m_toolBar.DockingFree = value;
            }
        }

        /// <summary>
        /// Gets or sets the docking behaviour.
        /// </summary>
        /// <value></value>
        [DefaultValue(ChartDockingFlags.All)]
        [Description("Indicates behavior of the dock control")]
        public ChartDockingFlags Behavior
        {
            get
            {
                return m_toolBar.Behavior;
            }

            set
            {
                m_toolBar.Behavior = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether grip is shown.
        /// </summary>
        /// <value><c>true</c> if grip is shown; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        [Description("Indicates the visibility of toolbar grip.")]
        public bool ShowGrip
        {
            get { return m_toolBar.ShowGrip; }

            set { m_toolBar.ShowGrip = value; }
        }

        /// <summary>
        /// Gets or sets the padding.
        /// </summary>
        /// <value>The padding.</value>
        [DefaultValue(2)]
        [Description("Indicates the spacing between items and border.")]
        public int Padding
        {
            get { return m_toolBar.Padding; }

            set { m_toolBar.Padding = value; }
        }

        /// <summary>
        /// Gets or sets the icon padding of items.
        /// </summary>
        /// <value>The icon padding.</value>
        [DefaultValue(2)]
        [Description("Indicates the margin of item images.")]
        public int IconPadding
        {
            get { return m_toolBar.IconPadding; }

            set { m_toolBar.IconPadding = value; }
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        [Editor(typeof(ChartToolBarItemCollectionEditor), typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(CollectionConverter))]
        [Description("Collection of toolbar items.")]
        public ChartToolBarItemCollection Items
        {
            get
            {
                return m_toolBar.Items;
            }
        }

        /// <summary>
        /// Conceals control from the user.
        /// </summary>
        public void Hide()
        {
            // m_toolBar.SuspendLayout();
            m_toolBar.Hide();
        }

        /// <summary>
        /// Displays control to the user.
        /// </summary>
        public void Show()
        {
            // m_toolBar.ResumeLayout();
            m_toolBar.Show();
        }

        /// <summary>
        /// Rewires the items.
        /// </summary>
        internal void RewireItems()
        {
            foreach (ChartToolBarItemBase item in m_toolBar.Items)
            {
                item.SetOwner(m_toolBar);
            }
        }

        #region ShouldSerialize
        /// <summary>
        /// Indicates that Border property was changed.
        /// </summary>
        /// <returns>True if property was changed, otherwise false.</returns>
        protected bool ShouldSerializeBorder()
        {
            return true;
        }

        /// <summary>
        /// Indicates if ButtonBackColor is set to default.
        /// </summary>
        /// <returns>Returns true if the element should serialize otherwise false.</returns>
        protected bool ShouldSerializeButtonBackColor()
        {
            return m_toolBar.ButtonBackColor != Color.Transparent;
        }

        /// <summary>
        /// Indicates if BackInterior of toolbar is set as parent.
        /// </summary>
        /// <returns>Returns true if the element should serialize otherwise false.</returns>
        protected bool ShouldSerializeBackColor()
        {
            return BackColor != m_toolBar.Parent.BackColor;
        }

        /// <summary>
        /// Indicates if ButtonForeColor is set to default.
        /// </summary>
        /// <returns>Returns true if the element should serialize otherwise false.</returns>
        protected bool ShouldSerializeButtonForeColor()
        {
            return ButtonForeColor != Color.Transparent;
        }

        /// <summary>
        /// Indicates if button size is set to default.
        /// </summary>
        /// <returns>Returns true if the element should serialize otherwise false.</returns>
        protected bool ShouldSerializeButtonSize()
        {
            return ButtonSize != new Size(22, 22);
        }

        /// <summary>
        /// Indicates if autosize is set to default.
        /// </summary>
        /// <returns>Returns true if the element should serialize otherwise false.</returns>
        protected bool ShouldSerializeSize()
        {
            return !m_toolBar.AutoSize;
        }

        /// <summary>
        /// Indicates if the location of the toolbar is set to default.
        /// </summary>
        /// <returns>Returns true if the element should serialize otherwise false.</returns>
        protected bool ShouldSerializeLocation()
        {
            return Position == ChartDock.Floating;
        }

        /// <summary>
        /// Indicates if the items of the toolbar is set to default.
        /// </summary>
        /// <returns>Returns true if the element should serialize otherwise false.</returns>
        internal bool ShouldSerializeItems()
        {
            return !m_isDefaultItems;
        }

        /// <summary>
        /// Resets the items.
        /// </summary>
        internal void ResetItems()
        {
            this.Items.Clear();
            this.Items.Add(new ChartToolBarSaveItem());
            this.Items.Add(new ChartToolBarCopyItem());
            this.Items.Add(new ChartToolBarPrintItem());
            this.Items.Add(new ChartToolBarPrintPreviewItem());
            this.Items.Add(new ChartToolBarSplitter());
            this.Items.Add(new ChartToolBarPaletteItem());
            this.Items.Add(new ChartToolBarStyleItem());
            this.Items.Add(new ChartToolBarTypeItem());
            this.Items.Add(new ChartToolBarSeries3DItem());
            this.Items.Add(new ChartToolBarShowLegendItem());
            m_isDefaultItems = true;
        }

        /// <summary>
        /// Gets default location.
        /// </summary>
        protected Point DefaultLocation
        {
            get
            {
                return Point.Empty;
            }
        }
        #endregion

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the ChartToolBarInfo class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        internal ChartToolBarInfo(ChartToolBar parent)
        {
            m_toolBar = parent;

            this.ResetItems();
            //this.Items.Changed += new ChartListChangeHandler(OnItemsChanged);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Called when items is changed.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="args">The args.</param>
        private void OnItemsChanged(ChartBaseList list, ChartListChangeArgs args)
        {
            if (m_isDefaultItems)
            {
                m_isDefaultItems = false;
                this.Items.Clear();

                if (args.NewItems != null)
                {
                    foreach (ChartToolBarItemBase item in args.NewItems)
                    {
                        this.Items.Add(item);
                    }
                }
            }
        }
        #endregion
    }
}
