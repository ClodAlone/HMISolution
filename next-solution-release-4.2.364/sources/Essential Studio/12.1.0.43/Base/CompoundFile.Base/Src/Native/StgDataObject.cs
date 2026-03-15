#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

#region file using directives
#if !DOCIO
using Syncfusion.XlsIO.Diagnostics;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using HRESULT = System.UInt32;
#endregion

#if DOCIO
namespace Syncfusion.CompoundFile.DocIO.Native
#else
namespace Syncfusion.CompoundFile.XlsIO.Native
#endif
{
    #region Enumerations

    /// <summary>
    /// Predefined Clipboard Formats.
    /// </summary>
#if !(WINRT || WP )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    public enum CLIPFORMAT : int
    {
        /// <summary>
        /// Text clipboard format.
        /// </summary>
        CF_TEXT = 1,
        /// <summary>
        /// Bitmap clipboard format.
        /// </summary>
        CF_BITMAP = 2,
        /// <summary>
        /// METAFILEPICT clipboard format.
        /// </summary>
        CF_METAFILEPICT = 3,
        /// <summary>
        /// Sylk clipboard format.
        /// </summary>
        CF_SYLK = 4,
        /// <summary>
        /// Dif clipboard format.
        /// </summary>
        CF_DIF = 5,
        /// <summary>
        /// Tiff clipboard format.
        /// </summary>
        CF_TIFF = 6,
        /// <summary>
        /// Oemtext clipboard format.
        /// </summary>
        CF_OEMTEXT = 7,
        /// <summary>
        /// Dib clipboard format.
        /// </summary>
        CF_DIB = 8,
        /// <summary>
        /// Pallette clipboard format.
        /// </summary>
        CF_PALETTE = 9,
        /// <summary>
        /// Pendata clipboard format.
        /// </summary>
        CF_PENDATA = 10,
        /// <summary>
        /// Riff clipboard format.
        /// </summary>
        CF_RIFF = 11,
        /// <summary>
        /// Wave clipboard format.
        /// </summary>
        CF_WAVE = 12,
        /// <summary>
        /// Unicodetext clipboard format.
        /// </summary>
        CF_UNICODETEXT = 13,
        /// <summary>
        /// Enhmetafile clipboard format.
        /// </summary>
        CF_ENHMETAFILE = 14,
        /// <summary>
        /// Hdrop clipboard format.
        /// </summary>
        CF_HDROP = 15,
        /// <summary>
        /// Locale clipboard format.
        /// </summary>
        CF_LOCALE = 16,
        /// <summary>
        /// Max clipboard format.
        /// </summary>
        CF_MAX = 17,
        /// <summary>
        /// Ownerdisplay clipboard format.
        /// </summary>
        CF_OWNERDISPLAY = 0x0080,
        /// <summary>
        /// Dsptext clipboard format.
        /// </summary>
        CF_DSPTEXT = 0x0081,
        /// <summary>
        /// Dspbitmap clipboard format.
        /// </summary>
        CF_DSPBITMAP = 0x0082,
        /// <summary>
        /// Dspmetafilepict clipboard format.
        /// </summary>
        CF_DSPMETAFILEPICT = 0x0083,
        /// <summary>
        /// Dspenhmetafile clipboard format.
        /// </summary>
        CF_DSPENHMETAFILE = 0x008E,
        /// <summary>
        /// Privatefirst clipboard format.
        /// </summary>
        CF_PRIVATEFIRST = 0x0200,
        /// <summary>
        /// Privatelast clipboard format.
        /// </summary>
        CF_PRIVATELAST = 0x02FF,
        /// <summary>
        /// Gdiobjfirst clipboard format.
        /// </summary>
        CF_GDIOBJFIRST = 0x0300,
        /// <summary>
        /// Gdiobjlast clipboard format.
        /// </summary>
        CF_GDIOBJLAST = 0x03FF
    }

    /// <summary>
    /// The DVASPECT enumeration values specify the desired data or view aspect of the object when drawing or getting data.
    /// </summary>
#if !(WINRT || WP )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    public enum DVASPECT : int
    {
        /// <summary>
        /// Provides a representation of an object so it can be displayed as an embedded object inside of a container.
        /// </summary>
        DVASPECT_CONTENT = 1,
        /// <summary>
        /// Provides a thumbnail representation of an object so it can be displayed in a browsing tool.
        /// </summary>
        DVASPECT_THUMBNAIL = 2,
        /// <summary>
        /// Provides an iconic representation of an object.
        /// </summary>
        DVASPECT_ICON = 4,
        /// <summary>
        /// Provides a representation of the object on the screen as though it were printed to a printer 
        /// using the Print command from the File menu.
        /// </summary>
        DVASPECT_DOCPRINT = 8
    }

    /// <summary>
    /// The TYMED enumeration values indicate the type of storage medium being used in a data transfer.
    /// </summary>
    [Flags]
#if !(WINRT || WP )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    public enum TYMED : int
    {
        /// <summary>
        /// No data is being passed. 
        /// </summary>
        TYMED_NULL = 0,
        /// <summary>
        /// The storage medium is a global memory handle (HGLOBAL). Allocate the global 
        /// handle with the GMEM_SHARE flag.
        /// </summary>
        TYMED_HGLOBAL = 1,
        /// <summary>
        /// The storage medium is a disk file identified by a path. If the STGMEDIUM 
        /// punkForRelease member is NULL, the destination process should use OpenFile to delete the file.
        /// </summary>
        TYMED_FILE = 2,
        /// <summary>
        /// The storage medium is a stream object identified by an IStream pointer. Use 
        /// ISequentialStream::Read to read the data.
        /// </summary>
        TYMED_ISTREAM = 4,
        /// <summary>
        /// The storage medium is a storage component identified by an IStorage pointer.
        /// </summary>
        TYMED_ISTORAGE = 8,
        /// <summary>
        /// The storage medium is a GDI component (HBITMAP). If the STGMEDIUM punkForRelease member is 
        /// NULL, the destination process should use DeleteObject to delete the bitmap.
        /// </summary>
        TYMED_GDI = 16,
        /// <summary>
        /// The storage medium is a metafile (HMETAFILE). Use the Windows or WIN32 functions to 
        /// access the metafile's data.
        /// </summary>
        TYMED_MFPICT = 32,
        /// <summary>
        /// The storage medium is an enhanced metafile. If the STGMEDIUM punkForRelease member is NULL, 
        /// the destination process should use DeleteEnhMetaFile to delete the bitmap.
        /// </summary>
        TYMED_ENHMF = 64,
    }

    /// <summary>
    /// CSIDL values provide a unique system-independent way to identify special folders used frequently 
    /// by applications, but which may not have the same name or location on any given system.
    /// </summary>
    [Flags]
#if !(WINRT || WP )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    public enum CSIDL : int
    {
        /// <summary>
        /// The virtual folder representing the Windows desktop, the root of the namespace.
        /// </summary>
        CSIDL_DESKTOP = 0x0000,
        /// <summary>
        /// A virtual folder representing the Internet.
        /// </summary>
        CSIDL_INTERNET = 0x0001,
        /// <summary>
        /// The file system directory that contains the user's program groups (which are themselves file system directories).
        /// </summary>
        CSIDL_PROGRAMS = 0x0002,
        /// <summary>
        /// The virtual folder containing icons for the Control Panel applications.
        /// </summary>
        CSIDL_CONTROLS = 0x0003,
        /// <summary>
        /// The virtual folder containing installed printers.
        /// </summary>
        CSIDL_PRINTERS = 0x0004,
        /// <summary>
        /// The virtual folder representing the My Documents desktop item.
        /// </summary>
        CSIDL_PERSONAL = 0x0005,
        /// <summary>
        /// The file system directory that serves as a common repository for the user's favorite items.
        /// </summary>
        CSIDL_FAVORITES = 0x0006,
        /// <summary>
        /// The file system directory that corresponds to the user's Startup program group.
        /// </summary>
        CSIDL_STARTUP = 0x0007,
        /// <summary>
        /// The file system directory that contains shortcuts to the user's most recently used documents.
        /// </summary>
        CSIDL_RECENT = 0x0008,
        /// <summary>
        /// The file system directory that contains Send To menu items.
        /// </summary>
        CSIDL_SENDTO = 0x0009,
        /// <summary>
        /// The virtual folder containing the objects in the user's Recycle Bin.
        /// </summary>
        CSIDL_BITBUCKET = 0x000a,
        /// <summary>
        /// The file system directory containing Start menu items.
        /// </summary>
        CSIDL_STARTMENU = 0x000b,
        /// <summary>
        /// The file system directory used to physically store file objects on the desktop.
        /// </summary>
        CSIDL_DESKTOPDIRECTORY = 0x0010,
        /// <summary>
        /// The virtual folder representing My Computer, containing everything on the local 
        /// computer: storage devices, printers, and Control Panel.
        /// </summary>
        CSIDL_DRIVES = 0x0011,
        /// <summary>
        /// A virtual folder representing Network Neighborhood, the root of the network namespace hierarchy.
        /// </summary>
        CSIDL_NETWORK = 0x0012,
        /// <summary>
        /// A file system directory containing the link objects that may exist in the My Network Places virtual folder.
        /// </summary>
        CSIDL_NETHOOD = 0x0013,
        /// <summary>
        /// A virtual folder containing fonts.
        /// </summary>
        CSIDL_FONTS = 0x0014,
        /// <summary>
        /// The file system directory that serves as a common repository for document templates.
        /// </summary>
        CSIDL_TEMPLATES = 0x0015,
        /// <summary>
        /// The file system directory that contains the programs and folders that appear on the Start menu for all users.
        /// </summary>
        CSIDL_COMMON_STARTMENU = 0x0016,
        /// <summary>
        /// The file system directory that contains the directories for the common program groups that appear on the Start 
        /// menu for all users.
        /// </summary>
        CSIDL_COMMON_PROGRAMS = 0x0017,
        /// <summary>
        /// The file system directory that contains the programs that appear in the Startup folder for all users.
        /// </summary>
        CSIDL_COMMON_STARTUP = 0x0018,
        /// <summary>
        /// The file system directory that contains files and folders that appear on the desktop for all users.
        /// </summary>
        CSIDL_COMMON_DESKTOPDIRECTORY = 0x0019,
        /// <summary>
        /// The file system directory that serves as a common repository for application-specific data.
        /// </summary>
        CSIDL_APPDATA = 0x001a,
        /// <summary>
        /// The file system directory that contains the link objects that 
        /// can exist in the Printers virtual folder.
        /// </summary>
        CSIDL_PRINTHOOD = 0x001b,
        /// <summary>
        /// The file system directory that serves as a data repository for local (nonroaming) applications.
        /// </summary>
        CSIDL_LOCAL_APPDATA = 0x001c,
        /// <summary>
        /// The file system directory that corresponds to the user's nonlocalized Startup program group.
        /// </summary>
        CSIDL_ALTSTARTUP = 0x001d,
        /// <summary>
        /// The file system directory that corresponds to the nonlocalized Startup program group for all users.
        /// </summary>
        CSIDL_COMMON_ALTSTARTUP = 0x001e,
        /// <summary>
        /// The file system directory that serves as a common repository for favorite items common to all users.
        /// </summary>
        CSIDL_COMMON_FAVORITES = 0x001f,
        /// <summary>
        /// The file system directory that serves as a common repository for temporary Internet files.
        /// </summary>
        CSIDL_INTERNET_CACHE = 0x0020,
        /// <summary>
        /// The file system directory that serves as a common repository for Internet cookies.
        /// </summary>
        CSIDL_COOKIES = 0x0021,
        /// <summary>
        /// The file system directory that serves as a common repository for Internet history items.
        /// </summary>
        CSIDL_HISTORY = 0x0022,
        /// <summary>
        /// The file system directory containing application data for all users.
        /// </summary>
        CSIDL_COMMON_APPDATA = 0x0023,
        /// <summary>
        /// The Windows directory or SYSROOT. This corresponds to the %windir% or %SYSTEMROOT% 
        /// environment variables.
        /// </summary>
        CSIDL_WINDOWS = 0x0024,
        /// <summary>
        /// The Windows System folder.
        /// </summary>
        CSIDL_SYSTEM = 0x0025,
        /// <summary>
        /// The Program Files folder.
        /// </summary>
        CSIDL_PROGRAM_FILES = 0x0026,
        /// <summary>
        /// The file system directory that serves as a common repository for image files.
        /// </summary>
        CSIDL_MYPICTURES = 0x0027,
        /// <summary>
        /// The file system directory containing user profile folders.
        /// </summary>
        CSIDL_PROFILE = 0x0028,
        /// <summary>
        /// x86 system directory on RISC.
        /// </summary>
        CSIDL_SYSTEMX86 = 0x0029,
        /// <summary>
        /// x86 C:\Program Files on RISC. 
        /// </summary>
        CSIDL_PROGRAM_FILESX86 = 0x002a,
        /// <summary>
        /// A folder for components that are shared across applications.
        /// </summary>
        CSIDL_PROGRAM_FILES_COMMON = 0x002b,
        /// <summary>
        /// x86 Program Files\Common on RISC
        /// </summary>
        CSIDL_PROGRAM_FILES_COMMONX86 = 0x002c,
        /// <summary>
        /// The file system directory that serves as a common repository for document templates.
        /// </summary>
        CSIDL_COMMON_TEMPLATES = 0x002d,
        /// <summary>
        /// The file system directory that contains documents that are common to all users.
        /// </summary>
        CSIDL_COMMON_DOCUMENTS = 0x002e,
        /// <summary>
        /// The file system directory containing administrative tools for all users of the computer.
        /// </summary>
        CSIDL_COMMON_ADMINTOOLS = 0x002f,
        /// <summary>
        /// The file system directory that is used to store administrative tools for an individual user.
        /// </summary>
        CSIDL_ADMINTOOLS = 0x0030,
        /// <summary>
        /// Network and Dial-up Connections
        /// </summary>
        CSIDL_CONNECTIONS = 0x0031,
        /// <summary>
        /// Combine with CSIDL_ value to force folder creation in SHGetFolderPath().
        /// </summary>
        CSIDL_FLAG_CREATE = 0x8000,
        /// <summary>
        /// Combine with CSIDL_ value to return an unverified folder path.
        /// </summary>
        CSIDL_FLAG_DONT_VERIFY = 0x4000,
        /// <summary>
        ///  Mask for all possible flag values.
        /// </summary>
        CSIDL_FLAG_MASK = 0xFF00,
    }


    /// <summary>
    /// Can be time intensive.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
#if !(WINRT || WP )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    public struct STGMEDIUM
    {
        /// <summary>
        /// A platform-specific type that is used to represent a pointer to TYMED struct.
        /// </summary>
        public TYMED tymed;
        /// <summary>
        /// A platform-specific type that is used to represent a pointer or a handle to storage.
        /// </summary>
        public IntPtr pStorage;
        /// <summary>
        /// A platform-specific type that is used to represent a pointer or a handle to unknown.
        /// </summary>
        public IntPtr pUnkForRelease;
    }

    /// <summary>
    /// The FORMATETC structure is a generalized Clipboard format.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
#if !(WINRT || WP )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    public struct FORMATETC
    {
        /// <summary>
        /// Particular clipboard format of interest.
        /// </summary>
        public CLIPFORMAT cfFormat;
        /// <summary>
        /// Pointer to a DVTARGETDEVICE structure containing information about the target 
        /// device for which the data is being composed.
        /// </summary>
        public IntPtr ptd;
        /// <summary>
        /// One of the DVASPECT enumeration constants that indicate how much detail should 
        /// be contained in the rendering.
        /// </summary>
        public DVASPECT dwAspect;
        /// <summary>
        /// Part of the aspect when the data must be split across page boundaries. 
        /// </summary>
        public int lindex;
        /// <summary>
        /// One of the TYMED enumeration constants which indicate the type of storage medium 
        /// used to transfer the object's data.
        /// </summary>
        public TYMED tymed;
    }

    /// <summary>
    ///The DATADIR enumeration values specify the direction of the data flow in the 
    ///dwDirection parameter of the IDataObject::EnumFormatEtc method.
    /// </summary>
#if !(WINRT || WP )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    public enum DATADIR : int
    {
        /// <summary>
        /// Requests that IDataObject::EnumFormatEtc supply an enumerator for the 
        /// formats that can be specified in IDataObject::GetData.
        /// </summary>
        DATADIR_GET = 1,
        /// <summary>
        /// Requests that IDataObject::EnumFormatEtc supply an enumerator for the 
        /// formats that can be specified in IDataObject::SetData.
        /// </summary>
        DATADIR_SET = 2,
    }

    #endregion Enumerations

    ///<exclude/>
    /// <summary>
    /// The IEnumFORMATETC interface is used to enumerate an array of FORMATETC structures.
    /// </summary>
    [ComImport(),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        //ComConversionLoss(),
     Guid("00000103-0000-0000-c000-000000000046")]
#if !(WINRT || WP )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    [CLSCompliant(false)]
    public interface IComEnumFORMATETC
    {
        /// <summary>
        /// This method creates another enumerator that contains the same enumeration state as the current one.
        /// </summary>
        /// <param name="ppenum">Address of the IEnumFORMATETC* pointer variable that receives the interface 
        /// pointer to the enumeration object. </param>
        /// <returns>This method supports the standard return values E_INVALIDARG, E_OUTOFMEMORY, and E_UNEXPECTED.</returns>
        [PreserveSig()]
        HRESULT Clone(ref IComEnumFORMATETC ppenum);
        /// <summary>
        /// Retrieves the next celt items in the enumeration sequence.
        /// </summary>
        /// <param name="celt">Number of elements being requested.</param>
        /// <param name="rgelt">Array of size celt (or larger) of the elements of interest. 
        /// The type of this parameter depends on the item being enumerated.</param>
        /// <param name="pceltFetched">Pointer to the number of elements actually supplied in rgelt. 
        /// The caller can pass in NULL if celt is 1.</param>
        /// <returns>S_OK is returned if the number of elements supplied is celt; S_FALSE otherwise.</returns>
        [PreserveSig()]
        HRESULT RemoteNext(uint celt, ref FORMATETC rgelt, ref uint pceltFetched);
        /// <summary>
        /// This method resets the enumeration sequence to the beginning.
        /// </summary>
        /// <returns>If the method succeeds, the return value is S_OK.</returns>
        [PreserveSig()]
        HRESULT Reset();
        /// <summary>
        /// This method skips over the next specified number of elements in the enumeration sequence.
        /// </summary>
        /// <param name="celt">Number of elements to be skipped.</param>
        /// <returns>S_OK is returned if the number of elements skipped is celt; otherwise, S_FALSE.</returns>
        [PreserveSig()]
        HRESULT Skip(uint celt);
    }

    /// <summary>
    ///The IDataObject interface specifies methods that enable data transfer and notification of changes in data.
    /// </summary>
    [ComImport(),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
     Guid("0000010e-0000-0000-C000-000000000046")]
#if !(WINRT || WP )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    [CLSCompliant(false)]
    public interface IComDataObject
    {
        /// <summary>
        /// Called by a data consumer to obtain data from a source data object.
        /// </summary>
        /// <param name="pformatetcIn">Pointer to the FORMATETC structure that defines the 
        /// format, medium, and target device to use when passing the data. </param>
        /// <param name="pRemoteMedium">Pointer to the STGMEDIUM structure that indicates the 
        /// storage medium containing the returned data through its tymed member, and the responsibility 
        /// for releasing the medium through the value of its pUnkForRelease member.</param>
        /// <returns>
        /// S_OK - Data was successfully retrieved and placed in the storage medium provided.
        /// Otherwise error code.
        /// </returns>
        [PreserveSig()]
        HRESULT GetData(ref FORMATETC pformatetcIn, ref STGMEDIUM pRemoteMedium);
        /// <summary>
        /// Called by a data consumer to obtain data from a source data object. This method differs 
        /// from the GetData method in that the caller must allocate and free the specified storage medium.
        /// </summary>
        /// <param name="pformatetc">Pointer to the FORMATETC structure that defines the format, medium, and target 
        /// device to use when passing the data.</param>
        /// <param name="pRemoteMedium">Pointer to the STGMEDIUM structure that defines the storage medium 
        /// containing the data being transferred.</param>
        /// <returns>
        /// S_OK - Data was successfully retrieved and placed in the storage medium provided. 
        /// Otherwise error code.
        /// </returns>
        [PreserveSig()]
        HRESULT GetDataHere(ref FORMATETC pformatetc, ref STGMEDIUM pRemoteMedium);
        /// <summary>
        /// Determines whether the data object is capable of rendering the data described in the FORMATETC structure.
        /// </summary>
        /// <param name="pformatetc">Pointer to the FORMATETC structure defining the format, medium, and target 
        /// device to use for the query.</param>
        /// <returns>
        /// S_OK - Subsequent call to IDataObject::GetData would probably be successful.
        /// Otherwise error code.
        /// </returns>
        [PreserveSig()]
        HRESULT QueryGetData(ref FORMATETC pformatetc);
        /// <summary>
        /// Provides a standard FORMATETC structure that is logically equivalent to one that is more complex.
        /// </summary>
        /// <param name="pformatectIn">Pointer to the FORMATETC structure that defines the format, medium, 
        /// and target device that the caller would like to use to retrieve data in a subsequent call such 
        /// as IDataObject::GetData.</param>
        /// <param name="pformatetcOut">Pointer to a FORMATETC structure that contains the most general information 
        /// possible for a specific rendering, making it canonically equivalent to pFormatetcIn.</param>
        /// <returns>
        /// S_OK - The returned FORMATETC structure is different from the one that was passed. 
        /// Otherwise error code.
        /// </returns>
        [PreserveSig()]
        HRESULT GetCanonicalFormatEtc(ref FORMATETC pformatectIn, ref FORMATETC pformatetcOut);
        /// <summary>
        /// Called by an object containing a data source to transfer data to the object that implements this method.
        /// </summary>
        /// <param name="pformatetc">Pointer to the FORMATETC structure defining the format used by the data object 
        /// when interpreting the data contained in the storage medium.</param>
        /// <param name="pmedium">Pointer to the STGMEDIUM structure defining the storage medium in which the 
        /// data is being passed.</param>
        /// <param name="fRelease">If TRUE, the data object called, which implements IDataObject::SetData, owns 
        /// the storage medium after the call returns. This means it must free the medium after it has been used 
        /// by calling the ReleaseStgMedium function.</param>
        /// <returns>S_OK - Data was successfully transferred.
        /// Otherwise error code.
        /// </returns>
        [PreserveSig()]
        HRESULT SetData(ref FORMATETC pformatetc, ref STGMEDIUM pmedium, int fRelease);
        /// <summary>
        /// Creates an object for enumerating the FORMATETC structures for a data object. 
        /// These structures are used in calls to IDataObject::GetData or IDataObject::SetData.
        /// </summary>
        /// <param name="dwDirection">Direction of the data through a value from the enumeration DATADIR.</param>
        /// <param name="ppenumFormatEtc">Address of IEnumFORMATETC* pointer variable that receives 
        /// the interface pointer to the new enumerator object.</param>
        /// <returns>
        /// S_OK - Enumerator object was successfully created.
        /// E_NOTIMPL - The direction specified by dwDirection is not supported.
        /// OLE_S_USEREG - Requests that OLE enumerate the formats from the registry. 
        /// </returns>
        [PreserveSig()]
        HRESULT EnumFormatEtc(uint dwDirection, ref IComEnumFORMATETC ppenumFormatEtc);
        /// <summary>
        /// Called by an object supporting an advise sink to create a connection between a data object and the advise sink.
        /// </summary>
        /// <param name="pformatetc">Pointer to a FORMATETC structure that defines the format, 
        /// target device, aspect, and medium that will be used for future notifications.</param>
        /// <param name="advf">DWORD that specifies a group of flags for controlling the advisory connection.</param>
        /// <param name="pAdvSink">Pointer to the IAdviseSink interface on the advisory sink that will 
        /// receive the change notification.</param>
        /// <param name="pdwConnection">Pointer to a DWORD token that identifies this connection.</param>
        /// <returns>
        /// S_OK - The advisory connection was created.
        /// Otherwise error code.
        /// </returns>
        [PreserveSig()]
        HRESULT DAdvise(ref FORMATETC pformatetc, uint advf, IntPtr pAdvSink, ref uint pdwConnection);
        /// <summary>
        /// Destroys a notification connection that had been previously set up.
        /// </summary>
        /// <param name="dwConnection"> DWORD token that specifies the connection to 
        /// remove. Use the value returned by IDataObject::DAdvise when the connection was originally established.</param>
        /// <returns>
        /// S_OK - The specified connection was successfully deleted. 
        /// OLE_E_NOCONNECTION - The specified dwConnection is not a valid connection. 
        /// OLE_E_ADVISENOTSUPPORTED - This IDataObject implementation does not support notification.
        /// </returns>
        [PreserveSig()]
        HRESULT DUnadvise(uint dwConnection);
        /// <summary>
        /// Creates an object that can be used to enumerate the current advisory connections.
        /// </summary>
        /// <param name="ppenumAdvise">Address of IEnumSTATDATA* pointer variable that 
        /// receives the interface pointer to the new enumerator object.</param>
        /// <returns>
        /// S_OK - The enumerator object is successfully instantiated or there are no connections. 
        /// OLE_E_ADVISENOTSUPPORTED - Advisory notifications are not supported by this object. 
        /// </returns>
        [PreserveSig()]
        HRESULT EnumDAdvise(ref IntPtr ppenumAdvise);
    }


    /// <summary>
    /// Error and succes codes
    /// </summary>
#if !(WINRT || WP )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    [CLSCompliant(false)]
    public enum ComErrorCodes : uint
    {
        /// <summary>
        /// Success code.
        /// </summary>
        S_OK = 0x00000000,
        /// <summary>
        /// Success code.
        /// </summary>
        S_FALSE = 0x00000001,
        /// <summary>
        /// General access denied error.
        /// </summary>
        E_FAIL = 0x80004005,
        /// <summary>
        /// Ran out of memory.
        /// </summary>
        E_NOTIMPL = 0x80000001,
        /// <summary>
        ///  No such interface supported.
        /// </summary>
        E_INVALIDARG = 0x80070057,
        /// <summary>
        /// Catastrophic failure.
        /// </summary>
        E_UNEXPECTED = 0x8000FFFF,
        /// <summary>
        /// Invalid FORMATETC structure
        /// </summary>
        DV_E_FORMATETC = 0x80040064,
        /// <summary>
        /// This implementation doesn't take advises.
        /// </summary>
        OLE_E_ADVISENOTSUPPORTED = 0x80040003,
    }

    /// <summary>
    /// Class thet represents the data object entry.
    /// </summary>
#if !(WINRT || WP )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    internal class DataObjectEntry
    {
        #region Class members
        private STGMEDIUM m_medium;
        private FORMATETC m_format;
        private DATADIR m_dir;
        #endregion

        #region Class properties
        public STGMEDIUM Medium
        {
            get
            {
                return m_medium;
            }
        }

        public FORMATETC Format
        {
            get
            {
                return m_format;
            }
        }

        public DATADIR Direction
        {
            get
            {
                return m_dir;
            }
        }
        #endregion

        /// <summary>
        /// Creates new instance of DataObjectEntry.
        /// </summary>
        /// <param name="dir">Datadir object.</param>
        /// <param name="medium">Parent StgMedium object.</param>
        /// <param name="format">Parent Formatec object.</param>
        public DataObjectEntry(DATADIR dir, STGMEDIUM medium, FORMATETC format)
        {
            m_medium = medium;
            m_format = format;
            m_dir = dir;
        }
    }

    /// <summary>
    /// Class thet represents the enum formatec class.
    /// </summary>
    [ComVisible(true)]
#if !(WINRT || WP )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    [CLSCompliant(false)]
    public class ComEnumFORMATETC :
#if ( WINRT || WP )
        List<object>
#else
        ArrayList
#endif
        , IComEnumFORMATETC
    {
        #region Class members
        private int m_position = -1;
        #endregion

        #region IComEnumFORMATETC Members
        /// <summary>
        /// Creates another connection point enumerator with the same state as the 
        /// current enumerator to iterate over the same list.
        /// </summary>
        /// <param name="ppenum">Pointer to the returned IComEnumFORMATETC interface.</param>
        /// <returns>This method supports the standard return values 
        /// E_INVALIDARG, E_OUTOFMEMORY, and E_UNEXPECTED.</returns>
        public HRESULT Clone(ref IComEnumFORMATETC ppenum)
        {
            return (HRESULT)ComErrorCodes.E_NOTIMPL;
        }
        /// <summary>
        /// The RemoteNext method retrieves a specified number of HRESULT structures.
        /// </summary>
        /// <param name="celt">The number of STATSTG structures requested.</param>
        /// <param name="rgelt">An array of STATSTG structures returned.</param>
        /// <param name="pceltFetched">The number of STATSTG structures retrieved in the rgelt parameter.</param>
        /// <returns>S_OK - The number of STATSTG structures returned equals the number 
        /// specified in the celt parameter.
        /// Otherwise - error code.</returns>
        public HRESULT RemoteNext(uint celt, ref FORMATETC rgelt, ref uint pceltFetched)
        {
            int step = (celt > 1) ? 1 : (int)celt;
            m_position += step;

            // if we reach the end of array.
            if (m_position >= Count)
                return (HRESULT)ComErrorCodes.E_FAIL;

            rgelt = ((DataObjectEntry)this[m_position]).Format;
            pceltFetched = (uint)step;

            return (HRESULT)ComErrorCodes.S_OK;
        }
        /// <summary>
        /// Resets the enumeration sequence to the beginning.
        /// </summary>
        /// <returns>S_OK</returns>
        public HRESULT Reset()
        {
            m_position = -1;
            return (HRESULT)ComErrorCodes.S_OK;
        }
        /// <summary>
        /// Instructs the enumerator to skip the next celt elements in the enumeration 
        /// so that the next call to IEnumConnectionPoints::Next will not return those elements.
        /// </summary>
        /// <param name="celt">Number of elements to be skipped.</param>
        /// <returns>S_OK if the number of elements skipped is celt; otherwise, S_FALSE.</returns>
        public HRESULT Skip(uint celt)
        {
            if (m_position + celt > Count)
                return (HRESULT)ComErrorCodes.E_FAIL;

            m_position += (int)celt;
            return (HRESULT)ComErrorCodes.S_OK;
        }
        #endregion
    }
    ///<exclude/>
    /// <summary>
    /// ComDataObject class.
    /// </summary>
    [ComVisible(true),
     ClassInterface(ClassInterfaceType.None),
     Guid("0000010e-0000-0000-C000-000000000046")]
    //Guid( "3AB69369-5DAA-4739-9302-E360680077B8" ) ]
#if !(WINRT || WP )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    [CLSCompliant(false)]
    public class ComDataObject : IComDataObject
    {
        #region Class members
        private ComEnumFORMATETC m_storage = new ComEnumFORMATETC();
        #endregion

        #region IComDataObject Members
        /// <summary>
        /// Retrieves data.
        /// </summary>
        /// <param name="pformatetcIn">Point to Formatec structure.</param>
        /// <param name="pRemoteMedium">Point to Stgmedium sturcture.</param>
        /// <returns>Returns error code.</returns>
        public HRESULT GetData(ref FORMATETC pformatetcIn, ref STGMEDIUM pRemoteMedium)
        {
            Trace.WriteLine("GetData");
            if (m_storage.Count > 0)
            {
                DataObjectEntry entry = (DataObjectEntry)m_storage[0];
                pformatetcIn = entry.Format;
                pRemoteMedium = entry.Medium;

                return (HRESULT)ComErrorCodes.S_OK;
            }

            return (HRESULT)ComErrorCodes.DV_E_FORMATETC;
        }
        /// <summary>
        /// Similar to GetData, except the client must allocate the STGMEDIUM structure.
        /// </summary>
        /// <param name="pformatetc">Point to FORMATETC sturcture.</param>
        /// <param name="pRemoteMedium">Point to STGMEDIUM structure.</param>
        /// <returns>Returns E_NOTIMPL.</returns>
        public HRESULT GetDataHere(ref FORMATETC pformatetc, ref STGMEDIUM pRemoteMedium)
        {
            Trace.WriteLine("GetDataHere");
            return (HRESULT)ComErrorCodes.E_NOTIMPL;
        }
        /// <summary>
        /// Determines whether the data object supports a particular 
        /// FORMATETC structure for transferring data.
        /// </summary>
        /// <param name="pformatetc">Point to FORMATETC sturcture.</param>
        /// <returns>Returns E_NOTIMPL.</returns>
        public HRESULT QueryGetData(ref FORMATETC pformatetc)
        {
            Trace.WriteLine("QueryGetData");
            return (HRESULT)ComErrorCodes.E_NOTIMPL;
        }
        /// <summary>
        /// Retrieves a logically equivalent FORMATETC structure to one that is more complex.
        /// </summary>
        /// <param name="pformatectIn">Pointer to the FORMATETC structure that defines the format.</param>
        /// <param name="pformatetcOut">Pointer to a FORMATETC structure that contains 
        /// the most general information possible for a specific rendering.</param>
        /// <returns>Returns E_NOTIMPL.</returns>
        public HRESULT GetCanonicalFormatEtc(ref FORMATETC pformatectIn, ref FORMATETC pformatetcOut)
        {
            Trace.WriteLine("GetCanonicalFormatEtc");
            return (HRESULT)ComErrorCodes.E_NOTIMPL;
        }
        /// <summary>
        /// Sets the value for a specific data point.
        /// </summary>
        /// <param name="pformatetc">Pointer to the FORMATETC structure that defines the format.</param>
        /// <param name="pmedium">Point to STGMEDIUM structure.</param>
        /// <param name="fRelease">Int to relise.</param>
        /// <returns>Returns S_OK</returns>
        public HRESULT SetData(ref FORMATETC pformatetc, ref STGMEDIUM pmedium, int fRelease)
        {
            Trace.WriteLine("SetData");

            FORMATETC format = pformatetc;
            STGMEDIUM medium = pmedium;

            if (IntPtr.Zero != medium.pStorage)
            {
                Marshal.AddRef(medium.pStorage);
            }

            medium.pUnkForRelease = IntPtr.Zero;

            // Release interfaces if asked about this action.
            if (fRelease > 0)
            {
                if (IntPtr.Zero != pmedium.pStorage)
                {
                    Marshal.Release(pmedium.pStorage);
                }

                if (IntPtr.Zero != pmedium.pUnkForRelease)
                {
                    Marshal.Release(pmedium.pUnkForRelease);
                }
            }

            m_storage.Add(new DataObjectEntry(DATADIR.DATADIR_SET, medium, format));
            return (HRESULT)ComErrorCodes.S_OK;
        }
        /// <summary>
        /// Creates an enumerator to iterate through the FORMATETC structures 
        /// supported by the data object.
        /// </summary>
        /// <param name="dwDirection">Direction of the data through a value from the enumeration DATADIR.</param>
        /// <param name="ppenumFormatEtc">Address of IEnumFORMATETC* pointer variable that 
        /// receives the interface pointer to the new enumerator object.</param>
        /// <returns>Returns E_NOTIMPL.</returns>
        public HRESULT EnumFormatEtc(uint dwDirection, ref IComEnumFORMATETC ppenumFormatEtc)
        {
            Trace.WriteLine("EnumFormatEtc");
            ppenumFormatEtc = m_storage;
            return (HRESULT)ComErrorCodes.S_OK;
        }
        /// <summary>
        /// Establishes a connection between the data object and an advise sink.
        /// </summary>
        /// <param name="pformatetc">Pointer to a FORMATETC structure that defines the format.</param>
        /// <param name="advf">DWORD that specifies a group of flags for 
        /// controlling the advisory connection.</param>
        /// <param name="pAdvSink">Pointer to the IAdviseSink interface on the advisory sink 
        /// that will receive the change notification.</param>
        /// <param name="pdwConnection">Pointer to a DWORD token that identifies this connection.</param>
        /// <returns>Returns E_ADVISENOTSUPPORTED.</returns>
        public HRESULT DAdvise(ref FORMATETC pformatetc, uint advf, IntPtr pAdvSink, ref uint pdwConnection)
        {
            Trace.WriteLine("DAdvise");
            return (HRESULT)ComErrorCodes.OLE_E_ADVISENOTSUPPORTED;
        }
        /// <summary>
        /// Terminates a connection previously established through DAdvise.
        /// </summary>
        /// <param name="dwConnection">DWORD token that specifies the connection to remove. 
        /// Use the value returned by IDataObject::DAdvise when the connection 
        /// was originally established.</param>
        /// <returns>Returns E_ADVISENOTSUPPORTED.</returns>
        public HRESULT DUnadvise(uint dwConnection)
        {
            Trace.WriteLine("DUnadvise");
            return (HRESULT)ComErrorCodes.OLE_E_ADVISENOTSUPPORTED;
        }
        /// <summary>
        /// Creates an enumerator to iterate through the current advisory connections.
        /// </summary>
        /// <param name="ppenumAdvise">Address of IEnumSTATDATA* pointer variable 
        /// that receives the interface pointer to the new enumerator object.</param>
        /// <returns>Returns E_ADVISENOTSUPPORTED.</returns>
        public HRESULT EnumDAdvise(ref IntPtr ppenumAdvise)
        {
            Trace.WriteLine("EnumDAdvise");
            return (HRESULT)ComErrorCodes.OLE_E_ADVISENOTSUPPORTED;
        }
        #endregion

        #region IDataObject Members
        /*
    public bool GetDataPresent(Type format)
    {
      // TODO:  Add ComDataObject.GetDataPresent implementation
      return false;
    }

    bool System.Windows.Forms.IDataObject.GetDataPresent(string format)
    {
      // TODO:  Add ComDataObject.System.Windows.Forms.IDataObject.GetDataPresent implementation
      return false;
    }

    bool System.Windows.Forms.IDataObject.GetDataPresent(string format, bool autoConvert)
    {
      // TODO:  Add ComDataObject.System.Windows.Forms.IDataObject.GetDataPresent implementation
      return false;
    }

    object System.Windows.Forms.IDataObject.GetData(Type format)
    {
      // TODO:  Add ComDataObject.System.Windows.Forms.IDataObject.GetData implementation
      return null;
    }

    object System.Windows.Forms.IDataObject.GetData(string format)
    {
      // TODO:  Add ComDataObject.System.Windows.Forms.IDataObject.GetData implementation
      return null;
    }

    object System.Windows.Forms.IDataObject.GetData(string format, bool autoConvert)
    {
      // TODO:  Add ComDataObject.System.Windows.Forms.IDataObject.GetData implementation
      return null;
    }

    public string[] GetFormats()
    {
      // TODO:  Add ComDataObject.GetFormats implementation
      return null;
    }

    string[] System.Windows.Forms.IDataObject.GetFormats(bool autoConvert)
    {
      // TODO:  Add ComDataObject.System.Windows.Forms.IDataObject.GetFormats implementation
      return null;
    }

    void System.Windows.Forms.IDataObject.SetData(object data)
    {
      // TODO:  Add ComDataObject.System.Windows.Forms.IDataObject.SetData implementation
    }

    void System.Windows.Forms.IDataObject.SetData(Type format, object data)
    {
      // TODO:  Add ComDataObject.System.Windows.Forms.IDataObject.SetData implementation
    }

    void System.Windows.Forms.IDataObject.SetData(string format, object data)
    {
      // TODO:  Add ComDataObject.System.Windows.Forms.IDataObject.SetData implementation
    }

    void System.Windows.Forms.IDataObject.SetData(string format, bool autoConvert, object data)
    {
      // TODO:  Add ComDataObject.System.Windows.Forms.IDataObject.SetData implementation
    }
*/
        #endregion
    }
}