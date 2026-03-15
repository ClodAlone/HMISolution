#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Windows.Forms;

using Syncfusion.Drawing;
using Syncfusion.Styles;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    [
    TypeConverter(typeof(TreeColumnAdvConverter)),
    Serializable()
    ]
    public class TreeColumnAdv :
      MarshalByRefObject,
      ICloneable,
      IComparable,
      ISupportInitialize,
      ISerializable
    {
        #region Class constants
        /// <summary>
        /// TODO: place correct comment here
        /// </summary>
        public const int DefaultHeaderHeigh = 22;
        #endregion

        #region Class members

        /// <summary>Style storage for column.</summary>
        private TreeColumnAdvStyleInfo m_style;

        /// <summary>Reference on parent control.</summary>
        private MultiColumnTreeView m_tree;

        /// <summary>Storage of text bounds</summary>
        private Rectangle m_rcTextBounds;

        /// <summary>Column bounds</summary>
        private Rectangle m_rcBounds;

        /// <summary>Column state</summary>
        private bool m_bHighlighted = false;
        #endregion

        #region Class properties
        /// <summary>Gets reference on style that used by column.</summary>
        [
        Browsable(false)
        ]
        public TreeColumnAdvStyleInfo ColumnStyle
        {
            get
            {
                return m_style;
            }
        }

        /// <summary>Gets or sets a value indicating whether column selected state</summary>
        [
        Browsable(false)
        ]
        public bool Highlighted
        {
            get
            {
                return m_bHighlighted;
            }
            set
            {
                if (value != m_bHighlighted)
                {
                    m_bHighlighted = value;
                    Style_Changed(this, null);
                }
            }
        }

        [
        Browsable(false)
        ]
        public MultiColumnTreeView TreeView
        {
            get
            {
                return m_tree;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether column visible to user or not.
        /// </summary>
        [
        Description("Gets or sets a value indicating whether column visible to user or not."),
        Category("Behavior"),
        DefaultValue(true)
        ]
        public bool Visible
        {
            get
            {
                return this.ColumnStyle.Visible;
            }
            set
            {
                this.ColumnStyle.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets column width.
        /// </summary>
        [
        Description("Gets or sets column width"),
        Category("Behavior"),
        DefaultValue(60)
        ]
        public int Width
        {
            get
            {
                return this.ColumnStyle.Width;
            }
            set
            {
                this.ColumnStyle.Width = value;
            }
        }

        /// <summary>
        /// Gets or sets font that will be used for column text painting.
        /// </summary>
        [
        Description("Gets or sets font that will be used for column text painting"),
        Category("Appearance")
        ]
        public Font Font
        {
            get
            {
                return this.ColumnStyle.Font;
            }
            set
            {
                this.ColumnStyle.Font = value;
            }
        }

        /// <summary>
        /// Gets or sets the column text.
        /// </summary>
        [
        Description("Gets or sets the column text."),
        Category("Appearance"),
        Localizable(true),
        DefaultValue("")
        ]
        public string Text
        {
            get
            {
                return this.ColumnStyle.Text;
            }
            set
            {
                this.ColumnStyle.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the column text color.
        /// </summary>
        [
        Description("Gets or sets the column text color."),
        Category("Appearance"),
        DefaultValue(typeof(Color), "WindowText")
        ]
        public Color TextColor
        {
            get
            {
                return this.ColumnStyle.TextColor;
            }
            set
            {
                this.ColumnStyle.TextColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the column background style.
        /// </summary>
        [
        Description("Gets or sets the column background style."),
        Category("Appearance")
        ]
        public BrushInfo Background
        {
            get
            {
                return this.ColumnStyle.Background;
            }
            set
            {
                this.ColumnStyle.Background = value;
            }
        }

        /// <summary>
        /// Gets or sets height for column.
        /// </summary>
        internal int Height
        {
            get
            {
                return m_rcBounds.Height;
            }
            set
            {
                m_rcBounds.Height = value;
            }
        }

        [
        Description("Gets or sets the background for column area reserved by control."),
        Category("Appearance")
        ]
        public BrushInfo AreaBackground
        {
            get
            {
                return this.ColumnStyle.AreaBackground;
            }
            set
            {
                this.ColumnStyle.AreaBackground = value;
            }
        }

        /// <summary>
        /// Gets or sets hint text for the column.
        /// </summary>
        [
        Description("Gets or sets hint text for the column."),
        Category("Appearance"),
        Localizable(true),
        DefaultValue("")
        ]
        public string HelpText
        {
            get
            {
                return this.ColumnStyle.HelpText;
            }
            set
            {
                this.ColumnStyle.HelpText = value;
            }
        }

        /// <summary>
        /// Gets or sets sorting order of the column.
        /// </summary>
        [
        Description("Gets or sets sorting order of the column."),
        Category("Data")
        ]
        public SortOrder SortOrder
        {
            get
            {
                return this.ColumnStyle.SortOrder;
            }
            set
            {
                this.ColumnStyle.SortOrder = value;
            }
        }

        /// <summary>
        /// Gets or sets custom comparer that will be used for column value sorting.
        /// </summary>
        [
        Description("Gets or sets custom comparer that will be used for column value sorting."),
        Category("Data"),
        DefaultValue(null)
        ]
        public IComparer Comparer
        {
            get
            {
                return this.ColumnStyle.Comparer;
            }
            set
            {
                this.ColumnStyle.Comparer = value;
            }
        }

        /// <summary>
        /// Gets or sets the column user data.
        /// </summary>
        [
        Description("Gets or sets the column user data."),
        Category("Data"),
        DefaultValue(null),
        Browsable(false)
        ]
        public object Tag
        {
            get
            {
                return this.ColumnStyle.Tag;
            }
            set
            {
                this.ColumnStyle.Tag = value;
            }
        }

        /// <summary>
        /// Gets or sets image that will be painted from left side of the column text. 
        /// Image will be stretched  to column header height.
        /// </summary>
        [
        Description("Gets or sets image that will be painted from left side of the column text. "),
        Category("Images"),
        Localizable(true),
        DefaultValue(null)
        ]
        public Image LeftImage
        {
            get
            {
                return this.ColumnStyle.LeftImage;
            }
            set
            {
                this.ColumnStyle.LeftImage = value;
            }
        }

        /// <summary>
        /// Gets or sets image that will be painted from right side of the column text. 
        /// Image will be stretched  to column header height.
        /// </summary>
        [
        Description("Gets or sets image that will be painted from right side of the column text."),
        Category("Images"),
        Localizable(true),
        DefaultValue(null)
        ]
        public Image RightImage
        {
            get
            {
                return this.ColumnStyle.RightImage;
            }
            set
            {
                this.ColumnStyle.RightImage = value;
            }
        }

        [
        Description("Gets or sets the image index to be drawn on the left of the column`s text."),
        Category("Images")
        ]
        public int[] LeftImageIndices
        {
            get
            {
                return this.ColumnStyle.LeftImageIndices;
            }
            set
            {
                this.ColumnStyle.LeftImageIndices = value;
            }
        }
        [
        Description("Gets or sets the padding of left image for the column."),
        Category("Images"),
        Localizable(true),
        DefaultValue(0)
        ]
        public int LeftImagePadding
        {
            get
            {
                return this.ColumnStyle.LeftImagePadding;
            }
            set
            {
                this.ColumnStyle.LeftImagePadding = value;
            }
        }
        [
        Description("Gets or sets the image index to be drawn on the right of the column`s text."),
        Category("Images")
        ]
        public int[] RightImageIndices
        {
            get
            {
                return this.ColumnStyle.RightImageIndices;
            }
            set
            {
                this.ColumnStyle.RightImageIndices = value;
            }
        }
        [
        Description("Gets or sets the padding of tight image for the column."),
        Category("Images"),
        Localizable(true),
        DefaultValue(0)
        ]
        public int RightImagePadding
        {
            get
            {
                return this.ColumnStyle.RightImagePadding;
            }
            set
            {
                this.ColumnStyle.RightImagePadding = value;
            }
        }

        [
        Description("Gets or sets the sides of a rectangle to apply a three-dimensional border to."),
        Category("Borders"),
        Localizable(true),
        DefaultValue(Border3DSide.All)
        ]
        public Border3DSide BorderSides
        {
            get
            {
                return this.ColumnStyle.BorderSides;
            }
            set
            {
                this.ColumnStyle.BorderSides = value;
            }
        }
        [
        Description("Gets or sets the border style for the column."),
        Category("Borders"),
        Localizable(true),
        DefaultValue(BorderStyle.Fixed3D)
        ]
        public BorderStyle BorderStyle
        {
            get
            {
                return this.ColumnStyle.BorderStyle;
            }
            set
            {
                this.ColumnStyle.BorderStyle = value;
            }
        }
        [
        Description("Gets or sets the border 3dStyle for the column."),
        Category("Borders"),
        Localizable(true),
        DefaultValue(Border3DStyle.Etched)
        ]
        public Border3DStyle Border3DStyle
        {
            get
            {
                return this.ColumnStyle.Border3DStyle;
            }
            set
            {
                this.ColumnStyle.Border3DStyle = value;
            }
        }
        [
        Description("Gets or sets the border color for the column."),
        Category("Borders"),
        Localizable(true),
        DefaultValue(typeof(Color), "ControlDark")
        ]
        public Color BorderColor
        {
            get
            {
                return this.ColumnStyle.BorderColor;
            }
            set
            {
                this.ColumnStyle.BorderColor = value;
            }
        }

        [
        Description("Gets or sets the border style for the column."),
        Category("Borders"),
        Localizable(true),
        DefaultValue(ButtonBorderStyle.None)
        ]
        public ButtonBorderStyle BorderSingle
        {
            get
            {
                return this.ColumnStyle.BorderSingle;
            }
            set
            {
                this.ColumnStyle.BorderSingle = value;
            }
        }

        /// <summary>
        /// Gets or sets the base style for the column from which to inherit.
        /// </summary>
        /// <remarks>The specified base style should be available in the <see cref="MultiColumnTreeView.BaseStyles"/>
        /// collection.</remarks>
        [
        Description("The base style for the column"),
        Category("Appearance - Inherited"),
        DefaultValue(""),
        Editor(typeof(BaseStyleSelectorUITypeEditor), typeof(UITypeEditor))
        ]
        public string BaseStyle
        {
            get
            {
                return this.ColumnStyle.BaseStyle;
            }
            set
            {
                this.ColumnStyle.BaseStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment of text in column bounds.
        /// </summary>
        [
        Description("Gets or sets the vertical alignment of text in column bounds."),
        Category("Appearance"),
        Localizable(true),
        DefaultValue(StringAlignment.Center)
        ]
        public StringAlignment VerticalAlignment
        {
            get
            {
                return this.ColumnStyle.VerticalAlignment;
            }
            set
            {
                this.ColumnStyle.VerticalAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of the text in column bounds.
        /// </summary>
        [
        Description("Gets / sets the horizontal alignment of the text in column bounds."),
        Category("Appearance"),
        Localizable(true),
        DefaultValue(StringAlignment.Center)
        ]
        public StringAlignment HorizontalAlignment
        {
            get
            {
                return this.ColumnStyle.HorizontalAlignment;
            }
            set
            {
                this.ColumnStyle.HorizontalAlignment = value;
            }
        }

        [
        Description("Gets / sets the column text drawing mode."),
        Category("Appearance"),
        Localizable(true),
        DefaultValue(false)
        ]
        public bool AllowTextOverlap
        {
            get
            {
                return this.ColumnStyle.AllowTextOverlap;
            }
            set
            {
                this.ColumnStyle.AllowTextOverlap = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the highlight border.
        /// </summary>
        /// <value>The color of the highlight border.</value>
        [
        Description("Gets or sets the hightlight border color for the column."),
        Category("Borders"),
        Localizable(true),
        DefaultValue(typeof(Color), "Highlight")
        ]
        public Color HighlightBorderColor
        {
            get
            {
                return this.ColumnStyle.HighlightBorderColor;
            }
            set
            {
                if(this.ColumnStyle.HighlightBorderColor != value)
                    this.ColumnStyle.HighlightBorderColor = value;
            }
        }

        /// <summary>Gets the column size.</summary>
        [
        Browsable(false)
        ]
        public Size Size
        {
            get
            {
                return new Size(this.Width, this.Height);
            }
        }

        /// <summary>Gets or sets the column bounds.</summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public Rectangle Bounds
        {
            get
            {
                return m_rcBounds;
            }
            set
            {
                m_rcBounds = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether RTL drawing algorithm is used or not.
        /// </summary>
        internal bool IsMirrored
        {
            get
            {
                if (this.TreeView != null)
                {
                    return this.TreeView.GetIsMirrored();
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets area reserved for text drawing
        /// </summary>
        internal Rectangle TextBounds
        {
            get
            {
                return m_rcTextBounds;
            }
            set
            {
                m_rcTextBounds = value;
            }
        }

        /// <summary>
        /// Gets padding reserved for column border drawing.
        /// </summary>
        internal int BorderPadding
        {
            get
            {
                int offset = (this.BorderStyle == BorderStyle.Fixed3D) ? -2 :
                  ((this.BorderStyle == BorderStyle.FixedSingle) ? -1 : 0);

                return offset;
            }
        }

        /// <summary>
        /// Gets area reserved for background drawing.
        /// </summary>
        internal Rectangle BackgroundBounds
        {
            get
            {
                int offset = this.BorderPadding;
                return Rectangle.Inflate(this.Bounds, offset, offset);
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
   
        public TreeColumnAdv()
        {
            m_style = new TreeColumnAdvStyleInfo(new TreeColumnAdvStyleInfoIdentity(this));
            m_style.Changed += new StyleChangedEventHandler(Style_Changed);
        }

        /// <summary> Initializes a new instance of the TreeColumnAdv class.</summary>
        /// <param name="text">text of column.</param>
        public TreeColumnAdv(string text)
            : this()
        {
            this.Text = text;
        }

        /// <summary>Called when control start initialization.</summary>
        void ISupportInitialize.BeginInit()
        {
        }

        /// <summary>Called when control ends own initialization.</summary>
        void ISupportInitialize.EndInit()
        {
        }

        public TreeColumnAdv(SerializationInfo info, StreamingContext context)
        {
            m_style = new TreeColumnAdvStyleInfo((TreeColumnAdvStyleInfoStore)info.GetValue("ColumnStyle", typeof(TreeColumnAdvStyleInfoStore)));
            m_style.Changed += new StyleChangedEventHandler(Style_Changed);
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ColumnStyle", m_style.Store);
        }
        #endregion

        #region Class CodeDOM Serialization
     
        public void ResetBaseStyle()
        {
            this.ColumnStyle.ResetBaseStyle();
        }

        protected bool ShouldSerializeBaseStyle()
        {
            return this.ColumnStyle.ShouldSerializeBaseStyle();
        }

        /// <summary>
        /// Reset property Visible value to default value
        /// </summary>
        public virtual void ResetVisible()
        {
            this.ColumnStyle.ResetVisible();
        }

        /// <summary>
        /// Indicate should or not we serialize Visible property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeVisible()
        {
            return this.ColumnStyle.ShouldSerializeVisible();
        }

        /// <summary>
        /// Reset property Width value to default value
        /// </summary>
        public virtual void ResetWidth()
        {
            this.ColumnStyle.ResetWidth();
        }

        /// <summary>
        /// Indicate should or not we serialize Width property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeWidth()
        {
            return this.ColumnStyle.ShouldSerializeWidth();
        }

        /// <summary>
        /// Reset property Font value to default value
        /// </summary>
        public virtual void ResetFont()
        {
            this.ColumnStyle.ResetFont();
        }

        /// <summary>
        /// Indicate should or not we serialize Font property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeFont()
        {
            return this.ColumnStyle.ShouldSerializeFont();
        }

        /// <summary>
        /// Reset property Text value to default value
        /// </summary>
        public virtual void ResetText()
        {
            this.ColumnStyle.ResetText();
        }

        /// <summary>
        /// Indicate should or not we serialize Text property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeText()
        {
            return this.ColumnStyle.ShouldSerializeText();
        }

        /// <summary>
        /// Reset property TextColor value to default value
        /// </summary>
        public virtual void ResetTextColor()
        {
            this.ColumnStyle.ResetTextColor();
        }

        /// <summary>
        /// Indicate should or not we serialize TextColor property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeTextColor()
        {
            return this.ColumnStyle.ShouldSerializeTextColor();
        }

        /// <summary>
        /// Reset property Background value to default value
        /// </summary>
        public virtual void ResetBackground()
        {
            this.ColumnStyle.ResetBackground();
        }

        /// <summary>
        /// Indicate should or not we serialize Background property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeBackground()
        {
            return this.ColumnStyle.ShouldSerializeBackground();
        }

        /// <summary>
        /// Reset property HelpText value to default value
        /// </summary>
        public virtual void ResetHelpText()
        {
            this.ColumnStyle.ResetHelpText();
        }

        /// <summary>
        /// Indicate should or not we serialize HelpText property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeHelpText()
        {
            return this.ColumnStyle.ShouldSerializeHelpText();
        }

        /// <summary>
        /// Reset property SortOrder value to default value
        /// </summary>
        public virtual void ResetSortOrder()
        {
            this.ColumnStyle.ResetSortOrder();
        }

        /// <summary>
        /// Indicate should or not we serialize SortOrder property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeSortOrder()
        {
            return this.ColumnStyle.ShouldSerializeSortOrder();
        }

        /// <summary>
        /// Reset property Comparer value to default value
        /// </summary>
        public virtual void ResetComparer()
        {
            this.ColumnStyle.ResetComparer();
        }

        /// <summary>
        /// Indicate should or not we serialize Comparer property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeComparer()
        {
            return this.ColumnStyle.ShouldSerializeComparer();
        }

        /// <summary>
        /// Reset property Tag value to default value
        /// </summary>
        public virtual void ResetTag()
        {
            this.ColumnStyle.ResetTag();
        }

        /// <summary>
        /// Indicate should or not we serialize Tag property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeTag()
        {
            return this.ColumnStyle.ShouldSerializeTag();
        }

        /// <summary>
        /// Reset property LeftImage value to default value
        /// </summary>
        public virtual void ResetLeftImage()
        {
            this.ColumnStyle.ResetLeftImage();
        }

        /// <summary>
        /// Indicate should or not we serialize LeftImage property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeLeftImage()
        {
            return this.ColumnStyle.ShouldSerializeLeftImage();
        }

        /// <summary>
        /// Reset property RightImage value to default value
        /// </summary>
        public virtual void ResetRightImage()
        {
            this.ColumnStyle.ResetRightImage();
        }

        /// <summary>
        /// Indicate should or not we serialize RightImage property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeRightImage()
        {
            return this.ColumnStyle.ShouldSerializeRightImage();
        }

        /// <summary>
        /// Reset property LeftImageIndices value to default value
        /// </summary>
        public virtual void ResetLeftImageIndices()
        {
            this.ColumnStyle.ResetLeftImageIndices();
        }

        /// <summary>
        /// Indicate should or not we serialize LeftImageIndices property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeLeftImageIndices()
        {
            return this.ColumnStyle.ShouldSerializeLeftImageIndices();
        }

        /// <summary>
        /// Reset property LeftImagePadding value to default value
        /// </summary>
        public virtual void ResetLeftImagePadding()
        {
            this.ColumnStyle.ResetLeftImagePadding();
        }

        /// <summary>
        /// Indicate should or not we serialize LeftImagePadding property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeLeftImagePadding()
        {
            return this.ColumnStyle.ShouldSerializeLeftImagePadding();
        }

        /// <summary>
        /// Reset property RightImageIndices value to default value
        /// </summary>
        public virtual void ResetRightImageIndices()
        {
            this.ColumnStyle.ResetRightImageIndices();
        }

        /// <summary>
        /// Indicate should or not we serialize RightImageIndices property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeRightImageIndices()
        {
            return this.ColumnStyle.ShouldSerializeRightImageIndices();
        }

        /// <summary>
        /// Reset property RightImagePadding value to default value
        /// </summary>
        public virtual void ResetRightImagePadding()
        {
            this.ColumnStyle.ResetRightImagePadding();
        }

        /// <summary>
        /// Indicate should or not we serialize RightImagePadding property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeRightImagePadding()
        {
            return this.ColumnStyle.ShouldSerializeRightImagePadding();
        }

        /// <summary>
        /// Reset property BorderSides value to default value
        /// </summary>
        public virtual void ResetBorderSides()
        {
            this.ColumnStyle.ResetBorderSides();
        }

        /// <summary>
        /// Indicate should or not we serialize BorderSides property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeBorderSides()
        {
            return this.ColumnStyle.ShouldSerializeBorderSides();
        }

        /// <summary>
        /// Reset property BorderStyle value to default value
        /// </summary>
        public virtual void ResetBorderStyle()
        {
            this.ColumnStyle.ResetBorderStyle();
        }

        /// <summary>
        /// Indicate should or not we serialize BorderStyle property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeBorderStyle()
        {
            return this.ColumnStyle.ShouldSerializeBorderStyle();
        }

        /// <summary>
        /// Reset property Border3DStyle value to default value
        /// </summary>
        public virtual void ResetBorder3DStyle()
        {
            this.ColumnStyle.ResetBorder3DStyle();
        }

        /// <summary>
        /// Indicate should or not we serialize Border3DStyle property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeBorder3DStyle()
        {
            return this.ColumnStyle.ShouldSerializeBorder3DStyle();
        }

        /// <summary>
        /// Reset property BorderColor value to default value
        /// </summary>
        public virtual void ResetBorderColor()
        {
            this.ColumnStyle.ResetBorderColor();
        }

        /// <summary>
        /// Reset  HighlightBorderColor value to default value
        /// </summary>
        public virtual void ResetHighlightBorderColor()
        {
            this.ColumnStyle.ResetHighlightBorderColor();
        }

        /// <summary>
        /// Indicate should or not we serialize BorderColor property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeBorderColor()
        {
            return this.ColumnStyle.ShouldSerializeBorderColor();
        }

        /// <summary>
        /// Indicate whether to serialize HighlightBorderColor property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeHighlightBorderColor()
        {
            return this.ColumnStyle.ShouldSerializeHighlightBorderColor();
        }

        /// <summary>
        /// Reset property BorderSingle value to default value
        /// </summary>
        public virtual void ResetBorderSingle()
        {
            this.ColumnStyle.ResetBorderSingle();
        }

        /// <summary>
        /// Indicate should or not we serialize BorderSingle property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeBorderSingle()
        {
            return this.ColumnStyle.ShouldSerializeBorderSingle();
        }

        /// <summary>
        /// Reset property AreaBackground value to default value
        /// </summary>
        public virtual void ResetAreaBackground()
        {
            this.ColumnStyle.ResetAreaBackground();
        }

        /// <summary>
        /// Indicate should or not we serialize AreaBackground property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeAreaBackground()
        {
            return this.ColumnStyle.ShouldSerializeAreaBackground();
        }

        /// <summary>
        /// Reset property VerticalAlignment value to default value
        /// </summary>
        public virtual void ResetVerticalAlignment()
        {
            this.ColumnStyle.ResetVerticalAlignment();
        }

        /// <summary>
        /// Indicate should or not we serialize VerticalAlignment property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeVerticalAlignment()
        {
            return this.ColumnStyle.ShouldSerializeVerticalAlignment();
        }

        /// <summary>
        /// Reset property HorizontalAlignment value to default value
        /// </summary>
        public virtual void ResetHorizontalAlignment()
        {
            this.ColumnStyle.ResetHorizontalAlignment();
        }

        /// <summary>
        /// Indicate should or not we serialize HorizontalAlignment property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeHorizontalAlignment()
        {
            return this.ColumnStyle.ShouldSerializeHorizontalAlignment();
        }
        #endregion

        #region Class Public Methods

        public TreeColumnAdv Clone()
        {
            TreeColumnAdv column = new TreeColumnAdv();

            column.ColumnStyle.ModifyStyle(this.ColumnStyle, StyleModifyType.Copy);
            column.m_tree = m_tree;

            return column;
        }

        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public Size GetPreferedSize()
        {
            return Size.Empty;
        }

        /// <summary>
        /// Updates column's location.
        /// </summary>
        /// <param name="x"> X position</param>
        /// <param name="y"> Y position</param>
        public void UpdateColumnLocation(int x, int y)
        {
            m_rcBounds = new Rectangle(x, y, this.Width, this.Height);
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Update parent reference of column.
        /// </summary>
        /// <param name="tree">Reference on parent control.</param>
        protected internal virtual void SetParent(MultiColumnTreeView tree)
        {
            m_tree = tree;
        }

        /// <summary>
        /// Reset column reference on parent.
        /// </summary>
        protected internal virtual void ResetParent()
        {
            m_tree = null;
        }
        #endregion

        #region Class event handlers

        private void Style_Changed(object sender, StyleChangedEventArgs e)
        {
            if (this.TreeView != null)
            {
                this.TreeView.OnColumnChanged(new TreeColumnStyleChangedEventArgs(this, e));
            }
        }
        #endregion

        #region Class paint logic
        /// <summary>Draw Column at specified position.</summary>
        /// <param name="g">Graphics on which we should draw ourself.</param>
        /// <param name="point">X,Y coordinates.</param>
        public void Draw(Graphics g, Point point)
        {
            Draw(g, point.X, point.Y);
        }

        /// <summary>Draw Column at specified position.</summary>
        /// <param name="g">Graphics on which we should draw ourself.</param>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        public void Draw(Graphics g, int x, int y)
        {
            if (null == g)
            {
                throw new ArgumentNullException("g");
            }

            // update bounds
            m_rcBounds = new Rectangle(x, y, this.Width, this.Height);

            Rectangle rcBounds = this.BackgroundBounds;
            Region old = g.Clip;

            using (Region newRg1 = new Region(rcBounds))
            {
                g.SetClip(newRg1, CombineMode.Exclude);

                // draw borders.
                if (this.Highlighted)
                {
                    ControlDrawing.DrawBorderInternal(g, this.Bounds, this.BorderStyle, Border3DStyle.RaisedInner, ButtonBorderStyle.Solid,this.HighlightBorderColor, Border3DSide.All, false);
                }
                else
                {
                   ControlDrawing.DrawBorderInternal(g, this.Bounds, this.BorderStyle, this.Border3DStyle, this.BorderSingle, this.BorderColor, this.BorderSides, false);                                        
                }
            }

            using (Region newRg = new Region(rcBounds))
            {
                newRg.Intersect(old);
                g.Clip = newRg;

                // draw background
                BrushPaint.FillRectangle(g, rcBounds, this.Background);

                // set borders offset
                int startX = this.IsMirrored ? rcBounds.Right : rcBounds.Left;
                int leftBorder = this.IsMirrored ? rcBounds.Left : rcBounds.Right;

                if (AllowTextOverlap)
                {
                    DrawColumnText(g, 0, 0);
                    DrawRightImages(g, leftBorder, true);
                    DrawLeftImages(g, startX);
                }
                else
                {
                    // draw left images, text and right images
                    int rightOffset = Math.Abs(DrawRightImages(g, leftBorder, true) - leftBorder);
                    leftBorder = Math.Abs(DrawLeftImages(g, startX) - startX);
                    DrawColumnText(g, leftBorder, rightOffset);
                }
            }

            g.Clip = old;
        }

        /// <summary>Method draw images from the left side of the column text. 
        /// In RTL mode logic is reversed and images drawn from right side.</summary>
        /// <param name="g">Graphics for drawing.</param>
        /// <param name="startX">Start position of images drawing.</param>
        /// <returns>New start position for other methods that will draw after us.</returns>
        private int DrawLeftImages(Graphics g, int startX)
        {
            bool mirrored = this.IsMirrored;
            int direction = this.IsMirrored ? -1 : 1;
            Rectangle rc = this.BackgroundBounds;
            startX += direction * this.LeftImagePadding;

            if (this.LeftImage != null)
            {
                Size sizeLeftImg = this.LeftImage.Size;
                Rectangle source = new Rectangle(Point.Empty, sizeLeftImg);

                if (mirrored)
                {
                    startX -= sizeLeftImg.Width;
                }

                Rectangle destination = new Rectangle(startX, rc.Top, sizeLeftImg.Width, rc.Height);
                g.DrawImage(this.LeftImage, destination, source, GraphicsUnit.Pixel);

                if (!mirrored)
                {
                    startX += sizeLeftImg.Width;
                }
            }
            else if (this.LeftImageIndices != null && this.LeftImageIndices.Length > 0)
            {
                if (this.TreeView != null && this.TreeView.LeftImageList != null)
                {
                    ImageList images = this.TreeView.LeftImageList;

                    if (images.Images.Count > 0)
                    {
                        int xStep = Math.Min(images.ImageSize.Width, rc.Height);
                        int[] indices = this.LeftImageIndices;

                        for (int i = 0, len = indices.Length; i < len; i++)
                        {
                            int index = indices[i];

                            // WARNING: do not use such code for image drawing, like in example.
                            // Example: images.Draw( g, startX, rc.Top, xStep, xStep, index );
                            // Drawing of image ignore Graphics Clip region!!! 
                            if (mirrored)
                            {
                                startX -= xStep;
                            }

                            // draw only images that exists in imagelist
                            if (index >= 0 && index < images.Images.Count)
                            {
                                g.DrawImage(images.Images[index], startX, rc.Top, xStep, xStep);
                            }

                            if (!mirrored)
                            {
                                startX += xStep;
                            }
                        }
                    }
                }
            }

            return startX;
        }

        private void DrawColumnText(Graphics g, int leftBorder, int rightBorder)
        {
            bool mirrored = this.IsMirrored;

            using (SolidBrush brush = new SolidBrush(this.TextColor))
            {
                using (StringFormat format = (StringFormat)StringFormat.GenericDefault.Clone())
                {
                    format.FormatFlags |= StringFormatFlags.NoWrap;
                    format.LineAlignment = this.VerticalAlignment;
                    format.Alignment = this.HorizontalAlignment;

                    Size szText = ControlDrawing.MeasureDisplayStringSize(g, this.Text, this.Font, mirrored);

                    if (mirrored)
                    {
                        format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                    }

                    Rectangle rc = this.BackgroundBounds;
                    Rectangle output;

                    if (AllowTextOverlap)
                    {
                        output = rc;
                    }
                    else
                    {
                        output = new Rectangle(rc.Left + leftBorder, rc.Top, rc.Width - leftBorder - rightBorder, rc.Height);
                    }

                    g.DrawString(this.Text, this.Font, brush, output, format);
                }
            }
        }

        private int DrawRightImages(Graphics g, int startX, bool forceMirror)
        {
            bool mirrored = this.IsMirrored ^ forceMirror;
            int direction = mirrored ? -1 : 1;
            Rectangle rc = this.BackgroundBounds;
            startX += direction * this.RightImagePadding;

            if (this.RightImage != null)
            {
                Size sizeRightImg = this.RightImage.Size;
                Rectangle source = new Rectangle(Point.Empty, sizeRightImg);

                if (mirrored)
                {
                    startX -= sizeRightImg.Width;
                }
                Rectangle destination = new Rectangle(startX, rc.Top, sizeRightImg.Width, rc.Height);
                g.DrawImage(this.RightImage, destination, source, GraphicsUnit.Pixel);
                if (!mirrored)
                {
                    startX += sizeRightImg.Width;
                }
            }
            else if (this.RightImageIndices != null && this.RightImageIndices.Length > 0)
            {
                if (this.TreeView != null && this.TreeView.RightImageList != null)
                {
                    ImageList images = this.TreeView.RightImageList;

                    if (images.Images.Count > 0)
                    {
                        int xStep = Math.Min(images.ImageSize.Width, rc.Height);
                        int[] indices = this.RightImageIndices;

                        for (int i = 0, len = indices.Length; i < len; i++)
                        {
                            int index = indices[i];

                            // WARNING: do not use such code for image drawing, like in example.
                            // Drawing of image ignore Graphics Clip region!!! 
                            // Example: images.Draw( g, startX, rc.Top, xStep, xStep, index );
                            if (mirrored)
                            {
                                startX -= xStep;
                            }

                            // draw only images that exists in imagelist
                            if (index >= 0 && index < images.Images.Count)
                            {
                                g.DrawImage(images.Images[index], startX, rc.Top, xStep, xStep);
                            }

                            if (!mirrored)
                            {
                                startX += xStep;
                            }
                        }
                    }
                }
            }

            return startX;
        }
        #endregion
    }
}