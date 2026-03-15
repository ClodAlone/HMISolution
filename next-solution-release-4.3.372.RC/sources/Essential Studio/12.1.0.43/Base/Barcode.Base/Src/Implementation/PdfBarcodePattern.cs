#if !SILVERLIGHT
#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Text;

# if WPF || BARCODE_SILVERLIGHT || BARCODE_WINRT
namespace Syncfusion.UI.Xaml.Controls.Barcode
#elif WINDOWS_PHONE
namespace Syncfusion.WP.Controls.Barcode
#elif ASPNET
namespace Syncfusion.Web.UI.WebControls.Barcode
#elif WINFORMS
namespace Syncfusion.Windows.Forms.Barcode
#elif MVC
namespace Syncfusion.Mvc.Barcode
#else
/// <summary>
/// The Syncfusion.Pdf.Barcode namespace contains classes for creating barcodes.
/// </summary>
namespace Syncfusion.Pdf.Barcode
#endif
{
    /// <summary>
    /// Represets the Utility class for storing barcode symbols.
    /// </summary>
    internal class BarcodeSymbolTable
    {
#region Fields
        /// <summary>
        /// Indicates the symbol.
        /// </summary>
        private char m_symbol;

        /// <summary>
        /// Indicates the check character.
        /// </summary>
        private int m_checkDigit;

        /// <summary>
        /// Indicates the Data.
        /// </summary>
        private byte[] m_bars;
        #endregion

#region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeSymbolTable"/> class.
        /// </summary>
        public BarcodeSymbolTable()
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeSymbolTable"/> class.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="checkDigit">The check digit.</param>
        /// <param name="bars">The bars.</param>
        public BarcodeSymbolTable(char symbol, int checkDigit, byte[] bars)
        {
            m_symbol = symbol;
            m_checkDigit = checkDigit;
            m_bars = bars;
        }
        #endregion

#region Properties
        /// <summary>
        /// Gets or sets the Symbol.
        /// </summary>
        public char Symbol
        {
            get
            {
                return m_symbol;
            }

            set
            {
                m_symbol = value;
            }
        }

        /// <summary>
        /// Gets or sets the check digit.
        /// </summary>
        public int CheckDigit
        {
            get
            {
                return m_checkDigit;
            }

            set
            {
                m_checkDigit = value;
            }
        }

        /// <summary>
        /// Gets or sets the bar information.
        /// </summary>
        public byte[] Bars
        {
            get
            {
                return m_bars;
            }

            set
            {
                m_bars = value;
            }
        }
        #endregion
    }
}
#endif