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
#if SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1
using Microsoft.Windows.Design.Metadata;
[assembly: ProvideMetadata(typeof(Syncfusion.PdfViewer.WPF.VisualStudio.Design.Metadata))]
#endif

namespace Syncfusion.PdfViewer.WPF.VisualStudio.Design
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Microsoft.Windows.Design;
    using Microsoft.Windows.Design.Metadata;
    using Microsoft.Windows.Design.PropertyEditing;
    using Syncfusion.Windows.PdfViewer;

#if SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1
    internal class Metadata : IProvideAttributeTable
    {
    #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();

                //Proceeds the toolbox hidden elements.
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PdfViewer.PdfViewerControl), ToolboxBrowsableAttribute.Yes);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PdfViewer.DocumentToolbar), ToolboxBrowsableAttribute.No);
				builder.AddCustomAttributes(typeof(Syncfusion.Windows.PdfViewer.NotificationBar), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PdfViewer.TextSearchBar), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PdfViewer.PdfDocumentView), ToolboxBrowsableAttribute.Yes);
				builder.AddCustomAttributes(typeof(Syncfusion.Windows.PdfViewer.PageNumberToolTip), ToolboxBrowsableAttribute.No);
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
            //// Tool box filtering
            //builder.AddCustomAttributes(typeof(PdfViewerControl), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(DocumentToolbar), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(NotificationBar), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TextSearchBar), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(PdfDocumentView), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(PageNumberToolTip), new ToolboxBrowsableAttribute(false));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif
}
