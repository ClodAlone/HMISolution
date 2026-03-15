#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Linq;
using System.Data;
using System.Collections;
using Syncfusion.Olap.Reports;
using System;

namespace Syncfusion.Olap.Engine
{
    /// <summary>
    /// Helper class for DataRow objects or IEnumerable objects.
    /// </summary>
    public class DataRowHelper
    {
        /// <summary>
        /// Computes the sum.
        /// </summary>
        /// <param name="ienum">The IEnumerable object.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public static string ComputeSum(IEnumerable ienum, string columnName)
        {
            DataRow[] dataRowsCollection = ienum.Cast<DataRow>().ToArray<DataRow>();
            return ComputeSum(dataRowsCollection, columnName);
        }

        /// <summary>
        /// Computes the average.
        /// </summary>
        /// <param name="ienum">The IEnumerable object.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public static string ComputeAvg(IEnumerable ienum, string columnName)
        {
            DataRow[] dataRowCollection = ienum.Cast<DataRow>().ToArray<DataRow>();
            return ComputeAvg(dataRowCollection, columnName);
        }

        /// <summary>
        /// Computes the count.
        /// </summary>
        /// <param name="ienum">The IEnumerable object.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public static string ComputeCount(IEnumerable ienum, string columnName)
        {
            DataRow[] dataRowCollection = ienum.Cast<DataRow>().ToArray<DataRow>();
            return ComputeCount(dataRowCollection, columnName);
        }

        /// <summary>
        /// Computes the maximum element.
        /// </summary>
        /// <param name="ienum">The IEnumerable object.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public static string ComputeMax(IEnumerable ienum, string columnName)
        {
            DataRow[] dataRowCollection = ienum.Cast<DataRow>().ToArray<DataRow>();
            return ComputeMax(dataRowCollection, columnName);
        }

        /// <summary>
        /// Computes the minimum element.
        /// </summary>
        /// <param name="ienum">The IEnumerable object.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public static string ComputeMin(IEnumerable ienum, string columnName)
        {
            DataRow[] dataRowCollection = ienum.Cast<DataRow>().ToArray<DataRow>();
            return ComputeMin(dataRowCollection, columnName);
        }

        /// <summary>
        /// Computes the sum.
        /// </summary>
        /// <param name="dataRowsCollection">The data rows collection.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public static string ComputeSum(DataRow[] dataRowsCollection, string columnName)
        {
            DataTable dataTable = GetDataTable(dataRowsCollection);
            if (dataTable != null)
            {
                object result = null;
                if (dataRowsCollection[0][columnName].GetType().Name != "DateTime")
                    result = dataTable.Compute(GetSumExpression(columnName), "1=1");
                else
                {
                    result = 0;
                    for (int i = 0; i < dataRowsCollection.Length; i++)
                    {
                        double totalDays = ((System.DateTime)dataRowsCollection[i][columnName] - new System.DateTime(1900, 1, 1, 0, 0, 0, 0)).TotalDays;
                        if (totalDays < 59)
                            result = Convert.ToDouble(result) + totalDays + 1;
                        else
                            result = Convert.ToDouble(result) + totalDays + 2;
                    }
                }
                if (result != null)
                {
                    return result.ToString();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Computes the maximum.
        /// </summary>
        /// <param name="dataRowCollection">The data row collection.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public static string ComputeMax(DataRow[] dataRowCollection, string columnName)
        {
            DataTable dataTable = GetDataTable(dataRowCollection);
            if (dataTable != null)
            {
                object result = dataTable.Compute(GetMaxExpression(columnName), "1=1");
                if (result != null)
                {
                    return result.ToString();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Computes the minimum.
        /// </summary>
        /// <param name="dataRowCollection">The data row collection.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public static string ComputeMin(DataRow[] dataRowCollection, string columnName)
        {
            DataTable dataTable = GetDataTable(dataRowCollection);
            if (dataTable != null)
            {
                object result = dataTable.Compute(GetMinExpression(columnName), "1=1");
                if (result != null)
                {
                    return result.ToString();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Computes the average.
        /// </summary>
        /// <param name="dataRowCollection">The data row collection.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public static string ComputeAvg(DataRow[] dataRowCollection, string columnName)
        {
            DataTable dataTable = GetDataTable(dataRowCollection);
            if (dataTable != null)
            {
                object result = null;
                if (dataRowCollection[0][columnName].GetType().Name != "DateTime")
                    result = dataTable.Compute(GetAvgExpression(columnName), "1=1");
                else
                {
                    result = 0;
                    for (int i = 0; i < dataRowCollection.Length; i++)
                    {
                        double totalDays = ((System.DateTime)dataRowCollection[i][columnName] - new System.DateTime(1900, 1, 1, 0, 0, 0, 0)).TotalDays;
                        if (totalDays < 59)
                            result = Convert.ToDouble(result) + totalDays + 1;
                        else
                            result = Convert.ToDouble(result) + totalDays + 2;
                    }
                    result = Convert.ToDouble(result) / dataRowCollection.Length;
                }
                if (result != null)
                {
                    return result.ToString();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Computes the count.
        /// </summary>
        /// <param name="dataRowCollection">The data row collection.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public static string ComputeCount(DataRow[] dataRowCollection, string columnName)
        {
            DataTable dataTable = GetDataTable(dataRowCollection);
            if (dataTable != null)
            {
                object result = dataTable.Compute(GetCountExpression(columnName), "1=1");
                if (result != null)
                {
                    return result.ToString();
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the data table.
        /// </summary>
        /// <param name="dataRowCollection">The data row collection.</param>
        /// <returns></returns>
        static DataTable GetDataTable(DataRow[] dataRowCollection)
        {
            if (dataRowCollection.Count() > 0)
            {
                DataTable dataTable = dataRowCollection[0].Table.Clone();
                foreach (var item in dataRowCollection)
                {
                    dataTable.ImportRow(item);
                }
                return dataTable;
            }
            return null;
        }

        /// <summary>
        /// Gets the sum expression.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        static string GetSumExpression(string columnName)
        {
            if(columnName.Contains(' '))
            return string.Format("SUM([{0}])", columnName);
            else
                return string.Format("SUM({0})", columnName);
        }

        /// <summary>
        /// Gets the avg expression.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        static string GetAvgExpression(string columnName)
        {
            if (columnName.Contains(' '))
            return string.Format("AVG([{0}])", columnName);
            else
                return string.Format("AVG({0})", columnName);
        }

        /// <summary>
        /// Gets the count expression.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        static string GetCountExpression(string columnName)
        {
            if (columnName.Contains(' '))
            return string.Format("COUNT([{0}])", columnName);
            else
                return string.Format("COUNT({0})", columnName);

        }

        /// <summary>
        /// Gets the max expression.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        private static string GetMaxExpression(string columnName)
        {
            if (columnName.Contains(' '))
            return string.Format("MAX([{0}])", columnName);
            else
                return string.Format("MAX({0})", columnName);
        }

        /// <summary>
        /// Gets the min expression.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        private static string GetMinExpression(string columnName)
        {
            if (columnName.Contains(' '))
            return string.Format("MIN([{0}])", columnName);
            else
                return string.Format("MIN({0})", columnName);
        }

        /// <summary>
        /// Selects the specified IEnumerable.
        /// </summary>
        /// <param name="iEnumerable">The i enumerable.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        internal static string Select(IEnumerable iEnumerable, string columnName)
        {            
            DataRow[] dataRowsCollection = iEnumerable.Cast<DataRow>().ToArray<DataRow>();

            if (dataRowsCollection != null)
            {
                string result = dataRowsCollection.Select(i => i[columnName]).ElementAt(0).ToString();
                if (result != null)
                {
                    return result;
                }
            }            
           
            return string.Empty;
        }

        /// <summary>
        /// Process for string summary type.
        /// </summary>
        /// <param name="iEnumerable">The i enumerable.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="engine">The engine.</param>
        /// <param name="col">The col.</param>
        /// <param name="row">The row.</param>
        /// <param name="summaryInfos">The summary infos.</param>
        /// <param name="gridLayout">The grid layout.</param>
        /// <param name="IListSource">if set to <c>true</c> [I list source].</param>
        /// <returns></returns>
        internal static string StringType(IEnumerable iEnumerable, string columnName, PivotEngine engine, int col, int row,SummaryInfo[] summaryInfos,GridLayout gridLayout,bool IListSource)
        {
            DataRow[] dataRowsCollection = iEnumerable.Cast<DataRow>().ToArray<DataRow>();

            if (dataRowsCollection.Count() == 1)
            {
                if (dataRowsCollection != null)
                {
                    string result = dataRowsCollection.Select(i => i[columnName]).ElementAt(0).ToString();
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            else
            {
                engine.InsertAdditionalRows(iEnumerable, columnName, col, row, summaryInfos, gridLayout,IListSource);
            }
            return null;
        }

        /// <summary>
        /// Computes the first element.
        /// </summary>
        /// <param name="iEnumerable">The i enumerable.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        internal static string ComputeFirst(IEnumerable iEnumerable, string columnName)
        {
            DataRow[] dataRowCollection = iEnumerable.Cast<DataRow>().ToArray<DataRow>();

            object result = dataRowCollection[0][columnName];
            if (result != null)
            {
                return result.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Computes the last element.
        /// </summary>
        /// <param name="iEnumerable">The i enumerable.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        internal static string ComputeLast(IEnumerable iEnumerable, string columnName)
        {
            DataRow[] dataRowCollection = iEnumerable.Cast<DataRow>().ToArray<DataRow>();

            object result = dataRowCollection[dataRowCollection.Count()-1][columnName];
            if (result != null)
            {
                return result.ToString();
            }
            return string.Empty;
        }
    }
}
