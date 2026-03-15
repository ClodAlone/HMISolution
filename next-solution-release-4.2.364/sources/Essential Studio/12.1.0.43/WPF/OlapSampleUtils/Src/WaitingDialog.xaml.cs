#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace SampleUtils
{
    using System.Windows;
    
   
    using System;

    /// <summary>
    /// Interaction logic for WaitingDialog.xaml
    /// </summary>
    public partial class WaitingDialog : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaitingDialog"/> class.
        /// </summary>
        public WaitingDialog()
        {
            InitializeComponent();
            this.WindowStyle = WindowStyle.None;
            this.ShowInTaskbar = false;
            //this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //this.LoadingProgressbarPopup.PlacementRectangle = new Rect(-100, -100, 300, 300);
            this.Loaded += new RoutedEventHandler(WaitingDialog_Loaded);
           
        }

        void WaitingDialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoadingProgressbarPopup.IsOpen = true;                      
        }

       

        /// <summary>
        /// Closes the window.
        /// </summary>
        public void CloseWindow()
        {
            this.Close();
            this.LoadingProgressbarPopup .IsOpen = false;
        }

        /// <summary>
        /// Displays this instance.
        /// </summary>
        public void Display()
        {
            this.Show();
        }
    }
}
