#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.Pdf.Native;

namespace Syncfusion.Pdf.Graphics.Images.Metafiles
{
    /// <summary>
    /// Help data during EMF metafiles parsing.
    /// </summary>
    internal class EmfObjectData : IDisposable
    {
#region Constants
        /// <summary>
        /// Number of 0.01 millimeter per inch.
        /// </summary>
        private const float UnitsInInch = 2540.0f;
        #endregion

#region Fields
        /// <summary>
        /// Collection of created objects.
        /// </summary>
        private EmfObjectCollection m_objects;

        /// <summary>
        /// Unmanaged handle used by the most of the GDI WinApi functions.
        /// </summary>
        private IntPtr m_handle;

        /// <summary>
        /// Font object.
        /// </summary>
        private Font m_font;

        /// <summary>
        /// Font object.
        /// </summary>
        private Brush m_brush;

        /// <summary>
        /// Pen object.
        /// </summary>
        private Pen m_pen;

        /// <summary>
        /// Current graphics path object.
        /// </summary>
        private GraphicsPath m_path;

        /// <summary>
        /// Image resource.
        /// </summary>
        private Bitmap m_image;

        /// <summary>
        /// Graphic state of the graphics context.
        /// </summary>
        private GraphicsState m_state;

        /// <summary>
        /// Graphics object.
        /// </summary>
        private System.Drawing.Graphics m_graphics;

        /// <summary>
        /// Indicates if there is open graphics path object.
        /// </summary>
        private bool m_bOpenPath;

        /// <summary>
        /// Angle of the text.
        /// </summary>
        private float m_textAngle;

        /// <summary>
        /// Cointrex stack.
        /// </summary>
        private Stack m_contStack;

        /// <summary>
        /// Bitmap for getting graphics from it.
        /// </summary>
        private Bitmap m_bmp;

        /// <summary>
        /// REsolution of the screen.
        /// </summary>
        private PointF m_defResolution;
        #endregion

#region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EmfObjectData"/> class.
        /// </summary>
        /// <param name="dpi">The dpi.</param>
        public EmfObjectData(SizeF dpi)
        {
            m_bmp = new Bitmap(1, 1);
            m_defResolution = dpi.ToPointF();
        }
        #endregion

#region Properties
        /// <summary>
        /// Gets collection of selected objects.
        /// </summary>
        public EmfObjectCollection SelectedObjects
        {
            get
            {
                if (m_objects == null)
                {
                    m_objects = new EmfObjectCollection();
                }

                return m_objects;
            }
        }

        /// <summary>
        /// Gets unmanaged handle used by the most of the GDI WinApi functions.
        /// </summary>
        public IntPtr Handle
        {
            get
            {
                if (m_handle == IntPtr.Zero)
                {
                    m_handle = Graphics.GetHdc();
                }

                return m_handle;
            }
        }

        /// <summary>
        /// Gets or sets graphic state of the graphics context.
        /// </summary>
        public GraphicsState GraphicsState
        {
            get
            {
                return m_state;
            }
            
            set
            {
                m_state = value;
            }
        }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>The font.</value>
        public Font Font
        {
            get
            {
                if (m_font == null)
                {
                    m_font = SelectedObjects.GetStockObject(STOCK.DEFAULT_GUI_FONT) as Font;
                }

                return m_font;
            }
            
            set
            {
                if (m_font != value)
                {
                    m_font = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets current brush object.
        /// </summary>
        public Brush Brush
        {
            get
            {
                if (m_brush == null)
                {
                    m_brush = SelectedObjects.GetStockObject(STOCK.DC_BRUSH) as Brush;
                }

                return m_brush;
            }
          
            set
            {
                if (m_brush != value)
                {
                    m_brush = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets current pen object.
        /// </summary>
        public Pen Pen
        {
            get
            {
                if (m_pen == null)
                {
                    m_pen = SelectedObjects.GetStockObject(STOCK.DC_PEN) as Pen;
                }

                return m_pen;
            }
            
            set
            {
                if (m_pen != value)
                {
                    m_pen = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets current graphics path.
        /// </summary>
        public GraphicsPath Path
        {
            get
            {
                return m_path;
            }
           
            set
            {
                if (m_path != value)
                {
                    if (m_path != null && !SelectedObjects.IsStockObject(m_path))
                    {
                        m_path.Dispose();
                    }

                    m_path = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets image object.
        /// </summary>
        public Bitmap Image
        {
            get
            {
                return m_image;
            }
           
            set
            {
                if (m_image != value)
                {
                    m_image = value;
                }
            }
        }

        /// <summary>
        /// Gets graphics object.
        /// </summary>
        public System.Drawing.Graphics Graphics
        {
            get
            {
                if (m_graphics == null)
                {
                    m_graphics = System.Drawing.Graphics.FromImage(m_bmp);
                }

                return m_graphics;
            }
        }

        /// <summary>
        /// Gets or sets the current point at DC.
        /// </summary>
        public PointF CurrentPoint
        {
            get
            {
                POINT prevPoint = new POINT();
                bool result = GdiApi.MoveToEx(Handle, 0, 0, ref prevPoint);
                MetafileParser.CheckResult(result);

                if (result)
                {
                    POINT tmpPoint = new POINT();
                    result = GdiApi.MoveToEx(Handle, prevPoint.x, prevPoint.y, ref tmpPoint);
                    MetafileParser.CheckResult(result);
                }

                return prevPoint;
            }
          
            set
            {
                POINT tmpPoint = new POINT();
                int x = (int)value.X;
                int y = (int)value.Y;
                bool result = GdiApi.MoveToEx(Handle, x, y, ref tmpPoint);
                MetafileParser.CheckResult(result);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open path.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is open path; otherwise, <c>false</c>.
        /// </value>
        public bool IsOpenPath
        {
            get
            {
                return (m_bOpenPath && Path != null);
            }
           
            set
            {
                if (m_bOpenPath != value)
                {
                    m_bOpenPath = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets arc direction of current device context.
        /// </summary>
        public AD_ANGLEDIRECTION ArcDirection
        {
            get
            {
                int direction = 0;

                direction = GdiApi.GetArcDirection(Handle);

                if (direction == 0)
                {
                    MetafileParser.CheckResult(false);
                }

                return ((AD_ANGLEDIRECTION)direction);
            }
           
            set
            {
                int result = GdiApi.SetArcDirection(Handle, (int)value);

                if (result == 0)
                {
                    MetafileParser.CheckResult(false);
                }
            }
        }

        /// <summary>
        /// Gets or sets text align.
        /// </summary>
        public TA_TEXT_ALIGN TextAlign
        {
            get
            {
                int iTextAlign = GdiApi.GetTextAlign(Handle);

                return ((TA_TEXT_ALIGN)iTextAlign);
            }
           
            set
            {
                GdiApi.SetTextAlign(Handle, (int)value);
            }
        }

        /// <summary>
        /// Gets or sets Text color.
        /// </summary>
        public Color ForeColor
        {
            get
            {
                int iColor = GdiApi.GetTextColor(Handle);
                Color color = ColorTranslator.FromWin32(iColor);

                return color;
            }
          
            set
            {
                int iColor = ColorTranslator.ToWin32(value);
                GdiApi.SetTextColor(Handle, iColor);
            }
        }

        /// <summary>
        /// Gets or sets back color.
        /// </summary>
        public Color BackColor
        {
            get
            {
                int iColor = GdiApi.GetBkColor(Handle);
                Color color = ColorTranslator.FromWin32(iColor);

                return color;
            }
           
            set
            {
                int iColor = ColorTranslator.ToWin32(value);
                GdiApi.SetBkColor(Handle, iColor);
            }
        }

        /// <summary>
        /// Gets or sets polygon fill mode.
        /// </summary>
        public System.Drawing.Drawing2D.FillMode FillMode
        {
            get
            {
                System.Drawing.Drawing2D.FillMode mode = System.Drawing.Drawing2D.FillMode.Winding;
                int iMode = GdiApi.GetPolyFillMode(Handle);

                if (iMode > 0)
                {
                    mode = (System.Drawing.Drawing2D.FillMode)(--iMode);
                }
                else
                {
                    MetafileParser.CheckResult(false);
                }

                return mode;
            }
          
            set
            {
                int iMode = (int)value + 1;
                int result = GdiApi.SetPolyFillMode(Handle, iMode);

                if (result == 0)
                {
                    MetafileParser.CheckResult(false);
                }
            }
        }

        /// <summary>
        /// Gets default resolution of the screen.
        /// </summary>
        public PointF Resolution
        {
            get
            {
                return m_defResolution;
            }
        }

        /// <summary>
        /// Gets or sets angle of the text.
        /// </summary>
        public float TextAngle
        {
            get
            {
                return m_textAngle;
            }
           
            set
            {
                if (m_textAngle != value)
                {
                    m_textAngle = value;
                }
            }
        }

        /// <summary>
        /// Gets context stack object.
        /// </summary>
        private Stack ContextStack
        {
            get
            {
                if (m_contStack == null)
                {
                    m_contStack = new Stack();
                }

                return m_contStack;
            }
        }
        #endregion

#region IDisposable
        /// <summary>
        /// Disposes object.
        /// </summary>
        public void Dispose()
        {
            if (Font != null && !SelectedObjects.IsStockObject(Font))
            {
                Font.Dispose();
            }

            if (Pen != null && !SelectedObjects.IsStockObject(Pen))
            {
                Pen.Dispose();
            }

            if (Brush != null && !SelectedObjects.IsStockObject(Brush))
            {
                Brush.Dispose();
            }

            if (Path != null && !SelectedObjects.IsStockObject(Path))
            {
                Path.Dispose();
            }

            if (m_handle != IntPtr.Zero && m_graphics != null)
            {
                m_graphics.ReleaseHdc(m_handle);
                m_graphics.Dispose();
                m_graphics = null;
                m_bmp.Dispose();
            }

            if (m_bmp != null)
            {
                m_bmp.Dispose();
                m_bmp = null;
            }

            if (m_contStack != null)
            {
                m_contStack.Clear();
                m_contStack = null;
            }

            DisposeSelectedObjects();

            m_font = null;
            m_pen = null;
            m_brush = null;
            m_path = null;
        }
        #endregion

#region Public Methods
        /// <summary>
        /// Recognizes selected object.
        /// </summary>
        /// <param name="obj">Selected object.</param>
        public void SelectObject(object obj)
        {
            if (obj != null)
            {
                Pen pen = obj as Pen;

                if (pen != null)
                {
                    Pen = pen;
                    return;
                }

                Brush brush = obj as Brush;

                if (brush != null)
                {
                    Brush = brush;
                    return;
                }

                FontEx fontEx = obj as FontEx;

                if (fontEx != null)
                {
                    Font = fontEx.Font;
                    TextAngle = fontEx.Angle;
                    return;
                }

                Font font = obj as Font;

                if (font != null)
                {
                    Font = font;
                    TextAngle = 0f;
                    return;
                }
            }
        }

        /// <summary>
        /// Deletes object from the context.
        /// </summary>
        /// <param name="obj">Object to be deleted.</param>
        public void DeleteObject(object obj)
        {
            if (obj != null)
            {
                Pen pen = obj as Pen;

                if (pen != null)
                {
                    pen.Dispose();
                    return;
                }

                Brush brush = obj as Brush;

                if (brush != null)
                {
                    brush.Dispose();
                    return;
                }

                FontEx fontEx = obj as FontEx;

                if (fontEx != null)
                {
                    fontEx.Dispose();
                    return;
                }

                Font font = obj as Font;

                if (font != null)
                {
                    font.Dispose();
                    return;
                }
            }
        }

        /// <summary>
        /// Saves state to context stack.
        /// </summary>
        public void Save()
        {
            EmfObjectData data = new EmfObjectData(new SizeF(m_defResolution.X, m_defResolution.Y));
            CopyTo(data);

            ContextStack.Push(data);
        }

        /// <summary>
        /// Restores state from context stack.
        /// </summary>
        /// <param name="index">Index in the stack.</param>
        public void Restore(int index)
        {
            int level;

            if (index < 0)
            {
                level = Math.Min(-index, ContextStack.Count);
            }
            else
            {
                level = Math.Max(ContextStack.Count - index, 0);
            }

            if (level > 0)
            {
                EmfObjectData data = null;

                while (level-- != 0)
                {
                    data = (EmfObjectData)ContextStack.Pop();
                }

                data.CopyTo(this);
            }
        }
        #endregion

#region Implementation
        /// <summary>
        /// Copies data from current object to specified.
        /// </summary>
        /// <param name="data">Destination data object.</param>
        private void CopyTo(EmfObjectData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            data.m_objects = m_objects;
            data.m_handle = m_handle;
            data.m_font = m_font;
            data.m_brush = m_brush;
            data.m_pen = m_pen;
            data.m_path = m_path;
            data.m_image = m_image;
            data.m_state = m_state;
            data.m_graphics = m_graphics;
            data.m_bOpenPath = m_bOpenPath;
            data.m_textAngle = m_textAngle;
            data.m_contStack = m_contStack;
        }

        /// <summary>
        /// Disposes selected objects.
        /// </summary>
        private void DisposeSelectedObjects()
        {
            if (SelectedObjects.CreatedGraphicObjects.Count > 0)
            {
                foreach (object key in SelectedObjects.CreatedGraphicObjects.Keys)
                {
                    object val = SelectedObjects.CreatedGraphicObjects[key];

                    if (val != null)
                    {
                        DeleteObject(val);
                    }
                }
            }

            SelectedObjects.Clear();
        }
        #endregion
    }

    /// <summary>
    /// Class holding font and it's rotating angle.
    /// </summary>
    internal class FontEx : IDisposable
    {
#region Fields
        /// <summary>
        /// Font object.
        /// </summary>
        private Font m_font;

        /// <summary>
        /// Structure describing font.
        /// </summary>
        private LOGFONT m_structure;
        #endregion

#region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FontEx"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="structure">The structure.</param>
        public FontEx(Font font, LOGFONT structure)
        {
            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            m_font = font;
            m_structure = structure;
        }
        #endregion

#region Properties
        /// <summary>
        /// Gets font object.
        /// </summary>
        public Font Font
        {
            get
            {
                return m_font;
            }
        }

        /// <summary>
        /// Gets text rotating angle.
        /// </summary>
        public float Angle
        {
            get
            {
                // Get angle of the text.
                float angle = -(float)(m_structure.lfEscapement / 10f);

                return angle;
            }
        }

        /// <summary>
        /// Gets LOGFONT structure from which font was created.
        /// </summary>
        public LOGFONT LogFont
        {
            get
            {
                return m_structure;
            }
        }
        #endregion

#region IDisposable
        /// <summary>
        /// Disposes object.
        /// </summary>
        public void Dispose()
        {
            if (m_font != null)
            {
                m_font.Dispose();
                m_font = null;
            }
        }
        #endregion
    }
}
#endif