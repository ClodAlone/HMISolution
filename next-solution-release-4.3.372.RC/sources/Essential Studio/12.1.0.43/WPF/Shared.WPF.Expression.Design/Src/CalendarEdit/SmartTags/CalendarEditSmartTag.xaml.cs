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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Tools;
using System.Diagnostics;
using Syncfusion.Windows.Design;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Shared.WPF.Expression.Design
{
    /// <summary>
    /// Interaction logic for AppMenuSmartTag.xaml
    /// </summary>
    public partial class CalendarEditSmartTag : SmartTagBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarEditSmartTag"/> class.
        /// </summary>
        public CalendarEditSmartTag()
        {
            InitializeComponent();
        }
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BindTextBox(Name1, "Name");
            BindSelectorWithBrushes(SelectionBorderBrush, "SelectionBorderBrush");
            BindSelectorWithBrushes(HeaderForeground, "HeaderForeground");
            BindSelectorWithBrushes(HeaderBackground, "HeaderBackground");
            BindSelectorWithBrushes(BackgroundBrush, "Background");
            BindSelectorWithBrushes(ForegroundBrush, "Foreground");
            BindSelectorWithEnum(CalendarStyle, "CalendarStyle", typeof(CalendarStyle));
            BindSelectorWithEnum(SelectionRangeMode, "SelectionRangeMode", typeof(SelectionRangeMode));
            BindSelectorWithEnum(MonthChangeDirection, "MonthChangeDirection", typeof(AnimationDirection));
            
            BindIntNumeric(FrameMovingTime, "FrameMovingTime");
            BindIntNumeric(ChangeModeTime, "ChangeModeTime");

            BindCheckBox(AllowSelection, "AllowSelection");
            BindCheckBox(AllowMultiplySelection, "AllowMultiplySelection");
            BindCheckBox(IsDayNamesAbbreviated, "IsDayNamesAbbreviated");
            BindCheckBox(IsMonthNameAbbreviated, "IsMonthNameAbbreviated");
            BindCheckBox(TodayRowIsVisible, "TodayRowIsVisible");
            BindCheckBox(IsShowWeekNumbers, "IsShowWeekNumbers");
            BindCheckBox(ShowPreviousMonthDays, "ShowPreviousMonthDays");
            BindCheckBox(ScrollToDateEnabled, "ScrollToDateEnabled");
            BindCheckBox(IsAllowYearSelection, "IsAllowYearSelection");
            BindCheckBox(ShowNextMonthDays, "ShowNextMonthDays");
        }
        /// <summary>
        /// Handles the LostFocus event of the Date control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Date_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Date.Text != null)
            {
                DateTime date;
                if (DateTime.TryParse(Date.Text, out date))
                    ModelItem.Properties["Date"].SetValue(date);
            }
        }

       

      

       

       
       

        


    }

}        

       


        
        
   

