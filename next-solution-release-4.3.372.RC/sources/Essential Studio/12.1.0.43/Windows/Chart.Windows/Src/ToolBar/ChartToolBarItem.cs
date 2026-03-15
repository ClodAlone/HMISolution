#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Specifies the abstract class for implementation of toolbar items.
    /// </summary>
    public abstract class ChartToolBarItemBase
    {
        #region Members
        private string m_name = "";
        private string m_toolTip = "";
        private object m_tag = null;

        private ChartToolBar m_owner = null;
        private Rectangle m_bounds = Rectangle.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), DefaultValue(null)]
        public object Tag
        {
            get
            {
                return m_tag;
            }

            set
            {
                m_tag = value;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name
        {
            get
            {
                return m_name;
            }

            set
            {
                m_name = value;
            }
        }

        /// <summary>
        /// Gets or sets the tool tip.
        /// </summary>
        /// <value>The tool tip.</value>
        public string ToolTip
        {
            get
            {
                return m_toolTip;
            }

            set
            {
                m_toolTip = value;
            }
        }

        /// <summary>
        /// Gets the default tool tip.
        /// </summary>
        /// <value>The default tool tip.</value>
        protected virtual string DefaultToolTip
        {
            get
            {
                return "";
            }
        }

        /// <summary>
        /// Gets the name of the default.
        /// </summary>
        /// <value>The name of the default.</value>
        protected virtual string DefaultName
        {
            get
            {
                return "";
            }
        }

        /// <summary>
        /// Gets the tool bar.
        /// </summary>
        /// <value>The tool bar.</value>
        internal ChartToolBar ToolBar
        {
            get
            {
                return m_owner;
            }
        }

        /// <summary>
        /// Gets the bounds.
        /// </summary>
        /// <value>The bounds.</value>
        protected internal Rectangle Bounds
        {
            get
            {
                return m_bounds;
            }
        }

        /// <summary>
        /// Gets the size of the desired.
        /// </summary>
        /// <value>The size of the desired.</value>
        protected internal virtual Size DesiredSize
        {
            get
            {
                if (m_owner != null)
                {
                    return m_owner.ButtonSize;
                }

                return Size.Empty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is focused.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is focused; otherwise, <c>false</c>.
        /// </value>
        protected bool IsFocused
        {
            get
            {
                return (m_owner != null) && (m_owner.FocusedItem == this);
            }
        }
        #endregion

        #region Conatructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartToolBarItemBase"/> class.
        /// </summary>
        public ChartToolBarItemBase()
        {
            m_toolTip = this.DefaultToolTip;
            m_name = this.DefaultName;
        }
        #endregion

        #region Serialization methods
        /// <summary>
        /// Should the serialize tool tip.
        /// </summary>
        /// <returns>Returns true if the element should serialize otherwise false.</returns>
        private bool ShouldSerializeToolTip()
        {
            return this.ToolTip != this.DefaultToolTip;
        }

        /// <summary>
        /// Resets the tool tip.
        /// </summary>
        private void ResetToolTip()
        {
            this.ToolTip = this.DefaultToolTip;
        }

        /// <summary>
        /// Should the serialize name.
        /// </summary>
        /// <returns>Returns true if the element should serialize otherwise false.</returns>
        private bool ShouldSerializeName()
        {
            return this.Name != this.DefaultName;
        }

        /// <summary>
        /// Resets the name.
        /// </summary>
        private void ResetName()
        {
            this.Name = this.DefaultName;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Sets the owner.
        /// </summary>
        /// <param name="toolBar">The tool bar.</param>
        internal void SetOwner(ChartToolBar toolBar)
        {
            m_owner = toolBar;
        }

        /// <summary>
        /// Arranges the specified rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        protected internal virtual void Arrange(Rectangle rect)
        {
            m_bounds = rect;
        }

        /// <summary>
        /// Draws the specified g.
        /// </summary>
        /// <param name="g">The g.</param>
        protected internal virtual void Draw(Graphics g)
        {
        }

        /// <summary>
        /// Called when it is clicked.
        /// </summary>
        /// <returns>Return true if clicked otherwise false.</returns>
        protected internal virtual bool Click()
        {
            return false;
        }
        #endregion
    }

    /// <summary>
    /// Specifies the toolbar splitter.
    /// </summary>
    [TypeConverter(typeof(ChartInstanceConverter))]
    public sealed class ChartToolBarSplitter : ChartToolBarItemBase
    {
        #region Constants
        private const int c_splitterSize = 3;
        #endregion

        #region Proeprties
        /// <summary>
        /// Gets the size of the desired.
        /// </summary>
        /// <value>The size of the desired.</value>
        protected internal override Size DesiredSize
        {
            get
            {
                if (this.ToolBar != null)
                {
                    bool isHor = this.ToolBar.Orientation == ChartOrientation.Horizontal;

                    if (isHor)
                    {
                        return new Size(c_splitterSize, this.ToolBar.ButtonSize.Height);
                    }
                    else
                    {
                        return new Size(this.ToolBar.ButtonSize.Width, c_splitterSize);
                    }
                }

                return Size.Empty;
            }
        }

        /// <summary>
        /// Gets the name of the default.
        /// </summary>
        /// <value>The name of the default.</value>
        protected override string DefaultName
        {
            get
            {
                return "Splitter";
            }
        }
        #endregion

        #region Implemetation
        /// <summary>
        /// Draws the specified g.
        /// </summary>
        /// <param name="g">The g.</param>
        protected internal override void Draw(Graphics g)
        {
            if (this.ToolBar != null)
            {
                Rectangle rect = Rectangle.Inflate(this.Bounds, -1, -1);
                bool isHor = this.ToolBar.Orientation == ChartOrientation.Horizontal;

                if (isHor)
                {
                    int x = rect.X + rect.Width / 2;
                    g.DrawLine(Pens.Gray, x, rect.Top, x, rect.Bottom - 1);
                }
                else
                {
                    int y = rect.Y + rect.Height / 2;
                    g.DrawLine(Pens.Gray, rect.Left, y, rect.Right - 1, y);
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents the default toolbar item.
    /// </summary>
    [TypeConverter(typeof(ChartInstanceConverter))]
    public class ChartToolBarItem : ChartToolBarItemBase
    {
        #region Members
        private Image m_image;

        private bool m_checked = false;
        private bool m_isCheckable = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ChartToolBarItem"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public virtual bool Checked
        {
            get
            {
                return m_checked;
            }

            set
            {
                m_checked = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checkable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is checkable; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public virtual bool IsCheckable
        {
            get
            {
                return m_isCheckable;
            }

            set
            {
                m_isCheckable = value;
            }
        }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        public virtual Image Image
        {
            get { return m_image; }

            set { m_image = value; }
        }

        /// <summary>
        /// Gets the default image.
        /// </summary>
        /// <value>The default image.</value>
        protected virtual Image DefaultImage
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is clicked.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is clicked; otherwise, <c>false</c>.
        /// </value>
        protected bool IsClicked
        {
            get
            {
                return this.IsFocused && Control.MouseButtons != MouseButtons.None;
            }
        }

        /// <summary>
        /// Gets <see cref="ChartControl"/> instance.
        /// </summary>
        /// <value>The chart.</value>
        protected ChartControl Chart
        {
            get
            {
                return this.ToolBar == null ? null : this.ToolBar.Parent as ChartControl;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartToolBarItem"/> class.
        /// </summary>
        public ChartToolBarItem()
        {
            m_image = this.DefaultImage;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Called when  <see cref="Checked"/> property is changed.
        /// </summary>
        protected virtual void OnCheckedChanged()
        {
        }

        /// <summary>
        /// Called when the item is clicked.
        /// </summary>
        protected virtual void OnClick()
        {
        }
        #endregion

        #region Serializarion methods
        /// <summary>
        /// Should the serialize image.
        /// </summary>
        /// <returns>Returns true if the element should serialize otherwise false.</returns>
        private bool ShouldSerializeImage()
        {
            return this.Image != this.DefaultImage;
        }

        /// <summary>
        /// Resets the image.
        /// </summary>
        private void ResetImage()
        {
            this.Image = this.DefaultImage;
        }
        #endregion

        #region Internal methods
        /// <summary>
        /// Arranges the specified rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        protected internal override void Arrange(Rectangle rect)
        {
            Size sz = this.DesiredSize;

            rect.X += (rect.Width - sz.Width) / 2;
            rect.Y += (rect.Height - sz.Height) / 2;
            rect.Size = sz;

            base.Arrange(rect);
        }

        /// <summary>
        /// Draws the background.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/>.</param>
        /// <param name="rect">The bounds of element.</param>
        /// <param name="focused">if set to <c>true</c> is item focused.</param>
        /// <param name="clicked">if set to <c>true</c> is item clicked.</param>
        protected void DrawBackground(Graphics g, Rectangle rect, bool focused, bool clicked)
        {
            using (SolidBrush sb = new SolidBrush(this.GetBackColor(focused, clicked)))
            {
                g.FillRectangle(sb, rect);
            }

            rect.Width -= 1;
            rect.Height -= 1;

            using (Pen pen = new Pen(this.GetForeColor(focused)))
            {
                g.DrawRectangle(pen, rect);
            }
        }

        /// <summary>
        /// Draws the icon.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="rect">The rect.</param>
        protected void DrawIcon(Graphics g, Rectangle rect)
        {
            if (this.Image != null)
            {
                int padding = this.ToolBar.IconPadding + 1;
                g.DrawImage(this.Image, Rectangle.Inflate(rect, -padding, -padding));
            }
        }

        /// <summary>
        /// Draws the item.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> instance.</param>
        protected internal override void Draw(Graphics g)
        {
            this.DrawBackground(g, this.Bounds, this.IsFocused || this.Checked, this.IsClicked);
            this.DrawIcon(g, this.Bounds);
        }

        /// <summary>
        /// Clicks this instance.
        /// </summary>
        protected internal override bool Click()
        {
            if (this.IsCheckable)
            {
                this.Checked = !this.Checked;
            }

            this.OnClick();

            return true;
        }

        /// <summary>
        /// Gets the color of the back.
        /// </summary>
        /// <param name="focused">if set to <c>true</c> [focused].</param>
        /// <param name="clicked">if set to <c>true</c> [clicked].</param>
        /// <returns>Returns the Back Color.</returns>
        protected Color GetBackColor(bool focused, bool clicked)
        {
            Color backColor = this.ToolBar == null ? Color.Empty : this.ToolBar.ButtonBackColor;

            if (focused || clicked)
            {
                int a = (backColor.A + 100) / 2;
                int r = backColor.R / 2;
                int g = backColor.G / 2;
                int b = (backColor.B + 255) / 2;

                if (clicked)
                {
                    a = (backColor.A + 150) / 2;
                }

                backColor = Color.FromArgb(a, r, g, b);
            }

            return backColor;
        }

        /// <summary>
        /// Gets the color of the back.
        /// </summary>
        /// <param name="focused">if set to <c>true</c> item is focused.</param>
        /// <returns>Returns the Fore color.</returns>
        protected Color GetForeColor(bool focused)
        {
            return focused ? Color.Blue :
                (this.ToolBar == null ? Color.Empty : this.ToolBar.ButtonForeColor);
        }
        #endregion
    }

    /// <summary>
    /// Specifies the toolbar item with the command.
    /// </summary>
    /// <seealso cref="ChartCommand"/>
    public class ChartToolBarCommandItem : ChartToolBarItem
    {
        #region Members
        private ChartCommand m_command;
        private string m_parameter;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>The command.</value>
        [Editor(typeof(ChartCommandEditor), typeof(UITypeEditor))]
        public ChartCommand Command
        {
            get { return m_command; }
            set { m_command = value; }
        }

        /// <summary>
        /// Gets or sets the parameter.
        /// </summary>
        /// <value>The parameter.</value>
        public string Parameter
        {
            get { return m_parameter; }
            set { m_parameter = value; }
        }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public override Image Image
        {
            get
            {
                return m_command != null ? m_command.Image : null;
            }

            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public override string Name
        {
            get
            {
                return m_command != null ? m_command.Name : base.Name;
            }

            set
            {
                base.Name = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is checkable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is checkable; otherwise, <c>false</c>.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public override bool IsCheckable
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
        /// Gets or sets a value indicating whether this <see cref="ChartToolBarItem"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public override bool Checked
        {
            get
            {
                if (m_command != null)
                {
                    return m_command.IsToggled(this.Chart, m_parameter);
                }

                return false;//// base.Checked;
            }

            set
            {
                ////base.Checked = value;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Called when the item is clicked.
        /// </summary>
        protected override void OnClick()
        {
            if (m_command != null)
            {
                m_command.Execute(this.Chart, m_parameter);
            }

            base.OnClick();
        }
        #endregion
    }

    /// <summary>
    /// Specifies the toolbar item with the drop-down menu.
    /// </summary>
    [TypeConverter(typeof(ChartInstanceConverter))]
    public class ChartToolBarDropDown : ChartToolBarItem
    {
        #region Constants
        private static Size c_dropDownIconSize = new Size(5, 3);
        #endregion

        #region Members
        /// <summary>
        /// The menu to drop-down;
        /// </summary>
        protected ContextMenu m_menu = new ContextMenu();
        #endregion

        #region Properites
        /// <summary>
        /// Gets the menu.
        /// </summary>
        /// <value>The menu.</value>
        public ContextMenu Menu
        {
            get
            {
                return m_menu;
            }
        }

        /// <summary>
        /// Gets the size of the desired.
        /// </summary>
        /// <value>The size of the desired.</value>
        protected internal override Size DesiredSize
        {
            get
            {
                Size size = base.DesiredSize;

                size.Width += c_dropDownIconSize.Width + 2 * this.ToolBar.IconPadding;
                size.Height = Math.Max(size.Height, c_dropDownIconSize.Height);

                return size;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Called when the item is clicked.
        /// </summary>
        protected override void OnClick()
        {
            if (m_menu != null)
            {
                m_menu.Show(this.ToolBar, new Point(this.Bounds.Left, this.Bounds.Bottom));
            }

            base.OnClick();
        }

        /// <summary>
        /// Draws the item.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> instance.</param>
        protected internal override void Draw(Graphics g)
        {
            bool focused = this.IsFocused || this.Checked;
            int padding = this.ToolBar.IconPadding;

            Rectangle symbolRect = this.Bounds;
            Rectangle imgRect = this.Bounds;

            symbolRect.X = symbolRect.Right - c_dropDownIconSize.Width - padding;
            symbolRect.Y = symbolRect.Bottom - c_dropDownIconSize.Height - padding;
            symbolRect.Size = c_dropDownIconSize;

            imgRect.Width -= c_dropDownIconSize.Width + 2 * padding;

            this.DrawBackground(g, this.Bounds, this.IsFocused || this.Checked, this.IsClicked);
            this.DrawDropDownIcon(g, symbolRect);
            this.DrawIcon(g, imgRect);
        }

        /// <summary>
        /// Draws the drop down icon.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="rect">The rect.</param>
        private void DrawDropDownIcon(Graphics g, Rectangle rect)
        {
            Point[] points = new Point[3];

            points[0] = new Point(rect.Left, rect.Top);
            points[1] = new Point(rect.Right, rect.Top);
            points[2] = new Point((rect.Left + rect.Right) / 2, rect.Bottom);

            g.FillPolygon(Brushes.Black, points);
        }
        #endregion
    }

    /// <summary>
    /// Represents the GUI editor of <see cref="ChartToolBarItemCollection"/> instances.
    /// </summary>
    class ChartToolBarItemCollectionEditor : CollectionEditor
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartToolBarItemCollectionEditor"/> class.
        /// </summary>
        public ChartToolBarItemCollectionEditor()
            : base(typeof(ChartToolBarItemCollection))
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the data types that this collection editor can contain.
        /// </summary>
        /// <returns>
        /// An array of data types that this collection can contain.
        /// </returns>
        protected override Type[] CreateNewItemTypes()
        {
            return new Type[]{ typeof(ChartToolBarItem),
												 typeof(ChartToolBarSplitter),
												 typeof(ChartToolBarCommandItem),
                         typeof(ChartToolBarSaveItem),
                         typeof(ChartToolBarCopyItem),
                         typeof(ChartToolBarPrintItem),
                         typeof(ChartToolBarPrintPreviewItem),
                         typeof(ChartToolBarPaletteItem),
                         typeof(ChartToolBarStyleItem),
                         typeof(ChartToolBarTypeItem),
                         typeof(ChartToolBarSeries3DItem),
                         typeof(ChartToolBarShowLegendItem)};
        }

        /// <summary>
        /// Gets the data type that this collection contains.
        /// </summary>
        /// <returns>
        /// The data type of the items in the collection, or an <see cref="T:System.Object"></see> if no Item property can be located on the collection.
        /// </returns>
        protected override Type CreateCollectionItemType()
        {
            return typeof(ChartToolBarItemBase);
        }
        #endregion
    }

    /// <summary>
    /// Specifies the collection of <see cref="ChartToolBarItemBase"/> objects.
    /// </summary>
    public class ChartToolBarItemCollection : ChartBaseList
    {
        #region Properties
        /// <summary>
        /// Gets or sets the <see cref="ChartToolBarItem"/> at the specified index.
        /// </summary>
        /// <value>The ChartToolBarItemCollection indexer. </value>
        /// <returns></returns>
        public ChartToolBarItemBase this[int index]
        {
            get
            {
                return List[index] as ChartToolBarItemBase;
            }

            set
            {
                List[index] = value;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The position into which the new element was inserted.</returns>
        public int Add(ChartToolBarItemBase value)
        {
            return List.Add(value);
        }

        /// <summary>
        /// Determines whether collection contains the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if collection  contains the specified value; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(ChartToolBarItemBase value)
        {
            return List.Contains(value);
        }

        /// <summary>
        /// Removes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Remove(ChartToolBarItemBase value)
        {
            List.Remove(value);
        }

        /// <summary>
        /// Gets indexes the of item.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The index of value if found in the list; otherwise, -1.</returns>
        public int IndexOf(ChartToolBarItemBase value)
        {
            return List.IndexOf(value);
        }

        /// <summary>
        /// Inserts item by the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public void Insert(int index, ChartToolBarItemBase value)
        {
            List.Insert(index, value);
        }
        #endregion
    }
}
