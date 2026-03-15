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
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using Syncfusion.Drawing;
using Syncfusion.Styles;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    /// <summary>
    /// Contains appearance and behavior information regarding the <see cref="TreeNodeAdv"/>s.
    /// </summary>
    [TypeConverter(typeof(TreeNodeAdvStyleInfoConverter))]
    public class TreeNodeAdvStyleInfo :
      StyleInfoBase,
      ITreeNodeAdvSubItemStyle
    {
        #region Class static members

        // Static Fields
        private static TreeNodeAdvStyleInfo defaultStyle = null;

        /// <summary>
        /// An empty style object.
        /// </summary>
        public static readonly TreeNodeAdvStyleInfo Empty = new TreeNodeAdvStyleInfo();
        #endregion
     
        /// <summary>
        /// Initializes a new instance of the TreeNodeAdvStyleInfo class.
        /// </summary>
        [DebuggerStepThrough()]
        public TreeNodeAdvStyleInfo()
            : base(new TreeNodeAdvStyleInfoStore())
        {
        }

        /// <summary>
        /// Initializes a new instance of the TreeNodeAdvStyleInfo class.
        /// </summary>
        /// <param name="style">The style object that contains the original data.</param>
        [DebuggerStepThrough()]
        public TreeNodeAdvStyleInfo(TreeNodeAdvStyleInfo style)
            : base(style.Store)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNodeAdvStyleInfo"/> class.
        /// </summary>
        /// <param name="store">A <see cref="TreeNodeAdvStyleInfoStore"/> that holds data for this <see cref="TreeNodeAdvStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="TreeNodeAdvStyleInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public TreeNodeAdvStyleInfo(TreeNodeAdvStyleInfoStore store)
            : base(store)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNodeAdvStyleInfo"/> class.
        /// </summary>
        /// <param name="identity">A <see cref="TreeNodeAdvStyleInfoIdentity"/> that holds the indentity for this <see cref="TreeNodeAdvStyleInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public TreeNodeAdvStyleInfo(StyleInfoIdentityBase identity)
            : base(identity, new TreeNodeAdvStyleInfoStore())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNodeAdvStyleInfo"/> class.
        /// </summary>
        /// <param name="identity">A <see cref="TreeNodeAdvStyleInfoIdentity"/> that holds the indentity for this <see cref="TreeNodeAdvStyleInfo"/>.
        /// </param>
        /// <param name="store">A <see cref="TreeNodeAdvStyleInfoStore"/> that holds data for this <see cref="TreeNodeAdvStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="TreeNodeAdvStyleInfoStore"/> object.
        /// </param>
        [DebuggerStepThrough()]
        public TreeNodeAdvStyleInfo(StyleInfoIdentityBase identity, TreeNodeAdvStyleInfoStore store)
            : base(identity, store)
        {
        }

        #region Class properties
        /// <summary>
        /// Gets or sets information such as TreeNode for the current <see cref="TreeNodeAdvStyleInfo"/>.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal TreeNodeAdvStyleInfoIdentity NodeIdentity
        {
            get
            {
                return base.Identity as TreeNodeAdvStyleInfoIdentity;
            }
            set
            {
                base.Identity = value;
            }
        }

        /// <summary>
        /// Returns the <see cref="TreeNodeAdv"/> for this style or null if style is used outside a grid model.
        /// </summary>
        /// <returns>The <see cref="TreeNodeAdv"/> this style belongs to or null.</returns>
        public TreeNodeAdv GetNode()
        {
            TreeNodeAdvStyleInfoIdentity nodeId = base.Identity as TreeNodeAdvStyleInfoIdentity;
            TreeNodeAdv node = (nodeId != null) ? nodeId.TreeNode : null;
            return node;
        }

        /// <summary>
        /// Gets the <see cref="TreeNodeAdvStyleInfoStore"/> object that holds all the data for this style object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new TreeNodeAdvStyleInfoStore Store
        {
            get
            {
                return (TreeNodeAdvStyleInfoStore)base.Store;
            }
        }

        // Default

        /// <summary>
        /// Gets a <see cref="TreeNodeAdvStyleInfo"/> with default settings.
        /// </summary>
        public static TreeNodeAdvStyleInfo Default
        {
            get
            {
                if (TreeNodeAdvStyleInfo.defaultStyle == null)
                {
                    defaultStyle = new TreeNodeAdvStyleInfo();
                    defaultStyle.Comparer = null;
                    defaultStyle.Text = string.Empty;
                    defaultStyle.Multiline = false;
                    defaultStyle.HelpText = string.Empty;
                    defaultStyle.DisplayMember = string.Empty;
                    defaultStyle.BaseStyle = String.Empty;
                    defaultStyle.Tag = null;
                    defaultStyle.LeftImageIndices = new int[0];
                    defaultStyle.RightImageIndices = new int[0];
                    defaultStyle.NoChildrenImgIndex = 0;
                    defaultStyle.ClosedImgIndex = 1;
                    defaultStyle.OpenImgIndex = 2;
                    defaultStyle.Font = FontUtil.CreateFont("Verdana", 8);
                    defaultStyle.Background = new BrushInfo(Color.Transparent);
                    defaultStyle.TextColor = SystemColors.WindowText;
                    defaultStyle.Height = 16;
                    defaultStyle.ShowCheckBox = false;
                    defaultStyle.CheckColor = SystemColors.ControlText;
                    defaultStyle.IntermediateCheckColor = SystemColors.ControlDark;
                    defaultStyle.CheckBoxBackground = SystemBrushes.Window;
                    defaultStyle.IntermediateCheckBoxBackground = SystemBrushes.Control;
                    defaultStyle.ShowOptionButton = false;
                    defaultStyle.ShowPlusMinus = true;
                    defaultStyle.InteractiveCheckBox = false;
                    defaultStyle.OptionButtonColor = Color.White;
                    defaultStyle.SelectedOptionButtonColor = Color.Black;

                    defaultStyle.SortOrder = SortOrder.None;
                    defaultStyle.SortType = TreeNodeAdvSortType.Text;
                    defaultStyle.ThemesEnabled = false;
                    defaultStyle.Comparer = null;
                    defaultStyle.CompareOptions = CompareOptions.None;
                    defaultStyle.CheckState = CheckState.Unchecked;
                    defaultStyle.Enabled = true;
                    defaultStyle.EnabledButtons = true;
                    defaultStyle.EnsureDefaultOptionedChild = true;

                    defaultStyle.LeftImage = null;
                    defaultStyle.LeftImagePadding = 0;
                    defaultStyle.RightImage = null;
                    defaultStyle.RightImagePadding = 0;
                    defaultStyle.LeftStateImagePadding = 0;
                    defaultStyle.RightStateImagePadding = 0;
                    defaultStyle.ClosedImage = null;
                    defaultStyle.OpenImage = null;
                }

                return TreeNodeAdvStyleInfo.defaultStyle;
            }
        }
        #endregion

        #region Class overrides

        [DebuggerStepThrough()]
        public override StyleInfoSubObjectIdentity CreateSubObjectIdentity(StyleInfoProperty sip)
        {
            throw new NotSupportedException();

            // return new TreeNodeAdvStyleInfoSubObjectIdentity(this, sip);
        }

#if later

    /// <summary>
    /// Creates a new <see cref="TreeNodeAdvStyleInfo"/> and copies its cell and identity information from the current object. The new
    /// instance will be made offline so that changes in this style object are not be stored in the GridData
    /// </summary>
    /// <returns>A new <see cref="TreeNodeAdvStyleInfo"/> intance.</returns>
    /// <remarks>
    /// Lets a style object load base styles, default values but disables
    /// saving changes back to the grid. (see OnStyleChanged below)
    /// </remarks>
    [ DebuggerStepThrough() ]
    public TreeNodeAdvStyleInfo GetOffLineCopy()
    {
      return new TreeNodeAdvStyleInfo( ( ( TreeNodeAdvStyleInfoIdentity )Identity ).MakeOfflineIdentity(), ( TreeNodeAdvStyleInfoStore )Store.Clone() );
    }

    private TreeNodeAdvStyleInfoCustomPropertiesCollection cpl = null;

    /// <summary>
    /// Returns a collection of custom property objects that have 
    /// at least one initialized value. The primary purpose of this 
    /// collection is to support design-time code serialization of
    /// custom properties.
    /// </summary>
    [ Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Content ) ]
    public TreeNodeAdvStyleInfoCustomPropertiesCollection CustomProperties
    {
      get
      {
        if( cpl == null )
        {
          cpl = new TreeNodeAdvStyleInfoCustomPropertiesCollection( this );
        }
        return cpl;
      }
    }

    private bool ShouldSerializeCustomProperties()
    {
      return CustomProperties.Count > 0;
    }
#endif

        protected override void OnStyleChanged(StyleInfoProperty sip)
        {
            // cpl = null;
            base.OnStyleChanged(sip);
        }

        protected override StyleInfoBase GetDefaultStyle()
        {
            return TreeNodeAdvStyleInfo.Default;
        }
        #endregion

        #region /* unclassified properties */
        /// <summary>
        /// Gets or sets the color of check symbol.
        /// </summary>
        [
        Description("Indicates the color of check symbol."),
        Category("Appearance"),
        DefaultValue(typeof(Color), "ControlText")
        ]
        public Color CheckColor
        {
            get
            {
                return (Color)GetValue(TreeNodeAdvStyleInfoStore.CheckColorProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.CheckColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of intermediate check symbol.
        /// </summary>
        [
        Description("Indicates the color of intermediate check symbol."),
        Category("Appearance"),
        DefaultValue(typeof(Color), "ControlDark")
        ]
        public Color IntermediateCheckColor
        {
            get
            {
                return (Color)GetValue(TreeNodeAdvStyleInfoStore.IntermediateCheckColorProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.IntermediateCheckColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the background of checkbox when it is in intermediate state.
        /// </summary>
        [
        Description("Indicates the background of checkbox when it is in intermediate state."),
        Browsable(false),
        Category("Appearance")
        ]
        public Brush IntermediateCheckBoxBackground
        {
            get
            {
                return (Brush)GetValue(TreeNodeAdvStyleInfoStore.IntermediateCheckBoxBackgroundProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.IntermediateCheckBoxBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the background of checkbox .
        /// </summary>
        [
        Description("Indicates the background of checkbox."),
        Browsable(false),
        Category("Appearance")
        ]
        public Brush CheckBoxBackground
        {
            get
            {
                return (Brush)GetValue(TreeNodeAdvStyleInfoStore.CheckBoxBackGroundProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.CheckBoxBackGroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of Option button.
        /// </summary>
        [Description("Indicates the color of option button."), Category("Appearance"), DefaultValue(typeof(Color), "White")]

        public Color OptionButtonColor
        {
            get
            {
                return (Color)GetValue(TreeNodeAdvStyleInfoStore.OptionButtonColorProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.OptionButtonColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of Selected Option button.
        /// </summary>
        [Description("Indicates the color of Selected option button."), Category("Appearance"), DefaultValue(typeof(Color), "Black")]

        public Color SelectedOptionButtonColor
        {
            get
            {
                return (Color)GetValue(TreeNodeAdvStyleInfoStore.SelectedOptionButtonColorProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.SelectedOptionButtonColorProperty, value);
            }
        }
        #endregion

        #region Font
        /// <summary>
        /// Gets or sets the font of the node.
        /// </summary>
        [
        Description("The font of the node."),
        Category("Appearance"),
        Localizable(true)
        ]
        public virtual Font Font
        {
            get
            {
                return ((Font)GetValue(TreeNodeAdvStyleInfoStore.FontProperty)).Clone() as Font;
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.FontProperty, value.Clone() as Font);
            }
        }

        public void ResetFont()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.FontProperty);
        }

        public bool ShouldSerializeFont()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.FontProperty);
        }

        [
        Browsable(false)
        ]
        public virtual bool HasFont
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.FontProperty);
            }
        }
        #endregion

        #region TextColor
        /// <summary>
        /// Gets or sets the Color of the text.
        /// </summary>
        [
        Description("The Color of the text."),
        Category("Appearance")
        ]
        public virtual Color TextColor
        {
            get
            {
                return (Color)GetValue(TreeNodeAdvStyleInfoStore.TextColorProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.TextColorProperty, value);
            }
        }

        public void ResetTextColor()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.TextColorProperty);
        }

        public bool ShouldSerializeTextColor()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.TextColorProperty);
        }

        [
        Browsable(false)
        ]
        public virtual bool HasTextColor
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.TextColorProperty);
            }
        }
        #endregion

        #region BaseStyle
        /// <summary>
        /// Gets or sets the base style for the node from which to inherit.
        /// </summary>
        /// <remarks>The specified base style should be available in the <see cref="TreeViewAdv.BaseStyles"/>
        /// collection.</remarks>
        [
        Description("The base style for the node"),
        Category("Appearance - Inherited")
        ]
        public string BaseStyle
        {
            get
            {
                return (string)GetValue(TreeNodeAdvStyleInfoStore.BaseStyleProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.BaseStyleProperty, value);
            }
        }

        public void ResetBaseStyle()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.BaseStyleProperty);
        }

        public bool ShouldSerializeBaseStyle()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.BaseStyleProperty);
        }

        [
        Browsable(false)
        ]
        public bool HasBaseStyle
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.BaseStyleProperty);
            }
        }
        #endregion

        #region Background
        /// <summary>
        /// Gets or sets the background of the node.
        /// </summary>
        [
        Description("The background of the node."),
        Category("Appearance")
        ]
        public BrushInfo Background
        {
            get
            {
                return (BrushInfo)GetValue(TreeNodeAdvStyleInfoStore.BackgroundProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.BackgroundProperty, value);
            }
        }

        public void ResetBackground()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.BackgroundProperty);
        }

        public bool ShouldSerializeBackground()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.BackgroundProperty);
        }

        [
        Browsable(false)
        ]
        public bool HasBackground
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.BackgroundProperty);
            }
        }
        #endregion

        #region Text
        /// <summary>
        /// Gets or sets the text of the node.
        /// </summary>
        [
        Description("The text of the node."),
        Category("Appearance"),
        Browsable(false),
        Localizable(true)
        ]
        public string Text
        {
            get
            {
                return (string)GetValue(TreeNodeAdvStyleInfoStore.TextProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.TextProperty, value);
            }
        }

        public void ResetText()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.TextProperty);
        }

        public bool ShouldSerializeText()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.TextProperty);
        }
        #endregion

        #region HelpText
        /// <summary>
        /// Gets or sets the help text of the node.
        /// </summary>
        [
        Description("The help text of the node."),
        Category("Appearance"),
        Localizable(true)
        ]
        public string HelpText
        {
            get
            {
                return (string)GetValue(TreeNodeAdvStyleInfoStore.HelpTextProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.HelpTextProperty, value);
            }
        }

        public void ResetHelpText()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.HelpTextProperty);
        }

        public bool ShouldSerializeHelpText()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.HelpTextProperty);
        }
        #endregion

        #region DisplayMember
        /// <summary>
        /// Gets or sets the display member of the data bound to the node.
        /// </summary>
        [
        Description("The display member of the data bound to the node."),
        Category("Data"),
        Localizable(true)
        ]
        public string DisplayMember
        {
            get
            {
                return (string)GetValue(TreeNodeAdvStyleInfoStore.DisplayMemberProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.DisplayMemberProperty, value);
            }
        }

        public void ResetDisplayMember()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.DisplayMemberProperty);
        }

        public bool ShouldSerializeDisplayMember()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.DisplayMemberProperty);
        }
        #endregion

        #region Height
        /// <summary>
        /// Gets or sets the height of the node.
        /// </summary>
        [
        Description("The height of the node."),
        Category("Appearance")
        ]
        public virtual int Height
        {
            get
            {
                return (int)GetValue(TreeNodeAdvStyleInfoStore.HeightProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.HeightProperty, value);
            }
        }

        public void ResetHeight()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.HeightProperty);
        }

        public bool ShouldSerializeHeight()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.HeightProperty);
        }

        [
        Browsable(false)
        ]
        public virtual bool HasHeight
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.HeightProperty);
            }
        }
        #endregion

        #region ShowCheckBox
        /// <summary>
        /// Gets or sets a value indicating whether the checkbox of the node is visible.
        /// </summary>
        [
        DefaultValue(false),
        Description("Indicates if the checkbox of the node is visible."),
        Category("Appearance")
        ]
        public bool ShowCheckBox
        {
            get
            {
                return (bool)GetValue(TreeNodeAdvStyleInfoStore.ShowCheckBoxProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.ShowCheckBoxProperty, value);
            }
        }

        [
        Browsable(false)
        ]
        public bool HasShowCheckBox
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.ShowCheckBoxProperty);
            }
        }

        public void ResetShowCheckBox()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.ShowCheckBoxProperty);
        }

        public bool ShouldSerializeShowCheckBox()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.ShowCheckBoxProperty);
        }
        #endregion

        #region InteractiveCheckBox
        /// <summary>
        /// Gets or sets a value indicating whether the node will have an interactive checkbox.
        /// </summary>
        [
        Description("Indicates if the node will have an interactive checkbox."),
        Category("Behavior")
        ]
        public bool InteractiveCheckBox
        {
            get
            {
                return (bool)GetValue(TreeNodeAdvStyleInfoStore.InteractiveCheckBoxProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.InteractiveCheckBoxProperty, value);
            }
        }

        public void ResetInteractiveCheckBox()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.InteractiveCheckBoxProperty);
        }

        public bool ShouldSerializeInteractiveCheckBox()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.InteractiveCheckBoxProperty);
        }

        [
        Browsable(false)
        ]
        public bool HasInteractiveCheckBox
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.InteractiveCheckBoxProperty);
            }
        }
        #endregion

        #region Tag
        /// <summary>
        /// Gets or sets the tag of the node. Can be used to store additional information for the node.
        /// </summary>
        [
        Description("The tag of the node. Can be used to store aditional information for the node."),
        Category("Data"),
        Browsable(false)
        ]
        public object Tag
        {
            get
            {
                return (object)GetValue(TreeNodeAdvStyleInfoStore.TagProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.TagProperty, value);
            }
        }
        public void ResetTag()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.TagProperty);
        }

        public bool ShouldSerializeTag()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.TagProperty);
        }

        [
        Browsable(false)
        ]
        public bool HasTag
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.TagProperty);
            }
        }
        #endregion

        #region ShowPlusMinus
        /// <summary>
        /// Gets or sets a value indicating whether the plus/minus of the node is visible.
        /// </summary>
        [
        Description("Indicates if the plus/minus of the node is visible."),
        Category("Appearance")
        ]
        public bool ShowPlusMinus
        {
            get
            {
                return (bool)GetValue(TreeNodeAdvStyleInfoStore.ShowPlusMinusProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.ShowPlusMinusProperty, value);
            }
        }

        public void ResetShowPlusMinus()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.ShowPlusMinusProperty);
        }

        public bool ShouldSerializeShowPlusMinus()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.ShowPlusMinusProperty);
        }
  
        [
        Browsable(false)
        ]
        public bool HasShowPlusMinus
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.ShowPlusMinusProperty);
            }
        }
        #endregion

        #region ShowOptionButton
        /// <summary>
        /// Gets or sets a value indicating whether the optionbutton of the node is visible.
        /// </summary>
        [
        DefaultValue(false),
        Description("Indicates if the optionbutton of the node is visible."),
        Category("Appearance")
        ]
        public bool ShowOptionButton
        {
            get
            {
                return (bool)GetValue(TreeNodeAdvStyleInfoStore.ShowOptionButtonProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.ShowOptionButtonProperty, value);
            }
        }

        [
        Browsable(false)
        ]
        public bool HasShowOptionButton
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.ShowOptionButtonProperty);
            }
        }

        public void ResetShowOptionButton()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.ShowOptionButtonProperty);
        }

        public bool ShouldSerializeShowOptionButton()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.ShowOptionButtonProperty);
        }
        #endregion

        #region LeftImageIndices

        /// <summary>
        /// Gets or sets the image indices of the images to be drawn on the left of the node`s text.
        /// </summary>
        [
        Description("The imageindex to be drawn on the left of the node`s text."),
        Category("Images"),
        Localizable(true)
        ]
        public int[] LeftImageIndices
        {
            get
            {
                return (int[])GetValue(TreeNodeAdvStyleInfoStore.LeftImageIndicesProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.LeftImageIndicesProperty, value);
            }
        }

        public void ResetLeftImageIndices()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.LeftImageIndicesProperty);
        }

        public bool ShouldSerializeLeftImageIndices()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.LeftImageIndicesProperty);
        }

        [
        Browsable(false)
        ]
        public bool HasLeftImageIndices
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.LeftImageIndicesProperty);
            }
        }
        #endregion

        #region RightImageIndices
        /// <summary>
        /// Gets or sets the image indices of the images  to be drawn on the right of the node`s text.
        /// </summary>
        [
        Description("The imageindex to be drawn on the right of the node`s text."),
        Category("Images"),
        Localizable(true)
        ]
        public int[] RightImageIndices
        {
            get
            {
                return (int[])GetValue(TreeNodeAdvStyleInfoStore.RightImageIndicesProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.RightImageIndicesProperty, value);
            }
        }

        public void ResetRightImageIndices()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.RightImageIndicesProperty);
        }

        public bool ShouldSerializeRightImageIndices()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.RightImageIndicesProperty);
        }

        [
        Browsable(false)
        ]
        public bool HasRightImageIndices
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.RightImageIndicesProperty);
            }
        }
        #endregion

        #region NoChildrenImgIndex
        /// <summary>
        /// Gets or sets the image index indicating the image in the StateImageList where the node has no children.
        /// </summary>
        [
        Description("The imageindex indicating the image in the StateImageList where the node has no children."),
        Category("Images"),
        Localizable(true)
        ]
        public int NoChildrenImgIndex
        {
            get
            {
                return (int)GetValue(TreeNodeAdvStyleInfoStore.NoChildrenImgIndexProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.NoChildrenImgIndexProperty, value);
            }
        }

        public void ResetNoChildrenImgIndex()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.NoChildrenImgIndexProperty);
        }

        public bool ShouldSerializeNoChildrenImgIndex()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.NoChildrenImgIndexProperty);
        }

        [
        Browsable(false)
        ]
        public bool HasNoChildrenImgIndex
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.NoChildrenImgIndexProperty);
            }
        }
        #endregion

        #region ExpandImageIndex
        /// <summary>
        /// Gets or sets the image index in the NodeStateImageList where the node is expanded.
        /// </summary>
        [
        Description("Indicates the imageindex in the NodeStateImageList where the node is expanded."),
        Category("Images"),
        Localizable(true)
        ]
        public int ExpandImageIndex
        {
            get
            {
                if (!HasExpandImageIndex)
                {
                    return -1;
                }

                return (int)GetValue(TreeNodeAdvStyleInfoStore.ExpandImageIndexProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.ExpandImageIndexProperty, value);
            }
        }

        public void ResetExpandImageIndex()
        {
            this.ResetValue(TreeNodeAdvStyleInfoStore.ExpandImageIndexProperty);
        }

        public bool ShouldSerializeExpandImageIndex()
        {
            return HasExpandImageIndex;
        }

        [
        Browsable(false)
        ]
        public bool HasExpandImageIndex
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.ExpandImageIndexProperty);
            }
        }
        #endregion

        #region CollapseImageIndex
        /// <summary>
        /// Gets or sets the image index in the NodeStateImageList where the node is collapsed.
        /// </summary>
        [
        Description("Indicates the imageindex in the NodeStateImageList where the node is collapsed."),
        Category("Images"),
        Localizable(true)
        ]
        public int CollapseImageIndex
        {
            get
            {
                if (!HasCollapseImageIndex)
                {
                    return -1;
                }

                return (int)GetValue(TreeNodeAdvStyleInfoStore.CollapseImageIndexProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.CollapseImageIndexProperty, value);
            }
        }

        public void ResetCollapseImageIndex()
        {
            this.ResetValue(TreeNodeAdvStyleInfoStore.CollapseImageIndexProperty);
        }

        public bool ShouldSerializeCollapseImageIndex()
        {
            return HasCollapseImageIndex;
        }
        [
        Browsable(false)
        ]
        public bool HasCollapseImageIndex
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.CollapseImageIndexProperty);
            }
        }
        #endregion

        #region OpenImgIndex
        /// <summary>
        /// Gets or sets the image index in the StateImageList where the node is expanded.
        /// </summary>
        [
        Description("Indicates the imageindex in the StateImageList where the node is expanded."),
        Category("Images"),
        Localizable(true)
        ]
        public int OpenImgIndex
        {
            get
            {
                return (int)GetValue(TreeNodeAdvStyleInfoStore.OpenImgIndexProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.OpenImgIndexProperty, value);
            }
        }

        public void ResetOpenImgIndex()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.OpenImgIndexProperty);
        }

        public bool ShouldSerializeOpenImgIndex()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.OpenImgIndexProperty);
        }

        [
        Browsable(false)
        ]
        public bool HasOpenImgIndex
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.OpenImgIndexProperty);
            }
        }
        #endregion

        #region ClosedImgIndex
        /// <summary>
        /// Gets or sets the image index in the StateImageList where the node is not expanded.
        /// </summary>
        [
        Description("Indicates the imageindex in the StateImageList where the node is not expanded."),
        Category("Images"),
        Localizable(true)
        ]
        public int ClosedImgIndex
        {
            get
            {
                return (int)GetValue(TreeNodeAdvStyleInfoStore.ClosedImgIndexProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.ClosedImgIndexProperty, value);
            }
        }

        public void ResetClosedImgIndex()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.ClosedImgIndexProperty);
        }
        public bool ShouldSerializeClosedImgIndex()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.ClosedImgIndexProperty);
        }

        [
        Browsable(false)
        ]
        public bool HasClosedImgIndex
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.ClosedImgIndexProperty);
            }
        }
        #endregion

        #region ThemesEnabled
        /// <summary>
        /// Gets or sets a value indicating whether the node`s controls will be themed.
        /// </summary>
        [
        Description("Indicates if the node`s controls will be themed."),
        Category("Appearance")
        ]
        public bool ThemesEnabled
        {
            get
            {
                return (bool)GetValue(TreeNodeAdvStyleInfoStore.ThemesEnabledProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.ThemesEnabledProperty, value);
            }
        }

        public void ResetThemesEnabled()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.ThemesEnabledProperty);
        }

        public bool ShouldSerializeThemesEnabled()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.ThemesEnabledProperty);
        }

        [
        Browsable(false)
        ]
        public bool HasThemesEnabled
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.ThemesEnabledProperty);
            }
        }
        #endregion

        #region SortType
        /// <summary>
        /// Gets or sets the sort type of the node.
        /// </summary>
        [
        Description("Indicates the sort type of the node."),
        Category("Sorting"),
        Localizable(true)
        ]
        public TreeNodeAdvSortType SortType
        {
            get
            {
                return (TreeNodeAdvSortType)GetValue(TreeNodeAdvStyleInfoStore.SortTypeProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.SortTypeProperty, value);
            }
        }

        public void ResetSortType()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.SortTypeProperty);
        }

        public bool ShouldSerializeSortType()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.SortTypeProperty);
        }

        [
        Browsable(false)
        ]
        public bool HasSortType
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.SortTypeProperty);
            }
        }
        #endregion

        #region SortOrder
        /// <summary>
        /// Gets or sets the sort order of the node.
        /// </summary>
        [
        Description("Indicates the sort order of the node."),
        Category("Sorting"),
        Localizable(true)
        ]
        public SortOrder SortOrder
        {
            get
            {
                return (SortOrder)GetValue(TreeNodeAdvStyleInfoStore.SortOrderProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.SortOrderProperty, value);
            }
        }

        public void ResetSortOrder()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.SortOrderProperty);
        }

        public bool ShouldSerializeSortOrder()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.SortOrderProperty);
        }

        [
        Browsable(false)
        ]
        public bool HasSortOrder
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.SortOrderProperty);
            }
        }
        #endregion

        #region Culture
        /// <summary>
        /// Gets or sets the culture of the node used while sorting.
        /// </summary>
        [
        Description("Indicates the culture of the node used while sorting."),
        Category("Sorting"),
        Localizable(true)
        ]
        public CultureInfo Culture
        {
            get
            {
                return (CultureInfo)GetValue(TreeNodeAdvStyleInfoStore.CultureProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.CultureProperty, value);
            }
        }

        public void ResetCulture()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.CultureProperty);
        }
        public bool ShouldSerializeCulture()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.CultureProperty);
        }
        #endregion

        #region Comparer
        /// <summary>
        /// Gets or sets the <see cref="IComparer"/> object that compares two nodes.
        /// </summary>
        [
        Description("Indicates the IComparer object that compares two nodes."),
        Category("Sorting")
        ]
        public IComparer Comparer
        {
            get
            {
                return (IComparer)GetValue(TreeNodeAdvStyleInfoStore.ComparerProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.ComparerProperty, value);
            }
        }

        public void ResetComparer()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.ComparerProperty);
        }

        public bool ShouldSerializeComparer()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.ComparerProperty);
        }

        [
        Browsable(false)
        ]
        public bool HasComparer
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.ComparerProperty);
            }
        }
        #endregion

        #region CompareOptions
        /// <summary>
        /// Gets or sets the compare options used in the sorting of the node.
        /// </summary>
        [
        Description("Indicates the compare options used in the sorting of the node."),
        Category("Sorting"),
        Localizable(true)
        ]
        public CompareOptions CompareOptions
        {
            get
            {
                return (CompareOptions)GetValue(TreeNodeAdvStyleInfoStore.CompareOptionsProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.CompareOptionsProperty, value);
            }
        }

        [
        Browsable(false)
        ]
        public bool HasCompareOptions
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.CompareOptionsProperty);
            }
        }

        public void ResetCompareOptions()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.CompareOptionsProperty);
        }

        public bool ShouldSerializeCompareOptions()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.CompareOptionsProperty);
        }
        #endregion

        #region Enabled
        /// <summary>
        /// Gets or sets a value indicating whether the node is enabled.
        /// </summary>
        [
        Description("Specifies if the node is enabled."),
        Category("Appearance"),
        Localizable(true)
        ]
        public bool Enabled
        {
            get
            {
                return (bool)GetValue(TreeNodeAdvStyleInfoStore.EnabledProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.EnabledProperty, value);
            }
        }
        
        [
        Browsable(false)
        ]
        public bool HasEnabled
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.EnabledProperty);
            }
        }
        public void ResetEnabled()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.EnabledProperty);
        }

        public bool ShouldSerializeEnabled()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.EnabledProperty);
        }
        #endregion

        #region EnabledButtons
        /// <summary>
        /// Gets or sets a value indicating whether the buttons in the node are enabled.
        /// </summary>
        [
        Description("Specifies if the buttons in the node are enabled."),
        Category("Appearance"),
        Localizable(true)
        ]
        public bool EnabledButtons
        {
            get
            {
                return (bool)GetValue(TreeNodeAdvStyleInfoStore.EnabledButtonsProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.EnabledButtonsProperty, value);
            }
        }

        public void ResetEnabledButtons()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.EnabledButtonsProperty);
        }

        public bool ShouldSerializeEnabledButtons()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.EnabledButtonsProperty);
        }
        #endregion

        #region EnsureDefaultOptionedChild
        /// <summary>
        /// Gets or sets a value indicating whetherIndicates whether the first child should be marked as <see cref="P:Optioned"/> if none of the other children is Optioned in a parent node.
        /// </summary>
        /// <value>True to ensure a default optioned child. False otherwise.</value>
        [
        Description("Specifies if atleast one child of the parent node should be Optioned at all times."),
        Category("Behavior"),
        DefaultValue(true)
        ]
        public bool EnsureDefaultOptionedChild
        {
            get
            {
                return (bool)GetValue(TreeNodeAdvStyleInfoStore.EnsureDefaultOptionedChildProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.EnsureDefaultOptionedChildProperty, value);
            }
        }
  
        [
        Browsable(false)
        ]
        public bool HasEnsureDefaultOptinedChild
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.EnsureDefaultOptionedChildProperty);
            }
        }

        public void ResetEnsureDefaultOptinedChild()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.EnsureDefaultOptionedChildProperty);
        }

        public bool ShouldSerializeEnsureDefaultOptinedChild()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.EnsureDefaultOptionedChildProperty);
        }
        #endregion

        #region CheckState
        /// <summary>
        /// Gets or sets the checkState of the node.
        /// </summary>
        [
        Description("Indicates the checkState of the node."),
        Category("Appearance")
        ]
        public CheckState CheckState
        {
            get
            {
                return (CheckState)GetValue(TreeNodeAdvStyleInfoStore.CheckStateProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.CheckStateProperty, value);
            }
        }

        public void ResetCheckState()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.CheckStateProperty);
        }

        public bool ShouldSerializeCheckState()
        {
            return HasValue(TreeNodeAdvStyleInfoStore.CheckStateProperty);
        }

        [
        Browsable(false)
        ]
        public bool HasCheckState
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.CheckStateProperty);
            }
        }
        #endregion

        #region LeftImage
        /// <summary>
        /// Gets or sets the image that will be drawn on the left of the node`s text.
        /// This value has higher priority in comparing to ImageList indexes.
        /// </summary>
        [
        Description("Image that will be drawn on the left of the node`s text."),
        Category("Images"),
        Localizable(true),
        DefaultValue(null)
        ]
        public Image LeftImage
        {
            get
            {
                return (Image)GetValue(TreeNodeAdvStyleInfoStore.LeftImageProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.LeftImageProperty, value);
            }
        }

        /// <summary>
        /// Reset value to default value
        /// </summary>
        public void ResetLeftImage()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.LeftImageProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize our property value
        /// </summary>
        /// <returns>Returns bool value</returns>
        public bool ShouldSerializeLeftImage()
        {
            return HasLeftImage;
        }

        [
        Browsable(false)
        ]
        public bool HasLeftImage
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.LeftImageProperty);
            }
        }
        #endregion

        #region RightImage
        /// <summary>
        /// Gets or sets the image that will be drawn on the right of the node`s text.
        /// This value has higher priority in comparing to ImageList indexes.
        /// </summary>
        [
        Description("Image that will be drawn on the right of the node`s text."),
        Category("Images"),
        Localizable(true),
        DefaultValue(null)
        ]
        public Image RightImage
        {
            get
            {
                return (Image)GetValue(TreeNodeAdvStyleInfoStore.RightImageProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.RightImageProperty, value);
            }
        }

        /// <summary>
        /// Reset value to default value
        /// </summary>
        public void ResetRightImage()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.RightImageProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize our property value
        /// </summary>
        /// <returns>Returns bool value</returns>
        public bool ShouldSerializeRightImage()
        {
            return HasRightImage;
        }

        /// <summary>
        /// Gets a value indicating whether RightImage property has or not.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasRightImage
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.RightImageProperty);
            }
        }
        #endregion

        #region OpenImage
        /// <summary>
        /// Gets or sets the image that will be shown where the node is expanded.
        /// This value has higher priority in comparing to ImageList indexes.
        /// </summary>
        [
        Description("Gets or sets the image that will be shown where the node is expanded."),
        Category("Images"),
        Localizable(true),
        DefaultValue(null)
        ]
        public Image OpenImage
        {
            get
            {
                return (Image)GetValue(TreeNodeAdvStyleInfoStore.OpenImageProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.OpenImageProperty, value);
            }
        }

        /// <summary>
        /// Reset value to default value
        /// </summary>
        public void ResetOpenImage()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.OpenImageProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize our property value
        /// </summary>
        /// <returns>Returns bool value</returns>
        public bool ShouldSerializeOpenImage()
        {
            return HasOpenImage;
        }

        /// <summary>
        /// Gets a value indicating whether OpenImage property has or not
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasOpenImage
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.OpenImageProperty);
            }
        }
        #endregion

        #region ClosedImage
        /// <summary>
        /// Gets or sets the image that will be shown where the node is collapsed.
        /// This value has higher priority in comparing to ImageList indexes.
        /// </summary>
        [
        Description("Gets or sets the image that will be shown where the node is collapsed."),
        Category("Images"),
        Localizable(true),
        DefaultValue(null)
        ]
        public Image ClosedImage
        {
            get
            {
                return (Image)GetValue(TreeNodeAdvStyleInfoStore.ClosedImageProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.ClosedImageProperty, value);
            }
        }

        /// <summary>
        /// Reset value to default value
        /// </summary>
        public void ResetClosedImage()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.ClosedImageProperty);
        }

        /// <summary>
        /// Gets a value indicating whether we serialize closedImage property value
        /// </summary>
        /// <returns>Returns bool value</returns>
        public bool ShouldSerializeClosedImage()
        {
            return HasClosedImage;
        }

        [
        Browsable(false)
        ]
        public bool HasClosedImage
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.ClosedImageProperty);
            }
        }
        #endregion

        #region NoChildrenImage
        /// <summary>
        /// Gets or sets the image that will be shown where the node has no children.
        /// This value has higher priority in comparing to ImageList indexes.
        /// </summary>
        [
        Description("Gets or sets the image that will be shown where the node has no children."),
        Category("Images"),
        Localizable(true),
        DefaultValue(null)
        ]
        public Image NoChildrenImage
        {
            get
            {
                return (Image)GetValue(TreeNodeAdvStyleInfoStore.NoChildrenImageProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.NoChildrenImageProperty, value);
            }
        }

        /// <summary>
        /// Reset value to default value
        /// </summary>
        public void ResetNoChildrenImage()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.NoChildrenImageProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize our property value
        /// </summary>
        /// <returns>Returns bool value</returns>
        public bool ShouldSerializeNoChildrenImage()
        {
            return HasNoChildrenImage;
        }

        [
        Browsable(false)
        ]
        public bool HasNoChildrenImage
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.NoChildrenImageProperty);
            }
        }
        #endregion

        #region ExpandedImage
        /// <summary>
        /// Gets or sets the image for state button where the node is expanded.
        /// This value has higher priority in comparing to ImageList indexes.
        /// </summary>
        [
        Description("Gets or sets the image for state button where the node is expanded."),
        Category("Images"),
        Localizable(true),
        DefaultValue(null)
        ]
        public Image ExpandedImage
        {
            get
            {
                return (Image)GetValue(TreeNodeAdvStyleInfoStore.ExpandedImageProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.ExpandedImageProperty, value);
            }
        }

        /// <summary>
        /// Reset value to default value
        /// </summary>
        public void ResetExpandedImage()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.ExpandedImageProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize our property value
        /// </summary>
        /// <returns>Returns bool value</returns>
        public bool ShouldSerializeExpandedImage()
        {
            return HasExpandedImage;
        }

        [
        Browsable(false)
        ]
        public bool HasExpandedImage
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.ExpandedImageProperty);
            }
        }
        #endregion

        #region CollapsedImage
        /// <summary>
        /// Gets or sets the image for state button where the node is collapsed.
        /// This value has higher priority in comparing to ImageList indexes.
        /// </summary>
        [
        Description("Gets or sets the image for state button where the node is collapsed."),
        Category("Images"),
        Localizable(true),
        DefaultValue(null)
        ]
        public Image CollapsedImage
        {
            get
            {
                return (Image)GetValue(TreeNodeAdvStyleInfoStore.CollapsedImageProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.CollapsedImageProperty, value);
            }
        }

        /// <summary>
        /// Reset value to default value
        /// </summary>
        public void ResetCollapsedImage()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.CollapsedImageProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize our property value
        /// </summary>
        /// <returns>Returns bool value</returns>
        public bool ShouldSerializeCollapsedImage()
        {
            return HasCollapsedImage;
        }

        [
        Browsable(false)
        ]
        public bool HasCollapsedImage
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.CollapsedImageProperty);
            }
        }
        #endregion

        #region LeftImagePadding

        [
        Description("Gets or sets the padding of left image for the node."),
        Category("Images"),
        Localizable(true),
        DefaultValue(0)
        ]
        public int LeftImagePadding
        {
            get
            {
                if (!HasLeftImagePadding)
                {
                    return 0;
                }

                return (int)GetValue(TreeNodeAdvStyleInfoStore.LeftImagePaddingProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.LeftImagePaddingProperty, value);
            }
        }

        /// <summary>
        /// Reset property LeftImagePadding value to default value
        /// </summary>
        public void ResetLeftImagePadding()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.LeftImagePaddingProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize LeftImagePadding property value.
        /// </summary>
        /// <returns>Returns bool value</returns>
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
                return HasValue(TreeNodeAdvStyleInfoStore.LeftImagePaddingProperty);
            }
        }
        #endregion

        #region RightImagePadding
  
        [
        Description("Gets or sets the padding of right image for the node."),
        Category("Images"),
        Localizable(true),
        DefaultValue(0)
        ]
        public int RightImagePadding
        {
            get
            {
                if (!HasRightImagePadding)
                {
                    return 0;
                }

                return (int)GetValue(TreeNodeAdvStyleInfoStore.RightImagePaddingProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.RightImagePaddingProperty, value);
            }
        }

        /// <summary>
        /// Reset property RightImagePadding value to default value
        /// </summary>
        public void ResetRightImagePadding()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.RightImagePaddingProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize RightImagePadding property value.
        /// </summary>
        /// <returns>Returns bool value</returns>
        public bool ShouldSerializeRightImagePadding()
        {
            return HasRightImagePadding;
        }

        /// <summary>
        /// Gets a value indicating whether RightImagePadding property value has set or not.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasRightImagePadding
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.RightImagePaddingProperty);
            }
        }
        #endregion

        #region LeftStateImagePadding

        [
        Description("Gets or sets the left side padding of state image for the node."),
        Category("Images"),
        Localizable(true),
        DefaultValue(0)
        ]
        public int LeftStateImagePadding
        {
            get
            {
                if (!HasLeftStateImagePadding)
                {
                    return 0;
                }

                return (int)GetValue(TreeNodeAdvStyleInfoStore.LeftStateImagePaddingProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.LeftStateImagePaddingProperty, value);
            }
        }

        /// <summary>
        /// Reset property LeftStateImagePadding value to default value
        /// </summary>
        public void ResetLeftStateImagePadding()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.LeftStateImagePaddingProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize LeftStateImagePadding property value.
        /// </summary>
        /// <returns>Returns bool value</returns>
        public bool ShouldSerializeLeftStateImagePadding()
        {
            return HasLeftStateImagePadding;
        }

        /// <summary>
        /// Gets a value indicating whether LeftStateImagePadding property value has set or not.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasLeftStateImagePadding
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.LeftStateImagePaddingProperty);
            }
        }
        #endregion

        #region RightStateImagePadding

        [
        Description("Gets or sets the right side padding of state image for the node."),
        Category("Images"),
        Localizable(true),
        DefaultValue(0)
        ]
        public int RightStateImagePadding
        {
            get
            {
                if (!HasRightStateImagePadding)
                {
                    return 0;
                }

                return (int)GetValue(TreeNodeAdvStyleInfoStore.RightStateImagePaddingProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.RightStateImagePaddingProperty, value);
            }
        }

        /// <summary>
        /// Reset property RightStateImagePadding value to default value
        /// </summary>
        public void ResetRightStateImagePadding()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.RightStateImagePaddingProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize RightStateImagePadding property value.
        /// </summary>
        /// <returns>Returns bool value</returns>
        public bool ShouldSerializeRightStateImagePadding()
        {
            return HasRightStateImagePadding;
        }

        /// <summary>
        /// Gets a value indicating whether RightStateImagePadding property value has set or not.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasRightStateImagePadding
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.RightStateImagePaddingProperty);
            }
        }
        #endregion

        #region Multiline
        /// <summary>
        /// Gets or sets a value indicating whether multiline text or single line..
        /// </summary>
        [
        Description("Gets or sets  a value indicating whether multiline text or single line."),
        Category("Appearance"),
        DefaultValue(false)
        ]
        public bool Multiline
        {
            get
            {
                return (bool)GetValue(TreeNodeAdvStyleInfoStore.MultilineProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.MultilineProperty, value);
            }
        }

        /// <summary>
        /// Reset property Multiline value to default value
        /// </summary>
        public void ResetMultiline()
        {
            ResetValue(TreeNodeAdvStyleInfoStore.MultilineProperty);
        }

        /// <summary>
        /// Indicate should or not we serialize Multiline property value.
        /// </summary>
        /// <returns>Returns bool value</returns>
        internal bool ShouldSerializeMultiline()
        {
            return HasMultiline;
        }

        /// <summary>
        /// Gets a value indicating whether Multiline property value has set or not.
        /// </summary>
        [
        Browsable(false)
        ]
        public bool HasMultiline
        {
            get
            {
                return HasValue(TreeNodeAdvStyleInfoStore.MultilineProperty);
            }
        }
        #endregion
    }
}