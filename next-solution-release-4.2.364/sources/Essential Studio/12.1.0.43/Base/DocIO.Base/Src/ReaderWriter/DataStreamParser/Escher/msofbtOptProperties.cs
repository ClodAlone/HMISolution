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
using System.IO;
using System.Collections.Generic;
using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    ///
    /// </summary>
    internal class msofbtRGFOPTE : DocIOSortedList<int, FOPTEBase>
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        public const int DEF_TXID = 128;
        public const uint DEF_LINE_WIDTH_PT = 12700;
        public const float DEF_LINE_WIDTH = 0.75f;

        public const uint DEF_COLOR_EMPTY = 4278190080;
        public const uint DEF_NO_LINE = 524288;
        public const uint DEF_COLOR_FILL = 1048592;
        public const uint DEF_NO_COLOR_FILL = 1048576;
        public const uint DEF_BEHIND_DOC = 2097184;
        public const uint DEF_NOT_BEHIND_DOC = 2097152;
        public const uint DEF_FIT_TEXT_TO_SHAPE = 131072;
        public const uint DEF_BACKGROND_SHAPE = 65537;

        //    /// <summary>
        //    /// Picture color constants.
        //    /// </summary>
        //    public const uint DEF_BRIGHTNESS_BORDER = 4294934528;
        //    public const uint DEF_BRIGHTNESS_STEP = 655;
        //    public const uint DEF_CONTRAST_BORDER = 64226;
        //    public const uint DEF_CONTRAST_STEP = 1310;
        //    public const uint DEF_GRAYSCALE_COLOR = 393220;
        //    public const uint DEF_BLACKWHITE_COLOR = 393222;
        //    public const uint DEF_CROP_STEP = 3302;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private int m_prevPid;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        public msofbtRGFOPTE()
        {
        }
        #endregion

        #region Class methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fopteBase"></param>
        internal void Add(FOPTEBase fopteBase)
        {
            Add(fopteBase.Id, fopteBase);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>Count of bytes written.</returns>
        internal int Write(Stream stream)
        {
            int startPos = (int)stream.Position;

            foreach( FOPTEBase fopte in this.Values )
            {
                if (fopte.Id > 10000)
                {
                    FOPTEBase fopteCloned = fopte.Clone();
                    //Handled specifically for duplicate properties.
                    fopteCloned.Id -= 10000;
                    fopteCloned.Write(stream);
                }
                else
                {
                    fopte.Write(stream);
                }
            }

            foreach (FOPTEBase fBase in this.Values)
            {
                FOPTEComplex fopteComplex = fBase as FOPTEComplex;
                if (fopteComplex != null)
                {
                    fopteComplex.WriteData(stream);
                }
            }

            return (int)(stream.Position - startPos);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="length"></param>
        internal void Read(Stream stream, int length)
        {
            long endPos = stream.Position + length;

            while (stream.Position < endPos)
            {
                byte[] buf = new byte[4];

                stream.Read(buf, 0, 2);
                int tmp = BitConverter.ToInt16(buf, 0);
                int pid = tmp & 0x3fff;
                bool fBid = (tmp & 0x4000) != 0;
                bool fComplex = (tmp & 0x8000) != 0;

                stream.Read(buf, 0, 4);
                uint op = BitConverter.ToUInt32(buf, 0);

                if (fComplex)
                {
                    Add(pid, new FOPTEComplex(pid, fBid, (int)op));
                    endPos -= op;
                    m_prevPid = pid;
                }
                else
                {
                    if (pid < m_prevPid)
                    {
                        m_prevPid = pid;
                        //Handled specifically for duplicate properties.
                        pid += 10000;
                    }
                    else
                        m_prevPid = pid;

                    Add(pid, new FOPTEBid(pid, fBid, op));
                }
            }

            foreach (FOPTEBase fBase in this.Values)
            {
                FOPTEComplex fopteComplex = fBase as FOPTEComplex;
                if (fopteComplex != null)
                {
                    fopteComplex.ReadData(stream);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal List<FOPTEBase> GetPostProps()
        {
            List<FOPTEBase> postProps = new List<FOPTEBase>();
            foreach( FOPTEBase fopte in this.Values )
            {
                if (fopte.Id < 118 && fopte.Id != 4)
                {
                    postProps.Add(fopte);
                }
            }
            return postProps;
        }

        #endregion
    }
}
