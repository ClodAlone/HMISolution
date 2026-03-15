//#region Copyright Syncfusion Inc. 2001 - 2014
////
////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
////
//#endregion

//#region file using directives
//using System;
//using System.Collections;
//using Syncfusion.DocIO.DLS;
//#endregion

//namespace Syncfusion.DocIO.DLS
//{
//    /// <summary>
//    /// Represents a collection of <see cref="Syncfusion.DocIO.DLS.IWordDocument"/> objects.
//    /// </summary>
//    public class DocumentCollection : CollectionBase
//    {
//        #region Properties
//        /// <summary>
//        /// Gets the <see cref="Syncfusion.DocIO.DLS.IWordDocument"/> at the specified index.
//        /// </summary>
//        /// <value></value>
//        public IWordDocument this[int index]
//        {
//            get
//            {
//                return (IWordDocument)List[index];
//            }
//        }
//        #endregion

//        #region Public methods
//        /// <summary>
//        /// Adds the specified document.
//        /// </summary>
//        /// <param name="document">The document.</param>
//        /// <returns></returns>
//        public int Add(IWordDocument document)
//        {
//            return InnerList.Add(document);
//        }
//        #endregion
//    }
//}