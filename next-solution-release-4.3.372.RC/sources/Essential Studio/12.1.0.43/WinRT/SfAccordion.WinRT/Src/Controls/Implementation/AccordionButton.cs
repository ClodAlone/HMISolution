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

#if WINDOWS_PHONE
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Media;
namespace Syncfusion.WP.Controls.Navigation
#else
#if SILVERLIGHT
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Media;
namespace Syncfusion.Tools.Controls.Layout
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;
namespace Syncfusion.UI.Xaml.Controls.Layout
#endif
#endif
{
    /// <summary>
    /// Represents the header for an accordion item.
    /// </summary>
    /// <remarks>By creating a seperate control, there is more flexibility in 
    /// the templating possibilities.</remarks>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]

    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]

    [TemplateVisualState(Name = VisualStates.StateExpanded, GroupName = VisualStates.GroupExpansion)]
    [TemplateVisualState(Name = VisualStates.StateCollapsed, GroupName = VisualStates.GroupExpansion)]
    public class AccordionButton : ToggleButton
    {
        #region Parent AccordionItem
        /// <summary>
        /// Gets or sets a reference to the parent AccordionItem 
        /// of an AccordionButton.
        /// </summary>
        /// <value>The parent accordion item.</value>
        internal SfAccordionItem ParentAccordionItem { get; set; }
        #endregion Parent AccordionItem

        /// <summary>
        /// Initializes a new instance of the <see cref="AccordionButton"/> 
        /// class.
        /// </summary>
        public AccordionButton()
        {
            DefaultStyleKey = typeof(AccordionButton);
        }

        #region public SolidColorBrush AccentBrush

        /// <summary>
        /// Gets or sets the accent brush
        /// </summary>
        public SolidColorBrush AccentBrush
        {
            get { return (SolidColorBrush)GetValue(AccentBrushProperty); }
            set { SetValue(AccentBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AccentBrush.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AccentBrushProperty =
            DependencyProperty.Register("AccentBrush", typeof(SolidColorBrush), typeof(AccordionButton), new PropertyMetadata(null));

        #endregion

        /// <summary>
        /// Occurs when the focus is obtained
        /// </summary>
        /// <param name="e"></param>
#if !WINRT
        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);
#else
            
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
#endif
            if (ParentAccordionItem != null && !ParentAccordionItem.IsSelected)
            {
                VisualStates.GoToState(this, true, VisualStates.StateMouseOver);
            }
        }

        /// <summary>
        /// Occurs when the focus is lost
        /// </summary>
        /// <param name="e"></param>
#if !WINRT
        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);
#else
            protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
#endif
            VisualStates.GoToState(this, true, VisualStates.StateNormal);
        }

        /// <summary>
        /// Updates the state of the visual.
        /// </summary>
        /// <param name="useTransitions">If set to <c>true</c> use transitions.</param>
        /// <remarks>The header will follow the parent accordionitem states.</remarks>
        internal virtual void UpdateVisualState(bool useTransitions)
        {
            // the visualstate of the header is completely dependent on the parent state.
            if (ParentAccordionItem == null)
            {
                return;
            }

            if (ParentAccordionItem.IsSelected)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateExpanded);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateCollapsed);
            }
        }
    }
}
