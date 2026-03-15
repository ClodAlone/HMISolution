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
[assembly: ProvideMetadata(typeof(Syncfusion.OlapClient.WPF.VisualStudio.Design.Metadata))]
#endif
namespace Syncfusion.OlapClient.WPF.VisualStudio.Design
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
    using Syncfusion.Windows.Tools.Olap;
    using Syncfusion.Windows.Client.Olap;

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
                builder.AddCustomAttributes(typeof(OlapClient), ToolboxBrowsableAttribute.Yes);
                builder.AddCustomAttributes(typeof(ConnectOptions), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ConnectToServer), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ReportNameWindow), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Client.Olap.WaitingDialog), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(MdxDialog), ToolboxBrowsableAttribute.No);
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
            builder.AddCustomAttributes(typeof(OlapClient), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(ConnectOptions), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ConnectToServer), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ReportNameWindow), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Client.Olap.WaitingDialog), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(MdxDialog), new ToolboxBrowsableAttribute(false));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif
}
