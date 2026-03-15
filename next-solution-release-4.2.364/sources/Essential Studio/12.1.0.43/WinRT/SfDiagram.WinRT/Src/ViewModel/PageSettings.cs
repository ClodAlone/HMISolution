#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
#if WINRT_USING
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media; 
using Windows.UI;
#else
using System.Windows;
using System.Windows.Media;
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
    public partial class PageSettings : IPageSettings
    {
        public PageSettings()
        {
#if WINRT
            switch (Application.Current.RequestedTheme)
            {
                case ApplicationTheme.Dark:
                    PageBackground = new SolidColorBrush(new Color() {A = 255, R = 26, G = 26, B = 26});
                    PageBorderBrush = new SolidColorBrush(new Color() {A = 255, R = 80, G = 81, B = 81});
                    break;
                case ApplicationTheme.Light:
                    PageBackground = new SolidColorBrush(new Color() { A = 255, R = 255, G = 255, B = 255 });
                    PageBorderBrush = new SolidColorBrush(new Color() { A = 255, R = 106, G = 107, B = 107 });
                    break;
            }
#else
            PageBackground = new SolidColorBrush(new Color() {A = 255, R = 255, G = 255, B = 255});
            PageBorderBrush = new SolidColorBrush(new Color() {A = 255, R = 106, G = 107, B = 107});

#endif
            PageBorderThickness = new Thickness(1);
        }

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public interface IPageSettings : INotifyPropertyChanged
    {
        double PageWidth { get; set; }
        double PageHeight { get; set; }
        bool MultiplePage { get; set; }
        Thickness? OffPageMinMargin { get; set; }
        Thickness? OffPageMaxMargin { get; set; }
        PageOrientation PageOrientation { get; set; }
        Brush PageBackground { get; set; }
        MeasurementUnit Unit { get; set; }
        Brush PageBorderBrush { get; set; }
        Thickness? PageBorderThickness { get; set; }
        bool ShowPageBreaks { get; set; }
        Thickness PrintMargin { get; set; } 
    }

    //public class PageSettings : IPageSettings
    //{
    //    private double _pageWidth;
    //    private double _pageHeight;
    //    private bool _multiplePage;
    //    private Thickness? _offpageminMargin;
    //    private Thickness? _offpagemaxMargin;
    //    private PageOrientation _pageOrientation;       
    //    private Brush _pageBackground = new SolidColorBrush(Colors.Transparent);
    //    private bool _pagebreaks;
    //    private Rect _printrect;
    //    private PageSetup _setup = PageSetup.PrintandPageProperties;
    //    private MeasurementUnit _unit;

    //    public double PageWidth
    //    {
    //        get { return _pageWidth; }
    //        set
    //        {
    //            if (_pageWidth != value)
    //            {
    //                _pageWidth = value;
    //                OnPropertyChanged("PageWidth");
    //            }
    //        }
    //    }

    //    public double PageHeight
    //    {
    //        get { return _pageHeight; }
    //        set
    //        {
    //            if (_pageHeight != value)
    //            {
    //                _pageHeight = value;
    //                OnPropertyChanged("PageHeight");
    //            }
    //        }
    //    }

    //    public bool MultiplePage
    //    {
    //        get { return _multiplePage; }
    //        set
    //        {
    //            if (_multiplePage != value)
    //            {
    //                _multiplePage = value;
    //                OnPropertyChanged("MultiplePage");
    //            }
    //        }
    //    }

    //    public Thickness? OffPageMinMargin
    //    {
    //        get { return _offpageminMargin; }
    //        set
    //        {
    //            if (_offpageminMargin != value)
    //            {
    //                _offpageminMargin = value;
    //                OnPropertyChanged("OffPageMinMargin");
    //            }
    //        }
    //    }

    //    public Thickness? OffPageMaxMargin
    //    {
    //        get { return _offpagemaxMargin; }
    //        set
    //        {
    //            if (_offpagemaxMargin != value)
    //            {
    //                _offpagemaxMargin = value;
    //                OnPropertyChanged("OffPageMaxMargin");
    //            }
    //        }
    //    }

    //    public PageOrientation PageOrientation
    //    {
    //        get { return _pageOrientation; }
    //        set
    //        {
    //            if (_pageOrientation != value)
    //            {
    //                _pageOrientation = value;
    //                OnPropertyChanged("PageOrientation");
    //            }
    //        }
    //    }

    //    public Brush PageBackground
    //    {
    //        get { return _pageBackground; }
    //        set
    //        {
    //            if (_pageBackground != value)
    //            {
    //                _pageBackground = value;
    //                OnPropertyChanged("PageBackground");
    //            }
    //        }
    //    }

    //    public bool ShowPageBreaks
    //    {
    //        get
    //        {
    //            return _pagebreaks;
    //        }
    //        set
    //        {
    //            if (_pagebreaks != value)
    //            {
    //                _pagebreaks = value;
    //                OnPropertyChanged("ShowPageBreaks");
    //            }
    //        }
    //    }

    //    public Rect PrintMargin
    //    {
    //        get
    //        {
    //            return _printrect;
    //        }
    //        set
    //        {
    //            if (_printrect != value)
    //            {
    //                _printrect = value;
    //                OnPropertyChanged("PrintMargin");
    //            }
    //        }
    //    }

    //    public PageSetup PageSetup
    //    {
    //        get
    //        {
    //            return _setup;
    //        }
    //        set
    //        {
    //            if (_setup != value)
    //            {
    //                _setup = value;
    //                OnPropertyChanged("PageSetup");
    //            }
    //        }
    //    }

    //    public MeasurementUnit Unit
    //    {
    //        get { return _unit; }
    //        set
    //        {
    //            if (_unit != value)
    //            {
    //                _unit = value;
    //                OnPropertyChanged("Unit");
    //            }
    //        }
    //    }

    //    public event PropertyChangedEventHandler PropertyChanged;

    //    protected virtual void OnPropertyChanged(string propertyName)
    //    {
    //        if (PropertyChanged != null)
    //        {
    //            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    //        }
    //    }

    //}

    public enum PageOrientation
    {
        Portrait,
        Landscape
    }

    internal enum PageExtendMode
    {
        Fixed = 0,
        ExtendOnScroll = 1 << 0,
        ExtendOnDrag = 1 << 1,
        ExtendOnDragComplete = 1 << 2,
        Extend = ExtendOnScroll | ExtendOnDrag | ExtendOnDragComplete,
        ContractOnScroll = 1 << 3,
        ContractOnScrollComplete = 1 << 4,
        ContractOnDrag = 1 << 5,
        ContractOnDragComplete = 1 << 6,
        Contract = ContractOnScroll | ContractOnScrollComplete | ContractOnDrag | ContractOnDragComplete,
    }

}
