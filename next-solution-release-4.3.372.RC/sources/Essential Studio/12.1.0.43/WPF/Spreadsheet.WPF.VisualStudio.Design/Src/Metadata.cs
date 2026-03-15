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
// The ProvideMetadata assembly-level attribute indicates to designers
// that this assembly contains a class that provides an attribute table. 
//using Microsoft.Windows.Design.Metadata;
using Syncfusion.Windows.Controls.Spreadsheet;

#if SyncfusionFramework4_0
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Syncfusion.Windows.Controls.Spreadsheet;
#endif
#if SyncfusionFramework4_0
[assembly: ProvideMetadata(typeof(Syncfusion.Spreadsheet.WPF.VisualStudio.Design.Metadata))]
#endif
namespace Syncfusion.Spreadsheet.WPF.VisualStudio.Design
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.ComponentModel;
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows;
    using Microsoft.Windows.Design;
    using Microsoft.Windows.Design.Metadata;
    using Syncfusion.Windows.Controls.Spreadsheet.Commands;


#if SyncfusionFramework4_0
    public class Metadata : Microsoft.Windows.Design.Metadata.IProvideAttributeTable
    {
        // Accessed by the designer to register any design-time metadata.
        public Microsoft.Windows.Design.Metadata.AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();
                // Add the menu provider to the design-time metadata.
                //Spreadsheet Control
                builder.AddCustomAttributes(typeof(SpreadsheetControl), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(SpreadsheetRibbon), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(SpreadsheetGrid), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CellFormatWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ConditionalFormatWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DataValidationWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(EncryptCommandWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FormatAsTableWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HyperlinkWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(InsertCommentWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ProtectCommandWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ProtectWorkbookWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(UnhideWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(UnprotectWorkbookWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GetPasswordWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FillDropDownItem), new ToolboxBrowsableAttribute(false));
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

#else

    internal class Metadata : IRegisterMetadata {

        // Called by Cider to register any design-time metadata
        public void Register() {
            AttributeTableBuilder builder = new AttributeTableBuilder();

                // tool box filtering
                //Spreadsheet Control
                builder.AddCustomAttributes(typeof(SpreadsheetControl), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(SpreadsheetRibbon), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(SpreadsheetGrid), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CellFormatWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ConditionalFormatWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DataValidationWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(EncryptCommandWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FormatAsTableWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HyperlinkWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(InsertCommentWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ProtectCommandWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ProtectWorkbookWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(UnhideWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(UnprotectWorkbookWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GetPasswordWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FillDropDownItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonTextBox), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PasteDropDownItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SpreadsheetGroupPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SpreadsheetGroupButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupbyWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupOptionsWindow), new ToolboxBrowsableAttribute(false));
                MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif
}
