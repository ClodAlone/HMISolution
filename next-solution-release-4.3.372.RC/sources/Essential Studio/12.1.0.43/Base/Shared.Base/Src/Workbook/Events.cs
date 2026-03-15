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
using System.ComponentModel;
using Syncfusion.ComponentModel;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Specifies information about the originating action of SheetMoved event.
	/// </summary>
	public enum SheetMovedReason
	{
		/// <summary>
		/// None specified.
		/// </summary>
		None,

		/// <summary>
		/// Sheet was moved.
		/// </summary>
		MoveSheet,

		/// <summary>
		/// Sheet was inserted.
		/// </summary>
		InsertSheet,

		/// <summary>
		/// Sheet was removed.
		/// </summary>
		RemoveSheet,

		/// <summary>
		/// All sheets were removed.
		/// </summary>
		ClearAll
	};

	
	/// <summary>
	/// Handles a <see cref="WorksheetModelCollection.SheetMoved"/> event of a <see cref="WorksheetModelCollection"/>.
	/// </summary>
	public delegate void SheetMovedEventHandler(object sender, SheetMovedEventArgs e);
    
	/// <summary>
	/// Provides data about a <see cref="WorksheetModelCollection.SheetMoved"/> event of a <see cref="WorksheetModelCollection"/>.
	/// </summary>
    public class SheetMovedEventArgs : SyncfusionEventArgs
    {
        private int index;
        private int destination;
        private SheetMovedReason reason;

		/// <summary>
		/// Initializes a SheetMovedEventArgs with event data.
		/// </summary>
		/// <param name="index">The sheet index.</param>
		/// <param name="destination">The destination sheet index.</param>
		/// <param name="reason">The originating action for this event.</param>
        public SheetMovedEventArgs(int index, int destination, SheetMovedReason reason)
        {
            this.index = index;
            this.destination = destination;
            this.reason = reason;
        }

		/// <summary>
		/// Returns the sheet index.
		/// </summary>
		[TraceProperty(true)]
		public int Index
        {
            get
            {
                return index;
            }
        }

		/// <summary>
		/// Returns the destination sheet index.
		/// </summary>
		[TraceProperty(true)]
		public int Destination
        {
            get
            {
                return destination;
            }
        }

		/// <summary>
		/// Returns the originating action for this event.
		/// </summary>
		[TraceProperty(true)]
		public SheetMovedReason Reason
        {
            get
            {
                return reason;
            }
        }
    }

	/// <summary>
	/// Handles a <see cref="TabBar.SelectedIndexChanged"/> and <see cref="TabBar.SelectedIndexChanging"/> events of a <see cref="TabBar"/>.
	/// </summary>
	public delegate void SelectedIndexEventHandler(object sender, SelectedIndexEventArgs e);

	/// <summary>
	/// Provides data about a <see cref="TabBar.SelectedIndexChanged"/> and <see cref="TabBar.SelectedIndexChanging"/> events of a <see cref="TabBar"/>.
	/// </summary>
    public class SelectedIndexEventArgs: SyncfusionCancelEventArgs
    {
		/// <summary>
		/// Initializes a new <see cref="SelectedIndexEventArgs"/> with event data.
		/// </summary>
		/// <param name="index">The selected index.</param>
		/// <param name="tab">The tab associated with the index.</param>
        public SelectedIndexEventArgs(int index, InternalTab tab)
        {
            this.index = index;
            this.tab = tab;
        }

		/// <summary>
		/// Returns the selected index.
		/// </summary>
		[TraceProperty(true)]
        public int Index
        {
            get 
            {
                return index; 
            }
        }

		/// <summary>
		/// Returns the tab associated with the index.
		/// </summary>
 		[TraceProperty(true)]
		public InternalTab Tab
        {
            get 
            {
                return tab; 
            }
        }

        private int index;
        private InternalTab tab;
    }


}
