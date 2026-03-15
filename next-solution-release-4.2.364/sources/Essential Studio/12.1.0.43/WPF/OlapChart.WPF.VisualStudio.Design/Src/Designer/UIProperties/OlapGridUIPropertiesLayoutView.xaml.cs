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
using System.ComponentModel;
using Syncfusion.Windows.Shared.Olap;

namespace Syncfusion.OlapChart.WPF.VisualStudio.Design
{
    /// <summary>
    /// Provides the view for the wizard during design time.
    /// </summary>
    public partial class OlapGridUIPropertiesLayoutView
        : UserControl, 
        INotifyPropertyChanged//, 
        // IVisualDesigner
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridUIPropertiesLayoutView"/> class.
        /// </summary>
        /// <param name="objModel">The obj model.</param>
        public OlapGridUIPropertiesLayoutView()
        {
            InitializeComponent();
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        //#region IVisualDesigner Members

        //private object model = null;

        ///// <summary>
        ///// Gets or sets the model.
        ///// </summary>
        ///// <value>The model.</value>
        //public object Model
        //{
        //    get
        //    {
        //        return this.model;
        //    }
        //    set
        //    {
        //        if (value != null)
        //        {
        //            this.model = value;
        //            this.DataContext = this.model;
        //        }
        //    }
        //}

        //#endregion
    }
}
