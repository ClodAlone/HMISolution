#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Controls
{
    /// <summary>
    /// Base class for controls that represents controls that can spin.
    /// </summary>
    /// <remarks>
    /// Spinner abstract class defines and implements common and focused visual state groups.
    /// Spinner abstract class defines and implements Spin event and OnSpin method.
    /// </remarks>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]

    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]

    [TemplateVisualState(Name = VisualStates.StateIncreaseEnabled, GroupName = VisualStates.GroupIncrease)]
    [TemplateVisualState(Name = VisualStates.StateIncreaseDisabled, GroupName = VisualStates.GroupIncrease)]

    [TemplateVisualState(Name = VisualStates.StateDecreaseEnabled, GroupName = VisualStates.GroupDecrease)]
    [TemplateVisualState(Name = VisualStates.StateDecreaseDisabled, GroupName = VisualStates.GroupDecrease)]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
        Type = typeof(Spinner), XamlResource = "/Syncfusion.Theming.Blend;component/Spinner.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(Spinner), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/Spinner.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(Spinner), XamlResource = "/Syncfusion.Theming.Office2007Black;component/Spinner.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(Spinner), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/Spinner.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(Spinner), XamlResource = "/Syncfusion.Theming.Default;component/Spinner.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(Spinner), XamlResource = "/Syncfusion.Theming.Office2003;component/Spinner.xaml")]
    public abstract partial class Spinner : Control, IUpdateVisualState
    {
        #region public ValidSpinDirections ValidSpinDirection
        /// <summary>
        /// Gets or sets the spin direction that is currently valid.
        /// </summary>
        public ValidSpinDirections ValidSpinDirection
        {
            get { return (ValidSpinDirections)GetValue(ValidSpinDirectionProperty); }
            set { SetValue(ValidSpinDirectionProperty, value); }
        }

        /// <summary>
        /// Identifies the ValidSpinDirection dependency property.
        /// </summary>
        public static readonly DependencyProperty ValidSpinDirectionProperty =
            DependencyProperty.Register(
                "ValidSpinDirection",
                typeof(ValidSpinDirections),
                typeof(Spinner),
                new PropertyMetadata(ValidSpinDirections.Increase | ValidSpinDirections.Decrease, OnValidSpinDirectionPropertyChanged));

        /// <summary>
        /// ValidSpinDirectionProperty property changed handler.
        /// </summary>
        /// <param name="d">ButtonSpinner that changed its ValidSpinDirection.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnValidSpinDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Spinner source = (Spinner)d;
            ValidSpinDirections oldvalue = (ValidSpinDirections)e.OldValue;
            ValidSpinDirections newvalue = (ValidSpinDirections)e.NewValue;

            source.OnValidSpinDirectionChanged(oldvalue, newvalue);
        }
        #endregion public ValidSpinDirections ValidSpinDirection
        
        /// <summary>
        /// Gets or sets the helper that provides all of the standard
        /// interaction functionality. Making it internal for subclass access.
        /// </summary>
        internal InteractionHelper Interaction { get; set; }

        /// <summary>
        /// Occurs when spinning is initiated by the end-user.
        /// </summary>
        public event EventHandler<SpinEventArgs> Spin;

        /// <summary>
        /// Initializes a new instance of the Spinner class.
        /// </summary>
        protected Spinner()
        {
            Interaction = new InteractionHelper(this);
        }

        /// <summary>
        /// Raises the OnSpin event when spinning is initiated by the end-user.
        /// </summary>
        /// <param name="e">Spin event args.</param>
        protected virtual void OnSpin(SpinEventArgs e)
        {
            ValidSpinDirections valid = e.Direction == SpinDirection.Increase ? ValidSpinDirections.Increase : ValidSpinDirections.Decrease;

            if ((ValidSpinDirection & valid) != valid)
            {
                // spin is not allowed.
                throw new InvalidOperationException(Properties.Resources.Spinner_SpinNotValid);
            }

            EventHandler<SpinEventArgs> handler = Spin;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Called when valid spin direction changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnValidSpinDirectionChanged(ValidSpinDirections oldValue, ValidSpinDirections newValue)
        {
            UpdateVisualState(true);
        }

        #region visual state management
        /// <summary>
        /// Update current visual state.
        /// </summary>
        /// <param name="useTransitions">
        /// True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        internal virtual void UpdateVisualState(bool useTransitions)
        {
            // Handle the Common and Focused states
            Interaction.UpdateVisualStateBase(useTransitions);

            // increment state
            VisualStateManager.GoToState(
                this,
                ((ValidSpinDirection & ValidSpinDirections.Increase) == ValidSpinDirections.Increase) ?
                    VisualStates.StateIncreaseEnabled : VisualStates.StateIncreaseDisabled,
                    useTransitions);

            // decrement state
            VisualStateManager.GoToState(
                this,
                ((ValidSpinDirection & ValidSpinDirections.Decrease) == ValidSpinDirections.Decrease) ?
                    VisualStates.StateDecreaseEnabled : VisualStates.StateDecreaseDisabled,
                    useTransitions);
        }

        /// <summary>
        /// Update the visual state of the control.
        /// </summary>
        /// <param name="useTransitions">A value indicating whether to automatically generate transitions to the new state, or instantly transition to the new state.</param>
        void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        {
            UpdateVisualState(useTransitions);
        }
        #endregion
    }
}
