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
    class ExtensionSegment : JBIG2Segment
    {
        public ExtensionSegment(JBIG2StreamDecoder streamDecoder)
            : base(streamDecoder)
        {
        }

        public override void readSegment()
        {
            for (int i = 0; i < m_segmentHeader.DataLength; i++)
            {
                m_decoder.ReadByte();
            }
        }
    }
}
