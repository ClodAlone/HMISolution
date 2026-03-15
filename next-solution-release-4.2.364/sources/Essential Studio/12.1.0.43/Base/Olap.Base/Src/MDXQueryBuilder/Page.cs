#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT
namespace Syncfusion.Olap.MDXQueryBuilder
{
#else
namespace Syncfusion.OlapSilverlight.Common
{

#endif
    /// <summary>
    /// Page class to help in generating MDX expression
    /// </summary>
    public class Page
    {
        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize { get; set; }
        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>The current page.</value>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Page"/> class.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <param name="pageSize">Size of the page.</param>
        public Page(int currentPage, int pageSize)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
        }

        /// <summary>
        /// Gets the start index.
        /// </summary>
        /// <returns></returns>
        public int GetStartIndex()
        {
            return (CurrentPage - 1) * PageSize;
        }

        /// <summary>
        /// Gets the end index.
        /// </summary>
        /// <returns></returns>
        public int GetEndIndex()
        {
            return CurrentPage * PageSize;
        }
    }
}
