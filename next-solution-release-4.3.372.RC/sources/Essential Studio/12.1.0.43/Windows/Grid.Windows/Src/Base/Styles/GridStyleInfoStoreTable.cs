//-------------------------------------------------------------------------------------------------
// <copyright file="GridStyleInfoStoreTable.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    ///    GridStyleInfoStoreTable holds a table where each item is a StyleInfoStore.
    /// </summary>
    /// <remarks>
    /// GridStyleInfoStoreTable is used with GetCells and SetCells method calls. Also, Insert and
    ///   Remove cells commands use GridStyleInfoStoreTable to store undo information for cells.
    /// </remarks>
    [Serializable]
    public class GridStyleInfoStoreTable : ISerializable
    {
        private GridStyleInfoStore[,] sfTable;
        private object tag;

        private const int delta = 1;

        /// <summary>
        /// Initializes a new <see cref="GridStyleInfoStoreTable"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridStyleInfoStoreTable(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            sfTable = (GridStyleInfoStore[,])info.GetValue("Table", typeof(GridStyleInfoStore[,]));
            tag = info.GetValue("Tag", typeof(object));
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridStyleInfoStoreTable"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, SerializationFormatter = true)]
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
            info.AddValue("Table", sfTable); // GridStyleInfoStore[,] 
            info.AddValue("Tag", tag); // object
        }

        /// <summary>
        /// Initializes a new <see cref="GridStyleInfoStoreTable"/> and initializes its row and column count.
        /// </summary>
        /// <param name="rowCount">Row count.</param>
        /// <param name="colCount">Column count.</param>
        public GridStyleInfoStoreTable(int rowCount, int colCount)
        {
            sfTable = new GridStyleInfoStore[rowCount + delta, colCount + delta];
        }

        /// <summary>
        /// Gets Row count of the table.
        /// </summary>
        public virtual int RowCount
        {
            get
            {
                return sfTable.GetLength(0) - delta;
            }
        }

        /// <summary>
        /// Gets Column count of the table.
        /// </summary>
        public virtual int ColCount
        {
            get
            {
                return sfTable.GetLength(1) - delta;
            }
        }

        /// <summary>
        /// Gets or sets User-defined data, e.g. covered ranges in clipboard copy / paste.
        /// </summary>
        public object Tag
        {
            get
            {
                return tag;
            }

            set
            {
                tag = value;
            }
        }

        /// <summary>
        /// The <see cref="GridStyleInfoStore"/> at a specific row and column index.
        /// </summary>
        public GridStyleInfoStore this[int rowIndex, int colIndex]
        {
            get
            {
                return sfTable[rowIndex + 1, colIndex + 1] as GridStyleInfoStore;
            }

            set
            {
                sfTable[rowIndex + 1, colIndex + 1] = value;
            }
        }
    }
}
