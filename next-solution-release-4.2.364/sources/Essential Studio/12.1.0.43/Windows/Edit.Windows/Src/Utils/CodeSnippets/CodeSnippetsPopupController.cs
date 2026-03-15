#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Forms.Popup;

namespace Syncfusion.Windows.Forms.Edit.Utils.CodeSnippets
{
	/// <summary>
	/// Coltroller for code snippets popup form.
	/// </summary>
	internal class CodeSnippetsPopupController
		: ContextChoiceController
	{
		#region Constants
		/// <summary>
		/// 
		/// </summary>
		private const string DEF_SNIPPET_IMAGE_NAME = "snippet";
		/// <summary>
		/// 
		/// </summary>
		private const string DEF_CONTAINER_IMAGE_NAME = "container";
		/// <summary>
		/// 
		/// </summary>
		private const string DEF_UPLEVEL_IMAGE_NAME = "uplevel";
		#endregion

		#region Fields
		/// <summary>
		/// Current code snippets container.
		/// </summary>
		private CodeSnippetsContainer m_curContainer;
		/// <summary>
		/// 
		/// </summary>
		private CodeSnippetsEditBox m_editBox;
		#endregion

		#region Properties
		/// <summary>
		/// Gets current snippets container.
		/// </summary>
		public CodeSnippetsContainer CurrentContainer
		{
			get
			{
				return m_curContainer;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates and initializes new instance of CodeSnippetsPopupController.
		/// </summary>
		/// <param name="control">StreamEditControl.</param>
		/// <param name="editBox">CodeSnippetsEditBox.</param>
		public CodeSnippetsPopupController( StreamEditControl control, CodeSnippetsEditBox editBox )
			: base( control, false )
		{
			if( editBox == null ) throw new ArgumentNullException( "editForm" );

			m_editBox = editBox;
			editBox.KeyDown += new KeyEventHandler( OnSnippetsEditBoxEditBoxKeyDown );
      editBox.CurrentTextChanged += new EventHandler( OnSnippetsEditBoxCurrentTextChanged );

			AddImage( DEF_SNIPPET_IMAGE_NAME, Image.FromStream( Assembly.GetExecutingAssembly().GetManifestResourceStream(
				"Syncfusion.Windows.Forms.Edit.Images.CodeSnippet.bmp" ) ), Color.White );
			AddImage( DEF_CONTAINER_IMAGE_NAME, Image.FromStream( Assembly.GetExecutingAssembly().GetManifestResourceStream(
				"Syncfusion.Windows.Forms.Edit.Images.CodeSnippetsContainer.bmp" ) ), Color.White );
			AddImage( DEF_UPLEVEL_IMAGE_NAME, Image.FromStream( Assembly.GetExecutingAssembly().GetManifestResourceStream(
				"Syncfusion.Windows.Forms.Edit.Images.CodeSnippetUpLevel.bmp" ) ), Color.White );
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Activates controller.
		/// </summary>
		/// <param name="snippetsContainer">Container to activate.</param>
		public void Activate( CodeSnippetsContainer snippetsContainer )
		{
			this.Items.Clear();

			foreach( string name in snippetsContainer.ContainersNames )
			{
				IContextChoiceItem item = Items.Add( name, this.Images[ DEF_CONTAINER_IMAGE_NAME ] );
				item.Type = ContextChoiceItemType.CodeSnippetsContainer;
			}

			foreach( CodeSnippet snippet in snippetsContainer )
			{
				IContextChoiceItem item = Items.Add( snippet.Title, snippet.Description, this.Images[ DEF_SNIPPET_IMAGE_NAME ] );
				item.Type = ContextChoiceItemType.CodeSnippet;
			}

			m_curContainer = snippetsContainer;
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Processes keys.
		/// </summary>
		/// <param name="keys">Keys to process.</param>
		protected internal override void ProccessKeys( System.Windows.Forms.KeyEventArgs keys )
		{
			switch( keys.KeyData )
			{
				case Keys.Enter :
				case Keys.Tab :
				{
					keys.Handled = ProcessItemActivation();
					break;
				}
			}

			if( !keys.Handled )	base.ProccessKeys (keys);
		}
		/// <summary>
		/// Proforms form creation-specific operations.
		/// </summary>
		/// <param name="form">Created form.</param>
		protected override void OnNewFormCreated( Syncfusion.Windows.Forms.Edit.Forms.Popup.ContextChoice form )
		{
			base.OnNewFormCreated (form);

      form.ProcessDblClick = false;
			form.ItemsView.DoubleClick += new EventHandler( OnFormDoubleClick );
		}
		/// <summary>
		/// Updates form.
		/// </summary>
		protected override void UpdateFormInternal()
		{
			m_form.UpdateNodesList( m_editBox.CurrentText.ToLower() );
		}
		/// <summary>
		/// Creates new ContextChoice form.
		/// </summary>
		/// <returns>ContextChoice form.</returns>
		protected override ContextChoice GetContextChoiceForm()
		{
			return new ContextChoice( m_images, m_items, m_control, true, m_editBox, false, false, FormSize);
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Processes activation of popup item (by keyboard or mouse).
		/// </summary>
		/// <returns>True if container was activated; otherwise - false.</returns>
		private bool ProcessItemActivation()
		{
			bool result = false;

			if( this.SelectedItem != null )
			{
				switch( this.SelectedItem.Type )
				{
					case ContextChoiceItemType.CodeSnippetsContainer :
					{
						string name = this.SelectedItem.Text;
						Activate( m_curContainer.GetContainerByName( name ) );
						m_editBox.AddLabel( name );
						result = true;
						break;
					}
				}
			}
			else
			{
				result = true;
			}

			return result;
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Handles mouse double click.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFormDoubleClick(object sender, EventArgs e)
		{
			if( !ProcessItemActivation() )
			{
				m_form.BeginInvoke( new MethodInvoker( m_form.CloseOk ) );
			}
		}

        /// <summary>
        /// Raises ContextChoiceClose event.
        /// </summary>
        protected override void RaiseCloseEvent()
        {
            base.RaiseCloseEvent();
            this.m_editBox.Visible = false;
            Form mainForm = this.m_editBox.m_parent.TopLevelControl as Form;
            if (mainForm != null)
            {
                mainForm.Activate();
            }
        }
                
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSnippetsEditBoxEditBoxKeyDown(object sender, KeyEventArgs e)
		{
      ProccessKeys( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSnippetsEditBoxCurrentTextChanged(object sender, EventArgs e)
		{
            if (this.IsVisible)
                UpdateFormInternal();
		}
		#endregion
	}
}