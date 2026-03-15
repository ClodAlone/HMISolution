#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Barcode
{
	[ToolboxItem(true)]
     [ToolboxBitmap(typeof(SfBarcode), "ToolboxIcons.SfBarcode.Icon.png")]
    public partial class SfBarcode : UserControl
    {
        # region Fields
        private string m_text;
        private const double m_marginFactor = 0.03; // 3%
        private BarcodeRotation m_rotation;
        private readonly double fontSize;
        private BarcodeQuietZones m_quietZones;
        private Settings m_symbologySettings;
        private BarcodeSymbolType m_symbology;
        private bool m_displayText = true;
        private BarcodeTextLocation m_textLocation;
        private Color m_lightBarColor;
        private Color m_darkBarColor;
        private Color m_textColor;
        private float m_textGapHeight;
        private BarcodeTextAlignment m_textAlignment;

        # endregion

        #region Constructor
        public SfBarcode()
        {
            InitializeComponent();

            m_symbology = BarcodeSymbolType.QRBarcode;
            m_quietZones = new BarcodeQuietZones();
            m_lightBarColor = Color.White;
            m_darkBarColor = Color.Black;
            m_textColor = Color.Black;
            m_textLocation = BarcodeTextLocation.Bottom;
            m_textAlignment = BarcodeTextAlignment.Center;
            m_rotation = BarcodeRotation.Angle0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the text to encode.
        /// </summary>
        public string Text
        {
            get { return m_text; }
            set { m_text = value; Invalidate(); }
        }

        /// <summary>
        /// Gets or sets whether to display original text.
        /// </summary>
        /// <value>
        /// The default value is True.
        /// </value>
        public bool DisplayText
        {
            get
            {
                return m_displayText;
            }
            set
            {
                m_displayText = value;
            }
        }

        public Color LightBarColor
        {
            get
            {
                return m_lightBarColor;
            }
            set
            {
                m_lightBarColor = value;
            }
        }

        public Color DarkBarColor
        {
            get
            {
                return m_darkBarColor;
            }
            set
            {
                m_darkBarColor = value;
            }
        }

        public float TextGapHeight
        {
            get
            {
                return m_textGapHeight;
            }
            set
            {
                m_textGapHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the Settings for the applied bar code type.
        /// </summary>
        public Settings SymbologySettings
        {
            get { return m_symbologySettings; }
            set { m_symbologySettings = value; Invalidate(); }
        }

        /// <summary>
        /// Gets or sets the bar code symbol type.
        /// </summary>
        /// <value>
        /// The default value is Code39.
        /// </value>
        public BarcodeSymbolType Symbology
        {
            get { return m_symbology; }
            set { m_symbology = value; Invalidate(); }
        }

        /// <summary>
        /// Gets of sets the vertical alignment of the text.
        /// </summary>
        /// <value>
        /// The default value is Bottom.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Barcode.BarcodeTextLocation"/>
        private BarcodeTextLocation TextLocation
        {
            get { return m_textLocation; }
            set { m_textLocation = value; Invalidate(); }
        }

        #region QuietZone
        //public BarcodeQuietZones QuietZone
        //{
        //    get
        //    {
        //        return m_quietZones;
        //    }
        //    set
        //    {
        //        m_quietZones = value;
        //    }
        //}
        #endregion

        public Color TextColor
        {
            get
            {
                return m_textColor;
            }
            set
            {
                m_textColor = value;
            }
        }

        public BarcodeTextAlignment TextAlignment
        {
            get
            {
                return m_textAlignment;
            }
            set
            {
                m_textAlignment = value;
            }
        }

        #endregion

        #region Implementation
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            SizeF controlSize = this.Size;
            SizeF barcodeSize = DrawBarcode();

            int x = (int)(controlSize.Width - barcodeSize.Width) / 2;
            int y = (int)(controlSize.Height - barcodeSize.Height) / 2;
            panel1.Location = new Point(x, y);
            panel1.Size = new Size((int)barcodeSize.Width, (int)barcodeSize.Height);

        }

        private SizeF DrawBarcode()
        {
            if (!String.IsNullOrEmpty(Text))
            {

                UnidimensionalBarcode m_barcode1D = null;
                DataMatrixBarcode m_dataMatrixBarcode = null;
                QRBarcode m_qrBarcode = null;
                Barcode1DSetting typeSetting = null;
                IBarcodeSetting setting = SymbologySettings as IBarcodeSetting;

                switch (Symbology)
                {
                    case BarcodeSymbolType.Code39:
                        {
                            m_barcode1D = new Code39Barcode();
                            typeSetting = setting as Code39Setting;
                        }
                        break;
                    case BarcodeSymbolType.Code39Extended:
                        {
                            m_barcode1D = new Code39ExtendedBarcode();
                            typeSetting = setting as Code39ExtendedSetting;
                        }
                        break;
                    case BarcodeSymbolType.Code11:
                        {
                            m_barcode1D = new Code11Barcode();
                            typeSetting = setting as Code11Setting;
                        }
                        break;
                    case BarcodeSymbolType.Codabar:
                        {
                            m_barcode1D = new CodabarBarcode();
                            typeSetting = setting as CodabarSetting;
                        }
                        break;
                    case BarcodeSymbolType.Code32:
                        {
                            m_barcode1D = new Code32Barcode();
                            typeSetting = setting as Code32Setting;
                        }
                        break;
                    case BarcodeSymbolType.Code93:
                        {
                            m_barcode1D = new Code93Barcode();
                            typeSetting = setting as Code93Setting;
                        }
                        break;
                    case BarcodeSymbolType.Code93Extended:
                        {
                            m_barcode1D = new Code93ExtendedBarcode();
                            typeSetting = setting as Code93ExtendedSetting;
                        }
                        break;
                    case BarcodeSymbolType.Code128A:
                        {
                            m_barcode1D = new Code128ABarcode();
                            typeSetting = setting as Code128ASetting;
                        }
                        break;
                    case BarcodeSymbolType.Code128B:
                        {
                            m_barcode1D = new Code128BBarcode();
                            typeSetting = setting as Code128BSetting;
                        }
                        break;
                    case BarcodeSymbolType.Code128C:
                        {
                            m_barcode1D = new Code128CBarcode();
                            typeSetting = setting as Code128CSetting;
                        }
                        break;
                    case BarcodeSymbolType.DataMatrix:
                        m_dataMatrixBarcode = new DataMatrixBarcode();
                        break;
                    case BarcodeSymbolType.QRBarcode:
                        m_qrBarcode = new QRBarcode();
                        break;
                    default:
                        break;
                }

                if (m_barcode1D != null)
                {
                    m_barcode1D.Text = Text;
                    m_barcode1D.BackColor = LightBarColor;
                    m_barcode1D.BarColor = DarkBarColor;
                    m_barcode1D.TextAlignment = TextAlignment;
                    m_barcode1D.TextDisplayLocation = TextLocation;
                    m_barcode1D.TextColor = TextColor;
                    m_barcode1D.BarcodeToTextGapHeight = TextGapHeight;

                    //if (QuietZone.All > 0.0)
                    //    m_barcode1D.QuietZone = QuietZone;

                    if (typeSetting != null)
                    {
                        m_barcode1D.BarHeight = (float)typeSetting.BarHeight;
                        m_barcode1D.EnableCheckDigit = typeSetting.EnableCheckDigit;
                        m_barcode1D.EncodeStartStopSymbols = typeSetting.EncodeStartStopSymbols;
                        m_barcode1D.ShowCheckDigit = typeSetting.ShowCheckDigit;
                        m_barcode1D.NarrowBarWidth = (float)typeSetting.NarrowBarWidth;
                    }

                    if (!DisplayText)
                        m_barcode1D.TextDisplayLocation = BarcodeTextLocation.None;

                    try
                    {
                        return m_barcode1D.Draw(panel1);
                    }
                    catch (BarcodeException ex) // Invalid data to encode.
                    {

                    }
                }
                else if (m_dataMatrixBarcode != null)
                {
                    m_dataMatrixBarcode.Text = m_text;
                    //if (QuietZone.All > 0.0)
                    //    m_dataMatrixBarcode.QuietZone = QuietZone;

                    DataMatrixSetting dmSetting = setting as DataMatrixSetting;

                    if (dmSetting != null)
                    {
                        m_dataMatrixBarcode.XDimension = (float)dmSetting.XDimension;
                        m_dataMatrixBarcode.Encoding = dmSetting.Encoding;
                        m_dataMatrixBarcode.Size = dmSetting.Size;
                    }

                    return m_dataMatrixBarcode.Draw(panel1);
                }
                else if (m_qrBarcode != null)
                {
                    m_qrBarcode.Text = m_text;

                    //if (QuietZone.All > 0.0)
                    //    m_qrBarcode.QuietZone = QuietZone;

                    QRBarcodeSetting qrSetting = setting as QRBarcodeSetting;

                    if (qrSetting != null)
                    {
                        m_qrBarcode.XDimension = (float)qrSetting.XDimension;
                        m_qrBarcode.QRVersion = qrSetting.Version;
                        m_qrBarcode.ErrorCorrectionLevel = qrSetting.ErrorCorrectionLevel;
                        m_qrBarcode.InputMode = qrSetting.InputMode;
                    }

                    return m_qrBarcode.Draw(panel1);
                }
            }
            return SizeF.Empty;
        }
        #endregion
    }
    # region Enumerations
    /// <summary>
    /// Specifies barcode symbol type
    /// </summary>
    public enum BarcodeSymbolType
    {
        Code39,
        Code39Extended,
        Code11,
        Codabar,
        Code32,
        Code93,
        Code93Extended,
        Code128A,
        Code128B,
        Code128C,
        DataMatrix,
        QRBarcode
    }

    /// <summary>
    /// Specifies barcode rotation.
    /// </summary>
    public enum BarcodeRotation
    {
        Angle0,
        Angle90,
        Angle180,
        Angle270,
    }
    #endregion
}
