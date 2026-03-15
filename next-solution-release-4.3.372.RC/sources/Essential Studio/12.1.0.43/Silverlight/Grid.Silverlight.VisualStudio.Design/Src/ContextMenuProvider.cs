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
    class GridContextMenuProvider : PrimarySelectionContextMenuProvider
    {
        private MenuAction SaveFileDialog;
        

        // The provider's constructor sets up the MenuAction objects 
        // and the the MenuGroup which holds them.
        public GridContextMenuProvider()
        {   
            // Set up the MenuAction which sets the control's 
            // background to Blue.
            SaveFileDialog = new MenuAction("Show Designer");
            //SaveFileDialog.Checkable = true;
            SaveFileDialog.Execute += 
                new EventHandler<MenuActionEventArgs>(SetBackgroundToBlue_Execute);

            // Set up the MenuAction which sets the control's 
            // background to its default value.
            //OpenFileDialog = new MenuAction("Designer");
            //OpenFileDialog.Checkable = true;
            //OpenFileDialog.Execute += 
            //    new EventHandler<MenuActionEventArgs>(ClearBackground_Execute);

            // Set up the MenuGroup which holds the MenuAction items.
            MenuGroup backgroundFlyoutGroup = 
                new MenuGroup("ShowDialogGroup", "Desiger View");

            // If HasDropDown is false, the group appears inline, 
            // instead of as a flyout. Set to true.
            backgroundFlyoutGroup.HasDropDown = true;
            backgroundFlyoutGroup.Items.Add(SaveFileDialog);
           // backgroundFlyoutGroup.Items.Add(OpenFileDialog);
            this.Items.Add(backgroundFlyoutGroup);
        }

       

        // The following method handles the Execute event. 
        // It sets the Background property to its default value.
        void ClearBackground_Execute(
            object sender, 
            MenuActionEventArgs e)
        {
            //OpenFileDialog fd = new OpenFileDialog();
            //fd.ShowDialog();
        }

        // The following method handles the Execute event. 
        // It sets the Background property to Brushes.Blue.
        void SetBackgroundToBlue_Execute(
            object sender, 
            MenuActionEventArgs e)
        {
            //SaveFileDialog fd = new SaveFileDialog();
            //fd.ShowDialog();
            ModelItem selectedControl = e.Selection.PrimarySelection;
            DesignerModel model = new DesignerModel(selectedControl);
            model.ShowWindow();
            //Window1 window1 = new Window1();
            ////window1
            ////window1.View.SetModel(model);
            //window1.SetModel(model);
        }
    }
}

