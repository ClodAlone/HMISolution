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
using System.Collections;
using System.Diagnostics;
using Syncfusion.UI.Xaml.Controls.Extensions;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Syncfusion.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents a control that redistributes space between the rows of
    /// columns of a <see cref="T:System.Windows.Controls.Grid" /> control.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    [TemplatePart(Name = SfGridSplitter.ElementHorizontalTemplateName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = SfGridSplitter.ElementVerticalTemplateName, Type = typeof(FrameworkElement))]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [StyleTypedProperty(Property = "PreviewStyle", StyleTargetType = typeof(Control))]
    public partial class SfGridSplitter : Control
    {
        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementHorizontalTemplateName = "HorizontalTemplate";

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementVerticalTemplateName = "VerticalTemplate";

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal FrameworkElement ElementHorizontalTemplateFrameworkElement { get; set; }

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal FrameworkElement ElementVerticalTemplateFrameworkElement { get; set; }

        internal int splitterindex = 0;
        internal GridLength Definition1GridLength;
        internal GridLength Definition2GridLength;
        internal Button Part_Up;
        internal Button Part_Down;
        internal Button Part_Left;
        internal Button Part_Right;
        internal bool isMouseOverbutton;
        /// <summary>
        /// Identifies the
        /// <see cref="P:System.Windows.Controls.SfGridSplitter.ShowsPreview" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// An identifier for the
        /// <see cref="P:System.Windows.Controls.SfGridSplitter.ShowsPreview" />
        /// dependency property.
        /// </value>
        public static readonly DependencyProperty ShowsPreviewProperty =
            DependencyProperty.Register(
                "ShowsPreview",
                typeof(bool),
                typeof(SfGridSplitter),
                null);

        /// <summary>
        /// Identifies the
        /// <see cref="P:System.Windows.Controls.SfGridSplitter.PreviewStyle" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// An identifier for the
        /// <see cref="P:System.Windows.Controls.SfGridSplitter.PreviewStyle" />
        /// dependency property.
        /// </value>
        public static readonly DependencyProperty PreviewStyleProperty =
            DependencyProperty.Register(
                "PreviewStyle",
                typeof(Style),
                typeof(SfGridSplitter),
                null);

        /// <summary>
        /// Called when the IsEnabled property changes.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Property changed args.</param>
        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Debug.Assert(e.NewValue is bool, "The new value should be a boolean!");
            bool isEnabled = (bool)e.NewValue;

            if (!isEnabled)
            {
                _isMouseOver = false;
            }
            ChangeVisualState();
        }

        /// <summary>
        /// Gets or sets the resize data.  This is null unless a resize
        /// operation is in progress.
        /// </summary>
        internal ResizeData ResizeDataInternal { get; set; }

        /// <summary>
        /// Is Null until a resize operation is initiated with ShowsPreview ==
        /// True, then it persists for the life of the SfGridSplitter.
        /// </summary>
        private Canvas _previewLayer;

        /// <summary>
        /// Is initialized in the constructor.
        /// </summary>
        private DragValidator _dragValidator;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private GridResizeDirection _currentGridResizeDirection = GridResizeDirection.Auto;

        /// <summary>
        /// Holds the state for whether the mouse is over the control or not.
        /// </summary>
        private bool _isMouseOver;

        /// <summary>
        /// Default increment parameter.
        /// </summary>
        private const double DragIncrement = 1.0;

        /// <summary>
        /// Default increment parameter.
        /// </summary>
        private const double KeyboardIncrement = 10.0;
        
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Controls.SfGridSplitter" /> class.
        /// </summary>
        public SfGridSplitter()
        {
            IsEnabledChanged += OnIsEnabledChanged;
            KeyDown += SfGridSplitter_KeyDown;
            LayoutUpdated += delegate { UpdateTemplateOrientation(); };
            _dragValidator = new DragValidator(this);
            _dragValidator.DragStartedEvent += DragValidator_DragStartedEvent;
            _dragValidator.DragDeltaEvent += DragValidator_DragDeltaEvent;
            _dragValidator.DragCompletedEvent += DragValidator_DragCompletedEvent;
            Loaded += SfGridSplitter_Loaded;
            PointerMoved += SfGridSplitter_PointerMoved;
            PointerEntered += delegate
                {
                _isMouseOver = true;
                ChangeVisualState();
            };

            PointerExited += delegate
                {
                _isMouseOver = false;

                // Only change the visual state if we're not currently resizing,
                // the visual state will get updated when the resize operation
                // comples
                if (ResizeDataInternal == null)
                {
                    ChangeVisualState();
                }
            };

            Loaded += delegate
            {
                ChangeVisualState();
            };

            GotFocus += delegate
            {
                ChangeVisualState();
            };

            LostFocus += delegate
            {
                ChangeVisualState();
            };

            DefaultStyleKey = typeof(SfGridSplitter);
        }

        void SfGridSplitter_Loaded(object sender, RoutedEventArgs e)
        {
            if (ElementVerticalTemplateFrameworkElement != null && ElementVerticalTemplateFrameworkElement.Visibility == Visibility.Visible)
            {
                if(ReadLocalValue(UpButtonTemplateProperty)==DependencyProperty.UnsetValue)
                    UpButtonTemplate = ElementVerticalTemplateFrameworkElement.Resources["DefaultLeftButtonTemplate"] as ControlTemplate;
                if(ReadLocalValue(DownButtonTemplateProperty)==DependencyProperty.UnsetValue)
                    DownButtonTemplate = ElementVerticalTemplateFrameworkElement.Resources["DefaultRightButtonTemplate"] as ControlTemplate;
            }
        }
        void SfGridSplitter_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            DependencyObject button=FindAncestor(e.OriginalSource as DependencyObject);
            if ( button!= null)
            {
                isMouseOverbutton = true;
                _isMouseOver = false;
                FrameworkElementExtensions.SetCursor(this, CursorDisplayHandler.DefaultCursor);
            }
            if (!isMouseOverbutton && _isMouseOver)
            {
                ChangeVisualState();
            }
        }

        private DependencyObject FindAncestor(DependencyObject element)
        {
            if (element != null)
            {
                var _element = VisualTreeHelper.GetParent(element);
                if (!(_element is Button))
                {
                    return FindAncestor(_element);
                }
                else
                {
                    return _element;
                }
            }
            else
                return null;
        }
        /// <summary>
        /// Builds the visual tree for the
        /// <see cref="T:System.Windows.Controls.SfGridSplitter" />
        /// control when a new template is applied.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ElementHorizontalTemplateFrameworkElement = this.GetTemplateChild(SfGridSplitter.ElementHorizontalTemplateName) as FrameworkElement;
            ElementVerticalTemplateFrameworkElement = this.GetTemplateChild(SfGridSplitter.ElementVerticalTemplateName) as FrameworkElement;
            Part_Up = this.GetTemplateChild("PART_Up") as Button;
            if (Part_Up != null)
            {
                Part_Up.Click += Part_Up_Click;
                Part_Up.PointerEntered += CollapseButton_PointerEntered;
                Part_Up.PointerExited += CollapseButton_PointerExited;
            }
            Part_Down = this.GetTemplateChild("PART_Down") as Button;
            if (Part_Down != null)
            {
                Part_Down.Click += Part_Down_Click;
                Part_Down.PointerEntered += CollapseButton_PointerEntered;
                Part_Down.PointerExited += CollapseButton_PointerExited;
            }
            Part_Left = this.GetTemplateChild("PART_Left") as Button;
            if (Part_Left != null)
            {
                Part_Left.Click += Part_Up_Click;
                Part_Left.PointerEntered += CollapseButton_PointerEntered;
                Part_Left.PointerExited += CollapseButton_PointerExited;
            }
            Part_Right = this.GetTemplateChild("PART_Right") as Button;
            if (Part_Right != null)
            {
                Part_Right.Click += Part_Down_Click;
                Part_Right.PointerEntered+=CollapseButton_PointerEntered;
                Part_Right.PointerExited += CollapseButton_PointerExited;
            }
            // We need to recalculate the orientation, so set
            // _currentGridResizeDirection back to Auto
            _currentGridResizeDirection = GridResizeDirection.Auto;

            UpdateTemplateOrientation();
            if (ResizeDataInternal == null)
                InitializeData(false);
            if (ResizeDataInternal != null)
            {
                splitterindex = ResizeDataInternal.Definition1Index + 1;
                foreach (UIElement child in ResizeDataInternal.Grid.Children)
                {
                    if (child.GetType() == typeof(SfGridSplitter))
                    {
                        splitterindex = ResizeDataInternal.Grid.Children.IndexOf(child);
                        break;
                    }
                }
                Binding beforeContentBinding = new Binding();
                beforeContentBinding.Source = ResizeDataInternal.Grid.Children[GetIndex(splitterindex,false)];
                beforeContentBinding.Path = new PropertyPath("Visibility");
                beforeContentBinding.Mode = BindingMode.TwoWay;
                this.SetBinding(SfGridSplitter.ElementBeforeVisibilityProperty, beforeContentBinding);
                Binding afterContentBinding = new Binding();
                afterContentBinding.Source = ResizeDataInternal.Grid.Children[GetIndex(splitterindex,true)];
                afterContentBinding.Path = new PropertyPath("Visibility");
                afterContentBinding.Mode = BindingMode.TwoWay;
                this.SetBinding(SfGridSplitter.ElementAfterVisibilityProperty, afterContentBinding);
            }
            ChangeVisualState(false);
        }

        void CollapseButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            _isMouseOver = true;
            isMouseOverbutton = false;
        }

        void CollapseButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            isMouseOverbutton = true;
            FrameworkElementExtensions.SetCursor(this, CursorDisplayHandler.DefaultCursor);
        }

        void Part_Down_Click(object sender, RoutedEventArgs e)
        {
            if (ResizeDataInternal == null)
                InitializeData(false);
            SetupDefinitionsToResize();
            if (ResizeDataInternal.Grid.Children[GetIndex(splitterindex, false)].Visibility == Visibility.Collapsed)
                ResizeDataInternal.Grid.Children[GetIndex(splitterindex, false)].Visibility = Visibility.Visible;
            else if (ResizeDataInternal.Grid.Children[GetIndex(splitterindex, true)].Visibility == Visibility.Visible)
                ResizeDataInternal.Grid.Children[GetIndex(splitterindex, true)].Visibility = Visibility.Collapsed;
        }

        void Part_Up_Click(object sender, RoutedEventArgs e)
        {
            if (ResizeDataInternal == null)
                InitializeData(false);
            SetupDefinitionsToResize();
            if (ResizeDataInternal.Grid.Children[GetIndex(splitterindex ,true)].Visibility == Visibility.Collapsed)
                ResizeDataInternal.Grid.Children[GetIndex(splitterindex, true)].Visibility = Visibility.Visible;
            else if (ResizeDataInternal.Grid.Children[GetIndex(splitterindex,false)].Visibility == Visibility.Visible)
                ResizeDataInternal.Grid.Children[GetIndex(splitterindex, false)].Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the
        /// <see cref="T:System.Windows.Controls.SfGridSplitter" /> displays a
        /// preview.
        /// </summary>
        /// <value>
        /// True if a preview is displayed; otherwise, false.
        /// </value>
        public bool ShowsPreview
        {
            get { return (bool)GetValue(SfGridSplitter.ShowsPreviewProperty); }
            set { SetValue(SfGridSplitter.ShowsPreviewProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.Style" /> that is used
        /// for previewing changes.
        /// </summary>
        /// <value>
        /// The style that is used to preview changes.
        /// </value>
        public Style PreviewStyle
        {
            get { return (Style)GetValue(SfGridSplitter.PreviewStyleProperty); }
            set { SetValue(SfGridSplitter.PreviewStyleProperty, value); }
        }


        /// <summary>
        /// Gets or sets a value indicating the
        /// <see cref="T:System.Windows.Controls.SfGridSplitter" /> Up button 
        /// Template.
        /// </summary>
        public ControlTemplate UpButtonTemplate
        {
            get { return (ControlTemplate)GetValue(UpButtonTemplateProperty); }
            set { SetValue(UpButtonTemplateProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for UpButtonTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty UpButtonTemplateProperty =
            DependencyProperty.Register("UpButtonTemplate", typeof(ControlTemplate), typeof(SfGridSplitter), new PropertyMetadata(0));


        /// <summary>
        /// Gets or sets a value indicating the
        /// <see cref="T:System.Windows.Controls.SfGridSplitter" /> Down button 
        ///  Template.
        /// </summary>
        public ControlTemplate DownButtonTemplate
        {
            get { return (ControlTemplate)GetValue(DownButtonTemplateProperty); }
            set { SetValue(DownButtonTemplateProperty, value); }
        }
        /// <summary>        
        /// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DownButtonTemplateProperty =
            DependencyProperty.Register("DownButtonTemplate", typeof(ControlTemplate), typeof(SfGridSplitter), new PropertyMetadata(0));

        /// <summary>
        /// Gets or sets a value indicating the
        /// <see cref="T:System.Windows.Controls.SfGridSplitter" /> EnableCollapseButton 
        /// </summary>
        public bool EnableCollapseButton
        {
            get { return (bool)GetValue(EnableCollapseButtonProperty); }
            set { SetValue(EnableCollapseButtonProperty, value); }
        }
        /// <summary>        
        /// Using a DependencyProperty as the backing store for EnableCollapseButton.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EnableCollapseButtonProperty =
            DependencyProperty.Register("EnableCollapseButton", typeof(bool), typeof(SfGridSplitter), new PropertyMetadata(false));

        internal Visibility ElementBeforeVisibility
        {
            get { return (Visibility)GetValue(ElementBeforeVisibilityProperty); }
            set { SetValue(ElementBeforeVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BeforeContent.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ElementBeforeVisibilityProperty =
            DependencyProperty.Register("ElementBeforeVisibility", typeof(Visibility), typeof(SfGridSplitter), new PropertyMetadata(0, OnElementVisibilityChanged));



        internal Visibility ElementAfterVisibility
        {
            get { return (Visibility)GetValue(ElementAfterVisibilityProperty); }
            set { SetValue(ElementAfterVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AfterContent.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ElementAfterVisibilityProperty =
            DependencyProperty.Register("ElementAfterVisibility", typeof(Visibility), typeof(SfGridSplitter), new PropertyMetadata(0, OnElementVisibilityChanged));
      
               
        private static void OnElementVisibilityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfGridSplitter control = sender as SfGridSplitter;
            if (control.ResizeDataInternal == null)
                control.InitializeData(false);
            if (control.ResizeDataInternal.Grid.Children.Count > control.GetIndex(control.splitterindex, true) && control.ResizeDataInternal.Grid.Children[control.GetIndex(control.splitterindex,false)].Visibility == Visibility.Collapsed)
            {
                control.Definition1GridLength = control.ResizeDataInternal.OriginalDefinition1Length;
                control.Definition2GridLength = control.ResizeDataInternal.OriginalDefinition2Length;
                SetDefinitionLength(control.ResizeDataInternal.Definition1, new GridLength(GetActualLength(control.ResizeDataInternal.Definition1), GridUnitType.Auto));
                SetDefinitionLength(control.ResizeDataInternal.Definition2, new GridLength(GetActualLength(control.ResizeDataInternal.Definition2), GridUnitType.Star));
            }
            else if (control.ResizeDataInternal.Grid.Children.Count > control.GetIndex(control.splitterindex, true) && control.ResizeDataInternal.Grid.Children[control.GetIndex(control.splitterindex, true)].Visibility == Visibility.Collapsed)
            {
                control.Definition1GridLength = control.ResizeDataInternal.OriginalDefinition1Length;
                control.Definition2GridLength = control.ResizeDataInternal.OriginalDefinition2Length;
                SetDefinitionLength(control.ResizeDataInternal.Definition1, new GridLength(GetActualLength(control.ResizeDataInternal.Definition1), GridUnitType.Star));
                SetDefinitionLength(control.ResizeDataInternal.Definition2, new GridLength(GetActualLength(control.ResizeDataInternal.Definition2), GridUnitType.Auto));
            }
            else if ((Visibility)args.OldValue == Visibility.Collapsed && (Visibility)args.NewValue == Visibility.Visible)
            {
                SetDefinitionLength(control.ResizeDataInternal.Definition1, control.Definition1GridLength);
                SetDefinitionLength(control.ResizeDataInternal.Definition2, control.Definition2GridLength);
            }
        }
        
        /// <summary>
        /// Method to change the visual state of the control.
        /// </summary>
        private void ChangeVisualState()
        {
            ChangeVisualState(true);
        }

        private int GetIndex(int index,bool isnext)
        {
            if (ResizeDataInternal!=null)
            {
                if (ResizeDataInternal.Definition1.AsColumnDefinition != null)
                {
                    index = isnext? Grid.GetColumn(ResizeDataInternal.Grid.Children[splitterindex] as FrameworkElement) + 1
                        :Grid.GetColumn(ResizeDataInternal.Grid.Children[splitterindex] as FrameworkElement) - 1;
                    foreach (FrameworkElement element in ResizeDataInternal.Grid.Children)
                        if (Grid.GetColumn(element) == index)
                            return ResizeDataInternal.Grid.Children.IndexOf(element);
                         
                }
                else if (ResizeDataInternal.Definition1.AsRowDefinition != null)
                {
                    index = isnext? Grid.GetRow(ResizeDataInternal.Grid.Children[splitterindex] as FrameworkElement) + 1
                        : Grid.GetRow(ResizeDataInternal.Grid.Children[splitterindex] as FrameworkElement) - 1;
                    foreach (FrameworkElement element in ResizeDataInternal.Grid.Children)
                        if (Grid.GetRow(element) == index)
                            return ResizeDataInternal.Grid.Children.IndexOf(element);
                }
            }
            return index;
        }
        /// <summary>
        /// Change to the correct visual state for the SfGridSplitter.
        /// </summary>
        /// <param name="useTransitions">
        /// True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        private void ChangeVisualState(bool useTransitions)
        {
            if (!IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateDisabled, VisualStates.StateNormal);
            }
            else if (_isMouseOver)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateMouseOver, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateNormal);
            }

            if (HasKeyboardFocus && this.IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateFocused, VisualStates.StateUnfocused);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateUnfocused);
            }

			if(useTransitions)
			{
				if (GetEffectiveResizeDirection() == GridResizeDirection.Columns)
				{
					if (!_isMouseOver)
                        FrameworkElementExtensions.SetCursor(this, CursorDisplayHandler.DefaultCursor);
                    else
                        FrameworkElementExtensions.SetCursor(this, new CoreCursor(CoreCursorType.SizeWestEast, 1));
				}
				else
				{
					if (!_isMouseOver)
                        FrameworkElementExtensions.SetCursor(this, CursorDisplayHandler.DefaultCursor);
                    else
                        FrameworkElementExtensions.SetCursor(this, new CoreCursor(CoreCursorType.SizeNorthSouth, 1));
				}
			}
        }

        /// <summary>
        /// Handle the drag completed event to commit or cancel the resize
        /// operation in progress.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment.</param>
        /// <param name="e">Inherited code: Requires comment 1.</param>
        internal void DragValidator_DragCompletedEvent(object sender, DragCompletedEventArgs e)
        {
            if (ResizeDataInternal != null)
            {
                if (e.Canceled)
                {
                    CancelResize();
                }
                else
                {
                    if (ResizeDataInternal.ShowsPreview)
                    {
                        MoveSplitter(ResizeDataInternal.PreviewControl.OffsetX, ResizeDataInternal.PreviewControl.OffsetY);
                        RemovePreviewControl();
                    }
                }
                ResizeDataInternal = null;
            }
            ChangeVisualState();
        }

        /// <summary>
        /// Handle the drag delta event to update the UI for the resize
        /// operation in progress.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment.</param>
        /// <param name="e">Inherited code: Requires comment 1.</param>
        internal void DragValidator_DragDeltaEvent(object sender, DragDeltaEventArgs e)
        {
            if (ResizeDataInternal != null)
            {
                double horizontalChange = e.HorizontalChange;
                double verticalChange = e.VerticalChange;

                if (ResizeDataInternal.ShowsPreview)
                {
                    if (ResizeDataInternal.ResizeDirection == GridResizeDirection.Columns)
                    {
                        ResizeDataInternal.PreviewControl.OffsetX = Math.Min(Math.Max(horizontalChange, ResizeDataInternal.MinChange), ResizeDataInternal.MaxChange);
                    }
                    else
                    {
                        ResizeDataInternal.PreviewControl.OffsetY = Math.Min(Math.Max(verticalChange, ResizeDataInternal.MinChange), ResizeDataInternal.MaxChange);
                    }
                }
                else
                {
                    MoveSplitter(horizontalChange, verticalChange);
                }
            }
        }

        /// <summary>
        /// Handle the drag started event to start a resize operation if the
        /// control is enabled.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment.</param>
        /// <param name="e">Inherited code: Requires comment 1.</param>
        internal void DragValidator_DragStartedEvent(object sender, DragStartedEventArgs e)
        {
            if (this.IsEnabled)
            {
                Focus(FocusState.Programmatic);
                InitializeData(this.ShowsPreview);
            }
        }

        /// <summary>
        /// Handle the key down event to allow keyboard resizing or canceling a
        /// resize operation.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment.</param>
        /// <param name="e">Inherited code: Requires comment 1.</param>
        internal void SfGridSplitter_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKey.Left:
                    e.Handled = KeyboardMoveSplitter(FlipForRTL(-KeyboardIncrement), 0.0);
                    return;

                case VirtualKey.Up:
                    e.Handled = KeyboardMoveSplitter(0.0, -KeyboardIncrement);
                    return;

                case VirtualKey.Right:
                    e.Handled = KeyboardMoveSplitter(FlipForRTL(KeyboardIncrement), 0.0);
                    return;

                case VirtualKey.Down:
                    e.Handled = KeyboardMoveSplitter(0.0, KeyboardIncrement);
                    break;

                case VirtualKey.Escape:
                    if (ResizeDataInternal == null)
                    {
                        break;
                    }
                    CancelResize();
                    e.Handled = true;
                    return;

                default:
                    return;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the control has keyboard
        /// focus.
        /// </summary>
        private bool HasKeyboardFocus
        {
            get { return FocusManager.GetFocusedElement() == this; }
        }

        /// <summary>
        /// Move the splitter and resize the affected columns or rows.
        /// </summary>
        /// <param name="Change">
        /// Pixels to resize horizontally or vertically.
        /// </param>
        public void MoveSplitter(double Change)
        {
            if (ResizeDataInternal == null)
                InitializeData(false);
            double definition1ActualLength = GetActualLength(ResizeDataInternal.Definition1);
            double definition2ActualLength = GetActualLength(ResizeDataInternal.Definition2);
            if ((ResizeDataInternal.SplitBehavior == SplitBehavior.Split) && !DoubleUtil.AreClose((double)(definition1ActualLength + definition2ActualLength), (double)(ResizeDataInternal.OriginalDefinition1ActualLength + ResizeDataInternal.OriginalDefinition2ActualLength)))
            {
                ResizeDataInternal.SplitBehavior = SplitBehavior.ResizeDefinition1;
            }
            if (ElementVerticalTemplateFrameworkElement.Visibility == Visibility.Visible)
                MoveSplitter(Change,0);
            else
                MoveSplitter(0,Change);
        }

        /// <summary>
        /// Initialize the resize data and move the splitter by the specified
        /// amount.
        /// </summary>
        /// <param name="horizontalChange">
        /// Horizontal amount to move the splitter.
        /// </param>
        /// <param name="verticalChange">
        /// Vertical amount to move the splitter.
        /// </param>
        /// <returns>Inherited code: Requires comment.</returns>
        internal bool InitializeAndMoveSplitter(double horizontalChange, double verticalChange)
        {
            // resizing directly is not allowed if there is a mouse initiated
            // resize operation in progress
            if (ResizeDataInternal != null)
            {
                return false;
            }

            InitializeData(false);
            if (ResizeDataInternal == null)
            {
                return false;
            }

            MoveSplitter(horizontalChange, verticalChange);
            ResizeDataInternal = null;
            return true;
        }

        /// <summary>
        /// Called by keyboard event handler to move the splitter if allowed.
        /// </summary>
        /// <param name="horizontalChange">
        /// Horizontal amount to move the splitter.
        /// </param>
        /// <param name="verticalChange">
        /// Vertical amount to move the splitter.
        /// </param>
        /// <returns>Inherited code: Requires comment.</returns>
        private bool KeyboardMoveSplitter(double horizontalChange, double verticalChange)
        {
            if (HasKeyboardFocus && this.IsEnabled)
            {
                return InitializeAndMoveSplitter(horizontalChange, verticalChange);
            }
            return false;
        }

        /// <summary>
        /// Creates the preview layer and adds it to the parent grid.
        /// </summary>
        /// <param name="parentGrid">Grid to add the preview layer to.</param>
        private void CreatePreviewLayer(Grid parentGrid)
        {
            Debug.Assert(parentGrid != null, "parentGrid should not be null!");
            Debug.Assert(parentGrid.RowDefinitions != null, "parentGrid.RowDefinitions should not be null!");
            Debug.Assert(parentGrid.ColumnDefinitions != null, "parentGrid.ColumnDefinitions should not be null!");

            _previewLayer = new Canvas();

            // RowSpan and ColumnSpan default to 1 and should not be set to 0 in
            // the case that a Grid has been created without explicitly setting
            // its ColumnDefinitions or RowDefinitions
            if (parentGrid.RowDefinitions.Count > 0)
            {
                _previewLayer.SetValue(Grid.RowSpanProperty, parentGrid.RowDefinitions.Count);
            }
            if (parentGrid.ColumnDefinitions.Count > 0)
            {
                _previewLayer.SetValue(Grid.ColumnSpanProperty, parentGrid.ColumnDefinitions.Count);
            }

            // REMOVE_RTM: Uncomment once Jolt Bug 11276 is fixed
            // this.previewLayer.SetValue(Grid.ZIndex, int.MaxValue);
            parentGrid.Children.Add(_previewLayer);
        }

        /// <summary>
        /// Add the preview layer to the Grid if it is not there already and
        /// then show the preview control.
        /// </summary>
        private void SetupPreview()
        {
            if (ResizeDataInternal.ShowsPreview)
            {
                if (_previewLayer == null)
                {
                    CreatePreviewLayer(ResizeDataInternal.Grid);
                }

                ResizeDataInternal.PreviewControl = new PreviewControl();
                ResizeDataInternal.PreviewControl.Bind(this);
                _previewLayer.Children.Add(ResizeDataInternal.PreviewControl);
                double[] changeRange = GetDeltaConstraints();
                Debug.Assert(changeRange.Length == 2, "The changeRange should have two elements!");
                ResizeDataInternal.MinChange = changeRange[0];
                ResizeDataInternal.MaxChange = changeRange[1];
            }
        }

        /// <summary>
        /// Remove the preview control from the preview layer if it exists.
        /// </summary>
        private void RemovePreviewControl()
        {
            if ((ResizeDataInternal.PreviewControl != null) && (_previewLayer != null))
            {
                Debug.Assert(_previewLayer.Children.Contains(ResizeDataInternal.PreviewControl), "The preview layer should contain the PreviewControl!");
                _previewLayer.Children.Remove(ResizeDataInternal.PreviewControl);
            }
        }

        /// <summary>
        /// Initialize the resizeData object to hold the information for the
        /// resize operation in progress.
        /// </summary>
        /// <param name="showsPreview">
        /// Whether or not the preview should be shown.
        /// </param>
        private void InitializeData(bool showsPreview)
        {
            Grid parent = Parent as Grid;
            if (parent != null)
            {
                ResizeDataInternal = new ResizeData();
                ResizeDataInternal.Grid = parent;
                ResizeDataInternal.ShowsPreview = showsPreview;
                ResizeDataInternal.ResizeDirection = GetEffectiveResizeDirection();
                ResizeDataInternal.ResizeBehavior = GetEffectiveResizeBehavior(ResizeDataInternal.ResizeDirection);
                ResizeDataInternal.SplitterLength = Math.Min(ActualWidth, ActualHeight);
                if (!SetupDefinitionsToResize())
                {
                    ResizeDataInternal = null;
                }
                else
                {
                    SetupPreview();
                }
            }
        }

        /// <summary>
        /// Move the splitter and resize the affected columns or rows.
        /// </summary>
        /// <param name="horizontalChange">
        /// Amount to resize horizontally.
        /// </param>
        /// <param name="verticalChange">
        /// Amount to resize vertically.
        /// </param>
        /// <remarks>
        /// Only one of horizontalChange or verticalChange will be non-zero.
        /// </remarks>
        private void MoveSplitter(double horizontalChange, double verticalChange)
        {
            if (EnableCollapseButton)
            {
                ElementBeforeVisibility = (ElementBeforeVisibility == Visibility.Collapsed) ? Visibility.Visible : ElementBeforeVisibility;
                ElementAfterVisibility = (ElementAfterVisibility==Visibility.Collapsed) ? Visibility.Visible : ElementAfterVisibility;
            }
            double resizeChange = (ResizeDataInternal.ResizeDirection == GridResizeDirection.Columns) ? horizontalChange : verticalChange;
            DefinitionAbstraction definition1 = ResizeDataInternal.Definition1;
            DefinitionAbstraction definition2 = ResizeDataInternal.Definition2;
            if ((definition1 != null) && (definition2 != null))
            {
                double definition1ActualLength = GetActualLength(definition1);
                double definition2ActualLength = GetActualLength(definition2);
                ElementBeforeVisibility = definition1ActualLength == 0 ? Windows.UI.Xaml.Visibility.Collapsed : Windows.UI.Xaml.Visibility.Visible;
                ElementAfterVisibility = definition2ActualLength == 0 ? Windows.UI.Xaml.Visibility.Collapsed : Windows.UI.Xaml.Visibility.Visible;
                if ((ResizeDataInternal.SplitBehavior == SplitBehavior.Split) && !DoubleUtil.AreClose((double)(definition1ActualLength + definition2ActualLength), (double)(ResizeDataInternal.OriginalDefinition1ActualLength + ResizeDataInternal.OriginalDefinition2ActualLength)))
                {
                    this.CancelResize();
                }
                else
                {
                    double[] changeRange = GetDeltaConstraints();
                    Debug.Assert(changeRange.Length == 2, "The changeRange should contain two elements!");
                    double minDelta = changeRange[0];
                    double maxDelta = changeRange[1];

                    resizeChange = Math.Min(Math.Max(resizeChange, minDelta), maxDelta);
                    if (definition2ActualLength - resizeChange >= definition2.MinSize && definition1ActualLength + resizeChange >=definition1.MinSize)
                    {
                        double newDefinition1Length = definition1ActualLength + resizeChange;
                        double newDefinition2Length = definition2ActualLength - resizeChange;
                        SetLengths(newDefinition1Length, newDefinition2Length);
                        Definition1GridLength = ResizeDataInternal.OriginalDefinition1Length;
                        Definition2GridLength = ResizeDataInternal.OriginalDefinition2Length;
                    }
                }
            }
        }

        /// <summary>
        /// Determine which adjacent column or row definitions need to be
        /// included in the resize operation and set up resizeData accordingly.
        /// </summary>
        /// <returns>True if it is a valid resize operation.</returns>
        private bool SetupDefinitionsToResize()
        {
            int spanAmount = (int)GetValue((ResizeDataInternal.ResizeDirection == GridResizeDirection.Columns) ? Grid.ColumnSpanProperty : Grid.RowSpanProperty);
            if (spanAmount == 1)
            {
                int definition1Index;
                int definition2Index;
                int splitterIndex = (int)GetValue((ResizeDataInternal.ResizeDirection == GridResizeDirection.Columns) ? Grid.ColumnProperty : Grid.RowProperty);
                switch (ResizeDataInternal.ResizeBehavior)
                {
                    case GridResizeBehavior.CurrentAndNext:
                        definition1Index = splitterIndex;
                        definition2Index = splitterIndex + 1;
                        break;

                    case GridResizeBehavior.PreviousAndCurrent:
                        definition1Index = splitterIndex - 1;
                        definition2Index = splitterIndex;
                        break;

                    default:
                        definition1Index = splitterIndex - 1;
                        definition2Index = splitterIndex + 1;
                        break;
                }
                int definitionCount = (ResizeDataInternal.ResizeDirection == GridResizeDirection.Columns) ? ResizeDataInternal.Grid.ColumnDefinitions.Count : ResizeDataInternal.Grid.RowDefinitions.Count;
                if ((definition1Index >= 0) && (definition2Index < definitionCount))
                {
                    ResizeDataInternal.SplitterIndex = splitterIndex;
                    ResizeDataInternal.Definition1Index = definition1Index;
                    ResizeDataInternal.Definition1 = GetGridDefinition(ResizeDataInternal.Grid, definition1Index, ResizeDataInternal.ResizeDirection);
                    ResizeDataInternal.OriginalDefinition1Length = ResizeDataInternal.Definition1.Size;
                    ResizeDataInternal.OriginalDefinition1ActualLength = GetActualLength(ResizeDataInternal.Definition1);
                    ResizeDataInternal.Definition2Index = definition2Index;
                    ResizeDataInternal.Definition2 = GetGridDefinition(ResizeDataInternal.Grid, definition2Index, ResizeDataInternal.ResizeDirection);
                    ResizeDataInternal.OriginalDefinition2Length = ResizeDataInternal.Definition2.Size;
                    ResizeDataInternal.OriginalDefinition2ActualLength = GetActualLength(ResizeDataInternal.Definition2);
                    bool isDefinition1Star = IsStar(ResizeDataInternal.Definition1);
                    bool isDefinition2Star = IsStar(ResizeDataInternal.Definition2);
                    if (isDefinition1Star && isDefinition2Star)
                    {
                        ResizeDataInternal.SplitBehavior = SplitBehavior.Split;
                    }
                    else
                    {
                        ResizeDataInternal.SplitBehavior = !isDefinition1Star ? SplitBehavior.ResizeDefinition1 : SplitBehavior.ResizeDefinition2;
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Cancel the resize operation in progress.
        /// </summary>
        private void CancelResize()
        {
            if (ResizeDataInternal.ShowsPreview)
            {
                RemovePreviewControl();
            }
            else
            {
                SetLengths(ResizeDataInternal.OriginalDefinition1ActualLength, ResizeDataInternal.OriginalDefinition2ActualLength);
            }
            ResizeDataInternal = null;
        }

        /// <summary>
        /// Get the actual length of the given definition.
        /// </summary>
        /// <param name="definition">
        /// Row or column definition to get the actual length for.
        /// </param>
        /// <returns>
        /// Height of a row definition or width of a column definition.
        /// </returns>
        private static double GetActualLength(DefinitionAbstraction definition)
        {
            if (definition.AsColumnDefinition != null)
            {
                return definition.AsColumnDefinition.ActualWidth;
            }
            return definition.AsRowDefinition.ActualHeight;
        }

        /// <summary>
        /// Determine the max and min that the two definitions can be resized.
        /// </summary>
        /// <returns>Inherited code: Requires comment.</returns>
        private double[] GetDeltaConstraints()
        {
            double definition1ActualLength = GetActualLength(ResizeDataInternal.Definition1);
            double definition1MinSize = ResizeDataInternal.Definition1.MinSize;
            double definition1MaxSize = ResizeDataInternal.Definition1.MaxSize;
            double definition2ActualLength = GetActualLength(ResizeDataInternal.Definition2);
            double definition2MinSize = ResizeDataInternal.Definition2.MinSize;
            double definition2MaxSize = ResizeDataInternal.Definition2.MaxSize;
            double minDelta, maxDelta;

            // Can't resize smaller than the size of the splitter control itself
            if (ResizeDataInternal.SplitterIndex == ResizeDataInternal.Definition1Index)
            {
                definition1MinSize = Math.Max(definition1MinSize, ResizeDataInternal.SplitterLength);
            }
            else if (ResizeDataInternal.SplitterIndex == ResizeDataInternal.Definition2Index)
            {
                definition2MinSize = Math.Max(definition2MinSize, ResizeDataInternal.SplitterLength);
            }

            if (ResizeDataInternal.SplitBehavior == SplitBehavior.Split)
            {
                minDelta = -Math.Min((double)(definition1ActualLength - definition1MinSize), (double)(definition2MaxSize - definition2ActualLength));
                maxDelta = Math.Min((double)(definition1MaxSize - definition1ActualLength), (double)(definition2ActualLength - definition2MinSize));
            }
            else if (ResizeDataInternal.SplitBehavior == SplitBehavior.ResizeDefinition1)
            {
                minDelta = definition1MinSize - definition1ActualLength;
                maxDelta = definition1MaxSize - definition1ActualLength;
            }
            else
            {
                minDelta = definition2ActualLength - definition2MaxSize;
                maxDelta = definition2ActualLength - definition2MinSize;
            }

            return new double[] { minDelta, maxDelta };
        }

        /// <summary>
        /// Determine the resize behavior based on the given direction and
        /// alignment.
        /// </summary>
        /// <param name="direction">Inherited code: Requires comment.</param>
        /// <returns>Inherited code: Requires comment 1.</returns>
        private GridResizeBehavior GetEffectiveResizeBehavior(GridResizeDirection direction)
        {
            if (direction != GridResizeDirection.Columns)
            {
                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Top:
                        return GridResizeBehavior.PreviousAndCurrent;

                    case VerticalAlignment.Bottom:
                        return GridResizeBehavior.CurrentAndNext;
                }
                return GridResizeBehavior.PreviousAndNext;
            }
            else
            {
                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        return GridResizeBehavior.PreviousAndCurrent;

                    case HorizontalAlignment.Right:
                        return GridResizeBehavior.CurrentAndNext;
                }
                return GridResizeBehavior.PreviousAndNext;
            }
        }

        /// <summary>
        /// Determine the resize direction based on the horizontal and vertical
        /// alignments.
        /// </summary>
        /// <returns>Inherited code: Requires comment.</returns>
        private GridResizeDirection GetEffectiveResizeDirection()
        {
            if (HorizontalAlignment != HorizontalAlignment.Stretch)
            {
                return GridResizeDirection.Columns;
            }
            if ((VerticalAlignment == VerticalAlignment.Stretch) && (ActualWidth <= ActualHeight))
            {
                return GridResizeDirection.Columns;
            }
            return GridResizeDirection.Rows;
        }

        /// <summary>
        /// Create a DefinitionAbstraction instance for the given row or column
        /// index in the grid.
        /// </summary>
        /// <param name="grid">Inherited code: Requires comment.</param>
        /// <param name="index">Inherited code: Requires comment 1.</param>
        /// <param name="direction">Inherited code: Requires comment 2.</param>
        /// <returns>Inherited code: Requires comment 3.</returns>
        private static DefinitionAbstraction GetGridDefinition(Grid grid, int index, GridResizeDirection direction)
        {
            if (direction != GridResizeDirection.Columns)
            {
                return new DefinitionAbstraction(grid.RowDefinitions[index]);
            }
            return new DefinitionAbstraction(grid.ColumnDefinitions[index]);
        }

        /// <summary>
        /// Flips a given length if FlowDirection is set to RightToLeft.  This is used for 
        /// keyboard handling.
        /// </summary>
        /// <param name="value">Value to flip for RightToLeft.</param>
        /// <returns>Value if FlowDirection is set to RightToLeft; otherwise, value.</returns>
        private double FlipForRTL(double value)
        {
            return (this.FlowDirection == FlowDirection.RightToLeft) ? -value : value;
        }

        /// <summary>
        /// Set the lengths of the two definitions depending on the split
        /// behavior.
        /// </summary>
        /// <param name="definition1Pixels">
        /// Inherited code: Requires comment.
        /// </param>
        /// <param name="definition2Pixels">
        /// Inherited code: Requires comment 1.
        /// </param>
        private void SetLengths(double definition1Pixels, double definition2Pixels)
        {
            if (ResizeDataInternal.SplitBehavior == SplitBehavior.Split)
            {
                IEnumerable enumerable = (ResizeDataInternal.ResizeDirection == GridResizeDirection.Columns) ? ((IEnumerable)ResizeDataInternal.Grid.ColumnDefinitions) : ((IEnumerable)ResizeDataInternal.Grid.RowDefinitions);
                int definitionIndex = 0;
                DefinitionAbstraction definitionAbstraction;
                foreach (DependencyObject definition in enumerable)
                {
                    definitionAbstraction = new DefinitionAbstraction(definition);
                    if (definitionIndex == ResizeDataInternal.Definition1Index)
                    {
                        SetDefinitionLength(definitionAbstraction, new GridLength(definition1Pixels, GridUnitType.Star));
                    }
                    else if (definitionIndex == ResizeDataInternal.Definition2Index)
                    {
                        SetDefinitionLength(definitionAbstraction, new GridLength(definition2Pixels, GridUnitType.Star));
                    }
                    else if (IsStar(definitionAbstraction))
                    {
                        SetDefinitionLength(definitionAbstraction, new GridLength(GetActualLength(definitionAbstraction), GridUnitType.Star));
                    }
                    definitionIndex++;
                }
            }
            else if (ResizeDataInternal.SplitBehavior == SplitBehavior.ResizeDefinition1)
            {
                SetDefinitionLength(ResizeDataInternal.Definition1, new GridLength(definition1Pixels));
            }
            else
            {
                SetDefinitionLength(ResizeDataInternal.Definition2, new GridLength(definition2Pixels));
            }
        }

        /// <summary>
        /// Set the height/width of the given row/column.
        /// </summary>
        /// <param name="definition">Inherited code: Requires comment.</param>
        /// <param name="length">Inherited code: Requires comment 1.</param>
        private static void SetDefinitionLength(DefinitionAbstraction definition, GridLength length)
        {
            if (definition.AsColumnDefinition != null)
            {
                definition.AsColumnDefinition.SetValue(ColumnDefinition.WidthProperty, length);
            }
            else
            {
                definition.AsRowDefinition.SetValue(RowDefinition.HeightProperty, length);
            }
        }

        /// <summary>
        /// Determine if the given definition has its size set to the "*" value.
        /// </summary>
        /// <param name="definition">Inherited code: Requires comment.</param>
        /// <returns>Inherited code: Requires comment 1.</returns>
        private static bool IsStar(DefinitionAbstraction definition)
        {
            if (definition.AsColumnDefinition != null)
            {
                return definition.AsColumnDefinition.Width.IsStar;
            }
            return definition.AsRowDefinition.Height.IsStar;
        }

        /// <summary>
        /// This code will run whenever the effective resize direction changes,
        /// to update the template being used to display this control.
        /// </summary>
        private void UpdateTemplateOrientation()
        {
            GridResizeDirection newGridResizeDirection = GetEffectiveResizeDirection();

            if (_currentGridResizeDirection != newGridResizeDirection)
            {
                if (newGridResizeDirection == GridResizeDirection.Columns)
                {
                    if (ElementHorizontalTemplateFrameworkElement != null)
                    {
                        ElementHorizontalTemplateFrameworkElement.Visibility = Visibility.Collapsed;
                    }
                    if (ElementVerticalTemplateFrameworkElement != null)
                    {
                        ElementVerticalTemplateFrameworkElement.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (ElementHorizontalTemplateFrameworkElement != null)
                    {
                        ElementHorizontalTemplateFrameworkElement.Visibility = Visibility.Visible;
                    }
                    if (ElementVerticalTemplateFrameworkElement != null)
                    {
                        ElementVerticalTemplateFrameworkElement.Visibility = Visibility.Collapsed;
                    }
                }
                _currentGridResizeDirection = newGridResizeDirection;
            }
        }
    }
}