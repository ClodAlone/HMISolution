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

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Provides event data for the various ProvidePersistenceID events.
	/// </summary>
	public class ProvidePersistenceIDEventArgs : EventArgs
	{
		private string id;
		public ProvidePersistenceIDEventArgs(string defaultID)
		{
			this.id = defaultID;
		}

		/// <summary>
		/// Gets / sets a unique ID.
		/// </summary>
		public string PersistenceID
		{
			get{return this.id;}
			set{this.id = value;}
		}
	}
	/// <summary>
	/// Represents a method that lets you specify a unique ID usually distinguishing different
	/// instances of a control type.
	/// </summary>
	public delegate void ProvidePersistenceIDEventHandler(object sender, ProvidePersistenceIDEventArgs e);
}