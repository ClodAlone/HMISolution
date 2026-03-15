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
using System.Collections;
using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;

using Syncfusion.DocIO.DLS;
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Class is responsible for saving the collection of text
    /// for each shape object (textbox, rectangle, etc.).
    /// </summary>
    [CLSCompliant(false)]
    public class ShapeObjectTextCollection
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<int, WTextBox> m_textTable = new Dictionary<int, WTextBox>();
        #endregion

        #region Class initialize finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeObjectTextCollection"/> class.
        /// </summary>
        public ShapeObjectTextCollection()
        { }
        #endregion

        #region Class public methods
        /// <summary>
        /// Add ShapeObject's text to text collection.
        /// </summary>
        /// <param name="shapeId">The shape id.</param>
        /// <param name="textBox">The text box.</param>
        public void AddTextBox(int shapeId, WTextBox textBox)
        {
            if (m_textTable == null)
                m_textTable = new Dictionary<int, WTextBox>();
            m_textTable.Add(shapeId, textBox);
        }

        /// <summary>
        /// Get ShapeObject's text body by text identifier.
        /// </summary>
        /// <param name="shapeId">The shape id.</param>
        /// <returns></returns>
        public WTextBox GetTextBox(int shapeId)
        {
            WTextBox textBox = null;

            if (m_textTable.ContainsKey(shapeId))
            {
                textBox = m_textTable[shapeId];
                m_textTable.Remove(shapeId);
            }

            return textBox;
        }
        #endregion
    }
}
