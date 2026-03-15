#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Media;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Windows.PdfViewer
{
    /// <summary>
    /// Represents the annotation with associated within a page.
    /// </summary>
    class PageAnnotation
    {
        private RectangleF rect = new RectangleF();
        private string uri = string.Empty;
        private float border;
        private PdfArray pageAnnotDestinations;

        internal string URI
        {
            get { return uri; }
            set
            {
                uri = value;
            }
        }
        internal RectangleF Rect
        {
            get { return rect; }
            set
            {
                rect = value;
            }
        }
        internal float Border
        {
            get { return border; }
            set
            {
                border = value;
            }
        }
        internal PdfArray PageAnnotDestinations
        {
            get { return pageAnnotDestinations; }
            set
            {
                pageAnnotDestinations = value;
            }
        }
        internal PageAnnotation(RectangleF rec, string uri, float border, PdfArray pageAnnotDestinations)
        {
            this.rect = rec;
            this.uri = uri;
            this.border = border;
            this.pageAnnotDestinations = pageAnnotDestinations;
        }
        internal PageAnnotation(RectangleF rec, string uri, float border)
        {
            this.rect = rec;
            this.uri = uri;
            this.border = border;
        }
        
    }

    /// <summary>
    /// Represents the arguments associated with a HyperLinkClicked event.
    /// </summary>
    public class AnnotEventArgs : EventArgs
    {        
        private string uri;      
        public AnnotEventArgs(string uri)
        {
            this.uri = uri;
        }
        public string URI
        {
            get { return uri; }
        }
        
    }

    /// <summary>
    /// Represents the URLS within a page.
    /// </summary>
    class PageURL
    {        
        PointF scalingFactor;
        String uri;
        PointF currentLocation;
        float textElementWidth;
        float fontSize;

        internal PointF ScalingFactor
        {
            get { return scalingFactor; }
        }
        internal float FontSize
        {
            get { return fontSize; }
        }
        internal string URI
        {
            get { return uri; }
        }
        internal PointF CurrentLocation
        {
            get { return currentLocation; }
        }
        internal float TextElementWidth
        {
            get { return textElementWidth; }
        }
        public PageURL(String URI, PointF CurrentLocation, float TextElementWidth, float fontSize,PointF scalingFactor)
        {            
            this.uri = URI;
            this.currentLocation = CurrentLocation;
            this.textElementWidth = TextElementWidth;
            this.fontSize = fontSize;
            this.scalingFactor = scalingFactor;
        }
    }

    /// <summary>
    /// Represents the texts and positions of a page.
    /// </summary>
    class TextSearch
    {
        private PointF m_scalingFactor;
        private String m_text;
        private PointF m_currentLocation;
        private float m_textElementWidth;
        private float m_fontSize;
        private PIFont m_textFont;
        private FormattedText m_fText;
        private Typeface m_tFace;

        internal PointF ScalingFactor
        {
            get { return m_scalingFactor; }
        }
        internal float FontSize
        {
            get { return m_fontSize; }
        }
        internal string Text
        {
            get { return m_text; }
        }
        internal PointF CurrentLocation
        {
            get { return m_currentLocation; }
        }
        internal float TextElementWidth
        {
            get { return m_textElementWidth; }
        }
        internal PIFont TextFont
        {
            get { return m_textFont; }
        }
        internal FormattedText Ftext
        {
            get { return m_fText; }
        }
        internal Typeface TFace
        {
            get { return m_tFace; }
        }
        public TextSearch(String text, PointF CurrentLocation, float TextElementWidth, float fontSize, PointF scalingFactor, PIFont font, FormattedText Ftext, Typeface face)
        {
            m_text = text;
            m_currentLocation = CurrentLocation;
            m_textElementWidth = TextElementWidth;
            m_fontSize = fontSize;
            m_scalingFactor = scalingFactor;
            m_textFont = font;
            m_fText = Ftext;
            m_tFace = face;
        }
    }

    /// <summary>
    /// Represents the text matchs associated with a page.
    /// </summary>
    class TextMatchRectangle
    {
        private RectangleF m_rect = new RectangleF();
        private string m_text = string.Empty;


        internal string Text
        {
            get { return m_text; }
            set
            {
                m_text = value;
            }
        }
        internal RectangleF Rect
        {
            get { return m_rect; }
            set
            {
                m_rect = value;
            }
        }

        internal TextMatchRectangle(RectangleF rec, string text)
        {
            this.m_rect = rec;
            this.m_text = text;
        }
    }
}