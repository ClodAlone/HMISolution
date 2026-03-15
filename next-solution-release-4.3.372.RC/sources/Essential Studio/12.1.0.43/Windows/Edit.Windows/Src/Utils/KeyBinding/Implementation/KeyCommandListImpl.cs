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
using System.Windows.Forms;
using Syncfusion.Shared.Utils.KeyBinding;

namespace Syncfusion.Shared.Utils.KeyBinding.Implementation
{
	/// <summary>
	/// Implementation of IKeyCommandList interface.
	/// </summary>
	internal class KeyCommandListImpl
		: IKeyCommandList
	{
		#region Class members
		/// <summary>
		/// Internal data.
		/// </summary>
		private Hashtable m_data = new Hashtable();
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets command by name.
		/// </summary>
		public IKeyCommand this[ string name ]
		{
			get
			{
				if( !m_data.Contains( name ) )
					return null;

				return m_data[ name ] as IKeyCommand;
			}
		}
		/// <summary>
		///  Returns TRUE if the object is synchronized, FALSE otherwise.
		/// </summary>
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}
		/// <summary>
		/// Count of items in collection.
		/// </summary>
		public int Count
		{
			get
			{
				return m_data.Count;
			}
		}

		/// <summary>
		/// Synchronization object.
		/// </summary>
		public object SyncRoot
		{
			get
			{
				return m_data.SyncRoot;
			}
		}

		#endregion

		#region Class Public Methods
		/// <summary>
		/// Copies commands to the specified array.
		/// </summary>
		/// <param name="array">Destination array.</param>
		/// <param name="index">Index in destination array.</param>
		public void CopyTo( Array array, int index )
		{
			m_data.Values.CopyTo( array, index );
		}

		/// <summary>
		/// Gets enumerator for commands.
		/// </summary>
		/// <returns>Enumerator.</returns>
		public IEnumerator GetEnumerator()
		{
			return m_data.Values.GetEnumerator();
		}
		/// <summary>
		/// Creates new command and adds it to list.
		/// </summary>
		/// <param name="name">Name of the command.</param>
		/// <returns>Newly created command.</returns>
		public IKeyCommand Add( string name )
		{
			if( name == null || name == string.Empty )
				throw new ArgumentNullException( "Name can not be null or empty.", Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_125 );

			if( m_data.Contains( name ) )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_126, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_125 );

			KeyCommandImpl impl = new KeyCommandImpl( name, null, this );
			m_data.Add( impl.Name, impl );
			return impl;
		}

		/// <summary>
		/// Removes command from list.
		/// </summary>
		/// <param name="name">Name of the command.</param>
		public void Remove( string name )
		{
			if( m_data.Contains( name ) )
				m_data.Remove( name );
		}

		/// <summary>
		/// Clears list.
		/// </summary>
		public void Clear()
		{
			m_data.Clear();
		}
		/// <summary>
		/// Checks whether command belongs to this list.
		/// </summary>
		/// <param name="command">Command to be checked.</param>
		/// <returns>True if command belongs to this list.</returns>
		public bool CheckIfBelong( IKeyCommand command )
		{
			if( command == null )
				throw new ArgumentNullException( "command" );

			KeyCommandImpl comImpl = command as KeyCommandImpl;

			if( comImpl == null ) return false;

			command = this[ comImpl.Name ];

			if( command == null ) return false;

			return ( command as KeyCommandImpl ).ID == comImpl.ID;
		}
		#endregion
	}
}
