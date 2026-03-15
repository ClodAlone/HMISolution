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
    public partial class DockingSmartTag : SmartTagBase
    {
        /// <summary>
        /// Initializes a new instance of the DockingSmartTag class.
        /// </summary>
        public DockingSmartTag()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This method is called when the DockingSmartTag Item template is initialized.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BindTextBox(txtName, "Name");
            BindCheckBox(chkShowTabItemContextMenu, "ShowTabItemContextMenu");
            BindCheckBox(chkusedocumentcontainer, "UseDocumentContainer");
            BindCheckBox(chkuseintrop, "UseInteropCompatibilityMode");
            BindCheckBox(chkpersiststate, "PersistState");
            BindCheckBox(chkShowTabListContextMenu, "ShowTabListContextMenu");

            BindSelectorWithEnum(tabstripplace, "DockTabAlignment", typeof(Dock));
            BindSelectorWithEnum(draggingtype, "DraggingType", typeof(DraggingType));
            BindSelectorWithEnum(animtype, "AutoHideAnimationMode", typeof(AutoHideAnimationMode));
            BindSelectorWithEnum(autohidetabsmode, "AutoHideTabsMode", typeof(AutoHideTabsMode));
            BindSelectorWithEnum(closetabsmode, "CloseTabs", typeof(CloseTabsMode));
            BindSelectorWithEnum(containermode, "ContainerMode", typeof(DocumentContainerMode));
            BindSelectorWithEnum(switchmode, "SwitchMode", typeof(SwitchMode));


           
        }

        
         
        private void AddChildControls(object sender, RoutedEventArgs e)
        {
            Grid grid = new Grid();
            int n = this.ModelItem.Properties["Children"].Collection.Count;
            DockingManager.SetHeader(grid,string.Format("Dock Window {0}",++n));
            this.ModelItem.Properties["Children"].Collection.Add(grid);
        }
       

       
       

        


    }

}        

       


        
        
   

