#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.ObjectModel;
#if WINRT
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Media;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    #region NonAccessibleBlock

    /// <summary>
    /// Represents a non accessible block.
    /// </summary>
    public class NonAccessibleBlock : DependencyObject
    {
        #region Dependency Properties

        #region Background
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(NonAccessibleBlock), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0XF1, 0XF2, 0XF2))));
        #endregion

        #region StartHour
        public double StartHour
        {
            get { return (double)GetValue(StartHourProperty); }
            set { SetValue(StartHourProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartHour.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartHourProperty =
            DependencyProperty.Register("StartHour", typeof(double), typeof(NonAccessibleBlock), new PropertyMetadata(0d));
        #endregion

        #region EndHour
        public double EndHour
        {
            get { return (double)GetValue(EndHourProperty); }
            set { SetValue(EndHourProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndHour.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndHourProperty =
            DependencyProperty.Register("EndHour", typeof(double), typeof(NonAccessibleBlock), new PropertyMetadata(0d));
        #endregion

        #region Label
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(NonAccessibleBlock), new PropertyMetadata(string.Empty));
        #endregion

        #region Margin
        internal Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Margin.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MarginProperty =
            DependencyProperty.Register("Margin", typeof(Thickness), typeof(NonAccessibleBlock), new PropertyMetadata(new Thickness()));
        #endregion

        #region Size
        internal double Size
        {
            get { return (double)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Size.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(double), typeof(NonAccessibleBlock), new PropertyMetadata(0d));
        #endregion

        #region CustomTemplate
        internal DataTemplate CustomTemplate
        {
            get { return (DataTemplate)GetValue(CustomTemplateProperty); }
            set { SetValue(CustomTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomTemplate.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CustomTemplateProperty =
            DependencyProperty.Register("CustomTemplate", typeof(DataTemplate), typeof(NonAccessibleBlock), new PropertyMetadata(null));
        #endregion

        #endregion
    } 

    #endregion

    #region NonAccessibleBlockCollection

    /// <summary>
    /// Represents a collection of non accessible blocks in schedule.
    /// </summary>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleAppointment"/>
    public class NonAccessibleBlockCollection : ObservableCollection<NonAccessibleBlock>
    {
    } 

    #endregion
}
