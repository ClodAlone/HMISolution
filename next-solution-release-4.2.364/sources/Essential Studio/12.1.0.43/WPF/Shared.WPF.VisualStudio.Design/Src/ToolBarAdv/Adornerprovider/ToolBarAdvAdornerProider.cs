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
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Tools;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Design;

namespace Syncfusion.Shared.WPF.VisualStudio.Design
{
    class ToolBarAdvAdornerProider : PrimarySelectionAdornerProviderBase
    {
        protected override SmartTagBase CreateSmartTag(ModelItem item)
        {
            ToolBarAdvSmartTag toolbarAdvTag = new ToolBarAdvSmartTag();
            toolbarAdvTag.ModelItem = item;
            toolbarAdvTag.Context = base.Context;
            return toolbarAdvTag;
        }
    }
    
}


