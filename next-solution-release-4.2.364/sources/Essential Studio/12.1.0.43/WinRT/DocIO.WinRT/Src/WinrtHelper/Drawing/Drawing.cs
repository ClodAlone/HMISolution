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
using System.Threading.Tasks;
using Windows;
using Windows.Foundation;

#if DOCIO
namespace Syncfusion.DocIO.DLS
#else
namespace System.Drawing
#endif
{
    public class SizeF
    {
       private float m_Height;
       private float m_Width;
       public float Height
       {
           get 
           {
               return m_Height;
           }
           set
           {
               m_Height = value;
           }
       }
       public float Width
       {
           get
           {
               return m_Width;
           }
           set
           {
               m_Width = value;
           }
       }
       public SizeF()
       {
       }
       public SizeF(float width, float height)
       {
           this.Height = height;
           this.Width = width;
       }
    }
    public class Size
    {
        private int m_Height;
        private int m_Width;
        public int Height
        {
            get
            {
                return m_Height;
            }
            set
            {
                m_Height = value;
            }
        }
        public int Width
        {
            get
            {
                return m_Width;
            }
            set
            {
                m_Width = value;
            }
        }
        public Size()
        {
        }
        public Size(int width, int height)
        {
            this.Height = height;
            this.Width = width;
        }
    }
    public class PointF
    {
       //Point point = new Point();
        
        private float m_X;
        private float m_Y;
        public float X
        {
          
            get
            {
                return m_X;
            }
            set
            {
                m_X = value;
            }
        }
        public float Y
        {
            get
            {
                return m_Y;
            }
            set
            {
                m_Y = value;
            }
        }
        public PointF()
        {
        }
        public PointF(float x, float y)
        {
            this.m_X = x;
            this.m_Y = y;
        }
    }
    public class RectangleF
    {
        // Point point = new Point();
        private Rect m_rect;
        public float X
        {
            get
            {
                return Convert.ToSingle(m_rect.X);
            }
            set
            {
                m_rect.Y = value;
            }
        }
        public float Y
        {
            get
            {
                return Convert.ToSingle(m_rect.Y);
            }
            set
            {
                m_rect.Y = value;
            }
        }
        public float Height
        {
            get
            {
                return Convert.ToSingle(m_rect.Height);
            }
            set
            {
                m_rect.Height = value;
            }
        }
        public float Width
        {
            get
            {
                return Convert.ToSingle(m_rect.Width);
            }
            set
            {
                m_rect.Width = value;
            }
        }
        public RectangleF()
        {
        }
        public RectangleF(float x, float y, float width, float height)
        {
            //this.X = x;
            //this.Y = y;
            //this.Height = Height;
            //this.Width = Width;
            m_rect = new Rect(x, y, width, height);
        }
    }
    public class Rectangle
    {
        // Point point = new Point();
        private Rect m_rect;
        public int X
        {
            get
            {
                return Convert.ToInt32(m_rect.X);
            }
            set
            {
                m_rect.X = value;
            }
        }
        public int Y
        {
            get
            {
                return Convert.ToInt32(m_rect.Y);
            }
            set
            {
                m_rect.Y = value;
            }
        }
        public int Height
        {
            get
            {
                return Convert.ToInt32(m_rect.Height);
            }
            set
            {
                m_rect.Height = value;
            }
        }
        public int Width
        {
            get
            {
                return Convert.ToInt32(m_rect.Width);
            }
            set
            {
                m_rect.Width = value;
            }
        }
        public Rectangle()
        {
        }
        public Rectangle(int x, int y, int width, int height)
        {
            //this.X = x;
            //this.Y = y;
            //this.Height = Height;
            //this.Width = Width;
            m_rect = new Rect(x, y, width, height);
        }
    }
}
