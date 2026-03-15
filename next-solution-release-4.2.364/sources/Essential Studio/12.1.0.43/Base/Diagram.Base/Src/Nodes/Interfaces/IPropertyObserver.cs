#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Base property container interface to notify all properties changes.
    /// </summary>
    public interface IPropertyObserver
    {
        /// <summary>
        /// Called when property changing.
        /// </summary>
        /// <param name="strPropertyContainerName">Name of the property container.</param>
        /// <param name="strPropertyName">Name of the property.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>true, if property changing otherwise, false.</returns>
        bool OnPropertyChanging(string strPropertyContainerName, string strPropertyName, object newValue);

        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="strPropertyContainerName">Name of the property container.</param>
        /// <param name="strPropertyName">Name of the property.</param>
        void OnPropertyChanged(string strPropertyContainerName, string strPropertyName);
    }
}
