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
using Syncfusion;
using Syncfusion.Windows.Forms.Edit;
using Syncfusion.Windows.Forms.Edit.Interfaces;

namespace Syncfusion.Windows.Forms.Edit.Utils
{
  /// <summary>
  /// Identifier of the long-runnign process.
  /// Counting startson it`s creation and stopped on call of Dispose method.
  /// </summary>
  internal class LongOperation
    : ILongOperation
    , IDisposable
  {
    #region Class members
    /// <summary>
    /// Unique identifier of the counter.
    /// </summary>
    private Guid m_id = Guid.NewGuid();
    /// <summary>
    /// Start time of the counter.
    /// </summary>
    private DateTime m_startTime;
    /// <summary>
    /// Sing of activity.
    /// </summary>
    private bool m_bRunning;
    /// <summary>
    /// Name of the operation.
    /// </summary>
    private string m_Name;
    /// <summary>
    /// Parent, who initiated this process.
    /// </summary>
    private ILongOperationControllerInternal m_parent;
    #endregion

    #region Class Properties
    /// <summary>
    /// GET ID of the operation.
    /// </summary>
    public Guid ID
    {
      get
      {
        return m_id;
      }
    }
    /// <summary>
    /// GET time of operation activity.
    /// </summary>
    public TimeSpan RunningTime
    {
      get
      {
        if( !m_bRunning )
          throw new ApplicationException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_95 );

        return DateTime.Now - m_startTime;
      }
    }
    /// <summary>
    /// GET name of the operation.
    /// </summary>
    public string Name
    {
      get
      {
        return m_Name;
      }
    }
    /// <summary>
    /// GET sing whether operation is running now.
    /// </summary>
    public bool IsRunning
    {
      get
      {
        return m_bRunning;
      }
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Stops operation.
    /// </summary>
    public void Stop()
    {
			TimeSpan time = RunningTime;
      m_parent.RaiseOperationEnd( this as ILongOperation );
      m_bRunning = false;
    }

    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance of LongOperation and initializes it's start time and running state.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name">Name of the operation.</param>
    public LongOperation( ILongOperationControllerInternal parent, string name )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      m_parent = parent;
      m_bRunning = true;
      m_startTime = DateTime.Now;
      m_Name = name;

      m_parent.RaiseOperationStart( this as ILongOperation );
    }
    /// <summary>
    /// Stops operation.
    /// </summary>
    public void Dispose()
    {
      if( m_bRunning )
        Stop();
    }
    #endregion
  }
}
