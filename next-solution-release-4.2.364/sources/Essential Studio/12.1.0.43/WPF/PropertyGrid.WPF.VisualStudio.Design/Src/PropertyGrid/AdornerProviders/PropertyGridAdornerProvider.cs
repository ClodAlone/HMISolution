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
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Design;

namespace Syncfusion.PropertyGrid.WPF.VisualStudio.Design
{
    /// <summary>
    /// Class represents the Adorner
    /// </summary>
    public class PropertyGridAdornerProvider : PrimarySelectionAdornerProviderBase
    {
        /// <summary>
        /// Creates the smart tag.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected override SmartTagBase CreateSmartTag( ModelItem item )
        {
            PropertyGridSmartTag propertyGridSmartTag = new PropertyGridSmartTag();

            propertyGridSmartTag.ModelItem = item;
            propertyGridSmartTag.Context = base.Context;

            return propertyGridSmartTag;
        }


    }
}
