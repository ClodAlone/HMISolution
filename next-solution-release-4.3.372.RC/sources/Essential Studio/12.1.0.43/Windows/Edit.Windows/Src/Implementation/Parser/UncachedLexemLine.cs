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
using System.Diagnostics;

using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Implementation.IO;

namespace Syncfusion.Windows.Forms.Edit.Implementation.Parser
{
  /// <summary>
  /// Class, that implements ILexemLine, but does not support caches of parsed data.
  /// </summary>
  public class UncachedLexemLine
    : ILexemLine
    , IDisposable
    , IComparable
  {
    #region Fields
    /// <summary>
    /// ParsePoint at the beginning of the line.
    /// </summary>
    private IParsePoint m_point;
    /// <summary>
    /// Stack at the beginning of the line.
    /// </summary>
    private ConfigStack m_stack;
    /// <summary>
    /// Parser, the line belongs to.
    /// </summary>
    protected LexemParser m_parser;
    /// <summary>
    /// Index of the line. It can be different from the one, stored in m_point because it also includes data from collapsing.
    /// </summary>
    private int m_lineIndex;
    #endregion

    #region Properties
    /// <summary>
    /// Gets ParsePoint at the beginning of the line.
    /// </summary>
    public IParsePoint LineStartPoint
    {
      get
      {
        if( m_point == null )
          throw new Exception( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_2 );
        return m_point;
      }
    }
    /// <summary>
    /// Gets ParsePoint at the end of the line.
    /// </summary>
    public virtual IParsePoint LineEndPoint
    {
      get
      {
        GetLineEndStack();
        return m_parser.BaseStream.GetParsePoint( m_parser.BaseStream.Position );
      }
    }
    /// <summary>
    /// Gets stack at the beginning of the line.
    /// </summary>
    public ConfigStack LineStartStack
    {
      get
      {
        return m_stack;
      }
    }
    /// <summary>
    /// Gets stack at the end of the line. If line was not parsed, it will be reparsed.
    /// </summary>
    public ConfigStack LineEndStack
    {
      get
      {
        return GetLineEndStack();
      }
    }
    /// <summary>
    /// Collection of all lexems, that belong to current line. If line was not parsed, it will be reparsed.
    /// </summary>
    public IList LineLexems
    {
      get
      {
        long lDummy;
        IList lexems = GetLineLexems( out lDummy );
				return lexems;

      }
    }
    /// <summary>
    /// Gets flag that determines, whether line is parsed. If line is parsed, than LineEndStack property contains Stack for the end of the line
    /// and LineLexems collection contains all lexems, that belong to current line. If line was changed, than Parsed will be set to false.
    /// </summary>
    public virtual bool Parsed
    {
      get
      {
        return false;
      }
      set
			{
			}
    }
    /// <summary>
    /// Gets parser, the line belongs to.
    /// </summary>
    public ILexemParser Parser
    {
      get
      {
        return m_parser;
      }
    }
    /// <summary>
    /// Gets length of the line.
    /// </summary>
    public int LineLength
    {
      get
      {
        long lDummy;
        IList lexems = GetLineLexems( out lDummy );
        int iLength = 0;
        foreach( ILexem lex in lexems )
        {
          iLength += lex.Length;
        }

        return iLength;
      }
    }
    /// <summary>
    /// Checks validity of the line. If line was already disposed, it is no longer valid.
    /// </summary>
    public bool IsValid
    {
      get
      {
        return ( m_point != null && m_parser != null );
      }
    }
    /// <summary>
    /// Index of the line.
    /// </summary>
    /// <remarks>
    /// It can be different from the one, stored in m_point because it also includes data from collapsing.
    /// </remarks>
    public int LineIndex
    {
      get
      {
        return m_lineIndex;
      }
      set
      {
        if( m_lineIndex != value )
        {
          m_lineIndex = value;
					RaiseLineIndexChangedEvent();
        }
      }
    }
    #endregion

    #region Events
    /// <summary>
    /// Event, that is raised when line is deleted. If position of the LineStartPoint is changed,
    /// than line is considered to be invalid and must be deleted. Or LineStartPoint was deleted.
    /// </summary>
    public event EventHandler LineDeleted;
    #endregion

		#region Initialization And Finalization
    /// <summary>
    /// Creates and initializes new instance of the class.
    /// </summary>
    /// <param name="parser">Parent parser.</param>
    /// <param name="pointLineStart">ParsePoint of the line start.</param>
    /// <param name="stack">Stack at the beginning of the line.</param>
    public UncachedLexemLine( LexemParser parser, IParsePoint pointLineStart, ConfigStack stack )
    {
      if( pointLineStart == null ) throw new ArgumentNullException( "pointLineStart" );
      if( pointLineStart.Position != 1 )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_3, "pointLineStart" );
      if( stack == null ) throw new ArgumentNullException( "stack" );
			if( parser == null ) throw new ArgumentNullException( "parser" );

      m_point = pointLineStart;
      m_stack = stack;
      m_parser = parser;
      m_point.ParsePointParameterChanged += new ParsePointParameterChangedEventHandler( PointPositionChanged );
      m_point.Deleted += new ParsePointDeletedEventHandler(PointDeleted);
      m_lineIndex = pointLineStart.Line;
    }
    /// <summary>
    /// Frees all used resources.
    /// </summary>
    public virtual void Dispose()
    {
      if( m_parser != null )
      {
        m_point.ParsePointParameterChanged -= new ParsePointParameterChangedEventHandler( PointPositionChanged );
        m_point.Deleted -= new ParsePointDeletedEventHandler(PointDeleted);

#if DEBUG
				UncachedLexemLine line = m_parser.FindLineInCache( this.LineIndex ) as UncachedLexemLine;

				if( null != line )
				{
					Debug.WriteLine( line.LineIndex, "The disposed line is still in the lines list." );
					Debug.WriteLineIf( ( line != this ), "But this instance is not the same as in cache.... someone has reparsed the line after it was deleted and before it is disposed." );
				}
#endif		

        m_point = null;
        m_parser = null;
      }
    }
    #endregion

		#region Nonpublic Methods
		/// <summary>
		/// Raises LineIndexChanged event.
		/// </summary>
		protected void RaiseLineIndexChangedEvent()
		{
			m_parser.RaiseLineIndexChanged( this );
		}
    /// <summary>
    /// Gets stack at the end of line. Line will be reparsed.
    /// </summary>
    /// <returns>Stack at the end of line. It can be treated as start stack for the next line.</returns>
    protected virtual ConfigStack GetLineEndStack()
    {
      IEnumerator enumerator = m_parser.GetEnumerator( m_stack, m_point );
      long lLastPosition = m_parser.BaseStream.Position;

      while( enumerator.MoveNext() )
      {
        ILexem lexem = ( ILexem )enumerator.Current;
				if( lexem.Text == m_parser.BaseStream.NewLineStr )
				{
					break;
				}
      }

      return m_parser.GetStackCopy();
    }
    /// <summary>
    /// Collection of all lexems, that belongs to the line. Line will be reparsed.
    /// </summary>
    /// <returns>List of the lexems.</returns>
    protected virtual IList GetLineLexems( out long lPositionBeforeNewLine )
    {
      IEnumerator enumerator = m_parser.GetEnumerator( m_stack, m_point );
      lPositionBeforeNewLine = m_parser.BaseStream.Position;
      IList result = new ArrayList();
			int iCurColumn = 1;

      while( enumerator.MoveNext() )
      {
        Lexem lexem = ( Lexem )enumerator.Current;
				if( ( lexem.Text == m_parser.BaseStream.NewLineStr ) )
				{
					break;
				}

        lPositionBeforeNewLine = m_parser.BaseStream.Position;
				lexem.Column = iCurColumn;
        result.Add( lexem );
				iCurColumn += lexem.Length;
      }

      return result;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Raises LineDeleted event and disposes line.
    /// </summary>
    public void DeleteSelf()
    {
			if( m_parser != null )
			{
				try
				{
					if( LineDeleted != null )
					{
						LineDeleted( this, EventArgs.Empty );
					}
				}
				finally
				{
					m_parser.DeleteLine( this );
				}
			}
    }
    /// <summary>
    /// Searches lexem, that contains given column index.
    /// </summary>
    /// <param name="column">Needed column.</param>
    /// <returns>Found lexem, or null if needed column is in virtual space.</returns>
    public virtual ILexem FindLexemByColumn( int column )
    {
			ILexem result = null;
      int iColumn = 1;

      foreach( ILexem lexem in LineLexems )
      {
        int length = m_parser.GetLexemLength( lexem );
				if( column >= iColumn && column < iColumn + length )
				{
					result = lexem;
					break;
				}
        iColumn += length;
      }

      return result;
    }
    /// <summary>
    /// Compares two ILexemLine objects, or ILexemLine object and integer line number.
    /// </summary>
    /// <param name="obj">ILexemLine object or integer line number.</param>
    /// <returns>Standart comparision result.</returns>
    public int CompareTo( object obj )
    {
      if( obj == null ) throw new ArgumentNullException( "obj" );

      ILexemLine line = obj as ILexemLine;
      int iLine;

			if( line == null )
			{
				iLine = ( int )obj;
			}
			else
			{
				iLine = line.LineIndex;
			}

      return LineIndex.CompareTo( iLine );
    }
    /// <summary>
    /// Gets stack copy for the lexem at the specified column.
    /// </summary>
    /// <param name="column">Needed column.</param>
    /// <returns>Copy of the stack.</returns>
    public virtual ConfigStack GetStackByColumn( int column )
    {
      throw new NotSupportedException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_4 );
    }
    #endregion
 
		#region Event Handlers
		/// <summary>
		/// Handler of the Deleted event of the StartPoint.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="lNewOffset"></param>
		private void PointDeleted( ParsePoint point, long lNewOffset )
		{
			DeleteSelf();
		}
		/// <summary>
		/// Handler of the ParsePointOffsetChanged event of the stream.
		/// </summary>
		/// <param name="sender">Changed ParsePoint.</param>
		/// <param name="e">Change.</param>
		protected virtual void PointPositionChanged( object sender, ParsePointParameterChangedEventArgs e )
		{
			if( e.PositionChanged || e.LineChanged ) DeleteSelf();
		}
		#endregion
	}
}