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
    /// This interface is implemented by command objects.
    /// </summary>
    /// <remarks>
    /// A command object encapsulates an action and the data required to
    /// perform the action. A command is executed using
    /// <see cref="Syncfusion.Windows.Forms.Diagram.ICommand.Do"/> and can
    /// be reversed using the <see cref="Syncfusion.Windows.Forms.Diagram.ICommand.Undo"/>
    /// method.
    /// </remarks>
    public interface ICommand
    {
        /// <summary>
        /// Gets short, user-friendly description of the command.
        /// </summary>
        string Description
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether or not the command supports undo.
        /// </summary>
        bool CanUndo
        {
            get;
        }

        /// <summary>
        /// Performs the action.
        /// </summary>
        /// <param name="target">Object that is acted upon (noun).</param>
        /// <returns>True if successful; otherwise False.</returns>
        bool Do(object target);

        /// <summary>
        /// Reverses the command.
        /// </summary>
        /// <returns>True if successful; otherwise False.</returns>
        bool Undo();

        /// <summary>
        /// Determines whether this instance can merge the specified command to previous recorded command.
        /// </summary>
        /// <param name="cmd">The command to merge.</param>
        /// <returns>
        /// <c>true</c> if this instance can merge the specified command to last recorded command; otherwise, <c>false</c>.
        /// </returns>
        bool CanMerge(ICommand cmd);

        /// <summary>
        /// Merges the specified command with last recorded user operation.
        /// </summary>
        /// <param name="cmd">The command to merge.</param>
        void Merge(ICommand cmd);
    }
}
