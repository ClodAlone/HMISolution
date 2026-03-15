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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Forms.Popup;
using Syncfusion.Windows.Forms.Edit.Implementation;
using Syncfusion.Windows.Forms.Edit.Implementation.IO;
using Syncfusion.Windows.Forms.Edit.Implementation.Parser;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Utils;
using Syncfusion.Windows.Forms.Edit.Utils.CodeSnippets;
using Syncfusion.ComponentModel;

namespace Syncfusion.Windows.Forms.Edit
{
	#region EventArgs Descendants
	/// <summary>
	/// Event arguments class used in StreamCloseEventHandler event handler.
	/// </summary>
	public class StreamCloseEventArgs
		: EventArgs
	{
		#region Public Fields
		/// <summary>
		/// Gets or sets action to be executed on the modified file.
		/// </summary>
		public SaveChangesAction Action;
		#endregion

		#region Initialization
		/// <summary>
		/// Creates and initalizes new instance of the event arguments class.
		/// </summary>
		/// <param name="action">Action to be executed on the modified file.</param>
		public StreamCloseEventArgs( SaveChangesAction action )
		{
			this.Action = action;
		}
		#endregion
	}

	/// <summary>
	/// This class sends messages to the event handlers. It contains all information needed
	/// for rendering and controlling the drawing process.
	/// </summary>
	public class CustomSnippetDrawEventArgs
		: PaintEventArgs
	{
		#region Fields
		/// <summary>
		/// Stoarge of Format property.
		/// </summary>
		private ISnippetFormat m_format;
		/// <summary>
		/// Storage of Snippet property.
		/// </summary>
		private string m_text;
		/// <summary>
		/// Storage of SkipDefaultDrawing property.
		/// </summary>
		private bool m_bSkipDefault;
		/// <summary>
		/// Storage for the result of measuring.
		/// </summary>
		private TextInfo m_textInfo;
		/// <summary>
		/// A flag that specifies whether the user has to Draw text or just Measure. Measuring is not needed if drawing is performed.
		/// </summary>
		private bool m_bMeasure;
		#endregion

		#region Properties
		/// <summary>
		/// Gets a flag that specifies whether the user has to Draw text or just Measure. Measuring is not needed if drawing is performed.
		/// </summary>
		public bool Measure
		{
			get
			{
				return m_bMeasure;
			}
		}
		/// <summary>
		/// Gets or sets result of measuring.
		/// </summary>
		public TextInfo MeasuringResult
		{
			get
			{
				return m_textInfo;
			}
			set
			{
				m_textInfo = value;
			}
		}
		/// <summary>
		/// Gets or sets format which will be used by default renderer. This format can be changed by user to influence the default renderer.
		/// </summary>
		public ISnippetFormat Format
		{
			get
			{
				return m_format;
			}
			set
			{
				m_format = value;
			}
		}
		/// <summary>
		/// Gets or sets the text which will be sent to the renderer.
		/// </summary>
		public string Text
		{
			get
			{
				return m_text;
			}
			set
			{
				if( m_text.Length != value.Length )
					throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_9, "value" );

				m_text = value;
			}
		}
		/// <summary>
		/// Gets or sets flag that determines whether user wishes to do his own drawing. If set to false, default renderer will step on user work.
		/// </summary>
		public bool SkipDefaultDrawing
		{
			get
			{
				return m_bSkipDefault;
			}
			set
			{
				m_bSkipDefault = value;
			}
		}
		#endregion

		#region Initialization & Finalization
        /// <summary>
        /// Second helper constructor
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        /// <param name="format">Default Format used for rendering</param>
        /// <param name="text">Text which must be rendered</param>
        /// <param name="measure">A flag that specifies whether the user has to Draw text or just Measure.
        /// Measuring is not needed if drawing is performed.</param>
		public CustomSnippetDrawEventArgs( PaintEventArgs args, ISnippetFormat format, string text, bool measure )
			: this( args.Graphics, args.ClipRectangle, format, text, measure )
		{
		}
		/// <summary>
		/// Main constructor
		/// </summary>
		/// <param name="g">Graphics object used for rendering</param>
		/// <param name="rc">Destination rectangle</param>
		/// <param name="format">Default Format used for rendering</param>
		/// <param name="text">Text which must be rendered</param>
		/// <param name="measure">A flag that specifies whether the user has to Draw text or just Measure.
		/// Measuring is not needed if drawing is performed.</param>
		public CustomSnippetDrawEventArgs( Graphics g, Rectangle rc, ISnippetFormat format, string text, bool measure )
			: base( g, rc )
		{
			if( g == null ) throw new ArgumentNullException( "g" );
			if( rc == RectangleF.Empty ) throw new ArgumentOutOfRangeException(
				"rc", rc, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_10 );
			if( format == null ) throw new ArgumentNullException( "format" );

			m_format = format;
			m_bMeasure = measure;
			m_text = text;
		}
		#endregion
	}

	/// <summary>
	///	 Event arguments for ValueChanged event handler.
	/// </summary>
	public class ValueChangedEventArgs
		: EventArgs
	{
		#region Fields
		/// <summary>
		/// Old value.
		/// </summary>
		private object m_old;
		/// <summary>
		///	New value.
		/// </summary>
		private object m_new;
		#endregion

		#region Properties
		/// <summary>
		///	Gets new value.
		/// </summary>
		public object newValue
		{
			get
			{
				return m_new;
			}
		}
		/// <summary>
		///	Gets old vale.
		/// </summary>
		public object oldValue
		{
			get
			{
				return m_old;
			}
		}
		#endregion

		#region Initialization & Finalization
        /// <summary>
        /// Creates new instance of ValueChangedEventArgs.
        /// </summary>
        /// <param name="old">The old value.</param>
        /// <param name="newValue">The new value.</param>
		public ValueChangedEventArgs( object old, object newValue )
		{
			m_old = old;
			m_new = newValue;
		}
		#endregion
	}

	/// <summary>
	/// Arguments for the text changes event.
	/// </summary>
	public class TextChangedEventArgs
		: EventArgs
	{
		#region Fields
		/// <summary>
		/// Event's text.
		/// </summary>
		private string m_Text;
		/// <summary>
		/// Virtual line of Insert/Delete start.
		/// </summary>
		private int m_StartLine;
		/// <summary>
		/// Virtual column of Insert/Delete start.
		/// </summary>
		private int m_StartColumn;
		/// <summary>
		/// Type of the event ( Insert/Delete ).
		/// </summary>
		private TextChange m_Type;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets event's text.
		/// </summary>
		public string Text
		{
			get
			{
				return m_Text;
			}
			set
			{
				m_Text = value;
			}
		}
		/// <summary>
		/// Gets or sets virtual line of Insert/Delete start.
		/// </summary>
		public int StartLine
		{
			get
			{
				return m_StartLine;
			}
			set
			{
				m_StartLine = value;
			}
		}
		/// <summary>
		/// Gets or sets virtual column of Insert/Delete start.
		/// </summary>
		public int StartColumn
		{
			get
			{
				return m_StartColumn;
			}
			set
			{
				m_StartColumn = value;
			}
		}
		/// <summary>
		/// Gets or sets type of the event ( Insert/Delete ).
		/// </summary>
		public TextChange Type
		{
			get
			{
				return m_Type;
			}
			set
			{
				m_Type = value;
			}
		}
		#endregion

		#region Initialization & Finalization
		/// <summary>
		/// Creates and initializes new instance of TextChangedEventArgs class.
		/// </summary>
		/// <param name="text">Event`s text.</param>
		/// <param name="iStartLine">Virtual line of Insert/Delete start.</param>
		/// <param name="iStartColumn">Virtual column of Insert/Delete start.</param>
		/// <param name="type">Type of the event ( Insert/Delete ).</param>
		public TextChangedEventArgs( string text, int iStartLine, int iStartColumn, TextChange type )
		{
			m_Text = text;
			m_StartLine = iStartLine;
			m_StartColumn = iStartColumn;
			m_Type = type;
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Gets string representation of the object.
		/// </summary>
		/// <returns>String representation of the object.</returns>
		public override string ToString()
		{
			return string.Format( "{{Action:{3}; Line:{1}; Column:{2}; Text: \"{0}\"}}", Text, StartLine, StartColumn, m_Type );
		}
		#endregion
	}

	/// <summary>
	/// Arguments for DrawLineMarkEventHandler.
	/// </summary>
	public class DrawLineMarkEventArgs
		: EventArgs
	{
		#region Public Fields
		/// <summary>
		/// Virtual line number.
		/// </summary>
		public readonly int PhysicalLine;
		/// <summary>
		/// Physical line number.
		/// </summary>
		public readonly int VirtualLine;
		/// <summary>
		/// Rectangle where linemark should be drawn.
		/// </summary>
		public readonly Rectangle MarkRect;
		/// <summary>
		/// Graphics object.
		/// </summary>
		public readonly Graphics Graphics;
		/// <summary>
		/// If set to true, user handles drawing of the bookmark.
		/// </summary>
		public bool CustomDraw;
		#endregion

		#region Initialization & Finalization
		/// <summary>
		/// Creates and initializes instance of the PaintLineMarkEventArgs class.
		/// </summary>
		/// <param name="graphics">Graphics objects where user has to draw line marks.</param>
		/// <param name="markRect">Rectangle where line mark should be drawn.</param>
		/// <param name="iVirtualLine">Virtual number of the line.</param>
		/// <param name="iPhysicalLine">Physical number of the line.</param>
		public DrawLineMarkEventArgs( Graphics graphics, Rectangle markRect, int iVirtualLine, int iPhysicalLine )
		{
			PhysicalLine = iPhysicalLine;
			VirtualLine = iVirtualLine;
			Graphics = graphics;
			MarkRect = markRect;
		}
		#endregion
	}

	/// <summary>
	/// Arguments for ContextPromptUpdateEventHandler.
	/// </summary>
	public class ContextPromptUpdateEventArgs
		: EventArgs
	{
		#region Fields
		/// <summary>
		/// List of the context prompts. 
		/// List can contain any objects with overidden ToString() method.
		/// </summary>
		private ContextPromptCollection m_list;
		/// <summary>
		/// Value that indicates whether form should be closed.
		/// </summary>
		private bool m_bCloseForm;
		/// <summary>
		/// Lexem causing context prompt to drop.
		/// </summary>
		private IRenderedLexem m_droppingLexem;
		/// <summary>
		/// Lexem situated before dropper (doesn't include whitespace and new lines marks).
		/// </summary>
		private IRenderedLexem m_lexemBeforeDropper;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets value that indicates whether form should be closed.
		/// </summary>
		public bool CloseForm
		{
			get
			{
				return m_bCloseForm;
			}
			set
			{
				m_bCloseForm = value;
			}
		}
		/// <summary>
		/// Gets collection of DictionaryItems.
		/// </summary>
		public ContextPromptCollection List
		{
			get
			{
				return m_list;
			}
		}
		/// <summary>
		/// Gets dropping lexem.
		/// </summary>
		public IRenderedLexem Dropper
		{
			get
			{
				return m_droppingLexem;
			}
		}
		/// <summary>
		/// Gets lexem situated before dropper (doesn't include whitespace and new lines marks).
		/// </summary>
		public IRenderedLexem LexemBeforeDropper
		{
			get
			{
				return m_lexemBeforeDropper;
			}
		}
		#endregion

		#region Initialization & Finalization
		/// <summary>
		/// Creates a new instance of ContextPromptUpdateEventArgs class.
		/// </summary>
		public ContextPromptUpdateEventArgs( ContextPromptCollection list, IRenderedLexem droppingLexem, IRenderedLexem lexemBeforeDropper )
		{
			if( list == null ) throw new ArgumentNullException( "list" );

			m_list = list;
			m_droppingLexem = droppingLexem;
			m_lexemBeforeDropper = lexemBeforeDropper;
		}
		#endregion

		#region Public Methods
        /// <summary>
        /// Adds a new prompt to the list of the prompts.
        /// </summary>
        /// <param name="textInBold">Text that will be shown in bold on the header line.</param>
        /// <param name="textDescription">Description text.</param>
        /// <param name="image">Associated image.</param>
        /// <returns></returns>
		public ContextPromptItem AddPrompt( string textInBold, string textDescription, Image image )
		{
			return List.Add( textInBold, textDescription, image );
		}

        /// <summary>
        /// Adds a new prompt to the list of the prompts.
        /// </summary>
        /// <param name="textInBold">Text that will be shown in bold on the header line.</param>
        /// <param name="textDescription">Description text.</param>
        /// <returns></returns>
		public ContextPromptItem AddPrompt( string textInBold, string textDescription )
		{
			return List.Add( textInBold, textDescription );
		}
		#endregion
	}

	/// <summary>
	/// Arguments for ContextPromptCloseEventHandler.
	/// </summary>
	public class ContextPromptCloseEventArgs
		: ContextPromptUpdateEventArgs
	{
		#region Public Fields
		/// <summary>
		/// Shows if opening of the context prompt is allowed.
		/// </summary>
		public readonly bool Canceled;
		#endregion

		#region Initialization & Finalization
		/// <summary>
		/// Creates and initializes new instance of the ContextPromptCloseEventArgs class.
		/// </summary>
		/// <param name="list">List of the context prompts.</param>
		/// <param name="canceled">True if selection was canceled by user.</param>
		/// <param name="dropper">Lexem causing context prompt to drop.</param>
		/// <param name="lexemBeforeDropper">Lexem situated before dropper (doesn't include whitespace and new lines marks).</param>
		public ContextPromptCloseEventArgs( ContextPromptCollection list, bool canceled, IRenderedLexem dropper, IRenderedLexem lexemBeforeDropper )
			: base( list, dropper, lexemBeforeDropper )
		{
			Canceled = canceled;
		}
		#endregion
	}

	/// <summary>
	/// Arguments for TextChangingEventHandler.
	/// </summary>
	public class TextChangingEventArgs
		: TextChangedEventArgs
	{
		#region Fields
		/// <summary>
		/// Specifies whether text change has been canceled.
		/// </summary>
		private bool m_bCancel;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets whether text change has been canceled.
		/// </summary>
		public bool Cancel
		{
			get
			{
				return m_bCancel;
			}
			set
			{
				m_bCancel = value;
			}
		}
		#endregion

		#region Initialization & Finalization
		/// <summary>
		/// Creates and initializes new instance of TextChangingEventArgs class.
		/// </summary>
		/// <param name="text">Event's text.</param>
		/// <param name="iStartLine">Virtual line of Insert/Delete start.</param>
		/// <param name="iStartColumn">Virtual column of Insert/Delete start.</param>
		/// <param name="type">Type of the event ( Insert/Delete ).</param>
		public TextChangingEventArgs( string text, int iStartLine, int iStartColumn, TextChange type )
			: base( text, iStartLine, iStartColumn, type )
		{
		}
		#endregion
	}
    /// <summary>
    /// 
    /// </summary>
    public class LinesEventArgs : SyncfusionEventArgs
    {
        #region Fields
        /// <summary>
        /// Event's text.
        /// </summary>
        private string[] m_Text;
        /// <summary>
        /// Event's text.
        /// </summary>
        private string text;
        /// <summary>
        /// Virtual line of Insert/Delete start.
        /// </summary>
        private int m_StartLine;
        /// <summary>
        /// Virtual column of Insert/Delete start.
        /// </summary>
        private int m_StartColumn;
        /// <summary>
        /// Virtual Number oF Lines count
        /// </summary>
        private int m_LinesCount;
        /// <summary>
        /// Type of the event ( Insert/Delete ).
        /// </summary>
        private TextChange m_Type;
        #endregion
        #region Properties
        /// <summary>
        /// Gets or sets event's text.
        /// </summary>
        public string[] TextOfArray
        {
            get
            {
                return m_Text;
            }
            set
            {
                m_Text = value;
            }
        }
        /// <summary>
        /// Gets or sets event's text.
        /// </summary>
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
            }
        }
        /// <summary>
        /// Gets or sets virtual line of Insert/Delete start.
        /// </summary>
        public int StartLine
        {
            get
            {
                return m_StartLine;
            }
            set
            {
                m_StartLine = value;
            }
        }
        /// <summary>
        /// Virtual Number oF Lines count
        /// </summary>
        public int LinesCount
        {
            get
            {
                return m_LinesCount;
            }
            set
            {
                m_LinesCount = value;
            }
        }
        /// <summary>
        /// Gets or sets virtual column of Insert/Delete start.
        /// </summary>
        public int StartColumn
        {
            get
            {
                return m_StartColumn;
            }
            set
            {
                m_StartColumn = value;
            }
        }
        /// <summary>
        /// Gets or sets type of the event ( Insert/Delete ).
        /// </summary>
        public TextChange Type
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
            }
        }
        #endregion
        #region Initialization & Finalization
        /// <summary>
        /// Creates and initializes new instance of LinesEventArgs class.
        /// </summary>
        /// <param name="textOfArray">Event`s text.</param>
        /// <param name="iStartLine">Virtual line of Insert/Delete start.</param>
        /// <param name="iStartColumn">Virtual column of Insert/Delete start.</param>
        /// <param name="type">Type of the event ( Insert/Delete ).</param>
        /// <param name="iLinesCount">Type of the event ( Insert/Delete ).</param>
        public LinesEventArgs(string[] textOfArray, int iStartLine, int iStartColumn, int iLinesCount, TextChange type)
        {
            foreach (string s in textOfArray)
                text += s;
            m_Text = textOfArray;
            m_StartLine = iStartLine;
            m_StartColumn = iStartColumn;
            m_Type = type;
            m_LinesCount = iLinesCount;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// Gets string representation of the object.
        /// </summary>
        /// <returns>String representation of the object.</returns>
        public override string ToString()
        {
            return string.Format("{{Action:{3}; StartLine:{1}; ChangesStartsFromColumn:{2}; ChangedLinesCount:{4}; Text: \"{0}\"}}", Text, StartLine, StartColumn, m_Type, m_LinesCount);
        }
        #endregion
    }
	/// <summary>
	/// Event arguments for ContextChoiceItemSelectedEventHandler.
	/// </summary>
	public class ContextChoiceItemSelectedEventArgs
		: EventArgs
	{
		#region Fields
		/// <summary>
		/// Selected item itself.
		/// </summary>
		private IContextChoiceItem m_item;
		#endregion

		#region Properties
		/// <summary>
		/// Get selected item.
		/// </summary>
		public IContextChoiceItem SelectedItem
		{
			get
			{
				return m_item;
			}
		}
		#endregion

		#region Initialization & Finalization
		/// <summary>
		/// Creates and initializes new instance of ContextChoiceItemSelectedEventArgs with selected item.
		/// </summary>
		/// <param name="item">Selected item itself.</param>
		public ContextChoiceItemSelectedEventArgs( IContextChoiceItem item )
		{
			m_item = item;
		}
		#endregion
	}

	/// <summary>
	/// EventArgs for ContextChoiceItemEventHandler.
	/// </summary>
	public class ContextChoiceItemEventArgs
	{
		#region Public Fields
		/// <summary>
		/// Underlying ContextChoiceItem.
		/// </summary>
		public IContextChoiceItem Item;
		#endregion

		#region Initialization
		/// <summary>
		/// Creates and initializes new instance of ContextChoiceItemEventArgs.
		/// </summary>
		/// <param name="item">Underlying ContextChoiceItem.</param>
		public ContextChoiceItemEventArgs( IContextChoiceItem item )
		{
			this.Item = item;
		}
		#endregion
	}
	/// <summary>
	/// Event arguments for ContextPromptSelectionChangedEventHandler.
	/// </summary>
	public class ContextPromptSelectionChangedEventArgs
		: EventArgs
	{
		#region Fields
		/// <summary>
		/// List of the context prompts. List can contain any objects with overidden ToString() method.
		/// </summary>
		private ContextPromptCollection m_list;
		#endregion

		#region Properties
		/// <summary>
		/// Gets collection of DictionaryItems.
		/// </summary>
		public ContextPromptCollection List
		{
			get
			{
				return m_list;
			}
		}
		/// <summary>
		/// Get index of the currently selected item in prompts list.
		/// </summary>
		public int SelectedIndex
		{
			get
			{
				int result = -1;

				ContextPromptItem item = m_list.SelectedItem;
				if( item != null )
				{
					result = m_list.IndexOf( item );
				}

				return result;
			}
		}
		#endregion

		#region Initialization & Finalization
		/// <summary>
		/// Creates a new instance of ContextPromptUpdateEventArgs class.
		/// </summary>
		public ContextPromptSelectionChangedEventArgs( ContextPromptCollection list )
		{
			m_list = list;
		}
		#endregion
	}

	/// <summary>
	/// Event arguments for ContextChoiceTextInsertEventArgsEventHandler.
	/// </summary>
	public class ContextChoiceTextInsertEventArgs
		: CancelEventArgs
	{
		#region Fields
		/// <summary>
		/// Text that is displayed in context choice list.
		/// </summary>
		private string m_textDisplay;
		/// <summary>
		/// Text that will be inserted to the text.
		/// </summary>
		private string m_textInsert;
		/// <summary>
		/// Selected item index.
		/// </summary>
		private IContextChoiceItem m_item;
		#endregion

		#region Properties
		/// <summary>
		/// Gets text that is displayed in context choice list.
		/// </summary>
		public string DisplayText
		{
			get
			{
				return m_textDisplay;
			}
		}
		/// <summary>
		/// Gets or sets text that will be inserted to the text.
		/// </summary>
		public string InsertText
		{
			get
			{
				return m_textInsert;
			}
			set
			{
				m_textInsert = value;
			}
		}
		/// <summary>
		/// Gets or sets selected item index.
		/// </summary>
		public IContextChoiceItem SelectedItem
		{
			get
			{
				return m_item;
			}
		}
		#endregion

		#region Class Initialization
		/// <summary>
		/// Initializes event arguments with display text and item index.
		/// </summary>
		/// <param name="textDisplay">Text of the context choice item. It will be also set as text to be inserted.</param>
		/// <param name="item">Selected item.</param>
		public ContextChoiceTextInsertEventArgs( string textDisplay, IContextChoiceItem item )
		{
			m_textDisplay = m_textInsert = textDisplay;
			m_item = item;
			this.Cancel = false;
		}
		#endregion
	}

	/// <summary>
	/// Event arguments for headline printing events.
	/// </summary>
	public class PrintHeadlineEventArgs
		: EventArgs
	{
		#region Fields
		/// <summary>
		/// Graphics object used to print headline.
		/// </summary>
		private Graphics m_graphics;
		/// <summary>
		/// Rectangle, reserved for the headline.
		/// </summary>
		private Rectangle m_rect;
		/// <summary>
		/// Resulting height if the headline.
		/// </summary>
		private int m_height;
		/// <summary>
		/// Printed page number.
		/// </summary>
		private int m_iPage;
		/// <summary>
		/// Text to be printed out.
		/// </summary>
		private string m_strText;
		/// <summary>
		/// Specifies whether text printing has been handled.
		/// </summary>
		private bool m_bHandled;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets text that should be printed out with default headline printing method.
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
		/// Gets or sets boolean value that indicates whether text printing has already been handled.
		/// </summary>
		public bool Handled
		{
			get
			{
				return m_bHandled;
			}
			set
			{
				m_bHandled = value;
			}
		}
		/// <summary>
		/// Gets graphics object to be used to draw headline.
		/// </summary>
		public Graphics Graphics
		{
			get
			{
				return m_graphics;
			}
		}
		/// <summary>
		/// Gets rectangle, reserved for headline.
		/// </summary>
		public Rectangle Rectangle
		{
			get
			{
				return m_rect;
			}
		}
		/// <summary>
		/// Gets number of the printed page.
		/// </summary>
		public int PageNumber
		{
			get
			{
				return m_iPage;
			}
		}
		/// <summary>
		/// Gets or sets height of the headline.
		/// </summary>
		public int HeadlineHeight
		{
			get
			{
				return m_height;
			}
			set
			{
				m_height = value;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates and initializes headline drawing event args.
		/// </summary>
		/// <param name="g">Graphics object.</param>
		/// <param name="rect">Area, reserved for headline.</param>
		/// <param name="page">Printed page number.</param>
		/// <param name="text">Text to be printed out.</param>
		public PrintHeadlineEventArgs( Graphics g, Rectangle rect, int page, string text )
		{
			if( g == null ) throw new ArgumentNullException( "g" );
			if( rect.IsEmpty ) throw new ArgumentNullException( "rect" );

			m_graphics = g;
			m_rect = rect;
			m_iPage = page;
			m_height = rect.Height;
			m_strText = text;
		}
		#endregion
	}

	/// <summary>
	/// Event arguments for IndicatorClickEventHandler.
	/// </summary>
	public class IndicatorClickEventArgs
		: EventArgs
	{
		#region Fields
		/// <summary>
		/// Number of the line the click has occured on.
		/// </summary>
		private int m_iLine;
		/// <summary>
		/// Currently existing custom bookmark on the line.
		/// </summary>
		private ICustomBookmark m_currentBookmark;
		#endregion

		#region Properties
		/// <summary>
		/// Gets clicked line index.
		/// </summary>
		public int LineIndex
		{
			get
			{
				return m_iLine;
			}
		}
		/// <summary>
		/// Gets clicked custom bookmark if available.
		/// </summary>
		public ICustomBookmark Bookmark
		{
			get
			{
				return m_currentBookmark;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates and inializes new instance of the class.
		/// </summary>
		/// <param name="lineIndex">Line index.</param>
		/// <param name="bookmark">Current custom bookmark on a line.</param>
		public IndicatorClickEventArgs( int lineIndex, ICustomBookmark bookmark )
		{
			m_iLine = lineIndex;
			m_currentBookmark = bookmark;
		}
		#endregion
	}

	/// <summary>
	/// Event arguments for DrawUserMarginTextEventHandler.
	/// </summary>
	public class DrawUserMarginTextEventArgs
		: EventArgs
	{
		#region Fields
		/// <summary>
		/// Graphics object to draw text on.
		/// </summary>
		private Graphics m_graphics;
		/// <summary>
		/// Rectangle of allowed text area.
		/// </summary>
		private Rectangle m_rect;
		/// <summary>
		/// Corresponding editor line.
		/// </summary>
		private ILexemLine m_line;
		/// <summary>
		/// Text color.
		/// </summary>
		private Color m_clr;
		/// <summary>
		/// Text font.
		/// </summary>
		private Font m_font;
		/// <summary>
		/// Text itself.
		/// </summary>
		private string m_strText;
		/// <summary>
		/// Indicates whether user draws text itself.
		/// </summary>
		private bool m_bCustomDraw;
		#endregion.

		#region Properties
		/// <summary>
		/// Gets graphics object to draw text on.
		/// </summary>
		public Graphics Graphics
		{
			get
			{
				return m_graphics;
			}
		}
		/// <summary>
		/// Gets rectangle of allowed text area.
		/// </summary>
		public Rectangle Rect
		{
			get
			{
				return m_rect;
			}
		}
		/// <summary>
		/// Gets corresponding editor line.
		/// </summary>
		public ILexemLine Line
		{
			get
			{
				return m_line;
			}
		}
		/// <summary>
		/// Gets or sets text color.
		/// </summary>
		public Color Color
		{
			get
			{
				return m_clr;
			}
			set
			{
				m_clr = value;
			}
		}
		/// <summary>
		/// Gets or sets text font.
		/// </summary>
		public Font Font
		{
			get
			{
				return m_font;
			}
			set
			{
				m_font = value;
			}
		}
		/// <summary>
		/// Gets or sets text itself.
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
		/// Gets or sets bool that indicates whether user draws text itself.
		/// </summary>
		public bool CustomDraw
		{
			get
			{
				return m_bCustomDraw;
			}
			set
			{
				m_bCustomDraw = value;
			}
		}
		#endregion

		#region Intialization
		/// <summary>
		/// Creates and initializes new instance of the class.
		/// </summary>
		/// <param name="g">Graphics object to draw text on.</param>
		/// <param name="rect">Rectangle of allowed text area.</param>
		/// <param name="line">Corresponding editor line.</param>
		/// <param name="font">Font of text.</param>
		/// <param name="color">Color of text.</param>
		public DrawUserMarginTextEventArgs( Graphics g, Rectangle rect, ILexemLine line, Font font, Color color )
		{
			if( null == g ) throw new ArgumentNullException( "g" );
			if( rect.IsEmpty ) throw new ArgumentOutOfRangeException( "rect" );
			if( null == line ) throw new ArgumentNullException( "line" );
			if( null == font ) throw new ArgumentNullException( "font" );
			if( color.IsEmpty ) throw new ArgumentOutOfRangeException( "color" );

			m_graphics = g;
			m_rect = rect;
			m_line = line;
			m_font = font;
			m_clr = color;
		}
		#endregion
	}

	/// <summary>
	/// Event arguments for SaveWithDataLosingEventHandler.
	/// </summary>
	public class SaveWithDataLosingEventArgs
		: EventArgs
	{
		#region Fields
		/// <summary>
		/// Indicates whether user handled the event.
		/// </summary>
		private bool m_bUserHandling = false;
		/// <summary>
		/// If user handled the event, indicates whether data have to be saved with loosing
		/// </summary>
		private bool m_bSaveWithLoss = false;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets bool that indicates whether user handled the event.
		/// </summary>
		public bool UserHandling
		{
			get
			{
				return m_bUserHandling;
			}
			set
			{
				m_bUserHandling = value;
			}
		}
		/// <summary>
		/// Gets or sets bool that indicates whether data have to be saved with loosing (if user handled the event).
		/// </summary>
		public bool SaveWithLoss
		{
			get
			{
				return m_bSaveWithLoss;
			}
			set
			{
				m_bSaveWithLoss = value;
			}
		}
		#endregion
	}

	/// <summary>
	/// Parent class for event args related to collapsing.
	/// </summary>
	public class CollapseEventArgs
		: EventArgs
	{
		#region Fields
		/// <summary>
		/// Collapse name.
		/// </summary>
		private string m_strCollapseName;
		/// <summary>
		/// Collapser.
		/// </summary>
		private CollapsableRegion m_collapser;
		/// <summary>
		/// Collapsed text.
		/// </summary>
		private string m_strCollapsedText;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets collapse name.
		/// </summary>
		public string CollapseName
		{
			get
			{
				return m_strCollapseName;
			}
			set
			{
				m_strCollapseName = value;
			}
		}
		/// <summary>
		/// Gets or sets collapser.
		/// </summary>
		public CollapsableRegion Collapser
		{
			get
			{
				return m_collapser;
			}
			set
			{
				m_collapser = value;
			}
		}
		/// <summary>
		/// Gets or sets collapsed text.
		/// </summary>
		public string CollapsedText
		{
			get
			{
				return m_strCollapsedText;
			}
			set
			{
				m_strCollapsedText = value;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates and initializes new instance of the class.
		/// </summary>
		/// <param name="strCollapseName">Collapse name.</param>
		/// <param name="collapser">Collapser.</param>
		/// <param name="strCollapsedText">Collapsed text.</param>
		public CollapseEventArgs( string strCollapseName, CollapsableRegion collapser, string strCollapsedText )
		{
			if( null == collapser ) throw new ArgumentNullException( "collapser" );

			m_strCollapseName = strCollapseName;
			m_collapser = collapser;
			m_strCollapsedText = strCollapsedText;
		}
		#endregion
	}

	/// <summary>
	/// Event arguments for events related to collapsing.
	/// </summary>
	public class OutliningEventArgs
		: CollapseEventArgs
	{
		#region Fields
		/// <summary>
		/// Specifies whether user calcels the underlying event.
		/// </summary>
		private bool m_bCancel = false;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets bool indicating whether user cancels the underlying event.
		/// </summary>
		public bool Cancel
		{
			get
			{
				return m_bCancel;
			}
			set
			{
				m_bCancel = value;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates and initializes new instance of the class.
		/// </summary>
		/// <param name="strCollapseName">Collapse name.</param>
		/// <param name="collapser">Collapser.</param>
		/// <param name="strCollapsedText">Collapsed text.</param>
		public OutliningEventArgs( string strCollapseName, CollapsableRegion collapser, string strCollapsedText )
			: base( strCollapseName, collapser, strCollapsedText )
		{
		}
		#endregion
	}

	/// <summary>
	/// Event arguments for OutliningTooltipBeforePopupEventHandler.
	/// </summary>
	public class OutliningTooltipBeforePopupEventArgs
		: CollapseEventArgs
	{
		#region Fields
		/// <summary>
		/// Mode of tooltip showing.
		/// </summary>
		private OutliningTooltipShowMode m_mode = OutliningTooltipShowMode.On;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets bool indicating whether user cancels the underlying event.
		/// </summary>
		public OutliningTooltipShowMode ShowMode
		{
			get
			{
				return m_mode;
			}
			set
			{
				m_mode = value;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates and initializes new instance of the class.
		/// </summary>
		/// <param name="strCollapseName">Collapse name.</param>
		/// <param name="collapser">Collapser.</param>
		/// <param name="strCollapsedText">Collapsed text.</param>
		public OutliningTooltipBeforePopupEventArgs( string strCollapseName, CollapsableRegion collapser, string strCollapsedText )
			: base( strCollapseName, collapser, strCollapsedText )
		{
		}
		#endregion
	}

	/// <summary>
	/// Event arguments for UpdateBookmarkTooltipEventHandler.
	/// </summary>
	public class UpdateBookmarkTooltipEventArgs
		: UpdateTooltipEventArgs
	{
		#region Public Fields
		/// <summary>
		/// Index of bookmarked line.
		/// </summary>
		public int Line;
		/// <summary>
		/// Bookmark.
		/// </summary>
		public IBookmark Bookmark;
		#endregion

		#region Initialization
		/// <summary>
		/// Creates and initializes new instance of BookmarkTooltipBeforePopupEventArgs.
		/// </summary>
		/// <param name="iLine">Index of bookmarked line.</param>
		/// <param name="bookmark">Bookmark.</param>
		/// <param name="tooltipInfo">UpdateTooltipEventArgs.</param>
		public UpdateBookmarkTooltipEventArgs( int iLine, IBookmark bookmark, UpdateTooltipEventArgs tooltipInfo )
			: base( tooltipInfo.Text, tooltipInfo.HintedArea )
		{
			this.Line = iLine;
			this.Bookmark = bookmark;
		}
		#endregion
	}

	/// <summary>
	/// Event arguments for GetBoolEventHandler.
	/// </summary>
	public class GetBoolEventArgs
		: EventArgs
	{
		#region Fields
		/// <summary>
		/// Resulting boolean value.
		/// </summary>
		private bool m_bValue = false;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets resulting boolean value.
		/// </summary>
		public bool Value
		{
			get
			{
				return m_bValue;
			}
			set
			{
				m_bValue = value;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates and initializes new instance of GetBoolEventArgs.
		/// </summary>
		/// <param name="defaultValue">Default initial value.</param>
		public GetBoolEventArgs( bool defaultValue )
		{
			m_bValue = defaultValue;
		}
		#endregion
	}

	/// <summary>
	/// Event arguments for ChangeValueEventHandler.
	/// </summary>
	public class ChangeValueEventArgs
		: EventArgs
	{
		#region Fields
		/// <summary>
		/// Value itself.
		/// </summary>
		private object m_value;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets value.
		/// </summary>
		public object Value
		{
			get
			{
				return m_value;
			}
			set
			{
				m_value = value;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates new instance of ChangeValueEventArgs.
		/// </summary>
		/// <param name="value">Value itself.</param>
		public ChangeValueEventArgs( object value )
		{
			if( null == value ) throw new ArgumentNullException( "value" );

			m_value = value;
		}
		#endregion
	}

	/// <summary>
	/// Helper class that keeps data about some hint when UpdateTooltip event is raised.
	/// </summary>
	public class UpdateTooltipEventArgs
		: EventArgs
	{
		#region Public Fields
		/// <summary>
		/// Text of the tooltip.
		/// </summary>
		public string Text;
		/// <summary>
		/// Rectangle, that represents an object which has this tooltip.
		/// </summary>
		public Rectangle HintedArea;
		/// <summary>
		/// Mouse X coordinate in client coordinates.
		/// </summary>
		public int X;
		/// <summary>
		/// Mouse Y coordinate in client coordinates.
		/// </summary>
		public int Y;
		#endregion

		#region Fields
		/// <summary>
		/// Image associated with tooltip.
		/// </summary>
		private Image m_image;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets image associated with tooltip.
		/// </summary>
		public Image Image
		{
			get
			{
				return m_image;
			}
			set
			{
				m_image = value;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates and initializes UpdateTooltipEventArgs.
		/// </summary>
		public UpdateTooltipEventArgs()
			: this( string.Empty, Rectangle.Empty )
		{
		}
		/// <summary>
		/// Creates and initializes UpdateTooltipEventArgs.
		/// </summary>
		/// <param name="text">Text of the tooltip.</param>
		public UpdateTooltipEventArgs( string text )
			: this( text, Rectangle.Empty )
		{
		}
		/// <summary>
		/// Creates and initializes UpdateTooltipEventArgs.
		/// </summary>
		/// <param name="text">Text of the tooltip.</param>
		/// <param name="rect">Hinted rectangle.</param>
		public UpdateTooltipEventArgs( string text, Rectangle rect )
		{
			Text = text;
			HintedArea = rect;
		}
		#endregion
	}

	/// <summary>
	/// Keeps data about text and it's location when unreachable text is found during find operation.
	/// </summary>
	public class UnreachableTextFoundEventArgs
		: EventArgs
	{
		#region Public Fields
		/// <summary>
		/// Searched text.
		/// </summary>
		public string Text;
		/// <summary>
		/// Point of the location of unreachable text.
		/// </summary>
		public IParsePoint Point;
		/// <summary>
		/// Indicates whether search must be continued.
		/// </summary>
		public bool ContinueSearch = true;
		#endregion

		#region Initialization
		/// <summary>
		/// Creates and initializes new instance of UnreachableTextFoundEventArgs.
		/// </summary>
		/// <param name="text">Searched text.</param>
		/// <param name="point">Point of the location of unreachable text.</param>
		public UnreachableTextFoundEventArgs( string text, IParsePoint point )
		{
			if( text == null ) throw new ArgumentNullException( "text" );
			if( point == null ) throw new ArgumentNullException( "point" );

			this.Text = text;
			this.Point = point;
		}
		#endregion
	}

	/// <summary>
	/// Event arguments for events related to code snippets.
	/// </summary>
	public class CodeSnippetsEventArgs
		: EventArgs
	{
		#region Public Fields
		/// <summary>
		/// Code snippet that is currently activated.
		/// </summary>
		public CodeSnippet CodeSnippet;
		#endregion

		#region Initialization
		/// <summary>
		/// Creates new instance of CodeSnippetsEventArgs.
		/// </summary>
		/// <param name="codeSnippet">Currently activated code snippet.</param>
		public CodeSnippetsEventArgs( CodeSnippet codeSnippet )
		{
			this.CodeSnippet = codeSnippet;
		}
		#endregion
	}

	/// <summary>
	/// Event arguments for events related to code snippets with actions that can be cancelled.
	/// </summary>
	public class CancellableCodeSnippetsEventArgs
		: CodeSnippetsEventArgs
	{
		#region Public Fields
		/// <summary>
		/// Indicates whether action has to be cancelled.
		/// </summary>
		public bool Cancel = false;
		#endregion

		#region Initialization
		/// <summary>
		/// Creates new instance of CancellableCodeSnippetsEventArgs.
		/// </summary>
		/// <param name="codeSnippet">Currently activated code snippet.</param>
		public CancellableCodeSnippetsEventArgs( CodeSnippet codeSnippet )
			: base( codeSnippet )
		{
		}
		#endregion
	}

	/// <summary>
	/// Event arguments for NewSnippetMemberHighlightingEventHandler.
	/// </summary>
	public class NewSnippetMemberHighlightingEventArgs
		: CancellableCodeSnippetsEventArgs
	{
		#region Public Fields
		/// <summary>
		/// Previously highlighted snippet member.
		/// </summary>
		public CodeSnippetsManager.SnippetMember OldSnippetMember;
		/// <summary>
		/// Snippet member that has to be highlighted.
		/// </summary>
		public CodeSnippetsManager.SnippetMember NewSnippetMember;
		#endregion

		#region Initialization
		/// <summary>
		/// Creates new instance of NewSnippetMemberHighlightedEventArgs.
		/// </summary>
		/// <param name="oldSnippetMember">Previously highlighted snippet member.</param>
		/// <param name="newSnippetMember">Snippet member that has to be highlighted.</param>
		/// <param name="codeSnippet">Currently activated code snippet.</param>
		public NewSnippetMemberHighlightingEventArgs(
			CodeSnippet codeSnippet, CodeSnippetsManager.SnippetMember oldSnippetMember, CodeSnippetsManager.SnippetMember newSnippetMember )
			: base( codeSnippet )
		{
			this.OldSnippetMember = oldSnippetMember;
			this.NewSnippetMember = newSnippetMember;
		}
		#endregion
	}

	/// <summary>
	/// Event arguments for CodeSnippetTemplateTextChangingEventHandler.
	/// </summary>
	public class CodeSnippetTemplateTextChangingEventArgs
		: CancellableCodeSnippetsEventArgs
	{
		#region Public Fields
		/// <summary>
		/// Name of template member that is to be changed.
		/// </summary>
		public string TemlateMemberName;
		/// <summary>
		/// New text.
		/// </summary>
		public string NewText;
		#endregion

		#region Initialization
		/// <summary>
		/// Creates new instance of CodeSnippetTemplateTextChangingEventArgs.
		/// </summary>
		/// <param name="codeSnippet">Currently activated code snippet.</param>
		/// <param name="templateMemberName">Name of template member that is to be changed.</param>
		/// <param name="newText">New text.</param>
		public CodeSnippetTemplateTextChangingEventArgs( CodeSnippet codeSnippet, string templateMemberName, string newText )
			: base( codeSnippet )
		{
			this.TemlateMemberName = templateMemberName;
			this.NewText = newText;
		}
		#endregion
	}

	/// <summary>
	/// Event arguments for ParsePointParameterChangedEventHandler.
	/// </summary>
	public class ParsePointParameterChangedEventArgs
		: EventArgs
	{
		#region Public Fields
		/// <summary>
		/// Old value of point offset.
		/// </summary>
		public long OldOffset;
		/// <summary>
		/// New value of point offset.
		/// </summary>
		public long NewOffset;
		/// <summary>
		/// Old value of point position.
		/// </summary>
		public int OldPosition;
		/// <summary>
		/// New value of point position.
		/// </summary>
		public int NewPosition;
		/// <summary>
		/// Old value of point line.
		/// </summary>
		public int OldLine;
		/// <summary>
		/// New value of point line.
		/// </summary>
		public int NewLine;
		#endregion

		#region Properties
		/// <summary>
		/// Gets bool indicating whether offset has been changed.
		/// </summary>
		public bool OffsetChanged
		{
			get
			{
				return ( OldOffset != NewOffset );
			}
		}
		/// <summary>
		/// Gets bool indicating whether position has been changed.
		/// </summary>
		public bool PositionChanged
		{
			get
			{
				return ( OldPosition != NewPosition );
			}
		}
		/// <summary>
		/// Gets bool indicating whether line has been changed.
		/// </summary>
		public bool LineChanged
		{
			get
			{
				return ( OldLine != NewLine );
			}
		}
		#endregion

		#region Intialization
		/// <summary>
		/// Creates and initializes new instance of ParsePointParameterChangedEventArgs.
		/// </summary>
		/// <param name="oldOffset">Old point offset.</param>
		/// <param name="newOffset">New point offset.</param>
		/// <param name="oldPos">Old point position.</param>
		/// <param name="newPos">New point position.</param>
		/// <param name="oldLine">Old point line.</param>
		/// <param name="newLine">New point line.</param>
		public ParsePointParameterChangedEventArgs( long oldOffset, long newOffset, int oldPos, int newPos, int oldLine, int newLine )
		{
			this.OldOffset = oldOffset;
			this.NewOffset = newOffset;
			this.OldPosition = oldPos;
			this.NewPosition = newPos;
			this.OldLine = oldLine;
			this.NewLine = newLine;
		}
		#endregion
	}

	/// <summary>
	/// This class sends messages to the event handlers. It contains all information needed for rendering and controlling the drawing process.
	/// </summary>
	public class BookmarkPaintEventArgs
		: PaintEventArgs
	{
		#region Fields
		/// <summary>
		/// Number of line.
		/// </summary>
		private int lineNumber;
		#endregion

		#region Properties
		/// <summary>
		/// Gets number of line.
		/// </summary>
		public int LineNumber
		{
			get
			{
				return lineNumber;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates and initializes new instance of BookmarkPaintEventArgs.
		/// </summary>
		/// <param name="args">Event args to get info for new object from.</param>
		/// <param name="lineNumber">Number of line.</param>
		public BookmarkPaintEventArgs( PaintEventArgs args, int lineNumber )
			: this( args.Graphics, args.ClipRectangle, lineNumber )
		{
		}
		/// <summary>
		/// Main constructor.
		/// </summary>
		/// <param name="g">Graphics object used for rendering</param>
		/// <param name="rc">Destination rectangle</param>
		/// <param name="lineNumber">Number of line.</param>
		public BookmarkPaintEventArgs( Graphics g, Rectangle rc, int lineNumber )
			: base( g, rc )
		{
			if( g == null ) throw new ArgumentNullException( "g" );
			if( rc == RectangleF.Empty )
				throw new ArgumentOutOfRangeException( "rc", rc, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_10 );

			this.lineNumber = lineNumber;
		}
		#endregion
	}

	/// <summary>
	/// Event arguments for CoordinatesChangeEventHandler.
	/// </summary>
	public class CoordinatesChangeEventArgs
		: EventArgs
	{
		#region Public Fields
		/// <summary>
		/// New point.
		/// </summary>
		public Point NewPoint;
		#endregion

		#region Initialization
		/// <summary>
		/// Creates and initializes new instance of CoordinatesChangeEventArgs.
		/// </summary>
		/// <param name="newPoint">New point.</param>
		public CoordinatesChangeEventArgs( Point newPoint )
		{
			if( newPoint.IsEmpty ) throw new ArgumentOutOfRangeException( "newPoint" );

			this.NewPoint = newPoint;
		}
		#endregion
	}
	#endregion

	#region Delegates
	/// <summary>
	/// Delegate for StreamClosing event of the editcontrol.
	/// </summary>
	public delegate void StreamCloseEventHandler( object sender, StreamCloseEventArgs e );
	/// <summary>
	/// Delegate for events, connected with long operations.
	/// </summary>
	public delegate void LongOperationEventHandler( ILongOperation operation );
	/// <summary>
	/// Delegate for OnCustomDraw event.
	/// </summary>
	public delegate void CustomSnippetDrawEventHandler( object sender, CustomSnippetDrawEventArgs e );
	/// <summary>
	///	Delegate for ValueChanged event.
	/// </summary>
	public delegate void ValueChangedEventHandler( object sender, ValueChangedEventArgs e );
	/// <summary>
	/// Delegate for ParsePoint Deleted even.
	/// </summary>
	public delegate void ParsePointDeletedEventHandler( ParsePoint point, long lNewOffset );
	/// <summary>
	/// Delegate for CoordinatePoint Deleted even.
	/// </summary>
	public delegate void CoordinatePointDeletedEventHandler( CoordinatePoint point, long lNewOffset );
	/// <summary>
	/// Delegate for events, raised on text changes in <see cref="LexemParser"/>.
	/// </summary>
	public delegate void TextChangedEventHandler( object sender, TextChangedEventArgs e );
	/// <summary>
	/// Delegate for events, raised before text changes.
	/// </summary>
	public delegate void TextChangingEventHandler( object sender, TextChangingEventArgs e );
    /// <summary>
    /// Delegate for events, raised on lines Inserted.
    /// </summary>
    public delegate void LineInsertedEventHandler(object sender, LinesEventArgs e);
    /// <summary>
    /// Delegate for events, raised on lines Deleted.
    /// </summary>
    public delegate void LineDeletedEventHandler(object sender, LinesEventArgs e);
	/// <summary>
	/// Delegate for ProcessKey events.
	/// </summary>
	public delegate void ProcessCommandEventHandler();
	/// <summary>
	/// Delegate for ProcessKey events.
	/// </summary>
	public delegate void ProcessCommandsEventHandler( Keys key );
	/// <summary>
	/// Delegate for event, related to the context choice.
	/// </summary>
	public delegate void ContextChoiceEventHandler( IContextChoiceController controller );
	/// <summary>
	/// Delegate for close event, related to the context choice.
	/// </summary>
	public delegate void ContextChoiceCloseEventHandler(
		IContextChoiceController controller, DialogResult dialogresult );
	/// <summary>
	/// Delegate for event raised when user selects some context choice item and it`s text should be inserted.
	/// </summary>
	public delegate void ContextChoiceTextInsertEventHandler( IContextChoiceController sender, ContextChoiceTextInsertEventArgs e );
	/// <summary>
	/// Delegate for events, raised when LineMark should be drawn.
	/// </summary>
	public delegate void DrawLineMarkEventHandler( object sender, DrawLineMarkEventArgs e );
	/// <summary>
	/// Delegate for events related with showing of the context prompt.
	/// </summary>
	public delegate void ContextPromptUpdateEventHandler( object sender, ContextPromptUpdateEventArgs e );
	/// <summary>
	/// Delegate for events related with closing of the context prompt.
	/// </summary>
	public delegate void ContextPromptCloseEventHandler( object sender, ContextPromptCloseEventArgs e );
	/// <summary>
	/// Delegate for ItemSelected event of the context choice list.
	/// </summary>
	public delegate void ContextChoiceItemSelectedEventHandler( IContextChoiceController sender, ContextChoiceItemSelectedEventArgs e );
	/// <summary>
	/// Delegate for events related to context choice items.
	/// </summary>
	public delegate void ContextChoiceItemEventHandler( IContextChoiceController sender, ContextChoiceItemEventArgs e );
	/// <summary>
	/// Delegate for SelectedPromptChanged event of the context prompt list.
	/// </summary>
	public delegate void ContextPromptSelectionChangedEventHandler( ContextPrompt sender, ContextPromptSelectionChangedEventArgs e );
	/// <summary>
	/// Delegate to the methods used for page header and footer printing.
	/// </summary>
	public delegate void PrintHeadlineEventHandler( object sender, PrintHeadlineEventArgs e );
	/// <summary>
	/// Delegate used for processing lexems and replacing their text.
	/// </summary>
	public delegate string LexemReplaceEventHandler( ILexem lexem );
	/// <summary>
	/// Delegate for event related to the margin area clicks processing.
	/// </summary>
	public delegate void IndicatorClickEventHandler( object sender, IndicatorClickEventArgs e );
	/// <summary>
	/// Delegate for events related with user margin text drawing.
	/// </summary>
	public delegate void DrawUserMarginTextEventHandler( object sender, DrawUserMarginTextEventArgs e );
	/// <summary>
	/// Delegate for events related with data loosing while saving.
	/// </summary>
	public delegate void SaveWithDataLosingEventHandler( object sender, SaveWithDataLosingEventArgs e );
	/// <summary>
	/// Delegate for events related with outlining tooltips.
	/// </summary>
	public delegate void OutliningEventHandler( object sender, CollapseEventArgs e );
	/// <summary>
	/// Delegate for cancelable events related with outlining tooltips.
	/// </summary>
	public delegate void OutliningCancellableEventHandler( object sender, OutliningEventArgs e );
	/// <summary>
	///	 Delegate for event related with collapsed regions.
	/// </summary>
	public delegate void CollapsedRegionRelatedEventHandler( object sender, CollapseEventArgs e );
	/// <summary>
	/// Delegate using for actions taking place before outlining popup is about to be shown.
	/// </summary>
	public delegate void OutliningTooltipBeforePopupEventHandler( object sender, OutliningTooltipBeforePopupEventArgs e );
	/// <summary>
	/// Delegate used for actions taking place when encoding was possibly changed.
	/// </summary>
	public delegate void EncodingChangedEventHandler( object sender, EventArgs e );
	/// <summary>
	/// Delegate used for retrieving some boolean value using event.
	/// </summary>
	public delegate void GetBoolEventHandler( object sender, GetBoolEventArgs e );
	/// <summary>
	/// Delegate used for changing some value.
	/// </summary>
	public delegate void ChangeValueEventHandler( object sender, ChangeValueEventArgs e );
	/// <summary>
	/// Handler for the UpdateTooltip event.
	/// </summary>
	public delegate void UpdateTooltipEventHandler( object sender, UpdateTooltipEventArgs e );
	/// <summary>
	/// Handler for the UnreachableTextFound event.
	/// </summary>
	public delegate void UnreachableTextFoundEventHandler( object sender, UnreachableTextFoundEventArgs e );
	/// <summary>
	/// Handler for OnBeforeLineNumbersPaint event.
	/// </summary>
	public delegate void OnBeforeLineNumberPaintEventHandler(object sender, LineNumberPaintEventArgs e);
	/// <summary>
	/// Handler for NewSnippetMemberHighlighting event.
	/// </summary>
	public delegate void NewSnippetMemberHighlightingEventHandler( object sender, NewSnippetMemberHighlightingEventArgs e );
	/// <summary>
	/// Handler for CodeSnippetTemplateTextChanging event.
	/// </summary>
	public delegate void CodeSnippetTemplateTextChangingEventHandler( object sender, CodeSnippetTemplateTextChangingEventArgs e );
	/// <summary>
	/// Handler for cancellable events related to code snippets.
	/// </summary>
	public delegate void CancellableCodeSnippetsEventHandler( object sender, CancellableCodeSnippetsEventArgs e );
	/// <summary>
	/// Handler for events related to code snippets.
	/// </summary>
	public delegate void CodeSnippetsEventHandler( object sender, CodeSnippetsEventArgs e );
	/// <summary>
	/// Handler for the UpdateBookmarkTooltip event.
	/// </summary>
	public delegate void UpdateBookmarkTooltipEventHandler( object sender, UpdateBookmarkTooltipEventArgs e );
	/// <summary>
	/// Handler for the ParsePointParameterChanged event.
	/// </summary>
	public delegate void ParsePointParameterChangedEventHandler( object sender, ParsePointParameterChangedEventArgs e );
	/// <summary>
	/// Handler for the ParsePointParameterChanged event.
	/// </summary>
	public delegate void BookmarkPaintEventHandler( object sender, BookmarkPaintEventArgs e );
	/// <summary>
	/// Handler for BeforeCoordinatesChange event.
	/// </summary>
	public delegate void CoordinatesChangeEventHandler( object sender, CoordinatesChangeEventArgs e );
	/// <summary>
	/// Handler for ChangingStream event.
	/// </summary>
	public delegate void ChangingStreamEventHandler( ref bool processed );
	/// <summary>
	/// Handler for InvalidateArea event.
	/// </summary>
	public delegate void InvalidateAreaEventHandler( StreamEditControl initiator, Rectangle areaToInvalidate );
	#endregion
}