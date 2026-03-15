#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Helping class that allows to control the graphic handles.
    /// It controls numbering for such handles that are changing 
    /// during actions such as SelectObject, DeleteObject
    /// </summary>
    internal class EmfObjectCollection
    {
        #region Constants
        /// <summary>
        /// Flag checking if object is system or created by user.
        /// </summary>
        private const uint StockFlag = 0x80000000;

        /// <summary>
        /// Help flag for retrieving object from the stock.
        /// </summary>
        private const int StockModifFlag = 0x7fffffff;
        #endregion

        #region Static fields
        /// <summary>
        /// The collection of standard graphic objects that can be used currently.
        /// </summary>
        private static Hashtable s_standartGraphicObjects;
        #endregion

        #region Fields
        /// <summary>
        /// Holds the collection of created graphic objects that can be used currently
        /// </summary>
        private Hashtable m_createdGraphicObjects;

        /// <summary>
        /// Holds the list of avaible indexes
        /// </summary>
        private ArrayList m_avaibleIndexes;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the collection of created graphic objects that can be used currently
        /// </summary>
        protected internal Hashtable CreatedGraphicObjects
        {
            get
            {
                if (m_createdGraphicObjects == null)
                {
                    m_createdGraphicObjects = new Hashtable();
                }

                return m_createdGraphicObjects;
            }
        }

        /// <summary>
        /// Gets the list of avaible indexes
        /// </summary>
        private ArrayList AvaibleIndexes
        {
            get
            {
                if (m_avaibleIndexes == null)
                {
                    m_avaibleIndexes = new ArrayList();
                }

                return m_avaibleIndexes;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="EmfObjectCollection"/> class.
        /// </summary>
        static EmfObjectCollection()
        {
            s_standartGraphicObjects = new Hashtable();

            s_standartGraphicObjects.Add((int)STOCK.WHITE_BRUSH, Brushes.White.Clone() as Brush);
            s_standartGraphicObjects.Add((int)STOCK.LTGRAY_BRUSH, Brushes.LightGray.Clone() as Brush);
            s_standartGraphicObjects.Add((int)STOCK.GRAY_BRUSH, Brushes.Gray.Clone() as Brush);
            s_standartGraphicObjects.Add((int)STOCK.DKGRAY_BRUSH, Brushes.DarkGray.Clone() as Brush);
            s_standartGraphicObjects.Add((int)STOCK.BLACK_BRUSH, Brushes.Black.Clone() as Brush);
            s_standartGraphicObjects.Add((int)STOCK.NULL_BRUSH, Brushes.Transparent.Clone() as Brush);
            s_standartGraphicObjects.Add((int)STOCK.WHITE_PEN, Pens.White.Clone() as Pen);
            s_standartGraphicObjects.Add((int)STOCK.BLACK_PEN, Pens.Black.Clone() as Pen);
            s_standartGraphicObjects.Add((int)STOCK.NULL_PEN, Pens.Transparent.Clone() as Pen);
            s_standartGraphicObjects.Add((int)STOCK.OEM_FIXED_FONT, Control.DefaultFont.Clone() as Font);
            s_standartGraphicObjects.Add((int)STOCK.ANSI_FIXED_FONT, Control.DefaultFont.Clone() as Font);
            s_standartGraphicObjects.Add((int)STOCK.ANSI_VAR_FONT, Control.DefaultFont.Clone() as Font);
            s_standartGraphicObjects.Add((int)STOCK.SYSTEM_FONT, Control.DefaultFont.Clone() as Font);
            s_standartGraphicObjects.Add((int)STOCK.DEVICE_DEFAULT_FONT, Control.DefaultFont.Clone() as Font);
            s_standartGraphicObjects.Add((int)STOCK.DEFAULT_PALETTE, Control.DefaultFont.Clone() as Font);
            s_standartGraphicObjects.Add((int)STOCK.SYSTEM_FIXED_FONT, Control.DefaultFont.Clone() as Font);
            s_standartGraphicObjects.Add((int)STOCK.DEFAULT_GUI_FONT, Control.DefaultFont.Clone() as Font);
            s_standartGraphicObjects.Add((int)STOCK.DC_BRUSH, Brushes.White.Clone() as Brush);
            s_standartGraphicObjects.Add((int)STOCK.DC_PEN, Pens.Black.Clone() as Pen);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds the object under specified unique index
        /// </summary>
        /// <param name="value">object to be added</param>
        /// <param name="index">unique index for the object</param>
        public void AddObject(object value, int index)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            CreatedGraphicObjects[index] = value;
        }

        /// <summary>
        /// Adds the object under specified unique index
        /// </summary>
        /// <param name="value">object to be added</param>
        public void AddObject(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            int index = 0;

            // the highest priority for assigning the indexes has m_avaibleIndexes array.
            if (AvaibleIndexes.Count > 0)
            {
                // take the first avaible in the list
                index = (int)AvaibleIndexes[0];
                AvaibleIndexes.RemoveAt(0);
            }
            else
            {
                index = CreatedGraphicObjects.Count;
            }

            AddObject(value, index);
        }

        /// <summary>
        /// Selects object by the unique index
        /// </summary>
        /// <param name="index">unique index for the object</param>
        /// <returns>selected object</returns>
        public object SelectObject(int index)
        {
            object result = null;

            if (!IsInStock(index))
            {
                if (AvaibleIndexes.Contains(index))
                {
                    AvaibleIndexes.Remove(index);
                }

                result = CreatedGraphicObjects[index];
            }
            else
            {
                result = GetStockObjectMasked(index);
            }

            return result;
        }

        /// <summary>
        /// Deletes objects
        /// </summary>
        /// <param name="index">unique index for the object to be deleted</param>
        /// <returns>Deleted object.</returns>
        public object DeleteObject(int index)
        {
            object obj = CreatedGraphicObjects[index];
            CreatedGraphicObjects[index] = null;

            if (!AvaibleIndexes.Contains(index))
            {
                AvaibleIndexes.Add(index);
            }

            return obj;
        }

        /// <summary>
        /// Cleares collection of selected objects.
        /// </summary>
        public void Clear()
        {
            if (m_createdGraphicObjects != null)
            {
                m_createdGraphicObjects.Clear();
            }
            if (m_avaibleIndexes != null)
            {
                m_avaibleIndexes.Clear();
            }
        }

        /// <summary>
        /// Checks if object is stock object.
        /// </summary>
        /// <param name="value">Object for checking.</param>
        /// <returns>True -if object is in stock, False otherwise.</returns>
        public bool IsStockObject(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return s_standartGraphicObjects.ContainsValue(value);
        }

        /// <summary>
        /// Retrieves object from the stock.
        /// </summary>
        /// <param name="objId">ID of the object.</param>
        /// <returns>Object from the stock if found, Null otherwise.</returns>
        public object GetStockObject(STOCK objId)
        {
            return s_standartGraphicObjects[(int)objId];
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Checks if object is in the stock or it's created by user.
        /// </summary>
        /// <param name="objId">ID of the object.</param>
        /// <returns>True - if object is in the stock, False otherwise.</returns>
        private bool IsInStock(int objId)
        {
            return ((objId & StockFlag) != 0);
        }

        /// <summary>
        /// Retrieves object from the stock.
        /// </summary>
        /// <param name="objId">ID of the object.</param>
        /// <returns>Object from the stock if found, Null otherwise.</returns>
        private object GetStockObjectMasked(int objId)
        {
            objId &= StockModifFlag;

            return s_standartGraphicObjects[objId];
        }
        #endregion
    }
}
