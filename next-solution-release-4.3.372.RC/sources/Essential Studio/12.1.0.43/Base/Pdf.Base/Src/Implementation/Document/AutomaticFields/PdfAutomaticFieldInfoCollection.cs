#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

/// <summary>
/// The Syncfusion.Pdf namespace contains classes for creating PDF document.
/// </summary>
namespace Syncfusion.Pdf
{
    /// <summary>
    /// Represent a collection of automatic fields information.
    /// </summary>
    /// <seealso cref="PdfCollection"/> Class    
    internal class PdfAutomaticFieldInfoCollection : PdfCollection
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAutomaticFieldInfoCollection"/> class.
        /// </summary>
        public PdfAutomaticFieldInfoCollection()
            : base()
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the specified field info.
        /// </summary>
        /// <param name="fieldInfo">The field info.</param>
        /// <returns>field Info</returns>
        public int Add(PdfAutomaticFieldInfo fieldInfo)
        {
            if (fieldInfo == null)
            {
                throw new ArgumentNullException("fieldInfo");
            }

            return List.Add(fieldInfo);
        }
        #endregion
    }
}
