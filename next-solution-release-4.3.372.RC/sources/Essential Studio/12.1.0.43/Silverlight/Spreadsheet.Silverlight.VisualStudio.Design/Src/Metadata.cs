#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
// ReSharper disable RedundantUsingDirective
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Windows.Design.Metadata;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Windows.Design;
using Syncfusion.Windows.Controls.Spreadsheet;


#if SyncfusionFramework3_5 && !SyncfusionSLFramework3_0

namespace Syncfusion.Spreadsheet.Silverlight.VisualStudio.Design
{
    internal class Metadata : IRegisterMetadata
    {

        // Called by Cider to register any design-time metadata
        public void Register()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            // tool box filtering                
            //Spreadsheet Control
            builder.AddCustomAttributes(typeof(SpreadsheetControl), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(SpreadsheetGrid), new ToolboxBrowsableAttribute(false));

            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }

}
#else
using Microsoft.Windows.Design.Features;
using Syncfusion.Windows.Controls.Spreadsheet.Commands;
[assembly: ProvideMetadata(typeof(Syncfusion.Spreadsheet.Silverlight.VisualStudio.Design.Metadata))]
namespace Syncfusion.Spreadsheet.Silverlight.VisualStudio.Design
{
    internal class Metadata : IProvideAttributeTable
    {
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();
                // tool box filtering      

                //Spreadsheet Control
                builder.AddCustomAttributes(typeof(SpreadsheetControl), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(SpreadsheetGrid), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CellFormatWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ConditionalFormatWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HyperlinkWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(InsertCommentWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DataValidationWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FormatAsTableWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(UnhideWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(EncryptCommandWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ProtectCommandWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ProtectWorkbookWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GetPasswordWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FillDropDownItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(UnprotectWorkbookWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonTextBox), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PasteDropDownItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SpreadsheetGroupPanel), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(SpreadsheetGroupButton), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(GroupbyWindow), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(GroupOptionsWindow), new ToolboxBrowsableAttribute(false));
				return builder.CreateTable();
            }
        }
    }
}

#endif
