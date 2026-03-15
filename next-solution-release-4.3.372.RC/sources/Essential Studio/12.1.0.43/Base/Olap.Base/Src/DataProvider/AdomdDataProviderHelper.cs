#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Data;
using Syncfusion.Olap.Reports;
using MASAC = Microsoft.AnalysisServices.AdomdClient;
using Syncfusion.Olap.MDXQueryBuilder;


namespace Syncfusion.Olap.DataProvider
{
    /// <summary>
    /// Helper class for the ADOMD data provider.
    /// </summary>
    public class AdomdDataProviderHelper
    {
        /// <summary>
        /// Executes the data table.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="report">The report.</param>
        /// <returns>A DataTable object.</returns>
        public static DataTable ExecuteDataTable(string connectionString, OlapReport report)
        {
            string mdxQuery = QueryBuilderEngine.GenerateQueryEx(GetMDXQuerySpecification(report));

            DataTable dataTable = new DataTable();
            
            
            using(MASAC.AdomdConnection adomdConnection = new MASAC.AdomdConnection(connectionString))
            {
                if (adomdConnection.State != ConnectionState.Open)
                {
                    adomdConnection.Open();
                }
                using (MASAC.AdomdCommand adomdCommand = new MASAC.AdomdCommand(mdxQuery, adomdConnection))
                {
                    using (MASAC.AdomdDataAdapter adomdDataAdapter = new MASAC.AdomdDataAdapter(adomdCommand))
                    {
                        adomdDataAdapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }

        private static MDXQuerySpecification GetMDXQuerySpecification(OlapReport report)
        {
            MDXQuerySpecification query = new MDXQuerySpecification();
            query.CubeName = report.CurrentCubeName;
            ////query.ShowGrandTotal = !this.CurrentReport.ShowGrandTotal;
            query.ShowEmptyRowData = report.ShowEmptyRowData;
            query.ShowEmptyColumnData = report.ShowEmptyColumnData;
            query.EngineVersion = report.EngineVersion;
            if (report.CategoricalElements.Count > 0)
            {
                foreach (Item item in report.CategoricalElements)
                {
                    //// Reversing the Axis Posting
                    if (report.TogglePivot)
                    {
                        item.Axis = AxisPosition.Series;
                    }
                    else
                    {
                        item.Axis = AxisPosition.Categorical;
                    }

                    query.Select.Items.Add(item);
                }

                foreach (Item item in report.SeriesElements)
                {
                    if (report.TogglePivot)
                    {
                        item.Axis = AxisPosition.Categorical;
                    }
                    else
                    {
                        item.Axis = AxisPosition.Series;
                    }

                    query.Select.Items.Add(item);
                }

                foreach (Item item in report.SlicerElements)
                {
                    query.Slicer.Items.Add(item);
                }

                if (report.FilterElements.Count > 0)
                {
                    if (report.TogglePivot)
                    {
                        Items clonedFilterElements = report.FilterElements.Clone();
                        ToggleElementAxis(clonedFilterElements);
                        foreach (Item item in clonedFilterElements)
                        {
                            query.Filter.Items.Add(item);
                        }
                    }
                    else
                    {
                        foreach (Item item in report.FilterElements)
                        {
                            query.Filter.Items.Add(item);
                        }
                    }
                }

                foreach (Item item in report.CalculatedMembers)
                {
                    query.With.Items.Add(item);
                }

                ////handled for adding subsetElement into Items
                if (report.CategoricalElements.SubSetElement != null)
                {
                    query.Select.Items.Add(new Item { ElementValue = report.CategoricalElements.SubSetElement, Axis = AxisPosition.Categorical });
                }

                if (report.SeriesElements.SubSetElement != null)
                {
                    query.Select.Items.Add(new Item { ElementValue = report.SeriesElements.SubSetElement, Axis = AxisPosition.Series });
                }
                query.IsPagingEnabled = report.EnablePaging;
                
                if (report.EnablePaging)
                {
                    query.Page.CategorialCurrentPage = report.PagerOptions.CategorialCurrentPage;
                    query.Page.CategorialPageSize = report.PagerOptions.CategorialPageSize;
                    query.Page.SeriesCurrentPage = report.PagerOptions.SeriesCurrentPage;
                    query.Page.SeriesPageSize = report.PagerOptions.SeriesPageSize;
                }

                return query;
            }

            return null;
        }

        private static void ToggleElementAxis(Items items)
        {
            foreach (var item in items)
            {
                if (item.Axis == AxisPosition.Categorical)
                {
                    item.Axis = AxisPosition.Series;
                }
                else if (item.Axis == AxisPosition.Series)
                {
                    item.Axis = AxisPosition.Categorical;
                }
            }
        }

    }
}
