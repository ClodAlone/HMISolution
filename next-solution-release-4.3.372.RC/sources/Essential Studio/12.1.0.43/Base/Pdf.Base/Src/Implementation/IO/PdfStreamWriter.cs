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

using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using Syncfusion.Pdf.ColorSpace;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.IO
{
    /// <summary>
    /// Helper class to write PDF graphic streams easily.
    /// </summary>
    internal class PdfStreamWriter : IPdfWriter
    {
        #region Fields
        /// <summary>
        /// The PDF stream where the data should be write into.
        /// </summary>
        private PdfStream m_stream;
        #endregion

        #region Initialize / Finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PDFStreamWriter"/> class.
        /// </summary>
        /// <param name="stream">The PDF stream.</param>
        public PdfStreamWriter(PdfStream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            m_stream = stream;
        }
        #endregion

        #region Public methods

        #region Text primitives
        /// <summary>
        /// Modifies TM (text matrix).
        /// </summary>
        /// <param name="matrix">Matrix to be inserted.</param>
        public void ModifyTM(PdfTransformationMatrix matrix)
        {
            m_stream.Write(matrix.ToString());
            m_stream.Write(Operators.WhiteSpace);
            WriteOperator(Operators.ModifyTM);
        }

        /// <summary>
        /// Sets font.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="name">The name of the font.</param>
        /// <param name="size">Font size.</param>
        public void SetFont(PdfFont font, string name, float size)
        {
            SetFont(font, new PdfName(name), size);
        }
        /// <summary>
        /// Sets font.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="name">The name of the font.</param>
        /// <param name="size">Font size.</param>
        public void SetFont(PdfFont font, PdfName name, float size)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            m_stream.Write(name.ToString());
            m_stream.Write(Operators.WhiteSpace);
            m_stream.Write(PdfNumber.FloatToString(size));
            m_stream.Write(Operators.WhiteSpace);
            WriteOperator(Operators.SetFont);
        }

        /// <summary>
        /// Set the Colorspace.
        /// </summary>
        /// <param name="colorspace"></param>
        /// <param name="name"></param>
        public void SetColorSpace(PdfColorSpaces colorspace, PdfName name)
        {
            if (colorspace == null)
                throw new ArgumentNullException("Color Space");
            m_stream.Write(name.ToString());
            m_stream.Write(Operators.WhiteSpace);
            WriteOperator(Operators.SelectColorSpaceForStroking);
            m_stream.Write(name.ToString());
            m_stream.Write(Operators.WhiteSpace);
            WriteOperator(Operators.SelectColorSpaceForNonStroking);
        }

        /// <summary>
        /// Sets the character spacing.
        /// </summary>
        /// <param name="charSpacing">The character spacing value.</param>
        public void SetCharacterSpacing(float charSpacing)
        {
            m_stream.Write(PdfNumber.FloatToString(charSpacing));
            m_stream.Write(Operators.WhiteSpace);
            m_stream.Write(Operators.SetCharacterSpace);
            m_stream.Write(Operators.NewLine);
        }
        /// <summary>
        /// Sets the word spacing.
        /// </summary>
        /// <param name="wordSpacing">The word spacing value.</param>
        public void SetWordSpacing(float wordSpacing)
        {
            m_stream.Write(PdfNumber.FloatToString(wordSpacing));
            m_stream.Write(Operators.WhiteSpace);
            WriteOperator(Operators.SetWordSpace);
        }
        /// <summary>
        /// Sets the scaling.
        /// </summary>
        /// <param name="scalingFactor">The scaling factor.</param>
        /// <remarks>The scaling factor of 100 means "normal size".</remarks>
        public void SetHorizontalScaling(float scalingFactor)
        {
            m_stream.Write(PdfNumber.FloatToString(scalingFactor));
            m_stream.Write(Operators.WhiteSpace);
            WriteOperator(Operators.SetHorizontalScaling);
        }
        /// <summary>
        /// Sets text leading.
        /// </summary>
        /// <param name="leading">The leading value.</param>
        public void SetLeading(float leading)
        {
            m_stream.Write(PdfNumber.FloatToString(leading));
            m_stream.Write(Operators.WhiteSpace);
            WriteOperator(Operators.SetTextLeading);
        }
        /// <summary>
        /// Sets the text rendering mode.
        /// </summary>
        /// <param name="renderingMode">The rendering mode.</param>
        public void SetTextRenderingMode(TextRenderingMode renderingMode)
        {
            m_stream.Write(((int)renderingMode).ToString());
            m_stream.Write(Operators.WhiteSpace);
            WriteOperator(Operators.SetRenderingMode);
        }
        /// <summary>
        /// Sets text rise.
        /// </summary>
        /// <param name="rise">The text rise value.</param>
        public void SetTextRise(float rise)
        {
            m_stream.Write(PdfNumber.FloatToString(rise));
            m_stream.Write(Operators.WhiteSpace);
            WriteOperator(Operators.SetTextRise);
        }

        /// <summary>
        /// Sets the text scaling.
        /// </summary>
        /// <param name="textScaling">The text scaling.</param>
        internal void SetTextScaling(float textScaling)
        {
            m_stream.Write(PdfNumber.FloatToString(textScaling));
            m_stream.Write(Operators.WhiteSpace);
            WriteOperator(Operators.SetTextScaling);
        }

        /// <summary>
        /// Starts the next line.
        /// </summary>
        public void StartNextLine()
        {
            WriteOperator(Operators.GoToNextLine);
        }
        /// <summary>
        /// Starts the next line.
        /// </summary>
        /// <param name="point">The start point of the line.</param>
        public void StartNextLine(PointF point)
        {
            WritePoint(point);
            WriteOperator(Operators.SetCoords);
        }
        /// <summary>
        /// Shifts the text to the point.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public void StartNextLine(float x, float y)
        {
            WritePoint(x, y);
            WriteOperator(Operators.SetCoords);
        }
        /// <summary>
        /// Sets the start of the next line and set leading.
        /// </summary>
        /// <param name="point">The point.</param>
        public void StartLineAndSetLeading(PointF point)
        {
            WritePoint(point);
            WriteOperator(Operators.SetCoordsAndLeading);
        }
        /// <summary>
        /// Sets the start of the next line and set leading.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void StartLineAndSetLeading(float x, float y)
        {
            WritePoint(x, y);
            WriteOperator(Operators.SetCoordsAndLeading);
        }

        /// <summary>
        /// Shows the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="hex">if set to <c>true</c> the text is in hex.</param>
        public void ShowText(byte[] text, bool hex)
        {
            CheckTextParam(text);
            WriteText(text, hex);
            WriteOperator(Operators.SetText);
        }
        /// <summary>
        /// Shows the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="hex">if set to <c>true</c> the text should be in hex.</param>
        public void ShowText(string text, bool hex)
        {
            CheckTextParam(text);
            WriteText(text, hex);
            WriteOperator(Operators.SetText);
        }
        /// <summary>
        /// Shows the text.
        /// </summary>
        /// <param name="text">The text.</param>
        public void ShowText(PdfString text)
        {
            CheckTextParam(text);
            WriteText(text);
            WriteOperator(Operators.SetText);
        }
        /// <summary>
        /// Flushes tokens to the stream.
        /// </summary>
        /// <param name="formattedText">String tokens.</param>
        public void ShowText(PdfArray formattedText)
        {
            if (formattedText == null)
                throw new ArgumentNullException("formattedText");

            formattedText.Save(this);
            WriteOperator(Operators.SetTextWithFormatting);
        }
        /// <summary>
        /// Shows the next line text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="hex">if set to <c>true</c> the text should be in hex.</param>
        public void ShowNextLineText(byte[] text, bool hex)
        {
            CheckTextParam(text);
            WriteText(text, hex);
            WriteOperator(Operators.SetTextOnNewLine);
        }
        /// <summary>
        /// Shows the next line text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="hex">if set to <c>true</c> the text should be in hex.</param>
        public void ShowNextLineText(string text, bool hex)
        {
            CheckTextParam(text);
            WriteText(text, hex);
            WriteOperator(Operators.SetTextOnNewLine);
        }
        /// <summary>
        /// Shows the next line text.
        /// </summary>
        /// <param name="text">The text.</param>
        public void ShowNextLineText(PdfString text)
        {
            CheckTextParam(text);
            WriteText(text);
            WriteOperator(Operators.SetTextOnNewLine);
        }
        /// <summary>
        /// Shows the text on the next line and sets word and character spacings.
        /// </summary>
        /// <param name="wordSpacing">The word spacing.</param>
        /// <param name="charSpacing">The char spacing.</param>
        /// <param name="text">The text.</param>
        /// <param name="hex">if set to <c>true</c> the text should be in hex.</param>
        public void ShowNextLineTextWithSpacings(float wordSpacing, float charSpacing, byte[] text, bool hex)
        {
            CheckTextParam(text);
            WritePoint(wordSpacing, charSpacing);
            WriteText(text, hex);
            WriteOperator(Operators.SetTextOnNewLineWithSpacings);
        }
        /// <summary>
        /// Shows the text on the next line and sets word and character spacings.
        /// </summary>
        /// <param name="wordSpacing">The word spacing.</param>
        /// <param name="charSpacing">The char spacing.</param>
        /// <param name="text">The text.</param>
        /// <param name="hex">if set to <c>true</c> the text should be in hex.</param>
        public void ShowNextLineTextWithSpacings(float wordSpacing, float charSpacing, string text, bool hex)
        {
            CheckTextParam(text);
            WritePoint(wordSpacing, charSpacing);
            WriteText(text, hex);
            WriteOperator(Operators.SetTextOnNewLineWithSpacings);
        }
        /// <summary>
        /// Shows the text on the next line and sets word and character spacings.
        /// </summary>
        /// <param name="wordSpacing">The word spacing.</param>
        /// <param name="charSpacing">The char spacing.</param>
        /// <param name="text">The text.</param>
        public void ShowNextLineTextWithSpacings(float wordSpacing, float charSpacing, PdfString text)
        {
            CheckTextParam(text);
            WritePoint(wordSpacing, charSpacing);
            WriteText(text);
            WriteOperator(Operators.SetTextOnNewLineWithSpacings);
        }
        /// <summary>
        /// Shows the text.
        /// </summary>
        /// <param name="formatting">The formatting.</param>
        public void ShowText(IList formatting)
        {
            if (formatting == null)
                throw new ArgumentNullException("formatting");

            m_stream.Write(PdfArray.StartMark);

            foreach (object obj in formatting)
            {
                if (obj == null)
                    throw new ArgumentException("Invalid formatting", "formatting");

                if (obj is PdfNumber)
                {
                    m_stream.Write((obj as PdfNumber).IntValue.ToString());
                    m_stream.Write(Operators.WhiteSpace);
                }
                else if (obj is int)
                {
                    m_stream.Write(obj.ToString());
                    m_stream.Write(Operators.WhiteSpace);
                }
                else
                {
                    WriteText(obj);
                }
            }
            m_stream.Write(PdfArray.EndMark);
            WriteOperator(Operators.SetTextWithFormatting);
        }

        /// <summary>
        /// Begins text.
        /// </summary>
        public void BeginText()
        {
            WriteOperator(Operators.BeginText);
        }

        /// <summary>
        /// Writes tags.
        /// </summary>
        /// <param name="tag"></param>
        internal void WriteTag(string tag)
        {
            m_stream.Write(tag);
            m_stream.Write(Operators.NewLine);
        }
        /// <summary>
        /// Ends text.
        /// </summary>
        public void EndText()
        {
            WriteOperator(Operators.EndText);
        }

        /// <summary>
        /// Begins start markup sequence text.
        /// </summary>
        /// <param name="name">The name of the markup sequence.</param>
        public void BeginMarkupSequence(string name)
        {
            if (name == null || name == string.Empty)
                throw new ArgumentNullException("name");

            m_stream.Write(Operators.Slash);
            m_stream.Write(name);
            m_stream.Write(Operators.WhiteSpace);
            WriteOperator(Operators.BeginMarkedSequence);
        }
        /// <summary>
        /// Begins start markup sequence text.
        /// </summary>
        /// <param name="name">The name of the markup sequence.</param>
        public void BeginMarkupSequence(PdfName name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            m_stream.Write(name.ToString());
            m_stream.Write(Operators.WhiteSpace);
            WriteOperator(Operators.BeginMarkedSequence);
        }
        /// <summary>
        /// Ends markup sequence text.
        /// </summary>
        public void EndMarkupSequence()
        {
            WriteOperator(Operators.EndMarkedSequence);
        }

        /// <summary>
        /// Writes comment to the file.
        /// </summary>
        /// <param name="comment"></param>
        public void WriteComment(string comment)
        {
            if (comment != null && comment.Length > 0)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(Operators.Comment);
                builder.Append(Operators.WhiteSpace);
                builder.Append(comment);
                //builder.Append( Operators.NewLine );

                WriteOperator(builder.ToString());
            }
        }
        #endregion Text primitives

        #region Path primitives
        /// <summary>
        /// Begins the path.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        public void BeginPath(PointF startPoint)
        {
            BeginPath(startPoint.X, startPoint.Y);
        }
        /// <summary>
        /// Begins the path.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void BeginPath(float x, float y)
        {
            WritePoint(x, y);
            WriteOperator(Operators.BeginPath);
        }

        /// <summary>
        /// Appends bezier segment.
        /// </summary>
        public void AppendBezierSegment(PointF p1, PointF p2, PointF p3)
        {
            AppendBezierSegment(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y);
        }
        /// <summary>
        /// Appends bezier segment.
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <param name="x3">The x3.</param>
        /// <param name="y3">The y3.</param>
        public void AppendBezierSegment(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            WritePoint(x1, y1);
            WritePoint(x2, y2);
            WritePoint(x3, y3);

            WriteOperator(Operators.AppendBezierCurve);
        }
        /// <summary>
        /// Appends the bezier segment.
        /// </summary>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <param name="useFirstPoint">if set to <c>true</c> p2 is used as p1.</param>
        public void AppendBezierSegment(PointF p2, PointF p3, bool useFirstPoint)
        {
            AppendBezierSegment(p2.X, p2.Y, p3.X, p3.Y, useFirstPoint);
        }
        /// <summary>
        /// Appends the bezier segment.
        /// </summary>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <param name="x3">The x3.</param>
        /// <param name="y3">The y3.</param>
        /// <param name="useFirstPoint">if set to <c>true</c>
        /// x2 is used as x1 and y2 is used as y1.</param>
        public void AppendBezierSegment(float x2, float y2, float x3, float y3, bool useFirstPoint)
        {
            WritePoint(x2, y2);
            WritePoint(x3, y3);

            if (useFirstPoint)
            {
                WriteOperator(Operators.AppendBezierCurve1);
            }
            else
            {
                WriteOperator(Operators.AppendBezierCurve2);
            }
        }

        /// <summary>
        /// Appends a line segment.
        /// </summary>
        /// <param name="point">The point.</param>
        public void AppendLineSegment(PointF point)
        {
            AppendLineSegment(point.X, point.Y);
        }
        /// <summary>
        /// Appends a line segment.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void AppendLineSegment(float x, float y)
        {
            WritePoint(x, y);

            WriteOperator(Operators.AppendLineSegment);
        }

        /// <summary>
        /// Appends the rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        public void AppendRectangle(RectangleF rect)
        {
            AppendRectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }
        /// <summary>
        /// Appends the rectangle.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void AppendRectangle(float x, float y, float width, float height)
        {
            WritePoint(x, y);
            WritePoint(width, height);
            WriteOperator(Operators.AppendRectangle);
        }

        /// <summary>
        /// Closes path object.
        /// </summary>
        public void ClosePath()
        {
            WriteOperator(Operators.ClosePath);
        }
        /// <summary>
        /// Closes the sub path.
        /// </summary>
        public void CloseSubPath()
        {
            WriteOperator(Operators.ClosePath);
        }
        /// <summary>
        /// Fills path.
        /// </summary>
        /// <param name="useEvenOddRule">if set to <c>true</c> use even-odd rule.</param>
        public void FillPath(bool useEvenOddRule)
        {
            m_stream.Write(Operators.Fill);

            if (useEvenOddRule)
            {
                m_stream.Write(Operators.EvenOdd);
            }

            m_stream.Write(Operators.NewLine);
        }
        /// <summary>
        /// Strokes path.
        /// </summary>
        public void StrokePath()
        {
            WriteOperator(Operators.Stroke);
        }
        /// <summary>
        /// Fills and strokes path.
        /// </summary>
        /// <param name="useEvenOddRule">if set to <c>true</c> use even-odd rule.</param>
        public void FillStrokePath(bool useEvenOddRule)
        {
            m_stream.Write(Operators.FillStroke);

            if (useEvenOddRule)
            {
                m_stream.Write(Operators.EvenOdd);
            }

            m_stream.Write(Operators.NewLine);
        }
        /// <summary>
        /// Closes and strokes the path.
        /// </summary>
        public void CloseStrokePath()
        {
            WriteOperator(Operators.CloseStrokePath);
        }
        /// <summary>
        /// Closes, then fills and strokes the path.
        /// </summary>
        /// <param name="useEvenOddRule">if set to <c>true</c> use even odd rule.</param>
        public void CloseFillStrokePath(bool useEvenOddRule)
        {
            m_stream.Write(Operators.CloseFillStrokePath);

            if (useEvenOddRule)
            {
                m_stream.Write(Operators.EvenOdd);
            }

            m_stream.Write(Operators.NewLine);
        }


        /// <summary>
        /// Closes and fills the path.
        /// </summary>
        /// <param name="useEvenOddRule">if set to <c>true</c> [use even odd rule].</param>
        public void CloseFillPath(bool useEvenOddRule)
        {
            WriteOperator(Operators.ClosePath);
            m_stream.Write(Operators.Fill);

            if (useEvenOddRule)
            {
                m_stream.Write(Operators.EvenOdd);
            }

            m_stream.Write(Operators.NewLine);

        }

        /// <summary>
        /// Clips the path.
        /// </summary>
        /// <param name="useEvenOddRule">if set to <c>true</c> use even odd rule.</param>
        public void ClipPath(bool useEvenOddRule)
        {
            m_stream.Write(Operators.ClipPath);

            if (useEvenOddRule)
                m_stream.Write(Operators.EvenOdd);

            m_stream.Write(Operators.WhiteSpace);
            m_stream.Write(Operators.EndPath);
            m_stream.Write(Operators.NewLine);
        }

        /// <summary>
        /// Ends the path.
        /// </summary>
        public void EndPath()
        {
            WriteOperator(Operators.n);
        }
        #endregion Path primitives

        #region Graphics state primitives
        /// <summary>
        /// Saves the graphics state.
        /// </summary>
        public void SaveGraphicsState()
        {
            WriteOperator(Operators.SaveState);
        }
        /// <summary>
        /// Restores the graphics state.
        /// </summary>
        public void RestoreGraphicsState()
        {
            WriteOperator(Operators.RestoreState);
        }
        /// <summary>
        /// Modifies current transformation matrix.
        /// </summary>
        /// <param name="matrix">Matrix to be inserted.</param>
        public void ModifyCTM(PdfTransformationMatrix matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            m_stream.Write(matrix.ToString());
            m_stream.Write(Operators.WhiteSpace);
            WriteOperator(Operators.ModifyCTM);
        }
        /// <summary>
        /// Sets the width of the line.
        /// </summary>
        /// <param name="width">The width.</param>
        public void SetLineWidth(float width)
        {
            m_stream.Write(PdfNumber.FloatToString(width));
            m_stream.Write(Operators.WhiteSpace);
            WriteOperator(Operators.SetLineWidth);
        }
        /// <summary>
        /// Sets the line cap.
        /// </summary>
        /// <param name="lineCapStyle">The line cap style.</param>
        public void SetLineCap(PdfLineCap lineCapStyle)
        {
            m_stream.Write(((int)lineCapStyle).ToString());
            m_stream.Write(Operators.WhiteSpace);
            WriteOperator(Operators.SetLineCapStyle);
        }
        /// <summary>
        /// Sets the line join.
        /// </summary>
        /// <param name="lineJoinStyle">The line join style.</param>
        public void SetLineJoin(PdfLineJoin lineJoinStyle)
        {
            m_stream.Write(((int)lineJoinStyle).ToString());
            m_stream.Write(Operators.WhiteSpace);
            WriteOperator(Operators.SetLineJoinStyle);
        }
        /// <summary>
        /// Sets the miter limit.
        /// </summary>
        /// <param name="miterLimit">The miter limit.</param>
        public void SetMiterLimit(float miterLimit)
        {
            m_stream.Write(PdfNumber.FloatToString(miterLimit));
            m_stream.Write(Operators.WhiteSpace);
            WriteOperator(Operators.SetMiterLimit);
        }
        /// <summary>
        /// Sets the line dash pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="patternOffset">The pattern offset.</param>
        public void SetLineDashPattern(float[] pattern, float patternOffset)
        {
            PdfArray pat = new PdfArray(pattern);
            PdfNumber off = new PdfNumber(patternOffset);
            SetLineDashPattern(pat, off);
        }
        /// <summary>
        /// Sets the line dash pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="patternOffset">The pattern offset.</param>
        private void SetLineDashPattern(PdfArray pattern, PdfNumber patternOffset)
        {
            pattern.Save(this);
            m_stream.Write(Operators.WhiteSpace);
            patternOffset.Save(this);
            m_stream.Write(Operators.WhiteSpace);
            WriteOperator(Operators.SetDashPattern);
        }

        /// <summary>
        /// Sets the color rendering intent.
        /// </summary>
        /// <param name="intent">The intent value.</param>
        public void SetColorRenderingIntent(ColorIntent intent)
        {
            m_stream.Write(Operators.Slash);
            m_stream.Write(intent.ToString());
            m_stream.Write(Operators.WhiteSpace);
            WriteOperator(Operators.SetColorRenderingIntent);
        }
        /// <summary>
        /// Sets the flatness tolerance.
        /// </summary>
        /// <param name="tolerance">The tolerance value.</param>
        public void SetFlatnessTolerance(int tolerance)
        {
            m_stream.Write(PdfNumber.FloatToString((float)tolerance));
            m_stream.Write(Operators.WhiteSpace);
            WriteOperator(Operators.SetFlatnessTolerance);
        }
        /// <summary>
        /// Sets the graphics state.
        /// </summary>
        /// <param name="dictionaryName">Name of the graphics state dictionary.</param>
        public void SetGraphicsState(PdfName dictionaryName)
        {
            if (dictionaryName == null || dictionaryName == string.Empty)
                throw new ArgumentNullException("dictionaryName");

            m_stream.Write(dictionaryName.ToString());
            m_stream.Write(Operators.WhiteSpace);
            WriteOperator(Operators.SetGraphicsState);
        }
        /// <summary>
        /// Sets the graphics state.
        /// </summary>
        /// <param name="dictionaryName">Name of the graphics state dictionary.</param>
        public void SetGraphicsState(string dictionaryName)
        {
            if (dictionaryName == null || dictionaryName == string.Empty)
                throw new ArgumentNullException("dictionaryName");

            m_stream.Write(Operators.Slash);
            m_stream.Write(dictionaryName);
            m_stream.Write(Operators.WhiteSpace);
            WriteOperator(Operators.SetGraphicsState);
        }
        #endregion Graphics state primitives

        #region Colour priitives
        /// <summary>
        /// Sets the color space.
        /// </summary>
        /// <param name="name">The name of the colour space.</param>
        /// <param name="forStroking">if set to <c>true</c>
        /// the space is set for stroking operations.</param>
        public void SetColorSpace(string name, bool forStroking)
        {
            SetColorSpace(new PdfName(name), forStroking);
        }

        /// <summary>
        /// Sets the color space.
        /// </summary>
        /// <param name="name">The name of the colour space.</param>
        /// <param name="forStroking">if set to <c>true</c>
        /// the space is set for stroking operations.</param>
        public void SetColorSpace(PdfName name, bool forStroking)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            string op = (forStroking) ? Operators.SelectColorSpaceForStroking :
                Operators.SelectColorSpaceForNonStroking;

            m_stream.Write(name.ToString());
            m_stream.Write(Operators.WhiteSpace);
            m_stream.Write(op);
            m_stream.Write(Operators.NewLine);
        }

        /// <summary>
        /// Sets the color and color space.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="colorSpace">The color space.</param>
        /// <param name="forStroking">if set to <c>true</c>
        /// the colour is set for stroking operations.</param>
        public void SetColorAndSpace(PdfColor color, PdfColorSpace colorSpace, bool forStroking)
        {
            if (!color.IsEmpty)
            {
                // bool test = color is PdfExtendedColor;
                m_stream.Write(color.ToString(colorSpace, forStroking));
                m_stream.Write(Operators.NewLine);
            }
        }
        /// <summary>
        /// Sets the color and color space.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="colorSpace"></param>
        /// <param name="forStroking"></param>
        /// <param name="check"></param>
        public void SetColorAndSpace(PdfColor color, PdfColorSpace colorSpace, bool forStroking, bool check)
        {
            if (!color.IsEmpty)
            {
                m_stream.Write(color.CalToString(colorSpace, forStroking));
                m_stream.Write(Operators.NewLine);
            }
        }
        /// <summary>
        /// Sets the color and space.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="colorSpace">The color space.</param>
        /// <param name="forStroking">if it is for stroking, set to <c>true</c>.</param>
        /// <param name="check">if it is check, set to <c>true</c>.</param>
        /// <param name="iccbased">if it is iccbased, set to <c>true</c>.</param>
        public void SetColorAndSpace(PdfColor color, PdfColorSpace colorSpace, bool forStroking, bool check, bool iccbased)
        {
            if (!color.IsEmpty)
            {
                m_stream.Write(color.IccColorToString(colorSpace, forStroking));
                m_stream.Write(Operators.NewLine);
            }
        }
        /// <summary>
        /// Sets the color and space.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="colorSpace">The color space.</param>
        /// <param name="forStroking">if it is for for stroking, set to <c>true</c>.</param>
        /// <param name="check">if it is to check, set to <c>true</c>.</param>
        /// <param name="iccbased">if it is iccbased, set to <c>true</c>.</param>
        /// <param name="indexed">if it is indexed, set to <c>true</c>.</param>
        public void SetColorAndSpace(PdfColor color, PdfColorSpace colorSpace, bool forStroking, bool check, bool iccbased, bool indexed)
        {
            if (!color.IsEmpty)
            {
                m_stream.Write(color.IndexedToString(forStroking));
                m_stream.Write(Operators.NewLine);
            }
        }
        /// <summary>
        /// Sets the color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="currentSpace">The current space.</param>
        /// <param name="forStroking">if set to <c>true</c>
        /// the colour is set for stroking operations.</param>
        public void SetColor(PdfColor color, PdfColorSpace currentSpace, bool forStroking)
        {
            string op = (forStroking) ? Operators.SetColorStroking : Operators.SetColorNonStroking;


            switch (currentSpace)
            {
                case PdfColorSpace.CMYK:
                    m_stream.Write(PdfNumber.FloatToString(color.C));
                    m_stream.Write(Operators.WhiteSpace);
                    m_stream.Write(PdfNumber.FloatToString(color.M));
                    m_stream.Write(Operators.WhiteSpace);
                    m_stream.Write(PdfNumber.FloatToString(color.Y));
                    m_stream.Write(Operators.WhiteSpace);
                    m_stream.Write(PdfNumber.FloatToString(color.K));
                    m_stream.Write(Operators.WhiteSpace);

                    break;

                case PdfColorSpace.GrayScale:
                    m_stream.Write(PdfNumber.FloatToString(color.Gray));
                    break;

                case PdfColorSpace.RGB:
                    m_stream.Write(PdfNumber.FloatToString(color.Red));
                    m_stream.Write(Operators.WhiteSpace);
                    m_stream.Write(PdfNumber.FloatToString(color.Green));
                    m_stream.Write(Operators.WhiteSpace);
                    m_stream.Write(PdfNumber.FloatToString(color.Blue));
                    m_stream.Write(Operators.WhiteSpace);
                    break;

                default:
                    throw new ArgumentException("Unknown current color space");
            }

            WriteOperator(op);
        }
        /// <summary>
        /// Sets the colour with pattern.
        /// </summary>
        /// <param name="colours">The colour array.</param>
        /// <param name="patternName">The name of the pattern.</param>
        /// <param name="forStroking">if set to <c>true</c> the colours and pattern
        /// are set for stroking operations.</param>
        public void SetColourWithPattern(IList colours, PdfName patternName, bool forStroking)
        {
            if (colours != null)
            {
                for (int i = 0, count = colours.Count; i < count; ++i)
                {
                    m_stream.Write(colours[i].ToString());
                    m_stream.Write(Operators.WhiteSpace);
                }
            }

            if (patternName != null)
            {
                m_stream.Write(patternName.ToString());
                m_stream.Write(Operators.WhiteSpace);
            }

            if (forStroking)
            {
                WriteOperator(Operators.SetColorAndPatternStroking);
            }
            else
            {
                WriteOperator(Operators.SetColorAndPattern);
            }
        }
        #endregion Colour primitives

        #region XObject primitives
        /// <summary>
        /// Executes the XObject.
        /// </summary>
        /// <param name="name">The name of the XObject.</param>
        public void ExecuteObject(string name)
        {
            ExecuteObject(new PdfName(name));
        }
        /// <summary>
        /// Executes the XObject.
        /// </summary>
        /// <param name="name">The name of the XObject.</param>
        public void ExecuteObject(PdfName name)
        {
            m_stream.Write(name.ToString());
            m_stream.Write(Operators.WhiteSpace);
            WriteOperator(Operators.PaintXObject);
        }
        #endregion

        #endregion Public Methods

        #region Implementation
        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <returns>The internal PdfStream object.</returns>
        internal PdfStream GetStream()
        {
            return m_stream;
        }

        /// <summary>
        /// Clears a stream.
        /// </summary>
        internal void Clear()
        {
            m_stream.Clear();
        }

        /// <summary>
        /// Writes the point.
        /// </summary>
        /// <param name="point">The point.</param>
        private void WritePoint(PointF point)
        {
            WritePoint(point.X, point.Y);
        }

        /// <summary>
        /// Writes the point.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        private void WritePoint(float x, float y)
        {
            m_stream.Write(PdfNumber.FloatToString(x));
            m_stream.Write(Operators.WhiteSpace);

            // NOTE: Change Y co-ordinate because we shifted co-ordinate system only.
            y = PdfGraphics.UpdateY(y);
            m_stream.Write(PdfNumber.FloatToString(y));
            m_stream.Write(Operators.WhiteSpace);
        }

        /// <summary>
        /// Writes the text.
        /// </summary>
        /// <param name="text">The text.</param>
        private void WriteText(object text)
        {
            if (text is PdfString)
            {
                WriteText(text as PdfString);
            }
            else if (text is string)
            {
                WriteText(text as string);
            }
            else if (text is byte[])
            {
                WriteText(text as byte[]);
            }
            else
            {
                throw new ArgumentException("Unknown text format", "text");
            }
        }

        /// <summary>
        /// Writes the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="hex">if set to <c>true</c> the text is in hex.</param>
        private void WriteText(byte[] text, bool hex)
        {
            char start;
            char end;

            if (hex)
            {
                start = PdfString.HexStringMark[0];
                end = PdfString.HexStringMark[1];
            }
            else
            {
                start = PdfString.StringMark[0];
                end = PdfString.StringMark[1];
            }

            m_stream.Write(start);

            if (hex)
            {
                m_stream.Write(PdfString.BytesToHex(text));
            }
            else
            {
                m_stream.Write(text);
            }
            m_stream.Write(end);
        }

        /// <summary>
        /// Writes the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="hex">if set to <c>true</c> the text is in hex.</param>
        private void WriteText(string text, bool hex)
        {
            char start;
            char end;

            if (hex)
            {
                start = PdfString.HexStringMark[0];
                end = PdfString.HexStringMark[1];
            }
            else
            {
                start = PdfString.StringMark[0];
                end = PdfString.StringMark[1];
            }

            m_stream.Write(start);
            m_stream.Write(text);
            m_stream.Write(end);
        }

        /// <summary>
        /// Writes the text.
        /// </summary>
        /// <param name="text">The text.</param>
        private void WriteText(PdfString text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            byte[] buff = text.PdfEncode(null);
            m_stream.Write(buff);
        }

        /// <summary>
        /// Writes the operator.
        /// </summary>
        /// <param name="opcode">The operator code.</param>
        private void WriteOperator(string opcode)
        {
            m_stream.Write(opcode);
            m_stream.Write(Operators.NewLine);
        }

        /// <summary>
        /// Checks the text param.
        /// </summary>
        /// <param name="text">The text.</param>
        private void CheckTextParam(byte[] text)
        {
            if (text == null)
                throw new ArgumentNullException("text");
        }

        /// <summary>
        /// Checks the text param.
        /// </summary>
        /// <param name="text">The text.</param>
        private void CheckTextParam(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (text == string.Empty)
                throw new ArgumentException("The text can't be an empty string", "text");
        }

        /// <summary>
        /// Checks the text param.
        /// </summary>
        /// <param name="text">The text.</param>
        private void CheckTextParam(PdfString text)
        {
            if (text == null)
                throw new ArgumentNullException("text");
        }
        #endregion

        #region IPdfWriter Members

        /// <summary>
        /// Gets or sets the current position within the stream.
        /// </summary>
        long IPdfWriter.Position
        {
            get
            {
                return m_stream.InternalStream.Position;
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        /// <summary>
        /// Gets stream length.
        /// </summary>
        /// <value></value>
        public long Length
        {
            get
            {
                return m_stream.InternalStream.Length;
            }
        }

        PdfDocumentBase IPdfWriter.Document
        {
            get
            {
                return null;
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        /// <summary>
        /// Writes the specified PDF object.
        /// </summary>
        /// <param name="pdfObject">The PDF object.</param>
        void IPdfWriter.Write(IPdfPrimitive pdfObject)
        {
            pdfObject.Save(this);
        }

        /// <summary>
        /// Writes the specified number.
        /// </summary>
        /// <param name="number">The number.</param>
        void IPdfWriter.Write(long number)
        {
            m_stream.Write(number.ToString());
        }

        /// <summary>
        /// Writes the specified number.
        /// </summary>
        /// <param name="number">The number.</param>
        void IPdfWriter.Write(float number)
        {
            m_stream.Write(PdfNumber.FloatToString(number));
        }

        /// <summary>
        /// Writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        void IPdfWriter.Write(string text)
        {
            m_stream.Write(text);
        }

        /// <summary>
        /// Writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        void IPdfWriter.Write(char[] text)
        {
            m_stream.Write(new string(text));
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        void IPdfWriter.Write(byte[] data)
        {
            m_stream.Write(data);
        }

        #endregion
    }
}
