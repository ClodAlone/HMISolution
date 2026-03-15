#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Runtime.InteropServices;
#endregion

#if DOCIO
namespace Syncfusion.CompoundFile.DocIO.Native
#else
namespace Syncfusion.CompoundFile.XlsIO.Native
#endif
{
    /// <summary>
    /// Summary description for ILockBytes.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
   Guid("0000000a-0000-0000-C000-000000000046")]
    [CLSCompliant(false)]
    public interface ILockBytes
    {
        /// <summary>
        /// The ReadAt method reads a specified number of bytes 
        /// starting at a specified offset from the beginning of the byte array object.
        /// </summary>
        /// <param name="ulOffset">Specifies the starting point from the beginning 
        /// of the byte array for reading data.</param>
        /// <param name="pv">Pointer to the buffer into which the byte array is read. 
        /// The size of this buffer is contained in cb.</param>
        /// <param name="cb">Specifies the number of bytes of data to attempt to read
        ///  from the byte array. </param>
        /// <param name="pcbRead">Pointer to a ULONG where this method writes the actual
        ///  number of bytes read from the byte array. You can set this pointer to NULL
        ///  to indicate that you are not interested in this value. In this case, this
        ///  method does not provide the actual number of bytes that were read.</param>
        /// <returns>S_OK - Indicates that the specified number of bytes were read, or the
        ///  maximum number of bytes were read to the end of the byte array.
        ///  E_FAIL - Data could not be read from the byte array. 
        ///  E_PENDING - Asynchronous Storage only: Part or all of the data to be
        ///  read is currently unavailable.
        ///  STG_E_ACCESSDENIED - The caller does not have permission to access the byte array.
        ///  STG_E_READFAULT - The number of bytes to be read does not equal the number of bytes
        ///   that were actually read</returns>
        int ReadAt(
          ulong ulOffset,
          [MarshalAs(UnmanagedType.LPArray)] byte[] pv,
         uint cb,
         out uint pcbRead);

        /// <summary>
        /// The WriteAt method writes the specified number of bytes starting at a specified offset
        ///  from the beginning of the byte array.
        /// </summary>
        /// <param name="ulOffset">Specifies the starting point from 
        /// the beginning of the byte array for the data to be written.</param>
        /// <param name="pv">Pointer to the buffer containing the data to be written.</param>
        /// <param name="cb">Specifies the number of bytes of data to attempt to write into 
        /// the byte array. </param>
        /// <param name="pcbWritten">Pointer to a location where this method specifies the 
        /// actual number of bytes written to the byte array. You can set this pointer to 
        /// NULL to indicate that you are not interested in this value. In this case, this
        ///  method does not provide the actual number of bytes written.</param>
        /// <returns>S_OK - Indicates that the specified number of bytes were written.
        /// E_FAIL - A general failure occurred during the write operation.
        /// E_PENDING - Asynchronous Storage only: Part or all of the data to be 
        /// written is currently unavailable.
        /// STG_E_ACCESSDENIED - The caller does not have enough permissions for writing 
        /// this byte array.
        /// STG_E_WRITEFAULT - The number of bytes to be written does not equal the number
        ///  of bytes that were actually written.
        ///  STG_E_MEDIUMFULL - The write operation was not completed because there is no 
        ///  space left on the storage device. The actual number of bytes written is still
        ///   returned in pcbWritten. </returns>
        int WriteAt(
          ulong ulOffset,
          [MarshalAs(UnmanagedType.LPArray)] byte[] pv,
         uint cb,
         out uint pcbWritten);

        /// <summary>
        /// The Flush method ensures that any internal buffers maintained by the ILockBytes
        ///  implementation are written out to the underlying physical storage.
        /// </summary>
        /// <returns>S_OK - The flush operation was successful.
        /// STG_E_ACCESSDENIED - The caller does not have permission to access the byte array. 
        /// STG_E_MEDIUMFULL - The flush operation is not completed because there is no space
        /// left on the storage device.
        /// E_FAIL - General failure writing data.
        /// STG_E_TOOMANYFILESOPEN - Under certain circumstances, the Flush method executes 
        /// a download-and-closeto flush, which can lead to a return value of 
        /// STG_E_TOOMANYFILESOPEN if no file handles are available. 
        /// STG_E_INVALIDHANDLE - An underlying file has been prematurely closed, or the 
        /// correct floppy disk has been replaced by an invalid one.</returns>
        int Flush();

        /// <summary>
        /// The SetSize method changes the size of the byte array.
        /// </summary>
        /// <param name="cb">Specifies the new size of the byte array as a number of bytes.</param>
        /// <returns>S_OK - The size of the byte array was successfully changed.
        /// STG_E_ACCESSDENIED - The caller does not have permission to access the byte array.
        /// STG_E_MEDIUMFULL - The byte array size is not changed because there is no
        ///  space left on the storage device.</returns>
        int SetSize(ulong cb);

        /// <summary>
        /// The LockRegion method restricts access to a specified range of bytes in the byte array.
        /// </summary>
        /// <param name="libOffset">Specifies the byte offset for the beginning of the range.</param>
        /// <param name="cb">Specifies, in bytes, the length of the range to be restricted.</param>
        /// <param name="dwLockType">Specifies the type of restrictions being requested on 
        /// accessing the range. This parameter uses one of the values from the LOCKTYPE enumeration.</param>
        /// <returns>S_OK - The specified range of bytes was locked
        /// STG_E_INVALIDFUNCTION - Locking is not supported at all or the specific type of lock 
        /// requested is not supported.
        /// STG_E_ACCESSDENIED - Access denied because the caller has insufficient permission, 
        /// or another caller has the file open and locked.
        /// STG_E_LOCKVIOLATION - Access denied because another caller has the file open and locked.
        /// STG_E_INVALIDHANDLE - An underlying file has been prematurely closed, or the
        /// correct floppy disk has been replaced by an invalid one</returns>
        int LockRegion(
          ulong libOffset,
          ulong cb,
          uint dwLockType);

        /// <summary>
        /// The UnlockRegion method removes the access restriction on a previously 
        /// locked range of bytes.
        /// </summary>
        /// <param name="libOffset">Specifies the byte offset for the beginning of the range. </param>
        /// <param name="cb">Specifies, in bytes, the length of the range that is restricted.</param>
        /// <param name="dwLockType">Specifies the type of access restrictions previously 
        /// placed on the range. This parameter uses a value from the LOCKTYPE enumeration. </param>
        /// <returns>S_OK - The byte range was unlocked.
        /// STG_E_INVALIDFUNCTION  - Locking is not supported at all or the specific type
        /// of lock requested is not supported.
        /// STG_E_LOCKVIOLATION The requested unlock cannot be granted.
        ///  </returns>
        int UnlockRegion(
          ulong libOffset,
          ulong cb,
          uint dwLockType);

        /// <summary>
        /// The Stat method retrieves a STATSTG structure containing information for
        ///  this byte array object.
        /// </summary>
        /// <param name="pstatstg">Pointer to a STATSTG structure in which this method
        ///  places information about this byte array object. The pointer is NULL if 
        ///  an error occurs.</param>
        /// <param name="grfStatFlag">Specifies whether this method should supply the
        ///  pwcsName member of the STATSTG structure through values taken from the
        ///  STATFLAG enumeration. If the STATFLAG_NONAME is specified, the pwcsName 
        ///  member of STATSTG is not supplied, thus saving a memory-allocation operation. 
        ///  The other possible value, STATFLAG_DEFAULT, indicates that all members of the 
        ///  STATSTG structure be supplied.</param>
        /// <returns>S_OK - The STATSTG structure was successfully returned at 
        /// the specified location.
        /// E_OUTOFMEMORY - The STATSTG structure was not returned due to a lack of memory 
        /// for the name member in the structure. 
        /// STG_E_ACCESSDENIED - The STATSTG structure was not returned because the caller
        /// did not have access to the byte array.
        /// STG_E_INSUFFICIENTMEMORY - The STATSTG structure was not returned, due to 
        /// insufficient memory. 
        /// STG_E_INVALIDFLAG - The value for the grfStateFlag parameter is not valid.
        /// STG_E_INVALIDPOINTER - The value for the pStatStg parameter is not valid. 
        /// </returns>
        int Stat(out STATSTG pstatstg, uint grfStatFlag);
    }
}
