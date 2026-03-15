#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Windows.Design.Interaction;
using System.Windows;
using Microsoft.Windows.Design.Model;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;


namespace Syncfusion.Grid.WPF.VisualStudio.Design
{
    // The CustomContextMenuProvider class provides two context menu items
    // at design time. These are implemented with the MenuAction class.
    class ContextMenuProvider : PrimarySelectionContextMenuProvider
    {
        private MenuAction DesignerDialog;
        
        public ContextMenuProvider()
        {   
            DesignerDialog = new MenuAction("Show Designer");
            DesignerDialog.Execute += 
                new EventHandler<MenuActionEventArgs>(ShowDesigner);
            
            MenuGroup DesingerView = 
                new MenuGroup("ShowDialogGroup", "Designer View");

            DesingerView.HasDropDown = true;
            DesingerView.Items.Add(DesignerDialog);
            this.Items.Add(DesingerView);
        }

        void ShowDesigner(
            object sender, 
            MenuActionEventArgs e)
        {
            ModelItem selectedControl = e.Selection.PrimarySelection;
            DesignerModel model = new DesignerModel(selectedControl);
            model.ShowWindow();
                       
        }
    }
}
