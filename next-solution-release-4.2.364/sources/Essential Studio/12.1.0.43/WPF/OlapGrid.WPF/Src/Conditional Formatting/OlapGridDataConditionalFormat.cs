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
using Syncfusion.Olap.Engine;
using Syncfusion.Windows.Data;
using System.Linq.Expressions;
using Syncfusion.Linq;
using System.Collections.ObjectModel;

#if !SILVERLIGHT
namespace Syncfusion.Windows.Grid.Olap
#else
using Syncfusion.OlapSilverlight.Engine;
namespace Syncfusion.Silverlight.Grid.Olap
#endif
{
    /// <summary>
    /// Defines the conditional formatting setting for <see cref="OlapGrid"/>
    /// </summary>            
    public class OlapGridDataConditionalFormat  
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
        /// Initializes a new instance of the <see cref="OlapGridDataConditionalFormat"/> class.
        /// </summary>
        public OlapGridDataConditionalFormat()
        {
#if !SILVERLIGHT
            this.Conditions = new FreezableCollection<OlapGridDataCondition>();
#else
            this.Conditions = new ObservableCollection<OlapGridDataCondition>();
#endif
        }

        #endregion

        #region [ Dependency Property Declaration ]

#if !SILVERLIGHT
        /// <summary>
        /// DependencyProperty for <see cref="OlapGridDataConditionalFormat.Conditions"/> property.
        /// </summary>
        public static readonly DependencyProperty ConditionsProperty =
            DependencyProperty.Register("Conditions", typeof(FreezableCollection<OlapGridDataCondition>), typeof(OlapGridDataConditionalFormat));
#endif

        /// <summary>
        /// DependencyProperty for <see cref="OlapGridDataConditionalFormat.Name"/> property.
        /// </summary>
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(OlapGridDataConditionalFormat), new PropertyMetadata(string.Empty));

        /// <summary>
        /// DependencyProperty for <see cref="OlapGridDataConditionalFormat.CellStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty StyleProperty =
            DependencyProperty.Register("CellStyle", typeof(OlapGridCellStyle), typeof(OlapGridDataConditionalFormat), new PropertyMetadata(null));

        #endregion

        #region [ Public Properties ]

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return (string)this.GetValue(OlapGridDataConditionalFormat.NameProperty);
            }

            set
            {
                this.SetValue(OlapGridDataConditionalFormat.NameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="OlapGridCellStyle"/> for the specified condition.
        /// </summary>
        /// <value>The style.</value>
        public OlapGridCellStyle CellStyle
        {
            get
            {
                return (OlapGridCellStyle)this.GetValue(OlapGridDataConditionalFormat.StyleProperty);
            }

            set
            {
                this.SetValue(OlapGridDataConditionalFormat.StyleProperty, value);
            }
        }

#if !SILVERLIGHT

        /// <summary>
        /// Gets or sets the conditions.
        /// </summary>
        /// <value>The conditions.</value>
        public FreezableCollection<OlapGridDataCondition> Conditions
        {
            get
            {
                return (FreezableCollection<OlapGridDataCondition>)GetValue(OlapGridDataConditionalFormat.ConditionsProperty);
            }
            set
            {
                SetValue(OlapGridDataConditionalFormat.ConditionsProperty, value);
            }
        }
#else
        /// <summary>
        /// Gets or sets the conditions.
        /// </summary>
        /// <value>The conditions.</value>
        public ObservableCollection<OlapGridDataCondition> Conditions
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
        /// <returns></returns>
        internal bool ApplyFormat(PivotCellDescriptor cellInfo, string associatedMeasure)
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
        /// <returns></returns>
        private Delegate GetCompliedDelegate(PivotCellDescriptor cellInfo, string associatedMeasure)
        {
            if (this.Conditions.Count > 0)
            {
                bool firstLoop = false;
                System.Linq.Expressions.Expression predicate = null;
                Type recordType = cellInfo.GetType();
                ParameterExpression paramExp = recordType.Parameter();

                foreach (var condition in this.Conditions)
                {
                    if (condition.MeasureElement == associatedMeasure)
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
        /// <param name="pivotGridDataConditionType">Type of the Olap grid data condition.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private System.Linq.Expressions.Expression Predicate(ParameterExpression paramExp, OlapGridDataConditionType pivotGridDataConditionType, object value)
        {
            System.Linq.Expressions.Expression predicate = null;
            switch (pivotGridDataConditionType)
            {
                case OlapGridDataConditionType.Equals:
                    predicate = paramExp.Equal("DoubleValue", value);
                    break;
                case OlapGridDataConditionType.GreaterThan:
                    predicate = paramExp.GreaterThan("DoubleValue", value);
                    break;
                case OlapGridDataConditionType.GreaterThanOrEqual:
                    predicate = paramExp.GreaterThanOrEqual("DoubleValue", value);
                    break;
                case OlapGridDataConditionType.LessThan:
                    predicate = paramExp.LessThan("DoubleValue", value);
                    break;
                case OlapGridDataConditionType.LessThanOrEqual:
                    predicate = paramExp.LessThanOrEqual("DoubleValue", value);
                    break;
                case OlapGridDataConditionType.NotEquals:
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
    /// Define the condition settings for <see cref="OlapGrid"/> to apply.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class OlapGridDataCondition
#if !SILVERLIGHT
 : Freezable
#else
 : DependencyObject
#endif
    {
        #region [ Initialize / Finalize ]
        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridDataCondition"/> class.
        /// </summary>
        public OlapGridDataCondition()
        {
            this.ConditionType = OlapGridDataConditionType.Equals;
            this.PredicateType = PredicateType.Or;
        }
        #endregion

        #region [ Dependency Property Declaration ]
        /// <summary>
        /// DependencyProperty for <see cref="OlapGridDataCondition.ConditionType"/> property.
        /// </summary>
        public static readonly DependencyProperty OlapGridDataConditionTypeProperty =
            DependencyProperty.Register("OlapGridDataConditionType", typeof(OlapGridDataConditionType), typeof(OlapGridDataCondition), new PropertyMetadata(OlapGridDataConditionType.Equals));

        /// <summary>
        /// DependencyProperty for <see cref="OlapGridDataCondition.Value"/> property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(OlapGridDataCondition), new PropertyMetadata(null));

        /// <summary>
        /// DependencyProperty for <see cref="PredicateType"/> property.
        /// </summary>
        public static readonly DependencyProperty PredicateTypeProperty =
            DependencyProperty.Register("PredicateType", typeof(PredicateType), typeof(OlapGridDataCondition), new PropertyMetadata(PredicateType.And));

        /// <summary>
        /// DependencyProperty for <see cref="OlapGridDataCondition.MeasureElement"/> property.
        /// </summary>
        public static readonly DependencyProperty MeasureElementProperty =
            DependencyProperty.Register("MeasureElement", typeof(string), typeof(OlapGridDataCondition), new PropertyMetadata(string.Empty));

        #endregion

        #region [ Public Properties ]
        /// <summary>
        /// Gets or sets the type of the condition.
        /// </summary>
        /// <value>The type of the condition.</value>
        public OlapGridDataConditionType ConditionType
        {
            get
            {
                return (OlapGridDataConditionType)this.GetValue(OlapGridDataCondition.OlapGridDataConditionTypeProperty);
            }

            set
            {
                this.SetValue(OlapGridDataCondition.OlapGridDataConditionTypeProperty, value);
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
                return this.GetValue(OlapGridDataCondition.ValueProperty);
            }

            set
            {
                this.SetValue(OlapGridDataCondition.ValueProperty, value);
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
                return (PredicateType)this.GetValue(OlapGridDataCondition.PredicateTypeProperty);
            }

            set
            {
                this.SetValue(OlapGridDataCondition.PredicateTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of measure element for which formatting need to be applied.
        /// </summary>
        /// <value>The name of the measure element.</value>
        public string MeasureElement
        {
            get
            {
                return (string)this.GetValue(OlapGridDataCondition.MeasureElementProperty);
            }

            set
            {
                this.SetValue(OlapGridDataCondition.MeasureElementProperty, value);
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
    public enum OlapGridDataConditionType
    {
        /// <summary>
        /// Performs an Equals opertion on the operands.
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
