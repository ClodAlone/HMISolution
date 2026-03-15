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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Tools;
using System.Diagnostics;
using Syncfusion.Windows.Design;

namespace Syncfusion.Tools.WPF.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for AppMenuSmartTag.xaml
    /// </summary>
    public partial class TabControlExtSmartTag : SmartTagBase
    {
        /// <summary>
        /// Initializes a new instance of the TabControlExtSmartTag class.
        /// </summary>
        public TabControlExtSmartTag()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This method is called when the TabControlExtSmartTag Item template is initialized.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            BindSelectorWithEnum(StripPlacementSelector, "TabStripPlacement", typeof(Dock));
            BindSelectorWithEnum(ScrollStyleSelector, "TabScrollStyle", typeof(TabScrollStyle));
            BindSelectorWithEnum(SizeModeSelector, "TabItemSize", typeof(TabItemSizeMode));
            BindSelectorWithEnum(ScrollVisibilitySelector, "TabScrollButtonVisibility", typeof(TabScrollButtonVisibility));
            BindSelectorWithEnum(CloseButtonTypeSelector, "CloseButtonType", typeof(CloseButtonType));
            BindSelectorWithEnum(TabItemLayoutTypeSelector, "TabItemLayout", typeof(TabItemLayoutType));
            BindTextBox(txtName, "Name");

            BindCheckBox(chkShowTabItemContextMenu, "ShowTabItemContextMenu");
            BindCheckBox(chkEnableLabelEdit, "EnableLabelEdit");
            BindCheckBox(chkShowTabListContextMenu, "ShowTabListContextMenu");
           
        }

         
        private void AddChildControls(object sender, RoutedEventArgs e)
        {
#if  SyncfusionFramework3_5
            ModelItem item = ModelFactory.CreateItem(this.Context, typeof(TabItemExt), new object[0]);
            TabControlExt tab = this.ModelItem.View as TabControlExt;
            int n = tab.Items.Count;
            item.Properties["Header"].SetValue(string.Format("tabItem{0}",++n));
			item.Properties["MinHeight"].SetValue(20d);
			item.Properties["MinWidth"].SetValue(150d);
            this.ModelItem.Properties["Items"].Collection.Add(item);
#endif

#if  SyncfusionFramework4_0
            ModelItem item = ModelFactory.CreateItem(this.Context, typeof(TabItemExt), new object[0]);
            int n = this.ModelItem.Properties["Items"].Collection.Count;
            item.Properties["Header"].SetValue(string.Format("tabItem{0}",++n));
			item.Properties["MinHeight"].SetValue(20d);
			item.Properties["MinWidth"].SetValue(150d);
            this.ModelItem.Properties["Items"].Collection.Add(item);
#endif
        }
       

       
       

        


    }

}        

       


        
        
   

