#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Microsoft.Windows.Design.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Design;
#if SyncfusionFramework4_0 || SyncfusionSLFramework4_0
using Microsoft.Windows.Design.Features;
using Syncfusion.Windows.Reports.Viewer.Utils;
using Syncfusion.Windows.Reports.Viewer.Dialogs;

[assembly: ProvideMetadata(typeof(Syncfusion.Reports.Viewer.Silverlight.dll.Design.Metadata))]
#endif

namespace Syncfusion.Reports.Viewer.Silverlight.dll.Design
{
    internal class Metadata : IProvideAttributeTable
    {
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();
                //Proceeds the toolbox hidden elements.
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Reports.Viewer.ReportViewer), ToolboxBrowsableAttribute.Yes);
                builder.AddCustomAttributes(typeof(LoadingIndicator), ToolboxBrowsableAttribute.No);
                return builder.CreateTable();
            }
        }
    }
}
