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

namespace Syncfusion.Windows.Controls.Gantt
{
    /// <summary>
    /// Class contains data about Current Project Information 
    /// </summary>
    public class ProjectInfo
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectInfo"/> class.
        /// </summary>
        public ProjectInfo() { }
        
        #endregion

        #region Fields

        /// <summary>
        /// Gets the name of the project.
        /// </summary>
        /// <value>
        /// The name of the project.
        /// </value>
        public string ProjectName
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the start date.
        /// </summary>
        /// /// <value>
        /// The start Date of the project.
        /// </value>
        public DateTime StartDate
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the actual start date.
        /// </summary>
        /// /// <value>
        /// The actual Start Date of the project.
        /// </value>
        public DateTime ActualStartDate
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the baseline start.
        /// </summary>
        /// /// <value>
        /// The Baseline start date of the project.
        /// </value>
        public DateTime BaselineStart
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the start variance.
        /// </summary>
        /// <value>
        /// The start variance the project.
        /// </value>
        public TimeSpan StartVariance
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the end date.
        /// </summary>
        /// /// <value>
        /// The finish date of the project.
        /// </value>
        public DateTime FinishDate
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the actual finish date.
        /// </summary>
        /// <value>The actual finish date.</value>
        public DateTime ActualFinishDate
        {
            get;
            internal set;
        }


        /// <summary>
        /// Gets or sets the baseline finish.
        /// </summary>
        /// <value>The baseline finish.</value>
        public DateTime BaselineFinish
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the end variance.
        /// </summary>
        /// /// <value>
        /// The end variance of the project.
        /// </value>
        public TimeSpan FinishVariance
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the cost.
        /// </summary>
        /// /// <value>
        /// The cost of the project.
        /// </value>
        public Double Cost
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the actual cost.
        /// </summary>
        /// /// <value>
        /// The actual cost of the project.
        /// </value>
        public Double ActualCost
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the baseline cost.
        /// </summary>
        /// /// <value>
        /// The baseline cost of the project.
        /// </value>
        public Double BaselineCost
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the remaining cost.
        /// </summary>
        /// /// <value>
        /// The remaining cost of the project.
        /// </value>
        public Double RemainingCost
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// /// <value>
        /// The duration of the project.
        /// </value>
        public TimeSpan Duration
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the actual duration.
        /// </summary>
        /// /// <value>
        /// The actual duration of the project.
        /// </value>
        public TimeSpan ActualDuration
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the duration of the baseline.
        /// </summary> 
        /// /// <value>
        /// The baseline Duration of the project.
        /// </value>
        public TimeSpan BaselineDuration
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the duration of the remaining.
        /// </summary>  
        /// /// <value>
        /// The remaining duration of the project.
        /// </value>
        public TimeSpan RemainingDuration
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the work.
        /// </summary>
        /// /// <value>
        /// The work of the project.
        /// </value>
        public TimeSpan Work
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the actual work.
        /// </summary>
        /// /// <value>
        /// The actual work of the project.
        /// </value>
        public TimeSpan ActualWork
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the baseline work.
        /// </summary>
        /// /// <value>
        /// The baseline work of the project.
        /// </value>
        public TimeSpan BaselineWork
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the remaining work.
        /// </summary>
        /// /// <value>
        /// The remaining work of the project.
        /// </value>
        public TimeSpan RemainingWork
        {
            get;
            internal set;
        }
        #endregion
    } 
}
