#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Any infringement will be prosecuted under
//  applicable laws. 
//
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Xml;

namespace Syncfusion.GridHelperClasses
{
	/// <summary>
	/// Summary description for UserControl1.
	/// </summary>
	public class RichTextBoxSupportsXHTML : System.Windows.Forms.RichTextBox
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private ArrayList errors = new ArrayList();

		/// <summary>
		/// Any errors encountered during XHTML-to-RTF translation will be contained
		/// in this ArrayList of strings.
		/// </summary>
		public ArrayList Errors
		{
			get
			{
				return errors;
			}
		}

		/// <summary>
		/// Setting this read-only property initiates the XHTML-to-RTF translation and
		/// then sets the box to display the resulting Rich Text. Any errors during
		/// translation can be viewed in the Errors public property.
		/// </summary>
		public string Xhtml
		{
			set
			{
				XmlTranslator xmlTranslator;

				// Sending a string of XHTML to the XmlTranslator constructor
				// creates a DOM of the XHTML in the XmlTranslator.
				xmlTranslator  = new XmlTranslator(value);

				// The XmlTranslator translates the XHTML into RTF code
				// wrapped in an RtfDocument object.
				this.Rtf = xmlTranslator.ToRtfDocument().ToString();

				// Any errors are passed from the translator to this text box.
				errors = xmlTranslator.Errors;
			}
		}
        /// <summary>
        /// RichTextBoxSupportsXHTML 
        /// </summary>
        public RichTextBoxSupportsXHTML(): base()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			this.ReadOnly = true;

			// TODO: Add any initialization after the InitComponent call
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
	}
}
