#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
#if !WinRT
namespace Syncfusion.Windows.ComponentModel
#else
namespace Syncfusion.WinRT.ComponentModel
#endif
{
    /// <summary>
    /// Indicates the current state of the operation.
    /// </summary>
    public enum OperationMilestone
    {
        /// <summary>
        /// The operation is starting.
        /// </summary>
        Start,
        /// <summary>
        /// The operation is in progress.
        /// </summary>
        Progress,
        /// <summary>
        /// The operation is finished.
        /// </summary>
        Finished
    }

    /// <summary>
    /// This class listens to a list of <see cref="IOperationFeedbackProvider"/> and fires a <see cref="OperationFeedbackListener.Progress"/> event
    /// if the operation takes longer than a specified <see cref="OperationFeedbackListener.Delay"/>.
    /// </summary>
    /// <remarks>
    /// Derive from this class if you want to add support for displaying status messages in your
    /// status bar.
    /// <note type="note"><see cref="OperationFeedbackListener"/> operates on the same thread
    /// where the long operation takes place. See the <see cref="Syncfusion.Windows.Forms.DelayedStatusDialog"/> class how to
    /// give feedback about the operation on a different thread and allow the user to cancel the
    /// operation by pressing the Cancel button in a dialog.</note>
    /// </remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.DelayedStatusDialog"/> <seealso cref="Syncfusion.Windows.Forms.DelayedWaitCursor"/>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class OperationFeedbackListener : NonFinalizeDisposable
    {
        private List<object> providers = new List<object>();
        private int delay = 250;
        private bool progressFired = false;

        /// <summary>
        /// Occurs to indicate the progress of an ongoing operation.
        /// </summary>
        public event OperationFeedbackEventHandler Progress;


        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                UnWireEvents();

            base.Dispose(disposing);
        }

        private void UnWireEvents()
        {
            foreach (WeakReference wr in providers)
            {
                IOperationFeedbackProvider provider = wr.Target as IOperationFeedbackProvider;
                if (provider != null)
                    provider.OperationFeedback -= new OperationFeedbackEventHandler(ProviderProgress);
            }
            providers.Clear();
            providers = null;
        }

        /// <summary>
        /// Adds an <see cref="IOperationFeedbackProvider"/> that this object will listen to and
        /// provide user feedback for.
        /// </summary>
        /// <param name="provider">An object that implements the <see cref="IOperationFeedbackProvider"/> interface.</param>
        public void AddProvider(IOperationFeedbackProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            provider.OperationFeedback += new OperationFeedbackEventHandler(ProviderProgress);
            // association will go away when provider is removed
            providers.Add(new WeakReference(provider));
        }

        /// <summary>
        /// Removes an <see cref="IOperationFeedbackProvider"/> from the list of feedback providers.
        /// </summary>
        /// <param name="provider">An object that implements the <see cref="IOperationFeedbackProvider"/> interface.</param>
        public void RemoveProvider(IOperationFeedbackProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            provider.OperationFeedback -= new OperationFeedbackEventHandler(ProviderProgress);
            // association will go away when provider is removed
            for (int n = 0; n < providers.Count; n++)
            {
                WeakReference wr = providers[n] as WeakReference;
                if (wr.Target == provider)
                {
                    providers.RemoveAt(n);
                    return;
                }
            }
        }

        /// <summary>
        /// Gets / sets the delay in milliseconds before Progress events about an operation should be raised.
        /// </summary>
        public int Delay
        {
            get { return delay; }
            set { delay = value; }
        }

        private void ProviderProgress(object sender, OperationFeedbackEventArgs e)
        {
            if (e.Ticks > Delay || progressFired)
            {
                OnProgress(e);
                progressFired = true;
            }
            if (e.Milestone == OperationMilestone.Finished)
                progressFired = false;

        }

        /// <summary>
        /// Raises the <see cref="OperationFeedbackListener.Progress"/> event.
        /// </summary>
        /// <param name="e">An <see cref="OperationFeedbackEventArgs" /> that contains the event data.</param>
        protected virtual void OnProgress(OperationFeedbackEventArgs e)
        {
            if (Progress != null)
                Progress(this, e);
        }
    }

    /// <summary>
    /// Holds status information about an ongoing operation.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public sealed class OperationFeedbackEventArgs : CancelEventArgs
    {
        // Fields
        private int percent;
        private string name;
        private string description;
        private bool allowCancel;
        private bool allowRollback;
        private bool rollback;
        private OperationMilestone milestone;
        private int ticks;

        /// <summary>
        /// Initializes the event args.
        /// </summary>
        /// <param name="milestone">The current status of the operation.</param>
        /// <param name="name">The name of the operation.</param>
        /// <param name="description">A textual description for the operation.</param>
        /// <param name="percent">The percentage of work the operation completed so far or -1 if the operation
        /// is not able to calculate a meaningful percentage value.</param>
        /// <param name="ticks"> The number of milliseconds that passed since the operation started.</param>
        /// <param name="allowCancel">Indicates whether the operation can be canceled by the user.</param>
        /// <param name="allowRollback">Indicates whether changes made by the operation can be rolled back if the
        /// operation is canceled by the user.</param>
        public OperationFeedbackEventArgs(OperationMilestone milestone, string name, string description, int percent, int ticks, bool allowCancel, bool allowRollback)
        {
            this.milestone = milestone;
            this.name = name;
            this.description = description;
            this.percent = percent;
            this.allowCancel = allowCancel;
            this.allowRollback = allowRollback;
            this.ticks = ticks;
        }

        /// <summary>
        /// Returns the percentage of work the operation completed so far; -1 if the operation
        /// is not able to calculate a meaningful percentage value.
        /// </summary>
        public int Percent
        {
            get { return percent; }
        }

        /// <summary>
        /// Returns the name of the operation.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Returns the textual description for the operation.
        /// </summary>
        public string Description
        {
            get { return description; }
        }

        /// <summary>
        /// Indicates whether the operation can be canceled by the user.
        /// </summary>
        public bool AllowCancel
        {
            get { return allowCancel; }
        }

        /// <summary>
        /// Indicates whether changes made by the operation can be rolled back if the
        /// operation is canceled by the user.
        /// </summary>
        public bool AllowRollback
        {
            get { return allowCancel; }
        }

        /// <summary>
        /// Indicates whether the operation should roll back.
        /// </summary>
        public bool Rollback
        {
            get { return rollback; }
            set { rollback = value; }
        }

        /// <summary>
        /// Returns the current status of the operation.
        /// </summary>
        public OperationMilestone Milestone
        {
            get { return milestone; }
        }

        /// <summary>
        /// Returns the number of milliseconds that passed since the operation started.
        /// </summary>
        public int Ticks
        {
            get { return ticks; }
        }
    }

    /// <summary>
    /// Represents the method that will handle the OperationFeedbackEvent event of certain classes.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An OperationFeedbackEventArgs object that
    /// contains the event data.</param>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public delegate void OperationFeedbackEventHandler(object sender, OperationFeedbackEventArgs e);

    /// <summary>
    /// The class that implements the component where operations are operated on should
    /// implement this interface.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public interface IOperationFeedbackProvider
    {
        /// <summary>
        /// Raises an RaiseOperationFeedbackEvent.
        /// </summary>
        /// <param name="e">An OperationFeedbackEventArgs object that
        /// contains the event data.</param>
        void RaiseOperationFeedbackEvent(OperationFeedbackEventArgs e);

        /// <summary>
        /// Returns the stack for nested operations.
        /// </summary>
        Stack<OperationFeedback> FeedbackStack { get; }

        /// <summary>
        /// Occurs to indicate the progress of an ongoing operation.
        /// </summary>
        event OperationFeedbackEventHandler OperationFeedback;
    }

    /// <summary>
    /// Use this class when you want to provide feedback during time-consuming operations or
    /// if you want the user to be able to abort an operation.
    /// </summary>
    /// <remarks>
    /// You need to implement IOperationFeedbackProvider in your class.
    /// <code>
    /// // interface IOperationFeedbackProvider
    /// public event OperationFeedbackEventHandler OperationFeedback;
    /// Stack feedbackStack = new Stack();
    /// void IOperationFeedbackProvider.RaiseOperationFeedbackEvent(OperationFeedbackEventArgs e)
    /// {
    ///     if (OperationFeedback != null)
    ///             OperationFeedback(this, e);
    /// }
    /// Stack IOperationFeedbackProvider.FeedbackStack
    /// {
    ///     get { return feedbackStack; }
    /// }
    /// </code>
    ///
    /// When you implement an operation that you want to be cancellable or where
    /// you want to show feedback (e.g. display percentage in status bar) you
    /// do this by creating an OperationFeedback object inside a using statement.<para/>
    ///
    /// Example:
    /// <code>
    /// using (OperationFeedback op = new OperationFeedback(this))
    /// {
    ///     op.Name = "Cell";
    ///     op.Description = "Command Description";
    ///     op.AllowCancel = true;
    ///     op.AllowNestedProgress = true;
    ///     op.AllowNestedFeedback = false;
    ///     while (n++ != 100)
    ///     {
    ///         if (op.ShouldCancel())
    ///             return;
    ///
    ///         op.PercentComplete = n;
    ///     }
    /// }
    ///     </code>
    ///     It is also supported in nest operations in case your method calls other
    ///     routines that also use OperationFeedback. AllowNestedProgress will disable
    ///     OperationFeedback and OperationProgress / ShouldCancel in nested routines.
    ///     AllowNestedFeedback will simply prohibit changing the description. But
    ///     the object will still fire OperationProgress events.<para/>
    ///
    ///     A sample for a consumer is the DelayedWaitCursor class. You can assign a DelayedWaitCursor
    ///     to a grid table. The DelayedWaitCursor object will listen to OperationFeedback events and
    ///     automatically change the cursor to a wait cursor if operations take more time.
    ///     </remarks>
    ///
    /// <seealso cref="Syncfusion.Windows.Forms.DelayedWaitCursor"/>
    /// <seealso cref="Syncfusion.Windows.Forms.DelayedStatusDialog"/>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public sealed class OperationFeedback : IDisposable
    {
        // Fields
        IOperationFeedbackProvider feedback;
        int tickCount;
        bool canceled = false;
        bool started = false;

        string name = "";
        string description = "";
        int percentage = -1;
        int counter = 0;
        int seriesCount = 1;
        int seriesIndex = -1;
        bool allowCancel = false;
        bool allowRollback = false;
        bool rollback = false;
        bool allowNestedFeedback = true;
        bool allowNestedProgress = true;
        bool outerNestedFeedback = true;
        bool outerNestedProgress = true;
        OperationFeedback outer = null;
        int lastTick = int.MaxValue;

        // Constructor
        /// <summary>
        /// Initializes a new OperationFeedback object and pushes the object
        /// onto the providers feedback stack.
        /// </summary>
        /// <param name="feedback">Component that implements IOperationFeedbackProvider.</param>
        public OperationFeedback(IOperationFeedbackProvider feedback)
        {
            if (feedback == null)
                throw new ArgumentNullException();
            this.feedback = feedback;
            if (FeedbackStack.Count > 0)
                outer = FeedbackStack.Peek() as OperationFeedback;
            if (outer != null)
            {
                name = outer.name;
                description = outer.description;
                percentage = outer.percentage;
                counter = outer.counter;
                allowCancel = outer.allowCancel;
                outerNestedFeedback = allowNestedFeedback = outer.allowNestedFeedback;
                outerNestedProgress = allowNestedProgress = outer.allowNestedProgress;
                tickCount = outer.tickCount;
                outer.seriesIndex++;
            }
            else
                tickCount = int.MaxValue;

            FeedbackStack.Push(this);
        }

        /// <summary>
        /// Closes the current operation. Removes the object from FeedbackStack.
        /// </summary>
        void IDisposable.Dispose()
        {
            OnClose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Call this method to signal the start of the operation. If you do not explicitly
        /// call this routine, it will be called automatically the first time Progress or ShouldCancel
        /// is called.
        /// </summary>
        public void Start()
        {
            started = true;
            OnStart();
        }

        /// <summary>
        /// Call this method to signal the end of the operation. If you do not explicitly
        /// call this routine it will be called automatically when you are using the "using"
        /// statement in C# and / or when Dispose is called.
        /// </summary>
        public void Close()
        {
            OnClose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Call this method inside a loop from your method that implements the operation.
        /// </summary>
        public void Progress()
        {
            if (!canceled)
                canceled = !OnProgress();
        }


        /// <summary>
        /// Gets / sets the number of series if you have a series of nested operations.
        /// </summary>
        /// <remarks>
        /// If you know the number of operations in advance, you should specify this value.
        /// This allows OperationFeedback to adjust the percentage display to reflect the percentage
        /// in the overall operation. If you have two operations, the first operation will show percentage
        /// from 0 to 50 and the second from 51 to 100. In the original program code for these operations, you
        /// can keep your original calculations (setting percentage from 0 to 100).
        /// OperationFeedback simply divides the percentage by SeriesCount when it fires the Progress event.
        /// </remarks>
        public int SeriesCount
        {
            get
            {
                return seriesCount;
            }
            set
            {
                seriesIndex = -1;
                seriesCount = value;
            }
        }


        /// <summary>
        /// Indicate whether this operation is nested inside another operation.
        /// </summary>
        public bool IsNested
        {
            get
            {
                return outer != null;
            }
        }

        /// <summary>
        /// Indicates whether nested operations are enabled / disabled.
        /// </summary>
        public bool AllowNestedProgress
        {
            get { return allowNestedProgress; }
            set
            {
                // can only be modified if outer operations allows nested progress
                if (outerNestedProgress)
                    allowNestedProgress = value;
            }
        }

        /// <summary>
        /// Indicates whether calls to Progress or ShouldCancel have any effect.
        /// </summary>
        public bool ShouldCallProgress
        {
            get { return outerNestedProgress; }
        }

        /// <summary>
        /// Indicates whether the percent complete and description should be shown or discarded
        /// for this operation.
        /// </summary>
        public bool ShouldShowFeedback
        {
            get { return outerNestedFeedback; }
        }

        /// <summary>
        /// Indicates whether percentage display and description for nested operations are enabled / disabled.
        /// </summary>
        public bool AllowNestedFeedback
        {
            get { return allowNestedFeedback; }
            set
            {
                // can only be modified if outer operations allows nested feedback
                if (outerNestedFeedback)
                    allowNestedFeedback = value;
            }
        }

        /// <summary>
        /// Indicates whether the user wants to abort the operation. ShouldCancel will call
        /// Progress.
        /// </summary>
        public bool ShouldCancel
        {
            get
            {
                if (!canceled && ShouldCallProgress)
                    canceled = !OnProgress();
                if (canceled && outer != null)
                    outer.canceled = true;
                return canceled;
            }
        }

        /// <summary>
        /// Indicates whether cancelling the current operation is enabled / disabled.
        /// </summary>
        public bool AllowCancel
        {
            get { return allowCancel; }
            set { allowCancel = value; }
        }

        /// <summary>
        /// Indicates whether rolling back (undoing) the current operation is enabled / disabled.
        /// </summary>
        public bool AllowRollback
        {
            get { return allowRollback; }
            set { allowRollback = value; }
        }


        /// <summary>
        /// Indicates whether the canceled operation should be rolled back.
        /// </summary>
        public bool Rollback
        {
            get { return rollback; }
            set { rollback = value; }
        }

        /// <summary>
        /// Returns False.
        /// </summary>
        public bool RollbackConfirmed
        {
            get { return false; }
        }

        /// <summary>
        /// Gets / sets the short name of the operation.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Returns the number of ticks elapsed since the operation was started.
        /// </summary>
        public int Ticks
        {
            get
            {
                return (tickCount == int.MaxValue) ? 0 : Environment.TickCount - tickCount;
            }
        }


        /// <summary>
        /// Gets / sets the description of the operation. Use localized string (SR.GetString("...")) if necessary.
        /// </summary>
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
            }
        }

        /// <summary>
        /// Gets / sets the progress in percentage for the current operation.
        /// </summary>
        public int PercentComplete
        {
            get { return percentage; }
            set
            {
                percentage = value;
            }
        }

        /// <summary>
        /// Returns the number of counts of calls to Progress.
        /// </summary>
        public int Counter
        {
            get { return counter; }
        }

        private Stack<OperationFeedback> FeedbackStack
        {
            get { return feedback.FeedbackStack; }
        }

        private void OnStart()
        {
            try
            {
                if (tickCount == int.MaxValue)
                    tickCount = Environment.TickCount;

                if (!IsNested)
                {
                    OperationFeedbackEventArgs e = new OperationFeedbackEventArgs(OperationMilestone.Start,
                        name, description, percentage, 0, false, false);
                    feedback.RaiseOperationFeedbackEvent(e);
                }
            }
            catch (Exception )
            {
                throw;
            }
        }

        private int GetAdjustePercentage()
        {
            int percent = percentage;
            if (outer != null && outer.SeriesCount > 1 && outer.seriesIndex >= 0)
                percent = (percentage / outer.SeriesCount) + (100 / outer.SeriesCount) * outer.seriesIndex;
            return percent;
        }

        private bool OnProgress()
        {
            if (!started)
                Start();
            counter++;

            if (lastTick != int.MaxValue && Environment.TickCount - lastTick < 30)
                return true;
            lastTick = Environment.TickCount;
            try
            {
                OperationFeedbackEventArgs e = new OperationFeedbackEventArgs(OperationMilestone.Progress,
                    name, description, GetAdjustePercentage(), Environment.TickCount - tickCount, allowCancel, allowRollback);
                feedback.RaiseOperationFeedbackEvent(e);
                this.rollback = e.Rollback;
                return !(allowCancel && e.Cancel);
            }
            catch (Exception )
            {
                throw;
                // if any exception is thrown, force cancellation of current command
                // return false; Unreachable code
            }
        }

        private void OnClose()
        {
            FeedbackStack.Pop();
            try
            {
                if (!IsNested)
                {
                    OperationFeedbackEventArgs e = new OperationFeedbackEventArgs(OperationMilestone.Finished,
                        name, description, GetAdjustePercentage(), Environment.TickCount - tickCount, false, false);
                    feedback.RaiseOperationFeedbackEvent(e);
                }
            }
            catch (Exception )
            {
                throw;
            }
        }
    }

}
