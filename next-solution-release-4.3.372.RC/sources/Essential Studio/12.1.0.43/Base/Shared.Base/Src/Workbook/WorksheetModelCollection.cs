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
using System.Diagnostics;
using System.Runtime.Serialization;

using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	///   <para>Represents a collection of <see cref="WorksheetModel" /> objects.</para>
	/// </summary>
	[
	Serializable, 
	]
	public class WorksheetModelCollection : CollectionBase, IDisposable, ISerializable
	{
		/// <summary>
		/// Occurs when the order of sheets has been changed.
		/// </summary>
		public event SheetMovedEventHandler SheetMoved;

		private int counter = 0;

		// Constructors
        
		/// <overload>
		///   <para>Initializes a new instance of the <see cref="WorksheetModelCollection" /> class.</para>
		/// </overload>
		/// <summary>
		///   <para>Initializes a new instance of the <see cref="WorksheetModelCollection" /> class.</para>
		/// </summary>
		public WorksheetModelCollection()
		{
		} 
        
		/// <summary>
		/// Initializes a new <see cref="WorksheetModelCollection"/> from a serialization stream.
		/// </summary>
		/// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
		/// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
		protected WorksheetModelCollection(SerializationInfo info, StreamingContext context)
		{
#if DEBUG
			if (Switches.Serialization.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
#else
			;
#endif


			SerializationInfoEnumerator sie = info.GetEnumerator();
			while (sie.MoveNext())
			{
				this.Add(sie.Value as WorksheetModel);
			}
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
#if DEBUG
			if (Switches.Serialization.TraceVerbose)
			    TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
#else
			;
#endif


			for (int index = 0; index < Count; index++)
				info.AddValue(index.ToString(), this[index]);
		}

		/// <summary>
		/// Releases any resources used.
		/// </summary>
		public void Dispose()
		{
		//	for (int index = 0; index < Count; index++)
		//		this[index].Dispose();
		}
        
		/// <summary>
		///   <para>Initializes a new instance of the <see cref="WorksheetModelCollection" /> class containing the
		/// elements of the specified source collection.</para>
		/// </summary>
		/// <param name="value">A <see cref="WorksheetModelCollection" /> with which to initialize the collection.</param>
		public WorksheetModelCollection(WorksheetModelCollection value)
		{
			this.AddRange(value);
		} 
        
        
		/// <summary>
		///   <para>Initializes a new instance of the <see cref="WorksheetModelCollection" /> class containing the specified array of <see cref="WorksheetModel" /> objects.</para>
		/// </summary>
		/// <param name="value">An array of <see cref="WorksheetModel" /> objects with which to initialize the collection.</param>
		public WorksheetModelCollection(WorksheetModel[] value)
		{
			this.AddRange(value);
		} 
        
        
		// Methods
        
		/// <summary>
		/// Gets / sets the <see cref="WorksheetModel"/> at the specified index.
		/// </summary>
		public WorksheetModel this[int index] 
		{ 
			get
			{
				return ((WorksheetModel)(base.List[index]));
			} // end of method get_Item
			set
			{
				base.List[index] = value;
			}
		}
        
		/// <summary>
		/// Returns the index of a sheet with the specified name.
		/// </summary>
		/// <param name="name">The name of the sheet to find.</param>
		/// <returns>The index of the sheet with the specified name; -1 if not found.</returns>
		public int IndexOf(string name) 
		{
			 for (int index = 0; index < Count; index++)
				 if (this[index].Name == name)
					 return index;
			 return -1;
		}

		/*public WorksheetModel IndexOf(string name)
		{
			foreach (WorksheetModel ws in this)
				if (ws.Name == name)
					return ws;
			return null;
		}*/

        
		/// <summary>
		///   <para>Adds the specified <see cref="WorksheetModel" /> to the collection.</para>
		/// </summary>
		/// <param name="value">The <see cref="WorksheetModel" /> to add.</param>
		/// <returns>
		///   <para>The index at which the new element was inserted.</para>
		/// </returns>
		public int Add(WorksheetModel value)
		{
			return base.List.Add(value);
		} 
        
        
        
		/// <summary>
		///   <para>Overloaded. Copies the elements of the specified array to the
		///  end of the collection.</para>
		/// </summary>
		/// <param name="value">An array of type <see cref="WorksheetModel" /> containing the objects to add to the collection.</param>
		public void AddRange(WorksheetModel[] value)
		{
			if (value == null) 
				throw new ArgumentNullException("value");

			for (int n = 0; n < value.Length; n++)
				this.Add(value[n]);
		} 
        
        
        
		/// <summary>
		///   <para>Adds the contents of another <see cref="WorksheetModelCollection" /> to the end of the collection.</para>
		/// </summary>
		/// <param name="value">A <see cref="WorksheetModelCollection" /> containing the objects to add to the collection.</param>
		public void AddRange(WorksheetModelCollection value)
		{
			if (value == null) 
				throw new ArgumentNullException("value");

			for (int n = 0; n < value.Count; n++)
				this.Add(value[n]);
		} // end of method AddRange
        
        
        
		/// <summary>
		///   <para>Indicates whether the collection contains the specified <see cref="WorksheetModel" />.</para>
		/// </summary>
		/// <param name="value">The <see cref="WorksheetModel" /> to search for in the collection.</param>
		/// <returns>
		///   <para>
		///     <see langword="True" /> if the collection contains the specified object;
		/// <see langword="False" /> otherwise.</para>
		/// </returns>
		public bool Contains(WorksheetModel value)
		{
			return base.List.Contains(value);
		} // end of method Contains
        
        
        
		/// <summary>
		///   <para>Copies the collection objects to a one-dimensional <see cref="System.Array" /> instance beginning at the
		/// specified index.</para>
		/// </summary>
		/// <param name="array">
		///   <para>The one-dimensional <see cref="System.Array" /> that is the destination of the values copied from the collection.</para>
		/// </param>
		/// <param name="index">The index of the array at which to begin inserting.</param>
		public void CopyTo(WorksheetModel[] array, int index)
		{
			base.List.CopyTo(array, index);
		}
        
        
        
		/// <summary>
		///   <para>Returns the index in the collection of the specified <see cref="WorksheetModel" />, if it exists in the
		/// collection.</para>
		/// </summary>
		/// <param name="value">The <see cref="WorksheetModel" /> to locate in the collection.</param>
		/// <returns>
		///   <para>The index in the collection of the specified object, if found; -1 otherwise.</para>
		/// </returns>
		public int IndexOf(WorksheetModel value)
		{
			return base.List.IndexOf(value);
		} 
        
        
        
		/// <summary>
		///   <para>Inserts the specified <see cref="WorksheetModel" /> into the collection at the specified index.</para>
		/// </summary>
		/// <param name="index">The zero-based index where the specified object should be inserted.</param>
		/// <param name=" value">The <see cref="WorksheetModel" /> to insert.</param>
		public void Insert(int index, WorksheetModel value)
		{
			base.List.Insert(index, value);
		} 
        
        
        
		/// <summary>
		///   <para> Removes the specified <see cref="WorksheetModel" /> from the collection.</para>
		/// </summary>
		/// <param name="value">The <see cref="WorksheetModel" /> to remove from the collection.</param>
		public void Remove(WorksheetModel value)
		{
			base.List.Remove(value);
		} 
        
		/// <summary>
		/// Moves a worksheet from one position to another.
		/// </summary>
		/// <param name="index">The original index of the worksheet.</param>
		/// <param name="destination">The new index of the worksheet.</param>
		public void Move(int index, int destination)
		{
			try
			{		
				BeginUpdate();
				WorksheetModel worksheet = this[index];
				if (destination == -1)
				{
					this.RemoveAt(index);
					this.Add(worksheet);
					destination = this.Count-1;
				}
				else if (destination < index)
				{
					this.RemoveAt(index);
					this.Insert(destination, worksheet);
				}
				else if (destination > index+1)
				{
					this.Insert(destination, worksheet);
					this.RemoveAt(index);
				}
				EndUpdate();
				OnSheetMoved(index, destination, SheetMovedReason.MoveSheet);
			}
			catch(Exception ex) 
			{
				TraceUtil.TraceExceptionCatched(ex);
				if (!ExceptionManager.RaiseExceptionCatched(this, ex))
					throw;
			}
		}


		/// <summary>
		/// Called when the order of the worksheet has changed.
		/// </summary>
		/// <param name="index">The original index of an affected worksheet.</param>
		/// <param name="destination">The new index of an affected worksheet</param>
		/// <param name="reason">The reason for the change.</param>
		protected virtual void OnSheetMoved(int index, int destination, SheetMovedReason reason) 
		{
			// WorkbookViews subscribe to this event and update current selected index position.
			if (Updating)
				return;
#if DEBUG
			
			if (Switches.TabBarSplitterControlEvents.TraceVerbose)
			
			    TraceUtil.TraceCurrentMethodInfo(index, destination, reason);
#else
			
			;
#endif

			if (SheetMoved != null)
			{
				SheetMovedEventArgs e = new SheetMovedEventArgs(index, destination, reason);
				SheetMoved(this, e);
			}
		}

		/// <override/>
		protected override void OnRemoveComplete(int index, object value)  
		{
			OnSheetMoved(index, -1, SheetMovedReason.RemoveSheet);
		}

		/// <override/>
		protected override void OnClearComplete()  
		{
			OnSheetMoved(-1, -1, SheetMovedReason.ClearAll);
		}

		/// <override/>
		protected override void OnInsertComplete(int index, object value)  
		{
			OnSheetMoved(-1, index, SheetMovedReason.InsertSheet);
		}

		/// <override/>
		protected override void OnSetComplete(int index, object oldValue, object newValue)  
		{
			// Called when sheet is replaced with new sheet
			// not possible. Use Insert / Remove instead.
			Debug.Assert(false);
		}

		/// <override/>
		protected override void OnValidate(object value)  
		{
			// Called before value is used in IndexOf, Insert, Remove, etc.
			if (value == null || !(value is WorksheetModel))
				throw new ArgumentException();
		}

		#region Updating
		int updateCount = 0;

		/// <summary>
		/// Suspend updating the display.
		/// </summary>
		public virtual void BeginUpdate()
		{
			this.updateCount++;
		}

		/// <summary>
		/// Resumes updating the display.
		/// </summary>
		public virtual void EndUpdate()
		{
			--this.updateCount;
		}

		/// <summary>
		/// Indicates whether <see cref="BeginUpdate"/> was called.
		/// </summary>
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false)
		]
		public virtual bool Updating
		{
			get
			{
				return this.updateCount > 0;
			}
		}
#endregion

		/// <summary>
		/// Creates a new empty <see cref="WorksheetModel"/> and names it "Sheet #".
		/// </summary>
		/// <param name="workbook">The <see cref="WorkbookModel"/> this new sheet is added to.</param>
		/// <returns>The new <see cref="WorksheetModel"/>.</returns>
		public WorksheetModel CreateEmptyWorksheet(WorkbookModel workbook)
		{
			string name;
			do 
			{
				this.counter++;
				name = "Sheet " + this.counter.ToString();
			} while (this.IndexOf(name) != -1);

			WorksheetModel sheet = new WorksheetModel(workbook, name);
			Add(sheet);
			return sheet;
		}


	} 
}
