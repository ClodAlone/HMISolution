#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Linq;
using System.Text;

namespace Syncfusion.Windows.Forms.Barcode
{
    /// <summary>
    /// Base interface for bar code type settings.
    /// </summary>
    public interface IBarcodeSetting
    {
    }

    /// <summary>
    /// Base class for 2 dimensional bar code type settings.
    /// </summary>
    public abstract class Barcode2DSetting : Settings
    {
        private double m_xDimension;

        /// <summary>
        /// Initializes Barcode2DSetting class.
        /// </summary>
        public Barcode2DSetting()
        {
            XDimension = 4.0;
        }

        /// <summary>
        /// Gets or sets the XDimension for the 2D barcodes.
        /// </summary>
        /// <remarks>
        /// The value is in pixel.
        /// </remarks>
        /// <value>
        /// The default value is 4.
        /// </value>
        public double XDimension
        {
            get
            {
                return m_xDimension;
            }
            set
            {
                m_xDimension = value;
            }
        }
    }

    /// <summary>
    /// Handles the settings for DataMatrix bar code.
    /// </summary>
    public class DataMatrixSetting : Barcode2DSetting
    {
        private DataMatrixSize m_size;
        private DataMatrixEncoding m_encoding;

        /// <summary>
        /// Initializes DataMatrixSetting class.
        /// </summary>
        public DataMatrixSetting()
        {
            Size = DataMatrixSize.Auto;
            Encoding = DataMatrixEncoding.Auto;
        }

        /// <summary>
        /// Gets or sets the size matrix of DataMatrix bar code.
        /// </summary>
        /// <value>
        ///  The default value is Auto.
        /// </value>
        public DataMatrixSize Size
        {
            get
            {
                return m_size;
            }
            set
            {
                m_size = value;
            }
        }

        /// <summary>
        /// Gets or sets the encoding used for DataMatrix bar code.
        /// </summary>
        /// <value>
        ///  The default value is Auto.
        /// </value>
        public DataMatrixEncoding Encoding
        {
            get
            {
                return m_encoding;
            }
            set
            {
                m_encoding = value;
            }
        }
    }

    /// <summary>
    /// Handles the settings for QR bar code.
    /// </summary>
    public class QRBarcodeSetting : Barcode2DSetting
    {
        private QRBarcodeVersion m_version;
        private QRInputMode m_inputMode;
        private ErrorCorrectionLevel m_errorCorrectionLevel;

        /// <summary>
        /// Initializes QRBarcodeSetting class.
        /// </summary>
        public QRBarcodeSetting()
        {
            Version = QRBarcodeVersion.Auto;
            InputMode = Syncfusion.Windows.Forms.Barcode.QRInputMode.BinaryMode;
            ErrorCorrectionLevel = ErrorCorrectionLevel.Low;
        }

        /// <summary>
        /// Gets of sets the version of QR bar code.
        /// </summary>
        /// <value>
        /// The default value is Auto.
        /// </value>
        public QRBarcodeVersion Version
        {
            get
            {
                return m_version;
            }
            set
            {
                m_version = value;
            }
        }

        /// <summary>
        /// Gets or sets the input mode of QR bar code
        /// </summary>
        /// <value>
        /// The default value is BinaryMode.
        /// </value>
        public QRInputMode InputMode
        {
            get
            {
                return m_inputMode;
            }
            set
            {
                m_inputMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the error correction level of QR bar code.
        /// </summary>
        /// <value>
        /// The default value is low.
        /// </value>
        public ErrorCorrectionLevel ErrorCorrectionLevel
        {
            get
            {
                return m_errorCorrectionLevel;
            }
            set
            {
                m_errorCorrectionLevel = value;
            }
        }
    }

    /// <summary>
    /// Base class for 1 dimensional bar code type settings.
    /// </summary>
    public abstract class Barcode1DSetting : Settings
    {
        private double m_barHeight;
        private bool m_enableCheckDigit;
        private bool m_encodeStartStopSymbols;
        private double m_narrowBarWidth;
        private bool m_showCheckDigit;

        /// <summary>
        /// Initializes Barcode1DSetting class.
        /// </summary>
        public Barcode1DSetting()
        {
            BarHeight = 80.0;
            EnableCheckDigit = false;
            EncodeStartStopSymbols = true;
            NarrowBarWidth = 1.0;
            ShowCheckDigit = false;
        }

        /// <summary>
        /// Gets or sets the height of each bar.
        /// </summary>
        /// <remarks>
        /// The value is in pixel.
        /// </remarks>
        /// <value>
        /// The default value is 80.
        /// </value>
        public double BarHeight
        {
            get
            {
                return m_barHeight;
            }
            set
            {
                m_barHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to enable check digit.
        /// </summary>
        /// <value>
        /// The default value is false.
        /// </value>
        public bool EnableCheckDigit
        {
            get
            {
                return m_enableCheckDigit;
            }
            set
            {
                m_enableCheckDigit = value;
            }
        }

        /// <summary>
        ///  Gets or sets whether to encode start stop symbols.
        /// </summary>
        /// <value>
        /// The default value is true.
        /// </value>
        public bool EncodeStartStopSymbols
        {
            get
            {
                return m_encodeStartStopSymbols;
            }
            set
            {
                m_encodeStartStopSymbols = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the narrow bar in bar code.
        /// </summary>
        /// <remarks>
        /// The value is in pixel.
        /// </remarks>
        /// <value>
        /// The default value is 1.
        /// </value>
        public double NarrowBarWidth
        {
            get
            {
                return m_narrowBarWidth;
            }
            set
            {
                m_narrowBarWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to display check digit.
        /// </summary>
        /// <value>
        /// The default value is false.
        /// </value>
        public bool ShowCheckDigit
        {
            get
            {
                return m_showCheckDigit;
            }
            set
            {
                m_showCheckDigit = value;
            }
        }
    }

    /// <summary>
    /// Handles the settings for Codabar bar code.
    /// </summary>
    public class CodabarSetting : Barcode1DSetting
    {
        /// <summary>
        /// Initializes CodabarSetting class.
        /// </summary>
        public CodabarSetting()
        {
        }
    }

    /// <summary>
    /// Handles the settings for Code11 bar code.
    /// </summary>
    public class Code11Setting : Barcode1DSetting
    {
        /// <summary>
        /// Initializes Code11Setting class.
        /// </summary>
        public Code11Setting()
        {
        }
    }

    /// <summary>
    /// Handles the settings for Code128A bar code.
    /// </summary>
    public class Code128ASetting : Barcode1DSetting
    {
        /// <summary>
        /// Initializes Code128ASetting class.
        /// </summary>
        public Code128ASetting()
        {
        }
    }

    /// <summary>
    /// Handles the settings for Code128B bar code.
    /// </summary>
    public class Code128BSetting : Barcode1DSetting
    {
        /// <summary>
        /// Initializes Code128BSetting class.
        /// </summary>
        public Code128BSetting()
        {
        }
    }

    /// <summary>
    /// Handles the settings for Code128C bar code.
    /// </summary>
    public class Code128CSetting : Barcode1DSetting
    {
        /// <summary>
        /// Initializes Code128CSetting class.
        /// </summary>
        public Code128CSetting()
        {
        }
    }

    /// <summary>
    /// Handles the settings for Code32 bar code
    /// </summary>
    public class Code32Setting : Barcode1DSetting
    {
        /// <summary>
        /// Initializes Code32Setting class.
        /// </summary>
        public Code32Setting()
        {
        }
    }

    /// <summary>
    /// Handles the settings for Code39 bar code.
    /// </summary>
    public class Code39Setting : Barcode1DSetting
    {
        /// <summary>
        /// Initializes Code39Setting class.
        /// </summary>
        public Code39Setting()
        {
        }
    }

    /// <summary>
    /// Handles the settings for Code39Extended bar code.
    /// </summary>
    public class Code39ExtendedSetting : Code39Setting
    {
        /// <summary>
        /// Initializes Code39ExtendedSetting class.
        /// </summary>
        public Code39ExtendedSetting()
        {
        }
    }

    /// <summary>
    /// Handles the settings for Code93 bar code.
    /// </summary>
    public class Code93Setting : Barcode1DSetting
    {
        /// <summary>
        /// Initializes Code93Setting class.
        /// </summary>
        public Code93Setting()
        {

        }
    }

    /// <summary>
    /// Handles the settings for Code93Extended bar code.
    /// </summary>
    public class Code93ExtendedSetting : Code93Setting
    {
        /// <summary>
        /// Initializes Code93ExtendedSetting class.
        /// </summary>
        public Code93ExtendedSetting()
        {
        }
    }

    /// <summary>
    /// Collection of bar code type settings.
    /// </summary>
    public class Settings : IBarcodeSetting
    {
        
    }
}
