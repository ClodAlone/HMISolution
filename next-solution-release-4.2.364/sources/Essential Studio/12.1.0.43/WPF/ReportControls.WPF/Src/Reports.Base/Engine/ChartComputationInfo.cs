#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT
using System.Runtime.Serialization.Formatters;
#endif

namespace Syncfusion.RDL.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.ComponentModel;

    using System.Xml.Serialization;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Linq.Expressions;
    using System.Collections.ObjectModel;
    using System.Reflection;

    #region ChartComputationInfo class

    /// <summary>
    /// This class holds the information needed for the calculations that appear in a Chart Grid. For each calculation seen, there is an associated
    /// ChartComputationInfo object that is added to the <see cref="Syncfusion.RDL.Internal.ChartComputationInfo"/> collection.
    /// </summary>
    internal class ChartComputationInfo : INotifyPropertyChanged
    {
        #region Initilize/Finalize

        public ChartComputationInfo()
        {
            this.ComputationType = ComputationType.Count;
        }

        #endregion

        #region properties

        public string FieldName
        {
            get;
            set;
        }

        private string fieldMappingName;
        /// <summary>
        /// Gets or sets the name of the property to be used in this calculation.
        /// </summary>
        public string FieldMappingName
        {
            get
            {
                return fieldMappingName;
            }
            set
            {
                fieldMappingName = value;
                this.OnPropertyChanged(pi => pi.FieldMappingName);
            }
        }

        private string description;

        /// <summary>
        /// Gets or sets a description of the calculation.
        /// </summary>
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                this.OnPropertyChanged(pi => pi.Description);
            }
        }

        private string calculationName;

        /// <summary>
        /// Gets or sets what is displayed in the Chart table if more than one calculation is included in the Chart Grid.
        /// </summary>
        public string CalculationName
        {
            get
            {
                return calculationName;
            }
            set
            {
                calculationName = value;
                this.OnPropertyChanged(pi => pi.CalculationName);
            }
        }

        private SummaryBase summary;

        /// <summary>
        /// Gets or sets the <see cref="SummaryBase"/> object that is used to define this calculation. This value is altomatically set 
        /// when you specify any non-custom value of <see cref="Summary"/>. If you specify SummaryType.Custom, then you are required
        /// to set Summary to be an instance of your custom SummaryBase derived object.
        /// </summary>
        [XmlIgnore]
        public SummaryBase Summary
        {
            get
            {
                return summary;
            }
            set
            {
                summary = value;
                this.OnPropertyChanged(pi => pi.Summary);
            }
        }

        ComputationType _ComputationType;
        /// <summary>
        /// Gets or sets the SummaryType enumeration for this calculation. Setting it to any value of than Custom
        /// will also properly set Summary.
        /// </summary>
        [DefaultValue(ComputationType.Count)]
        public ComputationType ComputationType
        {
            get
            {
                return _ComputationType;
            }
            set
            {
                _ComputationType = value;
                this.summary = GetSummaryInstance(_ComputationType);
                this.OnPropertyChanged(pi => pi.ComputationType);
            }
        }

        /// <summary>
        /// Returns a <see cref="SummaryBase"/> object of the specified <see cref="GetSummaryInstance"/>.
        /// </summary>
        /// <param name="st">The SummaryType.</param>
        /// <returns>A SummaryBase object.</returns>
        public static SummaryBase GetSummaryInstance(ComputationType st)
        {
            SummaryBase sb = null;
            switch (st)
            {
                case ComputationType.IntTotalSum:
                    sb = new IntTotalSummary();
                    break;
                case ComputationType.DecimalTotalSum:
                    sb = new DecimalTotalSummary();
                    break;
                case ComputationType.DoubleTotalSum:
                    sb = new DoubleTotalSummary();
                    break;
                case ComputationType.DoubleAverage:
                    sb = new DoubleAverageSummary();
                    break;
                case ComputationType.Count:
                    sb = new CountSummary();
                    break;
                case ComputationType.DoubleMinimum:
                    sb = new DoubleMinSummary();
                    break;
                case ComputationType.DoubleMaximum:
                    sb = new DoubleMaxSummary();
                    break;
                case ComputationType.DoubleStandardDeviation:
                    sb = new DoubleStDevSummary();
                    break;
                case ComputationType.DoubleVariance:
                    sb = new DoubleVarianceSummary();
                    break;
                case ComputationType.Text:
                    sb = new TextSummary();
                    break;
                case ComputationType.Custom:
                default:
                    break;
            }
            return sb;
        }

        private string format = "#.##";

        /// <summary>
        /// Gets of sets the format string to be used to format this calculation results in the Chart Grid.
        /// </summary>
        public string Format
        {
            get
            {
                return format;
            }
            set
            {
                format = value;
                this.OnPropertyChanged(pi => pi.Format);
            }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return this.CalculationName;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged<R>(Expression<Func<ChartComputationInfo, R>> expr)
        {
            OnPropertyChanged(((MemberExpression)expr.Body).Member.Name);
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        /// <summary>
        /// Returns a sorted list of computation names based on the <see cref="GetComputationTypes"/> enumerations.
        /// </summary>
        /// <returns>A list of computation names.</returns>
        public static List<string> GetComputationTypes()
        {
#if SILVERLIGHT
            List<string> list = new List<string>(GetEnumNames(typeof(ComputationType)));
#else
            List<string> list = new List<string>(Enum.GetNames(typeof(ComputationType)));
#endif
            list.Remove("Custom");
            list.Sort();
            return list;
        }

#if SILVERLIGHT

        private static string[] GetEnumNames(Type t)
        {
            List<string> enumNames = new List<string>();
#if WINRT
            foreach (System.Reflection.FieldInfo fi in t.GetTypeInfo().DeclaredFields)
            {
                enumNames.Add(fi.Name);
            }
#else            
            foreach (System.Reflection.FieldInfo fi in t.GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumNames.Add(fi.Name);
            }
#endif
            return enumNames.ToArray();
        }
#endif

    }

    #endregion
}
