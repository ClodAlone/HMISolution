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
using System.Windows;
using System.ComponentModel;

namespace Syncfusion.Windows.Controls.Gantt
{
    /// <summary>
    /// Class used to provide the mapping name to Gantt.
    /// </summary>
    public class TaskAttributeMapping : DependencyObject
    {
        #region DefaultCalss

        /// <summary>
        /// Gets the default.
        /// </summary>
        /// <value>The default.</value>
        internal static TaskAttributeMapping Default
        {
            get
            {
                return new TaskAttributeMapping
                {
                    ChildMapping ="Child",
                    TaskIdMapping = "TaskId",
                    TaskNameMapping ="TaskName",
                    DurationMapping = "Duration",
                    StartDateMapping ="StartDate",
                    FinishDateMapping ="FinishDate",
                    MileStoneMapping="IsMileStone",
                    ProgressMapping = "Progress",
                    PredecessorMapping ="Predecessor",
                    ResourceInfoMapping = "Resources",
                    CostMapping="Cost",
                    BaselineStartMapping="BaselineStart",
                    BaselineFinishMapping="BaselineFinish",
                    BaselineCostMapping="BaselineCost", 
                };
            }
        }

        #endregion

        #region Constructor & overrides

        Dictionary<string, string> _mappedAttributes;

        /// <summary>
        /// Gets the mapped attributes.
        /// </summary>
        /// <value>The mapped attributes.</value>
        public Dictionary<string, string> MappedAttributes
        {
            get
            {
                return _mappedAttributes;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has child mapping.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has child mapping; otherwise, <c>false</c>.
        /// </value>
        public bool HasChildMapping
        {
            get
            {
                return MappedAttributes.Count > 0 && MappedAttributes.Keys.Contains("ChildMapping");
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has mile stone mapping.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has mile stone mapping; otherwise, <c>false</c>.
        /// </value>
        public bool HasMileStoneMapping
        {
            get
            {
                return MappedAttributes.Count > 0 && MappedAttributes.Keys.Contains("MileStoneMapping");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskAttributeMapping"/> class.
        /// </summary>
        public TaskAttributeMapping()
        {
            _mappedAttributes = new Dictionary<string, string>();
        }

#if !SILVERLIGHT
        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.DependencyObject"/> has been updated. The specific dependency property that changed is reported in the event data.
        /// </summary>
        /// <param name="e">Event data that will contain the dependency property identifier of interest, the property metadata for the type, and old and new values.</param>
       protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
          
            if (this.MappedAttributes.Keys.Count > 0 && this.MappedAttributes.Keys.Contains(e.Property.Name))
            {
                MappedAttributes[e.Property.Name] = (string)e.NewValue;
            }
            else
            {
                MappedAttributes.Add(e.Property.Name, (string)e.NewValue);
            }
        }
#else
        private void OnPropertyChanged(string propertyName, string value)
        {
            if (this.MappedAttributes.Keys.Count > 0 && MappedAttributes.Keys.Contains(propertyName))
            {
                MappedAttributes[propertyName] = value;
            }
            else
            {
                MappedAttributes.Add(propertyName, value);
            }
        }
#endif
        #endregion

        #region TaskIdMapping (DependencyProperty)

        /// <summary>
        /// Gets or sets the subject mapping.
        /// </summary>
        /// <value>The subject mapping.</value>
        public string TaskIdMapping
        {
            get { return (string)GetValue(TaskIdMappingProperty); }
            set
            {
                SetValue(TaskIdMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("TaskIdMapping", value);
#endif
            }
        }
        public static readonly DependencyProperty TaskIdMappingProperty =
            DependencyProperty.Register("TaskIdMapping", typeof(string), typeof(TaskAttributeMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region TaskNameMapping (DependencyProperty)

        /// <summary>
        /// Gets or sets the subject mapping.
        /// </summary>
        /// <value>The subject mapping.</value>
        public string TaskNameMapping
        {
            get { return (string)GetValue(TaskNameMappingProperty); }
            set
            {
                SetValue(TaskNameMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("TaskNameMapping", value);
#endif
            }
        }
        public static readonly DependencyProperty TaskNameMappingProperty =
            DependencyProperty.Register("TaskNameMapping", typeof(string), typeof(TaskAttributeMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region StartDateMapping (DependencyProperty)

        /// <summary>
        /// Gets or sets the start time mapping.
        /// </summary>
        /// <value>The start time mapping.</value>
        public string StartDateMapping
        {
            get { return (string)GetValue(StartDateMappingProperty); }
            set
            {
                SetValue(StartDateMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("StartDateMapping", value);
#endif
            }
        }
        public static readonly DependencyProperty StartDateMappingProperty =
            DependencyProperty.Register("StartDateMapping", typeof(string), typeof(TaskAttributeMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region FinishDateMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the EndTimeMapping property for the bound object.
        /// </summary>
        public string FinishDateMapping
        {
            get { return (string)GetValue(FinishDateMappingProperty); }
            set
            {
                SetValue(FinishDateMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("FinishDateMapping", value);
#endif
            }
        }
        public static readonly DependencyProperty FinishDateMappingProperty =
            DependencyProperty.Register("FinishDateMapping", typeof(string), typeof(TaskAttributeMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region StartPointMapping (Dependency Property)

        public string StartPointMapping
        {
            get { return (string)GetValue(StartPointMappingProperty); }
            set
            {
                SetValue(StartPointMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("StartPointMapping", value);
#endif
            }
        }

        // Using a DependencyProperty as the backing store for StartPointMapping.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartPointMappingProperty =
            DependencyProperty.Register("StartPointMapping", typeof(string), typeof(TaskAttributeMapping), new PropertyMetadata(String.Empty));
        
        #endregion

        #region  Finish Point Mapping (Dependency Property)

        public string FinishPointMapping
        {
            get { return (string)GetValue(FinishPointMappingProperty); }
            set
            {
                SetValue(FinishPointMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("FinishPointMapping", value);
#endif
            }
        }

        // Using a DependencyProperty as the backing store for EndPoint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FinishPointMappingProperty =
            DependencyProperty.Register("FinishPointMapping", typeof(string), typeof(TaskAttributeMapping), new PropertyMetadata(String.Empty));
        #endregion

        #region DurationMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the Location mapping property for the bound object.
        /// </summary>
        public string DurationMapping
        {
            get { return (string)GetValue(DurationMappingProperty); }
            set
            {
                SetValue(DurationMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("DurationMapping", value);
#endif
            }
        }
        public static readonly DependencyProperty DurationMappingProperty =
            DependencyProperty.Register("DurationMapping", typeof(string), typeof(TaskAttributeMapping),
              new PropertyMetadata(string.Empty));

        #endregion

        #region CostMapping(Dependency Property)
        /// <summary>
        /// Gets/Sets the Subject Mapping property for the bound object
        /// </summary>
        public string CostMapping
        {
            get { return (string)GetValue(CostMappingProperty); }
            set
            {
                SetValue(CostMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("CostMapping", value);
#endif
            }

        }
        public static readonly DependencyProperty CostMappingProperty =
            DependencyProperty.Register("CostMapping", typeof(string), typeof(TaskAttributeMapping),
            new PropertyMetadata(string.Empty));
        #endregion

        #region BaselineStart Mapping (Dependency Property)
        public string BaselineStartMapping
        {
            get
            {
                return (string)GetValue(BaselineStartMappingProperty);
            }
            set
            {
                SetValue(BaselineStartMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("BaselineStartMapping", value);
#endif
            }
        }
        public static readonly DependencyProperty BaselineStartMappingProperty =
            DependencyProperty.Register("BaselineStartMapping", typeof(string), typeof(TaskAttributeMapping),
            new PropertyMetadata(string.Empty));
        #endregion

        #region BaselineFinish Mapping (Dependency Property)
        public string BaselineFinishMapping
        {
            get
            {
                return (string)GetValue(BaselineFinishMappingProperty);
            }
            set
            {
                SetValue(BaselineFinishMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("BaselineFinishMapping", value);
#endif
            }
        }
        public static readonly DependencyProperty BaselineFinishMappingProperty =
            DependencyProperty.Register("BaselineFinishMapping", typeof(string), typeof(TaskAttributeMapping),
            new PropertyMetadata(string.Empty));
        #endregion

        #region BaselineCost Mapping (Dependency Property)
        public string BaselineCostMapping
        {
            get
            {
                return (string)GetValue(BaselineCostMappingProperty);
            }
            set
            {
                SetValue(BaselineCostMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("BaselineCostMapping", value);
#endif
            }
        }
        public static readonly DependencyProperty BaselineCostMappingProperty =
            DependencyProperty.Register("BaselineCostMapping", typeof(string), typeof(TaskAttributeMapping),
            new PropertyMetadata(string.Empty));
        #endregion            

        #region PredecessorMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string PredecessorMapping
        {
            get { return (string)GetValue(PredecessorMappingProperty); }
            set
            {
                SetValue(PredecessorMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("PredecessorMapping", value);
#endif
            }
        }

        public static readonly DependencyProperty PredecessorMappingProperty =
            DependencyProperty.Register("PredecessorMapping", typeof(string), typeof(TaskAttributeMapping),
            new PropertyMetadata(string.Empty));

        #endregion

        #region ResourceInfoMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string ResourceInfoMapping
        {
            get { return (string)GetValue(ResourceInfoMappingProperty); }
            set
            {
                SetValue(ResourceInfoMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("ResourceInfoMapping", value);
#endif
            }
        }

        public static readonly DependencyProperty ResourceInfoMappingProperty =
            DependencyProperty.Register("ResourceInfoMapping", typeof(string), typeof(TaskAttributeMapping),
            new PropertyMetadata(string.Empty));

        #endregion

        #region ProgressMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string ProgressMapping
        {
            get { return (string)GetValue(ProgressMappingProperty); }
            set
            {
                SetValue(ProgressMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("ProgressMapping", value);
#endif
            }
        }

        public static readonly DependencyProperty ProgressMappingProperty =
            DependencyProperty.Register("ProgressMapping", typeof(string), typeof(TaskAttributeMapping),
            new PropertyMetadata(string.Empty));

        #endregion

        #region IsActiveMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string IsActiveMapping
        {
            get { return (string)GetValue(IsActiveMappingProperty); }
            set
            {
                SetValue(IsActiveMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("IsActiveMapping", value);
#endif
            }
        }

        public static readonly DependencyProperty IsActiveMappingProperty =
            DependencyProperty.Register("IsActiveMapping", typeof(string), typeof(TaskAttributeMapping),
            new PropertyMetadata(string.Empty));

        #endregion

        #region FixedCostMapping (DependencyProperty)

        /// <summary>
        /// Gets or sets the start time mapping.
        /// </summary>
        /// <value>The start time mapping.</value>
        public string FixedCostMapping
        {
            get { return (string)GetValue(FixedCostMappingProperty); }
            set
            {
                SetValue(FixedCostMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("FixedCostMapping", value);
#endif
            }
        }
        public static readonly DependencyProperty FixedCostMappingProperty =
            DependencyProperty.Register("FixedCostMapping", typeof(string), typeof(TaskAttributeMapping),
            new PropertyMetadata(string.Empty));

        #endregion

        #region BaseLineMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string BaseLineMapping
        {
            get { return (string)GetValue(BaseLineMappingProperty); }
            set
            {
                SetValue(BaseLineMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("BaseLineMapping", value);
#endif
            }
        }

        public static readonly DependencyProperty BaseLineMappingProperty =
            DependencyProperty.Register("BaseLineMapping", typeof(string), typeof(TaskAttributeMapping),
            new PropertyMetadata(string.Empty));

        #endregion

        #region TotalCostMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string TotalCostMapping
        {
            get { return (string)GetValue(TotalCostMappingProperty); }
            set
            {
                SetValue(TotalCostMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("TotalCostMapping", value);
#endif
            }
        }

        public static readonly DependencyProperty TotalCostMappingProperty =
            DependencyProperty.Register("TotalCostMapping", typeof(string), typeof(TaskAttributeMapping),
            new PropertyMetadata(string.Empty));

        #endregion

        #region VarianceMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string VarianceMapping
        {
            get { return (string)GetValue(VarianceMappingProperty); }
            set
            {
                SetValue(VarianceMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("VarianceMapping", value);
#endif
            }
        }

        public static readonly DependencyProperty VarianceMappingProperty =
            DependencyProperty.Register("VarianceMapping", typeof(string), typeof(TaskAttributeMapping),
            new PropertyMetadata(string.Empty));

        #endregion

        #region ActualCostMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string ActualCostMapping
        {
            get { return (string)GetValue(ActualCostMappingProperty); }
            set
            {
                SetValue(ActualCostMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("ActualCostMapping", value);
#endif
            }
        }

        public static readonly DependencyProperty ActualCostMappingProperty =
            DependencyProperty.Register("ActualCostMapping", typeof(string), typeof(TaskAttributeMapping),
            new PropertyMetadata(string.Empty));

        #endregion

        #region RemainingCostMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string RemainingCostMapping
        {
            get { return (string)GetValue(RemainingCostMappingProperty); }
            set
            {
                SetValue(RemainingCostMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("RemainingCostMapping", value);
#endif
            }
        }

        public static readonly DependencyProperty RemainingCostMappingProperty =
            DependencyProperty.Register("RemainingCostMapping", typeof(string), typeof(TaskAttributeMapping),
            new PropertyMetadata(string.Empty));

        #endregion

        #region IsSummaryRowMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string IsSummaryRowMappingMapping
        {
            get { return (string)GetValue(IsSummaryRowMappingProperty); }
            set
            {
                SetValue(IsSummaryRowMappingProperty, value);

#if SILVERLIGHT
                OnPropertyChanged("IsSummaryRowMappingMapping", value);
#endif
            }
        }

        public static readonly DependencyProperty IsSummaryRowMappingProperty =
            DependencyProperty.Register("IsSummaryRowMapping", typeof(string), typeof(TaskAttributeMapping),
            new PropertyMetadata(string.Empty));

        #endregion

        #region ParentNodeMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string ParentNodeMapping
        {
            get { return (string)GetValue(ParentNodeMappingProperty); }
            set
            {
                SetValue(ParentNodeMappingProperty, value);

#if SILVERLIGHT
                OnPropertyChanged("ParentNodeMapping", value);
#endif
            }
        }

        public static readonly DependencyProperty ParentNodeMappingProperty =
            DependencyProperty.Register("ParentNodeMapping", typeof(string), typeof(TaskAttributeMapping),
            new PropertyMetadata(string.Empty));

        #endregion

        #region ChildMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDay mapping property on the bound object.
        /// </summary>
        public string ChildMapping
        {
            get { return (string)GetValue(ChildMappingProperty); }
            set
            {
                SetValue(ChildMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("ChildMapping", value);
#endif
            }
        }

        public static readonly DependencyProperty ChildMappingProperty =
            DependencyProperty.Register("ChildMapping", typeof(string), typeof(TaskAttributeMapping),
            new PropertyMetadata(string.Empty));

        #endregion

        #region InLineTaskMapping

        /// <summary>
        /// Gets or sets the in line task mapping.
        /// </summary>
        /// <value>The in line task mapping.</value>
        public string InLineTaskMapping
        {
            get { return (string)GetValue(InLineTaskMappingProperty); }
            set
            {
                SetValue(InLineTaskMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("InLineTaskMapping", value);
#endif
            }
        }

        // Using a DependencyProperty as the backing store for InLineTaskMapping.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InLineTaskMappingProperty =
            DependencyProperty.Register("InLineTaskMapping", typeof(string), typeof(TaskAttributeMapping), new PropertyMetadata(string.Empty));

        #endregion

        #region MileStoneMapping

        /// <summary>
        /// Gets or sets the mile stone mapping.
        /// </summary>
        /// <value>The mile stone mapping.</value>
        public string MileStoneMapping
        {
            get { return (string)GetValue(MileStoneMappingProperty); }
            set 
            { 
                SetValue(MileStoneMappingProperty, value);
#if SILVERLIGHT
                OnPropertyChanged("MileStoneMapping", value);
#endif
            }
        }

        // Using a DependencyProperty as the backing store for MileStoneMapping.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MileStoneMappingProperty =
            DependencyProperty.Register("MileStoneMapping", typeof(string), typeof(TaskAttributeMapping), new PropertyMetadata(string.Empty));

        #endregion
    }
}
