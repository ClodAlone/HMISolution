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
using System.Diagnostics;
using System.Globalization;
using System.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.Drawing;
using Syncfusion.Documentation;

#endregion

namespace Syncfusion.Windows.Forms.Chart
{
	/// <summary>
	///    Interface that needs to be implemented to display custom axis Grouping Labels. 
	/// </summary>
	public interface IChartAxisGroupingLabelModel
	{
		/// <summary>
		///     Returns the Grouping Label at the specified index.
		/// </summary>
		/// <param name="index" type="int">
		///     <para>
		///     Index value to look for.    
		///     </para>
		/// </param>
		/// <returns>
		///     ChartAxisGroupingLabel to be used as Grouping Label.
		/// </returns>
		ChartAxisGroupingLabel GetGroupingLabelAt( int index );

		/// <summary>
		/// Returns the number of Grouping Labels.    
		/// </summary>
		int Count { get; }
	}

	/// <summary>
	/// Collection of custom ChartAxisGroupingLabel.
	/// <seealso cref="ChartValueType"/>
	/// </summary>
	public class ChartAxisGroupingLabelCollection
	  : CollectionBase
	  , IChartAxisGroupingLabelModel
	{
		#region Class properties

		/// <summary>
		///Returns the axis Grouping Label at the specified index value.
		/// </summary>
		public ChartAxisGroupingLabel this[int index]
		{
			get
			{
				return this.List[index] as ChartAxisGroupingLabel;
			}
		}

		#endregion

		#region Class events
		/// <summary>
		/// Event that is raised when a custom axis Grouping Label is changed.
		/// </summary>
		public event EventHandler Changed;

		#endregion

		#region Class Initialize/Finalize methods

		/// <summary>
		/// To prevent collection construction without ChartAxis reference.
		/// </summary>
		public ChartAxisGroupingLabelCollection()
		{
		}
		#endregion

		#region Class Public Methods

		/// <summary>
		///     Looks up this collection and returns the index value of the specified Grouping Label.
		/// </summary>
		/// <param name="label" type="ChartGroupingAxisLabel">
		///     <para>
		///     Grouping Label to look for in this collection.   
		///     </para>
		/// </param>
		/// <returns>
		///     The index value of the Grouping Label if the look up is successful; -1 otherwise.
		/// </returns>
		public int IndexOf( ChartAxisGroupingLabel label )
		{
			return InnerList.IndexOf(label);
		}

		/// <summary>
		///     Adds the specified Grouping Label to this collection.
		/// </summary>
		/// <param name="label" type="ChartAxisGroupingLabel">
		///     <para>
		///     An instance of the Grouping Label that is to be to add.    
		///     </para>
		/// </param>
		public void Add( ChartAxisGroupingLabel label )
		{

			if (label == null)
				throw new ArgumentNullException("label");

			if (this.List.IndexOf(label) == -1)
			{
				this.List.Add(label);
			}
		}

		/// <summary>
		///     Inserts the specified Grouping Label at the specified index.
		/// </summary>
		/// <param name="index" type="int">
		///     <para>
		///     Index value where the Grouping Label that is to be inserted.   
		///     </para>
		/// </param>
		/// <param name="label" type="ChartAxisGroupingLabel">
		///     <para>
		///     An instance of the Grouping Label that is to be added.    
		///     </para>
		/// </param>
		public void Insert( int index, ChartAxisGroupingLabel label )
		{

			if (index < 0)
				throw new ArgumentOutOfRangeException("index", index, "Value can not be less than 0.");

			if (label == null)
				throw new ArgumentNullException("label");

			this.List.Insert(index, label);
		}

		/// <summary>
		///     Removes the specified Grouping Label from this collection.
		/// </summary>
		/// <param name="label" type="ChartAxisGroupingLabel">
		///     <para>
		///      Grouping Label that is to be removed.   
		///     </para>
		/// </param>
		public void Remove( ChartAxisGroupingLabel label )
		{
			this.List.Remove(label);
		}

		/// <summary>
		///     Gets the Grouping Label at the specified index.
		/// </summary>
		/// <param name="index" type="int">
		///     <para>
		///      The index value to look for.   
		///     </para>
		/// </param>
		/// <returns>
		///     The ChartAxis Grouping Label at the specified index.
		/// </returns>
		public ChartAxisGroupingLabel GetGroupingLabelAt( int index )
		{
			return this[index];
		}
		#endregion

		#region Class overrides

		/// <internalonly/>
		[DocumentationExclude()]
		protected override void OnClearComplete()
		{
			BroadcastChange();
			base.OnClearComplete();
		}

		/// <internalonly/>
		[DocumentationExclude()]
		protected override void OnInsertComplete( int index, object value )
		{
			BroadcastChange();
			base.OnInsertComplete(index, value);
		}

		/// <internalonly/>
		[DocumentationExclude()]
		protected override void OnRemoveComplete( int index, object value )
		{
			BroadcastChange();
			base.OnRemoveComplete(index, value);
		}

		/// <internalonly/>
		[DocumentationExclude()]
		protected override void OnSetComplete( int index, object oldValue, object newValue )
		{
			BroadcastChange();
			base.OnSetComplete(index, oldValue, newValue);
		}

		/// <internalonly/>
		[DocumentationExclude()]
		protected override void OnValidate( object value )
		{

			if (value == null)
				throw new ArgumentNullException("value");
		}

		#endregion

		#region Class event raisers

		/// <summary>
		/// Raise Changed event.
		/// </summary>
		[DocumentationExclude()]
		private void BroadcastChange()
		{
			if (Changed != null)
			{
				Changed(this, EventArgs.Empty);
			}
		}

		#endregion
	}
}