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
    /// Solver Container msofbtSolverContainer
    /// Rules give special behaviors to shapes. Rules can govern a single shape, like in the case of a 
    /// callout shape, or multiple shapes, as in the case of connectors. 
    /// Each drawing can have a list of rules associated with it.
    /// </summary>
    internal class MsofbtSolverContainer : BaseContainer
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal MsofbtSolverContainer(WordDocument doc)
            : base(MSOFBT.msofbtSolverContainer, doc)
        { }
        #endregion

    }
}
