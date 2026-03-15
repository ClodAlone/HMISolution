#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Tools.Navigation
{
    /// <summary>
    /// Provides data for a <see cref="NavigationView.BarSelectionChanging"/> event.
    /// </summary>
    public class BarSelectionChangingEventArgs :
        CancelEventArgs
    {
        #region Fields

        private Bar _bar;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="BarSelectionChangingEventArgs"/> class.
        /// </summary>
        /// <param name="cancel"><c>true</c> to cancel the event; otherwise, <c>false</c>.</param>
        /// <param name="bar">The bar to be selected.</param>
        public BarSelectionChangingEventArgs(bool cancel, Bar bar) :
            base(cancel)
        {
            _bar = bar;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BarSelectionChangingEventArgs"/> class.
        /// </summary>
        /// <param name="bar">The bar to be selected.</param>
        /// <remarks><see cref="CancelEventArgs.Cancel"/> property is set to <c>false</c>.</remarks>
        public BarSelectionChangingEventArgs(Bar bar) :
            this(false, bar)
        {
        }

        #endregion

        /// <summary>
        /// Gets or sets the bar to be selected.
        /// </summary>
        public Bar Bar
        {
            get
            {
                return _bar;
            }
            set
            {
                _bar = value;
            }
        }
    }

    /// <summary>
    /// Provides data for a <see cref="NavigationView.BarSelectionChanged"/> event.
    /// </summary>
    public class BarSelectionChangedEventArgs :
        EventArgs
    {
        #region Fields

        private Bar _bar;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="BarSelectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="bar">The selected bar.</param>
        public BarSelectionChangedEventArgs(Bar bar)
        {
            _bar = bar;
        }

        #endregion

        /// <summary>
        /// Gets or sets the selected bar.
        /// </summary>
        public Bar Bar
        {
            get
            {
                return _bar;
            }
            set
            {
                _bar = value;
            }
        }
    }
}
