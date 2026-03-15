#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    public class PivotTableLayout : List<List<PivotValueCollections>>
    {
        /// <summary>
        /// 
        /// </summary>
        List<List<PivotValueCollections>> pivotValueCollections = new List<List<PivotValueCollections>>();

        public int maxRowCount;
        public int maxColumnCount;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="colIndex"></param>
        /// <returns></returns>
        public PivotValueCollections this[int rowIndex, int colIndex]
        {
            get
            {
                return pivotValueCollections[rowIndex][colIndex];
            }
            set
            {
                if (rowIndex > pivotValueCollections.Count - 1)
                {
                    int count = rowIndex - pivotValueCollections.Count;
                    for (int i = 0; i <= count; i++)
                        pivotValueCollections.Add(new List<PivotValueCollections>());
                }
                if (maxColumnCount < colIndex)
                    maxColumnCount = colIndex;
                if (rowIndex > maxRowCount)
                    maxRowCount = rowIndex;
                if (pivotValueCollections[rowIndex].Count <= colIndex)
                {
                    if ((colIndex - pivotValueCollections[rowIndex].Count) > 0)
                    {
                        for (int smallCount = pivotValueCollections[rowIndex].Count; smallCount <= colIndex; smallCount++)
                        {
                            PivotValueCollections PV = new PivotValueCollections();
                            PV.PivotTablePartStyle = value.PivotTablePartStyle;
                            if (pivotValueCollections[rowIndex].Count == 0)
                            {
                                //PV.PivotTablePartStyle &= ~PivotTableParts.ColumnSubHeading1;
                                //PV.PivotTablePartStyle &= ~PivotTableParts.ColumnSubHeading2;
                                //PV.PivotTablePartStyle &= ~PivotTableParts.ColumnSubHeading3;
                                PV.PivotTablePartStyle = PivotTableParts.WholeTable | PivotTableParts.FirstColumn | PivotTableParts.HeaderRow;
                            }
                            if (smallCount != colIndex)
                                pivotValueCollections[rowIndex].Add(PV);
                            else
                                pivotValueCollections[rowIndex].Add(value);
                        }
                    }
                    else
                    pivotValueCollections[rowIndex].Add(value);
                }
                else
                    pivotValueCollections[rowIndex][colIndex] = value;
            }
        }
    }
}
