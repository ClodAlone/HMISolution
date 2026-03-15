#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.UI.Xaml.Controls
{
    /// <summary>
    ///  LoopingSelectorItem is a <see
    /// cref="N:Windows.UI.Xaml.Controls.ContentControl"/>. It is a selectable item
    /// inside the <see cref="T:Syncfusion.UI.Xaml.Controls.LoopingSelector"/>.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class LoopingSelectorItem : ContentControl
    {
        #region variables
        
        private const string TransformPartName = "Transform";

        private const string CommonGroupName = "Common";
        
        private const string NormalStateName = "Normal";
        
        private const string ExpandedStateName = "Expanded";
        
        private const string SelectedStateName = "Selected";

        private const string DisabledStateName = "Disabled";

        private bool _shouldClick;

        internal bool IsTapped = false;

        internal Rectangle Part_Disabled;
        
        #endregion

        #region enum

        /// <summary>
        /// Represents an enum list for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.LoopingSelectorItem.State"/>
        /// </summary>
        public enum State
        {
            /// <summary>
            /// Not visible
            /// </summary>
            Normal,
            /// <summary>
            /// Visible
            /// </summary>
            Expanded,
            /// <summary>
            /// Selected
            /// </summary>
            Selected,
            /// <summary>
            /// Disabled
            /// </summary>
            Disabled
        };
        
        /// <summary>
        /// Declares a variable for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.LoopingSelectorItem.State"/>
        /// </summary>
        public State _state;

        #endregion

        #region construtor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.LoopingSelectorItem"/> class.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public LoopingSelectorItem()
        {
            DefaultStyleKey = typeof(LoopingSelectorItem);
            IsTabStop = false;
            PointerPressed += LoopingSelectorItem_MouseLeftButtonDown;
            PointerReleased += LoopingSelectorItem_MouseLeftButtonUp;
            PointerCaptureLost += LoopingSelectorItem_LostMouseCapture;
            Tapped += LoopingSelectorItem_Tap;
        }
        
        #endregion

        #region helper methods

        /// <summary>
        /// Sets the state <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.LoopingSelectorItem.State"/>
        /// </summary>
        /// <param name="newState"></param>
        /// <param name="useTransitions"></param>
        public void SetState(State newState, bool useTransitions)
        {
            if (_state != newState)
            {
                _state = newState;
                switch (_state)
                {
                    case State.Normal:
                        VisualStateManager.GoToState(this, NormalStateName, useTransitions);
                        if (Part_Disabled != null)
                            Part_Disabled.Visibility = Visibility.Collapsed;
                        break;
                    case State.Expanded:
                        VisualStateManager.GoToState(this, ExpandedStateName, useTransitions);
                        if (Part_Disabled != null)
                            Part_Disabled.Visibility = Visibility.Collapsed;
                        break;
                    case State.Selected:
                        VisualStateManager.GoToState(this, SelectedStateName, useTransitions);
                        if (Part_Disabled != null)
                            Part_Disabled.Visibility = Visibility.Collapsed;
                        break;
                    case State.Disabled:
                        VisualStateManager.GoToState(this, DisabledStateName, useTransitions);
                        if (Part_Disabled != null)
                            Part_Disabled.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        internal State GetState() { return _state; }

        internal LoopingSelectorItem Previous { get; private set; }

        internal LoopingSelectorItem Next { get; private set; }

        internal void Remove()
        {
            if (Previous != null)
            {
                Previous.Next = Next;
            }
            if (Next != null)
            {
                Next.Previous = Previous;
            }
            Next = Previous = null;
        }

        internal void InsertAfter(LoopingSelectorItem after)
        {
            Next = after.Next;
            Previous = after;

            if (after.Next != null)
            {
                after.Next.Previous = this;
            }

            after.Next = this;
        }

        internal void InsertBefore(LoopingSelectorItem before)
        {
            Next = before;
            Previous = before.Previous;

            if (before.Previous != null)
            {
                before.Previous.Next = this;
            }

            before.Previous = this;
        }

        internal TranslateTransform Transform { get; private set; }

        void LoopingSelectorItem_Tap(object sender, TappedRoutedEventArgs e)
        {
            (sender as LoopingSelectorItem).IsTapped = true;
            //e.Handled = true;
        }

        void LoopingSelectorItem_MouseLeftButtonDown(object sender, PointerRoutedEventArgs e)
        {
            //CapturePointer(e.Pointer);
            _shouldClick = true;
        }

        void LoopingSelectorItem_MouseLeftButtonUp(object sender, PointerRoutedEventArgs e)
        {
            //ReleasePointerCapture(e.Pointer);

            if (_shouldClick)
            {
                _shouldClick = false;
                SafeRaise.Raise(Click, this);
            }
        }

        void LoopingSelectorItem_LostMouseCapture(object sender, RoutedEventArgs e)
        {
            _shouldClick = false;
        }

        #endregion

        #region event

        /// <summary>
        /// The Click event. This is needed because there is no gesture for touch-down, pause 
        /// longer than the Hold time, and touch-up. Tap will not be raise, and Hold is not 
        /// adequate.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public event EventHandler<EventArgs> Click;

        #endregion

        #region override

        /// <summary>
        /// Initializes all the child elements of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.LoopingSelectorItem"/> control.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Part_Disabled = GetTemplateChild("Part_Disabled") as Rectangle;
            Transform = GetTemplateChild(TransformPartName) as TranslateTransform ?? new TranslateTransform();
        }

        #endregion

        #region dependency property

        /// <summary>
        /// Gets or sets the brush that is applied as the background of the item.
        /// </summary>
        public Brush AccentBrush
        {
            get { return (Brush)GetValue(AccentBrushProperty); }
            set { SetValue(AccentBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AccentBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AccentBrushProperty =
            DependencyProperty.Register("AccentBrush", typeof(Brush), typeof(LoopingSelectorItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the brush that is applied as the background of the item.
        /// </summary>
        public Brush SelectedForeground
        {
            get { return (Brush)GetValue(SelectedForegroundProperty); }
            set { SetValue(SelectedForegroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AccentBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedForegroundProperty =
            DependencyProperty.Register("SelectedForeground", typeof(Brush), typeof(LoopingSelectorItem), new PropertyMetadata(null));

        #endregion

    }
}
