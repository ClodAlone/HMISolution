// <copyright file="_delegates.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// A delegate used for StringValidationCompleted event in MaskedTextBox.
    /// </summary>
    /// <param name="sender">The Object value.</param>
    /// <param name="e">The validation string.</param>
    public delegate void StringValidationCompletedEventHandler(object sender, StringValidationEventArgs e);

    /// <summary>
    /// EventArgs used for StringValidationCompleted event in MaskedTextBox.
    /// </summary>
    public class StringValidationEventArgs : EventArgs
    {
        #region Private members

        /// <summary>
        /// Define Cancel
        /// </summary>
        private bool m_bCancel = false;

        /// <summary>
        /// Define ValidInput
        /// </summary>
        private bool m_bIsValidInput = true;

        /// <summary>
        /// Define Message
        /// </summary>
        private string m_message = string.Empty;

        /// <summary>
        /// Define ValidationString
        /// </summary>
        private string m_validationString = string.Empty;

        #endregion Private members

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StringValidationEventArgs"/> class.
        /// </summary>
        /// <param name="bCancel">if set to <c>true</c> [b cancel].</param>
        /// <param name="bIsValidInput">if set to <c>true</c> [b is valid input].</param>
        /// <param name="message">The message.</param>
        /// <param name="validationString">The validation string.</param>
        public StringValidationEventArgs(bool bCancel, bool bIsValidInput, string message, string validationString)
        {
            m_bCancel = bCancel;
            m_bIsValidInput = bIsValidInput;
            m_message = message;
            m_validationString = validationString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringValidationEventArgs"/> class.
        /// </summary>
        /// <param name="bIsValidInput">if set to <c>true</c> [b is valid input].</param>
        /// <param name="message">The message.</param>
        /// <param name="validationString">The validation string.</param>
        public StringValidationEventArgs(bool bIsValidInput, string message, string validationString)
            : this(false, bIsValidInput, message, validationString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the StringValidationEventArgs class.
        /// </summary>
        public StringValidationEventArgs()
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the event should be canceled.
        /// </summary>
        public bool Cancel
        {
            get
            {
                return m_bCancel;
            }

            set
            {
                if (m_bCancel != value)
                {
                    m_bCancel = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the formatted input string
        /// has successfully passed RegEx validation.
        /// </summary>
        public bool IsValidInput
        {
            get
            {
                return m_bIsValidInput;
            }
        }

        /// <summary>
        /// Gets a text message describing the conversion process.
        /// </summary>
        public string Message
        {
            get
            {
                return m_message;
            }
        }

        /// <summary>
        /// Gets the string that the formatted input string is being validated against.
        /// </summary>
        public string ValidationString
        {
            get
            {
                return m_validationString;
            }
        }

        #endregion Properties
    }
}