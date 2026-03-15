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

namespace Syncfusion.Windows.Forms.TreeMap
{
    #region SummaryBase class
    /// <summary>
    /// This class is an abstract class that defines the necessary functionality to do tablix calculations.
    /// </summary>
    internal abstract class SummaryBase
    {
        /// <summary>
        /// Use this method to combine a value from an object in the Tablix Grid's data source with the accumulation values held in this instance.
        /// </summary>
        /// <param name="other">The value to be included in the compuation.</param>
        public abstract void Combine(object other);

        /// <summary>
        /// Resets all internal values so the calculations begins anew.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Returns the calculation value.
        /// </summary>
        /// <returns>The calculation value.</returns>
        public abstract object GetResult();

        /// <summary>
        /// Provides a new instance of this SummaryBase object.
        /// </summary>
        /// <returns>New instance of this SummaryBase.</returns>
        public abstract SummaryBase GetInstance();

        /// <summary>
        /// Use this method to combine another SummaryBase object with the accumulation values held in this instance.
        /// </summary>
        /// <param name="other">The other SummaryBase object.</param>
        public abstract void CombineSummary(SummaryBase other);
    }

    #endregion

    #region CountSummary

    internal class CountSummary : SummaryBase
    {
        internal int count = 0;

        public override string ToString()
        {
            return "Count";
        }

        public override void Combine(object other)
        {
            count++;
        }

        public override void CombineSummary(SummaryBase other)
        {
            count += ((CountSummary)other).count;
        }

        public override void Reset()
        {
            count = 0;
        }

        public override object GetResult()
        {
            return count;
        }
        public override SummaryBase GetInstance()
        {
            return new CountSummary();
        }
    }

    #endregion

    #region CountDistinctSummary

    internal class CountDistinctSummary : SummaryBase
    {
        internal object count = null;
        internal List<object> list = new List<object>();


        public override string ToString()
        {
            return "CountDistinct";
        }

        public override void Combine(object other)
        {
            list.Add(other);
        }

        public override void CombineSummary(SummaryBase other)
        {
            list.AddRange(((CountDistinctSummary)other).list);
        }

        public override void Reset()
        {
            list.Clear();
        }

        public override object GetResult()
        {
            return list.Distinct().Count();
        }
        public override SummaryBase GetInstance()
        {
            return new CountDistinctSummary();
        }

    }

    #endregion

    #region double summaries

    #region DoubleMinSummary
    internal class DoubleMinSummary : SummaryBase
    {
        public override string ToString()
        {
            return "DoubleMinimum";
        }

        internal double min = double.MaxValue;

        public override void Combine(object other)
        {
            double d = double.MaxValue;
            if (other is double)
            {
                d = (double)other;
            }
            else if (other is int)
            {
#if SILVERLIGHT
                d = (double)Convert.ChangeType(other, typeof(double), null);
#else
                d = (double)Convert.ChangeType(other, typeof(double));
#endif
            }

            if (min > d)
                min = d;

        }

        public override void CombineSummary(SummaryBase other)
        {
            var asum = (DoubleMinSummary)other;
            if (min > asum.min)
                min = asum.min;
        }

        public override void Reset()
        {
            min = double.MaxValue;
        }

        public override object GetResult()
        {
            return min;
        }
        public override SummaryBase GetInstance()
        {
            return new DoubleMinSummary();
        }
    }

    #endregion

    #region DoubleMaxSummary

    internal class DoubleMaxSummary : SummaryBase
    {
        public override string ToString()
        {
            return "DoubleMaximum";
        }

        internal double max = double.MinValue;

        public override void Combine(object other)
        {
            double d = 0d;
            if (other is double)
            {
                d = (double)other;
            }
            else if (other is int)
            {
#if SILVERLIGHT
                d = (double)Convert.ChangeType(other, typeof(double), null);
#else
                d = (double)Convert.ChangeType(other, typeof(double));
#endif
            }

            if (max < d)
                max = d;

        }

        public override void CombineSummary(SummaryBase other)
        {
            var asum = (DoubleMaxSummary)other;
            if (max < asum.max)
                max = asum.max;
        }

        public override void Reset()
        {
            max = double.MinValue;
        }

        public override object GetResult()
        {
            return max;
        }
        public override SummaryBase GetInstance()
        {
            return new DoubleMaxSummary();
        }
    }

    #endregion

    #region DoubleStDevSummary

    //formula used is for sample standard deviation (as per excel)
    // sqrt(sum((x-xbar)*(x-xbar)) / (n-1) )
    // or the equivalent form is used below...
    // sqrt(sum(x*x) - n * xbar * xbar) / (n-1))

    internal class DoubleStDevSummary : SummaryBase
    {
        public override string ToString()
        {
            return "DoubleStandardDeviation";
        }

        internal double sumX2 = 0d;
        internal double sumX = 0d;
        internal int n = 0;

        public override void Combine(object other)
        {
            if (other != null)
            {
                double d = Convert.ToDouble(other.ToString());
                sumX2 += d * d;
                sumX += d;
            }

            n++;
        }

        public override void CombineSummary(SummaryBase other)
        {
            var asum = (DoubleStDevSummary)other;
            sumX2 += asum.sumX2;
            sumX += asum.sumX;
            n += asum.n;
        }

        public override void Reset()
        {
            sumX2 = 0d;
            sumX = 0;
            n = 0;
        }

        public override object GetResult()
        {
            if (n < 2)
                return double.NaN;
            // return sumX / n;
            return Math.Sqrt((sumX2 - sumX * sumX / n) / (n - 1));
        }
        public override SummaryBase GetInstance()
        {
            return new DoubleStDevSummary();
        }
    }

    #endregion

    #region DoubleStDevPSummary

    internal class DoubleStDevPSummary : SummaryBase
    {
        public override string ToString()
        {
            return "DoubleStandardDeviationPopulation";
        }

        internal double sumX2 = 0d;
        internal double sumX = 0d;
        internal int n = 0;

        public override void Combine(object other)
        {
            if (other != null)
            {
                double d = Convert.ToDouble(other.ToString());
                sumX2 += d * d;
                sumX += d;
            }

            n++;
        }

        public override void CombineSummary(SummaryBase other)
        {
            var asum = (DoubleStDevPSummary)other;
            sumX2 += asum.sumX2;
            sumX += asum.sumX;
            n += asum.n;
        }

        public override void Reset()
        {
            sumX2 = 0d;
            sumX = 0;
            n = 0;
        }

        public override object GetResult()
        {
            if (n < 2)
                return 0;
            // return sumX / n;
            return Math.Sqrt((sumX2 - sumX * sumX / n) / (n));
        }
        public override SummaryBase GetInstance()
        {
            return new DoubleStDevPSummary();
        }
    }
    #endregion

    #region DoubleVarianceSummary
    internal class DoubleVarianceSummary : SummaryBase
    {
        public override string ToString()
        {
            return "DoubleVariance";
        }

        internal double sumX2 = 0d;
        internal double sumX = 0d;
        internal int n = 0;

        public override void Combine(object other)
        {
            double d = 0d;
            if (other is double)
            {
                d = (double)other;
                n++;
            }
            else if (other is int)
            {
#if SILVERLIGHT
                d = (double)Convert.ChangeType(other, typeof(double), null);
#else
                d = (double)Convert.ChangeType(other, typeof(double));
#endif
                n++;
            }

            sumX2 += d * d;
            sumX += d;

        }

        public override void CombineSummary(SummaryBase other)
        {
            var asum = (DoubleVarianceSummary)other;
            sumX2 += asum.sumX2;
            sumX += asum.sumX;
            n += asum.n;
        }

        public override void Reset()
        {
            sumX2 = 0d;
            sumX = 0;
            n = 0;
        }

        public override object GetResult()
        {
            if (n < 2)
                return double.NaN;
            // return sumX / n;
            return (sumX2 - sumX * sumX / n) / (n - 1);
        }
        public override SummaryBase GetInstance()
        {
            return new DoubleVarianceSummary();
        }
    }

    #endregion

    #region DoubleVariancePSummary

    internal class DoubleVariancePSummary : SummaryBase
    {
        public override string ToString()
        {
            return "DoubleVariance";
        }

        internal double sumX2 = 0d;
        internal double sumX = 0d;
        internal int n = 0;

        public override void Combine(object other)
        {
            if (other != null)
            {
                double d = Convert.ToDouble(other.ToString());
                sumX2 += d * d;
                sumX += d;
            }
            n++;

        }

        public override void CombineSummary(SummaryBase other)
        {
            var asum = (DoubleVariancePSummary)other;
            sumX2 += asum.sumX2;
            sumX += asum.sumX;
            n += asum.n;
        }

        public override void Reset()
        {
            sumX2 = 0d;
            sumX = 0;
            n = 0;
        }

        public override object GetResult()
        {
            if (n < 2)
                return 0;
            // return sumX / n;

            return (sumX2 - sumX * sumX / n) / (n);
        }
        public override SummaryBase GetInstance()
        {
            return new DoubleVariancePSummary();
        }
    }
    #endregion

    #region DoubleAverageSummary

    internal class DoubleAverageSummary : SummaryBase
    {
        public override string ToString()
        {
            return "DoubleAverage";
        }

        internal double total = 0d;
        internal int count = 0;

        public override void Combine(object other)
        {
            if (other is double)
            {
                total += (double)other;
                count++;
            }
            else if (other is int)
            {
#if SILVERLIGHT
                total += (double)Convert.ChangeType(other, typeof(double), null);
#else
                total += (double)Convert.ChangeType(other, typeof(double));
#endif
                count++;
            }
        }

        public override void CombineSummary(SummaryBase other)
        {
            var asum = (DoubleAverageSummary)other;
            total += asum.total;
            count += asum.count;
        }

        public override void Reset()
        {
            total = 0d;
            count = 0;
        }

        public override object GetResult()
        {
            if (count == 0)
                return double.NaN;
            return total / count;
        }
        public override SummaryBase GetInstance()
        {
            return new DoubleAverageSummary();
        }
    }

    #endregion

    #region DoubleTotalSummary

    internal class DoubleTotalSummary : SummaryBase
    {
        public override string ToString()
        {
            return "DoubleTotal";
        }

        internal double total = double.NaN;

        public override void Combine(object other)
        {
            if (other is double)
            {
                if (double.IsNaN(total))
                {
                    total = 0;
                }

                total += (double)other;
            }
            else if (other is int || other is decimal)
            {
                if (double.IsNaN(total))
                {
                    total = 0;
                }

#if SILVERLIGHT
                total += (double)Convert.ChangeType(other, typeof(double), null);
#else
                total += (double)Convert.ChangeType(other, typeof(double));
#endif
            }
            if (other is string && !string.IsNullOrEmpty((string)other))// && !string.IsNullOrWhiteSpace((string)other))
            {
                if (double.IsNaN(total))
                {
                    total = 0;
                }

                double value;
                if (double.TryParse(other.ToString(), out value))
                {
                    total += value;
                }
            }
        }

        public override void CombineSummary(SummaryBase other)
        {
            Combine(((DoubleTotalSummary)other).total);
        }

        public override void Reset()
        {
            total = 0d;
        }

        public override object GetResult()
        {
            return total;
        }
        public override SummaryBase GetInstance()
        {
            return new DoubleTotalSummary();
        }
    }

    #endregion

    #endregion

    #region IntTotalSummary

    internal class IntTotalSummary : SummaryBase
    {
        public override string ToString()
        {
            return "IntTotal";
        }

        internal int total = 0;

        public override void Combine(object other)
        {
            if (other is int)
            {
                total += (int)other;
            }
            else if (other is double)
            {
                total += Convert.ToInt32(other);
            }
        }

        public override void CombineSummary(SummaryBase other)
        {
            Combine(((IntTotalSummary)other).total);
        }

        public override void Reset()
        {
            total = 0;
        }

        public override object GetResult()
        {
            return total;
        }
        public override SummaryBase GetInstance()
        {
            return new IntTotalSummary();
        }
    }

    #endregion

    #region decimal summaries

    #region DecimalTotalSummary

    internal class DecimalTotalSummary : SummaryBase
    {
        public override string ToString()
        {
            return "DecimalTotal";
        }

        internal decimal total = 0m;

        public override void Combine(object other)
        {
            if (other is decimal)
            {
                total += (decimal)other;
            }
            else if (other is double || other is int)
            {
#if SILVERLIGHT
                total += (decimal)Convert.ChangeType(other, typeof(decimal), null);
#else
                total += (decimal)Convert.ChangeType(other, typeof(decimal));
#endif
            }
        }

        public override void CombineSummary(SummaryBase other)
        {
            Combine(((DecimalTotalSummary)other).total);
        }

        public override void Reset()
        {
            total = 0m;
        }

        public override object GetResult()
        {
            return total;
        }
        public override SummaryBase GetInstance()
        {
            return new DecimalTotalSummary();
        }
    }
    #endregion

    #endregion

    #region TextSummary - for debug purposes

    internal class TextSummary : SummaryBase
    {
        internal string text = "";
        internal List<string> values = new List<string>();

        public override void Combine(object other)
        {
            if (other is string)
            {
                values.Add(other as string);
            }
            else if (other != null)
            {
                values.Add(other.ToString());
            }
        }
        public override void CombineSummary(SummaryBase other)
        {
            values.Add(other.GetResult() as string);
        }

        public override void Reset()
        {
            values.Clear();
        }

        public override object GetResult()
        {
            if (values.Count > 0)
            {
                return values[0];
            }
            return string.Empty;
        }
        public override SummaryBase GetInstance()
        {
            return new TextSummary();
        }
    }
    #endregion

    #region DataSummary

    internal class DataSummary : SummaryBase
    {
        object value;

        public override string ToString()
        {
            return "DataSummary";
        }

        public override void Combine(object other)
        {
            value = other;
        }

        public override void CombineSummary(SummaryBase other)
        {
            Combine(other.GetResult());
        }

        public override void Reset()
        {
            value = null;
        }

        public override object GetResult()
        {
            return value;
        }

        public override SummaryBase GetInstance()
        {
            return new DataSummary();
        }
    }

    #endregion

    #region FirstSummary

    internal class FirstSummary : SummaryBase
    {
        internal object count = null;
        internal List<object> list = new List<object>();

        public override string ToString()
        {
            return "First";
        }

        public override void Combine(object other)
        {
            list.Add(other);
        }

        public override void CombineSummary(SummaryBase other)
        {
            list.AddRange(((FirstSummary)other).list);
        }

        public override void Reset()
        {
            list.Clear();
        }

        public override object GetResult()
        {
            return list.First();
        }
        public override SummaryBase GetInstance()
        {
            return new FirstSummary();
        }

    }

    #endregion

    #region LastSummary

    internal class LastSummary : SummaryBase
    {
        internal object count = null;
        internal List<object> list = new List<object>();

        public override string ToString()
        {
            return "Last";
        }

        public override void Combine(object other)
        {
            list.Add(other);
        }

        public override void CombineSummary(SummaryBase other)
        {
            list.AddRange(((LastSummary)other).list);
        }

        public override void Reset()
        {
            list.Clear();
        }

        public override object GetResult()
        {
            return list.Last();
        }
        public override SummaryBase GetInstance()
        {
            return new LastSummary();
        }

    }

    #endregion
}
