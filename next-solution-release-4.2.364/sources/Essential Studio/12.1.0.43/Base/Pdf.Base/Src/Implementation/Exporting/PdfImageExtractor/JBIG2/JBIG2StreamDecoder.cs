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
using System.Collections;

namespace Syncfusion.Pdf
{
    internal class JBIG2StreamDecoder
    {
        #region Properties
        private ArithmeticDecoder m_arithmeticDecoder;
        private IList m_bitmaps = new List<object>();
        private byte[] m_globalData;
        private HuffmanDecoder m_huffmanDecoder;
        private MMRDecoder m_mmrDecoder;
        private int m_noOfPages = -1;
        private bool m_noOfPagesKnown;
        private bool m_randomAccessOrganisation;
        private Jbig2StreamReader m_reader;
        private IList m_segments = new List<object>();
        private BitOperation m_bitOperation = new BitOperation();
        #endregion

        internal ArithmeticDecoder ArithDecoder
        {
            get
            {
                return m_arithmeticDecoder;
            }
        }

        internal HuffmanDecoder HuffDecoder
        {
            get
            {
                return m_huffmanDecoder;
            }
        }

        internal MMRDecoder MmrDecoder
        {
            get
            {
                return m_mmrDecoder;
            }
        }

        // Methods
        internal void appendBitmap(JBIG2Image bitmap)
        {
            this.m_bitmaps.Add(bitmap);
        }

        private bool CheckHeader()
        {
            short[] objA = new short[] { 0x97, 0x4a, 0x42, 50, 13, 10, 0x1a, 10 };
            short[] buf = new short[8];
            this.m_reader.ReadByte(buf);
            return object.Equals(objA, buf);
        }

        internal void ConsumeRemainingBits()
        {
            this.m_reader.ConsumeRemainingBits();
        }

        internal void DecodeJBIG2(byte[] data)
        {
            this.m_reader = new Jbig2StreamReader(data);
            this.ResetDecoder();
            bool flag = this.CheckHeader();
            if (!flag)
            {
                this.m_noOfPagesKnown = true;
                this.m_randomAccessOrganisation = false;
                this.m_noOfPages = 1;
                if (this.m_globalData != null)
                {
                    this.m_reader = new Jbig2StreamReader(this.m_globalData);
                    this.m_huffmanDecoder = new HuffmanDecoder(this.m_reader);
                    this.m_mmrDecoder = new MMRDecoder(this.m_reader);
                    this.m_arithmeticDecoder = new ArithmeticDecoder(this.m_reader);
                    this.ReadSegments();
                    this.m_reader = new Jbig2StreamReader(data);
                }
                else
                {
                    this.m_reader.MovePointer(-8);
                }
            }
            else
            {
                this.SetFileHeaderFlags();
             
                if (this.m_noOfPagesKnown)
                {
                    this.m_noOfPages = this.m_noOfPages;
                }
            }
            this.m_huffmanDecoder = new HuffmanDecoder(this.m_reader);
            this.m_mmrDecoder = new MMRDecoder(this.m_reader);
            this.m_arithmeticDecoder = new ArithmeticDecoder(this.m_reader);
            this.ReadSegments();
        }

        internal JBIG2Image FindBitmap(int bitmapNumber)
        {
            IEnumerator enumerator = this.m_bitmaps.GetEnumerator();
            while (enumerator.MoveNext())
            {
                JBIG2Image current = (JBIG2Image)enumerator.Current;
                if (current.BitmapNumber == bitmapNumber)
                {
                    return current;
                }
            }
            return null;
        }

        internal PageInformationSegment FindPageSegement(int page)
        {
            IEnumerator enumerator = this.m_segments.GetEnumerator();
            while (enumerator.MoveNext())
            {
                JBIG2Segment current = (JBIG2Segment)enumerator.Current;
                SegmentHeader segmentHeader = current.m_segmentHeader;
                if ((segmentHeader.SegmentType == 0x30) && (segmentHeader.PageAssociation == page))
                {
                    return (PageInformationSegment)current;
                }
            }
            return null;
        }

        internal JBIG2Segment FindSegment(int segmentNumber)
        {
            IEnumerator enumerator = this.m_segments.GetEnumerator();
            while (enumerator.MoveNext())
            {
                JBIG2Segment current = (JBIG2Segment)enumerator.Current;
                if (current.m_segmentHeader.SegmentNumber == segmentNumber)
                {
                    return current;
                }
            }
            return null;
        }

        private int getnoOfPages()
        {
            short[] buf = new short[4];
            this.m_reader.ReadByte(buf);
            return m_bitOperation.GetInt32(buf);
        }

        internal JBIG2Image GetPageAsJBIG2Bitmap(int i)
        {
            return this.FindPageSegement(1).pageBitmap;
        }

        private void HandlePageAssociation(SegmentHeader segmentHeader)
        {
            int num;
            if (segmentHeader.IsPageAssociationSizeSet)
            {
                short[] buf = new short[4];
                this.m_reader.ReadByte(buf);
                num = m_bitOperation.GetInt32(buf);
            }
            else
            {
                num = this.m_reader.ReadByte();
            }
            segmentHeader.PageAssociation = num;
        }

        private void HandleReferedToSegmentNumbers(SegmentHeader segmentHeader)
        {
            int num = segmentHeader.ReferedToSegCount;
            int[] referredToSegments = new int[num];
            int num2 = segmentHeader.SegmentNumber;
            if (num2 <= 0x100)
            {
                for (int i = 0; i < num; i++)
                {
                    referredToSegments[i] = this.m_reader.ReadByte();
                }
            }
            else if (num2 <= 0x10000)
            {
                short[] buf = new short[2];
                for (int j = 0; j < num; j++)
                {
                    this.m_reader.ReadByte(buf);
                    referredToSegments[j] = m_bitOperation.GetInt16(buf);
                }
            }
            else
            {
                short[] numArray3 = new short[4];
                for (int k = 0; k < num; k++)
                {
                    this.m_reader.ReadByte(numArray3);
                    referredToSegments[k] = m_bitOperation.GetInt32(numArray3);
                }
            }
            segmentHeader.ReferredToSegments = referredToSegments;
        }

        private void HandleSegmentDataLength(SegmentHeader segmentHeader)
        {
            short[] buf = new short[4];
            this.m_reader.ReadByte(buf);
            int dataLength = m_bitOperation.GetInt32(buf);
            segmentHeader.DataLength = dataLength;
        }

        private void HandleSegmentHeaderFlags(SegmentHeader segmentHeader)
        {
            short segmentHeaderFlags = this.m_reader.ReadByte();
            segmentHeader.SetSegmentHeaderFlags(segmentHeaderFlags);
        }

        private void HandleSegmentNumber(SegmentHeader segmentHeader)
        {
            short[] buf = new short[4];
            this.m_reader.ReadByte(buf);
            int segmentNumber = m_bitOperation.GetInt32(buf);
            segmentHeader.SegmentNumber = segmentNumber;
        }

        private void HandleSegmentReferredToCountAndRententionFlags(SegmentHeader segmentHeader)
        {
            short num = this.m_reader.ReadByte();
            int referredToSegmentCount = (num & 0xe0) >> 5;
            short[] buf = null;
            short num3 = (short)(num & 0x1f);
            if (referredToSegmentCount <= 4)
            {
                buf = new short[] { num3 };
            }
            else if (referredToSegmentCount == 7)
            {
                short[] number = new short[4];
                number[0] = num3;
                for (int i = 1; i < 4; i++)
                {
                    number[i] = this.m_reader.ReadByte();
                }
                referredToSegmentCount = m_bitOperation.GetInt32(number);
                int num5 = (int)Math.Ceiling((double)(4.0 + (((double)(referredToSegmentCount + 1)) / 8.0)));
                int num6 = num5 - 4;
                buf = new short[num6];
                this.m_reader.ReadByte(buf);
            }
            segmentHeader.ReferedToSegCount = referredToSegmentCount;
            segmentHeader.RententionFlags = buf;
        }

        internal void MovePointer(int i)
        {
            this.m_reader.MovePointer(i);
        }

        internal int ReadBit()
        {
            return this.m_reader.ReadBit();
        }

        internal int ReadBits(int num)
        {
            return this.m_reader.ReadBits(num);
        }

        internal short ReadByte()
        {
            return this.m_reader.ReadByte();
        }

        internal void ReadByte(short[] buff)
        {
            this.m_reader.ReadByte(buff);
        }

        private void ReadSegmentHeader(SegmentHeader segmentHeader)
        {
            this.HandleSegmentNumber(segmentHeader);
            this.HandleSegmentHeaderFlags(segmentHeader);
            this.HandleSegmentReferredToCountAndRententionFlags(segmentHeader);
            this.HandleReferedToSegmentNumbers(segmentHeader);
            this.HandlePageAssociation(segmentHeader);
            if (segmentHeader.SegmentType != 0x33)
            {
                this.HandleSegmentDataLength(segmentHeader);
            }
        }

        private void ReadSegments()
        {
            bool flag = false;
            while (!this.m_reader.Getfinished() && !flag)
            {
                SegmentHeader segmentHeader = new SegmentHeader();
                
                this.ReadSegmentHeader(segmentHeader);
                JBIG2Segment segment = null;
                int num = segmentHeader.SegmentType;
                int[] referedToSegments = segmentHeader.ReferredToSegments;
                int noOfReferedToSegments = segmentHeader.ReferedToSegCount;
                switch (num)
                {
                    case 4:
                        segment = new TextRegionSegment(this, false);
                        segment.m_segmentHeader = segmentHeader;
                        break;

                    case 6:
                        segment = new TextRegionSegment(this, true);
                        segment.m_segmentHeader = segmentHeader;
                        break;

                    case 7:
                        segment = new TextRegionSegment(this, true);
                        segment.m_segmentHeader = segmentHeader;
                        break;

                    case 0x10:
                        segment = new PatternDictionarySegment(this);
                        segment.m_segmentHeader = segmentHeader;
                        break;

                    case 0:
                        segment = new SymbolDictionarySegment(this);
                        segment.m_segmentHeader = segmentHeader;
                        break;

                    case 20:
                        segment = new HalftoneRegionSegment(this, false);
                        segment.m_segmentHeader = segmentHeader;
                        break;

                    case 0x16:
                        segment = new HalftoneRegionSegment(this, true);
                        segment.m_segmentHeader = segmentHeader;
                        break;

                    case 0x17:
                        segment = new HalftoneRegionSegment(this, true);
                        segment.m_segmentHeader = segmentHeader;
                        break;

                    case 0x24:
                        segment = new GenericRegionSegment(this, false);
                        segment.m_segmentHeader = segmentHeader;
                        break;

                    case 0x26:
                        segment = new GenericRegionSegment(this, true);
                        segment.m_segmentHeader = segmentHeader;
                        break;

                    case 0x27:
                        segment = new GenericRegionSegment(this, true);
                        segment.m_segmentHeader = segmentHeader;
                        break;

                    case 40:
                        segment = new RefinementRegionSegment(this, false, referedToSegments, noOfReferedToSegments);
                        segment.m_segmentHeader = segmentHeader;
                        break;

                    case 0x2a:
                        segment = new RefinementRegionSegment(this, true, referedToSegments, noOfReferedToSegments);
                        segment.m_segmentHeader = segmentHeader;
                        break;

                    case 0x2b:
                        segment = new RefinementRegionSegment(this, true, referedToSegments, noOfReferedToSegments);
                        segment.m_segmentHeader = segmentHeader;
                        break;

                    case 0x30:
                        segment = new PageInformationSegment(this);
                        segment.m_segmentHeader = segmentHeader;
                        break;

                    case 0x31:
                        {
                            continue;
                        }
                    case 50:
                        segment = new EndOfStripeSegment(this);
                        segment.m_segmentHeader = segmentHeader;
                        break;

                    case 0x33:
                        {
                            flag = true;
                            continue;
                        }
                    case 0x34:
                    case 0x35:
                        break;

                    case 0x3e:
                        segment = new ExtensionSegment(this);
                        segment.m_segmentHeader = segmentHeader;
                        break;

                    default:
                        //Console.WriteLine("Unknown Segment type in JBIG2 stream");
                        break;
                }
                if (!this.m_randomAccessOrganisation && (segment != null))
                {
                    segment.readSegment();
                }
                this.m_segments.Add(segment);
            }
            if (this.m_randomAccessOrganisation)
            {
                IEnumerator enumerator = this.m_segments.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ((JBIG2Segment)enumerator.Current).readSegment();
                }
            }
        }

        private void ResetDecoder()
        {
            this.m_noOfPagesKnown = false;
            this.m_randomAccessOrganisation = false;
            this.m_noOfPages = -1;
            this.m_segments.Clear();
            this.m_bitmaps.Clear();
        }

        private void SetFileHeaderFlags()
        {
            short num = this.m_reader.ReadByte();
            if ((num & 0xfc) != 0)
            {
                //Console.WriteLine("Warning, reserved bits (2-7) of file header flags are not zero " + num);
            }
            int num2 = num & 1;
            this.m_randomAccessOrganisation = num2 == 0;
            int num3 = num & 2;
            this.m_noOfPagesKnown = num3 == 0;
        }

        // Properties
        internal IList AllSegments
        {
            get
            {
                return this.m_segments;
            }
        }

        internal byte[] GlobalData
        {
            set
            {
                this.m_globalData = value;
            }
        }

        internal int NumberOfPages
        {
            get
            {
                return this.m_noOfPages;
            }
        }

        internal bool NumberOfPagesKnown
        {
            get
            {
                return this.m_noOfPagesKnown;
            }
        }

        internal bool RandomAccessOrganisationUsed
        {
            get
            {
                return this.m_randomAccessOrganisation;
            }
        }
    }
}
