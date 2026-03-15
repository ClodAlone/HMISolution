using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Opc.Ua;
using Opc.Ua.Client;
using WpfApp3.Services;
using WpfApp3.ViewModels;

namespace WpfApp3.Plugins
{
    public partial class OpcMonitorControl : UserControl
    {
        private Subscription? _subscription;
        public ObservableCollection<MonitoredItemViewModel> Items { get; } = new();

        public OpcMonitorControl()
        {
            InitializeComponent();
            MonitorGrid.ItemsSource = Items;
            SelectionService.Instance.SelectionChanged += OnSelectionChanged;
        }

        private async void OnSelectionChanged(object? obj)
        {
            // Cleanup existing subscription
            if (_subscription != null)
            {
                // We typically remove items or delete subscription.
                // Since _subscription is attached to the session of the previous node,
                // and we might be switching sessions or just nodes.
                try 
                {
                    _subscription.Delete(true);
                    _subscription.RemoveItems(_subscription.MonitoredItems);
                } 
                catch { }
                _subscription = null;
            }

            Items.Clear();

            if (obj is OpcNodeViewModel nodeVm && nodeVm.Session != null && nodeVm.Session.Connected)
            {
                try
                {
                    // Ensure children are loaded so we can monitor variables in this folder
                    await nodeVm.LoadChildrenAsync();

                    _subscription = new Subscription(nodeVm.Session.DefaultSubscription)
                    {
                        PublishingInterval = 1000,
                        PublishingEnabled = true
                    };

                    nodeVm.Session.AddSubscription(_subscription);
                    _subscription.Create();

                    // Add items: Selected Node (if Variable) + Children (if Variables)
                    if (IsVariable(nodeVm))
                    {
                        AddMonitoredItem(nodeVm);
                    }

                    // Check loaded children
                    foreach (var child in nodeVm.Children)
                    {
                         if (IsVariable(child))
                         {
                             AddMonitoredItem(child);
                         }
                    }

                    _subscription.ApplyChanges();
                }
                catch (Exception ex)
                {
                    // Handle error
                    System.Diagnostics.Debug.WriteLine($"Monitor Error: {ex.Message}");
                }
            }
        }

        private bool IsVariable(OpcNodeViewModel vm)
        {
             // Check Reference NodeClass or assume from NodeId if possible, but Reference is better
             if (vm.Reference == null) return false;
             return vm.Reference.NodeClass == NodeClass.Variable;
        }

        private void AddMonitoredItem(OpcNodeViewModel vm)
        {
            if (_subscription == null) return;
            var session = _subscription.Session;

            var item = new MonitoredItem(_subscription.DefaultItem)
            {
                DisplayName = vm.DisplayName,
                StartNodeId = vm.NodeId,
                AttributeId = Attributes.Value
            };

            var itemVm = new MonitoredItemViewModel();
            itemVm.DisplayName = vm.DisplayName;
            itemVm.Value = "Pending...";

            itemVm.WriteHandler = async (newValue) =>
            {
                try
                {
                    var nodesToWrite = new WriteValueCollection();
                    var writeValue = new WriteValue
                    {
                         NodeId = vm.NodeId,
                         AttributeId = Attributes.Value,
                         Value = new DataValue(new Variant(Convert.ChangeType(newValue, Type.GetType("System.String")))) // Simplistic assumption: String
                    };

                     // Simple write as string for now, OPC UA usually requires correct type. 
                     // We would need to read DataType first to do this properly.
                     // Or assume the server can convert.
                     writeValue.Value.WrappedValue = new Variant(newValue);

                    nodesToWrite.Add(writeValue);

                    StatusCodeCollection results = null;
                    DiagnosticInfoCollection diagnosticInfos = null;

                    await Task.Run(() => session.Write(null, nodesToWrite, out results, out diagnosticInfos));

                    if (StatusCode.IsBad(results[0]))
                    {
                         MessageBox.Show($"Write failed: {results[0]}");
                         return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Write error: {ex.Message}");
                    return false;
                }
            };
            
            Items.Add(itemVm);

            item.Notification += (monitoredItem, args) =>
            {
                 if (args.NotificationValue is MonitoredItemNotification notification && notification.Value != null)
                 {
                     Dispatcher.Invoke(() =>
                     {
                         itemVm.UpdateFromNotification(
                             notification.Value.WrappedValue.ToString(),
                             notification.Value.StatusCode.ToString(),
                             notification.Value.SourceTimestamp.ToLocalTime().ToString("HH:mm:ss.fff"));
                     });
                 }
            };

            _subscription.AddItem(item);
        }
    }

    public class MonitoredItemViewModel : ViewModelBase
    {
        private string _value = "";
        private string _statusCode = "";
        private string _sourceTimestamp = "";
        private bool _isUpdatingExternal = false;

        public string DisplayName { get; set; } = "";
        
        public Func<string, Task<bool>>? WriteHandler { get; set; }

        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                     // If we are updating from notification, just set the value
                     if (_isUpdatingExternal)
                     {
                         SetProperty(ref _value, value);
                     }
                     else
                     {
                         // User edit
                         var oldValue = _value;
                         SetProperty(ref _value, value);
                         
                         // Trigger write
                         if (WriteHandler != null)
                         {
                             // Fire and forget, or handle errors?
                             // Ideally we should wait, but setter is sync.
                             _ = HandleWriteAsync(value, oldValue);
                         }
                     }
                }
            }
        }

        private async Task HandleWriteAsync(string newValue, string oldValue)
        {
            if (WriteHandler != null)
            {
                var success = await WriteHandler(newValue);
                if (!success)
                {
                    // Revert on failure? Or let next notification update it?
                    // Let's force revert for feedback
                    _isUpdatingExternal = true;
                    Value = oldValue;
                    _isUpdatingExternal = false;
                }
            }
        }

        public void UpdateFromNotification(string value, string statusCode, string sourceTimestamp)
        {
            _isUpdatingExternal = true;
            Value = value;
            StatusCode = statusCode;
            SourceTimestamp = sourceTimestamp;
            _isUpdatingExternal = false;
        }

        public string StatusCode
        {
            get => _statusCode;
            set => SetProperty(ref _statusCode, value);
        }

        public string SourceTimestamp
        {
            get => _sourceTimestamp;
            set => SetProperty(ref _sourceTimestamp, value);
        }
    }
}
