#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// A collection of <see cref="Tuple"/>.
    /// </summary>
    [CollectionDataContract]
    public class TupleCollection : Collection<Tuple>
    {
        #region Private Variables
        Axis _parentAxis;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TupleCollection"/> class.
        /// </summary>
        public TupleCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TupleCollection"/> class.
        /// </summary>
        /// <param name="parentAxis">The parent axis.</param>
        public TupleCollection(Axis parentAxis)
        {
            _parentAxis = parentAxis;         
        }
               
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.OlapSilverlight.Data.Tuple"/> at the specified index.
        /// </summary>
        /// <value></value>
        [Description("Gets tuple object by index.")]
        public Tuple this[int index]
        {
            get
            {
                return base.Items[index] as Tuple;
            }

            set
            {
                base.Items[index] = value;
            }
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="tuple">The tuple.</param>
        public void Insert(int index, Tuple tuple)
        {
            this.Insert(index, tuple);
        }

        /// <summary>
        /// Removes the specified tuple.
        /// </summary>
        /// <param name="tuple">The tuple.</param>
        public void Remove(Tuple tuple)
        {
            this.Remove(tuple);
        }
        #endregion   
    }
}
