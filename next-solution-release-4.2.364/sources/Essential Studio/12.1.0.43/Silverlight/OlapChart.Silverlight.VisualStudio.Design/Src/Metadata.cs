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
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design;
using Syncfusion.Silverlight.Chart;
using Syncfusion.Silverlight.Chart.Olap;

#if SyncfusionFramework4_0 || SyncfusionSLFramework3_5
using Microsoft.Windows.Design.Features;

// The ProvideMetadata assembly-level attribute indicates to designers
// that this assembly contains a class that provides an attribute table. 
[assembly: ProvideMetadata(typeof(Syncfusion.OlapChart.Silverlight.VisualStudio.Design.Metadata))]
#endif
namespace Syncfusion.OlapChart.Silverlight.VisualStudio.Design
{

#if SyncfusionFramework4_0 || SyncfusionSLFramework3_5
    internal class Metadata : IProvideAttributeTable
    {
    #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();

                builder.AddCallback(typeof(OlapArea), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(Syncfusion.Silverlight.Chart.Olap.OlapChart), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));
                builder.AddCallback(typeof(Syncfusion.Silverlight.Chart.Olap.OlapLabelPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(Syncfusion.Silverlight.Chart.Olap.OlapLabelPresenter), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(Syncfusion.Silverlight.Chart.Olap.OlapChartAxisPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                return builder.CreateTable();
            }
        }

    #endregion
    }
#else
    public class Metadata : IRegisterMetadata
    {
        public void Register()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            builder.AddCallback(typeof(OlapArea), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(Syncfusion.Silverlight.Chart.Olap.OlapChart), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));
            builder.AddCallback(typeof(Syncfusion.Silverlight.Chart.Olap.OlapLabelPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(Syncfusion.Silverlight.Chart.Olap.OlapLabelPresenter), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif
}
