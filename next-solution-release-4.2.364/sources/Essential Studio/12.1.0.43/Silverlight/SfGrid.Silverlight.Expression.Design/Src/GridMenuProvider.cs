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
using Microsoft.Windows.Design.Model;
using Syncfusion.UI.Xaml.Grid;

namespace Syncfusion.SfGrid.Silverlight.Expression.Design
{
    internal class GridMenuProvider : PrimarySelectionContextMenuProvider
    {
        private MenuAction gridTextColumn;
        private MenuAction gridNumericColumn;
        private MenuAction gridImageColumn;
        private MenuAction gridCheckBoxColumn;
        private MenuAction gridDateTimeColumn;
        private MenuAction gridTemplateColumn;
        private MenuAction gridMultiColumn;
        private MenuAction gridComboBoxColumn;
        private MenuAction gridCurrencyColumn;
        private MenuAction gridPercentColumn;
        private MenuAction gridMaskColumn;
        private MenuAction gridTimeSpanColumn;
        private MenuAction gridHyperlinkColumn;
        private MenuAction gridUnBoundColumn;

        public GridMenuProvider()
        {
            MenuGroup gridOperationsGroup = new MenuGroup("AddColumns", "Add Columns");
            gridTextColumn = new MenuAction("Add GridTextColumn");
            gridTextColumn.Execute += new EventHandler<MenuActionEventArgs>(AddColumnsMenuAction_Execute);

            gridNumericColumn = new MenuAction("Add GridNumericColumn");
            gridNumericColumn.Execute += new EventHandler<MenuActionEventArgs>(AddColumnsMenuAction_Execute);

            gridImageColumn = new MenuAction("Add GridImageColumn");
            gridImageColumn.Execute += new EventHandler<MenuActionEventArgs>(AddColumnsMenuAction_Execute);

            gridCheckBoxColumn = new MenuAction("Add GridCheckBoxColumn");
            gridCheckBoxColumn.Execute += new EventHandler<MenuActionEventArgs>(AddColumnsMenuAction_Execute);

            gridDateTimeColumn = new MenuAction("Add GridDateTimeColumn");
            gridDateTimeColumn.Execute += new EventHandler<MenuActionEventArgs>(AddColumnsMenuAction_Execute);

            gridTemplateColumn = new MenuAction("Add GridTemplateColumn");
            gridTemplateColumn.Execute += new EventHandler<MenuActionEventArgs>(AddColumnsMenuAction_Execute);

            gridMultiColumn = new MenuAction("Add GridMultiColumnDropDownList");
            gridMultiColumn.Execute += new EventHandler<MenuActionEventArgs>(AddColumnsMenuAction_Execute);

            gridComboBoxColumn = new MenuAction("Add GridComboBoxColumn");
            gridComboBoxColumn.Execute += new EventHandler<MenuActionEventArgs>(AddColumnsMenuAction_Execute);

            gridCurrencyColumn = new MenuAction("Add GridCurrencyColumn");
            gridCurrencyColumn.Execute += new EventHandler<MenuActionEventArgs>(AddColumnsMenuAction_Execute);

            gridPercentColumn = new MenuAction("Add GridPercentColumn");
            gridPercentColumn.Execute += new EventHandler<MenuActionEventArgs>(AddColumnsMenuAction_Execute);

            gridMaskColumn = new MenuAction("Add GridMaskColumn");
            gridMaskColumn.Execute += new EventHandler<MenuActionEventArgs>(AddColumnsMenuAction_Execute);

            gridTimeSpanColumn = new MenuAction("Add GridTimeSpanColumn");
            gridTimeSpanColumn.Execute += new EventHandler<MenuActionEventArgs>(AddColumnsMenuAction_Execute);

            gridHyperlinkColumn = new MenuAction("Add GridHyperlinkColumn");
            gridHyperlinkColumn.Execute += new EventHandler<MenuActionEventArgs>(AddColumnsMenuAction_Execute);

            gridUnBoundColumn = new MenuAction("Add GridUnBoundColumn");
            gridUnBoundColumn.Execute += new EventHandler<MenuActionEventArgs>(AddColumnsMenuAction_Execute);

            gridOperationsGroup.HasDropDown = true;
            gridOperationsGroup.Items.Add(gridTextColumn);
            gridOperationsGroup.Items.Add(gridNumericColumn);
            gridOperationsGroup.Items.Add(gridImageColumn);
            gridOperationsGroup.Items.Add(gridCheckBoxColumn);
            gridOperationsGroup.Items.Add(gridDateTimeColumn);
            gridOperationsGroup.Items.Add(gridTemplateColumn);
            gridOperationsGroup.Items.Add(gridMultiColumn);
            gridOperationsGroup.Items.Add(gridComboBoxColumn);
            gridOperationsGroup.Items.Add(gridCurrencyColumn);
            gridOperationsGroup.Items.Add(gridPercentColumn);
            gridOperationsGroup.Items.Add(gridMaskColumn);
            gridOperationsGroup.Items.Add(gridTimeSpanColumn);
            gridOperationsGroup.Items.Add(gridHyperlinkColumn);
            gridOperationsGroup.Items.Add(gridUnBoundColumn);

            this.Items.Add(gridOperationsGroup);
        }

        private void AddColumnsMenuAction_Execute(object sender, MenuActionEventArgs e)
        {
            ModelItem selectedGrid = e.Selection.PrimarySelection;
            using (ModelEditingScope scope = selectedGrid.BeginEdit("Add Columns"))
            {
                MenuAction eventSender = sender as MenuAction;
                GridColumn gridColumn = GridColumnHelper.CreateGridColumns(eventSender.DisplayName);
                if (gridColumn != null)
                {
                    selectedGrid.Properties["Columns"].Collection.Add(gridColumn);
                }
                scope.Complete();
            }
        }
    }
}
