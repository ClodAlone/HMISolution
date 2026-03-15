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

#if WINRT
using System.Threading.Tasks;
using Windows.Foundation;
#else
using System.Windows;
#if WINDOWSFORMS
using System.Drawing;
using Syncfusion.Windows.Forms.Maps;
#endif
#endif

namespace Syncfusion.UI.Xaml.Maps
{
    /// <summary>
    /// The ShapeFileRecord class represents the contents of
    /// a shape record, which is of variable length.
    /// </summary>
    internal class ShapeFileRecord
    {
        #region Constructor
        /// <summary>
        /// Constructor for the ShapeFileRecord class.
        /// </summary>
        public ShapeFileRecord()
        {
            this.Parts = new List<int>();
#if !WINDOWSFORMS
            this.Points = new List<Point>();
#else
            this.Points = new List<System.Drawing.Point>();
#endif
            this.Attributes = new Dictionary<string, object>();
        }
        #endregion Constructor

        #region Properties

#if WINDOWSFORMS
        /// <summary>
        /// specifies the Mid point between two variables
        /// </summary>
       internal  System.Drawing.Point MidPoint { get; set; }
#endif
        /// <summary>
        /// Indicates the record number (or index) which starts at 1.
        /// </summary>
        public int RecordIndex
        {
            get;
            internal set;
        }

        /// <summary>
        /// Specifies the length of this shape record in 16-bit words.
        /// </summary>
        public int ContentLength
        {
            get;
            internal set;
        }

        /// <summary>
        /// Specifies the shape type for this record.
        /// </summary>
        public ShapeType ShapeType
        {
            get;
            internal set;
        }

        /// <summary>
        /// Indicates the minimum x-position of the bounding
        /// box for the shape (expressed in degrees longitude).
        /// </summary>
        public double MinX
        {
            get;
            internal set;
        }

        /// <summary>
        /// Indicates the minimum y-position of the bounding
        /// box for the shape (expressed in degrees latitude).
        /// </summary>
        public double MinY
        {
            get;
            internal set;
        }

        /// <summary>
        /// Indicates the maximum x-position of the bounding
        /// box for the shape (expressed in degrees longitude).
        /// </summary>
        public double MaxX
        {
            get;
            internal set;
        }

        /// <summary>
        /// Indicates the maximum y-position of the bounding
        /// box for the shape (expressed in degrees latitude).
        /// </summary>
        public double MaxY
        {
            get;
            internal set;
        }

        /// <summary>
        /// Indicates the number of parts for this shape.
        /// A part is a connected set of points, analogous to
        /// a PathFigure in WPF.
        /// </summary>
        public int GetPartsCount()
        {
            return this.Parts.Count;
        }

        /// <summary>
        /// Specifies the total number of points defining
        /// this shape record.
        /// </summary>
        public int GetPointsCount()
        {
            return this.Points.Count;
        }

        /// <summary>      
        /// A collection of indices for the points array.
        /// Each index identifies the starting point of the
        /// corresponding part (or PathFigure using WPF
        /// terminology).
        /// </summary>
        public List<int> Parts
        {
            get;
            private set;
        }

        /// <summary>
        /// A collection of all of the points defining the
        /// shape record.
        /// </summary>
#if !WINDOWSFORMS
        public List<Point> Points
        {
            get;
            private set;
        }
#else
        public List<System.Drawing.Point> Points
        {
            get;
            private set;
        }
#endif
        /// <summary>
        /// Access the (dBASE) attribute values associated
        /// with this shape record.
        /// </summary>
        public Dictionary<string, object> Attributes
        {
            get;
            internal set;
        }
        #endregion Properties

        /// <summary>
        /// Output some of the fields of the shapefile record.
        /// </summary>
        /// <returns>A string representation of the record.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("ShapeFileRecord: RecordIndex={0}, ContentLength={1}, ShapeType={2}", this.RecordIndex, this.ContentLength, this.ShapeType);
            return sb.ToString();
        }
    }

}
