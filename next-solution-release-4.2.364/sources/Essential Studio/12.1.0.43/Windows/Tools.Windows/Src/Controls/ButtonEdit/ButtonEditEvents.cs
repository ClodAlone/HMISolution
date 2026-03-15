#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// The event data for <see cref="ButtonEdit.ButtonClicked"/> event.
    /// </summary>
    /// <remarks>
    /// The <see cref="ButtonEdit.ButtonClicked"/> event is raised when any of
    /// the <see cref="ButtonEditChildButton"/> child controls of the <see cref="ButtonEdit"/>
    /// class are clicked.
    /// <para>
    /// One of the members of the event data is the actual <see cref="ButtonEditChildButton"/>
    /// that was clicked.
    /// </para>
    /// </remarks>
    public class ButtonClickedEventArgs : System.EventArgs
    {
        /// <summary>
        /// The clicked button object.
        /// </summary>
        private ButtonEditChildButton clickedButtonValue;

        /// <summary>
        /// Initializes a new instance of the ButtonClickedEventArgs class. 
        /// </summary>
        /// <remarks>
        /// Needs to set the <see cref="ClickedButton"/> property with the
        /// <see cref="ButtonEditChildButton"/> that was clicked.
        /// </remarks>
        public ButtonClickedEventArgs()
        {
            this.clickedButtonValue = new ButtonEditChildButton();
        }

        /// <summary>
        /// Initializes a new instance of the ButtonClickedEventArgs class.
        /// </summary>
        /// <param name="value">The button that was clicked.</param>
        /// <remarks>
        /// This version of the constructor sets the <see cref="ClickedButton"/> property with the
        /// <see cref="ButtonEditChildButton"/> that was clicked.
        /// </remarks>
        public ButtonClickedEventArgs(ButtonEditChildButton value)
            : this()
        {
            this.clickedButtonValue = value;
        }

        /// <summary>
        /// Gets or sets the clicked <see cref="ButtonEditChildButton"/> object.
        /// </summary>
        /// <remarks>
        /// The <see cref="ButtonEdit.ButtonClicked"/> event handler will be able to use this
        /// to get the <see cref="ButtonEditChildButton"/> that was clicked.
        /// </remarks>
        public ButtonEditChildButton ClickedButton
        {
            get
            {
                return this.clickedButtonValue;
            }

            set
            {
                this.clickedButtonValue = value;
            }
        }
    }

    /// <summary>
    /// The delegate for the <see cref="ButtonEdit.ButtonClicked"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">A ButtonClickedEventArgs that contains the event data.</param>
    /// <remarks>
    /// See the <see cref="ButtonEdit.ButtonClicked"/> event for more information.
    /// </remarks>
    public delegate void ButtonClickedEventHandler(object sender, ButtonClickedEventArgs args);
}
