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
using Syncfusion.Windows.Controls.Grid;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Cells;
using System.Text.RegularExpressions;
using System.Collections;
using System.ComponentModel;
using System.Collections.Specialized;

namespace Syncfusion.Windows.Controls.Gantt.Grid
{
    /// <summary>
    /// Implements the model part of a Predecessor cell.
    /// </summary>
    public class PredecessorCellModel : GridCellModel<PredecessorCellRenderer>
    {
        /// <summary>
        /// Parses the display text and converts it into a cell value to be stored in the style object.
        /// GridStyleInfo.CultureInfo is used for parsing the string.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="text">The input text to be parsed.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText</param>
        /// <returns>
        /// True if value was parsed correctly and saved in style object as <see cref="GridStyleInfo.CellValue"/>; False otherwise.
        /// </returns>
        public override bool ApplyFormattedText(GridStyleInfo style, string text, int textInfo)
        {
            // This is to avoid exception on value converter,
            // It will try to conver the text to the collection type that is used in underlying source
            return false;
        }
    }

    /// <summary>
    /// Renders a TextBox control inside a grid cell.
    /// </summary>
    public class PredecessorCellRenderer : GridCellTextBoxRenderer
    {
        RowColumnIndex InternalRowColIndex = new RowColumnIndex(0, 0);

        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="rca">The rca.</param>
        /// <param name="style">The style.</param>
        protected override void OnRender(DrawingContext dc, RenderCellArgs rca, GridRenderStyleInfo style)
        {
            // These codes are to listen and refresh the cell when the change occur in underlying source
            if (style.CellValue != null && (style.CellValue is INotifyCollectionChanged))
            {
                var sourceList = style.CellValue as INotifyCollectionChanged;

                sourceList.CollectionChanged -= sourceList_CollectionChanged;
                sourceList.CollectionChanged += sourceList_CollectionChanged;
            }
            // This is to get the row col index on collection change event handler, because 
            // When the cell is not current cell we cant get the row and col index indise the renderer
            this.InternalRowColIndex = new RowColumnIndex(style.RowIndex, style.ColumnIndex);
            base.OnRender(dc, rca, style);
        }

        /// <summary>
        /// Handles the CollectionChanged event of the sourceList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        void sourceList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // To refresh the cell
            this.GridControl.InvalidateCell(GridRangeInfo.Cell(this.InternalRowColIndex.RowIndex,this.InternalRowColIndex.ColumnIndex));
        }

        /// <summary>
        /// Gets the control text core.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="cellValue">The cell value.</param>
        /// <returns></returns>
        protected override string GetControlTextCore(GridRenderStyleInfo style, object cellValue)
        {
            // Will return the text based on predecessor value
            return GetPredecessor(style);
        }

        /// <summary>
        /// Called when [save changes].
        /// </summary>
        /// <returns></returns>
        protected override bool OnSaveChanges()
        {
            // This will act as simillar to apply formatted text and update the cell value with the current text
            this.ApplyNewDependencyRelations(this.CurrentStyle, this.GetControlTextFromEditor());
            return base.OnSaveChanges();
        }

        /// <summary>
        /// Gets the predecessor.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns></returns>
        string GetPredecessor(GridRenderStyleInfo style)
        {
            string predecessors = string.Empty;

            if (!(style.CellValue is IEnumerable<Predecessor>))
                return string.Empty;

            foreach (Predecessor pre in (style.CellValue as IEnumerable<Predecessor>))
            {
                predecessors += predecessors.Length > 0 ? ", " + pre.GanttTaskIndex.ToString() : pre.GanttTaskIndex.ToString();

                switch (pre.GanttTaskRelationship)
                {
                    case GanttTaskRelationship.FinishToFinish:
                        predecessors += "FF";
                        break;
                    case GanttTaskRelationship.FinishToStart:
                        predecessors += "FS";
                        break;
                    case GanttTaskRelationship.StartToFinish:
                        predecessors += "SF";
                        break;
                    case GanttTaskRelationship.StartToStart:
                        predecessors += "SS";
                        break;
                }
            }
            return predecessors;
        }

        /// <summary>
        /// Applies the new dependency relations.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="predecessorText">The predecessor text.</param>
        /// <returns></returns>
        private bool ApplyNewDependencyRelations(GridStyleInfo style, string predecessorText)
        {
            IList preCollection = new List<Predecessor>();

            if (!string.IsNullOrEmpty(predecessorText))
            {
                string[] relations = predecessorText.Split(',');
                if (relations.Count() <= 0)
                    return false;

                 Regex regex = new Regex(@"^(\d+)(SS|SF|FS|FF)");

               // Below is the Regex to validate the predecessor with Lagging support
               // Regex reg = new Regex(@"^(\d+)(SS|SF|FS|FF)(\+\d+)?");

                foreach (string relation in relations)
                {
                    if (!regex.IsMatch(relation.Trim()))
                        continue;

                    Match validValue = regex.Match(relation.Trim());
                    string[] groups = regex.Split(validValue.Value);

                    if (groups.Count() < 4)
                        return false;
                    try
                    {
                        Predecessor predecessor = new Predecessor();
                        // The match group will always contain empy strin on 0th position hence we started from 1
                        predecessor.GanttTaskIndex = int.Parse(groups[1]);
                        switch (groups[2])
                        {
                            case "FF":
                                predecessor.GanttTaskRelationship = GanttTaskRelationship.FinishToFinish;
                                break;
                            case "FS":
                                predecessor.GanttTaskRelationship = GanttTaskRelationship.FinishToStart;
                                break;
                            case "SF":
                                predecessor.GanttTaskRelationship = GanttTaskRelationship.StartToFinish;
                                break;
                            case "SS":
                                predecessor.GanttTaskRelationship = GanttTaskRelationship.StartToStart;
                                break;
                        }

                        preCollection.Add(predecessor);
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
            }

            // Get the source list to update the new items
            IList sourceList = this.GetSourceList(style.CellValue);
            if (sourceList == null)
                return false;

            sourceList.Clear();
            foreach (Predecessor pre in preCollection)
                sourceList.Add(pre);

            // Assigning the new value
            return preCollection.Count > 0;
        }

        /// <summary>
        /// Gets the source list.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        private IList GetSourceList(object source)
        {
            IList list = null;
            if ((source as IList) != null)
            {
                list = source as IList;
            }
#if !SILVERLIGHT
            else if ((source as IListSource) != null)
            {
                var listSource = source as IListSource;
                list = listSource.GetList();
            }
#endif
            return list;
        }
    }
}
