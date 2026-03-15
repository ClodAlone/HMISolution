#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents form field with appearance custom support.
    /// </summary>   
    /// <seealso cref="PdfSignatureStyledField"/> Class    
    public abstract class PdfSignatureAppearanceField : PdfSignatureStyledField
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSignatureAppearanceField"/> class.
        /// </summary>
        protected PdfSignatureAppearanceField()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSignatureAppearanceField"/> class.
        /// </summary>
        /// <param name="page">page</param>
        /// <param name="name">The name.</param>
        protected PdfSignatureAppearanceField(PdfPageBase page, string name)
            : base(page, name)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the appearance.
        /// </summary>
        /// <value>The appearance.</value>
        public PdfAppearance Appearance
        {
            get
            {
                return Widget.Appearance;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Saves an object.
        /// </summary>
        internal override void Save()
        {
            base.Save();

            if (Form != null && !Form.NeedAppearances)
            {
                if (Widget.GetAppearance() == null)
                {
                    DrawAppearance(Widget.Appearance.Normal);
                }
            }
        }

        /// <summary>
        /// Draws this instance if it is flatten.
        /// </summary>
        internal override void Draw()
        {
            base.Draw();
        }

        /// <summary>
        /// Draws the appearance.
        /// </summary>
        /// <param name="template">The template.</param>
        protected virtual void DrawAppearance(PdfTemplate template)
        {
        }
        #endregion
    }
}
