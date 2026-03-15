#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.XlsIO
{
    /// <summary>
    /// This interface represents TextBox form control shape.
    /// </summary>
    public interface IOptionButtonShape :
      ITextBoxShape
    {
        /// <summary>
        /// Indicates whether option button is checked.
        /// </summary>
        ExcelCheckState CheckState { get; set; }

        /// <summary>
        /// Indicates whether option button is first button. Read Only
        /// </summary>
        bool IsFirstButton {   get;    }

        /// <summary>
        /// Gets or sets value indicating whether 3D shadow is present.
        /// </summary>
        bool Display3DShading { get; set; }

        /// <summary>
        /// Indicates the cell in which the option button points to
        /// </summary>
        IRange LinkedCell { get; set; }
    }
}
