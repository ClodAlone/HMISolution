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

namespace Syncfusion.OlapGrid.WPF.Expression.Design
{

#if SyncfusionFramework4_0
    internal class Metadata : IProvideAttributeTable
    {
    #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();

                //Proceeds the toolbox hidden elements.
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Grid.Olap.OlapGrid), ToolboxBrowsableAttribute.Yes);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Grid.Olap.OlapGridBase), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Grid.Olap.OlapGridExpandHyperlinkCell), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Grid.Olap.OlapGridHyperlinkCell), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Grid.Olap.OlapGridKpiCell), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Grid.Olap.OlapGridTemplateCell), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Grid.Olap.FormattingWindow), ToolboxBrowsableAttribute.No);
                //builder.AddCustomAttributes(typeof(OlapGrid), new FeatureAttribute(typeof(ContextMenuProvider)));
                return builder.CreateTable();
            }
        }

        #endregion
    }
#elif SyncfusionFramework3_5
    /// <summary>
    /// Container for any general design-time metadata that we want to initialize.
    /// Designers will look for a type in the design-time assembly that implements IRegisterMetadata.
    /// If found, they will instantiate it and call its Register() method automatically.
    /// </summary>
    internal class Metadata : IRegisterMetadata
    {
        public void Register()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();

            // tool box filtering            
           builder.AddCustomAttributes(typeof(Syncfusion.Windows.Grid.Olap.OlapGrid), ToolboxBrowsableAttribute.Yes);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Grid.Olap.OlapGridBase), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Grid.Olap.OlapGridExpandHyperlinkCell), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Grid.Olap.OlapGridHyperlinkCell), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Grid.Olap.OlapGridKpiCell), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Grid.Olap.OlapGridTemplateCell), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Grid.Olap.FormattingWindow), ToolboxBrowsableAttribute.No);
                //builder.AddCustomAttributes(typeof(Syncfusion.Windows.Grid.Olap.OlapGrid), new FeatureAttribute(typeof(ContextMenuProvider)));

            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }

#endif

}
