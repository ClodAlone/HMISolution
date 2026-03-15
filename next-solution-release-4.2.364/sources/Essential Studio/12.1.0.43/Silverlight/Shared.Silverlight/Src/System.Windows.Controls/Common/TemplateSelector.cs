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

namespace Syncfusion.Windows.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class TemplateSelector : ContentControl
    {
        /// <summary>
        /// 
        /// </summary>
        public TemplateSelector()
        {
            //this.Loaded+=new RoutedEventHandler(TemplateSelector_Loaded);
        }

        /// <summary>
        /// Called when the value of the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property changes. 
        /// </summary>
        /// <param name="oldContent">The old value of the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property.</param><param name="newContent">The new value of the <see cref="P:System.Windows.Controls.ContentControl.Content"/> property.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);            
            if (newContent != null)
            {
                try
                {
                    ContentTemplate = SelectTemplate(newContent);
                }
                catch(NullReferenceException)
                {
                    throw new NullReferenceException("Data Template should not be null");
                }
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual DataTemplate SelectTemplate(object item)
        {
            return new DataTemplate();
        }
    }
}
