#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Collections;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Markup;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// 
    /// </summary>
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
    Type = typeof(PinnableListBoxItem), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/PinnableListBox/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(PinnableListBoxItem), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/PinnableListBox/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(PinnableListBoxItem), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/PinnableListBox/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
    Type = typeof(PinnableListBoxItem), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/PinnableListBox/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(PinnableListBoxItem), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/PinnableListBox/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(PinnableListBoxItem), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/PinnableListBox/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(PinnableListBoxItem), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/PinnableListBox/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(PinnableListBoxItem), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/PinnableListBox/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(PinnableListBoxItem), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/PinnableListBox/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(PinnableListBoxItem), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/PinnableListBox/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
    Type = typeof(PinnableListBoxItem), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/PinnableListBox/Themes/TransparentStyle.xaml")]    
    public class PinnableListBoxItem : ContentControl, ICommandSource
    {
        static PinnableListBoxItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PinnableListBoxItem), new FrameworkPropertyMetadata(typeof(PinnableListBoxItem)));
        }
        /// <summary>
        /// 
        /// </summary>
        public PinnableListBoxItem()
        {
           DefaultStyleKey = typeof(PinnableListBoxItem);
           Loaded += new RoutedEventHandler(PinnableListBoxItem_Loaded);
           Unloaded += new RoutedEventHandler(PinnableListBoxItem_Unloaded);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PinnableListBoxItem_Unloaded(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow != null)
            {
                Loaded -= new RoutedEventHandler(PinnableListBoxItem_Loaded);
            }
            if (cMenu != null)
            {
                cMenu.CommandBindings.Clear();
                cMenu.Items.Clear();
                cMenu = null;
            }
         }
        ContextMenu cMenu;
        void PinnableListBoxItem_Loaded(object sender, RoutedEventArgs e)
        {
            CommandBinding binding = new CommandBinding(PinCommand, OnPinExecute, OnPinCanExecute);
            CommandBindings.Add(binding);
            cMenu = new ContextMenu();
            this.cMenu.Items.Add(new MenuItem() { Header = "Pin to List", Command = PinCommand });
            this.cMenu.Items.Add(new MenuItem() { Header = "_Remove from list", Command = RemoveCommand });
            this.cMenu.Items.Add(new MenuItem() { Header = "Cl_ear unpinned Documents", Command = ClearCommand });
            this.ContextMenu = cMenu;
            if (cMenu != null)
            {
                cMenu.Tag = this;
                CommandBinding binding1 = new CommandBinding(RemoveCommand, OnRemoveExecute, OnRemoveCanExecute);
                CommandBinding binding2 = new CommandBinding(ClearCommand, OnClearExecute, OnCLearCanExecute);
                cMenu.CommandBindings.Add(binding);
                cMenu.CommandBindings.Add(binding1);
                cMenu.CommandBindings.Add(binding2);
            }

            if(this.ContextMenu == null) 
                    this.ContextMenu = cMenu;

            if (pinnableListBox != null)
            {
                if (pinnableListBox.ItemsSource != null)
                {
                    if (!pinnableListBox.PinnedItems.Contains(this.DataContext) && IsPinned)
                    {
                        pinnableListBox.PinnedItems.Add(this.DataContext);
                        if (pinnableListBox.UnpinnedItems.Contains(this.DataContext))
                            pinnableListBox.UnpinnedItems.Remove(this.DataContext);
                    }

                }
                else
                {
                    if (!pinnableListBox.PinnedItems.Contains(this) && IsPinned)
                    {
                        pinnableListBox.PinnedItems.Add(this);
                    }
                }
            }
           
        }

        internal PinnableListBox pinnableListBox;
        /// <summary>
        /// 
        /// </summary>
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(PinnableListBoxItem), new UIPropertyMetadata(null));


        /// <summary>
        /// 
        /// </summary>
        public DateTime AddedTime
        {
            get { return (DateTime)GetValue(AddedTimeProperty); }
            set { SetValue(AddedTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddedTime.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AddedTimeProperty =
            DependencyProperty.Register("AddedTime", typeof(DateTime), typeof(PinnableListBoxItem), new UIPropertyMetadata(null));


        
        /// <summary>
        /// 
        /// </summary>
        public bool IsPinned
        {
            get { return (bool)GetValue(IsPinnedProperty); }
            set { SetValue(IsPinnedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPinned.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsPinnedProperty =
            DependencyProperty.Register("IsPinned", typeof(bool), typeof(PinnableListBoxItem), new PropertyMetadata(false,new PropertyChangedCallback(OnIsPinnedChanged)));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnIsPinnedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PinnableListBoxItem item = obj as PinnableListBoxItem;
            if (item != null)
            {
                if (item.pinnableListBox == null && item.Parent is PinnableListBox)
                    item.pinnableListBox = item.Parent as PinnableListBox;
                if (item.pinnableListBox != null && !item.pinnableListBox.isCalledByUpdateItems)
                {
                    item.pinnableListBox.pinnableItem = item;
                    if (item.pinnableListBox.ItemsSource != null )
                    {
                        if (item is PinnableListBoxItem)
                            item.pinnableListBox.UpdatePinItems(item.pinnableListBox, item,(bool)args.NewValue);
                        else
                            item.pinnableListBox.UpdatePinItems(item.pinnableListBox, item.DataContext, (bool)(args.NewValue));
                    }
                    else
                    {
                        item.pinnableListBox.UpdatePinItems(item.pinnableListBox, item, (bool)args.NewValue);
                    }
                }
            }
            if ((PinnableListBoxItem)obj != null)
                ((PinnableListBoxItem)obj).OnIsPinnedChanged(args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected void OnIsPinnedChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.ContextMenu == null)
            {
                cMenu = new ContextMenu();
                this.cMenu.Items.Add(new MenuItem() { Header = "Pin to List", Command = PinCommand });
                this.cMenu.Items.Add(new MenuItem() { Header = "_Remove from list", Command = RemoveCommand });
                this.cMenu.Items.Add(new MenuItem() { Header = "Cl_ear unpinned Documents", Command = ClearCommand });
            }
            if (pinnableListBox != null)
                pinnableListBox.FirePinStatusChanged();
        }       
        /// <summary>
        /// 
        /// </summary>
        public Thickness CornerRadius
        {
            get { return (Thickness)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(Thickness), typeof(PinnableListBoxItem), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(PinnableListBoxItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(PinnableListBoxItem), new UIPropertyMetadata(""));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
                     
            base.OnInitialized(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            IsSelected = true;
            if (pinnableListBox != null)
            {
                if (pinnableListBox.pinnableItem != null)
                {
                    pinnableListBox.pinnableItem.IsSelected = false;
                }
                pinnableListBox.SelectedItem = this;
                pinnableListBox.pinnableItem = this;     
            }
            RoutedCommand routedCommand = Command as RoutedCommand;
            if (routedCommand != null)
            {
                routedCommand.Execute(CommandParameter, CommandTarget);
            }
            else if(Command!=null)
            {
                Command.Execute(CommandParameter);
            }
            base.OnMouseLeftButtonUp(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnPinExecute(object sender, ExecutedRoutedEventArgs e)
        {
            PinnableListBoxItem item = e.Source as PinnableListBoxItem;

            if (item == null)
            {
                if (sender is PinnableListBoxItem)
                    item = sender as PinnableListBoxItem;
                else
                {
                    if(sender is ContextMenu)
                        item = ((ContextMenu)sender).PlacementTarget as PinnableListBoxItem;
                }
            }

            if (item != null)
            {
                item.IsPinned = !item.IsPinned;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnPinCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnRemoveExecute(object sender, ExecutedRoutedEventArgs args)
        {
            PinnableListBoxItem item = ((ContextMenu)sender).PlacementTarget as PinnableListBoxItem;
            if (item != null)
            {
                if (item.pinnableListBox != null)
                {
                    if (item.pinnableListBox.ItemsSource == null)
                    {
                        item.pinnableListBox.Items.Remove(item);
                        if (!item.IsPinned)
                        {
                            item.pinnableListBox.UnpinnedItems.Remove(item);
                        }
                        else
                        {
                            item.pinnableListBox.PinnedItems.Remove(item);
                        }
                    }
                    else
                    {
                        ((IList)item.pinnableListBox.ItemsSource).Remove(item.DataContext);
                        if (item.IsPinned)
                        {
                            item.pinnableListBox.PinnedItems.Remove(item.DataContext);
                        }
                        else
                        {
                            item.pinnableListBox.UnpinnedItems.Remove(item.DataContext);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnRemoveCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnClearExecute(object sender, ExecutedRoutedEventArgs args)
        {
            PinnableListBoxItem item = ((ContextMenu)sender).Tag as PinnableListBoxItem;
            if (item != null)
            {
                if (item.pinnableListBox != null)
                {
                    if (item.pinnableListBox.ItemsSource == null)
                    {
                        foreach (var _item in item.pinnableListBox.UnpinnedItems)
                        {
                            item.pinnableListBox.Items.Remove(_item);
                            item.pinnableListBox.UnpinnedItems.Remove(_item);
                        }
                    }
                    else
                    {
                        foreach (var _item in item.pinnableListBox.UnpinnedItems.ToList())
                        {
                            ((IList)item.pinnableListBox.ItemsSource).Remove(_item);
                            item.pinnableListBox.UnpinnedItems.Remove(_item);

                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnCLearCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            PinnableListBoxItem item = ((ContextMenu)sender).Tag as PinnableListBoxItem;
            if (item != null)
            {
                if (item.pinnableListBox != null)
                {
                    if (item.pinnableListBox.UnpinnedItems.Count > 0)
                    {
                        args.CanExecute = true;
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly RoutedCommand PinCommand = new RoutedCommand("PinCommand", typeof(PinnableListBoxItem));
        /// <summary>
        /// 
        /// </summary>
        public static readonly RoutedCommand RemoveCommand = new RoutedCommand("RemoveCommand", typeof(PinnableListBoxItem));
        /// <summary>
        /// 
        /// </summary>
        public static readonly RoutedCommand ClearCommand = new RoutedCommand("ClearCommand", typeof(PinnableListBoxItem));
        /// <summary>
        /// 
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(PinnableListBoxItem), new UIPropertyMetadata(null, new PropertyChangedCallback(OnCommandChanged)));

        /// <summary>
        /// 
        /// </summary>
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(PinnableListBoxItem), new UIPropertyMetadata(null));


        /// <summary>
        /// 
        /// </summary>
        public IInputElement CommandTarget
        {
            get { return (IInputElement)GetValue(CommandTargetProperty); }
            set { SetValue(CommandTargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandTarget.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CommandTargetProperty =
            DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(PinnableListBoxItem), new UIPropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ICommand oldCommand = args.OldValue as ICommand;
            ICommand newCommand = args.NewValue as ICommand;
            PinnableListBoxItem control = sender as PinnableListBoxItem;
            if (control != null)
            {
                control.HookCommand(oldCommand, newCommand);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="olcommand"></param>
        /// <param name="newommand"></param>
        private void HookCommand(ICommand olcommand, ICommand newommand)
        {
            if (olcommand != null)
            {
                EventHandler handler = CanExecuteChanged;
                olcommand.CanExecuteChanged -= handler;
            }
            if (newommand != null)
            {
                EventHandler handler = new EventHandler(CanExecuteChanged);
                newommand.CanExecuteChanged += handler;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanExecuteChanged(object sender, EventArgs e)
        {
            RoutedCommand routedCommand = this.Command as RoutedCommand;
            if (routedCommand != null)
            {
                if (routedCommand.CanExecute(CommandParameter, CommandTarget))
                {
                    IsEnabled = true;
                }
                else
                {
                    IsEnabled = false;
                }
            }
            else
            {
                if (Command.CanExecute(CommandParameter))
                {
                    IsEnabled = true;
                }
                else
                {
                    IsEnabled = false;
                }
            }
        }
    }
}
