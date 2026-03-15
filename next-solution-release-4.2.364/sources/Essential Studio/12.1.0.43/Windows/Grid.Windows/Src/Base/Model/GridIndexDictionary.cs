//-------------------------------------------------------------------------------------------------
// <copyright file="GridIndexDictionary.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Diagnostics;
using Syncfusion.Collections;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Provides a collection with index / value pairs stored in a <see cref="Hashtable"/> and
    /// lets you insert, remove, or move values in the collection and indexes will be updated.
    /// </summary>
    /// <remarks>
    /// <see cref="GridModelHideRowColsIndexer"/> and <see cref="GridModelRowColSizeIndexer"/>
    /// store row, column sizes, and hidden states in a <see cref="GridIndexDictionary"/>.
    /// </remarks>
    [Serializable]
    public class GridIndexDictionary : TypedDictionaryBase, ICloneable, ISerializable
    {
        // base class of GridRowColSizeDictionary and GridRowColHideDictionary
        int smallestKey = -1;
        int largestKey = -1;
        internal Type memberType;

        /// <internalonly/>
        /// <summary>Gets the MemberType. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public Type MemberType
        {
            get
            {
                return memberType;
            }
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>returns Hashtable.</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public Hashtable GetHashTable()
        {
            return this.InnerHashtable;
        }

        // Constructors

        /// <overload>
        /// Initializes a new <see cref="GridIndexDictionary"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridIndexDictionary"/> and specifies the type of values this collection holds.
        /// </summary>
        /// <param name="memberType">The type of values this collection holds.</param>
        public GridIndexDictionary(Type memberType)
        {
            this.memberType = memberType;
        }

        /// <summary>
        /// Initializes a new <see cref="GridIndexDictionary"/> and specifies the hashtable and type of values this collection holds.
        /// </summary>
        /// <param name="hashtable">The inner hashtable to be associated with this <see cref="GridIndexDictionary"/>.</param>
        /// <param name="memberType">The type of values this collection holds.</param>
        protected GridIndexDictionary(Hashtable hashtable, Type memberType)
        {
            this.InnerHashtable = hashtable;
            this.memberType = memberType;
        }

        /// <summary>
        /// Initializes a new <see cref="GridIndexDictionary"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridIndexDictionary(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            smallestKey = info.GetInt32("SmallestKey");
            largestKey = info.GetInt32("LargestKey");
            object typeObj = info.GetValue("MemberType", typeof(object)); ////, typeof(Type));
            if (typeObj is string)
            {
                memberType = Syncfusion.Styles.ValueConvert.GetType((string)typeObj);
            }
            else
            {
                memberType = (Type)typeObj;
            }

            int count = info.GetInt32("Count");

            Hashtable ht = new Hashtable(count);

            SerializationInfoEnumerator sie = info.GetEnumerator();
            while (sie.MoveNext())
            {
                if (Char.IsDigit(sie.Name[0]))
                {
                    ht[Convert.ToInt32(sie.Name)] = Convert.ChangeType(sie.Value, memberType);
                }
            }

            InnerHashtable = ht;
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridIndexDictionary"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            Hashtable ht = this.InnerHashtable;
            info.AddValue("SmallestKey", smallestKey); // Int32
            info.AddValue("LargestKey", largestKey); // Int32
            info.AddValue("Count", ht.Count); // Int32
            ////info.AddValue("MemberType", memberType); // Type
            info.AddValue("MemberType", Syncfusion.Styles.ValueConvert.GetTypeName(memberType)); // Type

            foreach (object key in ht.Keys)
            {
                info.AddValue(key.ToString(), ht[key]);
            }

            //// Don't call base.GetObjectData(info, context);
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        /// <summary>
        /// Creates an exact copy of this <see cref="GridIndexDictionary"/>.
        /// </summary>
        /// <returns>A <see cref="GridIndexDictionary"/> with all values copied over.</returns>
        public GridIndexDictionary Clone()
        {
            return new GridIndexDictionary((Hashtable)InnerHashtable.Clone(), memberType);
        }

        // Methods

        /// <summary>
        /// Adds an index / value pair.
        /// </summary>
        /// <param name="key">The index for the value.</param>
        /// <param name="value">The value.</param>
        public void /*IDictionary*/ Add(int key, object value)
        {
            this.Dictionary.Add(key, value);
        }

        /// <summary>
        /// Checks if the collection has an entry at the specified index.
        /// </summary>
        /// <param name="key">The index for the value.</param>
        /// <returns>True if a value exists at the index; False otherwise.</returns>
        public bool /*IDictionary*/ Contains(int key)
        {
            return this.Dictionary.Contains(key);
        }

        /// <summary>
        /// The value at the specified index.
        /// </summary>
        public object /*IDictionary*/ this[int key]
        {
            get
            {
                if (this.Dictionary.Contains(key))
                {
                    return this.Dictionary[key];
                }

                return null;
            }

            set
            {
                this.Dictionary[key] = value;
            }
        }

        /// <summary>
        ///   <para>Copies the values to a one-dimensional <see cref="System.Array" /> instance at the
        /// specified index.</para>
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="System.Array" /> that is the destination of the values copied from the <see cref="GridCellModelCollection" />.</param>
        /// <param name="index">The index in the array where copying begins.</param>
        public void CopyTo(object[] array, int index)
        {
            base.CopyTo(array, index);
        }

        /// <summary>
        /// Removes the value at the specified index.
        /// </summary>
        /// <param name="key">The Index.</param>
        public void Remove(int key)
        {
            this.Dictionary.Remove(key);
        }

        /// <summary>
        /// Inserts a specified number of values at a specific index and adjusts the index 
        /// for subsequent values.
        /// </summary>
        /// <param name="index">The index where to start inserting values.</param>
        /// <param name="count">The number of values to insert.</param>
        public void InsertIndex(int index, int count)
        {
            Hashtable newHashtable = new Hashtable(this.Count);
            smallestKey = largestKey = -1;

            foreach (DictionaryEntry entry in Dictionary)
            {
                int key = (int)entry.Key;
                if (key < index)
                {
                    newHashtable[key] = entry.Value;
                    CheckMinMax(key);
                }
                else
                {
                    newHashtable[key + count] = entry.Value;
                    CheckMinMax(key + count);
                }
            }

            this.InnerHashtable = newHashtable;
        }

        /// <summary>
        /// Removes a specified number of values at a specific index and adjusts the index 
        /// for subsequent values.
        /// </summary>
        /// <param name="from">The index where to start removing values.</param>
        /// <param name="count">The number of values to remove.</param>
        public void RemoveIndex(int from, int count)
        {
            int to = from + count - 1;
            smallestKey = largestKey = -1;

            Hashtable newHashtable = new Hashtable(this.Count);

            foreach (DictionaryEntry entry in Dictionary)
            {
                int key = (int)entry.Key;
                if (key < from)
                {
                    newHashtable[key] = entry.Value;
                    CheckMinMax(key);
                }
                else if (key > to)
                {
                    newHashtable[key - count] = entry.Value;
                    CheckMinMax(key - count);
                }
            }

            this.InnerHashtable = newHashtable;
        }

        /// <summary>
        /// Moves a specified number of values to a new index and adjusts the indexes 
        /// of subsequent values.
        /// </summary>
        /// <param name="from">The index where to start moving values.</param>
        /// <param name="count">The number of values to move.</param>
        /// <param name="dest">The destination index.</param>
        public void MoveIndex(int from, int count, int dest)
        {
            int to = from + count - 1;
            smallestKey = largestKey = -1;

            Hashtable newHashtable = new Hashtable(this.Count);

            foreach (DictionaryEntry entry in Dictionary)
            {
                int key = (int)entry.Key;
                int intKey = -1;

                if ((key < dest && key < from)
                    || (key >= dest + count && key > to))
                {
                    intKey = key;
                }
                else if (key >= from && key <= to)
                {
                    intKey = key - from + dest;
                }
                else if (key >= dest && key < from)
                {
                    intKey = key + count;
                }
                else if (key < dest + count && key > to)
                {
                    intKey = key - count;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }

                newHashtable[intKey] = entry.Value;
                CheckMinMax(intKey);
            }

            this.InnerHashtable = newHashtable;
        }

        void CheckMinMax()
        {
            smallestKey = largestKey = -1;
            foreach (DictionaryEntry entry in Dictionary)
            {
                CheckMinMax((int)entry.Key);
            }
        }

        void CheckMinMax(int key)
        {
            if (key > largestKey)
            {
                largestKey = key;
            }

            if (key < smallestKey || smallestKey == -1)
            {
                smallestKey = key;
            }
        }

        /*internal void CopyOrderedIndexTo(Array array, int destIndex, int from, int count)
        {
            for (int i = 0; i < count; i++)
                array[destIndex+i] = this[from+i];
        }

        internal void SetOrderedIndex(Array array, int arrayIndex, int dest, int count)
        {
            for (int i = 0; i < count; i++)
                this[dest+i] = array[arrayIndex+i];
        }*/

        /// <override/>
        protected override void OnInsert(object key, object value)
        {
            CheckMinMax((int)key);
        }

        /// <override/>
        protected override void OnRemove(object key, object value)
        {
            int intKey = (int)key;
            if (intKey == largestKey || intKey == smallestKey)
            {
                CheckMinMax();
            }
        }

        /// <override/>
        protected override void OnSet(object key, object oldValue, object newValue)
        {
        }

        /// <override/>
        protected override void OnValidate(object key, object value)
        {
            if ((int)key < 0)
            {
                throw new ArgumentException("negative key");
            }
        }

        /// <summary>
        /// Gets the smallest index in the collection.
        /// </summary>
        public int SmallestKey
        {
            get
            {
                return smallestKey;
            }
        }

        /// <summary>
        /// Gets the largest index in the collection.
        /// </summary>
        public int LargestKey
        {
            get
            {
                return largestKey;
            }
        }

        /// <overload>
        /// Checks if a value is stored at a specific index and returns its value.
        /// </overload>
        /// <summary>
        /// Checks if a <see cref="System.Int32"/> value is stored at a specific index and returns its value.
        /// </summary>
        /// <param name="key">The index to look up.</param>
        /// <param name="result">The value at the specified index.</param>
        /// <returns>True if value exists; False otherwise.</returns>
        public bool Lookup(int key, out int result)
        {
            if (this.Contains(key))
            {
                object obj = this[key];
                if (obj is int)
                {
                    result = (int)obj;
                    return true;
                }
            }

            result = 0;
            return false;
        }

        /// <summary>
        /// Checks if a <see cref="System.Single"/> value is stored at a specific index and returns its value.
        /// </summary>
        /// <param name="key">The index to look up.</param>
        /// <param name="result">The value at the specified index.</param>
        /// <returns>True if value exists; False otherwise.</returns>
        public bool Lookup(int key, out float result)
        {
            if (this.Contains(key))
            {
                object obj = this[key];
                if (obj is float)
                {
                    result = (float)obj;
                    return true;
                }
            }

            result = 0f;
            return false;
        }

        /// <summary>
        /// Checks if a <see cref="System.Object"/> value is stored at a specific index and returns its value.
        /// </summary>
        /// <param name="key">The index to look up.</param>
        /// <param name="obj">The value at the specified index.</param>
        /// <returns>True if value exists; False otherwise.</returns>
        public bool Lookup(int key, out object obj)
        {
            if (this.Contains(key))
            {
                obj = this[key];
                return true;
            }

            obj = null;
            return false;
        }
    }
}