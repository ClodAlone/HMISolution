#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.UI.Xaml.Maps
{
    using System;
    using System.Windows;
    using System.Collections;
    using System.Collections.Generic;
    /// <summary>
    /// Represents the ShapeFileDBFData class in the map.
    /// </summary>
     internal class ShapeFileDBFData
    {
        #region PrivateFields

        private ShapeFileDBFHeader m_dbfHeader;
        private List<DBFValues> m_dbfFields;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DBFReader.ShapeFileDBFData">ShapeFileDBFData</see> class. 
        /// </summary>
         public ShapeFileDBFData()
        {
        }

        #region Properties

        /// <summary>
        /// Gets  the dbf data file header. .
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public ShapeFileDBFHeader DBFHeader
        {
            get
            {
                return this.m_dbfHeader;
            }
           internal set
            {
                this.m_dbfHeader = value;
            }
        }


        /// <summary>
        /// Gets or sets .
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public List<DBFValues> DBFFields
        {
            get
            {
                return this.m_dbfFields;
            }
            set
            {
                this.m_dbfFields = value;
            }

        }


        #endregion
    }
}
