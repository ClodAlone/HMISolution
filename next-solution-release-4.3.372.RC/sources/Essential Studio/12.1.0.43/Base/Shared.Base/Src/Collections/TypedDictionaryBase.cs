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
using System.Collections;
using System.Runtime.Serialization;

namespace Syncfusion.Collections
{
	/// <summary>
	/// Provides the <see langword="abstract" /> base class for a strongly typed collection of key-and-value pairs.
	/// </summary>
	/// <remarks>
	/// This class is very similar to the <see cref="System.Collections.DictionaryBase" />
	/// class. The main difference is that this version allows you to set the InnerHashtable.
	/// <see cref="System.Collections.DictionaryBase" /> only provides a Read-only property for the
	/// InnerHashtable.</remarks>
    public abstract class TypedDictionaryBase: IDictionary
    {
        // Fields
        private Hashtable hashtable;

        // Constructors
		/// <overload>
		/// Initializes a new <see cref="TypedDictionaryBase"/>.
		/// </overload>
		/// <summary>
		/// Initializes a new <see cref="TypedDictionaryBase"/>.
		/// </summary>
		protected TypedDictionaryBase()  
        {
        }

		/// <summary>
		/// Initializes a new <see cref="TypedDictionaryBase"/> and associates it with a <see cref="Hashtable"/>.
		/// </summary>
		/// <param name="hashtable">The inner collection that will hold entries of this collection.</param>
		protected TypedDictionaryBase(Hashtable hashtable)  
        {
            this.hashtable = hashtable;
        }

        // Methods
		/// <summary>
		///   <para>Clears the contents of the <see cref="TypedDictionaryBase" /> instance.</para>
		/// </summary>
		public /*IDictionary*/ void Clear()  
        {
            this.OnClear();
            this.InnerHashtable.Clear();
        }

		/// <summary>
		///   <para>Copies the <see cref="TypedDictionaryBase" /> elements to a one-dimensional <see cref="System.Array" /> at the specified index.</para>
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="System.Array" /> that is the destination of the <see cref="System.Collections.DictionaryEntry" /> objects copied from the <see cref="TypedDictionaryBase" /> instance. The <see cref="System.Array" /> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in an <paramref name="array" /> at which copying begins.</param>
		public /*ICollection*/ void CopyTo(Array array, int index)  
        {
            this.InnerHashtable.CopyTo(array, index);
        }

		/// <summary>
		///   <para>Returns a <see cref="System.Collections.IDictionaryEnumerator" /> that can iterate through the <see cref="TypedDictionaryBase" /> instance.</para>
		/// </summary>
		/// <returns>
		///   <para>A <see cref="System.Collections.IDictionaryEnumerator" /> for the <see cref="TypedDictionaryBase" /> instance.</para>
		/// </returns>
		public /*IEnumerable*/ IDictionaryEnumerator GetEnumerator()  
        {
            return this.InnerHashtable.GetEnumerator();
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns>Enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()  
        {
            return this.GetEnumerator();
        }

		/// <summary>
		///   <para>Performs additional custom processes before clearing the contents of the <see cref="TypedDictionaryBase" /> instance.</para>
		/// </summary>
		protected virtual void OnClear()  
        {
        }

		/// <summary>
		///   <para>Returns the element with the specified key and value in the
		/// <see cref="TypedDictionaryBase" /> instance.</para>
		/// </summary>
		/// <param name="key">The key of the element to get.</param>
		/// <param name="currentValue">The current value of the element associated with <paramref name="key" />.</param>
		/// <returns>
		///   <para>A <see cref="System.Object" /> containing the element with the specified key and
		/// value.</para>
		/// </returns>
		protected virtual object OnGet(object key, object currentValue)  
        {
            return currentValue;
        }

		/// <summary>
		///   <para>Performs additional custom processes before inserting a new element into the
		/// <see cref="TypedDictionaryBase" /> instance.</para>
		/// </summary>
		/// <param name="key">The key of the element to insert.</param>
		/// <param name="value">The value of the element to insert.</param>
		protected virtual void OnInsert(object key, object value)  
        {
        }

		/// <summary>
		///   <para> Performs additional custom processes before removing an element from the <see cref="TypedDictionaryBase" /> instance.</para>
		/// </summary>
		/// <param name="key">The key of the element to remove.</param>
		/// <param name="value">The value of the element to remove.</param>
		protected virtual void OnRemove(object key, object value)  
        {
        }

		/// <summary>
		///   <para>Performs additional custom processes before setting a value in the <see cref="TypedDictionaryBase" /> instance.</para>
		/// </summary>
		/// <param name="key">The key of the element to locate.</param>
		/// <param name="oldValue">The old value of the element associated with <paramref name="key" />.</param>
		/// <param name="newValue">The new value of the element associated with <paramref name="key" />.</param>
		protected virtual void OnSet(object key, object oldValue, object newValue)  
        {
        }

		/// <summary>
		///   <para>Performs additional custom processes when validating the element with the specified key and value.</para>
		/// </summary>
		/// <param name="key">The key of the element to validate.</param>
		/// <param name="value">The value of the element to validate.</param>
		protected virtual void OnValidate(object key, object value)  
        {
        }

        /// <summary>
        /// Adds the element with the specified key and value to the <see cref="TypedDictionaryBase" /> instance.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void IDictionary.Add(object key, object value)  
        {
            this.OnValidate(key,value);
            this.InnerHashtable.Add(key,value);
            try
            {
                this.OnInsert(key,value);
            }  
            catch
            {
                this.InnerHashtable.Remove(key);
                throw;
            } 
        }

        
        bool IDictionary.Contains(object key)  
        {
            return this.InnerHashtable.Contains(key);
        }

        object IDictionary.this[object key]
        {
            get
            {
                this.InnerHashtable[key] = this.OnGet(key,this.InnerHashtable[key]);
                return this.InnerHashtable[key];
            }
            set
            {
                this.OnValidate(key,value);
                this.OnSet(key,this.InnerHashtable[key],value);
                this.InnerHashtable[key] = value;
            }
        }

        
		/// <summary>
		///   <para>Returns the list of keys contained in the <see cref="TypedDictionaryBase" /> instance.</para>
		/// </summary>
		public ICollection Keys
        {
            get 
            {
                return this.InnerHashtable.Keys;
            }
        }

        
		/// <summary>
		///   <para>Returns the list of values contained in the <see cref="TypedDictionaryBase" /> instance.</para>
		/// </summary>
		public ICollection Values  
        {
            get 
            {
                return this.InnerHashtable.Values;
            }
        }

        
        void IDictionary.Remove(object key)  
        {
            this.OnRemove(key,this.InnerHashtable[key]);
            this.InnerHashtable.Remove(key);
        }

		/// <summary>
		///   <para>Removes the number of elements contained in the
		/// <see cref="TypedDictionaryBase" /> instance.</para>
		/// </summary>
		public /*ICollection.*/ int Count
        {
            get 
            {
                if (this.hashtable != null) 
                    return this.hashtable.Count;
                return 0;
            }
        }

        
		/// <summary>
		///   <para>Removes the list of elements contained in the <see cref="TypedDictionaryBase" /> instance.</para>
		/// </summary>
		public IDictionary Dictionary
        {
            get 
            {
                return this;
            }
        }

		/// <summary>
		///   <para>Gets / sets the list of elements contained in the <see cref="TypedDictionaryBase" /> instance.</para>
		/// </summary>
		protected virtual Hashtable InnerHashtable
        {
            get 
            {
                if (this.hashtable == null) 
                    this.hashtable = new Hashtable();
                return this.hashtable;
            }
            set
            {
                this.hashtable = value;
            }
        }

		/// <summary>
		///   <para>Indicates whether
		///  the <see cref="TypedDictionaryBase" /> is Read-only.</para>
		/// </summary>
		public virtual /*ICollection.*/ bool IsReadOnly
        {
            get 
            {
                return this.InnerHashtable.IsReadOnly;
            }
        }

		/// <summary>
		///   <para>Indicates whether the <see cref="TypedDictionaryBase" /> has a fixed size.</para>
		/// </summary>
		public virtual /*ICollection.*/ bool IsFixedSize
        {
            get 
            {
                return this.InnerHashtable.IsFixedSize;
            }
        }

		/// <summary>
		///   <para>Indicates whether access to
		/// the <see cref="TypedDictionaryBase" /> is synchronized (thread-safe).</para>
		/// </summary>
		public virtual /*ICollection.*/ bool IsSynchronized
        {
            get 
            {
                return this.InnerHashtable.IsSynchronized;
            }
        }

		/// <summary>
		///   <para>Gets an object that can be used to
		/// synchronize access to the <see cref="TypedDictionaryBase" />.</para>
		/// </summary>
		public virtual /*ICollection.*/ object SyncRoot
        {
            get 
            {
                return this.InnerHashtable.SyncRoot;
            }
        }


    }

}


