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
using System.Text.RegularExpressions;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Implementation.IO;

namespace Syncfusion.Windows.Forms.Edit.Utils.CodeSnippets
{
	/// <summary>
	/// Manages work with code snippets.
	/// </summary>
	public class CodeSnippetsManager
	{
		#region Internal Classes
		/// <summary>
		/// Class for holding information about one snippet member.
		/// </summary>
		public class SnippetMember
		{
			#region Class Members
			/// <summary>
			/// Indicates whether this instance is a member of templete.
			/// </summary>
			private bool m_bIsTemplateMember;
			/// <summary>
			/// Text of snippet member.
			/// </summary>
			private string m_strText;
            /// <summary>
            /// Text of snippet ToolTip.
            /// </summary>
            private string m_strToolTip;
			/// <summary>
			/// Name of template member. Can be empty.
			/// </summary>
			private string m_strName;
			/// <summary>
			/// Start parse point of snippet member in parser.
			/// </summary>
			private IParsePoint m_startPoint;
			/// <summary>
			/// End parse point of snippet member in parser.
			/// </summary>
			private IParsePoint m_endPoint;
			/// <summary>
			/// Offset of start point of the snippet member.
			/// </summary>
			private long m_startOffset;
			#endregion

			#region Class Properties
			/// <summary>
			/// Gets or sets bool indicating whether this instance is a member of templete.
			/// </summary>
			public bool IsTemplateMember
			{
				get
				{
					return m_bIsTemplateMember;
				}
				set
				{
					m_bIsTemplateMember = value;
				}
			}
			/// <summary>
			/// Gets or sets text of snippet member.
			/// </summary>
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
			/// Gets or sets name of template member.
			/// </summary>
			public string Name
			{
				get
				{
					return m_strName;
				}
				set
				{
					m_strName = value;
				}
			}
            /// <summary>
            /// Gets or sets name of template member.
            /// </summary>
            public string ToolTip
            {
                get
                {
                    return m_strToolTip;
                }
                set
                {
                    m_strToolTip = value;
                }
            }
			/// <summary>
			/// Gets or sets start parse point of snippet member in parser.
			/// </summary>
			public IParsePoint StartPoint
			{
				get
				{
					return m_startPoint;
				}
				set
				{
					m_startPoint = value;
				}
			}
			/// <summary>
			/// Gets or sets end parse point of snippet member in parser.
			/// </summary>
			public IParsePoint EndPoint
			{
				get
				{
					return m_endPoint;
				}
				set
				{
					m_endPoint = value;
				}
			}
			#endregion

			#region Class Internal Properties
			/// <summary>
			/// Gets or sets offset of start point of the snippet member.
			/// </summary>
			internal long StartOffset
			{
				get
				{
					return m_startOffset;
				}
				set
				{
					m_startOffset = value;
				}
			}
			#endregion

			#region Class Initializatiion
			/// <summary>
			/// Creates and initializes new instance of SnippetMember.
			/// </summary>
			/// <param name="bIsTemplateMember">Bool indicating whether this is a member of template.</param>
			/// <param name="strText">Text of the snippet member.</param>
			public SnippetMember( bool bIsTemplateMember, string strText )
				: this( bIsTemplateMember, strText, string.Empty,string.Empty )
			{
			}
			/// <summary>
			/// Creates and initializes new instance of SnippetMember.
			/// </summary>
			/// <param name="bIsTemplateMember">Bool indicating whether this is a member of template.</param>
			/// <param name="strText">Text of the snippet member.</param>
			/// <param name="strName">Name of the template member.</param>
            /// <param name="strTooltip">tooltip.</param>
			public SnippetMember( bool bIsTemplateMember, string strText, string strName ,string strTooltip)
			{
				if( strText == null ) throw new ArgumentNullException( "strText" );
				if( strName == null ) throw new ArgumentNullException( "strName" );

				m_bIsTemplateMember = bIsTemplateMember;
				m_strText = strText;
				m_strName = strName;
                m_strToolTip = strTooltip;
			}
			#endregion
		}

		/// <summary>
		/// Snippet member that indicates place for cursor after work with code snippet
		/// </summary>
		public class EndSnippetMember
			: SnippetMember
		{
			/// <summary>
			/// Creates new instance of EndSnippetMember.
			/// </summary>
			public EndSnippetMember()
				: base( true, string.Empty )
			{
			}
		}
		#endregion

		#region Class Constants
		/// <summary>
		/// Regular expresson for template members search.
		/// </summary>
		const string DEF_TEMPLATE_MEMBER_REGEX = @"\$.*?\$";
		/// <summary>
		/// Number of symbols in template member opening string.
		/// </summary>
		const int DEF_TEMPLATE_OPEN_CHARS_COUNT = 1;
		/// <summary>
		/// Number of symbols in template member closing string.
		/// </summary>
		const int DEF_TEMPLATE_CLOSE_CHARS_COUNT = 1;
		/// <summary>
		/// Chars that should be trimmed in literal name.
		/// </summary>
		private readonly char[] DEF_CHARS_ARRAY_TO_TRIM = new char[] { '$' };
		/// <summary>
		/// Mark of selected text.
		/// </summary>
		const string DEF_STR_TEMPLATE_SELECTED = "selected";
		/// <summary>
		/// Mark of ending cursor point.
		/// </summary>
		const string DEF_STR_TEMPLATE_END = "end";
		#endregion

		#region Class Static Members
		/// <summary>
		/// Regex instance for template members search.
		/// </summary>
		private static Regex _templateMemberRegex = new Regex( DEF_TEMPLATE_MEMBER_REGEX );
		/// <summary>
		/// Pen for drawing template highlight border.
		/// </summary>
		private static Pen _templateHighlightBorderPen = new Pen( Color.FromArgb( 80, Color.Red ) );
		/// <summary>
		/// Brush for drawing template highlight.
		/// </summary>
		private static Brush _templateHighlightBrush = new SolidBrush( Color.FromArgb( 30, Color.Red ) );
		#endregion

		#region Class Members
		/// <summary>
		/// Indicates whether code snippets manager is active and processes keys.
		/// </summary>
		private bool m_bActivated;
		/// <summary>
		/// Underlying StreamEditControl.
		/// </summary>
		private StreamEditControl m_control;
		/// <summary>
		/// List of members of current snippet.
		/// </summary>
		private ArrayList m_curSnippetMembers = new ArrayList();
		/// <summary>
		/// List of members of current snippet template.
		/// </summary>
		internal ArrayList m_curTemplateMembers = new ArrayList();
		/// <summary>
		/// Index of current snippet member that is edited.
		/// </summary>
		private int m_curTemplateMemberIndex;
		/// <summary>
		/// Start parse point of current active snippet.
		/// </summary>
		private IParsePoint m_startPoint;
		/// <summary>
		/// End parse point of current active snippet.
		/// </summary>
		private IParsePoint m_endPoint;
		/// <summary>
		/// Indicates whether template member is just selected.
		/// </summary>
		private bool m_bTemplateJustSelected;
		/// <summary>
		/// Offset of start point. Used when start point is deleted and should be reassigned.
		/// </summary>
		private long m_startOffset;
		/// <summary>
		/// Currently activated code snippet.
		/// </summary>
		private CodeSnippet m_curCodeSnippet;
		/// <summary>
		/// Place to put cursor to after snippet deactivation.
		/// </summary>
		private IParsePoint m_cursorEndPoint;
		/// <summary>
		/// Determines whether code snippet template member text is currently being changed.
		/// Snippet shouldn't be deactivated on start point deleting in this case.
		/// </summary>
		private bool m_bTemplateMembersChanging;
		/// <summary>
		/// If set to false, cursor isn't put to the end point of code snippet after deactivating.
		/// </summary>
		private bool m_bPutCursorToEndPoint = true;
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets or sets bool indicating whether code snippets manager is activated and processes keys.
		/// </summary>
		public bool Activated
		{
			get
			{
				return m_bActivated;
			}
			set
			{
				if( m_bActivated != value )
				{
					if( !value && CodeSnippetDeactivating != null )
						CodeSnippetDeactivating( this, new CodeSnippetsEventArgs( m_curCodeSnippet ) );

					m_bActivated = value;

					if( !value && m_bPutCursorToEndPoint )
					{
						PutCursorToEndPoint();
					}

					m_bPutCursorToEndPoint = true;
					m_control.InvalidateAll();
				}
			}
		}
		/// <summary>
		/// Gets or sets start parse point of current active snippet.
		/// </summary>
		public IParsePoint StartPoint
		{
			get
			{
				return m_startPoint;
			}
			set
			{
				if( value == null ) throw new ArgumentNullException( "StartPoint" );
				if( !value.IsValid ) throw new ArgumentException( "StartPoint" );

				m_startPoint = value;
				m_startOffset = m_startPoint.Offset;
			}
		}
		/// <summary>
		/// Gets or sets end parse point of current active snippet.
		/// </summary>
		public IParsePoint EndPoint
		{
			get
			{
				return m_endPoint;
			}
			set
			{
				if( value == null ) throw new ArgumentNullException( "EndPoint" );
				if( !value.IsValid ) throw new ArgumentException( "EndPoint" );

				m_endPoint = value;
			}
		}
		/// <summary>
		/// Gets list of current snippet members.
		/// </summary>
		public ArrayList SnippetMembers
		{
			get
			{
				return m_curSnippetMembers;
			}
		}
		/// <summary>
		/// Gets or sets place to put cursor to after snippet deactivation.
		/// </summary>
		public IParsePoint CursorEndPoint
		{
			get
			{
				return m_cursorEndPoint;
			}
			set
			{
				m_cursorEndPoint = value;
			}
		}
		#endregion

		#region Class Initialization
		/// <summary>
		/// Creates new instance of CodeSnippetsManager.
		/// </summary>
		///<param name="control">Underlying stream edit control.</param>
		public CodeSnippetsManager( StreamEditControl control )
		{
			if( control == null ) throw new ArgumentNullException( "control" );

			m_control = control;
		}
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Activates code snippets manager.
		/// </summary>
		/// <param name="snippet">Code snippet.</param>
		/// <param name="selected">Selected text.</param>
		public void Activate( CodeSnippet snippet, string selected )
		{
			if( snippet == null ) throw new ArgumentNullException( "snippet" );

			if( CodeSnippetActivating != null )
			{
				CancellableCodeSnippetsEventArgs args = new CancellableCodeSnippetsEventArgs( snippet );
				CodeSnippetActivating( this, args );
				if( args.Cancel ) return;
			}

			m_curCodeSnippet = snippet;

			ExtractSnippetMembers( snippet, selected );

			m_control.InsertCodeSnippet();

			foreach( SnippetMember snippetMember in SnippetMembers )
			{
				if( snippetMember.EndPoint != null )
				{
					snippetMember.EndPoint.Deleted += new ParsePointDeletedEventHandler( EndPoint_Deleted );
				}
			}

			m_startPoint.Deleted += new ParsePointDeletedEventHandler( ParsePointDeleted );
			m_endPoint.Deleted += new ParsePointDeletedEventHandler( ParsePointDeleted );

			m_curTemplateMembers.Clear();

			foreach( SnippetMember snippetMember in m_curSnippetMembers )
			{
				if( snippetMember.IsTemplateMember ) m_curTemplateMembers.Add( snippetMember );
			}

			m_curTemplateMemberIndex = 0;

			this.Activated = FindNextTemplateMember();

			if( !this.Activated )
			{
				PutCursorToEndPoint();
			}
		}
		/// <summary>
		/// Finds the next template member in text and puts cursor there.
		/// </summary>
		/// <returns>True if succeeds, otherwise false.</returns>
		public bool FindNextTemplateMember()
		{
			if( m_curTemplateMemberIndex == m_curTemplateMembers.Count )
			{
				return false;
			}

			SnippetMember snippetMember = ( SnippetMember )m_curTemplateMembers[ m_curTemplateMemberIndex ];

			CoordinatePoint coorStart = m_control.Parser.GetCoordinatePoint( snippetMember.StartPoint );

			m_control.CurrentPosition = new Point( coorStart.VirtualColumn, coorStart.VirtualLine );

			m_bTemplateJustSelected = true;

			m_control.InvalidateAll();

			return true;
		}
		/// <summary>
		/// Processes keys.
		/// </summary>
		/// <param name="keys">Keys enum to process.</param>
		public void ProcessKeys( KeyEventArgs keys )
		{
			Keys key = keys.KeyCode;

			if( !m_bActivated )
			{
				if( key == Keys.Tab )
				{
					ILexem lex = m_control.GetLexemUnderCursor();
					if( lex != null && m_control.CurrentColumn == lex.Column + lex.Length )
					{
						string text = lex.Text;

						CodeSnippet snippet = m_control.Language.SnippetsContainer.GetSnippetByShortcut( text );
						if( snippet != null )
						{
							m_control.DeleteWordLeft();
							Activate( m_control.Language.SnippetsContainer.GetSnippetByShortcut( text ), m_control.SelectedText );
							keys.Handled = true;
						}
					}
				}
			}
			else
			{
				if( !AllowedKey( keys ) )
				{
					this.Activated = false;
				}
				else
				{
					if( key == Keys.Enter )
					{
						UpdateTemplateNames();
						ReinitParsePoints();

						if( m_curTemplateMemberIndex < m_curTemplateMembers.Count - 1 )
						{
							SnippetMember oldMember = ( SnippetMember )m_curTemplateMembers[ m_curTemplateMemberIndex ];
							SnippetMember newMember = ( SnippetMember )m_curTemplateMembers[ m_curTemplateMemberIndex + 1 ];

							NewSnippetMemberHighlightingEventArgs args =
								new NewSnippetMemberHighlightingEventArgs( m_curCodeSnippet, oldMember, newMember );

							if( NewSnippetMemberHighlighting != null ) NewSnippetMemberHighlighting( this, args );

							if( args.Cancel )
							{
								keys.Handled = true;
								return;
							}
						}

						m_curTemplateMemberIndex++;
						this.Activated = FindNextTemplateMember();
						keys.Handled = true;
					}
					else if( key == Keys.Delete || key == Keys.Back )
					{
						SnippetMember snippetMember = ( SnippetMember )m_curTemplateMembers[ m_curTemplateMemberIndex ];
						if( snippetMember.StartPoint != snippetMember.EndPoint )
						{
							keys.Handled = DeleteSnippetMemberText();
							ReinitParsePoints();
						}
						else
						{
							this.Activated = false;
							keys.Handled = true;
						}
					}
					else
					{
						ReinitParsePoints();

						SnippetMember snippetMember = ( SnippetMember )m_curTemplateMembers[ m_curTemplateMemberIndex ];
						CoordinatePoint coorStart = m_control.Parser.GetCoordinatePoint( snippetMember.StartPoint );
						CoordinatePoint coorEnd = m_control.Parser.GetCoordinatePoint( snippetMember.EndPoint );

						Point curPos = m_control.CurrentPosition;

						if( ( key == Keys.Left || key == Keys.Back ) && curPos.X == coorStart.VirtualColumn
							|| ( key == Keys.Right || key == Keys.Delete ) && curPos.X == coorEnd.VirtualColumn )
						{
							this.Activated = false;
						}
					}
				}
			}
		}
		/// <summary>
		/// Processes keys.
		/// </summary>
		/// <param name="keys">Keys enum to process.</param>
		public void ProcessKeys( KeyPressEventArgs keys )
		{
			if( !m_bActivated ) return;

			DeleteSnippetMemberText();
		}
		/// <summary>
		/// Marks area of code snippet.
		/// </summary>
		/// <param name="g">Graphics object to draw at.</param>
		public void MarkCodeSnippet( Graphics g )
		{
			if( g == null ) throw new ArgumentNullException( "g" );

			if( m_bActivated )
			{
				ReinitParsePoints();

				Matrix matr = g.Transform;
				g.TranslateTransform( m_control.AutoScrollPosition.X, 0 );

				GraphicsPath path = null;

				if( m_control.DrawCodeSnippetBorder )
				{
					path = m_control.GetTextDrawPath(
						m_control.Parser.GetCoordinatePoint( this.StartPoint ), m_control.Parser.GetCoordinatePoint( this.EndPoint ) );

					g.DrawPath( Pens.Blue, path );
					path.Dispose();
				}

				SnippetMember snippetMember = ( SnippetMember )m_curTemplateMembers[ m_curTemplateMemberIndex ];

				path = m_control.GetTextDrawPath(
					m_control.Parser.GetCoordinatePoint( snippetMember.StartPoint ),
					m_control.Parser.GetCoordinatePoint( snippetMember.EndPoint ) );

				if( path != null )
				{
					g.FillPath( _templateHighlightBrush, path );
					g.DrawPath( _templateHighlightBorderPen, path );
					path.Dispose();
				}

				for( int i = m_curTemplateMemberIndex + 1; i < m_curTemplateMembers.Count; i++ )
				{
					snippetMember = ( SnippetMember )m_curTemplateMembers[ i ];

					path = m_control.GetTextDrawPath(
						m_control.Parser.GetCoordinatePoint( snippetMember.StartPoint ),
						m_control.Parser.GetCoordinatePoint( snippetMember.EndPoint ) );

					if( path != null )
					{
						g.DrawPath( Pens.Green, path );
						path.Dispose();
					}
				}

				g.Transform = matr;
				matr.Dispose();
			}
		}
		/// <summary>
		/// Deletes currently active highlighted text of code snippet manager.
		/// </summary>
		/// <returns>True if succeeds.</returns>
		public bool DeleteSnippetMemberText()
		{
			bool bResult = false;

			if( m_bActivated && m_bTemplateJustSelected )
			{
				m_control.LockUpdate();

				SnippetMember snippetMember = ( SnippetMember )m_curTemplateMembers[ m_curTemplateMemberIndex ];
				CoordinatePoint coorStart = m_control.Parser.GetCoordinatePoint( snippetMember.StartPoint );
				CoordinatePoint coorEnd = m_control.Parser.GetCoordinatePoint( snippetMember.EndPoint );

				m_control.DeleteText( coorStart, coorEnd );
				snippetMember.StartOffset = snippetMember.EndPoint.Offset;

				m_control.UnlockUpdate();
				m_bTemplateJustSelected = false;
				bResult = true;
			}

			return bResult;
		}
		/// <summary>
		/// Adds new code snippet to current language.
		/// </summary>
		/// <param name="title">Title of code snippet.</param>
		/// <param name="literals">List of literals.</param>
		/// <param name="code">Code of snippet.</param>
		public void AddCodeSnippet( string title, ArrayList literals, string code )
		{
			if( title == null || title == string.Empty ) throw new ArgumentNullException( "title" );
			if( code == null || code == string.Empty ) throw new ArgumentOutOfRangeException( "code" );

			m_control.Language.AddCodeSnippet( title, literals, code );
		}
		/// <summary>
		/// Gets code snippet by it's title.
		/// </summary>
		/// <param name="title">Title of code snippet that has to be found.</param>
		/// <returns>Needed code snippet or null if there's no snippet with given name.</returns>
		public CodeSnippet GetSnippetByTitle( string title )
		{
			if( title == null || title == string.Empty ) throw new ArgumentNullException( "title" );

			return m_control.Language.SnippetsContainer.GetSnippetByTitle( title );
		}
		/// <summary>
		/// Changes text of all template members with defined name.
		/// </summary>
		/// <param name="memberName">Name of template member.</param>
		/// <param name="newText">New text.</param>
		public void ChangeTemplateText( string memberName, string newText )
		{
			if( memberName == null ) throw new ArgumentNullException( "memberName" );
			if( newText == null ) throw new ArgumentNullException( "newText" );
			if( memberName == string.Empty ) throw new ArgumentOutOfRangeException( "membername" );

			if( !m_bActivated ) return;
			ReinitParsePoints();

			CodeSnippetTemplateTextChangingEventArgs args =
				new CodeSnippetTemplateTextChangingEventArgs( m_curCodeSnippet, memberName, newText );

			if( CodeSnippetTemplateTextChanging != null ) CodeSnippetTemplateTextChanging( this, args );

			if( !args.Cancel )
			{
				m_bTemplateJustSelected = false;

				SnippetMember snippetMember = null;

				m_control.LockUpdate();
				m_bTemplateMembersChanging = true;

				for( int i = 0 ; i < m_curTemplateMembers.Count ; i++ )
				{
					snippetMember = ( SnippetMember )m_curTemplateMembers[ i ];

					if( snippetMember.Name == memberName )
					{
						CoordinatePoint coorStart = m_control.Parser.GetCoordinatePoint( snippetMember.StartPoint );
						CoordinatePoint coorEnd = m_control.Parser.GetCoordinatePoint( snippetMember.EndPoint );
						 m_control.DeleteText(coorStart, coorEnd);
						snippetMember.StartOffset = snippetMember.EndPoint.Offset;

						m_control.InsertText( coorStart.VirtualLine, coorStart.VirtualColumn, newText );

					}
				}

				m_bTemplateMembersChanging = false;
				m_control.UnlockUpdate();

				ReinitParsePoints();
			}
		}
		#endregion

		#region Clss Utiliy Methods
		/// <summary>
		/// Checks whether key is allowed when code snippets manager is activated.
		/// </summary>
		/// <param name="keys">Key to check.</param>
		/// <returns>True if the key is allowed, otherwise false.</returns>
		private bool AllowedKey( KeyEventArgs keys )
		{
			bool result = true;
			Keys key = keys.KeyCode;

			if( key == Keys.Down
				|| key == Keys.End
				|| key == Keys.Escape
				|| key == Keys.Home
				|| key == Keys.PageDown
				|| key == Keys.PageUp
				|| key == Keys.Up
				)
			{
				result = false;
			}

			if( ( key == Keys.Tab && keys.Shift ) )
			{
				result = false;
				m_bPutCursorToEndPoint = false;
			}

			return result;
		}
		/// <summary>
		/// Extracts snippet members and fills list of snippet members.
		/// </summary>
		/// <param name="snippet">Code snippet to extract members of.</param>
		/// <param name="selected">Selected text.</param>
		private void ExtractSnippetMembers( CodeSnippet snippet, string selected )
		{
			string template = snippet.Code;

			m_curSnippetMembers.Clear();

			// Parse template and fill m_curSnippetMembers.

			int curMatchPos = 0;
			Match match = _templateMemberRegex.Match( template, curMatchPos );

			while( match.Success )
			{
				int matchIndex = match.Index;

				// If there is text before template member.
				if( matchIndex > curMatchPos )
				{
					m_curSnippetMembers.Add(
						new SnippetMember( false, template.Substring( curMatchPos, matchIndex - curMatchPos ) ) );
				}

				string templateMember = template.Substring(
					matchIndex + DEF_TEMPLATE_OPEN_CHARS_COUNT,
					match.Length - DEF_TEMPLATE_OPEN_CHARS_COUNT - DEF_TEMPLATE_CLOSE_CHARS_COUNT );

				templateMember = templateMember.Trim( DEF_CHARS_ARRAY_TO_TRIM );

				if( templateMember == DEF_STR_TEMPLATE_SELECTED )
				{
					if( selected != null && selected != string.Empty )
					{
						m_curSnippetMembers.Add( new SnippetMember( false, selected ) );
					}
				}
				else if( templateMember == DEF_STR_TEMPLATE_END )
				{
					m_curSnippetMembers.Add( new EndSnippetMember() );
				}
				else
				{
					m_curSnippetMembers.Add(new SnippetMember(true, snippet.GetLiteralDefault(templateMember), templateMember, snippet.GetLiteralToolTip(templateMember)));
				}

				curMatchPos = match.Index + match.Length;
				match = _templateMemberRegex.Match( template, curMatchPos );
			}

			if( template.Length != curMatchPos )
			{
				m_curSnippetMembers.Add( new SnippetMember( false, template.Substring( curMatchPos ) ) );
			}

			// If one snippet member has "\n" in the middle, it must be divided into two or more members.
			// Also all "\r" marls are deleted.

			SnippetMember snippetMember;

			for( int i = 0 ; i < m_curSnippetMembers.Count ; i++ )
			{
				snippetMember = ( SnippetMember )m_curSnippetMembers[ i ];

				snippetMember.Text = snippetMember.Text.Replace( "\r", string.Empty );

				int nlIndex = snippetMember.Text.IndexOf( "\n" );

				if( nlIndex != -1 && nlIndex != snippetMember.Text.Length - 1 )
				{
					SnippetMember snippetToInsert = new SnippetMember( false, snippetMember.Text.Substring( nlIndex + 1 ) );
					m_curSnippetMembers.Insert( i + 1, snippetToInsert );
					snippetMember.Text = snippetMember.Text.Remove( nlIndex + 1, snippetMember.Text.Length - nlIndex - 1 );
				}
			}
		}
		/// <summary>
		/// Reinitializes start parse points of template members using start offset.
		/// </summary>
		private void ReinitParsePoints()
		{
			foreach( SnippetMember snippetMember in m_curTemplateMembers )
			{
				if( snippetMember.StartPoint == null || !snippetMember.StartPoint.IsValid || snippetMember.StartPoint.Offset == snippetMember.EndPoint.Offset )
				{
					snippetMember.StartPoint = m_control.Parser.BaseStream.GetParsePoint( snippetMember.StartOffset );
				}
			}

			if( !m_startPoint.IsValid )
			{
				m_startPoint = m_control.Parser.BaseStream.GetParsePoint( m_startOffset );
			}
		}
		/// <summary>
		/// Updates text for named snippets.
		/// </summary>
		private void UpdateTemplateNames()
		{
			ReinitParsePoints();

			SnippetMember snippetMember = ( SnippetMember )m_curTemplateMembers[ m_curTemplateMemberIndex ];

			string name = snippetMember.Name;

			if( name != string.Empty )
			{
				string text = m_control.Parser.BaseStream.GetTextInRange(
					snippetMember.StartPoint, snippetMember.EndPoint, false );

				ChangeTemplateText( name, text );
			}
		}
		/// <summary>
		/// Puts control cursor to the end point.
		/// </summary>
		private void PutCursorToEndPoint()
		{
			if( m_cursorEndPoint != null && m_cursorEndPoint.IsValid && m_control.Parser != null )
			{
				m_control.CurrentPosition = m_control.Parser.PhysicalToVirtual( m_cursorEndPoint );
				m_cursorEndPoint = null;
			}
		}
		#endregion

		#region Class Event Handlers
		/// <summary>
		/// Deactivates manager.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="lNewOffset"></param>
		private void ParsePointDeleted( ParsePoint point, long lNewOffset )
		{
			if( !m_bTemplateJustSelected && !m_bTemplateMembersChanging ) this.Activated = false;
		}
		/// <summary>
		/// Deactivates snippet.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="lNewOffset"></param>
		private void EndPoint_Deleted( ParsePoint point, long lNewOffset )
		{
			this.Activated = false;
		}
		#endregion

		#region Class Events
		/// <summary>
		/// Raised when new snippet member has to be highlighted.
		/// </summary>
		public NewSnippetMemberHighlightingEventHandler NewSnippetMemberHighlighting;
		/// <summary>
		/// Raised when text of template member is to be changed.
		/// </summary>
		public CodeSnippetTemplateTextChangingEventHandler CodeSnippetTemplateTextChanging;
		/// <summary>
		/// Raised when code snippet is to be activated.
		/// </summary>
		public CancellableCodeSnippetsEventHandler CodeSnippetActivating;
		/// <summary>
		/// Raised when code snippet is to be deactivated.
		/// </summary>
		public CodeSnippetsEventHandler CodeSnippetDeactivating;
		#endregion
	}
}