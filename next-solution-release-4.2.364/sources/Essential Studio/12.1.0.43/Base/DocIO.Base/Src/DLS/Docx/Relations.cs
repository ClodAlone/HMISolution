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


#region File using directives
using System;
using System.IO;
using Syncfusion.Compression.Zip;

#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for Relation.
    /// </summary>
    internal class Relations : Part
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Relations"/> class.
        /// </summary>
        /// <param name="item">The zip item.</param>
        public Relations(ZipArchiveItem item)
            : base(item.DataStream)
        {
            m_name = item.ItemName;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataStream"></param>
        /// <param name="name"></param>
        internal Relations(Stream dataStream, string name)
            : base(dataStream)
        {
            m_name = name;
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal Part Clone()
        {
            Relations newRels = new Relations(m_dataStream, m_name);
            return newRels;
        }
        #endregion
    }
}

