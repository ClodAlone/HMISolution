#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;

namespace Syncfusion.Windows.Controls.Gantt
{
    /// <summary>
    /// Class used to represent the dependency relationship between the tasks.
    /// </summary>
    public class Predecessor
    {
        public Predecessor()
        {
            GanttTaskRelationship = Gantt.GanttTaskRelationship.FinishToStart;
        }

        /// <summary>
        /// Gets or sets the index of the gantt task.
        /// </summary>
        /// <value>The index of the gantt task.</value>
        public int GanttTaskIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the gantt task relationship.
        /// </summary>
        /// <value>The gantt task relationship.</value>
        public GanttTaskRelationship GanttTaskRelationship
        {
            get;
            set;
        }
    }
}
