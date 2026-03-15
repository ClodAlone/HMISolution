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

namespace Syncfusion.Windows.Forms.Edit.Implementation
{
	/// <summary>
	/// Summary description for ContextPromptBoldTextItem.
	/// </summary>
	public class ContextPromptBoldTextItem
	{
		#region Class Private Members
		/// <summary>
		/// Index of the first bolded letter in subject text.
		/// </summary>
		private int m_iBoldTextStart;
		/// <summary>
		/// Length of the bolded text in subject.
		/// </summary>
		private int m_iBoldTextLenght;
		/// <summary>
		/// Description of the bolded item.
		/// </summary>
		private string m_sDescription;
		/// <summary>
		/// Value that specifies whether this item is selected.
		/// </summary>
		private bool m_bSelected;
		/// <summary>
		/// Collection, the item belongs to.
		/// </summary>
		private ContextPromptBoldItemCollection m_parentCollection;
		#endregion

		#region Class Events
		/// <summary>
		/// Event that is raised when Selected property changed.
		/// </summary>
		public event EventHandler SelectionChanged;
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets parent collection of the item.
		/// </summary>
		public ContextPromptBoldItemCollection ParentCollection
		{
			get
			{
				return m_parentCollection;
			}
		}
		/// <summary>
		/// Gets or sets start position of the bold text in 
		/// </summary>
		public int BoldTextStart
		{
			get
			{
				return m_iBoldTextStart;
			}
		}
		/// <summary>
		/// Gets or sets length of the bold text in 
		/// </summary>
		public int BoldTextLength
		{
			get
			{
				return m_iBoldTextLenght;
			}
		}
		/// <summary>
		/// Gets or Sets description of the item.
		/// </summary>
		public string Description
		{
			get
			{
				return m_sDescription;
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
		#endregion

		#region Class Initialization/Finalization
		/// <summary>
		/// Creates and initializes new instance of the class.
		/// </summary>
		/// <param name="parent">Collection, the item belongs to.</param>
		/// <param name="start">Index of the first bolded letter in subject text.</param>
		/// <param name="length">Length of the bolded text in subject.</param>
		/// <param name="description">Description of the bolded item.</param>
		public ContextPromptBoldTextItem( ContextPromptBoldItemCollection parent, int start, int length, string description )
		{
			if( parent == null )
				throw new ArgumentNullException( "parent" );

			m_parentCollection = parent;
			m_iBoldTextLenght = length;
			m_iBoldTextStart = start;
			m_sDescription = description;
		}
		#endregion

		#region Class Helper Methods
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
		#endregion
	}
}
