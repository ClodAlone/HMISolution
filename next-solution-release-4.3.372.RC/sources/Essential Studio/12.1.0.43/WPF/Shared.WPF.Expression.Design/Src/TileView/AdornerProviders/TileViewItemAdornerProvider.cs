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
using Syncfusion.Windows.Design;
using Syncfusion.Shared.WPF.Expression.Design;

namespace Syncfusion.Shared.WPF.Expression.Design
{
    /// <summary>
    /// Represents TileViewItemAdornerProvider for SmartTag support
    /// </summary>
    public class TileViewItemAdornerProvider : PrimarySelectionAdornerProviderBase
    {
        /// <summary>
        /// Creates the smart tag.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected override SmartTagBase CreateSmartTag( ModelItem item )
        {
            DateTimeEditSmartTag dateTimeEditSmarTag = new DateTimeEditSmartTag();

            dateTimeEditSmarTag.ModelItem = item;
            dateTimeEditSmarTag.Context = base.Context;

            return dateTimeEditSmarTag;
        }
    }
}
