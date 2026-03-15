#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Items Class for Custom Label support
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class Items : INotifyPropertyChanged
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        private string m_label;

        public string label
        {
            get { return m_label; }
            set
            {
                m_label = value;
                OnPropertyChanged("label");
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>

        private double m_value;

        public double value
        {
            get { return m_value; }
            set
            {
                m_value = value;
                OnPropertyChanged("value");
            }
        }

        #endregion Public Properties

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion INotifyPropertyChanged Members
    }
}