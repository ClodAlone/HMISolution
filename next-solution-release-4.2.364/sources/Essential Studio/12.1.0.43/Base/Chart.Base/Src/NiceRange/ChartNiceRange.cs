#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Globalization;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{
    #region interface INiceRangeMaker
    /// <summary>
    /// Interface that defines preferences and access methods used for the automatic calculation of 'nice' range
    /// given any range of data. 'Nice' ranges are generally more easily understood in comparison to 'raw' data.    
    /// </summary>
    internal interface INiceRangeMaker
    {
        /// <summary>
        /// Gets or sets the approximate number of intervals into which the range is to be partitioned. The actual number of
        /// intervals calculated will depend on the actual algorithm used.
        /// </summary>
        int DesiredIntervals { get; set; }

        /// <summary>
        /// Gets or sets the padding type that will be applied for calculating the ranges for this axis.
        /// </summary>
        ChartAxisRangePaddingType RangePaddingType { get; set; }

        /// <summary>
        /// Indicates whether one boundary of the calculated range should be tweaked to zero. Such tweaking will happen
        /// only if zero is within a resonable distance from the calculated boundary. To ensure that one boundary
        /// is always zero, use the <see cref="ForceZero"/> setting instead.
        /// </summary>
        bool PreferZero { get; set; }

        /// <summary>
        /// Gets or Sets the result whether one boundary of the calculated range should always be tweaked to zero. 
        /// </summary>
        bool ForceZero { get; set; }

        /// <summary>
        /// Given a minimum value and a maximum value, this method will calculate a 'nice' minimum value and a maxiumum value
        /// as well as an interval value that can be used for visually representing this data. 'Nice' values are better
        /// perceived by humans. For example, consider a range 1.21-3.5. A nice range that we can use to visually represent values
        /// in this range can be 0-3.6 with an interval of 2. You can tweak the results obtained by changing optional settings.
        /// </summary>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <param name="rangePaddingType">The range padding type.</param>
        /// <returns>Calculated <see cref="Syncfusion.Windows.Forms.Chart.MinMaxInfo"/>.</returns>
        MinMaxInfo MakeNiceRange(double min, double max, ChartAxisRangePaddingType rangePaddingType);
    }
    #endregion

    #region class MinMaxInfo
    /// <summary>
    /// Simple, unchangeable class to store information on minimum and maximum values and a suggested interval.
    /// </summary>
    [TypeConverter(typeof(MinMaxInfoConverter))]
    public class MinMaxInfo
    {
        #region Internal members
        /// <summary>
        /// Store min value of range.
        /// </summary>
        internal double min;

        /// <summary>
        /// Store max value of the range.
        /// </summary>
        internal double max;

        /// <summary>
        /// Store interval.
        /// </summary>
        internal double interval;
        #endregion

        #region Public methods
        /// <summary>
        /// Creates a copy of the MinMaxInfo object.
        /// </summary>
        /// <returns>The MinMaxInfo.</returns>
        public MinMaxInfo Clone()
        {
            return new MinMaxInfo(min, max, interval);
        }

        /// <summary>
        ///  Compares this object with another object of the same type.
        /// </summary>
        /// <param name="minMaxInfo" type="Syncfusion.Windows.Forms.Chart.MinMaxInfo">
        ///     <para>
        ///     The object with which this object is to be compared.
        ///     </para>
        /// </param>
        /// <returns>
        ///     Returns True if the objects are equal in value.
        /// </returns>
        public bool Equals(MinMaxInfo minMaxInfo)
        {
            return this.min == minMaxInfo.Min && this.max == minMaxInfo.Max && this.Interval == minMaxInfo.Interval;
        }

        /// <summary>
        /// Checks whether range contains double value.
        /// </summary>
        /// <param name="d">Double to check</param>
        /// <returns>Bool value.</returns>
        public bool Contains(double d)
        {
            return (this.min <= d) && (this.max >= d);
        }

        /// <summary>
        /// Checks whether range intersects with range.
        /// </summary>
        /// <param name="r">MinMaxInfo to check</param>
        /// <returns>Bool value.</returns>
        public bool Intersects(MinMaxInfo r)
        {
            return this.Contains(r.Min) || this.Contains(r.Max) || r.Contains(this.Min);
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when one of the range settings was changed.
        /// </summary>
        /// <internalonly/>
        [DocumentationExclude()]
        public event EventHandler SettingsChanged;
        #endregion

        #region Constructor
        /// <summary>
        /// MinMaxInfo represents a range of double type values. There is a lower bound, upper bound and an associated interval.
        /// </summary>
        /// <param name="min" type="double">
        ///     <para>
        ///     The lower bound value.
        ///     </para>
        /// </param>
        /// <param name="max" type="double">
        ///     <para>
        ///     The upper bound value.
        ///     </para>
        /// </param>
        /// <param name="interval" type="double">
        ///     <para>
        ///     The interval value.
        ///     </para>
        /// </param>
        public MinMaxInfo(double min, double max, double interval)
        {
           if (double.IsNaN(min) || double.IsNaN(max))
            {
                this.min =0;
                this.max =5;
                this.interval =1;
            }
            else
            {
                this.min = min;
                this.max = max;
                this.Interval = interval;
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// gets the difference between the upper and lower boundary of this range.
        /// </summary>
        [Browsable(false)]
        [Description("Returns the difference between the upper and lower boundary of this range.")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public double Delta
        {
            get
            {
                return max - min;
            }
        }

        /// <summary>
        /// Gets or sets the lower boundary of this range.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)] 
        public double Min
        {
            get
            {
                return this.min;
            }

            set
            {

                if (min != value)
                {
                    this.min = value;
                    this.OnSettingsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the upper boundary of this range.
        /// </summary>
        [Description("Gets or sets the upper boundary of this range.")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public double Max
        {
            get
            {
                return this.max;
            }

            set
            {
                if (max != value)
                {
                    this.max = value;
                    this.OnSettingsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the value of the interval associated with this range.
        /// </summary>
        [Description("Gets or sets the value of the interval associated with this range.")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public double Interval
        {
            get
            {
                return this.interval;
            }

            set
            {
                if (interval != value)
                {
                    if (value > 0)
                        this.interval = value;
                    this.OnSettingsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the number of intervals present in this range.
        /// <see cref="MinMaxInfo.Interval"/>
        /// </summary>
        [Browsable(false)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public int NumberOfIntervals
        {
            get
            {
                int count = (int)(Math.Round((max - min) / interval));
                // Interval can't be zero
                return count == 0 ? 1 : count;
            }
        }

        #endregion

        #region Overrides
        /// <summary>
        /// Raises the settings changed event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <internalonly/>
        [DocumentationExclude()]
        protected virtual void OnSettingsChanged(EventArgs e)
        {
            if (this.SettingsChanged != null)
            {
                this.SettingsChanged(this, e);
            }
        }

        /// <summary>
        /// Overridden. Returns a string representation of this object.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return new MinMaxInfoConverter().ConvertToString(this);
            //return string.Format( "Min: {0:G}, Max: {1:G}, Interval: {2:f8}", this.min, this.max, this.interval );
        }
        #endregion
    }
    #endregion

    #region class MinMaxInfoConverter
    /// <summary>
    /// Converts instances of other types to and from a <see cref="MinMaxInfo"/>. 
    /// </summary>
    public class MinMaxInfoConverter : TypeConverter
    {
        #region Implementation

        /// <summary>
        /// Returns a collection of properties for the type of array specified by the value parameter, using the specified context and attributes.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="value">An <see cref="T:System.Object"/> that specifies the type of array for which to get properties.</param>
        /// <param name="attributes">An array of type <see cref="T:System.Attribute"/> that is used as a filter.</param>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> with the properties that are exposed for this data type, or null if there are no properties.
        /// </returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection prts = TypeDescriptor.GetProperties(typeof(MinMaxInfo), attributes);
            string[] sortList = new string[] { "Min", "Max", "Interval" };
            return prts.Sort(sortList);
        }

        /// <summary>
        /// Returns whether this object supports properties, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <returns>
        /// true if <see cref="M:System.ComponentModel.TypeConverter.GetProperties(System.Object)"/> should be called to find the properties of this object; otherwise, false.
        /// </returns>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Returns whether changing a value on this object requires a call to <see cref="M:System.ComponentModel.TypeConverter.CreateInstance(System.Collections.IDictionary)"/> to create a new value, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <returns>
        /// true if changing a property on this object requires a call to <see cref="M:System.ComponentModel.TypeConverter.CreateInstance(System.Collections.IDictionary)"/> to create a new value; otherwise, false.
        /// </returns>
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Creates an instance of the type that this <see cref="T:System.ComponentModel.TypeConverter"/> is associated with, using the specified context, given a set of property values for the object.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="propertyValues">An <see cref="T:System.Collections.IDictionary"/> of new property values.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> representing the given <see cref="T:System.Collections.IDictionary"/>, or null if the object cannot be created. This method always returns null.
        /// </returns>
        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            return new MinMaxInfo((double)propertyValues["Min"], (double)propertyValues["Max"], (double)propertyValues["Interval"]);
        }

        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            bool res = base.CanConvertFrom(context, sourceType);

            if (sourceType == typeof(string))
            {
                res = true;
            }

            return res;
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            bool res = base.CanConvertTo(context, destinationType);

            if (destinationType == typeof(string))
            {
                res = true;
            }

            return true;
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The conversion cannot be performed.
        /// </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            object res = null;

            if (value is string)
            {
                string sValue = ((string)value).Trim();

                if (sValue.Length != 0)
                {
                    culture = culture == null ? CultureInfo.CurrentCulture : culture;
                    char[] separators = new char[] { culture.TextInfo.ListSeparator[0] };
                    string[] sAttrs = sValue.Split(separators);
                    double[] doubleAttrs = new double[sAttrs.Length];
                    TypeConverter doubleConverter = TypeDescriptor.GetConverter(typeof(double));

                    for (int i = 0; i < doubleAttrs.Length; i++)
                    {
                        doubleAttrs[i] = (double)doubleConverter.ConvertFromString(context, culture, sAttrs[i]);
                    }

                    if (doubleAttrs.Length == 3)
                    {
                        res = new MinMaxInfo(doubleAttrs[0], doubleAttrs[1], doubleAttrs[2]);
                    }
                }
            }
            else
            {
                res = base.ConvertFrom(context, culture, value);
            }

            return res;
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="destinationType"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The conversion cannot be performed.
        /// </exception>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            object res = null;

            if (value is MinMaxInfo)
            {
                MinMaxInfo minMaxInfo = value as MinMaxInfo;

                if (destinationType == typeof(string))
                {
                    culture = culture == null ? CultureInfo.CurrentCulture : culture;
                    string separator = culture.TextInfo.ListSeparator[0] + " ";
                    TypeConverter doubleConverter = TypeDescriptor.GetConverter(typeof(double));

                    res = string.Join(separator, new string[]{
                                                      doubleConverter.ConvertToString( context, culture, minMaxInfo.Min ),
                                                      doubleConverter.ConvertToString( context, culture, minMaxInfo.Max ),
                                                      doubleConverter.ConvertToString( context, culture, minMaxInfo.Interval )
                                                    });
                }
                else if (destinationType == typeof(InstanceDescriptor))
                {
                    Type[] intTypes = new Type[] { typeof(double), typeof(double), typeof(double) };
                    ConstructorInfo cnstrInfo = typeof(MinMaxInfo).GetConstructor(intTypes);

                    if (cnstrInfo != null)
                    {
                        object[] attrs = new object[] { minMaxInfo.Min, minMaxInfo.Max, minMaxInfo.Interval };
                        res = new InstanceDescriptor(cnstrInfo, attrs);
                    }
                }
            }
            else
            {
                res = base.ConvertTo(context, culture, value, destinationType);
            }

            return res;
        }
        #endregion
    }
    #endregion

    #region class NiceRangeMaker
    /// <summary>
    /// Provides the methods to compute the 'nice' range.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class NiceRangeMaker : INiceRangeMaker
    {
        #region Constants
        private const int DESIRED_INTERVALS = 12;
        #endregion

        #region Helper class OpState
        /// <summary>
        /// This class holds operational states (intermediate calculated values, support values, etc).
        /// </summary>
        protected class OpState
        {
            private NiceRangeMaker parent;

            // Original values.
            private double min;
            private double max;
            private double interval;

            // Calculated values. Could be transitional.
            private double calcMin;
            private double calcMax;
            private double calcInterval;

            // Indicates the number of power of 10 that the calculated values have
            // been raised to.
            private int adjustedPlaces;

            /// <summary>
            /// Initializes a new instance of the <see cref="OpState"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            /// <param name="min">The min.</param>
            /// <param name="max">The max.</param>
            /// <internalonly/>
            public OpState(NiceRangeMaker parent, double min, double max)
            {
                this.parent = parent;

                // Original values.
                this.min = min;
                this.max = max;
                this.interval = (this.max - this.min) / this.parent.DesiredIntervals;

                // Initially the calculated values are the same as the original values.
                this.calcMin = min;
                this.calcMax = max;
                this.calcInterval = this.Interval;
            }

            /// <summary>
            /// Gets the min.
            /// </summary>
            /// <value>The min.</value>
            /// <internalonly/>
            public double Min
            {
                get
                {
                    return this.min;
                }
            }

            /// <summary>
            /// Gets the max.
            /// </summary>
            /// <value>The max.</value>
            /// <internalonly/>
            public double Max
            {
                get
                {
                    return this.max;
                }
            }

            /// <summary>
            /// Gets the interval.
            /// </summary>
            /// <value>The interval.</value>
            /// <internalonly/>
            public double Interval
            {
                get
                {
                    return this.interval;
                }
            }

            /// <summary>
            /// Gets or sets the calc min.
            /// </summary>
            /// <value>The calc min.</value>
            /// <internalonly/>
            public double CalcMin
            {
                get
                {
                    return this.calcMin;
                }

                set
                {
                    this.calcMin = value;
                }
            }

            /// <summary>
            /// Gets or sets the calc max.
            /// </summary>
            /// <value>The calc max.</value>
            /// <internalonly/>
            public double CalcMax
            {
                get
                {
                    return this.calcMax;
                }

                set
                {
                    this.calcMax = value;
                }
            }

            /// <summary>
            /// Gets or sets the calc interval.
            /// </summary>
            /// <value>The calc interval.</value>
            /// <internalonly/>
            public double CalcInterval
            {
                get
                {
                    return this.calcInterval;
                }

                set
                {
                    this.calcInterval = value;
                }
            }

            /// <summary>
            /// Gets or sets the adjusted places.
            /// </summary>
            /// <value>The adjusted places.</value>
            /// <internalonly/>
            public int AdjustedPlaces
            {
                get
                {
                    return this.adjustedPlaces;
                }

                set
                {
                    this.adjustedPlaces = value;
                }
            }

            /// <summary>
            /// Updates the calc interval.
            /// </summary>
            /// <internalonly/>
            public void UpdateCalcInterval()
            {
                this.CalcInterval = (this.CalcMax - this.CalcMin) / this.parent.DesiredIntervals;
            }
        }

        #endregion

        #region Members
        private int desiredIntervals;
        private bool preferZero = true;
        private bool forceZero = false;
        private ChartAxisRangePaddingType m_rangePaddingType;
        private double m_padding = 0;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NiceRangeMaker"/> class.
        /// </summary>
        /// <param name="desiredIntervals">The desired intervals.</param>
        public NiceRangeMaker(int desiredIntervals)
        {
            this.desiredIntervals = desiredIntervals;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NiceRangeMaker"/> class.
        /// </summary>
        /// <internalonly/>
        public NiceRangeMaker()
            : this(DESIRED_INTERVALS)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the approximate number of intervals into which the range is to be partitioned. The actual number of
        /// intervals calculated will depend on the actual algorithm used.
        /// </summary>
        /// <value>The Desired Intervals.</value>
        /// <internalonly/>
        public virtual int DesiredIntervals
        {
            get
            {
                return this.desiredIntervals;
            }

            set
            {
                this.desiredIntervals = value;
            }
        }

        /// <summary>
        /// Gets or sets the padding type that will be applied for calculating the ranges for this axis.
        /// </summary>
        /// <value>The RangePaddingType type.</value>
        /// <internalonly/>
        public virtual ChartAxisRangePaddingType RangePaddingType
        {
            get
            {
                return m_rangePaddingType;                    
            }

            set
            {
                m_rangePaddingType = value;
            }
        }

        /// <summary>
        /// Gets or Sets whether one boundary of the calculated range should be tweaked to zero. Such tweaking will happen
        /// only if zero is within a resonable distance from the calculated boundary. To ensure that one boundary
        /// is always zero, use the <see cref="ForceZero"/> setting instead.
        /// </summary>
        /// <value>The PreferZero.</value>
        /// <internalonly/>
        public virtual bool PreferZero
        {
            get
            {
                return this.preferZero;
            }

            set
            {
                this.preferZero = value;
            }
        }

        /// <summary>
        /// Gets or Sets the result whether one boundary of the calculated range should always be tweaked to zero.
        /// </summary>
        /// <value>the ForceZero.</value>
        /// <internalonly/>
        public virtual bool ForceZero
        {
            get
            {
                return this.forceZero;
            }

            set
            {
                this.forceZero = value;
            }
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Given a minimum value and a maximum value, this method will calculate a 'nice' minimum value and a maxiumum value
        /// as well as an interval value that can be used for visually representing this data. 'Nice' values are better
        /// perceived by humans. For example, consider a range 1.21-3.5. A nice range that we can use to visually represent values
        /// in this range can be 0-3.6 with an interval of 2. You can tweak the results obtained by changing optional settings.
        /// </summary>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <param name="rangePaddingType">The range padding type.</param>
        /// <returns>
        /// Calculated <see cref="Syncfusion.Windows.Forms.Chart.MinMaxInfo"/>.
        /// </returns>
        /// <internalonly/>
        public virtual MinMaxInfo MakeNiceRange(double min, double max, ChartAxisRangePaddingType rangePaddingType)
        {
            if (max < min)
            {
                throw new ArgumentOutOfRangeException("Minimum value cannot be greater than or equals maximum value.");
            }

            DoubleRange range = new DoubleRange(min, max);

            if (forceZero)
            {
                range = DoubleRange.Union(range, 0);
            }

            OpState opState = new OpState(this, range.Start, range.End);

            this.CreatePadding(opState, rangePaddingType);
            this.TweakMinMax(opState);
            this.AdjustPlaces(opState);
            this.CalcNiceValues(opState);
            this.UndoAdjustPlaces(opState);
            this.TweakToFitZero(opState);

            if (rangePaddingType == ChartAxisRangePaddingType.Calculate)
            {
                this.TweakBoundaries(opState);
            }

            return new MinMaxInfo(opState.CalcMin, opState.CalcMax, opState.CalcInterval);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Add padding to the incoming minimum and maximum values.
        /// </summary>
        /// <param name="opState">The operational status.</param>
        /// <param name="rangePaddingType">The ChartAxisRangePaddingTyp.</param>
        protected virtual void CreatePadding(OpState opState, ChartAxisRangePaddingType rangePaddingType)
        {
            // some extra padding
            m_padding = 0.0;
            if (rangePaddingType == ChartAxisRangePaddingType.Calculate)
                m_padding = (opState.Max - opState.Min) / (this.desiredIntervals * 2);

            opState.CalcMax = opState.Max + m_padding;
            opState.CalcMin = opState.Min - m_padding;
        }

        /// <summary>
        /// Tweaks the incoming minimum and maximum values so that special conditions are handled
        /// properly.
        /// </summary>
        /// <param name="opState">The operational status.</param>
        protected virtual void TweakMinMax(OpState opState)
        {
            // Both min and max are 0.
            if (opState.CalcMin == 0 && opState.CalcMax == 0)
            {
                opState.CalcMax = 1;
                opState.UpdateCalcInterval();
            }

            // Min and max are equal.
            if (opState.CalcMin == opState.CalcMax)
            {
                if (Math.Sign(opState.Max) == -1)
                {
                    opState.CalcMax = 0;
                }
                else
                {
                    opState.CalcMin = 0;
                }

                opState.UpdateCalcInterval();
            }
        }

        /// <summary>
        /// Raises the working values to powers of 10 such that we work with whole
        /// numbers. The number of places adjusted is stored in the operating state.
        /// </summary>
        /// <param name="opState">The operational status.</param>
        protected virtual void AdjustPlaces(OpState opState)
        {

            // First adjust the interval since this is the number that we
            // will actually work with.
            double interval = opState.CalcInterval;

            int adjustedPlaces = (int)Math.Floor(Math.Log10(interval)) - 1;
            interval /= Math.Pow(10, adjustedPlaces);

            opState.AdjustedPlaces = adjustedPlaces;
            opState.CalcInterval = Math.Ceiling(interval);

            // Adjust the min and max values based on the adjusted interval.
            opState.CalcMin = opState.CalcMin / Math.Pow(10, adjustedPlaces);
            opState.CalcMax = opState.CalcMax / Math.Pow(10, adjustedPlaces);
        }

        /// <summary>
        /// Calculates 'nice' values by calling other methods. 
        /// </summary>
        /// <param name="opState">The operational status.</param>
        protected virtual void CalcNiceValues(OpState opState)
        {
            this.CalcNiceInterval(opState);
            this.CalcNiceMin(opState);
            this.CalcNiceMax(opState);
        }

        /// <summary>
        /// Calculates the nice interval.
        /// </summary>
        /// <param name="opState">The operational status.</param>
        /// <internalonly/>
        protected virtual void CalcNiceInterval(OpState opState)
        {
            // Ensure that we are working with a whole number.
            Trace.Assert(Math.Round(opState.CalcInterval, 0) == opState.CalcInterval);

            string val_str = opState.CalcInterval.ToString("F0");
            int num_digits = val_str.Length;

            Trace.Assert(num_digits >= 2);

            double sigDigits = double.Parse(val_str.Substring(0, 2));

            double niceNumber = this.MakeNiceNumber(sigDigits);
            string strRetval = new String('0', num_digits - 2);
            strRetval = niceNumber.ToString() + strRetval;

            opState.CalcInterval = Double.Parse(strRetval);
        }

        /// <summary>
        /// Simple logic for creating 'nice' numbers that are close to the numbers passed in.
        /// </summary>
        /// <param name="val">
        /// Value whose equivalent 'nice' number is to be found.
        /// </param>
        /// <returns>Returns double.</returns>
        protected virtual double MakeNiceNumber(double val)
        {
            if (val < 10)
            {
                return 10;
            }
            else if (val < 20)
            {
                return 20;
            }
            else if (val < 25)
            {
                return 25;
            }
            else if (val < 50)
            {
                return 50;
            }
            else
            {
                return 100;
            }
            // return 10 * Math.Ceiling(val / 10);
        }

        /// <summary>
        /// Calculates a 'nice' minimum value given a 'nice' interval. This function basically makes the minimum
        /// value divisible by the interval.
        /// </summary>
        /// <param name="opState"></param>
        protected virtual void CalcNiceMin(OpState opState)
        {
            opState.CalcMin = ChartMath.Round(opState.CalcMin, opState.CalcInterval, false);
        }

        /// <summary>
        /// Calculates a 'nice' maximum value given a 'nice' interval. This function basically makes the maximum
        /// value divisible by the interval.
        /// </summary>
        /// <param name="opState"></param>
        protected virtual void CalcNiceMax(OpState opState)
        {
            opState.CalcMax = ChartMath.Round(opState.CalcMax, opState.CalcInterval, true);
        }

        /// <summary>
        /// Divides the calculated values again by the adjustment factor to go back to correct values.
        /// </summary>
        /// <param name="opState"></param>
        protected virtual void UndoAdjustPlaces(OpState opState)
        {           
            opState.CalcMin = opState.CalcMin * Math.Pow(10, opState.AdjustedPlaces);
            opState.CalcMax = opState.CalcMax * Math.Pow(10, opState.AdjustedPlaces);
            opState.CalcInterval = opState.CalcInterval * Math.Pow(10, opState.AdjustedPlaces);
        }

        /// <summary>
        /// Checks the minimum and maximum values calculated to see if either of them can
        /// be made zero. Visual respresentation of data appears more readable if zero is used as a baseline.
        /// </summary>
        /// <param name="opState"></param>
        protected virtual void TweakToFitZero(OpState opState)
        {
            if (this.preferZero == false && this.forceZero == false)
            {
                return;
            }

            int numberOfIntervals = (int)((opState.CalcMax - opState.CalcMin) / (opState.CalcInterval));

            if ((Math.Sign(opState.Max) == -1) || (Math.Sign(opState.Max) == 0))
            {
                if (this.forceZero)
                {
                    opState.CalcMax = 0;
                }
                else if (opState.CalcMax + numberOfIntervals / 2 * opState.CalcInterval >= 0)
                {
                    opState.CalcMax = 0;
                }
            }
            else if ((Math.Sign(opState.Min) == 1) || (Math.Sign(opState.Min) == 0))
            {
                if (this.forceZero)
                {
                    opState.CalcMin = 0;
                }
                else if (opState.CalcMin - numberOfIntervals / 2 * opState.CalcInterval <= 0)
                {
                    opState.CalcMin = 0;
                }
            }
        }

        /// <summary>
        /// Checks the calculated minimum and maximum values to see if they need to be changed
        /// so that a visual representation does not result in values being displayed too close to the boundaries.
        /// </summary>
        /// <param name="opState"></param>
        protected virtual void TweakBoundaries(OpState opState)
        {
            if (opState.CalcMin == opState.Min && (!forceZero || opState.CalcMin != 0))
            {
                opState.CalcMin = opState.CalcMin - opState.CalcInterval;
            }

            if (opState.CalcMax == opState.Max && (!forceZero || opState.CalcMax != 0))
            {
                opState.CalcMax = opState.CalcMax + opState.CalcInterval;
            }
        }

        #endregion // protected
    }
    #endregion
}
