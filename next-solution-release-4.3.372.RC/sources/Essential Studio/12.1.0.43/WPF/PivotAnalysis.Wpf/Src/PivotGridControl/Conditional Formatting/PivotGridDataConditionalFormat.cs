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
using Syncfusion.Windows.Data;
using Syncfusion.PivotAnalysis.Base;
using System.Linq.Expressions;
using Syncfusion.Linq;
using System.Collections.ObjectModel;


#if !SILVERLIGHT
namespace Syncfusion.Windows.Controls.PivotGrid
#else
using Syncfusion.PivotAnalysis.Base.Silverlight;
namespace Syncfusion.Silverlight.Controls.PivotGrid
#endif
{
    /// <summary>
    /// Defines the conditional formatting setting for <see cref="PivotGridControl"/>
    /// </summary>    
    public class PivotGridDataConditionalFormat  
#if !SILVERLIGHT
       : Freezable
#else
       : DependencyObject
#endif
    {
        #region [ Private Members ]
        Dictionary<string, Delegate> delegateValues = new Dictionary<string, Delegate>();
        #endregion

        #region [ Initialize/Finalize ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PivotGridDataConditionalFormat"/> class.
        /// </summary>
        public PivotGridDataConditionalFormat()
        {
#if !SILVERLIGHT
            this.Conditions = new FreezableCollection<PivotGridDataCondition>();
#else
            this.Conditions = new ObservableCollection<PivotGridDataCondition>();
#endif
        }

        #endregion

        #region [ Dependency Property Declaration ]

#if !SILVERLIGHT
        /// <summary>
        /// DependencyProperty for <see cref="PivotGridDataConditionalFormat.Conditions"/> property.
        /// </summary>
        public static readonly DependencyProperty ConditionsProperty = 
            DependencyProperty.Register("Conditions", typeof(FreezableCollection<PivotGridDataCondition>), typeof(PivotGridDataConditionalFormat));
#endif

        /// <summary>
        /// DependencyProperty for <see cref="PivotGridDataConditionalFormat.ValueCellType"/> property.
        /// </summary>
        public static readonly DependencyProperty ValueCellTypeProperty =
            DependencyProperty.Register("ValueCellType", typeof(PivotGridValueCellType), typeof(PivotGridDataConditionalFormat), new PropertyMetadata(PivotGridValueCellType.All));

        /// <summary>
        /// DependencyProperty for <see cref="PivotGridDataConditionalFormat.Name"/> property.
        /// </summary>
        public static readonly DependencyProperty NameProperty = 
            DependencyProperty.Register("Name", typeof(string), typeof(PivotGridDataConditionalFormat), new PropertyMetadata(string.Empty));

        /// <summary>
        /// DependencyProperty for <see cref="PivotGridDataConditionalFormat.CellStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty StyleProperty = 
            DependencyProperty.Register("CellStyle", typeof(PivotGridCellStyle), typeof(PivotGridDataConditionalFormat), new PropertyMetadata(null));
     
        #endregion

        #region [ Public Properties ]

        /// <summary>
        /// Gets or sets the type of the value cell.
        /// </summary>
        /// <value>The type of the value cell.</value>
        public PivotGridValueCellType ValueCellType
        {
            get { return (PivotGridValueCellType)GetValue(ValueCellTypeProperty); }
            set { SetValue(ValueCellTypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return (string)this.GetValue(PivotGridDataConditionalFormat.NameProperty);
            }

            set
            {
                this.SetValue(PivotGridDataConditionalFormat.NameProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the <see cref="PivotGridCellStyle"/> for the specified condition.
        /// </summary>
        /// <value>The style.</value>
        public PivotGridCellStyle CellStyle
        {
            get
            {
                return (PivotGridCellStyle)this.GetValue(PivotGridDataConditionalFormat.StyleProperty);
            }

            set
            {
                this.SetValue(PivotGridDataConditionalFormat.StyleProperty, value);
            }
        }

#if !SILVERLIGHT

        /// <summary>
        /// Gets or sets the conditions.
        /// </summary>
        /// <value>The conditions.</value>
        public FreezableCollection<PivotGridDataCondition> Conditions
        {
            get
            {
                return (FreezableCollection<PivotGridDataCondition>)GetValue(PivotGridDataConditionalFormat.ConditionsProperty);
            }
            set
            {
                SetValue(PivotGridDataConditionalFormat.ConditionsProperty, value);
            }
        }
#else
        /// <summary>
        /// Gets or sets the conditions.
        /// </summary>
        /// <value>The conditions.</value>
        public ObservableCollection<PivotGridDataCondition> Conditions
        {
            get;
            set;
        }

#endif
        #endregion

        #region [ Helper Methods ]

        /// <summary>
        /// Checks whether the current cell qualifies for conditional formatting
        /// </summary>
        /// <param name="cellInfo">The cell info.</param>
        /// <param name="associatedMeasure">The associated measure.</param>
        /// <returns>True, if current cell qualifies for conditional formatting; False, otherwise.</returns>
        internal bool ApplyFormat(PivotCellInfo cellInfo,string associatedMeasure)
        {
            Delegate del = null;
            if (delegateValues.Count > 0)
            {
                if (delegateValues.ContainsKey(associatedMeasure))
                {
                    del = delegateValues[associatedMeasure];
                }
            }

            if (del == null)
            {
                Delegate comDelegate = this.GetCompliedDelegate(cellInfo, associatedMeasure);
                if (comDelegate != null)
                {
                    return (bool)comDelegate.DynamicInvoke(new object[] { cellInfo });
                }
                else
                    return false;
            }
            else
            {
                return (bool)del.DynamicInvoke(new object[] { cellInfo });
            }           
        }

        /// <summary>
        /// Gets the complied delegate.
        /// </summary>
        /// <param name="cellInfo">The cell info.</param>
        /// <param name="associatedMeasure">The associated measure.</param>
        /// <returns>a delegate</returns>
        private Delegate GetCompliedDelegate(PivotCellInfo cellInfo,string associatedMeasure)
        {
            if (this.Conditions.Count > 0)
            {
                bool firstLoop = false;
                System.Linq.Expressions.Expression predicate = null;
                Type recordType = cellInfo.GetType();
                ParameterExpression paramExp = recordType.Parameter();

                foreach (var condition in this.Conditions)
                {
                    if (condition.SummaryElement == associatedMeasure)
                    {
                        if (!firstLoop)
                        {
                            predicate = Predicate(paramExp, condition.ConditionType, condition.Value);
                            firstLoop = true;
                        }
                        else
                        {
                            if (condition.PredicateType == PredicateType.And)
                            {
                                predicate = predicate.AndAlsoPredicate(Predicate(paramExp, condition.ConditionType, condition.Value));
                            }
                            else if (condition.PredicateType == PredicateType.Or)
                            {
                                predicate = predicate.OrElsePredicate(Predicate(paramExp, condition.ConditionType, condition.Value));
                            }
                        }
                    }
                }
                if (predicate != null)
                {
                    var lambda = System.Linq.Expressions.Expression.Lambda(predicate, paramExp);
                    delegateValues.Add(associatedMeasure, lambda.Compile());
                    return this.delegateValues[associatedMeasure];
                }
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// Returns the Predicate based on ConditionType and Parameter Expression
        /// </summary>
        /// <param name="paramExp">The param exp.</param>
        /// <param name="pivotGridDataConditionType">Type of the pivot grid data condition.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private System.Linq.Expressions.Expression Predicate(ParameterExpression paramExp, PivotGridDataConditionType pivotGridDataConditionType, object value)
        {
            System.Linq.Expressions.Expression predicate = null;
            switch (pivotGridDataConditionType)
            { 
                case PivotGridDataConditionType.Equals:
                    predicate = paramExp.Equal("DoubleValue", value);
                    break;
                case PivotGridDataConditionType.GreaterThan:
                    predicate = paramExp.GreaterThan("DoubleValue", value);
                    break;                    
                case PivotGridDataConditionType.GreaterThanOrEqual:
                    predicate = paramExp.GreaterThanOrEqual("DoubleValue", value);
                    break;
                case PivotGridDataConditionType.LessThan:
                    predicate = paramExp.LessThan("DoubleValue", value);
                    break;
                case PivotGridDataConditionType.LessThanOrEqual:
                    predicate = paramExp.LessThanOrEqual("DoubleValue", value);
                    break;
                case PivotGridDataConditionType.NotEquals:
                    predicate = paramExp.NotEqual("DoubleValue", value);
                    break;
            }
            return predicate;
        }

        #endregion

#if !SILVERLIGHT
        #region [ Overrides ]
        /// <summary>
        /// When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable"/> derived class.
        /// </summary>
        /// <returns>The new instance.</returns>
        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
        #endregion
#endif       
    }

    /// <summary>
    /// Define the condition settings for <see cref="PivotGridControl"/> to apply.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class PivotGridDataCondition
#if !SILVERLIGHT
 : Freezable
#else
 : DependencyObject
#endif
    {
        #region [ Initialize / Finalize ]
        /// <summary>
        /// Initializes a new instance of the <see cref="PivotGridDataCondition"/> class.
        /// </summary>
        public PivotGridDataCondition()
        {
            this.ConditionType = PivotGridDataConditionType.Equals;
            this.PredicateType = PredicateType.Or;
        }
        #endregion

        #region [ Dependency Property Declaration ]
        /// <summary>
        /// DependencyProperty for <see cref="PivotGridDataCondition.ConditionType"/> property.
        /// </summary>
        public static readonly DependencyProperty PivotGridDataConditionTypeProperty = 
            DependencyProperty.Register("PivotGridDataConditionType", typeof(PivotGridDataConditionType), typeof(PivotGridDataCondition), new PropertyMetadata(PivotGridDataConditionType.Equals));

        /// <summary>
        /// DependencyProperty for <see cref="PivotGridDataCondition.Value"/> property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = 
            DependencyProperty.Register("Value", typeof(object), typeof(PivotGridDataCondition), new PropertyMetadata(null));

        /// <summary>
        /// DependencyProperty for <see cref="PredicateType"/> property.
        /// </summary>
        public static readonly DependencyProperty PredicateTypeProperty = 
            DependencyProperty.Register("PredicateType", typeof(PredicateType), typeof(PivotGridDataCondition), new PropertyMetadata(PredicateType.And));

        /// <summary>
        /// DependencyProperty for <see cref="PivotGridDataCondition.SummaryElement"/> property.
        /// </summary>
        public static readonly DependencyProperty SummaryElementProperty = 
            DependencyProperty.Register("SummaryElement", typeof(string), typeof(PivotGridDataCondition), new PropertyMetadata(string.Empty));

        #endregion

        #region [ Public Properties ]
        
        /// <summary>
        /// Gets or sets the type of the condition.
        /// </summary>
        /// <value>The type of the condition.</value>
        public PivotGridDataConditionType ConditionType
        {
            get
            {
                return (PivotGridDataConditionType)this.GetValue(PivotGridDataCondition.PivotGridDataConditionTypeProperty);
            }

            set
            {
                this.SetValue(PivotGridDataCondition.PivotGridDataConditionTypeProperty, value);
            }
        }
    
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get
            {
                return this.GetValue(PivotGridDataCondition.ValueProperty);
            }

            set
            {
                this.SetValue(PivotGridDataCondition.ValueProperty, value);
            }
        }
    
        /// <summary>
        /// Gets or sets the type of the predicate.
        /// </summary>
        /// <value>The type of the predicate.</value>
        public PredicateType PredicateType
        {
            get
            {
                return (PredicateType)this.GetValue(PivotGridDataCondition.PredicateTypeProperty);
            }

            set
            {
                this.SetValue(PivotGridDataCondition.PredicateTypeProperty, value);
            }
        }
      
        /// <summary>
        /// Gets or sets the name of summary element for which formatting need to be applied.
        /// </summary>
        /// <value>The name of the summary element.</value>
        public string SummaryElement
        {
            get
            {
                return (string)this.GetValue(PivotGridDataCondition.SummaryElementProperty);
            }

            set
            {
                this.SetValue(PivotGridDataCondition.SummaryElementProperty, value);
            }
        }

        #endregion

#if !SILVERLIGHT

        #region [ Overrides ]
        /// <summary>
        /// When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable"/> derived class.
        /// </summary>
        /// <returns>The new instance.</returns>
        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
        #endregion
#endif
    }

    /// <summary>
    /// Specifies the type of condition to be used in Conditional Formatting.
    /// </summary>
    public enum PivotGridDataConditionType
    {
        /// <summary>
        /// Performs an Equals operation on the operands.
        /// </summary>
        Equals,
        /// <summary>
        /// Performs a NotEquals operation on the operands.
        /// </summary>
        NotEquals,
        /// <summary>
        /// Performs a LessThan operation on the operands.
        /// </summary>
        LessThan,
        /// <summary>
        /// Performs a LessThanOrEqual operation on the operands.
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// Performs a GreatherThan operation on the operands.
        /// </summary>
        GreaterThan,
        /// <summary>
        /// Performs a GreaterThanOrEqual operation on the operands.
        /// </summary>
        GreaterThanOrEqual        
    }
}
