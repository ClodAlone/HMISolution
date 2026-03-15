// <copyright file="SelectionPointerChangedEventArgs.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System.ComponentModel;

namespace Syncfusion.Windows.Edit
{
    /// <summary>
    /// Delegate for SelectionPointerChangedEvent
    /// </summary>
    /// <param name="sender">Gets the sender object from the reporting source</param>
    /// <param name="args">Gets the SelectionPointerChangedEventArgs object from the reporting source</param>
    public delegate void SelectionPointerChangedEventHandler(object sender, SelectionPointerChangedEventArgs args);

    /// <summary>
    /// Custom eventargs class for Selection pointer values
    /// </summary>
#if SyncfusionFramework4_0

    [DesignTimeVisible(false)]
#endif
    public class SelectionPointerChangedEventArgs
    {
        #region Local Variables

        /// <summary>
        /// instance for NewValue property.
        /// </summary>
        private int mnewvalue;

        /// <summary>
        /// instance for OldValue property.
        /// </summary>
        private int moldvalue;

        /// <summary>
        /// instance for PropertyName property.
        /// </summary>
        private string mproperty;

        #endregion Local Variables

        #region Properties

        /// <summary>
        /// Gets the new value of the property
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        public int NewValue
        {
            get
            {
                return mnewvalue;
            }
        }

        /// <summary>
        /// Gets the old values of the property
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        public int OldValue
        {
            get
            {
                return moldvalue;
            }
        }

        /// <summary>
        /// Gets the name of the property
        /// </summary>
        /// <value>
        /// Type: System.String
        /// </value>
        public string PropertyName
        {
            get
            {
                return mproperty;
            }
        }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionPointerChangedEventArgs"/> class.
        /// </summary>
        /// <param name="property">Gets the property from the reporting source</param>
        /// <param name="oldValue">Gets the oldValue from the reporting source</param>
        /// <param name="newValue">Gets the newValue from the reporting source</param>
        public SelectionPointerChangedEventArgs(string property, int oldValue, int newValue)
        {
            mproperty = property;
            moldvalue = oldValue;
            mnewvalue = newValue;
        }

        #endregion Constructor
    }
}