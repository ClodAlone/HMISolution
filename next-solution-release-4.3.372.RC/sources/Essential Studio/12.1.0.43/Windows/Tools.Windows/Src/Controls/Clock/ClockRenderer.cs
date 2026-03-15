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

namespace Syncfusion.Windows.Forms.Tools
{
    # region Interface

    public interface IClockRenderer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <param name="Center"></param>
        /// <param name="color"></param>
        /// <param name="dateTime"></param>
        void DrawHourHand(Graphics g, int length, float  width, Point Center, Color color, DateTime dateTime);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <param name="Center"></param>
        /// <param name="color"></param>
        /// <param name="dateTime"></param>
        void DrawMinuteHand(Graphics g, int length, float width, Point Center, Color color, DateTime dateTime);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <param name="Center"></param>
        /// <param name="color"></param>
        /// <param name="dateTime"></param>
        void DrawSecondHand(Graphics g, int length, float width, Point Center, Color color, DateTime dateTime);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="Radius"></param>
        /// <param name="center"></param>
        /// <param name="color"></param>
        /// <param name="font"></param>
        /// <param name="thickness"></param>
        void DrawMinutesLine(Graphics g, int Radius, Point center,Color color,Font font, float thickness);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <param name="image"></param>
        void DrawBackGroundImage(Graphics g, Rectangle rect, Image image);
    }

    # endregion

    # region ClockRenderer

    public class ClockRenderer:IClockRenderer
    {
        # region Members

        /// <summary>
        /// Indicate angle value
        /// </summary>
        float angle  = 3.14F/30;

        /// <summary>
        /// Indicate the clock
        /// </summary>
        private Clock clock;

        # endregion

        # region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <param name="image"></param>
        public void DrawBackGroundImage(Graphics g, Rectangle rect,Image image)
        {
            g.DrawImage(image, rect);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <param name="Center"></param>
        /// <param name="color"></param>
        /// <param name="dateTime"></param>
        public void DrawHourHand(Graphics g, int length, float width, Point Center, Color color, DateTime dateTime)
        {
            int hour = dateTime.Hour * 5 + (dateTime.Minute / 12);
            PointF centerPoint = new PointF(getXPoint(Center.X, -(float)length / 9, hour), getYPoint(Center.X, (float)length / 9, hour));
            PointF radiusPoint = new PointF(getXPoint(Center.X, (float)length, hour), getYPoint(Center.X, -(float)length, hour));
            DrawInterior(g, width, centerPoint, radiusPoint, color, "HoursHand");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <param name="Center"></param>
        /// <param name="color"></param>
        /// <param name="dateTime"></param>
        public void DrawMinuteHand(Graphics g, int length, float width, Point Center, Color color, DateTime dateTime)
        {
            PointF centerPoint = new PointF(getXPoint(Center.X, -(float)length / 9, dateTime.Minute ), getYPoint(Center.X, (float)length / 9, dateTime.Minute ));
            PointF radiusPoint = new PointF(getXPoint(Center.X, (float)length, dateTime.Minute ), getYPoint(Center.X, -(float)length, dateTime.Minute ));
            DrawInterior(g, width, centerPoint, radiusPoint, color, "MinutesHand");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <param name="Center"></param>
        /// <param name="color"></param>
        /// <param name="dateTime"></param>
        public void DrawSecondHand(Graphics g, int length, float width, Point Center, Color color, DateTime dateTime)
        {
            PointF centerPoint = new PointF (getXPoint (Center .X , - (float)length / 9 ,dateTime.Second ),  getYPoint(Center.X, (float)length / 9, dateTime.Second));
            PointF radiusPoint = new PointF (getXPoint (Center .X ,  (float)length,dateTime.Second ), getYPoint(Center.X, -(float)length, dateTime.Second));
            DrawInterior(g, width, centerPoint, radiusPoint, color, "SecondsHand");
                    
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="midPoint"></param>
        /// <param name="length"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private float getXPoint(int midPoint, float length, int dateTime)
        {
           return  (float)(midPoint + (length *getSineValue (dateTime )));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="midPoint"></param>
        /// <param name="length"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private float getYPoint(int midPoint, float length, int dateTime)
        {
            return (float)(midPoint + (length * getCoSineValue (dateTime )));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private double getSineValue(int value)
        {
            return  System.Math.Sin((value) * angle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private double getCoSineValue(int value)
        {
            return System.Math.Cos((value) * angle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="Radius"></param>
        /// <param name="center"></param>
        /// <param name="color"></param>
        /// <param name="font"></param>
        /// <param name="thickness"></param>
        public void DrawMinutesLine(Graphics g,int Radius,Point center, Color color,Font font, float thickness)
        {
           g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
           float startValue = 1.15f;
           float endValue = 1.20f;
           for(int i=0;i<60;i++)
			{
                startValue = 1.15f;
                endValue = 1.20f;
				if (  i%5==0 ) 
				{
                    endValue += 0.15f;
                }
                PointF centerPoint = new PointF(getXPoint(center.X, (float)Radius / startValue, i), getYPoint(center.X, -(float)Radius / startValue, i));
                PointF radiusPoint = new PointF(getXPoint(center.X, (float)Radius / endValue, i), getYPoint(center.X, -(float)Radius / endValue, i));
                DrawInterior(g, thickness, centerPoint, radiusPoint, color, "MinutesLine");
			}
        }

        public virtual void DrawInterior(Graphics g, float thickness, PointF startPoint, PointF endPoint, Color color, string sender)
        {

            using (Pen pen = new Pen(color, thickness))
            {
                g.DrawLine(pen, startPoint, endPoint);
            }
        }

        # endregion

        # region Properties

        /// <summary>
        /// Get or Set the clock
        /// </summary>
        private Clock Clock
        {
            get
            {
                return clock;
            }
            set
            {
                clock = value;
            }
        }

        # endregion
    }
    public interface IDigitalClockRenderer
    {
        /// <summary>
        /// Drawing Digital clock Text
        /// </summary>
        /// <param name="g">Graphics used for drawing the text</param>
        /// <param name="text">Text for the digital clock<" text "></param>
        /// <param name="CustomTime">CustomTime used for customizing the digital text</param>
        /// <param name="font">Font or the digital text</param>
        /// <param name="brush">brush used for drawing the highlighted lines</param>
        /// <param name="brushLight">brushLight used for drawing the un highlighted lines</param>
        /// <param name="textPoint"> x points for drawing the text</param>
        /// <param name="showAMPM">Show/Hide the AMPM</param>
        /// <param name="isAM">Display AM/PM</param>
        void DrawDigits(Graphics graphics, string text, DateTime CustomTime, Font font, Brush brush, Brush brushLight, PointF textPoint, bool showAMPM, bool isAM);
        /// <summary>
        /// Drawing Digital clock Digit
        /// </summary>
        /// <param name="g">Graphics used for drawing the text</param>
        /// <param name="text">Text for the digital clock<" text "></param>
        /// <param name="font">Font or the digital text</param>
        /// <param name="brush">brush used for drawing the highlighted lines</param>
        /// <param name="brushLight">brushLight used for drawing the un highlighted lines</param>
        /// <param name="x"> x points for drawing the text</param>
        /// <param name="y">y points for drawing the text</param>
        float DrawDigit(Graphics graphics, int num, Font font, Brush brush, Brush brushLight, float x, float y);
        /// <summary>
        /// Drawing Digital clock Dot
        /// </summary>
        /// <param name="graphics">Graphics used for drawing the text</param>
        /// <param name="font">Font or the digital text</param>
        /// <param name="brush">brush used for drawing the highlighted lines</param>
        /// <param name="x"> x points for drawing the text</param>
        /// <param name="y">y points for drawing the text</param>
        float DrawDot(Graphics graphics, Font font, Brush brush, float x, float y);
        /// <summary>
        /// Drawing Digital clock colon
        /// </summary>
        /// <param name="graphics">Graphics used for drawing the text</param>
        /// <param name="font">Font or the digital text</param>
        /// <param name="brush">brush used for drawing the highlighted lines</param>
        /// <param name="x"> x points for drawing the text</param>
        /// <param name="y">y points for drawing the text</param>
        float DrawColon(Graphics graphics, Font font, Brush brush, float x, float y);
        /// <summary>
        /// Drawing Digital clock polygon
        /// </summary>
        /// <param name="graphics">Graphics used for drawing the text</param>
        /// <param name="polygonPoints">polygonPoints used for drawing the colon</param>
        /// <param name="font">Font for the digital text</param>
        /// <param name="brush">brush used for drawing the colon</param>
        /// <param name="x"> x co-ordinate point for colon</param>
        /// <param name="y">y co-ordinate point for colon</param>
        void FillPolygon(Graphics graphics, Point[] polygonPoints, Font font, Brush brush, float x, float y);
        /// <summary>
        /// Drawing Digital clock background frame
        /// </summary>
        /// <param name="g">Graphics used for drawing the text and frame</param>
        /// <param name="newImage">Image for digital clock frame</param>
        /// <param name="Clock">clock used for drawing the digital clock depends on respected properties</param>
        void DrawDigitalClockFrame(Graphics g, Image newImage, Clock clock);
        //void DrawDigitalClockFrame(Graphics g, DateTime CustomTime, Image newImage, Font font, SizeF digitSizeF, ClockFrames ClockFrame, String digitText, Color digitColorint, Size size, Color c, bool ShowAMPM, bool isAM, bool ShowDateTime);
        /// <summary>
        /// Drawing Digital clock border
        /// </summary>
        /// <param name="g">Graphics used for drawing the text</param>
        /// <param name="Clock">clock used for drawing the digital clock depends on respected properties</param>
        void DrawDigitalClockBorder(Graphics g, Clock clock);
        /// <summary>
        /// Get rounded frame
        /// </summary>
        /// <param name="x">begining x-co-ordinate points for the digital clock</param>
        /// <param name="y">begining y-co-ordinate points for the digital clock</param>
        /// <param name="width">width used for measuring rounded region width</param>
        /// <param name="height">height used for measuring rounded region height</param>
        /// <param name="radius">radius of the rounded region</param>
        GraphicsPath GetRoundedregion(float x, float y, float width, float height, float radius);
    }
    public class DigitalClockRenderer : IDigitalClockRenderer
    {
        public DigitalClockRenderer()
        {
            //segments for 0-9
            segmentPoints[0] = new Point[] { new Point(3, 2), new Point(39, 2), new Point(31, 10), new Point(11, 10) };
            segmentPoints[1] = new Point[] { new Point(2, 3), new Point(10, 11), new Point(10, 31), new Point(2, 35) };
            segmentPoints[2] = new Point[] { new Point(40, 3), new Point(40, 35), new Point(32, 31), new Point(32, 11) };
            segmentPoints[3] = new Point[] { new Point(3, 36), new Point(11, 32), new Point(31, 32), new Point(39, 36), new Point(31, 40), new Point(11, 40) };
            segmentPoints[4] = new Point[] { new Point(2, 37), new Point(10, 41), new Point(10, 61), new Point(2, 69) };
            segmentPoints[5] = new Point[] { new Point(40, 37), new Point(40, 69), new Point(32, 61), new Point(32, 41) };
            segmentPoints[6] = new Point[] { new Point(11, 62), new Point(31, 62), new Point(39, 70), new Point(3, 70) };
           //Segments for AM and PM
            segmentPoints1[0] = new Point[] { new Point(3, 2), new Point(39, 2), new Point(31, 10), new Point(11, 10) };
            segmentPoints1[1] = new Point[] { new Point(2, 3), new Point(10, 11), new Point(10, 31), new Point(2, 35) };
            segmentPoints1[2] = new Point[] { new Point(40, 3), new Point(40, 35), new Point(32, 31), new Point(32, 11) };
            segmentPoints1[3] = new Point[] { new Point(3, 36), new Point(11, 32), new Point(31, 32), new Point(39, 36), new Point(31, 40), new Point(11, 40) };
            segmentPoints1[4] = new Point[] { new Point(2, 37), new Point(10, 41), new Point(10, 61), new Point(2, 69) };
            segmentPoints1[5] = new Point[] { new Point(40, 37), new Point(40, 69), new Point(32, 61), new Point(32, 41) };
            segmentPoints1[6] = new Point[] { new Point(11, 62), new Point(31, 62), new Point(39, 70), new Point(3, 70) };
            segmentPoints1[7] = new Point[] { new Point(36, 2), new Point(72, 2), new Point(62, 10), new Point(43, 10) };
            segmentPoints1[8] = new Point[] { new Point(64, 11), new Point(64, 31), new Point(72, 35), new Point(72, 3) };
            segmentPoints1[9] = new Point[] { new Point(64, 40), new Point(64, 59), new Point(72, 68), new Point(72, 36) };

            daySegmentPoints[0] = new Point[] { new Point(3, 2), new Point(39, 2), new Point(31, 10), new Point(11, 10) };
            daySegmentPoints[1] = new Point[] { new Point(2, 3), new Point(10, 11), new Point(10, 31), new Point(2, 35) };
            daySegmentPoints[2] = new Point[] { new Point(40, 3), new Point(40, 35), new Point(32, 31), new Point(32, 11) };
            daySegmentPoints[3] = new Point[] { new Point(3, 36), new Point(11, 32), new Point(31, 32), new Point(39, 36), new Point(31, 40), new Point(11, 40) };
            daySegmentPoints[4] = new Point[] { new Point(2, 37), new Point(10, 41), new Point(10, 61), new Point(2, 69) };
            daySegmentPoints[5] = new Point[] { new Point(40, 37), new Point(40, 69), new Point(32, 61), new Point(32, 41) };
            daySegmentPoints[6] = new Point[] { new Point(11, 62), new Point(31, 62), new Point(39, 70), new Point(3, 70) };
            daySegmentPoints[7] = new Point[] { new Point(17, 12), new Point(24, 12), new Point(24, 30), new Point(21, 33), new Point(17, 30) };
            daySegmentPoints[8] = new Point[] { new Point(17, 40),new Point(21,37), new Point(24, 40), new Point(24, 62), new Point(17, 62) };
            daySegmentPoints[9] = new Point[] { new Point(11, 12), new Point(28, 30), new Point(31, 45), new Point(14, 27) };
            daySegmentPoints[10] = new Point[] { new Point(25, 43), new Point(38, 68), new Point(28, 66), new Point(15, 42) };
        }
        /// <summary>
        /// Drawing Digital clock background frame
        /// </summary>
        /// <param name="g">Graphics used for drawing the text</param>
        /// <param name="newImage">Image for digital clock frame</param>
        /// <param name="clock">clock used for drawing the digital clock depends on respected properties</param>
        public virtual void DrawDigitalClockFrame(Graphics g, Image newImage, Clock clock)
        {
            GraphicsPath pat = new GraphicsPath();
            g.DrawImage(newImage, new Rectangle(0, 0, clock.Width, clock.Height));
            switch (clock.ClockFrame)
            {
                case ClockFrames.RectangularFrame:
                    clock.Height = clock.Width / 2;
                    pat.AddRectangle(new Rectangle(0, 0, clock.Width, clock.Height));
                    g.DrawPath(new Pen(clock.color), pat);
                    pat.Dispose();
                    break;
                case ClockFrames.CircularFrame:
                    clock.Height = clock.Width;
                    pat.AddEllipse(new Rectangle(0, 0, clock.Width, clock.Height));
                    g.DrawPath(new Pen(clock.color), pat);
                    pat.Dispose();
                    break;
                case ClockFrames.SquareFrame:
                    clock.Height = clock.Width;
                    g.DrawPath(new Pen(clock.color), GetRoundedregion(0, 0, clock.Width, clock.Height, clock.Width / 18));
                    pat.Dispose();
                    break;
            }
            using (SolidBrush brush = new SolidBrush(clock.ForeColor))
            {
                using (SolidBrush lightBrush = new SolidBrush(Color.FromArgb(20, clock.ForeColor)))
                {
                    switch (clock.ClockFrame)
                    {
                        case ClockFrames.RectangularFrame:
                            if (clock.DisplayDates)
                            {
                                DrawDigits(g, "  " + clock.CustomTime.Date.Day.ToString() + "/" + clock.CustomTime.Month.ToString(), clock.CustomTime, new Font("Times New Roman", (clock.font.Height / 9) / (g.DpiX / 96)), brush, lightBrush,new PointF( clock.Width / 4, (clock.Height / 2 - ((clock.digitSizeF.Height / 4) + clock.font.Height / 3))), false, clock.isAM);
                                DrawDigits(g, "Mon Tue Wed Thu Fri Sat Sun", clock.CustomTime, new Font("Times New Roman", (clock.font.Height / 8) / (g.DpiX / 96)), brush, lightBrush,new PointF( (clock.Width / 2 - clock.Width / 8) - 6 * (g.DpiX / 96), ((clock.Height / 2 + clock.font.Height / 6 + clock.digitSizeF.Height) - ((clock.digitSizeF.Height / 4) + clock.font.Height / 2))), false, clock.isAM);
                            }
                            DrawDigits(g, clock.DigitText, clock.CustomTime, new Font("Times New Roman", (clock.font.Height / 2) / (g.DpiX / 96)), brush, lightBrush,new PointF( clock.Width / 4, (clock.Height / 2 - ((clock.digitSizeF.Height / 4) + clock.font.Height / 8))), clock.ShowHourDesignator, clock.isAM);
                            break;
                        case ClockFrames.CircularFrame:
                            if (clock.DisplayDates)
                            {
                                DrawDigits(g, "   " + clock.CustomTime.Date.Day.ToString() + "/" + clock.CustomTime.Month.ToString(), clock.CustomTime, new Font("Times New Roman", (clock.font.Height / 6) / (g.DpiX / 96)), brush, lightBrush,new PointF( clock.Width / 4, (clock.Height / 2 - ((clock.digitSizeF.Height / 4) + clock.font.Height / 3))), false, clock.isAM);
                                DrawDigits(g, "Mon Tue Wed Thu Fri Sat Sun", clock.CustomTime, new Font("Times New Roman", (clock.font.Height / 9) / (g.DpiX / 96)), brush, lightBrush,new PointF( clock.Width / 2 - clock.Width / 8, ((clock.Height / 2 + clock.font.Height / 6 + clock.digitSizeF.Height) - ((clock.digitSizeF.Height / 4) + clock.font.Height / 16))), false, clock.isAM);
                            }
                            DrawDigits(g, clock.DigitText, clock.CustomTime, new Font("Times New Roman", (clock.font.Height / 2) / (g.DpiX / 96)), brush, lightBrush,new PointF( clock.Width / 4, (clock.Height / 2 - ((clock.digitSizeF.Height / 4) + clock.font.Height / 8))), clock.ShowHourDesignator, clock.isAM);
                            break;
                        case ClockFrames.SquareFrame:
                            if (clock.DisplayDates)
                            {
                                DrawDigits(g, "  " + clock.CustomTime.Date.Day.ToString() + "/" + clock.CustomTime.Month.ToString(), clock.CustomTime, new Font("Times New Roman", (clock.font.Height / 9) / (g.DpiX / 96)), brush, lightBrush,new PointF(clock.Width / 4, (clock.Height / 2 - ((clock.digitSizeF.Height / 4) + clock.font.Height / 3))), false, clock.isAM);
                                DrawDigits(g, "Mon Tue Wed Thu Fri Sat Sun", clock.CustomTime, new Font("Times New Roman", (clock.font.Height / 9) / (g.DpiX / 96)), brush, lightBrush, new PointF( clock.Width / 2 - clock.Width / 8, ((clock.Height / 2 + clock.font.Height / 6 + clock.digitSizeF.Height) - ((clock.digitSizeF.Height / 4) + clock.font.Height / 4))), false, clock.isAM);
                            }
                            DrawDigits(g, clock.DigitText, clock.CustomTime, new Font("Times New Roman", (clock.font.Height / 2) / (g.DpiX / 96)), brush, lightBrush,new PointF( clock.Width / 4, (clock.Height / 2 - ((clock.digitSizeF.Height / 4) + clock.font.Height / 8))), clock.ShowHourDesignator, clock.isAM);
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// Drawing Digital clock Text
        /// </summary>
        /// <param name="g">Graphics used for drawing the text</param>
        /// <param name="text">Text for the digital clock<" text "></param>
        /// <param name="CustomTime">CustomTime used for customizing the digital text</param>
        /// <param name="font">Font for digital text</param>
        /// <param name="brush">brush used for drawing the highlighted lines</param>
        /// <param name="brushLight">brushLight used for drawing the un highlighted lines</param>
        /// <param name="textPoint">  points for drawing the text</param>
        /// <param name="showAMPM">Show/Hide the AMPM</param>
        /// <param name="isAM">Display AM/PM</param>
        public void DrawDigits(Graphics graphics, string text, DateTime CustomTime, Font font, Brush brush, Brush brushLight,PointF textPoint, bool showAMPM, bool isAM)
        {
            textPoint.X = (textPoint.X + 42 * graphics.DpiX * font.SizeInPoints / 72 / 72) / 2;
            float length = 0;
            if (text.Length < 8)
            {
                if (text.Length == 5)
                {
                    length = 42 * graphics.DpiX * font.SizeInPoints / 72 / 72;
                    length += (12 * graphics.DpiX * font.SizeInPoints / 72 / 72) / 2;
                }
                else if (text.Length == 2)
                {
                    length = 2 * (42 * graphics.DpiX * font.SizeInPoints / 72 / 72);
                    length += (12 * graphics.DpiX * font.SizeInPoints / 72 / 72);
                }
                textPoint.X += length;
            }
            DateTime dt = CustomTime;
            string currentDay = dt.DayOfWeek.ToString();
            string[] splitDay = text.Split(' ');
            int numbersForColoringText = 0;
            if (splitDay.Length > 1)
            {
                for (int i = 0; i < splitDay.Length; i++)
                {
                    if (currentDay.Contains(splitDay[i]))
                    {
                        break;
                    }
                    numbersForColoringText++;
                }
            }
            for (int i = 0; i < text.Length; i++)
            {
                // For digits 0-9
                if (Char.IsDigit(text[i]))
                    textPoint.X = DrawDigit(graphics, text[i] - '0', font, brush, brushLight, textPoint.X, textPoint.Y);
                // For colon :
                else if (text[i] == ':')
                    textPoint.X = DrawColon(graphics, font, brush, textPoint.X, textPoint.Y);
                // For dot .
                else if (text[i] == '.')
                    textPoint.X = DrawDot(graphics, font, brush, textPoint.X, textPoint.Y);
                else if (text[i] == '/')
                    textPoint.X = DrawSlace(graphics, font, brush, textPoint.X, textPoint.Y);
                else if (text[i] == ' ')
                    textPoint.X = DrawSpace(graphics, font, brush, textPoint.X, textPoint.Y);
                else if (Char.IsLetter(text[i]))
                    if (i >= (numbersForColoringText) * 4 && (i < (numbersForColoringText) * 4 + 4))
                        textPoint.X = DrawDigit(graphics, text[i], font, brush, new SolidBrush(Color.FromArgb(80, Color.Red)), textPoint.X, textPoint.Y);
                    else
                        textPoint.X = DrawDigit(graphics, text[i], font, new SolidBrush(Color.Gray), brushLight, textPoint.X, textPoint.Y);
            }
            drawAM(graphics, font, brush, brushLight, textPoint.X, textPoint.Y - ((font.Height / 5)), showAMPM, isAM);
        }
        /// <summary>
        /// Drawing Digital clock AM and PM
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="font"></param>
        /// <param name="brush"></param>
        /// <param name="brushLight"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="showAMPM"></param>
        /// <param name="isAM"></param>
        private void drawAM(Graphics graphics, Font font, Brush brush, Brush brushLight, float x, float y ,bool showAMPM, bool isAM)
        {
            y = y - 2 * (graphics.DpiX / 96);
            if (showAMPM)
            {
                for (int i = 0; i < segmentPoints1.Length; i++)
                {
                    if (isAM)
                    {
                        if (i != 6 && i < 7)
                        {
                            FillPolygon(graphics, segmentPoints1[i], new Font("Times New Roman", (font.Height / 6 )/(graphics.DpiX /96)), brush,( x - (3 * font.Height) / 6 ), y);
                        }
                    }
                    else
                    {
                        if (i != 6 && i != 5 && i < 7)
                        {
                            FillPolygon(graphics, segmentPoints1[i], new Font("Times New Roman", (font.Height / 6) / (graphics.DpiX / 96)), brush, (x - (3 * font.Height) / 6), y);
                        }
                    }
                    if (i != 6 && i != 3 )
                    {
                        FillPolygon(graphics, segmentPoints1[i], new Font("Times New Roman", (font.Height / 6) / (graphics.DpiX / 96)), brush, (x - (3 * font.Height) / 6) + font.Height / 6, y);
                    }
                }
            }
        }
        /// <summary>
        /// Drawing Digital rounded frame
        /// </summary>
        /// <param name="g">Graphics used for drawing the digital clock border</param>
        /// <param name="p">p used for drawing the digial clock border</param>
        /// <param name="x">begining x-co-ordinate points for the digital clock</param>
        /// <param name="y">begining y-co-ordinate points for the digital clock</param>
        /// <param name="width">width used drawing the rounded border</param>
        /// <param name="height">height used drawing the rounded border</param>
        /// <param name="radius">radius used drawing the rounded border</param>
        public void DrawRoundRect(Graphics g, Pen p, float x, float y, float width, float height, float radius)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(x + radius, y, x + width - (radius * 2), y); // Line
            gp.AddArc(x + width - (radius * 2), y, radius * 2, radius * 2, 270, 90); // Corner
            gp.AddLine(x + width, y + radius, x + width, y + height - (radius * 2)); // Line
            gp.AddArc(x + width - (radius * 2), y + height - (radius * 2), radius * 2, radius * 2, 0, 90); // Corner
            gp.AddLine(x + width - (radius * 2), y + height, x + radius, y + height); // Line
            gp.AddArc(x, y + height - (radius * 2), radius * 2, radius * 2, 90, 90); // Corner
            gp.AddLine(x, y + height - (radius * 2), x, y + radius); // Line
            gp.AddArc(x, y, radius * 2, radius * 2, 180, 90); // Corner
            gp.CloseFigure();
            g.DrawPath(p, gp);
            gp.Dispose();
        }
        /// <summary>
        /// Get rounded frame
        /// </summary>
        /// <param name="x">begining x-co-ordinate points for the digital clock</param>
        /// <param name="y">begining y-co-ordinate points for the digital clock</param>
        /// <param name="width">width used for measuring rounded region width</param>
        /// <param name="height">height used for measuring rounded region height</param>
        /// <param name="radius">radius of the rounded region</param>
        public GraphicsPath GetRoundedregion(float x, float y, float width, float height, float radius)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(x + radius, y, x + width - (radius * 2), y); // Line
            gp.AddArc(x + width - (radius * 2), y, radius * 2, radius * 2, 270, 90); // Corner
            gp.AddLine(x + width, y + radius, x + width, y + height - (radius * 2)); // Line
            gp.AddArc(x + width - (radius * 2), y + height - (radius * 2), radius * 2, radius * 2, 0, 90); // Corner
            gp.AddLine(x + width - (radius * 2), y + height, x + radius, y + height); // Line
            gp.AddArc(x, y + height - (radius * 2), radius * 2, radius * 2, 90, 90); // Corner
            gp.AddLine(x, y + height - (radius * 2), x, y + radius); // Line
            gp.AddArc(x, y, radius * 2, radius * 2, 180, 90); // Corner
            gp.CloseFigure();
            return gp;
        }
        /// <summary>
        /// Drawing Digital clock border
        /// </summary>
        /// <param name="g">Graphics used for drawing the text</param>
        /// <param name="CustomTime">CustomTime used for customizing the digital text</param>
        /// <param name="shape">Drawing the clock with respected shape</param>
        /// <param name="digitalOuterColor">digitalOuterColor used for drawing the digital clock border</param>
        /// <param name="font">Font for digital text</param>
        /// <param name="Width">Width used for drawing the digital clock shape with respected width</param>
        /// <param name="Height">Heigt used for drawing the digital clock shape with respected Height</param>
        /// <param name="c">C used for drawing the graphics path</param>
        public void DrawDigitalClockBorder(Graphics g, Clock clock)
        {
            using (Pen pen = new Pen(clock.digitalOuterColor, 4))
            {
                using (GraphicsPath pat = new GraphicsPath())
                {
                    switch (clock.ClockShape)
                    {
                        case ClockShapes.Circle:
                            clock.Height = clock.Width;
                            g.FillEllipse(new SolidBrush(clock.BackgroundColor), new Rectangle(2, 2, clock.Width - 4, clock.Width - 4));
                            g.DrawEllipse(pen, new Rectangle(2, 2, clock.Width - 4, clock.Width - 4));
                            pat.AddEllipse(new Rectangle(0, 0, clock.Width, clock.Width));
                            g.DrawPath(new Pen(clock.color), pat);
                            break;
                        case ClockShapes.Square:
                            clock.Height = clock.Width;
                            g.FillRectangle(new SolidBrush(clock.BackgroundColor), new Rectangle(2, 2, clock.Width - 5, clock.Height - 5));
                            g.DrawRectangle(pen, new Rectangle(2, 2, clock.Width - 5, clock.Height - 5));
                            pat.AddRectangle(new Rectangle(0, 0, clock.Width - 1, clock.Height - 1));
                            g.DrawPath(new Pen(clock.color), pat);
                            break;
                        case ClockShapes.RoundedSquare:
                            clock.Height = clock.Width;
                            g.FillPath(new SolidBrush(clock.BackgroundColor), GetRoundedregion(2, 2, clock.Width - 4, clock.Height - 4, clock.Width / 6));
                            DrawRoundRect(g, pen, 2, 2, clock.Width - 4, clock.Height - 4, clock.Width / 6);
                            g.DrawPath(new Pen(clock.color), GetRoundedregion(0, 0, clock.Width, clock.Height, clock.Width / 6));
                            break;
                        case ClockShapes.Rectangle:
                            clock.Height = clock.Width / 2;
                            g.FillRectangle(new SolidBrush(clock.BackgroundColor), new Rectangle(2, 2, clock.Width - 4, clock.Height - 4));
                            g.DrawRectangle(pen, new Rectangle(2, 2, clock.Width - 4, clock.Height - 4));
                            pat.AddRectangle(new Rectangle(0, 0, clock.Width, clock.Height));
                            g.DrawPath(new Pen(clock.color), pat);
                            break;
                        case ClockShapes.RoundedRectangle:
                            clock.Height = clock.Width / 2;
                            g.FillPath(new SolidBrush(clock.BackgroundColor), GetRoundedregion(2, 2, clock.Width - 4, clock.Height - 4, clock.Width / 7));
                            DrawRoundRect(g, pen, 2, 2, clock.Width - 4, clock.Height - 4, clock.Width / 7);
                            g.DrawPath(new Pen(clock.color, 3), GetRoundedregion(0, 0, clock.Width, clock.Height, clock.Width / 7));
                            break;
                    }
                    if (clock.DisplayDates)
                    {
                        using (SolidBrush brush = new SolidBrush(clock.digitColor))
                        {
                            using (SolidBrush lightBrush = new SolidBrush(Color.FromArgb(20, clock.digitColor)))
                            {
                                DrawDigits(g, clock.CustomTime.Date.Day.ToString() + "/" + clock.CustomTime.Month.ToString(), clock.CustomTime, new Font("Times New Roman", (clock.font.Height / 6) / (g.DpiX / 96)), brush, lightBrush,new PointF( clock.Width / 8, (clock.Height / 2 - ((clock.digitSizeF.Height / 4) + clock.font.Height / 2))), false, false);
                                DrawDigits(g, "Mon Tue Wed Thu Fri Sat Sun", clock.CustomTime, new Font("Times New Roman", (clock.font.Height / 9) / (g.DpiX / 96)), brush, lightBrush,new PointF( clock.Width / 2 - clock.Width / 8, ((clock.Height / 2 + clock.font.Height / 6 + clock.digitSizeF.Height) - ((clock.digitSizeF.Height / 4) + clock.font.Height / 6))), false, false);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Drawing Digital clock Digit
        /// </summary>
        /// </summary>
        /// <param name="graphics">Graphics used for drawing the text</param>
        /// <param name="num">num for the digital clock<" text "></param>
        /// <param name="font">Font or the digital text</param>
        /// <param name="brush">brush used for drawing the highlighted lines</param>
        /// <param name="brushLight">brushLight used for drawing the un highlighted lines</param>
        /// <param name="x"> x points for drawing the text</param>
        /// <param name="y">y points for drawing the text</param>
        public float DrawDigit(Graphics graphics, int num, Font font, Brush brush, Brush brushLight, float x, float y)
        {
            for (int i = 0; i < segmentPoints.Length; i++)
            {
                if (segmentData[num, i] == 1)
                {
                    FillPolygon(graphics, segmentPoints[i], font, brush, x, y);
                }
                else
                {
                    FillPolygon(graphics, segmentPoints[i], font, brushLight, x, y);
                }
            }
            return x + 42 * graphics.DpiX * font.SizeInPoints / 72 / 72;
        }
        /// <summary>
        /// Drawing Digital clock Digit
        /// </summary>
        /// </summary>
        /// <param name="graphics">Graphics used for drawing the text</param>
        /// <param name="num">num for the digital clock<" text "></param>
        /// <param name="font">Font for the digital text</param>
        /// <param name="brush">brush used for drawing the highlighted lines</param>
        /// <param name="brushLight">brushLight used for drawing the un highlighted lines</param>
        /// <param name="x"> x points for drawing the text</param>
        /// <param name="y">y points for drawing the text</param>
        public float DrawDigit(Graphics graphics, char num, Font font, Brush brush, Brush brushLight, float x, float y)
        {
            
            switch (num)
            {
                case 'M':
                    for (int i = 0; i < daySegmentPoints.Length; i++)
                    {
                        if (textSegmentData[0, i] == 1)
                        {
                            FillPolygon(graphics, daySegmentPoints[i], font, brush, x, y);
                        }
                    }
                    break;
                case 'o':
                    for (int i = 0; i < daySegmentPoints.Length; i++)
                    {
                        if (textSegmentData[1, i] == 1)
                        {
                            FillPolygon(graphics, daySegmentPoints[i], font, brush, x, y);
                        }
                    }
                    break;
                case 'n':
                    for (int i = 0; i < daySegmentPoints.Length; i++)
                    {
                        if (textSegmentData[2, i] == 1)
                        {
                            FillPolygon(graphics, daySegmentPoints[i], font, brush, x, y);
                        }
                    }
                    break;
                case 'T':
                       for (int i = 0; i < daySegmentPoints.Length; i++)
                    {
                        if (textSegmentData[3, i] == 1)
                        {
                            FillPolygon(graphics, daySegmentPoints[i], font, brush, x, y);
                        }
                    }
                    break;
                case 't':
                    for (int i = 0; i < daySegmentPoints.Length; i++)
                    {
                        if (textSegmentData[3, i] == 1)
                        {
                            FillPolygon(graphics, daySegmentPoints[i], font, brush, x, y);
                        }
                    }
                    break;
                case 'h':
                       for (int i = 0; i < daySegmentPoints.Length; i++)
                    {
                        if (textSegmentData[4, i] == 1)
                        {
                            FillPolygon(graphics, daySegmentPoints[i], font, brush, x, y);
                        }
                    }
                    break;
                case 'u':
                       for (int i = 0; i < daySegmentPoints.Length; i++)
                    {
                        if (textSegmentData[5, i] == 1)
                        {
                            FillPolygon(graphics, daySegmentPoints[i], font, brush, x, y);
                        }
                    }
                    break;
                case 'W':
                        for (int i = 0; i < daySegmentPoints.Length; i++)
                    {
                        if (textSegmentData[6, i] == 1)
                        {
                            FillPolygon(graphics, daySegmentPoints[i], font, brush, x, y);
                        }
                    }
                    break;
                case 'e':
                        for (int i = 0; i < daySegmentPoints.Length; i++)
                    {
                        if (textSegmentData[7, i] == 1)
                        {
                            FillPolygon(graphics, daySegmentPoints[i], font, brush, x, y);
                        }
                    }
                    break;
                case 'd':
                    for (int i = 0; i < daySegmentPoints.Length; i++)
                    {
                        if (textSegmentData[8, i] == 1)
                        {
                            FillPolygon(graphics, daySegmentPoints[i], font, brush, x, y);
                        }
                    }
                    break;
                case 'F':
                    for (int i = 0; i < daySegmentPoints.Length; i++)
                    {
                        if (textSegmentData[9, i] == 1)
                        {
                            FillPolygon(graphics, daySegmentPoints[i], font, brush, x, y);
                        }
                    }
                    break;
                case 'r':
                    for (int i = 0; i < daySegmentPoints.Length; i++)
                    {
                        if (textSegmentData[10, i] == 1)
                        {
                            FillPolygon(graphics, daySegmentPoints[i], font, brush, x, y);
                        }
                    }
                    break;
                case 'i':
                    for (int i = 0; i < daySegmentPoints.Length; i++)
                    {
                        if (textSegmentData[11, i] == 1)
                        {
                            FillPolygon(graphics, daySegmentPoints[i], font, brush, x, y);
                        }
                    }
                    break;
                case 'S':
                    for (int i = 0; i < daySegmentPoints.Length; i++)
                    {
                        if (textSegmentData[12, i] == 1)
                        {
                            FillPolygon(graphics, daySegmentPoints[i], font, brush, x, y);
                        }
                    }
                    break;
                case 'a':
                    for (int i = 0; i < daySegmentPoints.Length; i++)
                    {
                        if (textSegmentData[13, i] == 1)
                        {
                            FillPolygon(graphics, daySegmentPoints[i], font, brush, x, y);
                        }
                    }
                    break;
            }
      
            return x + 47 * graphics.DpiX * font.SizeInPoints / 72 / 72;
        }
        /// <summary>
        /// segmentdata
        /// </summary>
        static byte[,] textSegmentData = {
                             {1, 1, 1, 0, 1, 1, 0, 1, 0, 0 ,0},//M
                             {1, 1, 1, 0, 1, 1, 1 ,0 ,0 ,0 ,0},//O
                             {0, 1, 1, 0, 1, 1, 0 , 0 ,0 ,1,0},//N
                             {1, 0, 0, 0, 0, 0, 0 , 1 ,1,0 ,0},//T
                             {0, 1, 1, 1, 1, 1, 0 , 0 ,0 ,0,0},//H
                             {0, 1, 1, 0, 1, 1, 1 , 0 ,0 ,0,0},//U
                             {0, 1, 1, 0, 1, 1, 1 , 0 ,1 ,0,0},//W
                             {1, 1, 0, 1, 1, 0, 1 , 0 ,0 ,0,0},//E
                             {1, 0, 1, 0, 0, 1, 1 , 1 ,1 ,0,0},//D
                             {1, 1, 0, 1, 1, 0, 0, 0 ,0 ,0 ,0},//F
                             {1, 1, 1, 1, 1, 0, 0 , 0 ,0 ,0,1},//R
                             {0, 0, 1, 0, 0, 1, 0 , 0 ,0 ,0,0},//I
                             {1, 1, 0, 1, 0, 1, 1 , 0 ,0 ,0,0},//s
                             {1, 1, 1, 1, 1, 1, 0 , 0 ,0 ,0,0}//A
                                         };

        /// <summary>
        /// Drawing Digital clock Dot
        /// </summary>
        /// <param name="graphics">Graphics used for drawing the text</param>
        /// <param name="font">Font or the digital text</param>
        /// <param name="brush">brush used for drawing the highlighted lines</param>
        /// <param name="x"> x points for drawing the text</param>
        /// <param name="y">y points for drawing the text</param>
        public float DrawDot( Graphics graphics ,Font font, Brush brush, float x, float y)
        {
            Point[][] dotPoints = new Point[1][];
            dotPoints[0] = new Point[] { new Point(2, 64), new Point(6, 61), new Point(10, 64), new Point(6, 69) };
            for (int i = 0; i < dotPoints.Length; i++)
            {
                FillPolygon(graphics, dotPoints[i], font, brush, x, y);
            }
            return x + 12 * graphics.DpiX * font.SizeInPoints / 72 / 72;
        }
        /// <summary>
        /// Drawing Digital clock Dot
        /// </summary>
        /// <param name="graphics">Graphics used for drawing the text</param>
        /// <param name="font">Font or the digital text</param>
        /// <param name="brush">brush used for drawing the highlighted lines</param>
        /// <param name="x"> x points for drawing the text</param>
        /// <param name="y">y points for drawing the text</param>
        public float DrawSpace(Graphics graphics, Font font, Brush brush, float x, float y)
        {
            //Point[][] dotPoints = new Point[1][];
            //dotPoints[0] = new Point[] { new Point(2, 64), new Point(6, 61), new Point(10, 64), new Point(6, 69) };
            //for (int i = 0; i < dotPoints.Length; i++)
            //{
            //    FillPolygon(graphics, dotPoints[i], font, brush, x, y);
            //}
            return x + 24 * graphics.DpiX * font.SizeInPoints / 72 / 72;
        }
        /// <summary>
        /// Drawing Digital clock Dot
        /// </summary>
        /// <param name="graphics">Graphics used for drawing the text</param>
        /// <param name="font">Font or the digital text</param>
        /// <param name="brush">brush used for drawing the highlighted lines</param>
        /// <param name="x"> x points for drawing the text</param>
        /// <param name="y">y points for drawing the text</param>
        public float DrawSlace(Graphics graphics, Font font, Brush brush, float x, float y)
        {
            Point[][] dotPoints = new Point[1][];
            dotPoints[0] = new Point[] { new Point(40, 12), new Point(25, 38), new Point(18, 34), new Point(33, 8) };
            for (int i = 0; i < dotPoints.Length; i++)
            {
                FillPolygon(graphics, dotPoints[i], font, brush, x, y);
            }
            dotPoints[0] = new Point[] { new Point(23, 41), new Point(7, 67), new Point(0, 63), new Point(16, 37) };
            for (int i = 0; i < dotPoints.Length; i++)
            {
                FillPolygon(graphics, dotPoints[i], font, brush, x, y);
            }
            return x + 42 * graphics.DpiX * font.SizeInPoints / 72 / 72;
        }
        /// <summary>
        /// Drawing Digital clock colon
        /// </summary>
        /// <param name="graphics">Graphics used for drawing the text</param>
        /// <param name="font">Font or the digital text</param>
        /// <param name="brush">brush used for drawing the highlighted lines</param>
        /// <param name="x"> x points for drawing the text</param>
        /// <param name="y">y points for drawing the text</param>
        public float DrawColon(Graphics graphics, Font font, Brush brush, float x, float y)
        {
            Point[][] colonPoints = new Point[2][];
            colonPoints[0] = new Point[] { new Point(2, 21), new Point(6, 17), new Point(10, 21), new Point(6, 25) };
            colonPoints[1] = new Point[] { new Point(2, 51), new Point(6, 47), new Point(10, 51), new Point(6, 55) };
            for (int i = 0; i < colonPoints.Length; i++)
            {
                FillPolygon( graphics,colonPoints[i], font, brush, x, y);
            }
            return x + 12 * graphics.DpiX * font.SizeInPoints / 72 / 72;
        }
        /// <summary>
        /// Drawing Digital clock polygon
        /// </summary>
        /// <param name="graphics">Graphics used for drawing the text</param>
        /// <param name="polygonPoints">polygonPoints used for drawing the colon</param>
        /// <param name="font">Font for the digital text</param>
        /// <param name="brush">brush used for drawing the colon</param>
        /// <param name="x"> x co-ordinate point for colon</param>
        /// <param name="y">y co-ordinate point for colon</param>
        public void FillPolygon(Graphics graphics, Point[] polygonPoints, Font font, Brush brush, float x, float y)
        {
            PointF[] polygonPointsF = new PointF[polygonPoints.Length];
            for (int i = 0; i < polygonPoints.Length; i++)
            {
                polygonPointsF[i].X = x + polygonPoints[i].X * graphics.DpiX * font.SizeInPoints / 72 / 72;
                polygonPointsF[i].Y = y + polygonPoints[i].Y * graphics.DpiY * font.SizeInPoints / 72 / 72;
            }
            graphics.FillPolygon(brush, polygonPointsF);
        }
        /// <summary>
        /// segmentdata
        /// </summary>
        static byte[,] segmentData = {{1, 1, 1, 0, 1, 1, 1},
							 {0, 0, 1, 0, 0, 1, 0},  
							 {1, 0, 1, 1, 1, 0, 1},  
							 {1, 0, 1, 1, 0, 1, 1},  
							 {0, 1, 1, 1, 0, 1, 0},  
							 {1, 1, 0, 1, 0, 1, 1},  
							 {1, 1, 0, 1, 1, 1, 1},  
							 {1, 0, 1, 0, 0, 1, 0},  
							 {1, 1, 1, 1, 1, 1, 1},  
							 {1, 1, 1, 1, 0, 1, 1}};
     
        // Points that define each of the seven segments
        Point[][] segmentPoints = new Point[7][];
        Point[][] segmentPoints1 = new Point[10][];
        Point[][] daySegmentPoints = new Point[11][];
        /// <summary>
        /// Get Digital text size
        /// </summary>
        /// <param name="graphics">graphics used for measuring the text size</param>
        /// <param name="text">text used for measuring digital clock width and height</param>
        /// <param name="font">font used measuring the digital clock size</param>
        public SizeF GetStringSize( Graphics graphics, string text, Font font)
        {
            SizeF sizef = new SizeF(0, graphics.DpiX * font.SizeInPoints / 72);
            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsDigit(text[i]))
                    sizef.Width += 42 * graphics.DpiX * font.SizeInPoints / 72 / 72;
                else if (text[i] == ':' || text[i] == '.')
                    sizef.Width += 12 * graphics.DpiX * font.SizeInPoints / 72 / 72;
            }
            return sizef;
        }
    }
    #endregion
}
