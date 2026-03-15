using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Services;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<ActiveTaskViewModel> _activeTasks = new();

        private class ActiveTaskViewModel : INotifyPropertyChanged
        {
            public string Name { get; }
            public CancellationTokenSource Cancellation { get; }

            private double _progress;
            public double Progress { get => _progress; set { _progress = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress))); } }

            public ActiveTaskViewModel(string name)
            {
                Name = name;
                Cancellation = new CancellationTokenSource();
            }

            public event PropertyChangedEventHandler? PropertyChanged;
        }

        public MainWindow()
        {
            InitializeComponent();
            ActiveTasksControl.ItemsSource = _activeTasks;
            PluginList.MouseDoubleClick += PluginList_MouseDoubleClick;
        }

        private void SolutionList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (SolutionList.SelectedItem is ListBoxItem item)
            {
                var header = item.Content?.ToString() ?? "Document";
                var tab = new TabItem();
                tab.Header = header;
                tab.Content = new TextBlock { Text = $"// {header} contents", Margin = new Thickness(12), Foreground = (System.Windows.Media.Brush)Application.Current.Resources["VSInactiveText"] };
                DocumentTabControl.Items.Add(tab);
                DocumentTabControl.SelectedItem = tab;
            }
        }

        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Parent is Panel pnl)
            {
                // header StackPanel is parent of the close button; its parent is HeaderedContentControl (TabItem)
                if (pnl.Parent is TabItem tab)
                {
                    DocumentTabControl.Items.Remove(tab);
                }
            }
        }

        private async void RunBackgroundTask_Click(object sender, RoutedEventArgs e)
        {
            var app = Application.Current as App;
            if (app?.BackgroundProcessor == null)
            {
                MessageBox.Show("Background processor not available", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Create view model for active task
            var vm = new ActiveTaskViewModel("Demo Task");
            _activeTasks.Add(vm);

            var progress = new Progress<double>(p => vm.Progress = p * 100);

            try
            {
                var task = app.BackgroundProcessor.EnqueueAsync(async (ct, prog) =>
                {
                    // simulate incremental work reporting progress
                    for (int i = 0; i <= 10; i++)
                    {
                        ct.ThrowIfCancellationRequested();
                        await Task.Delay(200, ct).ConfigureAwait(false);
                        prog?.Report(i / 10.0);
                    }
                }, vm.Cancellation.Token, progress);

                await task.ConfigureAwait(true);
                MessageBox.Show("Background task completed", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Background task cancelled", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Background task failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _activeTasks.Remove(vm);
                vm.Cancellation.Dispose();
            }
        }

        private void CancelActiveTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ActiveTaskViewModel vm)
            {
                try
                {
                    vm.Cancellation.Cancel();
                }
                catch { }
            }
        }

        private void LoadPlugins_Click(object sender, RoutedEventArgs e)
        {
            var app = Application.Current as App;
            if (app?.PluginManager == null)
            {
                MessageBox.Show("Plugin manager not available", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var pluginsDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
            app.PluginManager.LoadPlugins(pluginsDir);

            PluginList.Items.Clear();
            foreach (var p in app.PluginManager.Plugins)
            {
                var li = new ListBoxItem { Content = p.Name, Tag = p };
                PluginList.Items.Add(li);
            }
        }

        private void PluginList_MouseDoubleClick(object? sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (PluginList.SelectedItem is ListBoxItem li && li.Tag is Services.PluginInfo info)
            {
                var ctrl = info.CreateControl();
                if (ctrl != null)
                {
                    var tab = new TabItem { Header = info.Name, Content = ctrl };
                    DocumentTabControl.Items.Add(tab);
                    DocumentTabControl.SelectedItem = tab;
                }
                else
                {
                    MessageBox.Show($"Plugin '{info.Name}' failed to create control.", "Plugin Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
