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
    /// Summary description for ShapeBase.
    /// </summary>
    [CLSCompliant(false)]
    internal class ShapeBase
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        protected BaseProps m_shapeProps;
        #endregion

        #region Class properties
        /// <summary>
        /// Get shape 
        /// </summary>
        internal BaseProps ShapeProps
        {
            get
            {
                return m_shapeProps;
            }
        }
        #endregion

        #region Class initialize / finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal ShapeBase()
        {
            this.CreateShapeImpl();
        }
        /// <summary>
        /// 
        /// </summary>
        protected internal virtual void CreateShapeImpl()
        {
            throw new NotImplementedException("Not implemented");
        }
        #endregion
    }
}
