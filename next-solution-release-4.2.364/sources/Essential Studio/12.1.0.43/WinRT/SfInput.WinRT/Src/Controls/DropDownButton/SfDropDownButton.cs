// <copyright file="SfDropDownButton.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Syncfusion.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents a control that provides a list of items when pressed
    /// </summary>
    public class SfDropDownButton : ContentControl
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDropDownButton"/> class.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SfDropDownButton()
        {
            DefaultStyleKey = typeof (SfDropDownButton);
            Loaded += SfDropDownButton_Loaded;
        }

        #endregion

        #region members

        private Border _partdropdown;

        private Popup _partpopup;

        #endregion

        #region Dependency Property

        /// <summary>
        /// Gets or sets the content of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDropDownButton"/> control.
        /// </summary>
        public object DropDownContent
        {
            get { return (object)GetValue(DropDownContentProperty); }
            set { SetValue(DropDownContentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DropDownContent.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DropDownContentProperty =
            DependencyProperty.Register("DropDownContent", typeof(object), typeof(SfDropDownButton), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the template for the content of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDropDownButton"/> control.
        /// </summary>
        public DataTemplate DropDownContentTemplate
        {
            get { return (DataTemplate)GetValue(DropDownContentTemplateProperty); }
            set { SetValue(DropDownContentTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DropDownContentTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DropDownContentTemplateProperty =
            DependencyProperty.Register("DropDownContentTemplate", typeof(DataTemplate), typeof(SfDropDownButton), new PropertyMetadata(null));

        /// <summary>
        /// Returns a value when set
        /// </summary>
        /// <value>
        /// <c>true</c> if instance is created ; otherwise, <c>false</c>.
        /// </value>
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsDropDownOpen.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(SfDropDownButton), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the height of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDropDownButton"/> control.
        /// </summary>
        public double DropDownHeight
        {
            get { return (double)GetValue(DropDownHeightProperty); }
            set { SetValue(DropDownHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DropDownHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DropDownHeightProperty =
            DependencyProperty.Register("DropDownHeight", typeof(double), typeof(SfDropDownButton), new PropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets the accent brush
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
            DependencyProperty.Register("AccentBrush", typeof(Brush), typeof(SfDropDownButton), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the direction of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDropDownButton"/> control.
        /// </summary>
        public DropDownDirection DropDownDirection
        {
            get { return (DropDownDirection)GetValue(DropDownDirectionProperty); }
            set { SetValue(DropDownDirectionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DropDownDirection.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DropDownDirectionProperty =
            DependencyProperty.Register("DropDownDirection", typeof(DropDownDirection), typeof(SfDropDownButton), new PropertyMetadata(DropDownDirection.Bottom,OnDropDownDirectionChanged));
       
        #endregion

        #region Override Methods
        /// <summary>
        /// Initializes all the child elements of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SfDropDownButton"/> control.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            _partdropdown = GetTemplateChild("PART_DropDown") as Border;
            _partpopup = GetTemplateChild("PART_DropDownPopup") as Popup;
            base.OnApplyTemplate();
        }

        #endregion

        #region Helper Methods
        
        void SfDropDownButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (_partdropdown != null)
                _partdropdown.MinWidth = this.ActualWidth;
            ValidatePopupPosition();
        }

        private static void OnDropDownDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dropdownButton = d as SfDropDownButton;
            dropdownButton.ValidatePopupPosition();
        }   

        private void ValidatePopupPosition()
        {
            if (_partpopup != null)
            {
                _partpopup.HorizontalOffset = 0;
                if (DropDownDirection.Equals(DropDownDirection.Top))
                {
                    _partpopup.VerticalOffset = -ActualHeight - DropDownHeight -
                                                (_partdropdown.Margin.Bottom + _partdropdown.Margin.Top);
                }
                else
                {
                    _partpopup.VerticalOffset = 0;
                }
            }
        }
        #endregion
    }
}
