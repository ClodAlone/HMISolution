#region	Copyright	Syncfusion Inc.	2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System.Collections;
using System.ComponentModel;

using Syncfusion.Documentation;
using Syncfusion.Styles;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    [DocumentationExclude()]
    public class StyleNamePairsList : ArrayList
    {
        #region Class members

        private MultiColumnTreeView m_tree;
        #endregion

        #region Class Initialize/Finalize methods
  
        public StyleNamePairsList(MultiColumnTreeView tree)
        {
            m_tree = tree;
        }
        #endregion

        #region Class Public Methods

        public void AddRange(StyleNamePair[] stylePairs)
        {
            foreach (StyleNamePair pair in stylePairs)
            {
                if (pair.StyleGeneral != null)
                {
                    StyleInfoIdentityBase identity = null;

                    if (pair.TreeNodeStyle != null)
                    {
                        identity = new TreeViewAdvStyleInfoIdentity(m_tree);
                    }
                    else if (pair.ColumnStyle != null)
                    {
                        identity = new TreeColumnAdvStyleInfoIdentity(m_tree);
                    }
                    else if (pair.SubItemStyle != null)
                    {
                        identity = new TreeNodeAdvSubItemStyleInfoIdentity(m_tree);
                    }

                    ((StyleInfoBase)pair.StyleGeneral).Identity = identity;
                }

                this.m_tree.BaseStyles[pair.Name] = pair.StyleGeneral;
            }
        }
        #endregion
    }

    [
      DocumentationExclude(),
        TypeConverter(typeof(StyleNamePairConverter))
      ]
    public class StyleNamePair
    {
        #region Class members

        private string m_name;

        private IStyleInfo m_style;
        #endregion

        #region Class properties
 
        public string Name
        {
            get
            {
                return this.m_name;
            }
        }

        public IStyleInfo StyleGeneral
        {
            get
            {
                return m_style;
            }
        }

        public TreeNodeAdvStyleInfo TreeNodeStyle
        {
            get
            {
                return m_style as TreeNodeAdvStyleInfo;
            }
        }

        public TreeColumnAdvStyleInfo ColumnStyle
        {
            get
            {
                return m_style as TreeColumnAdvStyleInfo;
            }
        }

        public TreeNodeAdvSubItemStyleInfo SubItemStyle
        {
            get
            {
                return m_style as TreeNodeAdvSubItemStyleInfo;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        public StyleNamePair(string name, IStyleInfo style)
        {
            m_name = name;
            m_style = style;
        }
        #endregion
    }
}