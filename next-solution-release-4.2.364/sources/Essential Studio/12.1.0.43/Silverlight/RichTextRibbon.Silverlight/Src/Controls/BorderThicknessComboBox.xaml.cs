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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls
{
    public partial class BorderThicknessComboBox : UserControl
    {
        public BorderThicknessComboBox()
        {
            InitializeComponent();

            DataContext = this;

            ComboBoxItems = new List<ComboBoxItemTemplate>
            {
                new ComboBoxItemTemplate{ Text="1/4 pt",FillBrush=new SolidColorBrush(Color),Height=2},
                new ComboBoxItemTemplate{ Text="1/2 pt",FillBrush=new SolidColorBrush(Color),Height=2},
                new ComboBoxItemTemplate{ Text="3/4 pt",FillBrush=new SolidColorBrush(Color),Height=2},
                new ComboBoxItemTemplate{ Text="1 pt",FillBrush=new SolidColorBrush(Color),Height=2},
                new ComboBoxItemTemplate{ Text="1 1/2 pt",FillBrush=new SolidColorBrush(Color),Height=4},
                new ComboBoxItemTemplate{ Text="2 1/4 pt",FillBrush=new SolidColorBrush(Color),Height=6},
                new ComboBoxItemTemplate{ Text="3 pt",FillBrush=new SolidColorBrush(Color),Height=8},
                new ComboBoxItemTemplate{ Text="4 1/2 pt",FillBrush=new SolidColorBrush(Color),Height=10},
                new ComboBoxItemTemplate{ Text="6 pt",FillBrush=new SolidColorBrush(Color),Height=11},
            };

            Combo.Loaded += (sender, e) =>
                {
                    Combo.SelectedIndex = 3;
                };

        }
        
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(BorderThicknessComboBox), new PropertyMetadata(Colors.Black,new PropertyChangedCallback(OnColorChanged)));

        
        internal static void OnColorChanged(DependencyObject obj,DependencyPropertyChangedEventArgs e)
        {
            BorderThicknessComboBox combo = obj as BorderThicknessComboBox;

            foreach (object item in combo.ComboBoxItems)
            {
                ComboBoxItemTemplate template = item as ComboBoxItemTemplate;
                if (template != null)
                {
                    template.FillBrush = new SolidColorBrush((Color)e.NewValue);
                }
            }
        }
        

        public List<ComboBoxItemTemplate> ComboBoxItems
        {
            get { return (List<ComboBoxItemTemplate>)GetValue(ComboBoxItemsProperty); }
            set { SetValue(ComboBoxItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ComboBoxItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ComboBoxItemsProperty =
            DependencyProperty.Register("ComboBoxItems", typeof(List<ComboBoxItemTemplate>), typeof(BorderThicknessComboBox), null);


    }

    public class ComboBoxItemTemplate:INotifyPropertyChanged
    {
        string text = string.Empty;
        Brush fill = new SolidColorBrush(Colors.Black);
        double height = 0.0;

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                OnPropertyChanged("Text");
            }
        }

        public Brush FillBrush
        {
            get
            {
                return fill;
            }
            set
            {
                fill = value;
                OnPropertyChanged("FillBrush");
            }
        }

        public double Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
                OnPropertyChanged("Height");
            }
        }

        internal void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

