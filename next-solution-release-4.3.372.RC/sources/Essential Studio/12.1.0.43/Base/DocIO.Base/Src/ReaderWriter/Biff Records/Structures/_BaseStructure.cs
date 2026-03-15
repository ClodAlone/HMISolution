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
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures
{
    /// <summary>
    /// Summary description for _BaseStructure.
    /// </summary>
    public interface IBaseStructure
    {
        #region Class Public Methods
        //    /// <summary>
        //    /// Copies data from arrData into this structure with help of MemoryConverter.
        //    /// </summary>
        //    /// <param name="arrData">Data to be copied.</param>
        //    /// <param name="provider">MemoryConverter that will do copy operation.</param>
        //    public virtual void Parse( byte[] arrData, MemoryConverter provider )
        //    {
        //      PrepareStructure( arrData );
        //      provider.CopyFrom( arrData );
        //      provider.CopyTo( this );
        //    }
        //
        //    /// <summary>
        //    /// Prepares structure to copy operation in order to make correct copy,
        //    /// i.e. enlarges some arrays, etc.
        //    /// </summary>
        //    /// <param name="arrData">Data that will be copied into structure.</param>
        //    void PrepareStructure( byte[] arrData );

        /// <summary>
        /// 
        /// </summary>
        int Length { get; }
        #endregion
    }
}
