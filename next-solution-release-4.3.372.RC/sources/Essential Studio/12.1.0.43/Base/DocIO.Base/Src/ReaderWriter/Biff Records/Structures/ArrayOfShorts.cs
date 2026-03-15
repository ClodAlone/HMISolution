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

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures
{
    /// <summary>
    /// Summary description for ArrayOfShorts.
    /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
    [StructLayout(LayoutKind.Sequential)]
#endif
    [CLSCompliant(false)]
    internal class ArrayOfShorts
    {
        #region Class constants
        private const int DEF_KNOWN_COUNT = 14;
        private const int DEF_OFFSET_MAGIC_CREATED = 0;//0;
        private const int DEF_OFFSET_MAGIC_REVISED = 1;//2;
        private const int DEF_OFFSET_MAGIC_CREATED_PRIVATE = 2;//4;
        private const int DEF_OFFSET_MAGIC_REVISED_PRIVATE = 3;//6;
        private const int DEF_OFFSET_FBPCHPFIRST_W6 = 4;//8;
        private const int DEF_OFFSET_CHPFIRST_W6 = 5;//10;
        private const int DEF_OFFSET_BTECHP_W6 = 6;//12;
        private const int DEF_OFFSET_FBPPAPFIRST_W6 = 7;//14;
        private const int DEF_OFFSET_PAPFIRST_W6 = 8;//16;
        private const int DEF_OFFSET_BTEPAP_W6 = 9;//18;
        private const int DEF_OFFSET_FBPLVCFIRST_W6 = 10;//20;
        private const int DEF_OFFSET_LVCFIRST_W6 = 11;//22;
        private const int DEF_OFFSET_BTELVC_W6 = 12;//24;
        private const int DEF_OFFSET_LIDFE = 13;//26;
        #endregion

        #region Class members
        /// <summary>
        /// Array of shorts.
        /// </summary>
        private short[] m_arrShorts = new short[0];
        #endregion

        #region Class Properties
        /// <summary>
        /// Length of the array.
        /// </summary>
        internal int Length
        {
            get
            {
                return m_arrShorts.Length;
            }
            set
            {
                Resize(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int BytesCount
        {
            get
            {
                return Length * Constants.BytesInWord;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int KnownCount
        {
            get
            {
                return DEF_KNOWN_COUNT;
            }
        }

        /// <summary>
        /// Gets / sets internal buffer.
        /// </summary>
        internal short[] Buffer
        {
            get
            {
                return m_arrShorts;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                m_arrShorts = value;
            }
        }


        //    /// <summary>
        //    /// Beginning of the array of shorts
        //    /// </summary>
        //    //[ FieldOffset( 34 ) ]
        //    public ushort rgsw;
        /// <summary>
        /// unique number Identifying the File's creator 0x6A62 is the creator ID for Word and is reserved. Other creators should choose a different value.
        /// </summary>
        //[ FieldOffset( 34 ) ]
        internal ushort wMagicCreated
        {
            get
            {
                return (ushort)m_arrShorts[DEF_OFFSET_MAGIC_CREATED];
            }
            set
            {
                m_arrShorts[DEF_OFFSET_MAGIC_CREATED] = (short)value;
            }
        }
        /// <summary>
        /// identifies the File's last modifier
        /// </summary>
        //[ FieldOffset( 36 ) ]
        internal ushort wMagicRevised
        {
            get
            {
                return (ushort)m_arrShorts[DEF_OFFSET_MAGIC_REVISED];
            }
            set
            {
                m_arrShorts[DEF_OFFSET_MAGIC_REVISED] = (short)value;
            }
        }
        /// <summary>
        /// private data
        /// </summary>
        //[ FieldOffset( 38 ) ]
        internal ushort wMagicCreatedPrivate
        {
            get
            {
                return (ushort)m_arrShorts[DEF_OFFSET_MAGIC_CREATED_PRIVATE];
            }
            set
            {
                m_arrShorts[DEF_OFFSET_MAGIC_CREATED_PRIVATE] = (short)value;
            }
        }
        /// <summary>
        /// private data
        /// </summary>
        //[ FieldOffset( 40 ) ]
        internal ushort wMagicRevisedPrivate
        {
            get
            {
                return (ushort)m_arrShorts[DEF_OFFSET_MAGIC_REVISED_PRIVATE];
            }
            set
            {
                m_arrShorts[DEF_OFFSET_MAGIC_REVISED_PRIVATE] = (short)value;
            }
        }
        /// <summary>
        /// not used
        /// </summary>
        //[ FieldOffset( 42 ) ]
        internal short pnFbpChpFirst_W6
        {
            get
            {
                return m_arrShorts[DEF_OFFSET_FBPCHPFIRST_W6];
            }
            set
            {
                m_arrShorts[DEF_OFFSET_FBPCHPFIRST_W6] = value;
            }
        }
        /// <summary>
        /// not used
        /// </summary>
        //[ FieldOffset( 44 ) ]
        internal short pnChpFirst_W6
        {
            get
            {
                return m_arrShorts[DEF_OFFSET_CHPFIRST_W6];
            }
            set
            {
                m_arrShorts[DEF_OFFSET_CHPFIRST_W6] = value;
            }
        }
        /// <summary>
        /// not used
        /// </summary>
        //[ FieldOffset( 46 ) ]
        internal short cpnBteChp_W6
        {
            get
            {
                return m_arrShorts[DEF_OFFSET_BTECHP_W6];
            }
            set
            {
                m_arrShorts[DEF_OFFSET_BTECHP_W6] = value;
            }
        }
        /// <summary>
        /// not used
        /// </summary>
        //[ FieldOffset( 48 ) ]
        internal short pnFbpPapFirst_W6
        {
            get
            {
                return m_arrShorts[DEF_OFFSET_FBPPAPFIRST_W6];
            }
            set
            {
                m_arrShorts[DEF_OFFSET_FBPPAPFIRST_W6] = value;
            }
        }
        /// <summary>
        /// not used
        /// </summary>
        //[ FieldOffset( 50 ) ]
        internal short pnPapFirst_W6
        {
            get
            {
                return m_arrShorts[DEF_OFFSET_PAPFIRST_W6];
            }
            set
            {
                m_arrShorts[DEF_OFFSET_PAPFIRST_W6] = value;
            }
        }
        /// <summary>
        /// not used
        /// </summary>
        //[ FieldOffset( 52 ) ]
        internal short cpnBtePap_W6
        {
            get
            {
                return m_arrShorts[DEF_OFFSET_BTEPAP_W6];
            }
            set
            {
                m_arrShorts[DEF_OFFSET_BTEPAP_W6] = value;
            }
        }
        /// <summary>
        /// not used
        /// </summary>
        //[ FieldOffset( 54 ) ]
        internal short pnFbpLvcFirst_W6
        {
            get
            {
                return m_arrShorts[DEF_OFFSET_FBPLVCFIRST_W6];
            }
            set
            {
                m_arrShorts[DEF_OFFSET_FBPLVCFIRST_W6] = value;
            }
        }
        /// <summary>
        /// not used
        /// </summary>
        //[ FieldOffset( 56 ) ]
        internal short pnLvcFirst_W6
        {
            get
            {
                return m_arrShorts[DEF_OFFSET_LVCFIRST_W6];
            }
            set
            {
                m_arrShorts[DEF_OFFSET_LVCFIRST_W6] = value;
            }
        }
        /// <summary>
        /// not used
        /// </summary>
        //[ FieldOffset( 58 ) ]
        internal short cpnBteLvc_W6
        {
            get
            {
                return m_arrShorts[DEF_OFFSET_BTELVC_W6];
            }
            set
            {
                m_arrShorts[DEF_OFFSET_BTELVC_W6] = value;
            }
        }
        /// <summary>
        /// Language id if document was written by Far East version of Word (i.e. FIB.fFarEast is on)
        /// </summary>
        //[ FieldOffset( 60 ) ]
        internal short lidFE
        {
            get
            {
                return m_arrShorts[DEF_OFFSET_LIDFE];
            }
            set
            {
                m_arrShorts[DEF_OFFSET_LIDFE] = value;
            }
        }

        #endregion

        #region Class methods
        /// <summary>
        /// Resizes array.
        /// </summary>
        /// <param name="iNewSize">New size of the array.</param>
        public void Resize(int iNewSize)
        {
            if (iNewSize < 0)
                throw new ArgumentOutOfRangeException("iNewSize can't be less than zero.");

            m_arrShorts = new short[iNewSize];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrBuffer"></param>
        public void SetBuffer(byte[] arrBuffer)
        {
            if (arrBuffer == null)
                throw new ArgumentNullException("arrBuffer");

            Resize(arrBuffer.Length / Constants.BytesInWord);
            System.Buffer.BlockCopy(arrBuffer, 0, m_arrShorts, 0, arrBuffer.Length);
            //API.CopyMemory( m_arrShorts, arrBuffer, arrBuffer.Length );
        }
        /// <summary>
        /// Copies data from array of shorts into array of bytes.
        /// </summary>
        /// <param name="arrData">Array of bytes to copy data into.</param>
        /// <param name="iOffset">Starting offset in the destination array.</param>
        /// <returns>Size in bytes of the copied data.</returns>
        public int Save(byte[] arrData, int iOffset)
        {
            if (arrData == null)
                throw new ArgumentNullException("arrData");

            if (iOffset < 0 || iOffset >= arrData.Length)
                throw new ArgumentOutOfRangeException("iOffset");

            int iDataSize = m_arrShorts.Length * Constants.BytesInWord;

            if (iOffset + iDataSize > arrData.Length)
                throw new ArgumentOutOfRangeException("iOffset");

            System.Buffer.BlockCopy(m_arrShorts, 0, arrData, iOffset, iDataSize);
            //      API.CopyMemory( ref arrData[ iOffset ], m_arrShorts, iDataSize );
            return iDataSize;
        }
        #endregion
    }
}
