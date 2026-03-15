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
using System.Text;
using System.IO;
using System.Collections;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Escher
{
    /// <summary>
    /// Summary description for SPContainer.
    /// </summary>
    //[ CLSCompliant( false ) ]
    internal class SPContainer : BaseWordRecord
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_MSOFBH_LENGTH = 8;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_SP_LENGTH = 8;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_FOPTE_LENGTH = 6;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_RECT_LENGTH = 4;
        #endregion

        #region Class Members
        /// <summary>
        /// 
        /// </summary>
        private FSP m_sp;
        /// <summary>
        /// 
        /// </summary>
        private List<FOPTE> m_opt;
        /// <summary>
        /// 
        /// </summary>
        private Rect m_anchor;
        /// <summary>
        /// 
        /// </summary>
        private List<MSOFBH> m_msofbhArray;

        #endregion

        #region Class Initialize/Finalize Methods
        /// <summary>
        /// 
        /// </summary>
        public SPContainer()
        {
            m_opt = new List<FOPTE>();
        }
        #endregion

        #region Class Properties
        /// <summary>
        /// 
        /// </summary>
        public new int Length
        {
            get
            {
                int len = 0;
                len += DEF_MSOFBH_LENGTH + DEF_SP_LENGTH;
                len += GetFoptesLength();
                len += DEF_MSOFBH_LENGTH + DEF_RECT_LENGTH;
                return len;
            }
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="lenght"></param>
        public void Read(Stream stream, uint lenght)
        {
            m_msofbhArray = new List<MSOFBH>();
            uint endPosition = (uint)stream.Position + lenght;

            while (stream.Position < endPosition)
            {
                MSOFBH msofbh = new MSOFBH();
                msofbh.Read(stream);
                m_msofbhArray.Add(msofbh);
                switch (msofbh.Msofbt)
                {
                    case MSOFBT.msofbtSp:
                        m_sp = new FSP();
                        m_sp.Read(stream);
                        break;

                    case MSOFBT.msofbtOPT:
                        ReadFoptes(stream, msofbh);
                        break;

                    case MSOFBT.msofbtAnchor:
                    case MSOFBT.msofbtClientAnchor:
                    case MSOFBT.msofbtChildAnchor:
                        m_anchor = new Rect();
                        m_anchor.Read(stream);
                        break;

                    default:
                        stream.Position += msofbh.Length;
                        break;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void Write(Stream stream)
        {
            //stream.Position -= 6;
            GenerateDefaultOPT();

            MSOFBH msofbh = new MSOFBH();
            msofbh.Msofbt = MSOFBT.msofbtSpContainer;
            msofbh.Inst = 0;
            msofbh.Length = (uint)Length;
            msofbh.Version = 15;
            msofbh.Write(stream);

            WriteFSP(stream);

            //default
            WriteFoptes(stream);

            m_anchor = new Rect();
            m_anchor.Bottom = 120;

            msofbh = new MSOFBH();
            msofbh.Msofbt = MSOFBT.msofbtClientAnchor;
            msofbh.Length = 4;
            msofbh.Inst = 0;
            msofbh.Version = 0;
            msofbh.Write(stream);

            m_anchor.Write(stream);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="msofbh"></param>
        private void ReadFoptes(Stream stream, MSOFBH msofbh)
        {
            int fopteCount = (int)msofbh.Length / 6;
            int bCount = 0;

            for (int i = 0; i < fopteCount && bCount < msofbh.Length; i++)
            {
                FOPTE fopte = new FOPTE();
                bCount += fopte.Read(stream);
                m_opt.Add(fopte);
            }

            for (int i = 0; i < m_opt.Count; i++)
            {
                if (m_opt[i].IsComplex)
                {
                    stream.Read(m_opt[i].NameBytes, 0, m_opt[i].NameBytes.Length);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        private void WriteFoptes(Stream stream)
        {
            MSOFBH msofbh = new MSOFBH();
            msofbh.Msofbt = MSOFBT.msofbtOPT;
            msofbh.Length = (uint)GetFoptesLength();
            msofbh.Inst = 4;
            msofbh.Version = 3;

            msofbh.Write(stream);

            for (int i = 0; i < m_opt.Count; i++)
            {
                m_opt[i].Write(stream);
            }
            for (int i = 0; i < m_opt.Count; i++)
            {
                if (m_opt[i].IsComplex)
                {
                    stream.Write(m_opt[i].NameBytes, 0, m_opt[i].NameBytes.Length);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int GetFoptesLength()
        {
            int len = 0;
            if (m_opt != null)
            {
                len += DEF_MSOFBH_LENGTH;
                for (int i = 0; i < m_opt.Count; i++)
                {
                    if (m_opt[i].IsComplex)
                    {
                        len += (int)(m_opt[i].Op + DEF_FOPTE_LENGTH);
                    }
                    else
                    {
                        len += DEF_FOPTE_LENGTH;
                    }
                }
            }
            return len;
        }
        /// <summary>
        /// 
        /// </summary>
        private void GenerateDefaultOPT()
        {
            m_opt = new List<FOPTE>();

            //pib
            FOPTE fopte = new FOPTE();
            fopte.IsBid = true;
            fopte.IsComplex = false;
            fopte.Pid = (ushort)BlipFopte.pib;
            fopte.Op = 1;

            m_opt.Add(fopte);

            //pibName
            fopte = new FOPTE();
            fopte.IsBid = true;
            fopte.IsComplex = true;
            fopte.Pid = (ushort)BlipFopte.pibName;
            fopte.NameBytes = Encoding.Unicode.GetBytes("autowalls_ru_17\0");
            fopte.Op = (uint)fopte.NameBytes.Length;

            m_opt.Add(fopte);

            //pibFlags
            fopte = new FOPTE();
            fopte.IsBid = false;
            fopte.IsComplex = false;
            fopte.Pid = (ushort)BlipFopte.pibFlags;
            fopte.Op = 2;

            m_opt.Add(fopte);

            //NoLineDrawDash
            fopte = new FOPTE();
            fopte.IsBid = false;
            fopte.IsComplex = false;
            fopte.Pid = (ushort)BlipFopte.NoLineDrawDash;
            fopte.Op = 524288;

            m_opt.Add(fopte);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        private void WriteFSP(Stream stream)
        {
            MSOFBH msofbh = new MSOFBH();
            msofbh.Msofbt = MSOFBT.msofbtSp;
            msofbh.Inst = 75;
            msofbh.Version = 2;
            msofbh.Length = DEF_SP_LENGTH;

            msofbh.Write(stream);

            //Default fsp
            FSP fsp = new FSP();
            fsp.Spid = 1026;
            fsp.GzfPersistent = 2560;

            fsp.Write(stream);
        }
        #endregion
    }
}
