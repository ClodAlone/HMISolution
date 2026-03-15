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
    using System.Linq;
    using System.Text;

    /// <summary>
    ///  ShapeFileHeader is a Caption or Title of the ShapeFile
    /// </summary>
    public class ShapeFileHeader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Maps.IO.ShapeFileHeader"/> class.
        /// </summary>
        public ShapeFileHeader()
        {
        }

        #region Properties
        /// <summary>
        /// Indicate the fixed-length of this header in bytes.
        /// </summary>
        public static int Length
        {
            get { return 100; }
        }

        /// <summary>
        /// Specifies the file code for an ESRI shapefile, which
        /// should be the value, 9994.
        /// </summary>
        public int FileCode
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies the length of the shapefile, expressed
        /// as the number of 16-bit words in the file.
        /// </summary>
        public int FileLength
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies the shapefile version number.
        /// </summary>
        public int Version
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies the shape type for the file. A shapefile
        /// contains only one type of shape.
        /// </summary>
        public ShapeType ShapeType
        {
            get;
            internal set;
        }

        /// <summary>
        /// Indicates the minimum x-position of the bounding box.
        /// </summary>
        public double MinX
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the minimum y-position of the bounding box.
        /// </summary>
        public double MinY
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the minimium z-position of the bounding box.
        /// </summary>
        public double MinZ
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the minimum m-position of the bounding polygon
        /// </summary>
        public double MinM
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the maximum x-position of the bounding box.
        /// </summary>       
        public double MaxX
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the maximum y-position of the bounding box.
        /// </summary>
        public double MaxY
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the maximum z-position of the bounding box.
        /// </summary>
        public double MaxZ
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the maximum m-position of the bounding polygon.
        /// </summary>
        public double MaxM
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("ShapeFileHeader: FileCode={0}, FileLength={1}, Version={2}, ShapeType={3}", this.FileCode, this.FileLength, this.Version, this.ShapeType);
            return sb.ToString();
        }
    }
}
