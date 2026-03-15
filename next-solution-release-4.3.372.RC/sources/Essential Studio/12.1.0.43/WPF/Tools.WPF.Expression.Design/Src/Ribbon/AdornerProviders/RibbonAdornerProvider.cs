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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Tools.Controls;
using System.Diagnostics;
using System.Collections;
using System.IO;
using Syncfusion.Windows.Design;

namespace Syncfusion.Tools.WPF.Expression.Design
{
    /// <summary>
    /// Represents AdornerProvider for SmartTag Support
    /// </summary>
    public class RibbonAdornerProvider : PrimarySelectionAdornerProviderBase
    {
        /// <summary>
        /// Creates the smart tag.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected override SmartTagBase CreateSmartTag( ModelItem item )
        {
            RibbonSmartTag ribbonSmartTag = new RibbonSmartTag( );

            ribbonSmartTag.ModelItem = item;
            ribbonSmartTag.Context = base.Context;

            return ribbonSmartTag;
        }
    }

}
