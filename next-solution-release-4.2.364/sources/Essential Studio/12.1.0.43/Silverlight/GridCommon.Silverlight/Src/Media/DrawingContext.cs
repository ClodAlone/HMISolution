#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Collections;
using System.Collections.Generic;
using Syncfusion.Windows.GridCommon;
using System.Globalization;

namespace Syncfusion.Windows
{
    public class DrawingContext : IDisposable
    {
        #region Fields

        private Border _border;
        internal Image _imageControl;
        private TranslateTransform _translateTransform;
        internal WriteableBitmap _bmp;
        private List<Geometry> _geometryClipStack;
        private UIElement _uiElement;

        #endregion

        #region Ctor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageControl"></param>
        /// <param name="size"></param>
        internal DrawingContext(Image imageControl, Size size)
        {
            this._imageControl = imageControl;
            this._border = new Border();
            this._border.Visibility = Visibility.Collapsed;
            this._translateTransform = new TranslateTransform();
            this._bmp = new WriteableBitmap(int.Parse(size.Width.ToString()), int.Parse(size.Height.ToString()));
            this._imageControl.Source = this._bmp;
            this.CreateGeometryClipStack();
        }

        #endregion

        #region PrivateMethods

        /// <summary>
        /// Creates GeometryClipStack for use in Push/Pop
        /// </summary>
        private void CreateGeometryClipStack()
        {
            if (this._geometryClipStack == null)
            {
                this._geometryClipStack = new List<Geometry>();
            }
        }


        private void SetUIElement(UIElement element)
        {
            this._uiElement = element;
            if (this._border == null)
            {
                this._border = new Border();
            }

            if (this._border != null)
            {
                this._border.Child = this._uiElement;
            }
        }

        #endregion

        #region InternalMethods

        /// <summary>
        /// Pushes WriteableBitmap into drawing context.
        /// </summary>
        internal void PushAssociatedBitmap(Image img)
        {
            if (this._bmp == null)
            {
                if (img.Source is WriteableBitmap)
                {
                    this._bmp = img.Source as WriteableBitmap;
                }
                else
                {
                    this._bmp = new WriteableBitmap(img, null);
                }


                this._imageControl = img;
                this._imageControl.Source = this._bmp;
            }
        }

        #endregion

        #region PublicMethods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formattedText"></param>
        /// <param name="origin"></param>
        public void DrawText(FormattedText formattedText, Rect textRect, bool forceInvalidate)
        {
            this.DrawText(formattedText, textRect);

            if (forceInvalidate)
            {
                this._bmp.Invalidate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formattedText"></param>
        /// <param name="origin"></param>
        public void DrawText(FormattedText formattedText, Rect textRect)
        {
            if (formattedText.FontOrientation != 0)
            {
                DrawTextRotated(formattedText, textRect);
                return;
            }


            if (this._uiElement == null || !(this._uiElement is TextBlock))
            {
                this.SetUIElement(new TextBlock());
            }

            if (this._translateTransform == null)
            {
                this._translateTransform = new TranslateTransform();
            }

            var tb = this._uiElement as TextBlock;
            tb.Text = formattedText.Text;
            tb.FontFamily = formattedText.FontFamily;
            tb.FontSize = formattedText.FontSize;
            tb.FontStretch = formattedText.FontStretch;
            tb.FontWeight = formattedText.FontWeight;
            tb.FontStyle = formattedText.FontStyle;
            tb.Foreground = formattedText.Foreground;
            tb.HorizontalAlignment = formattedText.HorizontalAlignment;
            tb.TextAlignment = FormattedText.HorizontalAlignmentToTextAlignment(formattedText.HorizontalAlignment);
            tb.Margin = formattedText.Margin;
            tb.Padding = formattedText.Padding;
            tb.TextDecorations = formattedText.TextDecorations;
            tb.TextWrapping = formattedText.TextWrapping;
            tb.VerticalAlignment = formattedText.VerticalAlignment;
            tb.Width = textRect.Width-(formattedText.Margin.Left + formattedText.Margin.Right);
            tb.Height = textRect.Height - (formattedText.Margin.Top + formattedText.Margin.Bottom);

            var tg = new TransformGroup();
            var rt = new RotateTransform();
            var tt = new TranslateTransform();

            //if (formattedText.FontOrientation != 0)
            //{
            //    rt.Angle = formattedText.FontOrientation;
            //    rt.CenterX = textRect.Width / 2;
            //    rt.CenterY = textRect.Height / 2;
            //}

            tt.X = 0;
            tt.Y = 0;
            double height = tb.ActualHeight;      
            double yOffset = (textRect.Height - height);

            if (yOffset < 0)
            {               
                yOffset = 0;
            }

            if (height < textRect.Height)
                textRect.Height = height;


            VerticalAlignment verticalAlignment = formattedText.VerticalAlignment;

            switch (verticalAlignment)
            {
                case VerticalAlignment.Center:
                    tt.Y += yOffset / 2;
                    break;

                case VerticalAlignment.Top:
                    break;

                case VerticalAlignment.Bottom:
                    tt.Y += yOffset;
                    break;
            }

            //if (formattedText.VerticalAlignment == VerticalAlignment.Top)
            //{

            //}
            //else if (formattedText.VerticalAlignment == VerticalAlignment.Bottom)
            //{
            //    tt.Y = textRect.Height - tb.BaselineOffset - 1;
            //}
            //else if (formattedText.VerticalAlignment == VerticalAlignment.Center)
            //{
            //    tt.Y = (textRect.Height / 2) - (tb.BaselineOffset / 2);
            //}

            tg.Children.Add(tt);
            tg.Children.Add(rt);
            tb.RenderTransform = tg;
            this._translateTransform.X = textRect.X;
            this._translateTransform.Y = textRect.Y;
            var rectWidth = textRect.Width;
            var rectHeight = textRect.Height;
            //if (formattedText.FontOrientation >= 60 && formattedText.FontOrientation <= 120)
            //{
            //     rectWidth = textRect.Height;
            //     rectHeight = textRect.Width;
            //}
          
            if ((textRect.Width - (tb.Margin.Left + tb.Margin.Right + tb.Padding.Left + tb.Padding.Right)) >= 0)
            {
                rectWidth -= (tb.Margin.Left + tb.Margin.Right + tb.Padding.Left + tb.Padding.Right);
            }
            else
            {
                rectWidth = 0;
            }

            if ((textRect.Height - (tb.Margin.Top + tb.Margin.Bottom + tb.Padding.Top + tb.Padding.Bottom)) >= 0)
            {
                rectHeight -= (tb.Margin.Top + tb.Margin.Bottom + tb.Padding.Top + tb.Padding.Bottom);
            }
            else
            {
                rectHeight = 0;
            }


            var rectangleGeometry = new RectangleGeometry()
            {
                Rect = new Rect(
                    0,
                    0,
                    rectWidth,
                    rectHeight
                    )
            };

            tb.Clip = rectangleGeometry;

            //if (formattedText.FontOrientation != 0)
            //{
            //    WriteableBitmap wbp = new WriteableBitmap((int)textRect.Width, (int)textRect.Height);
            //    var rect = new Rect(new Point(0, 0), GridUtil.GetSize(textRect));
            //    wbp.Render(tb, tg);
            //    wbp.Invalidate();
            //    this._bmp.Blit(textRect, wbp, rect);
            //    return;
            //}

            this._bmp.Render(this._border, this._translateTransform);

        }

        public void DrawTextRotated(FormattedText formattedText, Rect textRect)
        {
            if (this._uiElement == null || !(this._uiElement is TextBlock))
            {
                this.SetUIElement(new TextBlock());
            }


            var tb = this._uiElement as TextBlock;
            tb.Text = formattedText.Text;
            tb.FontFamily = formattedText.FontFamily;
            tb.FontSize = formattedText.FontSize;
            tb.FontStretch = formattedText.FontStretch;
            tb.FontWeight = formattedText.FontWeight;
            tb.FontStyle = formattedText.FontStyle;
            tb.Foreground = formattedText.Foreground;
            tb.HorizontalAlignment = formattedText.HorizontalAlignment;
            tb.TextAlignment = FormattedText.HorizontalAlignmentToTextAlignment(formattedText.HorizontalAlignment);
            tb.Margin = formattedText.Margin;
            tb.Padding = formattedText.Padding;
            tb.TextDecorations = formattedText.TextDecorations;
            tb.TextWrapping = formattedText.TextWrapping;
            tb.VerticalAlignment = formattedText.VerticalAlignment;
            tb.Width = textRect.Width;
            tb.Height = textRect.Height;


            var rectWidth = textRect.Width;
            var rectHeight = textRect.Height;

            //convert location to 0 to 360
            int angle = formattedText.FontOrientation;
            while (angle < 0)
                angle += 360;
            angle = angle % 360;


            if ((formattedText.FontOrientation >= 60 && formattedText.FontOrientation <= 120)
                || (formattedText.FontOrientation >= 240 && formattedText.FontOrientation <= 300))
            {
                rectWidth = textRect.Height;
                rectHeight = textRect.Width;

                tb.Width = rectWidth;
                tb.Height = rectHeight;
            }



            //                  ---y----------------------------------------------
            //                  |                                                 |
            //                  x                    some text                    |
            //                  |                                                 |
            //                  ---------------------------------------------------
            //
            //  the idea is to translate point x to point y and then rotate 90 degrees. From the TR, x is tb.ActualHeight / 2 (shift below)
            var tg = new TransformGroup();
            var rt = new RotateTransform();
            var tt = new TranslateTransform();
            var tt1 = new TranslateTransform();

            double shift = tb.ActualHeight / 2;

            rt.Angle = angle;
            rt.CenterX = 0;
            rt.CenterY = shift;

            tt.Y = -shift;
            tt.X = shift;

            tt1.Y = -2 * shift; // not sure why this is needed????

            switch (formattedText.VerticalAlignment)
            {
                case VerticalAlignment.Center:
                    tt1.X = Math.Max(0, (textRect.Width - tb.ActualHeight) / 2);
                    break;
                case VerticalAlignment.Bottom:
                    tt1.X = Math.Max(0, textRect.Width - tb.ActualHeight - 1);
                    break;
                default:

                    tt1.X = 0;
                    break;
            }

            if (formattedText.FontOrientation == 270)
            {
                rt.CenterX = 0;
                rt.CenterY = 0;

                tt.Y = 0;
                tt.X = 0;

                tt1.Y = textRect.Height;
                switch (formattedText.VerticalAlignment)
                {
                    case VerticalAlignment.Center:
                        tt1.X = Math.Max(0, (textRect.Width - tb.ActualHeight) / 2);
                        break;
                    case VerticalAlignment.Top:
                        tt1.X = Math.Max(0, textRect.Width - tb.ActualHeight - 1);
                        break;
                    default:

                        tt1.X = 0;
                        break;
                }
            }

            tg.Children.Add(tt);
            tg.Children.Add(rt);
            tg.Children.Add(tt1);
            tb.RenderTransform = tg;
            WriteableBitmap wbp = new WriteableBitmap((int)textRect.Width, (int)textRect.Height);
            var rect = new Rect(new Point(0, 0), GridUtil.GetSize(textRect));
            wbp.Render(tb, tg);
            wbp.Invalidate();
            this._bmp.Blit(textRect, wbp, rect);

        }





        public void DrawControl(UIElement element, Point origin)
        {
            if (this._translateTransform == null)
            {
                this._translateTransform = new TranslateTransform();
            }

            this._translateTransform.X = origin.X;
            this._translateTransform.Y = origin.Y;
            this._bmp.Render(element, this._translateTransform);
        }

        //
        // Summary:
        //     Draws a rectangle with the specified System.Windows.Media.Brush and System.Windows.Media.Pen.
        //     The pen and the brush can be null.
        //
        // Parameters:
        //   brush:
        //     The brush with which to fill the rectangle. This is optional, and can be
        //     null. If the brush is null, no fill is drawn.
        //
        //   pen:
        //     The pen with which to stroke the rectangle. This is optional, and can be
        //     null. If the pen is null, no stroke is drawn.
        //
        //   rectangle:
        //     The rectangle to draw.

        public void DrawRectangle(Brush brush, Pen pen, Rect rectangle, Rect clipRect)
        {
            Color c = Colors.White;
            if (brush is SolidColorBrush)
            {
                c = ((SolidColorBrush)brush).Color;
            }
            else if (brush == null)
            {
                return;
            }
            else //if (brush is LinearGradientBrush)
            {
                WriteableBitmap wbp = new WriteableBitmap((int)rectangle.Width, (int)rectangle.Height);
                var rect = new Rect(new Point(0, 0), GridUtil.GetSize(clipRect));
                var element = new Rectangle() { Fill = brush, Width = rectangle.Width, Height = rectangle.Height };
                wbp.Render(element, null);
                wbp.Invalidate();
                this._bmp.Blit(clipRect, wbp, rect);
                return;
                //throw new NotImplementedException("Brush is not of type SolidColorBrush");
            }

            //if (rectangle.Width > 0 && rectangle.Height > 0)
            //{
            //    int x = (int)Double.Parse(rectangle.X.ToString());
            //    int y = (int)Double.Parse(rectangle.Y.ToString());
            //    int right = (int)(Double.Parse(rectangle.Right.ToString()));
            //    int bottom = (int)(Double.Parse(rectangle.Bottom.ToString()));

            //    this._bmp.FillRectangle(x, y, right, bottom, c);
            //}
        }


        public void DrawRectangle(Brush brush, Pen pen, Rect rectangle)
        {
            //To - Do color has to be checked properly
            Color c = Colors.White;
            if (brush is SolidColorBrush)
            {
                c = ((SolidColorBrush)brush).Color;
            }
            else if (brush == null)
            {
                return;
            }
            else if (brush is LinearGradientBrush)
            {
                WriteableBitmap wbp = new WriteableBitmap((int)rectangle.Width, (int)rectangle.Height);
                var rect = new Rect(new Point(0, 0), GridUtil.GetSize(rectangle));
                var element = new Rectangle() { Fill = brush, Width = rectangle.Width, Height = rectangle.Height };
                wbp.Render(element, null);
                wbp.Invalidate();
                this._bmp.Blit(rectangle, wbp, rect);
                return;
                //throw new NotImplementedException("Brush is not of type SolidColorBrush");
            }

            if (rectangle.Width > 0 && rectangle.Height > 0)
            {
                int x = (int)Double.Parse(rectangle.X.ToString());
                int y = (int)Double.Parse(rectangle.Y.ToString());
                int right = (int)(Double.Parse(rectangle.Right.ToString()));
                int bottom = (int)(Double.Parse(rectangle.Bottom.ToString()));

                this._bmp.FillRectangle(x, y, right, bottom, c);
            }
        }


        /// <summary>
        /// Drawsline from Point A to Point B
        /// </summary>
        /// <param name="pen"></param>
        /// <param name="point0">First Point</param>
        /// <param name="point1">Second Point</param>
        public void DrawLine(Pen pen, Point point0, Point point1)
        {
            if (pen.Thickness == 0)
            {
                return;
            }

            Color c = Colors.White;
            if (pen.Brush is SolidColorBrush)
            {
                c = ((SolidColorBrush)pen.Brush).Color;
            }
            else if (pen.Brush == null)
            {
                return;
            }
            else
            {
                throw new NotImplementedException("Brush is not of type SolidColorBrush");
            }

            int x1 = (int)Double.Parse(point0.X.ToString());
            int y1 = (int)Double.Parse(point0.Y.ToString());
            int x2 = (int)Double.Parse(point1.X.ToString());
            int y2 = (int)Double.Parse(point1.Y.ToString());
            int t = 0;

            if (pen.Thickness < 1 && pen.Thickness > 0)
            {
                t = 1;
            }
            else
            {
                t = (int)pen.Thickness;
            }



            this._bmp.DrawLine(x1,
                y1,
                x2,
                y2,
                c,
                t
                );
        }

        public void DrawTriangle(Pen pen, Point point0, Point point1, Point point2)
        {
            Color c = Colors.White;
            if (pen.Brush is SolidColorBrush)
            {
                c = ((SolidColorBrush)pen.Brush).Color;
            }
            else
            {
                throw new NotImplementedException("Brush is not of type SolidColorBrush");
            }

            int x1 = (int)Double.Parse(point0.X.ToString());
            int y1 = (int)Double.Parse(point0.Y.ToString());
            int x2 = (int)Double.Parse(point1.X.ToString());
            int y2 = (int)Double.Parse(point1.Y.ToString());
            int x3 = (int)Double.Parse(point2.X.ToString());
            int y3 = (int)Double.Parse(point2.Y.ToString());

            this._bmp.FillTriangle(x1,
                y1,
                x2,
                y2,
                x3,
                y3,
                c);
        }

        /// <summary>
        /// Invalidates  Writeablebitmap
        /// </summary>
        public void Invalidate()
        {
            if (this._bmp == null)
            {
                throw new Exception("Invalidate should not be called after WriteableBitmap is disposed");
            }

            this._bmp.Invalidate();
        }

        /// <summary>
        /// Resets WriteableBitmap renderSize
        /// </summary>
        /// <param name="actualWidth">Width</param>
        /// <param name="actualHeight">Height</param>
        public void ResetBitmap(int actualWidth, int actualHeight)
        {
            this._bmp = new WriteableBitmap(actualWidth, actualHeight);
        }

        /// <summary>
        /// Resets WriteableBitmap renderSize
        /// </summary>
        /// <param name="actualWidth">Width</param>
        /// <param name="actualHeight">Height</param>
        public void ResetBitmap(double actualWidth, double actualHeight)
        {
            this.ResetBitmap((int)actualWidth, (int)actualHeight);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clipGeometry"></param>
        public void PushClip(Geometry clipGeometry)
        {
            this.CreateGeometryClipStack();
            this._geometryClipStack.Add(clipGeometry);
            this.SetClip(clipGeometry);
        }

        private void SetClip(Geometry geometry)
        {
            if (this._imageControl != null)
            {
                this._imageControl.Clip = geometry;
            }
            else
            {
                throw new Exception("Clip cannot be pushed with image intialization");
            }
        }



        /// <summary>
        /// 
        /// </summary>
        public void Pop()
        {
            var lastIndex = this._geometryClipStack.Count - 1;
            this._imageControl.Clip = null;
            this._geometryClipStack.RemoveAt(lastIndex);
        }



        // Summary:
        //     Closes the System.Windows.Media.DrawingContext and flushes the content. Afterward,
        //     the System.Windows.Media.DrawingContext cannot be modified.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     This object has already been closed or disposed.
        public void Close()
        {
            //To - Do check object disposal in close
            this.Dispose();
        }

        #region IDisposable Members
        /// <summary>
        /// Dispose used resources
        /// </summary>
        public void Dispose()
        {
            this._bmp = null;
            this._translateTransform = null;
            //this._border = null;
            this._imageControl = null;
            //this._uiElement = null;
            if (this._geometryClipStack != null)
            {
                this._geometryClipStack.Clear();
                this._geometryClipStack = null;
            }
        }

        #endregion

        #endregion
    }
}
