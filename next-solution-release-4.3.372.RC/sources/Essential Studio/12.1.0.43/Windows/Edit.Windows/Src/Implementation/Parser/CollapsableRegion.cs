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

using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Implementation.IO;

namespace Syncfusion.Windows.Forms.Edit.Implementation.Parser
{
	/// <summary>
	/// Region that can be collapsed.
	/// </summary>
	public class CollapsableRegion
		: IComparable
	{
		#region Fields
		/// <summary>
		/// Beginning of the collapsing. Can not be null.
		/// </summary>
		private IParsePoint m_StartPoint;
		/// <summary>
		/// End of the collapsible region. It can be null: in that case file will be parsed until the end of this region.
		/// </summary>
		private IParsePoint m_EndPoint;
		/// <summary>
		/// Flag, that specifies, whether region is collapsed.
		/// </summary>
		private bool m_bCollapsed;
		/// <summary>
		/// Stack at the end of the collapsing.
		/// </summary>
		private ConfigStack m_EndStack;
		/// <summary>
		/// Stack at the beginning of the collapsing.
		/// </summary>
		private ConfigStack m_StartStack;
		/// <summary>
		/// Lexem at StartPoint position.
		/// </summary>
		private ILexem m_lexem;
		/// <summary>
		/// Lexem at the end of region.
		/// </summary>
		private ILexem m_endLexem;
		/// <summary>
		/// Flag that specifies, whether ending of the collapsed region is reliable or not. Checking can be done by comparing starting stack.
		/// </summary>
		private bool m_bUnreliableEnding;
		/// <summary>
		/// Name of the collapse that is shown as text in collapsed region.
		/// </summary>
		private string m_strCollapseName;
		/// <summary>
		/// Delegate for m_EndPoint_OffsetChanged method.
		/// </summary>
		private ParsePointParameterChangedEventHandler m_handlerEndPointOffsetChanged;
		/// <summary>
		/// Delegate for MonitoredPointOffsetChanged method.
		/// </summary>
		private ParsePointParameterChangedEventHandler m_handlerMonitoredPointOffsetChanged;
		/// <summary>
		/// Delegate for m_EndPoint_Deleted method.
		/// </summary>
		private ParsePointDeletedEventHandler m_handlerEndPointDeleted;
		/// <summary>
		/// Delegate for m_StartPoint_Deleted method.
		/// </summary>
		private ParsePointDeletedEventHandler m_handlerStartPointDeleted;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets lexem at StartPoint position.
		/// </summary>
		public ILexem Lexem
		{
			get
			{
				ILexem result = null;
				if( this.End != null )
				{
					result = m_lexem;
				}
				return result;
			}
			set
			{
				m_lexem = value;
			}
		}
		/// <summary>
		/// Gets or sets lexem at End position - the last lexem in the region. Example: "}".
		/// </summary>
		public ILexem EndLexem
		{
			get
			{
				return m_endLexem;
			}
			set
			{
				m_endLexem = value;
			}
		}
		/// <summary>
		/// Gets or sets beginning of the collapsing. Can not be null.
		/// </summary>
		public IParsePoint Start
		{
			get
			{
				return m_StartPoint;
			}
			set
			{
				if( m_StartPoint != value )
				{
					if( m_StartPoint != null )
					{
						m_StartPoint.Deleted -= HandlerStartPointDeleted;
					}

					m_StartPoint = value;
					if( m_StartPoint != null )
					{
						m_StartPoint.Deleted += HandlerStartPointDeleted;
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets end of the collapsible region. (Points to the start of the last lexem in the region).
		/// It can be null: in that case file will be parsed until end the end of this region.
		/// </summary>
		public IParsePoint End
		{
			get
			{
				return m_EndPoint;
			}
			set
			{
				if( m_EndPoint != value )
				{
					if( m_EndPoint != null )
					{
						m_EndPoint.ParsePointParameterChanged -= HandlerEndPointOffsetChanged;
						m_EndPoint.Deleted -= HandlerEndPointDeleted;
					}

					m_EndPoint = value;
					if( m_EndPoint != null )
					{
						m_EndPoint.Deleted += HandlerEndPointDeleted;
						m_EndPoint.ParsePointParameterChanged += HandlerEndPointOffsetChanged;
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets flag, that specifies, whether region is collapsed.
		/// </summary>
		public bool Collapsed
		{
			get
			{
				return m_bCollapsed;
			}
			set
			{
				if( m_bCollapsed != value )
				{
					m_bCollapsed = value;
					RaiseCollapsedStateChanged();

					OutliningEventArgs args = new OutliningEventArgs( m_strCollapseName, this, null );
					bool bCanceled = false;

					if( value )
					{
						if( null != OutliningBeforeCollapse )
						{
							OutliningBeforeCollapse( this, args );
							if( args.Cancel )
							{
								bCanceled = true;
							}
						}
					}
					else
					{
						if( null != OutliningBeforeExpand )
						{
							OutliningBeforeExpand( this, args );
							if( args.Cancel ) bCanceled = true;
						}
					}

					if( bCanceled )
					{
						m_bCollapsed = !value;
						RaiseCollapsedStateChanged();
					}
					else
					{
						if( value )
						{
							if( OutliningCollapse != null )
							{
								OutliningCollapse( this, args );
							}
						}
						else
						{
							if( OutliningExpand != null )
							{
								OutliningExpand( this, args );
							}
						}
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets end stack of the collapsible region.
		/// </summary>
		public ConfigStack EndStack
		{
			get
			{
				ConfigStack result = null;
				if( m_EndPoint != null )
				{
					result = m_EndStack;
				}
				return result;
			}
			set
			{
				m_EndStack = value;
				m_bUnreliableEnding = false;
			}
		}
		/// <summary>
		/// Gets or sets start stack of the collapsible region.
		/// </summary>
		public ConfigStack StartStack
		{
			get
			{
				return m_StartStack;
			}
			set
			{
				m_StartStack = value;
			}
		}
		/// <summary>
		/// Gets flag, that shows whether regions end stack is reliable.
		/// </summary>
		public bool UnreliableEnding
		{
			get
			{
				return m_bUnreliableEnding;
			}
		}
		/// <summary>
		/// Gets or sets name of the collapse that is shown as text in collapsed region.
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
		/// Gets delegate for m_StartPoint_Deleted method.
		/// </summary>
		private ParsePointDeletedEventHandler HandlerStartPointDeleted
		{
			get
			{
				if( null == m_handlerStartPointDeleted )
				{
					m_handlerStartPointDeleted = new ParsePointDeletedEventHandler( OnStartPointDeleted );
				}
				return m_handlerStartPointDeleted;
			}
		}
		/// <summary>
		/// Gets delegate for m_EndPoint_Deleted method.
		/// </summary>
		private ParsePointDeletedEventHandler HandlerEndPointDeleted
		{
			get
			{
				if( null == m_handlerEndPointDeleted )
				{
					m_handlerEndPointDeleted = new ParsePointDeletedEventHandler( OnEndPointDeleted );
				}
				return m_handlerEndPointDeleted;
			}
		}
		/// <summary>
		/// Gets delegate for m_EndPoint_OffsetChanged method.
		/// </summary>
		private ParsePointParameterChangedEventHandler HandlerEndPointOffsetChanged
		{
			get
			{
				if( null == m_handlerEndPointOffsetChanged )
				{
					m_handlerEndPointOffsetChanged = new ParsePointParameterChangedEventHandler( OnEndPointOffsetChanged );
				}
				return m_handlerEndPointOffsetChanged;
			}
		}
		/// <summary>
		/// Gets delegate for MonitoredPointOffsetChanged method.
		/// </summary>
		private ParsePointParameterChangedEventHandler HandlerMonitoredPointOffsetChanged
		{
			get
			{
				if( null == m_handlerMonitoredPointOffsetChanged )
				{
					m_handlerMonitoredPointOffsetChanged = new ParsePointParameterChangedEventHandler( OnMonitoredPointOffsetChanged );
				}
				return m_handlerMonitoredPointOffsetChanged;
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Event that is raised when collapsed state of the region has changed.
		/// </summary>
		public event EventHandler CollapsedStateChanged;
		/// <summary>
		/// Event that is raised when start of the region was deleted.
		/// </summary>
		public event EventHandler RegionDeleted;
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
		#endregion

		#region Public Methods
		/// <summary>
		/// Adds monitored point.
		/// </summary>
		/// <remarks>When points is moved, region ending is treated as unreliable.</remarks>
		/// <param name="point">IParsePoint to be monitored.</param>
		public void AttachMonitoredEndPoint( IParsePoint point )
		{
			if( null == point ) throw new ArgumentNullException( "point" );

			point.ParsePointParameterChanged += HandlerMonitoredPointOffsetChanged;
		}
		/// <summary>
		/// Determines whether region contains given <see cref="IParsePoint"/> object.
		/// </summary>
		/// <param name="point"><see cref="IParsePoint"/> object.</param>
		/// <returns>True if contains, otherwise false.</returns>
		public bool Contains( IParsePoint point )
		{
			if( point == null ) throw new ArgumentNullException( "point" );

			bool result = false;
			if( m_EndPoint != null )
			{
				result = ( point.Offset >= m_StartPoint.Offset && point.Offset < m_EndPoint.Offset );
			}
			return result;
		}
		/// <summary>
		/// Resets information about region end.
		/// </summary>
		public void ResetEnd()
		{
			m_bUnreliableEnding = true;
		}
		#endregion

		#region Nonpublic Methods
		/// <summary>
		/// Raiser for CollapsedStateChanged event.
		/// </summary>
		protected void RaiseCollapsedStateChanged()
		{
			if( CollapsedStateChanged != null )
			{
				CollapsedStateChanged( this, EventArgs.Empty );
			}
		}
		/// <summary>
		/// Raiser form RegionDeleted event.
		/// </summary>
		protected void RaiseRegionDeletedEvent()
		{
			if( RegionDeleted != null )
			{
				RegionDeleted( this, EventArgs.Empty );
			}
		}
		#endregion

		#region IComparable Members
		/// <summary>
		/// Compares objects.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>Standard IComparable return value.</returns>
		public int CompareTo( object obj )
		{
			if( obj == null ) throw new ArgumentNullException( "obj" );

			int result;
			CollapsableRegion secRegion = obj as CollapsableRegion;
			if( secRegion != null )
			{
				result = CompareTo( m_StartPoint );
			}
			else
			{
				long offset = ( long )obj;
				result = m_StartPoint.Offset.CompareTo( offset );
			}
			return result;
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Handler of the Deleted event of StartPoint.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="lNewOffset"></param>
		private void OnStartPointDeleted( ParsePoint sender, long lNewOffset )
		{
			RaiseRegionDeletedEvent();

			this.Start = null;
			this.End = null;
		}
		/// <summary>
		/// Handler of the Deleted event of EndPoint.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="lNewOffset"></param>
		private void OnEndPointDeleted( ParsePoint sender, long lNewOffset )
		{
			this.End = null;
		}
		/// <summary>
		/// Handler for OffsetChanged event of EndPoint of the region.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnEndPointOffsetChanged( object sender, ParsePointParameterChangedEventArgs e )
		{
			// If region is collapsed, than 
			if( e.OffsetChanged && !Collapsed )
			{
				ResetEnd();
			}
		}
		/// <summary>
		/// Resets region end info and detaches event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMonitoredPointOffsetChanged( object sender, ParsePointParameterChangedEventArgs e )
		{
			if( e.OffsetChanged )
			{
				IParsePoint point = ( IParsePoint )sender;
				point.ParsePointParameterChanged -= HandlerMonitoredPointOffsetChanged;
				ResetEnd();
			}
		}
		#endregion
	}
}