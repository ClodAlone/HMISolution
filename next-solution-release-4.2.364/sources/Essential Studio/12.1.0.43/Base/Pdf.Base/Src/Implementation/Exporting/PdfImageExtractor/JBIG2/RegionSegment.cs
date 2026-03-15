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

namespace Syncfusion.Pdf
{
    internal abstract class JBIG2BaseSegment: JBIG2Segment
    {
        protected internal int regionBitmapWidth, regionBitmapHeight;
        protected internal int regionBitmapXLocation, regionBitmapYLocation;
        protected internal RegionFlags regionFlags = new RegionFlags();

        private BitOperation m_bitOperation = new BitOperation();
        public JBIG2BaseSegment(JBIG2StreamDecoder streamDecoder)
            : base(streamDecoder)
        {
        }

        public override void readSegment()
        {
            short[] buff = new short[4];
            m_decoder.ReadByte(buff);
            regionBitmapWidth = m_bitOperation.GetInt32(buff);

            buff = new short[4];
            m_decoder.ReadByte(buff);
            regionBitmapHeight = m_bitOperation.GetInt32(buff);
            buff = new short[4];
            m_decoder.ReadByte(buff);
            regionBitmapXLocation = m_bitOperation.GetInt32(buff);

            buff = new short[4];
            m_decoder.ReadByte(buff);
            regionBitmapYLocation = m_bitOperation.GetInt32(buff);

            /// <summary>
            /// extract region Segment flags </summary>
            short regionFlagsField = m_decoder.ReadByte();

            regionFlags.setFlags(regionFlagsField);
        }
    }
    public class RegionFlags : JBIG2BaseFlags
    {
        public const String EXTERNAL_COMBINATION_OPERATOR = "EXTERNAL_COMBINATION_OPERATOR";

        public override void setFlags(int flagsAsInt)
        {
            this.flagsAsInt = flagsAsInt;

            /** extract EXTERNAL_COMBINATION_OPERATOR */
            flags.Add(EXTERNAL_COMBINATION_OPERATOR, flagsAsInt & 7);
        }
    }
}
