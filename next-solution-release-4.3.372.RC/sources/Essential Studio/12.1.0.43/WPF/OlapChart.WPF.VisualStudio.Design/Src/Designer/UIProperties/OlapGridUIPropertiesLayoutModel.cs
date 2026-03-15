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
using System.ComponentModel;
using Microsoft.Windows.Design.Model;
using System.Windows.Media;
using System.Windows.Controls;

namespace Syncfusion.OlapChart.WPF.VisualStudio.Design
{
    /// <summary>
    /// This is the model class.
    /// </summary>
    public sealed class OlapGridUIPropertiesLayoutModel
        : INotifyPropertyChanged
    {
        #region Members

        private ModelItem selectedControl;

        #endregion

        #region Constructor

        public OlapGridUIPropertiesLayoutModel()
        {
            //// Initialize Properties here.
        }

        public OlapGridUIPropertiesLayoutModel(ModelItem selectedControl)
        {
            this.selectedControl = selectedControl;
        }

        #endregion

        #region Public Properties

        public string ModelName
        {
            get;
            set;
        }

        #endregion

        #region INotifyPropertyChanged Members

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
