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
using System.Collections;

using Syncfusion.Windows.Forms.Edit.Implementation.Parser;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Implementation.Config;

namespace Syncfusion.Windows.Forms.Edit.Utils.AutoFormatting
{
	/// <summary>
	/// Class for managing text autoformatting.
	/// </summary>
	public class AutoFormattingManager
	{
		#region Class Members
		/// <summary>
		/// Hashtable for storing registered formatters. Keys are members of KnownLanguages enumeration.
		/// </summary>
		private Hashtable m_hashFormatters = new Hashtable();
		/// <summary>
		/// Underlying lexem parser.
		/// </summary>
		private LexemParser m_parser;
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets or sets instance of the underlying parser.
		/// </summary>
		public LexemParser Parser
		{
			get
			{
				return m_parser;
			}
			set
			{
				if( null == value )
					throw new ArgumentNullException( "Parser" );

				m_parser = value;
			}
		}
		#endregion

		#region Class Initialization
		/// <summary>
		/// Creates new instance of AutoFormattingManager.
		/// </summary>
		public AutoFormattingManager()
		{
		}
		/// <summary>
		/// Creates and initializes new instance of AutoFormattingManager.
		/// </summary>
		/// <param name="parser">Underlying lexem parser.</param>
		public AutoFormattingManager( LexemParser parser )
		{
			if( null == parser )
				throw new ArgumentNullException( "parser" );

			m_parser = parser;
		}
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Registers new formatter for certain language.
		/// </summary>
		/// <param name="lang">Language to register formatter for.</param>
		/// <param name="formatter">IAutoFormatter instance.</param>
		public void RegisterFormatter( KnownLanguages lang, IAutoFormatter formatter )
		{
			if( null == formatter )
				throw new ArgumentNullException( "formatter" );

			m_hashFormatters[ lang ] = formatter;
		}
		/// <summary>
		/// Unregisters formatter for certain language.
		/// </summary>
		/// <param name="lang">Language to unregister formatter for.</param>
		public void UnregisterFormatter( KnownLanguages lang )
		{
			m_hashFormatters.Remove( lang );
		}
		/// <summary>
		/// Formats given range of text using registered language formatter. Range limits can be changed.
		/// If there's no formatter registered for the language, throws an exception.
		/// </summary>
		/// <param name="startPoint">Start point of text range to format.</param>
		/// <param name="endPoint">End point of text range to format.</param>
		/// <returns>String with formatted text.</returns>
		public string FormatText( IParsePoint startPoint, IParsePoint endPoint )
		{
			if( null == startPoint )
				throw new ArgumentNullException( "startPoint" );
			if( null == endPoint )
				throw new ArgumentNullException( "endPoint" );
			if( startPoint.Offset > endPoint.Offset )
				throw new ArgumentException();

			KnownLanguages lang = ( ( ConfigLanguage )m_parser.Formats ).KnownLanguage;

			IAutoFormatter formatter = m_hashFormatters[ lang ] as IAutoFormatter;

			if( null == formatter )
				throw new Exception( "Current language has no formatter registered." );

			CoordinatePoint startCoorPoint = m_parser.GetCoordinatePoint( startPoint );

			LexemLine firstLineInstance = ( LexemLine )m_parser.GetLine( startCoorPoint.VirtualLine );
			LexemParser.LexemParserEnumerator enumerator = ( LexemParser.LexemParserEnumerator )m_parser.GetEnumerator(
				firstLineInstance.LineStartStack, startPoint );

			int iLinesCount = endPoint.Line - startPoint.Line + 1;

			// Don't format last line if ending point points to it's beginning.
			if( 1 == endPoint.Position ) iLinesCount--;

			int iProcessedLines = 0;

			Lexem lexem = null;

			IList lexemWrappers = new ArrayList();
			string strNewLine = m_parser.BaseStream.NewLineStr;

			do
			{
				if( enumerator.MoveNext() )
				{
					lexem = ( Lexem )enumerator.Current;
					LexemWrapper wrapper = new LexemWrapper( lexem, enumerator.CurrentStack );
					lexemWrappers.Add( wrapper );

					if( lexem.Text == strNewLine ) iProcessedLines++;
				}
				else break;
			}
			while( iProcessedLines < iLinesCount );

			return formatter.Format( lexemWrappers );
		}
		#endregion
	}
}