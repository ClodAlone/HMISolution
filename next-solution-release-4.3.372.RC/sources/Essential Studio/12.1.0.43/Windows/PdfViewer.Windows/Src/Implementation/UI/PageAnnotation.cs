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
using System.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Windows.Forms.PdfViewer
{
    /// <summary>
    /// Represents the annotation with associated within a page.
    /// </summary>
    class PageAnnotation
    {
        private RectangleF m_rect = new RectangleF();
        private string m_uri = string.Empty;
        private  float m_border=1;
        private string m_annotType;
        private PdfArray m_pageAnnotDestinations;

        public PageAnnotation(RectangleF rec, string uri, float border,string annotType)
        {
            m_rect = rec;
            m_uri = uri;
            m_border = border;
            m_annotType = annotType;
        }
        public PageAnnotation(RectangleF rec, string uri, float border, string annotType,PdfArray pageAnnotDestinations)
        {
            m_rect = rec;
            m_uri = uri;
            m_border = border;
            m_annotType = annotType;
            m_pageAnnotDestinations = pageAnnotDestinations;
        }
        internal RectangleF Rect
        {
            get { return m_rect; }
            set
            {
                m_rect = value;
            }
        }
        internal String URI
        {
            get { return m_uri; }
            set
            {
                m_uri = value;
            }
        }
        internal float Border
        {
            get { return m_border; }
            set
            {
                m_border = value;
            }
        }
        internal string AnnotType
        {
            get { return m_annotType; }
            set
            {
                m_annotType = value;
            }
        }
        internal PdfArray PageAnnotDestinations
        {
            get { return m_pageAnnotDestinations; }
            set
            {
                m_pageAnnotDestinations = value;
            }
        }
    }

    /// <summary>
    /// Represents the arguments associated with a HyperLinkClicked event.
    /// </summary>
    public class AnnotEventArgs : EventArgs
    {        
        private string m_uri;

        /// <summary>
        /// Returns the URI associated with the HyperLink.
        /// </summary>
        public string URI
        {
            get { return m_uri; }
        }

        /// <summary>
        /// Changes the URL.
        /// </summary>
        public AnnotEventArgs(string uri,Point location)
        {
            m_uri = uri;
        }
    }

    /// <summary>
    /// Represents the URLS within a page.
    /// </summary>
    class PageURL
    {
        private Matrix m_transformPoints;
        private String m_uri;
        private PointF m_currentLocation;
        private float m_textElementWidth;
        private float m_fontSize;

        internal float FontSize
        {
            get { return m_fontSize; }
        }
        internal string URI
        {
            get { return m_uri; }
        }
        internal PointF CurrentLocation
        {
            get { return m_currentLocation; }
        }
        internal Matrix TransformPoints
        {
            get { return m_transformPoints; }
        }
        internal float TextElementWidth
        {
            get { return m_textElementWidth; }
        }
        public PageURL(Matrix transformPoints,String URI,PointF CurrentLocation,float TextElementWidth, float fontSize)
        {
            m_transformPoints = transformPoints;
            m_uri = URI;
            m_currentLocation = CurrentLocation;
            m_textElementWidth = TextElementWidth;
            m_fontSize = fontSize;
        }
    }
    /// <summary>
    /// Represents the URLS within a page.
    /// </summary>
    class PageText
    {
        private Matrix m_transformPoints;
        private String m_txt;
        private PointF m_currentLocation;
        private float m_textElementWidth;
        private float m_fontSize;
        private Font m_textFont;

        internal float FontSize
        {
            get { return m_fontSize; }
        }
        internal string Text
        {
            get { return m_txt; }
        }
        internal PointF CurrentLocation
        {
            get { return m_currentLocation; }
        }
        internal Matrix TransformPoints
        {
            get { return m_transformPoints; }
        }
        internal float TextElementWidth
        {
            get { return m_textElementWidth; }
        }
        internal Font TextFont
        {
            get { return m_textFont; }
        }
        public PageText(Matrix transformPoints, String txt, PointF CurrentLocation, float TextElementWidth, float fontSize, Font font)
        {
            m_transformPoints = transformPoints;
            m_txt = txt;
            m_currentLocation = CurrentLocation;
            m_textElementWidth = TextElementWidth;
            m_fontSize = fontSize;
            m_textFont = font;
        }
    }

    /// <summary>
    /// Represents the Rectangle position of the matching text.
    /// </summary>
    class TextMatchRectangle
    {
        private RectangleF m_rect = new RectangleF();
        private string m_text = string.Empty;
        private float m_textWidth;
        private float m_scaleX;
        private Font m_textFont;

        public TextMatchRectangle(RectangleF rec, string txt, float txtWidth, float scaleX, Font font)
        {
            m_rect = rec;
            m_text = txt;
            m_textWidth = txtWidth;
            m_scaleX = scaleX;
            m_textFont = font;
        }

        internal RectangleF Rect
        {
            get { return m_rect; }
            set
            {
                m_rect = value;
            }
        }
        internal String Text
        {
            get { return m_text; }
            set
            {
                m_text = value;
            }
        }
        internal float TextWidth
        {
            get { return m_textWidth; }
            set
            {
                m_textWidth = value;
            }
        }
        internal float ScaleX
        {
            get { return m_scaleX; }
            set
            {
                m_scaleX = value;
            }
        }

        internal Font TextFont
        {
            get { return m_textFont; }
            set
            {
                m_textFont = value;
            }
        }
    }

}
