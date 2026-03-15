#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
# region file using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
   

    # region interface

    public interface ISplitButtonRenderer
    {
        # region property
        /// <summary>
        /// Property for SplitButton
        /// </summary>
        SplitButton SplitButton
        {
            get;
            set;
        }
        #endregion

        void DrawText(PaintEventArgs e, string text, Font font, Color color, int totalwidth , int totalheight,int splitwidth);
        void DrawBorder(PaintEventArgs e, int width, int height, int splitwidth, Color outerColor, Color  innerColor, Color arrowOuter, Color  arrowInner, Color buttonInner);
        void DrawArrow(int left, int top, int width, int height, PaintEventArgs e, Color ArrowColor);
    }
    # endregion

    #region SplitButtonRenderer
    public class SplitButtonRenderer: ISplitButtonRenderer
    {
        # region Classmembers

        /// <summary>
        /// Color code for Button's Default State
        /// </summary>
        public string[] DefaultColor = new string[13]{ "#D2D2D2", "#D4D4D4", "#D6D6D6", "#D8D8D8", "#DADADA", "#DBDBDB", "#DDDDDD", "#EBEBEB", "#ECECEC", "#EDEDED", "#EFEFEF", "#F0F0F0", "#F1F1F1" };
        /// <summary>
        /// Color code for Button's Pressed State
        /// </summary>
        public string[] PressedColor = new string[13]{ "#78BDE2", "#7FC2E5", "#86C6E8", "#8CCAEB", "#93CEED", "#98D1EF", "#C4E5F6", "#C9E7F7", "#CEE9F8", "#D3ECF9", "#D8EEFA", "#DDF0FA", "#E1F2FB" };
        /// <summary>
        ///  Color code for Button's Actived State
        /// </summary>
        public string[] OnoverColor = new string[13] { "#ACDCF7", "#AFDEF8", "#B2E0F9", "#B5E2FA", "#B9E3FB", "#BCE5FC", "#BEE6FD", "#D9F0FC", "#DCF1FC", "#DEF2FC", "#E1F3FC", "#E4F4FC", "#E6F5FD" };
        /// <summary>
        ///  Color code for Button's Disabled State
        /// </summary>
        public string[] DisabledColor = new string[13] { "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", };
        /// <summary>
        /// Initializing Solid Brush to paint the button
        /// </summary>
        public SolidBrush ArrowColorDefault;
   
        # endregion

        # region Methods
         
        /// <summary>
        /// Arrow have painted, and its size is depends upon the width and heigh.
        /// And location is depends upon the left and top value
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="e"></param>
        /// <param name="ArrowColor"></param>
        public void DrawArrow(int left, int top,int width, int height, PaintEventArgs e, Color ArrowColor)
           {
               int StartPoint = left + 5;
               int EndPoint = StartPoint  +width/2 ;
               int HeightPoint = height;
               int TopPoint =  height/2 -2 ;
               ArrowColorDefault = new SolidBrush(ArrowColor) ;
               e.Graphics.FillPolygon(ArrowColorDefault, new Point[] { new Point(StartPoint, TopPoint ), 
                                                         new Point(EndPoint,TopPoint ),
                                                         new Point(Convert.ToInt32((StartPoint + EndPoint )/2) ,TopPoint  + 5) });
           }

           /// <summary>
           /// Creating the Border appearence of the Button And the Visual Style may achive throgh the Colors as an input
           /// </summary>
           /// <param name="e"></param>
           /// <param name="width"></param>
           /// <param name="height"></param>
           /// <param name="splitwidth"></param>
           /// <param name="outerColor"></param>
           /// <param name="innerColor"></param>
           /// <param name="arrowOuter"></param>
           /// <param name="arrowInner"></param>
           /// <param name="buttonInner"></param>
           public void DrawBorder(PaintEventArgs e, int width, int height, int splitwidth, Color outerColor, Color  innerColor, Color arrowOuter, Color  arrowInner, Color buttonInner)
           {
               Pen outercolor = new Pen (outerColor );
               Pen innercolor = new Pen(innerColor);
               Pen arrowinner = new Pen(arrowInner);
               Pen arrowouter = new Pen(arrowOuter);
               Pen buttoninner = new Pen(buttonInner);
                           
               e.Graphics.DrawLine(innercolor, new Point(1, 1), new Point(width - 2, 1));
               e.Graphics.DrawLine(innercolor, new Point(width - 2, 1), new Point(width - 2, height - 2));
               e.Graphics.DrawLine(innercolor, new Point(1, height - 2), new Point(width - 2, height - 2));
               e.Graphics.DrawLine(innercolor, new Point(1, 1), new Point(1, height - 2));
            
               e.Graphics.DrawLine(arrowouter, new Point(width - splitwidth, 0), new Point(width - splitwidth, height - 1));
               
               e.Graphics.DrawLine(buttoninner, new Point(width - splitwidth - 1, 2), new Point(width - splitwidth - 1, height - 3));

               e.Graphics.DrawRectangle(arrowinner, width - splitwidth + 1, 1, splitwidth - 3, height - 3);
                
               e.Graphics.DrawLine(outercolor, new Point(1, 0), new Point(width - 2, 0));
               e.Graphics.DrawLine(outercolor, new Point(width - 2, 0), new Point(width - 2, 1));
               e.Graphics.DrawLine(outercolor, new Point(width-1, 1), new Point(width-1, height-2));
               e.Graphics.DrawLine(outercolor, new Point(width - 2, height-2), new Point(width - 2, height-1));
               e.Graphics.DrawLine(outercolor, new Point(1, height -1), new Point(width - 2, height - 1));
               e.Graphics.DrawLine(outercolor, new Point(1, height - 1), new Point(1, height - 2));
               e.Graphics.DrawLine(outercolor, new Point(0, 1), new Point(0, height-2));
               e.Graphics.DrawLine(outercolor, new Point(1, 0), new Point(1, 1));
               
               buttoninner.Dispose();
               innercolor.Dispose();
               arrowinner.Dispose();
               arrowinner.Dispose();
               outercolor.Dispose();
         }

        /// <summary>
        /// Rendering of text.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="color"></param>
        /// <param name="textwidth"></param>
        /// <param name="totalwidth"></param>
        /// <param name="totalheight"></param>
        /// <param name="splitwidth"></param>
        public void DrawText(PaintEventArgs e, string text, Font font, Color color, int totalwidth , int totalheight,int splitwidth)
        {
           
            SolidBrush brush = new SolidBrush(color);
            StringFormat format = new StringFormat();
            format.Trimming = StringTrimming.EllipsisCharacter ;
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;
            Rectangle textArea = new Rectangle(1,1, totalwidth - splitwidth, totalheight );
            e.Graphics.DrawString(text, font, brush,textArea,format  );
            brush.Dispose();
        }

        # endregion

        #region ISplitButtonRenderer Members

        /// <summary>
        /// Instance for SplitButton
        /// </summary>
        private SplitButton splitButton;

        /// <summary>
        /// Get or Set the SplitButton Value
        /// </summary>
        public SplitButton SplitButton
        {
            get
            {
                return splitButton;
            }
            set
            {
                splitButton = value;
            }
        }

        #endregion
    }
    #endregion


    #region MetroSplitButtonRenderer
    public class MetroSplitButtonRenderer : ISplitButtonRenderer
    {
        # region Classmembers

        /// <summary>
        /// Color code for Button's Default State
        /// </summary>
        public string[] DefaultColor = new string[13] { "#D2D2D2", "#D4D4D4", "#D6D6D6", "#D8D8D8", "#DADADA", "#DBDBDB", "#DDDDDD", "#EBEBEB", "#ECECEC", "#EDEDED", "#EFEFEF", "#F0F0F0", "#F1F1F1" };
        /// <summary>
        /// Color code for Button's Pressed State
        /// </summary>
        public string[] PressedColor = new string[13] { "#78BDE2", "#7FC2E5", "#86C6E8", "#8CCAEB", "#93CEED", "#98D1EF", "#C4E5F6", "#C9E7F7", "#CEE9F8", "#D3ECF9", "#D8EEFA", "#DDF0FA", "#E1F2FB" };
        /// <summary>
        ///  Color code for Button's Actived State
        /// </summary>
        public string[] OnoverColor = new string[13] { "#ACDCF7", "#AFDEF8", "#B2E0F9", "#B5E2FA", "#B9E3FB", "#BCE5FC", "#BEE6FD", "#D9F0FC", "#DCF1FC", "#DEF2FC", "#E1F3FC", "#E4F4FC", "#E6F5FD" };
        /// <summary>
        ///  Color code for Button's Disabled State
        /// </summary>
        public string[] DisabledColor = new string[13] { "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", "#F4F4F4", };
        /// <summary>
        /// Initializing Solid Brush to paint the button
        /// </summary>
        public SolidBrush ArrowColorDefault;

        # endregion

        # region Methods

        /// <summary>
        /// Arrow have painted, and its size is depends upon the width and heigh.
        /// And location is depends upon the left and top value
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="e"></param>
        /// <param name="ArrowColor"></param>
        public void DrawArrow(int left, int top, int width, int height, PaintEventArgs e, Color ArrowColor)
        {
            int StartPoint = left + 5;
            int EndPoint = StartPoint + width / 2;
            int HeightPoint = height;
            int TopPoint = height / 2 - 2;
            ArrowColorDefault = new SolidBrush(ArrowColor);
            e.Graphics.FillPolygon(ArrowColorDefault, new Point[] { new Point(StartPoint, TopPoint ), 
                                                         new Point(EndPoint,TopPoint ),
                                                         new Point(Convert.ToInt32((StartPoint + EndPoint )/2) ,TopPoint  + 5) });
        }

        /// <summary>
        /// Creating the Border appearence of the Button And the Visual Style may achive throgh the Colors as an input
        /// </summary>
        /// <param name="e"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="splitwidth"></param>
        /// <param name="outerColor"></param>
        /// <param name="innerColor"></param>
        /// <param name="arrowOuter"></param>
        /// <param name="arrowInner"></param>
        /// <param name="buttonInner"></param>
        public void DrawBorder(PaintEventArgs e, int width, int height, int splitwidth, Color outerColor, Color innerColor, Color arrowOuter, Color arrowInner, Color buttonInner)
        {
            //Pen outercolor = new Pen(outerColor);
            //Pen innercolor = new Pen(innerColor);
            //Pen arrowinner = new Pen(arrowInner);
            //Pen arrowouter = new Pen(arrowOuter);
            //Pen buttoninner = new Pen(buttonInner);

            //e.Graphics.DrawLine(innercolor, new Point(1, 1), new Point(width - 2, 1));
            //e.Graphics.DrawLine(innercolor, new Point(width - 2, 1), new Point(width - 2, height - 2));
            //e.Graphics.DrawLine(innercolor, new Point(1, height - 2), new Point(width - 2, height - 2));
            //e.Graphics.DrawLine(innercolor, new Point(1, 1), new Point(1, height - 2));

            //e.Graphics.DrawLine(arrowouter, new Point(width - splitwidth, 0), new Point(width - splitwidth, height - 1));

            //e.Graphics.DrawLine(buttoninner, new Point(width - splitwidth - 1, 2), new Point(width - splitwidth - 1, height - 3));

            //e.Graphics.DrawRectangle(arrowinner, width - splitwidth + 1, 1, splitwidth - 3, height - 3);

            ////e.Graphics.DrawLine(outercolor, new Point(1, 0), new Point(width - 2, 0));
            ////e.Graphics.DrawLine(outercolor, new Point(width - 2, 0), new Point(width - 2, 1));
            ////e.Graphics.DrawLine(outercolor, new Point(width - 1, 1), new Point(width - 1, height - 2));
            ////e.Graphics.DrawLine(outercolor, new Point(width - 2, height - 2), new Point(width - 2, height - 1));
            ////e.Graphics.DrawLine(outercolor, new Point(1, height - 1), new Point(width - 2, height - 1));
            ////e.Graphics.DrawLine(outercolor, new Point(1, height - 1), new Point(1, height - 2));
            ////e.Graphics.DrawLine(outercolor, new Point(0, 1), new Point(0, height - 2));
            ////e.Graphics.DrawLine(outercolor, new Point(1, 0), new Point(1, 1));

            //buttoninner.Dispose();
            //innercolor.Dispose();
            //arrowinner.Dispose();
            //arrowinner.Dispose();
            //outercolor.Dispose();
        }

        /// <summary>
        /// Rendering of text.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="color"></param>
        /// <param name="textwidth"></param>
        /// <param name="totalwidth"></param>
        /// <param name="totalheight"></param>
        /// <param name="splitwidth"></param>
        public void DrawText(PaintEventArgs e, string text, Font font, Color color, int totalwidth, int totalheight, int splitwidth)
        {

            SolidBrush brush = new SolidBrush(color);
            StringFormat format = new StringFormat();
            format.Trimming = StringTrimming.EllipsisCharacter;
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;
            Rectangle textArea = new Rectangle(1, 1, totalwidth - splitwidth, totalheight);
            e.Graphics.DrawString(text, font, brush, textArea, format);
            brush.Dispose();
            format.Dispose();
        }

        # endregion

        #region ISplitButtonRenderer Members

        /// <summary>
        /// Instance for SplitButton
        /// </summary>
        private SplitButton splitButton;

        /// <summary>
        /// Get or Set the SplitButton Value
        /// </summary>
        public SplitButton SplitButton
        {
            get
            {
                return splitButton;
            }
            set
            {
                splitButton = value;
            }
        }

        #endregion
    }
    #endregion
}
