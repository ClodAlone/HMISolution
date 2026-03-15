// <copyright file="CartesianAxis3D.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Security.Permissions;
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
    using Syncfusion.Windows.Shared;

    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///     xmlns:MyNamespace="clr-namespace:Syncfusion.Windows.Chart._3DChart"
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///     xmlns:MyNamespace="clr-namespace:Syncfusion.Windows.Chart._3DChart;assembly=Syncfusion.Windows.Chart._3DChart"
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class CartesianAxis3D : Control
    {
        /// <summary>
        /// Gets or sets the value of the ChartAxis dependency property.
        /// </summary>
        public ChartAxis ChartAxis
        {
            get
            {
                return (ChartAxis)GetValue(ChartAxisProperty);
            }

            set
            {
                SetValue(ChartAxisProperty, value);
            }
        }

        /// <summary>
        /// Event that is raised when ChartAxis property is changed.
        /// </summary>
        public event PropertyChangedCallback ChartAxisChanged;

        /// <summary>
        /// Identifies the ChartAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty ChartAxisProperty =
            DependencyProperty.Register("ChartAxis", typeof(ChartAxis), typeof(CartesianAxis3D), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnChartAxisChanged)));

        /// <summary>
        /// Calls OnChartAxisChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnChartAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CartesianAxis3D instance = (CartesianAxis3D)d;
            instance.OnChartAxisChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises ChartAxisChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnChartAxisChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ChartAxisChanged != null)
            {
                ChartAxisChanged(this, e);
            }
        }

        /// <summary>
        /// Initializes static members of the <see cref="CartesianAxis3D"/> class.
        /// </summary>
        static CartesianAxis3D()
        {
            Syncfusion.Licensing.EnvironmentTest.ValidateLicense(typeof(CartesianAxis3D));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CartesianAxis3D), new FrameworkPropertyMetadata(typeof(CartesianAxis3D)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianAxis3D"/> class.
        /// </summary>
        /// <param name="axis">The axis value.</param>
        public CartesianAxis3D(ChartAxis axis)
        {
            ////ChartAxis = axis;
            Binding b = new Binding();
            b.Source = axis;
            ////BindingUtils.SetBinding( this, axis, ChartAxisProperty, new PropertyPath("ChartAxis"));
            BindingOperations.SetBinding(this, ChartAxisProperty, b);
            BindingOperations.SetBinding(this, DataContextProperty, b);
            ////this.DataContext = axis;
        }
    }
}
