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
    /// Group Container msofbtSpgrContainer
    /// 
    /// A group is a collection of other shapes. The contained shapes are placed in the coordinate
    /// system of the group. The group container contains a variable number of shapes 
    /// (msofbtSpContainer) and other groups (msofbtSpgrContainer, for nested groups). 
    /// The group itself is a shape, and always appears as the first msofbtSpContainer in the group 
    /// container.
    /// </summary>
    internal class MsofbtSpgrContainer : BaseContainer
    {
        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal MsofbtSp Shape
        {
            get
            {
                MsofbtSpContainer spContainer = Children[0] as MsofbtSpContainer;
                if (spContainer != null)
                {
                    return spContainer.Shape;
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal MsofbtSpgrContainer(WordDocument doc)
            : base(MSOFBT.msofbtSpgrContainer, doc)
        { }
        #endregion

        //    #region Class methods
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    internal void ClearAllExceptSpContainer()
        //    {
        //      Children.RemoveRange( 1, Children.Count - 1 );
        //    }
        //    #endregion
    }
}
