#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartImage
    /// </summary>
    public class ChartImage
    {
        private int _width = 0;
        private int _height = 0;
        private bool _init = false;
        private byte[] _buffer;
        private int _rowLength;

        /// <summary>
        /// Event for ImageError 
        /// </summary>
        public event EventHandler<ChartImageErrorEventArgs> ImageError;

        /// <summary>
        /// Called when instance created with two arguments for ChartImage
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public ChartImage(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Get or Set Width property
        /// </summary>
        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (_init)
                {
                    OnImageError("Error: Cannot change Width after the ChartImage has been initialized");
                }
                else if ((value <= 0) || (value > 2047))
                {
                    OnImageError("Error: Width must be between 0 and 2047");
                }
                else
                {
                    _width = value;
                }
            }
        }

        /// <summary>
        /// Get or Set Height Property
        /// </summary>
        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (_init)
                {
                    OnImageError("Error: Cannot change Height after the ChartImage has been initialized");
                }
                else if ((value <= 0) || (value > 2047))
                {
                    OnImageError("Error: Height must be between 0 and 2047");
                }
                else
                {
                    _height = value;
                }
            }
        }

        /// <summary>
        /// Method implementation for SetPixel
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="color"></param>
        public void SetPixel(int col, int row, Color color)
        {
            SetPixel(col, row, color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// Method implementation for SetPixel 
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <param name="alpha"></param>
        public void SetPixel(int col, int row, byte red, byte green, byte blue, byte alpha)
        {
            if (!_init)
            {
                _rowLength = _width * 4 + 1;
                _buffer = new byte[_rowLength * _height];

                // Initialize
                for (int idx = 0; idx < _height; idx++)
                {
                    _buffer[idx * _rowLength] = 0;      // Filter bit
                }

                _init = true;
            }

            if ((col > _width) || (col < 0))
            {
                OnImageError("Error: Column must be greater than 0 and less than the Width");
            }
            else if ((row > _height) || (row < 0))
            {
                OnImageError("Error: Row must be greater than 0 and less than the Height");
            }

            // Set the pixel
            int start = _rowLength * row + col * 4 + 1;
            _buffer[start] = red;
            _buffer[start + 1] = green;
            _buffer[start + 2] = blue;
            _buffer[start + 3] = alpha;
        }

        /// <summary>
        /// Return color value from the given int values
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public Color GetPixel(int col, int row)
        {
            if ((col > _width) || (col < 0))
            {
                OnImageError("Error: Column must be greater than 0 and less than the Width");
            }
            else if ((row > _height) || (row < 0))
            {
                OnImageError("Error: Row must be greater than 0 and less than the Height");
            }

            Color color = new Color();
            int _base = _rowLength * row + col + 1;

            color.R = _buffer[_base];
            color.G = _buffer[_base + 1];
            color.B = _buffer[_base + 2];
            color.A = _buffer[_base + 3];

            return color;
        }

        /// <summary>
        /// Return Stream value from the width and height
        /// </summary>
        /// <returns></returns>
        public Stream GetStream()
        {
            Stream stream;

            if (!_init)
            {
                OnImageError("Error: Image has not been initialized");
                stream = null;
            }
            else
            {
                stream = ChartPngEncoder.Encode(_buffer, _width, _height);
            }

            return stream;
        }

        private void OnImageError(string msg)
        {
            if (null != ImageError)
            {
                ChartImageErrorEventArgs args = new ChartImageErrorEventArgs();
                args.ErrorMessage = msg;
                ImageError(this, args);
            }
        }

        /// <summary>
        /// Class implementation for ChartImageErrorEventArgs
        /// </summary>
        public class ChartImageErrorEventArgs : EventArgs
        {
            private string _errorMessage = string.Empty;

            /// <summary>
            /// Get or Set ErrorMessage Property
            /// </summary>
            public string ErrorMessage
            {
                get { return _errorMessage; }
                set { _errorMessage = value; }
            }
        }

    }
}
