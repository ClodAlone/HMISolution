#region Copyright 
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Text;
using System.Runtime.InteropServices;
#endregion

#if DOCIO
namespace Syncfusion.CompoundFile.DocIO.Native
#else
namespace Syncfusion.CompoundFile.XlsIO.Native
#endif
{
    /// <summary>
    /// The STGTY enumeration values are used in the type member of the STATSTG
    /// structure to indicate the type of the storage element.
    /// </summary>
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    public enum STGTY
    {
        /// <summary>
        /// Indicates that the storage element is a storage object.
        /// </summary>
        STGTY_STORAGE = 1,
        /// <summary>
        /// Indicates that the storage element is a stream object.
        /// </summary>
        STGTY_STREAM = 2,
        /// <summary>
        /// Indicates that the storage element is a byte-array object.
        /// </summary>
        STGTY_LOCKBYTES = 3,
        /// <summary>
        /// Indicates that the storage element is a property storage object.
        /// </summary>
        STGTY_PROPERTY = 4
    }

    /// <summary>
    /// The STREAM_SEEK enumeration values specify the origin from which to
    /// calculate the new seek-pointer location.
    /// </summary>
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    public enum STREAM_SEEK
    {
        /// <summary>
        /// The new seek pointer is an offset relative to the beginning of
        /// the stream. In this case, the dlibMove parameter is the new seek
        /// position relative to the beginning of the stream.
        /// </summary>
        STREAM_SEEK_SET = 0,
        /// <summary>
        /// The new seek pointer is an offset relative to the current seek
        /// pointer location. In this case, the dlibMove parameter is the
        /// signed displacement from the current seek position.
        /// </summary>
        STREAM_SEEK_CUR = 1,
        /// <summary>
        /// The new seek pointer is an offset relative to the end of the stream.
        /// In this case, the dlibMove parameter is the new seek position
        /// relative to the end of the stream.
        /// </summary>
        STREAM_SEEK_END = 2
    }

    /// <summary>
    /// The LOCKTYPE enumeration values indicate the type of locking requested
    /// for the specified range of bytes.
    /// </summary>
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    public enum LOCKTYPE
    {
        /// <summary>
        /// If this lock is granted, the specified range of bytes can be opened
        /// and read any number of times, but writing to the locked range is
        /// prohibited except for the owner who granted this lock.
        /// </summary>
        LOCK_WRITE = 1,
        /// <summary>
        /// If this lock is granted, writing to the specified range of bytes is
        /// prohibited except by the owner granted this lock.
        /// </summary>
        LOCK_EXCLUSIVE = 2,
        /// <summary>
        /// If this lock is granted, no other LOCK_ONLYONCE lock can be obtained
        /// on the range. Usually this lock type is an alias for some other lock
        /// type. Thus, specific implementations can have additional behavior
        /// associated with this lock type.
        /// </summary>
        LOCK_ONLYONCE = 4
    }

    /// <summary>
    /// The STGM enumeration values are used in the IStorage, IStream, and
    /// IPropertySetStorage interfaces. These elements are often combined
    /// using an OR operator.
    /// </summary>
    [Flags]
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    public enum STGM : int
    {
        /// <summary>
        /// Indicates that the object is read-only, meaning that modifications
        /// cannot be made.
        /// </summary>
        STGM_READ = 0x00000000,
        /// <summary>
        /// STGM_WRITE lets you save changes to the object, but does not permit
        /// access to its data.
        /// </summary>
        STGM_WRITE = 0x00000001,
        /// <summary>
        /// STGM_READWRITE allows you to both access and modify an object's data.
        /// </summary>
        STGM_READWRITE = 0x00000002,
        /// <summary>
        /// Specifies that subsequent openings of the object are not denied read
        /// or write access. If no flag from the sharing group is specified,
        /// this flag is assumed.
        /// </summary>
        STGM_SHARE_DENY_NONE = 0x00000040,
        /// <summary>
        /// Prevents others from subsequently opening the object in STGM_READ mode.
        /// It is typically used on a root storage object.
        /// </summary>
        STGM_SHARE_DENY_READ = 0x00000030,
        /// <summary>
        /// Prevents others from subsequently opening the object for STGM_WRITE
        /// or STGM_READWRITE access.
        /// </summary>
        STGM_SHARE_DENY_WRITE = 0x00000020,
        /// <summary>
        /// Prevents others from subsequently opening the object in any mode. In
        /// transacted mode, sharing of STGM_SHARE_DENY_WRITE or STGM_SHARE_EXCLUSIVE
        /// can significantly improve performance since they don't require snapshotting.
        /// </summary>
        STGM_SHARE_EXCLUSIVE = 0x00000010,
        /// <summary>
        /// Opens the storage object with exclusive access to the most recently
        /// committed version.
        /// </summary>
        STGM_PRIORITY = 0x00040000,
        /// <summary>
        /// Indicates that an existing storage object or stream should be removed
        /// before the new one replaces it.
        /// </summary>
        STGM_CREATE = 0x00001000,
        /// <summary>
        /// Creates the new object while preserving existing data in a stream
        /// named "Contents".
        /// </summary>
        STGM_CONVERT = 0x00020000,
        /// <summary>
        /// Causes the create operation to fail if an existing object with the
        /// specified name exists.
        /// </summary>
        STGM_FAILIFTHERE = 0x00000000,
        /// <summary>
        /// In direct mode, each change to a storage or stream element is
        /// written as it occurs.
        /// </summary>
        STGM_DIRECT = 0x00000000,
        /// <summary>
        /// In transacted mode, changes are buffered and written only if an
        /// explicit commit operation is called.
        /// </summary>
        STGM_TRANSACTED = 0x00010000,
        /// <summary>
        /// In transacted mode, a temporary scratch file is usually used to
        /// save modifications until the Commit method is called.
        /// </summary>
        STGM_NOSCRATCH = 0x00100000,
        /// <summary>
        /// This flag is used when opening a storage object with STGM_TRANSACTED
        /// and without STGM_SHARE_EXCLUSIVE or STGM_SHARE_DENY_WRITE.
        /// </summary>
        STGM_NOSNAPSHOT = 0x00200000,
        /// <summary>
        /// STGM_SIMPLE is a mode that provides a much faster implementation of
        /// a compound file in a limited, but frequently used case.
        /// </summary>
        STGM_SIMPLE = 0x08000000,
        /// <summary>
        /// The STGM_DIRECT_SWMR supports direct mode for single-writer,
        /// multireader file operations.
        /// </summary>
        STGM_DIRECT_SWMR = 0x00400000,
        /// <summary>
        /// Indicates that the underlying file is to be automatically destroyed
        /// when the root storage object is released.
        /// </summary>
        STGM_DELETEONRELEASE = 0x04000000,
    }

    /// <summary>
    /// The STGFMT enumeration values specify the format of a storage object
    /// and are used in the StgCreateStorageEx and StgOpenStorageEx functions
    /// in the STGFMT parameter.
    /// </summary>
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    public enum STGFMT
    {
        /// <summary>
        /// Indicates that the file must be a compound file.
        /// </summary>
        STGFMT_STORAGE = 0,
        /// <summary>
        /// Indicates that the file must not be a compound file.
        /// </summary>
        STGFMT_FILE = 3,
        /// <summary>
        /// Indicates that the system will determine the file type and use the
        /// appropriate structured storage or property set implementation.
        /// </summary>
        STGFMT_ANY = 4,
        /// <summary>
        /// Indicates that the file must be a compound file and is similar to
        /// the STGFMT_STORAGE flag, but indicates that the compound-file form
        /// of the compound-file implementation must be used.
        /// </summary>
        STGFMT_DOCFILE = 5
    }

    /// <summary>
    /// Error code which StgOpenStorage method can return after execution.
    /// </summary>
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    [CLSCompliant(false)]
    public enum STG_ERRORS : uint
    {
        /// <summary>
        /// Success code.
        /// </summary>
        S_OK = 0x0,
        /// <summary>
        /// Filed.
        /// </summary>
        S_FAIL = 0x1,
        /// <summary>
        /// Access Denied.
        /// </summary>
        STG_E_ACCESSDENIED = 0x80030005,
        /// <summary>
        /// File already exists.
        /// </summary>
        STG_E_FILEALREADYEXISTS = 0x80030050,
        /// <summary>
        /// File could not be found.
        /// </summary>
        STG_E_FILENOTFOUND = 0x80030002,
        /// <summary>
        /// There is insufficient memory available to complete operation.
        /// </summary>
        STG_E_INSUFFICIENTMEMORY = 0x80030008,
        /// <summary>
        ///  Invalid flag error.
        /// </summary>
        STG_E_INVALIDFLAG = 0x800300FF,
        /// <summary>
        /// Unable to perform requested operation.
        /// </summary>
        STG_E_INVALIDFUNCTION = 0x80030001,
        /// <summary>
        /// Attempted an operation on an invalid object.
        /// </summary>
        STG_E_INVALIDHANDLE = 0x80030006,
        /// <summary>
        /// The name is not valid.
        /// </summary>
        STG_E_INVALIDNAME = 0x800300FC,
        /// <summary>
        /// Invalid pointer error.
        /// </summary>
        STG_E_INVALIDPOINTER = 0x80030009,
        /// <summary>
        /// A lock violation has occurred.
        /// </summary>
        STG_E_LOCKVIOLATION = 0x80030021,
        /// <summary>
        /// The compound file was not created with the STGM_SIMPLE flag.
        /// </summary>
        STG_E_NOTSIMPLEFORMAT = 0x80030112,
        /// <summary>
        /// The compound file was produced with a newer version of storage.
        /// </summary>
        STG_E_OLDDLL = 0x80030105,
        /// <summary>
        ///  The compound file was produced with an incompatible version of storage.
        /// </summary>
        STG_E_OLDFORMAT = 0x80030104,
        /// <summary>
        /// The path could not be found.
        /// </summary>
        STG_E_PATHNOTFOUND = 0x80030003,
        /// <summary>
        /// A share violation has occurred.
        /// </summary>
        STG_E_SHAREVIOLATION = 0x80030020,
        /// <summary>
        /// There are insufficient resources to open another file.
        /// </summary>
        STG_E_TOOMANYOPENFILES = 0x80030004,
    }

    /// <summary>
    /// The STGC enumeration constants specify the conditions for performing
    /// the commit operation in the IStorage::Commit and IStream::Commit methods.
    /// </summary>
    [Flags]
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    public enum STGC : int
    {
        /// <summary>
        /// You can specify this condition with STGC_CONSOLIDATE or some
        /// combination of the other three flags in this list of elements.
        /// </summary>
        STGC_DEFAULT = 0,
        /// <summary>
        /// The commit operation can overwrite existing data to reduce overall
        /// space requirements.
        /// </summary>
        STGC_OVERWRITE = 1,
        /// <summary>
        /// Prevents multiple users of a storage object from overwriting each
        /// other's changes.
        /// </summary>
        STGC_ONLYIFCURRENT = 2,
        /// <summary>
        /// Commits the changes to a write-behind disk cache, but does not save
        /// the cache to the disk.
        /// </summary>
        STGC_DANGEROUSLYCOMMITMERELYTODISKCACHE = 4,
        /// <summary>
        /// Microsoft Windows 2000/XP: Indicates that a storage should be
        /// consolidated after it is committed, resulting in a smaller file on disk.
        /// </summary>
        STGC_CONSOLIDATE = 8
    }

    /// <summary>
    /// Property IDs for the SummaryInformation Property Set.
    /// </summary>
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    public enum PIDSI
    {
        /// <summary>
        /// Title Id.
        /// </summary>
        Title = 0x00000002,
        /// <summary>
        /// Subject Id.
        /// </summary>
        Subject = 0x00000003,
        /// <summary>
        /// Author Id.
        /// </summary>
        Author = 0x00000004,
        /// <summary>
        /// Keywords Id.
        /// </summary>
        Keywords = 0x00000005,
        /// <summary>
        /// Comments Id.
        /// </summary>
        Comments = 0x00000006,
        /// <summary>
        /// Template Id.
        /// </summary>
        Template = 0x00000007,
        /// <summary>
        /// LastAuthor Id.
        /// </summary>
        LastAuthor = 0x00000008,
        /// <summary>
        /// Revnumber Id.
        /// </summary>
        Revnumber = 0x00000009,
        /// <summary>
        /// EditTime Id.
        /// </summary>
        EditTime = 0x0000000a,
        /// <summary>
        /// LastPrinted Id.
        /// </summary>
        LastPrinted = 0x0000000b,
        /// <summary>
        /// Create_dtm Id.
        /// </summary>
        Create_dtm = 0x0000000c,
        /// <summary>
        /// LastSave_dtm Id.
        /// </summary>
        LastSave_dtm = 0x0000000d,
        /// <summary>
        /// Pagecount Id.
        /// </summary>
        Pagecount = 0x0000000e,
        /// <summary>
        /// Wordcount Id.
        /// </summary>
        Wordcount = 0x0000000f,
        /// <summary>
        /// Charcount Id.
        /// </summary>
        Charcount = 0x00000010,
        /// <summary>
        /// Thumbnail Id.
        /// </summary>
        Thumbnail = 0x00000011,
        /// <summary>
        /// Appname Id.
        /// </summary>
        Appname = 0x00000012,
        /// <summary>
        /// Doc_security Id.
        /// </summary>
        Doc_security = 0x00000013,
    }

    /// <summary>
    /// Property IDs for the DocSummaryInformation Property Set.
    /// </summary>
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    public enum PIDDSI
    {
        /// <summary>
        /// Category Id.
        /// </summary>
        Category = 0x00000002,
        /// <summary>
        /// PresFormat Id.
        /// </summary>
        PresFormat = 0x00000003,
        /// <summary>
        /// ByteCount Id.
        /// </summary>
        ByteCount = 0x00000004,
        /// <summary>
        /// LineCount Id.
        /// </summary>
        LineCount = 0x00000005,
        /// <summary>
        /// ParCount Id.
        /// </summary>
        ParCount = 0x00000006,
        /// <summary>
        /// SlideCount Id.
        /// </summary>
        SlideCount = 0x00000007,
        /// <summary>
        /// NoteCount Id.
        /// </summary>
        NoteCount = 0x00000008,
        /// <summary>
        /// HiddenCount Id.
        /// </summary>
        HiddenCount = 0x00000009,
        /// <summary>
        /// MmclipCount Id.
        /// </summary>
        MmclipCount = 0x0000000A,
        /// <summary>
        /// Scale Id.
        /// </summary>
        Scale = 0x0000000B,
        /// <summary>
        /// HeadingPair Id.
        /// </summary>
        HeadingPair = 0x0000000C,
        /// <summary>
        /// DocParts Id.
        /// </summary>
        DocParts = 0x0000000D,
        /// <summary>
        /// Manager Id.
        /// </summary>
        Manager = 0x0000000E,
        /// <summary>
        /// Company Id.
        /// </summary>
        Company = 0x0000000F,
        /// <summary>
        /// LinksDirty Id.
        /// </summary>
        LinksDirty = 0x00000010,
    }
    /// <summary>
    /// Reserved global Property IDs.
    /// </summary>
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    [CLSCompliant(false)]
    public enum PID : uint
    {
        /// <summary>
        /// PID_DICTIONARY Id.
        /// </summary>
        PID_DICTIONARY = 0,
        /// <summary>
        /// PID_CODEPAGE Id.
        /// </summary>
        PID_CODEPAGE = 0x1,
        /// <summary>
        /// PID_FIRST_USABLE Id.
        /// </summary>
        PID_FIRST_USABLE = 0x2,
        /// <summary>
        /// PID_FIRST_NAME_DEFAULT Id.
        /// </summary>
        PID_FIRST_NAME_DEFAULT = 0xfff,
        /// <summary>
        /// PID_LOCALE Id.
        /// </summary>
        PID_LOCALE = 0x80000000,
        /// <summary>
        /// PID_MODIFY_TIME Id.
        /// </summary>
        PID_MODIFY_TIME = 0x80000001,
        /// <summary>
        /// PID_SECURITY Id.
        /// </summary>
        PID_SECURITY = 0x80000002,
        /// <summary>
        /// PID_BEHAVIOR Id.
        /// </summary>
        PID_BEHAVIOR = 0x80000003,
        /// <summary>
        /// PID_ILLEGAL Id.
        /// </summary>
        PID_ILLEGAL = 0xffffffff,
        /// <summary>
        /// PID_MIN_READONLY Id.
        /// </summary>
        PID_MIN_READONLY = 0x80000000,
        /// <summary>
        /// PID_MAX_READONLY Id.
        /// </summary>
        PID_MAX_READONLY = 0xbfffffff,
    }
    /// <summary>
    /// PRSPEC property ids.
    /// </summary>
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    [CLSCompliant(false)]
    public enum PRSPEC : uint
    {
        /// <summary>
        /// INVALID Id.
        /// </summary>
        PRSPEC_INVALID = 0xffffffff,
        /// <summary>
        /// LPWSTR Id.
        /// </summary>
        PRSPEC_LPWSTR = 0,
        /// <summary>
        /// PROPID Id.
        /// </summary>
        PRSPEC_PROPID = 1,
    }

    /// <summary>
    /// The STATSTG structure contains statistical information about an open storage,
    ///  stream, or byte-array object.
    /// </summary>
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    [StructLayout(LayoutKind.Sequential)]
    [CLSCompliant(false)]
    public struct STATSTG
    {
        /// <summary>
        /// Pointer to a NULL-terminated Unicode string containing the name. 
        /// Space for this string is allocated by the method called and freed by the caller
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pwcsName;
        /// <summary>
        /// Indicates the type of storage object. This is one of the 
        /// values from the STGTY enumeration.
        /// </summary>
        public STGTY type;
        /// <summary>
        /// Specifies the size in bytes of the stream or byte array.
        /// </summary>
        public ulong cbSize;
        /// <summary>
        /// Indicates the last modification time for this storage, stream, or byte array.
        /// </summary>
        public System.Runtime.InteropServices.ComTypes.FILETIME mtime;
        /// <summary>
        /// Indicates the creation time for this storage, stream, or byte array. 
        /// </summary>
        public System.Runtime.InteropServices.ComTypes.FILETIME ctime;
        /// <summary>
        /// Indicates the last access time for this storage, stream or byte array. 
        /// </summary>
        public System.Runtime.InteropServices.ComTypes.FILETIME atime;
        /// <summary>
        /// Indicates the access mode specified when the object was opened. 
        /// This member is only valid in calls to Stat methods.
        /// </summary>
        public uint grfMode;
        /// <summary>
        /// Indicates the types of region locking supported by the stream or byte array. 
        /// See the LOCKTYPE enumeration for the values available. 
        /// This member is not used for storage objects.
        /// </summary>
        public LOCKTYPE grfLocksSupported;
        /// <summary>
        /// Indicates the class identifier for the storage object; set to CLSID_NULL for new storage objects. 
        /// This member is not used for streams or byte arrays.
        /// </summary>
        public Guid clsid;
        /// <summary>
        /// Indicates the current state bits of the storage object; that is, the value most 
        /// recently set by the IStorage::SetStateBits method. 
        /// This member is not valid for streams or byte arrays. 
        /// </summary>
        public uint grfStateBits;
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        public uint reserved;
    }

    /// <summary>
    /// Structure that is used by STG API.
    /// </summary>
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    [StructLayout(LayoutKind.Sequential)]
    public struct STGOPTIONS
    {
        UInt16 usVersion;
        UInt16 reserved;
        uint ulSectorSize;
        [MarshalAs(UnmanagedType.LPWStr)]
        string pwcsTemplateFile;
    }

    /// <summary>
    /// Structure that is used by STG API.
    /// </summary>
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    [StructLayout(LayoutKind.Sequential)]
    [CLSCompliant(false)]
    public struct tagRemSNB
    {
        /// <summary>
        /// uint parameter.
        /// </summary>
        public uint ulCntStr;
        /// <summary>
        /// uint parameter.
        /// </summary>
        public uint ulCntChar;
        /// <summary>
        /// OLECHAR parameter.
        /// </summary>
        public IntPtr rgString;
    }
    /// <summary>
    /// Macros for parsing the OS Version of the Property Set Header.
    /// </summary>
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    [StructLayout(LayoutKind.Sequential)]
    [CLSCompliant(false)]
    public struct tagSTATPROPSETSTG
    {
        /// <summary>
        /// Time in UTC when this property set was last accessed.
        /// </summary>
        public System.Runtime.InteropServices.ComTypes.FILETIME atime;
        /// <summary>
        /// CLSID associated with this property set, specified when the property set was initially created 
        /// and possibly modified thereafter with IPropertyStorage::SetClass. If not set, the value will be CLSID_NULL.
        /// </summary>
        public Guid clsid;
        /// <summary>
        /// Time in UTC when this property set was created.
        /// </summary>
        public System.Runtime.InteropServices.ComTypes.FILETIME ctime;
        /// <summary>
        /// Os vorsion.
        /// </summary>
        public uint dwOSVersion;
        /// <summary>
        /// FMTID of the current property set, specified when the property set was initially created.
        /// </summary>
        public Guid fmtid;
        /// <summary>
        /// Flag values of the property set, as specified in IPropertySetStorage::Create. 
        /// </summary>
        public uint grfFlags;
        /// <summary>
        /// Time in Universal Coordinated Time (UTC) when the property set was last modified.
        /// </summary>
        public System.Runtime.InteropServices.ComTypes.FILETIME mtime;
    }

    ///<exclude/>
    /// <summary>
    /// The STATPROPSTG structure contains data about a single property in a property set. 
    /// This data is the property ID and type tag, and the optional string name that may be associated with the property.
    /// </summary>
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    //[StructLayout(LayoutKind.Sequential)]
    [CLSCompliant(false)]
    public struct tagSTATPROPSTG
    {
        /// <summary>
        /// A wide-character null-terminated Unicode string that contains the optional string name 
        /// associated with the property. May be NULL.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpwstrName;
        /// <summary>
        /// A 32-bit identifier that uniquely identifies the property within the property set. 
        /// All properties within property sets must have unique property identifiers.
        /// </summary>
        public uint propid;
        /// <summary>
        /// The property type.
        /// </summary>
        public VARTYPE vt;
    }

    /// <summary>
    /// The PROPSPEC structure is used by many of the methods of IPropertyStorage to specify a 
    /// property either by its property identifier (ID) or the associated string name.
    /// </summary>
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    [StructLayout(LayoutKind.Sequential)]
    [CLSCompliant(false)]
    public struct PROPSPEC
    {
        /// <summary>
        /// Indicates the union member used. This member can be one of the following values.
        /// </summary>
        public IntPtr ulKind;
        /// <summary>
        /// Specifies the value of the property ID. Use either this value or the following lpwstr, not both.
        /// </summary>
        public IntPtr propid;
    }

    /// <summary>
    /// This enumeration is used in VARIANT, TYPEDESC, OLE property sets, and safe arrays.
    /// </summary>
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    [Flags]
    public enum VARTYPE : int
    {
        /// <summary>
        /// Variable type is not specified.
        /// </summary>
        VT_EMPTY = 0,
        /// <summary>
        /// Variable type is 4-byte signed INT.
        /// </summary>
        VT_I4 = 3,
        /// <summary>
        /// Variable type is date.
        /// </summary>
        VT_DATE = 7,
        /// <summary>
        /// Variable type is binary string.
        /// </summary>
        VT_BSTR = 8,
        /// <summary>
        /// Variable type is Boolean; True=-1, False=0.
        /// </summary>
        VT_BOOL = 11,
        /// <summary>
        /// Variable type is VARIANT FAR*. 
        /// </summary>
        VT_VARIANT = 12,
        /// <summary>
        /// Variable type is int.
        /// </summary>
        VT_INT = 22,
        /// <summary>
        /// Variable type is LPSTR.
        /// </summary>
        VT_LPSTR = 30,
        /// <summary>
        /// Variable type is LPWSTR
        /// </summary>
        VT_LPWSTR = 31,
        /// <summary>
        /// Variable type is FILENAME string.
        /// </summary>
        VT_FILETIME = 64,
        /// <summary>
        /// Variable type is binary VECTOR.
        /// </summary>
        VT_VECTOR = 0x1000,
    }

    /// <summary>
    /// The IEnumSTATSTG interface enumerates an array of STATSTG structures. 
    /// These structures contain statistical data about open storage, stream, or byte array objects. 
    /// IEnumSTATSTG has the same methods as all enumerator interfaces: Next, Skip, Reset, and Clone. 
    /// </summary>
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
   Guid("0000000d-0000-0000-C000-000000000046")]
    [CLSCompliant(false)]
    public interface IEnumSTATSTG
    {
        /// <summary>
        /// The Next method retrieves a specified number of STATSTG structures, 
        /// that follow subsequently in the enumeration sequence. 
        /// If there are fewer than the requested number of STATSTG structures left 
        /// in the enumeration sequence, it retrieves the remaining STATSTG structures.
        /// </summary>
        /// <param name="celt">The number of STATSTG structures requested.</param>
        /// <param name="rgelt">An array of STATSTG structures returned.</param>
        /// <param name="pceltFetched"> The number of STATSTG structures retrieved in the rgelt parameter.</param>
        /// <returns>S_OK - The number of STATSTG structures returned equals the number 
        /// specified in the celt parameter.
        /// S_FALSE - The number of STATSTG structures returned is less than the number 
        /// specified in the celt parameter.</returns>
        int Next(uint celt,
          ref STATSTG rgelt,
          ref uint pceltFetched);
        /// <summary>
        /// The Skip method skips a specified number of STATSTG structures in the enumeration sequence.
        /// </summary>
        /// <param name="celt">The number of STATSTG structures to skip. </param>
        /// <returns>S_OK - The specified number of STATSTG structures were successfully skipped.
        /// S_FALSE - The number of STATSTG structures skipped is less than the celt parameter.</returns>
        int Skip(uint celt);
        /// <summary>
        /// The Reset method resets the enumeration sequence to the beginning of the STATSTG structure array.
        /// </summary>
        /// <returns>S_OK - The enumeration sequence was successfully reset to the 
        /// beginning of the enumeration.</returns>
        int Reset();
        /// <summary>
        /// The Clone method creates a new enumerator that contains the same enumeration state as 
        /// the current STATSTG structure enumerator. Using this method, a client can record a 
        /// particular point in the enumeration sequence and then return to that point at a later time. 
        /// The new enumerator supports the same IEnumSTATSTG interface.
        /// </summary>
        /// <param name="ppenum">A pointer to the variable that receives the IEnumSTATSTG interface pointer. 
        /// If the method is unsuccessful, the value of the ppenum parameter is undefined.</param>
        /// <returns>E_INVALIDARG - The ppenum parameter is NULL.
        /// E_OUTOFMEMORY - Insufficient memory.
        /// E_UNEXPECTED - An unexpected exception occurred.</returns>
        int Clone(ref IEnumSTATSTG ppenum);
    }

    /// <summary>
    /// For more information, refer to documentation on MSDN for interface with the same name.
    /// </summary>
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
   Guid("0000000c-0000-0000-C000-000000000046")]
    [CLSCompliant(false)]
    public interface IStream
    {
        /// <summary>
        /// Reads a specified number of bytes from the stream object into memory starting
        /// at the current seek pointer.
        /// </summary>
        /// <param name="pv">[out] Pointer to the buffer into which the stream data is read.</param>
        /// <param name="cb">[in] Specifies the number of bytes of data to attempt to read
        /// from the stream object.</param>
        /// <param name="pcbRead">[out] Pointer to a ULONG variable that receives the actual
        /// number of bytes read from the stream object. You can set this pointer to NULL to
        /// indicate that you are not interested in this value. In this case, this method
        /// does not provide the actual number of bytes read.</param>
        /// <returns>
        /// S_OK
        /// Data was successfully read from the stream object.
        /// S_FALSE
        /// The data could not be read from the stream object.
        /// E_PENDING
        /// Asynchronous storage only: Part or all of the data to be read is currently unavailable. For more information, see IFillLockBytes and Asynchronous Storage.
        /// STG_E_ACCESSDENIED
        /// The caller does not have enough permissions for reading this stream object.
        /// STG_E_INVALIDPOINTER
        /// One of the pointer values is not valid.
        /// STG_E_REVERTED
        /// The object has been invalidated by a revert operation above it in the transaction tree.
        /// </returns>
        int Read(
          [MarshalAs(UnmanagedType.LPArray)] byte[] pv,
          uint cb, ref uint pcbRead);
        /// <summary>
        /// Writes a specified number of bytes into the stream object starting at the
        /// current seek pointer.
        /// </summary>
        /// <param name="pv">[in] Pointer to the buffer containing the data that is to be
        /// written to the stream. A valid pointer must be provided for this parameter even
        /// when cb is zero.</param>
        /// <param name="cb">[in] The number of bytes of data to attempt to write into the
        /// stream. Can be zero.</param>
        /// <param name="pcbWritten">[out] Pointer to a ULONG variable where this method
        /// writes the actual number of bytes written to the stream object. The caller can
        /// set this pointer to NULL, in which case this method does not provide the actual
        /// number of bytes written.</param>
        /// <returns>
        /// S_OK
        /// The data was successfully written to the stream object.
        /// E_PENDING
        /// Asynchronous Storage only: Part or all of the data to be written is currently
        /// unavailable. For more information, see IFillLockBytes and Asynchronous Storage.
        /// STG_E_MEDIUMFULL
        /// The write operation was not completed because there is no space left on the storage
        /// device.
        /// STG_E_ACCESSDENIED
        /// The caller does not have enough permissions for writing to this stream object.
        /// STG_E_CANTSAVE
        /// Data cannot be written for reasons other than improper access or insufficient space.
        /// STG_E_INVALIDPOINTER
        /// One of the pointer values is not valid. The pv parameter must contain a valid pointer
        /// even if cb is zero.
        /// STG_E_REVERTED
        /// The object has been invalidated by a revert operation above it in the transaction tree.
        /// STG_E_WRITEFAULT
        /// The write operation was not completed due to a disk error. This value is also returned
        /// when this method attempts to write to a stream that was opened in simple mode (using
        /// the STGM_SIMPLE flag).
        /// </returns>
        int Write(
          [MarshalAs(UnmanagedType.LPArray)] byte[] pv,
          uint cb, ref uint pcbWritten);
        /// <summary>
        /// Changes the seek pointer to a new location relative to the beginning of
        /// the stream, the end of the stream, or the current seek pointer.
        /// </summary>
        /// <param name="dlibMove">[in] Displacement to be added to the location indicated by
        /// the dwOrigin parameter. If dwOrigin is STREAM_SEEK_SET, this is interpreted as an
        /// unsigned value rather than a signed value.</param>
        /// <param name="dwOrigin">[in] Specifies the origin for the displacement specified in
        /// dlibMove. The origin can be the beginning of the file, the current seek pointer,
        /// or the end of the file. See the STREAM_SEEK enumeration for the values.</param>
        /// <param name="plibNewPosition">[out] Pointer to the location where this method writes
        /// the value of the new seek pointer from the beginning of the stream. You can set this
        /// pointer to NULL to indicate that you are not interested in this value. In this case,
        /// this method does not provide the new seek pointer.</param>
        /// <returns>
        /// S_OK
        /// The seek pointer has been successfully adjusted.
        /// E_PENDING
        /// Asynchronous Storage only: Part or all of the stream's data is currently unavailable.
        /// For more information, see IFillLockBytes and Asynchronous Storage.
        /// STG_E_INVALIDPOINTER
        /// Indicates that the [out] parameter plibNewPosition points to invalid memory, because
        /// plibNewPosition is not read.
        /// STG_E_INVALIDFUNCTION
        /// The dwOrigin parameter contains an invalid value or the dlibMove parameter contains
        /// a bad offset value. For example, the result of the seek pointer is a negative offset
        /// value.
        /// STG_E_REVERTED
        /// The object has been invalidated by a revert operation above it in the transaction tree.
        /// </returns>
        int Seek(long dlibMove,
          System.IO.SeekOrigin dwOrigin,
          out long plibNewPosition);
        /// <summary>
        /// Changes the size of the stream object.
        /// </summary>
        /// <param name="libNewSize">[in] Specifies the new size of the stream as a number
        /// of bytes.</param>
        /// <returns></returns>
        int SetSize(ulong libNewSize);
        /// <summary>
        /// Copies a specified number of bytes from the current seek pointer in the stream to
        /// the current seek pointer in another stream.
        /// </summary>
        /// <param name="pstm">[in] Pointer to the destination stream. The stream pointed to
        /// by pstm can be a new stream or a clone of the source stream.</param>
        /// <param name="cb">[in] Specifies the number of bytes to copy from the source
        /// stream.</param>
        /// <param name="pcbRead">[out] Pointer to the location where this method writes the
        ///  actual number of bytes read from the source. You can set this pointer to NULL to
        ///  indicate that you are not interested in this value. In this case, this method
        ///  does not provide the actual number of bytes read.</param>
        /// <param name="pcbWritten">[out] Pointer to the location where this method writes
        /// the actual number of bytes written to the destination. You can set this pointer
        /// to NULL to indicate that you are not interested in this value. In this case,
        /// this method does not provide the actual number of bytes written.</param>
        /// <returns></returns>
        int CopyTo(IStream pstm,
          ulong cb,
          ref ulong pcbRead,
          ref ulong pcbWritten);
        /// <summary>
        /// Ensures that any changes made to a stream object open in transacted mode
        /// are reflected in the parent storage. If the stream object is open in direct
        /// mode, IStream::Commit has no effect other than flushing all memory buffers
        /// to the next-level storage object. The COM compound file implementation of
        /// streams does not support opening streams in transacted mode.
        /// </summary>
        /// <param name="grfCommitFlags">[in] Controls how the changes for the stream
        /// object are committed. See the STGC enumeration for a definition of these
        /// values.</param>
        /// <returns></returns>
        int Commit(uint grfCommitFlags);
        /// <summary>
        /// Discards all changes that have been made to a transacted stream since the
        /// last call to IStream::Commit.
        /// </summary>
        /// <returns></returns>
        int Revert();
        /// <summary>
        /// Restricts access to a specified range of bytes in the stream. Supporting
        /// this functionality is optional since some file systems do not provide it.
        /// </summary>
        /// <param name="libOffset"></param>
        /// <param name="cb"></param>
        /// <param name="dwLockType"></param>
        /// <returns></returns>
        int LockRegion(ulong libOffset,
          ulong cb,
          uint dwLockType);
        /// <summary>
        /// Removes the access restriction on a range of bytes previously restricted
        /// with IStream::LockRegion.
        /// </summary>
        /// <param name="libOffset"></param>
        /// <param name="cb"></param>
        /// <param name="dwLockType"></param>
        /// <returns></returns>
        int UnlockRegion(ulong libOffset,
          ulong cb,
          uint dwLockType);
        /// <summary>
        /// Retrieves the STATSTG structure for this stream.
        /// </summary>
        /// <param name="pstatstg"></param>
        /// <param name="grfStatFlag"></param>
        /// <returns></returns>
        int Stat(ref STATSTG pstatstg, uint grfStatFlag);
        /// <summary>
        /// Creates a new stream object that references the same bytes as the original
        /// stream but provides a separate seek pointer to those bytes.
        /// </summary>
        /// <param name="ppstm"></param>
        /// <returns></returns>
        int Clone(ref IStream ppstm);
    }

    /// <summary>
    /// Call the methods of IStorage to manage substorages or streams within the current storage. 
    /// This management includes creating, opening, or destroying substorages or streams, 
    /// as well as managing aspects such as time stamps, names, and so forth.
    /// </summary>
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
   Guid("0000000b-0000-0000-C000-000000000046")]
    [CLSCompliant(false)]
    public interface IStorage
    {
        /// <summary>
        /// The CreateStream method creates and opens a stream object with the specified 
        /// name contained in this storage object. All elements within a storage objects, 
        /// both streams and other storage objects, are kept in the same name space.
        /// </summary>
        /// <param name="pwcsName">A pointer to a wide character null-terminated Unicode 
        /// string that contains the name of the newly created stream. The name can be used 
        /// later to open or reopen the stream. The name must not exceed 31 characters in 
        /// length, not including the string terminator. The 000 through 01f characters, 
        /// serving as the first character of the stream/storage name, are reserved for use by OLE. 
        /// This is a compound file restriction, not a structured storage restriction.</param>
        /// <param name="grfMode">Specifies the access mode to use when opening the newly 
        /// created stream. For more information and descriptions of the possible values, 
        /// see STGM Constants.</param>
        /// <param name="reserved1">Reserved for future use; must be zero.</param>
        /// <param name="reserved2">Reserved for future use; must be zero. </param>
        /// <param name="ppstm">On return, pointer to the location of the new IStream interface pointer. 
        /// This is only valid if the operation is successful. 
        /// When an error occurs, this parameter is set to NULL.</param>
        /// <returns>
        /// S_OK - The new stream was successfully created. 
        /// E_PENDING - Asynchronous Storage only: Part or all of the 
        /// necessary data is currently unavailable. For more information, 
        /// see IFillLockBytes and Asynchronous Storage.
        /// STG_E_ACCESSDENIED - Not enough permissions to create stream. 
        /// STG_E_FILEALREADYEXISTS - The name specified for the stream already exists in the storage 
        /// object and the grfMode parameter includes the value STGM_FAILIFTHERE.
        /// STG_E_INSUFFICIENTMEMORY - The stream was not created due to a lack of memory.
        /// STG_E_INVALIDFLAG - The value specified for the grfMode parameter is not a valid STGM Constants value.
        /// STG_E_INVALIDFUNCTION - The specified combination of flags in the grfMode parameter is not supported;
        /// for example, when this method is called without the STGM_SHARE_EXCLUSIVE flag.
        /// STG_E_INVALIDNAME - Invalid value for pwcsName.
        /// STG_E_INVALIDPOINTER - The pointer specified for the stream object was invalid. 
        /// STG_E_INVALIDPARAMETER - One of the parameters was invalid.
        /// STG_E_REVERTED - The storage object has been invalidated by a revert operation 
        /// above it in the transaction tree.
        /// STG_E_TOOMANYOPENFILES - The stream was not created because there are too many open files.
        /// </returns>
        int CreateStream(
          [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
          STGM grfMode,
          uint reserved1,
          uint reserved2,
          ref IStream ppstm);
        /// <summary>
        /// The OpenStream method opens an existing stream object within this storage 
        /// object in the specified access mode.
        /// </summary>
        /// <param name="pwcsName">A pointer to a wide character null-terminated Unicode 
        /// string that contains the name of the stream to open. The 000 through 01f 
        /// characters, serving as the first character of the stream/storage name, are 
        /// reserved for use by OLE. This is a compound file restriction, not a structured 
        /// storage restriction.</param>
        /// <param name="cbReserved1">Reserved for future use; must be NULL.</param>
        /// <param name="grfMode"> Specifies the access mode to be assigned to the open stream. 
        /// For more information and descriptions of possible values, see STGM Constants. 
        /// Other modes you choose must at least specify STGM_SHARE_EXCLUSIVE when calling this 
        /// method in the compound file implementation.</param>
        /// <param name="reserved2">Reserved for future use; must be zero.</param>
        /// <param name="ppstm">A pointer to IStream pointer variable that receives the 
        /// interface pointer to the newly opened stream object. If an error occurs, *ppstm must be set to NULL.</param>
        /// <returns>
        /// S_OK - The stream was successfully opened.
        /// E_PENDING - Asynchronous Storage only: Part or all of the stream data is currently unavailable.
        /// STG_E_ACCESSDENIED - Not enough permissions to open stream. 
        /// STG_E_FILENOTFOUND - The stream with specified name does not exist.
        /// STG_E_INSUFFICIENTMEMORY - The stream was not opened due to a lack of memory. 
        /// STG_E_INVALIDFLAG - The value specified for the grfMode parameter is not a valid STGM Constants value.
        /// STG_E_INVALIDFUNCTION - The specified combination of flags in the grfMode parameter is not supported;
        /// for example, when this method is called without the STGM_SHARE_EXCLUSIVE flag.
        /// STG_E_INVALIDNAME - Invalid value for pwcsName.
        /// STG_E_INVALIDPOINTER - The pointer specified for the stream object was not valid.
        /// STG_E_INVALIDPARAMETER - One of the parameters was not valid. 
        /// STG_E_REVERTED - The storage object has been invalidated by a revert operation above it in the transaction tree.
        /// STG_E_TOOMANYOPENFILES - The stream was not opened because there are too many open files.
        /// </returns>
        int OpenStream(
          [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
          uint cbReserved1,
          STGM grfMode,
          uint reserved2,
          out IStream ppstm);
        /// <summary>
        /// The CreateStorage method creates and opens a new storage object nested within this storage 
        /// object with the specified name in the specified access mode.
        /// </summary>
        /// <param name="pwcsName">A pointer to a wide character null-terminated Unicode string that 
        /// contains the name of the newly created storage object. The name can be used later to 
        /// reopen the storage object. The name must not exceed 31 characters in length, not 
        /// including the string terminator. The 000 through 01f characters, serving as the 
        /// first character of the stream/storage name, are reserved for use by OLE. This is a 
        /// compound file restriction, not a structured storage restriction.</param>
        /// <param name="grfMode">A value that specifies the access mode to use when opening 
        /// the newly created storage object. For more information and a description of possible values</param>
        /// <param name="reserved1">Reserved for future use; must be zero.</param>
        /// <param name="reserved2">Reserved for future use; must be zero.</param>
        /// <param name="ppstg">A pointer, when successful, to the location of the IStorage pointer to 
        /// the newly created storage object. This parameter is set to NULL if an error occurs.</param>
        /// <returns>
        /// S_OK - The storage object was created successfully.
        /// E_PENDING - Asynchronous Storage only: Part or all of the necessary data is currently unavailable.
        /// STG_E_ACCESSDENIED - Not enough permissions to create storage object.
        /// STG_E_FILEALREADYEXISTS - The name specified for the storage object already exists in the 
        /// storage object and the grfMode parameter includes the flag STGM_FAILIFTHERE.
        /// STG_E_INSUFFICIENTMEMORY - The storage object was not created due to a lack of memory. 
        /// STG_E_INVALIDFLAG - The value specified for the grfMode parameter is not a valid STGM Constants value.
        /// STG_E_INVALIDFUNCTION - The specified combination of flags in the grfMode parameter is not supported. 
        /// STG_E_INVALIDNAME - Not a valid value for pwcsName.
        /// STG_E_INVALIDPOINTER - The pointer specified for the storage object was not valid. 
        /// STG_E_INVALIDPARAMETER - One of the parameters was not valid. 
        /// STG_E_REVERTED - The storage object has been invalidated by a revert operation above it in the transaction tree.
        /// STG_E_TOOMANYOPENFILES - The storage object was not created because there are too many open files. 
        /// STG_S_CONVERTED - The existing stream with the specified name was replaced with a new storage object 
        /// containing a single stream called CONTENTS. The new storage object will be added.
        /// </returns>
        int CreateStorage(
          [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
          STGM grfMode,
          uint reserved1,
          uint reserved2,
          out IStorage ppstg);
        /// <summary>
        /// The OpenStorage method opens an existing storage object with the specified name in the specified access mode.
        /// </summary>
        /// <param name="pwcsName">A pointer to a wide character null-terminated Unicode string that 
        /// contains the name of the storage object to open. The 000 through 01f characters, 
        /// serving as the first character of the stream/storage name, are reserved for use by 
        /// OLE. This is a compound file restriction, not a structured storage restriction. 
        /// It is ignored if pstgPriority is non-NULL. </param>
        /// <param name="pstgPriority">Must be NULL. A non-NULL value will return STG_E_INVALIDPARAMETER.</param>
        /// <param name="grfMode">Specifies the access mode to use when opening the storage object. 
        /// For descriptions of the possible values, see STGM Constants. 
        /// Other modes you choose must at least specify STGM_SHARE_EXCLUSIVE when calling this method.</param>
        /// <param name="snbExclude">Must be NULL. A non-NULL value will return STG_E_INVALIDPARAMETER.</param>
        /// <param name="reserved"> Reserved for future use; must be zero. </param>
        /// <param name="ppstg">When successful, pointer to the location of an IStorage pointer to 
        /// the opened storage object. This parameter is set to NULL if an error occurs.</param>
        /// <returns>
        /// S_OK - The storage object was opened successfully. 
        /// E_PENDING - Asynchronous Storage only: Part or all of the storage's data is currently unavailable.
        /// STG_E_ACCESSDENIED - Not enough permissions to open storage object.
        /// STG_E_FILENOTFOUND - The storage object with the specified name does not exist.
        /// STG_E_INSUFFICIENTMEMORY - The storage object was not opened due to a lack of memory. 
        /// STG_E_INVALIDFLAG - The value specified for the grfMode parameter is not a valid STGM Constants value.
        /// STG_E_INVALIDFUNCTION - The specified combination of flags in the grfMode parameter is not supported.
        /// STG_E_INVALIDNAME - Not a valid value for pwcsName. 
        /// STG_E_INVALIDPOINTER - The pointer specified for the storage object was not valid.
        /// STG_E_INVALIDPARAMETER - One of the parameters was not valid.
        /// STG_E_REVERTED - The storage object has been invalidated by a revert operation above it in the transaction tree.
        /// STG_E_TOOMANYOPENFILES - The storage object was not created because there are too many open files.
        /// STG_S_CONVERTED - The existing stream with the specified name was replaced with a new storage 
        /// object containing a single stream called CONTENTS. In direct mode, the new storage is 
        /// immediately written to disk. In transacted mode, the new storage is written to a 
        /// temporary storage in memory and later written to disk when it is committed.
        /// </returns>
        int OpenStorage(
          [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
          IntPtr pstgPriority,
          STGM grfMode,
          IntPtr snbExclude,
          uint reserved,
          out IStorage ppstg);
        /// <summary>
        /// The CopyTo method copies the entire contents of an open storage object to another storage object.
        /// </summary>
        /// <param name="ciidExclude">The number of elements in the array pointed to by rgiidExclude. 
        /// If rgiidExclude is NULL, then ciidExclude is ignored.</param>
        /// <param name="rgiidExclude">An array of interface identifiers (IIDs) that either the caller 
        /// knows about and does not want copied or that the storage object does not support but whose 
        /// state the caller will later explicitly copy. </param>
        /// <param name="snbExclude">A string name block (refer to SNB) that specifies a block of storage
        /// or stream objects that are not to be copied to the destination. These elements are not created 
        /// at the destination. If IID_IStorage is in the rgiidExclude array, this parameter is ignored. 
        /// This parameter may be NULL. </param>
        /// <param name="pstgDest">Pointer to the open storage object into which this storage object is to be copied.</param>
        /// <returns>
        /// S_OK - The storage object was successfully copied.
        /// E_PENDING - Asynchronous Storage only: Part or all of the data to be copied is currently unavailable.
        /// STG_E_ACCESSDENIED - The destination storage object is a child of the source storage object.
        /// STG_E_INSUFFICIENTMEMORY - The copy was not completed due to a lack of memory.
        /// Otherwise - Error code.
        /// </returns>
        int CopyTo(uint ciidExclude,
            //ref Guid rgiidExclude,
            //ref tagRemSNB snbExclude,
          IntPtr rgiidExclude,
          IntPtr snbExclude,
          IStorage pstgDest);

        /// <summary>
        /// The MoveElementTo method copies or moves a substorage or stream from this storage 
        /// object to another storage object.
        /// </summary>
        /// <param name="pwcsName">Pointer to a wide character null-terminated Unicode string 
        /// that contains the name of the element in this storage object to be moved or copied.</param>
        /// <param name="pstgDest"> IStorage pointer to the destination storage object.</param>
        /// <param name="pwcsNewName">Pointer to a wide character null-terminated unicode string 
        /// that contains the new name for the element in its new storage object.</param>
        /// <param name="grfFlags">Specifies whether the operation should be a move (STGMOVE_MOVE)
        /// or a copy (STGMOVE_COPY). See the STGMOVE enumeration.</param>
        /// <returns>S_OK - The storage object was successfully copied or moved.
        /// Otherwise error code.
        /// </returns>
        int MoveElementTo(string pwcsName,
          IStorage pstgDest,
          string pwcsNewName,
          uint grfFlags);

        /// <summary>
        /// The Commit method ensures that any changes made to a storage object open in transacted 
        /// mode are reflected in the parent storage.
        /// </summary>
        /// <param name="grfCommitFlags">Controls how the changes are committed to the storage object.</param>
        /// <returns>
        /// S_OK - Changes to the storage object were successfully committed to the parent level. 
        /// If STGC_CONSOLIDATE was specified, the storage was successfully consolidated, 
        /// or the storage was already too compact to consolidate further
        /// Otherwise error code.
        /// </returns>
        int Commit(uint grfCommitFlags);
        /// <summary>
        /// The Revert method discards all changes that have been made to the storage object since the last commit operation.
        /// </summary>
        /// <returns>
        /// S_OK - The revert operation was successful. 
        /// Otherwise error code.
        /// </returns>
        int Revert();
        /// <summary>
        /// The EnumElements method retrieves a pointer to an enumerator object that can be used 
        /// to enumerate the storage and stream objects contained within this storage object.
        /// </summary>
        /// <param name="reserved1">Reserved for future use; must be zero.</param>
        /// <param name="reserved2">Reserved for future use; must be zero.</param>
        /// <param name="reserved3">Reserved for future use; must be zero.</param>
        /// <param name="ppenum">Pointer to IEnumSTATSTG* pointer variable that receives the 
        /// interface pointer to the new enumerator object.</param>
        /// <returns>
        /// S_OK - The enumerator object was successfully returned.
        /// Otherwise error code.
        /// </returns>
        int EnumElements(uint reserved1,
          IntPtr reserved2,
          uint reserved3,
          ref IEnumSTATSTG ppenum);
        /// <summary>
        /// The DestroyElement method removes the specified storage or stream from this storage object.
        /// </summary>
        /// <param name="pwcsName">Pointer to a wide character null-terminated Unicode string that 
        /// contains the name of the storage or stream to be removed.</param>
        /// <returns>
        /// S_OK - The element was successfully removed.
        /// Otherwise error code.
        /// </returns>
        int DestroyElement(string pwcsName);
        /// <summary>
        /// The RenameElement method renames the specified substorage or stream in this storage object.
        /// </summary>
        /// <param name="pwcsOldName">Pointer to a wide character null-terminated Unicode string that 
        /// contains the name of the substorage or stream to be changed.</param>
        /// <param name="pwcsNewName">Pointer to a wide character null-terminated unicode string that 
        /// contains the new name for the specified substorage or stream.</param>
        /// <returns>S_OK - The element was successfully renamed.
        /// Otherwise error code.</returns>
        int RenameElement(string pwcsOldName, string pwcsNewName);
        /// <summary>
        /// The SetElementTimes method sets the modification, access, and creation times of the 
        /// specified storage element, if the underlying file system supports this method.
        /// </summary>
        /// <param name="pwcsName">The name of the storage object element whose times are to be modified. 
        /// If NULL, the time is set on the root storage rather than one of its elements. </param>
        /// <param name="pctime">Either the new creation time for the element or NULL 
        /// if the creation time is not to be modified.</param>
        /// <param name="patime">Either the new access time for the element or NULL if the 
        /// access time is not to be modified.</param>
        /// <param name="pmtime">Either the new modification time for the element or NULL 
        /// if the modification time is not to be modified.</param>
        /// <returns>
        /// S_OK - The time values were successfully set.
        /// Otherwise error code.
        /// </returns>
        int SetElementTimes(string pwcsName,
          ref System.Runtime.InteropServices.ComTypes.FILETIME pctime,
          ref System.Runtime.InteropServices.ComTypes.FILETIME patime,
          ref System.Runtime.InteropServices.ComTypes.FILETIME pmtime);
        /// <summary>
        /// The SetClass method assigns the specified class identifier (CLSID) to this storage object.
        /// </summary>
        /// <param name="clsid">The CLSID that is to be associated with the storage object.</param>
        /// <returns>S_OK - The CLSID was successfully assigned.
        /// Otherwise error code.
        /// </returns>
        int SetClass(ref Guid clsid);
        /// <summary>
        /// The SetStateBits method stores up to 32 bits of state information in this storage object. 
        /// This method is reserved for future use.
        /// </summary>
        /// <param name="grfStateBits">Specifies the new values of the bits to set. No legal values are 
        /// defined for these bits; they are all reserved for future use and must not be used by applications.</param>
        /// <param name="grfMask">A binary mask indicating which bits in grfStateBits are significant in this call.</param>
        /// <returns>S_OK - The state information was successfully set.
        /// Otherwise error code.
        /// </returns>
        int SetStateBits(uint grfStateBits, uint grfMask);
        /// <summary>
        /// The Stat method retrieves the STATSTG structure for this open storage object.
        /// </summary>
        /// <param name="pstatstg">On return, pointer to a STATSTG structure where this 
        /// method places information about the open storage object. This parameter is NULL if an error occurs.</param>
        /// <param name="grfStatFlag">Specifies that some of the members in the STATSTG structure 
        /// are not returned, thus saving a memory allocation operation. 
        /// Values are taken from the STATFLAG enumeration.</param>
        /// <returns>
        /// S_OK - The STATSTG structure was successfully returned at the specified location.
        /// Otherwise error code.
        /// </returns>
        int Stat(ref STATSTG pstatstg, uint grfStatFlag);
    }

    /// <summary>
    /// The IEnumSTATPROPSETSTG interface iterates through an array of STATPROPSETSTG structures.
    /// </summary>
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    [Guid("0000013b-0000-0000-c000-000000000046")]
    [CLSCompliant(false)]
    public interface IEnumSTATPROPSETSTG
    {
        /// <summary>
        /// The Next method retrieves a specified number of STATPROPSETSTG structures that follow 
        /// subsequently in the enumeration sequence.
        /// </summary>
        /// <param name="celt">The number of STATPROPSETSTG structures requested.</param>
        /// <param name="rgelt">An array of STATPROPSETSTG structures returned.</param>
        /// <param name="pceltFetched">The number of STATPROPSETSTG structures retrieved in the rgelt parameter.</param>
        void Next(uint celt, out tagSTATPROPSETSTG rgelt, out uint pceltFetched);
        /// <summary>
        /// The Skip method skips a specified number of STATPROPSETSTG structures in the enumeration sequence.
        /// </summary>
        /// <param name="celt">The number of STATPROPSETSTG structures to skip.</param>
        void Skip(uint celt);
        /// <summary>
        /// The Reset method resets the enumeration sequence to the beginning of the STATPROPSETSTG structure array.
        /// </summary>
        void Reset();
        /// <summary>
        /// The Clone method creates an enumerator that contains the same enumeration state as the current 
        /// STATPROPSETSTG structure enumerator. Using this method, a client can record a particular point 
        /// in the enumeration sequence and then return to that point later.
        /// </summary>
        /// <param name="ppenum">A pointer to the variable that receives the IEnumSTATPROPSETSTG interface pointer.</param>
        void Clone(out IEnumSTATPROPSETSTG ppenum);
    }

#if DOCIO
    /// <summary>
    /// 
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000139-0000-0000-C000-000000000046")]
    [CLSCompliant(false)]
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    public interface IEnumSTATPROPSTG
    {
        /// <summary>
        /// The Next method retrieves a specified number of STATPROPSTG structures, that follow subsequently 
        /// in the enumeration sequence. 
        /// </summary>
        /// <param name="celt">The number of STATPROPSTG structures requested.</param>
        /// <param name="rgelt">An array of STATPROPSTG structures returned.</param>
        /// <param name="pceltFetched">The number of STATPROPSTG structures retrieved in the rgelt parameter.</param>
        void Next(int celt, [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] tagSTATPROPSTG[] rgelt, out int pceltFetched);
        /// <summary>
        /// The Skip method skips the specified number of STATPROPSTG structures in the enumeration sequence.
        /// </summary>
        /// <param name="celt">The number of STATPROPSTG structures to skip.</param>
        void Skip(uint celt);
        /// <summary>
        /// The Reset method resets the enumeration sequence to the beginning of the STATPROPSTG structure array.
        /// </summary>
        void Reset();
        /// <summary>
        /// The Clone method creates an enumerator that contains the same enumeration state as the current 
        /// STATPROPSTG structure enumerator
        /// </summary>
        /// <param name="ppenum">A pointer to the variable that receives the IEnumSTATPROPSTG interface pointer.</param>
        void Clone(out IEnumSTATPROPSTG ppenum);
    }
#else
    /// <summary>
    /// The IEnumSTATPROPSTG interface iterates through an array of STATPROPSTG structures
    /// </summary>
#if !(WINRT ) 
    [Syncfusion.Documentation.DocumentationExclude()]  
#endif
    [Guid("00000139-0000-0000-c000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [CLSCompliant(false)]
    public interface IEnumSTATPROPSTG
    {
        /// <summary>
        /// The Next method retrieves a specified number of STATPROPSTG structures, that follow subsequently 
        /// in the enumeration sequence. 
        /// </summary>
        /// <param name="celt">The number of STATPROPSTG structures requested.</param>
        /// <param name="rgelt">An array of STATPROPSTG structures returned.</param>
        /// <param name="pceltFetched">The number of STATPROPSTG structures retrieved in the rgelt parameter.</param>
        void Next(int celt, ref tagSTATPROPSTG rgelt, out int pceltFetched);
        /// <summary>
        /// The Skip method skips the specified number of STATPROPSTG structures in the enumeration sequence.
        /// </summary>
        /// <param name="celt">The number of STATPROPSTG structures to skip.</param>
        void Skip(uint celt);
        /// <summary>
        /// The Reset method resets the enumeration sequence to the beginning of the STATPROPSTG structure array.
        /// </summary>
        void Reset();
        /// <summary>
        /// The Clone method creates an enumerator that contains the same enumeration state as the current 
        /// STATPROPSTG structure enumerator
        /// </summary>
        /// <param name="ppenum">A pointer to the variable that receives the IEnumSTATPROPSTG interface pointer.</param>
        void Clone(out IEnumSTATPROPSTG ppenum);
    }
#endif

    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 0x10, Pack = 1)]
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    [CLSCompliant(false)]
    public struct PROPVARIANT
    {
        // Fields
        [FieldOffset(0)]
        public short vt;
        [FieldOffset(8)]
        public IntPtr intPtr;
        [FieldOffset(8)]
        public byte byteVal;
        [FieldOffset(8)]
        public int intVal;
        [FieldOffset(8)]
        public bool boolVal;
        [FieldOffset(8)]
        public long fileTime;
        [FieldOffset(8)]
        public double doubleVal;
        [FieldOffset(8)]
        public short shortVal;
        [FieldOffset(12)]
        public IntPtr intPtr2;
    }

#if DOCIO
    /// <summary>
    /// The IPropertyStorage interface manages the persistent properties of a single property set. 
    /// Persistent properties consist of information that can be stored persistently in a 
    /// property set, such as the summary information associated with a file.
    /// </summary>
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
      Guid("00000138-0000-0000-C000-000000000046")]
    [CLSCompliant(false)]
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    public interface IPropertyStorage
    {
        /// <summary>
        /// The ReadMultiple method reads specified properties from the current property set.
        /// </summary>
        /// <param name="cpspec">The numeric count of properties to be specified in the rgpspec array.</param>
        /// <param name="rgpspec">An array of PROPSPEC structures specifies which properties are read. 
        /// Properties can be specified either by a property ID or by an optional string name.</param>
        /// <param name="rgpropvar">Caller-allocated array of a PROPVARIANT structure that, on return, 
        /// contains the values of the properties specified by the corresponding elements in the rgpspec array.</param>
        void ReadMultiple(uint cpspec, [In, MarshalAs(UnmanagedType.LPArray)] PROPSPEC[] rgpspec, [In, Out, MarshalAs(UnmanagedType.LPArray)] PROPVARIANT[] rgpropvar);
        /// <summary>
        /// The WriteMultiple method writes a specified group of properties to the current property set. 
        /// If a property with a specified name or property identifier already exists, it is replaced, 
        /// even when the old and new types for the property value are different.
        /// </summary>
        /// <param name="cpspec">The number of properties set. The value of this parameter can be set to zero;</param>
        /// <param name="rgpspec"> An array of the property IDs (PROPSPEC) to which properties are set.</param>
        /// <param name="rgpropvar">An array (of size cpspec) of PROPVARIANT structures that contain the property values to be written.</param>
        /// <param name="propidNameFirst">The minimum value for the property IDs that the method must assign if the 
        /// rgpspec parameter specifies string-named properties for which no property IDs currently exist.</param>
        void WriteMultiple(uint cpspec, [In, MarshalAs(UnmanagedType.LPArray)] PROPSPEC[] rgpspec, [In, MarshalAs(UnmanagedType.LPArray)] PROPVARIANT[] rgpropvar, int propidNameFirst);
        /// <summary>
        /// The DeleteMultiple method deletes as many of the indicated properties as exist in this property set.
        /// </summary>
        /// <param name="cpspec">The numerical count of properties to be deleted. The value of this parameter can
        /// legally be set to zero, however that defeats the purpose of the method as no properties are thereby 
        /// deleted, regardless of the value set in rgpspec. </param>
        /// <param name="rgpspec">Properties to be deleted. A mixture of property identifiers and string-named
        /// properties is permitted. There may be duplicates, and there is no requirement that properties be 
        /// specified in any order. </param>
        void DeleteMultiple(uint cpspec, ref PROPSPEC rgpspec);
        /// <summary>
        /// The ReadPropertyNames method retrieves any existing string names for the specified property IDs.
        /// </summary>
        /// <param name="cpropid">The number of elements on input of the array rgpropid. 
        /// The value of this parameter can be set to zero.</param>
        /// <param name="rgpropid">An array of property IDs for which names are to be retrieved.</param>
        /// <param name="rglpwstrName">A caller-allocated array of size cpropid of LPWSTR members. 
        /// On return, the implementation fills in this array.</param>
        void ReadPropertyNames(uint cpropid, ref uint rgpropid,
                                [MarshalAs(UnmanagedType.LPWStr)] out string rglpwstrName);
        /// <summary>
        /// The WritePropertyNames method assigns string names to a specified array of property IDs in the current property set.
        /// </summary>
        /// <param name="cpropid">The size on input of the array rgpropid. Can be zero. 
        /// However, making it zero causes this method to become non-operational.</param>
        /// <param name="rgpropid">An array of the property IDs for which names are to be set.</param>
        /// <param name="rglpwstrName">Array of new names to be assigned to the corresponding property 
        /// IDs in the rgpropid array. These names may not exceed 255 characters (not including the NULL terminator).</param>
        void WritePropertyNames(uint cpropid, ref uint rgpropid,
                                 [MarshalAs(UnmanagedType.LPWStr)] ref string rglpwstrName);
        /// <summary>
        /// The DeletePropertyNames method deletes specified string names from the current property set.
        /// </summary>
        /// <param name="cpropid">The size on input of the array rgpropid. If 0, no property names are deleted.</param>
        /// <param name="rgpropid">Property identifiers for which string names are to be deleted.</param>
        void DeletePropertyNames(uint cpropid, ref uint rgpropid);
        /// <summary>
        /// The IPropertyStorage::Commit method saves changes made to a property storage 
        /// object to the parent storage object.
        /// </summary>
        /// <param name="grfCommitFlags">The flags that specify the conditions under which the commit is to be performed.</param>
        void Commit(uint grfCommitFlags);
        /// <summary>
        /// The Revert method discards all changes to the named property set since it was last opened or 
        /// discards changes that were last committed to the property set.
        /// </summary>
        void Revert();
        /// <summary>
        /// The Enum method creates an enumerator object designed to enumerate data of type STATPROPSTG, 
        /// which contains information on the current property set.
        /// </summary>
        /// <param name="ppenum">Pointer to IEnumSTATPROPSTG pointer variable that receives the interface 
        /// pointer to the new enumerator object.</param>
        void Enum(out IEnumSTATPROPSTG ppenum);
        /// <summary>
        /// The SetTimes method sets the modification, access, and creation times of this property set, 
        /// if supported by the implementation. Not all implementations support all these time values.
        /// </summary>
        /// <param name="pctime">Pointer to the new creation time for the property set. May be NULL, 
        /// indicating that this time is not to be modified by this call.</param>
        /// <param name="patime">Pointer to the new access time for the property set. May be NULL, 
        /// indicating that this time is not to be modified by this call.</param>
        /// <param name="pmtime">Pointer to the new modification time for the property set. May be 
        /// NULL, indicating that this time is not to be modified by this call.</param>
        void SetTimes(ref System.Runtime.InteropServices.ComTypes.FILETIME pctime,
         ref System.Runtime.InteropServices.ComTypes.FILETIME patime,
         ref System.Runtime.InteropServices.ComTypes.FILETIME pmtime);
        /// <summary>
        /// The SetClass method assigns a new CLSID to the current property storage object, and 
        /// persistently stores the CLSID with the object.
        /// </summary>
        /// <param name="clsid">New CLSID to be associated with the property set.</param>
        void SetClass(ref Guid clsid);
        /// <summary>
        /// The Stat method retrieves information about the current open property set.
        /// </summary>
        /// <param name="pstatpsstg">Pointer to a STATPROPSETSTG structure, which contains 
        /// statistics about the current open property set.</param>
        void Stat(ref tagSTATPROPSETSTG pstatpsstg);
        /// <summary>
        /// The ReadMultiple method reads specified properties from the current property set.
        /// </summary>
        /// <param name="cpspec">The numeric count of properties to be specified in the rgpspec array.</param>
        /// <param name="rgpspec">An array of PROPSPEC structures specifies which properties are read. 
        /// Properties can be specified either by a property ID or by an optional string name.</param>
        /// <param name="rgpropvar">Caller-allocated array of a PROPVARIANT structure that, on return, 
        /// contains the values of the properties specified by the corresponding elements in the rgpspec array.</param>
        void ReadMultiple(uint cpspec, ref PROPSPEC rgpspec, IntPtr rgpropvar);
        /// <summary>
        /// The WriteMultiple method writes a specified group of properties to the current property set. 
        /// If a property with a specified name or property identifier already exists, it is replaced, 
        /// even when the old and new types for the property value are different.
        /// </summary>
        /// <param name="cpspec">The number of properties set. The value of this parameter can be set to zero;</param>
        /// <param name="rgpspec"> An array of the property IDs (PROPSPEC) to which properties are set.</param>
        /// <param name="rgpropvar">An array (of size cpspec) of PROPVARIANT structures that contain the property values to be written.</param>
        /// <param name="propidNameFirst">The minimum value for the property IDs that the method must assign if the 
        /// rgpspec parameter specifies string-named properties for which no property IDs currently exist.</param>
        void WriteMultiple(uint cpspec, ref PROPSPEC rgpspec, IntPtr rgpropvar, PID propidNameFirst);
        /// <summary>
        /// The IPropertyStorage::Commit method saves changes made to a property storage 
        /// object to the parent storage object.
        /// </summary>
        /// <param name="grfCommitFlags">The flags that specify the conditions under which the commit is to be performed.</param>
        void Commit(STGC grfCommitFlags);
    }
#else
    /// <summary>
    /// The IPropertyStorage interface manages the persistent properties of a single property set. 
    /// Persistent properties consist of information that can be stored persistently in a 
    /// property set, such as the summary information associated with a file.
    /// </summary>
#if !(WINRT ) 
    [Syncfusion.Documentation.DocumentationExclude()]  
#endif
    [Guid("00000138-0000-0000-c000-000000000046"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
#if !SILVERLIGHT && !WINRT && !WP
     ComConversionLoss(),
#endif
     CLSCompliant(false)]
    public interface IPropertyStorage
    {
        /// <summary>
        /// The ReadMultiple method reads specified properties from the current property set.
        /// </summary>
        /// <param name="cpspec">The numeric count of properties to be specified in the rgpspec array.</param>
        /// <param name="rgpspec">An array of PROPSPEC structures specifies which properties are read. 
        /// Properties can be specified either by a property ID or by an optional string name.</param>
        /// <param name="rgpropvar">Caller-allocated array of a PROPVARIANT structure that, on return, 
        /// contains the values of the properties specified by the corresponding elements in the rgpspec array.</param>
        void ReadMultiple(uint cpspec, ref PROPSPEC rgpspec, IntPtr rgpropvar);
        /// <summary>
        /// The WriteMultiple method writes a specified group of properties to the current property set. 
        /// If a property with a specified name or property identifier already exists, it is replaced, 
        /// even when the old and new types for the property value are different.
        /// </summary>
        /// <param name="cpspec">The number of properties set. The value of this parameter can be set to zero;</param>
        /// <param name="rgpspec"> An array of the property IDs (PROPSPEC) to which properties are set.</param>
        /// <param name="rgpropvar">An array (of size cpspec) of PROPVARIANT structures that contain the property values to be written.</param>
        /// <param name="propidNameFirst">The minimum value for the property IDs that the method must assign if the 
        /// rgpspec parameter specifies string-named properties for which no property IDs currently exist.</param>
        void WriteMultiple(uint cpspec, ref PROPSPEC rgpspec, IntPtr rgpropvar, PID propidNameFirst);
        /// <summary>
        /// The DeleteMultiple method deletes as many of the indicated properties as exist in this property set.
        /// </summary>
        /// <param name="cpspec">The numerical count of properties to be deleted. The value of this parameter can
        /// legally be set to zero, however that defeats the purpose of the method as no properties are thereby 
        /// deleted, regardless of the value set in rgpspec. </param>
        /// <param name="rgpspec">Properties to be deleted. A mixture of property identifiers and string-named
        /// properties is permitted. There may be duplicates, and there is no requirement that properties be 
        /// specified in any order. </param>
        void DeleteMultiple(uint cpspec, ref PROPSPEC rgpspec);
        /// <summary>
        /// The ReadPropertyNames method retrieves any existing string names for the specified property IDs.
        /// </summary>
        /// <param name="cpropid">The number of elements on input of the array rgpropid. 
        /// The value of this parameter can be set to zero.</param>
        /// <param name="rgpropid">An array of property IDs for which names are to be retrieved.</param>
        /// <param name="rglpwstrName">A caller-allocated array of size cpropid of LPWSTR members. 
        /// On return, the implementation fills in this array.</param>
        void ReadPropertyNames(uint cpropid, ref uint rgpropid,
          [MarshalAs(UnmanagedType.LPWStr)] out string rglpwstrName);
        /// <summary>
        /// The WritePropertyNames method assigns string names to a specified array of property IDs in the current property set.
        /// </summary>
        /// <param name="cpropid">The size on input of the array rgpropid. Can be zero. 
        /// However, making it zero causes this method to become non-operational.</param>
        /// <param name="rgpropid">An array of the property IDs for which names are to be set.</param>
        /// <param name="rglpwstrName">Array of new names to be assigned to the corresponding property 
        /// IDs in the rgpropid array. These names may not exceed 255 characters (not including the NULL terminator).</param>
        void WritePropertyNames(uint cpropid, ref uint rgpropid,
          [MarshalAs(UnmanagedType.LPWStr)] ref string rglpwstrName);
        /// <summary>
        /// The DeletePropertyNames method deletes specified string names from the current property set.
        /// </summary>
        /// <param name="cpropid">The size on input of the array rgpropid. If 0, no property names are deleted.</param>
        /// <param name="rgpropid">Property identifiers for which string names are to be deleted.</param>
        void DeletePropertyNames(uint cpropid, ref uint rgpropid);
        /// <summary>
        /// The IPropertyStorage::Commit method saves changes made to a property storage 
        /// object to the parent storage object.
        /// </summary>
        /// <param name="grfCommitFlags">The flags that specify the conditions under which the commit is to be performed.</param>
        void Commit(STGC grfCommitFlags);
        /// <summary>
        /// The Revert method discards all changes to the named property set since it was last opened or 
        /// discards changes that were last committed to the property set.
        /// </summary>
        void Revert();
        /// <summary>
        /// The Enum method creates an enumerator object designed to enumerate data of type STATPROPSTG, 
        /// which contains information on the current property set.
        /// </summary>
        /// <param name="ppenum">Pointer to IEnumSTATPROPSTG pointer variable that receives the interface 
        /// pointer to the new enumerator object.</param>
        int Enum(out IEnumSTATPROPSTG ppenum);
        /// <summary>
        /// The SetTimes method sets the modification, access, and creation times of this property set, 
        /// if supported by the implementation. Not all implementations support all these time values.
        /// </summary>
        /// <param name="pctime">Pointer to the new creation time for the property set. May be NULL, 
        /// indicating that this time is not to be modified by this call.</param>
        /// <param name="patime">Pointer to the new access time for the property set. May be NULL, 
        /// indicating that this time is not to be modified by this call.</param>
        /// <param name="pmtime">Pointer to the new modification time for the property set. May be 
        /// NULL, indicating that this time is not to be modified by this call.</param>
        void SetTimes(ref System.Runtime.InteropServices.ComTypes.FILETIME pctime,
          ref System.Runtime.InteropServices.ComTypes.FILETIME patime,
          ref System.Runtime.InteropServices.ComTypes.FILETIME pmtime);
        /// <summary>
        /// The SetClass method assigns a new CLSID to the current property storage object, and 
        /// persistently stores the CLSID with the object.
        /// </summary>
        /// <param name="clsid">New CLSID to be associated with the property set.</param>
        void SetClass(ref Guid clsid);
        /// <summary>
        /// The Stat method retrieves information about the current open property set.
        /// </summary>
        /// <param name="pstatpsstg">Pointer to a STATPROPSETSTG structure, which contains 
        /// statistics about the current open property set.</param>
        void Stat(ref tagSTATPROPSETSTG pstatpsstg);
    }
#endif
    /// <summary>
    /// The IPropertySetStorage interface creates, opens, deletes, and enumerates property set 
    /// storages that support instances of the IPropertyStorage interface.
    /// </summary>
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    [Guid("0000013a-0000-0000-c000-000000000046"),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
#if !SILVERLIGHT && !WINRT && !WP
     ComConversionLoss(),
#endif
     CLSCompliant(false)]
    public interface IPropertySetStorage
    {
        /// <summary>
        /// The Create method creates and opens a new property set in the property set storage object.
        /// </summary>
        /// <param name="rfmtid">The FMTID of the property set to be created. For information about 
        /// FMTIDs that are well-known and predefined in the Platform SDK, see Predefined Property Set 
        /// Format Identifiers. </param>
        /// <param name="pclsid"> A pointer to the initial class identifier CLSID for this property set. 
        /// May be NULL, in which case it is set to all zeroes.</param>
        /// <param name="grfFlags">The values from PROPSETFLAG Constants.</param>
        /// <param name="grfMode">An access mode in which the newly created property set is to be opened, 
        /// taken from certain values of STGM_Constants, as described in the following Remarks section.</param>
        /// <param name="ppprstg">A pointer to the output variable that receives the IPropertyStorage interface pointer.</param>
        void Create(ref Guid rfmtid, ref Guid pclsid, uint grfFlags, STGM grfMode, out IPropertyStorage ppprstg);
        /// <summary>
        /// The Open method opens a property set contained in the property set storage object.
        /// </summary>
        /// <param name="rfmtid">The format identifier (FMTID) of the property set to be opened. 
        /// For more information about well-known and predefined FMTIDs in the Platform SDK</param>
        /// <param name="grfMode">The access mode in which the newly created property set is to be 
        /// opened. These flags are taken from STGM Constants.</param>
        /// <param name="ppprstg">A pointer to the IPropertyStorage pointer variable that receives 
        /// the interface pointer to the requested property storage subobject.</param>
        [PreserveSig]
        int Open(ref Guid rfmtid, STGM grfMode, out IPropertyStorage ppprstg);
        /// <summary>
        /// The Delete method deletes one of the property sets contained in the property set storage object.
        /// </summary>
        /// <param name="rfmtid">FMTID of the property set to be deleted.</param>
        void Delete(ref Guid rfmtid);
        /// <summary>
        /// The Enum method creates an enumerator object which contains information on the 
        /// property sets stored in this property set storage.
        /// </summary>
        /// <param name="ppenum">Pointer to IEnumSTATPROPSETSTG pointer variable that 
        /// receives the interface pointer to the newly created enumerator object.</param>
        void Enum(out IEnumSTATPROPSETSTG ppenum);
    }

    /// <summary>
    /// Class provide access to STG API functions.
    /// </summary>
#if !(WINRT )
    [Syncfusion.Documentation.DocumentationExclude()]
#endif
    [CLSCompliant(false)]
    sealed public class API
    {
        #region Class constructors
        /// <summary>
        /// To prevent user from creation of this class instances.
        /// </summary>
        private API()
        {
        }
        #endregion

        /// <summary>
        /// StgOpenStorage opens an existing root storage object in the file system. You
        /// can use this function to open compound files but you cannot use it to open
        /// directories, files, or summary catalogs. Nested storage objects can only be
        /// opened using their parents' IStorage::OpenStorage method.
        /// </summary>
        /// <param name="wcsName">[in] Pointer to the path of the NULL-terminated
        /// Unicode string file containing the storage object to open. This parameter
        /// is ignored if the pstgPriority parameter is not NULL.</param>
        /// <param name="stgPriority">Most often NULL. If not NULL, this parameter is
        /// used instead of the pwcsName parameter to specify the pointer to the
        /// IStorage interface on the storage object to open. It points to a previous
        /// opening of a root storage object, most often one that was opened in priority
        /// mode. After the StgOpenStorage function returns, the storage object specified
        /// in the pstgPriority parameter on function entry is not valid and can no
        /// longer be used. Instead, use the storage object specified in the ppStgOpen
        /// parameter.</param>
        /// <param name="grfMode">Specifies the access mode to use to open the
        /// storage object.</param>
        /// <param name="snbExclude">If not NULL, pointer to a block of elements in
        /// the storage that are to be excluded as the storage object is opened. The
        /// exclusion occurs regardless of whether a snapshot copy happens on the open.
        /// May be NULL.</param>
        /// <param name="reserved">Indicates reserved for future use; must be zero.</param>
        /// <param name="storage">[out] Pointer IStorage* pointer variable that receives
        /// the interface pointer to the opened storage.</param>
        /// <returns>
        /// S_OK
        /// Indicates that the storage object was successfully opened.
        /// STG_E_FILENOTFOUND
        /// Indicates that the specified file does not exist.
        /// STG_E_ACCESSDENIED
        /// Access denied because the caller does not have enough permissions, or another caller
        /// has the file open and locked.
        /// STG_E_LOCKVIOLATION
        /// Access denied because another caller has the file open and locked.
        /// STG_E_SHAREVIOLATION
        /// Access denied because another caller has the file open and locked.
        /// STG_E_FILEALREADYEXISTS
        /// Indicates that the file exists but is not a storage object.
        /// STG_E_TOOMANYOPENFILES
        /// Indicates that the storage object was not opened because there are too many open files.
        /// STG_E_INSUFFICIENTMEMORY
        /// Indicates that the storage object was not opened due to inadequate memory.
        /// STG_E_INVALIDNAME
        /// Indicates a non-valid name in the pwcsName parameter.
        /// STG_E_INVALIDPOINTER
        /// Indicates a non-valid pointer in one of the parameters: snbExclude, pwcsName,
        /// pstgPriority, or ppStgOpen.
        /// STG_E_INVALIDFLAG
        /// Indicates a non-valid flag combination in the grfMode parameter.
        /// STG_E_INVALIDFUNCTION
        /// Indicates STGM_DELETEONRELEASE specified in the grfMode parameter.
        /// STG_E_OLDFORMAT
        /// Indicates that the storage object being opened was created by the Beta 1 storage
        /// provider. This format is no longer supported.
        /// STG_E_NOTSIMPLEFORMAT
        /// Indicates that the STGM_SIMPLE flag was specified in the grfMode parameter and the
        /// storage object being opened was not written in simple mode.
        /// STG_E_OLDDLL
        /// The DLL being used to open this storage object is a version of the DLL that is older
        /// than the one used to create it.
        /// STG_E_PATHNOTFOUND
        /// Specified path does not exist.
        /// STG_E_SHAREVIOLATION
        /// Access denied because another caller has the file open and locked.
        /// </returns>
        [DllImport("ole32.dll", SetLastError = true)]
        public static extern int StgOpenStorage(
          [MarshalAs(UnmanagedType.LPWStr)] string wcsName,
         IntPtr stgPriority,
         STGM grfMode,
         IntPtr snbExclude,
         uint reserved,
         out IStorage storage);

        /// <summary>
        /// Opens an existing root storage object in the file system. You can use this function
        /// to open compound files and regular files. To create a new file, use the
        /// StgCreateStorageEx function.
        /// </summary>
        /// <param name="pwcsName">[in] Pointer to the path of the NULL-terminated Unicode
        /// string file containing the storage object. This string size must not exceed
        /// MAX_PATH characters.</param>
        /// <param name="grfMode">[in] Specifies the access mode to open the new storage object.
        /// For more information, see the STGM enumeration. If the caller specifies transacted
        /// mode together with STGM_CREATE or STGM_CONVERT, the overwrite or conversion takes
        /// place when the commit operation is called for the root storage. If IStorage::Commit
        /// is not called for the root storage object, previous contents of the file will be
        /// restored. STGM_CREATE and STGM_CONVERT cannot be combined with the STGM_NOSNAPSHOT
        /// flag, because a snapshot copy is required when a file is overwritten or converted
        /// in the transacted mode. </param>
        /// <param name="stgfmt">[in] Specifies the storage file format. For more information,
        /// see the STGFMT enumeration.</param>
        /// <param name="grfAttrs">[in] Depends on the value of the stgfmt parameter.
        /// STGFMT_DOCFILE should be zero (0) or FILE_FLAG_NO_BUFFERING.</param>
        /// <param name="stgOptions">[in, out] Pointer to a STGOPTIONS structure that contains
        /// information about the storage object being opened. The pStgOptions parameter is
        /// valid only if the stgfmt parameter is set to STGFMT_DOCFILE.</param>
        /// <param name="reserved2">[in] Reserved for future use; must be zero.</param>
        /// <param name="riid">[in] Specifies the Guid of the interface pointer to return.</param>
        /// <param name="ppObjectOpen">[out] Address of an interface pointer variable that
        /// receives a pointer for an interface on the storage object being opened; contains
        /// NULL if operation failed.</param>
        /// <returns>
        /// S_OK
        /// Indicates that the storage object was successfully opened.
        /// STG_E_INVALIDPOINTER
        /// Indicates a non-valid pointer in the ppObjectOpen parameter.
        /// STG_E_INVALIDPARAMETER
        /// Indicates a non-valid value for the grfAttrs, reserved1, reserved2, grfMode, or
        /// stgfmt parameters. Can occur if the FILE_FLAG_NO_BUFFERING flag is specified for
        /// grfAttrs but the sector size of the file is not an integer multiple of the
        /// underlying disk's sector size.
        /// E_NOINTERFACE
        /// Indicates that the specified interface is not supported.
        /// STG_E_INVALIDFLAG
        /// Indicates a non-valid flag combination in the grfMode pointer (includes both
        /// STGM_DELETEONRELEASE and STGM_CONVERT flags).
        /// STG_E_INVALIDNAME
        /// Indicates a non-valid name in the pwcsName parameter.
        /// STG_E_INVALIDFUNCTION
        /// Indicates that the grfMode is set to STGM_DELETEONRELEASE.
        /// STG_E_LOCKVIOLATION
        /// Access denied because another caller has the file open and locked.
        /// STG_E_SHAREVIOLATION
        /// Access denied because another caller has the file open and locked.
        /// STG_E_UNIMPLEMENTEDFUNCTION
        /// Indicates that the StgOpenStorageEx function is not implemented by the operating
        /// system. In this case, use the StgOpenStorage function instead.
        /// STG_E_INCOMPLETE
        /// Indicates that the file could not be opened because it is on a high-latency device.
        /// This can only occur if the parameter is IID_IPropertySetStorage, and the
        /// stgfmt parameter is STGFMT_FILE.
        /// STG_E_ACCESSDENIED
        /// Indicates that the file could not be opened because the underlying storage device
        /// does not allow such access to the current user. When opening the storage object
        /// in transacted mode (STGM_TRANSACTED), this error may also indicate that a temporary
        /// file could not be created in the temporary directory as specified by the
        /// GetTempPath function. The GetTempPath function retrieves the path of the directory
        /// designated for temporary files.
        /// </returns>
        [DllImport("ole32.dll", SetLastError = true)]
        public static extern int StgOpenStorageEx(
          [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
         STGM grfMode,
         STGFMT stgfmt,
         uint grfAttrs,
         IntPtr stgOptions,
            //ref STGOPTIONS pStgOptions,
         IntPtr reserved2,
         ref Guid riid,
         out IStorage ppObjectOpen);

        /// <summary>
        /// StgCreateDocfile creates a new compound file storage object using the COM-provided
        /// compound file implementation for the IStorage interface.
        /// </summary>
        /// <param name="pwcsName">[in] Pointer to a NULL-terminated Unicode string name for the
        /// compound file being created. It is passed uninterpreted to the file system. This can
        /// be a relative name or NULL. If NULL, a temporary compound file is allocated with a
        /// unique name.</param>
        /// <param name="grfMode">[in] Specifies the access mode to use when opening the new
        /// storage object. For more information, see the STGM enumeration. If the caller
        /// specifies transacted mode together with STGM_CREATE or STGM_CONVERT, the overwrite
        /// or conversion takes place when the commit operation is called for the root storage.
        /// If IStorage::Commit is not called for the root storage object, previous contents of
        /// the file will be restored. STGM_CREATE and STGM_CONVERT cannot be combined with
        /// the STGM_NOSNAPSHOT flag, because a snapshot copy is required when a file is
        /// overwritten or converted in the transacted mode.</param>
        /// <param name="reserved">[in] Reserved for future use; must be zero.</param>
        /// <param name="ppstgOpen">[out] Pointer to the location of the IStorage pointer to
        /// the new storage object.</param>
        /// <returns>
        /// S_OK - Indicates that the compound file was successfully created.
        /// STG_E_ACCESSDENIED - Access denied because the caller does not have enough permissions
        /// or another caller has the file open and locked.
        /// STG_E_FILEALREADYEXISTS
        /// Indicates that the compound file already exists and grfMode is set to STGM_FAILIFTHERE.
        /// STG_E_INSUFFICIENTMEMORY
        /// Indicates that the compound file was not created due to inadequate memory.
        /// STG_E_INVALIDFLAG
        /// Indicates a non-valid flag combination in the grfMode parameter.
        /// STG_E_INVALIDNAME
        /// Indicates a non-valid name in the pwcsName parameter.
        /// STG_E_INVALIDPOINTER
        /// Indicates a non-valid pointer in the pwcsName parameter or the ppStgOpen parameter.
        /// STG_E_LOCKVIOLATION
        /// Access denied because another caller has the file open and locked.
        /// STG_E_SHAREVIOLATION
        /// Access denied because another caller has the file open and locked.
        /// STG_E_TOOMANYOPENFILES
        /// Indicates that the compound file was not created due to a lack of file handles.
        /// STG_S_CONVERTED
        /// Indicates that the specified file was successfully converted to storage format.
        /// </returns>
        [DllImport("ole32.dll", SetLastError = true)]
        public static extern int StgCreateDocfile(
          [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
         STGM grfMode,
         uint reserved,
         out IStorage ppstgOpen);

        [DllImport("ole32.dll", SetLastError = true, EntryPoint = "StgCreatePropSetStg")]
        private static extern int _StgCreatePropSetStg64(
          IStorage pStorage,
          uint dwReserved,
          out IPropertySetStorage ppPropSetStg);
#if !(WINRT )
        [DllImport("iprop.dll", SetLastError = true, EntryPoint = "StgCreatePropSetStg")]
        private static extern int _StgCreatePropSetStg32(
          IStorage pStorage,
          uint dwReserved,
          out IPropertySetStorage ppPropSetStg);

        /// <summary>
        /// The StgCreatePropSetStg function creates a property set storage object from a specified storage object.
        /// </summary>
        /// <param name="pStorage">Pointer to the storage object that contains or is to 
        /// contain one or more property sets.</param>
        /// <param name="dwReserved">Reserved for future use; must be zero.</param>
        /// <param name="ppPropSetStg">Pointer to IPropertySetStorage* pointer variable that receives the 
        /// interface pointer to the property-set storage object. </param>
        /// <returns>S_OK - The property set storage object was successfully created.</returns>
        public static int StgCreatePropSetStg(IStorage pStorage,
          uint dwReserved,
          out IPropertySetStorage ppPropSetStg)
        {
            if (IntPtr.Size == 8) // 64 bit machine
                return _StgCreatePropSetStg64(pStorage, dwReserved, out ppPropSetStg);
            else // default assume 32 bit
                return _StgCreatePropSetStg32(pStorage, dwReserved, out ppPropSetStg);
        }

        /// <summary>
        /// The StgCreatePropSetStg function creates a property set storage object from a specified storage object.
        /// </summary>
        /// <param name="pStorage">Pointer to the storage object that contains or is to 
        /// contain one or more property sets.</param>
        /// <param name="dwReserved">Reserved for future use; must be zero.</param>
        /// <param name="ppPropSetStg">Pointer to IPropertySetStorage* pointer variable that receives the 
        /// interface pointer to the property-set storage object. </param>
        /// <returns>S_OK - The property set storage object was successfully created.</returns>
        //[DllImport("ole32.dll", SetLastError = true, EntryPoint = "StgCreatePropSetStg")]
        public static /*extern*/ int StgCreatePropSetStgOle(
          IStorage pStorage,
          uint dwReserved,
          out IPropertySetStorage ppPropSetStg)
        {
            if (IntPtr.Size == 8) // 64 bit machine
                return _StgCreatePropSetStg64(pStorage, dwReserved, out ppPropSetStg);
            else // default assume 32 bit
                return _StgCreatePropSetStg32(pStorage, dwReserved, out ppPropSetStg);
        }
#endif
        /// <summary>
        /// The CreateILockBytesOnHGlobal function creates a byte array object, using global memory 
        /// as the physical device, which is intended to be the compound file foundation.
        /// </summary>
        /// <param name="hGlobal">The memory handle allocated by the GlobalAlloc function.</param>
        /// <param name="fDeleteOnRelease">A flag that specifies whether the underlying handle for 
        /// this byte array object should be automatically freed when the object is released.</param>
        /// <param name="ppLkbyt">The address of ILockBytes pointer variable that receives the interface 
        /// pointer to the new byte array object.</param>
        /// <returns>S_OK - The byte array object was created successfully.</returns>
        [DllImport("ole32.dll", SetLastError = true)]
        public static extern int CreateILockBytesOnHGlobal(
          IntPtr hGlobal,
          bool fDeleteOnRelease,
          out ILockBytes ppLkbyt);

        /// <summary>
        /// The StgCreateDocfileOnILockBytes function creates and opens a new compound file 
        /// storage object on top of a byte-array object provided by the caller.
        /// </summary>
        /// <param name="plkbyt">A pointer to the ILockBytes interface on the underlying 
        /// byte-array object on which to create a compound file.</param>
        /// <param name="grfMode">Specifies the access mode to use when opening the new compound file. 
        /// For more information, see STGM Constants.</param>
        /// <param name="reserved">Reserved for future use; must be zero.</param>
        /// <param name="ppstgOpen">A pointer to the location of the IStorage pointer on the new storage object.</param>
        /// <returns>
        /// S_OK - Indicates that the compound file was successfully created.
        /// Otherwise error code.
        /// </returns>
        [DllImport("ole32.dll", SetLastError = true)]
        public static extern int StgCreateDocfileOnILockBytes(
          ILockBytes plkbyt,
          STGM grfMode,
          int reserved,
          out IStorage ppstgOpen);

        /// <summary>
        /// The StgOpenStorageOnILockBytes function opens an existing storage object that does not reside in a 
        /// disk file, but instead has an underlying byte array provided by the caller.
        /// </summary>
        /// <param name="plkbyt"> ILockBytes pointer to the underlying byte array object that contains the 
        /// storage object to be opened.</param>
        /// <param name="pStgPriority"> Most often NULL. If not NULL, this parameter is used instead of the 
        /// plkbyt parameter to specify the storage object to open. In this case, it points to the IStorage 
        /// interface on a previously opened root storage object, most often one that was opened in priority mode.</param>
        /// <param name="grfMode">Specifies the access mode to use to open the storage object.</param>
        /// <param name="snbExclude">Can be NULL. If not NULL, this parameter points to a block of elements in this 
        /// storage that are to be excluded as the storage object is opened. This exclusion occurs independently of 
        /// whether a snapshot copy happens on the open.</param>
        /// <param name="reserved">Indicates reserved for future use; must be zero.</param>
        /// <param name="ppstgOpen">Points to the location of an IStorage pointer to the opened storage on successful return.</param>
        /// <returns>S_OK - The storage object was successfully opened.
        /// Otherwise error code.
        /// </returns>
        [DllImport("ole32.dll", SetLastError = true)]
        public static extern int StgOpenStorageOnILockBytes(
          ILockBytes plkbyt,
          IStorage pStgPriority,
          STGM grfMode,
          int snbExclude,
          int reserved,
          out IStorage ppstgOpen);
#if!(WINRT )
        /// <summary>
        /// The GlobalAlloc function allocates the specified number of bytes from the heap.
        /// Windows memory management does not provide a separate local heap and global heap.
        /// </summary>
        /// <param name="flags">Memory allocation attributes.</param>
        /// <param name="size">Number of bytes to allocate.</param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the newly
        /// allocated memory object. If the function fails, the return value is NULL.
        /// To get extended error information, call GetLastError.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GlobalAlloc(GlobalAllocFlags flags, int size);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hMem"></param>
        /// <param name="bytes"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GlobalReAlloc(IntPtr hMem, int bytes, GlobalAllocFlags flags);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hMem"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GlobalFree(IntPtr hMem);
#endif
        [DllImport("ole32.dll")]
        internal static extern int StgCreateStorageEx([MarshalAs(UnmanagedType.LPWStr)] string wcsName,
          STGM grfMode, STGFMT stgfmt, int grfAttrs, IntPtr pStgOptions,
          IntPtr reserved2, [In] ref Guid riid, out IStorage storage);

        /// <summary>
        /// Flags for GlobalAlloc function.
        /// </summary>
        [Flags]
        public enum GlobalAllocFlags
        {
            /// <summary>
            /// Allocates fixed memory. The return value is a pointer.
            /// </summary>
            GMEM_FIXED = 0x0000,
            /// <summary>
            /// Allocates movable memory. Memory blocks are never moved in physical memory, 
            /// but they can be moved within the default heap.
            /// </summary>
            GMEM_MOVEABLE = 0x0002,
            /// <summary>
            /// Initializes memory contents to zero.
            /// </summary>
            GMEM_ZEROINIT = 0x0040,
            /// <summary>
            /// NO Discard memory.
            /// </summary>
            GMEM_NODISCARD = 0x0020,
        }
    }
}
