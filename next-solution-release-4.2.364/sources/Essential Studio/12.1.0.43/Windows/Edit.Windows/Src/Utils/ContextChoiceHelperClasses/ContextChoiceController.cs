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
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;

using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Dialogs;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Forms.Popup;
using Syncfusion.Windows.Forms.Edit.Implementation.Parser;

namespace Syncfusion.Windows.Forms.Edit.Utils
{
	/// <summary>
	/// Class used to control context choice dialog and context choice items.
	/// </summary>
	internal class ContextChoiceController
		: IContextChoiceController
		, IDisposable
	{
		#region Internal Classes
		/// <summary>
		/// Data about auto complete string.
		/// </summary>
		internal class AutoCompleteStringInfo
		{
			#region Public Fields
			/// <summary>
			/// 
			/// </summary>
			public string Text = string.Empty;
			/// <summary>
			/// 
			/// </summary>
			public int Column = -1;
			#endregion

			#region Public Methods
			/// <summary>
			/// Checks whether string is empty.
			/// </summary>
			/// <returns>false if string is empty.</returns>
			public bool IsEmpty()
			{
				return Text == string.Empty || Column == 0;
			}
			#endregion
		}
		#endregion

		#region Constants
		/// <summary>
		/// Name of the unnamed images.
		/// </summary>
		private const string DEF_UNKNOWN_IMAGE_NAME = "~NoName~Image~";
		/// <summary>
		/// Default form width.
		/// </summary>
		private const int DEF_FORM_DEFAULT_WIDTH = 176;
		/// <summary>
		/// Default form height.
		/// </summary>
		internal const int DEF_FORM_DEFAULT_HEIGHT = 88;
		#endregion

		#region Fields
		/// <summary>
		/// Size of the context choice form.
		/// </summary>
		protected Size m_formSize = new Size( DEF_FORM_DEFAULT_WIDTH, DEF_FORM_DEFAULT_HEIGHT );
		/// <summary>
		/// Parent control.
		/// </summary>
		protected StreamEditControl m_control;
		/// <summary>
		/// Contect choice form.
		/// </summary>
		protected ContextChoice m_form;
		/// <summary>
		/// Context choice items.
		/// </summary>
		protected ContextChoiceItemCollection m_items;
		/// <summary>
		/// Named images list.
		/// </summary>
		protected NamedImageList m_images;
		/// <summary>
		/// Specifies value indicating whether autocomplete should be used.
		/// </summary>
		private bool m_bUseAutocomplete;
		/// <summary>
		/// Last selected item.
		/// </summary>
		private IContextChoiceItem m_itemSelected;
		/// <summary>
		/// Index of the last unnamed image.
		/// </summary>
		private static int m_iUnnamedImageIndex;
		/// <summary>
		/// Common part of words to auto complete.
		/// </summary>
		private string m_strCommonPart = string.Empty;
		///<summary>
		///Indicates whether auto complete string is shown and selected in control.
		///</summary>
		private bool m_bAutoCompleteStringShown;
		/// <summary>
		/// Dropping lexem.
		/// </summary>
		private IRenderedLexem m_droppingLexem;
		/// <summary>
		/// Lexem situated before dropper.
		/// </summary>
		private IRenderedLexem m_lexemBeforeDroper;
		/// <summary>
		/// Form border color.
		/// </summary>
		private Color m_clrFormBorder = Color.Black;
		/// <summary>
		/// Specifies value indicating whether items filtering string should be extended back to the whitespace.
		/// </summary>
		private bool m_bExtendItemsFilteringString = true;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets currently selected item.
		/// </summary>
		public IContextChoiceItem SelectedItem
		{
			get
			{
				IContextChoiceItem result = null;

				if( m_form != null )
				{
					int id = m_form.SelectedItemID;
					if( id >= 0 )
					{
						result = m_items[ id ];
					}
				}
				else
				{
					result = m_itemSelected;
				}

				return result;
			}
			set
			{
				if( m_form != null )
				{
					int id = ( value != null ) ? ( value.ID ) : ( -1 );
					if( m_form.SelectedItemID != id )
					{
						m_form.SelectedItemID = id;
					}
				}
				else throw new InvalidOperationException( "Can not select item while context choice is not visible." );
			}
		}
		/// <summary>
		/// Gets value indicating whether context choice form is visible.
		/// </summary>
		public bool IsVisible
		{
			get
			{
				return ( null != m_form ) && ( m_form.Visible ) && ( !m_form.Disposing );
			}
		}
		/// <summary>
		/// Gets named images collection.
		/// </summary>
		public INamedImagesCollection Images
		{
			get
			{
				return m_images;
			}
		}
		/// <summary>
		/// Gets items collection.
		/// </summary>
		public ContextChoiceItemCollection Items
		{
			get
			{
				return m_items;
			}
		}
		/// <summary>
		/// Gets or sets value that specifies whether autocomplete is used with current context choice.
		/// </summary>
		public bool UseAutocomplete
		{
			get
			{
				return m_bUseAutocomplete;
			}
			set
			{
				m_bUseAutocomplete = value;
			}
		}
		/// <summary>
		/// Gets or sets value that specifies whether items filtering string should be extended back to the whitespace.
		/// </summary>
		public bool ExtendItemsFilteringString
		{
			get
			{
				return m_bExtendItemsFilteringString;
			}
			set
			{
				m_bExtendItemsFilteringString = value;
			}
		}
		/// <summary>
		/// Gets common part of words to auto complete.
		/// </summary>
		public string CommonPart
		{
			get
			{
				return m_strCommonPart;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether auto complete string is shown and selected in control.
		/// </summary>
		public bool AutoCompleteStringShown
		{
			get
			{
				return m_bAutoCompleteStringShown;
			}
			set
			{
				m_bAutoCompleteStringShown = value;
			}
		}
		/// <summary>
		/// Gets or sets size of the context choice form.
		/// </summary>
		public Size FormSize
		{
			get
			{
				return m_formSize;
			}
			set
			{
				if( !value.IsEmpty )
				{
					m_formSize = value;
				}
				else throw new ArgumentOutOfRangeException( "FormSize" );
			}
		}
		/// <summary>
		/// Gets or sets dropping lexem.
		/// </summary>
		public IRenderedLexem Dropper
		{
			get
			{
				return m_droppingLexem;
			}
			set
			{
				m_droppingLexem = value;
			}
		}
		/// <summary>
		/// Gets or sets lexem situated before dropper.
		/// </summary>
		public IRenderedLexem LexemBeforeDropper
		{
			get
			{
				return m_lexemBeforeDroper;
			}
			set
			{
				m_lexemBeforeDroper = value;
			}
		}
		/// <summary>
		/// Gets or sets form border color.
		/// </summary>
		public Color FormBorderColor
		{
			get
			{
				return m_clrFormBorder;
			}
			set
			{
				m_clrFormBorder = value;
			}
		}
		#endregion

		#region Nonpublic Properties
		/// <summary>
		/// Gets bounds of ContextChoice form.
		/// </summary>
		internal Rectangle FormBounds
		{
			get
			{
				return ( m_form == null ) ? ( Rectangle.Empty ) : ( m_form.Bounds );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal Form Form
		{
			get
			{
				return m_form;
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Event, that is raised when some context choice list item gets selected.
		/// </summary>
		public event ContextChoiceItemSelectedEventHandler ItemSelected;
		/// <summary>
		/// Event that is raised before the ContextChoice dialog is shown to user.
		/// </summary>
		public event CancelEventHandler ContextChoiceBeforeOpen;
		/// <summary>
		/// Event that is raised when auto-complete dialog should be updated.
		/// </summary>
		public event ContextChoiceEventHandler ContextChoiceUpdate;
		/// <summary>
		/// Event that is raised when auto-complete dialog has been opened.
		/// </summary>
		public event ContextChoiceEventHandler ContextChoiceOpen;
		/// <summary>
		/// Event that is raised when auto-complete dialog has been closed.
		/// </summary>
		public event ContextChoiceCloseEventHandler ContextChoiceClose;
		/// <summary>
		/// Event that is raised when auto-complete dialog is being closed.
		/// </summary>
		public event EventHandler ContextChoiceBeforeClosing;
		/// <summary>
		/// Event that is raised when auto-complete sring should be inserted.
		/// </summary>
		public event ContextChoiceEventHandler ContextChoiceAutoComplete;
		/// <summary>
		/// Event that is raised when context choice form is loaded.  User can set it's coordinates at that time. 
		/// Sender parameter will refer to the loaded form.
		/// </summary>
		public event EventHandler FormLoad;
		/// <summary>
		/// Raised when context choice window is right clicked.
		/// </summary>
		public event ContextChoiceItemEventHandler ContextChoiceRightClick;
		#endregion

		#region Initialization & Finalization
		/// <summary>
		/// Creates and initializes context choice controller.
		/// </summary>
		/// <param name="control">Edit control, the controller is attached to.</param>
		/// <param name="autocomplete">Indicates whether autocomplete should be used.</param>
		public ContextChoiceController( StreamEditControl control, bool autocomplete )
		{
			if( null == control ) throw new ArgumentNullException( "control" );

			m_control = control;
			m_items = new ContextChoiceItemCollection();
			m_images = new NamedImageList();
			m_bUseAutocomplete = autocomplete;
		}
		/// <summary>
		/// Frees resources.
		/// </summary>
		public void Dispose()
		{
			if( m_form != null )
			{
				m_form.Dispose();
			}

			m_control = null;
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Adds named image to the image list.
		/// </summary>
		/// <param name="name">Name of the image. Must be unique.</param>
		/// <param name="img">The image to be added.</param>
		/// <param name="transparent">Transparent color.</param>
		/// <returns>INamedImage object that identifies the image.</returns>
		public INamedImage AddImage( string name, System.Drawing.Image img, System.Drawing.Color transparent )
		{
			return m_images.AddImage( name, img, transparent );
		}
		/// <summary>
		/// Adds named image to the image list.
		/// </summary>
		/// <param name="name">Name of the image. Must be unique.</param>
		/// <param name="img">The image to be added.</param>
		/// <returns>INamedImage object that identifies the image.</returns>
		public INamedImage AddImage( string name, System.Drawing.Image img )
		{
			return m_images.AddImage( name, img );
		}
		/// <summary>
		/// Adds unnamed image to the image list.
		/// </summary>
		/// <param name="img">The image to be added.</param>
		/// <param name="transparent">Transparent color.</param>
		/// <returns>INamedImage object that identifies the image.</returns>
		public INamedImage AddImage( System.Drawing.Image img, System.Drawing.Color transparent )
		{
			string name = GetIndexedImageName();
			return AddImage( name, img, transparent );
		}
		/// <summary>
		/// Adds unnamed image to the image list.
		/// </summary>
		/// <param name="img">The image to be added.</param>
		/// <returns>INamedImage object that identifies the image.</returns>
		public INamedImage AddImage( System.Drawing.Image img )
		{
			string name = GetIndexedImageName();
			return AddImage( name, img );
		}
		/// <summary>
		/// Shows context choice list.
		/// </summary>
		public void Show()
		{
			ShowFormInternal();
		}
		/// <summary>
		/// Closes form if visible.
		/// </summary>
		public void Close()
		{
			this.Close( true );
		}
		/// <summary>
		/// Closes form if visible.
		/// </summary>
		/// <param name="cancel">Specifies whether cancel action should be simulated.</param>
		public virtual void Close( bool cancel )
		{
			if( this.IsVisible )
			{
				m_bAutoCompleteStringShown = false;

				if( cancel )
				{
					m_form.Close();
				}
				else
				{
					m_form.CloseOk();
				}
			}
		}
		/// <summary>
		/// Updates form.
		/// </summary>
		public void Update()
		{
			UpdateFormInternal();
		}
		/// <summary>
		/// Gets array of the visible context choice items.
		/// </summary>
		/// <returns>IContextChoiceController array.</returns>
		internal IContextChoiceItem[] GetVisibleItems()
		{
			ArrayList list = new ArrayList();

			foreach( IContextChoiceItem item in m_items )
			{
				if( item.Visible )
				{
					list.Add( item );
				}
			}

			IContextChoiceItem[] result = ( IContextChoiceItem[] )list.ToArray( typeof( IContextChoiceItem ) );
			return result;
		}
		/// <summary>
		/// Gets lexem that should be auto-completed.
		/// </summary>
		/// <returns>IRenderedLexem.</returns>
		internal AutoCompleteStringInfo GetAutoCompleteStringInfo()
		{
			return GetAutoCompleteStringInfoInternal();
		}
		/// <summary>
		/// Selects node corresponding in the context choice tree if possible.
		/// </summary>
		/// <param name="p">Point to select node at in screen coordinates.</param>
		/// <returns>True if succeeds; otherwise false.</returns>
		internal bool ProcessMouseClick( Point p )
		{
			bool bResult = false;

			if( m_form != null )
			{
				TreeView tree = m_form.ItemsView as TreeView;
				if( tree != null )
				{
					TreeNode node = tree.GetNodeAt( tree.PointToClient( p ) );
					if( node != null )
					{
						tree.SelectedNode = node;
					}
				}
			}

			return bResult;
		}
		/// <summary>
		/// Called when new form is created.
		/// </summary>
		/// <param name="form"></param>
		protected virtual void OnNewFormCreated( ContextChoice form )
		{
		}
		#endregion

		#region Nonpublic Methods
		/// <summary>
		/// Creates new ContextChoice form.
		/// </summary>
		/// <returns>ContextChoice form.</returns>
		protected virtual ContextChoice GetContextChoiceForm()
		{
			return new ContextChoice( m_images, m_items, m_control, true, m_control, true, true, m_formSize );
		}
		/// <summary>
		/// Raises ContextChoiceUpdate
		/// </summary>
		protected virtual void UpdateFormInternal()
		{
			if( m_bAutoCompleteStringShown )
			{
				m_control.DeclineAutoComplete();
			}

			m_strCommonPart = FilterItemsByAutocomplete();

			AutoCompleteStringInfo ac = GetAutoCompleteStringInfoInternal();
			string stringToAutoComplete = ( !ac.IsEmpty() ) ? ( ac.Text ) : ( string.Empty );
			m_form.UpdateNodesList( stringToAutoComplete.ToLower() );

			if( ContextChoiceAutoComplete != null && m_bUseAutocomplete )
			{
				ContextChoiceAutoComplete( this );
			}
			if( ContextChoiceUpdate != null )
			{
				ContextChoiceUpdate( this );
			}
		}
		/// <summary>
		/// Gets lexem that should be auto-completed.
		/// </summary>
		/// <returns>IRenderedLexem.</returns>
		private AutoCompleteStringInfo GetAutoCompleteStringInfoInternal()
		{
			AutoCompleteStringInfo result = new AutoCompleteStringInfo();

			//if( this.UseAutocomplete )
			{
				IRenderedLexem lexem = m_control.GetLexemUnderCursor();
				if( lexem != null )
				{
					if( lexem.Config.Type == FormatType.Whitespace || lexem.Config.Type == FormatType.Operator )
					{
						if( lexem.Column == m_control.CurrentColumn )
						{
							lexem = ( lexem.Column > 1 ) ?
								( ( IRenderedLexem )m_control.CurrentLineInstanceInternal.FindLexemByColumn( lexem.Column - 1 ) ) : ( null );
						}
						else
						{
							lexem = null;
						}
					}

					if( lexem != null && !lexem.Config.DropContextChoiceList )
					{
						StringBuilder s = new StringBuilder();
						IList lexems = m_control.CurrentLineInstanceInternal.LineLexems;
						int lexIndex = lexems.IndexOf( lexem );
						IRenderedLexem lex = null;
						int col = -1;
						for( int i = lexIndex ; i >= 0 ; i-- )
						{
							lex = ( IRenderedLexem )lexems[ i ];
							if( !lex.Config.DropContextChoiceList && lex.Text.Trim() != string.Empty )
							{
								s.Insert( 0, lex.Text );
								col = lex.Column;
								if( !this.ExtendItemsFilteringString )
								{
									break;
								}
							}
							else
							{
								break;
							}
						}

						if( lex != null )
						{
							result.Text = s.ToString();
							result.Column = col;
						}
					}
				}
			}

			return result;
		}
		/// <summary>
		/// Shows context choice list.
		/// </summary>
		private void ShowFormInternal()
		{
			if( !this.IsVisible )
			{
				m_itemSelected = null;

				bool bCancel = false;
				if( ContextChoiceBeforeOpen != null )
				{
					CancelEventArgs argsCancel = new CancelEventArgs();
					ContextChoiceBeforeOpen( ( IContextChoiceController )this, argsCancel );
					if( argsCancel.Cancel )
					{
						bCancel = true;
					}
				}

				if( !bCancel )
				{
					m_items.Clear();

					if( null != ContextChoiceOpen )
					{
						ContextChoiceOpen( this );
					}

					if( m_form != null )
					{
						m_form.Dispose();
					}

					m_form = GetContextChoiceForm();
					m_form.BorderColor = this.FormBorderColor;
					m_form.Closed += new EventHandler( OnFormClosed );
					m_form.Closing += new CancelEventHandler( OnFormClosing );
					m_form.Load += new EventHandler( OnFormLoad );
					m_form.ItemSelected += new ContextChoiceItemSelectedEventHandler( OnFormItemSelected );
					m_form.ItemsView.MouseDown += new MouseEventHandler( OnItemsViewMouseDown );
					m_form.RightToLeft = m_control.RightToLeft;

					OnNewFormCreated( m_form );

					Form parentForm = m_control.TopLevelControl as Form;

					if( null != parentForm )
					{
						m_form.Owner = parentForm;
					}
					else
					{
						//m_form.Parent = m_control;
					}

					m_form.Show();
					m_control.UndoGroupOpen();
					UpdateFormInternal();
					//m_control.AcceptAutoComplete();

					m_form.CheckSelection();
				}
			}
		}
		/// <summary>
		/// Filters context choice items depending on the autocomplete state.
		/// </summary>
		/// <returns>Common part of items that has to be proposed for autocompliting.</returns>
		private string FilterItemsByAutocomplete()
		{
			string result = string.Empty;

			if( m_items.Count != 0 )
			{
				string textToFind = string.Empty;

				AutoCompleteStringInfo ac = GetAutoCompleteStringInfoInternal();
				if( this.UseAutocomplete && !ac.IsEmpty() )
				{
					string text = ac.Text;
					int length = Math.Min( m_control.CurrentColumn - ac.Column, text.Length );
					textToFind = ac.Text.Substring( 0, length ).ToLower().Trim();
				}

				foreach( ContextChoiceItem item in m_items )
				{
					string strItemText = item.Text.ToLower();
					bool bStartsOK = strItemText.StartsWith( textToFind );
					item.Visible = !UseAutocomplete || bStartsOK;

					if( bStartsOK && textToFind != string.Empty )
					{
						if( result == string.Empty )
						{
							result = strItemText;
						}
						else
						{
							result = GetCommonPart( result, item.Text.ToLower() );
						}
					}
				}

				if( result != string.Empty )
				{
					result = result.Remove( 0, textToFind.Length );
				}
			}

			return result;
		}
		/// <summary>
		/// Generates name for the unnamed indexed image.
		/// </summary>
		/// <returns>string that indentifies the image.</returns>
		private string GetIndexedImageName()
		{
			string name = DEF_UNKNOWN_IMAGE_NAME + ( m_iUnnamedImageIndex++ ).ToString();
			return name;
		}
		/// <summary>
		/// Raises ContextChoiceClose event.
		/// </summary>
		protected virtual void RaiseCloseEvent()
		{
			if( null != ContextChoiceClose )
			{
				ContextChoiceClose( this, m_form.DialogResult );
			}
		}
		#endregion

		#region Keyboard Handling
		/// <summary>
		/// 
		/// </summary>
		/// <param name="keys"></param>
		protected internal virtual void ProccessKeys( KeyEventArgs keys )
		{
			if( this.IsVisible )
			{
				if( keys.KeyCode == Keys.ControlKey )
				{
					keys.Handled = true;
				}

				switch( keys.KeyData )
				{
					case Keys.Control:
					case Keys.ControlKey:
					case Keys.Alt:
						{
							keys.Handled = true;
							break;
						}

					//case Keys.Tab:
					case Keys.Control | Keys.Space:
					case Keys.Alt | Keys.Right:
						{
							m_control.AcceptAutoComplete();
							keys.Handled = true;
							break;
						}

					case Keys.Escape:
					case Keys.Back:
						{
							m_control.DeclineAutoComplete();
							break;
						}
				}

				if( !keys.Handled )
				{
					m_form.ProccessKeys( keys );
				}
			}
		}
		/// <summary>
		/// Returns common start part of two strings.
		/// </summary>
		/// <param name="s1">First string.</param>
		/// <param name="s2">Second string.</param>
		/// <returns>Common start part of two srings.</returns>
		private string GetCommonPart( string s1, string s2 )
		{
			StringBuilder result = new StringBuilder();

			int len = Math.Min( s1.Length, s2.Length );
			for( int i = 0 ; i < len ; i++ )
			{
				if( s1[ i ] != s2[ i ] )
				{
					break;
				}

				result.Append( s1[ i ] );
			}

			return result.ToString();
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Raises ContextChoiceClose event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFormClosed( object sender, EventArgs e )
		{
			m_control.DeclineAutoComplete();
			RaiseCloseEvent();
		}
		/// <summary>
		/// Redirects form loading event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFormLoad( object sender, EventArgs e )
		{
			if( null != FormLoad )
			{
				FormLoad( sender, e );
			}
		}
		/// <summary>
		/// Updates reference to the selected item.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFormItemSelected( IContextChoiceController sender, ContextChoiceItemSelectedEventArgs e )
		{
			if( e.SelectedItem != null )
			{
				m_itemSelected = ( ContextChoiceItem )m_items[ e.SelectedItem.ID ];
			}
			else
			{
				m_itemSelected = null;
			}

			if( ItemSelected != null )
			{
				ItemSelected( this, e );
			}
		}
		/// <summary>
		/// Raises ContextChoiceRightClick event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnItemsViewMouseDown( object sender, MouseEventArgs e )
		{
			if( ContextChoiceRightClick != null )
			{
				if( e.Button == MouseButtons.Right )
				{
					TreeView tree = m_form.ItemsView as TreeView;
					if( tree != null )
					{
						ContextChoiceItem item = tree.GetNodeAt( e.X, e.Y ) as ContextChoiceItem;
						if( item != null )
						{
							ContextChoiceRightClick( this, new ContextChoiceItemEventArgs( item ) );
						}
					}
				}
			}
		}
		/// <summary>
		/// Raises Closing event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFormClosing( object sender, CancelEventArgs e )
		{
			if( ContextChoiceBeforeClosing != null )
			{
				ContextChoiceBeforeClosing( this, EventArgs.Empty );
			}
		}
		#endregion
	}
}