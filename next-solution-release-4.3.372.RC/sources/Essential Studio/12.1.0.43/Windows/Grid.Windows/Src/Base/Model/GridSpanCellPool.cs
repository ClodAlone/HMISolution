//-------------------------------------------------------------------------------------------------
// <copyright file="GridSpanCellPool.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Forms;

using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms.Grid
{
    [Serializable]
    internal class GridSpanCellPool : GridCoveredCellPool
    {
        /// <summary>
        /// Initializes a new <see cref="GridSpanCellPool"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridSpanCellPool(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
        }

        public GridSpanCellPool()
        {
        }

        public virtual void InsertRows(int rowIndex, int count)
        {
            innerRows.RemoveRange(rowIndex, innerRows.Count - rowIndex);
        }

        public virtual void RemoveRows(int fromRowIndex, int toRowIndex)
        {
            innerRows.RemoveRange(fromRowIndex, innerRows.Count - fromRowIndex);
        }

        public virtual void MoveRows(int fromRowIndex, int toRowIndex, int nDestRow)
        {
            int from = Math.Min(fromRowIndex, toRowIndex);
            int max = Math.Max(toRowIndex, nDestRow + toRowIndex - fromRowIndex);
            for (int index = from; index <= max; index++)
            {
                innerRows[index] = null;
            }
        }

        public virtual void InsertCols(int colIndex, int nCount)
        {
        }

        public virtual void RemoveCols(int fromColIndex, int toColIndex) 
        {
        }

        public virtual void MoveCols(int fromColIndex, int toColIndex, int nDestCol) 
        { 
        }

        /*
                public virtual bool GetSpanCellsRowCol(int rowIndex, int colIndex, out GridRangeInfo rg) 
                {
                    rg = GridRangeInfo.Empty;
                    return false;
                }

                public virtual bool StoreSpanCellsRowCol(int rowIndex, int colIndex, int toRowIndex, int toColIndex, bool bSet)
                {
                    return false;
                }

                public void InitFromRangeList(GridRangeInfoList pRangeList) {}
                public void Clear();
        */
    }
}
