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
using System.ComponentModel;

#if WPF
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Controls.Input
#elif WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Syncfusion.WP.Controls.Input
#elif SILVERLIGHT
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
namespace Syncfusion.Tools.Controls.Input
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
using System.Collections.ObjectModel;

namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// Items Class for Custom Label support
    /// </summary>
    public class Items : INotifyPropertyChanged
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        private string m_label;
        /// <summary>
        /// Gets or sets the label
        /// </summary>
        public string label
        {
            get { return m_label; }
            set { 
                m_label = value;
                OnPropertyChanged("label");
            }
        }
        

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
      
        private double m_value;

        /// <summary>
        /// Gets or sets the value entered by the user.
        /// </summary>
        public double value
        {
            get { return m_value; }
            set { 
                m_value = value;
                OnPropertyChanged("value");
            }
        }
        

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Invokes an event when there is a change in prperty.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when the property changed.
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
}
