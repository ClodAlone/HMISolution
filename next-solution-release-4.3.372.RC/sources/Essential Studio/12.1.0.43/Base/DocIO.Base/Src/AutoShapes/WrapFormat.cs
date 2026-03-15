#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.DocIO.DLS;
using System;
using System.Collections.Generic;
using System.Text;
#if !WINRT && !WP
using System.Drawing;
#endif

namespace Syncfusion.DocIO.DLS
{
    public class WrapFormat
    {
        //AllowOverlap	Returns or sets a value that specifies whether a given shape can overlap other shapes. Read/write Long.
        private bool m_AllowOverlap;
        //Application	Returns an Application object that represents the Microsoft Word application.
        //Creator	Returns a 32-bit integer that indicates the application in which the specified object was created. Read-only Long.
        //DistanceBottom	Returns or sets the distance (in points) between the document text and the bottom edge of the text-free area surrounding the specified shape. Read/write Single.
        private float m_DistanceBottom;
        //DistanceLeft	Returns or sets the distance (in points) between the document text and the left edge of the text-free area surrounding the specified shape. Read/write Single.
        private float m_DistanceLeft;
        //DistanceRight	Returns or sets the distance (in points) between the document text and the right edge of the text-free area surrounding the specified shape. Read/write Single.
        private float m_DistanceRight;
        //DistanceTop	Returns or sets the distance (in points) between the document text and the top edge of the text-free area surrounding the specified shape. Read/write Single.
        private float m_DistanceTop;
        //Parent	Returns an Object that represents the parent object of the specified WrapFormat object.
        //Side	Returns or sets a value that indicates whether the document text should wrap on both sides of the specified shape, on either the left or right side only, or on the side of the shape that's farthest from the page margin.Read/write WdWrapSideType.
        private TextWrappingType m_TextWrappingType;
        //Type	Returns the wrap type for the specified shape. Read/write WdWrapType.
        private TextWrappingStyle m_TextWrappingStyle;
        private WrapPolygon m_wrapPolygon;
        //indicate whether current wrapping bounds points added to the list or not. 
        internal bool IsWrappingBoundsAdded = false;
        //hold the wrapping bounds index.
        internal int WrapCollectionIndex = -1;

        //AllowOverlap	Returns or sets a value that specifies whether a given shape can overlap other shapes. Read/write Long.
        public bool AllowOverlap
        {
            get { return m_AllowOverlap; }
            set { m_AllowOverlap = value; }
        }
        //Application	Returns an Application object that represents the Microsoft Word application.
        //Creator	Returns a 32-bit integer that indicates the application in which the specified object was created. Read-only Long.
        //DistanceBottom	Returns or sets the distance (in points) between the document text and the bottom edge of the text-free area surrounding the specified shape. Read/write Single.
        public float DistanceBottom
        {
            get { return m_DistanceBottom; }
            set { m_DistanceBottom = value; }
        }
        //DistanceLeft	Returns or sets the distance (in points) between the document text and the left edge of the text-free area surrounding the specified shape. Read/write Single.
        public float DistanceLeft
        {
            get { return m_DistanceLeft; }
            set { m_DistanceLeft = value; }
        }
        //DistanceRight	Returns or sets the distance (in points) between the document text and the right edge of the text-free area surrounding the specified shape. Read/write Single.
        public float DistanceRight
        {
            get { return m_DistanceRight; }
            set { m_DistanceRight = value; }
        }
        //DistanceTop	Returns or sets the distance (in points) between the document text and the top edge of the text-free area surrounding the specified shape. Read/write Single.
        public float DistanceTop
        {
            get { return m_DistanceTop; }
            set { m_DistanceTop = value; }
        }
        //Parent	Returns an Object that represents the parent object of the specified WrapFormat object.
        //Side	Returns or sets a value that indicates whether the document text should wrap on both sides of the specified shape, on either the left or right side only, or on the side of the shape that's farthest from the page margin.Read/write WdWrapSideType.
        public TextWrappingType TextWrappingType
        {
            get { return m_TextWrappingType; }
            set { m_TextWrappingType = value; }
        }
        //Type	Returns the wrap type for the specified shape. Read/write WdWrapType.
        public TextWrappingStyle TextWrappingStyle
        {
            get { return m_TextWrappingStyle; }
            set { m_TextWrappingStyle = value; }
        }

        /// <summary>
        /// Gets or sets the wrap polygon.
        /// </summary>
        /// <value>
        /// The wrap polygon.
        /// </value>
        internal WrapPolygon WrapPolygon
        {
            get 
            {
                if (m_wrapPolygon == null)
                {
                    m_wrapPolygon = new WrapPolygon();
                    m_wrapPolygon.Edited = false;
                    //Handled to add default wrap polygon veritces
                    m_wrapPolygon.Vertices.Add(new PointF(0, 0));
                    m_wrapPolygon.Vertices.Add(new PointF(0, 21600));
                    m_wrapPolygon.Vertices.Add(new PointF(21600, 21600));
                    m_wrapPolygon.Vertices.Add(new PointF(21600, 0));
                    m_wrapPolygon.Vertices.Add(new PointF(0, 0));
                }
                return m_wrapPolygon;
            }
            set { m_wrapPolygon = value; }
        }

    }
}
