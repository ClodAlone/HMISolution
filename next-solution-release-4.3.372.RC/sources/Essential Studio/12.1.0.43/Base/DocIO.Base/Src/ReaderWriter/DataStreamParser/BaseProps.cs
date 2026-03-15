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

using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser
{
    /// <summary>
    /// Summary description for BaseProps.
    /// </summary>
    [CLSCompliant(false)]
    internal class BaseProps : FileShapeAddress
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        protected ShapeHorizontalAlignment m_horAlignment = ShapeHorizontalAlignment.None;
        protected ShapeVerticalAlignment m_vertAlignment = ShapeVerticalAlignment.None;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal BaseProps()
        { }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets / sets shape horizontal alignment.
        /// </summary>
        internal ShapeHorizontalAlignment HorizontalAlignment
        {
            get
            {
                return m_horAlignment;
            }
            set
            {
                m_horAlignment = value;
            }
        }
        /// <summary>
        /// Gets / sets shape vertical alignment.
        /// </summary>
        internal ShapeVerticalAlignment VerticalAlignment
        {
            get
            {
                return m_vertAlignment;
            }
            set
            {
                m_vertAlignment = value;
            }
        }
        #endregion
    }
}
