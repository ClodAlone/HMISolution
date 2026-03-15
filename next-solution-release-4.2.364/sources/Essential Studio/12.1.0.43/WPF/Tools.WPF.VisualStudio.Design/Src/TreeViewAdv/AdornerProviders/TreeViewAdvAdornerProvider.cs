#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Design.Model;
using System.Windows;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Design;

namespace Syncfusion.Tools.WPF.VisualStudio.Design
{
    /// <summary>
    /// TreeViewAdvAdornerProvider class
    /// </summary>
    public class TreeViewAdvAdornerProvider : PrimarySelectionAdornerProviderBase
    {
        /// <summary>
        /// Creates Smart Tag.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override SmartTagBase CreateSmartTag( ModelItem item )
        {
            TreeViewAdvSmartTag TreeViewSmartTag = new TreeViewAdvSmartTag();

            TreeViewSmartTag.ModelItem = item;
            TreeViewSmartTag.Context = base.Context;

            return TreeViewSmartTag;
        }


    }
}
