#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel.Design;

namespace Syncfusion.Windows.Forms.Tools.Design
{
   public class TabSplitterPagesCollectionEditor : CollectionEditor
    {
        public TabSplitterPagesCollectionEditor(Type type)
            : base(type)
        {
        }

        protected override Type[] CreateNewItemTypes()
        {
            return new Type[]
            {
                typeof(TabSplitterPage)
            };
        }
    }
}
