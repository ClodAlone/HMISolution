using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Opc.Ua;
using Opc.Ua.Client;
using ServerEditor.Services;
using ServerEditor.ViewModels;

namespace ServerEditor.Controls
{
    public partial class OpcMonitorControl : UserControl
    {
        private Subscription? _subscription;
        public ObservableCollection<MonitoredItemViewModel> Items { get; } = new();

        public OpcMonitorControl()
        {
            InitializeComponent();
            MonitorGrid.ItemsSource = Items;
            // Subscribe to global selection
            SelectionService.Instance.SelectionChanged += OnSelectionChanged;
            OpcSessionManager.Instance.SessionErrorMessage += OnSessionError;
        }

        private void OnSessionError(object? sender, string message)
        {
            Dispatcher.Invoke(() =>
            {
                Items.Clear();
                var errItem = new MonitoredItemViewModel { DisplayName = "Error", Value = message };
                Items.Add(errItem);
                MonitorGrid.IsEnabled = false;
            });
        }

        private async void OnSelectionChanged(object? obj)
        {
            // Clean up old subscription
             if (_subscription != null)
             {
                 try
                 {
                     _subscription.RemoveItems(_subscription.MonitoredItems);
                     _subscription.Delete(true);
                 }
                 catch { }
                 _subscription = null;
             }
             
             Items.Clear();

            // Check if selected object is OpcNodeViewModel
             if (obj is OpcNodeViewModel nodeVm && nodeVm.Session != null && nodeVm.Session.Connected)
             {
                 MonitorGrid.IsEnabled = true;
                 try
                 {
                     // Ensure loaded (if folder, might load children)
                     // If nodeVm is a folder, we want to monitor its variables (children)
                     // If nodeVm is a variable, monitor it.
                     
                     // Need to load children if not loaded?
                     // OpcNodeViewModel handles loading if expanded.
                     // But here we might want to ensure children are loaded regardless of expansion?
                     // Let's call LoadChildrenAsync() but allow it to no-op if loaded.
                     // My implementation of LoadChildrenAsync checks _childrenLoaded.
                     await nodeVm.LoadChildrenAsync();

                     _subscription = new Subscription(nodeVm.Session.DefaultSubscription)
                     {
                         PublishingInterval = 1000,
                         PublishingEnabled = true
                     };
                     
                     nodeVm.Session.AddSubscription(_subscription);
                     _subscription.Create();

                     if (IsVariable(nodeVm))
                     {
                         AddMonitoredItem(nodeVm);
                     }

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
                     System.Diagnostics.Debug.WriteLine($"Monitor Error: {ex.Message}");
                 }
             }
        }

        private bool IsVariable(OpcNodeViewModel vm)
        {
             if (vm.Reference == null) return false;
             return vm.Reference.NodeClass == NodeClass.Variable;
        }

        private void AddMonitoredItem(OpcNodeViewModel vm)
        {
            if (_subscription == null) return;
            
            var item = new MonitoredItem(_subscription.DefaultItem)
            {
                DisplayName = vm.DisplayName,
                StartNodeId = vm.NodeId,
                AttributeId = Attributes.Value
            };

            var itemVm = new MonitoredItemViewModel();
            itemVm.DisplayName = vm.DisplayName;
            itemVm.Value = "Pending...";

            Opc.Ua.Client.ISession session = _subscription.Session;

            _ = Task.Run(async () =>
            {
                var writable = await IsWritableAsync(session, vm.NodeId);
                Dispatcher.Invoke(() => itemVm.IsWritable = writable);
            });

            itemVm.WriteHandler = async (newValue) =>
            {
                try
                {
                    if (!itemVm.IsWritable)
                    {
                        return false;
                    }

                    // Read the node's DataType so we can convert the string to the correct type.
                    var nodesToRead = new ReadValueIdCollection
                    {
                        new ReadValueId { NodeId = vm.NodeId, AttributeId = Attributes.DataType }
                    };
                    DataValueCollection readResults = null!;
                    DiagnosticInfoCollection readDiag = null!;
                    await Task.Run(() => session.Read(null, 0, TimestampsToReturn.Neither, nodesToRead, out readResults, out readDiag));

                    var typedValue = ConvertToExpectedType(
                        readResults.Count > 0 && StatusCode.IsGood(readResults[0].StatusCode)
                            ? readResults[0].Value as NodeId
                            : null,
                        newValue);

                    var nodesToWrite = new WriteValueCollection();
                    var writeValue = new WriteValue
                    {
                         NodeId = vm.NodeId,
                         AttributeId = Attributes.Value,
                         Value = new DataValue(new Variant(typedValue))
                    };

                    nodesToWrite.Add(writeValue);

                    StatusCodeCollection results = null;
                    DiagnosticInfoCollection diagnosticInfos = null;

                    await Task.Run(() => session.Write(null, nodesToWrite, out results, out diagnosticInfos));

                    if (StatusCode.IsBad(results[0]))
                    {
                         MessageBox.Show($"Write failed: {results[0]}", "OPC UA Write", MessageBoxButton.OK, MessageBoxImage.Error);
                         return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Write error: {ex.Message}", "OPC UA Write", MessageBoxButton.OK, MessageBoxImage.Error);
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
                         var sc = notification.Value.StatusCode;
                         var text = Opc.Ua.StatusCodes.GetBrowseName(sc.Code);
                         if (text == "Unknown") text = $"{sc} (0x{sc.Code:X8})";
                         
                         itemVm.UpdateFromNotification(
                             notification.Value.WrappedValue.ToString(),
                             text,
                             notification.Value.SourceTimestamp.ToLocalTime().ToString("HH:mm:ss.fff"));
                     });
                 }
            };

            _subscription.AddItem(item);
        }

        private static Task<bool> IsWritableAsync(Opc.Ua.Client.ISession session, NodeId nodeId)
        {
            try
            {
                var nodesToRead = new ReadValueIdCollection
                {
                    new ReadValueId { NodeId = nodeId, AttributeId = Attributes.UserAccessLevel }
                };

                DataValueCollection results;
                DiagnosticInfoCollection diagnosticInfos;

                session.Read(null, 0, TimestampsToReturn.Neither, nodesToRead, out results, out diagnosticInfos);

                if (results.Count > 0 && StatusCode.IsGood(results[0].StatusCode))
                {
                    if (results[0].Value is byte b)
                    {
                        return Task.FromResult((b & AccessLevels.CurrentWrite) == AccessLevels.CurrentWrite);
                    }
                }
            }
            catch
            {
                // Ignore and treat as read-only.
            }
            return Task.FromResult(false);
        }

        private static object ConvertToExpectedType(NodeId? dataTypeId, string value)
        {
            if (dataTypeId == null)
                return value;

            var id = dataTypeId.Identifier is uint uid ? uid : 0u;
            return id switch
            {
                DataTypes.Boolean => bool.Parse(value),
                DataTypes.SByte => sbyte.Parse(value),
                DataTypes.Byte => byte.Parse(value),
                DataTypes.Int16 => short.Parse(value),
                DataTypes.UInt16 => ushort.Parse(value),
                DataTypes.Int32 => int.Parse(value),
                DataTypes.UInt32 => uint.Parse(value),
                DataTypes.Int64 => long.Parse(value),
                DataTypes.UInt64 => ulong.Parse(value),
                DataTypes.Float => float.Parse(value),
                DataTypes.Double => double.Parse(value),
                DataTypes.DateTime => DateTime.Parse(value),
                _ => value
            };
        }
    }

    public class MonitoredItemViewModel : ViewModelBase
    {
        private string _value = "";
        private string _statusCode = "";
        private string _sourceTimestamp = "";
        private bool _isUpdatingExternal = false;
        private string _lastServerValue = "";
        private bool _isWritable;

        public string DisplayName { get; set; } = "";
        
        public Func<string, Task<bool>>? WriteHandler { get; set; }

        public ICommand WriteCommand { get; }

        public MonitoredItemViewModel()
        {
            WriteCommand = new RelayCommand(async p => await CommitWriteAsync(p as string));
        }

        public bool IsWritable
        {
            get => _isWritable;
            set => SetProperty(ref _isWritable, value);
        }

        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    SetProperty(ref _value, value);
                }
            }
        }

        private async Task CommitWriteAsync(string? newValue)
        {
            if (_isUpdatingExternal) return;
            if (WriteHandler == null) return;
            if (!IsWritable) return;

            newValue ??= Value;
            var oldValue = _lastServerValue;

            var success = await WriteHandler(newValue);
            if (!success)
            {
                // MessageBox.Show("Write failed.", "OPC UA Write", MessageBoxButton.OK, MessageBoxImage.Error);
                _isUpdatingExternal = true;
                Value = oldValue;
                _isUpdatingExternal = false;
            }
            else
            {
                _lastServerValue = newValue;
            }
        }

        public void UpdateFromNotification(string value, string statusCode, string sourceTimestamp)
        {
            _isUpdatingExternal = true;
            Value = value;
            _lastServerValue = value;
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