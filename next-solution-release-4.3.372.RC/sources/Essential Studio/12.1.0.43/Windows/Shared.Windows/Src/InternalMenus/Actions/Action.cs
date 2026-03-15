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
using System.Diagnostics;
using System.Reflection;

namespace Syncfusion.Windows.Forms.InternalMenus
{

    /// <exclude/>
    /// <summary>
    ///		Wrapper object for menu actions 
	///</summary>
    public class ActionInfo
	{
		#region Members
		//the handler for the event
		EventHandler eventHandler;
		//the object that invokes the event
		object srcObj;
		BasicAction action;
		string actionName;
		#endregion

		#region Construction
		/// <summary>
		///     Creates an ActionInfo object
		/// </summary>
		/// <param name="eventName" type="string">
		///     <para>
		///         Fully qualified name of Event that will be fired
		///     </para>
		/// </param>
		/// <param name="parentForm" type="object">
		///     <para>
		///         Parent form/object that contains the menu that item is in
		///     </para>
		/// </param>
		/// <param name="srcObject" type="object">
		///     <para>
		///         The menu item tied to the event (e.g. MenuItem, ToolbarItem, etc)
		///     </para>
		/// </param>
		public ActionInfo(string eventName,object parentForm, object srcObject)
		{
			actionName = eventName;
			this.srcObj = srcObject;
			action = (BasicAction)parentForm.GetType().Module.Assembly.CreateInstance(eventName);
			if(action == null)
			{
				Trace.WriteLine("Can't create MenuAction " + eventName);
			}
			else
			{
				action.MainWindow = parentForm;
				eventHandler = new EventHandler(action.InvokeAction);
			}

			//Register the action if the parent can
			RegisterAction(parentForm);
		}

		/// <summary>
		///     Adds the action to the parent object's list of ActionItems. The parent object must have a "RegisterAction" method.
		/// </summary>
		/// <param name="parentForm" type="object">
		///     <para>
		///         Parent form/object
		///     </para>
		/// </param>
		public void RegisterAction(object parentForm)
		{
			if(parentForm != null)	
			{
				System.Reflection.MethodInfo mi = parentForm.GetType().GetMethod("RegisterAction");
				if(mi == null)
					return;

				try
				{
					mi.Invoke(parentForm,new object[]{this});
				}
				catch(Exception ex)
				{
					Trace.WriteLine("Unable to invoke RegisterToggleAction on MainWindow");
					Trace.WriteLine(ex.ToString());
				}
			}
		}
		#endregion

		#region Readonly Properties
		
		/// <exclude/>
		public EventHandler EventHandler
		{
			get
			{
				return eventHandler;
			}
		}

		/// <exclude/>
		public object SourceObject
		{
			get
			{
				return srcObj;
			}
		}

		/// <exclude/>
		public string ActionName
		{
			get
			{
				return actionName;
			}
		}

		/// <exclude/>
		public BasicAction Action
		{
			get
			{
				return action;
			}
		}
		#endregion
	}
	#region "'ActionInfoCollection' strongly typed collection class"


    /// <exclude/>
    /// <summary>
	///     A collection that stores 'ActionInfo' objects.
	/// </summary>
	[Serializable()]
    public class ActionInfoCollection : System.Collections.CollectionBase 
	{
    
		/// <summary>
		///     Initializes a new instance of 'ActionInfoCollection'.
		/// </summary>
		public ActionInfoCollection() 
		{
		}
    
		/// <summary>
		///     Initializes a new instance of 'ActionInfoCollection' based on an already existing instance.
		/// </summary>
		/// <param name='actValue'>
		///     A 'ActionInfoCollection' from which the contents is copied
		/// </param>
		public ActionInfoCollection(ActionInfoCollection actValue) 
		{
			this.AddRange(actValue);
		}
    
		/// <summary>
		///     Initializes a new instance of 'ActionInfoCollection' with an array of 'ActionInfo' objects.
		/// </summary>
		/// <param name='actValue'>
		///     An array of 'ActionInfo' objects with which to initialize the collection
		/// </param>
		public ActionInfoCollection(ActionInfo[] actValue) 
		{
			this.AddRange(actValue);
		}
    
		/// <summary>
		///     Represents the 'ActionInfo' item at the specified index position.
		/// </summary>
		/// <param name='intIndex'>
		///     The zero-based index of the entry to locate in the collection.
		/// </param>
		/// <value>
		///     The entry at the specified index of the collection.
		/// </value>
		public ActionInfo this[int intIndex] 
		{
			get 
			{
				return ((ActionInfo)(List[intIndex]));
			}
			set 
			{
				List[intIndex] = value;
			}
		}
    
		/// <summary>
		///     Adds a 'ActionInfo' item with the specified value to the 'ActionInfoCollection'
		/// </summary>
		/// <param name='actValue'>
		///     The 'ActionInfo' to add.
		/// </param>
		/// <returns>
		///     The index at which the new element was inserted.
		/// </returns>
		public int Add(ActionInfo actValue) 
		{
			return List.Add(actValue);
		}
    
		/// <summary>
		///     Copies the elements of an array at the end of this instance of 'ActionInfoCollection'.
		/// </summary>
		/// <param name='actValue'>
		///     An array of 'ActionInfo' objects to add to the collection.
		/// </param>
		public void AddRange(ActionInfo[] actValue) 
		{
			for (int intCounter = 0; (intCounter < actValue.Length); intCounter = (intCounter + 1)) 
			{
				this.Add(actValue[intCounter]);
			}
		}
    
		/// <summary>
		///     Adds the contents of another 'ActionInfoCollection' at the end of this instance.
		/// </summary>
		/// <param name='actValue'>
		///     A 'ActionInfoCollection' containing the objects to add to the collection.
		/// </param>
		public void AddRange(ActionInfoCollection actValue) 
		{
			for (int intCounter = 0; (intCounter < actValue.Count); intCounter = (intCounter + 1)) 
			{
				this.Add(actValue[intCounter]);
			}
		}
    
		/// <summary>
		///     Gets a value indicating whether the 'ActionInfoCollection' contains the specified value.
		/// </summary>
		/// <param name='actValue'>
		///     The item to locate.
		/// </param>
		/// <returns>
		///     True if the item exists in the collection; false otherwise.
		/// </returns>
		public bool Contains(ActionInfo actValue) 
		{
			return List.Contains(actValue);
		}
    
		/// <summary>
		///     Copies the 'ActionInfoCollection' values to a one-dimensional System.Array
		///     instance starting at the specified array index.
		/// </summary>
		/// <param name='actArray'>
		///     The one-dimensional System.Array that represents the copy destination.
		/// </param>
		/// <param name='intIndex'>
		///     The index in the array where copying begins.
		/// </param>
		public void CopyTo(ActionInfo[] actArray, int intIndex) 
		{
			List.CopyTo(actArray, intIndex);
		}
    
		/// <summary>
		///     Returns the index of a 'ActionInfo' object in the collection.
		/// </summary>
		/// <param name='actValue'>
		///     The 'ActionInfo' object whose index will be retrieved.
		/// </param>
		/// <returns>
		///     If found, the index of the value; otherwise, -1.
		/// </returns>
		public int IndexOf(ActionInfo actValue) 
		{
			return List.IndexOf(actValue);
		}
    
		/// <summary>
		///     Inserts an existing 'ActionInfo' into the collection at the specified index.
		/// </summary>
		/// <param name='intIndex'>
		///     The zero-based index where the new item should be inserted.
		/// </param>
		/// <param name='actValue'>
		///     The item to insert.
		/// </param>
		public void Insert(int intIndex, ActionInfo actValue) 
		{
			List.Insert(intIndex, actValue);
		}
    
		/// <summary>
		///     Returns an enumerator that can be used to iterate through
		///     the 'ActionInfoCollection'.
		/// </summary>
		public new ActionInfoEnumerator GetEnumerator() 
		{
			return new ActionInfoEnumerator(this);
		}
    
		/// <summary>
		///     Removes a specific item from the 'ActionInfoCollection'.
		/// </summary>
		/// <param name='actValue'>
		///     The item to remove from the 'ActionInfoCollection'.
		/// </param>
		public void Remove(ActionInfo actValue) 
		{
			List.Remove(actValue);
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
		///     before insering a new item in the collection
		/// </summary>
		protected override void OnInsert(int intIndex, object objValue) 
		{
			//  TODO: Add code here to handle inserting a new item into the collection
		}

        /// <exclude/>
        /// <summary>
		///     A strongly typed enumerator for 'ActionInfoCollection'
		/// </summary>
		public class ActionInfoEnumerator : object, System.Collections.IEnumerator 
		{
        
			private System.Collections.IEnumerator iEnBase;
        
			private System.Collections.IEnumerable iEnLocal;
        
			/// <summary>
			///     Enumerator constructor
			/// </summary>
			public ActionInfoEnumerator(ActionInfoCollection actMappings) 
			{
				this.iEnLocal = ((System.Collections.IEnumerable)(actMappings));
				this.iEnBase = iEnLocal.GetEnumerator();
			}
        
			/// <summary>
			///     Gets the current element from the collection (strongly typed)
			/// </summary>
			public ActionInfo Current 
			{
				get 
				{
					return ((ActionInfo)(iEnBase.Current));
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

	#endregion //('ActionInfoCollection' strongly typed collection class)

}
