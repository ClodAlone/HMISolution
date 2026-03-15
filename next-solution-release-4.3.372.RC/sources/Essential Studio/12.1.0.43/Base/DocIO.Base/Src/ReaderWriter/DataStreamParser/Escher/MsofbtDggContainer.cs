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

using Syncfusion.DocIO.ReaderWriter.Escher;
using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// Summary description for MsofbtDggContainer.
    /// </summary>
    internal class MsofbtDggContainer : BaseContainer
    {
        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal MsofbtBstoreContainer BstoreContainer
        {
            get
            {
                return (FindContainerByType(typeof(MsofbtBstoreContainer)) as MsofbtBstoreContainer);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal MsofbtDgg Dgg
        {
            get
            {
                return (FindContainerByType(typeof(MsofbtDgg)) as MsofbtDgg);
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal MsofbtDggContainer(WordDocument doc)
            : base(MSOFBT.msofbtDggContainer, doc)
        { }
        #endregion
    }
}
