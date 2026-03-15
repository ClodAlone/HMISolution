#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a PopupMenuItem control.
    /// </summary>
    public class PopupMenuItem
    {
        /// <summary>
        /// Gets or sets header text for item
        /// </summary>
        public ContentControl HeaderText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets header image for item
        /// </summary>
        public Image HeaderImage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets checked image for item
        /// </summary>
        public Image CheckedImage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets item's ID
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets item's parent ID
        /// </summary>
        public int? ParentId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets item's tag
        /// </summary>
        public object Tag
        {
            get;
            set;
        }
    }
}
