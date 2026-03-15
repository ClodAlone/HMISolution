#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

using Syncfusion.Pdf.IO;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represents the abstract brush, which containing a basic functionality of a brush.
    /// </summary>
    public abstract class PdfBrush :
        ICloneable
    {
        #region Public methods
        /// <summary>
        /// Monitors the changes of the brush and modify PDF state respectively.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="streamWriter">The stream writer.</param>
        /// <param name="getResources">The get resources delegate.</param>
        /// <param name="saveChanges">if set to <c>true</c> the changes should be saved anyway.</param>
        /// <param name="currentColorSpace">The current color space.</param>
        /// <returns>True if the brush was different.</returns>
        internal abstract bool MonitorChanges(PdfBrush brush, PdfStreamWriter streamWriter,
            PdfGraphics.GetResources getResources, bool saveChanges, PdfColorSpace currentColorSpace);

        /// <summary>
        /// Monitors the changes of the brush and modify PDF state respectively.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="streamWriter">The stream writer.</param>
        /// <param name="getResources">The get resources delegate.</param>
        /// <param name="saveChanges">if set to <c>true</c> the changes should be saved anyway.</param>
        /// <param name="currentColorSpace">The current color space.</param>
        /// <param name="check">check</param>
        /// <param name="iccbased">Indicates the IccBased Color Space.</param>
        /// <param name="indexed">Indicates the indexed Color Space.</param>
        /// <returns>True if the brush was different.</returns>
        internal abstract bool MonitorChanges(PdfBrush brush, PdfStreamWriter streamWriter,
            PdfGraphics.GetResources getResources, bool saveChanges, PdfColorSpace currentColorSpace, bool check, bool iccbased, bool indexed);

        /// <summary>
        /// Monitors the changes of the brush and modify PDF state respectively.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="streamWriter">The stream writer.</param>
        /// <param name="getResources">The get resources delegate.</param>
        /// <param name="saveChanges">if set to <c>true</c> the changes should be saved anyway.</param>
        /// <param name="currentColorSpace">The current color space.</param>
        /// <param name="check">check</param>
        /// <returns>True if the brush was different.</returns>
        internal abstract bool MonitorChanges(PdfBrush brush, PdfStreamWriter streamWriter,
            PdfGraphics.GetResources getResources, bool saveChanges, PdfColorSpace currentColorSpace, bool check);

        /// <summary>
        /// Monitors the changes of the brush and modify PDF state respectively.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="streamWriter">The stream writer.</param>
        /// <param name="getResources">The get resources delegate.</param>
        /// <param name="saveChanges">if set to <c>true</c> the changes should be saved anyway.</param>
        /// <param name="currentColorSpace">The current color space.</param>
        /// <param name="check">check</param>
        /// <param name="iccbased">Indicates the IccBased Color Space.</param>
        /// <returns>True if the brush was different.</returns>
        internal abstract bool MonitorChanges(PdfBrush brush, PdfStreamWriter streamWriter,
           PdfGraphics.GetResources getResources, bool saveChanges, PdfColorSpace currentColorSpace, bool check, bool iccbased);

        /// <summary>
        /// Resets the changes, which were made by the brush.
        /// In other words resets the state to the initial one.
        /// </summary>
        /// <param name="streamWriter">The stream writer.</param>
        internal abstract void ResetChanges(PdfStreamWriter streamWriter);
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Creates a new copy of a brush.
        /// </summary>
        /// <returns>A new instance of the Brush class.</returns>
        public abstract PdfBrush Clone();
        #endregion
    }
}
