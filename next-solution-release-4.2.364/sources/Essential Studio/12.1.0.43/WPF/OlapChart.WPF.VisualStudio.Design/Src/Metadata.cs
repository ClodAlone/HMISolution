#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

#if SyncfusionFramework4_0 || SyncfusionFramework4_5
using Microsoft.Windows.Design.Metadata;
[assembly: ProvideMetadata(typeof(Syncfusion.OlapChart.WPF.VisualStudio.Design.Metadata))]
#endif

namespace Syncfusion.OlapChart.WPF.VisualStudio.Design
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Windows.Design.Metadata;
    using System.ComponentModel;
    using Microsoft.Windows.Design.PropertyEditing;
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows;
    using Microsoft.Windows.Design;
    using Syncfusion.Windows.Chart.Olap;
    using Microsoft.Windows.Design.Interaction;
    using Microsoft.Windows.Design.Model;
    using Syncfusion.Olap.MDXQueryBuilder;
    using Syncfusion.Olap.Reports;
    using Microsoft.Windows.Design.Features;

#if SyncfusionFramework4_0 || SyncfusionFramework4_5
    internal class Metadata : IProvideAttributeTable
    {
        #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();

                //Proceeds the toolbox hidden elements.
                builder.AddCustomAttributes(typeof(OlapChart), ToolboxBrowsableAttribute.Yes);
                builder.AddCustomAttributes(typeof(OlapLabelsPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(OlapScrollingPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(WaitingControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(OlapArea), ToolboxBrowsableAttribute.No);

                builder.AddCustomAttributes(typeof(WaitingAdorner), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(OlapLabelPresenter), ToolboxBrowsableAttribute.No);
				builder.AddCustomAttributes(typeof(Appearance), ToolboxBrowsableAttribute.No);    
                builder.AddCustomAttributes(typeof(OlapChart), new FeatureAttribute(typeof(ContextMenuProvider)));
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
            builder.AddCustomAttributes(typeof(OlapChart), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(OlapLabelsPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(OlapScrollingPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(WaitingControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(OlapArea), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(WaitingAdorner), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Appearance), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(OlapLabelPresenter), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(OlapChart), new FeatureAttribute(typeof(ContextMenuProvider)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }

#endif
}
