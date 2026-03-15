//-------------------------------------------------------------------------------------------------
// <copyright file="PivotElements.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;


#if !SILVERLIGHT
using Syncfusion.Olap.Reports;
namespace Syncfusion.Olap.Engine
#else

using Syncfusion.OlapSilverlight.Data;
using Syncfusion.OlapSilverlight.Reports;
namespace Syncfusion.OlapSilverlight.Engine
#endif
{
    /// <summary>
    /// Represents the Pivot Element information.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class PivotElements
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PivotElements"/> class.
        /// </summary>
        /// <param name="olapReport">The OLAP report.</param>
        public PivotElements(OlapReport olapReport)
        {
            this.Report = olapReport;
            this.ColumnItems = new List<string>();
            this.SeriesItems = new List<string>();
            this.Summaries = new List<SummaryInfo>();
            this.GetElements();
        }

        /// <summary>
        /// Gets or sets a value indicating whether [expand all].
        /// </summary>
        /// <value><c>true</c> if [expand all]; otherwise, <c>false</c>.</value>
        public bool ExpandAll { get; set; }

        /// <summary>
        /// Gets or sets the report.
        /// </summary>
        /// <value>The report.</value>
        public OlapReport Report { get; set; }

        /// <summary>
        /// Gets or sets the column items.
        /// </summary>
        /// <value>The column items.</value>
        public List<string> ColumnItems { get; set; }

        /// <summary>
        /// Gets or sets the series items.
        /// </summary>
        /// <value>The series items.</value>
        public List<string> SeriesItems { get; set; }

        /// <summary>
        /// Gets or sets the summaries.
        /// </summary>
        /// <value>The summaries.</value>
        public List<SummaryInfo> Summaries { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is row summary.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is row summary; otherwise, <c>false</c>.
        /// </value>
        public bool IsRowSummary { get; set; }

        /// <summary>
        /// Gets or sets the summary string count.
        /// </summary>
        /// <value>The summary string count.</value>
        public int SummaryStringCount { get; set; }

        /// <summary>
        /// Gets the elements.
        /// </summary>
        /// <returns></returns>
        public PivotElements GetElements()
        {
            if (this.Report != null)
            {
                //// Getting categorical items
                foreach (Item item in this.Report.CategoricalElements)
                {
                    if (item.ElementValue is SummaryElements)
                    {
                        SummaryElements summaryElements = item.ElementValue as SummaryElements;
                        if (summaryElements.SummaryCollection != null)
                        {
                            this.IsRowSummary = false;
                            foreach (SummaryInfo summary in summaryElements.SummaryCollection)
                            {
                                this.Summaries.Add(summary);
                            }
                        }
                    }
                    else if (item.ItemName != null && item.ItemName != string.Empty)
                    {
                        this.ColumnItems.Add(item.ItemName);
                    }
                    else if (item.ElementValue != null && item.ElementValue is DimensionElement)
                    {
                        DimensionElement dimensionElement = item.ElementValue as DimensionElement;

                        if (dimensionElement.Hierarchy != null)
                        {
                            foreach (LevelElement element in dimensionElement.Hierarchy.LevelElements)
                            {
                                this.ColumnItems.Add(element.Name);
                            }
                        }
                        //this.ColumnItems.Add(item.ElementValue.Name);
                    }
                }
                //// getting series items
                foreach (Item item in this.Report.SeriesElements)
                {
                    if (item.ElementValue is SummaryElements)
                    {
                        SummaryElements summaryElements = item.ElementValue as SummaryElements;
                        if (summaryElements.SummaryCollection != null)
                        {
                            this.IsRowSummary = true;
                            foreach (SummaryInfo summary in summaryElements.SummaryCollection)
                            {
                                this.Summaries.Add(summary);
                            }
                        }
                    }

                    if (item.ItemName != null && item.ItemName != string.Empty)
                    {
                        this.SeriesItems.Add(item.ItemName);
                    }
                    else if (item.ElementValue != null && item.ElementValue is DimensionElement)
                    {
                        DimensionElement dimensionElement = item.ElementValue as DimensionElement;

                        if (dimensionElement.Hierarchy != null)
                        {
                            foreach (LevelElement element in dimensionElement.Hierarchy.LevelElements)
                            {
# if !SILVERLIGHT
                                if (!this.ColumnItems.Exists(i => i == element.Name))
                                {
                                    this.SeriesItems.Add(element.Name);
                                }
                                else
                                {
                                    throw new Exception("Item " + element.Name + " already exists in Categories");
                                }
                            
#else
                                if (!CheckItemExists(element.Name, this.ColumnItems))
                                {
                                    this.SeriesItems.Add(element.Name);
                                }
                                else
                                {
                                    throw new Exception("Item " + element.Name + " already exists in Categories");
                                }
#endif
                            }
                        }
                    }
                }
                if (!this.Report.ShowExpanders)
                {
                    this.ExpandAll = true;
                }
                return this;
            }
            return null;
        }

        /// <summary>
        /// Checks whether the item exists in the collection
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        private bool CheckItemExists(string elementName, List<string> list)
        {
            foreach (string  item in list)
            {
                if (item == elementName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
