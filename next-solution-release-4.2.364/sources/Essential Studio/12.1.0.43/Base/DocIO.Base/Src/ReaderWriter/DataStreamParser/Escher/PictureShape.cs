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
using Syncfusion.DocIO.DLS.Entities;
using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// Summary description for PictureShape.
    /// </summary>
    [CLSCompliant(false)]
    internal class PictureShape : ShapeBase
    {
        #region Class members
        private ImageRecord m_imageRecord;
        #endregion

        #region Class properties
        /// <summary>
        /// Get PictureShape's image
        /// </summary>
        internal ImageRecord ImageRecord
        {
            get
            {
                return m_imageRecord;
            }
        }
        /// <summary>
        /// Get PictureShapes properties
        /// </summary>
        internal PictureShapeProps PictureProps
        {
            get
            {
                return base.ShapeProps as PictureShapeProps;
            }
        }
        #endregion

        #region Class initialize / finalize methods
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="image"></param>
        internal PictureShape(ImageRecord imageRecord)
            : base()
        {
            m_imageRecord = imageRecord;
        }
        /// <summary>
        /// 
        /// </summary>
        internal PictureShape()
            : base()
        { }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        protected internal override void CreateShapeImpl()
        {
            m_shapeProps = new PictureShapeProps();
        }
        #endregion
    }
}
