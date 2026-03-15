//-------------------------------------------------------------------------------------------------
// <copyright file="MenuAction.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Soap;
using System.Reflection;
using Syncfusion.Windows.Forms.Grid.Design;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Grid.Design.Actions
{
    /// <exclude/>
    internal class ExitProgram : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            MainWindow.Close();
        }
    }

    /// <exclude/>
    internal class Find : GridDesignerBasicAction
    {
        GridDesignerPreviewGrid grid = null;

        public override void InvokeAction(object sender, EventArgs e)
        {
            grid = MainWindow.Grid as GridDesignerPreviewGrid;
            if (grid != null)
            {
                GridFindReplaceDialog frDialog = GridFindReplaceDialog.Instance;
                frDialog.SetState(grid.GridFindReplaceDialogSink, string.Empty, false);
                frDialog.Show();
            }
        }
    }

    /// <exclude/>
    internal class FindNext : GridDesignerBasicAction
    {
        GridDesignerPreviewGrid grid = null;

        public override void InvokeAction(object sender, EventArgs e)
        {
            grid = MainWindow.Grid as GridDesignerPreviewGrid;
            if (grid != null)
            {
                GridFindReplaceDialog frDialog = GridFindReplaceDialog.Instance;
                frDialog.FindNext();
            }
        }
    }

    /// <exclude/>
    internal class Replace : GridDesignerBasicAction
    {
        GridDesignerPreviewGrid grid = null;

        public override void InvokeAction(object sender, EventArgs e)
        {
            grid = MainWindow.Grid as GridDesignerPreviewGrid;
            if (grid != null)
            {
                GridFindReplaceDialog frDialog = GridFindReplaceDialog.Instance;
                frDialog.SetState(grid.GridFindReplaceDialogSink, string.Empty, true);
                frDialog.Show();
            }
        }
    }

    /// <exclude/>
    internal class SetCellBackgroundImage : GridDesignerBasicAction
    {
        GridDesignerPreviewGrid grid = null;
        public override void InvokeAction(object sender, EventArgs e)
        {
            grid = MainWindow.Grid as GridDesignerPreviewGrid;
            if (grid != null)
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "BMP (*.bmp)|*.bmp | JPG (*.jpg)|*.jpg |All Files (*.*)|*.*";
                dlg.Title = "Select Image";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Image image = null;
                    try
                    {
                        image = Image.FromStream(File.OpenRead(dlg.FileName));
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("Unable to load image:");
                        Trace.WriteLine(ex.ToString());
                        MessageBoxAdv.Show(SR.GetString(SR.UnabletoLoadImage), SR.GetString(SR.LoadFailed), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int row = 0;
                    int col = 0;
                    GridRangeInfoList list = null;
                    grid.Selections.GetSelectedRanges(out list, true);
                    foreach (GridRangeInfo rangeInfo in list)
                    {
                        rangeInfo.GetFirstCell(out row, out col);
                        if (row > 0 && col > 0)
                        {
                            grid[row, col].BackgroundImage = image;
                        }

                        while (rangeInfo.GetNextCell(ref row, ref col))
                        {
                            grid[row, col].BackgroundImage = image;
                        }
                    }
                }
            }
        }
    }

    /// <exclude/>
    internal class ClearCells : GridDesignerBasicAction
    {
        GridDesignerPreviewGrid grid = null;
        public override void InvokeAction(object sender, EventArgs e)
        {
            grid = MainWindow.Grid as GridDesignerPreviewGrid;
            if (grid != null)
            {
                GridRangeInfoList list = null;
                grid.Selections.GetSelectedRanges(out list, true);
                if (list != null)
                {
                    grid.ClearCells(list, false);
                }
            }
        }
    }

    /// <exclude/>
    internal class PrintPreviewFile : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                try
                {
                    GridPrintDocument pd = new GridPrintDocument(grid, true); ////Assumes the default printer

                    if (PrinterSettings.storedPageSettings != null)
                    {
                        pd.DefaultPageSettings = PrinterSettings.storedPageSettings;
                    }

                    PrintPreviewDialog dlg = new PrintPreviewDialog();
                    dlg.Document = pd;
                    dlg.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBoxAdv.Show(SR.GetString(SR.AnErrorOccurredAttemptingToPreviewtheFiletoprint) + ex.Message);
                }
            }
        }
    }

    /// <exclude/>
    internal class PrintFile : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                try
                {
                    GridPrintDocument pd = new GridPrintDocument(grid); ////Assumes the default printer

                    if (PrinterSettings.storedPageSettings != null)
                    {
                        pd.DefaultPageSettings = PrinterSettings.storedPageSettings;
                    }

                    PrintDialog dlg = new PrintDialog();
                    dlg.Document = pd;
                    dlg.AllowSelection = true;
                    dlg.AllowSomePages = true;
                    DialogResult result = dlg.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        pd.Print();
                    }
                }
                catch (Exception ex)
                {
                    MessageBoxAdv.Show(SR.GetString(SR.AnErrorOccurredAttemptingToPreviewtheFiletoprint) + ex.Message);
                }
            }
        }
    }

    /// <exclude/>
    internal class PrinterSettings : GridDesignerBasicAction
    {
        public static PageSettings storedPageSettings = null;

        public override void InvokeAction(object sender, EventArgs e)
        {
            try
            {
                PageSetupDialog psDlg = new PageSetupDialog();

                if (storedPageSettings == null)
                {
                    storedPageSettings = new PageSettings();
                }

                psDlg.PageSettings = storedPageSettings;
                psDlg.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBoxAdv.Show(SR.GetString(SR.AnErrorOccurred) + ex.Message);
            }
        }
    }

    /// <exclude/>
    internal class BlackWhiteGrid : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                grid.Model.Properties.BlackWhite = !grid.Model.Properties.BlackWhite;
                grid.Model.Refresh();
            }
        }
    }

    /// <exclude/>
    internal class AlphaBlending : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                grid.SupportsTransparentBackColor = !grid.SupportsTransparentBackColor;
                if (grid.SupportsTransparentBackColor)
                {
                    grid.Model.TableStyle.Interior = new BrushInfo(Color.FromArgb(50, Color.White));
                }
                else
                {
                    grid.Model.TableStyle.ResetInterior();
                }

                grid.Model.Options.TransparentBackground = !grid.SupportsTransparentBackColor && grid.BackgroundImage == null;
            }
        }
    }

    /// <exclude/>
    internal class TransparentBackground : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                grid.Model.Options.TransparentBackground = !grid.Model.Options.TransparentBackground;
            }
        }
    }

    /// <exclude/>
    internal class ResetTransparency : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                grid.Model.Options.TransparentBackground = false;
                grid.SupportsTransparentBackColor = false;
                grid.BackgroundImage = null;
                grid.BackColor = Color.White;
            }
        }
    }

    /// <exclude/>
    internal class FormatBaseStylesMap : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                GridBaseStylesMap.ShowGridBaseStylesMapDialog(grid.Model, "BaseStylesMap");
                grid.Model.Refresh();
            }
        }
    }

    /// <exclude/>
    internal class ToggleBold : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
#if SyncfusionFramework2_0
            ////for 2.0 framework 
            if (typeof(ToolStripButton).IsInstanceOfType(sender))
            {
                if (((ToolStripButton)sender).Checked)
                {
                    FontStyleBold.PerformAction(true, grid);
                }
                else
                {
                    FontStyleBold.PerformAction(false, grid);
                }
            }
#endif
            ////needed for WinForms toolbar
            if (e.GetType() == typeof(ToolBarButtonClickEventArgs))
            {
                if (((ToolBarButtonClickEventArgs)e).Button.Pushed)
                {
                    FontStyleBold.PerformAction(true, grid);
                }
                else
                {
                    FontStyleBold.PerformAction(false, grid);
                }
            }
        }
    }

    /// <exclude/>
    internal class FontStyleBold : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            FontStyleBold.PerformAction(true, grid);
        }

        public static void PerformAction(bool bold, GridControlBase grid)
        {
            GridStyleInfo style = new GridStyleInfo();
            style.Font.Bold = bold;
            ApplyStyle(style, grid);
        }
    }

    /// <exclude/>
    internal class ToggleItalic : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;

#if SyncfusionFramework2_0
            ////for 2.0 framework 
            if (typeof(ToolStripButton).IsInstanceOfType(sender))
            {
                if (((ToolStripButton)sender).Checked)
                {
                    FontStyleItalic.PerformAction(true, grid);
                }
                else
                {
                    FontStyleItalic.PerformAction(false, grid);
                }
            }
#endif
            ////needed for WinForms toolbar
            if (e.GetType() == typeof(ToolBarButtonClickEventArgs))
            {
                if (((ToolBarButtonClickEventArgs)e).Button.Pushed)
                {
                    FontStyleItalic.PerformAction(true, grid);
                }
                else
                {
                    FontStyleItalic.PerformAction(false, grid);
                }
            }
        }
    }

    /// <exclude/>
    internal class FontStyleItalic : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;

            FontStyleItalic.PerformAction(true, grid);
        }

        public static void PerformAction(bool italic, GridControlBase grid)
        {
            GridStyleInfo style = new GridStyleInfo();
            style.Font.Italic = italic;
            ApplyStyle(style, grid);
        }
    }

    /// <exclude/>
    internal class ToggleUnderline : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;

#if SyncfusionFramework2_0
            ////for 2.0 framework 
            if (typeof(ToolStripButton).IsInstanceOfType(sender))
            {
                if (((ToolStripButton)sender).Checked)
                {
                    FontStyleUnderline.PerformAction(true, grid);
                }
                else
                {
                    FontStyleUnderline.PerformAction(false, grid);
                }
            }
#endif
            ////needed for WinForms toolbar
            if (e.GetType() == typeof(ToolBarButtonClickEventArgs))
            {
                if (((ToolBarButtonClickEventArgs)e).Button.Pushed)
                {
                    FontStyleUnderline.PerformAction(true, grid);
                }
                else
                {
                    FontStyleUnderline.PerformAction(false, grid);
                }
            }
        }
    }

    /// <exclude/>
    internal class FontStyleUnderline : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            PerformAction(true, grid);
        }

        public static void PerformAction(bool underline, GridControlBase grid)
        {
            GridStyleInfo style = new GridStyleInfo();
            style.Font.Underline = underline;
            ApplyStyle(style, grid);
        }
    }

    /// <exclude/>
    internal class ToggleLeft : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;

#if SyncfusionFramework2_0
            ////for 2.0 framework 
            if (typeof(ToolStripButton).IsInstanceOfType(sender))
            {
                if (((ToolStripButton)sender).Checked)
                {
                    AlignLeft.PerformAction(grid);
                    MainWindow.SetAlignLeftState();
                }
            }
#endif
            ////needed for WinForms toolbar
            if (e.GetType() == typeof(ToolBarButtonClickEventArgs))
            {
                if (((ToolBarButtonClickEventArgs)e).Button.Pushed)
                {
                    AlignLeft.PerformAction(grid);
                    MainWindow.SetAlignLeftState();
                }
            }
        }
    }

    /// <exclude/>
    internal class AlignLeft : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            AlignLeft.PerformAction(grid);
        }

        public static void PerformAction(GridControlBase grid)
        {
            GridStyleInfo style = new GridStyleInfo();
            style.HorizontalAlignment = GridHorizontalAlignment.Left;
            ApplyStyle(style, grid);
        }
    }

    /// <exclude/>
    internal class ToggleCenter : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;

#if SyncfusionFramework2_0
            ////for 2.0 framework 
            if (typeof(ToolStripButton).IsInstanceOfType(sender))
            {
                if (((ToolStripButton)sender).Checked)
                {
                    AlignCenter.PerformAction(grid);
                    MainWindow.SetAlignCenterState();
                }
                else
                {
                    AlignLeft.PerformAction(grid);
                    MainWindow.SetAlignLeftState();
                }
            }
#endif
            ////needed for WinForms toolbar
            if (e.GetType() == typeof(ToolBarButtonClickEventArgs))
            {
                if (((ToolBarButtonClickEventArgs)e).Button.Pushed)
                {
                    AlignCenter.PerformAction(grid);
                    MainWindow.SetAlignCenterState();
                }
                else
                {
                    AlignLeft.PerformAction(grid);
                    MainWindow.SetAlignLeftState();
                }
            }
        }
    }

    /// <exclude/>
    internal class AlignCenter : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            AlignCenter.PerformAction(grid);
        }

        public static void PerformAction(GridControlBase grid)
        {
            GridStyleInfo style = new GridStyleInfo();
            style.HorizontalAlignment = GridHorizontalAlignment.Center;
            ApplyStyle(style, grid);
        }
    }

    /// <exclude/>
    internal class ToggleRight : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;

#if SyncfusionFramework2_0
            ////for 2.0 framework 
            if (typeof(ToolStripButton).IsInstanceOfType(sender))
            {
                if (((ToolStripButton)sender).Checked)
                {
                    AlignRight.PerformAction(grid);
                    MainWindow.SetAlignRightState();
                }
                else
                {
                    AlignLeft.PerformAction(grid);
                    MainWindow.SetAlignLeftState();
                }
            }
#endif
            ////needed for WinForms toolbar
            if (e.GetType() == typeof(ToolBarButtonClickEventArgs))
            {
                if (((ToolBarButtonClickEventArgs)e).Button.Pushed)
                {
                    AlignRight.PerformAction(grid);
                    MainWindow.SetAlignRightState();
                }
                else
                {
                    AlignLeft.PerformAction(grid);
                    MainWindow.SetAlignLeftState();
                }
            }
        }
    }

    /// <exclude/>
    internal class AlignRight : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            AlignRight.PerformAction(grid);
        }

        public static void PerformAction(GridControlBase grid)
        {
            GridStyleInfo style = new GridStyleInfo();
            style.HorizontalAlignment = GridHorizontalAlignment.Right;
            ApplyStyle(style, grid);
        }
    }

    /// <exclude/>
    internal class ResizeColumns : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                foreach (GridRangeInfo range in grid.Model.Selections)
                {
                    grid.Model.ColWidths.ResizeToFit(range, GridResizeToFitOptions.ResizeCoveredCells);
                }
            }
        }
    }

    /// <exclude/>
    internal class ResizeRows : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                foreach (GridRangeInfo range in grid.Model.Selections)
                {
                    grid.Model.RowHeights.ResizeToFit(range, GridResizeToFitOptions.ResizeCoveredCells);
                }
            }
        }
    }

    /// <exclude/>
    internal class RemoveRows : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                GridRangeInfoList ranges = grid.Model.Selections.Ranges.GetRowRanges(GridRangeInfoType.Rows);
                if (ranges.Count == 0)
                {
                    return;
                }

                grid.Model.CommandStack.BeginTrans("Remove Rows");

                foreach (GridRangeInfo range in ranges)
                {
                    grid.Model.Rows.RemoveRange(range.Top, range.Bottom);
                }

                grid.Model.CommandStack.CommitTrans();
            }
        }
    }
    internal class CustomHelp : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                grid.Model.CommandStack.BeginTrans("CustomHelp");

                grid.Model.CommandStack.CommitTrans();
            }
        }
    }

    internal class AboutObjectGrid : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {

                grid.Model.CommandStack.BeginTrans("AboutObjectGrid");

                grid.Model.CommandStack.CommitTrans();
            }
        }
    }
    /// <exclude/>
    internal class RemoveCols : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                GridRangeInfoList ranges = grid.Model.Selections.Ranges.GetColRanges(GridRangeInfoType.Cols);
                if (ranges.Count == 0)
                {
                    return;
                }

                grid.Model.CommandStack.BeginTrans("Remove Columns");

                foreach (GridRangeInfo range in ranges)
                {
                    grid.Model.Cols.RemoveRange(range.Left, range.Right);
                }

                grid.Model.CommandStack.CommitTrans();
            }
        }
    }

    /// <exclude/>
    internal class InsertRows : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                int rowIndex, colIndex;
                if (grid.CurrentCell.GetCurrentCell(out rowIndex, out colIndex))
                {
                    grid.Model.Rows.InsertRange(rowIndex, 1, null);
                    grid.CurrentCell.MoveTo(rowIndex + 1, colIndex);
                }
            }
        }
    }

    /// <exclude/>
    internal class InsertCols : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                int rowIndex, colIndex;
                if (grid.CurrentCell.GetCurrentCell(out rowIndex, out colIndex))
                {
                    grid.Model.Cols.InsertRange(colIndex, 1, null);
                    grid.CurrentCell.MoveTo(rowIndex, colIndex + 1);
                }
            }
        }
    }

    /// <exclude/>
    internal class ToggleBanneredRange : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
#if SyncfusionFramework2_0
                ////for 2.0 framework 
                if (typeof(ToolStripButton).IsInstanceOfType(sender))
                {
                    if (((ToolStripButton)sender).Checked)
                    {
                        MakeBanneredRange.PerformAction(grid);
                    }
                    else
                    {
                        ResetBanneredRange.PerformAction(grid);
                    }
                }
#endif
                ////needed for WinForms toolbar
                if (e.GetType() == typeof(ToolBarButtonClickEventArgs))
                {
                    if (((ToolBarButtonClickEventArgs)e).Button.Pushed)
                    {
                        MakeBanneredRange.PerformAction(grid);
                    }
                    else
                    {
                        ResetBanneredRange.PerformAction(grid);
                    }
                }

                MainWindow.MarkDirty(true); 
            }
        }
    }

    /// <exclude/>
    internal class MakeBanneredRange : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                MakeBanneredRange.PerformAction(grid);
                MainWindow.MarkDirty(true); 
            }
        }

        public static void PerformAction(GridControlBase grid)
        {
            GridRangeInfo range = grid.Model.Selections.Ranges.ActiveRange;
            grid.Model.BanneredRanges.Add(range);
        }
    }

    /// <exclude/>
    internal class ResetBanneredRange : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                ResetBanneredRange.PerformAction(grid);
                MainWindow.MarkDirty(true); 
            }
        }

        public static void PerformAction(GridControlBase grid)
        {
            GridRangeInfoList rl;
            if (grid.Selections.GetSelectedRanges(out rl, true))
            {
                GridRangeInfo range = rl.ActiveRange;
                if (!range.IsEmpty)
                {
                    range = grid.Model.BanneredRanges.Merge(range);
                }

                grid.Model.BanneredRanges.Remove(range);
                grid.Refresh();
            }
        }
    }

    /// <exclude/>
    internal class ToggleCoverCell : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
#if SyncfusionFramework2_0
                ////for 2.0 framework 
                if (typeof(ToolStripButton).IsInstanceOfType(sender))
                {
                    if (((ToolStripButton)sender).Checked)
                    {
                        CoverCell.PerformAction(grid);
                    }
                    else
                    {
                        ResetCover.PerformAction(grid);
                    }
                }
#endif
                ////needed for WinForms toolbar
                if (e.GetType() == typeof(ToolBarButtonClickEventArgs))
                {
                    if (((ToolBarButtonClickEventArgs)e).Button.Pushed)
                    {
                        CoverCell.PerformAction(grid);
                    }
                    else
                    {
                        ResetCover.PerformAction(grid);
                    }
                }

                MainWindow.MarkDirty(true);
            }
        }
    }

    /// <exclude/>
    internal class CoverCell : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                CoverCell.PerformAction(grid);
                MainWindow.MarkDirty(true); 
            }
        }

        public static void PerformAction(GridControlBase grid)
        {
            GridRangeInfo range = grid.Model.Selections.Ranges.ActiveRange;
            grid.Model.CoveredRanges.Add(range);
        }
    }

    /// <exclude/>
    internal class ResetCover : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                ResetCover.PerformAction(grid);
                MainWindow.MarkDirty(true); 
            }
        }

        public static void PerformAction(GridControlBase grid)
        {
            GridRangeInfoList rl;
            if (grid.Selections.GetSelectedRanges(out rl, true))
            {
                GridRangeInfo range = rl.ActiveRange;
                if (!range.IsEmpty)
                {
                    range = grid.Model.CoveredRanges.Merge(range);
                }

                grid.Model.CoveredRanges.Remove(range);
            }
        }
    }

    /// <exclude/>
    internal class FreezeColumns : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                grid.Model.Cols.FreezeSelection();
                MainWindow.MarkDirty(true);
                GridDesignerMain.syncProps.FrozenColCount = grid.Model.Cols.FrozenCount;
            }
        }
    }

    /// <exclude/>
    internal class ReleaseColumns : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                grid.Model.Cols.RestoreFrozen();
                MainWindow.MarkDirty(true); 
            }
        }
    }

    /// <exclude/>
    internal class FreezeRows : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                grid.Model.Rows.FreezeSelection();
                MainWindow.MarkDirty(true); 
                GridDesignerMain.syncProps.FrozenRowCount = grid.Model.Rows.FrozenCount;
            }
        }
    }

    /// <exclude/>
    internal class ReleaseRows : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                grid.Model.Rows.RestoreFrozen();
                MainWindow.MarkDirty(true); 
            }
        }
    }

    /// <exclude/>
    internal class SaveToXml : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "XML (*.xml)|*.xml";
            dlg.Title = "Save Grid Properties as Xml";
            dlg.OverwritePrompt = true;
            dlg.AddExtension = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(GridSyncProperties));

                    GridStyleInfoStore.RegisterXmlSerializer(typeof(GridSyncProperties), serializer);

                    GridDesignerMain.syncProps.ResetStyleGroups();
                    GridStylesParser.ParsePropertyStore(GridDesignerMain.syncProps.Store);

                    GridDesignerMain.syncProps.Cells.InitializeFrom(MainWindow.Grid);

                    FileStream stream = File.Create(dlg.FileName);
                    if (stream != null)
                    {
                        serializer.Serialize(stream, GridDesignerMain.syncProps);
                        stream.Close();
                    }

                    MessageBoxAdv.Show(dlg.FileName, SR.GetString(SR.FileSaved), MessageBoxButtons.OK);
                }
                catch (Exception ex)
                {
                    MessageBoxAdv.Show(ex.ToString() + "\n\n" + ex.InnerException.ToString());
                }
            }
        }
    }

    /// <exclude/>
    internal class SetGrid : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            SetGrid.PerformAction(MainWindow);
            MainWindow.MarkDirty(false); 
        }

        public static void PerformAction(GridFrame mainWindow)
        {
            try
            {
                mainWindow.Syncronizer.SyncOriginal();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }
    }

    /// <exclude/>
    internal class LoadFromXml : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "XML (*.xml)|*.xml";
            dlg.Title = "Load Grid Properties from Xml";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    this.MainWindow.Cursor = Cursors.WaitCursor;
                    FileStream stream = File.OpenRead(dlg.FileName);
                    if (stream != null)
                    {
                        System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Design.GridSyncProperties));
                        object result = serializer.Deserialize(stream);
                        stream.Close();
                        MainWindow.Syncronizer.InitializeGridSync(result as Design.GridSyncProperties);
                        MainWindow.pgrpropertyGrid1.SelectedObject = GridDesignerMain.syncProps;
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
                finally
                {
                    this.MainWindow.Cursor = Cursors.Default;
                }
            }
        }

        private void serializer_UnknownElement(object sender, XmlElementEventArgs e)
        {
            Trace.WriteLine(e);
        }
    }
    
    /// <exclude/>
    internal class LoadFromTemplate : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = SR.GetString(SR.GridControlName);
            dlg.Filter = "Essential Grid templates (*.egt)|*.egt|All files (*.*)|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    GridModel model = GridModel.LoadSoap(dlg.FileName);
                    MainWindow.Grid.Model = model;

                    ////force the modified flag
                    MainWindow.MarkDirty(true);
                }
                catch (Exception ex)
                {
                    MessageBoxAdv.Show(SR.GetString(SR.UnabletoLoadtheSavedTemplate), SR.GetString(SR.LoadFailure), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Trace.WriteLine(ex.ToString());
                }
                finally
                {
                }
            }
        }
    }

    /// <exclude/>
    internal class SaveToTemplate : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Title = SR.GetString(SR.GridControlName);
                dlg.Filter = "Essential Grid templates (*.egt)|*.egt|All files (*.*)|*.*";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        grid.Model.SaveSoap(dlg.FileName);
                        MessageBoxAdv.Show(dlg.FileName, SR.GetString(SR.FileSaved), MessageBoxButtons.OK);
                    }
                    catch (Exception ex)
                    {
                        MessageBoxAdv.Show(ex.ToString());
                    }
                    finally
                    {
                    }
                }
            }
        }
    }

    ////////////////// Edit Menu

    /// <exclude/>
    internal class UndoAction : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                grid.Model.CommandStack.Undo();
            }
        }
    }

    /// <exclude/>
    internal class RedoAction : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                grid.Model.CommandStack.Redo();
            }
        }
    }

    /// <exclude/>
    internal class CutAction : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                grid.Model.CutPaste.Cut();
            }
        }
    }

    /// <exclude/>
    internal class CopyAction : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                grid.Model.CutPaste.Copy();
            }
        }
    }

    /// <exclude/>
    internal class PasteAction : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                grid.Model.CutPaste.Paste();
            }
        }
    }

    /// <exclude/>
    internal class DeleteAction : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                grid.Model.Clear(true);
            }
        }
    }

    /// <exclude/>
    internal class SelectAllAction : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                grid.Selections.Clear();
                grid.Selections.Add(GridRangeInfo.Table());
            }
        }
    }

    /// <exclude/>
    internal class ExcelLikeSelection : GridDesignerBasicAction
    {
        public override void InvokeAction(object sender, EventArgs e)
        {
            GridControlBase grid = MainWindow.Grid;
            if (grid != null)
            {
                grid.Model.BeginUpdate(BeginUpdateOptions.None);
                bool bSelect = !grid.Model.Options.ExcelLikeCurrentCell;
                grid.Model.Options.ExcelLikeCurrentCell = bSelect;
                grid.Model.Options.ExcelLikeSelectionFrame = bSelect;
                GridRangeInfo range = grid.Selections.Ranges.ActiveRange;
                if (!range.IsEmpty)
                {
                    grid.CurrentCell.ConfirmChanges();
                    grid.CurrentCell.Deactivate(true);
                    grid.CurrentCell.MoveTo(range.Top, range.Left, GridSetCurrentCellOptions.NoSelectRange);
                }

                grid.Model.EndUpdate(false);
                grid.Model.Refresh();
            }
        }
    }
}
