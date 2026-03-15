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
using System.Collections;
using System.IO;

using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using System.Collections.Generic; 
#endregion

namespace Syncfusion.DocIO.ReaderWriter
{
    /// <summary>
    /// Summary description for annotations.
    /// </summary>
    [CLSCompliant(false)]
    internal class AnnotationsRW
      : SubDocumentRW
    {
        #region Class members
        /// <summary>
        /// Annotation owners.
        /// </summary>
        private List<String> m_grpXstAtnOwners;
        private AnnotationBookmarks m_bookmarks;

        private AnnotationDescriptor m_currDescriptor;
        private int m_descIndex = -1;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="AnnotationsRW"/> class.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fib"></param>
        internal AnnotationsRW(Stream stream, WPFIBData fib)
            : base(stream, fib)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="AnnotationsRW"/> class.
        /// </summary>
        internal AnnotationsRW()
            : base()
        {
            m_bookmarks = new AnnotationBookmarks();
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// Reads.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="fib">The fib.</param>
        internal override void Read(Stream stream, WPFIBData fib)
        {
            base.Read(stream, fib);
            ReadGXAO(fib.fcGrpXstAtnOwners, fib.lcbGrpXstAtnOwners);
            m_bookmarks = new AnnotationBookmarks(m_reader, fib);
        }
        /// <summary>
        /// Writes.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="fib">The fib.</param>
        internal override void Write(Stream stream, WPFIBData fib)
        {
            base.Write(stream, fib);
            WriteGXAO();

            m_bookmarks.Write(m_writer, fib);
        }

        /// <summary>
        /// Adds the descriptor.
        /// </summary>
        /// <param name="atrd">The annotation descriptor.</param>
        /// <param name="pos">The pos.</param>
        /// <param name="bkmkStart">The bookmark start.</param>
        /// <param name="bkmkEnd">The bookmark end.</param>
        internal void AddDescriptor(AnnotationDescriptor atrd, int pos, int bkmkStart, int bkmkEnd)
        {
            AddDescriptor(atrd, pos);
            int key = atrd.TagBkmk;

            if (key != -1)
            {
                m_bookmarks.Add(key, new AnnotationBookmark(bkmkStart, bkmkEnd));
            }
        }
        /// <summary>
        /// Adds the descriptor.
        /// </summary>
        /// <param name="atrd">The annotation descriptor.</param>
        /// <param name="pos">The pos.</param>
        internal void AddDescriptor(AnnotationDescriptor atrd, int pos)
        {
            m_descriptorsAnnot.Add(atrd);
            AddRefPosition(pos);
        }
        /// <summary>
        /// Adds the GXAO.
        /// </summary>
        /// <param name="gxao">The gxao.</param>
        /// <returns></returns>
        internal int AddGXAO(string gxao)
        {
            int index = m_grpXstAtnOwners.IndexOf(gxao);

            if (index == -1)
            {
            	index = m_grpXstAtnOwners.Count;
                m_grpXstAtnOwners.Add(gxao);
            }

            return index;
        }

        /// <summary>
        /// Gets the descriptor.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal AnnotationDescriptor GetDescriptor(int index)
        {
            if (index != m_descIndex && index < m_descriptorsAnnot.Count)
            {
                m_currDescriptor = m_descriptorsAnnot[index];
                m_descIndex = index;
            }

            return m_currDescriptor;
        }
        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="index">Index of the atn.</param>
        /// <returns></returns>
        internal string GetUser(int index)
        {
            AnnotationDescriptor desc = GetDescriptor(index);

            if (desc == null)
                return "";

            if (desc.IndexToGrpOwner < m_grpXstAtnOwners.Count)
            {
                return m_grpXstAtnOwners[(int)desc.IndexToGrpOwner].ToString();
            }

            return "";
        }
        /// <summary>
        /// Gets the bookmark start.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal int GetBookmarkStartOffset(int index)
        {
            AnnotationDescriptor desk = GetDescriptor(index);

            if (desk.TagBkmk == -1)
                return 0;

            AnnotationBookmark bookmark = m_bookmarks[desk.TagBkmk];

            if (bookmark == null)
                return 0;

            return (m_refPositions[index]) - bookmark.Start;
        }
        /// <summary>
        /// Gets the bookmark end.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal int GetBookmarkEndOffset(int index)
        {
            AnnotationDescriptor desk = GetDescriptor(index);

            if (desk.TagBkmk == -1)
                return 0;

            AnnotationBookmark bookmark = m_bookmarks[desk.TagBkmk];

            if (bookmark == null)
                return 0;

            return bookmark.End - m_refPositions[index];
        }
        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal int GetPosition(int index)
        {
            return m_refPositions[index];
        }
        #endregion

        #region Class helper methods / reads
        /// <summary>
        /// Reads the GXAO.
        /// </summary>
        /// <param name="pos">The pos.</param>
        /// <param name="length">The length.</param>
        private void ReadGXAO(int pos, int length)
        {
            if (length > 0)
            {
                m_reader.BaseStream.Position = pos;
                int count = m_reader.ReadInt16();

                while (count != 0)
                {
#if SILVERLIGHT || WP
					byte[] bytes = m_reader.ReadBytes( count * Constants.BytesInWord );
                    string gxao = Encoding.Unicode.GetString(bytes, 0, bytes.Length);
#else
					string gxao = Encoding.Unicode.GetString(m_reader.ReadBytes(count * Constants.BytesInWord));
#endif
                    
                    AddGXAO(gxao);
                    count = (m_reader.BaseStream.Position != pos + length) ? (int)m_reader.ReadInt16() : 0;
                }
            }
        }
        #endregion

        #region Class helper methods / writes
        /// <summary>
        /// Writes the GXAO.
        /// </summary>
        private void WriteGXAO()
        {
            if (m_grpXstAtnOwners.Count > 0)
            {
                m_fib.fcGrpXstAtnOwners = (int)m_writer.BaseStream.Position;

                //        foreach( string entry in m_grpXstAtnOwners )
                string entry = null;
                for (int i = 0, cnt = m_grpXstAtnOwners.Count; i < cnt; i++)
                {
                    entry = m_grpXstAtnOwners[i];
                    short count = (short)entry.Length;
                    m_writer.Write(count);
                    m_writer.Write(Encoding.Unicode.GetBytes(entry));
                }

                m_fib.lcbGrpXstAtnOwners = (int)(m_writer.BaseStream.Position - m_fib.fcGrpXstAtnOwners);
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Inits this instance.
        /// </summary>
        protected override void Init()
        {
            base.Init();
            m_grpXstAtnOwners = new List<String>();
        }
        #endregion

        #region Class overrides / reads
        /// <summary>
        /// Reads the text positions.
        /// </summary>
        protected override void ReadTxtPositions()
        {
            int length = m_fib.lcbPlcfandTxt;

            if (length > 0)
            {
                m_reader.BaseStream.Position = m_fib.fcPlcfandTxt;
                int count = length / 4;
                ReadTxtPositions(count);
            }
        }
        /// <summary>
        /// Reads the descriptors.
        /// </summary>
        protected override void ReadDescriptors()
        {
            int length = m_fib.lcbPlcfandRef;
            if (length > 0)
            {
                m_reader.BaseStream.Position = m_fib.fcPlcfandRef;
                ReadDescriptors(length, AnnotationDescriptor.DEF_LENGTH);
                base.ReadDescriptors();
            }
        }
        /// <summary>
        /// Reads the descriptor.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="pos">The position.</param>
        /// <param name="posNext">The next positiopn.</param>
        protected override void ReadDescriptor(BinaryReader reader, int pos, int posNext)
        {
            if (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                base.ReadDescriptor(reader, pos, posNext);
                m_descriptorsAnnot.Add(new AnnotationDescriptor(reader));
            }
        }
        #endregion

        #region Class overrides / writes
        /// <summary>
        /// Writes the descriptors.
        /// </summary>
        protected override void WriteDescriptors()
        {
            m_fib.fcPlcfandRef = (int)m_writer.BaseStream.Position;
            WriteRefPositions(m_endReference);

            //      foreach( AnnotationDescriptor entry in m_descriptors )
            AnnotationDescriptor entry = null;
            for (int i = 0, cnt = m_descriptorsAnnot.Count; i < cnt; i++)
            {
                entry = m_descriptorsAnnot[i];
                entry.Write(m_writer);
            }

            m_fib.lcbPlcfandRef = (int)(m_writer.BaseStream.Position - m_fib.fcPlcfandRef);
        }
        /// <summary>
        /// Writes the text positions.
        /// </summary>
        protected override void WriteTxtPositions()
        {
            if (m_txtPositions.Count > 0)
            {
                m_fib.fcPlcfandTxt = (int)m_writer.BaseStream.Position;
                WriteTxtPositionsBase();
                m_fib.lcbPlcfandTxt = (int)(m_writer.BaseStream.Position - m_fib.fcPlcfandTxt);
            }
        }
        #endregion

        #region Class internal declaration

        /// <summary>
        /// 
        /// </summary>
        internal class AnnotationBookmark
        {
            #region Fields
            private int m_iStartPos = -1;
            private int m_iEndPos = -1;
            #endregion

            #region Properties
            /// <summary>
            /// 
            /// </summary>
            internal int Start
            {
                get
                {
                    return m_iStartPos;
                }
                set
                {
                    m_iStartPos = value;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            internal int End
            {
                get
                {
                    return m_iEndPos;
                }
                set
                {
                    m_iEndPos = value;
                }
            }
            #endregion

            #region Constructors
            /// <summary>
            /// 
            /// </summary>
            /// <param name="start"></param>
            /// <param name="end"></param>
            internal AnnotationBookmark(int start, int end)
            {
                m_iEndPos = end;
                m_iStartPos = start;
            }
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        internal class AnnotationBookmarks
        {
            #region Fields
            private BookmarkDescriptor m_descriptor;
            private List<Int32> m_keys = new List<Int32>();
            private int m_bookmarkCount;
            #endregion

            #region Properties
            /// <summary>
            /// 
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            internal AnnotationBookmark this[int key]
            {
                get
                {
                    if (m_keys.Contains(key))
                    {
                        int index = m_keys.IndexOf(key);
                        AnnotationBookmark bookmark = new AnnotationBookmark(m_descriptor.GetBeginPos(index),
                          m_descriptor.GetEndPos(index));
                        return bookmark;
                    }
                    return null;
                }
            }
            #endregion

            #region Constructors
            /// <summary>
            ///
            /// </summary>
            /// <param name="reader"></param>
            /// <param name="fib"></param>
            internal AnnotationBookmarks(BinaryReader reader, WPFIBData fib)
            {
                Read(reader, fib);
            }
            /// <summary>
            /// 
            /// </summary>
            internal AnnotationBookmarks()
            {
                m_descriptor = new BookmarkDescriptor();
            }
            #endregion

            #region internal methods
            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            /// <param name="fib"></param>
            internal void Read(BinaryReader reader, WPFIBData fib)
            {
                ReadSttbf(reader, fib.fcSttbfAtnbkmk, fib.lcbSttbfAtnbkmk);

                if (m_bookmarkCount > 0)
                {
                    m_descriptor = new BookmarkDescriptor(reader.BaseStream, m_bookmarkCount,
                      fib.fcPlcfAtnbkf, fib.lcbPlcfAtnbkf, fib.fcPlcfAtnbkl, fib.lcbPlcfAtnbkl);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="key"></param>
            /// <param name="bookmark"></param>
            internal void Add(int key, AnnotationBookmark bookmark)
            {
                m_keys.Add(key);
                m_descriptor.Add(bookmark.Start);
                m_descriptor.SetEndPos(m_descriptor.BookmarkCount - 1, bookmark.End);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="writer"></param>
            /// <param name="fib"></param>
            internal void Write(BinaryWriter writer, WPFIBData fib)
            {
                if (m_descriptor.BookmarkCount == 0)
                    return;

                int count = m_descriptor.BookmarkCount;

                int[] keys = new int[count];
                int[] values = new int[count];
#if WINRT
                System.Collections.Generic.SortedDictionary<int, int> sortDic = new System.Collections.Generic.SortedDictionary<int, int>();
#endif
                for (int i = 0; i < m_descriptor.BookmarkCount; i++)
                {
                    keys[i] = m_descriptor.GetBeginPos(i);
                    values[i] = m_keys[i];
#if WINRT
                    sortDic.Add(keys[i], values[i]);
#endif
                }
                
#if WINRT
                keys = new int[sortDic.Count];
                values = new int[sortDic.Count];
                sortDic.Keys.CopyTo(keys, 0);
                sortDic.Values.CopyTo(values, 0);
#else
                Array.Sort(keys, values, new Comparer());
#endif

                fib.fcSttbfAtnbkmk = (int)writer.BaseStream.Position;
                WriteSttbf(writer, values);
                fib.lcbSttbfAtnbkmk = (int)(writer.BaseStream.Position - fib.fcSttbfAtnbkmk);

                int end = fib.ccpText + 2;

                fib.fcPlcfAtnbkf = (int)writer.BaseStream.Position;
                WriteBKF(writer, keys, values, end);
                fib.lcbPlcfAtnbkf = (int)(writer.BaseStream.Position - fib.fcPlcfAtnbkf);

                fib.fcPlcfAtnbkl = (int)writer.BaseStream.Position;
                WriteBKL(writer, end);
                fib.lcbPlcfAtnbkl = (int)(writer.BaseStream.Position - fib.fcPlcfAtnbkl);
            }
            #endregion

            #region Implementation / reads
            /// <summary>
            /// 
            /// </summary>
            /// <param name="reader"></param>
            /// <param name="start"></param>
            /// <param name="length"></param>
            private void ReadSttbf(BinaryReader reader, int start, int length)
            {
                if (length > 0)
                {
                    reader.BaseStream.Position = start + 2;
                    m_bookmarkCount = reader.ReadInt16();
                    reader.ReadInt16();

                    for (int i = 0; i < m_bookmarkCount; i++)
                    {
                        reader.ReadInt32();
                        m_keys.Add(reader.ReadInt32());
                        reader.ReadInt32();
                    }
                }
            }
            #endregion

            #region Implementation / writes
            /// <summary>
            /// 
            /// </summary>
            /// <param name="writer"></param>
            /// <param name="values"></param>
            private void WriteSttbf(BinaryWriter writer, int[] values)
            {
                writer.Write((short)-1);
                writer.Write((short)m_descriptor.BookmarkCount);
                writer.Write((short)10);

                for (int i = 0; i < values.Length; i++)
                {
                    writer.Write((int)0x1000000);
                    writer.Write(values[i]);
                    writer.Write((int)-1);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="writer"></param>
            /// <param name="keys"></param>
            /// <param name="values"></param>
            /// <param name="end"></param>
            private void WriteBKF(BinaryWriter writer, int[] keys, int[] values, int end)
            {
                int[] newKeys = new int[keys.Length];

                int index = 0;

                for (int i = 0; i < keys.Length; i++)
                {
                    newKeys[index] = m_keys.IndexOf(values[i]);
                    writer.Write(keys[i]);
                    index++;
                }

                writer.Write(end);

                for (int i = 0; i < keys.Length; i++)
                {
                    writer.Write(newKeys[i]);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="writer"></param>
            /// <param name="end"></param>
            private void WriteBKL(BinaryWriter writer, int end)
            {
                for (int i = 0; i < m_descriptor.BookmarkCount; i++)
                {
                    writer.Write(m_descriptor.GetEndPos(i));
                }

                writer.Write(end);
            }
            #endregion

            #region Internal declaration
            /// <summary>
            /// 
            /// </summary>
            internal class Comparer : IComparer
            {
                #region IComparer Members
                /// <summary>
                /// 
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <returns></returns>
                public int Compare(object x, object y)
                {
                    int xx = (int)x;
                    int yy = (int)y;

                    if (xx > yy)
                        return 1;
                    else
                        return (xx == yy) ? 0 : -1;
                }

                #endregion
            }
            #endregion
        }

        #endregion
    }
}