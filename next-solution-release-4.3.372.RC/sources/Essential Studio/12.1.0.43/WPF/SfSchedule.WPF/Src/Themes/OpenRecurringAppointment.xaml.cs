#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Input;

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents recurrence appointment window.
    /// </summary>
    public partial class OpenRecurringAppointment
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Schedule.OpenRecurringAppointment">OpenRecurringAppointment</see> class. 
        /// </summary>
        public OpenRecurringAppointment()
        {
            InitializeComponent();
        } 

        #endregion

        #region Events

        private void Close_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseBorder.Opacity = 0.6;
        }

        private void Close_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseBorder.Opacity = 1;
        }

        private void Close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void OpenOne_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Series_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        #endregion
    }
}

