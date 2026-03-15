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
using System.Security.Permissions;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Implementation.Config;
using Syncfusion.Windows.Forms.Edit.Design;
using Syncfusion.Windows.Forms.Edit.Utils;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.IO;
using Syncfusion.Windows.Forms.Localization;
using System.Threading;
using Syncfusion.Windows.Forms.Edit.Implementation.Parser;

namespace Syncfusion.Windows.Forms.Edit
{
	/// <summary>
	/// Summary description for FileEditControl.
	/// </summary>
	[Designer( typeof( EditControlDesigner ) )]
	[ToolboxItem( false )]
	[ToolboxBitmap( typeof( FileEditControl ), "ToolboxIcons.Edit.bmp" )]
	public class FileEditControl
		: StreamEditControl
	{
		#region Classes
		/// <summary>
		/// Class used to show message box from other thread.
		/// </summary>
		internal class MessageBoxShower
		{
			#region Fields
			/// <summary>
			/// Dialog result of the message box.
			/// </summary>
			private DialogResult m_res = DialogResult.Cancel;
			/// <summary>
			/// Dialog text.
			/// </summary>
			private string m_text;
			/// <summary>
			/// Dialog caption.
			/// </summary>
			private string m_caption;
			/// <summary>
			/// Dialog icon.
			/// </summary>
			private MessageBoxIcon m_icon;
			/// <summary>
			/// Dialog buttons.
			/// </summary>
			private MessageBoxButtons m_buttons;
			/// <summary>
			/// Dialog default button.
			/// </summary>
			private MessageBoxDefaultButton m_default;
			#endregion

			#region Initialization
			/// <summary>
			/// Creates and initializes new instance of the class.
			/// </summary>
			/// <param name="text">Text of the message box.</param>
			/// <param name="caption">Caption of the message box.</param>
			/// <param name="buttons">Buttons of the message box.</param>
			/// <param name="icon">Message box icon.</param>
			/// <param name="defaultbutton">Default message box buttoon.</param>
			private MessageBoxShower( string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultbutton )
			{
				m_text = text;
				m_caption = caption;
				m_icon = icon;
				m_buttons = buttons;
				m_default = defaultbutton;
			}
			#endregion

			#region Public Methods
			/// <summary>
			/// Shows message box using the new thread.
			/// </summary>
			/// <param name="text">Text of the message box.</param>
			/// <param name="caption">Caption of the message box.</param>
			/// <param name="buttons">Buttons of the message box.</param>
			/// <param name="icon">Message box icon.</param>
			/// <param name="defaultbutton">Default message box buttoon.</param>
			/// <returns>Dialog result of the message box.</returns>
			public static DialogResult ShowDialog(
				string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultbutton )
			{
				MessageBoxShower msgbox = new MessageBoxShower( text, caption, buttons, icon, defaultbutton );
				return msgbox.ShowDialogInternal( text, caption, buttons, icon, defaultbutton );
			}
			#endregion

			#region Nonpublic Methods Methods
			/// <summary>
			/// Shows dialog and saves it`s result.
			/// </summary>
			private void ShowDialogThread()
			{
				DialogResult res = MessageBox.Show( null, m_text, m_caption, m_buttons, m_icon, m_default );

				m_res = res;
			}
			/// <summary>
			/// Creates new thread and calls ShowDialogThread.
			/// </summary>
			/// <param name="text">Text of the message box.</param>
			/// <param name="caption">Caption of the message box.</param>
			/// <param name="buttons">Buttons of the message box.</param>
			/// <param name="icon">Message box icon.</param>
			/// <param name="defaultbutton">Default message box buttoon.</param>
			/// <returns></returns>
			private DialogResult ShowDialogInternal(
				string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultbutton )
			{
				Thread thread = new Thread( new ThreadStart( ShowDialogThread ) );
				thread.IsBackground = true;
				thread.Start();
				thread.Join();
				return m_res;
			}
			#endregion
		}
		#endregion

		#region Constants
		/// <summary>
		/// Size of the data blocks used for copying file content to memory stream if file sharing is enabled.
		/// </summary>
		private const int DEF_DATA_BUFFER_SIZE = 1024 * 8;
		/// <summary>
		/// Name of untitled file.
		/// </summary>
		private const string DEF_UNTITLED_NAME = "Untitled";
		#endregion

		#region Fields
		/// <summary>
		/// Currently opened file. If null, it means that we are working not with a file stream or if m_fileName is not empty,
		/// then we are working with some conversion layer.
		/// </summary>
		private FileStream m_file;
		/// <summary>
		/// Name of the opened file. If empty, editor is working not with a file.
		/// </summary>
		private string m_fileName;
		/// <summary>
		/// SaveAs dialog.
		/// </summary>
		private SaveFileDialog m_saveDialog;
		/// <summary>
		/// Open file dialog.
		/// </summary>
		private OpenFileDialog m_openDialog;
		/// <summary>
		/// If true, file should be converted when loading.
		/// </summary>
		private bool m_bConvertOnLoad = true;
		/// <summary>
		/// Specifies whether file is opened in shared mode.
		/// </summary>
		private bool m_bSharedFileMode;
		/// <summary>
		/// Specifies whether the Save prompt dialog should be displayed before the EditControl is disposed.
		/// </summary>
		private bool m_bSaveOnClose = true;
        /// <summary>
		/// Specifies whether control is disposing.
		/// </summary>
		private bool m_bDisposing;
		/// <summary>
		/// Numbers of lines that were changed and saved. This list is used for creating list of parse points for marking saved lines.
		/// </summary>
		private ArrayList m_savedLines;
		/// <summary>
		/// Indicates whether undo/redo actions were performed after the file saving. Used in indication of modified status.
		/// If is not 0, then undo/redo actions were performed.
		/// </summary>
		private int m_undoAfterSave = 0;
		/// <summary>
		/// File name to be shown in SaveAs dialog.
		/// </summary>
		private string m_strPseudoFileName = string.Empty;
		/// <summary>
		/// List of current parents of control (filled recursively). Used during subscription/unsubscription of ParentChanged events.
		/// </summary>
		private ArrayList m_chainOfParents;
        /// <summary>
        /// File name for loaded file.
        /// </summary>
        internal string getFileName = null;
		#endregion

		#region Properties
		/// <summary>
		/// Gets display name of the file.
		/// The main difference from the FileName property is that Untitled.[ext] will be returned if file name is not set.
		/// </summary>
		public string DisplayFileName
		{
			get
			{
                string result = this.getFileName;
				if( result == null || result == string.Empty )
				{
					if( m_strPseudoFileName != string.Empty )
					{
						result = m_strPseudoFileName;
					}
					else if( this.Language == null || this.Language.Language == Config.DEF_DEFAULT_LANGUAGE_NAME || this.Language.Extensions.Count == 0 )
					{ 
                        result = DEF_UNTITLED_NAME + this.ccount + ".*";
					}
					else
					{
                        result = DEF_UNTITLED_NAME + this.ccount +"."+ this.Language.Extensions[0];
					}
				}
				return result;
			}
		}
		/// <summary>
		/// Get or Set sign, whether file should be converted when loading.
		/// </summary>
		/// <remarks>Such file conversion is needed if file contains different new-line symbols or sequences.</remarks>
		[
		DefaultValue( true ),
		Browsable( true ),
		Category( "Behavior" )
		]
		public bool ConvertOnLoad
		{
			get
			{
				return m_bConvertOnLoad;
			}
			set
			{
				m_bConvertOnLoad = value;
			}
		}
		/// <summary>
		/// Get or Set file stream, that is used as an input.
		/// </summary>
		[Browsable( false )]
		public FileStream FileOpened
		{
			get
			{
				return m_file;
			}
			set
			{
				if( m_file != value )
				{
					LoadStream( value );
					m_file = value;
				}
			}
		}
		/// <summary>
		/// Get or set name of the currently opened file.
		/// </summary>
		[Browsable( false )]
		[DefaultValue( "" )]
		public string FileName
		{
			get
			{
				return ( m_file != null ) ? ( m_file.Name ) : ( m_fileName );
			}
			set
			{
				if( this.FileName != value )
				{
					if( value == string.Empty || value == null )
					{
						NewFile();
					}
					else
					{
                        if (AutoSave)
                            SaveFile(value, null, null);
                        else
                            getFileName = value;
					}
				}
			}
		}
		/// <summary>
		/// Gets or Sets whether file should be opened in shared mode.
		/// </summary>
		[
		DefaultValue( false ),
		Browsable( true ),
		Category( "Behavior" )
		]
		public bool SharedFileMode
		{
			get
			{
				return m_bSharedFileMode;
			}
			set
			{
				m_bSharedFileMode = value;
			}
		}
		/// <summary>
		/// Gets or Sets whether save prompt dialog should be displayed before EditControl is disposed.
		/// </summary>
		[Browsable( false )]
		[DefaultValue( true )]
		public bool SaveOnClose
		{
			get
			{
				return m_bSaveOnClose;
			}
			set
			{
				m_bSaveOnClose = value;
			}
		}
		/// <summary>
		/// Gets or sets file name to be shown in SaveAs dialog.
		/// </summary>
		public string PseudoFileName
		{
			get
			{
				return m_strPseudoFileName;
			}
			set
			{
				m_strPseudoFileName = value;
			}
		}
		#endregion

		#region Nonpublic Properties
		/// <summary>
		/// Gets value indicating whether control is being disposed.
		/// </summary>
		internal new bool Disposing
		{
			get
			{
				return m_bDisposing;
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Event that is raised when underlying stream is about to close and user should decide if he want to save the changes in file.
		/// </summary>
		public event StreamCloseEventHandler StreamClose;
		#endregion

		#region Public Methods
		/// <summary>
		/// Load file and configuration for it.
		/// </summary>
		/// <param name="fileName">Name of the file to load.</param>
		/// <returns>True if operation succeeds.</returns>
		public override bool LoadFile( string fileName )
		{
			return LoadFile( fileName, m_bConvertOnLoad, m_bSharedFileMode );
		}
		/// <summary>
		/// Load file and configuration for it.
		/// </summary>
		/// <param name="fileName">Name of the file to load.</param>
		/// <param name="encoding">Encoding to use while loading.</param>
		/// <returns>True if operation succeeds.</returns>
		public virtual bool LoadFile( string fileName, Encoding encoding )
		{
            if(encoding != null && encoding.EncodingName == "Western European (Windows)")
                return LoadFile(fileName, m_bConvertOnLoad, m_bSharedFileMode);
            else
			    return LoadFile( fileName, false, m_bSharedFileMode, encoding );
		}
		/// <summary>
		/// Shows open file dialog to user and opens selected file.
		/// </summary>
		/// <returns>True if operation succeeds.</returns>
		public virtual bool LoadFile()
		{
			FillInFilters( m_openDialog );
			DialogResult result = m_openDialog.ShowDialog( FindForm() as IWin32Window );

			bool bResult = false;
			if( result == DialogResult.OK )
			{
				bResult = LoadFile( m_openDialog.FileName );
			}

			return bResult;
		}
		/// <summary>
		/// Saves text to file.
		/// </summary>
		/// <returns>True if file was successfully saved. False is returned only if user has cancelled saving somehow.</returns>
		public virtual bool Save()
		{
			bool bResult;
			if( m_fileName == string.Empty )
			{
				bResult = SaveAs();
			}
			else
			{
				if( m_file != null )
				{
					if( !CheckEncoding() )
					{
						bResult = false;
					}
					else
					{
						FillSavedLinesNumbers();
						SaveToStream();
						m_undoAfterSave = 0;
						UpdateSavedLinesPoints();
						bResult = true;
						m_bModified = false;
					}
				}
				else
				{
					string name = m_fileName;
					ChangeFileName( string.Empty );
					SaveFile( name, m_encode, null );
					bResult = true;
					m_bModified = false;
				}
			}

			return bResult;
		}
		/// <summary>
		/// Shows SaveAs dialog and saves data to specified file.
		/// </summary>
		/// <returns>True if operation succeeds.</returns>
		public virtual bool SaveAs()
		{
            FillInFilters(m_saveDialog);
            if (getFileName != null)
            {
                m_saveDialog.FileName = this.getFileName;
            }
            else
            {
                m_saveDialog.FileName = this.DisplayFileName;
            }
			DialogResult result = m_saveDialog.ShowDialog( FindForm() as IWin32Window );

			bool bResult = false;
			if( result == DialogResult.OK )
			{
				SaveFile( m_saveDialog.FileName, null, null );
				bResult = true;
			}

			return bResult;
		}
        internal bool ISSaved = false;
        internal Encoding m_encode;
		/// <summary>
		/// Saves content to the specified file.
		/// </summary>
		/// <param name="fileName">Name of the file to which the text has to be saved.</param>
		/// <param name="encoding">Encoding that has to be used when saving. Can be null.</param>
		/// <param name="lineEndString">Line end string. Can be empty.</param>
		/// <returns>bool indicating whether saving succeeded.</returns>
		public virtual bool SaveFile( string fileName, Encoding encoding, string lineEndString )
		{
            m_encode = encoding;
			fileName = Path.GetFullPath( fileName );
            if(!AutoSave)
                this.FileName = fileName;
			bool bResult;
			if( fileName == this.FileName )
			{
				bResult = Save();
			}
			else
			{
                if (encoding == null && !CheckEncoding())
                {
                    bResult = false;
                }
                else
                {
                   
                    LockPaint();
                    bool bSaveOK = true;
                    LockUpdate();
                    try
                    {
                        // Buffering memory stream.
                        MemoryStream memStream = new MemoryStream();
                        SaveToStream(memStream, null, null);

                        bool bDataLost;
                        // Memory stream will be closed.
                        MemoryStream convertedStream = (MemoryStream)ConvertEncodingAndNewLine(memStream, encoding, lineEndString, out bDataLost);

                        if (bDataLost)
                        {
                            bool bHandled = false;

                            if (SaveFileWithDataLoss != null)
                            {
                                SaveWithDataLosingEventArgs args = new SaveWithDataLosingEventArgs();
                                if (SaveFileWithDataLoss != null)
                                {
                                    SaveFileWithDataLoss(this, args);
                                }

                                if (args.UserHandling)
                                {
                                    if (args.SaveWithLoss)
                                    {
                                        bHandled = true;
                                    }
                                    else
                                    {
                                        bSaveOK = false;
                                    }
                                }
                            }

                            if (!bHandled && MessageBox.Show(Localizer.GetString(Localizer.DEF_MSG_SAVE_FILE_USING_ENCODING), Localizer.GetString(Localizer.DEF_MSG_SAVE_FILE_USING_ENCODING_CAPTION),
                                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.No)
                            {
                                bSaveOK = false;
                            }
                        }

                        if (bSaveOK)
                        {
                            // No recoding - just read once more.
                            if (convertedStream == null)
                            {
                                convertedStream = new MemoryStream();
                                SaveToStream(convertedStream, null, null);
                            }

                            RenderedLine line = CurrentLineInstanceInternal;
                            float yOffset = line.Y + AutoScrollPosition.Y;

                            // Save to file.
                            FillSavedLinesNumbers();
                            FileInfo fileInfo = new FileInfo(fileName);
                            if (File.Exists(fileName) && fileInfo.IsReadOnly && Environment.OSVersion.Version.Major <= 5)
                            {
                                MessageBox.Show(fileName + " \r\nThis file is set to read-only. \r\nTry again with a different file name.", "Save As",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                SaveAs();
                            }
                            else
                            {
                                FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
                                convertedStream.WriteTo(stream);

                                Stream streamToLoad;

                                if (!this.SharedFileMode)
                                {
                                    convertedStream.Close();
                                    stream.Position = 0;
                                    streamToLoad = stream;
                                }
                                else
                                {
                                    stream.Close();
                                    convertedStream.Position = 0;
                                    streamToLoad = convertedStream;
                                }

                                ControlStateStore stateStore = new ControlStateStore();
                                stateStore.StoreData(this);
                                SavedViewInfo info = SaveCurrentViewInfo();

                                if (!m_bSharedFileMode)
                                {
                                    DiscardChanges();
                                    LoadStream(streamToLoad, GetFileLanguage(fileName));
                                }

                                ChangeFileName(fileName);

                                if (!this.SharedFileMode)
                                {
                                    m_file = stream;
                                }
                                ISSaved = bSaveOK;

                                if (m_oldEncoding != encoding || m_oldEncoding== null)
                                {
                                    stateStore.RestoreData(this, false);
                                    RestoreViewInfo(info);
                                    UpdateSavedLinesPoints();
                                }
                            }
                        }
                    }
                    finally
                    {
                        UnlockUpdate();
                        m_bModified = !bSaveOK;
                        UnlockPaint();

                        bResult = bSaveOK;
                    }
                }
                

			}

			if( bResult )
			{
				m_undoAfterSave = 0;
			}

			return bResult;
		}
		/// <summary>
		/// Prompts the user with a save dialog if the current file was modified and saves file if needed.
		/// </summary>
		/// <returns>False if file was changed but user decided not to save file, otherwise true.</returns>
		public virtual bool SaveModified()
		{
			bool bResult = true;

			if( this.IsModified || m_undoAfterSave != 0 )
			{
				StreamCloseEventArgs args = new StreamCloseEventArgs( SaveChangesAction.ShowDialog );
				if( StreamClose != null )
				{
					StreamClose( this, args );
				}

				if( args.Action == SaveChangesAction.ShowDialog )
				{
					bResult = SaveModifiedInternal();
				}
				else
				{
					switch( args.Action )
					{
						case SaveChangesAction.Cancel:
							{
								bResult = false;
								break;
							}
						case SaveChangesAction.Discard:
							{
								DiscardChanges();
								bResult = true;
								break;
							}
						case SaveChangesAction.Save:
							{
								bResult = Save();
								break;
							}
					}
				}
			}

			return bResult;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool SaveModifiedInternal()
		{
			bool bResult = true;

			if( this.IsModified || m_undoAfterSave != 0 )
			{
				if( DesignMode )
				{
					bResult = false;
				}
				else
				{
					DialogResult res = MessageBoxShower.ShowDialog( Localizer.GetString(Localizer.DEF_MSG_SAVE_MODIFIED), Localizer.GetString(Localizer.DEF_MSG_SAVE_MODIFIED_CAPTION),
						MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1 );

					if( res == DialogResult.Cancel )
					{
						bResult = false;
					}
					else if( res == DialogResult.Yes )
					{
						bResult = Save();
					}
					else
					{
						bResult = true;
					}
				}
			}

			return bResult;
		}
		/// <summary>
		/// Creates new empty file with default coloring.
		/// </summary>
		/// <returns>True if file was created, otherwise false.</returns>
		public virtual bool NewFile()
		{
			return base.New();
		}
		/// <summary>
		/// Creates new empty file with specified coloring.
		/// </summary>
		/// <param name="lang">Language to be used for text coloring.</param>
		/// <returns>True if file was created, otherwise false.</returns>
		public virtual bool NewFile( IConfigLanguage lang )
		{
			return base.New( lang );
		}
		#endregion

		#region Nonpublic Methods
		/// <summary>
		/// Load file and configuration for it.
		/// </summary>
		/// <param name="fileName">Name of the file to load.</param>
		/// <param name="convert">Specifies whether file should be corrected on load.</param>
		/// <param name="shared">Specifies whether file is opened in shared mode, when entire file is loaded into memory.</param>
		/// <returns>True if operation succeeds.</returns>
		protected virtual bool LoadFile( string fileName, bool convert, bool shared )
		{
			return LoadFile( fileName, convert, shared, null );
		}
		/// <summary>
		/// Load file and configuration for it.
		/// </summary>
		/// <param name="fileName">Name of the file to load.</param>
		/// <param name="convert">Specifies whether file should be corrected on load.</param>
		/// <param name="shared">Specifies whether file is opened in shared mode, when entire file is loaded into memory.</param>
		/// <param name="encoding">Encoding to use.</param>
		/// <returns>True if operation succeeds.</returns>
		[FileIOPermission( SecurityAction.Assert, Unrestricted = true )]
		[UIPermission( SecurityAction.Assert, Unrestricted = true, Window = UIPermissionWindow.AllWindows )]
		[SecurityPermission( SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode )]
		protected virtual bool LoadFile( string fileName, bool convert, bool shared, Encoding encoding )
		{
            Encoding encode = Encoding.UTF8;
			if( fileName == null ) throw new ArgumentNullException( "fileName" );
			if( fileName.Length == 0 )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_68 );

			bool bResult = false;

			using( ( ( ILongOperationController )this ).StartOperation( "Loading file" ) )
			{
				FileAttributes attr = File.GetAttributes( fileName );

				try
				{
					bool bReadOnly = ( ( attr & FileAttributes.ReadOnly ) == FileAttributes.ReadOnly );

					if( CloseStream() )
					{
						FileStream file = null;
						try
						{
							file = new FileStream( fileName, FileMode.Open, ( bReadOnly ) ? FileAccess.Read : FileAccess.ReadWrite );
						}
						catch( Exception )
						{
							bReadOnly = true;
							file = new FileStream( fileName, FileMode.Open, FileAccess.Read );
						}

						TimeCounter timer = new TimeCounter();
						timer.Start();
						// Note: ConvertStream method closes file after it is converted.
						Stream streamToOpen = null;

                        if (convert)
						{
							bool b;
                            if (encoding == null)
                                streamToOpen = (MemoryStream)ConvertStream(file, out b);
                            else
                                streamToOpen = (MemoryStream)ConvertStream(file, encode, out b);
						}
						else
						{
							streamToOpen = file;
						}

						timer.Finish();

						// If no conversion where done, use file directly.
						if( convert && streamToOpen == null )
						{
							bResult = LoadFile( fileName, false, shared );
						}
						else
						{
							if( shared && !convert )
							{
								MemoryStream memStream = new MemoryStream( ( int )file.Length );
								int read = 0;
								byte[] dataBuffer = new byte[ DEF_DATA_BUFFER_SIZE ];

								while( ( read = streamToOpen.Read( dataBuffer, 0, DEF_DATA_BUFFER_SIZE ) ) > 0 )
								{
									memStream.Write( dataBuffer, 0, read );
								}

								streamToOpen.Close();
								streamToOpen = memStream;
							}

							if( LoadStream( streamToOpen, GetFileLanguage( fileName ), encoding ) )
							{
								ChangeFileName( fileName );
								m_file = (convert || shared ) ? ( null ) : ( file );
								m_strPseudoFileName = string.Empty;
								ClearInfoAboutSavedLines();
								bResult = true;
							}
							else
							{
								bResult = false;
							}
						}
					}
				}
				catch( IOException e )
				{
					New();
					throw e;
				}
			}

			return bResult;
		}
		/// <summary>
		/// Fills in filters in dialog.
		/// </summary>
		/// <param name="dialog">Dialog whose filters should be updated.</param>
		private void FillInFilters( FileDialog dialog )
		{
			if( Configurator == null ) throw new Exception( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_137 );
			if( dialog == null ) throw new ArgumentNullException( "dialog" );

			StringBuilder filters = new StringBuilder();
			int iActiveFilter = 1;
			int iFilter = 1;

			foreach( IConfigLanguage lang in Configurator.KnownLanguages )
			{
				bool bCurrentLanguage = ( lang.Language == Language.Language );
				string[] extensions = new string[ lang.Extensions.Count ];
				for( int i = 0; i < lang.Extensions.Count; i++ )
				{
					extensions[ i ] = "*." + lang.Extensions[ i ].ToString();
				}

				if( filters.Length > 0 )
				{
					filters.Append( '|' );
				}

				string extList = string.Join( ";", extensions );
				string languageName = lang.Language;

				if( languageName == Config.DEF_DEFAULT_LANGUAGE_NAME )
				{
					languageName = "Default";
					extList = "*.*";
				}

				filters.Append( languageName );
				filters.Append( " (" );
				filters.Append( extList );
				filters.Append( ")|" );
				filters.Append( extList );

				if( bCurrentLanguage )
				{
					iActiveFilter = iFilter;
				}

				iFilter++;
			}

			dialog.Filter = filters.ToString();
			dialog.FilterIndex = iActiveFilter;
		}
		/// <summary>
		/// Changes file name.
		/// </summary>
		/// <param name="strNewFileName">New file name.</param>
		private void ChangeFileName( string strNewFileName )
		{
			m_fileName = strNewFileName;
			if( FileNameChanged != null )
			{
				FileNameChanged( this, EventArgs.Empty );
			}
		}
		/// <summary>
		/// Fills collection of changed lines numbers. Used for marking saved lines.
		/// </summary>
		private void FillSavedLinesNumbers()
		{
			m_savedLines.Clear();
			m_savedLines.AddRange( this.Parser.GetChangedLinesNumbers() );

			foreach( IParsePoint point in m_savedLinesPoints )
			{
				if( !m_savedLines.Contains( point.Line ) )
				{
					m_savedLines.Add( point.Line );
				}
			}
			m_savedLines.Sort();
		}
		/// <summary>
		/// Updates list of saved lines points. These points are used in marking saved lines.
		/// </summary>
		private void UpdateSavedLinesPoints()
		{
			m_savedLinesPoints.Clear();

			foreach( int index in m_savedLines )
			{
                if (index <= this.VisibleLineCount)
                {
                    IParsePoint point = this.Parser.GetParsePoint(index, 1);
                    point.Deleted += new ParsePointDeletedEventHandler(OnSavedLinePointDeleted);
                    m_savedLinesPoints.Add(point);
                }
			}

			this.Parser.ClearChangedLines();
		}
		/// <summary>
		/// Clears info about changed and saved lines.
		/// </summary>
		private void ClearInfoAboutSavedLines()
		{
			m_savedLines.Clear();
			m_savedLinesPoints.Clear();
		}
		/// <summary>
		/// Checks whether encoding was forcibly changed and prompts user about it.
		/// </summary>
		/// <returns>False if user cancels saving.</returns>
		private bool CheckEncoding()
		{
			bool result = true;

			if( m_bEncodingForcedlyChanged && !ISSaved)
			{
				DialogResult dialogResult =
					MessageBox.Show( Localizer.GetString(Localizer.DEF_MSG_ENCODING_CHANGED), string.Empty, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question );

				switch( dialogResult )
				{
					case DialogResult.Cancel:
						{
							result = false;
							break;
						}
					case DialogResult.No:
						{
							ChangeEncoding( m_oldEncoding, false );
							break;
						}
				}
			}

			return result;
		}
		/// <summary>
		/// Undates recursive list of parents; deattaches and attaches needed events.
		/// </summary>
		[UIPermission( SecurityAction.Assert, Unrestricted = true, Window = UIPermissionWindow.AllWindows )]
		private void UpdateTopmostFormClosingHandler()
		{
			DeattachParentChainEvents();

			EventHandler parentChangedHandler = new EventHandler( OnParentChanged );
			CancelEventHandler topmostFormClosingHandler = new CancelEventHandler( OnTopmostFormClosing );

			m_chainOfParents.Clear();
			Control parent = this.Parent;
			while( parent != null )
			{
				parent.ParentChanged += parentChangedHandler;
				m_chainOfParents.Add( parent );

				if( parent is Form )
				{
					( ( Form )parent ).Closing += topmostFormClosingHandler;
				}

				parent = parent.Parent;
			}
		}
		/// <summary>
		/// Deattaches events from parent chain.
		/// </summary>
		private void DeattachParentChainEvents()
		{
			EventHandler parentChangedHandler = new EventHandler( OnParentChanged );
			CancelEventHandler topmostFormClosingHandler = new CancelEventHandler( OnTopmostFormClosing );

			int countOfParents = m_chainOfParents.Count;
			if( countOfParents > 0 )
			{
				foreach( Control c in m_chainOfParents )
				{
					c.ParentChanged -= parentChangedHandler;
				}

				Form prevTopmostForm = m_chainOfParents[ countOfParents - 1 ] as Form;
				if( prevTopmostForm != null )
				{
					prevTopmostForm.Closing -= topmostFormClosingHandler;
				}
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Adds event handler for parent changed event.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		protected override void OnParentChanged( EventArgs e )
		{
			base.OnParentChanged( e );

			UpdateTopmostFormClosingHandler();
		}
		/// <summary>
		/// Creates empty stream and makes editor to edit it.
		/// </summary>
		/// <returns>True if operation succeeds.</returns>
		protected new bool New()
		{
			m_strPseudoFileName = string.Empty;
			return base.New();
		}
		/// <summary>
		/// Creates empty stream and makes editor to edit it.
		/// </summary>
		/// <param name="lang">Language of new stream.</param>
		/// <returns>True if operation succeeds.</returns>
		protected new bool New( IConfigLanguage lang )
		{
			m_strPseudoFileName = string.Empty;
			return base.New( lang );
		}
		/// <summary>
		/// Changes encoding of the underlying stream.
		/// </summary>
		/// <param name="newEncoding">New encoding.</param>
        /// <param name="bForced">bForced.</param>
		protected internal override void ChangeEncoding( Encoding newEncoding, bool bForced )
		{
			if( newEncoding == null ) throw new ArgumentNullException( "newEncoding" );

			string strFileName = m_fileName;
			base.ChangeEncoding( newEncoding, bForced );
			ChangeFileName( strFileName );
		}
		/// <summary>
		/// Closes input stream wrapper. Does not close underlying stream.
		/// </summary>
		/// <returns>True if operation succeeds.</returns>
		protected override bool CloseStream()
		{
			bool bResult = false;
			if( IsDisposed || m_bDisposing || !m_bSaveOnClose || SaveModified() )
			{
				if( base.CloseStream() )
				{
					m_undoAfterSave = 0;
					ChangeFileName( string.Empty );
					if( m_file != null )
					{
						m_file.Close();
						m_file = null;
					}

					bResult = true;
				}
			}

			return bResult;
		}
		/// <summary>
		/// Updates filters in Save and Open Dialog.
		/// </summary>
		protected override void OnConfigurationChanged()
		{
			base.OnConfigurationChanged();

			if( m_saveDialog != null )
			{
				FillInFilters( m_saveDialog );
				m_saveDialog.FileName = this.DisplayFileName;
			}

			if( m_openDialog != null )
			{
				FillInFilters( m_openDialog );
			}
		}
		/// <summary>
		/// Decrements m_undoAfterSave 
		/// </summary>
		public override void Undo()
		{
			base.Undo();
			m_undoAfterSave--;
		}
		/// <summary>
		/// Increments m_undoAfterSave 
		/// </summary>
		public override void Redo()
		{
			base.Redo();
			m_undoAfterSave++;
		}
		#endregion

		#region Initialization & Finalization
		/// <summary>
		/// Calls base constructor.
		/// </summary>
		public FileEditControl()
		{
			m_savedLines = new ArrayList();
			m_chainOfParents = new ArrayList();

			this.NewDocCreated += new EventHandler( OnNewDocCreated );

			m_saveDialog = new SaveFileDialog();
			m_openDialog = new OpenFileDialog();
			m_saveDialog.Title = "Save As";
			m_openDialog.Title = "Open File";
			m_openDialog.Multiselect = false;
			FillInFilters( m_saveDialog );
			FillInFilters( m_openDialog );
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose( bool disposing )
		{
			m_bDisposing = true;

			CloseStream();

			DeattachParentChainEvents();

			if( m_saveDialog != null )
			{
				m_saveDialog.Dispose();
				m_saveDialog = null;
			}

			if( m_openDialog != null )
			{
				m_openDialog.Dispose();
				m_openDialog = null;
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Events
		/// <summary>
		/// Event that is raised when user tries to save file with data loosing.
		/// </summary>
		public event SaveWithDataLosingEventHandler SaveFileWithDataLoss;
		/// <summary>
		/// Raised when file name is changed.
		/// </summary>
		public event EventHandler FileNameChanged;
		#endregion

		#region Event Handlers
		/// <summary>
		/// Handles File->Open context menu item.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		protected override void MenuHandlerOpen( object sender, EventArgs e )
		{
			LoadFile();
		}
		/// <summary>
		/// Handles File->Save context menu item.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		protected override void MenuHandlerSave( object sender, EventArgs e )
		{
			Save();
		}
		/// <summary>
		/// Handles File->SaveAs context menu item.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		protected override void MenuHandlerSaveAs( object sender, EventArgs e )
		{
			SaveAs();
		}
		/// <summary>
		/// Clears info abour changed lines.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		private void OnNewDocCreated( object sender, EventArgs e )
		{
			ClearInfoAboutSavedLines();
		}
		/// <summary>
		/// Removes deleted parse point from collection of saved lines points.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="lNewOffset"></param>
		private void OnSavedLinePointDeleted( Syncfusion.Windows.Forms.Edit.Implementation.IO.ParsePoint point, long lNewOffset )
		{
			m_savedLinesPoints.Remove( point );
		}
		/// <summary>
		/// Prompts user for further actions if text was modified.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnTopmostFormClosing( object sender, CancelEventArgs args )
		{
			if( !DesignMode && m_bSaveOnClose )
			{
				args.Cancel |= !SaveModified();
				if( !args.Cancel )
				{
					m_bClosing = true;
					DiscardChanges();
				}
			}
		}
		/// <summary>
		/// Updates chain of parents.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnParentChanged( object sender, EventArgs args )
		{
			UpdateTopmostFormClosingHandler();
		}
		#endregion
	}
}