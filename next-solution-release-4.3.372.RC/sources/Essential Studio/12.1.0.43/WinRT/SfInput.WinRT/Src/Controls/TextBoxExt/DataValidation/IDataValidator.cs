#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Windows.UI.Xaml;
namespace Syncfusion.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents an interface for the data validator
    /// </summary>
    public interface IDataValidator
    {
        /// <summary>
        /// Validates the arguments
        /// </summary>
        /// <param name="args"></param>
        void Validate(ValidationEventArgs args);
    }
}