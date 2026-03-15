// <copyright file="ChartData.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Data;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Xml;
//using Syncfusion.Windows.Data;

    /// <summary>
    /// Represents strongly typed Chart datapoints collection.
    /// </summary>
    /// <remarks>
    /// Interface defines methods and properties that are required for proper chart
    /// building.
    /// </remarks>
    /// <example>
    /// This sample demonstrates how interface can be implemented: C#: <code
    /// language="C#">
    /// class CustomPoint : IChartDataPoint
    /// {
    /// public CustomPoint(double X, double Y)
    /// {
    /// this.X = X;
    /// this.Y = Y;
    /// this.Values = new double[] {Y};
    /// }
    /// public double X {get; set;}
    /// public double Y {get; set;}
    /// public double[] Values {get; set;}
    /// public bool IsEmpty {get; set;}
    /// public bool Visible {get; set;}
    /// public ChartSegment ParentSegment {get; set;}
    /// public object Item {get; set;}
    /// public object Clone()
    /// {
    /// CustomPoint customPoint = new CustomPoint(this.X, this.Y);
    /// //...
    /// // Filling proper fields.
    /// //...
    /// return customPoint;
    /// }
    /// }
    /// // Custom collection strongly typed as CustomPoint that implements IChartData.
    /// class CustomChartPointsCollection : ObservableCollection &lt;CustomPoint&gt;,
    /// IChartData
    /// {
    /// public new IChartDataPoint this[int index]
    /// {
    /// get { return base[index]; }
    /// }
    /// public ChartValueType XValueType { get; set; }
    /// }
    /// //Using classes
    /// //...
    /// //Creating a new chart1 with area and series.
    /// //...
    /// CustomChartPointsCollection customCollection = new
    /// CustomChartPointsCollection();
    /// customCollection.Add(new CustomPoint(1, 3));
    /// customCollection.Add(new CustomPoint(2, 5));
    /// customCollection.Add(new CustomPoint(3, 2));
    /// customCollection.Add(new CustomPoint(4, 8));
    /// chart1.Areas[0].Series[0].Data = customCollection;
    /// </code> XAML: <code>
    /// <para>
    /// This type is not intended to be used from XAML.
    /// </para>
    /// </code>
    /// </example>
    /// <seealso cref="ChartListData">ChartListData</seealso>
    /// 
    [TypeConverter(typeof(ChartListDataConverter))]
    public interface IChartData : INotifyCollectionChanged, IDisposable
    {
        /// <summary>
        /// Gets the points count.
        /// </summary>
        /// <value>Total points count</value>
        int Count { get; }

        /// <summary>
        /// Gets the <see cref="Syncfusion.Windows.Chart.IChartDataPoint"/> at the specified index.
        /// </summary>
        /// <param name="index">The index value</param>
        /// <value><see cref="IChartDataPoint"/> point at specified index.</value>
        IChartDataPoint this[int index]
        {
            get;
        }

        /// <summary>
        /// Gets the type of the X value.
        /// </summary>
        /// <value>Value can be set from one of <see cref="ChartValueType"/> enumeration.</value>
        ChartValueType XValueType
        {
            get;
        }

        /// <summary>
        /// Gets or Sets the type of the Chart X value
        /// </summary>
        ChartValueType ChartXValueType
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents chart point interface that any chart point should implement.
    /// </summary>
    /// <remarks>
    /// Interface defines methods and properties that are required for proper chart
    /// building.
    /// </remarks>
    /// <example>
    /// This sample demonstrates how interface can be implemented: C#: <code
    /// language="C#">
    /// class CustomPoint : IChartDataPoint
    /// {
    /// public CustomPoint(double X, double Y)
    /// {
    /// this.X = X;
    /// this.Y = Y;
    /// this.Values = new double[] {Y};
    /// }
    /// public double X {get; set;}
    /// public double Y {get; set;}
    /// public double[] Values {get; set;}
    /// public bool IsEmpty {get; set;}
    /// public bool Visible {get; set;}
    /// public ChartSegment ParentSegment {get; set;}
    /// public object Item {get; set;}
    /// public object Clone()
    /// {
    /// CustomPoint customPoint = new CustomPoint(this.X, this.Y);
    /// //...
    /// // Filling proper fields.
    /// //...
    /// return customPoint;
    /// }
    /// }
    /// // Custom collection strongly typed as CustomPoint that implements IChartData.
    /// class CustomChartPointsCollection : ObservableCollection &lt;CustomPoint&gt;,
    /// IChartData
    /// {
    /// public new IChartDataPoint this[int index]
    /// {
    /// get { return base[index]; }
    /// }
    /// public ChartValueType XValueType { get; set; }
    /// }
    /// //Using classes
    /// //...
    /// //Creating a new chart1 with area and series.
    /// //...
    /// CustomChartPointsCollection customCollection = new
    /// CustomChartPointsCollection();
    /// customCollection.Add(new CustomPoint(1, 3));
    /// customCollection.Add(new CustomPoint(2, 5));
    /// customCollection.Add(new CustomPoint(3, 2));
    /// customCollection.Add(new CustomPoint(4, 8));
    /// chart1.Areas[0].Series[0].Data = customCollection;
    /// </code> XAML: <code>
    /// <para>
    /// This type is not intended to be used from XAML.
    /// </para>
    /// </code>
    /// </example>
    /// <seealso cref="ChartListData">ChartListData</seealso>
    public interface IChartDataPoint : ICloneable, IDisposable
    {
        /// <summary>
        /// Gets or sets the X point value.
        /// </summary>
        /// <value>The X value.</value>
        double X { get; set; }

        /// <summary>
        /// Gets or sets the Y point value.
        /// </summary>
        /// <value>The Y value.</value>
        double Y { get; set; }

        /// <summary>
        /// Gets or sets the point values.
        /// </summary>
        /// <remarks>
        /// Values array should be used to represent range of Y values that correspond to one X value.
        /// </remarks>
        /// <value>The values.</value>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        double[] Values { get; set; }

        /// <summary>
        /// Gets a value indicating whether this point is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        bool IsEmpty { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IChartDataPoint"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        bool Visible { get; set; }

        /// <summary>
        /// Gets or sets the parent segment.
        /// </summary>
        /// <remarks>
        /// Parent segment for point should be set in order to provide user with ability to disable certain points on series.
        /// <para>
        /// Should not use this property if dynamic points hiding is not required.
        /// </para>
        /// </remarks>
        /// <value>The parent segment <see cref="ChartSegment"/>.</value>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        ChartSegment ParentSegment { get; set; }

        /// <summary>
        /// Gets or sets the item that point represents. Item could be ether X,Y of custom value.
        /// </summary>
        /// <value>The item that point represent.</value>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        object Item { get; set; }

        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        /// <summary>
        /// Gets or sets the Tag that point represents. 
        /// </summary>
        object Tag { get; set; }

        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        /// <summary>
        /// Get or Sets the StringItem that point represents
        /// </summary>
        object StringItem { get; set; }

        /// <summary>
        /// Gets or sets label for point.
        /// </summary>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        string Label
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether empty point.
        /// </summary>
        /// <value><c>true</c> if empty point otherwise, <c>false</c>.</value>

        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        bool EmptyPoint
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents chart point base class that any chart point may implement.
    /// </summary>
    /// <seealso cref="ChartListData"/>
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public abstract class ChartPointBase : DependencyObject, IChartDataPoint
    {
        #region Properites

        /// <summary>
        /// Gets or sets the item that point represents. Item could be ether X,Y of custom value.
        /// </summary>
        /// <value>The item value.</value>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public virtual object Item
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Tag that point represents. 
        /// </summary>
        public virtual object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the item that points represents string values.
        /// </summary>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public virtual object StringItem
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parent segment.
        /// </summary>
        /// <value>The parent segment <see cref="ChartSegment"/>.</value>
        /// <remarks>
        /// Parent segment for point should be set in order to provide user with ability to disable certain points on series.
        /// <para>
        /// Should not use this property if dynamic points hiding is not required.
        /// </para>
        /// </remarks>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public virtual ChartSegment ParentSegment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IChartDataPoint"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public virtual bool Visible
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the X point value.
        /// </summary>
        /// <value>The X value.</value>
        public abstract double X
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the point values.
        /// </summary>
        /// <value>The values.</value>
        /// <remarks>
        /// Values array should be used to represent range of Y values that correspond to one X value.
        /// </remarks>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public abstract double[] Values
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Y point value.
        /// </summary>
        /// <value>The Y value that point represents.</value>
        public virtual double Y
        {
            get
            {
                return this.Values[0];
            }

            set
            {
                if (this.Values != null)
                {
                    this.Values[0] = value;
                }
                else
                {
                    this.Values = new double[] { value };
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this point is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public virtual bool IsEmpty
        {
            get
            {
                return this.Values == null;
            }
        }

        /// <summary>
        /// Gets the <see cref="DateTime"/> X.
        /// </summary>
        /// <value>The date time X.</value>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public DateTime DateTimeX
        {
            get
            {
                return DateTime.FromOADate(this.X);
            }
        }

        /// <summary>
        /// Gets or sets label for the point.
        /// </summary>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public virtual string Label
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether empty point.
        /// </summary>
        /// <value><c>true</c> if empty point otherwise, <c>false</c>.</value>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public abstract bool EmptyPoint
        {
            get;
            set;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Creates a new point that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public abstract object Clone();
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose()
        {
            this.ParentSegment = null;
            this.Item = null;
            this.StringItem = null;
        }

        #endregion

    }
}
