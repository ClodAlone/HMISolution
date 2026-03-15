#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Drawing;
using Syncfusion.Pdf.IO;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents the annotation with associated action.
    /// </summary>
    /// <seealso cref="PdfActionLinkAnnotation"/> Class
    public class PdfActionAnnotation : PdfActionLinkAnnotation
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfActionAnnotation"/> class.
        /// </summary>
        /// <param name="rectangle">Bounds of the annotation.</param>
        /// <param name="action">The Pdf action.</param>
        public PdfActionAnnotation(RectangleF rectangle, PdfAction action)
            : base(rectangle, action)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Saves annotation object.
        /// </summary>
        protected override void Save()
        {
            base.Save();
            Dictionary.SetProperty(DictionaryProperties.A, Action);
        }
        #endregion
    }
}
