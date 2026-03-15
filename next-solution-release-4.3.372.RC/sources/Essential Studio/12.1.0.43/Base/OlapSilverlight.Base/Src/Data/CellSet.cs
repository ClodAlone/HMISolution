#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Runtime.Serialization;


namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// Represents the CellSet information.
    /// </summary>
    [KnownType(typeof(AxisCollection))]
    [KnownType(typeof(CellCollection))]
    [DataContract]
    public class CellSet
    {
        #region Private Variables
        AxisCollection axisCollection;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OLAPData"/> class.
        /// </summary>
        public CellSet()
        {
            this.axisCollection = new AxisCollection(this);
        }
        #endregion
        /// <summary>
        /// Gets or sets the axes.
        /// </summary>
        /// <value>The axes.</value>
        [DataMember]
        public AxisCollection Axes
        {
            get
            {
                return this.axisCollection;
            }
            set
            {
                this.axisCollection=value ;
            }
        }

        /// <summary>
        /// Gets or sets the column max level.
        /// </summary>
        /// <value>The column max level.</value>
        [DataMember]
        public int ColumnMaxLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the row max level.
        /// </summary>
        /// <value>The row max level.</value>
        [DataMember]
        public int RowMaxLevel
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the cell collection.
        /// </summary>
        /// <value>The cell collection.</value>
        [DataMember]
        public CellCollection CellCollection
        {
            get;
            set;
        }
    }
}
