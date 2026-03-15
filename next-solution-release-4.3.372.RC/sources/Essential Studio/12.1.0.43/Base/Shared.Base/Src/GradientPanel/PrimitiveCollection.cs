#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// A collection that stores <see cref="Primitive"/> objects.
	/// </summary>
	[ Editor( typeof( GradientPanelExtCollectionEditor ), typeof( UITypeEditor ) ) ]
	public class PrimitiveCollection : CollectionBase, ICloneable
	{
		#region Initialize/Finalize Method

		/// <summary>
		/// Initializes a new instance of 'PrimitiveCollection'.
		/// </summary>
		public PrimitiveCollection()
		{}

		#endregion

		#region Class Events

		/// <summary>
		/// Raise by <see cref="OnCollectionChanged"/> method.
		/// </summary>
		public event CollectionChangeEventHandler CollectionChanged;

		#endregion 

		#region Class Event Raisers

		private void RaiseCollectionChanged( CollectionChangeEventArgs args )
		{
			if( this.CollectionChanged != null )
			{
				this.CollectionChanged( this, args );
			}
		}

		protected virtual void OnCollectionChanged( CollectionChangeEventArgs args )
		{
			RaiseCollectionChanged( args );
		}

		#endregion

		#region Class Public Method

		/// <summary>
		/// Adds primitive to collection.
		/// </summary>
		public void Add( Primitive primitive )
		{
			if( primitive != null )
			{
				this.InnerList.Add( primitive );

				CollectionChangeEventArgs args = new CollectionChangeEventArgs( CollectionChangeAction.Add, 
					primitive );
				OnCollectionChanged( args );
			}
		}

	
		/// <summary>
		/// Adds primitives to collection.
		/// </summary>
		public void AddRange( Primitive[] primitives )
		{
			if( primitives != null )
			{
				this.InnerList.AddRange( primitives );
				CollectionChangeEventArgs args = new CollectionChangeEventArgs( CollectionChangeAction.Add, 
					primitives );
				OnCollectionChanged( args );
			}
		}

		
		/// <summary>
		/// Removes primitive from collection.
		/// </summary>
		public void Remove( Primitive primitive )
		{
			if( primitive != null )
			{
				this.InnerList.Remove( primitive );
				CollectionChangeEventArgs args = new CollectionChangeEventArgs( CollectionChangeAction.Remove, 
					primitive );
				OnCollectionChanged( args );
			}
		}

        protected override void OnClear()
        {
            foreach (Primitive primitive in this.InnerList)
            {
                primitive.OwnerControl = null;
            }
            base.OnClear();   
        }
		
		/// <summary>
		/// Indexer.
		/// </summary>
		public Primitive this[ int index ]
		{
			get
			{
				return ( Primitive )this.InnerList[ index ];
			}
			set
			{
				this.InnerList[ index ] = value;
			}
		}

		#endregion

		#region Supprot ICloneable

		public virtual object Clone()
		{
			PrimitiveCollection clone = new PrimitiveCollection();

			foreach( Primitive primitive in this.InnerList )
			{
				clone.Add( primitive.Clone() as Primitive );
			}

			return clone;
		}

		#endregion
	}
}
