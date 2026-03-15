#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

#if SILVERLIGHT
using System.Windows.Data;
#endif

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// A Class having a single color and its variants colors.
    /// </summary>
    public class ColorGroupItem : Control
    {
        /// <summary>
        /// A collection of ColorGroupItem
        /// </summary>
        public Collection<ColorGroupItem> ColorGroupItemsCollection = new Collection<ColorGroupItem>();

        /// <summary>
        /// Identifies the <see cref="Color"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Brush), typeof(ColorGroupItem), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Identifies the <see cref="Variants"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty SetVariants = DependencyProperty.Register("Variants", typeof(bool), typeof(ColorGroupItem), new PropertyMetadata(false, new PropertyChangedCallback(IsVariantsChanged)));

        /// <summary>
        /// Identifies the <see cref="BorderWidth"/>  dependency property.
        /// </summary>
        public new static readonly DependencyProperty WidthProperty = DependencyProperty.Register("BorderWidth", typeof(double), typeof(ColorGroupItem), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the <see cref="BorderHeight"/>  dependency property.
        /// </summary>
        public static new readonly DependencyProperty HeightProperty = DependencyProperty.Register("BorderHeight", typeof(double), typeof(ColorGroupItem), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the <see cref="ColorName"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorNameProperty = DependencyProperty.Register("ColorName", typeof(string), typeof(ColorGroupItem), new PropertyMetadata("hi"));

        /// <summary>
        /// Identifies the <see cref="BorderMargin"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderMarginProperty = DependencyProperty.Register("BorderMargin", typeof(Thickness), typeof(ColorGroupItem), new PropertyMetadata(new Thickness(0, 0, 0, 0)));

        /// <summary>
        /// Identifies the <see cref="ItemMargin"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemMarginProperty = DependencyProperty.Register("ItemMargin", typeof(Thickness), typeof(ColorGroupItem), new PropertyMetadata(new Thickness(2, 0, 2, 0)));

        /// <summary>
        /// Identifies the <see cref="BorderThick"/>  dependency property.
        /// </summary>
        public static new readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThick", typeof(Thickness), typeof(ColorGroupItem), new PropertyMetadata(new Thickness(1, 1, 1, 1)));

        /// <summary>
        /// Identifies the <see cref="IsSelected"/>  dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(ColorGroupItem), new PropertyMetadata(false));

        /// <summary>
        /// Internal Vriables
        /// </summary>
        internal string colorname;

        internal Border StaticBorder;
        internal bool SelectedFlag = false;
        internal ColorGroup item;
        //internal bool IsSelected;
        //To remove Warnings
#if WPF
        internal new bool IsMouseOver = false;
#endif
#if SILVERLIGHT
         internal bool IsMouseOver=false;
#endif

        /// <summary>
        /// Gets or sets the value of the ColorName dependency property.
        /// </summary>
        public string ColorName
        {
            get
            {
                return (string)GetValue(ColorNameProperty);
            }

            set
            {
                SetValue(ColorNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Variants dependency property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:ColorGroupItem  Name=&quot;ColorGroupItem&quot;
        /// Variants=&quot;true&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>ColorGroupItem ColorGroup=new ColorGroupItem();</para>
        /// <para>ColorGroup.Variants=true;</para>
        /// </remarks>
        /// <value>
        /// Type : bool
        /// </value>
        public bool Variants
        {
            get
            {
                return (bool)GetValue(SetVariants);
            }

            set
            {
                SetValue(SetVariants, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the Color dependency property
        /// </summary>
        public Brush Color
        {
            get
            {
                return (Brush)GetValue(ColorProperty);
            }

            set
            {
                SetValue(ColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the BorderMargin dependency property
        /// </summary>
        public Thickness BorderMargin
        {
            get
            {
                return (Thickness)GetValue(BorderMarginProperty);
            }

            set
            {
                SetValue(BorderMarginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the BorderWidth dependency property.
        /// </summary>
        public double BorderWidth
        {
            get
            {
                return (double)GetValue(WidthProperty);
            }

            set
            {
                SetValue(WidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the BorderHeight dependency property.
        /// </summary>
        public double BorderHeight
        {
            get
            {
                return (double)GetValue(HeightProperty);
            }

            set
            {
                SetValue(HeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the BorderThickness dependency property
        /// </summary>
        public Thickness BorderThick
        {
            get
            {
                return (Thickness)GetValue(BorderThicknessProperty);
            }

            set
            {
                SetValue(BorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the ItemMargin dependency property
        /// </summary>
        public Thickness ItemMargin
        {
            get
            {
                return (Thickness)GetValue(ItemMarginProperty);
            }

            set
            {
                SetValue(ItemMarginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the IsSelected dependency property
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }

            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Creates the instance of ColorGroupItem control
        /// </summary>
        public ColorGroupItem()
        {
            DefaultStyleKey = typeof(ColorGroupItem);
        }

        private Border colorGroupItemBorder;

        /// <summary>
        /// Applies the Template for the control
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (colorGroupItemBorder != null)
            {
                colorGroupItemBorder.MouseEnter -= new MouseEventHandler(BorderMouseMove);
                colorGroupItemBorder.MouseLeave -= new MouseEventHandler(ColorGroupItemBorderMouseLeave);
                colorGroupItemBorder.MouseLeftButtonDown -= new MouseButtonEventHandler(ColorGroupItemBorderMouseLeftButtonDown);
            }

            this.LostFocus -= new RoutedEventHandler(ColorGroupItemLostFocus);

            ItemsControl itemcontrol = GetTemplateChild("Ic1") as ItemsControl;
            if (itemcontrol != null)
            {
                itemcontrol.ItemsSource = ColorGroupItemsCollection;
            }

            colorGroupItemBorder = GetTemplateChild("ItemBorder") as Border;

            if (colorGroupItemBorder != null)
            {
                colorGroupItemBorder.MouseEnter += new MouseEventHandler(BorderMouseMove);
                colorGroupItemBorder.MouseLeave += new MouseEventHandler(ColorGroupItemBorderMouseLeave);
                colorGroupItemBorder.MouseLeftButtonDown += new MouseButtonEventHandler(ColorGroupItemBorderMouseLeftButtonDown);
            }

            this.LostFocus += new RoutedEventHandler(ColorGroupItemLostFocus);

#if WPF
            ColorPickerPalette palette = VisualUtils.FindAncestor(this, typeof(ColorPickerPalette)) as ColorPickerPalette;
            if (palette != null)
            {
                BindingUtils.SetBinding(this, palette, ColorGroupItem.HeightProperty, ColorPickerPalette.BorderHeightProperty);
                BindingUtils.SetBinding(this, palette, ColorGroupItem.WidthProperty, ColorPickerPalette.BorderWidthProperty);
            }

            if (palette == null)
            {
                ColorGroup grp = VisualUtils.FindAncestor(this, typeof(ColorGroup)) as ColorGroup;
                if (grp != null && grp.colorpicker != null)
                {
                    palette = grp.colorpicker;
                    BindingUtils.SetBinding(this, palette, ColorGroupItem.HeightProperty, ColorPickerPalette.BorderHeightProperty);
                    BindingUtils.SetBinding(this, palette, ColorGroupItem.WidthProperty, ColorPickerPalette.BorderWidthProperty);
                }
            }
#endif

#if SILVERLIGHT
            ColorGroup grp = VisualUtil.FindAncestor(this, typeof(ColorGroup)) as ColorGroup;
            if (grp != null && grp.colorpicker != null)
            {
                Binding bind = new Binding();
                bind.Source = grp.colorpicker;
                bind.Path = new PropertyPath(ColorPickerPalette.BorderHeightProperty);
                BindingOperations.SetBinding(this, ColorGroupItem.HeightProperty, bind);

                bind = new Binding();
                bind.Source = grp.colorpicker;
                bind.Path = new PropertyPath(ColorPickerPalette.BorderWidthProperty);
                BindingOperations.SetBinding(this, ColorGroupItem.WidthProperty, bind);
            }
#endif
        }

        /// <summary>
        /// Method to find the parent of given element
        /// </summary>
        /// <param name="element">Element for which parent is to be found</param>
        /// <returns>Parent of type ColorGroup</returns>
        internal static ColorGroup GetBrushEditParentFromChildren(FrameworkElement element)
        {
            ColorGroup item = null;
            if (element != null)
            {
                item = element as ColorGroup;

                if (item == null)
                {
                    while (element != null)
                    {
                        element = VisualTreeHelper.GetParent(element) as FrameworkElement;

                        if (element is ColorGroup)
                        {
                            item = (ColorGroup)element;
                            break;
                        }
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Used to calculate the RGB values of variant colors
        /// </summary>
        /// <param name="inColor">The Color for which variants is to be generated</param>
        /// <param name="level">Indicates the level</param>
        /// <param name="name">Name of the color to be updated based on calculation</param>
        /// <param name="color">The name of the color</param>
        /// <returns>Returns Brush</returns>
        internal Brush Lighten(Color inColor, int level, out string name, string color)
        {
            byte a = inColor.A;
            byte r = inColor.R;
            byte g = inColor.G;
            byte b = inColor.B;
            double difr = 255 - r;
            double difg = 255 - g;
            double difb = 255 - b;
            name = String.Empty;
            if ((r == 255) && (g == 255) && (b == 255))
            {
                if (level == 1)
                {
                    r = g = b = (byte)(r - (r * 0.05));
                    name = color + ", darker 5%";
                }

                if (level == 2)
                {
                    r = g = b = (byte)(r - (r * 0.15));
                    name = color + ", darker 15%";
                }

                if (level == 3)
                {
                    r = g = b = (byte)(r - (r * 0.25));
                    name = color + ", darker 25%";
                }

                if (level == 4)
                {
                    r = g = b = (byte)(r - (r * 0.35));
                    name = color + ", darker 35%";
                }

                if (level == 5)
                {
                    r = g = b = (byte)(r - (r * 0.5));
                    name = color + ", darker 50%";
                }

                return new SolidColorBrush(System.Windows.Media.Color.FromArgb(a, r, g, b));
            }

            if ((r == 0) && (g == 0) && (b == 0))
            {
                if (level == 1)
                {
                    r = g = b = (byte)(255 * 0.5);
                    name = color + ", Lighter 50%";
                }

                if (level == 2)
                {
                    r = g = b = (byte)(255 * 0.35);
                    name = color + ", Lighter 35%";
                }

                if (level == 3)
                {
                    r = g = b = (byte)(255 * 0.25);
                    name = color + ", Lighter 25%";
                }

                if (level == 4)
                {
                    r = g = b = (byte)(255 * 0.15);
                    name = color + ",Lighter 15%";
                }

                if (level == 5)
                {
                    r = g = b = (byte)(255 * 0.05);
                    name = color + ", Lighter 5%";
                }

                return new SolidColorBrush(System.Windows.Media.Color.FromArgb(a, r, g, b));
            }

            if (((r >= 220) && (g >= 220) && (b >= 220)) || (r + g + b) > 690)
            {
                if (level == 1)
                {
                    r = (byte)(r - difr);
                    g = (byte)(g - difg);
                    b = (byte)(b - difb);
                    name = color + ", Darker 10%";
                }

                if (level == 2)
                {
                    r = (byte)(r - (difr * 2.5));
                    g = (byte)(g - (difg * 2.5));
                    b = (byte)(b - (difb * 2.5));
                    name = color + ", Darker 25%";
                }

                if (level == 3)
                {
                    r = (byte)(r - (difr * 5));
                    g = (byte)(g - (difg * 5));
                    b = (byte)(b - (difb * 5));
                    name = color + ", Darker 50%";
                }

                if (level == 4)
                {
                    r = (byte)((r - (difr * 5)) / 2);
                    g = (byte)((g - (difg * 5)) / 2);
                    b = (byte)((b - (difb * 5)) / 2);
                    name = color + ", Darker 75%";
                }

                if (level == 5)
                {
                    r = (byte)((r - (difr * 5)) / 6);
                    g = (byte)((g - (difg * 5)) / 6);
                    b = (byte)((b - (difb * 5)) / 6);
                    name = color + ", Darker 90%";
                }

                if (inColor.R == 255)
                {
                    r = 255;
                }

                if (inColor.G == 255)
                {
                    g = 255;
                }

                if (inColor.B == 255)
                {
                    b = 255;
                }
            }
            else
            {
                if (level == 1)
                {
                    r = (byte)(r + (difr * 0.8));
                    g = (byte)(g + (difg * 0.8));
                    b = (byte)(b + (difb * 0.8));
                    name = color + ", Lighter 80%";
                }

                if (level == 2)
                {
                    r = (byte)(r + (difr * 0.6));
                    g = (byte)(g + (difg * 0.6));
                    b = (byte)(b + (difb * 0.6));
                    name = color + ", Lighter 60%";
                }

                if (level == 3)
                {
                    r = (byte)(r + (difr * 0.4));
                    g = (byte)(g + (difg * 0.4));
                    b = (byte)(b + (difb * 0.4));
                    name = color + ", Lighter 40%";
                }

                if (level == 4)
                {
                    r = (byte)(r - (r / 4));
                    g = (byte)(g - (g / 4));
                    b = (byte)(b - (b / 4));
                    name = color + ", Darker 25%";
                }

                if (level == 5)
                {
                    double r1, g1, b1;
                    r1 = r - (r / 4);
                    g1 = g - (g / 4);
                    b1 = b - (b / 4);
                    r = (byte)(r1 - (r1 / 3));
                    g = (byte)(g1 - (g1 / 3));
                    b = (byte)(b1 - (b1 / 3));
                    name = color + ", Darker 50%";
                }
            }

            return new SolidColorBrush(System.Windows.Media.Color.FromArgb(a, r, g, b));
        }

        /// <summary>
        /// Called when changes in visual state of border takes place
        /// </summary>
        /// <param name="useTransitions">Indicate whether to apply transition or not</param>
        /// <param name="stateNames">Contain the state name</param>
        internal void GoToState(bool useTransitions, params string[] stateNames)
        {
            if (stateNames != null)
            {
                foreach (string str in stateNames)
                {
                    if (VisualStateManager.GoToState(this, str, useTransitions))
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Method is used to update the state of border
        /// </summary>
        /// <param name="useTransitions">Update the state</param>
        internal void UpdateVisualState(bool useTransitions)
        {
            if (this.IsSelected)
            {
                this.GoToState(useTransitions, new string[] { "Selected" });
            }
            else
            {
                this.GoToState(useTransitions, new string[] { "Normal" });
            }

            if (this.IsMouseOver)
            {
                this.GoToState(useTransitions, new string[] { "MouseOver" });
            }
            else
            {
                if (!this.IsSelected)
                {
                    this.GoToState(useTransitions, new string[] { "Normal" });
                }
            }
        }

        /// <summary>
        /// Event raised when Variants generation of colors is changed
        /// </summary>
        /// <param name="o">ColorGroupItem object where the change occures on</param>
        /// <param name="e">Property change details, such as old value and new value</param>
        private static void IsVariantsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColorGroupItem colorGroupItem = (ColorGroupItem)o;
            colorGroupItem.IsVariantsChanged(e);
        }

        /// <summary>
        /// Called when IsVariantsChanged Event is raised
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value</param>
        protected virtual void IsVariantsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.Variants)
            {
                Color color = ((SolidColorBrush)this.Color).Color;
                ColorGroupItemsCollection.Add(new ColorGroupItem() { Color = Lighten(color, 1, out colorname, ColorName), Variants = false, ColorName = colorname, BorderThick = new Thickness(1, 1, 1, 0) });
                ColorGroupItemsCollection.Add(new ColorGroupItem() { Color = Lighten(color, 2, out colorname, ColorName), Variants = false, ColorName = colorname, BorderThick = new Thickness(1, 0, 1, 0) });
                ColorGroupItemsCollection.Add(new ColorGroupItem() { Color = Lighten(color, 3, out colorname, ColorName), Variants = false, ColorName = colorname, BorderThick = new Thickness(1, 0, 1, 0) });
                ColorGroupItemsCollection.Add(new ColorGroupItem() { Color = Lighten(color, 4, out colorname, ColorName), Variants = false, ColorName = colorname, BorderThick = new Thickness(1, 0, 1, 0) });
                ColorGroupItemsCollection.Add(new ColorGroupItem() { Color = Lighten(color, 5, out colorname, ColorName), Variants = false, ColorName = colorname, BorderThick = new Thickness(1, 0, 1, 1) });
            }
        }

        /// <summary>
        /// Called when MouseLeave Occurs on ColorGroupItem
        /// </summary>
        /// <param name="sender">Object of the sender</param>
        /// <param name="e">Handles Mouse Event</param>
        private void ColorGroupItemBorderMouseLeave(object sender, MouseEventArgs e)
        {
            this.IsMouseOver = false;
            this.UpdateVisualState(false);
        }

        /// <summary>
        /// Called when LostFocus Occurs on ColorGroupItem
        /// </summary>
        /// <param name="sender">Object of the sender</param>
        /// <param name="e">Handles Routed Event</param>
        private void ColorGroupItemLostFocus(object sender, RoutedEventArgs e)
        {
            this.IsSelected = false;
            this.UpdateVisualState(false);
        }

        /// <summary>
        /// Called when LostFocus event occurs on border
        /// </summary>
        /// <param name="sender">Object of the sender</param>
        /// <param name="e">Handles RoutedEvent</param>
        private void ColorGroupItemBorder_LostFocus(object sender, RoutedEventArgs e)
        {
            this.IsSelected = false;
            this.UpdateVisualState(false);
        }

        /// <summary>
        /// Called when MouseLeftButtonDown occurs on border
        /// </summary>
        /// <param name="sender">Object of the sender</param>
        /// <param name="e">Handles Mouse Event</param>
        private void ColorGroupItemBorderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StaticBorder = sender as Border;
            this.IsMouseOver = false;
            this.IsSelected = true;
            this.UpdateVisualState(true);
            item = GetBrushEditParentFromChildren(StaticBorder);
            if (item.colorpicker.SelectedItem != null)
            {
                item.colorpicker.SelectedItem.IsSelected = false;
                item.colorpicker.SelectedItem.UpdateVisualState(false);
            }

            this.IsMouseOver = false;
            this.IsSelected = true;
            this.UpdateVisualState(true);
            SelectedFlag = true;
            if (item.colorpicker.IsSelected)
            {
                item.colorpicker.IsMouseOver = false;
                item.colorpicker.IsSelected = false;
                item.colorpicker.UpdateVisualState(false);
            }
            if (item.colorpicker.Popup != null)
                item.colorpicker.Popup.IsOpen = false;
            item.colorpicker.dispose();
            item.colorpicker.IsChecked = false;
            item.colorpicker.IsUpDownSelected = false;
            item.colorpicker.IsColorBorderSelected = false;
            item.colorpicker.UpdateVisualState(false);
            item.colorpicker.SelectedItem = this;
            if (item.colorpicker.SelectedMoreColor != null)
            {
                item.colorpicker.SelectedMoreColor = null;
            }

            item.colorpicker.ColorName = this.ColorName;
            item.colorpicker.Color = ((SolidColorBrush)this.Color).Color;
        }

        /// <summary>
        /// Called for mouseleave event over border
        /// </summary>
        /// <param name="sender">Object of the sender</param>
        /// <param name="e">Handles Mouse event</param>
        private void r_MouseLeave(object sender, MouseEventArgs e)
        {
            this.IsMouseOver = false;
            this.UpdateVisualState(false);
        }

        /// <summary>
        /// Called for mousemove event over border
        /// </summary>
        /// <param name="sender">Object of the sender</param>
        /// <param name="e">Handles Mouse event</param>
        private void BorderMouseMove(object sender, MouseEventArgs e)
        {
            this.IsMouseOver = true;
            this.UpdateVisualState(true);
        }
    }
}