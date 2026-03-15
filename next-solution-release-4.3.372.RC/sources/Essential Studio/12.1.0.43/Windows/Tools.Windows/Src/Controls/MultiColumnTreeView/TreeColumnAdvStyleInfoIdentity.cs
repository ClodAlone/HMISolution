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

using Syncfusion.Styles;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    internal class TreeColumnAdvStyleInfoIdentity : StyleInfoIdentityBase
    {
        #region Class members
        private MultiColumnTreeView m_tree;

        private TreeColumnAdv m_column;
        #endregion

        #region Class properties
        /// <summary>Gets the parent control.</summary>
        public MultiColumnTreeView TreeView
        {
            get
            {
                if (this.Column != null)
                {
                    return this.Column.TreeView;
                }

                return m_tree;
            }
        }

        /// <summary> Gets the parent column.</summary>
        public TreeColumnAdv Column
        {
            get
            {
                return m_column;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        public TreeColumnAdvStyleInfoIdentity(MultiColumnTreeView tree)
        {
            m_tree = tree;
        }

        public TreeColumnAdvStyleInfoIdentity(TreeColumnAdv column)
        {
            m_column = column;
        }
        public override void Dispose()
        {
            m_column = null;
            GC.SuppressFinalize(this);
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
            TreeColumnAdvStyleInfo styleInfo = info as TreeColumnAdvStyleInfo;

            // return style of the tree and it first column
            if (styleInfo != null)
            {
                if (this.TreeView != null)
                {
                    // add into list inherited 
                    if (styleInfo.HasBaseStyle &&
                      this.TreeView.BaseStyles.Contains(styleInfo.BaseStyle))
                    {
                        styles.Add(this.TreeView.BaseStyles[styleInfo.BaseStyle]);
                    }

                    styles.Add(this.TreeView.StandardColumnStyle);
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