#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.Serialization;
using System.Windows.Forms;

using Syncfusion.Drawing;
using Syncfusion.Styles;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    [
    TypeConverter(typeof(TreeNodeAdvSubItemConverter)),
    Serializable()
    ]
    public class TreeNodeAdvSubItem :
      MarshalByRefObject,
      ICloneable,
      IComparable,
      ISupportInitialize,
      ISerializable
    {
        #region Class members
  
        private TreeNodeAdvSubItemStyleInfo m_style = null;

        /// <summary>Reference on parent node. Can be NULL.</summary>
        private TreeNodeAdv m_parent;

        /// <summary>SubItem bounds.</summary>
        private Rectangle m_rcBounds;

        /// <summary>Storage of text bounds.</summary>
        private Rectangle m_rcTextBounds;
        #endregion

        #region Class properties
        /// <summary>Gets parent Node.</summary>
        [
        Browsable(false)
        ]
        public TreeNodeAdv TreeNode
        {
            get
            {
                return m_parent;
            }
        }

        /// <summary>Gets reference on parent control.</summary>
        [
        Browsable(false)
        ]
        public MultiColumnTreeView TreeView
        {
            get
            {
                if (this.TreeNode != null)
                {
                    return this.TreeNode.TreeView;
                }

                return null;
            }
        }

        /// <summary>Gets reference on subitem style.</summary>
        [
        Browsable(false)
        ]
        public TreeNodeAdvSubItemStyleInfo SubItemStyle
        {
            get
            {
                return m_style;
            }
        }

        /// <summary>
        /// Gets or sets the base style for the subitem from which to inherit.
        /// </summary>
        /// <remarks>The specified base style should be available in the <see cref="MultiColumnTreeView.BaseStyles"/>
        /// collection.</remarks>
        [
        Description("The base style for the subitem"),
        Category("Appearance - Inherited"),
        Editor(typeof(BaseStyleSelectorUITypeEditor), typeof(UITypeEditor))
        ]
        public string BaseStyle
        {
            get
            {
                return this.SubItemStyle.BaseStyle;
            }
            set
            {
                this.SubItemStyle.BaseStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether visible property has set.
        /// Visibility of subitem also depends on column <see cref="TreeColumnAdv.Visible"/> property value.
        /// If column not visible then all corresponding subitems will be skipped on painting.
        /// </summary>
        [
        Description("Gets / sets the subitem visibility."),
        Category("Appearance"),
        Localizable(true),
        DefaultValue(true)
        ]
        public bool Visible
        {
            get
            {
                return this.SubItemStyle.Visible;
            }
            set
            {
                this.SubItemStyle.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets the line alignment of the text in subitem.
        /// </summary>
        [
        Description("Gets / sets the line alignment of the text in subitem."),
        Category("Appearance"),
        Localizable(true),
        DefaultValue(StringAlignment.Center)
        ]
        public StringAlignment LineAlignment
        {
            get
            {
                return this.SubItemStyle.LineAlignment;
            }
            set
            {
                this.SubItemStyle.LineAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the text alignment in subitem bounds.
        /// </summary>
        [
        Description("Gets / sets the text alignment in subitem bounds."),
        Category("Appearance"),
        Localizable(true),
        DefaultValue(StringAlignment.Near)
        ]
        public StringAlignment Alignment
        {
            get
            {
                return this.SubItemStyle.Alignment;
            }
            set
            {
                this.SubItemStyle.Alignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the subitem text. 
        /// </summary>
        /// <remarks>Subitem that user get by ZERO index represents treenode text and style.</remarks>
        [
        Description("Gets / sets the subitem text."),
        Category("Appearance"),
        Localizable(true),
        DefaultValue("")
        ]
        public string Text
        {
            get
            {
                return this.SubItemStyle.Text;
            }
            set
            {
                this.SubItemStyle.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the subitem text color.
        /// </summary>
        [
        Description("Gets / sets the subitem text color."),
        Category("Appearance"),
        Localizable(true),
        DefaultValue(typeof(Color), "WindowText")
        ]
        public Color TextColor
        {
            get
            {
                return this.SubItemStyle.TextColor;
            }
            set
            {
                this.SubItemStyle.TextColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the subitem help text.
        /// </summary>
        [
        Description("Gets / sets the subitem help text."),
        Category("Appearance"),
        Localizable(true),
        DefaultValue("")
        ]
        public string HelpText
        {
            get
            {
                return this.SubItemStyle.HelpText;
            }
            set
            {
                this.SubItemStyle.HelpText = value;
            }
        }

        /// <summary>
        /// Gets or sets the subitem user data.
        /// </summary>
        [
        Description("Gets / sets the subitem user data."),
        Category("Appearance"),
        Localizable(true),
        DefaultValue(null)
        ]
        public object Tag
        {
            get
            {
                return this.SubItemStyle.Tag;
            }
            set
            {
                this.SubItemStyle.Tag = value;
            }
        }

        /// <summary>
        /// Gets or sets the subitem left image.
        /// </summary>
        [
        Description("Gets / sets the subitem left image."),
        Category("Images"),
        Localizable(true),
        DefaultValue(null)
        ]
        public Image LeftImage
        {
            get
            {
                return this.SubItemStyle.LeftImage;
            }
            set
            {
                this.SubItemStyle.LeftImage = value;
            }
        }

        /// <summary>
        /// Gets or sets the subitem left side indices of images that stored in tree LeftImageList property.
        /// </summary>
        [
        Description("Gets / sets the subitem left side indices of images that stored in tree LeftImageList property."),
        Category("Images"),
        Localizable(true)
        ]
        public int[] LeftImageIndices
        {
            get
            {
                return this.SubItemStyle.LeftImageIndices;
            }
            set
            {
                this.SubItemStyle.LeftImageIndices = value;
            }
        }

        /// <summary>
        /// Gets or sets the subitem padding between text and left images.
        /// </summary>
        [
        Description("Gets / sets the subitem padding between text and left images."),
        Category("Images"),
        Localizable(true),
        DefaultValue(0)
        ]
        public int LeftImagePadding
        {
            get
            {
                return this.SubItemStyle.LeftImagePadding;
            }
            set
            {
                this.SubItemStyle.LeftImagePadding = value;
            }
        }

        /// <summary>
        /// Gets or sets the subitem image from right side of the text.
        /// </summary>
        [
        Description("Gets / sets the subitem image from right side of the text."),
        Category("Images"),
        Localizable(true),
        DefaultValue(null)
        ]
        public Image RightImage
        {
            get
            {
                return this.SubItemStyle.RightImage;
            }
            set
            {
                this.SubItemStyle.RightImage = value;
            }
        }

        /// <summary>
        /// Gets or sets the subitem right side indices of images that stored in tree RightImageList property.
        /// </summary>
        [
        Description("Gets / sets the subitem right side indices of images that stored in tree RightImageList property."),
        Category("Images"),
        Localizable(true)
        ]
        public int[] RightImageIndices
        {
            get
            {
                return this.SubItemStyle.RightImageIndices;
            }
            set
            {
                this.SubItemStyle.RightImageIndices = value;
            }
        }

        /// <summary>
        /// Gets or sets padding between text and right side images.
        /// </summary>
        [
        Description("Gets / sets padding between text and right side images."),
        Category("Images"),
        Localizable(true),
        DefaultValue(0)
        ]
        public int RightImagePadding
        {
            get
            {
                return this.SubItemStyle.RightImagePadding;
            }
            set
            {
                this.SubItemStyle.RightImagePadding = value;
            }
        }

        /// <summary>
        /// Gets or sets the subitem background style.
        /// </summary>
        [
        Description("Gets / sets the subitem background style."),
        Category("Appearance"),
        Localizable(true),
        ]
        public BrushInfo Background
        {
            get
            {
                return this.SubItemStyle.Background;
            }
            set
            {
                this.SubItemStyle.Background = value;
            }
        }

        /// <summary>
        /// Gets or sets the subitem border sides settings.
        /// </summary>
        [
        Description("Gets / sets the subitem border sides settings."),
        Category("Borders"),
        Localizable(true),
        DefaultValue(Border3DSide.All & (~Border3DSide.Middle))
        ]
        public Border3DSide BorderSides
        {
            get
            {
                return this.SubItemStyle.BorderSides;
            }
            set
            {
                this.SubItemStyle.BorderSides = value;
            }
        }

        /// <summary>
        /// Gets or sets the subitem border style.
        /// </summary>
        [
        Description("Gets / sets the subitem border style."),
        Category("Borders"),
        Localizable(true),
        DefaultValue(BorderStyle.None)
        ]
        public BorderStyle BorderStyle
        {
            get
            {
                return this.SubItemStyle.BorderStyle;
            }
            set
            {
                this.SubItemStyle.BorderStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the subitem border 3D style.
        /// </summary>
        [
        Description("Gets / sets the subitem border 3D style."),
        Category("Borders"),
        Localizable(true),
        DefaultValue(Border3DStyle.Flat)
        ]
        public Border3DStyle Border3DStyle
        {
            get
            {
                return this.SubItemStyle.Border3DStyle;
            }
            set
            {
                this.SubItemStyle.Border3DStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the subitem border color.
        /// </summary>
        [
        Description("Gets / sets the subitem border color."),
        Category("Borders"),
        Localizable(true),
        DefaultValue(typeof(Color), "ControlDark")
        ]
        public Color BorderColor
        {
            get
            {
                return this.SubItemStyle.BorderColor;
            }
            set
            {
                this.SubItemStyle.BorderColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the subitem single line border style.
        /// </summary>
        [
        Description("Gets / sets the subitem single line border style."),
        Category("Borders"),
        Localizable(true),
        DefaultValue(ButtonBorderStyle.None)
        ]
        public ButtonBorderStyle BorderSingle
        {
            get
            {
                return this.SubItemStyle.BorderSingle;
            }
            set
            {
                this.SubItemStyle.BorderSingle = value;
            }
        }

        /// <summary>
        /// Gets or sets the subitem text Font.
        /// </summary>
        [
        Description("Gets / sets the subitem text Font."),
        Category("Borders"),
        Localizable(true)
        ]
        public Font Font
        {
            get
            {
                return this.SubItemStyle.Font;
            }
            set
            {
                this.SubItemStyle.Font = value;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        public TreeNodeAdvSubItem()
        {
            m_style = new TreeNodeAdvSubItemStyleInfo(new TreeNodeAdvSubItemStyleInfoIdentity(this));
            m_style.Changed += new StyleChangedEventHandler(Style_Changed);
        }
        public TreeNodeAdvSubItem(string text)
            : this()
        {
            this.Text = text;
        }

        /// <summary>Initializes a new instance of the TreeNodeAdvSubItem class.</summary>
        /// <param name="node">Tree node</param>
        public TreeNodeAdvSubItem(TreeNodeAdv node)
            : this()
        {
            this.SetParent(node);
        }

        public TreeNodeAdvSubItem(TreeNodeAdv node, string text)
            : this(node)
        {
            this.Text = text;
        }

        public TreeNodeAdvSubItem(SerializationInfo info, StreamingContext context)
        {
            TreeNodeAdvSubItemStyleInfoStore store = (TreeNodeAdvSubItemStyleInfoStore)info.GetValue(
              "SubItemStyle", typeof(TreeNodeAdvSubItemStyleInfoStore));

            m_style = new TreeNodeAdvSubItemStyleInfo(store);
            m_style.Changed += new StyleChangedEventHandler(Style_Changed);
        }

        /// <summary>Class serialization.</summary>
        /// <param name="info">Serialization Info</param>
        /// <param name="context">Streaming Context</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("SubItemStyle", m_style.Store);
        }

        /// <summary>Called when control start initialization.</summary>
        public void BeginInit()
        {
        }

        /// <summary>Called when control ends own initialization.</summary>
        public void EndInit()
        {
        }
        #endregion

        #region Class CodeDOM Serialization
        /// <summary>
        /// Reset property BaseStyle value to default value
        /// </summary>
        public virtual void ResetBaseStyle()
        {
            this.SubItemStyle.ResetBaseStyle();
        }

        /// <summary>
        /// Indicate should or not we serialize BaseStyle property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeBaseStyle()
        {
            return this.SubItemStyle.ShouldSerializeBaseStyle();
        }

        /// <summary>
        /// Reset property Visible value to default value
        /// </summary>
        public virtual void ResetVisible()
        {
            this.SubItemStyle.ResetVisible();
        }

        /// <summary>
        /// Indicate should or not we serialize Visible property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeVisible()
        {
            return this.SubItemStyle.ShouldSerializeVisible();
        }

        /// <summary>
        /// Reset property LineAlignment value to default value
        /// </summary>
        public virtual void ResetLineAlignment()
        {
            this.SubItemStyle.ResetLineAlignment();
        }

        /// <summary>
        /// Indicate should or not we serialize LineAlignment property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeLineAlignment()
        {
            return this.SubItemStyle.ShouldSerializeLineAlignment();
        }

        /// <summary>
        /// Reset property Alignment value to default value
        /// </summary>
        public virtual void ResetAlignment()
        {
            this.SubItemStyle.ResetAlignment();
        }

        /// <summary>
        /// Indicate should or not we serialize Alignment property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeAlignment()
        {
            return this.SubItemStyle.ShouldSerializeAlignment();
        }

        /// <summary>
        /// Reset property Text value to default value
        /// </summary>
        public virtual void ResetText()
        {
            this.SubItemStyle.ResetText();
        }

        /// <summary>
        /// Indicate should or not we serialize Text property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeText()
        {
            return this.SubItemStyle.ShouldSerializeText();
        }

        /// <summary>
        /// Reset property TextColor value to default value
        /// </summary>
        public virtual void ResetTextColor()
        {
            this.SubItemStyle.ResetTextColor();
        }

        /// <summary>
        /// Indicate should or not we serialize TextColor property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeTextColor()
        {
            return this.SubItemStyle.ShouldSerializeTextColor();
        }

        /// <summary>
        /// Reset property HelpText value to default value
        /// </summary>
        public virtual void ResetHelpText()
        {
            this.SubItemStyle.ResetHelpText();
        }

        /// <summary>
        /// Indicate should or not we serialize HelpText property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeHelpText()
        {
            return this.SubItemStyle.ShouldSerializeHelpText();
        }

        /// <summary>
        /// Reset property Tag value to default value
        /// </summary>
        public virtual void ResetTag()
        {
            this.SubItemStyle.ResetTag();
        }

        /// <summary>
        /// Indicate should or not we serialize Tag property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeTag()
        {
            return this.SubItemStyle.ShouldSerializeTag();
        }

        /// <summary>
        /// Reset property LeftImage value to default value
        /// </summary>
        public virtual void ResetLeftImage()
        {
            this.SubItemStyle.ResetLeftImage();
        }

        /// <summary>
        /// Indicate should or not we serialize LeftImage property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeLeftImage()
        {
            return this.SubItemStyle.ShouldSerializeLeftImage();
        }

        /// <summary>
        /// Reset property LeftImageIndices value to default value
        /// </summary>
        public virtual void ResetLeftImageIndices()
        {
            this.SubItemStyle.ResetLeftImageIndices();
        }

        /// <summary>
        /// Indicate should or not we serialize LeftImageIndices property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeLeftImageIndices()
        {
            return this.SubItemStyle.ShouldSerializeLeftImageIndices();
        }

        /// <summary>
        /// Reset property LeftImagePadding value to default value
        /// </summary>
        public virtual void ResetLeftImagePadding()
        {
            this.SubItemStyle.ResetLeftImagePadding();
        }

        /// <summary>
        /// Indicate should or not we serialize LeftImagePadding property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeLeftImagePadding()
        {
            return this.SubItemStyle.ShouldSerializeLeftImagePadding();
        }

        /// <summary>
        /// Reset property RightImage value to default value
        /// </summary>
        public virtual void ResetRightImage()
        {
            this.SubItemStyle.ResetRightImage();
        }

        /// <summary>
        /// Indicate should or not we serialize RightImage property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeRightImage()
        {
            return this.SubItemStyle.ShouldSerializeRightImage();
        }

        /// <summary>
        /// Reset property RightImageIndices value to default value
        /// </summary>
        public virtual void ResetRightImageIndices()
        {
            this.SubItemStyle.ResetRightImageIndices();
        }

        /// <summary>
        /// Indicate should or not we serialize RightImageIndices property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeRightImageIndices()
        {
            return this.SubItemStyle.ShouldSerializeRightImageIndices();
        }

        /// <summary>
        /// Reset property RightImagePadding value to default value
        /// </summary>
        public virtual void ResetRightImagePadding()
        {
            this.SubItemStyle.ResetRightImagePadding();
        }

        /// <summary>
        /// Indicate should or not we serialize RightImagePadding property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeRightImagePadding()
        {
            return this.SubItemStyle.ShouldSerializeRightImagePadding();
        }

        /// <summary>
        /// Reset property Background value to default value
        /// </summary>
        public virtual void ResetBackground()
        {
            this.SubItemStyle.ResetBackground();
        }

        /// <summary>
        /// Indicate should or not we serialize Background property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeBackground()
        {
            return this.SubItemStyle.ShouldSerializeBackground();
        }

        /// <summary>
        /// Reset property BorderSides value to default value
        /// </summary>
        public virtual void ResetBorderSides()
        {
            this.SubItemStyle.ResetBorderSides();
        }

        /// <summary>
        /// Indicate should or not we serialize BorderSides property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeBorderSides()
        {
            return this.SubItemStyle.ShouldSerializeBorderSides();
        }

        /// <summary>
        /// Reset property BorderStyle value to default value
        /// </summary>
        public virtual void ResetBorderStyle()
        {
            this.SubItemStyle.ResetBorderStyle();
        }

        /// <summary>
        /// Indicate should or not we serialize BorderStyle property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeBorderStyle()
        {
            return this.SubItemStyle.ShouldSerializeBorderStyle();
        }

        /// <summary>
        /// Reset property Border3DStyle value to default value
        /// </summary>
        public virtual void ResetBorder3DStyle()
        {
            this.SubItemStyle.ResetBorder3DStyle();
        }

        /// <summary>
        /// Indicate should or not we serialize Border3DStyle property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeBorder3DStyle()
        {
            return this.SubItemStyle.ShouldSerializeBorder3DStyle();
        }

        /// <summary>
        /// Reset property BorderColor value to default value
        /// </summary>
        public virtual void ResetBorderColor()
        {
            this.SubItemStyle.ResetBorderColor();
        }

        /// <summary>
        /// Indicate should or not we serialize BorderColor property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeBorderColor()
        {
            return this.SubItemStyle.ShouldSerializeBorderColor();
        }

        /// <summary>
        /// Reset property BorderSingle value to default value
        /// </summary>
        public virtual void ResetBorderSingle()
        {
            this.SubItemStyle.ResetBorderSingle();
        }

        /// <summary>
        /// Indicate should or not we serialize BorderSingle property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeBorderSingle()
        {
            return this.SubItemStyle.ShouldSerializeBorderSingle();
        }

        /// <summary>
        /// Reset property Font value to default value
        /// </summary>
        public virtual void ResetFont()
        {
            this.SubItemStyle.ResetFont();
        }

        /// <summary>
        /// Indicate should or not we serialize Font property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeFont()
        {
            return this.SubItemStyle.ShouldSerializeFont();
        }
        #endregion

        #region Class Public Methods
        /// <summary>Clone TreeNodeAdv sub-item</summary>
        /// <returns>copy of this sub item.</returns>
        public TreeNodeAdvSubItem Clone()
        {
            TreeNodeAdvSubItem subitem = new TreeNodeAdvSubItem();

            subitem.SubItemStyle.ModifyStyle(this.SubItemStyle, StyleModifyType.Copy);

            return subitem;
        }

        /// <summary>Clone TreeNodeAdv sub-item</summary>
        /// <returns>copy of this sub item.</returns>
        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }
        int IComparable.CompareTo(object obj)
        {
            return 0;
        }
        #endregion

        #region Class utility methods
      
        public override string ToString()
        {
            return string.Format("Text: {0}", this.Text);
        }

        protected internal virtual void SetParent(TreeNodeAdv node)
        {
            m_parent = node;
        }

        protected internal virtual void ResetParent()
        {
            m_parent = null;
        }
        #endregion

        #region Class event handlers

        private void Style_Changed(object sender, StyleChangedEventArgs e)
        {
            // notify treenode that subitem changed
            if (this.TreeNode != null)
            {
                this.TreeNode.OnSubItemChanged(e);
            }
        }
        #endregion

        #region Class layout logic

        /// <summary>
        /// Gets a value indicating whether Is mirrored. True if  RTL drawing algorithm, otherwise False.
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

        /// <summary> Gets or sets the column bounds.</summary>
        internal Rectangle Bounds
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
      
        internal int Index
        {
            get
            {
                if (this.TreeNode != null)
                {
                    return this.TreeNode.SubItems.IndexOf(this);
                }

                return -1;
            }
        }
       
        internal TreeColumnAdv Column
        {
            get
            {
                if (this.TreeView != null)
                {
                    int index = this.Index;

                    if (index >= 0)
                    {
                        return this.TreeView.Columns[index];
                    }
                }

                return null;
            }
        }
     
        internal int Width
        {
            get
            {
                if (this.Column != null)
                {
                    return this.Column.Width;
                }

                return 0;
            }
        }
      
        internal int Height
        {
            get
            {
                if (this.TreeNode != null)
                {
                    return this.TreeNode.Height;
                }

                return 0;
            }
        }
        #endregion

        #region Class Paint logic

        public virtual void Draw(Graphics g, int x, int y, TreeNodeAdv mouseDownNode, Rectangle selectionBounds)
        {
            if (null == g)
            {
                throw new ArgumentNullException("g");
            }

            // update bounds
            m_rcBounds = new Rectangle(x, y, this.Width, this.Height);

            ControlDrawing.DrawBorderInternal(g, this.Bounds, this.BorderStyle, this.Border3DStyle, this.BorderSingle, this.BorderColor, this.BorderSides, true);
           
            Rectangle rcBounds = this.BackgroundBounds;
            Region old = g.Clip;

            using (Region newRg = new Region(rcBounds))
            {
                newRg.Intersect(old);
                g.Clip = newRg;

                // draw subitem background
                bool selected = DrawBackGround(g, selectionBounds, mouseDownNode);

                // set borders offset
                int startX = this.IsMirrored ? rcBounds.Right : rcBounds.Left;

                // draw left images, text and right images
                startX = DrawLeftImages(g, startX);
                startX = DrawSubItemText(g, startX, selected);
                startX = DrawRightImages(g, startX);
            }

            g.Clip = old;
        }

        /// <summary>
        /// Draws background of subitem.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="bounds">Rectangle bounds</param>
        /// <param name="mouseDownNode">Mouse down node</param>
        /// <returns> If parent node for this subitem is selected then return true else return false </returns>
        private bool DrawBackGround(Graphics g, Rectangle bounds, TreeNodeAdv mouseDownNode)
        {
            // Also, assuming this is the case in ITreeNodeAdvPaintFilter.OnNodeBackgroundPaint
            BrushInfo bi = this.Background;
            bool selected = false;

            if (TreeView.FullRowSelect)
            {
                selected = this.TreeView.SelectedNodes.Contains(m_parent) && (!TreeView.HideSelection || TreeView.Focused);

                // If the mouseDownNode is a selected node, then draw the selected nodes.
                if (selected && (mouseDownNode == null ||
                  TreeView.SelectedNodes.Contains(mouseDownNode)))
                {
                    if (this.TreeView.Focused)
                    {
                        bi = this.TreeView.SelectedNodeBackground.Clone();
                    }
                    else if (!this.TreeView.HideSelection)
                    {
                        bi = this.TreeView.InactiveSelectedNodeBackground.Clone();
                    }
                }
                else 
                {
                    selected = false;
                }

                if (m_parent == mouseDownNode)
                {
                    selected = true;
                    bi = this.TreeView.SelectedNodeBackground.Clone();
                }
            }

            // draw background
            BrushPaint.FillRectangle(g, bounds, bi);

            return selected;
        }

        /// <summary>Method draw images from the left side of the column text. 
        /// In RTL mode logic is reversed and images drawn from right side.</summary>
        /// <param name="g">Graphics for drawing.</param>
        /// <param name="startX">Start position of images drawing.</param>
        /// <returns>New start position for other methods that will draw after us.</returns>
        private int DrawLeftImages(Graphics g, int startX)
        {
            bool bMirrored = this.IsMirrored;
            int direction = this.IsMirrored ? -1 : 1;
            Rectangle rc = this.BackgroundBounds;
            startX += direction * this.LeftImagePadding;

            if (this.LeftImage != null)
            {
                Size sizeLeftImg = this.LeftImage.Size;
                Rectangle source = new Rectangle(Point.Empty, sizeLeftImg);

                if (bMirrored)
                {
                    startX -= sizeLeftImg.Width;
                }
                Rectangle destination = new Rectangle(startX, rc.Top, sizeLeftImg.Width, rc.Height);
                g.DrawImage(this.LeftImage, destination, source, GraphicsUnit.Pixel);
                if (!bMirrored)
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
                            // Drawing of image ignore Graphics Clip region!!! 
                            // Example: images.Draw( g, startX, rc.Top, xStep, xStep, index );
                            if (bMirrored)
                            {
                                startX -= xStep;
                            }

                            // draw only images that exists in imagelist
                            if (index >= 0 && index < images.Images.Count)
                            {
                                g.DrawImage(images.Images[index], startX, rc.Top, xStep, xStep);
                            }

                            if (!bMirrored)
                            {
                                startX += xStep;
                            }
                        }
                    }
                }
            }

            return startX;
        }

        protected internal Color GetForeColor(bool selected)
        {
            MultiColumnTreeView tree = this.TreeView;
            if (tree != null)
            {
                if (selected)
                {
                    if (tree.Focused)
                    {
                        return tree.SelectedNodeForeColor;
                    }
                    else if (!tree.HideSelection)
                    {
                        return tree.InactiveSelectedNodeForeColor;
                    }
                }
                if (!tree.Enabled || !m_parent.Enabled)
                {
                    return SystemColors.GrayText;
                }
                else
                {
                    return SubItemStyle.TextColor;
                }
            }
            return SubItemStyle.TextColor;
        }
        private int DrawSubItemText(Graphics g, int startX, bool selected)
        {
            bool mirrored = this.IsMirrored;
            Color textColor = GetForeColor(selected);

            using (SolidBrush brush = new SolidBrush(textColor))
            {
                using (StringFormat format = (StringFormat)StringFormat.GenericTypographic.Clone())
                {
                    format.FormatFlags = StringFormatFlags.LineLimit | StringFormatFlags.FitBlackBox;
                    format.LineAlignment = this.LineAlignment;
                    format.Alignment = this.Alignment;
                    format.Trimming = StringTrimming.EllipsisWord;

                    if (mirrored)
                    {
                        format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                    }

                    Size szText = ControlDrawing.MeasureDisplayStringSize(g, this.Text, this.Font, mirrored);

                    Rectangle rc = this.BackgroundBounds;

                    if (mirrored)
                    {
                        startX -= rc.Width;
                    }

                    Rectangle rcText = new Rectangle(startX, rc.Top, rc.Width, rc.Height);
                    g.DrawString(this.Text, this.Font, brush, rcText, format);

                    if (!mirrored)
                    {
                        startX += szText.Width;
                    }
                }
            }

            return startX;
        }

        private int DrawRightImages(Graphics g, int startX)
        {
            bool mirrored = this.IsMirrored;
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