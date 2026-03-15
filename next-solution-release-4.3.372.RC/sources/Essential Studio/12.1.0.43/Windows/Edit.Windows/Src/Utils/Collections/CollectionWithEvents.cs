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

#region file using directives
using System;
using System.Collections;
#endregion

namespace Syncfusion.Windows.Forms.Edit.Utils
{
	/// <summary>
	/// Summary description for EventBaseCollection.
	/// </summary>
	public class EventBaseCollection : CollectionBase
	{
		#region Class members
		/// <summary>
		/// If TRUE then class must skip all event raising code, otherwise FALSE
		/// </summary>
		private bool m_bSkipEvents;
		#endregion

		#region Class Properties
		/// <summary>
		/// get or set does collection work in silent mode without raising any event
		/// to user or in normal mode.
		/// </summary>
		public bool QuietMode
		{
			get
			{
				return m_bSkipEvents;
			}
			set
			{
				m_bSkipEvents = value;
			}
		}
		#endregion

		#region Class Events
		/// <summary>
		/// Raised on any collection change
		/// </summary>
		public event EventHandler OnChanged;
		/// <summary>
		/// Raised by <see cref="OnClear"/> method
		/// </summary>
		public event CollectionEventHandler Clearing;
		/// <summary>
		/// Raised by <see cref="OnClearComplete"/> method
		/// </summary>
		public event CollectionEventHandler Cleared;
		/// <summary>
		/// Raised by <see cref="OnInsert"/> method
		/// </summary>
		public event CollectionEventHandler Inserting;
		/// <summary>
		/// Raised by <see cref="OnInsertComplete"/> method
		/// </summary>
		public event CollectionEventHandler Inserted;
		/// <summary>
		/// Raised by <see cref="OnRemove"/> method
		/// </summary>
		public event CollectionEventHandler Removing;
		/// <summary>
		/// Raised by <see cref="OnRemoveComplete"/> method
		/// </summary>
		public event CollectionEventHandler Removed;
		/// <summary>
		/// Raised by <see cref="OnSet"/> method
		/// </summary>
		public event CollectionEventHandler Setting;
		/// <summary>
		/// Raised by <see cref="OnSetComplete"/> method
		/// </summary>
		public event CollectionEventHandler Set;
		#endregion

		#region Class Event catchers
		/// <summary>
		///	Fires corresponding events.
		/// </summary>
		protected override void OnClear()
		{
			CollectionEventArgs ev = CollectionEventArgs.Empty;

			if( Clearing != null && !m_bSkipEvents )
			{
				Clearing( this, ev );
			}

			if( !ev.Cancel )
			{
				base.OnClear();
				RaiseOnChangedEvent();
			}
		}

		/// <summary>
		///Fires corresponding events.
		/// </summary>
		protected override void OnClearComplete()
		{
			CollectionEventArgs ev = CollectionEventArgs.Empty;

			if( Cleared != null && !m_bSkipEvents )
			{
				Cleared( this, ev );
			}

			if( !ev.Cancel )
			{
				base.OnClearComplete();
				RaiseOnChangedEvent();
			}
		}

		/// <summary>
		/// Fires corresponding events.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		protected override void OnInsert( int index, object value )
		{
			CollectionEventArgs ev = new CollectionEventArgs( index, value );

			if( Inserting != null && !m_bSkipEvents )
			{
				Inserting( this, ev );
			}

			if( !ev.Cancel )
			{
				base.OnInsert( index, value );
				RaiseOnChangedEvent();
			}
		}

		/// <summary>
		/// Fires corresponding events.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		protected override void OnInsertComplete( int index, object value )
		{
			CollectionEventArgs ev = new CollectionEventArgs( index, value );

			if( Inserted != null && !m_bSkipEvents )
			{
				Inserted( this, ev );
			}

			if( !ev.Cancel )
			{
				base.OnInsertComplete( index, value );
				RaiseOnChangedEvent();
			}
		}

		/// <summary>
		/// Fires corresponding events.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		protected override void OnRemove( int index, object value )
		{
			CollectionEventArgs ev = new CollectionEventArgs( index, value );

			if( Removing != null && !m_bSkipEvents )
			{
				Removing( this, ev );
			}

			if( !ev.Cancel )
			{
				base.OnRemove( index, value );
				RaiseOnChangedEvent();
			}
		}

		/// <summary>
		/// Fires corresponding events.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		protected override void OnRemoveComplete( int index, object value )
		{
			CollectionEventArgs ev = new CollectionEventArgs( index, value );

			if( Removed != null && !m_bSkipEvents )
			{
				Removed( this, ev );
			}

			if( !ev.Cancel )
			{
				base.OnRemoveComplete( index, value );
				RaiseOnChangedEvent();
			}
		}

		/// <summary>
		/// Fires corresponding events.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		protected override void OnSet( int index, object oldValue, object newValue )
		{
			CollectionEventArgs ev = new CollectionEventArgs( index, newValue, oldValue );

			if( Setting != null && !m_bSkipEvents )
			{
				Setting( this, ev );
			}

			if( !ev.Cancel )
			{
				base.OnSet( index, oldValue, newValue );
				RaiseOnChangedEvent();
			}
		}

		/// <summary>
		/// Fires corresponding events.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		protected override void OnSetComplete( int index, object oldValue, object newValue )
		{
			CollectionEventArgs ev = new CollectionEventArgs( index, newValue, oldValue );

			if( Set != null && !m_bSkipEvents )
			{
				Set( this, ev );
			}

			if( !ev.Cancel )
			{
				base.OnSetComplete( index, oldValue, newValue );
				RaiseOnChangedEvent();
			}
		}
		#endregion

		#region Class Event Raisers
		/// <summary>
		/// Fires corresponding events.
		/// </summary>
		protected void RaiseOnChangedEvent()
		{
			if( OnChanged != null && !m_bSkipEvents )
			{
				OnChanged( this, EventArgs.Empty );
			}
		}
		#endregion
	}
	///<exclude/>
	/// <summary>
	/// Delegate used by Collection to send messages to user throwing events.
	/// </summary>
	public delegate void CollectionEventHandler( EventBaseCollection sender, CollectionEventArgs e );

	/// <summary>
	/// Class which used for sending messages between collection and user
	/// </summary>
	public class CollectionEventArgs : EventArgs
	{
		#region Class members
		/// <summary>
		/// storage of Cancel property
		/// </summary>
		private bool m_bCancel;
		/// <summary>
		/// storage of Index property
		/// </summary>
		private int m_iIndex = -1;
		/// <summary>
		/// storage of Value property
		/// </summary>
		private object m_value;
		/// <summary>
		/// storage of OldValue proeprty
		/// </summary>
		private object m_oldValue;
		#endregion

		#region Class Properties
		/// <summary>
		/// If TRUE then class will skip call to base Collection method, otherwise
		/// CollectionBase class override methods will be called.
		/// </summary>
		public bool Cancel
		{
			get
			{
				return m_bCancel;
			}
			set
			{
				m_bCancel = value;
			}
		}

		/// <summary>
		/// Index of item
		/// </summary>
		public int Index
		{
			get
			{
				return m_iIndex;
			}
		}

		/// <summary>
		/// Value of Item
		/// </summary>
		public object Value
		{
			get
			{
				return m_value;
			}
		}

		/// <summary>
		/// If operation in which used this class has new and old value
		/// then this property in work
		/// </summary>
		public object OldValue
		{
			get
			{
				return m_oldValue;
			}
		}
		/// <summary>
		/// Return empty instance of this class
		/// </summary>
		new static public CollectionEventArgs Empty
		{
			get
			{
				return new CollectionEventArgs();
			}
		}
		#endregion

		#region Class constructors
		/// <summary>
		/// Default coinstructor
		/// </summary>
		public CollectionEventArgs()
		{

		}
		/// <summary>
		/// Set Value and Index
		/// </summary>
		/// <param name="index">Index of item</param>
		/// <param name="value">Value - refernce to collection item</param>
		public CollectionEventArgs( int index, object value )
		{
			m_iIndex = index;
			m_value = value;
		}

		/// <summary>
		/// Set Value, Index and Old Value
		/// </summary>
		/// <param name="index">Index of item</param>
		/// <param name="value">Value - refernce to collection item</param>
		/// <param name="old">Old value of item</param>
		public CollectionEventArgs( int index, object value, object old )
		{
			m_iIndex = index;
			m_value = value;
			m_oldValue = old;
		}
		#endregion
	}
}