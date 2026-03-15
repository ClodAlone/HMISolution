#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
#if WINRT || WP
#else
using System.Drawing;
#endif
namespace Syncfusion.DocIO.DLS
{
    public class LineFormat
    {
        //        BackColor	Returns or sets a ColorFormat object that represents the specified fill background color.
        private Color m_BackColor;
        //beginarrowheadlength	returns or sets the length of the arrowhead at the beginning of the specified line. read/write msoarrowheadlength.
        private LineEndLength m_BeginArrowheadLength;
        ////beginarrowheadstyle	returns or sets the style of the arrowhead at the beginning of the specified line. read/write msoarrowheadstyle.
        private LineEnd m_BeginArrowheadStyle;
        ////beginarrowheadwidth	returns or sets the width of the arrowhead at the beginning of the specified line. read/write msoarrowheadwidth.
                private LineEndWidth m_BeginArrowheadWidth;
        //Creator	Returns a 32-bit integer that indicates the application in which this object was created. Read-only Long.
        //DashStyle	Returns or sets the dash style for the specified line. Can be one of the MsoLineDashStyle contants. Read/write Long.
        private LineDashing m_DashStyle;
        //EndArrowheadLength	Returns or sets the length of the arrowhead at the end of the specified line. Read/write MsoArrowheadLength.
                private LineEndLength m_EndArrowheadLength;
        ////EndArrowheadStyle	Returns or sets the style of the arrowhead at the end of the specified line. Read/write MsoArrowheadStyle.
                private LineEnd m_EndArrowheadStyle;
        ////EndArrowheadWidth	Returns or sets the width of the arrowhead at the end of the specified line. Read/write MsoArrowheadWidth.
                private LineEndWidth m_EndArrowheadWidth;
        //ForeColor	Returns or sets a ColorFormat object that represents the specified foreground fill or solid color.
        //        private Color m_ForeColor;
        ////InsetPen	Returns or sets whether lines are drawn inside the specified shape's boundaries. Read/write
        private bool m_InsetPen;
        ////Parent	Returns the parent object for the specified object. Read-only.
        ////Pattern	Returns or sets an MsoPatternType value that represents the fill pattern.
        //        private PatternType m_PatternType;
        ////Style	Returns or sets a MsoLineStyle value that represents the style of the line.
        private LineStyle m_Style;
        //Transparency	Returns or sets the degree of transparency of the specified fill as a value from 0.0 (opaque) through 1.0 (clear). Read/write Double.
        private float m_Transparency;
        //Visible	Returns or sets a MsoTriState value that determines whether the object is visible. Read/write.
        //private bool m_Visible;
        //Weight	Returns or sets a Single value that represents the weight of the line.
        private float m_Weight;
        private bool m_Line;
        private LineCap m_LineCap;
        private GradientFill m_GradientFill;
        private LineFormatType m_LineFormatType;
        private LineJoin m_LineJoin;
        private PatternType m_Pattern = PatternType.Mixed;
        private Color m_ForeColor;
        private ImageRecord m_ImageRecord;

        internal ImageRecord ImageRecord
        {
            get { return m_ImageRecord; }
            set { m_ImageRecord = value; }
        }

        internal Color ForeColor
        {
            get { return m_ForeColor; }
            set { m_ForeColor = value; }
        }
        internal PatternType Pattern
        {
            get { return m_Pattern; }
            set { m_Pattern = value; }
        }

        internal LineJoin LineJoin
        {
            get { return m_LineJoin; }
            set { m_LineJoin = value; }
        }
        internal LineFormatType LineFormatType
        {
            get { return m_LineFormatType; }
            set { m_LineFormatType = value; }
        }

        internal GradientFill GradientFill
        {
            get
            {
                if (m_GradientFill == null)
                    m_GradientFill = new GradientFill();
                return m_GradientFill;
            }
            set
            {
                m_GradientFill = value;
            }
        }
        internal LineCap LineCap
        {
            get { return m_LineCap; }
            set { m_LineCap = value; }
        }
        public bool Line
        {
            get { return m_Line; }
            set { m_Line = value; }
        }
        internal Dictionary<string, Stream> m_docxProps;
        internal Dictionary<string, Stream> DocxProps
        {
            get
            {
                if (m_docxProps == null)
                {
                    m_docxProps = new Dictionary<string, Stream>();
                }
                return m_docxProps;
            }
        }
        // BackColor	Returns or sets a ColorFormat object that represents the specified fill background color.
        public Color Color
        {
            get { return m_BackColor; }
            set { m_BackColor = value; }
        }
        //BeginArrowheadLength	Returns or sets the length of the arrowhead at the beginning of the specified line. Read/write MsoArrowheadLength.
        internal LineEndLength BeginArrowheadLength
        {
            get { return m_BeginArrowheadLength; }
            set { m_BeginArrowheadLength = value; }
        }
        ////BeginArrowheadStyle	Returns or sets the style of the arrowhead at the beginning of the specified line. Read/write MsoArrowheadStyle.
        internal LineEnd BeginArrowheadStyle
        {
            get { return m_BeginArrowheadStyle; }
            set { m_BeginArrowheadStyle = value; }
        }
        ////BeginArrowheadWidth	Returns or sets the width of the arrowhead at the beginning of the specified line. Read/write MsoArrowheadWidth.
        internal LineEndWidth BeginArrowheadWidth
        {
            get { return m_BeginArrowheadWidth; }
            set { m_BeginArrowheadWidth = value; }
        }
        //Creator	Returns a 32-bit integer that indicates the application in which this object was created. Read-only Long.
        //DashStyle	Returns or sets the dash style for the specified line. Can be one of the MsoLineDashStyle contants. Read/write Long.
        public LineDashing DashStyle
        {
            get { return m_DashStyle; }
            set { m_DashStyle = value; }
        }
        //EndArrowheadLength	Returns or sets the length of the arrowhead at the end of the specified line. Read/write MsoArrowheadLength.
        internal LineEndLength EndArrowheadLength
        {
            get { return m_EndArrowheadLength; }
            set { m_EndArrowheadLength = value; }
        }
        ////EndArrowheadStyle	Returns or sets the style of the arrowhead at the end of the specified line. Read/write MsoArrowheadStyle.
        internal LineEnd EndArrowheadStyle
        {
            get { return m_EndArrowheadStyle; }
            set { m_EndArrowheadStyle = value; }
        }
        ////EndArrowheadWidth	Returns or sets the width of the arrowhead at the end of the specified line. Read/write MsoArrowheadWidth.
        internal LineEndWidth EndArrowheadWidth
        {
            get { return m_EndArrowheadWidth; }
            set { m_EndArrowheadWidth = value; }
        }
        ////ForeColor	Returns or sets a ColorFormat object that represents the specified foreground fill or solid color.
        //        internal Color ForeColor
        //        {
        //            get { return m_ForeColor; }
        //            set { m_ForeColor = value; }
        //        }
        ////InsetPen	Returns or sets whether lines are drawn inside the specified shape's boundaries. Read/write
        internal bool InsetPen
        {
            get { return m_InsetPen; }
            set { m_InsetPen = value; }
        }
        //Parent	Returns the parent object for the specified object. Read-only.
        ////Pattern	Returns or sets an MsoPatternType value that represents the fill pattern.
        //        internal PatternType PatternType
        //        {
        //            get { return m_PatternType; }
        //            set { m_PatternType = value; }
        //        }
        //Style	Returns or sets a MsoLineStyle value that represents the style of the line.
        public LineStyle Style
        {
            get { return m_Style; }
            set { m_Style = value; }
        }
        //Transparency	Returns or sets the degree of transparency of the specified fill as a value from 0.0 (opaque) through 1.0 (clear). Read/write Double.
        public float Transparency
        {
            get { return m_Transparency; }
            set { m_Transparency = value; }
        }
        //Visible	Returns or sets a MsoTriState value that determines whether the object is visible. Read/write.
        //internal bool Visible
        //{
        //    get { return m_Visible; }
        //    set { m_Visible = value; }
        //}
        //Weight	Returns or sets a Single value that represents the weight of the line.
        public float Weight
        {
            get { return m_Weight; }
            set { m_Weight = value; }
        }
        private Shape m_shape;
        public LineFormat(Shape shape)
        {
            m_shape = shape;
            this.m_BackColor = Color.Black;
            this.m_DashStyle = LineDashing.Solid;
            this.m_Line = true;
            this.m_Style = LineStyle.Single;
            this.m_Transparency = 0f;
            this.m_Weight = 1;
            this.m_LineJoin = ReaderWriter.DataStreamParser.Escher.LineJoin.Miter;
            this.m_LineCap = ReaderWriter.DataStreamParser.Escher.LineCap.Flat;
            LineFormatChanged();
        }
        private void LineFormatChanged()
        {
            if (this.DocxProps.ContainsKey("gradFill"))
                this.DocxProps.Remove("gradFill");
            if (this.DocxProps.ContainsKey("pattFill"))
                this.DocxProps.Remove("pattFill");
            if (m_shape.Docx2007Props.ContainsKey("stroke"))
                m_shape.Docx2007Props.Remove("stroke");
            this.m_Line = true;
        }
    }
}
