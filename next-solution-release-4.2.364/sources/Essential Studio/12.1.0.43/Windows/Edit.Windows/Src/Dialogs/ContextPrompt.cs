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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

using Syncfusion.Windows.Forms.Edit.Implementation;
using Syncfusion.Windows.Forms.Edit.Utils;
using Syncfusion.Windows.Forms.Localization;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Edit.Forms.Popup
{
	/// <summary>
	/// Form used for context prompt functionality.
	/// </summary>
	public class ContextPrompt
		: Syncfusion.Windows.Forms.Edit.Forms.Popup.BasePopupForm
	{
		#region Classes
		/// <summary>
		/// Measured information about context prompt.
		/// </summary>
		protected class MeasuredInfo
		{
			/// <summary>
			/// Rectangle occupied by Up Arrow.
			/// </summary>
			public Rectangle ArrowUp;
			/// <summary>
			/// Rectangle occupied by Down Arrow.
			/// </summary>
			public Rectangle ArrowDown;
			/// <summary>
			/// Rectangle occupied by text between arrows.
			/// </summary>
			public Rectangle Index;
			/// <summary>
			/// Rectangle occupied by subject.
			/// </summary>
			public Rectangle Subject;
			/// <summary>
			/// Rectangle occupied by description.
			/// </summary>
			public Rectangle Description;
			/// <summary>
			/// Rectangle occupied by entire prompt.
			/// </summary>
			public Rectangle Entire;
		}
		#endregion

		#region Constants
		/// <summary>
		/// Height of the first line.
		/// </summary>
		private const int DEF_FIRST_LINE_HEIGHT = 24;
		/// <summary>
		/// Down arrow resource name.
		/// </summary>
		private const string DEF_FILENAME_ARROW_DOWN = "Syncfusion.Windows.Forms.Edit.Images.DownArrow.bmp";
		/// <summary>
		/// Up arrow resource name.
		/// </summary>
		private const string DEF_FILENAME_ARROW_UP = "Syncfusion.Windows.Forms.Edit.Images.UpArrow.bmp";
		/// <summary>
		/// Free space between arrow and text.
		/// </summary>
		private const int DEF_OFFSET_COMMON = 1;
		/// <summary>
		/// Free space between border and text.
		/// </summary>
		private const int DEF_OFFSET_BORDER = 4;
		/// <summary>
		/// Size of square clip for image.
		/// </summary>
		private const int DEF_IMAGE_SIZE = 32;
		#endregion

		#region Fields
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		/// <summary>
		/// Editor, prompt belongs to.
		/// </summary>
		private Control m_ctrlParent;
		/// <summary>
		/// 
		/// </summary>
		private Font m_boldFont;
		/// <summary>
		/// 
		/// </summary>
		private Graphics m_defGraphics = Graphics.FromImage( new Bitmap( 1, 1 ) );
		/// <summary>
		/// Collection of the context prompts.
		/// </summary>
		private ContextPromptCollection m_list;
		/// <summary>
		/// Up arrow image.
		/// </summary>
		private static Image m_imgArrowUp;
		/// <summary>
		/// Down arrow image.
		/// </summary>
		private static Image m_imgArrowDown;
		/// <summary>
		/// Measured info.
		/// </summary>
		private ContextPrompt.MeasuredInfo m_info;
		/// <summary>
		/// String template used for displaying index of the selected prompt.
		/// </summary>
		private string m_sIndexTemplate = "{0} of {1}";
		/// <summary>
		/// Indicates whether XP style should be used.
		/// </summary>
		private bool m_bUseXPStyle = true;
		/// <summary>
		/// Indicates whether custom context prompt size should be used.
		/// </summary>
		private bool m_bUseCustomSize = false;
		/// <summary>
		/// Lexem causing context prompt to drop.
		/// </summary>
		private IRenderedLexem m_droppingLexem;
		/// <summary>
		/// Lexem situated before dropper.
		/// </summary>
		private IRenderedLexem m_lexemBeforeDropper;
		#endregion

		#region Initialize/Finalize Methods
		/// <summary>
		/// Creates and initializes new instance of then Context Prompt.
		/// </summary>
		/// <param name="parent">Editor, prompt belongs to.</param>
		public ContextPrompt( Control parent )
			: base( parent, true, 0.2f )
		{
			if( parent == null ) throw new ArgumentNullException( "parent" );

			m_sIndexTemplate = Localizer.DEF_PROMPT_INDEX;

			ForeColor = SystemColors.InfoText;

			InitializeComponent();

			m_list = new ContextPromptCollection();
			m_list.OnChanged += new EventHandler( m_list_OnChanged );
			m_list.SelectionChanged += new EventHandler( m_list_OnChanged );
			m_list.BoldedItemSelectionChanged += new EventHandler( m_list_OnChanged );

			SetStyle( ControlStyles.Selectable, false );
			SetStyle( ControlStyles.StandardClick, false );
			SetStyle( ControlStyles.ContainerControl, false );
			SetStyle( ControlStyles.UserPaint, true );
			SetStyle( ControlStyles.DoubleBuffer, true );
			SetStyle( ControlStyles.AllPaintingInWmPaint, true );
			m_boldFont = new Font( Font, FontStyle.Bold );

			this.m_ctrlParent = parent;
			this.Owner = parent.TopLevelControl as Form;

			if( UpArrow == null || DownArrow == null )
			{
				throw new ApplicationException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_103 );
			}
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose( bool disposing )
		{
			if( m_ctrlParent != null )
			{
				m_ctrlParent = null;
			}

			if( m_boldFont != null )
			{
				m_boldFont.Dispose();
				m_boldFont = null;
			}

			if( m_defGraphics != null )
			{
				m_defGraphics.Dispose();
				m_defGraphics = null;
			}

			if( disposing )
			{
				if( components != null )
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Shows context prompt window.
		/// </summary>
		/// <param name="location">Location of window to show.</param>
		/// <param name="size">Size of window to show.</param>
		public void ShowContextPrompt( Point location, Size size )
		{
			this.Location = location;
			this.Size = size;
			Show();
		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager( typeof( ContextPrompt ) );
			// 
			// frmContextPrompt
			// 
			this.AccessibleDescription = resources.GetString( "$this.AccessibleDescription" );
			this.AccessibleName = resources.GetString( "$this.AccessibleName" );
			this.AutoScaleBaseSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.AutoScaleBaseSize" ) ) );
			this.AutoScroll = ( ( bool )( resources.GetObject( "$this.AutoScroll" ) ) );
			this.AutoScrollMargin = ( ( System.Drawing.Size )( resources.GetObject( "$this.AutoScrollMargin" ) ) );
			this.AutoScrollMinSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.AutoScrollMinSize" ) ) );
			this.BackColor = System.Drawing.SystemColors.Info;
			this.BackgroundImage = ( ( System.Drawing.Image )( resources.GetObject( "$this.BackgroundImage" ) ) );
			this.ClientSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.ClientSize" ) ) );
			this.ControlBox = false;
			this.Cursor = System.Windows.Forms.Cursors.Default;
			this.Enabled = ( ( bool )( resources.GetObject( "$this.Enabled" ) ) );
			this.Font = ( ( System.Drawing.Font )( resources.GetObject( "$this.Font" ) ) );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ( ( System.Drawing.Icon )( resources.GetObject( "$this.Icon" ) ) );
			this.ImeMode = ( ( System.Windows.Forms.ImeMode )( resources.GetObject( "$this.ImeMode" ) ) );
			this.Location = ( ( System.Drawing.Point )( resources.GetObject( "$this.Location" ) ) );
			this.MaximumSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.MaximumSize" ) ) );
			this.MinimumSize = ( ( System.Drawing.Size )( resources.GetObject( "$this.MinimumSize" ) ) );
			this.Name = "frmContextPrompt";
			this.RightToLeft = ( ( System.Windows.Forms.RightToLeft )( resources.GetObject( "$this.RightToLeft" ) ) );
			this.RightToLeftLayout = true;
			this.ShowInTaskbar = false;
			this.StartPosition = ( ( System.Windows.Forms.FormStartPosition )( resources.GetObject( "$this.StartPosition" ) ) );
			this.Text = resources.GetString( "$this.Text" );
			this.MouseDown += new System.Windows.Forms.MouseEventHandler( this.ContextPrompt_MouseDown );

		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets measuring info of the current context prompt.
		/// </summary>
		protected ContextPrompt.MeasuredInfo Info
		{
			get
			{
				if( m_info == null )
				{
					m_info = GetMeasureInfo();
				}

				return m_info;
			}
		}
		/// <summary>
		/// Gets currently selected prompt.
		/// </summary>
		public ContextPromptItem SelectedPrompt
		{
			get
			{
				UpdateSelection();
				return m_list.SelectedItem;
			}
		}
		/// <summary>
		/// Gets text with the index.
		/// </summary>
		private string TextIndex
		{
			get
			{
				return string.Format( m_sIndexTemplate, m_list.IndexOf( SelectedPrompt ) + 1, TotalChoices );
			}
		}
		/// <summary>
		/// Subject of the currently selected item.
		/// </summary>
		private string TextSubject
		{
			get
			{
				ContextPromptItem item = SelectedPrompt;

				if( item != null )
					return item.Subject;
				else
					return string.Empty;
			}
		}
		/// <summary>
		/// Description of the currently selected item.
		/// </summary>
		private string TextDescription
		{
			get
			{
				ContextPromptItem item = SelectedPrompt;

				if( item != null )
					return item.Description;
				else
					return string.Empty;
			}
		}
		/// <summary>
		/// Gets image associated with the currently selected item.
		/// </summary>
		private Image Image
		{
			get
			{
				ContextPromptItem item = SelectedPrompt;

				if( item != null )
					return item.Image;
				else
					return null;
			}
		}
		/// <summary>
		/// Gets cursor, that shows four directions all together.
		/// </summary>
		public static Image UpArrow
		{
			get
			{
				if( m_imgArrowUp == null )
				{
					Assembly executing = Assembly.GetExecutingAssembly();
					Stream stream = executing.GetManifestResourceStream( DEF_FILENAME_ARROW_UP );

					m_imgArrowUp = Image.FromStream( stream );
				}

				return m_imgArrowUp;
			}
		}
		/// <summary>
		/// Gets cursor, that shows four directions all together.
		/// </summary>
		public static Image DownArrow
		{
			get
			{
				if( m_imgArrowDown == null )
				{
					Assembly executing = Assembly.GetExecutingAssembly();
					Stream stream = executing.GetManifestResourceStream( DEF_FILENAME_ARROW_DOWN );

					m_imgArrowDown = Image.FromStream( stream );
				}

				return m_imgArrowDown;
			}
		}
		/// <summary>
		/// Gets context prompt's list.
		/// </summary>
		public ContextPromptCollection List
		{
			get
			{
				return m_list;
			}
		}
		/// <summary>
		/// Gets or sets index of the currently selected prompt.
		/// </summary>
		public int CurrentPrompt
		{
			get
			{
				UpdateSelection();
				ContextPromptItem item = m_list.SelectedItem;

				if( item == null )
					return -1;

				return m_list.IndexOf( item );
			}
			set
			{
				if( CurrentPrompt != value )
				{
					if( value >= m_list.Count || value < 0 )
						throw new ArgumentOutOfRangeException( "CurrentPrompt" );


					ContextPromptItem item = m_list[ value ];
					item.Selected = true;
				}
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether XP style should be used.
		/// </summary>
		public bool UseXPStyle
		{
			get
			{
				return m_bUseXPStyle;
			}
			set
			{
				m_bUseXPStyle = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether custom context prompt size should be used.
		/// </summary>
		public bool UseCustomSize
		{
			get
			{
				return m_bUseCustomSize;
			}
			set
			{
				m_bUseCustomSize = value;
			}
		}
		/// <summary>
		/// Gets or sets dropping lexem.
		/// </summary>
		public IRenderedLexem Dropper
		{
			get
			{
				return m_droppingLexem;
			}
			set
			{
				m_droppingLexem = value;
			}
		}
		/// <summary>
		/// Gets or sets lexem situated before dropper.
		/// </summary>
		public IRenderedLexem LexemBeforeDropper
		{
			get
			{
				return m_lexemBeforeDropper;
			}
			set
			{
				m_lexemBeforeDropper = value;
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Event, that is raised when selected context prompt is changed.
		/// </summary>
		public event ContextPromptSelectionChangedEventHandler SelectedPromptChanged;
		#endregion

		#region Overrides
		/// <summary>
		/// Cancels selection and closes form.
		/// </summary>
		public new void Close()
		{
			DialogResult = DialogResult.Cancel;
			base.Close();
			Dispose( true );
			DestroyHandle();
		}
		/// <summary>
		/// Draws the whole popup prompt object.
		/// </summary>
		/// <param name="pe">A PaintEventArgs that contains the event data.</param>
		protected override void OnPaint( PaintEventArgs pe )
		{
			MeasuredInfo info = Info;
			StringFormat format = ( StringFormat )StringFormat.GenericTypographic.Clone();
			format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
			if( !m_bUseCustomSize ) this.Size = info.Entire.Size;
			else info.Entire.Size = this.Size;

			Graphics g = pe.Graphics;

			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			BrushPaint.FillRectangle( g, info.Entire, m_backgroundBrush );

			if( null != this.Image )
			{
				g.DrawImage( Image, DEF_OFFSET_BORDER, DEF_OFFSET_BORDER, DEF_IMAGE_SIZE, DEF_IMAGE_SIZE );
			}

			g.DrawImageUnscaled( UpArrow, info.ArrowUp );
			g.DrawImageUnscaled( DownArrow, info.ArrowDown );
			g.DrawString( TextIndex, Font, SystemBrushes.WindowText, info.Index.X, info.Index.Y, format );

			ContextPromptItem itemSelected = SelectedPrompt;
			ContextPromptBoldTextItem itemBolded = itemSelected.BoldedItems.SelectedItem;

			if( itemBolded != null )
			{
				string textBefore = TextSubject.Substring( 0, itemBolded.BoldTextStart );
				string textBolded = TextSubject.Substring( itemBolded.BoldTextStart, itemBolded.BoldTextLength );
				string textAfter = TextSubject.Substring(
					itemBolded.BoldTextStart + itemBolded.BoldTextLength,
					TextSubject.Length - itemBolded.BoldTextStart
					- itemBolded.BoldTextLength );
				//
				SizeF sizeBefore = g.MeasureString( textBefore, Font, PointF.Empty, format );
				SizeF sizeBolded = g.MeasureString( textBolded, m_boldFont, PointF.Empty, format );
				SizeF sizeAfter = g.MeasureString( textAfter, Font, PointF.Empty, format );

				g.DrawString( textBefore, Font, SystemBrushes.WindowText,
					info.Subject.X, info.Subject.Y, format );

				g.DrawString( textBolded, m_boldFont, SystemBrushes.WindowText,
					info.Subject.X + sizeBefore.Width, info.Subject.Y, format );

				g.DrawString( textAfter, Font, SystemBrushes.WindowText,
					info.Subject.X + sizeBefore.Width + sizeBolded.Width, info.Subject.Y, format );
			}
			else
				g.DrawString( TextSubject, Font, SystemBrushes.WindowText, info.Subject.X, info.Subject.Y, format );

			g.DrawString( TextDescription, Font, SystemBrushes.WindowText, info.Description.X, info.Description.Y, format );

			Rectangle entire = info.Entire;
			entire.Height--;
			entire.Width--;
			format.Dispose();
			if( m_bUseXPStyle )
			{
				GraphicsUtils.Draw3DBorder( g, entire );
			}
			else
			{
				g.DrawRectangle( m_borderPen, entire );
			}
			format.Dispose();
		}

		/// <summary>
		/// Processes arrow keys.
		/// </summary>
		/// <param name="keyData">One of the Keys values that represents the key to process.</param>
		/// <returns>True if the keystroke was processed and consumed by the control; otherwise, false to allow further processing.</returns>
		protected override bool ProcessDialogKey( Keys keyData )
		{
			if( !ProcessKey( keyData ) )
			{
				return base.ProcessDialogKey( keyData );
			}

			// Called only if ProcessKey has return True.
			return true;
		}
		/// <summary>
		/// Processes key combination.
		/// </summary>
		/// <param name="keyData">Key combination.</param>
		/// <returns>True, if combination can be processed, otherwise false.</returns>
		public bool ProcessKey( Keys keyData )
		{
			switch( keyData )
			{
				case Keys.Up:
					PreviousChoice();
					break;

				case Keys.Down:
					NextChoice();
					break;

				case Keys.Enter:
					DialogResult = DialogResult.OK;
					Close();
					break;

				case Keys.Escape:
					DialogResult = DialogResult.Cancel;
					Close();
					break;

				default:
					return false;
			}

			return true;
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Sets measuring info to null.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_list_OnChanged( object sender, EventArgs e )
		{
			OnCurrentPromptChanged();
			m_info = null;

			if( Visible )
				Invalidate();
		}
		/// <summary>
		/// Handles the MouseDown event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ContextPrompt_MouseDown( object sender, System.Windows.Forms.MouseEventArgs e )
		{
			Point mousePoint = new Point( e.X, e.Y );

			if( Info.ArrowUp.Contains( mousePoint ) )
			{
				PreviousChoice();
			}
			else if( Info.ArrowDown.Contains( mousePoint ) )
			{
				NextChoice();
			}
		}
		/// <summary>
		/// Hides the popup selection when focus is lost.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">An EventArgs that contains the event data.</param>
		private void ContextPrompt_LostFocus( object sender, EventArgs e )
		{
			this.Hide();
			CurrentPrompt = 0;
		}
		#endregion

		#region Utility Methods
		/// <summary>
		/// Converts SizeF to Size structure.
		/// </summary>
		/// <param name="size">SizeF structure.</param>
		/// <returns>Size structure.</returns>
		private Size SizeFToSize( SizeF size )
		{
			return new Size( ( int )Math.Ceiling( size.Width ),
				( int )Math.Ceiling( size.Height ) );
		}
		/// <summary>
		/// Gets measuring info for the current context prompt.
		/// </summary>
		/// <returns></returns>
		private ContextPrompt.MeasuredInfo GetMeasureInfo()
		{
			MeasuredInfo info = new MeasuredInfo();

			int imageHorOffset = ( null != this.Image ) ? ( DEF_OFFSET_BORDER + DEF_IMAGE_SIZE + DEF_OFFSET_COMMON ) : ( 0 );
			int imageVerOffset = ( null != this.Image ) ? ( DEF_OFFSET_BORDER + DEF_IMAGE_SIZE ) : ( 0 );

			info.ArrowUp.Location = new Point( DEF_OFFSET_BORDER + imageHorOffset, DEF_OFFSET_BORDER );
			info.ArrowUp.Size = UpArrow.Size;

			info.Index.Size = SizeFToSize( m_defGraphics.MeasureString( TextIndex, Font, PointF.Empty, StringFormat.GenericTypographic ) );
			info.Index.Location = new Point( info.ArrowUp.Right + DEF_OFFSET_COMMON, DEF_OFFSET_BORDER );

			info.ArrowDown.Location = new Point( info.Index.Right + DEF_OFFSET_COMMON, DEF_OFFSET_BORDER );
			info.ArrowDown.Size = DownArrow.Size;

			info.Subject.Location = new Point( info.ArrowDown.Right + DEF_OFFSET_COMMON, DEF_OFFSET_BORDER );
			info.Subject.Size = SizeFToSize( m_defGraphics.MeasureString( TextSubject, m_boldFont, PointF.Empty, StringFormat.GenericTypographic ) );

			info.Description.Location = new Point(
				DEF_OFFSET_BORDER + imageHorOffset,
				DEF_OFFSET_COMMON +
				Math.Max(
				Math.Max(
				Math.Max( info.ArrowUp.Bottom, info.Index.Bottom ),
				info.ArrowDown.Bottom ),
				info.Subject.Bottom ) );
			info.Description.Size = SizeFToSize( m_defGraphics.MeasureString( TextDescription, Font, PointF.Empty, StringFormat.GenericTypographic ) );

			info.Entire.Location = new Point( 0, 0 );
			info.Entire.Size = new Size(
				Math.Max( info.Description.Right, info.Subject.Right ) + DEF_OFFSET_BORDER,
				Math.Max(
				info.Description.Bottom, imageVerOffset ) + DEF_OFFSET_BORDER );

			return info;
		}
		/// <summary>
		/// Checks whether there is something selected and selects first item if needed.
		/// </summary>
		private void UpdateSelection()
		{
			if( m_list.SelectedItem == null && TotalChoices > 0 )
				m_list[ 0 ].Selected = true;
		}
		/// <summary>
		/// Displays the next choice.
		/// </summary>
		private void NextChoice()
		{
			if( CurrentPrompt == TotalChoices - 1 )
			{
				CurrentPrompt = 0;
			}
			else
			{
				CurrentPrompt++;
			}
		}
		/// <summary>
		/// Displays the previous choice.
		/// </summary>
		private void PreviousChoice()
		{
			if( CurrentPrompt == 0 )
			{
				CurrentPrompt = TotalChoices - 1;
			}
			else
			{
				CurrentPrompt--;
			}
		}
		/// <summary>
		/// Gets the total number of choices.
		/// </summary>
		internal int TotalChoices
		{
			get
			{
				if( m_list != null )
				{
					return m_list.Count;
				}

				return 0;
			}
		}
		/// <summary>
		/// Raises SelectedPromptChanged event.
		/// </summary>
		protected void OnCurrentPromptChanged()
		{
			if( SelectedPromptChanged != null )
			{
				ContextPromptSelectionChangedEventArgs args =
					new ContextPromptSelectionChangedEventArgs( m_list );

				SelectedPromptChanged( this, args );
			}

			m_info = null;
			Invalidate();
		}
		/// <summary>
		/// Updates form.
		/// </summary>
		protected void OnDataChanged()
		{
		}
		#endregion
	}
}
