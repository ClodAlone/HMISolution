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
using Syncfusion.Windows.Tools.Controls;

#if SyncfusionFramework4_0
using Microsoft.Windows.Design.Features;
using System.ComponentModel;
using Microsoft.Windows.Design.PropertyEditing; 

// The ProvideMetadata assembly-level attribute indicates to designers
// that this assembly contains a class that provides an attribute table. 
[assembly: ProvideMetadata(typeof(Syncfusion.Ribbon.Silverlight.dll.Design.Metadata))]
namespace Syncfusion.Ribbon.Silverlight.dll.Design
{
    // Container for any general design-time metadata to initialize.
    // Designers look for a type in the design-time assembly that 
    // implements IProvideAttributeTable. If found, designers instantiate 
    // this class and access its AttributeTable property automatically.

    internal class Metadata : IProvideAttributeTable
    {
        // Accessed by the designer to register any design-time metadata.
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();

                // builder.AddCallback(typeof(ApplicationButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ApplicationMenu), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(SimpleMenuButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(SplitMenuButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonBar), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonBarPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonComboBoxItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonDropDownItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonGallery), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonGalleryItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonGalleryPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                // builder.AddCallback(typeof(RibbonItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonItemBase), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonItemsControl), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ButtonPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonItemsGroupPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                // builder.AddCallback(typeof(RibbonScreenTip), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonSeparator), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonShadow), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonWindow), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                //builder.AddCallback(typeof(RibbonSplitItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

                builder.AddCallback(typeof(TabButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonTabPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonTabsGroup), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonTabStrip), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonTitlePanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));


                // builder.AddCallback(typeof(RibbonToolbarDropDownItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ScrollPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ScrollStrip), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

                builder.AddCallback(typeof(RibbonStatusBar), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(MiniToolbar), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonDropDown), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonMenu), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonTextBox), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonListBox), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonCheckBox), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonTab), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonRadioButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonComboBox), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonDropDownButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonSplitButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ScreenTip), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(QuickAccessToolBar), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(QATDropDownControl), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(QatCustomizationDialog), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonMenuGroup), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonCommandManager), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonCommandProvider), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonCommand), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(RibbonMenuItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

                builder.AddCallback(typeof(Backstage), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(BackStageButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(BackStageCommandButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(BackstageTabItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

                builder.AddCustomAttributes(typeof(RibbonDropDownItem), BrowsableAttribute.No);

                # region Ribbon
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Tools.Controls.Ribbon), "Items", new NewItemTypesAttribute(typeof(RibbonTab)));
                # endregion Ribbon

                # region RibbonTab
                builder.AddCustomAttributes(typeof(RibbonTab), "Items", new NewItemTypesAttribute(typeof(RibbonBar)));
                # endregion RibbonTab

                # region RibbonBar
                builder.AddCustomAttributes(typeof(RibbonBar), "Items", new NewItemTypesAttribute(typeof(RibbonCheckBox)));
                builder.AddCustomAttributes(typeof(RibbonBar), "Items", new NewItemTypesAttribute(typeof(RibbonComboBox)));
                builder.AddCustomAttributes(typeof(RibbonBar), "Items", new NewItemTypesAttribute(typeof(RibbonDropDownButton)));
                builder.AddCustomAttributes(typeof(RibbonBar), "Items", new NewItemTypesAttribute(typeof(RibbonGallery)));
                builder.AddCustomAttributes(typeof(RibbonBar), "Items", new NewItemTypesAttribute(typeof(RibbonButton)));
                builder.AddCustomAttributes(typeof(RibbonBar), "Items", new NewItemTypesAttribute(typeof(RibbonSplitButton)));
                builder.AddCustomAttributes(typeof(RibbonBar), "Items", new NewItemTypesAttribute(typeof(RibbonListBox)));
                builder.AddCustomAttributes(typeof(RibbonBar), "Items", new NewItemTypesAttribute(typeof(RibbonTextBox)));
                # endregion RibbonBar

                #region RibbonmenuGroup
                builder.AddCustomAttributes(typeof(RibbonMenuGroup), "Items", new NewItemTypesAttribute(typeof(RibbonMenuItem)));
                #endregion

                # region RibbonCombobox
                builder.AddCustomAttributes(typeof(RibbonComboBox), "Items", new NewItemTypesAttribute(typeof(RibbonComboBoxItem)));
                # endregion RibbonCombobox

                # region RibbonGallery
                builder.AddCallback(typeof(RibbonGallery),
                    delegate(AttributeCallbackBuilder builder1)
                    {
                        builder1.AddCustomAttributes("Footer", BrowsableAttribute.No);
                        builder1.AddCustomAttributes("SelectedItem", BrowsableAttribute.No);
                    });

                builder.AddCustomAttributes(typeof(RibbonGallery), "Items", new NewItemTypesAttribute(typeof(RibbonGalleryItem)));

                builder.AddCustomAttributes(typeof(RibbonGallery), BrowsableAttribute.No);
                # endregion RibbonGallery




                return builder.CreateTable();
            }
        }
    }

#elif SyncfusionFramework3_5

namespace Syncfusion.Ribbon.Silverlight.dll.Design
{
    /// <summary>
    /// Represent MetaData Class.
    /// </summary>
	public class Metadata : IRegisterMetadata
	{
		#region IRegisterMetadata Members

        /// <summary>
        /// Attaches design-time metadata to a particular control type.
        /// </summary>
		public void Register()
		{
			AttributeTableBuilder builder = new AttributeTableBuilder();

			builder.AddCallback(typeof(ApplicationButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
			builder.AddCallback(typeof(ApplicationMenu), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
			builder.AddCallback(typeof(RibbonItemsControl), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
			builder.AddCallback(typeof(RibbonBarPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
			builder.AddCallback(typeof(RibbonItemsGroupPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
			builder.AddCallback(typeof(ScrollPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
			builder.AddCallback(typeof(ScrollStrip), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

			builder.AddCallback(typeof(RibbonPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
			builder.AddCallback(typeof(RibbonShadow), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
			builder.AddCallback(typeof(TabButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
			builder.AddCallback(typeof(RibbonTabPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
			builder.AddCallback(typeof(RibbonTabStrip), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
			builder.AddCallback(typeof(RibbonTitlePanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
			builder.AddCallback(typeof(RibbonToolbar), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

			builder.AddCallback(typeof(RibbonGalleryPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
			builder.AddCallback(typeof(RibbonItemBase), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
			builder.AddCallback(typeof(RibbonToolbarDropDownItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(RibbonComboBoxItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

            builder.AddCallback(typeof(RibbonItemsControl), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(RibbonItemsGroup), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(RibbonTabsGroup), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(RibbonDropDownItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            
			MetadataStore.AddAttributeTable(builder.CreateTable());
		}

		#endregion
	}
    #endif
}