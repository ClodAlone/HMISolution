#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

using Syncfusion.Drawing;
using Syncfusion.Styles;
#endregion
namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    [TypeConverter(typeof(TreeColumnAdvStyleInfoConverter))]
    public class TreeColumnAdvStyleInfo : StyleInfoBase
    {
        #region Class static members
       
       private static readonly TreeColumnAdvStyleInfo s_empty = new TreeColumnAdvStyleInfo();
       private static TreeColumnAdvStyleInfo s_default;

        #endregion

        #region Class static properties
      
        public static TreeColumnAdvStyleInfo Empty
        {
            get
            {
                return s_empty;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the TreeColumnAdvStyleInfo class.
        /// </summary>
        [DebuggerStepThrough()]
        public TreeColumnAdvStyleInfo()
            : base(new TreeColumnAdvStyleInfoStore())
        {
        }

        /// <summary>
        /// Initializes a new instance of the TreeColumnAdvStyleInfo class.
        /// </summary>
        /// <param name="style">The style object that contains the original data.</param>
        [DebuggerStepThrough()]
        public TreeColumnAdvStyleInfo(TreeColumnAdvStyleInfo style)
            : base(style.Store)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeColumnAdvStyleInfo"/> class.
        /// </summary>
        /// <param name="store">A <see cref="TreeColumnAdvStyleInfoStore"/> that holds data for this <see cref="TreeColumnAdvStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="TreeColumnAdvStyleInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public TreeColumnAdvStyleInfo(TreeColumnAdvStyleInfoStore store)
            : base(store)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeColumnAdvStyleInfo"/> class.
        /// </summary>
        /// <param name="identity">A <see cref="TreeColumnAdvStyleInfoIdentity"/> that holds the indentity for this <see cref="TreeColumnAdvStyleInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public TreeColumnAdvStyleInfo(StyleInfoIdentityBase identity)
            : base(identity, new TreeColumnAdvStyleInfoStore())
        {
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Override this method to return a default style object for your derived class.
        /// </summary>
        /// <returns>A default style object.</returns>
        /// <remarks>
        /// You should cache the default style object in a static field.
        /// </remarks>
        protected override StyleInfoBase GetDefaultStyle()
        {
            if (s_default == null)
            {
                s_default = new TreeColumnAdvStyleInfo();

                s_default.Visible = true;
                s_default.Width = 60;
                s_default.Font = FontUtil.CreateFont("Verdana", 8);
                s_default.Text = string.Empty;
                s_default.TextColor = SystemColors.WindowText;
                s_default.Background = new BrushInfo(Color.Transparent);
                s_default.AreaBackground = new BrushInfo(Color.Transparent);
                s_default.HelpText = string.Empty;
                s_default.SortOrder = SortOrder.None;
                s_default.Comparer = null;
                s_default.Tag = null;
                s_default.LeftImage = null;
                s_default.LeftImageIndices = new int[0];
                s_default.LeftImagePadding = 0;
                s_default.RightImage = null;
                s_default.RightImageIndices = new int[0];
                s_default.RightImagePadding = 0;
                s_default.Border3DStyle = Border3DStyle.Etched;
                s_default.BorderColor = SystemColors.ControlDark;
                s_default.HighlightBorderColor = SystemColors.Highlight;
                s_default.BorderSides = Border3DSide.All;
                s_default.BorderSingle = ButtonBorderStyle.None;
                s_default.BorderStyle = BorderStyle.Fixed3D;
                s_default.BaseStyle = string.Empty;
                s_default.HorizontalAlignment = StringAlignment.Center;
                s_default.VerticalAlignment = StringAlignment.Center;
                s_default.AllowTextOverlap = false;
            }

            return s_default;
        }
        #endregion

        #region Class properties

        #region Visible
        /// <summary>
        /// Gets or sets a value indicating whether column is visible to user or not.
        /// </summary>
        [
        Description("Gets or sets a value indicating whether column visible to user or not."),
        Browsable(false),
        Category("Behavior")
        ]
        public bool Visible
        {
            get
            {
                return (bool)GetValue(TreeColumnAdvStyleInfoStore.VisibleProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.VisibleProperty, value);
            }
        }

        /// <summary>
        /// Reset property Visible value to default value
        /// </summary>
        public void ResetVisible()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.VisibleProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize Visible property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeVisible()
        {
            return HasVisible;
        }

        /// <summary>
        /// Gets a value indicating whether Visible property value.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasVisible
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.VisibleProperty);
            }
        }
        #endregion

        #region Width
        [
        Description("Gets or sets the width of the column "),
        Category("Appearance")
        ]
        public int Width
        {
            get
            {
                return (int)GetValue(TreeColumnAdvStyleInfoStore.WidthProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.WidthProperty, value);
            }
        }

        /// <summary>
        /// Reset property Width value to default value
        /// </summary>
        public void ResetWidth()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.WidthProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize Width property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeWidth()
        {
            return HasWidth;
        }

        /// <summary>
        /// Gets a value indicating whether Width property value.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasWidth
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.WidthProperty);
            }
        }
        #endregion

        #region Font
        [
        Description("Gets / sets the font of the column"),
        Category("Appearance")
        ]
        public Font Font
        {
            get
            {
                return (Font)GetValue(TreeColumnAdvStyleInfoStore.FontProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.FontProperty, value);
            }
        }

        /// <summary>
        /// Reset property Font value to default value
        /// </summary>
        public void ResetFont()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.FontProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize Font property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeFont()
        {
            return HasFont;
        }

        /// <summary>
        /// Gets a value indicating whether Font property value.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasFont
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.FontProperty);
            }
        }
        #endregion

        #region Text

        [
        Description("Gets / sets the text of the column"),
        Category("Appearance"),
        Browsable(false),
        ]
        public string Text
        {
            get
            {
                return (string)GetValue(TreeColumnAdvStyleInfoStore.TextProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.TextProperty, value);
            }
        }

        /// <summary>
        /// Reset property Text value to default value
        /// </summary>
        public void ResetText()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.TextProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize Text property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeText()
        {
            return HasText;
        }

        /// <summary>
        /// Gets a value indicating whether Text property value.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasText
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.TextProperty);
            }
        }
        #endregion

        #region TextColor
        [
        Description("Gets / sets the color of the column text"),
        Category("Appearance")
        ]
        public Color TextColor
        {
            get
            {
                return (Color)GetValue(TreeColumnAdvStyleInfoStore.TextColorProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.TextColorProperty, value);
            }
        }

        /// <summary>
        /// Reset property TextColor value to default value
        /// </summary>
        public void ResetTextColor()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.TextColorProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize TextColor property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeTextColor()
        {
            return HasTextColor;
        }

        /// <summary>
        /// Gets a value indicating whether TextColor property value.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasTextColor
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.TextColorProperty);
            }
        }
        #endregion

        #region Background
        /// <summary>
        /// Gets or sets the Background style of column.
        /// </summary>
        [
        Description("Gets / sets the Background style of column."),
        Category("Appearance")
        ]
        public BrushInfo Background
        {
            get
            {
                return (BrushInfo)GetValue(TreeColumnAdvStyleInfoStore.BackgroundProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.BackgroundProperty, value);
            }
        }

        /// <summary>
        /// Reset property Background value to default value
        /// </summary>
        public void ResetBackground()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.BackgroundProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize Background property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeBackground()
        {
            return HasBackground;
        }

        /// <summary>
        /// Gets a value indicating whether Background property value.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasBackground
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.BackgroundProperty);
            }
        }
        #endregion

        #region HelpText
        [
        Description("Gets / sets the help text of the column"),
        Category("Appearance"),
        Browsable(false),
        ]
        public string HelpText
        {
            get
            {
                return (string)GetValue(TreeColumnAdvStyleInfoStore.HelpTextProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.HelpTextProperty, value);
            }
        }

        /// <summary>
        /// Reset property HelpText value to default value
        /// </summary>
        public void ResetHelpText()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.HelpTextProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize HelpText property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeHelpText()
        {
            return HasHelpText;
        }

        /// <summary>
        /// Gets a value indicating whether HelpText property value.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasHelpText
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.HelpTextProperty);
            }
        }
        #endregion

        #region SortOrder
        [
        Description("Indicates the sort order of the column"),
        Browsable(false),
        Category("Behavior")
        ]
        public SortOrder SortOrder
        {
            get
            {
                return (SortOrder)GetValue(TreeColumnAdvStyleInfoStore.SortOrderProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.SortOrderProperty, value);
            }
        }

        /// <summary>
        /// Reset property SortOrder value to default value
        /// </summary>
        public void ResetSortOrder()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.SortOrderProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize SortOrder property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeSortOrder()
        {
            return HasSortOrder;
        }

        /// <summary>
        /// Gets a value indicating whether SortOrder property value.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasSortOrder
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.SortOrderProperty);
            }
        }
        #endregion

        #region Comparer
        [
        Description("Indicates the IComparer object that compares two columns."),
        Browsable(false),
        Category("Behavior")
        ]
        public IComparer Comparer
        {
            get
            {
                return (IComparer)GetValue(TreeColumnAdvStyleInfoStore.ComparerProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.ComparerProperty, value);
            }
        }

        /// <summary>
        /// Reset property Comparer value to default value
        /// </summary>
        public void ResetComparer()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.ComparerProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize Comparer property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeComparer()
        {
            return HasComparer;
        }

        /// <summary>
        /// Gets a value indicating whether Comparer property value.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasComparer
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.ComparerProperty);
            }
        }
        #endregion

        #region Tag
        [
        Description("The tag of the column. Can be used to store aditional information for the column."),
        Category("Data"),
        Browsable(false)
        ]
        public object Tag
        {
            get
            {
                return (object)GetValue(TreeColumnAdvStyleInfoStore.TagProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.TagProperty, value);
            }
        }

        /// <summary>
        /// Reset property Tag value to default value
        /// </summary>
        public void ResetTag()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.TagProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize Tag property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeTag()
        {
            return HasTag;
        }

        /// <summary>
        /// Gets a value indicating whether Tag property value.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasTag
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.TagProperty);
            }
        }
        #endregion

        #region LeftImage
        [
        Description("Gets / sets the image that will be drawn on the left of the column`s text."),
        Category("Images")
        ]
        public Image LeftImage
        {
            get
            {
                return (Image)GetValue(TreeColumnAdvStyleInfoStore.LeftImageProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.LeftImageProperty, value);
            }
        }

        /// <summary>
        /// Reset property LeftImage value to default value
        /// </summary>
        public void ResetLeftImage()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.LeftImageProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize LeftImage property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeLeftImage()
        {
            return HasLeftImage;
        }

        /// <summary>
        /// Gets a value indicating whether LeftImage property value.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasLeftImage
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.LeftImageProperty);
            }
        }
        #endregion

        #region LeftImageIndices
        [
        Description("Gets / sets the image index to be drawn on the left of the column`s text."),
        Category("Images")
        ]
        public int[] LeftImageIndices
        {
            get
            {
                return (int[])GetValue(TreeColumnAdvStyleInfoStore.LeftImageIndicesProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.LeftImageIndicesProperty, value);
            }
        }

        /// <summary>
        /// Reset property LeftImageIndices value to default value
        /// </summary>
        public void ResetLeftImageIndices()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.LeftImageIndicesProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize LeftImageIndices property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeLeftImageIndices()
        {
            return HasLeftImageIndices;
        }

        /// <summary>
        /// Gets a value indicating whether LeftImageIndices property value.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasLeftImageIndices
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.LeftImageIndicesProperty);
            }
        }
        #endregion

        #region LeftImagePadding
        [
        Description("Gets / sets the padding of left image for the column."),
        Category("Images")
        ]
        public int LeftImagePadding
        {
            get
            {
                return (int)GetValue(TreeColumnAdvStyleInfoStore.LeftImagePaddingProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.LeftImagePaddingProperty, value);
            }
        }

        /// <summary>
        /// Reset property LeftImagePadding value to default value
        /// </summary>
        public void ResetLeftImagePadding()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.LeftImagePaddingProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize LeftImagePadding property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeLeftImagePadding()
        {
            return HasLeftImagePadding;
        }

        /// <summary>
        /// Gets a value indicating whether LeftImagePadding property value.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasLeftImagePadding
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.LeftImagePaddingProperty);
            }
        }
        #endregion

        #region RightImage
        [
        Description("Gets / sets image that will be drawn on the right of the column`s text. "),
        Category("Images")
        ]
        public Image RightImage
        {
            get
            {
                return (Image)GetValue(TreeColumnAdvStyleInfoStore.RightImageProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.RightImageProperty, value);
            }
        }

        /// <summary>
        /// Reset property RightImage value to default value
        /// </summary>
        public void ResetRightImage()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.RightImageProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize RightImage property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeRightImage()
        {
            return HasRightImage;
        }

        /// <summary>
        /// Gets a value indicating whether RightImage property value.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasRightImage
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.RightImageProperty);
            }
        }
        #endregion

        #region RightImageIndices
        [
        Description("Gets / sets the image index to be drawn on the right of the column`s text."),
        Category("Images")
        ]
        public int[] RightImageIndices
        {
            get
            {
                return (int[])GetValue(TreeColumnAdvStyleInfoStore.RightImageIndicesProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.RightImageIndicesProperty, value);
            }
        }

        /// <summary>
        /// Reset property RightImageIndices value to default value
        /// </summary>
        public void ResetRightImageIndices()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.RightImageIndicesProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize RightImageIndices property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeRightImageIndices()
        {
            return HasRightImageIndices;
        }

        /// <summary>
        /// Gets a value indicating whether RightImageIndices property value.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasRightImageIndices
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.RightImageIndicesProperty);
            }
        }
        #endregion

        #region RightImagePadding
        [
        Description("Gets / sets the padding of tight image for the column."),
        Category("Images")
        ]
        public int RightImagePadding
        {
            get
            {
                return (int)GetValue(TreeColumnAdvStyleInfoStore.RightImagePaddingProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.RightImagePaddingProperty, value);
            }
        }

        /// <summary>
        /// Reset property RightImagePadding value to default value
        /// </summary>
        public void ResetRightImagePadding()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.RightImagePaddingProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize RightImagePadding property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeRightImagePadding()
        {
            return HasRightImagePadding;
        }

        /// <summary>
        /// Gets a value indicating whether RightImagePadding property value.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasRightImagePadding
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.RightImagePaddingProperty);
            }
        }
        #endregion

        #region BorderSides
        [
        Description("Gets / sets the sides of a rectangle to apply a three-dimensional border to."),
        Category("Borders")
        ]
        public Border3DSide BorderSides
        {
            get
            {
                return (Border3DSide)GetValue(TreeColumnAdvStyleInfoStore.BorderSidesProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.BorderSidesProperty, value);
            }
        }

        /// <summary>
        /// Reset property BorderSides value to default value
        /// </summary>
        public void ResetBorderSides()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.BorderSidesProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize BorderSides property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeBorderSides()
        {
            return HasBorderSides;
        }

        /// <summary>
        /// Gets a value indicating whether BorderSides property value.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasBorderSides
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.BorderSidesProperty);
            }
        }
        #endregion

        #region BorderStyle
        [
        Description("Gets / sets the border style for the column."),
        Category("Borders")
        ]
        public BorderStyle BorderStyle
        {
            get
            {
                return (BorderStyle)GetValue(TreeColumnAdvStyleInfoStore.BorderStyleProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.BorderStyleProperty, value);
            }
        }

        /// <summary>
        /// Reset property BorderStyle value to default value
        /// </summary>
        public void ResetBorderStyle()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.BorderStyleProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize BorderStyle property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeBorderStyle()
        {
            return HasBorderStyle;
        }

        /// <summary>
        /// Gets a value indicating whether BorderStyle property has set or not.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasBorderStyle
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.BorderStyleProperty);
            }
        }
        #endregion

        #region Border3DStyle
        [
        Description("Gets / sets the border 3dStyle for the column."),
        Category("Borders")
        ]
        public Border3DStyle Border3DStyle
        {
            get
            {
                return (Border3DStyle)GetValue(TreeColumnAdvStyleInfoStore.Border3DStyleProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.Border3DStyleProperty, value);
            }
        }

        /// <summary>
        /// Reset property Border3DStyle value to default value
        /// </summary>
        public void ResetBorder3DStyle()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.Border3DStyleProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize Border3DStyle property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeBorder3DStyle()
        {
            return HasBorder3DStyle;
        }

        /// <summary>
        /// Gets a value indicating whether Border3DStyle property has set or not.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasBorder3DStyle
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.Border3DStyleProperty);
            }
        }
        #endregion

        #region BorderColor
        [
        Description("Gets / sets the border color for the column."),
        Category("Borders")
        ]
        public Color BorderColor
        {
            get
            {
                return (Color)GetValue(TreeColumnAdvStyleInfoStore.BorderColorProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.BorderColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the highlight border.
        /// </summary>
        /// <value>The color of the highlight border.</value>
        [
        Description("Gets / sets the highlight border color for the column."),
        Category("Borders")
        ]
        public Color HighlightBorderColor
        {
            get
            {
                return (Color)GetValue(TreeColumnAdvStyleInfoStore.HighlightBorderColorProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.HighlightBorderColorProperty, value);
            }
        }

        /// <summary>
        /// Reset property BorderColor value to default value
        /// </summary>
        public void ResetBorderColor()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.BorderColorProperty);
        }

        /// <summary>
        /// Resets the color of the border.
        /// </summary>
        public void ResetHighlightBorderColor()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.HighlightBorderColorProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize  BorderColor property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeHighlightBorderColor()
        {
            return HasHighlightBorderColor;
        }

        /// <summary>
        /// Indicate should or not we serialize  BorderColor property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeBorderColor()
        {
            return HasBorderColor;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has highlight border color.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has highlight border color; otherwise, <c>false</c>.
        /// </value>
        [
        Browsable(false)
        ]
        public bool HasHighlightBorderColor
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.HighlightBorderColorProperty);
            }
        }
        /// <summary>
        /// Gets a value indicating whether BorderColor property has set or not.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasBorderColor
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.BorderColorProperty);
            }
        }
        #endregion

        #region BorderSingle
  
        [
        Description("Gets / sets the border style for the column."),
        Category("Borders")
        ]
        public ButtonBorderStyle BorderSingle
        {
            get
            {
                return (ButtonBorderStyle)GetValue(TreeColumnAdvStyleInfoStore.BorderSingleProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.BorderSingleProperty, value);
            }
        }

        /// <summary>
        /// Reset property BorderSingle value to default value
        /// </summary>
        public void ResetBorderSingle()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.BorderSingleProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize BorderSingle property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeBorderSingle()
        {
            return HasBorderSingle;
        }

        /// <summary>
        /// Gets a value indicating whether BorderSingle property has set or not.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasBorderSingle
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.BorderSingleProperty);
            }
        }
        #endregion

        #region AreaBackground
        /// <summary>
        /// Gets or sets the background for column area reserved by control.
        /// </summary>
        [
        Description("Gets / sets the background for column area reserved by control."),
        Category("Appearance")
        ]
        public BrushInfo AreaBackground
        {
            get
            {
                return (BrushInfo)GetValue(TreeColumnAdvStyleInfoStore.AreaBackgroundProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.AreaBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Reset property AreaBackground value to default value
        /// </summary>
        public void ResetAreaBackground()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.AreaBackgroundProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize AreaBackground property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeAreaBackground()
        {
            return HasAreaBackground;
        }

        /// <summary>
        /// Gets a value indicating whether AreaBackground property has set or not.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasAreaBackground
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.AreaBackgroundProperty);
            }
        }
        #endregion

        #region BaseStyle
    
        [
        Description("The base style for the subitem."),
        Browsable(false),
        Category("Appearance - Inherited"),
        Editor(typeof(BaseStyleSelectorUITypeEditor), typeof(UITypeEditor))
        ]
        public string BaseStyle
        {
            get
            {
                return (string)GetValue(TreeColumnAdvStyleInfoStore.BaseStyleProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.BaseStyleProperty, value);
            }
        }

        /// <summary>
        /// Reset property BaseStyle value to default value
        /// </summary>
        public void ResetBaseStyle()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.BaseStyleProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize BaseStyle property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeBaseStyle()
        {
            return HasBaseStyle;
        }

        /// <summary>
        /// Gets a value indicating whether  BaseStyle property has set or not.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasBaseStyle
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.BaseStyleProperty);
            }
        }
        #endregion

        #region VerticalAlignment
     
        [
        Description("Gets / sets the vertical alignment of text."),
        Category("Appearance"),
        Localizable(true),
        DefaultValue(StringAlignment.Center)
        ]
        public StringAlignment VerticalAlignment
        {
            get
            {
                return (StringAlignment)GetValue(TreeColumnAdvStyleInfoStore.VerticalAlignmentProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.VerticalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Reset property VerticalAlignment value to default value
        /// </summary>
        public void ResetVerticalAlignment()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.VerticalAlignmentProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize VerticalAlignment property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeVerticalAlignment()
        {
            return HasVerticalAlignment;
        }

        /// <summary>
        /// Gets a value indicating whether  VerticalAlignment property has set or not.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasVerticalAlignment
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.VerticalAlignmentProperty);
            }
        }
        #endregion

        #region HorizontalAlignment
     
        [
        Description("Gets / sets the horizontal text alignment of text."),
        Category("Appearance"),
        Localizable(true),
        DefaultValue(StringAlignment.Center)
        ]
        public StringAlignment HorizontalAlignment
        {
            get
            {
                return (StringAlignment)GetValue(TreeColumnAdvStyleInfoStore.HorizontalAlignmentProperty);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.HorizontalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Reset property HorizontalAlignment value to default value
        /// </summary>
        public void ResetHorizontalAlignment()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.HorizontalAlignmentProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize HorizontalAlignment property value.
        /// </summary>
        /// <returns>Returns a bool value</returns>
        internal bool ShouldSerializeHorizontalAlignment()
        {
            return HasHorizontalAlignment;
        }

        /// <summary>
        /// Gets a value indicating whether HorizontalAlignment property value has set or not.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasHorizontalAlignment
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.HorizontalAlignmentProperty);
            }
        }
        #endregion

        #region AllowTextOverlap
      
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
                return (bool)GetValue(TreeColumnAdvStyleInfoStore.AllowTextOverlap);
            }
            set
            {
                SetValue(TreeColumnAdvStyleInfoStore.AllowTextOverlap, value);
            }
        }

        /// <summary>
        /// Reset property AllowTextOverlap value to default value
        /// </summary>
        public void ResetAllowTextOverlap()
        {
            ResetValue(TreeColumnAdvStyleInfoStore.AllowTextOverlap);
        }

        /// <summary>
        /// Indicate should or not we serialize AllowTextOverlap property value.
        /// </summary>
        /// <returns>Returns a value indicating whether should serilize AllowTextOverlap property.</returns>
        internal bool ShouldSerializeAllowTextOverlap()
        {
            return HasAllowTextOverlap;
        }

        /// <summary>
        /// Gets a value indicating whether AllowTextOverlap.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasAllowTextOverlap
        {
            get
            {
                return HasValue(TreeColumnAdvStyleInfoStore.AllowTextOverlap);
            }
        }
        #endregion

        #endregion
    }
}