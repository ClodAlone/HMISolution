#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace Syncfusion.Windows.Controls.Gantt
{
    /// <summary>
    /// A class that can hold the information about a task/activity.
    /// </summary>
    public class TaskDetails : IGanttTask
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDetails"/> class.
        /// </summary>
        public TaskDetails()
        {
            Child = new ObservableCollection<IGanttTask>();
            _predecessor = new ObservableCollection<Predecessor>();
            _resources = new ObservableCollection<Resource>();           
        }

        #region Class Fields
        
        int _taskId;
        String _taskName;
        DateTime _startDate;
        DateTime _finishDate;
        TimeSpan _duration;
        Double _cost;
        DateTime _baselineStart;
        DateTime _baselineFinish;
        Double _baselineCost;       
        ObservableCollection<Predecessor> _predecessor;
        ObservableCollection<Resource> _resources;
        Double _progress;
        bool _isActive;
        Double _fixedCost;
        Double _totalCost;
        Double _baseline;
        Double _variance;
        Double _actualCost;
        Double _remainingCost;
        bool _isSummaryRow;
        IGanttTask _parentNode;
        ObservableCollection<IGanttTask> _child;
        bool _isMileStone;

        #endregion

        #region Public Properties

        #region Child Collection

        /// <summary>
        /// Gets or sets the child.
        /// </summary>
        /// <value>The child.</value>
        public ObservableCollection<IGanttTask> Child
        {
            get
            {
                return _child;
            }
            set
            {
                _child = value;
                OnPropertyChanged("Child");

            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the task id.
        /// </summary>
        /// <value>The task id.</value>
        public int TaskId
        {
            get
            {
                return _taskId;
            }
            set
            {
                _taskId = value;
                OnPropertyChanged("TaskId");
            }
        }

        /// <summary>
        /// Gets or sets the name of the task.
        /// </summary>
        /// <value>The name of the task.</value>
        public String TaskName
        {
            get
            {
                return _taskName;
            }
            set
            {
                _taskName = value;
                OnPropertyChanged("TaskName");
            }
        }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
                OnPropertyChanged("StartDate");
            }
        }

        /// <summary>
        /// Gets or sets the finish date.
        /// </summary>
        /// <value>The finish date.</value>
        public DateTime FinishDate
        {
            get
            {
                return _finishDate;
            }
            set
            {
                _finishDate = value;
                OnPropertyChanged("FinishDate");
            }
        }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        public TimeSpan Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                _duration = value;
                OnPropertyChanged("Duration");
            }
        }

        /// <summary>
        /// Gets/sets the Cost of the Task
        /// </summary>
        public Double Cost
        {
            get
            {
                return _cost;
            }
            set
            {
                _cost = value;
                OnPropertyChanged("Cost");
            }
        }
        
        /// <summary>
        /// Gets/sets the BaselineStart Date of the Task
        /// </summary>
        public DateTime BaselineStart
        {
            get
            {
                return _baselineStart;
            }
            set
            {
                _baselineStart = value;
                OnPropertyChanged("BaselineStart");
            }
        }

        /// <summary>
        /// Gets/sets the BaselineEnd Date of the Task
        /// </summary>
        public DateTime BaselineFinish
        {
            get
            {
                return _baselineFinish;
            }
            set
            {
                if (_baselineFinish != value)
                {
                    _baselineFinish = value;
                    OnPropertyChanged("BaselineFinish");
                }
            }
        }

        /// <summary>
        /// Gets/sets the BaselineCost of the Task
        /// </summary>
        public Double BaselineCost
        {
            get
            {
                return _baselineCost;
            }
            set
            {
                if (_baselineCost != value)
                {
                    _baselineCost = value;
                    OnPropertyChanged("BaselineCost");
                }
            }
        }

        /// <summary>
        /// Gets or sets the predecessor.
        /// </summary>
        /// <value>The predecessor.</value>
        public ObservableCollection<Predecessor> Predecessor
        {
            get
            {
                return _predecessor;
            }
            set
            {
                _predecessor = value;
                OnPropertyChanged("Predecessor");
            }
        }

        /// <summary>
        /// Gets or sets the resources.
        /// </summary>
        /// <value>The resources.</value>
        public ObservableCollection<Resource> Resources
        {
            get
            {
                return _resources;
            }
            set
            {
                _resources = value;
                OnPropertyChanged("Resources");
            }
        }

        /// <summary>
        /// Gets or sets the progress.
        /// </summary>
        /// <value>The progress.</value>
        public Double Progress
        {
            get
            {
                return Math.Round(_progress, 2);
            }
            set
            {
                if (value <= 100 && _progress != value)
                {
                    _progress = value;
                    OnPropertyChanged("Progress");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                OnPropertyChanged("IsActive");
            }
        }

        /// <summary>
        /// Gets or sets the fixed cost.
        /// </summary>
        /// <value>The fixed cost.</value>
        public Double FixedCost
        {
            get
            {
                return _fixedCost;
            }
            set
            {
                _fixedCost = value;
                OnPropertyChanged("FixedCost");
            }
        }

        /// <summary>
        /// Gets or sets the total cost.
        /// </summary>
        /// <value>The total cost.</value>
        public Double TotalCost
        {
            get
            {
                return _totalCost;
            }
            set
            {
                _totalCost = value;
                OnPropertyChanged("TotalCost");
            }
        }

        /// <summary>
        /// Gets or sets the baseline.
        /// </summary>
        /// <value>The baseline.</value>
        public Double Baseline
        {
            get
            {
                return _baseline;
            }
            set
            {
                _baseline = value;
                OnPropertyChanged("Baseline");
            }
        }

        /// <summary>
        /// Gets or sets the variance.
        /// </summary>
        /// <value>The variance.</value>
        public Double Variance
        {
            get
            {
                return _variance;
            }
            set
            {
                _variance = value;
                OnPropertyChanged("Variance");
            }
        }

        /// <summary>
        /// Gets or sets the actual cost.
        /// </summary>
        /// <value>The actual cost.</value>
        public Double ActualCost
        {
            get
            {
                return _actualCost;
            }
            set
            {
                _actualCost = value;
                OnPropertyChanged("ActualCost");
            }
        }

        /// <summary>
        /// Gets or sets the remaining cost.
        /// </summary>
        /// <value>The remaining cost.</value>
        public Double RemainingCost
        {
            get
            {
                return _remainingCost;
            }
            set
            {
                _remainingCost = value;
                OnPropertyChanged("RemainingCost");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is summary row.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is summary row; otherwise, <c>false</c>.
        /// </value>
        public bool IsSummaryRow
        {
            get
            {
                return _isSummaryRow;
            }
            set
            {
                _isSummaryRow = value;
                OnPropertyChanged("IsSummaryRow");
            }
        }

        /// <summary>
        /// Gets or sets the parent node.
        /// </summary>
        /// <value>The parent node.</value>
        public IGanttTask ParentNode
        {
            get
            {
                return _parentNode;
            }
            set
            {
                _parentNode = value;
                OnPropertyChanged("ParentNode");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is mile stone.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is mile stone; otherwise, <c>false</c>.
        /// </value>
        public bool IsMileStone
        {
            get
            {
                return _isMileStone;
            }
            set
            {
                _isMileStone = value;
                OnPropertyChanged("IsMileStone");
            }
        }

        #endregion

        #region INotifyPropertyChanged

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void OnPropertyChanged(string propertyName)
        {
            if (propertyName == null) 
                throw new ArgumentNullException(propertyName);
            
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}