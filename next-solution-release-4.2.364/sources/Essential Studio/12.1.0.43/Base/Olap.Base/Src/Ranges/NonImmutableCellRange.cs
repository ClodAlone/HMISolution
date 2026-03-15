#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Syncfusion.Olap.Engine
{
     /// <summary>
    /// GridNonImmutableRangeInfo is derived from <see cref="GridRangeInfo"/> and lets
    /// you change Top, Left, Bottom, and Right properties of the object.
    /// </summary>
    [ImmutableObject(false)]
    [Serializable]
    [Syncfusion.Documentation.DocumentationExclude()]
    public sealed class GridNonImmutableRangeInfo: GridRangeInfo
    {
        /// <summary>
        /// Initializes a new <see cref="GridNonImmutableRangeInfo"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        GridNonImmutableRangeInfo(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {}

        /// <summary>
        ///     Creates a deep copy of the <see cref="GridNonImmutableRangeInfo"/> object.
        /// </summary>
        public override object Clone()
        {
            GridNonImmutableRangeInfo rg = new GridNonImmutableRangeInfo();
            this.CopyAllMembers(rg);
            return rg;
        }

        /// <summary>
        ///     Returns a new <see cref="GridRangeInfo"/> based on the <see cref="GridNonImmutableRangeInfo"/> object.
        /// </summary>
        public GridRangeInfo ToRangeInfo()
        {
            GridRangeInfo rg = new GridRangeInfo();
            this.CopyAllMembers(rg);
            return rg;
        }

        /// <summary>
        /// Initializes an empty instance of <see cref="GridNonImmutableRangeInfo"/>.
        /// </summary>
        public GridNonImmutableRangeInfo() 
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="GridNonImmutableRangeInfo"/> and copies data from an existing <see cref="GridRangeInfo"/>.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that holds data to be copied from.</param>
        public GridNonImmutableRangeInfo(GridRangeInfo range)
        {
            _top = range.Top;
            _bottom = range.Bottom;
            _left = range.Left;
            _right  = range.Right;
            _rangeType = range.RangeType;
        }


        /// <copyfrom cref="GridRangeInfo.RangeType"/>
        public new GridRangeInfoType RangeType
        {
            get
            {
                return base.RangeType;
            }
            set
            {
                _rangeType = value;
            }
        }

        /// <summary>
        /// The top row index of the range.
        /// </summary>
        public new int Top 
        {
            get 
            {
                return base.Top;
            }
            set
            {
                _top = value;
            }
        }

        /// <summary>
        /// The left column index of the range.
        /// </summary>
        public new int Left 
        {
            get 
            {
                return base.Left;
            }
            set
            {
                _left = value;
            }
        }


        /// <summary>
        /// The bottom row index of the range.
        /// </summary>
        public new int Bottom 
        {
            get
            {
                return base.Bottom;
            }
            set
            {
                _bottom = value;
            }
        }

        /// <summary>
        /// The right column index of the range.
        /// </summary>
        public new int Right 
        {
            get
            {
                return base.Right;
            }
            set
            {
                _right = value;
            }
        }

    }

    /// <summary>
    ///     A collection that stores 'GridNonImmutableRangeInfo' objects.
    /// </summary>
    [Serializable()]
    public class  GridNonImmutableRangeInfoCollection : System.Collections.CollectionBase 
    {
    
        /// <summary>
        ///     Initializes a new instance of 'GridNonImmutableRangeInfoCollection'.
        /// </summary>
        public GridNonImmutableRangeInfoCollection() 
        {
        }
    
        /// <summary>
        ///     Initializes a new instance of 'GridNonImmutableRangeInfoCollection' based on an already existing instance.
        /// </summary>
        /// <param name='covValue'>
        ///     A 'GridNonImmutableRangeInfoCollection' from which the contents is copied
        /// </param>
        public GridNonImmutableRangeInfoCollection(GridNonImmutableRangeInfoCollection covValue) 
        {
            this.AddRange(covValue);
        }
    
        /// <summary>
        ///     Initializes a new instance of 'GridNonImmutableRangeInfoCollection' with an array of 'GridNonImmutableRangeInfo' objects.
        /// </summary>
        /// <param name='covValue'>
        ///     An array of 'GridNonImmutableRangeInfo' objects with which to initialize the collection
        /// </param>
        public GridNonImmutableRangeInfoCollection(GridNonImmutableRangeInfo[] covValue) 
        {
            this.AddRange(covValue);
        }
    
        /// <summary>
        ///     Represents the 'GridNonImmutableRangeInfo' item at the specified index position.
        /// </summary>
        /// <param name='intIndex'>
        ///     The zero-based index of the entry to locate in the collection.
        /// </param>
        /// <value>
        ///     The entry at the specified index of the collection.
        /// </value>
        public GridNonImmutableRangeInfo this[int intIndex] 
        {
            get 
            {
                return ((GridNonImmutableRangeInfo)(List[intIndex]));
            }
            set 
            {
                List[intIndex] = value;
            }
        }
    
        /// <summary>
        ///     Adds a 'GridNonImmutableRangeInfo' item with the specified value to the 'GridNonImmutableRangeInfoCollection'
        /// </summary>
        /// <param name='covValue'>
        ///     The 'GridNonImmutableRangeInfo' to add.
        /// </param>
        /// <returns>
        ///     The index at which the new element was inserted.
        /// </returns>
        public int Add(GridNonImmutableRangeInfo covValue) 
        {
            return List.Add(covValue);
        }
    
        /// <summary>
        ///     Copies the elements of an array at the end of this instance of 'GridNonImmutableRangeInfoCollection'.
        /// </summary>
        /// <param name='covValue'>
        ///     An array of 'GridNonImmutableRangeInfo' objects to add to the collection.
        /// </param>
        public void AddRange(GridNonImmutableRangeInfo[] covValue) 
        {
            for (int intCounter = 0; (intCounter < covValue.Length); intCounter = (intCounter + 1)) 
            {
                this.Add(covValue[intCounter]);
            }
        }
    
        /// <summary>
        ///     Adds the contents of another 'GridNonImmutableRangeInfoCollection' at the end of this instance.
        /// </summary>
        /// <param name='covValue'>
        ///     A 'GridNonImmutableRangeInfoCollection' containing the objects to add to the collection.
        /// </param>
        public void AddRange(GridNonImmutableRangeInfoCollection covValue) 
        {
            for (int intCounter = 0; (intCounter < covValue.Count); intCounter = (intCounter + 1)) 
            {
                this.Add(covValue[intCounter]);
            }
        }
    
        /// <summary>
        ///     Gets a value indicating whether the 'GridNonImmutableRangeInfoCollection' contains the specified value.
        /// </summary>
        /// <param name='covValue'>
        ///     The item to locate.
        /// </param>
        /// <returns>
        ///     True if the item exists in the collection; false otherwise.
        /// </returns>
        public bool Contains(GridNonImmutableRangeInfo covValue) 
        {
            return List.Contains(covValue);
        }
    
        /// <summary>
        ///     Copies the 'GridNonImmutableRangeInfoCollection' values to a one-dimensional System.Array
        ///     instance starting at the specified array index.
        /// </summary>
        /// <param name='covArray'>
        ///     The one-dimensional System.Array that represents the copy destination.
        /// </param>
        /// <param name='intIndex'>
        ///     The index in the array where copying begins.
        /// </param>
        public void CopyTo(GridNonImmutableRangeInfo[] covArray, int intIndex) 
        {
            List.CopyTo(covArray, intIndex);
        }
    
        /// <summary>
        ///     Returns the index of a 'GridNonImmutableRangeInfo' object in the collection.
        /// </summary>
        /// <param name='covValue'>
        ///     The 'GridNonImmutableRangeInfo' object whose index will be retrieved.
        /// </param>
        /// <returns>
        ///     If found, the index of the value; otherwise, -1.
        /// </returns>
        public int IndexOf(GridNonImmutableRangeInfo covValue) 
        {
            return List.IndexOf(covValue);
        }
    
        /// <summary>
        ///     Inserts an existing 'GridNonImmutableRangeInfo' into the collection at the specified index.
        /// </summary>
        /// <param name='intIndex'>
        ///     The zero-based index where the new item should be inserted.
        /// </param>
        /// <param name='covValue'>
        ///     The item to insert.
        /// </param>
        public void Insert(int intIndex, GridNonImmutableRangeInfo covValue) 
        {
            List.Insert(intIndex, covValue);
        }
    
        /// <summary>
        ///     Returns an enumerator that can be used to iterate through
        ///     the 'GridNonImmutableRangeInfoCollection'.
        /// </summary>
        public new GridNonImmutableRangeInfoEnumerator GetEnumerator() 
        {
            return new GridNonImmutableRangeInfoEnumerator(this);
        }
    
        /// <summary>
        ///     Removes a specific item from the 'GridNonImmutableRangeInfoCollection'.
        /// </summary>
        /// <param name='covValue'>
        ///     The item to remove from the 'GridNonImmutableRangeInfoCollection'.
        /// </param>
        public void Remove(GridNonImmutableRangeInfo covValue) 
        {
            List.Remove(covValue);
        }
    
        /// <summary>
        ///     TODO: Describe what custom processing this method does
        ///     before setting an item in the collection
        /// </summary>
        protected override void OnSet(int intIndex, object objOldValue, object objNewValue) 
        {
            //  TODO: Add code here to handle an existing value within
            //  the collection be replaced with a new value
        }
    
        /// <summary>
        ///     TODO: Describe what custom processing this method does
        ///     before inserting a new item in the collection
        /// </summary>
        protected override void OnInsert(int intIndex, object objValue) 
        {
            //  TODO: Add code here to handle inserting a new item into the collection
        }
    
        /// <summary>
        ///     A strongly typed enumerator for 'GridNonImmutableRangeInfoCollection'
        /// </summary>
        public class  GridNonImmutableRangeInfoEnumerator : object, System.Collections.IEnumerator 
        {
        
            private System.Collections.IEnumerator iEnBase;
        
            private System.Collections.IEnumerable iEnLocal;
        
            /// <summary>
            ///     Enumerator constructor
            /// </summary>
            public GridNonImmutableRangeInfoEnumerator(GridNonImmutableRangeInfoCollection covMappings) 
            {
                this.iEnLocal = ((System.Collections.IEnumerable)(covMappings));
                this.iEnBase = iEnLocal.GetEnumerator();
            }
        
            /// <summary>
            ///     Gets the current element from the collection (strongly typed)
            /// </summary>
            public GridNonImmutableRangeInfo Current 
            {
                get 
                {
                    return ((GridNonImmutableRangeInfo)(iEnBase.Current));
                }
            }
        
            /// <summary>
            ///     Gets the current element from the collection
            /// </summary>
            object System.Collections.IEnumerator.Current 
            {
                get 
                {
                    return iEnBase.Current;
                }
            }
        
            /// <summary>
            ///     Advances the enumerator to the next element of the collection
            /// </summary>
            public bool MoveNext() 
            {
                return iEnBase.MoveNext();
            }
        
            /// <summary>
            ///     Advances the enumerator to the next element of the collection
            /// </summary>
            bool System.Collections.IEnumerator.MoveNext() 
            {
                return iEnBase.MoveNext();
            }
        
            /// <summary>
            ///     Sets the enumerator to the first element in the collection
            /// </summary>
            public void Reset() 
            {
                iEnBase.Reset();
            }
        
            /// <summary>
            ///     Sets the enumerator to the first element in the collection
            /// </summary>
            void System.Collections.IEnumerator.Reset() 
            {
                iEnBase.Reset();
            }
        }
    }

}
