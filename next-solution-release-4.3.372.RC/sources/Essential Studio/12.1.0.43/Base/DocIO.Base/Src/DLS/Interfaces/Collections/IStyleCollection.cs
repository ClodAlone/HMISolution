#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a collection of <see cref="Syncfusion.DocIO.DLS.IStyle"/> objects.
    /// </summary>
    public interface IStyleCollection : ICollectionBase
    {
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.IStyle"/> at the specified index.
        /// </summary>
        /// <value></value>
        IStyle this[int index]
        {
            get;
        }
        /// <summary>
        /// Returns true, if the fixed index 13 in stylesheet has style. (other than empty style)
        /// </summary>
        /// <remarks>Reserved styles are applicable only for *.doc format</remarks>
        bool FixedIndex13HasStyle
        {
            get;
            set;
        }
        /// <summary>
        /// Returns true, if the fixed index 14 in stylesheet has style. (other than empty style)
        /// </summary>
        /// <remarks>Reserved styles are applicable only for *.doc format</remarks>
        bool FixedIndex14HasStyle
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the style name of the style present at the fixed index 13 in the stylesheet
        /// </summary>
        /// <remarks>Reserved styles are applicable only for *.doc format</remarks>
        string FixedIndex13StyleName
        {
            get;
            set;
        }
        /// <summary>
        /// Represents the style name of the style present at the fixed index 14 in the stylesheet
        /// </summary>
        /// <remarks>Reserved styles are applicable only for *.doc format</remarks>
        string FixedIndex14StyleName
        {
            get;
            set;
        }
        /// <summary>
        /// Adds the specified style.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns></returns>
        int Add(IStyle style);
        /// <summary>
        /// Finds a first style with specified style name
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        IStyle FindByName(string name);
        /// <summary>
        /// Finds a style by style name and style type
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="styleType">Type of the style.</param>
        /// <returns></returns>
        IStyle FindByName(string name, StyleType styleType);
    }
}