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
using Syncfusion.RichTextRibbon.Silverlight.Expression.Design;
using Syncfusion.RichTextRibbon.Silverlight.VisualStudio.Design;

[assembly: ProvideMetadata(typeof(Syncfusion.RichTextRibbon.Silverlight.Expression.Design.Metadata))]
namespace Syncfusion.RichTextRibbon.Silverlight.Expression.Design
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
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Tools.Controls.BorderThicknessComboBox), new ToolboxBrowsableAttribute(false));
                return builder.CreateTable();
            }
        }
    }
}
