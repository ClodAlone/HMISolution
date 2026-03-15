#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.DirectXWrapper.WinRT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.Pdf
{
    internal class CffGlyphs
    {
        /// <summary>
        ///  Variable to hold the glyph string and its glyph shapes 
        /// </summary>
        private Dictionary<string, byte[]> m_glyphs = new Dictionary<string, byte[]>();
        /// <summary>
        ///  Variable to hold the font matrix
        /// </summary>
        private double[] m_fontMatrix;
        /// <summary>
        ///  Variable to hold the character code and character
        /// </summary>
        private Dictionary<int, string> m_differenceEncoding = new Dictionary<int, string>();
        /// <summary>
        ///  Variable to hold the already cached path
        /// </summary>
        private Dictionary<string, GeometryGroup> m_renderedPath = new Dictionary<string, GeometryGroup>();

        /// <summary>
        /// Gets or sets the glyph string and its shapes 
        /// </summary>
        internal Dictionary<string, byte[]> Glyphs
        {
            get
            {
                return m_glyphs;
            }
            set
            {
                m_glyphs = value;
            }
        }

        /// <summary>
        /// Gets or sets the font matrix
        /// </summary>
        internal double[] FontMatrix
        {
            get
            {
                return m_fontMatrix;
            }
            set
            {
                m_fontMatrix = value;
            }
        }

        /// <summary>
        /// Gets or sets the character code and character
        /// </summary>
        internal Dictionary<int, string> DifferenceEncoding
        {
            get
            {
                return m_differenceEncoding;
            }
            set
            {
                m_differenceEncoding = value;
            }
        }

        /// <summary>
        /// Gets or sets the already cached path
        /// </summary>
        internal Dictionary<string, GeometryGroup> RenderedPath
        {
            get
            {
                return m_renderedPath;
            }
            set
            {
                m_renderedPath = value;
            }

        }
    }
}
