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
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// 
    /// </summary>
    class DoubleArrayConverter : TypeConverter
    {
        #region Constants
        private const char c_splitter = ',';
        #endregion

        #region Public methods
        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type"></see> that represents the type you want to convert to.</param>
        /// <returns>
        /// True if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"></see> that represents the type you want to convert from.</param>
        /// <returns>
        /// True if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"></see> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string[] values = (value as string).Split(c_splitter);
                double[] result = new double[values.Length];

                for (int i = 0; i < values.Length; i++)
                {
                    result[i] = double.Parse(values[i].Trim(), CultureInfo.InvariantCulture);
                }

                return result;
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"></see>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"></see> to convert the value parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        /// <exception cref="T:System.ArgumentNullException">The destinationType parameter is null. </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                StringBuilder sBuilder = new StringBuilder();
                double[] values = value as double[];

                for (int i = 0; i < values.Length; i++)
                {
                    if (i != 0)
                    {
                        sBuilder.Append(c_splitter);
                    }

                    sBuilder.Append(values[i].ToString(CultureInfo.InvariantCulture));
                }

                return sBuilder.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
        #endregion
    }
   
    internal interface IChartPointCore
    {
        /// <summary>
        /// Gets or sets the X value.
        /// </summary>
        /// <value>The X.</value>
        double X { get; set; }

        /// <summary>
        /// Gets or sets the Y value.
        /// </summary>
        /// <value>The Y value.</value>
        double[] Y { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        bool IsEmpty { get; set; }
    }

    internal class ChartPointDataCore : IChartPointCore
    {
        #region Members
        private IChartSeriesModel ds;
        private int xIndex;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPointDataCore"/> class.
        /// </summary>
        /// <param name="ds">The ds.</param>
        /// <param name="xIndex">Index of the x.</param>
        public ChartPointDataCore(IChartSeriesModel ds, int xIndex)
        {
            this.ds = ds;
            this.xIndex = xIndex;
        }
        #endregion

        #region Proeprties
        /// <summary>
        /// Gets or sets the X.
        /// </summary>
        /// <value>The X.</value>
        public double X
        {
            get
            {
                return this.ds.GetX(this.xIndex);
            }

            set
            {
                if (this.IsEditableData())
                {
                    this.GetEditableData().SetX(this.xIndex, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Y.
        /// </summary>
        /// <value>The Y.</value>
        public double[] Y
        {
            get
            {
                return this.ds.GetY(this.xIndex);
            }

            set
            {
                if (this.IsEditableData())
                {
                    this.GetEditableData().SetY(this.xIndex, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get
            {
                return this.ds.GetEmpty(this.xIndex);
            }

            set
            {
                if (this.IsEditableData())
                {
                    this.GetEditableData().SetEmpty(this.xIndex, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Y dates.
        /// </summary>
        /// <value>The Y dates.</value>
        public DateTime[] YDates
        {
            get
            {
                double[] y = this.Y;
                DateTime[] dates = new DateTime[y.GetLength(0)];

                for (int i = 0; i < y.GetLength(0); i++)
                {
                    dates[i] = DateTime.FromOADate(y[i]);
                }

                return dates;
            }

            set
            {
                DateTime[] dates = value;

                double[] y = new double[dates.GetLength(0)];

                for (int i = 0; i < dates.GetLength(0); i++)
                {
                    y[i] = dates[i].ToOADate();
                }

                this.Y = y;
            }
        }

        /// <summary>
        /// Determines whether data is editable.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if data is editable; otherwise, <c>false</c>.
        /// </returns>
        private bool IsEditableData()
        {
           return (this.ds != null && this.ds is IEditableChartSeriesModel);
        }

        /// <summary>
        /// Gets the editable data.
        /// </summary>
        /// <returns> Returns IEditableChartSeriesModel.</returns>
        private IEditableChartSeriesModel GetEditableData()
        {
            if (this.IsEditableData())
            {
                return this.ds as IEditableChartSeriesModel;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    internal class ChartPointContainedCore : IChartPointCore
    {
        #region Members
        private double x;
        private double[] y;
        private bool isEmpty = false;
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public ChartPointContainedCore(double x, double[] y)
        {
            this.x = x;
            this.y = y;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the X value.
        /// </summary>
        /// <value>The X value.</value>
        public double X
        {
            get
            {
                return this.x;
            }

            set
            {
                this.x = value;
            }
        }

        /// <summary>
        /// Gets the Y value.
        /// </summary>
        /// <param name="yIndex">Index of the y value.</param>
        /// <returns></returns>
        public double GetY(int yIndex)
        {
            return this.y[yIndex];
        }

        /// <summary>
        /// Gets or sets the Y value.
        /// </summary>
        /// <value>The Y value.</value>
        public double[] Y
        {
            get
            {
                return this.y;
            }

            set
            {
                this.y = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get
            {
                return this.isEmpty;
            }

            set
            {
                this.isEmpty = value;
            }

        }
        #endregion
    }

    /// <summary>
    /// This class provides an easy interface to interact with the underlying data points contained in the <see cref="IChartSeriesModel"/> associated
    /// with the <see cref="ChartSeries"/> that contains this data. Even though you are interacting with a friendly object model, the ChartPoint
    /// itself stores no data. It simply delegates to the underlying model that the ChartSeries is displaying.
    /// </summary>
    public class ChartPoint
    {
        #region Constants
        /// <summary>
        ///  Signifies the empty point.
        /// </summary>
        public readonly static ChartPoint Empty;
        #endregion

        #region Members
        private IChartPointCore m_chartPointCore;
        #endregion

        #region Propertites
        /// <summary>
        /// Returns Y values associated with this point as DateTime values.
        /// </summary>
        public DateTime[] GetYValuesAsDateTime()
        {
            double[] yValues = this.YValues;
            DateTime[] dates = new DateTime[yValues.GetLength(0)];

            for (int i = 0; i < yValues.GetLength(0); i++)
            {
                dates[i] = DateTime.FromOADate(yValues[i]);
            }

            return dates;
        }

        /// <summary>
        /// Gets or sets the X value associated with this point as a DateTime value.
        /// </summary>
        /// <value>The date X.</value>
        [ChartTemplate(ChartTemplateSet.Simple)]
        public DateTime DateX
        {
            get
            {
                return DateTime.FromOADate(m_chartPointCore.X);
            }

            set
            {
                m_chartPointCore.X = value.ToOADate();
            }
        }

        /// <summary>
        /// Gets or sets the X value associated with this point.
        /// </summary>
        /// <value>The X.</value>
        [ChartTemplate(ChartTemplateSet.Simple)]
        public double X
        {
            get
            {
                return m_chartPointCore.X;
            }

            set
            {
                m_chartPointCore.X = value;
            }
        }

        /// <summary>
        /// Gets or sets the Y values associated with this point.
        /// </summary>
        /// <value>The Y values.</value>
        [ChartTemplate(ChartTemplateSet.Simple)]
        [TypeConverter(typeof(DoubleArrayConverter))]
        public double[] YValues
        {
            get
            {
                return m_chartPointCore.Y;
            }

            set
            {
                m_chartPointCore.Y = value;
            }
        }

        /// <summary>
        /// Indicates whether this point should be plotted.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        [ChartTemplate(ChartTemplateSet.Simple)]
        public bool IsEmpty
        {
            get
            {
                return m_chartPointCore.IsEmpty;
            }

            set
            {
                m_chartPointCore.IsEmpty = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPoint"/> class.
        /// </summary>
        public ChartPoint()
        {
            m_chartPointCore = new ChartPointContainedCore(0, new double[] { 0 });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPoint"/> class.
        /// </summary>
        /// <param name="x" type="double">
        ///     <para>
        ///     X value of this ChartPoint.
        ///     </para>
        /// </param>
        /// <param name="y" type="double[]">
        ///     <para>
        ///     Y values pertaining to this ChartPoint. More than one Y value can be associated with a ChartPoint.
        ///     </para>
        /// </param>
        public ChartPoint(double x, double[] y)
        {
            m_chartPointCore = new ChartPointContainedCore(x, y);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPoint"/> class. Used when working with ChartPoints that have
        /// only one Y value.
        /// </summary>
        /// <param name="x" type="double">
        ///     <para>
        ///     X value of this ChartPoint.
        ///     </para>
        /// </param>
        /// <param name="y" type="double">
        ///     <para>
        ///     Y value of this ChartPoint.
        ///     </para>
        /// </param>
        public ChartPoint(double x, double y)
        {
            m_chartPointCore = new ChartPointContainedCore(x, new double[] { y });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPoint"/> class. Used when working with ChartPoints that have
        /// DateTime Y values.
        /// </summary>
        /// <param name="x" type="double">
        ///     <para>
        ///     X value of this ChartPoint.
        ///     </para>
        /// </param>
        /// <param name="dates" type="DateTime[]">
        ///     <para>
        ///     DateTime Y values of this ChartPoint.
        ///     </para>
        /// </param>
        public ChartPoint(double x, DateTime[] dates)
        {
            double[] yValues = new double[dates.GetLength(0)];

            for (int i = 0; i < yValues.GetLength(0); i++)
            {
                yValues[i] = dates[i].ToOADate();
            }

            m_chartPointCore = new ChartPointContainedCore(x, yValues);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPoint"/> class. Used when working with ChartPoints that have a single
        /// associated DateTime Y value.
        /// </summary>
        /// <param name="x" type="double">
        ///     <para>
        ///     X value of this ChartPoint.
        ///     </para>
        /// </param>
        /// <param name="date" type="DateTime">
        ///     <para>
        ///     DateTime Y value pertaining to this ChartPoint.
        ///     </para>
        /// </param>
        public ChartPoint(double x, DateTime date)
        {
            m_chartPointCore = new ChartPointContainedCore(x, new double[] { date.ToOADate() });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPoint"/> class. Used when working with ChartPoints that have
        /// a DateTime X value.
        /// </summary>
        /// <param name="date" type="DateTime">
        ///     <para>
        ///     DateTime X value of this ChartPoint.
        ///     </para>
        /// </param>
        /// <param name="yValues" type="double[]">
        ///     <para>
        ///     Y values of this ChartPoint.
        ///     </para>
        /// </param>
        public ChartPoint(DateTime date, double[] yValues)
        {
            m_chartPointCore = new ChartPointContainedCore(date.ToOADate(), yValues);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPoint"/> class. Used when working with ChartPoints that have
        /// a DateTime X value.
        /// </summary>
        /// <param name="date" type="DateTime">
        ///     <para>
        ///     DateTime X value of this ChartPoint.
        ///     </para>
        /// </param>
        /// <param name="y" type="double">
        ///     <para>
        ///     Y value of this ChartPoint.
        ///     </para>
        /// </param>
        public ChartPoint(DateTime date, double y)
        {
            m_chartPointCore = new ChartPointContainedCore(date.ToOADate(), new double[] { y });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPoint"/> class.
        /// </summary>
        /// <param name="ds">The IChartSeriesModel argument.</param>
        /// <param name="xIndex">Index of the x.</param>
        internal ChartPoint(IChartSeriesModel ds, int xIndex)
        {
            m_chartPointCore = new ChartPointDataCore(ds, xIndex);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            StringBuilder strBiulder = new StringBuilder();

            strBiulder.Append("X : ");
            strBiulder.Append(this.X);

            strBiulder.Append("; Y : ");

            for (int i = 0, ci = this.YValues.Length - 1; i <= ci; i++)
            {
                strBiulder.Append(this.YValues[i]);

                if (i != ci)
                {
                    strBiulder.Append(", ");
                }
            }

            return strBiulder.ToString();
        }
        #endregion
    }

    /// <summary>
    /// Contains chart point and index of point.
    /// </summary>
    public class ChartPointWithIndex
    {
        #region Members
        private ChartPoint m_point;
        private int m_index;
        #endregion

        #region Proeprties
        /// <summary>
        /// Specifies the ChartPoint.
        /// </summary>
        public ChartPoint Point
        {
            get
            {
                return m_point;
            }

            set
            {
                if (m_point != value)
                {
                    m_point = value;
                }
            }
        }

        /// <summary>
        /// Specifies the index of the ChartPoint
        /// </summary>
        public int Index
        {
            get
            {
                return m_index;
            }

            set
            {
                if (m_index != value)
                {
                    m_index = value;
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPointWithIndex"/> class.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="index">The index of point.</param>
        public ChartPointWithIndex(ChartPoint point, int index)
        {
            m_point = point;
            m_index = index;
        }
        #endregion
    }

    /// <summary>
    /// Provides the method to compare the <see cref="ChartPointWithIndex"/> by the X value.
    /// </summary>
    public class ComparerPointWithIndexByX : IComparer
    {
        #region Implementation
        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value Condition Less than zero x is less than y. Zero x equals y. Greater than zero x is greater than y.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">Neither x nor y implements the <see cref="T:System.IComparable"></see> interface.-or- x and y are of different types and neither one can handle comparisons with the other. </exception>
        int System.Collections.IComparer.Compare(object x, object y)
        {
            ChartPoint p1 = (x as ChartPointWithIndex).Point;
            ChartPoint p2 = (y as ChartPointWithIndex).Point;

            if (p1.X < p2.X)
            {
                return -1;
            }

            if (p1.X > p2.X)
            {
                return 1;
            }

            return 0;
        }
        #endregion
    }

    /// <summary>
    /// Provides the method to compare the <see cref="ChartPointWithIndex"/> by the first Y value.
    /// </summary>
    public class ComparerPointWithIndexByY : IComparer
    {
        #region IComparer Members
        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value Condition Less than zero x is less than y. Zero x equals y. Greater than zero x is greater than y.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">Neither x nor y implements the <see cref="T:System.IComparable"></see> interface.-or- x and y are of different types and neither one can handle comparisons with the other. </exception>
        int System.Collections.IComparer.Compare(object x, object y)
        {
            ChartPointWithIndex pwi1 = (ChartPointWithIndex)x;
            ChartPointWithIndex pwi2 = (ChartPointWithIndex)y;

            ChartPoint p1 = (ChartPoint)pwi1.Point;
            ChartPoint p2 = (ChartPoint)pwi2.Point;

            if (p1.YValues[0] > p2.YValues[0])
            {
                return 1;
            }
            else if (p1.YValues[0] < p2.YValues[0])
            {
                return -1;
            }

            return 0;
        }
        #endregion
    }
}