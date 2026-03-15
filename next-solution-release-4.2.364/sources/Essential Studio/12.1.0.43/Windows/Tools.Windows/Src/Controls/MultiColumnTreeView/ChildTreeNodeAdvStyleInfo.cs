#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System.ComponentModel;

using Syncfusion.Documentation;
using Syncfusion.Styles;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    /// <summary>
    /// Special class to support serializing the ChildStyle info.
    /// </summary>
    [
      DocumentationExclude(),
        TypeConverter(typeof(ExpandableObjectConverter))
      ]
    public class ChildTreeNodeAdvStyleInfo : TreeNodeAdvStyleInfo
    {
        public ChildTreeNodeAdvStyleInfo()
        {
        }

        public ChildTreeNodeAdvStyleInfo(StyleInfoIdentityBase identity, TreeNodeAdvStyleInfoStore store)
            : base(identity, store)
        {
        }

        public ChildTreeNodeAdvStyleInfo(StyleInfoIdentityBase identity)
            : base(identity)
        {
        }
    }
}