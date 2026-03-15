using System.Configuration;
using System.Data;
using System.Windows;
using WpfApp1.Services;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public BackgroundTaskProcessor? BackgroundProcessor { get; private set; }
        public Services.PluginManager? PluginManager { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // allow configuring concurrency via environment or default to 2
            int concurrency = 2;
            var env = System.Environment.GetEnvironmentVariable("BG_TASK_CONCURRENCY");
            if (!string.IsNullOrEmpty(env) && int.TryParse(env, out var parsed) && parsed > 0)
                concurrency = parsed;

            BackgroundProcessor = new BackgroundTaskProcessor(concurrency);
            PluginManager = new Services.PluginManager();
            BackgroundProcessor.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            BackgroundProcessor?.Dispose();
        }
    }

}
