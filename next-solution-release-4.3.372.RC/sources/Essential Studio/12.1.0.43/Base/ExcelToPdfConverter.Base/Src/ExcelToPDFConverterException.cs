#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.ExcelToPdfConverter
{
    /// <summary>
    /// Thrown when trying to convert the Excel file to an PDF document.
    /// </summary>
    public class ExcelToPDFConverterException :ApplicationException 
    {
        #region Class constants
        /// <summary>
        /// Default message.
        /// </summary>
        private const string DEF_MESSAGE = "Can't convert the Excel file to an PDF document.\n";
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the class with an empty error message.
        /// </summary>
        public ExcelToPDFConverterException()
            : this(DEF_MESSAGE)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class with a specified error message.
        /// </summary>
        /// <param name="message">Error message.</param>
        public ExcelToPDFConverterException(string message)
            :base(DEF_MESSAGE + message)
        {

        }
        #endregion
    }
}
