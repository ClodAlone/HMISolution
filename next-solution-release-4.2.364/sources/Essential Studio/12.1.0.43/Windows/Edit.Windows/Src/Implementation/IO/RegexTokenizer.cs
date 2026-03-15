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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Syncfusion.Windows.Forms.Edit.Implementation.Config;

namespace Syncfusion.IO
{
	#region *** NewLineStyle
	/// <summary>
	/// Represents new line in different OS
	/// </summary>
	public enum NewLineStyle
	{
		/// <summary>
		/// \r\n.
		/// </summary>
		Windows,
		/// <summary>
		/// \r.
		/// </summary>
		Mac,
		/// <summary>
		/// \n\r
		/// </summary>
		Unix,
		/// <summary>
		/// \n
		/// </summary>
		Control
	}
	#endregion

	#region *** RegexTokenizer
	/// <summary>
	/// Tokenizer that works with RegExes.
	/// </summary>
	public class RegexTokenizer
	{
		#region Classes
		/// <summary>
		/// 
		/// </summary>
		private class LengthComparer
			: IComparer
		{
			#region IComparer Members
			/// <summary>
			/// Compares two splits by their length, if it is equal, than calls standart comparision.
			/// </summary>
			/// <param name="x">First string.</param>
			/// <param name="y">Second string.</param>
			/// <returns>Standart comparision result.</returns>
			public int Compare( object x, object y )
			{
				string str1 = ( ( Split )x ).Text;
				string str2 = ( ( Split )y ).Text;
				int result = -str1.Length.CompareTo( str2.Length );
				if( result == 0 )
				{
					result = str1.CompareTo( str2 );
				}
				return result;
			}
			#endregion
		}
		#endregion

		#region Constants
		/// <summary>
		/// Default one-char splitters.
		/// </summary>
		public const string DEF_TOKEN_SPLITS = "!\"#$%&'()*+,-./:;<=>?[\\]^{|}~`";
		/// <summary>
		/// Regular expression pattern for new-line style detection.
		/// </summary>
		public const string DEF_NEW_LINE_DETECTION_PATTERN = "(\n\r)|(\r\n)|(\n)|(\r)";
		/// <summary>
		/// Length comparer.
		/// </summary>
		private readonly IComparer DEF_COMPARER = new LengthComparer();
		#endregion

		#region Fields
		/// <summary>
		/// Current compiled regular expression.
		/// </summary>
		/// <remarks>
		/// Must be set to null by any change of splitters, and recreated later as needed.
		/// </remarks>
		private Regex m_regex;
		/// <summary>
		/// Reader for the underlying stream.
		/// </summary>
		/// <remarks>
		/// Note: reader has it's own cache, so on every change of position cache must be resetted.
		/// </remarks>
		private StreamReader m_reader;
		/// <summary>
		/// Last match of the RegEx.
		/// </summary>
		private Match m_match;
		/// <summary>
		/// Current line cache.
		/// </summary>
		/// <remarks>
		/// Line can be loaded to cache just partialy.
		/// </remarks>
		private string m_line;
		/// <summary>
		/// Current position in stream.
		/// </summary>
		private long m_position;
		/// <summary>
		/// EndLine symbol(s).
		/// </summary>
		private string m_strEndLineString = "\n";
		/// <summary>
		/// Storage of EndLineStyle property
		/// </summary>
		private NewLineStyle m_style = NewLineStyle.Control;
        /// <summary>
        /// 
        /// </summary>
        internal static bool SetRegexAsNone = false;
		/// <summary>
		/// Arrays of multi-char splitters.
		/// </summary>
		private Split[] m_multiCharSplits = null;
		/// <summary>
		/// Arrays of multi-char splitters.
		/// </summary>
		private string m_oneCharSplits = DEF_TOKEN_SPLITS;
		/// <summary>
		/// Specifies case sensitivity of the tokenizer.
		/// </summary>
		private bool m_bCaseSensitive;
		/// <summary>
		/// Count of bytes in the beginning of the stream, that must be skipped (preambula).
		/// </summary>
		private int m_iSkippedBytes = -1;
		/// <summary>
		/// Position of the cached token.
		/// </summary>
		private long m_cachePosition = -1;
		/// <summary>
		/// Cached token.
		/// </summary>
		private string m_cacheToken = null;
		/// <summary>
		/// Indicated whether new line style was detected.
		/// </summary>
		private bool m_bNewLineStyleDetected = false;
		#endregion

		#region Properties
		/// <summary>
		/// Gets stream reader used by the tokenizer.
		/// </summary>
		public StreamReader Reader
		{
			get
			{
				return m_reader;
			}
		}
		/// <summary>
		/// Gets stream length.
		/// </summary>
		public long Length
		{
			get
			{
				return m_reader.BaseStream.Length;
			}
		}
		/// <summary>
		/// Gets current compiled regular expression, used to parse stream.
		/// </summary>
		public Regex Expression
		{
			get
			{
				if( m_regex == null )
				{
					m_regex = BuildRegex();
				}
				return m_regex;
			}
		}
		/// <summary>
		/// Gets or sets array of multi-char tokens.
		/// </summary>
		public Split[] MultiCharTokens
		{
			get
			{
				return m_multiCharSplits;
			}
			set
			{
				if( m_multiCharSplits != value )
				{
					m_multiCharSplits = value;

					ResetRegEx();
				}
			}
		}
		/// <summary>
		/// Gets or sets string, that is treated as an array of the one-char delimiters.
		/// </summary>
		public string OneCharTokens
		{
			get
			{
				return m_oneCharSplits;
			}
			set
			{
				if( m_oneCharSplits != value )
				{
					m_oneCharSplits = value;
					ResetRegEx();
				}
			}
		}
		/// <summary>
		/// Gets or sets current stream position.
		/// </summary>
		public long Position
		{
			get
			{
				return m_position;
			}
			set
			{
				if( m_position != value )
				{
					m_position = value;
					ResetBuffer(true);
				}
			}
		}
		/// <summary>
		/// Gets or sets case sensitivity of the tokenizer.
		/// </summary>
		public bool CaseSensitive
		{
			get
			{
				return m_bCaseSensitive;
			}
			set
			{
				if( m_bCaseSensitive != value )
				{
					m_bCaseSensitive = value;
					ResetRegEx();
				}
			}
		}
		/// <summary>
		/// Gets currently used encoding.
		/// </summary>
		public Encoding Encoding
		{
			get
			{
				return m_reader.CurrentEncoding;
			}
		}
		/// <summary>
		/// Gets or sets new line string.
		/// </summary>
		public virtual string NewLine
		{
			get
			{
				return m_strEndLineString;
			}
			set
			{
				if( m_strEndLineString != value )
				{
					m_strEndLineString = value;
					ResetRegEx();
				}
			}
		}
		/// <summary>
		/// Gets or sets end line style (for different OS).
		/// </summary>
		public virtual NewLineStyle EndLineStyle
		{
			get
			{
				return m_style;
			}
			set
			{
				if( m_style != value )
				{
					this.NewLine = RegexTokenizer.GetNewLineString( value );
					m_style = value;
					m_bNewLineStyleDetected = true;
				}
			}
		}
		/// <summary>
		/// Gets count of skipped bytes at the beginning of the file. Skipped bytes - size of the preambula for encoding.
		/// </summary>
		public int SkipBytes
		{
			get
			{
				if( m_iSkippedBytes == -1 )
				{
					m_iSkippedBytes = this.Encoding.GetPreamble().Length;
				}
				return m_iSkippedBytes;
			}
		}
		/// <summary>
		/// Gets bool indicating whether new line style was detected.
		/// </summary>
		public bool NewLineStyleDetected
		{
			get
			{
				return m_bNewLineStyleDetected;
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Method convert enumeration to it string representation.
		/// </summary>
		/// <param name="value">Value to convert.</param>
		/// <returns>End line string.</returns>
		public static string GetNewLineString( NewLineStyle value )
		{
			switch( value )
			{
				case NewLineStyle.Windows: return "\r\n";
				case NewLineStyle.Mac: return "\r";
				case NewLineStyle.Unix: return "\n";
				case NewLineStyle.Control: return "\n\r";
			}

			throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_156 );
		}
		/// <summary>
		/// Creates compiled regular expression, used for text parsing.
		/// </summary>
		/// <returns>Newly created regular expression.</returns>
		public Regex BuildRegex()
		{
			StringBuilder regexPattern = new StringBuilder();
			if( m_multiCharSplits != null && m_multiCharSplits.Length > 0 )
			{
				regexPattern.Append( '(' );

				ArrayList lst = new ArrayList( m_multiCharSplits );
				lst.Sort( DEF_COMPARER );

				string[] multiCharSplits = new string[ m_multiCharSplits.Length ];
                if (multiCharSplits.Length == 9)
                {
                    for (int i = 0; i < lst.Count; i++)
                    {
                        Split split = (Split)lst[i];
                        multiCharSplits[i] = (split.IsRegex) ? (split.Text) : Regex.Escape(split.Text);
                    }
                }
                else
                {
                    for (int i = 0; i < lst.Count; i++)
                    {
                        Split split = (Split)lst[i];
                        if (!split.Text.Equals("//") && !split.Text.Equals("(*") && !split.Text.Equals("<?") && !split.Text.Equals("?>") && !split.Text.Equals("*)") && !split.Text.Equals("'") && !split.Text.Equals("--") && !split.Text.Equals("<!--") && !split.Text.Equals("-->") && !split.Text.Equals("/*") && !split.Text.Equals("*/"))
                            multiCharSplits[i] = (split.IsRegex) ? (split.Text) : (string.Format(@"{0}\b", Regex.Escape(split.Text)));
                        else
                            multiCharSplits[i] = (split.IsRegex) ? (split.Text) : Regex.Escape(split.Text);
                    }
                }

				regexPattern.Append( string.Join( "|", multiCharSplits ) );
				regexPattern.Append( ")|" );
			}
            regexPattern.Append(@"(\p{IsArabic}+)|");
			regexPattern.Append( '(' );
			regexPattern.Append( Regex.Escape( m_strEndLineString ) );
			regexPattern.Append( ")|(" );
			regexPattern.Append( Regex.Escape( m_oneCharSplits ) );
			regexPattern.Append( @")|([ ]+)|([\t])|(\w+)|(.)|(\n|\r)" );

			string str = regexPattern.ToString();
            RegexOptions options;

            if (SetRegexAsNone)
                options = RegexOptions.None;
            else
                options = RegexOptions.Compiled;

            if (!m_bCaseSensitive)
            {
                options |= RegexOptions.IgnoreCase;
            }
			Regex result = new Regex( str, options );
			return result;
		}
		/// <summary>
		/// Reads string from stream.
		/// </summary>
		/// <param name="bytesCount">Count of bytes to read.</param>
		/// <returns>String read from stream.</returns>
		public string ReadString( int bytesCount )
		{
			if( bytesCount < 0 ) throw new ArgumentOutOfRangeException( "bytesCount" );

			string result = string.Empty;
			if( bytesCount > 0 )
			{
				byte[] array = new byte[ bytesCount ];
				int readBytes = m_reader.BaseStream.Read( array, 0, bytesCount );
				if( readBytes > 0 )
				{
					m_position += readBytes;
				}
				result = m_reader.CurrentEncoding.GetString( array, 0, readBytes );
			}

			return result;
		}
		/// <summary>
		/// Resets current compiled regular expression and all buffered data.
		/// </summary>
		public void ResetRegEx()
		{
			ResetBuffer( true );
			m_regex = null;
		}
		/// <summary>
		/// Discards all buffered data.
		/// </summary>
		/// <param name="bCorrectPosition">Specifies whether stream position must be set to currently calculated.</param>
		public void ResetBuffer( bool bCorrectPosition )
		{
			m_reader.DiscardBufferedData();
			m_match = null;
			m_line = null;
			if( bCorrectPosition )
			{
				m_reader.BaseStream.Position = m_position;
			}
			else
			{
				m_position = m_reader.BaseStream.Position;
			}
			m_cacheToken = null;
		}
		/// <summary>
		/// Reads line starting from the current position.
		/// </summary>
		/// <returns>Text line.</returns>
		public string ReadLine()
		{
			string result = ReadLineInternal();
			if( result != null )
			{
				m_position += m_reader.CurrentEncoding.GetByteCount( result );
			}
			return result;
		}
		/// <summary>
		/// Reads line starting from the current position.
		/// </summary>
		/// <returns>Text line.</returns>
		protected string ReadLineInternal()
		{
			ResetBuffer( true );
			StringBuilder result = new StringBuilder();
			char firstEndingChar = m_strEndLineString[ 0 ];
			int charsNeeded = m_strEndLineString.Length;
			while( true )
			{
				int readData = m_reader.Read();

				if( readData == -1 )
				{
					break;
				}

				char oneChar = ( char )readData;
				result.Append( oneChar );
				if( firstEndingChar == oneChar )
				{
					if( charsNeeded == 1 )
					{
						break;
					}

					readData = m_reader.Read();

					if( readData == -1 )
					{
						break;
					}

					oneChar = ( char )readData;
					result.Append( oneChar );
					if( oneChar == m_strEndLineString[ 1 ] )
					{
						break;
					}
				}
			}
			return ( result.Length == 0 ) ? ( null ) : ( result.ToString() );
		}
		/// <summary>
		/// Reads one token.
		/// </summary>
		/// <returns>Token.</returns>
		public string ReadToken()
		{
			long pos = m_position;
			string result = PeekToken();
			if( result != null )
			{
				m_position = pos + this.Encoding.GetByteCount( result );
			}
			return result;
		}
		/// <summary>
		/// Reads one token.
		/// </summary>
		/// <returns>Token.</returns>
		public string PeekToken()
		{
			string result = null;
			if( m_position == m_cachePosition && m_cacheToken != null )
			{
				result = m_cacheToken;
			}
			else
			{
				long pos = m_position;
				m_cacheToken = null;
				if( m_match == null || !m_match.Success )
				{
					m_line = ReadLineInternal();
					m_match = null;
				}

				if( m_line != null )
				{
					if( m_match == null )
					{
						m_match = this.Expression.Match( m_line );
					}
					else
					{
						m_match = m_match.NextMatch();
					}

					if( !m_match.Success && m_line != string.Empty )
					{
						result = PeekToken();
					}
					else
					{
						m_cachePosition = pos;
						m_cacheToken = m_match.Value;
						if( ( m_match.Length + m_match.Index ) == m_line.Length )
						{
							m_match = null;
						}
						result = m_cacheToken;
					}
				}
			}

			return result;
		}
		/// <summary>
		/// Tries to detect NewLine symbol.
		/// </summary>
		/// <returns>Detected new-line style, or Windows default if detection failed.</returns>
		public string DetectFileNewLineStyle()
		{
			long lOldPos = m_reader.BaseStream.Position;
			m_reader.BaseStream.Position = 0;
			m_reader.DiscardBufferedData();
			string endLineString = string.Empty;
			char[] buffer = new char[ 8192 ];
			Regex searchRegex = new Regex( DEF_NEW_LINE_DETECTION_PATTERN, RegexOptions.Compiled );
			do
			{
				int iLength = m_reader.Read( buffer, 0, 8192 );
				if( iLength == 0 )
				{
					break;
				}

				string text = new string( buffer, 0, iLength );
				Match match = searchRegex.Match( text );
				if( match.Success )
				{
					string foundtext = match.Value;
					// If we are at the end of buffer
					if( match.Index == iLength - 1 )
					{
						int readData = m_reader.Read();
						if( readData != -1 )
						{
							char oneChar = ( char )readData;
							foundtext += oneChar;
							match = searchRegex.Match( foundtext );
							if( match.Success )
							{
								foundtext = match.Value;
							}
						}
					}
					endLineString = foundtext;
					break;
				}
			}
			while( true );

			m_reader.BaseStream.Position = lOldPos;
			m_reader.DiscardBufferedData();
			return endLineString;
		}
		/// <summary>
		/// Closes reader.
		/// </summary>
		public void Close()
		{
			m_reader.Close();
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates new instance of the tokenizer, detects new-line style and save stream's position.
		/// </summary>
		/// <param name="input">Input stream.</param>
		/// <param name="encoding">Encoding to use.</param>
		public RegexTokenizer( Stream input, Encoding encoding )
		{
			if( encoding == null )
			{
				m_reader = new StreamReader( input, Encoding.ASCII, true );
			}
			else
			{
				m_reader = new StreamReader( input, encoding, false );
			}

			string strNewLine = DetectFileNewLineStyle();

			if( strNewLine == string.Empty )
			{
				strNewLine = RegexTokenizer.GetNewLineString( NewLineStyle.Control );
			}
			else
			{
				m_bNewLineStyleDetected = true;
			}

			m_strEndLineString = strNewLine;
			m_position = input.Position;
		}
		/// <summary>
		/// Creates new instance of the tokenizer, detects new-line style and save stream's position.
		/// </summary>
		/// <param name="input">Input stream.</param>
		public RegexTokenizer( Stream input )
			: this( input, null )
		{
		}
		#endregion
	}
	#endregion
}