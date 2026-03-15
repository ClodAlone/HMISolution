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
    public partial class DateTimeEditSmartTag : SmartTagBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeEditSmartTag"/> class.
        /// </summary>
        public DateTimeEditSmartTag()
        {
            InitializeComponent();
        }


        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            BindSelectorWithBrushes(IncorrectForegroundSelector, "IncorrectForeground");
            BindSelectorWithBrushes(CorrectForegroundSelector, "CorrectForeground");
            BindSelectorWithBrushes(UncertainForegroundselector, "UncertainForeground");
            BindTextBox(Name1, "Name");


            BindCheckBox(switchanimation, "IsAnimation");
            BindCheckBox(isAutoCorrect, "IsAutoCorrect");
            BindCheckBox(IsScrollingOnCircle, "IsScrollingOnCircle");
            BindCheckBox(IsEmptyDate, "IsEmptyDateEnabled");
            BindCheckBox(Iswatchenabled, "IsWatchEnabled");
            BindCheckBox(IsCalendarEnabled, "IsCalendarEnabled");
            //BindCheckBox(iseditable, "IsEditable");
            BindCheckBox(isreadonly, "ReadOnly");
            BindCheckBox(IsButtonPopUpEnabled, "IsButtonPopUpEnabled");
            
            BindCheckBox(IsEnabledRepeatButton, "IsEnabledRepeatButton");
            BindNumeric(AutoCorrectedHiglightDuration,"AutoCorrectedHiglightDuration");
            BindNumeric(ScrollDuration, "ScrollDuration");
            BindCheckBox(IsVisibleRepeatButton, "IsVisibleRepeatButton");
            //BindCheckBox(ShowNoDateTime, "ShowNoDateTime");
            BindCheckBox(IsHoldMaxWidth, "IsHoldMaxWidth");
            BindTextBox(NoneDateText, "NoneDateText");
            BindTextBox(CustomPattern, "CustomPattern");
            BindSelectorWithEnum(Patternselector, "Pattern", typeof(DateTimePattern));
        }
        /// <summary>
        /// Isdoubles the value.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        private bool IsdoubleValue(string str)
        {
            try
            {
                double.Parse(str);
                return true;
            }
            catch 
            {
                MessageBox.Show("Property value is invalid");
                return false;
            }
        }
        /// <summary>
        /// Handles the KeyDown event of the PopUpDelay control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void PopUpDelay_KeyDown(object sender, KeyEventArgs e)
        {
            if (PopUpDelay.Text.Trim() != string.Empty)
                if (IsdoubleValue(PopUpDelay.Text))
                    ModelItem.Properties["PopupDelay"].SetValue(TimeSpan.FromMilliseconds(double.Parse(PopUpDelay.Text)));
        }

        /// <summary>
        /// Handles the LostFocus event of the PopUpDelay control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void PopUpDelay_LostFocus(object sender, RoutedEventArgs e)
        {
            if (PopUpDelay.Text.Trim() != string.Empty)
                if (IsdoubleValue(PopUpDelay.Text))
                    ModelItem.Properties["PopupDelay"].SetValue(TimeSpan.FromMilliseconds(double.Parse(PopUpDelay.Text)));
        }


    }

}        

       


        
        
   

