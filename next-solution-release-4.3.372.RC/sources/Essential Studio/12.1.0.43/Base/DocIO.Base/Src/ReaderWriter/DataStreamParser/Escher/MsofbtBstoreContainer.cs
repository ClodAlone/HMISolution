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

using Syncfusion.DocIO.ReaderWriter.Escher;
using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// The BStore container is just an array of Blip Store Entry (BSE) records. Each shape stores indices 
    /// into the array for the BLIPs they use. BLIPs are used not only for inserted pictures, but also for 
    /// the textured and pictures fills of the shape. 
    /// </summary>
    internal class MsofbtBstoreContainer : BaseContainer
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal MsofbtBstoreContainer(WordDocument doc)
            : base(MSOFBT.msofbtBstoreContainer, doc)
        { }
        #endregion

        #region Class methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void WriteRecordData(Stream stream)
        {
            Header.Instance = Children.Count;
            base.WriteRecordData(stream);
        }
        #endregion
    }
}
