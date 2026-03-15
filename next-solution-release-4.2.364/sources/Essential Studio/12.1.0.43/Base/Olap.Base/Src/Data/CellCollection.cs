//-------------------------------------------------------------------------------------------------
// <copyright file="CellCollection.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using Syncfusion.Olap.Engine;
using Syncfusion.Olap.Manager;
using MASAC = Microsoft.AnalysisServices.AdomdClient;

namespace Syncfusion.Olap.Data
{
    /// <summary>
    /// Collection of Cell objects
    /// </summary>
    /// <remarks>
    /// Used for retrieving multiple cells from AdomdProvider.
    /// </remarks>
    public class CellCollection : List<List<Cell>>
    {
        #region Constructor
        /// <summary>
        /// Gets or sets the cell set.
        /// </summary>
        /// <value>The cell set.</value>
        public CellSet cellSet
        {
            get;
            set;
        }      

        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the cells.
        /// </summary>
        /// <param name="m_CellSet">The m_ cell set.</param>
        /// <returns>Returns the collection of Cells</returns>
        public CellCollection GetCells(CellSet m_CellSet)
        {
            this.cellSet = m_CellSet;
            Axis columnAxis = null;
            Axis rowAxis = null;

            if (cellSet.Axes.Count > 0)
            {
                columnAxis = cellSet.Axes[0];
            }

            if (cellSet.Axes.Count > 1)
            {
                rowAxis = cellSet.Axes[1];
            }

            List<PivotRowDescriptor> rowBufferRows = GetExpandableBufferRows(rowAxis);
            List<PivotRowDescriptor> colBufferRows = GetExpandableBufferRows(columnAxis);
            int rowHeaderLength = rowAxis == null || rowAxis.TupleSet.Count == 0 ? 1 : rowAxis.TupleSet.Count;
            int colHeaderLength = columnAxis == null || columnAxis.TupleSet.Count == 0 ? 1 : columnAxis.TupleSet.Count;
            int rowHeaderDepth = GetLevelDepth(rowBufferRows);
            int colHeaderDepth = GetLevelDepth(colBufferRows);
            for (int j = 0; j < colHeaderLength; j++)
            {
                List<int> intList = new List<int>();
                intList.Add(j);
                List<Cell> cellCollect = new List<Cell>();
                if (rowAxis != null)
                {
                    for (int i = 0; i < rowHeaderLength; i++)
                    {
                        intList.Add(i);
                        Cell olapCell = cellSet.GetCell(intList.ToArray());
                        cellCollect.Add(olapCell);
                        //this.Add(cellCollect);
                        intList.RemoveAt(1);
                    }
                    this.Add(cellCollect);
                }
                else
                {
                    //intList.Add(0);
                    Cell olapCell = cellSet.GetCell(intList.ToArray());
                    if (olapCell != null)
                    {
                        cellCollect.Add(olapCell);
                        this.Add(cellCollect);
                    }
                }
            }
            return this;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the expandable buffer rows.
        /// </summary>
        /// <param name="cubeAxis">The cube axis.</param>
        /// <returns>Returns the list of PivotRowDescriptor</returns>
        private static List<PivotRowDescriptor> GetExpandableBufferRows(Axis cubeAxis)
        {
            List<PivotRowDescriptor> bufferRows = new List<PivotRowDescriptor>();

            if (cubeAxis != null)
            {
                for (int i = 0; i < cubeAxis.TupleSet.Count;)
                {
                    Tuple tupleRows = cubeAxis.TupleSet[i];
                    for (int j = 0; j < tupleRows.Members.Count; j++)
                    {
                        int levelsCount = GetMaxLevel(cubeAxis, j) - GetMinLevel(cubeAxis, j) + 1;
                        PivotRowDescriptor bufferRow = new PivotRowDescriptor();

                        for (int levelInd = 0; levelInd < levelsCount; levelInd++)
                        {
                            PivotCellDescriptor cellDesc = new PivotCellDescriptor();
                            cellDesc.CellType = PivotCellDescriptorType.RowHeader;
                            bufferRow.Cells.Add(cellDesc);
                        }

                        if (levelsCount == 0)
                        {
                            levelsCount++;
                        }

                        bufferRows.Insert(j, bufferRow);
                    }

                    break;
                }
            }

            return bufferRows;
        }

        /// <summary>
        /// Gets the level depth.
        /// </summary>
        /// <param name="bufferRows">The buffer rows.</param>
        /// <returns>The Level Depth</returns>
        private static int GetLevelDepth(List<PivotRowDescriptor> bufferRows)
        {
            int depth = 0;

            foreach (PivotRowDescriptor rowDesc in bufferRows)
            {
                int cells = rowDesc.Cells.Count;
                if (cells == 0)
                {
                    cells++;
                }

                depth += cells;
            }

            return depth;
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Gets the max level.
        /// </summary>
        /// <param name="cubeAxis">The cube axis.</param>
        /// <param name="dimension">The dimension.</param>
        /// <returns>Returns the Max Level</returns>
        public static int GetMaxLevel(Axis cubeAxis, int dimension)
        {
            return cubeAxis.TupleSet.MaxLevel[dimension];
        }

        /// <summary>
        /// Gets the min level.
        /// </summary>
        /// <param name="cubeAxis">The cube axis.</param>
        /// <param name="dimension">The dimension.</param>
        /// <returns>Returns the min level</returns>
        public static int GetMinLevel(Axis cubeAxis, int dimension)
        {
            return cubeAxis.TupleSet.MinLevel[dimension];
        }
        #endregion
    }
}
