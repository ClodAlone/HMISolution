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

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Class which loads dependent assemblies in designer.
    /// </summary>
    public class LoadDependentAssemblies
    {

        /// <summary>
        /// Initializes a new instance of the LoadDependentAssemblies class.
        /// </summary>
        public LoadDependentAssemblies()
        {
            Syncfusion.Windows.Gauge.CircularGauge circularguage = new Syncfusion.Windows.Gauge.CircularGauge();
            Syncfusion.Windows.Gauge.LinearGauge linearguage = new Syncfusion.Windows.Gauge.LinearGauge();
            Syncfusion.Windows.Gauge.DigitalGauge digitalguage = new Syncfusion.Windows.Gauge.DigitalGauge();
            circularguage = null;
            linearguage = null;
            digitalguage = null;

        }
    }
}
