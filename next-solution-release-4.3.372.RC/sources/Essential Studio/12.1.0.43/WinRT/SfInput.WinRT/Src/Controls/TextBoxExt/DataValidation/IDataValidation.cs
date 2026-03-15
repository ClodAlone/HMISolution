#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents an interface for the data validation
    /// </summary>
    public interface IDataValidation
    {
        /// <summary>
        /// Gets the Error data
        /// </summary>
        string Error { get; }

        /// <summary>
        /// Gets an instance of the control
        /// </summary>
        /// <param name="columnname"></param>
        /// <returns></returns>
        string this[string columnname] { get; }
    }
}