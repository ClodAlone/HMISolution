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
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Windows.Design.Metadata;
using System.ComponentModel;
using Microsoft.Windows.Design.PropertyEditing;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Windows.Design;
using Syncfusion.Windows.Edit;
using Syncfusion.Windows.Design;
using Microsoft.Windows.Design.Features;

// The ProvideMetadata assembly-level attribute indicates to designers
// that this assembly contains a class that provides an attribute table. 

#if SyncfusionFramework4_0
[assembly: ProvideMetadata(typeof(Syncfusion.Edit.Wpf.Expression.Design.Metadata))]
#endif

namespace Syncfusion.Edit.Wpf.Expression.Design
{

    /// <summary>
    /// Container for any general design-time metadata that we want to initialize.
    /// Designers will look for a type in the design-time assembly that implements IRegisterMetadata.
    /// If found, they will instantiate it and call its Register() method automatically.
    /// </summary>
#if SyncfusionFramework4_0
    internal class Metadata : IProvideAttributeTable
    {
        // Accessed by the designer to register any design-time metadata.
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();

                // tool box filtering
                builder.AddCustomAttributes(typeof(LineItemsCollection), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LineItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(EditCommands), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(EditScrollControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SelectionPointer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SelectionPointerChangedEventArgs), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(WordDetails), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LineItemExpandInformation), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ApplyExpandCollapseArgs), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FindOptions), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FindReplaceControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FindReplacePopup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FindResult), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SearchResult), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(IntellisensePopup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(EditTypeInfo), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LanguageBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CSharpLanguage), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VBLanguage), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(XAMLLanguage), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(XMLLanguage), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SQLLanguage), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VariableLineWidthProvider), new ToolboxBrowsableAttribute(false));
                // Category Editor

				builder.AddCallback(typeof(EditControl), delegate(AttributeCallbackBuilder attribute)
            {
                attribute.AddCustomAttributes("Lines", BrowsableAttribute.No);
                attribute.AddCustomAttributes("SelectedText", BrowsableAttribute.No);
                attribute.AddCustomAttributes("FindOptions", BrowsableAttribute.No);
                attribute.AddCustomAttributes("SearchResults", BrowsableAttribute.No);
                attribute.AddCustomAttributes("ShowOutlining", BrowsableAttribute.No);
                attribute.AddCustomAttributes("CustomLanguage", BrowsableAttribute.No);
                attribute.AddCustomAttributes(
                    new FeatureAttribute(typeof(EditControlInitializer))
                  );
            });

				
                //builder.AddCustomAttributes(typeof(EditControl), new FeatureAttribute(typeof(EditControlInitializer)));
                //builder.AddCustomAttributes(typeof(EditControl), new FeatureAttribute(typeof(EditControlAdornerProvider)));
                //builder.AddCustomAttributes(typeof(EditControl), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                return builder.CreateTable();
            }
        }
    }
#elif SyncfusionFramework3_5

    internal class Metadata : IRegisterMetadata
    {

        // Called by Cider to register any design-time metadata
        public void Register()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();

            // tool box filtering

            builder.AddCustomAttributes(typeof(LineItemsCollection), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(LineItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(EditCommands), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(EditScrollControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(SelectionPointer), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(SelectionPointerChangedEventArgs), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(WordDetails), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(LineItemExpandInformation), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ApplyExpandCollapseArgs), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(FindOptions), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(FindReplaceControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(FindReplacePopup), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(FindResult), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(SearchResult), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(IntellisensePopup), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(EditTypeInfo), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(LanguageBase), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CSharpLanguage), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(VBLanguage), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(XAMLLanguage), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(XMLLanguage), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(SQLLanguage), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(VariableLineWidthProvider), new ToolboxBrowsableAttribute(false));
            // Category Editor

            builder.AddCallback(typeof(EditControl), delegate(AttributeCallbackBuilder attribute)
            {
                attribute.AddCustomAttributes("Lines", BrowsableAttribute.No);
                attribute.AddCustomAttributes("SelectedText", BrowsableAttribute.No);
                attribute.AddCustomAttributes("FindOptions", BrowsableAttribute.No);
                attribute.AddCustomAttributes("SearchResults", BrowsableAttribute.No);
                attribute.AddCustomAttributes("ShowOutlining", BrowsableAttribute.No);
                attribute.AddCustomAttributes("CustomLanguage", BrowsableAttribute.No);
                attribute.AddCustomAttributes(
                    new FeatureAttribute(typeof(EditControlInitializer))
                  );
            });

            //builder.AddCustomAttributes(typeof(EditControl), new FeatureAttribute(typeof(EditControlInitializer)));
            //builder.AddCustomAttributes(typeof(EditControl), new FeatureAttribute(typeof(EditControlAdornerProvider)));
            //builder.AddCustomAttributes(typeof(EditControl), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));


            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }

#endif
}