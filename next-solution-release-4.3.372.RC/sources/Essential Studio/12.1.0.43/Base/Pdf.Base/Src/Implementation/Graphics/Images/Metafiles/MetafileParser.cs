#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Syncfusion.Pdf.Native;

namespace Syncfusion.Pdf.Graphics.Images.Metafiles
{
    internal abstract class MetafileParser
    {
#region Constants
        /// <summary>
        /// Size of Int32 type.
        /// </summary>
        protected static readonly int IntSize;

        /// <summary>
        /// Size of Short type.
        /// </summary>
        protected static readonly int ShortSize;

        /// <summary>
        /// Size of Single type.
        /// </summary>
        protected static readonly int FloatSize;

        /// <summary>
        /// Number of numbers in the point type.
        /// </summary>
        protected const byte PointNumber = 2;

        /// <summary>
        /// Number of numbers in the rectangle type.
        /// </summary>
        protected const byte RectNumber = 4;
        #endregion

#region Fields
        /// <summary>
        /// Handler of function parsing metafile.
        /// </summary>
        private System.Drawing.Graphics.EnumerateMetafileProc m_enumerateHandler;

        /// <summary>
        /// Graphics context object.
        /// </summary>
        private PdfEmfRenderer m_renderer;

        /// <summary>
        /// Asociated with parser context object.
        /// </summary>
        private object m_context;

        /// <summary>
        /// Asociated with parser context object.
        /// </summary>
        private object m_imageContext;

        /// <summary>
        /// Parsing metafile object.
        /// </summary> 
        private Metafile m_metaFile;

        /// <summary>
        /// 
        /// </summary>
        private float m_pageScale;
        /// <summary>
        /// 
        /// </summary>
        private GraphicsUnit m_pageUnit;
        #endregion

#region Constructors
        /// <summary>
        /// Initializes the <see cref="MetafileParser"/> class.
        /// </summary>
        static MetafileParser()
        {
            IntSize = Marshal.SizeOf(typeof(Int32));
            ShortSize = Marshal.SizeOf(typeof(Int16));
            FloatSize = Marshal.SizeOf(typeof(Single));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetafileParser"/> class.
        /// </summary>
        public MetafileParser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetafileParser"/> class.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        public MetafileParser(PdfEmfRenderer renderer)
        {
            if (renderer == null)
            {
                throw new ArgumentNullException("renderer");
            }

            m_renderer = renderer;
        }

        /// <summary>
        /// Disposes object.
        /// </summary>
        public virtual void Dispose()
        {
            m_enumerateHandler = null;
            m_renderer = null;
        }
        #endregion

#region Properties
        /// <summary>
        /// Gets handler of parsing method.
        /// </summary>
        public System.Drawing.Graphics.EnumerateMetafileProc ParsingHandler
        {
            get
            {
                if (m_enumerateHandler == null)
                {
                    m_enumerateHandler = CreateParsingHandler();
                }

                return m_enumerateHandler;
            }
        }

        /// <summary>
        /// Gets or sets Graphics context.
        /// </summary>
        public PdfEmfRenderer Renderer
        {
            get
            {
                return m_renderer;
            }
            
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Renderer");
                }

                if (m_renderer != value)
                {
                    m_renderer = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets context of the parser.
        /// </summary>
        public object Context
        {
            get
            {
                return m_context;
            }
            
            set
            {
                if (m_context != value)
                {
                    m_context = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets context of the parser.
        /// </summary>
        public object ImageContext
        {
            get
            {
                return m_imageContext;
            }
          
            set
            {
                if (m_imageContext != value)
                {
                    m_imageContext = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the parsing metafile object.
        /// </summary>
        public Metafile Metafile
        {
            get
            {
                return m_metaFile;
            }
           
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Metafile");
                }

                if (m_metaFile != value)
                {
                    m_metaFile = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the page scale.
        /// </summary>
        /// <value>The page scale.</value>
        public float PageScale
        {
            get
            {
                return m_pageScale;
            }
            set
            {
                m_pageScale = value;
            }
        }

        /// <summary>
        /// Gets or sets the page unit.
        /// </summary>
        /// <value>The page unit.</value>
        public GraphicsUnit PageUnit
        {
            get
            {
                return m_pageUnit;
            }
            set
            {
                m_pageUnit = value;
            }
        }
        #endregion

#region Static methods
        /// <summary>
        /// Check result of function. If function failed - writes debug message.
        /// </summary>
        /// <param name="result">Result of the function.</param>
        protected internal static void CheckResult(bool result)
        {
            if (!result)
            {
                uint errCode = KernelApi.GetLastError();
                Debug.WriteLine("Function failed with code: " + errCode);
            }
        }
        #endregion

#region Abstract methods
        /// <summary>
        /// Gets type of metafile parser is able to parse.
        /// </summary>
        public abstract MetafileType Type
        {
            get;
        }

        /// <summary>
        /// Creates handler of parsing function.
        /// </summary>
        /// <returns>Handler of parsing function.</returns>
        protected abstract System.Drawing.Graphics.EnumerateMetafileProc CreateParsingHandler();

        #endregion

#region Implementation
        /// <summary>
        /// Reads number from the array.
        /// </summary>
        /// <param name="data">Array of data.</param>
        /// <param name="index">Index in the array.</param>
        /// <param name="step">Size of the number.</param>
        /// <returns>Number form the array.</returns>
        protected float ReadNumber(byte[] data, int index, int step)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            float result = 0.0f;

            // Short
            if (step == ShortSize)
            {
                result = BitConverter.ToInt16(data, index);
            }
            else if (step == FloatSize)
            {
                result = BitConverter.ToSingle(data, index);
            }

            return result;
        }

        #endregion
    }
}
#endif