using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Windows.Markup;
using System.Windows.Threading;
using System.IO;

namespace CommonControls
{
    /// <summary>
    /// Interaction logic for LazyXamlContentControl.xaml
    /// </summary>
    public partial class LazyXamlContentControl : UserControl
    {
        FrameworkElement content;
        bool isVisible;
        String _contentToLoad;
        public String contentToLoad 
        { 
            get
            {
                return _contentToLoad;
            }
            set
            {
                if (_contentToLoad == value)
                    return;
                _contentToLoad = value;
                if (isVisible)
                    Start();
            }
        }

        public FrameworkElement Content
        {
            get
            {
                return content;
            }
            set
            {
                if (content == value)
                    return;
                content = value;
                if (isVisible)
                    Start();
            }
        }

        public LazyXamlContentControl()
        {
            InitializeComponent();
        }

        private void HandleVisibleChanged(object sender,
            DependencyPropertyChangedEventArgs e)
        {
            bool _isVisible = (bool)e.NewValue;
            isVisible = _isVisible;
            if (isVisible)
                Start();
            else
                Stop();
        }

        private void Content_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Content_Unloaded(object sender, RoutedEventArgs e)
        {
            // Stop();
        }

        DispatcherTimer dt;
        static bool isLoading = false;
        void Start()
        {
            if (dt == null)
            {
                dt = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500), IsEnabled = true };
            }
            dt.Start();
            dt.Tick += (o, e) =>
                {
                    if (!isLoading)
                    {
                        isLoading = true;
                        dt.Stop();
                        if (content != null)
                        {
                            if (!(contentControl.Content is Image))
                                contentControl.Content = content;
                        }
                        else if (!String.IsNullOrEmpty(_contentToLoad))
                        {
                            try
                            {
                                String imagepath = System.IO.Path.ChangeExtension(contentToLoad, ".png");
                                if (File.Exists(imagepath))
                                {
                                    // Create source.
                                    var bi = new BitmapImage();
                                    // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                                    bi.BeginInit();
                                    bi.UriSource = new Uri(imagepath, UriKind.RelativeOrAbsolute);
                                    bi.EndInit();
                                    contentControl.Content = new Image() { Source = bi };
                                }
                                else
                                {
                                    using (var xmlReader = new FileStream(contentToLoad, FileMode.Open, 
                                                FileAccess.Read, FileShare.Read))
                                    {
                                        var folder = System.IO.Path.GetDirectoryName(contentToLoad);
                                        var pc = new ParserContext
                                        {
                                            // System.IO.Packaging.PackUriHelper.Create()
                                            BaseUri = new Uri(String.Format("{0}", folder))
                                        };

                                        content = XamlReader.Load(xmlReader, pc) as FrameworkElement;
                                        content.ClearValue(FrameworkElement.WidthProperty);
                                        content.ClearValue(FrameworkElement.HeightProperty);
                                        content.IsHitTestVisible = false;

                                        contentControl.Content = content;
                                    }
                                }
                                ToolTip = System.IO.Path.GetFileNameWithoutExtension(contentToLoad); 
                                // textBlock.Text = System.IO.Path.GetFileNameWithoutExtension(contentToLoad);
                            }
                            catch (Exception ex)
                            {
                                ToolTip = ex.ToString();
                                _contentToLoad = null;
                            }
                        }
                        progressBar.Visibility = Visibility.Collapsed;
                        isLoading = false;
                    }
                };
        }

        void Stop()
        {
            if (dt != null)
            {
                dt.Stop();
            }
        }
    }
}
