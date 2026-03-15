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
namespace Syncfusion.OlapChart.WPF.dll.Design
{
    using Microsoft.Windows.Design.Metadata;
    using Microsoft.Windows.Design;
    using Syncfusion.Windows.Chart.Olap;

#if SyncfusionFramework4_0 || SyncfusionFramework4_5
    /// Container for any general design-time metadata that we want to initialize.
    /// Designers will look for a type in the design-time assembly that implements IRegisterMetadata.
    /// If found, they will instantiate it and call its Register() method automatically.
    internal class Metadata : IProvideAttributeTable
    {
        #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {

                AttributeTableBuilder builder = new AttributeTableBuilder();
                // tool box filtering

                builder.AddCustomAttributes(typeof(OlapChart), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(OlapArea), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OlapLabelsPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OlapScrollingPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(WaitingControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(WaitingAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Appearance), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OlapLabelPresenter), new ToolboxBrowsableAttribute(false));
                return builder.CreateTable();
            }
        }

        #endregion
    }
#else
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
               
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif
}
