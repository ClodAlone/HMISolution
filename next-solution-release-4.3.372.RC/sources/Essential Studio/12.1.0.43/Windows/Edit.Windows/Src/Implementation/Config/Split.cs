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
using System.Xml.Serialization;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Edit.Implementation.Config
{
	/// <summary>
	/// Class representing one split element.
	/// </summary>
	[ XmlRoot( "split" ) ]
	public class Split
	{
		#region Fields
		/// <summary>
		/// Split text.
		/// </summary>
		private string m_strText;
		/// <summary>
		/// Indicates whether split text is regular expression.
		/// </summary>
		private bool m_bIsRegex;
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets or sets split text.
		/// </summary>
		[ XmlText ]
		public string Text
		{
			get
			{
				return m_strText;
			}
			set
			{
				m_strText = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether split text is regular expression.
		/// </summary>
		[ XmlAttribute ]
		[ DefaultValue( false ) ]
		public bool IsRegex
		{
			get
			{
				return m_bIsRegex;
			}
			set
			{
				m_bIsRegex = value;
			}
		}
		#endregion

		#region Class Initialization
		/// <summary>
		/// Default constructor. Creates new instance of splitter.
		/// </summary>
		public Split()
		{
		}
		/// <summary>
		/// Creates and initializes new instance of splitter.
		/// </summary>
		/// <param name="text">Specifies splitter text.</param>
		/// <param name="isRegex">Specifies whether splitter is regex.</param>
		public Split( string text, bool isRegex )
		{
			m_strText = text;
			m_bIsRegex = isRegex;
		}
		/// <summary>
		/// Creates and initializes new instance of the non-regex splitter.
		/// </summary>
		/// <param name="text">Specifies splitter text.</param>
		public Split( string text )
			: this( text, false )
		{
		}
		#endregion
	}
}