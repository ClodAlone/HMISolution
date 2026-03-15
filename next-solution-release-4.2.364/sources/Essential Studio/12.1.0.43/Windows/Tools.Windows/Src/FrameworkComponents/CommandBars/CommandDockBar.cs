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

#region File Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Runtime.InteropServices;

using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools.XPMenus;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public class RowMarker
	{
		protected CommandDockBar cmdDockBar;
		protected int nRefCount = 0;

		public CommandDockBar DockBar
		{
			get { return this.cmdDockBar; }
		}

		public RowMarker( CommandDockBar dockbar )
		{
			this.cmdDockBar = dockbar;
		}

		public void IncreaseRefCount()
		{
			this.nRefCount++;
		}

		public void DecreaseRefCount()
		{
			this.nRefCount--;
			if( this.nRefCount == 0 )
				this.cmdDockBar.RowMarkers.Remove( this );
		}
	}


	// CommandDockBar is the equivalent of the MFC/Win32 CDockBar. The main frame window has 4 CommandDockBars
	// aligned along each edge and the CommandDockBar, in turn, will host the various CommandBars.
	[
	DesignTimeVisible( false ),
	ToolboxItem( false )
	]
	[Syncfusion.Documentation.DocumentationExclude()]
	public class CommandDockBar: System.Windows.Forms.ContainerControl
	{
		#region Delegates
		delegate void ResumeLayoutAsync();
		#endregion

		#region Fields
		protected CommandBarController cbController = null;
		// Collection of commandbars hosted by this commanddockbar
		public int nRCCount = 0;
		// XPTheme drawing
		protected ThemedControlDrawing tdRebar = null;
		protected internal ArrayList alRowMarkers = new ArrayList();
		protected bool bExpanding = false;
		/// <summary>
		/// 
		/// </summary>
		private int m_nSuspendCount = 0;
		internal bool bSuspendIndexChanges = false;
		#endregion

		public ArrayList RowMarkers
		{
			get { return this.alRowMarkers; }
		}

		public CommandDockBar( CommandBarController cbc )
		{
			this.InitializeCommandDockBar( cbc );
			if( cbc != null )
			{
				cbc.LayoutSuspended += new EventHandler( OnControllerLayoutSuspended );
				cbc.LayoutResumed += new EventHandler( OnControllerLayoutResumed );
			}
		}

		public void InitializeCommandDockBar( CommandBarController cbc )
		{
			this.cbController = cbc;
			this.SetStyle( ControlStyles.Selectable, false );
			this.SetStyle( ControlStyles.AllPaintingInWmPaint|ControlStyles.UserPaint|ControlStyles.DoubleBuffer, true );
			this.TabStop = false;
			if( cbc.bBackColorSet == true )
				base.BackColor = cbc.BackColor;
			if( XPThemes.IsThemedOS )
				this.tdRebar = new ThemedControlDrawing( ThemedControls.REBAR );
		}

		private Hashtable m_barStateHash = new Hashtable();

		internal void ReplaceBarLocation( CommandBar cbar )
		{
			if( cbar == null )
				throw new ArgumentNullException( "cbar" );

			m_barStateHash[cbar.Name] = cbar.nRCIndex;

			if( cbar.nRCIndex >= cbar.cdbParent.nRCCount )
			{
				cbar.cdbParent.nRCCount = cbar.nRCIndex + 1;
			}
		}

		internal void ResetBarState()
		{
			m_barStateHash.Clear();
		}

		internal void RemoveBarIndex( CommandBar cbar )
		{
			if( cbar == null )
				throw new ArgumentNullException( "cbar" );

			if( m_barStateHash[cbar.Name] == null ) return;

			m_barStateHash.Remove( cbar.Name );
		}

		internal void MoveBarFirst( CommandBar cbar )
		{
			foreach( CommandBar cb in this.Controls )
			{
				if( cb != cbar )
				{
					cb.nRCIndex++;
				}
			}

			cbar.nRCIndex = 0;

			this.nRCCount++;
		}

		internal void SaveBarIndex( CommandBar cbar )
		{
			if( cbar == null )
				throw new ArgumentNullException( "cbar" );

			if( m_barStateHash[cbar.Name] == null )
			{
				m_barStateHash[cbar.Name] = cbar.nRCIndex;
			}
		}

		private void CorrectBarsLocation( CommandBar cbar )
		{
			if( cbar == null )
				throw new ArgumentNullException( "cbar" );

			if( m_barStateHash[cbar.Name] == null )
			{
				SaveBarIndex( cbar );
			}

			int savedRowIndex = 0;
			if( CommandBar.bDragging )
				savedRowIndex = cbar.nRCIndex;
			else
				savedRowIndex = (int)m_barStateHash[cbar.Name];

			int newBarIndex = 0;
			int actualIndex = 0;
			bool bNeedNewRow = false;
			Hashtable rowIndexHash = new Hashtable();
			foreach( CommandBar cb in this.Controls )
			{
				if( ( !cb.Visible ) || ( m_barStateHash[cb.Name] == null ) || ( cbar == cb ) ) continue;

				int rowIndex = 0;
				if( CommandBar.bDragging )
					rowIndex = cb.nRCIndex;
				else
					rowIndex = (int)m_barStateHash[cb.Name];

				if( savedRowIndex < rowIndex && cb.nRCIndex < rowIndex )
				{
					bNeedNewRow = true;
					cb.nRCIndex++;
				}
				else
				{
					if( rowIndex == savedRowIndex && !cb.bFullRow )
					{
						bNeedNewRow = false;
						newBarIndex = cb.nRCIndex;
						break;
					}
					else if( rowIndexHash[rowIndex] == null )
					{
						rowIndexHash[rowIndex] = true;
						bNeedNewRow = true;

						if( rowIndex > actualIndex )
							actualIndex = rowIndex;

						if( savedRowIndex >= rowIndex )
							newBarIndex = actualIndex + 1;
					}
				}
			}

			if( bNeedNewRow )
			{
				int count = Math.Max( GetMaxRowIndex(), newBarIndex ) + 1;
				nRCCount = Math.Min( nRCCount + 1, count );
			}

			cbar.nRCIndex = newBarIndex;
		}

		internal int GetMaxRowIndex()
		{
			int maxIndex = -1;

			if( m_barStateHash != null && m_barStateHash.Count > 0 )
			{
				foreach( DictionaryEntry entry in m_barStateHash )
				{
					CommandBar cb = this.GetCommandBarFromName( (string)entry.Key );
					if( cb != null && cb.Visible )
						maxIndex = Math.Max( (int)entry.Value, maxIndex );
				}
			}

			return maxIndex;
		}

		private CommandBar GetCommandBarFromName( string cbName )
		{
			foreach( CommandBar cb in this.Controls )
			{
				if( cb.Name == cbName )
					return cb;
			}

			return null;
		}

		internal int GetCommandBarIndex( CommandBar cbar )
		{
			if( m_barStateHash[cbar.Name] != null )
				return (int)m_barStateHash[cbar.Name];
			else
				return -1;
		}

		internal void SaveBarIndexChange( CommandBar cbar, int valueChanged )
		{
			if( m_barStateHash[cbar.Name] == null ) return;

			int newRowIndex = (int)m_barStateHash[cbar.Name] + valueChanged;
			if( newRowIndex < 0 )
			{
				newRowIndex = 0;
			}

			if( newRowIndex > GetMaxRowIndex() + 1 )
			{
				newRowIndex = GetMaxRowIndex();
			}

			m_barStateHash[cbar.Name] = newRowIndex;
		}

		internal void DockBar( CommandBar cbar, bool dockFirst )
		{
			if( dockFirst )
			{
				string[] arrKeys = new string[m_barStateHash.Count];
				m_barStateHash.Keys.CopyTo( arrKeys, 0 );

				for( int i = 0, len = arrKeys.Length; i < len; i++ )
				{
					int index = (int)m_barStateHash[arrKeys[i]];
					m_barStateHash[arrKeys[i]] = ( ++index );
				}

				m_barStateHash[cbar.Name] = 0;
			}
			else
			{
				m_barStateHash[cbar.Name] = GetMaxRowIndex() + 1;
			}
		}

		private bool m_bIsInitializing = false;

		internal bool IsInitializing
		{
			get
			{
				return m_bIsInitializing;
			}
			set
			{
				if( value != m_bIsInitializing )
				{
					m_bIsInitializing = value;
				}
			}
		}
		public void AddCommandBar( CommandBar cbar, bool bcreatenewrow )
		{
			if( this.Contains( cbar ) )
				return;

			CommandBarDockState border = this.GetDockBorder();
			if( cbar.cbarDockState != border )
				cbar.cbarDockState = border;

			if( !cbar.bRestrictedSizing )	// ControlBar
			{
				if( ( border == CommandBarDockState.Top ) || ( border == CommandBarDockState.Bottom ) )
				{
					if( cbar.nCommandBarHt < cbar.MinHeight )
						cbar.nCommandBarHt = cbar.MinHeight;
				}
				else
				{
					if( cbar.nCommandBarHt < cbar.MinLength )
						cbar.nCommandBarHt = cbar.MinLength;
				}
			}

			// If the CommandBar's RowMarker is referencing some other dockbar, then reset marker to null
			if( ( cbar.rMarker != null ) && ( cbar.rMarker.DockBar != this ) )
			{
				cbar.rMarker.DecreaseRefCount();
				cbar.rMarker = null;
			}

			if( !bcreatenewrow )
			{
				// If CommandBar.nRCIndex is -1 then a new row/col should be inserted at position 0.
				// This insertion is, however, conditional to the presence of other bars that may have
				// the leadingedge flag set.
				// If nRCIndex is equal to the rowcount then append a new row/column. This addition
				// is also conditional to the presence of bars that may have the trailingedge flag set.

				if( ( cbar.bLeadingEdge ) && ( cbar.nRCIndex != -1 ) )
					cbar.nRCIndex = 0;
				else if( cbar.bTrailingEdge )
					cbar.nRCIndex = this.nRCCount;

				//				if(cbar.nRCIndex > this.nRCCount)
				//					cbar.nRCIndex = this.nRCCount;

				if( ( cbar.nRCIndex == -1 ) || ( cbar.nRCIndex == 0 ) )
				{
					CommandBar[] cbarray = this.GetRowArray( 0 );
					foreach( CommandBar cb in cbarray )
					{
						if( cb == cbar )
							continue;
						if( cb.bLeadingEdge || ( cb.bFullRow && cbar.nRCIndexDrag == 0 ) )
						{
							if( cb.bFullRow )
							{
								cbar.nRCIndex = 1;
								// Downshift CommandBars in rows 1 and onwards by one row
								for( int i = this.nRCCount - 1; i >= 1; i-- )
								{
									CommandBar[] cbrowarray = this.GetRowArray( i );
									foreach( CommandBar cbrow in cbrowarray )
										cbrow.nRCIndex++;
								}
								this.nRCCount++;
							}
							else if( cbar.nRCIndex == -1 )
								cbar.nRCIndex = 0;
							break;
						}
					}
				}
				else if( ( cbar.nRCIndex == this.nRCCount ) || ( cbar.nRCIndex == ( this.nRCCount - 1 ) ) )
				{
					CommandBar[] cbarray = this.GetRowArray( this.nRCCount - 1 );
					foreach( CommandBar cb in cbarray )
					{
						if( cb == cbar )
							continue;
						if( cb.bTrailingEdge )
						{
							cbar.nRCIndex = cb.nRCIndex - 1;
							if( cbar.nRCIndex < 0 )
								cbar.nRCIndex = 0;

							CommandBar[] cbarArray = this.GetRowArray( cbar.nRCIndex );

							if( cb.bFullRow && ( cbarArray.Length == 0 || cbarArray.Length == 1 && cbarArray[0] == cb ) )
							{
								cb.nRCIndex++;
								this.ReplaceBarLocation( cb );
							}
							break;
						}
					}
				}

				if( ( cbar.nRCIndex == -1 ) || ( cbar.nRCIndex == this.nRCCount ) || 
					( this.nRCCount == 0 ) )
					this.nRCCount++;

				if( cbar.nRCIndex == -1 )
				{
					foreach( CommandBar cb in this.Controls )
					{
						if( cb.Visible == true )
							cb.nRCIndex++;
					}
					cbar.nRCIndex = 0;
				}

				// If the new bar has an rowindex greater than the bars in the prev array, then tack on
				// the new bar if a new row is desired. Else insert it into the previous position.
				CorrectBarsLocation( cbar );
				bool bappend = true;
				if( cbar.nRCIndex < 0 )
					bappend = false;
				else
				{
					CommandBar[] cbprevarray = this.GetRowArray( cbar.nRCIndex - 1 );
					if( cbprevarray.Length == 0 )
					{
						bappend = true;
					}
					else if( cbprevarray[0].nRCIndex > cbar.nRCIndex )
						bappend = false;
				}
				if( cbar.nRowOffsetInDir != cbar.nRowOffsetDir )
					cbar.nRowOffsetInDir = cbar.nRowOffsetDir;

				this.ValidateBarIndex( cbar, bappend );
			}
			else // (bcreatnewrow == true)
			{
				// Shift the existing CommandBars in cBar.nRCIndex and higher by one position
				if( !bSuspendIndexChanges )
				{
					for( int i = this.nRCCount - 1; i >= cbar.nRCIndex; i-- )
					{
						CommandBar[] cbrowarray = this.GetRowArray( i );
						foreach( CommandBar cbrow in cbrowarray )
						{
							cbrow.nRCIndex++;
							this.ReplaceBarLocation( cbrow );
						}
					}
				}

				// If the new bar has an rowindex greater than the bars in the prev array, then tack on
				// the new bar if a new row is desired. Else insert it into the previous position.
				CorrectBarsLocation( cbar );
				SaveBarIndex( cbar );
			}

			// Set the commandbar size
			if( ( this.Dock == DockStyle.Top ) || ( this.Dock == DockStyle.Bottom ) )
			{
				int nheight = this.GetRowMaxHeight( cbar.nRCIndex, false );
				cbar.Height = ( nheight > cbar.nCommandBarHt ) ? nheight : cbar.nCommandBarHt;
			}
			else
			{
				int nwidth = this.GetRowMaxHeight( cbar.nRCIndex, false );
				cbar.Width = ( nwidth > cbar.nCommandBarHt ) ? nwidth : cbar.nCommandBarHt;
			}

			CommandBar[] cbnewrow = this.GetRowArray( cbar.nRCIndex );
			this.Controls.Add( cbar );
			bool blayoutrecalc = this.AdjustNewRow( cbar, ref cbnewrow );
			if( !blayoutrecalc )
			{
				CalcDockbarSize();
				this.LayoutDockBar();
			}

			if( cbar.rMarker == null )
				this.SetCurrentRowMarker( cbar );

			SaveBarIndex( cbar );
		}

		public void RemoveCommandBar( CommandBar cbar )
		{
			if( this.Contains( cbar ) == false )
				return;

			Rectangle rccbar = cbar.VBounds;
			CommandBar[] cbarray = this.GetRowArray( cbar.nRCIndex );
			this.Controls.Remove( cbar );
			this.RemoveBarIndex( cbar );

			if( Array.IndexOf( cbarray, cbar ) < 0 )	// The CommandBar is invisible
				return;

			if( cbarray.Length == 1 )
			{
				if( !bSuspendIndexChanges )
				{
					if( this.nRCCount > 0 )
						this.nRCCount--;

					for( int i = cbar.nRCIndex + 1; i <= this.nRCCount; i++ )
					{
						CommandBar[] cbarr = this.GetRowArray( i );
						foreach( CommandBar cb in cbarr )
						{
							cb.nRCIndex--;
						}
					}
				}

				this.CalcDockbarSize();
				this.LayoutDockBar();
			}
			else
			{
				this.CalcDockbarSize();
				AdjustPreviousRow( cbar, ref cbarray, rccbar );
			}
		}

		public CommandBarDockState GetDockBorder()
		{
			CommandBarDockState border = 0;
			switch( this.Dock )
			{
				case DockStyle.Top:
				border = CommandBarDockState.Top;
				break;
				case DockStyle.Bottom:
				border = CommandBarDockState.Bottom;
				break;
				case DockStyle.Left:
				border = CommandBarDockState.Left;
				break;
				case DockStyle.Right:
				border = CommandBarDockState.Right;
				break;
			}
			return border;
		}

		public void SetCurrentRowMarker( CommandBar cbar )
		{
			if( cbar.rMarker != null )
			{
				cbar.rMarker.DecreaseRefCount();
				cbar.rMarker = null;
			}

			// If the row has other CommandBars then obtain the RowMarker from one of the other bars and assign it to cbar
			CommandBar[] currentrowarray = this.GetRowArray( cbar.nRCIndex );
			foreach( CommandBar barsinrow in currentrowarray )
			{
				if( barsinrow == cbar )
					continue;
				cbar.rMarker = barsinrow.rMarker;
				cbar.rMarker.IncreaseRefCount();
				break;
			}

			// cbar is being added to a new row. Create a new RowMarker for this row and add it to alRowMarkers
			if( cbar.rMarker == null )
			{
				RowMarker newmarker = new RowMarker( this );
				// If the CommandDockBar has rows greater than cbar.nRCIndex, then obtain the index of
				// the next lying RowMarker and insert cbar.rMarker one position before it.
				int nextmarkerindex= this.alRowMarkers.Count;
				if( cbar.nRCIndex+1 < this.nRCCount )
				{
					CommandBar[] nextrowarray = this.GetRowArray( cbar.nRCIndex+1 );
					if( nextrowarray.Length > 0 )
					{
						nextmarkerindex = this.alRowMarkers.IndexOf( nextrowarray[0].rMarker );
					}
				}
				this.alRowMarkers.Insert( nextmarkerindex, newmarker );

				// Assign this rowmarker to all CommandBars in the cbar.nrcIndex row
				foreach( CommandBar barsinrow in currentrowarray )
				{
					if( barsinrow.rMarker != null )
					{
						barsinrow.rMarker.DecreaseRefCount();
						barsinrow.rMarker = null;
					}
					barsinrow.rMarker = newmarker;
					barsinrow.rMarker.IncreaseRefCount();
				}
			}
		}

		// Set the initial RCIndex and RowOffsets
		public void InitializeIndexRowOffsets( CommandBar cbar )
		{
			Debug.Assert( cbar.nRCIndex == -1 );
			// If there is sufficient space in the CommandDockBar's last row, then add the CommandBar
			// to the same row. Else start a new row.
			int nrow = ( this.nRCCount > 0 ) ? this.nRCCount-1 : 0;
			cbar.nRCIndex = nrow;
			if( cbar.nRowOffsetDir == -1 )
				cbar.nRowOffsetDir = 0;
			this.InitializeRowOffsets( cbar );
			if( cbar.nRowOffsetDir > 0 )	// An offset has been suggested by the CommandDockBar
			{
				if( ( this.Dock == DockStyle.Top ) || ( this.Dock == DockStyle.Bottom ) )
				{
					if( cbar.nRowOffsetDir+cbar.nMaxLength > this.Right )
					{
						cbar.nRCIndex = nrow+1;
						cbar.nRowOffsetDir = 0;
						cbar.nRowOffsetInDir = 0;
					}
				}
				else
				{
					if( cbar.nRowOffsetDir+cbar.nMaxLength > this.Bottom )
					{
						cbar.nRCIndex = nrow+1;
						cbar.nRowOffsetDir = 0;
						cbar.nRowOffsetInDir = 0;
					}
				}
			}
		}

		// Set the initial rowoffsets for the commandbars
		public void InitializeRowOffsets( CommandBar cbar )
		{
			CommandBar[] cbarray = this.GetRowArray( cbar.nRCIndex );
			if( Array.IndexOf( cbarray, cbar ) < 0 )
			{
				if( cbarray.Length > 0 )
				{
					CommandBar cblast = cbarray[cbarray.Length-1];
					if( cblast.OccupyFullRow || cbar.OccupyFullRow )
					{
						cbar.nRowOffsetDir = 0;
						cbar.nRCIndex++;
					}
					else
						cbar.nRowOffsetDir = cblast.nRowOffsetDir + cblast.nMaxLength + 2;
				}
				else
				{
					cbar.nRowOffsetDir = 0;
				}
				cbar.nRowOffsetInDir = cbar.nRowOffsetDir;
			}
		}

		protected internal void AdjustPreviousRow( CommandBar cbar, ref CommandBar[] cbarray, Rectangle rccbar )
		{
			if( m_nSuspendCount == 0 )
			{
				// Restore the rowoffsets for the remaining commandbars
				CommandBar cbarprev = null, cbarnext = null;
				int ncbarindex = Array.IndexOf( cbarray, cbar );
				if( ncbarindex > 0 )
					cbarprev = cbarray[ncbarindex - 1];
				if( ncbarindex + 1 < cbarray.Length )
					cbarnext = cbarray[ncbarindex + 1];

				int ndelta = 0;
				if( ( this.Dock == DockStyle.Top ) || ( this.Dock == DockStyle.Bottom ) )
					ndelta = cbar.Width;
				else
					ndelta = cbar.Height;

				ndelta += ( cbarnext == null ) ? 0 : 2;

				// If either the previous or the next commandbars are shrunk/wrapped, expand these first.
				if( ncbarindex > 0 )
				{
					CommandBar[] cbarrprev = new CommandBar[ncbarindex];
					Array.Copy( cbarray, 0, cbarrprev, 0, cbarrprev.Length );
					RecalcHeightForBarsInRow( false, ref cbarrprev, ref ndelta );
				}

				if( ( ndelta > 0 ) && ( ncbarindex < cbarray.Length - 1 ) )
				{
					CommandBar[] cbarrnext = new CommandBar[cbarray.Length - 1 - ncbarindex];
					Array.Copy( cbarray, ncbarindex + 1, cbarrnext, 0, cbarrnext.Length );
					RecalcHeightForBarsInRow( true, ref cbarrnext, ref ndelta );
				}

				// If ndelta is still available after the resizing, then try repositioning the previous and next bars
				if( ( this.Dock == DockStyle.Top ) || ( this.Dock == DockStyle.Bottom ) )
				{
					if( ( ndelta > 0 ) && ( cbarprev != null ) && ( cbarprev.VLeft < cbarprev.nRowOffsetDir ) &&
						( ( cbarprev.nRowOffsetDir + cbarprev.nMaxLength + 2 ) > rccbar.Left ) )
					{
						int nreqdelta = cbarprev.VLeft - cbarprev.nRowOffsetDir;
						nreqdelta = ( nreqdelta > ndelta ) ? ndelta : nreqdelta;
						ndelta = Math.Abs( ndelta - nreqdelta );
						this.AdjustRowOffsets( ref cbarray, ncbarindex, rccbar, ref nreqdelta );
					}
					if( ( ndelta > 0 ) && ( cbarnext != null ) && ( cbarnext.VLeft > cbarnext.nRowOffsetDir ) &&
						( cbarnext.nRowOffsetDir - 2 < rccbar.Right ) )
					{
						int nreqdelta = cbarnext.VLeft - cbarnext.nRowOffsetDir;
						nreqdelta = ( nreqdelta > ndelta ) ? ndelta : nreqdelta;
						this.AdjustRowOffsets( ref cbarray, ncbarindex, rccbar, ref nreqdelta );
					}
				}
				else
				{
					if( ( ndelta > 0 ) && ( cbarprev != null ) && ( cbarprev.Top < cbarprev.nRowOffsetDir ) &&
						( ( cbarprev.nRowOffsetDir + cbarprev.nMaxLength + 2 ) > rccbar.Top ) )
					{
						int nreqdelta = cbarprev.Top - cbarprev.nRowOffsetDir;
						nreqdelta = ( nreqdelta > ndelta ) ? ndelta : nreqdelta;
						ndelta = Math.Abs( ndelta - nreqdelta );
						this.AdjustRowOffsets( ref cbarray, ncbarindex, rccbar, ref nreqdelta );
					}
					if( ( ndelta > 0 ) && ( cbarnext != null ) && ( cbarnext.Top > cbarnext.nRowOffsetDir ) &&
						( cbarnext.nRowOffsetDir - 2 < rccbar.Bottom ) )
					{
						int nreqdelta = cbarnext.Top - cbarnext.nRowOffsetDir;
						nreqdelta = ( nreqdelta > ndelta ) ? ndelta : nreqdelta;
						this.AdjustRowOffsets( ref cbarray, ncbarindex, rccbar, ref nreqdelta );
					}
				}
				this.LayoutDockBar();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="next">If True - indexes of command bars in array are higher, 
		/// then index of commandbar that is current processing.</param>
		/// <param name="cbarray"></param>
		/// <param name="ndelta"></param>
		protected void RecalcHeightForBarsInRow( bool next, ref CommandBar[] cbarray, ref int ndelta )
		{
			bool bRTL = this.IsRTL;

			if( ( this.Dock == DockStyle.Top ) || ( this.Dock == DockStyle.Bottom ) )
			{
				for( int i = 0; i < cbarray.Length; i++ )
				{
					CommandBar cb = cbarray[i];
					if( ( cb.Width < cb.nMaxLength ) || ( cb.Height > cb.nCommandBarHt ) )
					{
						int nexpand = cb.nMaxLength - cb.Width;
						nexpand = ( nexpand > ndelta ) ? ndelta : nexpand;
						for( int j = cbarray.Length - 1; j >= i + 1; j-- )
							cbarray[j].Left += nexpand;

						if( cb.DockModeWrapping )
						{
							cb.SetCommandBarSize( cb.Width + nexpand, cb.Height );

							if( bRTL )
							{
								cb.Left -= nexpand;
							}
						}
						else
						{
							int nheight = this.ConfirmNewRowHeight( cb, cb.nCommandBarHt );
							if( nheight != cb.Height )
							{
								this.AdjustRowHeight( cb, nheight - cb.Height );
								cb.SetCommandBarSize( cb.Width + nexpand, nheight );

								if( bRTL )
								{
									cb.Left -= nexpand;
								}
							}
						}

						if( !next )
							ndelta -= nexpand;

						if( ndelta <= 0 )
							break;
					}
				}
			}
			else
			{
				for( int i = 0; i < cbarray.Length; i++ )
				{
					CommandBar cb = cbarray[i];
					if( ( cb.Height < cb.nMaxLength ) || ( cb.Width > cb.nCommandBarHt ) )
					{
						int nexpand = cb.nMaxLength - cb.Height;
						nexpand = ( nexpand > ndelta ) ? ndelta : nexpand;
						for( int j = cbarray.Length - 1; j >= i + 1; j-- )
							cbarray[j].Top += nexpand;

						if( cb.DockModeWrapping )
							cb.SetCommandBarSize( cb.Width, cb.Height+nexpand );
						else
						{
							int nwidth = this.ConfirmNewRowHeight( cb, cb.nCommandBarHt );
							int nOffsetX = cb.Width - nwidth;

							if( nOffsetX != 0 )
							{
								this.AdjustRowHeight( cb, nwidth - cb.Width );
								cb.SetCommandBarSize( nwidth, cb.Height + nexpand );

								if( bRTL )
								{
									cb.Left += nOffsetX;
								}
							}
						}

						if( !next )
							ndelta -= nexpand;

						if( ndelta <= 0 )
							break;
					}
				}
			}
		}

		protected internal bool AllowLeadingEdgeDocking( CommandBar cbar )
		{
			CommandBar[] cbarray = this.GetRowArray( 0 );
			foreach( CommandBar cb in cbarray )
			{
				if( cb == cbar )
					continue;
				if( cb.bLeadingEdge == true )
					return false;
			}
			return true;
		}

		protected internal bool AllowTrailingEdgeDocking( CommandBar cbar )
		{
			CommandBar[] cbarray = this.GetRowArray( this.nRCCount-1 );
			foreach( CommandBar cb in cbarray )
			{
				if( cb == cbar )
					continue;
				if( cb.bTrailingEdge == true )
					return false;
			}
			return true;
		}

		// If the commandbar is assigned a row that is occupied by some other commandbar and either of these bars has the
		// occupyfullrow property set, then adjust the commandbar index to be equal to either the previous or next row.
		protected internal bool ValidateBarIndex( CommandBar cbar, bool bappend )
		{
			CommandBar[] cbarray = this.GetRowArray( cbar.nRCIndex );

			if( cbarray.Length > 0 )
			{
				if( cbar.bLeadingEdge )
					bappend = false;
				else if( cbar.bTrailingEdge )
					bappend = true;

				bool badjustforfullrow = cbar.OccupyFullRow;
				if( !badjustforfullrow )
				{
					foreach( CommandBar fullrowbar in cbarray )
					{
						if( fullrowbar.OccupyFullRow )
						{
							badjustforfullrow = true;
							break;
						}
					}
				}

				if( badjustforfullrow )
				{
					Array.Clear( cbarray, 0, cbarray.Length );
					if( bappend )
					{
						// Performing a reverse iteration ensures that commandbars who's nrcindex have been recently incremented
						// are not grouped into the collection returned by the subsequent GetRowArray calls.
						for( int i = this.nRCCount - 1; i >= cbar.nRCIndex + 1; i-- )
						{
							cbarray = this.GetRowArray( i );
							foreach( CommandBar cbrows in cbarray )
							{
								if( cbrows == cbar )
									continue;
								cbrows.nRCIndex++;
							}
						}
						this.nRCCount++;

						cbar.nRCIndex++;
					}
					else
					{
						for( int i = this.nRCCount - 1; i >= cbar.nRCIndex; i-- )
						{
							cbarray = this.GetRowArray( i );
							foreach( CommandBar cbrows in cbarray )
							{
								if( cbrows == cbar )
									continue;

								cbrows.nRCIndex++;
							}
						}

						this.nRCCount++;
					}

					return true;	// Return TRUE to trigger a layout recalculation
				}
			}

			return false;
		}

		protected internal void CalcDockbarSize()
		{
			if( m_nSuspendCount == 0 )
			{
				int ncy = 0;
				for( int i = 0; i < this.nRCCount; i++ )
				{
					int nmaxheight = this.GetRowMaxHeight( i, false );
					int nminheight = this.GetRowMaxHeight( i, true );

					int barsCountInRow = this.GetRowArray( i ).Length;

                  	if( barsCountInRow == 1 && ( this.Dock == DockStyle.Top || this.Dock == DockStyle.Bottom ) )
						nmaxheight = nminheight;

					ncy += ( nmaxheight > nminheight ) ? nmaxheight : nminheight;
				}
				this.cbController.FreezeLayoutInternal = true;
				switch( this.Dock )
				{
					case DockStyle.Left:
					this.Size = new Size( ncy, this.Height );
					break;
					case DockStyle.Top:
					this.Size = new Size( this.Width, ncy );
					break;
					case DockStyle.Right:
					this.Size = new Size( ncy, this.Height );
					break;
					case DockStyle.Bottom:
					this.Size = new Size( this.Width, ncy );
					break;
				}
				this.cbController.FreezeLayoutInternal = false;
			}
		}

		// Used because of some activeX controls, used on container form,
		// create unpredictable delays, when activating. So we need to ensure,
		// newly added child controls created succesfully.
		private ArrayList m_arrControlsToEnsure = new ArrayList();

		private void EnsureControlsCreation()
		{
			if( m_arrControlsToEnsure.Count > 0 )
			{
				Control ctrl = null;
				for( int i = 0, len = m_arrControlsToEnsure.Count; i < len; i++ )
				{
					ctrl = m_arrControlsToEnsure[i] as Control;

					if( null != ctrl && !ctrl.IsDisposed )
					{
						// Create handle for sure
						IntPtr handle = ctrl.Handle;
					}
				}

			}
		}

		/// <summary>
		/// Updates color scheme.
		/// </summary>
		private void UpdateColorScheme()
		{
			MenuColors.UpdateMenuColors();
			Office2003Colors.UpdateMenuColors();
			VS2005Colors.UpdateMenuColors();

			Office2007Theme theme = ( cbController != null ) ? 
				cbController.Office2007Theme : Office2007Theme.Blue;
			Office2007OutlookColors.UpdateMenuColors( theme );
		}


		/// <summary>
		/// Draws themed background.
		/// </summary>
		private void DrawBackgroundThemed( Graphics g )
		{
			this.tdRebar.DrawThemeBackground( g, 6, 1, this.ClientRectangle );

			// Draw border between the adjacent rows
			Pen pn1 = new Pen( ControlPaint.LightLight( SystemColors.ControlDark ), 1 );
			Pen pn2 = new Pen( SystemColors.ControlLightLight, 1 );
			Rectangle clientRect = this.ClientRectangle;
			int nXY = 0;
			DockStyle ds = this.Dock;

			for( int i = 0; i < this.nRCCount - 1; i++ )
			{
				nXY += this.GetRowMaxHeight( i, false ) - 1;

				if( ( ds == DockStyle.Top ) || ( ds == DockStyle.Bottom ) )
				{
					g.DrawLine( pn1, clientRect.Left, nXY, clientRect.Right, nXY );
					nXY += 1;
					g.DrawLine( pn2, clientRect.Left, nXY, clientRect.Right, nXY );
				}
				else
				{
					g.DrawLine( pn1, nXY, clientRect.Top, nXY, clientRect.Bottom );
					nXY += 1;
					g.DrawLine( pn2, nXY, clientRect.Top, nXY, clientRect.Bottom );
				}
			}

			pn1.Dispose();
			pn2.Dispose();
		}


		/// <summary>
		/// Draws background for Office2003 visual style.
		/// </summary>
		private void DrawBackgroundOffice2003( Graphics g )
		{
			Rectangle clientRect = this.ClientRectangle;

			if( ( clientRect.Width > 0 ) && ( clientRect.Height > 0 ) )
			{
				if( ( this.Dock == DockStyle.Top ) || ( this.Dock == DockStyle.Bottom ) )
				{
					LinearGradientBrush backBrush = new LinearGradientBrush( clientRect, Office2003Colors.DockBarColorDark,
						Office2003Colors.DockBarColorLight, LinearGradientMode.Horizontal );
					g.FillRectangle( backBrush, clientRect );
					backBrush.Dispose();
				}
				else
				{
					LinearGradientBrush backBrush = new LinearGradientBrush( clientRect, Office2003Colors.DockBarColorDark,
						Office2003Colors.DockBarColorLight, LinearGradientMode.Vertical );
					g.FillRectangle( backBrush, clientRect );
					backBrush.Dispose();
				}
			}
		}


		/// <summary>
		/// Draws background for VS2005 visual style.
		/// </summary>
		private void DrawBackgroundVS2005( Graphics g )
		{
			Rectangle clientRect = this.ClientRectangle;

			if( ( clientRect.Width > 0 ) && ( clientRect.Height > 0 ) )
			{
				if( ( this.Dock == DockStyle.Top ) || ( this.Dock == DockStyle.Bottom ) )
				{
					LinearGradientBrush backBrush = new LinearGradientBrush( clientRect, VS2005Colors.DockBarDarkColor,
						VS2005Colors.DockBarLightColor, LinearGradientMode.Horizontal );
					g.FillRectangle( backBrush, clientRect );
					backBrush.Dispose();
				}
				else
				{
					LinearGradientBrush backBrush = new LinearGradientBrush( clientRect, VS2005Colors.DockBarDarkColor,
						VS2005Colors.DockBarLightColor, LinearGradientMode.Vertical );
					g.FillRectangle( backBrush, clientRect );
					backBrush.Dispose();
				}
			}
		}

        /// <summary>
        /// Draws background for Metro visual style.
        /// </summary>
        private void DrawBackgroundMetro(Graphics g)
        {
            Rectangle clientRect = this.ClientRectangle;
            Pen pen = new Pen(Color.FromArgb(242, 242, 242),2);
            if ((clientRect.Width > 0) && (clientRect.Height > 0))
            {
                if ((this.Dock == DockStyle.Top) || (this.Dock == DockStyle.Bottom))
                {
                    SolidBrush backBrush = new SolidBrush(Color.White);
                    g.FillRectangle(backBrush, clientRect);
                    g.DrawRectangle(pen,clientRect.X,clientRect.Y,clientRect.Width,clientRect.Height);
                    backBrush.Dispose();
                }
                else
                {
                    SolidBrush backBrush = new SolidBrush(Color.White);
                    g.FillRectangle(backBrush, clientRect);
                    g.DrawRectangle(pen, clientRect);
                    backBrush.Dispose();
                }
            }
            pen.Dispose();
        }
		/// <summary>
		/// Draws background for Office2007 visual style.
		/// </summary>
		private void DrawBackgroundOffice2007( Graphics g )
		{
			Rectangle clientRect = this.ClientRectangle;

			if( ( clientRect.Width > 0 ) && ( clientRect.Height > 0 ) )
			{
				using( SolidBrush backBrush = new SolidBrush( Office2007ColorTable.DockBarBackColor ) )
				{
					g.FillRectangle( backBrush, clientRect );
				}
			}
		}

		/// <summary>
		/// Gets color table for Office2007 visual style.
		/// </summary>
		private Office2007Colors Office2007ColorTable
		{
			get
			{
				Office2007Colors colorTable = ( cbController == null ) ? 
					Office2007Colors.Default : cbController.Office2007ColorTable;

				return colorTable;
			}
		}
        /// <summary>
        /// Draws background for Office2010 visual style.
        /// </summary>
        private void DrawBackgroundOffice2010(Graphics g)
        {
            Rectangle clientRect = this.ClientRectangle;

            if ((clientRect.Width > 0) && (clientRect.Height > 0))
            {
                using (SolidBrush backBrush = new SolidBrush(Office2010ColorTable.DockBarBackColor))
                {
                    g.FillRectangle(backBrush, clientRect);
                }
            }
        }

        /// <summary>
        /// Gets color table for Office2010 visual style.
        /// </summary>
        private Office2010Colors Office2010ColorTable
        {
            get
            {
                Office2010Colors colorTable = (cbController == null) ?
                    Office2010Colors.Default : cbController.Office2010ColorTable;

                return colorTable;
            }
        }
		protected override void OnPaint( PaintEventArgs e )
		{
			EnsureControlsCreation();
			UpdateColorScheme();

			if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.cbController.ThemesEnabled
				&& this.cbController.Style != VisualStyle.Office2007
                && this.cbController.Style != VisualStyle.Office2010
				&& this.cbController.Style != VisualStyle.Office2007Outlook )
			{
				DrawBackgroundThemed( e.Graphics );
			}
			else if( this.cbController != null )
			{
				switch( this.cbController.Style )
				{
					case VisualStyle.Office2003:
					{
						DrawBackgroundOffice2003( e.Graphics );
						break;
					}
                    case VisualStyle.Metro:
                    {
                        DrawBackgroundMetro(e.Graphics);
                        break;
                    }
					case VisualStyle.VS2005:
					{
						DrawBackgroundVS2005( e.Graphics );
						break;
					}
					case VisualStyle.Office2007Outlook:
					case VisualStyle.Office2007:
					{
						DrawBackgroundOffice2007( e.Graphics );
						break;
					}
                    case VisualStyle.Office2010:
                    {
                        DrawBackgroundOffice2010(e.Graphics);
                        break;
                    }
				}
			}

			base.OnPaint( e );
		}

		protected override void OnSizeChanged( EventArgs e )
		{
			base.OnSizeChanged( e );

			if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.cbController.ThemesEnabled )
			{
				this.Invalidate( true );
			}
		}

		protected override void OnSystemColorsChanged( EventArgs e )
		{
			base.OnSystemColorsChanged( e );
			MenuColors.SysColorsChanged( false );
			Office2003Colors.SysColorsChanged( false );
		}

		protected override void SetBoundsCore( int x, int y, int width, int height, BoundsSpecified specified )
		{
            if ((this.Left != x) || (this.Top != y) || (this.Width != width) || (this.Height != height))
            {
                if (null != this.cbController && !this.cbController.FreezeLayout)
                {
                    int nOffsetX = width - this.Width;

                    if ((this.Dock == DockStyle.Left) || (this.Dock == DockStyle.Right))
                    {
                        if ((this.Height > 0) && (height > this.Height))
                            this.bExpanding = true;
                    }
                    else	// DockStyle.Top||DockStyle.Bottom
                    {
                        if ((this.Width > 0) && (width > this.Width))
                            this.bExpanding = true;
                    }
                    base.SetBoundsCore(x, y, width, height, specified);
                    bool bvisiblebars = false;
                    foreach (CommandBar cbar in this.Controls)
                    {
                        if (cbar.Visible)
                        {
                            bvisiblebars = true;
                            break;
                        }
                    }
                    if ((bvisiblebars) && (this.cbController.HostForm.WindowState != FormWindowState.Minimized))
                    {
                        if (this.IsRTL && (DockStyle.Top == this.Dock || DockStyle.Bottom == this.Dock))
                        {
                            for (int iRow = 0; iRow < this.nRCCount; ++iRow)
                            {
                                CommandBar[] acbCBars = this.GetRowArray(iRow);
                                foreach (CommandBar cb in acbCBars)
                                {
                                    cb.Left += nOffsetX;
                                }
                            }
                        }

                        this.LayoutDockBar();
                        this.Invalidate(false);
                    }
                    this.bExpanding = false;
                }
            }
		}

		protected override void OnControlAdded( ControlEventArgs e )
		{
			base.OnControlAdded( e );
			e.Control.TabIndexChanged += new EventHandler( this.Child_TabIndexChanged );
			this.ResetTabIndex();

			// Add control to array of controls needed to ensure their succesfull creation in OnPaint processing
			if( !m_arrControlsToEnsure.Contains( e.Control ) )
			{
				m_arrControlsToEnsure.Add( e.Control );
			}
		}

		protected override void OnControlRemoved( ControlEventArgs e )
		{
			base.OnControlAdded( e );
			e.Control.TabIndexChanged -= new EventHandler( this.Child_TabIndexChanged );
			this.ResetTabIndex();
			m_arrControlsToEnsure.Clear();
		}

		protected void Child_TabIndexChanged( Object sender, EventArgs e )
		{
			this.ResetTabIndex();
		}

		protected virtual void ResetTabIndex()
		{
			int ntabindex = 1000;	// Arbitrary big number
			foreach( CommandBar cbar in this.Controls )
			{
				if( ( cbar.Visible == true ) && ( cbar.TabIndex < ntabindex ) )
					ntabindex = cbar.TabIndex;
			}
			this.TabIndex = ntabindex;
		}

		public virtual void LayoutDockBar()
		{
			if( m_nSuspendCount == 0 )
			{
				if( this.cbController.FreezeLayoutInternal || this.cbController.FreezeLayout )
					return;

				int nyoff = 0;
				Rectangle rcclient = this.ClientRectangle;
				for( int rcindex = 0; rcindex < this.nRCCount; rcindex++ )
				{
					CommandBar[] cbarray = this.GetRowArray( rcindex );
					if( cbarray.Length == 0 )
						continue;

					int nreqdlength = 0;
					bool binwrap = false;
					for( int i = 0; i < cbarray.Length; i++ )
					{
						int npadding = ( i == cbarray.Length - 1 ) ? 0 : 2;
						CommandBar cb = cbarray[i];
						if( !cb.OccupyFullRow )
						{
							if( !binwrap )
							{
								if( nreqdlength > cb.nRowOffsetInDir )
								{
									// Prevents a bar from running over the previous one
									cb.nRowOffsetInDir = nreqdlength;
									if( ( this.Dock == DockStyle.Top ) || ( this.Dock == DockStyle.Bottom ) )
										cb.VLeft = cb.nRowOffsetInDir;
									else
										cb.Top = cb.nRowOffsetInDir;
								}
								nreqdlength = cb.nRowOffsetInDir + cb.nMaxLength + npadding;
							}
							else
							{
								nreqdlength += ( cb.nRowOffsetInDir + cb.nMaxLength + npadding );
							}
						}
						else
						{
							if( ( this.Dock == DockStyle.Top ) || ( this.Dock == DockStyle.Bottom ) )
								nreqdlength = this.Width;
							else
								nreqdlength = this.Height;
							cb.nRowOffsetDir = 0;
							cb.nRowOffsetInDir = 0;
						}

						if( !binwrap )
						{
							if( ( this.Dock == DockStyle.Top ) || ( this.Dock == DockStyle.Bottom ) )
							{
								if( ( cb.Width < cb.nMaxLength ) && ( cb.DockModeWrapping ) )
									binwrap = true;
							}
							else
							{
								if( ( cb.Height < cb.nMaxLength ) && ( cb.DockModeWrapping ) )
									binwrap = true;
							}
						}
					}

					if( ( this.Dock == DockStyle.Top ) || ( this.Dock == DockStyle.Bottom ) )
					{
						if( rcclient.Width > 0 )
							nreqdlength = nreqdlength - rcclient.Width;
						else
							nreqdlength = 0;
					}
					else
					{
						if( rcclient.Height > 0 )
							nreqdlength = nreqdlength - rcclient.Height;
						else
							nreqdlength = 0;
					}

					if( nreqdlength > 0 )
					{
						// The last commandbar in the row cannot be accomodated in full; repositioning/resizing has to take place.
						// First attempt repositioning If this fails to satisfy the size deficit then resort to resizing.
						if( ( this.Dock == DockStyle.Top ) || ( this.Dock == DockStyle.Bottom ) )
						{
							if( this.bExpanding )
							{
								// When the dockbar is expanding, CommandBars should be repositioned only if all bars
								// have been sized to their maxlengths
								int nreqdbarlength = 0;
								foreach( CommandBar bar in cbarray )
									nreqdbarlength += bar.nMaxLength + 2;
								if( nreqdbarlength <= this.Width )
									this.RepositionBars( ref cbarray, nreqdlength, nyoff );
							}
							else
								this.RepositionBars( ref cbarray, nreqdlength, nyoff );

							int ntotcurrwidth = 0;
							for( int i = 0; i < cbarray.Length; i++ )
							{
								int npadding = ( i == cbarray.Length - 1 ) ? 0 : 2;
								ntotcurrwidth += cbarray[i].Width + npadding;
							}
							nreqdlength = ntotcurrwidth - rcclient.Width;
							ResizeBarsTopBottom( ref cbarray, nreqdlength, nyoff );
						}
						else	//DockStyle.Left||DockStyle.Right
						{
							if( this.bExpanding )
							{
								// When the dockbar is expanding, CommandBars should be repositioned only if all bars
								// have been sized to their maxlengths
								int nreqdbarlength = 0;
								foreach( CommandBar bar in cbarray )
									nreqdbarlength += bar.nMaxLength;
								if( nreqdbarlength <= this.Height )
									this.RepositionBars( ref cbarray, nreqdlength, nyoff );
							}
							else
								this.RepositionBars( ref cbarray, nreqdlength, nyoff );

							int ntotcurrwidth = 0;
							for( int i = 0; i < cbarray.Length; i++ )
							{
								int npadding = ( i == cbarray.Length - 1 ) ? 0 : 2;
								ntotcurrwidth += cbarray[i].Height + npadding;
							}
							nreqdlength = ntotcurrwidth - rcclient.Height;
							ResizeBarsLeftRight( ref cbarray, nreqdlength, nyoff );
						}
					}
					else
					{
						for( int i = 0; i < cbarray.Length; i++ )
						{
							CommandBar cbar = cbarray[i];
							int nrowmaxheight = this.GetRowMaxHeight( cbar.nRCIndex, true );
							if( ( this.Dock == DockStyle.Top ) || ( this.Dock == DockStyle.Bottom ) )
							{
								int nallowedlen = 0;
								if( !cbar.OccupyFullRow )
								{
									nallowedlen = cbar.nMaxLength;
									if( binwrap )
									{
										// Move all the bars that lie after cbar in this row
										if( i + 1 < cbarray.Length )
										{
											int nmove = Math.Abs( nallowedlen - cbar.Width );
											for( int nindex = i + 1; nindex < cbarray.Length; nindex++ )
												cbarray[nindex].VLeft += nmove;
										}
									}
								}
								else
								{
									nallowedlen = this.Width;
								}

								cbar.SetCommandBarSize( nallowedlen, ( ( cbar.Height > nrowmaxheight ) ? cbar.Height : nrowmaxheight ) );
								cbar.VLocation = new Point( cbar.nRowOffsetInDir, nyoff );
							}
							else
							{
								int nallowedlen = 0;
								if( !cbar.OccupyFullRow )
								{
									nallowedlen = cbar.nMaxLength;
									if( binwrap )
									{
										if( i + 1 < cbarray.Length )
										{
											int nmove = Math.Abs( nallowedlen - cbar.Height );
											for( int nindex = i + 1; nindex < cbarray.Length; nindex++ )
												cbarray[nindex].Top += nmove;
										}
									}
								}
								else
								{
									nallowedlen = this.Height;
								}

								cbar.SetCommandBarSize( ( ( cbar.Width > nrowmaxheight ) ? cbar.Width : nrowmaxheight ), nallowedlen );
								cbar.VLocation = new Point( nyoff, cbar.nRowOffsetInDir );
							}
						}
					}

					if( ( this.Dock == DockStyle.Top ) || ( this.Dock == DockStyle.Bottom ) )
						nyoff = cbarray[0].Bounds.Bottom;
					else
						nyoff = cbarray[0].Bounds.Right;
				}

				this.EnsureCorrectLocation();
			}
		}

		private void EnsureCorrectLocation()
		{
			ArrayList rows = new ArrayList();

			for( int i = 0; i < this.nRCCount; i++ )
			{
				CommandBar[] cbarray = this.GetRowArray( i );

				if( cbarray.Length == 0 )
					continue;

				int loc = 0;
				for( int j = 0; j < rows.Count; j++ )
				{
					loc += this.GetRowMaxHeight( (int)rows[j], false );
				}

				if( this.Dock == DockStyle.Top || this.Dock == DockStyle.Bottom )
				{
					foreach( CommandBar cb in cbarray )
					{
						if( cb.Location.Y != loc )
						{
							cb.Location = new Point( cb.Location.X, loc );
						}
					}
				}
				else if( this.Dock == DockStyle.Left || this.Dock == DockStyle.Right )
				{
					foreach( CommandBar cb in cbarray )
					{
						if( cb.Location.X != loc )
						{
							cb.Location = new Point( loc, cb.Location.Y );
						}
					}
				}

				rows.Add( i );
			}
		}

		protected virtual void RepositionBars( ref CommandBar[] cbarray, int nreqdlength, int nyoff )
		{
			CommandBar cbarlast = cbarray[cbarray.Length-1];
			CommandBar cbarprev = null;
			if( cbarray.Length >= 2 )
				cbarprev = cbarray[cbarray.Length-2];
			if( ( this.Dock == DockStyle.Top ) || ( this.Dock == DockStyle.Bottom ) )
			{
				if( cbarprev != null )
				{
					if( cbarlast.nRowOffsetInDir-nreqdlength > cbarprev.VRight+2 )
					{
						if( ( this.bExpanding == true ) && ( cbarlast.nRowOffsetInDir-nreqdlength > cbarlast.nRowOffsetInDir ) )
							cbarlast.VLocation = new Point( cbarlast.nRowOffsetInDir, nyoff );
						else
							cbarlast.VLocation = new Point( cbarlast.nRowOffsetInDir-nreqdlength, nyoff );

						if( ( cbarlast.VLeft > cbarprev.VRight+2 ) && ( cbarprev.VLeft < cbarprev.nRowOffsetInDir ) )
						{
							// Client rect is being increased
							nreqdlength = ( cbarprev.VRight+2 )-cbarlast.VLeft;
							RepositionBarsRToLShift( ref cbarray, cbarray.Length-2, ref nreqdlength, nyoff );
						}
						else
							nreqdlength = 0;
					}
					else
					{
						nreqdlength -= cbarlast.nRowOffsetInDir-cbarlast.VLeft;
						RepositionBarsRToLShift( ref cbarray, cbarray.Length-2, ref nreqdlength, nyoff );
						cbarlast.VLocation = new Point( cbarprev.VRight+2, nyoff );
					}
				}
				else
				{
					if( cbarlast.nRowOffsetInDir-nreqdlength >= this.ClientRectangle.Left )
					{
						if( ( this.bExpanding == true ) && ( cbarlast.nRowOffsetInDir-nreqdlength > cbarlast.nRowOffsetInDir ) )
							cbarlast.VLocation = new Point( cbarlast.nRowOffsetInDir, nyoff );
						else
							cbarlast.VLocation = new Point( cbarlast.nRowOffsetInDir-nreqdlength, nyoff );
						nreqdlength = 0;
					}
					else
					{
						cbarlast.VLocation = new Point( this.ClientRectangle.Left, nyoff );
						nreqdlength -= ( cbarlast.nRowOffsetInDir - this.ClientRectangle.Left );
					}
				}
			}
			else	//DockStyle.Left|DockStyle.Right
			{
				if( cbarprev != null )
				{
					if( cbarlast.nRowOffsetInDir-nreqdlength > cbarprev.Bottom+2 )
					{
						if( ( this.bExpanding == true ) && ( cbarlast.nRowOffsetInDir-nreqdlength > cbarlast.nRowOffsetInDir ) )
							cbarlast.Location = new Point( nyoff, cbarlast.nRowOffsetInDir );
						else
							cbarlast.Location = new Point( nyoff, cbarlast.nRowOffsetInDir-nreqdlength );

						if( ( cbarlast.Location.Y > cbarprev.Bottom+2 ) && ( cbarprev.Top < cbarprev.nRowOffsetInDir ) )
						{
							// Client rect is being increased
							nreqdlength = ( cbarprev.Bottom+2 )-cbarlast.Top;
							RepositionBarsBToTShift( ref cbarray, cbarray.Length-2, ref nreqdlength, nyoff );
						}
						else
							nreqdlength = 0;
					}
					else
					{
						nreqdlength -= cbarlast.nRowOffsetInDir-cbarlast.Top;
						RepositionBarsBToTShift( ref cbarray, cbarray.Length-2, ref nreqdlength, nyoff );
						cbarlast.Location = new Point( nyoff, cbarprev.Bottom+2 );
					}
				}
				else
				{
					if( cbarlast.nRowOffsetInDir-nreqdlength >= this.ClientRectangle.Top )
					{
						if( ( this.bExpanding == true ) && ( cbarlast.nRowOffsetInDir-nreqdlength > cbarlast.nRowOffsetInDir ) )
							cbarlast.Location = new Point( nyoff, cbarlast.nRowOffsetInDir );
						else
							cbarlast.Location = new Point( nyoff, cbarlast.nRowOffsetInDir-nreqdlength );
						nreqdlength = 0;
					}
					else
					{
						cbarlast.Location = new Point( nyoff, this.ClientRectangle.Top );
						nreqdlength -= ( cbarlast.nRowOffsetInDir - this.ClientRectangle.Left );
					}
				}
			}
		}

		protected virtual void RepositionBarsRToLShift( ref CommandBar[] cbarray, int i, ref int nreqdlength, int nyoff )
		{
			CommandBar cbar = cbarray[i];
			CommandBar cbarprev = null;
			if( i > 0 )
				cbarprev = cbarray[i-1];
			if( cbarprev != null )
			{
				if( cbar.VLeft-nreqdlength > cbarprev.VRight+2 )
				{
					if( ( this.bExpanding == true ) && ( cbar.VLeft-nreqdlength > cbar.nRowOffsetInDir ) )
						cbar.VLocation = new Point( cbar.nRowOffsetInDir, nyoff );
					else
						cbar.VLocation = new Point( cbar.VLeft-nreqdlength, nyoff );
					if( ( cbar.VLeft > cbarprev.VRight+2 ) && ( cbarprev.VLeft < cbarprev.nRowOffsetInDir ) )	// Expanding
					{
						// Client Rectangle is expanding
						nreqdlength = ( cbarprev.VRight+2 )-cbar.VLeft;
						RepositionBarsRToLShift( ref cbarray, i-1, ref nreqdlength, nyoff );
					}
					nreqdlength = 0;
				}
				else
				{
					nreqdlength -= ( cbar.VLeft - ( cbarprev.VRight+2 ) );
					RepositionBarsRToLShift( ref cbarray, i-1, ref nreqdlength, nyoff );
					cbar.VLocation = new Point( cbarprev.VRight+2, nyoff );
				}
			}
			else // (cbarprev == null)
			{
				if( cbar.VLeft-nreqdlength > this.ClientRectangle.Left )
				{
					if( ( this.bExpanding == true ) && ( cbar.VLeft-nreqdlength > cbar.nRowOffsetInDir ) )
						cbar.VLocation = new Point( cbar.nRowOffsetInDir, nyoff );
					else
						cbar.VLocation = new Point( cbar.VLeft-nreqdlength, nyoff );
					nreqdlength = 0;
				}
				else
				{
					nreqdlength -= ( cbar.VLeft - this.ClientRectangle.Left );
					cbar.VLocation = new Point( this.ClientRectangle.Left, nyoff );
				}
			}
		}

		protected virtual void RepositionBarsLToRShift( ref CommandBar[] cbarray, int i, ref int nreqdlength, int nyoff )
		{
			CommandBar cbar = cbarray[i];
			CommandBar cbarnext = null;
			if( i+1 < cbarray.Length )
				cbarnext = cbarray[i+1];
			if( cbarnext != null )
			{
				if( cbar.VRight-nreqdlength < cbarnext.VLeft-2 )
				{
					cbar.Location = new Point( cbar.VLeft-nreqdlength, nyoff );
					nreqdlength = 0;
				}
				else
				{
					nreqdlength -= ( ( cbarnext.VLeft-2 ) - cbar.VRight );
					RepositionBarsLToRShift( ref cbarray, i+1, ref nreqdlength, nyoff );
					cbar.VLocation = new Point( cbarnext.VLeft-2-cbar.Width, nyoff );
				}
			}
			else // (cbarnext == null)
			{
				if( cbar.VRight-nreqdlength < this.ClientRectangle.Right )
				{
					cbar.VLocation = new Point( cbar.VLeft-nreqdlength, nyoff );
					nreqdlength = 0;
				}
				else
				{
					nreqdlength -= this.ClientRectangle.Right - cbar.VRight;
					cbar.VLocation = new Point( this.ClientRectangle.Right-cbar.Width, nyoff );
				}
			}
		}

		protected virtual void RepositionBarsBToTShift( ref CommandBar[] cbarray, int i, ref int nreqdlength, int nyoff )
		{
			CommandBar cbar = cbarray[i];
			CommandBar cbarprev = null;
			if( i > 0 )
				cbarprev = cbarray[i-1];
			if( cbarprev != null )
			{
				if( cbar.Top-nreqdlength > cbarprev.Bottom+2 )
				{
					if( ( this.bExpanding == true ) && ( cbar.Top-nreqdlength > cbar.nRowOffsetInDir ) )
						cbar.Location = new Point( nyoff, cbar.nRowOffsetInDir );
					else
						cbar.Location = new Point( nyoff, cbar.Top-nreqdlength );
					if( ( cbar.Location.Y > cbarprev.Bottom+2 ) && ( cbarprev.Top < cbarprev.nRowOffsetInDir ) )	// Expanding
					{
						// Client Rectangle is expanding
						nreqdlength = ( cbarprev.Bottom+2 )-cbar.Top;
						RepositionBarsBToTShift( ref cbarray, i-1, ref nreqdlength, nyoff );
					}
					nreqdlength = 0;
				}
				else
				{
					nreqdlength -= ( cbar.Top - ( cbarprev.Bottom+2 ) );
					RepositionBarsBToTShift( ref cbarray, i-1, ref nreqdlength, nyoff );
					cbar.Location = new Point( nyoff, cbarprev.Bottom+2 );
				}
			}
			else // (cbarprev == null)
			{
				if( cbar.Top-nreqdlength > this.ClientRectangle.Top )
				{
					if( ( this.bExpanding == true ) && ( cbar.Top-nreqdlength > cbar.nRowOffsetInDir ) )
						cbar.Location = new Point( nyoff, cbar.nRowOffsetInDir );
					else
						cbar.Location = new Point( nyoff, cbar.Top-nreqdlength );
					nreqdlength = 0;
				}
				else
				{
					nreqdlength -= ( cbar.Top - this.ClientRectangle.Top );
					cbar.Location = new Point( nyoff, this.ClientRectangle.Top );
				}
			}
		}

		protected virtual void RepositionBarsTToBShift( ref CommandBar[] cbarray, int i, ref int nreqdlength, int nyoff )
		{
			CommandBar cbar = cbarray[i];
			CommandBar cbarnext = null;
			if( i+1 < cbarray.Length )
				cbarnext = cbarray[i+1];
			if( cbarnext != null )
			{
				if( cbar.Bottom-nreqdlength < cbarnext.Top-2 )
				{
					cbar.Location = new Point( nyoff, cbar.Top-nreqdlength );
					nreqdlength = 0;
				}
				else
				{
					nreqdlength -= ( ( cbarnext.Top-2 ) - cbar.Bottom );
					RepositionBarsTToBShift( ref cbarray, i+1, ref nreqdlength, nyoff );
					cbar.Location = new Point( nyoff, cbarnext.Top-2-cbar.Height );
				}
			}
			else // (cbarnext == null)
			{
				if( cbar.Bottom-nreqdlength < this.ClientRectangle.Bottom )
				{
					cbar.Location = new Point( nyoff, cbar.Top-nreqdlength );
					nreqdlength = 0;
				}
				else
				{
					nreqdlength -= this.ClientRectangle.Bottom - cbar.Bottom;
					cbar.Location = new Point( nyoff, this.ClientRectangle.Bottom-cbar.Height );
				}
			}
		}

		protected virtual void ResizeBarsTopBottom( ref CommandBar[] cbarray, int nreqdlength, int nyoff )
		{
			bool bRTL = this.IsRTL;

			if( nreqdlength > 0 )	// Reducing in size
			{
				for( int i=cbarray.Length-1; i>=0; i-- )
				{
					CommandBar cbi = cbarray[i];
					if( cbi.Width > cbi.nMinLength )
					{
						int nbarmaxchange = cbi.Width - cbi.nMinLength;
						if( nreqdlength <= nbarmaxchange )
						{
							if( cbi.SetCommandBarSize( cbi.Width-nreqdlength, cbi.Height ) )
							{
								if( bRTL )
								{
									cbi.Left += nreqdlength;
								}

								for( int j=i+1; j<cbarray.Length; j++ )
								{
									CommandBar cbj = cbarray[j];
									cbj.VLocation = new Point( cbj.VLeft-nreqdlength, nyoff );
								}
							}
							nreqdlength = 0;
							break;
						}
						else
						{
							nreqdlength -= nbarmaxchange;

							int nWidth = cbi.Width;

							if( cbi.SetCommandBarSize( cbi.nMinLength, cbi.Height ) )
							{
								if( bRTL )
								{
									cbi.Left += cbi.nMinLength-nWidth;
								}

								for( int j=i+1; j<cbarray.Length; j++ )
								{
									CommandBar cbj = cbarray[j];
									cbj.VLocation = new Point( cbj.VLeft-nbarmaxchange, nyoff );
								}
							}
						}
					}
				}
			}
			else	// Increasing in size
			{
				nreqdlength = -nreqdlength;
				for( int i=0; i<cbarray.Length; i++ )
				{
					CommandBar cbi = cbarray[i];

					if( cbi.Width < cbi.nMaxLength )
					{
						int nbarmaxchange = cbi.nMaxLength - cbi.Width;
						if( nreqdlength <= nbarmaxchange )
						{
							if( cbi.SetCommandBarSize( cbi.Width+nreqdlength, cbi.Height ) )
							{
								if( bRTL )
								{
									cbi.Left -= nreqdlength;
								}

								// If the next CommandBar is located within the new bounds of cbi, then move all
								// adjacent bars by 'nreqdlength' extent.
								if( ( i+1 ) < cbarray.Length )
								{
									CommandBar cbnext = cbarray[i+1];
									if( cbnext.VLeft < cbi.VRight+2 )
									{
										int nmoveby = ( cbi.VRight+2 ) - cbnext.VLeft;
										for( int j=cbarray.Length-1; j>=i+1; j-- )
										{
											CommandBar cbj = cbarray[j];
											cbj.VLocation = new Point( cbj.VLeft+nmoveby, nyoff );
										}
									}
								}
							}
						}
						else
						{
							nreqdlength -= nbarmaxchange;

							int nWidth = cbi.Width;

							if( cbi.SetCommandBarSize( cbi.nMaxLength, cbi.Height ) )
							{
								if( bRTL )
								{
									cbi.Left += cbi.nMaxLength-nWidth;
								}

								// If the next CommandBar is located within the new bounds of cbi, then move all
								// adjacent bars by 'nreqdlength' extent.
								if( ( i+1 ) < cbarray.Length )
								{
									CommandBar cbnext = cbarray[i+1];
									if( cbnext.VLeft < cbi.VRight+2 )
									{
										int nmoveby = ( cbi.VRight+2 ) - cbnext.VLeft;
										for( int j=cbarray.Length-1; j>=i+1; j-- )
										{
											CommandBar cbj = cbarray[j];
											cbj.VLocation = new Point( cbj.VLeft+nmoveby, nyoff );
										}
									}
								}
							}
						}
					}
				}
			}
		}

		protected virtual void ResizeBarsLeftRight( ref CommandBar[] cbarray, int nreqdlength, int nyoff )
		{
			if( nreqdlength > 0 )	// Reducing in size
			{
				for( int i=cbarray.Length-1; i>=0; i-- )
				{
					CommandBar cbi = cbarray[i];
					if( cbi.Height > cbi.nMinLength )
					{
						int nbarmaxchange = cbi.Height - cbi.nMinLength;
						if( nreqdlength <= nbarmaxchange )
						{
							if( cbi.SetCommandBarSize( cbi.Width, cbi.Height-nreqdlength ) == true )
							{
								for( int j=i+1; j<cbarray.Length; j++ )
								{
									CommandBar cbj = cbarray[j];
									cbj.Location = new Point( nyoff, cbj.Location.Y-nreqdlength );
								}
							}
							nreqdlength = 0;
							break;
						}
						else
						{
							nreqdlength -= nbarmaxchange;
							if( cbi.SetCommandBarSize( cbi.Width, cbi.nMinLength ) == true )
							{
								for( int j=i+1; j<cbarray.Length; j++ )
								{
									CommandBar cbj = cbarray[j];
									cbj.Location = new Point( nyoff, cbj.Location.Y-nbarmaxchange );
								}
							}
						}
					}
				}
			}
			else	// Increasing in size
			{
				nreqdlength = -nreqdlength;
				for( int i=0; i<cbarray.Length; i++ )
				{
					CommandBar cbi = cbarray[i];
					if( cbi.Height < cbi.nMaxLength )
					{
						int nbarmaxchange = cbi.nMaxLength - cbi.Height;
						if( nreqdlength <= nbarmaxchange )
						{
							if( cbi.SetCommandBarSize( cbi.Width, cbi.Height+nreqdlength ) == true )
							{
								// If the next CommandBar is located within the new bounds of cbi, then move all
								// adjacent bars by 'nreqdlength' extent.
								if( ( i+1 ) < cbarray.Length )
								{
									CommandBar cbnext = cbarray[i+1];
									if( cbnext.Top < cbi.Bottom+2 )
									{
										int nmoveby = ( cbi.Bottom+2 ) - cbnext.Top;
										for( int j=cbarray.Length-1; j>=i+1; j-- )
										{
											CommandBar cbj = cbarray[j];
											cbj.Location = new Point( nyoff, cbj.Location.Y+nmoveby );
										}
									}
								}
							}
							nreqdlength = 0;
							break;
						}
						else
						{
							nreqdlength -= nbarmaxchange;
							if( cbi.SetCommandBarSize( cbi.Width, cbi.nMaxLength ) == true )
							{
								// If the next CommandBar is located within the new bounds of cbi, then move all
								// adjacent bars by 'nreqdlength' extent.
								if( ( i+1 ) < cbarray.Length )
								{
									CommandBar cbnext = cbarray[i+1];
									if( cbnext.Top < cbi.Bottom+2 )
									{
										int nmoveby = ( cbi.Bottom+2 ) - cbnext.Top;
										for( int j=cbarray.Length-1; j>=i+1; j-- )
										{
											CommandBar cbj = cbarray[j];
											cbj.Location = new Point( nyoff, cbj.Location.Y+nmoveby );
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public CommandBar[] GetRowArray( int nrow )
		{
			ArrayList cbarraylist = new ArrayList();
			foreach( CommandBar cbar in this.Controls )
			{
				if( cbar.Visible && ( cbar.nRCIndex == nrow ) )
				{
					int inspos = cbarraylist.Count;
					foreach( CommandBar cb in cbarraylist )
					{
						if( cb.nRowOffsetInDir > cbar.nRowOffsetInDir )
						{
							inspos = cbarraylist.IndexOf( cb );
							break;
						}
					}
					cbarraylist.Insert( inspos, cbar );
				}
			}
			return (CommandBar[])cbarraylist.ToArray( typeof( CommandBar ) );
		}

		public int GetRowMaxHeight( int nrow, bool bmin )
		{
			if( ( nrow >= 0 ) && ( nrow < this.nRCCount ) )
			{
				CommandBar[] cbarray = this.GetRowArray( nrow );
				int nmaxheight = 0;
				foreach( CommandBar cb in cbarray )
				{
					if( bmin )
					{
                        if ((this.Dock == DockStyle.Top) || (this.Dock == DockStyle.Bottom))
                        {
                            if (cb.Height > nmaxheight)
                                nmaxheight = cb.Height;
                        }
                        else
                        {
                            if (cb.Width > nmaxheight)
                                nmaxheight = cb.Width;
                        } 
						if( cb.nCommandBarHt > nmaxheight )
							nmaxheight = cb.nCommandBarHt;
					}
					else
					{
						if( ( this.Dock == DockStyle.Top ) || ( this.Dock == DockStyle.Bottom ) )
						{
							if( cb.Height > nmaxheight )
								nmaxheight = cb.Height;
						}
						else
						{
							if( cb.Width > nmaxheight )
								nmaxheight = cb.Width;
						}
					}
				}
				return nmaxheight;
			}
			return CommandBar.nDefaultUnitHt;
		}

		public Point HandleDragOver( CommandBar cbar, Point ptscreen )
		{
			if( ( this.Dock == DockStyle.Top ) || ( this.Dock == DockStyle.Bottom ) )
			{
				if( this.IsRTL )
				{
					ptscreen = this.PointToClient( ptscreen );
					ptscreen.X = this.ClientRectangle.Width - ptscreen.X;
					ptscreen = this.PointToScreen( ptscreen );
				}

				return this.HandleDragOverTopBottom( cbar, ptscreen );
			}
			else
				return this.HandleDragOverLeftRight( cbar, ptscreen );
		}

		protected Point HandleDragOverTopBottom( CommandBar cbar, Point ptscreen )
		{
			Point ptclient = this.PointToClient( ptscreen );
			CommandBar[] cbarray = this.GetRowArray( cbar.nRCIndex );
			if( this.IsRTL )
			{
				//				ptclient.X -= cbar.Width;
			}

			// If the commandbar is being dragged from the containing row/column into some
			// other row/col, then reinsert at that position.
			if( ptclient.Y > cbar.Bottom )
			{
				CommandBar[] cbnewrow = this.GetRowArray( cbar.nRCIndex+1 );

				if( cbnewrow.Length > 0 )
				{
					CommandBar cb = cbnewrow[0];
					if( cb.bTrailingEdge && cb.bFullRow )
						return new Point( -1, -1 );

					// The drag point should lie beyond atleast half the height of the next row for a reinsert to take place
					if( ptclient.Y < cb.Top + cb.Height / 2 )
						return new Point( -1, -1 );
				}

				if( cbar.bLeadingEdge && ( ( cbarray.Length > 1 ) || ( cbnewrow[0].bFullRow ) ) )
					return new Point( -1, -1 );

				int nindex = cbar.nRCIndex;
				cbar.nRCIndex++;
				if( cbarray.Length == 1 )
				{
					this.nRCCount--;
					foreach( CommandBar cb in this.Controls )
					{
						if( cb.Visible && ( cb.nRCIndex >= nindex ) )
						{
							cb.nRCIndex--;
						}
					}
					this.CalcDockbarSize();
				}

				if( this.ValidateBarIndex( cbar, true ) )
				{
					this.CalcDockbarSize();
					this.LayoutDockBar();
				}

				if( !cbar.OccupyFullRow )
				{
					this.AdjustPreviousRow( cbar, ref cbarray, cbar.VBounds );
					if( ( cbnewrow.Length > 0 ) && !cbnewrow[0].OccupyFullRow )
						this.AdjustNewRow( cbar, ref cbnewrow );
					cbar.nRowOffsetInDir = cbar.VLeft;
					cbar.nRowOffsetDir = cbar.nRowOffsetInDir;
				}

				return new Point( -1, -1 );
			}
			else if( ptclient.Y < cbar.Top )
			{
				CommandBar[] cbnewrow = this.GetRowArray( cbar.nRCIndex - 1 );

				if( cbnewrow.Length > 0 )
				{
					CommandBar cb = cbnewrow[0];
					if( cb.bLeadingEdge && cb.bFullRow )
						return new Point( -1, -1 );

					// The drag point should lie beyond atleast half the height of the previous row for a reinsert to take place
					if( ptclient.Y > cb.Bottom - cb.Height / 2 )
						return new Point( -1, -1 );
				}

				if( cbar.bTrailingEdge && ( ( cbarray.Length > 1 ) || cbnewrow[0].bFullRow ) )
					return new Point( -1, -1 );

				int nindex = cbar.nRCIndex;
				cbar.nRCIndex--;

				if( cbarray.Length == 1 )
				{
					this.nRCCount--;
					foreach( CommandBar cb in this.Controls )
					{
						if( cb.Visible && ( cb.nRCIndex >= nindex ) )
						{
							cb.nRCIndex--;
						}
					}
					this.CalcDockbarSize();
				}

				if( this.ValidateBarIndex( cbar, false ) )
				{
					this.CalcDockbarSize();
					this.LayoutDockBar();
				}

				if( !cbar.OccupyFullRow )
				{
					this.AdjustPreviousRow( cbar, ref cbarray, cbar.VBounds );
					if( ( cbnewrow.Length > 0 ) && !cbnewrow[0].OccupyFullRow )
						this.AdjustNewRow( cbar, ref cbnewrow );
					cbar.nRowOffsetInDir = cbar.VLeft;
					cbar.nRowOffsetDir = cbar.nRowOffsetInDir;
				}

				return new Point( -1, -1 );
			}

			// If any of the bars are sized-down or wrapped, then disallow sliding
			bool binsizing = false;
			foreach( CommandBar cb in cbarray )
			{
				if( cb.Width < cb.nMaxLength )
				{
					if( cb.bDockModeWrapping )
						return new Point( -1, -1 );
					else
						binsizing = true;
				}
			}

			// If the commandbar butts against an existing commandbar, try moving the other bar. If this succeeds return
			// true and the new commandbar will be located in this position. However if there is insufficient space to
			// accomodate the commandbar in the newposition return false.
			CommandBar[] cbdisplaced;
			int ncbarpos = Array.IndexOf( cbarray, cbar );
			int ndeltax = cbar.VLeft - ptclient.X;

			// If ndeltax is positive, ie., sliding to the left, then form an array of bars that contains all commandbars that
			// lie before the dragged bar. Else, populate array with bars that follow the dragged bar.
			if( ( ndeltax > 0 ) && ( ptclient.X <= ( cbar.VLeft + cbar.nHeaderOff ) ) )
			{
				cbdisplaced = new CommandBar[ncbarpos];
				Array.Copy( cbarray, 0, cbdisplaced, 0, cbdisplaced.Length );
				if( cbdisplaced.Length > 0 )	// The dockbar contains commandbars that lie before the dragged bar.
				{
					CommandBar cbprev = cbdisplaced[cbdisplaced.Length - 1];
					if( cbprev.VRight + 2 < ptclient.X )
						Array.Clear( cbdisplaced, 0, cbdisplaced.Length );
					else
					{
						if( ptclient.X < cbprev.nRowOffsetDir + 2 )
						{
							if( ptclient.X < this.ClientRectangle.Left )
								cbar.nRowOffsetInDir = this.ClientRectangle.Left;
							else
								cbar.nRowOffsetInDir = ptclient.X;
							cbar.VLeft = cbar.nRowOffsetInDir;
							cbprev.nRowOffsetInDir = cbar.nRowOffsetInDir + cbar.Width + 2;
							cbprev.VLeft = cbprev.nRowOffsetInDir;
							this.LayoutDockBar();

							return new Point( -1, -1 );
						}
						else if( !binsizing )
						{
							this.RepositionBarsRToLShift( ref cbdisplaced, cbdisplaced.Length - 1, ref ndeltax, cbar.Location.Y );
							if( ndeltax != 0 )
								return new Point( cbprev.VRight + 2, cbar.Location.Y );
						}
						else
							return new Point( -1, -1 );
					}
				}
				else
				{
					if( ptclient.X <= this.ClientRectangle.Left )
						return new Point( this.ClientRectangle.Left, cbar.Location.Y );
				}
				return new Point( ptclient.X, cbar.Location.Y );
			}
			else if( ( ndeltax < 0 ) && ( ptclient.X >= cbar.VLeft ) )
			{
				cbdisplaced = new CommandBar[cbarray.Length - 1 - ncbarpos];
				Array.Copy( cbarray, ncbarpos+1, cbdisplaced, 0, cbdisplaced.Length );
				if( cbdisplaced.Length > 0 )
				{
					CommandBar cbnext = cbdisplaced[0];
					if( cbnext.VLeft - 2 > ptclient.X + cbar.Width )
						Array.Clear( cbdisplaced, 0, cbdisplaced.Length );
					else
					{
						if( ptclient.X > cbnext.nRowOffsetDir )
						{
							if( ptclient.X + cbar.Width > this.ClientRectangle.Right )
								cbar.nRowOffsetInDir = this.ClientRectangle.Right - cbar.Width;
							else
								cbar.nRowOffsetInDir = ptclient.X;
							cbar.VLeft = cbar.nRowOffsetInDir;
							cbnext.nRowOffsetInDir = cbar.nRowOffsetInDir - 2 - cbnext.Width;
							cbnext.VLeft = cbnext.nRowOffsetInDir;
							this.LayoutDockBar();

							return new Point( -1, -1 );
						}
						else if( !binsizing )
						{
							this.RepositionBarsLToRShift( ref cbdisplaced, 0, ref ndeltax, cbar.Location.Y );
							if( ndeltax != 0 )
								return new Point( cbnext.VLeft - 2 - cbar.Width, cbar.Location.Y );
						}
						else
							return new Point( -1, -1 );
					}
				}
				else
				{
					if( ptclient.X + cbar.Width > this.ClientRectangle.Right )
						return new Point( this.ClientRectangle.Right - cbar.Width, cbar.Location.Y );
				}
				return new Point( ptclient.X, cbar.Location.Y );
			}

			return new Point( -1, -1 );
		}

		protected Point HandleDragOverLeftRight( CommandBar cbar, Point ptscreen )
		{
			Point ptclient = this.PointToClient( ptscreen );
			CommandBar[] cbarray = this.GetRowArray( cbar.nRCIndex );

			// If the commandbar is being dragged from the containing row/column into some other row/col, then
			// reinsert at position.
			if( ptclient.X > cbar.Right )
			{
				CommandBar[] cbnewrow = this.GetRowArray( cbar.nRCIndex+1 );

				if( cbnewrow.Length > 0 )
				{
					CommandBar cb = cbnewrow[0];
					if( ( cb.bTrailingEdge == true ) && ( cb.bFullRow == true ) )
						return new Point( -1, -1 );

					// The drag point should lie beyond atleast half the width of the next row for a reinsert to take place
					if( ptclient.X < cb.Left+cb.Width/2 )
						return new Point( -1, -1 );
				}

				if( ( cbar.bLeadingEdge == true ) && ( ( cbarray.Length > 1 ) || ( cbnewrow[0].bFullRow == true ) ) )
					return new Point( -1, -1 );

				int nindex = cbar.nRCIndex;
				cbar.nRCIndex++;
				if( cbarray.Length == 1 )
				{
					this.nRCCount--;
					foreach( CommandBar cb in this.Controls )
					{
						if( ( cb.Visible == true ) && ( cb.nRCIndex >= nindex ) )
						{
							cb.nRCIndex--;
						}
					}
					this.CalcDockbarSize();
				}
				if( this.ValidateBarIndex( cbar, true ) )
				{
					this.CalcDockbarSize();
					this.LayoutDockBar();
				}
				if( cbar.OccupyFullRow == false )
				{
					this.AdjustPreviousRow( cbar, ref cbarray, cbar.VBounds );
					if( ( cbnewrow.Length > 0 ) && ( cbnewrow[0].OccupyFullRow == false ) )
						this.AdjustNewRow( cbar, ref cbnewrow );
					cbar.nRowOffsetInDir = cbar.Location.Y;
					cbar.nRowOffsetDir = cbar.nRowOffsetInDir;
				}
				return new Point( -1, -1 );
			}
			else if( ptclient.X < cbar.Left )
			{
				CommandBar[] cbnewrow = this.GetRowArray( cbar.nRCIndex-1 );

				if( cbnewrow.Length > 0 )
				{
					CommandBar cb = cbnewrow[0];
					if( ( cb.bLeadingEdge == true ) && ( cb.bFullRow == true ) )
						return new Point( -1, -1 );

					// The drag point should lie beyond atleast half the width of the previous row for a reinsert to take place
					if( ptclient.X > cb.Right-cb.Width/2 )
						return new Point( -1, -1 );
				}

				if( ( cbar.bTrailingEdge == true ) && ( ( cbarray.Length > 1 ) || ( cbnewrow[0].bFullRow == true ) ) )
					return new Point( -1, -1 );

				int nindex = cbar.nRCIndex;
				cbar.nRCIndex--;
				if( cbarray.Length == 1 )
				{
					this.nRCCount--;
					foreach( CommandBar cb in this.Controls )
					{
						if( ( cb.Visible == true ) && ( cb.nRCIndex >= nindex ) )
						{
							cb.nRCIndex--;
						}
					}
					this.CalcDockbarSize();
				}
				if( this.ValidateBarIndex( cbar, false ) )
				{
					this.CalcDockbarSize();
					this.LayoutDockBar();
				}
				if( cbar.OccupyFullRow == false )
				{
					this.AdjustPreviousRow( cbar, ref cbarray, cbar.VBounds );
					if( ( cbnewrow.Length > 0 ) && ( cbnewrow[0].OccupyFullRow == false ) )
						this.AdjustNewRow( cbar, ref cbnewrow );
					cbar.nRowOffsetInDir = cbar.Location.Y;
					cbar.nRowOffsetDir = cbar.nRowOffsetInDir;
				}
				return new Point( -1, -1 );
			}

			// If any of the bars are sized-down or wrapped, then disallow sliding
			bool binsizing = false;
			foreach( CommandBar cb in cbarray )
			{
				if( cb.Height < cb.nMaxLength )
				{
					if( cb.bDockModeWrapping == true )
						return new Point( -1, -1 );
					else
						binsizing = true;
				}
			}

			// If the commandbar butts against an existing commandbar, try moving the other bar. If this succeeds return
			// true and the new commandbar will be located in this position. However if there is insufficient space to
			// accomodate the commandbar in the newposition return false.
			CommandBar[] cbdisplaced;
			int ncbarpos = Array.IndexOf( cbarray, cbar );
			int ndeltay = cbar.Location.Y - ptclient.Y;

			// If ndeltax is positive, ie., sliding to the top, then form an array of bars that contains all commandbars that
			// lie before the dragged bar. Else, populate array with bars that follow the dragged bar.
			if( ( ndeltay > 0 ) && ( ptclient.Y <= ( cbar.Top+cbar.nHeaderOff ) ) )
			{
				cbdisplaced = new CommandBar[ncbarpos];
				Array.Copy( cbarray, 0, cbdisplaced, 0, cbdisplaced.Length );
				if( cbdisplaced.Length > 0 )	// The dockbar contains commandbars that lie before the dragged bar.
				{
					CommandBar cbprev = cbdisplaced[cbdisplaced.Length-1];
					if( cbprev.Bottom+2 < ptclient.Y )
						Array.Clear( cbdisplaced, 0, cbdisplaced.Length );
					else
					{
						if( ptclient.Y < cbprev.nRowOffsetDir+2 )
						{
							if( ptclient.Y < this.ClientRectangle.Top )
								cbar.nRowOffsetInDir = this.ClientRectangle.Top;
							else
								cbar.nRowOffsetInDir = ptclient.Y;
							cbar.Top = cbar.nRowOffsetInDir;
							cbprev.nRowOffsetInDir = cbar.nRowOffsetInDir+cbar.Height+2;
							cbprev.Top = cbprev.nRowOffsetInDir;
							this.LayoutDockBar();

							return new Point( -1, -1 );
						}
						else if( binsizing == false )
						{
							this.RepositionBarsBToTShift( ref cbdisplaced, cbdisplaced.Length-1, ref ndeltay, cbar.Location.X );
							if( ndeltay != 0 )
								return new Point( cbar.Location.X, cbprev.Bottom+2 );
						}
						else
							return new Point( -1, -1 );
					}
				}
				else
				{
					if( ptclient.Y <= this.ClientRectangle.Top )
						return new Point( cbar.Location.X, this.ClientRectangle.Top );
				}
				return new Point( cbar.Location.X, ptclient.Y );
			}
			else if ((ndeltay < 0) && (ptclient.Y >= cbar.Top))
			{
				cbdisplaced = new CommandBar[cbarray.Length - 1 - ncbarpos];
				Array.Copy(cbarray, ncbarpos + 1, cbdisplaced, 0, cbdisplaced.Length);
				if (cbdisplaced.Length > 0)
				{
					CommandBar cbnext = cbdisplaced[0];
					if (cbnext.Top - 2 > ptclient.Y + cbar.Height)
						Array.Clear(cbdisplaced, 0, cbdisplaced.Length);
					else
					{
						if (ptclient.Y > cbnext.nRowOffsetDir)
						{
							if (ptclient.Y + cbar.Height > this.ClientRectangle.Bottom)
								cbar.nRowOffsetInDir = this.ClientRectangle.Bottom - cbar.Height;
							else
								cbar.nRowOffsetInDir = ptclient.Y;
							cbar.Top = cbar.nRowOffsetInDir;
							cbnext.nRowOffsetInDir = cbar.nRowOffsetInDir - 2 - cbnext.Height;
							cbnext.Top = cbnext.nRowOffsetInDir;
							this.LayoutDockBar();

							return new Point(-1, -1);
						}
						else if (binsizing == false)
						{
							this.RepositionBarsTToBShift(ref cbdisplaced, 0, ref ndeltay, cbar.Location.X);
							if (ndeltay != 0)
								return new Point(cbar.Location.X, cbnext.Top - 2 - cbar.Height);
						}
						else
							return new Point(-1, -1);
					}
				}
				else
				{
					int maxPos = Math.Max( this.ClientRectangle.Bottom - cbar.Height, 0);

					if (ptclient.Y > maxPos)
					{
						ptclient.Y = maxPos;
					}
				}
				return new Point(cbar.Location.X, ptclient.Y);
			}

			return new Point( -1, -1 );
		}

		protected bool AdjustNewRow( CommandBar cbar, ref CommandBar[] cbnewrow )
		{
			if( ( this.Dock == DockStyle.Top ) || ( this.Dock == DockStyle.Bottom ) )
			{
				int ncbrowcurrheight = ( cbnewrow.Length > 0 ) ? cbnewrow[0].Height : 0;
				int ncbrowheight = ( ncbrowcurrheight > cbar.nCommandBarHt ) ? ncbrowcurrheight : cbar.nCommandBarHt;
				cbar.Height = ncbrowcurrheight;
				if( ncbrowcurrheight != ncbrowheight )
				{
					CommandBar[] cbarray = this.GetRowArray( cbar.nRCIndex );
					foreach( CommandBar cb in cbarray )
					{
						Debug.Assert( cb.Height <= ncbrowheight );
						if( cb.Height  < ncbrowheight )
							cb.Height = ncbrowheight;
					}
					this.CalcDockbarSize();
					this.LayoutDockBar();
					return true;
				}
			}
			else
			{
				int ncbrowcurrheight = ( cbnewrow.Length > 0 ) ? cbnewrow[0].Width : 0;
				int ncbrowheight = ( ncbrowcurrheight > cbar.nCommandBarHt ) ? ncbrowcurrheight : cbar.nCommandBarHt;
				cbar.Width = ncbrowcurrheight;
				if( ncbrowcurrheight != ncbrowheight )
				{
					CommandBar[] cbarray = this.GetRowArray( cbar.nRCIndex );
					foreach( CommandBar cb in cbarray )
					{
						Debug.Assert( cb.Width <= ncbrowheight );
						if( cb.Width  < ncbrowheight )
							cb.Width = ncbrowheight;
					}
					this.CalcDockbarSize();
					this.LayoutDockBar();
					return true;
				}
			}
			return false;
		}

		public void AdjustRowOffsets( CommandBar cbar, ref int ndelta )
		{
			Debug.Assert( this.Controls.Contains( cbar ) );
			CommandBar[] cbarray = this.GetRowArray( cbar.nRCIndex );
			this.AdjustRowOffsets( ref cbarray, Array.IndexOf( cbarray, cbar ), cbar.VBounds, ref ndelta );
		}

		protected void AdjustRowOffsets( ref CommandBar[] cbarray, int cbindex, Rectangle rccbar, ref int ndelta )
		{
			CommandBar[] cbdisplaced;
			if( ndelta > 0 )
			{
				cbdisplaced = new CommandBar[( cbarray.Length - 1 ) - cbindex];
				Array.Copy( cbarray, cbindex + 1, cbdisplaced, 0, cbdisplaced.Length );
				if( cbdisplaced.Length > 0 )
				{
					CommandBar cbnext = cbdisplaced[0];
					// Reposition any adjacent trailing bars.
					if( ( this.Dock == DockStyle.Top ) || ( this.Dock == DockStyle.Bottom ) )
					{
						if( ( ( cbnext.VLeft - 2 - ndelta ) - 1 <= rccbar.Right ) && ( cbnext.VLeft > cbnext.nRowOffsetDir ) )
						{
							int nx = 0;
							if( cbnext.VLeft - ndelta >= cbnext.nRowOffsetDir )
							{
								nx = ndelta;
								ndelta = 0;
							}
							else
							{
								nx = cbnext.VLeft - cbnext.nRowOffsetDir;
								ndelta -= nx;
							}
							for( int i = 0; i < cbdisplaced.Length; i++ )
							{
								CommandBar cb = cbdisplaced[i];
								if( cb.VLeft - nx >= cb.nRowOffsetDir )
									cb.VLeft -= nx;
								else
									cb.VLeft = cb.nRowOffsetDir;
							}
						}
					}
					else
					{
						if( ( ( cbnext.Top - 2 - ndelta ) - 1 <= rccbar.Bottom )	&& ( cbnext.Top > cbnext.nRowOffsetDir ) )
						{
							int nx = 0;
							if( cbnext.Top-ndelta >= cbnext.nRowOffsetDir )
							{
								nx = ndelta;
								ndelta = 0;
							}
							else
							{
								nx = cbnext.Top - cbnext.nRowOffsetDir;
								ndelta -= nx;
							}
							for( int i = 0; i < cbdisplaced.Length; i++ )
							{
								CommandBar cb = cbdisplaced[i];
								if( cb.Top - nx >= cb.nRowOffsetDir )
									cb.Top -= nx;
								else
									cb.Top = cb.nRowOffsetDir;
							}
						}
					}
				}
			}
			else
			{
				cbdisplaced = new CommandBar[cbindex];
				Array.Copy( cbarray, 0, cbdisplaced, 0, cbdisplaced.Length );
				if( cbdisplaced.Length > 0 )
				{
					CommandBar cbnext = cbdisplaced[cbdisplaced.Length - 1];
					if( ( this.Dock == DockStyle.Top ) || ( this.Dock == DockStyle.Bottom ) )
					{
						if( ( ( cbnext.VRight + 2 - ndelta ) + 1 >= rccbar.Left ) && ( cbnext.nRowOffsetDir > cbnext.VLeft ) )
						{
							int nx = 0;
							if( cbnext.VLeft - ndelta <= cbnext.nRowOffsetDir )
							{
								nx = ndelta;
								ndelta = 0;
							}
							else
							{
								nx = cbnext.VLeft - cbnext.nRowOffsetDir;
								ndelta -= nx;
							}
							for( int i = cbdisplaced.Length - 1; i >= 0; i-- )
							{
								CommandBar cb = cbdisplaced[i];
								if( cb.VLeft - nx <= cb.nRowOffsetDir )
									cb.VLeft -= nx;
								else
									cb.VLeft = cb.nRowOffsetDir;
							}
						}
					}
					else
					{
						if( ( ( cbnext.Bottom + 2 - ndelta ) + 1 >= rccbar.Top ) && ( cbnext.nRowOffsetDir > cbnext.Top ) )
						{
							int nx = 0;
							if( cbnext.Top - ndelta <= cbnext.nRowOffsetDir )
							{
								nx = ndelta;
								ndelta = 0;
							}
							else
							{
								nx = cbnext.Top - cbnext.nRowOffsetDir;
								ndelta -= nx;
							}
							for( int i = cbdisplaced.Length - 1; i >= 0; i-- )
							{
								CommandBar cb = cbdisplaced[i];
								if( cb.Top - nx <= cb.nRowOffsetDir )
									cb.Top -= nx;
								else
									cb.Top = cb.nRowOffsetDir;
							}
						}
					}
				}
			}

			if( ( this.Dock == DockStyle.Top ) || ( this.Dock == DockStyle.Bottom ) )
			{
				foreach( CommandBar cb in this.Controls )
				{
					if( cb.Visible )
						cb.nRowOffsetInDir = cb.VLeft;
				}
			}
			else
			{
				foreach( CommandBar cb in this.Controls )
				{
					if( cb.Visible )
						cb.nRowOffsetInDir = cb.Location.Y;
				}
			}
		}

		public void AdjustNextBarsInRow( CommandBar cbar )
		{
			CommandBar[] cbarray = this.GetRowArray( cbar.nRCIndex );
			int ncount = cbarray.GetLength( 0 );
			for( int nindex=Array.IndexOf( cbarray, cbar ); nindex<ncount; nindex++ )
			{
				if( nindex < ncount-1 )
				{
					CommandBar cb = cbarray[nindex+1];
					if( ( nindex >= 0 ) && ( nindex+1 < ncount ) )
					{
						CommandBar cbnindex = cbarray[nindex];
						if( ( this.Dock == DockStyle.Top ) || ( this.Dock == DockStyle.Bottom ) )
						{
							cb.VLeft = cbnindex.VRight+2;
							cb.nRowOffsetDir = cb.VLeft;
						}
						else
						{
							cb.Location = new Point( cb.Location.X, cbnindex.Bottom + 2 );
							cb.nRowOffsetDir = cb.Location.Y;
						}
						cb.nRowOffsetInDir = cb.nRowOffsetDir;
					}
				}
			}
		}

		public int ConfirmNewRowHeight( CommandBar cbar, int nnewdim )
		{
			Debug.Assert( this.Controls.Contains( cbar ) );
			CommandBar[] cbarray = this.GetRowArray( cbar.nRCIndex );
			int nmaxht = nnewdim, htmin = 0, htwrap = 0;
			foreach( CommandBar cb in cbarray )
			{
				if( cb == cbar )
					continue;
				htmin = cb.nCommandBarHt;
				if( cb.DockModeWrapping )
				{
					if( ( this.Dock == DockStyle.Top ) || ( this.Dock == DockStyle.Bottom ) )
						htwrap = cb.GetDockWrapSize( cb.Size ).Height;
					else
						htwrap = cb.GetDockWrapSize( cb.Size ).Width;
				}
				int nmax = ( htmin > htwrap ) ? htmin : htwrap;
				if( nmax > nmaxht )
					nmaxht = nmax;
			}
			return nmaxht;
		}

		public void AdjustRowHeight( CommandBar cbar, int ndelta )
		{
			Debug.Assert( this.Controls.Contains( cbar ) );
			this.cbController.FreezeLayoutInternal = true;

			CommandBar[] cbarray;
			switch( this.Dock )
			{
				case DockStyle.Top:
				this.Height += ndelta;
				for( int i=cbar.nRCIndex+1; i<this.nRCCount; i++ )
				{
					CommandBar[] cbarr = this.GetRowArray( i );
					foreach( CommandBar cb in cbarr )
						cb.Location = new Point( cb.Location.X, cb.Location.Y+ndelta );
				}

				cbarray = this.GetRowArray( cbar.nRCIndex );
				foreach( CommandBar cb in cbarray )
				{
					if( cb == cbar )
						continue;
					cb.Height += ndelta;
				}
				break;
				case DockStyle.Bottom:
				this.Location = new Point( this.Location.X, this.Location.Y-ndelta );
				this.Height += ndelta;
				for( int i=cbar.nRCIndex+1; i<this.nRCCount; i++ )
				{
					CommandBar[] cbarr = this.GetRowArray( i );
					foreach( CommandBar cb in cbarr )
						cb.Location = new Point( cb.Location.X, cb.Location.Y+ndelta );
				}
				cbarray = this.GetRowArray( cbar.nRCIndex );
				foreach( CommandBar cb in cbarray )
				{
					if( cb == cbar )
						continue;
					cb.Height += ndelta;
				}
				break;
				case DockStyle.Left:
				this.Width += ndelta;
				for( int i=cbar.nRCIndex+1; i<this.nRCCount; i++ )
				{
					CommandBar[] cbarr = this.GetRowArray( i );
					foreach( CommandBar cb in cbarr )
						cb.Location = new Point( cb.Location.X+ndelta, cb.Location.Y );
				}

				cbarray = this.GetRowArray( cbar.nRCIndex );
				foreach( CommandBar cb in cbarray )
				{
					if( cb == cbar )
						continue;
					cb.Width += ndelta;
				}
				break;
				case DockStyle.Right:
				this.Location = new Point( this.Location.X- ndelta, this.Location.Y );
				this.Width += ndelta;
				for( int i=cbar.nRCIndex+1; i<this.nRCCount; i++ )
				{
					CommandBar[] cbarr = this.GetRowArray( i );
					foreach( CommandBar cb in cbarr )
						cb.Location = new Point( cb.Location.X+ndelta, cb.Location.Y );
				}
				cbarray = this.GetRowArray( cbar.nRCIndex );
				foreach( CommandBar cb in cbarray )
				{
					if( cb == cbar )
						continue;
					cb.Location = new Point( cb.Location.X-ndelta, cb.Location.Y );
					cb.Width += ndelta;
				}
				break;
			}

			this.cbController.FreezeLayoutInternal = false;
		}

		/// Clean up any resources being used.
		protected override void Dispose( bool disposing )
		{
			// Prevent postponed layouting
			m_nSuspendCount++;

			if( this.tdRebar != null )
			{
				this.tdRebar.Dispose();
				this.tdRebar = null;
			}

			if( this.cbController != null )
				this.cbController = null;

			base.Dispose( disposing );

			m_nSuspendCount--;
		}

		// Accessibility Implementation
		protected override AccessibleObject CreateAccessibilityInstance()
		{
			return new CommandDockBarAccessibleObject( this );
		}

		internal protected bool IsRTL
		{
			get
			{
				return ( RightToLeft.Yes == this.RightToLeft );
			}
		}

		internal protected int RowsCount
		{
			get
			{
				return this.nRCCount;
			}
		}

#if SyncfusionFramework2_0
		protected override void ScaleControl( SizeF factor, BoundsSpecified specified )
		{
			if( cbController != null && cbController is CommandBarControllerExt )
			{
				CommandBarControllerExt controllerExt = cbController as CommandBarControllerExt;

				if( controllerExt.Manager != null && controllerExt.Manager.AutoScale && this.Parent != null )
				{
					controllerExt.Manager.Font = this.Parent.Font;
					base.ScaleControl( factor, specified );
				}
			}
		}

		protected override bool ScaleChildren
		{
			get
			{
				bool bAutoScale = false;

				if( cbController != null && cbController is CommandBarControllerExt )
				{
					CommandBarControllerExt controllerExt = cbController as CommandBarControllerExt;

					if( controllerExt.Manager != null && controllerExt.Manager.AutoScale )
					{
						bAutoScale = true;
					}
				}

				return bAutoScale;
			}
		}
#endif

		#region Event handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnControllerLayoutSuspended( object sender, EventArgs e )
		{
			m_nSuspendCount++;

			if( m_nSuspendCount == 1 )
			{
				SuspendLayout();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnControllerLayoutResumed( object sender, EventArgs e )
		{
			if( this.IsHandleCreated )
			{
				BeginInvoke( new ResumeLayoutAsync( ResumeLayoutInternal ) );
			}
			else
			{
				ResumeLayoutInternal();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		void ResumeLayoutInternal()
		{
			if( m_nSuspendCount > 0 )
			{
				m_nSuspendCount--;

				if( m_nSuspendCount == 0 && !this.IsDisposed )
				{
					CalcDockbarSize();
					LayoutDockBar();
					LayoutDockBar();

					ResumeLayout();
				}
			}
		}
		#endregion

	}


	///   Form class that hosts the CommandBars in the floating state.
	[Syncfusion.Documentation.DocumentationExclude()]
	public class CommandBarForm: System.Windows.Forms.Form, ICommandBarDesignerMouseHook
	{
		static protected Cursor crDefault = null;
		static protected bool bSizing = false;
		static protected Point ptStart = Point.Empty;
		static protected CommandBarResizeType rtSizing = CommandBarResizeType.None;
		static protected Rectangle rcSizeBounds = Rectangle.Empty;

		protected CommandBarController cbController = null;

		// XPTheme drawing
		protected ThemedControlDrawing tdWindow = null;
		protected ThemedControlDrawing tdRebar = null;

		public static bool Sizing
		{
			get { return CommandBarForm.bSizing; }
		}

		/// <summary>
		/// Gets color table for Office2007 visual style.
		/// </summary>
		private Office2007Colors Office2007ColorTable
		{
			get
			{
				Office2007Colors colorTable = ( cbController == null ) ? 
					Office2007Colors.Default : cbController.Office2007ColorTable;

				return colorTable;
			}
		}
        /// <summary>
        /// Gets color table for Office2010 visual style.
        /// </summary>
        private Office2010Colors Office2010ColorTable
        {
            get
            {
                Office2010Colors colorTable = (cbController == null) ?
                    Office2010Colors.Default : cbController.Office2010ColorTable;

                return colorTable;
            }
        }
		public CommandBarForm( CommandBarController controller )
		{
			this.cbController = controller;

			this.SetStyle( ControlStyles.Selectable, false );
			this.SetStyle( ControlStyles.AllPaintingInWmPaint|ControlStyles.UserPaint|ControlStyles.DoubleBuffer, true );
			this.UpdateStyles();

			this.Visible = false;
			this.ShowInTaskbar = false;
			this.FormBorderStyle = FormBorderStyle.None;

			if( XPThemes.IsThemedOS )
			{
				this.tdWindow = new ThemedControlDrawing( ThemedControls.WINDOW );
				this.tdRebar = new ThemedControlDrawing( ThemedControls.REBAR );
			}

			Office2007Colors.ManagedColorsApplied += new Office2007Colors.ManagedColorsAppliedEventHandler( OnManagedColorsApplied );
		}

		private void OnManagedColorsApplied( Office2007Colors.ManagedColorsAppliedEventArgs args )
		{
			if( this.cbController != null
				&& ( this.cbController.Style == VisualStyle.Office2007 ) && ( this.cbController.Office2007Theme == Office2007Theme.Managed ) )
			{
				Refresh();
			}
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.cbController.ThemesEnabled
				&& cbController.Style != VisualStyle.Office2007
                && this.cbController.Style != VisualStyle.Office2010
				&& cbController.Style != VisualStyle.Office2007Outlook )
			{
				Rectangle rcframe = this.ClientRectangle;
				Rectangle rcborder = new Rectangle( rcframe.Left, rcframe.Top, 2, rcframe.Height );
				this.tdWindow.DrawThemeBackground( e.Graphics, ThemeParts.WP_SMALLFRAMELEFT, ThemeStates.FS_INACTIVE, rcborder );
				rcborder = new Rectangle( rcframe.Left, rcframe.Top, rcframe.Width, 2 );
				this.tdWindow.DrawThemeBackground( e.Graphics, ThemeParts.WP_SMALLCAPTION, ThemeStates.CS_INACTIVE, rcborder );
				rcborder = new Rectangle( rcframe.Right-2, rcframe.Top, 2, rcframe.Height );
				this.tdWindow.DrawThemeBackground( e.Graphics, ThemeParts.WP_SMALLFRAMERIGHT, ThemeStates.FS_INACTIVE, rcborder );
				rcborder = new Rectangle( rcframe.Left, rcframe.Bottom-2, rcframe.Width, 2 );
				this.tdWindow.DrawThemeBackground( e.Graphics, ThemeParts.WP_SMALLFRAMEBOTTOM, ThemeStates.FS_INACTIVE, rcborder );
				Rectangle rcclient = Rectangle.Inflate( rcframe, -2, -2 );

				this.tdRebar.DrawThemeBackground( e.Graphics, 6, 1, rcclient );
			}
			else
			{
				Color bordercolor = GetBorderColor( this.cbController.Style );
				Rectangle rcframe = new Rectangle( 1, 1, this.ClientRectangle.Width-2, this.ClientRectangle.Height-2 );
				Pen pnborder = new Pen( bordercolor, 2 );
				e.Graphics.DrawRectangle( pnborder, rcframe );
				pnborder.Dispose();
			}
		}

		/// <summary>
		/// Gets color for border amenably with VisualStyle.
		/// </summary>
		private Color GetBorderColor( VisualStyle style )
		{
			Color color = Color.Empty;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					color = Office2003Colors.FloatingCommandBarCaptionColor;
					break;
				}
				case VisualStyle.VS2005:
				{
					color = VS2005Colors.FloatBorderColor;
					break;
				}
				case VisualStyle.Office2007Outlook:
				case VisualStyle.Office2007:
				{
					color = Office2007ColorTable.FloatBorderColor;
					break;
				}
                case VisualStyle.Office2010:
                {
                    color = Office2010ColorTable.FloatBorderColor;
                    break;
                }
				default:
				{
					color = ControlPaint.Dark( SystemColors.Control, 0.2f );
					break;
				}
			}

			return color;
		}


		protected override void OnMouseDown( MouseEventArgs e )
		{
			base.OnMouseDown( e );

			if( ( this.cbController != null ) && ( this.cbController.DesignTime == false ) )
				this.HandleMouseDown( e.Button, this.PointToScreen( new Point( e.X, e.Y ) ) );
		}

		protected override void OnMouseMove( MouseEventArgs e )
		{
			base.OnMouseMove( e );

			if( ( this.cbController != null ) && ( this.cbController.DesignTime == false ) )
				this.HandleMouseMove( e.Button, this.PointToScreen( new Point( e.X, e.Y ) ) );
		}

		protected override void OnMouseUp( MouseEventArgs e )
		{
			base.OnMouseUp( e );

			if( ( this.cbController != null ) && ( this.cbController.DesignTime == false ) )
				this.HandleMouseUp( e.Button, this.PointToScreen( new Point( e.X, e.Y ) ) );
		}

		protected override void OnMouseLeave( EventArgs e )
		{
			base.OnMouseLeave( e );

			if( ( this.cbController != null ) && ( this.cbController.DesignTime == false ) )
				this.HandleMouseLeave();
		}

		// IDesignerMouseHook implementation
		public void HandleMouseDown( MouseButtons button, Point ptscreen )
		{
			Point ptclient = this.PointToClient( ptscreen );
			if( CommandBarForm.crDefault != null )
			{
				CommandBarForm.bSizing = true;
				CommandBarForm.ptStart = this.PointToScreen( ptclient );
				CommandBar cbchild = this.Controls[0] as CommandBar;

				if( cbchild.bRestrictedSizing == true )	// CommandBar
				{
					if( ptclient.X <= 3 )
						CommandBarForm.rtSizing = CommandBarResizeType.Left;
					else if( ptclient.X >= this.Width-3 )
						CommandBarForm.rtSizing = CommandBarResizeType.Right;
					else if( ptclient.Y <= 3 )
						CommandBarForm.rtSizing = CommandBarResizeType.Top;
					else if( ptclient.Y >= this.Height-3 )
						CommandBarForm.rtSizing = CommandBarResizeType.Bottom;
				}
				else // ControlBar
				{
					if( ( ptclient.X <= 3 ) && ( ptclient.Y <= 3 ) )
						CommandBarForm.rtSizing = ( CommandBarResizeType.Left|CommandBarResizeType.Top );
					else if( ( ptclient.X >= this.Width-3 ) && ( ptclient.Y >= this.Height-3 ) )
						CommandBarForm.rtSizing = ( CommandBarResizeType.Right|CommandBarResizeType.Bottom );
					else if( ( ptclient.X <= 3 ) && ( ptclient.Y >= this.Height-3 ) )
						CommandBarForm.rtSizing = ( CommandBarResizeType.Left|CommandBarResizeType.Bottom );
					else if( ( ptclient.X >= this.Width-3 ) && ( ptclient.Y <= 3 ) )
						CommandBarForm.rtSizing = ( CommandBarResizeType.Right|CommandBarResizeType.Top );
					else if( ptclient.X < 3 )
						CommandBarForm.rtSizing = CommandBarResizeType.Left;
					else if( ptclient.X > this.Width-3 )
						CommandBarForm.rtSizing = CommandBarResizeType.Right;
					else if( ptclient.Y < 3 )
						CommandBarForm.rtSizing = CommandBarResizeType.Top;
					else if( ptclient.Y > this.Height-3 )
						CommandBarForm.rtSizing = CommandBarResizeType.Bottom;
				}

				CommandBarForm.rcSizeBounds = this.Bounds;
			}
			if( this.Owner != null )
				this.Owner.Focus();
		}

		public void HandleMouseMove( MouseButtons button, Point ptscreen )
		{
			CommandBar cbchild = this.Controls[0] as CommandBar;

			CommandBarExt cbExt = this.Controls[0] as CommandBarExt;
			Bar barChild = null;

			if( cbExt != null )
			{
				barChild = cbExt.Bar;
			}

			Debug.Assert( cbchild != null );

			if( cbchild.FloatModeWrapping == false )
				return;

			Point ptclient = this.PointToClient( ptscreen );

			if( CommandBarForm.bSizing == false )
			{
				if( !cbchild.DesignProcess )
				{
					if( cbchild.bRestrictedSizing == true )	// CommandBar
					{
						if( ( ptclient.X <= 3 ) || ( ptclient.X >= this.Width-3 ) )
						{
							if( CommandBarForm.crDefault == null )
							{
								if( barChild != null )
								{
									if( barChild.AllowResizing )
									{
										CommandBarForm.crDefault = this.Cursor;
										this.Cursor = Cursors.SizeWE;
									}
									else
									{
										this.Cursor = CommandBarForm.crDefault;
									}
								}
								else
								{
									CommandBarForm.crDefault = this.Cursor;
									this.Cursor = Cursors.SizeWE;
								}
							}
						}
						else if( ( ptclient.Y <= 3 ) || ( ptclient.Y >= this.Height-3 ) )
						{
							if( CommandBarForm.crDefault == null )
							{
								if( barChild != null )
								{
									if( barChild.AllowResizing )
									{
										CommandBarForm.crDefault = this.Cursor;
										this.Cursor = Cursors.SizeNS;
									}
									else
									{
										this.Cursor = CommandBarForm.crDefault;
									}
								}
								else
								{
									CommandBarForm.crDefault = this.Cursor;
									this.Cursor = Cursors.SizeNS;
								}
							}
						}
						else if( CommandBarForm.crDefault != null )
						{
							this.Cursor = CommandBarForm.crDefault;
							CommandBarForm.crDefault = null;
						}
					}
					else if( !( cbchild is ControlBar ) )
					{
						if( ( ( ptclient.X <= 3 ) && ( ptclient.Y <= 3 ) )
							|| ( ( ptclient.X >= this.Width-3 ) && ( ptclient.Y >= this.Height-3 ) ) )
						{
							if( CommandBarForm.crDefault == null )
							{
								CommandBarForm.crDefault = this.Cursor;
								this.Cursor = Cursors.SizeNWSE;
							}
						}
						else if( ( ( ptclient.X <= 3 ) && ( ptclient.Y >= this.Height-3 ) )
							|| ( ( ptclient.X >= this.Width-3 ) && ( ptclient.Y <= 3 ) ) )
						{
							if( CommandBarForm.crDefault == null )
							{
								CommandBarForm.crDefault = this.Cursor;
								this.Cursor = Cursors.SizeNESW;
							}
						}
						else if( ( ptclient.X < 3 ) || ( ptclient.X > this.Width-3 ) )
						{
							if( CommandBarForm.crDefault == null )
							{
								CommandBarForm.crDefault = this.Cursor;
								this.Cursor = Cursors.SizeWE;
							}
						}
						else if( ( ptclient.Y < 3 ) || ( ptclient.Y > this.Height-3 ) )
						{
							if( CommandBarForm.crDefault == null )
							{
								CommandBarForm.crDefault = this.Cursor;
								this.Cursor = Cursors.SizeNS;
							}
						}
						else if( CommandBarForm.crDefault != null )
						{
							this.Cursor = CommandBarForm.crDefault;
							CommandBarForm.crDefault = null;
						}
					}
				}
			}
			else if( barChild != null && barChild.AllowResizing == true )	// (CommandBarForm.bSizing == true) Resizing in process
			{
				Rectangle rcbounds = CommandBarForm.rcSizeBounds;
				int ndeltaX = ptscreen.X - CommandBarForm.ptStart.X;
				int ndeltaY = ptscreen.Y - CommandBarForm.ptStart.Y;
				CommandBarForm.ptStart = ptscreen;
				if( CommandBarForm.rtSizing == CommandBarResizeType.Top )
				{
					rcbounds.Y += ndeltaY;
					rcbounds.Height = ( ndeltaY < 0 ) ? rcbounds.Height+Math.Abs( ndeltaY ) : rcbounds.Height-Math.Abs( ndeltaY );
				}
				else if( CommandBarForm.rtSizing == CommandBarResizeType.Bottom )
				{
					rcbounds.Height += ndeltaY;
				}
				else if( CommandBarForm.rtSizing == CommandBarResizeType.Left )
				{
					rcbounds.X += ndeltaX;
					rcbounds.Width = ( ndeltaX < 0 ) ? rcbounds.Width+Math.Abs( ndeltaX ) : rcbounds.Width-Math.Abs( ndeltaX );
				}
				else if( CommandBarForm.rtSizing == CommandBarResizeType.Right )
				{
					rcbounds.Width += ndeltaX;
				}
				else if( CommandBarForm.rtSizing == ( CommandBarResizeType.Left|CommandBarResizeType.Top ) )
				{
					rcbounds.X += ndeltaX;
					rcbounds.Width = ( ndeltaX < 0 ) ? rcbounds.Width+Math.Abs( ndeltaX ) : rcbounds.Width-Math.Abs( ndeltaX );
					rcbounds.Y += ndeltaY;
					rcbounds.Height = ( ndeltaY < 0 ) ? rcbounds.Height+Math.Abs( ndeltaY ) : rcbounds.Height-Math.Abs( ndeltaY );
				}
				else if( CommandBarForm.rtSizing == ( CommandBarResizeType.Top|CommandBarResizeType.Right ) )
				{
					rcbounds.Width += ndeltaX;
					rcbounds.Y += ndeltaY;
					rcbounds.Height = ( ndeltaY < 0 ) ? rcbounds.Height+Math.Abs( ndeltaY ) : rcbounds.Height-Math.Abs( ndeltaY );
				}
				else if( CommandBarForm.rtSizing == ( CommandBarResizeType.Right|CommandBarResizeType.Bottom ) )
				{
					rcbounds.Width += ndeltaX;
					rcbounds.Height += ndeltaY;
				}
				else if( CommandBarForm.rtSizing == ( CommandBarResizeType.Left|CommandBarResizeType.Bottom ) )
				{
					rcbounds.X += ndeltaX;
					rcbounds.Width = ( ndeltaX < 0 ) ? rcbounds.Width+Math.Abs( ndeltaX ) : rcbounds.Width-Math.Abs( ndeltaX );
					rcbounds.Height += ndeltaY;
				}

				CommandBarForm.rcSizeBounds = rcbounds;

				if( cbchild.bRestrictedSizing == true ) // CommandBar
				{
					int nCaptionHt = CommandBar.CaptionHeight;
					Size szbar = new Size( this.Width-4, this.Height-nCaptionHt-4 );
					Size szsize = cbchild.GetFloatWrapSize( new Size( rcbounds.Width-4, rcbounds.Height-4 ), CommandBarForm.rtSizing );
					int nminwidth = CommandBar.FormMinWidth;
					szsize.Width = ( szsize.Width < nminwidth ) ? nminwidth : szsize.Width;

					if( szsize != szbar )
					{
						if( CommandBarForm.rtSizing == CommandBarResizeType.Top )
							this.Top += szbar.Height - szsize.Height;
						else if( CommandBarForm.rtSizing == CommandBarResizeType.Left )
							this.Left += szbar.Width - szsize.Width;
						this.Size = new Size( szsize.Width+4, szsize.Height+nCaptionHt+4 );
						if( ( CommandBarForm.rtSizing == CommandBarResizeType.Left ) || ( CommandBarForm.rtSizing == CommandBarResizeType.Right ) )
							CommandBarForm.rcSizeBounds.Height = this.Bounds.Height;
						else
							CommandBarForm.rcSizeBounds.Width = this.Bounds.Width;
						cbchild.rcFloat = this.Bounds;
					}
				}
				else // ControlBar
				{
					ControlBar ctrlbar = cbchild as ControlBar;
					if( ( CommandBarForm.rcSizeBounds.Width >= ctrlbar.MinimumSize.Width ) && ( CommandBarForm.rcSizeBounds.Width <= ctrlbar.MaximumSize.Width )
						&& ( CommandBarForm.rcSizeBounds.Height >= ctrlbar.MinimumSize.Height ) && ( CommandBarForm.rcSizeBounds.Height <= ctrlbar.MaximumSize.Height ) )
					{
						this.Bounds = CommandBarForm.rcSizeBounds;
						cbchild.rcFloat = this.Bounds;
						this.Refresh();
					}
				}
			}
		}

		public void HandleMouseUp( MouseButtons button, Point ptscreen )
		{
			if( CommandBarForm.bSizing )
			{
				CommandBarForm.bSizing = false;
				CommandBarForm.ptStart = Point.Empty;
				CommandBarForm.rcSizeBounds = Rectangle.Empty;
				CommandBarForm.rtSizing = CommandBarResizeType.None;
			}
		}

		public void HandleDoubleClick( Point ptscreen )
		{
			// No implementation
		}

		public void HandleMouseLeave()
		{
			if( CommandBarForm.crDefault != null )
			{
				this.Cursor = CommandBarForm.crDefault;
                CommandBarForm.crDefault = null;
			}
            if ((CommandBar.bDragging)|| (CommandBar.ptDeltaOff != Point.Empty))
            {
                CommandBar.bDragging = false;
                CommandBar.ptDeltaOff  = Point.Empty;
            }
		}

		protected override void SetBoundsCore( int x, int y, int width, int height, BoundsSpecified specified )
		{
			base.SetBoundsCore( x, y, width, height, specified );
			if( this.Controls.Count > 0 )
			{
				CommandBar cbar = this.Controls[0] as CommandBar;
				cbar.Location = new Point( 2, 2 );
				cbar.Size = new Size( this.ClientRectangle.Width-4, this.ClientRectangle.Height-4 );
				cbar.rcFloat = this.Bounds;
				this.Invalidate( false );
			}
		}

		protected override void WndProc( ref Message m )
		{
			if( m.Msg == Syncfusion.Runtime.InteropServices.NativeMethods.WM_CBAR_SELANDDISP )
			{
				ISelectionService iss = this.cbController.GetDesignerService( typeof( ISelectionService ) ) as ISelectionService;
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				iss.SetSelectedComponents(new Object[1] { this.Owner }, SelectionTypes.MouseDown);
#else
				iss.SetSelectedComponents( new Object[1] { this.Owner }, SelectionTypes.Primary );
#endif
				this.Dispose();
				return;
			}
			else if( m.Msg == NativeMethods.WM_WINDOWPOSCHANGING )
			{
				NativeMethods.WINDOWPOS wndPos =
                    (NativeMethods.WINDOWPOS)Marshal.PtrToStructure( m.LParam, typeof( NativeMethods.WINDOWPOS ) );
				//(Syncfusion.Runtime.InteropServices.NativeMethods.WINDOWPOS)m.GetLParam(typeof(Syncfusion.Runtime.InteropServices.NativeMethods.WINDOWPOS));

				Rectangle scrRect = SystemInformation.VirtualScreen;

				//correct x-coordinates
				if( wndPos.x > scrRect.Right - 10 )
				{
					if( wndPos.x > scrRect.Right )
					{
						wndPos.x = scrRect.Right - this.Size.Width;
					}
					else
					{
						wndPos.x = scrRect.Right - 10;
					}
				}

				// correct y-coordinates
				if( wndPos.y > scrRect.Bottom - 10 )
				{
					if( wndPos.y > scrRect.Bottom )
					{
						wndPos.y = scrRect.Bottom - this.Size.Height;
					}
					else
					{
						wndPos.y = scrRect.Bottom - 10;
					}
				}

				Marshal.StructureToPtr( wndPos, m.LParam, false );
			}
			else if( m.Msg == NativeMethods.WM_SHOWWINDOW )
			{
				if( m.WParam.ToInt32() != 0 && 
                    m.LParam.ToInt32() == NativeMethods.SW_PARENTOPENING &&
                    this.Controls.Count > 0 && !this.Controls[0].Visible )
				{
					return;
				}
			}

			base.WndProc( ref m );
		}

		protected override void Dispose( bool disposing )
		{
			if( this.tdWindow != null )
			{
				this.tdWindow.Dispose();
				this.tdWindow = null;
				this.tdRebar.Dispose();
				this.tdRebar = null;
			}

			this.cbController = null;

			Office2007Colors.ManagedColorsApplied -= new Office2007Colors.ManagedColorsAppliedEventHandler( OnManagedColorsApplied );

			base.Dispose( disposing );
		}
	}


	/// <summary>
	/// ControlAccessibleObject derived class that implements the Accessibility object for the CommandDockBar.
	/// </summary>
	public class CommandDockBarAccessibleObject: Control.ControlAccessibleObject
	{
		public CommandDockBarAccessibleObject( CommandDockBar dockbar )
			: base( dockbar )
		{
		}

		public override AccessibleRole Role
		{
			get { return AccessibleRole.Client; }
		}

		public override string Name
		{
			get { return String.Concat( "CommandDockBar", " ", this.Owner.Dock.ToString() ); }
		}

		public override Rectangle Bounds
		{
			get { return this.Owner.RectangleToScreen( this.Owner.ClientRectangle ); }
		}

		public override string Description
		{
			get { return this.Name; }
		}

		public override string Help
		{
			get { return String.Empty; }
		}

		public override AccessibleStates State
		{
			get { return AccessibleStates.None; }
		}

		public override string Value
		{
			get { return this.Owner.Text; }
			set { this.Owner.Text = value; }
		}

		public override AccessibleObject Parent
		{
			get { return base.Parent; }
		}

		public override AccessibleObject HitTest( int x, int y )
		{
			Point pt = this.Owner.PointToClient( new Point( x, y ) );
			foreach( Control ctrl in this.Owner.Controls )
			{
				if( ( ctrl.Visible == true ) && ( ctrl.Bounds.Contains( pt ) == true ) )
					return ctrl.AccessibilityObject;
			}
			return base.HitTest( x, y );
		}

		public override AccessibleObject Navigate( AccessibleNavigation navdir )
		{
			return base.Navigate( navdir );
		}
	}
}
