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
using Syncfusion.Silverlight.Controls.PivotGrid;
using Microsoft.Windows.Design;
using Syncfusion.Silverlight.Controls.PivotSchemaDesigner;

namespace Syncfusion.PivotAnalysis.Silverlight.Expression.Design
{

    internal class Metadata : IProvideAttributeTable
    {
    #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();
                builder.AddCustomAttributes(typeof(PivotGridControl), ToolboxBrowsableAttribute.Yes);
                builder.AddCustomAttributes(typeof(PivotGridControlBase), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotExpanderCell), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGridHyperlinkCell), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGridTemplateCell), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(AnimatedGrid), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGridGroupingBar), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGroupingItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGridRowGroupBar), ToolboxBrowsableAttribute.No);

                builder.AddCustomAttributes(typeof(PivotSchemaDesigner), ToolboxBrowsableAttribute.Yes);
                builder.AddCustomAttributes(typeof(ComputationInfoWindow), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(DeletePivotItemCommand), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(DeleteFilterCommand), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ShowFilterCommand), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ShowCalculationCommand), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotTableField), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PopupWindow), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Utils), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotFieldListWindow), ToolboxBrowsableAttribute.No);
                return builder.CreateTable();

                
            }
        }

    #endregion
    }

}
