#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Diagram
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
        private EmfRenderer m_renderer;

        /// <summary>
        /// Asociated with parser context object.
        /// </summary>
        private object m_context;

        /// <summary>
        /// Metafile settings.
        /// </summary>
        private MetafileSettings m_settings = new MetafileSettings();

        /// <summary>
        /// Parsing metafile object.
        /// </summary> 
        private Metafile m_metaFile;
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
        public EmfRenderer Renderer
        {
            get
            {
                return m_renderer;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Renderer");

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
        /// Gets or sets internal metafile settings.
        /// </summary>
        protected MetafileSettings Settings
        {
            get
            {
                if (m_settings == null)
                    throw new ArgumentNullException("Settings");

                return m_settings;
            }
            set
            {
                m_settings = value;
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
                    throw new ArgumentNullException("Metafile");

                if (m_metaFile != value)
                {
                    m_metaFile = value;
                }
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Raises before the metafile parses.
        /// </summary>
        internal abstract event ParseEventHandler BeginParse;
        #endregion

        #region Delegates
        /// <summary>
        /// Delegate. Is used for raising events before metafile start parsing.
        /// </summary>
        /// <param name="sender">The object sender.</param>
        /// <param name="args">The arguments.</param>
        internal delegate void ParseEventHandler(object sender, EventArgs args);
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="MetafileParser"/> class.
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
        public MetafileParser(EmfRenderer renderer)
        {
            if (renderer == null)
                throw new ArgumentNullException("renderer");

            m_renderer = renderer;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public virtual void Dispose()
        {
            m_enumerateHandler = null;

            // m_renderer = null;
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
        protected abstract Graphics.EnumerateMetafileProc CreateParsingHandler();

        /// <summary>
        /// Initialize internal metafile settings.
        /// (Records count, etc.).
        /// </summary>
        public abstract void InitializeSettings();

        /// <summary>
        /// Raises <see cref="BeginParse"/> event.
        /// Called when the metafile is parsed.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        internal abstract void OnBeginParse(EventArgs e);
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
                throw new ArgumentNullException("data");

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
