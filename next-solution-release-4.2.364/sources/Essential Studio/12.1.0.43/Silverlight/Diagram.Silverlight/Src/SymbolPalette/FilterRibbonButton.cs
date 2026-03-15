// <copyright file="FilterRibbonButton.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;


namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents the button used for the symbol Palette.
    /// </summary>
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
    #endif
    public class FilterRibbonButton : ButtonBase
    {
        private static FilterRibbonButton prevSelected = null;
         public static readonly DependencyProperty FilterParentProperty =
        DependencyProperty.Register("FilterParent", typeof(SymbolPalette), typeof(FilterRibbonButton), new PropertyMetadata(null));

       

        #region	Initialization

         internal SymbolPalette FilterParent
        {
            get
            {
                return (SymbolPalette)GetValue(FilterParentProperty);
            }

            set
            {
                SetValue(FilterParentProperty, value);
            }
        }

        /// <summary>
         /// Initializes static members of the <see cref="FilterRibbonButton"/> class.
        /// </summary>
         static FilterRibbonButton()
        {
#if WPF
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FilterRibbonButton), new FrameworkPropertyMetadata(typeof(FilterRibbonButton)));
#endif
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterRibbonButton"/> class.
        /// </summary>
        public FilterRibbonButton()
        {
#if SILVERLIGHT
            DefaultStyleKey = typeof(FilterRibbonButton);
#endif
            this.Loaded += new RoutedEventHandler(FilterRibbonButton_Loaded);

        }


        void FilterRibbonButton_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement ele = (FrameworkElement)VisualTreeHelper.GetParent(this);
            UIElement ele1;
            do
            {
                if ((FrameworkElement)VisualTreeHelper.GetParent(ele) != null)
                    ele = (FrameworkElement)VisualTreeHelper.GetParent(ele);              
                else
                {
                    do
                    {
                        ele1 = (Popup)ele.Parent;
                        ele = (FrameworkElement)VisualTreeHelper.GetParent(ele1);
                    } while (ele == null);
                }
            }
            while (ele.GetType() != typeof(SymbolPalette));
            this.FilterParent = ele as SymbolPalette;
            if (this.IsSelected)
                prevSelected = this;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value that represents the degree to which the corners of a <see cref="FilterRibbonButton"/> are rounded.
        /// </summary>
        /// <value>
        /// Type: <see cref="CornerRadius"/>
        /// The CornerRadius that describes the degree to which corners are rounded. This property has no default value.
        /// </value>
        /// <remarks>
        /// Although the property name suggests that you can use only singular values, CornerRadius also supports non-uniform radii. Radius values that are too large are scaled so that they blend smoothly from corner to corner.
        /// </remarks>
        /// <example>
        /// <code>
        /// FilterRibbonButton button = new FilterRibbonButton();
        /// button.Label = "Button";        
        /// button.CornerRadius = new CornerRadius(3);
        /// </code>
        /// </example>
        /// <seealso cref="FilterRibbonButton"/>
        /// <seealso cref="CornerRadius"/>
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }

            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

     

     

        /// <summary>
        /// Gets or sets the text that labels the <see cref="FilterRibbonButton"/>.
        /// </summary>
        /// <value>
        /// Type: <see cref="String"/>
        /// Text that labels the <see cref="FilterRibbonButton"/>. The default is empty string.
        /// </value>
        /// <example>
        /// <code>
        /// FilterRibbonButton button = new FilterRibbonButton();
        /// button.Label = "Button";        
        /// button.CornerRadius = new CornerRadius(3);
        /// </code>
        /// </example>
        /// <seealso cref="FilterRibbonButton"/>
        /// <seealso cref="string"/>
        public string Label
        {
            get
            {
                return (string)GetValue(LabelProperty);
            }

            set
            {
                SetValue(LabelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="FilterRibbonButton"/> is selected.   
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// true if the <see cref="FilterRibbonButton"/> is selected; false if the <see cref="FilterRibbonButton"/> is not selected.  The default is false.
        /// </value>
        /// <example>
        /// <code>
        /// FilterRibbonButton button = new FilterRibbonButton();
        /// button.Label = "Button";  
        /// button.IsToggle = true;
        /// button.IsSelected = true;
        /// </code>
        /// </example>
        /// <seealso cref="FilterRibbonButton"/>
        /// <seealso cref="Boolean"/>
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }

            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="FilterRibbonButton"/> is toggle button.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// true if the <see cref="FilterRibbonButton"/> is toggle; false if the <see cref="FilterRibbonButton"/> is not toggle.  The default is false.
        /// </value>
        /// <example>
        /// <code>
        /// FilterRibbonButton button = new FilterRibbonButton();
        /// button.Label = "Button";  
        /// button.IsToggle = true;
        /// button.IsSelected = true;
        /// </code>
        /// </example>
        /// <seealso cref="FilterRibbonButton"/>
        /// <seealso cref="Boolean"/>
        public bool IsToggle
        {
            get
            {
                return (bool)GetValue(IsToggleProperty);
            }

            set
            {
                SetValue(IsToggleProperty, value);
            }
        }

        #endregion

        #region Events

        ///// <summary>
        ///// Event that is raised when SmallIcon property is changed.
        ///// </summary>
        //public event PropertyChangedCallback SmallIconChanged;

        ///// <summary>
        ///// Event that is raised when LargeIcon property is changed.
        ///// </summary>
        //public event PropertyChangedCallback LargeIconChanged;

        /// <summary>
        /// Event that is raised when Label property is changed.
        /// </summary>
        public event PropertyChangedCallback LabelChanged;

        /// <summary>
        /// Event that is raised when SizeForm property is changed.
        /// </summary>
        public event PropertyChangedCallback SizeFormChanged;

        /// <summary>
        /// Event that is raised when IsSelected property is changed.
        /// </summary>
        public event PropertyChangedCallback IsSelectedChanged;

        /// <summary>
        /// Event that is raised when IsToggle property is changed.
        /// </summary>
        public event PropertyChangedCallback IsToggleChanged;

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Defines corner radius of button. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty;
       
        /// <summary>
        /// Defines button label. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(FilterRibbonButton), new PropertyMetadata(null, new PropertyChangedCallback(OnLabelChanged)));

        /// <summary>
        /// Defines when button is selected. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(FilterRibbonButton), new PropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));

        /// <summary>
        /// Defines when button is toggle. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IsToggleProperty =
            DependencyProperty.Register("IsToggle", typeof(bool), typeof(FilterRibbonButton), new PropertyMetadata(false, new PropertyChangedCallback(OnIsToggleChanged)));

        #endregion

        #region	Implementation

       

        /// <summary>
        /// Calls OnLabelChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FilterRibbonButton instance = (FilterRibbonButton)d;
            instance.OnLabelChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises LabelChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnLabelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LabelChanged != null)
            {
                LabelChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnSizeFormChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnSizeFormChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FilterRibbonButton instance = (FilterRibbonButton)d;
            instance.OnSizeFormChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises SizeFormChanged
        /// event.
        /// </summary>
        /// <param name="e">Property change details, such as old value
        /// and new value.</param>
        protected virtual void OnSizeFormChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SizeFormChanged != null)
            {
                SizeFormChanged(this, e);
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.IsSelected = !this.IsSelected;
            if (prevSelected != null && prevSelected != this)
            {
                prevSelected.IsSelected = false;
                prevSelected.border.Background = FilterParent.PopUpBackground;
            }
            prevSelected = this;
            FrameworkElement ele=(FrameworkElement)VisualTreeHelper.GetParent(this);
            do
            {
                ele = (FrameworkElement)VisualTreeHelper.GetParent(ele);
            }
            while (ele.GetType() != typeof(ButtonChecker));

            ButtonChecker buttonchecker = ele as ButtonChecker;
            buttonchecker.CheckedButton = this;
            
           (((ele.Parent) as FrameworkElement).Parent as Popup).IsOpen=false;

        }

        /// <summary>
        /// Calls OnIsSelectedChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FilterRibbonButton instance = (FilterRibbonButton)d;
            instance.OnIsSelectedChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises IsSelectedChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsSelectedChanged != null)
            {
                IsSelectedChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIsToggleChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value
        /// and new value.</param>
        private static void OnIsToggleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FilterRibbonButton instance = (FilterRibbonButton)d;
            instance.OnIsToggleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises IsToggleChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsToggleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsToggleChanged != null)
            {
                IsToggleChanged(this, e);
            }
        }
        #endregion

        #region Overrides

        private Border border;
        
#if SILVERLIGHT
        private Border leftColumnBorder;
#endif
#if WPF
        private Border leftColumnBorder;
#endif

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.border = this.GetTemplateChild("PART_Button") as Border;
#if SILVERLIGHT
            this.leftColumnBorder = this.GetTemplateChild("PART_PopUpLeftColumnBorder") as Border;
#endif
#if WPF
            this.leftColumnBorder = this.GetTemplateChild("PART_PopupLeftColumnBorder") as Border;
#endif
            this.border.MouseEnter += new MouseEventHandler(border_MouseEnter);
            this.border.MouseLeave += new MouseEventHandler(border_MouseLeave);
        }

        void border_MouseLeave(object sender, MouseEventArgs e)
        {
            this.border.Background = new SolidColorBrush(Colors.Transparent);
            this.border.BorderBrush = FilterParent.PopUpBorderBrush;
#if SILVERLIGHT
            this.leftColumnBorder.Background = FilterParent.PopUpLeftColumnBackground;            
#endif    
#if WPF
            this.border.Background = new SolidColorBrush(Colors.Transparent);
            this.border.BorderThickness = new Thickness(0);
            this.leftColumnBorder.Background = FilterParent.PopUpLeftColumnBackground;
            this.leftColumnBorder.BorderThickness = new Thickness(0);
#endif
        }

        void border_MouseEnter(object sender, MouseEventArgs e)
        {

            this.border.Background = FilterParent.PopUpItemMouseOverBrush;
            this.border.BorderBrush = FilterParent.PopUpItemMouseOverBorderBrush;
#if SILVERLIGHT
            this.leftColumnBorder.Background = FilterParent.PopUpItemMouseOverBrush;
            this.leftColumnBorder.BorderBrush = FilterParent.PopUpItemMouseOverBorderBrush;
#endif
            
#if WPF
            this.border.BorderThickness = new Thickness(0.5);
            this.leftColumnBorder.Background = FilterParent.PopUpItemMouseOverBrush;
            this.leftColumnBorder.BorderBrush = FilterParent.PopUpItemMouseOverBorderBrush;
            this.leftColumnBorder.BorderThickness = new Thickness(0, 0.5, 0, 0.5);
#endif
        }

       

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Primitives.ButtonBase.Click"/> routed event.
        /// </summary>
        protected override void OnClick()
        {
            if (IsToggle)
            {
                IsSelected = !IsSelected;
            }

            base.OnClick();
        }

        #endregion
    }
}

