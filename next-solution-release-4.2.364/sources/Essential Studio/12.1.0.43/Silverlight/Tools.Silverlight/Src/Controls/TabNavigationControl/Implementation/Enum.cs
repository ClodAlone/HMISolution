#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    ///
    /// </summary>
    public enum TransitionEffects
    {
        /// <summary>
        ///
        /// </summary>
        Slide,

        /// <summary>
        ///
        /// </summary>
        Fade,

#if SILVERLIGHT

        /// <summary>
        ///
        /// </summary>
        NewsFlash,

        /// <summary>
        ///
        /// </summary>
        Flip,

        /// <summary>
        ///
        /// </summary>
        Uncover,

#endif

        /// <summary>
        ///
        /// </summary>
        Zoom,

        /// <summary>
        ///
        /// </summary>
        Blur,

        /// <summary>
        ///
        /// </summary>
        Push,

        /// <summary>
        ///
        /// </summary>
        PushIn,

        /// <summary>
        ///
        /// </summary>
        Wipe,

        None
    }
}