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
using Syncfusion.Windows.Shared;
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Tools.Controls
{
    public partial class HighlightColorPicker : UserControl
    {
        public HighlightColorPicker()
        {
            InitializeComponent();
            DeclareClickEvents();            
        }

        public event EventHandler ColorClicked;

        /// <summary>
        /// Color Property of HighlightColorPicker
        /// </summary>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(HighlightColorPicker), new PropertyMetadata(Colors.Yellow));
        
        /// <summary>
        /// 
        /// </summary>
        internal void DeclareClickEvents()
        {
            foreach (StackPanel stack in ParentStack.Children)
            {
                foreach (object obj in stack.Children)
                {
                    if (obj is Button)
                    {
                        (obj as Button).Click += new RoutedEventHandler(OnButtonClick);
                    }
                    else if (obj is ContextMenuItemAdv)
                    {
                        (obj as ContextMenuItemAdv).Click += new RoutedEventHandler(OnMenuItemClick);
                    }
                }
            }
        }

        internal void OnButtonClick(object sender,RoutedEventArgs e)
        {
            Button button = sender as Button;
            Color = (button.Background as SolidColorBrush).Color;
            ColorClicked.Invoke(this, EventArgs.Empty);
        }

        internal void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            Color = Colors.White;
            ColorClicked.Invoke(this, EventArgs.Empty);
        }
    }
}
