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

namespace Syncfusion.PdfViewer.Base
{
    /// <summary>
    /// TODO:  Can we use PdfMargins class instead of this ?
    /// </summary>
    internal class PageMargin
    {
        
        private const float m_margin = 0f;  

        #region Fields
        private float m_left;
        private float m_top;
        private float m_right;
        private float m_bottom;
        #endregion

      
        public float Left
        {
            get
            {
                return m_left;
            }
            set
            {
                m_left = value;
            }
        }

       
        public float Top
        {
            get
            {
                return m_top;
            }
            set
            {
                m_top = value;
            }
        }

       
        public float Right
        {
            get
            {
                return m_right;
            }
            set
            {
                m_right = value;
            }
        }

       
        public float Bottom
        {
            get
            {
                return m_bottom;
            }
            set
            {
                m_bottom = value;
            }
        }

  
        public float All
        {
            set
            {
                SetMargins(value);
            }
        }
     
     
        public PageMargin()
        {
            SetMargins(m_margin);
        }      

     
        internal void SetMargins(float margin)
        {
            m_left = m_top = m_right = m_bottom = margin;
        }
     
        internal void SetMargins(float leftRight, float topBottom)
        {
            m_left = m_right = leftRight;
            m_top = m_bottom = topBottom;
        }
      
        internal void SetMargins(float left, float top, float right, float bottom)
        {
            m_left = left;
            m_top = top;
            m_right = right;
            m_bottom = bottom;
        }

    }
}
