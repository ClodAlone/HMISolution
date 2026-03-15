// <copyright file="AutoCompleteMode.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WPF
namespace Syncfusion.Windows.Controls.Input
#elif WINDOWS_PHONE || WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls.Input
#elif SILVERLIGHT
namespace Syncfusion.Tools.Controls.Input
#else
namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    ///  Specifies the mode of the auto complete.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public enum AutoCompleteMode
    {
        /// <summary>
        /// Suggestion and append are not performed with auto complete.
        /// </summary>
        None,

        /// <summary>
        /// Perform append only with auto complete.
        /// </summary>
        Append,

        /// <summary>
        /// Perform suggestion only with auto complete.
        /// </summary>
        Suggest,

        /// <summary>
        /// Perform suggestions and append with auto complete.
        /// </summary>
        SuggestAppend
    }

    /// <summary>
    ///  Specify the placement of suggestion box with auto complete.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public enum SuggestionBoxPlacement
    {
        /// <summary>
        /// The SuggestionBox always placed top with auto complete.
        /// </summary>
        Top,

        /// <summary>
        /// The SuggestionBox always at the bottom with auto complete.
        /// </summary>
        Bottom,

        /// <summary>
        /// The SuggestionBox not displayed with auto complete.
        /// </summary>
        None
    }

    /// <summary>
    ///  specifies the modes of search with auto complete.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public enum SuggestionMode
    {
        /// <summary>
        /// Returns the all the AutoCompleteSource collection to suggestions
        /// </summary>
        None,

        /// <summary>
        /// Returns the suggestions whether the beginning of the string matches in the list which is culture sensitive.  
        /// </summary>
        StartsWith,

        /// <summary>
        /// Returns the suggestions whether the beginning of the string matches in the list which is culture and case sensitive.  
        /// </summary>
        StartsWithCaseSensitive,

        /// <summary>
        /// Returns the suggestions whether the beginning of the string matches in the list by using StringComparer.OrdinalIgnoreCase.  
        /// </summary>
        StartsWithOrdinal,

        /// <summary>
        /// Returns the suggestions whether the beginning of the string matches in the list by using StringComparer.Ordinal.  
        /// </summary>
        StartsWithOrdinalCaseSensitive,

       /// <summary>
        /// Returns the suggestions whether the beginning of the string contains in the list which is culture sensitive.  
        /// </summary>
        Contains,

        /// <summary>
        /// Returns the suggestions whether the beginning of the string contains in the list which is culture and case sensitive.  
        /// </summary>
        ContainsCaseSensitive,

        /// <summary>
        /// Returns the suggestions whether the beginning of the string contains in the list by using StringComparer.OrdinalIgnoreCase.  
        /// </summary>
        ContainsOrdinal,

        /// <summary>
        /// Returns the suggestions whether the beginning of the string contains in the list by using StringComparer.Ordinal.  
        /// </summary>
        ContainsOrdinalCaseSensitive,

        /// <summary>
        /// Returns the suggestions whether the beginning of the string equals in the list which is culture sensitive.  
        /// </summary>
        Equals,

        /// <summary>
        /// Returns the suggestions whether the beginning of the string equals in the list which is culture and case sensitive.  
        /// </summary>
        EqualsCaseSensitive,

        /// <summary>
        /// Returns the suggestions whether the beginning of the string equals in the list by using StringComparer.OrdinalIgnoreCase.  
        /// </summary>
        EqualsOrdinal,

        /// <summary>
        /// Returns the suggestions whether the beginning of the string equals in the list by using StringComparer.Ordinal.  
        /// </summary>
        EqualsOrdinalCaseSensitive,

        /// <summary>
        /// Returns the Suggestion whether the specified object occurs with in the list.
        /// </summary>
        Custom,   
    }

}
