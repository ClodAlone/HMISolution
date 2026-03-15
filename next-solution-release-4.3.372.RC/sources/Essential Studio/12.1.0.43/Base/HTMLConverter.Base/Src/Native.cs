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
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;


namespace Syncfusion.HtmlConverter
{
  /// <summary>
  /// Contains native function declarations.
  /// </summary>
  internal class Native
  {
    #region Class constants
    /// <summary>
    /// Windows message.
    /// </summary>
    internal const uint PRF_CHECKVISIBLE = 0x00000001;
    /// <summary>
    /// Windows message.
    /// </summary>
    internal const uint PRF_CHILDREN     = 0x00000010;
    /// <summary>
    /// Windows message.
    /// </summary>
    internal const uint PRF_CLIENT       = 0x00000004;
    /// <summary>
    /// Windows message.
    /// </summary>
    internal const uint PRF_ERASEBKGND   = 0x00000008;
    /// <summary>
    /// Windows message.
    /// </summary>
    internal const uint PRF_NONCLIENT    = 0x00000002;
    /// <summary>
    /// Windows message.
    /// </summary>
    internal const uint PRF_OWNED        = 0x00000020;
    /// <summary>
    /// Windows message.
    /// </summary>
    internal const uint PW_CLIENTONLY    = 0x00000001;
    /// <summary>
    /// Windows message.
    /// </summary>
    internal const uint WM_PRINT         = 0x0317;
    /// <summary>
    /// Windows message.
    /// </summary>
    internal const uint WM_PRINTCLIENT   = 0x0318;
    /// <summary>
    /// HRESULT S_OK constant.
    /// </summary>
    internal const int S_OK = 0;
    /// <summary>
    /// HRESULT S_OK constant.
    /// </summary>
    internal const int S_FALSE = 1;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new object.
    /// </summary>
    public Native()
    {
    }
    #endregion

    #region Class native methods
    [DllImport("Gdi32.dll")] 
    public static extern IntPtr CreateCompatibleDC( IntPtr hdc ); 
    [DllImport("Gdi32.dll")] 
    public static extern bool DeleteDC( IntPtr hdc ); 
    [DllImport("Gdi32.dll")] 
    public static extern bool DeleteObject( IntPtr hObject ); 
    [DllImport("User32.dll")] 
    public static extern bool PrintWindow( IntPtr hwnd, IntPtr hdcBlt, uint nFlags ); 
    [DllImport("Gdi32.dll")] 
    public static extern IntPtr SelectObject( IntPtr hdc, IntPtr hgdiobj ); 
    [DllImport("User32.dll")] 
    public static extern int SendMessage( IntPtr  hWnd, uint Msg, uint wParam, uint lParam ); 
    [DllImport("ole32.dll", ExactSpelling=true, SetLastError=true)]
    public static extern bool OleDraw
      ([MarshalAs(UnmanagedType.IUnknown)] object pUnkn,
      int dwAspect, IntPtr hDC, ref Rectangle R );
    #endregion
  }

  #region RECT
  [ StructLayout( LayoutKind.Sequential ) ]
  internal struct RECT
  {
    public int left;
    public int top;
    public int right;
    public int bottom;

    public RECT( int x1, int y1, int x2, int y2 )
    {
      left = x1;
      top = y1;
      right = x2;
      bottom = y2;
    }

    public int Width
    {
      get
      {
        return right - left;
      }
    }

    public int Height
    {
      get
      {
        return bottom - top;
      }
    }

    public Point TopLeft
    {
      get
      {
        return new Point( left, top );
      }
    }

    public Size Size
    {
      get
      {
        return new Size( Width, Height );
      }
    }

    public override string ToString()
    {
      return string.Format( "{0}x{1}", TopLeft, Size );
    }

    public static implicit operator Rectangle( RECT rect )
    {
      return Rectangle.FromLTRB( rect.left, rect.top, rect.right, rect.bottom );
    }
    public static implicit operator RectangleF( RECT rect )
    {
      return RectangleF.FromLTRB( rect.left, rect.top, rect.right, rect.bottom );
    }
    public static implicit operator Size( RECT rect )
    {
      return new Size( rect.right - rect.left, rect.bottom - rect.top );
    }

    public static explicit operator RECT( Rectangle rect )
    {
      RECT rc = new RECT();

      rc.left = rect.Left;
      rc.right = rect.Right;
      rc.top = rect.Top;
      rc.bottom = rect.Bottom;

      return rc;
    }
  }
  #endregion

  #region Customization enums
  /// <summary>
  /// System enum.
  /// </summary>
  internal enum DOCHOSTUITYPE
  {
    DOCHOSTUITYPE_BROWSE    = 0,
    DOCHOSTUITYPE_AUTHOR    = 1
  }
  /// <summary>
  /// System enum.
  /// </summary>
  internal enum DOCHOSTUIDBLCLK
  {
    DOCHOSTUIDBLCLK_DEFAULT         = 0,
    DOCHOSTUIDBLCLK_SHOWPROPERTIES  = 1,
    DOCHOSTUIDBLCLK_SHOWCODE        = 2
  }
  /// <summary>
  /// System enum.
  /// </summary>
  internal enum DOCHOSTUIFLAG
  {
    DOCHOSTUIFLAG_DIALOG                    = 0x00000001,
    DOCHOSTUIFLAG_DISABLE_HELP_MENU         = 0x00000002,
    DOCHOSTUIFLAG_NO3DBORDER                = 0x00000004,
    DOCHOSTUIFLAG_SCROLL_NO                 = 0x00000008,
    DOCHOSTUIFLAG_DISABLE_SCRIPT_INACTIVE   = 0x00000010,
    DOCHOSTUIFLAG_OPENNEWWIN                = 0x00000020,
    DOCHOSTUIFLAG_DISABLE_OFFSCREEN         = 0x00000040,
    DOCHOSTUIFLAG_FLAT_SCROLLBAR            = 0x00000080,
    DOCHOSTUIFLAG_DIV_BLOCKDEFAULT          = 0x00000100,
    DOCHOSTUIFLAG_ACTIVATE_CLIENTHIT_ONLY   = 0x00000200,
    DOCHOSTUIFLAG_OVERRIDEBEHAVIORFACTORY   = 0x00000400,
    DOCHOSTUIFLAG_CODEPAGELINKEDFONTS       = 0x00000800,
    DOCHOSTUIFLAG_URL_ENCODING_DISABLE_UTF8 = 0x00001000,
    DOCHOSTUIFLAG_URL_ENCODING_ENABLE_UTF8  = 0x00002000,
    DOCHOSTUIFLAG_ENABLE_FORMS_AUTOCOMPLETE = 0x00004000,
    DOCHOSTUIFLAG_ENABLE_INPLACE_NAVIGATION = 0x00010000,
    DOCHOSTUIFLAG_IME_ENABLE_RECONVERSION   = 0x00020000,
    DOCHOSTUIFLAG_THEME                     = 0x00040000,
    DOCHOSTUIFLAG_NOTHEME                   = 0x00080000,
    DOCHOSTUIFLAG_NOPICS                    = 0x00100000,
    DOCHOSTUIFLAG_NO3DOUTERBORDER           = 0x00200000,
    DOCHOSTUIFLAG_DELEGATESIDOFDISPATCH     = 0x00400000
  }
  
  /// <summary>
  /// System enum.
  /// </summary>
  internal enum INVOKE_PARAMS
  {
    DLCTL_DLIMAGES = 0x00000010,
    DLCTL_VIDEOS = 0x00000020,
    DLCTL_BGSOUNDS = 0x00000040,
    DLCTL_NO_SCRIPTS = 0x00000080,
    DLCTL_NO_JAVA = 0x00000100,
    DLCTL_NO_RUNACTIVEXCTLS = 0x00000200,
    DLCTL_NO_DLACTIVEXCTLS = 0x00000400,
    DLCTL_DOWNLOADONLY = 0x00000800,
    DLCTL_NO_FRAMEDOWNLOAD = 0x00001000,
    DLCTL_RESYNCHRONIZE = 0x00002000,
    DLCTL_PRAGMA_NO_CACHE = 0x00004000,
    DLCTL_NO_BEHAVIORS = 0x00008000,
    DLCTL_NO_METACHARSET = 0x00010000,
    DLCTL_URL_ENCODING_DISABLE_UTF8 = 0x00020000,
    DLCTL_URL_ENCODING_ENABLE_UTF8 = 0x00040000,
    DLCTL_NOFRAMES = 0x00080000,
    DLCTL_FORCEOFFLINE = 0x10000000,
    DLCTL_NO_CLIENTPULL = 0x20000000,
    DLCTL_SILENT = 0x40000000,
    //DLCTL_OFFLINEIFNOTCONNECTED = 0x80000000,
    //DLCTL_OFFLINE = DLCTL_OFFLINEIFNOTCONNECTED
  }

  /// <summary>
  /// System struct.
  /// </summary>
  [ StructLayout( LayoutKind.Sequential ) ]
  internal struct DOCHOSTUIINFO
  {
    uint cbSize;
    public uint dwFlags;
    uint dwDoubleClick;
    [ MarshalAs( UnmanagedType.BStr ) ] string pchHostCss;
    [ MarshalAs( UnmanagedType.BStr ) ] string pchHostNS;
  }
 
  /// <summary>
  /// System struct.
  /// </summary>
  [ StructLayout( LayoutKind.Sequential ) ]
  internal struct tagMSG 
  {
    IntPtr hwnd;
    int lParam;
    uint message;
    mshtml.tagPOINT pt;
    uint time;
    uint wParam;
  } 
  #endregion

  #region Customization interfaces
  /// <summary>
  /// System interface.
  /// </summary>
  [ ComImport(),
  InterfaceType( ComInterfaceType.InterfaceIsIUnknown ),
  GuidAttribute( "bd3f23c0-d43e-11cf-893b-00aa00bdce1a" ) ]
  internal interface IDocHostUIHandler 
  {
    /// <summary>
    /// System method.
    /// </summary>
    [PreserveSig]
    int ShowContextMenu( uint dwID, ref mshtml.tagPOINT ppt, [ MarshalAs( UnmanagedType.IUnknown ) ] object pcmdtReserved, [ MarshalAs( UnmanagedType.IDispatch) ]object pdispReserved );
    /// <summary>
    /// System method.
    /// </summary>
    [PreserveSig]
    int GetHostInfo( ref DOCHOSTUIINFO pInfo );
    /// <summary>
    /// System method.
    /// </summary>
    [PreserveSig]
    int ShowUI( uint dwID, IntPtr pActiveObject, IntPtr pCommandTarget, IntPtr pFrame, IntPtr pDoc );
    /// <summary>
    /// System method.
    /// </summary>
    [PreserveSig]
    int HideUI();
    /// <summary>
    /// System method.
    /// </summary>
    [PreserveSig]
    int UpdateUI();
    /// <summary>
    /// System method.
    /// </summary>
    [PreserveSig]
    int EnableModeless( int fEnable );
    /// <summary>
    /// System method.
    /// </summary>
    [PreserveSig]
    int OnDocWindowActivate( int fActivate );
    /// <summary>
    /// System method.
    /// </summary>
    [PreserveSig]
    int OnFrameWindowActivate( int fActivate );
    /// <summary>
    /// System method.
    /// </summary>
    [PreserveSig]
    int ResizeBorder( ref mshtml.tagRECT prcBorder, [ MarshalAs( UnmanagedType.Interface ) ] IntPtr pUIWindow, int fRameWindow );
    /// <summary>
    /// System method.
    /// </summary>
    [PreserveSig]
    int TranslateAccelerator( ref tagMSG lpmsg, ref Guid pguidCmdGroup, uint nCmdID );
    /// <summary>
    /// System method.
    /// </summary>
    [PreserveSig]
    int GetOptionKeyPath( ref string pchKey, uint dw );
    /// <summary>
    /// System method.
    /// </summary>
    void GetDropTarget( IntPtr pDropTarget, out IntPtr ppDropTarget );
    /// <summary>
    /// System method.
    /// </summary>
    [PreserveSig]
    int GetExternal( ref object ppDispatch );
    /// <summary>
    /// System method.
    /// </summary>
    [PreserveSig]
    int TranslateUrl( uint dwTranslate, ref ushort pchURLIn, IntPtr ppchURLOut );
    /// <summary>
    /// System method.
    /// </summary>
    [PreserveSig]
    int FilterDataObject( IDataObject pDO, ref IDataObject ppDORet );
  }

  /// <summary>
  /// System interface.
  /// </summary>
  [ ComImport(),
  InterfaceType( ComInterfaceType.InterfaceIsIUnknown ),
  GuidAttribute( "3050f3f0-98b5-11cf-bb82-00aa00bdce0b" ) ]
  internal interface ICustomDoc
  {
    /// <summary>
    /// System method.
    /// </summary>
    [ PreserveSig ]
    int SetUIHandler( ref IDocHostUIHandler pUIHandler );
  }

  /// <summary>
  /// System interface.
  /// </summary>
  [ ComImport,
  Guid( "00000112-0000-0000-C000-000000000046" ),
  InterfaceType( ComInterfaceType.InterfaceIsIUnknown ) ]
  internal interface IOleObject
  {
    /// <summary>
    /// System method.
    /// </summary>
    void SetClientSite( IOleClientSite pClientSite );
    /// <summary>
    /// System method.
    /// </summary>
    void GetClientSite( IOleClientSite ppClientSite );
    /// <summary>
    /// System method.
    /// </summary>
    void SetHostNames( object szContainerApp, object szContainerObj );
    /// <summary>
    /// System method.
    /// </summary>
    void Close(uint dwSaveOption);
    /// <summary>
    /// System method.
    /// </summary>
    void SetMoniker( uint dwWhichMoniker, object pmk );
    /// <summary>
    /// System method.
    /// </summary>
    void GetMoniker( uint dwAssign, uint dwWhichMoniker, object ppmk );
    /// <summary>
    /// System method.
    /// </summary>
    void InitFromData( IDataObject pDataObject, bool fCreation, uint dwReserved );
    /// <summary>
    /// System method.
    /// </summary>
    void GetClipboardData( uint dwReserved, IDataObject ppDataObject );
    /// <summary>
    /// System method.
    /// </summary>
    void DoVerb( uint iVerb, uint lpmsg, object pActiveSite, uint lindex, uint hwndParent, uint lprcPosRect );
    /// <summary>
    /// System method.
    /// </summary>
    void EnumVerbs( object ppEnumOleVerb );
    /// <summary>
    /// System method.
    /// </summary>
    void Update();
    /// <summary>
    /// System method.
    /// </summary>
    void IsUpToDate();
    /// <summary>
    /// System method.
    /// </summary>
    void GetUserClassID( uint pClsid );
    /// <summary>
    /// System method.
    /// </summary>
    void GetUserType( uint dwFormOfType, uint pszUserType );
    /// <summary>
    /// System method.
    /// </summary>
    void SetExtent( uint dwDrawAspect, uint psizel );
    /// <summary>
    /// System method.
    /// </summary>
    void GetExtent( uint dwDrawAspect, uint psizel );
    /// <summary>
    /// System method.
    /// </summary>
    void Advise( object pAdvSink, uint pdwConnection );
    /// <summary>
    /// System method.
    /// </summary>
    void Unadvise( uint dwConnection );
    /// <summary>
    /// System method.
    /// </summary>
    void EnumAdvise( object ppenumAdvise );
    /// <summary>
    /// System method.
    /// </summary>
    void GetMiscStatus( uint dwAspect,uint pdwStatus );
    /// <summary>
    /// System method.
    /// </summary>
    void SetColorScheme( object pLogpal );
  };

  /// <summary>
  /// System interface.
  /// </summary>
  [ ComImport,
  Guid("00000118-0000-0000-C000-000000000046"),
  InterfaceType( ComInterfaceType.InterfaceIsIUnknown ) ]
  internal interface IOleClientSite
  {
    /// <summary>
    /// System method.
    /// </summary>
    int SaveObject();
    /// <summary>
    /// System method.
    /// </summary>
    int GetMoniker( uint dwAssign, uint dwWhichMoniker, object ppmk );
    /// <summary>
    /// System method.
    /// </summary>
    int GetContainer( object ppContainer );
    /// <summary>
    /// System method.
    /// </summary>
    int ShowObject();
    /// <summary>
    /// System method.
    /// </summary>
    int OnShowWindow( bool fShow );
    /// <summary>
    /// System method.
    /// </summary>
    int RequestNewObjectLayout();
  }

  [ ComImport, Guid( "0000010d-0000-0000-C000-000000000046" ),
  InterfaceType( ComInterfaceType.InterfaceIsIUnknown ),
  ComVisible( true ) ]
  internal interface IViewObject
  {
    [return: MarshalAs( UnmanagedType.I4 ) ]
    [ PreserveSig ]
    int Draw( [ MarshalAs( UnmanagedType.U4 ) ] uint dwAspect,
      int lindex, IntPtr pvAspect,
      [In] IntPtr ptd,
      IntPtr hicTargetDev,
      IntPtr hdcDraw,
      [MarshalAs(UnmanagedType.Struct)] ref RECT lprcBounds,
      [MarshalAs(UnmanagedType.Struct)] ref RECT lprcWBounds,
      IntPtr pfnContinue,
      [MarshalAs(UnmanagedType.U4)] uint dwContinue );
  }
  #endregion
  #region Authentication Interface
  [ComImport, GuidAttribute("79EAC9D0-BAF9-11CE-8C82-00AA004BA90B"),
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown),
    ComVisible(false)]
  public interface IAuthenticate
  {
      [return: MarshalAs(UnmanagedType.I4)]
      [PreserveSig]
      int Authenticate(ref IntPtr phwnd,
          ref IntPtr pszUsername,
          ref IntPtr pszPassword
          );
  }
  
  
  [ComImport,
      GuidAttribute("6d5140c1-7436-11ce-8034-00aa006009fa"),
      InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown),
      ComVisible(false)]
  public interface IServiceProvider
  {
      [return: MarshalAs(UnmanagedType.I4)]
      [PreserveSig]
      int QueryService(ref Guid guidService, ref Guid riid, out IntPtr
ppvObject);
  }
  #endregion
}
