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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Licensing;
#if SyncfusionFramework4_5
    using System.Windows.Shell;
#elif SyncfusionFramework4_0
    using Syncfusion.Windows;
#endif

namespace Syncfusion.Windows.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Syncfusion.SfChromelessWindow.WPF"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Syncfusion.SfChromelessWindow.WPF;assembly=Syncfusion.SfChromelessWindow.WPF"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <see cref="SfChromelessWindow"/>
    ///
    /// </summary>
    public class SfChromelessWindow : Window
    {
        
        private StackPanel Part_WindowsButton = null;

        static SfChromelessWindow()
        {
#if WPF
            if (EnvironmentTestSfChromelessWindow.IsSecurityGranted)
            {
                EnvironmentTestSfChromelessWindow.StartValidateLicense(typeof(SfChromelessWindow));
            }
#endif
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SfChromelessWindow), new FrameworkPropertyMetadata(typeof(SfChromelessWindow)));
        }



        /// <summary>
        /// Using a DependencyProperty as the backing store for ResizeBorderThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public Thickness ResizeBorderThickness
        {
            get { return (Thickness)GetValue(ResizeBorderThicknessProperty); }
            set { SetValue(ResizeBorderThicknessProperty, value); }
        }

      
        /// <summary>
        /// Using a DependencyProperty as the backing store for ResizeBorderThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResizeBorderThicknessProperty =
            DependencyProperty.Register("ResizeBorderThickness", typeof(Thickness), typeof(SfChromelessWindow), new PropertyMetadata(new Thickness(4)));



        /// <summary>
        /// Using a DependencyProperty as the backing store for CaptionFontSize.  This enables animation, styling, binding, etc...
        /// </summary>
        public double CaptionFontSize
        {
            get { return (double)GetValue(CaptionFontSizeProperty); }
            set { SetValue(CaptionFontSizeProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for CaptionFontSize.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CaptionFontSizeProperty =
            DependencyProperty.Register("CaptionFontSize", typeof(double), typeof(SfChromelessWindow), new PropertyMetadata(12.0));



        /// <summary>
        /// Using a DependencyProperty as the backing store for CaptionAlignment.  This enables animation, styling, binding, etc...
        /// </summary>
        public HorizontalAlignment CaptionAlignment
        {
            get { return (HorizontalAlignment)GetValue(CaptionAlignmentProperty); }
            set { SetValue(CaptionAlignmentProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for CaptionAlignment.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CaptionAlignmentProperty =
            DependencyProperty.Register("CaptionAlignment", typeof(HorizontalAlignment), typeof(SfChromelessWindow), new PropertyMetadata(HorizontalAlignment.Stretch));


        

        /// <summary>
        /// Using a DependencyProperty as the backing store for CaptionForeground.  This enables animation, styling, binding, etc...
        /// </summary>
        public Brush CaptionForeground
        {
            get { return (Brush)GetValue(CaptionForegroundProperty); }
            set { SetValue(CaptionForegroundProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for CaptionForeground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CaptionForegroundProperty =
            DependencyProperty.Register("CaptionForeground", typeof(Brush), typeof(SfChromelessWindow), new PropertyMetadata(Brushes.Transparent));



        /// <summary>
        /// Using a DependencyProperty as the backing store for CaptionBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public Brush CaptionBackground
        {
            get { return (Brush)GetValue(CaptionBackgroundProperty); }
            set { SetValue(CaptionBackgroundProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for CaptionBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CaptionBackgroundProperty =
            DependencyProperty.Register("CaptionBackground", typeof(Brush), typeof(SfChromelessWindow), new PropertyMetadata(Brushes.Transparent));



        /// <summary>
        /// Using a DependencyProperty as the backing store for CaptionHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public double CaptionHeight
        {
            get { return (double)GetValue(CaptionHeightProperty); }
            set { SetValue(CaptionHeightProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for CaptionHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CaptionHeightProperty =
            DependencyProperty.Register("CaptionHeight", typeof(double), typeof(SfChromelessWindow), new PropertyMetadata(25.0));



        /// <summary>
        /// Using a DependencyProperty as the backing store for GlassFrameThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public Thickness GlassFrameThickness
        {
            get { return (Thickness)GetValue(GlassFrameThicknessProperty); }
            set { SetValue(GlassFrameThicknessProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for GlassFrameThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GlassFrameThicknessProperty =
            DependencyProperty.Register("GlassFrameThickness", typeof(Thickness), typeof(SfChromelessWindow), new PropertyMetadata(new Thickness(1)));


        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            InitializeCommands();
            SetChrome();
            base.OnInitialized(e);
        }

        private void InitializeCommands()
        {
            var closecommand = new CommandBinding(Close, ExecuteClose, CanExecuteClose);
            var maximizecommand = new CommandBinding(Maximize, ExecuteMaximize, CanExecuteMaximize);
            var minimizecommand = new CommandBinding(Minimize, ExecuteMinimize);
            var restorecommand = new CommandBinding(Restore, ExecuteRestore, CanExecuteRestore);

            CommandBindings.Add(closecommand);
            CommandBindings.Add(maximizecommand);
            CommandBindings.Add(minimizecommand);
            CommandBindings.Add(restorecommand);
        }

        private void SetChrome()
        {
            var chrome = new WindowChrome();

            chrome.GlassFrameThickness = GlassFrameThickness;
            chrome.CaptionHeight = CaptionHeight;
            chrome.ResizeBorderThickness = ResizeBorderThickness;
            chrome.UseAeroCaptionButtons = false;
            
            WindowChrome.SetWindowChrome(this, chrome);
        }

        /// <summary>
        /// Close Command
        /// </summary>
        public new static readonly RoutedUICommand Close = new RoutedUICommand("Close", "Close", typeof(SfChromelessWindow));

        /// <summary>
        /// Maximize Command
        /// </summary>
        public static readonly RoutedUICommand Maximize = new RoutedUICommand("Maximize", "Maximize", typeof(SfChromelessWindow));

        /// <summary>
        /// Minimize Command
        /// </summary>
        public static readonly RoutedUICommand Minimize = new RoutedUICommand("Minimize", "Minimize", typeof(SfChromelessWindow));

        /// <summary>
        /// Restore Command
        /// </summary>
        public static readonly RoutedUICommand Restore = new RoutedUICommand("Restore", "Restore", typeof(SfChromelessWindow));

        private void ExecuteMaximize(object sender, ExecutedRoutedEventArgs args)
        {
            this.WindowState = WindowState.Maximized;
        }

        private void CanExecuteMaximize(object sender, CanExecuteRoutedEventArgs args)
        {
            if (WindowState == WindowState.Normal)
            {
                args.CanExecute = true;
            }
            else
            {
                args.CanExecute = false;
            }
        }

        private void ExecuteMinimize(object sender, ExecutedRoutedEventArgs args)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CanExecuteRestore(object sender, CanExecuteRoutedEventArgs args)
        {
            if (WindowState == WindowState.Maximized)
            {
                args.CanExecute = true;
            }
            else
            {
                args.CanExecute = false;
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            Part_WindowsButton = GetTemplateChild("PART_WindowsButton") as StackPanel;

            if(Part_WindowsButton!=null)
            {
                WindowChrome.SetIsHitTestVisibleInChrome(Part_WindowsButton,true);
            }

            base.OnApplyTemplate();
        }

        private void ExecuteRestore(object sender, ExecutedRoutedEventArgs args)
        {
            this.WindowState = WindowState.Normal;
        }

        private void CanExecuteClose(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }

        private void ExecuteClose(object sender, ExecutedRoutedEventArgs args)
        {
            this.Close();
        }
    }
}
