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

[assembly: ProvideMetadata(typeof(Syncfusion.RichTextRibbon.Silverlight.dll.Design.Metadata))]
namespace Syncfusion.RichTextRibbon.Silverlight.dll.Design
{
    public class Metadata :IProvideAttributeTable
    {
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder tablebuilder = new AttributeTableBuilder();
                tablebuilder.AddCallback(typeof(Syncfusion.Windows.Tools.Controls.RichTextRibbon), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));
                tablebuilder.AddCustomAttributes(typeof(Syncfusion.Windows.Tools.Controls.HighlightColorPicker), new ToolboxBrowsableAttribute(false));
                tablebuilder.AddCustomAttributes(typeof(Syncfusion.Windows.Tools.Controls.BorderThicknessComboBox), new ToolboxBrowsableAttribute(false));
                return tablebuilder.CreateTable();
            }
        }
    }
}
