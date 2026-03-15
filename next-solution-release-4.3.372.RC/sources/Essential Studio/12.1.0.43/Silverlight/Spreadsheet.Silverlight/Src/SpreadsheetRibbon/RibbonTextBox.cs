#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Syncfusion.Windows.Controls.Spreadsheet
{
    [DesignTimeVisible(false)]
    public class RibbonTextBox : TextBox
    {
        public RibbonTextBox()
        {
          
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.Text == string.Empty)
            {
                this.Text = MaskText;
            }
        }



#if !SILVERLIGHT

      
      protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (this.Text == MaskText)
            {
                this.Text = string.Empty;
            }
        }


        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            if (this.Text == string.Empty)
            {
                this.Text = MaskText;
            }
        }

#else
       

      

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            this.BorderThickness = new Thickness(1.0);
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            this.BorderThickness = new Thickness(0.0);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            if (this.Text == string.Empty)
            {
                this.Text = MaskText;
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            this.BorderThickness = new Thickness(1.0);
            if (this.Text == MaskText)
            {
                this.Text = string.Empty;
            }
        }

#endif



        /// <summary>
        /// Gets or sets the MaskText.
        /// </summary>
        /// <value>The Mask Text.</value>
        public string MaskText
        {
            get { return (string)this.GetValue(MaskTextProperty); }
            set { this.SetValue(MaskTextProperty, value); }
        }
        public static readonly DependencyProperty MaskTextProperty = DependencyProperty.Register(
          "MaskText", typeof(string), typeof(RibbonTextBox), new PropertyMetadata("Type here", MaskChanged));

        private static void MaskChanged(DependencyObject source,
 DependencyPropertyChangedEventArgs e)
        {

            RibbonTextBox textbox = source as RibbonTextBox;
            if (textbox != null && e.NewValue != null)
            {
                textbox.Text = e.NewValue as string;
            }


        }
    }
}
