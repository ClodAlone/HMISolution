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
using Syncfusion.Silverlight.Grid.Olap;
using Microsoft.Windows.Design;

namespace Syncfusion.OlapGrid.Silverlight.Expression.Design
{

    internal class Metadata : IProvideAttributeTable
    {
    #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();
                builder.AddCustomAttributes(typeof(Syncfusion.Silverlight.Grid.Olap.OlapGrid), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(OlapGridBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OlapGridExpandHyperlinkCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OlapGridHyperlinkCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OlapGridKpiCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OlapGridTemplateCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FormattingWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Silverlight.Grid.Olap.GroupBox), new ToolboxBrowsableAttribute(false));
                return builder.CreateTable();

                
            }
        }

    #endregion
    }

}
