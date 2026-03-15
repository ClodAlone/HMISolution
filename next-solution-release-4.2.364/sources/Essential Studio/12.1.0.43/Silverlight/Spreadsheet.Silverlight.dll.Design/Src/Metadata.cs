#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Microsoft.Windows.Design.Metadata;

#if SyncfusionFramework4_0 || SyncfusionSLFramework4_0
using Microsoft.Windows.Design.Features;

[assembly: ProvideMetadata(typeof(Syncfusion.Spreadsheet.Silverlight.dll.Design.Metadata))]
#endif

namespace Syncfusion.Spreadsheet.Silverlight.dll.Design
{
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
    using Syncfusion.Windows.Controls.Spreadsheet.Commands;

#if SyncfusionFramework4_0 || SyncfusionSLFramework4_0
    internal class Metadata : IProvideAttributeTable
    {
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();
                // tool box filtering                
                //ExcelEditor Control
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
                builder.AddCustomAttributes(typeof(PasteDropDownItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SpreadsheetGroupPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SpreadsheetGroupButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupbyWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupOptionsWindow), new ToolboxBrowsableAttribute(false));
                return builder.CreateTable();
            }
        }
    }
#else

    internal class Metadata : IRegisterMetadata
    {

        // Called by Cider to register any design-time metadata
        public void Register()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            // tool box filtering                
             //Spreadsheet Control
                builder.AddCustomAttributes(typeof(SpreadsheetControl), new ToolboxBrowsableAttribute(true));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif
}