//-------------------------------------------------------------------------------------------------
// <copyright file="Summaries.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Text;

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Diagnostics;
using Syncfusion.Grouping.Internals;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// Implements the ITreeTableSummary interface and should be used as base class for
    /// custom summary objects.
    /// </summary>
    public abstract class SummaryBase : ITreeTableSummary
    {
        #region ITreeTableSummary Members
        ITreeTableSummary ITreeTableSummary.Combine(ITreeTableSummary other)
        {
            return Combine((SummaryBase)other);
        }
        #endregion

        /// <overload>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </overload>
        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object (of the same type).</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        public abstract SummaryBase Combine(SummaryBase other);

        /// <summary>
        /// Results of ToString method.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Info
        {
            get
            {
                return ToString();
            }
        }
    }
    
    /// <summary>
    /// Counts non-null entries. If SummaryDescriptor.Name is an empty string, records are counted.
    /// </summary>
    public sealed class CountSummary : SummaryBase, ICountAggregate
    {
        int valueCount;

        /// <summary>
        /// The initial summary object for empty records or tables.
        /// </summary>
        public static readonly CountSummary Empty = new CountSummary(0);

        /// <summary>
        /// Creates a summary object for the specified SummaryDescriptor and Record.
        /// </summary>
        /// <param name="sd">The summary descriptor.</param>
        /// <param name="record">The record with data.</param>
        /// <returns>A new summary object.</returns>
        public static ITreeTableSummary CreateSummaryMethod(SummaryDescriptor sd, Record record)
        {
            object obj = sd.GetValue(record);
            bool isNull = obj == null || obj is DBNull;
            if (isNull)
            {
                return Empty;
            }
            else
            {
                return new CountSummary(1);
            }
        }

        public static ITreeTableSummary CreatePageSummaryMethod(SummaryDescriptor sd, Record record)
        {
            object obj = sd.GetValue(record);
            bool isNull = obj == null || obj is DBNull;
            if (isNull)
            {
                return Empty;
            }
            else
            {
                //GridTable tab = record.ParentTable as GridTable;

                if ((record.ParentTable.Records.IndexOf(record) >= record.ParentTable.StartIndex) && (record.ParentTable.Records.IndexOf(record) <= record.ParentTable.EndIndex))
                {
                    return new CountSummary(1);
                }
                else
                {
                    return Empty;
                }
            }
        }

        /// <summary>
        /// Initializes a new summary object with the specified count.
        /// </summary>
        /// <param name="valueCount">The count value.</param>
        public CountSummary(int valueCount)
        {
            this.valueCount = valueCount;
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get
            {
                return valueCount;
            }
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object.</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        /// <override/>
        public override SummaryBase Combine(SummaryBase other)
        {
            return Combine((CountSummary)other);
        }
        
        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object (of the same type).</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        public CountSummary Combine(CountSummary other)
        {
            if (other.Count == 0)
            {
                return this;
            }
            else if (Count == 0)
            {
                return other;
            }
            else
            {
                return new CountSummary(this.Count + other.Count);
            }
        }

        /// <summary>Returns string representation of the summary object.</summary>
        /// <returns>A string holding the summary value.</returns>
        /// <override/>
        public override string ToString()
        {
            return "Count = " + this.Count.ToString() + " ";
        }
    }

    /// <summary>
    /// Summarizes string fields and lets you determine the maximum length of a string in the column.
    /// </summary>
    public sealed class MaxLengthSummary : SummaryBase
    {
        int maxLength;

        /// <summary>
        /// The initial summary object for empty records or tables.
        /// </summary>
        public static readonly MaxLengthSummary Empty = new MaxLengthSummary(0);

        /// <summary>
        /// Creates a summary object for the specified SummaryDescriptor and Record.
        /// </summary>
        /// <param name="sd">The summary descriptor.</param>
        /// <param name="record">The record with data.</param>
        /// <returns>A new summary object.</returns>
        public static ITreeTableSummary CreateSummaryMethod(SummaryDescriptor sd, Record record)
        {
            object obj = sd.GetValue(record);
            bool isNull = obj == null || obj is DBNull;
            if (isNull)
            {
                return Empty;
            }
            else
            {
                string text = Convert.ToString(obj);
                return new MaxLengthSummary(text.Length);
            }
        }



        public static ITreeTableSummary CreatePageSummaryMethod(SummaryDescriptor sd, Record record)
        {
            object obj = sd.GetValue(record);
            bool isNull = obj == null || obj is DBNull;
            if (isNull)
            {
                return Empty;
            }
            else
            {


                if ((record.ParentTable.Records.IndexOf(record) >= record.ParentTable.StartIndex) && (record.ParentTable.Records.IndexOf(record) <= record.ParentTable.EndIndex))
                {
                    string text = Convert.ToString(obj);
                    return new MaxLengthSummary(text.Length);
                }
                else
                {
                    return Empty;
                }
            }
        }
        /// <summary>
        /// Initializes a new summary object with the specified length.
        /// </summary>
        /// <param name="maxLength">The length value.</param>
        public MaxLengthSummary(int maxLength)
        {
            this.maxLength = maxLength;
        }

        /// <summary>
        /// Gets the maximum length.
        /// </summary>
        public int MaxLength
        {
            get
            {
                return maxLength;
            }
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object.</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        /// <override/>
        public override SummaryBase Combine(SummaryBase other)
        {
            return Combine((MaxLengthSummary)other);
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object (of the same type).</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        public MaxLengthSummary Combine(MaxLengthSummary other)
        {
            if (other.maxLength >= this.maxLength)
            {
                return other;
            }
            else
            {
                return this;
            }
        }

        /// <summary>
        /// Returns string representation of the summary object.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <override/>
        public override string ToString()
        {
            return "MaxLength = " + this.maxLength.ToString();
        }
    }

    /// <summary>
    /// Provides a <see cref="Count"/> getter.
    /// </summary>
    public interface ICountAggregate
    {
        /// <summary>
        /// Gets the count.
        /// </summary>
        int Count { get; }
    }

    /// <summary>
    /// Provides an <see cref="Average"/> getter.
    /// </summary>
    public interface IAverageAggregate
    {
        /// <summary>
        /// Gets the average.
        /// </summary>
        double Average { get; }
    }

    /// <summary>
    /// Summarizes integer fields. Provides Count, Minimum, Maximum, Sum, and Average.
    /// </summary>
    public sealed class Int32AggregateSummary : SummaryBase, ICountAggregate, IAverageAggregate
    {
        int count;
        int minimum;
        int maximum;
        int sum;

        /// <summary>
        /// The initial summary object for empty records or tables.
        /// </summary>
        public static readonly Int32AggregateSummary Empty = new Int32AggregateSummary(0, 0, 0, 0);

        /// <summary>
        /// Creates a summary object for the specified SummaryDescriptor and Record.
        /// </summary>
        /// <param name="sd">The summary descriptor.</param>
        /// <param name="record">The record with data.</param>
        /// <returns>A new summary object.</returns>
        public static ITreeTableSummary CreateSummaryMethod(SummaryDescriptor sd, Record record)
        {
            object obj = sd.GetValue(record);
            bool isNull = obj == null || obj is DBNull || Object.ReferenceEquals(obj, string.Empty);
            if (isNull)
            {
                return Empty;
            }
            else
            {
                int val = Convert.ToInt32(obj);
                return new Int32AggregateSummary(1, val, val, val);
            }
        }



        public static ITreeTableSummary CreatePageSummaryMethod(SummaryDescriptor sd, Record record)
        {
            object obj = sd.GetValue(record);
            bool isNull = obj == null || obj is DBNull || Object.ReferenceEquals(obj, string.Empty);
            if (isNull)
            {
                return Empty;
            }
            else
            {
                if ((record.ParentTable.Records.IndexOf(record) >= record.ParentTable.StartIndex) && (record.ParentTable.Records.IndexOf(record) <= record.ParentTable.EndIndex))
                {
                    int val = Convert.ToInt32(obj);
                    return new Int32AggregateSummary(1, val, val, val);
                }
                else
                {
                    return Empty;
                }
               
            }
        }



        /// <summary>
        /// Initializes a new summary object with the specified values.
        /// </summary>
        /// <param name="count">The Count value.</param>
        /// <param name="minimum">Minimum value.</param>
        /// <param name="maximum">Maximum value.</param>
        /// <param name="sum">The Sum value.</param>
        public Int32AggregateSummary(int count, int minimum, int maximum, int sum)
        {
            this.count = count;
            this.minimum = minimum;
            this.maximum = maximum;
            this.sum = sum;
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get
            {
                return count;
            }
        }

        /// <summary>
        /// Gets the minimum.
        /// </summary>
        public int Minimum
        {
            get
            {
                return minimum;
            }
        }

        /// <summary>
        /// Gets the maximum.
        /// </summary>
        public int Maximum
        {
            get
            {
                return maximum;
            }
        }

        /// <summary>
        /// Gets the sum.
        /// </summary>
        public int Sum
        {
            get
            {
                return sum;
            }
        }
        
        /// <summary>
        /// Gets the average.
        /// </summary>
        public double Average
        {
            get
            {
                if (Count == 0)
                {
                    return 0;
                }

                return (double)sum / (double)count;
            }
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object.</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        /// <override/>
        public override SummaryBase Combine(SummaryBase other)
        {
            return Combine((Int32AggregateSummary)other);
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object (of the same type).</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        public Int32AggregateSummary Combine(Int32AggregateSummary other)
        {
            if (other.count == 0)
            {  
                return this;
            }
            else if (this.count == 0)
            {
                return other;
            }
            else
            {  
                return new Int32AggregateSummary(
                    this.count + other.count,
                    Math.Min(this.minimum, other.minimum),
                    Math.Max(this.maximum, other.maximum),
                    this.sum + other.sum);
            }
        }

        /// <summary>Returns string representation of the summary object.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            return String.Concat(
                "Cnt = " + this.Count.ToString(),
                ", Min = " + this.Minimum.ToString(),
                ", Max = " + this.Maximum.ToString(),
                ", Sum = " + this.Sum.ToString(),
                ", Avg = " + this.Average.ToString());
        }
    }

    /// <summary>
    /// Summarizes System.Double fields. Provides Count, Minimum, Maximum, Sum, and Average.
    /// </summary>
    public sealed class DoubleAggregateSummary : SummaryBase, ICountAggregate, IAverageAggregate
    {
        int count;
        double minimum;
        double maximum;
        double sum;

        /// <summary>
        /// The initial summary object for empty records or tables.
        /// </summary>
        public static readonly DoubleAggregateSummary Empty = new DoubleAggregateSummary(0, 0.0, 0.0, 0.0);

        /// <summary>
        /// Creates a summary object for the specified SummaryDescriptor and Record.
        /// </summary>
        /// <param name="sd">The summary descriptor.</param>
        /// <param name="record">The record with data.</param>
        /// <returns>A new summary object.</returns>
        public static ITreeTableSummary CreateSummaryMethod(SummaryDescriptor sd, Record record)
        {
            object obj = sd.GetValue(record);
            bool isNull = obj == null || obj is DBNull || Object.ReferenceEquals(obj, string.Empty);
            if (isNull)
            {
                return Empty;
            }
            else
            {
                double val = Convert.ToDouble(obj);
                return new DoubleAggregateSummary(1, val, val, val);
            }
        }


        public static ITreeTableSummary CreatePageSummaryMethod(SummaryDescriptor sd, Record record)
        {
            object obj = sd.GetValue(record);
            bool isNull = obj == null || obj is DBNull || Object.ReferenceEquals(obj, string.Empty);
            if (isNull)
            {
                return Empty;
            }
            else
            {
                if ((record.ParentTable.Records.IndexOf(record) >= record.ParentTable.StartIndex ) && (record.ParentTable.Records.IndexOf(record) <= record.ParentTable.EndIndex))
                {

                    double val = Convert.ToDouble(obj);
                    return new DoubleAggregateSummary(1, val, val, val);
                }
                else
                {
                    return Empty;
                }
            }
        }

        /// <summary>
        /// Initializes a new summary object with the specified values.
        /// </summary>
        /// <param name="count">The Count value.</param>
        /// <param name="minimum">The Minimum value.</param>
        /// <param name="maximum">The Maximum value.</param>
        /// <param name="sum">The Sum value.</param>
        public DoubleAggregateSummary(int count, double minimum, double maximum, double sum)
        {
            this.count = count;
            this.minimum = minimum;
            this.maximum = maximum;
            this.sum = sum;
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get
            {
                return count;
            }
        }

        /// <summary>
        /// Gets the minimum.
        /// </summary>
        public double Minimum
        {
            get
            {
                return minimum;
            }
        }

        /// <summary>
        /// Gets the maximum.
        /// </summary>
        public double Maximum
        {
            get
            {
                return maximum;
            }
        }

        /// <summary>
        /// Gets the sum.
        /// </summary>
        public double Sum
        {
            get
            {
                return sum;
            }
        }

        /// <summary>
        /// Gets the average.
        /// </summary>
        public double Average
        {
            get
            {
                if (Count == 0)
                {
                    return 0;
                }

                return (double)sum / (double)count;
            }
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object.</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        /// <override/>
        public override SummaryBase Combine(SummaryBase other)
        {
            if (other is DoubleAggregateSummary)
                return Combine((DoubleAggregateSummary)other);
            return other;
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object (of the same type).</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        public DoubleAggregateSummary Combine(DoubleAggregateSummary other)
        {
            if (other.count == 0)
            {
                return this;
            }
            else if (this.count == 0)
            {
                return other;
            }
            else
            {
                return new DoubleAggregateSummary(
                    this.count + other.count,
                    Math.Min(this.minimum, other.minimum),
                    Math.Max(this.maximum, other.maximum),
                    this.sum + other.sum);
            }
        }

        /// <summary>Returns string representation of the summary object.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            return String.Concat(
                "Cnt = " + this.Count.ToString(),
                ", Min = " + this.Minimum.ToString(),
                ", Max = " + this.Maximum.ToString(),
                ", Sum = " + this.Sum.ToString(),
                ", Avg = " + this.Average.ToString());
        }
    }

    /// <summary>
    /// Summarizes Byte fields. Provides Count, Minimum, Maximum.
    /// </summary>
    public sealed class CharAggregateSummary : SummaryBase, ICountAggregate
    {
        int count;
        char minimum;
        char maximum;

        /// <summary>
        /// The initial summary object for empty records or tables.
        /// </summary>
        public static readonly CharAggregateSummary Empty = new CharAggregateSummary(0, '\0', '\0');

        /// <summary>
        /// Creates a summary object for the specified SummaryDescriptor and Record.
        /// </summary>
        /// <param name="sd">The summary descriptor.</param>
        /// <param name="record">The record with data.</param>
        /// <returns>A new summary object.</returns>
        public static ITreeTableSummary CreateSummaryMethod(SummaryDescriptor sd, Record record)
        {
            object obj = sd.GetValue(record);
            bool isNull = obj == null || obj is DBNull || Object.ReferenceEquals(obj, string.Empty);
            if (isNull)
            {
                return Empty;
            }
            else
            {
                char val = Convert.ToChar(obj);
                return new CharAggregateSummary(1, val, val);
            }
        }
        public static ITreeTableSummary CreatePageSummaryMethod(SummaryDescriptor sd, Record record)
        {
            object obj = sd.GetValue(record);
            bool isNull = obj == null || obj is DBNull || Object.ReferenceEquals(obj, string.Empty);
            if (isNull)
            {
                return Empty;
            }
            else
            {
                if ((record.ParentTable.Records.IndexOf(record) >= record.ParentTable.StartIndex) && (record.ParentTable.Records.IndexOf(record) <= record.ParentTable.EndIndex))
                {
                    char val = Convert.ToChar(obj);
                    return new CharAggregateSummary(1, val, val);
                }
                else
                {
                    return Empty;
                }
            }
        }


        
        /// <summary>
        /// Initializes a new summary object with the specified values.
        /// </summary>
        /// <param name="count">The Count value.</param>
        /// <param name="minimum">Minimum value.</param>
        /// <param name="maximum">Maximum value.</param>
        public CharAggregateSummary(int count, char minimum, char maximum)
        {
            this.count = count;
            this.minimum = minimum;
            this.maximum = maximum;
        }
        
        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get
            {
                return count;
            }
        }
        
        /// <summary>
        /// Gets the minimum.
        /// </summary>
        public char Minimum
        {
            get
            {
                return minimum;
            }
        }

        /// <summary>
        /// Gets the maximum.
        /// </summary>
        public char Maximum
        {
            get
            {
                return maximum;
            }
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object.</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        /// <override/>
        public override SummaryBase Combine(SummaryBase other)
        {
            return Combine((CharAggregateSummary)other);
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object (of the same type).</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        public CharAggregateSummary Combine(CharAggregateSummary other)
        {
            if (other.count == 0)
            {
                return this;
            }
            else if (this.count == 0)
            {
                return other;
            }
            else
            {
                return new CharAggregateSummary(
                      this.count + other.count,
                      (char)Math.Min(this.minimum, other.minimum),
                      (char)Math.Max(this.maximum, other.maximum));
            }
        }

        /// <summary>Returns string representation of the summary object.</summary>
        /// <returns>A string holding the summary value.</returns>
        /// <override/>
        public override string ToString()
        {
            return String.Concat(
                "Cnt = " + this.Count.ToString(),
                ", Min = " + this.Minimum.ToString(),
                ", Max = " + this.Maximum.ToString());
        }
    }

    /// <summary>
    /// Summarizes Byte fields. Provides Count, Minimum, Maximum, Sum, and Average.
    /// </summary>
    public sealed class ByteAggregateSummary : SummaryBase, ICountAggregate, IAverageAggregate
    {
        int count;
        byte minimum;
        byte maximum;
        int sum;

        /// <summary>
        /// The initial summary object for empty records or tables.
        /// </summary>
        public static readonly ByteAggregateSummary Empty = new ByteAggregateSummary(0, 0, 0, 0);

        /// <summary>
        /// Creates a summary object for the specified SummaryDescriptor and Record.
        /// </summary>
        /// <param name="sd">The summary descriptor.</param>
        /// <param name="record">The record with data.</param>
        /// <returns>A new summary object.</returns>
        public static ITreeTableSummary CreateSummaryMethod(SummaryDescriptor sd, Record record)
        {
            object obj = sd.GetValue(record);
            bool isNull = obj == null || obj is DBNull || Object.ReferenceEquals(obj, string.Empty);
            if (isNull)
            {
                return Empty;
            }
            else
            {
                byte val = Convert.ToByte(obj);
                return new ByteAggregateSummary(1, val, val, val);
            }
        }
         public static ITreeTableSummary CreatePageSummaryMethod(SummaryDescriptor sd, Record record)
        {
            object obj = sd.GetValue(record);
            bool isNull = obj == null || obj is DBNull || Object.ReferenceEquals(obj, string.Empty);
            if (isNull)
            {
                return Empty;
            }
            else
            {
                if ((record.ParentTable.Records.IndexOf(record) >= record.ParentTable.StartIndex) && (record.ParentTable.Records.IndexOf(record) <= record.ParentTable.EndIndex))
                {
                    byte val = Convert.ToByte(obj);
                    return new ByteAggregateSummary(1, val, val, val);
                }
                else
                {
                    return Empty;
                }
            }
        }
        
        /// <summary>
        /// Initializes a new summary object with the specified values.
        /// </summary>
        /// <param name="count">The Count value.</param>
        /// <param name="minimum">Minimum value.</param>
        /// <param name="maximum">Maximum value.</param>
        /// <param name="sum">The Sum value.</param>
        public ByteAggregateSummary(int count, byte minimum, byte maximum, int sum)
        {
            this.count = count;
            this.minimum = minimum;
            this.maximum = maximum;
            this.sum = sum;
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get
            {
                return count;
            }
        }

        /// <summary>
        /// Gets the minimum.
        /// </summary>
        public byte Minimum
        {
            get
            {
                return minimum;
            }
        }

        /// <summary>
        /// Gets the maximum.
        /// </summary>
        public byte Maximum
        {
            get
            {
                return maximum;
            }
        }

        /// <summary>
        /// Gets the sum.
        /// </summary>
        public int Sum
        {
            get
            {
                return sum;
            }
        }

        /// <summary>
        /// Gets the average.
        /// </summary>
        public double Average
        {
            get
            {
                if (Count == 0)
                {
                    return 0;
                }

                return (double)sum / (double)count;
            }
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object.</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        /// <override/>
        public override SummaryBase Combine(SummaryBase other)
        {
            return Combine((ByteAggregateSummary)other);
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object (of the same type).</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        public ByteAggregateSummary Combine(ByteAggregateSummary other)
        {
            if (other.count == 0)
            {
                return this;
            }
            else if (this.count == 0)
            {
                return other;
            }
            else
            {
                return new ByteAggregateSummary(
                 this.count + other.count,
                 Math.Min(this.minimum, other.minimum),
                 Math.Max(this.maximum, other.maximum),
                 this.sum + other.sum);
            }
        }

        /// <summary>Returns string representation of the summary object.</summary>
        /// <returns>A string holding the summary value.</returns>
        /// <override/>
        public override string ToString()
        {
            return String.Concat(
                "Cnt = " + this.Count.ToString(),
                ", Min = " + this.Minimum.ToString(),
                ", Max = " + this.Maximum.ToString(),
                ", Sum = " + this.Sum.ToString(),
                ", Avg = " + this.Average.ToString());
        }
    }

    /// <summary>
    /// Summarizes Boolean fields. Provides Count, TrueCount, and FalseCount.
    /// </summary>
    public sealed class BooleanAggregateSummary : SummaryBase, ICountAggregate
    {
        int countFalse;
        int countTrue;
        int count;

        /// <summary>
        /// The initial summary object for empty records or tables.
        /// </summary>
        public static readonly BooleanAggregateSummary Empty = new BooleanAggregateSummary(0, 0, 0);

        /// <summary>
        /// Creates a summary object for the specified SummaryDescriptor and Record.
        /// </summary>
        /// <param name="sd">The summary descriptor.</param>
        /// <param name="record">The record with data.</param>
        /// <returns>A new summary object.</returns>
        public static ITreeTableSummary CreateSummaryMethod(SummaryDescriptor sd, Record record)
        {
            object obj = sd.GetValue(record);
            bool isNull = obj == null || obj is DBNull || Object.ReferenceEquals(obj, string.Empty);
            if (isNull)
            {
                return Empty;
            }
            else
            {
                bool val = Convert.ToBoolean(obj);
                return new BooleanAggregateSummary(1, val ? 0 : 1, val ? 1 : 0);
            }
        }
         public static ITreeTableSummary CreatePageSummaryMethod(SummaryDescriptor sd, Record record)
        {
            object obj = sd.GetValue(record);
            bool isNull = obj == null || obj is DBNull || Object.ReferenceEquals(obj, string.Empty);
            if (isNull)
            {
                return Empty;
            }
            else
            {
                if ((record.ParentTable.Records.IndexOf(record) >= record.ParentTable.StartIndex) && (record.ParentTable.Records.IndexOf(record) <= record.ParentTable.EndIndex))
                {
                    bool val = Convert.ToBoolean(obj);
                    return new BooleanAggregateSummary(1, val ? 0 : 1, val ? 1 : 0);
                }
                else
                {
                    return Empty;
                }
            }
        }
       
        /// <summary>
        /// Initializes a new summary object with the specified values.
        /// </summary>
        /// <param name="count">The Count value.</param>
        /// <param name="countFalse">False count.</param>
        /// <param name="countTrue">True count.</param>
        public BooleanAggregateSummary(int count, int countFalse, int countTrue)
        {
            this.countFalse = countFalse;
            this.countTrue = countTrue;
            this.count = count;
        }
        
        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get
            {
                return count;
            }
        }
        
        /// <summary>
        /// Gets the count of False.
        /// </summary>
        public int FalseCount
        {
            get
            {
                return countFalse;
            }
        }

        /// <summary>
        /// Gets the count of True.
        /// </summary>
        public int TrueCount
        {
            get
            {
                return countTrue;
            }
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object.</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        /// <override/>
        public override SummaryBase Combine(SummaryBase other)
        {
            return Combine((BooleanAggregateSummary)other);
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object (of the same type).</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        public BooleanAggregateSummary Combine(BooleanAggregateSummary other)
        {
            if (other.count == 0)
            {
                return this;
            }
            else if (this.count == 0)
            {
                return other;
            }
            else
            {
                return new BooleanAggregateSummary(
                 this.count + other.count,
                 this.countFalse + other.countFalse,
                 this.countTrue + other.countTrue);
            }
        }

        /// <summary>Returns a string holding the summary value.</summary>
        /// <returns>A string holding the summary value.</returns>
        /// <override/>
        public override string ToString()
        {
            return String.Concat(
                "Count = " + this.Count.ToString(),
                "FalseCount = " + this.FalseCount.ToString(),
                ", TrueCount = " + this.TrueCount.ToString());
        }
    }

    /// <summary>
    /// Summarizes string fields. Provides Count and MaximumLength.
    /// </summary>
    public sealed class StringAggregateSummary : SummaryBase, ICountAggregate
    {
        int count;
        int maxLength;

        /// <summary>
        /// The initial summary object for empty records or tables.
        /// </summary>
        public static readonly StringAggregateSummary Empty = new StringAggregateSummary(0, 0);

        /// <summary>
        /// Creates a summary object for the specified SummaryDescriptor and Record.
        /// </summary>
        /// <param name="sd">The summary descriptor.</param>
        /// <param name="record">The record with data.</param>
        /// <returns>A new summary object.</returns>
        public static ITreeTableSummary CreateSummaryMethod(SummaryDescriptor sd, Record record)
        {
            object obj = sd.GetValue(record);
            bool isNull = obj == null || obj is DBNull;
            if (isNull)
            {
                return Empty;
            }
            else
            {
                string text = Convert.ToString(obj);
                return new StringAggregateSummary(1, text.Length);
            }
        }
        public static ITreeTableSummary CreatePageSummaryMethod(SummaryDescriptor sd, Record record)
        {
            object obj = sd.GetValue(record);
            bool isNull = obj == null || obj is DBNull;
            if (isNull)
            {
                return Empty;
            }
            else
            {
                if ((record.ParentTable.Records.IndexOf(record) >= record.ParentTable.StartIndex) && (record.ParentTable.Records.IndexOf(record) <= record.ParentTable.EndIndex))
                {
                    string text = Convert.ToString(obj);
                    return new StringAggregateSummary(1, text.Length);
                }
                else
                {
                    return Empty;
                }
            }
        }

        
        /// <summary>
        /// Initializes a new summary object with the specified values.
        /// </summary>
        /// <param name="count">The Count value.</param>
        /// <param name="maxLength">Maximum length.</param>
        public StringAggregateSummary(int count, int maxLength)
        {
            this.count = count;
            this.maxLength = maxLength;
        }
        
        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get
            {
                return count;
            }
        }

        /// <summary>
        /// Gets the maximum length.
        /// </summary>
        public int MaxLength
        {
            get
            {
                return maxLength;
            }
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object.</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        /// <override/>
        public override SummaryBase Combine(SummaryBase other)
        {
            if (other == null)
                return this;
            return Combine((StringAggregateSummary)other);
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object (of the same type).</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        public StringAggregateSummary Combine(StringAggregateSummary other)
        {
            if (other.count == 0)
            {
                return this;
            }
            else if (this.count == 0)
            {
                return other;
            }
            else
            {
                return new StringAggregateSummary(
                 this.count + other.count,
                 Math.Max(this.maxLength, other.maxLength));
            }
        }

        /// <summary>Returns string representation of the summary object.</summary>
        /// <returns>A string holding the summary object.</returns>
        /// <override/>
        public override string ToString()
        {
            return String.Concat(
                "Cnt = " + this.Count.ToString(),
                ", MaxLength = " + this.MaxLength.ToString());
        }
    }

    /// <summary>
    /// Collects all entries of a column in a sorted vector.
    /// </summary>
    public class VectorSummary : SummaryBase, ICountAggregate
    {
        object[] values;
        int length;

        /// <summary>
        /// The initial summary object for empty records or tables.
        /// </summary>
        public static readonly VectorSummary Empty = new VectorSummary(new object[0], 0);

        /// <summary>
        /// Creates a summary object for the specified SummaryDescriptor and Record.
        /// </summary>
        /// <param name="sd">The summary descriptor.</param>
        /// <param name="record">The record with data.</param>
        /// <returns>A new summary object.</returns>
        public static ITreeTableSummary CreateSummaryMethod(SummaryDescriptor sd, Record record)
        {
            object obj = sd.GetValue(record);
            bool isNull = obj == null || obj is DBNull;
            if (isNull)
            {
                return new VectorSummary(new object[0], 0);
            }
            else
            {
                return new VectorSummary(new object[] { obj }, 1);
            }
        }
        public static ITreeTableSummary CreatePageSummaryMethod(SummaryDescriptor sd, Record record)
        {
            object obj = sd.GetValue(record);
            bool isNull = obj == null || obj is DBNull;
            if (isNull)
            {
                return new VectorSummary(new object[0], 0);
            }
            else
            {
                if ((record.ParentTable.Records.IndexOf(record) >= record.ParentTable.StartIndex) && (record.ParentTable.Records.IndexOf(record) <= record.ParentTable.EndIndex))
                {
                    return new VectorSummary(new object[] { obj }, 1);
                }
                else
                {
                    return new VectorSummary(new object[0], 0);
                }
            }
        }

        
        /// <summary>
        /// Initializes a new summary object with the specified values.
        /// </summary>
        /// <param name="values">Summary values.</param>
        /// <param name="length">Number of values.</param>
        public VectorSummary(object[] values, int length)
        {
            this.values = values;
            this.length = length;
        }

        /// <summary>
        /// Gets the number of elements in the vector.
        /// </summary>
        public int Count
        {
            get
            {
                if (values == null)
                {
                    return 0;
                }

                return length;
            }
        }
        
        /// <summary>
        /// Gets the array with values in the vector.
        /// </summary>
        public object[] Values
        {
            get
            {
                return values;
            }
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object.</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        /// <override/>
        public override SummaryBase Combine(SummaryBase other)
        {
            return Combine((VectorSummary)other);
        }

        int _Compare(object x, object y)
        {
            int cmp;
            bool xIsNull = x == null || x is DBNull || Object.ReferenceEquals(x, string.Empty);
            bool yIsNull = y == null || y is DBNull || Object.ReferenceEquals(y, string.Empty);

            if (yIsNull && xIsNull)
            {
                cmp = 0;
            }
            else if (xIsNull)
            {
                cmp = -1;
            }
            else if (yIsNull)
            {
                cmp = 1;
            }
            else if (x is IComparable)
            {
                cmp = ((IComparable)x).CompareTo(y);
            }
            else
            {
                cmp = 0;
            }

            return cmp;
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object (of the same type).</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        public VectorSummary Combine(VectorSummary other)
        {
            int length;
            object[] d = CombineHelper(other, false, out length);
            if (length == this.Count)
            {
                return this;
            }
            else if (length == other.Count)
            {
                return other;
            }
            else
            {
                return new VectorSummary(d, length);
            }
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object (of the same type).</param>
        /// <param name="distinct">True if vector should only contain unique values; False if duplicates are allowed.</param>
        /// <param name="length">The resulting length of the vector.</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        protected object[] CombineHelper(VectorSummary other, bool distinct, out int length)
        {
            object[] d = new object[(Count + other.Count)];
            object[] others = other.Values;

            int n1 = 0;
            int n2 = 0;
            int len1 = Count;
            int len2 = other.Count;
            int n3 = 0;
            while (n1 < len1 && n2 < len2)
            {
                int cmp = _Compare(values[n1], others[n2]);
                if (cmp < 0)
                {
                    d[n3] = values[n1++];
                }
                else if (cmp > 0)
                {
                    d[n3] = others[n2++];
                }
                else
                {
                    d[n3] = values[n1++];
                    if (distinct)
                    {
                        n2++;
                    }
                }

                n3++;
            }

            while (n1 < len1)
            {
                d[n3++] = values[n1++];
            }

            while (n2 < len2)
            {
                d[n3++] = others[n2++];
            }

            length = n3;
            return d;
        }

        /// <summary>Returns string representation of the summary object.</summary>
        /// <returns>String representation of the summary object.</returns>
        /// <override/>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Concat(
                "Count = " + this.Count.ToString(),
                ", Values = {"));
            for (int n = 0; n < Math.Min(10, Count); n++)
            {
                if (n > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(values[n] == null || values[n] is DBNull ? "null" : values[n].ToString());
            }

            if (Count >= 10)
            {
                sb.Append(", ...");
            }

            sb.Append("} ");
            return sb.ToString();
        }
    }

    /// <summary>
    /// Provides distinct count of a field and a vector with all distinct values of the field.
    /// </summary>
    public sealed class DistinctCountSummary : VectorSummary, ICountAggregate
    {
        /// <summary>
        /// The initial summary object for empty records or tables.
        /// </summary>
        public new static readonly DistinctCountSummary Empty = new DistinctCountSummary(new object[0], 0);

        /// <summary>
        /// Creates a summary object for the specified SummaryDescriptor and Record.
        /// </summary>
        /// <param name="sd">The summary descriptor.</param>
        /// <param name="record">The record with data.</param>
        /// <returns>A new summary object.</returns>
        public new static ITreeTableSummary CreateSummaryMethod(SummaryDescriptor sd, Record record)
        {
            object obj = sd.GetValue(record);
            bool isNull = obj == null || obj is DBNull;
            if (isNull)
            {
                return new DistinctCountSummary(new object[] { "(null)" }, 0); //// { DBNull.Value }, 1);
            }
            else
            {
                return new DistinctCountSummary(new object[] { obj }, 1);
            }
        }
          public new static ITreeTableSummary CreatePageSummaryMethod(SummaryDescriptor sd, Record record)
        {
            object obj = sd.GetValue(record);
            bool isNull = obj == null || obj is DBNull;
            if (isNull)
            {
                return new DistinctCountSummary(new object[] { "(null)" }, 0); //// { DBNull.Value }, 1);
            }
            else
            {
                 if ((record.ParentTable.Records.IndexOf(record) >= record.ParentTable.StartIndex) && (record.ParentTable.Records.IndexOf(record) <= record.ParentTable.EndIndex))
                {
                     return new DistinctCountSummary(new object[] { obj }, 1);
                }
                else
                 {
                     return new DistinctCountSummary(new object[] { "(null)" }, 0);
                 }
            }
        }



        
        /// <summary>
        /// Initializes a new summary object with the specified values.
        /// </summary>
        /// <param name="values">Object array that contains summary values.</param>
        /// <param name="length">Number of values.</param>
        public DistinctCountSummary(object[] values, int length)
            : base(values, length)
        {
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object.</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        /// <override/>
        public override SummaryBase Combine(SummaryBase other)
        {
            return Combine((DistinctCountSummary)other);
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object (of the same type).</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        public DistinctCountSummary Combine(DistinctCountSummary other)
        {
            int length;
            object[] d = CombineHelper(other, true, out length);
            if (length == this.Count)
            {
                return this;
            }
            else if (length == other.Count)
            {
                return other;
            }
            else
            {
                return new DistinctCountSummary(d, length);
            }
        }
    }

    ////    ///// <summary>
    ////    ///// Provides distinct count of a field and a vector with all distinct values of the field.
    ////    ///// </summary>
    ////    public sealed class FilterBarSummary : VectorSummary, ICountAggregate
    ////    {
    ////        public new static readonly FilterBarSummary Empty = new FilterBarSummary(new object[0], 0);
    ////
    ////        public new static ITreeTableSummary CreateSummaryMethod(SummaryDescriptor sd, Record record)
    ////        {
    ////            object obj = sd.GetValue(record);
    ////            bool isNull = (obj == null || obj is DBNull);
    ////            if (isNull)
    ////                return new FilterBarSummary(new object[] { "(null)", 1 } );//// { DBNull.Value }, 1);
    ////            else
    ////            {
    ////                return new FilterBarSummary(new object[] { obj }, 1);
    ////            }
    ////        }
    ////
    ////        public FilterBarSummary(object[] values, int length)
    ////            : base(values, length)
    ////        {
    ////        }
    ////
    ////        public override SummaryBase Combine(SummaryBase other)
    ////        {
    ////            return Combine((FilterBarSummary) other);
    ////        }
    ////
    ////        public FilterBarSummary Combine(FilterBarSummary other)
    ////        {
    ////            int length;
    ////            object[] d = CombineHelper(other, true, out length);
    ////            if (length == this.Count)
    ////                return this;
    ////            else if (length == other.Count)
    ////                return other;
    ////            else
    ////                return new FilterBarSummary(d, length);
    ////        }
    ////    }

    /// <summary>
    /// Collects all entries of a column in a sorted vector. Provides statistical functions
    /// that work on this set such as Median, Percentile25, Percentile75, and PercentileQ.
    /// </summary>
    public class DoubleVectorSummary : SummaryBase, ICountAggregate
    {
        double[] values;
        int _length;

        /// <summary>
        /// The initial summary object for empty records or tables.
        /// </summary>
        public static readonly DoubleVectorSummary Empty = new DoubleVectorSummary(new double[0], 0);

        /// <summary>
        /// Creates a summary object for the specified SummaryDescriptor and Record.
        /// </summary>
        /// <param name="sd">The summary descriptor.</param>
        /// <param name="record">The record with data.</param>
        /// <returns>A new summary object.</returns>
        public static ITreeTableSummary CreateSummaryMethod(SummaryDescriptor sd, Record record)
        {
            object obj = sd.GetValue(record);
            bool isNull = (obj == null || obj is DBNull || Object.ReferenceEquals(obj, string.Empty))
                || ((obj is double) && double.IsNaN((double)obj));
            //// could also be double.NaN... which is also null

            if (isNull)
            {
                return new DoubleVectorSummary(new double[0], 0); //// { double.NaN }, 1);
            }
            else
            {
                double val = Convert.ToDouble(obj);
                return new DoubleVectorSummary(new double[] { val }, 1);
            }
        }
          public static ITreeTableSummary CreatePageSummaryMethod(SummaryDescriptor sd, Record record)
        {
            object obj = sd.GetValue(record);
            bool isNull = (obj == null || obj is DBNull || Object.ReferenceEquals(obj, string.Empty))
                || ((obj is double) && double.IsNaN((double)obj));
            //// could also be double.NaN... which is also null

            if (isNull)
            {
                return new DoubleVectorSummary(new double[0], 0); //// { double.NaN }, 1);
            }
            else
            {
                if ((record.ParentTable.Records.IndexOf(record) >= record.ParentTable.StartIndex) && (record.ParentTable.Records.IndexOf(record) <= record.ParentTable.EndIndex))
                {

                    double val = Convert.ToDouble(obj);
                    return new DoubleVectorSummary(new double[] { val }, 1);
                }
                else
                {
                    return new DoubleVectorSummary(new double[0], 0);
                }
            }
        }
         
        /// <summary>
        /// Initializes a new summary object with the specified values.
        /// </summary>
        /// <param name="values">An object array containing summary values.</param>
        /// <param name="_length">Number of values.</param>
        public DoubleVectorSummary(double[] values, int _length)
        {
            this.values = values;
            this._length = _length;
        }

        /// <summary>
        /// Gets the number of elements in the vector.
        /// </summary>
        public int Count
        {
            get
            {
                if (values == null)
                {
                    return 0;
                }

                return _length;
            }
        }

        /// <summary>
        /// Gets the array with values in the vector.
        /// </summary>
        public double[] Values
        {
            get
            {
                return values;
            }
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object.</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        /// <override/>
        public override SummaryBase Combine(SummaryBase other)
        {
            return Combine((DoubleVectorSummary)other);
        }

        int _Compare(double x, double y)
        {
            int cmp;
            bool xIsNull = double.IsNaN(x);
            bool yIsNull = double.IsNaN(y); 

            if (yIsNull && xIsNull)
            {
                cmp = 0;
            }
            else if (xIsNull)
            {
                cmp = -1;
            }
            else if (yIsNull)
            {
                cmp = 1;
            }
            else if (y == x)
            {
                cmp = 0;
            }
            else if (y > x)
            {
                cmp = 1;
            }
            else
            {
                cmp = -1;
            }

            return cmp;
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object (of the same type).</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        public DoubleVectorSummary Combine(DoubleVectorSummary other)
        {
            int _length;
            double[] d = CombineHelper(other, false, out _length);
            if (_length == this.Count)
            {
                return this;
            }
            else if (_length == other.Count)
            {
                return other;
            }
            else
            {
                return new DoubleVectorSummary(d, _length);
            }
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object (of the same type).</param>
        /// <param name="distinct">True if vector should only contain unique values; False if duplicates are allowed.</param>
        /// <param name="length">The resulting length of the vector.</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        protected double[] CombineHelper(DoubleVectorSummary other, bool distinct, out int length)
        {
            double[] d = new double[(Count + other.Count)];
            double[] others = other.Values;

            int n1 = 0;
            int n2 = 0;
            int len1 = Count;
            int len2 = other.Count;
            int n3 = 0;
            while (n1 < len1 && n2 < len2)
            {
                int cmp = _Compare(values[n1], others[n2]);
                if (cmp > 0)
                {
                    d[n3] = values[n1++];
                }
                else if (cmp < 0)
                {
                    d[n3] = others[n2++];
                }
                else
                {
                    d[n3] = values[n1++];
                    if (distinct)
                    {
                        n2++;
                    }
                }

                n3++;
            }

            while (n1 < len1)
            {
                d[n3++] = values[n1++];
            }

            while (n2 < len2)
            {
                d[n3++] = others[n2++];
            }

            length = n3;
            return d;
        }

        double GetPercentile(double p)
        {
            if (p < 0 || p > 1)
            {
                throw new ArgumentOutOfRangeException("Percentile out-of-range.");
            }

            double[] s = values;
            double t = p * (_length - 1);
            int i = (int)t;
            return ((i + 1 - t) * s[i]) + ((t - i) * s[i + 1]);
        }

        /// <summary>
        /// Gets the statistical median.
        /// </summary>
        public double Median
        {
            get
            {
                if (_length < 2)
                {
                    return double.NaN;
                }

                return GetPercentile(0.5);
            }
        }

        /// <summary>
        /// Gets the statistical Percentile25.
        /// </summary>
        public double Percentile25
        {
            get
            {
                if (_length < 2)
                {
                    return double.NaN;
                }

                return GetPercentile(0.25);
            }
        }

        /// <summary>
        /// Gets the statistical Percentile75.
        /// </summary>
        public double Percentile75
        {
            get
            {
                if (_length < 2)
                {
                    return double.NaN;
                }

                return GetPercentile(0.75);
            }
        }

        /// <summary>
        /// Gets the statistical PercentileQ.
        /// </summary>
        public double PercentileQ
        {
            get
            {
                if (_length < 2)
                {
                    return double.NaN;
                }

                return Percentile75 - Percentile25;
            }
        }

        /// <summary>Returns string representation of the summary object.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Concat(
                "Count = " + this.Count.ToString(),
                ", Values = { "));
            for (int n = 0; n < Math.Min(5, Count); n++)
            {
                if (n > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(double.IsNaN(values[n]) ? "null" : values[n].ToString("G"));
            }

            if (Count >= 5)
            {
                sb.Append(", ...");
            }

            sb.Append(" }");
            if (Count > 1)
            {
                sb.AppendFormat(", Med={0}", this.Median);
                sb.AppendFormat(", P25={0}", this.Percentile25);
                sb.AppendFormat(", P75={0}", this.Percentile75);
                sb.AppendFormat(", PQ={0}", this.PercentileQ);
            }

            return sb.ToString();
        }
    }    

    /// <summary>
    /// Provides distinct values of a field that can be used in a filterbar.
    /// </summary>
    public sealed class FilterBarChoicesSummary : VectorSummary, ICountAggregate
    {
        /// <summary>
        /// The initial summary object for empty records or tables.
        /// </summary>
        public new static readonly FilterBarChoicesSummary Empty = new FilterBarChoicesSummary(new object[0], 0);

        /// <summary>
        /// Creates a summary object for the specified SummaryDescriptor and record.
        /// </summary>
        /// <param name="sd">The summary descriptor.</param>
        /// <param name="record">The record with data.</param>
        /// <returns>A new summary object.</returns>
        public new static ITreeTableSummary CreateSummaryMethod(SummaryDescriptor sd, Record record)
        {
            bool criteria = sd.TableDescriptor.RecordFilters.CompareRecordFilterBar(record, sd.FieldDescriptor);
            if (!criteria)
            {
                return FilterBarChoicesSummary.Empty;
            }
           
            object obj = sd.GetValue(record);
            bool isNull = obj == null || obj is DBNull;
            if (isNull)
            {
                return new FilterBarChoicesSummary(new object[] { DBNull.Value }, 1);
            }
            else
            {
                return new FilterBarChoicesSummary(new object[] { obj }, 1);
            }
        }
        public new static ITreeTableSummary CreatePageSummaryMethod(SummaryDescriptor sd, Record record)
        {
            bool criteria = sd.TableDescriptor.RecordFilters.CompareRecordFilterBar(record, sd.FieldDescriptor);
            if (!criteria)
            {
                return FilterBarChoicesSummary.Empty;
            }
           
            object obj = sd.GetValue(record);
            bool isNull = obj == null || obj is DBNull;
            if (isNull)
            {
                return new FilterBarChoicesSummary(new object[] { DBNull.Value }, 1);
            }
            else
            {
                 if ((record.ParentTable.Records.IndexOf(record) >= record.ParentTable.StartIndex) && (record.ParentTable.Records.IndexOf(record) <= record.ParentTable.EndIndex))
                 {
                     return new FilterBarChoicesSummary(new object[] { obj }, 1);
                 }
                else
                 {
                      return new FilterBarChoicesSummary(new object[] { DBNull.Value }, 1);
                 }
                
            }
        }


        

        /// <summary>
        /// Initializes a new summary object with the specified values.
        /// </summary>
        /// <param name="values">Summary values.</param>
        /// <param name="length">Number of values.</param>
        public FilterBarChoicesSummary(object[] values, int length)
            : base(values, length)
        {
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object.</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        /// <override/>
        public override SummaryBase Combine(SummaryBase other)
        {
            return Combine((FilterBarChoicesSummary)other);
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object (of the same type).</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        public FilterBarChoicesSummary Combine(FilterBarChoicesSummary other)
        {
            int length;
            object[] d = CombineHelper(other, true, out length);
            if (length == this.Count)
            {
                return this;
            }
            else if (length == other.Count)
            {
                return other;
            }
            else
            {
                return new FilterBarChoicesSummary(d, length);
            } 
        }
    }

    ////    public sealed class DistinctInt32CountSummary : SummaryBase, ICountAggregate
    ////    {
    ////        Int32[] values;
    ////
    ////        public static readonly DistinctInt32CountSummary Empty = new DistinctInt32CountSummary(new Int32[0]);
    ////
    ////        public DistinctInt32CountSummary(object[] values)
    ////        {
    ////            this.values = values;
    ////        }
    ////
    ////        public int Count
    ////        {
    ////            get
    ////            {
    ////                if (values == null)
    ////                    return 0;
    ////                return values.Length;
    ////            }
    ////        }
    ////        public Int32[] Values
    ////        {
    ////            get
    ////            {
    ////                return values;
    ////            }
    ////        }
    ////
    ////        ITreeTableSummary ITreeTableSummary.Combine(ITreeTableSummary other)
    ////        {
    ////            return Combine((DistinctInt32CountSummary) other);
    ////        }
    ////
    ////        int _Compare(object x, object y)
    ////        {
    ////            int cmp;
    ////            bool xIsNull = (x == null || x is DBNull);
    ////            bool yIsNull = (y == null || y is DBNull);
    ////
    ////            if (yIsNull && xIsNull)
    ////                cmp = 0;
    ////            else if (xIsNull)
    ////                cmp = -1;
    ////            else if (yIsNull)
    ////                cmp = 1;
    ////            else
    ////                cmp = ((IComparable) x).CompareTo(y);
    ////
    ////            return cmp;
    ////        }
    ////
    ////        public DistinctInt32CountSummary Combine(DistinctInt32CountSummary other)
    ////        {
    ////            ArrayList d = new ArrayList(Count + other.Count);
    ////            object[] others = other.Values;
    ////
    ////            int n1 = 0;
    ////            int n2 = 0;
    ////            int len1 = values.Length;
    ////            int len2 = others.Length;
    ////            while (n1 < len1 && n2 < len2)
    ////            {
    ////                int cmp = _Compare(values[n1], others[n2]);
    ////                if (cmp > 0)
    ////                    d.Add(values[n1++]);
    ////                else if (cmp < 0)
    ////                    d.Add(others[n2++]);
    ////                else
    ////                {
    ////                    d.Add(values[n1++]);
    ////                    n2++;
    ////                }
    ////            }
    ////            while (n1 < len1)
    ////                d.Add(values[n1++]);
    ////
    ////            while (n2 < len2)
    ////                d.Add(others[n2++]);
    ////
    ////            return new DistinctInt32CountSummary(d.ToArray());
    ////        }
    ////
    ////        public override string ToString()
    ////        {
    ////            StringBuilder sb = new StringBuilder();
    ////            sb.Append(String.Concat(
    ////                "Count = " + this.Count.ToString(),
    ////                "Values = {"));
    ////            for (int n = 0; n < Math.Min(10, Count); n++)
    ////            {
    ////                if (n > 0)
    ////                    sb.Append(", ");
    ////                sb.Append(values[n] == null || values[n] is DBNull ? "null" : values[n].ToString());
    ////            }
    ////            if (Count >= 10)
    ////                sb.Append(", ...");
    ////            sb.Append("}" );
    ////            return sb.ToString();
    ////        }
    ////    }

    ////    public sealed class VectorSummary : SummaryBase, ICountAggregate
    ////    {
    ////        object[] values;
    ////
    ////        public static readonly VectorSummary Empty = new VectorSummary(new object[0]);
    ////
    ////        public static ITreeTableSummary CreateSummaryMethod(SummaryDescriptor sd, Record record)
    ////        {
    ////            object obj = sd.GetValue(record);
    ////            bool isNull = (obj == null || obj is DBNull);
    ////            if (isNull)
    ////                return Empty;
    ////            else
    ////            {
    ////                return new VectorSummary(new object[] { obj });
    ////            }
    ////        }
    ////
    ////        public VectorSummary(object[] values)
    ////        {
    ////            this.values = values;
    ////        }
    ////
    ////        public int Count
    ////        {
    ////            get
    ////            {
    ////                if (values == null)
    ////                    return 0;
    ////                return values.Length;
    ////            }
    ////        }
    ////        public object[] Values
    ////        {
    ////            get
    ////            {
    ////                return values;
    ////            }
    ////        }
    ////
    ////        ITreeTableSummary ITreeTableSummary.Combine(ITreeTableSummary other)
    ////        {
    ////            return Combine((VectorSummary) other);
    ////        }
    ////
    ////        public VectorSummary Combine(VectorSummary other)
    ////        {
    ////            ArrayList d = new ArrayList(Count + other.Count);
    ////
    ////            d.AddRange(values);
    ////            d.AddRange(other.values);
    ////
    ////            return new VectorSummary(d.ToArray());
    ////        }
    ////
    ////        public override string ToString()
    ////        {
    ////            StringBuilder sb = new StringBuilder();
    ////            sb.Append(String.Concat(
    ////                "Count = " + this.Count.ToString(),
    ////                ", Values = {"));
    ////            for (int n = 0; n < Math.Min(10, Count); n++)
    ////            {
    ////                if (n > 0)
    ////                    sb.Append(", ");
    ////                sb.Append(values[n] == null || values[n] is DBNull ? "null" : values[n].ToString());
    ////            }
    ////            if (Count >= 10)
    ////                sb.Append(", ...");
    ////            sb.Append("} " );
    ////            return sb.ToString();
    ////        }
    ////    }
}
