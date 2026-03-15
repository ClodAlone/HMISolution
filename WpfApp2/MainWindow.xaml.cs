using System.Windows;
using WpfApp2.Services;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly BackgroundService _backgroundService = BackgroundService.Instance;

        public MainWindow()
        {
            InitializeComponent();
            // Set the DataContext to the BackgroundService for binding
            this.DataContext = _backgroundService;
        }

        private void DarkTheme_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.Instance.SetTheme(Theme.Dark);
        }

        private void LightTheme_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.Instance.SetTheme(Theme.Light);
        }

        private async void StartSampleTask_Click(object sender, RoutedEventArgs e)
        {
            var task = new SampleBackgroundTask(5);
            await _backgroundService.EnqueueTaskAsync(task, "Sample Task");
        }

        private async void StartBuild_Click(object sender, RoutedEventArgs e)
        {
            var task = new BuildTask();
            await _backgroundService.EnqueueTaskAsync(task, "Build Solution");
        }

        private void ClearCompleted_Click(object sender, RoutedEventArgs e)
        {
            _backgroundService.ClearCompletedTasks();
        }
    }
}
