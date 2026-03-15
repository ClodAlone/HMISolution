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
using System.Globalization;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.Serialization;
using System.Windows.Forms;

using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Summary description for TreeViewAdvStyleInfoStore.
	/// </summary>
	[Serializable]
	[Syncfusion.Documentation.DocumentationExclude()]
	public class TreeNodeAdvStyleInfoStore : StyleInfoStore
	{

		static StaticData sd = new StaticData(typeof(TreeNodeAdvStyleInfoStore), typeof(TreeNodeAdvStyleInfo), false);

		// Objects - more frequently used fields should come first because of memory performance reasons
		// Data will be allocated per style object on a slot basis, 4 object references at a time.
		internal readonly static StyleInfoProperty FontProperty = sd.CreateStyleInfoProperty(typeof(Font), "Font");
		internal readonly static StyleInfoProperty TextColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "TextColor");
		internal readonly static StyleInfoProperty BackgroundProperty = sd.CreateStyleInfoProperty(typeof(BrushInfo), "Background");
		internal readonly static StyleInfoProperty TextProperty = sd.CreateStyleInfoProperty(typeof(string), "Text");
		internal readonly static StyleInfoProperty HelpTextProperty = sd.CreateStyleInfoProperty(typeof(string), "HelpText");
		internal readonly static StyleInfoProperty DisplayMemberProperty = sd.CreateStyleInfoProperty(typeof(string), "DisplayMember");
		internal readonly static StyleInfoProperty HeightProperty = sd.CreateStyleInfoProperty(typeof(int), "Height");
		internal readonly static StyleInfoProperty ShowCheckBoxProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowCheckBox");
		internal readonly static StyleInfoProperty CheckColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "ActiveCheckColor"); 
        internal readonly static StyleInfoProperty IntermediateCheckColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "InActiveCheckColor");
        internal readonly static StyleInfoProperty CheckBoxBackGroundProperty = sd.CreateStyleInfoProperty(typeof(Brush), "CheckBoxBackGround");
        internal readonly static StyleInfoProperty IntermediateCheckBoxBackgroundProperty = sd.CreateStyleInfoProperty(typeof(Brush), "IntermediateCheckBoxBackground");
        internal readonly static StyleInfoProperty SelectedOptionButtonColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "SelectedOptionButtonColor"); 
        internal readonly static StyleInfoProperty OptionButtonColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "OptionButtonColor"); 

        internal readonly static StyleInfoProperty TagProperty = sd.CreateStyleInfoProperty(typeof(object), "Tag");
		internal readonly static StyleInfoProperty ShowPlusMinusProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowPlusMinus");
		internal readonly static StyleInfoProperty ShowOptionButtonProperty = sd.CreateStyleInfoProperty(typeof(bool), "ShowOptionButton");
		internal readonly static StyleInfoProperty LeftImageIndicesProperty = sd.CreateStyleInfoProperty(typeof(int[]), "LeftImageIndices");
		internal readonly static StyleInfoProperty RightImageIndicesProperty = sd.CreateStyleInfoProperty(typeof(int[]), "RightImageIndices");
		internal readonly static StyleInfoProperty NoChildrenImgIndexProperty = sd.CreateStyleInfoProperty(typeof(int), "NoChildrenImgIndex");
		internal readonly static StyleInfoProperty ClosedImgIndexProperty = sd.CreateStyleInfoProperty(typeof(int), "ClosedImgIndex");
		internal readonly static StyleInfoProperty OpenImgIndexProperty = sd.CreateStyleInfoProperty(typeof(int), "OpenImgIndex");
		internal readonly static StyleInfoProperty ThemesEnabledProperty = sd.CreateStyleInfoProperty(typeof(bool), "ThemesEnabled");
		internal readonly static StyleInfoProperty InteractiveCheckBoxProperty = sd.CreateStyleInfoProperty(typeof(bool), "InteractiveCheckBox");
		internal readonly static StyleInfoProperty SortTypeProperty = sd.CreateStyleInfoProperty(typeof(TreeNodeAdvSortType), "SortType");
		internal readonly static StyleInfoProperty SortOrderProperty = sd.CreateStyleInfoProperty(typeof(SortOrder), "SortOrder");
		internal readonly static StyleInfoProperty ComparerProperty = sd.CreateStyleInfoProperty(typeof(IComparer), "Comparer");
		internal readonly static StyleInfoProperty CompareOptionsProperty = sd.CreateStyleInfoProperty(typeof(CompareOptions), "CompareOptions");
		internal readonly static StyleInfoProperty CheckStateProperty = sd.CreateStyleInfoProperty(typeof(CheckState), "CheckState");
		internal readonly static StyleInfoProperty BaseStyleProperty = sd.CreateStyleInfoProperty(typeof(string), "BaseStyle");
		internal readonly static StyleInfoProperty EnabledProperty = sd.CreateStyleInfoProperty(typeof(bool), "Enabled");
		internal readonly static StyleInfoProperty EnabledButtonsProperty = sd.CreateStyleInfoProperty(typeof(bool), "EnabledButtons");
		internal readonly static StyleInfoProperty EnsureDefaultOptionedChildProperty = sd.CreateStyleInfoProperty(typeof(bool), "EnsureDefaultOptionedChild");
		internal readonly static StyleInfoProperty CultureProperty = sd.CreateStyleInfoProperty(typeof(CultureInfo), "Culture");

		protected override StaticData StaticDataStore
		{
			get { return sd; }
		}

		public override object Clone()
		{
			StyleInfoStore target = new TreeNodeAdvStyleInfoStore();
			CopyTo(target);
			return target;
		}

		public TreeNodeAdvStyleInfoStore():base()
		{}

		protected TreeNodeAdvStyleInfoStore(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			//			TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);

			if (sd.IsEmpty)
				new TreeNodeAdvStyleInfo();
		}

	}

	internal class TreeViewAdvStyleInfoIdentity: StyleInfoIdentityBase
	{
		TreeViewAdv _tree;
		TreeNodeAdv _node;

		/// <override/>
		public override void Dispose()
		{
			_tree = null;
			_node = null;
			GC.SuppressFinalize(this);
		}

		public TreeViewAdvStyleInfoIdentity(TreeViewAdv tree)
		{
			this._tree = tree;
		}
		/// <summary>
		/// Call this constructor only to initialize ChildStyles. Assumption is made in GetBaseStyles
		/// that this is called only for ChildStyles.
		/// </summary>
		/// <param name="node"></param>
		public TreeViewAdvStyleInfoIdentity(TreeNodeAdv node)
		{
			this._node = node;
		}

		public TreeViewAdv TreeView
		{
			get 
			{
				if(this._tree != null)
					return _tree; 
				else
					return this._node.TreeView;
			}
		}

		public TreeNodeAdv Node
		{
			get{return this._node;}
		}

		/// <override/>
		public override IStyleInfo[] GetBaseStyles(IStyleInfo thisStyleInfo)
		{
			TreeNodeAdvStyleInfo treeNodeAdvStyleInfo = (TreeNodeAdvStyleInfo) thisStyleInfo;
			
			//IStyleInfo styleInfo = null;
			ArrayList styles = new ArrayList();
			TreeViewAdv tree = this.TreeView;

			if (tree != null)
			{
				// Style's base style
				if (treeNodeAdvStyleInfo.HasBaseStyle
					&& tree.BaseStyles.Contains(treeNodeAdvStyleInfo.BaseStyle))
					TreeNodeAdvStyleInfoIdentity.AddStyleAndItsBases
						(tree, styles, tree.BaseStyles[treeNodeAdvStyleInfo.BaseStyle] as TreeNodeAdvStyleInfo);

				if(this.Node != null && this.Node.TreeView == tree)
				{
					string nodeLevelStyle = TreeViewAdv.NodeLevelStyleBaseName;
					nodeLevelStyle += (this.Node.Level + 1).ToString();

					// Need to include this for ChildStyles
					// NodeLevel Style:
					if(tree.BaseStyles.Contains(nodeLevelStyle))
						TreeNodeAdvStyleInfoIdentity.AddStyleAndItsBases
							(tree, styles, tree.BaseStyles[nodeLevelStyle] as TreeNodeAdvStyleInfo);
				}
				// Standard style
				if(tree.BaseStyles.Contains(TreeViewAdv.DefaultBaseStyleName))
					TreeNodeAdvStyleInfoIdentity.AddStyleAndItsBases
						(tree, styles, tree.BaseStyles[TreeViewAdv.DefaultBaseStyleName] as TreeNodeAdvStyleInfo);

				// Add tree-bound style
				styles.Add(tree.BoundStyle);
			}

			if (styles.Count > 0)
			{
				IStyleInfo[] aStyles = new IStyleInfo[styles.Count];

				for(int i = 0; i < styles.Count; i++)
				{
					aStyles[i] = styles[i] as IStyleInfo;
				}

				return aStyles;
			}
			else
				return new IStyleInfo[0];
		}

		/// <override/>
		public override string ToString()
		{
			return base.ToString();
		}
		
		/// <override/>
		public override void OnStyleChanged(StyleInfoBase style, StyleInfoProperty sip)
		{
			//TraceUtil.TraceCurrentMethodInfo(sip.PropertyName, sip.FormatValue(style.GetValue(sip)));
		}
	}

	internal class TreeNodeAdvStyleInfoIdentity: StyleInfoIdentityBase
	{
		TreeNodeAdv treeNode;

		/// <override/>
		public override void Dispose()
		{
			treeNode = null;
			GC.SuppressFinalize(this);
		}

		public TreeNodeAdvStyleInfoIdentity(TreeNodeAdv treeNode)
		{
			this.treeNode = treeNode;
		}

		public TreeNodeAdv TreeNode
		{
			get { return treeNode; }
		}

		/// <override/>
		public override IStyleInfo[] GetBaseStyles(IStyleInfo thisStyleInfo)
		{
			TreeNodeAdvStyleInfo treeNodeAdvStyleInfo = (TreeNodeAdvStyleInfo) thisStyleInfo;
			
			//IStyleInfo styleInfo = null;
			ArrayList styles = new ArrayList();
			TreeViewAdv tree = treeNode.TreeView;

			if (tree != null)
			{
				string levelStyle = TreeViewAdv.NodeLevelStyleBaseName + treeNode.Level.ToString();

				// Node's base style
				if (treeNodeAdvStyleInfo.HasBaseStyle
						&& tree.BaseStyles.Contains(treeNodeAdvStyleInfo.BaseStyle))
					AddStyleAndItsBases(tree, styles, tree.BaseStyles[treeNodeAdvStyleInfo.BaseStyle] as TreeNodeAdvStyleInfo);

				// Parent's ChildStyle
				if(treeNode.Parent != null)
					AddStyleAndItsBases(tree, styles, treeNode.Parent.ChildStyle);

				// Node level style
				if(tree.BaseStyles.Contains(levelStyle))
					AddStyleAndItsBases(tree, styles, tree.BaseStyles[levelStyle] as TreeNodeAdvStyleInfo);

				// Standard style
				if(tree.BaseStyles.Contains(TreeViewAdv.DefaultBaseStyleName))
					AddStyleAndItsBases(tree, styles, tree.BaseStyles[TreeViewAdv.DefaultBaseStyleName] as TreeNodeAdvStyleInfo);
				
				// Add tree-bound style
				styles.Add(tree.BoundStyle);
			}

			if (styles.Count > 0)
			{
				IStyleInfo[] aStyles = new IStyleInfo[styles.Count];
				for(int i = 0; i < styles.Count; i++)
				{
					aStyles[i] = styles[i] as IStyleInfo;
				}

				return aStyles;
			}
			else
				return new IStyleInfo[0];
		}

		// Recursively add style and it's base styles
		internal static void AddStyleAndItsBases(TreeViewAdv tree, ArrayList styles, TreeNodeAdvStyleInfo style)
		{
			if(styles.Count < 16)
			{
				styles.Add(style);
				if(style.HasBaseStyle
					&& tree.BaseStyles.Contains(style.BaseStyle))
				{
					TreeNodeAdvStyleInfo baseStyle = tree.BaseStyles[style.BaseStyle] as TreeNodeAdvStyleInfo;
					AddStyleAndItsBases(tree, styles, baseStyle);
				}
			}
		}

		/// <override/>
		public override string ToString()
		{
			return base.ToString();
		}
		
		/// <override/>
		public override void OnStyleChanged(StyleInfoBase style, StyleInfoProperty sip)
		{
			//TraceUtil.TraceCurrentMethodInfo(sip.PropertyName, sip.FormatValue(style.GetValue(sip)));
		}
	}
	/// <summary>
	/// Contains appearance and behavior information regarding the <see cref="TreeNodeAdv"/>s.
	/// </summary>
	[TypeConverter(typeof(TreeNodeAdvStyleInfoConverter))]
	public class TreeNodeAdvStyleInfo : StyleInfoBase
	{
		// Static Fields
		private static TreeNodeAdvStyleInfo defaultStyle = null;

		/// <summary>
		/// An empty style object.
		/// </summary>
		public static readonly TreeNodeAdvStyleInfo Empty = new TreeNodeAdvStyleInfo();

		// Constructors
		static TreeNodeAdvStyleInfo()
		{
		}

		/// <summary>
		/// Overloaded. Initalizes a new style object.
		/// </summary>
		[DebuggerStepThrough()] public TreeNodeAdvStyleInfo()
			: base(new TreeNodeAdvStyleInfoStore())
		{
		}

		/// <summary>
		/// Initalizes a new style object and copies all data from an existing style object.
		/// </summary>
		/// <param name="style">The style object that contains the original data.</param>
		[DebuggerStepThrough()] public TreeNodeAdvStyleInfo(TreeNodeAdvStyleInfo style)
			: base(style.Store)
		{
		}

		/// <summary>
		/// Initalizes a new style object and associates it with an existing <see cref="TreeNodeAdvStyleInfoStore"/>.
		/// </summary>
		/// <param name="store">A <see cref="TreeNodeAdvStyleInfoStore"/> that holds data for this <see cref="TreeNodeAdvStyleInfo"/>.
		/// All changes in this style object will be saved in the <see cref="TreeNodeAdvStyleInfoStore"/> object.</param>
		[DebuggerStepThrough()] public TreeNodeAdvStyleInfo(TreeNodeAdvStyleInfoStore store)
			: base(store)
		{
		}

		/// <summary>
		/// Initalizes a new style object and associates it with an existing <see cref="TreeNodeAdvStyleInfoIdentity"/>.
		/// </summary>
		/// <param name="identity">A <see cref="TreeNodeAdvStyleInfoIdentity"/> that holds the indentity for this <see cref="TreeNodeAdvStyleInfo"/>.
		/// </param>
		[DebuggerStepThrough()] public TreeNodeAdvStyleInfo(StyleInfoIdentityBase identity)
			: base(identity, new TreeNodeAdvStyleInfoStore())
		{
		}

		/// <summary>
		/// Initalizes a new style object and associates it with an existing <see cref="TreeNodeAdvStyleInfoIdentity"/>.
		/// </summary>
		/// <param name="identity">A <see cref="TreeNodeAdvStyleInfoIdentity"/> that holds the indentity for this <see cref="TreeNodeAdvStyleInfo"/>.
		/// </param>
		/// <param name="store">A <see cref="TreeNodeAdvStyleInfoStore"/> that holds data for this <see cref="TreeNodeAdvStyleInfo"/>.
		/// All changes in this style object will be saved in the <see cref="TreeNodeAdvStyleInfoStore"/> object.
		/// </param>
		[DebuggerStepThrough()] public TreeNodeAdvStyleInfo(StyleInfoIdentityBase identity, TreeNodeAdvStyleInfoStore store)
			: base(identity, store)
		{
		}

		/// <summary>
		/// Holds identity information such as TreeNode for the current <see cref="TreeNodeAdvStyleInfo"/>.
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
		/// Returns the <see cref="TreeNodeAdvStyleInfoStore"/> object that holds all the data for this style object.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new TreeNodeAdvStyleInfoStore Store
		{
			get { return (TreeNodeAdvStyleInfoStore) base.Store; }
		}

		/// <override/>
		[DebuggerStepThrough()] public override StyleInfoSubObjectIdentity CreateSubObjectIdentity(StyleInfoProperty sip)
		{
			throw new NotSupportedException();
			//return new TreeNodeAdvStyleInfoSubObjectIdentity(this, sip);
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
		[DebuggerStepThrough()] public TreeNodeAdvStyleInfo GetOffLineCopy()
		{
			return new TreeNodeAdvStyleInfo(((TreeNodeAdvStyleInfoIdentity) Identity).MakeOfflineIdentity(), (TreeNodeAdvStyleInfoStore) Store.Clone());
		}

		TreeNodeAdvStyleInfoCustomPropertiesCollection cpl = null;

		/// <summary>
		/// Returns a collection of custom property objects that have 
		/// at least one initialized value. The primary purpose of this 
		/// collection is to support design-time code serialization of
		/// custom properties.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TreeNodeAdvStyleInfoCustomPropertiesCollection CustomProperties 
		{
			get 
			{
				if (cpl == null)
					cpl = new TreeNodeAdvStyleInfoCustomPropertiesCollection(this);
				return cpl;
			}
		}
		bool ShouldSerializeCustomProperties()
		{
			return CustomProperties.Count > 0;
		}
#endif

		/// <override/>
		protected override void OnStyleChanged(StyleInfoProperty sip)
		{
			//			cpl = null;
			base.OnStyleChanged(sip);
		}

		// Default

		/// <summary>
		/// Returns a <see cref="TreeNodeAdvStyleInfo"/> with default settings.
		/// </summary>
		public static TreeNodeAdvStyleInfo Default
		{
			get
			{
				if (TreeNodeAdvStyleInfo.defaultStyle == null)
				{
					defaultStyle = new TreeNodeAdvStyleInfo();
					defaultStyle.Comparer = null;
					defaultStyle.Text = "";
					defaultStyle.HelpText = "";
					defaultStyle.DisplayMember = "";
					defaultStyle.BaseStyle = String.Empty;
					defaultStyle.Tag = null;
					defaultStyle.LeftImageIndices = new int[0];
					defaultStyle.RightImageIndices = new int[0];
					defaultStyle.NoChildrenImgIndex = 0;
					defaultStyle.ClosedImgIndex = 1;
					defaultStyle.OpenImgIndex = 2;
					defaultStyle.Font = Syncfusion.Drawing.FontUtil.CreateFont("Verdana",8);
					defaultStyle.Background = new BrushInfo(Color.Transparent);
					defaultStyle.TextColor = SystemColors.WindowText;
					defaultStyle.Height = 16;
					defaultStyle.ShowCheckBox = false;
                    defaultStyle.CheckColor = SystemColors.ControlText;
                    defaultStyle.IntermediateCheckColor  = SystemColors.ControlDark;
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
				}

				return TreeNodeAdvStyleInfo.defaultStyle;
			}
		}

		/// <override/>
		protected override StyleInfoBase GetDefaultStyle()
		{
			return Default;
		}

		/// <summary>
		/// Gets / sets the font of the node.
		/// </summary>
		[Description("The font of the node.")]
		[Category("Appearance")]
		[Localizable(true)]
		public virtual Font Font
		{
			get 
			{
				return ((Font) GetValue(TreeNodeAdvStyleInfoStore.FontProperty)).Clone() as Font;
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
		
		internal bool ShouldSerializeFont()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.FontProperty);
		}
		
		public bool HasEnabled
		{
			get
			{
				return HasValue(TreeNodeAdvStyleInfoStore.EnabledProperty);
			}
		}
		public bool HasEnsureDefaultOptinedChild
		{
			get
			{
				return HasValue(TreeNodeAdvStyleInfoStore.EnsureDefaultOptionedChildProperty);
			}
		}
		public virtual bool HasFont
		{
			get
			{
				return HasValue(TreeNodeAdvStyleInfoStore.FontProperty);
			}
		}
		public virtual bool HasHeight
		{
			get
			{
				return HasValue(TreeNodeAdvStyleInfoStore.HeightProperty);
			}
		}
		/// <summary>
		/// Gets / sets the Color of the text.
		/// </summary>
		[Description("The Color of the text.")]
		[Category("Appearance")]
		public virtual Color TextColor
		{
			get 
			{
				return (Color) GetValue(TreeNodeAdvStyleInfoStore.TextColorProperty);
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
		
		internal bool ShouldSerializeTextColor()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.TextColorProperty);
		}
				
		public virtual bool HasTextColor
		{
			get
			{
				return HasValue(TreeNodeAdvStyleInfoStore.TextColorProperty);
			}
		}
		public bool HasBackground
		{
			get
			{
				return HasValue(TreeNodeAdvStyleInfoStore.BackgroundProperty);
			}
		}
		public bool HasShowCheckBox
		{
			get
			{
				return HasValue(TreeNodeAdvStyleInfoStore.ShowCheckBoxProperty);
			}
		}
		public bool HasShowOptionButton
		{
			get
			{
				return HasValue(TreeNodeAdvStyleInfoStore.ShowOptionButtonProperty);
			}
		}
		public bool HasShowPlusMinus
		{
			get
			{
				return HasValue(TreeNodeAdvStyleInfoStore.ShowPlusMinusProperty);
			}
		}

		/// <summary>
		/// Gets / sets the base style for the node from which to inherit.
		/// </summary>
		/// <remarks>The specified base style should be available in the <see cref="TreeViewAdv.BaseStyles"/>
		/// collection.</remarks>
		[Description("The base style for the node")]
		[Category("Appearance - Inherited")]
		public string BaseStyle
		{
			get 
			{
				return (string) GetValue(TreeNodeAdvStyleInfoStore.BaseStyleProperty);
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
		
		internal bool ShouldSerializeBaseStyle()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.BaseStyleProperty);
		}
				
		public bool HasBaseStyle
		{
			get
			{
				return HasValue(TreeNodeAdvStyleInfoStore.BaseStyleProperty);
			}
		}

		/// <summary>
		/// Gets / sets the background of the node.
		/// </summary>
		[Description("The background of the node.")]
		[Category("Appearance")]
		public BrushInfo Background
		{
			get 
			{
				return (BrushInfo) GetValue(TreeNodeAdvStyleInfoStore.BackgroundProperty);
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
		
		internal bool ShouldSerializeBackground()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.BackgroundProperty);
		}
				
		/// <summary>
		/// Gets / sets the text of the node.
		/// </summary>
		[Description("The text of the node.")]
		[Category("Appearance")]
		[Browsable(false)]
		[Localizable(true)]
		public string Text
		{
			get 
			{
				return (string) GetValue(TreeNodeAdvStyleInfoStore.TextProperty);
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
		
		internal bool ShouldSerializeText()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.TextProperty);
		}



		/// <summary>
		/// Gets / sets the help text of the node.
		/// </summary>
		[Description("The help text of the node.")]
		[Category("Appearance")]
		[Localizable(true)]
		public string HelpText
		{
			get 
			{
				return (string) GetValue(TreeNodeAdvStyleInfoStore.HelpTextProperty);
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
		
		internal bool ShouldSerializeHelpText()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.HelpTextProperty);
		}

		/// <summary>
		/// Gets / sets the display member of the data bound to the node.
		/// </summary>
		[Description("The display member of the data bound to the node.")]
		[Category("Data")]
		[Localizable(true)]
		public string DisplayMember
		{
			get 
			{
				return (string) GetValue(TreeNodeAdvStyleInfoStore.DisplayMemberProperty);
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
		
		internal bool ShouldSerializeDisplayMember()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.DisplayMemberProperty);
		}
		
		/// <summary>
		/// Gets / sets the height of the node.
		/// </summary>
		[Description("The height of the node.")]
		[Category("Appearance")]
		public virtual int Height
		{
			get 
			{
				return (int) GetValue(TreeNodeAdvStyleInfoStore.HeightProperty);
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
		
		internal bool ShouldSerializeHeight()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.HeightProperty);
		}
        /// <summary>
        /// Indicates the color of check symbol.
        /// </summary>
        [Description("Indicates the color of check symbol.")]
        [Category("Appearance"),
       DefaultValueAttribute(typeof(Color), "ControlText")
        ]
        public Color CheckColor
        {
            get
            {
                return (Color)GetValue(TreeNodeAdvStyleInfoStore.CheckColorProperty );
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.CheckColorProperty , value);
            }
        }
        /// <summary>
        /// Indicates the color of intermediate check symbol.
        /// </summary>
        [Description("Indicates the color of intermediate check symbol.")]
        [Category("Appearance"),
       DefaultValueAttribute(typeof(Color), "ControlDark")
        ]
        public Color  IntermediateCheckColor
        {
            get
            {
                return (Color )GetValue(TreeNodeAdvStyleInfoStore.IntermediateCheckColorProperty );
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.IntermediateCheckColorProperty, value);
            }
        }
        /// <summary>
        /// Indicates the background of checkbox when it is in intermediate state.
        /// </summary>
        [Description("Indicates the background of checkbox when it is in intermediate state.")]
        [Category("Appearance")]
        public Brush  IntermediateCheckBoxBackground
        {
            get
            {
                return (Brush )GetValue(TreeNodeAdvStyleInfoStore.IntermediateCheckBoxBackgroundProperty);
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.IntermediateCheckBoxBackgroundProperty , value);
            }
        }
        /// <summary>
        /// Indicates the background of checkbox .
        /// </summary>
        [Description("Indicates the background of checkbox.")]
        [Category("Appearance")]
        public Brush CheckBoxBackground
        {
            get
            {
                return (Brush)GetValue(TreeNodeAdvStyleInfoStore.CheckBoxBackGroundProperty );
            }
            set
            {
                SetValue(TreeNodeAdvStyleInfoStore.CheckBoxBackGroundProperty , value);
            }
        }
        
        
        /// <summary>
		/// Indicates whether the checkbox of the node is visible.
		/// </summary>
		[Description("Indicates if the checkbox of the node is visible.")]
		[Category("Appearance")]
		public bool ShowCheckBox
		{
			get 
			{
				return (bool) GetValue(TreeNodeAdvStyleInfoStore.ShowCheckBoxProperty);
			}
			set 
			{
				SetValue(TreeNodeAdvStyleInfoStore.ShowCheckBoxProperty, value);
			}
		}
		
		
		public void ResetShowCheckBox()
		{
			ResetValue(TreeNodeAdvStyleInfoStore.ShowCheckBoxProperty);
		}
		
		internal bool ShouldSerializeShowCheckBox()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.ShowCheckBoxProperty);
		}

		/// <summary>
		/// Indicates whether the node will have an interactive checkbox.
		/// </summary>
		[Description("Indicates if the node will have an interactive checkbox.")]
		[Category("Behavior")]
		public bool InteractiveCheckBox
		{
			get 
			{
				return (bool) GetValue(TreeNodeAdvStyleInfoStore.InteractiveCheckBoxProperty);
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
		
		internal bool ShouldSerializeInteractiveCheckBox()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.InteractiveCheckBoxProperty);
		}


		/// <summary>
		/// Gets / sets the tag of the node. Can be used to store additional information for the node.
		/// </summary>
		[Description("The tag of the node. Can be used to store aditional information for the node.")]
        [Category("Data")]
		[Browsable(false)]
		public object Tag
		{
			get 
			{
				return (object) GetValue(TreeNodeAdvStyleInfoStore.TagProperty);
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
		
		internal bool ShouldSerializeTag()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.TagProperty);
		}
				
		public bool HasTag
		{
			get
			{
				return HasValue(TreeNodeAdvStyleInfoStore.TagProperty);
			}
		}

		/// <summary>
		/// Indicates whether the plus/minus of the node is visible.
		/// </summary>
		[Description("Indicates if the plus/minus of the node is visible.")]
		[Category("Appearance")]
		public bool ShowPlusMinus
		{
			get 
			{
				return (bool) GetValue(TreeNodeAdvStyleInfoStore.ShowPlusMinusProperty);
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
		
		internal bool ShouldSerializeShowPlusMinus()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.ShowPlusMinusProperty);
		}
        /// <summary>
        /// Indicates the color of Option button.
        /// </summary>
        [Description("Indicates the color of option button.")]
        [Category("Appearance"),
       DefaultValueAttribute(typeof(Color), "White")
        ]
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
        /// Indicates the color of Selected Option button.
        /// </summary>
        [Description("Indicates the color of Selected option button.")]
        [Category("Appearance"),
       DefaultValueAttribute(typeof(Color), "Black")]
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
		
		/// <summary>
		/// Indicates whether the optionbutton of the node is visible.
		/// </summary>
		[Description("Indicates if the optionbutton of the node is visible.")]
		[Category("Appearance")]
		public bool ShowOptionButton
		{
			get 
			{
				return (bool) GetValue(TreeNodeAdvStyleInfoStore.ShowOptionButtonProperty);
			}
			set 
			{
				SetValue(TreeNodeAdvStyleInfoStore.ShowOptionButtonProperty, value);
			}
		}
		
		
		public void ResetShowOptionButton()
		{
			ResetValue(TreeNodeAdvStyleInfoStore.ShowOptionButtonProperty);
		}
		
		internal bool ShouldSerializeShowOptionButton()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.ShowOptionButtonProperty);
		}
				
		/// <summary>
		/// Gets / sets the image indices of the images to be drawn on the left of the node`s text.
		/// </summary>
		[Description("The imageindex to be drawn on the left of the node`s text.")]
		[Category("Appearance")]
		[Localizable(true)]
//		[Editor(typeof(ImageIndexEditor),typeof(UITypeEditor))]
		public int[] LeftImageIndices
		{
			get 
			{
				return (int[]) GetValue(TreeNodeAdvStyleInfoStore.LeftImageIndicesProperty);
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
		
		internal bool ShouldSerializeLeftImageIndices()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.LeftImageIndicesProperty);
		}
				
		public bool HasLeftImageIndices
		{
			get
			{
				return HasValue(TreeNodeAdvStyleInfoStore.LeftImageIndicesProperty);
			}
		}

		/// <summary>
		/// Gets / sets the image indices of the images  to be drawn on the right of the node`s text.
		/// </summary>
		[Description("The imageindex to be drawn on the right of the node`s text.")]
		[Category("Appearance")]
		[Localizable(true)]
		public int[] RightImageIndices
		{
			get 
			{
				return (int[]) GetValue(TreeNodeAdvStyleInfoStore.RightImageIndicesProperty);
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
		
		internal bool ShouldSerializeRightImageIndices()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.RightImageIndicesProperty);
		}
				
		public bool HasRightImageIndices
		{
			get
			{
				return HasValue(TreeNodeAdvStyleInfoStore.RightImageIndicesProperty);
			}
		}
		public bool HasNoChildrenImgIndex
		{
			get
			{
				return HasValue(TreeNodeAdvStyleInfoStore.NoChildrenImgIndexProperty);
			}
		}
		public bool HasClosedImgIndex
		{
			get
			{
				return HasValue(TreeNodeAdvStyleInfoStore.ClosedImgIndexProperty);
			}
		}
		public bool HasCheckState
		{
			get
			{
				return HasValue(TreeNodeAdvStyleInfoStore.CheckStateProperty);
			}
		}
		public bool HasOpenImgIndex
		{
			get
			{
				return HasValue(TreeNodeAdvStyleInfoStore.OpenImgIndexProperty);
			}
		}
		public bool HasThemesEnabled
		{
			get
			{
				return HasValue(TreeNodeAdvStyleInfoStore.ThemesEnabledProperty);
			}
		}
		public bool HasInteractiveCheckBox
		{
			get
			{
				return HasValue(TreeNodeAdvStyleInfoStore.InteractiveCheckBoxProperty);
			}
		}
		public bool HasSortType
		{
			get
			{
				return HasValue(TreeNodeAdvStyleInfoStore.SortTypeProperty);
			}
		}
		public bool HasSortOrder
		{
			get
			{
				return HasValue(TreeNodeAdvStyleInfoStore.SortOrderProperty);
			}
		}


		/// <summary>
		/// Gets / sets the image index indicating the image in the StateImageList where the node has no children.
		/// </summary>
		[Description("The imageindex indicating the image in the StateImageList where the node has no children.")]
		[Category("Appearance")]
		[Localizable(true)]
		public int NoChildrenImgIndex
		{
			get 
			{
				return (int) GetValue(TreeNodeAdvStyleInfoStore.NoChildrenImgIndexProperty);
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
		
		internal bool ShouldSerializeNoChildrenImgIndex()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.NoChildrenImgIndexProperty);
		}
				

		/// <summary>
		/// Gets / sets the image index in the StateImageList where the node is expanded.
		/// </summary>
		[Description("Indicates the imageindex in the StateImageList where the node is expanded.")]
		[Category("Appearance")]
		[Localizable(true)]
		public int OpenImgIndex
		{
			get 
			{
				return (int) GetValue(TreeNodeAdvStyleInfoStore.OpenImgIndexProperty);
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
		
		internal bool ShouldSerializeOpenImgIndex()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.OpenImgIndexProperty);
		}

		/// <summary>
		/// Gets / sets the image index in the StateImageList where the node is not expanded.
		/// </summary>
		[Description("Indicates the imageindex in the StateImageList where the node is not expanded.")]
		[Category("Appearance")]
		[Localizable(true)]
		public int ClosedImgIndex
		{
			get 
			{
				return (int) GetValue(TreeNodeAdvStyleInfoStore.ClosedImgIndexProperty);
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
		
		internal bool ShouldSerializeClosedImgIndex()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.ClosedImgIndexProperty);
		}
				

				

//		/// <summary>
//		/// The imagelist containing state images of the node.
//		/// </summary>
//		[Description("The imagelist containing state images of the node.")]
//		[Category("Appearance")]
//		public ImageList StateImageList
//		{
//			get 
//			{
//				return (ImageList) GetValue(TreeNodeAdvStyleInfoStore.StateImageListProperty);
//			}
//			set 
//			{
//				SetValue(TreeNodeAdvStyleInfoStore.StateImageListProperty, value);
//			}
//		}
//		
//		
//		public void ResetStateImageList()
//		{
//			ResetValue(TreeNodeAdvStyleInfoStore.StateImageListProperty);
//		}
//		
//		private bool ShouldSerializeStateImageList()
//		{
//			return HasValue(TreeNodeAdvStyleInfoStore.StateImageListProperty);
//		}
//				
//		public bool HasStateImageList
//		{
//			get
//			{
//				return HasValue(TreeNodeAdvStyleInfoStore.StateImageListProperty);
//			}
//		}

		/// <summary>
		/// Indicates whether the node`s controls will be themed.
		/// </summary>
		[Description("Indicates if the node`s controls will be themed.")]
		[Category("Appearance")]
		public bool ThemesEnabled
		{
			get 
			{
				return (bool) GetValue(TreeNodeAdvStyleInfoStore.ThemesEnabledProperty);
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
		
		internal bool ShouldSerializeThemesEnabled()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.ThemesEnabledProperty);
		}
				
		/// <summary>
		/// Gets / sets the sort type of the node.
		/// </summary>
		[Description("Indicates the sort type of the node.")]
		[Category("Sorting")]
		[Localizable(true)]
		public TreeNodeAdvSortType SortType
		{
			get 
			{
				return (TreeNodeAdvSortType) GetValue(TreeNodeAdvStyleInfoStore.SortTypeProperty);
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
		
		internal bool ShouldSerializeSortType()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.SortTypeProperty);
		}
		/// <summary>
		/// Gets / sets the sort order of the node.
		/// </summary>
		[Description("Indicates the sort order of the node.")]
		[Category("Sorting")]
		[Localizable(true)]
		public SortOrder SortOrder
		{
			get 
			{
				return (SortOrder) GetValue(TreeNodeAdvStyleInfoStore.SortOrderProperty);
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
		
		internal bool ShouldSerializeSortOrder()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.SortOrderProperty);
		}

		/// <summary>
		/// Gets / sets the culture of the node used while sorting.
		/// </summary>
		[Description("Indicates the culture of the node used while sorting.")]
		[Category("Sorting")]
		[Localizable(true)]
		public CultureInfo Culture
		{
			get 
			{
				return (CultureInfo) GetValue(TreeNodeAdvStyleInfoStore.CultureProperty);
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
		
		internal bool ShouldSerializeCulture()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.CultureProperty);
		}
				
		/// <summary>
		/// Gets / sets the <see cref="IComparer"/> object that compares two nodes.
		/// </summary>
		[Description("Indicates the IComparer object that compares two nodes.")]
		[Category("Sorting")]
		public IComparer Comparer
		{
			get 
			{
				return (IComparer) GetValue(TreeNodeAdvStyleInfoStore.ComparerProperty);
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
		
		internal bool ShouldSerializeComparer()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.ComparerProperty);
		}
				
		public bool HasComparer
		{
			get
			{
				return HasValue(TreeNodeAdvStyleInfoStore.ComparerProperty);
			}
		}
		public bool HasCompareOptions
		{
			get
			{
				return HasValue(TreeNodeAdvStyleInfoStore.CompareOptionsProperty);
			}
		}

		/// <summary>
		/// Gets / sets the compare options used in the sorting of the node.
		/// </summary>
		[Description("Indicates the compare options used in the sorting of the node.")]
		[Category("Sorting")]
		[Localizable(true)]
		public CompareOptions CompareOptions
		{
			get 
			{
				return (CompareOptions) GetValue(TreeNodeAdvStyleInfoStore.CompareOptionsProperty);
			}
			set 
			{
				SetValue(TreeNodeAdvStyleInfoStore.CompareOptionsProperty, value);
			}
		}
		
		
		public void ResetCompareOptions()
		{
			ResetValue(TreeNodeAdvStyleInfoStore.CompareOptionsProperty);
		}
		
		internal bool ShouldSerializeCompareOptions()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.CompareOptionsProperty);
		}
				
		/// <summary>
		/// Indicates whether the node is enabled.
		/// </summary>
		[Description("Specifies if the node is enabled.")]
		[Category("Appearance")]
		[Localizable(true)]
		public bool Enabled
		{
			get 
			{
				return (bool) GetValue(TreeNodeAdvStyleInfoStore.EnabledProperty);
			}
			set 
			{
				SetValue(TreeNodeAdvStyleInfoStore.EnabledProperty, value);
			}
		}
		public void ResetEnabled()
		{
			ResetValue(TreeNodeAdvStyleInfoStore.EnabledProperty);
		}
		
		internal bool ShouldSerializeEnabled()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.EnabledProperty);
		}
		
		/// <summary>
		/// Indicates whether the buttons in the node are enabled.
		/// </summary>
		[Description("Specifies if the buttons in the node are enabled.")]
		[Category("Appearance")]
		[Localizable(true)]
		public bool EnabledButtons
		{
			get 
			{
				return (bool) GetValue(TreeNodeAdvStyleInfoStore.EnabledButtonsProperty);
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
		
		internal bool ShouldSerializeEnabledButtons()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.EnabledButtonsProperty);
		}

		/// <summary>
		/// Indicates whether the first child should be marked as <see cref="Optioned"/> if none of the other children is Optioned in a parent node.
		/// </summary>
		/// <value>True to ensure a default optioned child. False otherwise.</value>
		[Description("Specifies if atleast one child of the parent node should be Optioned at all times.")]
		[Category("Behavior")]
		public bool EnsureDefaultOptionedChild
		{
			get 
			{
				return (bool) GetValue(TreeNodeAdvStyleInfoStore.EnsureDefaultOptionedChildProperty);
			}
			set 
			{
				SetValue(TreeNodeAdvStyleInfoStore.EnsureDefaultOptionedChildProperty, value);
			}
		}
		public void ResetEnsureDefaultOptinedChild()
		{
			ResetValue(TreeNodeAdvStyleInfoStore.EnsureDefaultOptionedChildProperty);
		}
		
		internal bool ShouldSerializeEnsureDefaultOptinedChild()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.EnsureDefaultOptionedChildProperty);
		}

		/// <summary>
		/// Gets / sets the checkState of the node.
		/// </summary>
		[Description("Indicates the checkState of the node.")]
		[Category("Appearance")]
		public CheckState CheckState
		{
			get 
			{
				return (CheckState) GetValue(TreeNodeAdvStyleInfoStore.CheckStateProperty);
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
		
		internal bool ShouldSerializeCheckState()
		{
			return HasValue(TreeNodeAdvStyleInfoStore.CheckStateProperty);
		}
	}
	
}
