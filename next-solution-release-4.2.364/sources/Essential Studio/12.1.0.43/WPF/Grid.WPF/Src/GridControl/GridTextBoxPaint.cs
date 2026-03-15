#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

using Syncfusion.Windows.GridCommon;
using System.Windows.Input;
using System.Linq;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Draws a text box control in the grid cells.
    /// </summary>
    public class GridTextBoxPaint
    {
        /// <summary>
        /// Gets or sets a value indicating whether [allow round draw text origin].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [allow round draw text origin]; otherwise, <c>false</c>.
        /// </value>
        public static bool AllowRoundDrawTextOrigin { get; set; }

        /// <summary>
        /// Initializes the <see cref="GridTextBoxPaint"/> class.
        /// </summary>
        static GridTextBoxPaint()
        {
            /// Default value as false to render the text in exact position
            AllowRoundDrawTextOrigin = false;
        }


        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="drawingContext">The drawing context.</param>
        /// <param name="textRectangle">The text rectangle.</param>
        /// <param name="displayText">The display text.</param>
        /// <param name="style">The cell style.</param>
        public static void DrawText(DrawingContext drawingContext, Rect textRectangle, string displayText, GridStyleInfo style)
        {
            try
            {
                if (string.IsNullOrEmpty(displayText))
                {
                    return;
                }

                var cultureInfo = style.GetCulture(true);
                var flowDirection = style.FlowDirection;
                var typeface = style.Typeface;
                double emSize = style.ReadOnlyFont.FontSize;
                double lineHeight = style.ReadOnlyFont.GetLineHeightValue();
                var foreground = style.Foreground;
                // TODO: Should foreground be frozen? - Is animation possible with DrawText
                //if (!foreground.IsFrozen)
                //    foreground.Freeze();
                var formattedText = new FormattedText(displayText, cultureInfo, flowDirection, typeface, emSize, foreground);

                formattedText.Trimming = style.TextTrimming;
                formattedText.TextAlignment = HorizontalAlignmentToTextAlignment(style.HorizontalAlignment);

                if (style.Font != null && style.Font.TextDecorations != null)
                {
                    formattedText.SetTextDecorations(style.Font.TextDecorations);
                }

                var originalTextRectangle = textRectangle;
                var rotatedTextRectangle = textRectangle;
                double orientation = style.Font.Orientation % 360;
                double minValue = 4;
                if (orientation < 0)
                {
                    orientation += 360;
                }

                var isVerticalOrientation = orientation == 90 || orientation == 270;
                var isHorizontalOrientation = orientation == 180 || orientation == 0;

                RotateTransform rotation = null;
                if (orientation != 0)
                {
                    rotation = new RotateTransform(orientation, textRectangle.X + textRectangle.Width / 2, textRectangle.Y + textRectangle.Height / 2);
                    rotatedTextRectangle = rotation.TransformBounds(textRectangle);
                }

                // TODO: When only a single line is drawn and text is not wrapped then drawingContext.DrawText
                // does break text at "(" character when text does not fit into cell and is for example: =SUM(C9:C14)
                //if (displayText.StartsWith("=SUM"))
                //    Console.WriteLine(displayText);

                textRectangle = rotatedTextRectangle;

                bool clip = false;
                bool wrapText = style.TextWrapping != TextWrapping.NoWrap;
                if (wrapText)
                {
                    if (textRectangle.Height >= lineHeight * 2)
                        formattedText.MaxLineCount = (int)(textRectangle.Height / lineHeight) + 1;
                    else
                        wrapText = false;
                }

                if (!wrapText)
                {
                    formattedText.MaxLineCount = 1;
                }

                // Set a maximum width and height. If the text overflows these values, an ellipsis "..." appears.
                if (isVerticalOrientation)
                {
                    formattedText.MaxTextWidth = Math.Max(minValue, originalTextRectangle.Height);
                    formattedText.MaxTextHeight = Math.Max(minValue, originalTextRectangle.Width);
                }
                else if (orientation == 180)
                {
                    formattedText.MaxTextWidth = Math.Max(minValue, originalTextRectangle.Width);
                    formattedText.MaxTextHeight = Math.Max(minValue, originalTextRectangle.Height);
                }
                else if (orientation != 0)
                {
                    formattedText.MaxTextWidth = Math.Max(minValue, Math.Min(originalTextRectangle.Height, originalTextRectangle.Width));
                    formattedText.MaxTextHeight = Math.Max(minValue, formattedText.MaxTextWidth);
                }
                else
                {
                    if (style.TextWrapping != TextWrapping.NoWrap || formattedText.Trimming != TextTrimming.None)
                        formattedText.MaxTextWidth = Math.Max(minValue, textRectangle.Width);
                    else
                        formattedText.MaxTextWidth = textRectangle.Width;
                }

                if (orientation != 0 && orientation != 270 && orientation != 90 && orientation != 180)
                {
                    double width = formattedText.Width;
                    double xOffset = (textRectangle.Width - width);

                    if (xOffset < 0)
                    {
                        clip = true;
                        xOffset = 0;
                    }
                    if (width < textRectangle.Width)
                        textRectangle.Width = width;
                    else
                        clip = true;

                    HorizontalAlignment hAlignment = style.HorizontalAlignment;

                    switch (hAlignment)
                    {
                        case HorizontalAlignment.Center:
                            textRectangle.X += xOffset / 2;
                            break;

                        case HorizontalAlignment.Left:
                            break;

                        case HorizontalAlignment.Right:
                            textRectangle.X += xOffset;
                            break;
                    }
                }

                double height = formattedText.Height;
                double yOffset = (textRectangle.Height - height);

                if (yOffset < 0)
                {
                    clip = true;
                    yOffset = 0;
                }

                if (height < textRectangle.Height)
                    textRectangle.Height = height;
                else
                    clip = true;

                VerticalAlignment verticalAlignment = style.VerticalAlignment;
                var top = style.BorderMargins.Top;
                var bottom = style.BorderMargins.Bottom;

                switch (verticalAlignment)
                {
                    case VerticalAlignment.Center:
                        textRectangle.Y += (yOffset / 2) - 2 * (top + bottom);//Here we have Subtract  2*(top + bottom) because vertical alignment of the text wrongly measured.
                        break;

                    case VerticalAlignment.Top:
                        textRectangle.Y -= 2 * (top + bottom);
                        break;

                    case VerticalAlignment.Bottom:
                        textRectangle.Y += yOffset - 2 * (top + bottom);
                        break;
                }

                if (clip || formattedText.Width > ((orientation != 0 && orientation != 180) ? originalTextRectangle.Height : originalTextRectangle.Width)
                    || formattedText.Height > ((orientation != 0 && orientation != 180) ? originalTextRectangle.Width : originalTextRectangle.Height))
                {

                    Rect r = textRectangle;
                    if (orientation == 90 || orientation == 180 || orientation == 270)
                    {
                        r.Intersect(originalTextRectangle);
                        r = rotation.TransformBounds(r);
                    }

                    RectangleGeometry ge = new RectangleGeometry(r);
                    ge.Freeze();
                    drawingContext.PushClip(ge);
                    if (rotation != null)
                    {
                        drawingContext.PushTransform(rotation);
                    }

                    Point alignedLocation;

                    /// Checking for drawing mode before positioning the text.
                    if (AllowRoundDrawTextOrigin)
                    {
                        // #I59953 fix for anti-aliased rendering mode
                        alignedLocation = new Point(Math.Ceiling(textRectangle.Location.X), Math.Ceiling(textRectangle.Location.Y));
                    }
                    else
                    {
                        // This is to avoid text positioning issue with automation and 0.5 is added with y to avoid aliased rendering mode issue.
                        alignedLocation = new Point(textRectangle.Location.X, textRectangle.Location.Y + 0.5);
                    }

                    if (flowDirection == FlowDirection.RightToLeft)
                    {
                        double m11 = -1;
                        double m22 = 1;
                        double offsetX = formattedText.Width + originalTextRectangle.Width;
                        double offsetY = 0;
                        drawingContext.PushTransform(new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY));
                        alignedLocation.X = formattedText.Width - alignedLocation.X;
                    }

                    // Draw the formatted text string to the DrawingContext of the control.
                    drawingContext.DrawText(formattedText, alignedLocation);
                    if (flowDirection == FlowDirection.RightToLeft)
                        drawingContext.Pop();


                    if (rotation != null)
                    {
                        drawingContext.Pop();
                    }
                    drawingContext.Pop();
                }
                else
                {
                    if (rotation != null)
                    {
                        drawingContext.PushTransform(rotation);
                    }

                    Point alignedLocation;

                    /// Checking for drawing mode before positioning the text.
                    if (AllowRoundDrawTextOrigin)
                    {
                        // #I59953 fix for anti-aliased rendering mode
                        alignedLocation = new Point(Math.Ceiling(textRectangle.Location.X), Math.Ceiling(textRectangle.Location.Y));
                    }
                    else
                    {
                        // This is to avoid text positioning issue with automation and 0.5 is added with y to avoid aliased rendering mode issue.
                        alignedLocation = new Point(textRectangle.Location.X, textRectangle.Location.Y + 0.5);
                    }

                    if (flowDirection == FlowDirection.RightToLeft)
                    {
                        double m11 = -1;
                        double m22 = 1;
                        double offsetX = formattedText.Width + originalTextRectangle.Width;
                        double offsetY = 0;
                        drawingContext.PushTransform(new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY));
                        alignedLocation.X = formattedText.Width - alignedLocation.X;
                    }

                    // Draw the formatted text string to the DrawingContext of the control.
                    drawingContext.DrawText(formattedText, alignedLocation);
                    if (flowDirection == FlowDirection.RightToLeft)
                        drawingContext.Pop();

                    if (rotation != null)
                    {
                        drawingContext.Pop();
                    }
                }
            }
            catch
            { }
        }

        /// <summary>
        /// Returns the size of the text.
        /// </summary>
        /// <param name="textRectangle">Text rectangle.</param>
        /// <param name="displayText">The display text.</param>
        /// <param name="style">Cell style.</param>
        /// <param name="queryBounds">Cell bounds.</param>
        /// <returns>Size of the text.</returns>
        public static Size MeasureText(Size textRectangle, string displayText, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            var cultureInfo = style.GetCulture(true);
            var flowDirection = style.FlowDirection;
            var typeface = style.Typeface;
            double emSize = style.ReadOnlyFont.FontSize;
            // double lineHeight = style.ReadOnlyFont.GetLineHeightValue();
            var foreground = style.Foreground;
            var formattedText = new FormattedText(displayText, cultureInfo, flowDirection, typeface, emSize, foreground);

            if (queryBounds == GridQueryBounds.Height)
                formattedText.MaxTextWidth = textRectangle.Width;
            //else
            //    formattedText.MaxTextHeight = textRectangle.Height;

            formattedText.Trimming = style.TextTrimming;
            //formattedText.TextAlignment = TextAlHorizontalAlignmentToTextAlignment(style.HorizontalAlignment);

            ////bool wrapText = style.TextWrapping != TextWrapping.NoWrap;
            ////if (wrapText)
            ////{
            ////    if (formattedText.Height > lineHeight)
            ////        formattedText.MaxLineCount = (int)(formattedText.Height / lineHeight) + 1;
            ////    else
            ////        wrapText = false;
            ////}

            ////if (!wrapText)
            ////{
            ////    formattedText.MaxLineCount = 1;
            ////    //formattedText.MaxTextHeight = lineHeight;
            ////    //formattedText.Trimming = TextTrimming.None;//.CharacterEllipsis;
            ////}

            ////// TODO: What about fontInfo.Orientation?
            return new Size(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);
        }

        // Converts a HorizontalAlignment enum to a TextAlignment enum.
        private static TextAlignment HorizontalAlignmentToTextAlignment(HorizontalAlignment horizontalAlignment)
        {
            TextAlignment textAlignment;

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Left:
                default:
                    textAlignment = TextAlignment.Left;
                    break;

                case HorizontalAlignment.Right:
                    textAlignment = TextAlignment.Right;
                    break;

                case HorizontalAlignment.Center:
                    textAlignment = TextAlignment.Center;
                    break;

                case HorizontalAlignment.Stretch:
                    textAlignment = TextAlignment.Justify;
                    break;
            }

            return textAlignment;
        }
    }
}