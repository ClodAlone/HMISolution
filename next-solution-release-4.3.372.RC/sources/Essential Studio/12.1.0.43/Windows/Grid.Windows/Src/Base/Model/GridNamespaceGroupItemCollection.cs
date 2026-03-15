//-------------------------------------------------------------------------------------------------
// <copyright file="GridNamespaceGroupItemCollection.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Xml.Serialization;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    ///     Used for Xml serialization, this struct contains information relating to shared expensive properties that could be
    ///     the same in various places in the grid. For example, the BackgroundImage for cells, or the grid itself. Instead of 
    ///     having multiple listings of the same expensive object, a <see cref="GridNamespaceGroupItem"/> object will contain the 
    ///     Value of the item (could be a Base64 string) and a Name, which the cells could refer to. 
    /// </summary>
    /// <remarks>
    ///     <see cref="Design.GridSyncProperties.StoredImages"/> and <see cref="Design.GridSyncProperties.StoredFonts"/> are a sample use of this.
    /// </remarks>
    [Serializable]
    public struct GridNamespaceGroupItem
    {
        /// <summary>
        ///     Constructs a new <see cref="GridNamespaceGroupItem"/> with the provided name and value.
        /// </summary>
        public GridNamespaceGroupItem(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        ///     Name of the <see cref="GridNamespaceGroupItem"/> object
        /// </summary>
        [XmlAttribute("Name")]
        public string Name;

        /// <summary>
        ///     The string representation of the objects value that is stored in the <see cref="GridNamespaceGroupItem"/> object. 
        /// </summary>
        /// <remarks>
        /// <para/>    
        /// </remarks>
        public string Value;
    }

    #region "'GridNamespaceGroupItemCollection' strongly typed collection class"
    
    /// <summary>
    ///     A collection that stores 'GridNamespaceGroupItem' objects.
    /// </summary>
    [Serializable()]
    public class GridNamespaceGroupItemCollection : System.Collections.CollectionBase
    {
        /// <summary>
        ///     Initializes a new instance of 'GridNamespaceGroupItemCollection'.
        /// </summary>
        public GridNamespaceGroupItemCollection()
        {
        }

        /// <summary>
        ///     Initializes a new instance of 'GridNamespaceGroupItemCollection' based on an already existing instance.
        /// </summary>
        /// <param name='styValue'>
        ///     A 'GridNamespaceGroupItemCollection' from which the contents is copied
        /// </param>
        public GridNamespaceGroupItemCollection(GridNamespaceGroupItemCollection styValue)
        {
            this.AddRange(styValue);
        }

        /// <summary>
        ///     Initializes a new instance of 'GridNamespaceGroupItemCollection' with an array of 'GridNamespaceGroupItem' objects.
        /// </summary>
        /// <param name='styValue'>
        ///     An array of 'GridNamespaceGroupItem' objects with which to initialize the collection
        /// </param>
        public GridNamespaceGroupItemCollection(GridNamespaceGroupItem[] styValue)
        {
            this.AddRange(styValue);
        }

        /// <summary>
        ///     Represents the 'GridNamespaceGroupItem' item at the specified index position.
        /// </summary>
        /// <param name='intIndex'>
        ///     The zero-based index of the entry to locate in the collection.
        /// </param>
        /// <value>
        ///     The entry at the specified index of the collection.
        /// </value>
        public GridNamespaceGroupItem this[int intIndex]
        {
            get
            {
                return (GridNamespaceGroupItem)List[intIndex];
            }

            set
            {
                List[intIndex] = value;
            }
        }

        /// <summary>
        ///     Adds a 'GridNamespaceGroupItem' item with the specified value to the 'GridNamespaceGroupItemCollection'
        /// </summary>
        /// <param name='styValue'>
        ///     The 'GridNamespaceGroupItem' to add.
        /// </param>
        /// <returns>
        ///     The index at which the new element was inserted.
        /// </returns>
        public int Add(GridNamespaceGroupItem styValue)
        {
            return List.Add(styValue);
        }

        /// <summary>
        ///     Adds the base64 string value to the collection, returning the Name for the item. If the base64 string
        ///     already exists, the current Name for the item is returned.
        /// </summary>
        /// <param name="base64Str" type="string">
        ///     <para>
        ///         The base64 string to add
        ///     </para>
        /// </param>
        /// <returns>
        ///     The Name of the GridNamespaceGroupItem which contains this value
        /// </returns>
        public string Add(string base64Str)
        {
            string returnNameStr = string.Empty;
            returnNameStr = this.GetNameForValue(base64Str);
            if (returnNameStr == string.Empty)
            {
                returnNameStr = "I" + this.Count.ToString();
                this.Add(new GridNamespaceGroupItem(returnNameStr, base64Str));
            }

            return returnNameStr;
        }

        /// <summary>
        ///     Copies the elements of an array at the end of this instance of 'GridNamespaceGroupItemCollection'.
        /// </summary>
        /// <param name='styValue'>
        ///     An array of 'GridNamespaceGroupItem' objects to add to the collection.
        /// </param>
        public void AddRange(GridNamespaceGroupItem[] styValue)
        {
            for (int intCounter = 0; intCounter < styValue.Length; intCounter = intCounter + 1)
            {
                this.Add(styValue[intCounter]);
            }
        }

        /// <summary>
        ///     Adds the contents of another 'GridNamespaceGroupItemCollection' at the end of this instance.
        /// </summary>
        /// <param name='styValue'>
        ///     A 'GridNamespaceGroupItemCollection' containing the objects to add to the collection.
        /// </param>
        public void AddRange(GridNamespaceGroupItemCollection styValue)
        {
            for (int intCounter = 0; intCounter < styValue.Count; intCounter = intCounter + 1)
            {
                this.Add(styValue[intCounter]);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the 'GridNamespaceGroupItemCollection' contains the specified value.
        /// </summary>
        /// <param name='styValue'>
        ///     The item to locate.
        /// </param>
        /// <returns>
        ///     True if the item exists in the collection; false otherwise.
        /// </returns>
        public bool Contains(GridNamespaceGroupItem styValue)
        {
            return List.Contains(styValue);
        }

        /// <summary>
        ///     Gets a value indicating whether the 'GridNamespaceGroupItemCollection' contains a <see cref="GridNamespaceGroupItem"/> object
        ///     with the specified value.
        /// </summary>
        /// <param name='val'>
        ///     The value to find in the collection.
        /// </param>
        /// <returns>
        ///     True if the item exists in the collection; false otherwise.
        /// </returns>
        public bool ContainsValue(string val)
        {
            GridNamespaceGroupItemCollection.GridNamespaceGroupItemEnumerator ienum = this.GetEnumerator();
            while (ienum.MoveNext())
            {
                if (ienum.Current.Value == val)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Copies the 'GridNamespaceGroupItemCollection' values to a one-dimensional System.Array
        ///     instance starting at the specified array index.
        /// </summary>
        /// <param name='styArray'>
        ///     The one-dimensional System.Array that represents the copy destination.
        /// </param>
        /// <param name='intIndex'>
        ///     The index in the array where copying begins.
        /// </param>
        public void CopyTo(GridNamespaceGroupItem[] styArray, int intIndex)
        {
            List.CopyTo(styArray, intIndex);
        }

        /// <summary>
        ///     Returns the index of a 'GridNamespaceGroupItem' object in the collection.
        /// </summary>
        /// <param name='styValue'>
        ///     The 'GridNamespaceGroupItem' object whose index will be retrieved.
        /// </param>
        /// <returns>
        ///     If found, the index of the value; otherwise, -1.
        /// </returns>
        public int IndexOf(GridNamespaceGroupItem styValue)
        {
            return List.IndexOf(styValue);
        }

        /// <summary>
        ///     Retrieves the value of the <see cref="GridNamespaceGroupItem"/> object that has the supplied name.
        /// </summary>
        /// <param name="name" type="string">
        ///     <para>
        ///         The name of the <see cref="GridNamespaceGroupItem"/> object.
        ///     </para>
        /// </param>
        /// <returns>Current object's value for the given name.</returns>
        public string GetValueForName(string name)
        {
            GridNamespaceGroupItemCollection.GridNamespaceGroupItemEnumerator ienum = this.GetEnumerator();
            while (ienum.MoveNext())
            {
                if (ienum.Current.Name == name)
                {
                    return ienum.Current.Value;
                }
            }

            return string.Empty;
        }

        /// <summary>
        ///     Retrieves the name of the <see cref="GridNamespaceGroupItem"/> object that has the supplied value.
        /// </summary>
        /// <param name="val" type="string">
        ///     <para>
        ///         The value of the <see cref="GridNamespaceGroupItem"/> object in question.
        ///     </para>
        /// </param>
        /// <returns>Current object's name for the specified value. </returns>
        public string GetNameForValue(string val)
        {
            GridNamespaceGroupItemCollection.GridNamespaceGroupItemEnumerator ienum = this.GetEnumerator();
            while (ienum.MoveNext())
            {
                if (ienum.Current.Value == val)
                {
                    return ienum.Current.Name;
                }
            }

            return string.Empty;
        }

        /// <summary>
        ///     Inserts an existing 'GridNamespaceGroupItem' into the collection at the specified index.
        /// </summary>
        /// <param name='intIndex'>
        ///     The zero-based index where the new item should be inserted.
        /// </param>
        /// <param name='styValue'>
        ///     The item to insert.
        /// </param>
        public void Insert(int intIndex, GridNamespaceGroupItem styValue)
        {
            List.Insert(intIndex, styValue);
        }

        /// <summary>
        ///     Returns an enumerator that can be used to iterate through
        ///     the 'GridNamespaceGroupItemCollection'.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public new GridNamespaceGroupItemEnumerator GetEnumerator()
        {
            return new GridNamespaceGroupItemEnumerator(this);
        }

        /// <summary>
        ///     Removes a specific item from the 'GridNamespaceGroupItemCollection'.
        /// </summary>
        /// <param name='styValue'>
        ///     The item to remove from the 'GridNamespaceGroupItemCollection'.
        /// </param>
        public void Remove(GridNamespaceGroupItem styValue)
        {
            List.Remove(styValue);
        }

        /// <summary>
        ///     TODO: Describe what custom processing this method does
        ///     before setting an item in the collection
        /// </summary>
        protected override void OnSet(int intIndex, object objOldValue, object objNewValue)
        {
            ////  TODO: Add code here to handle an existing value within
            ////  the collection be replaced with a new value
        }

        /// <summary>
        ///     TODO: Describe what custom processing this method does
        ///     before inserting a new item in the collection
        /// </summary>
        protected override void OnInsert(int intIndex, object objValue)
        {
            ////  TODO: Add code here to handle inserting a new item into the collection
        }

        /// <summary>
        ///     A strongly typed enumerator for 'GridNamespaceGroupItemCollection'
        /// </summary>
        public class GridNamespaceGroupItemEnumerator : object, System.Collections.IEnumerator
        {
            private System.Collections.IEnumerator iEnBase;

            private System.Collections.IEnumerable iEnLocal;

            /// <summary>
            ///     Enumerator constructor
            /// </summary>
            /// <param name="styMappings">A collection of GridNamespaceGroupItem objects.</param>
            public GridNamespaceGroupItemEnumerator(GridNamespaceGroupItemCollection styMappings)
            {
                this.iEnLocal = (System.Collections.IEnumerable)styMappings;
                this.iEnBase = iEnLocal.GetEnumerator();
            }

            /// <summary>
            ///     Gets the current element from the collection (strongly typed)
            /// </summary>
            public GridNamespaceGroupItem Current
            {
                get
                {
                    return (GridNamespaceGroupItem)iEnBase.Current;
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
            /// <returns>True if next element exists.</returns>
            public bool MoveNext()
            {
                return iEnBase.MoveNext();
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
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

    #endregion //('GridNamespaceGroupItemCollection' strongly typed collection class)
}
