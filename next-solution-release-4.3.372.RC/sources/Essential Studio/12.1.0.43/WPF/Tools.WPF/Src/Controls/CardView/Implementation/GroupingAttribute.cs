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

namespace Syncfusion.Windows.Tools.Controls
{
    [System.AttributeUsage(System.AttributeTargets.Property)]

    public class GroupingAttribute:System.Attribute,INotifyPropertyChanged
    {
        private bool _cangroup;

        public GroupingAttribute(bool cangroup)
        {
            CanGroup = cangroup;
        }

        public bool CanGroup 
        {
            get 
            { 
                return _cangroup; 
            }
            set 
            { 
                _cangroup=value;
                OnPropertyChanged("CanGroup");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string changecangroup)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(changecangroup));
            }
        }
    }
} 