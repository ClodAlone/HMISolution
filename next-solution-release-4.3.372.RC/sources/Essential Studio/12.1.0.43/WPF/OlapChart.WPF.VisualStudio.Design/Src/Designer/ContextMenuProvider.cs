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
using Microsoft.Windows.Design.Interaction;
using System.Windows;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Shared.Olap;

namespace Syncfusion.OlapChart.WPF.VisualStudio.Design
{
    /// <summary>
    /// Primary selection context menu provider.
    /// </summary>
    public sealed class ContextMenuProvider
        : PrimarySelectionContextMenuProvider
    {
        #region Members

        private const string MENU_ACTION_DATA_SOURCE_PROPERTIES = "Create/Edit data source...";
        private const string MENU_ACTION_UI_PROPERTIES = "Properties...";
        private MenuAction createOrEditMenuItem;
        private WizardWindow wizardWindow;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextMenuProvider"/> class.
        /// </summary>
        public ContextMenuProvider()
        {
            try
            {
                //// Creating sub-menu item.
                createOrEditMenuItem = new MenuAction(MENU_ACTION_DATA_SOURCE_PROPERTIES);

                //// Event tagging for the sub-menu item.
                createOrEditMenuItem.Execute += new EventHandler<MenuActionEventArgs>(wizard_Execute);

                //// Creating menu item group.
                MenuGroup contextMenu = new MenuGroup("Customization", "Configure data source");
                contextMenu.HasDropDown = true;

                //// Adding menu item to the group.
                contextMenu.Items.Add(createOrEditMenuItem);

                //// Adding menu to the context menu.
                this.Items.Add(contextMenu);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " at " + this.GetType().FullName);
            }
        } 

        #endregion

        #region Events

        /// <summary>
        /// Handles the Execute event of the wizard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Windows.Design.Interaction.MenuActionEventArgs"/> instance containing the event data.</param>
        private void wizard_Execute(object sender, MenuActionEventArgs e)
        {
            //// Selected Item
            ModelItem selectedControl = e.Selection.PrimarySelection;

            var action = sender as MenuAction;
            if (action.DisplayName == MENU_ACTION_DATA_SOURCE_PROPERTIES)
            {
                wizardWindow = new WizardWindow(selectedControl);
                wizardWindow.Show();
            }
            else if (action.DisplayName == MENU_ACTION_UI_PROPERTIES)
            {
            }
        } 

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        private object Model
        {
            get;
            set;
        }

        #endregion
    }        
}
