// <copyright file="CloneManager.cs" company="Syncfusion">
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
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using Syncfusion.Windows.Shared;
using System.Windows.Documents;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Clone Manager
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class CloneManager
    {
        #region Implementation

        /// <summary>
        /// Clones the general.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="needsSizing">if set to <c>true</c> [needs sizing].</param>
        /// <returns>Clones the general</returns>
        public static UIElement CloneGeneral(UIElement target, bool needsSizing)
        {
            UIElement cloned;
            if (target is FrameworkElement)
            {
                (target as FrameworkElement).ContextMenu = null;
            }

            if (target is RibbonButton)
            {
                cloned = CloneRibbonButton(target as RibbonButton);
            }
            else if (target is SimpleMenuButton)
            {
                cloned = CloneSimpleMenuButtonAsButton(target as SimpleMenuButton);
            }
            else if (target is MenuButton)
            {
                cloned = CloneMenuButton(target as MenuButton);
            }
            else if (target is SplitMenuButton)
            {
                cloned = CloneSplitMenuButton(target as SplitMenuButton);
            }
            else if (target is DropDownButton)
            {
                if (target is SplitButton)
                {
                    cloned = CloneSplitButton(target as SplitButton);
                }
                else
                {
                    cloned = CloneDropDownButton(target as DropDownButton);
                }
            }
            else if (target is RibbonGallery)
            {
                cloned = CloneRibbonGallery(target as RibbonGallery);
                (cloned as RibbonGallery).VisualMode = RibbonGalleryVisualMode.DropDown;
                (cloned as RibbonGallery).SizeForm = SizeForm.ExtraSmall;
                (cloned as RibbonGallery).Height = double.NaN;
                (cloned as RibbonGallery).Width = double.NaN;
            }
            else if (target is RibbonBar)
            {
                cloned = CloneRibbonBar(target as RibbonBar);
                needsSizing = false;
            }
            else if (target is RibbonComboBox)
            {
                cloned = CloneRibbonComboBox(target as RibbonComboBox);
            }
            else if (target is RibbonMenuItem)
            {
                cloned = CloneRibbonMenuItem(target as RibbonMenuItem);
            }
            else if (target is RibbonCheckBox)
            {
                cloned = CloneRibbonCheckBox(target as RibbonCheckBox);
            }
            else if (target is RibbonRadioButton)
            {
                cloned = CloneRibbonRadioButton(target as RibbonRadioButton);
            }
            else if (target is RibbonToggleButton)
            {
                cloned = CloneRibbonToggleButton(target as RibbonToggleButton);
            }
            else
            {
                cloned = CloneBasic(target);
            }

            if (needsSizing && cloned is ICollapsable)
            {
                ICollapsable collapsable = cloned as ICollapsable;
                collapsable.SizeForm = SizeForm.ExtraSmall;

                if (collapsable.SmallIcon == null && collapsable.LargeIcon != null)
                {
                    if (collapsable is RibbonButton)
                    {
                        (collapsable as RibbonButton).SmallIcon = collapsable.LargeIcon;
                    }
                    else if (collapsable is DropDownButton)
                    {
                        (collapsable as DropDownButton).SmallIcon = collapsable.LargeIcon;
                    }
                }
            }

            return cloned;
        }

        /// <summary>
        /// Clones the split button.
        /// </summary>
        /// <param name="splitButton">The split button.</param>
        /// <returns>Cloned the split button.</returns>
        private static UIElement CloneSplitButton(SplitButton splitButton)
        {
            SplitButton cloned = new SplitButton();
            RibbonCommandManager.SetSynchronizedItem(cloned, RibbonCommandManager.GetSynchronizedItem(splitButton));
            cloned.SizeForm = splitButton.SizeForm;
            if (splitButton.ItemsSource == null)
                CloneItemCollection(splitButton.Items, cloned.Items);
            object obj = Ribbon.GetRibbonQATCommandTag(splitButton);
            Ribbon.SetRibbonQATCommandTag(cloned, obj);
            CloneSplitButtonBinding(splitButton, cloned, "SmallIcon", SplitButton.SmallIconProperty);
            CloneSplitButtonBinding(splitButton, cloned, "LargeIcon", SplitButton.LargeIconProperty);
            CloneSplitButtonBinding(splitButton, cloned, "IsGroup", SplitButton.IsGroupProperty);
            CloneSplitButtonBinding(splitButton, cloned, "IsLargeImageVisible", SplitButton.IsLargeImageVisibleProperty);
            CloneSplitButtonBinding(splitButton, cloned, "ToolTip", SplitButton.ToolTipProperty);
            CloneSplitButtonBinding(splitButton, cloned, "Label", SplitButton.LabelProperty);
            CloneSplitButtonBinding(splitButton, cloned, "CommandParameter", SplitButton.CommandParameterProperty);
            CloneSplitButtonBinding(splitButton, cloned, "CommandTarget", SplitButton.CommandTargetProperty);
            CloneSplitButtonBinding(splitButton, cloned, "Command", SplitButton.CommandProperty);
            if (splitButton.ItemsSource != null)
            {
                Binding binding = new Binding();
                binding.Path = new PropertyPath("ItemsSource");
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                binding.Source = splitButton;
                binding.Mode = BindingMode.OneTime;
                cloned.SetBinding(SplitButton.ItemsSourceProperty, binding);

                CloneSplitButtonBinding(splitButton, cloned, "ItemContainerStyle", SplitButton.ItemContainerStyleProperty);
            }

            return cloned;
        }

        /// <summary>
        /// Clones the split button binding.
        /// </summary>
        /// <param name="sourceButton">The source button.</param>
        /// <param name="clonedButton">The cloned button.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="depProperty">The dep property.</param>
        private static void CloneSplitButtonBinding(SplitButton sourceButton, SplitButton clonedButton, string propertyName, DependencyProperty depProperty)
        {
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = sourceButton;
            binding.Mode = BindingMode.TwoWay;
            clonedButton.SetBinding(depProperty, binding);
        }

        /// <summary>
        /// Clones the split menu button.
        /// </summary>
        /// <param name="splitMenuButton">The split menu button.</param>
        /// <returns>cloned split menu button</returns>
        private static UIElement CloneSplitMenuButton(SplitMenuButton splitMenuButton)
        {
            SplitButton cloned = new SplitButton();
            object obj = Ribbon.GetRibbonQATCommandTag(splitMenuButton);
            Ribbon.SetRibbonQATCommandTag(cloned, obj);
            cloned.SizeForm = SizeForm.ExtraSmall;
            if (splitMenuButton.SmallIcon != null)
            {
                cloned.SmallIcon = splitMenuButton.SmallIcon;
            }
            cloned.Label = splitMenuButton.Label;
            cloned.ToolTip = splitMenuButton.ToolTip;
            CloneManager.CloneEventHandler(splitMenuButton, cloned, CommandManager.CanExecuteEvent);
            CloneManager.CloneEventHandler(splitMenuButton, cloned, CommandManager.ExecutedEvent);
            cloned.CommandParameter = splitMenuButton.CommandParameter;
            cloned.CommandTarget = splitMenuButton.CommandTarget;
            cloned.Command = splitMenuButton.Command;
            RibbonCommandManager.SetSynchronizedItem(cloned, RibbonCommandManager.GetSynchronizedItem(splitMenuButton));            
            CloneItemCollection(splitMenuButton.Items, cloned.Items);
            return cloned;
        }
     
        /// <summary>
        /// Clones the ApplicationMenuBar.
        /// </summary>
        /// <param name="appgroup"></param>
        /// <returns></returns>
        private static UIElement CloneApplicationMenuBar(ApplicationMenuGroup appgroup)
        {
            ApplicationMenuGroup cloned = new ApplicationMenuGroup();
            cloned.Header = appgroup.Header;
            cloned.ToolTip = appgroup.ToolTip;
            
            cloned.HeaderStringFormat = appgroup.HeaderStringFormat;
            cloned.HeaderTemplate = appgroup.HeaderTemplate;
            cloned.HeaderTemplateSelector = appgroup.HeaderTemplateSelector;
            CloneItemCollection(appgroup.Items, cloned.Items);
            
            return cloned;
        }

        /// <summary>
        /// Clones the menu button.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>cloned the menu button</returns>
        private static UIElement CloneMenuButton(MenuButton target)
        {
            DropDownButton button = new DropDownButton();
            button.SizeForm = SizeForm.ExtraSmall;
            button.SmallIcon = target.Icon;
            button.Label = target.Label;
            button.ToolTip = target.ToolTip;
            CloneItemCollection(target.Items, button.Items);

            return button;
        }

        /// <summary>
        /// Clones the Ribbon Button.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>clones the basic ribbon Button</returns>
        public static UIElement CloneBasicRibbonButton(UIElement element)
        {
            RibbonButton clonedButton = new RibbonButton();
            if (element is FrameworkElement)
            {
                clonedButton.ToolTip = (element as FrameworkElement).ToolTip;
            }
            object obj = Ribbon.GetRibbonQATCommandTag(element);
            Ribbon.SetRibbonQATCommandTag(clonedButton, obj);

            //Syncfusion.Windows.Shared.DictionaryList styleList = SkinStorage.GetVisualStylesList(element);
            //if (styleList != null)
            //{
            //    RibbonSkinObjectExtension ext = new RibbonSkinObjectExtension();
            //    SkinStorage.SetVisualStylesList(clonedButton, styleList);
            //}

            if (clonedButton != null)
            {
                CloneManager.CloneEventHandler(element, clonedButton, Button.ClickEvent);
            }

            if (element is ICommandSource && (element as ICommandSource).Command != null)
            {
                ICommandSource commandSource = element as ICommandSource;
                clonedButton.CommandTarget = commandSource.CommandTarget;
                CloneManager.CloneEventHandler(element, clonedButton, CommandManager.CanExecuteEvent);
                CloneManager.CloneEventHandler(element, clonedButton, CommandManager.ExecutedEvent);

                if (commandSource.CommandParameter != null)
                {
                    clonedButton.CommandParameter = commandSource.CommandParameter;
                }
                clonedButton.Command = commandSource.Command;
            }

            return clonedButton as UIElement;
        }

        /// <summary>
        /// clone objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
         public static T Clone<T>(T source)

        {
            if (source.GetType().Name != "BitmapFrameDecode")
            {
                T cloned = (T)Activator.CreateInstance(source.GetType());
                foreach (PropertyInfo curPropInfo in source.GetType().GetProperties())
                {
                    if (curPropInfo.GetGetMethod() != null && (curPropInfo.GetSetMethod() != null))
                    {
                        // Handle Non-indexer properties
                        if (curPropInfo.Name != "Item")
                        {
                            // get property from source

                            object getValue = curPropInfo.GetGetMethod().Invoke(source, new object[] { });
                            // clone if needed
                            if (getValue != null && getValue is DependencyObject || getValue is bool)
                            {
                                if (getValue is DependencyObject && getValue.GetType().ToString() != "Syncfusion.Windows.Tools.Controls.ColorPickerPalette")
                                    getValue = Clone((DependencyObject)getValue);
                                // set property on cloned
                                if (getValue != null)
                                    curPropInfo.GetSetMethod().Invoke(cloned, new object[] { getValue });
                            }
                            else
                            {
                                if (getValue != null && !(getValue is TextPointer))
                                    curPropInfo.GetSetMethod().Invoke(cloned, new object[] { getValue });
                            }
                        }
                            // handle indexer
                        else
                        {
                            // get count for indexer
                            int numberofItemInColleciton =(int)curPropInfo.ReflectedType.GetProperty("Count").GetGetMethod().Invoke(source, new object[] { });
                            // run on indexer
                            for (int i = 0; i < numberofItemInColleciton; i++)
                            {
                                // get item through Indexer
                                object getValue = curPropInfo.GetGetMethod().Invoke(source, new object[] { i });
                                // clone if needed
                                if (getValue != null && getValue is DependencyObject)
                                    getValue = Clone((DependencyObject)getValue);
                                // add item to collection
                                curPropInfo.ReflectedType.GetMethod("Add").Invoke(cloned, new object[] { getValue });

                            }

                        }

                    }

                }
                return cloned;
            }
            else
            {
                return default(T);
            }

        }
        
        /// <summary>
        /// Clones the basic.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>clones the basic</returns>
        public static UIElement CloneBasic(UIElement element)
        {
            UIElement cloned;
            Object depObj1 = element as DependencyObject;
            if (depObj1.GetType().Name == "RibbonGalleryItem")
            {
                StringReader strReader = new StringReader(XamlWriter.Save(element));
                XmlReader xmlReader = XmlReader.Create(strReader);

                cloned = XamlReader.Load(xmlReader) as UIElement;
            }

            else
            {
                if (element is UserControl)
                {
                    StringReader strReader = new StringReader(XamlWriter.Save(element));
                    XmlReader xmlReader = XmlReader.Create(strReader);

                    cloned = XamlReader.Load(xmlReader) as UIElement;
                }
                else
                {
                    object s = Clone(depObj1);
                    cloned = (UIElement)s;
                }
            }
          
            ButtonBase clonedButton = cloned as ButtonBase;
            if (clonedButton != null && element is FrameworkElement)
            {
                
                clonedButton.ToolTip = (element as FrameworkElement).ToolTip;
            }

            //Syncfusion.Windows.Shared.DictionaryList styleList = SkinStorage.GetVisualStylesList(element);
            //if (styleList != null)
            //{
            //    RibbonSkinObjectExtension ext = new RibbonSkinObjectExtension();
            //    SkinStorage.SetVisualStylesList(cloned, styleList);
            //}

            
            if (clonedButton != null)
            {
                CloneManager.CloneEventHandler(element, cloned, Button.ClickEvent);
                
            }
            else if (element is MenuItem)
            {
                CloneManager.CloneEventHandler(element, cloned, MenuItem.ClickEvent);
                if (cloned is FrameworkElement)
                {
                    (cloned as FrameworkElement).ToolTip = (element as FrameworkElement).ToolTip;
                }
            }
            else if (element is ComboBox)
            {
                //BindingUtils.SetBinding(element, cloned, ComboBox.SelectedIndexProperty, ComboBox.SelectedIndexProperty);
                CloneComboBoxBinding(element as ComboBox ,cloned as ComboBox  , "SelectedIndex", ComboBox.SelectedIndexProperty);
            }

            if (element is ToggleButton)
            {
                CloneToggleButtonBinding(element as ToggleButton, cloned as ToggleButton, "IsChecked", ToggleButton.IsCheckedProperty);
                //BindingUtils.SetBinding(element, clonedButton, ToggleButton.IsCheckedProperty, ToggleButton.IsCheckedProperty);
            }


            if (element is ICommandSource && (element as ICommandSource).Command != null)
            {
                ICommandSource commandSource = element as ICommandSource;
                if (clonedButton == null && cloned is MenuItem)
                {
                    MenuItem clonedMenuItem = cloned as MenuItem;
                    clonedMenuItem.CommandTarget = commandSource.CommandTarget;
                    CloneManager.CloneEventHandler(element, clonedButton, CommandManager.CanExecuteEvent);
                    CloneManager.CloneEventHandler(element, clonedButton, CommandManager.ExecutedEvent);

                    if (commandSource.CommandParameter != null)
                    {
                        clonedMenuItem.CommandParameter = commandSource.CommandParameter;
                    }
                    clonedMenuItem.Command = commandSource.Command;
                }
                else
                {
                    clonedButton.CommandTarget = commandSource.CommandTarget;
                    CloneManager.CloneEventHandler(element, clonedButton, CommandManager.CanExecuteEvent);
                    CloneManager.CloneEventHandler(element, clonedButton, CommandManager.ExecutedEvent);

                    if (commandSource.CommandParameter != null)
                    {
                        clonedButton.CommandParameter = commandSource.CommandParameter;
                    }
                    clonedButton.Command = commandSource.Command;
                }
            }

            return cloned as UIElement;
        }

        /// <summary>
        /// Clones the ribbon button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>cloned the ribbon button</returns>
        public static RibbonButton CloneRibbonButton(RibbonButton button)
        {
            RibbonButton cloned = CloneBasicRibbonButton(button) as RibbonButton;

            cloned.SizeForm = button.SizeForm;

            RibbonCommandManager.SetSynchronizedItem(cloned, RibbonCommandManager.GetSynchronizedItem(button));

            CloneRibbonButtonBinding(button, cloned, "CornerRadius", RibbonButton.CornerRadiusProperty );
            CloneRibbonButtonBinding(button, cloned, button is BackStageCommandButton ? "Header" : "Label", RibbonButton.LabelProperty);
            CloneRibbonButtonBinding(button, cloned, "LargeIcon", RibbonButton.LargeIconProperty);

            CloneRibbonButtonBinding(button, cloned, "IsSelected", RibbonButton.IsSelectedProperty);
            CloneRibbonButtonBinding(button, cloned, "IsToggle", RibbonButton.IsToggleProperty);
            CloneRibbonButtonBinding(button, cloned, button is BackStageCommandButton ? "Icon" : "SmallIcon", RibbonButton.SmallIconProperty);
            CloneRibbonButtonBinding(button, cloned, "ToolTip", RibbonButton.ToolTipProperty);

            object obj = Ribbon.GetRibbonQATCommandTag(button);
            Ribbon.SetRibbonQATCommandTag(cloned, obj);
            if (cloned.SmallIcon == null && button.LargeIcon != null)
            {
                cloned.SmallIcon = button.LargeIcon;
            }

            string[] eventNames = null;
            Delegate[] eventHandlers = null;
            button.GetEventHandlers(ref eventNames, ref eventHandlers);
            if (eventHandlers != null && eventNames != null)
            {
                foreach (string eventName in eventNames)
                {
                    if (eventName != null)
                    {
                        foreach (Delegate handler in eventHandlers)
                        {
                            bool flag = false;
                            if (eventName != null)
                            {
                                switch (eventName.Split(' ')[0])
                                {
                                    case "IsSelectedChanged":
                                        if (eventName.Split(' ')[1].Equals(handler.Method.Name))
                                        {
                                            cloned.IsSelectedChanged += (PropertyChangedCallback)handler;
                                            flag = true;
                                        }

                                        break;
                                    case "IsToggleChanged":
                                        if (eventName.Split(' ')[1].Equals(handler.Method.Name))
                                        {
                                            cloned.IsToggleChanged += (PropertyChangedCallback)handler;
                                            flag = true;
                                        }

                                        break;
                                    case "LabelChanged":
                                        if (eventName.Split(' ')[1].Equals(handler.Method.Name))
                                        {
                                            cloned.LabelChanged += (PropertyChangedCallback)handler;
                                            flag = true;
                                        }

                                        break;
                                    case "LargeIconChanged":
                                        if (eventName.Split(' ')[1].Equals(handler.Method.Name))
                                        {
                                            cloned.LargeIconChanged += (PropertyChangedCallback)handler;
                                            flag = true;
                                        }

                                        break;
                                    case "SizeFormChanged":
                                        if (eventName.Split(' ')[1].Equals(handler.Method.Name))
                                        {
                                            cloned.SizeFormChanged += (PropertyChangedCallback)handler;
                                            flag = true;
                                        }

                                        break;
                                }

                                if (flag)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return cloned;
        }

        /// <summary>
        /// Clones the ribbon button binding.
        /// </summary>
        /// <param name="sourceButton">The source button.</param>
        /// <param name="clonedButton">The cloned button.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="depProperty">The dep property.</param>
        private static void CloneRibbonButtonBinding(RibbonButton sourceButton, RibbonButton clonedButton, string propertyName, DependencyProperty depProperty)
        {
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = sourceButton;
            binding.Mode = BindingMode.TwoWay;
            clonedButton.SetBinding(depProperty, binding);
        }

        /// <summary>
        /// Clones the ribbon check box binding.
        /// </summary>
        /// <param name="sourcecheckbox">The sourcecheckbox.</param>
        /// <param name="clonedcheckbox">The clonedcheckbox.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="depProperty">The dep property.</param>
        private static void CloneRibbonCheckBoxBinding(RibbonCheckBox sourcecheckbox, RibbonCheckBox clonedcheckbox, string propertyName, DependencyProperty depProperty)
        {

            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = sourcecheckbox;
            binding.Mode = BindingMode.TwoWay;
            clonedcheckbox.SetBinding(depProperty, binding);
         
        }

        /// <summary>
        /// Clones the ribbon radio button binding.
        /// </summary>
        /// <param name="sourceradiobutton">The sourceradiobutton.</param>
        /// <param name="clonedradiobutton">The clonedradiobutton.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="depProperty">The dep property.</param>
        private static void CloneRibbonRadioButtonBinding(RibbonRadioButton sourceradiobutton, RibbonRadioButton clonedradiobutton, string propertyName, DependencyProperty depProperty)
        {
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = sourceradiobutton;
            binding.Mode = BindingMode.TwoWay;
            clonedradiobutton.SetBinding(depProperty, binding);
        }

        /// <summary>
        /// Clones the toggle button binding.
        /// </summary>
        /// <param name="sourcetogglebutton">The sourcetogglebutton.</param>
        /// <param name="clonedtogglebutton">The clonedtogglebutton.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="depProperty">The dep property.</param>
        private static void CloneToggleButtonBinding(ToggleButton  sourcetogglebutton, ToggleButton  clonedtogglebutton, string propertyName, DependencyProperty depProperty)
        {

            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = sourcetogglebutton;
            binding.Mode = BindingMode.TwoWay;
            clonedtogglebutton.SetBinding(depProperty, binding);

        }

        private static void CloneRibbonToggleButtonBinding(RibbonToggleButton sourcetogglebutton, RibbonToggleButton clonedtogglebutton, string propertyName, DependencyProperty depProperty)
        {
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = sourcetogglebutton;
            binding.Mode = BindingMode.TwoWay;
            clonedtogglebutton.SetBinding(depProperty, binding);
        }

        /// <summary>
        /// Clones the combo box binding.
        /// </summary>
        /// <param name="sourcecombobox">The sourcecombobox.</param>
        /// <param name="clonedcombobox">The clonedcombobox.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="depProperty">The dep property.</param>
        private static void CloneComboBoxBinding(ComboBox  sourcecombobox, ComboBox  clonedcombobox, string propertyName, DependencyProperty depProperty)
        {

            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = sourcecombobox;
            binding.Mode = BindingMode.TwoWay;
            clonedcombobox.SetBinding(depProperty, binding);
        }


        /// <summary>
        /// Clones the ribbon combo box binding.
        /// </summary>
        /// <param name="sourceribboncombobox">The sourceribboncombobox.</param>
        /// <param name="clonedribboncombobox">The clonedribboncombobox.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="depProperty">The dep property.</param>
        private static void CloneRibbonComboBoxBinding(RibbonComboBox  sourceribboncombobox, RibbonComboBox  clonedribboncombobox, string propertyName, DependencyProperty depProperty)
        {

            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = sourceribboncombobox;
            binding.Mode = BindingMode.TwoWay;
            clonedribboncombobox.SetBinding(depProperty, binding);
        }

        /// <summary>
        /// Clones the drop down button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>cloned the drop down button</returns>
        private static DropDownButton CloneDropDownButton(DropDownButton button)
        {
            DropDownButton cloned = new DropDownButton();
            cloned.SizeForm = button.SizeForm;
            object obj = Ribbon.GetRibbonQATCommandTag(button);
            Ribbon.SetRibbonQATCommandTag(cloned, obj);
            RibbonCommandManager.SetSynchronizedItem(cloned, RibbonCommandManager.GetSynchronizedItem(button));

            CloneDropDownButtonBinding(button, cloned, "LargeIcon", DropDownButton.LargeIconProperty);
            CloneDropDownButtonBinding(button, cloned, "Label", DropDownButton.LabelProperty);

            CloneDropDownButtonBinding(button, cloned, "SmallIcon", DropDownButton.SmallIconProperty);
            CloneDropDownButtonBinding(button, cloned, "IsGroup", DropDownButton.IsGroupProperty);
            CloneDropDownButtonBinding(button, cloned, "ToolTip", DropDownButton.ToolTipProperty);
            CloneItemCollection(button.Items, cloned.Items);

            if (cloned.SmallIcon == null && button.LargeIcon != null)
            {
                cloned.SmallIcon = button.LargeIcon;
            }

            return cloned;
        }

        /// <summary>
        /// Clones the ribbon check box.
        /// </summary>
        /// <param name="checkBox">The check box.</param>
        /// <returns></returns>
        private static RibbonCheckBox CloneRibbonCheckBox(RibbonCheckBox checkBox)
        {
            RibbonCheckBox cloned = new RibbonCheckBox();

            //cloned.SizeForm = SizeForm.ExtraSmall;
           
            CloneManager.CloneEventHandler(checkBox, cloned, CommandManager.CanExecuteEvent);
            CloneManager.CloneEventHandler(checkBox, cloned, CommandManager.ExecutedEvent);

            CloneManager.CloneEventHandler(checkBox, cloned, RibbonCheckBox.UncheckedEvent);
            CloneManager.CloneEventHandler(checkBox, cloned, RibbonCheckBox.CheckedEvent);
            CloneManager.CloneEventHandler(checkBox, cloned, RibbonCheckBox.ClickEvent);

            CloneRibbonCheckBoxBinding(checkBox, cloned, "SmallIcon", RibbonCheckBox.SmallIconProperty);
            CloneRibbonCheckBoxBinding(checkBox, cloned, "Content", RibbonCheckBox.ContentProperty);
            CloneRibbonCheckBoxBinding(checkBox, cloned, "ToolTip", RibbonCheckBox.ToolTipProperty);

            CloneRibbonCheckBoxBinding(checkBox, cloned, "IsChecked", RibbonCheckBox.IsCheckedProperty);
            CloneRibbonCheckBoxBinding(checkBox, cloned, "CommandParameter", RibbonCheckBox.CommandParameterProperty);
            CloneRibbonCheckBoxBinding(checkBox, cloned, "CommandTarget", RibbonCheckBox.CommandTargetProperty);
            cloned.Command = checkBox.Command;
            //BindingExpression bindingExp;

            //bindingExp = checkBox.GetBindingExpression(RibbonCheckBox.IsCheckedProperty);
            //if (bindingExp != null && bindingExp.DataItem is RibbonCheckBox)
            //    BindingUtils.SetBinding((RibbonCheckBox)bindingExp.DataItem, cloned, RibbonCheckBox.IsCheckedProperty, RibbonCheckBox.IsCheckedProperty,BindingMode.TwoWay );

            //bindingExp = checkBox.GetBindingExpression(RibbonCheckBox.CommandParameterProperty);
            //if (bindingExp != null && bindingExp.DataItem is RibbonCheckBox)
            //    BindingUtils.SetBinding((RibbonCheckBox)bindingExp.DataItem, cloned, RibbonCheckBox.CommandParameterProperty, RibbonCheckBox.CommandParameterProperty);



            //bindingExp = checkBox.GetBindingExpression(RibbonCheckBox.CommandTargetProperty);
            //if (bindingExp != null && bindingExp.DataItem is RibbonCheckBox)
            //    BindingUtils.SetBinding((RibbonCheckBox)bindingExp.DataItem, cloned, RibbonCheckBox.CommandTargetProperty, RibbonCheckBox.CommandTargetProperty,BindingMode.TwoWay );

            //BindingUtils.SetBinding(checkBox, cloned, RibbonCheckBox.IsCheckedProperty, RibbonCheckBox.IsCheckedProperty, BindingMode.TwoWay);

            //BindingUtils.SetBinding(checkBox, cloned, RibbonCheckBox.CommandParameterProperty, RibbonCheckBox.CommandParameterProperty, BindingMode.TwoWay);
            //BindingUtils.SetBinding(checkBox, cloned, RibbonCheckBox.CommandTargetProperty, RibbonCheckBox.CommandTargetProperty, BindingMode.TwoWay);
                                  
            return cloned; 
        }

        /// <summary>
        /// Clones the ribbon radio button.
        /// </summary>
        /// <param name="radiobutton">The radio button.</param>
        /// <returns></returns>
        private static RibbonRadioButton CloneRibbonRadioButton(RibbonRadioButton radioButton)
        {
            RibbonRadioButton cloned = new RibbonRadioButton();

            CloneManager.CloneEventHandler(radioButton, cloned, CommandManager.CanExecuteEvent);
            CloneManager.CloneEventHandler(radioButton, cloned, CommandManager.ExecutedEvent);

            CloneManager.CloneEventHandler(radioButton, cloned, RibbonRadioButton.UncheckedEvent);
            CloneManager.CloneEventHandler(radioButton, cloned, RibbonRadioButton.CheckedEvent);
            CloneManager.CloneEventHandler(radioButton, cloned, RibbonRadioButton.ClickEvent);

            CloneRibbonRadioButtonBinding(radioButton, cloned, "SmallIcon", RibbonRadioButton.SmallIconProperty);
            CloneRibbonRadioButtonBinding(radioButton, cloned, "Content", RibbonRadioButton.ContentProperty);
            CloneRibbonRadioButtonBinding(radioButton, cloned, "ToolTip", RibbonRadioButton.ToolTipProperty);

            CloneRibbonRadioButtonBinding(radioButton, cloned, "IsChecked", RibbonRadioButton.IsCheckedProperty);
            CloneRibbonRadioButtonBinding(radioButton, cloned, "CommandParameter", RibbonRadioButton.CommandParameterProperty);
            CloneRibbonRadioButtonBinding(radioButton, cloned, "CommandTarget", RibbonRadioButton.CommandTargetProperty);
            cloned.Command = radioButton.Command;
            return cloned;
        }

        private static RibbonToggleButton CloneRibbonToggleButton(RibbonToggleButton sourceroggleButton)
        {
            RibbonToggleButton cloned = new RibbonToggleButton();

            CloneRibbonToggleButtonBinding(sourceroggleButton, cloned, "Label", RibbonToggleButton.LabelProperty);
            CloneRibbonToggleButtonBinding(sourceroggleButton, cloned, "LargeIcon", RibbonToggleButton.LargeIconProperty);
            CloneRibbonToggleButtonBinding(sourceroggleButton, cloned, "SmallIcon", RibbonToggleButton.SmallIconProperty);
            CloneRibbonToggleButtonBinding(sourceroggleButton, cloned, "Command", RibbonToggleButton.CommandProperty);
            CloneRibbonToggleButtonBinding(sourceroggleButton, cloned, "CommandParameter", RibbonToggleButton.CommandParameterProperty);
            CloneRibbonToggleButtonBinding(sourceroggleButton, cloned, "CommandTarget", RibbonToggleButton.CommandTargetProperty);

            return cloned;
        }

        /// <summary>
        /// Clones the drop down button binding.
        /// </summary>
        /// <param name="sourceButton">The source button.</param>
        /// <param name="clonedButton">The cloned button.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="depProperty">The dep property.</param>
        private static void CloneDropDownButtonBinding(DropDownButton sourceButton, DropDownButton clonedButton, string propertyName, DependencyProperty depProperty)
        {
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = sourceButton;
            binding.Mode = BindingMode.TwoWay;
            clonedButton.SetBinding(depProperty, binding);
        }

        /// <summary>
        /// Clones the button panel.
        /// </summary>
        /// <param name="buttonPanel">The button panel.</param>
        /// <returns>cloned the button panel</returns>
        private static ButtonPanel CloneButtonPanel(ButtonPanel buttonPanel)
        {
            ButtonPanel cloned = new ButtonPanel();
            CloneItemCollection(buttonPanel.Items, cloned.Items);
            cloned.ToolTip = buttonPanel.ToolTip;
            return cloned;
        }

        /// <summary>
        /// Clones the simple menu button.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>cloned simple menu button</returns>
        private static UIElement CloneSimpleMenuButton(SimpleMenuButton target)
        {
            ICommandSource commandSource = target as ICommandSource;
            SimpleMenuButton b = new SimpleMenuButton();
            RibbonCommandManager.SetSynchronizedItem(b, RibbonCommandManager.GetSynchronizedItem(target));
            //RibbonButton b = new RibbonButton();
            //b.SizeForm = SizeForm.ExtraSmall;
            //if (target.Icon != null)
            //{
            //    b.SmallIcon = target.Icon;
            //}
            CloneManager.CloneEventHandler(target, b, SimpleMenuButton.ClickEvent);

            if(target.Icon !=null)
                b.Icon = target.Icon;
            if(target.SmallIcon !=null)
                b.SmallIcon = target.SmallIcon;

            b.Description = target.Description;
            b.Label = target.Label;
            b.ToolTip = target.ToolTip;            
            b.Command = commandSource.Command;
            b.CommandTarget = commandSource.CommandTarget;
            CloneManager.CloneEventHandler(target, b, CommandManager.CanExecuteEvent);
            CloneManager.CloneEventHandler(target, b, CommandManager.ExecutedEvent);
            if (commandSource.CommandParameter != null)
            {
                b.CommandParameter = commandSource.CommandParameter;
            }

            return b;
        }

        /// <summary>
        /// Clones the simple menu button.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>cloned simple menu button</returns>
        private static UIElement CloneSimpleMenuButtonAsButton(SimpleMenuButton target)
        {

            ICommandSource commandSource = target as ICommandSource;
            RibbonButton b = new RibbonButton();

            RibbonCommandManager.SetSynchronizedItem(b, RibbonCommandManager.GetSynchronizedItem(target));
            b.SizeForm = SizeForm.ExtraSmall;
            if (target.SmallIcon != null)
            {
                b.SmallIcon = target.SmallIcon;
            }
            
            b.Label = target.Label;
            b.ToolTip = target.ToolTip;
            b.Command = commandSource.Command;
            b.CommandTarget = commandSource.CommandTarget;
            CloneManager.CloneEventHandler(target, b, Button.ClickEvent);
            CloneManager.CloneEventHandler(target, b, CommandManager.CanExecuteEvent);
            CloneManager.CloneEventHandler(target, b, CommandManager.ExecutedEvent);
            if (commandSource.CommandParameter != null)
            {
                b.CommandParameter = commandSource.CommandParameter;
            }

            return b;
        }

        /// <summary>
        /// Clones the simple menu button.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>cloned simple menu button</returns>
        private static UIElement CloneRibbonMenuItem(RibbonMenuItem target)
        {
            ICommandSource commandSource = target as ICommandSource;
            if (target.HasItems)
            {
                SplitButton b = new SplitButton();
                b.SizeForm = SizeForm.ExtraSmall;
                b.Label = target.Header.ToString();
                b.ToolTip = target.ToolTip;
                b.Command = commandSource.Command;
                b.CommandTarget = commandSource.CommandTarget;
                CloneManager.CloneEventHandler(target, b, CommandManager.CanExecuteEvent);
                CloneManager.CloneEventHandler(target, b, CommandManager.ExecutedEvent);
                object obj = Ribbon.GetRibbonQATCommandTag(target);
                Ribbon.SetRibbonQATCommandTag(b, obj);

                if (commandSource.CommandParameter != null)
                {
                    b.CommandParameter = commandSource.CommandParameter;
                }

                if (target.Icon != null)
                {
                    ImageSource source = target.Icon as ImageSource;
                    if (source == null && target.Icon is Image)
                    {
                        source = (target.Icon as Image).Source;
                    }

                    b.SmallIcon = source;
                }

                CloneManager.CloneEventHandler(target, b, MenuItem.ClickEvent);
                CloneItemCollection(target.Items, b.Items);

                return b;
            }
            else
            {
                RibbonButton b = new RibbonButton();
                b.SizeForm = SizeForm.ExtraSmall;
                if (target.Icon != null)
                {
                    ImageSource source = target.Icon as ImageSource;
                    if (source == null && target.Icon is Image)
                    {
                        source = (target.Icon as Image).Source;
                    }

                    b.SmallIcon = source;
                }

                if (target.Header != null && target.ToolTip != null)
                {
                    b.Label = target.Header.ToString();
                    b.ToolTip = target.ToolTip;
                }

                CloneManager.CloneEventHandler(target, b, Button.ClickEvent);
                b.Command = commandSource.Command;
                CloneManager.CloneEventHandler(target, b, CommandManager.CanExecuteEvent);
                CloneManager.CloneEventHandler(target, b, CommandManager.ExecutedEvent);
                b.CommandTarget = commandSource.CommandTarget;
                if (commandSource.CommandParameter != null)
                {
                    b.CommandParameter = commandSource.CommandParameter;
                }

                return b;
            }
        }

        /// <summary>
        /// Clones the ribbon menu item in items clone call.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>Return the RibbonMenuItem</returns>
        private static UIElement CloneRibbonMenuItemInItemsCloneCall(RibbonMenuItem target)
        {
            ICommandSource commandSource = target as ICommandSource;

            RibbonMenuItem b = new RibbonMenuItem();
            b.IconBarEnabled = target.IconBarEnabled;
            if (target.Icon != null)
            {
                ImageSource source = target.Icon as ImageSource;
                if (source != null)
                {
                    b.Icon = source;
                }
                else if (source == null && target.Icon is Image)
                {
                    source = (target.Icon as Image).Source;
                    Image img = new Image();
                    img.Source = (target.Icon as Image).Source;
                    img.Width = (target.Icon as Image).Width;
                    img.Height = (target.Icon as Image).Height;
                    img.Stretch = (target.Icon as Image).Stretch;
                    b.Icon = img;
                }
            }

            b.Header = target.Header;
            b.ToolTip = target.ToolTip;
            b.Command = commandSource.Command;
            CloneManager.CloneEventHandler(target, b, CommandManager.CanExecuteEvent);
            CloneManager.CloneEventHandler(target, b, CommandManager.ExecutedEvent);
            b.CommandTarget = commandSource.CommandTarget;
            if (commandSource.CommandParameter != null)
            {
                b.CommandParameter = commandSource.CommandParameter;
            }

            CloneManager.CloneEventHandler(target, b, MenuItem.ClickEvent);
            if (target.HasItems)
            {
                CloneItemCollection(target.Items, b.Items);
            }

            return b;
        }

        /// <summary>
        /// Clones the ribbon gallery.
        /// </summary>
        /// <param name="gallery">The gallery.</param>
        /// <returns>cloned the ribbon gallery</returns>
        private static UIElement CloneRibbonGallery(RibbonGallery gallery)
        {
            RibbonGallery cloned = new RibbonGallery();
            cloned.ToolTip = gallery.ToolTip;

            foreach (RibbonGalleryFilter filter in gallery.GalleryFilters)
            {
                RibbonGalleryFilter clonedFilter = new RibbonGalleryFilter();
                clonedFilter.Label = filter.Label;
                cloned.GalleryFilters.Add(clonedFilter);
            }

            foreach (RibbonGalleryGroup group in gallery.GalleryGroups)
            {
                RibbonGalleryGroup clonedGroup = new RibbonGalleryGroup();
                clonedGroup.Label = group.Label;
                clonedGroup.ToolTip = group.ToolTip;
                clonedGroup.SetValue(RibbonGallery.FilterIndexesProperty, group.GetValue(RibbonGallery.FilterIndexesProperty));

                foreach (UIElement item in group.Items)
                {
                    clonedGroup.Items.Add(CloneBasic(item));
                }

                cloned.GalleryGroups.Add(clonedGroup);
            }

            foreach (UIElement item in gallery.MenuItems)
            {
                cloned.MenuItems.Add(CloneGeneral(item,false));
            }

            if(gallery.Items.Count!=cloned.Items.Count)
                CloneItemCollection(gallery.Items, cloned.Items);

            if (cloned.GalleryFilters.Contains(gallery.CurrentFilter))
                cloned.CurrentFilter = gallery.CurrentFilter;
            else if(gallery.GalleryFilters.Contains(gallery.CurrentFilter) && cloned.GalleryFilters.Count > gallery.GalleryFilters.IndexOf(gallery.CurrentFilter))
            {
                cloned.CurrentFilter = cloned.GalleryFilters[gallery.GalleryFilters.IndexOf(gallery.CurrentFilter)] as RibbonGalleryFilter;
            }
            cloned.Height = gallery.ActualHeight;
            cloned.Width = gallery.ActualWidth;

            return cloned;
        }

        /// <summary>
        /// Clones the ribbon bar.
        /// </summary>
        /// <param name="bar">The bar value.</param>
        /// <returns>cloned ribbon bar</returns>
        private static UIElement CloneRibbonBar(RibbonBar bar)
        {
            RibbonBar cloned = new RibbonBar();
            CloneItemCollection(bar.Items, cloned.Items);
            CloneRibbonBarBinding(bar, cloned, "IsLargeButtonPanel", RibbonBar.IsLargeButtonPanelProperty);
            CloneRibbonBarBinding(bar, cloned, "Header", RibbonBar.HeaderProperty);
            CloneRibbonBarBinding(bar, cloned, "ToolTip", RibbonBar.ToolTipProperty);
            CloneRibbonBarBinding(bar, cloned, "CollapseImage", RibbonBar.CollapseImageProperty);
            CloneRibbonBarBinding(bar, cloned, "Visibility", RibbonBar.VisibilityProperty);
            CloneRibbonBarBinding(bar, cloned, "IsLauncherButtonVisible", RibbonBar.IsLauncherButtonVisibleProperty);
            object obj = Ribbon.GetRibbonQATCommandTag(bar);
            Ribbon.SetRibbonQATCommandTag(cloned,obj);
            CloneManager.CloneEventHandler(bar, cloned, RibbonBar.LauncherClickEvent);
            cloned.PanelState = RibbonBarState.ExtraSmall;
            return cloned;
        }

        /// <summary>
        /// Clones the ribbon bar binding.
        /// </summary>
        /// <param name="sourceBar">The source bar.</param>
        /// <param name="clonedBar">The cloned bar.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="depProperty">The dep property.</param>
        private static void CloneRibbonBarBinding(RibbonBar sourceBar, RibbonBar clonedBar, string propertyName, DependencyProperty depProperty)
        {
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = sourceBar;
            binding.Mode = BindingMode.TwoWay;
            clonedBar.SetBinding(depProperty, binding);
        }


        /// <summary>
        /// Clones the ribbon combo box.
        /// </summary>
        /// <param name="ribbonCombo">The ribbon combo.</param>
        /// <returns>return UIElement.</returns>
        private static UIElement CloneRibbonComboBox(RibbonComboBox ribbonCombo)
        {         
            RibbonComboBox comboCloned = new RibbonComboBox();
            ItemCollection comboItems = comboCloned.Items;
            object obj = Ribbon.GetRibbonQATCommandTag(ribbonCombo);
            Ribbon.SetRibbonQATCommandTag(comboCloned, obj);
            CloneRibbonComboBoxBinding(ribbonCombo, comboCloned, "Label", RibbonComboBox.LabelProperty);
            CloneRibbonComboBoxBinding(ribbonCombo, comboCloned, "SmallIcon", RibbonComboBox.SmallIconProperty);
            CloneRibbonComboBoxBinding(ribbonCombo, comboCloned, "ToolTip", RibbonComboBox.ToolTipProperty);
            CloneRibbonComboBoxBinding(ribbonCombo, comboCloned, "Width", RibbonComboBox.WidthProperty);

            if (ribbonCombo.ItemsSource != null)
            {
                IEnumerator enumerator = (ribbonCombo.ItemsSource as IEnumerable).GetEnumerator();
                RibbonComboBox ribbonItemSourceCombo = new RibbonComboBox();

                while (enumerator.MoveNext())
                {
                    ComboBoxItem comboBoxItem = new ComboBoxItem();
                    comboBoxItem.Content = enumerator.Current;
                    ribbonItemSourceCombo.Items.Add(comboBoxItem);
                }
                CloneItemCollection(ribbonItemSourceCombo.Items, comboCloned.Items);
            }
            else
            {
                CloneItemCollection(ribbonCombo.Items, comboCloned.Items);
            }
            CloneRibbonComboBoxBinding(ribbonCombo, comboCloned, "SelectedIndex", RibbonComboBox.SelectedIndexProperty);
            //BindingUtils.SetBinding(ribbonCombo, comboCloned, RibbonComboBox.SelectedIndexProperty, RibbonComboBox.SelectedIndexProperty);
            return comboCloned;
        }

        /// <summary>
        /// Clones the combo box item.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>return combobox item.</returns>
        private static ComboBoxItem CloneComboBoxItem(ComboBoxItem target)
        {
            ICommandSource commandSource = target as ICommandSource;
            ComboBoxItem comBoxItem = new ComboBoxItem();

            CloneComboBoxItemBinding(target, comBoxItem, "Content", ComboBoxItem.ContentProperty);
            CloneComboBoxItemBinding(target, comBoxItem, "IsSelected", ComboBoxItem.IsSelectedProperty);
            CloneManager.CloneEventHandler(target, comBoxItem, ComboBoxItem.PreviewMouseMoveEvent);
            CloneManager.CloneEventHandler(target, comBoxItem, ComboBoxItem.PreviewMouseLeftButtonUpEvent);
            return comBoxItem;
        }

        /// <summary>
        /// Clones the combo box item binding.
        /// </summary>
        /// <param name="sourceButton">The source button.</param>
        /// <param name="clonedButton">The cloned button.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="depProperty">The dep property.</param>
        private static void CloneComboBoxItemBinding(ComboBoxItem sourceButton, ComboBoxItem clonedButton, string propertyName, DependencyProperty depProperty)
        {
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = sourceButton;
            binding.Mode = BindingMode.TwoWay;
            clonedButton.SetBinding(depProperty, binding);
        }

        /// <summary>
        /// Clones the event handler.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="clonedElement">The cloned element.</param>
        /// <param name="cloneEvent">The clone event.</param>
        /// <returns>cloned the event handler</returns>
        internal static bool CloneEventHandler(UIElement element, UIElement clonedElement, RoutedEvent cloneEvent)
        {
            MethodInfo getRoutedEventHandlers;
            RoutedEventHandlerInfo[] routedEventHandlers;
            object value;

            Type type = element.GetType();
            MethodInfo ensureEventHandlersStore = type.GetMethod("EnsureEventHandlersStore", BindingFlags.NonPublic | BindingFlags.Instance);

            if (ensureEventHandlersStore == null)
            {
                return false;
            }

            ensureEventHandlersStore.Invoke(element, (object[])null);

            PropertyInfo eventHandlersStore = type.GetProperty("EventHandlersStore", BindingFlags.NonPublic | BindingFlags.Instance);

            if (eventHandlersStore == null)
            {
                return false;
            }

            value = eventHandlersStore.GetValue(element, (object[])null);

            if (value == null)
            {
                return false;
            }

            getRoutedEventHandlers = value.GetType().GetMethod("GetRoutedEventHandlers", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (getRoutedEventHandlers == null)
            {
                return false;
            }

            routedEventHandlers = getRoutedEventHandlers.Invoke(value, new object[] { cloneEvent }) as RoutedEventHandlerInfo[];

            if (routedEventHandlers == null)
            {
                return false;
            }

            if (routedEventHandlers.Length != 0)
            {
                RoutedEventHandlerInfo[] infoArray = routedEventHandlers;

                for (int i = 0; i < infoArray.Length; i++)
                {
                    RoutedEventHandlerInfo routedEventHandler = infoArray[i];
                    clonedElement.AddHandler(cloneEvent, routedEventHandler.Handler, routedEventHandler.InvokeHandledEventsToo);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Clones the UI element collection.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        internal static void CloneUIElementCollection(UIElementCollection input, UIElementCollection output)
        {
            IEnumerator enumerator = input.GetEnumerator();
            UIElement cloned;
            try
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is RibbonButton)
                    {
                        cloned = CloneRibbonButton(enumerator.Current as RibbonButton);
                    }
                    else if (enumerator.Current is DropDownButton)
                    {
                        cloned = CloneDropDownButton(enumerator.Current as DropDownButton);
                    }
                    else
                    {
                        cloned = CloneBasic(enumerator.Current as UIElement) as UIElement;
                    }

                    output.Add(cloned);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                while (disposable != null)
                {
                    disposable.Dispose();
                    break;
                }
            }
        }

        /// <summary>
        /// Clones the item collection.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        internal static void CloneItemCollection(ItemCollection input, ItemCollection output)
        {
            IEnumerator enumerator = (input as IEnumerable).GetEnumerator();
            object cloned=null;
            try
            {
                while (enumerator.MoveNext())
                {
                   
                    if (enumerator.Current is RibbonButton)
                    {
                        cloned = CloneRibbonButton(enumerator.Current as RibbonButton);
                    }
                    else if (enumerator.Current is SplitButton)
                    {
                        cloned = CloneSplitButton(enumerator.Current as SplitButton);
                    }
                    else if (enumerator.Current is DropDownButton)
                    {
                        cloned = CloneDropDownButton(enumerator.Current as DropDownButton);
                    }
                    else if (enumerator.Current is RibbonGallery)
                    {
                        cloned = CloneRibbonGallery(enumerator.Current as RibbonGallery);
                    }
                    else if (enumerator.Current is ButtonPanel)
                    {
                        cloned = CloneButtonPanel(enumerator.Current as ButtonPanel);
                    }
                    else if (enumerator.Current is ApplicationMenuGroup)
                    {
                        cloned = CloneApplicationMenuBar(enumerator.Current as ApplicationMenuGroup);
                    }
                    else if (enumerator.Current is RibbonMenuItem)
                    {
                        ////Do not convert menu items here. It should be done only in CloneGeneral method.                  
                        cloned = CloneRibbonMenuItemInItemsCloneCall(enumerator.Current as RibbonMenuItem);
                    }
                    else if (enumerator.Current is SimpleMenuButton)
                    {
                        cloned = CloneSimpleMenuButton(enumerator.Current as SimpleMenuButton);
                    }
                    else if (enumerator.Current is ComboBoxItem)
                    {
                        cloned = CloneComboBoxItem(enumerator.Current as ComboBoxItem);
                    }
                    else if (enumerator.Current is RibbonCheckBox)
                    {
                        cloned = CloneRibbonCheckBox(enumerator.Current as RibbonCheckBox);
                    }
                    else if (enumerator.Current is RibbonRadioButton)
                    {
                        cloned = CloneRibbonRadioButton(enumerator.Current as RibbonRadioButton);
                    }
                    else if (enumerator.Current is RibbonComboBox)
                    {
                        cloned = CloneRibbonComboBox(enumerator.Current as RibbonComboBox);
                    }
                    else
                    {
                        if (enumerator.Current is UIElement)
                        {
                            cloned = CloneBasic(enumerator.Current as UIElement) as UIElement;
                        }                        
                    }

                    if (cloned == null && enumerator.Current is UIElement)
                    {
                        cloned = CloneBasic(enumerator.Current as UIElement) as UIElement;
                        Ribbon.SetKeyTip(cloned as FrameworkElement, Ribbon.GetKeyTip(enumerator.Current as FrameworkElement));
                        output.Add(cloned);
                    }
                    else if(cloned!=null)
                    {
                        Ribbon.SetKeyTip(cloned as FrameworkElement, Ribbon.GetKeyTip(enumerator.Current as FrameworkElement));
                        output.Add(cloned);
                    }
                    
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                while (disposable != null)
                {
                    disposable.Dispose();
                    break;
                }
            }
        }

        #endregion
    }
}
