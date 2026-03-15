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
using Syncfusion.Windows.Forms.Edit;

namespace Syncfusion.Shared.Utils.KeyBinding.Implementation
{
	/// <summary>
	/// Implementation of the IKeyCommand interface.
	/// </summary>
	internal class KeyCommandImpl
		: IKeyCommand
	{
		#region Class members
		/// <summary>
		/// Name of the Command.
		/// </summary>
		private string m_Name;
		/// <summary>
		/// ID of the command.
		/// </summary>
		private Guid m_ID = Guid.NewGuid();
		/// <summary>
		/// Parent list.
		/// </summary>
		private KeyCommandListImpl m_List;
		#endregion

		#region Class Events
		/// <summary>
		/// Event, that is raised when command must be processed.
		/// </summary>
		public event ProcessCommandEventHandler ProcessCommand;
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Executes command.
		/// </summary>
		public void Execute()
		{
			if( ProcessCommand != null )
			{
				ProcessCommand();
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Creates new instance of the class.
		/// </summary>
		/// <param name="name">Name of the Command.</param>
		/// <param name="handler">Handler for command.</param>
		/// <param name="list">Parent list.</param>
		public KeyCommandImpl( string name, ProcessCommandEventHandler handler, KeyCommandListImpl list )
		{
			m_Name = name;
			m_List = list;

			if( handler != null )
				ProcessCommand += handler;
		}
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets name of the command.
		/// </summary>
		public string Name
		{
			get
			{
				return m_Name;
			}
		}
		/// <summary>
		/// ID of the command.
		/// </summary>
		public Guid ID
		{
			get
			{
				return m_ID;
			}
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Gets name of the command.
		/// </summary>
		/// <returns>Name of the command.</returns>
		public override string ToString()
		{
			return m_Name;
		}
		#endregion
	}
}
