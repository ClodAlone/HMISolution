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

#region file using directives
using System;

#if !WINRT
using System.Drawing;
#endif  
using System.ComponentModel;

using Syncfusion.XlsIO.Implementation;

#if !WINRT
using System.Security.Permissions;
#endif
#if  SILVERLIGHT || WINRT || WP
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// This class gives access to the XlsIO IApplication interface.
  /// </summary>
  public class ExcelEngine : IDisposable
  {
    #region Class members
    /// <summary>
    /// Storage of XlsIO Application object which provides an IApplication interface
    /// </summary>
    private ApplicationImpl  m_appl;
    /// <summary>
    /// Indicates if the class was disposed.
    /// TRUE - If the class was disposed, otherwise FALSE
    /// </summary>
    private bool m_bDisposed;
    /// <summary>
    /// TRUE - Throw exception when object is disposed and it's data was
    /// not saved, otherwise FALSE
    /// </summary>
    private bool m_bAskSaveOnDestroy;
    #endregion

    #region Class Properties
    /// <summary>
    /// Interface to the XlsIO Application which gives
    /// access to all supported functions.
    /// </summary>
    public IApplication Excel
    {
      [System.Diagnostics.DebuggerStepThrough]
      get
      {
        if( m_bDisposed )
          throw new ObjectDisposedException( "Application", "Cannot use dipose object." );

        return m_appl;
      }
    } 
    /// <summary>
    /// Dispose will throw an ExcelWorkbookNotSavedException when the workbook is not saved 
    /// and this property is set to TRUE. Default value is FALSE.
    /// </summary>
    public bool ThrowNotSavedOnDestroy
    {
      get
      {
        return m_bAskSaveOnDestroy;
      }
      set
      {
        m_bAskSaveOnDestroy = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public ExcelEngine()
    {
      bool isEvalExpired = false;

      if( IsSecurityGranted )
      {
        isEvalExpired = ValidateLicense();
      }

      m_appl = new ApplicationImpl(this);
      m_appl.EvalExpired = isEvalExpired;
    }
    /// <summary>
    /// Destructor.
    /// </summary>
    ~ExcelEngine()
    {
      Dispose();
    }
    /// <summary>
    /// Releases all resources used by this XlsIO object.
    /// </summary>
    /// <exception cref="ExcelWorkbookNotSavedException">
    /// Thrown when ThrowNotSavedOnDestoy property is set to TRUE and XlsIO
    /// object data was not saved.
    /// </exception>
    public void Dispose()
    {
      if( m_bDisposed ) return;

      if( m_bAskSaveOnDestroy && !m_appl.IsSaved )
        throw new ExcelWorkbookNotSavedException( "Object cannot be disposed." +
          " Save workbook or set property ThrowNotSavedOnDestoy to false." );

      IWorkbooks workbooks = m_appl.Workbooks;

      for( int i = workbooks.Count - 1; i >= 0; i-- )
      {
       (workbooks[i] as WorkbookImpl).ClearExtendedFormats();
        workbooks[ i ].Close();
      }
      m_appl.Dispose();
      m_appl = null;
      m_bDisposed = true;
      GC.SuppressFinalize( this );
    }
    /// <summary>
    /// Checks whether security permission can be granted. Read-only.
    /// </summary>
    internal static bool IsSecurityGranted
    {
      get
        {
            bool bResult = false;
#if !SILVERLIGHT && !WINRT && !WP
        SecurityPermission perm = new SecurityPermission( PermissionState.Unrestricted );
        
        try
        {
          perm.Demand();
          bResult = true;
        }
        catch( System.Security.SecurityException )
        {
        }

            return bResult;
#else
        return true;
#endif
      }
    }
    /// <summary>
    /// Checks whether license is valid.
    /// </summary>
    internal static bool ValidateLicense()
    {
      bool isEvalExpired = false;
#if !SILVERLIGHT && !WINRT && !WP
      try
      {
        AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
#if AllowUnsafeCode
        new Syncfusion.Core.Licensing.LicensedComponent( typeof( ExcelEngine ), out isEvalExpired );
#endif
      }
      finally
      {
        AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
      }
#endif
      return isEvalExpired;
    }
    #endregion
  }

  #region library exceptions
  /// <summary>
  /// Exception that will be thrown when the user tries to dispose XlsIO
  /// application without saving it.
  /// </summary>
#if !SILVERLIGHT && !WINRT && !WP
  [ Serializable ]
#endif
  [Syncfusion.Documentation.DocumentationExclude()]
  public class ExcelWorkbookNotSavedException : ApplicationException
  {
    /// <summary>
    /// Creates new ExcelWorkbookNotSavedException.
    /// </summary>
    /// <param name="message">Text that showed after rising.</param>
    public ExcelWorkbookNotSavedException( string message )
      : base( "Excel Binary workbook was not saved. " + message )
    {
    }
    /// <summary>
    /// Initializes a new instance of the Exception class with
    /// a specified error message and a reference to the inner
    /// exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">
    /// The error message that explains the reason for the exception.
    /// </param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception.
    /// If the innerException parameter is not a null reference
    /// (Nothing in Visual Basic), the current exception is raised
    /// in a catch block that handles the inner exception.
    /// </param>
    public ExcelWorkbookNotSavedException( string message, Exception innerException )
      : base( message, innerException )
    {
    }
  }
  /// <summary>
  /// Represents config xls class.
  /// </summary>
#if !SILVERLIGHT && !WINRT && !WP
  [ ToolboxBitmap(typeof(XlsIOConfig), "ToolBoxIcons.XlsIO.bmp")]
#endif
  [Syncfusion.Documentation.DocumentationExclude()]
  public class XlsIOConfig
#if !SILVERLIGHT && !WINRT && !WP
      :Component 
#endif
  {
    /// <summary>
    /// Creates xls config object.
    /// </summary>
    public XlsIOConfig()
    {
      if( ExcelEngine.IsSecurityGranted )
      {
        ExcelEngine.ValidateLicense();
      }
    }
    /// <summary>
    /// Gets Copyright string. Read-only.
    /// </summary>
    public string Copyright
    {
      get
      {
        return "Syncfusion, Inc. 2001 - 2004";
      }
    }
  }
  #endregion
}