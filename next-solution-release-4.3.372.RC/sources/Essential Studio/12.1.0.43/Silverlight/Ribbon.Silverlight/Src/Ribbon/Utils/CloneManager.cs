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
using System.Reflection;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class CloneManager
    {
        private static bool isSizeMode = false;

        /// <summary>
        /// Clones the general.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="needsSizing">if set to <c>true</c> [needs sizing].</param>
        /// <returns>Clones the general</returns>
        public static UIElement CloneGeneral(UIElement target, bool needsSizing)
        {
            UIElement cloned = null;
            if (target is FrameworkElement)
            {
                //(target as FrameworkElement).ContextMenu = null;
            }

            if (target is RibbonButton)
            {
                cloned = CloneRibbonButton(target as RibbonButton);
            }
            else if (target is SimpleMenuButton)
            {
                cloned = CloneSimpleMenuButtonAsButton(target as SimpleMenuButton);
            }
            else if (target is ButtonPanel)
            {
                cloned = CloneRibbonItemsGroup(target as ButtonPanel);
            }
            //else if (target is MenuButton)
            //{
            //    cloned = CloneMenuButton(target as MenuButton);
            //}
            else if (target is SplitMenuButton)
            {
                cloned = CloneSplitMenuButton(target as SplitMenuButton);
            }
            else if (target is RibbonDropDownButton)
            {
                cloned = CloneDropDownButton(target as RibbonDropDownButton);
            }
            else if (target is RibbonSplitButton)
            {
                cloned = CloneSplitButton(target as RibbonSplitButton);
            }
            else if (target is RibbonGallery)
            {
                cloned = CloneRibbonGallery(target as RibbonGallery);
                //(cloned as RibbonGallery).VisualMode = RibbonGalleryVisualMode.DropDown;
                //(cloned as RibbonGallery).SizeMode = SizeMode.Small;
                (cloned as RibbonGallery).Mode = RibbonGalleryMode.General;
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
            else
            {
                cloned = CloneBasic(target);
            }

            //if (needsSizing && cloned is ICollapsable)
            //{
            //    ICollapsable collapsable = cloned as ICollapsable;
            //    collapsable.SizeMode = SizeMode.ExtraSmall;

            //    if (collapsable.SmallIcon == null && collapsable.LargeIcon != null)
            //    {
            //        if (collapsable is RibbonButton)
            //        {
            //            (collapsable as RibbonButton).SmallIcon = collapsable.LargeIcon;
            //        }
            //        else if (collapsable is RibbonDropDownButton)
            //        {
            //            (collapsable as RibbonDropDownButton).SmallIcon = collapsable.LargeIcon;
            //        }
            //    }
            //}
            return cloned;
        }

        private static UIElement CloneRibbonItemsGroup(ButtonPanel ribbonItemsGroup)
        {
            if (ribbonItemsGroup == null) return null;
            ButtonPanel b = new ButtonPanel();
            b.BorderBrush = ribbonItemsGroup.BorderBrush;
            b.BorderThickness = ribbonItemsGroup.BorderThickness;
            b.Margin = ribbonItemsGroup.Margin;
            b.RibbonItemTemplates = ribbonItemsGroup.RibbonItemTemplates;
            b.DisplayMemberPath = ribbonItemsGroup.DisplayMemberPath;
            if (ribbonItemsGroup.ItemsPanel != null)
                b.ItemsPanel = ribbonItemsGroup.ItemsPanel;
            if (ribbonItemsGroup.ItemTemplate != null)
                b.ItemTemplate = ribbonItemsGroup.ItemTemplate;
            if (ribbonItemsGroup.ItemsSource != null)
                b.ItemsSource = ribbonItemsGroup.ItemsSource;
            CloneItemCollection(ribbonItemsGroup.Items, b.Items);
            return b;
        }

        /// <summary>
        /// Clones the split button.
        /// </summary>
        /// <param name="splitButton">The split button.</param>
        /// <returns>Cloned the split button.</returns>
        private static RibbonSplitButton CloneSplitButton(RibbonSplitButton splitButton)
        {
            RibbonSplitButton cloned = new RibbonSplitButton();
            //RibbonCommandManager.SetSynchronizedItem(cloned, RibbonCommandManager.GetSynchronizedItem(splitButton));
            if (isSizeMode)
            {
                cloned.SizeMode = splitButton.SizeMode;
            }
            else
            {
                cloned.SizeMode = SizeMode.Small;
            }
            if(splitButton.SmallIcon != null)
                cloned.SmallIcon = splitButton.SmallIcon;
            if (splitButton.LargeIcon != null)
                cloned.LargeIcon = splitButton.LargeIcon;
            //cloned.IsGroup = splitButton.IsGroup;
            //cloned.IsLargeImageVisible = splitButton.IsLargeImageVisible;
            //cloned.ToolTip = splitButton.ToolTip;
            cloned.Label = splitButton.Label;
            if(splitButton.Content is RibbonMenuGroup)
                cloned.Content = CloneRibbonMenuGroup(splitButton.Content as RibbonMenuGroup);
            cloned.Command = splitButton.Command;
            cloned.CommandParameter = splitButton.CommandParameter;
            //cloned.CommandTarget = splitButton.CommandTarget;
            //CloneItemCollection(splitButton.Items, cloned.Items);
            return cloned;
        }

        /// <summary>
        /// Clones the split menu button.
        /// </summary>
        /// <param name="splitMenuButton">The split menu button.</param>
        /// <returns>cloned split menu button</returns>
        private static UIElement CloneSplitMenuButton(SplitMenuButton splitMenuButton)
        {
            SplitMenuButton cloned = new SplitMenuButton();
            //cloned.SizeMode = SizeMode.Small;
            cloned.Icon = splitMenuButton.Icon;
            cloned.Header = splitMenuButton.Header;
            //cloned.ToolTip = splitMenuButton.ToolTip;
            cloned.Command = splitMenuButton.Command;
            //CloneManager.CloneEventHandler(splitMenuButton, cloned, CommandManager.CanExecuteEvent);
            //CloneManager.CloneEventHandler(splitMenuButton, cloned, CommandManager.ExecutedEvent);
            cloned.CommandParameter = splitMenuButton.CommandParameter;
            //cloned.CommandTarget = splitMenuButton.CommandTarget;
            //RibbonCommandManager.SetSynchronizedItem(cloned, RibbonCommandManager.GetSynchronizedItem(splitMenuButton));
            CloneItemCollection(splitMenuButton.Items, cloned.Items);
            return cloned;
        }

        /// <summary>
        /// Clones the ApplicationMenuBar.
        /// </summary>
        /// <param name="appgroup"></param>
        /// <returns></returns>
        private static UIElement CloneRibbonMenuGroup(RibbonMenuGroup appgroup)
        {
            RibbonMenuGroup cloned = new RibbonMenuGroup();
            cloned.Header = appgroup.Header;
            cloned.IconBarEnabled = appgroup.IconBarEnabled;
            cloned.IsMoreItemsIconTrayEnabled = appgroup.IsMoreItemsIconTrayEnabled;
            cloned.IsResizable = appgroup.IsResizable;
            cloned.parentApplicationMenu = appgroup.parentApplicationMenu;
            cloned.ScrollBarVisibility = appgroup.ScrollBarVisibility;
            CloneItemCollection(appgroup.Items, cloned.Items);
            CloneObservableUICollection(appgroup.MoreItems, cloned.MoreItems);
            return cloned;
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
                //clonedButton.ToolTip = (element as FrameworkElement).ToolTip;
            }

            //Syncfusion.Windows.Shared.DictionaryList styleList = SkinStorage.GetVisualStylesList(element);
            //if (styleList != null)
            //{
            //    RibbonSkinObjectExtension ext = new RibbonSkinObjectExtension();
            //    SkinStorage.SetVisualStylesList(clonedButton, styleList);
            //}

            if (clonedButton != null)
            {
                //CloneManager.CloneEventHandler(element, clonedButton, Button.ClickEvent);
            }

            if (element is ICommandSource && (element as ICommandSource).Command != null)
            {
                ICommandSource commandSource = element as ICommandSource;
                clonedButton.Command = commandSource.Command;
                //clonedButton.CommandTarget = commandSource.CommandTarget;
                //CloneManager.CloneEventHandler(element, clonedButton, CommandManager.CanExecuteEvent);
                //CloneManager.CloneEventHandler(element, clonedButton, CommandManager.ExecutedEvent);

                if (commandSource.CommandParameter != null)
                {
                    clonedButton.CommandParameter = commandSource.CommandParameter;
                }
            }

            return clonedButton as UIElement;
        }

        /// <summary>
        /// Clones the basic.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>clones the basic</returns>
        public static UIElement CloneBasic(UIElement element)
        {
            //StringReader strReader = new StringReader(XamlWriter.Save(element));
            //XmlReader xmlReader = XmlReader.Create(strReader);

            //UIElement cloned = XamlReader.Load(xmlReader) as UIElement;
            UIElement cloned = element;

            ButtonBase clonedButton = cloned as ButtonBase;
            if (clonedButton != null && element is FrameworkElement)
            {
                //clonedButton.ToolTip = (element as FrameworkElement).ToolTip;
            }

            //Syncfusion.Windows.Shared.DictionaryList styleList = SkinStorage.GetVisualStylesList(element);
            //if (styleList != null)
            //{
            //    RibbonSkinObjectExtension ext = new RibbonSkinObjectExtension();
            //    SkinStorage.SetVisualStylesList(cloned, styleList);
            //}


            if (clonedButton != null)
            {
                //CloneManager.CloneEventHandler(element, cloned, Button.ClickEvent);

            }
            else if (element is RibbonMenuItem)
            {
                //CloneManager.CloneEventHandler(element, cloned, RibbonMenuItem.ClickEvent);
                if (cloned is FrameworkElement)
                {
                    //(cloned as FrameworkElement).ToolTip = (element as FrameworkElement).ToolTip;
                }
            }
            else if (element is ComboBox)
            {
                //BindingUtils.SetBinding(element, cloned, ComboBox.SelectedIndexProperty, ComboBox.SelectedIndexProperty);
            }

            if (element is ToggleButton)
            {
                //BindingUtils.SetBinding(element, clonedButton, ToggleButton.IsCheckedProperty, ToggleButton.IsCheckedProperty);
            }


            if (element is ICommandSource && (element as ICommandSource).Command != null)
            {
                ICommandSource commandSource = element as ICommandSource;
                if (clonedButton == null && cloned is RibbonMenuItem)
                {
                    RibbonMenuItem clonedMenuItem = cloned as RibbonMenuItem;
                    clonedMenuItem.Command = commandSource.Command;
                    //clonedMenuItem.CommandTarget = commandSource.CommandTarget;
                    //CloneManager.CloneEventHandler(element, clonedButton, CommandManager.CanExecuteEvent);
                    //CloneManager.CloneEventHandler(element, clonedButton, CommandManager.ExecutedEvent);

                    if (commandSource.CommandParameter != null)
                    {
                        clonedMenuItem.CommandParameter = commandSource.CommandParameter;
                    }
                }
                else if(clonedButton != null)
                {
                    clonedButton.Command = commandSource.Command;
                    //clonedButton.CommandTarget = commandSource.CommandTarget;
                    //CloneManager.CloneEventHandler(element, clonedButton, CommandManager.CanExecuteEvent);
                    //CloneManager.CloneEventHandler(element, clonedButton, CommandManager.ExecutedEvent);

                    if (commandSource.CommandParameter != null)
                    {
                        clonedButton.CommandParameter = commandSource.CommandParameter;
                    }
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
            //cloned.CornerRadius = button.CornerRadius;
            cloned.IsChecked = button.IsChecked;
            cloned.IsCheckable = button.IsCheckable;
            //cloned.IsToggle = button.IsToggle;
            cloned.Label = button.Label;
            cloned.LargeIcon = button.LargeIcon;
            if (isSizeMode)
            {
                cloned.SizeMode = button.SizeMode;
            }
            else
            {
                cloned.SizeMode = SizeMode.Small;
            }
            cloned.SmallIcon = button.SmallIcon;
            //cloned.ToolTip = button.ToolTip;
            //RibbonCommandManager.SetSynchronizedItem(cloned, RibbonCommandManager.GetSynchronizedItem(button));

            //CloneRibbonButtonBinding(button, cloned, "IsSelected", RibbonButton.IsSelectedProperty);
            //CloneRibbonButtonBinding(button, cloned, "IsToggle", RibbonButton.IsToggleProperty);
            CloneRibbonButtonBinding(button, cloned, "SmallIcon", RibbonButton.SmallIconProperty);
            //CloneRibbonButtonBinding(button, cloned, "ToolTip", RibbonButton.ToolTipProperty);
            CloneRibbonButtonBinding(button, cloned, "Label", RibbonButton.LabelProperty);
            CloneRibbonButtonBinding(button, cloned, "LargeIcon", RibbonButton.LargeIconProperty);

            if (cloned.SmallIcon == null && button.LargeIcon != null)
            {
                cloned.SmallIcon = button.LargeIcon;
            }

            string[] eventNames = null;
            Delegate[] eventHandlers = null;
            //button.GetEventHandlers(ref eventNames, ref eventHandlers);
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
                                            //cloned.IsSelectedChanged += (PropertyChangedCallback)handler;
                                            flag = true;
                                        }

                                        break;
                                    case "IsToggleChanged":
                                        if (eventName.Split(' ')[1].Equals(handler.Method.Name))
                                        {
                                            //cloned.IsToggleChanged += (PropertyChangedCallback)handler;
                                            flag = true;
                                        }

                                        break;
                                    case "LabelChanged":
                                        if (eventName.Split(' ')[1].Equals(handler.Method.Name))
                                        {
                                            //cloned.LabelChanged += (PropertyChangedCallback)handler;
                                            flag = true;
                                        }

                                        break;
                                    case "LargeIconChanged":
                                        if (eventName.Split(' ')[1].Equals(handler.Method.Name))
                                        {
                                            //cloned.LargeIconChanged += (PropertyChangedCallback)handler;
                                            flag = true;
                                        }

                                        break;
                                    case "SizeModeChanged":
                                        if (eventName.Split(' ')[1].Equals(handler.Method.Name))
                                        {
                                            //cloned.SizeModeChanged += (PropertyChangedCallback)handler;
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
            //binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = sourceButton;
            binding.Mode = BindingMode.TwoWay;
            clonedButton.SetBinding(depProperty, binding);
        }

        /// <summary>
        /// Clones the drop down button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>cloned the drop down button</returns>
        private static RibbonDropDownButton CloneDropDownButton(RibbonDropDownButton button)
        {
            RibbonDropDownButton cloned = new RibbonDropDownButton();
            if (isSizeMode)
            {
                cloned.SizeMode = button.SizeMode;
            }
            else
            {
                cloned.SizeMode = SizeMode.Small;
            }
            if (button.SmallIcon != null)
                cloned.SmallIcon = button.SmallIcon;
            if (button.LargeIcon != null)
                cloned.LargeIcon = button.LargeIcon;
            //cloned.ToolTip = button.ToolTip;
            cloned.Label = button.Label;
            //cloned.IsGroup = button.IsGroup;
            //RibbonCommandManager.SetSynchronizedItem(cloned, RibbonCommandManager.GetSynchronizedItem(button));
            CloneDropDownButtonBinding(button, cloned, "SmallIcon", RibbonDropDownButton.SmallIconProperty);
            //CloneDropDownButtonBinding(button, cloned, "IsGroup", RibbonDropDownButton.IsGroupProperty);
            //CloneDropDownButtonBinding(button, cloned, "ToolTip", RibbonDropDownButton.ToolTipProperty);
            //CloneItemCollection(button.Items, cloned.Items);
            if (button.Content is RibbonMenuGroup)
                cloned.Content = CloneRibbonMenuGroup(button.Content as RibbonMenuGroup);
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

            //cloned.SizeMode = SizeMode.ExtraSmall;
            //cloned.SmallIcon = checkBox.SmallIcon;
            cloned.Content = checkBox.Content;
            cloned.IsChecked = checkBox.IsChecked;
            //cloned.ToolTip = checkBox.ToolTip;
            cloned.Command = checkBox.Command;
            cloned.CommandParameter = checkBox.CommandParameter;
            //cloned.CommandTarget = checkBox.CommandTarget;

            //CloneManager.CloneEventHandler(checkBox, cloned, CommandManager.CanExecuteEvent);
            //CloneManager.CloneEventHandler(checkBox, cloned, CommandManager.ExecutedEvent);

            //CloneManager.CloneEventHandler(checkBox, cloned, RibbonCheckBox.UncheckedEvent);
            //CloneManager.CloneEventHandler(checkBox, cloned, RibbonCheckBox.CheckedEvent);

            BindingExpression bindingExp;

            bindingExp = checkBox.GetBindingExpression(RibbonCheckBox.IsCheckedProperty);
            //if (bindingExp != null && bindingExp.DataItem is RibbonCheckBox)
            //    BindingUtils.SetBinding((RibbonCheckBox)bindingExp.DataItem, cloned, RibbonCheckBox.IsCheckedProperty, RibbonCheckBox.IsCheckedProperty);

            bindingExp = checkBox.GetBindingExpression(RibbonCheckBox.CommandParameterProperty);
            //if (bindingExp != null && bindingExp.DataItem is RibbonCheckBox)
            //    BindingUtils.SetBinding((RibbonCheckBox)bindingExp.DataItem, cloned, RibbonCheckBox.CommandParameterProperty, RibbonCheckBox.CommandParameterProperty);

            //bindingExp = checkBox.GetBindingExpression(RibbonCheckBox.CommandTargetProperty);
            //if (bindingExp != null && bindingExp.DataItem is RibbonCheckBox)
            //    BindingUtils.SetBinding((RibbonCheckBox)bindingExp.DataItem, cloned, RibbonCheckBox.CommandTargetProperty, RibbonCheckBox.CommandTargetProperty);

            //BindingUtils.SetBinding(checkBox, cloned, RibbonCheckBox.IsCheckedProperty, RibbonCheckBox.IsCheckedProperty);

            //BindingUtils.SetBinding(checkBox, cloned, RibbonCheckBox.CommandParameterProperty, RibbonCheckBox.CommandParameterProperty, BindingMode.TwoWay);
            //BindingUtils.SetBinding(checkBox, cloned, RibbonCheckBox.CommandTargetProperty, RibbonCheckBox.CommandTargetProperty, BindingMode.TwoWay);

            return cloned;
        }
        
        /// <summary>
        /// Clones the drop down button binding.
        /// </summary>
        /// <param name="sourceButton">The source button.</param>
        /// <param name="clonedButton">The cloned button.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="depProperty">The dep property.</param>
        private static void CloneDropDownButtonBinding(RibbonDropDownButton sourceButton, RibbonDropDownButton clonedButton, string propertyName, DependencyProperty depProperty)
        {
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            //binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = sourceButton;
            binding.Mode = BindingMode.TwoWay;
            clonedButton.SetBinding(depProperty, binding);
        }

        ///// <summary>
        ///// Clones the button panel.
        ///// </summary>
        ///// <param name="buttonPanel">The button panel.</param>
        ///// <returns>cloned the button panel</returns>
        //private static ButtonPanel CloneButtonPanel(ButtonPanel buttonPanel)
        //{
        //    ButtonPanel cloned = new ButtonPanel();
        //    CloneItemCollection(buttonPanel.Items, cloned.Items);
        //    cloned.ToolTip = buttonPanel.ToolTip;
        //    return cloned;
        //}

        ///// <summary>
        ///// Clones the simple menu button.
        ///// </summary>
        ///// <param name="target">The target.</param>
        ///// <returns>cloned simple menu button</returns>
        private static UIElement CloneSimpleMenuButton(SimpleMenuButton target)
        {
            ICommandSource commandSource = target as ICommandSource;
            SimpleMenuButton b = new SimpleMenuButton();
            //RibbonCommandManager.SetSynchronizedItem(b, RibbonCommandManager.GetSynchronizedItem(target));
            //RibbonButton b = new RibbonButton();
            //b.SizeMode = SizeMode.ExtraSmall;
            //if (target.Icon != null)
            //{
            //    b.SmallIcon = target.Icon;
            //}
            //CloneManager.CloneEventHandler(target, b, SimpleMenuButton.ClickEvent);
            b.Icon = target.Icon;
            b.Description = target.Description;
            b.Label = target.Label;
            //b.ToolTip = target.ToolTip;
            b.Command = commandSource.Command;
            //b.CommandTarget = commandSource.CommandTarget;
            //CloneManager.CloneEventHandler(target, b, CommandManager.CanExecuteEvent);
            //CloneManager.CloneEventHandler(target, b, CommandManager.ExecutedEvent);
            if (commandSource.CommandParameter != null)
            {
                b.CommandParameter = commandSource.CommandParameter;
            }

            return b;
        }

        ///// <summary>
        ///// Clones the simple menu button.
        ///// </summary>
        ///// <param name="target">The target.</param>
        ///// <returns>cloned simple menu button</returns>
        private static UIElement CloneSimpleMenuButtonAsButton(SimpleMenuButton target)
        {

            ICommandSource commandSource = target as ICommandSource;
            RibbonButton b = new RibbonButton();

            //RibbonCommandManager.SetSynchronizedItem(b, RibbonCommandManager.GetSynchronizedItem(target));
            b.SizeMode = SizeMode.Small;
            if (target.Icon != null)
            {
                b.SmallIcon = target.Icon;
            }

            b.Label = target.Label;
            //b.ToolTip = target.ToolTip;
            b.Command = commandSource.Command;
            //b.CommandTarget = commandSource.CommandTarget;
            //CloneManager.CloneEventHandler(target, b, Button.ClickEvent);
            //CloneManager.CloneEventHandler(target, b, CommandManager.CanExecuteEvent);
            //CloneManager.CloneEventHandler(target, b, CommandManager.ExecutedEvent);
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

            RibbonMenuItem b = new RibbonMenuItem();
            b.Header = target.Header;
            b.Command = commandSource.Command;
            b.HeaderTemplate = target.HeaderTemplate;
            if (target.Icon is Image)
            {
                b.Icon = CloneImage((Image)target.Icon);
            }
            b.IsCheckable = target.IsCheckable;
            b.IsChecked = target.IsChecked;
            //b.IsCheckedChanged = target.IsCheckedChanged;
            if (commandSource.CommandParameter != null)
            {
                b.CommandParameter = commandSource.CommandParameter;
            }
            CloneItemCollection(b.Items, target.Items);
            return b;
            /*ICommandSource commandSource = target as ICommandSource;
            if (target.HasItems)
            {
                RibbonSplitButton b = new RibbonSplitButton();
                b.SizeMode = SizeMode.Small;
                b.Label = target.Header.ToString();
                //b.ToolTip = target.ToolTip;
                b.Command = commandSource.Command;
                b.Margin = target.Margin;
                //b.CommandTarget = commandSource.CommandTarget;
                //CloneManager.CloneEventHandler(target, b, CommandManager.CanExecuteEvent);
                //CloneManager.CloneEventHandler(target, b, CommandManager.ExecutedEvent);

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

                //CloneManager.CloneEventHandler(target, b, MenuItem.ClickEvent);
                //CloneItemCollection(target.Items, b.Items);

                return b;
            }
            else
            {
                RibbonButton b = new RibbonButton();
                b.SizeMode = SizeMode.Small;
                if (target.Icon != null)
                {
                    ImageSource source = target.Icon as ImageSource;
                    if (source == null && target.Icon is Image)
                    {
                        source = (target.Icon as Image).Source;
                    }

                    b.SmallIcon = source;
                }

                //if (target.Header != null && target.ToolTip != null)
                if (target.Header != null)
                {
                    b.Label = target.Header.ToString();
                    //b.ToolTip = target.ToolTip;
                }

                //CloneManager.CloneEventHandler(target, b, Button.ClickEvent);
                b.Command = commandSource.Command;
                b.Margin = target.Margin;
                //CloneManager.CloneEventHandler(target, b, CommandManager.CanExecuteEvent);
                //CloneManager.CloneEventHandler(target, b, CommandManager.ExecutedEvent);
                //b.CommandTarget = commandSource.CommandTarget;
                if (commandSource.CommandParameter != null)
                {
                    b.CommandParameter = commandSource.CommandParameter;
                }

                return b;
            }*/
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
            //b.IconBarEnabled = target.IconBarEnabled;
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
            b.Margin = target.Margin;
            //b.ToolTip = target.ToolTip;
            b.Command = commandSource.Command;
            //CloneManager.CloneEventHandler(target, b, CommandManager.CanExecuteEvent);
            //CloneManager.CloneEventHandler(target, b, CommandManager.ExecutedEvent);
            //b.CommandTarget = commandSource.CommandTarget;
            if (commandSource.CommandParameter != null)
            {
                b.CommandParameter = commandSource.CommandParameter;
            }

            //CloneManager.CloneEventHandler(target, b, MenuItem.ClickEvent);
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
            //cloned.ToolTip = gallery.ToolTip;

            //foreach (RibbonGalleryFilter filter in gallery.GalleryFilters)
            //{
            //    RibbonGalleryFilter clonedFilter = new RibbonGalleryFilter();
            //    clonedFilter.Label = filter.Label;
            //    cloned.GalleryFilters.Add(clonedFilter);
            //}

            //foreach (RibbonGalleryGroup group in gallery.GalleryGroups)
            //{
            //    RibbonGalleryGroup clonedGroup = new RibbonGalleryGroup();
            //    clonedGroup.Label = group.Label;
            //    //clonedGroup.ToolTip = group.ToolTip;
            //    clonedGroup.SetValue(RibbonGallery.FilterIndexesProperty, group.GetValue(RibbonGallery.FilterIndexesProperty));

            //    foreach (UIElement item in group.Items)
            //    {
            //        clonedGroup.Items.Add(CloneBasic(item));
            //    }

            //    cloned.GalleryGroups.Add(clonedGroup);
            //}

            //foreach (UIElement item in gallery.MenuItems)
            //{
            //    cloned.MenuItems.Add(CloneBasic(item));
            //}

            CloneItemCollection(gallery.Items, cloned.Items);

            //cloned.CurrentFilter = gallery.CurrentFilter;
            cloned.Height = gallery.ActualHeight;
            cloned.Width = gallery.ActualWidth;
            cloned.Mode = gallery.Mode;
            //cloned.SelectedItem = gallery.SelectedItem;

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
            cloned.Background = RibbonTab.RibbonBarBackground;
            isSizeMode = true;
            CloneItemCollection(bar.Items, cloned.Items);
            isSizeMode = false;
            cloned.Header = bar.Header;
            cloned.IsLauncherButtonVisible = bar.IsLauncherButtonVisible;
            //cloned.CollapsedImage = bar.CollapsedImage;
            cloned.Visibility = bar.Visibility;
            cloned.Margin = bar.Margin;
            cloned.LayoutMode = bar.LayoutMode;
            cloned.LauncherCommand = bar.LauncherCommand;
            RibbonDropDownButton barButton = new RibbonDropDownButton();
            barButton.Content = cloned;
            barButton.Label = bar.Header;
            barButton.SizeMode = SizeMode.Small;
            barButton.SmallIcon = bar.CollapseImage;
            return barButton;
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
            //comboCloned.Label = ribbonCombo.Label;
            //comboCloned.SmallIcon = ribbonCombo.SmallIcon;
            //comboCloned.ToolTip = ribbonCombo.ToolTip;
            comboCloned.Width = ribbonCombo.Width;
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

            //BindingUtils.SetBinding(ribbonCombo, comboCloned, RibbonComboBox.SelectedIndexProperty, RibbonComboBox.SelectedIndexProperty);
            return comboCloned;
        }

        /// <summary>
        /// Clones the combo box item.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>return combobox item.</returns>
        private static RibbonComboBoxItem CloneComboBoxItem(ComboBoxItem target)
        {
            ICommandSource commandSource = target as ICommandSource;
            RibbonComboBoxItem comBoxItem = new RibbonComboBoxItem();
            comBoxItem.Content = target.Content;
            comBoxItem.IsSelected = target.IsSelected;
            CloneComboBoxItemBinding(target, comBoxItem, "Content", ComboBoxItem.ContentProperty);
            CloneComboBoxItemBinding(target, comBoxItem, "IsSelected", ComboBoxItem.IsSelectedProperty);
            //CloneManager.CloneEventHandler(target, comBoxItem, ComboBoxItem.PreviewMouseMoveEvent);
            //CloneManager.CloneEventHandler(target, comBoxItem, ComboBoxItem.PreviewMouseLeftButtonUpEvent);
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
            //binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
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
            //RoutedEventHandlerInfo[] routedEventHandlers;
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

            //routedEventHandlers = getRoutedEventHandlers.Invoke(value, new object[] { cloneEvent }) as RoutedEventHandlerInfo[];

            //if (routedEventHandlers == null)
            //{
            //    return false;
            //}

            //if (routedEventHandlers.Length != 0)
            //{
            //    RoutedEventHandlerInfo[] infoArray = routedEventHandlers;

            //    for (int i = 0; i < infoArray.Length; i++)
            //    {
            //        RoutedEventHandlerInfo routedEventHandler = infoArray[i];
            //        clonedElement.AddHandler(cloneEvent, routedEventHandler.Handler, routedEventHandler.InvokeHandledEventsToo);
            //    }

            //    return true;
            //}

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
                    else if (enumerator.Current is RibbonDropDownButton)
                    {
                        cloned = CloneDropDownButton(enumerator.Current as RibbonDropDownButton);
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

        private static void CloneObservableUICollection(System.Collections.ObjectModel.ObservableCollection<UIElement> input, System.Collections.ObjectModel.ObservableCollection<UIElement> output)
        {
            IEnumerator enumerator = input.GetEnumerator();
            UIElement cloned = null;
            try
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is RibbonButton)
                    {
                        cloned = CloneRibbonButton(enumerator.Current as RibbonButton);
                    }
                    else if (enumerator.Current is RibbonDropDownButton)
                    {
                        cloned = CloneDropDownButton(enumerator.Current as RibbonDropDownButton);
                    }
                    else if (enumerator.Current is RibbonSplitButton)
                    {
                        cloned = CloneSplitButton(enumerator.Current as RibbonSplitButton);
                    }
                    else if (enumerator.Current is RibbonMenuItem)
                    {
                        cloned = CloneRibbonMenuItem(enumerator.Current as RibbonMenuItem);
                    }
                    else
                    {
                        cloned = CloneBasic(enumerator.Current as UIElement);
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
            object cloned;
            try
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is RibbonButton)
                    {
                        cloned = CloneRibbonButton(enumerator.Current as RibbonButton);
                    }
                    else if (enumerator.Current is RibbonSplitButton)
                    {
                        cloned = CloneSplitButton(enumerator.Current as RibbonSplitButton);
                    }
                    else if (enumerator.Current is RibbonDropDownButton)
                    {
                        cloned = CloneDropDownButton(enumerator.Current as RibbonDropDownButton);
                    }
                    else if (enumerator.Current is RibbonMenuItem)
                    {
                        cloned = CloneRibbonMenuItem(enumerator.Current as RibbonMenuItem);
                    }
                    else if (enumerator.Current is RibbonGallery)
                    {
                        cloned = CloneRibbonGallery(enumerator.Current as RibbonGallery);
                    }
                    else if (enumerator.Current is RibbonGalleryItem)
                    {
                        cloned = CloneRibbonGalleryItem(enumerator.Current as RibbonGalleryItem);
                    }
                    //else if (enumerator.Current is ButtonPanel)
                    //{
                    //    cloned = CloneButtonPanel(enumerator.Current as ButtonPanel);
                    //}
                    //else if (enumerator.Current is ApplicationMenuGroup)
                    //{
                    //    cloned = CloneApplicationMenuBar(enumerator.Current as ApplicationMenuGroup);
                    //}
                    //else if (enumerator.Current is RibbonMenuItem)
                    //{
                    //    ////Do not convert menu items here. It should be done only in CloneGeneral method.                  
                    //    cloned = CloneRibbonMenuItemInItemsCloneCall(enumerator.Current as RibbonMenuItem);
                    //}
                    else if (enumerator.Current is RibbonMenuGroup)
                    {
                        cloned = CloneRibbonMenuGroup(enumerator.Current as RibbonMenuGroup);
                    }
                    else if (enumerator.Current is ButtonPanel)
                    {
                        cloned = CloneRibbonItemsGroup(enumerator.Current as ButtonPanel);
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
                    else if (enumerator.Current is RibbonComboBox)
                    {
                        cloned = CloneRibbonComboBox(enumerator.Current as RibbonComboBox);
                    }
                    else
                    {
                        cloned = CloneBasic(enumerator.Current as UIElement) as UIElement;
                    }
                    
                    //Ribbon.SetKeyTip(cloned as FrameworkElement, Ribbon.GetKeyTip(enumerator.Current as FrameworkElement));
                    if(cloned is IRibbonControl)
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

        private static RibbonGalleryItem CloneRibbonGalleryItem(RibbonGalleryItem ribbonGalleryItem)
        {
            RibbonGalleryItem b = new RibbonGalleryItem();
            if (ribbonGalleryItem.Content is Image)
            {
                b.Content = CloneImage((Image)ribbonGalleryItem.Content);
            }
            //b.Content = ribbonGalleryItem.Content;
            //b.AppearanceState = ribbonGalleryItem.AppearanceState;
            //b.Checked = ribbonGalleryItem.Checked;
            b.ClickMode = ribbonGalleryItem.ClickMode;
            b.Command = ribbonGalleryItem.Command;
            //b.Content = CloneGeneral(ribbonGalleryItem.Content as UIElement, false);
            b.Image = ribbonGalleryItem.Image;
            //b.IsPressed = ribbonGalleryItem.IsPressed;
            b.IsSelected = ribbonGalleryItem.IsSelected;
            b.Label = ribbonGalleryItem.Label;
            b.LargeImage = ribbonGalleryItem.LargeImage;
            b.MouseDown = ribbonGalleryItem.MouseDown;
            b.Selector = ribbonGalleryItem.Selector;
            //b.VisualState = ribbonGalleryItem.VisualState;
            return b;
        }

        private static Image CloneImage(Image img)
        {
            var clonedImg = new Image();
            clonedImg.Source = img.Source;
            if (double.IsNaN(img.ActualHeight) == false && img.ActualHeight > 0)
                clonedImg.Height = img.ActualHeight;
            if (double.IsNaN(img.ActualWidth) == false && img.ActualWidth > 0)
                clonedImg.Width = img.ActualWidth;
            return clonedImg;
        }
    }
}
