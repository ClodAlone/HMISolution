#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if WINDOWS_PHONE ||WINDOWS_PHONE_7
 namespace Syncfusion.WP.Controls.Input
 {
#else
namespace Syncfusion.UI.Xaml.Controls.Input
{
#endif
    /// <summary>
    /// Reresents a list for the validation mode
    /// </summary>
    public enum ValidationMode
    {
        /// <summary>
        /// Validation when focus is lost
        /// </summary>
        LostFocus,

        /// <summary>
        /// Validation when Property is changed
        /// </summary>
        PropertyChanged
    }
}