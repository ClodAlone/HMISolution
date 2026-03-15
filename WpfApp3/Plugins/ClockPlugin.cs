using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WpfApp3.Contracts;

namespace WpfApp3.Plugins
{
    public class ClockPlugin : IPlugin
    {
        public string Name => "ClockPlugin";
        public string Header => "World Clock";
        public PreferredLocation Location => PreferredLocation.Right;

        public UIElement CreateView()
        {
            var grid = new Grid();
            var textBlock = new TextBlock
            {
                FontSize = 24,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = System.Windows.Media.Brushes.White
            };

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (s, e) => textBlock.Text = DateTime.Now.ToString("HH:mm:ss");
            timer.Start();

            textBlock.Text = DateTime.Now.ToString("HH:mm:ss");
            grid.Children.Add(textBlock);

            return grid;
        }
    }
}
