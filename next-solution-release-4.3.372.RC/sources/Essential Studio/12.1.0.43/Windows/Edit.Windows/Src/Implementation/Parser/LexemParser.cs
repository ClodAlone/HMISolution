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

using System.Collections;
using System;
using System.Drawing;
using System.Xml;
using System.Diagnostics;
using System.Text.RegularExpressions;

using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Implementation.IO;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Utils;
using Syncfusion.Windows.Forms.Edit.Implementation.Formatting;
using Syncfusion.Windows.Forms.Edit.Implementation.Config;

namespace Syncfusion.Windows.Forms.Edit.Implementation.Parser
{
	#region *** ConfigStack
	/// <summary>
	/// Stack with configuration.
	/// </summary>
	public class ConfigStack
		: Stack
	{
		#region Methods Hiding
		/// <summary>
		/// Hides Push method.
		/// </summary>
		/// <param name="obj">Object to push.</param>
		protected new void Push( object obj )
		{
			base.Push( obj );
		}
		/// <summary>
		/// Hides Contains method.
		/// </summary>
		/// <param name="obj">Object to check.</param>
		/// <returns>True if stack contains object.</returns>
		protected new bool Contains( object obj )
		{
			return base.Contains( obj );
		}
		#endregion

		#region Public Region
		/// <summary>
		/// Pop item from the stack.
		/// </summary>
		/// <returns>IStackData instance.</returns>
		public new IStackData Pop()
		{
			return ( IStackData )base.Pop();
		}
		/// <summary>
		/// Peeks data from stack.
		/// </summary>
		/// <returns>IStackData object.</returns>
		public new IStackData Peek()
		{
			return ( IStackData )base.Peek();
		}
		/// <summary>
		/// Pushes data to the stack.
		/// </summary>
		/// <param name="data">IStackData instance.</param>
		public void Push( IStackData data )
		{
			base.Push( data );
		}
		/// <summary>
		/// Creates data object and pushes it to the stack.
		/// </summary>
		/// <param name="config">Configurtion of the stack's element, can not be null.</param>
		/// <param name="lexem">Lexem instance. Can be null.</param>
		/// <param name="point">Location.</param>
		/// <param name="firstConfig">Configuration of the first lexem in sequence.</param>
		public void Push( IConfigLexem config, ILexem lexem, IParsePoint point, IConfigLexem firstConfig )
		{
			IStackData datum = new LexemParser.ConfigLexem_Lexem_Pair( config, lexem, point, firstConfig );
			base.Push( datum );
		}
		/// <summary>
		/// Creates data object and pushes it to the stack.
		/// </summary>
		/// <param name="config">Configurtion of the stack`s element, can not be null</param>
		/// <param name="lexem">Lexem instance. Can be null.</param>
		/// <param name="point">Location.</param>
		public void Push( IConfigLexem config, ILexem lexem, IParsePoint point )
		{
			IStackData datum = new LexemParser.ConfigLexem_Lexem_Pair( config, lexem, point );
			base.Push( datum );
		}
		/// <summary>
		/// Checks whether stack contains given data.
		/// </summary>
		/// <param name="data">IStackData instance.</param>
		/// <returns>True if stack contains given data, otherwise false.</returns>
		public bool Contains( IStackData data )
		{
			return base.Contains( data );
		}
		#endregion

		#region Initialization & Finalization
		/// <summary>
		/// Creates and initializes new instance of the class.
		/// </summary>
		public ConfigStack()
			: base()
		{ }
		#endregion

		#region Overrides
		/// <summary>
		/// Gets stack hashcode.
		/// </summary>
		/// <returns>Hash code.</returns>
		public override int GetHashCode()
		{
			return Count;
		}
		/// <summary>
		/// Checks whether 2 call stacks are equal.
		/// </summary>
		/// <param name="obj">Object to check.</param>
		/// <returns>True of stacks are equal.</returns>
		public override bool Equals( object obj )
		{
			ConfigStack stack = ( ConfigStack )obj;
			bool equal = ( stack.Count == this.Count );

			if( equal )
			{
				object[] arrayFirst = this.ToArray();
				object[] arraySecond = stack.ToArray();

				for( int i = 0, count = Count; i < count; i++ )
				{
					IStackData dataFirst = ( IStackData )arrayFirst[ i ];
					IStackData dataSecond = ( IStackData )arraySecond[ i ];

					if( dataFirst.Config.ID != dataSecond.Config.ID )
					{
						equal = false;
						break;
					}
				}

				arrayFirst = null;
				arraySecond = null;
			}

			return equal;
		}
		/// <summary>
		/// Clones object.
		/// </summary>
		/// <returns>Clone of the stack.</returns>
		public override object Clone()
		{
			object[] objs = this.ToArray();

			ConfigStack stack1 = new ConfigStack();

			for( int i = objs.Length - 1; i >= 0; i-- )
				stack1.Push( objs[ i ] );

			return stack1;
		}
		#endregion
	}
	#endregion

	#region *** ILexemEnumeratorParserInfo
	/// <summary>
	/// Interface used for acquiring current parser state from lexem enumerator.
	/// </summary>
	public interface ILexemEnumeratorParserInfo
	{
		#region Properties
		/// <summary>
		/// Gets current stack.
		/// </summary>
		ConfigStack CurrentStack { get; }
		/// <summary>
		/// Gets current stream position.
		/// </summary>
		long CurrentPosition { get; }
		#endregion
	}
	#endregion

	#region *** IStackData
	/// <summary>
	/// Public interface for accessing data, stored in the parser's stack.
	/// </summary>
	public interface IStackData
	{
		#region Properties
		/// <summary>
		/// Gets ParsePoint that points to the position of the lexem.
		/// </summary>
		IParsePoint Location { get; }
		/// <summary>
		/// Gets configuration of the stack element.
		/// </summary>
		IConfigLexem Config { get; }
		/// <summary>
		/// Gets configuration of the first lexem in sequence.
		/// </summary>
		IConfigLexem FirstConfig { get; }
		/// <summary>
		/// Gets lexem instance. Can be null for language configuration.
		/// </summary>
		ILexem Lexem { get; }
		#endregion
	}
	#endregion

	#region *** LexemParser
	/// <summary>
	/// Class that is used to parse stream to lexems.
	/// </summary>
	public class LexemParser
		: ILexemParser
		, IEnumerable
		, IXMLDataProvider
		, ILongOperationControllerInternal
	{
		#region Classes
		/// <summary>
		/// Class that represents enumerator for lexem parser. Is used to make by-lexem parsing of the file, based on some starting stack.
		/// </summary>
		public class LexemParserEnumerator
			: IEnumerator
			, ILexemEnumeratorParserInfo
		{
			#region Fields
			/// <summary>
			/// Currently opened complex lexems.
			/// </summary>
			private ConfigStack m_stack;
			/// <summary>
			/// Calling parser instance.
			/// </summary>
			private LexemParser m_parser;
			/// <summary>
			/// Initial stack.
			/// </summary>
			private ConfigStack m_initStack;
			/// <summary>
			/// Initial position in stream.
			/// </summary>
			private long m_initPosition;
			/// <summary>
			/// Current lexem.
			/// </summary>
			private ILexem m_current;
			/// <summary>
			/// Current position in stream. Used to ensure that reading of data is correct.
			/// </summary>
			private long m_currentPosition;
			/// <summary>
			/// Version of the stream. Used to ensure that data reading is correct.
			/// </summary>
			private int m_streamVersion;
			/// <summary>
			/// Size of the new-line symbol.
			/// </summary>
			private int m_newLineSize;
			#endregion

			#region Class Properties
			/// <summary>
			/// Gets current stack.
			/// </summary>
			public ConfigStack CurrentStack
			{
				get
				{
					return m_stack as ConfigStack;
				}
			}
			/// <summary>
			/// Gets current stream position.
			/// </summary>
			public long CurrentPosition
			{
				get
				{
					return m_currentPosition;
				}
			}
			#endregion

			#region Initialization And Finalization
			/// <summary>
			/// Creates enumerator instance.
			/// </summary>
			/// <param name="stack">Stack, that will be used as start point for parsing.</param>
			/// <param name="parser">Parser instance.</param>
			public LexemParserEnumerator( ConfigStack stack, LexemParser parser )
			{
				if( stack == null ) throw new ArgumentNullException( "stack" );
				if( parser == null ) throw new ArgumentNullException( "parser" );

				m_parser = parser;

				// Stack must be initialized by Reset
				m_stack = null;
				m_initStack = ( ConfigStack )stack.Clone();
				m_initPosition = parser.BaseStream.Position;
				m_streamVersion = parser.BaseStream.Version;
				Reset();
			}
			#endregion

			#region IEnumerator Members
			/// <summary>
			/// Initializes enumerator and parser by initial parameters.
			/// </summary>
			public void Reset()
			{
				m_stack = ( ConfigStack )m_initStack.Clone();
				m_parser.BaseStream.Position = m_initPosition;
				m_parser.m_lastStack = m_stack;
				m_currentPosition = m_initPosition;
				m_newLineSize = m_parser.BaseStream.NewLineSize;
			}
			/// <summary>
			/// Gets current lexem.
			/// </summary>
			public object Current
			{
				get
				{
					if( m_stack == null )
						throw new ApplicationException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_162 );

					return m_current;
				}
			}
			/// <summary>
			/// Moves to the next lexel.
			/// </summary>
			/// <returns>True if moved successfully, false if end of the stream reached.</returns>
			public bool MoveNext()
			{
				if( m_stack == null )
					throw new ApplicationException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_162 );
				if( m_streamVersion != m_parser.BaseStream.Version )
					throw new ApplicationException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_163 );

				m_parser.BaseStream.SetPositionAndResetCache( m_currentPosition );
				m_current = m_parser.GetLexemInternal( m_stack );
				m_currentPosition = m_parser.BaseStream.Position;

				return m_current != null;
			}
			#endregion
		}

		/// <summary>
		/// Single undo item.
		/// </summary>
		public struct UndoItem
		{
			#region Public Fields
			/// <summary>
			/// Start offset of the change.
			/// </summary>
			public long StartOffset;
			/// <summary>
			/// End offset of the change.
			/// </summary>
			public long EndOffset;
			#endregion
		}

		/// <summary>
		/// Class that represents stack elements of the parser.
		/// </summary>
		internal protected class ConfigLexem_Lexem_Pair
			: IStackData
		{
			#region Fields
			/// <summary>
			/// Configurtion of the stack`s element.
			/// </summary>
			private IConfigLexem m_config;
			/// <summary>
			/// Configuration of the first lexem in sequence.
			/// </summary>
			private IConfigLexem m_firstConfig;
			/// <summary>
			/// Lexem instance, for language`s configuration can be null.
			/// </summary>
			private ILexem m_lexem;
			/// <summary>
			/// ParsePoint that points to the position of the lexem.
			/// </summary>
			private IParsePoint m_point;
			#endregion

			#region Properties
			/// <summary>
			/// Gets ParsePoint that points to the position of the lexem.
			/// </summary>
			public IParsePoint Location
			{
				get
				{
					return m_point;
				}
			}
			/// <summary>
			/// Gets configuration of the stack's element.
			/// </summary>
			public IConfigLexem Config
			{
				get
				{
					return m_config;
				}
			}
			/// <summary>
			/// Gets configuration of the first lexem in sequence.
			/// </summary>
			public IConfigLexem FirstConfig
			{
				get
				{
					return m_firstConfig;
				}
			}
			/// <summary>
			/// Gets lexem instance, for language`s configuration can be null.
			/// </summary>
			public ILexem Lexem
			{
				get
				{
					return m_lexem;
				}
			}
			#endregion

			#region Initialization And Finalization
			/// <summary>
			/// Creates instance of the the class and initializes it.
			/// </summary>
			/// <param name="config">Configurtion of the stack's element, can not be null</param>
			/// <param name="lexem">Lexem instance. Can be null.</param>
			/// <param name="point">Location.</param>
			public ConfigLexem_Lexem_Pair( IConfigLexem config, ILexem lexem, IParsePoint point )
			{
				if( config == null ) throw new ArgumentNullException( "config" );

				m_config = config;
				m_lexem = lexem;
				m_point = point;
			}
			/// <summary>
			/// Creates instance of the the class and initializes it.
			/// </summary>
			/// <param name="config">Configurtion of the stack's element, can not be null</param>
			/// <param name="lexem">Lexem instance. Can be null.</param>
			/// <param name="point">Location.</param>
			/// <param name="firstConfig">Configuration of the first lexem in sequence.</param>
			public ConfigLexem_Lexem_Pair( IConfigLexem config, ILexem lexem, IParsePoint point, IConfigLexem firstConfig )
				: this( config, lexem, point )
			{
				m_firstConfig = firstConfig;
			}
			#endregion
		}

		/// <summary>
		/// Class that keeps undo\redo stacks of the parser.
		/// </summary>
		internal class UndoRedoData
			: ICloneable
		{
			#region Fields
			/// <summary>
			/// Stack of the undo.
			/// </summary>
			private Stack m_undo = new Stack();
			/// <summary>
			/// Stack of the redo.
			/// </summary>
			private Stack m_redo = new Stack();
			#endregion

			#region Properties
			/// <summary>
			/// Gets undo stack.
			/// </summary>
			public Stack UndoStack
			{
				get
				{
					return m_undo;
				}
			}
			/// <summary>
			/// Gets redo stack.
			/// </summary>
			public Stack RedoStack
			{
				get
				{
					return m_redo;
				}
			}
			#endregion

			#region ICloneable Members
			/// <summary>
			/// Creates copy of the undo/redo data.
			/// </summary>
			/// <returns></returns>
			public object Clone()
			{
				UndoRedoData data = new UndoRedoData();
				data.m_redo = ( Stack )m_redo.Clone();
				data.m_undo = ( Stack )m_undo.Clone();
				return data;
			}

			#endregion
		}
		#endregion

		#region Constants
		/// <summary>
		/// Maximal distance between last parsed line and requested line to make full text parsing.
		/// </summary>
		protected const int DEF_LINES_BEFORE_SPEEDUP = 200;
		/// <summary>
		/// Maximum count of the lines that can be parsed when reading name of the collapsed region.
		/// </summary>
		private const int DEF_COLLAPSE_NAME_MAX_LINES = 10;
		/// <summary>
		/// Begin block used to identify stack item used to parse plain text.
		/// </summary>
		private const string DEF_CONFIG_NAME_PLAIN_TEXT = "Plain Text Stack Item";
		#endregion

		#region Fields
		/// <summary>
		/// Input source of data.
		/// </summary>
		private StreamsWrapper m_streamsWrapper;
		/// <summary>
		/// Current language configuration.
		/// </summary>
		private IConfigLanguage m_language;
		/// <summary>
		/// Index of the current line.
		/// </summary>
		private int m_curLineIndex;
		/// <summary>
		/// List of the lines.
		/// </summary>
		private ArrayList m_linesList = new ArrayList();
		/// <summary>
		/// Last stack.
		/// </summary>
		private ConfigStack m_lastStack;
		/// <summary>
		/// List of the collapses.
		/// </summary>
		private ArrayList m_collapseList = new ArrayList();
		/// <summary>
		/// Index of the last used collapse region. Used for optimization.
		/// </summary>
		private int m_lastCollapseRegionIndex;
		/// <summary>
		/// Count of collapsed line. If it is -1, it must be recalculated.
		/// </summary>
		private int m_collapsedLines = -1;
		/// <summary>
		/// Flag that specifies, whether collapsing setting are used or not.
		/// </summary>
		private bool m_bCollapsingEnabled = true;
		/// <summary>
		/// Initial state of collapsing.
		/// </summary>
		private bool m_bInitialCollapsing;
		/// <summary>
		/// Count of collapsing locks.
		/// </summary>
		private int m_iCollapsingsLocks;
		/// <summary>
		/// Stack, used to parse plain text.
		/// </summary>
		private ConfigStack m_stackPlainText;
		/// <summary>
		/// Specifies text parsing mode.
		/// </summary>
		private TextParsingMode m_parsingMode = TextParsingMode.FullParsing;
		/// <summary>
		/// Specifies whether consistence checks are locked.
		/// </summary>
		private bool m_bConsistenceChecksLocked;
		/// <summary>
		/// Specifies undo/redo data storage.
		/// </summary>
		private UndoRedoData m_undoredoData;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets text parsing mode. User can select between high parsing speed or high syntax highlighting accuracy.
		/// </summary>
		public TextParsingMode ParsingMode
		{
			get
			{
				return m_parsingMode;
			}
			set
			{
				if( m_parsingMode != value )
				{
					m_parsingMode = value;
				}
			}
		}
		/// <summary>
		/// Gets used format manager.
		/// </summary>
		public IFormatManager Formats
		{
			get
			{
				return m_language;
			}
		}
		/// <summary>
		/// Gets input data source.
		/// </summary>
		public StreamsWrapper BaseStream
		{
			[DebuggerStepThrough]
			get
			{
				return m_streamsWrapper;
			}
		}

		internal bool canUpdate = true;

		private IList collapsedRegionList;

		internal IList CollapsedRegionList
		{
			get
			{
				if (canUpdate)
				{
					collapsedRegionList = GetCollapsedRegionsList();
				}

				canUpdate = false;

				return collapsedRegionList;
			}
		}
        internal bool collapseCollectionUpdate = true;
		/// <summary>
		/// Gets total lines count in text after applying of collapsing.
		/// </summary>
		public int TotalLines
		{
			get
			{
				int linescount = BaseStream.LinesCount;

				if( CollapsingEnabled && ( m_collapsedLines == -1 || m_iCollapsingsLocks > 0 ) )
				{
					int newlinelength = BaseStream.NewLineSize;
					m_collapsedLines = 0;
                    this.canUpdate = collapseCollectionUpdate;
                    foreach( CollapsableRegion region in CollapsedRegionList)
					{
						if( region.Collapsed && region.End != null )
						{
							m_collapsedLines += region.End.Line - region.Start.Line;

							if( region.EndLexem != null && region.EndLexem.Text == BaseStream.NewLineStr )
								m_collapsedLines++;
						}
					}
				}
				
				//Fixed SD3923
                if (linescount - ((CollapsingEnabled) ? m_collapsedLines : 0) == 0)
                {
                    return linescount;
                }
                //End

				return ( linescount - ( ( CollapsingEnabled ) ? m_collapsedLines : 0 ) );
			}
		}
		/// <summary>
		/// Gets or sets current line index
		/// </summary>
		public int CurrentLine
		{
			get
			{
				return m_curLineIndex;
			}
			set
			{
				if( m_curLineIndex != value )
				{
					if( value < 1 || value > TotalLines )
						throw new IndexOutOfRangeException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_164 );

					m_curLineIndex = value;
				}
			}
		}
		/// <summary>
		/// Gets flag whether there are actions to be undone.
		/// </summary>
		public bool CanUndo
		{
			get
			{
				if( !m_streamsWrapper.CanUndo && m_undoredoData.UndoStack.Count > 0 )
				{
					m_undoredoData.UndoStack.Clear();
				}

				return m_undoredoData.UndoStack.Count > 0;
			}
		}
		/// <summary>
		/// Gets sing of redo ability.
		/// </summary>
		public bool CanRedo
		{
			get
			{
				if( !m_streamsWrapper.CanRedo && m_undoredoData.RedoStack.Count > 0 )
				{
					m_undoredoData.RedoStack.Clear();
				}

				return m_undoredoData.RedoStack.Count > 0;
			}
		}
		/// <summary>
		/// Gets or sets collapsing state.
		/// </summary>
		public bool CollapsingEnabled
		{
			get
			{
				return m_bCollapsingEnabled;
			}
			set
			{
				if( m_bCollapsingEnabled != value )
				{
					m_bCollapsingEnabled = value;
					OnCollapsingEnabledChanged();
				}
			}
		}
		/// <summary>
		/// Count of the actions that can be undone.
		/// </summary>
		public int UndoQueueLength
		{
			get
			{
				return ( CanUndo ) ? m_undoredoData.UndoStack.Count : 0;
			}
		}
		/// <summary>
		/// Count of the undone actions that can be redone.
		/// </summary>
		public int RedoQueueLength
		{
			get
			{
				return ( CanRedo ) ? m_undoredoData.RedoStack.Count : 0;
			}
		}
		/// <summary>
		/// Gets or Sets length of one tab symbol.
		/// </summary>
		public int TabLength
		{
			get
			{
				return ( Formats as FormatManager ).SpacesInTab;
			}
			set
			{
				( Formats as FormatManager ).SpacesInTab = value;
			}
		}
		/// <summary>
		/// Specifies whether consistence checks are turned off.
		/// </summary>
		protected internal bool ConsistenceChecksLocked
		{
			get
			{
				return m_bConsistenceChecksLocked;
			}
			set
			{
				if( m_bConsistenceChecksLocked != value )
				{
					m_bConsistenceChecksLocked = value;
					CheckConsistence();
				}
			}
		}
		/// <summary>
		/// List of the lines.
		/// </summary>
		protected ArrayList LinesList
		{
			get
			{
				return m_linesList;
			}
		}
		/// <summary>
		/// Shows whether collapsing are locked.
		/// </summary>
		protected bool IsCollapsingLocked
		{
			get
			{
				return ( m_iCollapsingsLocks != 0 );
			}
		}
		/// <summary>
		/// Gets undo-redo data storage.
		/// </summary>
		internal UndoRedoData UndoData
		{
			get
			{
				return m_undoredoData;
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Event, that is raised when some line was created and added to the internal list.
		/// </summary>
		public event EventHandler LineInstanceCreated;
		/// <summary>
		/// Event, that is raised when line was deleted.
		/// </summary>
		public event EventHandler LineInstanceDeleted;
		/// <summary>
		/// Event, that is raised when count of lines has been changed.
		/// </summary>
		public event ValueChangedEventHandler LinesCountChanged;
        /// <summary>
        /// Event, that is raised when lines has been inserted.
        /// </summary>
        public event LineInsertedEventHandler LineInserted;
        /// <summary>
        /// Event, that is raised when line has been deleted.
        /// </summary>
        public event LineDeletedEventHandler LineDeleted;
		/// <summary>
		/// Event, that is raised when text was inserted;
		/// </summary>
		public event TextChangedEventHandler TextInserted;
		/// <summary>
		/// Event that is raised when text was deleted;
		/// </summary>
		public event TextChangedEventHandler TextDeleted;
		/// <summary>
		/// Event, that is raised when text is to be inserted;
		/// </summary>
		public event TextChangingEventHandler TextInserting;
		/// <summary>
		/// Event that is raised when text is to be deleted;
		/// </summary>
		public event TextChangingEventHandler TextDeleting;
		/// <summary>
		/// Event, raised on the start of the long operation.
		/// </summary>
		public event LongOperationEventHandler OperationStarted;
		/// <summary>
		/// Event, raised on the end of the long operation.
		/// </summary>
		public event LongOperationEventHandler OperationStopped;
		/// <summary>
		/// Event that is raised before region is about to expand.
		/// </summary>
		public event OutliningCancellableEventHandler OutliningBeforeExpand;
		/// <summary>
		/// Event that is raised when region expands.
		/// </summary>
		public event OutliningEventHandler OutliningExpand;
		/// <summary>
		/// Event that is raised before region is about to collapse.
		/// </summary>
		public event OutliningCancellableEventHandler OutliningBeforeCollapse;
		/// <summary>
		/// Event that is raised when region collapses.
		/// </summary>
		public event OutliningEventHandler OutliningCollapse;
		/// <summary>
		/// Event that is raised wneh the state of some outlining region changes.
		/// </summary>
		internal event EventHandler OutliningStateChanged;
		/// <summary>
		/// Event that is raised when index of some line 
		/// </summary>
		public event EventHandler LineIndexChanged;
		#endregion

		#region Initialization And Finalization
		/// <summary>
		/// Creates new instance of the parser and initializes it.
		/// </summary>
		/// <param name="source">Input source.</param>
		/// <param name="language">Language configuration.</param>
		public LexemParser( StreamsWrapper source, IConfigLanguage language )
			: this( source, language, new UndoRedoData() )
		{
		}
		/// <summary>
		/// Creates new instance of the parser and initializes it.
		/// </summary>
		/// <param name="source">Input source.</param>
		/// <param name="language">Language configuration.</param>
		/// <param name="undoData">Undo-Redo data storage to be used.</param>
		internal LexemParser( StreamsWrapper source, IConfigLanguage language, UndoRedoData undoData )
		{
			if( source == null ) throw new ArgumentNullException( "source" );
			if( language == null ) throw new ArgumentNullException( "language" );

			m_streamsWrapper = source;
			m_language = language;

			source.CaseSensitive = !language.CaseInsensitive;

			Split[] strs = new Split[ language.Splits.Count ];
			language.Splits.CopyTo( strs );

			m_streamsWrapper.MultiCharTokens = strs;

			m_curLineIndex = ( m_streamsWrapper.LinesCount > 0 ) ? ( 1 ) : ( 0 );
			m_language.Lexems.Sort();

			source.LinesCountChanged += new ValueChangedEventHandler( OnSourceLinesCountChanged );
            source.LineInserted += new LineInsertedEventHandler(source_LineInserted);
            source.LineDeleted += new LineDeletedEventHandler(source_LineDeleted);
			source.UndoBufferFlushed += new EventHandler( OnUndoBufferFlush );
			source.RedoBufferFlushed += new EventHandler( OnRedoBufferFlush );
#if DEBUG
			source.BeforeTextChange += new EventHandler( OnSourceBeforeTextChange );
			source.AfterTextChange += new EventHandler( OnSourceAfterTextChange );
#endif

			m_undoredoData = undoData;
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Deletes LexemLine from internal collection.
		/// </summary>
		/// <param name="line">Line to be deleted.</param>
		public void DeleteLine( ILexemLine line )
		{
			if( line == null ) throw new ArgumentNullException( "line" );

			int index = m_linesList.IndexOf( line );

			if( index >= 0 )
			{
				m_linesList.RemoveAt( index );
				( line as IDisposable ).Dispose();

				RaiseLineInstanceDeletedEvent( line );
			}
		}
		/// <summary>
		/// Looks for line in cache.
		/// </summary>
		/// <param name="iLine">Index of the line.</param>
		/// <returns>ILexemLine interface to the line, or null if nothing was found.</returns>
		public ILexemLine FindLineInCache( int iLine )
		{
			ILexemLine result = null;

			if( iLine >= 1 && iLine <= m_streamsWrapper.LinesCount )
			{
				int index = m_linesList.BinarySearch( iLine );
				if( index >= 0 )
				{
					result = m_linesList[ index ] as ILexemLine;
				}
			}

			return result;
		}
        internal int parsingIndex = 0;
		/// <summary>
		/// Gets line by specified line index. If line is in cache, then instance from cache will be returned.
		/// Otherwise it will be created and added to the cache.
		/// </summary>
		/// <param name="iLine">Line index.</param>
		/// <returns>ILexemLine object.</returns>
		public ILexemLine GetLine( int iLine )
		{
			if( iLine < 1 || iLine > TotalLines ) throw new ArgumentOutOfRangeException(
				"iLine", iLine, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_166 );

			ILexemLine result;

			int index = m_linesList.BinarySearch( iLine );

			if( index >= 0 )
			{
				result = ( m_linesList[ index ] as ILexemLine );
			}
			else
			{
				index = ~index - 1;

				// The last cached line before needed one.
				ILexemLine firstLine;

				// if there is no lines found in cache that are before needed one.
				if( index < 0 )
				{
					firstLine = CreateLine( m_streamsWrapper.GetParsePoint( 1, 1, true ), CreateDefaultStack() );
					InsertLexemLineIntoList( firstLine, 0 );
					index = 0;
				}
				else
				{
					firstLine = m_linesList[ index ] as ILexemLine;
				}

				ILongOperation operation = null;
				bool bParsingSpeedUp = ( m_parsingMode != TextParsingMode.FullParsing );

				if( iLine - firstLine.LineIndex > 3 )
				{
					operation = StartOperation( "Parsing" );
				}

				while( firstLine.LineIndex < iLine )
				{
					index++;

					if( bParsingSpeedUp && ( iLine - firstLine.LineIndex > DEF_LINES_BEFORE_SPEEDUP ) )
					{
						int iLineVirtual = iLine - DEF_LINES_BEFORE_SPEEDUP;
						int iLinePhisical = firstLine.LineStartPoint.Line + ( iLineVirtual - firstLine.LineIndex );
						firstLine = CreatePlainTextLine( iLineVirtual, iLinePhisical, index );
                        parsingIndex = iLineVirtual;
						continue;
					}

					firstLine = GetNextLine( firstLine );
					InsertLexemLineIntoList( firstLine, index );
				}

				if( operation != null )
				{
					operation.Stop();
				}

				result = firstLine;
			}

			return result;
		}
		/// <summary>
		/// Moves to the next line and parses it.
		/// </summary>
		/// <returns>Array of lexems that represent line.</returns>
		public IList NextLine()
		{
			CurrentLine++;
			return GetLine();
		}
		/// <summary>
		/// Moves to the previous line and parses it.
		/// </summary>
		/// <returns>Array of lexems that represent line.</returns>
		public IList PreviousLine()
		{
			CurrentLine--;
			return GetLine();
		}
		/// <summary>
		/// Parses current line.
		/// </summary>
		/// <returns>Array of lexems that represent line.</returns>
		public IList GetLine()
		{
			return GetLine( m_curLineIndex ).LineLexems;
		}
		/// <summary>
		/// Gets copy of the current parser's stack.
		/// </summary>
		/// <returns>Stack object.</returns>
		public ConfigStack GetStackCopy()
		{
			if( m_lastStack == null )
			{
				m_lastStack = CreateDefaultStack();
			}
			return ( ConfigStack )m_lastStack.Clone();
		}
		/// <summary>
		/// Creates default stack, filled with language configuration
		/// </summary>
		/// <returns>Stack object.</returns>
		public ConfigStack CreateDefaultStack()
		{
			ConfigStack result = new ConfigStack();
			result.Push( ( IConfigLexem )m_language, null, null );
			return result;
		}
		/// <summary>
		/// Creates fake configuration stack that can be used for plain text coloring.
		/// </summary>
		/// <returns>ConfigStack instance filled with language and configuration that parses all text as plain text.</returns>
		public ConfigStack CreatePlainTextStack()
		{
			if( m_stackPlainText == null )
			{
				ConfigStack result = new ConfigStack();
				result.Push( new ConfigLexem_Lexem_Pair( ( IConfigLexem )m_language, null, null ) );

				ConfigLexem configRoot = new ConfigLexem( DEF_CONFIG_NAME_PLAIN_TEXT, string.Empty, FormatType.Text, true );
				ConfigLexem configWordProcessor = new ConfigLexem( ".+", string.Empty, FormatType.Text, false );
				configWordProcessor.IsBeginRegex = true;
				configRoot.OnlyLocalSublexems = true;
				configRoot.SubLexems.Add( configWordProcessor );
				configRoot.ParentConfig = ( IConfigLexem )m_language;
				configRoot.UpdateSublexems();

				result.Push( new ConfigLexem_Lexem_Pair( configRoot, null, null ) );

				m_stackPlainText = result;
			}

			return ( ConfigStack )m_stackPlainText.Clone();
		}
		/// <summary>
		/// Sets new stack.
		/// </summary>
		/// <param name="stack">Stack to be set.</param>
		public void SetStack( ConfigStack stack )
		{
			if( stack == null ) throw new ArgumentNullException( "stack" );

			m_lastStack = ( ConfigStack )stack.Clone();
		}
		/// <summary>
		/// Creates enumerator of lexems
		/// </summary>
		/// <returns>IEnumerator.</returns>
		public IEnumerator GetEnumerator()
		{
			LexemParserEnumerator enumerator = new LexemParserEnumerator( GetStackCopy(), this );
			return enumerator;
		}
		/// <summary>
		/// Creates enumerator of lexems.
		/// </summary>
		/// <param name="stack">Stack for the current position.</param>>
		/// <returns>IEnumerator.</returns>
		public IEnumerator GetEnumerator( ConfigStack stack )
		{
			if( stack == null ) throw new ArgumentNullException( "stack" );

			SetStack( stack );
			return GetEnumerator();
		}
		/// <summary>
		/// Creates enumerator of lexems.
		/// </summary>
		/// <param name="stack">Stack for the current position.</param>>
		/// <param name="point">New current position.</param>
		/// <returns>IEnumerator.</returns>
		public IEnumerator GetEnumerator( ConfigStack stack, IParsePoint point )
		{
			if( stack == null ) throw new ArgumentNullException( "stack" );
			if( point == null ) throw new ArgumentNullException( "point" );

			BaseStream.SetPositionToParsePoint( point );
			return GetEnumerator( stack );
		}
		/// <summary>
		/// Creates enumerator of the lexem lines.
		/// </summary>
		/// <param name="line">Starting line.</param>
		/// <returns>Enumerator.</returns>
		public IEnumerator GetLineEnumerator( ILexemLine line )
		{
			if( line == null ) throw new ArgumentNullException( "line" );

			int index = m_linesList.BinarySearch( line.LineIndex );

			if( index < 0 ) throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_167, "line" );

			return m_linesList.GetEnumerator( index, m_linesList.Count - index );
		}
		/// <summary>
		/// Creates enumerator of the lexem lines.
		/// </summary>
		/// <returns>Enumerator.</returns>
		public IEnumerator GetLineEnumerator()
		{
			return m_linesList.GetEnumerator();
		}
		/// <summary>
		/// Undoes last change.
		/// </summary>
		/// <returns>Point of undo operation.</returns>
		public Point Undo()
		{
			Point result = Point.Empty;
			if( CanUndo )
			{
				WrapperUndoItem undoItem = BaseStream.GetFirstUndoItem();

				bool bOK = true;

				IParsePoint parsep = BaseStream.GetParsePoint( undoItem.ChangeContext.Position );
				EnsureVisibility( parsep );
				CoordinatePoint coordp = GetCoordinatePoint( parsep );

				if( undoItem.ChangeContext.Type == ChangeType.Delete )
				{
					if( !RaiseTextInsertingEvent( ( string )undoItem.AdditionalData, coordp.VirtualLine, coordp.VirtualColumn ) )
					{
						bOK = false;
					}
				}
				else
				{
					string text = BaseStream.Encoding.GetString( undoItem.ChangeContext.Data );

					if( !RaiseTextDeletingEvent( text, coordp.VirtualLine, coordp.VirtualColumn ) )
					{
						bOK = false;
					}
				}

				if( bOK )
				{
					UndoItem undo = ( UndoItem )m_undoredoData.UndoStack.Pop();
					m_undoredoData.RedoStack.Push( undo );

					IParsePoint pointStart = BaseStream.GetParsePoint( undo.StartOffset );
					CoordinatePoint coordinateStart = GetCoordinatePoint( pointStart );
					ILexemLine startLine = GetLine( coordinateStart.VirtualLine );
					int oldStackCount = startLine.LineEndStack.Count;

					startLine.DeleteSelf();
					undoItem = BaseStream.Undo();
					startLine = GetLine( coordinateStart.VirtualLine );

					if( startLine.LineEndStack.Count != oldStackCount )
					{
						ResetLines( startLine );
					}
					if( pointStart.IsValid )
					{
						coordinateStart = GetCoordinatePoint( pointStart );
					}

					IParsePoint pp = BaseStream.GetParsePoint( undoItem.ChangeContext.Position );
					EnsureVisibility( pp );
					CoordinatePoint cp = GetCoordinatePoint( pp );

					if( undoItem.ChangeContext.Type == ChangeType.Delete )
					{
						RaiseTextInsertedEvent( ( string )undoItem.AdditionalData, cp.VirtualLine, cp.VirtualColumn );
					}
					else
					{
						string text = BaseStream.Encoding.GetString( undoItem.ChangeContext.Data );

						RaiseTextDeletedEvent( text, cp.VirtualLine, cp.VirtualColumn );
					}

					result = new Point( coordinateStart.VirtualColumn, coordinateStart.VirtualLine );
				}
			}

			return result;
		}
		/// <summary>
		/// Undoes last change.
		/// </summary>
		/// <returns>Point of redo operation.</returns>
		public Point Redo()
		{
			return Redo( true );
		}
		/// <summary>
		/// Undoes last change.
		/// </summary>
		/// <param name="bRaiseTextChanging">Indicates whether text changing events should be raised.</param>
		/// <returns>Point of redo operation.</returns>
		public Point Redo( bool bRaiseTextChanging )
		{
			Point result = Point.Empty;
			if( CanRedo )
			{
				WrapperUndoItem redoItem = BaseStream.GetFirstRedoItem();

				bool bOK = true;
				if( bRaiseTextChanging )
				{
					IParsePoint parsep = BaseStream.GetParsePoint( redoItem.ChangeContext.Position );
					EnsureVisibility( parsep );
					CoordinatePoint coordp = GetCoordinatePoint( parsep );

					if( redoItem.ChangeContext.Type == ChangeType.Delete )
					{
						if( !RaiseTextDeletingEvent( ( string )redoItem.AdditionalData, coordp.VirtualLine, coordp.VirtualColumn ) )
						{
							bOK = false;
						}
					}
					else
					{
						string text = BaseStream.Encoding.GetString( redoItem.ChangeContext.Data );

						if( !RaiseTextInsertingEvent( text, coordp.VirtualLine, coordp.VirtualColumn ) )
						{
							bOK = false;
						}
					}
				}

				if( bOK )
				{
					UndoItem undo = ( UndoItem )m_undoredoData.RedoStack.Pop();
					m_undoredoData.UndoStack.Push( undo );

					IParsePoint pointStart = BaseStream.GetParsePoint( undo.StartOffset );
					CoordinatePoint coordinateStart = GetCoordinatePoint( pointStart );
					ILexemLine startLine = GetLine( coordinateStart.VirtualLine );
					int oldStackCount = startLine.LineEndStack.Count;

					startLine.DeleteSelf();
					BaseStream.Redo();
					startLine = GetLine( coordinateStart.VirtualLine );

					if( startLine.LineEndStack.Count != oldStackCount )
					{
						ResetLines( startLine );
					}
					if( pointStart.IsValid )
					{
						coordinateStart = GetCoordinatePoint( pointStart );
					}

					if( bRaiseTextChanging )
					{
						IParsePoint parsep = BaseStream.GetParsePoint( redoItem.ChangeContext.Position );
						EnsureVisibility( parsep );
						CoordinatePoint coordp = GetCoordinatePoint( parsep );

						if( redoItem.ChangeContext.Type == ChangeType.Delete )
						{
							RaiseTextDeletedEvent( ( string )redoItem.AdditionalData, coordp.VirtualLine, coordp.VirtualColumn );
						}
						else
						{
							string text = BaseStream.Encoding.GetString( redoItem.ChangeContext.Data );

							RaiseTextInsertedEvent( text, coordp.VirtualLine, coordp.VirtualColumn );
						}
					}

					result = new Point( coordinateStart.VirtualColumn, coordinateStart.VirtualLine );
				}
			}

			return result;
		}
		/// <summary>
		/// Inserts text into position.
		/// </summary>
		/// <param name="point">Point, text should be inserted to.</param>
		/// <param name="str">Text to be inserted.</param>
		public virtual void InsertText( string str, CoordinatePoint point )
		{
			if( str == null ) throw new ArgumentNullException( "str" );
			if( str == string.Empty )
				throw new ArgumentOutOfRangeException( "str", str, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_10 );
			if( point == null || !point.IsValid ) throw new ArgumentNullException( "point" );

			ILexemLine line = GetLine( point.VirtualLine );
			if( line != null )
			{
				int iLineLength = line.LineLength + 1;
				int iColumn = point.VirtualColumn;
				int iLine = point.VirtualLine;

				// Virtual space check.
				if( iLineLength < iColumn ) throw new Exception( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_168 );

				IParsePoint ppoint = GetParsePoint( iLine, iColumn );
				if( ppoint != null && RaiseTextInsertingEvent( str, point.VirtualLine, point.VirtualColumn ) )
				{
					str = BaseStream.ConvertNewLines( str );
					object oldStack = line.LineEndStack.Clone();
					EnsureVisibility( ppoint );

					line.Parsed = false;
					UndoItem undo = new UndoItem();
					undo.StartOffset = ppoint.Offset;
					undo.EndOffset = BaseStream.InsertText( ppoint, str ) + undo.StartOffset;
					m_undoredoData.UndoStack.Push( undo );

					if( !line.IsValid )
					{
						line = GetLine( iLine );
					}
					if( !line.LineEndStack.Equals( oldStack ) && line.LineStartPoint.Line < BaseStream.LinesCount )
					{
						ResetLines( line );
					}

					RaiseTextInsertedEvent( str, point.VirtualLine, point.VirtualColumn );
				}
			}
		}
		/// <summary>
		/// Deletes given range of text.
		/// </summary>
		/// <param name="pointEnd">Start point of text to delete.</param>
		/// <param name="pointStart">End point of text to delete.</param>
		public virtual void DeleteText( CoordinatePoint pointStart, CoordinatePoint pointEnd )
		{
			if( pointStart == null || !pointStart.IsValid ) throw new ArgumentNullException( "pointStart" );
			if( pointEnd == null || !pointEnd.IsValid ) throw new ArgumentNullException( "pointEnd" );
			if( pointStart >= pointEnd )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_169 );

			ILexemLine line = GetLine( pointStart.VirtualLine );
			object oldStack = line.LineEndStack.Clone();
			EnsureVisibility( pointStart.PhysicalPoint );
			EnsureVisibility( pointEnd.PhysicalPoint );

			UndoItem undo = new UndoItem();
			undo.StartOffset = pointStart.PhysicalPoint.Offset;
			undo.EndOffset = pointEnd.PhysicalPoint.Offset;

			line.DeleteSelf();
			string deletedText = BaseStream.GetTextInRange( pointStart.PhysicalPoint, pointEnd.PhysicalPoint, true );

			if( RaiseTextDeletingEvent( deletedText, pointStart.VirtualLine, pointStart.VirtualColumn ) )
			{
                m_undoredoData.UndoStack.Push(undo);
				// Mark first or last line changed.
				bool bFirstLineChanged = ( pointStart.PhysicalPoint.Offset == 0 );
				bool bLastLineChanged = ( pointEnd.PhysicalPoint.Offset == this.BaseStream.Length );

				BaseStream.DeleteText( pointStart.PhysicalPoint, pointEnd.PhysicalPoint );
				line = GetLine( pointStart.VirtualLine );
				IStackData stackDatum = ( IStackData )line.LineStartStack.Peek();
				bool bBadStack = ( stackDatum != null && stackDatum.Location != null && !stackDatum.Location.IsValid );

				if( !line.LineEndStack.Equals( oldStack ) && line.LineStartPoint.Line < BaseStream.LinesCount || bBadStack )
				{
					using( StartOperation( "Resetting" ) )
					{
						ResetLines( line );
						line = GetLine( pointStart.VirtualLine );
					}
				}

				if( bFirstLineChanged )
				{
					( ( RenderedLine )GetLine( 1 ) ).ForcedChanged = true;
				}

				if( bLastLineChanged )
				{
					( ( RenderedLine )GetLine( this.LinesList.Count ) ).ForcedChanged = true;
				}

				using( StartOperation( "Deleting" ) )
				{
					RaiseTextDeletedEvent( deletedText, pointStart.VirtualLine, pointStart.VirtualColumn );
				}
			}
		}
		/// <summary>
		/// Retrieves last line in the internal list (not the same as last line of text).
		/// </summary>
		/// <returns>Lexem line.</returns>
		public virtual ILexemLine GetLastRegisteredLine()
		{
			ILexemLine result = null;

			if( m_linesList.Count != 0 )
			{
				result = m_linesList[ m_linesList.Count - 1 ] as ILexemLine;
			}

			return result;
		}
		/// <summary>
		/// Searches for collapsible region by offset of the given parsepoint. Note: new region is not created.
		/// </summary>
		/// <param name="point">ParsePoint, that specifies position in stream, region is needed for.</param>
		/// <returns>Found region, or null if there is no such region.</returns>
		public virtual CollapsableRegion GetCollapsableRegion( IParsePoint point )
		{
			if( point == null ) throw new ArgumentNullException( "point" );

			CollapsableRegion result = null;

			int regionIndex = m_collapseList.BinarySearch( point.Offset );
			if( regionIndex >= 0 )
			{
				result = m_collapseList[ regionIndex ] as CollapsableRegion;
			}

			return result;
		}
		/// <summary>
		/// Gets ParsePoint of the given coordinates.
		/// </summary>
		/// <remarks>
		/// The main difference between this method, and those, presented by StreamsWrapper is that this method also checks collapsing.
		/// </remarks>
		/// <param name="iLine">Needed line.</param>
		/// <param name="iColumn">Needed column.</param>
		/// <returns>Statical <see cref="IParsePoint"/> that points to specified coordinates.</returns>
		public IParsePoint GetParsePoint( int iLine, int iColumn )
		{
			IParsePoint result = null;

			ILexemLine line = GetLine( iLine );
			if( line != null )
			{
				int iCurrentColumn = 1;
				int iPhysicalLine = line.LineStartPoint.Line;
				int iPhysicalColumn = 1;
				IList lexems = line.LineLexems;

				bool bFound = false;
				foreach( ILexem lex in lexems )
				{
					int iLength = lex.Length;
					bool bIsCollapsed = ( lex.Collapser != null && lex.Collapser.Collapsed );

					if( iColumn >= iCurrentColumn && iColumn < iCurrentColumn + iLength )
					{
						if( !bIsCollapsed )
						{
							iPhysicalColumn += iColumn - iCurrentColumn;
						}

						result = BaseStream.GetParsePoint( iPhysicalLine, iPhysicalColumn, true );
						bFound = true;
						break;
					}

					if( bIsCollapsed )
					{
						int iLineDifference = lex.Collapser.End.Line - lex.Collapser.Start.Line;
						bool bNewLineEnd = ( lex.Collapser.EndLexem != null && lex.Collapser.EndLexem.Text == BaseStream.NewLineStr );
						if( bNewLineEnd )
						{
							iLineDifference++;
						}

						iPhysicalLine += iLineDifference;
						if( bNewLineEnd )
						{
							iPhysicalColumn = 1;
						}
						else
						{
							int endLexLen = ( lex.Collapser.EndLexem != null ) ? ( lex.Collapser.EndLexem.Length ) : ( 0 );

							if( iLineDifference == 0 )
							{
								iPhysicalColumn += lex.Collapser.End.Position + endLexLen - lex.Collapser.Start.Position;
							}
							else
							{
								iPhysicalColumn = lex.Collapser.End.Position + endLexLen;
							}
						}
					}
					else
					{
						iPhysicalColumn += iLength;
					}

					iCurrentColumn += iLength;
				}

				if( !bFound && iCurrentColumn == iColumn )
				{
					result = BaseStream.GetParsePoint( iPhysicalLine, iPhysicalColumn, true );
				}
			}

			return result;
		}
		/// <summary>
		/// Gets list of top-level collapsed regions.
		/// </summary>
		/// <returns>List of collapsed regions.</returns>
		public IList GetCollapsedRegionsList()
		{
			IList collapses = new ArrayList();

			if( m_bCollapsingEnabled )
			{
				for( int i = 0; i < m_collapseList.Count; i++ )
				{
					CollapsableRegion region = m_collapseList[ i ] as CollapsableRegion;
					CollapsableRegion lastRegion = ( collapses.Count > 0 ) ? ( collapses[ collapses.Count - 1 ] as CollapsableRegion ) : ( null );

					if( region.Collapsed && region.End != null )
					{
						if( lastRegion == null || lastRegion.End.Line < region.Start.Line ||
							( ( lastRegion.End.Line == region.Start.Line ) && ( lastRegion.End.Position + lastRegion.EndLexem.Length <= region.Start.Position ) ) )
						{
							collapses.Add( region );
						}
					}
				}
			}

			return collapses;
		}
		/// <summary>
		/// Updates virtual line indexes according to current collapsing.
		/// </summary>
		public void UpdateLineInformation()
		{
			int iCompression = 0;
			int iLastRegion = 0;

			IList collapses = GetCollapsedRegionsList();
			IList toDelete = new ArrayList();

			LockConsistenceChecks();

			for( int i = 0; i < m_linesList.Count; i++ )
			{
				UncachedLexemLine line = m_linesList[ i ] as UncachedLexemLine;

				while( iLastRegion < collapses.Count )
				{
					CollapsableRegion region = collapses[ iLastRegion ] as CollapsableRegion;

					if( region.Start.Line >= line.LineStartPoint.Line )
					{
						break;
					}

					if( region.End.Line < line.LineStartPoint.Line )
					{
						iCompression += region.End.Line - region.Start.Line;
						iLastRegion++;
					}
					else
					{
						break;
					}
				}


				if( iLastRegion < collapses.Count )
				{
					CollapsableRegion currentRegion = collapses[ iLastRegion ] as CollapsableRegion;

					if( currentRegion.Start.Line < line.LineStartPoint.Line )
					{
						toDelete.Add( line );
					}
				}

				line.LineIndex = line.LineStartPoint.Line - iCompression;
				line.Parsed = false;
			}

			foreach( ILexemLine lineToDelete in toDelete )
			{
				lineToDelete.DeleteSelf();
			}

			UnlockConsistenceChecks();

			if( !IsCollapsingLocked )
			{
				RaiseLinesCountChangedEvent( -1 );
			}
		}
		/// <summary>
		/// Gets <see cref="CoordinatePoint"/> class instance, that represents some coordinates in stream.
		/// </summary>
		/// <param name="iLine">Virtual line.</param>
		/// <param name="iColumn">Virtual column.</param>
		/// <returns><see cref="CoordinatePoint"/> class instance.</returns>
		public CoordinatePoint GetCoordinatePoint( int iLine, int iColumn )
		{
			return GetCoordinatePoint( iLine, iColumn, false );
		}
		/// <summary>
		/// Gets <see cref="CoordinatePoint"/> class instance, that represents some coordinates in stream.
		/// </summary>
		/// <param name="iLine">Virtual line.</param>
		/// <param name="iColumn">Virtual column.</param>
		/// <param name="bTrackPosition">Specifies whether coordinate point should track position.</param>
		/// <returns><see cref="CoordinatePoint"/> class instance.</returns>
		public CoordinatePoint GetCoordinatePoint( int iLine, int iColumn, bool bTrackPosition )
		{
			IParsePoint point = GetParsePoint( iLine, iColumn );

			if( point == null ) return null;

			CoordinatePoint result = new CoordinatePoint( this, point, iLine, iColumn, bTrackPosition );

			return result;
		}
		/// <summary>
		/// Gets <see cref="CoordinatePoint"/> by given <see cref="IParsePoint"/>.
		/// </summary>
		/// <param name="point"><see cref="IParsePoint"/> that points to physical position in stream.</param>
		/// <returns><see cref="CoordinatePoint"/> that points to the given position.</returns>
		public CoordinatePoint GetCoordinatePoint( IParsePoint point )
		{
			return GetCoordinatePoint( point, false );
		}
		/// <summary>
		/// Gets <see cref="CoordinatePoint"/> by given <see cref="IParsePoint"/>.
		/// </summary>
		/// <param name="point"><see cref="IParsePoint"/> that points to physical position in stream.</param>
		/// <param name="bRedirectToStart">Indicates whether unavailable point should be redirected to the start of collapsed region.</param>
		/// <returns><see cref="CoordinatePoint"/> that points to the given position.</returns>
		public CoordinatePoint GetCoordinatePoint( IParsePoint point, bool bRedirectToStart )
		{
			return GetCoordinatePoint( point, bRedirectToStart, false );
		}
		/// <summary>
		/// Gets <see cref="CoordinatePoint"/> by given <see cref="IParsePoint"/>.
		/// </summary>
		/// <param name="point"><see cref="IParsePoint"/> that points to physical position in stream.</param>
		/// <param name="bRedirectToStart">Indicates whether unavailable point should be redirected to the start of collapsed region.</param>
		/// <param name="bTrackPosition">Specifies whether coordinate point should track position.</param>
		/// <returns><see cref="CoordinatePoint"/> that points to the given position.</returns>
		public CoordinatePoint GetCoordinatePoint( IParsePoint point, bool bRedirectToStart, bool bTrackPosition )
		{
			if( point == null ) throw new ArgumentNullException( "point" );

			ArrayList list = new ArrayList( GetCollapsedRegionsList() );
			int index = list.BinarySearch( point.Offset );

			if( index < 0 )
				index = ~index;

			index = Math.Min( index + 1, list.Count );

			int iLineCompression = 0;
			int iLastLine = -1;
			int iLineLength = 0;

			for( int i = 0; i < index; i++ )
			{
				CollapsableRegion region = list[ i ] as CollapsableRegion;

				if( region.Contains( point ) )
				{
					point = region.Start;
					break;
				}

				if( region.Start.Offset > point.Offset )
					break;

				int iStartLine = region.Start.Line - iLineCompression;
				iLineCompression += region.End.Line - region.Start.Line;

				if( iLastLine != iStartLine )
				{
					iLastLine = iStartLine;
					iLineLength = 0;
				}

				iLineLength += region.Start.Position - 1;
				iLineLength -= region.End.Position - 1;

				iLineLength += region.CollapseName.Length - 1;

				if( region.End.Offset < m_streamsWrapper.Length && null != region.EndLexem )
				{
					if( region.EndLexem.Text == BaseStream.NewLineStr )
					{
						iLineLength = 0;
						iLineCompression++;
					}
					else
					{
						iLineLength -= region.EndLexem.Length - 1;
					}
				}
				else
				{
					iLineLength += region.Lexem.Length;
				}
			}

			int newLine = point.Line - iLineCompression;

            //SD 3924 fixed
            if (newLine == 0)
            {
                newLine = 1;
            }
            //End

			int newColumn = point.Position +
				( ( iLastLine == newLine ) ? iLineLength : 0 );

			CoordinatePoint newPoint = new CoordinatePoint( this, point, newLine, newColumn, bTrackPosition );

			return newPoint;
		}
		/// <summary>
		/// Searches for the <see cref="Syncfusion.Windows.Forms.Edit.Interfaces.IParsePoint"/> at the given position.
		/// </summary>
		/// <remarks>
		/// If it can not be found (it is in virtual space), then you will get parse point, pointing to the beginning of the next line.
		/// If it can not be done, ParsePoint, pointing to the end of current line will be returned.
		/// </remarks>
		/// <param name="iLine">Line index, the ParsePoint is needed for.</param>
		/// <param name="iColumn">Column index, the ParsePoint is needed for. Can be 0.</param>
		/// <returns>ParsePoint to the given position.</returns>
		public CoordinatePoint GetNearestParsePointRight( int iLine, int iColumn )
		{
			iLine = Math.Min( iLine, TotalLines );

			if( iLine < 1 ) throw new ArgumentOutOfRangeException(
				"iLine", iLine, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_170 );

			CoordinatePoint point = ( iColumn > 0 ) ? GetCoordinatePoint( iLine, iColumn ) : null;

			if( point != null )
				return point;

			if( iColumn == 0 )
				return GetCoordinatePoint( iLine, 1 );

			// If we are here, then we are defenetly beyong the end of the line.
			// If we are not at the last line of text, then go to the beginning
			// of the next line, otherwise - to the end of current.
			if( iLine != TotalLines )
				return GetCoordinatePoint( iLine + 1, 1 );
			else
			{
				int iLineLength = GetLine( iLine ).LineLength + 1;
				return GetCoordinatePoint( iLine, iLineLength );
			}
		}
		/// <summary>
		/// Searches for the <see cref="Syncfusion.Windows.Forms.Edit.Interfaces.IParsePoint"/> at the given position.
		/// </summary>
		/// <remarks>
		/// If it can not be found ( or column is 0), and if it is in virtual space, then you will get parse point to the end of given line;
		/// If column is 0, then you will get parse point to the end of the previous line( if it is one ).
		/// </remarks>
		/// <param name="iLine">Line index the ParsePoint is needed for.</param>
		/// <param name="iColumn">Column index the ParsePoint is needed for. Can be 0.</param>
		/// <returns>ParsePoint to the given position.</returns>
		public CoordinatePoint GetNearestParsePointLeft( int iLine, int iColumn )
		{
			if( iLine < 1 ) throw new ArgumentOutOfRangeException(
				"iLine", iLine, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_170 );

			if( iLine > TotalLines )
			{
				ILexemLine line = GetLine( TotalLines );
				iColumn = line.LineLength + 1;
				iLine = TotalLines;
			}

			CoordinatePoint point = ( iColumn > 0 ) ? GetCoordinatePoint( iLine, iColumn ) : null;

			if( point != null )
				return point;

			bool bNotFirstLine = iLine > 1;
			iLine -= ( bNotFirstLine && iColumn == 0 ) ? 1 : 0;
			int iLineLength = ( iColumn != 0 || bNotFirstLine ) ? GetLine( iLine ).LineLength + 1 : 1;

			CoordinatePoint pointResult = GetCoordinatePoint( iLine, iLineLength );

            //if( pointResult == null)
            //    throw new NullReferenceException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_171 );
            
			return pointResult;
		}
		/// <summary>
		/// Gets collapsible region, point belongs to.
		/// </summary>
		/// <param name="point">Point, the region is to be looked for.</param>
		/// <param name="bCollapsed">Specifies, whether region must be collapsed.</param>
		/// <returns>Found region, or null.</returns>
		public CollapsableRegion GetOuterCollapsableRegion( IParsePoint point, bool bCollapsed )
		{
			if( point == null ) throw new ArgumentNullException( "point" );

			return GetOuterCollapsableRegion( point.Offset, bCollapsed );
		}
		/// <summary>
		/// Gets collapsible region, point belongs to.
		/// </summary>
		/// <param name="offset">Offset of point, the region is to be looked for.</param>
		/// <param name="bCollapsed">Specifies, whether region must be collapsed.</param>
		/// <returns>Found region, or null.</returns>
		public CollapsableRegion GetOuterCollapsableRegion( long offset, bool bCollapsed )
		{
			ArrayList list = ( ( bCollapsed ) ? ( GetCollapsedRegionsList() ) : ( m_collapseList ) ) as ArrayList;
			int regionIndex = list.BinarySearch( offset );

			if( regionIndex < 0 )
				regionIndex = ~regionIndex - 1;

			while( regionIndex >= 0 )
			{
				CollapsableRegion region = list[ regionIndex ] as CollapsableRegion;

				if( region.End == null )
				{
					ParseCollapsableRegion( region, ( ConfigStack )region.StartStack.Clone() );
				}

				if( region.End != null && region.End.Offset > offset ) return region;

				if( bCollapsed )
					break;

				regionIndex--;
			}

			return null;
		}
		/// <summary>
		/// Expands all collapsible regions inside the specified range.
		/// </summary>
		/// <param name="pointStart">Start of the range.</param>
		/// <param name="pointEnd">End of the range.</param>
		public void EnsureVisibility( IParsePoint pointStart, IParsePoint pointEnd )
		{
			if( pointStart == null ) throw new ArgumentNullException( "pointStart" );
			if( pointEnd == null ) throw new ArgumentNullException( "pointEnd" );

			ArrayList list = ( ArrayList )GetCollapsedRegionsList();

			int regionIndexStart = list.BinarySearch( pointStart.Offset - 1 );
			int regionIndexEnd = list.BinarySearch( pointEnd.Offset + 1 );

			if( regionIndexStart < 0 )
				regionIndexStart = ~regionIndexStart;

			if( regionIndexEnd < 0 )
				regionIndexEnd = ~regionIndexEnd - 1;

			bool workdone = false;

			BeginUpdateRegions();

			for( int i = regionIndexStart; i <= regionIndexEnd; i++ )
			{
				if( i >= 0 && i < list.Count )
				{
					CollapsableRegion region = ( CollapsableRegion )list[ i ];
					region.Collapsed = false;
					workdone = true;
				}
			}

			list.Clear();

			EndUpdateRegions();

			// Maybe we have some inner collapsed regions.
			if( workdone )
				EnsureVisibility( pointStart, pointEnd );
		}
		/// <summary>
		/// Uncollapses all collapsed regions, the point is in.
		/// </summary>
		/// <param name="point">ParsePoint to ensure visibility of.</param>
		public void EnsureVisibility( IParsePoint point )
		{
			if( point == null ) throw new ArgumentNullException( "point" );

			bool workDone = false;
			workDone |= FindAndExpandRegion( point.Offset );
			workDone |= FindAndExpandRegion( point.Offset - 1 );
			workDone |= FindAndExpandRegion( point.Offset + 1 );

			if( workDone )
				EnsureVisibility( point );
		}
		/// <summary>
		/// Checks visibility of the point.
		/// </summary>
		/// <param name="point">Point to check.</param>
		/// <returns>True if point is visible.</returns>
		public bool IsPointVisible( IParsePoint point )
		{
			if( point == null ) throw new ArgumentNullException( "point" );

			CollapsableRegion region = GetOuterCollapsableRegion( point, true );
			return ( region == null );
		}
		/// <summary>
		/// Toggles all collapsing to specified state.
		/// </summary>
		/// <param name="value">State of all collapsible regions to be set.</param>
		public void SetAllCollapsings( bool value )
		{
			for( int i = m_linesList.Count - 1; i >= 0; i-- )
			{
				ILexemLine lineToDelete = m_linesList[ i ] as ILexemLine;
				lineToDelete.DeleteSelf();
			}

			BeginUpdateRegions();
			m_collapseList.Clear();
			m_collapsedLines = -1;

			m_bInitialCollapsing = value;

			int index = 1;
			m_collapsedLines = -1;

			while( index <= TotalLines )
			{
				ILexemLine line = GetLine( index );
				line.Parsed = true;
				m_collapsedLines = -1;
				index++;
			}

			m_bInitialCollapsing = false;
			EndUpdateRegions();
		}
		/// <summary>
		/// Starts updating regions.
		/// </summary>
		public void BeginUpdateRegions()
		{
			if( m_iCollapsingsLocks == 0 )
			{
				BeginUpdateRegionsInternal();
			}

			m_iCollapsingsLocks++;
		}
		/// <summary>
		/// Ends updating regions.
		/// </summary>
		public void EndUpdateRegions()
		{
			if( m_iCollapsingsLocks != 0 )
			{
				m_collapsedLines = -1;
				m_iCollapsingsLocks--;

				if( m_iCollapsingsLocks == 0 )
				{
					EndUpdateRegionsInternal();
				}
			}
		}
		/// <summary>
		/// Writes data to xml.
		/// </summary>
		/// <param name="parent">Parent xml element, data must be written to.</param>
		public void AppendToXML( XmlElement parent )
		{
			if( parent == null ) throw new ArgumentNullException( "parent" );

			SetAllCollapsings( false );

			XmlElement element = parent.OwnerDocument.CreateElement( "lines" );
			element.SetAttribute( "count", TotalLines.ToString() );

			for( int i = 1; i <= TotalLines; i++ )
			{
				IXMLDataProvider data = GetLine( i ) as IXMLDataProvider;
				data.AppendToXML( element );
			}

			parent.AppendChild( element );
		}
		/// <summary>
		/// Writes data to xml.
		/// </summary>
		/// <param name="writer">XML writer, data must be written to.</param>
		public void AppendToXML( XmlTextWriter writer )
		{
			if( writer == null ) throw new ArgumentNullException( "writer" );

			SetAllCollapsings( false );

			writer.WriteStartElement( "lines" );
			writer.WriteAttributeString( "count", TotalLines.ToString() );
			WriteLinesToXML( writer, 1, TotalLines );
			writer.WriteEndElement();
		}
		/// <summary>
		/// Writes to XML data situated between specified points.
		/// </summary>
		/// <param name="start">Point representing start of the text.</param>
		/// <param name="end">Point representing end of the text.</param>
		/// <param name="writer">XML writer, data must be written to.</param>
		public void AppendToXML( XmlTextWriter writer, CoordinatePoint start, CoordinatePoint end )
		{
			if( writer == null ) throw new ArgumentNullException( "writer" );
			if( null == start ) throw new ArgumentNullException( "start" );
			if( null == end ) throw new ArgumentNullException( "end" );

			int startLine = start.VirtualLine;
			int startCol = start.VirtualColumn;
			int endLine = end.VirtualLine;
			int endCol = end.VirtualColumn;

			if( ( startLine < 1 ) || ( startCol < 1 ) || ( startLine > TotalLines ) ) throw new ArgumentOutOfRangeException( "start" );
			if( ( endLine < 1 ) || ( endCol < 1 ) || ( endLine > TotalLines ) ) throw new ArgumentOutOfRangeException( "end" );

			SetAllCollapsings( false );

			writer.WriteStartElement( "lines" );
			writer.WriteAttributeString( "count", ( endLine - startLine + 1 ).ToString() );

			if( startLine == endLine )
			{
				( ( LexemLine )GetLine( startLine ) ).AppendMiddlePartToXML( writer, startCol, endCol );
			}
			else if( startLine < endLine )
			{
				( ( LexemLine )GetLine( startLine ) ).AppendEndPartToXML( writer, startCol );

				WriteLinesToXML( writer, startLine + 1, endLine - 1 );

				( ( LexemLine )GetLine( endLine ) ).AppendStartPartToXML( writer, endCol );
			}

			writer.WriteEndElement();
		}
		/// <summary>
		/// Writes specified text lines to XML.
		/// </summary>
		/// <param name="writer">XML writer, data must be written to.</param>
		/// <param name="start">Index of first line to write.</param>
		/// <param name="end">Index of last line to write.</param>
		public void WriteLinesToXML( XmlTextWriter writer, int start, int end )
		{
			if( writer == null ) throw new ArgumentNullException( "writer" );
			if( start < 1 || start > TotalLines ) throw new ArgumentOutOfRangeException( "start" );
			if( end < 0 || end > TotalLines ) throw new ArgumentNullException( "end" );

			for( int i = start; i <= end; i++ )
			{
				IXMLDataProvider data = GetLine( i ) as IXMLDataProvider;
				data.AppendToXML( writer );
			}
		}
		/// <summary>
		/// Starts new operation.
		/// </summary>
		/// <param name="name">Name of the operation.</param>
		/// <returns>Operation.</returns>
		public ILongOperation StartOperation( string name )
		{
			return new LongOperation( this, name ) as ILongOperation;
		}
		/// <summary>
		/// Returns number of columns used by lexem.
		/// </summary>
		/// <param name="lexem">Lexem object to get the length of.</param>
		/// <returns>Length of lexem.</returns>
		public int GetLexemLength( ILexem lexem )
		{
			return lexem.Length;
		}
		#endregion

		#region Nonpublic Methods
		/// <summary>
		/// Resets all lines starting from given one.
		/// </summary>
		/// <param name="startLine">First line that must be reset.</param>
		protected internal void ResetLines( ILexemLine startLine )
		{
			ResetLines( startLine, true );
		}
		/// <summary>
		/// Resets all lines starting from given one.
		/// </summary>
		/// <param name="delete">Specifies whether lines should be deleted or just their parsing information should be dropped.</param>
		/// <param name="startLine">First line that must be reset.</param>
		protected internal void ResetLines( ILexemLine startLine, bool delete )
		{
			if( startLine == null ) throw new ArgumentNullException( "startLine" );

			using( StartOperation( "Resetting lines" ) )
			{
				IEnumerator enumerator = GetLineEnumerator( startLine );
				IList toRemove = new ArrayList();

				while( enumerator.MoveNext() )
				{
					if( delete )
					{
						toRemove.Add( enumerator.Current );
					}
					else
					{
						ILexemLine line = ( ILexemLine )enumerator.Current;
						line.Parsed = false;
					}
				}

				foreach( ILexemLine lexLine in toRemove )
				{
					lexLine.DeleteSelf();
				}
			}
		}
		/// <summary>
		/// Raises LineIndexChanged event on line index changes. 
		/// </summary>
		/// <param name="line">Line that was changed.</param>
		protected internal void RaiseLineIndexChanged( UncachedLexemLine line )
		{
			if( line == null )
				throw new NullReferenceException( "line parameter turned to be null. But null's line index can not be changed!!!" );

			if( LineIndexChanged != null )
			{
				LineIndexChanged( line, EventArgs.Empty );
			}

#if DEBUG
			CheckConsistence();
#endif
		}
		/// <summary>
		/// Removes all event handlers from events.
		/// </summary>
		protected void ClearEventHandlers()
		{
			LineInstanceCreated = null;
			LineInstanceDeleted = null;
			LinesCountChanged = null;
            LineInserted = null;
            LineDeleted = null;
			TextInserted = null;
			TextDeleted = null;
			TextInserting = null;
			TextDeleting = null;
			OutliningBeforeExpand = null;
			OutliningExpand = null;
			OutliningBeforeCollapse = null;
			OutliningCollapse = null;
			OutliningStateChanged = null;
		}
		/// <summary>
		/// Locks consistence checks.
		/// </summary>
		protected void LockConsistenceChecks()
		{
			m_bConsistenceChecksLocked = true;
		}
		/// <summary>
		/// Unlocks consistence checks.
		/// </summary>
		protected void UnlockConsistenceChecks()
		{
			m_bConsistenceChecksLocked = true;
			CheckConsistence();
		}
		/// <summary>
		/// Checks line list integrity.
		/// </summary>
		[Conditional( "DEBUG" )]
		protected virtual void CheckConsistence()
		{
			if( !ConsistenceChecksLocked )
			{
				int index = -1;
				for( int i = 0; i < m_linesList.Count; i++ )
				{
					UncachedLexemLine line = ( UncachedLexemLine )m_linesList[ i ];

					if( index >= line.LineIndex ) throw new Exception( "Integrity check failure" );

					index = line.LineIndex;
				}
			}
		}
		/// <summary>
		/// Insert lexem line in the lines list.
		/// </summary>
		/// <param name="line">Line to be inserted.</param>
		/// <param name="index">Position the line is to be inserted to.</param>
		protected virtual void InsertLexemLineIntoList( ILexemLine line, int index )
		{
			ILexemLine linePrev = ( index > 0 && index <= m_linesList.Count ) ? ( ILexemLine )m_linesList[ index - 1 ] : null;
			ILexemLine lineNext = ( index >= 0 && index <= m_linesList.Count - 1 ) ? ( ILexemLine )m_linesList[ index ] : null;

			if( ( null != linePrev && linePrev.LineIndex >= line.LineIndex ) || ( null != lineNext && lineNext.LineIndex <= line.LineIndex ) )
				throw new ArgumentException( "Line has invalid index", "line" );

			m_linesList.Insert( index, line );
		}
		/// <summary>
		/// Creates lexem line with plain text formatting.
		/// </summary>
		/// <param name="iLineIndexVirtual">Virtual line index.</param>
		/// <param name="iLineIndexPhysical">Phisical line index.</param>
		/// <param name="index">Index of the line in the lines list.</param>
		/// <returns>Newly created lexem line.</returns>
		protected virtual ILexemLine CreatePlainTextLine( int iLineIndexVirtual, int iLineIndexPhysical, int index )
		{
			ConfigStack stack = ( m_parsingMode == TextParsingMode.PartialParsingWithFallback ) ? ( CreatePlainTextStack() ) : ( CreateDefaultStack() );

			ILexemLine line = CreateLine( m_streamsWrapper.GetParsePoint( iLineIndexPhysical, 1, true ), stack );
			( ( UncachedLexemLine )line ).LineIndex = iLineIndexVirtual;
			InsertLexemLineIntoList( line, index );

			return line;
		}
		/// <summary>
		/// Looks for the collapsible region at the specified offset and expands it.
		/// </summary>
		/// <param name="offset">Offset of the position where collapsible region should be found.</param>
		/// <returns>True if region was found and expanded.</returns>
		protected bool FindAndExpandRegion( long offset )
		{
			CollapsableRegion region;
			region = GetOuterCollapsableRegion( offset, true );
			bool expand = ( region != null && region.Collapsed );

			if( expand )
			{
				region.Collapsed = false;
				expand = !region.Collapsed;
			}

			return expand;
		}
		/// <summary>
		/// Raises OperationStarted event.
		/// </summary>
		/// <param name="operation">Operation, that is started.</param>
		void ILongOperationControllerInternal.RaiseOperationStart( ILongOperation operation )
		{
			if( OperationStarted != null )
			{
				OperationStarted( operation );
			}
		}
		/// <summary>
		/// Raises OperationStopped event.
		/// </summary>
		/// <param name="operation">Operation, that is stopped.</param>
		void ILongOperationControllerInternal.RaiseOperationEnd( ILongOperation operation )
		{
			if( OperationStopped != null )
			{
				OperationStopped( operation );
			}
		}
		/// <summary>
		/// Starts updating regions.
		/// </summary>
		protected virtual void BeginUpdateRegionsInternal()
		{
		}
		/// <summary>
		/// Ends updating regions.
		/// </summary>
		protected virtual void EndUpdateRegionsInternal()
		{
			UpdateLineInformation();
		}
		/// <summary>
		/// Raiser for TextInserted event.
		/// </summary>
		/// <param name="text">Text, that is deleted.</param>
		/// <param name="iStartLine">Virtual line, where the text starts.</param>
		/// <param name="iStartColumn">Virtual column, where the text starts.</param>
		protected void RaiseTextInsertedEvent( string text, int iStartLine, int iStartColumn )
		{
			if( TextInserted != null )
			{
				TextInserted( this, new TextChangedEventArgs( text, iStartLine, iStartColumn, TextChange.Inserted ) );
			}
		}
		/// <summary>
		/// Raiser for TextDeleted event.
		/// </summary>
		/// <param name="text">Text, that is deleted.</param>
		/// <param name="iStartLine">Virtual line, where the text starts.</param>
		/// <param name="iStartColumn">Virtual column, where the text starts.</param>
		protected void RaiseTextDeletedEvent( string text, int iStartLine, int iStartColumn )
		{
			if( TextDeleted != null )
			{
				TextDeleted( this, new TextChangedEventArgs( text, iStartLine, iStartColumn, TextChange.Deleted ) );
			}
		}
		/// <summary>
		/// Raiser for TextInserting event.
		/// </summary>
		/// <param name="text">Text, that is deleted.</param>
		/// <param name="iStartLine">Virtual line, where the text starts.</param>
		/// <param name="iStartColumn">Virtual column, where the text starts.</param>
		/// <returns>False if action was canceled, otherwise - true.</returns>
		protected bool RaiseTextInsertingEvent( string text, int iStartLine, int iStartColumn )
		{
			if( TextInserting != null )
			{
				TextChangingEventArgs args = new TextChangingEventArgs( text, iStartLine, iStartColumn, TextChange.Inserted );
				TextInserting( this, args );
				return !args.Cancel;
			}

			return true;
		}
		/// <summary>
		/// Raiser for TextDeleting event.
		/// </summary>
		/// <param name="text">Text, that is deleted.</param>
		/// <param name="iStartLine">Virtual line, where the text starts.</param>
		/// <param name="iStartColumn">Virtual column, where the text starts.</param>
		/// <returns>False if action was canceled, otherwise - true.</returns>
		protected bool RaiseTextDeletingEvent( string text, int iStartLine, int iStartColumn )
		{
			if( TextDeleting != null )
			{
				TextChangingEventArgs args = new TextChangingEventArgs( text, iStartLine, iStartColumn, TextChange.Deleted );
                text = text.Replace("\r", string.Empty);
                string[] strings = text.Split('\n');
                int linesCount = 0;
                if (strings != null)
                    linesCount = strings.Length;
                if (linesCount <= 1)
				TextDeleting( this, args );
				return !args.Cancel;
			}

			return true;
		}
		/// <summary>
		/// Raisers of the LinesCountChanged event.
		/// </summary>
		/// <param name="oldValue">Old count of lines.</param>
		internal void RaiseLinesCountChangedEvent( int oldValue )
		{
			if( LinesCountChanged != null )
			{
				LinesCountChanged( this, new ValueChangedEventArgs( oldValue, TotalLines ) );
			}
		}
		/// <summary>
		/// Raises LineInstanceCreated event;
		/// </summary>
		/// <param name="line">Line, that is created.</param>
		protected void RaiseLineInstanceCreatedEvent( ILexemLine line )
		{
			if( line == null )
				throw new ArgumentNullException( "line" );

			if( LineInstanceCreated != null )
			{
				LineInstanceCreated( line, EventArgs.Empty );
			}
		}
		/// <summary>
		/// Raises LineInstanceDeleted event;
		/// </summary>
		/// <param name="line">Line, that is deleted.</param>
		protected void RaiseLineInstanceDeletedEvent( ILexemLine line )
		{
			if( line == null ) throw new ArgumentNullException( "line" );

			if( LineInstanceDeleted != null )
			{
				LineInstanceDeleted( line, EventArgs.Empty );
			}
		}
		/// <summary>
		/// Makes all needed updates after changing <see cref="CollapsingEnabled"/> state.
		/// </summary>
		protected virtual void OnCollapsingEnabledChanged()
		{
			UpdateLineInformation();
		}
		/// <summary>
		/// Creates new object that implements ILexemLine interface. This function is intended to be
		/// overridden to use other than default implementation of ILexemLine.
		/// </summary>
		/// <param name="pointLineStart">ParsePoint of the line start.</param>
		/// <param name="stack">Stack at the beginning of the line.</param>
		/// <returns>ILexemLine interface of the line</returns>
		protected virtual ILexemLine CreateLine( IParsePoint pointLineStart, ConfigStack stack )
		{
			LexemLine line = new LexemLine( this, pointLineStart, stack );
			RaiseLineInstanceCreatedEvent( line );

			return line;
		}
		/// <summary>
		/// Creates new lexem. Can be overriden.
		/// </summary>
		/// <param name="text">Text for the lexem.</param>
		/// <param name="config"></param>
		/// <returns>New lexem.</returns>
		protected virtual Lexem CreateLexem( string text, IConfigLexem config )
		{
			return new Lexem( text, config );
		}
		/// <summary>
		/// Parses line, that is next to given one. Cache is not used.
		/// </summary>
		/// <param name="line">Current line.</param>
		/// <returns>New line.</returns>
		protected virtual ILexemLine GetNextLine( ILexemLine line )
		{
			if( line == null ) throw new ArgumentNullException( "line" );

			int iLine = line.LineIndex + 1;
			IParsePoint startPoint = line.LineEndPoint;

			if( startPoint == null )
				throw new NullReferenceException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_97 );

			if( iLine <= this.TotalLines )
			{
				line = CreateLine( startPoint, line.LineEndStack );
				( line as UncachedLexemLine ).LineIndex = iLine;
				return line;
			}

			return null;
		}
		/// <summary>
		/// Reads token and creates lexem with configuration, based on token and stack of configurations.
		/// </summary>
		/// <param name="openLexems">Configs stack of the opened lexems. It must have at least 1 element (Language).
		/// After reading of the last element it will be empty.
		/// </param>
		/// <remarks>
		/// Checking of the collapsed regions is done
		/// </remarks>
		/// <returns>Readed lexem or null if end of file reached.</returns>
		private ILexem GetLexemInternal( ConfigStack openLexems )
		{
			if( openLexems == null ) throw new ArgumentNullException( "openLexems" );
			if( openLexems.Count == 0 )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_172, "openLexems" );

			long position = BaseStream.Position;
			ConfigStack oldStack = ( ConfigStack )openLexems.Clone();

			ILexem resultLexem = GetLexemInternalNoCollapse( openLexems );

			if( resultLexem != null )
			{
				ICollapsableConfigLexem collapsableConfig = resultLexem.Config as ICollapsableConfigLexem;

				// If this lexem supports collapsing, then we should check whether
				// it is collapsed or add it to the collection of collapsible regions
				if( collapsableConfig != null && collapsableConfig.IsCollapsable && resultLexem.Config.IsEqualToBegin( resultLexem.Text ) )
				{
					CollapsableRegion region = GetCollapsableRegion( position, oldStack );

					if( ( region.Collapsed ) && m_bCollapsingEnabled )
					{
						// Stack should be used only for comparision.
						resultLexem = ParseCollapsableRegion( region, ( ConfigStack )oldStack.Clone() );
						FillStack( oldStack, openLexems );
					}
					else
					{
						( resultLexem as IEditableLexem ).Collapser = region;
					}
				}
			}

			return resultLexem;
		}
		/// <summary>
		/// Fills destination stack by content of source stack.
		/// </summary>
		/// <remarks>
		/// All content of the destination stack is erased and it is filled by items of the source stack in those
		/// order as they are in source.
		/// </remarks>
		/// <param name="source">Source stack.</param>
		/// <param name="destination">Destination stack.</param>
		protected void FillStack( ConfigStack source, ConfigStack destination )
		{
			if( source == null )
				throw new ArgumentNullException( "source" );

			if( destination == null )
				throw new ArgumentNullException( "destination" );

			destination.Clear();
			object[] stackItems = source.ToArray();

			for( int i = stackItems.Length - 1; i >= 0; i-- )
				destination.Push( stackItems[ i ] );
		}
		/// <summary>
		/// Gets or creates new <see cref="CollapsableRegion"/> by give offset.
		/// </summary>
		/// <remarks>
		/// Note: if region does not exists, it will be created.
		/// </remarks>
		/// <param name="streamPosition">Offset in the stream.</param>
		/// <param name="stackStart">Start stack of the region.</param>
		/// <returns><see cref="CollapsableRegion"/> for this offset.</returns>
		private CollapsableRegion GetCollapsableRegion( long streamPosition, ConfigStack stackStart )
		{
			int regionIndex = m_lastCollapseRegionIndex;

			// We should check whether this index is correct for current offset in stream.
			if( regionIndex < 0 || regionIndex >= m_collapseList.Count )
				regionIndex = -1;

			if( regionIndex >= 0 )
			{
				CollapsableRegion reg = m_collapseList[ regionIndex ] as CollapsableRegion;
				CollapsableRegion regNext = ( regionIndex < m_collapseList.Count - 1 )
					? m_collapseList[ regionIndex ] as CollapsableRegion : null;

				if( reg.Start.Offset != streamPosition &&
					( regNext == null || regNext.Start.Offset != streamPosition ) )
					regionIndex = -1;
			}

			if( regionIndex < 0 )
			{
				regionIndex = m_collapseList.BinarySearch( streamPosition );

				if( regionIndex < 0 )
				{
					m_collapsedLines = -1;
					regionIndex = ~regionIndex;
					CollapsableRegion regNew = new CollapsableRegion();
					regNew.Collapsed = m_bInitialCollapsing;
					regNew.RegionDeleted += new EventHandler( OnRegNewRegionDeleted );
					regNew.CollapsedStateChanged += new EventHandler( OnRegNewCollapsedStateChanged );
					regNew.OutliningBeforeCollapse += new OutliningCancellableEventHandler( OnRegNewOutliningBeforeCollapse );
					regNew.OutliningBeforeExpand += new OutliningCancellableEventHandler( OnRegNewOutliningBeforeExpand );
					regNew.OutliningCollapse += new OutliningEventHandler( OnRegNewOutliningCollapse );
					regNew.OutliningExpand += new OutliningEventHandler( OnRegNewOutliningExpand );
					regNew.Start = BaseStream.GetParsePoint( streamPosition );
					regNew.StartStack = stackStart;
					m_collapseList.Insert( regionIndex, regNew );
				}
			}

			CollapsableRegion region = m_collapseList[ regionIndex ] as CollapsableRegion;
			m_lastCollapseRegionIndex = regionIndex;

			return region;
		}
		/// <summary>
		/// Peeks last lexem from stack.
		/// </summary>
		/// <param name="stack">Stack with lexems and their configurations.</param>
		/// <returns>Peeked lexem, or null if there is stack contains just language,
		/// or last entry is some delegated(with NextID) configuration.</returns>
		private ILexem PeekLexemFromStack( ConfigStack stack )
		{
			if( stack == null )
				throw new ArgumentNullException( "stack" );

			if( stack.Count == 0 )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_173 );

			ConfigLexem_Lexem_Pair pair = ( ConfigLexem_Lexem_Pair )stack.Peek();

			return pair.Lexem;
		}
		/// <summary>
		/// Peeks last lexem configuration from stack.
		/// </summary>
		/// <param name="stack">Stack with lexems and their configurations.</param>
		/// <returns>Peeked lexem configuration, never returns null.</returns>
		private IConfigLexem PeekConfigFromStack( ConfigStack stack )
		{
			if( stack == null )
				throw new ArgumentNullException( "stack" );

			if( stack.Count == 0 )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_173 );

			ConfigLexem_Lexem_Pair pair = ( ConfigLexem_Lexem_Pair )stack.Peek();

			return pair.Config;
		}
		/// <summary>
		/// Parses collapsed <see cref="CollapsableRegion"/>
		/// and represents it as one lexem.
		/// </summary>
		/// <param name="region">Region to parse.</param>
		/// <param name="openLexems">Stack at the beginning of the region.</param>
		/// <returns><see cref="ILexem"/> with CollapsedText formatting.</returns>
		private ILexem ParseCollapsableRegion( CollapsableRegion region, ConfigStack openLexems )
		{
			if( region == null ) throw new ArgumentNullException( "region" );
			if( openLexems == null ) throw new ArgumentNullException( "openLexems" );

			bool bPseudoEnd = ( ( IStackData )m_lastStack.Peek() ).Config.IsPseudoEnd;
			int stackLength = openLexems.Count;
			BaseStream.SetPositionToParsePoint( region.Start );
			ILexem firstLexem = GetLexemInternalNoCollapse( openLexems );
			long lContinuePosition = BaseStream.Position;
			ConfigStack startStack = ( ConfigStack )openLexems.Clone();

			ICollapsableConfigLexem config = firstLexem.Config as ICollapsableConfigLexem;

			string collapseName = string.Empty;

			if( config.IsCollapseAutoNamed )
			{
				string lastReadToken;
				Match autoNameMatch = null;
				int newStrCount = 0;
				collapseName = firstLexem.Text;
				bool stop = false;

				do
				{
					lastReadToken = BaseStream.ReadToken();

					if( lastReadToken == null )
					{
						stop = true;
						lastReadToken = BaseStream.NewLineStr;
					}

					if( lastReadToken == BaseStream.NewLineStr )
					{
						newStrCount++;
					}

					collapseName += lastReadToken;
					autoNameMatch = config.AutoNameRegex.Match( collapseName );

				}
				while( !stop && !autoNameMatch.Success && newStrCount <= DEF_COLLAPSE_NAME_MAX_LINES );

				if( autoNameMatch != null && autoNameMatch.Success )
				{
					collapseName = config.AutoNameRegex.Replace( collapseName, config.AutoNameTemplate );
				}
				else
				{
					collapseName = config.CollapseName;
				}

				// Return to the previous state.
				BaseStream.Position = lContinuePosition;
			}

			if( collapseName == string.Empty )
			{
				collapseName = config.CollapseName;
			}

			IConfigLexem configFirstLexem = firstLexem.Config;
			string configFirstLexemCollapseConfig = configFirstLexem.TypeCollapsed;

			// Crearing resulting lexem with formatting of the collapsed text.
			ConfigLexem resultConfig = new ConfigLexem();

			if( configFirstLexemCollapseConfig == null || configFirstLexemCollapseConfig == string.Empty )
			{
				resultConfig.Type = FormatType.CollapsedText;
			}
			else
			{
				resultConfig.Type = FormatType.Custom;
				resultConfig.FormatName = configFirstLexem.TypeCollapsed;
			}

			resultConfig.ParentConfig = m_language as IConfigLexem;
			resultConfig.IsCollapsable = true;
			resultConfig.BeginBlock = collapseName;
			resultConfig.EndBlock = collapseName;

			ILexem resultLexem = CreateLexem( collapseName, resultConfig );

			region.Lexem = firstLexem;
			bool bStartStackLengthMismatch = ( region.StartStack != null && openLexems.Count != region.StartStack.Count );

			if( region.EndStack != null && ( region.UnreliableEnding || bStartStackLengthMismatch ) )
			{
				region.EndStack = null;
			}

			if( region.EndStack != null )
			{
				//Read last lexem
				long oldStreamPosition = BaseStream.Position;
				BaseStream.SetPositionToParsePoint( region.End );
				ILexem lastLexem = GetLexemInternalNoCollapse( ( ConfigStack )region.EndStack.Clone() );
				BaseStream.Position = oldStreamPosition;

				if( lastLexem == null || region.EndLexem == null || lastLexem.Config != region.EndLexem.Config )
				{
					region.EndStack = null;
				}
			}

			// If we already have found end of the region,
			// we just skip some space and update stack
			if( region.EndStack != null )
			{
				// Just skip entire region.
				// Set position to the last lexem.
				BaseStream.SetPositionToParsePoint( region.End );
				// Read this lexem.
				GetLexemInternalNoCollapse( openLexems );
				// Set correct stack.
				FillStack( region.EndStack, openLexems );
			}
			else
			{
				int stackStartCount = openLexems.Count;

				long lastPosition = this.BaseStream.Position;
				ConfigStack stackOld = openLexems;
				long pos = lastPosition;

				do
				{
					if( BaseStream.Position >= BaseStream.Length )
					{
						break;
					}

					pos = BaseStream.Position;
					stackOld = ( ConfigStack )openLexems.Clone();
					int iPrevStackLength = openLexems.Count;
					IStackData stackLastData = openLexems.Peek();
					IConfigLexem configLexemLast = stackLastData.Config;

					ILexem lexem = GetLexemInternal( openLexems );

					// If we unfolded the stack, but current lexem is not the end of the block we where inside then it was pseudo ending.
					if( ( iPrevStackLength <= openLexems.Count ) || ( lexem != null && configLexemLast.ID == lexem.Config.ID ) )
					{
						region.EndLexem = lexem;
						stackOld = openLexems;
						lastPosition = pos;
					}

					if( bPseudoEnd )
					{
						lastPosition = pos;
					}
				}
				while( stackStartCount <= openLexems.Count );

				ILexem regionEndLexem = region.EndLexem;
				CollapsableRegion endRegion = ( regionEndLexem != null ) ? ( regionEndLexem.Collapser ) : ( null );

				// If lexem is the representation of the collapsed region, than we should use setting of this region.
				if( endRegion != null && endRegion.Collapsed )
				{
					region.End = endRegion.End;
					region.EndStack = endRegion.EndStack;
					region.EndLexem = endRegion.EndLexem;
				}
				else
				{
					region.End = this.BaseStream.GetParsePoint( lastPosition );
					region.EndStack = ( ConfigStack )stackOld.Clone();
				}

				if( lastPosition != pos )
				{
					IParsePoint pointMonitored = BaseStream.GetParsePoint( pos );
					region.AttachMonitoredEndPoint( pointMonitored );
				}

				if( bPseudoEnd )
				{
					this.BaseStream.Position = lastPosition;
				}
			}

			region.StartStack = startStack;
			( resultLexem as IEditableLexem ).Collapser = region;

			region.CollapseName = resultLexem.Text;
			return resultLexem;
		}
		/// <summary>
		/// Pops from stack last item and all upper items, that does not wait ending.
		/// </summary>
		/// <param name="openLexems">Stack of the ConfigLexem_Lexem_Pair objects.</param>
		private void PopStack( ConfigStack openLexems )
		{
			if( openLexems == null ) throw new ArgumentNullException( "openLexems" );
			if( openLexems.Count > 1 )
			{
				ConfigLexem_Lexem_Pair lastPair = openLexems.Pop() as ConfigLexem_Lexem_Pair;
				ConfigLexem lastConfig = lastPair.Config as ConfigLexem;
				if( lastConfig != null && lastConfig.NextID > 0 )
				{
					IConfigLexem firstConfig = ( null != lastPair.FirstConfig ) ? ( lastPair.FirstConfig ) : ( lastPair.Config );
					openLexems.Push( lastConfig.NextLexem, lastPair.Lexem, lastPair.Location, firstConfig );
				}
				else
				{
					while( openLexems.Count > 1 )
					{
						ConfigLexem_Lexem_Pair unclosedPair = ( ConfigLexem_Lexem_Pair )openLexems.Peek();
						if( unclosedPair.Config.EndBlock.Length != 0 )
						{
							break;
						}

						lastPair = openLexems.Pop() as ConfigLexem_Lexem_Pair;
						lastConfig = lastPair.Config as ConfigLexem;
						if( lastConfig != null && lastConfig.NextID > 0 )
						{
							IConfigLexem firstConfig = ( null != lastPair.FirstConfig ) ? ( lastPair.FirstConfig ) : ( lastPair.Config );
							openLexems.Push( lastConfig.NextLexem, lastPair.Lexem, lastPair.Location, firstConfig );
							break;
						}
					}
				}
			}
		}
		/// <summary>
		/// Tries to read lexem using specified non-complex config.
		/// </summary>
		/// <param name="config">Configuration to be used.</param>
		/// <param name="token"></param>
		/// <returns>Return null is configuration is not suitable in current context,
		/// or string, that represents possibly expanded token.
		/// (non-complex lexem configurations with continue or end blocks 
		/// must be represented with a single token and single lexem.)</returns>
		private object TryReadNonComplexLexem( IConfigLexem config, string token )
		{
			if( config == null )
				throw new ArgumentNullException( "config" );

			long oldPosition = BaseStream.Position;
			string lastContinueToken = string.Empty;

			// Try to expand using continue block.
			if( config.ContinueBlock != string.Empty )
			{
				string peekedToken = m_streamsWrapper.PeekToken();
				while( peekedToken != null && config.IsEqualToContinue( peekedToken ) )
				{
					if( peekedToken == m_streamsWrapper.NewLineStr )
						break;

					lastContinueToken = m_streamsWrapper.ReadToken();
					token += lastContinueToken;

					if( config.IsEqualToEnd( lastContinueToken ) ) break;

					peekedToken = m_streamsWrapper.PeekToken();
				}
			}

			// Maybe continue block must be the same as end block or end block is not present,
			// then finish processing.
			if( config.EndBlock.Length == 0 || ( lastContinueToken.Length != 0 && config.IsEqualToEnd( lastContinueToken ) ) )
			{
				return token;
			}

			// Check end block.
			string peekToken = m_streamsWrapper.PeekToken();
			if( peekToken != null && config.IsEqualToEnd( peekToken ) && peekToken != m_streamsWrapper.NewLineStr )
			{
				token += m_streamsWrapper.ReadToken();
				return token;
			}

			// Decide, that given config is not suitable in current context.
			BaseStream.Position = oldPosition;
			return null;
		}
		/// <summary>
		/// Selects lexem from the list which has DefaultInGroup set to true.
		/// Needed when parser can not choose lexem config by priority.
		/// </summary>
		/// <param name="list">List with lexem configs.</param>
		/// <returns>Default lexem config or list[0]</returns>
		private IConfigLexem SelectDefaultLexem( IList list )
		{
			if( list == null )
				throw new ArgumentNullException( "list" );

			if( list.Count == 0 ) return null;

			for( int i = list.Count - 1; i >= 0; i-- )
			{
				IConfigLexem lexem = ( IConfigLexem )list[ i ];

				if( lexem.DefaultInGroup )
					return lexem;
			}

			return ( IConfigLexem )list[ 0 ];
		}
		/// <summary>
		/// Selects configuration from the given list.
		/// </summary>
		/// <param name="parentconfig">Parent config.</param>
		/// <param name="configList">List of the configurations, sorted by priorities.</param>
		/// <param name="token">Token, the configuration is for.
		/// In some cases this token can advance to some sentence 
		/// with the same configuration.</param>
		/// <returns>Configuration, that was found, 
		/// or null if there were no suitable configuration found.</returns>
		private IConfigLexem SelectConfigFromList( IConfigLexem parentconfig, IList configList, ref string token )
		{
			try
			{
				// If it is not null, than it was found.
				IConfigLexem nonComplex_Config = null;
				string nonComplex_token = token;

				if( configList.Count > 0 )
				{
					long oldPosition = BaseStream.Position;
					ArrayList variants = new ArrayList( configList.Count );
					string newToken = token;
					int firstPriority = ( configList[ 0 ] as IConfigLexem ).Priority;
					bool bIsEqualToParentEnd =
						( parentconfig != null && parentconfig.IsEqualToEnd( token ) );

					// We should limit priorities with the priority of the lexem, the token is the ending of.
					if( bIsEqualToParentEnd ) firstPriority = parentconfig.Priority;

					for( int i = 0, count = configList.Count; i < count; i++ )
					{
						IConfigLexem conf = configList[ i ] as IConfigLexem;

						// If we already found something on higher priority level.
						if( firstPriority > conf.Priority && ( bIsEqualToParentEnd || ( variants.Count > 0 ) ) )
						{
							break;
						}

						if( conf.IsComplex )
						{
							if( firstPriority == conf.Priority || variants.Count == 0 )
							{
								variants.Add( conf );
							}
						}
						else
						{
							object nonComplexCheckResult = TryReadNonComplexLexem( conf, token );

							if( nonComplexCheckResult != null )
							{
								nonComplex_Config = conf;
								nonComplex_token = ( string )nonComplexCheckResult;

								// Non complex constructions always follow complex, 
								// so we can break processing right now.
								break;
							}
						}
					}

					// If we have just single non-complex config, then return it.
					if( variants.Count == 0 )
					{
						// if token is newline mark, we can't change it because it will cause lines parsing crash. Fix for def. 3870.
						if( token != BaseStream.NewLineStr )
						{
							token = nonComplex_token;
						}
						else
						{
							m_streamsWrapper.Position = oldPosition;
						}

						return nonComplex_Config;
					}
					else
					{
						m_streamsWrapper.Position = oldPosition;
					}

					if( variants.Count == 1 )
						return SelectDefaultLexem( variants );

					string nextToken = m_streamsWrapper.ReadToken();

					if( nextToken == null ) return SelectDefaultLexem( variants );

					Hashtable table = new Hashtable( variants.Count * 2 );
					ArrayList keysList = new ArrayList();

					foreach( IConfigLexem config in variants )
					{
						IList foundList = config.FindConfigs( nextToken );

						for( int i = 0; i < foundList.Count; i++ )
						{
							IConfigLexem foundConfig = foundList[ i ] as IConfigLexem;

							if( foundConfig.IsComplex )
							{
								if( !keysList.Contains( foundConfig ) )
								{
									keysList.Add( foundConfig );
									table[ foundConfig ] = config;
								}
							}
							else
							{
								if( !keysList.Contains( config ) )
								{
									keysList.Add( config );
									table[ config ] = config;
								}
							}
						}

						if( foundList.Count == 0 && config.IsEqualToEnd( nextToken ) )
						{
							keysList.Add( config );
							table[ config ] = config;
						}
					}

					keysList.Sort();

					string tmp = string.Empty;
					IConfigLexem foundLexem = SelectConfigFromList( null, keysList, ref tmp );
					BaseStream.Position = oldPosition;

					if( foundLexem == null ) return SelectDefaultLexem( variants );

					return table[ foundLexem ] as IConfigLexem;
				}

				return null;
			}
			finally
			{
			}
		}
		/// <summary>
		/// Reads one token, then looks for it`s format ( by stack )
		/// If it is complex lexem, current lexem config will be added to stack;
		/// </summary>
		/// <param name="openLexems">Configs stack of the opened lexems.
		/// It must have at least 1 element (Language).
		/// After reading of the last element it will be empty.
		/// </param>
		/// <returns>Readed lexem or null if end of file reached.</returns>
		private ILexem GetLexemInternalNoCollapse( ConfigStack openLexems )
		{
			if( openLexems == null ) throw new ArgumentNullException( "openLexems" );
			if( openLexems.Count == 0 )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_172, "openLexems" );

			// Read token from stream.
			long lTokenPosition = m_streamsWrapper.Position;
			string token = m_streamsWrapper.ReadToken();

			IConfigLexem config = null;
			IConfigLexem newLexemConfig = null;
			// If token caused close of Stacks Top lexem, then it must be processed once more.
			bool bNeedMoreProcessing = false;
			bool bNeedConfig = true;
			int iTokenProcessCount = 0;
			IConfigLexem parentConfig = null;

			// If end of file reached, then update lexems in stack, clear it and return null
			if( token == null || token == string.Empty )
			{
				while( openLexems.Count > 1 )
				{
					openLexems.Pop();
				}

				return null;
			}

			do
			{
				iTokenProcessCount++;
				bNeedMoreProcessing = false;

				ConfigLexem_Lexem_Pair topPair = ( ConfigLexem_Lexem_Pair )openLexems.Peek();
				parentConfig = topPair.Config;

				// If top of the stack is language, then parentLexem will be null
				Lexem parentLexem = topPair.Lexem as Lexem;

				if( bNeedConfig )
				{
					bNeedConfig = false;

					// Looking for lexem configs
					IList confList = parentConfig.FindConfigs( token );
					config = SelectConfigFromList( parentConfig, confList, ref token );

					newLexemConfig = null;

					if( config != null )
						newLexemConfig = config;
					else
					{
						if( parentConfig.OnlyLocalSublexems )
						{
							newLexemConfig = parentConfig.VirtualConfig;
						}
						else
							bNeedConfig = true;
					}
				}

				if( config != null && !parentConfig.OnlyLocalSublexems
					&& config.ParentConfig != parentConfig && parentConfig.IsEqualToEnd( token ) )
					config = null;

				if( config == null )
				{
					// Check for end of parentConfig. If true, remove it from stack
					if( parentConfig.IsEqualToEnd( token ) )
					{
						PopStack( openLexems );

						if( bNeedConfig || parentConfig.IsPseudoEnd )
						{
							// We already know format of current lexem
							newLexemConfig = parentConfig;
							config = null;
							bNeedConfig = parentConfig.IsPseudoEnd;
							bNeedMoreProcessing = parentConfig.IsPseudoEnd;
						}
					}
					else if( token == m_streamsWrapper.NewLineStr && parentLexem != null )
					{
						// If unprocessed endline found and it is not in global scope,
						// then upper lexem is incorrect, also this \n can be incorrect ending of 
						// some other lexems, so it must be processed again.
						bNeedMoreProcessing = true;
						bNeedConfig = true;
						PopStack( openLexems );
					}
					else if( parentConfig.EndBlock.Length == 0 && parentLexem != null )
					{
						// If there is no ending needed.
						PopStack( openLexems );
						bNeedMoreProcessing = true;
						bNeedConfig = true;
					}
					else
					{
						// Set language as a config lexem.
						bNeedMoreProcessing = false;

						if( newLexemConfig == null )
							newLexemConfig = m_language as IConfigLexem;
					}
				}
			}
			while( bNeedMoreProcessing );

			Lexem resultLexem = CreateLexem( token, newLexemConfig );

			// If it is beginning of lexem, push it to the stack
			if( config != null && config.IsComplex )
			{
				openLexems.Push( config, resultLexem, BaseStream.GetParsePoint( lTokenPosition ) );
			}

			return resultLexem;
		}
		/// <summary>
		/// Gets list of numbers of changed lines.
		/// </summary>
		/// <returns>List of indexes of changed lines.</returns>
		internal ArrayList GetChangedLinesNumbers()
		{
			ArrayList result = new ArrayList();

			for( int i = 0, count = this.LinesList.Count; i < count; i++ )
			{
				RenderedLine line = ( RenderedLine )this.LinesList[ i ];

				if( line.Changed )
				{
					result.Add( i + 1 );
				}
			}

			return result;
		}
		/// <summary>
		/// Clears info abuot changed lines.
		/// </summary>
		internal void ClearChangedLines()
		{
			foreach( RenderedLine line in this.LinesList )
			{
				line.ForcedChanged = false;
			}
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Handler of the LinesCountChanged event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSourceLinesCountChanged( object sender, ValueChangedEventArgs e )
		{
			RaiseLinesCountChangedEvent( -1 );
		}
        /// <summary>
        /// Handler of the LineInserted event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void source_LineInserted(object sender, LinesEventArgs e)
        {
            if (LineInserted != null)
                LineInserted(this, e);
        }
        /// <summary>
        /// Handler of the LineDeleted event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void source_LineDeleted(object sender, LinesEventArgs e)
        {
            if (LineDeleted != null)
                LineDeleted(this, e);
        }
		/// <summary>
		/// Handler of RegionDeleted event of all collapsible regions.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRegNewRegionDeleted( object sender, EventArgs e )
		{
			m_collapsedLines = -1;
			m_collapseList.Remove( sender );
		}
		/// <summary>
		/// Handler of CollapsedStateChanged event for all collapsible regions.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		protected virtual void OnRegNewCollapsedStateChanged( object sender, EventArgs e )
		{
			CollapsableRegion region = sender as CollapsableRegion;

			m_collapsedLines = -1;
			IParsePoint startPoint = region.Start;
			CoordinatePoint point = GetCoordinatePoint( startPoint, true );

			try
			{
				ILexemLine line = GetLine( point.VirtualLine );
				line.DeleteSelf();
				GetLine( point.VirtualLine ).Parsed = true;
			}
			catch { Debug.WriteLine( "Unparsed line collapsed." ); }

			UpdateLineInformation();

			if( OutliningStateChanged != null )
			{
				OutliningStateChanged( sender, e );
			}
		}
		/// <summary>
		/// Handler for the UndoBufferFlushed event of the changes stream.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnUndoBufferFlush( object sender, EventArgs e )
		{
			m_undoredoData.UndoStack.Clear();
		}
		/// <summary>
		/// Handler for the RedoBufferFlushed event of the changes stream.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRedoBufferFlush( object sender, EventArgs e )
		{
			m_undoredoData.RedoStack.Clear();
		}
		/// <summary>
		/// Handler for the OutliningBeforeCollapse event of the collapsible region.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRegNewOutliningBeforeCollapse( object sender, OutliningEventArgs e )
		{
			if( null == sender )
				throw new ArgumentNullException( "sender" );
			if( !( sender is CollapsableRegion ) )
				throw new ArgumentOutOfRangeException( "sender" );

			if( null != OutliningBeforeCollapse )
			{
				CollapsableRegion region = ( CollapsableRegion )sender;
				string text = BaseStream.GetTextInRange( region.Start, region.End, false );
				e.CollapsedText = text;
				OutliningBeforeCollapse( this, e );
			}
		}
		/// <summary>
		/// Handler for the OutliningBeforeExpand event of the collapsible region.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRegNewOutliningBeforeExpand( object sender, OutliningEventArgs e )
		{
			if( null == sender )
				throw new ArgumentNullException( "sender" );
			if( !( sender is CollapsableRegion ) )
				throw new ArgumentOutOfRangeException( "sender" );

			if( null != OutliningBeforeExpand )
			{
				CollapsableRegion region = ( CollapsableRegion )sender;
				string text = BaseStream.GetTextInRange( region.Start, region.End, false );
				e.CollapsedText = text;
				OutliningBeforeExpand( this, e );
			}
		}
		/// <summary>
		/// Handler for the OutliningCollapse event of the collapsible region.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRegNewOutliningCollapse( object sender, CollapseEventArgs e )
		{
			if( null == sender ) throw new ArgumentNullException( "sender" );
			if( !( sender is CollapsableRegion ) ) throw new ArgumentException( "sender" );

			if( OutliningCollapse != null )
			{
				CollapsableRegion region = ( CollapsableRegion )sender;
				string text = BaseStream.GetTextInRange( region.Start, region.End, false );
				e.CollapsedText = text;
				OutliningCollapse( this, e );
			}
		}
		/// <summary>
		/// Handler for the OutliningExpand event of the collapsible region.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnRegNewOutliningExpand( object sender, CollapseEventArgs e )
		{
			if( null == sender )
				throw new ArgumentNullException( "sender" );
			if( !( sender is CollapsableRegion ) )
				throw new ArgumentOutOfRangeException( "sender" );

			if( null != OutliningExpand )
			{
				CollapsableRegion region = ( CollapsableRegion )sender;
				string text = BaseStream.GetTextInRange( region.Start, region.End, false );
				e.CollapsedText = text;
				OutliningExpand( this, e );
			}
		}
		/// <summary>
		/// Locks consistence checks.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSourceBeforeTextChange( object sender, EventArgs e )
		{
			LockConsistenceChecks();
		}
		/// <summary>
		/// Unlocks consistence checks.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSourceAfterTextChange( object sender, EventArgs e )
		{
			UnlockConsistenceChecks();
		}
		#endregion
	}
	#endregion
}