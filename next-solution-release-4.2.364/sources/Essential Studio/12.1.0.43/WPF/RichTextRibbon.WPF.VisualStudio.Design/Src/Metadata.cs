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
[assembly: ProvideMetadata(typeof(Syncfusion.RichTextRibbon.WPF.VisualStudio.Design.Metadata))]
#endif
namespace Syncfusion.RichTextRibbon.WPF.VisualStudio.Design
{
#if SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1
    internal class Metadata : IProvideAttributeTable
    {
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Tools.Controls.RichTextRibbon), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Tools.Controls.HighlightColorPicker), new ToolboxBrowsableAttribute(false));
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
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Tools.Controls.RichTextRibbon), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Tools.Controls.HighlightColorPicker), new ToolboxBrowsableAttribute(false));
                MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif
}
