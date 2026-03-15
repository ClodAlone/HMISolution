#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    ///
    /// </summary>
    public class TabNavigationItem : Control
    {
        internal TabNavigationControl parent;

        /// <summary>
        ///
        /// </summary>
        public TabNavigationItem()
        {
#if WPF
#if SyncfusionFramework4_0
            ResourceDictionary rdlocal = new ResourceDictionary();
            rdlocal.Source = new Uri("/Syncfusion.Tools.WPF;component/Controls/TabNavigationControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
            Style = rdlocal[typeof(TabNavigationItem)] as Style;
#endif
#endif

            DefaultStyleKey = typeof(TabNavigationItem);
            this.Loaded += new RoutedEventHandler(TabNavigationItem_Loaded);
        }

        private void TabNavigationItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (parent != null)
            {
                parent.UpdateRect(parent.ActualWidth, parent.ActualHeight);
                UpdateVisualState();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(TabNavigationItem), new PropertyMetadata(null, new PropertyChangedCallback(OnContentChanged)));

        /// <summary>
        ///
        /// </summary>
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(TabNavigationItem), new PropertyMetadata(null));

        /// <summary>
        ///
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(TabNavigationItem), new PropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));

        private NavigationCommand selectCommand;

        /// <summary>
        ///
        /// </summary>
        public NavigationCommand SelectCommand
        {
            get
            {
                if (selectCommand == null)
                {
                    selectCommand = new NavigationCommand(param => ExecuteSelectCommand());
                }
                return selectCommand;
            }
        }

        private void ExecuteSelectCommand()
        {
            if (parent != null)
            {
                TabSelectionChangingEventArgs changingEventArgs = new TabSelectionChangingEventArgs()
                {
                    OldItem = parent.ItemContainerGenerator.ContainerFromIndex(parent.SelectedIndex),
                    NewItem = parent.ItemContainerGenerator.ContainerFromIndex(parent.Items.IndexOf(this)),
                };

                this.parent.OnSelectionChanging(changingEventArgs);

                if (!changingEventArgs.Cancel)
                    parent.SelectedIndex = parent.Items.IndexOf(this);
                else
                    return;

                TabSelectionChangedEventArgs eventArgs = new TabSelectionChangedEventArgs()
                {
                    OldItem = parent.oldSelectedIndex == -1 ? null : parent.ItemContainerGenerator.ContainerFromIndex(parent.oldSelectedIndex),
                    NewItem = parent.ItemContainerGenerator.ContainerFromIndex(parent.SelectedIndex),
                };

                parent.OnSelectedChanged(eventArgs);
            }
        }

        private static void OnIsSelectedChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            TabNavigationItem instance = sender as TabNavigationItem;
            if (((bool)args.NewValue) == true)
            {
                if (instance.parent != null)
                {
                    instance.parent.SelectedTabItem = instance;
                }
            }
            instance.UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            if (IsSelected)
            {
                VisualStateManager.GoToState(this, "Selected", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "UnSelected", false);
            }
        }

        private static void OnContentChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            TabNavigationItem instance = sender as TabNavigationItem;
            if (args.NewValue is UIElement)
            {
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class NavigationCommand : ICommand
    {
        /// <summary>
        ///
        /// </summary>
        public event EventHandler CanExecuteChanged;

        private readonly Predicate<Object> _canExecute = null;
        private readonly Action<Object> _executeAction = null;

        /// <summary>
        ///
        /// </summary>
        /// <param name="executeAction"></param>
        /// <param name="canExecute"></param>
        public NavigationCommand(Action<object> executeAction, Predicate<Object> canExecute)
        {
            _executeAction = executeAction;
            _canExecute = canExecute;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="executeAction"></param>
        public NavigationCommand(Action<object> executeAction)
            : this(executeAction, null)
        {
            _executeAction = executeAction;
        }

        /// <summary>
        ///
        /// </summary>
        public void UpdateCanExecute()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            if (_executeAction != null)
                _executeAction(parameter);
            UpdateCanExecute();
        }
    }
}