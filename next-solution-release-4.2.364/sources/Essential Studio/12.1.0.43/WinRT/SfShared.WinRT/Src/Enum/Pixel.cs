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

#if WINDOWS_PHONE || WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls
#else
#if SILVERLIGHT
namespace Syncfusion.Tools.Controls
#else
#if WPF
namespace Syncfusion.Windows.Controls
#else
namespace Syncfusion.UI.Xaml.Controls
#endif
#endif
#endif
{
    /// <summary>
    /// Represents a class for maintaining Pixels
    /// </summary>
    public class Pixel
    {
        /// <summary>
        /// Initializes an instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Pixel"/> class.
        /// </summary>
        /// <param name="R"></param>
        /// <param name="G"></param>
        /// <param name="B"></param>
        /// <param name="A"></param>
        public Pixel(byte R, byte G, byte B, byte A)
        {
            this.R = R;
            this.G = G;
            this.B = B;
            this.A = A;
        }

        /// <summary>
        /// Initializes an instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Pixel"/> class.
        /// </summary>
        public Pixel() { }
        
        /// <summary>
        /// Gets or sets the R byte
        /// </summary>
        public byte R
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the G byte
        /// </summary>
        public byte G
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the B byte
        /// </summary>
        public byte B
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the A byte
        /// </summary>
        public byte A
        {
            get;
            set;
        }
    }
}

