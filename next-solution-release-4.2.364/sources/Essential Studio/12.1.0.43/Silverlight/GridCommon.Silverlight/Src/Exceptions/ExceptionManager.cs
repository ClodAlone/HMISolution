#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
#if !WinRT
namespace Syncfusion.Windows.GridCommon
#else
namespace Syncfusion.WinRT.GridCommon
#endif
{

    /// <summary>
    /// Provides a global hook for exceptions that have been cached inside the framework and gives you
    /// the option to provide specialized handling of the exception. You can also temporarily suspend and resume
    /// caching exceptions.
    /// </summary>
    /// <remarks>
    /// The Syncfusion framework notifies <see cref="ExceptionManager"/> about exceptions that
    /// are cached by calling <see cref="ExceptionManager.RaiseExceptionCatched(object, System.Exception)"/> or <see cref="ExceptionManager"/>.<para/>
    /// The <see cref="ExceptionManager.RaiseExceptionCatched(object, System.Exception)"/> method will raise the <see cref="ExceptionCatched"/>
    /// event. By handling the <see cref="ExceptionCatched"/> event, your code can analyze the exception that was cached
    /// and optionally let it bubble up by rethrowing the exception.<para/>
    /// Your code can also temporarily suspend and resume caching exceptions. This is useful if you want to provide your
    /// own exception handling. Just call <see cref="SuspendCatchExceptions"/> to disable handling exceptions and <see cref="ResumeCatchExceptions"/>
    /// to resume caching exceptions.<para/>
    /// You also have the options to disable caching exceptions altogether by setting <see cref="PassThroughExceptions"/> to True.<para/>
    /// Note: All static settings for this class are thread local.
    /// </remarks>
    /// <example><code lang="C#">
    /// // The following example demonstrates temporarily suspending exception caching when calling a base class version
    /// // of a method.
    ///         protected override void OnMouseDown(MouseEventArgs e)
    ///             {
    ///             ExceptionManager.SuspendCatchExceptions();
    ///             try
    ///             {
    ///                 base.OnMouseDown(e);
    ///                 ExceptionManager.ResumeCatchExceptions();
    ///             }
    ///             catch (Exception ex)
    ///             {
    ///                 ExceptionManager.ResumeCatchExceptions();
    ///                     // Notify exception manager about the catched exception and
    ///                     // give it a chance to optionally rethrow the exception if necessary
    ///                     // (e.g. if this OnMouseDown was called from another class that
    ///                     // wants to provide its own exception handling).
    ///                 if (!ExceptionManager.RaiseExceptionCatched(this, ex))
    /// 					throw ex;
    ///                 // handle exception here
    ///                 MessageBox.Show(ex.ToString());
    ///             }
    ///         }
    /// </code></example>
    /// <example><code lang="C#">
    /// // This code sample shows how exceptions are handled within the framework:
    ///                 try
    ///                 {
    ///                     CurrentCell.Refresh();
    ///                 }
    ///                 catch (Exception ex)
    ///                 {
    ///                     TraceUtil.TraceExceptionCatched(ex);
    ///                     if (!ExceptionManager.RaiseExceptionCatched(this, ex))
    /// 						throw ex;
    ///                 }
    /// </code></example>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class ExceptionManager
    {
        /// <summary></summary>
        [ThreadStatic]
        private static bool passThroughExceptions = false;

        /// <summary></summary>
        [ThreadStatic]
        private static int suspendCatchExceptions = 0;

        /// <summary></summary>
        private static object onExceptionCatchedKey = new object();

        /// <summary>
        /// Occurs when an exception was cached within the framework and <see cref="ExceptionManager"/> was notified.
        /// </summary>
        public static event ExceptionCatchedEventHandler ExceptionCatched;


        /// <summary>
        /// Lets you disable caching exceptions altogether by setting <see cref="PassThroughExceptions"/> to True.<para/></summary>
        public static bool PassThroughExceptions
        {
            get
            {
                return passThroughExceptions;
            }
            set
            {
                passThroughExceptions = value;
            }
        }

        /// <summary>
        /// Temporariliy suspends caching exceptions. 
        /// </summary>
        public static void SuspendCatchExceptions()
        {
            suspendCatchExceptions++;
        }

        /// <summary>
        /// Temporariliy resumes caching exceptions. 
        /// </summary>
        public static void ResumeCatchExceptions()
        {
            if (suspendCatchExceptions > 0)
            {
                suspendCatchExceptions--;
            }
        }

        /// <summary>
        /// Indicates whether exceptions should be cached or if they should bubble up. <see cref="RaiseExceptionCatched"/>
        /// calls this method.
        /// </summary>
        /// <returns></returns>
        public static bool ShouldCatchExceptions()
        {
            return suspendCatchExceptions == 0 && !passThroughExceptions;
        }

        /// <overload>
        /// Raises the <see cref="ExceptionCatched"/> event.
        /// </overload>
        /// <summary>
        /// Raises the <see cref="ExceptionCatched"/> event. If caching exceptions has been disabled
        /// by a <see cref="SuspendCatchExceptions"/> call or if <see cref="PassThroughExceptions"/> has been set to True,
        /// the exception is rethrown.
        /// </summary>
        /// <param name="e">A <see cref="ExceptionCatchedEventArgs"/> that contains the event data.</param>
        /// <returns></returns>
        /// <param name="sender"/>
        public static bool RaiseExceptionCatched(object sender, ExceptionCatchedEventArgs e)
        {
            if (ExceptionCatched != null)
            {
                ExceptionCatched(sender, e);
            }

            return ShouldCatchExceptions();
        }

        /// <summary>
        /// Raises the <see cref="ExceptionCatched"/> event. If caching exceptions has been disabled
        /// by a <see cref="SuspendCatchExceptions"/> call or if <see cref="PassThroughExceptions"/> has been set to True,
        /// the exception is rethrown.
        /// </summary>
        /// <param name="ex">A <see cref="Exception"/> that was cached.</param>
        /// <returns></returns>
        /// <param name="sender"/>
        public static bool RaiseExceptionCatched(object sender, Exception ex)
        {
            return RaiseExceptionCatched(sender, new ExceptionCatchedEventArgs(ex));
        }
    }

}
