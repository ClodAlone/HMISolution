#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Runtime.Serialization;


#if !SILVERLIGHT
namespace Syncfusion.Olap.Manager
{
    [Serializable]
#else
namespace Syncfusion.OlapSilverlight.Manager
{
#endif
    /// <summary>
    /// PagerOptions class help us in getting user options
    /// </summary>
    public class PagerOptions
    {
        internal const int DEFAULTPAGESIZE = 50;
        /// <summary>
        /// Gets or sets the size of the horizontal page.
        /// </summary>
        /// <value>The size of the horizontal page.</value>
#if SILVERLIGHT
        [DataMember]
#endif
        public int CategorialPageSize { get; set; }
        /// <summary>
        /// Gets or sets the horizontal current page.
        /// </summary>
        /// <value>The horizontal current page.</value>
#if SILVERLIGHT
        [DataMember]
#endif
        public int CategorialCurrentPage { get; set; }
        /// <summary>
        /// Gets or sets the size of the vertical page.
        /// </summary>
        /// <value>The size of the vertical page.</value>
#if SILVERLIGHT
        [DataMember]
#endif
        public int SeriesPageSize { get; set; }
        /// <summary>
        /// Gets or sets the vertical current page.
        /// </summary>
        /// <value>The vertical current page.</value>
#if SILVERLIGHT
        [DataMember]
#endif
        public int SeriesCurrentPage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagerOptions"/> class.
        /// </summary>
        public PagerOptions()
        {
            SeriesPageSize = CategorialPageSize = DEFAULTPAGESIZE;
            SeriesCurrentPage = CategorialCurrentPage = 1;
        }

        /// <summary>
        /// Copies current to new one.
        /// </summary>
        /// <param name="pager">The pager.</param>
        public void CopyTo(ref PagerOptions pager)
        {
            pager = this.MemberwiseClone() as PagerOptions;
        }
    }
}
