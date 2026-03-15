#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP
using System;
using System.Drawing;

namespace Syncfusion.Pdf.Graphics.Images.Metafiles
{
    /// <summary>
    /// Summary description for ObjectData.
    /// </summary>
    internal class ObjectData
    {
#region Constants
        /// <summary>
        /// Mask for object index recognizing.
        /// </summary>
        private const int IndexMask = 0xff;
        #endregion

#region Fields
        /// <summary>
        /// Stores collection of the GDI objects.
        /// </summary>
        private object[] m_objects;
        /// <summary>
        /// Holds all graphigcs states.
        /// </summary>
        private object[] m_states;

        /// <summary>
        /// Internal graphics context.
        /// </summary>
        private System.Drawing.Graphics m_graphics;
        /// <summary>
        /// Internal image object.
        /// </summary>
        private Image m_bmp;
        #endregion

#region Properties
        /// <summary>
        /// Gets internal graphics context.
        /// </summary>
        public System.Drawing.Graphics Graphics
        {
            get
            {
                return m_graphics;
            }
        }
        #endregion

#region Constructors
        /// <summary>
        /// Creates a new object.
        /// </summary>
        public ObjectData()
        {
            m_bmp = new Bitmap(1, 1);
            m_graphics = System.Drawing.Graphics.FromImage(m_bmp);
            m_objects = new object[256];
            m_states = new object[256];

        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            DisposeObjects();
        }
        #endregion

#region Class Public methods
        /// <summary>
        /// Gets font by its index.
        /// </summary>
        /// <param name="index">Index of the object in the table of GDI objects.</param>
        /// <returns>Font by its index.</returns>
        public Font GetFont(int index)
        {
            index &= IndexMask;
            Font result = GetObject(index) as Font;

            return result;
        }

        /// <summary>
        /// Gets brush by its index.
        /// </summary>
        /// <param name="index">Index of the object in the table of GDI objects.</param>
        /// <returns>Brush by its index.</returns>
        public Brush GetBrush(int index)
        {
            index &= IndexMask;
            Brush result = GetObject(index) as Brush;

            return result;
        }

        /// <summary>
        /// Gets the pen from the collection.
        /// </summary>
        /// <param name="index">The index of the pen within the collection.</param>
        /// <returns>The pen.</returns>
        public Pen GetPen(int index)
        {
            index &= IndexMask;

            Pen pen = GetObject(index) as Pen;

            return pen;
        }

        /// <summary>
        /// Sets the pen.
        /// </summary>
        /// <param name="index">The index of the pen object.</param>
        /// <param name="pen">The pen which should be stored..</param>
        public void SetPen(int index, Pen pen)
        {
            index &= IndexMask;
            SetObject(index, pen);
        }

        /// <summary>
        /// Gets object by its index.
        /// </summary>
        /// <param name="index">Index of the object in the table of GDI objects.</param>
        /// <returns>Object by its index.</returns>
        public object GetObject(int index)
        {
            object result = null;

            index &= IndexMask;

            if (index >= 0 || index < m_objects.Length)
            {
                result = m_objects[index];
            }

            return result;
        }

        /// <summary>
        /// Sets object to the collection.
        /// </summary>
        /// <param name="index">Index of the object in the collection.</param>
        /// <param name="obj">GDI object.</param>
        public void SetObject(int index, object obj)
        {
            index &= IndexMask;
            if (index >= 0 && index < m_objects.Length && obj != null)
            {
                // Get an old object.
                IDisposable oldObj = m_objects[index] as IDisposable;
                if (oldObj != null)
                {
                    oldObj.Dispose();
                }

                // Store a new value.
                m_objects[index] = obj;
            }
        }

        /// <summary>
        /// Gets the state by its index.
        /// </summary>
        /// <param name="index">The index of the state.</param>
        /// <returns>The graphics state stored previously.</returns>
        public object GetState(int index)
        {
            object state = null;

            index &= IndexMask;

            if (index >= 0 || index < m_states.Length)
            {
                state = m_states[index];
            }

            return state;
        }

        /// <summary>
        /// Sets the state with it index.
        /// </summary>
        /// <param name="index">The index of the state.</param>
        /// <param name="state">The state.</param>
        public void SetState(int index, object state)
        {
            index &= IndexMask;
            if (index >= 0 && index < m_states.Length && state != null)
            {
                // Get old state.
                IDisposable oldObj = m_states[index] as IDisposable;
                if (oldObj != null)
                {
                    oldObj.Dispose();
                }

                // Store new state.
                m_states[index] = state;
            }
        }
        #endregion

#region Implementation
        /// <summary>
        /// Disposes collection of the GDI objects.
        /// </summary>
        private void DisposeObjects()
        {
            if (m_objects != null)
            {
                for (int i = 0, len = m_objects.Length; i < len; i++)
                {
                    object obj = m_objects[i];
                    IDisposable dispObj = obj as IDisposable;

                    if (dispObj != null)
                    {
                        dispObj.Dispose();
                        m_objects[i] = null;
                    }
                }

                m_objects = null;
            }
        }
        #endregion
    }
}
#endif