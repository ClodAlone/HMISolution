#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Syncfusion.Windows.Forms.Collections
{
	/// <summary>
	/// Extends List{T} with events.
	/// </summary>
	/// <typeparam name="T">Type of collection elements.</typeparam>
	public class ObservableList<T> : IList
	{
		#region Constructors
		public ObservableList()
		{
			list = new ArrayList();
		}
		public ObservableList(int capacity)
		{
			list = new ArrayList(capacity);
		}
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		public int Count
		{
			get { return this.List.Count; }
		}
		/// <summary>
		/// 
		/// </summary>
		public int Capacity
		{
			get { return this.list.Capacity; }
			set { this.list.Capacity = value; }
		}
		/// <summary>
		/// 
		/// </summary>
		protected IList List
		{
			get { return this; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Gets or sets the element at the specified index. 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public T this[int index]
		{
			get
			{
				return ((T)this.List[index]);
			}
			set
			{
				this.List[index] = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return this.List.GetEnumerator();
		}
		/// <summary>
		/// Add method.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int Add(T value)
		{
			return this.List.Add(value);
		}
		/// <summary>
		/// Determines the index of a specific item in the list.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int IndexOf(T value)
		{
			return this.List.IndexOf(value);
		}
		/// <summary>
		/// Inserts an item to the list at the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		public void Insert(int index, T value)
		{
			this.List.Insert(index, value);
		}
		/// <summary>
		/// Removes the first occurrence of a specific object from the collection.  
		/// </summary>
		/// <param name="value"></param>
		public void Remove(T value)
		{
			this.List.Remove(value);
		}
		/// <summary>
		/// Removes item at the specified index.  
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index)
		{
			this.List.RemoveAt(index);
		}
		/// <summary>
		/// Determines whether the list contains a specific value.  
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Contains(T value)
		{
			return this.List.Contains(value);
		}
		/// <summary>
		/// Removes all items from the list.  
		/// </summary>
		public void Clear()
		{
			this.List.Clear();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="comparer"></param>
		public void Sort(IComparer comparer)
		{
			this.list.Sort(comparer);
		}
		#endregion

		#region IList Members
		/// <summary>
		/// Adds an item to the list.  
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		int IList.Add(object value)
		{
			int index = -1;

			if( OnInsert(this.list.Count, value))
			{
				index = this.list.Add(value);
				try
				{
					OnInsertComplete(index, value);
				}
				catch
				{
					this.list.RemoveAt(index);
					throw;
				}
			}
			return index;
		}
		/// <summary>
		/// Removes all items from the list.  
		/// </summary>
		void IList.Clear()
		{
			OnClear();

			while (this.list.Count > 0)
			{
				object obj = this.list[0];

				this.list.RemoveAt(0);

				this.OnRemoveComplete(0, obj);
			}

			OnClearCompleted();
		}
		/// <summary>
		/// Determines whether the list contains a specific value.  
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		bool IList.Contains(object value)
		{
			return this.list.Contains(value);
		}
		/// <summary>
		/// Determines the index of a specific item in the list.  
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		int IList.IndexOf(object value)
		{
			return this.list.IndexOf(value);
		}
		/// <summary>
		/// Inserts an item to the list at the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		void IList.Insert(int index, object value)
		{
			if ( index >= 0 && index < this.list.Count )
			{
				if (OnInsert(index, value))
				{
					this.list.Insert(index, value);
					try
					{
						OnInsertComplete(index, value);
					}
					catch
					{
						this.list.RemoveAt(index);
						throw;
					}
				}
			}
		}
		/// <summary>
		/// Gets a value indicating whether the list has a fixed size. 
		/// </summary>
		bool IList.IsFixedSize
		{
			get { return this.list.IsFixedSize; }
		}
		/// <summary>
		/// Gets a value indicating whether the list is read-only. 
		/// </summary>
		bool IList.IsReadOnly
		{
			get { return this.list.IsReadOnly; }
		}
		/// <summary>
		/// Removes the first occurrence of a specific object from the list.  
		/// </summary>
		/// <param name="value"></param>
		void IList.Remove(object value)
		{
			this.List.RemoveAt( this.list.IndexOf(value) );
		}
		/// <summary>
		/// Removes item at the specified index.  
		/// </summary>
		/// <param name="index"></param>
		void IList.RemoveAt(int index)
		{
			if (index >= 0 && index < this.list.Count)
			{
				object obj = this.list[index];

				if (OnRemove(index, obj))
				{
					this.list.RemoveAt(index);
					try
					{
						this.OnRemoveComplete(index, obj);
					}
					catch
					{
						this.list.Insert(index, obj);
						throw;
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets the element at the specified index. 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		object IList.this[int index]
		{
			get
			{ 
				return this.list[index];
			}
			set
			{
				if (index >= 0 && index < this.list.Count)
				{
					if (OnRemove(index, this.list[index]) && OnInsert(index, value))
					{
						this.list[index] = value;
					}
				}
			}
		}
		#endregion

		#region ICollection Members
		/// <summary>
		/// Copies the elements of the ICollection to an Array, starting at a particular Array index. 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		void ICollection.CopyTo(Array array, int index)
		{
			this.list.CopyTo(array, index);
		}
		/// <summary>
		/// Gets the number of elements contained in the ICollection.
		/// </summary>
		int ICollection.Count
		{
			get { return this.list.Count; }
		}
		/// <summary>
		/// Gets a value indicating whether access to the collection is synchronized
		/// </summary>
		bool ICollection.IsSynchronized
		{
			get { return this.list.IsSynchronized; }
		}
		/// <summary>
		/// Gets an object that can be used to synchronize access to the collection. 
		/// </summary>
		object ICollection.SyncRoot
		{
			get { return this.list.SyncRoot; }
		}
		#endregion

		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Performs additional custom processes before inserting a new element into the collection. 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		/// <returns>TRUE to insert object</returns>
		protected virtual bool OnInsert(int index, object value)
		{
			return true;
		}
		/// <summary>
		/// Performs additional custom processes when removing an element from the collection. 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		/// <returns>TRUE to remove object</returns>
		protected virtual bool OnRemove(int index, object value)
		{
			return true;
		}
		/// <summary>
		/// Fires ItemAdded event.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		protected virtual void OnInsertComplete( int index, Object value )
		{
			if( ItemAdded != null )
			{
				ItemAdded( this, new ListItemEventArgs<T>( ( T )value ) );
			}
		}
		/// <summary>
		/// Fires ItemRemoved event.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		protected virtual void OnRemoveComplete(int index, Object value)
		{
			if( ItemRemoved != null )
			{
				ItemRemoved( this, new ListItemEventArgs<T>( ( T )value ) );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnClear()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnClearCompleted()
		{
		}
		#endregion

		#region Events
		/// <summary>
		/// Fired when new item is added to the collection.
		/// </summary>
		public event EventHandler<ListItemEventArgs<T>> ItemAdded;
		/// <summary>
		/// Fired when item is removed from collection.
		/// </summary>
		public event EventHandler<ListItemEventArgs<T>> ItemRemoved;
		#endregion

		#region Fields
		ArrayList list;
		#endregion
	}

	/// <summary>
	/// Event arguments for ObservableList events.
	/// </summary>
	/// <typeparam name="T">Type of items in InnerList.</typeparam>
	public class ListItemEventArgs<T> : EventArgs
	{
		#region Public Fields
		/// <summary>
		/// Underlying item.
		/// </summary>
		public T Item;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates and initializes new instance of ListItemEventArgs.
		/// </summary>
		/// <param name="item">Underlying item.</param>
		public ListItemEventArgs( T item )
		{
			this.Item = item;
		}
		#endregion
	}
}
#endif