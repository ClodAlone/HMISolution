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
using System.Runtime.InteropServices;
#endregion

namespace Syncfusion.DocIO.ReaderWriter
{
    /// <summary>
    /// Class that encapsulates pointer to unmanaged memory block
    /// and used to convert managed memory block (byte array)
    /// into managed object.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class MemoryConverter
    {
        #region Class constants
        /// <summary>
        /// Default memory block size.
        /// </summary>
        private const int DEF_MEMORY_BLOCK_SIZE = 8228;

        /// <summary>
        /// Minimum memory block size.
        /// </summary>
        private const int DEF_MIN_BLOCK_SIZE = 1024;

        /// <summary>
        /// Maximum memory block size.
        /// </summary>
        private const int DEF_MAX_BLOCK_SIZE = int.MaxValue;

        /// <summary>
        /// Exception message for OutOfMemoryException.
        /// </summary>
        private const string DEF_OUT_OF_MEMORY_MSG = "Application was unable to allocate memory block";
        #endregion

        #region Class members
        /// <summary>
        /// Pointer to the memory block.
        /// </summary>
        private IntPtr m_memoryBlock;

        /// <summary>
        /// Size of the memory block.
        /// </summary>
        private int m_iMemoryBlockSize;

        /// <summary>
        /// Instance of memory converter.
        /// </summary>
        [ThreadStatic]
        private static MemoryConverter m_instance;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryConverter"/> class.
        /// </summary>
        internal MemoryConverter()
            : this(DEF_MEMORY_BLOCK_SIZE)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryConverter"/> class.
        /// </summary>
        /// <param name="iMemoryBlockSize">Size of the i memory block.</param>
        internal MemoryConverter(int iMemoryBlockSize)
        {
            if (iMemoryBlockSize < DEF_MIN_BLOCK_SIZE)
            {
                iMemoryBlockSize = DEF_MIN_BLOCK_SIZE;
            }

            if (iMemoryBlockSize > DEF_MAX_BLOCK_SIZE)
            {
                throw new ArgumentOutOfRangeException("iMemoryBlock",
                                                       "Value can not be greater " + DEF_MAX_BLOCK_SIZE.ToString());
            }

            m_memoryBlock = Marshal.AllocCoTaskMem(iMemoryBlockSize);
            m_iMemoryBlockSize = iMemoryBlockSize;

            if (m_memoryBlock.ToInt64() == 0)
            {
                throw new OutOfMemoryException(DEF_OUT_OF_MEMORY_MSG);
            }
        }
        #endregion

        #region Class internal Methods
        /// <summary>
        /// Ensures that memory block will be able to accept iDesiredSize bytes.
        /// </summary>
        /// <param name="iDesiredSize">Bytes that memory block should be able to accept.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// If iDesiredSize is larger than maximum possible memory block size.
        /// </exception>
        /// <exception cref="System.OutOfMemoryException">
        /// When was unable to allocate desired memory block size.
        /// </exception>
        internal void EnsureMemoryBlockSize(int iDesiredSize)
        {
            if (iDesiredSize <= m_iMemoryBlockSize)
            {
                return;
            }

            if (iDesiredSize > DEF_MAX_BLOCK_SIZE)
            {
                throw new ArgumentOutOfRangeException("iDesiredSize", iDesiredSize,
                                                       "Value can not be greater than " + DEF_MAX_BLOCK_SIZE);
            }

            m_memoryBlock = Marshal.ReAllocCoTaskMem(m_memoryBlock, iDesiredSize);
            m_iMemoryBlockSize = iDesiredSize;

            if (m_memoryBlock.ToInt64() == 0)
            {
                throw new OutOfMemoryException(DEF_OUT_OF_MEMORY_MSG);
            }
        }

        /// <summary>
        /// Copies data from arrData into internal memory block.
        /// </summary>
        /// <param name="arrData">Data to copy.</param>
        internal void CopyFrom(byte[] arrData)
        {
            if (arrData == null)
            {
                throw new ArgumentNullException("arrData");
            }

            int iCount = arrData.Length;

            if (iCount == 0)
            {
                return;
            }

            EnsureMemoryBlockSize(iCount);
            Marshal.Copy(arrData, 0, m_memoryBlock, iCount);
        }

        /// <summary>
        /// Copies data from arrData into internal memory block starting
        /// from specified index to the end of the array.
        /// </summary>
        /// <param name="arrData">Data to copy.</param>
        /// <param name="iStartIndex">Start index of the data to copy.</param>
        internal void CopyFrom(byte[] arrData, int iStartIndex)
        {
            if (arrData == null)
            {
                throw new ArgumentNullException("arrData");
            }

            if (iStartIndex < 0)
            {
                throw new ArgumentOutOfRangeException("iStartIndex");
            }

            int iCount = arrData.Length - iStartIndex;

            if (iCount == 0)
            {
                return;
            }

            EnsureMemoryBlockSize(iCount);
            Marshal.Copy(arrData, iStartIndex, m_memoryBlock, iCount);
        }

        /// <summary>
        /// Copies specified number of bytes from arrData into internal memory block starting
        /// from specified index to the end of the array.
        /// </summary>
        /// <param name="arrData">Data to copy.</param>
        /// <param name="iStartIndex">Start index of the data to copy.</param>
        /// <param name="iCount">Number of bytes to copy.</param>
        internal void CopyFrom(byte[] arrData, int iStartIndex, int iCount)
        {
            if (arrData == null)
            {
                throw new ArgumentNullException("arrData");
            }

            if (iStartIndex < 0)
            {
                throw new ArgumentOutOfRangeException("iStartIndex");
            }

            if (iCount <= 0)
            {
                return;
            }

            int iAvailableCount = arrData.Length - iStartIndex;

            if (iAvailableCount < iCount)
            {
                throw new ArgumentOutOfRangeException("iCount is too large");
            }

            EnsureMemoryBlockSize(iCount);
            Marshal.Copy(arrData, iStartIndex, m_memoryBlock, iCount);
        }

        /// <summary>
        /// Copies data from internal memory block into object.
        /// </summary>
        /// <param name="destination">Destination object.</param>
        internal void CopyTo(object destination)
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            Marshal.PtrToStructure(m_memoryBlock, destination);
        }

        /// <summary>
        /// Copies data from byte array into specified object.
        /// </summary>
        /// <param name="arrData">Data to copy.</param>
        /// <param name="destination">Destination object.</param>
        internal void Copy(byte[] arrData, object destination)
        {
            // NOTE: maybe later for better perfomance here should be not method calls but 
            // code copied from those methods.
            CopyFrom(arrData);
            CopyTo(destination);
        }

        /// <summary>
        /// Copies data from byte array into specified object.
        /// </summary>
        /// <param name="arrData">Data to copy.</param>
        /// <param name="iStartIndex">Start index of the data to copy.</param>
        /// <param name="destination">Destination object.</param>
        internal void Copy(byte[] arrData, int iStartIndex, object destination)
        {
            CopyFrom(arrData, iStartIndex);
            CopyTo(destination);
        }

        /// <summary>
        /// Copies data from byte array into specified object.
        /// </summary>
        /// <param name="arrData">Data to copy.</param>
        /// <param name="iStartIndex">Start index of the data to copy.</param>
        /// <param name="iCount">Number of bytes to copy.</param>
        /// <param name="destination">Destination object.</param>
        internal void Copy(byte[] arrData, int iStartIndex, int iCount, object destination)
        {
            CopyFrom(arrData, iStartIndex, iCount);
            CopyTo(destination);
        }

        /// <summary>
        /// Copies data from source object into array of bytes.
        /// </summary>
        /// <param name="source">Source object to copy.</param>
        /// <param name="arrDestination">Destination array.</param>
        /// <param name="iStartIndex">Start index in the destination array.</param>
        /// <param name="iLength">Length of the data to copy.</param>
        internal void Copy(object source, byte[] arrDestination, int iStartIndex, int iLength)
        {
            if (arrDestination == null)
                throw new ArgumentNullException("arrDestination");

            if (iStartIndex < 0 || iStartIndex >= arrDestination.Length)
                throw new ArgumentOutOfRangeException("iStartIndex");

            if (iStartIndex + iLength > arrDestination.Length)
                throw new ArgumentOutOfRangeException("iLength");

            if (source == null)
                throw new ArgumentNullException("UnderlyingStructure");

            EnsureMemoryBlockSize(iLength);
            Marshal.StructureToPtr(source, m_memoryBlock, false);
            Marshal.Copy(m_memoryBlock, arrDestination, iStartIndex, iLength);
            Marshal.DestroyStructure(m_memoryBlock, source.GetType());
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        internal static MemoryConverter Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new MemoryConverter();
                }

                return m_instance;
            }
        }
        #endregion
    }
}