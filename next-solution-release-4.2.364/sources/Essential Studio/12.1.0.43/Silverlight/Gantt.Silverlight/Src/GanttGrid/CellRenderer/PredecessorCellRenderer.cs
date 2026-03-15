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
using Syncfusion.Windows.Controls.Grid;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Cells;
using System.Windows.Controls;
using System.Windows;
using Syncfusion.Windows.Controls.Scroll;
using System.Collections;
using System.Text.RegularExpressions;

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
    /// Renders a TextBlock control inside a grid cell.
    /// </summary>
    public class PredecessorCellRenderer : GridCellTextBoxCellRenderer
    {
        /// <summary>
        /// Gets the control text core.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="cellValue">The cell value.</param>
        /// <returns></returns>
        protected override string GetControlTextCore(GridRenderStyleInfo style, object cellValue)
        {
            return GetPredecessor(style);
        }

        /// <summary>
        /// Called when [save changes].
        /// </summary>
        /// <returns></returns>
        protected override bool OnSaveChanges()
        {
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
            return list;
        }
    }
}
