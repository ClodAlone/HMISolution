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
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Data;

namespace Syncfusion.UI.Xaml.Grid.Helpers
{
    public static class GridHelper
    {
        public static void UpdateDataRow(this SfDataGrid dataGrid, int rowIndex)
        {
            dataGrid.GridModel.UpdateDataRow(rowIndex);
        }
#if !WP
        public static SfDataGrid GetDetailsViewGrid(this SfDataGrid dataGrid, int recordIndex, string relationalColumn)
        {
            RecordEntry record;
            if (dataGrid.GridModel.HasGroup)
                record = dataGrid.View.TopLevelGroup.DisplayElements[recordIndex] as RecordEntry;
            else
                record = dataGrid.View.Records[recordIndex];

            if (record != null && record.IsExpanded)
            {
                int rowIndex = dataGrid.ResolveToRowIndex(recordIndex);
                var gridView = dataGrid.DetailsViewDefinition.FirstOrDefault(detsilsview => detsilsview.RelationalColumn == relationalColumn);
                var gridViewIndex = dataGrid.DetailsViewDefinition.IndexOf(gridView);
                var actualRowIndex = rowIndex + gridViewIndex + 1;
                var row = dataGrid.RowGenerator.Items.FirstOrDefault(detailsrow => detailsrow.RowIndex == actualRowIndex);
                if (row != null && row is DetailsViewDataRow)
                    return (row as DetailsViewDataRow).DetailsViewDataGrid;
            }
            return null;
        }
#endif
    }
}
