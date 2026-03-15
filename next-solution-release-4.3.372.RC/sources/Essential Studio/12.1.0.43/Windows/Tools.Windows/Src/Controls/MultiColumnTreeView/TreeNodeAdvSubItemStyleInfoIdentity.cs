#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System.Collections;

using Syncfusion.Styles;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{ 
    public class TreeNodeAdvSubItemStyleInfoIdentity : StyleInfoIdentityBase
    {
        #region Class members
     
        private MultiColumnTreeView m_tree;
    
        private TreeNodeAdv m_node;

        private TreeNodeAdvSubItem m_subitem;
        #endregion

        #region Class properties
        /// <summary>Gets parent control.</summary>
        public MultiColumnTreeView TreeView
        {
            get
            {
                if (this.TreeNode != null)
                {
                    return this.TreeNode.TreeView;
                }

                return m_tree;
            }
        }

        /// <summary>Gets parent node.</summary>
        public TreeNodeAdv TreeNode
        {
            get
            {
                if (this.TreeNodeSubItem != null)
                {
                    return this.TreeNodeSubItem.TreeNode;
                }

                return m_node;
            }
        }

        /// <summary>Gets parent subitem.</summary>
        public TreeNodeAdvSubItem TreeNodeSubItem
        {
            get
            {
                return m_subitem;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        public TreeNodeAdvSubItemStyleInfoIdentity(MultiColumnTreeView tree)
        {
            m_tree = tree;
        }

        public TreeNodeAdvSubItemStyleInfoIdentity(TreeNodeAdv node)
        {
            m_node = node;
        }
        public TreeNodeAdvSubItemStyleInfoIdentity(TreeNodeAdvSubItem subitem)
        {
            m_subitem = subitem;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Returns an array with base styles for the specified style object.
        /// </summary>
        /// <param name="info">The style object.</param>
        /// <returns>An array of style objects that are base styles for the current style object.</returns>
        public override IStyleInfo[] GetBaseStyles(IStyleInfo info)
        {
            ArrayList styles = new ArrayList(4);
            TreeNodeAdvSubItemStyleInfo styleInfo = info as TreeNodeAdvSubItemStyleInfo;

            // try to get style by subitem references first
            if (styleInfo != null)
            {
                if (this.TreeView != null)
                {
                    if (styleInfo.HasBaseStyle &&
                      this.TreeView.BaseStyles.Contains(styleInfo.BaseStyle))
                    {
                        styles.Add(this.TreeView.BaseStyles[styleInfo.BaseStyle]);
                    }

                    styles.Add(this.TreeView.StandardSubItemStyle);
                    styles.Add(this.TreeView.StandardStyle);
                }

                IStyleInfo[] results = new IStyleInfo[styles.Count];
                styles.CopyTo(results);
                return results;
            }

            // nothing found
            return new IStyleInfo[] { };
        }
        #endregion
    }
}