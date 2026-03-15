// <copyright file="Interop.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the static class Interop
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal static class Interop
    {
        #region Structures
        /// <summary>
        /// Contains parameters for the SHBrowseForFolder function and receives information about the folder selected by the user.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct BROWSEINFO
        {
            /// <summary>
            /// Handle to the owner window for the dialog box.
            /// </summary>
            public IntPtr hwndOwner;
            
            /// <summary>
            /// Pointer to an item identifier list (PIDL) specifying the location of the root folder from which to start browsing. Only the specified folder and any subfolders that are beneath it in the namespace hierarchy will appear in the dialog box. This member can be NULL; in that case, the namespace root (the desktop folder) is used.
            /// </summary>
            public IntPtr pidlRoot;
            
            /// <summary>
            /// Address of a buffer to receive the display name of the folder selected by the user. The size of this buffer is assumed to be MAX_PATH characters.
            /// </summary>
            public IntPtr pszDisplayName;
            
            /// <summary>
            /// Address of a null-terminated string that is displayed above the tree view control in the dialog box. This string can be used to specify instructions to the user.
            /// </summary>
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszTitle;
            
            /// <summary>
            /// Flags specifying the options for the dialog box. This member can include zero or a combination of flags.
            /// </summary>
            public int ulFlags;
            
            /// <summary>
            /// Address of an application-defined function that the dialog box calls when an event occurs. For more information, see the BrowseCallbackProc function. This member can be NULL.
            /// </summary>
            ////[MarshalAs( UnmanagedType.FunctionPtr )]            
            public IntPtr lpfn;
            
            /// <summary>
            /// Application-defined value that the dialog box passes to the callback function, if one is specified.
            /// </summary>
            public IntPtr lParam;
            
            /// <summary>
            /// Variable to receive the image associated with the selected folder. The image is specified as an index to the system image list.
            /// </summary>
            public int iImage;
        }
        #endregion

        #region Delegates
        /// <summary>
        /// Represents the call back
        /// </summary>
        /// <param name="hwnd">The h WND .</param>
        /// <param name="uMsg">The MSG UInt32.</param>
        /// <param name="lParam">The w param UInt32.</param>
        /// <param name="lpData">The l param Int32.</param>
        /// <returns>Int32 value type</returns>
        public delegate int BFFCALLBACK(IntPtr hwnd, UInt32 uMsg, Int32 lParam, Int32 lpData);
        #endregion

        #region Static methods
        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="hWnd">The h WND .</param>
        /// <param name="msg">The MSG UInt32.</param>
        /// <param name="wParam">The w param UInt32.</param>
        /// <param name="lParam">The l param Int32.</param>
        /// <returns>Int32 value type</returns>
        [DllImport("user32.dll")]
        public static extern Int32 SendMessage(
            IntPtr hWnd,
            UInt32 msg,
            UInt32 wParam,
            Int32 lParam);

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="hWnd">The h WND IntPtr.</param>
        /// <param name="msg">The MSG UInt32.</param>
        /// <param name="wParam">The w param UInt32.</param>
        /// <param name="lParam">The l param String.</param>
        /// <returns>Int32 value type</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 SendMessage(
            IntPtr hWnd,
            UInt32 msg,
            UInt32 wParam,
            [MarshalAs(UnmanagedType.LPWStr)]
            String lParam);

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="hWnd">The h WND IntPtr.</param>
        /// <param name="msg">The MSG UInt32.</param>
        /// <param name="wParam">The w param UInt32.</param>
        /// <param name="windowText">The window text.</param>
        /// <returns>Int32 windowText</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 SendMessage(
            IntPtr hWnd,
            UInt32 msg,
            UInt32 wParam,
            [Out] StringBuilder windowText);
        
        /// <summary>
        /// The GetWindowLong function retrieves information about the specified window. The function also retrieves the 32-bit (long) value at the specified offset into the extra window memory.
        /// </summary>
        /// <param name="hWnd">Handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="param">Specifies the zero-based offset to the value to be retrieved. Valid values are in the range zero through the number of bytes of extra window memory, minus four; for example, if you specified 12 or more bytes of extra memory, a value of 8 would be an index to the third 32-bit integer.</param>
        /// <returns>If the function succeeds, the return value is the requested 32-bit value. If the function fails, the return value is zero.</returns>
        [DllImport("user32.dll")]
        public static extern Int32 GetWindowLong(IntPtr hWnd, Int32 param);
        
        /// <summary>
        /// The SetWindowLong function changes an attribute of the specified window. The function also sets the 32-bit (long) value at the specified offset into the extra window memory.
        /// </summary>
        /// <param name="hWnd">Handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="lParam">Specifies the zero-based offset to the value to be set. Valid values are in the range zero through the number of bytes of extra window memory, minus the size of an integer.</param>
        /// <param name="wParam">Specifies the replacement value. </param>
        /// <returns>If the function succeeds, the return value is the previous value of the specified 32-bit integer. If the function fails, the return value is zero.</returns>
        [DllImport("user32.dll")]
        public static extern Int32 SetWindowLong(IntPtr hWnd, Int32 lParam, Int32 wParam);
        
        /// <summary>
        /// The FindWindowEx function retrieves a handle to a window whose class name and window name match the specified strings. The function searches child windows, beginning with the one following the specified child window. This function does not perform a case-sensitive search. 
        /// </summary>
        /// <param name="parentHandle">Handle to the parent window whose child windows are to be searched.</param>
        /// <param name="childAfter">Handle to a child window. The search begins with the next child window in the Z order. The child window must be a direct child window of hwndParent, not just a descendant window.</param>
        /// <param name="className">Pointer to a null-terminated string that specifies the class name or a class atom created by a previous call to the RegisterClass or RegisterClassEx function. The atom must be placed in the low-order word of lpszClass; the high-order word must be zero.</param>
        /// <param name="windowTitle"> Pointer to a null-terminated string that specifies the window name (the window's title). If this parameter is NULL, all window names match. </param>
        /// <returns>If the function succeeds, the return value is a handle to the window that has the specified class and window names. If the function fails, the return value is NULL.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);
        #endregion

        /// <summary>
        /// Class that imports shell api functions.
        /// </summary>
        internal static class Shell32
        {
            #region Enum
            /// <summary>
            /// Represents the SHGNO enum
            /// </summary>
            public enum SHGNO
            {
                /// <summary>
                /// Default (display purpose).
                /// </summary>
                SHGDN_NORMAL = 0x0000,
                
                /// <summary>
                /// Displayed under a folder (relative).
                /// </summary>
                SHGDN_INFOLDER = 0x0001,
                
                /// <summary>
                /// For in-place editing.
                /// </summary>
                SHGDN_FOREDITING = 0x1000,
                
                /// <summary>
                /// UI friendly parsing name (remove ugly stuff).
                /// </summary>
                SHGDN_FORADDRESSBAR = 0x4000,
                
                /// <summary>
                /// Parsing name for ParseDisplayName().
                /// </summary>
                SHGDN_FORPARSING = 0x8000
            }

            /// <summary>
            /// Represents the SHGNO :unit enum
            /// </summary>
            public enum SFGAO : uint
            {
                /// <summary>
                /// Objects can be copied    
                /// </summary>
                SFGAO_CANCOPY = 0x00000001,
                
                /// <summary>
                /// Objects can be moved     
                /// </summary>
                SFGAO_CANMOVE = 0x00000002,
                
                /// <summary>
                /// Objects can be linked    
                /// </summary>
                SFGAO_CANLINK = 0x00000004,
                
                /// <summary>
                /// supports BindToObject(IID_IStorage)
                /// </summary>
                SFGAO_STORAGE = 0x00000008,
                
                /// <summary>
                /// Objects can be renamed
                /// </summary>
                SFGAO_CANRENAME = 0x00000010,
                
                /// <summary>
                /// Objects can be deleted
                /// </summary>
                SFGAO_CANDELETE = 0x00000020,
                
                /// <summary>
                /// Objects have property sheets
                /// </summary>
                SFGAO_HASPROPSHEET = 0x00000040,
                
                /// <summary>
                /// Objects are drop target
                /// </summary>
                SFGAO_DROPTARGET = 0x00000100,
                
                /// <summary>
                /// This flag is a mask for the capability flags.
                /// </summary>
                SFGAO_CAPABILITYMASK = 0x00000177,
                
                /// <summary>
                /// object is encrypted (use alt color)
                /// </summary>
                SFGAO_ENCRYPTED = 0x00002000,
                
                /// <summary>
                /// 'slow' object
                /// </summary>
                SFGAO_ISSLOW = 0x00004000,
                
                /// <summary>
                /// ghosted icon
                /// </summary>
                SFGAO_GHOSTED = 0x00008000,
                
                /// <summary>
                /// Shortcut (link)
                /// </summary>
                SFGAO_LINK = 0x00010000,
                
                /// <summary>
                /// shared type
                /// </summary>
                SFGAO_SHARE = 0x00020000,
                
                /// <summary>
                /// read -only
                /// </summary>
                SFGAO_READONLY = 0x00040000,
                
                /// <summary>
                /// hidden object
                /// </summary>
                SFGAO_HIDDEN = 0x00080000,
                
                /// <summary>
                /// This flag is a mask for the display attributes.
                /// </summary>
                SFGAO_DISPLAYATTRMASK = 0x000FC000,
                
                /// <summary>
                /// may contain children with SFGAO_FILESYSTEM
                /// </summary>
                SFGAO_FILESYSANCESTOR = 0x10000000,
                
                /// <summary>
                /// support BindToObject(IID_IShellFolder)
                /// </summary>
                SFGAO_FOLDER = 0x20000000,
                
                /// <summary>
                /// is a win32 file system object (file/folder/root)
                /// </summary>
                SFGAO_FILESYSTEM = 0x40000000,
                
                /// <summary>
                /// may contain children with SFGAO_FOLDER
                /// </summary>
                SFGAO_HASSUBFOLDER = 0x80000000,
                
                /// <summary>
                /// This flag is a mask for the contents attributes.
                /// </summary>
                SFGAO_CONTENTSMASK = 0x80000000,
                
                /// <summary>
                /// invalidate cached information
                /// </summary>
                SFGAO_VALIDATE = 0x01000000,
                
                /// <summary>
                /// is this removable media?
                /// </summary>
                SFGAO_REMOVABLE = 0x02000000,
                
                /// <summary>
                /// Object is compressed (use alt color)
                /// </summary>
                SFGAO_COMPRESSED = 0x04000000,
                
                /// <summary>
                /// supports IShellFolder, but only implements CreateViewObject() (non-folder view)
                /// </summary>
                SFGAO_BROWSABLE = 0x08000000,
                
                /// <summary>
                /// is a non-enumerated object
                /// </summary>
                SFGAO_NONENUMERATED = 0x00100000,
                
                /// <summary>
                /// should show bold in explorer tree
                /// </summary>
                SFGAO_NEWCONTENT = 0x00200000,
                
                /// <summary>
                /// defunct type
                /// </summary>
                SFGAO_CANMONIKER = 0x00400000,
                
                /// <summary>
                /// defunct type
                /// </summary>
                SFGAO_HASSTORAGE = 0x00400000,
                
                /// <summary>
                /// supports BindToObject(IID_IStream)
                /// </summary>
                SFGAO_STREAM = 0x00400000,
                
                /// <summary>
                /// may contain children with SFGAO_STORAGE or SFGAO_STREAM
                /// </summary>
                SFGAO_STORAGEANCESTOR = 0x00800000,
                
                /// <summary>
                /// for determining storage capabilities, i.e. for open/save semantics
                /// </summary>
                SFGAO_STORAGECAPMASK = 0x70C50008
            }

            /// <summary>
            /// Represents the SHCONTF enum
            /// </summary>
            public enum SHCONTF
            {
                /// <summary>
                /// only want folders enumerated (SFGAO_FOLDER)
                /// </summary>
                SHCONTF_FOLDERS = 0x0020,
                
                /// <summary>
                /// include non folders
                /// </summary>
                SHCONTF_NONFOLDERS = 0x0040,
                
                /// <summary>
                /// show items normally hidden
                /// </summary>
                SHCONTF_INCLUDEHIDDEN = 0x0080,
                
                /// <summary>
                /// allow EnumObject() to return before validating enum
                /// </summary>
                SHCONTF_INIT_ON_FIRST_NEXT = 0x0100,
                
                /// <summary>
                /// hint that client is looking for printers
                /// </summary>
                SHCONTF_NETPRINTERSRCH = 0x0200,
                
                /// <summary>
                /// hint that client is looking sharable resources (remote shares)
                /// </summary>
                SHCONTF_SHAREABLE = 0x0400,
                
                /// <summary>
                /// include all items with accessible storage and their ancestors
                /// </summary>
                SHCONTF_STORAGE = 0x0800,
            }
            #endregion

            #region Structures
            /// <summary>
            /// Represents STREET struct
            /// </summary>
            [StructLayout(LayoutKind.Explicit)]
            public struct STRRET
            {
                /// <summary>
                /// One of the STRRET_* values
                /// </summary>
                [FieldOffset(0)]
                public UInt32 uType;
                
                /// <summary>
                /// must be freed by caller of GetDisplayNameOf
                /// </summary>
                [FieldOffset(4)]
                public IntPtr pOleStr;
                
                /// <summary>
                /// Not Used pStr
                /// </summary>
                [FieldOffset(4)]
                public IntPtr pStr;
                
                /// <summary>
                /// Offset into SHITEMID
                /// </summary>
                [FieldOffset(4)]
                public UInt32 uOffset;
                
                /// <summary>
                /// Buffer to fill in (ANSI)
                /// </summary>
                [FieldOffset(4)]
                public IntPtr cStr;
            }
            #endregion

            #region Static methods
            /// <summary>
            /// Translates a Shell namespace object's display name into an item 
            /// identifier list and returns the attributes of the object. This function is 
            /// the preferred method to convert a string to a pointer to an item identifier 
            /// list (PIDL). 
            /// </summary>
            /// <param name="pszName">Pointer to a zero-terminated wide string that contains the display name to parse</param>
            /// <param name="pbc">Optional bind context that controls the parsing operation. This parameter is normally set to NULL.</param>
            /// <param name="ppidl"> Address of a pointer to a variable of type ITEMIDLIST that receives the item identifier list for the object.</param>
            /// <param name="sfgaoIn">A ULONG value that specifies the attributes to query. To query for one or more attributes, initialize this parameter with the flags that represent the attributes of interest.</param>
            /// <param name="psfgaoOut">Pointer to a ULONG. On return, those attributes that are true for the object and were requested in sfgaoIn are set. An object's attribute flags can be zero or a combination of SFGAO flags.</param>
            /// <returns>Returns S_OK (0) if successful, or an error value otherwise.</returns>
            [DllImport("shell32.dll", CharSet = CharSet.Auto)]
            public static extern Int32 SHParseDisplayName(
                [MarshalAs(UnmanagedType.LPWStr)]
                String pszName,
                IntPtr pbc,
                out IntPtr ppidl,
                UInt32 sfgaoIn,
                out UInt32 psfgaoOut);
            
            /// <summary>
            /// Retrieves a pointer to the Shell's IMalloc interface.
            /// </summary>
            /// <param name="ppMalloc">Address of a pointer that receives the Shell's IMalloc interface pointer.</param>
            /// <returns>Returns S_OK (0) if successful, or an error value otherwise.</returns>
            [DllImport("shell32.DLL")]
            public static extern int SHGetMalloc(out IMalloc ppMalloc);
            
            /// <summary>
            /// Retrieves a pointer to the ITEMIDLIST structure of a special folder.
            /// </summary>
            /// <param name="hwndOwner">Handle to the owner window the client should specify if it displays a dialog box or message box.</param>
            /// <param name="nFolder">A CSIDL value that identifies the folder of interest.</param>
            /// <param name="ppidl">A pointer to an item identifier list (PIDL) specifying the folder's location relative to the root of the namespace (the desktop).</param>
            /// <returns>Returns S_OK (0) if successful, or an error value otherwise.</returns>
            [DllImport("shell32.DLL")]
            public static extern int SHGetSpecialFolderLocation(IntPtr hwndOwner, int nFolder, out IntPtr ppidl);
            
            /// <summary>
            /// Converts an item identifier list to a file system path. 
            /// </summary>
            /// <param name="pidl">Address of an item identifier list that specifies a file or directory location relative to the root of the namespace (the desktop).</param>
            /// <param name="path">Address of a buffer to receive the file system path. This buffer must be at least MAX_PATH characters in size. </param>
            /// <returns>Returns S_OK (0) if successful, or an error value otherwise.</returns>
            [DllImport("shell32.DLL")]
            public static extern int SHGetPathFromIDList(IntPtr pidl, StringBuilder path);
            
            /// <summary>
            /// Displays a dialog box enabling the user to select a Shell folder.
            /// </summary>
            /// <param name="bi">Pointer to a BROWSEINFO structure. On entry, this structure conveys information used to display the dialog box. On exit, it receives information concerning the selected folder.</param>
            /// <returns>Returns a pointer to an item identifier list (PIDL) specifying the location of the selected folder relative to the root of the namespace. If the user chooses the Cancel button in the dialog box, the return value is NULL.</returns>
            [DllImport("shell32.DLL", CharSet = CharSet.Auto)]
            public static extern IntPtr SHBrowseForFolder(ref BROWSEINFO bi);
            
            /// <summary>
            /// Accepts a STRRET structure returned by IShellFolder::GetDisplayNameOf that contains or points to a 
            /// string, and then returns that string as a BSTR.
            /// </summary>
            /// <param name="pstr">Pointer to a STRRET structure.</param>
            /// <param name="pidl">Pointer to an ITEMIDLIST uniquely identifying a file object or subfolder relative to the parent folder.</param>
            /// <param name="pbstr">Pointer to a variable of type BSTR that contains the converted string.</param>
            /// <returns>string as a BSTR</returns>
            [DllImport("shlwapi.dll")]
            public static extern Int32 StrRetToBSTR(
                ref STRRET pstr,
                IntPtr pidl,
                [MarshalAs(UnmanagedType.BStr)]
            out String pbstr);
            #endregion
        }
    }
}
