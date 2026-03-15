// <copyright file="SmartTagBase.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
using System.IO;
using System.Xml;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Design
{
    /// <summary>
    /// SmartTag Base class helps us to provide the SmartTag designer support to the controls.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class SmartTagBase : UserControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the model item.
        /// </summary>
        /// <value>The model item.</value>
        public ModelItem ModelItem { get; set; }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public EditingContext Context { get; set; }

        /// <summary>
        /// Gets or sets the the control instance for which the SmartTag is being created.
        /// </summary>
        /// <value>The view of the SmartTag</value>
        public DependencyObject View { get; set; }

        #endregion

        #region New Implementation

        /// <summary>
        /// Binds the double text box.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected void BindIntegerTextBox(IntegerTextBox control, string propertyName)
        {
            control.Tag = propertyName;
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = this.ModelItem.View;

#if SyncfusionFramework4_0
            binding.Converter = new ModelItemToComputedValueConverter();
#endif
            control.SetBinding(IntegerTextBox.ValueProperty, binding);

#if SyncfusionFramework4_0
            control.SetCurrentValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
#else
            if (this.ModelItem.Properties[propertyName].ComputedValue != null)
            {
                control.SetValue(IntegerTextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
            }
#endif
            control.KeyDown += new KeyEventHandler(IntegerTextBox_KeyDown);
            control.LostFocus += new RoutedEventHandler(IntegerTextBox_LostFocus);
        }

        protected void BindCurrencyTextBox(CurrencyTextBox control, string propertyName)
        {
            control.Tag = propertyName;
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = this.ModelItem.View;

#if SyncfusionFramework4_0
            binding.Converter = new ModelItemToComputedValueConverter();
#endif
            control.SetBinding(CurrencyTextBox.ValueProperty, binding);

#if SyncfusionFramework4_0
            control.SetCurrentValue(CurrencyTextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
#else
            if (this.ModelItem.Properties[propertyName].ComputedValue != null)
            {
                control.SetValue(CurrencyTextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
            }
#endif
            control.KeyDown += new KeyEventHandler(CurrencyValueFromIntegerTextBox_KeyDown);
            control.LostFocus += new RoutedEventHandler(CurrencyValueFromIntegerTextBox_LostFocus);
        }

        /// <summary>
        /// Binds the double text box.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected void BindDoubleTextBox(DoubleTextBox control, string propertyName)
        {
            control.Tag = propertyName;
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = this.ModelItem.View;

#if SyncfusionFramework4_0
            binding.Converter = new ModelItemToComputedValueConverter();
#endif
            control.SetBinding(DoubleTextBox.ValueProperty, binding);

#if SyncfusionFramework4_0
            control.SetCurrentValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
#else
            if (this.ModelItem.Properties[propertyName].ComputedValue != null)
            {
                control.SetValue(DoubleTextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
            }
#endif
            control.KeyDown += new KeyEventHandler(DoubleTextBox_KeyDown);
            control.LostFocus += new RoutedEventHandler(DoubleTextBox_LostFocus);
        }

        /// <summary>
        /// Handles the LostFocus event of the IntegerTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void DoubleTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            DoubleTextBox textBox = sender as DoubleTextBox;

            string propertyName = (string)textBox.Tag;

            this.ModelItem.Properties[propertyName].SetValue((double)textBox.Value);

            textBox.SetValue(DoubleTextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
        }

        /// <summary>
        /// Handles the KeyDown event of the IntegerTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void DoubleTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            DoubleTextBox textBox = sender as DoubleTextBox;
            if (e.Key == Key.Enter && textBox != null)
            {
                string propertyName = (string)textBox.Tag;
                this.ModelItem.Properties[propertyName].SetValue((double)textBox.Value);
            }
        }

        /// <summary>
        /// Binds the masked text box.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected void BindMaskedTextBox(MaskedTextBox control, string propertyName)
        {
            control.Tag = propertyName;
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = this.ModelItem.View;

#if SyncfusionFramework4_0
            binding.Converter = new ModelItemToComputedValueConverter();
#endif
            control.SetBinding(MaskedTextBox.TextProperty, binding);

#if SyncfusionFramework4_0
            control.SetCurrentValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
#else
            if (this.ModelItem.Properties[propertyName].ComputedValue != null)
            {
                control.SetValue(MaskedTextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
            }
#endif
            control.KeyDown += new KeyEventHandler(MaskedTextBox_KeyDown);
            control.LostFocus += new RoutedEventHandler(MaskedTextBox_LostFocus);
        }

        /// <summary>
        /// Handles the LostFocus event of the MaskedTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void MaskedTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            MaskedTextBox textBox = sender as MaskedTextBox;
            string propertyName = (string)textBox.Tag;
            System.Windows.CornerRadiusConverter cornerRadiusConverter = new System.Windows.CornerRadiusConverter();

            if (textBox.Text != String.Empty)
            {
                try
                {
                    CornerRadius cornerRadius = (CornerRadius)cornerRadiusConverter.ConvertFromString(FourPointConverter(textBox.Text));
                    this.ModelItem.Properties[propertyName].SetValue(cornerRadius);
                }
                catch
                {
                    //MessageBox.Show(ex.Message);

                    textBox.SetValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());

                }
            }
            else
            {
                //MessageBox.Show("Value must contain one or four delimited Lengths");
                textBox.SetValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());

            }
        }

        /// <summary>
        /// Handles the KeyDown event of the MaskedTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void MaskedTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MaskedTextBox textBox = sender as MaskedTextBox;
                string propertyName = (string)textBox.Tag;
                System.Windows.CornerRadiusConverter cornerRadiusConverter = new System.Windows.CornerRadiusConverter();

                if (textBox.Text != String.Empty)
                {

                    CornerRadius cornerRadius = (CornerRadius)cornerRadiusConverter.ConvertFromString(FourPointConverter(textBox.Text));
                    this.ModelItem.Properties[propertyName].SetValue(cornerRadius);

                }
                else
                {
                    //MessageBox.Show("Value must contain one or four delimited Lengths");
                    textBox.SetValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());

                }
            }
        }

        /// <summary>
        /// Handles the LostFocus event of the DoubleTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void IntegerTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            IntegerTextBox textBox = sender as IntegerTextBox;

            string propertyName = (string)textBox.Tag;
            if (!propertyName.Equals("Value"))
            {
                if (textBox.Value.HasValue)
                {
                    this.ModelItem.Properties[propertyName].SetValue((Int32)textBox.Value.Value);
                }
            }
            else
            {
                this.ModelItem.Properties[propertyName].SetValue(textBox.Value);
            }
            textBox.SetValue(IntegerTextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
        }

        /// <summary>
        /// Handles the KeyDown event of the DoubleTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void IntegerTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            IntegerTextBox textBox = sender as IntegerTextBox;
            if (e.Key == Key.Enter && textBox != null)
            {
                string propertyName = (string)textBox.Tag;
                if (!propertyName.Equals("Value"))
                {
                    if (textBox.Value.HasValue)
                    {
                        this.ModelItem.Properties[propertyName].SetValue((Int32)textBox.Value.Value);
                    }
                }
                else
                {
                    this.ModelItem.Properties[propertyName].SetValue(textBox.Value);
                }

            }
        }

        private void CurrencyValueFromIntegerTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            CurrencyTextBox textBox = sender as CurrencyTextBox;

            string propertyName = (string)textBox.Tag;
            if (!propertyName.Equals("Value"))
            {
                if (textBox.Value.HasValue)
                {
                    this.ModelItem.Properties[propertyName].SetValue(textBox.Value);
                }
            }
            else
            {
                this.ModelItem.Properties[propertyName].SetValue(textBox.Value);
            }
        }

    
        private void CurrencyValueFromIntegerTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            CurrencyTextBox textBox = sender as CurrencyTextBox;
            if (e.Key == Key.Enter && textBox != null)
            {
                string propertyName = (string)textBox.Tag;
                if (!propertyName.Equals("Value"))
                {
                    if (textBox.Value.HasValue)
                    {
                        this.ModelItem.Properties[propertyName].SetValue(textBox.Value);
                    }
                }
                else
                {
                    this.ModelItem.Properties[propertyName].SetValue(textBox.Value);
                }

            }
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Binding the check box value from SmartTag.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected void BindCheckBox(CheckBox control, string propertyName)
        {
            control.Tag = propertyName;
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = this.ModelItem.View;
            control.SetBinding(CheckBox.IsCheckedProperty, binding);
#if SyncfusionFramework4_0
            try
            {
                control.SetCurrentValue(CheckBox.IsCheckedProperty, this.ModelItem.Properties[propertyName].ComputedValue);
            }
            catch (Exception ee)
            {
                MessageBox.Show("Invalid Visual Parent, Please add " + this.ModelItem.ItemType.Name + " in proper container and reload the designer.");
            }
#endif
            control.Checked += OnCheckedChanged;
            control.Unchecked += OnCheckedChanged;
            control.Indeterminate += OnCheckedChanged;
        }

        /// <summary>
        /// Binding the text box value from SmartTag.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected void BindTextBox(TextBox control, string propertyName)
        {
            control.Tag = propertyName;
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = this.ModelItem.View;

#if SyncfusionFramework4_0
            binding.Converter = new ModelItemToComputedValueConverter();
#endif
            control.SetBinding(TextBox.TextProperty, binding);

#if SyncfusionFramework4_0

            if (this.ModelItem != null && this.ModelItem.Properties[propertyName] != null && this.ModelItem.Properties[propertyName].ComputedValue != null)
            {
                control.SetCurrentValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
            }
#else
            if (this.ModelItem.Properties[propertyName] != null && this.ModelItem.Properties[propertyName].ComputedValue != null)
            {
                control.SetValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
            }
#endif

            control.KeyDown += new System.Windows.Input.KeyEventHandler(TextBox_KeyDown);
            control.LostFocus += new RoutedEventHandler(TextBox_LostFocus);
        }

        /// <summary>
        /// Binding the thickness value from SmartTag.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected void BindThickness(TextBox control, string propertyName)
        {
            control.Tag = propertyName;
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = this.ModelItem.View;
            control.SetBinding(TextBox.TextProperty, binding);
#if SyncfusionFramework4_0
            control.SetCurrentValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
#endif
            control.KeyDown += new System.Windows.Input.KeyEventHandler(Thickness_KeyDown);
            control.LostFocus += new RoutedEventHandler(Thickness_LostFocus);
        }

        /// <summary>
        /// Binding the thickness value from SmartTag.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected void BindThicknessWithNull(TextBox control, string propertyName)
        {
            control.Tag = propertyName;
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = this.ModelItem.View;
            control.SetBinding(TextBox.TextProperty, binding);
#if SyncfusionFramework4_0
            control.SetCurrentValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
#endif
            control.KeyDown += new System.Windows.Input.KeyEventHandler(ThicknessNull_KeyDown);
            control.LostFocus += new RoutedEventHandler(ThicknessNull_LostFocus);
        }

        /// <summary>
        /// Binding the numeric value from SmartTag.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected void BindNumeric(TextBox control, string propertyName)
        {
            control.Tag = propertyName;
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = this.ModelItem.View;
            control.SetBinding(TextBox.TextProperty, binding);
#if SyncfusionFramework4_0
            control.SetCurrentValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
#endif
            control.KeyDown += new System.Windows.Input.KeyEventHandler(NumericTextBox_KeyDown);
            control.LostFocus += new RoutedEventHandler(NumericTextBox_LostFocus);
        }

        /// <summary>
        /// Binding the numeric value from SmartTag.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected void BindNumericWithNull(TextBox control, string propertyName)
        {
            control.Tag = propertyName;
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = this.ModelItem.View;
            control.SetBinding(TextBox.TextProperty, binding);
#if SyncfusionFramework4_0
            control.SetCurrentValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
#endif
            control.KeyDown += new System.Windows.Input.KeyEventHandler(NumericTextBoxNull_KeyDown);
            control.LostFocus += new RoutedEventHandler(NumericTextBoxNull_LostFocus);
        }

        /// <summary>
        /// Binding the corner radius value from SmartTag.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected void BindCornerRadius(TextBox control, string propertyName)
        {
            control.Tag = propertyName;
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = this.ModelItem.View;
            control.SetBinding(TextBox.TextProperty, binding);
#if SyncfusionFramework4_0
            control.SetCurrentValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
#endif
            control.KeyDown += new System.Windows.Input.KeyEventHandler(CornerRadiTextBox_KeyDown);
            control.LostFocus += new RoutedEventHandler(CornerRadiTextBox_LostFocus);
        }

        /// <summary>
        /// Binding the integer numeric value from SmartTag.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected void BindIntNumeric(TextBox control, string propertyName)
        {
            control.Tag = propertyName;
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = this.ModelItem.View;
            control.SetBinding(TextBox.TextProperty, binding);
#if SyncfusionFramework4_0
            control.SetCurrentValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
#endif
            control.KeyDown += new System.Windows.Input.KeyEventHandler(IntNumericTextBox_KeyDown);
            control.LostFocus += new RoutedEventHandler(IntNumericTextBox_LostFocus);
        }

        /// <summary>
        /// Binding the integer numeric value from SmartTag.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected void BindIntNumericWithNull(TextBox control, string propertyName)
        {
            control.Tag = propertyName;
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = this.ModelItem.View;
            control.SetBinding(TextBox.TextProperty, binding);
#if SyncfusionFramework4_0
            control.SetCurrentValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
#endif
            control.KeyDown += new System.Windows.Input.KeyEventHandler(IntNumericTextBoxNull_KeyDown);
            control.LostFocus += new RoutedEventHandler(IntNumericTextBoxNull_LostFocus);
        }

        /// <summary>
        /// Binding the selector value from SmartTag.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected void BindSelector(Selector control, string propertyName)
        {
            control.Tag = propertyName;
            control.SelectionChanged += new SelectionChangedEventHandler(Selector_SelectionChanged);
        }

        /// <summary>
        /// Binding the selector with enum value from SmartTag.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="enumType">Type of the enum.</param>
        protected void BindSelectorWithEnum(Selector control, string propertyName, Type enumType)
        {
            control.Tag = propertyName;
            foreach (object o in Enum.GetValues(enumType))
            {
                control.Items.Add(o.ToString());
            }

            control.SelectedItem = this.ModelItem.Properties[propertyName].ComputedValue.ToString();
            control.SelectionChanged += new SelectionChangedEventHandler(EnumSelector_SelectionChanged);
        }

        /// <summary>
        /// Binds the selector with brushes value from SmartTag..
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected void BindSelectorWithBrushes(Selector control, string propertyName)
        {
            control.Tag = propertyName;
            //foreach (MemberInfo info in typeof(Brushes).GetMembers())
            //{
            //    if (info is PropertyInfo)
            //    {
            //        PropertyInfo pi = info as PropertyInfo;
            //        control.Items.Add(pi.Name);
            //    }
            //}
            BrushCollection brushCollection = new BrushCollection();
            control.ItemsSource = brushCollection;
            StringReader reader = new StringReader(@"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                                        <TextBlock Text=""{Binding Name}""/>
                                 </DataTemplate>");
            XmlReader readerXml = XmlReader.Create(reader);
            control.ItemTemplate = (DataTemplate)XamlReader.Load(readerXml);

            if (this.ModelItem.Properties[propertyName].ComputedValue is SolidColorBrush)
            {
                var items = brushCollection.Where(br => br.Brush.Color.Equals(((SolidColorBrush)this.ModelItem.Properties[propertyName].ComputedValue).Color));
                foreach (BrushObject obj in items)
                {
                    int index = brushCollection.IndexOf(obj);
                    if (index >= 0)
                    {
#if SyncfusionFramework4_0
                        control.SetCurrentValue(Selector.SelectedIndexProperty, index);
#else
                        control.SelectedIndex = index;
#endif
                    }
                    break;
                }
            }
            control.SelectionChanged += new SelectionChangedEventHandler(BrushesSelector_SelectionChanged);
        }

        /// <summary>
        /// Binding the selector with color value from SmartTag.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected void BindSelectorWithColors(Selector control, string propertyName)
        {
            control.Tag = propertyName;

            foreach (MemberInfo info in typeof(Colors).GetMembers())
            {
                if (info is PropertyInfo)
                {
                    PropertyInfo pi = info as PropertyInfo;
                    control.Items.Add(pi.Name);
                }
            }

            Color color = (Color)ColorConverter.ConvertFromString(this.ModelItem.Properties[propertyName].ComputedValue.ToString());
            control.SelectedItem = color.ToString();
            control.SelectionChanged += new SelectionChangedEventHandler(ColorsSelector_SelectionChanged);
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ColorsSelector control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ColorsSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Selector selector = sender as Selector;
            string propertyName = (string)selector.Tag;
            Color color = (Color)ColorConverter.ConvertFromString(selector.SelectedItem.ToString());
            this.ModelItem.Properties[propertyName].SetValue(color);
        }

        /// <summary>
        /// Handles the SelectionChanged event of the EnumSelector control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void EnumSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Selector selector = sender as Selector;
            string propertyName = (string)selector.Tag;

            Type enumType = this.ModelItem.Properties[propertyName].PropertyType;
            this.ModelItem.Properties[propertyName].SetValue(Enum.Parse(enumType, selector.SelectedValue.ToString()));
        }

        /// <summary>
        /// Handles the SelectionChanged event of the BrushesSelector control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void BrushesSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Selector selector = sender as Selector;
            string propertyName = (string)selector.Tag;
            BrushConverter conv = new BrushConverter();
            SolidColorBrush brush = (selector.SelectedItem as BrushObject).Brush;
            this.ModelItem.Properties[propertyName].SetValue(brush);
        }

        /// <summary>
        /// Handles the KeyDown event of the CornerRadiTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void CornerRadiTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = sender as TextBox;
                string propertyName = (string)textBox.Tag;
                System.Windows.CornerRadiusConverter cornerRadiusConverter = new System.Windows.CornerRadiusConverter();

                if (textBox.Text != String.Empty)
                {

                    CornerRadius cornerRadius = (CornerRadius)cornerRadiusConverter.ConvertFromString(FourPointConverter(textBox.Text));
                    this.ModelItem.Properties[propertyName].SetValue(cornerRadius);

                }
                else
                {
                    //MessageBox.Show("Value must contain one or four delimited Lengths");
                    textBox.SetValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());

                }
            }
        }

        /// <summary>
        /// Handles the LostFocus event of the CornerRadiTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CornerRadiTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string propertyName = (string)textBox.Tag;
            System.Windows.CornerRadiusConverter cornerRadiusConverter = new System.Windows.CornerRadiusConverter();

            if (textBox.Text != String.Empty)
            {
                try
                {
                    CornerRadius cornerRadius = (CornerRadius)cornerRadiusConverter.ConvertFromString(FourPointConverter(textBox.Text));
                    this.ModelItem.Properties[propertyName].SetValue(cornerRadius);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);

                    textBox.SetValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());

                }
            }
            else
            {
                //MessageBox.Show("Value must contain one or four delimited Lengths");
                textBox.SetValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());

            }
        }

        /// <summary>
        /// Handles the KeyDown event of the Thickness control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void Thickness_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = sender as TextBox;
                string propertyName = (string)textBox.Tag;
                ThicknessConverter thicknessConverter = new ThicknessConverter();
                try
                {
                    Thickness thickness = (Thickness)thicknessConverter.ConvertFromString(FourPointConverter(textBox.Text));
                    this.ModelItem.Properties[propertyName].SetValue(thickness);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                    textBox.SetValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());

                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the LostFocus event of the Thickness control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Thickness_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string propertyName = (string)textBox.Tag;
            ThicknessConverter thicknessConverter = new ThicknessConverter();
            try
            {
                Thickness thickness = (Thickness)thicknessConverter.ConvertFromString(FourPointConverter(textBox.Text));
                this.ModelItem.Properties[propertyName].SetValue(thickness);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                textBox.SetValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());

            }
        }

        /// <summary>
        /// Handles the KeyDown event of the Thickness control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void ThicknessNull_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = sender as TextBox;
                string propertyName = (string)textBox.Tag;
                ThicknessConverter thicknessConverter = new ThicknessConverter();
                try
                {
                    Thickness thickness = (Thickness)thicknessConverter.ConvertFromString(FourPointConverter(textBox.Text));
                    if (this.ModelItem.Content != null)
                    {
                        this.ModelItem.Properties[propertyName].SetValue(thickness);
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                    textBox.SetValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());

                }
            }
        }

        /// <summary>
        /// Handles the LostFocus event of the Thickness control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ThicknessNull_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string propertyName = (string)textBox.Tag;
            ThicknessConverter thicknessConverter = new ThicknessConverter();
            try
            {
                Thickness thickness = (Thickness)thicknessConverter.ConvertFromString(FourPointConverter(textBox.Text));
                if (this.ModelItem.Content != null)
                {
                    this.ModelItem.Properties[propertyName].SetValue(thickness);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                textBox.SetValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());

            }
        }

        /// <summary>
        /// Handles the KeyDown event of the TextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (e.Key == Key.Enter && textBox != null)
            {
                string propertyName = (string)textBox.Tag;

                if (propertyName == "Name")
                {
                    try
                    {
                        textBox.Text = textBox.Text.Replace(" ", "");
                        //while (textBox.Text.Contains(' '))
                        //{
                        //    textBox.Text = textBox.Text.Remove(textBox.Text.IndexOf(' '), 1);
                        //}

                        if (textBox.Text.Length > 0)
                        {
                            bool flag = true;
                            while (flag)
                            {
                                if (Char.IsDigit(textBox.Text[0]))
                                    textBox.Text = textBox.Text.Remove(0, 1);
                                else
                                    flag = false;
                            }
                            this.ModelItem.Properties[propertyName].SetValue(textBox.Text);
                        }

                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("The specified name could not be set");
                        textBox.SetValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
                    }
                }
                else
                {
                    this.ModelItem.Properties[propertyName].SetValue(textBox.Text);
                }
                e.Handled = true;
                // textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }

        }

        /// <summary>
        /// Handles the LostFocus event of the TextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {

            TextBox textBox = sender as TextBox;
            string propertyName = (string)textBox.Tag;

            if (propertyName == "Name")
            {
                try
                {
                    textBox.Text = textBox.Text.Replace(" ", "");
                    //while (textBox.Text.Contains(' '))
                    //{
                    //    textBox.Text = textBox.Text.Remove(textBox.Text.IndexOf(' '), 1);
                    //}

                    if (textBox.Text.Length > 0)
                    {
                        bool flag = true;
                        while (flag)
                        {
                            if (Char.IsDigit(textBox.Text[0]))
                                textBox.Text = textBox.Text.Remove(0, 1);
                            else
                                flag = false;
                        }
                        this.ModelItem.Properties[propertyName].SetValue(textBox.Text);
                    }

                }
                catch (Exception ex)
                {
                    //MessageBox.Show("The specified name could not be set");
                    textBox.SetValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
                }
            }
            else
            {
                this.ModelItem.Properties[propertyName].SetValue(textBox.Text);
                textBox.SetValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
            }

        }

        /// <summary>
        /// Handles the KeyDown event of the NumericTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void NumericTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = sender as TextBox;
                string propertyName = (string)textBox.Tag;
                try
                {
                    var temp = double.Parse(textBox.Text);
                    this.ModelItem.Properties[propertyName].SetValue(temp);
                }
                catch (Exception mess)
                {
                    textBox.SetValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());

                }
            }
        }

        /// <summary>
        /// Handles the LostFocus event of the NumericTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void NumericTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string propertyName = (string)textBox.Tag;
            try
            {
                var temp = double.Parse(textBox.Text);
                this.ModelItem.Properties[propertyName].SetValue(temp);
            }
            catch (Exception mess)
            {
                textBox.SetValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());

            }
        }

        /// <summary>
        /// Handles the KeyDown event of the NumericTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void NumericTextBoxNull_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = sender as TextBox;
                string propertyName = (string)textBox.Tag;
                try
                {
                    var temp = double.Parse(textBox.Text);
                    this.ModelItem.Properties[propertyName].SetValue(temp);

                }
                catch (Exception mess)
                {
                    textBox.SetValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());

                }
            }
        }

        /// <summary>
        /// Handles the LostFocus event of the NumericTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void NumericTextBoxNull_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string propertyName = (string)textBox.Tag;
            try
            {

                var temp = double.Parse(textBox.Text);
                this.ModelItem.Properties[propertyName].SetValue(temp);

            }
            catch (Exception mess)
            {
                textBox.SetValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());

            }
        }

        /// <summary>
        /// Handles the KeyDown event of the IntNumericTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void IntNumericTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = sender as TextBox;
                string propertyName = (string)textBox.Tag;
                try
                {
                    var temp = Int32.Parse(textBox.Text);
                    this.ModelItem.Properties[propertyName].SetValue(temp);
                }
                catch (Exception mess)
                {
                    //MessageBox.Show(mess.Message);
                    textBox.SetValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
                }
            }
        }

        /// <summary>
        /// Handles the KeyDown event of the IntNumericTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void IntNumericTextBoxNull_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = sender as TextBox;
                string propertyName = (string)textBox.Tag;
                try
                {
                    var temp = Int32.Parse(textBox.Text);
                    this.ModelItem.Properties[propertyName].SetValue(temp);

                }
                catch (Exception mess)
                {
                    //MessageBox.Show(mess.Message);
                    textBox.SetValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
                }
            }
        }

        /// <summary>
        /// Handles the LostFocus event of the IntNumericTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void IntNumericTextBoxNull_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string propertyName = (string)textBox.Tag;
            try
            {

                var temp = Int32.Parse(textBox.Text);
                this.ModelItem.Properties[propertyName].SetValue(temp);

            }
            catch (Exception mess)
            {
                //MessageBox.Show(mess.Message);
                textBox.SetValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
            }
        }

        /// <summary>
        /// Handles the LostFocus event of the IntNumericTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void IntNumericTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string propertyName = (string)textBox.Tag;
            try
            {
                var temp = Int32.Parse(textBox.Text);
                this.ModelItem.Properties[propertyName].SetValue(temp);
            }
            catch (Exception mess)
            {
                //MessageBox.Show(mess.Message);

                textBox.SetValue(TextBox.TextProperty, this.ModelItem.Properties[propertyName].ComputedValue.ToString());
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the Selector control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void Selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Selector selector = sender as Selector;
            string propertyName = (string)selector.Tag;
            try
            {
                this.ModelItem.Properties[propertyName].SetValue(selector.SelectedValue);
            }
            catch (Exception mess)
            {
                //MessageBox.Show(mess.Message);
            }
        }

        /// <summary>
        /// Called when [checked changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnCheckedChanged(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            string propertyName = (string)checkBox.Tag;
            try
            {
                this.ModelItem.Properties[propertyName].SetValue(checkBox.IsChecked);
            }
            catch (Exception mess)
            {
                //MessageBox.Show(mess.Message);
            }
        }

        /// <summary>
        /// Adds the child control.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="itemType">Type of the item.</param>
        protected void AddChildControl(ModelProperty property, Type itemType)
        {
            ModelItem item = ModelFactory.CreateItem(this.Context, itemType, new object[0]);

            if (property.IsCollection)
            {
                property.Collection.Add(item);
            }
            else
            {
                property.SetValue(item);
            }
        }

        /// <summary>
        /// Adds the child control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        public void AddChildControl(object sender, RoutedEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            AddChildControl(this.ModelItem.Properties["Items"], hyperlink.Tag as Type);
        }

        /// <summary>
        /// Adds the chart area.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        public void AddChartArea(object sender, RoutedEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            AddChildControl(this.ModelItem.Properties["Areas"], hyperlink.Tag as Type);
        }

        /// <summary>
        /// Adds the chart legend.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        public void AddChartLegend(object sender, RoutedEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            AddChildControl(this.ModelItem.Properties["Legends"], hyperlink.Tag as Type);
        }

        /// <summary>
        /// Adds the chart series.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        public void AddChartSeries(object sender, RoutedEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            AddChildControl(this.ModelItem.Properties["Series"], hyperlink.Tag as Type);
        }

        /// <summary>
        /// Adds the chart area legend.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        public void AddChartAreaLegend(object sender, RoutedEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            AddChildControl(this.ModelItem.Properties["Legend"], hyperlink.Tag as Type);
        }

        /// <summary>
        /// Start the URL link in default browser.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Navigation.RequestNavigateEventArgs"/> instance containing the event data.</param>
        public void OnlineDocumentation_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        /// <summary>
        /// Fours the point converter.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <returns></returns>
        internal string FourPointConverter(string inputString)
        {
            string expectedString = inputString;
            expectedString += ",";
            string computedString = string.Empty;
            int commaCount = 0;
            int dotCount = 0;
            double[] cornerRad = new double[4] { 0, 0, 0, 0 };
            for (int i = 0; i < expectedString.Length; i++)
            {
                if (Char.IsDigit(expectedString[i]) == true || expectedString[i] == '.' && dotCount < 1)
                {
                    computedString += expectedString[i];

                    if (expectedString[i] == '.')
                        dotCount++;
                }

                if (expectedString[i] == ',' && commaCount < 4)
                {
                    cornerRad[commaCount] = Convert.ToDouble(computedString);
                    computedString = string.Empty;
                    commaCount++;
                    dotCount = 0;
                }
            }

            string computedCornerRadius = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                if (i == 0)
                    computedCornerRadius += cornerRad[i];
                else
                    computedCornerRadius += "," + cornerRad[i];
            }

            return computedCornerRadius;
        }

        #endregion
    }

    /// <summary>
    /// Class for Model Item to be computed
    /// </summary>
    public class ModelItemToComputedValueConverter : IValueConverter
    {
        
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ModelItem)
            {
                ModelItem item = value as ModelItem;

                if (item.Source != null)
                {
                    return item.Source.ComputedValue;
                }
            }
            return value;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}
