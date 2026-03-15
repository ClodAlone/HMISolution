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
#if SILVERLIGHT || WP
using Image = Syncfusion.DocIO.DLS.Entities.Image;
using Metafile = Syncfusion.DocIO.DLS.Entities.Metafile;
#else
using Image = System.Drawing.Image;
using Metafile = System.Drawing.Imaging.Metafile;
#endif
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Escher
{
    /// <summary>
    /// Summary description for Blip.
    /// </summary>
    internal abstract class Blip : BaseWordRecord
    {
        #region Class Initialize/Finalize Methods
        /// <summary>
        /// 
        /// </summary>
        public Blip()
        {
        }
        #endregion

        #region Class Abstract Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="length"></param>
        /// <param name="chr"></param>
        /// <returns></returns>
        public abstract Image Read(Stream stream, int length, bool chr);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="image"></param>
        /// <param name="imageFormat"></param>
        /// <param name="id"></param>
        internal abstract void Write(Stream stream, MemoryStream image, MSOBlipType imageFormat, byte[] id);
        #endregion
    }
}
