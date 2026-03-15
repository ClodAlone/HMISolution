using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Effects;
using System.ComponentModel;
using System.IO;

namespace UFWebClient.Controls
{
    public partial class ScreenViewer : UserControl, INotifyPropertyChanged, IDisposable
    {
        public ScreenViewer()
        {
            InitializeComponent();
        }

        #region Methods

        private void CleanSurface()
        {
            MainSurface.Children.Clear();
        }

        private void MainSurface_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control)
                return;

            ZoomLevelX = e.Delta > 0 ? ZoomLevelX + 0.1 : ZoomLevelX - 0.1;
            ZoomLevelY = e.Delta > 0 ? ZoomLevelY + 0.1 : ZoomLevelY - 0.1;
            e.Handled = true;
        }

        #endregion

        #region Properties

#if !WINDOWS_PHONE
        [Browsable(false)]
#endif
        public Canvas Surface
        {
            set
            {
                CleanSurface();

                if (value != null)
                {
                    MainSurface.Height = value.Height;
                    MainSurface.Width = value.Width;
                    MainSurface.Background = value.Background;

                    while (value.Children.Count > 0)
                    {
                        UIElement child = value.Children[0] as UIElement;

                        value.Children.Remove(child);
                        MainSurface.Children.Add(child);
                    }
                }
            }
        }


        private double _ZoomLevelX = 1.0;
        public double ZoomLevelX
        {
            get
            {
                return _ZoomLevelX;
            }
            set
            {
                if (Object.Equals(_ZoomLevelX, value))
                    return;

                _ZoomLevelX = value;
                OnPropertyChanged("ZoomLevelX");
            }
        }

        private double _ZoomLevelY = 1.0;
        public double ZoomLevelY
        {
            get
            {
                return _ZoomLevelY;
            }
            set
            {
                if (Object.Equals(_ZoomLevelY, value))
                    return;

                _ZoomLevelY = value;
                OnPropertyChanged("ZoomLevelY");
            }
        }

        private double _OffsetLevelX = 0;
        public double OffsetLevelX
        {
            get
            {
                return _OffsetLevelX;
            }
            set
            {
                if (Object.Equals(_OffsetLevelX, value))
                    return;

                _OffsetLevelX = value;
                OnPropertyChanged("OffsetLevelX");
            }
        }

        private double _OffsetLevelY = 0;
        public double OffsetLevelY
        {
            get
            {
                return _OffsetLevelY;
            }
            set
            {
                if (Object.Equals(_OffsetLevelY, value))
                    return;

                _OffsetLevelY = value;
                OnPropertyChanged("OffsetLevelY");
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                DependencyObject dispatcherObject = handler.Target as DependencyObject;

                var e = new PropertyChangedEventArgs(propertyName);
                // If the subscriber is a DispatcherObject and different thread
                if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                {
                    // Invoke handler in the target dispatcher's thread
                    dispatcherObject.Dispatcher.BeginInvoke(handler, this, e);
                }
                else // Execute handler as is
                    handler(this, e);
            }
        }

        protected void NotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion


        #region IDisposable Members

        bool bDisposed = false;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            foreach (var uie in MainSurface.Children)
            {
                if (uie is IDisposable)
                    (uie as IDisposable).Dispose();
            }
        }

        #endregion

    }
}
