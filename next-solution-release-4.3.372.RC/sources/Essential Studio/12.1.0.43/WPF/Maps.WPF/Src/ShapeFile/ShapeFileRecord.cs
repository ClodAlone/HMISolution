#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Maps.IO
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
#if WPF
    using System.Data;
#endif
    using System.Linq;
    using System.Text;
    using System.Windows;

    /// <summary>
    /// The ShapeFileRecord class represents the contents of
    /// a shape record, which is of variable length.
    /// </summary>
    public class ShapeFileRecord
    {
        #region Constructor
        /// <summary>
        /// Constructor for the ShapeFileRecord class.
        /// </summary>
        public ShapeFileRecord()
        {
            this.Parts = new List<int>();
            this.Points = new List<Point>();
            this.Attributes = new Dictionary<string, object>();
        }
        #endregion Constructor

        #region Properties
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
        public List<Point> Points
        {
            get;
            private set;
        }

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

    /// <summary>
    ///  This Represents the Type of Shapes.
    /// </summary>
    public enum ShapeType
    {
        /// <summary>
        /// Null shape / placeholder record.
        /// </summary>
        NullShape = 0,

        /// <summary>
        /// Point record, for defining point locations such as a city.
        /// </summary>
        Point = 1,

        /// <summary>
        /// One or more sets of connected points. Used to represent roads,
        /// hydrographs, etc.
        /// </summary>
        PolyLine = 3,

        /// <summary>
        /// One or more sets of closed figures. Used to represent political
        /// boundaries for countries, lakes, etc.
        /// </summary>
        Polygon = 5,

        /// <summary>
        /// A cluster of points represented by a single shape record.
        /// </summary>
        Multipoint = 8

        // Unsupported types:
        // PointZ = 11,        
        // PolyLineZ = 13,        
        // PolygonZ = 15,        
        // MultiPointZ = 18,        
        // PointM = 21,        
        // PolyLineM = 23,        
        // PolygonM = 25,        
        // MultiPointM = 28,        
        // MultiPatch = 31
    }
}
