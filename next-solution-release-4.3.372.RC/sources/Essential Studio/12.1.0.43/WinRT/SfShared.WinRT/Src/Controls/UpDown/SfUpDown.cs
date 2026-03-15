#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
#if WPF
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Syncfusion.Licensing;
namespace Syncfusion.Windows.Controls
#elif SILVERLIGHT
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace Syncfusion.Tools.Controls

#elif WINDOWS_PHONE || WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Syncfusion.WP.Utils;
using System.Windows;
using System.Windows.Media;

namespace Syncfusion.WP.Controls
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Syncfusion.UI.Xaml.Utils;
using Windows.UI.Xaml.Media;
namespace Syncfusion.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a control to navigate up and down through the items
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public class SfUpDown:Control
    {
        #region Constructor

        /// <summary>
        /// Initializes an instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.SfUpDown"/> class.
        /// </summary>
        public SfUpDown()
        {
#if WPF
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(SfUpDown));
            }
#endif

            DefaultStyleKey = typeof(SfUpDown);
        }
        #endregion

        #region Variables

#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        internal RepeatButton upButton = null;

        internal RepeatButton downButton = null;
#else 
        internal Button upButton = null;

        internal Button downButton = null;

#endif

        internal Grid outerGrid = null;

        internal ContentControl PART_Content = null;

        #endregion

        #region DependencyProperties

        /// <summary>
        /// Gets or sets the command for up motion
        /// </summary>
        public ICommand UpCommand
        {
            get { return (ICommand)GetValue(UpCommandProperty); }
            set { SetValue(UpCommandProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for UpCommand.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty UpCommandProperty =
            DependencyProperty.Register("UpCommand", typeof(ICommand), typeof(SfUpDown), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the command for down motion
        /// </summary>
        public ICommand DownCommand
        {
            get { return (ICommand)GetValue(DownCommandProperty); }
            set { SetValue(DownCommandProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DownCommand.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DownCommandProperty =
            DependencyProperty.Register("DownCommand", typeof(ICommand), typeof(SfUpDown), new PropertyMetadata(null));




        /// <summary>
        /// Gets or sets the parameter for up command
        /// </summary>
        public object UpCommandParameter
        {
            get { return (object)GetValue(UpCommandParameterProperty); }
            set { SetValue(UpCommandParameterProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for commandParameter.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty UpCommandParameterProperty =
            DependencyProperty.Register("UpCommandParameter", typeof(object), typeof(SfUpDown), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the parameter for down command
        /// </summary>
        public object DownCommandParameter
        {
            get { return (object)GetValue(DownCommandParameterProperty); }
            set { SetValue(DownCommandParameterProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DownCommandParameter.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DownCommandParameterProperty =
            DependencyProperty.Register("DownCommandParameter", typeof(object), typeof(SfUpDown), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the style for up button
        /// </summary>
        public Style UpButtonStyle
        {
            get { return (Style)GetValue(UpButtonStyleProperty); }
            set { SetValue(UpButtonStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for UpButtonStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty UpButtonStyleProperty =
            DependencyProperty.Register("UpButtonStyle", typeof(Style), typeof(SfUpDown), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the style for down button
        /// </summary>
        public Style DownButtonStyle
        {
            get { return (Style)GetValue(DownButtonStyleProperty); }
            set { SetValue(DownButtonStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DownButtonStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DownButtonStyleProperty =
            DependencyProperty.Register("DownButtonStyle", typeof(Style), typeof(SfUpDown), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the allignment for spin buttons
        /// </summary>
        public SpinButtonsAlignment SpinButtonsAlignment
        {
            get { return (SpinButtonsAlignment)GetValue(SpinButtonsAlignmentProperty); }
            set { SetValue(SpinButtonsAlignmentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SpinButtonsAlignment.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SpinButtonsAlignmentProperty =
            DependencyProperty.Register("SpinButtonsAlignment", typeof(SpinButtonsAlignment), typeof(SfUpDown), new PropertyMetadata(SpinButtonsAlignment.Right, new PropertyChangedCallback(OnSpinButtonsAlignmentChanged)));

       
        /// <summary>
        /// Gets or sets the content for up button
        /// </summary>
        public object UpDownContent
        {
            get { return (object)GetValue(UpDownContentProperty); }
            set { SetValue(UpDownContentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for UpDownContent.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty UpDownContentProperty =
            DependencyProperty.Register("UpDownContent", typeof(object), typeof(SfUpDown), new PropertyMetadata(null));

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
            DependencyProperty.Register("AccentBrush", typeof(Brush), typeof(SfUpDown), new PropertyMetadata(null));


        #endregion

        #region Private Methods

        void downButton_Click(object sender, RoutedEventArgs e)
        {
            if (DownButtonClick != null)
            {
                DownButtonClick(this, e);
            }
        }

        void upButton_Click(object sender, RoutedEventArgs e)
        {
            if (UpButtonClick != null)
            {
                UpButtonClick(this, e);
            }
        }

        //Changes the SpinButtons Alignment
        private void ChangeSpinButtonAlignment(SpinButtonsAlignment spinButtonAlignment)
        {
            if (spinButtonAlignment == SpinButtonsAlignment.Left)
            {
                if (outerGrid != null && upButton != null && downButton != null && PART_Content != null)
                {
                    (outerGrid.ColumnDefinitions[0] as ColumnDefinition).Width = new GridLength(1, GridUnitType.Auto);
                    (outerGrid.ColumnDefinitions[1] as ColumnDefinition).Width = new GridLength(1, GridUnitType.Auto);
                    (outerGrid.ColumnDefinitions[2] as ColumnDefinition).Width = new GridLength(1, GridUnitType.Star);
                    Grid.SetColumn(upButton, 1);
                    Grid.SetColumn(downButton, 0);
                    Grid.SetColumn(PART_Content, 2);
                }
            }
            else if (SpinButtonsAlignment == SpinButtonsAlignment.Right)
            {
                if (outerGrid != null && upButton != null && downButton != null && PART_Content != null)
                {
                    (outerGrid.ColumnDefinitions[0] as ColumnDefinition).Width = new GridLength(1, GridUnitType.Star);
                    (outerGrid.ColumnDefinitions[1] as ColumnDefinition).Width = new GridLength(1, GridUnitType.Auto);
                    (outerGrid.ColumnDefinitions[2] as ColumnDefinition).Width = new GridLength(1, GridUnitType.Auto);
                    Grid.SetColumn(PART_Content, 0);
                    Grid.SetColumn(upButton, 2);
                    Grid.SetColumn(downButton, 1);
                }
            }
            else
            {
                if (outerGrid != null && upButton != null && downButton != null && PART_Content != null)
                {
                    (outerGrid.ColumnDefinitions[0] as ColumnDefinition).Width = new GridLength(1, GridUnitType.Auto);
                    (outerGrid.ColumnDefinitions[1] as ColumnDefinition).Width = new GridLength(1, GridUnitType.Star);
                    (outerGrid.ColumnDefinitions[2] as ColumnDefinition).Width = new GridLength(1, GridUnitType.Auto);
                    Grid.SetColumn(upButton, 2);
                    Grid.SetColumn(PART_Content, 1);
                    Grid.SetColumn(downButton, 0);
                }
            }
        }

        #endregion

        #region Override Methods
        /// <summary>
        /// Initializes all the child elements of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.SfUpDown"/> control.
        /// </summary>
#if WINDOWS_PHONE||WINDOWS_PHONE_7||WPFSILVERLIGHT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            upButton = GetTemplateChild("PART_UpButton") as RepeatButton;
            downButton = GetTemplateChild("PART_DownButton") as RepeatButton;
#else
            upButton = GetTemplateChild("PART_UpButton") as Button;
            downButton = GetTemplateChild("PART_DownButton") as Button;
#endif
            outerGrid = GetTemplateChild("PART_OuterGrid") as Grid;
            PART_Content = GetTemplateChild("PART_Content") as ContentControl;
            if (upButton != null)
            {
                upButton.Click += upButton_Click;
            }
            if (downButton != null)
            {
                downButton.Click += downButton_Click;
            }
            ChangeSpinButtonAlignment(SpinButtonsAlignment);
            base.OnApplyTemplate();
        }

        #endregion       

        #region Callback Methods

        /// <summary>
        /// Invoked when Spin buttons allignment has changed
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnSpinButtonsAlignmentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((SfUpDown)obj != null)
            {
                ((SfUpDown)obj).OnSpinButtonsAlignmentChanged(args);
            }
        }

        /// <summary>
        /// Invoked when Spin buttons allignment has changed
        /// </summary>
        /// <param name="args"></param>
        protected void OnSpinButtonsAlignmentChanged(DependencyPropertyChangedEventArgs args)
        {
            ChangeSpinButtonAlignment((SpinButtonsAlignment)args.NewValue);
        }


        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when UpButton is clicked.
        /// </summary>
        public event RoutedEventHandler UpButtonClick;
        /// <summary>
        /// Event that is raised when DownButton is clicked.
        /// </summary>
        public event RoutedEventHandler DownButtonClick;

        #endregion  
     
    }
    /// <summary>
    /// Represents an enum list for the Spin button aignment
    /// </summary>
    public enum SpinButtonsAlignment
    {
        /// <summary>
        /// Align SpinButtons to Left
        /// </summary>
        Left,

        /// <summary>
        /// Align SpinButtons to Right
        /// </summary>
        Right,

        /// <summary>
        /// Align SpinButtons at both the end
        /// </summary>
        Both
    }

}
