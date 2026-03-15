#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;

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

        #region Dependency Properties

        #region IsSeriesClicked
        /// <summary>
        /// Gets or sets a value indicating whether IsSeries has been clicked or not.
        /// </summary>
        public bool IsSeriesClicked
        {
            get { return (bool)GetValue(IsSeriesClickedProperty); }
            set { SetValue(IsSeriesClickedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsSeriesClicked.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsSeriesClickedProperty =
            DependencyProperty.Register("IsSeriesClicked", typeof(bool), typeof(OpenRecurringAppointment), new PropertyMetadata(true));
        #endregion 

        #endregion

        #region Events

        private void OpenOne_Click(object sender, RoutedEventArgs e)
        {
            IsSeriesClicked = false;
            Close();
        }

        private void Series_Click(object sender, RoutedEventArgs e)
        {
            Close();
        } 

        #endregion
    }
}

