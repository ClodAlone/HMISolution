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

using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// 
    /// </summary>
    internal class ListInfo : BaseWordRecord
    {
        #region Class constants
        /// <summary>
        /// Default list id
        /// </summary>
        private const int DEF_LIST_ID = 1720085641;

        /// <summary>
        /// Strings for bulleted lists
        /// </summary>
        private const string DEF_BULLLET_FIRST = "\uf0b7";
        private const string DEF_BULLLET_SECOND = "o";
        private const string DEF_BULLLET_THIRD = "\uf0a7";

        /// <summary>
        /// 
        /// </summary>
        private const int DEF_MULTIPLIER = 1440;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private int m_listid = DEF_LIST_ID;

        private ListFormats m_listFormats;
        private ListFormatOverrides m_listFormatOverrides;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal ListInfo()
        {
            m_listFormats = new ListFormats();
            m_listFormatOverrides = new ListFormatOverrides();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="stream"></param>
        internal ListInfo(WPFIBData fib, Stream stream)
        {
            m_listFormats = new ListFormats();
            m_listFormatOverrides = new ListFormatOverrides();

            ReadLst(fib, stream);
            ReadLfo(fib, stream);
            ReadStringTable(fib, stream);
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="stream"></param>
        internal void ReadLst(WPFIBData fib, Stream stream)
        {
            if (fib.lcbPlcfLst != 0)
            {
                stream.Position = fib.fcPlcfLst;
                int count = ReadInt16(stream);
                for (int i = 0; i < count; i++)
                {
                    m_listFormats.Add(new ListData(stream));
                }
                foreach (ListData listData in m_listFormats)
                {
                    listData.ReadLvl(stream);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="stream"></param>
        internal void ReadLfo(WPFIBData fib, Stream stream)
        {
            if (fib.lcbPlfLfo != 0)
            {
                stream.Position = fib.fcPlfLfo;
                int count = ReadInt32(stream);
                for (int i = 0; i < count; i++)
                {
                    m_listFormatOverrides.Add(new ListFormatOverride(stream));
                }
                foreach (ListFormatOverride lfo in m_listFormatOverrides)
                {
                    lfo.ReadLfoLvls(stream);
                }
            }
        }

        /// <summary>
        /// Reads lists' names
        /// </summary>
        /// <param name="fib"></param>
        /// <param name="stream"></param>
        internal void ReadStringTable(WPFIBData fib, Stream stream)
        {
            stream.Position = fib.fcSttbListNames;
            //      StringCollection strCollection = new StringCollection();
            //      TODO : add functionality to read string table
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="memConvertor"></param>
        /// <returns></returns>
        internal int WriteLfo(Stream stream)
        {
            if (m_listFormatOverrides.Count == 0)
            {
                return 0;
            }
            int pos = (int)stream.Position;
            //stream.Write();
            WriteInt32(stream, m_listFormatOverrides.Count);
            foreach (ListFormatOverride lfo in m_listFormatOverrides)
            {
                lfo.WriteLfo(stream);
            }

            foreach (ListFormatOverride lfo in m_listFormatOverrides)
            {
                lfo.WriteLfoLvls(stream);
            }
            return (((int)stream.Position) - pos);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="memConvertor"></param>
        /// <returns></returns>
        internal int WriteLst(Stream stream)
        {
            if (m_listFormats.Count == 0)
            {
                return 0;
            }
            int pos = (int)stream.Position;
            WriteInt16(stream, (short)m_listFormats.Count);
            foreach (ListData listData in m_listFormats)
            {
                listData.WriteListData(stream);
            }
            int byteCount = (int)stream.Position - pos;
            foreach (ListData listDat in m_listFormats)
            {
                listDat.WriteLvl(stream);
            }

            return byteCount;
        }

        /// <summary>
        /// Writes lists' names
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal int WriteStringTable(Stream stream)
        {
            // TODO : add functionality to write string table
            byte[] arr = new byte[8];
            arr[0] = arr[1] = 0xFF;
            arr[2] = 1;
            //stream.Write( BitConverter.GetBytes( arr.Length ), 0, Constants.BytesInWord );
            stream.Write(arr, 0, arr.Length);
            return arr.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal short ApplyNumberList()
        {
            ListData listFormat = new ListData(m_listid);
            m_listid++;
            m_listFormats.Add(listFormat);
            int level = 0;

            for (float i = 0.5f; i < 4.5f; i += 1.5f)
            {
                listFormat.Levels.Add(
                  CreateNumberLvl((int)(DEF_MULTIPLIER * i), level++, ListPatternType.Arabic, ListNumberAlignment.Left));
                listFormat.Levels.Add(
                  CreateNumberLvl((int)(DEF_MULTIPLIER * (i + 0.5)),
                                   level++,
                                   ListPatternType.LowLetter,
                                   ListNumberAlignment.Right));
                listFormat.Levels.Add(
                  CreateNumberLvl((int)(DEF_MULTIPLIER * (i + 1)), level++, ListPatternType.LowRoman, ListNumberAlignment.Left));
            }

            ListFormatOverride lfo = new ListFormatOverride();
            lfo.ListID = listFormat.ListID;
            // returns index of created list
            m_listFormatOverrides.Add(lfo);
            return Convert.ToInt16(m_listFormatOverrides.Count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="levelNumber"></param>
        /// <returns></returns>
        internal ListData GetLevelFormat(int levelNumber)
        {
            ListFormatOverride lfo = m_listFormatOverrides[levelNumber - 1];
            return m_listFormats.FindListData(lfo.ListID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal ListInfo Clone()
        {
            ListInfo listInfo = MemberwiseClone() as ListInfo;
            return listInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal short ApplyBulletList()
        {
            ListData listFormat = new ListData(m_listid);
            m_listid++;
            m_listFormats.Add(listFormat);

            for (float i = 0.5f; i < 4.5f; i += 1.5f)
            {
                listFormat.Levels.Add(CreateBuletLvl((int)(DEF_MULTIPLIER * i), DEF_BULLLET_FIRST));
                listFormat.Levels.Add(CreateBuletLvl((int)(DEF_MULTIPLIER * (i + 0.5)), DEF_BULLLET_SECOND));
                listFormat.Levels.Add(CreateBuletLvl((int)(DEF_MULTIPLIER * (i + 1)), DEF_BULLLET_THIRD));
            }

            ListFormatOverride lfo = new ListFormatOverride();
            lfo.ListID = listFormat.ListID;
            // returns index of created list
            m_listFormatOverrides.Add(lfo);
            return Convert.ToInt16(m_listFormatOverrides.Count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listData"></param>
        /// <param name="listFormat"></param>
        /// <param name="styleSheet"></param>
        /// <returns></returns>
        internal short ApplyList(ListData listData, WListFormat listFormat, WordStyleSheet styleSheet)
        {
            m_listFormats.Add(listData);
            // returns index of created list
            return ApplyLFO(listData, listFormat, styleSheet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listData"></param>
        /// <param name="listFormat"></param>
        /// <param name="styleSheet"></param>
        /// <returns></returns>
        internal short ApplyLFO(ListData listData, WListFormat listFormat, WordStyleSheet styleSheet)
        {
            ListFormatOverride lfo = new ListFormatOverride();
            if (listFormat.LFOStyleName != null)
            {
                string lfoStyleName = listFormat.LFOStyleName;
                ListOverrideStyle lstOverrideStyle =
                  (listFormat.Document as WordDocument).ListOverrides.FindByName(lfoStyleName);
                if (lstOverrideStyle != null)
                {
                    ListPropertiesConverter.ImportListOverride(lstOverrideStyle, lfo, styleSheet);
                }
            }
            lfo.ListID = listData.ListID;
            // returns index of created list
            m_listFormatOverrides.Add(lfo);
            return Convert.ToInt16(m_listFormatOverrides.Count);
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets ListFormatOverrides object
        /// </summary>
        internal ListFormatOverrides ListFormatOverrides
        {
            get
            {
                return m_listFormatOverrides;
            }
        }

        /// <summary>
        /// Gets ListFormats object
        /// </summary>
        internal ListFormats ListFormats
        {
            get
            {
                return m_listFormats;
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private ListLevel CreateBuletLvl(int dxLeft, string str)
        {
            ListLevel lvl = new ListLevel();
            //      lvl.m_nfc = ListPatternType.Bullet;
            //      lvl.m_dxaSpace = 360;
            //      lvl.m_bPrev = true;
            //
            //      SinglePropertyModifierArray pap = new SinglePropertyModifierArray();
            //      pap.SetValue( WordSprmOptions.sprmPDxaLeft, dxLeft );
            //
            //      SinglePropertyModifierArray chp = new SinglePropertyModifierArray();
            //      
            //      // we switch font depending on bullet style
            //      int fontIndex = 0;
            //      switch (str)
            //      {
            //        case DEF_BULLLET_FIRST:
            //          fontIndex = 1;
            //          break;
            //          
            //        case DEF_BULLLET_SECOND:
            //          fontIndex = 5;
            //          break;
            //          
            //        case DEF_BULLLET_THIRD:
            //          fontIndex = 4;
            //          break;
            //      }
            //      
            //      chp.SetValue( WordSprmOptions.sprmCRgFtc0, fontIndex );
            //      chp.SetValue( WordSprmOptions.sprmCRgFtc2, fontIndex );
            //
            //      lvl.m_chpx = chp;
            //      lvl.m_papx = pap;
            //      lvl.m_str = str;
            //
            return lvl;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dxLeft"></param>
        /// <param name="levelNumber"></param>
        /// <param name="nfc"></param>
        /// <param name="align"></param>
        /// <returns></returns>
        private ListLevel CreateNumberLvl(int dxLeft, int levelNumber, ListPatternType nfc, ListNumberAlignment align)
        {
            ListLevel lvl = new ListLevel();
            //      lvl.m_nfc = nfc;
            //      lvl.m_jc = align;
            //      lvl.m_dxaSpace = 360;
            //      lvl.m_bPrev = true;
            //      lvl.m_unused = levelNumber > 0;
            //
            //      SinglePropertyModifierArray pap = new SinglePropertyModifierArray();
            //      pap.SetValue( WordSprmOptions.sprmPDxaLeft, dxLeft );
            //
            //      SinglePropertyModifierArray chp = new SinglePropertyModifierArray();
            //
            //      lvl.m_papx = pap;
            //      lvl.m_chpx = chp;
            //      char[] chArray1 = new char[2] {( char )( ( ushort )levelNumber ), '.'};
            //      lvl.m_str = new string( chArray1 );
            //      lvl.m_rgbxchNums[ 0 ] = 1;
            //
            return lvl;
        }
        #endregion
    }
}

