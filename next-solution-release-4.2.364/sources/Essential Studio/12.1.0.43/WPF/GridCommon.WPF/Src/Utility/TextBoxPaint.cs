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

namespace Syncfusion.Windows.GridCommon
{
    /// <summary>
    /// A class for painting text and make it emulate the look how
    /// text is rendered by a <see cref="System.Windows.Controls.TextBox"/>
    /// </summary>
    public class TextBoxPaint
    {
        Typeface typeface;

        /// <summary>
        /// Gets or sets the typeface.
        /// </summary>
        /// <value>The typeface.</value>
        public Typeface Typeface
        {
            get { return typeface; }
            set { typeface = value; }
        }
        double emSize;

        /// <summary>
        /// Gets or sets the size of the em.
        /// </summary>
        /// <value>The size of the em.</value>
        public double EmSize
        {
            get { return emSize; }
            set { emSize = value; }
        }
        Brush foreground;

        /// <summary>
        /// Gets or sets the fore ground.
        /// </summary>
        /// <value>The fore ground.</value>
        public Brush Foreground
        {
            get { return foreground; }
            set { foreground = value; }
        }
        FlowDirection flowDirection = FlowDirection.LeftToRight;

        /// <summary>
        /// Gets or sets the flow direction.
        /// </summary>
        /// <value>The flow direction.</value>
        public FlowDirection FlowDirection
        {
            get { return flowDirection; }
            set { flowDirection = value; }
        }
        CultureInfo cultureInfo;

        /// <summary>
        /// Gets or sets the culture info.
        /// </summary>
        /// <value>The culture info.</value>
        public CultureInfo CultureInfo
        {
            get { return cultureInfo; }
            set { cultureInfo = value; }
        }
        TextTrimming textTrimming = TextTrimming.None;

        /// <summary>
        /// Gets or sets the trimming.
        /// </summary>
        /// <value>The trimming.</value>
        public TextTrimming Trimming
        {
            get { return textTrimming; }
            set { textTrimming = value; }
        }
        //FontStyles fontStyles = FontStyles.Normal;
        //FontWeights fontWeights = FontWeights.Normal;
        //static readonly Typeface arialTypeface = new Typeface("Arial");
        TextAlignment textAlignment = TextAlignment.Left;

        TextWrapping textWrapping = TextWrapping.NoWrap;

        /// <summary>
        /// Gets or sets the text wrapping.
        /// </summary>
        /// <value>The text wrapping.</value>
        public TextWrapping TextWrapping
        {
            get { return textWrapping; }
            set { textWrapping = value; }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment.
        /// </summary>
        /// <value>The horizontal alignment.</value>
        public TextAlignment HorizontalAlignment
        {
            get { return textAlignment; }
            set { textAlignment = value; }
        }
        TextDecorationCollection textDecorations = new TextDecorationCollection(); // Strikethrough, Underline

        /// <summary>
        /// Gets or sets the text decorations.
        /// </summary>
        /// <value>The text decorations.</value>
        public TextDecorationCollection TextDecorations
        {
            get { return textDecorations; }
            set { textDecorations = value; }
        }
        double orientation = 0; // 0-360

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        public double Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }
        bool wrapText = false;

        /// <summary>
        /// Gets or sets a value indicating whether text is wrapped.
        /// </summary>
        /// <value><c>true</c> if text is wrapped; otherwise, <c>false</c>.</value>
        public bool WrapText
        {
            get { return wrapText; }
            set { wrapText = value; }
        }
        VerticalAlignment verticalAlignment = VerticalAlignment.Center;

        /// <summary>
        /// Gets or sets the vertical alignment.
        /// </summary>
        /// <value>The vertical alignment.</value>
        public VerticalAlignment VerticalAlignment
        {
            get { return verticalAlignment; }
            set { verticalAlignment = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBoxPaint"/> class.
        /// </summary>
        /// <param name="typeface">The typeface.</param>
        /// <param name="emSize">Size of the em.</param>
        /// <param name="foreGround">The fore ground.</param>
        public TextBoxPaint(Typeface typeface, double emSize, Brush foreGround)
        {
            this.typeface = typeface;
            this.emSize = emSize;
            this.foreground = foreGround;

            cultureInfo = CultureInfo.CurrentUICulture;
        }

        /// <summary>
        /// Gets the frozen brush.
        /// </summary>
        /// <param name="textColor">Color of the text.</param>
        /// <returns></returns>
        public static Brush GetFrozenBrush(Color textColor)
        {
            Brush textBrush = new SolidColorBrush(textColor);
            textBrush.Freeze();
            return textBrush;
        }

        /// <summary>
        /// Gets the typeface.
        /// </summary>
        /// <param name="faceName">Name of the type face.</param>
        /// <returns></returns>
        public static Typeface GetTypeface(string faceName)
        {
            Typeface typeface = new Typeface(faceName);
            return typeface;
        }

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="drawingContext">The drawing context.</param>
        /// <param name="textRectangle">The text rectangle.</param>
        /// <param name="displayText">The display text.</param>
        public void DrawText(DrawingContext drawingContext, Rect textRectangle, string displayText)
        {
            FormattedText formattedText = new FormattedText(displayText, cultureInfo, flowDirection, typeface, emSize, foreground);

            formattedText.Trimming = textTrimming;
            //formattedText.SetFontStyle(fontStyles);
            //formattedText.SetFontWeight(fontWeights);
            formattedText.TextAlignment = textAlignment;

            if (textDecorations.Count > 0)
            {
                textDecorations.Freeze();
                formattedText.SetTextDecorations(textDecorations);
            }

            Rect originalTextRectangle = textRectangle;
            Rect rotatedTextRectangle = textRectangle;

            if (orientation != 0)
            {
                rotatedTextRectangle = CalcInsideRect(originalTextRectangle, -orientation);
            }

            textRectangle = rotatedTextRectangle;

            if (wrapText)
                formattedText.MaxLineCount = Math.Max(2, (int)(textRectangle.Height / emSize));

            // Set a maximum width and height. If the text overflows these values, an ellipsis "..." appears.
            //if (fontInfo.Orientation == 90 || fontInfo.Orientation == 270)
            //{
            //    formattedText.MaxTextWidth = textRectangle.Height;
            //    formattedText.MaxTextHeight = textRectangle.Width;
            //}
            //else
            {
                formattedText.MaxTextWidth = textRectangle.Width;
                //formattedText.MaxTextHeight = textRectangle.Height;
            }
            double height = formattedText.Height;
            double yOffset = (textRectangle.Height - height);

            bool clip = false;

            if (yOffset < 0)
            {
                clip = true;
                yOffset = 0;
            }


            
            // Set a maximum width and height. If the text overflows these values, an ellipsis "..." appears.
            //Point pt = new Point(textRectangle.Width, textRectangle.Height);
            //Point pt0 = new Point(0, 0);
            //Point ptr = RotatePoint(pt, orientation, pt0);
            //formattedText.MaxTextWidth = Math.Abs(ptr.X);
            //formattedText.MaxTextHeight = Math.Abs(ptr.Y);
            //double dd = Math.Abs(orientation % 180);
            //if (dd > 45 && dd < 135)
            //{
            //    formattedText.MaxTextWidth = textRectangle.Height;
            //    formattedText.MaxTextHeight = textRectangle.W;
            //}
            //double height = Math.Abs(RotatePoint(new Point(textRectangle.Width, formattedText.Height), fontInfo.Orientation, pt0).Y);
            //double yOffset = (textRectangle.Height - height);
            //*/


            #region Rotate
            if (orientation != 0)
            {
                Rect rect = textRectangle;
                Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
                double angle = orientation;

                // Calculated rotated rectangle that fits inside the given cell.
                //Rect rcOutside = CalcOutsideRect(rect, angle);

                drawingContext.PushTransform(new TranslateTransform(center.X, center.Y));
                drawingContext.PushTransform(new RotateTransform(orientation));

                textRectangle = originalTextRectangle;

                //rcOutside.Offset(-center.X, -center.Y);

                textRectangle.Location = new Point(-rect.Width / 2, -rect.Height / 2);

                textRectangle.Height = height;

                switch (verticalAlignment)
                {
                    case VerticalAlignment.Center:
                        textRectangle.Y += yOffset / 2;
                        break;

                    case VerticalAlignment.Top:
                        break;

                    case VerticalAlignment.Bottom:
                        textRectangle.Y += yOffset;
                        break;
                }

                // Draw the formatted text string to the DrawingContext of the control.
                drawingContext.DrawText(formattedText, textRectangle.Location);

                drawingContext.Pop();
                drawingContext.Pop();
            }
            else
            #endregion
            {
                if (height < textRectangle.Height)
                    textRectangle.Height = height;
                else
                    clip = true;

                switch (verticalAlignment)
                {
                    case VerticalAlignment.Center:
                        textRectangle.Y += yOffset / 2;
                        break;

                    case VerticalAlignment.Top:
                        break;

                    case VerticalAlignment.Bottom:
                        textRectangle.Y += yOffset;
                        break;
                }

                if (clip || formattedText.Width > textRectangle.Width
                    || formattedText.Height > textRectangle.Height)
                {
                    RectangleGeometry ge = new RectangleGeometry(textRectangle);
                    ge.Freeze();
                    drawingContext.PushClip(ge);

                    //formattedText.MaxTextWidth = textRectangle.Width;
                    //formattedText.MaxTextHeight = textRectangle.Height;

                    // Draw the formatted text string to the DrawingContext of the control.
                    drawingContext.DrawText(formattedText, textRectangle.Location);

                    drawingContext.Pop();
                }
                else
                {
                    // Draw the formatted text string to the DrawingContext of the control.
                    drawingContext.DrawText(formattedText, textRectangle.Location);
                }
            }
        }

#if false
        
        public static Size MeasureText(GridStyleInfo style, Rect textRectangle, string displayText)
        {
            GridFontInfo fontInfo = style.Font;

            FormattedText formattedText;
            formattedText = new FormattedText(
            displayText,
            System.Globalization.CultureInfo.GetCultureInfo("en-us"),
            FlowDirection.LeftToRight,
            new Typeface(fontInfo.Facename),
            fontInfo.Size,
            Brushes.Black);

            formattedText.FlowDirection = style.FlowDirection;

            formattedText.SetCulture(style.GetCulture(true));

            formattedText.SetFontStyle(fontInfo.Italic ? FontStyles.Italic : FontStyles.Normal);
            formattedText.SetFontWeight(fontInfo.Bold ? FontWeights.Bold : FontWeights.Normal);
            switch (style.HorizontalAlignment)
            {
                case GridHorizontalAlignment.Center:
                    formattedText.TextAlignment = TextAlignment.Center;
                    break;

                case GridHorizontalAlignment.Left:
                    formattedText.TextAlignment = TextAlignment.Left;
                    break;

                case GridHorizontalAlignment.Right:
                    formattedText.TextAlignment = TextAlignment.Right;
                    break;

                case GridHorizontalAlignment.Justify:
                    formattedText.TextAlignment = TextAlignment.Justify;
                    break;
            }

            TextDecorationCollection tcc = new TextDecorationCollection();
            if (fontInfo.Strikeout)
                tcc.Add(TextDecorations.Strikethrough);

            if (fontInfo.Underline)
                tcc.Add(TextDecorations.Underline);

            if (tcc.Count > 0)
            {
                tcc.Freeze();
                formattedText.SetTextDecorations(tcc);
            }

            // Set a maximum width and height. If the text overflows these values, an ellipsis "..." appears.
            //if (fontInfo.Orientation == 90 || fontInfo.Orientation == 270)
            //{
            //    formattedText.MaxTextWidth = textRectangle.Height;
            //    formattedText.MaxTextHeight = textRectangle.Width;
            //}
            else
            {
                formattedText.MaxTextWidth = textRectangle.Width;
                formattedText.MaxTextHeight = textRectangle.Height;
            }

            if (style.WrapText)
                formattedText.MaxLineCount = Math.Max(1, (int)(textRectangle.Height / fontInfo.Size));
            formattedText.Trimming = style.Trimming;

            return new Size(formattedText.Width, formattedText.Height);
        }

#endif

        internal static Point RotatePoint(Point p, double angle, Point origin)
        {
            double normAngle = angle;

            while (normAngle < 0)
                normAngle += 360.0;

            while (normAngle >= 360.0)
                normAngle -= 360.0;

            double radianAngle = angle * Math.PI / 180.0;

            double ox = origin.X;
            double oy = origin.Y;
            double cos = Math.Cos(radianAngle);
            double sin = Math.Sin(radianAngle);
            double dx = p.X - origin.X;
            double dy = p.Y - origin.Y;
            double x = ox + dx * cos - dy * sin;
            double y = oy + dy * cos + dx * sin;

            return new Point((double)x, (double)y);
        }

        internal static Point[] RotateRectangle(Rect r, double angle)
        {
            return RotateRectangle(r, angle, new Point(0, 0));
        }

        internal static Point[] RotateRectangle(Rect r, double angle, Point origin)
        {
            return new Point[] 
			{
				RotatePoint(new Point(r.Left, r.Top), angle, origin),
				RotatePoint(new Point(r.Left, r.Bottom), angle, origin),
				RotatePoint(new Point(r.Right, r.Bottom), angle, origin),
				RotatePoint(new Point(r.Right, r.Top), angle, origin)
			};
        }

        internal static Rect CalcOutsideRect(Rect rect, double angle)
        {
            // Calculated rotated rectangle that fits inside the given cell
            Rect r = rect; //new Rect(Point.Empty, rect.Size);
            Point center = new Point(r.X + r.Width / 2, r.Y + r.Height / 2);
            Point[] points = RotateRectangle(r, angle, center);
            double left = double.MaxValue, right = 0;
            double top = double.MaxValue, bottom = 0;
            foreach (Point pt in points)
            {
                left = Math.Min(left, pt.X);
                right = Math.Max(right, pt.X);
                top = Math.Min(top, pt.Y);
                bottom = Math.Max(bottom, pt.Y);
            }
            return FromLTRB(left, top, right, bottom);
        }

        /// <summary>
        /// Creates a Rect.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        /// <returns></returns>
        public static Rect FromLTRB(double left, double top, double right, double bottom)
        {
            return new Rect(left, top, right - left, bottom - top);
        }

        internal static Rect CalcInsideRect(Rect rect, double angle)
        {
            // Calculated rotated rectangle that fits inside the given cell
            Rect r = CalcOutsideRect(rect, angle); //new Rect(Point.Empty, rect.Size);

            double normAngle = angle;

            while (normAngle < 0)
                normAngle += 360.0;

            while (normAngle >= 360.0)
                normAngle -= 360.0;

            double radianAngle = angle * Math.PI / 180.0;

            double cos = Math.Cos(radianAngle);
            double sin = Math.Sin(radianAngle);
            double dx = r.Width;
            double dy = r.Height;

            double x = Math.Abs(dx * cos - dy * sin);
            double y = Math.Abs(dy * cos + dx * sin);

            if (normAngle == 90 || normAngle == 270)
                return CenterInRect(r, new Size(x, y));

            Rect rcInside = CenterInRect(rect, new Size(
                (double)(rect.Width * ((double)rect.Width / (double)r.Width)),
                (double)(rect.Height * ((double)rect.Height / (double)r.Height))
                ));
            return rcInside;
        }


        /// <summary>
        /// Returns a centered rectangle of a specified size within a given rectangle.
        /// </summary>
        /// <param name="rect">The outer rectangle.</param>
        /// <param name="size">The size of the rectangle to be centered.</param>
        /// <returns>The centered rectangle.</returns>
        static public Rect CenterInRect(Rect rect, Size size)
        {
            double dx = 0;
            if (size.Width < rect.Width)
                dx = rect.Width - size.Width;

            double dy = 0;
            if (size.Height < rect.Height)
                dy = rect.Height - size.Height;

            return new Rect(rect.Left + dx / 2, rect.Top + dy / 2,
                Math.Min(size.Width, rect.Width), Math.Min(size.Height, rect.Height));
        }

        /// <summary>
        /// Returns a centered point within a given rectangle.
        /// </summary>
        /// <param name="rect">The outer rectangle.</param>
        /// <returns>The centered point.</returns>
        static public Point CenterPoint(Rect rect)
        {
            return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }

    }
}
