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
using System.ComponentModel;

namespace Syncfusion.RDL.Data
{
    public class ParameterReportData : INotifyPropertyChanged
    {
        bool selected = false;

        public bool IsSelected
        {
            get
            {
                return this.selected;
            }
            set
            {
                if (value != this.selected)
                {
                    this.selected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }

        public string ValueField { get; set; }

        public string DisplayField { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
