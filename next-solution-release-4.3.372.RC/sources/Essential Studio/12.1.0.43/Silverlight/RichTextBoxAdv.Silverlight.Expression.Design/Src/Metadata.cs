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
using Syncfusion.Windows.Tools.Controls;
using Microsoft.Windows.Design;

[assembly: ProvideMetadata(typeof(Syncfusion.RichTextBoxAdv.Silverlight.Expression.Design.Metadata))]
namespace Syncfusion.RichTextBoxAdv.Silverlight.Expression.Design
{
    public class Metadata :IProvideAttributeTable
    {
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder tablebuilder = new AttributeTableBuilder();
                tablebuilder.AddCustomAttributes(typeof(Syncfusion.Windows.Tools.Controls.RichTextBoxAdv), new ToolboxBrowsableAttribute(true)); 
                tablebuilder.AddCallback(typeof(PageAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                tablebuilder.AddCallback(typeof(PageLayoutViewer), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                tablebuilder.AddCallback(typeof(FlowLayoutViewer), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                tablebuilder.AddCallback(typeof(LayoutViewer), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                tablebuilder.AddCallback(typeof(CaretAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                tablebuilder.AddCallback(typeof(FontDialog), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                tablebuilder.AddCallback(typeof(ParagraphDialog), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                tablebuilder.AddCallback(typeof(HyperlinkDialog), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                tablebuilder.AddCallback(typeof(InsertTableDialog), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                tablebuilder.AddCallback(typeof(ImageResizer), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                return tablebuilder.CreateTable();
            }
        }
    }
}
