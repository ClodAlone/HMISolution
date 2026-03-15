//-------------------------------------------------------------------------------------------------
// <copyright file="GridCoveredCellPool.cs" company="syncfusion">
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
using System.Security;
using System.Security.Permissions;

using Syncfusion.Collections;
using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Provides storage for covered cells in the grid and allows a fast way to look up if a specific cell
    /// is part of a covered range.
    /// </summary>
    [Serializable]
    public class GridCoveredCellPool : ISerializable
    {
        /// <internalonly/>
        internal SFArrayList innerRows = new SFArrayList();
        ////private int version = 0;

        /// <overload>
        /// Initializes a <see cref="GridCoveredCellPool"/>.
        /// </overload>
        /// <summary>
        /// Initializes a <see cref="GridCoveredCellPool"/>.
        /// </summary>
        public GridCoveredCellPool()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridCoveredCellPool"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridCoveredCellPool(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            innerRows = (SFArrayList) info.GetValue("Rows", typeof(SFArrayList));
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridCoveredCellPool"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter=true)]
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, SerializationFormatter=true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            info.AddValue("Rows", innerRows); // SFArrayList
        }

        /// <overload>
        /// Resets spanned cells in the specified range.
        /// </overload>
        /// <summary>
        /// Resets spanned cells in the specified range.
        /// </summary>
        /// <param name="range">The range to be reset.</param>
        /// <returns>True if changes were made; False if there were no covered ranges.</returns>
        public bool ResetSpanCells(GridRangeInfo range)
        {
            return ResetSpanCells(range, true);
        }
            
        /// <summary>
        /// Resets spanned cells in the specified range.
        /// </summary>
        /// <param name="range">The range to be reset.</param>
        /// <param name="checkContainment">True if only covered cells should be reset that are contained in the <paramref name="range"/></param>
        /// <returns>True if changes were made; False if there were no covered ranges.</returns>
        public bool ResetSpanCells(GridRangeInfo range, bool checkContainment /*=true*/)
        {
            if (range == null || !range.IsCells)
            {
                throw new ArgumentException();
            }

            bool bDone = false;

            for (int rowIndex = range.Top; rowIndex <= range.Bottom; rowIndex++)
            {
                SFArrayList row = (SFArrayList) innerRows[rowIndex];

                if (row == null)
                {
                    continue;
                }

                int toColIndex = Math.Min(range.Right, row.Count-1);
                bool bRightBound = range.Right >= row.Count-1;
                if (!bRightBound || checkContainment)
                {
                    for (int colIndex = range.Left; colIndex <= toColIndex; colIndex++)
                    {
                        object rangeObj = row[colIndex];
                        if (rangeObj != null)
                        {
                            //// Ensure range is contained. 
                            if (checkContainment && !GridRangeInfo.IntersectRange((GridRangeInfo)rangeObj, range).Equals(rangeObj))
                            {
                                continue;
                            }
////                                throw new ArgumentException(
////                                    String.Concat(new string[] {
////                                    "Found covered cell (", 
////                                    rangeObj.ToString(),
////                                    ") that is not contained in range ",
////                                    range.ToString()
////                                    })
////                                );

                            row[colIndex] = null;

                            bDone = true;
                        }
                    }
                }

                if (bRightBound)
                {
                    row.RemoveRange(range.Left, row.Count - 1 - range.Left);
                }
            }

            return bDone;
        }

        /// <summary>
        /// Saves the covered cell's information for a specific range.
        /// </summary>
        /// <param name="range">The covered cell's range to be saved.</param>
        /// <returns>True if successful; False if there was an error or a range was already existing.</returns>
        public bool StoreSpanCells(GridRangeInfo range)
        {
            bool bDone = false;

            // Make a copy of range object and store pointer to the new object.
            GridNonImmutableRangeInfo rangeCopy = new GridNonImmutableRangeInfo(range);

            if (range == null || !range.IsCells)
            {
                throw new ArgumentException();
            }

            if (range.Width == 1 && range.Height == 1)
            {
                return false; // nothing to do
            }

            for (int rowIndex = range.Top; rowIndex <= range.Bottom; rowIndex++)
            {
                SFArrayList row = (SFArrayList) innerRows[rowIndex];
                if (row == null)
                {
                    innerRows[rowIndex] = row = new SFArrayList();
                }

                row.EnsureCount(range.Right+1);
                for (int colIndex = range.Left; colIndex <= range.Right; colIndex++)
                {
                    object rangeObj = row[colIndex];
                    if (rangeObj != null)
                    {
                        continue;
                    }
                    ////                           throw new ArgumentException(
                    ////                                String.Concat(new string[] {
                    ////                                "Found covered cell (", 
                    ////                                rangeObj.ToString(),
                    ////                                ") in range ",
                    ////                                range.ToString(),
                    ////                                " You first have to call ResetSpanCellsRowCol."
                    ////                                })
                    ////                            );
                        
                    row[colIndex] = rangeCopy;

                    bDone = true;
                }
            }

            return bDone;
        }

        /// <summary>
        /// Returns the covered range information for a cell specified with row and column index.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="range">A place holder where covered cell's range information is returned.</param>
        /// <returns>True if a covered cell was found; False otherwise.</returns>
        public bool GetSpanCellsRowCol(int rowIndex, int colIndex, out GridRangeInfo range)
        {
            object rangeObj = null;
            SFArrayList row = (SFArrayList) innerRows[rowIndex];
            if (row != null && colIndex < row.Count)
            {
                rangeObj = row[colIndex];
            }

            if (rangeObj == null)
            {
                range = GridRangeInfo.Cell(rowIndex, colIndex);
                return false;
            }

            range = (GridRangeInfo) rangeObj;
            return true;
        }

        /// <summary>
        /// Clears all covered ranges from this pool.
        /// </summary>
        public void Clear()
        {
            innerRows.Clear();
        }

        /// <summary>
        /// Initializes the pool with covered range information from a range list.
        /// </summary>
        /// <param name="ranges">The range list with all covered cells.</param>
        public void InitFromRangeList(GridRangeInfoList ranges)
        {
            Clear();
            ranges.RemoveEmptyRanges();
            foreach (GridRangeInfo range in ranges)
            {
                StoreSpanCells(range);
            }
        }
    }
}

