// <copyright file="DomainUpDown.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
#if WPF
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using Syncfusion.Windows.Primitives;
using Syncfusion.Windows.Utils;
using Syncfusion.Licensing;
using System.Windows.Media.Animation;

namespace Syncfusion.Windows.Controls.Input
#elif WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using Syncfusion.WP.Utils;
using System.Windows;
using System.Windows.Media;
using Syncfusion.WP.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Syncfusion.WP.Controls.Input
#elif SILVERLIGHT
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
    
using Syncfusion.Tools.Primitives;
using Syncfusion.Tools.Utils;
using System.Windows.Media.Animation;

namespace Syncfusion.Tools.Controls.Input

#else
using Syncfusion.UI.Xaml.Primitives;
using Syncfusion.UI.Xaml.Utils;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using System.Windows.Input;
using Windows.Foundation;

namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.DomainUpDown"/> control is a pair
    /// of spin buttons by clicking the up and down buttons the user can scroll and
    /// select the data.
    /// </summary>
    /// <remarks>
    /// DomainUpDown is a control
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class SfDomainUpDown : Control
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DomainUpDown"/> class.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SfDomainUpDown()
        {
#if WPF
            if (EnvironmentTestInput.IsSecurityGranted)
            {
                EnvironmentTestInput.StartValidateLicense(typeof(SfDomainUpDown));
            }
#endif
            DefaultStyleKey = typeof(SfDomainUpDown);
            this.Loaded += DomainUpDown_Loaded;
        }

        #endregion

        #region Variables

        private TransitionContentControl PART_Content;

        private Grid InnerGrid;

        private ICommand incrementCommand;

        private ICommand decrementCommand;

        private ObservableCollection<object> internalItems = new ObservableCollection<object>();

        /// <summary>
        /// Gets or sets the InternalValue
        /// </summary>
        protected object InternalValue
        {
            get;
            set;
        }

        #endregion

        #region Dependency Properties

#if !(WINDOWS_PHONE||WINDOWS_PHONE_7 || SILVERLIGHT || WPF)
        /// <summary>
        /// Gets or sets the collection of transitions that apply to the content area of
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.DomainUpDown"/>.
        /// </summary>
        /// <value>
        /// <see cref="N:Windows.UI.Xaml.Media.Animation.">Transition</see>.
        /// </value>
        /// <seealso cref="N:Windows.UI.Xaml.Media.Animation.TransitionCollection"/>
        [ClassReference(IsReviewed = false)]
        public TransitionCollection ContentTransition
        {
            get { return (TransitionCollection)GetValue(ContentTransitionProperty); }
            set { SetValue(ContentTransitionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ContentTransitionProperty =
            DependencyProperty.Register("ContentTransition", typeof(TransitionCollection), typeof(SfDomainUpDown), new PropertyMetadata(null));
#endif
        /// <summary>
        /// Gets or sets the current value for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DomainUpDown"/>.
        /// </summary>
        /// <remarks>
        /// The Default value is null.
        /// </remarks>
        /// <value>
        /// The value.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(SfDomainUpDown), new PropertyMetadata(null, new PropertyChangedCallback(OnValueChanged)));


        /// <summary>
        /// Gets or sets a value indicating whether the values can be auto reversed.
        /// </summary>
        /// <value>
        /// <c>true</c> if items can be return to initial, once reached maximum; otherwise, <c>false</c>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public bool AutoReverse
        {
            get { return (bool)GetValue(AutoReverseProperty); }
            set { SetValue(AutoReverseProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AutoReverseProperty =
            DependencyProperty.Register("AutoReverse", typeof(bool), typeof(SfDomainUpDown), new PropertyMetadata(true));


        /// <summary>
        /// Gets or sets a value indicating whether <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DomainUpDown"/> can animate on
        /// increment/decrement items.
        /// </summary>
        /// <value>
        /// <c>true</c> if animation be run on increment/decrement items; otherwise, <c>false</c>.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.DomainUpDown.SpinButtonsAlignment"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.DomainUpDown.UpDownStyle"/>
        [ClassReference(IsReviewed = false)]
        public bool EnableSpinAnimation
        {
            get { return (bool)GetValue(EnableSpinAnimationProperty); }
            set { SetValue(EnableSpinAnimationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableSpinAnimation.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EnableSpinAnimationProperty = 
            DependencyProperty.Register("EnableSpinAnimation", typeof(bool), typeof(SfDomainUpDown), new PropertyMetadata(true));



        /// <summary>
        /// Gets or sets the spin buttons alignment.
        /// </summary>
        /// <value>
        /// The default value is <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.SpinButtonsAlignment"/>.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.DomainUpDown.EnableSpinAnimation"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.DomainUpDown.UpDownStyle"/>
        [ClassReference(IsReviewed = false)]
#if WPF
        [CLSCompliant(false)]
#endif
        public SpinButtonsAlignment SpinButtonsAlignment
        {
            get { return (SpinButtonsAlignment)GetValue(SpinButtonsAlignmentProperty); }
            set { SetValue(SpinButtonsAlignmentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SpinButtonsAlignment.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SpinButtonsAlignmentProperty =
            DependencyProperty.Register("SpinButtonsAlignment", typeof(SpinButtonsAlignment), typeof(SfDomainUpDown), new PropertyMetadata(SpinButtonsAlignment.Right));



        /// <summary>
        /// Gets or sets the style for updown buttons.
        /// </summary>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.DomainUpDown.EnableSpinAnimation"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.DomainUpDown.SpinButtonsAlignment"/>
        [ClassReference(IsReviewed = false)]
        public Style UpDownStyle
        {
            get { return (Style)GetValue(UpDownStyleProperty); }
            set { SetValue(UpDownStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for UpDownStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty UpDownStyleProperty =
            DependencyProperty.Register("UpDownStyle", typeof(Style), typeof(SfDomainUpDown), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the background for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DomainUpDown"/>.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush AccentBrush
        {
            get { return (Brush)GetValue(AccentBrushProperty); }
            set { SetValue(AccentBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AccentBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AccentBrushProperty =
            DependencyProperty.Register("AccentBrush", typeof(Brush), typeof(SfDomainUpDown), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a collection used to generate the content of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DomainUpDown"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SfDomainUpDown), new PropertyMetadata(null,new PropertyChangedCallback(OnItemsSourceChanged)));


        /// <summary>
        /// Gets or sets the data template used to display the content for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DomainUpDown"/>
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ContentTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(SfDomainUpDown), new PropertyMetadata(null,OnContentTemplateChanged));

        /// <summary>
        /// Gets or sets the zero-based index of the currently selected item in a <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DomainUpDown"/>.
        /// </summary>
        /// <value>
        /// The default value is zero.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public int CurrentIndex
        {
            get { return (int)GetValue(CurrentIndexProperty); }
            set { SetValue(CurrentIndexProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CurrentIndex.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CurrentIndexProperty =
            DependencyProperty.Register("CurrentIndex", typeof(int), typeof(SfDomainUpDown), new PropertyMetadata(-1, new PropertyChangedCallback(OnCurrentIndexChanged)));   
        

        #endregion             

        #region Public Methods

        /// <summary>
        /// Decrements the associated performance counter by one and stores the result, as
        /// an atomic operation.
        /// </summary>
        /// <remarks>
        /// Returns void
        /// </remarks>
        /// <seealso cref="M:Syncfusion.UI.Xaml.Controls.Input.DomainUpDown.Increment"/>
        [ClassReference(IsReviewed = false)]
        public void Decrement()
        {
            if (ItemsSource != null)
            {
                var Items = from object item in ItemsSource
                            select item;

                if (Items.Count() > 0)
                {
                    if (PART_Content != null && PART_Content.Transition is SlideTransition)
                        ((SlideTransition)PART_Content.Transition).Direction = SlideDirection.Down;

                    int index = Items.ToList().IndexOf(InternalValue);
                    if (index > -1 && index > 0)
                    {
                        InternalValue = Items.ToList().ElementAt(index - 1);

                            Value = Items.ToList().ElementAt(index - 1);

                        index = Items.ToList().IndexOf(InternalValue);
                    }
                    else
                    {
                        if (AutoReverse)
                        {
                            InternalValue = Items.ToList().ElementAt(Items.Count() - 1);

                                Value = Items.ToList().ElementAt(Items.Count() - 1);

                            index = Items.ToList().IndexOf(InternalValue);
                        }
                        else
                        {
                            if (index > -1)
                            {
                                InternalValue = Items.ToList().ElementAt(index);

                                if (this.ContentTemplate != null)
                                    Value = Items.ToList().ElementAt(index);

                                index = Items.ToList().IndexOf(InternalValue);

                            }
                            else
                            {
                                InternalValue = Items.ToList().ElementAt(Items.Count() - 1);

                                    Value = Items.ToList().ElementAt(Items.Count() - 1);

                                index = Items.ToList().IndexOf(InternalValue);
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Increments the associated performance counter by one and stores the result, as
        /// an atomic operation.
        /// </summary>
        /// <remarks>
        /// Returns void
        /// </remarks>
        /// <seealso cref="M:Syncfusion.UI.Xaml.Controls.Input.DomainUpDown.Decrement"/>
        [ClassReference(IsReviewed = false)]
        public void Increment()
        {
            if (ItemsSource != null)
            {
                var Items = from object item in ItemsSource
                            select item;

                if (Items.Count() > 0)
                {
                    if (PART_Content != null && PART_Content.Transition is SlideTransition)
                        ((SlideTransition)PART_Content.Transition).Direction = SlideDirection.Up;

                    int index = Items.ToList().IndexOf(InternalValue);
                    if (index > -1 && index < Items.Count() - 1)
                    {
                        InternalValue = Items.ToList().ElementAt(index + 1);

                            Value = Items.ToList().ElementAt(index + 1);

                        index = Items.ToList().IndexOf(InternalValue);
                    }
                    else
                    {
                        if (AutoReverse)
                        {
                            InternalValue = Items.ToList().ElementAt(0);

                                Value = Items.ToList().ElementAt(0);

                            index = Items.ToList().IndexOf(InternalValue);
                        }
                        else
                        {
                            if (index > -1)
                            {
                                InternalValue = Items.ToList().ElementAt(index);

                                    Value = Items.ToList().ElementAt(index);

                                index = Items.ToList().IndexOf(InternalValue);
                            }
                            else
                            {
                                InternalValue = Items.ToList().ElementAt(0);

                                    Value = Items.ToList().ElementAt(0);

                                index = Items.ToList().IndexOf(InternalValue);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Override Methods
        /// <summary>
        /// Initializes all the child elements of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DomainUpDown"/> control.
        /// </summary>
#if WINDOWS_PHONE||WINDOWS_PHONE_7 ||WPFSILVERLIGHT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            PART_Content = GetTemplateChild("PART_Content") as TransitionContentControl;
            InnerGrid = GetTemplateChild("InnerGrid") as Grid;
            if (InnerGrid != null)
            {
#if WINDOWS_PHONE||WINDOWS_PHONE_7 || WPFSILVERLIGHT
                InnerGrid.MouseEnter+=InnerGrid_MouseEnter;
                InnerGrid.MouseLeave+=InnerGrid_MouseLeave;
#else
                InnerGrid.PointerEntered += InnerGrid_PointerEntered;
                InnerGrid.PointerExited += InnerGrid_PointerExited; 
#endif
            }
            base.OnApplyTemplate();
        }
#if WINDOWS_PHONE||WINDOWS_PHONE_7 || WPFSILVERLIGHT
        void InnerGrid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
#else
        void InnerGrid_PointerExited(object sender, PointerRoutedEventArgs e)
#endif
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }
#if WINDOWS_PHONE||WINDOWS_PHONE_7 || WPFSILVERLIGHT
        void InnerGrid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
#else
        void InnerGrid_PointerEntered(object sender, PointerRoutedEventArgs e)
#endif
        {
            VisualStateManager.GoToState(this, "PointerOver", true);
        }

        /// <summary>
        /// Occurs when the pointer is moved
        /// </summary>
        /// <param name="e"></param>
#if WINDOWS_PHONE||WINDOWS_PHONE_7 || WPFSILVERLIGHT
        protected override void OnMouseWheel(System.Windows.Input.MouseWheelEventArgs e)
#else
        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
#endif
        {
#if WINDOWS_PHONE||WINDOWS_PHONE_7 || WPFSILVERLIGHT
             base.OnMouseWheel(e);
            if(e.Delta>0)
#else
            base.OnPointerWheelChanged(e);
            if (e.GetCurrentPoint(this).Properties.MouseWheelDelta > 0)
#endif
            {
                Increment();
            }
            else
            {
                Decrement();
            }
        }

#if WINRT
        /// <summary>
        /// Occurs when the focus is obtained
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (FocusState == FocusState.Keyboard || FocusState == FocusState.Programmatic)
                VisualStateManager.GoToState(this, "Focused", true);
            else if (FocusState == FocusState.Pointer)
                VisualStateManager.GoToState(this, "PointerFocused", true);
            base.OnGotFocus(e);
        }

        /// <summary>
        /// Occurs when the focus is lost
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if(FocusState==FocusState.Unfocused)
                VisualStateManager.GoToState(this, "Unfocused", true);
            base.OnLostFocus(e);
        }
#endif
        #endregion

        #region Callback Methods

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SfDomainUpDown control = (SfDomainUpDown)obj;
            if (control != null)
            {
                control.UpdateValue(args);
                ((SfDomainUpDown)obj).OnValueChanged(args);
            }
        }

        void DomainUpDown_Loaded(object sender, RoutedEventArgs e)
        {
            if (ItemsSource != null && ItemsSource is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)ItemsSource).CollectionChanged += SfDomainUpDown_CollectionChanged;
                this.Unloaded += DomainUpDown_Unloaded;    
            }
            IsEnabledChanged += SfDomainUpDown_IsEnabledChanged;
            Unloaded += SfDomainUpDown_Unloaded;
            if (!IsEnabled)
                VisualStateManager.GoToState(this, "Disabled", true);
        }

        void SfDomainUpDown_Unloaded(object sender, RoutedEventArgs e)
        {
            IsEnabledChanged -= SfDomainUpDown_IsEnabledChanged;
            Unloaded -= SfDomainUpDown_Unloaded;
            Loaded -= DomainUpDown_Loaded;
        }

        void SfDomainUpDown_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsEnabled)
                VisualStateManager.GoToState(this, "Disabled", true);
            else
                VisualStateManager.GoToState(this, "Normal", true);
        }

        void SfDomainUpDown_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var Items = from object item in ItemsSource
                        select item;
            internalItems = new ObservableCollection<object>(this.ItemsSource.Cast<object>());
            if(e.Action.Equals(NotifyCollectionChangedAction.Add))
            {
                if (CurrentIndex < 0 && internalItems.Count > 0)
                {
                    Value = Items.ToList().ElementAt(0);
                }
            }
            else if (e.Action.Equals(NotifyCollectionChangedAction.Remove))
            {                
                if (CurrentIndex >= 0 && internalItems.Count > 0 && internalItems.Count > CurrentIndex &&
                    !internalItems[CurrentIndex].Equals(Value))
                {
                    if (CurrentIndex > (internalItems.Count - 1))
                    {
                        CurrentIndex = 0;
                    }
                    else
                    {
                        Value = Items.ToList().ElementAt(CurrentIndex);
                    }
                    if (Value == null)
                        CurrentIndex = -1;

                }
                else if (internalItems.Count > 0 && internalItems.Count <= CurrentIndex)
                {
                    CurrentIndex = 0;
                    Value = Items.ToList().ElementAt(CurrentIndex);
                }

                else if (internalItems.Count <= 0)
                {
                    CurrentIndex = -1;
                    Value = null;
                }
            }
            else if(e.Action.Equals(NotifyCollectionChangedAction.Reset))
            {
                CurrentIndex = -1;
                Value = null;
            }

        }

        void DomainUpDown_Unloaded(object sender, RoutedEventArgs e)
        {
            ((INotifyCollectionChanged)ItemsSource).CollectionChanged -= SfDomainUpDown_CollectionChanged;
            this.Unloaded -= DomainUpDown_Unloaded;
        }

        private void UpdateValue(DependencyPropertyChangedEventArgs args)
        {
            InternalValue = args.NewValue;
            if (ItemsSource != null)
            {
                var Items = from object item in ItemsSource
                            select item;
                CurrentIndex = Items.ToList().IndexOf(InternalValue);
            }
        }

        /// <summary>
        /// Occurs when the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DomainUpDown"/> control Value has changed.
        /// </summary>
        /// <param name="args"></param>
        protected void OnValueChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.ValueChanged != null)
            {
                ValueChangedEventArgs valueArgs = new ValueChangedEventArgs() { NewValue = args.NewValue, OldValue = args.OldValue };
                this.ValueChanged(this, valueArgs);
            }
        }

        private static void OnItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((SfDomainUpDown)obj != null)
            {
                ((SfDomainUpDown)obj).OnItemsSourceChanged(args);
            }
        }

        /// <summary>
        /// Occurs when the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DomainUpDown"/> control ItemsSource has changed.
        /// </summary>
        /// <param name="args"></param>
        protected void OnItemsSourceChanged(DependencyPropertyChangedEventArgs args)
        {
            if (ItemsSource != null)
            {
                var Items = from object item in ItemsSource
                            select item;

                int index = Items.ToList().IndexOf(Value);
                if (index >= 0)
                {
                    CurrentIndex = index;
                }
                else if (CurrentIndex >= 0 && CurrentIndex < Items.Count())
                {
                    InternalValue = Items.ToList().ElementAt(CurrentIndex);
                    Value = InternalValue;
                }
                else if(Items.Count()!=0) 
                {
                    if (args.OldValue ==null && CurrentIndex >= Items.Count())
                    {
                        throw new ArgumentOutOfRangeException("Invalid index value '" + CurrentIndex + "'");
                    }
                    else
                    {
                        CurrentIndex = 0;
                        if(Value!=Items.ToList().ElementAt(CurrentIndex))
                            Value = Items.ToList().ElementAt(CurrentIndex);
                    }
                }
            }
            else
            {
                Value = null;
                CurrentIndex = -1;
            }
        }

        /// <summary>
        /// Occurs when the current index has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected static void OnCurrentIndexChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = sender as SfDomainUpDown;

            if (control != null && control.ItemsSource != null)
            {
                var Items = from object item in control.ItemsSource
                            select item;
                if ((int)args.NewValue > Items.Count())
                {
                   throw new ArgumentOutOfRangeException("Invalid index value '" + control.CurrentIndex + "'");
                }
                if ((int)args.NewValue >= 0)
                {
                    control.InternalValue = Items.ToList().ElementAt((int)args.NewValue);
                    if(control.Value!=control.InternalValue)
                        control.Value = control.InternalValue;
                }
                else if(Items.Count()!= 0 && control.CurrentIndex >= 0)
                {
                    control.CurrentIndex = 0;
                }
                else if (control.CurrentIndex < 0)
                {
                    control.Value = null;
                }
            }
        }             

        protected static void OnContentTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            SfDomainUpDown domainupdown = (SfDomainUpDown)sender;
            domainupdown.ApplyTemplate();
            if (domainupdown != null && domainupdown.PART_Content!=null && args.NewValue!=null && args.OldValue!=args.NewValue)
            {
                ContentPresenter Part_Content = FindVisualChildByName<DependencyObject>(domainupdown.PART_Content, "PART_Content") as ContentPresenter;
                ContentControl Part_TempContent = FindVisualChildByName<DependencyObject>(domainupdown.PART_Content, "PART_TempContent") as ContentControl;
                Grid LayoutGrid = FindVisualChildByName<DependencyObject>(domainupdown.PART_Content, "PART_LayoutRoot") as Grid;
                if (Part_Content != null && Part_TempContent!=null && LayoutGrid!=null)
                {
                    Part_Content.Measure(new Size(domainupdown.ActualWidth, domainupdown.ActualHeight));
                    Part_TempContent.Opacity = 0;
#if WPF
                    Part_TempContent.RenderTransform = new TranslateTransform();
#else
                    Part_TempContent.RenderTransform = new CompositeTransform();
#endif
                    Timeline exitaction = (domainupdown.PART_Content.Transition as SlideTransition).CreateExitAnimation(domainupdown.ActualHeight);
                    var exit = new Storyboard();
                    exit.Children.Add(exitaction);
                    Storyboard.SetTarget(exitaction, Part_TempContent);
#if WINDOWS_PHONE||WINDOWS_PHONE_7||SILVERLIGHT
                    Storyboard.SetTargetProperty(exitaction, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
#else
#if WPF
                    Storyboard.SetTargetProperty(exitaction, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
#else
                    Storyboard.SetTargetProperty(exitaction, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");
#endif
#endif
                    exit.Begin();
#if WINRT
                    exit.Completed += delegate(object s, object e)
#else
                    exit.Completed += delegate(object s, EventArgs e)
#endif
                    {
                        Part_TempContent.Opacity = 1;
                    };
                    LayoutGrid.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, domainupdown.ActualWidth, domainupdown.ActualHeight) };
                }
            }
        }

        public static T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                string controlName = child.GetValue(Control.NameProperty) as string;
                if (controlName == name)
                {
                    return child as T;
                }
                else
                {
                    T result = FindVisualChildByName<T>(child, name);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }
        #endregion

        #region Commands

        /// <summary>
        /// Gets the next Value
        /// </summary>
        public ICommand IncrementCommand
        {
            get
            {
                if (incrementCommand == null)
                {
                    incrementCommand = new DelegateCommand(param => Increment());
                }
                return incrementCommand;
            }
        }

        /// <summary>
        /// Gets the previous Value
        /// </summary>
        public ICommand DecrementCommand
        {
            get
            {
                if (decrementCommand == null)
                {
                    decrementCommand = new DelegateCommand(param => Decrement());
                }
                return decrementCommand;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when current <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.DomainUpDown.Value"/> changed.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public event ValueChangedEventHandler ValueChanged;

        #endregion

    }
}
