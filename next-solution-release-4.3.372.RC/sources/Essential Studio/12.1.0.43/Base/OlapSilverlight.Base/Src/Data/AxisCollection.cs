#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// A collection of <see cref="Axis"/>.
    /// </summary>
    [CollectionDataContract]
    public class AxisCollection : Collection<Axis>
    {
        #region Private Variables
        CellSet _parentCellset;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AxisCollection"/> class.
        /// </summary>
        public AxisCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AxisCollection"/> class.
        /// </summary>
        /// <param name="parentCellset">The parent <see cref="CellSet"/>.</param>
        public AxisCollection(CellSet parentCellset)
        {
            this._parentCellset = parentCellset;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Data.Axis"/> at the specified index.
        /// </summary>
        /// <value></value>
        [Description("Axis collection indexer.")]
        public Axis this[int index]
        {
            get
            {
                return base.Items[index] as Axis;
            }

            set
            {
                base.Items[index] = value;
            }
        }

        /// <summary>
        /// Insets the specified index.
        /// </summary>
        /// <param name="index">insertion index of the Axis</param>
        /// <param name="axis">Column or Row axis</param>
        public void Insert(int index, Axis axis)
        {
            this.Insert(index, axis);
        }

        /// <summary>
        /// Removes the specified axis.
        /// </summary>
        /// <param name="axis">Column or Row axis</param>
        public void Remove(Axis axis)
        {
            this.Remove(axis);
        }
        #endregion

    }
}
