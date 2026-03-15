#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Represent class with setting of metafile to parse.
    /// </summary>
    internal class MetafileSettings
    {
        #region Fields
        /// <summary>
        /// Internal variable to store the list of the records within metafile.
        /// </summary>
        private ArrayList m_records = new ArrayList();

        /// <summary>
        /// Internal variable to store the count of the records within metafile.
        /// </summary>
        private int m_count;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the list of EMF and EMF+ records within metafile. 
        /// </summary>
        public ArrayList Records
        {
            get
            {
                return m_records;
            }
        }

        /// <summary>
        /// Gets or sets the count of records.
        /// </summary>
        public int Count
        {
            get
            {
                return m_count;
            }
            set
            {
                m_count = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MetafileSettings"/> class.
        /// </summary>
        public MetafileSettings()
        { 
        }
        #endregion
    }
}
