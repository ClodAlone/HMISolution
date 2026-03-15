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

namespace Syncfusion.Pdf
{
    class ImagePointer
    {
        private int m_x, m_y, m_width, m_height;
        private JBIG2Image m_bitmap;

        internal ImagePointer(JBIG2Image bitmap)
        {
            this.m_bitmap = bitmap;
            this.m_height = bitmap.Height;
            this.m_width = bitmap.Width;
        }

        internal void SetPointer(int x, int y)
        {
            this.m_x = x;
            this.m_y = y;
        }

        internal int NextPixel()
        {
            if (m_y < 0 || m_y >= m_height || m_x >= m_width)
            {
                return 0;
            }
            else if (m_x < 0)
            {
                m_x++;
                return 0;
            }
            int pixel = m_bitmap.GetPixel(m_x, m_y);
            m_x++;
            return pixel;
        }
    }
}
