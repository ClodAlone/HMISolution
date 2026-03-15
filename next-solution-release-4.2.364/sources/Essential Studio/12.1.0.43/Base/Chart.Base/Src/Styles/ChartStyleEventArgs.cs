#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Diagnostics;

using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    ///   Delegate that is to be used for events that broadcast changes to <see cref="ChartStyleInfo"/>.
    /// </summary>
    /// <param name="sender" type="object">
    ///     <para>
    ///	     Sender.
    ///     </para>
    /// </param>
    /// <param name="args" type="Syncfusion.Windows.Forms.Chart.ChartStyleChangedEventArgs">
    ///     <para>
    ///      Argument.
    ///     </para>
    /// </param>
    /// <remarks>
    ///
    /// </remarks>
    public delegate void ChartStyleChangedEventHandler(object sender, ChartStyleChangedEventArgs args);

    /// <summary>
    /// Argument that is to be used in the <see cref="ChartStyleChangedEventHandler"/> delegate.
    /// </summary>
    public class ChartStyleChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Specifies the types of changes.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// Style has been changed.
            /// </summary>
            Changed,

            /// <summary>
            /// Style has been reset to default.
            /// </summary>
            Reset
        }

        /// <summary>
        /// The Invalid Index.
        /// </summary>
        /// /// <internalonly/>
        [DocumentationExclude()]
        public const int InvalidIndex = -1;
        private Type type;
        private int xIndex;

        /// <summary>
        /// Creates the Reset typeof of arguments.
        /// </summary>
        /// <returns>Returns ChartStyleChangedEventArgs object.</returns>
        /// <internalonly/>
        [DocumentationExclude()]
        public static ChartStyleChangedEventArgs CreateResetEventArgs()
        {
            return new ChartStyleChangedEventArgs(Type.Reset, ChartStyleChangedEventArgs.InvalidIndex);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartStyleChangedEventArgs"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="xIndex">Index of the x.</param>
        public ChartStyleChangedEventArgs(Type type, int xIndex)
        {
            this.type = type;
            this.xIndex = xIndex;
        }

        /// <summary>
        ///     Gets the type of the event.
        /// </summary>
        public Type EventType
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>
        ///     Gets the index value of the changed style.
        /// </summary>
        public int Index
        {
            get
            {
                return this.xIndex;
            }
        }
    }
}