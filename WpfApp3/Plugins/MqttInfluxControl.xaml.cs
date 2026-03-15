using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DataLogger;
using WpfApp3.Services;
using WpfApp3.ViewModels;

namespace WpfApp3.Plugins
{
    public partial class MqttInfluxControl : UserControl
    {
        private MqttInfluxBridge? _bridge;
        private OpcInfluxBridge? _opcBridge;
        private System.Collections.ObjectModel.ObservableCollection<string> _monitoredNodes = new();

        public MqttInfluxControl()
        {
            InitializeComponent();
            OpcNodesList.ItemsSource = _monitoredNodes;
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (_bridge == null)
            {
                var config = GetConfig();

                _bridge = new MqttInfluxBridge(config);
                try
                {
                    await _bridge.StartAsync();
                    StatusBlock.Text = "Running...";
                    ConnectButton.Content = "Stop Bridge";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error starting bridge: {ex.Message}");
                    _bridge = null;
                }
            }
            else
            {
                await _bridge.StopAsync();
                _bridge.Dispose();
                _bridge = null;
                StatusBlock.Text = "Stopped";
                ConnectButton.Content = "Start Bridge";
            }
        }

        private void AddSelection_Click(object sender, RoutedEventArgs e)
        {
            var selection = SelectionService.Instance.SelectedObjects.OfType<OpcNodeViewModel>().ToList();
            if (!selection.Any())
            {
                MessageBox.Show("Please select nodes in the OPC Browse plugin first.");
                return;
            }

            foreach (var node in selection)
            {
                // Assuming NodeId.ToString() gives us what we need (e.g. "ns=2;s=MyNode")
                var id = node.NodeId.ToString();
                if (!_monitoredNodes.Contains(id))
                {
                    _monitoredNodes.Add(id);
                }
            }
        }

        private void RemoveNode_Click(object sender, RoutedEventArgs e)
        {
            var selected = OpcNodesList.SelectedItems.Cast<string>().ToList();
            foreach (var item in selected)
            {
                _monitoredNodes.Remove(item);
            }
        }

        private async void OpcConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (_opcBridge == null)
            {
                if (!_monitoredNodes.Any())
                {
                    MessageBox.Show("Please add at least one node to monitor.");
                    return;
                }

                var config = GetOpcConfig();

                _opcBridge = new OpcInfluxBridge(config);
                try
                {
                    await _opcBridge.StartAsync();
                    OpcStatusBlock.Text = "Running...";
                    OpcConnectButton.Content = "Stop OPC Bridge";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error starting OPC bridge: {ex.Message}");
                    _opcBridge = null;
                }
            }
            else
            {
                _opcBridge.Stop();
                _opcBridge.Dispose();
                _opcBridge = null;
                OpcStatusBlock.Text = "Stopped";
                OpcConnectButton.Content = "Start OPC Bridge";
            }
        }

        private OpcInfluxConfig GetOpcConfig()
        {
            return new OpcInfluxConfig
            {
                OpcServerUrl = OpcUrlBox.Text,
                NodeIds = _monitoredNodes.ToList(),
                InfluxUrl = OpcInfluxUrlBox.Text,
                InfluxToken = OpcTokenBox.Password,
                InfluxBucket = OpcBucketBox.Text,
                InfluxOrg = OpcOrgBox.Text
            };
        }

        private async void QueryButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var config = GetConfig();
                var range = RangeBox.Text;
                
                var results = await MqttInfluxBridge.QueryAsync(config, range);
                ResultsGrid.ItemsSource = results;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Query failed: {ex.Message}");
            }
        }

        private MqttInfluxConfig GetConfig()
        {
            return new MqttInfluxConfig
            {
                MqttBroker = BrokerBox.Text,
                MqttTopic = TopicBox.Text,
                InfluxUrl = InfluxUrlBox.Text,
                InfluxToken = TokenBox.Password,
                InfluxBucket = BucketBox.Text,
                InfluxOrg = OrgBox.Text
            };
        }
    }
}
