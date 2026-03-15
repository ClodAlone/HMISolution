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
using Syncfusion.Windows.Diagram;
using Syncfusion.Windows.Design;

namespace Syncfusion.Diagram.WPF.VisualStudio.Design
{
    public class DiagramControlAdornerProvider : PrimarySelectionAdornerProviderBase
    {
        protected override SmartTagBase CreateSmartTag( ModelItem item )
        {
            DiagramControlSmartTag diagramSmartTag = new DiagramControlSmartTag();
            diagramSmartTag.ModelItem = item;
            diagramSmartTag.Context = base.Context;
            return diagramSmartTag;
        }


    }
}
