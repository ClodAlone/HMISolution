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
	/// Collection throw event on any it context change
	/// </summary>
	public class EventBaseDictionary : DictionaryBase
	{
		#region Class members
		private bool m_bSkipEvents;
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets or sets value indicating whether events should be fired.
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

		#region Class events
		/// <summary>
		/// Raised by <see cref="OnClear"/> method
		/// </summary>
		public event DictionaryEventHandler Clearing;
		/// <summary>
		/// Raised by <see cref="OnClearComplete"/> method
		/// </summary>
		public event DictionaryEventHandler Cleared;
		/// <summary>
		/// Raised by <see cref="OnGet"/> method
		/// </summary>
		public event DictionaryEventHandler Get;
		/// <summary>
		/// Raised by <see cref="OnSet"/> method
		/// </summary>
		public event DictionaryEventHandler Setting;
		/// <summary>
		/// Raised by <see cref="OnSetComplete"/> method
		/// </summary>
		public event DictionaryEventHandler Set;
		/// <summary>
		/// Raised by <see cref="OnInsert"/> method
		/// </summary>
		public event DictionaryEventHandler Inserting;
		/// <summary>
		/// Raised by <see cref="OnInsertComplete"/> method
		/// </summary>
		public event DictionaryEventHandler Inserted;
		/// <summary>
		/// Raised by <see cref="OnRemove"/> method
		/// </summary>
		public event DictionaryEventHandler Removing;
		/// <summary>
		/// Raised by <see cref="OnRemoveComplete"/> method
		/// </summary>
		public event DictionaryEventHandler Removed;
		#endregion

		#region Class Event raisers
		/// <summary>
		/// Fires corresponding events.
		/// </summary>
		protected override void OnClear()
		{
			DictionaryEventArgs ev = DictionaryEventArgs.Empty;

			if( Clearing != null && !m_bSkipEvents )
			{
				Clearing( this, ev );
			}

			if( !ev.Cancel )
			{
				base.OnClear();
			}
		}
		/// <summary>
		/// Fires corresponding events.
		/// </summary>
		protected override void OnClearComplete()
		{
			DictionaryEventArgs ev = DictionaryEventArgs.Empty;

			if( Cleared != null && !m_bSkipEvents )
			{
				Cleared( this, ev );
			}

			if( !ev.Cancel )
			{
				base.OnClearComplete();
			}
		}
		/// <summary>
		/// Fires corresponding events.
		/// </summary>
		/// <param name="key">The key of the element to get.</param>
		/// <param name="currentValue">The current value of the element associated with key.</param>
		/// <returns>An Object containing the element with the specified key and value.</returns>
		protected override object OnGet( object key, object currentValue )
		{
			DictionaryEventArgs ev = new DictionaryEventArgs( key, currentValue );

			if( Get != null && !m_bSkipEvents )
			{
				Get( this, ev );
			}

			if( !ev.Cancel )
			{
				return base.OnGet( key, currentValue );
			}

			return null;
		}
		/// <summary>
		/// Fires corresponding events.
		/// </summary>
		/// <param name="key">The key of the element to locate.</param>
		/// <param name="oldValue">The old value of the element associated with key.</param>
		/// <param name="newValue">The new value of the element associated with key.</param>
		protected override void OnSet( object key, object oldValue, object newValue )
		{
			DictionaryEventArgs ev = new DictionaryEventArgs( key, newValue, oldValue );

			if( Setting != null && !m_bSkipEvents )
			{
				Setting( this, ev );
			}

			if( !ev.Cancel )
			{
				base.OnSet( key, oldValue, newValue );
			}
		}
		/// <summary>
		/// Fires corresponding events.
		/// </summary>
		/// <param name="key">The key of the element to locate.</param>
		/// <param name="oldValue">The old value of the element associated with key.</param>
		/// <param name="newValue">The new value of the element associated with key.</param>
		protected override void OnSetComplete( object key, object oldValue, object newValue )
		{
			DictionaryEventArgs ev = new DictionaryEventArgs( key, newValue, oldValue );

			if( Set != null && !m_bSkipEvents )
			{
				Set( this, ev );
			}

			if( !ev.Cancel )
			{
				base.OnSetComplete( key, oldValue, newValue );
			}
		}
		/// <summary>
		/// Fires corresponding events.
		/// </summary>
		/// <param name="key">The key of the element to insert.</param>
		/// <param name="value">The value of the element to insert.</param>
		protected override void OnInsert( object key, object value )
		{
			DictionaryEventArgs ev = new DictionaryEventArgs( key, value );

			if( Inserting != null && !m_bSkipEvents )
			{
				Inserting( this, ev );
			}

			if( !ev.Cancel )
			{
				base.OnInsert( key, value );
			}
		}
		/// <summary>
		/// Fires corresponding events.
		/// </summary>
		/// <param name="key">The key of the element to insert.</param>
		/// <param name="value">The value of the element to insert.</param>
		protected override void OnInsertComplete( object key, object value )
		{
			DictionaryEventArgs ev = new DictionaryEventArgs( key, value );

			if( Inserted != null && !m_bSkipEvents )
			{
				Inserted( this, ev );
			}

			if( !ev.Cancel )
			{
				base.OnInsertComplete( key, value );
			}
		}
		/// <summary>
		/// Fires corresponding events.
		/// </summary>
		/// <param name="key">The key of the element to remove.</param>
		/// <param name="value">The value of the element to remove.</param>
		protected override void OnRemove( object key, object value )
		{
			DictionaryEventArgs ev = new DictionaryEventArgs( key, value );

			if( Removing != null && !m_bSkipEvents )
			{
				Removing( this, ev );
			}

			if( !ev.Cancel )
			{
				base.OnRemove( key, value );
			}
		}
		/// <summary>
		/// Fires corresponding events.
		/// </summary>
		/// <param name="key">The key of the element to remove.</param>
		/// <param name="value">The value of the element to remove.</param>
		protected override void OnRemoveComplete( object key, object value )
		{
			DictionaryEventArgs ev = new DictionaryEventArgs( key, value );

			if( Removed != null && !m_bSkipEvents )
			{
				Removed( this, ev );
			}

			if( !ev.Cancel )
			{
				base.OnRemoveComplete( key, value );
			}
		}
		#endregion
	}
	///<exclude/>
	/// <summary>
	/// Delegate which is used for throwing events
	/// </summary>
	public delegate void DictionaryEventHandler( EventBaseDictionary sender, DictionaryEventArgs e );

	/// <summary>
	/// Message/Data sender class.
	/// </summary>
	public class DictionaryEventArgs : EventArgs
	{
		#region Class members
		private bool m_bCancel;
		private object m_key;
		private object m_value;
		private object m_oldValue;
		#endregion

		#region Class Properties
		/// <summary>
		/// TRUE - cancel current action
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
		/// Get Key Value of Dictionary Item
		/// </summary>
		public object Key
		{
			get
			{
				return m_key;
			}
		}

		/// <summary>
		/// Get Dictionary Item
		/// </summary>
		public object Value
		{
			get
			{
				return m_value;
			}
		}

		/// <summary>
		/// On Replace Get Dictionary item whicg will be replaced
		/// </summary>
		public object OldValue
		{
			get
			{
				return m_oldValue;
			}
		}

		/// <summary>
		/// Get Empty object
		/// </summary>
		new static public DictionaryEventArgs Empty
		{
			get
			{
				return new DictionaryEventArgs();
			}
		}
		#endregion

		#region Class constructors
		/// <summary>
		/// Creates new DictionaryEventArgs.
		/// </summary>
		public DictionaryEventArgs()
		{
		}
		/// <summary>
		/// Creates new DictionaryEventArgs.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		public DictionaryEventArgs( object key, object value )
		{
			m_key = key;
			m_value = value;
		}
		/// <summary>
		/// Creates new DictionaryEventArgs.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		/// <param name="old">Old value.</param>
		public DictionaryEventArgs( object key, object value, object old )
			: this( key, value )
		{
			m_oldValue = old;
		}
		#endregion
	}
}
