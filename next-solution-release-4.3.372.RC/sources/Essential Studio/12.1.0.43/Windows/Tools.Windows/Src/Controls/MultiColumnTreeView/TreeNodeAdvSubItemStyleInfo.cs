#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
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
    [TypeConverter(typeof(TreeNodeAdvSubItemStyleInfoConverter))]
    public class TreeNodeAdvSubItemStyleInfo :
      StyleInfoBase,
      ITreeNodeAdvSubItemStyle
    {
        #region Class static members

        private static readonly TreeNodeAdvSubItemStyleInfo s_empty = new TreeNodeAdvSubItemStyleInfo();

        private static TreeNodeAdvSubItemStyleInfo s_default;
        #endregion

        #region Class static properties
      
        public static TreeNodeAdvSubItemStyleInfo Empty
        {
            get
            {
                return s_empty;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the TreeNodeAdvSubItemStyleInfo class.
        /// </summary>
        [DebuggerStepThrough()]
        public TreeNodeAdvSubItemStyleInfo()
            : base(new TreeNodeAdvSubItemStyleInfoStore())
        {
        }

        /// <summary>
        /// Initializes a new instance of the TreeNodeAdvSubItemStyleInfo class.
        /// </summary>
        /// <param name="style">The style object that contains the original data.</param>
        [DebuggerStepThrough()]
        public TreeNodeAdvSubItemStyleInfo(TreeNodeAdvSubItemStyleInfo style)
            : base(style.Store)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TreeNodeAdvSubItemStyleInfo class.
        /// </summary>
        /// <param name="store">A <see cref="TreeNodeAdvSubItemStyleInfoStore"/> that holds data for this <see cref="TreeColumnAdvStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="TreeNodeAdvSubItemStyleInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public TreeNodeAdvSubItemStyleInfo(TreeNodeAdvSubItemStyleInfoStore store)
            : base(store)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TreeNodeAdvSubItemStyleInfo class.
        /// </summary>
        /// <param name="identity">A <see cref="TreeColumnAdvStyleInfoIdentity"/> that holds the indentity for this <see cref="TreeColumnAdvStyleInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public TreeNodeAdvSubItemStyleInfo(StyleInfoIdentityBase identity)
            : base(identity, new TreeNodeAdvSubItemStyleInfoStore())
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
                s_default = new TreeNodeAdvSubItemStyleInfo();

                s_default.Visible = true;
                s_default.Tag = null;
                s_default.BaseStyle = string.Empty;

                // text
                s_default.Font = FontUtil.CreateFont("Verdana", 8);
                s_default.LineAlignment = StringAlignment.Center;
                s_default.Alignment = StringAlignment.Near;
                s_default.Text = string.Empty;
                s_default.HelpText = string.Empty;
                s_default.TextColor = SystemColors.WindowText;
                s_default.Background = new BrushInfo(Color.Transparent);

                // images
                s_default.LeftImage = null;
                s_default.LeftImageIndices = new int[0];
                s_default.LeftImagePadding = 0;
                s_default.RightImage = null;
                s_default.RightImageIndices = new int[0];
                s_default.RightImagePadding = 0;

                // borders
                s_default.Border3DStyle = Border3DStyle.Flat;
                s_default.BorderColor = SystemColors.ControlDark;
                s_default.BorderSides = Border3DSide.All & (~Border3DSide.Middle);
                s_default.BorderSingle = ButtonBorderStyle.None;
                s_default.BorderStyle = BorderStyle.None;
            }

            return s_default;
        }
        #endregion

        #region BaseStyle
      
        [
          Description("The base style for the subitem"),
            Browsable(false),
            Category("Appearance - Inherited"),
            Editor(typeof(BaseStyleSelectorUITypeEditor), typeof(UITypeEditor))
          ]
        public string BaseStyle
        {
            get
            {
                return (string)GetValue(TreeNodeAdvSubItemStyleInfoStore.BaseStyleProperty);
            }
            set
            {
                SetValue(TreeNodeAdvSubItemStyleInfoStore.BaseStyleProperty, value);
            }
        }

        /// <summary>
        /// Reset property BaseStyle value to default value
        /// </summary>
        public void ResetBaseStyle()
        {
            ResetValue(TreeNodeAdvSubItemStyleInfoStore.BaseStyleProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize BaseStyle property value.
        /// </summary>
        /// <returns>Returns a value indicating whether should serilize BaseStyle</returns>
        public bool ShouldSerializeBaseStyle()
        {
            return HasBaseStyle;
        }

        /// <summary>
        /// Gets a value indicating whether BaseStyle property value has or not.
        /// </summary>
        [
          Browsable(false)
          ]
        public bool HasBaseStyle
        {
            get
            {
                return HasValue(TreeNodeAdvSubItemStyleInfoStore.BaseStyleProperty);
            }
        }
        #endregion

        #region Visible
        /// <summary> Gets or sets a value indicating whether the is Node subitem is visible or not.</summary>
        [
          Browsable(false),
            Description("Gets / sets the is Node subitem visible or not."),
            Category("Appearance")
          ]
        public bool Visible
        {
            get
            {
                return (bool)GetValue(TreeNodeAdvSubItemStyleInfoStore.VisibleProperty);
            }
            set
            {
                SetValue(TreeNodeAdvSubItemStyleInfoStore.VisibleProperty, value);
            }
        }

        /// <summary>
        /// Reset property Visible value to default value
        /// </summary>
        public void ResetVisible()
        {
            ResetValue(TreeNodeAdvSubItemStyleInfoStore.VisibleProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize Visible property value.
        /// </summary>
        /// <returns>Returns a value indicating whether should serilize visible</returns>
        public bool ShouldSerializeVisible()
        {
            return HasVisible;
        }

        /// <summary>
        /// Gets a value indicating whether Visible property value has or not.
        /// </summary>
        [
          Browsable(false)
          ]
        public bool HasVisible
        {
            get
            {
                return HasValue(TreeNodeAdvSubItemStyleInfoStore.VisibleProperty);
            }
        }
        #endregion

        #region Font
        /// <summary>Gets or sets the subitem text font.</summary>
        [
          Description("Gets / sets the subitem text font."),
            Category("Appearance")
          ]
        public Font Font
        {
            get
            {
                return (Font)GetValue(TreeNodeAdvSubItemStyleInfoStore.FontProperty);
            }
            set
            {
                SetValue(TreeNodeAdvSubItemStyleInfoStore.FontProperty, value);
            }
        }

        /// <summary>
        /// Reset property Font value to default value
        /// </summary>
        public void ResetFont()
        {
            ResetValue(TreeNodeAdvSubItemStyleInfoStore.FontProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize Font property value.
        /// </summary>
        /// <returns>Returns a value indicating whether should serilize Font</returns>
        public bool ShouldSerializeFont()
        {
            return HasFont;
        }

        /// <summary>
        /// Gets a value indicating whether Font property value has or not.
        /// </summary>
        [
          Browsable(false)
          ]
        public bool HasFont
        {
            get
            {
                return HasValue(TreeNodeAdvSubItemStyleInfoStore.FontProperty);
            }
        }
        #endregion

        #region LineAlignment
        /// <summary>Gets or sets the line alignment of the text in subitem.</summary>
        [
          Description("Gets / sets the line alignment of the text in subitem."),
            Category("Appearance")
          ]
        public StringAlignment LineAlignment
        {
            get
            {
                return (StringAlignment)GetValue(TreeNodeAdvSubItemStyleInfoStore.LineAlignmentProperty);
            }
            set
            {
                SetValue(TreeNodeAdvSubItemStyleInfoStore.LineAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Reset property LineAlignment value to default value
        /// </summary>
        public void ResetLineAlignment()
        {
            ResetValue(TreeNodeAdvSubItemStyleInfoStore.LineAlignmentProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize LineAlignment property value.
        /// </summary>
        /// <returns>Returns a value indicating whether should serilize Line Alignment</returns>
        public bool ShouldSerializeLineAlignment()
        {
            return HasLineAlignment;
        }

        /// <summary>
        /// Gets a value indicating whether LineAlignment property value has or not.
        /// </summary>
        [
          Browsable(false)
          ]
        public bool HasLineAlignment
        {
            get
            {
                return HasValue(TreeNodeAdvSubItemStyleInfoStore.LineAlignmentProperty);
            }
        }
        #endregion

        #region Alignment
        /// <summary>Gets or sets the text alignment in subitem bounds.</summary>
        [
          Description("Gets / sets the text alignment in subitem bounds."),
            Category("Appearance")
          ]
        public StringAlignment Alignment
        {
            get
            {
                return (StringAlignment)GetValue(TreeNodeAdvSubItemStyleInfoStore.AlignmentProperty);
            }
            set
            {
                SetValue(TreeNodeAdvSubItemStyleInfoStore.AlignmentProperty, value);
            }
        }

        /// <summary>
        /// Reset property Alignment value to default value
        /// </summary>
        public void ResetAlignment()
        {
            ResetValue(TreeNodeAdvSubItemStyleInfoStore.AlignmentProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize Alignment property value.
        /// </summary>
        /// <returns>Returns a value indicating whether should serilize Alignment</returns>
        public bool ShouldSerializeAlignment()
        {
            return HasAlignment;
        }

        /// <summary>
        /// Gets a value indicating whether Alignment property value has or not.
        /// </summary>
        [
          Browsable(false)
          ]
        public bool HasAlignment
        {
            get
            {
                return HasValue(TreeNodeAdvSubItemStyleInfoStore.AlignmentProperty);
            }
        }
        #endregion

        #region Text
        /// <summary>Gets or sets the subitem text.</summary>
        [
          Description("Gets / sets the subitem text."),
            Browsable(false),
            Category("Appearance")
          ]
        public string Text
        {
            get
            {
                return (string)GetValue(TreeNodeAdvSubItemStyleInfoStore.TextProperty);
            }
            set
            {
                SetValue(TreeNodeAdvSubItemStyleInfoStore.TextProperty, value);
            }
        }

        /// <summary>
        /// Reset property Text value to default value
        /// </summary>
        public void ResetText()
        {
            ResetValue(TreeNodeAdvSubItemStyleInfoStore.TextProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize Text property value.
        /// </summary>
        /// <returns>Returns a value indicating whether should serilize text</returns>
        public bool ShouldSerializeText()
        {
            return HasText;
        }

        /// <summary>
        /// Gets a value indicating whether Text property value has or not.
        /// </summary>
        [
          Browsable(false)
          ]
        public bool HasText
        {
            get
            {
                return HasValue(TreeNodeAdvSubItemStyleInfoStore.TextProperty);
            }
        }
        #endregion

        #region TextColor
        /// <summary>Gets or sets the subitem text color.</summary>
        [
          Description("Gets / sets the subitem text color."),
            Category("Appearance")
          ]
        public Color TextColor
        {
            get
            {
                return (Color)GetValue(TreeNodeAdvSubItemStyleInfoStore.TextColorProperty);
            }
            set
            {
                SetValue(TreeNodeAdvSubItemStyleInfoStore.TextColorProperty, value);
            }
        }

        /// <summary>
        /// Reset property TextColor value to default value
        /// </summary>
        public void ResetTextColor()
        {
            ResetValue(TreeNodeAdvSubItemStyleInfoStore.TextColorProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize TextColor property value.
        /// </summary>
        /// <returns>Returns a value indicating whether should serilize text Color</returns>
        public bool ShouldSerializeTextColor()
        {
            return HasTextColor;
        }

        /// <summary>
        /// Gets a value indicating whether TextColor property value has or not.
        /// </summary>
        [
          Browsable(false)
          ]
        public bool HasTextColor
        {
            get
            {
                return HasValue(TreeNodeAdvSubItemStyleInfoStore.TextColorProperty);
            }
        }
        #endregion

        #region HelpText
        /// <summary>Gets or sets the subitem help text.</summary>
        [
          Description("Gets / sets the subitem help text."),
            Browsable(false),
            Category("Appearance")
          ]
        public string HelpText
        {
            get
            {
                return (string)GetValue(TreeNodeAdvSubItemStyleInfoStore.HelpTextProperty);
            }
            set
            {
                SetValue(TreeNodeAdvSubItemStyleInfoStore.HelpTextProperty, value);
            }
        }

        /// <summary>
        /// Reset property HelpText value to default value
        /// </summary>
        public void ResetHelpText()
        {
            ResetValue(TreeNodeAdvSubItemStyleInfoStore.HelpTextProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize HelpText property value.
        /// </summary>
        /// <returns>Returns a value indicating whether should serilize Help text</returns>
        public bool ShouldSerializeHelpText()
        {
            return HasHelpText;
        }

        /// <summary>
        /// Gets a value indicating whether HelpText property value has or not.
        /// </summary>
        [
          Browsable(false)
          ]
        public bool HasHelpText
        {
            get
            {
                return HasValue(TreeNodeAdvSubItemStyleInfoStore.HelpTextProperty);
            }
        }
        #endregion

        #region Tag
        /// <summary>Gets or sets the subitem user data.</summary>
        [
          Description("Gets / sets the subitem user data."),
            Category("Data"),
            Browsable(false)
          ]
        public object Tag
        {
            get
            {
                return GetValue(TreeNodeAdvSubItemStyleInfoStore.TagProperty);
            }
            set
            {
                SetValue(TreeNodeAdvSubItemStyleInfoStore.TagProperty, value);
            }
        }

        /// <summary>
        /// Reset property Tag value to default value
        /// </summary>
        public void ResetTag()
        {
            ResetValue(TreeNodeAdvSubItemStyleInfoStore.TagProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize Tag property value.
        /// </summary>
        /// <returns>Returns a value indicating whether should serilize Tag property</returns>
        public bool ShouldSerializeTag()
        {
            return HasTag;
        }

        /// <summary>
        /// Gets a value indicating whether Tag property value has or not.
        /// </summary>
        [
          Browsable(false)
          ]
        public bool HasTag
        {
            get
            {
                return HasValue(TreeNodeAdvSubItemStyleInfoStore.TagProperty);
            }
        }
        #endregion

        #region LeftImage
        /// <summary>Gets or sets the subitem left image.</summary>
        [
          Description("Gets / sets the subitem left image."),
            Category("Images")
          ]
        public Image LeftImage
        {
            get
            {
                return (Image)GetValue(TreeNodeAdvSubItemStyleInfoStore.LeftImageProperty);
            }
            set
            {
                SetValue(TreeNodeAdvSubItemStyleInfoStore.LeftImageProperty, value);
            }
        }

        /// <summary>
        /// Reset property LeftImage value to default value
        /// </summary>
        public void ResetLeftImage()
        {
            ResetValue(TreeNodeAdvSubItemStyleInfoStore.LeftImageProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize LeftImage property value.
        /// </summary>
        /// <returns>Returns a value indicating whether should serilize Left Image</returns>
        public bool ShouldSerializeLeftImage()
        {
            return HasLeftImage;
        }

        /// <summary>
        /// Gets a value indicating whether LeftImage property value has or not.
        /// </summary>
        [
          Browsable(false)
          ]
        public bool HasLeftImage
        {
            get
            {
                return HasValue(TreeNodeAdvSubItemStyleInfoStore.LeftImageProperty);
            }
        }
        #endregion

        #region LeftImageIndices
        /// <summary>Gets or sets the subitem left side indices of images that stored in tree LeftImageList property.</summary>
        [
          Description("Gets / sets the subitem left side indices of images that stored in tree LeftImageList property."),
            Category("Images")
          ]
        public int[] LeftImageIndices
        {
            get
            {
                return (int[])GetValue(TreeNodeAdvSubItemStyleInfoStore.LeftImageIndicesProperty);
            }
            set
            {
                SetValue(TreeNodeAdvSubItemStyleInfoStore.LeftImageIndicesProperty, value);
            }
        }

        /// <summary>
        /// Reset property LeftImageIndices value to default value
        /// </summary>
        public void ResetLeftImageIndices()
        {
            ResetValue(TreeNodeAdvSubItemStyleInfoStore.LeftImageIndicesProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize LeftImageIndices property value.
        /// </summary>
        /// <returns>Returns a value indicating whether should serilize left Image Indics</returns>
        public bool ShouldSerializeLeftImageIndices()
        {
            return HasLeftImageIndices;
        }

        /// <summary>
        /// Gets a value indicating whether LeftImageIndices property value has or not.
        /// </summary>
        [
          Browsable(false)
          ]
        public bool HasLeftImageIndices
        {
            get
            {
                return HasValue(TreeNodeAdvSubItemStyleInfoStore.LeftImageIndicesProperty);
            }
        }
        #endregion

        #region LeftImagePadding
        /// <summary>Gets or sets the subitem padding between text and left images.</summary>
        [
          Description("Gets / sets the subitem padding between text and left images."),
            Category("Images")
          ]
        public int LeftImagePadding
        {
            get
            {
                return (int)GetValue(TreeNodeAdvSubItemStyleInfoStore.LeftImagePaddingProperty);
            }
            set
            {
                SetValue(TreeNodeAdvSubItemStyleInfoStore.LeftImagePaddingProperty, value);
            }
        }

        /// <summary>
        /// Reset property LeftImagePadding value to default value
        /// </summary>
        public void ResetLeftImagePadding()
        {
            ResetValue(TreeNodeAdvSubItemStyleInfoStore.LeftImagePaddingProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize LeftImagePadding property value.
        /// </summary>
        /// <returns>Returns a value indicating whether should serilize Image Padding</returns>
        public bool ShouldSerializeLeftImagePadding()
        {
            return HasLeftImagePadding;
        }

        /// <summary>
        /// Gets a value indicating whether LeftImagePadding property value has or not.
        /// </summary>
        [
          Browsable(false)
          ]
        public bool HasLeftImagePadding
        {
            get
            {
                return HasValue(TreeNodeAdvSubItemStyleInfoStore.LeftImagePaddingProperty);
            }
        }
        #endregion

        #region RightImage
        /// <summary>Gets or sets the subitem image from right side of the text.</summary>
        [
          Description("Gets / sets the subitem image from right side of the text."),
            Category("Images")
          ]
        public Image RightImage
        {
            get
            {
                return (Image)GetValue(TreeNodeAdvSubItemStyleInfoStore.RightImageProperty);
            }
            set
            {
                SetValue(TreeNodeAdvSubItemStyleInfoStore.RightImageProperty, value);
            }
        }

        /// <summary>
        /// Reset property RightImage value to default value
        /// </summary>
        public void ResetRightImage()
        {
            ResetValue(TreeNodeAdvSubItemStyleInfoStore.RightImageProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize RightImage property value.
        /// </summary>
        /// <returns>Returns a value indicating whether should serilize Right Image</returns>
        public bool ShouldSerializeRightImage()
        {
            return HasRightImage;
        }

        /// <summary>
        /// Gets a value indicating whether RightImage property value has or not.
        /// </summary>
        [
          Browsable(false)
          ]
        public bool HasRightImage
        {
            get
            {
                return HasValue(TreeNodeAdvSubItemStyleInfoStore.RightImageProperty);
            }
        }
        #endregion

        #region RightImageIndices
        /// <summary>Gets or sets the subitem right side indices of images that stored in tree RightImageList property.</summary>
        [
          Description("Gets / sets the subitem right side indices of images that stored in tree RightImageList property."),
            Category("Images")
          ]
        public int[] RightImageIndices
        {
            get
            {
                return (int[])GetValue(TreeNodeAdvSubItemStyleInfoStore.RightImageIndicesProperty);
            }
            set
            {
                SetValue(TreeNodeAdvSubItemStyleInfoStore.RightImageIndicesProperty, value);
            }
        }

        /// <summary>
        /// Reset property RightImageIndices value to default value
        /// </summary>
        public void ResetRightImageIndices()
        {
            ResetValue(TreeNodeAdvSubItemStyleInfoStore.RightImageIndicesProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize RightImageIndices property value.
        /// </summary>
        /// <returns>Returns a value indicating whether should serilize RightImageIndics</returns>
        public bool ShouldSerializeRightImageIndices()
        {
            return HasRightImageIndices;
        }

        /// <summary>
        /// Gets a value indicating whether RightImageIndices property value has or not.
        /// </summary>
        [
          Browsable(false)
          ]
        public bool HasRightImageIndices
        {
            get
            {
                return HasValue(TreeNodeAdvSubItemStyleInfoStore.RightImageIndicesProperty);
            }
        }
        #endregion

        #region RightImagePadding
        /// <summary>Gets or sets padding between text and right side images.</summary>
        [
          Description("Gets / sets padding between text and right side images."),
            Category("Images")
          ]
        public int RightImagePadding
        {
            get
            {
                return (int)GetValue(TreeNodeAdvSubItemStyleInfoStore.RightImagePaddingProperty);
            }
            set
            {
                SetValue(TreeNodeAdvSubItemStyleInfoStore.RightImagePaddingProperty, value);
            }
        }

        /// <summary>
        /// Reset property RightImagePadding value to default value
        /// </summary>
        public void ResetRightImagePadding()
        {
            ResetValue(TreeNodeAdvSubItemStyleInfoStore.RightImagePaddingProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize RightImagePadding property value.
        /// </summary>
        /// <returns>Returns should serialize RightImagePadding property</returns>
        public bool ShouldSerializeRightImagePadding()
        {
            return HasRightImagePadding;
        }

        /// <summary>
        /// Gets a value indicating whether RightImagePadding property value has or not.
        /// </summary>
        [
          Browsable(false)
          ]
        public bool HasRightImagePadding
        {
            get
            {
                return HasValue(TreeNodeAdvSubItemStyleInfoStore.RightImagePaddingProperty);
            }
        }
        #endregion

        #region Background
        /// <summary>Gets or sets the subitem background style.</summary>
        [
          Description("Gets / sets the subitem background style."),
            Category("Appearance")
          ]
        public BrushInfo Background
        {
            get
            {
                return (BrushInfo)GetValue(TreeNodeAdvSubItemStyleInfoStore.BackgroundProperty);
            }
            set
            {
                SetValue(TreeNodeAdvSubItemStyleInfoStore.BackgroundProperty, value);
            }
        }

        /// <summary>
        /// Reset property Background value to default value
        /// </summary>
        public void ResetBackground()
        {
            ResetValue(TreeNodeAdvSubItemStyleInfoStore.BackgroundProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize Background property value.
        /// </summary>
        /// <returns>Returns should serialize Background property</returns>
        public bool ShouldSerializeBackground()
        {
            return HasBackground;
        }

        /// <summary>
        /// Gets a value indicating whether Background has or not.
        /// </summary>
        [
          Browsable(false)
          ]
        public bool HasBackground
        {
            get
            {
                return HasValue(TreeNodeAdvSubItemStyleInfoStore.BackgroundProperty);
            }
        }
        #endregion

        #region BorderSides
        /// <summary>Gets or sets the subitem border sides settings.</summary>
        [
          Description("Gets / sets the subitem border sides settings."),
            Category("Borders")
          ]
        public Border3DSide BorderSides
        {
            get
            {
                return (Border3DSide)GetValue(TreeNodeAdvSubItemStyleInfoStore.BorderSidesProperty);
            }
            set
            {
                SetValue(TreeNodeAdvSubItemStyleInfoStore.BorderSidesProperty, value);
            }
        }

        /// <summary>
        /// Reset property BorderSides value to default value
        /// </summary>
        public void ResetBorderSides()
        {
            ResetValue(TreeNodeAdvSubItemStyleInfoStore.BorderSidesProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize BorderSides property value.
        /// </summary>
        /// <returns>Returns should serialize Bordersides property</returns>
        public bool ShouldSerializeBorderSides()
        {
            return HasBorderSides;
        }

        /// <summary>
        /// Gets a value indicating whether BorderSides has or not.
        /// </summary>
        [
          Browsable(false)
          ]
        public bool HasBorderSides
        {
            get
            {
                return HasValue(TreeNodeAdvSubItemStyleInfoStore.BorderSidesProperty);
            }
        }
        #endregion

        #region BorderStyle
        /// <summary>Gets or sets the subitem border style.</summary>
        [
          Description("Gets / sets the subitem border style."),
            Category("Borders")
          ]
        public BorderStyle BorderStyle
        {
            get
            {
                return (BorderStyle)GetValue(TreeNodeAdvSubItemStyleInfoStore.BorderStyleProperty);
            }
            set
            {
                SetValue(TreeNodeAdvSubItemStyleInfoStore.BorderStyleProperty, value);
            }
        }

        /// <summary>
        /// Reset property BorderStyle value to default value
        /// </summary>
        public void ResetBorderStyle()
        {
            ResetValue(TreeNodeAdvSubItemStyleInfoStore.BorderStyleProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize BorderStyle property value.
        /// </summary>
        /// <returns>Returns should serialize Border3DStyle property</returns>
        public bool ShouldSerializeBorderStyle()
        {
            return HasBorderStyle;
        }

        /// <summary>
        /// Gets a value indicating whether BorderStyle has or not.
        /// </summary>
        [
          Browsable(false)
          ]
        public bool HasBorderStyle
        {
            get
            {
                return HasValue(TreeNodeAdvSubItemStyleInfoStore.BorderStyleProperty);
            }
        }
        #endregion

        #region Border3DStyle
        /// <summary>Gets or sets the subitem border 3D style.</summary>
        [
          Description("Gets / sets the subitem border 3D style."),
            Category("Borders")
          ]
        public Border3DStyle Border3DStyle
        {
            get
            {
                return (Border3DStyle)GetValue(TreeNodeAdvSubItemStyleInfoStore.Border3DStyleProperty);
            }
            set
            {
                SetValue(TreeNodeAdvSubItemStyleInfoStore.Border3DStyleProperty, value);
            }
        }

        /// <summary>
        /// Reset property Border3DStyle value to default value
        /// </summary>
        public void ResetBorder3DStyle()
        {
            ResetValue(TreeNodeAdvSubItemStyleInfoStore.Border3DStyleProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize Border3DStyle property value.
        /// </summary>
        /// <returns>Returns serialize Border3DStyle property</returns>    
        public bool ShouldSerializeBorder3DStyle()
        {
            return HasBorder3DStyle;
        }

        /// <summary>
        /// Gets a value indicating whether Border3DStyle gas or not.
        /// </summary>
        [
          Browsable(false)
          ]
        public bool HasBorder3DStyle
        {
            get
            {
                return HasValue(TreeNodeAdvSubItemStyleInfoStore.Border3DStyleProperty);
            }
        }
        #endregion

        #region BorderColor
        /// <summary>Gets or sets the subitem border color.</summary>
        [
          Description("Gets / sets the subitem border color."),
            Category("Borders")
          ]
        public Color BorderColor
        {
            get
            {
                return (Color)GetValue(TreeNodeAdvSubItemStyleInfoStore.BorderColorProperty);
            }
            set
            {
                SetValue(TreeNodeAdvSubItemStyleInfoStore.BorderColorProperty, value);
            }
        }

        /// <summary>
        /// Reset property BorderColor value to default value
        /// </summary>
        public void ResetBorderColor()
        {
            ResetValue(TreeNodeAdvSubItemStyleInfoStore.BorderColorProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize BorderColor property value. 
        /// </summary>
        /// <returns>Returns Bool value</returns>
        public bool ShouldSerializeBorderColor()
        {
            return HasBorderColor;
        }

        /// <summary>
        /// Gets a value indicating whether BorderColor has not.
        /// </summary>     
        [
          Browsable(false)
          ]
        public bool HasBorderColor
        {
            get
            {
                return HasValue(TreeNodeAdvSubItemStyleInfoStore.BorderColorProperty);
            }
        }
        #endregion

        #region BorderSingle
        /// <summary>Gets or sets the subitem single line border style.</summary>
        [
          Description("Gets or sets the subitem single line border style."),
            Category("Borders")
          ]
        public ButtonBorderStyle BorderSingle
        {
            get
            {
                return (ButtonBorderStyle)GetValue(TreeNodeAdvSubItemStyleInfoStore.BorderSingleProperty);
            }
            set
            {
                SetValue(TreeNodeAdvSubItemStyleInfoStore.BorderSingleProperty, value);
            }
        }

        /// <summary>
        /// Reset property BorderSingle value to default value
        /// </summary>
        public void ResetBorderSingle()
        {
            ResetValue(TreeNodeAdvSubItemStyleInfoStore.BorderSingleProperty);
        }
    
        public bool ShouldSerializeBorderSingle()
        {
            return HasBorderSingle;
        }

        /// <summary>
        /// Gets a value indicating whether BorderSingle or not.
        /// </summary>
        [
          Browsable(false)
          ]
        public bool HasBorderSingle
        {
            get
            {
                return HasValue(TreeNodeAdvSubItemStyleInfoStore.BorderSingleProperty);
            }
        }
        #endregion
    }
}