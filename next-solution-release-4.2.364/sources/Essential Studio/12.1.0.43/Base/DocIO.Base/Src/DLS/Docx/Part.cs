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


#region File using directives
using System;
using System.IO;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for Part.
    /// </summary>
    internal class Part
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        protected Stream m_dataStream;
        protected string m_name;
        #endregion

        #region Properties
        /// <summary>
        /// Gets data stream.
        /// </summary>
        internal Stream DataStream
        {
            get
            {
                return m_dataStream;
            }
        }
        /// <summary>
        /// Gets/sets part name.
        /// </summary>
        internal string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Part"/> class.
        /// </summary>
        /// <param name="dataStream">The data stream.</param>
        public Part(Stream dataStream)
        {
            if (dataStream == null)
            {
                m_dataStream = new MemoryStream();
                return;
            }
            byte[] streamBytes = new byte[dataStream.Length];
            dataStream.Position = 0;
            dataStream.Read(streamBytes, 0, (int)dataStream.Length);
            m_dataStream = new MemoryStream(streamBytes);
            //m_dataStream = dataStream;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal Part Clone()
        {
            Part newPart = new Part(m_dataStream);
            newPart.Name = m_name;

            return newPart;
        }
        /// <summary>
        /// Sets the data stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void SetDataStream(Stream stream)
        {
            m_dataStream.Dispose();
            m_dataStream = stream;
        }
        #endregion
    }
}

