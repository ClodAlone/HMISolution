#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

#if WINRT_USING
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media; 
#else
using System.Windows.Controls;
using System.Windows.Media; 
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
#if !WINRT
    [DesignTimeVisible(false)] 
#endif
    public abstract class PortBase :
        ContentControl, 
        IPort
    {
#if WPF
        static PortBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PortBase), new FrameworkPropertyMetadata(typeof(PortBase)));
        } 
#endif

        protected PortBase()
        {
#if !WPF
            this.DefaultStyleKey = typeof(PortBase); 
#endif
            this.Loaded += PortBase_Loaded;
            Background = new SolidColorBrush(Colors.Transparent);
        }

        public PortConstraints Constraints
        {
            get { return (PortConstraints)GetValue(ConstraintsProperty); }
            set { SetValue(ConstraintsProperty, value); }
        }

        public static readonly DependencyProperty ConstraintsProperty =
            DependencyProperty.Register("Constraints", typeof(PortConstraints), typeof(NodePort), new PropertyMetadata(PortConstraints.Inherit));

        void PortBase_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            protected set { SetValue(OffsetXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OffsetX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register("OffsetX", typeof(double), typeof(PortBase), new PropertyMetadata(0d));

        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            protected set { SetValue(OffsetYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OffsetY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register("OffsetY", typeof(double), typeof(PortBase), new PropertyMetadata(0d));
        

        public bool IsConnecting
        {
            get { return (bool)GetValue(IsConnectingProperty); }
            set { SetValue(IsConnectingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDragConnectionOver.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsConnectingProperty =
            DependencyProperty.Register("IsConnecting", typeof(bool), typeof(PortBase), new PropertyMetadata(false));

        public object ID
        {
            get { return GetValue(IDProperty); }
            set { SetValue(IDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IDProperty =
            DependencyProperty.Register("ID", typeof(object), typeof(PortBase), new PropertyMetadata(null, 
                OnIDChanged));

        private static void OnIDChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PortBase).OnIDChanged(e);
        }

        private void OnIDChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        public object Key
        {
            get { return GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Key.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(object), typeof(PortBase), new PropertyMetadata(null, OnKeyChanged));

        private static void OnKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PortBase).OnKeyChanged(e);
        }

        private void OnKeyChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public object Shape
        {
            get { return (object)GetValue(ShapeProperty); }
            set { SetValue(ShapeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Shape.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShapeProperty =
            DependencyProperty.Register("Shape", typeof(object), typeof(PortBase), new PropertyMetadata("M0,0 L10,10 M10,0 L0,10", OnShapeChanged));

        private static void OnShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PortBase).OnPropertyChanged("Shape");
        }

        public Style ShapeStyle
        {
            get { return (Style)GetValue(ShapeStyleProperty); }
            set { SetValue(ShapeStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShapeStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShapeStyleProperty =
            DependencyProperty.Register("ShapeStyle", typeof(Style), typeof(PortBase), new PropertyMetadata(null, OnShapeStyleChanged));

        private static void OnShapeStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PortBase).OnShapeStyleChanged(e);
        }

        private void OnShapeStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged("ShapeStyle");
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
