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

#region file using directives
using System;

using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser
{
    /// <summary>
    /// Summary description for PictureShapeProps.
    /// </summary>
    [CLSCompliant(false)]
    internal class PictureShapeProps : BaseProps
    {
        #region Class initialize/finalize methods
        internal PictureShapeProps()
        { }
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private float m_brightness = 50;
        private float m_contrast = 50;
        private PictureColor m_color;
        private string m_altText;
        //    private float m_cropLeft;
        //    private float m_cropRight;
        //    private float m_cropTop;
        //    private float m_cropBottom;
        #endregion

        #region Class proparties
        /// <summary>
        /// Gets/sets picture brightness.
        /// </summary>
        internal float PictureBrightness
        {
            get
            {
                return m_brightness;
            }
            set
            {
                m_brightness = value;
            }
        }
        /// <summary>
        /// Get/set picture contrast.
        /// </summary>
        internal float PictureContrast
        {
            get
            {
                return m_contrast;
            }
            set
            {
                m_contrast = value;
            }
        }
        /// <summary>
        /// Get/set picture color
        /// </summary>
        internal PictureColor PictureColor
        {
            get
            {
                return m_color;
            }
            set
            {
                m_color = value;
            }
        }
        /// <summary>
        /// Gets or sets the picture's alternative text.
        /// </summary>
        /// <value>The alternative text.</value>
        internal string AlternativeText
        {
            get
            {
                return m_altText;
            }
            set
            {
                m_altText = value;
            }
        }
        //    /// <summary>
        //    /// Get/set crop from left value.
        //    /// </summary>
        //    internal float CropFromLeft
        //    {
        //      get
        //      {
        //        return m_cropLeft;
        //      }
        //      set
        //      {
        //        m_cropLeft = value;
        //      }
        //    }
        //    /// <summary>
        //    /// Get/set crop from right value.
        //    /// </summary>
        //    internal float CropFromRight
        //    {
        //      get
        //      {
        //        return m_cropRight;
        //      }
        //      set
        //      {
        //        m_cropRight = value;
        //      }
        //    }
        //    /// <summary>
        //    /// Get/set crop from top value.
        //    /// </summary>
        //    internal float CropFromTop
        //    {
        //      get
        //      {
        //        return m_cropTop;
        //      }
        //      set
        //      {
        //        m_cropTop = value;
        //      }
        //    }
        //    /// <summary>
        //    /// Get/set crop from bottom value.
        //    /// </summary>
        //    internal float CropFromBottom
        //    {
        //      get
        //      {
        //        return m_cropBottom;
        //      }
        //      set
        //      {
        //        m_cropBottom = value;
        //      }
        //    }

        #endregion
    }
}
