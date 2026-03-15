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
    ///  ShapeFileData holds the Information of ShapeFile  like Filename, Records etc.
    /// </summary>
    public class ShapeFileData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Maps.IO.ShapeFileData"/> class.
        /// </summary>
        /// <param name="fileName">Name of the ShapeFile</param>
        public ShapeFileData(string fileName)
        {
            this.Records = new List<ShapeFileRecord>();
        }

        /// <summary>
        /// Gets or sets Name of the shapefile.
        /// </summary>
        public string FileName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets Header of the shape file.
        /// </summary>
        /// <value>
        /// ShapeFileHeader
        /// </value>
        public ShapeFileHeader FileHeader
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets list of records in the shapefile.
        /// </summary>
        public List<ShapeFileRecord> Records
        {
            get;
            internal set;
        }
    }
}
