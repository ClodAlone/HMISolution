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
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Design;

namespace Syncfusion.Shared.WPF.VisualStudio.Design
{
    /// <summary>
    /// ComboBoxAdvAdornerProvider class
    /// </summary>
   public class ComboBoxAdvAdornerProvider : PrimarySelectionAdornerProviderBase
    {
       /// <summary>
        /// Creates Smart Tag.
       /// </summary>
       /// <param name="item"></param>
       /// <returns></returns>
        protected override SmartTagBase CreateSmartTag(ModelItem item)
        {

            ComboBoxAdvSmartTag comboboxSmartTag = new ComboBoxAdvSmartTag();
            comboboxSmartTag.ModelItem = item;
            comboboxSmartTag.Context = base.Context;

            return comboboxSmartTag;

        }
    }
}
