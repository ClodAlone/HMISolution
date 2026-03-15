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
using System.Windows.Input;
using Syncfusion.Windows.Controls.Cells;
using System.Windows;

namespace Syncfusion.Windows.Controls.Grid
{
    #region GridTreeModel
    /// <exclude/>
    /// <summary>
    /// Derived GridModel for the GridTreeControl.
    /// </summary>
    /// <remarks>Used to upldating Columns collection during the user dragging and dropping a column
    /// in another position. OnSelectionChanged is also handled to support persistent cell seelctions
    /// in the GridTreeControl.</remarks>
    public class GridTreeModel : GridModel
    {
        GridTreeControlImpl treeControl = null;
        public GridTreeControlImpl TreeGrid
        {
            get { return treeControl; }
        }

        public GridTreeModel()
        {
            this.Options.WrapCell = true;
            this.Options.ListBoxModeAllowUIElementClick = true;
            this.Options.DrawSelectionOptions = GridDrawSelectionOptions.ReplaceBackground | GridDrawSelectionOptions.ReplaceTextColor | GridDrawSelectionOptions.ExcelLikeSelectionMarker;
        }

        private GridTreeModelTextDataExchange textDataExchange;
        ///<summary>
        ///Gets text data exchange for the grid. Lets you copy cell text to a stream or clipboard and recreate the
        ///cell text at a later time.
        ///</summary>
        public override GridModelTextDataExchange TextDataExchange
        {
            get
            {
                if (this.textDataExchange == null)
                {
                    this.textDataExchange = new GridTreeModelTextDataExchange(this);
                }

                return this.textDataExchange;
            }
        }

        //used to handle dragging columns...
        /// <override/>
        protected override void OnColumnsMoved(GridRangeMovedEventArgs e)
        {
            if (e.InsertAt == e.RemoveAt)
                return;

            base.OnColumnsMoved(e);
            foreach (GridControlBase grid in this.Views)
            {
                GridTreeControlImpl tree = grid as GridTreeControlImpl;
                if (tree != null)
                {
                    int pos = tree.ColumnIndexToPosition(e.RemoveAt);

                    if (pos > -1)
                    {
                        if (tree.CurrentCell.HasCurrentCell)
                        {
                            tree.CurrentCell.Deactivate();
                        }

                        GridTreeColumn col = tree.Columns[pos];
                        tree.Columns.Remove(col);
                        int adjustment = e.RemoveAt < e.InsertAt ? 1 : 0;
                        int insertPos = tree.ColumnIndexToPosition(e.InsertAt - adjustment);
                        if (insertPos > -1)
                        {
                            tree.Columns.Insert(insertPos, col);
                        }
                        else
                        {
                            tree.Columns.Add(col);
                        }
                        tree.InvalidateCells();
                    }
                }
            }
        }

        protected override void OnSelectionChanging(GridSelectionChangingEventArgs e)
        {

            //ignore changes initiated by clicking on column header (since this is used for sorts)
            if (treeControl == null)
            {
                foreach (GridControlBase grid in this.Views)
                {
                    treeControl = grid as GridTreeControlImpl;
                    if (treeControl != null)
                        break;
                }
            }



            RowColumnIndex cell = treeControl.PointToCellRowColumnIndex
#if !SILVERLIGHT
(Mouse.GetPosition(treeControl));
#else
(DependencyObjectExtensions.GetMousePosition(Application.Current.RootVisual));
#endif
            if ((cell.RowIndex < 0 ) && !treeControl.CurrentCell.IsInMove)
            {
                e.Cancel = true;
                return;
            }


            base.OnSelectionChanging(e);
        }
        //support from persistence of cell selections during expand/collapse/sorts
        GridRangeInfo activeRange = GridRangeInfo.Empty;
        GridRangeInfo lastActiveRange = GridRangeInfo.Empty;
        internal GridRangeInfoList lastRangeList = GridRangeInfoList.Empty;
        internal bool inSort = false;
        /// <exclude/>
        protected override void OnSelectionChanged(GridSelectionChangedEventArgs e)
        {
            if (treeControl == null)
            {
                foreach (GridControlBase grid in this.Views)
                {
                    treeControl = grid as GridTreeControlImpl;
                    if (treeControl != null)
                        break;
                }
            }

            if (treeControl == null || inSort) //change no selections during a sort...
                return;
            //if (treeControl.overComboButton)
            //{
            //    return;
            //}


            if (!treeControl.EnableNodeSelection && treeControl.ParentTreeControl.EnableSelections && e.Range != null && !e.Range.IsEmpty)
            {
                treeControl.InvalidateCell(e.Range);
                treeControl.InvalidateVisual();
            }

            if (treeControl.inExpandCollapseClick
               && (treeControl.Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.AlphaBlend) == 0
               && treeControl.EnableNodeSelection == false
               && treeControl.Model.Options.AllowSelection != GridSelectionFlags.None)
            {
                return;
            }

            bool shift = (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
            bool ctl = (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None;

            if (e.Reason == GridSelectionReason.SetCurrentCell && !treeControl.EnableNodeSelection && !treeControl.inExpandCollapseClick)
            {
                if (!shift && !ctl)
                {
                    this.ClearNodes();
                }
            }

            //maintain the old selection ranges
            if (e.Reason == GridSelectionReason.Clear && treeControl.isSelectedNodesChangedOnCollapsing)
            {
                treeControl.isSelectedNodesChangedOnCollapsing = false;
                treeControl.CurrentCell.Deactivate();
                return;
            }

            if (treeControl.inExpandCollapseClick)
                return;

            if (e.Reason == GridSelectionReason.MouseUp)
            {
                //lastRangeList.Clear();
                base.OnSelectionChanged(e);
                return;
            }

            switch (e.Reason)
            {
                case GridSelectionReason.Clear:
                    ClearAll();
                    break;
                case GridSelectionReason.MouseUp:
                    break;
                case GridSelectionReason.MouseDown:
                    if (!shift && !ctl && Options.ListBoxSelectionMode != GridSelectionMode.MultiSimple && !lastActiveRange.IsEmpty&&
                        !lastActiveRange.Contains(activeRange))
                    {
                        ClearAll();
                        activeRange = GridRangeInfo.Cell(e.Range.Top, e.Range.Left);
                    }
                    else if (ctl)
                        activeRange = GridRangeInfo.Cell(e.Range.Top, e.Range.Left);
                    else //shift
                    {
                        activeRange = e.Range;
                        if (Options.ExcelLikeSelectionFrame)
                        {
                            ClearSelectedCellsOutOfFrame(activeRange);
                            SelectedRanges.Add(activeRange);
                        }
                    }
                    break;
                case GridSelectionReason.SetCurrentCell:
                    if (!shift) // && !ctl)
                    {
                        ClearAll();
                    }

                    activeRange = e.Range;

                    break;
                case GridSelectionReason.MouseMove:
                    if (!lastActiveRange.Contains(activeRange))
                    {
                        ClearAll(false); activeRange = e.Range; 
                    }
                    else
                        activeRange = e.Range;
                    break;
                default:
                    activeRange = e.Range;

                    break;
            }

            //tweak selection to handle selecting rows...
            if (activeRange.IsRows || (activeRange.Left == 0 && activeRange.IsCells && activeRange.Width == 1))
                activeRange = GridRangeInfo.Cells(activeRange.Top, 1, activeRange.Bottom, this.ColumnCount - 1);

            if (!ctl && !shift && Options.ListBoxSelectionMode != GridSelectionMode.MultiSimple && !activeRange.Contains(lastActiveRange))
            {
                if (lastRangeList.Count > 0)
                {
                    ClearAll(false);
                }
                else
                    if (e.Range != null)
                    {
                        InvalidateCell(e.Range);
                    }
            }
             
            GridRangeInfo range = activeRange;

#if SILVERLIGHT
            bool isValidationMsgShown;
            bool suspendMoveTo;
#endif

            if (range.IsTable)
                treeControl.SelectedNodes.AddRange(new HashSet<GridTreeNode>(treeControl.Nodes));
            else
            {
                for (int row = range.Top; row <= range.Bottom; ++row)
                {
                    GridTreeNode n = treeControl.GetNodeAtRowIndex(row);
                    if (n != null)
                    {
                        if (!n.IsSelected && n.SelectedColumns != null && n.SelectedColumns.Count > 0)
                        {
                            n.SelectedColumns.Clear();
                        }
                        if (n.SelectedColumns.Count == 0)// && (!treeControl.EnableSelections || treeControl.EnableSelections && Options.AllowSelection != GridSelectionFlags.Any && Options.AllowSelection != GridSelectionFlags.Cell))
                        {
                            treeControl.SelectedNodes.SetSelected(n, true);
                        }
                        else if (!treeControl.EnableNodeSelection && Options.ListBoxSelectionMode == GridSelectionMode.MultiSimple && n.IsSelected)
                            treeControl.SelectedNodes.SetSelected(n, false);

                        for (int col = range.Left; col <= range.Right; ++col)
                        {
                            string colName = treeControl.ColumnIndexToName(col);
                            int loc = n.SelectedColumns.IndexOf(colName);
                            if (loc == -1)
                            {
//                                if (treeControl.CurrentCell != null && treeControl.CurrentCell.IsEditing &&
//                                        treeControl.CurrentCell.RangeInfo != range && !treeControl.Model.Options.ExcelLikeSelection)
//                                {
//                                    // Check the cell value is modified and save the modified value before deactivating. 
//                                    //var style = treeControl.Model[treeControl.CurrentCell.RowIndex, treeControl.CurrentCell.ColumnIndex];
//                                    var style = treeControl.RenderStyles.GetRenderStyleInfo(treeControl.CurrentCell.RowIndex, treeControl.CurrentCell.ColumnIndex);
//                                    var renderer = treeControl.CurrentCell.Renderer;
//                                    //null check for renderer
//                                    if (null != style.CellValue && renderer != null && style.CellValue.ToString() != renderer.ControlText)
//                                    {
//#if !SILVERLIGHT
//                                        treeControl.CurrentCell.ConfirmChanges();
//                                        treeControl.CurrentCell.EndEdit();
//#else
//                                        treeControl.CurrentCell.ConfirmChanges(out isValidationMsgShown, out suspendMoveTo);
//                                        treeControl.CurrentCell.EndEdit();
//#endif
//                                    }

//#if !SILVERLIGHT
//                                    treeControl.CurrentCell.Deactivate(true);
//#else
//                                    treeControl.CurrentCell.Deactivate(true, out isValidationMsgShown, out suspendMoveTo);
//#endif
//                                }
                                n.SelectedColumns.Add(colName);
                            }
                        }
                    }
                }
            }

            foreach (GridRangeInfo r in lastRangeList)
                treeControl.InternalGrid.InvalidateCell(r);

            if (treeControl.InternalGrid.Model.Options.ExcelLikeSelectionFrame)
            {
                treeControl.InternalGrid.Model.SelectedRanges.Clear();
                treeControl.InternalGrid.Model.SelectedRanges.Add(e.Range);
            }

            treeControl.InvalidateVisual();
            lastActiveRange = activeRange;
            lastRangeList.Clear();
            if (treeControl.EnableNodeSelection || treeControl.InternalGrid.Model.Options.ExcelLikeSelectionFrame)
                treeControl.InternalGrid.Model.SelectedRanges.Clear();
            foreach (GridTreeNode n in treeControl.SelectedNodes)
            {
                var r = GridRangeInfo.Row(treeControl.GetRowIndexFromNode(n));
                if (!lastRangeList.Contains(r))
                {
                    lastRangeList.Add(r);
                    if (treeControl.EnableNodeSelection || treeControl.InternalGrid.Model.Options.ExcelLikeSelectionFrame)
                        treeControl.InternalGrid.Model.SelectedRanges.Add(r);
                }
            }
            
            base.OnSelectionChanged(e);
        }

        internal void ClearNodes()
        {
            if (treeControl == null)
            {
                foreach (GridControlBase grid in this.Views)
                {
                    treeControl = grid as GridTreeControlImpl;
                    if (treeControl != null)
                        break;
                }
            }
            if (treeControl == null)
                return;
            List<object> items = new List<object>();
            foreach (GridTreeNode n in treeControl.SelectedNodes)
            {
                items.Add(n.Item);
            }
            treeControl.SelectedNodes.Clear();
            foreach (object o in items)
            {
                int row = treeControl.GetRowIndexFromItem(o);
                treeControl.InvalidateCell(GridRangeInfo.Row(row));
            }
            items.Clear();
            treeControl.InvalidateVisual();
        }

        private void ClearSelectedCellsOutOfFrame(GridRangeInfo activeRange)
        {
            if (activeRange.IsEmpty)
                return;
            List<string> colNames = new List<string>();
            for (int col = activeRange.Left; col <= activeRange.Right; ++col)
            {
                colNames.Add(treeControl.PropertyNameFromColumnIndex(col));
            }
            for (int row = activeRange.Top; row <= activeRange.Bottom; ++row)
            {
                GridTreeNode n = treeControl.GetNodeAtRowIndex(row);
                n.SelectedColumns.Clear();
                n.SelectedColumns.AddRange(colNames);
                if (!n.IsSelected)
                {
                    treeControl.SelectedNodes.SetSelected(n, true);
                }
            }
        }

        private void ClearAll()
        {
            ClearAll(true);
        }
        private void ClearAll(bool reset)
        {
            if (treeControl.lockSelectedNodes)
                return;

            foreach (GridTreeNode n in treeControl.SelectedNodes)
            {
                n.SelectedColumns.Clear();
            }

            //if (treeControl.SelectedNodes.Count > 0)
            //    treeControl.SelectedNodes.RemoveRange(treeControl.SelectedNodes.ToList());
            treeControl.SelectedNodes.Clear();

            if (reset)
            {
                activeRange = GridRangeInfo.Empty;
                lastActiveRange = GridRangeInfo.Empty;
            }
            treeControl.InvalidateVisual();
        }

        public ResourceDictionary GetVisualStyleDictionary(FrameworkElement element)
        {
            foreach (GridControlBase grid in this.Views)
            {
                treeControl = grid as GridTreeControlImpl;
                if (treeControl != null)
                    break;
            }
            if (treeControl.VisualStyle == VisualStyle.BureauBlue || this.treeControl.VisualStyle == VisualStyle.GlassyGreen || this.treeControl.VisualStyle == VisualStyle.TwilightBlue || this.treeControl.VisualStyle == VisualStyle.SyncfusionTheme || this.treeControl.VisualStyle == VisualStyle.Windows7
                    || this.treeControl.VisualStyle == VisualStyle.SunBlack || this.treeControl.VisualStyle == VisualStyle.Office2007Blue || this.treeControl.VisualStyle == VisualStyle.Office2007Black || this.treeControl.VisualStyle == VisualStyle.Office2007Silver || this.treeControl.VisualStyle == VisualStyle.Blend || this.treeControl.VisualStyle == VisualStyle.Office14Blue ||
                    this.treeControl.VisualStyle == VisualStyle.Office14Black || this.treeControl.VisualStyle == VisualStyle.Office14Silver || this.treeControl.VisualStyle == VisualStyle.ShinyRed || this.treeControl.VisualStyle == VisualStyle.ShinyBlue || this.treeControl.VisualStyle == VisualStyle.VS2010 || this.treeControl.VisualStyle == VisualStyle.Default || this.treeControl.VisualStyle == VisualStyle.Metro)
            {
                string str = this.treeControl.VisualStyle.ToString();
                if (this.treeControl.VisualStyle == VisualStyle.Default)
                    str = "Windows7";
#if !SILVERLIGHT
                str = "/Syncfusion.Grid.Wpf;component/GridDataControl/Control/Themes/" + str + "Style.xaml";
#else
                str = "/Syncfusion.Grid.Silverlight;component/GridDataControl/Control/Themes/" + str + "Style.xaml";
#endif
                if (RemoveDictionaryIfExist(element, str))
                {
                    ResourceDictionary dictionary = new ResourceDictionary();
                    dictionary.Source = new Uri(str, UriKind.RelativeOrAbsolute);
                    element.Resources.MergedDictionaries.Add(dictionary);
                }
            }
            else
                RemoveDictionaryIfExist(element, this.treeControl.VisualStyle.ToString());
            return element.Resources;
        }

        private static bool RemoveDictionaryIfExist(FrameworkElement element, string dictionary)
        {
            if (element != null)
            {
                for (int i = 0; i < element.Resources.MergedDictionaries.Count; i++)
                {
                    var rdic = element.Resources.MergedDictionaries[i];
                    if (rdic.Source.ToString() == dictionary)
                    {
                        return false;
                    }
#if SILVERLIGHT
                        else if (rdic.Source.ToString().Contains("/Syncfusion.Grid.Silverlight;component/GridDataControl/Control/Themes/"))
#else
                    else if (rdic.Source.ToString().Contains("/Syncfusion.Grid.Wpf;component/GridDataControl/Control/Themes/"))
#endif
                    {
                        element.Resources.MergedDictionaries.RemoveAt(i);
                        i--;
                    }
                }

                return true;
            }

            return false;
        }
    }
    #endregion
}
