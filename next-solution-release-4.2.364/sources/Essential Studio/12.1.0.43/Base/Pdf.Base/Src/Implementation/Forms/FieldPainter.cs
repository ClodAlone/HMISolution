#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;

using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represents class which draws form fields.
    /// </summary>
    internal class FieldPainter
    {
        #region Fields
        /// <summary>
        /// Internal variable to store white brush.
        /// </summary>
        private static PdfBrush s_whiteBrush = null;

        /// <summary>
        /// Internal variable to store black brush.
        /// </summary>
        private static PdfBrush s_blackBrush = null;

        /// <summary>
        /// Internal variable to store silver brush.
        /// </summary>
        private static PdfBrush s_silverBrush = null;

        /// <summary>
        /// Internal variable to store gray brush.
        /// </summary>
        private static PdfBrush s_grayBrush = null;

        /// <summary>
        /// Internal variable to store cached pens.
        /// </summary>
        private static Dictionary<string, PdfPen> s_pens = new Dictionary<string, PdfPen>();
        #endregion

        #region Constants
        /// <summary>
        /// Internal variable to store string format for check field symbols.
        /// </summary>
        private static PdfStringFormat s_checkFieldFormat = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the white brush.
        /// </summary>
        /// <value>The white brush.</value>
        private static PdfBrush WhiteBrush
        {
            get
            {
                lock (s_pens)
                {
                    if (s_whiteBrush == null)
                    {
                        s_whiteBrush = PdfBrushes.White;
                    }

                    return s_whiteBrush;
                }
            }
        }

        /// <summary>
        /// Gets the black brush.
        /// </summary>
        /// <value>The black brush.</value>
        private static PdfBrush BlackBrush
        {
            get
            {
                lock (s_pens)
                {
                    if (s_blackBrush == null)
                    {
                        s_blackBrush = PdfBrushes.Black;
                    }

                    return s_blackBrush;
                }
            }
        }

        /// <summary>
        /// Gets the gray brush.
        /// </summary>
        /// <value>The gray brush.</value>
        private static PdfBrush GrayBrush
        {
            get
            {
                lock (s_pens)
                {
                    if (s_grayBrush == null)
                    {
                        s_grayBrush = PdfBrushes.Gray;
                    }

                    return s_grayBrush;
                }
            }
        }

        /// <summary>
        /// Gets the silver brush.
        /// </summary>
        /// <value>The silver brush.</value>
        private static PdfBrush SilverBrush
        {
            get
            {
                lock (s_pens)
                {
                    if (s_silverBrush == null)
                    {
                        s_silverBrush = PdfBrushes.Silver;
                    }

                    return s_silverBrush;
                }
            }
        }

        /// <summary>
        /// Gets the check box format.
        /// </summary>
        /// <value>The check box format.</value>
        private static PdfStringFormat CheckFieldFormat
        {
            get
            {
                lock (s_pens)
                {
                    if (s_checkFieldFormat == null)
                    {
                        s_checkFieldFormat = new PdfStringFormat(PdfTextAlignment.Center,
                            PdfVerticalAlignment.Middle);
                    }

                    return s_checkFieldFormat;
                }
            }
        }
        #endregion

        #region Static methods
        /// <summary>
        /// Draws a button.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="paintParams">The paint params.</param>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="format">The format.</param>
        public static void DrawButton(PdfGraphics g, PaintParams paintParams,
            string text, PdfFont font, PdfStringFormat format)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            if (paintParams == null)
            {
                throw new ArgumentNullException("paintParams");
            }

            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            DrawRectangularControl(g, paintParams);

            g.DrawString(text, font, paintParams.ForeBrush, new RectangleF(new PointF(paintParams.Bounds.X, paintParams.Bounds.Y), paintParams.Bounds.Size), format);
        }

        /// <summary>
        /// Draws a pressed button.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="paintParams">The paint params.</param>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="format">The format.</param>
        public static void DrawPressedButton(PdfGraphics g, PaintParams paintParams,
            string text, PdfFont font, PdfStringFormat format)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            if (paintParams == null)
            {
                throw new ArgumentNullException("paintParams");
            }

            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            switch (paintParams.BorderStyle)
            {
                case PdfBorderStyle.Inset:
                    g.DrawRectangle(paintParams.ShadowBrush, paintParams.Bounds);
                    break;

                default:
                    g.DrawRectangle(paintParams.BackBrush, paintParams.Bounds);
                    break;
            }

            DrawBorder(g, paintParams.Bounds, paintParams.BorderPen,
                paintParams.BorderStyle, paintParams.BorderWidth);

            RectangleF rectangle = new RectangleF(paintParams.BorderWidth, paintParams.BorderWidth,
                paintParams.Bounds.Size.Width - paintParams.BorderWidth,
                paintParams.Bounds.Size.Height - paintParams.BorderWidth);

            g.DrawString(text, font, paintParams.ForeBrush, rectangle, format);

            switch (paintParams.BorderStyle)
            {
                case PdfBorderStyle.Inset:
                    DrawLeftTopShadow(g, paintParams.Bounds, paintParams.BorderWidth,
                        GrayBrush);
                    DrawRightBottomShadow(g, paintParams.Bounds, paintParams.BorderWidth,
                        SilverBrush);
                    break;

                case PdfBorderStyle.Beveled:
                    DrawLeftTopShadow(g, paintParams.Bounds, paintParams.BorderWidth,
                        paintParams.ShadowBrush);
                    DrawRightBottomShadow(g, paintParams.Bounds, paintParams.BorderWidth,
                        WhiteBrush);
                    break;

                default:
                    DrawLeftTopShadow(g, paintParams.Bounds, paintParams.BorderWidth,
                        paintParams.ShadowBrush);
                    break;
            }
        }

        /// <summary>
        /// Draws a button.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="paintParams">The paint params.</param>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="format">The format.</param>
        public static void DrawTextBox(PdfGraphics g, PaintParams paintParams,
            string text, PdfFont font, PdfStringFormat format,bool multiLine, bool scroll)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            if (paintParams == null)
            {
                throw new ArgumentNullException("paintParams");
            }

            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            DrawRectangularControl(g, paintParams);

            RectangleF rectangle = paintParams.Bounds;

            if (paintParams.BorderStyle == PdfBorderStyle.Beveled ||
                paintParams.BorderStyle == PdfBorderStyle.Inset)
            {
                rectangle.X = rectangle.X + 4 * paintParams.BorderWidth;
                rectangle.Width = rectangle.Width - 8 * paintParams.BorderWidth;
            }
            else
            {
                rectangle.X = rectangle.X + 2 * paintParams.BorderWidth;
                rectangle.Width = rectangle.Width - 4 * paintParams.BorderWidth;
            }

            // Calculate position of the text.
            if (multiLine)
            {
                float tempheight = (format == null || format.LineSpacing == 0f) ? font.Height : format.LineSpacing;
                bool subScript = (format != null && format.SubSuperScript == PdfSubSuperScript.SubScript);
                float ascent = font.Metrics.GetAscent(format);
                float descent = font.Metrics.GetDescent(format);
                float shift = (subScript) ? tempheight - (font.Height + descent) : (tempheight - ascent);
                if (rectangle.Location != PointF.Empty)// This will be true if the field is flattened.
                    rectangle.Y -= shift;
                else
                    rectangle.Y = -(rectangle.Y - shift);
            }

            bool shouldRotate = false;
            PdfDictionary parentDictionary = null;
            if (g.Layer != null && g.Page != null)
            {
                if (g.Page.Dictionary.ContainsKey(DictionaryProperties.Rotate))
                {
                    shouldRotate = true;
                }
                else
                {
                    parentDictionary = new PdfDictionary();
                    parentDictionary = (g.Page.Dictionary[DictionaryProperties.Parent] as PdfReferenceHolder).Object as PdfDictionary;
                    if (parentDictionary != null && parentDictionary.ContainsKey(DictionaryProperties.Rotate))
                        shouldRotate = true;
                }
            }
            if (paintParams.RotationAngle > 0 )
                shouldRotate = true;
            if (paintParams.RotationAngle > 0 && shouldRotate)
            {
                PdfGraphicsState state = g.Save();

                float z = rectangle.X;
                rectangle.X = -(rectangle.Y + rectangle.Height);
                rectangle.Y = z;

                float height = rectangle.Height;
                rectangle.Height = rectangle.Width > font.Height ? rectangle.Width : font.Height;
                rectangle.Width = height;
                if (paintParams.RotationAngle == 90 && parentDictionary == null)
                {
                    g.TranslateTransform(g.Size.Height, 0);
                }
                if (paintParams.RotationAngle == 180 && parentDictionary == null)
                {
                    g.TranslateTransform(g.Size.Height, g.Size.Width);
                }
                if (paintParams.RotationAngle == 270 && parentDictionary == null)
                {
                    g.TranslateTransform(0, g.Size.Width);
                }
                if (parentDictionary != null)
                {
                    g.RotateTransform(-paintParams.RotationAngle);
                }
                g.DrawString(text, font, paintParams.ForeBrush, rectangle, format);

                g.Restore(state);
            }
            else
            {
                g.DrawString(text, font, paintParams.ForeBrush, rectangle, format);
            }

        }

        /// <summary>
        /// Draws a list box.
        /// </summary>
        /// <param name="g">The graphics.</param>
        /// <param name="paintParams">The paint params.</param>
        /// <param name="items">The items.</param>
        /// <param name="selectedItem">The selected item index.</param>
        /// <param name="font">The font.</param>
        /// <param name="stringFormat">The string format.</param>
        public static void DrawListBox(PdfGraphics g, PaintParams paintParams,
            PdfListFieldItemCollection items, int[] selectedItem, PdfFont font, PdfStringFormat stringFormat)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            if (paintParams == null)
            {
                throw new ArgumentNullException("paintParams");
            }

            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            DrawRectangularControl(g, paintParams);

            for (int index = 0, count = items.Count; index < count; ++index)
            {
                PdfListFieldItem item = items[index];
                PointF point = PointF.Empty;
                float borderWidth = paintParams.BorderWidth;
                float doubleBorderWidth = 2 * borderWidth;

                bool padding = (paintParams.BorderStyle == PdfBorderStyle.Inset || paintParams.BorderStyle == PdfBorderStyle.Beveled);

                if (padding)
                {
                    point.X = 2 * doubleBorderWidth;
                    point.Y = (index + 2) * borderWidth + font.Size * index;
                }
                else
                {
                    point.X = doubleBorderWidth;
                    point.Y = (index + 1) * borderWidth + font.Size * index;
                }

                PdfBrush brush = paintParams.ForeBrush;
                RectangleF rect = paintParams.Bounds;
                float width = rect.Width - doubleBorderWidth;

                RectangleF rectangle = rect;

                if (padding)
                {
                    rectangle.Height -= doubleBorderWidth;
                }
                else
                {
                    rectangle.Height -= borderWidth;
                }

                g.SetClip(rectangle, PdfFillMode.Winding);
                
				bool selected = false;

                foreach (int selectedIndex in selectedItem)
                {
                    if (selectedIndex == index)
                    {
                        selected = true;
                    }
                }
                
				if (selected)
                {
                    float x = rect.X + borderWidth;

                    if (padding)
                    {
                        x += borderWidth;
                        width -= doubleBorderWidth;
                    }

                    brush = new PdfSolidBrush(
                        new PdfColor(
                            255, 51, 153, 255));

                    g.DrawRectangle(brush, x, point.Y, width, font.Height);
                    brush = new PdfSolidBrush(new PdfColor(255, 255, 255, 255));
                }

                string value = (item.Text != null) ? item.Text : item.Value;

                RectangleF itemTextBound = new RectangleF(point.X, point.Y, width - point.X, font.Height);

                g.DrawString(value, font, brush, itemTextBound, stringFormat);
            }
        }

        /// <summary>
        /// Draws a check box.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="paintParams">The paint params.</param>
        /// <param name="checkSymbol">The check symbol.</param>
        /// <param name="state">The state.</param>
        public static void DrawCheckBox(PdfGraphics g, PaintParams paintParams,
            string checkSymbol, PdfCheckFieldState state)
        {
            DrawCheckBox(g, paintParams, checkSymbol, state, null);
        }

        /// <summary>
        /// Draws a check box.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="paintParams">The paint params.</param>
        /// <param name="checkSymbol">The check symbol.</param>
        /// <param name="state">The state.</param>
        /// <param name="font">The font.</param>
        public static void DrawCheckBox(PdfGraphics g, PaintParams paintParams,
            string checkSymbol, PdfCheckFieldState state, PdfFont font)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            if (paintParams == null)
            {
                throw new ArgumentNullException("paintParams");
            }

            if (checkSymbol == null)
            {
                throw new ArgumentNullException("checkSymbol");
            }

            switch (state)
            {
                case PdfCheckFieldState.Unchecked:
                case PdfCheckFieldState.Checked:
                    g.DrawRectangle(paintParams.BackBrush, paintParams.Bounds);
                    break;

                case PdfCheckFieldState.PressedChecked:
                case PdfCheckFieldState.PressedUnchecked:
                    if ((paintParams.BorderStyle == PdfBorderStyle.Beveled) ||
                        (paintParams.BorderStyle == PdfBorderStyle.Underline))
                    {
                        g.DrawRectangle(paintParams.BackBrush, paintParams.Bounds);
                    }
                    else
                    {
                        g.DrawRectangle(paintParams.ShadowBrush, paintParams.Bounds);
                    }

                    break;
            }

            DrawBorder(g, paintParams.Bounds, paintParams.BorderPen,
                paintParams.BorderStyle, paintParams.BorderWidth);

            if ((state == PdfCheckFieldState.PressedChecked) ||
                (state == PdfCheckFieldState.PressedUnchecked))
            {
                switch (paintParams.BorderStyle)
                {
                    case PdfBorderStyle.Inset:
                        DrawLeftTopShadow(g, paintParams.Bounds, paintParams.BorderWidth,
                            BlackBrush);
                        DrawRightBottomShadow(g, paintParams.Bounds, paintParams.BorderWidth,
                            WhiteBrush);
                        break;

                    case PdfBorderStyle.Beveled:
                        DrawLeftTopShadow(g, paintParams.Bounds, paintParams.BorderWidth,
                            paintParams.ShadowBrush);
                        DrawRightBottomShadow(g, paintParams.Bounds, paintParams.BorderWidth,
                            WhiteBrush);
                        break;
                }
            }
            else
            {
                switch (paintParams.BorderStyle)
                {
                    case PdfBorderStyle.Inset:
                        DrawLeftTopShadow(g, paintParams.Bounds, paintParams.BorderWidth,
                            GrayBrush);
                        DrawRightBottomShadow(g, paintParams.Bounds, paintParams.BorderWidth,
                            SilverBrush);
                        break;

                    case PdfBorderStyle.Beveled:
                        DrawLeftTopShadow(g, paintParams.Bounds, paintParams.BorderWidth,
                            WhiteBrush);
                        DrawRightBottomShadow(g, paintParams.Bounds, paintParams.BorderWidth,
                            paintParams.ShadowBrush);
                        break;
                }
            }

            switch (state)
            {
                case PdfCheckFieldState.PressedChecked:
                case PdfCheckFieldState.Checked:
                    if (font == null)
                    {
                        int fontSize = (int)(paintParams.Bounds.Height * 0.4);
                        font = new PdfStandardFont(PdfFontFamily.ZapfDingbats,
                            fontSize);
                    }
                    else
                    {
                        font=new PdfStandardFont(PdfFontFamily.ZapfDingbats,
                            font.Size);
                    }
                        if (paintParams.Bounds.Height < font.Size)
                        {
                            throw new Exception("Font size cannot be greater than CheckBox height");
                        }
                    g.DrawString(checkSymbol, font, paintParams.ForeBrush,
                        paintParams.Bounds, CheckFieldFormat);
                    break;
            }
        }

        /// <summary>
        /// Draws a combo box.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="paintParams">The paint params.</param>
        public static void DrawComboBox(PdfGraphics g, PaintParams paintParams)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            if (paintParams == null)
            {
                throw new ArgumentNullException("paintParams");
            }

            DrawRectangularControl(g, paintParams);
        }

        /// <summary>
        /// Draws the radio button.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="paintParams">The paint params.</param>
        /// <param name="checkSymbol">The check symbol.</param>
        /// <param name="state">The state.</param>
        public static void DrawRadioButton(PdfGraphics g, PaintParams paintParams,
            string checkSymbol, PdfCheckFieldState state)
        {
            switch (state)
            {
                case PdfCheckFieldState.Unchecked:
                case PdfCheckFieldState.Checked:
                    g.DrawEllipse(paintParams.BackBrush, paintParams.Bounds);
                    break;

                case PdfCheckFieldState.PressedChecked:
                case PdfCheckFieldState.PressedUnchecked:
                    if ((paintParams.BorderStyle == PdfBorderStyle.Beveled) ||
                        (paintParams.BorderStyle == PdfBorderStyle.Underline))
                    {
                        g.DrawEllipse(paintParams.BackBrush, paintParams.Bounds);
                    }
                    else
                    {
                        g.DrawEllipse(paintParams.ShadowBrush, paintParams.Bounds);
                    }

                    break;
            }

            DrawRoundBorder(g, paintParams.Bounds, paintParams.BorderPen, paintParams.BorderWidth);
            DrawRoundShadow(g, paintParams, state);

            switch (state)
            {
                case PdfCheckFieldState.Checked:
                case PdfCheckFieldState.PressedChecked:
                    float fontSize = 0;
                    switch (paintParams.BorderStyle)
                    {
                        case PdfBorderStyle.Beveled:
                        case PdfBorderStyle.Inset:
                            fontSize = (float)((paintParams.Bounds.Height - 4 * paintParams.BorderWidth) / 1.5f);
                            break;

                        default:
                            fontSize = (float)((paintParams.Bounds.Height - 2 * paintParams.BorderWidth) / 1.5f);
                            break;
                    }

                    if (paintParams.Bounds == RectangleF.Empty)
                    {
                        fontSize = 0;
                    }

                    PdfFont font = new PdfStandardFont(PdfFontFamily.ZapfDingbats, fontSize);

                    g.DrawString(checkSymbol, font, paintParams.ForeBrush/*BlackBrush*/,
                        paintParams.Bounds, CheckFieldFormat);
                    break;
            }
        }


        /// <summary>
        /// Draws the signature.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="paintParams">The paint params.</param>
        public static void DrawSignature(PdfGraphics g, PaintParams paintParams)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            if (paintParams == null)
            {
                throw new ArgumentNullException("paintParams");
            }

            DrawRectangularControl(g, paintParams);

            RectangleF rectangle = paintParams.Bounds;

            if (paintParams.BorderStyle == PdfBorderStyle.Beveled ||
                paintParams.BorderStyle == PdfBorderStyle.Inset)
            {
                rectangle.X = rectangle.X + 4 * paintParams.BorderWidth;
                rectangle.Width = rectangle.Width - 8 * paintParams.BorderWidth;
            }
            else
            {
                rectangle.X = rectangle.X + 2 * paintParams.BorderWidth;
                rectangle.Width = rectangle.Width - 4 * paintParams.BorderWidth;
            }
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Draws a border.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="borderPen">The border pen.</param>
        /// <param name="style">The style.</param>
        /// <param name="borderWidth">Width of the border.</param>
        private static void DrawBorder(PdfGraphics g, RectangleF bounds, PdfPen borderPen,
            PdfBorderStyle style, int borderWidth)
        {
            if (borderPen != null)
            {
                if ((borderWidth > 0) && (!borderPen.Color.IsEmpty))
                {
                    if (style == PdfBorderStyle.Underline)
                    {
                        g.DrawLine(borderPen, bounds.X, bounds.Y + bounds.Height - borderWidth / 2.0f,
                            bounds.X + bounds.Width, bounds.Y + bounds.Height - borderWidth / 2.0f);
                    }
                    else
                    {
                        RectangleF outward = new RectangleF(bounds.X + borderWidth / 2.0f,
                            bounds.Y + borderWidth / 2.0f,
                            bounds.Width - borderWidth,
                            bounds.Height - borderWidth);
                        g.DrawRectangle(borderPen, outward);
                    }
                }
            }
        }

        /// <summary>
        /// Draws the round border.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="borderPen">The border pen.</param>
        /// <param name="borderWidth">Width of the border.</param>
        private static void DrawRoundBorder(PdfGraphics g, RectangleF bounds, PdfPen borderPen,
            int borderWidth)
        {
            RectangleF outward = bounds;

            if (outward != RectangleF.Empty)
            {
                outward = new RectangleF(bounds.X + borderWidth / 2.0f,
                    bounds.Y + borderWidth / 2.0f,
                    bounds.Width - borderWidth,
                    bounds.Height - borderWidth);

                g.DrawEllipse(borderPen, outward);
            }
        }

        /// <summary>
        /// Draws a rectangular control.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="paintParams">The paint params.</param>
        private static void DrawRectangularControl(PdfGraphics g, PaintParams paintParams)
        {
            g.DrawRectangle(paintParams.BackBrush, paintParams.Bounds);
            DrawBorder(g, paintParams.Bounds, paintParams.BorderPen,
                paintParams.BorderStyle, paintParams.BorderWidth);

            switch (paintParams.BorderStyle)
            {
                case PdfBorderStyle.Inset:
                    DrawLeftTopShadow(g, paintParams.Bounds, paintParams.BorderWidth,
                        GrayBrush);
                    DrawRightBottomShadow(g, paintParams.Bounds, paintParams.BorderWidth,
                        SilverBrush);
                    break;

                case PdfBorderStyle.Beveled:
                    DrawLeftTopShadow(g, paintParams.Bounds, paintParams.BorderWidth,
                        WhiteBrush);
                    DrawRightBottomShadow(g, paintParams.Bounds, paintParams.BorderWidth,
                        paintParams.ShadowBrush);
                    break;
            }
        }

        /// <summary>
        /// Draws the left top shadow.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="width">The width.</param>
        /// <param name="brush">The brush.</param>
        private static void DrawLeftTopShadow(PdfGraphics g, RectangleF bounds, int width,
            PdfBrush brush)
        {
            PdfPath path = new PdfPath();
            PointF[] points = new PointF[6];

            points[0] = new PointF(bounds.X + width, bounds.Y + width);
            points[1] = new PointF(bounds.X + width, bounds.Bottom - width);
            points[2] = new PointF(bounds.X + 2 * width, bounds.Bottom - 2 * width);
            points[3] = new PointF(bounds.X + 2 * width, bounds.Y + 2 * width);
            points[4] = new PointF(bounds.Right - 2 * width, bounds.Y + 2 * width);
            points[5] = new PointF(bounds.Right - width, bounds.Y + width);

            path.AddPolygon(points);
            g.DrawPath(brush, path);
        }

        /// <summary>
        /// Draws the right bottom shadow.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="width">The width.</param>
        /// <param name="brush">The brush.</param>
        private static void DrawRightBottomShadow(PdfGraphics g, RectangleF bounds, int width,
            PdfBrush brush)
        {
            PdfPath path = new PdfPath();
            PointF[] points = new PointF[6];

            points[0] = new PointF(bounds.X + width, bounds.Bottom - width);
            points[1] = new PointF(bounds.X + 2 * width, bounds.Bottom - 2 * width);
            points[2] = new PointF(bounds.Right - 2 * width, bounds.Bottom - 2 * width);
            points[3] = new PointF(bounds.Right - 2 * width, bounds.Y + 2 * width);
            points[4] = new PointF(bounds.X + bounds.Width - width, bounds.Y + width);
            points[5] = new PointF(bounds.Right - width, bounds.Bottom - width);

            path.AddPolygon(points);
            g.DrawPath(brush, path);
        }

        /// <summary>
        /// Draws the round shadow.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="paintParams">The paint params.</param>
        /// <param name="state">The state.</param>
        private static void DrawRoundShadow(PdfGraphics g, PaintParams paintParams,
            PdfCheckFieldState state)
        {
            float borderWidth = paintParams.BorderWidth;
            RectangleF rectangle = paintParams.Bounds;
            rectangle.Inflate(-1.5f * borderWidth, -1.5f * borderWidth);

            PdfPen leftTopPen = null;
            PdfPen rightBottomPen = null;

            PdfSolidBrush shadowBrush = (PdfSolidBrush)paintParams.ShadowBrush;
            PdfColor shadowColor = shadowBrush.Color;

            switch (paintParams.BorderStyle)
            {
                case PdfBorderStyle.Beveled:
                    switch (state)
                    {
                        case PdfCheckFieldState.PressedChecked:
                        case PdfCheckFieldState.PressedUnchecked:
                            leftTopPen = GetPen(shadowColor, borderWidth);
                            rightBottomPen = GetPen(new PdfColor(255, 255, 255), borderWidth);
                            break;

                        case PdfCheckFieldState.Checked:
                        case PdfCheckFieldState.Unchecked:
                            leftTopPen = GetPen(new PdfColor(255, 255, 255), borderWidth);
                            rightBottomPen = GetPen(shadowColor, borderWidth);

                            break;
                    }

                    break;

                case PdfBorderStyle.Inset:
                    switch (state)
                    {
                        case PdfCheckFieldState.PressedChecked:
                        case PdfCheckFieldState.PressedUnchecked:
                            leftTopPen = GetPen(new PdfColor(0, 0, 0), borderWidth);
                            rightBottomPen = GetPen(new PdfColor(0, 0, 0), borderWidth);
                            break;

                        case PdfCheckFieldState.Checked:
                        case PdfCheckFieldState.Unchecked:
                            leftTopPen = GetPen(new PdfColor(255, 128, 128, 128), borderWidth);
                            rightBottomPen = GetPen(new PdfColor(255, 192, 192, 192), borderWidth);
                            break;
                    }
                    break;
            }

            if (leftTopPen != null && rightBottomPen != null)
            {
                g.DrawArc(leftTopPen, rectangle, 135, 180);
                g.DrawArc(rightBottomPen, rectangle, -45, 180);
            }
        }

        /// <summary>
        /// Gets the pen.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        private static PdfPen GetPen(PdfColor color, float width)
        {
            lock (s_pens)
            {
                string name = String.Format("{0}{1}", color, width);
                PdfPen pen = s_pens.ContainsKey(name) ? s_pens[name] : null;

                if (pen == null)
                {
                    pen = new PdfPen(color, width);
                    s_pens[name] = pen;
                }

                return pen;
            }
        }
        #endregion
    }
}
