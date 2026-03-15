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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Syncfusion.UI.Xaml.Diagram
{
    public class ExportSettings : INotifyPropertyChanged
    {
        public ExportSettings()
        {

        }
        /// <summary>
        /// BitmapEncoder for exporting
        /// </summary>
        private BitmapEncoder _coder;
        public BitmapEncoder ExportBitmapEncoder
        {
            get
            {
                return _coder;
            }
            set
            {
                if (_coder != value)
                {
                    _coder = value;
                    OnPropertyChanged("ExportBitmapEncoder");
                }
            }
        }

        /// <summary>
        /// Background of Exporting Page
        /// </summary>
        private SolidColorBrush _pagebackground;
        public SolidColorBrush ExportBackground
        {
            get
            {
                return _pagebackground;
            }
            set
            {
                if (_pagebackground != value)
                {
                    _pagebackground = value;
                    OnPropertyChanged("ExportBackground");
                }
            }
        }

        /// <summary>
        /// Specifies the alpha mode of pixel data.
        /// </summary>
        private BitmapAlphaMode _alphamode = BitmapAlphaMode.Ignore;
        public BitmapAlphaMode BitmapAlphaMode
        {
            get
            {
                return _alphamode;
            }
            set
            {
                if (_alphamode != value)
                {
                    _alphamode = value;
                    OnPropertyChanged("BitmapAlphaMode");
                }

            }
        }

        /// <summary>
        ///  Specifies the pixel format of pixel data. Each enumeration value defines
        ///  a channel ordering, bit depth, and data type.
        /// </summary>
        private BitmapPixelFormat _pixel = BitmapPixelFormat.Bgra8;
        public BitmapPixelFormat BitmapPixelFormat
        {
            get
            {
                return _pixel;
            }
            set
            {
                if (_pixel != value)
                {
                    _pixel = value;
                    OnPropertyChanged("BitmapPixelFormat");
                }

            }
        }

        /// <summary>
        /// Clipping Rect for exporting the Specific Region of SfDiagram
        /// </summary>
        private Rect _mclip = Rect.Empty;
        public Rect Clip
        {
            get
            {
                return _mclip;
            }
            set
            {
                if (_mclip != value)
                {
                    _mclip = value;
                    OnPropertyChanged("Clip");
                }
            }
        }
        /// <summary>
        /// Stretch Option for Exporting the Image
        /// </summary>
        private Stretch _mImageStretch;
        public Stretch ImageStretch
        {
            get
            {
                return _mImageStretch;
            }
            set
            {
                if (_mImageStretch != value)
                {
                    _mImageStretch = value;
                    OnPropertyChanged("ImageStretch");
                }
            }
        }

        /// <summary>
        /// Specifies the Modes for Exporting
        /// </summary>
        private ExportMode _exportmode;
        public ExportMode ExportMode
        {
            get
            {
                return _exportmode;
            }
            set
            {
                if (_exportmode != value)
                {
                    _exportmode = value;
                    OnPropertyChanged("ExportMode");
                }
            }
        }

        /// <summary>
        /// Count of Pages for Exporting Based on PageHeight
        /// </summary>
        private int _mRowCount = 1;
        public int RowCount
        {
            get
            {
                return _mRowCount;
            }
            internal set
            {
                if (_mRowCount != value)
                {
                    _mRowCount = value;
                    OnPropertyChanged("RowCount");
                }

            }
        }

        /// <summary>
        /// Count of Pages for Exporting Based on PageWidth
        /// </summary>
        private int _mColumnCount = 1;
        public int ColumnCount
        {
            get
            {
                return _mColumnCount;
            }
            internal set
            {
                if (_mColumnCount != value)
                {
                    _mColumnCount = value;
                    OnPropertyChanged("ColumnCount");
                }
            }
        }
       
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
