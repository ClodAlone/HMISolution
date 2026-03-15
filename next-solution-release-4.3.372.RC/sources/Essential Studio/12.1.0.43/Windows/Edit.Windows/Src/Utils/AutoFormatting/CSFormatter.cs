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
using System.Text;

using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Enums;

namespace Syncfusion.Windows.Forms.Edit.Utils.AutoFormatting
{
	/// <summary>
	/// Autoformatter for CSharp.
	/// </summary>
	public class CSFormatter
		: IAutoFormatter
	{
		#region Internal Classes
		/// <summary>
		/// Type of indentation. Used for work with indentation stack.
		/// </summary>
		private enum IndentType
		{
			/// <summary>
			/// Lines are indented until some indentation closing lexem is found.
			/// </summary>
			Permanent,
			/// <summary>
			/// Only one line is indented.
			/// </summary>
			Single
		}
		/// <summary>
		/// Data to be stored in indentation stack.
		/// </summary>
		private class IndentStackDatum
		{
			#region Class Public Members
			/// <summary>
			/// ????
			/// </summary>
			public IndentType IndentType;
			/// <summary>
			///	?????
			/// </summary>
			public string LexemText;
			#endregion

			#region Class Initialization
			/// <summary>
			/// Creates and initializes new instance of IndentStackDatum
			/// </summary>
			/// <param name="indentType">Type of indentation.</param>
			/// <param name="strLexemText">Text of indentation lexem.</param>
			public IndentStackDatum( IndentType indentType, string strLexemText )
			{
				if( null == strLexemText )
					throw new ArgumentNullException( "strLexemText" );

				this.IndentType = indentType;
				this.LexemText = strLexemText;
			}
			#endregion
		}
		/// <summary>
		/// Indentation stack.
		/// </summary>
		private class IndentStack
			: Stack
		{
			#region Class Overrides
			/// <summary>
			/// Peeks the stack and casts result to IndentStackDatum.
			/// </summary>
			/// <returns>Peeked object casted to IndentStackDatum.</returns>
			public new IndentStackDatum Peek()
			{
				return ( IndentStackDatum )base.Peek();
			}
			/// <summary>
			/// Pops the stack and casts result to IndentStackDatum.
			/// </summary>
			/// <returns>Popped object casted to IndentStackDatum.</returns>
			public new IndentStackDatum Pop()
			{
				return ( IndentStackDatum )base.Pop();
			}
			#endregion
		}
		#endregion

		#region IAutoFormatter Members
		/// <summary>
		/// Formats given list of lexem wrappers.
		/// </summary>
		/// <param name="lexems">List of ILexemWrapper instances.</param>
		/// <returns>String with formatted text.</returns>
		public string Format( IList lexems )
		{
			if( null == lexems )
				throw new ArgumentNullException( "lexems" );

			// Whitespace at the beginning of first line. Is inserted at the beginning of every line.
			string strStartLineWhitespace = GetStartWhitespace( lexems );

			if( 0 == lexems.Count ) return strStartLineWhitespace;

			// Number of indentation tabs to insert at the beginning of current line.
			int iIndents = 0;

			StringBuilder result = new StringBuilder( strStartLineWhitespace );
			ILexemWrapper curLex = null;

			IndentStack indentLexemsStack = new IndentStack();

			// Helper variable.
			string strLexText = string.Empty;

			do
			{
				curLex = ( ILexemWrapper )lexems[ 0 ];

				// Omit parsing of comments.
				if( FormatType.Comment == curLex.Config.Type || FormatType.String == curLex.Config.Type
					|| "CommentXML" == curLex.Config.FormatName )
				{
					AddFirstLexem( result, lexems );
				}
				// If lexem is new line mark.
				else if( -1 != curLex.Text.IndexOf( "\n" ) ) 
				{
					RemoveEndWhitespace( result, false );

					iIndents -= ClearStack( indentLexemsStack );

					AddNewLineAndIndent( result, iIndents, strStartLineWhitespace, lexems );
					lexems.RemoveAt( 0 );
					RemoveStartWhitespace( lexems, false );
				}
				else
				{
					switch( curLex.Text )
					{
						case "{" :
							RemoveEndWhitespace( result, true );
							
							if( iIndents > 0 && IndentType.Single == indentLexemsStack.Peek().IndentType )
							{
								iIndents--;
								indentLexemsStack.Pop();
							}

							AddNewLineAndIndent( result, iIndents, strStartLineWhitespace, lexems );
							AddFirstLexem( result, lexems );
							
							iIndents++;
							indentLexemsStack.Push( new IndentStackDatum( IndentType.Permanent, curLex.Text ) );

							AddNewLineAndIndent( result, iIndents, strStartLineWhitespace, lexems );
							RemoveStartWhitespace( lexems, true );
							break;

						case "}" :
							RemoveEndWhitespace( result, true );

							if( iIndents > 0 )
							{
								iIndents--;

								if( "case" == indentLexemsStack.Peek().LexemText ) iIndents--;

								indentLexemsStack.Pop();
							}

							iIndents -= ClearStack( indentLexemsStack );

							AddNewLineAndIndent( result, iIndents, strStartLineWhitespace, lexems );

							AddFirstLexem( result, lexems );

							AddNewLineAndIndent( result, iIndents, strStartLineWhitespace, lexems );
							RemoveFirstNewLineMark( lexems );
							break;

						case "if" :
						case "for" :
							// For pushing into indentation stack.
							strLexText = curLex.Text;

							AddFirstLexem( result, lexems );

							if( 0 == lexems.Count ) break;

							AddLexemsInBrackets( result, lexems );

							iIndents++;
							indentLexemsStack.Push( new IndentStackDatum( IndentType.Single, strLexText ) );

							AddNewLineAndIndent( result, iIndents, strStartLineWhitespace, lexems );
							RemoveStartWhitespace( lexems, true );
							break;

						case "case" :
							RemoveEndWhitespace( result, true );
							
							iIndents -= ClearStack( indentLexemsStack );

							if( iIndents > 0 && "case" == indentLexemsStack.Peek().LexemText )
							{
								iIndents--;
								indentLexemsStack.Pop();
								result.Append( "\n" );
							}

							AddNewLineAndIndent( result, iIndents, strStartLineWhitespace, lexems );
							AddFirstLexem( result, lexems );

							AddUntilLexem( result, lexems, ":" );
							
							iIndents++;
							indentLexemsStack.Push( new IndentStackDatum( IndentType.Permanent, curLex.Text ) );

							AddNewLineAndIndent( result, iIndents, strStartLineWhitespace, lexems );
							RemoveStartWhitespace( lexems, true );
							break;

						case "switch" :
							strLexText = curLex.Text;
							AddFirstLexem( result, lexems );

							if( 0 == lexems.Count ) break;

							AddLexemsInBrackets( result, lexems );

							iIndents++;
							indentLexemsStack.Push( new IndentStackDatum( IndentType.Single, strLexText ) );

							AddNewLineAndIndent( result, iIndents, strStartLineWhitespace, lexems );
							RemoveStartWhitespace( lexems, true );
							break;

						default:
							AddFirstLexem( result, lexems );
							break;
					}
				}
			}
			while( lexems.Count > 0 );

			return result.ToString();
		}
		#endregion

		#region Class Utility Methods
		/// <summary>
		/// Removes whitespace from the end of string builder.
		/// </summary>
		/// <param name="s">StringBuilder instance to remove whitespace from.</param>
		/// <param name="bRemoveNewLineMarks">Indicates whether new line marks should be removerd.</param>
		private void RemoveEndWhitespace( StringBuilder s, bool bRemoveNewLineMarks )
		{
			if( null == s )
				throw new ArgumentNullException( "s" );

			int iWhitespaceLength = 0;

			for( int i = s.Length - 1; i >= 0; i-- )
			{
				if(
					( ' ' == s[ i ] || '\t' == s[ i ] )
					||
					bRemoveNewLineMarks && ( '\n' == s[ i ] || '\r' == s[ i ] )
					)
				{
					iWhitespaceLength++; 
				}
				else
				{
					break;
				}
			}

			s.Remove( s.Length - iWhitespaceLength, iWhitespaceLength );
		}
		/// <summary>
		/// Adds to string builder new line mark and required indentation whitespace at the beginning of the next line.
		/// </summary>
		/// <param name="s">StringBuildet instance to work with.</param>
		/// <param name="iIndents">Number of required indents at the beginning of the next line.</param>
		/// <param name="strStartLineWhitespace">Whitespace to insert at the beginning
		/// of the next line (indentation follows it).</param>
		/// <param name="lexems">List of lexems. Used for checking whether current line is the last one.</param>
		private void AddNewLineAndIndent( StringBuilder s, int iIndents, string strStartLineWhitespace, IList lexems )
		{
			if( null == s )
				throw new ArgumentNullException( "s" );
			if( iIndents < 0 )
				throw new ArgumentOutOfRangeException( "iIndents" );
			if( null == strStartLineWhitespace )
				throw new ArgumentNullException( "strStartLineWhitespace" );

			if( 0 != s.Length )	s.Append( '\n' );

			if( IsLastLine( lexems ) ) return;

			s.Append( strStartLineWhitespace + new string( '\t', iIndents ) );
		}
		/// <summary>
		/// Removes all whitespace lexems from the beginning of lexems list.
		/// </summary>
		/// <param name="lexems">List of lexems.</param>
		/// <param name="bRemoveNewLineMarks">Indicates whether new line marks should be removerd.</param>
		private void RemoveStartWhitespace( IList lexems, bool bRemoveNewLineMarks )
		{
			ILexemWrapper lex = null;

			while( lexems.Count > 0 )
			{
				lex = ( ILexemWrapper )lexems[ 0 ];

				if( FormatType.Whitespace == lex.Config.Type || ( bRemoveNewLineMarks && -1 != lex.Text.IndexOf( "\n" ) ) )
				{
					lexems.RemoveAt( 0 );
				}
				else return;
			}
		}
		/// <summary>
		/// Gets whitespace at the beginning of first line.
		/// </summary>
		/// <param name="lexems">List of lexem wrappers.</param>
		/// <returns>String with required whitespace.</returns>
		private string GetStartWhitespace( IList lexems )
		{
			if( null == lexems )
				throw new ArgumentNullException( "lexems" );

			string result = string.Empty;
			ILexemWrapper lex = null;

			while( lexems.Count > 0 )
			{
				lex = ( ILexemWrapper )lexems[ 0 ];

				if( FormatType.Whitespace == lex.Config.Type )
				{
					result += lex.Text;
					lexems.RemoveAt( 0 );
				}
				else break;
			}

			return result;
		}
		/// <summary>
		/// Adds first lexem of list to string builder and removes it from list.
		/// </summary>
		/// <param name="s">StringBuilder instance to add lexem to.</param>
		/// <param name="lexems">List of lexems.</param>
		private void AddFirstLexem( StringBuilder s, IList lexems )
		{
			if( null == s )
				throw new ArgumentNullException( "s" );
			if( null == lexems )
				throw new ArgumentNullException( "lexems" );
			if( lexems.Count < 1 )
				throw new ArgumentException( "lexems" );

			s.Append( ( ( ILexemWrapper )lexems[ 0 ] ).Text );
			lexems.RemoveAt( 0 );
		}
		/// <summary>
		/// Removes all single indentations from the head of the stack.
		/// </summary>
		/// <param name="stack">IndentStack instance to be cleared.</param>
		/// <returns>Number of elements removed.</returns>
		private int ClearStack( IndentStack stack )
		{
			if( null == stack )
				throw new ArgumentNullException( "stack" );

			int result = 0;

			while( stack.Count > 0 && IndentType.Single == stack.Peek().IndentType )
			{
				stack.Pop();
				result++;
			}

			return result;
		}
		/// <summary>
		/// Adds lexems in brackets without changes.
		/// </summary>
		/// <param name="s">String builder to work with.</param>
		/// <param name="lexems">List of lexems.</param>
		private void AddLexemsInBrackets( StringBuilder s, IList lexems )
		{
			if( null == s )
				throw new ArgumentNullException( "s" );
			if( null == lexems )
				throw new ArgumentNullException( "lexems" );

			int iBrackets = 0;
			string curLexText = string.Empty;

			while( ( ")" != curLexText || 0 != iBrackets) && 0 != lexems.Count )
			{
				curLexText = ( ( ILexemWrapper )lexems[ 0 ] ).Text;

				if( "(" == curLexText ) iBrackets++;
				else if( ")" == curLexText ) iBrackets--;

				AddFirstLexem( s, lexems );
			}
		}
		/// <summary>
		/// Adds lexems without any change until specefied
		/// </summary>
		/// <param name="s">String builder instance to add text to.</param>
		/// <param name="lexems">List of lexems.</param>
		/// <param name="lexem">Text of lexem that should and operation.</param>
		private void AddUntilLexem( StringBuilder s, IList lexems, string lexem )
		{
			if( null == s )
				throw new ArgumentNullException( "s" );
			if( null == lexems )
				throw new ArgumentNullException( "lexems" );
			if( null == lexem )
				throw new ArgumentNullException( "lexem" );
			if( string.Empty == lexem )
				throw new ArgumentOutOfRangeException( "lexem" );

			string curLexText = string.Empty;

			while( lexem != curLexText && 0 != lexems.Count )
			{
				curLexText = ( ( ILexemWrapper )lexems[ 0 ] ).Text;
				AddFirstLexem( s, lexems );
			}
		}
		/// <summary>
		/// Checks whether given line is the last in given list of lexems..
		/// </summary>
		/// <param name="lexems">List of lexems.</param>
		/// <returns>True if current line is the last; otherwise false.</returns>
		private bool IsLastLine( IList lexems )
		{
			ILexemWrapper lex;

      if( 0 == lexems.Count ) return true;

			for( int i = 0; i < lexems.Count - 1; i++ )
			{
				lex = ( ILexemWrapper )lexems[ i ];

				if( -1 != lex.Text.IndexOf( "\n" ) || FormatType.Whitespace != lex.Config.Type ) return false;
			}

			return true;
		}
		/// <summary>
		/// Removes all whitespace until the second new line mark.
		/// </summary>
		/// <param name="lexems">List of lexems.</param>
		private void RemoveFirstNewLineMark( IList lexems )
		{
			ILexemWrapper lex = null;

			bool bFirstNewLineFound = false;

			while( lexems.Count > 0 )
			{
				lex = ( ILexemWrapper )lexems[ 0 ];

				if( FormatType.Whitespace == lex.Config.Type ) 
				{
					lexems.RemoveAt( 0 );
					continue;
				}

				if( -1 != lex.Text.IndexOf( "\n" ) && !bFirstNewLineFound )
				{
					lexems.RemoveAt( 0 );
					bFirstNewLineFound = true;
					continue;
				}

				return;
			}
		}
		#endregion
	}
}