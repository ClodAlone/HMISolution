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
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Syncfusion.Windows.Tools.Controls;
#if SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1
[assembly: ProvideMetadata(typeof(Syncfusion.RichTextBoxAdv.WPF.VisualStudio.Design.Metadata))]
#endif
namespace Syncfusion.RichTextBoxAdv.WPF.VisualStudio.Design
{
#if SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1
    internal class Metadata : IProvideAttributeTable
    {
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Tools.Controls.RichTextBoxAdv), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(PageAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PageLayoutViewer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FlowLayoutViewer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LayoutViewer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CaretAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FontDialog), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HyperlinkDialog), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(InsertTableDialog), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ParagraphDialog), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ImageResizer), new ToolboxBrowsableAttribute(false));
                return builder.CreateTable();
            }
        }
    }
#else
    internal class Metadata : IRegisterMetadata
    {
        public void Register()
        {
                AttributeTableBuilder builder = new AttributeTableBuilder();
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Tools.Controls.RichTextBoxAdv), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(PageAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PageLayoutViewer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FlowLayoutViewer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LayoutViewer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CaretAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FontDialog), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HyperlinkDialog), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(InsertTableDialog), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ParagraphDialog), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ImageResizer), new ToolboxBrowsableAttribute(false));
                MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif
}
