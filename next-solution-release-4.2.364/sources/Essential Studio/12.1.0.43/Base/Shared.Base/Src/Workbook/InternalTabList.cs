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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Windows.Forms;
using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// A collection OF <see cref="InternalTab"/> items.
	/// </summary>
	public class InternalTabCollection : CollectionBase 
	{
		TabBar owner;

		/// <summary>
		/// Initializes a <see cref="InternalTabCollection"/> and associates it with an owner.
		/// </summary>
		/// <param name="owner">The owner.</param>
		public InternalTabCollection(TabBar owner)
		{
			this.owner = owner;
		}


		/// <summary>
		/// Adds an array of InternalTab into the list.
		/// </summary>
		/// <param name="value">An InternalTab array.</param>
		/// <remarks><para>Available to enable serialization using AddRange in designer.</para></remarks>
		public void AddRange(InternalTab[] value)
		{
			owner.SuspendLayout();
			this.Clear();
			for (int i = 0; i < value.Length; ++i) 
			{
				value[i].Owner = owner;
				List.Add(value[i]);
			}

			if (owner.SelectedIndex >= this.Count)
				owner.SelectedIndex = Math.Max(0, this.Count-1);

			owner.ResumeLayout();
			owner.PerformLayout();
		}

		/// <summary>
		/// Gets / sets the <see cref="InternalTab"/> at the specified index.
		/// </summary>
		public InternalTab this[int index] 
		{
			get 
			{
				return (InternalTab)(List[index]);
			}
			set 
			{
				value.Owner = owner;
				List[index] = value;
				owner.PerformLayout();
			}
		}

		/// <summary>
		/// Adds an <see cref="InternalTab"/> to the list.
		/// </summary>
		/// <param name="value">The <see cref="InternalTab"/> to be added.</param>
		/// <returns>The index of the added value.</returns>
		public int Add(InternalTab value) 
		{
			value.Owner = owner;
			try
			{
				//MessageBox.Show("List.Add " + value.Label + "(" + this.Count.ToString() + ")");
				int n = List.Add(value);
				if (owner.IsHandleCreated)
					owner.PerformLayout();
				return n;
			}
			catch (Exception ex)
			{
				TraceUtil.TraceExceptionCatched(ex);
				if (!ExceptionManager.RaiseExceptionCatched(null, ex))
					throw;
				return -1;
			}
		}

		/// <summary>
		/// Inserts an <see cref="InternalTab"/> at the specified index.
		/// </summary>
		/// <param name="value">The <see cref="InternalTab"/> to be added.</param>
		/// <param name="index">The index of the added value.</param>
		public void Insert(int index, InternalTab value) 
		{
			value.Owner = owner;
			List.Insert(index, value);
			if (owner.IsHandleCreated)
				owner.PerformLayout();
		}

		/// <summary>
		/// Returns the index of the specific tab.
		/// </summary>
		/// <param name="value">The tab to search for.</param>
		/// <returns>The index of the tab; -1 if not found.</returns>
		public int IndexOf(InternalTab value) 
		{
			return List.IndexOf(value);
		}

		/// <summary>
		/// Indicates whether the specified object is a member of this collection.
		/// </summary>
		/// <param name="value">The object to search for.</param>
		/// <returns>True if is a member; False otherwise.</returns>
		public bool Contains(InternalTab value) 
		{
			return List.Contains(value);
		}

		/// <summary>
		/// Removes an <see cref="InternalTab"/> from the collection.
		/// </summary>
		/// <param name="value">The <see cref="InternalTab"/> to be removed.</param>
		public void Remove(InternalTab value) 
		{
			List.Remove(value);

//			if (owner.SelectedIndex >= this.Count)
//				owner.SelectedIndex = Math.Max(0, this.Count-1);
//           
//			owner.PerformLayout();
		}

        
		/// <overload>
		///   <para>Copies the collection objects to a one-dimensional <see cref="System.Array" /> instance beginning at the
		/// specified index.</para>
		/// </overload>
		/// <summary>
		///   <para>Copies the collection objects to a one-dimensional <see cref="System.Array" /> instance beginning at the
		/// specified index.</para>
		/// </summary>
		/// <param name="array">
		///   <para>The one-dimensional <see cref="System.Array" /> that is the destination of the values copied from the collection.</para>
		/// </param>
		/// <param name="index">The index of the array at which to begin inserting.</param>
		public void CopyTo(InternalTab[] array, int index) 
		{
			List.CopyTo(array, index);
		}

		/// <summary>
		///   <para>Copies the collection objects to a one-dimensional <see cref="System.Array" /> instance beginning at the
		/// specified index.</para>
		/// </summary>
		/// <param name="array">
		///   <para>The one-dimensional <see cref="System.Array" /> that is the destination of the values copied from the collection.</para>
		/// </param>
		/// <param name="index">The index of the array at which to begin inserting.</param>
		public void CopyTo(Array array, int index) 
		{
			List.CopyTo(array, index);
		}
	}
}
