#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    ///     <para>
    ///       A collection that stores <see cref='FieldDefinition'/> objects.
    ///    </para>
    /// </summary>
    /// <seealso cref='FieldDefinitionCollection'/>
    [Serializable()]
    public class FieldDefinitionCollection : CollectionBase
    {
        #region CollectionBase Members

        /// <summary>
        /// Initializes a new instance of the <see cref='FieldDefinitionCollection'/> class.
        /// </summary>
        public FieldDefinitionCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='FieldDefinitionCollection'/> class.
        /// </summary>
        /// <param name='value'>
        /// A <see cref='FieldDefinitionCollection'/> from which the contents are copied
        /// </param>
        public FieldDefinitionCollection(FieldDefinitionCollection value)
        {
            this.AddRange(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='FieldDefinitionCollection'/> class.
        /// </summary>
        /// <param name='value'>An array of <see cref='FieldDefinition'/> objects with which to intialize the collection</param>
        public FieldDefinitionCollection(FieldDefinition[] value)
        {
            this.AddRange(value);
        }

        /// <summary>
        /// <para>Gets / sets the field definition at the specified index of the <see cref='FieldDefinition'/>.</para>
        /// </summary>
        /// <param name='index'><para>The zero-based index of the entry to locate in the collection.</para></param>
        /// <value>
        ///    <para> The entry at the specified index of the collection.</para>
        /// </value>
        /// <exception cref='System.ArgumentOutOfRangeException'><paramref name='index'/> is outside the valid range of indexes for the collection.</exception>
        public FieldDefinition this[int index]
        {
            get
            {
                return (FieldDefinition)List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        /// <summary>
        ///    <para>Adds a <see cref='FieldDefinition'/> with the specified value to the 
        ///    <see cref='FieldDefinitionCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='FieldDefinition'/> to add.</param>
        /// <returns>
        ///    <para>The index at which the new element was inserted.</para>
        /// </returns>
        /// <seealso cref='FieldDefinitionCollection.AddRange'/>
        public int Add(FieldDefinition value)
        {
            return List.Add(value);
        }

        /// <summary>
        /// <para>Copies the elements of an array to the end of the <see cref='FieldDefinitionCollection'/>.</para>
        /// </summary>
        /// <param name='value'>
        ///    An array of type <see cref='FieldDefinition'/> containing the objects to add to the collection.
        /// </param>
        /// <seealso cref='FieldDefinitionCollection.Add'/>
        public void AddRange(FieldDefinition[] value)
        {
            for (int i = 0; i < value.Length; i = i + 1)
            {
                this.Add(value[i]);
            }
        }

        /// <summary>
        ///     <para>
        ///       Adds the contents of another <see cref='FieldDefinitionCollection'/> to the end of the collection.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///    A <see cref='FieldDefinitionCollection'/> containing the objects to add to the collection.
        /// </param>
        /// <seealso cref='FieldDefinitionCollection.Add'/>
        public void AddRange(FieldDefinitionCollection value)
        {
            for (int i = 0; i < value.Count; i = i + 1)
            {
                this.Add(value[i]);
            }
        }

        /// <summary>
        /// <para>Indicates whether the <see cref='FieldDefinitionCollection'/> 
        /// contains the specified <see cref='FieldDefinition'/>.</para>
        /// </summary>
        /// <param name='value'>The <see cref='FieldDefinition'/> to locate.</param>
        /// <returns>
        /// <para><see langword='true'/> if the <see cref='FieldDefinition'/> is contained in the collection; 
        /// <see langword='false'/> otherwise.</para>
        /// </returns>
        /// <seealso cref='FieldDefinitionCollection.IndexOf'/>
        public bool Contains(FieldDefinition value)
        {
            return List.Contains(value);
        }

        /// <summary>
        /// <para>Copies the <see cref='FieldDefinitionCollection'/> values to a one-dimensional <see cref='System.Array'/> instance at the 
        ///    specified index.</para>
        /// </summary>
        /// <param name='array'><para>The one-dimensional <see cref='System.Array'/> that is the destination of the values copied from <see cref='FieldDefinitionCollection'/> .</para></param>
        /// <param name='index'>The index in <paramref name='array'/> where copying begins.</param>
        /// <exception cref='System.ArgumentException'><para><paramref name='array'/> is multidimensional.</para> <para>-or-</para> 
        /// <para>The number of elements in the <see cref='FieldDefinitionCollection'/> is greater than the available space between <paramref name='arrayIndex'/> and the end of <paramref name='array'/>.</para></exception>
        /// <exception cref='System.ArgumentNullException'><paramref name='array'/> is <see langword='null'/>. </exception>
        /// <exception cref='System.ArgumentOutOfRangeException'><paramref name='arrayIndex'/> is less than <paramref name='array'/>'s lowbound. </exception>
        /// <seealso cref='System.Array'/>
        public void CopyTo(FieldDefinition[] array, int index)
        {
            List.CopyTo(array, index);
        }

        /// <summary>
        ///    <para>Returns the index of a <see cref='FieldDefinition'/> in 
        ///       the <see cref='FieldDefinitionCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='FieldDefinition'/> to locate.</param>
        /// <returns>
        /// <para>The index of the <see cref='FieldDefinition'/> of <paramref name='value'/> in the 
        /// <see cref='FieldDefinitionCollection'/>, if found; -1 otherwise.</para>
        /// </returns>
        /// <seealso cref='FieldDefinitionCollection.Contains'/>
        public int IndexOf(FieldDefinition value)
        {
            return List.IndexOf(value);
        }

        /// <summary>
        /// Inserts a <see cref='FieldDefinition'/> into the <see cref='FieldDefinitionCollection'/> at the specified index.
        /// </summary>
        /// <param name='index'>The zero-based index where <paramref name='value'/> should be inserted.</param>
        /// <param name='value'>The <see cref='FieldDefinition'/> to insert.</param>
        public void Insert(int index, FieldDefinition value)
        {
            List.Insert(index, value);
        }

        /// <summary>
        /// <para>Returns an enumerator that can iterate through 
        /// the <see cref='FieldDefinitionCollection'/> .</para>
        /// </summary>
        /// <returns><para>Returns FieldDefenition</para></returns>
        /// <seealso cref='System.Collections.IEnumerator'/>
        public new FieldDefinitionEnumerator GetEnumerator()
        {
            return new FieldDefinitionEnumerator(this);
        }

        /// <summary>
        /// <para> Removes a specific <see cref='FieldDefinition'/> from the 
        /// <see cref='FieldDefinitionCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='FieldDefinition'/> to remove from the <see cref='FieldDefinitionCollection'/> .</param>
        /// <exception cref='System.ArgumentException'><paramref name='value'/> is not found in the Collection. </exception>
        public void Remove(FieldDefinition value)
        {
            List.Remove(value);
        }

        #endregion

        #region IEnumerator Members
        public class FieldDefinitionEnumerator : object, IEnumerator
        {
            private IEnumerator baseEnumerator;

            private IEnumerable temp;

            public FieldDefinitionEnumerator(FieldDefinitionCollection mappings)
            {
                this.temp = (IEnumerable)mappings;
                this.baseEnumerator = temp.GetEnumerator();
            }

            public FieldDefinition Current
            {
                get
                {
                    return (FieldDefinition)baseEnumerator.Current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return baseEnumerator.Current;
                }
            }

            public bool MoveNext()
            {
                return baseEnumerator.MoveNext();
            }

            bool IEnumerator.MoveNext()
            {
                return baseEnumerator.MoveNext();
            }

            public void Reset()
            {
                baseEnumerator.Reset();
            }

            void IEnumerator.Reset()
            {
                baseEnumerator.Reset();
            }
        }
        #endregion
    }
}
