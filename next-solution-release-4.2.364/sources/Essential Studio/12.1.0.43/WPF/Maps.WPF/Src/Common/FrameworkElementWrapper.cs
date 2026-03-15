#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Data;

    internal class FrameworkElementContext : FrameworkElement
    {
        #region Value (DependencyProperty)

        /// <summary>
        /// Gets / sets the value
        /// </summary>
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(FrameworkElementContext), new PropertyMetadata(null));

        #endregion

        public void SetValueBinding(Binding binding)
        {
            var previousBinding = this.GetBindingExpression(ValueProperty);
            if (previousBinding != null)
            {
                this.ClearValue(ValueProperty);
            }

            this.SetBinding(ValueProperty, binding);
        }
    }
}
