#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

#if DOCIO
namespace Syncfusion.CompoundFile.DocIO
#else
namespace Syncfusion.CompoundFile.XlsIO
#endif
{
    /// <summary>
    /// This interface represents stream in the compound file.
    /// </summary>
    public abstract class CompoundStream : Stream
    {
        #region Members
        /// <summary>
        /// Name of the stream.
        /// </summary>
        private string m_strStreamName;
        #endregion

        #region Methods
        ///// <summary>
        ///// Default constructor.
        ///// </summary>
        //public CompoundStream()
        //{
        //}
        /// <summary>
        /// Initializes new instance of the compound stream object.
        /// </summary>
        /// <param name="streamName">Name of the stream.</param>
        public CompoundStream(string streamName)
        {
            m_strStreamName = streamName;
        }
        /// <summary>
        /// Copies stream content into another stream object.
        /// </summary>
        /// <param name="stream">Stream to copy data into.</param>
        public virtual void CopyTo(CompoundStream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            const int BufferSize = 32768;
            byte[] arrBuffer = new byte[BufferSize];
            long lStartPosition = Position;
            int iReadCount;

            while ((iReadCount = Read(arrBuffer, 0, BufferSize)) > 0)
            {
                stream.Write(arrBuffer, 0, iReadCount);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Returns name of the stream.
        /// </summary>
        public string Name
        {
            get
            {
                return m_strStreamName;
            }
            protected set
            {
                m_strStreamName = value;
            }
        }
        #endregion
    }
}
