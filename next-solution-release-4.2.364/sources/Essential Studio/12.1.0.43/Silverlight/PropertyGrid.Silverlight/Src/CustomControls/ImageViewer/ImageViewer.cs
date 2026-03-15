#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Win32;

#if SILVERLIGHT
namespace Syncfusion.Windows.PropertyGrid
#endif
#if WPF
namespace Syncfusion.PropertyGrid.WPF
#endif
{
    public class ImageViewer : Control
    {
        public ImageViewer()
        {
            DefaultStyleKey = typeof(ImageViewer);
        }

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(ImageViewer), new PropertyMetadata(null));


        public FileInfo File
        {
            get { return (FileInfo)GetValue(FileProperty); }
            set { SetValue(FileProperty, value); }
        }

        public static readonly DependencyProperty FileProperty =
            DependencyProperty.Register("File", typeof(FileInfo), typeof(ImageViewer), new PropertyMetadata(null));

        public void OpenFile()
        {
            OpenFileDialog filedialog = new OpenFileDialog();
            if (filedialog.ShowDialog() == true)
            {
                try
                {
                    Stream stream = filedialog.File.OpenRead();
                    File = filedialog.File;
                    BitmapImage bi = new BitmapImage();
                    bi.SetSource(stream);
                    Source = bi;
                    stream.Close();
                }
                catch
                {

                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Button button = this.GetTemplateChild("button") as Button;
            if(button != null)
                button.Click += new RoutedEventHandler(button_Click);
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

    }
}
