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
using System.IO;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Cells;
using System.Windows;
using System.ComponentModel;
using Syncfusion.Windows.ComponentModel;

namespace Syncfusion.Windows.Controls.Grid
{
    class GridTreeModelTextDataExchange : GridModelTextDataExchange
    {
        public GridTreeModelTextDataExchange(GridTreeModel model) :
            base(model)
        {

        }       

        public override void CutValueFromCell(int rowindex, int colindex, bool clear)
        {
            GridTreeControl treeGrid = null;
            foreach (GridControlBase grid in Model.Views)
            {
                if (grid != null)
                {
                    treeGrid = grid.FindParentElementOfType<GridTreeControl>();
                    if (treeGrid != null)
                    {
                        if (clear)
                        {
                            GridStyleInfo style = this.Model[rowindex, colindex];
                            if (colindex <= treeGrid.Columns.Count && colindex > 0)
                            {
                                //From RowIndex get the Record and then set the Value for the property
                                GridTreeColumn tc = treeGrid.Columns[colindex - 1];
                                GridTreeNode n = treeGrid.InternalGrid.GetNodeAtRowIndex(rowindex);
                                Type cellType = treeGrid.InternalGrid.ItemProperties[tc.MappingName].PropertyType;
                                {
                                    //if (treeGrid.InternalGrid.ItemProperties[treeGrid.ChildPropertyName].Name != tc.MappingName)
                                    {
                                        (treeGrid.InternalGrid as GridTreeControlImpl).SetValueToComponent(tc.MappingName, n.Item, DefaultValue(cellType));
                                        this.Model[rowindex, colindex].CellValue = DefaultValue(cellType);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }      

        private static object DefaultValue(Type myType)
        {
            if (!myType.IsValueType)
                return string.Empty;
            else
                return Activator.CreateInstance(myType);
        }
    }
}