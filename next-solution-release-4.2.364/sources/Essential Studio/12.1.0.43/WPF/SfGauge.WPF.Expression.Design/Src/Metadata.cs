#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Syncfusion.UI.Xaml.Gauges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: ProvideMetadata(typeof(Syncfusion.SfGauge.WPF.Expression.Design.Metadata))]

namespace Syncfusion.SfGauge.WPF.Expression.Design
{
    public class Metadata : IProvideAttributeTable
    {
        private AttributeTableBuilder builder;
        public AttributeTable AttributeTable
        {
            get
            {
                if (builder == null)
                {
                    builder = new AttributeTableBuilder();
                }

                //Toolbox filtering
                //Circular Gauge
                builder.AddCustomAttributes(typeof(CircularPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CircularScale), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SquaredPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TickLinesPanel), new ToolboxBrowsableAttribute(false));

                ////Digital Gauge
                builder.AddCustomAttributes(typeof(DigitalCharactersPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DigitalCharacter), new ToolboxBrowsableAttribute(false));

                ////Linear Guage
                builder.AddCustomAttributes(typeof(LinearGaugeUniformPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LinearScale), new ToolboxBrowsableAttribute(false));
                return builder.CreateTable();
            }
        }
    }
}
