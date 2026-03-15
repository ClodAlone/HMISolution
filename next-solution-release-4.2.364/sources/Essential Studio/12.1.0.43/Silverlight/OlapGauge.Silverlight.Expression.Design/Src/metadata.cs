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
using Syncfusion.Silverlight.Olap.Gauge;
using Microsoft.Windows.Design;

namespace Syncfusion.OlapGauge.Silverlight.Expression.Design
{

    internal class Metadata : IProvideAttributeTable
    {
    #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();
                builder.AddCallback(typeof(OlapCircularGauge), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(Syncfusion.Silverlight.Olap.Gauge.OlapGauge), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));
                return builder.CreateTable();
            }
        }

    #endregion
    }

}
