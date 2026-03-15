#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Syncfusion.Windows.Controls.Gantt
{
    /// <summary>
    /// An Interface to create the project tasks/activities.
    /// </summary>
    public interface IGanttTask : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the task id.
        /// </summary>
        /// <value>The task id.</value>
        int TaskId { get; set; }

        /// <summary>
        /// Gets or sets the name of the task.
        /// </summary>
        /// <value>The name of the task.</value>
        String TaskName { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the finish date.
        /// </summary>
        /// <value>The finish date.</value>
        DateTime FinishDate { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets the cost.
        /// </summary>
        /// <value>The cost.</value>
        Double Cost { get; set; }

        /// <summary>
        /// Gets or sets the baseline start.
        /// </summary>
        /// <value>The baseline start.</value>
        DateTime BaselineStart { get; set; }

        /// <summary>
        /// Gets or sets the baseline end.
        /// </summary>
        /// <value>The baseline end.</value>
        DateTime BaselineFinish { get; set; }

        /// <summary>
        /// Gets or sets the baseline cost.
        /// </summary>
        /// <value>The baseline cost.</value>
        Double BaselineCost { get; set; }

        /// <summary>
        /// Gets or sets the predecessor.
        /// </summary>
        /// <value>The predecessor.</value>
        ObservableCollection<Predecessor> Predecessor { get; set; }

        /// <summary>
        /// Gets or sets the resources.
        /// </summary>
        /// <value>The resources.</value>
        ObservableCollection<Resource> Resources { get; set; }

        /// <summary>
        /// Gets or sets the progress.
        /// </summary>
        /// <value>The progress.</value>
        Double Progress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is mile stone.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is mile stone; otherwise, <c>false</c>.
        /// </value>
        bool IsMileStone { get; set; }

        /// <summary>
        /// Gets or sets the fixed cost.
        /// </summary>
        /// <value>The fixed cost.</value>
        Double FixedCost { get; set; }

        /// <summary>
        /// Gets or sets the total cost.
        /// </summary>
        /// <value>The total cost.</value>
        Double TotalCost { get; set; }

        /// <summary>
        /// Gets or sets the baseline.
        /// </summary>
        /// <value>The baseline.</value>
        Double Baseline { get; set; }

        /// <summary>
        /// Gets or sets the variance.
        /// </summary>
        /// <value>The variance.</value>
        Double Variance { get; set; }

        /// <summary>
        /// Gets or sets the actual cost.
        /// </summary>
        /// <value>The actual cost.</value>
        Double ActualCost { get; set; }

        /// <summary>
        /// Gets or sets the remaining cost.
        /// </summary>
        /// <value>The remaining cost.</value>
        Double RemainingCost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is summary row.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is summary row; otherwise, <c>false</c>.
        /// </value>
        bool IsSummaryRow { get; set; }

        /// <summary>
        /// Gets or sets the parent node.
        /// </summary>
        /// <value>The parent node.</value>
        IGanttTask ParentNode { get; set; }

        /// <summary>
        /// Gets or sets the child.
        /// </summary>
        /// <value>The child.</value>
        ObservableCollection<IGanttTask> Child { get; set; }
    }
}
