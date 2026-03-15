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

#if !WINRT
using System.Windows;
using System.Windows.Controls;
#if WPFSILVERLIGHT
using System.Windows.Media;
#endif
#endif

#if WINDOWS_PHONE || WINDOWS_PHONE_7

namespace Syncfusion.WP.Controls.Input
#elif WPF
namespace Syncfusion.Windows.Controls.Input
#elif SILVERLIGHT

namespace Syncfusion.Tools.Controls.Input
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// Represents a class for Displaying the output.
    /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.SfCalculator"/>
    /// </summary>
    public class DisplayPane : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DisplayPane"/> class.
        /// </summary>
        public DisplayPane()
        {
            DefaultStyleKey = typeof (DisplayPane);
        }
     
#if WINRT || WPFSILVERLIGHT
#if WINRT
        /// <summary>
        /// Invoked when the mouse focus is lost
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerExited(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#else
        /// <summary>
        /// Occurs when the mouse focus is lost.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
#endif
        {

          SfCalculator parent=  FindVisualParent<SfCalculator>(this);
            if(parent!=null)
            {
#if WINRT
                parent.Focus(FocusState.Keyboard);
#else
                parent.Focus();
#endif
            }

#if WINRT
             base.OnPointerExited(e);
#else
            base.OnMouseLeftButtonUp(e);
#endif
        } 
#endif

        /// <summary>
        /// Gets or sets the data to be stored in the memory
        /// </summary>
        public decimal Memory
        {
            get { return (decimal)GetValue(MemoryProperty); }
            set { SetValue(MemoryProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Memory.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MemoryProperty =
            DependencyProperty.Register("Memory", typeof(decimal), typeof(DisplayPane), new PropertyMetadata(0.0m));



        /// <summary>
        /// Gets or sets the data to be displayed
        /// </summary>
        public string DisplayText
        {
            get { return (string)GetValue(DisplayTextProperty); }
            set { SetValue(DisplayTextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DisplayText.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DisplayTextProperty =
            DependencyProperty.Register("DisplayText", typeof(string), typeof(DisplayPane), new PropertyMetadata(String.Empty));


        /// <summary>
        /// Gets or sets the Expression
        /// </summary>
        public string Expression
        {
            get { return (string)GetValue(ExpressionProperty); }
            set { SetValue(ExpressionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Expression.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ExpressionProperty =
            DependencyProperty.Register("Expression", typeof(string), typeof(DisplayPane), new PropertyMetadata(String.Empty));

#if WINRT || WPFSILVERLIGHT
        /// <summary>
        /// Identifies the parent of the child object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child"></param>
        /// <returns></returns>
        public static T FindVisualParent<T>(DependencyObject child)
                where T : DependencyObject
        {
            // get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we’re looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                // use recursion to proceed with next level
                return FindVisualParent<T>(parentObject);
            }
        }
#endif
    }
}
