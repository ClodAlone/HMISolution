#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Tools.Controls
{
    public interface IVirtualTree
    {
        /// <summary>
        /// Gets and set the expanded.
        /// </summary>
        /// <value>The is expanded.</value>
        bool IsExpanded
        {
            get;

            set;
        }

        /// <summary>
        /// Gets and set the itemscount.
        /// </summary>
        /// <value>The items count.</value>
        int ItemsCount
        {
            get;

            set;
        }

        /// <summary>
        /// Gets and set the extend height.
        /// </summary>
        /// <value>The extent height.</value>
        double ExtentHeight
        {
            get;

            set;
        }

        /// <summary>
        /// Gets and set the is selected.
        /// </summary>
        /// <value>The is selected.</value>
        bool IsSelected
        {
            get;

            set;
        }

        /// <summary>
        /// Gets and set the parent.
        /// </summary>
        /// <value>The parent.</value>
        IVirtualTree Parent
        {
            get;

            set;
        }
    }
}