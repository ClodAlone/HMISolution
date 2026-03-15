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
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Design;
using Syncfusion.Windows.Forms.Tools.Design;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Specifies the button alignment with respect to the edit control.
    /// </summary>
    /// <remarks>
    /// Any <see cref="ButtonEditChildButton"/> included as part of a <see cref="ButtonEdit"/>
    /// control can be aligned to the right or to the left of the edit control (the TextBox).
    /// <para>
    /// The default value is <see cref="ButtonAlignment.Right"/>.
    /// </para>
    /// </remarks>
    [
    Serializable()
    ]
    public enum ButtonAlignment
    {
        /// <summary>
        /// The button appears to the left of the edit control.
        /// </summary>
        /// <remarks>
        /// See the <see cref="ButtonEditChildButton.ButtonAlign"/> property for more information.
        /// </remarks>
        Left = 0,

        /// <summary>
        /// The button appears to the right of the edit control.
        /// </summary>
        /// <remarks>
        /// See the <see cref="ButtonEditChildButton.ButtonAlign"/> property for more information.
        /// </remarks>
        Right
    }

    /// <summary>
    /// This interface is implemented by the <see cref="ButtonEdit"/> to listen
    /// to notifications from <see cref="ButtonEditChildButton"/> classes.
    /// </summary>
    /// <remarks>
    /// You will not need to use this class directly. It is useful for listening to notifications
    /// from <see cref="ButtonEditChildButton"/> class.
    /// </remarks>
    public interface IButtonEditParent
    {
        /// <summary>
        /// Notifies the listener that the <see cref="ButtonEditChildButton"/>'s size has changed.
        /// </summary>
        /// <param name="btn">The <see cref="ButtonEditChildButton"/> that has changed its size.</param>
        /// <param name="newSize">The new size of the button.</param>
        /// <remarks>
        /// This notification is sent by the <see cref="ButtonEditChildButton"/> when the 
        /// <see cref="ButtonEditChildButton.PreferredWidth"/> property value is changed.
        /// The <see cref="ButtonEdit"/> control implements this interface and receives
        /// the notification to change its layout in accordance with the new size of the
        /// ButtonEditChildButton.
        /// </remarks>
        void ChildButtonSizeChanged(ButtonEditChildButton btn, Size newSize);

        /// <summary>
        /// Notifies the listener that the <see cref="ButtonEditChildButton"/>'s alignment has changed.
        /// </summary>
        /// <param name="btn">The <see cref="ButtonEditChildButton"/> that has changed its alignment.</param>
        /// <param name="newAlign">The new alignment.</param>
        /// <remarks>
        /// This notification is sent by the <see cref="ButtonEditChildButton"/> when the 
        /// <see cref="ButtonEditChildButton.ButtonAlign"/> property value is changed.
        /// The <see cref="ButtonEdit"/> control implements this interface and receives
        /// the notification to change its layout in accordance with the new alignment of the
        /// ButtonEditChildButton. See the <see cref="ButtonAlignment"/> type for the values
        /// that the ButtonAlignment can support.
        /// </remarks>
        void ChildButtonAlignmentChanged(ButtonEditChildButton btn, ButtonAlignment newAlign);
    }
}
