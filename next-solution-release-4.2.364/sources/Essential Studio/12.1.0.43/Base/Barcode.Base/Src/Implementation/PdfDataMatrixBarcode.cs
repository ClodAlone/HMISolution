#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !SILVERLIGHT
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

#if !XAML && !GDI
using System.Drawing;
using Syncfusion.Pdf.Graphics;
#elif XAML && !BARCODE_WINRT
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
#elif GDI
using System.Drawing;
#if WINFORMS
using System.Windows.Forms;
#endif
#else
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI;
#endif

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
namespace Syncfusion.Pdf.Barcode
#endif
{
# if !XAML && !GDI
    public class PdfDataMatrixBarcode : PdfBidimensionalBarcode
#else
    public class DataMatrixBarcode : BidimensionalBarcode
#endif
    {
        # region Private Members
        /// <summary>
        /// Holds the encoding.
        /// </summary>
# if !XAML && !GDI
        private PdfDataMatrixEncoding m_dataMatrixEncoding;
#else
        private DataMatrixEncoding m_dataMatrixEncoding;
#endif

        /// <summary>
        /// Holds the data matrix size.
        /// </summary>
# if !XAML && !GDI
        private PdfDataMatrixSize m_size;
#else
        private DataMatrixSize m_size;
#endif

        /// <summary>
        /// Holds the final array.
        /// </summary>
        private byte[,] m_dataMatrixArray;

        /// <summary>
        /// Array containing all possible datamatrix symbol attributes.
        /// </summary>
        private PdfDataMatrixSymbolAttribute[] m_symbolAttributes;

        /// <summary>
        /// Holds the suitable symbol attribute based on input text.
        /// </summary>
        private PdfDataMatrixSymbolAttribute m_symbolAttribute;

        /// <summary>
        /// Holds log array.
        /// </summary>
        private int[] m_log;

        /// <summary>
        /// Holds ALog array.
        /// </summary>
        private int[] m_aLog;

        /// <summary>
        /// Internal variable for RS polynomial.
        /// </summary>
        private int[] m_rsPolynomial;
        # endregion

        # region Constructors
# if !XAML && !GDI
        /// <summary>
        /// Initializes PdfDataMatrixBarcode class.
        /// </summary>
        public PdfDataMatrixBarcode()
#else
        /// <summary>
        /// Initializes DataMatrixBarcode class.
        /// </summary>
        public DataMatrixBarcode()
#endif
            : base()
        {
            Initialize();
        }

# if !XAML && !GDI
        /// <summary>
        /// Initializes PdfDataMatrixBarcode class.
        /// </summary>
        /// <param name="text">Data to be converted as barcode.</param>
        public PdfDataMatrixBarcode(string text)
#else
        /// <summary>
        /// Initializes DataMatrixBarcode class.
        /// </summary>
        /// <param name="text">Data to be converted as barcode.</param>
        public DataMatrixBarcode(string text)
#endif
            : this()
        {
            Text = text;
        }

        # endregion

        # region Properties
        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
#if !XAML && !GDI
        public PdfDataMatrixEncoding Encoding
#else
        public DataMatrixEncoding Encoding
#endif
        {
            get
            {
                return m_dataMatrixEncoding;
            }
            set
            {
                m_dataMatrixEncoding = value;
            }
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
#if !XAML && !GDI
        public PdfDataMatrixSize Size
#else
        public DataMatrixSize Size
#endif
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
        /// Returns the actual number of rows (including quietzones).
        /// </summary>
        internal int ActualRows
        {
            get
            {
                return m_symbolAttribute.SymbolRow + (int)(QuietZone.Top + QuietZone.Bottom);
            }
        }

        /// <summary>
        /// Returns the actual number of columns (including quietzones).
        /// </summary>
        internal int ActualColumns
        {
            get
            {
                return m_symbolAttribute.SymbolColumn + (int)(QuietZone.Left + QuietZone.Right);
            }
        }

        # endregion

        # region Helpher Methods
        /// <summary>
        /// Initializes properties and calculation array for DataMatrix barcode.
        /// </summary>
        private void Initialize()
        {
            QuietZone.All = 1; // same on all four sides for DataMatrix Barcode.

#if!XAML && !GDI
            Encoding = PdfDataMatrixEncoding.Auto;
            Size = PdfDataMatrixSize.Auto;
            XDimension = 0.86f; // equal to 1 pixel. minimum dimension supported by barcode readers is (0.72f).
#else
            Encoding = DataMatrixEncoding.Auto;
            Size = DataMatrixSize.Auto;
            XDimension = 1;
#endif

            m_symbolAttributes = new PdfDataMatrixSymbolAttribute[]{
                new PdfDataMatrixSymbolAttribute(10, 10, 1, 1, 3, 5, 1, 3),
                new PdfDataMatrixSymbolAttribute(12, 12, 1, 1, 5, 7, 1, 5),
                new PdfDataMatrixSymbolAttribute(14, 14, 1, 1, 8, 10, 1, 8),
                new PdfDataMatrixSymbolAttribute(16, 16, 1, 1, 12, 12, 1, 12),
                new PdfDataMatrixSymbolAttribute(18, 18, 1, 1, 18, 14, 1, 18),
                new PdfDataMatrixSymbolAttribute(20, 20, 1, 1, 22, 18, 1, 22),
                new PdfDataMatrixSymbolAttribute(22, 22, 1, 1, 30, 20, 1, 30),
                new PdfDataMatrixSymbolAttribute(24, 24, 1, 1, 36, 24, 1, 36),
                new PdfDataMatrixSymbolAttribute(26, 26, 1, 1, 44, 28, 1, 44),
                new PdfDataMatrixSymbolAttribute(32, 32, 2, 2, 62, 36, 1, 62),
                new PdfDataMatrixSymbolAttribute(36, 36, 2, 2, 86, 42, 1, 86),
                new PdfDataMatrixSymbolAttribute(40, 40, 2, 2, 114, 48, 1, 114),
                new PdfDataMatrixSymbolAttribute(44, 44, 2, 2, 144, 56, 1, 144),
                new PdfDataMatrixSymbolAttribute(48, 48, 2, 2, 174, 68, 1, 174),
                new PdfDataMatrixSymbolAttribute(52, 52, 2, 2, 204, 84, 2, 102),
                new PdfDataMatrixSymbolAttribute(64, 64, 4, 4, 280, 112, 2, 140),
                new PdfDataMatrixSymbolAttribute(72, 72, 4, 4, 368, 144, 4, 92),
                new PdfDataMatrixSymbolAttribute(80, 80, 4, 4, 456, 192, 4, 114),
                new PdfDataMatrixSymbolAttribute(88, 88, 4, 4, 576, 224, 4, 144),
                new PdfDataMatrixSymbolAttribute(96, 96, 4, 4, 696, 272, 4, 174),
                new PdfDataMatrixSymbolAttribute(104, 104, 4, 4, 816, 336, 6, 136),
                new PdfDataMatrixSymbolAttribute(120, 120, 6, 6, 1050, 408, 6, 175),
                new PdfDataMatrixSymbolAttribute(132, 132, 6, 6, 1304, 496, 8, 163),
                new PdfDataMatrixSymbolAttribute(144, 144, 6, 6, 1558, 620, 10, 156),
                
                // Rectangle matrix
                new PdfDataMatrixSymbolAttribute(8, 18, 1, 1, 5, 7, 1, 5),
                new PdfDataMatrixSymbolAttribute(8, 32, 2, 1, 10, 11, 1, 10),
                new PdfDataMatrixSymbolAttribute(12, 26, 1, 1, 16, 14, 1, 16),
                new PdfDataMatrixSymbolAttribute(12, 36, 2, 1, 22, 18, 1, 22),
                new PdfDataMatrixSymbolAttribute(16, 36, 2, 1, 32, 24, 1, 32),
                new PdfDataMatrixSymbolAttribute(16, 48, 2, 1, 49, 28, 1, 49)
            };

            CreateLogArrays();
        }

        /// <summary>
        /// Create log and alog arrays.
        /// </summary>
        private void CreateLogArrays()
        {
            m_log = new int[256];
            m_aLog = new int[256];

            m_log[0] = -255;
            m_aLog[0] = 1;

            for (int i = 1; i <= 255; i++)
            {
                m_aLog[i] = m_aLog[i - 1] * 2;

                if (m_aLog[i] >= 256)
                    m_aLog[i] = m_aLog[i] ^ 301;

                m_log[m_aLog[i]] = i;
            }
        }

        /// <summary>
        /// Creates factors for polynomial based on the data.
        /// </summary>
        /// <param name="size">Number of correction codewords required.</param>
        private void CreateRSPolynomial(int size)
        {
            int i, k, index = 1;

            int[] temp = new int[size + 1];

            temp[0] = 1;
            for (i = 1; i <= size; i++)
            {
                temp[i] = 1;
                for (k = i - 1; k > 0; k--)
                {
                    if (temp[k] != 0)
                        temp[k] =
                            m_aLog[(m_log[temp[k]] + index) % 255];
                    temp[k] ^= temp[k - 1];
                }
                temp[0] = m_aLog[(m_log[temp[0]] + index) % 255];
                index++;
            }

            m_rsPolynomial = new int[size];

            // Remove the last element in the array.
            Array.Copy(temp, m_rsPolynomial, size);
        }

        # region MatrixCreation
        /// <summary>
        /// Create matrix.
        /// </summary>
        /// <param name="codeword">Input data matrix.</param>
        private void CreateMatrix(int[] codeword)
        {
            int x, y, NC, NR; int[] places;

            int W = m_symbolAttribute.SymbolColumn, H = m_symbolAttribute.SymbolRow;
            int FW = W / m_symbolAttribute.HoriDataRegion, FH = H / m_symbolAttribute.VertDataRegion;

            NC = W - 2 * (W / FW);
            NR = H - 2 * (H / FH);

            places = new int[NC * NR];
            ecc200placement(places, NR, NC);
            byte[] matrix = new byte[W * H];

            for (y = 0; y < H; y += FH)
            {
                for (x = 0; x < W; x++)
                    matrix[y * W + x] = 1;
                for (x = 0; x < W; x += 2)
                    matrix[(y + FH - 1) * W + x] = 1;
            }
            for (x = 0; x < W; x += FW)
            {
                for (y = 0; y < H; y++)
                    matrix[y * W + x] = 1;
                for (y = 0; y < H; y += 2)
                    matrix[y * W + x + FW - 1] = 1;
            }
            for (y = 0; y < NR; y++)
            {
                for (x = 0; x < NC; x++)
                {
                    int v = places[(NR - y - 1) * NC + x];
                    if (v == 1 || v > 7 && (codeword[(v >> 3) - 1] & (1 << (v & 7))) != 0)
                        matrix[(1 + y + 2 * (y / (FH - 2))) * W +
                            1 + x + 2 * (x / (FW - 2))] = 1;
                }
            }

            int _w = m_symbolAttribute.SymbolColumn, _h = m_symbolAttribute.SymbolRow;

            byte[,] tempArray = new byte[_w, _h];

            for (int x1 = 0; x1 < _w; x1++)
            {
                for (int y1 = 0; y1 < _h; y1++)
                {
                    tempArray[x1, y1] = matrix[_w * y1 + x1];
                }
            }

            // rotate the array left.
            byte[,] tempArray2 = new byte[_h, _w];

            for (int i = 0; i < _h; i++)
            {
                for (int j = 0; j < _w; j++)
                {
                    tempArray2[_h - 1 - i, j] = tempArray[j, i];
                }
            }

            AddQuiteZone(tempArray2);
        }

        private void ecc200placement(int[] array, int NR, int NC)
        {
            int r, c, p;
            // invalidate
            for (r = 0; r < NR; r++)
                for (c = 0; c < NC; c++)
                    array[r * NC + c] = 0;
            // start
            p = 1;
            r = 4;
            c = 0;
            do
            {
                // check corner
                if (r == NR && !(c != 0))
                    ecc200placementcornerA(array, NR, NC, p++);
                if ((r == NR - 2) && !(c != 0) && ((NC % 4) != 0))
                    ecc200placementcornerB(array, NR, NC, p++);
                if (r == NR - 2 && !(c != 0) && (NC % 8) == 4)
                    ecc200placementcornerC(array, NR, NC, p++);
                if (r == NR + 4 && c == 2 && !((NC % 8) != 0))
                    ecc200placementcornerD(array, NR, NC, p++);
                // up/right
                do
                {
                    if (r < NR && c >= 0 && !(array[r * NC + c] != 0))
                        ecc200placementblock(array, NR, NC, r, c, p++);
                    r -= 2;
                    c += 2;
                }
                while (r >= 0 && c < NC);
                r++;
                c += 3;
                // down/left
                do
                {
                    if (r >= 0 && c < NC && !(array[r * NC + c] != 0))
                        ecc200placementblock(array, NR, NC, r, c, p++);
                    r += 2;
                    c -= 2;
                }
                while (r < NR && c >= 0);
                r += 3;
                c++;
            }
            while (r < NR || c < NC);
            // unfilled corner
            if (!(array[NR * NC - 1] != 0))
                array[NR * NC - 1] = array[NR * NC - NC - 2] = 1;
        }

        private void ecc200placementcornerA(int[] array, int NR, int NC, int p)
        {
            ecc200placementbit(array, NR, NC, NR - 1, 0, p, (char)7);
            ecc200placementbit(array, NR, NC, NR - 1, 1, p, (char)6);
            ecc200placementbit(array, NR, NC, NR - 1, 2, p, (char)5);
            ecc200placementbit(array, NR, NC, 0, NC - 2, p, (char)4);
            ecc200placementbit(array, NR, NC, 0, NC - 1, p, (char)3);
            ecc200placementbit(array, NR, NC, 1, NC - 1, p, (char)2);
            ecc200placementbit(array, NR, NC, 2, NC - 1, p, (char)1);
            ecc200placementbit(array, NR, NC, 3, NC - 1, p, (char)0);
        }

        private void ecc200placementcornerB(int[] array, int NR, int NC, int p)
        {
            ecc200placementbit(array, NR, NC, NR - 3, 0, p, (char)7);
            ecc200placementbit(array, NR, NC, NR - 2, 0, p, (char)6);
            ecc200placementbit(array, NR, NC, NR - 1, 0, p, (char)5);
            ecc200placementbit(array, NR, NC, 0, NC - 4, p, (char)4);
            ecc200placementbit(array, NR, NC, 0, NC - 3, p, (char)3);
            ecc200placementbit(array, NR, NC, 0, NC - 2, p, (char)2);
            ecc200placementbit(array, NR, NC, 0, NC - 1, p, (char)1);
            ecc200placementbit(array, NR, NC, 1, NC - 1, p, (char)0);
        }

        private void ecc200placementcornerC(int[] array, int NR, int NC, int p)
        {
            ecc200placementbit(array, NR, NC, NR - 3, 0, p, (char)7);
            ecc200placementbit(array, NR, NC, NR - 2, 0, p, (char)6);
            ecc200placementbit(array, NR, NC, NR - 1, 0, p, (char)5);
            ecc200placementbit(array, NR, NC, 0, NC - 2, p, (char)4);
            ecc200placementbit(array, NR, NC, 0, NC - 1, p, (char)3);
            ecc200placementbit(array, NR, NC, 1, NC - 1, p, (char)2);
            ecc200placementbit(array, NR, NC, 2, NC - 1, p, (char)1);
            ecc200placementbit(array, NR, NC, 3, NC - 1, p, (char)0);
        }

        private void ecc200placementcornerD(int[] array, int NR, int NC, int p)
        {
            ecc200placementbit(array, NR, NC, NR - 1, 0, p, (char)7);
            ecc200placementbit(array, NR, NC, NR - 1, NC - 1, p, (char)6);
            ecc200placementbit(array, NR, NC, 0, NC - 3, p, (char)5);
            ecc200placementbit(array, NR, NC, 0, NC - 2, p, (char)4);
            ecc200placementbit(array, NR, NC, 0, NC - 1, p, (char)3);
            ecc200placementbit(array, NR, NC, 1, NC - 3, p, (char)2);
            ecc200placementbit(array, NR, NC, 1, NC - 2, p, (char)1);
            ecc200placementbit(array, NR, NC, 1, NC - 1, p, (char)0);
        }

        private void ecc200placementblock(int[] array, int NR, int NC, int r, int c, int p)
        {
            ecc200placementbit(array, NR, NC, r - 2, c - 2, p, (char)7);
            ecc200placementbit(array, NR, NC, r - 2, c - 1, p, (char)6);
            ecc200placementbit(array, NR, NC, r - 1, c - 2, p, (char)5);
            ecc200placementbit(array, NR, NC, r - 1, c - 1, p, (char)4);
            ecc200placementbit(array, NR, NC, r - 1, c - 0, p, (char)3);
            ecc200placementbit(array, NR, NC, r - 0, c - 2, p, (char)2);
            ecc200placementbit(array, NR, NC, r - 0, c - 1, p, (char)1);
            ecc200placementbit(array, NR, NC, r - 0, c - 0, p, (char)0);
        }

        private void ecc200placementbit(int[] array, int NR, int NC, int r, int c, int p, char b)
        {
            if (r < 0)
            {
                r += NR;
                c += 4 - ((NR + 4) % 8);
            }
            if (c < 0)
            {
                c += NC;
                r += 4 - ((NC + 4) % 8);
            }
            array[r * NC + c] = (p << 3) + b;
        }
        # endregion

        # endregion

        # region Implementation
        /// <summary>
        /// Builds data matrix.
        /// </summary>
        private void BuildDataMatrix()
        {
            int[] codeword = PrepareCodeword(GetData());

            CreateMatrix(codeword);
        }

        /// <summary>
        /// Prepares data codeword by encoding and appending error correction codes.
        /// </summary>
        /// <param name="dataCodeword">Data in bytes.</param>
        /// <returns>Final codeword ready for generating matrix.</returns>
        private int[] PrepareCodeword(byte[] dataCodeword)
        {
            byte[] encodedCodeword = PrepareDataCodeword(dataCodeword);

            int[] correctCodeword = ComputeErrorCorrection(ref encodedCodeword);

            int[] finalCodeword = new int[encodedCodeword.Length + correctCodeword.Length];

            encodedCodeword.CopyTo(finalCodeword, 0);
            correctCodeword.CopyTo(finalCodeword, encodedCodeword.Length);

            return finalCodeword;
        }

        /// <summary>
        /// Encodes the data using Base256 encoder.
        /// </summary>
        /// <param name="dataCodeword">Data to be encoded.</param>
        /// <returns>Encoded data.</returns>
        private byte[] DataMatrixBaseEncoder(byte[] dataCodeword)
        {
            int num = 1;

            if (dataCodeword.Length > 249)
            {
                num++;
            }

            byte[] result = new byte[(1 + num) + dataCodeword.Length];

            result[0] = 231;

            if (dataCodeword.Length <= 249)
                result[1] = (byte)dataCodeword.Length;
            else
            {
                result[1] = (byte)((dataCodeword.Length / 250) + 249);
                result[2] = (byte)(dataCodeword.Length % 250);
            }
            Array.Copy(dataCodeword, 0, result, 1 + num, dataCodeword.Length);

            for (int i = 1; i < result.Length; i++)
                result[i] = ComputeBase256Codeword(result[i], i);

            return result;
        }

        /// <summary>
        /// Compute codeword using 255 state algorithm.
        /// </summary>
        /// <param name="val">Codeword to compute.</param>
        /// <param name="index">Index of the codeword.</param>
        /// <returns>Encoded codeword.</returns>
        private byte ComputeBase256Codeword(int val, int index)
        {
            int num = ((149 * (index + 1)) % 255) + 1;
            int num2 = val + num;

            if (num2 <= 255)
                return (byte)num2;

            return (byte)(num2 - 256);
        }

        /// <summary>
        /// Encodes the data using Numeric encoder.
        /// </summary>
        /// <param name="dataCodeword">Data to be encoded.</param>
        /// <returns>Encoded data.</returns>
        private byte[] DataMatrixASCIINumericEncoder(byte[] dataCodeword)
        {
            byte[] destinationArray = dataCodeword;

            // if the input data length is odd, add 0 in front of the data.
            if ((destinationArray.Length % 2) == 1)
            {
                destinationArray = new byte[dataCodeword.Length + 1];
                destinationArray[0] = 48;
                Array.Copy(dataCodeword, 0, destinationArray, 1, dataCodeword.Length);
            }

            byte[] result = new byte[destinationArray.Length / 2];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (byte)((((destinationArray[2 * i] - 48) * 10) + (destinationArray[(2 * i) + 1] - 48)) + 130);
            }
            return result;
        }

        /// <summary>
        /// Encodes the data using ASCII encoder.
        /// </summary>
        /// <param name="dataCodeword">Data to be encoded.</param>
        /// <returns>Encoded data.</returns>
        private byte[] DataMatrixASCIIEncoder(byte[] dataCodeword)
        {
            byte[] result;

            using (MemoryStream stream = new MemoryStream())
            {
                for (int i = 0; i < dataCodeword.Length; i++)
                {
                    if (dataCodeword[i] < 127)
                        stream.WriteByte((byte)(dataCodeword[i] + 1));
                    else
                    {
                        stream.WriteByte(235);
                        stream.WriteByte((byte)((dataCodeword[i] - 127)));
                    }
                }

                int length = (int)stream.Length;
                result = new byte[length];

#if!BARCODE_WINRT
                Array.Copy(stream.ToArray(), result, length);
#else
                Array.Copy(stream.ToArray(), result, length);
#endif
            }

            return result;
        }

        /// <summary>
        /// Compute error correction codewords.
        /// </summary>
        /// <param name="codeword">Data codewords.</param>
        /// <returns>Correction codeword array.</returns>
        private int[] ComputeErrorCorrection(ref byte[] codeword)
        {
            int dataLength = codeword.Length; // number of data code words.
            m_symbolAttribute = new PdfDataMatrixSymbolAttribute();

            // Find suitable symbol attribute and size.
#if!XAML && !GDI
            if (Size == PdfDataMatrixSize.Auto)
#else
            if (Size == DataMatrixSize.Auto)
#endif
            {
                foreach (PdfDataMatrixSymbolAttribute attr in m_symbolAttributes)
                {
                    if (attr.DataCodewords >= dataLength)
                    {
                        m_symbolAttribute = attr;
                        break;
                    }
                }
            }
            else
                m_symbolAttribute = m_symbolAttributes[(int)Size - 1];

            byte[] temp;

            // Pad data codeword if the length is less than the selected symbol attribute.
            if (m_symbolAttribute.DataCodewords > dataLength)
            {
                PadCodewords(m_symbolAttribute.DataCodewords, codeword, out temp);

                codeword = new byte[temp.Length];
                temp.CopyTo(codeword, 0);
                dataLength = codeword.Length;
            }
            else if (m_symbolAttribute.DataCodewords == 0)
            {
#if!XAML && !NETFX_CORE && !GDI && !WP
                throw new PdfBarcodeException("Data cannot be encoded as barcode");
#else
                throw new BarcodeException("Data cannot be encoded as barcode");
#endif
            }
            else if (m_symbolAttribute.DataCodewords < dataLength)
            {
                string r = m_symbolAttribute.SymbolRow.ToString();
                string c = m_symbolAttribute.SymbolColumn.ToString();
#if!XAML && !NETFX_CORE && !GDI && !WP
                throw new PdfBarcodeException(string.Format("Data too long for {0}x{1} barcode.", r, c));
#else
                throw new BarcodeException(string.Format("Data too long for {0}x{1} barcode.", r, c));
#endif
            }

            int k = m_symbolAttribute.CorrectionCodewords; //number of correction codeword.

            CreateRSPolynomial(k); // Updates m_rsPolynomial based on the required number of correction bytes.

            int t = 0; //temp

            // Create error correction array.
            int[] ctArray = new int[k];

            for (int i = 0; i < k; i++)
                ctArray[i] = 0;

            for (int i = 0; i < dataLength; i++)
            {
                t = ctArray[k - 1] ^ codeword[i];
                for (int j = k - 1; j > 0; j--)
                {
                    if (t != 0 && m_rsPolynomial[j] != 0)
                        ctArray[j] = (byte)(ctArray[j - 1] ^ m_aLog[(m_log[t] + m_log[m_rsPolynomial[j]]) % 255]);
                    else
                        ctArray[j] = ctArray[j - 1];
                }
                if (t != 0 && m_rsPolynomial[0] != 0)
                    ctArray[0] = (byte)m_aLog[(m_log[t] + m_log[m_rsPolynomial[0]]) % 255];
                else
                    ctArray[0] = 0;
            }

            Array.Reverse(ctArray);

            return ctArray;
        }

        /// <summary>
        /// Data codeword is padded to match the chosen symbol attribute.
        /// </summary>
        /// <param name="dataCWLength">Length of data codeword.</param>
        /// <param name="temp">Codeword without padding.</param>
        /// <param name="codeword">Codeword with padding.</param>
        private void PadCodewords(int dataCWLength, byte[] temp, out byte[] codeword)
        {
            int l = temp.Length;

            using (MemoryStream ms = new MemoryStream())
            {
                for (int i = 0; i < l; i++)
                    ms.WriteByte(temp[i]);

                if (l < dataCWLength)
                    ms.WriteByte(129); // denotes padding.

                l = (int)ms.Length;

                while (l < dataCWLength)
                {	// more padding
                    int v = 129 + (((l + 1) * 149) % 253) + 1;	// see Annex H
                    if (v > 254)
                        v -= 254;
                    ms.WriteByte((byte)v);

                    l = (int)ms.Length;
                }

                codeword = new byte[ms.Length];
#if!BARCODE_WINRT
                Array.Copy(ms.ToArray(), codeword, l);
#else
                Array.Copy(ms.ToArray(), codeword, l);
#endif
            }
        }

        /// <summary>
        /// Special field multiplication.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private int Mult(int a, int b)
        {
            return m_aLog[(m_log[a] + m_log[b]) % 255];
        }

        /// <summary>
        /// Choose suitable encoding.
        /// </summary>
        /// <param name="dataCodeword">Data codeword.</param>
        /// <returns>Encoded codeword.</returns>
        private byte[] PrepareDataCodeword(byte[] dataCodeword)
        {
            // Choose the suitable encoding.
#if !XAML && !GDI
            if (Encoding == PdfDataMatrixEncoding.Auto || Encoding == PdfDataMatrixEncoding.ASCIINumeric)
#else
            if (Encoding == DataMatrixEncoding.Auto || Encoding == DataMatrixEncoding.ASCIINumeric)
#endif
            {
                bool number = true;
                bool extended = false;
                int num = 0;
                byte[] data = dataCodeword;

#if !XAML && !GDI
                PdfDataMatrixEncoding encoding = PdfDataMatrixEncoding.ASCII;
#else
                DataMatrixEncoding encoding = DataMatrixEncoding.ASCII;
#endif

                for (int i = 0; i < data.Length; i++)
                {
                    if ((data[i] < 48) || (data[i] > 57))
                    {
                        number = false;
                    }
                    else if (data[i] > 127)
                    {
                        num++;
                        if (num > 3)
                        {
                            extended = true;
                            break;
                        }
                    }
                }
#if !XAML && !GDI
                if (number)
                    encoding = PdfDataMatrixEncoding.ASCIINumeric;
                if (extended)
                    encoding = PdfDataMatrixEncoding.Base256;

#if !NETFX_CORE && !WP
                if (Encoding == PdfDataMatrixEncoding.ASCIINumeric && Encoding != encoding)
                    throw new PdfBarcodeException("Data contains invalid characters and cannot be encoded as ASCIINumeric.");
#else
                if (Encoding == PdfDataMatrixEncoding.ASCIINumeric && Encoding != encoding)
                    throw new BarcodeException("Data contains invalid characters and cannot be encoded as ASCIINumeric.");

#endif
#else
                if (number)
                    encoding = DataMatrixEncoding.ASCIINumeric;
                if (extended)
                    encoding = DataMatrixEncoding.Base256;

                if (Encoding == DataMatrixEncoding.ASCIINumeric && Encoding != encoding)
                    throw new BarcodeException("Data contains invalid characters and cannot be encoded as ASCIINumeric.");
#endif
                Encoding = encoding;
            }

            byte[] result = null;
            switch (Encoding)
            {
#if !XAML && !GDI
                case PdfDataMatrixEncoding.ASCII:
                    result = DataMatrixASCIIEncoder(dataCodeword);
                    break;
                case PdfDataMatrixEncoding.ASCIINumeric:
                    result = DataMatrixASCIINumericEncoder(dataCodeword);
                    break;
                case PdfDataMatrixEncoding.Base256:
                    result = DataMatrixBaseEncoder(dataCodeword);
                    break;
#else
                case DataMatrixEncoding.ASCII:
                    result = DataMatrixASCIIEncoder(dataCodeword);
                    break;
                case DataMatrixEncoding.ASCIINumeric:
                    result = DataMatrixASCIINumericEncoder(dataCodeword);
                    break;
                case DataMatrixEncoding.Base256:
                    result = DataMatrixBaseEncoder(dataCodeword);
                    break;
#endif
            }

            return result;
        }

        /// <summary>
        /// Adds quietzone on all sides of the data matrix.
        /// </summary>
        /// <param name="tempArray2">Input data matrix.</param>
        private void AddQuiteZone(byte[,] tempArray2)
        {
            int w = ActualRows;
            int h = ActualColumns;
            int quietZone = (int)QuietZone.All;

            m_dataMatrixArray = new byte[w, h];

            // Top quietzone.
            for (int i = 0; i < h; i++)
            {
                m_dataMatrixArray[0, i] = 0;
            }

            for (int i = quietZone; i < w - quietZone; i++)
            {
                // Left quietzone.
                m_dataMatrixArray[i, 0] = 0;

                for (int j = quietZone; j < h - quietZone; j++)
                {
                    m_dataMatrixArray[i, j] = tempArray2[i - quietZone, j - quietZone];
                }

                // Right quietzone.
                m_dataMatrixArray[i, h - quietZone] = 0;
            }

            //Bottom quietzone.
            for (int i = 0; i < h; i++)
            {
                m_dataMatrixArray[w - quietZone, i] = 0;
            }
        }
        # endregion

        # region Public Methods

#if !XAML && !GDI
        /// <summary>
        /// Draws datamatrix in the PdfPage.
        /// </summary>
        /// <param name="page">PdfPage.</param>
        /// <param name="location">Location to draw barcode.</param>
        public override void Draw(PdfPageBase page, PointF location)
        {
            BuildDataMatrix();

            // Draw in PdfPage.
            PdfBrush blackBrush = PdfBrushes.Black;
            PdfBrush whiteBrush = PdfBrushes.White;

            float x = location.X;
            float y = location.Y;

            int w = ActualRows, h = ActualColumns;

            for (int i = 0; i < w; i++)
            {
                x = location.X;

                for (int j = 0; j < h; j++)
                {
                    PdfBrush colorBrush = null;
                    if (m_dataMatrixArray[(int)i, (int)j] == 1)
                        colorBrush = blackBrush;
                    else
                        colorBrush = whiteBrush;

                    page.Graphics.DrawRectangle(colorBrush, x, y, XDimension, XDimension);

                    x = x + XDimension;
                }

                y = y + XDimension;
            }
        }

        /// <summary>
        /// Converts barcode to image.
        /// </summary>
        /// <returns>Bitmap image of barcode.</returns>
#if !NETFX_CORE && !WP
        public override Image ToImage()
        {
            BuildDataMatrix();

            PdfUnitConvertor convertor = new PdfUnitConvertor();
            int dimension = (int)convertor.ConvertToPixels(XDimension, PdfGraphicsUnit.Point);

            int width = ActualColumns * dimension;
            int height = ActualRows * dimension;

            int x = 0, y = 0;

            Bitmap bmp = new Bitmap((int)width, (int)height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
            {
                Brush whiteBrush = Brushes.White;
                Brush blackBrush = Brushes.Black;

                int w = ActualRows, h = ActualColumns;

                for (int i = 0; i < w; i++)
                {
                    x = 0;
                    for (int j = 0; j < h; j++)
                    {
                        Brush solidBrush = null;
                        if (m_dataMatrixArray[i, j] == 1)
                            solidBrush = blackBrush;
                        else
                            solidBrush = whiteBrush;

                        g.FillRectangle(solidBrush, new Rectangle(x, y, dimension, dimension));

                        x = x + dimension;
                    }

                    y = y + dimension;
                }
            }

            return bmp;
        }
#endif
#elif GDI
        public override Image Draw(int angle)
        {
            BuildDataMatrix();

            int dimension = (int)XDimension;

            int width = ActualColumns * dimension;
            int height = ActualRows * dimension;

            int x = 0, y = 0;

            Bitmap bmp = null;
            if (angle == 90 || angle == 270)
                bmp = new Bitmap((int)height, (int)width, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            else
                bmp = new Bitmap((int)width, (int)height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
            {
                System.Drawing.Drawing2D.GraphicsState state = g.Save();
                if (angle == 90 || angle == 270)
                {
                    g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                    g.RotateTransform(angle);
                    g.TranslateTransform(-(float)bmp.Height / 2, -(float)bmp.Width / 2);
                }

                Brush whiteBrush = Brushes.White;
                Brush blackBrush = Brushes.Black;

                int w = ActualRows, h = ActualColumns;

                for (int i = 0; i < w; i++)
                {
                    x = 0;
                    for (int j = 0; j < h; j++)
                    {
                        Brush solidBrush = null;
                        if (m_dataMatrixArray[i, j] == 1)
                            solidBrush = blackBrush;
                        else
                            solidBrush = whiteBrush;

                        g.FillRectangle(solidBrush, new Rectangle(x, y, dimension, dimension));

                        x = x + dimension;
                    }

                    y = y + dimension;
                }

                if (state != null && (angle == 90 || angle == 270))
                    g.Restore(state);
            }

            return bmp;
        }
#if WINFORMS
        public SizeF Draw(Panel panel)
        {
            Graphics g = panel.CreateGraphics();
            g.Clear(Color.White);
            BuildDataMatrix();

            Brush blackBrush = Brushes.Black;
            Brush whiteBrush = Brushes.White;

            float x = 0;
            float y = 0;

            int w = ActualRows, h = ActualColumns;

            for (int i = 0; i < w; i++)
            {
                x = 0;

                for (int j = 0; j < h; j++)
                {
                    Brush colorBrush = null;
                    if (m_dataMatrixArray[(int)i, (int)j] == 1)
                        colorBrush = blackBrush;
                    else
                        colorBrush = whiteBrush;

                    g.FillRectangle(colorBrush, x, y, XDimension, XDimension);

                    x = x + XDimension;
                }

                y = y + XDimension;
            }
            return new SizeF(x, y);
        }
#endif
#else
        internal override void Draw(Canvas m_canvas)
        {
            BuildDataMatrix();

            Brush blackBrush = new SolidColorBrush(Colors.Black);
            Brush whiteBrush = new SolidColorBrush(Colors.White);

            float x = 0;
            float y = 0;

            int w = ActualRows, h = ActualColumns;

            for (int i = 0; i < w; i++)
            {
                x = 0;

                for (int j = 0; j < h; j++)
                {
                    Brush colorBrush = null;
                    if (m_dataMatrixArray[(int)i, (int)j] == 1)
                        colorBrush = blackBrush;
                    else
                        colorBrush = whiteBrush;

                    RectangleGeometry rectangleGeometry = new RectangleGeometry();

#if XAML && !BARCODE_WINRT
                    System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
#else
                    Windows.UI.Xaml.Shapes.Path path = new Windows.UI.Xaml.Shapes.Path();
#endif
                    path.Fill = colorBrush;
                    path.Stroke = colorBrush;
#if XAML && !BARCODE_WINRT
                    rectangleGeometry.Rect = new System.Windows.Rect(x, y, XDimension, XDimension);
#else
                    rectangleGeometry.Rect = new Rect(x, y, XDimension, XDimension);
#endif
                    path.Data = rectangleGeometry;

                    m_canvas.Children.Add(path);

                    x = x + XDimension;
                }

                y = y + XDimension;
            }

            m_canvas.Width = x;
            m_canvas.Height = y;
            //m_canvas.Background = blackBrush;
        }
#endif
        # endregion
    }

    /// <summary>
    /// Symbol attribute structure for the DataMatrix.
    /// </summary>
    struct PdfDataMatrixSymbolAttribute
    {
        internal readonly int SymbolRow;
        internal readonly int SymbolColumn;
        internal readonly int HoriDataRegion;
        internal readonly int VertDataRegion;
        internal readonly int DataCodewords;
        internal readonly int CorrectionCodewords;
        internal readonly int InterleavedBlock;
        internal readonly int InterleavedDataBlock;

        internal PdfDataMatrixSymbolAttribute(int m_SymbolRow, int m_SymbolColumn, int m_horiDataRegions, int m_vertDataRegions, int m_dataCodewords, int m_correctionCodewords, int m_interleavedBlock, int m_interleavedDataBlock)
        {
            SymbolRow = m_SymbolRow;
            SymbolColumn = m_SymbolColumn;
            HoriDataRegion = m_horiDataRegions;
            VertDataRegion = m_vertDataRegions;
            DataCodewords = m_dataCodewords;
            CorrectionCodewords = m_correctionCodewords;
            InterleavedBlock = m_interleavedBlock;
            InterleavedDataBlock = m_interleavedDataBlock;
        }
    }
}
#endif