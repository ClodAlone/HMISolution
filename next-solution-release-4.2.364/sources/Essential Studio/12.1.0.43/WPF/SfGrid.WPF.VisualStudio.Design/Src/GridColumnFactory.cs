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
using Microsoft.Windows.Design.PropertyEditing;
using Syncfusion.UI.Xaml.Grid;

namespace Syncfusion.SfGrid.WPF.VisualStudio.Design
{
    class GridColumnFactory : NewItemFactory
    {
        GridColumn gridColumn = null;
        public override object CreateInstance(Type type)
        {
            if (type.IsAssignableFrom(typeof(GridTemplateColumn)))
            {
                gridColumn = CreateTemplateColumn();
            }
            else
            {
                gridColumn = Activator.CreateInstance(type) as GridColumn;
            }

            if (gridColumn != null)
            {
                gridColumn.HeaderText = "Header";
            }

            return gridColumn;
        }

        private GridColumn CreateTemplateColumn()
        {
            GridTemplateColumn gridColumn = new GridTemplateColumn();
            gridColumn.CellTemplate = new DataTemplate();
            gridColumn.EditTemplate = new DataTemplate();

            return gridColumn;
        }

    }
}
