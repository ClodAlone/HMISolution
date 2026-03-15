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
using System.Drawing;

namespace Syncfusion.Windows.Forms.Edit.Implementation
{
	/// <summary>
	/// Summary description for ContextPromptItem.
	/// </summary>
	public class ContextPromptItem
		: IDisposable
	{
		#region Class Private Members
		/// <summary>
		/// Subject string.
		/// </summary>
		private string m_sSubject;
		/// <summary>
		/// Description of the item.
		/// </summary>
		private string m_sDescription;
		/// <summary>
		/// Value that indicates whether this item is currently selected.
		/// </summary>
		private bool m_bSelected;
		/// <summary>
		/// Parent collection.
		/// </summary>
		private ContextPromptCollection m_parent;
		/// <summary>
		/// List of the items that are bolded.
		/// </summary>
		private ContextPromptBoldItemCollection m_listBoldedItems;
		/// <summary>
		/// Associated image.
		/// </summary>
		private Image m_image;
		#endregion

		#region Class Events
		/// <summary>
		/// Event that is raised when Selected property changed.
		/// </summary>
		public event EventHandler SelectionChanged;
		/// <summary>
		/// Event that is raised when bolded item selection changed.
		/// </summary>
		public event EventHandler BoldedItemSelectionChanged;
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets or Sets description of the item.
		/// </summary>
		public string DefaultDescription
		{
			get
			{
				return m_sDescription;
			}
			set
			{
				if( value != m_sDescription )
				{
					m_sDescription = value;
					OnDataChanged();
				}
			}
		}
		/// <summary>
		/// Gets or set subject of the item.
		/// </summary>
		public string Subject
		{
			get
			{
				return m_sSubject;
			}
			set
			{
				if( value != m_sSubject )
				{
					m_sSubject = value;

					OnDataChanged();
				}
			}
		}
		/// <summary>
		/// Gets or Sets value that specifies whether item is selected.
		/// </summary>
		public bool Selected
		{
			get
			{
				return m_bSelected;
			}
			set
			{
				if( m_bSelected != value )
				{
					m_bSelected = value;
					OnSelectedChanged();
				}
			}
		}
		/// <summary>
		/// Gets list of the bolded items.
		/// </summary>
		public ContextPromptBoldItemCollection BoldedItems
		{
			get
			{
				return m_listBoldedItems;
			}
		}
		/// <summary>
		/// Gets description of the currently selected text in bold.
		/// </summary>
		public string Description
		{
			get
			{
				foreach( ContextPromptBoldTextItem item in BoldedItems )
				{
					if( item.Selected )
						return item.Description;
				}

				return DefaultDescription;
			}
		}
		/// <summary>
		/// Gets or sets associated image.
		/// </summary>
		public Image Image
		{
			get
			{
				return m_image;
			}
			set
			{
				m_image = value;
			}
		}
		#endregion

		#region Class Initialization/Finalization
		/// <summary>
		/// Creates and initializes context prompt item.
		/// </summary>
		/// <param name="parent">Parent ContextPromptCollection.</param>
		public ContextPromptItem( ContextPromptCollection parent )
		{
			if( parent == null )
				throw new ArgumentNullException( "parent" );

			m_parent = parent;

			m_listBoldedItems = new ContextPromptBoldItemCollection( this );
			m_listBoldedItems.SelectionChanged += new EventHandler( m_listBoldedItems_SelectionChanged );
		}
		/// <summary>
		/// Disposes context prompt item.
		/// </summary>
		public void Dispose()
		{
			if( m_listBoldedItems != null )
			{
				m_listBoldedItems.Clear();
				m_listBoldedItems = null;
			}
		}
		#endregion

		#region Class Helper Methods
		/// <summary>
		/// Notifies about data changes.
		/// </summary>
		protected void OnDataChanged()
		{
		}
		/// <summary>
		/// Raises SelectionChanged event.
		/// </summary>
		private void OnSelectedChanged()
		{
			if( SelectionChanged != null )
			{
				SelectionChanged( this, EventArgs.Empty );
			}
		}
		/// <summary>
		/// Raises BoldedItemSelectionChanged event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_listBoldedItems_SelectionChanged( object sender, EventArgs e )
		{
			if( BoldedItemSelectionChanged != null )
			{
				BoldedItemSelectionChanged( sender, e );
			}
		}
		#endregion
	}
}
