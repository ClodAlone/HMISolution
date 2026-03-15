#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

#if WINRT_USING
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    public class Gridlines : INotifyPropertyChanged
    {
        static readonly Style _mPathStyle = new Style(typeof(Path));

        static Gridlines()
        {
#if WINRT
            if(Application.Current.RequestedTheme.Equals(ApplicationTheme.Dark))
            _mPathStyle.Setters.Add(new Setter(Path.StrokeProperty, new SolidColorBrush(new Color(){A=255,R=46,G=46,B=46})));
            else if(Application.Current.RequestedTheme.Equals(ApplicationTheme.Light))
#endif
            {
                _mPathStyle.Setters.Add(new Setter(Path.StrokeProperty,new SolidColorBrush(new Color(){A=255,R=211,G=211,B=211})));
            }
        }

        public Gridlines()
        {
            LinesInterval = new List<double>() {0.8, 19.2, 0.25, 19.75, 0.25, 19.75, 0.25, 19.75,0.25,19.75};
            Strokes = new List<Style>() {_mPathStyle};
            DynamicZoom = true;
            SnapInterval = new List<double>() {20};
        }

        private IEnumerable<double> _linesInterval;

        public IEnumerable<double> LinesInterval
        {
            get { return _linesInterval; }
            set
            {
                _linesInterval = value;
                OnPropertyChanged("LinesInterval");
            }
        }

        private IEnumerable<Style> _Strokes;

        public IEnumerable<Style> Strokes
        {
            get { return _Strokes; }
            set
            {
                _Strokes = value;
                OnPropertyChanged("Strokes");
            }
        }
       

        public bool DynamicZoom { get; set; }

        public IEnumerable<double> SnapInterval { get; set; }

        protected virtual void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(prop));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
