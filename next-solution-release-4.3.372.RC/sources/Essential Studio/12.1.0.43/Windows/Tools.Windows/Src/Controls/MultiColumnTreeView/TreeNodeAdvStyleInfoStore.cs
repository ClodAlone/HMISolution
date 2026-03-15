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
using System.Drawing;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Forms;

using Syncfusion.Documentation;
using Syncfusion.Drawing;
using Syncfusion.Styles;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    [
    Serializable,
    DocumentationExclude()
    ]
    public class TreeNodeAdvStyleInfoStore : StyleInfoStore
    {
        #region Class static members
        internal static StaticData Sd = new StaticData(typeof(TreeNodeAdvStyleInfoStore), typeof(TreeNodeAdvStyleInfo), false);
        #endregion

        #region Storage properties declaration

        // Objects - more frequently used fields should come first because of memory performance reasons
        // Data will be allocated per style object on a slot basis, 4 object references at a time.
        internal static readonly StyleInfoProperty FontProperty = Sd.CreateStyleInfoProperty(typeof(Font), "Font");

        internal static readonly StyleInfoProperty TextColorProperty = Sd.CreateStyleInfoProperty(typeof(Color), "TextColor");

        internal static readonly StyleInfoProperty BackgroundProperty = Sd.CreateStyleInfoProperty(typeof(BrushInfo), "Background");

        internal static readonly StyleInfoProperty TextProperty = Sd.CreateStyleInfoProperty(typeof(string), "Text");
   
        internal static readonly StyleInfoProperty HelpTextProperty = Sd.CreateStyleInfoProperty(typeof(string), "HelpText");

        internal static readonly StyleInfoProperty DisplayMemberProperty = Sd.CreateStyleInfoProperty(typeof(string), "DisplayMember");

        internal static readonly StyleInfoProperty HeightProperty = Sd.CreateStyleInfoProperty(typeof(int), "Height");

        internal static readonly StyleInfoProperty ShowCheckBoxProperty = Sd.CreateStyleInfoProperty(typeof(bool), "ShowCheckBox");

        internal static readonly StyleInfoProperty CheckColorProperty = Sd.CreateStyleInfoProperty(typeof(Color), "ActiveCheckColor");

        internal static readonly StyleInfoProperty IntermediateCheckColorProperty = Sd.CreateStyleInfoProperty(typeof(Color), "InActiveCheckColor");

        internal static readonly StyleInfoProperty CheckBoxBackGroundProperty = Sd.CreateStyleInfoProperty(typeof(Brush), "CheckBoxBackGround", StyleInfoPropertyOptions.All & ~StyleInfoPropertyOptions.Disposable);

        internal static readonly StyleInfoProperty IntermediateCheckBoxBackgroundProperty = Sd.CreateStyleInfoProperty(typeof(Brush), "IntermediateCheckBoxBackground");

        internal static readonly StyleInfoProperty SelectedOptionButtonColorProperty = Sd.CreateStyleInfoProperty(typeof(Color), "SelectedOptionButtonColor");

        internal static readonly StyleInfoProperty OptionButtonColorProperty = Sd.CreateStyleInfoProperty(typeof(Color), "OptionButtonColor");

        internal static readonly StyleInfoProperty TagProperty = Sd.CreateStyleInfoProperty(typeof(object), "Tag");

        internal static readonly StyleInfoProperty ShowPlusMinusProperty = Sd.CreateStyleInfoProperty(typeof(bool), "ShowPlusMinus");

        internal static readonly StyleInfoProperty ShowOptionButtonProperty = Sd.CreateStyleInfoProperty(typeof(bool), "ShowOptionButton");

        internal static readonly StyleInfoProperty LeftImageProperty = Sd.CreateStyleInfoProperty(typeof(Image), "LeftImage");

        internal static readonly StyleInfoProperty RightImageProperty = Sd.CreateStyleInfoProperty(typeof(Image), "RightImage");

        internal static readonly StyleInfoProperty OpenImageProperty = Sd.CreateStyleInfoProperty(typeof(Image), "StateOpenImage");

        internal static readonly StyleInfoProperty ClosedImageProperty = Sd.CreateStyleInfoProperty(typeof(Image), "StateClosedImage");

        internal static readonly StyleInfoProperty NoChildrenImageProperty = Sd.CreateStyleInfoProperty(typeof(Image), "StateNoChildrenImage");

        internal static readonly StyleInfoProperty ExpandedImageProperty = Sd.CreateStyleInfoProperty(typeof(Image), "ExpandedImage");

        internal static readonly StyleInfoProperty CollapsedImageProperty = Sd.CreateStyleInfoProperty(typeof(Image), "CollapsedImage");

        internal static readonly StyleInfoProperty LeftImageIndicesProperty = Sd.CreateStyleInfoProperty(typeof(int[]), "LeftImageIndices");

        internal static readonly StyleInfoProperty RightImageIndicesProperty = Sd.CreateStyleInfoProperty(typeof(int[]), "RightImageIndices");

        internal static readonly StyleInfoProperty NoChildrenImgIndexProperty = Sd.CreateStyleInfoProperty(typeof(int), "NoChildrenImgIndex");

        internal static readonly StyleInfoProperty ClosedImgIndexProperty = Sd.CreateStyleInfoProperty(typeof(int), "ClosedImgIndex");
   
        internal static readonly StyleInfoProperty OpenImgIndexProperty = Sd.CreateStyleInfoProperty(typeof(int), "OpenImgIndex");

        internal static readonly StyleInfoProperty ExpandImageIndexProperty = Sd.CreateStyleInfoProperty(typeof(int), "ExpandImageIndex");

        internal static readonly StyleInfoProperty CollapseImageIndexProperty = Sd.CreateStyleInfoProperty(typeof(int), "CollapseImageIndex");

        internal static readonly StyleInfoProperty ThemesEnabledProperty = Sd.CreateStyleInfoProperty(typeof(bool), "ThemesEnabled");

        internal static readonly StyleInfoProperty InteractiveCheckBoxProperty = Sd.CreateStyleInfoProperty(typeof(bool), "InteractiveCheckBox");

        internal static readonly StyleInfoProperty SortTypeProperty = Sd.CreateStyleInfoProperty(typeof(TreeNodeAdvSortType), "SortType");

        internal static readonly StyleInfoProperty SortOrderProperty = Sd.CreateStyleInfoProperty(typeof(SortOrder), "SortOrder");

        internal static readonly StyleInfoProperty ComparerProperty = Sd.CreateStyleInfoProperty(typeof(IComparer), "Comparer");

        internal static readonly StyleInfoProperty CompareOptionsProperty = Sd.CreateStyleInfoProperty(typeof(CompareOptions), "CompareOptions");

        internal static readonly StyleInfoProperty CheckStateProperty = Sd.CreateStyleInfoProperty(typeof(CheckState), "CheckState");

        internal static readonly StyleInfoProperty BaseStyleProperty = Sd.CreateStyleInfoProperty(typeof(string), "BaseStyle");

        internal static readonly StyleInfoProperty EnabledProperty = Sd.CreateStyleInfoProperty(typeof(bool), "Enabled");

        internal static readonly StyleInfoProperty EnabledButtonsProperty = Sd.CreateStyleInfoProperty(typeof(bool), "EnabledButtons");

        internal static readonly StyleInfoProperty EnsureDefaultOptionedChildProperty = Sd.CreateStyleInfoProperty(typeof(bool), "EnsureDefaultOptionedChild");

        internal static readonly StyleInfoProperty CultureProperty = Sd.CreateStyleInfoProperty(typeof(CultureInfo), "Culture");

        /// <summary>Declaration of storage propterty: LeftImagePadding.</summary>
        internal static readonly StyleInfoProperty LeftImagePaddingProperty = Sd.CreateStyleInfoProperty(typeof(int), "LeftImagePadding");

        /// <summary>Declaration of storage propterty: RightImagePadding.</summary>
        internal static readonly StyleInfoProperty RightImagePaddingProperty = Sd.CreateStyleInfoProperty(typeof(int), "RightImagePadding");

        /// <summary>Declaration of storage propterty: LeftStateImagePadding.</summary>
        internal static readonly StyleInfoProperty LeftStateImagePaddingProperty = Sd.CreateStyleInfoProperty(typeof(int), "LeftStateImagePadding");

        /// <summary>Declaration of storage propterty: RightStateImagePadding.</summary>
        internal static readonly StyleInfoProperty RightStateImagePaddingProperty = Sd.CreateStyleInfoProperty(typeof(int), "RightStateImagePadding");

        /// <summary>Declaration of storage propterty: Multiline.</summary>
        internal static readonly StyleInfoProperty MultilineProperty = Sd.CreateStyleInfoProperty(typeof(bool), "Multiline");
        #endregion

        #region Class properties

        protected override StaticData StaticDataStore
        {
            get
            {
                return Sd;
            }
        }
        #endregion

        #region Class Initialization methods
        public TreeNodeAdvStyleInfoStore()
            : base()
        {
        }

        protected TreeNodeAdvStyleInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (Sd.IsEmpty)
            {
                new TreeNodeAdvStyleInfo();
            }
        }
        #endregion

        #region Class Public Methods

        public override object Clone()
        {
            StyleInfoStore target = new TreeNodeAdvStyleInfoStore();
            CopyTo(target);
            return target;
        }
        #endregion
    }

    internal class TreeViewAdvStyleInfoIdentity : StyleInfoIdentityBase
    {
        #region Class members

        private MultiColumnTreeView _tree;

        private TreeNodeAdv _node;
        #endregion

        #region Class properties

        public MultiColumnTreeView TreeView
        {
            get
            {
                if (this._tree != null)
                {
                    return _tree;
                }
                else
                {
                    return this._node.TreeView;
                }
            }
        }

        public TreeNodeAdv Node
        {
            get
            {
                return this._node;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        public TreeViewAdvStyleInfoIdentity(MultiColumnTreeView tree)
        {
            this._tree = tree;
        }

        /// <summary>
        /// Initializes a new instance of the TreeViewAdvStyleInfoIdentity class.
        /// that this is called only for ChildStyles.
        /// </summary>
        /// <param name="node">Tree node</param>
        public TreeViewAdvStyleInfoIdentity(TreeNodeAdv node)
        {
            this._node = node;
        }

        public override void Dispose()
        {
            _tree = null;
            _node = null;
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Class overrides

        public override IStyleInfo[] GetBaseStyles(IStyleInfo thisStyleInfo)
        {
            TreeNodeAdvStyleInfo treeNodeAdvStyleInfo = (TreeNodeAdvStyleInfo)thisStyleInfo;

            // IStyleInfo styleInfo = null;
            ArrayList styles = new ArrayList();
            MultiColumnTreeView tree = this.TreeView;

            if (tree != null)
            {
                // Style's base style
                if (treeNodeAdvStyleInfo.HasBaseStyle
                  && tree.BaseStyles.Contains(treeNodeAdvStyleInfo.BaseStyle))
                {
                    TreeNodeAdvStyleInfoIdentity.AddStyleAndItsBases(tree, styles, tree.BaseStyles[treeNodeAdvStyleInfo.BaseStyle] as TreeNodeAdvStyleInfo);
                }

                if (this.Node != null && this.Node.TreeView == tree)
                {
                    string nodeLevelStyle = MultiColumnTreeView.NodeLevelStyleBaseName;
                    nodeLevelStyle += (this.Node.Level + 1).ToString();

                    // Need to include this for ChildStyles
                    // NodeLevel Style:
                    if (tree.BaseStyles.Contains(nodeLevelStyle))
                    {
                        TreeNodeAdvStyleInfoIdentity.AddStyleAndItsBases(tree, styles, tree.BaseStyles[nodeLevelStyle] as TreeNodeAdvStyleInfo);                          
                    }
                }

                // Standard style
                if (tree.BaseStyles.Contains(MultiColumnTreeView.DefaultBaseStyleName))
                {
                    TreeNodeAdvStyleInfoIdentity.AddStyleAndItsBases(tree, styles, tree.BaseStyles[MultiColumnTreeView.DefaultBaseStyleName] as TreeNodeAdvStyleInfo);              
                }

                // Add tree-bound style
                styles.Add(tree.BoundStyle);
            }

            if (styles.Count > 0)
            {
                IStyleInfo[] aStyles = new IStyleInfo[styles.Count];

                for (int i = 0; i < styles.Count; i++)
                {
                    aStyles[i] = styles[i] as IStyleInfo;
                }

                return aStyles;
            }
            else
            {
                return new IStyleInfo[0];
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override void OnStyleChanged(StyleInfoBase style, StyleInfoProperty sip)
        {
            // TraceUtil.TraceCurrentMethodInfo(sip.PropertyName, sip.FormatValue(style.GetValue(sip)));
        }
        #endregion
    }

    internal class TreeNodeAdvStyleInfoIdentity : StyleInfoIdentityBase
    {
        #region Class members

        private TreeNodeAdv treeNode;
        #endregion

        #region Class properties

        public TreeNodeAdv TreeNode
        {
            get
            {
                return treeNode;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        public TreeNodeAdvStyleInfoIdentity(TreeNodeAdv treeNode)
        {
            this.treeNode = treeNode;
        }

        public override void Dispose()
        {
            treeNode = null;
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Class overrides

        public override IStyleInfo[] GetBaseStyles(IStyleInfo thisStyleInfo)
        {
            TreeNodeAdvStyleInfo treeNodeAdvStyleInfo = (TreeNodeAdvStyleInfo)thisStyleInfo;

            ArrayList styles = new ArrayList(16);
            MultiColumnTreeView tree = treeNode.TreeView;

            if (tree != null)
            {
                string levelStyle = MultiColumnTreeView.NodeLevelStyleBaseName + treeNode.Level.ToString();

                // Node's base style
                if (treeNodeAdvStyleInfo.HasBaseStyle
                  && tree.BaseStyles.Contains(treeNodeAdvStyleInfo.BaseStyle))
                {
                    AddStyleAndItsBases(tree, styles, tree.BaseStyles[treeNodeAdvStyleInfo.BaseStyle] as TreeNodeAdvStyleInfo);
                }

                // Parent's ChildStyle
                if (treeNode.Parent != null)
                {
                    AddStyleAndItsBases(tree, styles, treeNode.Parent.ChildStyle);
                }

                // Node level style
                if (tree.BaseStyles.Contains(levelStyle))
                {
                    AddStyleAndItsBases(tree, styles, tree.BaseStyles[levelStyle] as TreeNodeAdvStyleInfo);
                }

                // Standard style
                if (tree.BaseStyles.Contains(MultiColumnTreeView.DefaultBaseStyleName))
                {
                    AddStyleAndItsBases(tree, styles, tree.BaseStyles[MultiColumnTreeView.DefaultBaseStyleName] as TreeNodeAdvStyleInfo);
                }

                // Add tree-bound style
                styles.Add(tree.BoundStyle);
            }

            if (styles.Count > 0)
            {
                IStyleInfo[] results = new IStyleInfo[styles.Count];
                styles.CopyTo(results);
                return results;
            }
            else
            {
                return new IStyleInfo[0];
            }
        }

        /// <summary>Recursively add style and it's base styles</summary>
        /// <param name="tree">MultiColumn TreeView</param>
        /// <param name="styles">Array List for styles</param>
        /// <param name="style">TreeNodeAdv StyleInfo</param>
        internal static void AddStyleAndItsBases(MultiColumnTreeView tree, ArrayList styles, TreeNodeAdvStyleInfo style)
        {
            if (styles.Count < 16)
            {
                styles.Add(style);

                if (style != null && style.HasBaseStyle
          && tree.BaseStyles.Contains(style.BaseStyle))
                {
                    TreeNodeAdvStyleInfo baseStyle = tree.BaseStyles[style.BaseStyle] as TreeNodeAdvStyleInfo;
                    AddStyleAndItsBases(tree, styles, baseStyle);
                }
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override void OnStyleChanged(StyleInfoBase style, StyleInfoProperty sip)
        {
            // TraceUtil.TraceCurrentMethodInfo(sip.PropertyName, sip.FormatValue(style.GetValue(sip)));
        }
        #endregion
    }

    internal class TreeBoundStyleInfoStore : TreeNodeAdvStyleInfoStore
    {
        #region Class members

        private MultiColumnTreeView tree;
        #endregion

        #region Class Initialize/Finalize methods

        public TreeBoundStyleInfoStore(MultiColumnTreeView tree)
        {
            this.tree = tree;
        }
        #endregion

        #region Class overrides

        public override bool HasValue(StyleInfoProperty sip)
        {
            if (sip == TreeNodeAdvStyleInfoStore.FontProperty)
            {
                return true;
            }
            else if (sip == TreeNodeAdvStyleInfoStore.TextColorProperty)
            {
                return true;
            }
            else if (sip == TreeNodeAdvStyleInfoStore.HeightProperty)
            {
                return true;
            }
            return base.HasValue(sip);
        }

        public override object GetValue(StyleInfoProperty sip)
        {
            if (sip == TreeNodeAdvStyleInfoStore.FontProperty)
            {
                return tree.Font;
            }
            else if (sip == TreeNodeAdvStyleInfoStore.TextColorProperty)
            {
                return tree.ForeColor;
            }
            else if (sip == TreeNodeAdvStyleInfoStore.HeightProperty)
            {
                return tree.ItemHeight;
            }

            return base.GetValue(sip);
        }
        #endregion
    }
}