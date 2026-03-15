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
    class SegmentHeader
    {
        private int m_segmentNumber;
        private int m_segmentType;
        private bool m_pageAssociationSizeSet;
        private bool m_deferredNonRetainSet;
        private int m_referredToSegmentCount;
        private short[] m_rententionFlags;
        private int[] m_referredToSegments;
        private int m_pageAssociation;
        private int m_dataLength;

        internal int SegmentNumber
        {
            get
            {
                return m_segmentNumber;
            }
            set
            {
                m_segmentNumber = value;
            }
        }

        internal int SegmentType
        {
            get
            {
                return m_segmentType;
            }
            set
            {
                m_segmentType = value;
            }
        }

        internal bool IsPageAssociationSizeSet
        {
            get
            {
                return m_pageAssociationSizeSet;
            }
        }

        internal int ReferedToSegCount
        {
            get
            {
                return m_referredToSegmentCount;
            }
            set
            {
                m_referredToSegmentCount = value;
            }
        }

        internal int DataLength
        {
            get
            {
                return m_dataLength;
            }
            set
            {
                m_dataLength = value;
            }
        }

        internal int PageAssociation
        {
            get
            {
                return m_pageAssociation;
            }
            set
            {
                m_pageAssociation = value;
            }
        }

        internal short[] RententionFlags
        {
            get
            {
                return m_rententionFlags;
            }
            set
            {
                m_rententionFlags = value;
            }
        }

        internal bool IsDeferredNonRetainSet
        {
            get
            {
                return m_deferredNonRetainSet;
            }
        }

        internal int[] ReferredToSegments
        {
            get
            {
                return m_referredToSegments;
            }
            set
            {
                m_referredToSegments = value;
            }
        }
        public void SetSegmentHeaderFlags(short SegmentHeaderFlags)
        {
            m_segmentType = SegmentHeaderFlags & 63; // 63 = 00111111
            m_pageAssociationSizeSet = (SegmentHeaderFlags & 64) == 64; // 64 = // 01000000
            m_deferredNonRetainSet = (SegmentHeaderFlags & 80) == 80; // 64 = 10000000
        }
    }
}
