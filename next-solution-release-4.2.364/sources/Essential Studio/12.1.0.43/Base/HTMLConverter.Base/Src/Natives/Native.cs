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
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.CompilerServices;



namespace Syncfusion.HtmlConverter.Natives
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

    public const int S_NOTIMPL = unchecked((int)0x80004001);
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
    [DllImport("gdi32.dll")] 
    public static extern IntPtr CreateCompatibleDC( IntPtr hdc ); 
    [DllImport("gdi32.dll")] 
    public static extern bool DeleteDC( IntPtr hdc ); 
    [DllImport("gdi32.dll")] 
    public static extern bool DeleteObject( IntPtr hObject ); 
    [DllImport("user32.dll")] 
    public static extern bool PrintWindow( IntPtr hwnd, IntPtr hdcBlt, uint nFlags ); 
    [DllImport("gdi32.dll")] 
    public static extern IntPtr SelectObject( IntPtr hdc, IntPtr hgdiobj ); 
    [DllImport("user32.dll")] 
    public static extern int SendMessage( IntPtr  hWnd, uint Msg, uint wParam, uint lParam ); 
    [DllImport("ole32.dll", ExactSpelling=true, SetLastError=true)]
    public static extern bool OleDraw
      ([MarshalAs(UnmanagedType.IUnknown)] object pUnkn,
      int dwAspect, IntPtr hDC, ref Rectangle R );
    [DllImport("urlmon.dll", ExactSpelling = true, SetLastError = true)]
    public static extern int CoInternetSetFeatureEnabled(INTERNETFEATURELIST FeatureEntry, [MarshalAs(UnmanagedType.U4)] int dwFlags, bool fEnable);
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

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct _POINTL
  {
      public int x;
      public int y;
  }

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

  ///// <summary>
  ///// System struct.
  ///// </summary>
  [ComVisible(true), StructLayout(LayoutKind.Sequential)]
  public struct DOCHOSTUIINFO
  {
      [MarshalAs(UnmanagedType.U4)]
      public uint cbSize;
      [MarshalAs(UnmanagedType.U4)]
      public uint dwFlags;
      [MarshalAs(UnmanagedType.U4)]
      public uint dwDoubleClick;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pchHostCss;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pchHostNS;
  }
 
  /// <summary>
  /// System struct.
  /// </summary>
  [ StructLayout( LayoutKind.Sequential ) ]
  public struct tagMSG 
  {
    IntPtr hwnd;
    int lParam;
    uint message;
    tagPOINT pt;
    uint time;
    uint wParam;
  } 
  #endregion

  #region Customization interfaces
  [ComImport, ComVisible(true)]
  [Guid("C4D244B0-D43E-11CF-893B-00AA00BDCE1A")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  internal interface IDocHostShowUI
  {
      //[return: MarshalAs(UnmanagedType.I4)]
      [PreserveSig]
      int ShowMessage(
          IntPtr hwnd,
          [MarshalAs(UnmanagedType.LPWStr)] string lpstrText,
          [MarshalAs(UnmanagedType.LPWStr)] string lpstrCaption,
          [MarshalAs(UnmanagedType.U4)] uint dwType,
          [MarshalAs(UnmanagedType.LPWStr)] string lpstrHelpFile,
          [MarshalAs(UnmanagedType.U4)] uint dwHelpContext,
          [In, Out] ref int lpResult);

      //[return: MarshalAs(UnmanagedType.I4)]
      [PreserveSig]
      int ShowHelp(
          IntPtr hwnd,
          [MarshalAs(UnmanagedType.LPWStr)] string pszHelpFile,
          [MarshalAs(UnmanagedType.U4)] uint uCommand,
          [MarshalAs(UnmanagedType.U4)] uint dwData,
          [In, MarshalAs(UnmanagedType.Struct)] tagPOINT ptMouse,
          [Out, MarshalAs(UnmanagedType.IDispatch)] out object pDispatchObjectHit);
  }

  [Guid("BD3F23C0-D43E-11CF-893B-00AA00BDCE1A"), InterfaceType(1)]
  [ComImport]
  internal interface IDocHostUIHandler
  {
      [MethodImpl(4096)]
      void ShowContextMenu(uint dwID, ref tagPOINT ppt, [MarshalAs(25)] object pcmdtReserved, [MarshalAs(26)] object pdispReserved);
      [MethodImpl(4096)]
      void GetHostInfo(ref DOCHOSTUIINFO pInfo);
      [MethodImpl(4096)]
      void ShowUI(uint dwID, IntPtr pActiveObject, IntPtr pCommandTarget, IntPtr pFrame, IntPtr pDoc);
      [MethodImpl(4096)]
      void HideUI();
      [MethodImpl(4096)]
      void UpdateUI();
      [MethodImpl(4096)]
      void EnableModeless(int fEnable);
      [MethodImpl(4096)]
      void OnDocWindowActivate(int fActivate);
      [MethodImpl(4096)]
      void OnFrameWindowActivate(int fActivate);
      [MethodImpl(4096)]
      void ResizeBorder(ref tagRECT prcBorder, IntPtr pUIWindow, int fRameWindow);
      [MethodImpl(4096)]
      void TranslateAccelerator(ref tagMSG lpmsg, ref Guid pguidCmdGroup, uint nCmdID);
      [MethodImpl(4096)]
      void GetOptionKeyPath([MarshalAs(21)] out string pchKey, uint dw);
      [MethodImpl(4096)]
      void GetDropTarget([MarshalAs(28)] IDropTarget pDropTarget, [MarshalAs(28)] out IDropTarget ppDropTarget);
      [MethodImpl(4096)]
      void GetExternal([MarshalAs(26)] out object ppDispatch);
      [MethodImpl(4096)]
      void TranslateUrl(uint dwTranslate, ref ushort pchURLIn, [Out] IntPtr ppchURLOut);
      [MethodImpl(4096)]
      void FilterDataObject([MarshalAs(28)] IDataObject pDO, [MarshalAs(28)] out IDataObject ppDORet);
  }

  [Guid("00000122-0000-0000-C000-000000000046"), InterfaceType(1)]
  [ComImport]
  internal interface IDropTarget
  {
      [MethodImpl(4096)]
      void DragEnter([MarshalAs(28)] IDataObject pDataObj, uint grfKeyState, _POINTL pt, ref uint pdwEffect);
      [MethodImpl(4096)]
      void DragOver(uint grfKeyState, _POINTL pt, ref uint pdwEffect);
      [MethodImpl(4096)]
      void DragLeave();
      [MethodImpl(4096)]
      void Drop([MarshalAs(28)] IDataObject pDataObj, uint grfKeyState, _POINTL pt, ref uint pdwEffect);
  }

  /// <summary>
  /// System interface.
  /// </summary>
  [Guid("3050F3F0-98B5-11CF-BB82-00AA00BDCE0B"), InterfaceType(1)]
  [ComImport]
  internal interface ICustomDoc
  {
      [MethodImpl(4096)]
      void SetUIHandler([MarshalAs(28)] IDocHostUIHandler pUIHandler);
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
  internal interface IAuthenticate
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
  internal interface IServiceProvider
  {
      [return: MarshalAs(UnmanagedType.I4)]
      [PreserveSig]
      int QueryService(ref Guid guidService, ref Guid riid, out IntPtr ppvObject);
  }
  #endregion

  #region IInternetSecurityManager Interface
  [ComVisible(true), ComImport,
  GuidAttribute("79EAC9EE-BAF9-11CE-8C82-00AA004BA90B"),
  InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
  internal interface IInternetSecurityManager
  {
      [return: MarshalAs(UnmanagedType.I4)]
      [PreserveSig]
      int SetSecuritySite(
          [In] IntPtr pSite);

      [return: MarshalAs(UnmanagedType.I4)]
      [PreserveSig]
      int GetSecuritySite(
          out IntPtr pSite);

      [return: MarshalAs(UnmanagedType.I4)]
      [PreserveSig]
      int MapUrlToZone(
          [In, MarshalAs(UnmanagedType.LPWStr)] string pwszUrl,
          out UInt32 pdwZone,
          [In] UInt32 dwFlags);

      [return: MarshalAs(UnmanagedType.I4)]
      [PreserveSig]
      int GetSecurityId(
          [In, MarshalAs(UnmanagedType.LPWStr)] string pwszUrl,
          [Out] IntPtr pbSecurityId, [In, Out] ref UInt32 pcbSecurityId,
          [In] ref UInt32 dwReserved);

      [return: MarshalAs(UnmanagedType.I4)]
      [PreserveSig]
      int ProcessUrlAction(
          [In, MarshalAs(UnmanagedType.LPWStr)] string pwszUrl,
          UInt32 dwAction,
          IntPtr pPolicy, UInt32 cbPolicy,
          IntPtr pContext, UInt32 cbContext,
          UInt32 dwFlags,
          UInt32 dwReserved);

      [return: MarshalAs(UnmanagedType.I4)]
      [PreserveSig]
      int QueryCustomPolicy(
          [In, MarshalAs(UnmanagedType.LPWStr)] string pwszUrl,
          ref Guid guidKey,
          out IntPtr ppPolicy, out UInt32 pcbPolicy,
          IntPtr pContext, UInt32 cbContext,
          UInt32 dwReserved);

      [return: MarshalAs(UnmanagedType.I4)]
      [PreserveSig]
      int SetZoneMapping(
          UInt32 dwZone,
          [In, MarshalAs(UnmanagedType.LPWStr)] string lpszPattern,
          UInt32 dwFlags);

      [return: MarshalAs(UnmanagedType.I4)]
      [PreserveSig]
      int GetZoneMappings(
          [In] UInt32 dwZone,
          out System.Runtime.InteropServices.ComTypes.IEnumString ppenumString,
          [In] UInt32 dwFlags);
  }
  #endregion

  #region IInternetZoneManager Interface
  [ComImport, GuidAttribute("79eac9ef-baf9-11ce-8c82-00aa004ba90b"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
  internal interface IInternetZoneManager
  {
      [return: MarshalAs(UnmanagedType.I4)]
      [PreserveSig]
      int GetZoneActionPolicy(UInt32 dwZone, UInt32 dwAction, out IntPtr pPolicy, UInt32 cbPolicy, URLZONEREG urlZoneReg);
  }
  #endregion

 

}


