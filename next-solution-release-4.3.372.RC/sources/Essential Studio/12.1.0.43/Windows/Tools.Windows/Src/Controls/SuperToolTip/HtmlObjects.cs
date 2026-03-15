#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Tools
{
    internal class HtmlRootBox : HtmlBox
    {
        #region Fields
        private Dictionary<string, Dictionary<string, HtmlBlock>> elementStyleBlocks;
        private string documentSource;
        private SizeF maxSize;
        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the HtmlRootBox class.
        /// </summary>
        public HtmlRootBox()
        {
            htmlRootBox = this;
            elementStyleBlocks = new Dictionary<string, Dictionary<string, HtmlBlock>>();
            ElementStyleBlocks.Add("all", new Dictionary<string, HtmlBlock>());

            Display = Constants.Block;

            ApplyStyleSheet(Constants.DefaultStyleSheet);
        }

        /// <summary>
        /// Initializes a new instance of the HtmlRootBox class.
        /// </summary>
        /// <param name="documentSource">The document source.</param>
        public HtmlRootBox(string documentSource, Rectangle rect)
            : this()
        {
            this.documentSource = documentSource;
            ParseDocumentSource();
            ApplyStyles(this);
            AdjustBlock(this);
            this.Bounds = rect;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the blocks of style defined on this structure.
        /// </summary>
        internal Dictionary<string, Dictionary<string, HtmlBlock>> ElementStyleBlocks
        {
            get { return elementStyleBlocks; }
        }

        /// <summary>
        /// Gets the document's source
        /// </summary>
        public string DocumentSource
        {
            get { return documentSource; }
        }

         /// <summary>
        /// Gets or sets the maximum size of the container
        /// </summary>
        public SizeF MaxSize
        {
            get { return maxSize; }
            set { maxSize = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Feeds the blocks of the stylesheet
        /// </summary>
        /// <param name="stylesheet"></param>
        public void ApplyStyleSheet(string stylesheet)
        {
            if (string.IsNullOrEmpty(stylesheet)) 
                return;
            stylesheet = stylesheet.ToLower();

            Do.RemoveComments(ref stylesheet);

            MatchCollection blocks = Do.MatchPattern(Constants.HtmlBlocks, stylesheet);

            foreach (Match match in blocks)
            {
                ApplyStyleBlock("all", match.Value);
            }

        }

        /// <summary>
        /// Feeds the style with a block.
        /// </summary>
        private void ApplyStyleBlock(string media, string block)
        {
            if (string.IsNullOrEmpty(media)) media = "all";

            int bracketIndex = block.IndexOf("{");
            string blockSource = block.Substring(bracketIndex).Replace("{", string.Empty).Replace("}", string.Empty);

            if (bracketIndex < 0) return;

            string[] classes = block.Substring(0, bracketIndex).Split(',');

            for (int i = 0; i < classes.Length; i++)
            {
                string className = classes[i].Trim(); if(string.IsNullOrEmpty(className)) continue;

                HtmlBlock newblock = new HtmlBlock(blockSource);

                if (!ElementStyleBlocks.ContainsKey(media)) ElementStyleBlocks.Add(media, new Dictionary<string, HtmlBlock>());

                if (!ElementStyleBlocks[media].ContainsKey(className))
                {
                    ElementStyleBlocks[media].Add(className, newblock);
                }
                else
                {

                    HtmlBlock oldblock = ElementStyleBlocks[media][className];

                    foreach (string property in newblock.Properties.Keys)
                    {
                        if (oldblock.Properties.ContainsKey(property))
                        {
                            oldblock.Properties[property] = newblock.Properties[property];
                        }
                        else
                        {
                            oldblock.Properties.Add(property, newblock.Properties[property]);
                        }
                    }

                    oldblock.ChangePropertyValues();
                }
            }
        }

        /// <summary>
        /// Parses the HTML document
        /// </summary>
        private void ParseDocumentSource()
        {
            HtmlRootBox root = this;
            MatchCollection tags = Do.MatchPattern(Constants.HtmlTag, DocumentSource);
            HtmlBox currentBox = root;
            int lastEnd = -1;

            foreach (Match tagmatch in tags)
            {
                string text = tagmatch.Index > 0 ? DocumentSource.Substring(lastEnd + 1, tagmatch.Index - lastEnd - 1) : string.Empty;

                if (!string.IsNullOrEmpty(text.Trim()))
                {
                    HtmlBox abox = new HtmlBox(currentBox);
                    abox.Text = text;
                    abox.IsAnonymous = true;
                }
                else if(text != null && text.Length > 0)
                {
                    HtmlBox sbox = new HtmlBox(currentBox);
                    sbox.Text = text;
                    sbox.IsAnonymousSpace = true;
                }

                HtmlTag tag = new HtmlTag(tagmatch.Value);
                
                if (tag.IsClosing)
                {
                    currentBox = Do.FindParentBox(tag.TagName, currentBox, this);
                }
                else if(tag.IsSingle)
                {
                    HtmlBox foo = new HtmlBox(currentBox, tag);
                }
                else
                {
                    currentBox = new HtmlBox(currentBox, tag);
                }
                lastEnd = tagmatch.Index + tagmatch.Length - 1;
            }               
            
            string finaltext = DocumentSource.Substring((lastEnd > 0 ? lastEnd + 1 : 0), DocumentSource.Length - lastEnd - 1 + (lastEnd == 0 ? 1 : 0)) ;

            if (!string.IsNullOrEmpty(finaltext))
            {
                HtmlBox abox = new HtmlBox(currentBox);
                abox.Text = finaltext;
            }
        }

        /// <summary>
        /// Applies style to all boxes in the tree
        /// </summary>
        private void ApplyStyles(HtmlBox startBox)
        {
            bool someBlock = false;

            foreach (HtmlBox htmlBox in startBox.Boxes)
            {
                htmlBox.InheritStyle(htmlBox.ParentBox);

                if (htmlBox.HtmlTag != null)
                {
                    if (ElementStyleBlocks["all"].ContainsKey(htmlBox.HtmlTag.TagName))
                    {
                        ElementStyleBlocks["all"][htmlBox.HtmlTag.TagName].AssignStyleBlockToBox(htmlBox);
                    }

                    if (htmlBox.HtmlTag.IsAttributeExists("class") &&
                        ElementStyleBlocks["all"].ContainsKey("." + htmlBox.HtmlTag.Attributes["class"]))
                    {
                        ElementStyleBlocks["all"]["." + htmlBox.HtmlTag.Attributes["class"]].AssignStyleBlockToBox(htmlBox);
                    }
                    
                    htmlBox.HtmlTag.AssignAttributesValues(htmlBox);

                    if (htmlBox.HtmlTag.IsAttributeExists("style"))
                    {
                        HtmlBlock block = new HtmlBlock(htmlBox.HtmlTag.Attributes["style"]);
                        block.AssignStyleBlockToBox(htmlBox);
                    }

                    if (htmlBox.HtmlTag.TagName.Equals("style", StringComparison.CurrentCultureIgnoreCase) &&
                        htmlBox.Boxes.Count == 1)
                    {
                        ApplyStyleSheet(htmlBox.Boxes[0].Text);
                    } 

                    if (htmlBox.HtmlTag.TagName.Equals("link", StringComparison.CurrentCultureIgnoreCase) &&
                        htmlBox.GetAttribute("rel", string.Empty).Equals("stylesheet", StringComparison.CurrentCultureIgnoreCase))
                    {
                        ApplyStyleSheet(Do.GetStyleSheet(htmlBox.GetAttribute("href", string.Empty)));
                    }
                }

                ApplyStyles(htmlBox);
            }

            if (someBlock)
            {
                foreach (HtmlBox box in startBox.Boxes)
                {
                    box.Display = Constants.Block;
                }
            }
            
        }

        /// <summary>
        /// Makes block boxes be among only block boxes. 
        /// </summary>
        /// <param name="startBox"></param>
        private void AdjustBlock(HtmlBox startBox)
        {
            bool inlinesonly = startBox.HasInlinesOnly();

            if (!inlinesonly)
            {

                List<List<HtmlBox>> inlinegroups = GetInlineBoxes(startBox);

                foreach (List<HtmlBox> groupBox in inlinegroups)
                {
                    if (groupBox.Count == 0) continue;

                    if (groupBox.Count == 1 && groupBox[0].IsAnonymousSpace)
                    {
                        HtmlBox sbox = new HtmlBox(startBox, groupBox[0], Constants.None);
                        sbox.IsAnonymousSpaceBlock = true;
                        groupBox[0].ParentBox = sbox;
                    }
                    else
                    {
                        HtmlBox newbox = new HtmlBox(startBox, groupBox[0], Constants.Block);
                        newbox.IsAnonymousBlock = true;
                        foreach (HtmlBox inline in groupBox)
                        {
                            inline.ParentBox = newbox;
                        }
                    }
                }
            }

            foreach (HtmlBox htmlBox in startBox.Boxes)
            {
                AdjustBlock(htmlBox);
            }
        }


        /// <summary>
        /// Gets the inline boxes.
        /// </summary>
        private List<List<HtmlBox>> GetInlineBoxes(HtmlBox box)
        {
            List<List<HtmlBox>> result = new List<List<HtmlBox>>();
            List<HtmlBox> current = null;
            for (int i = 0; i < box.Boxes.Count; i++)
            {
                HtmlBox hBox = box.Boxes[i];

                if (hBox.Display == Constants.Inline)
                {
                    if (current == null)
                    {
                        current = new List<HtmlBox>();
                        result.Add(current);
                    }
                    current.Add(hBox);
                }
                else
                {
                    current = null;
                }
            }

            if (result.Count > 0 && result[result.Count - 1].Count == 0)
            {
                result.RemoveAt(result.Count - 1);
            }

            return result;
        }

        public override void CalcBoxBounds(Graphics g)
        {
            base.CalcBoxBounds(g);
        }

        #endregion
    }

    /// <summary>
    /// Represents HtmlTags
    /// </summary>
    internal class HtmlTag
    {
        #region Fields

        private string tagName;
        private bool isClosing;
        private Dictionary<string, string> attributes;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlTag"/> class.
        /// </summary>
        private HtmlTag()
        {
            attributes = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlTag"/> class.
        /// </summary>
        /// <param name="tag">The tag.</param>
        public HtmlTag(string tag)
            : this()
        {
            tag = tag.Substring(1, tag.Length - 2);

            int spaceIndex = tag.IndexOf(" ");

            if (spaceIndex < 0)
            {
                tagName = tag;
            }
            else
            {
                tagName = tag.Substring(0, spaceIndex);
            }

            if (tagName.StartsWith("/"))
            {
                isClosing = true;
                tagName = tagName.Substring(1);
            }

            tagName = tagName.ToLower();

            MatchCollection attributes = Do.MatchPattern(Constants.HmlTagAttributes, tag);

            foreach (Match attribute in attributes)
            {
                string[] chunks = attribute.Value.Split('=');

                if (chunks.Length == 1)
                {
                    if (!Attributes.ContainsKey(chunks[0]))
                        Attributes.Add(chunks[0].ToLower(), string.Empty);
                }
                else if (chunks.Length == 2)
                {
                    string attname = chunks[0].Trim();
                    string attvalue = chunks[1].Trim();

                    if (attvalue.StartsWith("\"") && attvalue.EndsWith("\"") && attvalue.Length > 2)
                    {
                        attvalue = attvalue.Substring(1, attvalue.Length - 2);
                    }

                    if (!Attributes.ContainsKey(attname))
                        Attributes.Add(attname, attvalue);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the attributes in the tag
        /// </summary>
        public Dictionary<string, string> Attributes
        {
            get { return attributes; }
        }

        /// <summary>
        /// Gets the name of this tag
        /// </summary>
        public string TagName
        {
            get { return tagName; }
        }

        /// <summary>
        /// Gets the value whether the tag is a closing tag
        /// </summary>
        public bool IsClosing
        {
            get { return isClosing; }
        }

        /// <summary>
        /// Gets the value whether the tag doesn't need a closing tag; 
        /// </summary>
        public bool IsSingle
        {
            get
            {
                return TagName.StartsWith("!")
                    || (new List<string>(
                            new string[]{
                             "area", "base", "basefont", "br", "col",
                             "frame", "hr", "img", "input", "isindex",
                             "link", "meta", "param"
                            }
                        )).Contains(TagName)
                    ;
            }
        }

        
        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="box"></param>
        public void AssignAttributesValues(HtmlBox box)
        {
            string t = TagName.ToUpper();

            foreach (string attribute in Attributes.Keys)
            {
                string value = Attributes[attribute];

                switch (attribute)
                {
                    case Constants.align:
                        if (value == Constants.left || value == Constants.center || value == Constants.right || value == Constants.justify)
                            box.Text_Align = value;
                        else
                            box.VerticalAlign = value;
                        break;
                    case Constants.background:
                        box.Background_Image = value;
                        break;
                    case Constants.bgcolor:
                        box.Background_Color = value;
                        break;
                    case Constants.border:
                        box.Border_Width = PixelLength(value);

                        if (t == Constants.TABLE)
                        {
                            AssignTableBorder(box, value);
                        }
                        else
                        {
                            box.Border_Style = Constants.Solid;
                        }
                        break;
                    case Constants.bordercolor:
                        box.Border_Color = value;
                        break;
                    case Constants.cellspacing:
                        box.Border_Spacing = PixelLength(value);
                        break;
                    case Constants.cellpadding:
                        AssignTablePadding(box, value);
                        break;
                    case Constants.color:
                        box.Color = value;
                        break;
                    case Constants.dir:
                        box.Direction = value;
                        break;
                    case Constants.face:
                        box.Font_Family = value;
                        break;
                    case Constants.height:
                        box.Height = PixelLength(value);
                        break;
                    case Constants.hspace:
                        box.Margin_Right = box.Margin_Left = PixelLength(value);
                        break;
                    case Constants.nowrap:
                        box.White_Space = Constants.Nowrap;
                        break;
                    case Constants.size:
                        if (t == Constants.HR)
                            box.Height = PixelLength(value);
                        if (t == Constants.FONT)
                            box.Font_Size = PixelLength(value);
                        break;
                    case Constants.valign:
                        box.VerticalAlign = value;
                        break;
                    case Constants.vspace:
                        box.Margin_Top = box.Margin_Bottom = PixelLength(value);
                        break;
                    case Constants.width:
                        box.Width = PixelLength(value);
                        break;
                }
            }
        }

        /// <summary>
        /// Converts an HTML length into a Css length
        /// </summary>
        private string PixelLength(string htmlLength)
        {

            if (Do.IsHtmlLengthHasError(htmlLength))
            {
                return htmlLength + "px";
            }

            return htmlLength;
        }

        /// <summary>
        /// Applies the table border.
        /// </summary>
        private void AssignTableBorder(HtmlBox table, string border)
        {
            foreach (HtmlBox box in table.Boxes)
            {
                foreach (HtmlBox cell in box.Boxes)
                {
                    cell.Border_Width = PixelLength(border);
                }
            }
        }

        /// <summary>
        /// Applies the table padding.
        /// </summary>
        private void AssignTablePadding(HtmlBox table, string padding)
        {
            foreach (HtmlBox box in table.Boxes)
            {
                foreach (HtmlBox cell in box.Boxes)
                {
                    cell.Padding = PixelLength(padding);

                }
            }
        }

        /// <summary>
        /// Gets the value indicating if the attribute list has the specified attribute
        /// </summary>
        public bool IsAttributeExists(string attribute)
        {
            return Attributes.ContainsKey(attribute);
        }

        public override string ToString()
        {
            return string.Format("<{1}{0}>", TagName, IsClosing ? "/" : string.Empty);
        }

        #endregion
    }

    internal class HtmlBox
    {
        #region Static Fields

        /// <summary>
        /// contains css properties.
        /// </summary>
        public static Dictionary<string, PropertyInfo> properties;

        /// <summary>
        /// Contains default values
        /// </summary>
        private static Dictionary<string, string> defaults;

        /// <summary>
        /// Contains all inhertiable properties
        /// </summary>
        private static List<PropertyInfo> inheritables;

        static HtmlBox()
        {
            #region Initialize properties, inheritables and defaults Dictionaries

            properties = new Dictionary<string, PropertyInfo>();
            defaults = new Dictionary<string, string>();
            inheritables = new List<PropertyInfo>();

            PropertyInfo[] props = typeof(HtmlBox).GetProperties();
            for (int i = 0; i < props.Length; i++)
            {
                if (Do.IsValidProperty(props[i].Name, "Actual"))
                {
                    properties.Add(props[i].Name.ToLower().Replace("_", "-"), props[i]);
                    defaults.Add(props[i].Name.ToLower().Replace("_", "-"), Do.GetPropertyDefaultValue(props[i].Name));
                    if (Do.IsValidProperty(props[i].Name, "inherited")) 
                        inheritables.Add(props[i]);
                }
            }
            #endregion
        }

        #endregion

        #region Fields

        private float lfCalculatedWordSpacing = float.NaN;
        private string backgroundColor;
        private string backgroundImage;
        private string backgroundRepeat;
        private string borderTopWidth;
        private string borderRightWidth;
        private string borderBottomWidth;
        private string borderLeftWidth;
        private string borderWidth;
        private string borderTopColor;
        private string borderRightColor;
        private string borderBottomColor;
        private string borderLeftColor;
        private string borderColor;
        private string borderTopStyle;
        private string borderRightStyle;
        private string borderBottomStyle;
        private string borderLeftStyle;
        private string borderStyle;
        private string borderBottom;
        private string borderLeft;
        private string borderRight;
        private string borderTop;
        private string borderSpacing;
        private string borderCollapse;
        private string border;
        private string color;
        private string emptyCells;
        private string direction;
        private string display;
        private string font;
        private string fontFamily;
        private string fontSize;
        private string fontStyle;
        private string fontVariant;
        private string fontWeight;
        private string lfloat;
        private string height;
        private string marginBottom;
        private string marginLeft;
        private string marginRight;
        private string marginTop;
        private string margin;
        private string lineHeight;
        private string listStyleType;
        private string listStyleImage;
        private string listStylePosition;
        private string listStyle;
        private string paddingLeft;
        private string paddingBottom;
        private string paddingRight;
        private string paddingTop;
        private string padding;
        private string text;
        private string textAlign;
        private string textDecoration;
        private string textIndent;
        private string position;
        private string verticalAlign;
        private string width;
        private string wordSpacing;
        private string whiteSpace;
        private List<HtmlWordBox> boxWords;
        private List<HtmlBox> boxes;
        private HtmlBox parentBox;
        private bool wordsSizeMeasured;
        private SizeF size;
        private PointF location;
        private List<HtmlLineBox> lineBoxes;
        private List<HtmlLineBox> parentLineBoxes;
        private float fontLineSpacing = float.NaN;
        private HtmlTag htmltag;
        private Dictionary<HtmlLineBox, RectangleF> rectangles = null;
        protected HtmlRootBox htmlRootBox = null;
        private HtmlBox listItemBox;
        private HtmlLineBox firstHostingLineBox;
        private HtmlLineBox lastHostingLineBox;
        private bool isAnonymous;
        private bool isAnonymousSpace;
        private bool isAnonymousBlock;
        private bool isAnonymousSpaceBlock;
        public bool TableFixed;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlBox"/> class.
        /// </summary>
        protected HtmlBox()
        {
            boxWords = new List<HtmlWordBox>();
            boxes = new List<HtmlBox>();
            lineBoxes = new List<HtmlLineBox>();
            parentLineBoxes = new List<HtmlLineBox>();
            rectangles = new Dictionary<HtmlLineBox, RectangleF>();

            #region Initialize properties with default values

            foreach (string prop in properties.Keys)
            {
                properties[prop].SetValue(this, defaults[prop], null);
            }

            #endregion
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlBox"/> class.
        /// </summary>
        internal HtmlBox(HtmlBox parent, string blockType)
            : this(parent)
        {
            Display = blockType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlBox"/> class.
        /// </summary>
        public HtmlBox(HtmlBox parent, HtmlBox insertBefore, string blockType)
            : this(parent, blockType)
        {
            int index = parent.Boxes.IndexOf(insertBefore);

            if (index < 0)
            {
                throw new Exception("insertBefore box doesn't exist on parent");
            }
            parent.Boxes.Remove(this);
            parent.Boxes.Insert(index, this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlBox"/> class.
        /// </summary>
        public HtmlBox(HtmlBox parentBox)
            : this()
        {
            ParentBox = parentBox;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlBox"/> class.
        /// </summary>
        internal HtmlBox(HtmlBox parentBox, HtmlTag tag)
            : this(parentBox)
        {
            htmltag = tag;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the width of the bottom border.
        /// </summary>
        /// <value>The width of the bottom border.</value>
        public string Bottom_Border_Width
        {
            get { return borderBottomWidth; }
            set { borderBottomWidth = value; }
        }

        /// <summary>
        /// Gets or sets the width of the border left.
        /// </summary>
        /// <value>The width of the border left.</value>
        public string Border_Left_Width
        {
            get { return borderLeftWidth; }
            set { borderLeftWidth = value; }
        }

        /// <summary>
        /// Gets or sets the width of the border right.
        /// </summary>
        /// <value>The width of the border right.</value>
        public string Border_Right_Width
        {
            get { return borderRightWidth; }
            set { borderRightWidth = value; }
        }

        /// <summary>
        /// Gets or sets the width of the border top.
        /// </summary>
        /// <value>The width of the border top.</value>
        public string Border_Top_Width
        {
            get { return borderTopWidth; }
            set { borderTopWidth = value; }
        }

        /// <summary>
        /// Gets or sets the width of the border.
        /// </summary>
        /// <value>The width of the border.</value>
        public string Border_Width
        {
            get { return borderWidth; }
            set
            {
                borderWidth = value;

                string[] values = Do.SplitValues(value);

                switch (values.Length)
                {
                    case 1:
                        Border_Top_Width = Border_Left_Width = Border_Right_Width = Bottom_Border_Width = values[0];
                        break;
                    case 2:
                        Border_Top_Width = Bottom_Border_Width = values[0];
                        Border_Left_Width = Border_Right_Width = values[1];
                        break;
                    case 3:
                        Border_Top_Width = values[0];
                        Border_Left_Width = Border_Right_Width = values[1];
                        Bottom_Border_Width = values[2];
                        break;
                    case 4:
                        Border_Top_Width = values[0];
                        Border_Right_Width = values[1];
                        Bottom_Border_Width = values[2];
                        Border_Left_Width = values[3];
                        break;
                    default:
                        break;
                }

            }
        }

        /// <summary>
        /// Gets or sets the border bottom style.
        /// </summary>
        /// <value>The border bottom style.</value>
        public string Border_Bottom_Style
        {
            get { return borderBottomStyle; }
            set { borderBottomStyle = value; }
        }

        /// <summary>
        /// Gets or sets the border left style.
        /// </summary>
        /// <value>The border left style.</value>
        public string Border_Left_Style
        {
            get { return borderLeftStyle; }
            set { borderLeftStyle = value; }
        }

        /// <summary>
        /// Gets or sets the border right style.
        /// </summary>
        /// <value>The border right style.</value>
        public string Border_Right_Style
        {
            get { return borderRightStyle; }
            set { borderRightStyle = value; }
        }

        /// <summary>
        /// Gets or sets the border style.
        /// </summary>
        /// <value>The border style.</value>
        public string Border_Style
        {
            get { return borderStyle; }
            set
            {
                borderStyle = value;

                string[] values = Do.SplitValues(value);

                switch (values.Length)
                {
                    case 1:
                        Border_Top_Style = Border_Left_Style = Border_Right_Style = Border_Bottom_Style = values[0];
                        break;
                    case 2:
                        Border_Top_Style = Border_Bottom_Style = values[0];
                        Border_Left_Style = Border_Right_Style = values[1];
                        break;
                    case 3:
                        Border_Top_Style = values[0];
                        Border_Left_Style = Border_Right_Style = values[1];
                        Border_Bottom_Style = values[2];
                        break;
                    case 4:
                        Border_Top_Style = values[0];
                        Border_Right_Style = values[1];
                        Border_Bottom_Style = values[2];
                        Border_Left_Style = values[3];
                        break;
                    default:
                        break;
                }

            }
        }

        /// <summary>
        /// Gets or sets the border top style.
        /// </summary>
        /// <value>The border top style.</value>
        public string Border_Top_Style
        {
            get { return borderTopStyle; }
            set { borderTopStyle = value; }
        }

        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        /// <value>The color of the border.</value>
        public string Border_Color
        {
            get { return borderColor; }
            set
            {
                borderColor = value;

                MatchCollection colors = Do.MatchPattern(Constants.CssColors, value);

                string[] values = new string[colors.Count];

                for (int i = 0; i < values.Length; i++)
                    values[i] = colors[i].Value;

                switch (values.Length)
                {
                    case 1:
                        Border_Top_Color = Border_Left_Color = Border_Right_Color = Border_Bottom_Color = values[0];
                        break;
                    case 2:
                        Border_Top_Color = Border_Bottom_Color = values[0];
                        Border_Left_Color = Border_Right_Color = values[1];
                        break;
                    case 3:
                        Border_Top_Color = values[0];
                        Border_Left_Color = Border_Right_Color = values[1];
                        Border_Bottom_Color = values[2];
                        break;
                    case 4:
                        Border_Top_Color = values[0];
                        Border_Right_Color = values[1];
                        Border_Bottom_Color = values[2];
                        Border_Left_Color = values[3];
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the border bottom.
        /// </summary>
        /// <value>The color of the border bottom.</value>
        public string Border_Bottom_Color
        {
            get { return borderBottomColor; }
            set { borderBottomColor = value; }
        }

        /// <summary>
        /// Gets or sets the color of the border left.
        /// </summary>
        /// <value>The color of the border left.</value>
        public string Border_Left_Color
        {
            get { return borderLeftColor; }
            set { borderLeftColor = value; }
        }


        /// <summary>
        /// Gets or sets the color of the border right.
        /// </summary>
        /// <value>The color of the border right.</value>
        public string Border_Right_Color
        {
            get { return borderRightColor; }
            set { borderRightColor = value; }
        }

        /// <summary>
        /// Gets or sets the color of the border top.
        /// </summary>
        /// <value>The color of the border top.</value>
        public string Border_Top_Color
        {
            get { return borderTopColor; }
            set { borderTopColor = value; }
        }

        /// <summary>
        /// Gets or sets the border.
        /// </summary>
        /// <value>The border.</value>
        public string Border
        {
            get { return border; }
            set
            {
                border = value;

                string borderWidth = Do.SearchPattern(Constants.CssBorderWidth, value);
                string borderStyle = Do.SearchPattern(Constants.CssBorderStyle, value);
                string borderColor = Do.SearchPattern(Constants.CssColors, value);

                if (borderWidth != null) Border_Width = borderWidth;
                if (borderStyle != null) Border_Style = borderStyle;
                if (borderColor != null) Border_Color = borderColor;
            }
        }

        /// <summary>
        /// Gets or sets the border bottom.
        /// </summary>
        /// <value>The border bottom.</value>
        public string Border_Bottom
        {
            get { return borderBottom; }
            set
            {
                borderBottom = value;

                string borderWidth = Do.SearchPattern(Constants.CssBorderWidth, value);
                string borderStyle = Do.SearchPattern(Constants.CssBorderStyle, value);
                string borderColor = Do.SearchPattern(Constants.CssColors, value);

                if (borderWidth != null) Bottom_Border_Width = borderWidth;
                if (borderStyle != null) Border_Bottom_Style = borderStyle;
                if (borderColor != null) Border_Bottom_Color = borderColor;
            }
        }

        /// <summary>
        /// Gets or sets the border left.
        /// </summary>
        /// <value>The border left.</value>
        public string Border_Left
        {
            get { return borderLeft; }
            set
            {
                borderLeft = value;

                string borderWidth = Do.SearchPattern(Constants.CssBorderWidth, value);
                string borderStyle = Do.SearchPattern(Constants.CssBorderStyle, value);
                string borderColor = Do.SearchPattern(Constants.CssColors, value);

                if (borderWidth != null) Border_Left_Width = borderWidth;
                if (borderStyle != null) Border_Left_Style = borderStyle;
                if (borderColor != null) Border_Left_Color = borderColor;
            }
        }

        /// <summary>
        /// Gets or sets the border right.
        /// </summary>
        /// <value>The border right.</value>
        public string Border_Right
        {
            get { return borderRight; }
            set
            {
                borderRight = value;

                string borderWidth = Do.SearchPattern(Constants.CssBorderWidth, value);
                string borderStyle = Do.SearchPattern(Constants.CssBorderStyle, value);
                string borderColor = Do.SearchPattern(Constants.CssColors, value);

                if (borderWidth != null) Border_Right_Width = borderWidth;
                if (borderStyle != null) Border_Right_Style = borderStyle;
                if (borderColor != null) Border_Right_Color = borderColor;
            }
        }

        /// <summary>
        /// Gets or sets the border top.
        /// </summary>
        /// <value>The border top.</value>
        public string Border_Top
        {
            get { return borderTop; }
            set
            {
                borderTop = value;

                string borderWidth = Do.SearchPattern(Constants.CssBorderWidth, value);
                string borderStyle = Do.SearchPattern(Constants.CssBorderStyle, value);
                string borderColor = Do.SearchPattern(Constants.CssColors, value);

                if (borderWidth != null) Border_Top_Width = borderWidth;
                if (borderStyle != null) Border_Top_Style = borderStyle;
                if (borderColor != null) Border_Top_Color = borderColor;
            }
        }

        /// <summary>
        /// Gets or sets the border spacing.
        /// </summary>
        /// <value>The border spacing.</value>
        public string Border_Spacing
        {
            get { return borderSpacing; }
            set { borderSpacing = value; }
        }

        /// <summary>
        /// Gets or sets the border collapse.
        /// </summary>
        /// <value>The border collapse.</value>
        public string Border_Collapse
        {
            get { return borderCollapse; }
            set { borderCollapse = value; }
        }

        /// <summary>
        /// Gets or sets the margin.
        /// </summary>
        /// <value>The margin.</value>
        public string Margin
        {
            get { return margin; }
            set
            {
                margin = value;
                string[] values = Do.SplitValues(value);

                switch (values.Length)
                {
                    case 1:
                        Margin_Top = Margin_Left = Margin_Right = Margin_Bottom = values[0];
                        break;
                    case 2:
                        Margin_Top = Margin_Bottom = values[0];
                        Margin_Left = Margin_Right = values[1];
                        break;
                    case 3:
                        Margin_Top = values[0];
                        Margin_Left = Margin_Right = values[1];
                        Margin_Bottom = values[2];
                        break;
                    case 4:
                        Margin_Top = values[0];
                        Margin_Right = values[1];
                        Margin_Bottom = values[2];
                        Margin_Left = values[3];
                        break;
                    default:
                        break;
                }

            }
        }

        /// <summary>
        /// Gets or sets the margin bottom.
        /// </summary>
        /// <value>The margin bottom.</value>
        public string Margin_Bottom
        {
            get { return marginBottom; }
            set { marginBottom = value; }
        }

        /// <summary>
        /// Gets or sets the margin left.
        /// </summary>
        /// <value>The margin left.</value>
        public string Margin_Left
        {
            get { return marginLeft; }
            set { marginLeft = value; }
        }

        /// <summary>
        /// Gets or sets the margin right.
        /// </summary>
        /// <value>The margin right.</value>
        public string Margin_Right
        {
            get { return marginRight; }
            set { marginRight = value; }
        }

        /// <summary>
        /// Gets or sets the margin top.
        /// </summary>
        /// <value>The margin top.</value>
        public string Margin_Top
        {
            get { return marginTop; }
            set { marginTop = value; }
        }

        /// <summary>
        /// Gets or sets the padding.
        /// </summary>
        /// <value>The padding.</value>
        public string Padding
        {
            get { return padding; }
            set
            {
                padding = value;

                string[] values = Do.SplitValues(value);

                switch (values.Length)
                {
                    case 1:
                        Padding_Top = Padding_Left = Padding_Right = Padding_Bottom = values[0];
                        break;
                    case 2:
                        Padding_Top = Padding_Bottom = values[0];
                        Padding_Left = Padding_Right = values[1];
                        break;
                    case 3:
                        Padding_Top = values[0];
                        Padding_Left = Padding_Right = values[1];
                        Padding_Bottom = values[2];
                        break;
                    case 4:
                        Padding_Top = values[0];
                        Padding_Right = values[1];
                        Padding_Bottom = values[2];
                        Padding_Left = values[3];
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the padding bottom.
        /// </summary>
        /// <value>The padding bottom.</value>
        public string Padding_Bottom
        {
            get { return paddingBottom; }
            set { paddingBottom = value; }
        }

        /// <summary>
        /// Gets or sets the padding left.
        /// </summary>
        /// <value>The padding left.</value>
        public string Padding_Left
        {
            get { return paddingLeft; }
            set { paddingLeft = value; }
        }

        /// <summary>
        /// Gets or sets the padding right.
        /// </summary>
        /// <value>The padding right.</value>
        public string Padding_Right
        {
            get { return paddingRight; }
            set { paddingRight = value; }
        }

        /// <summary>
        /// Gets or sets the padding top.
        /// </summary>
        /// <value>The padding top.</value>
        public string Padding_Top
        {
            get { return paddingTop; }
            set { paddingTop = value; }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public string Width
        {
            get { return width; }
            set { width = value; }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public string Height
        {
            get { return height; }
            set { height = value; }
        }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        public string Background_Color
        {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }

        /// <summary>
        /// Gets or sets the background image.
        /// </summary>
        /// <value>The background image.</value>
        public string Background_Image
        {
            get { return backgroundImage; }
            set { backgroundImage = value; }
        }

        /// <summary>
        /// Gets or sets the background repeat.
        /// </summary>
        /// <value>The background repeat.</value>
        public string Background_Repeat
        {
            get { return backgroundRepeat; }
            set { backgroundRepeat = value; }
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        public string Color
        {
            get { return color; }
            set { color = value; }
        }

        /// <summary>
        /// Gets or sets the display.
        /// </summary>
        /// <value>The display.</value>
        public string Display
        {
            get { return display; }
            set { display = value; }
        }

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>The direction.</value>
        public string Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        /// <summary>
        /// Gets or sets the empty cells.
        /// </summary>
        /// <value>The empty cells.</value>
        public string Empty_Cells
        {
            get { return emptyCells; }
            set { emptyCells = value; }
        }

        /// <summary>
        /// Gets or sets the float.
        /// </summary>
        /// <value>The float.</value>
        public string Float
        {
            get { return lfloat; }
            set { lfloat = value; }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public string Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Gets or sets the height of the line.
        /// </summary>
        /// <value>The height of the line.</value>
        public string Line_Height
        {
            get { return lineHeight; }
            set { lineHeight = CalcPixel(value); }
        }

        /// <summary>
        /// Gets or sets the vertical align.
        /// </summary>
        /// <value>The vertical align.</value>
        public string VerticalAlign
        {
            get { return verticalAlign; }
            set { verticalAlign = value; }
        }

        /// <summary>
        /// Gets or sets the text indent.
        /// </summary>
        /// <value>The text indent.</value>
        public string Text_Indent
        {
            get { return textIndent; }
            set { textIndent = CalcPixel(value); }
        }

        /// <summary>
        /// Gets or sets the text align.
        /// </summary>
        /// <value>The text align.</value>
        public string Text_Align
        {
            get { return textAlign; }
            set { textAlign = value; }
        }

        /// <summary>
        /// Gets or sets the text decoration.
        /// </summary>
        /// <value>The text decoration.</value>
        public string Text_Decoration
        {
            get { return textDecoration; }
            set { textDecoration = value; }
        }

        /// <summary>
        /// Gets or sets the white space.
        /// </summary>
        /// <value>The white space.</value>
        public string White_Space
        {
            get { return whiteSpace; }
            set { whiteSpace = value; }
        }

        /// <summary>
        /// Gets or sets the word spacing.
        /// </summary>
        /// <value>The word spacing.</value>
        public string Word_Spacing
        {
            get { return wordSpacing; }
            set { wordSpacing = CalcPixel(value); }
        }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>The font.</value>
        public string Font
        {
            get { return font; }
            set
            {
                font = value;

                int mustBePos;
                string mustBe = Do.SearchPattern(Constants.CssFontSizeAndLineHeight, value, out mustBePos);

                if (!string.IsNullOrEmpty(mustBe))
                {
                    mustBe = mustBe.Trim();
                    string leftSide = value.Substring(0, mustBePos);
                    string fontStyle = Do.SearchPattern(Constants.CssFontStyle, leftSide);
                    string fontVariant = Do.SearchPattern(Constants.CssFontVariant, leftSide);
                    string fontWeight = Do.SearchPattern(Constants.CssFontWeight, leftSide);

                    string rightSide = value.Substring(mustBePos + mustBe.Length);
                    string fontFamily = rightSide.Trim();

                    string fontSize = mustBe;
                    string lineHeight = string.Empty;

                    if (mustBe.Contains("/") && mustBe.Length > mustBe.IndexOf("/") + 1)
                    {
                        int slashPos = mustBe.IndexOf("/");
                        fontSize = mustBe.Substring(0, slashPos);
                        lineHeight = mustBe.Substring(slashPos + 1);
                    }

                    if (!string.IsNullOrEmpty(fontStyle)) 
                        Font_Style = fontStyle;
                    if (!string.IsNullOrEmpty(fontVariant)) 
                        Font_Variant = fontVariant;
                    if (!string.IsNullOrEmpty(fontWeight)) 
                        Font_Weight = fontWeight;
                    if (!string.IsNullOrEmpty(fontFamily)) 
                        Font_Family = fontFamily;
                    if (!string.IsNullOrEmpty(fontSize)) 
                        Font_Size = fontSize;
                    if (!string.IsNullOrEmpty(lineHeight)) 
                        Line_Height = lineHeight;
                }
            }
        }

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>The font family.</value>
        public string Font_Family
        {
            get { return fontFamily; }
            set
            {
                switch (value)
                {
                    case Constants.Serif:
                        fontFamily = Constants.FontSerif; break;
                    case Constants.SansSerif:
                        fontFamily = Constants.FontSansSerif; break;
                    case Constants.Cursive:
                        fontFamily = Constants.FontCursive; break;
                    case Constants.Fantasy:
                        fontFamily = Constants.FontFantasy; break;
                    case Constants.Monospace:
                        fontFamily = Constants.FontMonospace; break;
                    default:
                        fontFamily = value; break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>The size of the font.</value>
        public string Font_Size
        {
            get { return fontSize; }
            set
            {
                string length = Do.SearchPattern(Constants.HtmlLength, value);
                if (length != null)
                {
                    string computedValue = string.Empty;

                    if (Do.IsHtmlLengthHasError(length))
                    {
                        computedValue = defaults["font-size"];
                    }
                    else if (Do.GetUnit(length) == Do.Unit.Ems && ParentBox != null)
                    {
                        computedValue = Do.ConvertEmToPixels(ParentBox.GetFont().SizeInPoints,length,true);
                    }
                    else
                    {
                        computedValue = Do.GetLengthString(length);
                    }

                    fontSize = computedValue;
                }
                else
                {
                    fontSize = value;
                }

            }
        }

        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        /// <value>The font style.</value>
        public string Font_Style
        {
            get { return fontStyle; }
            set { fontStyle = value; }
        }

        /// <summary>
        /// Gets or sets the font variant.
        /// </summary>
        /// <value>The font variant.</value>
        public string Font_Variant
        {
            get { return fontVariant; }
            set { fontVariant = value; }
        }

        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        /// <value>The font weight.</value>
        public string Font_Weight
        {
            get { return fontWeight; }
            set { fontWeight = value; }
        }

        /// <summary>
        /// Gets or sets the list style.
        /// </summary>
        /// <value>The list style.</value>
        public string List_Style
        {
            get { return listStyle; }
            set { listStyle = value; }
        }

        /// <summary>
        /// Gets or sets the list style position.
        /// </summary>
        /// <value>The list style position.</value>
        public string List_Style_Position
        {
            get { return listStylePosition; }
            set { listStylePosition = value; }
        }

        /// <summary>
        /// Gets or sets the list style image.
        /// </summary>
        /// <value>The list style image.</value>
        public string List_Style_Image
        {
            get { return listStyleImage; }
            set { listStyleImage = value; }
        }

        /// <summary>
        /// Gets or sets the type of the list style.
        /// </summary>
        /// <value>The type of the list style.</value>
        public string List_Style_Type
        {
            get { return listStyleType; }
            set { listStyleType = value; }
        }

        /// <summary>
        /// Gets the list item box.
        /// </summary>
        /// <value>The list item box.</value>
        public HtmlBox ListItemBox
        {
            get
            {
                return listItemBox;
            }
        }

        /// <summary>
        /// Gets the width available on the box, counting padding and margin.
        /// </summary>
        public float AvailableWidth
        {
            get { return Size.Width - ParseFloat(Border_Left_Width, "BORDER") - ParseFloat(Padding_Left, "FLOAT") - ParseFloat(Padding_Right, "FLOAT") - ParseFloat(Border_Right_Width, "BORDER"); }
        }

        /// <summary>
        /// Gets the bounds of the box
        /// </summary>
        public RectangleF Bounds
        {
            get { return new RectangleF(Location, Size); }
            set { Location = value.Location; Size = value.Size; }
        }

        /// <summary>
        /// Gets or sets the bottom of the box. 
        /// </summary>
        public float ActualBottom
        {
            get
            {
                return Location.Y + Size.Height;
            }
            set
            {
                Size = new SizeF(Size.Width, value - Location.Y);
            }
        }

        /// <summary>
        /// Gets the children boxes of this box
        /// </summary>
        public List<HtmlBox> Boxes
        {
            get { return boxes; }
        }

        /// <summary>
        /// Gets the left of the client rectangle (Where content starts rendering)
        /// </summary>
        public float ClientLeft
        {
            get { return Location.X + ParseFloat(Border_Left_Width, "BORDER") + ParseFloat(Padding_Left, "FLOAT"); }
        }

        /// <summary>
        /// Gets the top of the client rectangle.
        /// </summary>
        public float ClientTop
        {
            get { return Location.Y + ParseFloat(Border_Top_Width, "BORDER") + ParseFloat(Padding_Top, "FLOAT"); }
        }

        /// <summary>
        /// Gets the right of the client rectangle
        /// </summary>
        public float ClientRight
        {
            get { return ActualRight - ParseFloat(Padding_Right, "FLOAT") - ParseFloat(Border_Right_Width, "BORDER"); }
        }

        /// <summary>
        /// Gets the bottom of the client rectangle
        /// </summary>
        public float ClientBottom
        {
            get { return ActualBottom - ParseFloat(Padding_Bottom, "FLOAT") - ParseFloat(Bottom_Border_Width, "BORDER"); }
        }

        /// <summary>
        /// Gets the client rectangle
        /// </summary>
        public RectangleF ClientRectangle
        {
            get { return RectangleF.FromLTRB(ClientLeft, ClientTop, ClientRight, ClientBottom); }
        }

        /// <summary>
        /// Gets the containing block-box of this box. 
        /// </summary>
        public HtmlBox ContainingBlock
        {
            get
            {
                if (ParentBox == null)
                {
                    return this; 
                }

                HtmlBox htmlBox = ParentBox;

                while (
                    htmlBox.Display != Constants.Block &&
                    htmlBox.Display != Constants.Table &&
                    htmlBox.Display != Constants.TableCell &&
                    htmlBox.ParentBox != null)
                {
                    htmlBox = htmlBox.ParentBox;
                }

                if (htmlBox == null) 
                    throw new Exception("There's no containing block.");

                return htmlBox;
            }
        }

        /// <summary>
        /// Gets the font's line spacing
        /// </summary>
        public float FontLineSpacing
        {
            get
            {
                if (float.IsNaN(fontLineSpacing))
                {
                    fontLineSpacing = Do.GetLineSpacing(GetFont());
                }

                return fontLineSpacing;
            }
        }

        /// <summary>
        /// Gets or sets the first linebox where content of this box appear
        /// </summary>
        internal HtmlLineBox FirstHostingLineBox
        {
            get { return firstHostingLineBox; }
            set { firstHostingLineBox = value; }
        }

        /// <summary>
        /// Gets or sets the last linebox where content of this box appear
        /// </summary>
        internal HtmlLineBox LastHostingLineBox
        {
            get { return lastHostingLineBox; }
            set { lastHostingLineBox = value; }
        }

        /// <summary>
        /// Gets the HTMLTag that hosts this box
        /// </summary>
        internal HtmlTag HtmlTag
        {
            get { return htmltag; }
        }

        /// <summary>
        /// Gets the HtmlContainer of the Box.
        /// </summary>
        public HtmlRootBox HtmlRootBox
        {
            get { return htmlRootBox; }
        }

        /// <summary>
        /// Gets if this box represents an image
        /// </summary>
        public bool IsImage
        {
            get { return Words.Count == 1 && Words[0].IsImage; }
        }

        public bool IsSpaceOrEmpty
        {
            get
            {
                if ((Words.Count == 0 && Boxes.Count == 0) ||
                (Words.Count == 1 && Words[0].IsSpaces) ||
                Boxes.Count == 1 && Boxes[0].IsAnonymousSpaceBlock) return true;

                foreach (HtmlWordBox word in Words)
                {
                    if (!word.IsSpaces)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the line-boxes of this box (if block box)
        /// </summary>
        internal List<HtmlLineBox> LineBoxes
        {
            get { return lineBoxes; }
        }

        /// <summary>
        /// Gets or sets the location of the box
        /// </summary>
        public PointF Location
        {
            get { return location; }
            set { location = value; }
        }

        /// <summary>
        /// Gets or sets the parent box of this box
        /// </summary>
        public HtmlBox ParentBox
        {
            get { return parentBox; }
            set
            {
                if (parentBox != null && parentBox.Boxes.Contains(this))
                {
                    parentBox.Boxes.Remove(this);
                }

                parentBox = value;

                if (value != null && !value.Boxes.Contains(this))
                {
                    parentBox.Boxes.Add(this);
                    htmlRootBox = value.HtmlRootBox;
                }
            }
        }

        /// <summary>
        /// Gets the linebox(es) that contains words of this box (if inline)
        /// </summary>
        internal List<HtmlLineBox> ParentLineBoxes
        {
            get { return parentLineBoxes; }
        }

        /// <summary>
        /// Gets the rectangles where this box should be painted
        /// </summary>
        internal Dictionary<HtmlLineBox, RectangleF> Rectangles
        {
            get
            {
                return rectangles;
            }
        }

        /// <summary>
        /// Gets the right of the box. When setting, it will affect only the width of the box.
        /// </summary>
        public float ActualRight
        {
            get { return Location.X + Size.Width; }
            set
            {
                Size = new SizeF(value - Location.X, Size.Height);
            }
        }

        /// <summary>
        /// Gets or sets the size of the box
        /// </summary>
        public SizeF Size
        {
            get { return size; }
            set { size = value; }
        }

        /// <summary>
        /// Gets or sets the inner text of the box
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; Do.SplitWords(this,Text,Words); }
        }
     
        public bool IsAnonymous
        {
            get { return isAnonymous; }
            set { isAnonymous = value; }
        }
   
        public bool IsAnonymousSpace
        {
            get { return isAnonymousSpace; }
            set { isAnonymousSpace = value; }
        }
  
        public bool IsAnonymousBlock
        {
            get { return isAnonymousBlock; }
            set { isAnonymousBlock = value; }
        }
    
        public bool IsAnonymousSpaceBlock
        {
            get { return isAnonymousSpaceBlock; }
            set { isAnonymousSpaceBlock = value; }
        }

        /// <summary>
        /// Gets the BoxWords of text in the box
        /// </summary>
        internal List<HtmlWordBox> Words
        {
            get { return boxWords; }
        }

        #endregion

        #region Methods

        public float ParseFloat(string Value, string sType)
        {
            float fReturnVal;
            fReturnVal = 0f;
            if (sType == "BORDER")
            {
                fReturnVal = Do.GetBorderWidth(Border_Top_Width, this);
                if (string.IsNullOrEmpty(Border_Top_Style) || Border_Top_Style == Constants.None)
                {
                    fReturnVal = 0f;
                }
            }
            if (sType == "FLOAT")
            {
                if (Value == Constants.Auto) Value = "0";
                fReturnVal = Do.ParseLength(Value, Size.Width, this);
            }
            if (sType == "HORIZONTAL" || sType == "VERTICAL")
            {
                MatchCollection matches = Do.MatchPattern(Constants.HtmlLength, Value);

                if (matches.Count == 0)
                {
                    fReturnVal = 0;
                }
                else if (matches.Count > 0)
                {
                    fReturnVal = Do.ParseLength(matches[0].Value, 1, this);
                }
                else
                {
                    if (sType == "VERTICAL") fReturnVal = Do.ParseLength(matches[1].Value, 1, this);
                }
            }
            return fReturnVal;
        }

        public Color ParseColor(string Value)
        {
            return Do.GetColor(Value);
        }

        public Font GetFont()
        {
            if (string.IsNullOrEmpty(Font_Family)) { Font_Family = Constants.FontSerif; }
            if (string.IsNullOrEmpty(Font_Size)) { Font_Size = Constants.FontSize + "pt"; }

            FontStyle ftStyle = System.Drawing.FontStyle.Regular;

            if (Font_Style == Constants.Italic || Font_Style == Constants.Oblique)
            {
                ftStyle |= System.Drawing.FontStyle.Italic;
            }

            if (Font_Weight != Constants.Normal && Font_Weight != Constants.Lighter && !string.IsNullOrEmpty(Font_Weight))
            {
                ftStyle |= System.Drawing.FontStyle.Bold;
            }

            float fsize = 0f;
            float parentSize = Constants.FontSize;

            if (ParentBox != null) parentSize = ParentBox.GetFont().Size;

            switch (Font_Size)
            {
                case Constants.Medium:
                    fsize = Constants.FontSize; break;
                case Constants.XXSmall:
                    fsize = Constants.FontSize - 4; break;
                case Constants.XSmall:
                    fsize = Constants.FontSize - 3; break;
                case Constants.Small:
                    fsize = Constants.FontSize - 2; break;
                case Constants.Large:
                    fsize = Constants.FontSize + 2; break;
                case Constants.XLarge:
                    fsize = Constants.FontSize + 3; break;
                case Constants.XXLarge:
                    fsize = Constants.FontSize + 4; break;
                case Constants.Smaller:
                    fsize = parentSize - 2; break;
                case Constants.Larger:
                    fsize = parentSize + 2; break;
                default:
                    fsize = Do.ParseLength(Font_Size, parentSize, this, parentSize, true);
                    break;
            }

            if (fsize <= 1f) fsize = Constants.FontSize;


            return new Font(Font_Family, fsize, ftStyle);
        }
        /// <summary>
        /// Sets the initial Html box of the box
        /// </summary>
        private void SetHtmlRootBox(HtmlRootBox rootBox)
        {
            htmlRootBox = rootBox;
        }

        /// <summary>
        /// Returns false if some of the boxes
        /// </summary>
        internal bool HasInlinesOnly()
        {
            foreach (HtmlBox htmlBox in Boxes)
            {
                if (htmlBox.Display != Constants.Inline)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the index of the box to be used on a (ordered) list
        /// </summary>
        private int GetIndexForList()
        {
            int index = 0;

            foreach (HtmlBox htmlBox in ParentBox.Boxes)
            {
                if (htmlBox.Display == Constants.ListItem) index++;

                if (htmlBox.Equals(this)) return index;
            }

            return index;
        }

        /// <summary>
        /// Creates the <see cref="ListItemBox"/>
        /// </summary>
        private void CreateListItemBox(Graphics g)
        {
            if (Display == Constants.ListItem)
            {
                if (listItemBox == null)
                {
                    listItemBox = new HtmlBox();
                    listItemBox.InheritStyle(this);
                    listItemBox.Display = Constants.Inline;
                    listItemBox.SetHtmlRootBox(HtmlRootBox);

                    if (ParentBox != null && List_Style_Type == Constants.Decimal)
                    {
                        listItemBox.Text = GetIndexForList().ToString() + ".";
                    }
                    else
                    {
                        listItemBox.Text = "?";
                    }

                    listItemBox.CalcBoxBounds(g);
                    listItemBox.Size = new SizeF(listItemBox.Words[0].Width, listItemBox.Words[0].Height);
                }
                listItemBox.Words[0].Left = Location.X - listItemBox.Size.Width - 5;
                listItemBox.Words[0].Top = Location.Y + ParseFloat(Padding_Top, "FLOAT");
            }
        }
        /// <summary>
        /// Searches for the first word occourence inside the box, on the specified linebox
        /// </summary>
        internal HtmlWordBox FirstWordMatch(HtmlBox htmlBox, HtmlLineBox line)
        {
            if (htmlBox.Words.Count == 0 && htmlBox.Boxes.Count == 0)
            {
                return null;
            }

            if (htmlBox.Words.Count > 0)
            {
                foreach (HtmlWordBox word in htmlBox.Words)
                {
                    if (line.Words.Contains(word))
                    {
                        return word;
                    }
                }
                return null;
            }
            else
            {
                foreach (HtmlBox box in htmlBox.Boxes)
                {
                    HtmlWordBox wordBox = FirstWordMatch(box, line);

                    if (wordBox != null)
                    {
                        return wordBox;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the specified Attribute, returns string.Empty if no attribute specified
        /// </summary>
        internal string GetAttribute(string attribute)
        {
            return GetAttribute(attribute, string.Empty);
        }

        /// <summary>
        /// Gets the value of the specified attribute of the source HTML tag.
        /// </summary>
        internal string GetAttribute(string attribute, string defaultValue)
        {
            if (HtmlTag == null)
            {
                return defaultValue;
            }

            if (!HtmlTag.IsAttributeExists(attribute))
            {
                return defaultValue;
            }

            return HtmlTag.Attributes[attribute];
        }

        /// <summary>
        /// Gets the height of the font in the specified units
        /// </summary>
        public float GetEmHeight()
        {
            float height = GetFont().GetHeight();
            return height;
        }

        /// <summary>
        /// Gets the previous sibling of this box.
        /// </summary>
        private HtmlBox GetPreviousBox(HtmlBox box)
        {
            if (box.ParentBox == null)
            {
                return null; 
            }

            int index = box.ParentBox.Boxes.IndexOf(this);

            if (index < 0) throw new Exception("Child Box is not available in parent");

            if (index == 0) return null; 


            int diff = 1;
            HtmlBox siblingBox = box.ParentBox.Boxes[index - diff];

            while ((siblingBox.Display == Constants.None || siblingBox.Position == Constants.Absolute) && index - diff - 1 >= 0)
            {
                siblingBox = box.ParentBox.Boxes[index - ++diff];
            }

            return siblingBox.Display == Constants.None ? null : siblingBox;
        }

        internal float GetMinWidth()
        {
            float maxwidth = 0f;
            float padding = 0f;
            HtmlWordBox word = null;

            GetMinWidthLongWord(this, ref maxwidth, ref word);

            if (word != null)
            {
                GetMinWidthBubblePadding(word.OwnerBox, this, ref padding);
            }

            return maxwidth + padding;
        }

        private void GetMinWidthBubblePadding(HtmlBox box, HtmlBox endbox, ref float sum)
        {
            float padding = box.ParseFloat(box.Border_Left_Width, "BORDER") + box.ParseFloat(box.Padding_Left, "FLOAT") +
                 box.ParseFloat(box.Border_Right_Width, "BORDER") + box.ParseFloat(box.Padding_Right, "FLOAT");

            sum += padding;

            if (!box.Equals(endbox))
            {
                GetMinWidthBubblePadding(box.ParentBox, endbox, ref sum);
            }
        }

        /// <summary>
        /// Gets the longest word (in width) inside the box, deeply.
        /// </summary>
        private void GetMinWidthLongWord(HtmlBox box, ref float maxw, ref HtmlWordBox word)
        {

            if (box.Words.Count > 0)
            {
                foreach (HtmlWordBox wdBox in box.Words)
                {
                    if (wdBox.FullWidth > maxw)
                    {
                        maxw = wdBox.FullWidth;
                        word = wdBox;
                    }
                }
            }
            else
            {
                foreach (HtmlBox htmlBox in box.Boxes)
                    GetMinWidthLongWord(htmlBox, ref maxw, ref word);
            }

        }

        /// <summary>
        /// Gets the maximum bottom of the boxes inside the startBox
        /// </summary>
        internal float GetMaxBottom(HtmlBox startBox, float currentMaxBottom)
        {
            foreach (HtmlLineBox line in startBox.Rectangles.Keys)
            {
                currentMaxBottom = Math.Max(currentMaxBottom, startBox.Rectangles[line].Bottom);
            }

            foreach (HtmlBox box in startBox.Boxes)
            {
                currentMaxBottom = Math.Max(currentMaxBottom, box.ActualBottom);
                currentMaxBottom = Math.Max(currentMaxBottom, GetMaxBottom(box, currentMaxBottom));
            }

            return currentMaxBottom;
        }

        /// <summary>
        /// Get the width of the box at full width (No line breaks)
        /// </summary>
        internal float GetWidth(Graphics g)
        {
            float sum = 0f;
            float paddingsum = 0f;
            GetFullWordsWidth(this, g, ref sum, ref paddingsum);

            return paddingsum + sum;
        }

        /// <summary>
        /// Gets the longest word (in width) inside the box, deeply.
        /// </summary>
        private void GetFullWordsWidth(HtmlBox htmlBox, Graphics g, ref float sum, ref float paddingsum)
        {
            if (htmlBox.Display != Constants.Inline)
            {
                sum = 0;
            }

            paddingsum += htmlBox.ParseFloat(htmlBox.Border_Left_Width, "BORDER") + htmlBox.ParseFloat(htmlBox.Border_Right_Width, "BORDER") + htmlBox.ParseFloat(htmlBox.Padding_Right, "FLOAT") + htmlBox.ParseFloat(Padding_Left, "FLOAT");

            if (htmlBox.Words.Count > 0)
            {
                foreach (HtmlWordBox word in htmlBox.Words)
                    sum += word.FullWidth;
            }
            else
            {
                foreach (HtmlBox box in htmlBox.Boxes)
                {
                    GetFullWordsWidth(box, g, ref sum, ref paddingsum);
                }
            }

        }

        /// <summary>
        /// Inherits inheritable values from specified box.
        /// </summary>
        internal void InheritStyle(HtmlBox box)
        {
            if (box != null)
            {
                IEnumerable<PropertyInfo> propInfo = inheritables;
                foreach (PropertyInfo prop in propInfo)
                {
                    prop.SetValue(this,
                        prop.GetValue(box, null),
                        null);
                }
            }
        }

        /// <summary>
        /// Gets the result of collapsing the vertical margins of the two boxes
        /// </summary>
        private float MarginCollapse(HtmlBox htmlBox, HtmlBox box)
        {

            return Math.Max(
                htmlBox == null ? 0 : htmlBox.ParseFloat(htmlBox.Margin_Bottom, "FLOAT"),
                box == null ? 0 : box.ParseFloat(box.Margin_Top, "FLOAT"));
        }

        /// <summary>
        /// Calculates the bounds of box and children, recursively.
        /// </summary>
        public virtual void CalcBoxBounds(Graphics g)
        {
            if (Display == Constants.None) return;

            ClearRectangles();

            CalculateWordsSize(g);

            if (Display == Constants.Block ||
                Display == Constants.ListItem ||
                Display == Constants.Table ||
                Display == Constants.InlineTable ||
                Display == Constants.TableCell ||
                Display == Constants.None)
            {
                if (Display != Constants.TableCell)
                {
                    HtmlBox prevSibling = GetPreviousBox(this);
                    float left = ContainingBlock.Location.X + ContainingBlock.ParseFloat(ContainingBlock.Padding_Left, "FLOAT") + ParseFloat(Margin_Left, "FLOAT") + ContainingBlock.ParseFloat(ContainingBlock.Border_Left_Width, "BORDER");
                    float top =
                        (prevSibling == null && ParentBox != null ? ParentBox.ClientTop : 0) +
                        MarginCollapse(prevSibling, this) +
                        (prevSibling != null ? prevSibling.ActualBottom + prevSibling.ParseFloat(prevSibling.Bottom_Border_Width, "BORDER") : 0);
                    if (this.ParentBox != null)
                    {
                        Location = new PointF(left, top);
                        ActualBottom = top;
                    }
                }

                if (Display != Constants.TableCell &&
                    Display != Constants.Table) 
                {
                    float minwidth = GetMinWidth();
                    float width =
                        ContainingBlock.Size.Width
                        - ContainingBlock.ParseFloat(ContainingBlock.Padding_Left, "FLOAT") - ContainingBlock.ParseFloat(ContainingBlock.Padding_Right, "FLOAT")
                        - ContainingBlock.ParseFloat(ContainingBlock.Border_Left_Width, "BORDER") - ContainingBlock.ParseFloat(ContainingBlock.Border_Right_Width, "BORDER")
                        - ParseFloat(Margin_Left, "FLOAT") - ParseFloat(Margin_Right, "FLOAT") - ParseFloat(Border_Left_Width, "BORDER") - ParseFloat(Border_Right_Width, "BORDER");

                    if (Width != Constants.Auto && !string.IsNullOrEmpty(Width))
                    {
                        width = Do.ParseLength(Width, width, this);
                    }

                    if (width < minwidth) width = minwidth;

                    Size = new SizeF(width, Size.Height);

                }

                if (Display == Constants.Table || Display == Constants.InlineTable)
                {
                    HtmlTable table = new HtmlTable(this, g);
                }
                else if (Display != Constants.None)
                {
                    if (HasInlinesOnly())
                    {
                        ActualBottom = Location.Y;
                        Do.ConstructLineBoxes(g, this); 
                    }
                    else
                    {
                        HtmlBox lastOne = null; 

                        foreach (HtmlBox box in Boxes)
                        {
                            if (box.Display == Constants.None) continue;
                            box.CalcBoxBounds(g);
                            lastOne = box;
                        }

                        if (lastOne != null)
                            ActualBottom = Math.Max(ActualBottom, lastOne.ActualBottom + lastOne.ParseFloat(lastOne.Margin_Bottom, "FLOAT") + ParseFloat(Padding_Bottom, "FLOAT"));
                    }
                }
            }

            if (HtmlRootBox != null)
            {
                HtmlRootBox.MaxSize = new SizeF(
                    Math.Max(HtmlRootBox.MaxSize.Width, ActualRight),
                    Math.Max(HtmlRootBox.MaxSize.Height, ActualBottom));
            }
        }

        /// <summary>
        /// Calculate the word spacing
        /// </summary>
        private void CalculateWordSpacing(Graphics g)
        {
            lfCalculatedWordSpacing = Do.WhiteSpace(g, this);

            if (Word_Spacing != Constants.Normal)
            {
                string len = Do.SearchPattern(Constants.HtmlLength, Word_Spacing);

                lfCalculatedWordSpacing += Do.ParseLength(len, 1, this);
            }
        }

        /// <summary>
        /// Assigns words its width and height
        /// </summary>
        internal void CalculateWordsSize(Graphics g)
        {
            if (wordsSizeMeasured) return;

            if (float.IsNaN(lfCalculatedWordSpacing))
                CalculateWordSpacing(g);

            if (HtmlTag != null && HtmlTag.TagName.Equals("img", StringComparison.CurrentCultureIgnoreCase))
            {
                HtmlWordBox word = new HtmlWordBox(this, Do.GetImage(GetAttribute("src")));
                Words.Clear();
                Words.Add(word);
            }
            else
            {
                bool lastWasSpace = false;

                foreach (HtmlWordBox wordBox in Words)
                {
                    bool collapse = (this.White_Space == Constants.Normal ||
                                    this.White_Space == Constants.Nowrap ||
                                    this.White_Space == Constants.PreLine);
                    if (this.White_Space == Constants.Normal || this.White_Space == Constants.Nowrap) 
                        wordBox.ReplaceLineBreaksAndTabs();

                    if (wordBox.IsSpaces)
                    {
                        wordBox.Height = FontLineSpacing;

                        if (wordBox.IsTab)
                        {
                            wordBox.Width = lfCalculatedWordSpacing * 4; 
                        }
                        else if (wordBox.IsLineBreak)
                        {
                            wordBox.Width = 0;
                        }
                        else
                        {
                            if (!(lastWasSpace && collapse))
                            {
                                wordBox.Width = lfCalculatedWordSpacing * (collapse ? 1 : wordBox.Text.Length);
                            }
                        }

                        lastWasSpace = true;
                    }
                    else
                    {
                        string word = wordBox.Text;

                        CharacterRange[] measurable = { new CharacterRange(0, word.Length) };
                        StringFormat stringFormat = new StringFormat();

                        stringFormat.SetMeasurableCharacterRanges(measurable);

                        Region[] regions = g.MeasureCharacterRanges(word, GetFont(),
                            new RectangleF(0, 0, float.MaxValue, float.MaxValue),
                            stringFormat);

                        SizeF sizeF = regions[0].GetBounds(g).Size;
                        PointF pointF = regions[0].GetBounds(g).Location;

                        wordBox.LastMeasureOffset = new PointF(pointF.X, pointF.Y);
                        wordBox.Width = sizeF.Width;
                        wordBox.Height = sizeF.Height;

                        lastWasSpace = false;
                    }
                }
            }

            wordsSizeMeasured = true;
        }

        /// <summary>
        /// Ensures that the specified length is converted to pixels if necessary
        /// </summary>
        private string CalcPixel(string length)
        {
            if (Do.GetUnit(length) == Do.Unit.Ems)
            {
                length = Do.ConvertEmToPixels(GetEmHeight(),length, false);
            }

            return length;
        }

        /// <summary>
        /// Deeply offsets the top of the box and its contents
        /// </summary>
        internal void OffsetTop(float amount)
        {
            List<HtmlLineBox> lines = new List<HtmlLineBox>();
            foreach (HtmlLineBox line in Rectangles.Keys)
                lines.Add(line);

            foreach (HtmlLineBox line in lines)
            {
                RectangleF r = Rectangles[line];
                Rectangles[line] = new RectangleF(r.X, r.Y + amount, r.Width, r.Height);
            }

            foreach (HtmlWordBox word in Words)
            {
                word.Top += amount;
            }

            foreach (HtmlBox box in Boxes)
            {
                box.OffsetTop(amount);
            }
            Location = new PointF(Location.X, Location.Y + amount);
        }


        /// <summary>
        /// Paints the specified g.
        /// </summary>
        public void Paint(Graphics g)
        {
            if (Display == Constants.None)
                return;

            if (Display == Constants.TableCell &&
                Empty_Cells == Constants.Hide &&
                IsSpaceOrEmpty)
                return;

            List<RectangleF> areas = Rectangles.Count == 0 ?
                new List<RectangleF>(new RectangleF[] { Bounds }) :
                new List<RectangleF>(Rectangles.Values);

            RectangleF[] rects = areas.ToArray();

            for (int i = 0; i < rects.Length; i++)
            {
                RectangleF actualRect = rects[i];
                PaintBackground(g, actualRect);
                PaintBorder(g, actualRect, i == 0, i == rects.Length - 1);
            }

            if (IsImage)
            {
                RectangleF rectangleF = Words[0].Bounds; 
                rectangleF.Height -= ParseFloat(Border_Top_Width, "BORDER") + ParseFloat(Bottom_Border_Width, "BORDER") + ParseFloat(Padding_Top, "FLOAT") + ParseFloat(Padding_Bottom, "FLOAT");
                rectangleF.Y += ParseFloat(Border_Top_Width, "BORDER") + ParseFloat(Padding_Top, "FLOAT");
                g.DrawImage(Words[0].Image, Rectangle.Round(rectangleF));
            }
            else
            {
                Font f = GetFont();
                using (SolidBrush solidBrush = new SolidBrush(Do.GetColor(Color)))
                {
                    foreach (HtmlWordBox word in Words)
                    {
                        g.DrawString(word.Text, f, solidBrush, word.Left - word.LastMeasureOffset.X , word.Top);
                    }
                }

            }
            for (int i = 0; i < rects.Length; i++)
            {
                RectangleF actualRect = rects[i]; 

                PaintAdornment(g, actualRect, i == 0, i == rects.Length - 1);
            }

            foreach (HtmlBox box in Boxes)
            {
                box.Paint(g);
            }

            CreateListItemBox(g);

            if (ListItemBox != null)
            {
                ListItemBox.Paint(g);
            }
        }


        /// <summary>
        /// Paints the border.
        /// </summary>
        private void PaintBorder(Graphics g, RectangleF rectangle, bool isFirst, bool isLast)
        {

            SmoothingMode smooth = g.SmoothingMode;

            if (!(string.IsNullOrEmpty(Border_Top_Style) || Border_Top_Style == Constants.None))
            {
                using (SolidBrush solidBrush = new SolidBrush(ParseColor(Border_Top_Color)))
                {
                    if (Border_Top_Style == Constants.Inset) solidBrush.Color = Do.DarkTheColor(ParseColor(Border_Top_Color));
                    g.FillPath(solidBrush, Do.GetBorderPath(Do.Border.Top, this, rectangle, isFirst, isLast));
                }
            }


            if (isLast)
            {
                if (!(string.IsNullOrEmpty(Border_Right_Style) || Border_Right_Style == Constants.None))
                {
                    using (SolidBrush solidBrush = new SolidBrush(ParseColor(Border_Right_Color)))
                    {
                        if (Border_Right_Style == Constants.Outset) solidBrush.Color = Do.DarkTheColor(ParseColor(Border_Right_Color));
                        g.FillPath(solidBrush, Do.GetBorderPath(Do.Border.Right, this, rectangle, isFirst, isLast));
                    }
                }
            }

            if (!(string.IsNullOrEmpty(Border_Bottom_Style) || Border_Bottom_Style == Constants.None))
            {
                using (SolidBrush solidBrush = new SolidBrush(ParseColor(Border_Bottom_Color)))
                {
                    if (Border_Bottom_Style == Constants.Outset) solidBrush.Color = Do.DarkTheColor(ParseColor(Border_Bottom_Color));
                    g.FillPath(solidBrush, Do.GetBorderPath(Do.Border.Bottom, this, rectangle, isFirst, isLast));
                }
            }

            if (isFirst)
            {
                if (!(string.IsNullOrEmpty(Border_Left_Style) || Border_Left_Style == Constants.None))
                {
                    using (SolidBrush solidBrush = new SolidBrush(ParseColor(Border_Left_Color)))
                    {
                        if (Border_Left_Style == Constants.Inset) solidBrush.Color = Do.DarkTheColor(ParseColor(Border_Left_Color));
                        g.FillPath(solidBrush, Do.GetBorderPath(Do.Border.Left, this, rectangle, isFirst, isLast));
                    }
                }
            }

            g.SmoothingMode = smooth;

        }

        /// <summary>
        /// Paints the background.
        /// </summary>
        private void PaintBackground(Graphics g, RectangleF rectangle)
        {
            if (ContainingBlock.Text_Align == Constants.Justify) 
                return;

            Brush brush = new SolidBrush(ParseColor(Background_Color));
            SmoothingMode smooth = g.SmoothingMode;
            g.FillRectangle(brush, rectangle);
            g.SmoothingMode = smooth;
            if (brush != null)
            {
                brush.Dispose();
            }
        }

        /// <summary>
        /// Paints the Decorations like UnderLine, Strike etc.
        /// </summary>
        private void PaintAdornment(Graphics g, RectangleF rectangle, bool isFirst, bool isLast)
        {
            if (string.IsNullOrEmpty(Text_Decoration) || Text_Decoration == Constants.None || IsImage) 
                return;

            float desc = Do.GetDescent(GetFont());
            float asc = Do.GetAscent(GetFont());
            float y = 0f;

            if (Text_Decoration == Constants.Underline)
            {
                y = rectangle.Bottom - desc;
            }
            else if (Text_Decoration == Constants.LineThrough)
            {
                y = rectangle.Bottom - desc - asc / 2;
            }
            else if (Text_Decoration == Constants.Overline)
            {
                y = rectangle.Bottom - desc - asc - 2;
            }

            y -= ParseFloat(Padding_Bottom, "FLOAT") - ParseFloat(Bottom_Border_Width, "BORDER");

            float x1 = rectangle.X;
            float x2 = rectangle.Right;

            if (isFirst) x1 += ParseFloat(Padding_Left, "FLOAT") + ParseFloat(Border_Left_Width, "BORDER");
            if (isLast) x2 -= ParseFloat(Padding_Right, "FLOAT") + ParseFloat(Border_Right_Width, "BORDER");

            g.DrawLine(new Pen(ParseColor(Color)), x1, y, x2, y);
        }

        /// <summary>
        /// Offsets the rectangle of the specified linebox by the specified value.
        /// </summary>
        internal void OffsetRectangle(HtmlLineBox lineBox, float gap)
        {
            if (Rectangles.ContainsKey(lineBox))
            {
                RectangleF rectangle = Rectangles[lineBox];
                Rectangles[lineBox] = new RectangleF(rectangle.X, rectangle.Y + gap, rectangle.Width, rectangle.Height);
            }
        }

        /// <summary>
        /// Resets the <see cref="Rectangles"/> array
        /// </summary>
        internal void ClearRectangles()
        {
            rectangles.Clear();
        }

        /// <summary>
        /// Removes boxes that are just blank spaces
        /// </summary>
        internal void RemoveAnonymousSpaces()
        {
            for (int i = 0; i < Boxes.Count; i++)
            {
                if (Boxes[i].IsAnonymousSpaceBlock || Boxes[i].IsAnonymousSpace)
                {
                    Boxes.RemoveAt(i);
                    i--;
                }
            }
        }
        /// <summary>
        /// ToString override.
        /// </summary>
        public override string ToString()
        {
            string t = GetType().Name;
            if (HtmlTag != null)
            {
                t = string.Format("<{0}>", HtmlTag.TagName);
            }

            if (ParentBox == null)
            {
                return "HtmlRootBox";
            }
            else if (Display == Constants.Block)
            {
                return string.Format("{0} BlockBox {2}, Children:{1}", t, Boxes.Count, Font_Size);
            }
            else if (Display == Constants.None)
            {
                return string.Format("{0} None", t);
            }
            else
            {
                return string.Format("{0} {2}: {1}", t, Text, Display);
            }
        }
        #endregion

    }

    internal class HtmlBlock
    {
        #region Fields
        private string block;
        private Dictionary<PropertyInfo, string> propertyValues;
        private Dictionary<string, string> properties;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlBlock"/> class.
        /// </summary>
        private HtmlBlock()
        {
            propertyValues = new Dictionary<PropertyInfo, string>();
            properties = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlBlock"/> class.
        /// </summary>
        /// <param name="blockSource">The block source.</param>
        internal HtmlBlock(string blockSource)
            : this()
        {
            block = blockSource;

            MatchCollection matches = Do.MatchPattern(Constants.CssProperties, blockSource);

            foreach (Match match in matches)
            {
                string[] chunks = match.Value.Split(':');

                if (chunks.Length != 2) continue;

                string propName = chunks[0].Trim();
                string propValue = chunks[1].Trim();

                if (propValue.EndsWith(";")) propValue = propValue.Substring(0, propValue.Length - 1).Trim();

                Properties.Add(propName, propValue);

                if (HtmlBox.properties.ContainsKey(propName))
                    PropertyValues.Add(HtmlBox.properties[propName], propValue);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the properties and its values
        /// </summary>
        public Dictionary<string, string> Properties
        {
            get { return properties; }
        }

        /// <summary>
        /// Gets the dictionary with property-ready values
        /// </summary>
        public Dictionary<PropertyInfo, string> PropertyValues
        {
            get { return propertyValues; }
        }

        #endregion

        #region Method

        /// <summary>
        /// Updates the PropertyValues dictionary
        /// </summary>
        internal void ChangePropertyValues()
        {
            PropertyValues.Clear();

            foreach (string prop in Properties.Keys)
            {
                if (HtmlBox.properties.ContainsKey(prop))
                    PropertyValues.Add(HtmlBox.properties[prop], Properties[prop]);
            }
        }
        /// <summary>
        /// Asigns the style on this block o the specified box
        /// </summary>
        public void AssignStyleBlockToBox(HtmlBox b)
        {
            foreach (PropertyInfo prop in PropertyValues.Keys)
            {
                string value = PropertyValues[prop];

                if (value == Constants.Inherit && b.ParentBox != null)
                {
                    value = Convert.ToString(prop.GetValue(b.ParentBox, null));
                }

                prop.SetValue(b, value, null);
            }
        }
        #endregion
    }

    internal class HtmlWordBox
    {
        #region Fields

        private float left;
        private float top;
        private float width;
        private float height;
        private string word;
        private PointF lastMeasureOffset;
        private HtmlBox ownerBox;
        private Image image;


        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlWordBox"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal HtmlWordBox(HtmlBox owner)
        {
            ownerBox = owner;
            word = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlWordBox"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="image">The image.</param>
        public HtmlWordBox(HtmlBox owner, Image image)
            : this(owner)
        {
            Image = image;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the width of the word including white-spaces
        /// </summary>
        public float FullWidth
        {
            get { return Width; }
        }

        /// <summary>
        /// Gets the image this words represents (if one)
        /// </summary>
        public Image Image
        {
            get { return image; }
            set
            {
                image = value;

                if (value != null)
                {
                    if (Do.GetHtmlLengthNumber(OwnerBox.Width) > 0 && Do.GetUnit(OwnerBox.Width) == Do.Unit.Pixels)
                    {
                        Width = Do.GetHtmlLengthNumber(OwnerBox.Width);
                    }
                    else
                    {
                        Width = value.Width;
                    }

                    if (Do.GetHtmlLengthNumber(OwnerBox.Height) > 0 && Do.GetUnit(OwnerBox.Height) == Do.Unit.Pixels)
                    {

                        Height = Do.GetHtmlLengthNumber(OwnerBox.Height);
                    }
                    else
                    {
                        Height = value.Height;
                    }

                    Height += OwnerBox.ParseFloat(OwnerBox.Bottom_Border_Width, "BORDER") + OwnerBox.ParseFloat(OwnerBox.Border_Top_Width, "BORDER") + OwnerBox.ParseFloat(OwnerBox.Padding_Top, "FLOAT") + OwnerBox.ParseFloat(OwnerBox.Padding_Bottom, "FLOAT");

                }
            }
        }

        /// <summary>
        /// Gets if the word represents an image.
        /// </summary>
        public bool IsImage
        {
            get { return Image != null; }
        }

        /// <summary>
        /// Gets a bool indicating if this word is composed only by spaces.
        /// Spaces include tabs and line breaks
        /// </summary>
        public bool IsSpaces
        {
            get { return string.IsNullOrEmpty(Text.Trim()); }
        }

        /// <summary>
        /// Gets if the word is composed by only a line break
        /// </summary>
        public bool IsLineBreak
        {
            get { return Text == "\n"; }
        }

        /// <summary>
        /// Gets if the word is composed by only a tab
        /// </summary>
        public bool IsTab
        {
            get { return Text == "\t"; }
        }

        /// <summary>
        /// Gets the Box where this word belongs.
        /// </summary>
        public HtmlBox OwnerBox
        {
            get { return ownerBox; }
        }

        /// <summary>
        /// Gets the text of the word
        /// </summary>
        public string Text
        {
            get { return word; }
        }

        internal void ReplaceLineBreaksAndTabs()
        {
            word = word.Replace('\n', ' ');
            word = word.Replace('\t', ' ');
        }

        /// <summary>
        /// Appends the specified char to the word's text
        /// </summary>
        /// <param name="c"></param>
        internal void AppendChar(char c)
        {
            word += c;
        }

        /// <summary>
        /// Gets or sets an offset to be considered in measurements
        /// </summary>
        internal PointF LastMeasureOffset
        {
            get { return lastMeasureOffset; }
            set { lastMeasureOffset = value; }
        }

        /// <summary>
        /// Gets or sets the left of the rectangle.
        /// </summary>
        /// <value>The left.</value>
        public float Left
        {
            get { return left; }
            set { left = value; }
        }

        /// <summary>
        /// Top of the rectangle
        /// </summary>
        public float Top
        {
            get { return top; }
            set { top = value; }
        }

        /// <summary>
        /// Width of the rectangle
        /// </summary>
        public float Width
        {
            get { return width; }
            set { width = value; }
        }

        /// <summary>
        /// Height of the rectangle
        /// </summary>
        public float Height
        {
            get { return height; }
            set { height = value; }
        }

        /// <summary>
        /// Gets or sets the right of the rectangle.
        /// </summary>
        public float Right
        {
            get { return Bounds.Right; }
            set { Width = value - Left; }
        }

        /// <summary>
        /// Gets or sets the bottom of the rectangle.
        /// </summary>
        public float Bottom
        {
            get { return Bounds.Bottom; }
            set { Height = value - Top; }
        }

        /// <summary>
        /// Gets or sets the bounds of the rectangle
        /// </summary>
        public RectangleF Bounds
        {
            get { return new RectangleF(Left, Top, Width, Height); }
            set { Left = value.Left; Top = value.Top; Width = value.Width; Height = value.Height; }
        }

        /// <summary>
        /// Gets or sets the location of the rectangle
        /// </summary>
        public PointF Location
        {
            get { return new PointF(Left, Top); }
            set { Left = value.X; Top = value.Y; }
        }

        /// <summary>
        /// Gets or sets the size of the rectangle
        /// </summary>
        public SizeF Size
        {
            get { return new SizeF(Width, Height); }
            set { Width = value.Width; Height = value.Height; }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Represents this word for debugging purposes
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {

            return string.Format("{0} ({1} char{2})", Text.Replace(' ', '-').Replace("\n", "\\n"), Text.Length, Text.Length != 1 ? "s" : string.Empty);
        }

        
        #endregion
    }

    internal class HtmlLineBox
    {

        #region Fields

        private List<HtmlWordBox> words;
        private HtmlBox ownerBox;
        private Dictionary<HtmlBox, RectangleF> rects;
        private List<HtmlBox> relatedBoxes;

        #endregion

        #region Ctors

        /// <summary>
        /// Creates a new LineBox
        /// </summary>
        public HtmlLineBox(HtmlBox ownerBox)
        {
            rects = new Dictionary<HtmlBox, RectangleF>();
            relatedBoxes = new List<HtmlBox>();
            words = new List<HtmlWordBox>();
            this.ownerBox = ownerBox;
            this.ownerBox.LineBoxes.Add(this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the related boxes.
        /// </summary>
        /// <value>The related boxes.</value>
        public List<HtmlBox> RelatedBoxes
        {
            get { return relatedBoxes; }
        }

        /// <summary>
        /// Gets the words.
        /// </summary>
        /// <value>The words.</value>
        internal List<HtmlWordBox> Words
        {
            get { return words; }
        }

        /// <summary>
        /// Gets the owner box.
        /// </summary>
        /// <value>The owner box.</value>
        public HtmlBox OwnerBox
        {
            get { return ownerBox; }
        }

        /// <summary>
        /// Gets the rectangles.
        /// </summary>
        /// <value>The rectangles.</value>
        public Dictionary<HtmlBox, RectangleF> Rectangles
        {
            get { return rects; }
        }


        #endregion

        #region Methods

        /// <summary>
        /// Gets the maximum word bottom.
        /// </summary>
        /// <returns></returns>
        public float GetMaxWordBottom()
        {
            float res = float.MinValue;

            foreach (HtmlWordBox word in Words)
            {
                res = Math.Max(res, word.Bottom);
            }

            return res;
        }

        /// <summary>
        /// Add the word box to their lists if necessary.
        /// </summary>
        internal void AddWordBox(HtmlWordBox word)
        {
            if (!Words.Contains(word))
            {
                Words.Add(word);
            }

            if (!RelatedBoxes.Contains(word.OwnerBox))
            {
                RelatedBoxes.Add(word.OwnerBox);
            }
        }

        /// <summary>
        /// Return the words of the specified box
        /// </summary>
        internal List<HtmlWordBox> GetWordBoxes(HtmlBox box)
        {
            List<HtmlWordBox> wordBoxes = new List<HtmlWordBox>();

            foreach (HtmlWordBox word in Words)
                if (word.OwnerBox.Equals(box)) wordBoxes.Add(word);

            return wordBoxes;
        }

        /// <summary>
        /// Updates the specified rectangle of the specified box.
        /// </summary>
        internal void ChangeRectangle(HtmlBox box, float x, float y, float r, float b)
        {
            float leftspacing = box.ParseFloat(box.Border_Left_Width, "BORDER") + box.ParseFloat(box.Padding_Left, "FLOAT");
            float rightspacing = box.ParseFloat(box.Border_Right_Width, "BORDER") + box.ParseFloat(box.Padding_Right, "FLOAT");
            float topspacing = box.ParseFloat(box.Border_Top_Width, "BORDER") + box.ParseFloat(box.Padding_Top, "FLOAT");
            float bottomspacing = box.ParseFloat(box.Bottom_Border_Width, "BORDER") + box.ParseFloat(box.Padding_Bottom, "FLOAT");

            if ((box.FirstHostingLineBox != null && box.FirstHostingLineBox.Equals(this)) || box.IsImage) x -= leftspacing;
            if ((box.LastHostingLineBox != null && box.LastHostingLineBox.Equals(this)) || box.IsImage) r += rightspacing;

            if (!box.IsImage)
            {
                y -= topspacing;
                b += bottomspacing;
            }


            if (!Rectangles.ContainsKey(box))
            {
                Rectangles.Add(box, RectangleF.FromLTRB(x, y, r, b));
            }
            else
            {
                RectangleF f = Rectangles[box];
                Rectangles[box] = RectangleF.FromLTRB(
                    Math.Min(f.X, x), Math.Min(f.Y, y),
                    Math.Max(f.Right, r), Math.Max(f.Bottom, b));
            }

            if (box.ParentBox != null && box.ParentBox.Display == Constants.Inline)
            {
                ChangeRectangle(box.ParentBox, x, y, r, b);
            }
        }

        /// <summary>
        /// Assign the rectangles to their specified box
        /// </summary>
        internal void AssignRectanglesToBoxes()
        {
            foreach (HtmlBox htmlBox in Rectangles.Keys)
            {
                htmlBox.Rectangles.Add(this, Rectangles[htmlBox]);
            }
        }

        /// <summary>
        /// Gets the baseline Height of the rectangle
        /// </summary>
        public float GetBaseLineHeight(HtmlBox htmlBox, Graphics g)
        {
            Font f = htmlBox.GetFont();
            FontFamily ff = f.FontFamily;
            FontStyle s = f.Style;
            return f.GetHeight(g) * ff.GetCellAscent(s) / ff.GetLineSpacing(s);
        }

        /// <summary>
        /// Sets the baseline of the words of the specified box to certain height
        /// </summary>
        internal void SetBaseLine(Graphics g, HtmlBox box, float baseline)
        {
            List<HtmlWordBox> wordBox = GetWordBoxes(box);

            if (!Rectangles.ContainsKey(box)) return;

            RectangleF rectangle = Rectangles[box];

            float gap = 0f;

            if (wordBox.Count > 0)
            {
                gap = wordBox[0].Top - rectangle.Top;
            }
            else
            {
                HtmlWordBox firstWord = box.FirstWordMatch(box, this);

                if (firstWord != null)
                {
                    gap = firstWord.Top - rectangle.Top;
                }
            }

            float newtop = baseline - GetBaseLineHeight(box, g); 

            if (box.ParentBox != null &&
                box.ParentBox.Rectangles.ContainsKey(this) &&
                rectangle.Height < box.ParentBox.Rectangles[this].Height)
            {
                float recttop = newtop - gap;
                RectangleF newRectangle = new RectangleF(rectangle.X, recttop, rectangle.Width, rectangle.Height);
                Rectangles[box] = newRectangle;
                box.OffsetRectangle(this, gap);
            }
            foreach (HtmlWordBox word in wordBox)
                if (!word.IsImage)
                    word.Top = newtop;
        }

        /// <summary>
        /// Returns the words of the linebox
        /// </summary>
        public override string ToString()
        {
            string[] strWords = new string[Words.Count];
            for (int i = 0; i < strWords.Length; i++)
            {
                strWords[i] = Words[i].Text;
            }
            return string.Join(" ", strWords);
        }

        #endregion
    }

    internal class HtmlTable
    {
        #region Inner Class

        /// <summary>
        /// Used to make space on vertical cell combination
        /// </summary>
        internal class HtmlSpanBox
            : HtmlBox
        {
            internal readonly HtmlBox ExtendedBox;

            internal HtmlSpanBox(HtmlBox tableBox, ref HtmlBox extendedBox, int startRow)
                : base(tableBox, new HtmlTag("<none colspan=" + extendedBox.GetAttribute("colspan", "1") + ">"))
            {
                ExtendedBox = extendedBox;
                Display = Constants.None;

                this.startRow = startRow;
                this.endRow = startRow + int.Parse(extendedBox.GetAttribute("rowspan", "1")) - 1;
            }

            #region Properties

            private int startRow;

            private int endRow;

            /// <summary>
            /// Gets the index of the row where box ends
            /// </summary>
            public int EndRowIndex
            {
                get { return endRow; }
            }


            #endregion
        }

        #endregion

        #region Fields

        private HtmlBox tableBox;
        private int rowCount;
        private int columnCount;
        private List<HtmlBox> bodyrows;
        private HtmlBox caption;
        private List<HtmlBox> columns;
        private HtmlBox headerBox;
        private HtmlBox footerBox;
        private List<HtmlBox> allRows;
        private float[] columnWidths;
        private bool isWidthCalculated;
        private float[] columnMinWidths;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlTable"/> class.
        /// </summary>
        private HtmlTable()
        {
            bodyrows = new List<HtmlBox>();
            columns = new List<HtmlBox>();
            allRows = new List<HtmlBox>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlTable"/> class.
        /// </summary>
        /// <param name="tableBox">The table box.</param>
        /// <param name="g">The g.</param>
        public HtmlTable(HtmlBox tableBox, Graphics g)
            : this()
        {
            if (!(tableBox.Display == Constants.Table || tableBox.Display == Constants.InlineTable))
                throw new ArgumentException("Box is not a table", "tableBox");

            this.tableBox = tableBox;

            CalculateWords(tableBox, g);

            ConstructHtmlTable(g);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets if the user specified a width for the table
        /// </summary>
        public bool IsWidthCalculated
        {
            get { return isWidthCalculated; }
        }

        /// <summary>
        /// Hosts a list of all rows in the table, including those on the TFOOT, THEAD and TBODY
        /// </summary>
        public List<HtmlBox> GetAllRows
        {
            get { return allRows; }
        }

        /// <summary>
        /// Gets the column count of this table
        /// </summary>
        public int GetColCount
        {
            get { return columnCount; }
        }

        /// <summary>
        /// Gets the minimum width of each column
        /// </summary>
        public float[] ColMinWidths
        {
            get
            {
                if (columnMinWidths == null)
                {
                    columnMinWidths = new float[ColWidths.Length];

                    foreach (HtmlBox row in GetAllRows)
                    {
                        foreach (HtmlBox cell in row.Boxes)
                        {
                            int colspan = GetColumnSpan(cell);
                            int col = GetCellIndex(row, cell);
                            int affectcol = col + colspan - 1;
                            float spannedwidth = GetSpanMinWidth(row, cell, col, colspan) + (colspan - 1) * HorizontalSpacing;

                            columnMinWidths[affectcol] = Math.Max(columnMinWidths[affectcol], cell.GetMinWidth() - spannedwidth);

                        }
                    }

                }

                return columnMinWidths;
            }
        }

        /// <summary>
        /// Gets the declared Columns on the TABLE tag
        /// </summary>
        public List<HtmlBox> Columns
        {
            get { return columns; }
        }

        /// <summary>
        /// Gets an array of column widths.
        /// </summary>
        public float[] ColWidths
        {
            get { return columnWidths; }
        }

        /// <summary>
        /// Gets the boxes that represents the table-row Boxes of the table, 
        /// </summary>
        public List<HtmlBox> BodyRows
        {
            get { return bodyrows; }
        }

        /// <summary>
        /// Gets the table-footer-group Box
        /// </summary>
        public HtmlBox FooterBox
        {
            get { return footerBox; }
        }

        /// <summary>
        /// Gets the table-header-group Box
        /// </summary>
        public HtmlBox HeaderBox
        {
            get { return headerBox; }
        }

        /// <summary>
        /// Gets the actual horizontal spacing of the table
        /// </summary>
        public float HorizontalSpacing
        {
            get
            {
                if (TableBox.Border_Collapse == Constants.Collapse)
                {
                    return -1f;
                }

                return TableBox.ParseFloat(TableBox.Border_Spacing, "HORIZONTAL");
            }
        }

        /// <summary>
        /// Gets the actual vertical spacing of the table
        /// </summary>
        public float VerticalSpacing
        {
            get
            {
                if (TableBox.Border_Collapse == Constants.Collapse)
                {
                    return -1f;
                }

                return TableBox.ParseFloat(TableBox.Border_Spacing, "VERTICAL");
            }
        }

        /// <summary>
        /// Gets the original table box
        /// </summary>
        public HtmlBox TableBox
        {
            get { return tableBox; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Analyzes the Table and assigns values to this HtmlTable object.
        /// </summary>
        private void ConstructHtmlTable(Graphics g)
        {
            float availSpace = GetAvailableWidth();
            float availCellSpace = float.NaN; 

            foreach (HtmlBox box in TableBox.Boxes)
            {
                box.RemoveAnonymousSpaces();
                switch (box.Display)
                {
                    case Constants.TableCaption:
                        caption = box;
                        break;
                    case Constants.TableColumn:
                        for (int i = 0; i < GetSpan(box); i++)
                        {
                            Columns.Add(box);
                        }
                        break;
                    case Constants.TableColumnGroup:
                        if (box.Boxes.Count == 0)
                        {
                            int gspan = GetSpan(box);
                            for (int i = 0; i < gspan; i++)
                            {
                                Columns.Add(box);
                            }
                        }
                        else
                        {
                            foreach (HtmlBox htBox in box.Boxes)
                            {
                                int bbspan = GetSpan(htBox);
                                for (int i = 0; i < bbspan; i++)
                                {
                                    Columns.Add(htBox);
                                }
                            }
                        }
                        break;
                    case Constants.TableFooterGroup:
                        if (FooterBox != null)
                            BodyRows.Add(box);
                        else
                            footerBox = box;
                        break;
                    case Constants.TableHeaderGroup:
                        if (HeaderBox != null)
                            BodyRows.Add(box);
                        else
                            headerBox = box;
                        break;
                    case Constants.TableRow:
                        BodyRows.Add(box);
                        break;
                    case Constants.TableRowGroup:
                        foreach (HtmlBox bb in box.Boxes)
                            if (box.Display == Constants.TableRow)
                                BodyRows.Add(box);
                        break;
                    default:
                        break;
                }
            }

            if (HeaderBox != null) allRows.AddRange(HeaderBox.Boxes);
            allRows.AddRange(BodyRows);
            if (FooterBox != null) allRows.AddRange(FooterBox.Boxes);

            if (!TableBox.TableFixed)
            {
                int currow = 0;
                int curcol = 0;
                List<HtmlBox> rows = BodyRows;

                foreach (HtmlBox row in rows)
                {
                    row.RemoveAnonymousSpaces();
                    curcol = 0;
                    for (int k = 0; k < row.Boxes.Count; k++)
                    {

                        HtmlBox cell = row.Boxes[k];
                        int rowspan = GetRowSpan(cell);
                        int realcol = GetCellIndex(row, cell); 

                        for (int i = currow + 1; i < currow + rowspan; i++)
                        {
                            int colcount = 0;
                            for (int j = 0; j <= rows[i].Boxes.Count; j++)
                            {
                                if (colcount == realcol)
                                {
                                    rows[i].Boxes.Insert(colcount, new HtmlSpanBox(TableBox, ref cell, currow));
                                    break;
                                }
                                colcount++;
                                realcol -= GetColumnSpan(rows[i].Boxes[j]) - 1;
                            }

                        } 
                        curcol++;
                    } 
                    currow++;
                }

                TableBox.TableFixed = true;

            } 

            rowCount = BodyRows.Count +
                (HeaderBox != null ? HeaderBox.Boxes.Count : 0) +
                (FooterBox != null ? FooterBox.Boxes.Count : 0);

            if (Columns.Count > 0)
                columnCount = Columns.Count;
            else
                foreach (HtmlBox box in GetAllRows)
                    columnCount = Math.Max(columnCount, box.Boxes.Count);

            columnWidths = new float[columnCount];

            for (int i = 0; i < columnWidths.Length; i++)
                columnWidths[i] = float.NaN;

            availCellSpace = GetAvailableCellWidth();

            if (Columns.Count > 0)
            {

                for (int i = 0; i < Columns.Count; i++)
                {
                    if (Do.GetHtmlLengthNumber(Columns[i].Width) > 0) 
                    {
                        if (Columns[i].Width.EndsWith("%"))
                        {
                            ColWidths[i] = Do.ParseLength(Columns[i].Width, availCellSpace);
                        }
                        else if (Do.GetUnit(Columns[i].Width) == Do.Unit.Pixels || Do.GetUnit(Columns[i].Width) == Do.Unit.None)
                        {
                            ColWidths[i] = Do.GetHtmlLengthNumber(Columns[i].Width); 
                        }
                    }
                }

            }
            else
            {
                foreach (HtmlBox row in GetAllRows)
                {
                    for (int i = 0; i < columnCount; i++)
                    {
                        if (float.IsNaN(ColWidths[i]) &&                 
                            i < row.Boxes.Count &&                         
                            row.Boxes[i].Display == Constants.TableCell)
                        {

                            if (Do.GetHtmlLengthNumber(row.Boxes[i].Width) > 0) 
                            {
                                int colspan = GetColumnSpan(row.Boxes[i]);
                                float flen = 0f;
                                if (row.Boxes[i].Width.EndsWith("%"))
                                {
                                    flen = Do.ParseLength(row.Boxes[i].Width, availCellSpace);
                                }
                                else if (Do.GetUnit(row.Boxes[i].Width) == Do.Unit.Pixels || Do.GetUnit(row.Boxes[i].Width) == Do.Unit.None)
                                {
                                    flen = Do.GetHtmlLengthNumber(row.Boxes[i].Width); 
                                }
                                flen /= Convert.ToSingle(colspan);

                                for (int j = i; j < i + colspan; j++)
                                {
                                    ColWidths[j] = flen;
                                }
                            }
                        }
                    }
                }
            }

            if (IsWidthCalculated) 
            {
                int numberOfNans = 0;
                float occupedSpace = 0f;

                for (int i = 0; i < ColWidths.Length; i++)
                    if (float.IsNaN(ColWidths[i]))
                        numberOfNans++;
                    else
                        occupedSpace += ColWidths[i];

                float nanWidth = (availCellSpace - occupedSpace) / Convert.ToSingle(numberOfNans);

                for (int i = 0; i < ColWidths.Length; i++)
                    if (float.IsNaN(ColWidths[i]))
                        ColWidths[i] = nanWidth;
            }
            else
            {
                float[] _maxFullWidths = new float[ColWidths.Length];

                foreach (HtmlBox row in GetAllRows)
                {
                    for (int i = 0; i < row.Boxes.Count; i++)
                    {
                        int col = GetCellIndex(row, row.Boxes[i]);

                        if (float.IsNaN(ColWidths[col]) &&
                            i < row.Boxes.Count &&
                            GetColumnSpan(row.Boxes[i]) == 1)
                        {
                            _maxFullWidths[col] = Math.Max(_maxFullWidths[col], row.Boxes[i].GetWidth(g));
                        }
                    }
                }

                for (int i = 0; i < ColWidths.Length; i++)
                    if (float.IsNaN(ColWidths[i]))
                        ColWidths[i] = _maxFullWidths[i];
            }

            int curCol = 0;
            float reduceAmount = 1f;

            while (GetWidthSum() > GetAvailableWidth() && IsWidthReducable())
            {
                while (!IsWidthReducable(curCol)) curCol++;

                ColWidths[curCol] -= reduceAmount;

                curCol++;

                if (curCol >= ColWidths.Length) curCol = 0;
            }

            foreach (HtmlBox row in GetAllRows)
            {
                foreach (HtmlBox cell in row.Boxes)
                {
                    int colspan = GetColumnSpan(cell);
                    int col = GetCellIndex(row, cell);
                    int affectcol = col + colspan - 1;

                    if (ColWidths[col] < ColMinWidths[col])
                    {
                        float diff = ColMinWidths[col] - ColWidths[col];
                        ColWidths[affectcol] = ColMinWidths[affectcol];

                        if (col < ColWidths.Length - 1)
                        {
                            ColWidths[col + 1] -= diff;
                        }
                    }
                }
            }

            TableBox.Padding = "0";

            float startx = TableBox.ClientLeft + HorizontalSpacing;
            float starty = TableBox.ClientTop + VerticalSpacing;
            float currentX = startx;
            float currentY = starty;
            float maxRight = startx;
            float maxBottom = 0f;
            int currentrow = 0;

            foreach (HtmlBox row in GetAllRows)
            {
                if (row.IsAnonymousSpaceBlock || row.IsAnonymousSpace) continue;

                currentX = startx;
                curCol = 0;

                foreach (HtmlBox cell in row.Boxes)
                {
                    if (curCol >= ColWidths.Length) break;

                    int rowspan = GetRowSpan(cell);
                    float width = GetCellWidth(GetCellIndex(row, cell), cell);

                    cell.Location = new PointF(currentX, currentY);
                    cell.Size = new SizeF(width, 0f);
                    cell.CalcBoxBounds(g); 
                    HtmlSpanBox sb = cell as HtmlSpanBox;
                    if (sb != null)
                    {
                        if (sb.EndRowIndex == currentrow)
                        {
                            maxBottom = Math.Max(maxBottom, sb.ExtendedBox.ActualBottom);
                        }
                    }
                    else if (rowspan == 1)
                    {
                        maxBottom = Math.Max(maxBottom, cell.ActualBottom);
                    }
                    maxRight = Math.Max(maxRight, cell.ActualRight);
                    curCol++;
                    currentX = cell.ActualRight + HorizontalSpacing;
                }

                foreach (HtmlBox cell in row.Boxes)
                {
                    HtmlSpanBox spacer = cell as HtmlSpanBox;

                    if (spacer == null && GetRowSpan(cell) == 1)
                    {
                        cell.ActualBottom = maxBottom;
                        Do.CellVerticalAlign(g, cell);
                    }
                    else if (spacer != null && spacer.EndRowIndex == currentrow)
                    {
                        spacer.ExtendedBox.ActualBottom = maxBottom;
                        Do.CellVerticalAlign(g, spacer.ExtendedBox);
                    }
                }

                currentY = maxBottom + VerticalSpacing;
                currentrow++;
            }

            TableBox.ActualRight = maxRight + HorizontalSpacing + TableBox.ParseFloat(TableBox.Border_Right_Width, "BORDER");
            TableBox.ActualBottom = maxBottom + VerticalSpacing + TableBox.ParseFloat(TableBox.Bottom_Border_Width, "BORDER");
        }

        /// <summary>
        /// Gets the spanned width of a cell
        /// </summary>
        private float GetSpanMinWidth(HtmlBox row, HtmlBox cell, int realcolindex, int colspan)
        {
            float width = 0f;

            for (int i = realcolindex; i < row.Boxes.Count || i < realcolindex + colspan - 1; i++)
            {
                width += ColMinWidths[i];
            }

            return width;
        }

        /// <summary>
        /// Gets the cell column index checking its position and other cells colspans
        /// </summary>
        private int GetCellIndex(HtmlBox row, HtmlBox cell)
        {
            int i = 0;

            foreach (HtmlBox box in row.Boxes)
            {
                if (box.Equals(cell)) break;
                i += GetColumnSpan(box);
            }

            return i;
        }

        /// <summary>
        /// Gets the cells width, taking colspan and being in the specified column
        /// </summary>
        private float GetCellWidth(int column, HtmlBox box)
        {
            float colspan = Convert.ToSingle(GetColumnSpan(box));
            float sum = 0f;

            for (int i = column; i < column + colspan; i++)
            {
                if (column >= ColWidths.Length) break;
                if (ColWidths.Length <= i) break;
                sum += ColWidths[i];
            }

            sum += (colspan - 1) * HorizontalSpacing;

            return sum; ;
        }

        /// <summary>
        /// Gets the colspan of the specified box
        /// </summary>
        private int GetColumnSpan(HtmlBox box)
        {
            string att = box.GetAttribute("colspan", "1");
            int colspan;

            if (!int.TryParse(att, out colspan))
            {
                return 1;
            }

            return colspan;
        }

        /// <summary>
        /// Gets the rowspan of the specified box
        /// </summary>
        private int GetRowSpan(HtmlBox box)
        {
            string attribute = box.GetAttribute("rowspan", "1");
            int rowspan;

            if (!int.TryParse(attribute, out rowspan))
            {
                return 1;
            }

            return rowspan;
        }

        /// <summary>
        /// Recursively calculate the specified box
        /// </summary>
        private void Calculate(HtmlBox box, Graphics g)
        {
            if (box == null) return;

            foreach (HtmlBox htmlBox in box.Boxes)
            {
                htmlBox.CalcBoxBounds(g);
                Calculate(htmlBox, g);
            }
        }

        /// <summary>
        /// Recursively calculates words inside the box
        /// </summary>
        private void CalculateWords(HtmlBox box, Graphics g)
        {
            if (box == null) return;

            foreach (HtmlBox htmlBox in box.Boxes)
            {
                htmlBox.CalculateWordsSize(g);
                CalculateWords(htmlBox, g);
            }
        }

        /// <summary>
        /// To indicate is it possible to reduce the width
        /// </summary>
        private bool IsWidthReducable()
        {
            for (int i = 0; i < ColWidths.Length; i++)
            {
                if (IsWidthReducable(i))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// To indicate is it possible to reduce the width
        /// </summary>
        private bool IsWidthReducable(int columnIndex)
        {
            if (ColWidths.Length >= columnIndex || ColMinWidths.Length >= columnIndex) return false;
            return ColWidths[columnIndex] > ColMinWidths[columnIndex];
        }

        /// <summary>
        /// Gets the available width.
        /// </summary>
        /// <returns></returns>
        private float GetAvailableWidth()
        {
            if (Do.GetHtmlLengthNumber(TableBox.Width) > 0)
            {
                isWidthCalculated = true;

                if (TableBox.Width.EndsWith("%"))
                {
                    return Do.ParseLength(TableBox.Width, TableBox.ParentBox.AvailableWidth);
                }
                else
                {
                    return Do.GetHtmlLengthNumber(TableBox.Width);
                }
            }
            else
            {
                return TableBox.ParentBox.AvailableWidth;
            }
        }

        private float GetAvailableCellWidth()
        {
            return GetAvailableWidth() -
                HorizontalSpacing * (GetColCount + 1) -
                TableBox.ParseFloat(TableBox.Border_Left_Width, "BORDER") - TableBox.ParseFloat(TableBox.Border_Right_Width, "BORDER");
        }

        /// <summary>
        /// Gets the current sum of column widths
        /// </summary>
        private float GetWidthSum()
        {
            float f = 0f;

            for (int i = 0; i < ColWidths.Length; i++)
                if (float.IsNaN(ColWidths[i]))
                    throw new Exception("HtmlTable error: There's a NaN in column widths");
                else
                    f += ColWidths[i];

            f += HorizontalSpacing * (ColWidths.Length + 1);

            f += TableBox.ParseFloat(TableBox.Border_Left_Width, "BORDER") + TableBox.ParseFloat(TableBox.Border_Right_Width, "BORDER");

            return f;
        }

        private int GetSpan(HtmlBox b)
        {
            float f = Do.ParseLength(b.GetAttribute("span"), 1);

            return Math.Max(1, Convert.ToInt32(f));
        }

        #endregion
    }
}
