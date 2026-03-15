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
using System.Reflection;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.PropertyEditing;
using Syncfusion.Windows.Tools.Controls;
using System.ComponentModel;
using Syncfusion.RichTextRibbon.Silverlight.VisualStudio.Design;
using Syncfusion.RichTextRibbon.Silverlight.VisualStudio.Design.RichTextRibbon;

[assembly: ProvideMetadata(typeof(Syncfusion.RichTextRibbon.Silverlight.VisualStudio.Design.Metadata))]
namespace Syncfusion.RichTextRibbon.Silverlight.VisualStudio.Design
{
    internal class Metadata : IProvideAttributeTable
    {
       AttributeTable IProvideAttributeTable.AttributeTable 
       {
            get
            {
                RichTextRibbonAttributesTableBuilder builder = new RichTextRibbonAttributesTableBuilder();
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Tools.Controls.RichTextRibbon), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Tools.Controls.HighlightColorPicker), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Tools.Controls.BorderThicknessComboBox),new ToolboxBrowsableAttribute(false));
                return builder.CreateTable();
            }
        }                
    }
}
