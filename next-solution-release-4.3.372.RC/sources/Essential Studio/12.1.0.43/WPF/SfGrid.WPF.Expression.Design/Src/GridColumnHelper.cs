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
using System.Threading.Tasks;
using Microsoft.Windows.Design.Interaction;
using Syncfusion.UI.Xaml.Grid;

namespace Syncfusion.SfGrid.WPF.Expression.Design
{
    internal static class GridColumnHelper
    {
        internal static GridColumn CreateGridColumns(string actionName)
        {
            GridColumn gridColumn = null;
            if (actionName == "Add GridTextColumn")
            {
                gridColumn = new GridTextColumn();
            }
            else if (actionName == "Add GridNumericColumn")
            {
                gridColumn = new GridNumericColumn();
            }
            else if (actionName == "Add GridImageColumn")
            {
                gridColumn = new GridImageColumn();
            }
            else if (actionName == "Add GridCheckBoxColumn")
            {
                gridColumn = new GridCheckBoxColumn();
            }
            else if (actionName == "Add GridDateTimeColumn")
            {
                gridColumn = new GridDateTimeColumn();
            }
            else if (actionName == "Add GridTemplateColumn")
            {
                gridColumn = new GridTemplateColumn();
            }
            else if (actionName == "Add GridMultiColumnDropDownList")
            {
                gridColumn = new GridMultiColumnDropDownList();
            }
            else if (actionName == "Add GridComboBoxColumn")
            {
                gridColumn = new GridComboBoxColumn();
            }
            else if (actionName == "Add GridCurrencyColumn")
            {
                gridColumn = new GridCurrencyColumn();
            }
            else if (actionName == "Add GridPercentColumn")
            {
                gridColumn = new GridPercentColumn();
            }
            else if (actionName == "Add GridMaskColumn")
            {
                gridColumn = new GridMaskColumn();
            }
            else if (actionName == "Add GridTimeSpanColumn")
            {
                gridColumn = new GridTimeSpanColumn();
            }
            else if (actionName == "Add GridHyperlinkColumn")
            {
                gridColumn = new GridHyperlinkColumn();
            }
            else if (actionName == "Add GridUnBoundColumn")
            {
                gridColumn = new GridUnBoundColumn();
            }
            return gridColumn;
        }
    }
}
 