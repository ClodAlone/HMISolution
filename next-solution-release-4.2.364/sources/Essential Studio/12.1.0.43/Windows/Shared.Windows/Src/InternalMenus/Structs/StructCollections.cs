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
using System.Xml.Serialization;
using System.Runtime.Serialization;


namespace Syncfusion.Windows.Forms.InternalMenus
{
	
	#region "'MenuItemStructCollection' strongly typed collection class"


    /// <exclude/>
    /// <summary>
	///     A collection that stores 'MenuItemStruct' objects.
	/// </summary>
	[Serializable]
	public class MenuItemStructCollection : System.Collections.CollectionBase
	{
		string menuName = "";
		/// <summary>
		///     Initializes a new instance of 'MenuItemStructCollection'.
		/// </summary>
		public MenuItemStructCollection() 
		{
		}
    
		/// <exclude/>
		/// <summary>Name of the menu</summary>
		public string MenuName
		{
			get
			{
				if(menuName == "")
					menuName = "Default Menu";
				return menuName;
			}
			set
			{
				menuName = value;
			}	
		}

		/// <summary>
		///     Initializes a new instance of 'MenuItemStructCollection' based on an already existing instance.
		/// </summary>
		/// <param name='menValue'>
		///     A 'MenuItemStructCollection' from which the contents is copied
		/// </param>
		public MenuItemStructCollection(MenuItemStructCollection menValue) 
		{
			this.AddRange(menValue);
		}
    
		/// <summary>
		///     Initializes a new instance of 'MenuItemStructCollection' with an array of 'MenuItemStruct' objects.
		/// </summary>
		/// <param name='menValue'>
		///     An array of 'MenuItemStruct' objects with which to initialize the collection
		/// </param>
		public MenuItemStructCollection(MenuItemStruct[] menValue) 
		{
			this.AddRange(menValue);
		}
    
		/// <summary>
		///     Represents the 'MenuItemStruct' item at the specified index position.
		/// </summary>
		/// <param name='intIndex'>
		///     The zero-based index of the entry to locate in the collection.
		/// </param>
		/// <value>
		///     The entry at the specified index of the collection.
		/// </value>
		public MenuItemStruct this[int intIndex] 
		{
			get 
			{
				return ((MenuItemStruct)(List[intIndex]));
			}
			set 
			{
				List[intIndex] = value;
			}
		}
    
		/// <summary>
		///     Adds a 'MenuItemStruct' item with the specified value to the 'MenuItemStructCollection'
		/// </summary>
		/// <param name='menValue'>
		///     The 'MenuItemStruct' to add.
		/// </param>
		/// <returns>
		///     The index at which the new element was inserted.
		/// </returns>
		public int Add(MenuItemStruct menValue) 
		{
			return List.Add(menValue);
		}
    
		/// <summary>
		///     Copies the elements of an array at the end of this instance of 'MenuItemStructCollection'.
		/// </summary>
		/// <param name='menValue'>
		///     An array of 'MenuItemStruct' objects to add to the collection.
		/// </param>
		public void AddRange(MenuItemStruct[] menValue) 
		{
			for (int intCounter = 0; (intCounter < menValue.Length); intCounter = (intCounter + 1)) 
			{
				this.Add(menValue[intCounter]);
			}
		}
    
		/// <summary>
		///     Adds the contents of another 'MenuItemStructCollection' at the end of this instance.
		/// </summary>
		/// <param name='menValue'>
		///     A 'MenuItemStructCollection' containing the objects to add to the collection.
		/// </param>
		public void AddRange(MenuItemStructCollection menValue) 
		{
			for (int intCounter = 0; (intCounter < menValue.Count); intCounter = (intCounter + 1)) 
			{
				this.Add(menValue[intCounter]);
			}
		}
    
		/// <summary>
		///     Gets a value indicating whether the 'MenuItemStructCollection' contains the specified value.
		/// </summary>
		/// <param name='menValue'>
		///     The item to locate.
		/// </param>
		/// <returns>
		///     True if the item exists in the collection; false otherwise.
		/// </returns>
		public bool Contains(MenuItemStruct menValue) 
		{
			return List.Contains(menValue);
		}
    
		/// <summary>
		///     Copies the 'MenuItemStructCollection' values to a one-dimensional System.Array
		///     instance starting at the specified array index.
		/// </summary>
		/// <param name='menArray'>
		///     The one-dimensional System.Array that represents the copy destination.
		/// </param>
		/// <param name='intIndex'>
		///     The index in the array where copying begins.
		/// </param>
		public void CopyTo(MenuItemStruct[] menArray, int intIndex) 
		{
			List.CopyTo(menArray, intIndex);
		}
    
		/// <summary>
		///     Returns the index of a 'MenuItemStruct' object in the collection.
		/// </summary>
		/// <param name='menValue'>
		///     The 'MenuItemStruct' object whose index will be retrieved.
		/// </param>
		/// <returns>
		///     If found, the index of the value; otherwise, -1.
		/// </returns>
		public int IndexOf(MenuItemStruct menValue) 
		{
			return List.IndexOf(menValue);
		}
    
		/// <summary>
		///     Inserts an existing 'MenuItemStruct' into the collection at the specified index.
		/// </summary>
		/// <param name='intIndex'>
		///     The zero-based index where the new item should be inserted.
		/// </param>
		/// <param name='menValue'>
		///     The item to insert.
		/// </param>
		public void Insert(int intIndex, MenuItemStruct menValue) 
		{
			List.Insert(intIndex, menValue);
		}
    
		/// <summary>
		///     Returns an enumerator that can be used to iterate through
		///     the 'MenuItemStructCollection'.
		/// </summary>
		public new MenuItemStructEnumerator GetEnumerator() 
		{
			return new MenuItemStructEnumerator(this);
		}
    
		/// <summary>
		///     Removes a specific item from the 'MenuItemStructCollection'.
		/// </summary>
		/// <param name='menValue'>
		///     The item to remove from the 'MenuItemStructCollection'.
		/// </param>
		public void Remove(MenuItemStruct menValue) 
		{
			List.Remove(menValue);
		}

        /// <exclude/>
        /// <summary>
		///     A strongly typed enumerator for 'MenuItemStructCollection'
		/// </summary>
		public class MenuItemStructEnumerator : object, System.Collections.IEnumerator 
		{
        
			private System.Collections.IEnumerator iEnBase;
        
			private System.Collections.IEnumerable iEnLocal;
        
			/// <summary>
			///     Enumerator constructor
			/// </summary>
			public MenuItemStructEnumerator(MenuItemStructCollection menMappings) 
			{
				this.iEnLocal = ((System.Collections.IEnumerable)(menMappings));
				this.iEnBase = iEnLocal.GetEnumerator();
			}
        
			/// <summary>
			///     Gets the current element from the collection (strongly typed)
			/// </summary>
			public MenuItemStruct Current 
			{
				get 
				{
					return ((MenuItemStruct)(iEnBase.Current));
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

	#endregion //('MenuItemStructCollection' strongly typed collection class)

	#region "'ToolBarItemStructCollection' strongly typed collection class"


    /// <exclude/>
    /// <summary>
	///     A collection that stores 'ToolBarItemStruct' objects.
	/// </summary>
	[Serializable()]
	public class ToolBarItemStructCollection : System.Collections.CollectionBase 
	{
		string toolBarName = "";
    
		/// <summary>
		///     Initializes a new instance of 'ToolBarItemStructCollection'.
		/// </summary>
		public ToolBarItemStructCollection() 
		{
		}
    
		/// <exclude/>
		/// <summary>Name of the toolbar</summary>
		public string ToolBarName
		{
			get
			{
				if(toolBarName=="")
					toolBarName="Default ToolBar";

				return toolBarName;
			}
			set
			{
				toolBarName = value;
			}
		}
		/// <summary>
		///     Initializes a new instance of 'ToolBarItemStructCollection' based on an already existing instance.
		/// </summary>
		/// <param name='tooValue'>
		///     A 'ToolBarItemStructCollection' from which the contents is copied
		/// </param>
		public ToolBarItemStructCollection(ToolBarItemStructCollection tooValue) 
		{
			this.AddRange(tooValue);
		}
    
		/// <summary>
		///     Initializes a new instance of 'ToolBarItemStructCollection' with an array of 'ToolBarItemStruct' objects.
		/// </summary>
		/// <param name='tooValue'>
		///     An array of 'ToolBarItemStruct' objects with which to initialize the collection
		/// </param>
		public ToolBarItemStructCollection(ToolBarItemStruct[] tooValue) 
		{
			this.AddRange(tooValue);
		}
    
		/// <summary>
		///     Represents the 'ToolBarItemStruct' item at the specified index position.
		/// </summary>
		/// <param name='intIndex'>
		///     The zero-based index of the entry to locate in the collection.
		/// </param>
		/// <value>
		///     The entry at the specified index of the collection.
		/// </value>
		public ToolBarItemStruct this[int intIndex] 
		{
			get 
			{
				return ((ToolBarItemStruct)(List[intIndex]));
			}
			set 
			{
				List[intIndex] = value;
			}
		}
    
		/// <summary>
		///     Adds a 'ToolBarItemStruct' item with the specified value to the 'ToolBarItemStructCollection'
		/// </summary>
		/// <param name='tooValue'>
		///     The 'ToolBarItemStruct' to add.
		/// </param>
		/// <returns>
		///     The index at which the new element was inserted.
		/// </returns>
		public int Add(ToolBarItemStruct tooValue) 
		{
			return List.Add(tooValue);
		}
    
		/// <summary>
		///     Copies the elements of an array at the end of this instance of 'ToolBarItemStructCollection'.
		/// </summary>
		/// <param name='tooValue'>
		///     An array of 'ToolBarItemStruct' objects to add to the collection.
		/// </param>
		public void AddRange(ToolBarItemStruct[] tooValue) 
		{
			for (int intCounter = 0; (intCounter < tooValue.Length); intCounter = (intCounter + 1)) 
			{
				this.Add(tooValue[intCounter]);
			}
		}
    
		/// <summary>
		///     Adds the contents of another 'ToolBarItemStructCollection' at the end of this instance.
		/// </summary>
		/// <param name='tooValue'>
		///     A 'ToolBarItemStructCollection' containing the objects to add to the collection.
		/// </param>
		public void AddRange(ToolBarItemStructCollection tooValue) 
		{
			for (int intCounter = 0; (intCounter < tooValue.Count); intCounter = (intCounter + 1)) 
			{
				this.Add(tooValue[intCounter]);
			}
		}
    
		/// <summary>
		///     Gets a value indicating whether the 'ToolBarItemStructCollection' contains the specified value.
		/// </summary>
		/// <param name='tooValue'>
		///     The item to locate.
		/// </param>
		/// <returns>
		///     True if the item exists in the collection; false otherwise.
		/// </returns>
		public bool Contains(ToolBarItemStruct tooValue) 
		{
			return List.Contains(tooValue);
		}
    
		/// <summary>
		///     Copies the 'ToolBarItemStructCollection' values to a one-dimensional System.Array
		///     instance starting at the specified array index.
		/// </summary>
		/// <param name='tooArray'>
		///     The one-dimensional System.Array that represents the copy destination.
		/// </param>
		/// <param name='intIndex'>
		///     The index in the array where copying begins.
		/// </param>
		public void CopyTo(ToolBarItemStruct[] tooArray, int intIndex) 
		{
			List.CopyTo(tooArray, intIndex);
		}
    
		/// <summary>
		///     Returns the index of a 'ToolBarItemStruct' object in the collection.
		/// </summary>
		/// <param name='tooValue'>
		///     The 'ToolBarItemStruct' object whose index will be retrieved.
		/// </param>
		/// <returns>
		///     If found, the index of the value; otherwise, -1.
		/// </returns>
		public int IndexOf(ToolBarItemStruct tooValue) 
		{
			return List.IndexOf(tooValue);
		}
    
		/// <summary>
		///     Inserts an existing 'ToolBarItemStruct' into the collection at the specified index.
		/// </summary>
		/// <param name='intIndex'>
		///     The zero-based index where the new item should be inserted.
		/// </param>
		/// <param name='tooValue'>
		///     The item to insert.
		/// </param>
		public void Insert(int intIndex, ToolBarItemStruct tooValue) 
		{
			List.Insert(intIndex, tooValue);
		}
    
		/// <summary>
		///     Returns an enumerator that can be used to iterate through
		///     the 'ToolBarItemStructCollection'.
		/// </summary>
		public new ToolBarItemStructEnumerator GetEnumerator() 
		{
			return new ToolBarItemStructEnumerator(this);
		}
    
		/// <summary>
		///     Removes a specific item from the 'ToolBarItemStructCollection'.
		/// </summary>
		/// <param name='tooValue'>
		///     The item to remove from the 'ToolBarItemStructCollection'.
		/// </param>
		public void Remove(ToolBarItemStruct tooValue) 
		{
			List.Remove(tooValue);
		}

        /// <exclude/>
        /// <summary>
		///     A strongly typed enumerator for 'ToolBarItemStructCollection'
		/// </summary>
		public class ToolBarItemStructEnumerator : object, System.Collections.IEnumerator 
		{
        
			private System.Collections.IEnumerator iEnBase;
        
			private System.Collections.IEnumerable iEnLocal;
        
			/// <summary>
			///     Enumerator constructor
			/// </summary>
			public ToolBarItemStructEnumerator(ToolBarItemStructCollection tooMappings) 
			{
				this.iEnLocal = ((System.Collections.IEnumerable)(tooMappings));
				this.iEnBase = iEnLocal.GetEnumerator();
			}
        
			/// <summary>
			///     Gets the current element from the collection (strongly typed)
			/// </summary>
			public ToolBarItemStruct Current 
			{
				get 
				{
					return ((ToolBarItemStruct)(iEnBase.Current));
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

	#endregion //('ToolBarItemStructCollection' strongly typed collection class)


}
