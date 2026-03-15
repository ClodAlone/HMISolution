#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Linq;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Syncfusion.Windows.Tools.Controls;
using System.Collections.Generic;
using Syncfusion.Windows.Tools.Controls.Resources;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class QatCustomizationDialog : ChildWindow, INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ribbon"></param>
        /// <param name="toolbar"></param>
        public QatCustomizationDialog(Ribbon ribbon, QuickAccessToolBar toolbar)
        {
            _ribbon = ribbon;
            _toolbar = toolbar;
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(QatCustomizationDialog_Loaded);
            AddedItems = new ObservableCollection<RibbonCommandProvider>();
            RemovedItems = new ObservableCollection<RibbonCommandProvider>();

            this.QATStateCaption = (new ResourceWrapper()).QATShowBelow;

            this.Title = (new Syncfusion.Windows.Tools.Controls.Resources.ResourceWrapper()).QATPopupHeader;
            if (RibbonCommandManager.CommandDictionary.Count != 0)
                foreach (string key in RibbonCommandManager.GroupDictionary.Keys)
                {
                    PART_ComboBox.Items.Add(key);
                }

            foreach (var _tab in _ribbon.Items)
            {
                RibbonTab tab = _tab as RibbonTab;
                if (tab != null)
                {
                    foreach (var barItem in tab.Items)
                    {
                        RibbonBar bar = barItem as RibbonBar;
                        if (bar != null)
                        {
                            var query = from FrameworkElement element in bar.Items
                                        where element is ICommandSource
                                        select element;

                            foreach (var i in query)
                            {
                                IRibbonItem item = i as IRibbonItem;
                                if (item != null)
                                {
                                    if (((ICommandSource)item).Command != null)
                                    {
                                        if (!(RibbonCommandManager.CommandDictionary.ContainsKey(((ICommandSource)item).Command)))
                                        {
                                            var rbnCmdPrvdr = new RibbonCommandProvider(tab.Caption, item.Label, item.SmallIcon, ToolTipService.GetToolTip(item as DependencyObject));
                                            if (item is UIElement)
                                                rbnCmdPrvdr.Host = CloneManager.CloneGeneral(item as UIElement, false);
                                            RibbonCommandManager.Register(((ICommandSource)item).Command, rbnCmdPrvdr);
                                        }
                                    }
                                }
                            }


                            var query1 = from item in bar.Items
                                         where item is ButtonPanel
                                         select item;

                            foreach (ButtonPanel element in query1)
                            {
                                foreach (var i in element.Items)
                                {
                                    if (i is ICommandSource)
                                    {
                                        IRibbonItem item = i as IRibbonItem;
                                        if (item != null)
                                        {
                                            if (((ICommandSource)item).Command != null)
                                            {
                                                if (!(RibbonCommandManager.CommandDictionary.ContainsKey(((ICommandSource)item).Command)))
                                                {
                                                    var rbnCmdPrvdr = new RibbonCommandProvider(tab.Caption, item.Label, item.SmallIcon, ToolTipService.GetToolTip(item as DependencyObject));
                                                    if (item is UIElement)
                                                        rbnCmdPrvdr.Host = CloneManager.CloneGeneral(item as UIElement, false);
                                                    RibbonCommandManager.Register(((ICommandSource)item).Command, rbnCmdPrvdr);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //PART_ComboBox.Items.Add(_groupSeparator);

            if (RibbonCommandManager.CommandDictionary.Count != 0)
                foreach (string key in RibbonCommandManager.GroupDictionary.Keys)
                {
                    if (!PART_ComboBox.Items.Contains(key))
                    {
                        PART_ComboBox.Items.Add(key);
                    }
                }

            RibbonCommandManager.GroupDictionary["All Commands"] = RibbonCommandManager.CommandDictionary;

            if (PART_ComboBox.Items.Count > 0)
            {
                PART_ComboBox.SelectedIndex = 0;
            }
            PART_ListSource.SelectionChanged += new SelectionChangedEventHandler(UpdateExecute);
            PART_ListDestination.SelectionChanged += new SelectionChangedEventHandler(UpdateExecute);

        }

        void QatCustomizationDialog_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeDestination();
            Initialize();
            UpdateAddRemoveCanExecute();
            UpdateUpDownCanExecute();
            if (Destination.Count == 0 && Source.Count == 0)
            {
                PART_btnReset.IsEnabled = false;
            }
            else
            {
                PART_btnReset.IsEnabled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string QATStateCaption
        {
            get { return (string)GetValue(QATStateCaptionProperty); }
            set { SetValue(QATStateCaptionProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for QATStateCaption.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty QATStateCaptionProperty =
            DependencyProperty.Register("QATStateCaption", typeof(string), typeof(QatCustomizationDialog), new PropertyMetadata(string.Empty));

        private Ribbon _ribbon;

        private string _groupSeparator = "------------------------------";

        private QuickAccessToolBar _toolbar;

        internal ObservableCollection<RibbonCommandProvider> _source = new ObservableCollection<RibbonCommandProvider>();

        internal ObservableCollection<RibbonCommandProvider> _destination = new ObservableCollection<RibbonCommandProvider>();

        internal Dictionary<object, RibbonCommandProvider> QATItemCollections = new Dictionary<object, RibbonCommandProvider>();

        /// <summary>
        /// 
        /// </summary>
        public RibbonCommand Add
        {
            get
            {
                return new RibbonCommand(param => AddExecute());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public RibbonCommand Remove
        {
            get
            {
                return new RibbonCommand(param => RemoveExecute());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public RibbonCommand Up
        {
            get
            {
                return new RibbonCommand(param => UpExecute());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public RibbonCommand Down
        {
            get
            {
                return new RibbonCommand(param => DownExecute());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public RibbonCommand Reset
        {
            get
            {
                return new RibbonCommand(param => ResetExecute());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public RibbonCommand Ok
        {
            get
            {
                return new RibbonCommand(param => OkExecute());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public RibbonCommand Cancel
        {
            get
            {
                return new RibbonCommand(param => CancelExecute());
            }
        }

        internal ObservableCollection<RibbonCommandProvider> AddedItems
        {
            get;
            set;
        }

        internal ObservableCollection<RibbonCommandProvider> RemovedItems
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<RibbonCommandProvider> Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Source"));
                }
            }
        }

        internal bool? QATState
        {
            get
            {
                return PART_chkShowQATBelow.IsChecked;
            }
            set
            {
                PART_chkShowQATBelow.IsChecked = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public new bool DialogResult
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<RibbonCommandProvider> Destination
        {
            get
            {
                return _destination;
            }
            set
            {
                _destination = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Destination"));
                }
            }
        }

        private void Initialize()
        {
            if (PART_ListSource.Items.Count > 0)
            {
                PART_ListSource.Focus();
                PART_ListSource.SelectedIndex = 0;
            }

            if (PART_ListDestination.Items.Count > 0)
            {
                PART_ListDestination.Focus();
                PART_ListDestination.SelectedIndex = 0;
            }
        }

        private void InitializeDestination()
        {
            AddedItems.Clear();
            RemovedItems.Clear();
            QATItemCollections.Clear();
            if (_destination.Count > 0)
            {
                _destination.Clear();
            }
            if (_toolbar != null)
            {
                foreach (var item in _toolbar.Items)
                {
                    var _ribbonitem = item as IRibbonItem;
                    if (_ribbonitem != null && _ribbonitem is IRibbonItem)
                    {
                        RibbonCommandProvider _provider = new RibbonCommandProvider("QAT Commands", _ribbonitem.Label, _ribbonitem.SmallIcon, ToolTipService.GetToolTip(_ribbonitem as DependencyObject));
                        if (_ribbonitem is ICommandSource)
                        {
                            _provider.Command = ((ICommandSource)_ribbonitem).Command;
                            _provider.CommandParameter = ((ICommandSource)_ribbonitem).CommandParameter;                          
                            _provider.Host = item as UIElement;
                        }
                        _destination.Add(_provider);
                        if (!QATItemCollections.ContainsKey(item) && !QATItemCollections.ContainsKey(_provider))
                            QATItemCollections.Add(item, _provider);
                    }
                    else if (item is RibbonSplitButton)
                    {
                        var _splitbutton = item as RibbonSplitButton;
                        RibbonCommandProvider _provider = new RibbonCommandProvider("QAT Commands", _splitbutton.Label, _splitbutton.SmallIcon, ToolTipService.GetToolTip(_splitbutton as DependencyObject));
                        if (_splitbutton is ICommandSource)
                        {
                            _provider.Command = ((ICommandSource)_splitbutton).Command;
                            _provider.CommandParameter = ((ICommandSource)_splitbutton).CommandParameter;                          
                            _provider.IsItemsHost = true;                           
                            _provider.Host = item as UIElement;
                        }
                        _destination.Add(_provider);
                        if (!QATItemCollections.ContainsKey(item) && !QATItemCollections.ContainsKey(_provider))
                            QATItemCollections.Add(item, _provider);
                    }
                    else if (item is RibbonCommandProvider)
                    {
                        _destination.Add(item as RibbonCommandProvider);
                        if (!QATItemCollections.ContainsKey(item) && !QATItemCollections.ContainsKey((RibbonCommandProvider)item))
                            QATItemCollections.Add(item, item as RibbonCommandProvider);
                    }
                    else
                    {
                        var rbnCmdPrvdr = new RibbonCommandProvider();
                        if (item is FrameworkElement && ((FrameworkElement)item).Name != string.Empty)
                            rbnCmdPrvdr.Label = ((FrameworkElement)item).Name;
                        else
                            rbnCmdPrvdr.Label = item.GetType().Name;
                        _destination.Add(rbnCmdPrvdr);
                        if (!QATItemCollections.ContainsKey(item) && !QATItemCollections.ContainsKey(rbnCmdPrvdr))
                            QATItemCollections.Add(item, rbnCmdPrvdr);
                    }
                    PART_ListDestination.SelectionChanged += new SelectionChangedEventHandler(PART_ListDestination_SelectionChanged);
                    PART_ListDestination.Focus();
                }

                foreach (var item in _toolbar.OverFlowItems)
                {
                    var _ribbonitem = item as IRibbonItem;
                    if (_ribbonitem != null && _ribbonitem is IRibbonItem)
                    {
                        RibbonCommandProvider _provider = new RibbonCommandProvider("QAT Commands", _ribbonitem.Label, _ribbonitem.SmallIcon, ToolTipService.GetToolTip(_ribbonitem as DependencyObject));
                        if (_ribbonitem is ICommandSource)
                        {
                            _provider.Command = ((ICommandSource)_ribbonitem).Command;
                            _provider.CommandParameter = ((ICommandSource)_ribbonitem).CommandParameter;
                            if (_ribbonitem is RibbonSplitButton)
                            {
                                _provider.IsItemsHost = true;
                            }
                            _provider.Host = item as UIElement;
                        }
                        if (!QATItemCollections.ContainsKey(item) && !QATItemCollections.ContainsKey(_provider))
                            QATItemCollections.Add(item, _provider);
                        _destination.Add(_provider);
                    }
                }
            }
        }

        void PART_ListDestination_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUpDownCanExecute();
        }

        void UpdateExecute(object sender, SelectionChangedEventArgs e)
        {
            UpdateAddRemoveCanExecute();
        }

        private void UpdateAddRemoveCanExecute()
        {
            PART_btnAdd.IsEnabled = PART_ListSource.SelectedItem != null &&
                                    !Destination.Contains(((RibbonCommandProvider)PART_ListSource.SelectedItem)) && PART_ListSource.Items.Count > 0;
            PART_btnRemove.IsEnabled = PART_ListDestination.SelectedItem != null && PART_ListDestination.Items.Count > 0;
        }

        private void PartComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PART_ComboBox.SelectedItem != null)
            {
                if (PART_ComboBox.SelectedItem.ToString() != _groupSeparator)
                {
                    if (RibbonCommandManager.GroupDictionary[PART_ComboBox.SelectedItem.ToString()] != null)
                    {
                        Source.Clear();
                        foreach (var item in RibbonCommandManager.GroupDictionary[PART_ComboBox.SelectedItem.ToString()].Values)
                        {
                            Source.Add(item);
                        }
                    }
                }
                else
                {
                    PART_ComboBox.SelectedIndex = 0;
                }
            }
        }

        private void AddExecute()
        {
            if (PART_ListSource.SelectedItem != null)
            {
                int index = PART_ListSource.SelectedIndex;
                RibbonCommandProvider commandprovider = PART_ListSource.SelectedItem as RibbonCommandProvider;

                var query = from provider in Destination
                            where (provider.Command == commandprovider.Command && provider.CommandParameter == commandprovider.CommandParameter) || (provider.Command == commandprovider.Command && provider.CommandParameter == null && commandprovider.CommandParameter == null)
                            select provider;

                if (query.Count() == 0)
                {
                    Destination.Add(commandprovider);
                    UpdateAddRemoveCanExecute();
                    AddedItems.Add(((RibbonCommandProvider)PART_ListSource.SelectedItem));
                    if (index != PART_ListSource.Items.Count - 1)
                    {
                        PART_ListSource.SelectedIndex = index + 1;
                    }
                }
                else
                {
                    MessageBox.Show("The Selected Command is already on the Quick Access Toolbar.", "Quick Access Toolbar", MessageBoxButton.OK);
                }
            }
        }

        private void RemoveExecute()
        {
            if (PART_ListDestination.SelectedItem != null)
            {
                if (AddedItems.Contains((RibbonCommandProvider)PART_ListDestination.SelectedItem))
                {
                    AddedItems.Remove(((RibbonCommandProvider)PART_ListDestination.SelectedItem));
                }
                else
                {
                    RemovedItems.Add(((RibbonCommandProvider)PART_ListDestination.SelectedItem));
                }
                int index = PART_ListDestination.SelectedIndex;
                Destination.Remove(((RibbonCommandProvider)PART_ListDestination.SelectedItem));
                if (index != 0)
                {
                    PART_ListDestination.SelectedIndex = index - 1;
                }
            }
        }

        private void OkExecute()
        {
            if (_toolbar != null)
            {
                _toolbar.Items.Clear();
                _toolbar.parentRibbon.QATItems.Clear();
                foreach (var item in Destination)
                {
                    IRibbonItem element;
                    element = new RibbonButton();
                    var cmdpro = (RibbonCommandProvider)item;
                    element.Label = cmdpro.Label;
                    element.SmallIcon = cmdpro.SmallIcon;
                    element.SizeMode = SizeMode.Small;
                    ((RibbonButton)element)._provider = item;
                    ToolTipService.SetToolTip(element as DependencyObject, cmdpro.ToolTip);
                    ((RibbonButton)element).Command = cmdpro.Command;
                    ((RibbonButton)element).CommandParameter = cmdpro.CommandParameter;
                    _toolbar.ClearOverFlowItems();
                    if (item.Host != null)
                    {
                        //_toolbar.Items.Add(element);
                        _toolbar.Items.Add(item.Host);
                        _toolbar.parentRibbon.QATItems.Add(item.Host, item.Host);
                    }
                    else
                    {
                        if (item != null)
                        {
                            var getQatItem = from res in this.QATItemCollections
                                             where res.Value != null && res.Value == item
                                             select res;
                            if (getQatItem.Count() > 0)
                            {
                                _toolbar.Items.Add(getQatItem.FirstOrDefault().Key);
                                _toolbar._parentRibbon.QATItems.Add((UIElement)getQatItem.FirstOrDefault().Key, (UIElement)getQatItem.FirstOrDefault().Key);
                            }
                            else if (item is RibbonCommandProvider)
                            {
                                _toolbar.Items.Add(element);
                                _toolbar.parentRibbon.QATItems.Add((UIElement)element,(UIElement)element);
                            }
                        }
                    }
                }
            }
            DialogResult = true;
            if (QATItemCollections != null) QATItemCollections.Clear();
            this.Close();
        }

        private void CancelExecute()
        {
            DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Builds the visual tree for the <see cref="T:System.Windows.Controls.ChildWindow"/> control when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        private void UpExecute()
        {
            RibbonCommandProvider _command = PART_ListDestination.SelectedItem as RibbonCommandProvider;
            int index = Destination.IndexOf(_command);
            if (index != -1)
            {
                Destination.RemoveAt(index);
                Destination.Insert(index - 1, _command);
                PART_ListDestination.SelectedIndex = index - 1;
            }
            UpdateUpDownCanExecute();
        }

        private void DownExecute()
        {
            RibbonCommandProvider _command = PART_ListDestination.SelectedItem as RibbonCommandProvider;
            int index = Destination.IndexOf(_command);
            if (index != -1 && index != Destination.Count - 1)
            {
                Destination.RemoveAt(index);
                Destination.Insert(index + 1, _command);
                PART_ListDestination.SelectedIndex = index + 1;

            }
            UpdateUpDownCanExecute();
        }

        private void UpdateUpDownCanExecute()
        {
            if (PART_ListDestination.Items.Count > 0)
            {
                if (PART_ListDestination.SelectedIndex == 0)
                {
                    PART_btnUp.IsEnabled = false;
                }
                else
                {
                    PART_btnUp.IsEnabled = true;
                }

                if (PART_ListDestination.SelectedIndex == PART_ListDestination.Items.Count - 1)
                {
                    PART_btnDown.IsEnabled = false;
                }
                else
                {
                    PART_btnDown.IsEnabled = true;
                }
            }
            else
            {
                PART_btnUp.IsEnabled = PART_btnDown.IsEnabled = false;
            }
        }

        private void ResetExecute()
        {
            if (MessageBox.Show("Do you want to reset the Quick Access Toolbar customizations for this program?", "QAT Customization", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                InitializeDestination();
            }
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.KeyDown"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event. </param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CancelExecute();
            }
            if (e.Key == Key.Enter)
            {
                OkExecute();
            }
            base.OnKeyDown(e);
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class RibbonCommand : ICommand
    {
        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        readonly Predicate<Object> _canExecute = null;
        readonly Action<Object> _executeAction = null;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="executeAction"></param>
        /// <param name="canExecute"></param>
        public RibbonCommand(Action<object> executeAction, Predicate<Object> canExecute)
        {
            _executeAction = executeAction;
            _canExecute = canExecute;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="executeAction"></param>
        public RibbonCommand(Action<object> executeAction)
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
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }


        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        public void Execute(object parameter)
        {
            if (_executeAction != null)
                _executeAction(parameter);
            UpdateCanExecute();
        }
    }
}
