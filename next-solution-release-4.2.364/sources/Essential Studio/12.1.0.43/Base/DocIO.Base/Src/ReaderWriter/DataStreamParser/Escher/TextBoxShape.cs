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
using System.IO;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser;

#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// Summary description for TextBoxShape.
    /// </summary>
    [CLSCompliant(false)]
    internal class TextBoxShape : ShapeBase
    {
        #region Class initialize/finakize methods
        /// <summary>
        /// 
        /// </summary>
        internal TextBoxShape()
            : base()
        { }

        #endregion

        #region Class properties
        /// <summary>
        /// Get TextBoxShape's properties
        /// </summary>
        internal TextBoxProps TextBoxProps
        {
            get
            {
                return base.ShapeProps as TextBoxProps;
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        protected internal override void CreateShapeImpl()
        {
            m_shapeProps = new TextBoxProps();
        }

        #endregion

    }
}
