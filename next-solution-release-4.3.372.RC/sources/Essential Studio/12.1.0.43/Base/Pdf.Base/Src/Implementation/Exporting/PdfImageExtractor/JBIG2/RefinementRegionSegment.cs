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
    class RefinementRegionSegment : JBIG2BaseSegment
    {
        private bool m_inlineImage;
        private int m_noOfReferedToSegments;
        private int[] m_referedToSegments;

        public RefinementRegionSegment(JBIG2StreamDecoder streamDecoder, bool inlineImage, int[] referedToSegments, int noOfReferedToSegments)
            : base(streamDecoder)
        {
            this.m_inlineImage = inlineImage;
            this.m_referedToSegments = referedToSegments;
            this.m_noOfReferedToSegments = noOfReferedToSegments;
        }
    }

    public class RefinementRegionFlags : JBIG2BaseFlags
    {
        public const string GR_TEMPLATE = "GR_TEMPLATE";
        public const string TPGDON = "TPGDON";

        public override void setFlags(int flagsAsInt)
        {
            this.flagsAsInt = flagsAsInt;

            /// <summary>
            /// extract GR_TEMPLATE </summary>
            flags.Add(GR_TEMPLATE, new int?(flagsAsInt & 1));

            /// <summary>
            /// extract TPGDON </summary>
            flags.Add(TPGDON, new int?((flagsAsInt >> 1) & 1));
        }
    }
}
