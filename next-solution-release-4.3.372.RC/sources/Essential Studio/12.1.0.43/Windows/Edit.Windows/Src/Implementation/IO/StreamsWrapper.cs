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
using Syncfusion.IO;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Implementation.Config;

namespace Syncfusion.Windows.Forms.Edit.Implementation.IO
{
	/// <summary>
	/// Class, that manages cooperational work of the ChangesStream, RegexTokenizer and ParsePointManager.
	/// </summary>
	public class StreamsWrapper
	{
		#region Constants
		/// <summary>
		/// Stream buffer used for optimizing speed of read/write operations
		/// </summary>
		protected const int DEF_BUFFER_SIZE = 8192;
		/// <summary>
		/// Name of the data group for search.
		/// </summary>
		public const string DEF_SEARCH_DATA_GROUP = "Grp_4A719BA8B7594cb08B1779B91851604D";
		/// <summary>
		/// Number of lines to skip for to insert new parse poimnt.
		/// </summary>
		private const int DEF_LINES_TO_SKIP = 20;
		#endregion

		#region Fields
        /// <summary>
        /// Search the text like in visual studio editor
        /// </summary>
        private bool m_likeVisualStudioSearch = false;
		/// <summary>
		/// Input stream for RegexTokenizer.
		/// </summary>
		private ChangesStream m_changesStream;
		/// <summary>
		/// Token stream, used to read stream by tokens.
		/// </summary>
		private RegexTokenizer m_tokenStream;
		/// <summary>
		/// Input stream, that must be closed later.
		/// </summary>
		private Stream m_inputStream;
		/// <summary>
		/// Manager of the ParsePoints.
		/// </summary>
		private ParsePointManager m_parsePointManager;
		/// <summary>
		/// Count of lines in file.
		/// </summary>
		private int m_linesCount = -1;
		/// <summary>
		/// Current version of the wrapper.
		/// </summary>
		private int m_version;
		/// <summary>
		/// Stack for the undo.
		/// </summary>
		private Stack m_undoStack = new Stack();
		/// <summary>
		/// Stack for the redo.
		/// </summary>
		private Stack m_redoStack = new Stack();
		#endregion

		#region Properties

        /// <summary>
        /// Gets or sets the search the text like in visual studion editor 
        /// </summary>
        public bool LikeVisualStudioEditorSearch
        {
            get
            {
                return this.m_likeVisualStudioSearch;
            }
            set
            {
                if (this.m_likeVisualStudioSearch != value)
                {
                    this.m_likeVisualStudioSearch = value;
                }
            }
        }

		/// <summary>
		/// Gets current version of the wrapper. It is incremented on every insert, delete or replace.
		/// </summary>
		public int Version
		{
			get
			{
				return m_version;
			}
		}
		/// <summary>
		/// Gets count of the ParsePoints.
		/// </summary>
		public int ParsePointsCount
		{
			get
			{
				return m_parsePointManager.Count;
			}
		}
		/// <summary>
		/// Gets or sets current position in the stream.
		/// </summary>
		public long Position
		{
			get
			{
				return m_tokenStream.Position;
			}
			set
			{
				m_tokenStream.Position = value;
			}
		}
		/// <summary>
		/// Gets total length of the stream.
		/// </summary>
		public long Length
		{
			get
			{
				return m_tokenStream.Length;
			}
		}
		/// <summary>
		/// Gets value indicating whether we have reached end of file.
		/// </summary>
		public bool IsEOF
		{
			get
			{
				return ( this.Position >= this.Length );
			}
		}
		/// <summary>
		/// Gets lines count in stream.
		/// </summary>
		public int LinesCount
		{
			get
			{
				if( m_linesCount == -1 )
				{
					ForceFileRescan();
				}
				return m_linesCount;
			}
		}
		/// <summary>
		/// Gets or sets array of tokens, that consists of more than one splitters.
		/// </summary>
		public Split[] MultiCharTokens
		{
			get
			{
				return m_tokenStream.MultiCharTokens;
			}
			set
			{
				m_tokenStream.MultiCharTokens = value;
			}
		}
		/// <summary>
		/// Gets or sets line end symbols (/r, /n, /r/n)
		/// </summary>
		public string NewLineStr
		{
			get
			{
				return m_tokenStream.NewLine;
			}
			set
			{
				m_tokenStream.NewLine = value;
			}
		}
		/// <summary>
		/// Gets or sets AutoPush property of the ChangesStream.
		/// </summary>
		public int AutoPush
		{
			get
			{
				return m_changesStream.AutoPush;
			}
			set
			{
				m_changesStream.AutoPush = value;
			}
		}
		/// <summary>
		/// Gets flag that specifies whether there are actions to be undone.
		/// </summary>
		public bool CanUndo
		{
			get
			{
				if( !m_changesStream.CanUndo && m_undoStack.Count != 0 )
				{
					m_undoStack.Clear();
				}
				return ( m_undoStack.Count > 0 );
			}
		}
		/// <summary>
		/// Gets sign of redo ability.
		/// </summary>
		public bool CanRedo
		{
			get
			{
				if( !m_changesStream.CanRedo && m_redoStack.Count != 0 )
				{
					m_redoStack.Clear();
				}
				return ( m_redoStack.Count > 0 );
			}
		}
		/// <summary>
		/// Gets size (in bytes) of the new-line symbol.
		/// </summary>
		public int NewLineSize
		{
			get
			{
				return Encoding.GetByteCount( NewLineStr );
			}
		}
		/// <summary>
		/// Gets the value, indicating that stream supports write operation.
		/// </summary>
		public bool CanWrite
		{
			get
			{
				return m_inputStream.CanWrite;
			}
		}
		/// <summary>
		/// Gets currently used encoding.
		/// </summary>
		public Encoding Encoding
		{
			get
			{
				return m_tokenStream.Encoding;
			}
		}
		/// <summary>
		/// Gets or sets case sensitivity of the tokenizer.
		/// </summary>
		public bool CaseSensitive
		{
			get
			{
				return m_tokenStream.CaseSensitive;
			}
			set
			{
				m_tokenStream.CaseSensitive = value;
			}
		}
		/// <summary>
		/// Gets or sets syle of new line of underlying stream.
		/// </summary>
		public NewLineStyle NewLineStyle
		{
			get
			{
				return m_tokenStream.EndLineStyle;
			}
			set
			{
				m_tokenStream.EndLineStyle = value;
			}
		}
		/// <summary>
		/// Gets bool indicating whether new line style was detected.
		/// </summary>
		public bool NewLineStyleDetected
		{
			get
			{
				return m_tokenStream.NewLineStyleDetected;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Default constructor.
		/// </summary>
		private StreamsWrapper()
		{
		}
		/// <summary>
		/// Constructor must receive a file name and try to open it.
		/// </summary>
		/// <param name="fileName">File name.</param>
		public StreamsWrapper( string fileName )
			: this( fileName, FileMode.Open, FileAccess.Read, FileShare.Read )
		{
		}
		/// <summary>
		/// constructor allow to control mode of file open operation
		/// </summary>
		/// <param name="fileName">File name.</param>
		/// <param name="mode">A FileMode constant that determines how to open or create the file.</param>
		public StreamsWrapper( string fileName, FileMode mode )
			: this( fileName, mode, FileAccess.Read, FileShare.Read )
		{
		}
		/// <summary>
		/// Constructor allow to control File open mode and access flags
		/// </summary>
		/// <param name="fileName">File name.</param>
		/// <param name="mode">A FileMode constant that determines how to open or create the file.</param>
		/// <param name="access">A FileAccess constant that determines how the file can be
		/// accessed by the TokenStream object. This gets the CanRead and CanWrite properties
		/// of the FileStream object. CanSeek is true if path specifies a disk file.</param>
		public StreamsWrapper( string fileName, FileMode mode, FileAccess access )
			: this( fileName, mode, access, FileShare.Read )
		{
		}
		/// <summary>
		/// Open stream. Constructor allow to specify parameter of stream open
		/// </summary>
		/// <param name="fileName">File name</param>
		/// <param name="mode">A FileMode constant that determines how to open or create the file. </param>
		/// <param name="access">A FileAccess constant that determines how the file can be
		/// accessed by the TokenStream object. This gets the CanRead and CanWrite properties
		/// of the FileStream object. CanSeek is true if path specifies a disk file</param>
		/// <param name="share">A FileShare constant that determines how the file will be shared
		/// by processes</param>
		public StreamsWrapper( string fileName, FileMode mode, FileAccess access, FileShare share )
			: this( new FileStream( fileName, mode, access, share ), NewLineStyle.Windows )
		{
		}
		/// <summary>
		/// Constructor, initializes new object
		/// </summary>
		/// <param name="input">Input data Stream</param>
		/// <param name="newLineStyle">Style of new line.</param>
		public StreamsWrapper( Stream input, NewLineStyle newLineStyle )
			: this( input, newLineStyle, null )
		{
		}
		/// <summary>
		/// Constructor, initializes new object
		/// </summary>
		/// <param name="input">Input data Stream</param>
		/// <param name="newLineStyle">Style of new line.</param>
		/// <param name="encoding">Encoding to use.</param>
		public StreamsWrapper( Stream input, NewLineStyle newLineStyle, Encoding encoding )
		{
			if( input == null ) throw new ArgumentNullException( "input" );

			// All initialization here
			m_changesStream = new ChangesStream( input );
			m_changesStream.UndoBufferFlushed += new EventHandler( OnUndoBufferFlush );
			m_changesStream.RedoBufferFlushed += new EventHandler( OnRedoBufferFlush );
			m_tokenStream = new RegexTokenizer( m_changesStream, encoding );
			m_parsePointManager = new ParsePointManager();
			m_inputStream = input;

			if( !this.NewLineStyleDetected )
			{
				this.NewLineStyle = newLineStyle;
			}

#if TAG_ALLOWED
      m_ParsePointManager.ParsePointTagChanged += new ParsePointChangeHandler( RaiseParsePointTagChangedEvent );
#endif
		}
		/// <summary>
		/// Initializes StreamsWrapper from string input
		/// </summary>
		/// <param name="buffer">String with input data</param>
		/// <returns>StreamsWrapper Object</returns>
		public static StreamsWrapper FromString( string buffer )
		{
			if( buffer == null ) throw new ArgumentNullException( "buffer" );
			if( buffer.Length == 0 ) throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_57 );

			byte[] byteString = Encoding.Unicode.GetBytes( buffer );
			byte[] preamble = Encoding.Unicode.GetPreamble();
			byte[] bytes = new byte[ byteString.Length + preamble.Length ];

			if( preamble.Length > 0 )
			{
				Array.Copy( preamble, 0, bytes, 0, preamble.Length );
			}
			Array.Copy( byteString, 0, bytes, preamble.Length, byteString.Length );

			MemoryStream ms = new MemoryStream( byteString, false );
			StreamsWrapper output = new StreamsWrapper( ms, NewLineStyle.Windows );
			Debug.Assert( output.m_tokenStream.Encoding == Encoding.Unicode );
			return output;
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Flushes all changes.
		/// </summary>
		public void FlushChanges()
		{
			m_changesStream.FlushChanges();
		}
		/// <summary>
		/// Looks for ParsePoint, that is left to the given value.
		/// </summary>
		/// <param name="value">Value, to be found</param>
		/// <param name="comparer">Comparer, that is used to find needed value</param>
		/// <returns>ParsePoint, that is on given Value or left to it</returns>
		public IParsePoint GetParsePoint( object value, IComparer comparer )
		{
			if( comparer == null ) throw new ArgumentNullException( "comparer" );

			return m_parsePointManager.GetParsePointByComparer( value, comparer );
		}
		/// <summary>
		/// Retrieves ParsePoint by given offset in stream.
		/// </summary>
		/// <param name="streamOffset">Needed offset in stream.</param>
		/// <returns><see cref="IParsePoint"/> that coresponds to the needed offset.</returns>
		public IParsePoint GetParsePoint( long streamOffset )
		{
			if( streamOffset < 0 || streamOffset > m_tokenStream.Length ) throw new ArgumentOutOfRangeException(
				"streamOffset", streamOffset, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_58 );

			IParsePoint result;
			streamOffset = Math.Max( streamOffset, m_tokenStream.SkipBytes );
			int leftPoint, rightPoint;
			bool foundAny = m_parsePointManager.GetNearestParsePoints( streamOffset, out leftPoint, out rightPoint );
			IParsePoint leftNearest = ( leftPoint != -1 ) ? m_parsePointManager.GetParsePointByIndex( leftPoint ) : null;
			IParsePoint rightNearest = ( rightPoint != -1 ) ? m_parsePointManager.GetParsePointByIndex( rightPoint ) : null;

			// If we found needed parsepoint
			if( leftPoint != -1 && leftPoint == rightPoint )
			{
				result = leftNearest;
			}
			else
			{
				// if LeftPoint is null then parse from the very beginning!
				long parseOffset = ( leftNearest == null ) ? m_tokenStream.SkipBytes : leftNearest.Offset;
				long oldPosition = m_tokenStream.Position;

				// Reading text in specified range
				string text = GetTextInRange( new ParsePoint( 1, 1, parseOffset ), new ParsePoint( 1, 1, streamOffset ), false );
				int linesCount, lastLineLength;
				// Calculating space, occupied by text.
				CalculateTextData( ref text, out linesCount, out lastLineLength );

				// convert linesCount and lastLineLength to line and column of the new ParsePoint
				lastLineLength += ( leftNearest != null && ( linesCount == 0 ) ) ? leftNearest.Position : 1;
				linesCount += ( leftNearest != null ) ? leftNearest.Line : 1;

				IParsePoint res = m_parsePointManager.InsertPointInPosition( streamOffset, linesCount, lastLineLength, leftPoint + 1 );

				m_tokenStream.Position = oldPosition;
				result = res;
			}
			return result;
		}
		/// <summary>
		/// Searches for the ParsePoint in given position, or, if there is no such position in file, the nearest to it couple.
		/// If there is no ParsePoint, but such position exists in file, new ParsePoint will be created.
		/// </summary>
		/// <param name="line">Positions line</param>
		/// <param name="column">Positions column</param>
		/// <param name="leftNearest">Nearest ParsePoint from the left</param>
		/// <param name="rightNearest">Nearest ParsePoint from the right</param>
		/// <param name="create">If false, you have not remove returned ParsePoint after usage.
		/// Also newly created ParsePoint will not be added to update list if thi parameter is false.</param>
		/// <returns>IParsePoint interface of the found/created ParsePoint</returns>
		public IParsePoint GetParsePoint( int line, int column, out IParsePoint leftNearest, out IParsePoint rightNearest, bool create )
		{
			return GetParsePoint( line, column, out leftNearest, out rightNearest, create, false );
		}
		/// <summary>
		/// Gets parse point by given physical coordinates.
		/// </summary>
		/// <param name="x">X-coordinate of point to retrieve.</param>
		/// <param name="y">X-coordinate of point to retrieve.</param>
		/// <returns>Parse point with given coordinates.</returns>
		public ParsePoint GetParsePoint( int x, int y )
		{
			IParsePoint left, right;
			return ( ParsePoint )GetParsePoint( y, x, out left, out right, true, true );
		}
		/// <summary>
		/// Searches for the ParsePoint in given position, or, if there is no such position in file, the nearest to it couple.
		/// If there is no ParsePoint, but such position exists in file, new ParsePoint will be created.
		/// </summary>
		/// <param name="line">Positions line</param>
		/// <param name="column">Positions column</param>
		/// <param name="leftNearest">Nearest ParsePoint from the left</param>
		/// <param name="rightNearest">Nearest ParsePoint from the right</param>
		/// <param name="create">If false, you have not remove returned ParsePoint after usage.
		/// Also newly created ParsePoint will not be added to update list if thi parameter is false.</param>
		/// <param name="bAllowVirtualSpace">Indicates whether virtual space is allowed.
		/// If false, exeption is thrown if given column is in virtual space.</param>
		/// <returns>IParsePoint interface of the found/created ParsePoint</returns>
		public IParsePoint GetParsePoint(
			int line, int column, out IParsePoint leftNearest, out IParsePoint rightNearest, bool create, bool bAllowVirtualSpace )
		{
			IParsePoint result = null;
			int leftPoint, rightPoint;
			bool foundAny = m_parsePointManager.GetNearestParsePoints( line, column, out leftPoint, out rightPoint );
			leftNearest = ( leftPoint != -1 ) ? ( m_parsePointManager.GetParsePointByIndex( leftPoint ) ) : ( null );
			rightNearest = ( rightPoint != -1 ) ? ( m_parsePointManager.GetParsePointByIndex( rightPoint ) ) : ( null );

            
			// If we found needed parsepoint
            if (leftPoint != -1 && leftPoint == rightPoint)
			{
				result = leftNearest;
			}

			else
			{
				// Maybe "leftNearest" is what we need, but column is in virtual space, so we have to check it out.
				if( bAllowVirtualSpace && leftPoint != -1 )
				{
					if( leftNearest.Offset == this.Length )
					{
						result = leftNearest;
					}
					else
					{
						long offsetPlusNewLine = leftNearest.Offset + this.NewLineSize;
						if( offsetPlusNewLine <= this.Length )
						{
							IParsePoint pointAfterNewLine = GetParsePoint( offsetPlusNewLine );

							// If "pointAfterNewLine" is really at the new line, then "leftNearest" is what we need.
							if( leftNearest.Line == pointAfterNewLine.Line - 1 )
							{
								result = leftNearest;
							}
							else
							{
								// New parse point was possibly created, so we need to update left & right points.
								m_parsePointManager.GetNearestParsePoints( line, column, out leftPoint, out rightPoint );
								leftNearest = ( leftPoint != -1 ) ? ( m_parsePointManager.GetParsePointByIndex( leftPoint ) ) : ( null );
								rightNearest = ( rightPoint != -1 ) ? ( m_parsePointManager.GetParsePointByIndex( rightPoint ) ) : ( null );
								// If we found needed parsepoint
								if( leftPoint != -1 && leftPoint == rightPoint )
								{
									result = leftNearest;
								}
							}
						}
					}
				}

				if( result == null )
				{
					// if LeftPoint is null then parse from the very beginning!
					long parseOffset = ( leftNearest == null ) ? ( m_tokenStream.SkipBytes ) : ( leftNearest.Offset );
					int iLine = ( leftNearest == null ) ? ( 1 ) : ( leftNearest.Line );
					int iColumn = ( leftNearest == null || iLine != line ) ? ( 1 ) : ( leftNearest.Position );
					long oldPosition = m_tokenStream.Position;
					bool isPositionWrong = false;
					bool bFirstLineSkipped = false;
					ParsePoint res = null;
					m_tokenStream.Position = parseOffset;

					while( line > iLine )
					{
						if( iLine % DEF_LINES_TO_SKIP == 0 && bFirstLineSkipped )
						{
							m_parsePointManager.InsertPointInPosition( parseOffset, iLine, 1, ++leftPoint );
						}

						string str = m_tokenStream.ReadLine();

						if( str == null )
						{
							isPositionWrong = true;
							break;
						}

						//Setting ParseOffset to current offset in the stream manualy. StreamReader reads 1024 bytes blocks to internal buffer.
						parseOffset += m_tokenStream.Encoding.GetByteCount( str );
						bFirstLineSkipped = true;
						iLine++;
					}

					if( column > iColumn )
					{
						string read = m_tokenStream.ReadLine();
						if( read == null )
						{
							isPositionWrong = true;
						}
						else
						{
							int len = column - iColumn;
							if( bAllowVirtualSpace && len > read.Length )
							{
								len = read.Length;
							}

							if( this.NewLineSize == 2 && read.EndsWith( this.NewLineStr ) && len == read.Length - 1 )
							{
								isPositionWrong = true;
							}
							else
							{
								char[] chars = read.ToCharArray( 0, len );
								int bytesCount = m_tokenStream.Encoding.GetByteCount( chars );
                                if ('�' == chars[0])
                                {
                                    bytesCount = 2;
                                }                                
								parseOffset += bytesCount;
							}
						}
					}

					if( !isPositionWrong )
					{
						if( !create )
						{
							res = new ParsePoint( line, column, parseOffset );
						}
						else
						{
							res = m_parsePointManager.InsertPointInPosition( parseOffset, line, column, leftPoint + 1 ) as ParsePoint;
						}
					}

					m_tokenStream.Position = oldPosition;
					result = res;
				}
			}

            //Begin
            //SD 2756
            //Defect Name - EditControl throws exception when a bookmark in a collapsed area is removed. 
            if (result == null)
            {
                if (leftNearest != null && rightNearest != null)
                {
                    if (leftNearest.Line < rightNearest.Line && leftPoint != -1 && leftPoint != rightPoint)
                    {
                        result = leftNearest;
                    }
                }
            }
            //End

			return result;
		}
		/// <summary>
		/// Searches for the ParsePoint in given position, or, if there is no such position in file, the nearest to it couple.
		/// If there is no ParsePoint, but such position exists in file, new ParsePoint will be created.
		/// </summary>
		/// <param name="line">Positions line</param>
		/// <param name="column">Positions column</param>
		/// <param name="create">If false, you have not remove returned ParsePoint after usage.
		/// Also newly created ParsePoint will not be added to update list if thi parameter is false.</param>
		/// <returns>IParsePoint interface of the found/created ParsePoint</returns>
		public IParsePoint GetParsePoint( int line, int column, bool create )
		{
			IParsePoint LeftPoint, RightPoint;
			return GetParsePoint( line, column, out LeftPoint, out RightPoint, create );
		}
		/// <summary>
		/// Sets current position to the given ParsePoint
		/// </summary>
		/// <param name="point">IParsePoint interface to the ParsePoint</param>
		public void SetPositionToParsePoint( IParsePoint point )
		{
			if( point == null ) throw new ArgumentNullException( "point" );

			this.Position = point.Offset;
		}
		/// <summary>
		/// Reads token from current stream position.
		/// </summary>
		/// <returns>Token</returns>
		public string ReadToken()
		{
			return m_tokenStream.ReadToken();
		}
		/// <summary>
		/// Reads token from stream, but does not advances it's position.
		/// </summary>
		/// <returns>Token.</returns>
		public string PeekToken()
		{
			return m_tokenStream.PeekToken();
		}
		/// <summary>
		/// Reads token from given position.
		/// </summary>
		/// <param name="position">ParsePoint with position in the stream.</param>
		/// <returns>Token.</returns>
		public string ReadToken( IParsePoint position )
		{
			if( position == null ) throw new ArgumentNullException( "position" );

			SetPositionToParsePoint( position );
			return m_tokenStream.ReadToken();
		}
		/// <summary>
		/// Writes string to the stream starting from the given position.
		/// </summary>
		/// <param name="position">ParsePoint of the starting position</param>
		/// <param name="str">Text to write.</param>
		/// <returns>Number of inserted bytes.</returns>
		public int InsertText( IParsePoint position, string str )
		{
			if( position == null ) throw new ArgumentNullException( "position" );
			if( str == null ) throw new ArgumentNullException( "str" );
			if( str == string.Empty )
				throw new ArgumentOutOfRangeException( "str", str, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_10 );

			if( "\n" != m_tokenStream.NewLine )
			{
				str = str.Replace( "\n", m_tokenStream.NewLine );
			}

			int iLinesCount, LastLineCharsCount;
			long iOldPosition = Position = position.Offset;
			m_tokenStream.ResetBuffer( true );

			//Converting string using current encoding to bytes array and writing it to stream
			byte[] bytes = m_tokenStream.Encoding.GetBytes( str );

			WrapperUndoItem item = new WrapperUndoItem();
			item.ChangeContext = new ChangeContext( ChangeType.Insert, bytes );
			item.ChangeContext.Position = position.Offset;
			m_changesStream.AddChange( item.ChangeContext );
			item.BytesOffset = bytes.Length;
			m_undoStack.Push( item );

			ResetTokensCache();

			//Updating positions of all ParsePoints
			CalculateTextData( ref str, out iLinesCount, out LastLineCharsCount );

			OffsetChanged( position, item.BytesOffset, iLinesCount, LastLineCharsCount );
			m_version++;

			SetNewLinesCount( m_linesCount + iLinesCount );
			return bytes.Length;
		}
		/// <summary>
		/// Deletes text in specified range. Start and end are not included in to the delition range.
		/// </summary>
		/// <param name="start">Start ParsePoint.</param>
		/// <param name="end">End ParsePoint.</param>
		/// <returns>Number of deleted bytes.</returns>
		public int DeleteText( IParsePoint start, IParsePoint end )
		{
			if( start == null ) throw new ArgumentNullException( "start" );
			if( end == null ) throw new ArgumentNullException( "end" );

			int result = 0;
			m_version++;
			if( start != end )
			{
				string oldText = GetTextInRange( start, end, false );

				if( oldText == null || oldText == string.Empty )
					throw new Exception( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_60 );

				int lines, iLastLineLength;
				int deleteSize = ( int )( end.Offset - start.Offset );
				WrapperUndoItem item = new WrapperUndoItem();
				item.ChangeContext = new ChangeContext( ChangeType.Delete, null, deleteSize );
				item.ChangeContext.Position = start.Offset;
				item.AdditionalData = oldText;
				item.BytesOffset = deleteSize;
				m_undoStack.Push( item );

				ResetTokensCache();

				CalculateTextData( ref oldText, out lines, out iLastLineLength );
				m_changesStream.AddChange( item.ChangeContext.Position, item.ChangeContext );

				// For proper lines counting and indexing m_linesCount must be set before calling OffsetChanged.
				int newValue = Math.Max( m_linesCount - lines, 1 );
				int oldValue = m_linesCount;
				m_linesCount = newValue;

				OffsetChanged( end, -item.BytesOffset, lines, end.Position - start.Position );
				if( oldValue != newValue )
				{
					RaiseLinesCountChangedEvent( oldValue, newValue );
				}

				result = deleteSize;
			}
			return result;
		}
		/// <summary>
		/// Undoes last action.
		/// </summary>
		/// <returns>Undo item, that was undone.</returns>
		public WrapperUndoItem Undo()
		{
			WrapperUndoItem result = WrapperUndoItem.Empty;
			if( this.CanUndo )
			{
				WrapperUndoItem item = ( WrapperUndoItem )m_undoStack.Pop();
				m_redoStack.Push( item );
				int iLinesCount, LastLineCharsCount;
				string str;
				switch( item.ChangeContext.Type )
				{
					case ChangeType.Delete:
						str = ( string )item.AdditionalData;
						CalculateTextData( ref str, out iLinesCount, out LastLineCharsCount );
						IParsePoint position = GetParsePoint( item.ChangeContext.Position );
						m_changesStream.Undo();
						ResetTokensCache();
						OffsetChanged( position, item.BytesOffset, iLinesCount, LastLineCharsCount );
						SetNewLinesCount( m_linesCount + iLinesCount );
						m_version++;
						break;

					case ChangeType.Insert:
						str = m_tokenStream.Encoding.GetString( item.ChangeContext.Data );
						CalculateTextData( ref str, out iLinesCount, out LastLineCharsCount );
						IParsePoint positionStart = GetParsePoint( item.ChangeContext.Position );
						IParsePoint positionEnd = GetParsePoint( item.ChangeContext.Position + item.BytesOffset );
						m_changesStream.Undo();
						ResetTokensCache();
						OffsetChanged( positionEnd, -item.BytesOffset, iLinesCount, positionEnd.Position - positionStart.Position );
						SetNewLinesCount( m_linesCount - iLinesCount );
						m_version++;
						break;
				}

				result = item;
			}
			return result;
		}
		/// <summary>
		/// Redoes last undone action.
		/// </summary>
		/// <returns>Undo item that was redone.</returns>
		public WrapperUndoItem Redo()
		{
			WrapperUndoItem result = WrapperUndoItem.Empty;
			if( CanRedo )
			{
				WrapperUndoItem item = ( WrapperUndoItem )m_redoStack.Pop();
				m_undoStack.Push( item );
				int iLinesCount, LastLineCharsCount;
				string str;

				switch( item.ChangeContext.Type )
				{
					case ChangeType.Delete:
						str = ( string )item.AdditionalData;
						CalculateTextData( ref str, out iLinesCount, out LastLineCharsCount );
						IParsePoint positionStart = GetParsePoint( item.ChangeContext.Position );
						IParsePoint positionEnd = GetParsePoint( item.ChangeContext.Position + item.BytesOffset );
						m_changesStream.Redo();
						ResetTokensCache();
						OffsetChanged( positionEnd, -item.BytesOffset, iLinesCount, positionEnd.Position - positionStart.Position );
						SetNewLinesCount( m_linesCount - iLinesCount );
						m_version++;
						break;

					case ChangeType.Insert:
						str = m_tokenStream.Encoding.GetString( item.ChangeContext.Data );
						CalculateTextData( ref str, out iLinesCount, out LastLineCharsCount );
						IParsePoint position = GetParsePoint( item.ChangeContext.Position );
						m_changesStream.Redo();
						ResetTokensCache();
						OffsetChanged( position, item.BytesOffset, iLinesCount, LastLineCharsCount );
						SetNewLinesCount( m_linesCount + iLinesCount );
						m_version++;
						break;
				}

				result = item;
			}
			return result;
		}
		/// <summary>
		/// Converts all new-line symbols in string. All new line symbols are converted to \n.
		/// </summary>
		/// <param name="str">String to be converted.</param>
		/// <returns>Converted string.</returns>
		public string ConvertNewLines( string str )
		{
			if( str == null ) throw new ArgumentNullException( "str" );
			if( str.Length == 0 ) throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_61 );

			int winNewLineIndex = str.IndexOf( "\r\n" );
			int unixNewLineIndex = str.IndexOf( "\n\r" );
			if( winNewLineIndex != -1 && unixNewLineIndex != -1 )
			{
				if( winNewLineIndex < unixNewLineIndex )
				{
					str = str.Replace( "\r\n", "\n" ).Replace( "\n\r", "\n" );
				}
				else
				{
					str = str.Replace( "\n\r", "\n" ).Replace( "\r\n", "\n" );
				}
			}
			else
			{
				if( winNewLineIndex != -1 )
				{
					str = str.Replace( "\r\n", "\n" );
				}

				if( unixNewLineIndex != -1 )
				{
					str = str.Replace( "\n\r", "\n" );
				}

				str = str.Replace( "\r", "\n" );
			}

			return str;
		}
		/// <summary>
		/// Reads text from specified range.
		/// </summary>
		/// <param name="start">Start of the range.</param>
		/// <param name="end">End of the range.</param>
		/// <param name="bConvert">Flag, that specifies whether all new-line symbols have to be converted to /n.</param>
		/// <returns>Text from the stream.</returns>
		public string GetTextInRange( IParsePoint start, IParsePoint end, bool bConvert )
		{
			if( start == null ) throw new ArgumentNullException( "start" );
			//if( end == null ) throw new ArgumentNullException( "end" );
            if (end == null)
            {
                end = start; //for fix issue - "matching parents and collapsing problem in WF Edit"
            }


			string result = string.Empty;
			long bytesCount = end.Offset - start.Offset;
			if( bytesCount != 0 )
			{
				SetPositionToParsePoint( start );
				m_tokenStream.ResetBuffer( true );								// fix for defect 3589.
				string str = m_tokenStream.ReadString( ( int )bytesCount );
				if( bConvert )
				{
					str = ConvertNewLines( str );
				}
				result = str;
			}
			return result;
		}
		/// <summary>
		/// Rescans line count in file and updates line-start ParsePoints
		/// </summary>
		public void ForceFileRescan()
		{
			m_linesCount = 0;
			long OldPosition = m_changesStream.Position;

			IParsePoint pointEnd = GetParsePoint( m_changesStream.Length );

			if( pointEnd == null ) throw new InvalidOperationException( "Can not get end of file." );

			m_linesCount = pointEnd.Line;
		}
		/// <summary>
		/// Searches text, specified by regular expression, in stream.
		/// </summary>
		/// <param name="start">Start position of the search.</param>
		/// <param name="expression">Regular expression, used for search.</param>
		/// <param name="searchUp">Indicates whether search should be performed bottom-up.</param>
		/// <returns>Result of the search. Never can be null.</returns>
		/// <remarks>
		/// If you want to treat as found sub-string just some part of the text, than 
		/// you have to wrap this search part in to a named group with name "_data_".
		/// </remarks>
        public FindResult FindNext(IParsePoint start, Regex expression, bool searchUp)
        {
            if (start == null) throw new ArgumentNullException("start");
            if (expression == null) throw new ArgumentNullException("expression");

            FindResult result = new FindResult();
            do
            {
                int iLine = start.Line;
                int iStartColumn = start.Position;
                if (iStartColumn != 1)
                {
                    start = GetParsePoint(iLine, 1, true);
                }
                else if (searchUp)
                {
                    if (iLine == 1)
                    {
                        break;
                    }

                    start = GetParsePoint(--iLine, 1, true);
                }

                IParsePoint end = (iLine < LinesCount) ? GetParsePoint(iLine + 1, 1, true) : GetParsePoint(this.Length);
                // Find data in text
                string text = GetTextInRange(start, end, false);
                Match match = null;
                MatchCollection matches = expression.Matches(text);

                if (!searchUp)
                {
                    for (int i = 0; i < matches.Count; i++)
                    {
                        if (matches[i].Index + 1 >= iStartColumn)
                        {
                            match = matches[i];
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = matches.Count - 1; i >= 0; i--)
                    {
                        if (iStartColumn == 1 || matches[i].Index + 1 < iStartColumn)
                        {
                            match = matches[i];
                            break;
                        }
                    }
                }

                if (match != null && match.Success)
                {
                    Group group = match.Groups[DEF_SEARCH_DATA_GROUP];
                    int iGrpIndex = expression.GroupNumberFromName(DEF_SEARCH_DATA_GROUP);

                    // If there was no such group specified at all, 
                    // than search of entire expression was done.
                    if (iGrpIndex == -1)
                    {
                        group = match.Groups[0];
                    }

                    // Find position for new ParsePoint
                    SetPositionToParsePoint(start);

                    string strToRead = (group.Index == text.Length) ? (text) : (text.Substring(1, group.Index));
                    
					//** Fix for 2839 **
					string tempStr = group.Value;
					
                    for (int i = 0; i < group.Value.ToString().Length; i++)
                    {
                        if (/*group.Value[i] < 0x20 ||*/ group.Value[i] > 127 && group.Value[i] != 0xA3) //Fix for Issue SD5306 and SD7947_S2
                        {
                            tempStr = group.Value.Substring(0, i);
                        }
                    }
					//** end 2839 **
                    m_tokenStream.ReadString(m_tokenStream.Encoding.GetByteCount(strToRead) - start.Position + 1);

                    result.StartPoint = GetParsePoint(Position);

                    m_tokenStream.ReadString(m_tokenStream.Encoding.GetByteCount(tempStr));
                    result.EndPoint = GetParsePoint(Position);

                    result.Result = match;
                }
                else
                {
                    result.Result = null;
                    result.StartPoint = start;
                    result.EndPoint = end;
                }

                if (!searchUp)
                {
                    start = result.EndPoint;
                }
                else
                {
                    start = result.StartPoint;
                }
            }
            while ((result.Result == null || !result.Result.Success) && ((!searchUp && result.EndPoint.Offset < Length) ||
                (searchUp && result.StartPoint.Offset > this.m_tokenStream.SkipBytes)));

            if (result.Result == null)
            {
                result.Result = Match.Empty;
            }
            return result;
        }

        /// <summary>
        /// Searches text, as like in visual studio editor
        /// </summary>
        /// <param name="start">Start position of the search.</param>
        /// <param name="expression">Regular expression, used for search.</param>
        /// <param name="searchUp">Indicates whether search should be performed bottom-up.</param>
        /// <param>Indicates whether text is seleted from any where search.</param>
        /// <param name="isSelection">isSelection</param>
        /// <returns>Result of the search. Never can be null.</returns>
        /// <remarks>
        /// If you want to treat as found sub-string just some part of the text, than 
        /// you have to wrap this search part in to a named group with name "_data_".
        /// </remarks>
        public FindResult FindNext(IParsePoint start, Regex expression, bool searchUp,bool isSelection)
        {
            if (start == null) throw new ArgumentNullException("start");
            if (expression == null) throw new ArgumentNullException("expression");

            FindResult result = new FindResult();
            do
            {

                int iLine = start.Line;
                int iStartColumn = start.Position;

                if (iStartColumn != 1)
                {
                    start = GetParsePoint(iLine, 1, true);
                }
                else if (searchUp)
                {
                    if (iLine == 1)
                    {
                        break;
                    }

                    start = GetParsePoint(--iLine, 1, true);
                }

                IParsePoint end = (iLine < LinesCount) ? GetParsePoint(iLine + 1, 1, true) : GetParsePoint(this.Length);
                // Find data in text
                string text = GetTextInRange(start, end, false);
                Match match = null;
                MatchCollection matches = expression.Matches(text);

                int regexStrCount = expression.ToString().Length;

                bool visualStudioEditorFindStyle = this.LikeVisualStudioEditorSearch;

                if (visualStudioEditorFindStyle && iStartColumn>1)
                {
                    if (isSelection)
                    {
                        if (!searchUp)
                        {
                            string tempString = string.Copy(text);
                            tempString = tempString.Remove(0, iStartColumn - regexStrCount);
                            matches = expression.Matches(tempString);
                            iStartColumn = iStartColumn - regexStrCount;
                        }
                        else
                        {
                            int i=0;
                            string temp = null;
                            int istartClumn= iStartColumn;
                            while (i <= iStartColumn)
                            {
                                if (text.Length < iStartColumn-1 || istartClumn-1<0)
                                    break;
                                temp = text.Substring(istartClumn-1, i);
                              
                                matches = expression.Matches(temp);
                                if (matches.Count > 0)
                                    break;

                                i++;
                                istartClumn--;
                            }
                            if (matches.Count > 0)
                            {
                                iStartColumn = istartClumn-1;
                            }
                        }
                    }
                    else
                    {
                        if (!searchUp)
                        {
                            string tempString = string.Copy(text);

                            tempString = tempString.Remove(0, iStartColumn - 1);
                            matches = expression.Matches(tempString);
                            iStartColumn = iStartColumn - 1;

                        }
                        else
                        {
                            int i = 0;
                            string temp = null;
                            int istartClumn = iStartColumn;
                            while (i <= iStartColumn && iStartColumn!=0)
                            {
                                if (text.Length < iStartColumn-1 || istartClumn-1<0)
                                    break;
                                temp = text.Substring(istartClumn - 1, i);

                                matches = expression.Matches(temp);
                                if (matches.Count > 0)
                                    break;

                                i++;
                                istartClumn--;
                            }
                            if (matches.Count > 0)
                            {
                                iStartColumn = istartClumn - 1;
                            }
                        }
                    }
                }

                else
                {
                    visualStudioEditorFindStyle = false;
                    iStartColumn = 0;
                }

                if (!searchUp)
                {
                    for (int i = 0; i < matches.Count; )
                    {
                        if (isSelection && visualStudioEditorFindStyle)
                        {

                            match = matches[i];
                            break;

                        }
                        else //if (matches[i].Index + 1 >= iStartColumn)
                        {
                            match = matches[i];
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = matches.Count - 1; i >= 0; )
                    {
                                              
                            match = matches[i];
                            break;
                       
                    }
                }

                if (match != null && match.Success)
                {
                    Group group = match.Groups[DEF_SEARCH_DATA_GROUP];
                    int iGrpIndex = expression.GroupNumberFromName(DEF_SEARCH_DATA_GROUP);

                    // If there was no such group specified at all, 
                    // than search of entire expression was done.
                    if (iGrpIndex == -1)
                    {
                        group = match.Groups[0];
                    }

                    // Find position for new ParsePoint
                    SetPositionToParsePoint(start);

                    string strToRead = (group.Index == text.Length) ? (text) : (text.Substring(1, group.Index));
                    m_tokenStream.ReadString(m_tokenStream.Encoding.GetByteCount(strToRead) - start.Position + 1);

                    if (visualStudioEditorFindStyle && !searchUp && isSelection)
                    {
                        result.StartPoint = GetParsePoint(Position + iStartColumn);
                    }
                    else
                    {
                        result.StartPoint = GetParsePoint(Position+iStartColumn);
                    }

                    m_tokenStream.ReadString(m_tokenStream.Encoding.GetByteCount(group.Value));

                    if (visualStudioEditorFindStyle && !searchUp && isSelection)
                    {
                        result.EndPoint = GetParsePoint(Position + iStartColumn);
                    }
                    else
                    {
                        result.EndPoint = GetParsePoint(Position+iStartColumn);
                    }

                    result.Result = match;
                }
                else
                {
                    result.Result = null;
                    result.StartPoint = start;
                    result.EndPoint = end;
                }

                if (!searchUp)
                {
                    start = result.EndPoint;
                }
                else
                {
                    start = result.StartPoint;
                }

            }
            while ((result.Result == null || !result.Result.Success) && ((!searchUp && result.EndPoint.Offset < Length) ||
                (searchUp && result.StartPoint.Offset > this.m_tokenStream.SkipBytes)));

            if (result.Result == null)
            {
                result.Result = Match.Empty;
            }
            return result;
        }

		/// <summary>
		/// Saves changes to file.
		/// </summary>
		public void Save()
		{
			m_changesStream.Flush();
		}
		/// <summary>
		/// Saves data to given stream.
		/// </summary>
		/// <param name="stream">Output stream.</param>
		public void SaveTo( Stream stream )
		{
			if( stream == null ) throw new ArgumentNullException( "stream" );

			// Makes a backup copy.
			MemoryStream tempCopy = new MemoryStream( ( int )m_changesStream.Length );
			m_changesStream.CopyTo( tempCopy );
			tempCopy.WriteTo( stream );
		}
		/// <summary>
		/// Closes internal stream. It is no longer accessible.
		/// </summary>
		public void Close( bool closeInput )
		{
			m_parsePointManager.Dispose();
			m_parsePointManager = null;
			m_tokenStream.Close();
			m_tokenStream = null;
			m_changesStream.Close();

			if( closeInput )
			{
				m_inputStream.Close();
			}
		}
		/// <summary>
		/// Gets enumerator for all ParsePoints
		/// </summary>
		/// <returns>Enumerator. Every element is IParsePoint.</returns>
		public IEnumerator GetParsePointEnumerator()
		{
			return m_parsePointManager.GetEnumerator();
		}
		/// <summary>
		/// Gets enumerator for all ParsePoints in specified range.
		/// </summary>
		/// <param name="startPoint">Start of the range.</param>
		/// <param name="endPoint">End of the range.</param>
		/// <returns>Enumerator. Every element is IParsePoint.</returns>
		public IEnumerator GetParsePointEnumerator( IParsePoint startPoint, IParsePoint endPoint )
		{
			if( startPoint == null ) throw new ArgumentNullException( "startPoint" );
			if( endPoint == null ) throw new ArgumentNullException( "endPoint" );

			return m_parsePointManager.GetEnumerator( startPoint, endPoint );
		}
		/// <summary>
		/// Gets enumerator for all ParsePoints in range from specified one to the last one.
		/// </summary>
		/// <param name="startPoint">Start of the range.</param>
		/// <returns>Enumerator. Every element is IParsePoint.</returns>
		public IEnumerator GetParsePointEnumerator( IParsePoint startPoint )
		{
			if( startPoint == null ) throw new ArgumentNullException( "startPoint" );

			return m_parsePointManager.GetEnumerator( startPoint );
		}
		/// <summary>
		/// Discards all unsaved changes.
		/// </summary>
		internal void DiscardChanges()
		{
			ResetTokensCache();
			m_changesStream.DiscardChanges();

			// Clear buffers.
			bool useless = CanUndo;
			useless = CanRedo;
		}
		/// <summary>
		/// Checks whether data between given parse points has been changed.
		/// </summary>
		/// <param name="startOffset">Offset of the beginning of range to check.</param>
		/// <param name="endOffset">Offset of the end of range to check.</param>
		/// <returns>Bool indicating whether data in given range has been changed.</returns>
		public bool RangeChanged( long startOffset, long endOffset )
		{
			if( startOffset < 0 ) throw new ArgumentOutOfRangeException( "startOffset" );
			if( endOffset < 0 ) throw new ArgumentOutOfRangeException( "endOffset" );

			bool result;
			if( endOffset == this.Length )
			{
				endOffset--;
			}

			ArrayList windows = m_changesStream.DataWindows;
			int startIndex = windows.BinarySearch( startOffset, ChangesStream.DATA_WINDOW_SEARCH_COMPARER );
			int endIndex = windows.BinarySearch( endOffset, ChangesStream.DATA_WINDOW_SEARCH_COMPARER );

			if( startIndex < 0 || endIndex < 0 )
			{
				result = false;
			}
			else
			{
				if( startIndex != endIndex )
				{
					result = true;
				}
				else
				{
					result = ( ( ( IDataWindow )windows[ startIndex ] ).Source is ChangeContext );
				}
			}
			return result;
		}
		/// <summary>
		/// Gets the first (the highest) undo item from the undo stack.
		/// </summary>
		/// <returns>First undo item.</returns>
		public WrapperUndoItem GetFirstUndoItem()
		{
			if( m_undoStack.Count == 0 ) throw new Exception( "Undo stack is empty." );

			return ( WrapperUndoItem )m_undoStack.Peek();
		}
		/// <summary>
		/// Gets the first (the highest) redo item from the undo stack.
		/// </summary>
		/// <returns>First redo item.</returns>
		public WrapperUndoItem GetFirstRedoItem()
		{
			if( m_redoStack.Count == 0 ) throw new Exception( "Redo stack is empty." );

			return ( WrapperUndoItem )m_redoStack.Peek();
		}
		/// <summary>
		/// Sets position and resets tokenizer cache.
		/// </summary>
		/// <param name="pos">Position to set.</param>
		public void SetPositionAndResetCache( long pos )
		{
			if( this.Position != pos )
			{
				this.Position = pos;
				m_tokenStream.ResetBuffer( true );
			}
		}
		/// <summary>
		/// Calculates parameters of the text.
		/// </summary>
		/// <param name="str">Text to process.</param>
		/// <param name="LinesCount">OUT count of lines in text.</param>
		/// <param name="LastLineCharsCount">OUT length of the last line.</param>
		public virtual void CalculateTextData( ref string str, out int LinesCount, out int LastLineCharsCount )
		{
			LinesCount = 0;
			LastLineCharsCount = 0;
			int Index = -1;

			string endline = m_tokenStream.NewLine;

			while( ( Index = str.IndexOf( endline, Index + 1 ) ) >= 0 )
			{
				LinesCount++;
				LastLineCharsCount = Index + endline.Length;
			}

			LastLineCharsCount = str.Length - LastLineCharsCount;
		}
		/// <summary>
		/// Resets tokens cache after any text changes.
		/// </summary>
		public void ResetTokensCache()
		{
			m_tokenStream.ResetBuffer( false );
		}
		#endregion

		#region Events
		/// <summary>
		/// Event, that is raised when count of lines has been changed.
		/// </summary>
		public event ValueChangedEventHandler LinesCountChanged;
#pragma warning disable //LineInserted is used in another file
        /// <summary>
        /// Event, that is raised when lines has been inserted.
        /// </summary>
        public event LineInsertedEventHandler LineInserted;
        /// <summary>
        /// Event, that is raised when lines has been Deleted.
        /// </summary>
        public event LineDeletedEventHandler LineDeleted;
		/// <summary>
		/// Event, that is raised when undo buffer is flushed.
        /// </summary>
#pragma warning enable  //LineDeleted is used in another file
        public event EventHandler UndoBufferFlushed;
		/// <summary>
		/// Event, that is raised when redo buffer is flushed.
		/// </summary>
		public event EventHandler RedoBufferFlushed;
		/// <summary>
		/// Event that is raised before updating parsepoints offsets.
		/// </summary>
		public event EventHandler BeforeTextChange;
		/// <summary>
		/// Event that is raised after updating parsepoints offsets.
		/// </summary>
		public event EventHandler AfterTextChange;
		#endregion

		#region Event Raisers
		/// <summary>
		/// Raisers of the LinesCountChanged event.
		/// </summary>
		/// <param name="oldValue">Old count of lines.</param>
		/// <param name="newValue">New count of lines.</param>
		internal void RaiseLinesCountChangedEvent( int oldValue, int newValue )
		{
			if( LinesCountChanged != null )
			{
				LinesCountChanged( this, new ValueChangedEventArgs( oldValue, newValue ) );
			}
		}
		/// <summary>
		/// Raises UndoBufferFlushed event.
		/// </summary>
		protected virtual void RaiseUndoBufferFlushedEvent()
		{
			if( UndoBufferFlushed != null )
			{
				UndoBufferFlushed( this, EventArgs.Empty );
			}
		}
		/// <summary>
		/// Raises RedoBufferFlushed event.
		/// </summary>
		protected virtual void RaiseRedoBufferFlushedEvent()
		{
			if( RedoBufferFlushed != null )
			{
				RedoBufferFlushed( this, EventArgs.Empty );
			}
		}
		#endregion

		#region Nonpublic Methods
		/// <summary>
		/// Adds delegate to the after update invoke list.
		/// </summary>
		/// <param name="d"></param>
		internal void AddDelegateToAfterUpdateInvokeList( Delegate d )
		{
			m_parsePointManager.AddDelegateToAfterUpdateInvokeList( d );
		}
		/// <summary>
		/// Checks whether after update invoke list contains specified delegate.
		/// </summary>
		/// <param name="d">Delegate to check.</param>
		/// <returns>True if list contains specified delegate; otherwise false.</returns>
		internal bool AfterUpdateListContains( Delegate d )
		{
			return m_parsePointManager.AfterUpdateListContains( d );
		}
		/// <summary>
		/// Sets new value of lines count property and raises LinesCountChanged event.
		/// </summary>
		/// <param name="newValue">New value of the LinesCount property.</param>
		protected virtual void SetNewLinesCount( int newValue )
		{
			int oldValue = m_linesCount;
			m_linesCount = newValue;

			if( oldValue != newValue )
			{
				RaiseLinesCountChangedEvent( oldValue, newValue );
			}
		}
		/// <summary>
		/// Handler for the UndoBufferFlushed event of the changes stream.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnUndoBufferFlush( object sender, EventArgs e )
		{
			m_undoStack.Clear();
			RaiseUndoBufferFlushedEvent();
		}
		/// <summary>
		/// Handler for the UndoBufferFlushed event of the changes stream.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRedoBufferFlush( object sender, EventArgs e )
		{
			m_redoStack.Clear();
			RaiseRedoBufferFlushedEvent();
		}
		/// <summary>
		/// Updates offsets of all ParsePoints, starting from the given position.
		/// If data was deleted, than iBytesInserted must be negative, everything else must be positive.
		/// </summary>
		/// <param name="startPoint">StartPoint, that is in position, that was moved</param>
		/// <param name="iBytesInserted">Count of inserted byte (if negative, then bytes were deleted)</param>
		/// <param name="iLinesInData">Count of lines in inserted/deleted data</param>
		/// <param name="iLastLineLength">Position offset for the last line.</param>
		private void OffsetChanged( IParsePoint startPoint, long iBytesInserted, int iLinesInData, int iLastLineLength )
		{
			if( BeforeTextChange != null )
			{
				BeforeTextChange( this, EventArgs.Empty );
			}

			m_parsePointManager.OffsetChanged( startPoint, iBytesInserted, iLinesInData, iLastLineLength );

			if( AfterTextChange != null )
			{
				AfterTextChange( this, EventArgs.Empty );
			}
		}
		#endregion
	}
}
