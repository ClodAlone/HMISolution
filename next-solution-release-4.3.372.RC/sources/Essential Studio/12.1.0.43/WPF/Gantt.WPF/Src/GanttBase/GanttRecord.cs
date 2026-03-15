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
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Controls.Gantt
{
    /// <summary>
    /// Class that holds the information oabout a record.
    /// </summary>
    internal class GanttRecord
    {
        List<GanttRecord> childRecords = new List<GanttRecord>();
        List<GanttRecord> inLineRecords = new List<GanttRecord>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GanttRecord"/> class.
        /// </summary>
        public GanttRecord()
        {

        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is expanded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        public bool IsExpanded { get; set; }

        /// <summary>
        /// Gets or sets the data item.
        /// </summary>
        /// <value>The data item.</value>
        public object DataItem { get; set; }

        /// <summary>
        /// Gets or sets the parent record.
        /// </summary>
        /// <value>The parent record.</value>
        public GanttRecord ParentRecord { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is realized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is realized; otherwise, <c>false</c>.
        /// </value>
        public bool IsRealized { get; internal set; }

        /// <summary>
        /// Gets or sets the child records.
        /// </summary>
        /// <value>The child records.</value>
        public List<GanttRecord> ChildRecords
        {
            get { return childRecords; }
            set { childRecords = value; }
        }

        /// <summary>
        /// Gets or sets the in line records.
        /// </summary>
        /// <value>The in line records.</value>
        public List<GanttRecord> InLineRecords
        {
            get { return inLineRecords; }
            set { inLineRecords = value; }
        }
    }

    /// <summary>
    /// Collection that holds created Gantt Records
    /// </summary>
    internal class GanttRecordCollection : ObservableCollection<GanttRecord>
    {
        /// <summary>
        /// Records from item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public GanttRecord RecordFromItem(object item)
        {
            return this.Where(o => o.DataItem.Equals(item)).FirstOrDefault();
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool RemoveItem(object item)
        {
            return this.Remove(this.RecordFromItem(item));
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns></returns>
        public new int IndexOf(GanttRecord record)
        {
            return base.IndexOf(record);
        }


        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public int IndexOf(object item)
        {
            GanttRecord record = this.RecordFromItem(item);
            if (record != null)
                return this.IndexOf(record);

            return -1;
        }
    }
}
