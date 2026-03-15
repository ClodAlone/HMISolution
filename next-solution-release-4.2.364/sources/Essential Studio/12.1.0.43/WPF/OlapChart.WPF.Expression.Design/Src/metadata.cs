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

namespace Syncfusion.OlapChart.WPF.Expression.Design
{
#if SyncfusionFramework4_0
    internal class Metadata : IProvideAttributeTable
    {
        #region IProvideAttributeTable Members



        public AttributeTable AttributeTable
        {
            get
            {


                //Proceeds the toolbox hidden elements.
                AttributeTableBuilder builder = new AttributeTableBuilder();

                //Proceeds the toolbox hidden elements.
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Olap.OlapChart), ToolboxBrowsableAttribute.Yes);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Olap.OlapLabelsPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Olap.OlapScrollingPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Olap.WaitingControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Olap.OlapArea), ToolboxBrowsableAttribute.No);

                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Olap.WaitingAdorner), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Olap.OlapLabelPresenter), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Olap.Appearance), ToolboxBrowsableAttribute.No);

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
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Olap.OlapChart), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Olap.OlapLabelsPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Olap.OlapScrollingPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Olap.WaitingControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Olap.OlapArea), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Olap.WaitingAdorner), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Olap.Appearance), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Olap.OlapLabelPresenter), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(Syncfusion.Windows.Chart.Olap.OlapChart), new FeatureAttribute(typeof(ContextMenuProvider)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }


   
#endif
}
