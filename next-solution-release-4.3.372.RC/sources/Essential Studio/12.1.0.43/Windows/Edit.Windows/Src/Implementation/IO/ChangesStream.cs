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

#define NO_LOCK

using System;
using System.Collections;
using System.IO;

using Syncfusion.IO;
using Syncfusion.Windows.Forms.Edit.Interfaces;

namespace Syncfusion.Windows.Forms.Edit.Implementation.IO
{
	#region *** DataWindowsSearch
	/// <summary>
	/// Comparer for Data Windows.
	/// </summary>
	internal class DataWindowsSearch
		: IComparer
	{
		#region IComparer Members
		/// <summary>
		/// Implementation of Compare method.
		/// </summary>
		/// <param name="x">Datawindow that is compared.</param>
		/// <param name="y">Object, that datawindow is compared to.</param>
		/// <returns></returns>
		public int Compare( object x, object y )
		{
			DataWindow win1 = x as DataWindow;
			DataWindow win2 = y as DataWindow;

			long lPos = ( win2 == null ) ? ( ( long )y ) : ( win2.Position );

			int result = 1;
			if( win1.Position + win1.Size > lPos && win1.Position <= lPos )
			{
				result = 0;
			}
			else if( win1.Position < lPos )
			{
				result = -1;
			}

			return result;
		}
		#endregion
	}
	#endregion

	#region *** ChangesStream
	/// <summary>
	/// Stream, that can track changes.
	/// </summary>
	public class ChangesStream
		: Stream
		, IChangesStream
	{
		#region Constants
		/// <summary>
		/// Default buffer size.
		/// </summary>
		internal protected const int DEF_BUFFER_SIZE = 8192;
		/// <summary>
		/// Length of the block of data for flushing.
		/// </summary>
		internal protected const int DEF_FLUSH_BLOCK_LENGTH = 1024 * 50;
		#endregion

		#region Static Fields
		/// <summary>
		/// Comparer for DataWindows.
		/// </summary>
		protected internal static IComparer DATA_WINDOW_SEARCH_COMPARER = new DataWindowsSearch();
		#endregion

		#region Fields
		/// <summary>
		/// Underlying stream, used as source for ChangesStream.
		/// </summary>
		private Stream m_source;
		/// <summary>
		/// List of changes.
		/// </summary>
		private ArrayList m_changes;
		/// <summary>
		/// List of changes.
		/// </summary>
		private ArrayList m_UndoneChanges;
		/// <summary>
		/// List of DataWindows.
		/// </summary>
		private ArrayList m_dataWindows;
		/// <summary>
		/// Current length of the stream.
		/// </summary>
		private long m_length;
		/// <summary>
		/// Current position in the stream. If buffer is filled, then it is position of the buffer start.
		/// </summary>
		private long m_position;
		/// <summary>
		/// Current data buffer.
		/// </summary>
		private byte[] m_data;
		/// <summary>
		/// Current position in buffer.
		/// </summary>
		private int m_dataPoint;
		/// <summary>
		/// End position of the buffer.
		/// </summary>
		private int m_dataEnd;
		/// <summary>
		/// If true, buffer will be rereaded on next read or position change.
		/// </summary>
		private bool m_bResetCache;
		/// <summary>
		/// Stack of the saved states.
		/// </summary>
		private Stack m_statesStack = new Stack();
		/// <summary>
		/// Count of changes in last state saved in stack.
		/// </summary>
		private int m_lastChangesCount = -1;
		/// <summary>
		/// Count of changes, that can be done before state will be automatically pushed to stack.
		/// </summary>
		private int m_iAutoPush;
		/// <summary>
		/// Sync object for all view and stream
		/// </summary>
		private object m_sync = new object();
		#endregion

		#region Properties
		/// <summary>
		/// Gets value indicating whether user can Undo some actions in stream or not.
		/// </summary>
		public bool CanUndo
		{
			get
			{
				return ( m_changes.Count > 0 );
			}
		}
		/// <summary>
		/// Gets sign of redo ability.
		/// </summary>
		public bool CanRedo
		{
			get
			{
				return ( m_UndoneChanges.Count > 0 );
			}
		}
		/// <summary>
		/// Gets value indicating whether user can read from stream.
		/// </summary>
		public override bool CanRead
		{
			get
			{
				return m_source.CanRead;
			}
		}
		/// <summary>
		/// Gets value indicating whether user can seek position in stream.
		/// </summary>
		public override bool CanSeek
		{
			get
			{
				return m_source.CanSeek;
			}
		}
		/// <summary>
		/// Gets value indicating whether user can write to stream.
		/// </summary>
		public override bool CanWrite
		{
			get
			{
				return m_source.CanWrite;
			}
		}
		/// <summary>
		/// Gets length of the stream
		/// </summary>
		public override long Length
		{
			get
			{
				return m_length;
			}
		}
		/// <summary>
		/// Gets or sets current position in the stream.
		/// </summary>
		public override long Position
		{
			get
			{
				return ( m_position + m_dataPoint );
			}
			set
			{
				if( value < 0 || value > this.Length ) throw new ArgumentOutOfRangeException(
					"Position", value, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_27 );

				if( value >= m_position && value < m_position + m_dataEnd )
				{
					m_dataPoint = ( int )( value - m_position );
				}
				else
				{
					m_position = value;
					OnPositionChanged();
				}
			}
		}
		/// <summary>
		/// Gets or sets count of changes that can be done before state will be automatically pushed to stack.
		/// </summary>
		public int AutoPush
		{
			get
			{
				return m_iAutoPush;
			}
			set
			{
				if( value <= 0 ) throw new ArgumentOutOfRangeException(
					"property value", value, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_28 );

				if( value != m_iAutoPush )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_iAutoPush, value );
					m_iAutoPush = value;

					// raise event on property change
					if( AutoPushChanged != null )
					{
						AutoPushChanged( this, args );
					}
				}
			}
		}
		/// <summary>
		/// Gets list of active windows.
		/// </summary>
		internal protected ArrayList DataWindows
		{
			get
			{
				return m_dataWindows;
			}
		}
		/// <summary>
		/// Gets list of changes.
		/// </summary>
		internal protected ArrayList Changes
		{
			get
			{
				return m_changes;
			}
		}
		/// <summary>
		/// Looks for the last saved count of changes in the stack of Saved States
		/// </summary>
		/// <returns>Integer value of count or -1 if stack is empty</returns>
		internal protected int LastSavedChangesCount
		{
			get
			{
				if( m_statesStack.Count == 0 ) return -1;

				StreamState lastState = m_statesStack.Peek() as StreamState;
				return lastState.ChangesCount;
			}
		}
		/// <summary>
		/// Looks for the last saved list of DataWindows in the stack of Saved States
		/// </summary>
		/// <returns>List of data windows or null if stack is empty</returns>
		internal protected IList LastSavedDataWindows
		{
			get
			{
				if( m_statesStack.Count == 0 ) return null;

				StreamState lastState = m_statesStack.Peek() as StreamState;
				return lastState.WindowsList;
			}
		}
		/// <summary>
		/// Gets synchronization object.
		/// </summary>
		internal protected object SyncObject
		{
			get
			{
				return m_sync;
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Event, that is raised on every change of data.
		/// </summary>
		public event EventHandler DataChanged;
		/// <summary>
		/// Event, that is raised when position of some data was changed. Example: if some data was deleted, data, that is after deleted block,
		/// will be moved back for the length of deleted data, so it`s position will be changed.
		/// </summary>
		public event ValueChangedEventHandler DataPositionChanged;
		/// <summary>
		/// Utility event raised on AutoPush property value change
		/// </summary>
		public event ValueChangedEventHandler AutoPushChanged;
		/// <summary>
		/// Event, that is raised when undo buffer is flushed.
		/// </summary>
		public event EventHandler UndoBufferFlushed;
		/// <summary>
		/// Event, that is raised when redo buffer is flushed.
		/// </summary>
		public event EventHandler RedoBufferFlushed;
		#endregion

		#region Initialization & Finalization
		/// <summary>
		/// Initializes stream by data from other stream.
		/// </summary>
		/// <param name="input">Source stream, must support Read and Seek operations</param>
		public ChangesStream( Stream input )
		{
			if( input == null ) throw new ArgumentNullException( "input" );
			if( !input.CanSeek || !input.CanRead )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_29, "input" );

			m_changes = new ArrayList();
			m_UndoneChanges = new ArrayList();
			m_dataWindows = new ArrayList();

			m_length = input.Length;
			m_position = input.Position;

			m_data = new byte[ DEF_BUFFER_SIZE ];
			m_source = input;

			// add first datawindow which encapsulate file
			m_dataWindows.Add( new DataWindow( new InputStreamSource( m_source ) ) );
			OnPositionChanged();
		}
		/// <summary>
		/// Initializes stream by data from file.
		/// </summary>
		/// <param name="FileName">Name of the file.</param>
		/// <param name="mode">Mode of file opening.</param>
		/// <param name="access">File access type.</param>
		public ChangesStream( string FileName, FileMode mode, FileAccess access )
			: this( new FileStream( FileName, mode, access ) )
		{
		}
		/// <summary>
		///
		/// </summary>
		~ChangesStream()
		{
			Dispose();
		}
		/// <summary>
		/// Frees used memory.
		/// </summary>
		public new void Dispose()
		{
			( ( IDisposable )m_source ).Dispose();
		}
		#endregion

		#region Nonpublic Methods
		/// <summary>
		/// Method, that is raised on every change of m_position. It means that cache must be reloaded.
		/// </summary>
		protected virtual void OnPositionChanged()
		{
			int count = FillInternalBuffer( m_data, m_position, DEF_BUFFER_SIZE );

			m_dataEnd = count;
			m_dataPoint = 0;
			m_bResetCache = false;
		}
		/// <summary>
		/// Raiser for DataChanged event.
		/// </summary>
		protected virtual void RaiseDataChangedEvent()
		{
			if( DataChanged != null )
			{
				DataChanged( this, EventArgs.Empty );
			}
		}
		/// <summary>
		/// Resets cache. Cache will be reloaded on next position change or read from stream.
		/// </summary>
		protected void ResetCache()
		{
			// reset cache
			m_bResetCache = true;
			m_position = this.Position;
			m_dataPoint = 0;
			m_dataEnd = 0;
		}

		/// <summary>
		/// Raiser for DataPositionChanged event
		/// </summary>
		/// <param name="OldPosition">Old position.</param>
		/// <param name="NewPosition">New position.</param>
		protected virtual void RaiseDataPositionChanged( long OldPosition, long NewPosition )
		{
			if( DataPositionChanged != null )
			{
				DataPositionChanged( this, new ValueChangedEventArgs( OldPosition, NewPosition ) );
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

		#region Class Overrides
		/// <summary>
		/// Reads data from stream to buffer. Reading is strarted from the current position.
		/// </summary>
		/// <param name="buffer">Array of bytes, where data must be put.</param>
		/// <param name="offset">Offset in buffer.</param>
		/// <param name="count">Count of bytes to be read.</param>
		/// <returns>Count of bytes, actually read.</returns>
		public override int Read( byte[] buffer, int offset, int count )
		{
			if( buffer == null ) throw new ArgumentNullException( "buffer" );
			if( offset < 0 || offset >= buffer.Length ) throw new ArgumentOutOfRangeException(
				"offset", offset, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_30 );
			if( count < 0 || offset + count > buffer.Length ) throw new ArgumentOutOfRangeException(
				"count", count, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_31 );

			int result = 0;

			if( count != 0 )
			{
				// SYNCHRONIZATION: all read operations from stream must be in lock( m_sync ) scope
				// to prevent change of DataWindows and cache till end of read
#if !NO_LOCK
				lock( m_sync )
#endif
				{
					int read = 0;

					if( m_bResetCache )
					{
						long position = this.Position;
						OnPositionChanged();
						this.Position = position;
						m_bResetCache = false;
					}

					do
					{
						int tocopy = Math.Min( m_dataEnd - m_dataPoint, count - read );
						if( tocopy == 0 )
						{
							break;
						}

						Buffer.BlockCopy( m_data, m_dataPoint, buffer, offset, tocopy );
						// update destination buffer inplace position
						offset += tocopy;
						// update in stream position (loaded new portion of data here if needed)
						this.Position += tocopy;
						// update read bytes counter
						read += tocopy;
						// if we reach end of stream
						if( this.Position == this.Length )
						{
							break;
						}
					}
					while( read != count );

					result = read;
				}
			}

			return result;
		}
		/// <summary>
		/// Writes data to stream from current position. Actually it executes AddChange method with Insert change type.
		/// </summary>
		/// <param name="buffer">Data to be written.</param>
		/// <param name="offset">Offset of data to be written in buffer.</param>
		/// <param name="count">Count of bytes to be written.</param>
		public override void Write( byte[] buffer, int offset, int count )
		{
			if( buffer == null ) throw new ArgumentNullException( "buffer" );
			if( offset < 0 || offset >= buffer.Length ) throw new ArgumentOutOfRangeException(
				"offset", offset, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_30 );
			if( count < 0 || offset + count > buffer.Length ) throw new ArgumentOutOfRangeException(
				"count", count, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_31 );

			if( count != 0 )
			{
				// Creates copy of user data for ChangeContext
				byte[] newBuffer = new byte[ count ];
				Buffer.BlockCopy( buffer, offset, newBuffer, 0, count );

				// Creates change and inserts it.
				ChangeContext change = new ChangeContext( ChangeType.Replace, newBuffer );
				AddChange( change );
			}
		}
		/// <summary>
		/// Sets current position in the stream.
		/// </summary>
		/// <param name="offset">Offset, that position must be moved for.</param>
		/// <param name="origin">Specifies point, where offset must be applied.</param>
		/// <returns>Current position.</returns>
		public override long Seek( long offset, SeekOrigin origin )
		{
			switch( origin )
			{
				case SeekOrigin.Begin:
					this.Position = offset;
					break;

				case SeekOrigin.Current:
					this.Position += offset;
					break;

				case SeekOrigin.End:
					this.Position = this.Length - offset;
					break;
			}

			return this.Position;
		}
		/// <summary>
		/// Changes length of the stream.
		/// </summary>
		/// <param name="value">new length of stream</param>
		public override void SetLength( long value )
		{
			if( value != m_length )
			{
				ChangeContext change = null;

				if( value > m_length )
				{
					byte[] newBuffer = new byte[ value - m_length ];
					change = new ChangeContext( ChangeType.Insert, newBuffer );
				}
				else
				{
					change = new ChangeContext( ChangeType.Delete, null, m_length - value );
				}

				// Add Change method automatically update stream length
				AddChange( m_length, change );

				OnPositionChanged();
			}
		}
		/// <summary>
		/// Saves output stream to input stream.
		/// </summary>
		public override void Flush()
		{
			FlushChanges();
		}
		/// <summary>
		/// Implemented for avoiding PEVerify warnings.
		/// </summary>
		public override void Close()
		{
			base.Close();
		}
		/// <summary>
		/// Implemented for avoiding PEVerify warnings.
		/// </summary>
		/// <returns>Parent value.</returns>
		public override int ReadByte()
		{
			return base.ReadByte();
		}
		/// <summary>
		/// Implemented for avoiding PEVerify warnings.
		/// </summary>
		/// <param name="value">Byte to write.</param>
		public override void WriteByte( byte value )
		{
			base.WriteByte( value );
		}
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Applies change in current position.
		/// </summary>
		/// <param name="context">Context of the change.</param>
		public void AddChange( ChangeContext context )
		{
			AddChange( this.Position, context );
		}
		/// <summary>
		/// Applies change.
		/// </summary>
		/// <param name="position">Position in stream, where change is to be applied.</param>
		/// <param name="context">Context of the change.</param>
		public void AddChange( long position, ChangeContext context )
		{
			AddChange( position, context, true );
		}
		/// <summary>
		/// Applies change.
		/// </summary>
		/// <param name="position">Position in stream, where change is to be applied.</param>
		/// <param name="context">Context of the change.</param>
		/// <param name="bResetRedo">If true, redo buffer will be resetted.</param>
		private void AddChange( long position, ChangeContext context, bool bResetRedo )
		{
			// SYNCHRONIZATION: all add changes to stream must be in lock( m_sync ) scope.
			// Also must be reset cache of all ChangeStreamView's... Till end of change
			// operation no one can not read or write to stream
#if !NO_LOCK
      lock( m_sync )
#endif
			{
				context.Position = position;

				if( bResetRedo && m_UndoneChanges.Count > 0 )
				{
					m_UndoneChanges.Clear();
					RaiseRedoBufferFlushedEvent();
				}

				m_changes.Add( context );

				ResetCache();

				DataWindow firstPart, lastPart;
				long start = position;
				long end = start + context.Size;
				long posChange = 0;

				ArrayList list = GetDataWindowsInRange( start, end );
				if( list.Count == 0 && start != m_length )
					throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_32, "position" );

				if( list.Count > 0 )
				{
					firstPart = list[ 0 ] as DataWindow;
					lastPart = list[ list.Count - 1 ] as DataWindow;
				}
				else
				{
					firstPart = m_dataWindows[ m_dataWindows.Count - 1 ] as DataWindow;
				}

				int index = m_dataWindows.IndexOf( firstPart );
				DataWindow change = new DataWindow( context );
				change.Position = start;

				switch( context.Type )
				{
					case ChangeType.Insert:
						ChangeInsert( index, change, start, end, ref posChange, firstPart );
						break;

					case ChangeType.Replace:
						ChangeReplace( index, change, start, end, ref posChange, firstPart, context );
						break;

					case ChangeType.Delete:
						ChangeDelete( index, change, start, end, ref posChange, context );
						break;
				}

				UpdateDataWindowsPositions( Math.Max( 0, index - 1 ) );

				m_position += posChange;
				int lastCount = ( m_lastChangesCount != -1 ) ? m_lastChangesCount : 0;

				if( m_dataWindows.Count == 0 )
				{
					m_dataWindows.Add( new DataWindow( new InputStreamSource( m_source ), 0, 0 ) );
				}

				// if AutoPush set and we rich it limit then save state of datawindows
				if( ( m_changes.Count - lastCount ) == m_iAutoPush )
				{
					PushState();
				}
			}
		}
		/// <summary>
		/// Undo last change.
		/// WARNING: it simply redos all operation except the last one.
		/// </summary>
		public void Undo()
		{
			if( m_changes.Count > 0 )
			{
				// SYNCHRONIZATION: undo operation must be in lock( m_sync ) scope to prevent any read or write operations till end of stream update
#if !NO_LOCK
        lock( m_sync )
#endif
				{
					int lastCount = m_lastChangesCount;
					ArrayList newChanges;

					ChangeContext undoneChange = m_changes[ m_changes.Count - 1 ] as ChangeContext;

					if( lastCount != -1 )
					{
						// If we just saved, then Pop stack and ReUndo
						if( m_changes.Count == lastCount )
						{
							PopState();
							Undo();
							return;
						}

						newChanges = ( m_changes.Count - 1 != lastCount ) ?
							( ( ArrayList )m_changes.GetRange( lastCount, m_changes.Count - 1 - lastCount ).Clone() ) : ( null );

						if( lastCount != m_changes.Count - 1 )
						{
							ExtractLastState();
						}
						else
						{
							PopState();
						}
					}
					else
					{
						m_dataWindows.Clear();
						m_dataWindows.Add( new DataWindow( new InputStreamSource( m_source ) ) );
						newChanges = ( ArrayList )m_changes.GetRange( 0, m_changes.Count - 1 ).Clone();
						m_changes.Clear();
						UpdateDataWindowsPositions();
						ResetCache();
						Position = 0;
					}

					if( newChanges != null )
					{
						int oldAutoPush = m_iAutoPush;
						m_iAutoPush = -100;

						foreach( ChangeContext change in newChanges )
						{
							AddChange( change.Position, change, false );
						}

						m_iAutoPush = oldAutoPush;
					}

					m_UndoneChanges.Add( undoneChange );
				}
			}
		}
		/// <summary>
		/// Redoes last undone change.
		/// </summary>
		public void Redo()
		{
#if !NO_LOCK
      lock( m_sync )
#endif
			{
				if( CanRedo )
				{
					// Saving action to be redone.
					ChangeContext toRedo = m_UndoneChanges[ m_UndoneChanges.Count - 1 ] as ChangeContext;

					// Redoes action
					AddChange( toRedo.Position, toRedo, false );
					m_UndoneChanges.RemoveAt( m_UndoneChanges.Count - 1 );
				}
			}
		}
		/// <summary>
		/// Restores datawindows and changes to the last state, saved in stack.
		/// Last saved state is removed from stack.
		/// </summary>
		public void PopState()
		{
			if( m_statesStack.Count == 0 )
				return;

			// SYNCHRONIZATION: do not allow any stream changes till end
			// of stream state recovery
#if !NO_LOCK
      lock( m_sync )
#endif
			{
				ExtractLastState();
				m_statesStack.Pop();
				m_lastChangesCount = this.LastSavedChangesCount;
			}
		}
		/// <summary>
		/// Saves current datawindows and changes to stack.
		/// </summary>
		public void PushState()
		{
			// Do not push state to stack if it is already in stack
			if( this.LastSavedChangesCount == m_changes.Count ||
				m_changes.Count == 0 ) return;

			// SYNCHRONIZATION: do not allow any stream changes till end
			// of stream state saving
#if !NO_LOCK
      lock( m_sync )
#endif
			{
				StreamState state = new StreamState( m_changes.Count, Position, m_dataWindows.Count );

				foreach( DataWindow window in m_dataWindows )
				{
					state.WindowsList.Add( window.Clone() );
				}

				m_statesStack.Push( state );
				m_lastChangesCount = state.ChangesCount;
			}
		}
		/// <summary>
		/// Saves all changes.
		/// </summary>
		public void FlushChanges()
		{
			if( !m_source.CanWrite )
				throw new IOException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_33 );

			// SYNCHRONIZATION: on flush fully changed datawindows and
			// context collections that is why we must keep sync
#if !NO_LOCK
      lock( m_sync )
#endif
			{
				if( this.Length > 0 )
				{
					MemoryStream stream = new MemoryStream( ( int )Length );

					CopyTo( stream );

					m_source.SetLength( Length );
					m_source.Position = 0;
					stream.Position = 0;

					stream.WriteTo( m_source );
				}
				else
				{
					m_source.SetLength( 0 );
					m_source.Position = 0;
				}

				m_source.Flush();
				ResetStream();
			}
		}
		/// <summary>
		/// Sets new line style to the underlying stream.
		/// </summary>
		/// <param name="style">Style of new line.</param>
		public void SetNewLineStyle( NewLineStyle style )
		{

		}
		/// <summary>
		/// Writes current stream data to output stream.
		/// </summary>
		/// <param name="stream">Output stream.</param>
 
#if SyncfusionFramework4_0

		public new void CopyTo( Stream stream )
#else
		public void CopyTo( Stream stream )
#endif
		{
			if( stream == null )
				throw new ArgumentNullException( "stream" );

			if( !stream.CanWrite )
				throw new ArgumentNullException( "stream", Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_34 );

#if !NO_LOCK
      lock( m_sync )
#endif
			{
				long oldPosition = this.Position;
				this.Position = 0;
				byte[] data = new byte[ DEF_FLUSH_BLOCK_LENGTH ];

				while( Position < Length )
				{
					int bytesRead = this.Read( data, 0, DEF_FLUSH_BLOCK_LENGTH );
					stream.Write( data, 0, bytesRead );
				}

				this.Position = oldPosition;
			}
		}
		/// <summary>
		/// Resets all changes, done to stream.
		/// </summary>
		internal void DiscardChanges()
		{
			ResetStream();
		}
		#endregion

		#region Class utility methods
		/// <summary>
		/// Reset stream to default startup state
		/// </summary>
		private void ResetStream()
		{
			m_UndoneChanges.Clear();
			m_changes.Clear();
			m_dataWindows.Clear();
			m_dataWindows.Add( new DataWindow( new InputStreamSource( m_source ) ) );
			m_statesStack.Clear();
			m_lastChangesCount = -1;

			ResetCache();
			OnPositionChanged();

			RaiseUndoBufferFlushedEvent();
			RaiseRedoBufferFlushedEvent();
		}
		/// <summary>
		/// Make update of DataWindows according to Insert operation type logic
		/// </summary>
		/// <param name="index">Index of first DataWindow in update range</param>
		/// <param name="change">DataWindow with chages</param>
		/// <param name="start">Start position of update region</param>
		/// <param name="end">End position of update region</param>
		/// <param name="posChange">influence on current stream position</param>
		/// <param name="firstPart">DataWindow to which start position belong</param>
		protected void ChangeInsert( int index, DataWindow change, long start,
			long end, ref long posChange, DataWindow firstPart )
		{
			if( index < 0 )
				throw new ArgumentOutOfRangeException( "index", index, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_17 );

			if( change == null )
				throw new ArgumentNullException( "change" );

			if( start < 0 )
				throw new ArgumentOutOfRangeException( "start", start, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_17 );

			if( end < 0 )
				throw new ArgumentOutOfRangeException( "end", end, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_17 );

			if( posChange < 0 )
				throw new ArgumentOutOfRangeException( "posChange", posChange, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_17 );

			if( firstPart == null )
				throw new ArgumentNullException( "firstPart" );

			// insert new DataWindow before update range
			if( start == firstPart.Position )
			{
				m_dataWindows.Insert( index, change );
			}
			// insert new DataWindow after update range
			else if( firstPart.Position + firstPart.Size == start )
			{
				m_dataWindows.Insert( index + 1, change );
			}
			else // insert new DataWindow into update range
			{
				DataWindow secondPart = SplitDataWindow( firstPart, start );
				m_dataWindows.InsertRange( index + 1, new DataWindow[] { change, secondPart } );
			}

			// update position only if data was insert before it
			posChange = ( start <= m_position ) ? change.Size : 0;

			RaiseDataPositionChanged( start, end );
		}
		/// <summary>
		/// Make update of DataWindows according to Replace operation type logic
		/// </summary>
		/// <param name="index">Index of first DataWindow in update range</param>
		/// <param name="change">DataWindow with chages</param>
		/// <param name="start">Start position of update region</param>
		/// <param name="end">End position of update region</param>
		/// <param name="posChange">influence on current stream position</param>
		/// <param name="firstPart">DataWindow to which start position belong</param>
		/// <param name="context">Changes to context object</param>
		protected void ChangeReplace( int index, DataWindow change, long start,
			long end, ref long posChange, DataWindow firstPart, ChangeContext context )
		{
			if( index < 0 )
				throw new ArgumentOutOfRangeException( "index", index, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_17 );

			if( change == null )
				throw new ArgumentNullException( "change" );

			if( start < 0 )
				throw new ArgumentOutOfRangeException( "start", start, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_17 );

			if( end < 0 )
				throw new ArgumentOutOfRangeException( "end", end, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_17 );

			if( posChange < 0 )
				throw new ArgumentOutOfRangeException( "posChange", posChange, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_17 );

			if( firstPart == null )
				throw new ArgumentNullException( "firstPart" );

			if( context == null )
				throw new ArgumentNullException( "context" );

			// Replace operation can be replaced by delete and insert operations
			int diff = ( firstPart.Position == start ) ? 0 : 1;
			change.Size = context.Size;
			TrimDataWindows( index, start, change );
			change.Size = context.Length;

			m_dataWindows.Insert( index + diff, change );

			// update current position
			if( start + context.Size <= m_position )
			{
				posChange += -context.Size;
			}
			else if( start + context.Size > m_position && m_position > start ) // in center
			{
				posChange += ( m_position - start );
			}

			posChange += ( start <= m_position ) ? context.Length : 0;

			RaiseDataPositionChanged( start, start + context.Length );
		}

		/// <summary>
		/// Make update of DataWindows according to Delete operation type logic
		/// </summary>
		/// <param name="index">Index of first DataWindow in update range</param>
		/// <param name="change">DataWindow with chages</param>
		/// <param name="start">Start position of update region</param>
		/// <param name="end">End position of update region</param>
		/// <param name="posChange">influence on current stream position</param>
		/// <param name="context">Changes to context object</param>
		protected void ChangeDelete( int index, DataWindow change, long start,
			long end, ref long posChange, ChangeContext context )
		{
			if( index < 0 )
				throw new ArgumentOutOfRangeException( "index", index, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_17 );

			if( change == null )
				throw new ArgumentNullException( "change" );

			if( start < 0 )
				throw new ArgumentOutOfRangeException( "start", start, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_17 );

			if( end < 0 )
				throw new ArgumentOutOfRangeException( "end", end, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_17 );

			if( posChange < 0 )
				throw new ArgumentOutOfRangeException( "posChange", posChange, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_17 );

			if( context == null )
				throw new ArgumentNullException( "context" );

			change.Size = context.Size;
			TrimDataWindows( index, start, change );

			// update position only if data was deleted before it
			if( start + change.Size <= m_position )
			{
				posChange += -change.Size;
			}
			// in center
			else if( start + change.Size > m_position && m_position > start )
			{
				posChange += ( m_position - start );
			}

			RaiseDataPositionChanged( end, start );
		}
		/// <summary>
		/// Restores datawindows and changes to the last state, saved in stack.
		/// WARNING: Method always must be called in lock( m_sync ) scope!!!
		/// </summary>
		protected void ExtractLastState()
		{
			if( m_statesStack.Count == 0 )
				return;

			StreamState state = m_statesStack.Peek() as StreamState;
			m_dataWindows.Clear();
			m_dataWindows = new ArrayList( state.WindowsList.Count );

			foreach( DataWindow window in state.WindowsList )
			{
				m_dataWindows.Add( window.Clone() );
			}

			m_changes.RemoveRange( state.ChangesCount, m_changes.Count - state.ChangesCount );
			UpdateDataWindowsPositions();
			ResetCache();
			Position = state.Position;
		}
		/// <summary>
		/// Fills cache by data from stream.
		/// </summary>
		/// <param name="data">Buffer</param>
		/// <param name="position">Position in stream</param>
		/// <param name="size">Size of data to be read</param>
		/// <returns>Number of read operations.</returns>
		internal protected int FillInternalBuffer( byte[] data, long position, long size )
		{
			if( data == null )
				throw new ArgumentNullException( "data" );

			if( position < 0 || position > this.Length )
				throw new ArgumentOutOfRangeException( "position", position, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_35 );

			if( size < 0 || size > data.Length )
				throw new ArgumentOutOfRangeException( "size", size, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_36 );

#if !NO_LOCK
      lock( m_sync )
#endif
			{
				int read = 0;
				int offset = 0;

				ArrayList list = GetDataWindowsInRange( position, position + size );

				for( int i = 0; i < list.Count; i++ )
				{
					DataWindow window = list[ i ] as DataWindow;
					if( position == window.Position + window.Size ) return read;

					long lWinOffset = window.Position + window.Size - position;
					bool bInWindow = lWinOffset > 0;

					// get quantity of data to copy
					long copy = Math.Min( window.Size, size );

					// if new position fully jump over window then set window.Start as start point
					long lStart = ( bInWindow ) ? position - window.Position + window.Start : window.Start;

					// update quantity of data to copy
					if( bInWindow ) copy = Math.Min( copy, lWinOffset );

					// copy data
					int inread = window.Source.GetData( lStart, data, offset, ( int )copy );

					// update offset of destination buffer
					offset += inread;

					// update position in stream
					position += inread;

					// reduce size
					size -= inread;

					// accumulate read operations
					read += inread;

					if( position == m_length ) return read;
				}

				return read;
			}
		}
		/// <summary>
		/// Searches for all data windwos in specified range.
		/// </summary>
		/// <param name="start">Start position</param>
		/// <param name="end">End position</param>
		/// <returns>Array of DataWindows which contains data from start to end points</returns>
		protected ArrayList GetDataWindowsInRange( long start, long end )
		{
			if( start >= m_length ) return m_dataWindows.GetRange( 0, 0 );

			int startPos = m_dataWindows.BinarySearch( start, DATA_WINDOW_SEARCH_COMPARER );
			int endPos = m_dataWindows.BinarySearch( end, DATA_WINDOW_SEARCH_COMPARER );

			if( startPos < 0 ) startPos = m_dataWindows.Count - 1;
			if( endPos < 0 ) endPos = m_dataWindows.Count - 1;

			int count = endPos - startPos + 1;
			return m_dataWindows.GetRange( startPos, count );
		}
		/// <summary>
		/// Divides one window into two in some position.
		/// </summary>
		/// <param name="window">Window to be divided</param>
		/// <param name="position">Position in stream of division point</param>
		/// <returns>Second data window, that is cut</returns>
		protected DataWindow SplitDataWindow( DataWindow window, long position )
		{
			if( position < window.Position || position > window.Size + window.Position )
				throw new ArgumentOutOfRangeException( "position", position, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_37 );

			// create second part of data window
			long startOffset = position - window.Position;
			DataWindow second = new DataWindow( window.Source, startOffset + window.Start, window.Size - startOffset );

			// update source data window size
			window.Size = startOffset;

			// update second window position
			second.Position = position;

			return second;
		}

		/// <summary>
		/// Trims datawindows in specified range.
		/// </summary>
		/// <param name="index">Index of the first datawindow, that must be cut.
		/// Can be 0, then scan of entire list will occure.</param>
		/// <param name="position">Start position of trimming.</param>
		/// <param name="change">DataWindow, which size will be used for trimming.</param>
		protected void TrimDataWindows( int index, long position, DataWindow change )
		{
			long size = change.Size;
			long end = position + size;

			ArrayList todelete = new ArrayList();

			for( int i = index; i < m_dataWindows.Count; i++ )
			{
				DataWindow window = m_dataWindows[ i ] as DataWindow;
				long winEnd = window.Position + window.Size;

				if( window.Position >= position && winEnd <= end ) // delete
				{
					todelete.Add( window );
				}
				else if( window.Position < position && winEnd <= end ) // trim right side of window
				{
					int diff = ( int )( winEnd - position );
					window.Size -= diff;

					if( window.Size == 0 ) todelete.Add( window );
				}
				else if( window.Position >= position && window.Position <= end && end < winEnd ) // trim left side of window
				{
					int diff = ( int )( end - window.Position );
					window.Start += diff;
					window.Size -= diff;

					if( window.Size == 0 ) todelete.Add( window );
					break;
				}
				else // split window
				{
					DataWindow secondPart = SplitDataWindow( window, position );
					m_dataWindows.Insert( i + 1, secondPart );
				}
			}

			// remove unneeded to us DataWindows from collection
			foreach( DataWindow win in todelete )
			{
				m_dataWindows.Remove( win );
				win.Dispose();
			}
		}

		/// <summary>
		/// Updates all window's positions.
		/// Length is also updated.
		/// </summary>
		protected void UpdateDataWindowsPositions()
		{
			UpdateDataWindowsPositions( 0 );
		}
		/// <summary>
		/// Updates all window's positions starting from some index.
		/// Length is also updated.
		/// </summary>
		/// <param name="start">Index of the first windwo to be updated</param>
		protected void UpdateDataWindowsPositions( int start )
		{
			long position = 0;

			for( int i = start; i < m_dataWindows.Count; i++ )
			{
				DataWindow window = m_dataWindows[ i ] as DataWindow;

				// "&& start != 0" - if first element was deleted, then it`s
				// position will not be updated correctly without this condition
				if( i == start && start != 0 ) position = window.Position;

				window.Position = position;
				position += window.Size;
			}

			// update stream length
			m_length = position;
		}
		#endregion
	}
	#endregion
}